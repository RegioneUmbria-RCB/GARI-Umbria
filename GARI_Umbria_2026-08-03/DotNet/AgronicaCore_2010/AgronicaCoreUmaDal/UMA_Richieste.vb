Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Data.Entity.Core.Metadata.Edm

Public Class UMA_Richieste_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Const PERCENTUALE_PENDENZA_B As Integer = 10

    Public Function Leggi(ByVal piva As String,
                          ByVal richiesta_cod As Integer,
                          ByVal gruppo_uma As String,
                          ByVal Programmazione_Cod As Integer,
                          ByVal Selezione_Variabile As enumSelezioneVariabile,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal Regolamento_Cod As Integer = 0,
                          Optional ByVal EscludiFascicoliFittizi As Boolean = False
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim selectList As String
        Dim PivaSuperUser = objParametri.PivaSuperUser

        If (Selezione_Variabile = enumSelezioneVariabile.Selezione_TabellaDatiMinimi) Then
            selectList = "u.Piva, u.Programmazione_Cod, u.Gruppo_Colturale_UMA, u.Programmazione_Cod, u.Zona_Pendenza_A_UMA, u.Zona_Pendenza_B_UMA"
        Else
            selectList = "u.*, UMA_Macrousi.Macrouso_UMA_Des, Programmazione_Testata.Programmazione_Des"
        End If

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT " + selectList + " ")
            stb.AppendLine(" FROM UMA_Richieste u")
            stb.AppendLine(" LEFT JOIN Programmazione_Testata ON u.Programmazione_Cod = Programmazione_Testata.Programmazione_Cod ")
            stb.AppendLine(" LEFT JOIN UMA_Macrousi ON u.Gruppo_Colturale_UMA = UMA_Macrousi.Macrouso_UMA_Cod ")
            stb.AppendLine(" JOIN UMA_Richieste_Testata urt on urt.Richiesta_Cod = u.Richiesta_Cod ")
            stb.AppendLine(" WHERE ")

            If (gruppo_uma <> "") Then
                stb.AppendLine(" u.Gruppo_Colturale_UMA = '" + Agro_SQL_SaveText(gruppo_uma) + "' AND ")
            End If

            If piva <> "" Then
                stb.AppendLine(" urt.Piva = '" + Agro_SQL_SaveText(piva) + "' AND ")
            End If

            If Programmazione_Cod <> 0 Then
                If EscludiFascicoliFittizi Then
                    stb.AppendLine(" u.Programmazione_Cod = " + Agro_SQL_SaveNum(Programmazione_Cod) + " AND ")
                Else
                    stb.AppendLine(" u.Programmazione_Cod IN (" + Agro_SQL_SaveNum(Programmazione_Cod) + ", -1, -2, -3, -4) AND ")
                End If
            End If

            If Regolamento_Cod <> 0 Then
                stb.AppendLine(" u.Regolamento_Cod = " + Agro_SQL_SaveNum(Regolamento_Cod) + " AND ")
            End If

            stb.AppendLine(" u.Piva_SuperUser = '" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "' AND ")
            stb.AppendLine(" u.Richiesta_Cod = " + Agro_SQL_SaveNum(richiesta_cod) + " ")

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
    Public Function Leggi_CodicePratica(ByVal Pratica_Cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste.Leggi_CodicePratica()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim PivaSuperUser = objParametri.PivaSuperUser



        Try

            stb.Length = 0

            stb.AppendLine(" SELECT t.*,p.Numero ")
            stb.AppendLine(" FROM UMA_Richieste_Testata T")
            stb.AppendLine(" inner join  Pratiche p ")
            stb.AppendLine(" on p.Pratica_Cod  = t.Pratica_Cod  ")
            stb.AppendLine(" WHERE ")
            stb.AppendLine($" t.Piva_SuperUser = '{Agro_SQL_SaveText(objParametri.PivaSuperUser)}' AND ")
            stb.AppendLine($" p.Pratica_Cod = {Agro_SQL_SaveNum(Pratica_Cod)} ")

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

    Public Function Leggi_Richieste_Terzisti(ByVal piva As String,
                                             ByVal richiesta_cod As Integer,
                                             ByVal anno As Integer,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste.Leggi_Richieste_Terzisti()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable


        Try

            stb.Length = 0

            stb.AppendLine(" SELECT DISTINCT i.rag_soc, r.gruppo_colturale_UMA, c.macrouso_UMA_Des, r.*, tt.Programmazione_Des ")
            stb.AppendLine(" FROM UMA_Richieste r")
            stb.AppendLine(" JOIN UMA_Richieste_Testata t ON ")
            If (richiesta_cod > -1) Then
                stb.AppendLine("t.richiesta_cod = " + Agro_SQL_SaveNum(richiesta_cod) + " AND ")
            End If
            stb.AppendLine(" t.richiesta_cod = r.richiesta_cod ")
            stb.AppendLine(" LEFT JOIN UMA_Macrousi c on c.macrouso_UMA_Cod = r.gruppo_colturale_UMA") 'Codifica_SpecieVegetali_Agea_2015_2020
            stb.AppendLine(" JOIN Imprese i ON r.piva = i.piva")
            stb.AppendLine(" LEFT JOIN Programmazione_Testata tt on tt.Programmazione_Cod = r.Programmazione_Cod ")
            stb.AppendLine(" WHERE t.piva = '" + Agro_SQL_SaveText(piva) + "' ")
            stb.AppendLine(" AND t.Tipo_Richiesta = -1 ")
            'stb.AppendLine(" AND YEAR(r.data_Creazione) = " + anno.ToString() + " ")
            stb.AppendLine(" ORDER BY i.rag_soc, r.Data_Creazione DESC ")

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

    Public Function ControlloMacrousiBio(ByVal richiesta_cod As Integer, ByVal anno As Integer,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         Optional ByVal isTerzista As Boolean = False,
                                         Optional ByVal rendicontazione As Boolean = True) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste.ControlloMacrousiBio()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" select x.rag_soc, um.Macrouso_UMA_Des, r.* from UMA_Richieste r")
            stb.AppendLine(" join UMA_Richieste_Testata rt on r.Richiesta_Cod = rt.Richiesta_Cod ")
            stb.AppendLine(" join UMA_Macrousi um on um.Macrouso_UMA_Cod = r.Gruppo_Colturale_UMA")
            stb.AppendLine(" join (Select i.rag_soc, rr.* from UMA_Richieste rr join UMA_Richieste_Testata rrt ")
            stb.AppendLine(" on rr.Richiesta_Cod = rrt.Richiesta_Cod ")
            stb.AppendLine(" join Pratiche p on p.pratica_cod = rrt.pratica_cod ")
            stb.AppendLine(" join imprese i on i.PIVA = rrt.Piva ")
            stb.AppendLine(" where rr.Richiesta_Cod <> " + richiesta_cod.ToString + " and ")
            stb.AppendLine(" rrt.Tipo_Richiesta <> " + IIf(isTerzista, "-1", "0") + " And ")
            stb.AppendLine(" rrt.Avanzamento_Richiesta = " + IIf(rendicontazione, "1", "0") + " And p.anno = '" + Agro_SQL_SaveText(anno) + "') as x ")
            stb.AppendLine(" on x.Piva = r.Piva and x.Gruppo_Colturale_UMA = r.Gruppo_Colturale_UMA and ")
            stb.AppendLine(" x.Programmazione_Cod = r.Programmazione_Cod And x.Regolamento_Cod <> r.Regolamento_Cod and NOT (x.Regolamento_Cod IN (0,1) AND r.Regolamento_Cod IN (0,1)) ")
            stb.AppendLine(" where rt.Richiesta_Cod = " + richiesta_cod.ToString + " ")

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

    Public Function EstraiLegamiRichiestaxAppezzamento(ByVal piva As String, ByVal richiesta_Cod As Integer,
                                                       ByVal xFiltroAggiuntivo As String, ByRef objParametri As AgronicaCoreParametri, Optional anno As Integer = 0) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste.TrovaColtureAppezzamentiSenzaCatasto()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" With Reg_Impianti_Codici_Dettaglio AS ( ")
            stb.AppendLine("  SELECT Piva, Appezza, Sa_Cod, Id_Reg, SUBSTRING(val_cod, 1, CHARINDEX('-', val_cod)-1) AS Occupazione_Agea, ")
            stb.AppendLine("  SUBSTRING(val_cod, 5, CHARINDEX('-', val_cod)-1) AS Destinazione_Agea, ")
            stb.AppendLine(" SUBSTRING(val_cod, 9, CHARINDEX('-', val_cod)-1) AS Uso_Agea, SUBSTRING(val_cod, 13, CHARINDEX('-', val_cod)-1) AS Qualita_Agea, ")
            stb.AppendLine(" SUBSTRING(val_cod, 17, CHARINDEX('-', val_cod)-1) AS Cul_Cod_Agea ")
            stb.AppendLine(" FROM Reg_Impianti_Codici ")
            stb.AppendLine(" Where piva = '" & Agro_SQL_SaveText(piva) & "' and id_cod = 2315 ")
            stb.AppendLine(" ), ")
            stb.AppendLine(" Appezzamento_Piva AS (")
            stb.AppendLine(" Select PIVA, APPEZZA, SA_COD, SUP_APP From Appezzamento where piva = '" & Agro_SQL_SaveText(piva) & "' ")
            stb.AppendLine(" ) ")
            stb.AppendLine(" select ri.PIVA, ri.APPEZZA, ri.SA_COD, ri.ID_REG,  ")
            stb.AppendLine(" COALESCE(a.macrouso_UMA_Cod, b.macrouso_UMA_Cod, c.Macrouso_UMA_Cod, d.Macrouso_UMA_Cod, '') as 'macrouso_UMA_Cod' ")
            stb.AppendLine(" from Reg_Impianti ri ")
            stb.AppendLine(" JOIN Appezzamento_Piva ap ON ap.APPEZZA = ri.APPEZZA and ap.SA_COD = ri.SA_COD ")
            stb.AppendLine(" INNER JOIN Reg_Impianti_Codici rc ON rc.APPEZZA = ri.APPEZZA And rc.SA_COD = ri.SA_COD And ri.PIVA = rc.PIVA AND rc.Id_Reg = ri.ID_REG")
            If richiesta_Cod > 0 AndAlso anno = 0 Then
                stb.AppendLine(" INNER JOIN UMA_Richieste_Testata rt ON rt.Richiesta_Cod = " & Agro_SQL_SaveNum(richiesta_Cod) & "")
                stb.AppendLine(" INNER JOIN Pratiche p ON p.Pratica_Cod = rt.Pratica_Cod ")
            End If
            stb.AppendLine(" JOIN Reg_Impianti_Codici_Dettaglio ric on ric.APPEZZA = ri.APPEZZA and ric.SA_COD = ri.SA_COD and ri.PIVA = ric.PIVA AND ric.Id_Reg = ri.ID_REG ")
            stb.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = ric.Occupazione_Agea AND ci.Destinazione_Cod = ric.Destinazione_Agea AND ci.Qualita_Cod = ric.Qualita_Agea AND ci.Uso_Cod = ric.Uso_Agea AND ci.Cul_Cod_Agea = ric.Cul_Cod_Agea AND ci.Macrouso_UMA_Cod IS NOT NULL AND ci.Macrouso_UMA_Des IS NOT NULL) a  ")
            stb.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = ric.Occupazione_Agea AND ci.Destinazione_Cod = ric.Destinazione_Agea AND ci.Qualita_Cod = ric.Qualita_Agea AND ci.Uso_Cod = ric.Uso_Agea AND ci.Macrouso_UMA_Cod IS NOT NULL AND ci.Macrouso_UMA_Des IS NOT NULL) b  ")
            stb.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = ric.Occupazione_Agea AND ci.Destinazione_Cod = ric.Destinazione_Agea AND ci.Macrouso_UMA_Cod IS NOT NULL AND ci.Macrouso_UMA_Des IS NOT NULL) c  ")
            stb.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = ric.Occupazione_Agea AND ci.Macrouso_UMA_Cod IS NOT NULL AND ci.Macrouso_UMA_Des IS NOT NULL) d ")
            stb.AppendLine(" where rc.id_cod = 1362 ")
            stb.AppendLine(" AND (ri.flagCessata = 0 OR ri.flagCessata IS NULL) ")
            If richiesta_Cod > 0 AndAlso anno = 0 Then
                stb.AppendLine(" AND p.anno = rc.val_cod ")
            ElseIf richiesta_Cod = 0 AndAlso anno > 0 Then
                stb.AppendLine(" And ric.val_cod = " & Agro_SQL_SaveNum(anno) & "")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
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

    ''' <summary>
    ''' Restituisce le colture (superficie totale, pendenze e macrousi UMA) degli appezzamenti che hanno validità inizio antecedente alla validità specificata.
    ''' Le pendenze vengono prese prima dalle particelle e, in assenza di esse, dall'appezzamento.
    ''' </summary>
    ''' <param name="piva">L'azienda che possiede gli appezzamenti</param>
    ''' <param name="validitaInizio">La data alla quale gli appezzamenti devono essere validi</param>
    ''' <param name="objParametri"></param>
    ''' <param name="particelle">Se true, include nel risultato la chiave delle particelle (FOGLIO, SUBALTERNO, PROV, COM, NUMERO, SEZIONE)</param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="terreniInUmbria">Se true, include solo i terreni presenti in Umbria, altrimenti ritorna solo i terreni NON in Umbria</param>
    ''' <returns></returns>
    Public Function TrovaColtureAppezzamentiSenzaFascicolo(ByVal piva As String, ByVal richiesta_Cod As Integer, ByRef objParametri As AgronicaCoreParametri,
                                                         Optional ByVal particelle As Boolean = False, Optional xFiltroAggiuntivo As String = "", Optional terreniInUmbria As Boolean = True, Optional anno As Integer = 0,
                                                           Optional ByVal TrovaTutti As Boolean = False) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreUmaDAL.UMA_Richieste.TrovaColtureAppezzamentiSenzaCatasto()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim listaProvinceRegioniLimitrofe As String = "'041', '042', '043', '044', '045', '046','047','048','049','050','051','052','053','056','057','058','059', '060'"
        Dim outerSelect As String = "ci.macrouso_UMA_Cod, ci.macrouso_UMA_Des, ci.Id_Cod, ci.Veg_Cod_Agea, ci.Veg_Des_Agea, ci.Destinazione_Des "

        Try

            stb.Length = 0

            stb.AppendLine(" ;WITH Reg_Impianti_Codici_Dettaglio AS ( ")
            stb.AppendLine("    SELECT")
            stb.AppendLine("         Piva, Appezza, Sa_Cod, Id_Reg, SUBSTRING(val_cod, 1, CHARINDEX('-', val_cod)-1) AS Occupazione_Agea ")
            stb.AppendLine("       , SUBSTRING(val_cod, 5, CHARINDEX('-', val_cod)-1) AS Destinazione_Agea ")
            stb.AppendLine("       , SUBSTRING(val_cod, 9, CHARINDEX('-', val_cod)-1) AS Uso_Agea, SUBSTRING(val_cod, 13, CHARINDEX('-', val_cod)-1) AS Qualita_Agea ")
            stb.AppendLine("       , SUBSTRING(val_cod, 17, CHARINDEX('-', val_cod)) AS Cul_Cod_Agea ")
            stb.AppendLine("    FROM Reg_Impianti_Codici")
            stb.AppendLine($" WHERE piva = '{ Agro_SQL_SaveText(piva) }' and id_cod = 2315 ")
            stb.AppendLine(" ), ")

            If TrovaTutti Then
                stb.AppendLine(" Centri_indirizzi As ( ")
                stb.AppendLine("   Select DISTINCT caz.sa_nome, ca.sa_cod, ca.PIVA, ind.com_des, ind.pro_cod from reg_impianti ri ")
                stb.AppendLine("   join Centri_Aziendali caz on caz.PIVA = ri.PIVA and caz.sa_cod = ri.SA_COD ")
                stb.AppendLine("   join CentrixIndirizzi ca on ca.PIVA = ri.PIVA and ca.sa_cod = ri.SA_COD ")
                stb.AppendLine("   join Indirizzi ind on ind.cod_indirizzo = ca.cod_indirizzo ")
                stb.AppendLine(" ), ")
            End If

            stb.AppendLine(" ZonexParticelleCTE AS ( ")
            stb.AppendLine("    select * from ZonexParticelle ")
            stb.AppendLine("    Where prov In ")
            If terreniInUmbria Then
                stb.AppendLine(" ('054', '055') ")
            Else
                stb.AppendLine(" (" & listaProvinceRegioniLimitrofe & ") ")
            End If
            stb.AppendLine(" ), ")

            stb.AppendLine(" Appezzamento_Piva AS ( ")
            stb.AppendLine("    SELECT PIVA, APPEZZA, SA_COD, SUP_APP, PENDE, APP_NOME, Validita_Inizio, Validita_Fine  ")
            stb.AppendLine("    FROM Appezzamento ")
            stb.AppendLine($"    WHERE piva = '{ Agro_SQL_SaveText(piva) }' ")
            stb.AppendLine(" ), ")

            stb.AppendLine(" Particelle AS (")
            stb.AppendLine("                     SELECT")
            If TrovaTutti Then
                stb.AppendLine("                         APP_NOME, ic.Provincia as PROV, ic.Descrizione as COM, ap.validita_inizio As INIZIO_GESTIONE, ap.validita_fine as FINE_GESTIONE, ")
            End If
            stb.AppendLine("                         ri.PIVA, ri.APPEZZA, ri.SA_COD, ri.ID_REG, ri.REGOLAMENTO, ri.Sup_Imp, ap.SUP_APP, MAX(ap.PENDE) AS pendenza,")
            stb.AppendLine($"                         SUM(COALESCE(CASE WHEN (zz.Zona_Cod = {CInt(enum_Zone.A_LEGGERA_PENDENZA_NO_MAGGIORAZIONE)}) THEN axp.AREA END, 0)) AS sup_A,")
            stb.AppendLine($"                         SUM(COALESCE(CASE zz.Zona_Cod WHEN {CInt(enum_Zone.B_PENDENZA_ACCENTUATA_MAGGIORAZIONE_20perc)} THEN axp.AREA END, 0)) AS sup_B,")
            stb.AppendLine("                         SUM(COALESCE(CASE WHEN zz.Zona_Cod Is NULL THEN axp.AREA END, 0)) AS sup_Pend_NULL")

            If particelle Then
                stb.AppendLine("    , axp.COM, axp.PROV, axp.SEZIONE, axp.FOGLIO, axp.SUBALTERNO, axp.NUMERO, APP_NOME ")
            End If

            stb.AppendLine("                     FROM Reg_Impianti ri")
            stb.AppendLine("                     JOIN Appezzamento_Piva ap ON ap.APPEZZA = ri.APPEZZA And ap.SA_COD = ri.SA_COD")
            stb.AppendLine("                     INNER JOIN Reg_Impianti_Codici ric ON ric.APPEZZA = ri.APPEZZA And ric.SA_COD = ri.SA_COD And ri.PIVA = ric.PIVA AND ric.Id_Reg = ri.ID_REG")
            If richiesta_Cod > 0 AndAlso anno = 0 Then
                stb.AppendLine(" INNER JOIN UMA_Richieste_Testata rt ON rt.Richiesta_Cod = " & Agro_SQL_SaveNum(richiesta_Cod) & "")
                stb.AppendLine(" INNER JOIN Pratiche p ON p.Pratica_Cod = rt.Pratica_Cod ")
            End If
            stb.AppendLine("                     LEFT OUTER join AppezzamentixIndirizzi axi on axi.appezza = ap.APPEZZA and axi.sa_cod = ap.SA_COD and axi.PIVA = ap.PIVA ")
            stb.AppendLine("                     Left OUTER join Indirizzi ind on ind.cod_indirizzo = axi.cod_indirizzo ")
            stb.AppendLine("                     LEFT OUTER JOIN AppezzamentiXParticelle axp On axp.APPEZZA = ap.APPEZZA And axp.SA_COD = ap.SA_COD And axp.PIVA = ap.PIVA ")
            stb.AppendLine("                     LEFT OUTER JOIN ZonexParticelleCTE z On z.Piva_SuperUser = '" & objParametri.PivaSuperUser & "' and axp.prov = z.PROV And axp.Com = z.COM And axp.Sezione = z.SEZIONE And axp.Foglio = z.FOGLIO And axp.Numero = z.NUMERO And axp.Subalterno = z.SUBALTERNO ")
            stb.AppendLine("                     LEFT OUTER JOIN Zone zz On zz.zona_cod = z.Zona_Cod ")

            If TrovaTutti Then
                stb.AppendLine("                     LEFT OUTER JOIN ISTAT_Comuni ic on ic.Com_Cod_Istat = axp.COM and ic.Pro_Cod_Istat = axp.PROV ")
            End If

            stb.AppendLine($"                    where ric.id_cod = 1362 And (z.Zona_Cod IS NULL or z.Zona_Cod IN (-46, -47)) ")
            stb.AppendLine("                    AND (ri.flagCessata = 0 OR ri.flagCessata IS NULL) ")
            If richiesta_Cod > 0 AndAlso anno = 0 Then
                stb.AppendLine(" AND p.anno = ric.val_cod ")
            ElseIf richiesta_Cod = 0 AndAlso anno > 0 Then
                stb.AppendLine(" And ric.val_cod = " & Agro_SQL_SaveNum(anno) & "")
            End If
            'stb.AppendLine($"                           And ri.Piva = '{ Agro_SQL_SaveText(piva) }' ")
            If terreniInUmbria Then
                stb.AppendLine("    AND (axp.prov In ('054', '055') OR ind.pro_cod_istat IN ('054','055')) ")
            Else
                stb.AppendLine("    AND (axp.prov In (" & listaProvinceRegioniLimitrofe & ") OR ind.pro_cod_istat IN (" & listaProvinceRegioniLimitrofe & "))")
            End If
            stb.AppendLine("                     GROUP BY ri.PIVA, ri.APPEZZA, ri.SA_COD, ri.ID_REG, ri.REGOLAMENTO, ri.Sup_Imp, ap.SUP_APP")

            If TrovaTutti Then
                stb.AppendLine(" , APP_NOME, ic.Provincia, ic.Descrizione, ap.validita_inizio, ap.validita_fine ")
            End If

            If particelle Then
                stb.AppendLine("    , axp.COM, axp.PROV, axp.SEZIONE, axp.FOGLIO, axp.SUBALTERNO, axp.NUMERO, APP_NOME ")
            End If

            stb.AppendLine("                 ),")
            stb.AppendLine("                 Particelle_Aggregate AS (")
            stb.AppendLine("                     SELECT")
            stb.AppendLine("                         PIVA, APPEZZA, SA_COD, ID_REG, REGOLAMENTO, Sup_Imp,")
            stb.AppendLine("                         CASE")
            stb.AppendLine("                             WHEN Sup_Imp = SUP_APP THEN")
            stb.AppendLine("                                 CASE")
            stb.AppendLine("                                     WHEN sup_Pend_NULL > 0 THEN CASE WHEN pendenza >  " & PERCENTUALE_PENDENZA_B & "  THEN 0 ELSE CASE WHEN sup_A + sup_B + sup_Pend_NULL = Sup_Imp THEN sup_Pend_NULL + sup_A + sup_B ELSE Sup_Imp END END")
            stb.AppendLine("                                     WHEN sup_A + sup_B <> Sup_Imp THEN CASE WHEN pendenza >  " & PERCENTUALE_PENDENZA_B & "  THEN 0 ELSE Sup_Imp END")
            stb.AppendLine("                                     ELSE sup_A")
            stb.AppendLine("                                 END")
            stb.AppendLine("                             ELSE")
            stb.AppendLine("                                 CASE")
            stb.AppendLine("                                     WHEN sup_A + sup_B > 0 THEN CASE WHEN sup_A > 0 THEN Sup_Imp ELSE 0 END")
            stb.AppendLine("                                     ELSE CASE WHEN pendenza >  " & PERCENTUALE_PENDENZA_B & "  THEN 0 ELSE Sup_Imp END")
            stb.AppendLine("                                 END")
            stb.AppendLine("                         END AS sup_A,")
            stb.AppendLine("                         CASE")
            stb.AppendLine("                             WHEN Sup_Imp = SUP_APP THEN")
            stb.AppendLine("                                 CASE")
            stb.AppendLine("                                     WHEN sup_Pend_NULL > 0 THEN CASE WHEN pendenza >  " & PERCENTUALE_PENDENZA_B & "  THEN CASE WHEN sup_A + sup_B + sup_Pend_NULL = Sup_Imp THEN sup_Pend_NULL + sup_A + sup_B ELSE Sup_Imp END ELSE 0 END")
            stb.AppendLine("                                     WHEN sup_A + sup_B <> Sup_Imp THEN CASE WHEN pendenza >  " & PERCENTUALE_PENDENZA_B & "  THEN Sup_Imp ELSE 0 END")
            stb.AppendLine("                                     ELSE sup_B")
            stb.AppendLine("                                 END")
            stb.AppendLine("                             ELSE")
            stb.AppendLine("                                 CASE")
            stb.AppendLine("                                     WHEN sup_A + sup_B > 0 THEN CASE WHEN sup_B > 0 THEN Sup_Imp ELSE 0 END")
            stb.AppendLine("                                     ELSE CASE WHEN pendenza >  " & PERCENTUALE_PENDENZA_B & "  THEN Sup_Imp ELSE 0 END")
            stb.AppendLine("                                 END")
            stb.AppendLine("                         END AS sup_B")

            If particelle Then
                stb.AppendLine("    , COM, PROV, SEZIONE, FOGLIO, SUBALTERNO, NUMERO ")
            End If

            If TrovaTutti Then
                stb.AppendLine(" , APP_NOME, prov, com, INIZIO_GESTIONE, FINE_GESTIONE ")
            End If

            stb.AppendLine("                     FROM Particelle")
            stb.AppendLine("                 ),")
            stb.AppendLine("                  PendenzaApp AS ( ")
            stb.AppendLine("                 SELECT")
            stb.AppendLine("                     PIVA, APPEZZA, SA_COD, ID_REG, REGOLAMENTO, Sup_Imp,")
            stb.AppendLine("                     CASE")
            stb.AppendLine("                         WHEN sup_A + sup_B <> Sup_Imp THEN CASE WHEN sup_A > 0 THEN sup_A ELSE 0 END")
            stb.AppendLine("                         ELSE sup_A")
            stb.AppendLine("                     END AS sup_A,")
            stb.AppendLine("                     CASE")
            stb.AppendLine("                         WHEN sup_A + sup_B <> Sup_Imp THEN CASE WHEN sup_A > 0 THEN 0 ELSE sup_B END")
            stb.AppendLine("                         ELSE sup_B")
            stb.AppendLine("                     END AS sup_B")

            If particelle Then
                stb.AppendLine("    , COM, PROV, SEZIONE, FOGLIO, SUBALTERNO, NUMERO ")
            End If

            If TrovaTutti Then
                stb.AppendLine(" , APP_NOME, prov, com, INIZIO_GESTIONE, FINE_GESTIONE ")
            End If

            stb.AppendLine("                 FROM Particelle_Aggregate ")
            stb.AppendLine(" ) ")
            stb.AppendLine(" SELECT * FROM (")
            stb.AppendLine(" SELECT DISTINCT")
            stb.AppendLine("      ri.PIVA, ri.APPEZZA, ri.SA_COD, ri.ID_REG, ri.REGOLAMENTO, ri.Sup_Imp AS SUP_APP ")
            stb.AppendLine("    , ric.Occupazione_Agea, ric.Destinazione_Agea, ric.Uso_Agea, ric.Qualita_Agea, ric.Cul_Cod_Agea")
            stb.AppendLine("    , COALESCE(a.macrouso_UMA_Cod, b.macrouso_UMA_Cod, c.Macrouso_UMA_Cod, d.Macrouso_UMA_Cod, '') AS 'macrouso_UMA_Cod' ")
            stb.AppendLine("    , COALESCE(a.macrouso_UMA_Des, b.macrouso_UMA_Des, c.macrouso_UMA_Des, d.Macrouso_UMA_Des, '') AS 'macrouso_UMA_Des' ")

            If TrovaTutti Then
                stb.AppendLine("    , COALESCE(a.Veg_Des_Agea, b.Veg_Des_Agea, c.Veg_Des_Agea, d.Veg_Des_Agea, '') AS 'Veg_Des' ")
                stb.AppendLine("    , COALESCE(a.Destinazione_Des, b.Destinazione_Des, c.Destinazione_Des, d.Destinazione_Des, '') AS 'Destinazione' ")
            End If
            stb.AppendLine("    , COALESCE(a.Id_Cod, b.Id_Cod, c.Id_Cod, d.Id_Cod, '') AS 'Id_Cod' ")
            stb.AppendLine("    , COALESCE(a.Veg_Cod_Agea, b.Veg_Cod_Agea, c.Veg_Cod_Agea, d.Veg_Cod_Agea, '') AS 'Veg_Cod' ")
            stb.AppendLine("    , sup_A, sup_B, ri.Sup_Imp AS Sup_Normale, 0.0 AS Sup_Media, 0.0 AS Sup_Tenace ")

            If particelle Then
                stb.AppendLine("    , COM, PROV, SEZIONE, FOGLIO, SUBALTERNO, NUMERO ")
            End If

            If TrovaTutti Then
                stb.AppendLine(" , APP_NOME, CASE WHEN ri.prov IS NULL THEN ci.pro_cod ELSE ri.PROV END AS PROV, CASE WHEN ri.COM IS NULL THEN ci.com_des ELSE ri.COM END AS COM, ci.sa_nome AS Centro, INIZIO_GESTIONE, FINE_GESTIONE ")
            End If

            stb.AppendLine(" FROM PendenzaApp ri ")
            stb.AppendLine(" JOIN Reg_Impianti_Codici_Dettaglio ric on ric.APPEZZA = ri.APPEZZA and ric.SA_COD = ri.SA_COD and ri.PIVA = ric.PIVA AND ric.Id_Reg = ri.ID_REG ")

            If TrovaTutti Then
                stb.AppendLine(" JOIN Centri_indirizzi ci on ci.PIVA = ri.PIVA and ci.sa_cod = ri.SA_COD ")
            End If

            stb.AppendLine(" OUTER APPLY (Select TOP 1 " + outerSelect + " FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = ric.Occupazione_Agea And ci.Destinazione_Cod = ric.Destinazione_Agea And ci.Qualita_Cod = ric.Qualita_Agea And ci.Uso_Cod = ric.Uso_Agea And ci.Cul_Cod_Agea = ric.Cul_Cod_Agea And ci.Macrouso_UMA_Cod Is Not NULL And ci.Macrouso_UMA_Des Is Not NULL) a ")
            stb.AppendLine(" OUTER APPLY (Select TOP 1 " + outerSelect + " FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = ric.Occupazione_Agea And ci.Destinazione_Cod = ric.Destinazione_Agea And ci.Qualita_Cod = ric.Qualita_Agea And ci.Uso_Cod = ric.Uso_Agea And ci.Macrouso_UMA_Cod Is Not NULL And ci.Macrouso_UMA_Des Is Not NULL) b ")
            stb.AppendLine(" OUTER APPLY (Select TOP 1 " + outerSelect + " FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = ric.Occupazione_Agea And ci.Destinazione_Cod = ric.Destinazione_Agea And ci.Macrouso_UMA_Cod Is Not NULL And ci.Macrouso_UMA_Des Is Not NULL) c ")
            stb.AppendLine(" OUTER APPLY (Select TOP 1 " + outerSelect + " FROM Codifica_SpecieVegetali_Agea_2015_2020 ci WHERE ci.Occupazione_Cod = ric.Occupazione_Agea And ci.Macrouso_UMA_Cod Is Not NULL And ci.Macrouso_UMA_Des Is Not NULL) d ) AS x ")

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" WHERE " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            stb.AppendLine(" ORDER BY macrouso_UMA_Cod ")

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

Public Class UMA_Richieste_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Aggiorna_Richieste(ByVal toInsert As ArrayList,
                                       ByVal toUpdate As ArrayList,
                                       ByVal toDelete As ArrayList,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_W.Aggiorna_Richieste()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each richiestaDel As UMA_Richieste In toDelete
                    GiasContext.UMA_Richieste.Attach(richiestaDel)
                    GiasContext.UMA_Richieste.Remove(richiestaDel)

                Next

                For Each richiestaIn As UMA_Richieste In toInsert

                    GiasContext.UMA_Richieste.Add(richiestaIn)

                Next

                For Each richiestaUp As UMA_Richieste In toUpdate

                    GiasContext.UMA_Richieste.Attach(richiestaUp)
                    GiasContext.Entry(richiestaUp).State = EntityState.Modified

                Next

                ' COMMIT Effettivo
                GiasContext.SaveChanges()


            End Using
        Catch e As DataException
            'lascio andare avanti
            Scrivi_LOG(objParametri, nomeRoutine, e.Message)
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato


    End Function

    Public Function Aggiorna_Richieste_Terzisti(ByVal toInsert As ArrayList,
                                                ByVal toUpdate As ArrayList,
                                                ByVal toDelete As ArrayList,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_W.Aggiorna_Richieste_Terzisti()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try


            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                For Each richiestaDel As UMA_Richieste In toDelete

                    GiasContext.UMA_Richieste.Attach(richiestaDel)
                    GiasContext.Entry(richiestaDel).State = EntityState.Deleted

                Next

                For Each richiestaIn As UMA_Richieste In toInsert

                    GiasContext.UMA_Richieste.Add(richiestaIn)

                Next

                For Each richiestaUp As UMA_Richieste In toUpdate

                    GiasContext.UMA_Richieste.Attach(richiestaUp)
                    GiasContext.Entry(richiestaUp).State = EntityState.Modified

                Next


                ' COMMIT Effettivo
                GiasContext.SaveChanges()


            End Using


        Catch ex As Exception
            If (ex.InnerException.InnerException.Message.Contains("PRIMARY KEY 'PK_UMA_Richieste'")) Then
                messaggioErrore = "Impossibile inserire una riga duplicata"
            Else
                messaggioErrore = ex.Message
            End If

            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
            risultato = -1
        End Try

        Return risultato


    End Function

    Public Function Nuova_Richiesta(ByVal Richiesta As UMA_Richieste,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreUmaDAL.UMA_Richieste_W.Nuova_Richiesta()"
        Dim messaggioErrore As String = ""
        Dim risultato As Integer = 0

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try


            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)


                GiasContext.UMA_Richieste.Add(Richiesta)

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

End Class
