Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports System.Transactions
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class UMA_RichiesteXReg_Impianti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Legge da tabella UMA_RichiestexReg_Impianti, associazione fra UMA_Richieste e Reg_Impianti. Legge impianti coinvolti per una richiesta carburanti UMA. 
    ''' L'ultimo parametro serve a ritornare solo associazioni/impianti per le quali sono avvenute operazioni anagrafiche sugli Impianti, Appezzamenti o Particelle Catastali coinvolti, 
    ''' successive all'ultima data di modifica della richiesta, oppure per il caso in cui questa è tornata in Compilazione da uno stato successivo, e quindi potrebbe non essere più allineata 
    ''' come superfici dichiarate o distribuzione delle superfici fra pendenze e/o tessiture. 
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="richiesta_cod"></param>
    ''' <param name="gruppo_uma"></param>
    ''' <param name="Programmazione_Cod"></param>
    ''' <param name="rendicontazione_terzista"></param>
    ''' <param name="Selezione_Variabile"></param>
    ''' <param name="objParametri"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="filtroSuLogOpAnagraficheSuccessive">Solo richieste per le quali sono presenti log anagrafici successivi a ultima modifica, o sono tornate in Compilazione</param>
    ''' <returns></returns>
    Public Function Leggi_Richiesta_X_Reg_Impianti(ByVal piva As String,
                                                   ByVal richiesta_cod As Integer,
                                                   ByVal gruppo_uma As String,
                                                   ByVal Programmazione_Cod As Integer,
                                                   ByVal rendicontazione_terzista As Boolean,
                                                   ByVal Selezione_Variabile As enumSelezioneVariabile,
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                   Optional ByVal xFiltroAggiuntivo As String = "",
                                                   Optional ByVal xOrderBy As String = "",
                                                   Optional ByVal filtroSuLogOpAnagraficheSuccessive As Boolean = False
                                                   ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_RichiesteXReg_Impianti_R.Leggi_Richiesta_X_Reg_Impianti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Dim PivaSuperUser = objParametri.PivaSuperUser

        Try
            If filtroSuLogOpAnagraficheSuccessive Then
                stb.AppendLine("--Con questa query tiro fuori le chiavi degli impianti per fare letture mirate")
                stb.AppendLine(" SELECT")
                stb.AppendLine("   Piva_Impianto AS Piva ")
                stb.AppendLine(" , sa_cod ")
                stb.AppendLine(" , appezza ")
                stb.AppendLine(" , ID_Reg")
                stb.AppendLine(" , t.Data_Modifica AS Data_Ultima_Modifica_Richiesta_Testata ")
                stb.AppendLine(" INTO #UMA_RichiestexReg_Impianti")
                stb.AppendLine(" FROM UMA_RichiestexReg_Impianti")
                stb.AppendLine(" INNER JOIN UMA_Richieste r ON r.Richiesta_Cod = UMA_RichiestexReg_Impianti.Richiesta_Cod ")
                stb.AppendLine(" INNER JOIN UMA_Richieste_Testata t ON r.Richiesta_Cod = t.Richiesta_Cod ")
                stb.AppendLine(" WHERE UMA_RichiestexReg_Impianti.Richiesta_Cod = " + Agro_SQL_SaveNum(richiesta_cod) + " ")

                stb.AppendLine("")

                stb.AppendLine("-- Estraggo tutti gli ultimi log anagrafe per:")
                stb.AppendLine("-- Impianti")
                stb.AppendLine("-- Appezzamenti (modifica puntuale solo pendenze/sabbia-limo-argilla)")
                stb.AppendLine("-- ImpreseXParticelle modifica particella associata ad un'azienda")
                stb.AppendLine(" SELECT")
                stb.AppendLine(" 	Piva, sa_cod, appezza, ID_Reg, Data_Modifica, Tipo")
                stb.AppendLine(" INTO #LOG_Anagrafe")
                stb.AppendLine(" FROM (")
                stb.AppendLine(" 	SELECT")
                stb.AppendLine(" 		UMA.Piva, UMA.sa_cod, UMA.appezza, UMA.ID_Reg, MAX(Agronica_Log_Anagrafe.Data_Ora_RegistrazioneLog) AS Data_Modifica, 'Reg_Impianti' AS Tipo")
                stb.AppendLine(" 	FROM Agronica_Log_Anagrafe")
                stb.AppendLine(" 	INNER JOIN #UMA_RichiestexReg_Impianti UMA ON")
                stb.AppendLine(" 	    UMA.Piva = Agronica_Log_Anagrafe.Param1")
                stb.AppendLine(" 	AND UMA.Sa_Cod = Agronica_Log_Anagrafe.Param2")
                stb.AppendLine(" 	AND UMA.Appezza = Agronica_Log_Anagrafe.Param3")
                stb.AppendLine(" 	AND UMA.ID_Reg = Agronica_Log_Anagrafe.Param4")
                stb.AppendLine(" 	WHERE 1 = 1 ")
                stb.AppendLine($" 	AND tipo = '{CStr(enum_TipoEntita_Des.Impianti)}'")
                stb.AppendLine(" 	AND Agronica_Log_Anagrafe.Data_Ora_RegistrazioneLog >= UMA.Data_Ultima_Modifica_Richiesta_Testata ")
                stb.AppendLine(" 	GROUP BY UMA.PIVA, UMA.sa_cod, UMA.appezza, UMA.ID_Reg")

                stb.AppendLine("")
                stb.AppendLine(" 	UNION")
                stb.AppendLine("")

                stb.AppendLine(" 	SELECT")
                stb.AppendLine(" 		UMA.Piva, UMA.sa_cod, UMA.appezza, UMA.ID_Reg, MAX(Agronica_Log_Anagrafe.Data_Ora_RegistrazioneLog) AS Data_Modifica, 'Appezzamento'")
                stb.AppendLine(" 	FROM Agronica_Log_Anagrafe")
                stb.AppendLine(" 	INNER JOIN #UMA_RichiestexReg_Impianti UMA ON")
                stb.AppendLine(" 	    UMA.Piva = Agronica_Log_Anagrafe.Param1")
                stb.AppendLine(" 	AND UMA.Sa_Cod = Agronica_Log_Anagrafe.Param2")
                stb.AppendLine(" 	AND UMA.Appezza = Agronica_Log_Anagrafe.Param3")
                stb.AppendLine(" 	WHERE 1 = 1 ")
                stb.AppendLine($" 	AND tipo = '{CStr(enum_TipoEntita_Des.Appezza)}'")
                stb.AppendLine(" 	AND Agronica_Log_Anagrafe.Data_Ora_RegistrazioneLog >= UMA.Data_Ultima_Modifica_Richiesta_Testata ")
                stb.AppendLine(" 	GROUP BY UMA.PIVA, UMA.sa_cod, UMA.appezza, UMA.ID_Reg")

                stb.AppendLine("")
                stb.AppendLine(" 	UNION")
                stb.AppendLine("")

                stb.AppendLine(" 	SELECT")
                stb.AppendLine(" 	  UMA.Piva, UMA.sa_cod, UMA.appezza, UMA.ID_Reg, MAX(Agronica_Log_Anagrafe.Data_Ora_RegistrazioneLog) AS Data_Modifica, 'Particelle'")
                stb.AppendLine(" 	FROM Agronica_Log_Anagrafe")
                stb.AppendLine(" 	INNER JOIN #UMA_RichiestexReg_Impianti UMA ON")
                stb.AppendLine(" 	    UMA.Piva = Agronica_Log_Anagrafe.Param1")
                stb.AppendLine(" 	AND UMA.Sa_Cod = Agronica_Log_Anagrafe.Param2")
                stb.AppendLine(" 	WHERE 1 = 1 ")
                stb.AppendLine($" 	AND tipo = '{CStr(enum_TipoEntita_Des.ImpreseXParticelle)}'")
                stb.AppendLine(" 	AND Agronica_Log_Anagrafe.Data_Ora_RegistrazioneLog >= UMA.Data_Ultima_Modifica_Richiesta_Testata ")
                stb.AppendLine(" 	GROUP BY UMA.PIVA, UMA.sa_cod, UMA.appezza, UMA.ID_Reg")
                stb.AppendLine(" ) AS [LOG]")

                stb.AppendLine("")

                stb.AppendLine("--Estraggo le chiavi delle particelle associate agli impianti della richiesta")
                stb.AppendLine(" SELECT")
                stb.AppendLine(" 	  ap.Piva, ap.SA_COD, ap.Appezza, UMA.Id_Reg, Prov, Com, Foglio, Numero ")
                stb.AppendLine(" 	, CASE WHEN Sezione = '0' THEN '' ELSE Sezione END AS Sezione ")
                stb.AppendLine(" 	, CASE WHEN SUBALTERNO = '0' THEN '' ELSE SUBALTERNO END AS SUBALTERNO ")
                stb.AppendLine(" 	, UMA.Data_Ultima_Modifica_Richiesta_Testata ")
                stb.AppendLine(" INTO #AppezzamentixParticelle")
                stb.AppendLine(" FROM AppezzamentiXParticelle ap")
                stb.AppendLine(" JOIN #UMA_RichiestexReg_Impianti UMA ON")
                stb.AppendLine("     UMA.PIVA = ap.PIVA COLLATE DATABASE_DEFAULT")
                stb.AppendLine(" AND UMA.SA_COD = ap.SA_COD")
                stb.AppendLine(" AND UMA.APPEZZA = ap.APPEZZA")

                stb.AppendLine("")

                stb.AppendLine("--Estraggo i log di modifica delle analisi per tutte le entità possibilmente associate agli impianti della richiesta")
                stb.AppendLine(" SELECT")
                stb.AppendLine(" 	 Piva, sa_cod, appezza, ID_Reg, Data_Modifica, TipoDes, Analisi_Testata_Cod")
                stb.AppendLine(" INTO #LOG_Analisi")
                stb.AppendLine(" FROM (")

                stb.AppendLine("")

                stb.AppendLine(" 	--Estraggo le analisi associate agli appezzamenti")
                stb.AppendLine(" 	SELECT")
                stb.AppendLine(" 	      1 AS TipoCod, '1 - ANALISI SU APPEZZAMENTO' AS TipoDes")
                stb.AppendLine(" 		, UMA.Piva, UMA.Sa_Cod, UMA.Appezza, UMA.Id_Reg, at.Data_Modifica, at.Analisi_Testata_Cod")
                stb.AppendLine(" 	FROM Analisi_Testata [at]")
                stb.AppendLine(" 	JOIN Analisi_EntitaxTestata aet ON")
                stb.AppendLine(" 	aet.Analisi_Testata_Cod = at.Analisi_Testata_Cod")
                stb.AppendLine(" 	AND aet.Analisi_Entita_Cod = " & enum_Entita_Analisi.Appezzamento & " ")
                stb.AppendLine(" 	-- Filtro sugli Appezzamenti passati come parametri")
                stb.AppendLine(" 	JOIN #UMA_RichiestexReg_Impianti UMA ON")
                stb.AppendLine(" 	    UMA.PIVA = aet.PIVA COLLATE DATABASE_DEFAULT")
                stb.AppendLine(" 	AND UMA.SA_COD = aet.SA_COD")
                stb.AppendLine(" 	AND UMA.APPEZZA = aet.APPEZZA")
                stb.AppendLine("  WHERE 1 = 1")
                stb.AppendLine("  AND [at].Data_Modifica >= UMA.Data_Ultima_Modifica_Richiesta_Testata")

                stb.AppendLine("")
                stb.AppendLine(" 	UNION")
                stb.AppendLine("")

                stb.AppendLine(" 	--Estraggo le analisi associate ai campi x appezzamenti")
                stb.AppendLine(" 	SELECT")
                stb.AppendLine(" 	    2 AS TipoCod, '2 - ANALISI SU CAMPO' AS TipoDes")
                stb.AppendLine(" 	  , a.Piva, a.Sa_Cod, a.Appezza, UMA.id_Reg, at.Data_Modifica, at.Analisi_Testata_Cod")
                stb.AppendLine(" 	FROM Analisi_Testata [at]")
                stb.AppendLine(" 	JOIN Analisi_EntitaxTestata aet ON")
                stb.AppendLine(" 	    aet.Analisi_Testata_Cod = at.Analisi_Testata_Cod")
                stb.AppendLine(" 	AND aet.Analisi_Entita_Cod = " & enum_Entita_Analisi.Campo & " ")
                stb.AppendLine(" 	-- Join sull'appezzamento per estrarre l'area")
                stb.AppendLine(" 	JOIN Appezzamento a ON")
                stb.AppendLine(" 	    a.piva = aet.piva")
                stb.AppendLine(" 	AND a.SA_COD = aet.SA_COD")
                stb.AppendLine(" 	AND a.Campo_Cod = aet.Campo_Cod")
                stb.AppendLine(" 	JOIN #UMA_RichiestexReg_Impianti UMA ON")
                stb.AppendLine(" 	    UMA.PIVA = a.PIVA COLLATE DATABASE_DEFAULT")
                stb.AppendLine(" 	AND UMA.SA_COD = a.SA_COD")
                stb.AppendLine(" 	AND UMA.APPEZZA = a.APPEZZA")
                stb.AppendLine("  WHERE 1 = 1")
                stb.AppendLine("  AND [at].Data_Modifica >= UMA.Data_Ultima_Modifica_Richiesta_Testata")

                stb.AppendLine("")
                stb.AppendLine(" 	UNION")
                stb.AppendLine("")

                stb.AppendLine(" 	--Estraggo le analisi associate alle particelle degli appezzamenti")
                stb.AppendLine(" 	SELECT")
                stb.AppendLine(" 		3 AS TipoCod, '3 - ANALISI SU CATASTO' AS TipoDes")
                stb.AppendLine(" 		, ap.Piva, ap.Sa_Cod, ap.Appezza, ap.id_Reg, at.Data_Modifica, at.Analisi_Testata_Cod")
                stb.AppendLine(" 	FROM Analisi_Testata [at]")
                stb.AppendLine(" 	JOIN Analisi_EntitaxTestata aet ON")
                stb.AppendLine(" 	    aet.Analisi_Testata_Cod = at.Analisi_Testata_Cod")
                stb.AppendLine(" 	AND aet.Analisi_Entita_Cod = " & enum_Entita_Analisi.Particella & " ")
                stb.AppendLine(" 	JOIN #AppezzamentixParticelle ap ON")
                stb.AppendLine(" 	    ap.piva = aet.Piva")
                stb.AppendLine(" 	AND ap.SA_COD = aet.Sa_Cod")
                stb.AppendLine(" 	AND ap.prov = aet.prov")
                stb.AppendLine(" 	AND ap.com = aet.com")
                stb.AppendLine(" 	AND ap.sezione = aet.sezione")
                stb.AppendLine(" 	AND ap.FOGLIO = aet.FOGLIO")
                stb.AppendLine(" 	AND ap.NUMERO = aet.NUMERO")
                stb.AppendLine(" 	AND ap.SUBALTERNO = aet.SUBALTERNO")
                stb.AppendLine("  WHERE 1 = 1")
                stb.AppendLine("  AND [at].Data_Modifica >= ap.Data_Ultima_Modifica_Richiesta_Testata")

                stb.AppendLine("")
                stb.AppendLine(" 	UNION")
                stb.AppendLine("")

                stb.AppendLine(" 	--Estraggo le analisi associate ai campi x appezzamenti")
                stb.AppendLine(" 	SELECT")
                stb.AppendLine(" 		5 AS TipoCod, '5 - ANALISI SU CENTRO' AS TipoDes")
                stb.AppendLine(" 		, UMA.Piva, UMA.Sa_Cod, UMA.Appezza, UMA.id_Reg, at.Data_Modifica, at.Analisi_Testata_Cod")
                stb.AppendLine(" 	FROM Analisi_Testata [at]")
                stb.AppendLine(" 	JOIN Analisi_EntitaxTestata aet ON")
                stb.AppendLine(" 	    aet.Analisi_Testata_Cod = at.Analisi_Testata_Cod")
                stb.AppendLine(" 	AND aet.Analisi_Entita_Cod = " & enum_Entita_Analisi.Centro & " ")
                stb.AppendLine(" 	-- Filtro sugli Appezzamenti passati come parametri")
                stb.AppendLine(" 	JOIN #UMA_RichiestexReg_Impianti UMA ON")
                stb.AppendLine(" 	    UMA.PIVA = aet.PIVA COLLATE DATABASE_DEFAULT")
                stb.AppendLine(" 	AND UMA.SA_COD = aet.SA_COD")
                stb.AppendLine("  WHERE 1 = 1")
                stb.AppendLine("  AND [at].Data_Modifica >= UMA.Data_Ultima_Modifica_Richiesta_Testata")

                stb.AppendLine(") LOG_Analisi")

                stb.AppendLine("")
                stb.AppendLine("--Estraggo i log delle analisi del terreno cancellate, oppure dalle quali è stata potenzialmente rimosso un elemento")
                stb.AppendLine(" SELECT ")
                stb.AppendLine("    UMA.PIVA, MAX(Data_Ora_RegistrazioneLog) AS Data_Modifica ")
                stb.AppendLine(" INTO #LOG_AnalisiCancellazione ")
                stb.AppendLine(" FROM Agronica_Log_Analisi ")
                stb.AppendLine(" INNER JOIN #UMA_RichiestexReg_Impianti UMA ON UMA.Piva = Agronica_Log_Analisi.Piva ")
                stb.AppendLine(" WHERE Analisi_Testata_Cod NOT IN (SELECT Analisi_Testata_Cod FROM #LOG_Analisi) ")
                stb.AppendLine($" AND Tipo_Operazione IN ({CInt(enum_TipoOperazioneDB.Modifica)}, {CInt(enum_TipoOperazioneDB.Cancellazione)}) ")
                stb.AppendLine(" AND Data_Ora_RegistrazioneLog >= UMA.Data_Ultima_Modifica_Richiesta_Testata ")
                stb.AppendLine(" GROUP BY UMA.Piva ")
            End If

            stb.AppendLine("SELECT")

            If filtroSuLogOpAnagraficheSuccessive Then
                stb.AppendLine(" DISTINCT")
            End If

            Select Case Selezione_Variabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi, enumSelezioneVariabile.Selezione_JoinDescrizioni

                    stb.AppendLine("     Piva_Impianto")
                    stb.AppendLine("   , urxri.Sa_Cod ")
                    stb.AppendLine("   , urxri.Appezza ")
                    stb.AppendLine("   , urxri.ID_Reg")
                    stb.AppendLine("   , Gruppo_Colturale_UMA")
                    If Selezione_Variabile = enumSelezioneVariabile.Selezione_JoinDescrizioni Then
                        stb.AppendLine("   , Macrouso_UMA_Des")
                    End If
                    stb.AppendLine("   , Programmazione_Cod")
                    stb.AppendLine("   , Totale_Superficie_UMA")
                    stb.AppendLine("   , Zona_Pendenza_A_UMA")
                    stb.AppendLine("   , Zona_Pendenza_B_UMA")
                    stb.AppendLine("   , Zona_Tessitura_Normale_UMA")
                    stb.AppendLine("   , Zona_Tessitura_Normale_UMA_Edit")
                    stb.AppendLine("   , Zona_Tessitura_Media_UMA")
                    stb.AppendLine("   , Zona_Tessitura_Media_UMA_Edit")
                    stb.AppendLine("   , Zona_Tessitura_Tenace_UMA")
                    stb.AppendLine("   , Zona_Tessitura_Tenace_UMA_Edit")

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    stb.AppendLine("    ur.*, ")
                    stb.AppendLine("    urxri.Piva_Impianto, ")
                    stb.AppendLine("    urxri.Sa_Cod, ")
                    stb.AppendLine("    urxri.Appezza, ")
                    stb.AppendLine("    urxri.ID_Reg, ")
                    stb.AppendLine("    um.Macrouso_UMA_Des, ")
                    stb.AppendLine("    ri.*, ")
                    stb.AppendLine("    ri.Validita_Inizio Validita_Inizio_Impianto, ")
                    stb.AppendLine("    ri.Validita_Fine Validita_Fine_Impianto, ")
                    stb.AppendLine("    a.* ")

            End Select

            stb.AppendLine(" FROM UMA_Richieste ur")

            stb.AppendLine(" INNER JOIN UMA_RichiestexReg_Impianti urxri ON ")
            If rendicontazione_terzista Then
                stb.AppendLine("    ur.Piva = urxri.Piva_Impianto ")
            Else
                stb.AppendLine("    ur.Piva = urxri.Piva_Richiesta ")
            End If
            stb.AppendLine(" AND ur.Gruppo_Colturale_UMA = urxri.Gruppo_Colturale")
            stb.AppendLine(" AND ur.Richiesta_Cod = urxri.Richiesta_Cod")

            Select Case Selezione_Variabile

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    stb.AppendLine(" LEFT JOIN UMA_Macrousi um ON ")
                    stb.AppendLine("    ur.Gruppo_Colturale_UMA = um.Macrouso_UMA_Cod")

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    stb.AppendLine(" LEFT JOIN UMA_Macrousi um ON ")
                    stb.AppendLine("    ur.Gruppo_Colturale_UMA = um.Macrouso_UMA_Cod")
                    stb.AppendLine(" LEFT JOIN Reg_Impianti ri ")
                    stb.AppendLine("    ON urxri.Piva_Impianto = ri.Piva ")
                    stb.AppendLine("    AND urxri.Sa_Cod = ri.Sa_Cod ")
                    stb.AppendLine("    AND urxri.Appezza = ri.Appezza ")
                    stb.AppendLine("    AND urxri.ID_Reg = ri.ID_Reg ")
                    stb.AppendLine(" LEFT JOIN Appezzamento a ")
                    stb.AppendLine("    ON ri.PIVA = a.PIVA ")
                    stb.AppendLine("    AND ri.SA_COD = a.SA_COD ")
                    stb.AppendLine("    AND ri.APPEZZA = a.APPEZZA ")

            End Select

            If filtroSuLogOpAnagraficheSuccessive Then
                stb.AppendLine(" LEFT JOIN #LOG_Anagrafe LOG_Anagrafe ON")
                stb.AppendLine("     urxri.Piva_Impianto = LOG_Anagrafe.Piva")
                stb.AppendLine(" AND urxri.Sa_Cod = LOG_Anagrafe.Sa_Cod")
                stb.AppendLine(" AND urxri.Appezza = LOG_Anagrafe.Appezza")
                stb.AppendLine(" AND urxri.ID_Reg = LOG_Anagrafe.ID_Reg")
                stb.AppendLine(" AND LOG_Anagrafe.Data_Modifica >= ur.Data_Modifica")

                stb.AppendLine(" LEFT JOIN #LOG_Analisi LOG_Analisi ON")
                stb.AppendLine("     urxri.Piva_Impianto = LOG_Analisi.Piva")
                stb.AppendLine(" AND urxri.Sa_Cod = LOG_Analisi.Sa_Cod")
                stb.AppendLine(" AND urxri.Appezza = LOG_Analisi.Appezza")
                stb.AppendLine(" AND urxri.ID_Reg = LOG_Analisi.ID_Reg")
                stb.AppendLine(" AND LOG_Analisi.Data_Modifica >= ur.Data_Modifica")

                stb.AppendLine(" LEFT JOIN #LOG_AnalisiCancellazione LOG_AnalisiCancellazione ON")
                stb.AppendLine("     urxri.Piva_Impianto = LOG_AnalisiCancellazione.Piva")
                stb.AppendLine(" AND LOG_AnalisiCancellazione.Data_Modifica >= ur.Data_Modifica")

                'stb.AppendLine(" 	--Se la richiesta è eventualmente tornata in compilazione da stato successivo")
                stb.AppendLine(" LEFT JOIN UMA_Richieste_Testata urt ON")
                stb.AppendLine("     ur.Richiesta_Cod = urt.Richiesta_Cod")
                stb.AppendLine(" LEFT JOIN Pratiche_Stati ps ON")
                stb.AppendLine("     urt.Pratica_Cod = ps.Pratica_Cod")
                stb.AppendLine(" AND ps.Stato_Cod = " & enum_WAnagraficaStati.In_Compilazione & " ")
                stb.AppendLine(" AND ps.Data_Creazione >= ur.Data_Modifica")
            End If

            stb.AppendLine(" WHERE 1 = 1")
            stb.AppendLine(" AND ur.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "' ")
            stb.AppendLine(" AND ur.Richiesta_Cod = " + Agro_SQL_SaveNum(richiesta_cod) + " ")

            If filtroSuLogOpAnagraficheSuccessive Then
                stb.AppendLine(" AND (LOG_Anagrafe.Piva IS NOT NULL")
                stb.AppendLine("   OR LOG_Analisi.Piva IS NOT NULL")
                stb.AppendLine("   OR LOG_AnalisiCancellazione.Piva IS NOT NULL")
                stb.AppendLine("   OR ur.Data_Creazione > urt.Data_Modifica")
                stb.AppendLine("   OR ps.Pratica_Cod IS NOT NULL)")
            End If

            If (gruppo_uma <> "") Then
                stb.AppendLine(" AND ur.Gruppo_Colturale_UMA = '" + Agro_SQL_SaveText(gruppo_uma) + "' ")
            End If

            If Programmazione_Cod <> 0 Then
                stb.AppendLine(" AND ur.Programmazione_Cod = " + Agro_SQL_SaveNum(Programmazione_Cod) + " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            If filtroSuLogOpAnagraficheSuccessive Then
                stb.AppendLine("")
                stb.AppendLine(" DROP TABLE #UMA_RichiestexReg_Impianti")
                stb.AppendLine(" DROP TABLE #LOG_Anagrafe")
                stb.AppendLine(" DROP TABLE #LOG_Analisi")
                stb.AppendLine(" DROP TABLE #AppezzamentixParticelle")
                stb.AppendLine(" DROP TABLE #LOG_AnalisiCancellazione")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            If filtroSuLogOpAnagraficheSuccessive Then
                stb.Clear()

                stb.AppendLine(" IF OBJECT_ID('tempdb..#UMA_RichiestexReg_Impianti') IS NOT NULL")
                stb.AppendLine("    DROP TABLE #UMA_RichiestexReg_Impianti")
                stb.AppendLine(" IF OBJECT_ID('tempdb..#LOG_Anagrafe') IS NOT NULL")
                stb.AppendLine("    DROP TABLE #LOG_Anagrafe")
                stb.AppendLine(" IF OBJECT_ID('tempdb..#LOG_Analisi') IS NOT NULL")
                stb.AppendLine("    DROP TABLE #LOG_Analisi")
                stb.AppendLine(" IF OBJECT_ID('tempdb..#AppezzamentixParticelle') IS NOT NULL")
                stb.AppendLine("    DROP TABLE #AppezzamentixParticelle")
                stb.AppendLine(" IF OBJECT_ID('tempdb..#LOG_AnalisiCancellazione') IS NOT NULL")
                stb.AppendLine("    DROP TABLE #LOG_AnalisiCancellazione")

                EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            End If

        End Try

        Return dt

    End Function

    Public Function Leggi_Richieste_x_Impianto_xControlliAnagrafica(ByVal piva As String,
                                                                    ByVal sa_cod As Integer,
                                                                    ByVal appezza As Integer,
                                                                    ByVal id_reg As Integer,
                                                                    ByRef objParametri As AgronicaCoreParametri
                                                                    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_RichiesteXReg_Impianti_R.Leggi_Richieste_x_Impianto_xControlliAnagrafica()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try
            stb.AppendLine(" SELECT 1 ")
            stb.AppendLine(" FROM UMA_RichiestexReg_Impianti")
            stb.AppendLine($" WHERE Piva_Impianto = '{Agro_SQL_SaveText(piva)}' ")
            stb.AppendLine($" AND Sa_Cod = {Agro_SQL_SaveNum(sa_cod)} ")
            stb.AppendLine($" AND Appezza = {Agro_SQL_SaveNum(appezza)} ")
            stb.AppendLine($" AND Id_Reg = {Agro_SQL_SaveNum(id_reg)} ")

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

    Public Function Controlla_Presenza_Impianti_Cessati(ByVal piva As String,
                                                        ByVal richiesta_cod As Integer,
                                                        ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_RichiesteXReg_Impianti_R.Controlla_Presenza_Impianti_Cessati()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try
            stb.AppendLine(" SELECT 1 ")
            stb.AppendLine(" FROM UMA_RichiestexReg_Impianti uri")
            stb.AppendLine(" JOIN Reg_Impianti ri on ri.Piva = uri.Piva_Impianto AND ri.Sa_Cod = uri.Sa_Cod AND ri.Appezza = uri.Appezza AND ri.ID_Reg = uri.ID_Reg")
            stb.AppendLine($" WHERE Piva_Richiesta = '{Agro_SQL_SaveText(piva)}' ")
            stb.AppendLine($" AND Richiesta_Cod = {Agro_SQL_SaveNum(richiesta_cod)} ")
            stb.AppendLine(" AND ri.FlagCessata <> 0 AND ri.FlagCessata IS NOT NULL ")

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

Public Class UMA_RichiesteXReg_Impianti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Private Const ChunkSize As Integer = 200

    Public Function InserisciAssociazioniPerNuovaPraticaContoProprio(ByVal dtInserimenti As DataTable,
                                                                       ByVal richiestaCod As Integer,
                                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_RichiesteXReg_Impianti_W.InserisciAssociazioniPerNuovaPraticaContoProprio()"
        Dim res As Boolean = True

        Try
            Dim tables = SplitDataTable(dtInserimenti, ChunkSize)
            For Each part As DataTable In tables
                Dim stb As New StringBuilder
                stb.AppendLine(" INSERT INTO [dbo].[UMA_RichiestexReg_Impianti] ")
                stb.AppendLine("  ([Piva_Richiesta] ")
                stb.AppendLine("  ,[Gruppo_Colturale] ")
                stb.AppendLine("  ,[Richiesta_Cod] ")
                stb.AppendLine("  ,[Piva_Impianto] ")
                stb.AppendLine("  ,[Sa_Cod] ")
                stb.AppendLine("  ,[Appezza] ")
                stb.AppendLine("  ,[ID_Reg] ")
                stb.AppendLine("  ,[inviato] ")
                stb.AppendLine("  ,[datainvio] ")
                stb.AppendLine("  ,[Data_Creazione] ")
                stb.AppendLine("  ,[Data_Modifica] ")
                stb.AppendLine("  ,[Username_Creazione] ")
                stb.AppendLine("  ,[Username_Modifica] ")
                stb.AppendLine("  ,[Validita_Inizio] ")
                stb.AppendLine("  ,[Validita_Fine]) ")
                stb.AppendLine("  VALUES ")

                For Each row As DataRow In part.Rows
                    stb.AppendLine(" ( ")
                stb.AppendLine("  '" & Agro_SQL_SaveText(CStr(row.Item("Piva"))) & "' ")
                stb.AppendLine("  , '" & Agro_SQL_SaveText(CStr(row.Item("Macrouso_UMA_Cod"))) & "' ")
                    stb.AppendLine("  , " & richiestaCod & " ")
                stb.AppendLine("  , '" & Agro_SQL_SaveText(CStr(row.Item("Piva"))) & "' ")
                    stb.AppendLine("  , " & CStr(row.Item("SA_COD")) & " ")
                    stb.AppendLine("  , " & CStr(row.Item("APPEZZA")) & " ")
                    stb.AppendLine("  , " & CStr(row.Item("ID_REG")) & " ")
                    stb.AppendLine("  ,0 ")
                    stb.AppendLine("  ,NULL ")
                    stb.AppendLine("  , " & Agro_SQL_SaveDate(Date.Now) & " ")
                    stb.AppendLine("  , " & Agro_SQL_SaveDate(Date.Now) & " ")
                    stb.AppendLine("  , '" & objParametri.UtenteUsername & "' ")
                    stb.AppendLine("  , '" & objParametri.UtenteUsername & "' ")
                    stb.AppendLine("  , " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
                    stb.AppendLine("  , " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")
                    stb.AppendLine(" ), ")
                Next

                stb.Remove(stb.Length - 4, 2)
                res = res AndAlso EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            Next
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            res = False
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return res
    End Function

    Public Function InserisciAssociazioniPerNuovaPraticaContoTerzi(ByVal pivaRichiesta As String,
                                                                   ByVal dtInserimenti As DataTable,
                                                                       ByVal richiestaCod As Integer,
                                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_RichiesteXReg_Impianti_W.InserisciAssociazioniPerNuovaPraticaContoTerzi()"
        Dim res As Boolean = True

        Try
            Dim tables = SplitDataTable(dtInserimenti, ChunkSize)
            For Each part As DataTable In tables
                Dim stb As New StringBuilder
                stb.AppendLine(" INSERT INTO [dbo].[UMA_RichiestexReg_Impianti] ")
                stb.AppendLine("  ([Piva_Richiesta] ")
                stb.AppendLine("  ,[Gruppo_Colturale] ")
                stb.AppendLine("  ,[Richiesta_Cod] ")
                stb.AppendLine("  ,[Piva_Impianto] ")
                stb.AppendLine("  ,[Sa_Cod] ")
                stb.AppendLine("  ,[Appezza] ")
                stb.AppendLine("  ,[ID_Reg] ")
                stb.AppendLine("  ,[inviato] ")
                stb.AppendLine("  ,[datainvio] ")
                stb.AppendLine("  ,[Data_Creazione] ")
                stb.AppendLine("  ,[Data_Modifica] ")
                stb.AppendLine("  ,[Username_Creazione] ")
                stb.AppendLine("  ,[Username_Modifica] ")
                stb.AppendLine("  ,[Validita_Inizio] ")
                stb.AppendLine("  ,[Validita_Fine]) ")
                stb.AppendLine("  VALUES ")

                For Each row As DataRow In part.Rows
                    stb.AppendLine(" ( ")
                stb.AppendLine("  '" & Agro_SQL_SaveText(pivaRichiesta) & "' ")
                stb.AppendLine("  , '" & Agro_SQL_SaveText(CStr(row.Item("Macrouso_UMA_Cod"))) & "' ")
                    stb.AppendLine("  , " & richiestaCod & " ")
                stb.AppendLine("  , '" & Agro_SQL_SaveText(CStr(row.Item("Piva"))) & "' ")
                    stb.AppendLine("  , " & CStr(row.Item("SA_COD")) & " ")
                    stb.AppendLine("  , " & CStr(row.Item("APPEZZA")) & " ")
                    stb.AppendLine("  , " & CStr(row.Item("ID_REG")) & " ")
                    stb.AppendLine("  ,0 ")
                    stb.AppendLine("  ,NULL ")
                    stb.AppendLine("  , " & Agro_SQL_SaveDate(Date.Now) & " ")
                    stb.AppendLine("  , " & Agro_SQL_SaveDate(Date.Now) & " ")
                    stb.AppendLine("  , '" & objParametri.UtenteUsername & "' ")
                    stb.AppendLine("  , '" & objParametri.UtenteUsername & "' ")
                    stb.AppendLine("  , " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
                    stb.AppendLine("  , " & Agro_SQL_SaveDate(AGRODATAFINE) & "  ")
                    stb.AppendLine(" ), ")

                Next

                stb.Remove(stb.Length - 4, 2)
                res = res AndAlso EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            Next
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            res = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return res

    End Function

    Public Function EliminaAssociazioni(ByVal pivaRichiesta As String,
                                                                   ByVal deleteList As List(Of Tuple(Of String, String)),
                                                                       ByVal richiestaCod As Integer,
                                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_RichiesteXReg_Impianti_W.EliminaAssociazioniPerPraticaContoTerzi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim res As Boolean

        Try

            stb.Length = 0

            stb.AppendLine(" DELETE FROM [dbo].[UMA_RichiestexReg_Impianti] ")
            stb.AppendLine("  WHERE ")
            stb.AppendLine(" Piva_Richiesta = '" & Agro_SQL_SaveText(pivaRichiesta) & "' ")
            stb.AppendLine(" AND richiesta_Cod = " & Agro_SQL_SaveNum(richiestaCod) & " ")

            If deleteList.Count > 0 Then

                stb.AppendLine(" AND ( ")

                For Each tupla In deleteList

                    stb.AppendLine(" (  ")

                    stb.AppendLine(" Piva_Impianto = '" & Agro_SQL_SaveText(tupla.Item1) & "' AND Gruppo_Colturale = '" & Agro_SQL_SaveText(tupla.Item2) & "' ")

                    stb.AppendLine(" ) OR ")

                Next

                stb.Remove(stb.Length - 5, 3)

                stb.AppendLine(" ) ")

            End If

            '--------------------------------------------------------------------------
            res = EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            res = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return res

    End Function

    Public Function EliminaAssociazioni(ByVal Piva_Richiesta As String,
                                        ByVal Richiesta_Cod As Integer,
                                        ByVal dtCancellazioni As DataTable,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_RichiesteXReg_Impianti_W.EliminaAssociazioni()"

        Dim messaggioErrore As String = ""
        Dim res As Boolean = True

        Try
            For Each row In dtCancellazioni.Rows
                Dim stb As New StringBuilder
                stb.Length = 0

                stb.AppendLine(" DELETE FROM [dbo].[UMA_RichiestexReg_Impianti] ")
                stb.AppendLine(" WHERE ")
                stb.AppendLine("    Piva_Richiesta = '" & Agro_SQL_SaveText(Piva_Richiesta) & "' ")
                stb.AppendLine("    AND Gruppo_Colturale = '" & Agro_SQL_SaveText(CStr(row.Item("Gruppo_Colturale_UMA"))) & "' ")
                stb.AppendLine("    AND Richiesta_Cod = " & Agro_SQL_SaveNum(Richiesta_Cod) & " ")
                stb.AppendLine("    AND Piva_Impianto = '" & Agro_SQL_SaveText(CStr(row.Item("Piva_Impianto"))) & "' ")
                stb.AppendLine("    AND Sa_Cod = " & CStr(row.Item("Sa_Cod")) & " ")
                stb.AppendLine("    AND Appezza = " & CStr(row.Item("Appezza")) & " ")
                stb.AppendLine("    AND ID_Reg = " & CStr(row.Item("ID_Reg")) & " ")

                '--------------------------------------------------------------------------
                res = res And EseguiQuery_Scrittura(objParametri, stb.ToString, nomeRoutine)
                '--------------------------------------------------------------------------
            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            res = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return res
    End Function

    Private Function SplitDataTable(dt As DataTable, chunkSize As Integer) As List(Of DataTable)
        Dim tables As New List(Of DataTable)
        Dim totalRows = dt.Rows.Count
        Dim currentIndex = 0

        While currentIndex < totalRows
            Dim newTable = dt.Clone()
            For i = currentIndex To Math.Min(currentIndex + chunkSize - 1, totalRows - 1)
                newTable.ImportRow(dt.Rows(i))
            Next
            tables.Add(newTable)
            currentIndex += chunkSize
        End While

        Return tables
    End Function

End Class

