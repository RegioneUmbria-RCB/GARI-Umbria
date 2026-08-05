Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Tracciabilita
    Inherits AgronicaCoreDataProvider.DataProvider


    '###################################################################################################################
    Public Function SchedaTracciabilitaVegetale_SezioneVendite(ByVal Piva As String, _
                                                                ByVal ElencoChiaviImpianto As String, _
                                                                ByVal Validita_Inizio As Date, _
                                                                ByVal Validita_Fine As Date, _
                                                                ByVal FiltroAggiuntivo As String, _
                                                                ByVal Ordinamento As String, _
                                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Tracciabilita.SchedaTracciabilitaVegetale_SezioneVendite"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Dim StbSQL As New System.Text.StringBuilder
        Dim Stb_Select As New System.Text.StringBuilder
        Dim Stb_SelectContab As New System.Text.StringBuilder
        Dim Stb_SelectContab2 As New System.Text.StringBuilder
        Dim Stb_SelectContabDF As New System.Text.StringBuilder
        Dim Stb_SelectContab2DF As New System.Text.StringBuilder
        Dim Stb_SelectContatti As New System.Text.StringBuilder
        Dim Stb_SelectContattiSU As New System.Text.StringBuilder
        Dim Stb_SelectContattiDF As New System.Text.StringBuilder
        Dim Stb_SelectIndirizzi As New System.Text.StringBuilder
        Dim Stb_SelectIndirizziDF As New System.Text.StringBuilder
        Dim Stb_SelectExtra As New System.Text.StringBuilder
        Dim Stb_SelectExtraDF As New System.Text.StringBuilder
        Dim Stb_Join As New System.Text.StringBuilder
        Dim Stb_JoinContab As New System.Text.StringBuilder
        Dim Stb_JoinContab2 As New System.Text.StringBuilder
        Dim Stb_JoinContatti As New System.Text.StringBuilder
        Dim Stb_JoinContattiSU As New System.Text.StringBuilder
        Dim Stb_JoinIndirizzi As New System.Text.StringBuilder
        'Dim Stb_JoinExtra As New System.Text.StringBuilder
        Dim Stb_JoinExtraLeft As New System.Text.StringBuilder
        Dim Stb_JoinRif As New System.Text.StringBuilder
        Dim Stb_Where As New System.Text.StringBuilder
        Dim Stb_Where1 As New System.Text.StringBuilder
        Dim Stb_Where2 As New System.Text.StringBuilder
        Dim Stb_Where3 As New System.Text.StringBuilder
        Dim Stb_Where4 As New System.Text.StringBuilder
        Dim Stb_Where5 As New System.Text.StringBuilder
        Dim Stb_Where6 As New System.Text.StringBuilder
        Dim Stb_Ordinamento As New System.Text.StringBuilder


        Try

            StbSQL.Length = 0



            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------

            'NOTA SUL DISTINCT:
            'E' necessario mettere il distinct per evitare la ripetizione dei record.
            'tali record vengono visualizzati tante volte quanti sono i dettagli legati al movimento della raccolta (2200)
            'non è possibile scremare questi dettagli, per ottenere solo quello della materia prima di riferimento,
            'perchè nei dettagli del movimento di raccolta vengono salvati cal_cod diversi da quelli dei dettagli del movimento di magazzino
            'Nei cal_cod dei dettagli del mov di magazzino ci sono i cal_cod negativi, che rimandano alla tabella materie_prime_campionature (che sono negativi)
            'mentre nei cal_cod dei dettagli del mov di raccolta ci sono i cal_cod della tabella calibri_frutti (che sono positivi e sono i calibri di default) o materie_prime_calibri (che sono negativi e sono quelli personalizzati dall'utente).

            Stb_Select.Append(" SELECT DISTINCT Agenda.PIVA, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, " & vbCrLf)
            Stb_Select.Append("         Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo, Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, Cultivar.Cul_Des, Movimenti_dettagli.Qta, UnitaMisura.Udm_Sim, UnitaMisura.Udm_Des,  " & vbCrLf)
            Stb_Select.Append("         Movimenti_dettagli.elem_cod, Movimenti_dettagli.pro_cod, Movimenti_dettagli.mat_cod, Movimenti_dettagli.cod_progetto, Movimenti_dettagli.fase_cod, ISNULL(Movimenti_dettagli.lotto, '') AS Lotto, Movimenti_dettagli.cal_cod, Movimenti_dettagli.udm_cod,  ")

            Stb_SelectContab.Append("         Mov_Contabile.Mov_Desc,  " & vbCrLf)
            Stb_SelectContab.Append("         Mov_Contabile.Data_Movimento AS Data, Mov_Contabile.Doc_Numero_Sin, Mov_Contabile.Doc_Numero, Mov_Contabile.Doc_Numero_Des, " & vbCrLf)
            Stb_SelectContab.Append("         Mov_Contabile.Mezzo, " & vbCrLf)

            Stb_SelectContab2.Append("          Mov_Conf.Data_Movimento AS Data_DDTConf, Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_DDTConf, Mov_Conf.Doc_Numero AS Doc_Numero_DDTConf, Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_DDTConf, " & vbCrLf)
            Stb_SelectContab2DF.Append("        '' AS Data_DDTConf, '' AS Doc_Numero_Sin_DDTConf, 0 AS Doc_Numero_DDTConf, '' AS Doc_Numero_Des_DDTConf, " & vbCrLf)

            Stb_SelectContabDF.Append("         Mov_Mag.Mov_Desc,  " & vbCrLf)
            Stb_SelectContabDF.Append("         Mov_Mag.Data_Movimento  AS Data, '' AS Doc_Numero_Sin, 0 AS Doc_Numero, '' AS Doc_Numero_Des, " & vbCrLf)
            Stb_SelectContabDF.Append("         0 AS Mezzo, " & vbCrLf)

            Stb_SelectContatti.Append(" Contatti.Codice_Fiscale, Contatti.Cod_Contatto, Contatti.Rag_Soc, Contatti.Nome, Contatti.Cognome,  " & vbCrLf)
            Stb_SelectContattiSU.Append(" '' AS Codice_Fiscale, agenda.piva AS Cod_Contatto, imprese.Rag_Soc, '' AS Nome, '' AS Cognome,  " & vbCrLf)
            Stb_SelectIndirizzi.Append(" Indirizzi.ind_des, Indirizzi.frz_des, Indirizzi.CAP, ISNULL(ISTAT.LOCALITA, '') AS com_des, ISNULL(ISTAT.COMUNI_PROV, '') AS comuni_prov, Indirizzi.stato, --Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat " & vbCrLf)

            Stb_SelectContattiDF.Append(" '' AS Codice_Fiscale, '' AS Cod_Contatto, '' AS Rag_Soc, '' AS Nome, '' AS Cognome,  " & vbCrLf)
            Stb_SelectIndirizziDF.Append(" '' AS ind_des, '' AS frz_des, '' AS CAP, '' AS com_des, '' AS comuni_prov, '' AS stato, --Indirizzi.note, Indirizzi.pro_cod_istat, Indirizzi.com_cod_istat " & vbCrLf)

            Stb_SelectExtra.Append(" ISNULL(Movimento_Extra.Mac_Cod, 0 ) AS Mac_Cod, ISNULL(Movimento_Extra.Mezzo_Trasporto, '') AS Mezzo_Trasporto, ISNULL(Movimento_Extra.Targa, '') AS Targa --,Movimento_Extra.N_Immatricolazione AS N_Immatricolazione_2, Movimento_Extra.N_Immatricolazione_Rimorchio AS N_Immatricolazione_Rimorchio_2, " & vbCrLf)
            'Stb_SelectExtra.Append(" Movimento_Extra.N_Autorizzazione_Trasporto AS N_Autorizzazione_Trasporto_2, Movimento_Extra.Data_Rilascio_Autorizzazione AS Data_Rilascio_Autorizzazione_2, Movimento_Extra.Peso AS Peso_2, " & vbCrLf)

            Stb_SelectExtraDF.Append(" 0 AS Mac_Cod, '' AS Mezzo_Trasporto, '' AS Targa --,Movimento_Extra.N_Immatricolazione AS N_Immatricolazione_2, Movimento_Extra.N_Immatricolazione_Rimorchio AS N_Immatricolazione_Rimorchio_2, " & vbCrLf)


            'JOIN AGENDA - MOVIMENTI
            Stb_Join.Append(" FROM    Agenda  " & vbCrLf)
            'JOIN AGENDA - MOVIMENTI MAG
            Stb_Join.Append(" INNER JOIN Movimenti Mov_Mag " & vbCrLf)
            Stb_Join.Append(" ON Agenda.PIVA = Mov_Mag.PIVA AND Agenda.Id_Agenda = Mov_Mag.Id_Agenda" & vbCrLf)
            'JOIN MOVIMENTI - MOVIMENTI DETTAGLI
            Stb_Join.Append(" INNER JOIN Movimenti_dettagli " & vbCrLf)
            Stb_Join.Append(" ON Movimenti_dettagli.PIVA = Mov_Mag.PIVA AND Movimenti_dettagli.Id_Agenda = Mov_Mag.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Mag.Id_Mov " & vbCrLf)
            'JOIN MOVIMENTI DETTAGLI - MATERIE PRIME
            Stb_Join.Append(" INNER JOIN Materie_Prime ON Movimenti_dettagli.elem_cod = Materie_Prime.elem_cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            'JOIN MATERIE PRIME - CULTIVAR
            Stb_Join.Append(" INNER JOIN Cultivar ON Cultivar.Cul_Cod = Materie_Prime.Cul_Cod " & vbCrLf)
            'JOIN MOVIMENTI DETTAGLI - UNITA MISURA
            Stb_Join.Append(" INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod " & vbCrLf)

            'JOIN CON OP. DI RACCOLTA
            Stb_Join.Append(" INNER JOIN Movimenti_dettagli Mov_DettMag_Raccolta ON Mov_DettMag_Raccolta.Elem_Cod = Movimenti_dettagli.Elem_Cod " & vbCrLf)
            Stb_Join.Append(" AND Mov_DettMag_Raccolta.pro_cod = Movimenti_dettagli.pro_cod " & vbCrLf)
            Stb_Join.Append(" AND Mov_DettMag_Raccolta.mat_cod = Movimenti_dettagli.mat_cod " & vbCrLf)
            Stb_Join.Append(" AND Mov_DettMag_Raccolta.cod_progetto = Movimenti_dettagli.cod_progetto " & vbCrLf)
            Stb_Join.Append(" AND Mov_DettMag_Raccolta.fase_cod = Movimenti_dettagli.fase_cod " & vbCrLf)
            Stb_Join.Append(" AND Mov_DettMag_Raccolta.lotto = Movimenti_dettagli.lotto " & vbCrLf)
            Stb_Join.Append(" AND Mov_DettMag_Raccolta.cal_cod = Movimenti_dettagli.cal_cod " & vbCrLf)
            Stb_Join.Append(" AND Mov_DettMag_Raccolta.udm_cod = Movimenti_dettagli.udm_cod " & vbCrLf)
            Stb_Join.Append(" INNER JOIN Movimenti Mov_Mag_Raccolta ON Mov_Mag_Raccolta.PIVA = Mov_DettMag_Raccolta.PIVA " & vbCrLf)
            'Mi raccomando, questo join non ci va altrimenti non vengono caricate le bolle di conf verso diversi!
            'Stb_Join.Append(" -- AND Mov_Mag_Raccolta.Sa_Cod = Mov_DettMag_Raccolta.Sa_Cod " & vbCrLf)
            Stb_Join.Append(" AND Mov_DettMag_Raccolta.Id_Agenda = Mov_Mag_Raccolta.Id_Agenda " & vbCrLf)
            Stb_Join.Append(" INNER JOIN Agenda Agenda_Raccolta ON Agenda_Raccolta.PIVA = Mov_Mag_Raccolta.PIVA AND Agenda_Raccolta.Sa_Cod = Mov_Mag_Raccolta.Sa_Cod AND " & vbCrLf)
            Stb_Join.Append(" Agenda_Raccolta.Id_Agenda = Mov_Mag_Raccolta.Id_Agenda " & vbCrLf)
            Stb_Join.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda_Raccolta.PIVA = Mov_Raccolta.PIVA AND Agenda_Raccolta.Sa_Cod = Mov_Raccolta.Sa_Cod AND " & vbCrLf)
            Stb_Join.Append(" Agenda_Raccolta.Id_Agenda = Mov_Raccolta.Id_Agenda " & vbCrLf)
            Stb_Join.Append(" INNER JOIN Movimenti_dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Sa_Cod = Mov_Dett_Raccolta.Sa_Cod AND  " & vbCrLf)
            Stb_Join.Append(" Mov_Dett_Raccolta.Id_Agenda = Mov_Raccolta.Id_Agenda And Mov_Dett_Raccolta.id_mov = Mov_Raccolta.Id_mov " & vbCrLf)
            Stb_Join.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND " & vbCrLf)
            Stb_Join.Append(" Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND " & vbCrLf)
            Stb_Join.Append(" Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND " & vbCrLf)
            Stb_Join.Append(" Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det  " & vbCrLf)

            'JOIN AGENDA - MOVIMENTI CONTAB
            Stb_JoinContab.Append(" INNER JOIN Movimenti Mov_Contabile " & vbCrLf)
            Stb_JoinContab.Append(" ON Agenda.PIVA = Mov_Contabile.PIVA AND Agenda.Sa_Cod = Mov_Contabile.Sa_Cod AND Agenda.Id_Agenda = Mov_Contabile.Id_Agenda" & vbCrLf)
            'JOIN MOVIMENTI CONTAB - MOV DETT TEC EXTRA
            'Stb_JoinExtra.Append(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra ON Movimento_Extra.PIVA = Mov_Contabile.PIVA AND Movimento_Extra.Sa_Cod = Mov_Contabile.Sa_Cod AND Movimento_Extra.Id_Agenda = Mov_Contabile.Id_Agenda AND Movimento_Extra.Id_Mov = Mov_Contabile.Id_Mov " & vbCrLf)
            Stb_JoinExtraLeft.Append(" LEFT OUTER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra ON Movimento_Extra.PIVA = Mov_Contabile.PIVA AND Movimento_Extra.Sa_Cod = Mov_Contabile.Sa_Cod AND Movimento_Extra.Id_Agenda = Mov_Contabile.Id_Agenda AND Movimento_Extra.Id_Mov = Mov_Contabile.Id_Mov " & vbCrLf)
            'JOIN MOVIMENTI DETTAGLI - RISORSE UMANE CONTAB
            'non mettere in join la piva, mi raccomando!!!!!!
            Stb_JoinContatti.Append(" INNER JOIN Risorse_Umane ON Risorse_Umane.Cod_RisUm = Mov_Contabile.Cod_RisUm " & vbCrLf)
            'JOIN RISORSE UMANE - CONTATTI CONTAB
            Stb_JoinContatti.Append(" INNER JOIN Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto " & vbCrLf)
            'JOIN MOVIMENTI CONTAB - INDIRIZZI
            Stb_JoinContatti.Append(" INNER JOIN Indirizzi ON Mov_Contabile.Cod_IndirizzoRisUm = Indirizzi.cod_indirizzo " & vbCrLf)

            Stb_JoinContattiSU.Append(" INNER JOIN Imprese ON Agenda.Piva = Imprese.Piva " & vbCrLf)
            Stb_JoinContattiSU.Append(" INNER JOIN ImpreseXIndirizzi ON Imprese.Piva = ImpreseXIndirizzi.Piva " & vbCrLf)
            Stb_JoinContattiSU.Append(" INNER JOIN Indirizzi ON ImpreseXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)

            'ISTAT
            Stb_JoinIndirizzi.Append(" LEFT OUTER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM " & vbCrLf)

            Stb_JoinRif.Append(" INNER JOIN Mov_Dettagli_Riferimenti ON Movimenti_dettagli.Piva = Mov_Dettagli_Riferimenti.Piva  " & vbCrLf)
            Stb_JoinRif.Append(" AND Movimenti_dettagli.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda " & vbCrLf)
            'con la raccolta in join, id_mov e id_mov_det =-1
            'Stb_JoinRif.Append(" --AND Movimenti_dettagli.Id_mov = Mov_Dettagli_Riferimenti.Id_mov AND Movimenti_dettagli.Id_mov_det = Mov_Dettagli_Riferimenti.Id_mov_det " & vbCrLf)
            'questo join serve per indicare che nella prima operazione si tratta di conferimento
            Stb_JoinRif.Append(" AND Agenda.lav_cod = Mov_Dettagli_Riferimenti.lav_cod  " & vbCrLf)
            'questo join serve per indicare che anche il movimento è quello di conferimento
            Stb_JoinRif.Append(" AND mov_mag.cau_mov = Mov_Dettagli_Riferimenti.cau_mov  " & vbCrLf)

            Stb_JoinContab2.Append(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda  " & vbCrLf)
            Stb_JoinContab2.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Contabile.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  " & vbCrLf)


            'CONDIZIONI SULLA RACCOLTA E GLI IMPIANTI
            Stb_Where.Append(" AND Agenda_Raccolta.Lav_Cod = " & CStr(LAVCOD_RACCOLTA) & " " & vbCrLf)
            Stb_Where.Append(" AND Mov_Raccolta.Cau_Mov = '" & CAU_RILIEVO_RACCOLTA & "'" & vbCrLf)
            '23/08/2018 MAGA:
            'da quando il GiasOnline ha iniziato a salvare anche l'ora in data_movimento
            'l'ultimo giorno non veniva conteggiato
            Validita_Fine = DateAdd(DateInterval.Day, 1, CDate(Validita_Fine))
            'Stb_Where.Append(" AND Mov_Raccolta.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & " " & vbCrLf)
            Stb_Where.Append(" AND Mov_Raccolta.Data_Movimento < " & Agro_SQL_SaveDate(Validita_Fine) & " " & vbCrLf)
            Stb_Where.Append(" AND Mov_Raccolta.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " & vbCrLf)
            Stb_Where.Append(" AND               ((Mov_Dest_Raccolta.Piva  " & vbCrLf)
            Stb_Where.Append("                     + '_' + CONVERT(varchar(10), Mov_Dest_Raccolta.Sa_Cod)  " & vbCrLf)
            Stb_Where.Append("                     + '_' + CONVERT(varchar(10), Mov_Dest_Raccolta.Appezza)  " & vbCrLf)
            Stb_Where.Append("                     + '_' + CONVERT(varchar(10), Mov_Dest_Raccolta.Id_Destinazione))  " & vbCrLf)
            Stb_Where.Append("                     IN (" & Agro_SQL_Save_Clausola_IN(ElencoChiaviImpianto, True) & "))  " & vbCrLf)

            If Ordinamento = "" Then
                Stb_Ordinamento.Append(" ORDER BY Data ASC " & vbCrLf)
            Else
                Stb_Ordinamento.Append(Ordinamento)
            End If


            StbSQL.Length = 0

            StbSQL.Append("( " & vbCrLf)

            '###################################################################################################
            '   FATTURE IMMEDIATE - DDT EMESSI - BOLLE CONFERIMENTO A CONTATTO
            '###################################################################################################

            StbSQL.Append(" -- FATTURE IMMEDIATE - DDT EMESSI - BOLLE CONFERIMENTO A CONTATTO " & vbCrLf)
            StbSQL.Append(Stb_Select)
            StbSQL.Append(Stb_SelectContab)
            StbSQL.Append(Stb_SelectContab2DF)
            StbSQL.Append(Stb_SelectContatti)
            StbSQL.Append(Stb_SelectIndirizzi)
            StbSQL.Append(Stb_SelectExtra)

            StbSQL.Append(Stb_Join)
            StbSQL.Append(Stb_JoinContab)
            StbSQL.Append(Stb_JoinContatti)
            StbSQL.Append(Stb_JoinIndirizzi)
            StbSQL.Append(Stb_JoinExtraLeft)

            Stb_Where1.Append(" WHERE   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            Stb_Where1.Append(" AND     Agenda.Lav_Cod IN ( " & _
                                                                CStr(LAVCOD_BOLLA_EMESSA) & "," & _
                                                                CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & "," & _
                                                                CStr(LAVCOD_DOCO_EMESSO) & "," & _
                                                                CStr(LAVCOD_DAA_EMESSO) & "," & _
                                                                CStr(LAVCOD_FATTURA_EMESSA) & "," & _
                                                                CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & "," & _
                                                                CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & "," & _
                                                                CStr(LAVCOD_CONFERIMENTO_DIVERSI) & _
                                                            " ) " & vbCrLf)

            Stb_Where1.Append(" AND     Mov_Contabile.Cau_Mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)
            Stb_Where1.Append(" AND     Mov_Mag.Cau_Mov IN (" & "'" & CAU_SCARICO & "'," & "'" & CAU_CONFERIMENTO_DIVERSI & "'" & ")" & vbCrLf)
            'SOLO FATTURE IMMEDIATE, NON DIFFERITE (CHE HANNO IL JOLLY_INT=1)
            Stb_Where1.Append(" AND     Movimenti_dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   ")
            If FiltroAggiuntivo <> "" Then
                Stb_Where1.Append(Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , objParametri))
            End If
            StbSQL.Append(Stb_Where1)
            StbSQL.Append(Stb_Where)


            '###################################################################################################

            StbSQL.Append(") " & vbCrLf)

            StbSQL.Append(" UNION ALL " & vbCrLf)

            StbSQL.Append(" ( " & vbCrLf)

            '###################################################################################################
            '   RICEVUTA FISCALE
            '###################################################################################################

            StbSQL.Append(" --  RICEVUTA FISCALE " & vbCrLf)
            StbSQL.Append(Stb_Select)
            StbSQL.Append(Stb_SelectContab)
            StbSQL.Append(Stb_SelectContab2DF)
            StbSQL.Append(Stb_SelectContatti)
            StbSQL.Append(Stb_SelectIndirizzi)
            StbSQL.Append(Stb_SelectExtraDF)

            StbSQL.Append(Stb_Join)
            StbSQL.Append(Stb_JoinContab)
            StbSQL.Append(Stb_JoinContatti)
            StbSQL.Append(Stb_JoinIndirizzi)

            Stb_Where2.Append(" WHERE   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            Stb_Where2.Append(" AND     Agenda.Lav_Cod = " & CStr(LAVCOD_RICEVUTA_EMESSA) & " " & vbCrLf)
            Stb_Where2.Append(" AND     Mov_Contabile.Cau_Mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)
            Stb_Where2.Append(" AND     Mov_Mag.Cau_Mov = " & "'" & CAU_SCARICO & "'" & " " & vbCrLf)
            If FiltroAggiuntivo <> "" Then
                Stb_Where2.Append(Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , objParametri))
            End If
            StbSQL.Append(Stb_Where2)
            StbSQL.Append(Stb_Where)

            '###################################################################################################

            StbSQL.Append(" ) " & vbCrLf)

            StbSQL.Append(" UNION ALL " & vbCrLf)

            StbSQL.Append(" ( " & vbCrLf)

            '###################################################################################################
            '   AUTOCONSUMO - VENDITA
            '###################################################################################################

            StbSQL.Append(" --  AUTOCONSUMO - VENDITA " & vbCrLf)
            StbSQL.Append(Stb_Select)
            StbSQL.Append(Stb_SelectContabDF)
            StbSQL.Append(Stb_SelectContab2DF)
            StbSQL.Append(Stb_SelectContattiDF)
            StbSQL.Append(Stb_SelectIndirizziDF)
            StbSQL.Append(Stb_SelectExtraDF)

            StbSQL.Append(Stb_Join)

            Stb_Where3.Append(" WHERE   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            Stb_Where3.Append(" AND     Agenda.Lav_Cod IN ( " & CStr(LAVCOD_AUTOCONSUMO) & ", " & CStr(LAVCOD_AUTOCONSUMO_VINO_SFUSO) & ", " & CStr(LAVCOD_VENDITA) & " ) " & vbCrLf)
            Stb_Where3.Append(" AND     Mov_Mag.Cau_Mov = " & "'" & CAU_SCARICO & "'" & " " & vbCrLf)
            If FiltroAggiuntivo <> "" Then
                Stb_Where3.Append(Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , objParametri))
            End If
            StbSQL.Append(Stb_Where3)
            StbSQL.Append(Stb_Where)


            '###################################################################################################

            StbSQL.Append(") " & vbCrLf)

            StbSQL.Append(" UNION ALL " & vbCrLf)

            StbSQL.Append(" ( " & vbCrLf)


            '###################################################################################################
            ' BOLLE CONFERIMENTO AL S.U. 
            '###################################################################################################

            StbSQL.Append(" --  BOLLE CONFERIMENTO AL S.U.  " & vbCrLf)
            StbSQL.Append(Stb_Select)
            StbSQL.Append(Stb_SelectContab)
            StbSQL.Append(Stb_SelectContab2DF)
            StbSQL.Append(Stb_SelectContattiSU)
            StbSQL.Append(Stb_SelectIndirizzi)
            StbSQL.Append(Stb_SelectExtra)

            StbSQL.Append(Stb_Join)
            StbSQL.Append(Stb_JoinContab)
            StbSQL.Append(Stb_JoinContattiSU)
            StbSQL.Append(Stb_JoinIndirizzi)
            StbSQL.Append(Stb_JoinExtraLeft)
            StbSQL.Append(Stb_JoinRif)

            Stb_Where4.Append(" WHERE   Agenda.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            Stb_Where4.Append(" AND     Agenda.Lav_Cod = " & CStr(LAVCOD_CONFERIMENTO) & " " & vbCrLf)
            Stb_Where4.Append(" AND     Mov_Contabile.Cau_Mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)
            Stb_Where4.Append(" AND     Mov_Mag.Cau_Mov = " & "'" & CAU_CONFERIMENTO & "'" & "" & vbCrLf)
            'piva_rif indica che l'operazione riferita è del socio di cui si sta stampando la scheda
            Stb_Where4.Append(" AND     Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            'indica che l'operazione collegata è lo scarico di magazzino (del socio)
            'Stb_Where4.Append(" AND     Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & CStr(LAVCOD_SCARICO) & " " & vbCrLf)
            Stb_Where4.Append(" AND     Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & CStr(LAVCOD_RACCOLTA) & " " & vbCrLf)
            'indica che l'operazione collegata è lo scarico di magazzino (del socio)
            'Stb_Where4.Append(" AND     Mov_Dettagli_Riferimenti.Cau_Mov_Rif = " & "'" & CAU_SCARICO & "'" & "" & vbCrLf)
            Stb_Where4.Append(" AND     Mov_Dettagli_Riferimenti.Cau_Mov_Rif = " & "'" & CAU_RILIEVO_RACCOLTA & "'" & "" & vbCrLf)
            If FiltroAggiuntivo <> "" Then
                Stb_Where1.Append(Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , objParametri))
            End If
            StbSQL.Append(Stb_Where4)
            StbSQL.Append(Stb_Where)

            '###################################################################################################

            StbSQL.Append(") " & vbCrLf)

            StbSQL.Append(" UNION ALL " & vbCrLf)

            StbSQL.Append(" ( " & vbCrLf)

            '###################################################################################################
            ' ACCETTAZIONE DA DIVERSI (MODALITA' FRUTTAGEL)
            '###################################################################################################

            StbSQL.Append(" --  ACCETTAZIONE DA DIVERSI (MODALITA' FRUTTAGEL) " & vbCrLf)
            StbSQL.Append(Stb_Select)
            StbSQL.Append(Stb_SelectContab)
            StbSQL.Append(Stb_SelectContab2)
            StbSQL.Append(Stb_SelectContattiSU)
            StbSQL.Append(Stb_SelectIndirizzi)
            StbSQL.Append(Stb_SelectExtraDF)

            StbSQL.Append(Stb_Join)
            StbSQL.Append(Stb_JoinContab)
            StbSQL.Append(Stb_JoinContattiSU)
            StbSQL.Append(Stb_JoinIndirizzi)
            StbSQL.Append(Stb_JoinContab2)

            Stb_Where5.Append(" WHERE   Agenda.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
            Stb_Where5.Append(" AND     Agenda.Lav_Cod = " & CStr(LAVCOD_ACCETTAZIONE_DIVERSI) & " " & vbCrLf)
            Stb_Where5.Append(" AND     Mov_Contabile.Cau_Mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)
            Stb_Where5.Append(" AND     Mov_Conf.Cau_Mov = '" & CAU_REGISTRAZIONI_ALLEGATE & "' " & vbCrLf)
            Stb_Where5.Append(" AND     Mov_Mag.Cau_Mov = " & "'" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "'" & "" & vbCrLf)
            Stb_Where5.Append(" AND     Risorse_Umane_Produttori.Cod_contatto = " & "'" & Agro_SQL_SaveText(Piva) & "'" & "" & vbCrLf)
            If FiltroAggiuntivo <> "" Then
                Stb_Where1.Append(Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , objParametri))
            End If
            StbSQL.Append(Stb_Where5)
            StbSQL.Append(Stb_Where)

            '###################################################################################################

            StbSQL.Append(") " & vbCrLf)



            '###################################################################################################
            'TOPPA PER CONSORZIO AGRARIO RAVENNA -> DA GESTIRE MEGLIO
            'STESSA QUERY DI BOLLE CONFERIMENTO AL S.U., MA NON E' IL SUPERUSER E' CEREALI PADENNA, UNA COOP SOTTO AL S.U.
            '###################################################################################################

            If objParametri.PivaSuperUser = "03297010401" Then

                StbSQL.Append(" UNION ALL " & vbCrLf)

                StbSQL.Append(" ( " & vbCrLf)

                StbSQL.Append(" -- CONSORZIO AGRARIO RAVENNA (STESSA QUERY DI BOLLE CONFERIMENTO AL S.U., MA NON E' IL SUPERUSER E' CEREALI PADENNA, UNA COOP SOTTO AL S.U.) " & vbCrLf)
                StbSQL.Append(Stb_Select)
                StbSQL.Append(Stb_SelectContab)
                StbSQL.Append(Stb_SelectContab2DF)
                StbSQL.Append(Stb_SelectContattiSU)
                StbSQL.Append(Stb_SelectIndirizzi)
                StbSQL.Append(Stb_SelectExtra)

                StbSQL.Append(Stb_Join)
                StbSQL.Append(Stb_JoinContab)
                StbSQL.Append(Stb_JoinContattiSU)
                StbSQL.Append(Stb_JoinIndirizzi)
                StbSQL.Append(Stb_JoinExtraLeft)
                StbSQL.Append(Stb_JoinRif)

                '                           -> piva di cereali padenna
                Stb_Where6.Append(" WHERE   Agenda.Piva = '02095520397' " & vbCrLf)
                Stb_Where6.Append(" AND     Agenda.Lav_Cod = " & CStr(LAVCOD_CONFERIMENTO) & " " & vbCrLf)
                Stb_Where6.Append(" AND     Mov_Contabile.Cau_Mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)
                Stb_Where6.Append(" AND     Mov_Mag.Cau_Mov = " & "'" & CAU_CONFERIMENTO & "'" & "" & vbCrLf)
                'piva_rif indica che l'operazione riferita è del socio di cui si sta stampando la scheda
                Stb_Where6.Append(" AND     Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
                'indica che l'operazione collegata è la raccolta (del socio)
                Stb_Where6.Append(" AND     Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & CStr(LAVCOD_RACCOLTA) & " " & vbCrLf)
                'indica che l'operazione collegata è la raccolta (del socio)
                Stb_Where6.Append(" AND     Mov_Dettagli_Riferimenti.Cau_Mov_Rif = " & "'" & CAU_RILIEVO_RACCOLTA & "'" & "" & vbCrLf)

                If FiltroAggiuntivo <> "" Then
                    Stb_Where1.Append(Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , objParametri))
                End If
                StbSQL.Append(Stb_Where6)
                StbSQL.Append(Stb_Where)

                StbSQL.Append(") " & vbCrLf)

            End If

            '###################################################################################################

            StbSQL.Append(Stb_Ordinamento)

            '###################################################################################################


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




    '###################################################################################################################
    Public Function SchedaTracciabilitaVegetale_SezioneOpColturali(ByVal Piva As String, _
                                                                    ByVal ElencoChiaviImpianto As String, _
                                                                    ByVal Validita_Inizio As Date, _
                                                                    ByVal Validita_Fine As Date, _
                                                                     ByVal xFiltroAggiuntivo As String, _
                                                                        ByVal xOrderBy As String, _
                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Tracciabilita.SchedaTracciabilitaVegetale_SezioneOpColturali"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim StbSQL As New System.Text.StringBuilder

        Try

            StbSQL.Length = 0

            StbSQL.Append(" SELECT DISTINCT " & vbCrLf)
            StbSQL.Append(" Agenda.Lav_Cod, Operazioni.LAV_DES, Reg_Impianti.PIVA, Reg_Impianti.SA_COD, Reg_Impianti.APPEZZA, Reg_Impianti.ID_REG, Reg_Impianti.Sup_Imp," & vbCrLf)
            StbSQL.Append(" Appezzamento.App_Nome, Agenda.Id_Agenda, Movimenti.Data_Movimento, Movimenti.Cau_Mov, Movimenti.Mezzo, Movimenti_dettagli.Elem_Cod, " & vbCrLf)
            StbSQL.Append(" Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, Movimenti_dettagli.Udm_Cod, UnitaMisura.UDM_SIM, Movimenti_dettagli.Qta AS Qta_Tot,  Movimenti_dettagli.Lotto, " & vbCrLf)
            StbSQL.Append(" Mov_Destinazioni.Qta, Fertilizzanti.Fer_Des, Formulati.Fr_Des, Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo, Trappole.TRAP_DES,  " & vbCrLf)

            StbSQL.Append(" ISNULL( (SELECT TOP 1 Appezzamento_Codici.val_cod  ")
            StbSQL.Append(" FROM Appezzamento_Codici ")
            StbSQL.Append(" WHERE id_cod = " & Agro_SQL_SaveNum(CStr(enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento)) & " ")
            StbSQL.Append(" AND Appezzamento_Codici.PIVA = Appezzamento.PIVA ")
            StbSQL.Append(" AND Appezzamento_Codici.sa_cod = Appezzamento.sa_cod ")
            StbSQL.Append(" AND Appezzamento_Codici.appezza = Appezzamento.appezza ")
            StbSQL.Append(" ), '') AS App_Nome_Breve ")

            StbSQL.Append(" FROM Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND " & vbCrLf)
            StbSQL.Append(" Agenda.Id_Agenda = Movimenti.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND " & vbCrLf)
            StbSQL.Append(" Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND " & vbCrLf)
            StbSQL.Append(" Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " & vbCrLf)
            StbSQL.Append(" Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND " & vbCrLf)
            StbSQL.Append(" Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)
            StbSQL.Append(" INNER JOIN Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND " & vbCrLf)
            StbSQL.Append(" Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG " & vbCrLf)
            StbSQL.Append(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD " & vbCrLf)
            StbSQL.Append(" INNER JOIN Appezzamento ON Reg_Impianti.APPEZZA = Appezzamento.APPEZZA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND " & vbCrLf)
            StbSQL.Append(" Reg_Impianti.PIVA = Appezzamento.PIVA " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Trappole ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Materie_Prime ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND " & vbCrLf)
            StbSQL.Append(" Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Fertilizzanti ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN  Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod " & vbCrLf)

            StbSQL.Append(" WHERE  Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Piva) & "'" & vbCrLf)

            'StbSQL.Append(strFiltroImpianti & vbCrLf)

            StbSQL.Append(" AND                (Reg_Impianti.Piva  " & vbCrLf)
            StbSQL.Append("                     + '_' + CONVERT(varchar(10), Reg_Impianti.Sa_Cod)  " & vbCrLf)
            StbSQL.Append("                     + '_' + CONVERT(varchar(10), Reg_Impianti.Appezza)  " & vbCrLf)
            StbSQL.Append("                     + '_' + CONVERT(varchar(10), Reg_Impianti.Id_Reg)  " & vbCrLf)
            StbSQL.Append("                     IN (" & Agro_SQL_Save_Clausola_IN(ElencoChiaviImpianto, True) & "))  " & vbCrLf)

            StbSQL.Append(" AND Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & vbCrLf)
            StbSQL.Append(" AND Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & vbCrLf)
            StbSQL.Append(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & vbCrLf)
            StbSQL.Append(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & vbCrLf)

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Movimenti.Data_Movimento, Agenda.Id_Agenda " & vbCrLf)
            End If

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



End Class
