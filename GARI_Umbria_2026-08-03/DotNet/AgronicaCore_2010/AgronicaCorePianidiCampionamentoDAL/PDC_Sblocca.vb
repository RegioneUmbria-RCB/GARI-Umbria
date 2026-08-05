Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class PDC_Sblocca_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal ID_PDC_Testata As Integer,
                          ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Appezza As Integer,
                          ByVal Id_Reg As Integer,
                          ByVal CapitolatoCliente_Cod As Integer,
                          ByVal Analisi_Tipologia As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM PDC_Sblocca ")
            StrSQL.AppendLine(" WHERE PDC_Sblocca.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If CapitolatoCliente_Cod <> 0 Then
                StrSQL.AppendLine(" AND CapitolatoCliente_Cod = " & Agro_SQL_SaveNum(CapitolatoCliente_Cod))
            End If

            If Analisi_Tipologia <> 0 Then
                StrSQL.AppendLine(" AND Analisi_Tipologia = " & Agro_SQL_SaveNum(Analisi_Tipologia))
            End If


            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Id_Reg = " & Agro_SQL_SaveNum(Id_Reg))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

    Public Function Leggi_x_CancellaPerLfo(ByVal ID_PDC_Testata As Integer,
                               ByVal Analisi_Tipo As Integer,
                               ByVal ID_LFO_InStr As String,
                               ByRef objParametri As AgronicaCoreParametri
                               ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_R.Leggi_x_CancellaPerLfo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If ID_PDC_Testata = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID_PDC_Testata = 0)")
            End If

            If Analisi_Tipo = 0 Then
                Throw New Exception("Parametro non corretto nella query (Analisi_Tipo = 0)")
            End If

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT COUNT(*) ")
            StrSQL.AppendLine(" FROM PDC_Sblocca PDC_S ")
            StrSQL.AppendLine(" INNER JOIN PDC_Dettagli PDC_D ")
            StrSQL.AppendLine(" ON PDC_S.PivaSuperUser = PDC_D.PivaSuperUser AND PDC_S.ID_PDC_Testata = PDC_D.ID_PDC_Testata")
            StrSQL.AppendLine(" AND PDC_S.Piva = PDC_D.Piva AND PDC_S.Sa_Cod = PDC_D.Sa_Cod AND PDC_S.Appezza = PDC_D.Appezza AND PDC_S.Id_Reg = PDC_D.Id_Reg")
            StrSQL.AppendLine()
            StrSQL.AppendLine(" WHERE PDC_S.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND PDC_S.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            StrSQL.AppendLine(" AND PDC_S.Analisi_Tipologia = " & Agro_SQL_SaveNum(Analisi_Tipo))

            If ID_LFO_InStr <> "" Then
                StrSQL.AppendLine(" AND PDC_D.ID_LFO IN (" & Agro_SQL_Save_Clausola_IN(ID_LFO_InStr) & ") ")
            End If


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine).Rows(0)(0) = 0
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Leggi_x_MarketAccess(ByVal ID_PDC_Testata As Integer,
                                         ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Appezza As Integer,
                                         ByVal Id_Reg As Integer,
                                         ByVal CapitolatoCliente_Cod As Integer,
                                         ByVal Analisi_Tipologia As Integer,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_R.Leggi_x_MarketAccess()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT PDC_S.* ")
            StrSQL.AppendLine(" , PDC_T.PDC_Data_Istantanea ")
            StrSQL.AppendLine(" , ISNULL(PDC_C.ID_PDC_Campione, 0) AS ID_PDC_Campione ")
            StrSQL.AppendLine(" , ISNULL(PDC_C.Codice_Campione, '') AS Codice_Campione ")
            StrSQL.AppendLine(" , ISNULL(r.Sup_Imp, 0) AS Sup_Imp ")
            StrSQL.AppendLine(" , ISNULL(ipp.Produzione_Prevista, 0) AS Produzione_Prevista ")
            StrSQL.AppendLine(" FROM PDC_Sblocca PDC_S ")
            StrSQL.AppendLine(" INNER JOIN PDC_Testata PDC_T ON PDC_S.PivaSuperUser = PDC_T.PivaSuperUser AND PDC_S.ID_PDC_Testata = PDC_T.ID_PDC_Testata ")
            StrSQL.AppendLine(" INNER JOIN PDC_Dettagli PDC_D ON PDC_S.PivaSuperUser = PDC_D.PivaSuperUser AND PDC_S.ID_PDC_Testata = PDC_D.ID_PDC_Testata AND PDC_S.Piva = PDC_D.Piva AND PDC_S.Sa_Cod = PDC_D.Sa_Cod AND PDC_S.Appezza = PDC_D.Appezza AND PDC_S.Id_Reg = PDC_D.Id_Reg ")
            StrSQL.AppendLine(" INNER JOIN Reg_Impianti r ON PDC_S.Piva = r.Piva AND PDC_S.Sa_Cod = r.Sa_Cod AND PDC_S.Appezza = r.Appezza AND PDC_S.Id_Reg = r.Id_Reg ")
            StrSQL.AppendLine(" INNER JOIN Imprese_Progetti ipp ON PDC_S.Piva = ipp.Piva AND PDC_S.Sa_Cod = ipp.Sa_Cod AND PDC_S.Appezza = ipp.Appezza AND PDC_S.Id_Reg = ipp.Id_Reg ")
            StrSQL.AppendLine("        AND ipp.Validita_Inizio <= PDC_T.PDC_Data_Istantanea AND ipp.Validita_Fine >= PDC_T.PDC_Data_Istantanea ")
            StrSQL.AppendLine(" LEFT JOIN PDC_Analisi PDC_A ON PDC_D.PivaSuperUser = PDC_A.PivaSuperUser AND PDC_D.ID_PDC_Testata = PDC_A.ID_PDC_Testata AND PDC_D.ID_PDC_Dettagli = PDC_A.ID_PDC_Dettagli AND PDC_S.Analisi_Associata = PDC_A.Analisi_Testata_Cod ")
            StrSQL.AppendLine(" LEFT JOIN PDC_Campioni PDC_C ON PDC_A.PivaSuperUser = PDC_C.PivaSuperUser AND PDC_A.ID_PDC_Testata = PDC_C.ID_PDC_Testata AND PDC_A.ID_PDC_Dettagli = PDC_C.ID_PDC_Dettagli AND PDC_A.ID_PDC_Campione = PDC_C.ID_PDC_Campione ")

            StrSQL.AppendLine(" WHERE PDC_S.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_S.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If CapitolatoCliente_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_S.CapitolatoCliente_Cod = " & Agro_SQL_SaveNum(CapitolatoCliente_Cod))
            End If

            If Analisi_Tipologia <> 0 Then
                StrSQL.AppendLine(" AND PDC_S.Analisi_Tipologia = " & Agro_SQL_SaveNum(Analisi_Tipologia))
            End If


            If Piva <> "" Then
                StrSQL.AppendLine(" AND PDC_S.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_S.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND PDC_S.Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND PDC_S.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

    Public Function LeggiJoinDettagli(ByVal ID_PDC_Testata As Integer,
                                      ByVal ID_PDC_Dettagli As Integer,
                                      ByVal Piva As String,
                                      ByVal Sa_Cod As Integer,
                                      ByVal Appezza As Integer,
                                      ByVal Id_Reg As Integer,
                                      ByVal CapitolatoCliente_Cod As Integer,
                                      ByVal Analisi_Tipologia As Integer,
                                      ByVal Analisi_Associata As Integer,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_R.LeggiJoinDettagli()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT PDC_Sblocca.*, PDC_Dettagli.ID_PDC_Dettagli ")
            StrSQL.AppendLine(" FROM PDC_Sblocca ")
            StrSQL.AppendLine(" INNER JOIN PDC_Dettagli ON PDC_Sblocca.PivaSuperUser = PDC_Dettagli.PivaSuperUser AND PDC_Sblocca.ID_PDC_Testata = PDC_Dettagli.ID_PDC_Testata AND PDC_Sblocca.Piva = PDC_Dettagli.Piva AND PDC_Sblocca.Sa_Cod = PDC_Dettagli.Sa_Cod AND PDC_Sblocca.Appezza = PDC_Dettagli.Appezza AND PDC_Sblocca.Id_Reg = PDC_Dettagli.Id_Reg ")
            StrSQL.AppendLine(" WHERE PDC_Sblocca.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Sblocca.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If CapitolatoCliente_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Sblocca.CapitolatoCliente_Cod = " & Agro_SQL_SaveNum(CapitolatoCliente_Cod))
            End If

            If Analisi_Tipologia <> 0 Then
                StrSQL.AppendLine(" AND PDC_Sblocca.Analisi_Tipologia = " & Agro_SQL_SaveNum(Analisi_Tipologia))
            End If

            If Analisi_Associata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Sblocca.Analisi_Associata = " & Agro_SQL_SaveNum(Analisi_Associata))
            End If


            If Piva <> "" Then
                StrSQL.AppendLine(" AND PDC_Sblocca.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Sblocca.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND PDC_Sblocca.Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND PDC_Sblocca.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg))
            End If

            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

    Public Function ContaAnalisi(ByVal ID_PDC_Testata As Integer,
                                 ByVal Piva As String,
                                 ByVal Sa_Cod As Integer,
                                 ByVal Appezza As Integer,
                                 ByVal Id_Reg As Integer,
                                 ByVal Analisi_Tipologia As Integer,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_R.ContaAnalisi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT PivaSuperUser, ID_PDC_Testata, Piva, Sa_Cod, Appezza, Id_Reg, Analisi_Tipologia, Analisi_Associata, COUNT(*) as Conta ")
            StrSQL.AppendLine(" FROM PDC_Sblocca ")
            StrSQL.AppendLine(" WHERE PDC_Sblocca.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND (Analisi_Associata IS NOT NULL AND Analisi_Associata <> 0)")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If Analisi_Tipologia <> 0 Then
                StrSQL.AppendLine(" AND Analisi_Tipologia = " & Agro_SQL_SaveNum(Analisi_Tipologia))
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Id_Reg = " & Agro_SQL_SaveNum(Id_Reg))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StrSQL.AppendLine(" GROUP BY PivaSuperUser, ID_PDC_Testata, Piva, Sa_Cod, Appezza, Id_Reg, Analisi_Tipologia, Analisi_Associata ")

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

    Public Function Leggi_x_Sincro_Terremerse(ByVal ID_PDC_Testata As Integer,
                                              ByVal Capitolato_Cliente_cod As Integer,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Progetto_Cod, pdc_sblocca.*, PDC_Testata.PDC_Data_Istantanea   ")
            StrSQL.AppendLine(" FROM PDC_Sblocca ")
            StrSQL.AppendLine("    inner join PDC_Testata on PDC_Testata.Id_PDC_Testata = PDC_Sblocca.ID_PDC_Testata ")
            StrSQL.AppendLine("    inner join Imprese_Progetti on Imprese_Progetti.piva = pdc_sblocca.Piva  and Imprese_Progetti.Sa_Cod = PDC_Sblocca.Sa_Cod And Imprese_Progetti.Appezza = PDC_Sblocca.Appezza And Imprese_Progetti.Id_Reg = PDC_Sblocca.Id_Reg ")

            StrSQL.AppendLine(" WHERE PDC_Sblocca.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            StrSQL.AppendLine(" AND PDC_Sblocca.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Fine>=PDC_Testata.PDC_Data_Istantanea  ")
            StrSQL.AppendLine(" and Imprese_Progetti.Validita_Inizio <=PDC_Testata.PDC_Data_Istantanea ")
            StrSQL.AppendLine(" AND PDC_Sblocca.CapitolatoCliente_cod  = " & Agro_SQL_SaveNum(Capitolato_Cliente_cod) & " ")

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

Public Class PDC_Sblocca_W
    Inherits AgronicaCoreDataProvider.DataProvider

#Region "Scrivi"

    Public Function Scrivi_Tabella_Fast(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Appezza As Integer,
                                        ByVal Id_Reg As Integer,
                                        ByVal CapitolatoCliente_Cod As Integer,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_W.Scrivi_Tabella_Fast()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO PDC_Fast_Sblocca([Piva] , [Sa_Cod],   ")
            StrSQL.AppendLine("         [Appezza], [Id_Reg], [CapitolatoCliente_Cod] ")

            StrSQL.AppendLine("  ) ")

            StrSQL.AppendLine("  VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Appezza))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Reg))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(CapitolatoCliente_Cod))

            StrSQL.AppendLine(")")

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

    Public Function Scrivi(ByVal ID_PDC_Testata As Integer,
                           ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Appezza As Integer,
                           ByVal ID_Reg As Integer,
                           ByVal CapitolatoCliente_Cod As Integer,
                           ByVal Esito As Integer,
                           ByVal Note As String,
                           ByVal Analisi_Tipo As Integer,
                           ByVal Analisi_Associata As Integer,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
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

            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO PDC_Sblocca(PivaSuperUser , ID_PDC_Testata,   ")
            StrSQL.AppendLine("         Piva, Sa_Cod, Appezza, Id_Reg, ")
            StrSQL.AppendLine("         CapitolatoCliente_Cod , Esito , Note , data_sblocco, ")
            StrSQL.AppendLine("         Analisi_Tipologia, Analisi_Associata, ")

            StrSQL.AppendLine("         Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica")
            StrSQL.AppendLine("  ) ")

            StrSQL.AppendLine("  VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Testata))

            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Appezza))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_Reg))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(CapitolatoCliente_Cod))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Esito))
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Note) & "'")
            StrSQL.AppendLine("         , GETDATE() ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Tipo))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Associata))

            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica))
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(username_creazione) & "'")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(username_modifica) & "'")
            StrSQL.AppendLine(")")

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

    Public Function ScriviAutomaticamente(ByVal ID_PDC_Testata As Integer,
                                          ByVal Analisi_Tipo As Integer,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                          Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                          Optional ByVal username_creazione As String = "",
                                          Optional ByVal username_modifica As String = ""
                                          ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_W.ScriviAutomaticamente()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
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

            StrSQL.Length = 0

            StrSQL.AppendLine(";WITH ANA_VERIFICATE AS (")
            StrSQL.AppendLine(" SELECT PDC_A2.PivaSuperUser, PDC_A2.ID_PDC_Testata, PDC_A2.ID_PDC_Dettagli, PDC_D2.ID_LFO")
            StrSQL.AppendLine("      , PDC_A2.Analisi_Testata_Cod, PDC_A2.PDC_Stato_Analisi, PDC_A2.Data_Richiesta_Analisi, PDC_A2.Analisi_Tipologia_Tipo")
            StrSQL.AppendLine("      , ACC2.CapitolatoCliente_Cod, ACC2.Capitolato_Des, ACC2.Esito")
            StrSQL.AppendLine(" FROM PDC_Analisi PDC_A2")
            StrSQL.AppendLine(" INNER JOIN Analisi_Conformita_Capitolato_Cliente ACC2 ON PDC_A2.PivaSuperUser = ACC2.PivaSuperUser AND PDC_A2.Analisi_Testata_Cod = ACC2.Analisi_Testata_Cod")
            StrSQL.AppendLine(" INNER JOIN PDC_Dettagli PDC_D2 ON PDC_A2.PivaSuperUser = PDC_D2.PivaSuperUser AND PDC_A2.ID_PDC_Testata = PDC_D2.ID_PDC_Testata AND PDC_A2.ID_PDC_Dettagli = PDC_D2.ID_PDC_Dettagli")
            StrSQL.AppendLine(" WHERE PDC_A2.PDC_Stato_Analisi = " & enum_PDC_Stato_Analisi.Analizzata)
            StrSQL.AppendLine(")")

            StrSQL.AppendLine("INSERT INTO PDC_Sblocca ")
            StrSQL.AppendLine("   (PivaSuperUser , ID_PDC_Testata, Piva, Sa_Cod, Appezza, Id_Reg, ")
            StrSQL.AppendLine("    CapitolatoCliente_Cod , Esito , Note , data_sblocco, ")
            StrSQL.AppendLine("    Analisi_Tipologia, Analisi_Associata, ")
            StrSQL.AppendLine("    Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica) ")
            StrSQL.AppendLine()
            StrSQL.AppendLine("SELECT * FROM ( ")
            StrSQL.AppendLine("  SELECT ")
            StrSQL.AppendLine("     PDC_D.PivaSuperUser, PDC_D.ID_PDC_Testata, PDC_D.Piva, PDC_D.Sa_Cod, PDC_D.Appezza, PDC_D.Id_Reg")
            StrSQL.AppendLine("   , PDC_CCA.ID_CapitolatoPrivato")

            StrSQL.AppendLine("   , (")
            StrSQL.AppendLine("       -- c'è un'analisi nell'lfo che è positiva, allora tutto l'lfo è sbloccato")
            StrSQL.AppendLine("       ISNULL((SELECT TOP 1 ANA_VERIFICATE.Esito FROM ANA_VERIFICATE ")
            StrSQL.AppendLine("               WHERE ANA_VERIFICATE.PivaSuperUser = PDC_D.PivaSuperUser AND ANA_VERIFICATE.ID_PDC_Testata = PDC_D.ID_PDC_Testata AND ANA_VERIFICATE.ID_LFO = PDC_D.ID_LFO")
            StrSQL.AppendLine("               AND PDC_CCA.ID_CapitolatoPrivato = ANA_VERIFICATE.CapitolatoCliente_Cod")
            StrSQL.AppendLine("               AND ANA_VERIFICATE.Analisi_Tipologia_Tipo = " & Agro_SQL_SaveNum(Analisi_Tipo))
            StrSQL.AppendLine("               AND ANA_VERIFICATE.Esito = -1")
            StrSQL.AppendLine("               ), 0)")
            StrSQL.AppendLine("   ) AS Esito")

            StrSQL.AppendLine("   , '' AS Note, GETDATE() AS data_Sblocco")
            StrSQL.AppendLine("   , " & Agro_SQL_SaveNum(Analisi_Tipo) & " AS Analisi_Tipologia")

            StrSQL.AppendLine("   , (")
            StrSQL.AppendLine("       -- c'è un'analisi nell'lfo che è positiva, allora scelgo questa, altrimenti se ce n'è una negativa memorizzo l'associazione con quella")
            StrSQL.AppendLine("       ISNULL((")
            StrSQL.AppendLine("                   SELECT TOP 1 ANA_VERIFICATE.Analisi_Testata_Cod FROM ANA_VERIFICATE ")
            StrSQL.AppendLine("                   WHERE ANA_VERIFICATE.PivaSuperUser = PDC_D.PivaSuperUser AND ANA_VERIFICATE.ID_PDC_Testata = PDC_D.ID_PDC_Testata AND ANA_VERIFICATE.ID_LFO = PDC_D.ID_LFO")
            StrSQL.AppendLine("                   AND PDC_CCA.ID_CapitolatoPrivato = ANA_VERIFICATE.CapitolatoCliente_Cod")
            StrSQL.AppendLine("                   AND ANA_VERIFICATE.Analisi_Tipologia_Tipo = " & Agro_SQL_SaveNum(Analisi_Tipo))
            StrSQL.AppendLine("                   AND ANA_VERIFICATE.Esito = -1")
            StrSQL.AppendLine("                   ORDER BY ANA_VERIFICATE.Data_Richiesta_Analisi DESC")
            StrSQL.AppendLine("               ),")
            StrSQL.AppendLine("               (")
            StrSQL.AppendLine("                   SELECT TOP 1 ANA_VERIFICATE.Analisi_Testata_Cod FROM ANA_VERIFICATE ")
            StrSQL.AppendLine("                   WHERE ANA_VERIFICATE.PivaSuperUser = PDC_D.PivaSuperUser AND ANA_VERIFICATE.ID_PDC_Testata = PDC_D.ID_PDC_Testata AND ANA_VERIFICATE.ID_LFO = PDC_D.ID_LFO")
            StrSQL.AppendLine("                   AND PDC_CCA.ID_CapitolatoPrivato = ANA_VERIFICATE.CapitolatoCliente_Cod")
            StrSQL.AppendLine("                   AND ANA_VERIFICATE.Analisi_Tipologia_Tipo = " & Agro_SQL_SaveNum(Analisi_Tipo))
            StrSQL.AppendLine("                   AND ANA_VERIFICATE.Esito = 0")
            StrSQL.AppendLine("                   ORDER BY ANA_VERIFICATE.Data_Richiesta_Analisi DESC")
            StrSQL.AppendLine("               )")
            StrSQL.AppendLine("       )")
            StrSQL.AppendLine("   ) AS Analisi_Associata")

            StrSQL.AppendLine("   , " & Agro_SQL_SaveDateTime(Data_creazione) & " AS Data_Creazione")
            StrSQL.AppendLine("   , " & Agro_SQL_SaveDateTime(Data_modifica) & " AS Data_Modifica")
            StrSQL.AppendLine("   , '" & Agro_SQL_SaveText(username_creazione) & "' AS Username_Creazione")
            StrSQL.AppendLine("   , '" & Agro_SQL_SaveText(username_modifica) & "' AS Username_Modifica")

            StrSQL.AppendLine("  FROM PDC_Dettagli PDC_D")
            StrSQL.AppendLine("  INNER JOIN PDC_LFO ON PDC_D.PivaSuperUser = PDC_LFO.PivaSuperUser AND PDC_D.ID_PDC_Testata = PDC_LFO.ID_PDC_Testata AND PDC_D.ID_LFO = PDC_LFO.ID_LFO")

            StrSQL.AppendLine("  INNER JOIN (")
            If Analisi_Tipo <> enum_AnalisiTipo.Analisi_Merceologiche Then
                StrSQL.AppendLine("       SELECT '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' AS PivaSuperUser ")
                StrSQL.AppendLine("       , " & Agro_SQL_SaveNum(ID_PDC_Testata) & " AS ID_PDC_Testata ")
                StrSQL.AppendLine("       , 0 AS ID_CapitolatoPrivato, 'Limiti di Legge' AS Des_Capitolato_Privato ")
                StrSQL.AppendLine("       UNION")
            End If
            StrSQL.AppendLine("       SELECT PivaSuperUser, ID_PDC_Testata, ID_CapitolatoPrivato, Des_Capitolato_Privato FROM PDC_CapitolatiCliente_Attivi ")
            StrSQL.AppendLine("  ) PDC_CCA ON PDC_D.PivaSuperUser = PDC_CCA.PivaSuperUser AND PDC_D.ID_PDC_Testata = PDC_CCA.ID_PDC_Testata ")

            StrSQL.AppendLine("  WHERE PDC_D.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine("  AND PDC_D.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))

            StrSQL.AppendLine("  AND NOT EXISTS(SELECT 1 FROM PDC_Sblocca")
            StrSQL.AppendLine("               WHERE PivaSuperUser = PDC_D.PivaSuperUser AND ID_PDC_Testata = PDC_D.ID_PDC_Testata")
            StrSQL.AppendLine("               AND Piva = PDC_D.Piva AND Sa_Cod = PDC_D.Sa_Cod AND Appezza = PDC_D.Appezza AND Id_Reg = PDC_D.Id_Reg")
            StrSQL.AppendLine("               AND ID_CapitolatoPrivato = PDC_CCA.ID_CapitolatoPrivato")
            StrSQL.AppendLine("               AND Analisi_Tipologia = " & Agro_SQL_SaveNum(Analisi_Tipo) & ")")

            StrSQL.AppendLine(") AS t ")
            StrSQL.AppendLine("-- Lo scrivo solo dove ho trovato una analisi associata (nel blocco LFO) ")
            StrSQL.AppendLine("WHERE t.Analisi_Associata IS NOT NULL ")

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

#End Region

#Region "Modifica"

    Public Function UpdateMASSIVO_vacchiFalse(ByVal ID_PDC_Testata As Integer,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_W.UpdateMASSIVO_vacchiFalse()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            'StrSQL.AppendLine(" update PDC_Sblocca  ")
            'StrSQL.AppendLine("set Esito = 0 ")

            'StrSQL.AppendLine("from pdc_sblocca p inner join  ")
            'StrSQL.AppendLine("      (select PDC_Dettagli.piva,PDC_Dettagli.sa_cod,PDC_Dettagli.Appezza ,PDC_Dettagli.Id_Reg , PDC_CapitolatiCliente_Attivi.ID_CapitolatoPrivato  ")
            'StrSQL.AppendLine("      FROM            PDC_Dettagli INNER JOIN PDC_CapitolatiCliente_Attivi ON PDC_Dettagli.ID_PDC_Testata = PDC_CapitolatiCliente_Attivi.ID_PDC_Testata AND PDC_Dettagli.PivaSuperUser = PDC_CapitolatiCliente_Attivi.PivaSuperUser  ")
            'StrSQL.AppendLine("      WHERE(PDC_Dettagli.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata) & ") ")

            'StrSQL.AppendLine("      and not exists (select * from PDC_Fast_Sblocca where PDC_Fast_Sblocca.piva = PDC_Dettagli.Piva And PDC_Fast_Sblocca.Sa_Cod = PDC_Dettagli.Sa_Cod And PDC_Fast_Sblocca.Appezza = PDC_Dettagli.Appezza And PDC_Fast_Sblocca.Id_Reg = PDC_Dettagli.Id_Reg and PDC_Fast_Sblocca.CapitolatoCliente_Cod = PDC_CapitolatiCliente_Attivi.ID_CapitolatoPrivato )   ")
            'StrSQL.AppendLine("      and  exists (select * from PDC_Sblocca   ")
            'StrSQL.AppendLine("      WHERE PDC_Sblocca.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata) & "  ")
            'StrSQL.AppendLine("      And PDC_Sblocca.Piva = PDC_Dettagli.Piva And PDC_Sblocca.Sa_Cod = PDC_Dettagli.Sa_Cod And PDC_Sblocca.Appezza = PDC_Dettagli.Appezza And PDC_Sblocca.Id_Reg = PDC_Dettagli.Id_Reg And PDC_Sblocca.CapitolatoCliente_Cod = PDC_CapitolatiCliente_Attivi.ID_CapitolatoPrivato   ")
            'StrSQL.AppendLine("      and esito = -1  )   ")
            'StrSQL.AppendLine("      AND PDC_Dettagli.PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            'StrSQL.AppendLine("       ) pp on pp.Piva = p.piva and pp.sa_cod = p.Sa_Cod and pp.Appezza =p.Appezza and pp.Id_Reg =p.Id_Reg and pp.ID_CapitolatoPrivato= p.CapitolatoCliente_Cod    ")

            StrSQL.AppendLine(" update PDC_Sblocca  ")
            StrSQL.AppendLine("set Esito = 0 ")
            StrSQL.AppendLine(", data_sblocco =  GETDATE() ")
            StrSQL.AppendLine("   from PDC_Sblocca ps where ID_PDC_Testata =" & Agro_SQL_SaveNum(ID_PDC_Testata) & " ")
            StrSQL.AppendLine(" and esito = -1 ")
            StrSQL.AppendLine(" and not exists ( ")
            StrSQL.AppendLine(" select * from PDC_Fast_Sblocca pf where pf.Piva = ps.Piva and pf.Sa_Cod  = ps.sa_cod and pf.appezza = ps.Appezza and pf.Id_Reg  = ps.id_reg and pf.CapitolatoCliente_Cod = ps.CapitolatoCliente_Cod ) ")

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

    Public Function ModificaEsitoMASSIVO(ByVal ID_PDC_Testata As Integer,
                                         ByVal Esito As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_W.ModificaEsitoMASSIVO()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Sblocca SET ")
            StrSQL.AppendLine("   Esito   =  " & Agro_vb_SaveNum(Esito))
            StrSQL.AppendLine("   , data_sblocco =  GETDATE() ")

            StrSQL.AppendLine(" FROM PDC_Sblocca p JOIN PDC_Fast_Sblocca P_F  ON p.piva = p_f.piva and p.sa_cod = p_f.sa_cod and p.appezza = p_f.appezza and p.id_reg = p_f.id_reg and p.Capitolatocliente_cod = p_f.Capitolatocliente_cod ")

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")

            'If strQuery <> "" Then
            '    StrSQL.AppendLine(" AND not ( " & (strQuery) & " )   ")
            'End If

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

    Public Function ModificaPuntuale(ByVal ID_PDC_Testata As Integer,
                                     ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Appezza As Integer,
                                     ByVal Id_Reg As Integer,
                                     ByVal CapitolatoCliente_Cod As Integer,
                                     ByVal Analisi_Tipologia As Integer,
                                     ByRef objParametri As AgronicaCoreParametri,
                                     Optional ByVal Esito As Integer? = Nothing,
                                     Optional ByVal Note As String = Nothing,
                                     Optional ByVal Analisi_Associata As Integer? = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = ""
                                     ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_W.ModificaPuntuale()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_Modifica = #2/1/1900# Then
                Data_Modifica = Date.Now
            End If

            If Username_Modifica = "" Then
                Username_Modifica = objParametri.UsernameOperazione
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If ID_PDC_Testata = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID_PDC_Testata = 0)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE PDC_Sblocca ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")

            If Not IsNothing(Esito) Then
                strSql.AppendLine("   , Esito = " & Agro_vb_SaveNum(Esito) & " ")
            End If

            If Not IsNothing(Note) Then
                strSql.AppendLine("   , Note = '" & Agro_SQL_SaveText(Note) & "' ")
            End If

            strSql.AppendLine("   , data_sblocco =  GETDATE() ")

            If Not IsNothing(Analisi_Associata) Then
                strSql.AppendLine("   , Analisi_Associata = " & Agro_SQL_SaveNum(Analisi_Associata) & " ")
            End If

            '---------------------------------------------            

            strSql.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            strSql.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Piva   =  '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod   =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Appezza <> 0 Then
                strSql.AppendLine(" AND Appezza   =  " & Agro_SQL_SaveNum(Appezza) & " ")
            End If
            If Id_Reg <> 0 Then
                strSql.AppendLine(" AND Id_Reg   =  " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If CapitolatoCliente_Cod <> -1 Then
                strSql.AppendLine(" AND CapitolatoCliente_Cod   =  " & Agro_SQL_SaveNum(CapitolatoCliente_Cod) & " ")
            End If

            If Analisi_Tipologia <> 0 Then
                strSql.AppendLine(" AND Analisi_Tipologia   =  " & Agro_SQL_SaveNum(Analisi_Tipologia) & " ")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

#End Region

#Region "Cancellazione"

    Public Function Cancella_Tabella_Fast(ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_W.Cancella_Tabella_Fast()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE FROM PDC_Fast_Sblocca ")

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

    Public Function Cancella(ByVal ID_PDC_Testata As Integer,
                             ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Appezza As Integer,
                             ByVal Id_Reg As Integer,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE FROM PDC_Sblocca ")

            StrSQL.AppendLine("  WHERE ")
            StrSQL.AppendLine("         PivaSuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         AND ID_PDC_Testata =  " & Agro_SQL_SaveNum(ID_PDC_Testata))


            If Piva <> "" Then
                StrSQL.AppendLine("     AND Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine("     AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If
            If Appezza <> 0 Then
                StrSQL.AppendLine("     AND Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If
            If Id_Reg <> 0 Then
                StrSQL.AppendLine("     AND Id_Reg = " & Agro_SQL_SaveNum(Id_Reg))
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

    Public Function CancellaPerLfo(ByVal ID_PDC_Testata As Integer,
                                   ByVal Analisi_Tipo As Integer,
                                   ByVal ID_LFO_InStr As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_W.CancellaPerLfo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If ID_PDC_Testata = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID_PDC_Testata = 0)")
            End If

            If Analisi_Tipo = 0 Then
                Throw New Exception("Parametro non corretto nella query (Analisi_Tipo = 0)")
            End If

            StrSQL.Length = 0

            StrSQL.AppendLine("DELETE PDC_S ")
            StrSQL.AppendLine(" FROM PDC_Sblocca PDC_S ")
            StrSQL.AppendLine(" INNER JOIN PDC_Dettagli PDC_D ")
            StrSQL.AppendLine(" ON PDC_S.PivaSuperUser = PDC_D.PivaSuperUser AND PDC_S.ID_PDC_Testata = PDC_D.ID_PDC_Testata")
            StrSQL.AppendLine(" AND PDC_S.Piva = PDC_D.Piva AND PDC_S.Sa_Cod = PDC_D.Sa_Cod AND PDC_S.Appezza = PDC_D.Appezza AND PDC_S.Id_Reg = PDC_D.Id_Reg")
            StrSQL.AppendLine()
            StrSQL.AppendLine(" WHERE PDC_S.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND PDC_S.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            StrSQL.AppendLine(" AND PDC_S.Analisi_Tipologia = " & Agro_SQL_SaveNum(Analisi_Tipo))

            If ID_LFO_InStr <> "" Then
                StrSQL.AppendLine(" AND PDC_D.ID_LFO IN (" & Agro_SQL_Save_Clausola_IN(ID_LFO_InStr) & ") ")
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

    Public Function CancellaGenericoJoin(ByVal ID_PDC_Testata As Integer,
                                         ByVal ID_PDC_Dettagli As Integer,
                                         ByVal Analisi_Testata_Cod As Integer,
                                         ByVal CapitolatoCliente_Cod As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Sblocca_W.CancellaGenericoJoin()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If ID_PDC_Testata = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID_PDC_Testata = 0)")
            End If

            StrSQL.Length = 0

            StrSQL.AppendLine("DELETE PDC_S ")
            StrSQL.AppendLine(" FROM PDC_Sblocca PDC_S ")
            StrSQL.AppendLine(" INNER JOIN PDC_Dettagli PDC_D ")
            StrSQL.AppendLine(" ON PDC_S.PivaSuperUser = PDC_D.PivaSuperUser AND PDC_S.ID_PDC_Testata = PDC_D.ID_PDC_Testata")
            StrSQL.AppendLine(" AND PDC_S.Piva = PDC_D.Piva AND PDC_S.Sa_Cod = PDC_D.Sa_Cod AND PDC_S.Appezza = PDC_D.Appezza AND PDC_S.Id_Reg = PDC_D.Id_Reg")
            StrSQL.AppendLine()
            StrSQL.AppendLine(" WHERE PDC_S.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" AND PDC_S.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))

            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine(" AND PDC_D.ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If

            If Analisi_Testata_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_S.Analisi_Associata = " & Agro_SQL_SaveNum(Analisi_Testata_Cod))
            End If

            If CapitolatoCliente_Cod <> -99 Then
                StrSQL.AppendLine(" AND PDC_S.CapitolatoCliente_Cod = " & Agro_SQL_SaveNum(CapitolatoCliente_Cod))
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

#End Region

End Class
