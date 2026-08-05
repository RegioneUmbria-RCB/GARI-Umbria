Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Audit_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_Completamento_Checklists(
        ByVal ImpreseVisibili As String,
        ByVal listaChecklistVisibili As List(Of Integer),
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_R.Leggi_Widget()"
        Dim StrSQL As New System.Text.StringBuilder

        If listaChecklistVisibili.Count = 0 Then
            Return New DataTable
        End If

        Try
            StrSQL.AppendLine(" SELECT Audit_Tipo, Coltura, SUM(CASE WHEN ok = 0 THEN quantitaChecklist ELSE 0 END) As Non_Completate, ")
            StrSQL.AppendLine(" SUM(CASE WHEN ok = 1 THEN quantitaChecklist ELSE 0 END) As Completate FROM (")

            If listaChecklistVisibili.Contains(20) Then
                StrSQL.AppendLine(" Select Audit_Tipo, ok, MAX(x.Veg_Des) As Coltura, count(*) as quantitaChecklist FROM (  ")
                StrSQL.AppendLine("  Select Audit_Tipo, Regolamento_Cod,   ")
                StrSQL.AppendLine("  Case when psa.Stato_Cod IN (503,507,514) THEN 1 ELSE 0 END as ok, sv.Veg_Cod, sv.Veg_Des   ")
                StrSQL.AppendLine("  From Audit a  ")
                StrSQL.AppendLine("  Join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = a.Pratica_Cod  ")
                StrSQL.AppendLine("  Join WAnagraficaStati wa on wa.WAnagraficaStati_Cod = psa.Stato_Cod  ")
                StrSQL.AppendLine("  Left Join SpecieVegetali sv On sv.Veg_Cod = RIGHT(a.Riferimento_Cod, CHARINDEX('_', REVERSE(a.Riferimento_Cod)) - 1)  ")
                StrSQL.AppendLine("  where a.Audit_Tipo = 20 AND Regolamento_Cod = (  ")
                StrSQL.AppendLine("    Select MAX(aa.regolamento_cod) from Audit aa where aa.Audit_Tipo = a.Audit_Tipo  ")
                StrSQL.AppendLine(") ")

                If ImpreseVisibili <> "" Then
                    StrSQL.AppendLine(" AND a.Piva IN (" & Agro_SQL_Save_Clausola_IN(ImpreseVisibili, True) & ") ")
                End If
                StrSQL.AppendLine("  ) as x  ")

                If xFiltroAggiuntivo <> "" Then
                    StrSQL.AppendLine(" WHERE " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If

                StrSQL.AppendLine(" Group By Audit_Tipo, Regolamento_Cod, Veg_Cod, ok  ")

                If listaChecklistVisibili.Count > 1 Then
                    StrSQL.AppendLine(" UNION ALL ")
                End If

                listaChecklistVisibili.Remove(20)
            End If

            If listaChecklistVisibili.Count > 0 Then
                StrSQL.AppendLine("  Select Audit_Tipo, ok, NULL As Coltura, count(*) as quantitaChecklist FROM (  ")
                StrSQL.AppendLine("  Select Audit_Tipo, Regolamento_Cod,   ")
                StrSQL.AppendLine("  Case when psa.Stato_Cod IN (503,507,514) THEN 1 ELSE 0 END as ok ")
                StrSQL.AppendLine("  From Audit a  ")
                StrSQL.AppendLine("  Join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = a.Pratica_Cod  ")
                StrSQL.AppendLine("  Join WAnagraficaStati wa On wa.WAnagraficaStati_Cod = psa.Stato_Cod  ")
                StrSQL.AppendLine("  where a.Audit_Tipo IN (" + Agro_SQL_Save_Clausola_IN(String.Join(",", listaChecklistVisibili)) + ") And Regolamento_Cod = (  ")
                StrSQL.AppendLine("    Select MAX(aa.regolamento_cod) from Audit aa where aa.Audit_Tipo = a.Audit_Tipo  ")
                StrSQL.AppendLine(") ")

                If ImpreseVisibili <> "" Then
                    StrSQL.AppendLine(" AND a.Piva IN (" & Agro_SQL_Save_Clausola_IN(ImpreseVisibili, True) & ") ")
                End If

                StrSQL.AppendLine("  ) As x  ")

                If xFiltroAggiuntivo <> "" Then
                    StrSQL.AppendLine(" WHERE " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If

                StrSQL.AppendLine("  Group By Audit_Tipo, Regolamento_Cod, ok  ")
            End If

            StrSQL.AppendLine(" ) As totali  ")
            StrSQL.AppendLine(" Group By Audit_Tipo, Coltura ")

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            Return EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function Leggi(
        ByVal Audit_Cod As Int32,
        ByVal Audit_Tipo As Int32,
        ByVal Regolamento_Cod As Int32,
        ByVal Piva As String,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal workFlow As Boolean = False,
        Optional ByVal Riferimento_Cod As String = ""
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_R.Leggi()"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Audit ")
            If workFlow Then
                StrSQL.AppendLine(" LEFT JOIN Pratiche p on p.pratica_Cod = Audit.Pratica_Cod ")
                StrSQL.AppendLine(" LEFT JOIN Pratiche_Stati_Attuali psa on psa.pratica_Cod = Audit.Pratica_Cod ")
            End If
            StrSQL.AppendLine(" WHERE Audit.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   Audit.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND Audit.Audit_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Audit_Cod <> 0 Then
                StrSQL.AppendLine(" AND Audit.Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod) & "   ")
            End If

            If Audit_Tipo <> 0 Then
                StrSQL.AppendLine(" AND Audit.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & "   ")
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.AppendLine(" AND Audit.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & "   ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Audit.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Riferimento_Cod <> "" Then
                StrSQL.AppendLine(" AND Audit.Riferimento_Cod = '" & Agro_SQL_SaveText(Riferimento_Cod) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Audit.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Audit.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Audit.Audit_SuperUser, Audit.Piva, Audit.Audit_Tipo, Audit.Audit_Cod Asc ")
            End If

            Return EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function LeggiAuditTipi(
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_R.LeggiAuditTipi()"
        Dim StrSQL As New Text.StringBuilder
        Try
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Audit_Tipi ")
            StrSQL.AppendLine(" WHERE 1=1")
            return EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function LeggiServiziAuditChecklist(
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_R.LeggiServiziAudit()"
        Dim StrSQL As New Text.StringBuilder
        Try
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Servizi ")
            StrSQL.AppendLine(" WHERE Servizio_Cod >= 300 and Servizio_Cod <= 399")
            return EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function LeggiServiziAuditWorkflow(
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_R.LeggiServiziAuditWorkflow()"
        Dim StrSQL As New Text.StringBuilder
        Try
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Servizi ")
            StrSQL.AppendLine(" WHERE Servizio_Cod >= 200 and Servizio_Cod <= 299")
            return EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function LeggiAudit(ByVal Audit_Cod As Integer,
        ByVal Audit_Tipo As Integer,
        ByVal Regolamento_Cod As Integer,
        ByVal Piva As String,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal NonConformita As Boolean = False,
        Optional ByVal workFlow As Boolean = False
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_R.LeggiAudit()"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            If Audit_Tipo > 10 Then
                StrSQL.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#campi_codici_CC1279') IS NULL ")
                StrSQL.AppendLine("    BEGIN ")
                StrSQL.AppendLine("    DROP TABLE #campi_codici_CC1279 ")
                StrSQL.AppendLine(" END")
                StrSQL.AppendLine(" ")

                StrSQL.AppendLine(" SELECT ")
                StrSQL.AppendLine(" '3_'+Piva+'_'+CAST(SA_Cod AS VARCHAR)+'_'+CAST(Campo_Cod AS VARCHAR) riferimento_cod ")
                StrSQL.AppendLine(", id_cod ")
                StrSQL.AppendLine(", val_cod ")
                StrSQL.AppendLine(" INTO #campi_codici_CC1279")
                StrSQL.AppendLine(" FROM campi_codici ")
                StrSQL.AppendLine(" WHERE id_cod = 1279 ")
                StrSQL.AppendLine(" ")

                StrSQL.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#campi_codici_CC1326') IS NULL ")
                StrSQL.AppendLine("    BEGIN ")
                StrSQL.AppendLine("    DROP TABLE #campi_codici_CC1326 ")
                StrSQL.AppendLine(" END")
                StrSQL.AppendLine(" ")

                StrSQL.AppendLine(" SELECT ")
                StrSQL.AppendLine(" '3_'+Piva+'_'+CAST(SA_Cod AS VARCHAR)+'_'+CAST(Campo_Cod AS VARCHAR) riferimento_cod ")
                StrSQL.AppendLine(", id_cod ")
                StrSQL.AppendLine(", val_cod ")
                StrSQL.AppendLine(" INTO #campi_codici_CC1326")
                StrSQL.AppendLine(" FROM campi_codici ")
                StrSQL.AppendLine(" WHERE id_cod = 1326 ")
                StrSQL.AppendLine(" ")
            End If

            StrSQL.AppendLine(" SELECT DISTINCT Audit.*,Imprese.Rag_soc " & If(NonConformita, ",NC.ID_NC ", ""))

            If workFlow Then
                StrSQL.AppendLine(" ,p.Servizio_Cod ,psa.Stato_Cod ")
                StrSQL.AppendLine(" ,(select top 1 data_creazione from pratiche_stati where pratica_cod=Audit.pratica_cod and stato_cod=" & enum_AuditStatoWorkflow.PraticaProntaAutocontrollo & " order by data_creazione desc) as Data_Chiusura_Pratica ")
            Else
                StrSQL.AppendLine(" ,null AS Stato_Cod ")
            End If

            If Audit_Tipo > 10 Then
                StrSQL.AppendLine(" ,IC.val_cod AS Codice_Socio, IC1010.val_cod AS CUAA, IC1324.val_cod AS Contratto_Produzione ")
                StrSQL.AppendLine(" ,CC1279.val_cod AS Codice_Campo, CC1326.val_cod AS Filiera ")
                StrSQL.AppendLine(" ,i.piva AS Piva_OP, i.Rag_Soc AS Rag_Soc_OP ")
            End If

            StrSQL.AppendLine(" ,CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Audit.PIVA ELSE Imprese.partitaIvaReale END PivaReale")

            StrSQL.AppendLine(" FROM  Audit INNER Join Imprese ON Audit.Piva = Imprese.PIVA ")

            If workFlow Then
                StrSQL.AppendLine(" LEFT JOIN pratiche p ON p.pratica_Cod = Audit.pratica_Cod ")
                StrSQL.AppendLine(" LEFT JOIN pratiche_Stati_Attuali psa ON psa.pratica_Cod = Audit.pratica_Cod ")
            End If

            If NonConformita Then
                StrSQL.AppendLine(" LEFT JOIN NC_Testata NC ON NC.TipoNC_Chiave = Audit.Piva+'|'+CAST(Audit.Audit_Cod AS VARCHAR)+'|'+CAST(Audit.Audit_Tipo AS VARCHAR)+'|'+CAST(Audit.Regolamento_Cod AS VARCHAR) ")
            End If

            If Audit_Tipo > 10 Then
                StrSQL.AppendLine(" LEFT JOIN Imprese_Codici IC ON IC.Piva = Imprese.Piva AND IC.id_cod = 1033 ")
                StrSQL.AppendLine(" LEFT JOIN Imprese_Codici IC1010 ON IC1010.Piva = Imprese.Piva AND IC1010.id_cod = 1010 ")
                StrSQL.AppendLine(" LEFT JOIN Imprese_Codici IC1324 ON IC1324.Piva = Imprese.Piva AND IC1324.id_Cod = 1324 ")
                StrSQL.AppendLine(" LEFT JOIN #campi_codici_CC1279 CC1279 ON CC1279.riferimento_cod = Audit.Riferimento_Cod ")
                StrSQL.AppendLine(" LEFT JOIN #campi_codici_CC1326 CC1326 ON CC1326.riferimento_cod = Audit.Riferimento_Cod ")
                StrSQL.AppendLine(" LEFT JOIN GerarchiaImprese gi on gi.Figlio=Audit.Piva LEFT JOIN Imprese i on i.piva=gi.Padre LEFT JOIN Imprese_Codici ic1088 on ic1088.PIVA=Audit.Piva and ic1088.id_cod=1088 ")
            End If

            StrSQL.AppendLine(" WHERE Audit.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   Audit.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND Audit.Audit_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Audit_Cod <> 0 Then
                StrSQL.AppendLine(" AND Audit.Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod) & "   ")
            End If

            If Audit_Tipo <> 0 Then
                StrSQL.AppendLine(" AND Audit.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & "   ")
            End If

            If Regolamento_Cod <> 0 Then
                StrSQL.AppendLine(" AND Audit.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & "   ")
            End If

            ' filtro su visibilita imprese
            If Piva <> "" Then
                StrSQL.AppendLine(" AND Audit.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            Else
                Dim FiltroImprese As String = ""
                Dim UtentiVisibilitaLeggi As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim ImpreseVisibili As DataTable = UtentiVisibilitaLeggi.Leggi(1, "", "", objParametri)
                If Not ImpreseVisibili Is Nothing AndAlso ImpreseVisibili.Rows.Count > 0 Then
                    For i = 0 To ImpreseVisibili.Rows.Count - 1
                        FiltroImprese &= "'" & Agro_SQL_SaveText(ImpreseVisibili.Rows(i).Item("Piva")) & "',"
                    Next
                    If FiltroImprese <> "" Then
                        StrSQL.AppendLine(" AND Audit.Piva IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroImprese, FiltroImprese.Length - 1), True) & ") ")
                    End If
                End If
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Audit.Audit_SuperUser, Audit.Piva, Audit.Audit_Tipo, Audit.Audit_Cod Asc ")
            End If

            If LivelloCompatibilita(objParametri) >= 150 Then
                StrSQL.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ")
            End If

            If Audit_Tipo > 10 Then
                StrSQL.AppendLine(" ")
                StrSQL.AppendLine(" DROP TABLE #campi_codici_CC1279")
                StrSQL.AppendLine(" DROP TABLE #campi_codici_CC1326")
            End If

            return EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    'Include lo Stato_Des
    Public Function LeggiAudit2(
        ByVal Audit_Cod As Integer,
        ByVal Audit_Tipo As Integer,
        ByVal Regolamento_Cod As Integer,
        ByVal Piva As String,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal NC As Boolean = False,
        Optional ByVal workFlow As Boolean = False
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_R.LeggiAudit2()"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            StrSQL.AppendLine("SELECT Audit.*,Imprese.Rag_soc, Audit_Stati.Stato_Des ")
            StrSQL.AppendLine(" FROM   Audit  ")
            StrSQL.AppendLine(" INNER JOIN Imprese ON Audit.Piva = Imprese.PIVA ")
            StrSQL.AppendLine(" LEFT JOIN Audit_Stati ON Audit.Audit_Stato = Audit_Stati.Stato_Cod AND Audit.Audit_Tipo = Audit_Stati.Audit_Tipo ")
            StrSQL.AppendLine(" WHERE  Audit.Validita_Inizio >= " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine(" AND    Audit.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(" AND    Audit.Audit_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            StrSQL.AppendLine(" AND    Audit.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo))

            If Regolamento_Cod <> 0 Then
                StrSQL.AppendLine(" And Audit.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod))
            End If
            If Audit_Cod <> 0 Then
                StrSQL.AppendLine(" And Audit.Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod))
            End If
            If Piva <> "" Then
                StrSQL.AppendLine(" And Audit.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            return EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function


    Public Function LeggiAuditLabel(ByVal Audit_Tipo As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_R.LeggiAuditLabel()"
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable
        Try
            StrSQL.AppendLine(" SELECT * FROM Audit_Label ")
            StrSQL.AppendLine(" WHERE Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & "   ")
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT
    End Function

    Public Function LeggiAuditLabel(ByVal Audit_Tipo As Integer, ByVal Label As String, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_R.LeggiAuditLabel()"
        Dim StrSQL As New System.Text.StringBuilder
        Dim result As String = ""
        Try
            StrSQL.AppendLine(" SELECT * FROM Audit_Label ")
            StrSQL.AppendLine(" WHERE Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & "   ")
            StrSQL.AppendLine(" AND Label = '" & Agro_SQL_SaveText(Label) & "'")
            Dim DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

            If DT.Rows.Count > 0 Then
                result = DT.Rows(0).Item("Testo").ToString()
            End If
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
        End Try
        Return result
    End Function

End Class


Public Class Audit_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Audit_Copia(
        ByVal Audit_Cod_from As Integer,
        ByVal Audit_Cod_to As Integer,
        ByVal Audit_SuperUser_From As String,
        ByVal Piva_Origine As String,
        ByVal Piva_Destinazione As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal Audit_Tipo As Integer = 0,
        Optional ByVal Regolamento_Cod As Integer = 0
    ) As Boolean
        Dim messaggioErrore As String
        Dim DescrizioneFunzione As String = "Audit : Copia"
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_W.Audit_Copia()"
        Dim xRisp As Boolean
        Dim StrSQL As New System.Text.StringBuilder
        'Se non specificata la piva destinazione allora la copia si esegue sulla stessa azienda
        If Piva_Destinazione = "" Then
            Piva_Destinazione = Piva_Origine
        End If

        Try
            If Audit_Cod_from = 0 Or Audit_SuperUser_From = "" Or objParametri.UsernameOperazione = "" Then
                Throw New Exception("Audit_Cod = 0 Or Audit_SuperUser = "" Or UserName_Creazione = "" Or UserName_Modifica = "" Or MessaggioErrore <> """)
            End If

            '-------Aggiungo il nuovo record ad Audit-----------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" INSERT INTO [Audit]  ")
            'StrSQL.AppendLine(" select (select max(audit.Audit_Cod)+1 as MaxAuditCod from audit) as [Audit_Cod]  ")
            StrSQL.AppendLine(" select    " & Agro_SQL_SaveNum(Audit_Cod_to) & "  as [Audit_Cod]  ")
            StrSQL.AppendLine("           , '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' as [Audit_SuperUser]  ")
            StrSQL.AppendLine("           ,[Audit_Tipo]  ")
            StrSQL.AppendLine("           , '" & Agro_SQL_SaveText(Piva_Destinazione) & "'  as [Piva]  ")
            StrSQL.AppendLine("           ,null as[Audit_Responsabile]  ")
            StrSQL.AppendLine("           ,[Note]  ")
            StrSQL.AppendLine("           ,null as [inviato]  ")
            StrSQL.AppendLine("           ,null as[datainvio]  ")
            StrSQL.AppendLine("           ,[Validita_Inizio]  ")
            StrSQL.AppendLine("           ,[Validita_Fine]  ")
            StrSQL.AppendLine("           ,[Data_Creazione]  ")
            StrSQL.AppendLine("           ,[Data_Modifica]  ")
            StrSQL.AppendLine("           , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' as [Username_Creazione]  ")
            StrSQL.AppendLine("           , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' as [Username_Modifica]  ")
            StrSQL.AppendLine("           ,[DataLock]  ")
            StrSQL.AppendLine("           ,[Audit_Stato]  ")
            StrSQL.AppendLine("           ,[Regolamento_Cod]  ")
            StrSQL.AppendLine(" from audit  ")
            StrSQL.AppendLine(" where  ")
            StrSQL.AppendLine("           Audit_SuperUser = '" & Agro_SQL_SaveText(Audit_SuperUser_From) & "' ")
            StrSQL.AppendLine("           AND Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod_from) & " ")
            If Piva_Origine <> "" Then
                StrSQL.AppendLine("       AND Piva = '" & Agro_SQL_SaveText(Piva_Origine) & "'  ")
            End If

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

            'Verifico la presenza di errori
            If Not xRisp Then
                Throw New Exception("Modulo Condizionalita : " & DescrizioneFunzione & " : " & messaggioErrore)
                Return False
            End If

            '--------Copio i nuovi record nell'Audit_Risposte ------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" INSERT INTO [Audit_Risposte]  ")
            StrSQL.AppendLine(" SELECT " & Agro_SQL_SaveNum(Audit_Cod_to) & "   as [Audit_Cod] ")
            StrSQL.AppendLine("       ,[Audit_SuperUser] ")
            StrSQL.AppendLine("       ,[Disp_Cod] ")
            StrSQL.AppendLine("       ,[Punto_Numero] ")
            StrSQL.AppendLine("       ,[Valore] ")
            StrSQL.AppendLine("       ,null as [inviato] ")
            StrSQL.AppendLine("       ,null as [datainvio] ")
            StrSQL.AppendLine("       ,[Validita_Inizio] ")
            StrSQL.AppendLine("       ,[Validita_Fine] ")
            StrSQL.AppendLine("       ,[Data_Creazione] ")
            StrSQL.AppendLine("       ,[Data_Modifica] ")
            StrSQL.AppendLine("       , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' as [Username_Creazione]  ")
            StrSQL.AppendLine("       , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' as [Username_Modifica]  ")
            StrSQL.AppendLine("       ,[DataLock] ")
            StrSQL.AppendLine("       ,[Audit_Tipo] ")
            StrSQL.AppendLine("       ,[Regolamento_Cod] ")
            StrSQL.AppendLine("       ,[PropostaCorrettiva] ")
            StrSQL.AppendLine("   FROM [Audit_Risposte] ")
            StrSQL.AppendLine("   WHERE ")
            StrSQL.AppendLine("           [Audit_SuperUser] = '" & Agro_SQL_SaveText(Audit_SuperUser_From) & "' ")
            StrSQL.AppendLine("           AND [Audit_Cod] = " & Agro_SQL_SaveNum(Audit_Cod_from) & " ")

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

            'Verifico la presenza di errori
            If Not xRisp Then
                Throw New Exception("Modulo Condizionalita : " & DescrizioneFunzione & " : " & messaggioErrore)
                Return False
            End If
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        Finally
            StrSQL = Nothing
        End Try

        Return True
    End Function

    Public Function scrivi(
        ByVal Audit_Cod As Integer,
        ByVal Audit_Tipo As Integer,
        ByRef Audit_Responsabile As Integer,
        ByRef Audit_Stato As Integer,
        ByVal Regolamento_Cod As Integer,
        ByVal Piva As String,
        ByVal Note As String,
        ByVal Campionato As Integer,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByVal Data_creazione As Date,
        ByVal Data_modifica As Date,
        ByVal username_creazione As String,
        ByVal username_modifica As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal Riferimento_Cod As String = Nothing,
        Optional ByVal Profilo_Cod As Integer = 0,
        Optional ByVal Rintracciabilita As Integer = 0,
        Optional ByVal ExtraInfo As String = "",
        Optional ByVal Pratica_Cod As Integer = 0
    ) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_W.Scrivi()"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            StrSQL.AppendLine("INSERT INTO Audit( " & vbCrLf)
            StrSQL.AppendLine("   [Audit_Cod] " & vbCrLf)
            StrSQL.AppendLine("  ,[Audit_Tipo] " & vbCrLf)
            StrSQL.AppendLine("  ,[Audit_Responsabile] " & vbCrLf)
            StrSQL.AppendLine("  ,[Audit_Stato] " & vbCrLf)
            StrSQL.AppendLine("  ,[Regolamento_Cod] " & vbCrLf)
            StrSQL.AppendLine("  ,[Piva] " & vbCrLf)
            StrSQL.AppendLine("  ,[Note] " & vbCrLf)
            StrSQL.AppendLine("  ,[Audit_SuperUser] " & vbCrLf)
            StrSQL.AppendLine("  ,[Inviato] " & vbCrLf)
            StrSQL.AppendLine("  ,[DataInvio] " & vbCrLf)
            StrSQL.AppendLine("  ,[Data_Creazione] " & vbCrLf)
            StrSQL.AppendLine("  ,[Data_Modifica] " & vbCrLf)
            StrSQL.AppendLine("  ,[UserName_Creazione] " & vbCrLf)
            StrSQL.AppendLine("  ,[UserName_Modifica] " & vbCrLf)
            StrSQL.AppendLine("  ,[Validita_Inizio] " & vbCrLf)
            StrSQL.AppendLine("  ,[Validita_Fine] " & vbCrLf)
            StrSQL.AppendLine("  ,[Campionato] " & vbCrLf)
            StrSQL.AppendLine("  ,[Rintracciabilita] " & vbCrLf)
            StrSQL.AppendLine("  ,[ExtraInfo] " & vbCrLf)
            StrSQL.AppendLine("  ,[Pratica_Cod] " & vbCrLf)

            ' riferimento elemento anagrafico
            If Not IsNothing(Riferimento_Cod) Then
                StrSQL.AppendLine("  ,[Riferimento_Cod] " & vbCrLf)
            End If
            ' riferimento profilo azienda
            If Profilo_Cod <> 0 Then
                StrSQL.AppendLine("  ,[Profilo_Cod] " & vbCrLf)
            End If

            StrSQL.AppendLine("       ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine(" ")
            StrSQL.AppendLine(" " & Agro_SQL_SaveNum(Audit_Cod) & " " & vbCrLf)
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Audit_Tipo) & " " & vbCrLf)

            If IsNothing(Audit_Responsabile) Then
                StrSQL.AppendLine(", NULL " & vbCrLf)
            Else
                StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Audit_Responsabile) & " " & vbCrLf)
            End If
            If IsNothing(Audit_Stato) Then
                StrSQL.AppendLine(", NULL ")
            Else
                StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Audit_Stato) & " " & vbCrLf)
            End If

            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Regolamento_Cod) & " " & vbCrLf)
            StrSQL.AppendLine(",'" & Agro_SQL_SaveText(Piva) & "'" & vbCrLf)

            If Note Is Nothing Then
                StrSQL.AppendLine(", NULL ")
            Else
                StrSQL.AppendLine(",'" & Agro_SQL_SaveText(Note) & "'" & vbCrLf)
            End If

            StrSQL.AppendLine(",'" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'" & vbCrLf)
            StrSQL.AppendLine(", 0  ")
            StrSQL.AppendLine(", Null  ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine(",'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine(",'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            If IsNothing(Campionato) Then
                Campionato = 0
            End If

            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Campionato) & "  ")
            StrSQL.AppendLine(", " & Agro_SQL_SaveNum(Rintracciabilita) & "  ")
            StrSQL.AppendLine(",'" & Agro_SQL_SaveText(ExtraInfo) & "' ")
            StrSQL.AppendLine("," & Agro_SQL_SaveNum(Pratica_Cod) & " ")

            ' riferimento elemento anagrafico
            If Not IsNothing(Riferimento_Cod) Then
                StrSQL.AppendLine(",'" & Agro_SQL_SaveText(Riferimento_Cod) & "' ")
            End If
            ' riferimento profilo azienda
            If Profilo_Cod <> 0 Then
                StrSQL.AppendLine("," & Agro_SQL_SaveNum(Profilo_Cod) & " ")
            End If

            StrSQL.AppendLine(")")

            return EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function Cancellazione(
        ByVal Audit_Cod As Long,
        ByVal Audit_SuperUser As String,
        ByVal Piva As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Integer
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_W.Cancellazione()"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM  Audit ")
            StrSQL.AppendLine(" WHERE Audit_SuperUser = '" & Agro_SQL_SaveText(Audit_SuperUser) & "'")
            StrSQL.AppendLine(" AND Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod))
            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If
            Return EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

End Class