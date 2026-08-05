Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class UMA_Richieste_Lavorazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal piva As String,
                          ByVal gruppo_colturale As String,
                          ByVal Programmazione_Cod As Integer,
                          ByVal Richiesta_Cod As Integer,
                          ByVal Richiesta_Dettaglio_Cod As Integer,
                          ByVal Selezione_Variabile As enumSelezioneVariabile,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal Regolamento_Cod As Integer = 0) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Lavorazioni.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT UMA_Richieste_Lavorazioni.Piva_SuperUser, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Piva, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Gruppo_Colturale_UMA, ")
            stb.AppendLine(" 	UMA_Macrousi.Macrouso_UMA_Des, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Richiesta_Cod, ")
            stb.AppendLine("  UMA_Richieste_Lavorazioni.Programmazione_Cod, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Richiesta_Dettaglio_Cod, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Lavorazione_UMA, ")
            stb.AppendLine(" 	UMA_Lavorazioni.Lav_UMA_Des, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Lavorazione_GIAS, ")
            stb.AppendLine(" 	Operazioni.Lav_Des, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Tipo_Carburante, ")
            stb.AppendLine(" 	Carburanti.Car_Des, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Superficie_Maggiorazione_Trasferimenti, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Nr_Lavorazioni_Previste, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Nr_Lavorazioni_Richieste, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Piu_Lavorazioni_Previste, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Piu_Raccolti_Previsti, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Fabbisogno_Calcolato, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Fabbisogno_Richiesto, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Fabbisogno_Assegnato, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Validita_Inizio, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Validita_Fine, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Inviato, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.DataInvio, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Data_Creazione, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Data_Modifica, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Username_Creazione, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Username_Modifica, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Totale_Superficie_UMA, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Zona_Pendenza_A_UMA, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Zona_Pendenza_B_UMA, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Zona_Tessitura_Normale_UMA, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Zona_Tessitura_Media_UMA, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Zona_Tessitura_Tenace_UMA, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Programmazione_Cod, ")
            stb.AppendLine(" 	ISNULL(UMA_Richieste_Lavorazioni.Id_Attivita, 0) as Attivita_Cod, ")
            stb.AppendLine(" 	ISNULL(Attivita.[Desc], '') as Attivita_Des, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Note_Compilatore, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Note_Approvatore, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Qta_Manuale, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.Mesi, ")
            stb.AppendLine(" 	ISNULL(c.Udm_Alternativa, '') as Udm_Alternativa, ")
            stb.AppendLine(" 	UMA_Richieste_Lavorazioni.CUAA_Terzista as CUAA, ")
            stb.AppendLine("  I.rag_soc, ") 'aggiunto da Gloria per estrapolare anche la ragione sociale del terzista
            stb.AppendLine("  UMA_Richieste_Lavorazioni.Regolamento_Cod ") 'aggiunto da Tommaso per estrapolare anche il regolamento_cod
            stb.AppendLine(" FROM UMA_Richieste_Lavorazioni ")
            stb.AppendLine(" JOIN UMA_Lavorazioni ON UMA_Richieste_Lavorazioni.Lavorazione_UMA = UMA_Lavorazioni.Lav_UMA_Cod ")
            stb.AppendLine(" JOIN Operazioni ON UMA_Richieste_Lavorazioni.Lavorazione_GIAS = Operazioni.Lav_Cod ")
            stb.AppendLine(" JOIN Carburanti ON UMA_Richieste_Lavorazioni.Tipo_Carburante = Carburanti.Car_Cod ")
            stb.AppendLine(" LEFT JOIN UMA_Macrousi ON UMA_Richieste_Lavorazioni.Gruppo_Colturale_UMA = UMA_Macrousi.Macrouso_UMA_Cod ")
            stb.AppendLine(" LEFT JOIN Attivita ON UMA_Richieste_Lavorazioni.Id_Attivita = Attivita.ID_Attivita ")
            stb.AppendLine(" LEFT JOIN UMA_Richieste_Testata t on t.Piva_SuperUser = UMA_Richieste_Lavorazioni.Piva_SuperUser")
            stb.AppendLine(" 											    AND t.Piva = UMA_Richieste_Lavorazioni.Piva ")
            stb.AppendLine(" 											    AND t.Richiesta_Cod = UMA_Richieste_Lavorazioni.Richiesta_Cod ")
            stb.AppendLine(" LEFT JOIN UMA_Configurazione_MacrousixLavorazioni c ON c.Macrouso_UMA_Cod = UMA_Richieste_Lavorazioni.Gruppo_Colturale_UMA ")
            stb.AppendLine(" 											    AND c.Regolamento_Cod IN (0, UMA_Richieste_Lavorazioni.Regolamento_Cod) ")
            stb.AppendLine(" 											    AND c.Lav_UMA_Cod = UMA_Richieste_Lavorazioni.Lavorazione_UMA ")
            stb.AppendLine(" 												AND c.Lav_Cod = UMA_Richieste_Lavorazioni.Lavorazione_GIAS ")
            stb.AppendLine(" 												AND c.Id_Attivita = UMA_Richieste_Lavorazioni.id_attivita ")
            stb.AppendLine(" 												AND c.Validita_Inizio <= t.Validita_Fine ")
            stb.AppendLine(" 												AND c.Validita_Fine >= t.Validita_Inizio ")
            stb.AppendLine(" LEFT JOIN Imprese_Codici IC ON IC.val_cod=UMA_Richieste_Lavorazioni.CUAA_Terzista ")   'aggiunto da Gloria per estrapolare anche la ragione sociale del terzista
            stb.AppendLine(" LEFT JOIN Imprese I on I.PIVA=IC.PIVA ")                                               'aggiunto da Gloria per estrapolare anche la ragione sociale del terzista
            stb.AppendLine(" WHERE 1 = 1 ")
            stb.AppendLine(" 	AND UMA_Richieste_Lavorazioni.Piva_SuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & " ")

            If piva <> "" Then
                stb.AppendLine(" 	AND UMA_Richieste_Lavorazioni.Piva = " & Agro_SQL_SaveText_NULL(piva) & " ")
            End If

            If gruppo_colturale <> "" Then
                stb.AppendLine(" 	AND UMA_Richieste_Lavorazioni.Gruppo_Colturale_UMA = " & Agro_SQL_SaveText_NULL(gruppo_colturale) & " ")
            End If

            If Richiesta_Cod <> 0 Then
                stb.AppendLine(" 	AND UMA_Richieste_Lavorazioni.Richiesta_Cod = " & Agro_SQL_SaveNum(Richiesta_Cod) & " ")
            End If

            If Programmazione_Cod <> 0 Then
                stb.AppendLine(" 	AND UMA_Richieste_Lavorazioni.Programmazione_Cod = " & Agro_SQL_SaveNum(Programmazione_Cod) & " ")
            End If

            If Richiesta_Dettaglio_Cod <> 0 Then
                stb.AppendLine(" 	AND UMA_Richieste_Lavorazioni.Richiesta_Dettaglio_Cod = " & Agro_SQL_SaveNum(Richiesta_Dettaglio_Cod) & " ")
            End If

            If Regolamento_Cod <> 0 Then
                stb.AppendLine(" 	AND UMA_Richieste_Lavorazioni.Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            stb.AppendLine(" ORDER BY UMA_Richieste_Lavorazioni.Data_Creazione DESC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Lavorazioni(ByVal piva As String,
                                      ByVal anno As Integer,
                                      ByVal avanzamento_richiesta As Integer,
                                      ByVal tipo_richiesta As Integer,
                                      ByVal gruppo_colturale_uma As String,
                                      ByVal lavorazione_uma As String,
                                      ByVal programmazione_cod As Integer,
                                      ByVal stato_pratica As Integer,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                      Optional lavorazione_Gias As Integer = 0,
                                      Optional xFiltroAggiuntivo As String = "") As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Lavorazioni.Leggi_Lavorazione()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT ")

            If lavorazione_Gias > 0 Then
                stb.AppendLine(" DISTINCT l.*, ucm.Coeff_acq_distr")
            Else
                stb.AppendLine(" l.* ")
            End If

            stb.AppendLine(" FROM UMA_Richieste_Lavorazioni l ")
            stb.AppendLine(" inner join UMA_Richieste_Testata t on t.Richiesta_Cod = l.Richiesta_Cod ")
            stb.AppendLine(" inner join Pratiche_Stati_Attuali psa On t.Pratica_Cod = psa.Pratica_Cod ")

            If lavorazione_Gias > 0 Then
                stb.AppendLine(" inner join UMA_Configurazione_MacrousixLavorazioni ucm ON ucm.Macrouso_UMA_Cod = l.Gruppo_Colturale_UMA ")
                stb.AppendLine("                                                       AND ucm.Regolamento_Cod IN (0, l.Regolamento_Cod) ")
                stb.AppendLine("                                                       AND ucm.Lav_UMA_Cod = l.Lavorazione_UMA ")
                stb.AppendLine("                                                       AND ucm.Lav_Cod = l.Lavorazione_GIAS ")
                stb.AppendLine("                                                       AND ucm.Id_Attivita = l.id_attivita ")
                stb.AppendLine("                                                       AND ucm.Validita_Inizio <= t.Validita_Fine ")
                stb.AppendLine("                                                       AND ucm.Validita_Fine >= t.Validita_Inizio ")
            End If

            stb.AppendLine(" WHERE 1 = 1 ")
            stb.AppendLine(" And l.Piva_SuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & " ")

            If piva <> "" Then
                stb.AppendLine(" And l.Piva = " & Agro_SQL_SaveText_NULL(piva) & " ")
            End If

            If anno <> 0 Then
                stb.AppendLine(" And year(t.validita_inizio) = " & Agro_SQL_SaveNum(anno) & " ")
            End If

            If avanzamento_richiesta <> 0 Then
                stb.AppendLine(" And t.avanzamento_richiesta = " & Agro_SQL_SaveNum(avanzamento_richiesta) & " ")
            End If

            If tipo_richiesta <> 1 Then
                stb.AppendLine(" And t.tipo_richiesta = " & Agro_SQL_SaveNum(tipo_richiesta) & " ")
            End If

            If gruppo_colturale_uma <> "" Then
                stb.AppendLine(" And l.Gruppo_Colturale_UMA = " & Agro_SQL_SaveText_NULL(gruppo_colturale_uma) & " ")
            End If

            If lavorazione_uma <> "" Then
                stb.AppendLine(" And l.Lavorazione_UMA = " & Agro_SQL_SaveText_NULL(lavorazione_uma) & " ")
            End If

            If programmazione_cod <> 0 Then
                stb.AppendLine(" And l.Programmazione_Cod = " & Agro_SQL_SaveNum(programmazione_cod) & " ")
            End If

            If stato_pratica <> 0 Then
                stb.AppendLine(" And psa.Stato_Cod = " & Agro_SQL_SaveNum(stato_pratica) & " ")
            End If

            If lavorazione_Gias > 0 Then
                stb.AppendLine(" And ucm.Lav_Cod = " & Agro_SQL_SaveNum(lavorazione_Gias) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            If lavorazione_Gias = 1 Then
                stb.AppendLine(" AND ucm.Coeff_acq_distr > 0 ")
            End If

            If lavorazione_Gias = 1 Then
                stb.AppendLine(" AND ucm.Coeff_acq_distr > 0 ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "]  " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Lavorazioni_Alternative(ByVal gruppo_colturale As Integer,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  Optional ByVal xFiltroAggiuntivo As String = "",
                                                  Optional ByVal xOrderBy As String = "",
                                                  Optional validitaInizio As Date = AGRODATAINIZIO,
                                                  Optional validitaFine As Date = AGRODATAFINE) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Lavorazioni.Leggi_Lavorazioni_Alternative()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT l.Lavorazione_UMA , umal.Lav_UMA_Des, l.Lavorazione_UMA_Alt,l.Regolamento_Cod ")
            stb.AppendLine(" FROM UMA_Lavorazioni_Alternative l ")
            stb.AppendLine(" JOIN UMA_Lavorazioni umal ON l.Lavorazione_UMA = umal.Lav_UMA_Cod ")
            stb.AppendLine(" WHERE Gruppo_Colturale_UMA = " & Agro_SQL_SaveNum(gruppo_colturale) & " ")

            If validitaInizio <> AGRODATAINIZIO OrElse validitaFine <> AGRODATAFINE Then
                stb.AppendLine(" AND l.validita_inizio <= " & Agro_SQL_SaveDate(validitaFine) & " ")
                stb.AppendLine(" AND l.validita_fine >= " & Agro_SQL_SaveDate(validitaInizio) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" And  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy) & " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "]  " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Lista_Lavorazioni_Multiple_Richiesta(ByVal richiestaCod As Integer,
                                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                               ) As List(Of DataRow)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Lavorazioni.Leggi_Lista_Lavorazioni_Multiple_Richiesta()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim listaLavorazioni = New List(Of DataRow)

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT DISTINCT c.Lav_UMA_Cod, c.Macrouso_UMA_Cod, c.Regolamento_Cod, c.N_Max_Operazioni ")
            stb.AppendLine(" FROM UMA_Richieste_Testata t ")
            stb.AppendLine(" JOIN UMA_Configurazione_MacrousixLavorazioni c ON c.Validita_Inizio <= t.Validita_Fine ")
            stb.AppendLine("                                               And c.Validita_Fine >= t.Validita_Inizio ")
            stb.AppendLine("                                               And c.N_Max_Operazioni > 1 ")

            stb.AppendLine(" WHERE t.Richiesta_Cod = " & Agro_SQL_SaveNum(richiestaCod))

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            listaLavorazioni = dt.AsEnumerable().ToList()
        End If

        Return listaLavorazioni

    End Function

    Public Function Leggi_Per_Controllo_Incrociato(ByVal piva As String,
                                                   ByVal programmazione_cod As Integer,
                                                   ByVal pivaChiamante As String,
                                                   ByVal gruppo_colturale As Integer,
                                                   ByVal anno As Integer,
                                                   ByVal xOrderBy As String,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                   Optional isTerzista As Boolean = False,
                                                   Optional ByVal richiesta_Cod As Integer = 0,
                                                   Optional ByVal lavorazioneUMA As Integer = 0,
                                                   Optional ByVal avanzamento As Integer = 1,
                                                   Optional ByVal integrativa As Boolean = False,
                                                   Optional ByVal soloContoTerzi As Boolean = False,
                                                   Optional ByVal Regolamento_Cod As Integer = 0
                                                   ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Lavorazioni.Leggi_Per_Controllo_Incrociato()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.AppendLine("SELECT cml.N_Max_Operazioni, ic.val_cod, p.numero, i.rag_soc, ul.Lav_UMA_Des,")
            stb.AppendLine("       l.Gruppo_Colturale_UMA, l.Regolamento_Cod, l.Lavorazione_UMA, l.Totale_Superficie_UMA,")
            stb.AppendLine("       l.Zona_Pendenza_A_UMA, l.Zona_Pendenza_B_UMA, l.Zona_Tessitura_Normale_UMA,")
            stb.AppendLine("       l.Zona_Tessitura_Media_UMA, l.Zona_Tessitura_Tenace_UMA, l.Validita_Inizio,")
            stb.AppendLine("       l.piva as piva_lavorazione, psa.Stato_Cod, l.Fabbisogno_Richiesto, l.Fabbisogno_Calcolato,")
            stb.AppendLine("       l.Fabbisogno_Assegnato, r.Totale_Superficie_UMA as Totale_Richiedibile,")
            stb.AppendLine("       tt.Programmazione_Des, um.Macrouso_UMA_Des, l.Data_Modifica")
            stb.AppendLine("FROM UMA_Richieste_Lavorazioni l")
            stb.AppendLine("INNER JOIN UMA_Richieste r ON r.Richiesta_Cod = l.Richiesta_Cod AND r.Gruppo_Colturale_UMA = l.Gruppo_Colturale_UMA AND l.Programmazione_Cod = r.Programmazione_Cod")
            stb.AppendLine("INNER JOIN UMA_Richieste_Testata t ON t.richiesta_cod = l.Richiesta_Cod")
            stb.AppendLine("INNER JOIN Pratiche p ON p.Pratica_Cod = t.Pratica_Cod")
            stb.AppendLine("INNER JOIN Pratiche_Stati_Attuali psa ON psa.Pratica_Cod = t.Pratica_Cod")
            stb.AppendLine("INNER JOIN Imprese i ON i.piva = t.piva")
            stb.AppendLine("INNER JOIN UMA_LAvorazioni ul ON l.Lavorazione_UMA = ul.Lav_UMA_Cod")
            stb.AppendLine("INNER JOIN imprese_codici ic ON ic.piva = t.piva AND ic.id_cod = 1010")
            stb.AppendLine("INNER JOIN UMA_Configurazione_MacrousixLavorazioni cml ON cml.Macrouso_UMA_Cod = l.Gruppo_Colturale_UMA AND cml.Lav_UMA_Cod = l.Lavorazione_UMA AND cml.Lav_Cod = l.Lavorazione_GIAS")
            stb.AppendLine("LEFT JOIN Programmazione_Testata tt ON tt.Programmazione_Cod = l.Programmazione_Cod")
            stb.AppendLine("LEFT JOIN UMA_Macrousi um ON l.Gruppo_Colturale_UMA = um.Macrouso_UMA_Cod")
            stb.AppendLine("WHERE psa.Stato_Cod NOT IN (2009, 2006)")
            stb.AppendLine("  AND l.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            stb.AppendLine("  AND t.Avanzamento_Richiesta = " & avanzamento)
            stb.AppendLine("  AND p.Anno = '" & Agro_SQL_SaveText(anno.ToString) & "' ")
            stb.AppendLine("  AND cml.Validita_Inizio < GETDATE()")
            stb.AppendLine("  AND cml.Validita_Fine > GETDATE()")
            stb.AppendLine("  AND (cml.Regolamento_Cod = l.Regolamento_Cod OR cml.Regolamento_Cod = 0)")
            stb.AppendLine("  AND t.Tipo_Richiesta = -1")
            stb.AppendLine("  AND r.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            stb.AppendLine("  AND EXISTS (SELECT 1 FROM UMA_Richieste_Lavorazioni ll WHERE ll.Richiesta_Cod = l.Richiesta_Cod)")

            If lavorazioneUMA <> 0 Then
                stb.AppendLine("  AND l.Lavorazione_UMA = " & lavorazioneUMA)
            End If

            If gruppo_colturale <> 0 Then
                stb.AppendLine("  AND l.Gruppo_Colturale_UMA = " & gruppo_colturale)
            End If

            If Regolamento_Cod <> 0 Then
                stb.AppendLine("  AND l.Regolamento_Cod = " & Regolamento_Cod)
            End If
            If avanzamento = 1 Then

                If isTerzista Then

                    stb.AppendLine(" AND t.Piva != " & IIf(pivaChiamante <> "", "'" & Agro_SQL_SaveText(pivaChiamante) & "'", "(Select piva From UMA_Richieste_Testata Where Richiesta_Cod = " & richiesta_Cod.ToString & ")") & " ")

                Else

                    stb.AppendLine(" AND t.Tipo_Richiesta = -1 ")

                End If

            Else

                If isTerzista Then

                    stb.AppendLine(" AND (t.Piva != " & IIf(pivaChiamante <> "", "'" & Agro_SQL_SaveText(pivaChiamante) & "'", "(Select piva From UMA_Richieste_Testata Where Richiesta_Cod = " & richiesta_Cod.ToString & ")") & " OR ")
                    stb.AppendLine(" NOT ( t.Piva = " & IIf(pivaChiamante <> "", "'" & Agro_SQL_SaveText(pivaChiamante) & "'", "(Select piva From UMA_Richieste_Testata Where Richiesta_Cod = " & richiesta_Cod.ToString & ")") & " AND t.Richiesta_Integrativa = " & If(integrativa, 1, 0).ToString & " )) ")

                Else

                    If soloContoTerzi Then

                        stb.AppendLine(" AND (t.Tipo_Richiesta = -1) ")

                    Else

                        stb.AppendLine(" AND (t.Tipo_Richiesta = -1 OR (t.Piva = '" & Agro_SQL_SaveText(piva) & "' AND t.Richiesta_Cod != " & richiesta_Cod & ")) ")

                    End If

                End If

            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy) & " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    'inserita Gloria per lettura da tabella UMA_Lavorazioni_Parziali
    Public Function Leggi_Lavorazioni_Parziali(ByVal piva As String,
                          ByVal Richiesta_Cod As Integer,
                          ByVal Lavorazione_Parziale_Cod As Integer,
                          ByVal Selezione_Variabile As enumSelezioneVariabile,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Lavorazioni.Leggi_Lavorazioni_Parziali()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT ULP.Piva_SuperUser, ")
            stb.AppendLine(" 	ULP.Piva, ")
            stb.AppendLine(" 	ULP.Lavorazione_UMA, ")
            stb.AppendLine(" 	ULP.Regolamento_Cod, ")
            stb.AppendLine(" 	ULP.Lavorazione_GIAS, ")
            stb.AppendLine(" 	ULP.Richiesta_Cod, ")
            stb.AppendLine(" 	ULP.Lavorazione_Parziale_Cod, ")
            stb.AppendLine(" 	ULP.Tipo_Carburante, ")
            stb.AppendLine(" 	C.Car_Des, ")
            stb.AppendLine(" 	ULP.Superficie_Maggiorazione_Trasferimenti, ")
            stb.AppendLine(" 	ULP.Nr_Lavorazioni_Previste, ")
            stb.AppendLine(" 	ULP.Nr_Lavorazioni_Richieste, ")
            stb.AppendLine(" 	ULP.Piu_Lavorazioni_Previste, ")
            stb.AppendLine(" 	ULP.Piu_Raccolti_Previsti, ")
            stb.AppendLine(" 	ULP.Fabbisogno_Calcolato, ")
            stb.AppendLine(" 	ULP.Fabbisogno_Richiesto, ")
            stb.AppendLine(" 	ULP.Fabbisogno_Assegnato, ")
            stb.AppendLine(" 	ULP.Validita_Inizio, ")
            stb.AppendLine(" 	ULP.Validita_Fine, ")
            stb.AppendLine(" 	ULP.Inviato, ")
            stb.AppendLine(" 	ULP.DataInvio, ")
            stb.AppendLine(" 	ULP.Data_Creazione, ")
            stb.AppendLine(" 	ULP.Data_Modifica, ")
            stb.AppendLine(" 	ULP.Username_Creazione, ")
            stb.AppendLine(" 	ULP.Username_Modifica, ")
            stb.AppendLine(" 	ULP.Totale_Superficie_UMA, ")
            stb.AppendLine(" 	ULP.Zona_Pendenza_A_UMA, ")
            stb.AppendLine(" 	ULP.Zona_Pendenza_B_UMA, ")
            stb.AppendLine(" 	ULP.Zona_Tessitura_Normale_UMA, ")
            stb.AppendLine(" 	ULP.Zona_Tessitura_Media_UMA, ")
            stb.AppendLine(" 	ULP.Zona_Tessitura_Tenace_UMA, ")
            stb.AppendLine(" 	UL.Lav_UMA_Des, ")
            'stb.AppendLine(" 	O.Lav_Des, ")
            stb.AppendLine(" 	'' as Udm_Alternativa ")

            stb.AppendLine(" FROM UMA_Lavorazioni_Parziali as ULP  ")
            stb.AppendLine(" JOIN UMA_Lavorazioni as UL ON ULP.Lavorazione_UMA = UL.Lav_UMA_Cod ")
            'stb.AppendLine(" JOIN Operazioni as O ON ULP.Lavorazione_GIAS = O.Lav_Cod ")
            stb.AppendLine(" JOIN Carburanti as C ON ULP.Tipo_Carburante = C.Car_Cod ")
            'stb.AppendLine(" LEFT JOIN UMA_Configurazione_MacrousixLavorazioni UCM ON UCM.Lav_UMA_Cod = ULP.Lavorazione_UMA")
            'stb.AppendLine(" 												AND UCM.Lav_Cod = ULP.Lavorazione_GIAS ")
            stb.AppendLine(" WHERE 1= 1 ")
            stb.AppendLine(" 	AND ULP.Piva_SuperUser = " & Agro_SQL_SaveText_NULL(objParametri.PivaSuperUser) & " ")

            If piva <> "" Then
                stb.AppendLine(" 	AND ULP.Piva = " & Agro_SQL_SaveText_NULL(piva) & " ")
            End If


            If Richiesta_Cod <> 0 Then
                stb.AppendLine(" 	AND ULP.Richiesta_Cod = " & Agro_SQL_SaveNum(Richiesta_Cod) & " ")
            End If

            If Lavorazione_Parziale_Cod <> 0 Then
                stb.AppendLine(" 	AND ULP.Lavorazione_Parziale_Cod = " & Agro_SQL_SaveNum(Lavorazione_Parziale_Cod) & " ")
            End If

            stb.AppendLine(" ORDER BY ULP.Data_Creazione DESC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    'fine lettura da UMA_lavorazioni_Parziali 



    Public Function Leggi_Lavorazioni_Testa(ByVal piva As String,
                                            ByVal programmazione_cod As Integer,
                                            ByVal pivaChiamante As String,
                                            ByVal gruppo_colturale As Integer,
                                            ByVal anno As Integer,
                                            ByVal xOrderBy As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional isTerzista As Boolean = False,
                                            Optional ByVal richiesta_Cod As Integer = 0,
                                            Optional ByVal lavorazioneUMA As Integer = 0,
                                            Optional ByVal Regolamento_Cod As Integer = 0
                                            ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUMADAL.UMA_Richieste_Lavorazioni.Leggi_Per_Controllo_Incrociato()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim strselect As String

        Try
            If (programmazione_cod <> 0) Then
                strselect = "ic.val_cod, p.numero, i.rag_soc, ul.Lav_UMA_Des, l.Gruppo_Colturale_UMA, l.Lavorazione_UMA, l.Totale_Superficie_UMA, l.Zona_Pendenza_A_UMA, l.Zona_Pendenza_B_UMA,  "
                strselect &= "l.Zona_Tessitura_Normale_UMA, l.Zona_Tessitura_Media_UMA, l.Zona_Tessitura_Tenace_UMA, l.Validita_Inizio"
            Else
                strselect = "l.Programmazione_Cod,tt.Programmazione_Des,l.Gruppo_Colturale_UMA, ic.id_cod, ic.val_cod,i.rag_soc,l.Totale_Superficie_UMA,UM.Macrouso_UMA_Des,um.Macrouso_UMA_Cod" ',l.Lavorazione_UMA,ul.Lav_UMA_Des,ul.Lav_UMA_Des,l.Richiesta_Dettaglio_Cod,
            End If
            stb.Length = 0

            'stb.AppendLine(" SELECT ic.val_cod, p.numero, i.rag_soc, ul.Lav_UMA_Des, l.Gruppo_Colturale_UMA, l.Lavorazione_UMA, l.Totale_Superficie_UMA, l.Zona_Pendenza_A_UMA, l.Zona_Pendenza_B_UMA,  ")
            'stb.AppendLine(" l.Zona_Tessitura_Normale_UMA, l.Zona_Tessitura_Media_UMA, l.Zona_Tessitura_Tenace_UMA, l.Validita_Inizio ")
            stb.AppendLine("SELECT " & strselect & "")
            stb.AppendLine(" from UMA_Richieste l")
            'stb.AppendLine(" from UMA_Richieste_Lavorazioni l")
            stb.AppendLine(" join UMA_Richieste_Testata t on t.richiesta_cod = l.Richiesta_Cod ")
            stb.AppendLine(" Join Pratiche p on p.Pratica_Cod = t.Pratica_Cod ")
            stb.AppendLine(" Join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = t.Pratica_Cod ")
            stb.AppendLine(" Join Imprese i on i.piva = t.piva ")
            'stb.AppendLine(" Join UMA_LAvorazioni ul on l.Lavorazione_UMA = ul.Lav_UMA_Cod ")
            stb.AppendLine(" join imprese_codici ic on ic.piva = t.piva and ic.id_cod = 1010 ")
            'If (programmazione_cod = 0) Then
            stb.AppendLine(" LEFT JOIN Programmazione_Testata tt on tt.Programmazione_Cod = l.Programmazione_Cod ")
            stb.AppendLine(" LEFT JOIN UMA_Macrousi um ON l.Gruppo_Colturale_UMA = um.Macrouso_UMA_Cod ")
            'End If
            stb.AppendLine(" where  psa.Stato_Cod <> 2009 " & IIf(lavorazioneUMA <> 0, "AND Lavorazione_UMA = " & lavorazioneUMA.ToString & " ", "") & IIf(programmazione_cod <> 0, " AND l.Programmazione_Cod = " & programmazione_cod.ToString & " ", "") & " AND l.Richiesta_Cod IN (Select r.Richiesta_Cod from UMA_Richieste r ")
            stb.AppendLine(" join UMA_Richieste_Testata t on t.Richiesta_Cod = r.Richiesta_Cod ")
            stb.AppendLine(" join pratiche p on t.pratica_cod = p.pratica_cod ")
            stb.AppendLine(" where t.Avanzamento_Richiesta = 1 AND p.Anno = " & IIf(anno <> 0, anno.ToString, "(Select Anno From Pratiche ppp JOIN UMA_Richieste_Testata ttt ON ttt.Pratica_Cod = ppp.Pratica_cod Where ttt.Richiesta_Cod = " & richiesta_Cod.ToString & ")") & " And ")

            If isTerzista Then
                stb.AppendLine(" t.Piva != " & IIf(pivaChiamante <> "", "'" & Agro_SQL_SaveText(pivaChiamante) & "'", "(Select piva From UMA_Richieste_Testata Where Richiesta_Cod = " & richiesta_Cod.ToString & ")") & " AND ")
            Else
                'stb.AppendLine(" t.Tipo_Richiesta = -1 AND ")
            End If

            If gruppo_colturale <> 0 Then
                stb.AppendLine(" l.Gruppo_Colturale_UMA = " & gruppo_colturale.ToString & " AND  ")
            End If

            If Regolamento_Cod <> 0 Then
                stb.AppendLine(" l.Regolamento_Cod = " & Regolamento_Cod.ToString & " AND  ")
            End If

            stb.AppendLine(" r.Piva = '" & Agro_SQL_SaveText(piva) & "' AND ")
            stb.AppendLine(" (Select COUNT(*) from UMA_Richieste_Lavorazioni ll where ll.Richiesta_Cod = l.Richiesta_Cod) > 0 ) ")

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy) & " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Controllo_Lavorazioni_Da_Terzista_Rendicontazioni(ByVal piva As String, ByVal anno As Integer,
                                                                      ByVal xOrderBy As String, ByVal xFiltroAggiuntivo As String,
                                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                      ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUMADAL.UMA_Richieste_Lavorazioni.Controllo_Lavorazioni_Da_Terzista_Rendicontazioni()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            Dim lsEscludiStati As Integer() = {
                enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo,
                enum_WAnagraficaStati.Verifica_Intermedia_Non_Superata,
                enum_WAnagraficaStati.Rinuncia
            }

            stb.Length = 0

            stb.AppendLine("SELECT DISTINCT i.Rag_Soc As Terzisti")
            stb.AppendLine(" from UMA_Richieste_Lavorazioni l")
            stb.AppendLine(" join UMA_Richieste_Testata t on t.richiesta_cod = l.Richiesta_Cod ")
            stb.AppendLine(" Join Pratiche p on p.Pratica_Cod = t.Pratica_Cod ")
            stb.AppendLine(" Join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = t.Pratica_Cod ")
            stb.AppendLine(" Join Imprese i on i.piva = t.piva ")
            stb.AppendLine(" where t.Avanzamento_Richiesta = 1 AND p.Anno = " & anno.ToString & " AND ")
            stb.AppendLine(" t.Tipo_Richiesta = -1 and l.Piva = '" & Agro_SQL_SaveText(piva) & "' AND ")
            stb.AppendLine(" psa.Stato_Cod NOT IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", lsEscludiStati)) & ") ")

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy) & " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Leggi_Elenco_Lavorazioni(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUMADAL.UMA_Richieste_Lavorazioni.Leggi_Elenco_Lavorazioni()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine("SELECT Lav_UMA_Cod, Lav_UMA_Des")
            stb.AppendLine(" from UMA_Lavorazioni l")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Public Function Leggi_Elenco_Colture(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUMADAL.UMA_Richieste_Lavorazioni.Leggi_Elenco_Colture()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine("SELECT Macrouso_UMA_Cod, Macrouso_UMA_Des")
            stb.AppendLine(" from UMA_Macrousi m")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
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


Public Class UMA_Richieste_Lavorazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Aggiorna_Lavorazioni(ByVal InsertArray As ArrayList,
                                         ByVal UpdateArray As ArrayList,
                                         ByVal RichiesteUpdateArray As ArrayList,
                                         ByVal DeleteArray As ArrayList,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Lavorazioni.Aggiorna_Lavorazioni()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each del As UMA_Richieste_Lavorazioni In DeleteArray
                    GiasContext.UMA_Richieste_Lavorazioni.Attach(del)
                    GiasContext.UMA_Richieste_Lavorazioni.Remove(del)
                Next

                For Each up As UMA_Richieste_Lavorazioni In UpdateArray
                    GiasContext.UMA_Richieste_Lavorazioni.Attach(up)
                    GiasContext.Entry(up).State = EntityState.Modified
                Next

                For Each upRichieste As UMA_Richieste In RichiesteUpdateArray
                    GiasContext.UMA_Richieste.Attach(upRichieste)
                    GiasContext.Entry(upRichieste).State = EntityState.Modified
                Next

                For Each ins As UMA_Richieste_Lavorazioni In InsertArray
                    GiasContext.UMA_Richieste_Lavorazioni.Add(ins)
                Next

                ' COMMIT Effettivo
                GiasContext.SaveChanges()


            End Using


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato

    End Function

    Public Function Nuova_Richiesta(ByVal Richiesta As UMA_Richieste_Lavorazioni,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UMA_Richieste_W.Nuova_Richiesta()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try


            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)


                GiasContext.UMA_Richieste_Lavorazioni.Add(Richiesta)

                ' COMMIT Effettivo
                GiasContext.SaveChanges()


            End Using


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato


    End Function


    Public Function Aggiorna_Lavorazioni_Parziali(ByVal InsertArray As ArrayList,
                                         ByVal UpdateArray As ArrayList,
                                         ByVal DeleteArray As ArrayList,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UMA_Richieste_Lavorazioni.Aggiorna_Lavorazioni_Parziali()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each del As UMA_Lavorazioni_Parziali In DeleteArray
                    GiasContext.UMA_Lavorazioni_Parziali.Attach(del)
                    GiasContext.UMA_Lavorazioni_Parziali.Remove(del)
                Next

                For Each up As UMA_Lavorazioni_Parziali In UpdateArray
                    GiasContext.UMA_Lavorazioni_Parziali.Attach(up)
                    GiasContext.Entry(up).State = EntityState.Modified
                Next

                'For Each upRichieste As UMA_Richieste In RichiesteUpdateArray
                '    GiasContext.UMA_Richieste.Attach(upRichieste)
                '    GiasContext.Entry(upRichieste).State = EntityState.Modified
                'Next

                For Each ins As UMA_Lavorazioni_Parziali In InsertArray
                    GiasContext.UMA_Lavorazioni_Parziali.Add(ins)
                Next

                ' COMMIT Effettivo
                GiasContext.SaveChanges()


            End Using


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato

    End Function

    Public Function Modifica_Campo_Richiesta_Lavorazione(Campo As String,
                                         Valore As Object,
                                         Piva As String,
                                          gruppo_colturale As String,
                                          Programmazione_Cod As Integer,
                                          Richiesta_Cod As Integer,
                                          Richiesta_Dettaglio_Cod As Integer,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_Lavorazioni.Modifica_Campo_Richiesta_Lavorazione()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                Dim PivaSuperUser = objParametri.PivaSuperUser
                Dim lavorazione = (From t In GiasContext.UMA_Richieste_Lavorazioni
                                   Where t.Piva_SuperUser = PivaSuperUser AndAlso
                                     t.Piva = Piva AndAlso
                                     t.Gruppo_Colturale_UMA = gruppo_colturale AndAlso
                                     t.Programmazione_Cod = Programmazione_Cod AndAlso
                                     t.Richiesta_Cod = Richiesta_Cod AndAlso
                                     t.Richiesta_Dettaglio_Cod = Richiesta_Dettaglio_Cod).FirstOrDefault

                If lavorazione IsNot Nothing Then

                    lavorazione.GetType.GetProperty(Campo).SetValue(lavorazione, Valore)
                    lavorazione.Data_Modifica = DateTime.Now
                    lavorazione.Username_Modifica = objParametri.UtenteUsername

                    ' COMMIT Effettivo
                    GiasContext.SaveChanges()

                End If


            End Using


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato


    End Function


End Class


