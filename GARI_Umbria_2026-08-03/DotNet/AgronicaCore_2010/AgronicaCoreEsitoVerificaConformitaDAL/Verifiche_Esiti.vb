Imports System.Globalization
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Verifiche_Esiti_R
    Inherits AgronicaCoreDataProvider.DataProvider

     Public Function LeggiStatoRichiesta(ByVal idTestata As Integer, ByRef objParametriServer As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine = "AgronicaCoreEsitoVerificaConformitaDAL.Verifiche_Esiti_R.LeggiRisultati"
        Dim sb As New StringBuilder
        sb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
        sb.AppendLine(" SELECT ")
        sb.AppendLine("         t.status, t.Errore")
        sb.AppendLine(" FROM")
        sb.AppendLine("         Verifica_Conformita_Testata t")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("         t.Id_Testata = " & Agro_SQL_SaveNum(idTestata))

        Try
            Dim dt = EseguiQuery_Lettura(objParametriServer, sb.ToString(), nomeRoutine)
            Return dt
        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, MessaggioErrore)
            Return Nothing
        End Try

    End Function

    Public Function LeggiRisultati(ByVal idTestata As Integer, ByRef objParametriServer As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine = "AgronicaCoreEsitoVerificaConformitaDAL.Verifiche_Esiti_R.LeggiRisultati"
        Dim sb As New StringBuilder
        sb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
        sb.AppendLine(" SELECT ")
        sb.AppendLine("         e.Id_Esito, r.Id_Agenda, e.Conforme, e.Err_Code, e.Dettagli")
        sb.AppendLine(" FROM")
        sb.AppendLine("         Verifica_Conformita_Testata t")
        sb.AppendLine(" JOIN ")
        sb.AppendLine("         Verifica_Conformita_Risultati r")
        sb.AppendLine("         ON r.Id_Testata = t.Id_Testata")
        sb.AppendLine(" JOIN ")
        sb.AppendLine("         Verifica_Conformita_Esiti e")
        sb.AppendLine("         ON e.Id_Testata = r.Id_Testata")
        sb.AppendLine("         AND e.Id_Risultato = r.Id_Risultato")
        sb.AppendLine(" WHERE ")
        sb.AppendLine("         t.Id_Testata = " & Agro_SQL_SaveNum(idTestata))
        sb.AppendLine($"         AND t.Status = {CInt(AnalysisRequestStatus.Completed)} ") 'mi assicuro che il processo di scrittura sia finito

        Try
            Dim dt = EseguiQuery_Lettura(objParametriServer, sb.ToString(), nomeRoutine)
            Return dt
        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, MessaggioErrore)
            Return Nothing
        End Try

    End Function

End Class
Public Class Verifiche_Esiti_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Sub ScriviEsiti(dtEsiti As List(Of DataRow), ByRef objParametri_Server As AgronicaCoreParametri)
        Dim stbInserimentoEsito As New StringBuilder
        Dim nomeRoutine As String = "AgronicaCoreEsitoVerificaConformitaDAL.Verifiche_Esiti_W.ScriviInserimentoEsiti()"

        If Not IsNothing(dtEsiti) AndAlso dtEsiti.Any Then
            Dim chunks = ChunkBy(dtEsiti, 100)
            For Each chunk In chunks
                stbInserimentoEsito.AppendLine(" insert into Verifica_Conformita_Esiti ( ")
                stbInserimentoEsito.AppendLine("  Id_Esito,  ")
                stbInserimentoEsito.AppendLine("  Id_Risultato,  ")
                stbInserimentoEsito.AppendLine("  Id_Testata,  ")
                stbInserimentoEsito.AppendLine("  Conforme, ")
                stbInserimentoEsito.AppendLine("  Err_Code, ")
                stbInserimentoEsito.AppendLine("  dettagli ")
                stbInserimentoEsito.AppendLine(" ) values ")

                For Each p As DataRow In chunk
                    Dim conforme As String = If(CBool(p.Item("Conforme")), "1", "0")

                    stbInserimentoEsito.AppendLine(String.Format("({0}, {1}, {2}, {3}, '{4}', '{5}'),",
                                                               p.Item("Id_Esito"), p.Item("Id_Risultato"), p.Item("Id_Testata"), conforme, p.Item("Err_Code"), p.Item("dettagli").ToString().Replace("'", "''")))
                Next
                Dim strSqlInsert As String = stbInserimentoEsito.ToString
                strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                stbInserimentoEsito.Clear()
                EseguiQuery_Scrittura(objParametri_Server, strSqlInsert, nomeRoutine)
            Next
        End If
    End Sub

    Public Sub ScriviEsitiMagazzino(dtEsitiMagazzino As List(Of DataRow), ByRef objParametri_Server As AgronicaCoreParametri)
        Dim stbInserimentoEsitoMagazzino As New StringBuilder
        Dim nomeRoutine As String = "AgronicaCoreEsitoVerificaConformitaDAL.Verifiche_Esiti_W.ScriviEsitiMagazzino()"

        If Not IsNothing(dtEsitiMagazzino) AndAlso dtEsitiMagazzino.Any Then
            Dim chunks = ChunkBy(dtEsitiMagazzino, 100)
            For Each chunk In chunks
                stbInserimentoEsitoMagazzino.AppendLine(" insert into Verifica_Conformita_Esiti_Magazzino ( ")
                stbInserimentoEsitoMagazzino.AppendLine("  Id_Esito_Magazzino,  ")
                stbInserimentoEsitoMagazzino.AppendLine("  Id_Testata,  ")
                stbInserimentoEsitoMagazzino.AppendLine("  Id_Risultato,  ")
                stbInserimentoEsitoMagazzino.AppendLine("  Piva_Magazzino,  ")
                stbInserimentoEsitoMagazzino.AppendLine("  Sa_Cod_Magazzino,  ")
                stbInserimentoEsitoMagazzino.AppendLine("  Fabbricato_Cod_Magazzino,  ")
                stbInserimentoEsitoMagazzino.AppendLine("  Lotto,  ")
                stbInserimentoEsitoMagazzino.AppendLine("  Conforme,  ")
                stbInserimentoEsitoMagazzino.AppendLine("  Id_Prodotto,  ")
                stbInserimentoEsitoMagazzino.AppendLine("  Categoria_Prodotto,  ")
                stbInserimentoEsitoMagazzino.AppendLine("  Qta_Scaricata,  ")
                stbInserimentoEsitoMagazzino.AppendLine("  Giacenza_Magazzino,  ")
                stbInserimentoEsitoMagazzino.AppendLine("  Unita_Misura,  ")
                stbInserimentoEsitoMagazzino.AppendLine("  Dettagli  ")
                stbInserimentoEsitoMagazzino.AppendLine(" ) values ")

                For Each p As DataRow In chunk
                    Dim conforme As String = If(CBool(p.Item("Conforme")), "1", "0")

                    stbInserimentoEsitoMagazzino.AppendLine(String.Format("({0}, {1}, {2}, '{3}', {4}, {5}, '{6}', {7}, {8}, {9}, {10}, {11}, '{12}', '{13}'),",
                                                               p.Item("Id_Esito_Magazzino"),
                                                               p.Item("Id_Testata"),
                                                               p.Item("Id_Risultato"),
                                                               Agro_SQL_SaveText(p.Item("Piva_Magazzino").ToString()),
                                                               p.Item("Sa_Cod_Magazzino"),
                                                               p.Item("Fabbricato_Cod_Magazzino"),
                                                               Agro_SQL_SaveText(p.Item("Lotto").ToString()),
                                                               conforme,
                                                               p.Item("Id_Prodotto"),
                                                               p.Item("Categoria_Prodotto"),
                                                               CDbl(p.Item("Qta_Scaricata")).ToString(Globalization.CultureInfo.InvariantCulture),
                                                               CDbl(p.Item("Giacenza_Magazzino")).ToString(Globalization.CultureInfo.InvariantCulture),
                                                               Agro_SQL_SaveText(p.Item("Unita_Misura").ToString()),
                                                               Agro_SQL_SaveText(p.Item("Dettagli").ToString())
                                                               ))
                Next
                Dim strSqlInsert As String = stbInserimentoEsitoMagazzino.ToString
                strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                stbInserimentoEsitoMagazzino.Clear()
                EseguiQuery_Scrittura(objParametri_Server, strSqlInsert, nomeRoutine)

                ' Aggiorna la conformità magazzino per questa batch di dati
                AggiornaConformitaMagazzinoPerChunk(chunk, objParametri_Server)
            Next
        End If
    End Sub

    Public Sub ScriviDettagliGiacenzeMagazzino(dtDettagliGiacenze As List(Of DataRow), ByRef objParametri_Server As AgronicaCoreParametri)
        Dim stbInserimentoDettagliGiacenze As New StringBuilder
        Dim nomeRoutine As String = "AgronicaCoreEsitoVerificaConformitaDAL.Verifiche_Esiti_W.ScriviDettagliGiacenzeMagazzino()"

        If Not IsNothing(dtDettagliGiacenze) AndAlso dtDettagliGiacenze.Any Then
            Dim chunks = ChunkBy(dtDettagliGiacenze, 100)
            For Each chunk In chunks
                stbInserimentoDettagliGiacenze.AppendLine(" insert into Verifica_Conformita_Esiti_Giacenze ( ")
                stbInserimentoDettagliGiacenze.AppendLine("  Id_Esito_Giacenza,  ")
                stbInserimentoDettagliGiacenze.AppendLine("  Id_Testata,  ")
                stbInserimentoDettagliGiacenze.AppendLine("  Piva_Magazzino,  ")
                stbInserimentoDettagliGiacenze.AppendLine("  Sa_Cod_Magazzino,  ")
                stbInserimentoDettagliGiacenze.AppendLine("  Fabbricato_Cod_Magazzino,  ")
                stbInserimentoDettagliGiacenze.AppendLine("  Lotto,  ")
                stbInserimentoDettagliGiacenze.AppendLine("  Conforme,  ")
                stbInserimentoDettagliGiacenze.AppendLine("  Id_Prodotto,  ")
                stbInserimentoDettagliGiacenze.AppendLine("  Categoria_Prodotto,  ")
                stbInserimentoDettagliGiacenze.AppendLine("  Giacenza_Magazzino,  ")
                stbInserimentoDettagliGiacenze.AppendLine("  Unita_Misura,  ")
                stbInserimentoDettagliGiacenze.AppendLine("  Dettagli,  ")
                stbInserimentoDettagliGiacenze.AppendLine("  DataGiacenza,  ")
                stbInserimentoDettagliGiacenze.AppendLine("  TipoGiacenza  ")
                stbInserimentoDettagliGiacenze.AppendLine(" ) values ")

                For Each p As DataRow In chunk
                    Dim conforme As String = If(CInt(p.Item("Conforme")) = 1, "1", "0")

                    stbInserimentoDettagliGiacenze.AppendLine(String.Format("({0}, {1}, '{2}', {3}, {4}, '{5}', {6}, {7}, {8}, {9}, {10}, '{11}', {12}, {13}),",
                                                               p.Item("Id_Esito_Giacenza"),
                                                               p.Item("Id_Testata"),
                                                               Agro_SQL_SaveText(p.Item("Piva_Magazzino").ToString()),
                                                               p.Item("Sa_Cod_Magazzino"),
                                                               p.Item("Fabbricato_Cod_Magazzino"),
                                                               Agro_SQL_SaveText(p.Item("Lotto").ToString()),
                                                               conforme,
                                                               p.Item("Id_Prodotto"),
                                                               p.Item("Categoria_Prodotto"),
                                                               CDbl(p.Item("Giacenza_Magazzino")).ToString(Globalization.CultureInfo.InvariantCulture),
                                                               p.Item("Unita_Misura"),
                                                               Agro_SQL_SaveText(p.Item("Dettagli").ToString()),
                                                               Agro_SQL_SaveDate(CDate(p.Item("DataGiacenza"))),
                                                               p.Item("TipoGiacenza")
                                                               ))
                Next
                Dim strSqlInsert As String = stbInserimentoDettagliGiacenze.ToString
                strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                stbInserimentoDettagliGiacenze.Clear()
                EseguiQuery_Scrittura(objParametri_Server, strSqlInsert, nomeRoutine)
            Next
        End If
    End Sub

    Public Sub ScriviRisultati(dtVerifiche As List(Of DataRow), ByRef objParametri_Server As AgronicaCoreParametri)
        Dim stbInserimento As New StringBuilder
        Dim nomeRoutine As String = "AgronicaCoreEsitoVerificaConformitaDAL.Verifiche_Esiti_W.ScriviInserimentoVerifiche()"

        If Not IsNothing(dtVerifiche) AndAlso dtVerifiche.Any Then
            Dim chunks = ChunkBy(dtVerifiche, 100)
            For Each chunk In chunks
                stbInserimento.AppendLine(" insert into Verifica_Conformita_Risultati ( ")
                stbInserimento.AppendLine(" 	Id_Risultato,  ")
                stbInserimento.AppendLine(" 	Id_Testata,  ")
                stbInserimento.AppendLine(" 	piva, ")
                stbInserimento.AppendLine(" 	sa_cod, ")
                stbInserimento.AppendLine(" 	id_agenda,  ")
                stbInserimento.AppendLine(" 	Lav_Cod,  ")
                stbInserimento.AppendLine(" 	data_operazione,  ")
                stbInserimento.AppendLine(" 	Dpi_Cod, ")
                stbInserimento.AppendLine(" 	sup_trattata, ")
                stbInserimento.AppendLine(" 	conforme, ")
                stbInserimento.AppendLine(" 	conforme_Magazzino ")
                stbInserimento.AppendLine(" ) values ")

                For Each p As DataRow In chunk
                    Dim conformeMagazzino As Object = DBNull.Value
                    If p.Item("conforme_Magazzino") IsNot DBNull.Value Then
                        conformeMagazzino = If(CBool(p.Item("conforme_Magazzino")), 1, 0)
                    End If

                    stbInserimento.AppendLine(String.Format("({0}, {1}, '{2}', {3}, {4}, {5}, {6}, '{7}', {8}, {9}, {10}),",
                                                               Agro_SQL_SaveNum(p.Item("Id_Risultato")),
                                                               Agro_SQL_SaveNum(p.Item("Id_Testata")),
                                                               Agro_SQL_SaveText(p.Item("piva").ToString()),
                                                               Agro_SQL_SaveNum(p.Item("sa_cod")),
                                                               Agro_SQL_SaveNum(p.Item("id_agenda")),
                                                               Agro_SQL_SaveNum(p.Item("Lav_Cod")),
                                                               Agro_SQL_SaveDate(p.Item("data_operazione")),
                                                               Agro_SQL_SaveText(p.Item("Dpi_Cod").ToString()),
                                                               CDbl(p.Item("sup_trattata")).ToString(Globalization.CultureInfo.InvariantCulture),
                                                               If(CBool(p.Item("conforme")), 1, 0), If(conformeMagazzino Is DBNull.Value, "NULL", conformeMagazzino.ToString())
                                                               ))

                Next
                Dim strSqlInsert As String = stbInserimento.ToString
                strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                stbInserimento.Clear()
                EseguiQuery_Scrittura(objParametri_Server, strSqlInsert, nomeRoutine)
            Next
        End If
    End Sub

    Public Function ControllaScriviTestata(ByVal PIVA As String,
                                                    ByVal sa_Cod As String,
                                                    ByVal veg_Cod As Integer,
                                                    ByVal dpi As String,
                                                    ByVal data_Da As Date,
                                                    ByVal data_A As Date,
                                                    ByVal countOperazioni As Integer,
                                                    ByVal flagImpostazioni As String,
                                                    ByVal flagMagazzino As String,
                                                    ByVal flagIaf As String,
                                                    ByVal timeSpan As Double,
                                                    ByRef objParametri_Server As AgronicaCoreParametri,
                                                    Optional ByVal origine As Integer = 0) As Integer

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim stbTestata As New StringBuilder
        Dim idVerificataTestata As Integer = -1

        Dim nomeRoutine As String = "AgronicaCoreEsitoVerificaConformitaDAL.Verifiche_Esiti_W.ControllaScriviTestata()"
        Dim idTestataEsistente As Integer = -1

        Try
            Dim queryCheck As New StringBuilder
            queryCheck.AppendLine("SELECT id_testata FROM Verifica_Conformita_Testata ")
            queryCheck.AppendLine(" WHERE piva = '" & Agro_SQL_SaveText(PIVA) & "'")
            queryCheck.AppendLine(" AND sa_cod = " & Agro_SQL_SaveNum(sa_Cod))
            queryCheck.AppendLine(" AND veg_cod = " & Agro_SQL_SaveNum(veg_Cod))
            queryCheck.AppendLine(" AND dpi_Cod = '" & Agro_SQL_SaveText(dpi) & "'")
            queryCheck.AppendLine(" AND Intervallo_Inizio = " & Agro_SQL_SaveDate(data_Da))
            queryCheck.AppendLine(" AND Intervallo_Fine = " & Agro_SQL_SaveDate(data_A))
            queryCheck.AppendLine(" AND Flag_Solo_Controlli_Utente = " & If(CBool(flagImpostazioni), 1, 0))
            queryCheck.AppendLine(" AND Flag_Verifica_Magazzino = " & If(CBool(flagMagazzino), 1, 0))
            queryCheck.AppendLine(" AND Flag_Verifica_IAF = " & If(CBool(flagIaf), 1, 0))
            queryCheck.AppendLine(" AND Origine = 0")

            Dim dtCheck As DataTable = EseguiQuery_Lettura(objParametri_Server, queryCheck.ToString(), "Scrivi___InterventiEVerifiche")

            If dtCheck IsNot Nothing AndAlso dtCheck.Rows.Count > 0 Then
                ' Esiste già, prendo id_testata
                idTestataEsistente = CInt(dtCheck.Rows(0)("id_testata"))

                ' Cancella record collegati da Esiti, Risultati, Testata
                Dim deleteBuilder As New StringBuilder
                deleteBuilder.AppendLine(" DELETE FROM Verifica_Conformita_Esiti WHERE id_testata = " & Agro_SQL_SaveNum(idTestataEsistente))
                deleteBuilder.AppendLine(" DELETE FROM Verifica_Conformita_Risultati WHERE id_testata = " & Agro_SQL_SaveNum(idTestataEsistente))
                deleteBuilder.AppendLine(" DELETE FROM Verifica_Conformita_Testata WHERE id_testata = " & Agro_SQL_SaveNum(idTestataEsistente))

                Dim deletedOk As Boolean = EseguiQuery_Scrittura(objParametri_Server, deleteBuilder.ToString(), nomeRoutine)
            End If

            Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze
            idVerificataTestata = xAgrosequenze.NuovoId_Tabella("Verifica_Conformita_Testata", 0, 20000000, objParametri_Server)

            'salvataggio testata
            stbTestata.AppendLine(" insert into Verifica_Conformita_Testata ( ")
            stbTestata.AppendLine(" 	id_testata,  ")
            stbTestata.AppendLine(" 	piva,  ")
            stbTestata.AppendLine(" 	sa_cod,  ")
            stbTestata.AppendLine(" 	veg_cod,  ")
            stbTestata.AppendLine(" 	Intervallo_Inizio, ")
            stbTestata.AppendLine(" 	Intervallo_Fine, ")
            stbTestata.AppendLine(" 	Dpi_Cod,  ")
            stbTestata.AppendLine(" 	Numero_Operazioni_Verificate,  ")
            stbTestata.AppendLine(" 	Flag_Solo_Controlli_Utente, ")
            stbTestata.AppendLine(" 	Flag_Verifica_Magazzino, ")
            stbTestata.AppendLine(" 	Flag_Verifica_IAF, ")
            stbTestata.AppendLine(" 	Tempo_Impiegato_ss, ")
            stbTestata.AppendLine(" 	Origine ")
            stbTestata.AppendLine(" ) ")

            stbTestata.AppendLine(" values (  ")
            stbTestata.AppendLine(Agro_SQL_SaveNum(idVerificataTestata) + ", ")
            stbTestata.AppendLine("'" & Agro_SQL_SaveText(PIVA) + "', ")
            stbTestata.AppendLine(Agro_SQL_SaveNum(sa_Cod) + ", ")
            stbTestata.AppendLine(Agro_SQL_SaveNum(veg_Cod) + ", ")
            stbTestata.AppendLine(Agro_SQL_SaveDate(data_Da) + ", ")
            stbTestata.AppendLine(Agro_SQL_SaveDate(data_A) + ", ")
            stbTestata.AppendLine("'" & Agro_SQL_SaveText(dpi) + "', ")
            stbTestata.AppendLine(Agro_SQL_SaveNum(countOperazioni) + ", ")
            stbTestata.AppendLine(Agro_SQL_SaveNum(If(CBool(flagImpostazioni), 1, 0)) + ", ")
            stbTestata.AppendLine(Agro_SQL_SaveNum(If(CBool(flagMagazzino), 1, 0)) + ", ")
            stbTestata.AppendLine(Agro_SQL_SaveNum(If(CBool(flagIaf), 1, 0)) + ", ")
            stbTestata.AppendLine(Agro_SQL_SaveNum(timeSpan) + ",")
            stbTestata.AppendLine(Agro_SQL_SaveNum(origine) + ") ")

            Dim rVal As Boolean = EseguiQuery_Scrittura(objParametri_Server, stbTestata.ToString, nomeRoutine)

            Return idVerificataTestata
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function

    Private Sub AggiornaConformitaRisultato(idTestata As Integer, idRisultato As Integer, conformita As Boolean, ByRef objParametri_Server As AgronicaCoreParametri)
        Dim nomeRoutine As String = "AgronicaCoreEsitoVerificaConformitaDAL.Verifiche_Esiti_W.AggiornaConformitaRisultato()"
        Dim sb As New StringBuilder()

        Try
            sb.AppendLine(" UPDATE ")
            sb.AppendLine("      Verifica_Conformita_Risultati ")
            sb.AppendLine(" SET")
            sb.AppendLine("      conforme_Magazzino = " & If(conformita, "1", "0"))
            sb.AppendLine(" WHERE")
            sb.AppendLine("      Id_Testata = " & Agro_SQL_SaveNum(idTestata))
            sb.AppendLine("      AND Id_Risultato = " & Agro_SQL_SaveNum(idRisultato))

            EseguiQuery_Scrittura(objParametri_Server, sb.ToString(), nomeRoutine)

        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            ' Non lancia eccezione per non bloccare il processo principale
        End Try
    End Sub

    Private Sub AggiornaConformitaMagazzinoPerChunk(chunk As IEnumerable(Of DataRow), ByRef objParametri_Server As AgronicaCoreParametri)
        Dim nomeRoutine As String = "AgronicaCoreEsitoVerificaConformitaDAL.Verifiche_Esiti_W.AggiornaConformitaMagazzinoPerChunk()"

        Try
            ' Raggruppa per Id_Testata e Id_Risultato solo per questo chunk
            Dim gruppiRisultati = chunk.GroupBy(Function(row) New With {
                .IdTestata = CInt(row("Id_Testata")),
                .IdRisultato = CInt(row("Id_Risultato"))
            }).ToList()

            For Each gruppo In gruppiRisultati
                ' Verifica se tutti gli esiti del gruppo sono conformi
                Dim tuttiConformi As Boolean = gruppo.All(Function(row) CBool(row("Conforme")))

                ' Aggiorna solo se tutti sono conformi (solo da False a True, mai da True a False)
                If tuttiConformi Then
                    AggiornaConformitaMagazzinoSeNecessario(gruppo.Key.IdTestata, gruppo.Key.IdRisultato, True, objParametri_Server)
                End If
            Next

        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            ' Non lancia eccezione per non bloccare il processo principale
        End Try
    End Sub

    Private Sub AggiornaConformitaMagazzinoSeNecessario(idTestata As Integer, idRisultato As Integer, conformitaMagazzino As Boolean, ByRef objParametri_Server As AgronicaCoreParametri)
        Dim nomeRoutine As String = "AgronicaCoreEsitoVerificaConformitaDAL.Verifiche_Esiti_W.AggiornaConformitaMagazzinoSeNecessario()"

        Try
            ' Prima verifica se TUTTI gli esiti magazzino per questo Id_Risultato sono conformi
            Dim sbCheck As New StringBuilder()
            sbCheck.AppendLine(" SELECT COUNT(*) as Totale, ")
            sbCheck.AppendLine("        SUM(CASE WHEN Conforme = 1 THEN 1 ELSE 0 END) as Conformi ")
            sbCheck.AppendLine(" FROM Verifica_Conformita_Esiti_Magazzino ")
            sbCheck.AppendLine(" WHERE Id_Testata = " & Agro_SQL_SaveNum(idTestata))
            sbCheck.AppendLine("   AND Id_Risultato = " & Agro_SQL_SaveNum(idRisultato))

            Dim dtCheck As DataTable = EseguiQuery_Lettura(objParametri_Server, sbCheck.ToString(), nomeRoutine)

            If dtCheck IsNot Nothing AndAlso dtCheck.Rows.Count > 0 Then
                Dim totale As Integer = CInt(dtCheck.Rows(0)("Totale"))
                Dim conformi As Integer = CInt(dtCheck.Rows(0)("Conformi"))

                ' Aggiorna solo se TUTTI gli esiti sono conformi
                If totale > 0 AndAlso conformi = totale Then
                    Dim sbUpdate As New StringBuilder()
                    sbUpdate.AppendLine(" UPDATE Verifica_Conformita_Risultati ")
                    sbUpdate.AppendLine(" SET conforme_Magazzino = 1")
                    sbUpdate.AppendLine(" WHERE Id_Testata = " & Agro_SQL_SaveNum(idTestata))
                    sbUpdate.AppendLine("   AND Id_Risultato = " & Agro_SQL_SaveNum(idRisultato))

                    EseguiQuery_Scrittura(objParametri_Server, sbUpdate.ToString(), nomeRoutine)
                End If
            End If

        Catch ex As Exception
            Scrivi_LOG(objParametri_Server, nomeRoutine, ex.Message)
            ' Non lancia eccezione per non bloccare il processo principale
        End Try
    End Sub
End Class
