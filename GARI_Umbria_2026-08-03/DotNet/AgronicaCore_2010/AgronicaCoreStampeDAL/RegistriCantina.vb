Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Text
Imports System.Data.Common
Imports AgronicaCoreDataProvider


'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'#################         REGISTRO DI VINIFICAZIONE       #########################
'#################       REGISTRO DI COMMERCIALIZZAZIONE       #####################
'#################         REGISTRO DI IMBOTTIGLIAMENTO       ######################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################
'###################################################################################


Public Class RegistriCantina
    Inherits AgronicaCoreDataProvider.DataProvider


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' brogliaccio/diario dei movimenti
    ''' nato come copia-incolla da vinificazione + commercializzazione
    ''' con queste modifiche:
    ''' - la data torna a essere la data_movimento (non la data_spedizione)
    ''' - descr operazione senza data
    ''' - designazione diventa azienda - linea
    ''' - desc prodotto
    ''' - vasca
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    'Lista_CodRisUm non usato
    Public Function BrogliaccioMovimenti(ByVal Piva As String,
                                            ByVal DataReportInizio As Date,
                                            ByVal DataReportFine As Date,
                                            ByVal Cod_Contatto_Terzi As String,
                                            ByVal Lista_CodRisUm As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Id_Destinazione As Integer,
                                            ByVal Cal_Cod As Integer,
                                            ByVal Mat_Cod As Integer,
                                            ByVal Linea_Cod As Integer,
                                            ByVal Cau_Mov As String,
                                            ByVal Gestione_Conto_Terzi As enum_RegistroContoTerzi,
                                            ByVal str_idagenda_filtrocategoria As String,
                                            ByVal str_matcod_filtrocategoria As String,
                                            ByVal str_lotto_filtrocategoria As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable
        ') As Boolean

        ' ByVal Opt_Gestione_RegistroVinificazione As Integer, _
        'ByVal DataInizio As Date, _
        '                     ByVal DataFine As Date, _

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.RegistriCantina.BrogliaccioMovimenti"

        Dim MessaggioErrore As String = ""
        Dim stbQ As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim DataFineControllo As Date
            DataFineControllo = DateAdd(DateInterval.Day, 1, DataReportFine)


            '=========================================================
            '--------------- 1) PREPARAZIONI (QUERY CHE LEGGE LE LINEE) ------------------
            '=========================================================
            stbQ.Append("  " & vbCrLf)
            stbQ.Append(" -- PARTE 1: OPERAZIONI SULLE LINEE " & vbCrLf)
            stbQ.Append("  " & vbCrLf)

            stbQ.Append(" SELECT Agenda.Id_Agenda, Movimenti.Data_Movimento, Movimenti.Ora AS DataOra, convert(char(8), movimenti.ora, 108) as ora, '' AS NDoc, agenda.lav_cod, lav_des, Agenda.des_lib, Linee_Preparazioni.Preparazione_Des, " & vbCrLf)

            Select Case Gestione_Conto_Terzi
                Case enum_RegistroContoTerzi.Nessuno,
                   enum_RegistroContoTerzi.RegistroGlobale,
                         enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    stbQ.Append(" '' AS Azienda, " & vbCrLf)
                Case enum_RegistroContoTerzi.RegistroUnicoDiversificato,
                     enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    '2= registro unico ma diversificato per c/lav (MODELLO RUGGERI)
                    '3 = reg c/terzi separato
                    stbQ.AppendLine(" (ISNULL( ( SELECT TOP 1 Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome  ")
                    stbQ.AppendLine("       FROM Contatti ")
                    stbQ.AppendLine("       WHERE Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto ")
                    stbQ.AppendLine("       AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    stbQ.AppendLine("       AND Linee_Produzioni.Cod_Contatto_Terzi <> '' ")
                    stbQ.AppendLine("       ), '') ")
                    stbQ.AppendLine(" ) AS Azienda, ")
                    stbQ.AppendLine(" ")
            End Select
            stbQ.AppendLine(" ISNULL(Linee_Produzioni.Linea_Des,'') AS Linea_Des,  ISNULL(Trasformazione_Des,'') AS Lotto_Linea,  ")

            'stbQ.AppendLine(" Linee_PreparazionixReport.StrCampo_Registri, ")

            stbQ.AppendLine(" ISNULL( ( SELECT TOP 1 StrCampo_Registri ")
            stbQ.AppendLine("           FROM Linee_PreparazionixReport LPR  ")
            stbQ.AppendLine("           WHERE LPR.Piva = Linee_Preparazioni.Piva  ")
            stbQ.AppendLine("           AND LPR.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod  ")
            stbQ.AppendLine("           AND StrCampo_Registri <>'' ")
            stbQ.AppendLine("           AND Id_Report <> " & Agro_SQL_SaveNum(enum_AgroReportistica.PreparazioniBio) & " ")
            stbQ.AppendLine("           ) ,'' )  AS StrCampo_Registri,   ")

            'stbQ.AppendLine(" CASE WHEN  Linee_PreparazionixReport.StrCampo_Registri <>'' THEN ")
            stbQ.AppendLine("   ISNULL( ")
            stbQ.AppendLine("       ( SELECT TOP 1 Descrizione ")
            stbQ.AppendLine("       FROM MovimentiXReport MR ")
            stbQ.AppendLine("       WHERE MR.piva=agenda.piva ")
            stbQ.AppendLine("       AND  MR.id_agenda=agenda.id_agenda ")
            stbQ.AppendLine("       AND Descrizione <>'' ")
            stbQ.AppendLine("       AND Id_Report <> " & Agro_SQL_SaveNum(enum_AgroReportistica.PreparazioniBio) & " ")
            stbQ.AppendLine("       ) ,'' ) ")
            'stbQ.AppendLine("  ELSE '' END AS desc_agg,  ")
            stbQ.AppendLine("       AS desc_agg,   ")

            stbQ.AppendLine(" Movimenti.Cau_Mov, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Lotto,  ")
            stbQ.AppendLine(" Movimenti_dettagli.Udm_Cod, UnitaMisura.Udm_Sim, ")
            stbQ.AppendLine(" Movimenti_dettagli.qta, Materie_Prime.Mat_Des, ")
            stbQ.AppendLine(" Materie_Prime.Cod_Articolo, ")
            stbQ.AppendLine(" Materie_Prime.Peso_Set,   ")

            stbQ.AppendLine(" CASE WHEN Materie_Prime.Peso_Set > 0 ")
            stbQ.AppendLine(" THEN  'effettiva'  ")
            stbQ.AppendLine(" ELSE  'nominale' ")
            stbQ.AppendLine(" END  AS capacita,  ")

            stbQ.AppendLine(" Movimenti_dettagli.udm_cod_extra, UM_Extra.udm_Sim AS Udm_Sim_Extra, Movimenti_dettagli.QTA_EXTRA,  ")

            stbQ.AppendLine(" Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Sa_Cod AS SaCod_Dest, Mov_Destinazioni.Id_Destinazione AS Id_Dest ")
            stbQ.AppendLine(" , Linee_Preparazioni.Tipo_Integrazione, sa_nome, ISNULL(Cantina_Vasche.Identificativo, '') AS Num_Vasca, ISNULL(Fabbricati.Fabbricato_Des, '') AS Magazzino,  ")

            stbQ.AppendLine(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=2 THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS CaricoKg, ")
            stbQ.AppendLine(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=2 THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS ScaricoKg, ")

            stbQ.AppendLine(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=29 THEN SUM(Movimenti_dettagli.Qta)  ")
            stbQ.AppendLine(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti_dettagli.Udm_Cod<>2 AND Materie_Prime.Peso_Set>0 THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) ")
            stbQ.AppendLine(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti_dettagli.Udm_Cod<>2 AND Materie_Prime.Peso_Set=0 THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) ")
            stbQ.AppendLine(" ELSE 0 END AS CaricoLt,    ")

            stbQ.AppendLine(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=29 THEN SUM(Movimenti_dettagli.Qta)  ")
            stbQ.AppendLine(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti_dettagli.Udm_Cod<>2 AND Materie_Prime.Peso_Set>0  THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) ")
            stbQ.AppendLine(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti_dettagli.Udm_Cod<>2 AND Materie_Prime.Peso_Set=0  THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) ")
            stbQ.AppendLine(" ELSE 0 END AS ScaricoLt   ")

            stbQ.Append(" FROM Linee_Preparazioni " & vbCrLf)
            ' stbQ.Append(" FROM Linee_PreparazionixReport " + vbCrLf)
            'stbQ.Append(" INNER JOIN Linee_Preparazioni ON Linee_PreparazionixReport.Piva = Linee_Preparazioni.Piva AND Linee_PreparazionixReport.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod  " + vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Linee_Preparazioni.Preparazione_Cod = Agenda.PREPARAZIONE_COD AND Linee_Preparazioni.Piva = Agenda.PIVA " & vbCrLf)
            stbQ.Append(" INNER JOIN Trasformazioni ON Trasformazioni.ID_trasformazione=Agenda.ID_Trasformazione AND  Trasformazioni.PIVA=Agenda.PIVA " & vbCrLf)
            stbQ.Append(" INNER JOIN Operazioni ON Agenda.Lav_Cod = operazioni.lav_cod  " & vbCrLf)

            stbQ.Append(" INNER JOIN Linee_Produzioni ON Agenda.Piva = Linee_Produzioni.Piva AND Agenda.Linea_Cod = Linee_Produzioni.Linea_Cod " & vbCrLf)

            stbQ.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod  " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov  " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod AND Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_dettagli.Udm_Cod " & vbCrLf)
            stbQ.Append(" LEFT OUTER JOIN UnitaMisura UM_Extra ON UM_Extra.Udm_Cod = Movimenti_dettagli.Udm_Cod_Extra " & vbCrLf)

            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)
            stbQ.Append(" INNER JOIN Centri_Aziendali  ON Mov_Destinazioni.PIVA = Centri_Aziendali.PIVA AND Mov_Destinazioni.Sa_Cod = Centri_Aziendali.Sa_Cod " & vbCrLf)
            stbQ.Append(" LEFT OUTER JOIN Cantina_Vasche ON Mov_Destinazioni.PIVA = Cantina_Vasche.PIVA AND Mov_Destinazioni.Sa_Cod = Cantina_Vasche.Sa_Cod AND Mov_Destinazioni.Id_Destinazione = Cantina_Vasche.Vas_Cod AND Mov_Destinazioni.Tipo_Destinazione = 13 " & vbCrLf)
            stbQ.Append(" LEFT OUTER JOIN Fabbricati  ON Mov_Destinazioni.PIVA = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.Sa_Cod AND Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod AND Mov_Destinazioni.Tipo_Destinazione = 20 " & vbCrLf)

            ''nota del 17/06/2014: siam d'accordo con Marco di non introdurre il join su AND Materie_Prime_ParametriQualitativi.PIVA = Materie_Prime.piva
            ''non ci devono essere record duplicati per lo stesso mat_cod -> introduciamo una query di delete di sicurezza
            'stbQ.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod  " + vbCrLf)
            'stbQ.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " + vbCrLf)

            stbQ.Append(" WHERE 1= 1 " & vbCrLf)
            ''11/07/2017: brogliaccio
            ''stbQ.Append(" WHERE Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " + vbCrLf)
            'stbQ.Append(" WHERE Linee_PreparazionixReport.Id_Report IN ( " & Agro_SQL_SaveNum(enum_AgroReportistica.Vinificazione_DOC) & ", " & Agro_SQL_SaveNum(enum_AgroReportistica.Commercializzazione) & " )  " + vbCrLf)
            stbQ.Append(" AND Agenda.Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_PREPARAZIONE) & vbCrLf)

            stbQ.Append(" AND Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                                      "'" & CAU_SCARICO & "', " &
                                                      "'" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', " &
                                                      "'" & CAU_CONFERIMENTO & "', " &
                                                      "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                                            ") " & vbCrLf)

            stbQ.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If str_idagenda_filtrocategoria <> "" Then
                stbQ.Append(" AND Agenda.Id_Agenda IN " & Agro_SQL_Save_Clausola_IN(str_idagenda_filtrocategoria) & " " & vbCrLf)
            End If
            If str_matcod_filtrocategoria <> "" Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod IN " & Agro_SQL_Save_Clausola_IN(str_matcod_filtrocategoria) & " " & vbCrLf)
            End If
            If str_lotto_filtrocategoria <> "" Then
                'non ci va Agro_SQL_SaveText() perché se no mette il doppio apice
                'Ma Agro_SQL_save_Clausola_IN risolve il problema
                stbQ.Append(" AND Movimenti_Dettagli.Lotto IN " & Agro_SQL_Save_Clausola_IN(str_lotto_filtrocategoria, True) & " " & vbCrLf)
            End If

            '--------------
            stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " " & vbCrLf)
            stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " " & vbCrLf)
            'stbQ.Append(" AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " " + vbCrLf)

            '--------------
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            'stbQ.Append(" AND Materie_Prime_Calibri.Piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")
            'stbQ.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = 'calibro' ")
            'stbQ.Append(" AND ( Materie_Prime_ParametriQualitativi.ChkRegistri_Vinificazione = 1 OR Materie_Prime_ParametriQualitativi.ChkRegistri = 1 ) ")

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Id_Destinazione <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " & vbCrLf)
            End If
            If Cal_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime_ParametriQualitativi.tipo_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If
            If Linea_Cod <> 0 Then
                stbQ.Append(" AND Linee_Produzioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "  " & vbCrLf)
            End If
            If Cau_Mov <> "" Then
                stbQ.Append(" AND Movimenti.CAU_MOV = '" & Agro_SQL_SaveText(Cau_Mov) & "'" & vbCrLf)
            End If
            'If Cod_Contatto_Terzi <> "" Then
            '    'modifica del 21/01/2013: separato il filtro per la parte agenda e la parte linee
            '    stbQ.Append(RegistroVinificazione_FiltroSQL_ContoTerzi_ParteLinee(Piva, Cod_Contatto_Terzi))
            'End If

            stbQ.Append(" AND NOT EXISTS (SELECT 1 FROM  Linee_Preparazioni_Report_Esclusi LPRE " & vbCrLf)
            stbQ.Append("                 INNER JOIN Linee_Preparazioni_Dettagli LPD ON LPRE.Piva = LPD.Piva AND LPRE.Preparazione_Cod = LPD.Preparazione_Cod AND LPRE.Dettaglio_Cod = LPD.Dettaglio_Cod " & vbCrLf)
            stbQ.Append("                 WHERE LPRE.Piva = Linee_Preparazioni.Piva " & vbCrLf)
            stbQ.Append("                 AND LPRE.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " & vbCrLf)
            '09/08/17 COMMENTATO X BROGLIACCIO
            stbQ.Append("                 -- AND LPRE.Id_Report = Linee_PreparazionixReport.Id_Report  " & vbCrLf)
            stbQ.Append("                 AND LPD.CAU_MOV = Movimenti.CAU_MOV " & vbCrLf)
            stbQ.Append("                 AND LPD.Elem_Cod = Movimenti_dettagli.Elem_Cod " & vbCrLf)
            'modifica del 05/10/2012: se la preparazione passaggio da registro di vinificazione a registro di commercializzazione
            'utilizzava 'risorse indefinita' come ingrediente e/o preparato
            'il join con Movimenti_dettagli non produceva alcun record e quindi venivano visualizzati entrambi i movimenti:
            'sia lo scarico dal reg vinificazione sia il carico al reg di commerc. e la qta si azzerava
            'INTRODOTTO L'OR per LPD.Pro_Cod - LPD.Mat_Cod - LPD.Udm_Cod 
            stbQ.Append("               AND ( " & vbCrLf)
            stbQ.Append("                       ( " & vbCrLf)
            stbQ.Append("                       LPD.Pro_Cod = Movimenti_dettagli.Pro_Cod " & vbCrLf)
            stbQ.Append("                       AND LPD.Mat_Cod = Movimenti_dettagli.Mat_Cod " & vbCrLf)
            stbQ.Append("                       AND LPD.Udm_Cod = Movimenti_dettagli.Udm_Cod   " & vbCrLf)
            stbQ.Append("                       ) OR ( " & vbCrLf)
            stbQ.Append("                       LPD.Pro_Cod = 0 AND LPD.Mat_Cod = 0 AND LPD.Udm_Cod = 0  " & vbCrLf)
            stbQ.Append("                       ) " & vbCrLf)
            stbQ.Append("                   ) " & vbCrLf)
            stbQ.Append("               ) " & vbCrLf)

            'Modifica del 02/03/2022: è stato commentato perché la rigenerazione nel GiasLan non scrive più in queste tabelle, aggiunto il filtro sul modulo generazione
            'stbQ.Append(" AND EXISTS (SELECT 1  " & vbCrLf)
            'stbQ.Append(" 			FROM Linee_PreparazionixReport " & vbCrLf)
            'stbQ.Append("			WHERE  Linee_PreparazionixReport.Piva = Linee_Preparazioni.Piva      " & vbCrLf)
            'stbQ.Append(" 			AND Linee_PreparazionixReport.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod" & vbCrLf)
            'stbQ.Append("           AND Linee_PreparazionixReport.Id_Report <> " & Agro_SQL_SaveNum(enum_AgroReportistica.PreparazioniBio) & ") " & vbCrLf)

            stbQ.Append("           AND Linee_Produzioni.modulo_generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & vbCrLf)

            ''FILTRO PER GESTIONE DELLA VINIFICAZIONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            'stbQ.Append(RegistroVinificazione_FiltroSQL_Importante(objParametri, Lista_PrepCod, Lista_IdTrasf_NoComm, Piva))

            'MODIFICA DEL 18/10/2013: lettura del campo ora (visto che si verifica quello nel controllo del passaggio a registro)
            'stbQ.Append(" GROUP BY Agenda.Id_Agenda, Movimenti.Data_Movimento, Agenda.des_lib, Linee_Preparazioni.Preparazione_Des, Linee_Produzioni.Linea_Des, " + vbCrLf)
            stbQ.Append(" GROUP BY Agenda.Id_Agenda, agenda.lav_cod, Movimenti.Ora, Movimenti.Data_Movimento, Agenda.des_lib, Linee_Preparazioni.Preparazione_Des, Linee_Produzioni.Linea_Des, " & vbCrLf)
            stbQ.Append(" Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Lotto, Trasformazione_Des, " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Udm_Cod, UnitaMisura.Udm_Sim, Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo, " & vbCrLf)
            ', Linee_PreparazionixReport.StrCampo_Registri,
            stbQ.Append(" Linee_Preparazioni.Piva , Linee_Preparazioni.Preparazione_Cod, " & vbCrLf)
            stbQ.Append(" Materie_Prime.Peso_Set, Movimenti_dettagli.QTA_EXTRA, Movimenti_dettagli.udm_cod_extra, " & vbCrLf)
            stbQ.Append(" Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Sa_Cod, sa_nome, Mov_Destinazioni.Id_Destinazione, Linee_Preparazioni.Tipo_Integrazione , Movimenti_dettagli.Qta, UM_Extra.udm_Sim, Cantina_Vasche.Identificativo, Fabbricati.Fabbricato_Des, lav_des, Agenda.PIVA " & vbCrLf)

            'MODIFICA DEL 03/06/2014: gestito flag conto terzi, in un caso aggiunge Cod_Contatto_Terzi
            Select Case Gestione_Conto_Terzi
                Case enum_RegistroContoTerzi.Nessuno,
                   enum_RegistroContoTerzi.RegistroGlobale
                    '0 = se non c'è esiste conto terzi
                    '1 = se esiste, ma è stato richiesto registro unico

                Case enum_RegistroContoTerzi.RegistroUnicoDiversificato,
                          enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    '2= registro unico ma diversificato per c/lav (MODELLO RUGGERI)
                    stbQ.Append(", Linee_Produzioni.Cod_Contatto_Terzi ")
            End Select

            stbQ.Append("  " & vbCrLf)
            stbQ.Append("  " & vbCrLf)
            stbQ.Append(" UNION ALL " & vbCrLf)
            stbQ.Append("  " & vbCrLf)
            stbQ.Append("  " & vbCrLf)

            '=========================================================
            '--------------- 2) AGENDA  (OPERAZIONI FUORI DALLA GESTIONE LINEE) ------------------
            '=========================================================
            stbQ.Append("  " & vbCrLf)
            stbQ.Append(" -- PARTE 2: OPERAZIONI DI AGENDA " & vbCrLf)
            stbQ.Append("  " & vbCrLf)

            stbQ.Append(" SELECT Agenda.Id_Agenda, Movimenti.Data_Movimento, Movimenti.Ora AS DataOra, convert(char(8), movimenti.ora, 108) as ora, " & vbCrLf)

            'MODIFICA DEL 18/06/2014, per gestire numerazione particolare dei ddt ricevuti conferimento
            'stbQ.Append(" (SELECT Doc_Numero_Sin + CAST(Doc_Numero AS Varchar(100)) + doc_numero_des FROM Movimenti MovContabili WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda and cau_mov='4000') AS NDoc " + vbCrLf)
            stbQ.Append(" ISNULL( ( CASE WHEN agenda.lav_cod IN (" & LAVCOD_ACCETTAZIONE_DIVERSI & ") " & vbCrLf)
            stbQ.Append(" THEN (SELECT Doc_Numero_Sin + right('00000' + CAST(Doc_Numero AS Varchar(100)),5) + doc_numero_des FROM Movimenti MovContabili WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda and cau_mov='" & CAU_REGISTRAZIONI & "') " & vbCrLf)
            stbQ.Append(" ELSE (SELECT Doc_Numero_Sin + CAST(Doc_Numero AS Varchar(100)) + doc_numero_des FROM Movimenti MovContabili WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda and cau_mov='" & CAU_REGISTRAZIONI & "') " & vbCrLf)
            stbQ.Append(" END), '') AS NDoc, agenda.lav_cod, lav_des, " & vbCrLf)

            stbQ.Append(" Agenda.des_lib, ' ' AS Preparazione_des, -- c'è volutamente lo spazio, per poter fare il filtro sulla griglia " & vbCrLf)

            'MODIFICA DEL 03/06/2014
            Select Case Gestione_Conto_Terzi
                Case enum_RegistroContoTerzi.Nessuno,
                   enum_RegistroContoTerzi.RegistroGlobale
                    '0 = se non c'è esiste conto terzi
                    '1 = se esiste, ma è stato richiesto registro unico
                    stbQ.Append("  '' AS Azienda,  " & vbCrLf)
                Case enum_RegistroContoTerzi.RegistroUnicoDiversificato,
                     enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    '2= registro unico ma diversificato per c/lav (MODELLO RUGGERI)
                    stbQ.Append("   ------------------------------------------------- " & vbCrLf)
                    stbQ.Append("   ----------- AZIENDA C/TERZI----------- " & vbCrLf)
                    stbQ.Append("   ------------------------------------------------- " & vbCrLf)
                    stbQ.Append("   ISNULL ( " & vbCrLf)
                    stbQ.Append("   ( " & vbCrLf)
                    stbQ.Append("  " & vbCrLf)
                    stbQ.Append("  " & vbCrLf)

                    'nuova select del 23/03/2016:
                    'ottimizzata + per i prodotti che non sono nè uva nè altre materie prime
                    'non gestisce diversamente le operazioni di carico da quelle di scarico
                    'ma fa la stessa cosa, partendo dal lotto del prodotto risale alla linea del c/lavoro

                    stbQ.Append("   ----------- caso UVE ----------- " & vbCrLf)
                    stbQ.Append("   CASE WHEN (select distinct TOP 1 codice_generazione " & vbCrLf)
                    stbQ.Append("               from OGenerazioni_Anagrafe_Log " & vbCrLf)
                    stbQ.Append("               where OGenerazioni_Anagrafe_Log.elem_cod=movimenti_dettagli.elem_cod  " & vbCrLf)
                    stbQ.Append("               and OGenerazioni_Anagrafe_Log.mat_cod= movimenti_dettagli.mat_cod " & vbCrLf)
                    stbQ.Append("               and tipo_generazione = 4 " & vbCrLf)
                    stbQ.Append("               ) in (50,131,165) THEN " & vbCrLf)
                    stbQ.Append("               --se RACCOLTA, CARICO, SCARICO  " & vbCrLf)
                    stbQ.Append("               CASE  WHEN Agenda.lav_cod IN (125,1022,1023)   " & vbCrLf)
                    stbQ.Append("                   THEN" & vbCrLf)
                    stbQ.Append("                   ( " & vbCrLf)
                    stbQ.Append("                   SELECT Imprese.Rag_Soc  AS Designazione " & vbCrLf)
                    stbQ.Append("                   FROM Imprese " & vbCrLf)
                    stbQ.Append("                   WHERE agenda.piva = Imprese.piva   " & vbCrLf)
                    stbQ.Append("                   ) " & vbCrLf)
                    stbQ.Append("               ELSE  -- lav_cod " & vbCrLf)
                    'per tutte le altre operazioni (lav_cod), trattiamo diversamente in base al movimento di carico o scarico
                    stbQ.Append("               (   CASE  WHEN Movimenti.cau_mov = '" & CAU_CARICO & "' " & vbCrLf)
                    stbQ.Append("                       THEN   " & vbCrLf)
                    '                       nel caso di carico l'ingresso avviene con un documento contabile (ddt, doco, mvv
                    stbQ.Append("                       ( SELECT TOP 1 ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS Designazione " & vbCrLf)
                    stbQ.Append("                           FROM Contatti " & vbCrLf)
                    stbQ.Append("                           INNER JOIN Risorse_Umane ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto " & vbCrLf)
                    stbQ.Append("                           INNER JOIN Movimenti MovContabili ON MovContabili.cod_Risum = Risorse_Umane.Cod_Risum " & vbCrLf)
                    stbQ.Append("                           WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda " & vbCrLf)
                    stbQ.Append("                           AND Movimenti.piva = MovContabili.piva  " & vbCrLf)
                    stbQ.Append("                           AND cau_mov='" & CStr(CAU_REGISTRAZIONI) & "' " & vbCrLf)
                    stbQ.Append("                           AND EXISTS (SELECT 1   " & vbCrLf)
                    stbQ.Append("                                       FROM Linee_Produzioni LPCT " & vbCrLf)
                    stbQ.Append("                                       WHERE LPCT.cod_contatto_terzi = Contatti.Cod_Contatto " & vbCrLf)
                    stbQ.Append("                                       ) " & vbCrLf)
                    stbQ.Append("                       ) --select mov_contabile " & vbCrLf)
                    stbQ.Append("                   ELSE " & vbCrLf)
                    stbQ.Append("                       ( SELECT TOP 1 ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS Designazione " & vbCrLf)
                    stbQ.Append("                           FROM Contatti " & vbCrLf)
                    stbQ.Append("                           INNER JOIN Linee_Produzioni ON Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto " & vbCrLf)
                    stbQ.Append("                           AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
                    stbQ.Append("                           INNER JOIN  Trasformazioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva " & vbCrLf)
                    stbQ.Append("                           WHERE Linee_Produzioni.Cod_Contatto_Terzi <> '' " & vbCrLf)
                    stbQ.Append("                           AND Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " & vbCrLf)
                    stbQ.Append("                       ) -- SELECT SU LOTTO " & vbCrLf)
                    stbQ.Append("                   END -- case cau_mov " & vbCrLf)
                    stbQ.Append("               ) -- else del lav_cod" & vbCrLf)
                    stbQ.Append("               END -- case lav_cod " & vbCrLf)
                    stbQ.Append("  " & vbCrLf)
                    '10/08/2017: aggiunto case sui confezionati, visto che questa parte era solo sul registro di vinificazione
                    stbQ.Append("   ----------- caso CONFEZIONATI ----------- " & vbCrLf)
                    stbQ.Append("   WHEN EXISTS (select   1  " & vbCrLf)
                    stbQ.Append("               from OGenerazioni_Anagrafe_Log  " & vbCrLf)
                    stbQ.Append("               where OGenerazioni_Anagrafe_Log.elem_cod=movimenti_dettagli.elem_cod " & vbCrLf)
                    stbQ.Append("               and OGenerazioni_Anagrafe_Log.mat_cod= movimenti_dettagli.mat_cod " & vbCrLf)
                    stbQ.Append("               and tipo_generazione in (" & enum_Omni_Tipo_Generazione.ProdottoFinito & ", " & enum_Omni_Tipo_Generazione.Condizionati & ", " & enum_Omni_Tipo_Generazione.BagInBox & ")  " & vbCrLf)
                    stbQ.Append("               )  THEN	 " & vbCrLf)
                    stbQ.Append("                   ( " & vbCrLf)
                    stbQ.Append("                       SELECT TOP 1 ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS azienda " & vbCrLf)
                    stbQ.Append("                           FROM Contatti " & vbCrLf)
                    stbQ.Append("                           INNER JOIN Risorse_Umane ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto " & vbCrLf)
                    stbQ.Append("                           INNER JOIN Movimenti MovContabili ON MovContabili.cod_Risum = Risorse_Umane.Cod_Risum " & vbCrLf)
                    stbQ.Append("                           WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda " & vbCrLf)
                    stbQ.Append("                           AND Movimenti.piva = MovContabili.piva  " & vbCrLf)
                    stbQ.Append("                           AND cau_mov='" & CStr(CAU_REGISTRAZIONI) & "' " & vbCrLf)
                    stbQ.Append("                           AND EXISTS (SELECT 1   " & vbCrLf)
                    stbQ.Append("                                       FROM Linee_Produzioni LPCT " & vbCrLf)
                    stbQ.Append("                                       WHERE LPCT.cod_contatto_terzi = Contatti.Cod_Contatto " & vbCrLf)
                    stbQ.Append("                                       ) " & vbCrLf)
                    stbQ.Append("                   ) -- FINE CONFEZIONATI  " & vbCrLf)
                    stbQ.Append("    " & vbCrLf)
                    stbQ.Append("   ----------- caso ALTRE MATERIE PRIME ----------- " & vbCrLf)
                    stbQ.Append("   WHEN EXISTS (select   1  " & vbCrLf)
                    stbQ.Append("               from OGenerazioni_Anagrafe_Log  " & vbCrLf)
                    stbQ.Append("               where OGenerazioni_Anagrafe_Log.elem_cod=movimenti_dettagli.elem_cod " & vbCrLf)
                    stbQ.Append("               and OGenerazioni_Anagrafe_Log.mat_cod= movimenti_dettagli.mat_cod " & vbCrLf)
                    stbQ.Append("               and tipo_generazione =9 " & vbCrLf)
                    stbQ.Append("               )  THEN	 " & vbCrLf)
                    stbQ.Append("                   ( " & vbCrLf)
                    stbQ.Append("                   SELECT TOP 1 rag_soc " & vbCrLf)
                    stbQ.Append("                   FROM Materie_PrimexLotto_Proprieta, Contatti " & vbCrLf)
                    stbQ.Append("                   where piva_superuser = '" & objParametri.PivaSuperUser & "' " & vbCrLf)
                    stbQ.Append("                   and Materie_PrimexLotto_Proprieta.piva = Agenda.PIVA " & vbCrLf)
                    stbQ.Append("                   and id_proprieta = " & CStr(enum_TipoProprieta_Lotto.Categoria_Omni) & "  " & vbCrLf)
                    stbQ.Append("                   and lotto_val1 = movimenti_dettagli.lotto " & vbCrLf)
                    stbQ.Append("                   and elem_cod = movimenti_dettagli.elem_cod  " & vbCrLf)
                    stbQ.Append("                   and mat_cod = movimenti_dettagli.mat_cod " & vbCrLf)
                    '                                   non serve al momento filtrare lotto_cod1 (si imposta solo nome/lotto)
                    'stbQ.Append("              and lotto_cod1 = 16 " & vbCrLf)
                    stbQ.Append("                   and proprieta_val LIKE '%' + Contatti.cod_contatto + '%' " & vbCrLf)
                    stbQ.Append("                   ) -- FINE ALTRE MATERIE PRIME  " & vbCrLf)
                    stbQ.Append("    " & vbCrLf)
                    stbQ.Append(" ELSE   " & vbCrLf)
                    stbQ.Append("  ------- TUTTO IL RESTO ---------  " & vbCrLf)
                    stbQ.Append("           ( SELECT TOP 1 ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS azienda " & vbCrLf)
                    stbQ.Append("               FROM Contatti " & vbCrLf)
                    stbQ.Append("               INNER JOIN Linee_Produzioni ON Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto " & vbCrLf)
                    stbQ.Append("               AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
                    stbQ.Append("               INNER JOIN  Trasformazioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva " & vbCrLf)
                    stbQ.Append("               WHERE Linee_Produzioni.Cod_Contatto_Terzi <> '' " & vbCrLf)
                    stbQ.Append("               AND Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " & vbCrLf)
                    stbQ.Append("           ) -- SELECT SU LOTTO " & vbCrLf)
                    stbQ.Append(" END -- case codice generazione  " & vbCrLf)

                    stbQ.Append("   ) -- contenuto isnull " & vbCrLf)
                    stbQ.Append("   , '') -- isnull   " & vbCrLf)
                    stbQ.Append("   AS Azienda,  " & vbCrLf)
                    stbQ.Append("   ----- FINE AZIENDA C/TERZI----------- " & vbCrLf)
            End Select
            'stbQ.Append(" '' AS Linea_Des,    " + vbCrLf)
            stbQ.Append(" ISNULL(  " & vbCrLf)

            stbQ.Append("   ( SELECT TOP 1 linea_des FROM " & vbCrLf)
            stbQ.Append("       (  " & vbCrLf)
            stbQ.Append("         SELECT TOP 1 linea_des " & vbCrLf)
            stbQ.Append("           FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
            stbQ.Append("           INNER JOIN Linee_Produzioni ON OGenerazioni_Anagrafe_Log.piva=Linee_Produzioni.piva " & vbCrLf)
            stbQ.Append("           AND OGenerazioni_Anagrafe_Log.linea_cod=Linee_Produzioni.linea_cod " & vbCrLf)
            stbQ.Append("           WHERE OGenerazioni_Anagrafe_Log.mat_cod = movimenti_dettagli.mat_cod " & vbCrLf)
            stbQ.Append("           AND OGenerazioni_Anagrafe_Log.elem_cod = movimenti_dettagli.elem_cod " & vbCrLf)
            stbQ.Append("           AND tipo_generazione in (" & enum_Omni_Tipo_Generazione.ProdottoFinito & ", " & enum_Omni_Tipo_Generazione.Condizionati & ", " & enum_Omni_Tipo_Generazione.BagInBox & ") " & vbCrLf)
            'Modifica del 02/03/2022: Aggiunto il filtro sul modulo generazione
            stbQ.Append("           AND Linee_Produzioni.modulo_generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & vbCrLf)
            stbQ.Append("       UNION ALL " & vbCrLf)
            stbQ.Append("           SELECT TOP 1 linea_des" & vbCrLf)
            stbQ.Append("           from trasformazioni " & vbCrLf)
            stbQ.Append("           INNER JOIN Linee_Produzioni ON trasformazioni.piva=Linee_Produzioni.piva " & vbCrLf)
            stbQ.Append("           AND trasformazioni.linea_cod=Linee_Produzioni.linea_cod " & vbCrLf)
            stbQ.Append("           where trasformazione_des = movimenti_dettagli.lotto " & vbCrLf)
            stbQ.Append("       ) AS tabella  " & vbCrLf)
            stbQ.Append("   ) " & vbCrLf)

            stbQ.Append("  , '' ) AS Linea_Des, " & vbCrLf)

            stbQ.Append(" '' AS Lotto_Linea,  " & vbCrLf)
            stbQ.Append(" '' StrCampo_Registri, '' AS desc_agg,  " & vbCrLf)
            stbQ.Append(" Movimenti.Cau_Mov,  " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Lotto,  " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Udm_Cod, UnitaMisura.Udm_Sim, " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.qta, Materie_Prime.Mat_Des, " & vbCrLf)
            stbQ.Append(" Materie_Prime.Cod_Articolo, " & vbCrLf)
            stbQ.Append(" Materie_Prime.Peso_Set,  " & vbCrLf)

            stbQ.Append(" CASE WHEN Materie_Prime.Peso_Set > 0 " & vbCrLf)
            stbQ.Append(" THEN  'effettiva'  " & vbCrLf)
            stbQ.Append(" ELSE  'nominale' " & vbCrLf)
            stbQ.Append(" END  AS capacita,  " & vbCrLf)

            stbQ.Append(" Movimenti_dettagli.udm_cod_extra, UM_Extra.udm_Sim AS Udm_Sim_Extra, Movimenti_dettagli.QTA_EXTRA,  " & vbCrLf)

            stbQ.Append(" Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Sa_Cod AS SaCod_Dest, Mov_Destinazioni.Id_Destinazione AS Id_Dest, " & vbCrLf)
            stbQ.Append(" 0 AS Tipo_Integrazione, sa_nome, ISNULL(Cantina_Vasche.Identificativo, '') AS Num_Vasca, ISNULL(Fabbricati.Fabbricato_Des, '') AS Magazzino,  " & vbCrLf)

            stbQ.Append(" CASE WHEN CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=2 THEN Movimenti_dettagli.Qta ELSE 0 END AS CaricoKg, " & vbCrLf)
            stbQ.Append(" CASE WHEN CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=2 THEN Movimenti_dettagli.Qta ELSE 0 END AS ScaricoKg, " & vbCrLf)

            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=29 THEN Movimenti_dettagli.Qta  " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod NOT IN (29,2) AND Materie_Prime.Peso_Set>0 THEN Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod NOT IN (29,2) AND Materie_Prime.Peso_Set=0 THEN Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra " & vbCrLf)
            stbQ.Append(" ELSE 0 END AS CaricoLt,    " & vbCrLf)

            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=29 THEN Movimenti_dettagli.Qta  " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod NOT IN (29,2) AND Materie_Prime.Peso_Set>0  THEN Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod NOT IN (29,2) AND Materie_Prime.Peso_Set=0  THEN Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra " & vbCrLf)
            stbQ.Append(" ELSE 0 END AS ScaricoLt   " & vbCrLf)

            'stbQ.Append(" FROM  Materie_PrimexReport " + vbCrLf)
            'stbQ.Append(" INNER JOIN Materie_Prime ON Materie_PrimexReport.Mat_Cod = Materie_Prime.Mat_Cod AND Materie_PrimexReport.Piva = Materie_Prime.Piva  " + vbCrLf)
            stbQ.Append("  FROM  Materie_Prime " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND Movimenti.Id_Agenda = Agenda.Id_Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Operazioni ON Agenda.Lav_Cod = operazioni.lav_cod  " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD " & vbCrLf)
            stbQ.Append(" LEFT OUTER JOIN UnitaMisura UM_Extra ON UM_Extra.Udm_Cod = Movimenti_dettagli.Udm_Cod_Extra " & vbCrLf)

            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)
            stbQ.Append(" INNER JOIN Centri_Aziendali  ON Mov_Destinazioni.PIVA = Centri_Aziendali.PIVA AND Mov_Destinazioni.Sa_Cod = Centri_Aziendali.Sa_Cod " & vbCrLf)
            stbQ.Append(" LEFT OUTER JOIN Cantina_Vasche  ON Mov_Destinazioni.PIVA = Cantina_Vasche.PIVA AND Mov_Destinazioni.Sa_Cod = Cantina_Vasche.Sa_Cod AND Mov_Destinazioni.Id_Destinazione = Cantina_Vasche.Vas_Cod AND Mov_Destinazioni.Tipo_Destinazione = 13 " & vbCrLf)
            stbQ.Append(" LEFT OUTER JOIN Fabbricati  ON Mov_Destinazioni.PIVA = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.Sa_Cod AND Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod AND Mov_Destinazioni.Tipo_Destinazione = 20 " & vbCrLf)

            ''nota del 17/06/2014: siam d'accordo con Marco di non introdurre il join su AND Materie_Prime_ParametriQualitativi.PIVA = Materie_Prime.piva
            ''non ci devono essere record duplicati per lo stesso mat_cod -> introduciamo una query di delete di sicurezza
            'stbQ.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod  " + vbCrLf)
            'stbQ.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " + vbCrLf)

            stbQ.Append(" WHERE Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                                        "'" & CAU_SCARICO & "', " &
                                                        "'" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', " &
                                                        "'" & CAU_CONFERIMENTO & "', " &
                                                        "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                                        ") " & vbCrLf)

            stbQ.Append(" AND Agenda.Lav_Cod <> " & Agro_SQL_SaveNum(LAVCOD_PREPARAZIONE) & vbCrLf)
            stbQ.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If str_idagenda_filtrocategoria <> "" Then
                stbQ.Append(" AND Agenda.Id_Agenda IN " & Agro_SQL_Save_Clausola_IN(str_idagenda_filtrocategoria) & " " & vbCrLf)
            End If
            If str_matcod_filtrocategoria <> "" Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod IN " & Agro_SQL_Save_Clausola_IN(str_matcod_filtrocategoria) & " " & vbCrLf)
            End If
            If str_lotto_filtrocategoria <> "" Then
                stbQ.Append(" AND Movimenti_Dettagli.Lotto IN " & Agro_SQL_Save_Clausola_IN(str_lotto_filtrocategoria, True) & " " & vbCrLf)
            End If

            '--------------
            stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " " & vbCrLf)
            stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " " & vbCrLf)
            'stbQ.Append(" AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " " + vbCrLf)

            ''stbQ.Append(" AND Materie_PrimexReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " + vbCrLf)
            ''11/07/2017: brogliaccio
            'stbQ.Append(" AND Materie_PrimexReport.Id_Report IN ( " & Agro_SQL_SaveNum(enum_AgroReportistica.Vinificazione_DOC) & ", " & Agro_SQL_SaveNum(enum_AgroReportistica.Commercializzazione) & " )  " + vbCrLf)

            '--------------
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            'Modifica del 05/07/2023: aggiunta condizione per mostrare i soli movimenti legati al modulo cantine (fix per completare modifica del 02/03/2022)
            stbQ.Append(" AND EXISTS (SELECT 1 FROM OGenerazioni_Anagrafe_Log WHERE OGenerazioni_Anagrafe_Log.Elem_Cod = Movimenti_Dettagli.Elem_Cod " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Mat_Cod  = Movimenti_Dettagli.Mat_Cod  " & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Modulo_generazione = " & enum_Omni_Modulo_Generazione.Cantine & ")  " & vbCrLf)
            '--------------

            'stbQ.Append(" AND Materie_Prime_Calibri.Piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")
            'stbQ.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = 'calibro' ")
            ''stbQ.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri_Vinificazione = 1 ")
            'stbQ.Append(" AND ( Materie_Prime_ParametriQualitativi.ChkRegistri_Vinificazione = 1 OR Materie_Prime_ParametriQualitativi.ChkRegistri = 1 ) ")

            'Modifica del 02/03/2022: è stato commentato perché la rigenerazione nel GiasLan non scrive più in queste tabelle, aggiunto il filtro sul modulo generazione
            'stbQ.Append("  AND EXISTS (SELECT 1 FROM OperazionixReport OPR  " & vbCrLf)
            'stbQ.Append("               WHERE  Agenda.Piva = OPR.Piva  " & vbCrLf)
            'stbQ.Append("               AND Agenda.Lav_Cod = OPR.Lav_Cod  " & vbCrLf)
            'stbQ.Append("               AND OPR.Id_Report <> " & Agro_SQL_SaveNum(enum_AgroReportistica.PreparazioniBio) & ") " & vbCrLf)

            'stbQ.Append("  AND EXISTS (SELECT 1 FROM Materie_PrimexReport MPR  " & vbCrLf)
            'stbQ.Append("               WHERE  Materie_Prime.Piva = MPR.Piva   " & vbCrLf)
            'stbQ.Append("               AND Materie_Prime.mat_cod = MPR.mat_Cod  " & vbCrLf)
            'stbQ.Append("               AND MPR.Id_Report <> " & Agro_SQL_SaveNum(enum_AgroReportistica.PreparazioniBio) & ") " & vbCrLf)

            ''CORREZIONE DEL 07/05/2014: la piva di OperazionixReport è quella del superuser,
            ''non va quindi messa in join con la piva di agenda!
            ''è stato scoperto ora perchè qualitoscana è il primo cliente col quale si stampano i registri di cantina sulle aziende figlie in gerarchia
            'stbQ.Append(" AND EXISTS (SELECT 1 FROM OperazionixReport OPR  " + vbCrLf)
            'stbQ.Append("             WHERE  OPR.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " + vbCrLf)
            'stbQ.Append("             AND OPR.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " + vbCrLf)
            'stbQ.Append("               AND Agenda.Lav_Cod = OPR.Lav_Cod ) " + vbCrLf)

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Id_Destinazione <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " & vbCrLf)
            End If
            If Cal_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime_ParametriQualitativi.tipo_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If
            If Linea_Cod <> 0 Then
                stbQ.Append("   AND  EXISTS (SELECT 1  " & vbCrLf)
                stbQ.Append("               FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
                stbQ.Append(" 				WHERE materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod " & vbCrLf)
                stbQ.Append(" 				AND materie_prime.elem_cod = OGenerazioni_Anagrafe_Log.elem_cod " & vbCrLf)
                stbQ.Append("               AND OGenerazioni_Anagrafe_Log.linea_cod = " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
                stbQ.Append("               ) " & vbCrLf)
            End If
            If Cau_Mov <> "" Then
                stbQ.Append(" AND Movimenti.CAU_MOV = '" & Agro_SQL_SaveText(Cau_Mov) & "'" & vbCrLf)
            End If
            'If Cod_Contatto_Terzi <> "" Then
            '    'modifica del 21/01/2013: separato il filtro per la parte agenda e la parte linee
            '    stbQ.Append(RegistroVinificazione_FiltroSQL_ContoTerzi_ParteAgenda(objParametri.PivaSuperUser, _
            '                                                                        Piva, _
            '                                                                        Cod_Contatto_Terzi, _
            '                                                                        Lista_CodRisUm))
            'End If

            ''FILTRO PER GESTIONE DELLA VINIFICAZIONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            'stbQ.Append(RegistroVinificazione_FiltroSQL_Importante(objParametri, Lista_PrepCod, Lista_IdTrasf_NoComm, Piva))

            stbQ.Append(" ORDER BY  Movimenti.Data_Movimento, convert(char(8), movimenti.ora, 108), Agenda.Id_Agenda, Movimenti.Cau_Mov DESC, Movimenti_dettagli.Elem_Cod DESC  " & vbCrLf)
            'aggiunto Elem_Cod DESC per avere i cali per ultimi

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    '###################################################################################
    Public Function Lista_IdTrasformazione_Lotti_NonPassatiACommercializzazione(ByVal Piva As String,
                                                                                ByVal Lista_PrepCod_PassaggiAComm As String,
                                                                                ByVal xOrderBy As String,
                                                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                                ) As String


        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.RegistriCantina.Lista_IdTrasformazione_Lotti_NonPassatiACommercializzazione"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim Lista_Id As String = ""
        Dim i As Integer

        Try

            DT = ElencoLotti_NonPassatiACommercializzazione(Piva,
                                                            Lista_PrepCod_PassaggiAComm,
                                                            "", xOrderBy,
                                                            objParametri)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                For i = 0 To DT.Rows.Count - 1
                    Lista_Id &= CStr(DT.Rows(i).Item("Id_trasformazione")) & ","
                Next
                Lista_Id = Left(Lista_Id, Lista_Id.Length - 1)
                Lista_Id = "(" & Lista_Id & ")"
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Lista_Id


    End Function


    '################################################################################
    Public Function ElencoLotti_NonPassatiACommercializzazione(ByVal Piva As String,
                                                                 ByVal Lista_PrepCod_PassaggiAComm As String,
                                                                ByVal Lotto As String,
                                                                ByVal xOrderBy As String,
                                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.RegistriCantina.ElencoLotti_NonPassatiACommercializzazione"
        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim xFiltroAggiuntivo1 As String = ""
        Dim xFiltroAggiuntivo2 As String = ""
        Dim xFiltroAggiuntivo3 As String = ""

        Try

            StbSQL.Length = 0

            If Lotto <> "" Then
                xFiltroAggiuntivo1 = " UPPER(Trasformazioni2.Trasformazione_Des) = UPPER('" & Agro_SQL_SaveText(Lotto) & "') " & vbCrLf
                xFiltroAggiuntivo2 = " UPPER(Trasformazioni2.Trasformazione_Des) = UPPER('" & Agro_SQL_SaveText(Lotto) & "') " & vbCrLf
                xFiltroAggiuntivo3 = " UPPER(Trasformazioni.Trasformazione_Des) = UPPER('" & Agro_SQL_SaveText(Lotto) & "') " & vbCrLf
            End If

            'NON POSSO FARE IL NOT EXISTS PERCHE' DEVO INCLUDERE IL JOIN DENTRO ALLA QUERY
            'e ora c'è filtro del lotto al suo posto
            'StbSQL.Append(" SELECT * " & vbCrLf)
            'StbSQL.Append(" FROM trasformazioni Trasf_Ext " & vbCrLf)
            'StbSQL.Append(" WHERE  NOT EXISTS (  " & vbCrLf)
            'StbSQL.Append("                     SELECT 1 " & vbCrLf)
            'StbSQL.Append("                     FROM " & vbCrLf)
            'StbSQL.Append("                     ( " & vbCrLf)
            StbSQL.Append(" SELECT * " & vbCrLf)
            StbSQL.Append(" FROM trasformazioni Trasf_Ext " & vbCrLf)
            StbSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "'  " & vbCrLf)
            StbSQL.Append(" AND id_trasformazione NOT IN   " & vbCrLf)
            StbSQL.Append("                     (  " & vbCrLf)
            StbSQL.Append("                     SELECT id_trasformazione_commercializzazione " & vbCrLf)
            'StbSQL.Append("                     SELECT id_trasformazione " & vbCrLf)
            StbSQL.Append("                     FROM " & vbCrLf)
            StbSQL.Append("                         ( " & vbCrLf)
            StbSQL.Append(SQL_Operazioni_PassaggioACommercializzazione(Piva,
                                                                        Lista_PrepCod_PassaggiAComm,
                                                                        "",
                                                                        xFiltroAggiuntivo1,
                                                                        xFiltroAggiuntivo2,
                                                                        xFiltroAggiuntivo3
                                                                        ))
            'StbSQL.Append(SQL_Passaggi_DaARegistro_DaALotto_Completa(Piva, _
            '                                                        Lista_PrepCod_PassaggiAComm, _
            '                                                        "", _
            '                                                        Lotto, _
            '                                                        xFiltroAggiuntivo1, _
            '                                                        xFiltroAggiuntivo2, _
            '                                                        xFiltroAggiuntivo3, _
            '                                                        xFiltroAggiuntivo4, _
            '                                                        ""))

            StbSQL.Append("                         ) AS PASSAGGIO " & vbCrLf)
            StbSQL.Append("                     ) -- chiusura NOT IN" & vbCrLf)
            If Lotto <> "" Then
                StbSQL.Append(" AND Trasformazione_Des = '" & Agro_SQL_SaveText(Lotto) & "' " & vbCrLf)
            End If
            StbSQL.Append("     ORDER BY trasformazione_des " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    '################################################################################
    Public Function SQL_Operazioni_PassaggioACommercializzazione(ByVal Piva As String,
                                                                ByVal Lista_PrepCod_PassaggiAComm As String,
                                                                ByVal Lotto As String,
                                                                ByVal xFiltroAggiuntivo1 As String,
                                                                ByVal xFiltroAggiuntivo2 As String,
                                                                ByVal xFiltroAggiuntivo3 As String
                                                                 ) As String

        'ByVal Lista_IdTrasf_NoComm As String, _

        Dim StbSQL As New StringBuilder

        StbSQL.Length = 0

        StbSQL.Append(SQL_Operazioni_PassaggioACommercializzazione_Caso1(Piva,
                                                                        Lista_PrepCod_PassaggiAComm,
                                                                        Lotto,
                                                                        xFiltroAggiuntivo1))


        StbSQL.AppendLine(" UNION ALL ")

        StbSQL.Append(SQL_Operazioni_PassaggioACommercializzazione_Caso2(Piva,
                                                                        Lista_PrepCod_PassaggiAComm,
                                                                        Lotto,
                                                                        xFiltroAggiuntivo2))


        StbSQL.AppendLine(" UNION ALL ")

        StbSQL.Append(SQL_Operazioni_PassaggioACommercializzazione_Caso3(Piva,
                                                                        Lotto,
                                                                        xFiltroAggiuntivo3))


        Return StbSQL.ToString

    End Function

    '################################################################################
    'restituisce la parte di query per la lettura delle operazioni di passaggio a commercializzazione
    Public Function SQL_Operazioni_PassaggioACommercializzazione_Caso1(ByVal Piva As String,
                                                                        ByVal Lista_PrepCod_PassaggiAComm As String,
                                                                         ByVal Lotto As String,
                                                                        ByVal xFiltroAggiuntivo1 As String
                                                                        ) As String

        'ByVal Lista_IdTrasf_NoComm As String, _

        Dim StbSQL As New System.Text.StringBuilder

        StbSQL.Length = 0

        '------------------------------------------------------ 
        '-------------- PASSAGGIO A REGISTRO --------------------
        'lotto 1 passa a commercializzazione e diventa lotto 2
        'lotti passati a commercializzazione con operazione di passaggio a registro
        '------------------------------------------------------
        StbSQL.Append(" -- CASO1: PASSAGGIO A REGISTRO " & vbCrLf)
        StbSQL.Append(" -- lotti passati a commercializzazione con operazione di passaggio a registro " & vbCrLf)
        StbSQL.Append(" -- (lotto 1 passa a commercializzazione e diventa lotto 2) " & vbCrLf)

        StbSQL.Append(" ( " & vbCrLf)
        StbSQL.Append(" SELECT DISTINCT TR.Validita_Inizio AS Data, TR.Id_Trasformazione_Rif AS Id_Trasformazione_Commercializzazione, Trasformazioni2.Trasformazione_Des AS Lotto_Commercializzazione " & vbCrLf)
        StbSQL.Append(" -- TR.Id_Trasformazione, Trasformazioni1.Trasformazione_Des, tipo_default " & vbCrLf)
        StbSQL.Append(" FROM Trasformazioni_Riferimenti AS TR  " & vbCrLf)
        StbSQL.Append(" -- INNER JOIN Linee_Preparazioni ON Linee_Preparazioni.Piva = TR.Piva  AND Linee_Preparazioni.preparazione_cod = TR.preparazione_cod " & vbCrLf)
        StbSQL.Append(" INNER JOIN Trasformazioni Trasformazioni1 ON Trasformazioni1.Piva = TR.Piva AND Trasformazioni1.Id_Trasformazione = TR.Id_Trasformazione " & vbCrLf)
        StbSQL.Append(" INNER JOIN Trasformazioni Trasformazioni2 ON Trasformazioni2.Piva = TR.Piva AND Trasformazioni2.Id_Trasformazione = TR.Id_Trasformazione_Rif " & vbCrLf)
        StbSQL.Append(" WHERE TR.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
        StbSQL.Append(" AND TR.Preparazione_Cod IN " & Agro_SQL_SaveText(Lista_PrepCod_PassaggiAComm) & vbCrLf)
        If Lotto <> "" Then
            StbSQL.Append(" AND UPPER(Trasformazioni2.Trasformazione_Des) = UPPER('" & Agro_SQL_SaveText(Lotto) & "') " & vbCrLf)
        End If
        If xFiltroAggiuntivo1 <> "" Then
            StbSQL.Append(" AND " & xFiltroAggiuntivo1 & vbCrLf)
        End If
        StbSQL.Append(" ) -- fine caso2 " & vbCrLf)

        Return StbSQL.ToString

    End Function

    '################################################################################
    'restituisce la parte di query per la lettura delle operazioni che comportano passaggio a commercializzazione
    'LOTTO 1-> (PASSA A COMMERCIALIZZAZIONE) LOTTO2 -> (DIVENTA) LOTTO3
    Public Function SQL_Operazioni_PassaggioACommercializzazione_Caso2(ByVal Piva As String,
                                                                        ByVal Lista_PrepCod_PassaggiAComm As String,
                                                                         ByVal Lotto As String,
                                                                        ByVal xFiltroAggiuntivo2 As String
                                                                        ) As String

        'ByVal Lista_IdTrasf_NoComm As String, _

        Dim StbSQL As New StringBuilder

        StbSQL.Length = 0

        '------------------------------------------------------ 
        '--------------INTERAZIONE TRA LOTTI --------------------
        'operazioni che comportano passaggio a commercializzazione
        'LOTTO 1-> (PASSA A COMMERCIALIZZAZIONE) LOTTO2 -> (DIVENTA) LOTTO3
        '------------------------------------------------------
        StbSQL.AppendLine(" -- CASO2: INTERAZIONE TRA LOTTI ")
        StbSQL.AppendLine(" -- operazioni che comportano passaggio a commercializzazione ")
        StbSQL.AppendLine(" -- LOTTO 1-> (PASSA A COMMERCIALIZZAZIONE) LOTTO2 -> (DIVENTA) LOTTO3 ")
        StbSQL.AppendLine(" ( ")
        StbSQL.AppendLine(" SELECT DISTINCT TR2.Validita_Inizio AS Data, TR2.Id_Trasformazione_Rif AS Id_Trasformazione_Commercializzazione, Trasformazioni2.Trasformazione_Des AS Lotto_Commercializzazione ")
        StbSQL.AppendLine(" -- TR.Id_Trasformazione, Trasformazioni1.Trasformazione_Des, tipo_default ")
        StbSQL.AppendLine(" FROM Trasformazioni_Riferimenti AS TR2  ")
        StbSQL.AppendLine(" -- INNER JOIN Linee_Preparazioni ON Linee_Preparazioni.Piva = TR2.Piva  AND Linee_Preparazioni.preparazione_cod = TR2.preparazione_cod ")
        StbSQL.AppendLine(" INNER JOIN Trasformazioni Trasformazioni1 ON Trasformazioni1.Piva = TR2.Piva AND Trasformazioni1.Id_Trasformazione = TR2.Id_Trasformazione ")
        StbSQL.AppendLine(" INNER JOIN Trasformazioni Trasformazioni2 ON Trasformazioni2.Piva = TR2.Piva AND Trasformazioni2.Id_Trasformazione = TR2.Id_Trasformazione_Rif ")
        StbSQL.AppendLine(" WHERE TR2.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
        StbSQL.AppendLine(" AND EXISTS ( ")
        StbSQL.AppendLine("             SELECT DISTINCT TR_Int.Id_Trasformazione_Rif  ")
        StbSQL.AppendLine("             FROM Trasformazioni_Riferimenti TR_Int ")
        StbSQL.AppendLine("             WHERE TR_Int.Id_Trasformazione_Rif = TR2.Id_Trasformazione   ")
        StbSQL.AppendLine("             AND TR_Int.Preparazione_Cod IN " & Agro_SQL_SaveText(Lista_PrepCod_PassaggiAComm))
        StbSQL.AppendLine("             )    ")
        If Lotto <> "" Then
            StbSQL.AppendLine(" AND UPPER(Trasformazioni2.Trasformazione_Des) = UPPER('" & Agro_SQL_SaveText(Lotto) & "') ")
        End If
        If xFiltroAggiuntivo2 <> "" Then
            StbSQL.AppendLine(" AND " & xFiltroAggiuntivo2)
        End If
        StbSQL.AppendLine(" ) -- fine caso2 ")

        Return StbSQL.ToString

    End Function

    '################################################################################
    Public Function SQL_Operazioni_PassaggioACommercializzazione_Caso3(ByVal Piva As String,
                                                                        ByVal Lotto As String,
                                                                        ByVal xFiltroAggiuntivo3 As String
                                                                        ) As String

        Dim StbSQL As New System.Text.StringBuilder

        StbSQL.Length = 0

        '------------------------------------------------------ 
        '------------- CASO 3 - LOTTI STORICIZZATI O NUOVO RILEVAMENTO CONSISTENZE ENO ----------------
        'lettura dei campi (dati relativi al passaggio) della tabella trasformazioni  
        '------------------------------------------------------
        StbSQL.Append(" -- 3 - LOTTI STORICIZZATI O NUOVO RILEVAMENTO CONSISTENZE ENO " & vbCrLf)
        StbSQL.Append(" -- lettura dei campi (dati relativi al passaggio) della tabella trasformazioni  " & vbCrLf)
        StbSQL.Append(" ( " & vbCrLf)
        StbSQL.Append(" SELECT DISTINCT passaggio_data AS Data, Id_Trasformazione AS Id_Trasformazione_Commercializzazione, Trasformazione_Des AS Lotto_Commercializzazione " & vbCrLf)
        StbSQL.Append(" --  0 AS Id_Trasformazione, 0 AS Trasformazione_Des, 0 AS tipo_default " & vbCrLf)
        StbSQL.Append(" FROM Trasformazioni  " & vbCrLf)
        StbSQL.Append(" WHERE Trasformazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
        StbSQL.Append(" AND ChkPassaggio = 1 " & vbCrLf)
        If Lotto <> "" Then
            StbSQL.Append(" AND UPPER(Trasformazioni.Trasformazione_Des) = UPPER('" & Agro_SQL_SaveText(Lotto) & "') " & vbCrLf)
        End If
        If xFiltroAggiuntivo3 <> "" Then
            StbSQL.Append(" AND " & xFiltroAggiuntivo3 & vbCrLf)
        End If
        StbSQL.Append(" ) -- fine caso3 " & vbCrLf)

        Return StbSQL.ToString


    End Function




    '################################################################################
    'restituisce il pezzo di query che recupera l'elenco delle operazioni che comportano passaggio a commercializzazione
    'viene chiamata in due volte
    '1) si passa piva e Lista_PrepCod_PassaggiAComm -> utilizzata nella query che restituisce l'elenco dei lotti non passati a commercializzazione
    '2) si passa piva, Lista_PrepCod_PassaggiAComm e Lista_IdTrasf_NoComm -> utilizzata nella query del registro di commercializzazione per verificare se un lotto è passato a commercializzazione
    Public Function Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_Completa(ByVal Piva As String,
                                                                ByVal Lista_PrepCod_PassaggiAComm As String,
                                                                ByVal Lista_IdTrasf_NoComm As String,
                                                                ByVal Lotto As String,
                                                                ByVal xFiltroAggiuntivo1 As String,
                                                                ByVal xFiltroAggiuntivo2 As String,
                                                                ByVal xFiltroAggiuntivo3 As String,
                                                                ByVal xFiltroAggiuntivo4 As String,
                                                                ByVal xOrderBy As String
                                                                ) As String

        Dim StbSQL As New StringBuilder

        StbSQL.Length = 0

        StbSQL.Append(Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteI(Piva,
                                                                Lista_PrepCod_PassaggiAComm,
                                                                Lotto,
                                                                xFiltroAggiuntivo1))


        StbSQL.AppendLine(" UNION ALL ")

        StbSQL.Append(Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteII(Piva,
                                                                Lista_PrepCod_PassaggiAComm,
                                                                Lista_IdTrasf_NoComm,
                                                                Lotto,
                                                                xFiltroAggiuntivo2))


        StbSQL.AppendLine(" UNION ALL ")

        StbSQL.Append(Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteIII(Piva,
                                                                Lista_PrepCod_PassaggiAComm,
                                                                Lista_IdTrasf_NoComm,
                                                                Lotto,
                                                                xFiltroAggiuntivo3))

        StbSQL.AppendLine(" UNION ALL ")

        StbSQL.Append(Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteIV(Piva,
                                                                Lotto,
                                                                xFiltroAggiuntivo4))


        Return StbSQL.ToString

    End Function

    '################################################################################
    'viene utilizzata dalla query del registro di vinificazione
    Public Function Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteIIeIII(ByVal Piva As String,
                                                                    ByVal Lista_PrepCod_PassaggiAComm As String,
                                                                    ByVal Lista_IdTrasf_NoComm As String,
                                                                    ByVal Lotto As String,
                                                                    ByVal xFiltroAggiuntivo2 As String,
                                                                    ByVal xFiltroAggiuntivo3 As String
                                                                    ) As String

        Dim StbSQL As New StringBuilder

        StbSQL.Length = 0

        StbSQL.Append(Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteII(Piva,
                                                                Lista_PrepCod_PassaggiAComm,
                                                                Lista_IdTrasf_NoComm,
                                                                Lotto,
                                                                xFiltroAggiuntivo2))


        StbSQL.AppendLine(" UNION ALL ")

        StbSQL.Append(Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteIII(Piva,
                                                                Lista_PrepCod_PassaggiAComm,
                                                                Lista_IdTrasf_NoComm,
                                                                Lotto,
                                                                xFiltroAggiuntivo3))


        Return StbSQL.ToString

    End Function

    '################################################################################
    'viene utilizzata dalla query del registro di vinificazione
    Public Function Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteIeIV(ByVal Piva As String,
                                                                    ByVal Lista_PrepCod_PassaggiAComm As String,
                                                                    ByVal Lotto As String,
                                                                    ByVal xFiltroAggiuntivo1 As String,
                                                                    ByVal xFiltroAggiuntivo4 As String
                                                                    ) As String

        Dim StbSQL As New StringBuilder

        StbSQL.Length = 0

        StbSQL.Append(Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteI(Piva,
                                                                Lista_PrepCod_PassaggiAComm,
                                                                Lotto,
                                                                xFiltroAggiuntivo1))

        StbSQL.AppendLine(" UNION ALL ")

        StbSQL.Append(Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteIV(Piva,
                                                                Lotto,
                                                                xFiltroAggiuntivo4))


        Return StbSQL.ToString

    End Function


    '################################################################################
    'restituisce la parte di query per la lettura delle operazioni di
    'passaggio da vinificazione a commercializzazione con lo stesso lotto (vecchia modalità)
    Public Function Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteI(ByVal Piva As String,
                                                            ByVal Lista_PrepCod_PassaggiAComm As String,
                                                            ByVal Lotto As String,
                                                            ByVal xFiltroAggiuntivo1 As String
                                                            ) As String

        Dim StbSQL As New System.Text.StringBuilder

        StbSQL.Length = 0

        '------------------------------------------------------ 
        '----------- PASSAGGIO CON LO STESSO LOTTO ------------
        'Passati a se stessi (Totale)--> cioè quelli passati a commercializzazione senza riferimento
        '------------------------------------------------------
        StbSQL.Append(" -- 1 - PASSAGGIO CON LO STESSO LOTTO " & vbCrLf)
        StbSQL.Append(" -- Passati a se stessi (Totale)--> cioè quelli passati a commercializzazione senza riferimento " & vbCrLf)
        StbSQL.Append(" ( " & vbCrLf)
        'Agenda.Validita_Inizio
        StbSQL.Append(" SELECT DISTINCT Agenda.Id_Trasformazione, Trasformazione_Des, Movimenti.Ora AS Data, 0 as Rif_Id_Trasformazione, '' AS Rif_Trasformazione_Des, tipo_default " & vbCrLf)
        StbSQL.Append(" FROM Agenda " & vbCrLf)
        StbSQL.Append(" INNER JOIN Linee_Preparazioni ON Linee_Preparazioni.Piva = Agenda.Piva  AND Linee_Preparazioni.preparazione_cod = Agenda.preparazione_cod " & vbCrLf)
        StbSQL.Append(" INNER JOIN Trasformazioni ON Trasformazioni.Piva = Agenda.Piva AND Trasformazioni.Id_Trasformazione = Agenda.Id_Trasformazione " & vbCrLf)
        StbSQL.Append(" INNER JOIN Movimenti ON Movimenti.Piva = Agenda.Piva AND Movimenti.Id_Agenda = Agenda.Id_Agenda " & vbCrLf)
        StbSQL.Append(" WHERE Agenda.Preparazione_Cod IN " & Agro_SQL_SaveText(Lista_PrepCod_PassaggiAComm) & vbCrLf)
        StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
        StbSQL.Append(" AND Movimenti.cau_mov = '" & CAU_LINEA_PRODUZIONE & "' " & vbCrLf)
        StbSQL.Append(" AND Agenda.Id_Agenda NOT IN ( " & vbCrLf)
        StbSQL.Append("                             SELECT  Id_Agenda  " & vbCrLf)
        StbSQL.Append("                             FROM    Trasformazioni_Riferimenti " & vbCrLf)
        StbSQL.Append("                             WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
        StbSQL.Append("                             ) " & vbCrLf)
        If Lotto <> "" Then
            StbSQL.Append(" AND UPPER(Trasformazioni.Trasformazione_Des) = UPPER('" & Agro_SQL_SaveText(Lotto) & "') " & vbCrLf)
        End If
        If xFiltroAggiuntivo1 <> "" Then
            StbSQL.Append(" AND " & xFiltroAggiuntivo1 & vbCrLf)
        End If
        StbSQL.Append(" ) -- fine 1 " & vbCrLf)

        Return StbSQL.ToString


    End Function

    '################################################################################
    'restituisce la parte di query per la lettura delle operazioni che comportano passaggio a commercializzazione
    'con cambio di lotto 
    'operazioni di UNIONE
    'es: taglio\accorpamento, separazione consistenze enologiche
    Public Function Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteII(ByVal Piva As String,
                                                                ByVal Lista_PrepCod_PassaggiAComm As String,
                                                                ByVal Lista_IdTrasf_NoComm As String,
                                                                ByVal Lotto As String,
                                                                ByVal xFiltroAggiuntivo2 As String
                                                                ) As String

        Dim StbSQL As New System.Text.StringBuilder

        StbSQL.Length = 0

        '------------------------------------------------------ 
        '----------- PASSAGGIO CON CAMBIO LOTTO ------------
        'Referenziati in Unione
        '------------------------------------------------------
        StbSQL.Append(" -- 2 - PASSAGGIO CON CAMBIO LOTTO " & vbCrLf)
        StbSQL.Append(" -- Referenziati in UNIONE " & vbCrLf)
        StbSQL.Append(" ( " & vbCrLf)
        StbSQL.Append(" SELECT DISTINCT TR.Id_Trasformazione, Trasformazioni1.Trasformazione_Des, TR.Validita_Inizio AS Data, TR.Id_Trasformazione_Rif AS Riferimento_Id, Trasformazioni2.Trasformazione_Des AS Rif_Trasformazione_Des, tipo_default " & vbCrLf)
        StbSQL.Append(" FROM Trasformazioni_Riferimenti AS TR  " & vbCrLf)
        StbSQL.Append(" INNER JOIN Linee_Preparazioni ON Linee_Preparazioni.Piva = TR.Piva  AND Linee_Preparazioni.preparazione_cod = TR.preparazione_cod " & vbCrLf)
        StbSQL.Append(" INNER JOIN Trasformazioni Trasformazioni1 ON Trasformazioni1.Piva = TR.Piva AND Trasformazioni1.Id_Trasformazione = TR.Id_Trasformazione " & vbCrLf)
        StbSQL.Append(" INNER JOIN Trasformazioni Trasformazioni2 ON Trasformazioni2.Piva = TR.Piva AND Trasformazioni2.Id_Trasformazione = TR.Id_Trasformazione_Rif " & vbCrLf)
        StbSQL.Append(" WHERE TR.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
        'If Lista_IdTrasf_NoComm <> "" Then
        '    StbSQL.Append(" AND TR.Id_Trasformazione NOT IN  " & Agro_SQL_SaveText(Lista_IdTrasf_NoComm) & vbCrLf)
        'End If
        StbSQL.Append(" AND TR.Id_Trasformazione_Rif IN  ( " & vbCrLf)
        StbSQL.Append("                                 SELECT DISTINCT Id_Trasformazione  " & vbCrLf)
        StbSQL.Append("                                 FROM Agenda  " & vbCrLf)
        StbSQL.Append("                                 INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda   " & vbCrLf)
        '                                               Lista_PrepCod_PassaggiAComm deve essere valorizzato
        StbSQL.Append("                                 WHERE Agenda.Preparazione_Cod IN " & Agro_SQL_SaveText(Lista_PrepCod_PassaggiAComm) & vbCrLf)
        If Lista_IdTrasf_NoComm <> "" Then
            StbSQL.Append("                             AND Agenda.Id_Trasformazione NOT IN  " & Agro_SQL_SaveText(Lista_IdTrasf_NoComm) & vbCrLf)
        End If
        'If Flag_1VINIF_2COMM = 1 Then
        '    StbSQL.Append("                             AND Movimenti.Ora >= ( " & vbCrLf)
        'ElseIf Flag_1VINIF_2COMM = 2 Then
        '    StbSQL.Append("                             AND Movimenti.Ora <= ( " & vbCrLf)
        'End If
        StbSQL.Append("                             AND Movimenti.Ora <= ( " & vbCrLf)
        StbSQL.Append("                                                     SELECT TOP 1 Ora" & vbCrLf)
        StbSQL.Append("                                                     FROM Movimenti" & vbCrLf)
        StbSQL.Append("                                                     WHERE Movimenti.Piva = TR.Piva   " & vbCrLf)
        StbSQL.Append("                                                     AND Movimenti.Id_Agenda = TR.Id_Agenda   " & vbCrLf)
        StbSQL.Append("                                                     AND Movimenti.Cau_Mov = '" & CAU_LINEA_PRODUZIONE & "' " & vbCrLf)
        StbSQL.Append("                                                     ) " & vbCrLf)
        StbSQL.Append("                                 ) " & vbCrLf)
        If Lotto <> "" Then
            StbSQL.Append(" AND UPPER(Trasformazioni1.Trasformazione_Des) = UPPER('" & Agro_SQL_SaveText(Lotto) & "') " & vbCrLf)
        End If
        If xFiltroAggiuntivo2 <> "" Then
            StbSQL.Append(" AND " & xFiltroAggiuntivo2 & vbCrLf)
        End If
        StbSQL.Append(" ) -- fine 2 " & vbCrLf)

        Return StbSQL.ToString

    End Function


    '################################################################################
    'restituisce la parte di query per la lettura delle operazioni che comportano passaggio a commercializzazione
    'con cambio di lotto 
    'operazioni di DIVISIONE
    'es: passaggio a commercializzaione (nuova modalità)
    Public Function Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteIII(ByVal Piva As String,
                                                                ByVal Lista_PrepCod_PassaggiAComm As String,
                                                                ByVal Lista_IdTrasf_NoComm As String,
                                                                ByVal Lotto As String,
                                                                ByVal xFiltroAggiuntivo3 As String
                                                                ) As String


        Dim StbSQL As New System.Text.StringBuilder

        StbSQL.Length = 0

        '------------------------------------------------------ 
        '----------- PASSAGGIO CON CAMBIO LOTTO ------------
        'Referenziati in Divisione
        '------------------------------------------------------
        StbSQL.Append(" -- 3 - PASSAGGIO CON CAMBIO LOTTO " & vbCrLf)
        StbSQL.Append(" -- Referenziati in DIVISIONE " & vbCrLf)

        StbSQL.Append(" ( " & vbCrLf)
        StbSQL.Append(" SELECT DISTINCT TR2.Id_Trasformazione_Rif, Trasformazioni2.Trasformazione_Des, TR2.Validita_Inizio AS Data, TR2.Id_Trasformazione AS Riferimento_Id, Trasformazioni1.Trasformazione_Des AS Rif_Trasformazione_Des, tipo_default " & vbCrLf)
        StbSQL.Append(" FROM Trasformazioni_Riferimenti AS TR2  " & vbCrLf)
        StbSQL.Append(" INNER JOIN Linee_Preparazioni ON Linee_Preparazioni.Piva = TR2.Piva  AND Linee_Preparazioni.preparazione_cod = TR2.preparazione_cod " & vbCrLf)
        StbSQL.Append(" INNER JOIN Trasformazioni Trasformazioni1 ON Trasformazioni1.Piva = TR2.Piva AND Trasformazioni1.Id_Trasformazione = TR2.Id_Trasformazione " & vbCrLf)
        StbSQL.Append(" INNER JOIN Trasformazioni Trasformazioni2 ON Trasformazioni2.Piva = TR2.Piva AND Trasformazioni2.Id_Trasformazione = TR2.Id_Trasformazione_Rif " & vbCrLf)
        StbSQL.Append(" WHERE TR2.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
        'If Lista_IdTrasf_NoComm <> "" Then
        '    StbSQL.Append(" AND TR2.Id_Trasformazione_Rif NOT IN  " & Agro_SQL_SaveText(Lista_IdTrasf_NoComm) & vbCrLf)
        'End If
        StbSQL.Append(" AND TR2.Id_Trasformazione IN  ( " & vbCrLf)
        StbSQL.Append("                                 SELECT DISTINCT Id_Trasformazione  " & vbCrLf)
        StbSQL.Append("                                 FROM Agenda  " & vbCrLf)
        StbSQL.Append("                                 INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda   " & vbCrLf)
        '                                               Lista_PrepCod_PassaggiAComm deve essere valorizzato
        StbSQL.Append("                                 WHERE Agenda.Preparazione_Cod IN " & Agro_SQL_SaveText(Lista_PrepCod_PassaggiAComm) & vbCrLf)

        If Lista_IdTrasf_NoComm <> "" Then
            StbSQL.Append(" " & vbCrLf)
            StbSQL.Append("                             AND ( " & vbCrLf)
            StbSQL.Append("                                 Agenda.Id_Trasformazione NOT IN  " & Agro_SQL_SaveText(Lista_IdTrasf_NoComm) & vbCrLf)
            StbSQL.Append("                                 OR " & vbCrLf)
            StbSQL.Append("                                 Agenda.Id_Trasformazione IN ( " & vbCrLf)
            StbSQL.Append("                                                              SELECT Id_Trasformazione " & vbCrLf)
            StbSQL.Append("                                                             FROM Trasformazioni_Riferimenti AS TRRR  " & vbCrLf)
            StbSQL.Append("                                                             WHERE TRRR.Preparazione_Cod IN " & Agro_SQL_SaveText(Lista_PrepCod_PassaggiAComm) & vbCrLf)
            StbSQL.Append("                                                             AND TRRR.id_trasformazione_rif = TR2.Id_Trasformazione_rif " & vbCrLf)
            StbSQL.Append("                                                             ) " & vbCrLf)
            StbSQL.Append("                                 ) " & vbCrLf)
            StbSQL.Append(" " & vbCrLf)
        End If

        'StbSQL.Append("                             AND Movimenti.Ora <= ( " & vbCrLf)
        'StbSQL.Append("                                                     SELECT TOP 1 Ora" & vbCrLf)
        'StbSQL.Append("                                                     FROM Movimenti" & vbCrLf)
        'StbSQL.Append("                                                     WHERE Movimenti.Piva = TR2.Piva   " & vbCrLf)
        'StbSQL.Append("                                                     AND Movimenti.Id_Agenda = TR2.Id_Agenda   " & vbCrLf)
        'StbSQL.Append("                                                     AND Movimenti.Cau_Mov = '" & CAU_LINEA_PRODUZIONE & "' " & vbCrLf)
        'StbSQL.Append("                                                     ) " & vbCrLf)
        StbSQL.Append("                                 ) " & vbCrLf)
        If Lotto <> "" Then
            StbSQL.Append(" AND UPPER(Trasformazioni2.Trasformazione_Des) = UPPER('" & Agro_SQL_SaveText(Lotto) & "') " & vbCrLf)
        End If
        If xFiltroAggiuntivo3 <> "" Then
            StbSQL.Append(" AND " & xFiltroAggiuntivo3 & vbCrLf)
        End If
        StbSQL.Append(" ) -- fine 3 " & vbCrLf)

        Return StbSQL.ToString

    End Function

    '################################################################################
    Public Function Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteIV(ByVal Piva As String,
                                                            ByVal Lotto As String,
                                                            ByVal xFiltroAggiuntivo4 As String
                                                            ) As String

        Dim StbSQL As New System.Text.StringBuilder

        StbSQL.Length = 0

        '------------------------------------------------------ 
        '-------------  4 - LOTTI STORICIZZATI ----------------
        'lettura dei campi (dati relativi al passaggio) della tabella trasformazioni  
        '------------------------------------------------------
        StbSQL.Append(" -- 4 - LOTTI STORICIZZATI " & vbCrLf)
        StbSQL.Append(" -- lettura dei campi (dati relativi al passaggio) della tabella trasformazioni  " & vbCrLf)
        StbSQL.Append(" ( " & vbCrLf)
        StbSQL.Append(" SELECT DISTINCT Id_Trasformazione, Trasformazione_Des, passaggio_data AS Data, 0 AS Rif_Id_Trasformazione, '' AS Rif_Trasformazione_Des, 1 AS tipo_default  " & vbCrLf)
        StbSQL.Append(" FROM Trasformazioni  " & vbCrLf)
        StbSQL.Append(" WHERE ChkPassaggio <> 0 AND ChkPassaggio <> NULL " & vbCrLf)
        'StbSQL.Append(" AND  UPPER(Trasformazioni.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) " & vbCrLf)
        If Lotto <> "" Then
            StbSQL.Append(" AND UPPER(Trasformazioni.Trasformazione_Des) = UPPER('" & Agro_SQL_SaveText(Lotto) & "') " & vbCrLf)
        End If
        If xFiltroAggiuntivo4 <> "" Then
            StbSQL.Append(" AND " & xFiltroAggiuntivo4 & vbCrLf)
        End If
        StbSQL.Append(" ) -- fine 4 " & vbCrLf)

        Return StbSQL.ToString


    End Function



    '##############################################################################################
    'il filtro lo fa RegistroVinificazione_FiltroSQL_Importante_ContoTerzi_ParteAgenda
    Public Function Z_OLD_Filtro_CodRisUm_xRegistri(ByVal Piva As String,
                                            ByVal Cod_Contatto As String,
                                            ByVal Piva_SuperUser_Origine As String,
                                            ByVal Flag_AncheImportatati As Boolean,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As String

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.RegistriCantina.Filtro_CodRisUm_ByLista()"
        Dim MessaggioErrore As String = ""

        Dim Filtro As String = ""
        Dim Lista As String = ""

        Try

            Dim objRisUm As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R

            Lista = objRisUm.Lista_CodRisUm_ByChiaveContatto(Piva,
                                                Cod_Contatto,
                                                "",
                                                True,
                                                xFiltroAggiuntivo,
                                                objParametri)

            If Lista <> "" Then
                Filtro = " AND ( Movimenti.Cod_risum = 0 " &
                                " OR Movimenti.Cod_risum IN ( " & Lista & ") " &
                                ")"
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Filtro

    End Function


    '##############################################################################################
    Private Function RegistroCommercializzazione_FiltroSQL_ConGestioneVinificazione(ByRef objParametri_Server As AgronicaCoreParametri,
                                                                                    ByVal Opt_Gestione_RegistroVinificazione As Integer,
                                                                                    ByVal Lista_PrepCod As String,
                                                                                    ByVal Lista_IdTrasf_NoComm As String,
                                                                                    ByVal Piva As String) As String

        Dim stbQ As New System.Text.StringBuilder
        Dim FiltroSQL As String = ""

        'modifica del 19/10/2012: SE C'E' LA GESTIONE DEL REGISTRO DI VINIFICAZIONE
        'allora nel reg di comm. compariranno tutte le operazioni inserite 
        'dopo il giorno e l'ora della data di passaggio da reg vinificazione a reg comm.
        If Opt_Gestione_RegistroVinificazione = 1 Then

            'query con tracciabilità:
            'da usare quando ci sarà sempre un'operazione di passaggio

            ''operazione di agenda del prodotto stesso o prodotto padre 
            ''scaricato 
            'stbQ.Append(" AND (            " + vbCrLf)

            ''caso 1
            ''1. che un prodotto scaricato (padre) in una preparazione in cui il prodotto in questione (figlio) 
            ''è stato caricato con data precedente alla movimentazione in oggetto, 
            ''sia presente in un passaggio da registro a registro precedente alla data in oggetto.
            'stbQ.Append("   ( EXISTS ( SELECT 1              " + vbCrLf)
            'stbQ.Append("              FROM  Agenda AgP " + vbCrLf)
            'stbQ.Append("              INNER JOIN  Movimenti movP ON AgP.PIVA = movP.PIVA AND AgP.Id_Agenda = movP.Id_Agenda  " + vbCrLf)
            'stbQ.Append("              INNER JOIN Movimenti_dettagli movdetP ON movP.PIVA = movdetP.PIVA AND movP.Id_Agenda = movdetP.Id_Agenda AND movP.Id_Mov = movdetP.Id_Mov  " + vbCrLf)
            'stbQ.Append("              WHERE movdetP.Elem_Cod = Movimenti_dettagli.Elem_Cod " + vbCrLf)
            'stbQ.Append("              AND movP.CAu_Mov = '" & CStr(CAU_SCARICO) & "' " & vbCrLf)

            ''LETTURA DELLE OP. DI AGENDA CHE SONO PASSAGGIO DA REGISTRO DI VINIFICAZIONE A REGISTRO DI COMM.
            'stbQ.Append("              AND EXISTS ( SELECT 1              " + vbCrLf)
            'stbQ.Append("                           FROM Agenda AgPAS              " + vbCrLf)
            'stbQ.Append("                           INNER JOIN Movimenti movPAS ON AgPAS.PIVA = movPAS.PIVA AND AgPAS.Id_Agenda = movPAS.Id_Agenda  " + vbCrLf)
            'stbQ.Append("                           INNER JOIN Movimenti_dettagli movdetPAS ON movPAS.PIVA = movdetPAS.PIVA AND movPAS.Id_Agenda = movdetPAS.Id_Agenda AND movPAS.Id_MOV = movdetPAS.Id_MOV  " + vbCrLf)
            'stbQ.Append("                            WHERE AgPAS.Preparazione_Cod IN " & CStr(Lista_PrepCod) + vbCrLf)
            'stbQ.Append("                           AND movPAS.data_movimento <= movP.Data_Movimento " & vbCrLf)
            'stbQ.Append("                           AND movdetPAS.Elem_Cod = movdetP.Elem_Cod " + vbCrLf)
            'stbQ.Append("                           AND movdetPAS.Pro_Cod = movdetP.Pro_Cod " + vbCrLf)
            'stbQ.Append("                           AND movdetPAS.Mat_Cod = movdetP.Mat_Cod " + vbCrLf)
            'stbQ.Append("                           AND movdetPAS.udm_Cod = movdetP.udm_Cod " + vbCrLf)
            'stbQ.Append("                           AND movdetPAS.lotto = movdetP.lotto   " + vbCrLf)
            'stbQ.Append("                           ) " + vbCrLf)

            ''op. di agenda in cui il prodotto in oggetto è caricato
            'stbQ.Append("              AND EXISTS ( SELECT 1  " + vbCrLf)
            'stbQ.Append("                           FROM  Agenda AgF " + vbCrLf)
            'stbQ.Append("                           INNER JOIN  Movimenti movF ON AgF.PIVA = movF.PIVA AND AgF.Id_Agenda = movF.Id_Agenda  " + vbCrLf)
            'stbQ.Append("                           INNER JOIN Movimenti_dettagli movdetF ON movF.PIVA = movdetF.PIVA AND movF.Id_Agenda = movdetF.Id_Agenda AND movF.Id_Mov = movdetF.Id_Mov  " + vbCrLf)
            'stbQ.Append("                           WHERE movdetF.Elem_Cod = Movimenti_dettagli.Elem_Cod " + vbCrLf)
            'stbQ.Append("                           AND movdetF.Pro_Cod = Movimenti_dettagli.Pro_Cod " + vbCrLf)
            'stbQ.Append("                           AND movdetF.Mat_Cod = Movimenti_dettagli.Mat_Cod " + vbCrLf)
            'stbQ.Append("                           AND movdetF.udm_Cod = Movimenti_dettagli.udm_Cod " + vbCrLf)
            'stbQ.Append("                           AND UPPER(movdetF.lotto) = UPPER(Movimenti_dettagli.lotto)   " + vbCrLf)
            'stbQ.Append("                           AND movF.CAu_Mov = '" & CStr(CAU_CARICO) & "' " & vbCrLf)
            'stbQ.Append("                           AND movF.data_movimento <= Movimenti.Data_Movimento " & vbCrLf)
            'stbQ.Append("                           AND AgP.pIVA = Agf.PIVA AND AgP.id_agenda = Agf.id_agenda  " + vbCrLf)
            'stbQ.Append("                           )  " + vbCrLf)
            'stbQ.Append("           ) " + vbCrLf)
            'stbQ.Append("      ) -- FINE CASO 1.  " + vbCrLf)
            'stbQ.Append("     OR  -- INIZIO CASO 2         " + vbCrLf)
            ''CASO 2
            ''2. sia un confezionato/condizionato (udm_cod_extra <> 0)
            ''- la preparazione in questione potrebbe quindi essere il passaggio da registro a registro.
            ''- per il registro quello di vinificazione si usa il duale.
            ''- se non si usa il trucco sui confezionati/condizionati occorre controllare anche il nonno e sarebbe troppo annidato.
            'stbQ.Append("     (           " + vbCrLf)
            'stbQ.Append("       Movimenti_dettagli.UDM_COD_EXTRA <> 0  -- confezionato       " + vbCrLf)
            'stbQ.Append("                " + vbCrLf)
            'stbQ.Append("      ) -- FINE CASO 2         " + vbCrLf)
            'stbQ.Append("                " + vbCrLf)
            'stbQ.Append("     OR  -- INIZIO CASO 3         " + vbCrLf)
            ''CASO 3
            ''il lotto non deve esistere nella tabella Trasformazioni
            'stbQ.Append("     (           " + vbCrLf)
            'stbQ.Append("      NOT EXISTS (     " + vbCrLf)
            'stbQ.Append("                   SELECT 1" + vbCrLf)
            'stbQ.Append("                   FROM Trasformazioni " + vbCrLf)
            'stbQ.Append("                   WHERE UPPER(Trasformazioni.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) " + vbCrLf)
            'stbQ.Append("                   ) " + vbCrLf)
            'stbQ.Append("                " + vbCrLf)
            'stbQ.Append("                " + vbCrLf)
            'stbQ.Append("      ) -- FINE CASO 3         " + vbCrLf)
            'stbQ.Append("                " + vbCrLf)
            'stbQ.Append("  ) -- AND INIZIALE              " + vbCrLf)
            'stbQ.Append("                " + vbCrLf)


            'modifica del 19/10/2012:
            stbQ.Append(" AND (            " & vbCrLf)

            '04/10/2013: RIFATTO IL CASO 1 UTILIZZANDO LA STESSA QUERY DEL GIASLAN!!!!!!!!!!!!!!!
            'stbQ.Append("  (   -- CASO 1         " + vbCrLf)
            ''                   MODIFICA DEL 02/04/2013: sostituito Data_Movimento con Ora (oltre alla data salva l'orario )
            'stbQ.Append("       Movimenti.Ora >= (SELECT TOP 1 movP.Ora  " + vbCrLf)
            ''stbQ.Append("       Movimenti.Data_Movimento >= (SELECT TOP 1 movP.Data_Movimento  " + vbCrLf)
            'stbQ.Append("                                   FROM  Agenda AgP " + vbCrLf)
            'stbQ.Append("                                   INNER JOIN  Movimenti movP ON AgP.PIVA = movP.PIVA AND AgP.Id_Agenda = movP.Id_Agenda  " + vbCrLf)
            'stbQ.Append("                                   INNER JOIN Movimenti_dettagli movdetP ON movP.PIVA = movdetP.PIVA AND movP.Id_Agenda = movdetP.Id_Agenda AND movP.Id_Mov = movdetP.Id_Mov  " + vbCrLf)
            ''                                               MODIFICA DEL 15/04/2013: per gestione caso Tenuta Casali:
            ''                                               se un prodotto viene passato a comemrcializzazione in più volte (prima una parte e poi un'altra)
            'stbQ.Append("                                  INNER JOIN OGenerazioni_Anagrafe_Log OGAL " & vbCrLf)
            'stbQ.Append("                                   ON OGAL.Piva_SuperUser = '" & Trim(objParametri_Server.PivaSuperUser) & "' ")
            'stbQ.Append("                                   AND OGAL.piva = movdetP.Piva" + vbCrLf)
            'stbQ.Append("                                   AND OGAL.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & "  " + vbCrLf)
            'stbQ.Append("                                   AND OGAL.elem_cod = movdetP.elem_cod " + vbCrLf)
            'stbQ.Append("                                   AND OGAL.mat_cod = movdetP.mat_cod " + vbCrLf)
            'stbQ.Append("                                   WHERE movdetP.Elem_Cod = Movimenti_dettagli.Elem_Cod " + vbCrLf)
            'stbQ.Append("                                   -- AND movdetP.Pro_Cod = Movimenti_dettagli.Pro_Cod " + vbCrLf)
            'stbQ.Append("                                   -- AND movdetP.Mat_Cod = Movimenti_dettagli.Mat_Cod " + vbCrLf)
            'stbQ.Append("                                   -- AND movdetP.udm_Cod = Movimenti_dettagli.udm_Cod " + vbCrLf)
            'stbQ.Append("                                   AND UPPER(movdetP.lotto) = UPPER(Movimenti_dettagli.lotto)   " + vbCrLf)
            ''anzichè fare la stessa query per ogni operazione
            ''controllo se preparazione cod è nella lista dei preparzione cod (che ho letto fuori dalla query del registro)
            'stbQ.Append("                                  AND AgP.Preparazione_Cod IN " & CStr(Lista_PrepCod) + vbCrLf)
            ''                                               MODIFICA DEL 15/04/2013: per gestione caso Tenuta Casali:
            ''                                               se un prodotto viene passato a comemrcializzazione in più volte (prima una parte e poi un'altra)
            'stbQ.Append("                                  AND OGAL.Codice_Generazione <= OGenerazioni_Anagrafe_Log.Codice_Generazione " & vbCrLf)
            ''stbQ.Append("                                   AND EXISTS ( SELECT 1 " + vbCrLf)
            ''stbQ.Append("                                               FROM Linee_Preparazioni " + vbCrLf)
            ''stbQ.Append("                                               WHERE Linee_Preparazioni.Preparazione_Cod = AgP.Preparazione_Cod " + vbCrLf)
            ''stbQ.Append("                                               AND Linee_Preparazioni.Piva = AgP.Piva " + vbCrLf)
            ''stbQ.Append("                                               AND Linee_Preparazioni.Modulo_Generazione = " & CStr(enum_Modulo_Generazione.Cantine) & " " + vbCrLf)
            ''stbQ.Append("                                               AND Linee_Preparazioni.Codice_Generazione = " & CStr(enum_Codice_Generazione_Preparazioni.Passaggio_RegVinificazione_Commercializzazione) & " " + vbCrLf)
            ''stbQ.Append("                                               ) " + vbCrLf)
            ''                                               MODIFICA PER MARTELLI: ha due passaggi, uno a gennaio, l'altro a febbraio
            ''                                               mettendo desc veniva letto febbraio e l'operazione di gennaio non veniva stampata
            ''stbQ.Append("                                   ORDER BY movP.Data_Movimento DESC  " + vbCrLf)
            'stbQ.Append("                                   ORDER BY movP.Data_Movimento ASC  " + vbCrLf)
            'stbQ.Append("                               ) " + vbCrLf)
            'stbQ.Append("      ) -- FINE CASO 1.  " + vbCrLf)

            stbQ.Append("  (   -- CASO 1         " & vbCrLf)
            stbQ.Append("       Movimenti.Ora >= (  SELECT TOP 1 Data  " & vbCrLf)
            stbQ.Append("                           FROM ( " & vbCrLf)
            stbQ.Append(SQL_Operazioni_PassaggioACommercializzazione(Piva,
                                                      Lista_PrepCod,
                                                      "",
                                                      " UPPER(Trasformazioni2.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ",
                                                      " UPPER(Trasformazioni2.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ",
                                                      " UPPER(Trasformazioni.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) "
                                                      ))
            'stbQ.Append(SQL_Passaggi_DaARegistro_DaALotto_Completa(Piva, _
            '                                                Lista_PrepCod, _
            '                                                Lista_IdTrasf_NoComm, _
            '                                                "", _
            '                                                " UPPER(Trasformazioni.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ", _
            '                                                " UPPER(Trasformazioni1.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ", _
            '                                                " UPPER(Trasformazioni2.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ", _
            '                                                " UPPER(Trasformazioni.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ", _
            '                                                ""))
            stbQ.Append("                                   ) AS PASSAGGIO" & vbCrLf)
            'stbQ.Append("                           ORDER BY Tipo_Default, Data " + vbCrLf)
            stbQ.Append("                           ORDER BY Data " & vbCrLf)
            stbQ.Append("                           ) " & vbCrLf)
            stbQ.Append("      ) -- FINE CASO 1.  " & vbCrLf)

            'CASO 2
            stbQ.Append("     OR  -- INIZIO CASO 2         " & vbCrLf)
            '2. sia un confezionato/condizionato (udm_cod_extra <> 0)
            stbQ.Append("     (           " & vbCrLf)
            'MODIFICA DEL 4/10/2013: si verificava UDM_COD_EXTRA in Movimenti_dettagli e non materie_prime
            stbQ.Append("       Materie_Prime.UDM_COD_EXTRA <> 0   -- confezionato       " & vbCrLf)
            stbQ.Append("                " & vbCrLf)
            stbQ.Append("      ) -- FINE CASO 2         " & vbCrLf)
            stbQ.Append("                " & vbCrLf)

            'CASO 3
            stbQ.Append("     OR  -- INIZIO CASO 3         " & vbCrLf)
            'il lotto non deve esistere nella tabella Trasformazioni
            '--> quando si farà la storicizzazione, questo pezzo non servirà più
            '(è per chi non ha l'omni??)
            stbQ.Append("     (           " & vbCrLf)
            stbQ.Append("      NOT EXISTS (     " & vbCrLf)
            stbQ.Append("                   SELECT 1" & vbCrLf)
            stbQ.Append("                   FROM Trasformazioni " & vbCrLf)
            stbQ.Append("                   WHERE UPPER(Trasformazioni.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) " & vbCrLf)
            stbQ.Append("                   AND UPPER(Trasformazioni.Trasformazione_Des) <> 'INDEFINITO'    " & vbCrLf)
            stbQ.Append("                " & vbCrLf)
            stbQ.Append("                   ) " & vbCrLf)
            stbQ.Append("                " & vbCrLf)
            stbQ.Append("                " & vbCrLf)
            stbQ.Append("      ) -- FINE CASO 3         " & vbCrLf)

            'CASO 4
            'oppure è un calo o uno scarto di lavorazione (da codice si verifica poi se deve essere stampato o meno)
            'inserito il 27/05/2015 (per gestire il caso di calo imbottigliamento con lotto stringa vuota)
            stbQ.Append("     OR  -- INIZIO CASO 4         " & vbCrLf)
            stbQ.Append("     (           " & vbCrLf)
            stbQ.Append("       Materie_Prime.elem_cod = -1   -- calo/scarto       " & vbCrLf)
            stbQ.Append("                " & vbCrLf)
            stbQ.Append("      ) -- FINE CASO 4         " & vbCrLf)
            stbQ.Append("                " & vbCrLf)


            stbQ.Append("                " & vbCrLf)
            stbQ.Append("  ) -- AND INIZIALE              " & vbCrLf)
            stbQ.Append("                " & vbCrLf)

        End If


        FiltroSQL = stbQ.ToString

        Return FiltroSQL

    End Function



    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' registro di commercializzazione
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    ''' ' ByVal Filtro_CodRisum_XCTerzi As String, _
    Public Function RegistroCommercializzazione(ByRef DataSet2Fill As DataSet,
                                                ByVal strNomeDtNelDS As String,
                                                ByVal Piva As String,
                                                ByVal Id_Report As Integer,
                                                ByVal DataReportInizio As Date,
                                                ByVal DataReportFine As Date,
                                                ByVal Cod_Contatto_Terzi As String,
                                                ByVal Opt_Gestione_RegistroVinificazione As Integer,
                                                ByVal Lista_PrepCod As String,
                                                ByVal Lista_IdTrasf_NoComm As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Id_Destinazione As Integer,
                                                ByVal Cal_Cod As Integer,
                                                ByVal Mat_Cod As Integer,
                                                ByVal Linea_Cod As Integer,
                                                ByVal Cau_Mov As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable
        ') As Boolean

        'ByVal DataInizio As Date, _
        'ByVal DataFine As Date, _

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.RegistriCantina.RegistroCommercializzazione"

        Dim MessaggioErrore As String = ""
        Dim stbQ As New System.Text.StringBuilder
        'Dim Risp As Boolean = False
        Dim DT As DataTable

        Try

            Dim DataFineControllo As Date
            DataFineControllo = DateAdd(DateInterval.Day, 1, DataReportFine)

            '=========================================================
            '--------------- 1) PREPARAZIONI ------------------
            '=========================================================


            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            'MODIFICA DEL 18/10/2013: lettura del campo ora (visto che si verifica quello nel controllo del passaggio a registro)
            'stbQ.Append(" SELECT Agenda.Id_Agenda, Movimenti.Data_Movimento, Agenda.des_lib, Linee_Preparazioni.Preparazione_Des AS Descrizione, " + vbCrLf)
            stbQ.Append(" SELECT Agenda.Id_Agenda, agenda.lav_cod, Movimenti.Ora AS Data_Movimento, Agenda.des_lib AS des_lib, Linee_Preparazioni.Preparazione_Des + ' del ' + CONVERT(VARCHAR(10), Movimenti.Data_Movimento, 103) AS Descrizione, " & vbCrLf)
            stbQ.Append(" ISNULL(Linee_Produzioni.Linea_Des + '-','')  + ISNULL(Trasformazione_Des,'') AS Designazione, Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_Cod, " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Lotto, " & vbCrLf)

            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV='7300' AND Movimenti_dettagli.Udm_Cod=29 THEN SUM(Movimenti_dettagli.Qta) " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV='7300' AND Movimenti_dettagli.Udm_Cod<>29 THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) ELSE 0 END AS CaricoLt, " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV='7300' AND Movimenti_dettagli.Udm_Cod<>29 THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) ELSE 0 END AS CaricoLt, " + vbCrLf)

            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=29 THEN SUM(Movimenti_dettagli.Qta)  " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=2 THEN SUM(Movimenti_dettagli.Qta) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti_dettagli.Udm_Cod<>2 AND Materie_Prime.Peso_Set>0 THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti_dettagli.Udm_Cod<>2 AND Materie_Prime.Peso_Set=0 THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) " & vbCrLf)
            stbQ.Append(" ELSE 0 END AS CaricoLt,    " & vbCrLf)

            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV='7350' AND Movimenti_dettagli.Udm_Cod=29 THEN SUM(Movimenti_dettagli.Qta) " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV='7350' AND Movimenti_dettagli.Udm_Cod<>29 THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) ELSE 0 END AS ScaricoLt, " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV='7350' AND Movimenti_dettagli.Udm_Cod<>29 THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) ELSE 0 END AS ScaricoLt, " + vbCrLf)

            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=29 THEN SUM(Movimenti_dettagli.Qta)  " & vbCrLf)
            stbQ.Append("  WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=2 THEN SUM(Movimenti_dettagli.Qta) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti_dettagli.Udm_Cod<>2 AND Materie_Prime.Peso_Set>0  THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti_dettagli.Udm_Cod<>2 AND Materie_Prime.Peso_Set=0  THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) " & vbCrLf)
            stbQ.Append(" ELSE 0 END AS ScaricoLt,   " & vbCrLf)

            stbQ.Append(" Movimenti_dettagli.Udm_Cod, UnitaMisura.Udm_Sim,  " & vbCrLf)


            'modifica del 12/02/2015 x Baldetti
            'visualizzato anche il numero di bottiglie nel nome del prodotto
            'stbQ.Append(" Materie_Prime.Mat_Des, " + vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti_dettagli.Udm_Cod = " & Agro_SQL_SaveNum(enum_UnitaMisura.Numero) & "  THEN ( " & vbCrLf)
            stbQ.Append(" -- UnitaMisura.udm_sim COLLATE SQL_Latin1_General_CP850_CI_AS + " & vbCrLf)
            stbQ.Append(" 'n.' " & vbCrLf)
            stbQ.Append(" + CONVERT(varchar(25),  Movimenti_dettagli.qta) " & vbCrLf)
            stbQ.Append(" + ' ' +  Materie_Prime.Mat_Des )  " & vbCrLf)
            stbQ.Append(" ELSE Materie_Prime.Mat_Des END AS Mat_Des, " & vbCrLf)

            stbQ.Append(" Materie_Prime.Cod_Articolo, '' AS NDoc " & vbCrLf)
            stbQ.Append(", Linee_PreparazionixReport.StrCampo_Registri " & vbCrLf)
            stbQ.Append(", Materie_Prime.Peso_Set, Movimenti_dettagli.QTA_EXTRA, Movimenti_dettagli.udm_cod_extra " & vbCrLf)

            'inserito il 15/10/2012 per stampare il numero di vasca
            stbQ.Append(", Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Sa_Cod AS SaCod_Dest, Mov_Destinazioni.Id_Destinazione AS Id_Dest " & vbCrLf)
            stbQ.Append(" , Linee_Preparazioni.Tipo_Integrazione   " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            stbQ.Append(" FROM Linee_PreparazionixReport " & vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Preparazioni ON Linee_PreparazionixReport.Piva = Linee_Preparazioni.Piva AND Linee_PreparazionixReport.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod  " & vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Linee_Preparazioni.Preparazione_Cod = Agenda.PREPARAZIONE_COD AND Linee_Preparazioni.Piva = Agenda.PIVA " & vbCrLf)
            stbQ.Append(" INNER JOIN Trasformazioni ON Trasformazioni.ID_trasformazione=Agenda.ID_Trasformazione AND  Trasformazioni.PIVA=Agenda.PIVA " & vbCrLf)

            'modifica del 27/04/2012 by Maga&Marco: mancava il join della piva
            ''left join per il condizionamento che non è legato a nessuna linea produzione
            'stbQ.Append(" LEFT JOIN Linee_Produzioni ON Agenda.Linea_Cod = Linee_Produzioni.Linea_Cod " + vbCrLf)
            stbQ.Append(" LEFT JOIN Linee_Produzioni ON Agenda.LINEA_COD = Linee_Produzioni.Linea_Cod AND Agenda.PIVA = Linee_Produzioni.Piva " & vbCrLf)

            stbQ.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod  " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov  " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod AND Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_dettagli.Udm_Cod " & vbCrLf)

            'inserito il 15/10/2012 per stampare il numero di vasca
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            '15/04/2013: inutile il left, ci sono due where sulla tabella
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " + vbCrLf)
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " + vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " & vbCrLf)

            'MODIFICA DEL 04/10/2013: non serve più, abbiamo cambiato l'algoritmo di lettura di passaggio
            'If Opt_Gestione_RegistroVinificazione = 1 Then
            '    stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva = Agenda.Piva" + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & "  " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.elem_cod = Movimenti_dettagli.elem_cod " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.mat_cod = Movimenti_dettagli.mat_cod " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.id =(             " + vbCrLf)
            '    stbQ.Append("                                   SELECT MAX(id) " + vbCrLf)
            '    stbQ.Append("                                    FROM OGenerazioni_Anagrafe_Log OLOG " + vbCrLf)
            '    stbQ.Append("                                    WHERE OLOG.piva_superuser = OGenerazioni_Anagrafe_Log.Piva_SuperUser " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.piva = OLOG.Piva " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = OLOG.Modulo_Generazione" + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.elem_cod = OLOG.elem_cod  " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.mat_cod = OLOG.mat_cod " + vbCrLf)
            '    stbQ.Append("                                   ) " + vbCrLf)
            'End If

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            stbQ.Append(" WHERE Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)

            stbQ.Append(" AND Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                                    "'" & CAU_SCARICO & "', " &
                                                    "'" & CAU_CONFERIMENTO & "', " &
                                                    "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                                    ") " & vbCrLf)

            stbQ.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            '--------------
            '19/06/2014: adeguato il filtro con la select (viaggia tutto sul campo Ora)
            'stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " " + vbCrLf)
            stbQ.Append(" AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " " & vbCrLf)
            stbQ.Append(" AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " " & vbCrLf)

            '--------------
            'aggiunto in data 23/09/2013
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = 'calibro' " & vbCrLf)
            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri = 1 " & vbCrLf)

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Id_Destinazione <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " & vbCrLf)
            End If
            If Cal_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime_ParametriQualitativi.tipo_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If
            If Linea_Cod <> 0 Then
                stbQ.Append(" AND Linee_Produzioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "  " & vbCrLf)
            End If
            If Cau_Mov <> "" Then
                stbQ.Append(" AND Movimenti.CAU_MOV = '" & Agro_SQL_SaveText(Cau_Mov) & "'" & vbCrLf)
            End If

            'aggiunta in data 25/09/2012:
            'stampa del registro separata per fornitore uve (Cod_Contatto_Terzi)
            If Cod_Contatto_Terzi <> "" Then
                'modifica del 12/08/2013
                'FILTRO C/TERZI
                stbQ.Append(RegistroCommercializzazione_FiltroSQL_ContoTerzi_ParteLinee(Piva, Cod_Contatto_Terzi) & vbCrLf)
            End If

            'correzione del 9/11/2010: mancavano i join con Movimenti_dettagli, per cui
            'se c'era almeno un dettaglio escluso per uno scarico, venivano esclusi tutti gli scarichi
            stbQ.Append(" AND NOT EXISTS (SELECT 1 FROM  Linee_Preparazioni_Report_Esclusi LPRE " & vbCrLf)
            stbQ.Append("                 INNER JOIN Linee_Preparazioni_Dettagli LPD ON LPRE.Piva = LPD.Piva AND LPRE.Preparazione_Cod = LPD.Preparazione_Cod AND LPRE.Dettaglio_Cod = LPD.Dettaglio_Cod " & vbCrLf)
            stbQ.Append("                 WHERE LPRE.Piva = Linee_Preparazioni.Piva " & vbCrLf)
            stbQ.Append("                 AND LPRE.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " & vbCrLf)
            stbQ.Append("                 AND LPRE.Id_Report = Linee_PreparazionixReport.Id_Report  " & vbCrLf)
            stbQ.Append("                 AND LPD.CAU_MOV = Movimenti.CAU_MOV " & vbCrLf)
            stbQ.Append("                 AND LPD.Elem_Cod = Movimenti_dettagli.Elem_Cod " & vbCrLf)
            'modifica del 05/10/2012: se la preparazione passaggio da registro di vinificazione a registro di commercializzazione
            'utilizzava 'risorse indefinita' come ingrediente e/o preparato
            'il join con Movimenti_dettagli non produceva alcun record e quindi venivano visualizzati entrambi i movimenti:
            'sia lo scarico dal reg vinificazione sia il carico al reg di commerc. e la qta si azzerava
            'INTRODOTTO L'OR per LPD.Pro_Cod - LPD.Mat_Cod - LPD.Udm_Cod 
            stbQ.Append("               AND ( " & vbCrLf)
            stbQ.Append("                       ( " & vbCrLf)
            stbQ.Append("                       LPD.Pro_Cod = Movimenti_dettagli.Pro_Cod " & vbCrLf)
            stbQ.Append("                       AND LPD.Mat_Cod = Movimenti_dettagli.Mat_Cod " & vbCrLf)
            stbQ.Append("                       AND LPD.Udm_Cod = Movimenti_dettagli.Udm_Cod   " & vbCrLf)
            stbQ.Append("                       ) OR ( " & vbCrLf)
            stbQ.Append("                       LPD.Pro_Cod = 0 AND LPD.Mat_Cod = 0 AND LPD.Udm_Cod = 0  " & vbCrLf)
            stbQ.Append("                       ) " & vbCrLf)
            stbQ.Append("                   ) " & vbCrLf)
            stbQ.Append("               ) " & vbCrLf)

            'FILTRO DA AGGIUNGERE NEL CASO DI GESTIONE DELLA VINIFICAZIONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            stbQ.Append(RegistroCommercializzazione_FiltroSQL_ConGestioneVinificazione(objParametri,
                                                                                        Opt_Gestione_RegistroVinificazione,
                                                                                        Lista_PrepCod,
                                                                                        Lista_IdTrasf_NoComm,
                                                                                        Piva))

            'MODIFICA DEL 18/10/2013: lettura del campo ora (visto che si verifica quello nel controllo del passaggio a registro)
            'stbQ.Append(" GROUP BY Agenda.Id_Agenda, Movimenti.Data_Movimento, Agenda.des_lib, Linee_Preparazioni.Preparazione_Des, Linee_Produzioni.Linea_Des, " + vbCrLf)
            stbQ.Append(" GROUP BY Agenda.Id_Agenda, agenda.lav_cod, Movimenti.Ora, Movimenti.Data_Movimento, Agenda.des_lib, Linee_Preparazioni.Preparazione_Des, Linee_Produzioni.Linea_Des, " & vbCrLf)
            stbQ.Append(" Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_Cod, Trasformazione_Des, " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Lotto, Movimenti_dettagli.Udm_Cod, UnitaMisura.Udm_Sim, Materie_Prime.Peso_Set, Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo " & vbCrLf)
            stbQ.Append(", Linee_PreparazionixReport.StrCampo_Registri, Movimenti_dettagli.QTA_EXTRA,Movimenti_dettagli.udm_cod_extra " & vbCrLf)
            stbQ.Append(", Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione,  Linee_Preparazioni.Tipo_Integrazione, Movimenti_dettagli.Qta    " & vbCrLf)

            '-------------------------------------------------------------------------------
            '-- ATTENZIONE! USARE L'UNION ALL, PERCHE' L'UNION FA IL DISTINCT! -------------
            '-------------------------------------------------------------------------------
            stbQ.Append("  " & vbCrLf)
            stbQ.Append(" UNION ALL " & vbCrLf)
            stbQ.Append("  " & vbCrLf)


            '=========================================================
            '--------------- 2) AGENDA ------------------
            '=========================================================

            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            'MODIFICA DEL 18/10/2013: lettura del campo ora (visto che si verifica quello nel controllo del passaggio a registro)
            'stbQ.Append(" SELECT Agenda.Id_Agenda, Movimenti.Data_Movimento, Agenda.des_lib, Agenda.des_lib AS Descrizione,'' AS Designazione, Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti_dettagli.Elem_Cod, " + vbCrLf)
            stbQ.Append(" SELECT Agenda.Id_Agenda, agenda.lav_cod, Movimenti.Ora AS Data_Movimento, Agenda.des_lib, Agenda.des_lib + ' del ' + CONVERT(VARCHAR(10), Movimenti.Data_Movimento, 103) AS Descrizione,'' AS Designazione, Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti_dettagli.Elem_Cod, " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Lotto, " & vbCrLf)

            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('7300','4100') AND Movimenti_dettagli.Udm_Cod=29 THEN Movimenti_dettagli.Qta " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('7300','4100') AND Movimenti_dettagli.Udm_Cod<>29 THEN Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra ELSE 0 END AS CaricoLt,  " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('7300','4100') AND Movimenti_dettagli.Udm_Cod<>29 THEN Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra ELSE 0 END AS CaricoLt,  " + vbCrLf)

            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=29 THEN Movimenti_dettagli.Qta  " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=2 THEN Movimenti_dettagli.Qta " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod NOT IN (29,2) AND Materie_Prime.Peso_Set>0 THEN Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod NOT IN (29,2) AND Materie_Prime.Peso_Set=0 THEN Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra " & vbCrLf)
            stbQ.Append(" ELSE 0 END AS CaricoLt,    " & vbCrLf)

            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('7350','4200') AND Movimenti_dettagli.Udm_Cod=29 THEN Movimenti_dettagli.Qta " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('7350','4200') AND Movimenti_dettagli.Udm_Cod<>29 THEN Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra ELSE 0 END AS ScaricoLt, " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('7350','4200') AND Movimenti_dettagli.Udm_Cod<>29 THEN Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra ELSE 0 END AS ScaricoLt, " + vbCrLf)

            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=29 THEN Movimenti_dettagli.Qta  " & vbCrLf)

            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=2 THEN Movimenti_dettagli.Qta " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod NOT IN (29,2) AND Materie_Prime.Peso_Set>0  THEN Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod NOT IN (29,2) AND Materie_Prime.Peso_Set=0  THEN Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra " & vbCrLf)
            stbQ.Append(" ELSE 0 END AS ScaricoLt,   " & vbCrLf)

            stbQ.Append(" Movimenti_dettagli.Udm_Cod, UnitaMisura.UDM_SIM, " & vbCrLf)

            'modifica del 12/02/2015 x Baldetti
            'visualizzato anche il numero di bottiglie nel nome del prodotto
            'stbQ.Append(" Materie_Prime.Mat_Des, " + vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti_dettagli.Udm_Cod = " & Agro_SQL_SaveNum(enum_UnitaMisura.Numero) & "  THEN ( " & vbCrLf)
            stbQ.Append(" -- UnitaMisura.udm_sim COLLATE SQL_Latin1_General_CP850_CI_AS + " & vbCrLf)
            stbQ.Append(" 'n.' " & vbCrLf)
            stbQ.Append(" + CONVERT(varchar(25),  Movimenti_dettagli.qta) " & vbCrLf)
            stbQ.Append(" + ' ' +  Materie_Prime.Mat_Des )  " & vbCrLf)
            stbQ.Append(" ELSE Materie_Prime.Mat_Des END AS Mat_Des, " & vbCrLf)

            stbQ.Append(" Materie_Prime.Cod_Articolo, " & vbCrLf)
            'stbQ.Append(" (SELECT Doc_Numero_Sin + ' ' + CAST(Doc_Numero AS Varchar(100)) + ' ' + doc_numero_des FROM Movimenti MovContabili WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda and cau_mov='4000') AS NDoc " + vbCrLf)
            stbQ.Append(" ISNULL((SELECT Doc_Numero_Sin + CAST(Doc_Numero AS Varchar(500)) + doc_numero_des FROM Movimenti MovContabili WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda and cau_mov='4000'),'') AS NDoc " & vbCrLf)
            stbQ.Append(", '' AS StrCampo_Registri " & vbCrLf)
            stbQ.Append(", Materie_Prime.Peso_Set, Movimenti_dettagli.QTA_EXTRA, Movimenti_dettagli.udm_cod_extra " & vbCrLf)

            'inserito il 15/10/2012 per stampare il numero di vasca
            stbQ.Append(", Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Sa_Cod AS SaCod_Dest, Mov_Destinazioni.Id_Destinazione AS Id_Dest " & vbCrLf)
            stbQ.Append(" , 0 AS Tipo_Integrazione   " & vbCrLf)

            ' Movimenti.Doc_Numero_Sin + CAST(Movimenti.Doc_Numero AS varchar(10)) + Doc_Numero_Des AS NDoc  " + vbcrlf)

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            stbQ.Append(" FROM  Materie_PrimexReport " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Materie_PrimexReport.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND Movimenti.Id_Agenda = Agenda.Id_Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD " & vbCrLf)

            'inserito il 15/10/2012 per stampare il numero di vasca
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            '15/04/2013: inutile il left, ci sono due where sulla tabella
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " + vbCrLf)
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " + vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " & vbCrLf)

            'MODIFICA DEL 04/10/2013: non serve più, abbiamo cambiato l'algoritmo di lettura di passaggio
            'If Opt_Gestione_RegistroVinificazione = 1 Then
            '    stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva = Agenda.Piva" + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & "  " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.elem_cod = Movimenti_dettagli.elem_cod " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.mat_cod = Movimenti_dettagli.mat_cod " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.id =(             " + vbCrLf)
            '    stbQ.Append("                                   SELECT MAX(id) " + vbCrLf)
            '    stbQ.Append("                                    FROM OGenerazioni_Anagrafe_Log OLOG " + vbCrLf)
            '    stbQ.Append("                                    WHERE OLOG.piva_superuser = OGenerazioni_Anagrafe_Log.Piva_SuperUser " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.piva = OLOG.Piva " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = OLOG.Modulo_Generazione" + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.elem_cod = OLOG.elem_cod  " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.mat_cod = OLOG.mat_cod " + vbCrLf)
            '    stbQ.Append("                                   ) " + vbCrLf)
            'End If

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            ''stbQ.Append(" WHERE Movimenti.CAU_MOV IN ('7300', '7350', '4100', '4200') " + vbcrlf)
            'stbQ.Append(" WHERE Movimenti.CAU_MOV IN ('7300', '7350') " + vbCrLf)
            stbQ.Append(" WHERE Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                                        "'" & CAU_SCARICO & "', " &
                                                        "'" & CAU_CONFERIMENTO & "', " &
                                                        "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                                        ") " & vbCrLf)

            '--------------
            '19/06/2014: adeguato il filtro con la select (viaggia tutto sul campo Ora)
            'stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " " + vbCrLf)
            stbQ.Append(" AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " " & vbCrLf)
            stbQ.Append(" AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " " & vbCrLf)

            stbQ.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            'stbQ.Append(" AND EXISTS (SELECT 1 FROM Agenda Agenda_Lav INNER JOIN OperazionixReport OPR ON Agenda_Lav.Lav_Cod = OPR.Lav_Cod " + vbCrLf)
            'stbQ.Append("             AND OPR.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " + vbCrLf)
            'stbQ.Append("             WHERE  Agenda.Piva = Agenda_Lav.Piva AND Agenda.Id_Agenda = Agenda_Lav.Id_Agenda ) " + vbCrLf)
            'CORREZIONE DEL 07/05/2014: la piva di OperazionixReport è quella del superuser,
            'non va quindi messa in join con la piva di agenda!
            'è stato scoperto ora perchè qualitoscana è il primo cliente col quale si stampano i registri di cantina sulle aziende figlie in gerarchia
            stbQ.Append(" AND EXISTS (SELECT 1 FROM OperazionixReport OPR  " & vbCrLf)
            stbQ.Append("             WHERE  OPR.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append("             AND OPR.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)
            stbQ.Append("               AND Agenda.Lav_Cod = OPR.Lav_Cod ) " & vbCrLf)

            stbQ.Append(" AND Materie_PrimexReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)

            '--------------
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)

            'aggiunta il 19/08/13:per escludere carichi/scarichi di operazioni pendenti (pianificate)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = 'calibro' " & vbCrLf)
            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri = 1 " & vbCrLf)

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Id_Destinazione <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " & vbCrLf)
            End If
            If Cal_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime_ParametriQualitativi.tipo_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If
            If Linea_Cod <> 0 Then
                stbQ.Append("   AND  EXISTS (SELECT 1  " & vbCrLf)
                stbQ.Append("               FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
                stbQ.Append(" 				WHERE materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod " & vbCrLf)
                stbQ.Append(" 				AND materie_prime.elem_cod = OGenerazioni_Anagrafe_Log.elem_cod " & vbCrLf)
                stbQ.Append("               AND OGenerazioni_Anagrafe_Log.linea_cod = " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
                stbQ.Append("               ) " & vbCrLf)
            End If
            If Cau_Mov <> "" Then
                stbQ.Append(" AND Movimenti.CAU_MOV = '" & Agro_SQL_SaveText(Cau_Mov) & "'" & vbCrLf)
            End If

            'aggiunta in data 25/09/2012:
            'stampa del registro separata per fornitore uve (Cod_Contatto_Terzi)
            If Cod_Contatto_Terzi <> "" Then
                'modifica del 12/08/2013
                'FILTRO C/TERZI
                stbQ.Append(RegistroCommercializzazione_FiltroSQL_ContoTerzi_ParteAgenda(objParametri.PivaSuperUser, Piva, Cod_Contatto_Terzi) & vbCrLf)
            End If

            'FILTRO DA AGGIUNGERE NEL CASO DI GESTIONE DELLA VINIFICAZIONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            stbQ.Append(RegistroCommercializzazione_FiltroSQL_ConGestioneVinificazione(objParametri,
                                                                                        Opt_Gestione_RegistroVinificazione,
                                                                                        Lista_PrepCod,
                                                                                        Lista_IdTrasf_NoComm,
                                                                                        Piva))

            'stbQ.Append(" ORDER BY  Movimenti.Data_Movimento, Agenda.Id_Agenda " + vbCrLf)
            'modifica del 02/05/2011: richiesta ordinamento per data e numero documento
            'stbQ.Append(" ORDER BY  Movimenti.Data_Movimento, NDoc, Agenda.Id_Agenda " + vbCrLf)
            'MODIFICA DEL 18/10/2013: lettura del campo ora (visto che si verifica quello nel controllo del passaggio a registro)
            'modifica del 15/10/2014: aggiunto Elem_Cod DESC per avere i cali per ultimi
            stbQ.Append(" ORDER BY  Movimenti.Ora, NDoc, Agenda.Id_Agenda,  Movimenti.CAU_MOV DESC, Movimenti_dettagli.Elem_Cod DESC " & vbCrLf)


            'modifica del 14/10/2014: non carico il dataset, ma ritorno il datatable
            ''--------------------------------------------------------------------------
            'Risp = EseguiQuery_Lettura(objParametri, stbQ.ToString, NomeRoutine, DataSet2Fill, strNomeDtNelDS)
            ''--------------------------------------------------------------------------
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            ' Risp = False
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        'Return Risp
        Return DT

    End Function

    'DATA_REPORT_INIZIO = 01/MESE/ANNO
    'DATA_REPORT_FINE = FINE/MESE/ANNO

    'DATA_RIEPILOGO_INIZIO = 01/01/1900
    'DATA_RIEPILOGO_FINE = DATA_REPORT_FINE

    'DATA FINE CONTROLLO = DATA_REPORT_FINE + 1 GIORNO

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' registro di commercializzazione: riepilogo
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    '''  ByVal Filtro_CodRisum_XCTerzi As String, _
    Public Function RegistroCommercializzazioneRiepilogo(ByVal Piva As String,
                                                        ByVal Id_Report As Integer,
                                                        ByVal DataRiepilogoInizio As Date,
                                                        ByVal DataReportInizio As Date,
                                                        ByVal DataReportFine As Date,
                                                        ByVal Cod_Contatto_Terzi As String,
                                                        ByVal Opt_Gestione_RegistroVinificazione As Integer,
                                                        ByVal Lista_PrepCod As String,
                                                        ByVal Lista_IdTrasf_NoComm As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal Id_Destinazione As Integer,
                                                        ByVal Cal_Cod As Integer,
                                                        ByVal Mat_Cod As Integer,
                                                        ByVal Linea_Cod As Integer,
                                                        ByVal Cau_Mov As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        'ByVal DataRiepilogoFine As Date, _

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.RegistriCantina.RegistroCommercializzazioneRiepilogo"

        Dim MessaggioErrore As String = ""
        Dim stbQ As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim DataFineControllo As Date
            DataFineControllo = DateAdd(DateInterval.Day, 1, DataReportFine)


            stbQ.Length = 0

            stbQ.Append(" ( " & vbCrLf)
            stbQ.Append(" SELECT Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Movimenti_Dettagli.Lotto, Movimenti_Dettagli.Udm_cod,   " & vbCrLf)

            'nel registro di commercializzazione si considera il peso_Set
            'MODIFICA DEL 21/08/14: allineato case alla gestione del campo ORA
            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " THEN SUM(Movimenti_dettagli.Qta)  " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " AND Materie_Prime.Peso_Set>0 THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " AND Materie_Prime.Peso_Set=0 THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) " + vbCrLf)
            'stbQ.Append(" ELSE 0 END AS CaricoLt,    " + vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta)  " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=2 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta)  " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod NOT IN (29,2) AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " AND Materie_Prime.Peso_Set>0 THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod NOT IN (29,2)AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " AND Materie_Prime.Peso_Set=0 THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) " & vbCrLf)
            stbQ.Append(" ELSE 0 END AS CaricoLt,    " & vbCrLf)
            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " THEN SUM(Movimenti_dettagli.Qta)  " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " AND Materie_Prime.Peso_Set>0  THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " AND Materie_Prime.Peso_Set=0  THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) " + vbCrLf)
            'stbQ.Append(" ELSE 0 END AS ScaricoLt,   " + vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta)  " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=2 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta)  " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " AND Materie_Prime.Peso_Set>0  THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " AND Materie_Prime.Peso_Set=0  THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) " & vbCrLf)
            stbQ.Append(" ELSE 0 END AS ScaricoLt,   " & vbCrLf)

            stbQ.Append(" ISNULL(Materie_Prime_Calibri.Cal_Cod, 0) AS Cal_Cod, isnull(Materie_Prime_Calibri.Cal_Des, '') as Cal_Des,  " & vbCrLf)

            'MODIFICA DEL 19/10/2012:
            'ottimizzata: leggo cifra_start cifra_end e Lotto_Cod direttamente in una query
            stbQ.Append("                ISNULL((SELECT TOP 1   " & vbCrLf)
            stbQ.Append("                ( CONVERT(varchar(250), Materie_PrimexLotto_Configurazione.Cifra_Start) + '|' + " & vbCrLf)
            stbQ.Append("                CONVERT(varchar(250), Materie_PrimexLotto_Configurazione.Cifra_End) + '|' + " & vbCrLf)
            stbQ.Append("                CONVERT(varchar(250), Materie_PrimexLotto_Configurazione.Lotto_Cod) ) AS ConfigLotto " & vbCrLf)
            stbQ.Append("                FROM Lotto_Configurazione " & vbCrLf)
            stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser   " & vbCrLf)
            stbQ.Append("                AND Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod   " & vbCrLf)
            stbQ.Append("                AND Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva   " & vbCrLf)
            stbQ.Append("                AND Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " & vbCrLf)
            'MODIFICA DEL 26/11/2012: non veniva stampato l'anno per i semilavorati
            'stbQ.Append("                WHERE (Lotto_Des like '%Anno%produzione%' OR Lotto_Des_Estesa like '%Anno%produzione%' )  " + vbCrLf)
            stbQ.Append("                WHERE (Lotto_Des like '%Anno%produzione%' OR Lotto_Des like '%Anno%vendemmia%'  " & vbCrLf)
            stbQ.Append("                       OR Lotto_Des_Estesa like '%Anno%produzione%' OR Lotto_Des_Estesa like '%Anno%vendemmia%')  " & vbCrLf)
            stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " & vbCrLf)
            stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " & vbCrLf)
            stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),'0|0|0') AS ConfigLotto " & vbCrLf)

            ''MODIFICA GESTIONE CIFRA_START E CIFRA_END DEL 21/02/2012
            'stbQ.Append("                ISNULL((SELECT TOP 1 Materie_PrimexLotto_Configurazione.Cifra_Start  " + vbCrLf)
            'stbQ.Append("                FROM Lotto_Configurazione " + vbCrLf)
            'stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " + vbCrLf)
            'stbQ.Append("                WHERE Lotto_Des like '%Anno%produzione%'  " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),0) AS Cifra_Start, " + vbCrLf)

            'stbQ.Append("                ISNULL((SELECT TOP 1 Materie_PrimexLotto_Configurazione.Cifra_End " + vbCrLf)
            'stbQ.Append("                FROM Lotto_Configurazione " + vbCrLf)
            'stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " + vbCrLf)
            'stbQ.Append("                WHERE Lotto_Des like '%Anno%produzione%'  " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),0) AS Cifra_End, " + vbCrLf)

            'stbQ.Append("                ISNULL((SELECT TOP 1 Materie_PrimexLotto_Configurazione.Lotto_Cod " + vbCrLf)
            'stbQ.Append("                FROM Lotto_Configurazione " + vbCrLf)
            'stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " + vbCrLf)
            'stbQ.Append("                WHERE Lotto_Des like '%Anno%produzione%'  " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),0) AS Lotto_Cod " + vbCrLf)

            stbQ.Append(" FROM Materie_PrimexReport  INNER JOIN Materie_Prime ON Materie_PrimexReport.Mat_Cod = Materie_Prime.Mat_Cod   " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod   " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD   " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov   " & vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND Movimenti.Id_Agenda = Agenda.Id_Agenda   " & vbCrLf)

            'inserito il 10/04/2013 x filtro id_dest
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            '15/04/2013: inutile il left, ci sono due where sulla tabella
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " + vbCrLf)
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " + vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " & vbCrLf)

            'MODIFICA DEL 04/10/2013: non serve più, abbiamo cambiato l'algoritmo di lettura di passaggio
            'If Opt_Gestione_RegistroVinificazione = 1 Then
            '    stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva = Agenda.Piva" + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & "  " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.elem_cod = Movimenti_dettagli.elem_cod " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.mat_cod = Movimenti_dettagli.mat_cod " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.id =(             " + vbCrLf)
            '    stbQ.Append("                                   SELECT MAX(id) " + vbCrLf)
            '    stbQ.Append("                                    FROM OGenerazioni_Anagrafe_Log OLOG " + vbCrLf)
            '    stbQ.Append("                                    WHERE OLOG.piva_superuser = OGenerazioni_Anagrafe_Log.Piva_SuperUser " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.piva = OLOG.Piva " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = OLOG.Modulo_Generazione" + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.elem_cod = OLOG.elem_cod  " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.mat_cod = OLOG.mat_cod " + vbCrLf)
            '    stbQ.Append("                                   ) " + vbCrLf)
            'End If

            ' stbQ.Append(" WHERE Movimenti.CAU_MOV IN ('" & CStr(CAU_CARICO) & "', '" & CStr(CAU_SCARICO) & "') " + vbCrLf)
            stbQ.Append(" WHERE Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                                        "'" & CAU_SCARICO & "', " &
                                                        "'" & CAU_CONFERIMENTO & "', " &
                                                        "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                                        ") " & vbCrLf)

            stbQ.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            'CORREZIONE DEL 07/05/2014: la piva di OperazionixReport è quella del superuser,
            'non va quindi messa in join con la piva di agenda!
            'è stato scoperto ora perchè qualitoscana è il primo cliente col quale si stampano i registri di cantina sulle aziende figlie in gerarchia
            'stbQ.Append(" AND EXISTS (SELECT 1 FROM Agenda Agenda_Lav  " + vbCrLf)
            'stbQ.Append("               INNER JOIN OperazionixReport OPR  " + vbCrLf)
            'stbQ.Append("               ON Agenda_Lav.Lav_Cod = OPR.Lav_Cod " + vbCrLf)
            'stbQ.Append("             AND OPR.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " + vbCrLf)
            'stbQ.Append("             WHERE  Agenda.Piva = Agenda_Lav.Piva " + vbCrLf)
            'stbQ.Append("               AND Agenda.Id_Agenda = Agenda_Lav.Id_Agenda ) " + vbCrLf)
            stbQ.Append(" AND EXISTS (SELECT 1 FROM OperazionixReport OPR  " & vbCrLf)
            stbQ.Append("             WHERE  OPR.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append("             AND OPR.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)
            stbQ.Append("               AND Agenda.Lav_Cod = OPR.Lav_Cod ) " & vbCrLf)

            stbQ.Append(" AND Materie_PrimexReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)

            'stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " " + vbCrLf)

            'nel riepilogo non ci vanno solo i prodotti movimentati nel mese

            '--------------
            '19/06/2014: adeguato il filtro con la select (viaggia tutto sul campo Ora)
            'stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataRiepilogoInizio) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " " + vbCrLf)
            stbQ.Append(" AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataRiepilogoInizio) & " " & vbCrLf)
            stbQ.Append(" AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " " & vbCrLf)


            ' stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataMeseInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataMeseFine) & " " + vbCrLf)

            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = 'calibro' " & vbCrLf)
            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri = 1 " & vbCrLf)

            '--------------
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)

            'aggiunta il 19/08/13:per escludere carichi/scarichi di operazioni pendenti (pianificate)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Id_Destinazione <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " & vbCrLf)
            End If
            If Cal_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime_ParametriQualitativi.tipo_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If
            If Linea_Cod <> 0 Then
                stbQ.Append("   AND  EXISTS (SELECT 1  " & vbCrLf)
                stbQ.Append("               FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
                stbQ.Append(" 				WHERE materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod " & vbCrLf)
                stbQ.Append(" 				AND materie_prime.elem_cod = OGenerazioni_Anagrafe_Log.elem_cod " & vbCrLf)
                stbQ.Append("               AND OGenerazioni_Anagrafe_Log.linea_cod = " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
                stbQ.Append("               ) " & vbCrLf)
            End If
            If Cau_Mov <> "" Then
                stbQ.Append(" AND Movimenti.CAU_MOV = '" & Agro_SQL_SaveText(Cau_Mov) & "'" & vbCrLf)
            End If
            'aggiunta in data 25/09/2012:
            'stampa del registro separata per fornitore uve (Cod_Contatto_Terzi)
            If Cod_Contatto_Terzi <> "" Then
                'modifica del 12/08/2013
                'FILTRO C/TERZI
                stbQ.Append(RegistroCommercializzazione_FiltroSQL_ContoTerzi_ParteAgenda(objParametri.PivaSuperUser, Piva, Cod_Contatto_Terzi) & vbCrLf)
            End If

            'FILTRO DA AGGIUNGERE NEL CASO DI GESTIONE DELLA VINIFICAZIONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            stbQ.Append(RegistroCommercializzazione_FiltroSQL_ConGestioneVinificazione(objParametri,
                                                                                        Opt_Gestione_RegistroVinificazione,
                                                                                        Lista_PrepCod,
                                                                                        Lista_IdTrasf_NoComm,
                                                                                        Piva))


            'MODIFICA DEL 21/08/14: allineato case alla gestione del campo ORA
            'stbQ.Append(" GROUP BY Materie_Prime.Piva, Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Movimenti_Dettagli.Lotto,  Movimenti.CAU_MOV, Movimenti_dettagli.Udm_Cod, Materie_Prime.Peso_Set, Materie_Prime_Calibri.Cal_Cod, Materie_Prime_Calibri.Cal_Des, Movimenti.Data_Movimento " + vbCrLf)
            stbQ.Append(" GROUP BY Materie_Prime.Piva, Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Movimenti_Dettagli.Lotto,  Movimenti.CAU_MOV, Movimenti_dettagli.Udm_Cod, Materie_Prime.Peso_Set, Materie_Prime_Calibri.Cal_Cod, Materie_Prime_Calibri.Cal_Des, Movimenti.Ora " & vbCrLf)
            stbQ.Append(" ) " & vbCrLf)


            '-------------------------------------------------------------------------------
            '-- ATTENZIONE! USARE L'UNION ALL, PERCHE' L'UNION FA IL DISTINCT! -------------
            '-------------------------------------------------------------------------------
            stbQ.Append("  " & vbCrLf)
            stbQ.Append(" UNION ALL " & vbCrLf)
            stbQ.Append("  " & vbCrLf)

            stbQ.Append(" ( " & vbCrLf)
            stbQ.Append(" SELECT Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Movimenti_Dettagli.Lotto, Movimenti_Dettagli.Udm_cod,  " & vbCrLf)

            'MODIFICA DEL 21/08/14: allineato case alla gestione del campo ORA
            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " THEN SUM(Movimenti_dettagli.Qta)  " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " AND Materie_Prime.Peso_Set>0 THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " AND Materie_Prime.Peso_Set=0 THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) " + vbCrLf)
            'stbQ.Append(" ELSE 0 END AS CaricoLt,    " + vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta)  " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=2 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta)  " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod NOT IN (2,29) AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " AND Materie_Prime.Peso_Set>0 THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod NOT IN (2,29) AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " AND Materie_Prime.Peso_Set=0 THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) " & vbCrLf)
            stbQ.Append(" ELSE 0 END AS CaricoLt,    " & vbCrLf)

            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " THEN SUM(Movimenti_dettagli.Qta)  " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " AND Materie_Prime.Peso_Set>0  THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) " + vbCrLf)
            'stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod<>29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " AND Materie_Prime.Peso_Set=0  THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) " + vbCrLf)
            'stbQ.Append(" ELSE 0 END AS ScaricoLt,   " + vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta)  " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=2 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta)  " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod NOT IN (2,29) AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " AND Materie_Prime.Peso_Set>0  THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod NOT IN (2,29) AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " AND Materie_Prime.Peso_Set=0  THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) " & vbCrLf)
            stbQ.Append(" ELSE 0 END AS ScaricoLt,   " & vbCrLf)

            stbQ.Append(" ISNULL(Materie_Prime_Calibri.Cal_Cod, 0) AS Cal_Cod, isnull(Materie_Prime_Calibri.Cal_Des, '') as Cal_Des,  " & vbCrLf)

            'MODIFICA DEL 19/10/2012:
            'ottimizzata: leggo cifra_start cifra_end e Lotto_Cod direttamente in una query
            stbQ.Append("                ISNULL((SELECT TOP 1   " & vbCrLf)
            stbQ.Append("                ( CONVERT(varchar(250), Materie_PrimexLotto_Configurazione.Cifra_Start) + '|' + " & vbCrLf)
            stbQ.Append("                CONVERT(varchar(250), Materie_PrimexLotto_Configurazione.Cifra_End) + '|' + " & vbCrLf)
            stbQ.Append("                CONVERT(varchar(250), Materie_PrimexLotto_Configurazione.Lotto_Cod) ) AS ConfigLotto " & vbCrLf)
            stbQ.Append("                FROM Lotto_Configurazione " & vbCrLf)
            stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser   " & vbCrLf)
            stbQ.Append("                AND Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod   " & vbCrLf)
            stbQ.Append("                AND Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva   " & vbCrLf)
            stbQ.Append("                AND Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " & vbCrLf)
            'MODIFICA DEL 26/11/2012: non veniva stampato l'anno per i semilavorati
            'stbQ.Append("                WHERE (Lotto_Des like '%Anno%produzione%' OR Lotto_Des_Estesa like '%Anno%produzione%' )  " + vbCrLf)
            stbQ.Append("                WHERE (Lotto_Des like '%Anno%produzione%' OR Lotto_Des like '%Anno%vendemmia%'  " & vbCrLf)
            stbQ.Append("                       OR Lotto_Des_Estesa like '%Anno%produzione%' OR Lotto_Des_Estesa like '%Anno%vendemmia%')  " & vbCrLf)
            stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " & vbCrLf)
            stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " & vbCrLf)
            stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),'0|0|0') AS ConfigLotto " & vbCrLf)

            ''MODIFICA GESTIONE CIFRA_START E CIFRA_END DEL 21/02/2012
            'stbQ.Append("                ISNULL((SELECT TOP 1 Materie_PrimexLotto_Configurazione.Cifra_Start  " + vbCrLf)
            'stbQ.Append("                FROM Lotto_Configurazione " + vbCrLf)
            'stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " + vbCrLf)
            'stbQ.Append("                WHERE Lotto_Des like '%Anno%produzione%'  " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),0) AS Cifra_Start, " + vbCrLf)

            'stbQ.Append("                ISNULL((SELECT TOP 1 Materie_PrimexLotto_Configurazione.Cifra_End " + vbCrLf)
            'stbQ.Append("                FROM Lotto_Configurazione " + vbCrLf)
            'stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " + vbCrLf)
            'stbQ.Append("                WHERE Lotto_Des like '%Anno%produzione%'  " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),0) AS Cifra_End, " + vbCrLf)

            'stbQ.Append("                ISNULL((SELECT TOP 1 Materie_PrimexLotto_Configurazione.Lotto_Cod " + vbCrLf)
            'stbQ.Append("                FROM Lotto_Configurazione " + vbCrLf)
            'stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " + vbCrLf)
            'stbQ.Append("                WHERE Lotto_Des like '%Anno%produzione%'  " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),0) AS Lotto_Cod " + vbCrLf)

            stbQ.Append(" FROM Linee_PreparazionixReport " & vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Preparazioni ON Linee_PreparazionixReport.Piva = Linee_Preparazioni.Piva AND Linee_PreparazionixReport.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Linee_Preparazioni.Preparazione_Cod = Agenda.PREPARAZIONE_COD AND Linee_Preparazioni.Piva = Agenda.PIVA " & vbCrLf)
            'stbQ.Append(" INNER JOIN Trasformazioni ON Trasformazioni.ID_trasformazione=Agenda.ID_Trasformazione AND  Trasformazioni.PIVA=Agenda.PIVA " + vbCrLf)
            ' stbQ.Append(" INNER JOIN Trasformazioni ON Linee_Produzioni.Linea_Cod = Trasformazioni.Linea_Cod " + vbCrLf)

            'modifica del 27/04/2012 by Maga&Marco: mancava il join della piva
            ''left join per il condizionamento che non è legato a nessuna linea produzione
            'stbQ.Append(" LEFT JOIN Linee_Produzioni ON Agenda.Linea_Cod = Linee_Produzioni.Linea_Cod " + vbCrLf)
            stbQ.Append(" LEFT JOIN Linee_Produzioni ON Agenda.LINEA_COD = Linee_Produzioni.Linea_Cod AND Agenda.PIVA = Linee_Produzioni.Piva " & vbCrLf)

            stbQ.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod AND Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod " & vbCrLf)

            stbQ.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_dettagli.Udm_Cod " & vbCrLf)

            'inserito il 10/04/2013 x filtro id_dest
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            '15/04/2013: inutile il left, ci sono due where sulla tabella
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " + vbCrLf)
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " + vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " & vbCrLf)

            'MODIFICA DEL 04/10/2013: non serve più, abbiamo cambiato l'algoritmo di lettura di passaggio
            'If Opt_Gestione_RegistroVinificazione = 1 Then
            '    stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva = Agenda.Piva" + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & "  " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.elem_cod = Movimenti_dettagli.elem_cod " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.mat_cod = Movimenti_dettagli.mat_cod " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.id =(             " + vbCrLf)
            '    stbQ.Append("                                   SELECT MAX(id) " + vbCrLf)
            '    stbQ.Append("                                    FROM OGenerazioni_Anagrafe_Log OLOG " + vbCrLf)
            '    stbQ.Append("                                    WHERE OLOG.piva_superuser = OGenerazioni_Anagrafe_Log.Piva_SuperUser " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.piva = OLOG.Piva " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = OLOG.Modulo_Generazione" + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.elem_cod = OLOG.elem_cod  " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.mat_cod = OLOG.mat_cod " + vbCrLf)
            '    stbQ.Append("                                   ) " + vbCrLf)
            'End If

            stbQ.Append(" WHERE Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)

            'MODIFICA DEL 29/10/2014
            'LEONARDI CI HA DETTO CHE NON VANNO CONTEGGIATI I SALDI DEI CALI: LI ESCLUDIAMO DALLA QUERY DI RIEPILOGO
            stbQ.Append(" AND Movimenti_dettagli.Elem_Cod <> " & Agro_SQL_SaveNum(CALI_LAVORAZIONE) & "  " & vbCrLf)

            'stbQ.Append(" AND Movimenti.CAU_MOV IN ('" & CStr(CAU_CARICO) & "', '" & CStr(CAU_SCARICO) & "') " + vbCrLf)
            stbQ.Append(" AND Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                                    "'" & CAU_SCARICO & "', " &
                                                    "'" & CAU_CONFERIMENTO & "', " &
                                                    "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                                     ") " & vbCrLf)

            stbQ.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            'stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " " + vbCrLf)

            'nel riepilogo non ci vanno solo i prodotti movimentati nel mese
            '--------------
            '19/06/2014: adeguato il filtro con la select (viaggia tutto sul campo Ora)
            'stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataRiepilogoInizio) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " " + vbCrLf)
            stbQ.Append(" AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataRiepilogoInizio) & " " & vbCrLf)
            stbQ.Append(" AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " " & vbCrLf)

            '--------------
            'aggiunto in data 23/09/2013
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            ''''stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataFine) & " " + vbCrLf)
            ''''stbQ.Append(" AND (Trasformazioni.Validita_Fine = " & Agro_SQL_SaveDate(#12/31/2100#) & " ") ' trasformazioni ancora attive
            ''''stbQ.Append(" OR (Trasformazioni.Validita_Fine <= " & Agro_SQL_SaveDate(DataReportFine) & " ")  ' trasformazioni attive nell'intervallo selezionato
            ''''stbQ.Append("     AND Trasformazioni.Validita_Fine >= " & Agro_SQL_SaveDate(DataReportInizio) & ")) ")

            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = 'calibro' " & vbCrLf)
            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri = 1 " & vbCrLf)

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Id_Destinazione <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " & vbCrLf)
            End If
            If Cal_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime_ParametriQualitativi.tipo_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If
            If Linea_Cod <> 0 Then
                stbQ.Append(" AND Linee_Produzioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "  " & vbCrLf)
            End If
            If Cau_Mov <> "" Then
                stbQ.Append(" AND Movimenti.CAU_MOV = '" & Agro_SQL_SaveText(Cau_Mov) & "'" & vbCrLf)
            End If

            'aggiunta in data 25/09/2012:
            'stampa del registro separata per fornitore uve (Cod_Contatto_Terzi)
            If Cod_Contatto_Terzi <> "" Then
                'modifica del 12/08/2013
                'FILTRO C/TERZI
                stbQ.Append(RegistroCommercializzazione_FiltroSQL_ContoTerzi_ParteLinee(Piva, Cod_Contatto_Terzi) & vbCrLf)
            End If

            'correzione del 9/11/2010: mancavano i join con Movimenti_dettagli, per cui
            'se c'era almeno un dettaglio escluso per uno scarico, venivano esclusi tutti gli scarichi
            stbQ.Append(" AND NOT EXISTS (SELECT 1 FROM  Linee_Preparazioni_Report_Esclusi LPRE " & vbCrLf)
            stbQ.Append("                 INNER JOIN Linee_Preparazioni_Dettagli LPD ON LPRE.Piva = LPD.Piva AND LPRE.Preparazione_Cod = LPD.Preparazione_Cod AND LPRE.Dettaglio_Cod = LPD.Dettaglio_Cod " & vbCrLf)
            stbQ.Append("                 WHERE LPRE.Piva = Linee_Preparazioni.Piva " & vbCrLf)
            stbQ.Append("                 AND LPRE.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " & vbCrLf)
            stbQ.Append("                 AND LPRE.Id_Report = Linee_PreparazionixReport.Id_Report  " & vbCrLf)
            stbQ.Append("                 AND LPD.CAU_MOV = Movimenti.CAU_MOV " & vbCrLf)
            stbQ.Append("                 AND LPD.Elem_Cod = Movimenti_dettagli.Elem_Cod " & vbCrLf)
            'modifica del 05/10/2012: se la preparazione passaggio da registro di vinificazione a registro di commercializzazione
            'utilizzava 'risorse indefinita' come ingrediente e/o preparato
            'il join con Movimenti_dettagli non produceva alcun record e quindi venivano visualizzati entrambi i movimenti:
            'sia lo scarico dal reg vinificazione sia il carico al reg di commerc. e la qta si azzerava
            'INTRODOTTO L'OR per LPD.Pro_Cod - LPD.Mat_Cod - LPD.Udm_Cod 
            stbQ.Append("               AND ( " & vbCrLf)
            stbQ.Append("                       ( " & vbCrLf)
            stbQ.Append("                       LPD.Pro_Cod = Movimenti_dettagli.Pro_Cod " & vbCrLf)
            stbQ.Append("                       AND LPD.Mat_Cod = Movimenti_dettagli.Mat_Cod " & vbCrLf)
            stbQ.Append("                       AND LPD.Udm_Cod = Movimenti_dettagli.Udm_Cod   " & vbCrLf)
            stbQ.Append("                       ) OR ( " & vbCrLf)
            stbQ.Append("                       LPD.Pro_Cod = 0 AND LPD.Mat_Cod = 0 AND LPD.Udm_Cod = 0  " & vbCrLf)
            stbQ.Append("                       ) " & vbCrLf)
            stbQ.Append("                   ) " & vbCrLf)
            stbQ.Append("               ) " & vbCrLf)

            'FILTRO DA AGGIUNGERE NEL CASO DI GESTIONE DELLA VINIFICAZIONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            stbQ.Append(RegistroCommercializzazione_FiltroSQL_ConGestioneVinificazione(objParametri,
                                                                                        Opt_Gestione_RegistroVinificazione,
                                                                                        Lista_PrepCod,
                                                                                        Lista_IdTrasf_NoComm,
                                                                                        Piva))

            'MODIFICA DEL 19/02/2016: aggiunto udm_cod per discriminare il caso dei kg
            'MODIFICA DEL 21/08/14: allineato alla gestione del campo ORA
            'stbQ.Append(" GROUP BY Materie_Prime.Piva, Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Movimenti_Dettagli.Lotto,  Movimenti.CAU_MOV, Movimenti_dettagli.Udm_Cod, Materie_Prime.Peso_Set, Materie_Prime_Calibri.Cal_Cod, Materie_Prime_Calibri.Cal_Des, Movimenti.Data_Movimento " + vbCrLf)
            stbQ.Append(" GROUP BY Materie_Prime.Piva, Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Movimenti_Dettagli.Lotto, Movimenti_Dettagli.Udm_cod,   Movimenti.CAU_MOV, Movimenti_dettagli.Udm_Cod, Materie_Prime.Peso_Set, Materie_Prime_Calibri.Cal_Cod, Materie_Prime_Calibri.Cal_Des, Movimenti.Ora " & vbCrLf)
            stbQ.Append(" ) " & vbCrLf)

            stbQ.Append(" ORDER BY Materie_Prime.Elem_Cod DESC, Cal_Des, Movimenti_Dettagli.Lotto " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' registro di commercializzazione: saldo per il riepilogo (in base alla data passata calcola il saldo precedente o di 3 mesi prima))
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    '''  ByVal Filtro_CodRisum_XCTerzi As String, _
    Public Function RegistroCommercializzazioneSaldoxRiepilogo(
                                    ByVal Piva As String,
                                    ByVal Cal_Cod As String,
                                    ByVal AnnoProduzione As String,
                                    ByVal CifraStart As Integer,
                                    ByVal CifraEnd As Integer,
                                    ByVal DataInizioIntervallo As Date,
                                    ByVal Data As Date,
                                    ByVal ComprendiData As Boolean,
                                    ByVal FiltroLotti As String,
                                    ByVal Cod_Contatto_Terzi As String,
                                    ByVal Opt_Gestione_RegistroVinificazione As Integer,
                                    ByVal Lista_PrepCod As String,
                                    ByVal Lista_IdTrasf_NoComm As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Id_Destinazione As Integer,
                                    ByVal Mat_Cod As Integer,
                                    ByVal Linea_Cod As Integer,
                                    ByVal StrFiltroLottoConfigurazione As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.RegistriCantina.RegistroCommercializzazioneSaldoxRiepilogo"

        Dim MessaggioErrore As String = ""
        Dim stbQ As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer
        Dim dRet As Decimal = 0

        Try

            stbQ.Length = 0

            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            stbQ.Append(" SELECT CASE  " & vbCrLf)

            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND ( Movimenti_dettagli.Udm_Cod = " & CStr(enum_UnitaMisura.Litri) & " OR Movimenti_dettagli.Udm_Cod = " & CStr(enum_UnitaMisura.KG) & " ) THEN SUM(Movimenti_dettagli.Qta) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND ( Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.Litri) & " AND Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.KG) & " ) AND Materie_Prime.Peso_Set>0 THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND ( Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.Litri) & " AND Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.KG) & " ) AND Materie_Prime.Peso_Set=0 THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) " & vbCrLf)

            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND ( Movimenti_dettagli.Udm_Cod = " & CStr(enum_UnitaMisura.Litri) & " OR Movimenti_dettagli.Udm_Cod = " & CStr(enum_UnitaMisura.KG) & " ) THEN SUM(-Movimenti_dettagli.Qta) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND ( Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.Litri) & " AND Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.KG) & " ) AND Materie_Prime.Peso_Set>0 THEN SUM(-Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND ( Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.Litri) & " AND Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.KG) & " ) AND Materie_Prime.Peso_Set=0 THEN SUM(-Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) " & vbCrLf)

            stbQ.Append(" ELSE 0 END AS Saldo " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            stbQ.Append(" FROM Linee_PreparazionixReport " & vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Preparazioni ON Linee_PreparazionixReport.Piva = Linee_Preparazioni.Piva AND Linee_PreparazionixReport.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod  " & vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Linee_Preparazioni.Preparazione_Cod = Agenda.PREPARAZIONE_COD AND Linee_Preparazioni.Piva = Agenda.PIVA " & vbCrLf)
            stbQ.Append(" INNER JOIN Trasformazioni ON Trasformazioni.ID_trasformazione=Agenda.ID_Trasformazione AND  Trasformazioni.PIVA=Agenda.PIVA " & vbCrLf)

            'modifica del 27/04/2012 by Maga&Marco: mancava il join della piva
            ''left join per il condizionamento che non è legato a nessuna linea produzione
            'stbQ.Append(" LEFT JOIN Linee_Produzioni ON Agenda.Linea_Cod = Linee_Produzioni.Linea_Cod " + vbCrLf)
            stbQ.Append(" LEFT JOIN Linee_Produzioni ON Agenda.LINEA_COD = Linee_Produzioni.Linea_Cod AND Agenda.PIVA = Linee_Produzioni.Piva " & vbCrLf)

            stbQ.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod  " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov  " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod AND Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_dettagli.Udm_Cod " & vbCrLf)

            '15/04/2013: inutile il left, ci sono due where sulla tabella
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " + vbCrLf)
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " + vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " & vbCrLf)

            'inserito il 10/04/2013 x filtro id_dest
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            'MODIFICA DEL 04/10/2013: non serve più, abbiamo cambiato l'algoritmo di lettura di passaggio
            'If Opt_Gestione_RegistroVinificazione = 1 Then
            '    stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva = Agenda.Piva" + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & "  " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.elem_cod = Movimenti_dettagli.elem_cod " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.mat_cod = Movimenti_dettagli.mat_cod " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.id =(             " + vbCrLf)
            '    stbQ.Append("                                   SELECT MAX(id) " + vbCrLf)
            '    stbQ.Append("                                    FROM OGenerazioni_Anagrafe_Log OLOG " + vbCrLf)
            '    stbQ.Append("                                    WHERE OLOG.piva_superuser = OGenerazioni_Anagrafe_Log.Piva_SuperUser " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.piva = OLOG.Piva " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = OLOG.Modulo_Generazione" + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.elem_cod = OLOG.elem_cod  " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.mat_cod = OLOG.mat_cod " + vbCrLf)
            '    stbQ.Append("                                   ) " + vbCrLf)
            'End If

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            stbQ.Append(" WHERE Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(enum_AgroReportistica.Commercializzazione) & " " & vbCrLf)

            stbQ.Append(" AND Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                                    "'" & CAU_SCARICO & "', " &
                                                    "'" & CAU_CONFERIMENTO & "', " &
                                                    "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                                    ") " & vbCrLf)

            stbQ.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            '--------------
            '19/06/2014: adeguato il filtro con la select (viaggia tutto sul campo Ora)
            'stbQ.Append(" AND Movimenti.Data_Movimento <" & IIf(ComprendiData, "= ", " ") & Agro_SQL_SaveDate(Data) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Data_Movimento >=" & Agro_SQL_SaveDate(DataInizioIntervallo) & " " + vbCrLf)
            stbQ.Append(" AND CONVERT(date, Movimenti.Ora) < " & IIf(ComprendiData, "= ", " ") & Agro_SQL_SaveDate(Data) & " " & vbCrLf)
            stbQ.Append(" AND CONVERT(date, Movimenti.Ora) >= " & Agro_SQL_SaveDate(DataInizioIntervallo) & " " & vbCrLf)

            '--------------
            'aggiunto in data 23/09/2013
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = 'calibro' AND Materie_Prime_ParametriQualitativi.ChkRegistri = 1 " & vbCrLf)
            stbQ.Append(" AND Materie_Prime_Calibri.Cal_Cod=" & Agro_SQL_SaveNum(Cal_Cod) & " " & vbCrLf)

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Id_Destinazione <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " & vbCrLf)
            End If
            If Cal_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime_ParametriQualitativi.tipo_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If
            If Linea_Cod <> 0 Then
                stbQ.Append(" AND Linee_Produzioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "  " & vbCrLf)
            End If

            'aggiunta in data 25/09/2012:
            'stampa del registro separata per fornitore uve (Cod_Contatto_Terzi)
            If Cod_Contatto_Terzi <> "" Then
                'modifica del 12/08/2013
                'FILTRO C/TERZI
                stbQ.Append(RegistroCommercializzazione_FiltroSQL_ContoTerzi_ParteLinee(Piva, Cod_Contatto_Terzi) & vbCrLf)
            End If

            'correzione del 7/12/2010: mancavano i join con Movimenti_dettagli, per cui
            'se c'era almeno un dettaglio escluso per uno scarico, venivano esclusi tutti gli scarichi
            'questa query era rimasta indietro, nel report era stato sistemato in data 9/11/2010
            stbQ.Append(" AND NOT EXISTS (SELECT 1 FROM  Linee_Preparazioni_Report_Esclusi LPRE " & vbCrLf)
            stbQ.Append("                 INNER JOIN Linee_Preparazioni_Dettagli LPD ON LPRE.Piva = LPD.Piva AND LPRE.Preparazione_Cod = LPD.Preparazione_Cod AND LPRE.Dettaglio_Cod = LPD.Dettaglio_Cod " & vbCrLf)
            stbQ.Append("                 WHERE LPRE.Piva = Linee_Preparazioni.Piva " & vbCrLf)
            stbQ.Append("                 AND LPRE.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " & vbCrLf)
            stbQ.Append("                 AND LPRE.Id_Report = Linee_PreparazionixReport.Id_Report  " & vbCrLf)
            stbQ.Append("                 AND LPD.CAU_MOV = Movimenti.CAU_MOV " & vbCrLf)
            stbQ.Append("                 AND LPD.Elem_Cod = Movimenti_dettagli.Elem_Cod " & vbCrLf)
            'modifica del 05/10/2012: se la preparazione passaggio da registro di vinificazione a registro di commercializzazione
            'utilizzava 'risorse indefinita' come ingrediente e/o preparato
            'il join con Movimenti_dettagli non produceva alcun record e quindi venivano visualizzati entrambi i movimenti:
            'sia lo scarico dal reg vinificazione sia il carico al reg di commerc. e la qta si azzerava
            'INTRODOTTO L'OR per LPD.Pro_Cod - LPD.Mat_Cod - LPD.Udm_Cod 
            stbQ.Append("               AND ( " & vbCrLf)
            stbQ.Append("                       ( " & vbCrLf)
            stbQ.Append("                       LPD.Pro_Cod = Movimenti_dettagli.Pro_Cod " & vbCrLf)
            stbQ.Append("                       AND LPD.Mat_Cod = Movimenti_dettagli.Mat_Cod " & vbCrLf)
            stbQ.Append("                       AND LPD.Udm_Cod = Movimenti_dettagli.Udm_Cod   " & vbCrLf)
            stbQ.Append("                       ) OR ( " & vbCrLf)
            stbQ.Append("                       LPD.Pro_Cod = 0 AND LPD.Mat_Cod = 0 AND LPD.Udm_Cod = 0  " & vbCrLf)
            stbQ.Append("                       ) " & vbCrLf)
            stbQ.Append("                   ) " & vbCrLf)
            stbQ.Append("               ) " & vbCrLf)

            'stbQ.Append(" AND NOT EXISTS (SELECT * FROM  Linee_Preparazioni_Report_Esclusi LPRE " + vbCrLf)
            'stbQ.Append("                 INNER JOIN Linee_Preparazioni_Dettagli LPD ON LPRE.Piva = LPD.Piva AND LPRE.Preparazione_Cod = LPD.Preparazione_Cod AND LPRE.Dettaglio_Cod = LPD.Dettaglio_Cod " + vbCrLf)
            ''stbQ.Append("                 WHERE Materie_Prime.Elem_Cod = LPD.Elem_Cod AND Materie_Prime.Mat_Cod = LPD.Mat_Cod" + vbCrLf)
            'stbQ.Append("                 WHERE LPD.CAU_MOV = Movimenti.CAU_MOV " + vbCrLf)
            'stbQ.Append("                 AND LPRE.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod  " + vbCrLf)
            'stbQ.Append("                 AND LPRE.Id_Report = Linee_PreparazionixReport.Id_Report ) " + vbCrLf)



            'FILTRO DA AGGIUNGERE NEL CASO DI GESTIONE DELLA VINIFICAZIONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            stbQ.Append(RegistroCommercializzazione_FiltroSQL_ConGestioneVinificazione(objParametri,
                                                                                        Opt_Gestione_RegistroVinificazione,
                                                                                        Lista_PrepCod,
                                                                                        Lista_IdTrasf_NoComm,
                                                                                        Piva))


            If CifraStart <> 0 AndAlso CifraEnd <> 0 Then
                stbQ.Append(" AND SUBSTRING(Movimenti_Dettagli.Lotto, " & CifraStart & "," & CifraEnd - CifraStart + 1 & ")='" & Agro_SQL_SaveNum(AnnoProduzione) & "' " & vbCrLf)
            Else
                stbQ.Append(FiltroLotti)
            End If

            'aggiunto in data 23/01/2015:
            'per risolvere il problema di due anagrafiche con stessa voce di riepilogo
            'ma configurazione annata di produzione diversa
            If StrFiltroLottoConfigurazione <> "" Then
                stbQ.Append(StrFiltroLottoConfigurazione)
            End If

            stbQ.Append(" GROUP BY Movimenti.Cau_Mov, Movimenti_dettagli.Udm_Cod, Materie_Prime.Peso_Set " & vbCrLf)


            '##################### UNION ##############################
            stbQ.Append(" UNION ALL " & vbCrLf)
            '##################### UNION ##############################


            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            stbQ.Append(" SELECT CASE  " & vbCrLf)

            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND ( Movimenti_dettagli.Udm_Cod = " & CStr(enum_UnitaMisura.Litri) & " OR Movimenti_dettagli.Udm_Cod = " & CStr(enum_UnitaMisura.KG) & " ) THEN SUM(Movimenti_dettagli.Qta) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND ( Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.Litri) & " AND Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.KG) & " ) AND Materie_Prime.Peso_Set>0 THEN SUM(Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_CONFERIMENTO & "') AND ( Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.Litri) & " AND Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.KG) & " ) AND Materie_Prime.Peso_Set=0 THEN SUM(Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) " & vbCrLf)

            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND ( Movimenti_dettagli.Udm_Cod = " & CStr(enum_UnitaMisura.Litri) & " OR Movimenti_dettagli.Udm_Cod = " & CStr(enum_UnitaMisura.KG) & " ) THEN SUM(-Movimenti_dettagli.Qta) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND ( Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.Litri) & " AND Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.KG) & " ) AND Materie_Prime.Peso_Set>0 THEN SUM(-Movimenti_dettagli.Qta*Movimenti_dettagli.Qta_Extra) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND ( Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.Litri) & " AND Movimenti_dettagli.Udm_Cod <> " & CStr(enum_UnitaMisura.KG) & " ) AND Materie_Prime.Peso_Set=0 THEN SUM(-Movimenti_dettagli.Qta*Materie_Prime.Qta_Extra) " & vbCrLf)

            stbQ.Append(" ELSE 0 END AS Saldo " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            stbQ.Append(" FROM Materie_PrimexReport " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Materie_PrimexReport.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov  " & vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND Movimenti.Id_Agenda = Agenda.Id_Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD " & vbCrLf)

            'inserito il 10/04/2013 x filtro id_dest
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            '15/04/2013: inutile il left, ci sono due where sulla tabella
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " + vbCrLf)
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " + vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " & vbCrLf)

            'MODIFICA DEL 04/10/2013: non serve più, abbiamo cambiato l'algoritmo di lettura di passaggio
            'If Opt_Gestione_RegistroVinificazione = 1 Then
            '    stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva = Agenda.Piva" + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & "  " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.elem_cod = Movimenti_dettagli.elem_cod " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.mat_cod = Movimenti_dettagli.mat_cod " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.id =(             " + vbCrLf)
            '    stbQ.Append("                                   SELECT MAX(id) " + vbCrLf)
            '    stbQ.Append("                                    FROM OGenerazioni_Anagrafe_Log OLOG " + vbCrLf)
            '    stbQ.Append("                                    WHERE OLOG.piva_superuser = OGenerazioni_Anagrafe_Log.Piva_SuperUser " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.piva = OLOG.Piva " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = OLOG.Modulo_Generazione" + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.elem_cod = OLOG.elem_cod  " + vbCrLf)
            '    stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.mat_cod = OLOG.mat_cod " + vbCrLf)
            '    stbQ.Append("                                   ) " + vbCrLf)
            'End If

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            'stbQ.Append(" WHERE Movimenti.CAU_MOV IN ('7300', '7350') " + vbCrLf)
            stbQ.Append(" WHERE Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                                        "'" & CAU_SCARICO & "', " &
                                                        "'" & CAU_CONFERIMENTO & "', " &
                                                        "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                                        ") " & vbCrLf)

            stbQ.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            'stbQ.Append(" AND EXISTS (SELECT 1 FROM Agenda Agenda_Lav INNER JOIN OperazionixReport OPR ON Agenda_Lav.Lav_Cod = OPR.Lav_Cod " + vbCrLf)
            'stbQ.Append("             AND Agenda_Lav.Piva = OPR.Piva AND OPR.Id_Report = 4 " + vbCrLf)
            'stbQ.Append("             WHERE  Agenda.Piva = Agenda_Lav.Piva AND Agenda.Id_Agenda = Agenda_Lav.Id_Agenda ) " + vbCrLf)
            'CORREZIONE DEL 07/05/2014: la piva di OperazionixReport è quella del superuser,
            'non va quindi messa in join con la piva di agenda!
            'è stato scoperto ora perchè qualitoscana è il primo cliente col quale si stampano i registri di cantina sulle aziende figlie in gerarchia
            stbQ.Append(" AND EXISTS (SELECT 1 FROM OperazionixReport OPR  " & vbCrLf)
            stbQ.Append("             WHERE  OPR.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append("             AND OPR.Id_Report = " & Agro_SQL_SaveNum(enum_AgroReportistica.Commercializzazione) & "  " & vbCrLf)
            stbQ.Append("               AND Agenda.Lav_Cod = OPR.Lav_Cod ) " & vbCrLf)


            stbQ.Append(" AND Materie_PrimexReport.Id_Report = 4 " & vbCrLf)

            '--------------
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)

            'aggiunta il 19/08/13:per escludere carichi/scarichi di operazioni pendenti (pianificate)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            '--------------
            '19/06/2014: adeguato il filtro con la select (viaggia tutto sul campo Ora)
            'stbQ.Append(" AND Movimenti.Data_Movimento <" & IIf(ComprendiData, "= ", " ") & Agro_SQL_SaveDate(Data) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Data_Movimento >=" & Agro_SQL_SaveDate(DataInizioIntervallo) & " " + vbCrLf)
            stbQ.Append(" AND CONVERT(date, Movimenti.Ora) < " & IIf(ComprendiData, "= ", " ") & Agro_SQL_SaveDate(Data) & " " & vbCrLf)
            stbQ.Append(" AND CONVERT(date, Movimenti.Ora) >= " & Agro_SQL_SaveDate(DataInizioIntervallo) & " " & vbCrLf)


            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = 'calibro'  AND Materie_Prime_ParametriQualitativi.ChkRegistri = 1 " & vbCrLf)
            stbQ.Append(" AND Materie_Prime_Calibri.Cal_Cod=" & Agro_SQL_SaveNum(Cal_Cod) & " " & vbCrLf)

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Id_Destinazione <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " & vbCrLf)
            End If
            If Cal_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime_ParametriQualitativi.tipo_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If
            If Linea_Cod <> 0 Then
                stbQ.Append("   AND  EXISTS (SELECT 1  " & vbCrLf)
                stbQ.Append("               FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
                stbQ.Append(" 				WHERE materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod " & vbCrLf)
                stbQ.Append(" 				AND materie_prime.elem_cod = OGenerazioni_Anagrafe_Log.elem_cod " & vbCrLf)
                stbQ.Append("               AND OGenerazioni_Anagrafe_Log.linea_cod = " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
                stbQ.Append("               ) " & vbCrLf)
            End If

            'aggiunta in data 25/09/2012:
            'stampa del registro separata per fornitore uve (Cod_Contatto_Terzi)
            If Cod_Contatto_Terzi <> "" Then
                'modifica del 12/08/2013
                'FILTRO C/TERZI
                stbQ.Append(RegistroCommercializzazione_FiltroSQL_ContoTerzi_ParteAgenda(objParametri.PivaSuperUser, Piva, Cod_Contatto_Terzi) & vbCrLf)
            End If

            If CifraStart <> 0 AndAlso CifraEnd <> 0 Then
                stbQ.Append(" AND SUBSTRING(Movimenti_Dettagli.Lotto, " & CifraStart & "," & CifraEnd - CifraStart + 1 & ")='" & Agro_SQL_SaveNum(AnnoProduzione) & "' " & vbCrLf)
            Else
                stbQ.Append(FiltroLotti)
            End If

            'aggiunto in data 23/01/2015:
            'per risolvere il problema di due anagrafiche con stessa voce di riepilogo
            'ma configurazione annata di produzione diversa
            If StrFiltroLottoConfigurazione <> "" Then
                stbQ.Append(StrFiltroLottoConfigurazione)
            End If

            'FILTRO DA AGGIUNGERE NEL CASO DI GESTIONE DELLA VINIFICAZIONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            stbQ.Append(RegistroCommercializzazione_FiltroSQL_ConGestioneVinificazione(objParametri,
                                                                                        Opt_Gestione_RegistroVinificazione,
                                                                                        Lista_PrepCod,
                                                                                        Lista_IdTrasf_NoComm,
                                                                                        Piva))


            stbQ.Append(" GROUP BY Movimenti.Cau_Mov, Movimenti_dettagli.Udm_Cod, Materie_Prime.Peso_Set " & vbCrLf)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            For i = 0 To DT.Rows.Count - 1
                dRet += CDbl(IIf(Not IsDBNull(DT.Rows(i).Item("saldo")), DT.Rows(i).Item("saldo"), 0))
            Next


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dRet

    End Function

    '##########################################################################################
    '##########################################################################################
    '##########################################################################################
    '##########################################################################################


    '##############################################################################################
    'separata dalla query principale il 12/08/2013
    Private Function RegistroCommercializzazione_FiltroSQL_ContoTerzi_ParteAgenda(ByVal PivaSuperUser As String,
                                                                                    ByVal Piva As String,
                                                                                    ByVal Cod_contatto_Terzi As String) As String

        Dim stbQ As New StringBuilder
        Dim FiltroSQL As String

        'il filtro è unico, sia per azienda che contatto
        'If Piva = Cod_contatto_Terzi Then
        '    '==========================================
        '    '==== il c/terzi è l'impresa stessa =======
        '    '==========================================
        'Else
        '    '==========================================
        '    '==== il c/terzi è un contatto =======
        '    '==========================================
        'End If


        stbQ.Append(" AND (  -- INIZIO FILTRO C/TERZI PARTE AGENDA " & vbCrLf)
        stbQ.Append("   (  movimenti_dettagli.elem_cod <> " & CStr(ALTRE_MATERIE) & " " & vbCrLf)
        stbQ.Append("   AND ( " & vbCrLf)
        '                           query interna per verificare i semilavorati
        '                           (è la stessa del report consistenze enologiche)
        stbQ.Append("       EXISTS (  " & vbCrLf)
        stbQ.Append("               SELECT 1  " & vbCrLf)
        stbQ.Append("               FROM     Trasformazioni   ")
        stbQ.Append("               INNER JOIN Linee_Produzioni ON Linee_Produzioni.Linea_Cod = Trasformazioni.linea_cod " & vbCrLf)
        stbQ.Append("               AND Linee_Produzioni.piva = Trasformazioni.piva " & vbCrLf)
        stbQ.Append("               WHERE Movimenti_Dettagli.lotto = Trasformazioni.Trasformazione_des " & vbCrLf)
        stbQ.Append("               AND Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "' " & vbCrLf)
        stbQ.Append("               ) " & vbCrLf)
        stbQ.Append("       OR " & vbCrLf)
        stbQ.Append("       EXISTS (  " & vbCrLf)
        '                           query interna per verificare i confezionati (bottiglie, bag in box, damigiane, ecc)
        stbQ.Append("               SELECT 1  " & vbCrLf)
        stbQ.Append("               FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
        stbQ.Append(" 				INNER JOIN  Linee_Produzioni ON OGenerazioni_Anagrafe_Log.linea_cod = Linee_Produzioni.linea_cod  " & vbCrLf)
        stbQ.Append(" 				AND OGenerazioni_Anagrafe_Log.piva = Linee_Produzioni.piva  " & vbCrLf)
        stbQ.Append(" 				WHERE materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod " & vbCrLf)
        stbQ.Append(" 				AND materie_prime.elem_cod = OGenerazioni_Anagrafe_Log.elem_cod " & vbCrLf)
        stbQ.Append(" 				AND OGenerazioni_Anagrafe_Log.Tipo_Generazione IN ( ")
        stbQ.Append(" 				" & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.ProdottoFinito) & ", ")
        stbQ.Append(" 				" & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.Condizionati) & ", ")
        stbQ.Append(" 				" & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.BagInBox) & " ) " & vbCrLf)
        stbQ.Append("               AND Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "' " & vbCrLf)
        stbQ.Append("               ) " & vbCrLf)
        stbQ.Append("   )  " & vbCrLf)

        'aggiunto il 12/08/2015 per gestire
        stbQ.Append("   )  -- elem_cod <> 200 " & vbCrLf)
        stbQ.Append("   OR (  movimenti_dettagli.elem_cod = " & CStr(ALTRE_MATERIE) & " " & vbCrLf)
        stbQ.Append("       AND (" & vbCrLf)

        stbQ.Append("      -- esiste la configurazione in lotto_proprieta per quel lotto " & vbCrLf)
        stbQ.Append("      -- per il c/terzi selezionato " & vbCrLf)
        stbQ.Append("      EXISTS ( " & vbCrLf)
        stbQ.Append("              SELECT 1 " & vbCrLf)
        stbQ.Append("              FROM Materie_PrimexLotto_Proprieta " & vbCrLf)
        stbQ.Append("              where piva_superuser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' " & vbCrLf)
        stbQ.Append("              and piva = Agenda.PIVA " & vbCrLf)
        stbQ.Append("              and id_proprieta = " & CStr(enum_TipoProprieta_Lotto.Categoria_Omni) & "  " & vbCrLf)
        stbQ.Append("              and lotto_val1 = movimenti_dettagli.lotto " & vbCrLf)
        stbQ.Append("              and elem_cod = movimenti_dettagli.elem_cod  " & vbCrLf)
        stbQ.Append("              and mat_cod = movimenti_dettagli.mat_cod " & vbCrLf)
        '                                   nno serve al momento filtrare lotto_cod1 (si imposta solo nome/lotto)
        'stbQ.Append("              and lotto_cod1 = 16 " & vbCrLf)
        stbQ.Append("              and proprieta_val LIKE '%" & Cod_contatto_Terzi & "%' " & vbCrLf)
        stbQ.Append("              ) -- EXISTS " & vbCrLf)

        'per il momento commento questo pezzo per fare in modo che se il lotto dell'MCR non è configurato
        'tale lotto non venga stampato da nessuna parte

        'stbQ.Append("      OR " & vbCrLf)
        'stbQ.Append("      -- non c'è alcun record in lotto_proprieta per il tipo = 3 categoria " & vbCrLf)
        'stbQ.Append("      NOT	EXISTS ( " & vbCrLf)
        'stbQ.Append("                  SELECT 1 " & vbCrLf)
        'stbQ.Append("                  FROM Materie_PrimexLotto_Proprieta " & vbCrLf)
        'stbQ.Append("                  where piva_superuser='" & PivaSuperUser & "' " & vbCrLf)
        'stbQ.Append("                  and piva= Agenda.PIVA " & vbCrLf)
        'stbQ.Append("                  and  id_proprieta = " & CStr(enum_TipoProprieta_Lotto.Categoria_Omni) & "  " & vbCrLf)
        'stbQ.Append("                  and lotto_val1 = movimenti_dettagli.lotto " & vbCrLf)
        'stbQ.Append("                  and elem_cod = movimenti_dettagli.elem_cod  " & vbCrLf)
        'stbQ.Append("                  and mat_cod = movimenti_dettagli.mat_cod " & vbCrLf)
        ''                                   nno serve al momento filtrare lotto_cod1 (si imposta solo nome/lotto)
        ''stbQ.Append("              and lotto_cod1 = 16 " & vbCrLf)
        'stbQ.Append("                  ) -- NOT EXISTS" & vbCrLf)

        stbQ.Append("           )   " & vbCrLf)
        stbQ.Append("       )  -- elem_cod = 200 " & vbCrLf)

        stbQ.Append("   ) -- FINE FILTRO C/TERZI PARTE AGENDA   " & vbCrLf)


        'stbQ.Append("                " + vbCrLf)
        'stbQ.Append("                " + vbCrLf)
        'stbQ.Append("                " + vbCrLf)
        'stbQ.Append("                " + vbCrLf)

        'NON VA BENE :(
        'davamo per scontato che il confezionato (bottiglia, bag in box, damigiana, ecc fosse caricato con una operazione delle linee (imbottigliamento, condizionam, ecc)
        'invece possono essere stati caricati con un carico di magazzino (e con questa query venivano scartati in tronco)
        'stbQ.Append(" -- la materia prima movimentata (nella query principale) è presente  " + vbCrLf)
        'stbQ.Append(" --in un'operazione di una linea che è del c/terzi selezionato  " + vbCrLf)
        'stbQ.Append(" AND  EXISTS (SELECT 1  " + vbCrLf)
        'stbQ.Append("               FROM    Agenda AgCon " & vbCrLf)
        'stbQ.Append("               INNER JOIN Movimenti  Movcon ON AgCon.PIVA = Movcon.PIVA  AND AgCon.Id_Agenda = Movcon.id_agenda " & vbCrLf)
        'stbQ.Append("               INNER JOIN Movimenti_dettagli  MovDetcon ON MovDetcon.PIVA = Movcon.PIVA  AND MovDetcon.Id_Agenda = Movcon.id_agenda " & vbCrLf)
        'stbQ.Append("                           AND MovDetcon.Id_mov = Movcon.id_mov " & vbCrLf)
        'stbQ.Append("               INNER JOIN Linee_Produzioni LP on LP.Linea_Cod = AgCon.linea_cod  and LP.piva = AgCon.piva  " & vbCrLf)
        'stbQ.Append("               WHERE MovDetcon.lotto = Movimenti_Dettagli.lotto " & vbCrLf)
        'stbQ.Append("               AND MovDetcon.elem_cod = Movimenti_Dettagli.elem_cod " & vbCrLf)
        'stbQ.Append("               AND MovDetcon.mat_cod = Movimenti_Dettagli.mat_cod " & vbCrLf)
        'stbQ.Append("               AND LP.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "' " + vbCrLf)
        'stbQ.Append("               ) " + vbCrLf)

        '''NON VA BENE!!! perchè le anagrafiche possono essere comuni tra una linea c/terzi e un'altra
        '''ad esempio, martelli: vino bianco (21) mat_cod=11 usato sia da martelli che da linea c/terzi bersani
        '''verifico che la materia prima movimentata sia loggata nell'omni su una linea di quel c/terzi
        ''stbQ.Append("   AND  EXISTS (SELECT 1  " + vbCrLf)
        ''stbQ.Append("               FROM OGenerazioni_Anagrafe_Log " + vbCrLf)
        ''stbQ.Append(" 				INNER JOIN  Linee_Produzioni ON OGenerazioni_Anagrafe_Log.linea_cod = Linee_Produzioni.linea_cod  " + vbCrLf)
        ''stbQ.Append(" 				            AND OGenerazioni_Anagrafe_Log.piva = Linee_Produzioni.piva  " + vbCrLf)
        ''stbQ.Append(" 				WHERE materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod " + vbCrLf)
        ''stbQ.Append("               AND Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "' " + vbCrLf)
        ''stbQ.Append("               ) " + vbCrLf)

        FiltroSQL = stbQ.ToString

        Return FiltroSQL

    End Function


    '##############################################################################################
    'separata dalla query principale il 12/08/2013
    Private Function RegistroCommercializzazione_FiltroSQL_ContoTerzi_ParteLinee(ByVal Piva As String,
                                                                                    ByVal Cod_contatto_Terzi As String) As String

        Dim stbQ As New StringBuilder
        Dim FiltroSQL As String

        'modifica del 09/09/2014: non va bene, se sul c/terzi viene fatta una separazione consistenze enologiche 
        '(su un lotto in commercializzazione), l'operazioen non viene vista perhè linea_cod è quello della linea di aprtenza, quindi di martelli
        ''al momento il filtro è unico, sia per azienda che contatto
        ''stbQ.Append(" AND Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "' " + vbCrLf)

        If Piva = Cod_contatto_Terzi Then
            '==========================================
            '==== il c/terzi è l'impresa stessa =======
            '==========================================
            stbQ.Append(" AND ( " & vbCrLf)
            '                   cerco le fasi della linea dell'impresa
            stbQ.Append("        Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "' " & vbCrLf)
            stbQ.Append("       AND " & vbCrLf)
            stbQ.Append("       ( " & vbCrLf)
            '                   e il lotto NON deve essere un lotto di vinificazione di una linea c/terzi
            '                   ( ad esempio nel taglio/accorpamento non deve venire considerato lo scarico della qta sulla linea c/terzi)
            stbQ.Append("         NOT  EXISTS (SELECT 1  " & vbCrLf)
            stbQ.Append("  				    FROM trasformazioni " & vbCrLf)
            stbQ.Append("                   INNER JOIN  Linee_Produzioni LProdExt ON trasformazioni.linea_cod = LProdExt.linea_cod AND trasformazioni.Piva = LProdExt.Piva " & vbCrLf)
            stbQ.Append("                   WHERE Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " & vbCrLf)
            stbQ.Append("                   AND LProdExt.Cod_Contatto_Terzi <> '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "'" & vbCrLf)
            stbQ.Append("                   )" & vbCrLf)
            stbQ.Append("       )  " & vbCrLf)
            stbQ.Append("   ) -- AND   " & vbCrLf)
        Else
            '==========================================
            '==== il c/terzi è un contatto =======
            '==========================================
            stbQ.Append(" AND ( " & vbCrLf)
            '                   cerco le fasi della linea del contatto
            stbQ.Append("        Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "' " & vbCrLf)
            stbQ.Append("       OR " & vbCrLf)
            stbQ.Append("       ( " & vbCrLf)
            '                       oppure il lotto è un lotto di vinificazione che fa parte di una linea del contatto
            '                       (serve ad esempio per caricare i tagli/accorpamenti dove la fase è nella linea dell'impresa, non nella linea del contatto e con il filtro sopra non verrebbero trovati)
            stbQ.Append("           EXISTS (SELECT 1  " & vbCrLf)
            stbQ.Append("  				    FROM trasformazioni " & vbCrLf)
            stbQ.Append("                   INNER JOIN  Linee_Produzioni LProdExt ON trasformazioni.linea_cod = LProdExt.linea_cod AND trasformazioni.Piva = LProdExt.Piva " & vbCrLf)
            stbQ.Append("                   WHERE Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " & vbCrLf)
            stbQ.Append("                   AND LProdExt.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "'" & vbCrLf)
            stbQ.Append("                   )" & vbCrLf)
            stbQ.Append("       )  " & vbCrLf)
            stbQ.Append("   ) -- AND   " & vbCrLf)
        End If


        FiltroSQL = stbQ.ToString

        Return FiltroSQL

    End Function


    '##############################################################################################
    'introdotta il 21/01/2013 x separare il filtro per la parte linee da quello della parte agenda
    Private Function RegistroVinificazione_FiltroSQL_ContoTerzi_ParteLinee(ByVal Piva As String,
                                                                            ByVal Cod_contatto_Terzi As String) As String

        Dim stbQ As New StringBuilder
        Dim FiltroSQL As String

        If Piva = Cod_contatto_Terzi Then
            '==========================================
            '==== il c/terzi è l'impresa stessa =======
            '==========================================
            stbQ.Append(" AND ( " & vbCrLf)
            '                   cerco le fasi della linea dell'impresa
            stbQ.Append("        Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "' " & vbCrLf)
            stbQ.Append("       AND " & vbCrLf)
            stbQ.Append("       ( " & vbCrLf)
            '                   e il lotto NON deve essere un lotto di vinificazione di una linea c/terzi
            '                   ( ad esempio nel taglio/accorpamento non deve venire consuiderato lo scarico della qta sulla linea c/terzi)
            stbQ.Append("         NOT  EXISTS (SELECT 1  " & vbCrLf)
            stbQ.Append("  				    FROM trasformazioni " & vbCrLf)
            stbQ.Append("                   INNER JOIN  Linee_Produzioni LProdExt ON trasformazioni.linea_cod = LProdExt.linea_cod AND trasformazioni.Piva = LProdExt.Piva " & vbCrLf)
            stbQ.Append("                   WHERE Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " & vbCrLf)
            stbQ.Append("                   AND LProdExt.Cod_Contatto_Terzi <> '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "'" & vbCrLf)
            stbQ.Append("                   )" & vbCrLf)
            stbQ.Append("       )  " & vbCrLf)
            stbQ.Append("   ) -- AND   " & vbCrLf)
        Else
            '==========================================
            '==== il c/terzi è un contatto =======
            '==========================================
            stbQ.Append(" AND ( " & vbCrLf)
            '                   cerco le fasi della linea del contatto
            stbQ.Append("        Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "' " & vbCrLf)
            stbQ.Append("       OR " & vbCrLf)
            stbQ.Append("       ( " & vbCrLf)
            '                       oppure il lotto è un lotto di vinificazione che fa parte di una linea del contatto
            '                       (serve ad esempio per caricare i tagli/accorpamenti dove la fase è nella linea dell'impresa, non nella linea del contatto e con il filtro sopra non verrebbero trovati)
            stbQ.Append("           EXISTS (SELECT 1  " & vbCrLf)
            stbQ.Append("  				    FROM trasformazioni " & vbCrLf)
            stbQ.Append("                   INNER JOIN  Linee_Produzioni LProdExt ON trasformazioni.linea_cod = LProdExt.linea_cod AND trasformazioni.Piva = LProdExt.Piva " & vbCrLf)
            stbQ.Append("                   WHERE Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " & vbCrLf)
            stbQ.Append("                   AND LProdExt.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "'" & vbCrLf)
            stbQ.Append("                   )" & vbCrLf)
            stbQ.Append("       )  " & vbCrLf)
            stbQ.Append("   ) -- AND   " & vbCrLf)
        End If

        FiltroSQL = stbQ.ToString

        Return FiltroSQL

    End Function

    '##############################################################################################
    'introdotta il 21/01/2013 x separare il filtro per la parte linee da quello della parte agenda
    Private Function RegistroVinificazione_FiltroSQL_ContoTerzi_ParteAgenda(ByVal PivaSuperUser As String,
                                                                            ByVal Piva As String,
                                                                            ByVal Cod_contatto_Terzi As String,
                                                                            ByVal Lista_CodRisUm As String) As String
        Dim stbQ As New StringBuilder
        Dim FiltroSQL As String

        stbQ.Append(" AND (  -- INIZIO FILTRO C/TERZI PARTE AGENDA " & vbCrLf)



        stbQ.Append("       (  movimenti_dettagli.elem_cod <> " & CStr(ALTRE_MATERIE) & " " & vbCrLf)
        stbQ.Append("       AND ( " & vbCrLf)

        If Piva = Cod_contatto_Terzi Then

            '==========================================
            '==== il c/terzi è l'impresa stessa =======
            '----> per i carichi cerco le raccolte e i carichi di magazzino secchi
            '----> OPPURE cerco i movimenti di scarico che hanno dettagli con il lotto = lotto trasformazione
            'a livello di carico non posso prendere in considerazione il mat_cod perchè i semilavorati possono essere gli stessi tra azienda e c/terzi (ad esempio per martelli e per c/terzi gubellini)
            'le uve le devo trattare a parte dal resto, perchè possono avere il lotto diverso da quello di trasformazione_des
            '==========================================

            stbQ.Append(" -- IL C/TERZI E' L'AZIENDA PRINCIPALE " & vbCrLf)

            stbQ.Append(" -- prima parte sul carico " & vbCrLf)
            stbQ.Append("       ( Movimenti.cau_mov = '" & CAU_CARICO & "' " & vbCrLf)
            stbQ.Append("       AND agenda.lav_cod IN ( " & LAVCOD_RACCOLTA & ", " & LAVCOD_CARICO & ") " & vbCrLf)
            stbQ.Append("       ) " & vbCrLf)

            'MODIFICA DEL 20/02/2014:
            stbQ.Append("       OR " & vbCrLf)
            stbQ.Append(" -- seconda parte dell'or " & vbCrLf)
            stbQ.Append("       EXISTS (  " & vbCrLf)
            stbQ.Append("               SELECT 1  " & vbCrLf)
            stbQ.Append("               FROM     Trasformazioni   ")
            stbQ.Append("               INNER JOIN Linee_Produzioni ON Linee_Produzioni.Linea_Cod = Trasformazioni.linea_cod " & vbCrLf)
            stbQ.Append("               AND Linee_Produzioni.piva = Trasformazioni.piva " & vbCrLf)
            stbQ.Append("               WHERE Movimenti_Dettagli.lotto = Trasformazioni.Trasformazione_des " & vbCrLf)
            stbQ.Append("               AND Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "' " & vbCrLf)
            stbQ.Append("               ) " & vbCrLf)

            'per i carichi guardavo l'incrocio con i documenti tramite il cod_risum (non andava bene perchè il movimento è quello di magazzino, non quello contabile), 
            'per gli scarichi facevo la query semilavorati
            '->ora la faccio per entrambi i casi
            'stbQ.Append("       OR " + vbCrLf)
            'stbQ.Append("       ( Movimenti.cau_mov = '" & CAU_CARICO & "' " + vbCrLf)
            'stbQ.Append("       AND NOT EXISTS (SELECT 1  " + vbCrLf)
            'stbQ.Append("  				        FROM risorse_umane " + vbCrLf)
            'stbQ.Append("                       INNER JOIN  Linee_Produzioni ON risorse_umane.Cod_Contatto = Linee_Produzioni.Cod_Contatto_Terzi  " + vbCrLf)
            'stbQ.Append("                       WHERE Movimenti.Cod_RisUm = risorse_umane.cod_risum  " + vbCrLf)
            'stbQ.Append("                       )" + vbCrLf)
            'stbQ.Append("       )  " + vbCrLf)
            'stbQ.Append("       OR " + vbCrLf)
            'stbQ.Append("       ( Movimenti.cau_mov = '" & CAU_SCARICO & "' " + vbCrLf)
            'stbQ.Append("       AND  EXISTS (SELECT 1  " + vbCrLf)
            'stbQ.Append("  				    FROM trasformazioni " + vbCrLf)
            'stbQ.Append("                   INNER JOIN  Linee_Produzioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva  " + vbCrLf)
            'stbQ.Append("                   WHERE Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " + vbCrLf)
            'stbQ.Append("                   AND Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "'" + vbCrLf)
            'stbQ.Append("                   )" + vbCrLf)
            'stbQ.Append("       )  " + vbCrLf)


        Else

            stbQ.Append(" -- IL C/TERZI E' UN CONTATTO " & vbCrLf)

            '==========================================
            '==== il c/terzi è un contatto =======
            '----> per i carichi:
            '       nel caso di documenti non impostati su modulo gias_cantine (documenti normali. ddt, doco, ecc)
            '           si cercano i doc contabili dove il c/terzi è lui stesso il fornitore 
            '       nel caso di documenti impostati su modulo gias_cantine (ddt conferimento uva)
            '           cerco i movimenti che hanno il lotto = lotto trasformazione della linea del c/terzi
            '----> per gli scarichi cerco i movimenti che hanno il lotto = lotto trasformazione della linea del c/terzi
            'a livello di carico non posso prendere in considerazione il mat_cod perchè le uve fresche sono univoche (ad esempio per martelli e per c/terzi gubellini)
            '==========================================

            'MODIFICATO IL 31/07/2015: 
            'leggi commento sopra
            'introdotta modifica x Andreola, per fare in modo che sul c/terzi non venga stampato il ddt di conferimento (dove lui è il fornitore) che invece va nelal linea dell'azienda principale

            stbQ.Append("       -- prima parte OR (carico <> ddt conf) " & vbCrLf)
            stbQ.Append("       ( Movimenti.cau_mov = '" & CAU_CARICO & "' AND agenda.modulo <> " & CStr(enum_Omni_Modulo_Generazione.Cantine) & " " & vbCrLf)
            ''correzione in data 12/09/13: non tutti i doc_contabili salvano nel record movimenti il cod_risum,
            '--> devo verificare il cod_risum nel record del movimento contabile
            'stbQ.Append("       AND Movimenti.Cod_RisUm in ( " & Agro_SQL_SaveText(Lista_CodRisUm) & ") " + vbCrLf)
            stbQ.Append("        AND EXISTS ( SELECT 1  " & vbCrLf)
            stbQ.Append("                   FROM Movimenti MovContabili  " & vbCrLf)
            stbQ.Append("                   WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda " & vbCrLf)
            stbQ.Append("                   AND Movimenti.piva = MovContabili.piva  " & vbCrLf)
            stbQ.Append("                   AND cau_mov='" & CStr(CAU_REGISTRAZIONI) & "' " & vbCrLf)
            stbQ.Append("                   AND MovContabili.Cod_RisUm in ( " & Agro_SQL_Save_Clausola_IN(Lista_CodRisUm) & ") " & vbCrLf)
            stbQ.Append("                   )  " & vbCrLf)
            stbQ.Append("       ) " & vbCrLf)

            stbQ.Append("       OR " & vbCrLf)
            stbQ.Append("       -- seconda parte OR (ddt conf) " & vbCrLf)
            stbQ.Append("       ( Movimenti.cau_mov = '" & CAU_CARICO & "' AND agenda.modulo = " & CStr(enum_Omni_Modulo_Generazione.Cantine) & " " & vbCrLf)
            stbQ.Append("       AND  EXISTS (SELECT 1  " & vbCrLf)
            stbQ.Append("  				    FROM trasformazioni " & vbCrLf)
            stbQ.Append("                   INNER JOIN  Linee_Produzioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva " & vbCrLf)
            stbQ.Append("                   WHERE Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " & vbCrLf)
            stbQ.Append("                   AND Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "'" & vbCrLf)
            stbQ.Append("                   )  " & vbCrLf)
            stbQ.Append("       ) " & vbCrLf)

            stbQ.Append("       OR " & vbCrLf)
            stbQ.Append("       -- terza parte OR (scarico) " & vbCrLf)
            stbQ.Append("       ( Movimenti.cau_mov IN ( '" & CAU_SCARICO & "', '" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "' ) " & vbCrLf)
            stbQ.Append("       AND  EXISTS (SELECT 1  " & vbCrLf)
            stbQ.Append("  				    FROM trasformazioni " & vbCrLf)
            stbQ.Append("                   INNER JOIN  Linee_Produzioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva " & vbCrLf)
            stbQ.Append("                   WHERE Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " & vbCrLf)
            stbQ.Append("                   AND Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_contatto_Terzi) & "'" & vbCrLf)
            stbQ.Append("                   )" & vbCrLf)
            stbQ.Append("       )  " & vbCrLf)

        End If
        stbQ.Append("       )   " & vbCrLf)
        stbQ.Append("       )  -- elem_cod <> 200 " & vbCrLf)
        stbQ.Append("       OR (  movimenti_dettagli.elem_cod = " & CStr(ALTRE_MATERIE) & " " & vbCrLf)
        stbQ.Append("       AND (" & vbCrLf)

        stbQ.Append("      -- esiste la configurazione in lotto_proprieta per quel lotto " & vbCrLf)
        stbQ.Append("      -- per il c/terzi selezionato " & vbCrLf)
        stbQ.Append("      EXISTS ( " & vbCrLf)
        stbQ.Append("              SELECT 1 " & vbCrLf)
        stbQ.Append("              FROM Materie_PrimexLotto_Proprieta " & vbCrLf)
        stbQ.Append("              where piva_superuser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' " & vbCrLf)
        stbQ.Append("              and piva = Agenda.PIVA " & vbCrLf)
        stbQ.Append("              and id_proprieta = " & CStr(enum_TipoProprieta_Lotto.Categoria_Omni) & "  " & vbCrLf)
        stbQ.Append("              and lotto_val1 = movimenti_dettagli.lotto " & vbCrLf)
        stbQ.Append("              and elem_cod = movimenti_dettagli.elem_cod  " & vbCrLf)
        stbQ.Append("              and mat_cod = movimenti_dettagli.mat_cod " & vbCrLf)
        '                                   nno serve al momento filtrare lotto_cod1 (si imposta solo nome/lotto)
        'stbQ.Append("              and lotto_cod1 = 16 " & vbCrLf)
        stbQ.Append("              and proprieta_val LIKE '%" & Cod_contatto_Terzi & "%' " & vbCrLf)
        stbQ.Append("              ) -- EXISTS " & vbCrLf)

        'per il momento commento questo pezzo per fare in modo che se il lotto dell'MCR non è configurato
        'tale lotto non venga stampato da nessuna parte

        'stbQ.Append("      OR " & vbCrLf)
        'stbQ.Append("      -- non c'è alcun record in lotto_proprieta per il tipo = 3 categoria " & vbCrLf)
        'stbQ.Append("      NOT	EXISTS ( " & vbCrLf)
        'stbQ.Append("                  SELECT 1 " & vbCrLf)
        'stbQ.Append("                  FROM Materie_PrimexLotto_Proprieta " & vbCrLf)
        'stbQ.Append("                  where piva_superuser='" & PivaSuperUser & "' " & vbCrLf)
        'stbQ.Append("                  and piva= Agenda.PIVA " & vbCrLf)
        'stbQ.Append("                  and  id_proprieta = " & CStr(enum_TipoProprieta_Lotto.Categoria_Omni) & "  " & vbCrLf)
        'stbQ.Append("                  and lotto_val1 = movimenti_dettagli.lotto " & vbCrLf)
        'stbQ.Append("                  and elem_cod = movimenti_dettagli.elem_cod  " & vbCrLf)
        'stbQ.Append("                  and mat_cod = movimenti_dettagli.mat_cod " & vbCrLf)
        ''                                   nno serve al momento filtrare lotto_cod1 (si imposta solo nome/lotto)
        ''stbQ.Append("              and lotto_cod1 = 16 " & vbCrLf)
        'stbQ.Append("                  ) -- NOT EXISTS" & vbCrLf)



        stbQ.Append("       )   " & vbCrLf)
        stbQ.Append("       )  -- elem_cod = 200 " & vbCrLf)

        stbQ.Append("   ) -- FINE FILTRO C/TERZI PARTE AGENDA   " & vbCrLf)

        FiltroSQL = stbQ.ToString

        Return FiltroSQL

    End Function


    '##############################################################################################
    Private Function RegistroVinificazione_FiltroSQL_Importante(ByRef objParametri_Server As AgronicaCoreParametri,
                                                                ByVal Lista_PrepCod As String,
                                                                ByVal Lista_IdTrasf_NoComm As String,
                                                                ByVal Piva As String) As String

        Dim stbQ As New StringBuilder
        Dim FiltroSQL As String

        stbQ.Append(" AND (   --INIZIALE         " & vbCrLf)

        stbQ.Append("    ( -- CASI 1, 2 e 3  " & vbCrLf)

        stbQ.Append("  (   -- CASO 1         " & vbCrLf)

        ''                   MODIFICA DEL 02/04/2013: sostituito Data_Movimento con Ora (oltre alla data salva l'orario )
        '  stbQ.Append("       (         " + vbCrLf)
        'stbQ.Append("       Movimenti.Ora < (SELECT TOP 1 movP.Ora  " + vbCrLf)
        'stbQ.Append("                                   FROM  Agenda AgP " + vbCrLf)
        'stbQ.Append("                                   INNER JOIN  Movimenti movP ON AgP.PIVA = movP.PIVA AND AgP.Id_Agenda = movP.Id_Agenda  " + vbCrLf)
        'stbQ.Append("                                   INNER JOIN Movimenti_dettagli movdetP ON movP.PIVA = movdetP.PIVA AND movP.Id_Agenda = movdetP.Id_Agenda AND movP.Id_Mov = movdetP.Id_Mov  " + vbCrLf)
        ''                                               MODIFICA DEL 15/04/2013: per gestione caso Tenuta Casali:
        ''                                               se un prodotto viene passato a comemrcializzazione in più volte (prima una parte e poi un'altra)
        'stbQ.Append("                                  INNER JOIN OGenerazioni_Anagrafe_Log OGAL " & vbCrLf)
        'stbQ.Append("                                   ON OGAL.Piva_SuperUser = '" & Trim(objParametri_Server.PivaSuperUser) & "' ")
        'stbQ.Append("                                   AND OGAL.piva = movdetP.Piva" + vbCrLf)
        'stbQ.Append("                                   AND OGAL.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & "  " + vbCrLf)
        'stbQ.Append("                                   AND OGAL.elem_cod = movdetP.elem_cod " + vbCrLf)
        'stbQ.Append("                                   AND OGAL.mat_cod = movdetP.mat_cod " + vbCrLf)
        'stbQ.Append("                                   WHERE movdetP.Elem_Cod = Movimenti_dettagli.Elem_Cod " + vbCrLf)
        'stbQ.Append("                                   -- AND movdetP.Pro_Cod = Movimenti_dettagli.Pro_Cod " + vbCrLf)
        'stbQ.Append("                                   -- AND movdetP.Mat_Cod = Movimenti_dettagli.Mat_Cod " + vbCrLf)
        'stbQ.Append("                                   -- AND movdetP.udm_Cod = Movimenti_dettagli.udm_Cod " + vbCrLf)
        ''                                               modifica del 15/04/2013 -> nell'operazione, il lotto può diventare un altro
        ''                                               ad esempio 11 PAL DIVENTA 11 BAR
        ''stbQ.Append("                                   AND UPPER(movdetP.lotto) = UPPER(Movimenti_dettagli.lotto)   " + vbCrLf)
        'stbQ.Append("                                   AND (     " + vbCrLf)
        'stbQ.Append("                                        UPPER(movdetP.lotto) IN (          " + vbCrLf)
        'stbQ.Append("                                                               ( SELECT UPPER(Movimenti_dettagli.lotto) )" + vbCrLf)
        'stbQ.Append("                                                               UNION " + vbCrLf)
        'stbQ.Append("                                                               ( " + vbCrLf)
        'stbQ.Append("                                                               SELECT trasformazione_des " + vbCrLf)
        'stbQ.Append("                                                               FROM Trasformazioni_Riferimenti  " + vbCrLf)
        'stbQ.Append("                                                               INNER JOIN trasformazioni ON ( " + vbCrLf)
        'stbQ.Append("                                                                       trasformazioni.id_trasformazione = Trasformazioni_Riferimenti.id_trasformazione " + vbCrLf)
        'stbQ.Append("                                                                       OR  trasformazioni.id_trasformazione = Trasformazioni_Riferimenti.Id_Trasformazione_Rif " + vbCrLf)
        'stbQ.Append("                                                                       )" + vbCrLf)
        'stbQ.Append("                                                               WHERE UPPER(trasformazioni.trasformazione_des) = UPPER(Movimenti_dettagli.lotto)  " + vbCrLf)
        'stbQ.Append("                                                               AND Trasformazioni_Riferimenti.id_agenda = Movimenti_dettagli.id_agenda " + vbCrLf)
        'stbQ.Append("                                                               ) -- fine seconda parte union " + vbCrLf)
        'stbQ.Append("                                                               ) -- fine in " + vbCrLf)
        ''PURTROPPO L'EXISTS NON FUNZIONA BENE, SE LA PRIMA PARTE E' VERA, MA NON LO E' LA SECONDA, NON RITORNA ALCUN RISULTATO
        ''stbQ.Append("                                        UPPER(movdetP.lotto) = UPPER(Movimenti_dettagli.lotto) " + vbCrLf)
        ''stbQ.Append("                                        OR EXISTS ( " + vbCrLf)
        ''stbQ.Append("                                                   SELECT 1 " + vbCrLf)
        ''stbQ.Append("                                                   FROM Trasformazioni_Riferimenti  " + vbCrLf)
        ''stbQ.Append("                                                   INNER JOIN trasformazioni ON ( " + vbCrLf)
        ''stbQ.Append("                                                               trasformazioni.id_trasformazione = Trasformazioni_Riferimenti.id_trasformazione " + vbCrLf)
        ''stbQ.Append("                                                               OR  trasformazioni.id_trasformazione = Trasformazioni_Riferimenti.Id_Trasformazione_Rif " + vbCrLf)
        ''stbQ.Append("                                                               )" + vbCrLf)
        ''stbQ.Append("                                                   WHERE UPPER(trasformazioni.trasformazione_des) = UPPER(Movimenti_dettagli.lotto)  " + vbCrLf)
        ''stbQ.Append("                                                   AND Trasformazioni_Riferimenti.id_agenda = Movimenti_dettagli.id_agenda " + vbCrLf)
        ''stbQ.Append("                                                   ) --exists" + vbCrLf)
        'stbQ.Append("                                   ) -- fine and filtro lotto     " + vbCrLf)
        ''                                               MODIFICA DEL 15/04/2013: per gestione caso Tenuta Casali:
        ''                                               se un prodotto viene passato a comemrcializzazione in più volte (prima una parte e poi un'altra)
        'stbQ.Append("                                  AND OGAL.Codice_Generazione >= OGenerazioni_Anagrafe_Log.Codice_Generazione " & vbCrLf)
        'stbQ.Append("                                  AND AgP.Preparazione_Cod IN " & CStr(Lista_PrepCod) + vbCrLf)
        'stbQ.Append("                                   ORDER BY movP.Data_Movimento DESC  " + vbCrLf)
        'stbQ.Append("                               ) -- controllo ora " + vbCrLf)

        'MODIFICA DEL 04/10/13: nuova query di lettura passaggio
        stbQ.Append("       (         " & vbCrLf)
        stbQ.Append("       --  CASO 1-A: verifica della data rispetto al passaggio a registro " & vbCrLf)

        'ATTENZIONE
        'in data 09/09/2014 provato < invece di <=
        'non va bene, non considererebbe gli scarichi per passaggio a commercializzazione
        stbQ.Append("       Movimenti.Ora <= (  SELECT TOP 1 Data  " & vbCrLf)
        'stbQ.Append("       Movimenti.Ora < (  SELECT TOP 1 Data  " + vbCrLf)
        stbQ.Append("                           FROM ( " & vbCrLf)
        stbQ.Append(SQL_Operazioni_PassaggioACommercializzazione(Piva,
                                                  Lista_PrepCod,
                                                  "",
                                                  " UPPER(Trasformazioni2.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ",
                                                  " UPPER(Trasformazioni2.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ",
                                                  " UPPER(Trasformazioni.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) "
                                                  ))
        stbQ.Append("                           ) AS PASSAGGIO" & vbCrLf)
        stbQ.Append("                           ORDER BY Data " & vbCrLf)

        'stbQ.Append("       Movimenti.Ora <= (  SELECT TOP 1 Data  " + vbCrLf)
        'stbQ.Append("                           FROM ( " + vbCrLf)
        ''SQL_Passaggi_DaARegistro_DaALotto_ParteI
        'stbQ.Append(Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteIeIV(Piva, _
        '                                                Lista_PrepCod, _
        '                                                "", _
        '                                                " UPPER(Trasformazioni.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ", _
        '                                                " UPPER(Trasformazioni.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ") & vbCrLf)
        'stbQ.Append("                                   ) AS PASSAGGIO " + vbCrLf)
        'stbQ.Append("                           ORDER BY Tipo_Default, Data " + vbCrLf)

        'stbQ.Append("       OR Movimenti.Ora < (  SELECT TOP 1 Data  " + vbCrLf)
        'stbQ.Append("                           FROM ( " + vbCrLf)
        'stbQ.Append(Z_OLD_SQL_Passaggi_DaARegistro_DaALotto_ParteIIeIII(Piva, _
        '                                                Lista_PrepCod, _
        '                                                Lista_IdTrasf_NoComm, _
        '                                                "", _
        '                                                " UPPER(Trasformazioni1.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ", _
        '                                                " UPPER(Trasformazioni2.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ") & vbCrLf)
        'stbQ.Append("                                   ) AS PASSAGGIO" + vbCrLf)
        'stbQ.Append("                           ORDER BY Tipo_Default, Data " + vbCrLf)

        stbQ.Append("                           ) --ora " & vbCrLf)
        stbQ.Append("       ) -- fine  CASO 1-A       " & vbCrLf)

        '10/10/13: non serve più perchè viene fatto implicatimente dal caso 1-A della query sopra
        'stbQ.Append("   OR             " + vbCrLf)
        'stbQ.Append("                " + vbCrLf)
        'stbQ.Append("       (         " + vbCrLf)
        ''oppure l'operazione stessa è un passaggio da registro a registro
        'stbQ.Append("         Agenda.Preparazione_Cod IN " & CStr(Lista_PrepCod) + vbCrLf)
        'stbQ.Append("       )  -- secondo pezzo caso 1        " + vbCrLf)
        'stbQ.Append("                " + vbCrLf)

        '10/10/13: sostituita con il pezzo sotto che fa il not exists sulla stessa query sopra
        'stbQ.Append("   OR " + vbCrLf)
        'stbQ.Append("       ( " + vbCrLf)
        ''oppure non esiste un'operazione di passaggio per il lotto in oggetto
        'stbQ.Append("       NOT EXISTS ( " + vbCrLf)
        'stbQ.Append("                   SELECT 1  " + vbCrLf)
        'stbQ.Append("                   FROM  Agenda AgP " + vbCrLf)
        'stbQ.Append("                   INNER JOIN  Movimenti movP ON AgP.PIVA = movP.PIVA AND AgP.Id_Agenda = movP.Id_Agenda  " + vbCrLf)
        'stbQ.Append("                   INNER JOIN Movimenti_dettagli movdetP ON movP.PIVA = movdetP.PIVA AND movP.Id_Agenda = movdetP.Id_Agenda AND movP.Id_Mov = movdetP.Id_Mov  " + vbCrLf)
        'stbQ.Append("                   WHERE movdetP.Elem_Cod = Movimenti_dettagli.Elem_Cod " + vbCrLf)
        'stbQ.Append("                   -- AND movdetP.Pro_Cod = Movimenti_dettagli.Pro_Cod " + vbCrLf)
        'stbQ.Append("                   -- AND movdetP.Mat_Cod = Movimenti_dettagli.Mat_Cod " + vbCrLf)
        'stbQ.Append("                   -- AND movdetP.udm_Cod = Movimenti_dettagli.udm_Cod " + vbCrLf)
        'stbQ.Append("                   AND movdetP.lotto = Movimenti_dettagli.lotto   " + vbCrLf)
        'stbQ.Append("                   AND AgP.Preparazione_Cod IN " & CStr(Lista_PrepCod) + vbCrLf)
        'stbQ.Append("                   ) " + vbCrLf)
        'stbQ.Append("       ) " + vbCrLf)

        '17/10/2014: modificato caso 1-B:
        'se Lista_IdTrasf_NoComm è vuota (ad esempio cleinte appena avviato che ha solo lotti in commercializzazione)
        'faceva la query senza il filtro sugli id_trasformazione e l' EXISTS aveva sempre esito positivo
        '-> spostato quindi l'if
        If Lista_IdTrasf_NoComm <> "" Then
            stbQ.Append("   -- CASO 1-B: il lotto non è mai stato passato a commercializzazione " & vbCrLf)
            stbQ.Append("   OR " & vbCrLf)
            stbQ.Append("       ( " & vbCrLf)
            stbQ.Append("       EXISTS ( " & vbCrLf)
            stbQ.Append("                   SELECT 1  " & vbCrLf)
            stbQ.Append("                   FROM Trasformazioni TrasfInt  " & vbCrLf)
            stbQ.Append("                   WHERE UPPER(Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto)  " & vbCrLf)
            ' If Lista_IdTrasf_NoComm <> "" Then
            stbQ.Append("                   AND Id_Trasformazione IN  " & Agro_SQL_SaveText(Lista_IdTrasf_NoComm) & vbCrLf)
            'End If
            stbQ.Append("               )  " & vbCrLf)

            'stbQ.Append("       NOT EXISTS ( " + vbCrLf)
            'stbQ.Append("                   SELECT 1  " + vbCrLf)
            'stbQ.Append("                   FROM ( " + vbCrLf)
            'stbQ.Append(SQL_Passaggi_DaARegistro_DaALotto_Completa(1, _
            '                                                Piva, _
            '                                                Lista_PrepCod, _
            '                                                Lista_IdTrasf_NoComm, _
            '                                                "", _
            '                                                " UPPER(Trasformazioni.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ", _
            '                                                " UPPER(Trasformazioni1.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ", _
            '                                                " UPPER(Trasformazioni2.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ", _
            '                                                " UPPER(Trasformazioni.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) ", _
            '                                                ""))
            'stbQ.Append("                       ) AS PASSAGGIO" + vbCrLf)
            'stbQ.Append("                   ) --  " + vbCrLf)
            stbQ.Append("       ) -- fine CASO 1-B         " & vbCrLf)
        End If

        stbQ.Append("   ) -- FINE CASO 1.  " & vbCrLf)
        stbQ.Append("                " & vbCrLf)

        'CASO 2
        ' non è un confezionato/condizionato (udm_cod_extra = 0)
        stbQ.Append("     -- CASO 2: non è un confezionato/condizionato          " & vbCrLf)
        'MODIFICA DEL 4/10/2013: si verificava UDM_COD_EXTRA in Movimenti_dettagli e non materie_prime
        'stbQ.Append("     AND  Movimenti_dettagli.UDM_COD_EXTRA = 0 -- NO CONFEZIONATI       " + vbCrLf)
        stbQ.Append("     AND  Materie_Prime.UDM_COD_EXTRA = 0 -- NO CONFEZIONATI      " & vbCrLf)
        stbQ.Append("                " & vbCrLf)
        'CASO 3
        'il lotto deve esistere nella tabella Trasformazioni
        stbQ.Append("     -- CASO 3: il lotto deve esistere nella tabella Trasformazioni          " & vbCrLf)
        stbQ.Append("     AND  EXISTS    (" & vbCrLf)
        stbQ.Append("                   SELECT 1" & vbCrLf)
        stbQ.Append("                   FROM Trasformazioni " & vbCrLf)
        stbQ.Append("                   WHERE UPPER(Trasformazioni.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) " & vbCrLf)
        stbQ.Append("                   ) " & vbCrLf)
        stbQ.Append("               " & vbCrLf)
        stbQ.Append("    ) -- fine CASI 1-2-3  " & vbCrLf)

        'CASO 4 IN OR
        'il prodotto in oggetto è loggato come calo di lavorazione o scarto
        'modifica del 14/07/2015: o loggato come altre materie prime aziendali (tipo genarazione = 9), fatto x stampare l'MCR
        stbQ.Append("     OR         " & vbCrLf)
        stbQ.Append("         -- CASO 4: il prodotto in oggetto è loggato come calo di lavorazione o scarto         " & vbCrLf)
        stbQ.Append("          EXISTS    (  " & vbCrLf)
        stbQ.Append("                   SELECT 1  " & vbCrLf)
        stbQ.Append("                   FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
        stbQ.Append("                   WHERE OGenerazioni_Anagrafe_Log.mat_cod = movimenti_dettagli.mat_cod" & vbCrLf)
        stbQ.Append("                   AND ( " & vbCrLf)
        stbQ.Append("                       Tipo_Generazione IN (" & CStr(enum_Omni_Tipo_Generazione.CaliLavorazione) & ", " & enum_Omni_Tipo_Generazione.Scarti & ", " & enum_Omni_Tipo_Generazione.AltreMateriePrime & "   ) " & vbCrLf)
        stbQ.Append("                       OR ( Modulo_Generazione = " & CStr(enum_Omni_Modulo_Generazione.Cantine))
        stbQ.Append("                          AND Codice_Generazione IN (" & CStr(enum_Omni_Codice_Generazione.Tipo4_Uve_Fresche) & ", " &
                                                                            CStr(enum_Omni_Codice_Generazione.Tipo4_Uve_Passite) & ", " &
                                                                            CStr(enum_Omni_Codice_Generazione.Tipo4_Uve_Stramature) & " " &
                                                                    " ) -- in " & vbCrLf)
        '                                                                   CStr(enum_Omni_Codice_Generazione.CompostiLiquidiBaseDiSolfiti) & ", " & _
        '                                                                   CStr(enum_Omni_Codice_Generazione.MetabisolfitoDiPotassio) & " " & _
        stbQ.Append("                           ) -- or " & vbCrLf)
        stbQ.Append("                       ) -- and " & vbCrLf)
        stbQ.Append("                    ) -- exists " & vbCrLf)
        stbQ.Append("            " & vbCrLf)

        stbQ.Append("  ) -- AND INIZIALE              " & vbCrLf)
        stbQ.Append("                " & vbCrLf)

        ''FILTRO CON LA RINTRACCIABILITA':
        '' NON SI PUO' USARE ORA PERCHE' ALCUNI HANNO DATI VECCHI SENZA OPERAZIONE DI PASSAGGIO DA REGISTRO A REGISTRO
        ''operazione di agenda del prodotto stesso o prodotto padre 
        ''scaricato 
        'stbQ.Append(" AND (            " + vbCrLf)
        'stbQ.Append("                " + vbCrLf)

        'stbQ.Append("   (     -- PRIMA PARTE DELL'OR VINIFICAZIONE       " + vbCrLf)

        ''caso 1
        ''1. che un prodotto scaricato (padre) in una preparazione in cui il prodotto in questione (figlio) 
        ''è stato caricato con data precedente alla movimentazione in oggetto, 
        ''sia presente in un passaggio da registro a registro precedente alla data in oggetto.
        'stbQ.Append("   ( NOT EXISTS ( SELECT 1              " + vbCrLf)
        'stbQ.Append("              FROM  Agenda AgP " + vbCrLf)
        'stbQ.Append("              INNER JOIN  Movimenti movP ON AgP.PIVA = movP.PIVA AND AgP.Id_Agenda = movP.Id_Agenda  " + vbCrLf)
        'stbQ.Append("              INNER JOIN Movimenti_dettagli movdetP ON movP.PIVA = movdetP.PIVA AND movP.Id_Agenda = movdetP.Id_Agenda AND movP.Id_Mov = movdetP.Id_Mov  " + vbCrLf)
        'stbQ.Append("              WHERE movdetP.Elem_Cod = Movimenti_dettagli.Elem_Cod " + vbCrLf)
        'stbQ.Append("              AND movP.CAu_Mov = '" & CStr(CAU_SCARICO) & "' " & vbCrLf)

        ''LETTURA DELLE OP. DI AGENDA CHE SONO PASSAGGIO DA REGISTRO DI VINIFICAZIONE A REGISTRO DI COMM.
        'stbQ.Append("              AND EXISTS ( SELECT 1              " + vbCrLf)
        'stbQ.Append("                           FROM Agenda AgPAS              " + vbCrLf)
        'stbQ.Append("                           INNER JOIN Movimenti movPAS ON AgPAS.PIVA = movPAS.PIVA AND AgPAS.Id_Agenda = movPAS.Id_Agenda  " + vbCrLf)
        'stbQ.Append("                           INNER JOIN Movimenti_dettagli movdetPAS ON movPAS.PIVA = movdetPAS.PIVA AND movPAS.Id_Agenda = movdetPAS.Id_Agenda AND movPAS.Id_MOV = movdetPAS.Id_MOV  " + vbCrLf)
        'stbQ.Append("                            WHERE AgPAS.Preparazione_Cod IN " & CStr(Lista_PrepCod) + vbCrLf)
        'stbQ.Append("                           AND movPAS.data_movimento <= movP.Data_Movimento " & vbCrLf)
        'stbQ.Append("                           AND movdetPAS.Elem_Cod = movdetP.Elem_Cod " + vbCrLf)
        'stbQ.Append("                           AND movdetPAS.Pro_Cod = movdetP.Pro_Cod " + vbCrLf)
        'stbQ.Append("                           AND movdetPAS.Mat_Cod = movdetP.Mat_Cod " + vbCrLf)
        'stbQ.Append("                           AND movdetPAS.udm_Cod = movdetP.udm_Cod " + vbCrLf)
        'stbQ.Append("                           AND UPPER(movdetPAS.lotto) = UPPER(movdetP.lotto)   " + vbCrLf)
        'stbQ.Append("                           ) " + vbCrLf)

        ''op. di agenda in cui il prodotto in oggetto è caricato
        'stbQ.Append("              AND EXISTS ( SELECT 1  " + vbCrLf)
        'stbQ.Append("                           FROM  Agenda AgF " + vbCrLf)
        'stbQ.Append("                           INNER JOIN  Movimenti movF ON AgF.PIVA = movF.PIVA AND AgF.Id_Agenda = movF.Id_Agenda  " + vbCrLf)
        'stbQ.Append("                           INNER JOIN Movimenti_dettagli movdetF ON movF.PIVA = movdetF.PIVA AND movF.Id_Agenda = movdetF.Id_Agenda AND movF.Id_Mov = movdetF.Id_Mov  " + vbCrLf)
        'stbQ.Append("                           WHERE movdetF.Elem_Cod = Movimenti_dettagli.Elem_Cod " + vbCrLf)
        'stbQ.Append("                           AND movdetF.Pro_Cod = Movimenti_dettagli.Pro_Cod " + vbCrLf)
        'stbQ.Append("                           AND movdetF.Mat_Cod = Movimenti_dettagli.Mat_Cod " + vbCrLf)
        'stbQ.Append("                           AND movdetF.udm_Cod = Movimenti_dettagli.udm_Cod " + vbCrLf)
        'stbQ.Append("                           AND UPPER(movdetF.lotto) = UPPER(Movimenti_dettagli.lotto)   " + vbCrLf)
        'stbQ.Append("                           AND movF.CAu_Mov = '" & CStr(CAU_CARICO) & "' " & vbCrLf)
        'stbQ.Append("                           AND movF.data_movimento <= Movimenti.Data_Movimento " & vbCrLf)
        'stbQ.Append("                           AND AgP.pIVA = Agf.PIVA AND AgP.id_agenda = Agf.id_agenda  " + vbCrLf)
        'stbQ.Append("                           )  " + vbCrLf)
        'stbQ.Append("           ) " + vbCrLf)
        'stbQ.Append("      ) -- FINE CASO 1.  " + vbCrLf)
        'stbQ.Append("                " + vbCrLf)
        'stbQ.Append("     AND  -- INIZIO CASO 2         " + vbCrLf)
        ''CASO 2
        ''2. non sia un confezionato/condizionato (udm_cod_extra = 0)
        'stbQ.Append("     (           " + vbCrLf)
        'stbQ.Append("       Movimenti_dettagli.UDM_COD_EXTRA = 0 -- NO CONFEZIONATI       " + vbCrLf)
        'stbQ.Append("                " + vbCrLf)
        'stbQ.Append("      ) -- FINE CASO 2         " + vbCrLf)
        'stbQ.Append("                " + vbCrLf)

        ''questo è del reg di commercializzazione
        ''stbQ.Append("     OR  -- INIZIO CASO 3         " + vbCrLf)
        '''CASO 3
        '''il lotto non deve esistere nella tabella Trasformazioni
        ''stbQ.Append("     (           " + vbCrLf)
        ''stbQ.Append("      NOT EXISTS (     " + vbCrLf)
        ''stbQ.Append("                   SELECT 1" + vbCrLf)
        ''stbQ.Append("                   FROM Trasformazioni " + vbCrLf)
        ''stbQ.Append("                   WHERE UPPER(Trasformazioni.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) " + vbCrLf)
        ''stbQ.Append("                   ) " + vbCrLf)
        ''stbQ.Append("                " + vbCrLf)
        ''stbQ.Append("                " + vbCrLf)
        ''stbQ.Append("      ) -- FINE CASO 3         " + vbCrLf)
        ''stbQ.Append("                " + vbCrLf)

        'stbQ.Append("   )     -- FINE PRIMA PARTE DELL'OR VINIFICAZIONE       " + vbCrLf)

        'stbQ.Append("   OR             " + vbCrLf)
        'stbQ.Append("   (     -- SECONDA PARTE DELL'OR VINIFICAZIONE       " + vbCrLf)
        ''           L'OPERAZIONE STESSA E' UN PASSAGGIO DA REGISTRO DI VINIFICAZIONE A REGISTRO DI COMM.
        'stbQ.Append("         Agenda.Preparazione_Cod IN " & CStr(Lista_PrepCod) + vbCrLf)
        'stbQ.Append("   )     -- FINE SECONDA PARTE DELL'OR VINIFICAZIONE       " + vbCrLf)
        'stbQ.Append("                " + vbCrLf)
        'stbQ.Append("  ) -- AND INIZIALE              " + vbCrLf)
        'stbQ.Append("                " + vbCrLf)

        'End If

        'SECONDA PROVA del 18/10/2012
        '    'operazione di agenda del prodotto stesso o proDotto padre 
        '    'scaricato 
        '    stbQ.Append(" AND (            " + vbCrLf)

        '    'caso 1
        '    '1. che un prodotto scaricato (padre) in una preparazione in cui il prodotto in questione (figlio) 
        '    'è stato caricato con data precedente alla movimentazione in oggetto, 
        '    'sia presente in un passaggio da registro a registro precedente alla data in oggetto.
        '    stbQ.Append("   ( NOT EXISTS ( SELECT 1              " + vbCrLf)
        '    stbQ.Append("              FROM  Agenda AgP " + vbCrLf)
        '    stbQ.Append("              INNER JOIN  Movimenti movP ON AgP.PIVA = movP.PIVA AND AgP.Id_Agenda = movP.Id_Agenda  " + vbCrLf)
        '    stbQ.Append("              INNER JOIN Movimenti_dettagli movdetP ON movP.PIVA = movdetP.PIVA AND movP.Id_Agenda = movdetP.Id_Agenda AND movP.Id_Mov = movdetP.Id_Mov  " + vbCrLf)
        '    stbQ.Append("              WHERE movdetP.Elem_Cod = Movimenti_dettagli.Elem_Cod " + vbCrLf)
        '    stbQ.Append("              AND movP.CAu_Mov = '" & CStr(CAU_SCARICO) & "' " & vbCrLf)

        '    'LETTURA DELLE OP. DI AGENDA CHE SONO PASSAGGIO DA REGISTRO DI VINIFICAZIONE A REGISTRO DI COMM.
        '    stbQ.Append("              AND EXISTS ( SELECT 1              " + vbCrLf)
        '    stbQ.Append("                           FROM Agenda AgPAS              " + vbCrLf)
        '    stbQ.Append("                           INNER JOIN Movimenti movPAS ON AgPAS.PIVA = movPAS.PIVA AND AgPAS.Id_Agenda = movPAS.Id_Agenda  " + vbCrLf)
        '    stbQ.Append("                           INNER JOIN Movimenti_dettagli movdetPAS ON movPAS.PIVA = movdetPAS.PIVA AND movPAS.Id_Agenda = movdetPAS.Id_Agenda AND movPAS.Id_MOV = movdetPAS.Id_MOV  " + vbCrLf)
        '    stbQ.Append("                            WHERE AgPAS.Preparazione_Cod IN " & CStr(Lista_PrepCod) + vbCrLf)
        '    stbQ.Append("                           AND movPAS.data_movimento < movP.Data_Movimento " & vbCrLf)
        '    stbQ.Append("                           AND movdetPAS.Elem_Cod = movdetP.Elem_Cod " + vbCrLf)
        '    stbQ.Append("                           AND movdetPAS.Pro_Cod = movdetP.Pro_Cod " + vbCrLf)
        '    stbQ.Append("                           AND movdetPAS.Mat_Cod = movdetP.Mat_Cod " + vbCrLf)
        '    stbQ.Append("                           AND movdetPAS.udm_Cod = movdetP.udm_Cod " + vbCrLf)
        '    stbQ.Append("                           AND movdetPAS.lotto = movdetP.lotto   " + vbCrLf)
        '    stbQ.Append("                           ) " + vbCrLf)

        '    'op. di agenda in cui il prodotto in oggetto è caricato
        '    stbQ.Append("              AND EXISTS ( SELECT 1  " + vbCrLf)
        '    stbQ.Append("                           FROM  Agenda AgF " + vbCrLf)
        '    stbQ.Append("                           INNER JOIN  Movimenti movF ON AgF.PIVA = movF.PIVA AND AgF.Id_Agenda = movF.Id_Agenda  " + vbCrLf)
        '    stbQ.Append("                           INNER JOIN Movimenti_dettagli movdetF ON movF.PIVA = movdetF.PIVA AND movF.Id_Agenda = movdetF.Id_Agenda AND movF.Id_Mov = movdetF.Id_Mov  " + vbCrLf)
        '    stbQ.Append("                           WHERE movdetF.Elem_Cod = Movimenti_dettagli.Elem_Cod " + vbCrLf)
        '    stbQ.Append("                           AND movdetF.Pro_Cod = Movimenti_dettagli.Pro_Cod " + vbCrLf)
        '    stbQ.Append("                           AND movdetF.Mat_Cod = Movimenti_dettagli.Mat_Cod " + vbCrLf)
        '    stbQ.Append("                           AND movdetF.udm_Cod = Movimenti_dettagli.udm_Cod " + vbCrLf)
        '    stbQ.Append("                           AND UPPER(movdetF.lotto) = UPPER(Movimenti_dettagli.lotto)   " + vbCrLf)
        '    stbQ.Append("                           AND movF.CAu_Mov = '" & CStr(CAU_CARICO) & "' " & vbCrLf)
        '    stbQ.Append("                           AND movF.data_movimento < Movimenti.Data_Movimento " & vbCrLf)
        '    stbQ.Append("                           AND AgP.pIVA = Agf.PIVA AND AgP.id_agenda = Agf.id_agenda  " + vbCrLf)
        '    stbQ.Append("                           )  " + vbCrLf)
        '    stbQ.Append("           ) " + vbCrLf)
        '    stbQ.Append("      ) -- FINE CASO 1.  " + vbCrLf)
        '    stbQ.Append("     AND  -- INIZIO CASO 2         " + vbCrLf)
        '    'CASO 2
        '    '2. sia un confezionato/condizionato (udm_cod_extra <> 0)
        '    '- la preparazione in questione potrebbe quindi essere il passaggio da registro a registro.
        '    '- per il registro quello di vinificazione si usa il duale.
        '    '- se non si usa il trucco sui confezionati/condizionati occorre controllare anche il nonno e sarebbe troppo annidato.
        '    stbQ.Append("     (           " + vbCrLf)
        '    stbQ.Append("       Movimenti_dettagli.UDM_COD_EXTRA = 0    " + vbCrLf)
        '    stbQ.Append("                " + vbCrLf)
        '    stbQ.Append("      ) -- FINE CASO 2         " + vbCrLf)
        '    stbQ.Append("                " + vbCrLf)
        '    'stbQ.Append("     OR  -- INIZIO CASO 3         " + vbCrLf)
        '    ''CASO 3
        '    ''il lotto non deve esistere nella tabella Trasformazioni
        '    'stbQ.Append("     (           " + vbCrLf)
        '    'stbQ.Append("      NOT EXISTS (     " + vbCrLf)
        '    'stbQ.Append("                   SELECT 1" + vbCrLf)
        '    'stbQ.Append("                   FROM Trasformazioni " + vbCrLf)
        '    'stbQ.Append("                   WHERE UPPER(Trasformazioni.Trasformazione_Des) = UPPER(Movimenti_dettagli.lotto) " + vbCrLf)
        '    'stbQ.Append("                   ) " + vbCrLf)
        '    'stbQ.Append("                " + vbCrLf)
        '    'stbQ.Append("                " + vbCrLf)
        '    'stbQ.Append("      ) -- FINE CASO 3         " + vbCrLf)
        '    stbQ.Append("                " + vbCrLf)
        '    stbQ.Append("  ) -- AND INIZIALE              " + vbCrLf)
        '    stbQ.Append("                " + vbCrLf)


        'PRIMA PROVA DI FILTRO DEL 15/10/2012
        'If Opt_Gestione_RegistroVinificazione = 1 Then
        '    'ci deve essere una sola operazione di passaggio
        '    'per sicurezza metto il TOP 1 con l'ordinamento decrescente
        '    stbQ.Append(" AND ( " + vbCrLf)
        '    stbQ.Append("       ( " + vbCrLf)
        '    stbQ.Append("       Movimenti.Data_Movimento < (SELECT TOP 1 movP.Data_Movimento  " + vbCrLf)
        '    stbQ.Append("                                   FROM  Agenda AgP " + vbCrLf)
        '    stbQ.Append("                                   INNER JOIN  Movimenti movP ON AgP.PIVA = movP.PIVA AND AgP.Id_Agenda = movP.Id_Agenda  " + vbCrLf)
        '    stbQ.Append("                                   INNER JOIN Movimenti_dettagli movdetP ON movP.PIVA = movdetP.PIVA AND movP.Id_Agenda = movdetP.Id_Agenda AND movP.Id_Mov = movdetP.Id_Mov  " + vbCrLf)
        '    stbQ.Append("                                   WHERE movdetP.Elem_Cod = Movimenti_dettagli.Elem_Cod " + vbCrLf)
        '    stbQ.Append("                                   AND movdetP.Pro_Cod = Movimenti_dettagli.Pro_Cod " + vbCrLf)
        '    stbQ.Append("                                   AND movdetP.Mat_Cod = Movimenti_dettagli.Mat_Cod " + vbCrLf)
        '    stbQ.Append("                                   AND movdetP.udm_Cod = Movimenti_dettagli.udm_Cod " + vbCrLf)
        '    stbQ.Append("                                   AND movdetP.lotto = Movimenti_dettagli.lotto   " + vbCrLf)
        '    'anzichè fare la stessa query per ogni operazione
        '    'controllo se preparazione cod è nella lista dei preparzione cod (che ho letto fuori dalla query del registro)
        '    stbQ.Append("                                  AND AgP.Preparazione_Cod IN " & CStr(Lista_PrepCod) + vbCrLf)
        '    'stbQ.Append("                                   AND EXISTS ( SELECT 1 " + vbCrLf)
        '    'stbQ.Append("                                               FROM Linee_Preparazioni " + vbCrLf)
        '    'stbQ.Append("                                               WHERE Linee_Preparazioni.Preparazione_Cod = AgP.Preparazione_Cod " + vbCrLf)
        '    'stbQ.Append("                                               AND Linee_Preparazioni.Piva = AgP.Piva " + vbCrLf)
        '    'stbQ.Append("                                               AND Linee_Preparazioni.Modulo_Generazione = " & CStr(enum_Modulo_Generazione.Cantine) & " " + vbCrLf)
        '    'stbQ.Append("                                               AND Linee_Preparazioni.Codice_Generazione = " & CStr(enum_Codice_Generazione_Preparazioni.Passaggio_RegVinificazione_Commercializzazione) & " " + vbCrLf)
        '    'stbQ.Append("                                               ) " + vbCrLf)
        '    stbQ.Append("                                   ORDER BY movP.Data_Movimento DESC  " + vbCrLf)
        '    stbQ.Append("                               ) " + vbCrLf)
        '    stbQ.Append("       ) " + vbCrLf)
        '    stbQ.Append("       OR " + vbCrLf)
        '    stbQ.Append("       ( " + vbCrLf)
        '    stbQ.Append("       NOT EXISTS ( " + vbCrLf)
        '    stbQ.Append("                   SELECT 1  " + vbCrLf)
        '    stbQ.Append("                   FROM  Agenda AgP " + vbCrLf)
        '    stbQ.Append("                   INNER JOIN  Movimenti movP ON AgP.PIVA = movP.PIVA AND AgP.Id_Agenda = movP.Id_Agenda  " + vbCrLf)
        '    stbQ.Append("                   INNER JOIN Movimenti_dettagli movdetP ON movP.PIVA = movdetP.PIVA AND movP.Id_Agenda = movdetP.Id_Agenda AND movP.Id_Mov = movdetP.Id_Mov  " + vbCrLf)
        '    stbQ.Append("                   WHERE movdetP.Elem_Cod = Movimenti_dettagli.Elem_Cod " + vbCrLf)
        '    stbQ.Append("                   AND movdetP.Pro_Cod = Movimenti_dettagli.Pro_Cod " + vbCrLf)
        '    stbQ.Append("                   AND movdetP.Mat_Cod = Movimenti_dettagli.Mat_Cod " + vbCrLf)
        '    stbQ.Append("                   AND movdetP.udm_Cod = Movimenti_dettagli.udm_Cod " + vbCrLf)
        '    stbQ.Append("                   AND movdetP.lotto = Movimenti_dettagli.lotto   " + vbCrLf)
        '    stbQ.Append("                   AND AgP.Preparazione_Cod IN " & CStr(Lista_PrepCod) + vbCrLf)
        '    stbQ.Append("                   ) " + vbCrLf)
        '    stbQ.Append("       ) " + vbCrLf) 'seconda parte dell'OR
        '    stbQ.Append("   ) " + vbCrLf) 'and aggiuntivo
        'End If


        FiltroSQL = stbQ.ToString

        Return FiltroSQL

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' registro di vinificazione
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RegistroVinificazione(ByRef DataSet2Fill As DataSet,
                                            ByVal strNomeDtNelDS As String,
                                            ByVal Piva As String,
                                            ByVal Id_Report As Integer,
                                            ByVal DataReportInizio As Date,
                                            ByVal DataReportFine As Date,
                                            ByVal Cod_Contatto_Terzi As String,
                                            ByVal Lista_CodRisUm As String,
                                            ByVal Lista_PrepCod As String,
                                            ByVal Lista_IdTrasf_NoComm As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Id_Destinazione As Integer,
                                            ByVal Cal_Cod As Integer,
                                            ByVal Mat_Cod As Integer,
                                            ByVal Linea_Cod As Integer,
                                            ByVal Cau_Mov As String,
                                            ByVal Gestione_Conto_Terzi As enum_RegistroContoTerzi,
                                            ByVal str_idagenda_filtrocategoria As String,
                                            ByVal str_matcod_filtrocategoria As String,
                                            ByVal str_lotto_filtrocategoria As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable
        ') As Boolean

        ' ByVal Opt_Gestione_RegistroVinificazione As Integer, _
        'ByVal DataInizio As Date, _
        '                     ByVal DataFine As Date, _

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.RegistriCantina.RegistroVinificazione"

        Dim MessaggioErrore As String = ""
        Dim stbQ As New System.Text.StringBuilder
        ' Dim Risp As Boolean = False
        Dim DT As DataTable

        Try

            Dim DataFineControllo As Date
            DataFineControllo = DateAdd(DateInterval.Day, 1, DataReportFine)


            '=========================================================
            '--------------- 1) PREPARAZIONI (QUERY CHE LEGGE LE LINEE) ------------------
            '=========================================================

            'MODIFICA DEL 18/10/2013: lettura del campo ora (visto che si verifica quello nel controllo del passaggio a registro)
            'stbQ.Append(" SELECT Agenda.Id_Agenda, Movimenti.Data_Movimento, Agenda.des_lib, Linee_Preparazioni.Preparazione_Des AS Descrizione, " + vbCrLf)
            stbQ.Append(" SELECT Agenda.Id_Agenda, agenda.lav_cod, Movimenti.Ora AS Data_Movimento, Agenda.des_lib, Linee_Preparazioni.Preparazione_Des + ' del ' + CONVERT(VARCHAR(10), Movimenti.Data_Movimento, 103) AS Descrizione, " & vbCrLf)

            'MODIFICA DEL 03/06/2014
            'stbQ.Append(" ISNULL(Linee_Produzioni.Linea_Des + '-','')  + ISNULL(Trasformazione_Des,'') AS Designazione, Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_Cod, " + vbCrLf)
            Select Case Gestione_Conto_Terzi

                Case enum_RegistroContoTerzi.Nessuno,
                   enum_RegistroContoTerzi.RegistroGlobale,
                         enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    '0 = se non c'è esiste conto terzi
                    '1 = se esiste, ma è stato richiesto registro unico
                    '3 = questa select va bene anche nel reg c/terzi separato
                    stbQ.Append(" ISNULL(Linee_Produzioni.Linea_Des + '-','')  + ISNULL(Trasformazione_Des,'') AS Designazione " & vbCrLf)

                Case enum_RegistroContoTerzi.RegistroUnicoDiversificato
                    '2= registro unico ma diversificato per c/lav (MODELLO RUGGERI)
                    stbQ.Append(" (ISNULL( ( SELECT TOP 1 Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome  " & vbCrLf)
                    stbQ.Append("       FROM Contatti " & vbCrLf)
                    stbQ.Append("       WHERE Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto " & vbCrLf)
                    stbQ.Append("       AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
                    stbQ.Append("       AND Linee_Produzioni.Cod_Contatto_Terzi <> '' " & vbCrLf)
                    stbQ.Append("       ), '') " & vbCrLf)
                    stbQ.Append("   + Linee_Produzioni.Linea_Des + '-' + Trasformazione_Des  " & vbCrLf)
                    stbQ.Append(" ) AS Designazione " & vbCrLf)
                    stbQ.Append(" " & vbCrLf)
            End Select

            'If Flag_Conto_Terzi = False Then
            '    'se non c'è conto terzi abilitato
            '    stbQ.Append(" ISNULL(Linee_Produzioni.Linea_Des + '-','')  + ISNULL(Trasformazione_Des,'') AS Designazione, " + vbCrLf)
            'Else
            '    If Cod_Contatto_Terzi <> "" Then
            '        'se è impostato il filtro del c/terzi nel menù a tendina
            '        stbQ.Append(" ISNULL(Linee_Produzioni.Linea_Des + '-','')  + ISNULL(Trasformazione_Des,'') AS Designazione, " + vbCrLf)
            '    Else
            '        'registro massivo delle linee c/Terzi
            '        stbQ.Append(" (ISNULL( ( SELECT TOP 1 Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome  " + vbCrLf)
            '        stbQ.Append("       FROM Contatti " + vbCrLf)
            '        stbQ.Append("       WHERE Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto " + vbCrLf)
            '        stbQ.Append("       AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " + vbCrLf)
            '        stbQ.Append("       AND Linee_Produzioni.Cod_Contatto_Terzi <> '' " + vbCrLf)
            '        stbQ.Append("       ), '') " + vbCrLf)
            '        stbQ.Append("   + Linee_Produzioni.Linea_Des + '-' + Trasformazione_Des  " + vbCrLf)
            '        stbQ.Append(" ) AS Designazione, " + vbCrLf)
            '        stbQ.Append(" " + vbCrLf)
            '    End If
            'End If

            stbQ.Append(" , Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_Cod, " & vbCrLf)

            stbQ.Append(" Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Lotto,  " & vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=2 THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS CaricoKg, " & vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=2 THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS ScaricoKg, " & vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=29 THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS CaricoLt, " & vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=29 THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS ScaricoLt," & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Udm_Cod, UnitaMisura.Udm_Sim, Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo, '' AS NDoc " & vbCrLf)
            stbQ.Append(", Linee_PreparazionixReport.StrCampo_Registri " & vbCrLf)

            'inserito il 15/10/2012 per stampare il numero di vasca
            stbQ.Append(", Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Sa_Cod AS SaCod_Dest, Mov_Destinazioni.Id_Destinazione AS Id_Dest " & vbCrLf)
            stbQ.Append(" , Linee_Preparazioni.Tipo_Integrazione   " & vbCrLf)

            stbQ.Append(" FROM Linee_PreparazionixReport " & vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Preparazioni ON Linee_PreparazionixReport.Piva = Linee_Preparazioni.Piva AND Linee_PreparazionixReport.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod  " & vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Linee_Preparazioni.Preparazione_Cod = Agenda.PREPARAZIONE_COD AND Linee_Preparazioni.Piva = Agenda.PIVA " & vbCrLf)
            stbQ.Append(" INNER JOIN Trasformazioni ON Trasformazioni.ID_trasformazione=Agenda.ID_Trasformazione AND  Trasformazioni.PIVA=Agenda.PIVA " & vbCrLf)

            'Modifica del 27/04/2012 by Maga&Marco:
            'nella vinificazione c'è sempre la linea_Produzione, quindi mettiamo inner join 
            'e aggiungiamo join della piva (altrimenti vengono sdoppiati i record a causa della piva AAAAAAAAAAA)
            'stbQ.Append(" LEFT JOIN Linee_Produzioni ON Agenda.Linea_Cod = Linee_Produzioni.Linea_Cod " + vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Produzioni ON Agenda.Piva = Linee_Produzioni.Piva AND Agenda.Linea_Cod = Linee_Produzioni.Linea_Cod " & vbCrLf)

            stbQ.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod  " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov  " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod AND Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_dettagli.Udm_Cod " & vbCrLf)

            'inserito il 15/10/2012 per stampare il numero di vasca
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            '15/04/2013: inutile il left, ci sono due where sulla tabella
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " + vbCrLf)
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " + vbCrLf)

            'nota del 17/06/2014: siam d'accordo con Marco di non introdurre il join su AND Materie_Prime_ParametriQualitativi.PIVA = Materie_Prime.piva
            'non ci devono essere record duplicati per lo stesso mat_cod -> introduciamo una query di delete di sicurezza
            stbQ.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod  " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " & vbCrLf)

            'stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva = Agenda.Piva" + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & "  " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.elem_cod = Movimenti_dettagli.elem_cod " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.mat_cod = Movimenti_dettagli.mat_cod " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.id =(             " + vbCrLf)
            'stbQ.Append("                                   SELECT MAX(id) " + vbCrLf)
            'stbQ.Append("                                    FROM OGenerazioni_Anagrafe_Log OLOG " + vbCrLf)
            'stbQ.Append("                                    WHERE OLOG.piva_superuser = OGenerazioni_Anagrafe_Log.Piva_SuperUser " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.piva = OLOG.Piva " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = OLOG.Modulo_Generazione" + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.elem_cod = OLOG.elem_cod  " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.mat_cod = OLOG.mat_cod " + vbCrLf)
            'stbQ.Append("                                   ) " + vbCrLf)
            'stbQ.Append("               " + vbCrLf)

            stbQ.Append(" WHERE Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)

            stbQ.Append(" AND Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                                      "'" & CAU_SCARICO & "', " &
                                                      "'" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', " &
                                                      "'" & CAU_CONFERIMENTO & "', " &
                                                      "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                                            ") " & vbCrLf)

            stbQ.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If str_idagenda_filtrocategoria <> "" Then
                stbQ.Append(" AND Agenda.Id_Agenda IN " & Agro_SQL_Save_Clausola_IN(str_idagenda_filtrocategoria) & " " & vbCrLf)
            End If
            If str_matcod_filtrocategoria <> "" Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod IN " & Agro_SQL_Save_Clausola_IN(str_matcod_filtrocategoria) & " " & vbCrLf)
            End If
            If str_lotto_filtrocategoria <> "" Then
                'non ci va Agro_SQL_SaveText() perchè no mette il doppio apice
                stbQ.Append(" AND Movimenti_Dettagli.Lotto IN " & Agro_SQL_Save_Clausola_IN(str_lotto_filtrocategoria, True) & " " & vbCrLf)
            End If

            '--------------
            '21/08/2014: adeguato il filtro con la select (viaggia tutto sul campo Ora)
            'stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " " + vbCrLf)
            stbQ.Append(" AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " " & vbCrLf)
            stbQ.Append(" AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " " & vbCrLf)

            '--------------
            'aggiunto in data 23/09/2013
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            stbQ.Append(" AND Materie_Prime_Calibri.Piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")
            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = 'calibro' ")
            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri_Vinificazione = 1 ")

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Id_Destinazione <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " & vbCrLf)
            End If
            If Cal_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime_ParametriQualitativi.tipo_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If
            If Linea_Cod <> 0 Then
                stbQ.Append(" AND Linee_Produzioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "  " & vbCrLf)
            End If
            If Cau_Mov <> "" Then
                stbQ.Append(" AND Movimenti.CAU_MOV = '" & Agro_SQL_SaveText(Cau_Mov) & "'" & vbCrLf)
            End If
            If Cod_Contatto_Terzi <> "" Then
                'modifica del 21/01/2013: separato il filtro per la parte agenda e la parte linee
                stbQ.Append(RegistroVinificazione_FiltroSQL_ContoTerzi_ParteLinee(Piva, Cod_Contatto_Terzi))

                ''aggiunta in data 27/04/2012: gestione conto terzi
                ''modificata in data 25/09/2012:
                ''stampa del registro separata per fornitore uve (Cod_Contatto_Terzi)
                'stbQ.Append(" AND Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_Contatto_Terzi) & "' " + vbCrLf)

                'stbQ.Append("  AND  EXISTS (SELECT 1  " + vbCrLf)
                'stbQ.Append("  				FROM OGenerazioni_Anagrafe_Log " + vbCrLf)
                'stbQ.Append("               WHERE OGenerazioni_Anagrafe_Log.linea_cod = Linee_Produzioni.linea_cod " + vbCrLf)
                'stbQ.Append("               AND materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod " + vbCrLf)
                'stbQ.Append("               )" + vbCrLf)
            End If

            'correzione del 9/11/2010: mancavano i join con Movimenti_dettagli, per cui
            'se c'era almeno un dettaglio escluso per uno scarico, venivano esclusi tutti gli scarichi
            stbQ.Append(" AND NOT EXISTS (SELECT 1 FROM  Linee_Preparazioni_Report_Esclusi LPRE " & vbCrLf)
            stbQ.Append("                 INNER JOIN Linee_Preparazioni_Dettagli LPD ON LPRE.Piva = LPD.Piva AND LPRE.Preparazione_Cod = LPD.Preparazione_Cod AND LPRE.Dettaglio_Cod = LPD.Dettaglio_Cod " & vbCrLf)
            stbQ.Append("                 WHERE LPRE.Piva = Linee_Preparazioni.Piva " & vbCrLf)
            stbQ.Append("                 AND LPRE.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " & vbCrLf)
            stbQ.Append("                 AND LPRE.Id_Report = Linee_PreparazionixReport.Id_Report  " & vbCrLf)
            stbQ.Append("                 AND LPD.CAU_MOV = Movimenti.CAU_MOV " & vbCrLf)
            stbQ.Append("                 AND LPD.Elem_Cod = Movimenti_dettagli.Elem_Cod " & vbCrLf)
            'modifica del 05/10/2012: se la preparazione passaggio da registro di vinificazione a registro di commercializzazione
            'utilizzava 'risorse indefinita' come ingrediente e/o preparato
            'il join con Movimenti_dettagli non produceva alcun record e quindi venivano visualizzati entrambi i movimenti:
            'sia lo scarico dal reg vinificazione sia il carico al reg di commerc. e la qta si azzerava
            'INTRODOTTO L'OR per LPD.Pro_Cod - LPD.Mat_Cod - LPD.Udm_Cod 
            stbQ.Append("               AND ( " & vbCrLf)
            stbQ.Append("                       ( " & vbCrLf)
            stbQ.Append("                       LPD.Pro_Cod = Movimenti_dettagli.Pro_Cod " & vbCrLf)
            stbQ.Append("                       AND LPD.Mat_Cod = Movimenti_dettagli.Mat_Cod " & vbCrLf)
            stbQ.Append("                       AND LPD.Udm_Cod = Movimenti_dettagli.Udm_Cod   " & vbCrLf)
            stbQ.Append("                       ) OR ( " & vbCrLf)
            stbQ.Append("                       LPD.Pro_Cod = 0 AND LPD.Mat_Cod = 0 AND LPD.Udm_Cod = 0  " & vbCrLf)
            stbQ.Append("                       ) " & vbCrLf)
            stbQ.Append("                   ) " & vbCrLf)
            stbQ.Append("               ) " & vbCrLf)

            'FILTRO PER GESTIONE DELLA VINIFICAZIONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            stbQ.Append(RegistroVinificazione_FiltroSQL_Importante(objParametri, Lista_PrepCod, Lista_IdTrasf_NoComm, Piva))

            'MODIFICA DEL 18/10/2013: lettura del campo ora (visto che si verifica quello nel controllo del passaggio a registro)
            'stbQ.Append(" GROUP BY Agenda.Id_Agenda, Movimenti.Data_Movimento, Agenda.des_lib, Linee_Preparazioni.Preparazione_Des, Linee_Produzioni.Linea_Des, " + vbCrLf)
            stbQ.Append(" GROUP BY Agenda.Id_Agenda, agenda.lav_cod, Movimenti.Ora, Movimenti.Data_Movimento, Agenda.des_lib, Linee_Preparazioni.Preparazione_Des, Linee_Produzioni.Linea_Des, " & vbCrLf)
            stbQ.Append(" Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Lotto, Trasformazione_Des, " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Udm_Cod, UnitaMisura.Udm_Sim, Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo " & vbCrLf)
            stbQ.Append(", Linee_PreparazionixReport.StrCampo_Registri " & vbCrLf)
            stbQ.Append(", Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione, Linee_Preparazioni.Tipo_Integrazione  " & vbCrLf)

            'MODIFICA DEL 03/06/2014: gestito flag conto terzi, in un caso aggiunge Cod_Contatto_Terzi
            Select Case Gestione_Conto_Terzi
                Case enum_RegistroContoTerzi.Nessuno,
                   enum_RegistroContoTerzi.RegistroGlobale,
                         enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    '0 = se non c'è esiste conto terzi
                    '1 = se esiste, ma è stato richiesto registro unico
                    '3 = questa select va bene anche nel reg c/terzi separato

                Case enum_RegistroContoTerzi.RegistroUnicoDiversificato
                    '2= registro unico ma diversificato per c/lav (MODELLO RUGGERI)
                    stbQ.Append(", Linee_Produzioni.Cod_Contatto_Terzi ")
            End Select

            'If Flag_Conto_Terzi = False Then
            '    'MODIFICA DEL 18/10/2013: lettura del campo ora (visto che si verifica quello nel controllo del passaggio a registro)
            '    'stbQ.Append(" GROUP BY Agenda.Id_Agenda, Movimenti.Data_Movimento, Agenda.des_lib, Linee_Preparazioni.Preparazione_Des, Linee_Produzioni.Linea_Des, " + vbCrLf)
            '    stbQ.Append(" GROUP BY Agenda.Id_Agenda, Movimenti.Ora, Agenda.des_lib, Linee_Preparazioni.Preparazione_Des, Linee_Produzioni.Linea_Des, " + vbCrLf)
            '    stbQ.Append(" Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Lotto, Trasformazione_Des, " + vbCrLf)
            '    stbQ.Append(" Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Udm_Cod, UnitaMisura.Udm_Sim, Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo " + vbCrLf)
            '    stbQ.Append(", Linee_PreparazionixReport.StrCampo_Registri " + vbCrLf)
            '    stbQ.Append(", Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione  " + vbCrLf)
            'Else
            '    If Cod_Contatto_Terzi <> "" Then
            '        'MODIFICA DEL 18/10/2013: lettura del campo ora (visto che si verifica quello nel controllo del passaggio a registro)
            '        'stbQ.Append(" GROUP BY Agenda.Id_Agenda, Movimenti.Data_Movimento, Agenda.des_lib, Linee_Preparazioni.Preparazione_Des, Linee_Produzioni.Linea_Des, " + vbCrLf)
            '        stbQ.Append(" GROUP BY Agenda.Id_Agenda, Movimenti.Ora, Agenda.des_lib, Linee_Preparazioni.Preparazione_Des, Linee_Produzioni.Linea_Des, " + vbCrLf)
            '        stbQ.Append(" Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Lotto, Trasformazione_Des, " + vbCrLf)
            '        stbQ.Append(" Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Udm_Cod, UnitaMisura.Udm_Sim, Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo " + vbCrLf)
            '        stbQ.Append(", Linee_PreparazionixReport.StrCampo_Registri " + vbCrLf)
            '        stbQ.Append(", Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione  " + vbCrLf)
            '    Else
            '        stbQ.Append(" GROUP BY Agenda.Id_Agenda, Movimenti.Ora, Agenda.des_lib, Linee_Preparazioni.Preparazione_Des, Linee_Produzioni.Linea_Des, " + vbCrLf)
            '        stbQ.Append(" Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Lotto, Trasformazione_Des, " + vbCrLf)
            '        stbQ.Append(" Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Udm_Cod, UnitaMisura.Udm_Sim, Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo " + vbCrLf)
            '        stbQ.Append(", Linee_PreparazionixReport.StrCampo_Registri " + vbCrLf)
            '        stbQ.Append(", Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Id_Destinazione  " + vbCrLf)
            '        stbQ.Append(", Linee_Produzioni.Cod_Contatto_Terzi ")
            '    End If
            'End If

            stbQ.Append("  " & vbCrLf)
            stbQ.Append("  " & vbCrLf)
            stbQ.Append(" UNION ALL " & vbCrLf)
            stbQ.Append("  " & vbCrLf)
            stbQ.Append("  " & vbCrLf)

            '=========================================================
            '--------------- 2) AGENDA  (OPERZIONI FUORI DALLA GESTIONE LINEE) ------------------
            '=========================================================

            'MODIFICA DEL 18/10/2013: lettura del campo ora (visto che si verifica quello nel controllo del passaggio a registro)
            'stbQ.Append(" SELECT Agenda.Id_Agenda, Movimenti.Data_Movimento, Agenda.des_lib, Agenda.des_lib AS Descrizione, " + vbCrLf)
            stbQ.Append(" SELECT Agenda.Id_Agenda, agenda.lav_cod, Movimenti.Ora AS Data_Movimento, Agenda.des_lib, Agenda.des_lib + ' del ' + CONVERT(VARCHAR(10), Movimenti.Data_Movimento, 103) AS Descrizione, " & vbCrLf)

            'MODIFICA DEL 03/06/2014
            'stbQ.Append(" CASE  WHEN Movimenti_Dettagli.Pendente IN (9,10) THEN 'Smaltimento ' + Mat_Des ELSE '' END AS Designazione, Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti_dettagli.Elem_Cod, " + vbCrLf)
            Select Case Gestione_Conto_Terzi

                Case enum_RegistroContoTerzi.Nessuno,
                   enum_RegistroContoTerzi.RegistroGlobale,
                         enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    '0 = se non c'è esiste conto terzi
                    '1 = se esiste, ma è stato richiesto registro unico
                    '3 = questa select va bene anche nel reg c/terzi separato
                    stbQ.Append(" CASE  WHEN Movimenti_Dettagli.Pendente IN (9,10) THEN 'Smaltimento ' + Mat_Des ELSE '' END AS Designazione " & vbCrLf)

                Case enum_RegistroContoTerzi.RegistroUnicoDiversificato
                    '2= registro unico ma diversificato per c/lav (MODELLO RUGGERI)
                    stbQ.Append("   ------------------------------------------------- " & vbCrLf)
                    stbQ.Append("   -----------DESIGNAZIONE: LEGGE C/TERZI----------- " & vbCrLf)
                    stbQ.Append("   ------------------------------------------------- " & vbCrLf)
                    stbQ.Append("   ISNULL ( " & vbCrLf)
                    stbQ.Append("   ( " & vbCrLf)
                    stbQ.Append("  " & vbCrLf)
                    stbQ.Append("  " & vbCrLf)

                    'nuova select del 23/03/2016:
                    'ottimizzata + per i prodotti che non sono nè uva nè altre materie prime
                    'non gestisce diversamente le operazioni di carico da quelle di scarico
                    'ma fa la stessa cosa, partendo dal lotto del prodotto risale alla linea del c/lavoro

                    stbQ.Append("   ----------- caso UVE ----------- " & vbCrLf)
                    stbQ.Append("   CASE WHEN (select distinct TOP 1 codice_generazione " & vbCrLf)
                    stbQ.Append("               from OGenerazioni_Anagrafe_Log " & vbCrLf)
                    stbQ.Append("               where OGenerazioni_Anagrafe_Log.elem_cod=movimenti_dettagli.elem_cod  " & vbCrLf)
                    stbQ.Append("               and OGenerazioni_Anagrafe_Log.mat_cod= movimenti_dettagli.mat_cod " & vbCrLf)
                    stbQ.Append("               and tipo_generazione =4 " & vbCrLf)
                    stbQ.Append("               ) in (50,131,165) THEN " & vbCrLf)
                    stbQ.Append("               --se CARICO, SCARICO, RACCOLTA  " & vbCrLf)
                    stbQ.Append("               CASE  WHEN Agenda.lav_cod IN (125,1022,1023)   " & vbCrLf)
                    stbQ.Append("                   THEN" & vbCrLf)
                    stbQ.Append("                   ( " & vbCrLf)
                    stbQ.Append("                   SELECT Imprese.Rag_Soc  AS Designazione " & vbCrLf)
                    stbQ.Append("                   FROM Imprese " & vbCrLf)
                    stbQ.Append("                   WHERE agenda.piva = Imprese.piva   " & vbCrLf)
                    stbQ.Append("                   ) " & vbCrLf)
                    stbQ.Append("               ELSE  -- lav_cod " & vbCrLf)
                    'per tutte le altre operazioni (lav_cod), trattiamo diversamente in base al movimento di carico o scarico
                    stbQ.Append("               (   CASE  WHEN Movimenti.cau_mov = '" & CAU_CARICO & "' " & vbCrLf)
                    stbQ.Append("                       THEN   " & vbCrLf)
                    '                       nel caso di carico l'ingresso avviene con un documento contabile (ddt, doco, mvv
                    stbQ.Append("                       ( SELECT TOP 1 ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS Designazione " & vbCrLf)
                    stbQ.Append("                           FROM Contatti " & vbCrLf)
                    stbQ.Append("                           INNER JOIN Risorse_Umane ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto " & vbCrLf)
                    stbQ.Append("                           INNER JOIN Movimenti MovContabili ON MovContabili.cod_Risum = Risorse_Umane.Cod_Risum " & vbCrLf)
                    stbQ.Append("                           WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda " & vbCrLf)
                    stbQ.Append("                           AND Movimenti.piva = MovContabili.piva  " & vbCrLf)
                    stbQ.Append("                           AND cau_mov='" & CStr(CAU_REGISTRAZIONI) & "' " & vbCrLf)
                    stbQ.Append("                       ) --select mov_contabile " & vbCrLf)
                    stbQ.Append("                   ELSE " & vbCrLf)
                    stbQ.Append("                       ( SELECT TOP 1 ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS Designazione " & vbCrLf)
                    stbQ.Append("                           FROM Contatti " & vbCrLf)
                    stbQ.Append("                           INNER JOIN Linee_Produzioni ON Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto " & vbCrLf)
                    stbQ.Append("                           AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
                    stbQ.Append("                           INNER JOIN  Trasformazioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva " & vbCrLf)
                    stbQ.Append("                           WHERE Linee_Produzioni.Cod_Contatto_Terzi <> '' " & vbCrLf)
                    stbQ.Append("                           AND Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " & vbCrLf)
                    stbQ.Append("                       ) -- SELECT SU LOTTO " & vbCrLf)
                    stbQ.Append("                   END -- case cau_mov " & vbCrLf)
                    stbQ.Append("               ) -- else del lav_cod" & vbCrLf)
                    stbQ.Append("               END -- case lav_cod " & vbCrLf)
                    stbQ.Append("  " & vbCrLf)
                    stbQ.Append("  " & vbCrLf)
                    stbQ.Append("   ----------- caso ALTRE MATERIE PRIME ----------- " & vbCrLf)
                    stbQ.Append("   WHEN EXISTS (select   1  " & vbCrLf)
                    stbQ.Append("               from OGenerazioni_Anagrafe_Log  " & vbCrLf)
                    stbQ.Append("               where OGenerazioni_Anagrafe_Log.elem_cod=movimenti_dettagli.elem_cod " & vbCrLf)
                    stbQ.Append("               and OGenerazioni_Anagrafe_Log.mat_cod= movimenti_dettagli.mat_cod " & vbCrLf)
                    stbQ.Append("               and tipo_generazione =9 " & vbCrLf)
                    stbQ.Append("               )  THEN	 " & vbCrLf)
                    stbQ.Append("                   ( " & vbCrLf)
                    stbQ.Append("                   SELECT TOP 1 rag_soc " & vbCrLf)
                    stbQ.Append("                   FROM Materie_PrimexLotto_Proprieta, Contatti " & vbCrLf)
                    stbQ.Append("                   where piva_superuser = '" & objParametri.PivaSuperUser & "' " & vbCrLf)
                    stbQ.Append("                   and Materie_PrimexLotto_Proprieta.piva = Agenda.PIVA " & vbCrLf)
                    stbQ.Append("                   and id_proprieta = " & CStr(enum_TipoProprieta_Lotto.Categoria_Omni) & "  " & vbCrLf)
                    stbQ.Append("                   and lotto_val1 = movimenti_dettagli.lotto " & vbCrLf)
                    stbQ.Append("                   and elem_cod = movimenti_dettagli.elem_cod  " & vbCrLf)
                    stbQ.Append("                   and mat_cod = movimenti_dettagli.mat_cod " & vbCrLf)
                    '                                   nno serve al momento filtrare lotto_cod1 (si imposta solo nome/lotto)
                    'stbQ.Append("              and lotto_cod1 = 16 " & vbCrLf)
                    stbQ.Append("                   and proprieta_val LIKE '%' + Contatti.cod_contatto + '%' " & vbCrLf)
                    stbQ.Append("                   ) -- FINE ALTRE MATERIE PRIME  " & vbCrLf)
                    stbQ.Append("    " & vbCrLf)
                    stbQ.Append(" ELSE   " & vbCrLf)
                    stbQ.Append("  ------- TUTTO IL RESTO ---------  " & vbCrLf)
                    stbQ.Append("           ( SELECT TOP 1 ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS Designazione " & vbCrLf)
                    stbQ.Append("               FROM Contatti " & vbCrLf)
                    stbQ.Append("               INNER JOIN Linee_Produzioni ON Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto " & vbCrLf)
                    stbQ.Append("               AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
                    stbQ.Append("               INNER JOIN  Trasformazioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva " & vbCrLf)
                    stbQ.Append("               WHERE Linee_Produzioni.Cod_Contatto_Terzi <> '' " & vbCrLf)
                    stbQ.Append("               AND Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " & vbCrLf)
                    stbQ.Append("           ) -- SELECT SU LOTTO " & vbCrLf)
                    stbQ.Append(" END -- case codice generazione  " & vbCrLf)


                    'COMMENTATO IL 23/03/2016
                    'stbQ.Append("   ----------- caso UVE ----------- " + vbCrLf)
                    'stbQ.Append("   CASE WHEN (select distinct TOP 1 codice_generazione " + vbCrLf)
                    'stbQ.Append("               from OGenerazioni_Anagrafe_Log " + vbCrLf)
                    'stbQ.Append("               where OGenerazioni_Anagrafe_Log.elem_cod=movimenti_dettagli.elem_cod  " + vbCrLf)
                    'stbQ.Append("               and OGenerazioni_Anagrafe_Log.mat_cod= movimenti_dettagli.mat_cod " + vbCrLf)
                    'stbQ.Append("               and tipo_generazione =4 " + vbCrLf)
                    'stbQ.Append("               ) in (50,131,165) THEN " + vbCrLf)
                    'stbQ.Append("               --se CARICO, SCARICO, RACCOLTA  " + vbCrLf)
                    'stbQ.Append("               CASE  WHEN Agenda.lav_cod IN (125,1022,1023)   " + vbCrLf)
                    'stbQ.Append("                   THEN" + vbCrLf)
                    'stbQ.Append("                   ( " + vbCrLf)
                    'stbQ.Append("                   SELECT Imprese.Rag_Soc  AS Designazione " + vbCrLf)
                    'stbQ.Append("                   FROM Imprese " + vbCrLf)
                    'stbQ.Append("                   WHERE agenda.piva = Imprese.piva   " + vbCrLf)
                    'stbQ.Append("                   ) " + vbCrLf)
                    'stbQ.Append("               ELSE  -- lav_cod " + vbCrLf)
                    ''per tutte le altre operazioni (lav_cod), trattiamo diversamente in base al movimento di carico o scarico
                    'stbQ.Append("               (   CASE  WHEN Movimenti.cau_mov = '" & CAU_CARICO & "' " + vbCrLf)
                    'stbQ.Append("                       THEN   " + vbCrLf)
                    ''                       nel caso di carico l'ingresso avviene con un documento contabile (ddt, doco, mvv
                    'stbQ.Append("                       ( SELECT TOP 1 ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS Designazione " + vbCrLf)
                    'stbQ.Append("                           FROM Contatti " + vbCrLf)
                    'stbQ.Append("                           INNER JOIN Risorse_Umane ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto " + vbCrLf)
                    'stbQ.Append("                           INNER JOIN Movimenti MovContabili ON MovContabili.cod_Risum = Risorse_Umane.Cod_Risum " + vbCrLf)
                    'stbQ.Append("                           WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda " + vbCrLf)
                    'stbQ.Append("                           AND Movimenti.piva = MovContabili.piva  " + vbCrLf)
                    'stbQ.Append("                           AND cau_mov='" & CStr(CAU_REGISTRAZIONI) & "' " + vbCrLf)
                    'stbQ.Append("                       ) --select mov_contabile " + vbCrLf)
                    'stbQ.Append("                   ELSE " + vbCrLf)
                    'stbQ.Append("                       ( SELECT TOP 1 ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS Designazione " + vbCrLf)
                    'stbQ.Append("                           FROM Contatti " + vbCrLf)
                    'stbQ.Append("                           INNER JOIN Linee_Produzioni ON Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto " + vbCrLf)
                    'stbQ.Append("                           AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " + vbCrLf)
                    'stbQ.Append("                           INNER JOIN  Trasformazioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva " + vbCrLf)
                    'stbQ.Append("                           WHERE Linee_Produzioni.Cod_Contatto_Terzi <> '' " + vbCrLf)
                    'stbQ.Append("                           AND Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " + vbCrLf)
                    'stbQ.Append("                       ) -- SELECT SU LOTTO " + vbCrLf)
                    'stbQ.Append("                   END -- case cau_mov " + vbCrLf)
                    'stbQ.Append("               ) -- else del lav_cod" + vbCrLf)
                    'stbQ.Append("               END -- case lav_cod " + vbCrLf)
                    'stbQ.Append("  " + vbCrLf)
                    'stbQ.Append("  " + vbCrLf)
                    'stbQ.Append("   ----------- caso ALTRE MATERIE PRIME ----------- " + vbCrLf)
                    'stbQ.Append("   WHEN EXISTS (select   1  " + vbCrLf)
                    'stbQ.Append("               from OGenerazioni_Anagrafe_Log  " + vbCrLf)
                    'stbQ.Append("               where OGenerazioni_Anagrafe_Log.elem_cod=movimenti_dettagli.elem_cod " + vbCrLf)
                    'stbQ.Append("               and OGenerazioni_Anagrafe_Log.mat_cod= movimenti_dettagli.mat_cod " + vbCrLf)
                    'stbQ.Append("               and tipo_generazione =9 " + vbCrLf)
                    'stbQ.Append("               )  THEN	 " + vbCrLf)
                    'stbQ.Append("                   ( " + vbCrLf)
                    'stbQ.Append("                   SELECT TOP 1 rag_soc " + vbCrLf)
                    'stbQ.Append("                   FROM Materie_PrimexLotto_Proprieta, Contatti " & vbCrLf)
                    'stbQ.Append("                   where piva_superuser = '" & objParametri.PivaSuperUser & "' " & vbCrLf)
                    'stbQ.Append("                   and Materie_PrimexLotto_Proprieta.piva = Agenda.PIVA " & vbCrLf)
                    'stbQ.Append("                   and id_proprieta = " & CStr(enum_TipoProprieta_Lotto.Categoria_Omni) & "  " & vbCrLf)
                    'stbQ.Append("                   and lotto_val1 = movimenti_dettagli.lotto " & vbCrLf)
                    'stbQ.Append("                   and elem_cod = movimenti_dettagli.elem_cod  " & vbCrLf)
                    'stbQ.Append("                   and mat_cod = movimenti_dettagli.mat_cod " & vbCrLf)
                    ''                                   nno serve al momento filtrare lotto_cod1 (si imposta solo nome/lotto)
                    ''stbQ.Append("              and lotto_cod1 = 16 " & vbCrLf)
                    'stbQ.Append("                   and proprieta_val LIKE '%' + Contatti.cod_contatto + '%' " & vbCrLf)
                    'stbQ.Append("                   ) " + vbCrLf)
                    'stbQ.Append("    " + vbCrLf)
                    'stbQ.Append(" ELSE   " + vbCrLf)
                    'stbQ.Append("  ------- TUTTO IL RESTO ---------  " + vbCrLf)
                    ''modifica del 26/02/2016:
                    ''per il passaggio a commercializzazione non han mai usato la vera operazione
                    ''ma uno scarico di vino dalla vasca con descrizione libera
                    ''gli scarichi di magazzino erano battezzati solo per l'azienda superuser
                    ''bisogna quindi modifiacre e fargli fare la stessa cosa che viene fatta per il cau_scarico

                    '''nel caso dei carichi e scarichi "secchi" di magazzino e della raccolta, non c'è il movimento contabile e quindi si prende come riferimento la piva dell'operazione (che sarà quella dell'azienda superuser)
                    ''stbQ.Append("       CASE  WHEN Agenda.lav_cod IN (" & LAVCOD_RACCOLTA & "," & LAVCOD_CARICO & "," & LAVCOD_SCARICO & ") " + vbCrLf)
                    ''stbQ.Append("       THEN " + vbCrLf)
                    ''stbQ.Append("       ( " + vbCrLf)
                    ''stbQ.Append("       SELECT Imprese.Rag_Soc  AS Designazione " + vbCrLf)
                    ''stbQ.Append("       FROM Imprese " + vbCrLf)
                    ''stbQ.Append("       WHERE agenda.piva = Imprese.piva   " + vbCrLf)
                    ''stbQ.Append("       ) " + vbCrLf)
                    ''stbQ.Append("       ELSE  -- lav_cod " + vbCrLf)
                    ''stbQ.Append("  " + vbCrLf)
                    ''per tutte le altre operazioni (lav_cod), trattiamo diversamente in base al movimento di carico o scarico
                    'stbQ.Append("       (   CASE  WHEN Movimenti.cau_mov = '" & CAU_CARICO & "' " + vbCrLf)
                    'stbQ.Append("           THEN   " + vbCrLf)
                    ''                       nel caso di carico l'ingresso avviene con un documento contabile (ddt, doco, mvv
                    'stbQ.Append("           ( SELECT TOP 1 ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS Designazione " + vbCrLf)
                    'stbQ.Append("               FROM Contatti " + vbCrLf)
                    'stbQ.Append("               INNER JOIN Risorse_Umane ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto " + vbCrLf)
                    'stbQ.Append("               INNER JOIN Movimenti MovContabili ON MovContabili.cod_Risum = Risorse_Umane.Cod_Risum " + vbCrLf)
                    'stbQ.Append("               WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda " + vbCrLf)
                    'stbQ.Append("               AND Movimenti.piva = MovContabili.piva  " + vbCrLf)
                    'stbQ.Append("               AND cau_mov='" & CStr(CAU_REGISTRAZIONI) & "' " + vbCrLf)
                    'stbQ.Append("           ) --select mov_contabile " + vbCrLf)
                    'stbQ.Append("           ELSE " + vbCrLf)
                    'stbQ.Append("           ( SELECT TOP 1 ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS Designazione " + vbCrLf)
                    'stbQ.Append("               FROM Contatti " + vbCrLf)
                    'stbQ.Append("               INNER JOIN Linee_Produzioni ON Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto " + vbCrLf)
                    'stbQ.Append("               AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " + vbCrLf)
                    'stbQ.Append("               INNER JOIN  Trasformazioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva " + vbCrLf)
                    'stbQ.Append("               WHERE Linee_Produzioni.Cod_Contatto_Terzi <> '' " + vbCrLf)
                    'stbQ.Append("               AND Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " + vbCrLf)
                    'stbQ.Append("           ) -- SELECT SU LOTTO " + vbCrLf)
                    'stbQ.Append("           END -- case cau_mov " + vbCrLf)
                    'stbQ.Append("       ) " + vbCrLf)
                    ''stbQ.Append("    END -- case lav_cod " + vbCrLf)

                    'stbQ.Append(" END -- case codice generazione  " + vbCrLf)

                    '''nel caso dei carichi e scarichi "secchi" di magazzino e della raccolta, non c'è il movimento contabile e quindi si prende come riferimento la piva dell'operazione (che sarà quella dell'azienda superuser)
                    ''stbQ.Append("       CASE  WHEN Agenda.lav_cod IN (" & LAVCOD_RACCOLTA & "," & LAVCOD_CARICO & "," & LAVCOD_SCARICO & ") " + vbCrLf)
                    ''stbQ.Append("       THEN " + vbCrLf)
                    ''stbQ.Append("       ( " + vbCrLf)
                    ''stbQ.Append("       SELECT Imprese.Rag_Soc  AS Designazione " + vbCrLf)
                    ''stbQ.Append("       FROM Imprese " + vbCrLf)
                    ''stbQ.Append("       WHERE agenda.piva = Imprese.piva   " + vbCrLf)
                    ''stbQ.Append("       ) " + vbCrLf)
                    ''stbQ.Append("       ELSE  -- lav_cod " + vbCrLf)
                    ''stbQ.Append("  " + vbCrLf)
                    '''per tutte le altre operazioni (lav_cod), trattiamo diversamente in base al movimento di carico o scarico
                    ''stbQ.Append("       (   CASE  WHEN Movimenti.cau_mov = '" & CAU_CARICO & "' " + vbCrLf)
                    ''stbQ.Append("           THEN   " + vbCrLf)
                    '''                       nel caso di carico l'ingresso avviene con un documento contabile (ddt, doco, mvv
                    ''stbQ.Append("           ( SELECT TOP 1 ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS Designazione " + vbCrLf)
                    ''stbQ.Append("               FROM Contatti " + vbCrLf)
                    ''stbQ.Append("               INNER JOIN Risorse_Umane ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto " + vbCrLf)
                    ''stbQ.Append("               INNER JOIN Movimenti MovContabili ON MovContabili.cod_Risum = Risorse_Umane.Cod_Risum " + vbCrLf)
                    ''stbQ.Append("               WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda " + vbCrLf)
                    ''stbQ.Append("               AND Movimenti.piva = MovContabili.piva  " + vbCrLf)
                    ''stbQ.Append("               AND cau_mov='" & CStr(CAU_REGISTRAZIONI) & "' " + vbCrLf)
                    ''stbQ.Append("           ) --select mov_contabile " + vbCrLf)
                    ''stbQ.Append("           ELSE " + vbCrLf)
                    ''stbQ.Append("           ( SELECT TOP 1 ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS Designazione " + vbCrLf)
                    ''stbQ.Append("               FROM Contatti " + vbCrLf)
                    ''stbQ.Append("               INNER JOIN Linee_Produzioni ON Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto " + vbCrLf)
                    ''stbQ.Append("               AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " + vbCrLf)
                    ''stbQ.Append("               INNER JOIN  Trasformazioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva " + vbCrLf)
                    ''stbQ.Append("               WHERE Linee_Produzioni.Cod_Contatto_Terzi <> '' " + vbCrLf)
                    ''stbQ.Append("               AND Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " + vbCrLf)
                    ''stbQ.Append("           ) -- SELECT SU LOTTO " + vbCrLf)
                    ''stbQ.Append("           END -- case cau_mov " + vbCrLf)
                    ''stbQ.Append("       ) " + vbCrLf)
                    ''stbQ.Append("    END -- case lav_cod " + vbCrLf)




                    stbQ.Append("   ) -- contenuto isnull " & vbCrLf)
                    stbQ.Append("   , '') -- isnull   " & vbCrLf)
                    stbQ.Append("   AS Designazione   " & vbCrLf)
                    stbQ.Append("   --- FINE DESIGNAZIONE C/TERZI----------- " & vbCrLf)
            End Select

            'If Flag_Conto_Terzi = False Then
            '    'se non c'è conto terzi abilitato
            '    stbQ.Append(" CASE  WHEN Movimenti_Dettagli.Pendente IN (9,10) THEN 'Smaltimento ' + Mat_Des ELSE '' END AS Designazione, " + vbCrLf)
            'Else
            '    If Cod_Contatto_Terzi <> "" Then
            '        'se è impostato il filtro del c/terzi nel menù a tendina
            '        stbQ.Append(" CASE  WHEN Movimenti_Dettagli.Pendente IN (9,10) THEN 'Smaltimento ' + Mat_Des ELSE '' END AS Designazione, " + vbCrLf)
            '    Else
            '        'registro massivo delle linee c/Terzi
            '        ' stbQ.Append(" ( CASE  WHEN Movimenti_Dettagli.Pendente IN (9,10) THEN 'Smaltimento ' + Mat_Des ELSE '' END ) " + vbCrLf)
            '        'stbQ.Append(" + ")

            '        ' stbQ.Append(" SELECT TOP 1 Designazione FROM ( " + vbCrLf)
            '        stbQ.Append("       ( CASE  WHEN Movimenti.cau_mov = '" & CAU_CARICO & "' " + vbCrLf)
            '        stbQ.Append("        THEN   " + vbCrLf)
            '        stbQ.Append("       ( SELECT TOP 1 ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS Designazione " + vbCrLf)
            '        stbQ.Append("               FROM Contatti " + vbCrLf)
            '        stbQ.Append("               INNER JOIN Risorse_Umane ON Contatti.Piva = Risorse_Umane.Piva AND Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto " + vbCrLf)
            '        stbQ.Append("               INNER JOIN Movimenti MovContabili ON MovContabili.cod_Risum = Risorse_Umane.Cod_Risum " + vbCrLf)
            '        stbQ.Append("               WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda " + vbCrLf)
            '        stbQ.Append("               AND Movimenti.piva = MovContabili.piva  " + vbCrLf)
            '        stbQ.Append("               AND cau_mov='" & CStr(CAU_REGISTRAZIONI) & "' " + vbCrLf)
            '        ' stbQ.Append("               AND MovContabili.Cod_RisUm in ( " & Agro_SQL_SaveText(Lista_CodRisUm) & ") " + vbCrLf)
            '        stbQ.Append("       ) " + vbCrLf)
            '        stbQ.Append("       ELSE " + vbCrLf)
            '        'stbQ.Append("       ( Movimenti.cau_mov = '" & CAU_SCARICO & "' " + vbCrLf)
            '        stbQ.Append("       ( SELECT TOP 1 ISNULL(Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome, '') AS Designazione " + vbCrLf)
            '        stbQ.Append("               FROM Contatti " + vbCrLf)
            '        stbQ.Append("           INNER JOIN Linee_Produzioni ON Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto " + vbCrLf)
            '        stbQ.Append("           AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " + vbCrLf)
            '        stbQ.Append("           INNER JOIN  Trasformazioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva " + vbCrLf)
            '        stbQ.Append("           WHERE Linee_Produzioni.Cod_Contatto_Terzi <> '' " + vbCrLf)
            '        stbQ.Append("           AND Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " + vbCrLf)
            '        stbQ.Append("       ) -- SELECT SU LOTTO " + vbCrLf)
            '        stbQ.Append("       END  " + vbCrLf)
            '        stbQ.Append("       ) -- CASE   " + vbCrLf)
            '        '  stbQ.Append("   ) -- SELECT INTERNA   " + vbCrLf)   
            '    End If
            'End If

            stbQ.Append(" , Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti_dettagli.Elem_Cod,  " & vbCrLf)

            stbQ.Append(" Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Mov_Det_Des, Movimenti_dettagli.Lotto,  " & vbCrLf)
            stbQ.Append(" CASE WHEN CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=2 THEN Movimenti_dettagli.Qta ELSE 0 END AS CaricoKg, " & vbCrLf)
            stbQ.Append(" CASE WHEN CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=2 THEN Movimenti_dettagli.Qta ELSE 0 END AS ScaricoKg, " & vbCrLf)
            stbQ.Append(" CASE WHEN CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=29 THEN Movimenti_dettagli.Qta ELSE 0 END AS CaricoLt, " & vbCrLf)
            stbQ.Append(" CASE WHEN CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=29 THEN Movimenti_dettagli.Qta ELSE 0 END AS ScaricoLt," & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Udm_Cod, UnitaMisura.UDM_SIM, " & vbCrLf)
            stbQ.Append(" Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo, " & vbCrLf)

            'MODIFICA DEL 18/06/2014, per gestire numerazione particolare dei ddt ricevuti conferimento
            'stbQ.Append(" (SELECT Doc_Numero_Sin + CAST(Doc_Numero AS Varchar(100)) + doc_numero_des FROM Movimenti MovContabili WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda and cau_mov='4000') AS NDoc " + vbCrLf)
            stbQ.Append(" ISNULL( ( CASE WHEN agenda.lav_cod IN (" & LAVCOD_ACCETTAZIONE_DIVERSI & ") " & vbCrLf)
            stbQ.Append(" THEN (SELECT Doc_Numero_Sin + right('00000' + CAST(Doc_Numero AS Varchar(100)),5) + doc_numero_des FROM Movimenti MovContabili WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda and cau_mov='" & CAU_REGISTRAZIONI & "') " & vbCrLf)
            stbQ.Append(" ELSE (SELECT Doc_Numero_Sin + CAST(Doc_Numero AS Varchar(100)) + doc_numero_des FROM Movimenti MovContabili WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda and cau_mov='" & CAU_REGISTRAZIONI & "') " & vbCrLf)
            stbQ.Append(" END), '') AS NDoc " & vbCrLf)

            stbQ.Append(", '' AS StrCampo_Registri " & vbCrLf)
            ' Movimenti.Doc_Numero_Sin + CAST(Movimenti.Doc_Numero AS varchar(10)) + Doc_Numero_Des AS NDoc  ")
            'inserito il 15/10/2012 per stampare il numero di vasca
            stbQ.Append(", Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Sa_Cod AS SaCod_Dest, Mov_Destinazioni.Id_Destinazione AS Id_Dest " & vbCrLf)
            stbQ.Append(" , 0 AS Tipo_Integrazione   " & vbCrLf)

            stbQ.Append(" FROM  Materie_PrimexReport " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Materie_PrimexReport.Mat_Cod = Materie_Prime.Mat_Cod AND Materie_PrimexReport.Piva = Materie_Prime.Piva  " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND Movimenti.Id_Agenda = Agenda.Id_Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD " & vbCrLf)

            'inserito il 15/10/2012 per stampare il numero di vasca
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            '15/04/2013: inutile il left, ci sono due where sulla tabella
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " + vbCrLf)
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " + vbCrLf)

            'nota del 17/06/2014: siam d'accordo con Marco di non introdurre il join su AND Materie_Prime_ParametriQualitativi.PIVA = Materie_Prime.piva
            'non ci devono essere record duplicati per lo stesso mat_cod -> introduciamo una query di delete di sicurezza
            stbQ.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod  " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " & vbCrLf)

            'stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva = Agenda.Piva" + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & "  " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.elem_cod = Movimenti_dettagli.elem_cod " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.mat_cod = Movimenti_dettagli.mat_cod " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.id =(             " + vbCrLf)
            'stbQ.Append("                                   SELECT MAX(id) " + vbCrLf)
            'stbQ.Append("                                    FROM OGenerazioni_Anagrafe_Log OLOG " + vbCrLf)
            'stbQ.Append("                                    WHERE OLOG.piva_superuser = OGenerazioni_Anagrafe_Log.Piva_SuperUser " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.piva = OLOG.Piva " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = OLOG.Modulo_Generazione" + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.elem_cod = OLOG.elem_cod  " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.mat_cod = OLOG.mat_cod " + vbCrLf)
            'stbQ.Append("                                   ) " + vbCrLf)

            'stbQ.Append(" WHERE Movimenti.CAU_MOV IN ('" + CAU_CARICO + "', '" + CAU_SCARICO + "') " + vbCrLf)

            stbQ.Append(" WHERE Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                                        "'" & CAU_SCARICO & "', " &
                                                        "'" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', " &
                                                        "'" & CAU_CONFERIMENTO & "', " &
                                                        "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                                        ") " & vbCrLf)

            stbQ.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If str_idagenda_filtrocategoria <> "" Then
                stbQ.Append(" AND Agenda.Id_Agenda IN " & Agro_SQL_Save_Clausola_IN(str_idagenda_filtrocategoria) & " " & vbCrLf)
            End If
            If str_matcod_filtrocategoria <> "" Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod IN " & Agro_SQL_Save_Clausola_IN(str_matcod_filtrocategoria) & " " & vbCrLf)
            End If
            If str_lotto_filtrocategoria <> "" Then
                stbQ.Append(" AND Movimenti_Dettagli.Lotto IN " & Agro_SQL_Save_Clausola_IN(str_lotto_filtrocategoria, True) & " " & vbCrLf)
            End If

            '--------------
            '21/08/2014: adeguato il filtro con la select (viaggia tutto sul campo Ora)
            'stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " " + vbCrLf)
            stbQ.Append(" AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " " & vbCrLf)
            stbQ.Append(" AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " " & vbCrLf)

            stbQ.Append(" AND Materie_PrimexReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)

            '--------------
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)

            'aggiunta il 19/08/13:per escludere carichi/scarichi di operazioni pendenti (pianificate)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            stbQ.Append(" AND Materie_Prime_Calibri.Piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")
            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = 'calibro' ")
            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri_Vinificazione = 1 ")

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Id_Destinazione <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " & vbCrLf)
            End If
            If Cal_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime_ParametriQualitativi.tipo_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If
            If Linea_Cod <> 0 Then
                stbQ.Append("   AND  EXISTS (SELECT 1  " & vbCrLf)
                stbQ.Append("               FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
                stbQ.Append(" 				WHERE materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod " & vbCrLf)
                stbQ.Append(" 				AND materie_prime.elem_cod = OGenerazioni_Anagrafe_Log.elem_cod " & vbCrLf)
                stbQ.Append("               AND OGenerazioni_Anagrafe_Log.linea_cod = " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
                stbQ.Append("               ) " & vbCrLf)
            End If
            If Cau_Mov <> "" Then
                stbQ.Append(" AND Movimenti.CAU_MOV = '" & Agro_SQL_SaveText(Cau_Mov) & "'" & vbCrLf)
            End If
            If Cod_Contatto_Terzi <> "" Then
                'modifica del 21/01/2013: separato il filtro per la parte agenda e la parte linee
                stbQ.Append(RegistroVinificazione_FiltroSQL_ContoTerzi_ParteAgenda(objParametri.PivaSuperUser,
                                                                                    Piva,
                                                                                    Cod_Contatto_Terzi,
                                                                                    Lista_CodRisUm))
                ''aggiunta in data 25/09/2012:
                ''stampa del registro separata per fornitore uve (Cod_Contatto_Terzi)
                'stbQ.Append("   AND  EXISTS (SELECT 1  " + vbCrLf)
                'stbQ.Append("               FROM OGenerazioni_Anagrafe_Log " + vbCrLf)
                'stbQ.Append(" 				INNER JOIN  Linee_Produzioni ON OGenerazioni_Anagrafe_Log.linea_cod =Linee_Produzioni.linea_cod  " + vbCrLf)
                'stbQ.Append(" 				WHERE materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod " + vbCrLf)
                'stbQ.Append("               AND Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_Contatto_Terzi) & "' " + vbCrLf)
                'stbQ.Append("               ) " + vbCrLf)
            End If

            'stbQ.Append(" AND EXISTS (SELECT 1 FROM Agenda Agenda_Lav INNER JOIN OperazionixReport OPR ON Agenda_Lav.Lav_Cod = OPR.Lav_Cod " + vbCrLf)
            'stbQ.Append("             AND OPR.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " + vbCrLf)
            'stbQ.Append("             WHERE  Agenda.Piva = Agenda_Lav.Piva AND Agenda.Id_Agenda = Agenda_Lav.Id_Agenda ) " + vbCrLf)
            'CORREZIONE DEL 07/05/2014: la piva di OperazionixReport è quella del superuser,
            'non va quindi messa in join con la piva di agenda!
            'è stato scoperto ora perchè qualitoscana è il primo cliente col quale si stampano i registri di cantina sulle aziende figlie in gerarchia
            stbQ.Append(" AND EXISTS (SELECT 1 FROM OperazionixReport OPR  " & vbCrLf)
            stbQ.Append("             WHERE  OPR.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append("             AND OPR.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)
            stbQ.Append("               AND Agenda.Lav_Cod = OPR.Lav_Cod ) " & vbCrLf)


            'FILTRO PER GESTIONE DELLA VINIFICAZIONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            stbQ.Append(RegistroVinificazione_FiltroSQL_Importante(objParametri, Lista_PrepCod, Lista_IdTrasf_NoComm, Piva))

            'stbQ.Append(" ORDER BY  Movimenti.Data_Movimento, Agenda.Id_Agenda, Movimenti.Cau_Mov DESC " + vbCrLf)
            'MODIFICA DEL 18/10/2013: lettura del campo ora (visto che si verifica quello nel controllo del passaggio a registro)
            'modifica del 15/10/2014: aggiunto Elem_Cod DESC per avere i cali per ultimi
            stbQ.Append(" ORDER BY  Movimenti.Ora, Agenda.Id_Agenda, Movimenti.Cau_Mov DESC, Movimenti_dettagli.Elem_Cod DESC " & vbCrLf)

            'modifica del 14/10/2014: non carico il dataset, ma ritorno il datatable
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            'Risp = False
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        '  Return Risp
        Return DT

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' registro di vinificazione: riepilogo
    ''' DataRiepilogoInizio è 01/01/1900
    ''' i movimenti vengono letti dall'01/01/1900 alla data di fine
    ''' mentre nel select ci sono i SUM sull'intervallo selezionato
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RegistroVinificazioneRiepilogo(ByVal Piva As String,
                                                    ByVal Id_Report As Integer,
                                                    ByVal DataRiepilogoInizio As Date,
                                                    ByVal DataReportInizio As Date,
                                                    ByVal DataReportFine As Date,
                                                    ByVal Cod_Contatto_Terzi As String,
                                                    ByVal Lista_CodRisUm As String,
                                                    ByVal Lista_PrepCod As String,
                                                    ByVal Lista_IdTrasf_NoComm As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Id_Destinazione As Integer,
                                                    ByVal Cal_Cod As Integer,
                                                    ByVal Mat_Cod As Integer,
                                                    ByVal Linea_Cod As Integer,
                                                    ByVal Cau_Mov As String,
                                                    ByVal Gestione_Conto_Terzi As enum_RegistroContoTerzi,
                                                    ByVal str_idagenda_filtrocategoria As String,
                                                    ByVal str_matcod_filtrocategoria As String,
                                                    ByVal str_lotto_filtrocategoria As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        'ByVal DataFine As Date, _
        ' ByVal Opt_Gestione_RegistroVinificazione As Integer, _

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.RegistriCantina.RegistroVinificazioneRiepilogo"

        Dim MessaggioErrore As String = ""
        Dim stbQ As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim DataFineControllo As Date
            DataFineControllo = DateAdd(DateInterval.Day, 1, DataReportFine)

            '=========================================================
            '--------------- 1) PREPARAZIONI ------------------
            '=========================================================

            stbQ.Append(" ( " & vbCrLf)

            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            stbQ.Append(" SELECT Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Movimenti_Dettagli.Lotto,  " & vbCrLf)

            '21/08/14: adeguamento gestione al campo ora
            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=2 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS CaricoKg, " + vbCrLf)
            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=2 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS ScaricoKg, " + vbCrLf)
            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS CaricoLt, " + vbCrLf)
            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS ScaricoLt, " + vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=2 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS CaricoKg, " & vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=2 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS ScaricoKg, " & vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS CaricoLt, " & vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS ScaricoLt, " & vbCrLf)

            stbQ.Append(" Materie_Prime_Calibri.Cal_Cod, Materie_Prime_Calibri.Cal_Des, Movimenti_dettagli.Udm_Cod, " & vbCrLf)

            'MODIFICA DEL 19/10/2012:
            'ottimizzata: leggo cifra_start cifra_end e Lotto_Cod direttamente in una query
            'e CORRETTO BUG: non veniva cercato l'anno di vendemmia
            stbQ.Append("                ISNULL((SELECT TOP 1   " & vbCrLf)
            stbQ.Append("                ( CONVERT(varchar(250), Materie_PrimexLotto_Configurazione.Cifra_Start) + '|' + " & vbCrLf)
            stbQ.Append("                CONVERT(varchar(250), Materie_PrimexLotto_Configurazione.Cifra_End) + '|' + " & vbCrLf)
            stbQ.Append("                CONVERT(varchar(250), Materie_PrimexLotto_Configurazione.Lotto_Cod) ) AS ConfigLotto " & vbCrLf)
            stbQ.Append("                FROM Lotto_Configurazione " & vbCrLf)
            stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser   " & vbCrLf)
            stbQ.Append("                AND Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod   " & vbCrLf)
            stbQ.Append("                AND Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva   " & vbCrLf)
            stbQ.Append("                AND Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " & vbCrLf)
            stbQ.Append("                WHERE (Lotto_Des like '%Anno%produzione%' OR Lotto_Des like '%Anno%vendemmia%')  " & vbCrLf)
            stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " & vbCrLf)
            stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " & vbCrLf)
            stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),'0|0|0') AS ConfigLotto " & vbCrLf)

            ''MODIFICA GESTIONE CIFRA_START E CIFRA_END DEL 21/02/2012
            'stbQ.Append("                ISNULL((SELECT TOP 1 Materie_PrimexLotto_Configurazione.Cifra_Start  " + vbCrLf)
            'stbQ.Append("                FROM Lotto_Configurazione " + vbCrLf)
            'stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " + vbCrLf)
            'stbQ.Append("                WHERE Lotto_Des like '%Anno%produzione%'  " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),0) AS Cifra_Start, " + vbCrLf)

            'stbQ.Append("                ISNULL((SELECT TOP 1 Materie_PrimexLotto_Configurazione.Cifra_End " + vbCrLf)
            'stbQ.Append("                FROM Lotto_Configurazione " + vbCrLf)
            'stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " + vbCrLf)
            'stbQ.Append("                WHERE Lotto_Des like '%Anno%produzione%'  " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),0) AS Cifra_End, " + vbCrLf)

            'stbQ.Append("                ISNULL((SELECT TOP 1 Materie_PrimexLotto_Configurazione.Lotto_Cod " + vbCrLf)
            'stbQ.Append("                FROM Lotto_Configurazione " + vbCrLf)
            'stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " + vbCrLf)
            'stbQ.Append("                WHERE Lotto_Des like '%Anno%produzione%'  " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),0) AS Lotto_Cod " + vbCrLf)

            Select Case Gestione_Conto_Terzi
                Case enum_RegistroContoTerzi.Nessuno,
                   enum_RegistroContoTerzi.RegistroGlobale,
                         enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    '0 = se non c'è esiste conto terzi
                    '1 = se esiste, ma è stato richiesto registro unico
                    '3 = questa select va bene anche nel reg c/terzi separato
                Case enum_RegistroContoTerzi.RegistroUnicoDiversificato
                    '2= registro unico ma diversificato per c/lav (MODELLO RUGGERI)
                    stbQ.Append(" , Cod_Contatto_Terzi " & vbCrLf)
            End Select



            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            stbQ.Append(" FROM Linee_PreparazionixReport " & vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Preparazioni ON Linee_PreparazionixReport.Piva = Linee_Preparazioni.Piva AND Linee_PreparazionixReport.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Linee_Preparazioni.Preparazione_Cod = Agenda.PREPARAZIONE_COD AND Linee_Preparazioni.Piva = Agenda.PIVA " & vbCrLf)

            'Modifica del 27/04/2012 by Maga&Marco:
            'nella vinificazione c'è sempre la linea_Produzione, quindi mettiamo inner join 
            'e aggiungiamo join della piva (altrimenti vengono sdoppiati i record a causa della piva AAAAAAAAAAA)
            'stbQ.Append(" LEFT JOIN Linee_Produzioni ON Agenda.Linea_Cod = Linee_Produzioni.Linea_Cod " + vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Produzioni ON Agenda.Piva = Linee_Produzioni.Piva AND Agenda.Linea_Cod = Linee_Produzioni.Linea_Cod " & vbCrLf)

            'nel riepilogo deve venir su anche l'uva vendemmiata
            'stbQ.Append(" INNER JOIN Trasformazioni ON Linee_Produzioni.Linea_Cod = Trasformazioni.Linea_Cod ")

            stbQ.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod AND Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_dettagli.Udm_Cod " & vbCrLf)

            'inserito il 10/04/2013 x filtro id_dest
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            '15/04/2013: inutile il left, ci sono due where sulla tabella
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " + vbCrLf)
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " + vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " & vbCrLf)

            'stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva = Agenda.Piva" + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & "  " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.elem_cod = Movimenti_dettagli.elem_cod " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.mat_cod = Movimenti_dettagli.mat_cod " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.id =(             " + vbCrLf)
            'stbQ.Append("                                   SELECT MAX(id) " + vbCrLf)
            'stbQ.Append("                                    FROM OGenerazioni_Anagrafe_Log OLOG " + vbCrLf)
            'stbQ.Append("                                    WHERE OLOG.piva_superuser = OGenerazioni_Anagrafe_Log.Piva_SuperUser " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.piva = OLOG.Piva " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = OLOG.Modulo_Generazione" + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.elem_cod = OLOG.elem_cod  " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.mat_cod = OLOG.mat_cod " + vbCrLf)
            'stbQ.Append("                                   ) " + vbCrLf)

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            stbQ.Append(" WHERE Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)

            'MODIFICA DEL 29/10/2014
            'LEONARDI CI HA DETTO CHE NON VANNO CONTEGGIATI I SALDI DEI CALI: LI ESCLUDIAMO DALLA QUERY DI RIEPILOGO
            stbQ.Append(" AND Movimenti_dettagli.Elem_Cod <> " & Agro_SQL_SaveNum(CALI_LAVORAZIONE) & "  " & vbCrLf)

            stbQ.Append(" AND Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                                    "'" & CAU_SCARICO & "', " &
                                                    "'" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', " &
                                                    "'" & CAU_CONFERIMENTO & "', " &
                                                    "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                                    ") " & vbCrLf)

            stbQ.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If str_idagenda_filtrocategoria <> "" Then
                stbQ.Append(" AND Agenda.Id_Agenda IN " & Agro_SQL_Save_Clausola_IN(str_idagenda_filtrocategoria) & " " & vbCrLf)
            End If
            If str_matcod_filtrocategoria <> "" Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod IN " & Agro_SQL_Save_Clausola_IN(str_matcod_filtrocategoria) & " " & vbCrLf)
            End If
            If str_lotto_filtrocategoria <> "" Then
                stbQ.Append(" AND Movimenti_Dettagli.Lotto IN " & Agro_SQL_Save_Clausola_IN(str_lotto_filtrocategoria, True) & " " & vbCrLf)
            End If

            'nel riepilogo non ci vanno solo i prodotti movimentati nel mese
            'stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " ")
            'stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " ")
            '--------------
            '21/08/2014: adeguato il filtro con la select (viaggia tutto sul campo Ora)
            'stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataRiepilogoInizio) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " " + vbCrLf)
            stbQ.Append(" AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataRiepilogoInizio) & " " & vbCrLf)
            stbQ.Append(" AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " " & vbCrLf)

            '--------------
            'aggiunto in data 23/09/2013
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            stbQ.Append(" AND   Materie_Prime_Calibri.Piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")
            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = 'calibro' " & vbCrLf)
            'modifica del 21/09/2010: sostituito ChkRegistri con ChkRegistri_Vinificazione
            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri_Vinificazione = 1 " & vbCrLf)

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Id_Destinazione <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " & vbCrLf)
            End If
            If Cal_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime_ParametriQualitativi.tipo_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If
            If Linea_Cod <> 0 Then
                stbQ.Append(" AND Linee_Produzioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "  " & vbCrLf)
            End If
            If Cau_Mov <> "" Then
                stbQ.Append(" AND Movimenti.CAU_MOV = '" & Agro_SQL_SaveText(Cau_Mov) & "'" & vbCrLf)
            End If

            'correzione del 9/11/2010: mancavano i join con Movimenti_dettagli, per cui
            'se c'era almeno un dettaglio escluso per uno scarico, venivano esclusi tutti gli scarichi
            stbQ.Append(" AND NOT EXISTS (SELECT 1 FROM  Linee_Preparazioni_Report_Esclusi LPRE " & vbCrLf)
            stbQ.Append("                 INNER JOIN Linee_Preparazioni_Dettagli LPD ON LPRE.Piva = LPD.Piva AND LPRE.Preparazione_Cod = LPD.Preparazione_Cod AND LPRE.Dettaglio_Cod = LPD.Dettaglio_Cod " & vbCrLf)
            stbQ.Append("                 WHERE LPRE.Piva = Linee_Preparazioni.Piva " & vbCrLf)
            stbQ.Append("                 AND LPRE.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " & vbCrLf)
            stbQ.Append("                 AND LPRE.Id_Report = Linee_PreparazionixReport.Id_Report  " & vbCrLf)
            stbQ.Append("                 AND LPD.CAU_MOV = Movimenti.CAU_MOV " & vbCrLf)
            stbQ.Append("                 AND LPD.Elem_Cod = Movimenti_dettagli.Elem_Cod " & vbCrLf)
            'modifica del 05/10/2012: se la preparazione passaggio da registro di vinificazione a registro di commercializzazione
            'utilizzava 'risorse indefinita' come ingrediente e/o preparato
            'il join con Movimenti_dettagli non produceva alcun record e quindi venivano visualizzati entrambi i movimenti:
            'sia lo scarico dal reg vinificazione sia il carico al reg di commerc. e la qta si azzerava
            'INTRODOTTO L'OR per LPD.Pro_Cod - LPD.Mat_Cod - LPD.Udm_Cod 
            stbQ.Append("               AND ( " & vbCrLf)
            stbQ.Append("                       ( " & vbCrLf)
            stbQ.Append("                       LPD.Pro_Cod = Movimenti_dettagli.Pro_Cod " & vbCrLf)
            stbQ.Append("                       AND LPD.Mat_Cod = Movimenti_dettagli.Mat_Cod " & vbCrLf)
            stbQ.Append("                       AND LPD.Udm_Cod = Movimenti_dettagli.Udm_Cod   " & vbCrLf)
            stbQ.Append("                       ) OR ( " & vbCrLf)
            stbQ.Append("                       LPD.Pro_Cod = 0 AND LPD.Mat_Cod = 0 AND LPD.Udm_Cod = 0  " & vbCrLf)
            stbQ.Append("                       ) " & vbCrLf)
            stbQ.Append("                   ) " & vbCrLf)
            stbQ.Append("               ) " & vbCrLf)

            ''''stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataFine) & " ")
            ''''stbQ.Append(" AND (Trasformazioni.Validita_Fine = " & Agro_SQL_SaveDate(#12/31/2100#) & " ") ' trasformazioni ancora attive
            ''''stbQ.Append(" OR (Trasformazioni.Validita_Fine <= " & Agro_SQL_SaveDate(DataReportFine) & " ")  ' trasformazioni attive nell'intervallo selezionato
            ''''stbQ.Append("     AND Trasformazioni.Validita_Fine >= " & Agro_SQL_SaveDate(DataReportInizio) & ")) ")

            If Cod_Contatto_Terzi <> "" Then
                'modifica del 21/01/2013: separato il filtro per la parte agenda e la parte linee
                stbQ.Append(RegistroVinificazione_FiltroSQL_ContoTerzi_ParteLinee(Piva, Cod_Contatto_Terzi))

                ''aggiunta in data 27/04/2012: gestione conto terzi
                ''modificata in data 25/09/2012:
                ''stampa del registro separata per fornitore uve (Cod_Contatto_Terzi)
                'stbQ.Append(" AND Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_Contatto_Terzi) & "' " + vbCrLf)

                'stbQ.Append("  AND  EXISTS (SELECT 1  " + vbCrLf)
                'stbQ.Append("  				FROM OGenerazioni_Anagrafe_Log " + vbCrLf)
                'stbQ.Append("               WHERE OGenerazioni_Anagrafe_Log.linea_cod = Linee_Produzioni.linea_cod " + vbCrLf)
                'stbQ.Append("               AND materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod " + vbCrLf)
                'stbQ.Append("               )" + vbCrLf)
            End If

            'FILTRO PER GESTIONE DELLA VINIFICAZIONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            stbQ.Append(RegistroVinificazione_FiltroSQL_Importante(objParametri, Lista_PrepCod, Lista_IdTrasf_NoComm, Piva))

            '21/08/14: adeguamento gestione al campo ora
            'stbQ.Append(" GROUP BY Materie_Prime.Piva, Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Movimenti_Dettagli.Lotto,  Movimenti.CAU_MOV, Movimenti_dettagli.Udm_Cod, Materie_Prime_Calibri.Cal_Cod, Materie_Prime_Calibri.Cal_Des, Movimenti.Data_Movimento " + vbCrLf)
            stbQ.Append(" GROUP BY Materie_Prime.Piva, Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Movimenti_Dettagli.Lotto,  Movimenti.CAU_MOV, Movimenti_dettagli.Udm_Cod, Materie_Prime_Calibri.Cal_Cod, Materie_Prime_Calibri.Cal_Des, Movimenti.Ora " & vbCrLf)

            Select Case Gestione_Conto_Terzi
                Case enum_RegistroContoTerzi.Nessuno,
                   enum_RegistroContoTerzi.RegistroGlobale,
                         enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    '0 = se non c'è esiste conto terzi
                    '1 = se esiste, ma è stato richiesto registro unico
                    '3 = questa select va bene anche nel reg c/terzi separato
                Case enum_RegistroContoTerzi.RegistroUnicoDiversificato
                    '2= registro unico ma diversificato per c/lav (MODELLO RUGGERI)
                    stbQ.Append(" , Cod_Contatto_Terzi " & vbCrLf)
            End Select

            '-------------------------------------------------------------------------------
            '-- ATTENZIONE! USARE L'UNION ALL, PERCHE' L'UNION FA IL DISTINCT! -------------
            '-------------------------------------------------------------------------------
            stbQ.Append(" ) " & vbCrLf)

            stbQ.Append(" UNION ALL " & vbCrLf)
            stbQ.Append(" ( " & vbCrLf)


            '=========================================================
            '--------------- 2) AGENDA ------------------
            '=========================================================

            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            stbQ.Append(" SELECT Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Movimenti_Dettagli.Lotto,  " & vbCrLf)

            '21/08/14: adeguamento gestione al campo ora
            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=2 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS CaricoKg, " + vbCrLf)
            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=2 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS ScaricoKg, " + vbCrLf)
            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS CaricoLt, " + vbCrLf)
            'stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS ScaricoLt, " + vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=2 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS CaricoKg, " & vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=2 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS ScaricoKg, " & vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS CaricoLt, " & vbCrLf)
            stbQ.Append(" CASE WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod=29 AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " THEN SUM(Movimenti_dettagli.Qta) ELSE 0 END AS ScaricoLt, " & vbCrLf)

            stbQ.Append(" Materie_Prime_Calibri.Cal_Cod, Materie_Prime_Calibri.Cal_Des, Movimenti_dettagli.Udm_Cod, " & vbCrLf)

            'MODIFICA DEL 19/10/2012:
            'ottimizzata: leggo cifra_start cifra_end e Lotto_Cod direttamente in una query
            'e CORRETTO BUG: non veniva cercato l'anno di vendemmia
            stbQ.Append("                ISNULL((SELECT TOP 1   " & vbCrLf)
            stbQ.Append("                ( CONVERT(varchar(250), Materie_PrimexLotto_Configurazione.Cifra_Start) + '|' + " & vbCrLf)
            stbQ.Append("                CONVERT(varchar(250), Materie_PrimexLotto_Configurazione.Cifra_End) + '|' + " & vbCrLf)
            stbQ.Append("                CONVERT(varchar(250), Materie_PrimexLotto_Configurazione.Lotto_Cod) ) AS ConfigLotto " & vbCrLf)
            stbQ.Append("                FROM Lotto_Configurazione " & vbCrLf)
            stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser   " & vbCrLf)
            stbQ.Append("                AND Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod   " & vbCrLf)
            stbQ.Append("                AND Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva   " & vbCrLf)
            stbQ.Append("                AND Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " & vbCrLf)
            stbQ.Append("                WHERE (Lotto_Des like '%Anno%produzione%' OR Lotto_Des like '%Anno%vendemmia%'  " & vbCrLf)
            stbQ.Append("                       OR Lotto_Des_Estesa like '%Anno%produzione%' OR Lotto_Des_Estesa like '%Anno%vendemmia%')  " & vbCrLf)
            stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " & vbCrLf)
            stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " & vbCrLf)
            stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),'0|0|0') AS ConfigLotto " & vbCrLf)


            '''MODIFICA GESTIONE CIFRA_START E CIFRA_END DEL 21/02/2012
            'stbQ.Append("                ISNULL((SELECT TOP 1 Materie_PrimexLotto_Configurazione.Cifra_Start  " + vbCrLf)
            'stbQ.Append("                FROM Lotto_Configurazione " + vbCrLf)
            'stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " + vbCrLf)
            'stbQ.Append("                WHERE Lotto_Des like '%Anno%produzione%'  " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),0) AS Cifra_Start, " + vbCrLf)

            'stbQ.Append("                ISNULL((SELECT TOP 1 Materie_PrimexLotto_Configurazione.Cifra_End " + vbCrLf)
            'stbQ.Append("                FROM Lotto_Configurazione " + vbCrLf)
            'stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " + vbCrLf)
            'stbQ.Append("                WHERE Lotto_Des like '%Anno%produzione%'  " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),0) AS Cifra_End, " + vbCrLf)

            'stbQ.Append("                ISNULL((SELECT TOP 1 Materie_PrimexLotto_Configurazione.Lotto_Cod " + vbCrLf)
            'stbQ.Append("                FROM Lotto_Configurazione " + vbCrLf)
            'stbQ.Append("                INNER JOIN Materie_PrimexLotto_Configurazione ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva AND   " + vbCrLf)
            'stbQ.Append("                Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod   " + vbCrLf)
            'stbQ.Append("                WHERE Lotto_Des like '%Anno%produzione%'  " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod " + vbCrLf)
            'stbQ.Append("                AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva),0) AS Lotto_Cod " + vbCrLf)

            Select Case Gestione_Conto_Terzi
                Case enum_RegistroContoTerzi.Nessuno,
                   enum_RegistroContoTerzi.RegistroGlobale,
                         enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    '0 = se non c'è esiste conto terzi
                    '1 = se esiste, ma è stato richiesto registro unico
                    '3 = questa select va bene anche nel reg c/terzi separato
                Case enum_RegistroContoTerzi.RegistroUnicoDiversificato
                    '2= registro unico ma diversificato per c/lav (MODELLO RUGGERI)
                    stbQ.Append("       , ISNULL( (" & vbCrLf)

                    'modifica del 23/03/2016: 
                    'ottimizzata + per i prodotti che non sono nè uva nè altre materie prime
                    'non gestisce diversamente le operazioni di carico da quelle di scarico
                    'ma fa la stessa cosa, partendo dal lotto del prodotto risale alla linea del c/lavoro

                    stbQ.Append("   ----------- caso UVE ----------- " & vbCrLf)
                    stbQ.Append("   CASE WHEN (select distinct TOP 1 codice_generazione " & vbCrLf)
                    stbQ.Append("               from OGenerazioni_Anagrafe_Log " & vbCrLf)
                    stbQ.Append("               where OGenerazioni_Anagrafe_Log.elem_cod=movimenti_dettagli.elem_cod  " & vbCrLf)
                    stbQ.Append("               and OGenerazioni_Anagrafe_Log.mat_cod= movimenti_dettagli.mat_cod " & vbCrLf)
                    stbQ.Append("               and tipo_generazione =4 " & vbCrLf)
                    stbQ.Append("               ) in (50,131,165) THEN " & vbCrLf)
                    stbQ.Append("               --se CARICO, SCARICO, RACCOLTA  " & vbCrLf)
                    stbQ.Append("               CASE  WHEN Agenda.lav_cod IN (125,1022,1023)   " & vbCrLf)
                    stbQ.Append("                   THEN" & vbCrLf)
                    stbQ.Append("                   ( " & vbCrLf)
                    stbQ.Append("                   agenda.piva   " & vbCrLf)
                    stbQ.Append("                   ) " & vbCrLf)
                    stbQ.Append("               ELSE  -- lav_cod " & vbCrLf)
                    'per tutte le altre operazioni (lav_cod), trattiamo diversamente in base al movimento di carico o scarico
                    stbQ.Append("               (   CASE  WHEN Movimenti.cau_mov = '" & CAU_CARICO & "' " & vbCrLf)
                    stbQ.Append("                       THEN   " & vbCrLf)
                    '                       nel caso di carico l'ingresso avviene con un documento contabile (ddt, doco, mvv
                    stbQ.Append("                       ( SELECT TOP 1 cod_contatto " & vbCrLf)
                    stbQ.Append("                           FROM Risorse_Umane  " & vbCrLf)
                    stbQ.Append("                           INNER JOIN Movimenti MovContabili ON MovContabili.cod_Risum = Risorse_Umane.Cod_Risum " & vbCrLf)
                    stbQ.Append("                           WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda " & vbCrLf)
                    stbQ.Append("                           AND Movimenti.piva = MovContabili.piva  " & vbCrLf)
                    stbQ.Append("                           AND cau_mov='" & CStr(CAU_REGISTRAZIONI) & "' " & vbCrLf)
                    stbQ.Append("                       ) --select mov_contabile " & vbCrLf)
                    stbQ.Append("                   ELSE " & vbCrLf)
                    stbQ.Append("                       ( SELECT TOP 1 cod_contatto " & vbCrLf)
                    stbQ.Append("                           FROM Contatti " & vbCrLf)
                    stbQ.Append("                           INNER JOIN Linee_Produzioni ON Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto " & vbCrLf)
                    stbQ.Append("                           AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
                    stbQ.Append("                           INNER JOIN  Trasformazioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva " & vbCrLf)
                    stbQ.Append("                           WHERE Linee_Produzioni.Cod_Contatto_Terzi <> '' " & vbCrLf)
                    stbQ.Append("                           AND Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " & vbCrLf)
                    stbQ.Append("                       ) -- SELECT SU LOTTO " & vbCrLf)
                    stbQ.Append("                   END -- case cau_mov " & vbCrLf)
                    stbQ.Append("               ) -- else del lav_cod" & vbCrLf)
                    stbQ.Append("               END -- case lav_cod " & vbCrLf)
                    stbQ.Append("  " & vbCrLf)
                    stbQ.Append("  " & vbCrLf)
                    stbQ.Append("   ----------- caso ALTRE MATERIE PRIME ----------- " & vbCrLf)
                    stbQ.Append("   WHEN EXISTS (select   1  " & vbCrLf)
                    stbQ.Append("               from OGenerazioni_Anagrafe_Log  " & vbCrLf)
                    stbQ.Append("               where OGenerazioni_Anagrafe_Log.elem_cod=movimenti_dettagli.elem_cod " & vbCrLf)
                    stbQ.Append("               and OGenerazioni_Anagrafe_Log.mat_cod= movimenti_dettagli.mat_cod " & vbCrLf)
                    stbQ.Append("               and tipo_generazione =9 " & vbCrLf)
                    stbQ.Append("               )  THEN	 " & vbCrLf)
                    stbQ.Append("                   ( " & vbCrLf)
                    stbQ.Append("                   SELECT TOP 1 Contatti.cod_contatto " & vbCrLf)
                    stbQ.Append("                   FROM Materie_PrimexLotto_Proprieta, Contatti " & vbCrLf)
                    stbQ.Append("                   where piva_superuser = '" & objParametri.PivaSuperUser & "' " & vbCrLf)
                    stbQ.Append("                   and Materie_PrimexLotto_Proprieta.piva = Agenda.PIVA " & vbCrLf)
                    stbQ.Append("                   and id_proprieta = " & CStr(enum_TipoProprieta_Lotto.Categoria_Omni) & "  " & vbCrLf)
                    stbQ.Append("                   and lotto_val1 = movimenti_dettagli.lotto " & vbCrLf)
                    stbQ.Append("                   and elem_cod = movimenti_dettagli.elem_cod  " & vbCrLf)
                    stbQ.Append("                   and mat_cod = movimenti_dettagli.mat_cod " & vbCrLf)
                    '                                   nno serve al momento filtrare lotto_cod1 (si imposta solo nome/lotto)
                    'stbQ.Append("              and lotto_cod1 = 16 " & vbCrLf)
                    stbQ.Append("                   and proprieta_val LIKE '%' + Contatti.cod_contatto + '%' " & vbCrLf)
                    stbQ.Append("                   ) " & vbCrLf)
                    stbQ.Append("    " & vbCrLf)
                    stbQ.Append(" ELSE   " & vbCrLf)
                    stbQ.Append("  ------- TUTTO IL RESTO ---------  " & vbCrLf)
                    stbQ.Append("           ( SELECT TOP 1 cod_contatto " & vbCrLf)
                    stbQ.Append("               FROM Contatti " & vbCrLf)
                    stbQ.Append("               INNER JOIN Linee_Produzioni ON Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto " & vbCrLf)
                    stbQ.Append("               AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
                    stbQ.Append("               INNER JOIN  Trasformazioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva " & vbCrLf)
                    stbQ.Append("               WHERE Linee_Produzioni.Cod_Contatto_Terzi <> '' " & vbCrLf)
                    stbQ.Append("               AND Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " & vbCrLf)
                    stbQ.Append("           ) -- SELECT SU LOTTO " & vbCrLf)
                    stbQ.Append(" END -- case codice generazione  " & vbCrLf)


                    'COMMENTATO IL 23/03/2016: leggi sopra il commento sul pezzo nuovo della query
                    'stbQ.Append("   ----------- caso UVE ----------- " + vbCrLf)
                    'stbQ.Append("   CASE WHEN (select distinct TOP 1 codice_generazione " + vbCrLf)
                    'stbQ.Append("               from OGenerazioni_Anagrafe_Log " + vbCrLf)
                    'stbQ.Append("               where OGenerazioni_Anagrafe_Log.elem_cod=movimenti_dettagli.elem_cod  " + vbCrLf)
                    'stbQ.Append("               and OGenerazioni_Anagrafe_Log.mat_cod= movimenti_dettagli.mat_cod " + vbCrLf)
                    'stbQ.Append("               and tipo_generazione =4 " + vbCrLf)
                    'stbQ.Append("               ) in (50,131,165) THEN " + vbCrLf)
                    'stbQ.Append("               --se CARICO, SCARICO, RACCOLTA  " + vbCrLf)
                    'stbQ.Append("               CASE  WHEN Agenda.lav_cod IN (125,1022,1023)   " + vbCrLf)
                    'stbQ.Append("                   THEN" + vbCrLf)
                    'stbQ.Append("                   ( " + vbCrLf)
                    'stbQ.Append("                   agenda.piva   " + vbCrLf)
                    'stbQ.Append("                   ) " + vbCrLf)
                    'stbQ.Append("               ELSE  -- lav_cod " + vbCrLf)
                    ''per tutte le altre operazioni (lav_cod), trattiamo diversamente in base al movimento di carico o scarico
                    'stbQ.Append("               (   CASE  WHEN Movimenti.cau_mov = '" & CAU_CARICO & "' " + vbCrLf)
                    'stbQ.Append("                       THEN   " + vbCrLf)
                    ''                       nel caso di carico l'ingresso avviene con un documento contabile (ddt, doco, mvv
                    'stbQ.Append("                       ( SELECT TOP 1 cod_contatto " + vbCrLf)
                    'stbQ.Append("                           FROM Risorse_Umane  " + vbCrLf)
                    'stbQ.Append("                           INNER JOIN Movimenti MovContabili ON MovContabili.cod_Risum = Risorse_Umane.Cod_Risum " + vbCrLf)
                    'stbQ.Append("                           WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda " + vbCrLf)
                    'stbQ.Append("                           AND Movimenti.piva = MovContabili.piva  " + vbCrLf)
                    'stbQ.Append("                           AND cau_mov='" & CStr(CAU_REGISTRAZIONI) & "' " + vbCrLf)
                    'stbQ.Append("                       ) --select mov_contabile " + vbCrLf)
                    'stbQ.Append("                   ELSE " + vbCrLf)
                    'stbQ.Append("                       ( SELECT TOP 1 cod_contatto " + vbCrLf)
                    'stbQ.Append("                           FROM Contatti " + vbCrLf)
                    'stbQ.Append("                           INNER JOIN Linee_Produzioni ON Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto " + vbCrLf)
                    'stbQ.Append("                           AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " + vbCrLf)
                    'stbQ.Append("                           INNER JOIN  Trasformazioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva " + vbCrLf)
                    'stbQ.Append("                           WHERE Linee_Produzioni.Cod_Contatto_Terzi <> '' " + vbCrLf)
                    'stbQ.Append("                           AND Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " + vbCrLf)
                    'stbQ.Append("                       ) -- SELECT SU LOTTO " + vbCrLf)
                    'stbQ.Append("                   END -- case cau_mov " + vbCrLf)
                    'stbQ.Append("               ) -- else del lav_cod" + vbCrLf)
                    'stbQ.Append("               END -- case lav_cod " + vbCrLf)
                    'stbQ.Append("  " + vbCrLf)
                    'stbQ.Append("  " + vbCrLf)
                    'stbQ.Append("   ----------- caso ALTRE MATERIE PRIME ----------- " + vbCrLf)
                    'stbQ.Append("   WHEN EXISTS (select   1  " + vbCrLf)
                    'stbQ.Append("               from OGenerazioni_Anagrafe_Log  " + vbCrLf)
                    'stbQ.Append("               where OGenerazioni_Anagrafe_Log.elem_cod=movimenti_dettagli.elem_cod " + vbCrLf)
                    'stbQ.Append("               and OGenerazioni_Anagrafe_Log.mat_cod= movimenti_dettagli.mat_cod " + vbCrLf)
                    'stbQ.Append("               and tipo_generazione =9 " + vbCrLf)
                    'stbQ.Append("               )  THEN	 " + vbCrLf)
                    'stbQ.Append("                   ( " + vbCrLf)
                    'stbQ.Append("                   SELECT TOP 1 Contatti.cod_contatto " + vbCrLf)
                    'stbQ.Append("                   FROM Materie_PrimexLotto_Proprieta, Contatti " & vbCrLf)
                    'stbQ.Append("                   where piva_superuser = '" & objParametri.PivaSuperUser & "' " & vbCrLf)
                    'stbQ.Append("                   and Materie_PrimexLotto_Proprieta.piva = Agenda.PIVA " & vbCrLf)
                    'stbQ.Append("                   and id_proprieta = " & CStr(enum_TipoProprieta_Lotto.Categoria_Omni) & "  " & vbCrLf)
                    'stbQ.Append("                   and lotto_val1 = movimenti_dettagli.lotto " & vbCrLf)
                    'stbQ.Append("                   and elem_cod = movimenti_dettagli.elem_cod  " & vbCrLf)
                    'stbQ.Append("                   and mat_cod = movimenti_dettagli.mat_cod " & vbCrLf)
                    ''                                   nno serve al momento filtrare lotto_cod1 (si imposta solo nome/lotto)
                    ''stbQ.Append("              and lotto_cod1 = 16 " & vbCrLf)
                    'stbQ.Append("                   and proprieta_val LIKE '%' + Contatti.cod_contatto + '%' " & vbCrLf)
                    'stbQ.Append("                   ) " + vbCrLf)
                    'stbQ.Append("    " + vbCrLf)
                    'stbQ.Append(" ELSE   " + vbCrLf)
                    'stbQ.Append("  ------- TUTTO IL RESTO ---------  " + vbCrLf)

                    ''modifica del 26/02/2016:
                    ''per il passaggio a commercializzazione non han mai usato la vera operazione
                    ''ma uno scarico di vino dalla vasca con descrizione libera
                    ''gli scarichi di magazzino erano battezzati solo per l'azienda superuser
                    ''bisogna quindi modifiacre e fargli fare la stessa cosa che viene fatta per il cau_scarico

                    '''nel caso dei carichi e scarichi "secchi" di magazzino e della raccolta, non c'è il movimento contabile e quindi si prende come riferimento la piva dell'operazione (che sarà quella dell'azienda superuser)
                    ''stbQ.Append("       CASE  WHEN Agenda.lav_cod IN (" & LAVCOD_RACCOLTA & "," & LAVCOD_CARICO & "," & LAVCOD_SCARICO & ") " + vbCrLf)
                    ''stbQ.Append("       THEN " + vbCrLf)
                    ''stbQ.Append("       ( " + vbCrLf)
                    ''stbQ.Append("       agenda.piva  " + vbCrLf)
                    ''stbQ.Append("       ) " + vbCrLf)
                    ''stbQ.Append("       ELSE  -- lav_cod " + vbCrLf)
                    ''stbQ.Append("  " + vbCrLf)
                    ''per tutte le altre operazioni (lav_cod), trattiamo diversamente in base al movimento di carico o scarico
                    'stbQ.Append("       (   CASE  WHEN Movimenti.cau_mov = '" & CAU_CARICO & "' " + vbCrLf)
                    'stbQ.Append("           THEN   " + vbCrLf)
                    ''                       nel caso di carico l'ingresso avviene con un documento contabile (ddt, doco, mvv
                    'stbQ.Append("           ( SELECT TOP 1 cod_contatto " + vbCrLf)
                    'stbQ.Append("               FROM Risorse_Umane  " + vbCrLf)
                    'stbQ.Append("               INNER JOIN Movimenti MovContabili ON MovContabili.cod_Risum = Risorse_Umane.Cod_Risum " + vbCrLf)
                    'stbQ.Append("               WHERE Movimenti.Id_Agenda = MovContabili.Id_Agenda " + vbCrLf)
                    'stbQ.Append("               AND Movimenti.piva = MovContabili.piva  " + vbCrLf)
                    'stbQ.Append("               AND cau_mov='" & CStr(CAU_REGISTRAZIONI) & "' " + vbCrLf)
                    'stbQ.Append("           ) --select mov_contabile " + vbCrLf)
                    'stbQ.Append("           ELSE " + vbCrLf)
                    'stbQ.Append("           ( SELECT TOP 1 cod_contatto " + vbCrLf)
                    'stbQ.Append("               FROM Contatti " + vbCrLf)
                    'stbQ.Append("               INNER JOIN Linee_Produzioni ON Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto " + vbCrLf)
                    'stbQ.Append("               AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " + vbCrLf)
                    'stbQ.Append("               INNER JOIN  Trasformazioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva " + vbCrLf)
                    'stbQ.Append("               WHERE Linee_Produzioni.Cod_Contatto_Terzi <> '' " + vbCrLf)
                    'stbQ.Append("               AND Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " + vbCrLf)
                    'stbQ.Append("           ) -- SELECT SU LOTTO " + vbCrLf)
                    'stbQ.Append("           END -- case cau_mov " + vbCrLf)
                    'stbQ.Append("       ) " + vbCrLf)
                    ''stbQ.Append("    END -- case lav_cod " + vbCrLf)

                    'stbQ.Append(" END -- case codice generazione  " + vbCrLf)

                    ''stbQ.Append("       SELECT Cod_Contatto_Terzi " + vbCrLf)
                    ''stbQ.Append("       FROM Linee_Produzioni  " + vbCrLf)
                    ''stbQ.Append("       INNER JOIN  Trasformazioni ON trasformazioni.linea_cod = Linee_Produzioni.linea_cod AND trasformazioni.Piva = Linee_Produzioni.Piva " + vbCrLf)
                    ''stbQ.Append("       WHERE Linee_Produzioni.Cod_Contatto_Terzi <> '' " + vbCrLf)
                    ''stbQ.Append("       AND Movimenti_dettagli.lotto = trasformazioni.Trasformazione_Des  " + vbCrLf)
                    ''stbQ.Append("       AND Movimenti_dettagli.Piva = trasformazioni.Piva  " + vbCrLf)

                    stbQ.Append("       ), '' ) AS Cod_Contatto_Terzi " & vbCrLf)
            End Select

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            stbQ.Append(" FROM Materie_PrimexReport  INNER JOIN Materie_Prime ON Materie_PrimexReport.Mat_Cod = Materie_Prime.Mat_Cod AND Materie_PrimexReport.Piva = Materie_Prime.Piva  " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod   " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD   " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov   " & vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND Movimenti.Id_Agenda = Agenda.Id_Agenda   " & vbCrLf)

            'inserito il 10/04/2013 x filtro id_dest
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            '15/04/2013: inutile il left, ci sono due where sulla tabella
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " + vbCrLf)
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " + vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " & vbCrLf)

            'stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva = Agenda.Piva" + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & "  " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.elem_cod = Movimenti_dettagli.elem_cod " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.mat_cod = Movimenti_dettagli.mat_cod " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.id =(             " + vbCrLf)
            'stbQ.Append("                                   SELECT MAX(id) " + vbCrLf)
            'stbQ.Append("                                    FROM OGenerazioni_Anagrafe_Log OLOG " + vbCrLf)
            'stbQ.Append("                                    WHERE OLOG.piva_superuser = OGenerazioni_Anagrafe_Log.Piva_SuperUser " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.piva = OLOG.Piva " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = OLOG.Modulo_Generazione" + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.elem_cod = OLOG.elem_cod  " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.mat_cod = OLOG.mat_cod " + vbCrLf)
            'stbQ.Append("                                   ) " + vbCrLf)

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            stbQ.Append(" WHERE Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                                      "'" & CAU_SCARICO & "', " &
                                                      "'" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', " &
                                                      "'" & CAU_CONFERIMENTO & "', " &
                                                      "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                                        ") " & vbCrLf)

            stbQ.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If str_idagenda_filtrocategoria <> "" Then
                stbQ.Append(" AND Agenda.Id_Agenda IN " & Agro_SQL_Save_Clausola_IN(str_idagenda_filtrocategoria) & " " & vbCrLf)
            End If
            If str_matcod_filtrocategoria <> "" Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod IN " & Agro_SQL_Save_Clausola_IN(str_matcod_filtrocategoria) & " " & vbCrLf)
            End If
            If str_lotto_filtrocategoria <> "" Then
                stbQ.Append(" AND Movimenti_Dettagli.Lotto IN " & Agro_SQL_Save_Clausola_IN(str_lotto_filtrocategoria, True) & " " & vbCrLf)
            End If

            'stbQ.Append(" AND EXISTS (SELECT 1 FROM Agenda Agenda_Lav INNER JOIN OperazionixReport OPR ON Agenda_Lav.Lav_Cod = OPR.Lav_Cod " + vbCrLf)
            'stbQ.Append("             AND OPR.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " + vbCrLf)
            'stbQ.Append("             WHERE  Agenda.Piva = Agenda_Lav.Piva AND Agenda.Id_Agenda = Agenda_Lav.Id_Agenda ) " + vbCrLf)
            'CORREZIONE DEL 07/05/2014: la piva di OperazionixReport è quella del superuser,
            'non va quindi messa in join con la piva di agenda!
            'è stato scoperto ora perchè qualitoscana è il primo cliente col quale si stampano i registri di cantina sulle aziende figlie in gerarchia
            stbQ.Append(" AND EXISTS (SELECT 1 FROM OperazionixReport OPR  " & vbCrLf)
            stbQ.Append("             WHERE  OPR.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append("             AND OPR.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)
            stbQ.Append("               AND Agenda.Lav_Cod = OPR.Lav_Cod ) " & vbCrLf)


            stbQ.Append(" AND Materie_PrimexReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)

            'nel riepilogo non ci vanno solo i prodotti movimentati nel mese
            'stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " ")
            'stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " ")

            '--------------
            '21/08/2014: adeguato il filtro con la select (viaggia tutto sul campo Ora)
            'stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataRiepilogoInizio) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " " + vbCrLf)
            stbQ.Append(" AND Movimenti.Ora >= " & Agro_SQL_SaveDate(DataRiepilogoInizio) & " " & vbCrLf)
            stbQ.Append(" AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " " & vbCrLf)

            stbQ.Append(" AND   Materie_Prime_Calibri.Piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")
            stbQ.Append(" AND   Materie_Prime_ParametriQualitativi.Tipo = 'calibro' " & vbCrLf)
            'modifica del 21/09/2010: sostituito ChkRegistri con ChkRegistri_Vinificazione
            stbQ.Append(" AND   Materie_Prime_ParametriQualitativi.ChkRegistri_Vinificazione = 1 " & vbCrLf)

            '--------------
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)

            'aggiunta il 19/08/13:per escludere carichi/scarichi di operazioni pendenti (pianificate)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Id_Destinazione <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " & vbCrLf)
            End If
            If Cal_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime_ParametriQualitativi.tipo_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If

            If Linea_Cod <> 0 Then
                stbQ.Append("   AND  EXISTS (SELECT 1  " & vbCrLf)
                stbQ.Append("               FROM OGenerazioni_Anagrafe_Log " & vbCrLf)
                stbQ.Append(" 				WHERE materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod " & vbCrLf)
                stbQ.Append(" 				AND materie_prime.elem_cod = OGenerazioni_Anagrafe_Log.elem_cod " & vbCrLf)
                stbQ.Append("               AND OGenerazioni_Anagrafe_Log.linea_cod = " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
                stbQ.Append("               ) " & vbCrLf)
            End If
            If Cau_Mov <> "" Then
                stbQ.Append(" AND Movimenti.CAU_MOV = '" & Agro_SQL_SaveText(Cau_Mov) & "'" & vbCrLf)
            End If

            If Cod_Contatto_Terzi <> "" Then
                'modifica del 21/01/2013: separato il filtro per la parte agenda e la parte linee
                stbQ.Append(RegistroVinificazione_FiltroSQL_ContoTerzi_ParteAgenda(objParametri.PivaSuperUser,
                                                                                    Piva,
                                                                                    Cod_Contatto_Terzi,
                                                                                    Lista_CodRisUm))
                ''aggiunta in data 25/09/2012:
                ''stampa del registro separata per fornitore uve (Cod_Contatto_Terzi)
                'stbQ.Append("   AND  EXISTS (SELECT 1  " + vbCrLf)
                'stbQ.Append("               FROM OGenerazioni_Anagrafe_Log " + vbCrLf)
                'stbQ.Append(" 				INNER JOIN  Linee_Produzioni ON OGenerazioni_Anagrafe_Log.linea_cod =Linee_Produzioni.linea_cod  " + vbCrLf)
                'stbQ.Append(" 				WHERE materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod " + vbCrLf)
                'stbQ.Append("               AND Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_Contatto_Terzi) & "' " + vbCrLf)
                'stbQ.Append("               ) " + vbCrLf)
            End If

            'FILTRO PER GESTIONE DELLA VINIFICAZIONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            stbQ.Append(RegistroVinificazione_FiltroSQL_Importante(objParametri, Lista_PrepCod, Lista_IdTrasf_NoComm, Piva))

            '21/08/14: adeguamento gestione al campo ora
            'stbQ.Append(" GROUP BY Materie_Prime.Piva, Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Movimenti_Dettagli.Lotto,  Movimenti.CAU_MOV, Movimenti_dettagli.Udm_Cod, Materie_Prime_Calibri.Cal_Cod, Materie_Prime_Calibri.Cal_Des, Movimenti.Data_Movimento " + vbCrLf)
            stbQ.Append(" GROUP BY Materie_Prime.Piva, Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Movimenti_Dettagli.Lotto,  Movimenti.CAU_MOV, Movimenti_dettagli.Udm_Cod, Materie_Prime_Calibri.Cal_Cod, Materie_Prime_Calibri.Cal_Des, Movimenti.Ora " & vbCrLf)
            'aggiunto il 13/08/2015
            stbQ.Append(" , movimenti_dettagli.piva, agenda.lav_cod,movimenti.id_agenda, movimenti.piva,agenda.piva, movimenti_dettagli.elem_cod ,movimenti_dettagli.mat_cod " & vbCrLf)

            Select Case Gestione_Conto_Terzi
                Case enum_RegistroContoTerzi.Nessuno,
                   enum_RegistroContoTerzi.RegistroGlobale,
                         enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    '0 = se non c'è esiste conto terzi
                    '1 = se esiste, ma è stato richiesto registro unico
                    '3 = questa select va bene anche nel reg c/terzi separato
                Case enum_RegistroContoTerzi.RegistroUnicoDiversificato
                    '2= registro unico ma diversificato per c/lav (MODELLO RUGGERI)
                    stbQ.Append(" , movimenti_dettagli.piva " & vbCrLf)
            End Select

            stbQ.Append(" ) " & vbCrLf)

            'stbQ.Append(" ORDER BY Cal_Des ")           
            Select Case Gestione_Conto_Terzi
                Case enum_RegistroContoTerzi.Nessuno,
                   enum_RegistroContoTerzi.RegistroGlobale,
                         enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    '0 = se non c'è esiste conto terzi
                    '1 = se esiste, ma è stato richiesto registro unico
                    '3 = questa select va bene anche nel reg c/terzi separato
                    stbQ.Append(" ORDER BY Materie_Prime.elem_cod DESC, Cal_Des, Movimenti_Dettagli.Lotto " & vbCrLf)
                Case enum_RegistroContoTerzi.RegistroUnicoDiversificato
                    '2= registro unico ma diversificato per c/lav (MODELLO RUGGERI)
                    stbQ.Append(" ORDER BY Materie_Prime.elem_cod DESC, Cal_Des, Cod_Contatto_Terzi, Movimenti_Dettagli.Lotto " & vbCrLf)

            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Registro di vinificazione: saldo
    ''' DataInizioIntervallo -> 01/01/1900
    ''' Data -> data fine
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RegistroVinificazioneSaldoxRiepilogo(ByVal Piva As String,
                                                        ByVal Cal_Cod As String,
                                                        ByVal AnnoProduzione As String,
                                                        ByVal CifraStart As Integer,
                                                        ByVal CifraEnd As Integer,
                                                        ByVal UdmCod As Integer,
                                                        ByVal TipoReport As Integer,
                                                        ByVal DataInizioIntervallo As String,
                                                        ByVal Data As String,
                                                        ByVal ComprendiData As Boolean,
                                                        ByVal FiltroLotti As String,
                                                        ByVal Cod_Contatto_Terzi As String,
                                                        ByVal Lista_CodRisUm As String,
                                                        ByVal Lista_PrepCod As String,
                                                        ByVal Lista_IdTrasf_NoComm As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal Id_Destinazione As Integer,
                                                        ByVal Mat_Cod As Integer,
                                                        ByVal Linea_Cod As Integer,
                                                        ByVal str_idagenda_filtrocategoria As String,
                                                        ByVal str_matcod_filtrocategoria As String,
                                                        ByVal str_lotto_filtrocategoria As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Decimal

        'Return 0
        'Exit Function
        'ByVal Opt_Gestione_RegistroVinificazione As Integer, _

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.RegistriCantina.RegistroVinificazioneSaldoxRiepilogo"

        Dim MessaggioErrore As String = ""
        Dim stbQ As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer
        Dim dRet As Decimal = 0

        Try

            '=========================================================
            '--------------- 1) PREPARAZIONI ------------------
            '=========================================================

            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            stbQ.Append(" SELECT CASE " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod= " & Agro_SQL_SaveNum(UdmCod) & " THEN SUM(Movimenti_dettagli.Qta) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod= " & Agro_SQL_SaveNum(UdmCod) & "  THEN SUM(-Movimenti_dettagli.Qta) " & vbCrLf)
            stbQ.Append(" ELSE 0 END AS Saldo " & vbCrLf)


            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            stbQ.Append(" FROM Linee_PreparazionixReport " & vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Preparazioni ON Linee_PreparazionixReport.Piva = Linee_Preparazioni.Piva AND Linee_PreparazionixReport.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod  " & vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Linee_Preparazioni.Preparazione_Cod = Agenda.PREPARAZIONE_COD AND Linee_Preparazioni.Piva = Agenda.PIVA " & vbCrLf)

            'Modifica del 27/04/2012 by Maga&Marco:
            'nella vinificazione c'è sempre la linea_Produzione, quindi mettiamo inner join 
            'e aggiungiamo join della piva (altrimenti vengono sdoppiati i record a causa della piva AAAAAAAAAAA)
            'stbQ.Append(" LEFT JOIN Linee_Produzioni ON Agenda.Linea_Cod = Linee_Produzioni.Linea_Cod " + vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Produzioni ON Agenda.Piva = Linee_Produzioni.Piva AND Agenda.Linea_Cod = Linee_Produzioni.Linea_Cod " & vbCrLf)

            'nel riepilogo deve venir su anche l'uva vendemmiata
            'stbQ.Append(" INNER JOIN Trasformazioni ON Trasformazioni.ID_trasformazione=Agenda.ID_Trasformazione AND  Trasformazioni.PIVA=Agenda.PIVA ")
            'stbQ.Append(" INNER JOIN Trasformazioni ON Linee_Produzioni.Linea_Cod = Trasformazioni.Linea_Cod ")

            stbQ.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod  " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov  " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod AND Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_dettagli.Udm_Cod " & vbCrLf)

            'inserito il 10/04/2013 x filtro id_dest
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            '15/04/2013: inutile il left, ci sono due where sulla tabella
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " + vbCrLf)
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " + vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " & vbCrLf)

            'stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva = Agenda.Piva" + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & "  " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.elem_cod = Movimenti_dettagli.elem_cod " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.mat_cod = Movimenti_dettagli.mat_cod " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.id =(             " + vbCrLf)
            'stbQ.Append("                                   SELECT MAX(id) " + vbCrLf)
            'stbQ.Append("                                    FROM OGenerazioni_Anagrafe_Log OLOG " + vbCrLf)
            'stbQ.Append("                                    WHERE OLOG.piva_superuser = OGenerazioni_Anagrafe_Log.Piva_SuperUser " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.piva = OLOG.Piva " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = OLOG.Modulo_Generazione" + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.elem_cod = OLOG.elem_cod  " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.mat_cod = OLOG.mat_cod " + vbCrLf)
            'stbQ.Append("                                   ) " + vbCrLf)

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            stbQ.Append(" WHERE Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(TipoReport) & " " & vbCrLf)
            stbQ.Append(" AND Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                                    "'" & CAU_SCARICO & "', " &
                                                    "'" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', " &
                                                    "'" & CAU_CONFERIMENTO & "', " &
                                                    "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                                    ") " & vbCrLf)

            stbQ.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If str_idagenda_filtrocategoria <> "" Then
                stbQ.Append(" AND Agenda.Id_Agenda IN " & Agro_SQL_Save_Clausola_IN(str_idagenda_filtrocategoria) & " " & vbCrLf)
            End If
            If str_matcod_filtrocategoria <> "" Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod IN " & Agro_SQL_Save_Clausola_IN(str_matcod_filtrocategoria) & " " & vbCrLf)
            End If
            If str_lotto_filtrocategoria <> "" Then
                stbQ.Append(" AND Movimenti_Dettagli.Lotto IN " & Agro_SQL_Save_Clausola_IN(str_lotto_filtrocategoria, True) & " " & vbCrLf)
            End If

            '--------------
            '21/08/2014: adeguato il filtro con la select (viaggia tutto sul campo Ora)
            'stbQ.Append(" AND Movimenti.Data_Movimento <" & IIf(ComprendiData, "= ", " ") & Agro_SQL_SaveDate(Data) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Data_Movimento >=" & Agro_SQL_SaveDate(DataInizioIntervallo) & " " + vbCrLf)  ' altrimenti prenderebbe tutto lo storico
            stbQ.Append(" AND CONVERT(date, Movimenti.Ora) < " & IIf(ComprendiData, "= ", " ") & Agro_SQL_SaveDate(Data) & " " & vbCrLf)
            stbQ.Append(" AND CONVERT(date, Movimenti.Ora) >= " & Agro_SQL_SaveDate(DataInizioIntervallo) & " " & vbCrLf)


            'stbQ.Append(" AND NOT EXISTS (SELECT * FROM  Linee_Preparazioni_Report_Esclusi LPRE " + vbCrLf)
            'stbQ.Append("                 INNER JOIN Linee_Preparazioni_Dettagli LPD ON LPRE.Piva = LPD.Piva AND LPRE.Preparazione_Cod = LPD.Preparazione_Cod AND LPRE.Dettaglio_Cod = LPD.Dettaglio_Cod " + vbCrLf)
            ''stbQ.Append("                 WHERE Materie_Prime.Elem_Cod = LPD.Elem_Cod AND Materie_Prime.Mat_Cod = LPD.Mat_Cod")
            'stbQ.Append("                 WHERE LPD.CAU_MOV = Movimenti.CAU_MOV " + vbCrLf)
            'stbQ.Append("                 AND LPRE.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " + vbCrLf)
            'stbQ.Append("                 AND LPRE.Id_Report = Linee_PreparazionixReport.Id_Report ) " + vbCrLf)

            'correzione del 7/12/2010: mancavano i join con Movimenti_dettagli, per cui
            'se c'era almeno un dettaglio escluso per uno scarico, venivano esclusi tutti gli scarichi
            'questa query era rimasta indietro, nel report era stato sistemato in data 9/11/2010
            stbQ.Append(" AND NOT EXISTS (SELECT 1 FROM  Linee_Preparazioni_Report_Esclusi LPRE " & vbCrLf)
            stbQ.Append("                 INNER JOIN Linee_Preparazioni_Dettagli LPD ON LPRE.Piva = LPD.Piva AND LPRE.Preparazione_Cod = LPD.Preparazione_Cod AND LPRE.Dettaglio_Cod = LPD.Dettaglio_Cod " & vbCrLf)
            stbQ.Append("                 WHERE LPRE.Piva = Linee_Preparazioni.Piva " & vbCrLf)
            stbQ.Append("                 AND LPRE.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " & vbCrLf)
            stbQ.Append("                 AND LPRE.Id_Report = Linee_PreparazionixReport.Id_Report  " & vbCrLf)
            stbQ.Append("                 AND LPD.CAU_MOV = Movimenti.CAU_MOV " & vbCrLf)
            stbQ.Append("                 AND LPD.Elem_Cod = Movimenti_dettagli.Elem_Cod " & vbCrLf)
            'modifica del 05/10/2012: se la preparazione passaggio da registro di vinificazione a registro di commercializzazione
            'utilizzava 'risorse indefinita' come ingrediente e/o preparato
            'il join con Movimenti_dettagli non produceva alcun record e quindi venivano visualizzati entrambi i movimenti:
            'sia lo scarico dal reg vinificazione sia il carico al reg di commerc. e la qta si azzerava
            'INTRODOTTO L'OR per LPD.Pro_Cod - LPD.Mat_Cod - LPD.Udm_Cod 
            stbQ.Append("               AND ( " & vbCrLf)
            stbQ.Append("                       ( " & vbCrLf)
            stbQ.Append("                       LPD.Pro_Cod = Movimenti_dettagli.Pro_Cod " & vbCrLf)
            stbQ.Append("                       AND LPD.Mat_Cod = Movimenti_dettagli.Mat_Cod " & vbCrLf)
            stbQ.Append("                       AND LPD.Udm_Cod = Movimenti_dettagli.Udm_Cod   " & vbCrLf)
            stbQ.Append("                       ) OR ( " & vbCrLf)
            stbQ.Append("                       LPD.Pro_Cod = 0 AND LPD.Mat_Cod = 0 AND LPD.Udm_Cod = 0  " & vbCrLf)
            stbQ.Append("                       ) " & vbCrLf)
            stbQ.Append("                   ) " & vbCrLf)
            stbQ.Append("               ) " & vbCrLf)

            '--------------
            'aggiunto in data 23/09/2013
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = 'calibro' " & vbCrLf)
            'corretto bug in data 08/11/2012: leggeva ChkRegistri invece di ChkRegistri_Vinificazione
            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri_Vinificazione = 1 " & vbCrLf)

            stbQ.Append(" AND Materie_Prime_Calibri.Cal_Cod=" & Agro_SQL_SaveNum(Cal_Cod) & " " & vbCrLf)
            stbQ.Append(" AND Movimenti_dettagli.Udm_Cod=" & Agro_SQL_SaveNum(UdmCod) & " " & vbCrLf)

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Id_Destinazione <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If
            If Linea_Cod <> 0 Then
                stbQ.Append(" AND Linee_Produzioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "  " & vbCrLf)
            End If

            If Cod_Contatto_Terzi <> "" Then
                'modifica del 21/01/2013: separato il filtro per la parte agenda e la parte linee
                stbQ.Append(RegistroVinificazione_FiltroSQL_ContoTerzi_ParteLinee(Piva, Cod_Contatto_Terzi))

                ''aggiunta in data 27/04/2012: gestione conto terzi
                ''modificata in data 25/09/2012:
                ''stampa del registro separata per fornitore uve (Cod_Contatto_Terzi)
                'stbQ.Append(" AND Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_Contatto_Terzi) & "' " + vbCrLf)

                'stbQ.Append("  AND  EXISTS (SELECT 1  " + vbCrLf)
                'stbQ.Append("  				FROM OGenerazioni_Anagrafe_Log " + vbCrLf)
                'stbQ.Append("               WHERE OGenerazioni_Anagrafe_Log.linea_cod = Linee_Produzioni.linea_cod " + vbCrLf)
                'stbQ.Append("               AND materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod " + vbCrLf)
                'stbQ.Append("               )" + vbCrLf)
            End If


            'FILTRO PER GESTIONE DELLA VINIFICAZIONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            stbQ.Append(RegistroVinificazione_FiltroSQL_Importante(objParametri, Lista_PrepCod, Lista_IdTrasf_NoComm, Piva))

            'If CifraStart <> 0 And CifraEnd <> 0 Then
            '    stbQ.Append(" AND SUBSTRING(Movimenti_Dettagli.Lotto, " & CifraStart & "," & CifraEnd - CifraStart + 1 & ")='" & Agro_SQL_SaveNum(AnnoProduzione) & "' " + vbCrLf)
            'End If

            If CifraStart <> 0 AndAlso CifraEnd <> 0 Then
                stbQ.Append(" AND SUBSTRING(Movimenti_Dettagli.Lotto, " & CifraStart & "," & CifraEnd - CifraStart + 1 & ")='" & Agro_SQL_SaveNum(AnnoProduzione) & "' " & vbCrLf)
            Else
                stbQ.Append(FiltroLotti)
            End If


            '------------------------------------------------------ 
            '------------------- GROUP BY -------------------------
            '------------------------------------------------------
            stbQ.Append(" GROUP BY Movimenti.Cau_Mov, Movimenti_dettagli.Udm_Cod " & vbCrLf)


            '##################### UNION ##############################
            stbQ.Append(" UNION ALL " & vbCrLf)
            '##################### UNION ##############################

            '=========================================================
            '--------------- 2) AGENDA ------------------
            '=========================================================

            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            stbQ.Append(" SELECT CASE  " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_CARICO & "','" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', '" & CAU_CONFERIMENTO & "') AND Movimenti_dettagli.Udm_Cod= " & Agro_SQL_SaveNum(UdmCod) & "  THEN SUM(Movimenti_dettagli.Qta) " & vbCrLf)
            stbQ.Append(" WHEN Movimenti.CAU_MOV IN ('" & CAU_SCARICO & "','" & CAU_CONFERIMENTO_DIVERSI & "') AND Movimenti_dettagli.Udm_Cod= " & Agro_SQL_SaveNum(UdmCod) & "  THEN SUM(-Movimenti_dettagli.Qta) " & vbCrLf)
            stbQ.Append(" ELSE 0 END AS Saldo " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            stbQ.Append(" FROM Materie_PrimexReport " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Materie_PrimexReport.Mat_Cod = Materie_Prime.Mat_Cod AND Materie_PrimexReport.Piva = Materie_Prime.Piva " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov  " & vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND Movimenti.Id_Agenda = Agenda.Id_Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD " & vbCrLf)

            'inserito il 10/04/2013 x filtro id_dest
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " & vbCrLf)

            '15/04/2013: inutile il left, ci sono due where sulla tabella
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " + vbCrLf)
            'stbQ.Append(" LEFT OUTER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " + vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " & vbCrLf)

            'stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Trim(objParametri.PivaSuperUser) & "' ")
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.piva = Agenda.Piva" + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Modulo_Generazione.Cantine) & "  " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.elem_cod = Movimenti_dettagli.elem_cod " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.mat_cod = Movimenti_dettagli.mat_cod " + vbCrLf)
            'stbQ.Append(" AND OGenerazioni_Anagrafe_Log.id =(             " + vbCrLf)
            'stbQ.Append("                                   SELECT MAX(id) " + vbCrLf)
            'stbQ.Append("                                    FROM OGenerazioni_Anagrafe_Log OLOG " + vbCrLf)
            'stbQ.Append("                                    WHERE OLOG.piva_superuser = OGenerazioni_Anagrafe_Log.Piva_SuperUser " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.piva = OLOG.Piva " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = OLOG.Modulo_Generazione" + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.elem_cod = OLOG.elem_cod  " + vbCrLf)
            'stbQ.Append("                                    AND OGenerazioni_Anagrafe_Log.mat_cod = OLOG.mat_cod " + vbCrLf)
            'stbQ.Append("                                   ) " + vbCrLf)

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            stbQ.Append(" WHERE Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                                        "'" & CAU_SCARICO & "', " &
                                                        "'" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', " &
                                                        "'" & CAU_CONFERIMENTO & "', " &
                                                        "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                                        ") " & vbCrLf)

            stbQ.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If str_idagenda_filtrocategoria <> "" Then
                stbQ.Append(" AND Agenda.Id_Agenda IN " & Agro_SQL_Save_Clausola_IN(str_idagenda_filtrocategoria) & " " & vbCrLf)
            End If
            If str_matcod_filtrocategoria <> "" Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod IN " & Agro_SQL_Save_Clausola_IN(str_matcod_filtrocategoria) & " " & vbCrLf)
            End If
            If str_lotto_filtrocategoria <> "" Then
                stbQ.Append(" AND Movimenti_Dettagli.Lotto IN " & Agro_SQL_Save_Clausola_IN(str_lotto_filtrocategoria, True) & " " & vbCrLf)
            End If

            'stbQ.Append(" AND EXISTS (SELECT 1 FROM Agenda Agenda_Lav INNER JOIN OperazionixReport OPR ON Agenda_Lav.Lav_Cod = OPR.Lav_Cod " + vbCrLf)
            'stbQ.Append("             AND Agenda_Lav.Piva = OPR.Piva AND OPR.Id_Report =  " & Agro_SQL_SaveNum(TipoReport) & "  " + vbCrLf)
            'stbQ.Append("             WHERE  Agenda.Piva = Agenda_Lav.Piva AND Agenda.Id_Agenda = Agenda_Lav.Id_Agenda ) " + vbCrLf)
            'CORREZIONE DEL 07/05/2014: la piva di OperazionixReport è quella del superuser,
            'non va quindi messa in join con la piva di agenda!
            'è stato scoperto ora perchè qualitoscana è il primo cliente col quale si stampano i registri di cantina sulle aziende figlie in gerarchia
            stbQ.Append(" AND EXISTS (SELECT 1 FROM OperazionixReport OPR  " & vbCrLf)
            stbQ.Append("             WHERE  OPR.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQ.Append("             AND OPR.Id_Report = " & Agro_SQL_SaveNum(TipoReport) & "  " & vbCrLf)
            stbQ.Append("               AND Agenda.Lav_Cod = OPR.Lav_Cod ) " & vbCrLf)


            stbQ.Append(" AND Materie_PrimexReport.Id_Report =  " & Agro_SQL_SaveNum(TipoReport) & "  " & vbCrLf)

            '--------------
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)

            'aggiunta il 19/08/13:per escludere carichi/scarichi di operazioni pendenti (pianificate)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            '--------------
            '21/08/2014: adeguato il filtro con la select (viaggia tutto sul campo Ora)
            'stbQ.Append(" AND Movimenti.Data_Movimento <" & IIf(ComprendiData, "= ", " ") & Agro_SQL_SaveDate(Data) & " " + vbCrLf)
            'stbQ.Append(" AND Movimenti.Data_Movimento >=" & Agro_SQL_SaveDate(DataInizioIntervallo) & " " + vbCrLf)
            stbQ.Append(" AND CONVERT(date, Movimenti.Ora) < " & IIf(ComprendiData, "= ", " ") & Agro_SQL_SaveDate(Data) & " " & vbCrLf)
            stbQ.Append(" AND CONVERT(date, Movimenti.Ora) >= " & Agro_SQL_SaveDate(DataInizioIntervallo) & " " & vbCrLf)

            stbQ.Append(" AND Movimenti_dettagli.Udm_Cod=" & Agro_SQL_SaveNum(UdmCod) & " " & vbCrLf)

            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = 'calibro'  " & vbCrLf)
            'corretto bug in data 08/11/2012: leggeva ChkRegistri invece di ChkRegistri_Vinificazione
            stbQ.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri_Vinificazione = 1 " & vbCrLf)

            stbQ.Append(" AND Materie_Prime_Calibri.Cal_Cod=" & Agro_SQL_SaveNum(Cal_Cod) & " " & vbCrLf)

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Id_Destinazione <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " & vbCrLf)
            End If
            If Mat_Cod <> 0 Then
                stbQ.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If
            If Linea_Cod <> 0 Then
                stbQ.AppendLine("   AND  EXISTS (SELECT 1  ")
                stbQ.AppendLine("               FROM OGenerazioni_Anagrafe_Log ")
                stbQ.AppendLine("               WHERE materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod ")
                stbQ.AppendLine("               AND materie_prime.elem_cod = OGenerazioni_Anagrafe_Log.elem_cod ")
                stbQ.AppendLine("               AND OGenerazioni_Anagrafe_Log.linea_cod = " & Agro_SQL_SaveNum(Linea_Cod) & " ")
                stbQ.AppendLine("               ) ")
            End If

            'If CifraStart <> 0 And CifraEnd <> 0 Then
            '    stbQ.Append(" AND SUBSTRING(Movimenti_Dettagli.Lotto, " & CifraStart & "," & CifraEnd - CifraStart + 1 & ")='" & Agro_SQL_SaveNum(AnnoProduzione) & "' " + vbCrLf)
            'End If

            If CifraStart <> 0 AndAlso CifraEnd <> 0 Then
                stbQ.Append(" AND SUBSTRING(Movimenti_Dettagli.Lotto, " & CifraStart & "," & CifraEnd - CifraStart + 1 & ")='" & Agro_SQL_SaveNum(AnnoProduzione) & "' " & vbCrLf)
            Else
                stbQ.Append(FiltroLotti)
            End If

            If Cod_Contatto_Terzi <> "" Then
                'modifica del 21/01/2013: separato il filtro per la parte agenda e la parte linee
                stbQ.Append(RegistroVinificazione_FiltroSQL_ContoTerzi_ParteAgenda(objParametri.PivaSuperUser,
                                                                                    Piva,
                                                                                    Cod_Contatto_Terzi,
                                                                                    Lista_CodRisUm))
                ''aggiunta in data 25/09/2012:
                ''stampa del registro separata per fornitore uve (Cod_Contatto_Terzi)
                'stbQ.Append("   AND  EXISTS (SELECT 1  " + vbCrLf)
                'stbQ.Append("               FROM OGenerazioni_Anagrafe_Log " + vbCrLf)
                'stbQ.Append(" 				INNER JOIN  Linee_Produzioni ON OGenerazioni_Anagrafe_Log.linea_cod =Linee_Produzioni.linea_cod  " + vbCrLf)
                'stbQ.Append(" 				WHERE materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod " + vbCrLf)
                'stbQ.Append("               AND Linee_Produzioni.Cod_Contatto_Terzi = '" & Agro_SQL_SaveText(Cod_Contatto_Terzi) & "' " + vbCrLf)
                'stbQ.Append("               ) " + vbCrLf)
            End If

            'FILTRO PER GESTIONE DELLA VINIFICAZIONE!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            stbQ.Append(RegistroVinificazione_FiltroSQL_Importante(objParametri, Lista_PrepCod, Lista_IdTrasf_NoComm, Piva))


            '------------------------------------------------------ 
            '------------------- GROUP BY -------------------------
            '------------------------------------------------------
            stbQ.Append(" GROUP BY Movimenti.Cau_Mov, Movimenti_dettagli.Udm_Cod " & vbCrLf)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            For i = 0 To DT.Rows.Count - 1
                dRet += CDbl(IIf(Not IsDBNull(DT.Rows(i).Item("saldo")), DT.Rows(i).Item("saldo"), 0))
            Next


        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return dRet

    End Function


    '##########################################################################################
    '##########################################################################################
    '##########################################################################################
    '##########################################################################################

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' registro di imbottigliamento
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RegistroImbottigliamento(ByRef DataSet2Fill As DataSet,
                                            ByVal strNomeDtNelDS As String,
                                            ByVal Piva As String,
                                            ByVal Id_Report As Integer,
                                            ByVal Gestione_Conto_Terzi As enum_RegistroContoTerzi,
                                            ByVal Cod_Contatto_Terzi As String,
                                            ByVal flag_docg As Boolean,
                                            ByVal flag_dop As Boolean,
                                            ByVal flag_igp As Boolean,
                                            ByVal flag_tavola As Boolean,
                                            ByVal DataReportInizio As Date,
                                            ByVal DataReportFine As Date,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByVal Sa_Cod As Integer
                                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreStampeDAL.RegistriCantina.RegistroImbottigliamento"

        Dim messaggioErrore As String = ""
        Dim stbQ As New System.Text.StringBuilder
        'Dim DT As DataTable
        Dim Risp As Boolean = False

        Try

            Dim Str_categorie_log As String = ""

            If flag_docg AndAlso flag_dop AndAlso flag_igp AndAlso flag_tavola Then
                'non facciamo il filtro
            Else
                If flag_docg Then
                    Str_categorie_log &= CStr(enum_Omni_Gradi_Liberta.GL2_Docg) & ","
                End If
                If flag_dop Then
                    Str_categorie_log &= CStr(enum_Omni_Gradi_Liberta.GL2_Dop) & ","
                End If
                If flag_igp Then
                    Str_categorie_log &= CStr(enum_Omni_Gradi_Liberta.GL2_Igp) & ","
                End If
                If flag_tavola Then
                    Str_categorie_log &= CStr(enum_Omni_Gradi_Liberta.GL2_Tavola) & ","
                End If
                If Str_categorie_log <> "" Then
                    Str_categorie_log = Mid(Str_categorie_log, 1, Str_categorie_log.Length - 1)
                End If
            End If

            stbQ.Length = 0

            stbQ.Append(" SELECT DISTINCT Agenda.Id_Agenda, Movimenti.Data_Movimento, Agenda.des_lib, " & vbCrLf)

            ' ho eliminato l'inner join con le Linee_Produzioni per il condizionamento che non è legato a nessuna linea
            'stbQ.Append(" Linee_Produzioni.Linea_Cod_Des, Linee_Produzioni.Linea_Des, Linee_Produzioni.Colore, ")
            stbQ.Append(" ISNULL(Linee_Produzioni.Linea_Cod_Des,'') AS Linea_Cod_Des, ISNULL(Linee_Produzioni.Linea_Des,'') AS Linea_Des, " & vbCrLf)
            'stbQ.Append(" CASE WHEN ISNULL(Linee_Produzioni.Colore, -1) = -1 THEN  ISNULL(Linee_Preparazioni.Colore,-1) ELSE ISNULL(Linee_Produzioni.Colore,-1) END AS Colore, " + vbCrLf)

            '12/04/2017: il lan ha iniziato a salvare male Materie_Prime.Colore, Luke ha chiesto di correggere la stampa:
            'stbQ.Append(" Materie_Prime.Colore,  " + vbCrLf)
            stbQ.Append(" ISNULL(  " & vbCrLf)
            stbQ.Append(" ( SELECT TOP 1 Colore_Log " & vbCrLf)
            'non faccio il case perché nel report c'è la formula il campo del dataset è int
            'stbQ.Append(" CASE WHEN Colore_Log = 16 THEN 'B'   " + vbCrLf)
            'stbQ.Append(" WHEN Colore_Log = 17 THEN 'R'    " + vbCrLf)
            'stbQ.Append(" WHEN Colore_Log = 18 THEN 'S'  " + vbCrLf)
            'stbQ.Append(" WHEN Colore_Log = 678 THEN 'G'  " + vbCrLf)
            'stbQ.Append(" ELSE '' END   " + vbCrLf)
            stbQ.Append(" FROM OGenerazioni_Anagrafe_Log  " & vbCrLf)
            stbQ.Append(" WHERE OGenerazioni_Anagrafe_Log.mat_cod=materie_prime.mat_cod  " & vbCrLf)
            stbQ.Append(" ORDER BY id_generazione DESC  " & vbCrLf)
            stbQ.Append(" )  " & vbCrLf)
            stbQ.Append(" ,'') AS Colore,  " & vbCrLf)

            stbQ.Append(" Movimenti.Cau_Mov, Movimenti.Mov_Desc, Materie_Prime.Mat_Des, " & vbCrLf)

            stbQ.Append(" Materie_Prime.Cod_Articolo, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_cod, " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Lotto, UnitaMisura.UDM_SIM, Movimenti_dettagli.Qta, " & vbCrLf)

            stbQ.Append("       (SELECT SUM(Qta) FROM Agenda A INNER JOIN " & vbCrLf)
            stbQ.Append("        Movimenti ON A.PIVA = Movimenti.PIVA AND A.Id_Agenda = Movimenti.Id_Agenda INNER JOIN " & vbCrLf)
            stbQ.Append("        Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND  " & vbCrLf)
            stbQ.Append("        Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            stbQ.Append("        WHERE Agenda.Id_Agenda = A.Id_Agenda " & vbCrLf)
            stbQ.Append("        AND Cau_Mov='" & CAU_SCARICO & "' AND Udm_Cod=29) AS Qta_Origine, " & vbCrLf)

            stbQ.Append(" CASE WHEN Materie_Prime.Peso_Set>0 THEN Movimenti_dettagli.QTA_EXTRA*Movimenti_dettagli.QTA " & vbCrLf)
            stbQ.Append(" WHEN Materie_Prime.Peso_Set=0 THEN Materie_Prime.QTA_EXTRA*Movimenti_dettagli.QTA " & vbCrLf)
            stbQ.Append(" ELSE 0 END AS Qta_Confezionato  " & vbCrLf)
            'stbQ.Append(" Movimenti_dettagli.QTA_EXTRA*Movimenti_dettagli.QTA AS Qta_Confezionato ")

            stbQ.Append(", Linee_PreparazionixReport.StrCampo_Registri " & vbCrLf)
            stbQ.Append(", Materie_Prime.Peso_Set, Movimenti_dettagli.QTA_EXTRA, Movimenti_dettagli.udm_cod_extra " & vbCrLf)
            stbQ.Append(" , Linee_Produzioni.Categoria_Gias_Cod, Trasformazioni.Trasformazione_Des " & vbCrLf)

            Select Case Gestione_Conto_Terzi
                Case enum_RegistroContoTerzi.RegistroSoloContoLavoro, enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    stbQ.Append(" , Contatti.cod_contatto AS CodContatto_CLavoro " & vbCrLf)
                    stbQ.Append(" , (Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.cognome) AS RagSoc_CLavoro " & vbCrLf)
                Case Else
                    stbQ.Append(" , '' AS CodContatto_CLavoro " & vbCrLf)
                    stbQ.Append(" , '' AS RagSoc_CLavoro " & vbCrLf)
            End Select

            ''inserito il 30/04/2013 per stampare il numero di vasca
            'stbQ.Append(", Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Sa_Cod AS SaCod_Dest, Mov_Destinazioni.Id_Destinazione AS Id_Dest " + vbCrLf)

            stbQ.Append(" FROM Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " & vbCrLf)
            stbQ.Append("            Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Movimenti_dettagli.PIVA = Materie_Prime.Piva AND Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND " & vbCrLf)
            stbQ.Append("            Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)

            '26/08/2015: ormai tutti quelli che fanno i condizionamenti hanno l'omni, quindi le linee
            '-> 22/02/2016: Il mio casale aveva ancora le preparazioni dei condizionamenti scollegati da linea, aggiunto pezzo alla union
            '' left join per il condizionamento che non è legato a nessuna linea produzione
            'stbQ.Append(" LEFT JOIN Linee_Produzioni ON Agenda.LINEA_COD = Linee_Produzioni.Linea_Cod AND Agenda.PIVA = Linee_Produzioni.Piva " + vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Produzioni ON Agenda.LINEA_COD = Linee_Produzioni.Linea_Cod AND Agenda.PIVA = Linee_Produzioni.Piva " & vbCrLf)

            stbQ.Append(" INNER JOIN Trasformazioni ON Linee_Produzioni.linea_cod= Trasformazioni.linea_cod  " & vbCrLf)
            stbQ.Append("       AND Linee_Produzioni.piva= Trasformazioni.piva and Trasformazioni.Id_Trasformazione=Agenda.ID_TRASFORMAZIONE " & vbCrLf)

            'NO, ho la info nella tabella agenda <- metto in join i dettagli di questa operazione per scremare i lotti e ottenere quello della linea  
            'stbQ.Append(" INNER JOIN Movimenti_dettagli Mov_Det_Scarico ON Mov_Det_Scarico.Lotto= Trasformazioni.Trasformazione_Des " + vbCrLf)
            'stbQ.Append(" AND Mov_Det_Scarico.piva= Trasformazioni.piva  AND Mov_Det_Scarico.id_agenda= Movimenti_dettagli.id_agenda  " + vbCrLf)

            stbQ.Append(" INNER JOIN Linee_Preparazioni ON Linee_Preparazioni.Preparazione_Cod = Agenda.PREPARAZIONE_COD AND Linee_Preparazioni.Piva = Agenda.PIVA " & vbCrLf)
            'Modifica del 02/03/2022: Considerata la matrice (modello) perché il GiasLan non scrive più nella tabella Linee_PreparazionixReport
            'stbQ.Append(" INNER JOIN Linee_PreparazionixReport ON Linee_PreparazionixReport.Piva = Linee_Preparazioni.Piva AND Linee_PreparazionixReport.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod " + vbCrLf)
            stbQ.Append(" INNER JOIN Linee_PreparazionixReport ON Linee_PreparazionixReport.Piva = 'AAAAAAAAAAA' AND Linee_PreparazionixReport.Preparazione_Cod = Linee_Preparazioni.Codice_Generazione " & vbCrLf)

            stbQ.Append(" INNER JOIN Materie_PrimexReport ON Materie_Prime.Piva = Materie_PrimexReport.Piva AND Materie_Prime.Mat_Cod = Materie_PrimexReport.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD" & vbCrLf)

            If Str_categorie_log <> "" Then
                stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON Movimenti_Dettagli.Mat_Cod = OGenerazioni_Anagrafe_Log.Mat_Cod AND Movimenti_Dettagli.elem_Cod = OGenerazioni_Anagrafe_Log.elem_Cod AND Linee_Produzioni.Linea_Cod = OGenerazioni_Anagrafe_Log.linea_Cod  " & vbCrLf)
            End If

            If Gestione_Conto_Terzi = enum_RegistroContoTerzi.RegistroSoloContoLavoro OrElse Gestione_Conto_Terzi = enum_RegistroContoTerzi.RegistroSeparatoContoTerzi Then
                stbQ.Append(" INNER JOIN contatti ON contatti.piva = Linee_Produzioni.piva AND contatti.cod_contatto = Linee_Produzioni.cod_contatto_terzi  " & vbCrLf)
            End If

            stbQ.AppendLine(" WHERE  movimenti.Cau_Mov='" & CAU_CARICO & "' ")
            stbQ.AppendLine(" AND Agenda.PIVA='" & Agro_SQL_SaveText(Piva) & "'  ")
            stbQ.AppendLine(" AND Materie_PrimexReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  ")
            stbQ.AppendLine(" AND Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  ")
            stbQ.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " ")
            stbQ.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " ")

            '--------------
            'aggiunto in data 23/09/2013
            stbQ.AppendLine(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   ")
            stbQ.AppendLine(" AND   Movimenti_Dettagli.Contabilizzato >= 0  ")
            '--------------

            Select Case Gestione_Conto_Terzi
                Case enum_RegistroContoTerzi.RegistroSoloContoLavoro
                    stbQ.AppendLine(" AND Linee_Produzioni.cod_contatto_terzi <> '" & Agro_SQL_SaveText(Piva) & "'  ")
                Case enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    stbQ.AppendLine(" AND Linee_Produzioni.cod_contatto_terzi = '" & Agro_SQL_SaveText(Cod_Contatto_Terzi) & "'  ")
            End Select

            If Str_categorie_log <> "" Then
                stbQ.AppendLine(" AND OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                stbQ.AppendLine(" AND OGenerazioni_Anagrafe_Log.Categoria_Gias_Cod_Log IN (" & Agro_SQL_Save_Clausola_IN(Str_categorie_log) & ") ")
            End If

            If Sa_Cod <> 0 Then
                stbQ.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If
            'If Id_Destinazione <> 0 Then
            '    stbQ.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  ")
            'End If


            'aggiunta del 22/02/2016: c'è chi ancora ha i condizionamenti slegati dalle linee (il mio casale)
            stbQ.AppendLine("  ")
            stbQ.AppendLine("  ")
            stbQ.AppendLine(" -- PARTE DI UNION CHE LEGGE LE PREPARAZIONI SCOLLEGATE DA LINEE (AD ESEMPIO VECCHI CONDIZIONAMENTI) ")
            stbQ.AppendLine(" UNION ALL ")
            stbQ.AppendLine("  ")

            stbQ.AppendLine(" SELECT DISTINCT Agenda.Id_Agenda, Movimenti.Data_Movimento, Agenda.des_lib, ")
            stbQ.AppendLine(" '' AS Linea_Cod_Des, '' AS Linea_Des, ")
            stbQ.AppendLine(" Materie_Prime.Colore,  ")
            stbQ.AppendLine(" Movimenti.Cau_Mov, Movimenti.Mov_Desc, Materie_Prime.Mat_Des, ")

            stbQ.AppendLine(" Materie_Prime.Cod_Articolo, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_cod, ")
            stbQ.AppendLine(" Movimenti_dettagli.Lotto, UnitaMisura.UDM_SIM, Movimenti_dettagli.Qta, ")

            stbQ.AppendLine("       (SELECT SUM(Qta) FROM Agenda A INNER JOIN ")
            stbQ.AppendLine("        Movimenti ON A.PIVA = Movimenti.PIVA AND A.Id_Agenda = Movimenti.Id_Agenda INNER JOIN ")
            stbQ.AppendLine("        Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND  ")
            stbQ.AppendLine("        Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            stbQ.AppendLine("        WHERE Agenda.Id_Agenda = A.Id_Agenda ")
            stbQ.AppendLine("        AND Cau_Mov='" & CAU_SCARICO & "' AND Udm_Cod=29) AS Qta_Origine, ")

            stbQ.AppendLine(" CASE WHEN Materie_Prime.Peso_Set>0 THEN Movimenti_dettagli.QTA_EXTRA*Movimenti_dettagli.QTA ")
            stbQ.AppendLine(" WHEN Materie_Prime.Peso_Set=0 THEN Materie_Prime.QTA_EXTRA*Movimenti_dettagli.QTA ")
            stbQ.AppendLine(" ELSE 0 END AS Qta_Confezionato  ")

            stbQ.AppendLine(", Linee_PreparazionixReport.StrCampo_Registri ")
            stbQ.AppendLine(", Materie_Prime.Peso_Set, Movimenti_dettagli.QTA_EXTRA, Movimenti_dettagli.udm_cod_extra ")
            stbQ.AppendLine(" , 0 AS Categoria_Gias_Cod, Trasformazioni.Trasformazione_Des ")

            stbQ.AppendLine(" , '' AS CodContatto_CLavoro ")
            stbQ.AppendLine(" , '' AS RagSoc_CLavoro ")

            stbQ.Append(" FROM Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " & vbCrLf)
            stbQ.Append("            Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Movimenti_dettagli.PIVA = Materie_Prime.Piva AND Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND " & vbCrLf)
            stbQ.Append("            Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)

            stbQ.Append(" INNER JOIN Trasformazioni ON Agenda.piva= Trasformazioni.piva AND Trasformazioni.Id_Trasformazione = Agenda.ID_TRASFORMAZIONE " & vbCrLf)
            'Modifica del 02/03/2022: Considerata la matrice (modello) perché il GiasLan non scrive più nella tabella Linee_PreparazionixReport
            'stbQ.Append(" INNER JOIN Linee_PreparazionixReport ON Linee_PreparazionixReport.Piva = Agenda.Piva AND Linee_PreparazionixReport.Preparazione_Cod = Agenda.Preparazione_Cod " + vbCrLf)
            stbQ.Append(" INNER JOIN Linee_PreparazionixReport ON Linee_PreparazionixReport.Piva = 'AAAAAAAAAAA' AND Linee_PreparazionixReport.Preparazione_Cod In (select  Codice_Generazione from Linee_Preparazioni as LP where LP.Preparazione_Cod =  Agenda.Preparazione_Cod ) " & vbCrLf)

            stbQ.Append(" INNER JOIN Materie_PrimexReport ON Materie_Prime.Piva = Materie_PrimexReport.Piva AND Materie_Prime.Mat_Cod = Materie_PrimexReport.Mat_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD" & vbCrLf)

            If Str_categorie_log <> "" Then
                'in questo caso non c'è AND Linee_Produzioni.Linea_Cod = OGenerazioni_Anagrafe_Log.linea_Cod
                stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON Movimenti_Dettagli.Mat_Cod = OGenerazioni_Anagrafe_Log.Mat_Cod AND Movimenti_Dettagli.elem_Cod = OGenerazioni_Anagrafe_Log.elem_Cod   " & vbCrLf)
            End If

            stbQ.Append(" WHERE  movimenti.Cau_Mov='" & CAU_CARICO & "' " & vbCrLf)
            stbQ.Append(" AND Agenda.Lav_cod = " & Agro_SQL_SaveNum(LAVCOD_TRASFORMAZIONI) & "  " & vbCrLf)
            'linea_cod = 0 -> preparazioni non collegate a linee
            stbQ.Append(" AND Agenda.Linea_cod = 0  " & vbCrLf)
            stbQ.Append(" AND Agenda.PIVA='" & Agro_SQL_SaveText(Piva) & "'  " & vbCrLf)
            'i filtri sull'id_report permettono di scremare le preparazioni delle piadine/farine/ecc (anche quelle hanno lav_cod=5000)
            stbQ.Append(" AND Materie_PrimexReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)
            stbQ.Append(" AND Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)
            stbQ.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " " & vbCrLf)
            stbQ.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataReportFine) & " " & vbCrLf)

            '--------------
            'aggiunto in data 23/09/2013
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            If Str_categorie_log <> "" Then
                stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
                stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Categoria_Gias_Cod_Log IN (" & Agro_SQL_Save_Clausola_IN(Str_categorie_log) & ") " & vbCrLf)
            End If

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            'If Id_Destinazione <> 0 Then
            '    stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " + vbCrLf)
            'End If

            'nota del 15/02/16: la seconda parte legge dei dati ormai non più gestiti (agli albori del gias cantine)
            'lato giaslan il codice c'è ancora, ma Marco ha detto ceh si può togliere dalla query

            ''-------------------------------------------------------------------------------
            ''-- ATTENZIONE! USARE L'UNION ALL, PERCHE' L'UNION FA IL DISTINCT! -------------
            ''-------------------------------------------------------------------------------
            'stbQ.Append("  " + vbCrLf)
            'stbQ.Append(" UNION ALL " + vbCrLf)
            'stbQ.Append("  " + vbCrLf)

            'stbQ.Append(" SELECT Agenda.Id_Agenda, Movimenti.Data_Movimento, Agenda.des_lib, '' AS Linea_Cod_Des, '' AS Linea_Des,  " + vbCrLf)
            ''stbQ.Append(" '' AS Colore,  " + vbCrLf)
            'stbQ.Append(" Materie_Prime.Colore,  " + vbCrLf)
            'stbQ.Append(" Movimenti.Cau_Mov, Movimenti.Mov_Desc, Materie_Prime.Mat_Des,   " + vbCrLf)

            'stbQ.Append(" Materie_Prime.Cod_Articolo, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_cod, " + vbCrLf)
            'stbQ.Append(" Movimenti_dettagli.Lotto, UnitaMisura.UDM_SIM, Movimenti_dettagli.Qta, " + vbCrLf)

            'stbQ.Append("       (SELECT SUM(Qta) FROM Agenda A   " + vbCrLf)
            'stbQ.Append("       INNER JOIN Movimenti ON A.PIVA = Movimenti.PIVA AND A.Id_Agenda = Movimenti.Id_Agenda " + vbCrLf)
            'stbQ.Append("       INNER JOIN   Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov   " + vbCrLf)
            'stbQ.Append("       WHERE(Mov_Dettagli_Riferimenti.Id_Agenda = A.Id_Agenda Or Mov_Dettagli_Riferimenti.Id_Agenda_Rif = A.Id_Agenda) " + vbCrLf)
            'stbQ.Append("       AND Cau_Mov='7350' AND Udm_Cod=29) AS Qta_Origine,  " + vbCrLf)

            'stbQ.Append(" CASE WHEN Materie_Prime.Peso_Set>0 THEN Movimenti_dettagli.QTA_EXTRA*Movimenti_dettagli.QTA " + vbCrLf)
            'stbQ.Append(" WHEN Materie_Prime.Peso_Set=0 THEN Materie_Prime.QTA_EXTRA*Movimenti_dettagli.QTA " + vbCrLf)
            'stbQ.Append(" ELSE 0 END AS Qta_Confezionato    " + vbCrLf)

            ''stbQ.Append(" Materie_Prime.QTA_EXTRA*Movimenti_dettagli.QTA AS Qta_Confezionato  ")

            'stbQ.Append(", '' AS StrCampo_Registri " + vbCrLf)
            'stbQ.Append(", Materie_Prime.Peso_Set, Movimenti_dettagli.QTA_EXTRA, Movimenti_dettagli.udm_cod_extra " + vbCrLf)
            'stbQ.Append(" , 0 AS Categoria_Gias_Cod, '' AS Trasformazione_Des " + vbCrLf)

            '''inserito il 30/04/2013 per stampare il numero di vasca
            ''stbQ.Append(", Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Sa_Cod AS SaCod_Dest, Mov_Destinazioni.Id_Destinazione AS Id_Dest " + vbCrLf)

            'If Gestione_Conto_Terzi = enum_RegistroContoTerzi.RegistroSoloContoLavoro Then
            '    stbQ.Append(" , '' AS CodContatto_CLavoro " + vbCrLf)
            '    stbQ.Append(" , '' AS RagSoc_CLavoro " + vbCrLf)
            'End If

            'stbQ.Append(" FROM Mov_dettagli_Riferimenti, Agenda " + vbCrLf)
            'stbQ.Append(" INNER JOIN OperazionixReport ON Agenda.Lav_Cod = OperazionixReport.Lav_Cod  " + vbCrLf)
            'stbQ.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod  " + vbCrLf)
            'stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov  " + vbCrLf)
            'stbQ.Append(" INNER JOIN Materie_Prime ON Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod AND Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod  " + vbCrLf)
            'stbQ.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_dettagli.Udm_Cod  " + vbCrLf)
            'stbQ.Append(" INNER JOIN Materie_PrimexReport ON Materie_Prime.Piva = Materie_PrimexReport.Piva AND Materie_Prime.Mat_Cod = Materie_PrimexReport.Mat_Cod   " + vbCrLf)

            '''inserito il 30/04/2013 per stampare il numero di vasca
            ''stbQ.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " + vbCrLf)

            'stbQ.Append(" WHERE  Movimenti.Cau_Mov= '" + CStr(CAU_CARICO) + "' " + vbCrLf)
            'stbQ.Append(" AND OperazionixReport.Id_report = " & Agro_SQL_SaveNum(enum_AgroReportistica.Imbottigliamento) & "  " + vbCrLf)
            'stbQ.Append(" AND Materie_PrimexReport.Id_report = " & Agro_SQL_SaveNum(enum_AgroReportistica.Imbottigliamento) & "  " + vbCrLf)

            'stbQ.Append(" AND ((Mov_Dettagli_Riferimenti.Piva = Agenda.Piva AND Mov_Dettagli_Riferimenti.Id_Agenda = Agenda.Id_Agenda)  " + vbCrLf)
            'stbQ.Append(" OR  (Mov_Dettagli_Riferimenti.Piva_Rif = Agenda.Piva  AND  Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Agenda.Id_Agenda))  " + vbCrLf)

            'stbQ.Append(" AND Mov_Dettagli_Riferimenti.Cau_Mov = '10500'  " + vbCrLf)

            ''stbQ.Append(" AND Movimenti.Data_Movimento >= CONVERT(DateTime,'2008/10/01',120)  AND Movimenti.Data_Movimento <= CONVERT(DateTime,'2008/10/31',120)    ")
            'stbQ.Append(" AND Agenda.PIVA='" & Agro_SQL_SaveText(Piva) & "'  " + vbCrLf)

            ''--------------
            ''aggiunto in data 23/09/2013
            'stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " + CStr(MagazzinoMovimentato) + "   " + vbCrLf)
            'stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " + vbCrLf)
            ''--------------

            'stbQ.Append(" ORDER BY Agenda.Id_Agenda, Movimenti.Data_Movimento ")
            stbQ.Append(" ORDER BY Movimenti.Data_Movimento, Agenda.Id_Agenda  " & vbCrLf)


            '--------------------------------------------------------------------------
            Risp = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine, DataSet2Fill, strNomeDtNelDS)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Risp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return Risp

    End Function

    '###################################################################################
    Public Sub Verifica_JollyInt(ByVal Piva As String,
                                ByRef LogXUtente As String,
                                ByRef LogXAdmin As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                )

        Dim dt As DataTable
        Dim j As Integer
        Dim Str_Query As String = ""

        'non azzero perchè il log è comune a più funzioni di verifica
        'LogXUtente = ""
        'LogXAdmin = ""

        dt = Leggi_OpNoMagazzino_JollyInt0(Piva, Str_Query, objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            LogXAdmin += vbCrLf & Str_Query & vbCrLf & vbCrLf

            LogXAdmin += "Verifica causali che movimentano il magazzino." & vbCrLf
            LogXAdmin += "Ci sono " & CStr(dt.Rows.Count) & " operazioni errate." & vbCrLf

            LogXUtente += vbCrLf & "ATTENZIONE!!!" & vbCrLf
            LogXUtente += "Ci sono le seguenti operazioni da sistemare." & vbCrLf
            LogXUtente += "Movimentano il magazzino invece non dovrebbero farlo. " & vbCrLf & vbCrLf

            For j = 0 To dt.Rows.Count - 1

                LogXAdmin += "id_agenda= " & CStr(dt.Rows(j).Item("id_agenda")) & " - " &
                                "lav_cod= " & CStr(dt.Rows(j).Item("lav_cod")) & " - " &
                                "des_lib= " & CStr(dt.Rows(j).Item("des_lib")) & " - " &
                                "mat_des= " & CStr(dt.Rows(j).Item("mat_des")) & " - " &
                                "data_movimento= " & CStr(dt.Rows(j).Item("data_movimento")) & " - " &
                                "ora= " & CStr(dt.Rows(j).Item("ora")) & " " &
                                "data_creazione= " & CStr(dt.Rows(j).Item("data_creazione")) & " " &
                                vbCrLf & vbCrLf

                LogXUtente += " " & CStr(dt.Rows(j).Item("des_lib")) & " " &
                                " del " & CStr(dt.Rows(j).Item("data_movimento")) & " " &
                                " prodotto: " & (dt.Rows(j).Item("mat_des")) & " " &
                                vbCrLf

            Next


        End If


        dt = Leggi_OpMagazzino_JollyInt1(Piva, Str_Query, objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            LogXAdmin += vbCrLf & Str_Query & vbCrLf & vbCrLf

            LogXAdmin += "Verifica causali che NON movimentano il magazzino." & vbCrLf
            LogXAdmin += "Ci sono " & CStr(dt.Rows.Count) & " operazioni errate." & vbCrLf

            LogXUtente += "ATTENZIONE!!!" & vbCrLf
            LogXUtente += "Ci sono le seguenti operazioni da sistemare." & vbCrLf
            LogXUtente += "Non movimentano il magazzino invece dovrebbero farlo. " & vbCrLf

            For j = 0 To dt.Rows.Count - 1

                LogXAdmin += "id_agenda= " & CStr(dt.Rows(j).Item("id_agenda")) & " - " &
                                "lav_cod= " & CStr(dt.Rows(j).Item("lav_cod")) & " - " &
                                "des_lib= " & CStr(dt.Rows(j).Item("des_lib")) & " - " &
                                "mat_des= " & CStr(dt.Rows(j).Item("mat_des")) & " - " &
                                "data_movimento= " & CStr(dt.Rows(j).Item("data_movimento")) & " - " &
                                "ora= " & CStr(dt.Rows(j).Item("ora")) & " " &
                                "data_creazione= " & CStr(dt.Rows(j).Item("data_creazione")) & " " &
                                vbCrLf & vbCrLf

                LogXUtente += " " & CStr(dt.Rows(j).Item("des_lib")) & " " &
                                " del " & CStr(dt.Rows(j).Item("data_movimento")) & " " &
                                " prodotto: " & (dt.Rows(j).Item("mat_des")) & " " &
                                vbCrLf

            Next


        End If


    End Sub

    '
    '###################################################################################
    Public Sub Verifica_AltreMateriePrime_ConfigProprieta(ByVal Piva As String,
                                                            ByRef LogXUtente As String,
                                                            ByRef LogXAdmin As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            )

        Dim dt As DataTable
        Dim j As Integer
        Dim Str_Query As String = ""

        'non azzero perchè il log è comune a più funzioni di verifica
        'LogXUtente = ""
        'LogXAdmin = ""

        dt = Leggi_AltreMateriePrime_SenzaConfigProprieta(Piva, Str_Query, objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            LogXAdmin += vbCrLf & Str_Query & vbCrLf & vbCrLf

            'LogXAdmin += "Verifica causali che movimentano il magazzino." & vbCrLf
            'LogXAdmin += "Ci sono " & CStr(dt.Rows.Count) & " operazioni errate." & vbCrLf

            LogXUtente += vbCrLf & "ATTENZIONE!!!" & vbCrLf
            LogXUtente += "Ci sono i seguenti lotti non configurati per il c/lavoro e/o la categoria." & vbCrLf
            LogXUtente += "Verificare e sistemare per il corretto funzionamento dei registri di cantina. " & vbCrLf & vbCrLf

            For j = 0 To dt.Rows.Count - 1

                'LogXAdmin += "id_agenda= " & CStr(dt.Rows(j).Item("id_agenda")) & " - " & _
                '                "lav_cod= " & CStr(dt.Rows(j).Item("lav_cod")) & " - " & _
                '                "des_lib= " & CStr(dt.Rows(j).Item("des_lib")) & " - " & _
                '                "mat_des= " & CStr(dt.Rows(j).Item("mat_des")) & " - " & _
                '                "data_movimento= " & CStr(dt.Rows(j).Item("data_movimento")) & " - " & _
                '                "ora= " & CStr(dt.Rows(j).Item("ora")) & " " & _
                '                "data_creazione= " & CStr(dt.Rows(j).Item("data_creazione")) & " " & _
                '                vbCrLf & vbCrLf

                LogXAdmin += "elem_cod= " & CStr(dt.Rows(j).Item("elem_cod")) & " - " &
                             "mat_cod= " & CStr(dt.Rows(j).Item("mat_cod")) & " - " &
                             "mat_des= " & CStr(dt.Rows(j).Item("mat_des")) & " - " &
                             "lotto= " & CStr(dt.Rows(j).Item("lotto")) &
                             vbCrLf & vbCrLf

                LogXUtente += CStr(dt.Rows(j).Item("mat_des")) & " " &
                             "lotto: " & CStr(dt.Rows(j).Item("lotto")) &
                                vbCrLf

            Next


        End If




    End Sub


    '###################################################################################
    Public Sub Verifica_Ora_31_12_2100(ByVal Piva As String,
                                        ByRef LogXUtente As String,
                                        ByRef LogXAdmin As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        )

        Dim dt As DataTable
        Dim j As Integer
        Dim Str_Query As String = ""

        'non azzero perchè il log è comune a più funzioni di verifica
        'LogXUtente = ""
        'LogXAdmin = ""

        dt = Leggi_Ora_31_12_2100_OperazionixReport(Piva, Str_Query, objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            LogXAdmin += vbCrLf & Str_Query & vbCrLf & vbCrLf

            LogXAdmin += "Verifica record di movimenti che hanno 31/12/2100 o 01/01/1900 nel campo ORA (quello considerato nei registri di cantina)." & vbCrLf
            LogXAdmin += "Ci sono " & CStr(dt.Rows.Count) & " record da sistemare." & vbCrLf

            LogXUtente += vbCrLf & "ATTENZIONE!!!" & vbCrLf
            LogXUtente += "Ci sono le seguenti operazioni da sistemare." & vbCrLf
            LogXUtente += "Hanno data di consegna indefinita e non verrebbero considerate nei registri di cantina, " & vbCrLf
            LogXUtente += "oppure hanno la data di consegna futura o antecedente alla data del movimento (in tal caso verificare se è corretto)." & vbCrLf

            For j = 0 To dt.Rows.Count - 1

                LogXAdmin += "id_agenda= " & CStr(dt.Rows(j).Item("id_agenda")) & " - " &
                                "lav_cod= " & CStr(dt.Rows(j).Item("lav_cod")) & " - " &
                                "des_lib= " & CStr(dt.Rows(j).Item("des_lib")) & " - " &
                                "data_movimento= " & CStr(dt.Rows(j).Item("data_movimento")) & " - " &
                                "ora= " & CStr(dt.Rows(j).Item("ora")) & " " &
                                "data_creazione= " & CStr(dt.Rows(j).Item("data_creazione")) & " " &
                                vbCrLf & vbCrLf

                'non lo leggo più, così non sdoppia i movimenti
                '"cau_mov= " & CStr(dt.Rows(j).Item("cau_mov")) & " " & _

                LogXUtente += " " & CStr(dt.Rows(j).Item("des_lib")) & " " &
                                " del " & CStr(dt.Rows(j).Item("data_movimento")) & " " &
                                " data consegna: " & CDate(dt.Rows(j).Item("ora")).ToShortDateString & " " &
                                vbCrLf

            Next


        End If


    End Sub

    '###################################################################################
    Public Sub Verifica_Doppioni_Lotto(ByVal Piva As String,
                                        ByRef LogXUtente As String,
                                        ByRef LogXAdmin As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        )

        Dim dt As DataTable
        Dim j As Integer

        'non azzero perchè il log è comune a più funzioni di verifica
        'LogXUtente = ""
        'LogXAdmin = ""

        dt = LeggiDoppioniLotto(Piva, "", "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            LogXAdmin += "Verifica LOTTI DUPLICATI." & vbCrLf
            LogXAdmin += "Trovati " & CStr(dt.Rows.Count) & " doppioni." & vbCrLf

            LogXUtente += vbCrLf & "ATTENZIONE, ci sono LOTTI DUPLICATI!" & vbCrLf & vbCrLf

            'LogXUtente += "" & vbCrLf

            For j = 0 To dt.Rows.Count - 1

                LogXAdmin += "piva= " & CStr(dt.Rows(j).Item("piva")) & " - " &
                                "trasformazione_des= " & CStr(dt.Rows(j).Item("trasformazione_des")) & "  " &
                                vbCrLf & vbCrLf

                LogXUtente += CStr(dt.Rows(j).Item("trasformazione_des")) & "  " & vbCrLf


            Next

        End If

    End Sub

    '###################################################################################
    Public Sub Verifica_LavCodAgenda_OperazionixReport(ByVal Piva As String,
                                                        ByRef LogXUtente As String,
                                                        ByRef LogXAdmin As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        )

        Dim dt As DataTable
        Dim j As Integer

        'non azzero perchè il log è comune a più funzioni di verifica
        'LogXUtente = ""
        'LogXAdmin = ""

        dt = Leggi_LavCodAgenda_OperazionixReport(Piva, objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            LogXAdmin += "Verifica operazioni non configurate su registri di cantina." & vbCrLf
            LogXAdmin += "Trovati " & CStr(dt.Rows.Count) & " lav_cod." & vbCrLf

            LogXUtente += vbCrLf & "ATTENZIONE! ci sono operazioni non configurate sui registri di cantina, contattare l'amministratore." & vbCrLf & vbCrLf

            'LogXUtente += "" & vbCrLf

            For j = 0 To dt.Rows.Count - 1

                LogXAdmin += "lav_cod= " & CStr(dt.Rows(j).Item("lav_cod")) & " - " &
                                "lav_des= " & CStr(dt.Rows(j).Item("lav_des")) & "  " &
                                vbCrLf & vbCrLf

                LogXUtente += CStr(dt.Rows(j).Item("lav_des")) & "  " & vbCrLf


            Next

        End If

    End Sub


    '###################################################################################
    Public Sub Verifica_Operazioni_Pianificate(ByVal Piva As String,
                                            ByRef LogXUtente As String,
                                            ByRef LogXAdmin As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            )

        Dim dt As DataTable
        Dim j As Integer

        'non azzero perchè il log è comune a più funzioni di verifica
        'LogXUtente = ""
        'LogXAdmin = ""

        dt = Leggi_Operazioni_Pianificate(Piva, objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

            LogXAdmin += "Verifica operazioni pianificate." & vbCrLf
            LogXAdmin += "Trovati " & CStr(dt.Rows.Count) & " lav_cod." & vbCrLf

            LogXUtente += vbCrLf & "ATTENZIONE! Ci sono le seguenti operazioni pianificate: " & vbCrLf
            LogXUtente += " (non movimentano magazzino/vasca) " & vbCrLf & vbCrLf

            'LogXUtente += "" & vbCrLf

            For j = 0 To dt.Rows.Count - 1

                LogXAdmin += "lav_cod= " & CStr(dt.Rows(j).Item("lav_cod")) & " - " &
                                "id_agenda= " & CStr(dt.Rows(j).Item("id_agenda")) & " - " &
                                "des_lib= " & CStr(dt.Rows(j).Item("des_lib")) & " - " &
                                " data_movimento " & CStr(dt.Rows(j).Item("data_movimento")) & " " &
                                vbCrLf & vbCrLf

                LogXUtente += " " & CStr(dt.Rows(j).Item("des_lib")) & " " &
                                " del " & CStr(dt.Rows(j).Item("data_movimento")) & " " &
                                vbCrLf

                ' LogXUtente += " " 

            Next

        End If

    End Sub

    '###################################################################################
    Public Sub Verifica_RilevamentiVinoSfuso_GiaACommercializzazione(
                                        ByVal Piva As String,
                                        ByRef LogXUtente As String,
                                        ByRef LogXAdmin As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        )

        Dim dt As DataTable
        Dim j As Integer
        Dim Str_Query As String = ""

        Dim NomeRoutine As String = "Verifica_RilevamentiVinoSfuso_GiaACommercializzazione"
        Dim MessaggioErrore As String = ""

        'non azzero perchè il log è comune a più funzioni di verifica
        'LogXUtente = ""
        'LogXAdmin = ""

        Try

            dt = Leggi_RilevamentiVinoSfuso_GiaACommercializzazione(Piva, Str_Query, objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

                LogXAdmin += vbCrLf & Str_Query & vbCrLf & vbCrLf

                LogXAdmin += "Verifica Lotti settati a Commercializzazione (ChkPassaggio=1) dove Passaggio_Data coincide con il campo ORA di Movimenti " & vbCrLf
                LogXAdmin += "(non va bene, deve essere antecedente di qualche minuto, altrimenti il rilevamento viene stampato nel registro di vinificazione. " & vbCrLf
                LogXAdmin += "Ci sono " & CStr(dt.Rows.Count) & " record da sistemare." & vbCrLf

                'LogXUtente += "ATTENZIONE!!!" & vbCrLf
                'LogXUtente += "Ci sono le seguenti operazioni da sistemare." & vbCrLf

                Dim Passaggio_Data_Old As DateTime
                Dim Passaggio_Data_New As DateTime
                Dim risp As Boolean

                For j = 0 To dt.Rows.Count - 1

                    LogXAdmin += "piva= " & CStr(dt.Rows(j).Item("piva")) & " - " &
                                    "id_agenda= " & CStr(dt.Rows(j).Item("id_agenda")) & " - " &
                                    "des_lib= " & CStr(dt.Rows(j).Item("des_lib")) & " - " &
                                    "id_trasformazione= " & CStr(dt.Rows(j).Item("id_trasformazione")) & " - " &
                                    "Passaggio_Data= " & CStr(dt.Rows(j).Item("Passaggio_Data")) & " - " &
                                    "data_creazione= " & CStr(dt.Rows(j).Item("data_creazione")) & " " &
                                    vbCrLf & vbCrLf


                    'QUERY DI UPDATE PER CORREGGERE LA DATA
                    Passaggio_Data_Old = dt.Rows(j).Item("Ora")
                    Passaggio_Data_New = Passaggio_Data_Old.AddMinutes(-10)

                    risp = Update_Passaggio_Data(Piva, dt.Rows(j).Item("Id_Trasformazione"), Passaggio_Data_New, "", objParametri)

                    LogXAdmin += "Update OK, sostituito " & CStr(Passaggio_Data_Old) & " con " & CStr(Passaggio_Data_New) & vbCrLf

                    'LogXUtente += " " & CStr(dt.Rows(j).Item("des_lib")) & " " & _
                    '                " del " & CStr(dt.Rows(j).Item("data_movimento")) & " " & _
                    '                " data consegna: " & CDate(dt.Rows(j).Item("ora")).ToShortDateString & " " & _
                    '                vbCrLf

                Next


            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            LogXAdmin += MessaggioErrore
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub

    '#########################################################################
    Public Function Update_Passaggio_Data(ByVal Piva As String,
                                            ByVal Id_Trasformazione As Int32,
                                            ByVal Passaggio_Data As DateTime,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Boolean

        Dim NomeRoutine As String = "Update_Passaggio_Data()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Id_Trasformazione = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Trasformazione obbligatorio)")
            End If

            StrSQL.Length = 0
            StrSQL.Append(" UPDATE Trasformazioni ")
            StrSQL.Append(" SET Passaggio_Data = " & Agro_SQL_SaveDateTime(Passaggio_Data))
            StrSQL.Append(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.Append(" AND Id_Trasformazione = " & Agro_SQL_SaveNum(Id_Trasformazione) & "   ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return xRisp

    End Function

    '###################################################################################
    Public Function Leggi_OpNoMagazzino_JollyInt0(ByVal Piva As String,
                                                ByRef Str_Query As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.RegistriCantina.Leggi_OpNoMagazzino_JollyInt0"
        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Str_Query = ""

        Try

            StbSQL.Length = 0
            StbSQL.Append(" -- elenco operazioni che non devono movimentare il magazzino " & vbCrLf)
            StbSQL.Append(" -- ma sono salvate male e lo movimentano " & vbCrLf)
            StbSQL.Append(" SELECT DISTINCT des_lib, agenda.lav_cod, data_movimento, ora, agenda.data_creazione, Agenda.id_agenda, mat_des  " & vbCrLf)
            StbSQL.Append(" FROM movimenti  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Agenda  " & vbCrLf)
            StbSQL.Append(" ON Agenda.PIVA = movimenti.piva  " & vbCrLf)
            StbSQL.Append(" AND Agenda.Id_Agenda = movimenti.id_agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN OperazionixReport  " & vbCrLf)
            StbSQL.Append(" ON operazionixreport.LAV_COD = Agenda.Lav_Cod  " & vbCrLf)
            StbSQL.Append(" INNER JOIN movimenti_dettagli ON movimenti_dettagli.PIVA = movimenti.piva AND movimenti_dettagli.sa_cod = movimenti.sa_cod " & vbCrLf)
            StbSQL.Append(" AND movimenti_dettagli.Id_Agenda = movimenti.id_agenda AND movimenti_dettagli.Id_MOV = movimenti.id_mov " & vbCrLf)
            StbSQL.Append(" INNER JOIN materie_prime ON movimenti_dettagli.elem_cod = materie_prime.elem_cod  " & vbCrLf)
            StbSQL.Append(" AND movimenti_dettagli.mat_cod = materie_prime.mat_cod  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_PrimexReport   " & vbCrLf)
            StbSQL.Append(" ON materie_prime.mat_cod  = Materie_PrimexReport.MAT_COD  " & vbCrLf)
            StbSQL.Append(" AND Materie_PrimexReport.pro_cod =0  " & vbCrLf)
            StbSQL.Append("               " & vbCrLf)
            StbSQL.Append(" WHERE 1 = 1 " & vbCrLf)
            StbSQL.Append(" AND Jolly_Int = " & Agro_SQL_SaveNum(enum_TipoMovimentazioneMagazzino.MagazzinoMovimentato) & "  " & vbCrLf)
            StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL.Append(" AND    Agenda.Lav_Cod IN ( " & LAVCOD_FATTURA_PROFORMA & ", " &
                                                        LAVCOD_DAA_EMESSO & ", " &
                                                        LAVCOD_ORDINE_VENDITA & ", " &
                                                        LAVCOD_ORDINE_ACQUISTO & ", " &
                                                        LAVCOD_PREVENTIVO_VENDITA &
                                                        "  ) " & vbCrLf)

            StbSQL.Append(" ORDER BY Data_Movimento DESC  " & vbCrLf)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Str_Query = StbSQL.ToString

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    '###################################################################################
    Public Function Leggi_OpMagazzino_JollyInt1(ByVal Piva As String,
                                                ByRef Str_Query As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.RegistriCantina.Leggi_OpMagazzino_JollyInt1"
        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Str_Query = ""

        Try

            StbSQL.Length = 0
            StbSQL.Append(" -- elenco operazioni che  devono movimentare il magazzino " & vbCrLf)
            StbSQL.Append(" -- ma sono salvate male e non lo movimentano " & vbCrLf)
            StbSQL.Append(" SELECT DISTINCT des_lib, agenda.lav_cod, data_movimento, ora, agenda.data_creazione, Agenda.id_agenda, mat_des  " & vbCrLf)
            StbSQL.Append(" FROM movimenti  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Agenda  " & vbCrLf)
            StbSQL.Append(" ON Agenda.PIVA = movimenti.piva  " & vbCrLf)
            StbSQL.Append(" AND Agenda.Id_Agenda = movimenti.id_agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN OperazionixReport  " & vbCrLf)
            StbSQL.Append(" ON operazionixreport.LAV_COD = Agenda.Lav_Cod  " & vbCrLf)
            StbSQL.Append(" INNER JOIN movimenti_dettagli ON movimenti_dettagli.PIVA = movimenti.piva AND movimenti_dettagli.sa_cod = movimenti.sa_cod " & vbCrLf)
            StbSQL.Append(" AND movimenti_dettagli.Id_Agenda = movimenti.id_agenda AND movimenti_dettagli.Id_MOV = movimenti.id_mov " & vbCrLf)
            StbSQL.Append(" INNER JOIN materie_prime ON movimenti_dettagli.elem_cod = materie_prime.elem_cod  " & vbCrLf)
            StbSQL.Append(" AND movimenti_dettagli.mat_cod = materie_prime.mat_cod  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_PrimexReport   " & vbCrLf)
            StbSQL.Append(" ON materie_prime.mat_cod  = Materie_PrimexReport.MAT_COD  " & vbCrLf)
            StbSQL.Append(" AND Materie_PrimexReport.pro_cod =0  " & vbCrLf)
            StbSQL.Append("               " & vbCrLf)
            StbSQL.Append(" WHERE 1 = 1 " & vbCrLf)
            StbSQL.Append(" AND Jolly_Int = " & Agro_SQL_SaveNum(enum_TipoMovimentazioneMagazzino.MagazzinoNONMovimentato) & "  " & vbCrLf)
            StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StbSQL.Append(" AND movimenti_dettagli.elem_cod NOT IN ( " & Agro_SQL_SaveNum(ALTRI_BENI) & ", " & Agro_SQL_SaveNum(SERVIZI) & " ) " & vbCrLf)

            StbSQL.Append(" AND LEFT(agenda.des_lib,4) NOT IN ('GIIN', 'USSD', 'CASD') " & vbCrLf)

            StbSQL.Append(" AND    Agenda.Lav_Cod IN ( " & Agro_SQL_SaveNum(LAVCOD_DOCO_EMESSO) & ", " &
                                                        Agro_SQL_SaveNum(LAVCOD_DOCO_RICEVUTO) & ", " &
                                                        Agro_SQL_SaveNum(LAVCOD_MVV_EMESSO) & ", " &
                                                        Agro_SQL_SaveNum(LAVCOD_MVV_RICEVUTO) & ", " &
                                                        Agro_SQL_SaveNum(LAVCOD_CARICO) & ", " &
                                                        Agro_SQL_SaveNum(LAVCOD_SCARICO) & ", " &
                                                        Agro_SQL_SaveNum(LAVCOD_TRASFERIMENTO) & ", " &
                                                        Agro_SQL_SaveNum(LAVCOD_AUTOCONSUMO) & ", " &
                                                        Agro_SQL_SaveNum(LAVCOD_VENDITA) & ", " &
                                                        Agro_SQL_SaveNum(LAVCOD_ACQUISTO) & ", " &
                                                        Agro_SQL_SaveNum(LAVCOD_ALTRI_RICAVI) & ", " &
                                                        Agro_SQL_SaveNum(LAVCOD_ALTRI_COSTI) & ", " &
                                                        Agro_SQL_SaveNum(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                                        Agro_SQL_SaveNum(LAVCOD_BOLLA_RICEVUTA) & ", " &
                                                        Agro_SQL_SaveNum(LAVCOD_BOLLA_EMESSA) & ", " &
                                                        Agro_SQL_SaveNum(LAVCOD_RICEVUTA_EMESSA) &
                                                        "  ) " & vbCrLf)

            'al momento non considero fatture e note di accredito che andrebbero gestite più nel dettaglio 
            '(se fattura allegata a bolla oppure se nota di accredito abbuono)

            StbSQL.Append(" ORDER BY Data_Movimento DESC  " & vbCrLf)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Str_Query = StbSQL.ToString

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    '###################################################################################
    Public Function Leggi_AltreMateriePrime_SenzaConfigProprieta(ByVal Piva As String,
                                                                ByRef Str_Query As String,
                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.RegistriCantina.Leggi_AltreMateriePrime_SenzaConfigProprieta"
        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Str_Query = ""

        Try

            StbSQL.Length = 0
            StbSQL.Append(" -- elenco lotti altre materie prime " & vbCrLf)
            StbSQL.Append(" -- che non sono configurati nelel proprieta " & vbCrLf)
            StbSQL.Append(" SELECT DISTINCT movimenti_dettagli.elem_cod, movimenti_dettagli.mat_cod, mat_des ,lotto " & vbCrLf)
            'StbSQL.Append(" FROM movimenti  " & vbCrLf)
            'StbSQL.Append(" INNER JOIN Agenda  " & vbCrLf)
            'StbSQL.Append(" ON Agenda.PIVA = movimenti.piva  " & vbCrLf)
            'StbSQL.Append(" AND Agenda.Id_Agenda = movimenti.id_agenda  " & vbCrLf)
            StbSQL.Append(" FROM  movimenti_dettagli  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Agenda  " & vbCrLf)
            StbSQL.Append(" ON Agenda.PIVA = movimenti_dettagli.piva  " & vbCrLf)
            StbSQL.Append(" AND Agenda.Id_Agenda = movimenti_dettagli.id_agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN materie_prime ON movimenti_dettagli.elem_cod = materie_prime.elem_cod  " & vbCrLf)
            StbSQL.Append(" AND movimenti_dettagli.mat_cod = materie_prime.mat_cod  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_PrimexReport   " & vbCrLf)
            StbSQL.Append(" ON materie_prime.mat_cod  = Materie_PrimexReport.MAT_COD  " & vbCrLf)
            StbSQL.Append(" AND Materie_PrimexReport.pro_cod =0  " & vbCrLf)

            StbSQL.Append(" WHERE 1 = 1 " & vbCrLf)
            'escludo i record statici delle giacenze
            StbSQL.Append(" AND Agenda.lav_cod > 0 " & vbCrLf)
            StbSQL.Append(" AND NOT	EXISTS ( " & vbCrLf)
            StbSQL.Append("                  SELECT 1 " & vbCrLf)
            StbSQL.Append("                  FROM Materie_PrimexLotto_Proprieta " & vbCrLf)
            StbSQL.Append("                  where piva_superuser='" & objParametri.PivaSuperUser & "' " & vbCrLf)
            StbSQL.Append("                  and piva= movimenti_dettagli.PIVA " & vbCrLf)
            StbSQL.Append("                  and  id_proprieta = " & CStr(enum_TipoProprieta_Lotto.Categoria_Omni) & "  " & vbCrLf)
            StbSQL.Append("                  and lotto_val1 = movimenti_dettagli.lotto " & vbCrLf)
            StbSQL.Append("                  and elem_cod = movimenti_dettagli.elem_cod  " & vbCrLf)
            StbSQL.Append("                  and mat_cod = movimenti_dettagli.mat_cod " & vbCrLf)
            '                                   nno serve al momento filtrare lotto_cod1 (si imposta solo nome/lotto)
            'stbQ.Append("                      and lotto_cod1 = 16 " & vbCrLf)
            StbSQL.Append("                  ) -- NOT EXISTS" & vbCrLf)
            'StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " + vbCrLf)
            StbSQL.Append(" AND movimenti_dettagli.elem_cod = " & Agro_SQL_SaveNum(ALTRE_MATERIE) & vbCrLf)


            'al momento non considero fatture e note di accredito che andrebbero gestite più nel dettaglio 
            '(se fattura allegata a bolla oppure se nota di accredito abbuono)

            StbSQL.Append(" ORDER BY mat_des, lotto  " & vbCrLf)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Str_Query = StbSQL.ToString

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function



    '###################################################################################
    Public Function Leggi_Ora_31_12_2100_OperazionixReport(ByVal Piva As String,
                                                            ByRef Str_Query As String,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.RegistriCantina.Leggi_Ora_31_12_2100_OperazionixReport"
        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Str_Query = ""

        Try

            StbSQL.Length = 0
            StbSQL.Append(" -- elenco record di movimenti che hanno 31/12/2100 o 01/01/1900 nel campo ORA (quello considerato nei registri di cantina)              " & vbCrLf)
            StbSQL.Append(" -- o che hanno il campo ora futuro o < della data del movimento (quello considerato nei registri di cantina)              " & vbCrLf)
            StbSQL.Append(" -- che sono di operazioni che vanno nei registri di cantina (OperazionixReport)               " & vbCrLf)
            StbSQL.Append(" -- che movimentano il magazzino (Jolly_Int=0)               " & vbCrLf)
            StbSQL.Append(" -- e che contengono prodotti che vanno nei registri di cantina (Materie_PrimexReport)           " & vbCrLf)
            StbSQL.Append("               " & vbCrLf)
            StbSQL.Append(" -- SERVE IL DISTINCT PERCHE' I RECORD DI AGENDA SONO MOLTIPLICATI PER GLI ID_REPORT di operazionixreport e per CAU_MOV (id_mov, movimento) " & vbCrLf)
            StbSQL.Append(" SELECT DISTINCT des_lib, agenda.lav_cod, data_movimento, ora, agenda.data_creazione, Agenda.id_agenda  " & vbCrLf)
            StbSQL.Append(" FROM movimenti  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Agenda  " & vbCrLf)
            StbSQL.Append(" ON Agenda.PIVA = movimenti.piva  " & vbCrLf)
            StbSQL.Append(" AND Agenda.Id_Agenda = movimenti.id_agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN OperazionixReport  " & vbCrLf)
            StbSQL.Append(" ON operazionixreport.LAV_COD = Agenda.Lav_Cod  " & vbCrLf)
            StbSQL.Append(" WHERE (        ( CONVERT(date,ora) = '" & CStr(AGRODATAFINE) & "')   " & vbCrLf)
            StbSQL.Append("             OR ( CONVERT(date,ora) = '" & CStr(AGRODATAINIZIO) & "')   " & vbCrLf)
            StbSQL.Append("             OR ( CONVERT(date,ora) > '" & CStr(Date.Today) & "')   " & vbCrLf)
            StbSQL.Append("             OR ( CONVERT(date,ora) < Movimenti.data_movimento )   " & vbCrLf)
            StbSQL.Append("       )  " & vbCrLf)
            StbSQL.Append(" AND EXISTS (  " & vbCrLf)
            StbSQL.Append("             SELECT 1  " & vbCrLf)
            StbSQL.Append("             FROM movimenti_dettagli  " & vbCrLf)
            StbSQL.Append("             INNER JOIN Materie_PrimexReport   " & vbCrLf)
            StbSQL.Append("             ON movimenti_dettagli.pro_cod = Materie_PrimexReport.pro_cod  " & vbCrLf)
            StbSQL.Append("             AND movimenti_dettagli.mat_cod = Materie_PrimexReport.mat_cod  " & vbCrLf)
            StbSQL.Append("             WHERE movimenti_dettagli.PIVA = movimenti.piva  " & vbCrLf)
            StbSQL.Append("             AND movimenti_dettagli.Id_Agenda = movimenti.id_agenda  " & vbCrLf)
            StbSQL.Append("             AND Jolly_Int= " & Agro_SQL_SaveNum(enum_TipoMovimentazioneMagazzino.MagazzinoMovimentato) & "  " & vbCrLf)
            StbSQL.Append("             )  " & vbCrLf)
            StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StbSQL.Append(" ORDER BY Data_Movimento DESC  " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Str_Query = StbSQL.ToString

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    '##################################################################
    Public Function LeggiDoppioniLotto(
                            ByVal Piva As String,
                            ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.RegistriCantina.LeggiDoppioniLotto()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '------------------------------------------------------------------

            'If Piva = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            'End If

            '------------------------------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT piva, trasformazione_des ")
            StrSQL.Append(" FROM  Trasformazioni ")
            'escludo i lotti stringa vuoti creati con i condizionamenti
            StrSQL.Append(" WHERE trasformazione_des <> ''  ")

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            StrSQL.Append(" GROUP BY  piva, trasformazione_des ")
            StrSQL.Append(" HAVING COUNT(*)>1 ")


            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Piva, trasformazione_des ")
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

    '###################################################################################
    Public Function Leggi_LavCodAgenda_OperazionixReport(ByVal Piva As String,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.RegistriCantina.Leggi_LavCodAgenda_OperazionixReport"
        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.Length = 0
            StbSQL.Append(" -- elenco delle op. di agenda che non hanno il lav_cod in operazionixreport  " & vbCrLf)
            StbSQL.Append(" -- vengono filtrate quelle che non devono esserci (doc che non movimentano magazzino, op colturali, ecc)  " & vbCrLf)
            StbSQL.Append(" SELECT DISTINCT agenda.lav_cod, lav_des  " & vbCrLf)
            StbSQL.Append(" FROM agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN operazioni on operazioni.LAV_COD = agenda.lav_cod  " & vbCrLf)
            StbSQL.Append(" WHERE agenda.lav_cod NOT IN (  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_TRASFORMAZIONI) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_DANNI_RACCOLTA) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_RILIEVO_INDICI_MATURITA) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_ACQUISTO_BENI) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_MOV_FINANZIARIO) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_FATTURA_PROFORMA) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_TRASFERIMENTO) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_FATTURA_PROFESSIONISTI) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_ALTRI_RICAVI) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_ALTRI_COSTI) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_REG_COMPENSI) & " , " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_PREVENTIVO_VENDITA) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_ORDINE_VENDITA) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_ORDINE_ACQUISTO) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_DAA_EMESSO) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_NOTE) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_MANUTENZIONE_MACCHINE) & ",  " & vbCrLf)
            StbSQL.Append("                             " & Agro_SQL_SaveNum(LAVCOD_REVISIONE_MACCHINE) & "  " & vbCrLf)
            'StbSQL.Append("                             " & Agro_SQL_SaveNum() & "  " & vbCrLf)
            StbSQL.Append("                             )  " & vbCrLf)
            StbSQL.Append(" AND NOT EXISTS  " & vbCrLf)
            StbSQL.Append("     (SELECT 1  " & vbCrLf)
            StbSQL.Append("     FROM operazionixreport " & vbCrLf)
            StbSQL.Append("     WHERE operazionixreport.Lav_Cod=Agenda.lav_cod " & vbCrLf)
            StbSQL.Append("     )" & vbCrLf)
            StbSQL.Append(" AND NOT EXISTS (  " & vbCrLf)
            StbSQL.Append("     SELECT 1  " & vbCrLf)
            StbSQL.Append("     from Movimenti " & vbCrLf)
            StbSQL.Append("     where Cau_Mov in ('" & CAU_LAVORAZIONE & "','" & CAU_TRATTAMENTO & "','" & CAU_RILIEVO_CAMPO & "') " & vbCrLf)
            StbSQL.Append("     and agenda.piva=Movimenti.piva " & vbCrLf)
            StbSQL.Append("     and agenda.id_agenda=Movimenti.id_agenda " & vbCrLf)
            StbSQL.Append("     ) " & vbCrLf)
            StbSQL.Append("   " & vbCrLf)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function

    '###################################################################################
    Public Function Leggi_Operazioni_Pianificate(ByVal Piva As String,
                                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.RegistriCantina.Leggi_Operazioni_Pianificate"
        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.Length = 0
            StbSQL.Append(" -- elenco operazioni pianificate (configurate per registri di cantina)   " & vbCrLf)
            StbSQL.Append(" -- che NON movimentano il magazzino (contabilizzato < 0 )               " & vbCrLf)
            StbSQL.Append(" -- e che contengono prodotti che vanno nei registri di cantina (Materie_PrimexReport)           " & vbCrLf)
            StbSQL.Append("               " & vbCrLf)
            StbSQL.Append(" -- SERVE IL DISTINCT PERCHE' I RECORD DI AGENDA SONO MOLTIPLICATI PER CAU_MOV (id_mov, movimento) " & vbCrLf)
            StbSQL.Append(" SELECT DISTINCT des_lib, agenda.lav_cod, data_movimento, ora, agenda.data_creazione, Agenda.id_agenda  " & vbCrLf)
            StbSQL.Append(" FROM movimenti  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Agenda  " & vbCrLf)
            StbSQL.Append(" ON Agenda.PIVA = movimenti.piva  " & vbCrLf)
            StbSQL.Append(" AND Agenda.Id_Agenda = movimenti.id_agenda  " & vbCrLf)
            StbSQL.Append(" WHERE 1 = 1   " & vbCrLf)
            StbSQL.Append(" AND EXISTS (  " & vbCrLf)
            StbSQL.Append("             SELECT 1  " & vbCrLf)
            StbSQL.Append("             FROM movimenti_dettagli  " & vbCrLf)
            StbSQL.Append("             INNER JOIN Materie_PrimexReport   " & vbCrLf)
            StbSQL.Append("             ON movimenti_dettagli.pro_cod = Materie_PrimexReport.pro_cod  " & vbCrLf)
            StbSQL.Append("             AND movimenti_dettagli.mat_cod = Materie_PrimexReport.mat_cod  " & vbCrLf)
            StbSQL.Append("             WHERE movimenti_dettagli.PIVA = movimenti.piva  " & vbCrLf)
            StbSQL.Append("             AND movimenti_dettagli.Id_Agenda = movimenti.id_agenda  " & vbCrLf)
            StbSQL.Append("             AND movimenti_dettagli.Contabilizzato < 0   " & vbCrLf)
            StbSQL.Append("             )  " & vbCrLf)
            StbSQL.Append(" AND (lav_cod = " & CStr(LAVCOD_TRASFORMAZIONI) & "  " & vbCrLf)
            StbSQL.Append("     OR " & vbCrLf)
            StbSQL.Append("     EXISTS (SELECT 1 " & vbCrLf)
            StbSQL.Append("             FROM OperazionixReport  " & vbCrLf)
            StbSQL.Append("             WHERE operazionixreport.LAV_COD = Agenda.Lav_Cod  " & vbCrLf)
            StbSQL.Append("             ) " & vbCrLf)
            StbSQL.Append("     ) " & vbCrLf)
            StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StbSQL.Append(" ORDER BY Data_Movimento DESC  " & vbCrLf)
            StbSQL.Append("   " & vbCrLf)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    '###################################################################################
    Public Function Leggi_RilevamentiVinoSfuso_GiaACommercializzazione(ByVal Piva As String,
                                                                        ByRef Str_Query As String,
                                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.RegistriCantina.Leggi_RilevamentiVinoSfuso_GiaACommercializzazione"
        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Str_Query = ""

        Try

            StbSQL.Length = 0
            StbSQL.Append(" -- Verifica Lotti settati a Commercializzazione (ChkPassaggio=1) dove Passaggio_Data è >= campo ORA di Movimenti" & vbCrLf)
            StbSQL.Append(" -- (non va bene, deve essere antecedente di qualche minuto, altrimenti il rilevamento viene stampato nel registro di vinificazione. " & vbCrLf)
            StbSQL.Append(" " & vbCrLf)
            StbSQL.Append(" SELECT Movimenti.Ora, Agenda.piva, Agenda.Id_Agenda, Agenda.Des_Lib, Trasformazioni.id_trasformazione, Trasformazioni.Passaggio_Data, Agenda.data_creazione  " & vbCrLf)
            StbSQL.Append(" FROM Trasformazioni  " & vbCrLf)
            StbSQL.Append(" inner join Agenda on Agenda.piva = Trasformazioni.piva " & vbCrLf)
            StbSQL.Append(" AND Agenda.id_trasformazione =Trasformazioni.id_trasformazione " & vbCrLf)
            StbSQL.Append(" AND Agenda.id_agenda =Trasformazioni.Passaggio_Id_Agenda " & vbCrLf)
            StbSQL.Append(" inner join Movimenti on Agenda.piva = Movimenti.piva " & vbCrLf)
            StbSQL.Append(" AND Agenda.id_agenda =Movimenti.Id_Agenda " & vbCrLf)
            StbSQL.Append(" inner join Linee_Preparazioni ON Agenda.piva = Linee_Preparazioni.piva " & vbCrLf)
            StbSQL.Append(" AND Agenda.preparazione_cod =Linee_Preparazioni.preparazione_cod " & vbCrLf)
            StbSQL.Append(" WHERE Linee_Preparazioni.Codice_Generazione= " & enum_Omni_Preparazione_Cod.RilevamentoProdottiSfusiVasca & " " & vbCrLf)
            StbSQL.Append(" AND ChkPassaggio = 1 " & vbCrLf)
            StbSQL.Append(" AND movimenti.Cau_Mov = '" & CAU_CARICO & "' " & vbCrLf)
            StbSQL.Append(" AND Movimenti.Ora <= Trasformazioni.Passaggio_Data " & vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Str_Query = StbSQL.ToString

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT


    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' registro di vinificazione: 
    ''' pre query per ricavare gli id_agenda da dare in pasto alla query generale
    ''' (filtra in base alla categoria docg,dop, ecc)
    ''' 
    ''' questa query è una copia della query dei movimenti del registro di vinificazione
    ''' modificata con eliminazione di quello che non serviva
    ''' ed aggiunto dei filtri per filtrare la categoria del vino
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Sub PreFiltro_RegistroVinificazione_xCategoria(ByVal Piva As String,
                                                                ByVal Id_Report As Integer,
                                                                ByVal DataReportInizio As Date,
                                                                ByVal DataReportFine As Date,
                                                                ByVal flag_docg As Boolean,
                                                                ByVal flag_dop As Boolean,
                                                                ByVal flag_igp As Boolean,
                                                                ByVal flag_tavola As Boolean,
                                                                ByRef Str_IdAgenda As String,
                                                                ByRef Str_MatCod As String,
                                                                ByRef Str_Lotto As String,
                                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                ) ' As String

        'ByVal Cod_Contatto_Terzi As String, _
        'ByVal Lista_CodRisUm As String, _
        'ByVal Lista_PrepCod As String, _
        'ByVal Lista_IdTrasf_NoComm As String, _
        'ByVal Sa_Cod As Integer, _
        'ByVal Id_Destinazione As Integer, _
        'ByVal Cal_Cod As Integer, _
        'ByVal Mat_Cod As Integer, _
        'ByVal Linea_Cod As Integer, _
        'ByVal Cau_Mov As String, _
        'ByVal Gestione_Conto_Terzi As enum_RegistroContoTerzi, _


        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.RegistriCantina.PreFiltro_RegistroVinificazione_xCategoria"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim stbQ As New StringBuilder

        Dim stbQPrep_Select As New StringBuilder
        Dim stbQPrep_Join As New StringBuilder
        Dim stbQPrep_Where As New StringBuilder

        Dim stbQAgenda_Select As New StringBuilder
        Dim stbQAgenda_Join As New StringBuilder
        Dim stbQAgenda_Where As New StringBuilder


        Dim stbQMCR_Where As New StringBuilder


        Try

            Dim DataFineControllo As Date
            DataFineControllo = DateAdd(DateInterval.Day, 1, DataReportFine)

            Dim Str_categorie_log As String = ""
            Dim filtrosql_proprieta As String = ""

            If flag_docg Then
                Str_categorie_log &= CStr(enum_Omni_Gradi_Liberta.GL2_Docg) & ","
                filtrosql_proprieta &= "            AND  proprieta_val like '" & CStr(enum_Omni_Gradi_Liberta.GL2_Docg) & "|%'" & vbCrLf
            End If
            If flag_dop Then
                Str_categorie_log &= CStr(enum_Omni_Gradi_Liberta.GL2_Dop) & ","
                filtrosql_proprieta &= "            AND  proprieta_val like '" & CStr(enum_Omni_Gradi_Liberta.GL2_Dop) & "|%'" & vbCrLf
            End If
            If flag_igp Then
                Str_categorie_log &= CStr(enum_Omni_Gradi_Liberta.GL2_Igp) & ","
                filtrosql_proprieta &= "            AND  proprieta_val like '" & CStr(enum_Omni_Gradi_Liberta.GL2_Igp) & "|%'" & vbCrLf
            End If
            If flag_tavola Then
                Str_categorie_log &= CStr(enum_Omni_Gradi_Liberta.GL2_Tavola) & ","
                filtrosql_proprieta &= "            AND  proprieta_val like '" & CStr(enum_Omni_Gradi_Liberta.GL2_Tavola) & "|%'" & vbCrLf
            End If
            Str_categorie_log = Mid(Str_categorie_log, 1, Str_categorie_log.Length - 1)

            If filtrosql_proprieta = "" Then
                'non deve succedere, se sono in questa funzione è perché ho scelto di fare il filtro della categoria
                Throw New Exception("filtro proprieta vuoto")
            End If

            If Str_categorie_log = "" Then
                'non deve succedere, se sono in questa funzione è perché ho scelto di fare il filtro della categoria
                Throw New Exception("filtro categorie vuoto")
            End If



            '==============================================================================================
            '---- PARTE DI QUERY PER LA SEZIONE PREPARAZIONI ------------------
            '==============================================================================================

            stbQPrep_Select.Append(" SELECT DISTINCT Agenda.Id_Agenda, Agenda.des_lib, Materie_Prime.mat_cod, mat_des, Movimenti_dettagli.Lotto " & vbCrLf)
            'stbQPrep_Select.Append(" , Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Mat_Cod, " + vbCrLf)
            'stbQPrep_Select.Append(" Movimenti_dettagli.Lotto,  " + vbCrLf)
            'stbQPrep_Select.Append("  Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo,  " + vbCrLf)

            stbQPrep_Join.Append(" FROM Linee_PreparazionixReport " & vbCrLf)
            stbQPrep_Join.Append(" INNER JOIN Linee_Preparazioni ON Linee_PreparazionixReport.Piva = Linee_Preparazioni.Piva AND Linee_PreparazionixReport.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod  " & vbCrLf)
            stbQPrep_Join.Append(" INNER JOIN Agenda ON Linee_Preparazioni.Preparazione_Cod = Agenda.PREPARAZIONE_COD AND Linee_Preparazioni.Piva = Agenda.PIVA " & vbCrLf)
            'stbQPrep_Join.Append(" INNER JOIN Trasformazioni ON Trasformazioni.ID_trasformazione=Agenda.ID_Trasformazione AND  Trasformazioni.PIVA=Agenda.PIVA " + vbCrLf)
            'stbQPrep_Join.Append(" INNER JOIN Linee_Produzioni ON Agenda.Piva = Linee_Produzioni.Piva AND Agenda.Linea_Cod = Linee_Produzioni.Linea_Cod " + vbCrLf)

            stbQPrep_Join.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod  " & vbCrLf)
            stbQPrep_Join.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov  " & vbCrLf)
            stbQPrep_Join.Append(" INNER JOIN Materie_Prime ON Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod AND Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod " & vbCrLf)
            'stbQPrep_Join.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Movimenti_dettagli.Udm_Cod " + vbCrLf)

            ''inserito il 15/10/2012 per stampare il numero di vasca
            'stbQPrep_Join.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det " + vbCrLf)

            'nota del 17/06/2014: siam d'accordo con Marco di non introdurre il join su AND Materie_Prime_ParametriQualitativi.PIVA = Materie_Prime.piva
            'non ci devono essere record duplicati per lo stesso mat_cod -> introduciamo una query di delete di sicurezza
            stbQPrep_Join.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod  " & vbCrLf)
            stbQPrep_Join.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " & vbCrLf)

            stbQPrep_Join.Append(" INNER JOIN Materie_PrimexReport ON Materie_PrimexReport.Mat_Cod = Materie_Prime.Mat_Cod   " & vbCrLf)
            stbQPrep_Join.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON Movimenti_Dettagli.Mat_Cod = OGenerazioni_Anagrafe_Log.Mat_Cod AND Movimenti_Dettagli.elem_Cod = OGenerazioni_Anagrafe_Log.elem_Cod " & vbCrLf)

            stbQPrep_Where.Append(" WHERE Linee_PreparazionixReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)

            stbQPrep_Where.Append(" AND Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                                      "'" & CAU_SCARICO & "', " &
                                                      "'" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', " &
                                                      "'" & CAU_CONFERIMENTO & "', " &
                                                      "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                                            ") " & vbCrLf)

            stbQPrep_Where.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            stbQPrep_Where.Append(" AND OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQPrep_Where.Append(" AND OGenerazioni_Anagrafe_Log.Categoria_Gias_Cod_Log IN (" & Agro_SQL_Save_Clausola_IN(Str_categorie_log) & ") " & vbCrLf)

            ' spostato nel join direttamente sulla 1a
            'stbQPrep_Where.Append(" AND Linee_Produzioni.Categoria_Gias_Cod IN (" & Agro_SQL_SaveText(Str_categorie_log) & ") " + vbCrLf)

            stbQPrep_Where.Append(" AND Materie_PrimexReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)

            '--------------
            'devo cercare da agrodatainizio, 
            'altrimenti nel riepilogo non vengono visualizzate le anagrafiche NOn movimentate nel mese selezioanto
            stbQPrep_Where.Append(" AND Movimenti.Ora >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " & vbCrLf)
            stbQPrep_Where.Append(" AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " " & vbCrLf)

            '--------------
            'aggiunto in data 23/09/2013
            stbQPrep_Where.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            stbQPrep_Where.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            stbQPrep_Where.Append(" AND Materie_Prime_Calibri.Piva_SuperUser = '" & objParametri.PivaSuperUser & "' " & vbCrLf)
            stbQPrep_Where.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = 'calibro' " & vbCrLf)
            stbQPrep_Where.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri_Vinificazione = 1 " & vbCrLf)

            'If Sa_Cod <> 0 Then
            '    stbQPrep_Where.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " + vbCrLf)
            'End If
            'If Id_Destinazione <> 0 Then
            '    stbQPrep_Where.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " + vbCrLf)
            'End If
            'If Cal_Cod <> 0 Then
            '    stbQPrep_Where.Append(" AND Materie_Prime_ParametriQualitativi.tipo_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " + vbCrLf)
            'End If
            'If Mat_Cod <> 0 Then
            '    stbQPrep_Where.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " + vbCrLf)
            'End If
            'If Linea_Cod <> 0 Then
            '    stbQPrep_Where.Append(" AND Linee_Produzioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "  " + vbCrLf)
            'End If
            'If Cau_Mov <> "" Then
            '    stbQPrep_Where.Append(" AND Movimenti.CAU_MOV = '" & Agro_SQL_SaveText(Cau_Mov) & "'" & vbCrLf)
            'End If
            'correzione del 9/11/2010: mancavano i join con Movimenti_dettagli, per cui
            'se c'era almeno un dettaglio escluso per uno scarico, venivano esclusi tutti gli scarichi
            stbQPrep_Where.AppendLine(" AND NOT EXISTS (SELECT 1 FROM  Linee_Preparazioni_Report_Esclusi LPRE ")
            stbQPrep_Where.AppendLine("                 INNER JOIN Linee_Preparazioni_Dettagli LPD ON LPRE.Piva = LPD.Piva AND LPRE.Preparazione_Cod = LPD.Preparazione_Cod AND LPRE.Dettaglio_Cod = LPD.Dettaglio_Cod ")
            stbQPrep_Where.AppendLine("                 WHERE LPRE.Piva = Linee_Preparazioni.Piva ")
            stbQPrep_Where.AppendLine("                 AND LPRE.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod ")
            stbQPrep_Where.AppendLine("                 AND LPRE.Id_Report = Linee_PreparazionixReport.Id_Report  ")
            stbQPrep_Where.AppendLine("                 AND LPD.CAU_MOV = Movimenti.CAU_MOV ")
            stbQPrep_Where.AppendLine("                 AND LPD.Elem_Cod = Movimenti_dettagli.Elem_Cod ")
            'modifica del 05/10/2012: se la preparazione passaggio da registro di vinificazione a registro di commercializzazione
            'utilizzava 'risorse indefinita' come ingrediente e/o preparato
            'il join con Movimenti_dettagli non produceva alcun record e quindi venivano visualizzati entrambi i movimenti:
            'sia lo scarico dal reg vinificazione sia il carico al reg di commerc. e la qta si azzerava
            'INTRODOTTO L'OR per LPD.Pro_Cod - LPD.Mat_Cod - LPD.Udm_Cod 
            stbQPrep_Where.AppendLine("               AND ( ")
            stbQPrep_Where.AppendLine("                       ( ")
            stbQPrep_Where.AppendLine("                       LPD.Pro_Cod = Movimenti_dettagli.Pro_Cod ")
            stbQPrep_Where.AppendLine("                       AND LPD.Mat_Cod = Movimenti_dettagli.Mat_Cod ")
            stbQPrep_Where.AppendLine("                       AND LPD.Udm_Cod = Movimenti_dettagli.Udm_Cod   ")
            stbQPrep_Where.AppendLine("                       ) OR ( ")
            stbQPrep_Where.AppendLine("                       LPD.Pro_Cod = 0 AND LPD.Mat_Cod = 0 AND LPD.Udm_Cod = 0  ")
            stbQPrep_Where.AppendLine("                       ) ")
            stbQPrep_Where.AppendLine("                   ) ")
            stbQPrep_Where.AppendLine("               ) ")


            '==============================================================================================
            '---- PARTE DI QUERY PER LA SEZIONE AGENDA ------------------
            '==============================================================================================

            stbQAgenda_Select.Append(" SELECT DISTINCT Agenda.Id_Agenda, Agenda.des_lib, Materie_Prime.mat_cod, mat_des, Movimenti_dettagli.Lotto  " & vbCrLf)

            stbQAgenda_Join.Append(" FROM  Materie_PrimexReport " & vbCrLf)
            stbQAgenda_Join.Append(" INNER JOIN Materie_Prime ON Materie_PrimexReport.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)

            stbQAgenda_Join.Append(" INNER JOIN Movimenti_dettagli ON Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod " & vbCrLf)
            stbQAgenda_Join.Append(" INNER JOIN Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov " & vbCrLf)
            stbQAgenda_Join.Append(" INNER JOIN Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND Movimenti.Id_Agenda = Agenda.Id_Agenda " & vbCrLf)

            'nota del 17/06/2014: siam d'accordo con Marco di non introdurre il join su AND Materie_Prime_ParametriQualitativi.PIVA = Materie_Prime.piva
            'non ci devono essere record duplicati per lo stesso mat_cod -> introduciamo una query di delete di sicurezza
            stbQAgenda_Join.Append(" INNER JOIN Materie_Prime_ParametriQualitativi ON Materie_Prime_ParametriQualitativi.Mat_Cod = Materie_Prime.Mat_Cod  " & vbCrLf)
            stbQAgenda_Join.Append(" INNER JOIN Materie_Prime_Calibri ON Materie_Prime_ParametriQualitativi.Tipo_Cod = Materie_Prime_Calibri.Cal_Cod " & vbCrLf)

            'OperazionixReport è quella del superuser,
            'non va quindi messa in join con la piva di agenda!
            stbQAgenda_Join.Append(" INNER JOIN OperazionixReport OPR  " & vbCrLf)
            stbQAgenda_Join.Append("               ON OPR.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQAgenda_Join.Append("           AND OPR.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)
            stbQAgenda_Join.Append("           AND Agenda.Lav_Cod = OPR.Lav_Cod  " & vbCrLf)

            stbQAgenda_Join.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON Movimenti_Dettagli.Mat_Cod = OGenerazioni_Anagrafe_Log.Mat_Cod AND Movimenti_Dettagli.elem_Cod = OGenerazioni_Anagrafe_Log.elem_Cod " & vbCrLf)


            stbQAgenda_Where.Append(" WHERE Movimenti.CAU_MOV IN ('" & CAU_CARICO & "', " &
                                               "'" & CAU_SCARICO & "', " &
                                               "'" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "', " &
                                               "'" & CAU_CONFERIMENTO & "', " &
                                               "'" & CAU_CONFERIMENTO_DIVERSI & "' " &
                                               ") " & vbCrLf)

            stbQAgenda_Where.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            stbQAgenda_Where.Append(" AND OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            stbQAgenda_Where.Append(" AND OGenerazioni_Anagrafe_Log.Categoria_Gias_Cod_Log IN (" & Agro_SQL_Save_Clausola_IN(Str_categorie_log) & ") " & vbCrLf)

            'stbQ.Append(" AND Linee_Produzioni.Categoria_Gias_Cod IN (" & Agro_SQL_SaveText(Str_categorie_log) & ") " + vbCrLf)


            '--------------
            'altrimenti nel riepilogo non vengono visualizzate le anagrafiche NOn movimentate nel mese selezioanto
            stbQAgenda_Where.Append(" AND Movimenti.Ora >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " & vbCrLf)
            stbQAgenda_Where.Append(" AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " " & vbCrLf)

            stbQAgenda_Where.Append(" AND Materie_PrimexReport.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " & vbCrLf)

            '--------------
            stbQAgenda_Where.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)

            'aggiunta il 19/08/13:per escludere carichi/scarichi di operazioni pendenti (pianificate)
            stbQAgenda_Where.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            stbQAgenda_Where.Append(" AND Materie_Prime_Calibri.Piva_SuperUser = '" & objParametri.PivaSuperUser & "' " & vbCrLf)
            stbQAgenda_Where.Append(" AND Materie_Prime_ParametriQualitativi.Tipo = 'calibro' " & vbCrLf)
            stbQAgenda_Where.Append(" AND Materie_Prime_ParametriQualitativi.ChkRegistri_Vinificazione = 1 " & vbCrLf)

            'If Sa_Cod <> 0 Then
            '    stbQ.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " + vbCrLf)
            'End If
            'If Id_Destinazione <> 0 Then
            '    stbQ.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "  " + vbCrLf)
            'End If
            'If Cal_Cod <> 0 Then
            '    stbQ.Append(" AND Materie_Prime_ParametriQualitativi.tipo_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "  " + vbCrLf)
            'End If
            'If Mat_Cod <> 0 Then
            '    stbQ.Append(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " + vbCrLf)
            'End If
            'If Linea_Cod <> 0 Then
            '    stbQ.Append("   AND  EXISTS (SELECT 1  " + vbCrLf)
            '    stbQ.Append("               FROM OGenerazioni_Anagrafe_Log " + vbCrLf)
            '    stbQ.Append(" 				WHERE materie_prime.mat_cod = OGenerazioni_Anagrafe_Log.mat_cod " + vbCrLf)
            '    stbQ.Append(" 				AND materie_prime.elem_cod = OGenerazioni_Anagrafe_Log.elem_cod " + vbCrLf)
            '    stbQ.Append("               AND OGenerazioni_Anagrafe_Log.linea_cod = " & Agro_SQL_SaveNum(Linea_Cod) & " " + vbCrLf)
            '    stbQ.Append("               ) " + vbCrLf)
            'End If
            'If Cau_Mov <> "" Then
            '    stbQ.Append(" AND Movimenti.CAU_MOV = '" & Agro_SQL_SaveText(Cau_Mov) & "'" & vbCrLf)
            'End If

            ''CORREZIONE DEL 07/05/2014: la piva di OperazionixReport è quella del superuser,
            ' 'non va quindi messa in join con la piva di agenda!
            ' 'è stato scoperto ora perché qualitoscana è il primo cliente col quale si stampano i registri di cantina sulle aziende figlie in gerarchia
            ' stbQ.Append(" AND EXISTS (SELECT 1 FROM OperazionixReport OPR  " + vbCrLf)
            ' stbQ.Append("             WHERE  OPR.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " + vbCrLf)
            ' stbQ.Append("             AND OPR.Id_Report = " & Agro_SQL_SaveNum(Id_Report) & "  " + vbCrLf)
            ' stbQ.Append("               AND Agenda.Lav_Cod = OPR.Lav_Cod ) " + vbCrLf)

            'stbQ.Append(" ORDER BY  Movimenti.Ora, Agenda.Id_Agenda, Movimenti.Cau_Mov DESC, Movimenti_dettagli.Elem_Cod DESC " + vbCrLf)


            '==============================================================================================
            '---- PARTE DI QUERY PER FILTRO ALTRE MATERIE ------------------
            '==============================================================================================
            stbQMCR_Where.Append(" AND ( " & vbCrLf)
            stbQMCR_Where.Append("      -- esiste la configurazione in lotto_proprieta per quel lotto " & vbCrLf)
            stbQMCR_Where.Append("      -- per la categoria di registro selezionata " & vbCrLf)
            stbQMCR_Where.Append("      EXISTS ( " & vbCrLf)
            stbQMCR_Where.Append("              SELECT 1 " & vbCrLf)
            stbQMCR_Where.Append("              FROM Materie_PrimexLotto_Proprieta " & vbCrLf)
            stbQMCR_Where.Append("              where piva_superuser = '" & objParametri.PivaSuperUser & "' " & vbCrLf)
            stbQMCR_Where.Append("              and piva = Agenda.PIVA " & vbCrLf)
            stbQMCR_Where.Append("              and id_proprieta = " & CStr(enum_TipoProprieta_Lotto.Categoria_Omni) & "  " & vbCrLf)
            stbQMCR_Where.Append("              and lotto_val1 = movimenti_dettagli.lotto " & vbCrLf)
            stbQMCR_Where.Append("              and elem_cod = movimenti_dettagli.elem_cod  " & vbCrLf)
            stbQMCR_Where.Append("              and mat_cod = movimenti_dettagli.mat_cod " & vbCrLf)
            '                                   nno serve al momento filtrare lotto_cod1 (si imposta solo nome/lotto)
            'stbQMCR_Where.Append("              and lotto_cod1 = 16 " & vbCrLf)
            'stbQMCR_Where.Append("              and proprieta_val in (" & Str_categorie_log & ") " & vbCrLf)
            stbQMCR_Where.Append(filtrosql_proprieta & vbCrLf)
            stbQMCR_Where.Append("              ) -- EXISTS " & vbCrLf)
            stbQMCR_Where.Append("      OR " & vbCrLf)
            stbQMCR_Where.Append("      -- non c'è alcun record in lotto_proprieta per il tipo = 3 categoria " & vbCrLf)
            stbQMCR_Where.Append("      NOT	EXISTS ( " & vbCrLf)
            stbQMCR_Where.Append("                  SELECT 1 " & vbCrLf)
            stbQMCR_Where.Append("                  FROM Materie_PrimexLotto_Proprieta " & vbCrLf)
            stbQMCR_Where.Append("                  where piva_superuser='" & objParametri.PivaSuperUser & "' " & vbCrLf)
            stbQMCR_Where.Append("                  and piva= Agenda.PIVA " & vbCrLf)
            stbQMCR_Where.Append("                  and  id_proprieta = " & CStr(enum_TipoProprieta_Lotto.Categoria_Omni) & "  " & vbCrLf)
            stbQMCR_Where.Append("                  and lotto_val1 = movimenti_dettagli.lotto " & vbCrLf)
            stbQMCR_Where.Append("                  and elem_cod = movimenti_dettagli.elem_cod  " & vbCrLf)
            stbQMCR_Where.Append("                  and mat_cod = movimenti_dettagli.mat_cod " & vbCrLf)
            '                                   nno serve al momento filtrare lotto_cod1 (si imposta solo nome/lotto)
            'stbQMCR_Where.Append("              and lotto_cod1 = 16 " & vbCrLf)
            stbQMCR_Where.Append("                  ) -- NOT EXISTS" & vbCrLf)
            stbQMCR_Where.Append("      )-- AND PRINCIPALE DEL FILTRO LOTTO_PROPRIETA" & vbCrLf)
            'stbQMCR_Where.Append(" " & vbCrLf)
            'stbQMCR_Where.Append(" " & vbCrLf)
            'stbQMCR_Where.Append(" " & vbCrLf)



            '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
            '§§§§§§§§§§§§ generazione query §§§§§§§§§§§§§§§§§§§§§§§
            '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

            '==============================================================================================
            '---- 1a) PREPARAZIONI (tutte le anagrafiche tranne le altre materie prime) ------------------
            '==============================================================================================

            stbQ.Append("   " & vbCrLf)
            stbQ.Append(" -- 1a) PREPARAZIONI (tutte le anagrafiche tranne le altre materie prime)  " & vbCrLf)
            stbQ.Append("  " & vbCrLf)
            stbQ.Append(stbQPrep_Select.ToString & vbCrLf)
            stbQ.Append(stbQPrep_Join.ToString & vbCrLf)
            stbQ.Append(" INNER JOIN Trasformazioni ON  Trasformazioni.trasformazione_des= Movimenti_dettagli.Lotto AND Trasformazioni.piva= Movimenti_dettagli.piva  " & vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Produzioni ON Trasformazioni.piva= Linee_Produzioni.piva AND  Trasformazioni.linea_cod=Linee_Produzioni.linea_cod AND Linee_Produzioni.Categoria_Gias_Cod IN (" & Agro_SQL_Save_Clausola_IN(Str_categorie_log) & ") " & vbCrLf)
            stbQ.Append(stbQPrep_Where.ToString & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Tipo_Generazione <> " & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.AltreMateriePrime) & " " & vbCrLf)


            '==============================================================================================
            '---- 1b) PREPARAZIONI (solo per le altre materie prime) ------------------
            '==============================================================================================

            stbQ.Append("  " & vbCrLf)
            stbQ.Append(" UNION ALL " & vbCrLf)
            stbQ.Append("  " & vbCrLf)
            stbQ.Append(" -- 1b) PREPARAZIONI (solo per le altre materie prime) " & vbCrLf)
            stbQ.Append("  " & vbCrLf)
            stbQ.Append(stbQPrep_Select.ToString & vbCrLf)
            stbQ.Append(stbQPrep_Join.ToString & vbCrLf)
            stbQ.Append(stbQPrep_Where.ToString & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Tipo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.AltreMateriePrime) & " " & vbCrLf)
            stbQ.Append(stbQMCR_Where.ToString)


            '=========================================================
            '--------------- 2a) AGENDA (tutte le anagrafiche tranne le altre materie prime)  ------------------
            '=========================================================

            stbQ.Append("  " & vbCrLf)
            stbQ.Append(" UNION ALL " & vbCrLf)
            stbQ.Append("  " & vbCrLf)
            stbQ.Append(" -- 2a) AGENDA (tutte le anagrafiche tranne le altre materie prime)  " & vbCrLf)
            stbQ.Append("  " & vbCrLf)
            stbQ.Append(stbQAgenda_Select.ToString & vbCrLf)
            stbQ.Append(stbQAgenda_Join.ToString & vbCrLf)
            stbQ.Append(" INNER JOIN Trasformazioni ON  Trasformazioni.trasformazione_des= Movimenti_dettagli.Lotto AND Trasformazioni.piva= Movimenti_dettagli.piva  " & vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Produzioni ON Trasformazioni.piva= Linee_Produzioni.piva AND  Trasformazioni.linea_cod=Linee_Produzioni.linea_cod AND Linee_Produzioni.Categoria_Gias_Cod IN (" & Agro_SQL_Save_Clausola_IN(Str_categorie_log) & ") " & vbCrLf)
            stbQ.Append(stbQAgenda_Where.ToString & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Tipo_Generazione <> " & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.AltreMateriePrime) & " " & vbCrLf)


            '=========================================================
            '--------------- 2b) AGENDA ------------------
            '=========================================================

            stbQ.Append("  " & vbCrLf)
            stbQ.Append(" UNION ALL " & vbCrLf)
            stbQ.Append("  " & vbCrLf)
            stbQ.Append(" -- 2b) AGENDA (solo per le altre materie prime) " & vbCrLf)
            stbQ.Append("  " & vbCrLf)
            stbQ.Append(stbQAgenda_Select.ToString & vbCrLf)
            stbQ.Append(stbQAgenda_Join.ToString & vbCrLf)
            stbQ.Append(stbQAgenda_Where.ToString & vbCrLf)
            stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Tipo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.AltreMateriePrime) & " " & vbCrLf)
            stbQ.Append(stbQMCR_Where.ToString)


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stbQ.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Dim i As Integer
            Dim id_agenda As Integer
            Dim mat_cod As Integer
            Dim lotto As String
            Dim HT_IdAgenda As Hashtable
            Dim HT_Matcod As Hashtable
            Dim HT_Lotto As Hashtable

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                HT_IdAgenda = New Hashtable
                HT_Matcod = New Hashtable
                HT_Lotto = New Hashtable

                For i = 0 To DT.Rows.Count - 1

                    id_agenda = DT.Rows(i).Item("Id_Agenda")
                    mat_cod = DT.Rows(i).Item("mat_cod")
                    lotto = DT.Rows(i).Item("lotto")

                    'If i = DT.Rows.Count - 1 Then
                    '    'ultimo
                    '    Str_MatCod += CStr(mat_cod)
                    'Else
                    '    Str_MatCod += CStr(mat_cod) & ", "
                    'End If

                    If Not HT_IdAgenda.Contains(id_agenda) Then
                        HT_IdAgenda.Add(id_agenda, 0)
                    End If

                    If Not HT_Matcod.Contains(mat_cod) Then
                        HT_Matcod.Add(mat_cod, 0)
                    End If

                    If Not HT_Lotto.Contains(lotto) Then
                        HT_Lotto.Add(lotto, 0)
                    End If

                Next

                '  Str_MatCod = "( " & Str_MatCod & ") "

                Dim key As Object

                For Each key In HT_IdAgenda.Keys
                    Str_IdAgenda += CStr(key) & "," 'no spazio dopo la virgola!
                Next
                Str_IdAgenda = "( " & Mid(Str_IdAgenda, 1, Str_IdAgenda.Length - 1) & ") "

                For Each key In HT_Matcod.Keys
                    Str_MatCod += CStr(key) & "," 'no spazio dopo la virgola!
                Next
                Str_MatCod = "( " & Mid(Str_MatCod, 1, Str_MatCod.Length - 1) & ") "

                For Each key In HT_Lotto.Keys
                    Str_Lotto += "'" & CStr(key) & "'," 'no spazio dopo la virgola!
                Next
                Str_Lotto = "( " & Mid(Str_Lotto, 1, Str_Lotto.Length - 1) & ") "


            Else
                '-------------------------------------------------
                '----- la query non ha restituito valori-------
                ' devo impostare un filtro per fare in modo
                '-------------------------------------------------

            End If 'dt vuoto

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub






#Region "Gestione Cache nei registri di cantina"
    Public Function RegistroCacheGet_Cod(
                ByVal NomeTabella As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                ) As Integer

        Dim rVal As Integer
        Dim AgSeq As New AgronicaCoreDataProvider.Agro_Sequenze
        Try
            rVal = AgSeq.NuovoId_Tabella("GIAS_Registri_Commercializzazione_Testata", "", 1, 2000000000, objParametri.objConnessione, Nothing, objParametri.StringaConnessione, 0, "", "", "")
        Catch ex As Exception
            rVal = -1
        End Try

        Return rVal

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' memorizza in cache il riepilogo
    ''' </summary>
    ''' <param name="DS_comm">dataset di dettaglio</param>
    ''' <param name="DS_comm_riepilogo">dataset con il riepilogo</param>
    ''' <param name="Piva">Piva di appartenza del registro</param>
    ''' <param name="DataDa">Data Da</param>
    ''' <param name="DataA">Data A</param>
    ''' <param name="objParametri">Parametri in stile Gias</param>
    ''' <returns></returns>
    ''' <remarks>
    '''     ''' </remarks>
    ''' <history>
    ''' 	[costa]	03/10/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function CacheDati_Commercializzazione(ByVal DS_comm As DataSet,
                                                  ByVal DS_comm_riepilogo As DataSet,
                                                  ByVal Piva As String,
                                                  ByVal Descrizione As String,
                                                  ByVal sa_cod As Integer,
                                                  ByVal DataDa As Date,
                                                  ByVal DataA As Date,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As Integer

        Dim progressivoRegistro As Integer

        Dim CantinaWriteHelper As New RegistriCantina_Commercializzazione_W

        progressivoRegistro = RegistroCacheGet_Cod("GIAS_Registri_Commercializzazione_Testata", objParametri)
        If progressivoRegistro <> -1 Then

            'dati di testata
            CantinaWriteHelper.Scrivi(progressivoRegistro, Piva, sa_cod, Descrizione, DataDa, DataA, objParametri)

            'riversa i dati di dettaglio e dei riepiloghi
            'CacheDati_Commercializzazione(DS_comm, ProgressivoRegistro, Piva, objParametri)
            CacheDati_commercializzazione_UsaDAL(DS_comm, DS_comm_riepilogo, progressivoRegistro, Piva, objParametri)

        End If

    End Function

    Private Sub CacheDati_commercializzazione_UsaDAL(ByVal DS_comm As DataSet,
                                                     ByVal DS_comm_riepilogo As DataSet,
                                                     ByVal Reg_Comm_cod As Integer,
                                                     ByVal Piva As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     )

        Dim DalHlp As New GIAS_Registri_Commercializzazione_DatiAgenda_W
        Dim DalHlp_riepilogo As New GIAS_Registri_Commercializzazione_Riepilogo_W

        Dim tbl_comm As DataTable =
                    CacheDati_Commercializzazione_getDatatable(DS_comm, Reg_Comm_cod, Piva)

        Dim tbl_comm_riepilogo As DataTable =
                            CacheDati_Commercializzazione_getDatatable(DS_comm_riepilogo, Reg_Comm_cod, Piva)

        'dati riepilogo
        For Each dRow As DataRow In tbl_comm_riepilogo.Rows
            DalHlp_riepilogo.Scrivi(
               dRow("Reg_Comm_cod") _
            , dRow("Progressivo") _
            , Agro_SQL_SaveText(DBNull_toEmptyString(dRow("Linea"))) _
            , Agro_SQL_SaveText(DBNull_toEmptyString(dRow("Dettagli"))) _
            , Agro_SQL_SaveNum(DBNull_toZero(dRow("CaricoKg"))) _
            , Agro_SQL_SaveNum(DBNull_toZero(dRow("ScaricoKg"))) _
            , Agro_SQL_SaveNum(DBNull_toZero(dRow("CaricoLt"))) _
            , Agro_SQL_SaveNum(DBNull_toZero(dRow("ScaricoLt"))) _
            , Agro_SQL_SaveNum(DBNull_toZero(dRow("SaldoPrima"))) _
            , Agro_SQL_SaveNum(DBNull_toZero(dRow("SaldoDopo"))) _
            , DBNull_toEmptyString(dRow("DataFineRiepilogo")) _
            , objParametri
            )

        Next



        'dati dettaglio agenda
        For Each dRow As DataRow In tbl_comm.Rows
            DalHlp.Scrivi(
                     dRow("Reg_Comm_cod") _
                , Agro_SQL_SaveText(dRow("piva")) _
                , dRow("Id_Agenda") _
                , dRow("Data_Movimento") _
                , dRow("des_lib") _
                , Agro_SQL_SaveText(dRow("Descrizione")) _
                , Agro_SQL_SaveText(dRow("Designazione")) _
                , dRow("Cau_Mov") _
                , Agro_SQL_SaveText(dRow("Mov_Desc")) _
                , dRow("Elem_Cod") _
                , dRow("Mat_Cod") _
                , Agro_SQL_SaveText(dRow("Mov_Det_Des")) _
                , DBNull_toZero(dRow("CaricoKg")) _
                , dRow("Udm_Cod") _
                , Agro_SQL_SaveText(dRow("Udm_Sim")) _
                , Agro_SQL_SaveText(dRow("Mat_Des")) _
                , dRow("Cod_Articolo") _
                , DBNull_toZero(dRow("ScaricoKg")) _
                , dRow("NDoc") _
                , dRow("CaricoLt") _
                , dRow("ScaricoLt") _
                , Agro_SQL_SaveText(dRow("Lotto")) _
                , objParametri
                )
        Next

    End Sub

    Private Function DBNull_toEmptyString(ByVal valore As Object) As String
        If valore Is DBNull.Value Then
            Return ""
        Else
            Return CStr(valore)
        End If
    End Function

    Private Function DBNull_toZero(ByVal valore As Object) As String
        If valore Is DBNull.Value Then
            Return "0"
        Else
            Return valore
        End If
    End Function

    Private Function CacheDati_Commercializzazione_getDatatable(ByVal DS_Comm As DataSet, ByVal reg_Comm_cod As Integer, ByVal Piva As String) As DataTable
        Dim tbl_comm As DataTable = DS_Comm.Tables(0)

        tbl_comm.Columns.Add("Reg_Comm_cod", Type.GetType("System.Int32"))
        tbl_comm.Columns.Add("Piva", Type.GetType("System.String"))
        tbl_comm.Columns.Add("Progressivo", Type.GetType("System.Int32"))

        For i As Integer = 0 To tbl_comm.Rows.Count - 1
            tbl_comm.Rows(i).BeginEdit()
            tbl_comm.Rows(i)("Reg_Comm_cod") = reg_Comm_cod
            tbl_comm.Rows(i)("Piva") = Piva
            tbl_comm.Rows(i)("Progressivo") = i + 1
            tbl_comm.Rows(i).EndEdit()
        Next

        tbl_comm.AcceptChanges()
        Return tbl_comm
    End Function

    Private Sub CacheDati_Commercializzazione(ByVal DS_comm As DataSet,
                                              ByVal Reg_Comm_cod As Integer,
                                              ByVal Piva As String,
                                              ByRef objParametri As AgronicaCoreParametri)

        Dim oConn As DbConnection = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)

        Dim DA_comm As DbDataAdapter = DataProviderFactory.Instance.CreaDataAdapter(
                DataProviderFactory.Instance.CreaCommand("select * from GIAS_Registri_Commercializzazione_DatiAgenda", oConn))

        Dim tbl_comm As DataTable =
            CacheDati_Commercializzazione_getDatatable(DS_comm, Reg_Comm_cod, Piva)

        Dim objCommandBuilder As DbCommandBuilder = DataProviderFactory.Instance.CreaCommandBuilder(DA_comm)
        DA_comm.TableMappings.Add(tbl_comm.TableName, "GIAS_Registri_Commercializzazione_DatiAgenda")
        DA_comm.Update(tbl_comm)

    End Sub

#End Region

End Class

Public Class GIAS_Registri_Commercializzazione_Riepilogo_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '################################################################################
    Public Sub Scrivi(ByVal Reg_Comm_cod As Integer _
                            , ByVal Progressivo As Integer _
                            , ByVal Linea As String _
                            , ByVal Dettagli As String _
                            , ByVal CaricoKg As String _
                            , ByVal ScaricoKg As String _
                            , ByVal CaricoLt As String _
                            , ByVal ScaricoLt As String _
                            , ByVal SaldoPrima As String _
                            , ByVal SaldoDopo As String _
                            , ByVal DataFineRiepilogo As String _
                            , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                        )

        Dim NomeRoutine As String = "GIAS_Registri_Commercializzazione_Riepilogo_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try

            StrSql =
            "INSERT INTO  GIAS_Registri_Commercializzazione_Riepilogo " &
            " ([Reg_Comm_cod] " &
            "  ,[Progressivo] " &
            "  ,[Linea] " &
            "  ,[Dettagli] " &
            "  ,[CaricoKg] " &
            "  ,[ScaricoKg] " &
            "  ,[CaricoLt] " &
            "  ,[ScaricoLt] " &
            "  ,[SaldoPrima] " &
            "  ,[SaldoDopo] " &
            "  ,[DataFineRiepilogo]) " &
            "            VALUES " &
            " (" & Agro_SQL_SaveNum(Reg_Comm_cod) & " " &
            ", " & Agro_SQL_SaveNum(Progressivo) & " " &
            ",'" & Agro_SQL_SaveText(Linea) & "'" &
            ",'" & Agro_SQL_SaveText(Dettagli) & "'" &
            ", " & Agro_SQL_SaveNum(CaricoKg) & " " &
            ", " & Agro_SQL_SaveNum(ScaricoKg) & " " &
            ", " & Agro_SQL_SaveNum(CaricoLt) & " " &
            ", " & Agro_SQL_SaveNum(ScaricoLt) & " " &
            ", " & Agro_SQL_SaveNum(SaldoPrima) & " " &
            ", " & Agro_SQL_SaveNum(SaldoDopo) & " " &
            ",'" & Agro_SQL_SaveText(DataFineRiepilogo) & "')"

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub


End Class

Public Class GIAS_Registri_Commercializzazione_DatiAgenda_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '################################################################################
    Public Sub Scrivi(
        ByVal Reg_Comm_cod As Integer _
        , ByVal piva As String _
        , ByVal Id_Agenda As Integer _
        , ByVal Data_Movimento As Date _
        , ByVal des_lib As String _
        , ByVal Descrizione As String _
        , ByVal Designazione As String _
        , ByVal Cau_Mov As String _
        , ByVal Mov_Desc As String _
        , ByVal Elem_Cod As Integer _
        , ByVal Mat_Cod As Integer _
        , ByVal Mov_Det_Des As String _
        , ByVal CaricoKg As String _
        , ByVal Udm_Cod As Integer _
        , ByVal Udm_Sim As String _
        , ByVal Mat_Des As String _
        , ByVal Cod_Articolo As String _
        , ByVal ScaricoKg As String _
        , ByVal NDoc As String _
        , ByVal CaricoLt As String _
        , ByVal ScaricoLt As String _
        , ByVal Lotto As String _
        , ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        )


        Dim NomeRoutine As String = "GIAS_Registri_Commercializzazione_DatiAgenda_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql =
            "INSERT INTO  GIAS_Registri_Commercializzazione_DatiAgenda " &
             "  ([Reg_Comm_cod] " &
                "  ,[piva] " &
                "  ,[Id_Agenda] " &
                "  ,[Data_Movimento] " &
                "  ,[des_lib] " &
                "  ,[Descrizione] " &
                "  ,[Designazione] " &
                "  ,[Cau_Mov] " &
                "  ,[Mov_Desc] " &
                "  ,[Elem_Cod] " &
                "  ,[Mat_Cod] " &
                "  ,[Mov_Det_Des] " &
                "  ,[CaricoKg] " &
                "  ,[Udm_Cod] " &
                "  ,[Udm_Sim] " &
                "  ,[Mat_Des] " &
                "  ,[Cod_Articolo] " &
                "  ,[ScaricoKg] " &
                "  ,[NDoc] " &
                "  ,[CaricoLt] " &
                "  ,[ScaricoLt] " &
                "  ,[Lotto] )" &
             "            VALUES " &
             " (" & Reg_Comm_cod & " " &
                ",'" & Agro_SQL_SaveText(piva) & "'" &
                ", " & Agro_SQL_SaveNum(Id_Agenda) & " " &
                "," & Agro_SQL_SaveDate(Data_Movimento) & "" &
                ",'" & Agro_SQL_SaveText(des_lib) & "'" &
                ",'" & Agro_SQL_SaveText(Descrizione) & "'" &
                ",'" & Agro_SQL_SaveText(Designazione) & "'" &
                ",'" & Agro_SQL_SaveText(Cau_Mov) & "' " &
                ",'" & Agro_SQL_SaveText(Mov_Desc) & "'" &
                ", " & Agro_SQL_SaveNum(Elem_Cod) & " " &
                ", " & Agro_SQL_SaveNum(Mat_Cod) & " " &
                ",'" & Agro_SQL_SaveText(Mov_Det_Des) & "'" &
                ", " & Agro_SQL_SaveNum(CaricoKg) & " " &
                ", " & Agro_SQL_SaveNum(Udm_Cod) & " " &
                ",'" & Agro_SQL_SaveText(Udm_Sim) & "'" &
                ",'" & Agro_SQL_SaveText(Mat_Des) & "'" &
                ",'" & Agro_SQL_SaveText(Cod_Articolo) & "'" &
                ", " & Agro_SQL_SaveNum(ScaricoKg) & " " &
                ",'" & Agro_SQL_SaveText(NDoc) & "'" &
                ", " & Agro_SQL_SaveNum(CaricoLt) & " " &
                ", " & Agro_SQL_SaveNum(ScaricoLt) & " " &
                ",'" & Agro_SQL_SaveText(Lotto) & "')"


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

    End Sub


End Class

Public Class RegistriCantina_Commercializzazione_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '################################################################################
    Public Sub Scrivi(
                ByVal Reg_Comm_cod As Integer,
                ByVal Piva As String,
                ByVal sa_cod As Integer,
                ByVal Descrizione As String,
                ByVal DataDa As String,
                ByVal DataA As String,
                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "RegistriCantina_Commercializzazione_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Dim StrSql As String

        Try
            StrSql =
            "INSERT INTO  GIAS_Registri_Commercializzazione_Testata " &
             "           (" &
             "            [Reg_Comm_cod] " &
             "           ,[PIVA] " &
             "           ,[sa_cod] " &
             "           ,[Descrizione] " &
             "           ,[DataDa] " &
             "           ,[DataA]) " &
             "            VALUES " &
             "           (" & Agro_SQL_SaveNum(Reg_Comm_cod) & " " &
             "           ,'" & Agro_SQL_SaveText(Piva) & "'" &
             "           ," & Agro_SQL_SaveNum(sa_cod) & "" &
             "           ,'" & Agro_SQL_SaveText(Descrizione) & "' " &
             "           ," & Agro_SQL_SaveDate(DataDa) & "" &
             "           ," & Agro_SQL_SaveDate(DataA) & ")"

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSql, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

    End Sub





End Class