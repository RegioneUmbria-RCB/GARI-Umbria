Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text

Public Class Cantina_RegistriTelematici
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiAnagrafiche(ByVal pivaSu As String, _
                                ByVal sa_cod As String, _
                                ByVal TipoAnagrafica As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByVal ChiamataDaGiasLan As Boolean, _
                                ByRef objParametri As AgronicaCoreParametri, _
                                ByVal codOper As String,
                                ByVal codIcqrf As String,
                                Optional ByVal id As String = "", _
                                Optional ByRef strSQLOutput As String = "",
                                Optional ByVal data As DateTime = AGRODATAINIZIO
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.LeggiAnagrafiche()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0

            Select Case TipoAnagrafica
                Case 0
                    Stb.Append(" ( " & vbCrLf)
                    Stb.Append(SoggettiContatti(pivaSu, sa_cod, TipoAnagrafica, xFiltroAggiuntivo, xOrderBy, ChiamataDaGiasLan, objParametri, codOper, codIcqrf, id, strSQLOutput))
                    Stb.Append(" ) UNION ( " & vbCrLf)
                    Stb.Append(SoggettiImprese(pivaSu, sa_cod, TipoAnagrafica, xFiltroAggiuntivo, xOrderBy, ChiamataDaGiasLan, objParametri, codOper, codIcqrf, id, strSQLOutput))
                    Stb.Append(" ) " & vbCrLf)
                    Stb.Append(" ORDER BY Cognome, Nome, Rag_soc, TipoSoggetto, CodiceCUAA ")

                Case 1
                    Stb.Append(" SELECT  " & vbCrLf)
                    Stb.Append(" ap.APPEZZA AS CodVigna, " & vbCrLf)
                    Stb.Append(" ap.APP_NOME AS Descrizione, " & vbCrLf)
                    Stb.Append(" (CASE WHEN regVig.GIAS_Stato IS NOT NULL THEN stati.WAnagraficaStati_Des ELSE 'Vigna non configurata per l''invio al SIAN' END) AS stato, " & vbCrLf)
                    Stb.Append(" '' as S, " & vbCrLf)
                    Stb.Append(" regVig.TipoRichiesta as tipoRichiesta, " & vbCrLf)
                    Stb.Append(" regVig.GIAS_Stato as IDStato " & vbCrLf)
                    Stb.Append(" FROM Appezzamento ap " & vbCrLf)
                    Stb.Append(" LEFT JOIN ws_regVino_Vigne regVig ON ap.APPEZZA = regVig.CodVigna " & vbCrLf)
                    Stb.Append(" LEFT JOIN WAnagraficaStati stati ON regVig.GIAS_Stato = stati.WAnagraficaStati_Cod" & vbCrLf)
                    Stb.Append(" WHERE 1=1 " & vbCrLf)
                    If id <> "" Then
                        Stb.Append("  AND ap.CodVigna=" & Agro_SQL_SaveText_NULL(id) & " " & vbCrLf)
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        Stb.Append("  AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " " & vbCrLf)
                    End If

                Case 2
                    Stb.Append("SELECT   " & vbCrLf)
                    Stb.Append("  ic.val_cod as CodOper,  " & vbCrLf)
                    Stb.Append("  REPLACE(REPLACE(REPLACE(cdc.val_cod,'/',''),'-',''),'\','') as CodIcqrf,  " & vbCrLf)
                    Stb.Append("  vas.Identificativo as CodVaso,   " & vbCrLf)
                    Stb.Append("   Materiale_Cod AS TipoVaso,    " & vbCrLf)
                    Stb.Append("   'Vasca Enologica ' + vas.Identificativo AS Descrizione,     " & vbCrLf)
                    Stb.Append("   (Capacita_Effettiva) AS Volume,  --In Gias è espressa in hl       " & vbCrLf)
                    Stb.Append("   (CASE WHEN regVasi.Gias_Stato is not null THEN stati.WAnagraficaStati_Des ELSE 'Vaso non configurato per l''invio al SIAN' END) as StatoSIAN,   " & vbCrLf)
                    Stb.Append("                         '' as S,   " & vbCrLf)
                    Stb.Append("   regVasi.CodVaso as ID,   " & vbCrLf)
                    Stb.Append("   regVasi.CodiceIcqrf as icqrf,   " & vbCrLf)
                    Stb.Append("   regVasi.TipoRichiesta as TipoRichiesta,  " & vbCrLf)
                    Stb.Append("   regVasi.GIAS_Stato as IDStato,  " & vbCrLf)
                    Stb.Append("   vas.sa_Cod as Sa_Cod,  " & vbCrLf)
                    Stb.Append("   vas.vas_cod as vas_cod  " & vbCrLf)
                    Stb.Append("   FROM   Cantina_Vasche vas   " & vbCrLf)
                    Stb.Append("   LEFT JOIN Centri_Aziendali_Codici cdc ON vas.piva=cdc.piva AND vas.sa_cod=cdc.sa_cod AND cdc.id_cod=1109  " & vbCrLf)
                    Stb.Append("   LEFT JOIN Imprese_Codici ic ON vas.piva=ic.piva AND ic.id_cod=1010  " & vbCrLf)
                    Stb.Append("   LEFT JOIN ws_RegVino_Vasi regVasi ON vas.Identificativo = regVasi.CodVaso AND regVasi.CodiceIcqrf = (SELECT val_cod FROM Centri_Aziendali_Codici WHERE id_cod = 1109 AND sa_cod=vas.Sa_Cod AND Piva=" & Agro_SQL_SaveText_NULL(pivaSu) & ")  " & vbCrLf)
                    Stb.Append("   LEFT JOIN WAnagraficaStati stati ON regVasi.Gias_Stato = stati.WAnagraficaStati_Cod  AND regVasi.CodiceIcqrf = REPLACE(REPLACE(REPLACE(cdc.val_cod,'/',''),'-',''),'\','') " & vbCrLf)
                    Stb.Append("   Where 1=1 ")
                    Stb.Append("  AND ic.val_cod = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)

                    If codIcqrf <> "" Then
                        Stb.Append("  AND REPLACE(REPLACE(REPLACE(cdc.val_cod,'/',''),'-',''),'\','')= " & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
                    End If

                    If id <> "" Then
                        Stb.Append("  AND vas.Identificativo=" & Agro_SQL_SaveText_NULL(id) & " " & vbCrLf)
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        Stb.Append("  AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " " & vbCrLf)
                    End If

                    If data <> AGRODATAINIZIO Then
                        Stb.Append("  AND vas.Validita_Inizio<= " & Agro_SQL_SaveDateTime(data) & " " & vbCrLf)
                        Stb.Append("  AND vas.Validita_Fine>= " & Agro_SQL_SaveDateTime(data) & " " & vbCrLf)
                    End If

                    Stb.Append("  ORDER BY vas.Identificativo ASC")

                Case 3
                    Stb.Append("SELECT p.CodOper,  " & vbCrLf)
                    Stb.Append("       p.CodIcqrf, " & vbCrLf)
                    Stb.Append("       p.TipoRichiesta, " & vbCrLf)
                    Stb.Append("       mp.Mat_Des, " & vbCrLf)
                    Stb.Append("       p.Lotto, " & vbCrLf)
                    Stb.Append("       p.CodPrimario, " & vbCrLf)
                    Stb.Append("       p.CodSecondario, " & vbCrLf)
                    Stb.Append("       (CASE WHEN p.Gias_Stato is not null THEN stati.WAnagraficaStati_Des ELSE '' END) as StatoSIAN, " & vbCrLf)
                    Stb.Append("       '' as S, " & vbCrLf)
                    Stb.Append("       p.GIAS_Stato AS IDStato, " & vbCrLf)
                    Stb.Append("       p.Mat_Cod, " & vbCrLf)
                    Stb.Append("       ISNULL(p.modificato, 0) as modificato " & vbCrLf)
                    Stb.Append(" FROM ws_RegVino_Prodotti p " & vbCrLf)
                    Stb.Append(" LEFT JOIN Materie_Prime mp ON p.Mat_Cod = mp.Mat_Cod  " & vbCrLf)
                    Stb.Append(" LEFT JOIN WAnagraficaStati stati ON p.GIAS_Stato = stati.WAnagraficaStati_Cod " & vbCrLf)
                    Stb.Append(" WHERE 1=1 " & vbCrLf)
                    Stb.Append(" AND p.CodOper= " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)

                    If codIcqrf <> "" Then
                        Stb.Append(" AND p.CodIcqrf= " & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        Stb.Append("  AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " " & vbCrLf)
                    End If

                    If id <> "" Then
                        Stb.Append("  AND p.Mat_Cod=" & Agro_SQL_SaveNum_NULL(CInt(id.Split("|")(0))) & " AND p.Lotto= " & Agro_SQL_SaveText_NULL(CStr(id.Split("|")(1))) & " " & vbCrLf)
                    End If

                Case Else

            End Select


            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '--------------------------------------------------------------------------


            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)



        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function SoggettiContatti(ByVal pivaSu As String, _
                                ByVal sa_cod As String, _
                                ByVal TipoAnagrafica As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByVal ChiamataDaGiasLan As Boolean, _
                                ByRef objParametri As AgronicaCoreParametri, _
                                ByVal codOper As String,
                                ByVal codIcqrf As String,
                                Optional ByVal id As String = "", _
                                Optional ByRef strSQLOutput As String = "") As String
        Dim Stb As New StringBuilder
        Stb.Length = 0
        Stb.Append("    SELECT DISTINCT " & vbCrLf)
        Stb.Append("    " & Agro_SQL_SaveText_NULL(codOper) & " as CodOper, " & vbCrLf)
        Stb.Append("    C.Cod_Contatto AS CodiceSoggetto,     " & vbCrLf)
        Stb.Append("    ISNULL((CASE WHEN C.Id_CF in (0,1) THEN C.Cod_Contatto ELSE NULL END),'') AS CodiceCUAA,     " & vbCrLf)
        Stb.Append("    (CASE WHEN C.Id_CF in (0,1) THEN 'IT' ELSE (CASE WHEN statiUE.codice is not null THEN 'UE' ELSE (CASE WHEN statiEX.codice is not null THEN 'EX' ELSE '' END) END) END ) AS TipoSoggetto,     -- COMPLETARE: in base allo sato può essere UE o EX --      " & vbCrLf)
        Stb.Append("    C.Nome AS Nome,     " & vbCrLf)
        Stb.Append("    C.Cognome AS Cognome,     " & vbCrLf)
        Stb.Append("    C.Rag_Soc,      " & vbCrLf)
        Stb.Append("    ISNULL((CASE WHEN Id_CF=0 AND (DATALENGTH(C.cod_contatto)<>11 AND (SUBSTRING(C.cod_contatto, 1,1) NOT IN ('8','9'))) THEN C.cod_contatto END),'') AS CUAA_PersonaFisica,     " & vbCrLf)
        Stb.Append("    ISNULL((CASE WHEN Id_CF=1 OR (DATALENGTH(C.cod_contatto)=11 AND (SUBSTRING(C.cod_contatto, 1,1) IN ('8','9'))) THEN C.cod_contatto END),'') AS CUAA_PersonaGiuridica,         " & vbCrLf)
        Stb.Append("    I.CAP AS IndirizzoSede_CAP,     " & vbCrLf)
        Stb.Append("    I.ind_des AS IndirizzoSede_Indirizzo,     " & vbCrLf)
        Stb.Append("    ISNULL(comuni.COMUNI_PROV,'') AS IndirizzoSede_Provincia,     " & vbCrLf)
        Stb.Append("    ISNULL(comuni.LOCALITA,'') AS IndirizzoSede_Comune,     " & vbCrLf)
        Stb.Append("    ISNULL((CASE WHEN I.stato = 'ITALIA' THEN 'IT' ELSE I.stato END),'') AS IndirizzoSede_Stato,    " & vbCrLf)
        Stb.Append("    (CASE WHEN regSog.GIAS_Stato is not null THEN anStati.WAnagraficaStati_Des ELSE 'Contatto non ancora inviato al SIAN' END) as Stato,    " & vbCrLf)
        Stb.Append("                         '' as S,    " & vbCrLf)
        Stb.Append("    ISNULL(regSog.CodiceSoggetto,'') as ID,    " & vbCrLf)
        Stb.Append("    ISNULL(regSog.CodOper,'') as CodOpers,    " & vbCrLf)
        Stb.Append("    ISNULL(regSog.TipoRichiesta,'') as TipoRichiesta,   " & vbCrLf)
        Stb.Append("    regSog.GIAS_Stato as IDStato,   " & vbCrLf)
        Stb.Append("    ISNULL(comuni.PROV,'') as IstatProv,   " & vbCrLf)
        Stb.Append("    ISNULL(comuni.COM,'') as IstatCom,   " & vbCrLf)
        Stb.Append("    C.id_CF ,  " & vbCrLf)
        Stb.Append("    ISNULL(statiUE.codice,'') as StatoUE,  " & vbCrLf)
        Stb.Append("    ISNULL(statiEX.codice,'') as StatoEX,  " & vbCrLf)
        Stb.Append("    ISNULL(statiEX.Codice_Numerico,'') as Codice_numerico_Stato,  " & vbCrLf)
        Stb.Append("    ISNULL(statiEX.Codice_Alpha_3,'') as Codice_Alpha_3_Stato  " & vbCrLf)
        Stb.Append("    FROM Contatti C     " & vbCrLf)
        Stb.Append("    LEFT JOIN ContattixIndirizzi CXI ON C.Piva=CXI.Piva AND C.Sa_cod=CXI.Sa_Cod AND C.cod_contatto=CXI.Cod_Contatto AND C.Tipo_Indirizzo_Default=CXI.Tipo_Indirizzo --AND CXI.Cod_Indirizzo IN (SELECT Max(ci1.Cod_Indirizzo) FROM ContattiXIndirizzi ci1 LEFT JOIN Indirizzi i1 ON ci1.Cod_Indirizzo = i1.cod_indirizzo WHERE ci1.Tipo_Indirizzo in (3,1,2,101) AND i1.ind_des<>'' AND ci1.cod_Contatto=c.cod_contatto)   " & vbCrLf)
        Stb.Append("    LEFT JOIN Indirizzi I ON (CXI.Cod_Indirizzo = I.Cod_Indirizzo)     " & vbCrLf)
        Stb.Append("    LEFT JOIN ws_RegVino_Soggetti regSog ON C.Cod_Contatto = regSog.CodiceSoggetto  " & vbCrLf)
        Stb.Append("    LEFT JOIN WAnagraficaStati anStati ON regSog.GIAS_Stato = anStati.WAnagraficaStati_Cod    " & vbCrLf)
        Stb.Append("    LEFT JOIN ISTAT comuni ON I.pro_cod_istat=comuni.PROV AND I.com_cod_istat = comuni.COM " & vbCrLf)
        Stb.Append("    LEFT JOIN [dbo].[ACCDAA_ANAG_T004_TabellaCodiciStatiMembri] statiUE ON I.stato = statiUE.Codice  " & vbCrLf)
        Stb.Append("    LEFT JOIN [dbo].[ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166] statiEX ON I.stato = statiEX.Codice  " & vbCrLf)
        Stb.Append("    LEFT JOIN Imprese_Codici ic ON ic.id_cod=1010 AND ic.PIVA = C.Piva " & vbCrLf)
        Stb.Append("    LEFT JOIN Risorse_Umane ru ON ru.piva = c.piva AND ru.cod_Contatto = c.cod_Contatto " & vbCrLf)
        Stb.Append("    LEFT JOIN Rapporti_Contabili clienti ON ru.Cod_Rapporto = clienti.Cod_Rapporto AND clienti.Cliente=1 " & vbCrLf)
        Stb.Append("    LEFT JOIN Rapporti_Contabili fornitori ON ru.Cod_Rapporto = fornitori.Cod_Rapporto AND fornitori.Fornitore =1 " & vbCrLf)
        Stb.Append("    WHERE (ic.val_cod=" & Agro_SQL_SaveText_NULL(codOper) & " OR c.sa_cod = -1) " & vbCrLf)
        Stb.Append("   AND SUBSTRING(C.Cod_Contatto,0,2) <> '-' " & vbCrLf)
        Stb.Append("   AND C.Cod_Contatto not in (SELECT Piva From Imprese) " & vbCrLf)


        If id <> "" Then
            Stb.Append("  AND C.Cod_Contatto=" & Agro_SQL_SaveText_NULL(id) & " " & vbCrLf)
        End If

        If xFiltroAggiuntivo <> "" Then
            Stb.Append("  AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " " & vbCrLf)
        End If
        Return Stb.ToString
    End Function

    Public Function SoggettiImprese(ByVal pivaSu As String, _
                                ByVal sa_cod As String, _
                                ByVal TipoAnagrafica As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByVal ChiamataDaGiasLan As Boolean, _
                                ByRef objParametri As AgronicaCoreParametri, _
                                ByVal codOper As String,
                                ByVal codIcqrf As String,
                                Optional ByVal id As String = "", _
                                Optional ByRef strSQLOutput As String = "") As String
        Dim Stb As New StringBuilder
        Stb.Length = 0

        Stb.Append("Select DISTINCT " & vbCrLf  )
        Stb.Append("     " & Agro_SQL_SaveText_NULL(codOper) & " as CodOper,  " & vbCrLf)
        Stb.Append("     C.Cod_Contatto AS CodiceSoggetto,      " & vbCrLf)
        Stb.Append("     ISNULL((CASE WHEN C.Id_CF in (0,1) THEN C.Cod_Contatto ELSE NULL END),'') AS CodiceCUAA,      " & vbCrLf)
        Stb.Append("     (CASE WHEN C.Id_CF in (0,1) THEN 'IT' ELSE (CASE WHEN statiUE.codice is not null THEN 'UE' ELSE (CASE WHEN statiEX.codice is not null THEN 'EX' ELSE '' END) END) END ) AS TipoSoggetto,     -- COMPLETARE: in base allo sato può essere UE o EX --       " & vbCrLf)
        Stb.Append("     C.Nome AS Nome,      " & vbCrLf)
        Stb.Append("     C.Cognome AS Cognome,      " & vbCrLf)
        Stb.Append("     C.Rag_Soc,       " & vbCrLf)
        Stb.Append("     ISNULL((CASE WHEN Id_CF=0 AND (DATALENGTH(C.cod_contatto)<>11 AND (SUBSTRING(C.cod_contatto, 1,1) NOT IN ('8','9'))) THEN C.cod_contatto END),'') AS CUAA_PersonaFisica,     " & vbCrLf)
        Stb.Append("     ISNULL((CASE WHEN Id_CF=1 OR (DATALENGTH(C.cod_contatto)=11 AND (SUBSTRING(C.cod_contatto, 1,1) IN ('8','9'))) THEN C.cod_contatto END),'') AS CUAA_PersonaGiuridica,       " & vbCrLf)
        Stb.Append("     I.CAP AS IndirizzoSede_CAP,      " & vbCrLf)
        Stb.Append("     I.ind_des AS IndirizzoSede_Indirizzo,      " & vbCrLf)
        Stb.Append("     ISNULL(comuni.COMUNI_Prov,'') AS IndirizzoSede_Provincia,      " & vbCrLf)
        Stb.Append("     ISNULL(comuni.LOCALITA,'') AS IndirizzoSede_Comune,      " & vbCrLf)
        Stb.Append("     ISNULL((CASE WHEN I.stato = 'ITALIA' THEN 'IT' ELSE I.stato END),'') AS IndirizzoSede_Stato,     " & vbCrLf)
        Stb.Append("     (CASE WHEN regSog.GIAS_Stato is not null THEN anStati.WAnagraficaStati_Des ELSE 'Contatto non ancora inviato al SIAN' END) as Stato,     " & vbCrLf)
        Stb.Append("             '' as S,     " & vbCrLf)
        Stb.Append("     ISNULL(regSog.CodiceSoggetto,'') as ID,     " & vbCrLf)
        Stb.Append("     ISNULL(regSog.CodOper,'') as CodOpers,     " & vbCrLf)
        Stb.Append("     ISNULL(regSog.TipoRichiesta,'') as TipoRichiesta,    " & vbCrLf)
        Stb.Append("     regSog.GIAS_Stato as IDStato,    " & vbCrLf)
        Stb.Append("     ISNULL(comuni.PROV,'') as IstatProv,    " & vbCrLf)
        Stb.Append("     ISNULL(comuni.COM,'') as IstatCom,    " & vbCrLf)
        Stb.Append("     C.id_CF ,   " & vbCrLf)
        Stb.Append("     ISNULL(statiUE.codice,'') as StatoUE,   " & vbCrLf)
        Stb.Append("     ISNULL(statiEX.codice,'') as StatoEX,   " & vbCrLf)
        Stb.Append("     ISNULL(statiEX.Codice_Numerico,'') as Codice_numerico_Stato,   " & vbCrLf)
        Stb.Append("     ISNULL(statiEX.Codice_Alpha_3,'') as Codice_Alpha_3_Stato   " & vbCrLf)
        Stb.Append("     FROM Imprese iii " & vbCrLf)
        Stb.Append("    LEFT JOIN Contatti C  ON iii.PIVA = C.Cod_Contatto  " & vbCrLf)
        Stb.Append("     LEFT JOIN ImpresexIndirizzi CXI ON C.Cod_Contatto=CXI.Piva --AND C.Tipo_Indirizzo_Default=CXI.Tipo_Indirizzo --AND CXI.Cod_Indirizzo IN (SELECT Max(ci1.Cod_Indirizzo) FROM ImpresexIndirizzi ci1 LEFT JOIN Indirizzi i1 ON ci1.Cod_Indirizzo = i1.cod_indirizzo WHERE ci1.Tipo_Indirizzo in (3,1,2,101) AND i1.ind_des<>'' AND ci1.piva=iii.piva)    " & vbCrLf)
        Stb.Append("     LEFT JOIN Indirizzi I ON CXI.Cod_Indirizzo = I.Cod_Indirizzo " & vbCrLf)
        Stb.Append("     LEFT JOIN ws_RegVino_Soggetti regSog ON C.Cod_Contatto = regSog.CodiceSoggetto   " & vbCrLf)
        Stb.Append("     LEFT JOIN WAnagraficaStati anStati ON regSog.GIAS_Stato = anStati.WAnagraficaStati_Cod     " & vbCrLf)
        Stb.Append("     LEFT JOIN ISTAT comuni ON I.pro_cod_istat=comuni.PROV AND I.com_cod_istat = comuni.COM " & vbCrLf)
        Stb.Append("     LEFT JOIN [dbo].[ACCDAA_ANAG_T004_TabellaCodiciStatiMembri] statiUE ON I.stato = statiUE.Codice   " & vbCrLf)
        Stb.Append("     LEFT JOIN [dbo].[ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166] statiEX ON I.stato = statiEX.Codice   " & vbCrLf)
        Stb.Append("     LEFT JOIN Imprese_Codici ic ON ic.id_cod=1010 AND ic.PIVA = C.Piva      " & vbCrLf)
        Stb.Append("    LEFT JOIN Risorse_Umane ru ON ru.piva = c.piva AND ru.cod_Contatto = c.cod_Contatto  " & vbCrLf)
        Stb.Append("     LEFT JOIN Rapporti_Contabili clienti ON ru.Cod_Rapporto = clienti.Cod_Rapporto AND clienti.Cliente=1  " & vbCrLf)
        Stb.Append("     LEFT JOIN Rapporti_Contabili fornitori ON ru.Cod_Rapporto = fornitori.Cod_Rapporto AND fornitori.Fornitore =1     " & vbCrLf)
        Stb.Append("    WHERE (ic.val_cod=" & Agro_SQL_SaveText_NULL(codOper) & " OR c.sa_cod = -1) " & vbCrLf)
        Stb.Append("    AND SUBSTRING(C.Cod_Contatto,0,2) <> '-' ")

        If id <> "" Then
            Stb.Append("  AND C.Cod_Contatto=" & Agro_SQL_SaveText_NULL(id) & " " & vbCrLf)
            End If

        If xFiltroAggiuntivo <> "" Then
            Stb.Append("  AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " " & vbCrLf)
        End If
        Return Stb.ToString
    End Function

    Public Function LeggiOperazioni(ByVal Piva As String,
                            ByVal icqrf As String,
                            ByVal TipoOperazione As String,
                            ByVal dataInizio As DateTime,
                            ByVal dataFine As DateTime,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri,
                            Optional ByRef strSQLOutput As String = "",
                            Optional ByVal ws_RegVino_Operazione_cod As Integer = 0) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.LeggiOperazioni()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Length = 0
            Stb.Append(" SELECT  " & vbCrLf)
            Stb.Append(" op.CodOper,  " & vbCrLf)
            Stb.Append(" op.CodiceIcqrf,  " & vbCrLf)
            Stb.Append(" op.CodiceOperazione,  " & vbCrLf)
            Stb.Append(" op.TipoRichiesta,  " & vbCrLf)
            Stb.Append(" op.NumOperazione,  " & vbCrLf)
            Stb.Append(" op.DataOperazione,  " & vbCrLf)
            Stb.Append(" a.des_lib,  " & vbCrLf)
            Stb.Append(" s.WAnagraficaStati_Des, " & vbCrLf)
            Stb.Append(" '' as S, " & vbCrLf)
            Stb.Append(" op.ws_RegVino_Operazione_cod as ID, " & vbCrLf)
            Stb.Append(" op.GIAS_Stato as IDStato, " & vbCrLf)
            Stb.Append(" op.ID_Agenda as ID_Agenda, " & vbCrLf)
            Stb.Append(" a.Data_Creazione " & vbCrLf)
            Stb.Append(" FROM Ws_RegVino_Operazioni op " & vbCrLf)
            Stb.Append(" LEFT JOIN WAnagraficaStati s ON op.GIAS_Stato = s.[WAnagraficaStati_Cod] " & vbCrLf)
            Stb.Append(" LEFT JOIN Agenda a ON op.Id_Agenda = a.Id_Agenda " & vbCrLf)
            Stb.Append(" WHERE 1=1" & vbCrLf)


            Stb.Append("AND op.DataOperazione>=" & Agro_SQL_SaveDateTime(dataInizio) & " AND op.DataOperazione<=" & Agro_SQL_SaveDateTime(dataFine) & " " & vbCrLf)

            Select Case TipoOperazione

                Case "nGIIN"
                    Stb.Append("AND op.CodiceOperazione <> 'GIIN' " & vbCrLf)
                Case ""

                Case Else
                    Stb.Append("AND op.CodiceOperazione = " & Agro_SQL_SaveText_NULL(TipoOperazione) & " " & vbCrLf)
            End Select

            If xFiltroAggiuntivo <> "" Then
                Stb.Append("  AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " " & vbCrLf)
            End If

            If Piva <> "" Then
                Stb.Append("AND op.CodOper=" & Agro_SQL_SaveText_NULL(Piva) & " " & vbCrLf)
            End If

            If icqrf <> "" Then
                Stb.Append("AND op.CodiceIcqrf=" & Agro_SQL_SaveText_NULL(icqrf) & " " & vbCrLf)
            End If

            If ws_RegVino_Operazione_cod <> 0 Then
                Stb.Append("AND op.ws_RegVino_Operazione_cod=" & Agro_SQL_SaveNum(ws_RegVino_Operazione_cod) & " " & vbCrLf)
            End If

            Stb.Append("AND op.GIAS_Stato > -1 " & vbCrLf)

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                Stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                Stb.Append(" ORDER BY op.DataOperazione, NumOperazione, Id_Agenda ASC " & vbCrLf)
            End If


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Function LeggiWs_RegVino_Prodotti(objParametri As AgronicaCoreParametri, id As String, codOper As String, codIcqrf As String) As Object
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.LeggiWs_RegVino_Operazioni()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Append("Select [CodOper] " & vbCrLf)
            Stb.Append("       ,[CodOper_Fisiche_Giuridiche] " & vbCrLf)
            Stb.Append("       ,[CodIcqrf] " & vbCrLf)
            Stb.Append("       ,[TipoRichiesta] " & vbCrLf)
            Stb.Append("       ,[Mat_Cod] " & vbCrLf)
            Stb.Append("       ,[Lotto] " & vbCrLf)
            Stb.Append("       ,[CodCategoria] " & vbCrLf)
            Stb.Append("       ,[AttoCert] " & vbCrLf)
            Stb.Append("       ,[CodClassificazione] " & vbCrLf)
            Stb.Append("       ,[CodDopIgp] " & vbCrLf)
            Stb.Append("       ,[CodEbacchus] " & vbCrLf)
            Stb.Append("       ,[OrigineUve] " & vbCrLf)
            Stb.Append("       ,[Provenienza] " & vbCrLf)
            Stb.Append("       ,[PaesiProvenienza] " & vbCrLf)
            Stb.Append("       ,[CodZonaViticola] " & vbCrLf)
            Stb.Append("       ,[Varieta] " & vbCrLf)
            Stb.Append("       ,[AltreVarieta] " & vbCrLf)
            Stb.Append("       ,[CodSottozona] " & vbCrLf)
            Stb.Append("       ,[CodVigna] " & vbCrLf)
            Stb.Append("       ,[CodColore] " & vbCrLf)
            Stb.Append("       ,[Menzioni] " & vbCrLf)
            Stb.Append("       ,[Biologico] " & vbCrLf)
            Stb.Append("       ,[PraticheEnologiche] " & vbCrLf)
            Stb.Append("       ,[CodPartita] " & vbCrLf)
            Stb.Append("       ,[Annata] " & vbCrLf)
            Stb.Append("       ,[MassaVolumica] " & vbCrLf)
            Stb.Append("       ,[CodStatoFisico] " & vbCrLf)
            Stb.Append("       ,[DataCertDOP] " & vbCrLf)
            Stb.Append("       ,[NumCertDOP] " & vbCrLf)
            Stb.Append("       ,[GIAS_Stato] " & vbCrLf)
            Stb.Append("       ,[modificato] " & vbCrLf)
            Stb.Append("   FROM [dbo].[ws_RegVino_Prodotti]")
            Stb.Append("   WHERE 1=1 " & vbCrLf)
            If id <> "" Then
                Stb.Append("   AND Mat_Cod=" & Agro_SQL_SaveNum_NULL(CInt(id.Split("|")(0))) & " " & vbCrLf)
                Stb.Append("   AND Lotto=" & Agro_SQL_SaveText_NULL(id.Split("|")(1)) & " " & vbCrLf)
            End If

            If codOper <> "" Then
                Stb.Append("   AND CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            End If

            If codIcqrf <> "" Then
                Stb.Append("   AND CodIcqrf=" & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function InserisciAggiornaWs_RegVino_Prodotti(objParametri As AgronicaCoreParametri, id As String, pivasu As String, codIcqrf As String, nuovaRichiesta As String, Stato As Integer)
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.InserisciAggiornaWs_RegVino_Prodotti()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim ret As Boolean = False
        Try

            Stb.Append("UPDATE ws_RegVino_Prodotti " & vbCrLf)
            Stb.Append(" SET " & vbCrLf)
            Stb.Append(" TipoRichiesta = " & Agro_SQL_SaveText_NULL(nuovaRichiesta) & ", " & vbCrLf)
            Stb.Append(" GIAS_Stato = " & Agro_SQL_SaveNum_NULL(Stato) & " " & vbCrLf)
            Stb.Append("   WHERE Mat_Cod=" & Agro_SQL_SaveNum_NULL(CInt(id.Split("|")(0))) & " " & vbCrLf)
            Stb.Append("   AND Lotto=" & Agro_SQL_SaveText_NULL(id.Split("|")(1)) & " " & vbCrLf)
            Stb.Append("   AND CodOper=" & Agro_SQL_SaveText_NULL(pivasu) & " " & vbCrLf)
            Stb.Append("   AND CodIcqrf=" & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            ret = EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return ret
    End Function

    Sub EliminaWs_RegVino_Prodotti(objParametri As AgronicaCoreParametri, codOper As String, codIcqrf As String, id As String)
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.LeggiWs_RegVino_Operazioni()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Append("UPDATE [dbo].[ws_RegVino_Prodotti] " & vbCrLf)
            Stb.Append(" SET GIAS_Stato = " & Agro_SQL_SaveNum_NULL(18000001) & ",  " & vbCrLf)
            Stb.Append("    TipoRichiesta = 'E' " & vbCrLf)
            Stb.Append("   WHERE 1=1 " & vbCrLf)
            If id <> "" Then
                Stb.Append("   AND Mat_Cod=" & Agro_SQL_SaveNum_NULL(CInt(id.Split("|")(0))) & " " & vbCrLf)
                Stb.Append("   AND Lotto=" & Agro_SQL_SaveText_NULL(id.Split("|")(1)) & " " & vbCrLf)
            End If

            If codOper <> "" Then
                Stb.Append("   AND CodOper=" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
            End If

            If codIcqrf <> "" Then
                Stb.Append("   AND CodIcqrf=" & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            EseguiQuery_Scrittura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub

    Function getCodOperFromPivaSu(pivaSu As String, objParametri As AgronicaCoreParametri) As String
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.getCodOperFromPivaSu()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Append("Select Val_cod " & vbCrLf)
            Stb.Append("   FROM Imprese_Codici " & vbCrLf)
            Stb.Append("   WHERE id_Cod=1010 " & vbCrLf)
            Stb.Append("   AND PIVA=" & Agro_SQL_SaveText_NULL(pivaSu) & " " & vbCrLf)




            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                Return CStr(DT.Rows(0).Item("val_Cod"))
            Else
                Return Nothing
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function

    Function getCodIcqrfFromSaCod(pivaSu As String, saCod As Integer, objParametri As AgronicaCoreParametri) As String
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.getCodOperFromPivaSu()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Append("Select Val_cod " & vbCrLf)
            Stb.Append("   FROM Centri_Aziendali_Codici ")
            Stb.Append("   WHERE id_Cod=1109 " & vbCrLf)
            Stb.Append("   AND PIVA=" & Agro_SQL_SaveText_NULL(pivaSu) & " " & vbCrLf)
            Stb.Append("   AND SA_COD=" & Agro_SQL_SaveNum_NULL(saCod) & " " & vbCrLf)




            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                Dim codIcqrf = CStr(DT.Rows(0).Item("val_Cod"))
                codIcqrf = codIcqrf.Replace("*", "")
                codIcqrf = codIcqrf.Replace("-", "")
                codIcqrf = codIcqrf.Replace("/", "")
                codIcqrf = codIcqrf.Replace("\", "")
                codIcqrf = codIcqrf.Replace("_", "")
                Return codIcqrf
            Else
                Return Nothing
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function

    Function getPivaFromCodOper(codOper As String, objParametri As AgronicaCoreParametri) As String
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.getPivaFromCodOper()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Append("Select PIVA " & vbCrLf)
            Stb.Append("   FROM Imprese_Codici " & vbCrLf)
            Stb.Append("   WHERE id_Cod=1010 " & vbCrLf)
            Stb.Append("   AND val_Cod=" & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)




            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                Return CStr(DT.Rows(0).Item("val_Cod"))
            Else
                Return Nothing
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function

    Function getErrore(codOper As String, codIcqrf As String, id As String, tipo As Integer, objParametri As AgronicaCoreParametri) As String
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.getErrore()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Stb.Length = 0
            Select Case tipo
                Case 1 'Vasi
                    Stb.Append("SELECT e.CodiceErrore, e.DescrizioneErrore  " & vbCrLf)
                    Stb.Append(" FROM [dbo].[ws_RegVino_Vasi] v " & vbCrLf)
                    Stb.Append(" INNER JOIN [dbo].[ws_RegVino_LogInvio_DettaglioVasi] dv ON dv.CodOper = v.CodOper AND dv.ws_RegVino_CodiceIcqrf = v.CodiceIcqrf AND dv.ws_RegVino_CodVaso = v.CodVaso  " & vbCrLf)
                    Stb.Append(" INNER JOIN [dbo].[ws_RegVino_LogInvio] i ON dv.ws_RegVino_LogInvio_Cod = i.ws_RegVino_LogInvio_Cod  " & vbCrLf)
                    Stb.Append(" INNER JOIN [dbo].[ws_RegVino_LogInvio_Dettaglio_Errori] e ON i.ws_RegVino_LogInvio_Cod = e.ws_RegVino_LogInvio_Cod  " & vbCrLf)
                    Stb.Append(" WHERE v.CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
                    Stb.Append(" AND v.CodiceIcqrf = " & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
                    Stb.Append(" AND v.CodVaso= " & Agro_SQL_SaveText_NULL(id) & " " & vbCrLf)
                    Stb.Append(" ORDER BY i.ws_RegVino_LogInvio_Cod DESC")

                Case 2 'Vigne

                Case 3 'Soggetti
                    Stb.Append("SELECT e.CodiceErrore , e.DescrizioneErrore  " & vbCrLf)
                    Stb.Append(" FROM [dbo].[ws_RegVino_Soggetti] s " & vbCrLf)
                    Stb.Append(" INNER JOIN [dbo].[ws_RegVino_LogInvio_DettaglioSoggetti] ds ON s.CodOper = ds.CodOper AND s.CodiceSoggetto = ds.ws_RegVino_Soggetto_Cod  " & vbCrLf)
                    Stb.Append(" INNER JOIN [dbo].[ws_RegVino_LogInvio] i ON ds.ws_RegVino_LogInvio_Cod = i.ws_RegVino_LogInvio_Cod  " & vbCrLf)
                    Stb.Append(" INNER JOIN [dbo].[ws_RegVino_LogInvio_Dettaglio_Errori] e ON i.ws_RegVino_LogInvio_Cod = e.ws_RegVino_LogInvio_Cod  " & vbCrLf)
                    Stb.Append(" WHERE s.CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
                    Stb.Append(" AND s.CodiceSoggetto=" & Agro_SQL_SaveText_NULL(id) & " " & vbCrLf)
                    Stb.Append(" ORDER BY i.ws_RegVino_LogInvio_Cod DESC")

                Case 4 'Operazioni
                    Stb.Append("SELECT e.CodiceErrore , e.DescrizioneErrore  " & vbCrLf)
                    Stb.Append(" FROM [dbo].[ws_RegVino_Operazioni] o " & vbCrLf)
                    Stb.Append(" INNER JOIN [dbo].[ws_RegVino_LogInvio_Dettaglio] do ON o.ws_RegVino_Operazione_cod = do.ws_RegVino_Operazione_cod " & vbCrLf)
                    Stb.Append(" INNER JOIN [dbo].[ws_RegVino_LogInvio] i ON do.ws_RegVino_LogInvio_Cod = i.ws_RegVino_LogInvio_Cod " & vbCrLf)
                    Stb.Append(" INNER JOIN [dbo].[ws_RegVino_LogInvio_Dettaglio_Errori] e ON i.ws_RegVino_LogInvio_Cod = e.ws_RegVino_LogInvio_Cod " & vbCrLf)
                    Stb.Append(" WHERE o.ws_RegVino_Operazione_cod = " & Agro_SQL_SaveNum(id) & " " & vbCrLf)
                    Stb.Append(" ANd do.ws_RegVino_LogInvio_Cod = (Select MAX(ws_RegVino_LogInvio_Cod) FROM [dbo].[ws_RegVino_LogInvio_Dettaglio] WHERE ws_RegVino_Operazione_cod= " & Agro_SQL_SaveNum(id) & ") " & vbCrLf)
                    Stb.Append(" ORDER BY i.ws_RegVino_LogInvio_Cod DESC")

                Case 5 'Prodotti
                    Stb.Append("SELECT e.CodiceErrore , e.DescrizioneErrore   " & vbCrLf)
                    Stb.Append(" FROM [dbo].[ws_RegVino_Prodotti] p " & vbCrLf)
                    Stb.Append(" INNER JOIN [dbo].[ws_RegVino_LogInvio_DettaglioProdotti] dp ON p.CodOper = dp.CodOper AND p.CodIcqrf = dp.CodIcqrf AND p.Mat_Cod = dp.Mat_Cod AND p.Lotto = dp.Lotto  " & vbCrLf)
                    Stb.Append(" INNER JOIN [dbo].[ws_RegVino_LogInvio] i ON dp.ws_RegVino_LogInvio_Cod = i.ws_RegVino_LogInvio_Cod  " & vbCrLf)
                    Stb.Append(" INNER JOIN [dbo].[ws_RegVino_LogInvio_Dettaglio_Errori] e ON i.ws_RegVino_LogInvio_Cod = e.ws_RegVino_LogInvio_Cod  " & vbCrLf)
                    Stb.Append(" WHERE p.CodOper = " & Agro_SQL_SaveText_NULL(codOper) & " " & vbCrLf)
                    Stb.Append(" AND p.CodIcqrf = " & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)
                    Stb.Append(" AND p.Mat_Cod = " & Agro_SQL_SaveNum(id.Split("|")(0)) & " " & vbCrLf)
                    Stb.Append(" AND p.Lotto = " & Agro_SQL_SaveText_NULL(id.Split("|")(1)) & " " & vbCrLf)
                    Stb.Append(" ORDER BY i.ws_RegVino_LogInvio_Cod desc")

                Case Else
                    Return ""
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                Dim returnStr As String = ""
                For Each row As DataRow In DT.Rows
                    returnStr &= CStr(row.Item("CodiceErrore")) & " - " & CStr(row.Item("DescrizioneErrore")) & vbCrLf
                Next
                Return returnStr
            Else
                Return Nothing
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function

    Function getErroreSingolo(codOper As String, codIcqrf As String, id As String, tipo As Integer, objParametri As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.getErrore()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable = Nothing

        Try
            Stb.Length = 0
            Select Case tipo
                Case 1 'Vasi
                    Stb.AppendLine("Select TOP 1 CodiceErrore, DescrizioneErrore  ")
                    Stb.AppendLine(" From [dbo].[ws_RegVino_LogInvio_Dettaglio_Errori] ")
                    Stb.AppendLine(" Where codOper = " & Agro_SQL_SaveText_NULL(codOper) & " ")
                    Stb.AppendLine(" And [ws_RegVino_CodVaso] = " & Agro_SQL_SaveText_NULL(id) & " ")
                    Stb.AppendLine(" And [ws_RegVino_CodiceIcqrf] = " & Agro_SQL_SaveText_NULL(codIcqrf) & " ")
                    Stb.AppendLine(" Order By ws_RegVino_LogInvio_Cod DESC")

                Case 2 'Vigne
                    Return DT
                Case 3 'Soggetti
                    Stb.AppendLine("Select TOP 1 CodiceErrore, DescrizioneErrore  ")
                    Stb.AppendLine(" From [dbo].[ws_RegVino_LogInvio_Dettaglio_Errori] ")
                    Stb.AppendLine(" Where codOper = " & Agro_SQL_SaveText_NULL(codOper) & " ")
                    Stb.AppendLine(" And [ws_RegVino_Soggetto_Cod] = " & Agro_SQL_SaveText_NULL(id) & " ")
                    Stb.AppendLine(" Order By ws_RegVino_LogInvio_Cod DESC")

                Case 4 'Operazioni
                    Stb.AppendLine("Select TOP 1 CodiceErrore, DescrizioneErrore  ")
                    Stb.AppendLine(" From [dbo].[ws_RegVino_LogInvio_Dettaglio_Errori] ")
                    Stb.AppendLine(" Where codOper = " & Agro_SQL_SaveText_NULL(codOper) & " ")
                    Stb.AppendLine(" And [ws_regVino_Operazione_Cod] = " & Agro_SQL_SaveNum(id) & " ")
                    Stb.AppendLine(" Order By ws_RegVino_LogInvio_Cod DESC")

                Case 5 'Prodotti
                    Return DT
                Case Else
                    Return DT
            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            'If DT.Rows.Count > 0 Then
            '    Dim returnStr As String = ""
            '    For Each row As DataRow In DT.Rows
            '        returnStr += CStr(row.Item("CodiceErrore")) + " - " + CStr(row.Item("DescrizioneErrore")) + vbCrLf
            '    Next
            '    Return returnStr
            'Else
            '    Return Nothing
            'End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Function getSaCodFromCodIcqrf(piva As String, codIcqrf As String, objParametri As AgronicaCoreParametri) As String
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Cantina_RegistriTelematici.getSaCodFromCodIcqrf()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Stb.Append("Select SA_COD " & vbCrLf)
            Stb.Append("   FROM Centri_Aziendali_Codici ")
            Stb.Append("   WHERE id_Cod=1109 " & vbCrLf)
            Stb.Append("   AND PIVA=" & Agro_SQL_SaveText_NULL(piva) & " " & vbCrLf)
            Stb.Append("   AND val_cod=" & Agro_SQL_SaveText_NULL(codIcqrf) & " " & vbCrLf)




            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If DT.Rows.Count > 0 Then
                Dim saCod = CStr(DT.Rows(0).Item("Sa_Cod"))
                Return saCod
            Else
                Return Nothing
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
    End Function

End Class
