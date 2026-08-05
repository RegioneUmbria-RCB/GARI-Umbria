Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports System.Transactions
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Data.Linq

Public Class UMA_Richieste_Testata_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Const AVANZAMENTO_RICHIESTA_FITTIZIO As Integer = -10 'Per indicare uno stato in cui non siano necessari i controlli sull'avanzamento (anticipo, richiesta, rendicontazione) alla lettura delle testate

    Public Function Leggi(ByVal piva As String,
                          ByVal Richiesta_Cod As Integer,
                          ByVal Pratica_cod As Integer,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional isTerzista As Integer = 1,
                          Optional anno As Integer = 0,
                          Optional ByVal avanzamento As Integer = -2,
                          Optional ByVal xOrderBy As String = "") As DataTable


        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM UMA_Richieste_Testata t ")
            If anno <> 0 Then
                stb.AppendLine("JOIN Pratiche p on p.pratica_cod = t.pratica_cod")
            End If
            stb.AppendLine(" WHERE t.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            If piva <> "" Then
                stb.AppendLine(" AND t.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If Richiesta_Cod <> 0 Then
                stb.AppendLine(" AND t.Richiesta_Cod = " & Agro_SQL_SaveNum(Richiesta_Cod) & " ")
            End If

            If Pratica_cod <> 0 Then
                stb.AppendLine(" AND t.Pratica_cod = " & Agro_SQL_SaveNum(Pratica_cod) & " ")
            End If

            If Validita_Inizio <> AGRODATAINIZIO Then
                stb.AppendLine(" AND t.Validita_Inizio >= " & Agro_SQL_SaveDateTime(Validita_Inizio) & " ")
            End If

            If Validita_Fine <> AGRODATAFINE Then
                stb.AppendLine(" AND t.Validita_Fine <= " & Agro_SQL_SaveDateTime(Validita_Fine) & " ")
            End If

            If anno <> 0 Then
                stb.AppendLine(" AND p.anno = " & anno.ToString & " ")
            End If

            If avanzamento <> -2 Then
                stb.AppendLine(" AND t.avanzamento_richiesta = " & avanzamento.ToString & " ")
            End If

            If isTerzista <> 1 Then
                stb.AppendLine(" AND t.Tipo_Richiesta = " & isTerzista.ToString & " ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & xOrderBy & " ")
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

    Public Function Leggi(ByVal piva As String,
                          ByVal Richiesta_Cod As Integer,
                          ByVal Pratica_cod As Integer,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal Anno As Integer,
                          ByVal Numero As String,
                          ByVal Avanzamento_Richiesta As Integer,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional isTerzista As Boolean = False,
                          Optional xFiltroAggiuntivo As String = "",
                          Optional ByVal Tipo_Azienda As Integer = 0,
                          Optional ByVal disabilitaFiltroTipoRichiesta As Boolean = False
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM UMA_Richieste_Testata ")
            stb.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            stb.AppendLine(" JOIN Pratiche_Stati_Attuali ON Pratiche_Stati_Attuali.Pratica_Cod = Pratiche.Pratica_Cod ")
            stb.AppendLine(" WHERE UMA_Richieste_Testata.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            If piva <> "" Then
                stb.AppendLine(" AND UMA_Richieste_Testata.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If Richiesta_Cod <> 0 Then
                stb.AppendLine(" AND UMA_Richieste_Testata.Richiesta_Cod = " & Agro_SQL_SaveNum(Richiesta_Cod) & " ")
            End If

            If Pratica_cod <> 0 Then
                stb.AppendLine(" AND UMA_Richieste_Testata.Pratica_cod = " & Agro_SQL_SaveNum(Pratica_cod) & " ")
            End If

            If Anno <> 0 Then
                stb.AppendLine(" AND Pratiche.Anno = " & Agro_SQL_SaveNum(Anno) & " ")
            End If

            If Numero <> "" Then
                stb.AppendLine(" AND Pratiche.Numero =  '" & Agro_SQL_SaveText(Numero) & "' ")
            End If

            If Avanzamento_Richiesta <> AVANZAMENTO_RICHIESTA_FITTIZIO Then
                If Avanzamento_Richiesta = -2 Then
                    stb.AppendLine(" AND UMA_Richieste_Testata.Avanzamento_Richiesta IN (-1, 0) ")
                Else
                    stb.AppendLine(" AND UMA_Richieste_Testata.Avanzamento_Richiesta = " & Agro_SQL_SaveNum(Avanzamento_Richiesta) & " ")
                End If
            End If

            If Tipo_Azienda <> 0 Then
                stb.AppendLine(" AND UMA_Richieste_Testata.Tipo_Azienda = " & Agro_SQL_SaveNum(Tipo_Azienda) & " ")
            End If
            If Validita_Inizio <> AGRODATAINIZIO Then
                stb.AppendLine(" AND UMA_Richieste_Testata.Validita_Inizio >= " & Agro_SQL_SaveDateTime(Validita_Inizio) & " ")
            End If

            If Validita_Fine <> AGRODATAFINE Then
                stb.AppendLine(" AND UMA_Richieste_Testata.Validita_Fine <= " & Agro_SQL_SaveDateTime(Validita_Fine) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            If Not disabilitaFiltroTipoRichiesta Then
                stb.AppendLine(" AND Tipo_Richiesta = " & If(isTerzista, "-1", "0") & " ")
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

    Public Function Leggi_ConSommaCarburanti(ByVal piva As String,
                                             ByVal richiesta_cod As Integer,
                                             ByVal tipo_richiesta As Integer,
                                             ByVal anno As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreParametri,
                                             Optional ByVal solo_approvate As Boolean = True) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.Leggi_rimanenza_iniziale()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            'Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2)
            'NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)
            Dim NomeDB_Utenti = NomeDataBase_FromStringaConnessione(objParametri_Utenti.StringaConnessione)

            stb.Length = 0

            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            stb.AppendLine("")

            stb.AppendLine(";with CTE_Utenti as (")
            stb.AppendLine("    SELECT UserName, CodFisc, Cognome, Nome, Rag_Soc ")
            stb.AppendLine("    FROM " & NomeDB_Utenti & ".dbo.utenti_dettagli (NOLOCK) ")
            stb.AppendLine("),")

            stb.AppendLine("CTE_RichiesteCod as (")
            stb.AppendLine("    SELECT Richiesta_Cod ")
            stb.AppendLine("    FROM UMA_Richieste_Testata ")
            stb.AppendLine("    WHERE Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            stb.AppendLine("    GROUP BY Richiesta_Cod")
            stb.AppendLine("),")

            stb.AppendLine("CTE_Lav as (")
            stb.AppendLine("    SELECT Richiesta_Cod, Tipo_Carburante, SUM(Fabbisogno_Calcolato) as Fabbisogno_Calcolato, SUM(Fabbisogno_Richiesto) as Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) as Fabbisogno_Assegnato ")
            stb.AppendLine("    FROM UMA_Richieste_Lavorazioni ")
            stb.AppendLine("    WHERE exists (SELECT Richiesta_Cod FROM CTE_RichiesteCod WHERE UMA_Richieste_Lavorazioni.Richiesta_Cod = CTE_RichiesteCod.Richiesta_Cod)")
            stb.AppendLine("    GROUP BY Richiesta_Cod, Tipo_Carburante")
            stb.AppendLine("),")

            stb.AppendLine("CTE_LavParz as (")
            stb.AppendLine("    SELECT Richiesta_Cod, Tipo_Carburante, SUM(Fabbisogno_Calcolato) as Fabbisogno_Calcolato, SUM(Fabbisogno_Richiesto) as Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) as Fabbisogno_Assegnato ")
            stb.AppendLine("    FROM UMA_Lavorazioni_Parziali ")
            stb.AppendLine("    WHERE exists (SELECT Richiesta_Cod FROM CTE_RichiesteCod WHERE UMA_Lavorazioni_Parziali.Richiesta_Cod = CTE_RichiesteCod.Richiesta_Cod)")
            stb.AppendLine("    GROUP BY Richiesta_Cod, Tipo_Carburante ")
            stb.AppendLine("),")

            stb.AppendLine("CTE_Allevamenti as (")
            stb.AppendLine("    SELECT Richiesta_Cod, Tipo_Carburante, SUM(Carburante_Richiesto) as Carburante_Richiesto, SUM(Carburante_Calcolato) as Carburante_Calcolato, SUM(Carburante_Approvato) as Carburante_Approvato ")
            stb.AppendLine("    FROM UMA_Richieste_Allevamenti ")
            stb.AppendLine("    WHERE exists (SELECT Richiesta_Cod FROM CTE_RichiesteCod WHERE UMA_Richieste_Allevamenti.Richiesta_Cod = CTE_RichiesteCod.Richiesta_Cod)")
            stb.AppendLine("    GROUP BY Richiesta_Cod, Tipo_Carburante")
            stb.AppendLine(")")

            stb.AppendLine("")

            stb.AppendLine(" SELECT DISTINCT x.* ")
            stb.AppendLine(" FROM ( ")
            stb.AppendLine("     SELECT ")
            stb.AppendLine("        p.Anno, ")
            stb.AppendLine("        p.Numero, ")
            stb.AppendLine("        p.Pratica_Cod, ")
            stb.AppendLine("        psa.Stato_Cod, ")
            stb.AppendLine("        t.Validita_Inizio, ")
            stb.AppendLine("        t.Validita_Fine, ")
            stb.AppendLine("        t.richiesta_Cod, ")
            stb.AppendLine("        t.Tipo_Richiesta,")
            stb.AppendLine("        t.Avanzamento_Richiesta,")
            stb.AppendLine("        t.Richiesta_Integrativa,")
            stb.AppendLine("        t.Rimanenza_Gasolio,")
            stb.AppendLine("        t.Rimanenza_Benzina,")
            stb.AppendLine("        t.Rimanenza_Gasolio_Serra,")
            stb.AppendLine("        ISNULL(ul_gas.Fabbisogno_Calcolato, 0) + ISNULL(ul_gas_Parz.Fabbisogno_Calcolato, 0) + ISNULL(uag.Carburante_Calcolato, 0) as calcolato_Gasolio, ")
            stb.AppendLine("        ISNULL(ul_benz.Fabbisogno_Calcolato, 0) + ISNULL(ul_benz_Parz.Fabbisogno_Calcolato, 0) + ISNULL(uab.Carburante_Calcolato, 0) as calcolato_Benzina, ")
            stb.AppendLine("        ISNULL(ul_serra.Fabbisogno_Calcolato, 0) + ISNULL(ul_serra_Parz.Fabbisogno_Calcolato, 0)  as calcolato_Gasolio_Serra, ")
            stb.AppendLine("        ISNULL(CASE ")
            stb.AppendLine("            WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Richiesta_Iniziale_Gasolio > 0 THEN CASE ")
            stb.AppendLine("                WHEN t.Richiesta_Iniziale_Gasolio > (ISNULL(ul_gas.Fabbisogno_Richiesto, 0) + ISNULL(ul_gas_Parz.Fabbisogno_Richiesto, 0)) THEN t.Richiesta_Iniziale_Gasolio ")
            stb.AppendLine("                ELSE (ISNULL(ul_gas.Fabbisogno_Richiesto, 0) + ISNULL(ul_gas_Parz.Fabbisogno_Richiesto, 0)) ")
            stb.AppendLine("                END ")
            stb.AppendLine("            ELSE (ISNULL(ul_gas.Fabbisogno_Richiesto, 0) + ISNULL(ul_gas_Parz.Fabbisogno_Richiesto, 0)) ")
            stb.AppendLine("            END")
            stb.AppendLine("            , 0) + ISNULL(uag.Carburante_Richiesto, 0) as richiesto_Gasolio, ")
            stb.AppendLine("        ISNULL(CASE ")
            stb.AppendLine("            WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Richiesta_Iniziale_Benzina > 0 THEN t.Richiesta_Iniziale_Benzina ")
            stb.AppendLine("            ELSE (ISNULL(ul_benz.Fabbisogno_Richiesto, 0) + ISNULL(ul_benz_Parz.Fabbisogno_Richiesto, 0)) ")
            stb.AppendLine("            END")
            stb.AppendLine("            , 0) + ISNULL(uab.Carburante_Richiesto, 0) as richiesto_Benzina, ")
            stb.AppendLine("        ISNULL(CASE ")
            stb.AppendLine("            WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Richiesta_Iniziale_Gasolio_Serra > 0 THEN t.Richiesta_Iniziale_Gasolio_Serra ")
            stb.AppendLine("            ELSE (ISNULL(ul_serra.Fabbisogno_Richiesto, 0) + ISNULL(ul_serra_Parz.Fabbisogno_Richiesto, 0))  ")
            stb.AppendLine("            END, 0) as richiesto_Gasolio_Serra, ")
            stb.AppendLine("        ISNULL(CASE ")
            stb.AppendLine("            WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Approvazione_Iniziale_Gasolio > 0 THEN t.Approvazione_Iniziale_Gasolio ")
            stb.AppendLine("            ELSE ISNULL(ul_gas.Fabbisogno_Assegnato, 0) + ISNULL(ul_gas_Parz.Fabbisogno_Assegnato, 0) ")
            stb.AppendLine("            END")
            stb.AppendLine("            , 0) + ISNULL(uag.Carburante_Approvato, 0) as assegnato_gasolio, ")
            stb.AppendLine("        ISNULL(CASE ")
            stb.AppendLine("            WHEN t.Tipo_Richiesta = -1 And t.Avanzamento_Richiesta = 0 AND t.Approvazione_Iniziale_Benzina > 0 Then t.Approvazione_Iniziale_Benzina ")
            stb.AppendLine("            Else ISNULL(ul_benz.Fabbisogno_Assegnato, 0) + ISNULL(ul_benz_Parz.Fabbisogno_Assegnato, 0)  ")
            stb.AppendLine("            End")
            stb.AppendLine("            , 0) + ISNULL(uab.Carburante_Approvato, 0) As assegnato_benzina, ")
            stb.AppendLine("        ISNULL(Case ")
            stb.AppendLine("            When t.Tipo_Richiesta = -1 And t.Avanzamento_Richiesta = 0 AND t.Approvazione_Iniziale_Gasolio_Serra > 0 Then t.Approvazione_Iniziale_Gasolio_Serra ")
            stb.AppendLine("            Else ISNULL(ul_serra.Fabbisogno_Assegnato, 0) + ISNULL(ul_serra_Parz.Fabbisogno_Assegnato, 0) ")
            stb.AppendLine("            End")
            stb.AppendLine("            , 0) As assegnato_gasolio_serra,")
            stb.AppendLine("         ISNULL(uag.Carburante_Calcolato, 0) as Carburante_Calcolato_Gasolio_Allevamenti, ")
            stb.AppendLine("         ISNULL(uag.Carburante_Richiesto, 0) as Carburante_Richiesto_Gasolio_Allevamenti, ")
            stb.AppendLine("         ISNULL(uag.Carburante_Approvato, 0) as Carburante_Approvato_Gasolio_Allevamenti, ")
            stb.AppendLine("         ISNULL(uab.Carburante_Calcolato, 0) as Carburante_Calcolato_Benzina_Allevamenti, ")
            stb.AppendLine("         ISNULL(uab.Carburante_Richiesto, 0) as Carburante_Richiesto_Benzina_Allevamenti, ")
            stb.AppendLine("         ISNULL(uab.Carburante_Approvato, 0) as Carburante_Approvato_Benzina_Allevamenti, ")
            stb.AppendLine("         ISNULL(t.Rec_Acc_Dich_Gasolio, 0) as Rec_Acc_Dich_Gasolio, ")
            stb.AppendLine("         ISNULL(t.Rec_Acc_Dich_Benzina, 0) as Rec_Acc_Dich_Benzina, ")
            stb.AppendLine("         ISNULL(t.Rec_Acc_Dich_Gasolio_Serra, 0) as Rec_Acc_Dich_Gasolio_Serra, ")
            stb.AppendLine("         ISNULL(t.Rec_Acc_Conf_Gasolio, 0) as Rec_Acc_Conf_Gasolio, ")
            stb.AppendLine("         ISNULL(t.Rec_Acc_Conf_Benzina, 0) as Rec_Acc_Conf_Benzina, ")
            stb.AppendLine("         ISNULL(t.Rec_Acc_Conf_Gasolio_Serra, 0) as Rec_Acc_Conf_Gasolio_Serra, ")
            stb.AppendLine("         ISNULL(richiedenti.Nome, '') as Nome_Richiedente,")
            stb.AppendLine("         ISNULL(richiedenti.Cognome, '') as Cognome_Richiedente,")
            stb.AppendLine("         ISNULL(richiedenti.Rag_Soc, '') as Rag_Soc_Richiedente, ")
            stb.AppendLine("         CASE WHEN approvatoriUN.UserName is not null THEN ISNULL(approvatoriUN.Nome, '') ELSE ISNULL(approvatoriCF.Nome, '') END as Nome_Approvatore, ")
            stb.AppendLine("         CASE WHEN approvatoriUN.UserName is not null THEN ISNULL(approvatoriUN.Cognome, '') ELSE ISNULL(approvatoriCF.Cognome, '') END as Cognome_Approvatore, ")
            stb.AppendLine("         CASE WHEN approvatoriUN.UserName is not null THEN ISNULL(approvatoriUN.Rag_Soc, '') ELSE ISNULL(approvatoriCF.Rag_Soc, '') END as Rag_Soc_Approvatore ")
            stb.AppendLine("     FROM UMA_Richieste_Testata t ")
            stb.AppendLine("     Left Join CTE_Lav ul_gas ON t.Richiesta_Cod = ul_gas.Richiesta_Cod And ul_gas.Tipo_Carburante = 2 ")
            stb.AppendLine("     Left Join CTE_Lav ul_benz ON t.Richiesta_Cod = ul_benz.Richiesta_Cod And ul_benz.Tipo_Carburante = 3 ")
            stb.AppendLine("     Left Join CTE_Lav ul_serra ON t.Richiesta_Cod = ul_serra.Richiesta_Cod And ul_serra.Tipo_Carburante = 8 ")
            stb.AppendLine("     Left Join CTE_LavParz ul_gas_Parz ON t.Richiesta_Cod = ul_gas_Parz.Richiesta_Cod And ul_gas_Parz.Tipo_Carburante = 2  ")
            stb.AppendLine("     Left Join CTE_LavParz ul_benz_Parz ON t.Richiesta_Cod = ul_benz_Parz.Richiesta_Cod And ul_benz_Parz.Tipo_Carburante = 3 ")
            stb.AppendLine("     Left Join CTE_LavParz ul_serra_Parz ON t.Richiesta_Cod = ul_serra_Parz.Richiesta_Cod And ul_serra_Parz.Tipo_Carburante = 8 ")
            stb.AppendLine("     Left Join CTE_Allevamenti uag ON t.Richiesta_Cod = uag.Richiesta_Cod AND uag.Tipo_Carburante = 2 ")
            stb.AppendLine("     Left Join CTE_Allevamenti uab ON t.Richiesta_Cod = uab.Richiesta_Cod And uab.Tipo_Carburante = 3 ")
            stb.AppendLine("     join Pratiche p on t.Piva = p.Piva And t.Pratica_Cod = p.Pratica_Cod ")
            stb.AppendLine("     join Pratiche_Stati_Attuali psa on p.Piva_SuperUser = psa.Piva_SuperUser And p.Pratica_Cod = psa.Pratica_Cod ")
            stb.AppendLine("     left join CTE_Utenti richiedenti on t.Username_Creazione = richiedenti.UserName ")
            stb.AppendLine("     left join CTE_Utenti approvatoriUN on t.Approvatore = approvatoriUN.UserName ")
            stb.AppendLine("     left join CTE_Utenti approvatoriCF on t.Approvatore = approvatoriCF.CodFisc ")
            stb.AppendLine("     WHERE t.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "' ")
            stb.AppendLine("     AND t.piva = '" & Agro_SQL_SaveText(piva) & "' ")

            If richiesta_cod <> 0 Then
                stb.AppendLine("     AND t.Richiesta_Cod = " & Agro_SQL_SaveNum(richiesta_cod) & " ")
            End If

            If tipo_richiesta <> 1 Then
                stb.AppendLine("     AND t.Tipo_Richiesta = " & Agro_SQL_SaveNum(tipo_richiesta) & " ")
            End If

            If anno <> 0 Then
                stb.AppendLine("     AND p.Anno = " & Agro_SQL_SaveNum(anno) & " ")
            End If

            If solo_approvate Then
                stb.AppendLine("     AND psa.Stato_Cod = " & enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo & " ")
            End If

            stb.AppendLine(" ) As x ")

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy) & " ")
            End If

            stb.AppendLine(" ;")

            stb.AppendLine("")

            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ COMMITTED; ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Public Function Leggi_rimanenza_iniziale(ByVal piva As String,
                                             ByVal Avanzamento_Richiesta As Integer,
                                             ByVal Tipo_Richiesta As Integer,
                                             ByVal anno As Integer,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             Optional permesso_Acqua As Boolean = False) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.Leggi_rimanenza_iniziale()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            If Not permesso_Acqua Then
                stb.AppendLine(" SELECT TOP (1) t.Rimanenza_Gasolio, t.Rimanenza_Benzina, t.Rimanenza_Gasolio_Serra ")
            Else
                stb.AppendLine(" SELECT TOP (1) t.permesso_acqua, t.Note_permesso_acqua ")
            End If
            stb.AppendLine(" FROM UMA_Richieste_Testata t")
            stb.AppendLine(" JOIN Pratiche p ON p.pratica_cod = t.pratica_cod ")
            stb.AppendLine(" JOIN Pratiche_Stati_Attuali ps ON p.pratica_cod = ps.pratica_cod ")
            stb.AppendLine(" WHERE t.Avanzamento_Richiesta = " & Avanzamento_Richiesta & " and t.Richiesta_Integrativa = 0 and t.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            stb.AppendLine(" And t.tipo_richiesta = " & Tipo_Richiesta & " And p.Anno = " & anno & " And ps.stato_Cod NOT IN (2006, 2009)")
            stb.AppendLine(" ORDER BY p.Numero ASC, t.Data_Creazione ASC")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiDaRichiestaCod(ByVal richiestaCod As Integer,
                                        ByRef objParametri As AgronicaCoreParametri) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM UMA_Richieste_Testata ")
            stb.AppendLine(" WHERE richiesta_cod = " & Agro_SQL_SaveNum(richiestaCod) & " ")
            stb.AppendLine(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

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


    Public Function Leggi_Elenco(ByVal piva As String,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreParametri,
                                 Optional ByVal rendicontazioni As Boolean = False,
                                 Optional ByVal statoCod As Integer = -1,
                                 Optional ByVal citta As String = "-1") As DataTable


        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.Leggi_Elenco()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT t.Piva, t.Pratica_Cod, p.Anno, p.Numero, psa.stato_cod, psaw.WAnagraficaStati_Des ")
            stb.AppendLine(" ,psa.Validita_Inizio As Data_Ultimo_Passaggio_Stato, t.Validita_Inizio, t.Validita_Fine, t.richiesta_Cod ")
            stb.AppendLine(" ,t.Carburante_Calcolato, t.Carburante_Richiesto, t.Carburante_Approvato, t.Tipo_Richiesta  ")
            stb.AppendLine(" FROM UMA_Richieste_Testata t ")
            stb.AppendLine(" join Pratiche p on t.Piva = p.Piva and t.Pratica_Cod = p.Pratica_Cod ")
            stb.AppendLine(" join Pratiche_Stati_Attuali psa on p.Piva_SuperUser = psa.Piva_SuperUser and p.Pratica_Cod = psa.Pratica_Cod ")
            stb.AppendLine(" join WAnagraficaStati psaw on psa.Stato_Cod = psaw.WAnagraficaStati_Cod ")
            If (citta <> "-1") Then
                stb.AppendLine(" INNER JOIN ImpresexIndirizzi ON t.Piva = ImpresexIndirizzi.Piva ")
                stb.AppendLine(" INNER JOIN Indirizzi ON ImpresexIndirizzi.Cod_Indirizzo = Indirizzi.Cod_Indirizzo  ")
                stb.AppendLine(" JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM AND ISTAT.LOCALITA = '" & Agro_SQL_SaveText(citta) & "' ")
            End If
            stb.AppendLine(" WHERE 1 = 1")
            If (piva <> "") Then
                stb.AppendLine(" AND t.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            stb.AppendLine(" AND t.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            stb.AppendLine(" AND t.Avanzamento_Richiesta " & If(rendicontazioni, " = 1 ", " = 0 "))

            If (statoCod > 0) Then
                stb.AppendLine(" AND psaw.WAnagraficaStati_Cod = " & statoCod.ToString & " ")
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


    Public Function Leggi_Elenco2(ByVal piva As String,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreParametri,
                                 Optional ByVal rendicontazioni As Boolean = False,
                                 Optional ByVal statoCod As Integer = -1,
                                 Optional ByVal citta As String = "-1") As DataTable


        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.Leggi_Elenco2()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT t.Piva, t.Pratica_Cod, p.Anno, p.Numero, ")
            stb.AppendLine(" t.Validita_Inizio, t.Validita_Fine, t.richiesta_Cod ")
            stb.AppendLine(" ,t.Carburante_Calcolato, t.Carburante_Richiesto, t.Carburante_Approvato, t.Tipo_Richiesta  ")
            stb.AppendLine(" FROM UMA_Richieste_Testata t ")
            stb.AppendLine(" join Pratiche p on t.Piva = p.Piva and t.Pratica_Cod = p.Pratica_Cod ")

            stb.AppendLine(" WHERE 1 = 1")
            If (piva <> "") Then
                stb.AppendLine(" AND t.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            stb.AppendLine(" AND t.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If (statoCod > 0) Then
                stb.AppendLine(" AND psaw.WAnagraficaStati_Cod = " & statoCod.ToString & " ")
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

    Public Function Leggi_Elenco3(ByVal piva As String,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreParametri,
                                  Optional ByVal statoCod As Integer = -1) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.Leggi_Elenco3()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT t.Piva, t.Pratica_Cod, p.Anno, p.Numero, psa.stato_cod, psaw.WAnagraficaStati_Des ")
            stb.AppendLine(" ,t.Validita_Inizio, t.Validita_Fine, t.richiesta_Cod, t.avanzamento_richiesta ")
            stb.AppendLine(" ,t.Tipo_Richiesta  ")
            stb.AppendLine(" FROM UMA_Richieste_Testata t ")
            stb.AppendLine(" join Pratiche p on t.Piva = p.Piva and t.Pratica_Cod = p.Pratica_Cod ")
            stb.AppendLine(" join Pratiche_Stati_Attuali psa on p.Piva_SuperUser = psa.Piva_SuperUser and p.Pratica_Cod = psa.Pratica_Cod ")
            stb.AppendLine(" join WAnagraficaStati psaw on psa.Stato_Cod = psaw.WAnagraficaStati_Cod ")
            stb.AppendLine(" WHERE 1 = 1")

            If (piva <> "") Then
                stb.AppendLine(" AND t.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            stb.AppendLine(" AND t.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If (statoCod > 0) Then
                stb.AppendLine(" AND psaw.WAnagraficaStati_Cod = " & statoCod.ToString & " ")
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

    Public Function Leggi_Stato_Modificabile(ByVal piva As String, ByVal richiestea_Cod As Integer, ByVal isApprovazione As Boolean,
                                ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.Leggi_Stato_Modificabile()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim res As Boolean = False

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM UMA_Richieste_Testata t")
            stb.AppendLine(" JOIN Pratiche_Stati_Attuali p on p.Pratica_Cod = t.Pratica_Cod ")
            stb.AppendLine(" WHERE richiesta_cod = " & Agro_SQL_SaveNum(richiestea_Cod) & " ")
            stb.AppendLine(" AND t.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            stb.AppendLine(" AND p.stato_cod = " & If(isApprovazione, "2002", "2001") & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If (dt.Rows.Count > 0) Then
            res = True
        End If

        Return res

    End Function

    Public Function LeggiRiepilogo(ByVal piva As String,
                                   ByVal anno As Integer,
                                   ByVal filtroVisibilita As Boolean,
                                   ByVal piveVisibili As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreParametri,
                                   ByVal rendicontazioni As Boolean,
                                   ByVal statoCod As Integer,
                                   ByVal citta As String,
                                   ByVal prov As String,
                                   ByVal conto As Integer,
                                   ByVal FiltroNuovaVisibilita As Boolean,
                                   ByVal FiltroUtente As Boolean,
                                   ByVal FiltroGruppoUtente As Boolean,
                                   ByVal GruppoUtente As Integer,
                                   ByVal VisibilitaTotale As Boolean) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.LeggiRiepilogo()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            'Creazione delle CTE
            stb.AppendLine(" WITH  ")
            stb.AppendLine(" PraticheXAnno_CTE as ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select  p.anno, p.Piva_SuperUser, p.piva, p.Pratica_Cod from pratiche p (nolock) ")
            stb.AppendLine(" JOIN Pratiche_Stati_Attuali psa (nolock) On p.Piva_SuperUser = psa.Piva_SuperUser And p.Pratica_Cod = psa.Pratica_Cod  ")
            stb.AppendLine(" JOIN WAnagraficaStati psaw (nolock) On psa.Stato_Cod = psaw.WAnagraficaStati_Cod  ")
            stb.AppendLine(" where p.anno = " & anno.ToString & " And p.Servizio_Cod = 2007 And psa.Stato_Cod IN (2003, 2005, 2008) ")
            If (statoCod > 0) Then
                stb.AppendLine(" AND psaw.WAnagraficaStati_Cod = " & statoCod.ToString & " ")
            End If
            stb.AppendLine(" ), ")

            stb.AppendLine(" Testa_CTE AS  ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select t.piva, p.anno, t.tipo_richiesta, t.richiesta_integrativa, t.Tipo_Azienda, ")
            stb.AppendLine(" MAX(t.Rimanenza_Gasolio) As Rimanenza_Gasolio, MAX(t.Rimanenza_Benzina) As Rimanenza_Benzina, MAX(t.Rimanenza_Gasolio_Serra) As Rimanenza_Gasolio_Serra, ")
            stb.AppendLine(" SUM(t.Richiesta_Iniziale_Gasolio) As Iniziale_Gasolio, SUM(t.Approvazione_Iniziale_Gasolio) As Approvazione_Iniziale_Gasolio, ")
            stb.AppendLine(" SUM(t.Richiesta_Iniziale_Benzina) As Iniziale_Benzina, SUM(t.Approvazione_Iniziale_Benzina) As Approvazione_Iniziale_Benzina, ")
            stb.AppendLine(" SUM(t.Richiesta_Iniziale_Gasolio_Serra) As Iniziale_Gasolio_Serra, SUM(t.Approvazione_Iniziale_Gasolio_Serra) As Approvazione_Iniziale_Gasolio_Serra ")
            stb.AppendLine(" from UMA_Richieste_Testata t (nolock) ")
            stb.AppendLine(" Join PraticheXAnno_CTE p on p.Pratica_Cod = t.Pratica_Cod ")
            stb.AppendLine(" Where Avanzamento_Richiesta IN (-1, 0) ")
            stb.AppendLine(" GROUP BY p.Anno, t.piva, t.tipo_richiesta, t.richiesta_integrativa, t.Tipo_Azienda  ")
            stb.AppendLine(" ), ")

            stb.AppendLine(" Data_CTE AS")
            stb.AppendLine(" ( Select t.piva, p.anno, t.tipo_richiesta, ")
            stb.AppendLine(" t.Data_Creazione As Data_Presentazione_Rendicontazione")
            stb.AppendLine(" from UMA_Richieste_Testata t (nolock) ")
            stb.AppendLine(" Join pratiche p on p.Pratica_Cod = t.Pratica_Cod  ")
            stb.AppendLine("Where Avanzamento_Richiesta = 1 and p.anno = " & anno.ToString & " and p.Servizio_Cod = 2007")
            stb.AppendLine(" ), ")

            'Una CTE per ogni tipo di carburante sia per richieste, per rendicontazioni che per anticipi

            stb.AppendLine(" Lav_CTE(Piva, Avanzamento_Richiesta, Tipo_Richiesta, Tipo_Carburante, Fabbisogno_Richiesto, Fabbisogno_Assegnato, Pratica_Cod, Rimanenza_Gasolio, Rimanenza_Benzina, Rimanenza_Gasolio_Serra) AS  ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" SELECT t.piva, t.Avanzamento_Richiesta, t.Tipo_Richiesta, l.Tipo_Carburante, l.Fabbisogno_Richiesto, l.Fabbisogno_Assegnato, t.pratica_cod, t.Rimanenza_Gasolio, t.rimanenza_benzina, t.rimanenza_gasolio_serra ")
            stb.AppendLine(" FROM UMA_Richieste_Lavorazioni l (nolock) ")
            stb.AppendLine(" JOIN UMA_Richieste_Testata t (nolock) on t.Richiesta_Cod = l.Richiesta_Cod ")
            stb.AppendLine(" JOIN PraticheXAnno_CTE p on p.Pratica_Cod = t.pratica_cod ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_Anticipo_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select * from Lav_CTE where Avanzamento_Richiesta = -1 ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_0_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select * from Lav_CTE where Avanzamento_Richiesta = 0 ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_1_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select * from Lav_CTE where Avanzamento_Richiesta = 1 ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_Anticipo_Gasolio_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato ")
            stb.AppendLine(" from Lav_Avanzamento_Anticipo_CTE where Tipo_Carburante = 2   ")
            stb.AppendLine(" Group By Piva, Tipo_Richiesta  ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_Anticipo_Benzina_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato ")
            stb.AppendLine(" from Lav_Avanzamento_Anticipo_CTE where Tipo_Carburante = 3  ")
            stb.AppendLine(" Group By Piva, Tipo_Richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_Anticipo_GasolioSerra_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato ")
            stb.AppendLine(" from Lav_Avanzamento_Anticipo_CTE where Tipo_Carburante = 8 ")
            stb.AppendLine("  Group By Piva, Tipo_Richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_0_Gasolio_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato ")
            stb.AppendLine(" from Lav_Avanzamento_0_CTE where Tipo_Carburante = 2   ")
            stb.AppendLine(" Group By Piva, Tipo_Richiesta  ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_0_Benzina_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato ")
            stb.AppendLine(" from Lav_Avanzamento_0_CTE where Tipo_Carburante = 3  ")
            stb.AppendLine(" Group By Piva, Tipo_Richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_0_GasolioSerra_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato ")
            stb.AppendLine(" from Lav_Avanzamento_0_CTE where Tipo_Carburante = 8 ")
            stb.AppendLine("  Group By Piva, Tipo_Richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_0_Elettricita_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta ")
            stb.AppendLine(" from Lav_Avanzamento_0_CTE where Tipo_Carburante = 9  ")
            stb.AppendLine(" Group By Piva, Tipo_Richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_0_CarburanteNonAgricolo_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta ")
            stb.AppendLine(" from Lav_Avanzamento_0_CTE where Tipo_Carburante = 10 ")
            stb.AppendLine("  Group By Piva, Tipo_Richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_1_Gasolio_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato, ")
            stb.AppendLine(" AVG(Rimanenza_Gasolio) As Rimanenza_Gasolio")
            stb.AppendLine(" from Lav_Avanzamento_1_CTE where Tipo_Carburante = 2   ")
            stb.AppendLine(" Group By Piva, Tipo_Richiesta  ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_1_Benzina_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato, ")
            stb.AppendLine(" AVG(Rimanenza_Benzina) As Rimanenza_Benzina ")
            stb.AppendLine(" from Lav_Avanzamento_1_CTE where Tipo_Carburante = 3  ")
            stb.AppendLine(" Group By Piva, Tipo_Richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_1_GasolioSerra_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato, ")
            stb.AppendLine(" AVG(Rimanenza_Gasolio_Serra) As Rimanenza_Gasolio_Serra ")
            stb.AppendLine(" from Lav_Avanzamento_1_CTE where Tipo_Carburante = 8 ")
            stb.AppendLine(" Group By Piva, Tipo_Richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_1_Elettricita_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta ")
            stb.AppendLine(" from Lav_Avanzamento_1_CTE where Tipo_Carburante = 9  ")
            stb.AppendLine(" Group By Piva, Tipo_Richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Lav_Avanzamento_1_CarburanteNonAgricolo_CTE ")
            stb.AppendLine(" As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select Piva, Tipo_Richiesta ")
            stb.AppendLine(" from Lav_Avanzamento_1_CTE where Tipo_Carburante = 10 ")
            stb.AppendLine(" Group By Piva, Tipo_Richiesta ")
            stb.AppendLine(" ), ")

            ' CTE terzisti carburante richiesto/approvato raggruppata per richiesta
            stb.AppendLine(" Terzisti_Rich_Appr AS ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select t.piva, t.richiesta_cod, ")
            stb.AppendLine(" max(t.Richiesta_Iniziale_Gasolio) As Richiesta_Iniziale_Gasolio, ")
            stb.AppendLine(" max(t.Richiesta_Iniziale_Benzina) As Richiesta_Iniziale_Benzina, ")
            stb.AppendLine(" max(t.Richiesta_Iniziale_Gasolio_Serra) As Richiesta_Iniziale_Gasolio_Serra, ")
            stb.AppendLine(" max(t.Approvazione_Iniziale_Gasolio) As Approvazione_Iniziale_Gasolio, ")
            stb.AppendLine(" max(t.Approvazione_Iniziale_Benzina) As Approvazione_Iniziale_Benzina, ")
            stb.AppendLine(" max(t.Approvazione_Iniziale_Gasolio_Serra) As Approvazione_Iniziale_Gasolio_Serra, ")
            stb.AppendLine(" sum(case when rl.tipo_carburante = 2 then isnull(rl.Fabbisogno_Richiesto, 0) Else 0 End) lav_fabb_rich_gasolio, ")
            stb.AppendLine(" sum(case when rl.tipo_carburante = 3 then isnull(rl.Fabbisogno_Richiesto, 0) Else 0 End) lav_fabb_rich_benzina, ")
            stb.AppendLine(" sum(case when rl.tipo_carburante = 8 then isnull(rl.Fabbisogno_Richiesto, 0) Else 0 End) lav_fabb_rich_gasolio_serra, ")
            stb.AppendLine(" sum(case when rl.tipo_carburante = 2 then isnull(rl.Fabbisogno_Assegnato, 0) Else 0 End) lav_fabb_asse_gasolio, ")
            stb.AppendLine(" sum(case when rl.tipo_carburante = 3 then isnull(rl.Fabbisogno_Assegnato, 0) Else 0 End) lav_fabb_asse_benzina, ")
            stb.AppendLine(" sum(case when rl.tipo_carburante = 8 then isnull(rl.Fabbisogno_Assegnato, 0) Else 0 End) lav_fabb_asse_gasolio_serra, ")
            stb.AppendLine(" sum(case when lp.tipo_carburante = 2 then isnull(lp.Fabbisogno_Richiesto, 0) Else 0 End) parz_fabb_rich_gasolio, ")
            stb.AppendLine(" sum(case when lp.tipo_carburante = 3 then isnull(lp.Fabbisogno_Richiesto, 0) Else 0 End) parz_fabb_rich_benzina, ")
            stb.AppendLine(" sum(case when lp.tipo_carburante = 8 then isnull(lp.Fabbisogno_Richiesto, 0) Else 0 End) parz_fabb_rich_gasolio_serra, ")
            stb.AppendLine(" sum(case when lp.tipo_carburante = 2 then isnull(lp.Fabbisogno_Assegnato, 0) Else 0 End) parz_fabb_asse_gasolio, ")
            stb.AppendLine(" sum(case when lp.tipo_carburante = 3 then isnull(lp.Fabbisogno_Assegnato, 0) Else 0 End) parz_fabb_asse_benzina, ")
            stb.AppendLine(" sum(case when lp.tipo_carburante = 8 then isnull(lp.Fabbisogno_Assegnato, 0) Else 0 End) parz_fabb_asse_gasolio_serra ")
            stb.AppendLine(" From UMA_Richieste_Testata t (nolock) ")
            stb.AppendLine(" Join PraticheXAnno_CTE p on p.Pratica_Cod = t.pratica_cod ")
            stb.AppendLine(" Left Join UMA_Richieste_Lavorazioni rl (nolock) on rl.Piva_SuperUser = t.Piva_SuperUser And rl.richiesta_cod = t.richiesta_cod ")
            stb.AppendLine(" Left Join UMA_Lavorazioni_Parziali lp (nolock) on lp.Piva_SuperUser = t.Piva_SuperUser And lp.Piva = t.Piva And lp.richiesta_cod = t.richiesta_cod ")
            stb.AppendLine(" WHERE t.Tipo_Richiesta = -1 ")
            stb.AppendLine(" And t.Avanzamento_Richiesta = 0 ")
            stb.AppendLine(" Group By t.Piva, t.richiesta_cod ")
            stb.AppendLine(" ), ")

            'CTE terzisti carburante richiesto/approvato raggruppata per piva, per prendere il valore maggiore fra richiesta iniziale ed effettive lavorazioni
            stb.AppendLine(" Terzisti_Rich_Appr_Finale AS  ")
            stb.AppendLine(" (   ")
            stb.AppendLine(" select piva ")
            stb.AppendLine(" ,Sum(case when Richiesta_Iniziale_Gasolio > (lav_fabb_rich_gasolio + parz_fabb_rich_gasolio) ")
            stb.AppendLine("           then Richiesta_Iniziale_Gasolio ")
            stb.AppendLine("           else lav_fabb_rich_gasolio + parz_fabb_rich_gasolio ")
            stb.AppendLine("      end) terzista_richiesta_gasolio ")
            stb.AppendLine(" ,Sum(case when Richiesta_Iniziale_Benzina > (lav_fabb_rich_benzina + parz_fabb_rich_benzina) ")
            stb.AppendLine("           then Richiesta_Iniziale_Benzina ")
            stb.AppendLine("           else lav_fabb_rich_benzina + parz_fabb_rich_benzina ")
            stb.AppendLine("      end) terzista_richiesta_benzina ")
            stb.AppendLine(" ,Sum(case when Richiesta_Iniziale_Gasolio_Serra > (lav_fabb_rich_gasolio_serra + parz_fabb_rich_gasolio_serra) ")
            stb.AppendLine("           then Richiesta_Iniziale_Gasolio_Serra ")
            stb.AppendLine("           else lav_fabb_rich_gasolio_serra + parz_fabb_rich_gasolio_serra ")
            stb.AppendLine("      end) terzista_richiesta_gasolio_serra ")
            stb.AppendLine(" ,Sum(case when Approvazione_Iniziale_Gasolio > (lav_fabb_asse_gasolio + parz_fabb_asse_gasolio) ")
            stb.AppendLine("           then Approvazione_Iniziale_Gasolio ")
            stb.AppendLine("           else lav_fabb_asse_gasolio + parz_fabb_asse_gasolio ")
            stb.AppendLine("      end) terzista_approvazione_gasolio ")
            stb.AppendLine(" ,Sum(case when Approvazione_Iniziale_Benzina > (lav_fabb_asse_benzina + parz_fabb_asse_benzina) ")
            stb.AppendLine("           then Approvazione_Iniziale_Benzina ")
            stb.AppendLine("           else lav_fabb_asse_benzina + parz_fabb_asse_benzina ")
            stb.AppendLine("      end) terzista_approvazione_benzina ")
            stb.AppendLine(" ,Sum(case when Approvazione_Iniziale_Gasolio_Serra > (lav_fabb_asse_gasolio_serra + parz_fabb_asse_gasolio_serra) ")
            stb.AppendLine("           then Approvazione_Iniziale_Gasolio_Serra ")
            stb.AppendLine("           else lav_fabb_asse_gasolio_serra + parz_fabb_asse_gasolio_serra ")
            stb.AppendLine("      end) terzista_approvazione_gasolio_serra ")
            stb.AppendLine(" from Terzisti_Rich_Appr ")
            stb.AppendLine(" Group By Piva ")
            stb.AppendLine(" ), ")

            'CTE per ogni tipo di carburante utilizzato in allevamenti
            stb.AppendLine(" Allevamenti_CTE AS ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select al.piva, t.tipo_Richiesta, al.Carburante_Richiesto, al.Carburante_Approvato As Carburante_Approvato, al.Tipo_Carburante, t.Avanzamento_Richiesta, ")
            stb.AppendLine(" t.Rimanenza_Benzina, t.rimanenza_gasolio From UMA_Richieste_Allevamenti al (nolock) ")
            stb.AppendLine(" Join UMA_Richieste_Testata t (nolock) on t.Richiesta_Cod = al.Richiesta_Cod ")
            stb.AppendLine(" Join PraticheXAnno_CTE p on p.Pratica_Cod = t.Pratica_Cod ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Carburante_Gasolio_Richieste_Allevamenti_CTE As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select piva, tipo_richiesta, SUM(Carburante_Richiesto) As Carburante_Richiesto, SUM(Carburante_Approvato) As Carburante_Approvato ")
            stb.AppendLine(" from Allevamenti_CTE ")
            stb.AppendLine(" where Tipo_Carburante = 2 and Avanzamento_Richiesta = 0 ")
            stb.AppendLine(" group by piva, tipo_richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Carburante_Benzina_Richieste_Allevamenti_CTE As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select piva, tipo_richiesta, SUM(Carburante_Richiesto) As Carburante_Richiesto, SUM(Carburante_Approvato) As Carburante_Approvato ")
            stb.AppendLine(" from Allevamenti_CTE ")
            stb.AppendLine(" where Tipo_Carburante = 3 and Avanzamento_Richiesta = 0 ")
            stb.AppendLine(" group by piva, tipo_richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Carburante_Gasolio_Rendicontazione_Allevamenti_CTE As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select piva, tipo_richiesta, SUM(Carburante_Richiesto) As Carburante_Richiesto, SUM(Carburante_Approvato) As Carburante_Approvato, ")
            stb.AppendLine(" MAX(rimanenza_gasolio) As Rimanenza_Gasolio from Allevamenti_CTE ")
            stb.AppendLine(" where Tipo_Carburante = 2 and Avanzamento_Richiesta = 1 ")
            stb.AppendLine(" group by piva, tipo_richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Carburante_Benzina_Rendicontazione_Allevamenti_CTE As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select piva, tipo_richiesta, SUM(Carburante_Richiesto) As Carburante_Richiesto, SUM(Carburante_Approvato) As Carburante_Approvato, ")
            stb.AppendLine(" MAX(rimanenza_Benzina) As Rimanenza_Benzina from Allevamenti_CTE ")
            stb.AppendLine(" where Tipo_Carburante = 3 and Avanzamento_Richiesta = 1 ")
            stb.AppendLine(" group by piva, tipo_richiesta ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Rendicontazioni_Senza_Lav_CTE As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select t.* from UMA_Richieste_Testata t Join PraticheXAnno_CTE p on p.Pratica_Cod = t.Pratica_Cod  ")
            stb.AppendLine(" where t.Avanzamento_Richiesta = 1 and t.Carburante_Richiesto = 0 and  ")
            stb.AppendLine(" (t.Rimanenza_Benzina > 0 or t.Rimanenza_Gasolio > 0 or t.Rimanenza_Gasolio_Serra > 0) ")
            stb.AppendLine(" ),")

            stb.AppendLine(" Pratiche_Stati_Attuali_CTE As ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select * From Pratiche_Stati_Attuali (nolock) Where Stato_Cod In (2003, 2005, 2008) ")
            stb.AppendLine(" And Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "' ")
            stb.AppendLine(" ), ")

            'CTE per indirizzi e imprese
            stb.AppendLine(" Indirizzi_CTE as ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select  ImpresexIndirizzi.PIVA ")
            stb.AppendLine(" From ImpresexIndirizzi (nolock)  ")
            stb.AppendLine(" INNER Join Indirizzi ind (nolock) On ImpresexIndirizzi.Cod_Indirizzo = ind.Cod_Indirizzo   ")
            stb.AppendLine(" Join ISTAT (nolock) ON ind.pro_cod_istat = ISTAT.PROV And ind.com_cod_istat = ISTAT.COM  ")
            stb.AppendLine(" Where 1=1 ")
            If (citta <> "-1" AndAlso citta <> "") Then
                stb.AppendLine(" AND ISTAT.COM = '" & Agro_SQL_SaveText(citta) & "' ")
            End If
            If (prov <> "-1") Then
                stb.AppendLine(" AND ISTAT.PROV = '" & Agro_SQL_SaveText(prov) & "' ")
            End If
            stb.AppendLine(" ), ")

            stb.AppendLine(" Imprese_CTE as ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select  i.piva, i.Rag_Soc, ic.val_cod as CUAA from Imprese i (nolock) ")
            stb.AppendLine(" join Imprese_Codici ic on i.PIVA = ic.Piva ")
            stb.AppendLine(" WHERE ic.id_cod = 1010 ")
            stb.AppendLine(" ) ")

            'Query più esterna per il calcolo dell'acquistabile
            stb.AppendLine(" select *, ")
            stb.AppendLine(" CASE WHEN qf.approvato_Gasolio_Totale > 0 THEN qf.approvato_Gasolio_Totale ELSE qf.Anticipo_Gasolio END - qf.Acquistato_Gasolio - qf.Rimanenza_Iniziale_Gasolio As Acquistabile_Gasolio, ")
            stb.AppendLine(" CASE WHEN qf.approvato_Benzina_Totale > 0 THEN qf.approvato_Benzina_Totale ELSE qf.Anticipo_Benzina END - qf.Acquistato_Benzina - qf.Rimanenza_Iniziale_Benzina As Acquistabile_Benzina, ")
            stb.AppendLine(" CASE WHEN qf.Richiesto_Approvato_Gasolio_Serra > 0 THEN qf.Richiesto_Approvato_Gasolio_Serra ELSE qf.Anticipo_Gasolio_Serra END - qf.Acquistato_Gasolio_Serra - qf.Rimanenza_Iniziale_Gasolio_Serra As Acquistabile_Gasolio_Serra ")
            stb.AppendLine(" from ")

            'Query per calcolo dei totali (Colture + Allevamenti)
            stb.AppendLine(" ( ")
            stb.AppendLine(" select  ")
            stb.AppendLine(" Rag_Soc, CUAA, ")
            stb.AppendLine(" cast(ROUND(Anticipo_Gasolio,0) as int) as Anticipo_Gasolio, cast(ROUND(Anticipo_Benzina,0) as int) as Anticipo_Benzina, cast(ROUND(Anticipo_Gasolio_Serra,0) as int) as Anticipo_Gasolio_Serra, ")
            stb.AppendLine(" cast(ROUND(Richiesta_Gasolio_Coltura + Gasolio_Richiesto_Allevamenti,0) as int) as richiesto_Gasolio_Totale , ")
            stb.AppendLine(" cast(ROUND(Richiesta_Benzina_Coltura + Benzina_Richiesto_Allevamenti,0) as int) as richiesto_Benzina_Totale ,  ")
            stb.AppendLine(" cast(ROUND(Richiesto_Gasolio_Serra,0) as int) as Richiesto_Gasolio_Serra,  ")
            stb.AppendLine(" Richiesto_Numero_Lavorazioni_Elettricita, ")
            stb.AppendLine(" Richiesto_Numero_Lavorazioni_Non_Agricolo, ")
            stb.AppendLine(" cast(ROUND(Rendicontato_Gasolio + Gasolio_Rendicontato_Allevamenti,0) as int) as Rendicontato_Gasolio, ")
            stb.AppendLine(" cast(ROUND(Rendicontato_Benzina + Benzina_Rendicontato_Allevamenti,0) as int) as Rendicontato_Benzina, ")
            stb.AppendLine(" cast(ROUND(Rendicontato_Gasolio_Serra,0) as int) as Rendicontato_Gasolio_Serra, ")
            stb.AppendLine(" Rendicontato_Numero_Lavorazioni_Elettricita, ")
            stb.AppendLine(" Rendicontato_Numero_Lavorazioni_Non_Agricolo,")
            stb.AppendLine(" cast(ROUND(Gasolio_Approvato_Allevamenti + Approvato_Gasolio_Coltura,0) as int) as approvato_Gasolio_Totale , ")
            stb.AppendLine(" cast(ROUND(Benzina_Approvato_Allevamenti + Approvato_Benzina_Coltura,0) as int) as approvato_Benzina_Totale , ")
            stb.AppendLine(" cast(ROUND(Richiesto_Approvato_Gasolio_Serra,0) as int) as Richiesto_Approvato_Gasolio_Serra,  ")
            stb.AppendLine(" cast(ROUND(Rendicontato_Approvato_Gasolio + Gasolio_Rendicontato_Approvato_Allevamenti,0) as int) as Rendicontato_Approvato_Gasolio, ")
            stb.AppendLine(" cast(ROUND(Rendicontato_Approvato_Benzina + Benzina_Rendicontato_Approvato_Allevamenti,0) as int) as Rendicontato_Approvato_Benzina, ")
            stb.AppendLine(" cast(ROUND(Rendicontato_Approvato_Gasolio_Serra,0) as int) as Rendicontato_Approvato_Gasolio_Serra, ")
            stb.AppendLine(" Rimanenza_Iniziale_Gasolio,  ")
            stb.AppendLine(" Rimanenza_Iniziale_Benzina, ")
            stb.AppendLine(" Rimanenza_Iniziale_Gasolio_Serra, ")
            stb.AppendLine(" Rimanenza_Finale_Gasolio, ")
            stb.AppendLine(" Rimanenza_Finale_Benzina, ")
            stb.AppendLine(" Rimanenza_Finale_Gasolio_Serra, ")
            stb.AppendLine(" Conto, ")
            stb.AppendLine(" Tipo_Azienda, Tipo_Azienda_Des, ")
            stb.AppendLine(" Acquistato_Gasolio, ")
            stb.AppendLine(" Acquistato_Benzina,   ")
            stb.AppendLine(" Acquistato_Gasolio_Serra, ")
            stb.AppendLine(" Anno, Data_Presentazione_Rendicontazione ")
            stb.AppendLine(" from ")

            'Query per distinguere tra carburante conto terzi o conto proprio
            stb.AppendLine(" ( ")
            stb.AppendLine(" select  ")
            stb.AppendLine(" qp.Rag_Soc, qp.CUAA, ")
            stb.AppendLine(" ISNULL(cgr.Carburante_Richiesto,0) as Gasolio_Richiesto_Allevamenti, ")
            stb.AppendLine(" ISNULL(cbr.Carburante_Richiesto, 0) as Benzina_Richiesto_Allevamenti, ")
            stb.AppendLine(" ISNULL(cgra.Carburante_Richiesto,0) as Gasolio_Rendicontato_Allevamenti, ")
            stb.AppendLine(" ISNULL(cbra.Carburante_Richiesto, 0) as Benzina_Rendicontato_Allevamenti,  ")
            stb.AppendLine(" ISNULL(qp.Anticipo_Gasolio, 0) as Anticipo_Gasolio, ")
            stb.AppendLine(" ISNULL(qp.Anticipo_Benzina, 0) as Anticipo_Benzina, ")
            stb.AppendLine(" ISNULL(qp.Anticipo_Gasolio_Serra, 0) as Anticipo_Gasolio_Serra, ")

            'In alcuni casi occorre differenziare conto proprio e conto terzi perchè cambiano i campi che si devono considerare

            'Richiesta_Gasolio_Coltura
            stb.AppendLine(" CASE WHEN qp.Tipo_Richiesta = -1 THEN ISNULL(terzista_richiesta_gasolio,0) ")
            stb.AppendLine(" ELSE ISNULL(qp.Richiesta_Iniziale_Gasolio,0) END As Richiesta_Gasolio_Coltura, ")

            'Richiesta_Benzina_Coltura
            stb.AppendLine(" CASE WHEN qp.Tipo_Richiesta = -1 THEN ISNULL(terzista_richiesta_benzina,0) ")
            stb.AppendLine(" ELSE ISNULL(qp.Richiesta_Iniziale_Benzina,0) END As Richiesta_Benzina_Coltura,  ")

            'Richiesto_Gasolio_Serra
            stb.AppendLine(" CASE WHEN qp.Tipo_Richiesta = -1 THEN ISNULL(terzista_richiesta_gasolio_serra,0) ")
            stb.AppendLine(" ELSE ISNULL(qp.Richiesta_Iniziale_Gasolio_Serra,0) END As Richiesto_Gasolio_Serra, ")

            stb.AppendLine(" Rendicontato_Gasolio,  ")
            stb.AppendLine(" Rendicontato_Benzina,  ")
            stb.AppendLine(" Rendicontato_Gasolio_Serra, ")
            stb.AppendLine(" ISNULL(cgr.Carburante_Approvato,0) As Gasolio_Approvato_Allevamenti, ")
            stb.AppendLine(" ISNULL(cbr.Carburante_Approvato, 0) As Benzina_Approvato_Allevamenti, ")
            stb.AppendLine(" ISNULL(cgra.Carburante_Approvato,0) As Gasolio_Rendicontato_Approvato_Allevamenti, ")
            stb.AppendLine(" ISNULL(cbra.Carburante_Approvato, 0) As Benzina_Rendicontato_Approvato_Allevamenti,  ")

            'Approvato_Gasolio_Coltura
            stb.AppendLine(" CASE WHEN qp.Tipo_Richiesta = -1 THEN ISNULL(terzista_approvazione_gasolio,0) ")
            stb.AppendLine(" ELSE ISNULL(qp.Approvazione_Iniziale_Gasolio,0) End As Approvato_Gasolio_Coltura, ")

            'Approvato_Benzina_Coltura
            stb.AppendLine(" CASE WHEN qp.Tipo_Richiesta = -1 THEN ISNULL(terzista_approvazione_benzina,0) ")
            stb.AppendLine(" ELSE ISNULL(qp.Approvazione_Iniziale_Benzina,0) End As Approvato_Benzina_Coltura, ")

            'Richiesto_Approvato_Gasolio_Serra
            stb.AppendLine(" CASE WHEN qp.Tipo_Richiesta = -1 THEN ISNULL(terzista_approvazione_gasolio_serra,0) ")
            stb.AppendLine(" ELSE ISNULL(qp.Approvazione_Iniziale_Gasolio_Serra,0) End As Richiesto_Approvato_Gasolio_Serra, ")

            'Rendicontato approvato
            stb.AppendLine(" Rendicontato_Approvato_Gasolio, ")
            stb.AppendLine(" Rendicontato_Approvato_Benzina, ")
            stb.AppendLine(" Rendicontato_Approvato_Gasolio_Serra, ")

            'Numero lavorazioni
            stb.AppendLine(" Richiesto_Numero_Lavorazioni_Elettricita, ")
            stb.AppendLine(" Richiesto_Numero_Lavorazioni_Non_Agricolo, ")
            stb.AppendLine(" Numero_Lavorazioni_Elettricita As Rendicontato_Numero_Lavorazioni_Elettricita, ")
            stb.AppendLine(" Numero_Lavorazioni_Non_Agricolo As Rendicontato_Numero_Lavorazioni_Non_Agricolo, ")

            'Rimanenza iniziale
            stb.AppendLine(" Rimanenza_Iniziale_Gasolio, ")
            stb.AppendLine(" Rimanenza_Iniziale_Benzina, ")
            stb.AppendLine(" Rimanenza_Iniziale_Gasolio_Serra, ")

            'Rimanenza finale
            stb.AppendLine(" CASE WHEN (Rimanenza_Finale_Gasolio + Rimanenza_Finale_Gasolio_Rendicontazioni_Senza_Lavorazioni) > ISNULL(cgra.Rimanenza_Gasolio,0) ")
            stb.AppendLine(" THEN ISNULL(Rimanenza_Finale_Gasolio + Rimanenza_Finale_Gasolio_Rendicontazioni_Senza_Lavorazioni, 0) ")
            stb.AppendLine(" Else ISNULL(cgra.Rimanenza_Gasolio, 0) End As Rimanenza_Finale_Gasolio, ")
            stb.AppendLine(" Case When (Rimanenza_Finale_Benzina + Rimanenza_Finale_Benzina_Rendicontazioni_Senza_Lavorazioni) > ISNULL(cbra.Rimanenza_Benzina,0) ")
            stb.AppendLine(" THEN ISNULL(Rimanenza_Finale_Benzina + Rimanenza_Finale_Benzina_Rendicontazioni_Senza_Lavorazioni, 0) ")
            stb.AppendLine(" Else ISNULL(cbra.Rimanenza_Benzina, 0) End As Rimanenza_Finale_Benzina, ")
            stb.AppendLine(" Rimanenza_Finale_Gasolio_Serra + Rimanenza_Finale_Gasolio_Serra_Rendicontazioni_Senza_Lavorazioni As Rimanenza_Finale_Gasolio_Serra, ")

            'Altri campi...
            stb.AppendLine(" Conto, ")
            stb.AppendLine(" Tipo_Azienda, Tipo_Azienda_Des, ")
            stb.AppendLine(" Acquistato_Gasolio, ")
            stb.AppendLine(" Acquistato_Benzina, ")
            stb.AppendLine(" Acquistato_Gasolio_Serra, ")
            stb.AppendLine(" Anno, Data_Presentazione_Rendicontazione ")
            stb.AppendLine(" FROM ( ")

            'Query più interna 
            stb.AppendLine(" Select t.Anno, i.Rag_Soc, i.CUAA, t.piva, t.Tipo_Richiesta, ")

            'Anticipo Gasolio
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select f.fabbisogno_Richiesto from Lav_Avanzamento_Anticipo_Gasolio_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta  ")
            stb.AppendLine(" ) As Anticipo_Gasolio, ")

            'Anticipo Benzina
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select f.fabbisogno_Richiesto from Lav_Avanzamento_Anticipo_Benzina_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta  ")
            stb.AppendLine(" ) As Anticipo_Benzina, ")

            'Anticipo Gasolio Serra
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select f.fabbisogno_Richiesto from Lav_Avanzamento_Anticipo_GasolioSerra_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta  ")
            stb.AppendLine(" ) As Anticipo_Gasolio_Serra, ")

            'Richiesta iniziale Gasolio
            stb.AppendLine(" MAX(tsa.terzista_richiesta_gasolio) As terzista_richiesta_gasolio, ")
            stb.AppendLine(" SUM(t.Iniziale_Gasolio) As Iniziale_Gasolio, ")
            stb.AppendLine(" ISNULL(( ")
            stb.AppendLine(" Select f.fabbisogno_Richiesto from Lav_Avanzamento_0_Gasolio_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine(" ), 0) As Richiesta_Iniziale_Gasolio, ")

            'Richiesta iniziale Benzina
            stb.AppendLine(" MAX(tsa.terzista_richiesta_benzina) As terzista_richiesta_benzina, ")
            stb.AppendLine(" SUM(t.Iniziale_Benzina) As Iniziale_Benzina, ")
            stb.AppendLine(" ISNULL(( ")
            stb.AppendLine(" Select f.fabbisogno_Richiesto from Lav_Avanzamento_0_Benzina_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine(" ), 0) As Richiesta_Iniziale_Benzina, ")

            'Richiesta iniziale Gasolio Serra
            stb.AppendLine(" MAX(tsa.terzista_richiesta_gasolio_serra ) As terzista_richiesta_gasolio_serra, ")
            stb.AppendLine(" SUM(t.Iniziale_Gasolio_Serra) As Iniziale_Gasolio_Serra, ")
            stb.AppendLine(" ISNULL(( ")
            stb.AppendLine(" Select f.fabbisogno_Richiesto from Lav_Avanzamento_0_GasolioSerra_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine(" ), 0) As Richiesta_Iniziale_Gasolio_Serra, ")

            'Carburanti rendicontati
            stb.AppendLine(" ISNULL( (Select f.fabbisogno_Richiesto from Lav_Avanzamento_1_Gasolio_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta), 0 ) As Rendicontato_Gasolio,  ")
            stb.AppendLine(" ISNULL( (Select f.fabbisogno_Richiesto from Lav_Avanzamento_1_Benzina_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta), 0 ) As Rendicontato_Benzina,  ")
            stb.AppendLine(" ISNULL( (Select f.fabbisogno_Richiesto from Lav_Avanzamento_1_GasolioSerra_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta), 0 ) As Rendicontato_Gasolio_Serra,  ")

            'Gasolio Approvato in Richiesta
            stb.AppendLine(" MAX(tsa.terzista_approvazione_gasolio) As terzista_approvazione_gasolio, ")
            stb.AppendLine(" SUM(t.Approvazione_Iniziale_Gasolio) As Approvazione_Iniziale_Gasolio_Testata, ")
            stb.AppendLine(" ISNULL(( ")
            stb.AppendLine(" Select f.Fabbisogno_Assegnato from Lav_Avanzamento_0_Gasolio_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine(" ), 0) As Approvazione_Iniziale_Gasolio, ")

            'Benzina Approvata in Richiesta
            stb.AppendLine(" MAX(tsa.terzista_approvazione_benzina) As terzista_approvazione_benzina, ")
            stb.AppendLine(" SUM(t.Approvazione_Iniziale_Benzina) As Approvazione_Iniziale_Benzina_Testata, ")
            stb.AppendLine(" ISNULL(( ")
            stb.AppendLine(" Select f.Fabbisogno_Assegnato from Lav_Avanzamento_0_Benzina_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine(" ), 0) As Approvazione_Iniziale_Benzina, ")

            'Gasolio Serra Approvato in Richiesta
            stb.AppendLine(" MAX(tsa.terzista_approvazione_gasolio_serra) As terzista_approvazione_gasolio_serra, ")
            stb.AppendLine(" SUM(t.Approvazione_Iniziale_Gasolio_Serra) As Approvazione_Iniziale_Gasolio_Serra_Testata, ")
            stb.AppendLine(" ISNULL(( ")
            stb.AppendLine(" Select f.Fabbisogno_Assegnato from Lav_Avanzamento_0_GasolioSerra_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine(" ), 0) As Approvazione_Iniziale_Gasolio_Serra, ")

            'Carburanti rendicontati approvati
            stb.AppendLine(" isnull( (Select f.Fabbisogno_Assegnato from Lav_Avanzamento_1_Gasolio_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta), 0) As Rendicontato_Approvato_Gasolio,  ")
            stb.AppendLine(" isnull( (Select f.Fabbisogno_Assegnato from Lav_Avanzamento_1_Benzina_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta), 0) As Rendicontato_Approvato_Benzina,  ")
            stb.AppendLine(" isnull( (Select f.Fabbisogno_Assegnato from Lav_Avanzamento_1_GasolioSerra_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta), 0) As Rendicontato_Approvato_Gasolio_Serra,  ")

            'Elettricità e carburanti non agricoli per rendicontazioni e richieste
            stb.AppendLine(" isnull( (Select COUNT(*) from Lav_Avanzamento_0_Elettricita_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta), 0) As Richiesto_Numero_Lavorazioni_Elettricita, ")
            stb.AppendLine(" isnull( (Select COUNT(*) from Lav_Avanzamento_0_CarburanteNonAgricolo_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta), 0) As Richiesto_Numero_Lavorazioni_Non_Agricolo, ")
            stb.AppendLine(" isnull( (Select COUNT(*) from Lav_Avanzamento_1_Elettricita_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta), 0) As Numero_Lavorazioni_Elettricita, ")
            stb.AppendLine(" isnull( (Select COUNT(*) from Lav_Avanzamento_1_CarburanteNonAgricolo_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta), 0) As Numero_Lavorazioni_Non_Agricolo, ")

            'Carburanti acquistati
            stb.AppendLine(" isnull((Select SUM(v.Lt) from UMA_Vendite v (nolock)   ")
            stb.AppendLine(" WHERE v.PIVA_Cliente = t.Piva And v.Anno = t.anno And v.Conto_Proprio_Terzi = t.tipo_richiesta And v.tipo_Carburante = 2), 0) As Acquistato_Gasolio,   ")

            stb.AppendLine(" isnull((Select SUM(v.Lt) from UMA_Vendite v (nolock)   ")
            stb.AppendLine(" WHERE v.PIVA_Cliente = t.Piva And v.Anno = t.anno And v.Conto_Proprio_Terzi = t.tipo_richiesta And v.tipo_Carburante = 3), 0) As Acquistato_Benzina,   ")

            stb.AppendLine(" isnull((Select SUM(v.Lt) from UMA_Vendite v (nolock)   ")
            stb.AppendLine(" WHERE v.PIVA_Cliente = t.Piva And v.Anno = t.anno And v.Conto_Proprio_Terzi = t.tipo_richiesta And v.tipo_Carburante = 8), 0) As Acquistato_Gasolio_Serra,  ")

            'Rimanenze Iniziali
            stb.AppendLine(" ( ")
            stb.AppendLine(" MAX(t.Rimanenza_Gasolio) ")
            stb.AppendLine(" ) As Rimanenza_Iniziale_Gasolio,  ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" MAX(t.Rimanenza_Benzina) ")
            stb.AppendLine(" ) As Rimanenza_Iniziale_Benzina,  ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" MAX(t.Rimanenza_Gasolio_Serra) ")
            stb.AppendLine(" ) As Rimanenza_Iniziale_Gasolio_Serra,  ")

            'Rimanenze Finali
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select isnull(MAX(f.Rimanenza_Gasolio),0) from Lav_Avanzamento_1_Gasolio_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine(" ) As Rimanenza_Finale_Gasolio,  ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select isnull(MAX(r.Rimanenza_Gasolio),0) from Rendicontazioni_Senza_Lav_CTE r ")
            stb.AppendLine(" where t.piva = r.Piva And t.Tipo_Richiesta = r.Tipo_Richiesta ")
            stb.AppendLine(" ) As Rimanenza_Finale_Gasolio_Rendicontazioni_Senza_Lavorazioni, ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select isnull(MAX(f.Rimanenza_Benzina),0) from Lav_Avanzamento_1_Benzina_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine(" ) As Rimanenza_Finale_Benzina,  ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select isnull(MAX(r.Rimanenza_Benzina),0) from Rendicontazioni_Senza_Lav_CTE r ")
            stb.AppendLine(" where t.piva = r.Piva And t.Tipo_Richiesta = r.Tipo_Richiesta ")
            stb.AppendLine(" ) As Rimanenza_Finale_Benzina_Rendicontazioni_Senza_Lavorazioni, ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select isnull(MAX(f.Rimanenza_Gasolio_Serra),0) from Lav_Avanzamento_1_GasolioSerra_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta ")
            stb.AppendLine(" ) As Rimanenza_Finale_Gasolio_Serra,  ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select isnull(MAX(r.Rimanenza_Gasolio_Serra),0) from Rendicontazioni_Senza_Lav_CTE r ")
            stb.AppendLine(" where t.piva = r.Piva And t.Tipo_Richiesta = r.Tipo_Richiesta ")
            stb.AppendLine(" ) As Rimanenza_Finale_Gasolio_Serra_Rendicontazioni_Senza_Lavorazioni, ")

            stb.AppendLine(" FORMAT((Select MAX(f.Data_Presentazione_Rendicontazione) from Data_CTE f where f.piva = t.Piva And f.tipo_Richiesta = t.tipo_Richiesta), 'dd/MM/yyyy') as Data_Presentazione_Rendicontazione,   ")

            'Conto Proprio/Terzi
            stb.AppendLine(" Case When t.Tipo_Richiesta = 0 Then 'Conto Proprio' else 'Conto Terzi' END As Conto, ")
            stb.AppendLine(" COALESCE( t.Tipo_Azienda , " & enum_TipoAzienda_UMA.Azienda_Agricola_Privata & ") as Tipo_Azienda,")
            stb.AppendLine(" CASE t.Tipo_Azienda ")
            stb.AppendLine(" WHEN " & enum_TipoAzienda_UMA.Azienda_Terzista & " THEN '" & Azienda_Terzista & "'")
            stb.AppendLine(" WHEN " & enum_TipoAzienda_UMA.Cooperativa_Agricola & " THEN '" & Cooperativa_Agricola & "'")
            stb.AppendLine(" WHEN " & enum_TipoAzienda_UMA.Azienda_Agricola_Istituzioni_Pubbliche & " THEN '" & Azienda_Agricola_Istituzioni_Pubbliche & "'")
            stb.AppendLine(" WHEN " & enum_TipoAzienda_UMA.Consorzio_Bonifica_Irrigazione & " THEN '" & Consorzio_Bonifica_Irrigazione & "'")
            stb.AppendLine(" ELSE '" & Azienda_Agricola_Privata & "' END AS Tipo_Azienda_Des,")

            stb.AppendLine(" SUM(t.richiesta_Integrativa) As Integrative ")
            stb.AppendLine(" FROM Testa_CTE t ")
            stb.AppendLine(" JOIN Imprese_CTE i ON i.PIVA = t.Piva  ")
            stb.AppendLine(" LEFT JOIN Terzisti_Rich_Appr_Finale tsa on tsa.Piva = t.Piva ")
            stb.AppendLine(" join Indirizzi_CTE ind on ind.PIVA = t.Piva ")

            If filtroVisibilita Then
                stb.AppendLine(" Left Join Utenti_Visibilita_Appoggio (nolock) On t.Piva = Utenti_Visibilita_Appoggio.Piva  ")
                stb.AppendLine(" And Utenti_Visibilita_Appoggio.Entita_Cod=1  ")
            End If

            If FiltroNuovaVisibilita Then
                If Not VisibilitaTotale Then
                    If FiltroUtente AndAlso Not FiltroGruppoUtente Then
                        stb.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON t.Piva = visibilita.Piva_Azienda AND visibilita.Username = '" & objParametri_Utenti.UtenteUsername & "' ")
                    ElseIf Not FiltroUtente AndAlso FiltroGruppoUtente Then
                        stb.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON t.Piva = visibilita.Piva_Azienda AND visibilita.Gruppo = " & GruppoUtente & " ")
                    Else
                        stb.AppendLine(" INNER JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON t.Piva = visibilita.Piva_Azienda AND visibilita.Gruppo = -1 ")
                    End If
                End If
            End If

            stb.AppendLine(" where  1=1 ")
            If filtroVisibilita Then
                stb.AppendLine(" And Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & "  ")
            End If

            If FiltroNuovaVisibilita AndAlso Not VisibilitaTotale Then
                stb.Append(" AND (NOT visibilita.Piva_Azienda IS NULL) ")
            End If

            If piveVisibili.Length > 0 Then
                stb.AppendLine(" AND t.Piva IN ('" & Agro_SQL_Save_Clausola_IN(piveVisibili, True) & "')  ")
            End If

            If (piva <> "") Then
                stb.AppendLine(" AND t.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If (conto < 1) Then
                stb.AppendLine(" AND t.tipo_richiesta = " & conto.ToString & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            stb.AppendLine(" GROUP BY Anno, i.Rag_Soc, i.CUAA, t.piva, t.tipo_richiesta, t.Tipo_Azienda ) As qp ")
            stb.AppendLine(" LEFT JOIN Carburante_Gasolio_Richieste_Allevamenti_CTE cgr (nolock) ON qp.piva = cgr.piva AND qp.tipo_richiesta = cgr.Tipo_Richiesta ")
            stb.AppendLine(" LEFT JOIN Carburante_Benzina_Richieste_Allevamenti_CTE cbr (nolock) ON qp.piva = cbr.piva AND qp.tipo_richiesta = cbr.Tipo_Richiesta ")
            stb.AppendLine(" LEFT JOIN Carburante_Gasolio_Rendicontazione_Allevamenti_CTE cgra (nolock) ON qp.piva = cgra.piva AND qp.tipo_richiesta = cgra.Tipo_Richiesta ")
            stb.AppendLine(" LEFT JOIN Carburante_Benzina_Rendicontazione_Allevamenti_CTE cbra (nolock) ON qp.piva = cbra.piva AND qp.tipo_richiesta = cbra.Tipo_Richiesta ) as qs ")

            stb.AppendLine(" ) as qf ")

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy) & " ")
            Else
                stb.AppendLine(" order by qf.Rag_Soc ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function Leggi_Schema_Template(ByVal Id_Schema_Template As Integer,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.Leggi_Schema_Template()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * From Schema_Documenti_Template ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If (Id_Schema_Template <> 0) Then
                stb.AppendLine(" AND Id_Schema_Template = " & Id_Schema_Template & " ")
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

    Public Function CheckValiditaLavorazioniDaPratica(ByVal pratica_cod As Integer,
                                                      ByVal xFiltroAggiuntivo As String,
                                                      ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.CheckValiditaLavorazioniDaPratica()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        'Lettura chiave test UMA
        Dim objCfgSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim VCS_TestUMA = objCfgSiti.Leggi_Valore_JSON(Of AgronicaCoreVarieDAL.VCS_TestUMA)(objParametri)

        Try

            stb.Length = 0

            'RICHIESTE

            stb.AppendLine(" SELECT i.rag_soc, um.Macrouso_UMA_Des, ul.Lav_UMA_Des, l.* ")
            stb.AppendLine(" FROM Pratiche p ")
            stb.AppendLine(" join UMA_Richieste_Testata t on t.Pratica_Cod = p.Pratica_Cod")
            stb.AppendLine(" join UMA_Richieste_Lavorazioni l on l.Richiesta_Cod = t.Richiesta_Cod")
            stb.AppendLine(" join Imprese i on i.PIVA = l.Piva ")
            stb.AppendLine(" join UMA_Macrousi um on um.Macrouso_UMA_Cod = l.Gruppo_Colturale_UMA ")
            stb.AppendLine(" join UMA_Lavorazioni ul on ul.Lav_UMA_Cod = l.Lavorazione_UMA ")
            stb.AppendLine(" join UMA_Configurazione_MacrousixLavorazioni ml on ml.Lav_UMA_Cod = l.Lavorazione_UMA ")
            stb.AppendLine("                                                AND ml.Macrouso_UMA_Cod = l.Gruppo_Colturale_UMA ")
            stb.AppendLine("                                                AND ml.Regolamento_Cod IN (0, l.Regolamento_Cod) ")
            stb.AppendLine("                                                AND ml.Validita_Inizio <= t.Validita_Fine ")
            stb.AppendLine("                                                AND ml.Validita_Fine >= t.Validita_Inizio ")
            stb.AppendLine(" Where p.pratica_cod = " & pratica_cod.ToString & " AND t.Avanzamento_Richiesta = 0 ")

            Select Case VCS_TestUMA.Abilitato
                Case 1
                    stb.AppendLine(" AND ( ")
                    stb.AppendLine(" ( (NOT GETDATE() BETWEEN ml.Validita_Inizio And ml.Validita_Fine) AND GETDATE() BETWEEN t.Validita_Inizio AND t.Validita_Fine ) OR ")
                    stb.AppendLine(" ( (NOT DATEFROMPARTS(YEAR(t.Validita_Inizio)," & VCS_TestUMA.Mese.ToString & ",1) BETWEEN ml.Validita_Inizio AND ml.Validita_Fine) AND GETDATE() NOT BETWEEN t.Validita_Inizio AND t.Validita_Fine ) ")
                    stb.AppendLine(" ) ")
                Case 2
                    stb.AppendLine(" AND ( ")
                    stb.AppendLine("  ( NOT (GETDATE()       BETWEEN ml.Validita_Inizio AND ml.Validita_Fine) AND GETDATE()     BETWEEN t.Validita_Inizio AND t.Validita_Fine ) OR ")
                    stb.AppendLine("  ( NOT (t.Validita_Fine BETWEEN ml.Validita_Inizio AND ml.Validita_Fine) AND GETDATE() NOT BETWEEN t.Validita_Inizio AND t.Validita_Fine ) ")
                    stb.AppendLine(" ) ")
                Case Else
                    stb.AppendLine(" AND NOT (GETDATE() BETWEEN ml.Validita_Inizio AND ml.Validita_Fine)")
            End Select

            stb.AppendLine(" UNION")

            'RENDICONTAZIONI

            stb.AppendLine(" SELECT i.rag_soc, um.Macrouso_UMA_Des, ul.Lav_UMA_Des, l.* ")
            stb.AppendLine(" FROM Pratiche p ")
            stb.AppendLine(" join UMA_Richieste_Testata t on t.Pratica_Cod = p.Pratica_Cod")
            stb.AppendLine(" join UMA_Richieste_Lavorazioni l on l.Richiesta_Cod = t.Richiesta_Cod")
            stb.AppendLine(" join Imprese i on i.PIVA = l.Piva ")
            stb.AppendLine(" join UMA_Macrousi um on um.Macrouso_UMA_Cod = l.Gruppo_Colturale_UMA ")
            stb.AppendLine(" join UMA_Lavorazioni ul on ul.Lav_UMA_Cod = l.Lavorazione_UMA ")
            stb.AppendLine(" join UMA_Configurazione_MacrousixLavorazioni ml on ml.Lav_UMA_Cod = l.Lavorazione_UMA ")
            stb.AppendLine("                                                AND ml.Macrouso_UMA_Cod = l.Gruppo_Colturale_UMA ")
            stb.AppendLine("                                                AND ml.Regolamento_Cod IN (0, l.Regolamento_Cod) ")
            stb.AppendLine("                                                AND ml.Validita_Inizio <= t.Validita_Fine ")
            stb.AppendLine("                                                AND ml.Validita_Fine >= t.Validita_Inizio ")
            stb.AppendLine(" Where p.pratica_cod = " & pratica_cod.ToString & " AND t.Avanzamento_Richiesta = 1 AND NOT (l.Validita_Inizio BETWEEN ml.Validita_Inizio AND ml.Validita_Fine)")

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
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

    Public Function Leggi_Elenco_Con_Indirizzi(ByVal piva As String,
                                               ByVal anno As Integer,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri_Server As AgronicaCoreParametri,
                                               ByRef objParametri_Utenti As AgronicaCoreParametri,
                                               ByVal Filtro_Visibilita_Utente As Boolean,
                                               ByVal piveVisibili As String,
                                               ByVal rendicontazioni As Boolean,
                                               ByVal statoCod As Integer,
                                               ByVal citta As String,
                                               ByVal prov As String,
                                               ByVal conto As Integer,
                                               ByVal nuovaVisibilita As Boolean,
                                               ByVal FiltroUtente As Boolean,
                                               ByVal FiltroGruppoUtente As Boolean,
                                               ByVal GruppoUtente As Integer,
                                               ByVal VisibilitaTotale As Boolean
                                               ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.Leggi_Elenco()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        'Anna 12/08/21: Aggiunte due nuove colonne per UMA che necessitano JOIN con DB Utenti
        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2)
        NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

        'Lettura chiave test UMA
        Dim objCfgSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim VCS_TestUMA = objCfgSiti.Leggi_Valore_JSON(Of AgronicaCoreVarieDAL.VCS_TestUMA)(objParametri_Server)

        'VCS_TestUMA.SetIsolationLevelElencoPratiche = 1
        'VCS_TestUMA.UsaTempTableElencoPratiche = 1
        'VCS_TestUMA.LogCaricaElencoPratiche = 1

        Try

            stb.Length = 0

            If VCS_TestUMA.SetIsolationLevelElencoPratiche = 1 Then
                stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                stb.AppendLine("")
            End If

            Dim nomeTabUtenti = NomeDB_Utenti & ".dbo.utenti_dettagli"
            If VCS_TestUMA.UsaTempTableElencoPratiche = 1 Then
                '---
                Const nomeTempLav = "#TempLav"
                DropTempTableIfExists(stb, nomeTempLav)
                stb.AppendLine(" SELECT Richiesta_Cod, Tipo_Carburante, SUM(Fabbisogno_Calcolato) as Fabbisogno_Calcolato, SUM(Fabbisogno_Richiesto) as Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) as Fabbisogno_Assegnato ")
                stb.AppendLine(" INTO " & nomeTempLav)
                stb.AppendLine(" FROM UMA_Richieste_Lavorazioni ")
                stb.AppendLine(" GROUP BY Richiesta_Cod, Tipo_Carburante; ")
                stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_1_TempLav ON " & nomeTempLav & " (Richiesta_Cod, Tipo_Carburante); ")
                stb.AppendLine("")
                '---
                Const nomeTempLavParz = "#TempLavParz"
                DropTempTableIfExists(stb, nomeTempLavParz)
                stb.AppendLine(" SELECT Richiesta_Cod, Tipo_Carburante, SUM(Fabbisogno_Calcolato) as Fabbisogno_Calcolato, SUM(Fabbisogno_Richiesto) as Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) as Fabbisogno_Assegnato ")
                stb.AppendLine(" INTO " & nomeTempLavParz)
                stb.AppendLine(" FROM UMA_Lavorazioni_Parziali ")
                stb.AppendLine(" GROUP BY Richiesta_Cod, Tipo_Carburante; ")
                stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_1_TempLavParz ON " & nomeTempLavParz & " (Richiesta_Cod, Tipo_Carburante); ")
                stb.AppendLine("")
                '---
                Const nomeTempAllevamenti = "#TempAllevamenti"
                DropTempTableIfExists(stb, nomeTempAllevamenti)
                stb.AppendLine(" SELECT Richiesta_Cod, Tipo_Carburante, SUM(Carburante_Richiesto) as Carburante_Richiesto, SUM(Carburante_Calcolato) as Carburante_Calcolato, SUM(Carburante_Approvato) as Carburante_Approvato ")
                stb.AppendLine(" INTO " & nomeTempAllevamenti)
                stb.AppendLine(" FROM UMA_Richieste_Allevamenti ")
                stb.AppendLine(" GROUP BY Richiesta_Cod, Tipo_Carburante; ")
                stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_1_TempAllevamenti ON " & nomeTempAllevamenti & " (Richiesta_Cod, Tipo_Carburante); ")
                stb.AppendLine("")
                '---
                Const nomeTempDataRilascio = "#TempDataRilascio"
                DropTempTableIfExists(stb, nomeTempDataRilascio)
                stb.AppendLine("  SELECT ")
                stb.AppendLine("  t.richiesta_cod ")
                stb.AppendLine("  ,MIN(ps.Data_Creazione) Data_Rilascio ")
                stb.AppendLine("  ,COUNT(*) rk ")
                stb.AppendLine(" INTO " & nomeTempDataRilascio)
                stb.AppendLine("  FROM UMA_Richieste_Testata t ")
                stb.AppendLine("  INNER JOIN Pratiche_Stati_Attuali psa ON psa.Pratica_Cod = t.Pratica_Cod ")
                stb.AppendLine("  INNER JOIN Pratiche_Stati ps ON ps.Pratica_Cod = t.Pratica_Cod AND ps.Stato_Cod = 2007 ")
                stb.AppendLine("  WHERE Year(t.Validita_Inizio) = " & Agro_SQL_SaveNum(anno) & " ")
                If (rendicontazioni) Then
                    stb.AppendLine("  AND t.Avanzamento_Richiesta = 1 ")
                Else
                    stb.AppendLine("  AND t.Avanzamento_Richiesta = 0 ")
                End If
                stb.AppendLine("  AND psa.Stato_Cod NOT IN ('2006','2009') ")
                stb.AppendLine("  GROUP BY t.richiesta_cod ")
                stb.AppendLine(";")
                stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_1_TempDataRilascio ON " & nomeTempDataRilascio & " (Richiesta_Cod); ")
                stb.AppendLine("")
                '---
                Const nomeTempVerificaInCorso = "#TempVerificaInCorso"
                DropTempTableIfExists(stb, nomeTempVerificaInCorso)
                stb.AppendLine("  SELECT Pratiche.Pratica_Cod, ")
                stb.AppendLine("  (SELECT TOP 1 Username_Creazione FROM Pratiche_Stati psa WHERE psa.Pratica_Cod = Pratiche.Pratica_Cod AND psa.Stato_Cod = 2002 ORDER BY Data_Creazione DESC) as Username_Creazione ")
                stb.AppendLine(" INTO " & nomeTempVerificaInCorso)
                stb.AppendLine("  FROM Pratiche ")
                stb.AppendLine("  WHERE pratiche.Servizio_Cod = 2007 AND Pratiche.Anno = '" & Agro_SQL_SaveText(anno, False) & "'; ")
                stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_1_TempVerificaInCorso ON " & nomeTempVerificaInCorso & " (Pratica_Cod); ")
                stb.AppendLine("")
                '---
                If (rendicontazioni) Then
                    Const nomeTempRimanenze = "#TempRimanenze"
                    DropTempTableIfExists(stb, nomeTempRimanenze)
                    stb.AppendLine(" Select SUM(Rimanenza_Gasolio) as Rimanenza_Gasolio, SUM(Rimanenza_Benzina) as Rimanenza_Benzina, SUM(Rimanenza_Gasolio_Serra) as Rimanenza_Gasolio_Serra, UMA_Richieste_Testata.Piva, Tipo_Richiesta  ")
                    stb.AppendLine(" INTO " & nomeTempRimanenze)
                    stb.AppendLine(" From UMA_Richieste_Testata ")
                    stb.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
                    stb.AppendLine(" join Pratiche_Stati_Attuali psa ON pratiche.pratica_cod = psa.Pratica_Cod ")
                    stb.AppendLine(" where Avanzamento_Richiesta = 0 AND Pratiche.Anno = '" & Agro_SQL_SaveText(anno, False) & "' and psa.Stato_Cod between 2001 and 2005 ")
                    stb.AppendLine(" GROUP BY UMA_Richieste_Testata.Piva, Tipo_Richiesta; ")
                    stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_1_TempRimanenze ON " & nomeTempRimanenze & " (Piva, Tipo_Richiesta); ")
                    stb.AppendLine("")
                End If
                '---
                Const nomeTempUtenti = "#TempUtenti"
                DropTempTableIfExists(stb, nomeTempUtenti)
                stb.AppendLine(" SELECT UserName, CodFisc, Cognome, Nome, Rag_Soc ")
                stb.AppendLine(" INTO " & nomeTempUtenti)
                stb.AppendLine(" FROM " & nomeTabUtenti & " (NOLOCK); ")
                stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_1_TempUtenti ON " & nomeTempUtenti & " (UserName); ")
                stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_2_TempUtenti ON " & nomeTempUtenti & " (CodFisc); ")
                nomeTabUtenti = nomeTempUtenti
                '---
                Const nomeTempVendite = "#TempVendite"
                DropTempTableIfExists(stb, nomeTempVendite)
                stb.AppendLine(" SELECT PIVA_Cliente, Tipo_Carburante, Conto_Proprio_Terzi, SUM(lt) lt ")
                stb.AppendLine(" INTO " & nomeTempVendite)
                stb.AppendLine(" FROM UMA_Vendite WHERE Anno = " & Agro_SQL_SaveNum(anno) & " ")
                stb.AppendLine(" GROUP BY PIVA_Cliente, Tipo_Carburante, Conto_Proprio_Terzi; ")
                stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_1_TempVendite On " & nomeTempVendite & " (PIVA_Cliente, Tipo_Carburante, Conto_Proprio_Terzi); ")
            Else
                AppendWithCteTempLav(stb, True)
                AppendCteTempLavParz(stb)
                AppendCteAllevamenti(stb)
                '---
                stb.AppendLine("  ,#TempVerificaInCorso As( ")
                stb.AppendLine("  Select Pratiche.Pratica_Cod,  ")
                stb.AppendLine("  (Select TOP 1 Username_Creazione FROM Pratiche_Stati psa WHERE psa.Pratica_Cod = Pratiche.Pratica_Cod And psa.Stato_Cod = 2002 ORDER BY Data_Creazione DESC) As Username_Creazione ")
                stb.AppendLine("  FROM Pratiche  ")
                stb.AppendLine("  WHERE pratiche.Servizio_Cod = 2007 And Pratiche.Anno = '" & Agro_SQL_SaveText(anno, False) & "' ) ")
                '---
                stb.AppendLine("  ,#TempDataRilascio as( ")
                stb.AppendLine("  SELECT ")
                stb.AppendLine("  t.richiesta_cod ")
                stb.AppendLine("  ,MIN(ps.Data_Creazione) Data_Rilascio ")
                stb.AppendLine("  ,COUNT(*) rk ")
                stb.AppendLine("  FROM UMA_Richieste_Testata t ")
                stb.AppendLine("  INNER JOIN Pratiche_Stati_Attuali psa ON psa.Pratica_Cod = t.Pratica_Cod ")
                stb.AppendLine("  INNER JOIN Pratiche_Stati ps ON ps.Pratica_Cod = t.Pratica_Cod AND ps.Stato_Cod = 2007 ")
                stb.AppendLine("  WHERE Year(t.Validita_Inizio) = " & Agro_SQL_SaveNum(anno) & " ")
                If (rendicontazioni) Then
                    stb.AppendLine("  AND t.Avanzamento_Richiesta = 1 ")
                Else
                    stb.AppendLine("  AND t.Avanzamento_Richiesta = 0 ")
                End If
                stb.AppendLine("  AND psa.Stato_Cod NOT IN ('2006','2009') ")
                stb.AppendLine("  GROUP BY t.richiesta_cod ")
                stb.AppendLine(")")
                '---
                If (rendicontazioni) Then
                    stb.AppendLine(" ,#TempRimanenze as( ")
                    stb.AppendLine(" Select SUM(Rimanenza_Gasolio) as Rimanenza_Gasolio, SUM(Rimanenza_Benzina) as Rimanenza_Benzina, SUM(Rimanenza_Gasolio_Serra) as Rimanenza_Gasolio_Serra, UMA_Richieste_Testata.Piva, Tipo_Richiesta  ")
                    stb.AppendLine(" From UMA_Richieste_Testata ")
                    stb.AppendLine(" JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
                    stb.AppendLine(" join Pratiche_Stati_Attuali psa ON pratiche.pratica_cod = psa.Pratica_Cod ")
                    stb.AppendLine(" where Avanzamento_Richiesta = 0 AND Pratiche.Anno = '" & Agro_SQL_SaveText(anno, False) & "' and psa.Stato_Cod between 2001 and 2005 ")
                    stb.AppendLine(" GROUP BY UMA_Richieste_Testata.Piva, Tipo_Richiesta) ")
                End If
            End If

            stb.AppendLine(" SELECT DISTINCT x.* FROM ( ")
            stb.AppendLine(" SELECT CASE WHEN ISNULL(imp.partitaIvaReale, '') = '' THEN t.Piva ELSE imp.partitaIvaReale END AS ID, t.Piva as piva, imp.rag_soc As azienda, imp.PartitaIvaReale, ic.val_cod AS CUAA, p.Numero AS nRichiesta, p.Pratica_Cod, p.Anno AS annoRichiesta, psa.stato_cod AS CodstatoAv, psaw.WAnagraficaStati_Des AS statoAv, ")
            stb.AppendLine(" ISTAT.Localita AS citta, i.ind_des AS via, i.CAP, i.pro_cod AS Prov, t.Data_Creazione  ")
            stb.AppendLine(" ,psa.Validita_Inizio As ultimo_Avanzamento, t.Validita_Inizio AS val_Inizio, t.Validita_Fine AS val_Fine, t.richiesta_Cod AS richiestaCod, ")

            'Carburante calcolato
            stb.AppendLine(" ISNULL(ul_gas.Fabbisogno_Calcolato, 0) + ISNULL(ul_gas_Parz.Fabbisogno_Calcolato, 0) + ISNULL(uag.Carburante_Calcolato, 0) as calcolato_Gasolio, ")
            stb.AppendLine(" ISNULL(ul_benz.Fabbisogno_Calcolato, 0) + ISNULL(ul_benz_Parz.Fabbisogno_Calcolato, 0) + ISNULL(uab.Carburante_Calcolato, 0) as calcolato_Benzina, ")
            stb.AppendLine(" ISNULL(ul_serra.Fabbisogno_Calcolato, 0) + ISNULL(ul_serra_Parz.Fabbisogno_Calcolato, 0)  as calcolato_Gasolio_Serra, ")

            'Carburante richiesto
            stb.AppendLine(" ISNULL(CASE ")
            stb.AppendLine("       WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Richiesta_Iniziale_Gasolio > 0 THEN CASE WHEN t.Richiesta_Iniziale_Gasolio > (ISNULL(ul_gas.Fabbisogno_Richiesto, 0) + ISNULL(ul_gas_Parz.Fabbisogno_Richiesto, 0)) THEN t.Richiesta_Iniziale_Gasolio ELSE (ISNULL(ul_gas.Fabbisogno_Richiesto, 0) + ISNULL(ul_gas_Parz.Fabbisogno_Richiesto, 0)) END ")
            stb.AppendLine("       ELSE (ISNULL(ul_gas.Fabbisogno_Richiesto, 0) + ISNULL(ul_gas_Parz.Fabbisogno_Richiesto, 0)) ")
            stb.AppendLine(" END, 0) + ISNULL(uag.Carburante_Richiesto, 0) as richiesto_Gasolio, ")
            stb.AppendLine(" ISNULL(CASE ")
            stb.AppendLine("        WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Richiesta_Iniziale_Benzina > 0 THEN t.Richiesta_Iniziale_Benzina ")
            stb.AppendLine("        ELSE (ISNULL(ul_benz.Fabbisogno_Richiesto, 0) + ISNULL(ul_benz_Parz.Fabbisogno_Richiesto, 0)) ")
            stb.AppendLine(" END, 0) + ISNULL(uab.Carburante_Richiesto, 0) as richiesto_Benzina, ")
            stb.AppendLine(" ISNULL(CASE ")
            stb.AppendLine("        WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Richiesta_Iniziale_Gasolio_Serra > 0 THEN t.Richiesta_Iniziale_Gasolio_Serra ")
            stb.AppendLine(" ELSE (ISNULL(ul_serra.Fabbisogno_Richiesto, 0) + ISNULL(ul_serra_Parz.Fabbisogno_Richiesto, 0))  ")
            stb.AppendLine(" END, 0) as richiesto_Gasolio_Serra, ")

            'Carburante assegnato
            AppendCarburanteAssegnato(stb)

            'Carburante allevamenti
            stb.AppendLine(" ISNULL(uag.Carburante_Calcolato, 0) as Carburante_Calcolato_Gasolio_Allevamenti, ")
            stb.AppendLine(" ISNULL(uag.Carburante_Richiesto, 0) as Carburante_Richiesto_Gasolio_Allevamenti, ")
            stb.AppendLine(" ISNULL(uag.Carburante_Approvato, 0) as Carburante_Approvato_Gasolio_Allevamenti, ")
            stb.AppendLine(" ISNULL(uab.Carburante_Calcolato, 0) as Carburante_Calcolato_Benzina_Allevamenti, ")
            stb.AppendLine(" ISNULL(uab.Carburante_Richiesto, 0) as Carburante_Richiesto_Benzina_Allevamenti, ")
            stb.AppendLine(" ISNULL(uab.Carburante_Approvato, 0) as Carburante_Approvato_Benzina_Allevamenti, ")

            'Tipo richiesta
            stb.AppendLine(" IIf(t.Tipo_Richiesta = 0, IIf(t.Avanzamento_Richiesta = -1, 'Anticipo Conto Proprio', 'Conto Proprio'), IIf(t.Avanzamento_Richiesta = -1, 'Anticipo Conto Terzi', 'Conto Terzi')) AS tipo_richiesta,  ")
            stb.AppendLine(" IIf(t.Avanzamento_Richiesta = -1, 'Anticipo', IIf(t.Richiesta_Integrativa = 1, 'Richiesta Integrativa', 'Prima Richiesta' )) AS Prima_Richiesta,  ")

            'Anna 12/08/21: Aggiunte due nuove colonne per UMA
            stb.AppendLine(" CASE WHEN richiedente.Nome = '' AND Richiedente.cognome = ''  ")
            stb.AppendLine("    THEN (' [' + richiedente.Rag_Soc + ']') ")
            stb.AppendLine(" ELSE (' [' + richiedente.Nome + ' ' + Richiedente.cognome +']') ")
            stb.AppendLine(" END AS Richiedente,  ")

            stb.AppendLine(" CASE WHEN psa.Stato_Cod BETWEEN 2002 AND 2006 THEN ")
            stb.AppendLine(" CASE WHEN Approvatoret.UserName IS NOT NULL THEN (' [' + Approvatoret.Nome + ' ' + Approvatoret.cognome +']')  ")
            stb.AppendLine(" ELSE (' [' + Approvatorep.Nome + ' ' + Approvatorep.cognome +']') END  ")
            stb.AppendLine(" END AS Approvatore,  ")
            stb.AppendLine(" COALESCE( t.Tipo_Azienda , 0) as Tipo_Azienda, ")
            stb.AppendLine(" CASE t.Tipo_Azienda ")
            stb.AppendLine(" WHEN 2 THEN '" & Azienda_Terzista & "'")
            stb.AppendLine(" WHEN 3 THEN '" & Cooperativa_Agricola & "'")
            stb.AppendLine(" WHEN 4 THEN '" & Azienda_Agricola_Istituzioni_Pubbliche & "'")
            stb.AppendLine(" WHEN 5 THEN '" & Consorzio_Bonifica_Irrigazione & "'")
            stb.AppendLine(" ELSE '" & Azienda_Agricola_Privata & "' END AS Tipo_Azienda_Des")

            If (rendicontazioni) Then
                stb.AppendLine(" , '' As 'Litri_In_Esubero', ")
                If VCS_TestUMA.UsaTempTableElencoPratiche = 1 Then
                    stb.AppendLine(" ROUND(ISNULL(uvg.lt, 0) + ")
                    stb.AppendLine(" ISNULL(tr.Rimanenza_Gasolio, 0) - ")
                    stb.AppendLine(" (t.Rimanenza_Gasolio), 0) as Acquistato_e_Rimanenza_Gasolio, ")
                    stb.AppendLine(" ROUND(ISNULL(uvb.lt, 0) + ")
                    stb.AppendLine(" ISNULL(tr.Rimanenza_Benzina, 0) - ")
                    stb.AppendLine(" (t.Rimanenza_Benzina), 0) as Acquistato_e_Rimanenza_Benzina, ")
                    stb.AppendLine(" ROUND(ISNULL(uvs.lt, 0) + ")
                    stb.AppendLine(" ISNULL(tr.Rimanenza_Gasolio_Serra, 0) - ")
                    stb.AppendLine(" (t.Rimanenza_Gasolio_Serra), 0) as Acquistato_e_Rimanenza_Gasolio_Serra, ")
                Else
                    stb.AppendLine(" ROUND(ISNULL((Select SUM(uv.lt) From UMA_Vendite uv where uv.PIVA_Cliente = t.Piva AND uv.Tipo_Carburante = 2 AND uv.Conto_Proprio_Terzi = t.Tipo_Richiesta AND uv.Anno = " + Agro_SQL_SaveNum(anno) + "), 0) + ")
                    stb.AppendLine(" ISNULL((Select tr.Rimanenza_Gasolio From #TempRimanenze tr where tr.Piva = t.Piva And tr.Tipo_Richiesta = t.Tipo_Richiesta),0) - ")   'gloria aggiunto ISNULL
                    stb.AppendLine(" (t.Rimanenza_Gasolio), 0) as Acquistato_e_Rimanenza_Gasolio, ")
                    stb.AppendLine(" ROUND(ISNULL((Select SUM(uv.lt) From UMA_Vendite uv where uv.PIVA_Cliente = t.Piva AND uv.Tipo_Carburante = 3 AND uv.Conto_Proprio_Terzi = t.Tipo_Richiesta AND uv.Anno = " + Agro_SQL_SaveNum(anno) + "), 0) + ")
                    stb.AppendLine(" ISNULL((Select tr.Rimanenza_Benzina From #TempRimanenze tr where tr.Piva = t.Piva And tr.Tipo_Richiesta = t.Tipo_Richiesta),0) - ") 'gloria aggiunto ISNULL
                    stb.AppendLine(" (t.Rimanenza_Benzina), 0) as Acquistato_e_Rimanenza_Benzina, ")
                    stb.AppendLine(" ROUND(ISNULL((Select SUM(uv.lt) From UMA_Vendite uv where uv.PIVA_Cliente = t.Piva AND uv.Tipo_Carburante = 8 AND uv.Conto_Proprio_Terzi = t.Tipo_Richiesta AND uv.Anno = " + Agro_SQL_SaveNum(anno) + "), 0) + ")
                    stb.AppendLine(" ISNULL((Select tr.Rimanenza_Gasolio_Serra From #TempRimanenze tr where tr.Piva = t.Piva And tr.Tipo_Richiesta = t.Tipo_Richiesta),0) - ")   'gloria aggiunto ISNULL
                    stb.AppendLine(" (t.Rimanenza_Gasolio_Serra), 0) as Acquistato_e_Rimanenza_Gasolio_Serra, ")
                End If
                stb.AppendLine(" ROUND(((ISNULL((ISNULL(ul_gas.Fabbisogno_Richiesto, 0) + ISNULL(uag.Carburante_Richiesto, 0)) * ((100 - uset.Per_Riduzione) / 100), 0))), 0) AS Richiesto_Netto_Gasolio,  ")
                stb.AppendLine(" ROUND(((ISNULL((ISNULL(ul_benz.Fabbisogno_Richiesto, 0) + ISNULL(uab.Carburante_Richiesto, 0)) * ((100 - uset.Per_Riduzione) / 100), 0))), 0) AS Richiesto_Netto_Benzina,  ")
                stb.AppendLine(" ROUND(((ISNULL((ISNULL(ul_serra.Fabbisogno_Richiesto, 0)) * ((100 - uset.Per_Riduzione) / 100), 0))), 0) AS Richiesto_Netto_Gasolio_Serra  ")
            Else
                stb.AppendLine(", ISNULL(pOrigin.Numero, '') as Rendicontazine_Originale  ")
            End If

            stb.AppendLine(", ISNULL(t.Rec_Acc_Conf_Gasolio, 0) as Rec_Acc_Conf_Gasolio ")
            stb.AppendLine(", ISNULL(t.Rec_Acc_Conf_Benzina, 0) as Rec_Acc_Conf_Benzina ")
            stb.AppendLine(", ISNULL(t.Rec_Acc_Conf_Gasolio_Serra, 0) as Rec_Acc_Conf_Gasolio_Serra ")

            stb.AppendLine(", dtril.Data_Rilascio as Data_Rilascio")

            'From

            stb.AppendLine(" FROM UMA_Richieste_Testata t ")

            'Join #CTE
            AppendJoinCteTemp(stb)

            'Join #Temp
            If rendicontazioni AndAlso VCS_TestUMA.UsaTempTableElencoPratiche = 1 Then
                stb.AppendLine(" left join #TempVendite uvg ON uvg.PIVA_Cliente = t.Piva AND uvg.Tipo_Carburante = 2 AND uvg.Conto_Proprio_Terzi = t.Tipo_Richiesta ")
                stb.AppendLine(" left join #TempVendite uvb ON uvb.PIVA_Cliente = t.Piva AND uvb.Tipo_Carburante = 3 AND uvb.Conto_Proprio_Terzi = t.Tipo_Richiesta ")
                stb.AppendLine(" left join #TempVendite uvs ON uvs.PIVA_Cliente = t.Piva AND uvs.Tipo_Carburante = 8 AND uvs.Conto_Proprio_Terzi = t.Tipo_Richiesta ")
                stb.AppendLine(" left join #TempRimanenze tr ON tr.Piva = t.Piva And tr.Tipo_Richiesta = t.Tipo_Richiesta ")
            End If

            'Altre join
            stb.AppendLine(" join Imprese imp on imp.Piva = t.Piva ")
            stb.AppendLine(" join Pratiche p on t.Piva = p.Piva And t.Pratica_Cod = p.Pratica_Cod ")
            stb.AppendLine(" left join Pratiche_Stati ps on t.Pratica_Cod = ps.Pratica_Cod AND ps.stato_Cod BETWEEN 2003 AND 2006 ")
            stb.AppendLine(" join Pratiche_Stati_Attuali psa on p.Piva_SuperUser = psa.Piva_SuperUser And p.Pratica_Cod = psa.Pratica_Cod ")
            stb.AppendLine(" join WAnagraficaStati psaw on psa.Stato_Cod = psaw.WAnagraficaStati_Cod ")
            stb.AppendLine(" join Imprese_Codici ic on t.Piva = ic.PIVA And ic.id_cod = 1010 ")
            stb.AppendLine(" INNER JOIN ImpresexIndirizzi ixi ON t.Piva = ixi.Piva ")
            stb.AppendLine(" INNER JOIN Indirizzi i ON ixi.Cod_Indirizzo = i.Cod_Indirizzo  ")

            If (rendicontazioni) Then
                stb.AppendLine(" Join UMA_Setup uset On uset.Anno = '" & Agro_SQL_SaveText(anno, False) & "' ")
            End If

            stb.AppendLine(" LEFT JOIN ISTAT ON i.pro_cod_istat = ISTAT.PROV And i.com_cod_istat = ISTAT.COM ")

            stb.AppendLine(" LEFT JOIN #TempVerificaInCorso appr ON t.Pratica_Cod = appr.Pratica_Cod ")

            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" LEFT JOIN Utenti_Visibilita_Appoggio (NOLOCK) On imp.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1 AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & " ")
            End If

            If nuovaVisibilita AndAlso Not VisibilitaTotale Then
                If FiltroUtente AndAlso Not FiltroGruppoUtente Then
                    stb.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON t.Piva = visibilita.Piva_Azienda AND visibilita.Username = '" & objParametri_Utenti.UtenteUsername & "' ")
                ElseIf Not FiltroUtente AndAlso FiltroGruppoUtente Then
                    stb.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON t.Piva = visibilita.Piva_Azienda AND visibilita.Gruppo = " & GruppoUtente & " ")
                Else
                    stb.AppendLine(" INNER JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON t.Piva = visibilita.Piva_Azienda AND visibilita.Gruppo = -1 ")
                End If
            End If

            'Anna 12/08/21: Aggiunte due nuove colonne per UMA
            stb.AppendLine(" LEFT JOIN " & nomeTabUtenti & " Richiedente  ON Richiedente.UserName = t.Username_Creazione")
            stb.AppendLine(" LEFT JOIN " & nomeTabUtenti & " Approvatoret ON appr.UserName_Creazione = Approvatoret.Username ")
            stb.AppendLine(" LEFT JOIN " & nomeTabUtenti & " Approvatorep ON appr.UserName_Creazione = Approvatorep.CodFisc ")

            If (Not rendicontazioni) Then
                stb.AppendLine(" LEFT JOIN UMA_Richieste_Testata origin ON t.Richiesta_Origine_Cod = origin.Richiesta_Cod ")
                stb.AppendLine(" LEFT JOIN Pratiche pOrigin ON origin.Pratica_Cod = pOrigin.Pratica_Cod ")
            End If

            stb.AppendLine(" LEFT JOIN #TempDataRilascio dtril ON dtril.richiesta_cod = t.richiesta_cod ")

            stb.AppendLine(" WHERE ")

            If piva <> "" Then
                stb.AppendLine(" t.piva = '" & Agro_SQL_SaveText(piva, False) & "' ")
                stb.AppendLine(" AND t.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser, False) & "' AND ")
            End If

            stb.AppendLine(" t.Avanzamento_Richiesta " & If(rendicontazioni, " = 1 ", " IN (-1, 0) "))

            If (anno > 0) Then
                stb.AppendLine(" AND p.Anno = '" & Agro_SQL_SaveText(anno, False) & "' ")
            End If

            If (statoCod > 0) Then
                stb.AppendLine(" AND psaw.WAnagraficaStati_Cod = " + statoCod.ToString + " ")
            End If

            If (conto < 1) Then
                stb.AppendLine(" AND t.tipo_richiesta = " + conto.ToString + " ")
            End If

            If (citta <> "-1" AndAlso citta <> "") Then
                stb.AppendLine(" And ISTAT.COM = '" + Agro_SQL_SaveText(citta, False) + "' ")
            End If

            If (prov <> "-1") Then
                stb.AppendLine(" AND ISTAT.PROV = '" + Agro_SQL_SaveText(prov, False) + "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " + Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, False) + " ")
            End If

            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername, False) & "  ")
            End If

            If nuovaVisibilita AndAlso Not VisibilitaTotale AndAlso ((FiltroUtente And Not FiltroGruppoUtente) OrElse
                (Not FiltroUtente And FiltroGruppoUtente)) Then
                stb.Append(" AND (NOT visibilita.Piva_Azienda IS NULL) ")
            End If

            If piveVisibili.Length > 0 Then
                stb.AppendLine(" AND t.Piva IN ('" & Agro_SQL_SaveText(piveVisibili) & "')  ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " + Agro_SQL_Save_xOrderBy(xOrderBy) + " ")
            End If

            stb.AppendLine(" ) As x ")
            stb.AppendLine(" where (x.CodstatoAv between 2003 and 2006) OR x.CodstatoAv IN (2001, 2002, 2007, 2008, 2009, 2010) ")

            If VCS_TestUMA.SetIsolationLevelElencoPratiche = 1 Then
                stb.AppendLine(" ; ")
                stb.AppendLine("")
                stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ COMMITTED; ")
            End If
            Dim startTime As Long = DateTime.Now.Ticks
            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If VCS_TestUMA.LogCaricaElencoPratiche = 1 Then
                Dim endTime As Long = DateTime.Now.Ticks
                Dim nomeDirLogCaricaElencoPratiche = objParametri_Server.LogDirectory + "\CarburantiUMA"
                Dim nomeFileLogCaricaElencoPratiche = String.Format("Log_CaricaElencoPratiche_{0}_{1}.txt",
                                                                    Now().ToString("yyyy"),
                                                                    Now().ToString("MM"))
                Dim durata = Math.Round(CDec((endTime - startTime) / TimeSpan.TicksPerSecond), 0)
                Dim messaggioLog = String.Format("Carica {0} {1} - Righe: {2} - Durata: {3} [Isolation={4} UsaTemp={5}]",
                                                 If(rendicontazioni, "Rendicontazioni", "Richieste"),
                                                 anno,
                                                 dt.Rows.Count(),
                                                 durata,
                                                 VCS_TestUMA.SetIsolationLevelElencoPratiche,
                                                 VCS_TestUMA.UsaTempTableElencoPratiche)
                Dim customLOGParams As New CustomLOGParams With {
                    .LogDescrizioneUtente = objParametri_Server.LogDescrizioneUtente,
                    .LogDirectory = nomeDirLogCaricaElencoPratiche,
                    .LogFileName = nomeFileLogCaricaElencoPratiche
                }
                Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioLog, CustomLOGParams:=CustomLOGParams)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Ricerca_Macrousi_Lavorazioni(anno As Integer,
                                                 xFiltroAggiuntivo As String,
                                                 xOrderBy As String,
                                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                 Filtro_Visibilita_Utente As Boolean,
                                                 piveVisibili As String,
                                                 rendicontazioni As Integer,
                                                 statoCod As Integer,
                                                 citta As String,
                                                 prov As String,
                                                 approvatore As String,
                                                 causali As String,
                                                 dettaglio As Integer,
                                                 colture As Integer,
                                                 lavorazione As Integer,
                                                 allevamento As String,
                                                 conto As Integer,
                                                 nuovaVisibilita As Boolean,
                                                 FiltroUtente As Boolean,
                                                 FiltroGruppoUtente As Boolean,
                                                 GruppoUtente As Integer,
                                                 VisibilitaTotale As Boolean
                                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.Leggi_Elenco()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        'Anna 12/08/21: Aggiunte due nuove colonne per UMA che necessitano JOIN con DB Utenti
        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2)
        NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

        'Lettura chiave test UMA
        Dim objCfgSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim lavo As String = "(SELECT TOP 1 Lav_UMA_Des FROM UMA_Lavorazioni WHERE Lav_UMA_Cod = ISNULL(ul_gasD.Lavorazione_UMA, ISNULL(ul_benzD.Lavorazione_UMA,ISNULL(ul_serraD.Lavorazione_UMA,ISNULL(ul_gas_ParzD.Lavorazione_UMA,ISNULL(ul_benz_ParzD.Lavorazione_UMA,ISNULL(ul_serra_ParzD.Lavorazione_UMA,'')))))))"
        Dim alleva As String = "(SELECT TOP 1 UMA_All_Des FROM UMA_Allevamenti WHERE UMA_All_Cod = ISNULL(uagD.UMA_All_Cod, ISNULL(uabD.UMA_All_Cod,'')))"
        Dim coltu As String = "(SELECT TOP 1 Macrouso_UMA_Des FROM UMA_Macrousi WHERE Macrouso_UMA_Cod = ISNULL(ul_gasD.Gruppo_Colturale_UMA, ISNULL(ul_benzD.Gruppo_Colturale_UMA,ISNULL(ul_serraD.Gruppo_Colturale_UMA,''))))"


        Try

            stb.Length = 0

            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
            stb.AppendLine("")

            Dim nomeTabUtenti = NomeDB_Utenti + ".dbo.utenti_dettagli"

            '---
            TempTableLav(stb, "#TempLav", 0, 0)

            If dettaglio > 0 AndAlso (colture > 0 OrElse lavorazione > 0) Then
                TempTableLav(stb, "#TempLavDettagli", colture, lavorazione)
            End If
            '---
            TempTableLavParz(stb, "#TempLavParz", 0, 0)

            If dettaglio > 0 AndAlso (colture > 0 OrElse lavorazione > 0) Then
                TempTableLavParz(stb, "#TempLavParzDettagli", colture, lavorazione)
            End If
            '--- 
            TempTableAllevamenti(stb, "#TempAllevamenti", "0")

            If dettaglio > 0 AndAlso (colture > 0 OrElse allevamento <> "0") Then
                TempTableAllevamenti(stb, "#TempAllevamentiDettagli", allevamento)
            End If
            '---
            Const nomeTempVerificaInCorso = "#TempVerificaInCorso"
            DropTempTableIfExists(stb, nomeTempVerificaInCorso)
            stb.AppendLine("  SELECT Pratiche.Pratica_Cod, ")
            stb.AppendLine("  (SELECT TOP 1 Username_Creazione FROM Pratiche_Stati psa WHERE psa.Pratica_Cod = Pratiche.Pratica_Cod ORDER BY Data_Creazione DESC) as Username_Creazione ")
            stb.AppendLine(" INTO " + nomeTempVerificaInCorso)
            stb.AppendLine("  FROM Pratiche ")
            stb.AppendLine("  WHERE pratiche.Servizio_Cod = 2007 AND Pratiche.Anno = '" & Agro_SQL_SaveText(anno, False) & "'; ")
            stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_1_TempVerificaInCorso ON " + nomeTempVerificaInCorso + " (Pratica_Cod); ")
            stb.AppendLine("")

            Const nomeTempUtenti = "#TempUtenti"
            DropTempTableIfExists(stb, nomeTempUtenti)
            stb.AppendLine(" SELECT UserName, CodFisc, Cognome, Nome, Rag_Soc ")
            stb.AppendLine(" INTO " + nomeTempUtenti)
            stb.AppendLine(" FROM " + nomeTabUtenti + " (NOLOCK); ")
            stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_1_TempUtenti ON " + nomeTempUtenti + " (UserName); ")
            stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_2_TempUtenti ON " + nomeTempUtenti + " (CodFisc); ")
            nomeTabUtenti = nomeTempUtenti
            '---
            Const nomeTempVendite = "#TempVendite"
            DropTempTableIfExists(stb, nomeTempVendite)
            stb.AppendLine(" SELECT PIVA_Cliente, Tipo_Carburante, Conto_Proprio_Terzi, SUM(lt) lt ")
            stb.AppendLine(" INTO " + nomeTempVendite)
            stb.AppendLine(" FROM UMA_Vendite WHERE Anno = " + Agro_SQL_SaveNum(anno) + " ")
            stb.AppendLine(" GROUP BY PIVA_Cliente, Tipo_Carburante, Conto_Proprio_Terzi; ")
            stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_1_TempVendite On " + nomeTempVendite + " (PIVA_Cliente, Tipo_Carburante, Conto_Proprio_Terzi); ")

            stb.AppendLine(" SELECT DISTINCT x.* FROM ( ")
            stb.AppendLine(" SELECT CASE WHEN ISNULL(imp.partitaIvaReale, '') = '' THEN t.Piva ELSE imp.partitaIvaReale END AS ID, t.Piva as piva, imp.rag_soc As azienda, ic.val_cod AS CUAA, p.Numero AS nRichiesta, p.Pratica_Cod, p.Anno AS annoRichiesta, psa.stato_cod AS CodstatoAv, psaw.WAnagraficaStati_Des AS statoAv, ")
            stb.AppendLine(" ISTAT.Localita AS citta, i.ind_des AS via, i.CAP, i.pro_cod AS Prov, t.Data_Creazione  ")
            stb.AppendLine(" ,CASE WHEN t.Avanzamento_Richiesta = 0 THEN 'Richiesta' ELSE 'Rendicontazione' END AS Tipo_Pratica ")
            stb.AppendLine(" ,psa.Validita_Inizio As ultimo_Avanzamento, t.Validita_Inizio AS val_Inizio, t.Validita_Fine AS val_Fine, t.richiesta_Cod AS richiestaCod, ")

            'Carburante calcolato
            Carb_Calcolato_Macro_Lav(stb, 0, 0, "0")

            'Carburante richiesto
            Carb_Richiesto_Macro_Lav(stb, 0, 0, "0")

            'Carburante assegnato
            Carb_Assegnato_Macro_Lav(stb, 0, 0, "0")

            If dettaglio > 0 AndAlso (colture > 0 OrElse lavorazione > 0 OrElse allevamento <> "0") Then
                Carb_Calcolato_Macro_Lav(stb, colture, lavorazione, allevamento)

                Carb_Richiesto_Macro_Lav(stb, colture, lavorazione, allevamento)

                Carb_Assegnato_Macro_Lav(stb, colture, lavorazione, allevamento)
            End If

            'Carburante allevamenti
            If dettaglio = 0 OrElse allevamento <> "0" Then
                stb.AppendLine(" ISNULL(uag.Carburante_Calcolato, 0) as Carburante_Calcolato_Gasolio_Allevamenti, ")
                stb.AppendLine(" ISNULL(uag.Carburante_Richiesto, 0) as Carburante_Richiesto_Gasolio_Allevamenti, ")
                stb.AppendLine(" ISNULL(uag.Carburante_Approvato, 0) as Carburante_Approvato_Gasolio_Allevamenti, ")
                stb.AppendLine(" ISNULL(uab.Carburante_Calcolato, 0) as Carburante_Calcolato_Benzina_Allevamenti, ")
                stb.AppendLine(" ISNULL(uab.Carburante_Richiesto, 0) as Carburante_Richiesto_Benzina_Allevamenti, ")
                stb.AppendLine(" ISNULL(uab.Carburante_Approvato, 0) as Carburante_Approvato_Benzina_Allevamenti, ")
            End If

            'Tipo richiesta
            stb.AppendLine(" IIf(t.Tipo_Richiesta = 0, IIf(t.Avanzamento_Richiesta = -1, 'Anticipo Conto Proprio', 'Conto Proprio'), IIf(t.Avanzamento_Richiesta = -1, 'Anticipo Conto Terzi', 'Conto Terzi')) AS tipo_richiesta,  ")
            stb.AppendLine(" IIf(t.Avanzamento_Richiesta = 1, 'Rendicontazione', IIf(t.Richiesta_Integrativa = 1, 'Richiesta Integrativa', 'Prima Richiesta' )) AS Prima_Richiesta,  ")

            'Anna 12/08/21: Aggiunte due nuove colonne per UMA
            stb.AppendLine(" CASE WHEN richiedente.Nome = '' AND Richiedente.cognome = ''  ")
            stb.AppendLine("    THEN (' [' + richiedente.Rag_Soc + ']') ")
            stb.AppendLine(" ELSE (' [' + richiedente.Nome + ' ' + Richiedente.cognome +']') ")
            stb.AppendLine(" END AS Richiedente,  ")

            stb.AppendLine(" CASE WHEN psa.Stato_Cod BETWEEN 2002 AND 2006 THEN ")
            stb.AppendLine(" CASE WHEN Approvatoret.UserName IS NOT NULL THEN (' [' + Approvatoret.Nome + ' ' + Approvatoret.cognome +']')  ")
            stb.AppendLine(" ELSE (' [' + Approvatorep.Nome + ' ' + Approvatorep.cognome +']') END  ")
            stb.AppendLine(" END AS Approvatore,  ")
            stb.AppendLine(" COALESCE( t.Tipo_Azienda , 0) as Tipo_Azienda, ")
            stb.AppendLine(" CASE t.Tipo_Azienda ")
            stb.AppendLine(" WHEN 2 THEN '" & Azienda_Terzista & "'")
            stb.AppendLine(" WHEN 3 THEN '" & Cooperativa_Agricola & "'")
            stb.AppendLine(" WHEN 4 THEN '" & Azienda_Agricola_Istituzioni_Pubbliche & "'")
            stb.AppendLine(" WHEN 5 THEN '" & Consorzio_Bonifica_Irrigazione & "'")
            stb.AppendLine(" ELSE '" & Azienda_Agricola_Privata & "' END AS Tipo_Azienda_Des")

            stb.AppendLine(", ISNULL(t.Rec_Acc_Conf_Gasolio, 0) as Rec_Acc_Conf_Gasolio ")
            stb.AppendLine(", ISNULL(t.Rec_Acc_Conf_Benzina, 0) as Rec_Acc_Conf_Benzina ")
            stb.AppendLine(", ISNULL(t.Rec_Acc_Conf_Gasolio_Serra, 0) as Rec_Acc_Conf_Gasolio_Serra ")

            If dettaglio > 0 Then
                stb.AppendLine(" , " + IIf(allevamento <> "0", "'Allevamento'", IIf(colture > 0, "'Coltura'", "'Lavorazione'")) + " As TipoRiga ")

                If allevamento <> "0" Then
                    stb.AppendLine(" , " + IIf(allevamento <> "0", alleva, "''") + " ")
                Else
                    stb.AppendLine(" , " + IIf(colture > 0, coltu, "''") + " ")
                End If
                stb.Append(" As DescrizioneRiga ")

                stb.AppendLine(" , " + IIf(lavorazione > 0, lavo, "''") + " As Lavorazione ")
            Else
                stb.AppendLine(" , '' As TipoRiga ")
                stb.AppendLine(" , '' As DescrizioneRiga ")
                stb.AppendLine(" , '' As Lavorazione ")
            End If

            'From

            stb.AppendLine(" FROM UMA_Richieste_Testata t ")

            'Join #Temp lavorazioni e allevamenti
            JoinTempTableLavAll(stb, dettaglio > 0, colture > 0, lavorazione > 0, allevamento <> "0")

            'Altre join
            stb.AppendLine(" join Imprese imp on imp.Piva = t.Piva ")
            stb.AppendLine(" join Pratiche p on t.Piva = p.Piva And t.Pratica_Cod = p.Pratica_Cod ")
            stb.AppendLine(" left join Pratiche_Stati ps on t.Pratica_Cod = ps.Pratica_Cod AND ps.stato_Cod BETWEEN 2003 AND 2006 ")
            stb.AppendLine(" join Pratiche_Stati_Attuali psa on p.Piva_SuperUser = psa.Piva_SuperUser And p.Pratica_Cod = psa.Pratica_Cod ")
            stb.AppendLine(" join WAnagraficaStati psaw on psa.Stato_Cod = psaw.WAnagraficaStati_Cod ")
            stb.AppendLine(" join Imprese_Codici ic on t.Piva = ic.PIVA And ic.id_cod = 1010 ")
            stb.AppendLine(" INNER JOIN ImpresexIndirizzi ixi ON t.Piva = ixi.Piva ")
            stb.AppendLine(" INNER JOIN Indirizzi i ON ixi.Cod_Indirizzo = i.Cod_Indirizzo  ")

            stb.AppendLine(" LEFT JOIN ISTAT ON i.pro_cod_istat = ISTAT.PROV And i.com_cod_istat = ISTAT.COM ")

            stb.AppendLine(" LEFT JOIN #TempVerificaInCorso appr ON t.Pratica_Cod = appr.Pratica_Cod ")

            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" LEFT JOIN Utenti_Visibilita_Appoggio (NOLOCK) On imp.Piva = Utenti_Visibilita_Appoggio.Piva AND Utenti_Visibilita_Appoggio.Entita_Cod=1 AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & " ")
            End If

            If nuovaVisibilita Then
                If Not VisibilitaTotale Then
                    If FiltroUtente And Not FiltroGruppoUtente Then
                        stb.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON t.Piva = visibilita.Piva_Azienda AND visibilita.Username = '" & objParametri_Utenti.UtenteUsername & "' ")
                    ElseIf Not FiltroUtente And FiltroGruppoUtente Then
                        stb.AppendLine(" LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON t.Piva = visibilita.Piva_Azienda AND visibilita.Gruppo = " & GruppoUtente & " ")
                    Else
                        stb.AppendLine(" INNER JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Visibilita visibilita ON t.Piva = visibilita.Piva_Azienda AND visibilita.Gruppo = -1 ")
                    End If
                End If
            End If

            'Anna 12/08/21: Aggiunte due nuove colonne per UMA
            stb.AppendLine(" LEFT JOIN " & nomeTabUtenti & " Richiedente  ON Richiedente.UserName = t.Username_Creazione")
            stb.AppendLine(" LEFT JOIN " & nomeTabUtenti & " Approvatoret ON appr.UserName_Creazione = Approvatoret.Username ")
            stb.AppendLine(" LEFT JOIN " & nomeTabUtenti & " Approvatorep ON appr.UserName_Creazione = Approvatorep.CodFisc ")

            stb.AppendLine(" WHERE ")

            If rendicontazioni = 0 Then
                stb.AppendLine(" t.Avanzamento_Richiesta = 0 ")
            ElseIf rendicontazioni = 1 Then
                stb.AppendLine(" t.Avanzamento_Richiesta = 1  ")
            Else
                stb.AppendLine(" t.Avanzamento_Richiesta IN (1, 0) ")
            End If

            If (anno > 0) Then
                stb.AppendLine(" AND p.Anno = '" + Agro_SQL_SaveText(anno, False) + "' ")
            End If

            If dettaglio > 0 Then
                If colture > 0 Then
                    stb.AppendLine(" AND (ul_gasD.Gruppo_Colturale_UMA IS NOT NULL OR ul_benzD.Gruppo_Colturale_UMA IS NOT NULL OR ul_serraD.Gruppo_Colturale_UMA IS NOT NULL) ")
                End If
                If lavorazione > 0 Then
                    stb.AppendLine(" AND (ul_gasD.Lavorazione_UMA IS NOT NULL OR ul_benzD.Lavorazione_UMA IS NOT NULL OR ul_serraD.Lavorazione_UMA IS NOT NULL OR ")
                    stb.AppendLine(" ul_gas_ParzD.Lavorazione_UMA IS NOT NULL OR ul_benz_ParzD.Lavorazione_UMA IS NOT NULL OR ul_serra_ParzD.Lavorazione_UMA IS NOT NULL) ")
                End If
                If allevamento <> "0" Then
                    stb.Append(" AND (uagD.UMA_All_Cod IS NOT NULL OR uabD.UMA_All_Cod IS NOT NULL) ")
                End If
            End If

            If approvatore <> "0" Then
                stb.AppendLine(" AND (Approvatoret.UserName = '" + Agro_SQL_SaveText(approvatore) + "' OR Approvatorep.UserName = '" + Agro_SQL_SaveText(approvatore) + "') ")
            End If

            If causali.Length > 0 Then
                Dim causaliList = causali.Split("|")

                stb.AppendLine(" AND ( ")
                stb.Append(" t.Causale_Non_Utilizzo LIKE '%" + causaliList.First + "%' ")

                causaliList.Skip(1).ToList.ForEach(Sub(x) stb.Append(" OR t.Causale_Non_Utilizzo LIKE '%" + x + "%' "))

                stb.Append(" ) ")

            End If

            If (statoCod > 0) Then
                stb.AppendLine(" AND psaw.WAnagraficaStati_Cod = " & statoCod.ToString & " ")
            End If

            If (conto < 1) Then
                stb.AppendLine(" AND t.tipo_richiesta = " & conto.ToString & " ")
            End If

            If (citta <> "-1" AndAlso citta <> "") Then
                stb.AppendLine(" And ISTAT.COM = '" & Agro_SQL_SaveText(citta, False) & "' ")
            End If

            If (prov <> "-1") Then
                stb.AppendLine(" AND ISTAT.PROV = '" & Agro_SQL_SaveText(prov, False) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, False) & " ")
            End If

            If Filtro_Visibilita_Utente Then
                stb.AppendLine(" AND Utenti_Visibilita_Appoggio.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername, False) & "  ")
            End If

            If nuovaVisibilita And Not VisibilitaTotale Then
                stb.Append(" AND (NOT visibilita.Piva_Azienda IS NULL) ")
            End If

            If piveVisibili.Length > 0 Then
                stb.AppendLine(" AND t.Piva IN ('" & Agro_SQL_Save_Clausola_IN(piveVisibili, True) & "')  ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy) & " ")
            End If

            stb.AppendLine(" ) As x ")
            stb.AppendLine(" where (x.CodstatoAv between 2003 and 2006) OR x.CodstatoAv IN (2001, 2002, 2007, 2008, 2009, 2010) ")

            stb.AppendLine(" ; ")
            stb.AppendLine("")
            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ COMMITTED; ")

            'Dim startTime As Long = DateTime.Now.Ticks

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Shared Sub DropTempTableIfExists(stb As StringBuilder, tempName As String)

        'stb.AppendLine(String.Format(" IF EXISTS (select 1 from tempdb.sys.tables tt where tt.name like '{0}%') DROP TABLE {0}; ", tempName))

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo." + tempName + "') IS NOT NULL BEGIN ")
        stb.AppendLine("    DROP TABLE " + tempName)
        stb.AppendLine(" END; ")

    End Sub

    Public Shared Sub AppendJoinCteTemp(stb As StringBuilder)
        stb.AppendLine(" Left Join #TempLav ul_gas ON t.Richiesta_Cod = ul_gas.Richiesta_Cod And ul_gas.Tipo_Carburante = 2 ")
        stb.AppendLine(" Left Join #TempLav ul_benz ON t.Richiesta_Cod = ul_benz.Richiesta_Cod And ul_benz.Tipo_Carburante = 3 ")
        stb.AppendLine(" Left Join #TempLav ul_serra ON t.Richiesta_Cod = ul_serra.Richiesta_Cod And ul_serra.Tipo_Carburante = 8 ")
        stb.AppendLine(" Left Join #TempLavParz ul_gas_Parz ON t.Richiesta_Cod = ul_gas_Parz.Richiesta_Cod And ul_gas_Parz.Tipo_Carburante = 2  ")
        stb.AppendLine(" Left Join #TempLavParz ul_benz_Parz ON t.Richiesta_Cod = ul_benz_Parz.Richiesta_Cod And ul_benz_Parz.Tipo_Carburante = 3 ")
        stb.AppendLine(" Left Join #TempLavParz ul_serra_Parz ON t.Richiesta_Cod = ul_serra_Parz.Richiesta_Cod And ul_serra_Parz.Tipo_Carburante = 8 ")
        stb.AppendLine(" Left Join #TempAllevamenti uag ON t.Richiesta_Cod = uag.Richiesta_Cod AND uag.Tipo_Carburante = 2 ")
        stb.AppendLine(" Left Join #TempAllevamenti uab ON t.Richiesta_Cod = uab.Richiesta_Cod And uab.Tipo_Carburante = 3 ")
    End Sub

    Public Shared Sub AppendCarburanteAssegnato(stb As StringBuilder, Optional virgolaPrima As Boolean = False)

        If virgolaPrima Then
            stb.AppendLine(",")
        End If

        stb.AppendLine(" ISNULL(CASE ")
        stb.AppendLine("        WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Approvazione_Iniziale_Gasolio > 0 THEN t.Approvazione_Iniziale_Gasolio ")
        stb.AppendLine("        ELSE ISNULL(ul_gas.Fabbisogno_Assegnato, 0) + ISNULL(ul_gas_Parz.Fabbisogno_Assegnato, 0) ")
        stb.AppendLine(" END, 0) + ISNULL(uag.Carburante_Approvato, 0) as assegnato_gasolio, ")

        stb.AppendLine(" ISNULL(CASE ")
        stb.AppendLine("        WHEN t.Tipo_Richiesta = -1 And t.Avanzamento_Richiesta = 0 AND t.Approvazione_Iniziale_Benzina > 0 Then t.Approvazione_Iniziale_Benzina ")
        stb.AppendLine("        Else ISNULL(ul_benz.Fabbisogno_Assegnato, 0) + ISNULL(ul_benz_Parz.Fabbisogno_Assegnato, 0)  ")
        stb.AppendLine(" End, 0) + ISNULL(uab.Carburante_Approvato, 0) As assegnato_benzina, ")

        stb.AppendLine(" ISNULL(Case ")
        stb.AppendLine("       When t.Tipo_Richiesta = -1 And t.Avanzamento_Richiesta = 0 AND t.Approvazione_Iniziale_Gasolio_Serra > 0 Then t.Approvazione_Iniziale_Gasolio_Serra ")
        stb.AppendLine("        Else ISNULL(ul_serra.Fabbisogno_Assegnato, 0) + ISNULL(ul_serra_Parz.Fabbisogno_Assegnato, 0) ")
        stb.AppendLine(" End, 0) As assegnato_gasolio_serra ")

        If Not virgolaPrima Then
            stb.AppendLine(",")
        End If

    End Sub

    Public Shared Sub AppendCteAllevamenti(stb As StringBuilder)
        stb.AppendLine(" ,#TempAllevamenti  as( ")
        stb.AppendLine(" SELECT Richiesta_Cod, Tipo_Carburante, SUM(Carburante_Richiesto) as Carburante_Richiesto, SUM(Carburante_Calcolato) as Carburante_Calcolato, SUM(Carburante_Approvato) as Carburante_Approvato ")
        stb.AppendLine(" From UMA_Richieste_Allevamenti ")
        stb.AppendLine(" GROUP BY Richiesta_Cod, Tipo_Carburante ) ")
    End Sub

    Public Shared Sub AppendCteTempLavParz(stb As StringBuilder)
        stb.AppendLine(" ,#TempLavParz as ( ")
        stb.AppendLine(" SELECT Richiesta_Cod, Tipo_Carburante, SUM(Fabbisogno_Calcolato) as Fabbisogno_Calcolato, SUM(Fabbisogno_Richiesto) as Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) as Fabbisogno_Assegnato ")
        stb.AppendLine(" From UMA_Lavorazioni_Parziali ")
        stb.AppendLine(" GROUP BY Richiesta_Cod, Tipo_Carburante ) ")
    End Sub

    Public Shared Sub AppendWithCteTempLav(stb As StringBuilder, primaCte As Boolean)
        If primaCte Then
            stb.Append(" WITH ")
        Else
            stb.Append(" , ")
        End If
        stb.AppendLine(" #TempLav as ( ")
        stb.AppendLine(" SELECT Richiesta_Cod, Tipo_Carburante, SUM(Fabbisogno_Calcolato) as Fabbisogno_Calcolato, SUM(Fabbisogno_Richiesto) as Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) as Fabbisogno_Assegnato ")
        stb.AppendLine(" From UMA_Richieste_Lavorazioni ")
        stb.AppendLine(" GROUP BY Richiesta_Cod, Tipo_Carburante ) ")
    End Sub

    Public Shared Sub AppendCteRendicontazioni(stb As StringBuilder, anno As Integer, primaCte As Boolean)
        If primaCte Then
            stb.Append(" WITH ")
        Else
            stb.Append(" , ")
        End If
        stb.AppendLine(" cte_rendicontazioni as")
        stb.AppendLine("(")
        stb.AppendLine("  select ")
        stb.AppendLine("   t.Piva")
        stb.AppendLine("  ,year(t.Validita_Inizio) Anno")
        stb.AppendLine("  ,t.Tipo_Richiesta")
        stb.AppendLine("  ,min(ps.Data_Creazione) Data_Prima_Compilazione")
        stb.AppendLine("  ,COUNT(*) rk")
        stb.AppendLine("  from UMA_Richieste_Testata t")
        stb.AppendLine("  inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("  inner join Pratiche_Stati ps on ps.Pratica_Cod = t.Pratica_Cod and ps.Stato_Cod = 2007")
        stb.AppendLine("  inner join Imprese i on i.PIVA = t.piva")
        stb.AppendLine("  where year(t.Validita_Inizio) = " + anno.ToString + " ")
        stb.AppendLine("  and t.Avanzamento_Richiesta = 1")
        stb.AppendLine("  and psa.Stato_Cod not in ('2006','2009') -- Non superata / Rinuncia")
        stb.AppendLine("  group by ")
        stb.AppendLine("   t.piva")
        stb.AppendLine("  ,year(t.Validita_Inizio)")
        stb.AppendLine("  ,t.Tipo_Richiesta")
        stb.AppendLine("  )")
    End Sub

    Public Shared Sub AppendCteRichieste(stb As StringBuilder, anno As Integer, primaCte As Boolean,
                                         soloApprovate As Boolean, soloPrimeRichieste As Boolean)
        If primaCte Then
            stb.Append(" WITH ")
        Else
            stb.Append(" , ")
        End If

        If soloPrimeRichieste Then
            stb.AppendLine("cte_prime_richieste as")
        Else
            stb.AppendLine("cte_richieste as")
        End If
        stb.AppendLine("(")
        stb.AppendLine("select ")
        stb.AppendLine(" t.Piva")
        stb.AppendLine(",year(t.Validita_Inizio) Anno")
        stb.AppendLine(",t.Tipo_Richiesta")
        stb.AppendLine(",COUNT(*) rk")
        If soloPrimeRichieste Then
            stb.AppendLine(",SUM(t.Rimanenza_Gasolio) As Rimanenza_Gasolio")
            stb.AppendLine(",SUM(t.Rimanenza_Benzina) As Rimanenza_Benzina")
            stb.AppendLine(",SUM(t.Rimanenza_Gasolio_Serra) As Rimanenza_Gasolio_Serra")
        Else
            stb.AppendLine(",max(t.Rimanenza_Gasolio) Rimanenza_Gasolio")
            stb.AppendLine(",max(t.Rimanenza_Benzina) Rimanenza_Benzina")
            stb.AppendLine(",max(t.Rimanenza_Gasolio_Serra) Rimanenza_Gasolio_Serra")
        End If
        stb.AppendLine("from UMA_Richieste_Testata t")
        stb.AppendLine("inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join Imprese i on i.PIVA = t.piva")
        stb.AppendLine("where year(t.Validita_Inizio) = " + anno.ToString + "")
        stb.AppendLine("and t.Avanzamento_Richiesta = 0")

        If soloApprovate Then
            stb.AppendLine("and psa.Stato_Cod = 2005 -- Verifica Intermedia completata con successo")
        Else
            stb.AppendLine("and psa.Stato_Cod not in ('2006','2009') -- Non superata / Rinuncia")
        End If

        If soloPrimeRichieste Then
            stb.AppendLine(" and t.Richiesta_Integrativa = 0 ")
        End If

        stb.AppendLine("group by ")
        stb.AppendLine(" t.piva")
        stb.AppendLine(",year(t.Validita_Inizio)")
        stb.AppendLine(",t.Tipo_Richiesta")

        stb.AppendLine(")")
    End Sub

    Public Shared Sub AppendCteAnticipi(stb As StringBuilder, anno As Integer, primaCte As Boolean, soloApprovati As Boolean)
        If primaCte Then
            stb.Append(" WITH ")
        Else
            stb.Append(" , ")
        End If

        stb.AppendLine("cte_anticipi_richieste as")
        stb.AppendLine("(")
        stb.AppendLine("select ")
        stb.AppendLine(" t.Piva")
        stb.AppendLine(",year(t.Validita_Inizio) Anno")
        stb.AppendLine(",t.Tipo_Richiesta")
        stb.AppendLine(",COUNT(*) rk")
        stb.AppendLine("from UMA_Richieste_Testata t")
        stb.AppendLine("inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join Imprese i on i.PIVA = t.piva")
        stb.AppendLine("where year(t.Validita_Inizio) = " + anno.ToString + "")
        stb.AppendLine("and t.Avanzamento_Richiesta in (-1,0)")
        If soloApprovati Then
            stb.AppendLine("and psa.Stato_Cod = 2005 -- Verifica Intermedia completata con successo")
        Else
            stb.AppendLine("and psa.Stato_Cod not in ('2006','2009') -- Non superata / Rinuncia")
        End If
        stb.AppendLine("group by ")
        stb.AppendLine(" t.piva")
        stb.AppendLine(",year(t.Validita_Inizio)")
        stb.AppendLine(",t.Tipo_Richiesta")
        stb.AppendLine(")")
    End Sub

    Public Shared Sub AppendCteAcquistato(stb As StringBuilder, tipoCarb As enum_TipoCarburante_UMA, primaCte As Boolean, anno As Integer)
        If primaCte Then
            stb.Append(" WITH ")
        Else
            stb.Append(" , ")
        End If

        stb.Append(" #cte_Acquistato_")
        Select Case tipoCarb
            Case 2
                stb.Append("Gasolio")
            Case 3
                stb.Append("Benzina")
            Case 8
                stb.Append("Gasolio_Serra")
            Case Else
                stb.Append("Carburante")
        End Select
        stb.AppendLine(" as ( ")
        stb.AppendLine("select PIVA_Cliente As Piva, Anno, Conto_Proprio_Terzi, SUM(Lt) As Acquistato from UMA_Vendite")
        stb.AppendLine("where Tipo_Carburante = " + CInt(tipoCarb).ToString + " ")
        If anno > 0 Then
            stb.AppendLine(" AND Anno = " + anno.ToString + " ")
        End If
        stb.AppendLine("Group by PIVA_Cliente, Anno, Conto_Proprio_Terzi")
        stb.AppendLine(" ) ")
    End Sub

    Public Shared Sub AppendCteAcquistatoXCarb(stb As StringBuilder)
        stb.AppendLine(", cte_acquisti_x_carb as (")
        stb.AppendLine("select Piva,Conto_Proprio_Terzi ")
        stb.AppendLine(", SUM(Acquistato_Gasolio) Acquistato_Gasolio  ")
        stb.AppendLine(",SUM(Acquistato_Benzina) Acquistato_Benzina ")
        stb.AppendLine(",SUM(Acquistato_Gasolio_Serra) Acquistato_Gasolio_Serra ")
        stb.AppendLine("from (select PIVA,Conto_Proprio_Terzi ")
        stb.AppendLine(", Acquistato As Acquistato_Gasolio, 0 As Acquistato_Benzina, 0 As Acquistato_Gasolio_Serra from #cte_Acquistato_Gasolio Union  ")
        stb.AppendLine("select PIVA,Conto_Proprio_Terzi ")
        stb.AppendLine(", 0 As Acquistato_Gasolio, Acquistato As Acquistato_Benzina, 0 As Acquistato_Gasolio_Serra from #cte_Acquistato_Benzina Union ")
        stb.AppendLine("select PIVA,Conto_Proprio_Terzi ")
        stb.AppendLine(", 0 As Acquistato_Gasolio, 0 As Acquistato_Benzina, Acquistato As Acquistato_Gasolio_Serra from #cte_Acquistato_Gasolio_Serra) as temp ")
        stb.AppendLine("group by ")
        stb.AppendLine(" PIVA ")
        stb.AppendLine(",Conto_Proprio_Terzi ")
        stb.AppendLine(" ) ")
    End Sub

    Public Shared Sub AppendJoinCteAcquistato(stb As StringBuilder, anno As Integer)
        stb.AppendLine("left join #cte_Acquistato_Gasolio cte_acq_gas on cte_acq_gas.Piva = t.Piva and")
        stb.AppendLine("                                                cte_acq_gas.Anno = " + anno.ToString() + " and")
        stb.AppendLine("                                                cte_acq_gas.Conto_Proprio_Terzi = t.Tipo_Richiesta")
        stb.AppendLine("left join #cte_Acquistato_Benzina cte_acq_benz on cte_acq_benz.Piva = t.Piva and")
        stb.AppendLine("                                                 cte_acq_benz.Anno = " + anno.ToString() + " and")
        stb.AppendLine("                                                 cte_acq_benz.Conto_Proprio_Terzi = t.Tipo_Richiesta")
        stb.AppendLine("left join #cte_Acquistato_Gasolio_Serra cte_acq_gas_Serra on cte_acq_gas_Serra.Piva = t.Piva and")
        stb.AppendLine("                                                            cte_acq_gas_Serra.Anno = " + anno.ToString + " and")
        stb.AppendLine("                                                            cte_acq_gas_Serra.Conto_Proprio_Terzi = t.Tipo_Richiesta")
    End Sub

    Public Function CheckTestate(ByVal piva As String,
                                 ByVal Richiesta_Cod As Integer,
                                 ByVal Tipo_Richiesta As Integer,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.CheckTestate()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim cod As Integer = -1

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT UMA_Richieste_Testata.*, Pratiche.Anno, Pratiche.Numero ")
            stb.AppendLine(" FROM UMA_Richieste_Testata ")
            stb.AppendLine(" LEFT JOIN Pratiche ON UMA_Richieste_Testata.Pratica_Cod = Pratiche.Pratica_Cod ")
            stb.AppendLine(" WHERE UMA_Richieste_Testata.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            stb.AppendLine(" AND UMA_Richieste_Testata.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            If Tipo_Richiesta <> -2 Then
                stb.AppendLine(" AND UMA_Richieste_Testata.Tipo_Richiesta = " & Agro_SQL_SaveNum(Tipo_Richiesta) & " ")
            End If

            If Richiesta_Cod <> 0 AndAlso Richiesta_Cod <> -1 Then
                stb.AppendLine(" AND UMA_Richieste_Testata.Richiesta_Cod = " & Agro_SQL_SaveNum(Richiesta_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            'If (dt.Rows.Count > 0) Then
            '    cod = dt.Rows.Item(0).Item("Richiesta_Cod")
            'End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function GetProvince(ByRef objParametri As AgronicaCoreParametri, Stato_Country As String) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.GetProvince()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT Distinct Lista_Province.PROV ,Lista_Province.PROVINCIA ")
            stb.AppendLine(" FROM  Lista_Province ")
            stb.AppendLine(" WHERE SIGLA <> '00' AND PROV > '0' ")
            If Stato_Country <> "" Then
                stb.AppendLine(" AND Stato_Country = " & Agro_SQL_SaveText_NULL(Stato_Country) & " ")
            End If
            stb.AppendLine(" ORDER BY PROVINCIA ")

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

    Public Function Macrousi(ByRef objParametri As AgronicaCoreParametri) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.Macrousi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT distinct Macrouso_UMA_Cod, Macrouso_UMA_Des ")
            stb.AppendLine(" FROM Codifica_SpecieVegetali_Agea_2015_2020 ")
            stb.AppendLine(" where Macrouso_UMA_Cod > 0 ")

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

    Public Function RecuperaDataPassaggioDiStato(ByVal richiesta_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) As DataRow

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.RecuperaDataPassaggioDiStato()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT distinct ps.Data_Creazione ")
            stb.AppendLine(" FROM UMA_Richieste_Testata t ")
            stb.AppendLine(" JOIN Pratiche_Stati ps on ps.pratica_Cod = t.pratica_Cod")
            stb.AppendLine(" where t.Richiesta_Cod = " & richiesta_Cod.ToString & " AND ps.stato_Cod = 2001 ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If dt.Rows.Count <= 0 Then
            dt.Rows.Add(AGRODATAFINE)
        End If

        Return dt.Rows.Item(0)


    End Function

    Public Function RecuperaAnnoRichiesta(ByVal richiesta_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.RecuperaAnnoRichiesta()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim res As Integer

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT anno ")
            stb.AppendLine(" FROM UMA_Richieste_Testata t ")
            stb.AppendLine(" JOIN Pratiche p on p.pratica_cod = t.pratica_cod")
            stb.AppendLine(" where t.Richiesta_Cod = " & richiesta_Cod.ToString)

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If dt.Rows.Count > 0 Then
            res = CInt(dt.Rows.Item(0).Item("anno"))
        Else
            res = 0
        End If

        Return res


    End Function

    Public Function RecuperaSeIntegrativaDaRichiestaCod(ByVal richiesta_Cod As Integer, ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.RecuperaSeIntegrativaDaRichiestaCod()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim res As Boolean

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT Richiesta_Integrativa ")
            stb.AppendLine(" FROM UMA_Richieste_Testata t ")
            stb.AppendLine(" where t.Richiesta_Cod = " & richiesta_Cod.ToString)

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If dt.Rows.Count > 0 Then
            res = If(dt.Rows.Item(0).Item("Richiesta_Integrativa") = 1, True, False)
        Else
            res = False
        End If

        Return res


    End Function

    Public Function EsistonoTrasferimentiRestituzioni(ByVal piva As String,
                                                      ByVal Richiesta_Cod As Integer,
                                                      ByVal Validita_Inizio As Date,
                                                      ByVal Validita_Fine As Date,
                                                      ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.EsistonoTrasferimentiRestituzioni()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Dim esistono_TrasferimentiRestituzioni = False

        Try

            stb.Length = 0

            ComponiQueryTrasferimentiRestituzioni("UMA_Richieste_Trasferimenti",
                                                  piva,
                                                  Richiesta_Cod,
                                                  Validita_Inizio,
                                                  Validita_Fine,
                                                  objParametri,
                                                  stb)
            stb.AppendLine(" UNION ALL ")

            ComponiQueryTrasferimentiRestituzioni("UMA_Richieste_Restituzioni",
                                                  piva,
                                                  Richiesta_Cod,
                                                  Validita_Inizio,
                                                  Validita_Fine,
                                                  objParametri,
                                                  stb)

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If Not IsNothing(dt) Then
            If dt.Rows.Count > 0 AndAlso dt(0).Item("righe_trasf_rest") > 0 Then
                esistono_TrasferimentiRestituzioni = True
            Else
                If dt.Rows.Count > 1 AndAlso dt(1).Item("righe_trasf_rest") > 0 Then
                    esistono_TrasferimentiRestituzioni = True
                End If
            End If
        End If

        Return esistono_TrasferimentiRestituzioni

    End Function

    Public Function RecuperaAnticipazioniColturaliTerzisti(ByVal piva As String,
                                                           ByVal anno As Integer,
                                                           ByRef objParametriServer As AgronicaCoreParametri,
                                                           Optional ByVal dettaglio As Boolean = True) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.RecuperaAnticipazioniColturaliTerzisti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            If dettaglio Then
                stb.AppendLine(" SELECT t.piva, l.Lavorazione_UMA, l.Totale_Superficie_UMA, l.Fabbisogno_Assegnato, l.Tipo_Carburante, ")
                stb.AppendLine(" i.rag_soc, ul.Lav_UMA_Des, c.Car_Des")
            Else
                stb.AppendLine(" SELECT l.Tipo_Carburante, SUM(l.Fabbisogno_Assegnato) As Fabbisogno_Assegnato")
            End If

            stb.AppendLine(" From UMA_Richieste_Testata t ")
            stb.AppendLine(" Join Pratiche p on p.Pratica_Cod = t.Pratica_Cod ")
            stb.AppendLine(" join Pratiche_Stati_Attuali psa ON psa.Pratica_cod = p.pratica_cod ")
            stb.AppendLine(" Join UMA_Richieste_Lavorazioni l on l.Richiesta_Cod = t.Richiesta_Cod ")

            If dettaglio Then
                stb.AppendLine(" JOIN imprese i on i.piva = t.Piva ")
                stb.AppendLine(" JOIN CARBURANTI c on c.Car_Cod = l.Tipo_Carburante ")
                stb.AppendLine(" JOIN UMA_Lavorazioni ul on ul.Lav_UMA_Cod = l.Lavorazione_UMA ")
            End If

            stb.AppendLine(" Where t.Piva <> '" & Agro_SQL_SaveText(piva) & "' ")
            stb.AppendLine(" And t.Tipo_Richiesta = " & Agro_SQL_SaveNum(enum_UMA_TipoRichiesta.Conto_Terzi) & " ")
            stb.AppendLine(" And t.Avanzamento_Richiesta = " & Agro_SQL_SaveNum(enum_UMA_Avanzamento.Rendicontazione) & " ")
            stb.AppendLine(" And l.Gruppo_Colturale_UMA = '9997'  ") 'Anticipazioni Colturali
            stb.AppendLine(" And psa.stato_cod = 2005 ") 'approvato con successo
            stb.AppendLine(" And p.Anno = '" & Agro_SQL_SaveText((anno - 1).ToString) & "' AND l.Piva like '" & Agro_SQL_SaveText(piva) & "' ")

            If Not dettaglio Then
                stb.AppendLine(" GROUP BY l.Tipo_Carburante")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' <summary>
    ''' Recupera l'attuale utente approvatore della pratica interessata
    ''' </summary>
    ''' <param name="pratica_cod">Intero contenente il numero della pratica di cui si desidera conoscere l'attuale approvatore</param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <returns>Una stringa contenente Username[Nome Cognome] dell'approvatore</returns>
    Public Function RecuperaApprovatore(ByVal pratica_cod As Integer, ByRef objParametri_Server As AgronicaCoreParametri,
                                        ByRef objParametri_Utenti As AgronicaCoreParametri) As String

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2)
        NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)
        Dim nomeTabUtenti = NomeDB_Utenti + ".dbo.utenti_dettagli"

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.RecuperaApprovatore()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim res As String = ""

        Try

            stb.Length = 0

            stb.AppendLine("with praticheStati as ( ")
            stb.AppendLine("Select top 1 p.*, ps.Stato_Cod from Pratiche p ")
            stb.AppendLine("join Pratiche_Stati ps On ps.Pratica_Cod = p.pratica_Cod ")
            stb.AppendLine("where p.pratica_cod = " & Agro_SQL_SaveNum(pratica_cod) & " ")
            stb.AppendLine("order by ps.data_creazione desc ")
            stb.AppendLine(") ")

            stb.AppendLine("Select t.richiesta_cod, Case When appr.Stato_Cod BETWEEN 2002 And 2006 Then  ")
            stb.AppendLine(" Case When Approvatoret.UserName Is Not NULL Then (Approvatoret.UserName +' [' + Approvatoret.Nome + ' ' + Approvatoret.cognome +']')   ")
            stb.AppendLine(" ELSE (Approvatorep.UserName +' [' + Approvatorep.Nome + ' ' + Approvatorep.cognome +']') END   ")
            stb.AppendLine(" End As Approvatore ")
            stb.AppendLine("From UMA_Richieste_Testata t  ")
            stb.AppendLine("Join praticheStati appr On t.Pratica_Cod = appr.Pratica_Cod  ")
            stb.AppendLine("Left JOIN " & nomeTabUtenti & " Approvatoret On appr.UserName_Modifica = Approvatoret.Username  ")
            stb.AppendLine("Left JOIN " & nomeTabUtenti & " Approvatorep On appr.UserName_Modifica = Approvatorep.CodFisc ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If dt.Rows.Count > 0 AndAlso Not IsDBNull(dt.Rows.Item(0).Item("Approvatore")) Then
            res = dt.Rows.Item(0).Item("Approvatore")
        End If

        Return res
    End Function

    Public Function Leggi_Utenti_Approvatori(ByRef objParametri_Utenti As AgronicaCoreParametri,
                                             ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeDB_Server As String = objParametri_Server.StringaConnessione.Split(";")(2)
        NomeDB_Server = NomeDB_Server.Split("=")(1)
        Dim nomeTabServer = NomeDB_Server + ".dbo.pratiche_Stati"

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_R.Leggi_Utenti_Approvatori()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim res As String = ""

        Try

            stb.Length = 0

            stb.AppendLine("select ud.UserName AS cod, ")
            stb.AppendLine(" ud.Nome + ' ' + ud.Cognome + ' [' + ud.CodFisc + ']' as Approvatore from Utenti_Dettagli ud ")
            stb.AppendLine("join Utenti_xGruppi_Utente u on u.UserName = ud.UserName ")
            stb.AppendLine("where u.Gruppi_Utente_cod IN ( ")
            stb.AppendLine("select distinct (ud.Gruppi_Utente_cod) As Approvatore from " + nomeTabServer + " ps ")
            stb.AppendLine("join Utenti_xGruppi_Utente ud on ud.UserName = ps.Username_Creazione ")
            stb.AppendLine("where ps.Username_Creazione <> '03761180961' and ps.Stato_Cod IN (2002) ")
            stb.AppendLine(") ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Utenti, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Private Sub ComponiQueryTrasferimentiRestituzioni(ByVal nomeTabella As String,
                                                      ByVal piva As String,
                                                      ByVal Richiesta_Cod As Integer,
                                                      ByVal Validita_Inizio As Date,
                                                      ByVal Validita_Fine As Date,
                                                      ByRef objParametri As AgronicaCoreParametri,
                                                      ByRef stb As StringBuilder)

        stb.AppendLine(" Select COUNT(*) righe_trasf_rest ")
        stb.AppendLine(" FROM " & nomeTabella & " ")
        stb.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

        If piva <> "" Then
            stb.AppendLine(" AND piva = '" & Agro_SQL_SaveText(piva) & "' ")
        End If

        If Richiesta_Cod <> 0 Then
            stb.AppendLine(" AND Richiesta_Cod = " & Agro_SQL_SaveNum(Richiesta_Cod) & " ")
        End If

        If Validita_Inizio <> AGRODATAINIZIO Then
            stb.AppendLine(" AND Validita_Inizio >= " & Agro_SQL_SaveDateTime(Validita_Inizio) & " ")
        End If

        If Validita_Fine <> AGRODATAFINE Then
            stb.AppendLine(" AND Validita_Fine <= " + Agro_SQL_SaveDateTime(Validita_Fine) + " ")
        End If

    End Sub

    Private Sub TempTableLav(stb As StringBuilder, nomeTempLav As String, colture As Integer, lavorazione As Integer)
        'Const nomeTempLav = "#TempLav"
        DropTempTableIfExists(stb, nomeTempLav)
        stb.AppendLine(" SELECT Richiesta_Cod, Tipo_Carburante, ")
        If colture > 0 OrElse lavorazione > 0 Then
            If colture > 0 Then
                stb.Append(" Gruppo_Colturale_UMA, ")
            End If

            If lavorazione > 0 Then
                stb.Append(" Lavorazione_UMA, ")
            End If
        End If
        stb.AppendLine(" SUM(Fabbisogno_Calcolato) As Fabbisogno_Calcolato, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato ")
        stb.AppendLine(" INTO " + nomeTempLav)
        stb.AppendLine(" FROM UMA_Richieste_Lavorazioni ")
        If colture > 0 OrElse lavorazione > 0 Then
            stb.AppendLine(" WHERE 1 = 1 ")
            If colture > 0 Then
                stb.Append(" AND Gruppo_Colturale_UMA = " + Agro_SQL_SaveNum(colture) + " ")
            End If

            If lavorazione > 0 Then
                stb.Append(" AND Lavorazione_UMA = " + Agro_SQL_SaveNum(lavorazione) + " ")
            End If
        End If
        stb.AppendLine(" GROUP BY Richiesta_Cod, ")
        If colture > 0 OrElse lavorazione > 0 Then
            If colture > 0 Then
                stb.Append(" Gruppo_Colturale_UMA, ")
            End If

            If lavorazione > 0 Then
                stb.Append(" Lavorazione_UMA, ")
            End If
        End If
        stb.AppendLine(" Tipo_Carburante; ")
        stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_1_TempLav On " + nomeTempLav + " (Richiesta_Cod, Tipo_Carburante); ")
        stb.AppendLine("")
    End Sub

    Private Sub TempTableLavParz(stb As StringBuilder, nomeTempLavParz As String, colture As Integer, lavorazione As Integer)
        'Const nomeTempLavParz = "#TempLavParz"
        DropTempTableIfExists(stb, nomeTempLavParz)
        stb.AppendLine(" Select Richiesta_Cod, Tipo_Carburante, ")
        If colture > 0 OrElse lavorazione > 0 Then
            If lavorazione > 0 Then
                stb.Append(" Lavorazione_UMA, ")
            End If
        End If
        stb.AppendLine("SUM(Fabbisogno_Calcolato) As Fabbisogno_Calcolato, SUM(Fabbisogno_Richiesto) As Fabbisogno_Richiesto, SUM(Fabbisogno_Assegnato) As Fabbisogno_Assegnato ")
        stb.AppendLine(" INTO " + nomeTempLavParz)
        stb.AppendLine(" FROM UMA_Lavorazioni_Parziali ")
        If colture > 0 OrElse lavorazione > 0 Then
            If lavorazione > 0 Then
                stb.AppendLine(" WHERE 1 = 1 AND ")
                stb.Append(" Lavorazione_UMA = " + Agro_SQL_SaveNum(lavorazione) + " ")
            End If
        End If
        stb.AppendLine(" GROUP BY Richiesta_Cod, ")
        If colture > 0 OrElse lavorazione > 0 Then
            If lavorazione > 0 Then
                stb.Append(" Lavorazione_UMA, ")
            End If
        End If
        stb.AppendLine("Tipo_Carburante; ")
        stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_1_TempLavParz On " + nomeTempLavParz + " (Richiesta_Cod, Tipo_Carburante); ")
        stb.AppendLine("")
    End Sub

    Private Sub TempTableAllevamenti(stb As StringBuilder, nomeTempAllevamenti As String, allevamento As String)
        'Const nomeTempAllevamenti = "#TempAllevamenti"
        DropTempTableIfExists(stb, nomeTempAllevamenti)
        stb.AppendLine(" Select Richiesta_Cod, ")
        If allevamento <> "0" Then
            stb.Append(" UMA_All_Cod, ")
        End If
        stb.AppendLine(" Tipo_Carburante, SUM(Carburante_Richiesto) As Carburante_Richiesto, SUM(Carburante_Calcolato) As Carburante_Calcolato, SUM(Carburante_Approvato) As Carburante_Approvato ")
        stb.AppendLine(" INTO " + nomeTempAllevamenti)
        stb.AppendLine(" FROM UMA_Richieste_Allevamenti ")
        If allevamento <> "0" Then
            stb.AppendLine(" WHERE 1 = 1 AND ")
            stb.Append(" UMA_All_Cod = '" + Agro_SQL_SaveText(allevamento) + "' ")
        End If
        stb.AppendLine(" GROUP BY Richiesta_Cod, ")
        If allevamento <> "0" Then
            stb.Append(" UMA_All_Cod, ")
        End If
        stb.AppendLine(" Tipo_Carburante; ")
        stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_1_TempAllevamenti On " + nomeTempAllevamenti + " (Richiesta_Cod, Tipo_Carburante); ")
        stb.AppendLine("")
    End Sub

    Private Sub Carb_Calcolato_Macro_Lav(stb As StringBuilder, colture As Integer, lavorazioni As Integer, Allevamento As String)
        If colture = 0 AndAlso lavorazioni = 0 AndAlso Allevamento = "0" Then
            stb.AppendLine(" ISNULL(ul_gas.Fabbisogno_Calcolato, 0) + ISNULL(ul_gas_Parz.Fabbisogno_Calcolato, 0) + ISNULL(uag.Carburante_Calcolato, 0) as calcolato_Gasolio, ")
            stb.AppendLine(" ISNULL(ul_benz.Fabbisogno_Calcolato, 0) + ISNULL(ul_benz_Parz.Fabbisogno_Calcolato, 0) + ISNULL(uab.Carburante_Calcolato, 0) as calcolato_Benzina, ")
            stb.AppendLine(" ISNULL(ul_serra.Fabbisogno_Calcolato, 0) + ISNULL(ul_serra_Parz.Fabbisogno_Calcolato, 0)  as calcolato_Gasolio_Serra, ")
        ElseIf Allevamento <> "0" Then
            stb.AppendLine(" ISNULL(uagD.Carburante_Calcolato, 0) as calcolato_Gasolio_Dettaglio, ")
            stb.AppendLine(" ISNULL(uabD.Carburante_Calcolato, 0) as calcolato_Benzina_Dettaglio, ")
            stb.AppendLine(" 0 as calcolato_Gasolio_Serra_Dettaglio, ")
        Else
            stb.AppendLine(" ISNULL(ul_gasD.Fabbisogno_Calcolato, 0) + ISNULL(ul_gas_ParzD.Fabbisogno_Calcolato, 0) as calcolato_Gasolio_Dettaglio, ")
            stb.AppendLine(" ISNULL(ul_benzD.Fabbisogno_Calcolato, 0) + ISNULL(ul_benz_ParzD.Fabbisogno_Calcolato, 0) as calcolato_Benzina_Dettaglio, ")
            stb.AppendLine(" ISNULL(ul_serraD.Fabbisogno_Calcolato, 0) + ISNULL(ul_serra_ParzD.Fabbisogno_Calcolato, 0)  as calcolato_Gasolio_Serra_Dettaglio, ")
        End If
    End Sub

    Private Sub Carb_Richiesto_Macro_Lav(stb As StringBuilder, colture As Integer, lavorazioni As Integer, Allevamento As String)
        If colture = 0 AndAlso lavorazioni = 0 AndAlso Allevamento = "0" Then
            stb.AppendLine(" ISNULL(CASE ")
            stb.AppendLine("       WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Richiesta_Iniziale_Gasolio > 0 THEN CASE WHEN t.Richiesta_Iniziale_Gasolio > (ISNULL(ul_gas.Fabbisogno_Richiesto, 0) + ISNULL(ul_gas_Parz.Fabbisogno_Richiesto, 0)) THEN t.Richiesta_Iniziale_Gasolio ELSE (ISNULL(ul_gas.Fabbisogno_Richiesto, 0) + ISNULL(ul_gas_Parz.Fabbisogno_Richiesto, 0)) END ")
            stb.AppendLine("       ELSE (ISNULL(ul_gas.Fabbisogno_Richiesto, 0) + ISNULL(ul_gas_Parz.Fabbisogno_Richiesto, 0)) ")
            stb.AppendLine(" END, 0) + ISNULL(uag.Carburante_Richiesto, 0) as richiesto_Gasolio, ")
            stb.AppendLine(" ISNULL(CASE ")
            stb.AppendLine("        WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Richiesta_Iniziale_Benzina > 0 THEN t.Richiesta_Iniziale_Benzina ")
            stb.AppendLine("        ELSE (ISNULL(ul_benz.Fabbisogno_Richiesto, 0) + ISNULL(ul_benz_Parz.Fabbisogno_Richiesto, 0)) ")
            stb.AppendLine(" END, 0) + ISNULL(uab.Carburante_Richiesto, 0) as richiesto_Benzina, ")
            stb.AppendLine(" ISNULL(CASE ")
            stb.AppendLine("        WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Richiesta_Iniziale_Gasolio_Serra > 0 THEN t.Richiesta_Iniziale_Gasolio_Serra ")
            stb.AppendLine(" ELSE (ISNULL(ul_serra.Fabbisogno_Richiesto, 0) + ISNULL(ul_serra_Parz.Fabbisogno_Richiesto, 0))  ")
            stb.AppendLine(" END, 0) as richiesto_Gasolio_Serra, ")
        ElseIf Allevamento <> "0" Then
            stb.AppendLine(" ISNULL(uagD.Carburante_Richiesto, 0) as richiesto_Gasolio_Dettaglio, ")
            stb.AppendLine(" ISNULL(uabD.Carburante_Richiesto, 0) as richiesto_Benzina_Dettaglio, ")
            stb.AppendLine(" 0 as richiesto_Gasolio_Serra_Dettaglio, ")
        Else
            stb.AppendLine(" (ISNULL(ul_gasD.Fabbisogno_Richiesto, 0) + ISNULL(ul_gas_ParzD.Fabbisogno_Richiesto, 0)) ")
            stb.AppendLine("    as richiesto_Gasolio_Dettaglio, ")
            stb.AppendLine(" (ISNULL(ul_benzD.Fabbisogno_Richiesto, 0) + ISNULL(ul_benz_ParzD.Fabbisogno_Richiesto, 0)) ")
            stb.AppendLine("    as richiesto_Benzina_Dettaglio, ")
            stb.AppendLine(" (ISNULL(ul_serraD.Fabbisogno_Richiesto, 0) + ISNULL(ul_serra_ParzD.Fabbisogno_Richiesto, 0))  ")
            stb.AppendLine("    as richiesto_Gasolio_Serra_Dettaglio, ")
        End If
    End Sub

    Private Sub Carb_Assegnato_Macro_Lav(stb As StringBuilder, colture As Integer, lavorazioni As Integer, Allevamento As String)

        If colture = 0 AndAlso lavorazioni = 0 AndAlso Allevamento = "0" Then
            stb.AppendLine(" ISNULL(CASE ")
            stb.AppendLine("        WHEN t.Tipo_Richiesta = -1 AND t.Avanzamento_Richiesta = 0 AND t.Approvazione_Iniziale_Gasolio > 0 THEN t.Approvazione_Iniziale_Gasolio ")
            stb.AppendLine("        ELSE ISNULL(ul_gas.Fabbisogno_Assegnato, 0) + ISNULL(ul_gas_Parz.Fabbisogno_Assegnato, 0) ")
            stb.AppendLine(" END, 0) + ISNULL(uag.Carburante_Approvato, 0) as assegnato_gasolio, ")

            stb.AppendLine(" ISNULL(CASE ")
            stb.AppendLine("        WHEN t.Tipo_Richiesta = -1 And t.Avanzamento_Richiesta = 0 AND t.Approvazione_Iniziale_Benzina > 0 Then t.Approvazione_Iniziale_Benzina ")
            stb.AppendLine("        Else ISNULL(ul_benz.Fabbisogno_Assegnato, 0) + ISNULL(ul_benz_Parz.Fabbisogno_Assegnato, 0)  ")
            stb.AppendLine(" End, 0) + ISNULL(uab.Carburante_Approvato, 0) As assegnato_benzina, ")

            stb.AppendLine(" ISNULL(Case ")
            stb.AppendLine("       When t.Tipo_Richiesta = -1 And t.Avanzamento_Richiesta = 0 AND t.Approvazione_Iniziale_Gasolio_Serra > 0 Then t.Approvazione_Iniziale_Gasolio_Serra ")
            stb.AppendLine("        Else ISNULL(ul_serra.Fabbisogno_Assegnato, 0) + ISNULL(ul_serra_Parz.Fabbisogno_Assegnato, 0) ")
            stb.AppendLine(" End, 0) As assegnato_gasolio_serra, ")
        ElseIf Allevamento <> "0" Then
            stb.AppendLine(" ISNULL(uagD.Carburante_Approvato, 0) as assegnato_gasolio_Dettaglio, ")
            stb.AppendLine(" ISNULL(uabD.Carburante_Approvato, 0) As assegnato_benzina_Dettaglio, ")
            stb.AppendLine(" 0 As assegnato_gasolio_serra_Dettaglio, ")
        Else
            stb.AppendLine(" (ISNULL(ul_gasD.Fabbisogno_Assegnato, 0) + ISNULL(ul_gas_ParzD.Fabbisogno_Assegnato, 0) ")
            stb.AppendLine(" ) as assegnato_gasolio_Dettaglio, ")

            stb.AppendLine(" (ISNULL(ul_benzD.Fabbisogno_Assegnato, 0) + ISNULL(ul_benz_ParzD.Fabbisogno_Assegnato, 0)  ")
            stb.AppendLine(" ) As assegnato_benzina_Dettaglio, ")

            stb.AppendLine(" (ISNULL(ul_serraD.Fabbisogno_Assegnato, 0) + ISNULL(ul_serra_ParzD.Fabbisogno_Assegnato, 0) ")
            stb.AppendLine(" ) As assegnato_gasolio_serra_Dettaglio, ")
        End If
    End Sub

    Private Sub JoinTempTableLavAll(stb As StringBuilder, dettagli As Boolean, colture As Boolean, lavorazione As Boolean, allevamento As Boolean)
        stb.AppendLine(" Left Join #TempLav ul_gas ON t.Richiesta_Cod = ul_gas.Richiesta_Cod And ul_gas.Tipo_Carburante = 2 ")
        stb.AppendLine(" Left Join #TempLav ul_benz ON t.Richiesta_Cod = ul_benz.Richiesta_Cod And ul_benz.Tipo_Carburante = 3 ")
        stb.AppendLine(" Left Join #TempLav ul_serra ON t.Richiesta_Cod = ul_serra.Richiesta_Cod And ul_serra.Tipo_Carburante = 8 ")
        stb.AppendLine(" Left Join #TempLavParz ul_gas_Parz ON t.Richiesta_Cod = ul_gas_Parz.Richiesta_Cod And ul_gas_Parz.Tipo_Carburante = 2  ")
        stb.AppendLine(" Left Join #TempLavParz ul_benz_Parz ON t.Richiesta_Cod = ul_benz_Parz.Richiesta_Cod And ul_benz_Parz.Tipo_Carburante = 3 ")
        stb.AppendLine(" Left Join #TempLavParz ul_serra_Parz ON t.Richiesta_Cod = ul_serra_Parz.Richiesta_Cod And ul_serra_Parz.Tipo_Carburante = 8 ")
        stb.AppendLine(" Left Join #TempAllevamenti uag ON t.Richiesta_Cod = uag.Richiesta_Cod AND uag.Tipo_Carburante = 2 ")
        stb.AppendLine(" Left Join #TempAllevamenti uab ON t.Richiesta_Cod = uab.Richiesta_Cod And uab.Tipo_Carburante = 3 ")

        If dettagli Then
            If colture OrElse lavorazione Then
                stb.AppendLine(" Left Join #TempLavDettagli ul_gasD ON t.Richiesta_Cod = ul_gasD.Richiesta_Cod And ul_gasD.Tipo_Carburante = 2 ")
                stb.AppendLine(" Left Join #TempLavDettagli ul_benzD ON t.Richiesta_Cod = ul_benzD.Richiesta_Cod And ul_benzD.Tipo_Carburante = 3 ")
                stb.AppendLine(" Left Join #TempLavDettagli ul_serraD ON t.Richiesta_Cod = ul_serraD.Richiesta_Cod And ul_serraD.Tipo_Carburante = 8 ")
                stb.AppendLine(" Left Join #TempLavParzDettagli ul_gas_ParzD ON t.Richiesta_Cod = ul_gas_ParzD.Richiesta_Cod And ul_gas_ParzD.Tipo_Carburante = 2  ")
                stb.AppendLine(" Left Join #TempLavParzDettagli ul_benz_ParzD ON t.Richiesta_Cod = ul_benz_ParzD.Richiesta_Cod And ul_benz_ParzD.Tipo_Carburante = 3 ")
                stb.AppendLine(" Left Join #TempLavParzDettagli ul_serra_ParzD ON t.Richiesta_Cod = ul_serra_ParzD.Richiesta_Cod And ul_serra_ParzD.Tipo_Carburante = 8 ")
            ElseIf allevamento Then
                stb.AppendLine(" Left Join #TempAllevamentiDettagli uagD ON t.Richiesta_Cod = uagD.Richiesta_Cod AND uagD.Tipo_Carburante = 2 ")
                stb.AppendLine(" Left Join #TempAllevamentiDettagli uabD ON t.Richiesta_Cod = uabD.Richiesta_Cod And uabD.Tipo_Carburante = 3 ")
            End If
        End If

    End Sub

End Class


Public Class UMA_Richieste_Testata_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Const StatoInCompilazione = "In compilazione"

    Public Function Nuova_Richiesta(ByVal Richiesta As UMA_Richieste_Testata,
                                    ByRef objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_W.Nuova_Richiesta()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try


            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)


                GiasContext.UMA_Richieste_Testata.Add(Richiesta)

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

    Public Function Elimina_Richiesta(ByRef GiasContext As Gias_DeveloperServer_Entities,
                                      ByVal testataToDelete As ArrayList,
                                      ByVal richiesteToDelete As ArrayList,
                                      ByVal lavorazioniToDelete As ArrayList,
                                      ByVal lavorazioniParzialiToDelete As ArrayList,
                                      ByRef objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_Testata.Elimina_Richiesta()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try
            Dim gsWasNothing As Boolean = False
            If GiasContext Is Nothing Then
                GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
                gsWasNothing = True
            End If
            'aggiunto gloria per eliminare le lavorazioni parziali
            For Each delLP As UMA_Lavorazioni_Parziali In lavorazioniParzialiToDelete.ToArray
                GiasContext.UMA_Lavorazioni_Parziali.Attach(delLP)
                GiasContext.UMA_Lavorazioni_Parziali.Remove(delLP)
            Next
            'fine

            For Each delL As UMA_Richieste_Lavorazioni In lavorazioniToDelete.ToArray
                GiasContext.UMA_Richieste_Lavorazioni.Attach(delL)
                GiasContext.UMA_Richieste_Lavorazioni.Remove(delL)
            Next

            For Each delR As UMA_Richieste In richiesteToDelete
                GiasContext.UMA_Richieste.Attach(delR)
                GiasContext.UMA_Richieste.Remove(delR)
            Next

            For Each delT As UMA_Richieste_Testata In testataToDelete
                GiasContext.UMA_Richieste_Testata.Attach(delT)
                GiasContext.UMA_Richieste_Testata.Remove(delT)
            Next

            ' COMMIT Effettivo
            GiasContext.SaveChanges()

            If gsWasNothing Then
                GiasContext.Dispose()
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato

    End Function

    Public Function Modifica_Campo_Richiesta(Campo As String,
                                             Valore As Object,
                                             Piva As String,
                                             Richiesta_Cod As Integer,
                                             ByRef objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_W.Modifica_Campo_Richiesta()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try


            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                Dim PivaSuperUser = objParametri.PivaSuperUser
                Dim testata = (From t In GiasContext.UMA_Richieste_Testata 
                                Where t.Piva_SuperUser = PivaSuperUser AndAlso
                                      t.Piva = Piva AndAlso
                                      t.Richiesta_Cod = Richiesta_Cod).FirstOrDefault

                If testata IsNot Nothing Then

                    testata.GetType.GetProperty(Campo).SetValue(testata, Valore)
                    testata.Data_Modifica = DateTime.Now
                    testata.Username_Modifica = objParametri.UtenteUsername

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

    Public Function Modifica_Carburante(ByVal Piva As String,
                                        ByVal Richiesta_Cod As Integer,
                                        ByVal Rim_Riass_Gasolio As Double,
                                        ByVal Rim_Riass_Benzina As Double,
                                        ByVal Rim_Riass_Gasolio_Serra As Double,
                                        ByVal Rim_Riass_Conf_Gasolio As Double,
                                        ByVal Rim_Riass_Conf_Benzina As Double,
                                        ByVal Rim_Riass_Conf_Gasolio_Serra As Double,
                                        ByVal Rec_Acc_Dich_Gasolio As Double,
                                        ByVal Rec_Acc_Dich_Benzina As Double,
                                        ByVal Rec_Acc_Dich_Gasolio_Serra As Double,
                                        ByVal Rec_Acc_Conf_Gasolio As Double,
                                        ByVal Rec_Acc_Conf_Benzina As Double,
                                        ByVal Rec_Acc_Conf_Gasolio_Serra As Double,
                                        ByVal Causale_Non_Utilizzo As String,
                                        ByVal stato As String,
                                        ByVal avanzamento As Integer,
                                        ByVal Rim_Dich_Gasolio As Double,
                                        ByVal Rim_Dich_Benzina As Double,
                                        ByVal Rim_Dich_Gasolio_Serra As Double,
                                        ByRef objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_W.Modifica_Carburante()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim Rimanenza_Gasolio As Double
        Dim Rimanenza_Benzina As Double
        Dim Rimanenza_Gasolio_Serra As Double

        Try

            StrSQL = New System.Text.StringBuilder

            StrSQL.AppendLine("UPDATE UMA_Richieste_Testata SET ")

            StrSQL.AppendLine("  Causale_Non_Utilizzo    = '" & Agro_SQL_SaveText(Causale_Non_Utilizzo) & "' ")
            StrSQL.AppendLine(", Rim_Dich_Gasolio        = " & Agro_SQL_SaveNum(Rim_Dich_Gasolio) & " ")
            StrSQL.AppendLine(", Rim_Dich_Benzina        = " & Agro_SQL_SaveNum(Rim_Dich_Benzina) & " ")
            StrSQL.AppendLine(", Rim_Dich_Gasolio_Serra  = " & Agro_SQL_SaveNum(Rim_Dich_Gasolio_Serra) & " ")

            ' Rendicontazione
            If avanzamento = 1 Then
                If stato = StatoInCompilazione Then
                    ' Rimanenze da riassegnare
                    StrSQL.AppendLine(", Rim_Riass_Gasolio       = " & Agro_SQL_SaveNum(Rim_Riass_Gasolio) & " ")
                    StrSQL.AppendLine(", Rim_Riass_Benzina       = " & Agro_SQL_SaveNum(Rim_Riass_Benzina) & " ")
                    StrSQL.AppendLine(", Rim_Riass_Gasolio_Serra = " & Agro_SQL_SaveNum(Rim_Riass_Gasolio_Serra) & " ")
                    ' Aggiorno anche i campi standard
                    Rimanenza_Gasolio = Rim_Riass_Gasolio
                    Rimanenza_Benzina = Rim_Riass_Benzina
                    Rimanenza_Gasolio_Serra = Rim_Riass_Gasolio_Serra
                    StrSQL.AppendLine(", Rimanenza_Gasolio       = " & Agro_SQL_SaveNum(Rimanenza_Gasolio) & " ")
                    StrSQL.AppendLine(", Rimanenza_Benzina       = " & Agro_SQL_SaveNum(Rimanenza_Benzina) & " ")
                    StrSQL.AppendLine(", Rimanenza_Gasolio_Serra = " & Agro_SQL_SaveNum(Rimanenza_Gasolio_Serra) & " ")
                Else
                    ' Rimanenze da riassegnare confermate
                    StrSQL.AppendLine(", Rim_Riass_Conf_Gasolio       = " & Agro_SQL_SaveNum(Rim_Riass_Conf_Gasolio) & " ")
                    StrSQL.AppendLine(", Rim_Riass_Conf_Benzina       = " & Agro_SQL_SaveNum(Rim_Riass_Conf_Benzina) & " ")
                    StrSQL.AppendLine(", Rim_Riass_Conf_Gasolio_Serra = " & Agro_SQL_SaveNum(Rim_Riass_Conf_Gasolio_Serra) & " ")
                End If
            End If

            If stato = StatoInCompilazione Then
                ' Recupero accise
                StrSQL.AppendLine(", Rec_Acc_Dich_Gasolio       = " & Agro_SQL_SaveNum(Rec_Acc_Dich_Gasolio) & " ")
                StrSQL.AppendLine(", Rec_Acc_Dich_Benzina       = " & Agro_SQL_SaveNum(Rec_Acc_Dich_Benzina) & " ")
                StrSQL.AppendLine(", Rec_Acc_Dich_Gasolio_Serra = " & Agro_SQL_SaveNum(Rec_Acc_Dich_Gasolio_Serra) & " ")
            Else
                ' Recupero accise confermate
                StrSQL.AppendLine(", Rec_Acc_Conf_Gasolio       = " & Agro_SQL_SaveNum(Rec_Acc_Conf_Gasolio) & " ")
                StrSQL.AppendLine(", Rec_Acc_Conf_Benzina       = " & Agro_SQL_SaveNum(Rec_Acc_Conf_Benzina) & " ")
                StrSQL.AppendLine(", Rec_Acc_Conf_Gasolio_Serra = " & Agro_SQL_SaveNum(Rec_Acc_Conf_Gasolio_Serra) & " ")
            End If

            StrSQL.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" and   Piva           = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" and   Richiesta_Cod  = " & Agro_SQL_SaveNum(Richiesta_Cod) & " ")

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True

    End Function

    Public Function Modifica_Recupero_Accise(ByVal Piva As String,
                                             ByVal Richiesta_Cod As Integer,
                                             ByVal Rec_Acc_Gasolio As Double,
                                             ByVal Rec_Acc_Benzina As Double,
                                             ByVal Rec_Acc_Gasolio_Serra As Double,
                                             ByVal stato As String,
                                             ByRef objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_W.Modifica_Recupero_Accise()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL = New System.Text.StringBuilder

            StrSQL.AppendLine("UPDATE UMA_Richieste_Testata SET ")

            If stato = StatoInCompilazione Then
                ' Recupero accise
                StrSQL.AppendLine("  Rec_Acc_Dich_Gasolio       = " & Agro_SQL_SaveNum(Rec_Acc_Gasolio) & " ")
                StrSQL.AppendLine(", Rec_Acc_Dich_Benzina       = " & Agro_SQL_SaveNum(Rec_Acc_Benzina) & " ")
                StrSQL.AppendLine(", Rec_Acc_Dich_Gasolio_Serra = " & Agro_SQL_SaveNum(Rec_Acc_Gasolio_Serra) & " ")
            Else
                ' Recupero accise confermate
                StrSQL.AppendLine("  Rec_Acc_Conf_Gasolio       = " & Agro_SQL_SaveNum(Rec_Acc_Gasolio) & " ")
                StrSQL.AppendLine(", Rec_Acc_Conf_Benzina       = " & Agro_SQL_SaveNum(Rec_Acc_Benzina) & " ")
                StrSQL.AppendLine(", Rec_Acc_Conf_Gasolio_Serra = " & Agro_SQL_SaveNum(Rec_Acc_Gasolio_Serra) & " ")
            End If

            StrSQL.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" and   Piva           = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" and   Richiesta_Cod  = " & Agro_SQL_SaveNum(Richiesta_Cod) & " ")

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True

    End Function


    Public Function Azzera_Carburante_Confermato(ByVal Piva As String,
                                                 ByVal Richiesta_Cod As Integer,
                                                 ByVal avanzamento As Integer,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As Integer

        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_W.Azzera_Carburante_Confermato()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL = New System.Text.StringBuilder

            StrSQL.AppendLine("UPDATE UMA_Richieste_Testata SET ")

            ' Recupero accise confermate
            StrSQL.AppendLine("  Rec_Acc_Conf_Gasolio       = 0 ")
            StrSQL.AppendLine(", Rec_Acc_Conf_Benzina       = 0 ")
            StrSQL.AppendLine(", Rec_Acc_Conf_Gasolio_Serra = 0 ")

            ' Rendicontazione
            If avanzamento = 1 Then
                ' Rimanenze da riassegnare confermate
                StrSQL.AppendLine(", Rim_Riass_Conf_Gasolio       = 0 ")
                StrSQL.AppendLine(", Rim_Riass_Conf_Benzina       = 0 ")
                StrSQL.AppendLine(", Rim_Riass_Conf_Gasolio_Serra = 0 ")
            End If

            StrSQL.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" and   Piva           = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" and   Richiesta_Cod  = " & Agro_SQL_SaveNum(Richiesta_Cod) & " ")

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True

    End Function

    Public Function Modifica_Rimanenze_Rendicontazione(ByVal Piva As String,
                                                       ByVal Richiesta_Cod As Integer,
                                                       ByVal Rimanenze_Confermate As Boolean,
                                                       ByRef objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_W.Modifica_Rimanenze_Rendicontazione()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL = New System.Text.StringBuilder

            StrSQL.AppendLine("UPDATE UMA_Richieste_Testata SET ")

            If Rimanenze_Confermate Then
                StrSQL.AppendLine("  Rimanenza_Gasolio       = ISNULL(Rim_Riass_Conf_Gasolio,0) ")
                StrSQL.AppendLine(", Rimanenza_Benzina       = ISNULL(Rim_Riass_Conf_Benzina,0) ")
                StrSQL.AppendLine(", Rimanenza_Gasolio_Serra = ISNULL(Rim_Riass_Conf_Gasolio_Serra,0) ")
            Else
                StrSQL.AppendLine("  Rimanenza_Gasolio       = ISNULL(Rim_Riass_Gasolio,0) ")
                StrSQL.AppendLine(", Rimanenza_Benzina       = ISNULL(Rim_Riass_Benzina,0) ")
                StrSQL.AppendLine(", Rimanenza_Gasolio_Serra = ISNULL(Rim_Riass_Gasolio_Serra,0) ")
            End If

            StrSQL.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" and   Piva           = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine(" and   Richiesta_Cod  = " & Agro_SQL_SaveNum(Richiesta_Cod) & " ")
            StrSQL.AppendLine(" and   Avanzamento_Richiesta = 1 ")

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True

    End Function

    Public Function Aggiorna_Richiesta_Carburanti(richiestaTesta As UMA_Richieste_Testata, objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_Testata.Aggiorna_Richiesta_Carburanti()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                GiasContext.UMA_Richieste_Testata.Attach(richiestaTesta)
                GiasContext.Entry(richiestaTesta).State = EntityState.Modified

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

    Public Function Modifica_Allevati_In_Montagna(ByVal valore As Boolean,
                                                  ByVal richiesta_cod As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri) As Boolean
        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_Testata.Aggiorna_Richiesta_Carburanti()"
        Dim messaggioErrore As String = ""
        Dim risultato As Boolean = False
        Dim strSql As New StringBuilder()

        Try

            strSql.Length = 0

            strSql.AppendLine(" Update UMA_Richieste_Testata ")
            strSql.AppendLine(" Set Allevati_Montagna = " & If(valore, 1, 0).ToString & " ")
            strSql.AppendLine(" WHERE Richiesta_Cod = " & richiesta_cod.ToString & " ")

            '--------------------------------------------------------------------------
            risultato = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = False
        End Try

        Return risultato

    End Function

    Public Sub Sincronizza_Rimanenze_Su_Passaggio_Stato(ByVal pratica_Richiesta As Integer,
                                                             ByVal pratica_Rendicontazione As Integer,
                                                             ByRef objParametri As AgronicaCoreParametri)
        Dim r = New UMA_Richieste_Testata_R

        Dim richiesta = r.Leggi("", 0, pratica_Richiesta, AGRODATAINIZIO, AGRODATAFINE, objParametri, avanzamento:=0).Rows.Item(0)
        Dim rendicontazione = r.Leggi("", 0, pratica_Rendicontazione, AGRODATAINIZIO, AGRODATAFINE, objParametri, avanzamento:=1).Rows.Item(0)

        Modifica_Campo_Richiesta("Rimanenza_Gasolio", rendicontazione.Item("Rimanenza_Gasolio"), richiesta.Item("Piva"), richiesta.Item("Richiesta_Cod"), objParametri)
        Modifica_Campo_Richiesta("Rimanenza_Benzina", rendicontazione.Item("Rimanenza_Benzina"), richiesta.Item("Piva"), richiesta.Item("Richiesta_Cod"), objParametri)
        Modifica_Campo_Richiesta("Rimanenza_Gasolio_Serra", rendicontazione.Item("Rimanenza_Gasolio_Serra"), richiesta.Item("Piva"), richiesta.Item("Richiesta_Cod"), objParametri)

    End Sub

    Public Sub Azzera_Assegnato_Su_Passaggio_Di_Stato(ByVal pratica_Cod As Integer,
                                                      ByRef objParametri As AgronicaCoreParametri)
        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_W.Azzera_Assegnato_Su_Passaggio_Di_Stato()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0


        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                Dim PivaSuperUser = objParametri.PivaSuperUser
                Dim testata = (From t In GiasContext.UMA_Richieste_Testata
                                Where t.Piva_SuperUser = PivaSuperUser AndAlso
                                      t.Pratica_Cod = pratica_Cod).FirstOrDefault

                If testata IsNot Nothing Then

                    testata.Approvazione_Iniziale_Benzina = 0
                    testata.Approvazione_Iniziale_Gasolio = 0
                    testata.Approvazione_Iniziale_Gasolio_Serra = 0
                    testata.Carburante_Approvato = 0
                    testata.Approvatore = ""
                    testata.Data_Modifica = DateTime.Now
                    testata.Username_Modifica = objParametri.UtenteUsername

                End If

                Dim richieste = (From r In GiasContext.UMA_Richieste
                                Where r.Piva_SuperUser = PivaSuperUser AndAlso
                                      r.Richiesta_Cod = testata.Richiesta_Cod)

                For Each rich In richieste

                    rich.Carburante_Approvato = 0
                    rich.Data_Modifica = DateTime.Now
                    rich.Username_Modifica = objParametri.UtenteUsername

                Next

                Dim lavorazioni = (From l In GiasContext.UMA_Richieste_Lavorazioni
                                    Where l.Piva_SuperUser = PivaSuperUser AndAlso
                                          l.Richiesta_Cod = testata.Richiesta_Cod)

                For Each lav In lavorazioni

                    lav.Fabbisogno_Assegnato = 0
                    lav.Data_Modifica = DateTime.Now
                    lav.Username_Modifica = objParametri.UtenteUsername

                Next

                Dim lavorazioni_Parz = (From l In GiasContext.UMA_Lavorazioni_Parziali
                                        Where l.Piva_SuperUser = PivaSuperUser AndAlso
                                              l.Richiesta_Cod = testata.Richiesta_Cod)

                For Each lav In lavorazioni_Parz

                    lav.Fabbisogno_Assegnato = 0
                    lav.Data_Modifica = DateTime.Now
                    lav.Username_Modifica = objParametri.UtenteUsername

                Next

                Dim allevamenti = (From a In GiasContext.UMA_Richieste_Allevamenti
                                    Where a.Piva_SuperUser = PivaSuperUser AndAlso
                                          a.Richiesta_Cod = testata.Richiesta_Cod)

                For Each all In allevamenti

                    all.Carburante_Approvato = 0
                    all.Data_Modifica = DateTime.Now
                    all.Username_Modifica = objParametri.UtenteUsername

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

    End Sub

    Public Sub Azzera_Confermato_Gestione_Rimanenze(ByVal pratica_Cod As Integer,
                                                    ByRef objParametri As AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste_Testata_W.Azzera_Confermato_Gestione_Rimanenze()"
        Dim messaggioErrore As String = ""

        Dim richieste As New AgronicaCoreUmaDal.UMA_Richieste_R

        Try

            Dim DtTestata As DataTable = richieste.Leggi_CodicePratica(pratica_Cod, objParametri)

            If (DtTestata.Rows.Count > 0) Then

                Dim RigaT As DataRow = DtTestata.Rows(0)
                Dim Richiesta_Cod = IIf(IsDBNull(RigaT("Richiesta_Cod")), 0, RigaT("Richiesta_Cod"))

                If Richiesta_Cod > 0 Then
                    'Lettura setup
                    Dim setup As New UMASetup_R
                    Dim gestioneRimanenze = 0
                    Dim ValInizioData As Date = RigaT("Validita_Inizio")
                    Dim Anno = Year(ValInizioData)
                    Dim dtRiduzione = setup.LeggiSetup(Anno, objParametri)
                    If dtRiduzione.Rows.Count > 0 Then
                        gestioneRimanenze = dtRiduzione.Rows(0).Item("Gestione_Rimanenze")
                    End If

                    Dim avanzamento = RigaT("Avanzamento_Richiesta")

                    'Se gestione rimanenze abilitata
                    If gestioneRimanenze = 1 OrElse (gestioneRimanenze = 2 AndAlso avanzamento = 1) Then

                        Dim piva = RigaT("Piva")

                        'Azzeramento campi confermato in testata
                        Dim objRichTestata As New UMA_Richieste_Testata_W
                        objRichTestata.Azzera_Carburante_Confermato(piva, Richiesta_Cod, avanzamento, objParametri)

                        'Azzeramento campi confermato in trasferimenti
                        Dim objRichTrasferimenti As New UMA_Richieste_Trasferimenti_W
                        objRichTrasferimenti.Azzera_Confermato(piva, Richiesta_Cod, objParametri)

                        'Azzeramento campi confermato in restituzioni
                        Dim objRichRestituzioni As New UMA_Richieste_Restituzioni_W
                        objRichRestituzioni.Azzera_Confermato(piva, Richiesta_Cod, objParametri)

                    End If

                End If

            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)

        End Try

    End Sub

    Public Sub SalvaNoProssimaRichiesta(ByVal richiesta_Cod As Integer,
                                        ByVal noProxRichiesta As Boolean,
                                        ByRef objParametri_Server As AgronicaCoreParametri)

        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreUMADAL.UMA_Richieste_Testata.SalvaNoProssimaRichiesta()"
        Dim log As New AgronicaCoreDataProvider.LogProvider
        Dim noProxRichiestaInt As Integer = If(noProxRichiesta, 1, 0)

        Try
            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = IsolationLevel.ReadCommitted
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim testa = (From el In GiasContext.UMA_Richieste_Testata Where el.Richiesta_Cod = richiesta_Cod).FirstOrDefault

                    testa.Rinuncia_Nuova_Richiesta_Anno_Successivo = noProxRichiestaInt

                    GiasContext.SaveChanges()

                    If (String.IsNullOrEmpty(MessaggioErrore)) Then
                        ' COMMIT Effettivo
                        scope.Complete()
                    Else
                        ' Rollback
                        scope.Dispose()
                    End If

                End Using
            End Using

        Catch ex As Exception
            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            log.Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception(MessaggioErrore)
        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

    End Sub
End Class