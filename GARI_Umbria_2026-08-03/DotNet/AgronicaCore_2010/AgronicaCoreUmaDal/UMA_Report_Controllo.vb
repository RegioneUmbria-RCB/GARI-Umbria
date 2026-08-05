Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider
Imports System.Runtime.Serialization

Public Class UMA_Report_Controllo_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi_ReportELAS(ByVal bimestre As Integer,
                                     ByVal anno As Integer,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByVal xOrderBy As String,
                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                     ByRef objParametri_Utenti As AgronicaCoreParametri
                                    ) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Report_Controllo_R.Leggi_ReportELAS()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2)
        NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

        Try

            stb.Length = 0

            UMA_Richieste_Testata_R.AppendWithCteTempLav(stb, True)

            UMA_Richieste_Testata_R.AppendCteTempLavParz(stb)

            UMA_Richieste_Testata_R.AppendCteAllevamenti(stb)

            stb.AppendLine(" SELECT DISTINCT x.* FROM ( ")
            stb.AppendLine(" SELECT t.Richiesta_Cod AS ID, imp.rag_soc As azienda, ic.val_cod AS CUAA, p.Numero AS nRichiesta, p.Anno AS annoRichiesta,  ")
            stb.AppendLine(" ISTAT.Localita AS citta, i.ind_des AS via, i.pro_cod AS Prov, ")
            stb.AppendLine(" psa.Validita_Inizio As ultimo_Avanzamento,  ")

            UMA_Richieste_Testata_R.AppendCarburanteAssegnato(stb)

            stb.AppendLine(" IIf(t.Tipo_Richiesta = 0, IIf(t.Avanzamento_Richiesta = -1, 'Anticipo Conto Proprio', 'Conto Proprio'), IIf(t.Avanzamento_Richiesta = -1, 'Anticipo Conto Terzi', 'Conto Terzi')) AS tipo_richiesta  ")

            'From
            stb.AppendLine(" FROM UMA_Richieste_Testata t ")

            'Join #CTE
            UMA_Richieste_Testata_R.AppendJoinCteTemp(stb)

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

            stb.AppendLine(" WHERE ")

            stb.AppendLine(" t.Avanzamento_Richiesta  IN (-1, 0) ")

            If (anno > 0) Then
                stb.AppendLine(" AND p.Anno = '" & Agro_SQL_SaveText(anno, False) & "' ")
            Else
                stb.AppendLine(" AND p.Anno = '" & Agro_SQL_SaveText(Date.Now.Year, False) & "' ")
            End If

            stb.AppendLine(" AND psaw.WAnagraficaStati_Cod = 2005 ")

            If bimestre > 0 Then
                stb.AppendLine("AND MONTH(psa.Validita_Inizio) between " & (bimestre * 2 - 1).ToString & " and " & (bimestre * 2).ToString & " ")
                stb.AppendLine("AND YEAR(psa.Validita_Inizio) = " & Agro_SQL_SaveNum(anno) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy) & " ")
            End If

            stb.AppendLine(" ) As x ")

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

    Public Function Leggi_ElencoInadempienti(ByVal anno As Integer,
                                             ByVal prov As String,
                                             ByVal com As String,
                                             ByVal statoPrat As String,
                                             ByVal conto As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreParametri
                                            ) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Report_Controllo_R.Leggi_ElencoInadempienti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2)
        NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

        prov = Trim(prov)
        If prov.Length < 3 Then
            If prov.Length = 1 Then
                prov = "00" & prov
            End If
            If prov.Length = 2 Then
                prov = "0" & prov
            End If
        End If

        com = Trim(com)
        If com.Length < 3 Then
            If com.Length = 1 Then
                com = "00" & com
            End If
            If com.Length = 2 Then
                com = "0" & com
            End If
        End If

        ' Il calcolo dell'assegnato viene effettuato SOLO su prime richieste e rendicontazioni anno corrente.
        ' Per le richieste viene calcolato SOLO l'assegnato relativo alla prima richiesta, senza considerare le integrative.
        Dim calcolaAssegnato As Boolean = False

        ' Usa temporanee
        Dim usaTemp = True

        Try

            stb.Length = 0

            If usaTemp Then
                AppendCreaTempAcquistato(anno, stb, "#cte_Acquistato_Gasolio", 2)
                AppendCreaTempAcquistato(anno, stb, "#cte_Acquistato_Benzina", 3)
                AppendCreaTempAcquistato(anno, stb, "#cte_Acquistato_Gasolio_Serra", 8)
            End If

            If calcolaAssegnato Then
                UMA_Richieste_Testata_R.AppendWithCteTempLav(stb, True)
                UMA_Richieste_Testata_R.AppendCteTempLavParz(stb)
                UMA_Richieste_Testata_R.AppendCteAllevamenti(stb)
                UMA_Richieste_Testata_R.AppendCteRendicontazioni(stb, anno, False)
            Else
                UMA_Richieste_Testata_R.AppendCteRendicontazioni(stb, anno, True)
            End If

            UMA_Richieste_Testata_R.AppendCteRichieste(stb, anno, False, False, False)

            UMA_Richieste_Testata_R.AppendCteAnticipi(stb, anno, False, False)

            AppendCteProvECom(stb, prov, com, False)

            If Not usaTemp Then
                UMA_Richieste_Testata_R.AppendCteAcquistato(stb, TipiEnumerativi.enum_TipoCarburante_UMA.Gasolio, False, 0)
                UMA_Richieste_Testata_R.AppendCteAcquistato(stb, TipiEnumerativi.enum_TipoCarburante_UMA.Benzina, False, 0)
                UMA_Richieste_Testata_R.AppendCteAcquistato(stb, TipiEnumerativi.enum_TipoCarburante_UMA.Gasolio_Serra, False, 0)
            End If

            '--------------------------------------------------------------------------------
            'RICHIESTE
            '--------------------------------------------------------------------------------
            '- Rendicontazione compilata non presente
            '--------------------------------------------------------------------------------

            stb.AppendLine("Select ")
            stb.AppendLine("  'Richiesta' Pratica")
            stb.AppendLine(",p.Numero")
            stb.AppendLine(", year(t.Validita_Inizio) Anno")
            stb.AppendLine(", case t.Tipo_Richiesta when 0 then 'C/Proprio' when -1 then 'C/Terzi' end Tipo")
            stb.AppendLine(", i.rag_soc Azienda")
            stb.AppendLine(", cte_pc.CUAA")
            stb.AppendLine(", cte_pc.Prov")
            stb.AppendLine(", cte_pc.citta")
            stb.AppendLine(", cte_pc.via")
            stb.AppendLine(", cast(cte_rend.Data_Prima_Compilazione as date) Data_Comp_Rend")
            stb.AppendLine(", ISNULL(cte_acq_gas.Acquistato, 0) as Acquistato_Gasolio")
            stb.AppendLine(", ISNULL(cte_acq_benz.Acquistato, 0) as Acquistato_Benzina")
            stb.AppendLine(", ISNULL(cte_acq_gas_serra.Acquistato, 0) as Acquistato_Gasolio_Serra")
            stb.AppendLine(", ISNULL(t.Rimanenza_Gasolio, 0) as Rimanenza_Iniziale_Gasolio")
            stb.AppendLine(", ISNULL(t.Rimanenza_Benzina, 0) as Rimanenza_Iniziale_Benzina")
            stb.AppendLine(", ISNULL(t.Rimanenza_Gasolio_Serra, 0) as Rimanenza_Iniziale_Gasolio_Serra")
            stb.AppendLine(", 0 as Rimanenza_Finale_Gasolio")
            stb.AppendLine(", 0 as Rimanenza_Finale_Benzina")
            stb.AppendLine(", 0 as Rimanenza_Finale_Gasolio_Serra")
            If calcolaAssegnato Then
                UMA_Richieste_Testata_R.AppendCarburanteAssegnato(stb, True)
            End If
            stb.AppendLine(", 'Rendicontazione compilata non presente' Inadempienza")
            stb.AppendLine("from UMA_Richieste_Testata t")
            stb.AppendLine("inner join Pratiche p on p.Pratica_Cod = t.Pratica_Cod")
            stb.AppendLine("inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = p.Pratica_Cod")
            stb.AppendLine("inner join Imprese i on i.PIVA = t.piva")
            stb.AppendLine("inner join cte_prov_com cte_pc on cte_pc.piva = t.Piva ")
            stb.AppendLine("left join cte_rendicontazioni cte_rend on cte_rend.Piva = t.Piva and")
            stb.AppendLine("                                          cte_rend.Anno = " & Agro_SQL_SaveNum(anno) & " and")
            stb.AppendLine("                                          cte_rend.Tipo_Richiesta = t.Tipo_Richiesta")
            UMA_Richieste_Testata_R.AppendJoinCteAcquistato(stb, anno)
            If calcolaAssegnato Then
                UMA_Richieste_Testata_R.AppendJoinCteTemp(stb)
            End If
            stb.AppendLine("where year(t.Validita_Inizio) = " & Agro_SQL_SaveNum(anno) & " ")
            stb.AppendLine("and   t.Avanzamento_Richiesta = 0 ")
            stb.AppendLine("and   t.Richiesta_Integrativa = 0 ")
            stb.AppendLine("and   psa.Stato_Cod = 2005 ")
            If conto = 0 OrElse conto = -1 Then
                stb.AppendLine("and   t.Tipo_Richiesta = " & Agro_SQL_SaveNum(conto) & " ")
            End If
            'If statoPrat <> "-1" Then
            '    stb.AppendLine(" and psa.Stato_Cod = " & Agro_SQL_SaveText(statoPrat) & " ")
            'End If
            stb.AppendLine("and cte_rend.Piva Is null ")

            '--------------------------------------------------------------------------------
            'ANTICIPI
            '--------------------------------------------------------------------------------
            '- Rendicontazione compilata non presente
            '--------------------------------------------------------------------------------

            stb.AppendLine("UNION")
            stb.AppendLine("select ")
            stb.AppendLine("  'Anticipo' Pratica")
            stb.AppendLine(",p.Numero")
            stb.AppendLine(", year(t.Validita_Inizio) Anno")
            stb.AppendLine(", case t.Tipo_Richiesta when 0 then 'C/Proprio' when -1 then 'C/Terzi' end Tipo")
            stb.AppendLine(", i.rag_soc Azienda")
            stb.AppendLine(", cte_pc.CUAA")
            stb.AppendLine(", cte_pc.Prov")
            stb.AppendLine(", cte_pc.citta")
            stb.AppendLine(", cte_pc.via")
            stb.AppendLine(", NULL Data_Comp_Rend")
            stb.AppendLine(", ISNULL(cte_acq_gas.Acquistato, 0) as Acquistato_Gasolio")
            stb.AppendLine(", ISNULL(cte_acq_benz.Acquistato, 0) as Acquistato_Benzina")
            stb.AppendLine(", ISNULL(cte_acq_gas_serra.Acquistato, 0) as Acquistato_Gasolio_Serra")
            stb.AppendLine(", 0 as Rimanenza_Iniziale_Gasolio")
            stb.AppendLine(", 0 as Rimanenza_Iniziale_Benzina")
            stb.AppendLine(", 0 as Rimanenza_Iniziale_Gasolio_Serra")
            stb.AppendLine(", 0 as Rimanenza_Finale_Gasolio")
            stb.AppendLine(", 0 as Rimanenza_Finale_Benzina")
            stb.AppendLine(", 0 as Rimanenza_Finale_Gasolio_Serra")
            If calcolaAssegnato Then
                stb.AppendLine(", 0 as assegnato_gasolio")
                stb.AppendLine(", 0 as assegnato_benzina")
                stb.AppendLine(", 0 as assegnato_gasolio_serra")
            End If
            stb.AppendLine(", 'Rendicontazione compilata non presente' Inadempienza")
            stb.AppendLine("from UMA_Richieste_Testata t")
            stb.AppendLine("inner join Pratiche p on p.Pratica_Cod = t.Pratica_Cod")
            stb.AppendLine("inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = p.Pratica_Cod")
            stb.AppendLine("inner join Imprese i on i.PIVA = t.piva")
            stb.AppendLine("inner join cte_prov_com cte_pc on cte_pc.piva = t.Piva ")
            stb.AppendLine("left join cte_richieste cte_rich on cte_rich.Piva = t.piva and")
            stb.AppendLine("                                    cte_rich.Anno = " & Agro_SQL_SaveNum(anno) & " and")
            stb.AppendLine("                                    cte_rich.Tipo_Richiesta = t.Tipo_Richiesta")
            UMA_Richieste_Testata_R.AppendJoinCteAcquistato(stb, anno)
            stb.AppendLine("where year(t.Validita_Inizio) = " & Agro_SQL_SaveNum(anno) & " ")
            stb.AppendLine("and   t.Avanzamento_Richiesta = -1 ")
            stb.AppendLine("and   psa.Stato_Cod = 2005 ")
            stb.AppendLine("and   cte_rich.piva is null ")
            If conto = 0 OrElse conto = -1 Then
                stb.AppendLine("and   t.Tipo_Richiesta = " & Agro_SQL_SaveNum(conto) & " ")
            End If
            'If statoPrat <> "-1" Then
            '    stb.AppendLine(" and psa.Stato_Cod = " & Agro_SQL_SaveText(statoPrat) & " ")
            'End If

            '--------------------------------------------------------------------------------
            'RENDICONTAZIONI 
            '--------------------------------------------------------------------------------
            '- Rendicontazione successiva alla scadenza
            '--------------------------------------------------------------------------------

            stb.AppendLine("UNION")
            stb.AppendLine("select ")
            stb.AppendLine("  'Rendicontazioni' Pratica")
            stb.AppendLine(",p.Numero")
            stb.AppendLine(", year(t.Validita_Inizio) Anno")
            stb.AppendLine(", case t.Tipo_Richiesta when 0 then 'C/Proprio' when -1 then 'C/Terzi' end Tipo")
            stb.AppendLine(", i.rag_soc Azienda")
            stb.AppendLine(", cte_pc.CUAA")
            stb.AppendLine(", cte_pc.Prov")
            stb.AppendLine(", cte_pc.citta")
            stb.AppendLine(", cte_pc.via")
            stb.AppendLine(", cte_rend.Data_Prima_Compilazione Data_Comp_Rend")
            stb.AppendLine(", ISNULL(cte_acq_gas.Acquistato, 0) as Acquistato_Gasolio")
            stb.AppendLine(", ISNULL(cte_acq_benz.Acquistato, 0) as Acquistato_Benzina")
            stb.AppendLine(", ISNULL(cte_acq_gas_serra.Acquistato, 0) as Acquistato_Gasolio_Serra")
            stb.AppendLine(", ISNULL(cte_rich.Rimanenza_Gasolio, 0) as Rimanenza_Iniziale_Gasolio")
            stb.AppendLine(", ISNULL(cte_rich.Rimanenza_Benzina, 0) as Rimanenza_Iniziale_Benzina")
            stb.AppendLine(", ISNULL(cte_rich.Rimanenza_Gasolio_Serra, 0) as Rimanenza_Iniziale_Gasolio_Serra")
            stb.AppendLine(", ISNULL(t.Rimanenza_Gasolio, 0) as Rimanenza_Finale_Gasolio")
            stb.AppendLine(", ISNULL(t.Rimanenza_Benzina, 0) as Rimanenza_Finale_Benzina")
            stb.AppendLine(", ISNULL(t.Rimanenza_Gasolio_Serra, 0) as Rimanenza_Finale_Gasolio_Serra")
            If calcolaAssegnato Then
                UMA_Richieste_Testata_R.AppendCarburanteAssegnato(stb, True)
            End If
            stb.AppendLine(", 'Rendicontazione successiva alla scadenza' Inadempienza")
            stb.AppendLine("from UMA_Richieste_Testata t")
            stb.AppendLine("inner join Pratiche p on p.Pratica_Cod = t.Pratica_Cod")
            stb.AppendLine("inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = p.Pratica_Cod")
            stb.AppendLine("inner join Imprese i on i.PIVA = t.piva")
            stb.AppendLine("inner join cte_prov_com cte_pc on cte_pc.piva = t.Piva ")
            stb.AppendLine("inner join cte_rendicontazioni cte_rend on cte_rend.Piva = t.Piva and")
            stb.AppendLine("                                           cte_rend.Anno = " & Agro_SQL_SaveNum(anno) & " and")
            stb.AppendLine("                                           cte_rend.Tipo_Richiesta = t.Tipo_Richiesta")
            'stb.AppendLine("inner join UMA_Setup_Date_Rendicontazione setupdate on setupdate.Tipo_Azienda = t.Tipo_Azienda and setupdate.Anno_Richiesta = YEAR(t.validita_inizio)")
            stb.AppendLine("left join cte_richieste cte_rich on cte_rich.Piva = t.piva and")
            stb.AppendLine("                                    cte_rich.Anno = " & Agro_SQL_SaveNum(anno) & " and")
            stb.AppendLine("                                    cte_rich.Tipo_Richiesta = t.Tipo_Richiesta")
            UMA_Richieste_Testata_R.AppendJoinCteAcquistato(stb, anno)
            If calcolaAssegnato Then
                UMA_Richieste_Testata_R.AppendJoinCteTemp(stb)
            End If
            stb.AppendLine("where year(t.Validita_Inizio) = " & Agro_SQL_SaveNum(anno) & " ")
            stb.AppendLine("And   t.Avanzamento_Richiesta = 1")
            stb.AppendLine("and   psa.Stato_Cod = 2005")

            'stb.AppendLine("and   cast(cte_rend.Data_Prima_Compilazione as date) > cast(setupdate.Termine_Ultimo_Rendicontazione as date)")

            Dim scadenzaInadempienti = New Date(anno + 1, 6, 30)
            stb.AppendLine("and   cast(cte_rend.Data_Prima_Compilazione as date) > " & Agro_SQL_SaveDate(scadenzaInadempienti) & " ")

            If conto = 0 OrElse conto = -1 Then
                stb.AppendLine("and   t.Tipo_Richiesta = " & Agro_SQL_SaveNum(conto) & " ")
            End If
            'If statoPrat <> "-1" Then
            '    stb.AppendLine(" and psa.Stato_Cod = " & Agro_SQL_SaveText(statoPrat) & " ")
            'End If

            '--------------------------------------------------------------------------------
            'RENDICONTAZIONI ANNO PRECEDENTE
            '--------------------------------------------------------------------------------
            '- Rendicontazione anno precedente con rimanenze senza pratiche nell'anno
            '--------------------------------------------------------------------------------

            stb.AppendLine("UNION")
            stb.AppendLine("select ")
            stb.AppendLine("  'Rendicontazioni' Pratica")
            stb.AppendLine(", p.Numero Numero")
            stb.AppendLine(", year(t.Validita_Inizio) Anno")
            stb.AppendLine(", case t.Tipo_Richiesta when 0 then 'C/Proprio' when -1 then 'C/Terzi' end Tipo")
            stb.AppendLine(", i.rag_soc Azienda")
            stb.AppendLine(", cte_pc.CUAA")
            stb.AppendLine(", cte_pc.Prov")
            stb.AppendLine(", cte_pc.citta")
            stb.AppendLine(", cte_pc.via")
            stb.AppendLine(", NULL Data_Comp_Rend")
            stb.AppendLine(", 0 as Acquistato_Gasolio")
            stb.AppendLine(", 0 as Acquistato_Benzina")
            stb.AppendLine(", 0 as Acquistato_Gasolio_Serra")
            stb.AppendLine(", 0 as Rimanenza_Iniziale_Gasolio")
            stb.AppendLine(", 0 as Rimanenza_Iniziale_Benzina")
            stb.AppendLine(", 0 as Rimanenza_Iniziale_Gasolio_Serra")
            stb.AppendLine(", ISNULL(t.Rimanenza_Gasolio, 0) as Rimanenza_Finale_Gasolio")
            stb.AppendLine(", ISNULL(t.Rimanenza_Benzina, 0) as Rimanenza_Finale_Benzina")
            stb.AppendLine(", ISNULL(t.Rimanenza_Gasolio_Serra, 0) as Rimanenza_Finale_Gasolio_Serra")
            If calcolaAssegnato Then
                stb.AppendLine(", 0 as assegnato_gasolio")
                stb.AppendLine(", 0 as assegnato_benzina")
                stb.AppendLine(", 0 as assegnato_gasolio_serra")
            End If
            stb.AppendLine(", 'Rendicontazione anno precedente con rimanenze senza pratiche nell''anno' Inadempienza")
            stb.AppendLine("from UMA_Richieste_Testata t")
            stb.AppendLine("inner join Pratiche p on p.Pratica_Cod = t.Pratica_Cod")
            stb.AppendLine("inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = p.Pratica_Cod")
            stb.AppendLine("inner join Imprese i on i.PIVA = t.piva")
            stb.AppendLine("inner join cte_prov_com cte_pc on cte_pc.piva = t.Piva ")
            stb.AppendLine("left join cte_anticipi_richieste cte_ant_rich on cte_ant_rich.Piva = t.piva and")
            stb.AppendLine("                                                       cte_ant_rich.Anno = " & Agro_SQL_SaveNum(anno) & "  and")
            stb.AppendLine("                                                 cte_ant_rich.Tipo_Richiesta = t.Tipo_Richiesta")
            stb.AppendLine("where year(t.Validita_Inizio) = " & Agro_SQL_SaveNum(anno - 1) & " ")
            stb.AppendLine("And   t.Avanzamento_Richiesta = 1")
            stb.AppendLine("and   psa.Stato_Cod = 2005")
            stb.AppendLine("and   (isnull(t.Rimanenza_Benzina,0) + isnull(t.Rimanenza_Gasolio,0) + isnull(t.Rimanenza_Gasolio_Serra,0)) > 0")
            stb.AppendLine("and   cte_ant_rich.Piva is null")
            If conto = 0 OrElse conto = -1 Then
                stb.AppendLine("and   t.Tipo_Richiesta = " & Agro_SQL_SaveNum(conto) & " ")
            End If
            'If statoPrat <> "-1" Then
            '    stb.AppendLine(" and psa.Stato_Cod = " & Agro_SQL_SaveText(statoPrat) & " ")
            'End If

            '--------------------------------------------------------------------------------
            'TRASFERIMENTI RICEVUTI SENZA PRATICHE IN ESSERE
            '--------------------------------------------------------------------------------
            '- Trasferimenti ricevuti senza pratiche in essere
            '--------------------------------------------------------------------------------

            stb.AppendLine("UNION")
            stb.AppendLine("select ")
            stb.AppendLine("  '' Pratica")
            stb.AppendLine(", NULL Numero")
            stb.AppendLine(", " & anno.ToString & " Anno")
            stb.AppendLine(", case trasf.Confermato_Tipo_Rich when 0 then 'C/Proprio' when -1 then 'C/Terzi' end Tipo")
            stb.AppendLine(", i.rag_soc Azienda")
            stb.AppendLine(", cte_pc.CUAA")
            stb.AppendLine(", cte_pc.Prov")
            stb.AppendLine(", cte_pc.citta")
            stb.AppendLine(", cte_pc.via")
            stb.AppendLine(", NULL Data_Comp_Rend")
            stb.AppendLine(", trasf.Confermato_Gasolio as Acquistato_Gasolio")
            stb.AppendLine(", trasf.Confermato_Benzina as Acquistato_Benzina")
            stb.AppendLine(", trasf.Confermato_Gasolio_Serra as Acquistato_Gasolio_Serra")
            stb.AppendLine(", 0 as Rimanenza_Iniziale_Gasolio")
            stb.AppendLine(", 0 as Rimanenza_Iniziale_Benzina")
            stb.AppendLine(", 0 as Rimanenza_Iniziale_Gasolio_Serra")
            stb.AppendLine(", 0 as Rimanenza_Finale_Gasolio")
            stb.AppendLine(", 0 as Rimanenza_Finale_Benzina")
            stb.AppendLine(", 0 as Rimanenza_Finale_Gasolio_Serra")
            If calcolaAssegnato Then
                stb.AppendLine(", 0 as assegnato_gasolio")
                stb.AppendLine(", 0 as assegnato_benzina")
                stb.AppendLine(", 0 as assegnato_gasolio_serra")
            End If
            stb.AppendLine(", 'Trasferimenti ricevuti senza pratiche nell''anno' Inadempienza")
            stb.AppendLine("from UMA_Richieste_Testata t")
            stb.AppendLine("inner join Pratiche p on p.Pratica_Cod = t.Pratica_Cod")
            stb.AppendLine("inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = p.Pratica_Cod")
            stb.AppendLine("inner join UMA_Richieste_Trasferimenti trasf on trasf.Piva_SuperUser = t.Piva_SuperUser ")
            stb.AppendLine("                                            and trasf.Piva = t.Piva ")
            stb.AppendLine("                                            and trasf.Richiesta_Cod = t.Richiesta_Cod ")
            stb.AppendLine("inner join Imprese i on i.PIVA = trasf.Piva_Ricevente")
            stb.AppendLine("inner join cte_prov_com cte_pc on cte_pc.piva = trasf.Piva_Ricevente ")
            stb.AppendLine("left join cte_anticipi_richieste cte_ant_rich on cte_ant_rich.Piva = trasf.Piva_Ricevente and ")
            stb.AppendLine("                                                       cte_ant_rich.Anno = " & Agro_SQL_SaveNum(anno) & "  and ")
            stb.AppendLine("                                                 cte_ant_rich.Tipo_Richiesta = trasf.Confermato_Tipo_Rich ")
            stb.AppendLine("where year(trasf.Data_Trasferimento) = " & Agro_SQL_SaveNum(anno) & " ")
            stb.AppendLine("and   psa.Stato_Cod = 2005")
            stb.AppendLine("and   cte_ant_rich.Piva is null")
            If conto = 0 OrElse conto = -1 Then
                stb.AppendLine("and   trasf.Confermato_Tipo_Rich = " & Agro_SQL_SaveNum(conto) & " ")
            End If
            'If statoPrat <> "-1" Then
            '    stb.AppendLine(" and psa.Stato_Cod = " & Agro_SQL_SaveText(statoPrat) & " ")
            'End If

            '--------------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy) & " ")
            Else
                stb.AppendLine(" ORDER BY 1,3,2 ")
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

    Private Shared Sub AppendCreaTempAcquistato(anno As Integer,
                                                     stb As StringBuilder,
                                                     nomeTempVendite As String,
                                                     tipoCarb As Integer)
        UMA_Richieste_Testata_R.DropTempTableIfExists(stb, nomeTempVendite)
        stb.AppendLine("SELECT PIVA_Cliente As Piva, Anno, Conto_Proprio_Terzi, SUM(Lt) As Acquistato ")
        stb.AppendLine("INTO " & nomeTempVendite & " ")
        stb.AppendLine("FROM UMA_Vendite ")
        stb.AppendLine("WHERE Tipo_Carburante = " & tipoCarb.ToString & " ")
        stb.AppendLine("AND   Anno = " & anno.ToString & " ")
        stb.AppendLine("GROUP BY PIVA_Cliente, Anno, Conto_Proprio_Terzi; ")
        stb.AppendLine(" CREATE NONCLUSTERED INDEX Idx_1_TempVendite On " & nomeTempVendite & " (Piva, Anno, Conto_Proprio_Terzi); ")
    End Sub

    Public Function Leggi_ElencoTrasferimenti(ByVal anno As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Utenti As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Report_Controllo_R.Leggi_ElencoTrasferimenti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2)
        NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

        Try

            stb.Length = 0

            stb.AppendLine("select  ")
            stb.AppendLine(" ic.val_cod AS CUAA, ")
            stb.AppendLine(" i.rag_soc, ")
            stb.AppendLine(" CASE WHEN t.Confermato_Tipo_Rich = 0 THEN 'Conto Proprio' ELSE 'Conto Terzi' END As Tipo_Richiesta, ")
            stb.AppendLine(" t.Confermato_Gasolio, ")
            stb.AppendLine(" t.Confermato_Benzina, ")
            stb.AppendLine(" t.Confermato_Gasolio_Serra, ")
            stb.AppendLine(" t.Data_Trasferimento, ")
            stb.AppendLine(" i2.rag_soc as Azienda_Trasferente, ")
            stb.AppendLine(" CASE WHEN r.Avanzamento_Richiesta = 0 THEN 'Richiesta' ELSE 'Rendicontazione' END As Avanzamento_Richiesta, ")
            stb.AppendLine(" p.Anno, ")
            stb.AppendLine(" p.Numero ")
            stb.AppendLine(" from UMA_Richieste_Trasferimenti t ")
            stb.AppendLine(" inner join imprese i on i.piva = t.Piva_Ricevente ")
            stb.AppendLine(" inner join imprese_codici ic on ic.PIVA = i.PIVA ")
            stb.AppendLine(" inner join UMA_Richieste_Testata r on r.Piva_SuperUser = t.Piva_SuperUser  ")
            stb.AppendLine("                                   and r.Piva = t.Piva ")
            stb.AppendLine("                                   and r.Richiesta_Cod = t.Richiesta_Cod ")
            stb.AppendLine(" inner join imprese i2 on i2.PIVA = r.Piva ")
            stb.AppendLine(" inner join pratiche p on p.pratica_cod = r.Pratica_Cod ")
            stb.AppendLine(" inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = r.Pratica_Cod ")
            stb.AppendLine(" where year(t.data_trasferimento) = " & Agro_SQL_SaveNum(anno) & " ")
            stb.AppendLine(" and psa.Stato_Cod = 2005 ")
            stb.AppendLine(" and ic.id_cod = 1010 ")
            stb.AppendLine(" and 0 =  ")
            stb.AppendLine(" ( ")
            stb.AppendLine(" Select count(*)  ")
            stb.AppendLine(" from   UMA_Richieste_Testata  ")
            stb.AppendLine(" where  Avanzamento_Richiesta in (-1,0) ")
            stb.AppendLine(" and    Piva_SuperUser = r.Piva_SuperUser ")
            stb.AppendLine(" and    piva = t.Piva_Ricevente ")
            stb.AppendLine(" and    Tipo_Richiesta = t.Confermato_Tipo_Rich ")
            stb.AppendLine(" and    year(validita_inizio) =  " & Agro_SQL_SaveNum(anno) & "  ")
            stb.AppendLine(" ) ")

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy) & " ")
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

    Public Function Leggi_SegnalazioniAccise(anno As Integer,
                                             prov As String,
                                             com As String,
                                             tipoPrat As Integer,
                                             conto As Integer,
                                             giaSegnalate As Boolean,
                                             segnalateDal As String,
                                             rimanenze As Integer,
                                             xFiltroAggiuntivo As String,
                                             xOrderBy As String,
                                             ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Report_Controllo_R.Leggi_SegnalazioniAccise()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Dim primaUnion As Boolean = False

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2)
        NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

        Try

            stb.Length = 0

            If segnalateDal = "" Then
                segnalateDal = "01/01/1900"
            End If

            prov = Trim(prov)
            If prov.Length < 3 Then
                If prov.Length = 1 Then
                    prov = "00" & prov
                End If
                If prov.Length = 2 Then
                    prov = "0" & prov
                End If
            End If

            com = Trim(com)
            If com.Length < 3 Then
                If com.Length = 1 Then
                    com = "00" & com
                End If
                If com.Length = 2 Then
                    com = "0" & com
                End If
            End If

            UMA_Richieste_Testata_R.AppendCteRichieste(stb, anno, True, True, False)

            UMA_Richieste_Testata_R.AppendCteAnticipi(stb, anno, False, True)

            UMA_Richieste_Testata_R.AppendCteRichieste(stb, anno, False, True, True)

            AppendCteProvECom(stb, prov, com, False)

            UMA_Richieste_Testata_R.AppendCteAcquistato(stb, TipiEnumerativi.enum_TipoCarburante_UMA.Gasolio, False, anno)

            UMA_Richieste_Testata_R.AppendCteAcquistato(stb, TipiEnumerativi.enum_TipoCarburante_UMA.Benzina, False, anno)

            UMA_Richieste_Testata_R.AppendCteAcquistato(stb, TipiEnumerativi.enum_TipoCarburante_UMA.Gasolio_Serra, False, anno)

            UMA_Richieste_Testata_R.AppendCteAcquistatoXCarb(stb)

            UMA_Richieste_Testata_R.AppendCteAllevamenti(stb)

            UMA_Richieste_Testata_R.AppendCteTempLavParz(stb)

            UMA_Richieste_Testata_R.AppendWithCteTempLav(stb, False)

            stb.AppendLine(", cte_rendicontazioni as (")
            stb.AppendLine("select ")
            stb.AppendLine(" t.Piva")
            stb.AppendLine(",year(t.Validita_Inizio) Anno")
            stb.AppendLine(",t.Tipo_Richiesta")
            stb.AppendLine(",t.Rimanenza_Gasolio")
            stb.AppendLine(",t.Rimanenza_Benzina,")
            UMA_Richieste_Testata_R.AppendCarburanteAssegnato(stb)
            stb.AppendLine("t.Rimanenza_Gasolio_Serra")

            stb.AppendLine("from UMA_Richieste_Testata t")
            stb.AppendLine("inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = t.Pratica_Cod")
            stb.AppendLine("inner join Imprese i on i.PIVA = t.piva")

            UMA_Richieste_Testata_R.AppendJoinCteTemp(stb)

            stb.AppendLine("where year(t.Validita_Inizio) = " & Agro_SQL_SaveNum(anno) & "")
            stb.AppendLine("and t.Avanzamento_Richiesta = 1")
            stb.AppendLine("and psa.Stato_Cod = 2005 -- Verifica intermedia completata con Successo")

            stb.AppendLine(" ) ")

            'CASO 1 Richieste - Causale: Dichiarato in richiesta
            If rimanenze = 1 AndAlso (tipoPrat = 0 OrElse tipoPrat = 2) Then
                AppendCaso1(stb, anno, conto, giaSegnalate, segnalateDal, NomeDB_Utenti)
                primaUnion = True
            End If

            'CASO 2A Anticipi - Causale: Richiesta mancante
            If tipoPrat = -1 OrElse tipoPrat = 2 Then
                CheckPrimaUnion(stb, primaUnion)
                AppendCaso2A(stb, anno, conto, giaSegnalate, segnalateDal, NomeDB_Utenti)
            End If

            'CASO 2B Richieste - Causale: Rendicontazione mancante
            If tipoPrat = 0 OrElse tipoPrat = 2 Then
                CheckPrimaUnion(stb, primaUnion)
                AppendCaso2B(stb, anno, conto, giaSegnalate, segnalateDal, NomeDB_Utenti)
            End If

            'CASO 4 Rendicontazioni - Causale: Rimanenze senza pratiche
            If tipoPrat = 1 OrElse tipoPrat = 2 Then
                CheckPrimaUnion(stb, primaUnion)
                AppendCaso4(stb, anno, conto, giaSegnalate, segnalateDal, NomeDB_Utenti)
            End If

            'CASI 3A e 3B Rendicontazioni - Causale: Recupero accise senza/con gestione rimanenze
            If tipoPrat = 1 OrElse tipoPrat = 2 Then
                CheckPrimaUnion(stb, primaUnion)
                If rimanenze = 0 Then
                    AppendCaso3A(stb, anno, conto, giaSegnalate, segnalateDal, NomeDB_Utenti)
                Else
                    AppendCaso3B(stb, anno, conto, giaSegnalate, segnalateDal, NomeDB_Utenti)
                End If
            End If

            'CASO 5 - Causale: Trasferimenti ricevuti senza pratiche
            If tipoPrat = 1 OrElse tipoPrat = 2 Then
                CheckPrimaUnion(stb, primaUnion)
                AppendCaso5(stb, anno, conto, giaSegnalate, segnalateDal, NomeDB_Utenti)
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy) & " ")
            Else
                stb.AppendLine(" ORDER BY 2,3 ")
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

    Private Sub CheckPrimaUnion(stb As StringBuilder, ByRef primaUnion As Boolean)
        If primaUnion Then
            stb.AppendLine("UNION")
        Else
            primaUnion = True
        End If
    End Sub

    Private Sub AppendCaso1(stb As StringBuilder, anno As Integer, conto As Integer, giaSegnalate As Boolean, segnalateDal As String, NomeDB_Utenti As String)
        stb.AppendLine("select ")
        stb.AppendLine(" '1' Caso")
        stb.AppendLine(",'Richiesta' Pratica")
        stb.AppendLine(",p.Numero")
        stb.AppendLine(",year(t.Validita_Inizio) Anno")
        stb.AppendLine(",case t.Tipo_Richiesta when 0 then 'C/Proprio' when -1 then 'C/Terzi' end Tipo")
        stb.AppendLine(",t.Richiesta_Cod, t.Piva")
        stb.AppendLine(",i.rag_soc Azienda")
        stb.AppendLine(",cpt.CUAA")
        stb.AppendLine(",cpt.Prov")
        stb.AppendLine(",cpt.citta")
        stb.AppendLine(",cpt.via")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Gasolio,0) AcqGas")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Benzina,0) AcqBenz")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Gasolio_Serra,0) AcqSerra")
        stb.AppendLine(",t.Rimanenza_Gasolio RimGas")
        stb.AppendLine(",t.Rimanenza_Benzina RimBenz")
        stb.AppendLine(",t.Rimanenza_Gasolio_Serra RimSerra")
        stb.AppendLine(",0 RendGas")
        stb.AppendLine(",0 RendBenz")
        stb.AppendLine(",0 RendSerra")
        stb.AppendLine(",0 RimFinGas")
        stb.AppendLine(",0 RimFinBenz")
        stb.AppendLine(",0 RimFinSerra")
        stb.AppendLine("-- RECUPERO ACCISE: Recupero Accise Confermato")
        stb.AppendLine(", isnull(t.Rec_Acc_Conf_Gasolio, 0)       RecAccGas")
        stb.AppendLine(", isnull(t.Rec_Acc_Conf_Benzina, 0)       RecAccBenz")
        stb.AppendLine(", isnull(t.Rec_Acc_Conf_Gasolio_Serra, 0) RecAccSerra")
        stb.AppendLine("--")
        stb.AppendLine(",'6' Causale")
        stb.AppendLine(",'Dichiarato in richiesta' CausaleDescr")
        stb.AppendLine(", urra.Rec_Acc_Data as Data_Segnalazione")
        AppendUtenteSegnalazione(stb)
        stb.AppendLine("from UMA_Richieste_Testata t")
        stb.AppendLine("inner join Pratiche p on p.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join Imprese i on i.PIVA = t.piva")
        stb.AppendLine("inner join cte_Prov_Com cpt on cpt.Piva = i.Piva")
        'stb.AppendLine("inner join cte_rendicontazioni cte_rend on cte_rend.Piva = t.piva and cte_rend.tipo_Richiesta = t.Tipo_Richiesta and cte_rend.Anno = '" & anno.ToString & "' ")
        stb.AppendLine("left join cte_acquisti_x_carb cte_acq on cte_acq.PIVA = t.piva and cte_acq.Conto_Proprio_Terzi = t.Tipo_Richiesta")
        AppendJoinRecAcc(stb, anno, 0, 0)
        stb.AppendLine("left join " & NomeDB_Utenti & ".dbo.utenti_dettagli utentesegn ON utentesegn.UserName = urra.Rec_Acc_Utente")
        stb.AppendLine("where p.Anno = '" & Agro_SQL_SaveText(anno) & "' ")
        stb.AppendLine("and   t.Avanzamento_Richiesta = 0")
        AppendFiltroGiaSegnalate(stb, giaSegnalate, Agro_SQL_SaveText(segnalateDal))
        If conto < 2 Then
            stb.AppendLine("and t.Tipo_Richiesta = " & conto.ToString & "")
        End If
        stb.AppendLine("and   psa.Stato_Cod = 2005")
        stb.AppendLine("and  (isnull(t.Rec_Acc_Conf_Gasolio,0) + isnull(t.Rec_Acc_Conf_Benzina,0) + isnull(t.Rec_Acc_Conf_Gasolio_Serra,0)) > 0")
    End Sub

    Private Shared Sub AppendJoinRecAcc(stb As StringBuilder, anno As Integer, docManc As Integer, trasferimento As Integer)

        Dim joinRecAcc = String.Format("left join UMA_Richieste_Rec_Acc urra on urra.Richiesta_Cod = t.richiesta_Cod and urra.Piva = {0} and urra.Rec_Acc_Anno = {1} and urra.Doc_Manc = {2} ",
                                       If(trasferimento = 1, "trasf.Piva_Ricevente", "t.Piva"),
                                       anno,
                                       docManc)

        stb.AppendLine(joinRecAcc)

    End Sub

    Private Shared Sub AppendFiltroGiaSegnalate(stb As StringBuilder, giaSegnalate As Boolean, segnalateDal As String)
        If giaSegnalate Then
            stb.AppendLine(" and urra.Rec_Acc_Data >= '" & segnalateDal.ToString & "' ")
        Else
            stb.AppendLine(" and urra.Rec_Acc_Data IS NULL")
        End If
    End Sub

    Private Shared Sub AppendUtenteSegnalazione(stb As StringBuilder)
        stb.AppendLine(", CASE WHEN utentesegn.Nome = '' AND utentesegn.cognome = '' ")
        stb.AppendLine("  THEN (urra.Rec_Acc_Utente +' [' + utentesegn.Rag_Soc + ']') ")
        stb.AppendLine("  ELSE (urra.Rec_Acc_Utente +' [' + utentesegn.Nome + ' ' + utentesegn.cognome +']') ")
        stb.AppendLine("  END AS Utente_Segnalazione ")
    End Sub

    Private Sub AppendCaso2A(stb As StringBuilder, anno As Integer, conto As Integer, giaSegnalate As Boolean, segnalateDal As String, NomeDB_Utenti As String)
        stb.AppendLine("select ")
        stb.AppendLine(" '2A' Caso")
        stb.AppendLine(",'Anticipo' Pratica")
        stb.AppendLine(",p.Numero")
        stb.AppendLine(",year(t.Validita_Inizio) Anno")
        stb.AppendLine(",case t.Tipo_Richiesta when 0 then 'C/Proprio' when -1 then 'C/Terzi' end Tipo")
        stb.AppendLine(",t.Richiesta_Cod, t.Piva")
        stb.AppendLine(",i.rag_soc Azienda")
        stb.AppendLine(",cpt.CUAA")
        stb.AppendLine(",cpt.Prov")
        stb.AppendLine(",cpt.citta")
        stb.AppendLine(",cpt.via")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Gasolio,0) AcqGas")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Benzina,0) AcqBenz")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Gasolio_Serra,0) AcqSerra")
        stb.AppendLine(",isnull(cte_rend_prec.Rimanenza_Gasolio,0) RimGas")
        stb.AppendLine(",isnull(cte_rend_prec.Rimanenza_Benzina,0) RimBenz")
        stb.AppendLine(",isnull(cte_rend_prec.Rimanenza_Gasolio_Serra,0) RimSerra")
        stb.AppendLine(",0 RendGas")
        stb.AppendLine(",0 RendBenz")
        stb.AppendLine(",0 RendSerra")
        stb.AppendLine(",0 RimFinGas")
        stb.AppendLine(",0 RimFinBenz")
        stb.AppendLine(",0 RimFinSerra")
        stb.AppendLine("-- RECUPERO ACCISE: Acquistato +Rimanenze Finali")
        stb.AppendLine(", isnull(cte_acq.Acquistato_Gasolio, 0) + isnull(cte_rend_prec.Rimanenza_Gasolio, 0) As RecAccGas")
        stb.AppendLine(", isnull(cte_acq.Acquistato_Benzina, 0) + isnull(cte_rend_prec.Rimanenza_Benzina, 0) As RecAccBenz")
        stb.AppendLine(", isnull(cte_acq.Acquistato_Gasolio_Serra, 0) + isnull(cte_rend_prec.Rimanenza_Gasolio_Serra, 0) As RecAccSerra")
        stb.AppendLine("--")
        stb.AppendLine(",'7' Causale")
        stb.AppendLine(",'Richiesta mancante' CausaleDescr")
        stb.AppendLine(", urra.Rec_Acc_Data as Data_Segnalazione")
        AppendUtenteSegnalazione(stb)
        stb.AppendLine("from UMA_Richieste_Testata t")
        stb.AppendLine("inner join Pratiche p on p.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join Imprese i on i.PIVA = t.piva")
        stb.AppendLine("inner join cte_Prov_Com cpt on cpt.Piva = i.Piva")
        'stb.AppendLine("left join cte_richieste cte_rich on cte_rich.Piva = t.piva and")
        'stb.AppendLine("                                    cte_rich.Anno = " & Agro_SQL_SaveNum(anno) & " and")
        'stb.AppendLine("                                    cte_rich.Tipo_Richiesta = t.Tipo_Richiesta")
        stb.AppendLine("left join cte_acquisti_x_carb cte_acq on cte_acq.PIVA = t.piva and cte_acq.Conto_Proprio_Terzi = t.Tipo_Richiesta")
        stb.AppendLine("left join cte_rendicontazioni cte_rend_prec on cte_rend_prec.Piva = t.Piva and")
        stb.AppendLine("                                               cte_rend_prec.Anno = " & Agro_SQL_SaveNum(anno) & " and")
        stb.AppendLine("                                               cte_rend_prec.Tipo_Richiesta = t.Tipo_Richiesta")
        AppendJoinRecAcc(stb, anno, 1, 0)
        stb.AppendLine("left join " & NomeDB_Utenti & ".dbo.utenti_dettagli utentesegn ON utentesegn.UserName = urra.Rec_Acc_Utente")
        stb.AppendLine("where p.Anno = '" & Agro_SQL_SaveText(anno) & "' ")
        stb.AppendLine("and   t.Avanzamento_Richiesta = -1")
        AppendFiltroGiaSegnalate(stb, giaSegnalate, Agro_SQL_SaveText(segnalateDal))
        If conto < 2 Then
            stb.AppendLine("and t.Tipo_Richiesta = " & conto.ToString & "")
        End If
        stb.AppendLine("and   psa.Stato_Cod = 2005")
        'stb.AppendLine("and   isnull(cte_rich.piva,'') = '' ")
        stb.AppendLine("and   0 = (select count(*) from cte_richieste cte_rich ")
        stb.AppendLine("           where cte_rich.Piva = t.piva ")
        stb.AppendLine("           and   cte_rich.Anno = " & Agro_SQL_SaveNum(anno))
        stb.AppendLine("           and   cte_rich.Tipo_Richiesta = t.Tipo_Richiesta)")
        stb.AppendLine("and ( isnull(cte_acq.Acquistato_Gasolio,0) + isnull(cte_rend_prec.Rimanenza_Gasolio,0) > 0 or ")
        stb.AppendLine("      isnull(cte_acq.Acquistato_Benzina,0) + isnull(cte_rend_prec.Rimanenza_Benzina,0) > 0 or ")
        stb.AppendLine("      isnull(cte_acq.Acquistato_Gasolio_Serra,0) + isnull(cte_rend_prec.Rimanenza_Gasolio_Serra,0) > 0 ")
        stb.AppendLine("    ) ")
    End Sub

    Private Sub AppendCaso2B(stb As StringBuilder, anno As Integer, conto As Integer, giaSegnalate As Boolean, segnalateDal As String, NomeDB_Utenti As String)
        stb.AppendLine("select ")
        stb.AppendLine(" '2B' Caso")
        stb.AppendLine(",'Richiesta' Pratica")
        stb.AppendLine(",p.Numero")
        stb.AppendLine(",year(t.Validita_Inizio) Anno")
        stb.AppendLine(",case t.Tipo_Richiesta when 0 then 'C/Proprio' when -1 then 'C/Terzi' end Tipo")
        stb.AppendLine(",t.Richiesta_Cod, t.Piva")
        stb.AppendLine(",i.rag_soc Azienda")
        stb.AppendLine(",cpt.CUAA")
        stb.AppendLine(",cpt.Prov")
        stb.AppendLine(",cpt.citta")
        stb.AppendLine(",cpt.via")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Gasolio,0) AcqGas")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Benzina,0) AcqBenz")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Gasolio_Serra,0) AcqSerra")
        stb.AppendLine(",t.Rimanenza_Gasolio RimGas")
        stb.AppendLine(",t.Rimanenza_Benzina ")
        stb.AppendLine(",t.Rimanenza_Gasolio_Serra RimSerra")
        stb.AppendLine(",0 RendGas")
        stb.AppendLine(",0 RendBenz")
        stb.AppendLine(",0 RendSerra")
        stb.AppendLine(",0 RimFinGas")
        stb.AppendLine(",0 RimFinBenz")
        stb.AppendLine(",0 RimFinSerra")
        stb.AppendLine("-- RECUPERO ACCISE: Acquistato +Rimanenze Finali")
        stb.AppendLine(", isnull(cte_acq.Acquistato_Gasolio, 0) + t.Rimanenza_Gasolio As RecAccGas")
        stb.AppendLine(", isnull(cte_acq.Acquistato_Benzina, 0) + t.Rimanenza_Benzina As RecAccBenz")
        stb.AppendLine(", isnull(cte_acq.Acquistato_Gasolio_Serra, 0) + t.Rimanenza_Gasolio_Serra As RecAccSerra")
        stb.AppendLine("--")
        stb.AppendLine(",'4' Causale")
        stb.AppendLine(",'Rendicontazione mancante' CausaleDescr")
        stb.AppendLine(", urra.Rec_Acc_Data as Data_Segnalazione")
        AppendUtenteSegnalazione(stb)
        stb.AppendLine("from UMA_Richieste_Testata t")
        stb.AppendLine("inner join Pratiche p on p.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join Imprese i on i.PIVA = t.piva")
        stb.AppendLine("inner join cte_Prov_Com cpt on cpt.Piva = i.Piva")
        stb.AppendLine("left join cte_rendicontazioni cte_rend on cte_rend.Piva = t.Piva and")
        stb.AppendLine("                                          cte_rend.Anno = " & Agro_SQL_SaveNum(anno) & " and")
        stb.AppendLine("                                          cte_rend.Tipo_Richiesta = t.Tipo_Richiesta")
        stb.AppendLine("left join cte_acquisti_x_carb cte_acq on cte_acq.PIVA = t.piva and cte_acq.Conto_Proprio_Terzi = t.Tipo_Richiesta")
        AppendJoinRecAcc(stb, anno, 1, 0)
        stb.AppendLine("left join " & NomeDB_Utenti & ".dbo.utenti_dettagli utentesegn ON utentesegn.UserName = urra.Rec_Acc_Utente")
        stb.AppendLine("where p.Anno = '" & Agro_SQL_SaveText(anno) & "' ")
        stb.AppendLine("and   t.Avanzamento_Richiesta = 0")
        AppendFiltroGiaSegnalate(stb, giaSegnalate, Agro_SQL_SaveText(segnalateDal))
        If conto < 2 Then
            stb.AppendLine("and t.Tipo_Richiesta = " & conto.ToString & "")
        End If
        stb.AppendLine("and   t.Richiesta_Integrativa = 0")
        stb.AppendLine("and   psa.Stato_Cod = 2005")
        stb.AppendLine("and   isnull(cte_rend.piva,'') = '' ")
        stb.AppendLine("and ( isnull(cte_acq.Acquistato_Gasolio,0) + t.Rimanenza_Gasolio > 0 or ")
        stb.AppendLine("      isnull(cte_acq.Acquistato_Benzina,0) +  t.Rimanenza_Benzina > 0 or ")
        stb.AppendLine("      isnull(cte_acq.Acquistato_Gasolio_Serra,0) + t.Rimanenza_Gasolio_Serra > 0 ")
        stb.AppendLine("    ) ")
    End Sub

    Private Sub AppendCaso4(stb As StringBuilder, anno As Integer, conto As Integer, giaSegnalate As Boolean, segnalateDal As String, NomeDB_Utenti As String)
        stb.AppendLine("select ")
        stb.AppendLine(" '4' Caso")
        stb.AppendLine(",'Rendicontazione' Pratica")
        stb.AppendLine(",p.Numero")
        stb.AppendLine(",year(t.Validita_Inizio) Anno")
        stb.AppendLine(",case t.Tipo_Richiesta when 0 then 'C/Proprio' when -1 then 'C/Terzi' end Tipo")
        stb.AppendLine(",t.Richiesta_Cod, t.Piva")
        stb.AppendLine(",i.rag_soc Azienda")
        stb.AppendLine(",cpt.CUAA")
        stb.AppendLine(",cpt.Prov")
        stb.AppendLine(",cpt.citta")
        stb.AppendLine(",cpt.via")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Gasolio,0) AcqGas")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Benzina,0) AcqBenz")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Gasolio_Serra,0) AcqSerra")
        stb.AppendLine(",0 RimGas")
        stb.AppendLine(",0 RimBenz")
        stb.AppendLine(",0 RimSerra")
        stb.AppendLine(",0 RendGas")
        stb.AppendLine(",0 RendBenz")
        stb.AppendLine(",0 RendSerra")
        stb.AppendLine(",t.Rimanenza_Gasolio RimFinGas")
        stb.AppendLine(",t.Rimanenza_Benzina RimFinBenz")
        stb.AppendLine(",t.Rimanenza_Gasolio_Serra RimFinSerra")
        stb.AppendLine("-- RECUPERO ACCISE: Rimanenze Finali")
        stb.AppendLine(", t.Rimanenza_Gasolio As RecAccGas")
        stb.AppendLine(", t.Rimanenza_Benzina As RecAccBenz")
        stb.AppendLine(", t.Rimanenza_Gasolio_Serra As RecAccSerra")
        stb.AppendLine("--")
        stb.AppendLine(", '5' Causale")
        stb.AppendLine(", 'Rimanenze senza pratiche' CausaleDescr")
        stb.AppendLine(", urra.Rec_Acc_Data as Data_Segnalazione")
        AppendUtenteSegnalazione(stb)
        stb.AppendLine("from UMA_Richieste_Testata t")
        stb.AppendLine("inner join Pratiche p on p.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join Imprese i on i.PIVA = t.piva")
        stb.AppendLine("inner join cte_Prov_Com cpt on cpt.Piva = i.Piva")
        stb.AppendLine("left join cte_anticipi_richieste cte_ant_rich on cte_ant_rich.Piva = t.piva and")
        stb.AppendLine("                                                 cte_ant_rich.Anno = " & Agro_SQL_SaveNum(anno) & " and")
        stb.AppendLine("                                                 cte_ant_rich.Tipo_Richiesta = t.Tipo_Richiesta")
        stb.AppendLine("left join cte_acquisti_x_carb cte_acq on cte_acq.PIVA = t.piva and cte_acq.Conto_Proprio_Terzi = t.Tipo_Richiesta")
        AppendJoinRecAcc(stb, anno, 0, 0)
        stb.AppendLine("left join " & NomeDB_Utenti & ".dbo.utenti_dettagli utentesegn ON utentesegn.UserName = urra.Rec_Acc_Utente")
        stb.AppendLine("where p.Anno = '" & Agro_SQL_SaveText(anno - 1) & "' ")
        stb.AppendLine("and   t.Avanzamento_Richiesta = 1")
        AppendFiltroGiaSegnalate(stb, giaSegnalate, Agro_SQL_SaveText(segnalateDal))
        If conto < 2 Then
            stb.AppendLine("and t.Tipo_Richiesta = " & conto.ToString & "")
        End If
        stb.AppendLine("and   psa.Stato_Cod = 2005")
        stb.AppendLine("and   (t.Rimanenza_Benzina + t.Rimanenza_Gasolio + t.Rimanenza_Gasolio_Serra) > 0")
        stb.AppendLine("and   cte_ant_rich.Piva is null")
    End Sub

    Private Sub AppendCaso5(stb As StringBuilder, anno As Integer, conto As Integer, giaSegnalate As Boolean, segnalateDal As String, NomeDB_Utenti As String)
        stb.AppendLine("select ")
        stb.AppendLine(" '5' Caso")
        stb.AppendLine(",'' Pratica")
        stb.AppendLine(",NULL Numero")
        stb.AppendLine("," & anno.ToString() & " Anno")
        stb.AppendLine(",case t.Tipo_Richiesta when 0 then 'C/Proprio' when -1 then 'C/Terzi' end Tipo")
        stb.AppendLine(",t.Richiesta_Cod, trasf.Piva_Ricevente")
        stb.AppendLine(",i.rag_soc Azienda")
        stb.AppendLine(",cpt.CUAA")
        stb.AppendLine(",cpt.Prov")
        stb.AppendLine(",cpt.citta")
        stb.AppendLine(",cpt.via")
        stb.AppendLine(",trasf.Confermato_Gasolio AcqGas")
        stb.AppendLine(",trasf.Confermato_Benzina AcqBenz")
        stb.AppendLine(",trasf.Confermato_Gasolio_Serra AcqSerra")
        stb.AppendLine(",0 RimGas")
        stb.AppendLine(",0 RimBenz")
        stb.AppendLine(",0 RimSerra")
        stb.AppendLine(",0 RendGas")
        stb.AppendLine(",0 RendBenz")
        stb.AppendLine(",0 RendSerra")
        stb.AppendLine(",0 RimFinGas")
        stb.AppendLine(",0 RimFinBenz")
        stb.AppendLine(",0 RimFinSerra")
        stb.AppendLine("-- RECUPERO ACCISE: Trasferimenti")
        stb.AppendLine(", trasf.Confermato_Gasolio As RecAccGas")
        stb.AppendLine(", trasf.Confermato_Benzina As RecAccBenz")
        stb.AppendLine(", trasf.Confermato_Gasolio_Serra As RecAccSerra")
        stb.AppendLine("--")
        stb.AppendLine(", '9' Causale")
        stb.AppendLine(", 'Trasferimenti ricevuti senza pratiche' CausaleDescr")
        stb.AppendLine(", urra.Rec_Acc_Data as Data_Segnalazione")
        AppendUtenteSegnalazione(stb)
        stb.AppendLine("from UMA_Richieste_Testata t")
        stb.AppendLine("inner join Pratiche p on p.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join UMA_Richieste_Trasferimenti trasf on trasf.Piva_SuperUser = t.Piva_SuperUser ")
        stb.AppendLine("                                            and trasf.Piva = t.Piva ")
        stb.AppendLine("                                            and trasf.Richiesta_Cod = t.Richiesta_Cod ")
        stb.AppendLine("inner join Imprese i on i.PIVA =  trasf.Piva_Ricevente")
        stb.AppendLine("inner join cte_Prov_Com cpt on cpt.Piva =  trasf.Piva_Ricevente")
        stb.AppendLine("left join cte_anticipi_richieste cte_ant_rich on cte_ant_rich.Piva =  trasf.Piva_Ricevente and ")
        stb.AppendLine("                                                 cte_ant_rich.Anno = " & Agro_SQL_SaveNum(anno) & " and ")
        stb.AppendLine("                                                 cte_ant_rich.Tipo_Richiesta = trasf.Confermato_Tipo_Rich ")
        ' In questo caso UMA_Richieste_Rec_Acc è agganciato alla pratica che trasferisce, ma con la PIVA del ricevente
        AppendJoinRecAcc(stb, anno, 0, 1)
        '---
        stb.AppendLine("left join " & NomeDB_Utenti & ".dbo.utenti_dettagli utentesegn ON utentesegn.UserName = urra.Rec_Acc_Utente")
        stb.AppendLine("where year(trasf.Data_Trasferimento) = " & Agro_SQL_SaveNum(anno) & " ")
        AppendFiltroGiaSegnalate(stb, giaSegnalate, Agro_SQL_SaveText(segnalateDal))
        If conto < 2 Then
            stb.AppendLine("and trasf.Confermato_Tipo_Rich = " & conto.ToString & "")
        End If
        stb.AppendLine("and psa.Stato_Cod = 2005")
        stb.AppendLine("and cte_ant_rich.Piva is null")
    End Sub

    Private Sub AppendCaso3A(stb As StringBuilder, anno As Integer, conto As Integer, giaSegnalate As Boolean, segnalateDal As String, NomeDB_Utenti As String)
        stb.AppendLine("select ")
        stb.AppendLine(" '3A' Caso")
        stb.AppendLine(",'Rendicontazione' Pratica")
        stb.AppendLine(",p.Numero")
        stb.AppendLine(",year(t.Validita_Inizio) Anno")
        stb.AppendLine(",case t.Tipo_Richiesta when 0 then 'C/Proprio' when -1 then 'C/Terzi' end Tipo")
        stb.AppendLine(",t.Richiesta_Cod, t.Piva")
        stb.AppendLine(",i.rag_soc Azienda")
        stb.AppendLine(",cpt.CUAA")
        stb.AppendLine(",cpt.Prov")
        stb.AppendLine(",cpt.citta")
        stb.AppendLine(",cpt.via")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Gasolio,0) AcqGas")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Benzina,0) AcqBenz")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Gasolio_Serra,0) AcqSerra")
        stb.AppendLine(",isnull(cte_prime_rich.Rimanenza_Gasolio,0) RimGas")
        stb.AppendLine(",isnull(cte_prime_rich.Rimanenza_Benzina,0) RimBenz")
        stb.AppendLine(",isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) RimSerra")
        stb.AppendLine(",isnull(cte_rend.assegnato_gasolio,0) RendGas")
        stb.AppendLine(",isnull(cte_rend.assegnato_benzina,0) RendBenz")
        stb.AppendLine(",isnull(cte_rend.assegnato_gasolio_serra,0) RendSerra")
        stb.AppendLine(",t.Rimanenza_Gasolio RimFinGas")
        stb.AppendLine(",t.Rimanenza_Benzina RimFinBenz")
        stb.AppendLine(",t.Rimanenza_Gasolio_Serra RimFinSerra")
        'RECUPERO ACCISE: RimanenzeIniziali + Acquistato - Assegnato - RimanenzeFinali
        stb.AppendLine(", CASE WHEN isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_acq.Acquistato_Gasolio,0) - ")
        stb.AppendLine(" isnull(cte_rend.assegnato_gasolio,0) - t.Rimanenza_Gasolio > 0 THEN ")
        stb.AppendLine(" isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_acq.Acquistato_Gasolio,0) - ")
        stb.AppendLine(" isnull(cte_rend.assegnato_gasolio,0) - t.Rimanenza_Gasolio ELSE 0 END As RecAccGas")
        stb.AppendLine(", CASE WHEN isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_acq.Acquistato_Benzina,0) - ")
        stb.AppendLine(" isnull(cte_rend.assegnato_benzina,0) - t.Rimanenza_Benzina > 0 THEN")
        stb.AppendLine(" isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_acq.Acquistato_Benzina,0) - ")
        stb.AppendLine(" isnull(cte_rend.assegnato_benzina,0) - t.Rimanenza_Benzina ELSE 0 END As RecAccBenz")
        stb.AppendLine(", CASE WHEN isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) + isnull(cte_acq.Acquistato_Gasolio_Serra,0) - ")
        stb.AppendLine(" isnull(cte_rend.assegnato_gasolio_serra,0) - t.Rimanenza_Gasolio_Serra > 0 THEN")
        stb.AppendLine(" isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) + isnull(cte_acq.Acquistato_Gasolio_Serra,0) - ")
        stb.AppendLine(" isnull(cte_rend.assegnato_gasolio_serra,0) - t.Rimanenza_Gasolio_Serra ELSE 0 END As RecAccSerra")
        '----------------------------------------------------------------------------------------------------
        'CAUSALE RECUPERO ACCISE
        '----------------------------------------------------------------------------------------------------
        stb.AppendLine(",case")
        '--- RIMANENZE NON RIASSEGNATE
        '                            Rimanenze Iniziali Presenti
        stb.AppendLine("when ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) > 0 ")
        '                            Assegnato < Rimanenze Iniziali
        stb.AppendLine(" And ( isnull(cte_rend.assegnato_gasolio,0) + isnull(cte_rend.assegnato_benzina,0) + isnull(cte_rend.assegnato_gasolio_serra,0) ) < ")
        stb.AppendLine("     ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) ")
        '                            Acquistato + Rimanenze Iniziali - Assegnato <= Rimanenze Iniziali
        stb.AppendLine(" And ( ( isnull(cte_acq.Acquistato_Gasolio, 0) + isnull(cte_acq.Acquistato_Benzina, 0) + isnull(cte_acq.Acquistato_Gasolio_Serra, 0) ) +")
        stb.AppendLine("       ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) -")
        stb.AppendLine("	     ( isnull(cte_rend.assegnato_gasolio,0) + isnull(cte_rend.assegnato_benzina,0) + isnull(cte_rend.assegnato_gasolio_serra,0) )")
        stb.AppendLine("     ) <= ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) )")
        stb.AppendLine("then 2 ")
        '--- RIMANENZE NON RIASSEGNATE E LITRI IN ESUBERO
        '                            Rimanenze Iniziali Presenti
        stb.AppendLine("when ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0)) > 0 ")
        '                            Assegnato < Rimanenze Iniziali
        stb.AppendLine(" And ( isnull(cte_rend.assegnato_gasolio,0) + isnull(cte_rend.assegnato_benzina,0) +  isnull(cte_rend.assegnato_gasolio_serra,0) ) < ")
        stb.AppendLine("     ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) )")
        '                            Acquistato + Rimanenze Iniziali - Assegnato > Rimanenze Iniziali
        stb.AppendLine(" And ( ( isnull(cte_acq.Acquistato_Gasolio, 0) + isnull(cte_acq.Acquistato_Benzina, 0) + isnull(cte_acq.Acquistato_Gasolio_Serra, 0)) + ")
        stb.AppendLine("       ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) - ")
        stb.AppendLine("	     ( isnull(cte_rend.assegnato_gasolio,0) + isnull(cte_rend.assegnato_benzina,0) + isnull(cte_rend.assegnato_gasolio_serra,0) ) ")
        stb.AppendLine("     ) > (isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0))")
        stb.AppendLine("then 3 ")
        '--- LITRI IN ESUBERO
        stb.AppendLine("else 1 ")
        '---
        stb.AppendLine("end Causale")
        '----------------------------------------------------------------------------------------------------
        'DESCRIZIONE CAUSALE RECUPERO ACCISE
        '----------------------------------------------------------------------------------------------------
        stb.AppendLine(",case")
        '--- RIMANENZE NON RIASSEGNATE
        '                            Rimanenze Iniziali Presenti
        stb.AppendLine("when ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) > 0 ")
        '                            Assegnato < Rimanenze Iniziali
        stb.AppendLine(" And ( isnull(cte_rend.assegnato_gasolio,0) + isnull(cte_rend.assegnato_benzina,0) + isnull(cte_rend.assegnato_gasolio_serra,0) ) < ")
        stb.AppendLine("     ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) ")
        '                            Acquistato + Rimanenze Iniziali - Assegnato <= Rimanenze Iniziali
        stb.AppendLine(" And ( ( isnull(cte_acq.Acquistato_Gasolio, 0) + isnull(cte_acq.Acquistato_Benzina, 0) + isnull(cte_acq.Acquistato_Gasolio_Serra, 0) ) +")
        stb.AppendLine("       ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) -")
        stb.AppendLine("	     ( isnull(cte_rend.assegnato_gasolio,0) + isnull(cte_rend.assegnato_benzina,0) + isnull(cte_rend.assegnato_gasolio_serra,0) )")
        stb.AppendLine("     ) <= ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) )")
        stb.AppendLine("then 'Rimanenze non riassegnate'")
        '--- RIMANENZE NON RIASSEGNATE E LITRI IN ESUBERO
        '                            Rimanenze Iniziali Presenti
        stb.AppendLine("when ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0)) > 0 ")
        '                            Assegnato < Rimanenze Iniziali
        stb.AppendLine(" And ( isnull(cte_rend.assegnato_gasolio,0) + isnull(cte_rend.assegnato_benzina,0) +  isnull(cte_rend.assegnato_gasolio_serra,0) ) < ")
        stb.AppendLine("     ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) )")
        '                            Acquistato + Rimanenze Iniziali - Assegnato > Rimanenze Iniziali
        stb.AppendLine(" And ( ( isnull(cte_acq.Acquistato_Gasolio, 0) + isnull(cte_acq.Acquistato_Benzina, 0) + isnull(cte_acq.Acquistato_Gasolio_Serra, 0)) + ")
        stb.AppendLine("       ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) - ")
        stb.AppendLine("	     ( isnull(cte_rend.assegnato_gasolio,0) + isnull(cte_rend.assegnato_benzina,0) + isnull(cte_rend.assegnato_gasolio_serra,0) ) ")
        stb.AppendLine("     ) > (isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0))")
        stb.AppendLine("then 'Rimanenze non riassegnate + Litri in esubero' ")
        '--- LITRI IN ESUBERO
        stb.AppendLine("else 'Litri in esubero' ")
        '---
        stb.AppendLine("end CausaleDescr")
        '----------------------------------------------------------------------------------------------------
        stb.AppendLine(", urra.Rec_Acc_Data as Data_Segnalazione")
        AppendUtenteSegnalazione(stb)
        stb.AppendLine("from UMA_Richieste_Testata t")
        stb.AppendLine("inner join Pratiche p on p.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join Imprese i on i.PIVA = t.piva")
        stb.AppendLine("inner join cte_Prov_Com cpt on cpt.Piva = i.Piva")
        stb.AppendLine("left join cte_prime_richieste cte_prime_rich  on cte_prime_rich.Piva = t.piva and")
        stb.AppendLine("                                                 cte_prime_rich.Anno = " & Agro_SQL_SaveNum(anno) & " and")
        stb.AppendLine("                                                 cte_prime_rich.Tipo_Richiesta = t.Tipo_Richiesta")
        stb.AppendLine("left join cte_rendicontazioni cte_rend on cte_rend.Piva = t.Piva and")
        stb.AppendLine("                                          cte_rend.Anno = " & Agro_SQL_SaveNum(anno) & " and")
        stb.AppendLine("                                          cte_rend.Tipo_Richiesta = t.Tipo_Richiesta")
        stb.AppendLine("left join cte_acquisti_x_carb cte_acq on cte_acq.PIVA = t.piva and cte_acq.Conto_Proprio_Terzi = t.Tipo_Richiesta")
        AppendJoinRecAcc(stb, anno, 0, 0)
        stb.AppendLine("left join " & NomeDB_Utenti & ".dbo.utenti_dettagli utentesegn ON utentesegn.UserName = urra.Rec_Acc_Utente")
        stb.AppendLine("where p.Anno = '" & Agro_SQL_SaveText(anno) & "' ")
        stb.AppendLine("and   t.Avanzamento_Richiesta = 1")
        AppendFiltroGiaSegnalate(stb, giaSegnalate, Agro_SQL_SaveText(segnalateDal))
        If conto < 2 Then
            stb.AppendLine("and t.Tipo_Richiesta = " & conto.ToString & "")
        End If
        stb.AppendLine("and   psa.Stato_Cod = 2005")
        stb.AppendLine("and  ")
        'stb.AppendLine("(")
        'stb.AppendLine(" ( isnull(cte_acq.Acquistato_Gasolio,0) + isnull(cte_acq.Acquistato_Benzina,0) + isnull(cte_acq.Acquistato_Gasolio_Serra,0) ) +")
        'stb.AppendLine(" ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) )")
        'stb.AppendLine(") > ")
        'stb.AppendLine("(")
        'stb.AppendLine(" ( isnull(cte_rend.assegnato_gasolio,0) + isnull(cte_rend.assegnato_benzina,0) + isnull(cte_rend.assegnato_gasolio_serra,0) ) +")
        'stb.AppendLine(" ( t.Rimanenza_Gasolio + t.Rimanenza_Benzina + t.Rimanenza_Gasolio_Serra )")
        'stb.AppendLine(")")
        stb.AppendLine("(")
        stb.AppendLine(" ( isnull(cte_acq.Acquistato_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Gasolio,0) > isnull(cte_rend.assegnato_gasolio,0) + t.Rimanenza_Gasolio ) or ")
        stb.AppendLine(" ( isnull(cte_acq.Acquistato_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) > isnull(cte_rend.assegnato_benzina,0) + t.Rimanenza_Benzina ) or ")
        stb.AppendLine(" ( isnull(cte_acq.Acquistato_Gasolio_Serra,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) > isnull(cte_rend.assegnato_gasolio_serra,0) + t.Rimanenza_Gasolio_Serra ) ")
        stb.AppendLine(")")

    End Sub

    Private Sub AppendCaso3B(stb As StringBuilder, anno As Integer, conto As Integer, giaSegnalate As Boolean, segnalateDal As String, NomeDB_Utenti As String)
        stb.AppendLine("select ")
        stb.AppendLine(" '3B' Caso")
        stb.AppendLine(",'Rendicontazione' Pratica")
        stb.AppendLine(",p.Numero")
        stb.AppendLine(",year(t.Validita_Inizio) Anno")
        stb.AppendLine(",case t.Tipo_Richiesta when 0 then 'C/Proprio' when -1 then 'C/Terzi' end Tipo")
        stb.AppendLine(",t.Richiesta_Cod, t.Piva")
        stb.AppendLine(",i.rag_soc Azienda")
        stb.AppendLine(",cpt.CUAA")
        stb.AppendLine(",cpt.Prov")
        stb.AppendLine(",cpt.citta")
        stb.AppendLine(",cpt.via")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Gasolio,0) AcqGas")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Benzina,0) AcqBenz")
        stb.AppendLine(",isnull(cte_acq.Acquistato_Gasolio_Serra,0) AcqSerra")
        stb.AppendLine(",isnull(cte_prime_rich.Rimanenza_Gasolio,0) RimGas")
        stb.AppendLine(",isnull(cte_prime_rich.Rimanenza_Benzina,0) RimBenz")
        stb.AppendLine(",isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) RimSerra")
        stb.AppendLine(",isnull(cte_rend.assegnato_gasolio,0) RendGas")
        stb.AppendLine(",isnull(cte_rend.assegnato_benzina,0) RendBenz")
        stb.AppendLine(",isnull(cte_rend.assegnato_gasolio_serra,0) RendSerra")
        stb.AppendLine(",t.Rimanenza_Gasolio RimFinGas")
        stb.AppendLine(",t.Rimanenza_Benzina RimFinBenz")
        stb.AppendLine(",t.Rimanenza_Gasolio_Serra RimFinSerra")
        ' RECUPERO ACCISE: Recupero Accise Confermato
        stb.AppendLine(", isnull(t.Rec_Acc_Conf_Gasolio, 0) As RecAccGas")
        stb.AppendLine(", isnull(t.Rec_Acc_Conf_Benzina, 0) As RecAccBenz")
        stb.AppendLine(", isnull(t.Rec_Acc_Conf_Gasolio_Serra, 0) As RecAccSerra")
        '----------------------------------------------------------------------------------------------------
        'CAUSALE RECUPERO ACCISE
        '----------------------------------------------------------------------------------------------------
        'stb.AppendLine(",case")
        ''--- RIMANENZE NON RIASSEGNATE
        ''                            Rimanenze Iniziali Presenti
        'stb.AppendLine("when ( isnull(cte_prime_rich.Rimanenza_Gasolio, 0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) > 0 ")
        ''                            Assegnato < Rimanenze Iniziali
        'stb.AppendLine(" and ( isnull(cte_rend.assegnato_gasolio,0) + isnull(cte_rend.assegnato_benzina,0) + isnull(cte_rend.assegnato_gasolio_serra,0) ) < ")
        'stb.AppendLine("     ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) ")
        ''                            Recupero Accise Confermato <= Rimanenze Iniziali
        'stb.AppendLine(" and ( isnull(t.Rec_Acc_Conf_Gasolio,0) + isnull(t.Rec_Acc_Conf_Benzina,0) + isnull(t.Rec_Acc_Conf_Gasolio_Serra,0) ) <= ")
        'stb.AppendLine("     ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) ")
        'stb.AppendLine("then 2 ")
        ''--- RIMANENZE NON RIASSEGNATE E LITRI IN ESUBERO
        ''                            Rimanenze Iniziali Presenti
        'stb.AppendLine("when ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) > 0 ")
        ''                            Assegnato < Rimanenze Iniziali
        'stb.AppendLine(" and ( isnull(cte_rend.assegnato_gasolio,0) + isnull(cte_rend.assegnato_benzina,0) + isnull(cte_rend.assegnato_gasolio_serra,0) ) < ")
        'stb.AppendLine("     ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) ")
        ''                            Recupero Accise Confermato > Rimanenze Iniziali
        'stb.AppendLine(" and ( isnull(t.Rec_Acc_Conf_Gasolio,0) + isnull(t.Rec_Acc_Conf_Benzina,0) + isnull(t.Rec_Acc_Conf_Gasolio_Serra,0) ) >")
        'stb.AppendLine("     ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) ")
        'stb.AppendLine("then 3 ")
        ''--- LITRI IN ESUBERO
        'stb.AppendLine("else 1 ")
        ''---
        'stb.AppendLine("end Causale")
        stb.AppendLine(",8 Causale ")
        '----------------------------------------------------------------------------------------------------
        'DESCRIZIONE CAUSALE RECUPERO ACCISE
        '----------------------------------------------------------------------------------------------------
        'stb.AppendLine(",case")
        ''--- RIMANENZE NON RIASSEGNATE
        ''                            Rimanenze Iniziali Presenti
        'stb.AppendLine("when ( isnull(cte_prime_rich.Rimanenza_Gasolio, 0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) > 0 ")
        ''                            Assegnato < Rimanenze Iniziali
        'stb.AppendLine(" and ( isnull(cte_rend.assegnato_gasolio,0) + isnull(cte_rend.assegnato_benzina,0) + isnull(cte_rend.assegnato_gasolio_serra,0) ) < ")
        'stb.AppendLine("     ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) ")
        ''                            Recupero Accise Confermato <= Rimanenze Iniziali
        'stb.AppendLine(" and ( isnull(t.Rec_Acc_Conf_Gasolio,0) + isnull(t.Rec_Acc_Conf_Benzina,0) + isnull(t.Rec_Acc_Conf_Gasolio_Serra,0) ) <= ")
        'stb.AppendLine("     ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) ")
        'stb.AppendLine("then 'Rimanenze non riassegnate'")
        ''--- RIMANENZE NON RIASSEGNATE E LITRI IN ESUBERO
        ''                            Rimanenze Iniziali Presenti
        'stb.AppendLine("when ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) > 0 ")
        ''                            Assegnato < Rimanenze Iniziali
        'stb.AppendLine(" and ( isnull(cte_rend.assegnato_gasolio,0) + isnull(cte_rend.assegnato_benzina,0) + isnull(cte_rend.assegnato_gasolio_serra,0) ) < ")
        'stb.AppendLine("     ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) ")
        ''                            Recupero Accise Confermato > Rimanenze Iniziali
        'stb.AppendLine(" and ( isnull(t.Rec_Acc_Conf_Gasolio,0) + isnull(t.Rec_Acc_Conf_Benzina,0) + isnull(t.Rec_Acc_Conf_Gasolio_Serra,0) ) >")
        'stb.AppendLine("     ( isnull(cte_prime_rich.Rimanenza_Gasolio,0) + isnull(cte_prime_rich.Rimanenza_Benzina,0) + isnull(cte_prime_rich.Rimanenza_Gasolio_Serra,0) ) ")
        'stb.AppendLine("then 'Rimanenze non riassegnate + Litri in esubero' ")
        ''--- LITRI IN ESUBERO
        'stb.AppendLine("else 'Litri in esubero' ")
        ''---
        'stb.AppendLine("end CausaleDescr")
        stb.AppendLine(",'Dichiarato in rendicontazione' CausaleDescr ")
        '----------------------------------------------------------------------------------------------------
        stb.AppendLine(", urra.Rec_Acc_Data as Data_Segnalazione")
        AppendUtenteSegnalazione(stb)
        stb.AppendLine("from UMA_Richieste_Testata t")
        stb.AppendLine("inner join Pratiche p on p.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join Pratiche_Stati_Attuali psa on psa.Pratica_Cod = t.Pratica_Cod")
        stb.AppendLine("inner join Imprese i on i.PIVA = t.piva")
        stb.AppendLine("inner join cte_Prov_Com cpt on cpt.Piva = i.Piva")
        stb.AppendLine("left join cte_prime_richieste cte_prime_rich  on cte_prime_rich.Piva = t.piva and")
        stb.AppendLine("                                                 cte_prime_rich.Anno = " & Agro_SQL_SaveNum(anno) & " and")
        stb.AppendLine("                                                 cte_prime_rich.Tipo_Richiesta = t.Tipo_Richiesta")
        stb.AppendLine("left join cte_rendicontazioni cte_rend on cte_rend.Piva = t.Piva and")
        stb.AppendLine("                                          cte_rend.Anno = " & Agro_SQL_SaveNum(anno) & " and")
        stb.AppendLine("                                          cte_rend.Tipo_Richiesta = t.Tipo_Richiesta")
        stb.AppendLine("left join cte_acquisti_x_carb cte_acq on cte_acq.PIVA = t.piva and cte_acq.Conto_Proprio_Terzi = t.Tipo_Richiesta")
        AppendJoinRecAcc(stb, anno, 0, 0)
        stb.AppendLine("left join " & NomeDB_Utenti & ".dbo.utenti_dettagli utentesegn ON utentesegn.UserName = urra.Rec_Acc_Utente")
        stb.AppendLine("where year(t.Validita_Inizio) = " & Agro_SQL_SaveNum(anno) & "")
        stb.AppendLine("and   t.Avanzamento_Richiesta = 1")
        AppendFiltroGiaSegnalate(stb, giaSegnalate, Agro_SQL_SaveText(segnalateDal))
        If conto < 2 Then
            stb.AppendLine("and t.Tipo_Richiesta = " & conto.ToString & "")
        End If
        stb.AppendLine("and   psa.Stato_Cod = 2005")
        stb.AppendLine("and  (isnull(t.Rec_Acc_Conf_Gasolio,0) + isnull(t.Rec_Acc_Conf_Benzina,0) + isnull(t.Rec_Acc_Conf_Gasolio_Serra,0)) > 0")
    End Sub

    Private Sub AppendCteProvECom(stb As StringBuilder, prov As String, com As String, primaCte As Boolean)
        If primaCte Then
            stb.Append(" WITH ")
        Else
            stb.Append(" , ")
        End If

        stb.AppendLine("cte_Prov_Com as")
        stb.AppendLine("(")
        stb.AppendLine("select ic.piva, ic.val_cod cuaa, ISTAT.LOCALITA AS citta, i.ind_des As via, i.pro_cod As Prov from ")
        stb.AppendLine("Imprese_Codici ic   ")
        stb.AppendLine("INNER JOIN ImpresexIndirizzi ixi ON ic.Piva = ixi.Piva ")
        stb.AppendLine("INNER JOIN Indirizzi i ON ixi.Cod_Indirizzo = i.Cod_Indirizzo   ")
        stb.AppendLine("LEFT JOIN ISTAT ON i.pro_cod_istat = ISTAT.PROV And i.com_cod_istat = ISTAT.COM ")
        stb.AppendLine("where  ")
        stb.AppendLine("ic.id_cod = 1010 ")

        If (com <> "0-1" AndAlso com <> "") Then
            stb.AppendLine(" And ISTAT.COM = '" + Agro_SQL_SaveText(com) + "' ")
        End If

        If (prov <> "0-1") Then
            stb.AppendLine(" AND ISTAT.PROV = '" + Agro_SQL_SaveText(prov) + "' ")
        End If

        stb.AppendLine(" ) ")

    End Sub

End Class

Public Class UMA_Richieste_Rec_Acc_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiRecAcc(piva As String,
                                Richiesta_Cod As Integer,
                                rec_acc_anno As Integer,
                                doc_manc As Integer,
                                xOrderBy As String,
                                xFiltroAggiuntivo As String,
                                ByRef objParametriServer As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Report_Controllo.UMA_Richieste_Rec_Acc.LeggiRecAcc()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT urra.* ")
            stb.AppendLine(" FROM UMA_Richieste_Rec_Acc urra ")
            stb.AppendLine(" WHERE 1=1 ")

            If piva <> "" Then
                stb.AppendLine(" AND urra.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If Richiesta_Cod > 0 Then
                stb.AppendLine(" AND urra.Richiesta_Cod = " & Agro_SQL_SaveNum(Richiesta_Cod) & " ")
            End If

            If rec_acc_anno > 0 Then
                stb.AppendLine(" AND urra.Rec_Acc_Anno = " & Agro_SQL_SaveNum(rec_acc_anno) & " ")
            End If

            If doc_manc > 0 Then
                stb.AppendLine(" AND urra.Doc_Manc = " & Agro_SQL_SaveNum(doc_manc) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy) & " ")
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

End Class

Public Class UMA_Richieste_Rec_Acc_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Private Const DIMENSIONE_BULK = 10

    Public Function AggiungiNuovi(listaNuovi As List(Of UMA_Rec_Acc), ByRef objParametriServer As AgronicaCoreParametri) As Boolean

        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_Rec_Acc_W.AggiungiNuovi()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim PivaSuperUser = objParametriServer.PivaSuperUser
        Dim bulks As Integer = Math.Round(listaNuovi.Count / DIMENSIONE_BULK, 0)
        Dim listaTemp As New List(Of UMA_Rec_Acc)

        For temp = 0 To bulks + 1

            If temp > 0 Then
                listaTemp = listaNuovi.Skip(DIMENSIONE_BULK * temp).Take(DIMENSIONE_BULK).ToList
            Else
                listaTemp = listaNuovi.Take(DIMENSIONE_BULK).ToList
            End If

            If listaTemp.Count > 0 Then

                Try

                    stb = New System.Text.StringBuilder

                    stb.AppendLine(" INSERT INTO UMA_Richieste_Rec_Acc ( ")
                    stb.AppendLine(" Piva_SuperUser, Piva, Richiesta_Cod, Rec_Acc_Anno, ")
                    stb.AppendLine(" Validita_Inizio, Validita_Fine, Inviato, DataInvio, ")
                    stb.AppendLine(" Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica,")
                    stb.AppendLine(" Rec_Acc_Gasolio, Rec_Acc_Benzina, Rec_Acc_Gasolio_Serra,")
                    stb.AppendLine(" Rec_Acc_Data, Rec_Acc_Utente, Rec_Acc_Causale, Doc_Manc ) VALUES ")

                    For Each nuovo In listaTemp

                        If Not stb.ToString.Trim.EndsWith("VALUES") Then
                            stb.AppendLine(",")
                        End If

                        stb.AppendLine(" ( '" & Agro_SQL_SaveText(objParametriServer.PivaSuperUser) & "', ")
                        stb.AppendLine(" '" & Agro_SQL_SaveText(nuovo.Piva) & "', ")
                        stb.AppendLine(" " & Agro_SQL_SaveNum(nuovo.Richiesta_Cod) & ", ")
                        stb.AppendLine(" " & Agro_SQL_SaveNum(nuovo.Rec_Acc_Anno) & ", ")
                        stb.AppendLine(" " & Agro_SQL_SaveDate(nuovo.Validita_Inizio) & ", ")
                        stb.AppendLine(" " & Agro_SQL_SaveDate(nuovo.Validita_Fine) & ", ")
                        stb.AppendLine(" " & Agro_SQL_SaveNum(nuovo.Inviato) & ", ")
                        stb.AppendLine(" " & Agro_SQL_SaveDate(nuovo.Data_Invio) & ", ")
                        stb.AppendLine(" " & Agro_SQL_SaveDateTime(nuovo.Data_Creazione) & ", ")
                        stb.AppendLine(" " & Agro_SQL_SaveDateTime(nuovo.Data_modifica) & ", ")
                        stb.AppendLine(" '" & Agro_SQL_SaveText(nuovo.Username_creazione) & "', ")
                        stb.AppendLine(" '" & Agro_SQL_SaveText(nuovo.Username_Modifica) & "', ")
                        stb.AppendLine(" " & Agro_SQL_SaveNum(nuovo.Rec_acc_Gasolio) & ", ")
                        stb.AppendLine(" " & Agro_SQL_SaveNum(nuovo.Rec_acc_Benzina) & ", ")
                        stb.AppendLine(" " & Agro_SQL_SaveNum(nuovo.Rec_acc_Gasolio_Serra) & ", ")
                        stb.AppendLine(" " & Agro_SQL_SaveDate(nuovo.Rec_acc_Data) & ", ")
                        stb.AppendLine(" '" & Agro_SQL_SaveText(nuovo.Rec_acc_Utente) & "', ")
                        stb.AppendLine(" " & Agro_SQL_SaveNum(nuovo.Rec_acc_Causale) & ", ")
                        stb.AppendLine(" " & Agro_SQL_SaveNum(nuovo.Doc_Manc) & " ) ")

                    Next

                    xRisp = EseguiQuery_Scrittura(objParametriServer, stb.ToString, nomeRoutine)

                Catch ex As Exception
                    MessaggioErrore = ex.Message
                    Scrivi_LOG(objParametriServer, nomeRoutine, MessaggioErrore)
                    Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
                End Try

            End If

        Next

        Return True

    End Function

    Public Function AggiornaEsistenti(listaAggiornamento As List(Of UMA_Rec_Acc), ByRef objParametriServer As AgronicaCoreParametri) As Boolean

        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_Rec_Acc_W.AggiornaEsistenti()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim PivaSuperUser = objParametriServer.PivaSuperUser

        Try

            stb = New System.Text.StringBuilder

            For Each aggiorna In listaAggiornamento

                stb.AppendLine(" UPDATE UMA_Richieste_Rec_Acc SET")

                stb.AppendLine("  Rec_Acc_Data = " & Agro_SQL_SaveDate(aggiorna.Rec_acc_Data) & " ")
                stb.AppendLine(" ,Rec_Acc_Utente = '" & Agro_SQL_SaveText(aggiorna.Rec_acc_Utente) & "' ")
                stb.AppendLine(" ,Rec_Acc_Causale = " & Agro_SQL_SaveNum(aggiorna.Rec_acc_Causale) & " ")
                stb.AppendLine(" ,Rec_Acc_Gasolio = " & Agro_SQL_SaveNum(aggiorna.Rec_acc_Gasolio) & " ")
                stb.AppendLine(" ,Rec_Acc_Benzina = " & Agro_SQL_SaveNum(aggiorna.Rec_acc_Benzina) & " ")
                stb.AppendLine(" ,Rec_Acc_Gasolio_Serra = " & Agro_SQL_SaveNum(aggiorna.Rec_acc_Gasolio_Serra) & " ")
                stb.AppendLine(" ,Username_Modifica = '" & Agro_SQL_SaveText(aggiorna.Username_Modifica) & "' ")
                stb.AppendLine(" ,Data_Modifica = " & Agro_SQL_SaveDateTime(aggiorna.Data_modifica) & " ")

                stb.AppendLine(" WHERE ")

                stb.AppendLine(" Piva = '" & Agro_SQL_SaveText(aggiorna.Piva) & "' ")
                stb.AppendLine(" AND Richiesta_Cod = " & Agro_SQL_SaveNum(aggiorna.Richiesta_Cod) & " ")
                stb.AppendLine(" AND Rec_Acc_Anno = " & Agro_SQL_SaveNum(aggiorna.Rec_Acc_Anno) & " ")
                stb.AppendLine(" AND Doc_Manc = " & Agro_SQL_SaveNum(aggiorna.Doc_Manc) & " ; ")

            Next

            xRisp = EseguiQuery_Scrittura(objParametriServer, stb.ToString, nomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True

    End Function

    Public Function AnnullaSegnalazioni(listaAnnullati As List(Of UMA_Rec_Acc), ByRef objParametriServer As AgronicaCoreParametri) As Boolean

        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_Rec_Acc_W.AnnullaSegnalazioni()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Dim PivaSuperUser = objParametriServer.PivaSuperUser

        Try

            stb = New System.Text.StringBuilder

            For Each annullamento In listaAnnullati

                stb.AppendLine(" DELETE FROM UMA_Richieste_Rec_Acc WHERE")

                stb.AppendLine(" Piva = '" & Agro_SQL_SaveText(annullamento.Piva) & "' ")
                stb.AppendLine(" AND Richiesta_Cod = " & Agro_SQL_SaveNum(annullamento.Richiesta_Cod) & " ")
                stb.AppendLine(" AND Rec_Acc_Anno = " & Agro_SQL_SaveNum(annullamento.Rec_Acc_Anno) & " ")
                stb.AppendLine(" AND Doc_Manc = " & Agro_SQL_SaveNum(annullamento.Doc_Manc) & " ; ")

            Next

            xRisp = EseguiQuery_Scrittura(objParametriServer, stb.ToString, nomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return True

    End Function

End Class

'TODO Eliminare una volta aggiunta la tabella UMA_Richieste_Rec_Acc all'EF e aggiornare il BIZ
<Serializable()>
<DataContract(IsReference:=True)>
Public Class UMA_Rec_Acc

    'Simple Properties
    <DataMember()>
    Public Property Piva As String

    <DataMember()>
    Public Property Richiesta_Cod As Integer

    <DataMember()>
    Public Property Rec_Acc_Anno As Integer

    <DataMember()>
    Public Property Validita_Inizio As DateTime

    <DataMember()>
    Public Property Validita_Fine As DateTime

    <DataMember()>
    Public Property Inviato As Integer

    <DataMember()>
    Public Property Data_Invio As DateTime

    <DataMember()>
    Public Property Data_Creazione As DateTime

    <DataMember()>
    Public Property Data_modifica As DateTime

    <DataMember()>
    Public Property Username_creazione As String

    <DataMember()>
    Public Property Username_Modifica As String

    <DataMember()>
    Public Property Rec_acc_Gasolio As Decimal

    <DataMember()>
    Public Property Rec_acc_Benzina As String

    <DataMember()>
    Public Property Rec_acc_Gasolio_Serra As String

    <DataMember()>
    Public Property Rec_acc_Utente As String

    <DataMember()>
    Public Property Rec_acc_Data As DateTime

    <DataMember()>
    Public Property Rec_acc_Causale As String

    <DataMember()>
    Public Property Doc_Manc As Integer

End Class
