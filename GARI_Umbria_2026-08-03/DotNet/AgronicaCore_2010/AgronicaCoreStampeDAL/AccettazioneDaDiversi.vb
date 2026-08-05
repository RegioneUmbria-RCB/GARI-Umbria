Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

'/******************************************************************************
'/******************************************************************************
'/******************************************************************************
' CLASSE USATA PER IL PRIMO PORTING SU STAMPE_2010 DEI REPORT AD PERSONAM
' ED ESPORTAZIONI DI FRUTTAGEL SU CONFERIMENTO AD PERSONAM SU GIASLAN
'/******************************************************************************
'/******************************************************************************
'/******************************************************************************



Public Class AccettazioneDaDiversi
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Stampa del report della bolla di accettazione salvata con il giaslan
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function StampaBolla_VecchioConf(ByVal Piva As String,
                                            ByVal Id_Agenda As Integer,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.StampaBolla_VecchioConf"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine("SELECT  Agenda.des_lib, ")
            StbSQL.AppendLine("         Mov_Accett.ChkLayout_Join_Prodotti, Mov_Accett.Peso, Mov_Accett.Extra_Str, Mov_Accett.Natura_Beni, ")
            StbSQL.AppendLine("         Mov_Accett.Tara_Veicolo, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso,  ")
            StbSQL.AppendLine("         Mov_Accett.Data_Movimento AS Data_Accett, Mov_Accett.Ora AS Ora_Accett, ")
            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero,Mov_Accett.Doc_Numero_Des, Mov_Accett.Mov_Desc AS Note, ")
            StbSQL.AppendLine("         Sequenza_Progressivi.Lunghezza_Sin, Sequenza_Progressivi.Lunghezza_Centro, Sequenza_Progressivi.Lunghezza_Des, Sequenza_Progressivi.CarattereFormattazione, ")
            StbSQL.AppendLine("         Mov_Accett.Cod_RisUm,")
            StbSQL.AppendLine("         Risorse_Umane_Conferenti.Cod_Rapporto AS Cod_Rapporto_Conferente, Contatti_Conferenti.Cod_Contatto AS Cod_Contatto_Conferente,  ")
            StbSQL.AppendLine("         Contatti_Conferenti.Rag_Soc AS Rag_Soc_Conferente, ISNULL(Contatti_Conferenti.Codice_Fiscale, '') AS Codice_Fiscale_Conferente, ")
            StbSQL.AppendLine("         Contatti_Conferenti.Nome AS Nome_Conferente, Contatti_Conferenti.Cognome AS Cognome_Conferente,  ")
            StbSQL.AppendLine("         Mov_Accett.Cod_IndirizzoRisUm,   Indirizzi_Conferenti.ind_des AS ind_des_Conferente, Indirizzi_Conferenti.frz_des AS frz_des_Conferente, Indirizzi_Conferenti.CAP AS cap_Conferente,  ")
            StbSQL.AppendLine("         Indirizzi_Conferenti.stato AS stato_Conferente, Indirizzi_Conferenti.pro_cod_istat AS pro_cod_istat_Conferente,  ")
            StbSQL.AppendLine("         Indirizzi_Conferenti.com_cod_istat AS com_cod_istat_Conferente, Istat_Conferenti.LOCALITA AS localita_Conferente, Istat_Conferenti.COMUNI_PROV AS comuni_prov_Conferente, ")
            StbSQL.AppendLine("         Mov_Accett.Cod_RisUm_Altro,  ")
            StbSQL.AppendLine("         Risorse_Umane_Coop.Cod_Rapporto AS Cod_Rapporto_Coop, Contatti_Coop.Cod_Contatto AS Cod_Contatto_Coop,  ")
            StbSQL.AppendLine("         Contatti_Coop.Rag_Soc AS Rag_Soc_Coop, ISNULL(Contatti_Coop.Codice_Fiscale, '') AS Codice_Fiscale_Coop,  ")
            StbSQL.AppendLine("         Contatti_Coop.Nome AS Nome_Coop, Contatti_Coop.Cognome AS Cognome_Coop,  ")
            StbSQL.AppendLine("         ImpreseXIndirizzi_Coop.Cod_Indirizzo AS Cod_Indirizzo_Coop, Indirizzi_Coop.ind_des AS ind_des_Coop, Indirizzi_Coop.frz_des AS frz_des_Coop, Indirizzi_Coop.CAP AS cap_Coop, ")
            StbSQL.AppendLine("         Indirizzi_Coop.stato AS stato_Coop, Indirizzi_Coop.pro_cod_istat AS pro_cod_istat_Coop,  ")
            StbSQL.AppendLine("         Indirizzi_Coop.com_cod_istat AS com_cod_istat_Coop, Istat_Coop.LOCALITA AS localita_Coop, Istat_Coop.COMUNI_PROV AS comuni_prov_Coop, ")
            StbSQL.AppendLine("         Mov_Accett.Extra_Int,  ")
            StbSQL.AppendLine("         Risorse_Umane_Coop_2.Cod_Rapporto AS Cod_Rapporto_Coop_2, Contatti_Coop_2.Cod_Contatto AS Cod_Contatto_Coop_2,  ")
            StbSQL.AppendLine("         Contatti_Coop_2.Rag_Soc AS Rag_Soc_Coop_2, ISNULL(Contatti_Coop_2.Codice_Fiscale, '') AS Codice_Fiscale_Coop_2,  ")
            StbSQL.AppendLine("         Contatti_Coop_2.Nome AS Nome_Coop_2, Contatti_Coop_2.Cognome AS Cognome_Coop_2,  ")
            StbSQL.AppendLine("         ImpreseXIndirizzi_Coop_2.Cod_Indirizzo AS Cod_Indirizzo_Coop_2, Indirizzi_Coop_2.ind_des AS ind_des_Coop_2, Indirizzi_Coop_2.frz_des AS frz_des_Coop_2, Indirizzi_Coop_2.CAP AS cap_Coop_2, ")
            StbSQL.AppendLine("         Indirizzi_Coop_2.stato AS stato_Coop_2, Indirizzi_Coop_2.pro_cod_istat AS pro_cod_istat_Coop_2,  ")
            StbSQL.AppendLine("         Indirizzi_Coop_2.com_cod_istat AS com_cod_istat_Coop_2, Istat_Coop_2.LOCALITA AS localita_Coop_2, Istat_Coop_2.COMUNI_PROV AS comuni_prov_Coop_2, ")
            StbSQL.AppendLine("         Mov_Accett.Cod_Destinazione, ")
            StbSQL.AppendLine("         Risorse_Umane_Produttori.Cod_Rapporto AS Cod_Rapporto_Produttore, Contatti_Produttori.Cod_Contatto AS Cod_Contatto_Produttore,  ")
            StbSQL.AppendLine("         Contatti_Produttori.Rag_Soc AS Rag_Soc_Produttore, ISNULL(Contatti_Produttori.Codice_Fiscale, '') AS Codice_Fiscale_Produttore,  ")
            StbSQL.AppendLine("         Contatti_Produttori.Nome AS Nome_Produttore, Contatti_Produttori.Cognome AS Cognome_Produttore,  ")
            StbSQL.AppendLine("         Mov_Accett.Cod_IndirizzoDestinazione, Indirizzi_Produttori.ind_des AS ind_des_Produttore, Indirizzi_Produttori.frz_des AS frz_des_Produttore, Indirizzi_Produttori.CAP AS cap_Produttore,  ")
            StbSQL.AppendLine("         Indirizzi_Produttori.stato AS stato_Produttore, Indirizzi_Produttori.pro_cod_istat AS pro_cod_istat_Produttore,  ")
            StbSQL.AppendLine("         Indirizzi_Produttori.com_cod_istat AS com_cod_istat_Produttore, Istat_Produttori.LOCALITA AS localita_Produttore, Istat_Produttori.COMUNI_PROV AS comuni_prov_Produttore, ")
            StbSQL.AppendLine("")
            StbSQL.AppendLine("         Mov_Accett.Mezzo, Mov_Accett.Cod_Vettore, Mov_Accett.Cod_IndirizzoVettore,  ")
            StbSQL.AppendLine("         Risorse_Umane_Vettore.Cod_Rapporto AS Cod_Rapporto_Produttore, Contatti_Vettore.Cod_Contatto AS Cod_Contatto_Vettore,  ")
            StbSQL.AppendLine("         Contatti_Vettore.Rag_Soc AS Rag_Soc_Vettore, ISNULL(Contatti_Vettore.Codice_Fiscale, '') AS Codice_Fiscale_Vettore,  ")
            StbSQL.AppendLine("         Contatti_Vettore.Nome AS Nome_Vettore, Contatti_Vettore.Cognome AS Cognome_Vettore,  ")
            StbSQL.AppendLine("         Mov_Accett.Cod_IndirizzoVettore, Indirizzi_Vettore.ind_des AS ind_des_Vettore, Indirizzi_Vettore.frz_des AS frz_des_Vettore, Indirizzi_Vettore.CAP AS cap_Vettore,  ")
            StbSQL.AppendLine("         Indirizzi_Vettore.stato AS stato_Vettore, Indirizzi_Vettore.pro_cod_istat AS pro_cod_istat_Vettore,  ")
            StbSQL.AppendLine("         Indirizzi_Vettore.com_cod_istat AS com_cod_istat_Vettore, Istat_Vettore.LOCALITA AS localita_Vettore, Istat_Vettore.COMUNI_PROV AS comuni_prov_Vettore, ")
            StbSQL.AppendLine("")
            StbSQL.AppendLine("         Mov_Conf.Id_Mov AS Id_Mov_Conf, Mov_Conf.Cau_Mov AS Cau_Mov_Conf, Mov_Conf.Mov_Desc AS Mov_Desc_Conf,  ")
            StbSQL.AppendLine("         Mov_Conf.Data_Movimento AS Data_Conf, Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf,  ")
            StbSQL.AppendLine("         Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf,  Mov_Conf.Colli, ")
            StbSQL.AppendLine("")
            StbSQL.AppendLine("         Mov_Raccolta.Id_Mov AS Id_Mov_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Raccolta.Cau_Mov AS Cau_Mov_Raccolta, Mov_Raccolta.Mov_Desc AS Mov_Desc_Raccolta, Mov_Raccolta.Data_Movimento AS Data_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Raccolta.Ora AS Ora_Raccolta, C_Az_Raccolta.sa_cod AS Sa_Cod_Raccolta, Fabbr_Raccolta.Fabbricato_Cod AS Fabbricato_Cod_Raccolta, ")
            StbSQL.AppendLine("         C_Az_Raccolta.sa_nome AS Sa_Nome_Raccolta, Fabbr_Raccolta.Fabbricato_Des AS Fabbricato_Des_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Id_Mov_Det AS Id_Mov_Det_Raccolta, Mov_Dest_Raccolta.Id_Destinazione AS Id_Destinazione_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Mov_Det_Des AS Mov_Det_Des_Raccolta, Mov_Dett_Raccolta.Elem_Cod AS Elem_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Pro_Cod AS Pro_Cod_Raccolta, Mov_Dett_Raccolta.Mat_Cod AS Mat_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Cod_Progetto AS Cod_Progetto_Raccolta, Mov_Dett_Raccolta.Fase_Cod AS Fase_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Qta AS Qta_Raccolta, UDM_Qta.Udm_Sim AS Udm_Sim_Raccolta, UDM_Qta.Udm_des AS Udm_des_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Listino_Cod AS Listino_Cod_Raccolta, Mov_Dett_Raccolta.Tara AS Tara, Mov_Dett_Raccolta.Prezzo_Unitario,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Prezzo_Unitario_Netto, Mov_Dett_Raccolta.Imponibile, Mov_Dett_Raccolta.Imponibile_Netto,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Jolly_Int AS Jolly_Int_Raccolta, Mov_Dett_Raccolta.Contabilizzato AS Contabilizzato_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Pendente AS Pendente_Raccolta, Mov_Dett_Raccolta.Prezzo_Effettivo, Mov_Dett_Raccolta.ChkLayOut_Hide, ")
            StbSQL.AppendLine("         MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta,  ")
            StbSQL.AppendLine("         MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, MP_Raccolta.Cul_Cod, MP_Raccolta.Veg_Cod, MP_Raccolta.GRVA_COD_VEG,  ")
            StbSQL.AppendLine("         MP_Raccolta.Regolamento, MP_Raccolta.ChkListino ")
            StbSQL.AppendLine(" , ISNULL( (SELECT TOP 1 ISNULL( materie_prime_calibri.cal_des , ' ') AS Calibro ")
            StbSQL.AppendLine("         FROM materie_prime_calibri  ")
            StbSQL.AppendLine("         INNER JOIN Materie_Prime_Campionature MP_Camp_Calibro ON MP_Camp_Calibro.tipo_cod =  materie_prime_calibri.cal_cod ")
            StbSQL.AppendLine("         WHERE MP_Camp_Calibro.progressivo = Mov_Dett_Raccolta.cal_cod  ")
            StbSQL.AppendLine("         AND MP_Camp_Calibro.tipo = 'calibro'), 0 ) AS Calibro ")
            StbSQL.AppendLine(" , ISNULL( (SELECT TOP 1 VAL_COD  ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature MP_Camp_Indice  ")
            StbSQL.AppendLine("             WHERE MP_Camp_Indice.progressivo = Mov_Dett_Raccolta.cal_cod  ")
            StbSQL.AppendLine("             AND MP_Camp_Indice.tipo = 'indice' ")
            'StbSQL.AppendLine("             AND MP_Camp_Indice.tipo_cod = " & CStr(FRUTTAGEL_INDMATCOD_PUNTEGGIO) & " ), 0 ) AS Punteggio ")
            StbSQL.AppendLine("             AND MP_Camp_Indice.tipo_cod IN ( " & CStr(FRUTTAGEL_INDMATCOD_PUNTEGGIO) & ", " & CStr(FRUTTAGEL_INDMATCOD_GRADOTEND) & ", " & CStr(FRUTTAGEL_INDMATCOD_GRADOBRIX) & " )  ), 0 ) AS Punteggio ")
            StbSQL.AppendLine("")
            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" INNER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Accett.PIVA  ")
            StbSQL.AppendLine(" AND Sequenza_Progressivi.Anno = YEAR(Mov_Accett.Data_Movimento)  ")
            StbSQL.AppendLine(" AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Accett.Doc_Numero_Sin  ")
            StbSQL.AppendLine(" AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Accett.Doc_Numero_Des  ")
            StbSQL.AppendLine("")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")
            StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Conferenti ON  Mov_Accett.Cod_IndirizzoRisUm = Indirizzi_Conferenti.cod_indirizzo ")
            StbSQL.AppendLine(" INNER JOIN ISTAT Istat_Conferenti ON Indirizzi_Conferenti.pro_cod_istat = Istat_Conferenti.PROV AND Indirizzi_Conferenti.com_cod_istat = Istat_Conferenti.COM ")
            StbSQL.AppendLine("")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Coop ON Risorse_Umane_Coop.Piva = Contatti_Coop.Piva AND Risorse_Umane_Coop.Cod_Contatto = Contatti_Coop.Cod_Contatto  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN ImpreseXIndirizzi ImpreseXIndirizzi_Coop ON  Contatti_Coop.Cod_Contatto = ImpreseXIndirizzi_Coop.Piva ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Coop ON  ImpreseXIndirizzi_Coop.Cod_Indirizzo = Indirizzi_Coop.cod_indirizzo ")
            StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Coop ON Indirizzi_Coop.pro_cod_istat = Istat_Coop.PROV AND Indirizzi_Coop.com_cod_istat = Istat_Coop.COM ")
            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop_2 ON Mov_Accett.Extra_Int = Risorse_Umane_Coop_2.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Coop_2 ON Risorse_Umane_Coop_2.Piva = Contatti_Coop_2.Piva AND Risorse_Umane_Coop_2.Cod_Contatto = Contatti_Coop_2.Cod_Contatto  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN ImpreseXIndirizzi ImpreseXIndirizzi_Coop_2 ON  Contatti_Coop_2.Cod_Contatto = ImpreseXIndirizzi_Coop_2.Piva ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Coop_2 ON  ImpreseXIndirizzi_Coop_2.Cod_Indirizzo = Indirizzi_Coop_2.cod_indirizzo ")
            StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Coop_2 ON Indirizzi_Coop_2.pro_cod_istat = Istat_Coop_2.PROV AND Indirizzi_Coop_2.com_cod_istat = Istat_Coop_2.COM ")
            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Produttori ON  Mov_Accett.Cod_IndirizzoDestinazione = Indirizzi_Produttori.cod_indirizzo ")
            StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Produttori ON Indirizzi_Produttori.pro_cod_istat = Istat_Produttori.PROV AND Indirizzi_Produttori.com_cod_istat = Istat_Produttori.COM ")
            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Vettore ON Mov_Accett.Cod_Vettore = Risorse_Umane_Vettore.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Vettore ON Risorse_Umane_Vettore.Piva = Contatti_Vettore.Piva AND Risorse_Umane_Vettore.Cod_Contatto = Contatti_Vettore.Cod_Contatto  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Vettore ON  Mov_Accett.Cod_IndirizzoVettore = Indirizzi_Vettore.cod_indirizzo ")
            StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Vettore ON Indirizzi_Vettore.pro_cod_istat = Istat_Vettore.PROV AND Indirizzi_Vettore.com_cod_istat = Istat_Vettore.COM ")
            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda ")

            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")

            StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det ")
            StbSQL.AppendLine(" INNER JOIN Centri_Aziendali C_Az_Raccolta ON Mov_Dett_Raccolta.PIVA = C_Az_Raccolta.PIVA AND Mov_Dett_Raccolta.Sa_Cod = C_Az_Raccolta.sa_cod ")
            StbSQL.AppendLine(" INNER JOIN Fabbricati Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Fabbricato_Cod ")
            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" INNER JOIN UnitaMisura UDM_Qta ON Mov_Dett_Raccolta.Udm_Cod = UDM_Qta.Udm_Cod ")
            StbSQL.AppendLine(" ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "")

            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov      = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'")
            StbSQL.AppendLine(" AND Mov_Conf.Cau_Mov        = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) & "'")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov    = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'")

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StbSQL.AppendLine(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "  ")

            StbSQL.AppendLine(" AND Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(CStr(objParametri.PivaSuperUser)) & "'  ")
            StbSQL.AppendLine(" AND Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(CStr(enum_SequenzaProgressiviTipi.BolleAccettDaDiversi)) & "  ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.AppendLine(" ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge gli eventuali imballi in entrata specificati nella bolla.
    ''' Attualmente chiamata da Carica_DSImballaggi_StampaBollaECertificato (utilizzata nella stampa bolla e nella stampa del certificato)
    ''' default: Flag_LeggiCentro As Boolean = False, Flag_LeggiFabbricato As Boolean = False
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function ImballiEntrata_Leggi(ByVal Piva As String,
                                         ByVal Id_Agenda As Integer,
                                         ByVal Flag_LeggiCentro As Boolean,
                                         ByVal Flag_LeggiFabbricato As Boolean,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.ImballiEntrata_Leggi"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT  Mov_ImballiEntrata.Cau_Mov AS Cau_Mov_ImballiEntrata,  ")
            StbSQL.AppendLine("         Mov_ImballiEntrata.Mov_Desc AS Mov_Desc_ImballiEntrata, Mov_ImballiEntrata.Data_Movimento AS Data_ImballiEntrata,  ")
            StbSQL.AppendLine("         Mov_ImballiEntrata.Doc_Numero AS Doc_Numero_ImballiEntrata, Mov_ImballiEntrata.Doc_Numero_Des AS Doc_Numero_Des_ImballiEntrata, ")
            StbSQL.AppendLine("         Mov_ImballiEntrata.Doc_Numero_Sin AS Doc_Numero_Sin_ImballiEntrata, Mov_ImballiEntrata.Colli AS Colli_ImballiEntrata, Mov_ImballiEntrata.Tara_Imballi AS Tara_Imballi_Entrata, ")
            StbSQL.AppendLine("         Mov_Dett_ImballiEntrata.Id_Mov_Det AS Id_Mov_Det_ImballiEntrata,   ")
            StbSQL.AppendLine("         Mov_Dett_ImballiEntrata.Mov_Det_Des AS Mov_Det_Des_ImballiEntrata, Mov_Dett_ImballiEntrata.Elem_Cod AS Elem_Cod_ImballiEntrata,  ")
            StbSQL.AppendLine("         Mov_Dett_ImballiEntrata.Pro_Cod AS Pro_Cod_ImballiEntrata, Mov_Dett_ImballiEntrata.Mat_Cod AS Mat_Cod_ImballiEntrata,  ")
            StbSQL.AppendLine("         Mov_Dett_ImballiEntrata.Cod_Progetto AS Cod_Progetto_ImballiEntrata, Mov_Dett_ImballiEntrata.Fase_Cod AS Fase_Cod_ImballiEntrata,  ")
            StbSQL.AppendLine("         Mov_Dett_ImballiEntrata.Lotto AS Lotto_ImballiEntrata, Mov_Dett_ImballiEntrata.Cal_Cod AS Cal_Cod_ImballiEntrata,  ")
            StbSQL.AppendLine("         Mov_Dett_ImballiEntrata.Udm_Cod AS Udm_Cod_ImballiEntrata, Mov_Dett_ImballiEntrata.Qta AS Qta_ImballiEntrata,  ")
            StbSQL.AppendLine("         Mov_Dett_ImballiEntrata.QTA_EXTRA AS Qta_Extra_ImballiEntrata, Mov_Dett_ImballiEntrata.UDM_COD_EXTRA AS Udm_Cod_Extra_ImballiEntrata,  ")
            StbSQL.AppendLine("         Mov_Dett_ImballiEntrata.Qta_Extra_Totale AS Qta_Extra_Totale_ImballiEntrata, Mov_Dett_ImballiEntrata.Tara AS Tara_ImballiEntrata,  ")
            StbSQL.AppendLine("         Mov_Dett_ImballiEntrata.Contabilizzato AS Contabilizzato_ImballiEntrata, Mov_Dett_ImballiEntrata.Pendente AS Pendente_ImballiEntrata,  ")
            StbSQL.AppendLine("         Mov_Dett_ImballiEntrata.Jolly_Int AS Jolly_Int_ImballiEntrata,   ")
            StbSQL.AppendLine("         MP_Imballi.Cod_Articolo AS Cod_Articolo_ImballiEntrata, MP_Imballi.Mat_Des AS Mat_Des_ImballiEntrata, MP_Imballi.Udm_Cod_Extra AS Udm_Cod_Extra_ImballiEntrata,  ")
            StbSQL.AppendLine("         MP_Imballi.Flag_Extra AS Flag_Extra_ImballiEntrata, MP_Imballi.Qta_Extra AS Qta_Extra_MP_ImballiEntrata, MP_Imballi.ChkImballaggio ")

            If Flag_LeggiCentro Then
                StbSQL.AppendLine(" , C_Az_Imballi.sa_cod AS sa_cod_ImballiEntrata, C_Az_Imballi.sa_nome AS sa_nome_ImballiEntrata ")
            End If

            If Flag_LeggiFabbricato Then
                StbSQL.AppendLine(" , Mov_Dest_ImballiEntrata.Id_Destinazione AS Id_Destinazione_ImballiEntrata ")
                StbSQL.AppendLine(" , Fabbr_Imballi.Fabbricato_Cod AS Fabbricato_Cod_ImballiEntrata, Fabbr_Imballi.Fabbricato_Des AS Fabbricato_Des_ImballiEntrata ")
            End If

            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" FROM Movimenti Mov_ImballiEntrata ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_ImballiEntrata ON Mov_ImballiEntrata.PIVA = Mov_Dett_ImballiEntrata.PIVA AND Mov_ImballiEntrata.Id_Agenda = Mov_Dett_ImballiEntrata.Id_Agenda AND Mov_ImballiEntrata.Id_Mov = Mov_Dett_ImballiEntrata.Id_Mov  ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Imballi ON Mov_Dett_ImballiEntrata.Elem_Cod = MP_Imballi.Elem_Cod AND Mov_Dett_ImballiEntrata.Mat_Cod = MP_Imballi.Mat_Cod  ")

            If Flag_LeggiCentro Then
                StbSQL.AppendLine(" INNER JOIN Centri_Aziendali C_Az_Imballi ON Mov_Dett_ImballiEntrata.PIVA = C_Az_Imballi.PIVA AND Mov_Dett_ImballiEntrata.Sa_Cod = C_Az_Imballi.sa_cod ")
            End If

            If Flag_LeggiFabbricato Then
                StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_ImballiEntrata ON Mov_Dett_ImballiEntrata.PIVA = Mov_Dest_ImballiEntrata.Piva AND Mov_Dett_ImballiEntrata.Sa_Cod = Mov_Dest_ImballiEntrata.Sa_Cod AND  Mov_Dett_ImballiEntrata.Id_Agenda = Mov_Dest_ImballiEntrata.Id_Agenda AND Mov_Dett_ImballiEntrata.Id_Mov = Mov_Dest_ImballiEntrata.Id_Mov AND Mov_Dett_ImballiEntrata.Id_Mov_Det = Mov_Dest_ImballiEntrata.Id_Mov_Det ")
                StbSQL.AppendLine(" INNER JOIN Fabbricati Fabbr_Imballi ON Mov_Dest_ImballiEntrata.Piva = Fabbr_Imballi.PIVA AND Mov_Dest_ImballiEntrata.Sa_Cod = Fabbr_Imballi.SA_COD AND Mov_Dest_ImballiEntrata.Id_Destinazione = Fabbr_Imballi.Fabbricato_Cod  ")
            End If

            StbSQL.AppendLine(" WHERE   Mov_ImballiEntrata.Cau_Mov = '" & CAU_CARICO & "'")

            '14/02/2020: aggiunto filtro per escludere i trasformati vegetali (nella nuova versione del conferimento vengono agganciati al cau_carico 7300)
            '20/01/2021: ora potrebbero esserci anche i trasformati animali ==> beni confezionamento vegetali ed animali
            StbSQL.AppendLine(" AND    Mov_Dett_ImballiEntrata.Elem_Cod IN (" & BENI_CONFEZ_VEGETALE & "," & BENI_CONFEZ_ANIMALE & ")")

            If Piva <> "" Then
                StbSQL.AppendLine(" AND Mov_ImballiEntrata.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Id_Agenda <> 0 Then
                StbSQL.AppendLine(" AND Mov_ImballiEntrata.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.AppendLine(" ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge gli eventuali imballi in uscita specificati nel ddt emesso agganciato alla bolla
    ''' Attualmente chiamata da Carica_DSImballaggi_StampaBollaECertificato (utilizzata nella stampa bolla e nella stampa del certificato)
    ''' Default:
    ''' ByVal Flag_LeggiSequenzaProgressivi As Boolean = False
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function ImballiUscita_Leggi(ByVal Piva_Rif As String,
                                        ByVal Sa_Cod_Rif As Integer,
                                        ByVal Id_Agenda_Rif As Integer,
                                        ByVal Lav_Cod_Rif As Integer,
                                        ByVal Flag_LeggiSequenzaProgressivi As Boolean,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.ImballiUscita_Leggi"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT  Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, Mov_Dettagli_Riferimenti.Id_Agenda_Rif, ")
            StbSQL.AppendLine("         Mov_Dettagli_Riferimenti.Lav_Cod_Rif, Agenda_Accettazione.des_lib AS Des_Lib_Rif, Mov_Dettagli_Riferimenti.Cau_Mov_Rif,  ")
            StbSQL.AppendLine("         Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, Mov_Dettagli_Riferimenti.Id_Agenda, Mov_Dettagli_Riferimenti.Lav_Cod, ")
            StbSQL.AppendLine("         Agenda_ImballiUscita.des_lib, Mov_Dettagli_Riferimenti.Cau_Mov, ")

            StbSQL.AppendLine("         MovimentiCont_ImballiUscita.Data_Movimento,  ")
            StbSQL.AppendLine("         MovimentiCont_ImballiUscita.Doc_Numero_Sin, MovimentiCont_ImballiUscita.Doc_Numero, MovimentiCont_ImballiUscita.Doc_Numero_Des, ")

            StbSQL.AppendLine("         Movimenti_dettagli_ImballiUscita.Id_Mov_Det, ")
            StbSQL.AppendLine("         Movimenti_dettagli_ImballiUscita.Elem_Cod, Movimenti_dettagli_ImballiUscita.Pro_Cod, Movimenti_dettagli_ImballiUscita.Mat_Cod,  ")
            StbSQL.AppendLine("         Movimenti_dettagli_ImballiUscita.Mov_Det_Des, Movimenti_dettagli_ImballiUscita.Udm_Cod, Movimenti_dettagli_ImballiUscita.Cod_Progetto,  ")
            StbSQL.AppendLine("         Movimenti_dettagli_ImballiUscita.Fase_Cod, Movimenti_dettagli_ImballiUscita.Contabilizzato, Movimenti_dettagli_ImballiUscita.Pendente,  ")
            StbSQL.AppendLine("         Movimenti_dettagli_ImballiUscita.Cal_Cod, Movimenti_dettagli_ImballiUscita.Lotto, Movimenti_dettagli_ImballiUscita.Qta, Movimenti_dettagli_ImballiUscita.Jolly_Int, ")
            StbSQL.AppendLine("         Movimenti_dettagli_ImballiUscita.UDM_COD_EXTRA, Movimenti_dettagli_ImballiUscita.QTA_EXTRA,  ")
            StbSQL.AppendLine("         Movimenti_dettagli_ImballiUscita.Qta_Extra_Totale, Movimenti_dettagli_ImballiUscita.Tara, Mov_Destinazioni_ImballiUscita.Id_Destinazione, ")
            StbSQL.AppendLine("         Fabbricati_ImballiUscita.Fabbricato_Des, Centri_Aziendali_ImballiUscita.sa_nome, Centri_Aziendali_ImballiUscita.sa_cod AS sa_cod_ImballiUscita,  ")
            StbSQL.AppendLine("         MP_ImballiUscita.Cod_Articolo, MP_ImballiUscita.Mat_Des, MP_ImballiUscita.Udm_Cod_Extra AS udm_cod_extra_MP_ImballiUscita,  ")
            StbSQL.AppendLine("         MP_ImballiUscita.Flag_Extra, MP_ImballiUscita.ChkImballaggio, MP_ImballiUscita.Qta_Extra AS Qta_extra_MP_ImballiUscita ")
            StbSQL.AppendLine("")
            If Flag_LeggiSequenzaProgressivi Then
                StbSQL.AppendLine("     , Sequenza_Progressivi.Doc_Numero_Iniziale, Sequenza_Progressivi.Doc_Numero_UltimoValore,  ")
                StbSQL.AppendLine("     Sequenza_Progressivi.Lunghezza_Sin, Sequenza_Progressivi.Lunghezza_Centro, Sequenza_Progressivi.Lunghezza_Des, ")
                StbSQL.AppendLine("     Sequenza_Progressivi.CarattereFormattazione ")
            End If
            StbSQL.AppendLine("")
            StbSQL.AppendLine(" FROM        Mov_Dettagli_Riferimenti ")
            StbSQL.AppendLine(" INNER JOIN  Agenda Agenda_Accettazione ON Mov_Dettagli_Riferimenti.Piva_Rif = Agenda_Accettazione.PIVA  ")
            StbSQL.AppendLine("             AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = Agenda_Accettazione.Sa_Cod  ")
            StbSQL.AppendLine("             AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Agenda_Accettazione.Id_Agenda  ")
            StbSQL.AppendLine(" INNER JOIN  Agenda Agenda_ImballiUscita ON Mov_Dettagli_Riferimenti.Piva = Agenda_ImballiUscita.PIVA  ")
            StbSQL.AppendLine("             AND Mov_Dettagli_Riferimenti.Sa_Cod = Agenda_ImballiUscita.Sa_Cod  ")
            StbSQL.AppendLine("             AND Mov_Dettagli_Riferimenti.Id_Agenda = Agenda_ImballiUscita.Id_Agenda  ")

            StbSQL.AppendLine(" INNER JOIN  Movimenti MovimentiCont_ImballiUscita ON Agenda_ImballiUscita.PIVA = MovimentiCont_ImballiUscita.PIVA          ")
            StbSQL.AppendLine("             AND Agenda_ImballiUscita.Id_Agenda = MovimentiCont_ImballiUscita.Id_Agenda ")

            StbSQL.AppendLine(" INNER JOIN  Movimenti Movimenti_ImballiUscita ON Agenda_ImballiUscita.PIVA = Movimenti_ImballiUscita.PIVA  ")
            StbSQL.AppendLine("             AND Agenda_ImballiUscita.Id_Agenda = Movimenti_ImballiUscita.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN  Movimenti_dettagli Movimenti_dettagli_ImballiUscita ON Movimenti_ImballiUscita.PIVA = Movimenti_dettagli_ImballiUscita.PIVA  ")
            StbSQL.AppendLine("             AND Movimenti_ImballiUscita.Id_Agenda = Movimenti_dettagli_ImballiUscita.Id_Agenda  ")
            StbSQL.AppendLine("             AND Movimenti_ImballiUscita.Id_Mov = Movimenti_dettagli_ImballiUscita.Id_Mov  ")
            StbSQL.AppendLine(" INNER JOIN  Mov_Destinazioni Mov_Destinazioni_ImballiUscita ON Movimenti_dettagli_ImballiUscita.PIVA = Mov_Destinazioni_ImballiUscita.Piva  ")
            StbSQL.AppendLine("             AND Movimenti_dettagli_ImballiUscita.Sa_Cod = Mov_Destinazioni_ImballiUscita.Sa_Cod  ")
            StbSQL.AppendLine("             AND Movimenti_dettagli_ImballiUscita.Id_Agenda = Mov_Destinazioni_ImballiUscita.Id_Agenda  ")
            StbSQL.AppendLine("             AND Movimenti_dettagli_ImballiUscita.Id_Mov = Mov_Destinazioni_ImballiUscita.Id_Mov  ")
            StbSQL.AppendLine("             AND Movimenti_dettagli_ImballiUscita.Id_Mov_Det = Mov_Destinazioni_ImballiUscita.Id_Mov_Det  ")
            StbSQL.AppendLine(" INNER JOIN  Fabbricati Fabbricati_ImballiUscita ON Mov_Destinazioni_ImballiUscita.Piva = Fabbricati_ImballiUscita.PIVA  ")
            StbSQL.AppendLine("             AND Mov_Destinazioni_ImballiUscita.Sa_Cod = Fabbricati_ImballiUscita.SA_COD  ")
            StbSQL.AppendLine("             AND Mov_Destinazioni_ImballiUscita.Id_Destinazione = Fabbricati_ImballiUscita.Fabbricato_Cod ")
            StbSQL.AppendLine(" INNER JOIN  Centri_Aziendali Centri_Aziendali_ImballiUscita ON Fabbricati_ImballiUscita.PIVA = Centri_Aziendali_ImballiUscita.PIVA ")
            StbSQL.AppendLine("             AND Fabbricati_ImballiUscita.SA_COD = Centri_Aziendali_ImballiUscita.sa_cod  ")
            StbSQL.AppendLine(" INNER JOIN  Materie_Prime MP_ImballiUscita ON Movimenti_dettagli_ImballiUscita.Elem_Cod = MP_ImballiUscita.Elem_Cod ")
            StbSQL.AppendLine("             AND Movimenti_dettagli_ImballiUscita.Mat_Cod = MP_ImballiUscita.Mat_Cod ")
            StbSQL.AppendLine("")
            If Flag_LeggiSequenzaProgressivi Then
                StbSQL.AppendLine(" INNER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = MovimentiCont_ImballiUscita.PIVA ")
                StbSQL.AppendLine("         AND  Sequenza_Progressivi.Anno = YEAR(MovimentiCont_ImballiUscita.Data_Movimento)  ")
                StbSQL.AppendLine("         AND Sequenza_Progressivi.Doc_Numero_Sin = MovimentiCont_ImballiUscita.Doc_Numero_Sin ")
                StbSQL.AppendLine("         AND Sequenza_Progressivi.Doc_Numero_Des = MovimentiCont_ImballiUscita.Doc_Numero_Des  ")
            End If
            StbSQL.AppendLine("")
            StbSQL.AppendLine(" WHERE       Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(LAVCOD_ACCETTAZIONE_DIVERSI) & "  ")
            StbSQL.AppendLine(" AND         Mov_Dettagli_Riferimenti.Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_BOLLA_EMESSA) & " ")
            StbSQL.AppendLine(" AND		    MovimentiCont_ImballiUscita.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "' ")

            If Piva_Rif <> "" Then
                StbSQL.AppendLine(" AND     Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(Piva_Rif) & "'  ")
            End If

            If Sa_Cod_Rif <> 0 Then
                StbSQL.AppendLine(" AND     Mov_Dettagli_Riferimenti.Sa_Cod_Rif = " & Agro_SQL_SaveNum(Sa_Cod_Rif) & "  ")
            End If

            If Id_Agenda_Rif <> 0 Then
                StbSQL.AppendLine(" AND     Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Rif) & "  ")
            End If

            If Flag_LeggiSequenzaProgressivi Then
                StbSQL.AppendLine(" AND     Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
                StbSQL.AppendLine(" AND     Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(CStr(enum_SequenzaProgressiviTipi.DDTEmessi)) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.AppendLine(" ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge gli impianti colturali (per la tracciabilità) legati alla raccolta sul produttore, che è agganciata con i riferimenti alla bolla
    ''' Attualmente usata da: esportazione bolle ad altro cliente gias, esportazione excel bolle
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RifImpiantiDaRaccolta_Leggi(ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Id_Agenda As Integer,
                                                ByVal Piva_Rif As String,
                                                ByVal Sa_Cod_Rif As Integer,
                                                ByVal Id_Agenda_Rif As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.RifImpiantiDaRaccolta_Leggi"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT  Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, Mov_Dettagli_Riferimenti.Id_Agenda_Rif, ")
            StbSQL.AppendLine("         Mov_Dettagli_Riferimenti.Lav_Cod_Rif, Agenda_Accettazione.des_lib AS Des_Lib_Rif, Mov_Dettagli_Riferimenti.Cau_Mov_Rif,  ")
            StbSQL.AppendLine("         Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, Mov_Dettagli_Riferimenti.Id_Agenda, Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, ")

            StbSQL.AppendLine("         Agenda_RaccoltaImpianti.des_lib, Movimenti_RaccoltaImpianti.Data_Movimento,   ")
            StbSQL.AppendLine("         Movimenti_dettagli_RaccoltaImpianti.Id_Mov_Det,  ")
            StbSQL.AppendLine("         Movimenti_dettagli_RaccoltaImpianti.Elem_Cod, Movimenti_dettagli_RaccoltaImpianti.Pro_Cod, Movimenti_dettagli_RaccoltaImpianti.Mat_Cod,  ")
            StbSQL.AppendLine("         Movimenti_dettagli_RaccoltaImpianti.Mov_Det_Des, Movimenti_dettagli_RaccoltaImpianti.Udm_Cod, Movimenti_dettagli_RaccoltaImpianti.Cod_Progetto,  ")
            StbSQL.AppendLine("         Movimenti_dettagli_RaccoltaImpianti.Fase_Cod,   ")
            StbSQL.AppendLine("         Movimenti_dettagli_RaccoltaImpianti.Cal_Cod, Movimenti_dettagli_RaccoltaImpianti.Lotto, Movimenti_dettagli_RaccoltaImpianti.Qta, ")
            StbSQL.AppendLine("         Movimenti_dettagli_RaccoltaImpianti.UDM_COD_EXTRA, Movimenti_dettagli_RaccoltaImpianti.QTA_EXTRA,  ")
            StbSQL.AppendLine("         Movimenti_dettagli_RaccoltaImpianti.Qta_Extra_Totale, Movimenti_dettagli_RaccoltaImpianti.Tara,  ")

            StbSQL.AppendLine("         Mov_Destinazioni_RaccoltaImpianti.Appezza, Mov_Destinazioni_RaccoltaImpianti.Id_Destinazione, ")
            StbSQL.AppendLine("         Imprese_Progetti.Progetto_Nome, Reg_Impianti.Sup_Imp, Reg_Impianti.Cul_Cod, Cul_des, Reg_Impianti.Grfi_Cod, GruppoFinalita.Grfi_Des, Reg_Impianti.Grva_Cod_Veg, ISNULL(GruppoVarietale.Grva_Des, '') AS Grva_Des, Cultivar.Veg_Cod, veg_des, Reg_Impianti.Setup_Cod, ")
            StbSQL.AppendLine("         Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto, Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto, ")
            StbSQL.AppendLine("         Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta, Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta ")
            StbSQL.AppendLine("")
            StbSQL.AppendLine(" FROM        Mov_Dettagli_Riferimenti ")
            'l'accettazione sta in piva sa_cod lav_cod
            StbSQL.AppendLine(" INNER JOIN  Agenda Agenda_Accettazione ON Mov_Dettagli_Riferimenti.Piva = Agenda_Accettazione.PIVA  ")
            StbSQL.AppendLine("             AND Mov_Dettagli_Riferimenti.Sa_Cod = Agenda_Accettazione.Sa_Cod  ")
            StbSQL.AppendLine("             AND Mov_Dettagli_Riferimenti.Id_Agenda = Agenda_Accettazione.Id_Agenda  ")

            'la raccolta sta in piva_rif sa_cod_rif lav_cod_rif
            StbSQL.AppendLine(" INNER JOIN  Agenda Agenda_RaccoltaImpianti ON Mov_Dettagli_Riferimenti.Piva_Rif = Agenda_RaccoltaImpianti.PIVA  ")
            StbSQL.AppendLine("             AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = Agenda_RaccoltaImpianti.Sa_Cod  ")
            StbSQL.AppendLine("             AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Agenda_RaccoltaImpianti.Id_Agenda  ")
            StbSQL.AppendLine(" INNER JOIN  Movimenti Movimenti_RaccoltaImpianti ON Agenda_RaccoltaImpianti.PIVA = Movimenti_RaccoltaImpianti.PIVA  ")
            StbSQL.AppendLine("             AND Agenda_RaccoltaImpianti.Id_Agenda = Movimenti_RaccoltaImpianti.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN  Movimenti_dettagli Movimenti_dettagli_RaccoltaImpianti ON Movimenti_RaccoltaImpianti.PIVA = Movimenti_dettagli_RaccoltaImpianti.PIVA  ")
            StbSQL.AppendLine("             AND Movimenti_RaccoltaImpianti.Id_Agenda = Movimenti_dettagli_RaccoltaImpianti.Id_Agenda  ")
            StbSQL.AppendLine("             AND Movimenti_RaccoltaImpianti.Id_Mov = Movimenti_dettagli_RaccoltaImpianti.Id_Mov  ")
            StbSQL.AppendLine(" INNER JOIN  Mov_Destinazioni Mov_Destinazioni_RaccoltaImpianti ON Movimenti_dettagli_RaccoltaImpianti.PIVA = Mov_Destinazioni_RaccoltaImpianti.Piva  ")
            StbSQL.AppendLine("             AND Movimenti_dettagli_RaccoltaImpianti.Sa_Cod = Mov_Destinazioni_RaccoltaImpianti.Sa_Cod  ")
            StbSQL.AppendLine("             AND Movimenti_dettagli_RaccoltaImpianti.Id_Agenda = Mov_Destinazioni_RaccoltaImpianti.Id_Agenda  ")
            StbSQL.AppendLine("             AND Movimenti_dettagli_RaccoltaImpianti.Id_Mov = Mov_Destinazioni_RaccoltaImpianti.Id_Mov  ")
            StbSQL.AppendLine("             AND Movimenti_dettagli_RaccoltaImpianti.Id_Mov_Det = Mov_Destinazioni_RaccoltaImpianti.Id_Mov_Det  ")
            StbSQL.AppendLine(" INNER JOIN Reg_Impianti ON Reg_Impianti.PIVA = Mov_Destinazioni_RaccoltaImpianti.Piva  ")
            StbSQL.AppendLine("             AND Reg_Impianti.Sa_Cod = Mov_Destinazioni_RaccoltaImpianti.Sa_Cod  ")
            StbSQL.AppendLine("             AND Reg_Impianti.Appezza = Mov_Destinazioni_RaccoltaImpianti.Appezza  ")
            StbSQL.AppendLine("             AND Reg_Impianti.Id_Reg = Mov_Destinazioni_RaccoltaImpianti.Id_Destinazione ")
            StbSQL.AppendLine("")
            StbSQL.AppendLine(" INNER JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva ")
            StbSQL.AppendLine("             AND Reg_Impianti.Sa_Cod = Imprese_Progetti.Sa_Cod  ")
            StbSQL.AppendLine("             AND Reg_Impianti.Appezza = Imprese_Progetti.Appezza  ")
            StbSQL.AppendLine("             AND Reg_Impianti.Id_Reg = Imprese_Progetti.Id_reg ")
            StbSQL.AppendLine("")
            StbSQL.AppendLine(" INNER JOIN Cultivar ON Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod ")
            StbSQL.AppendLine(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StbSQL.AppendLine(" INNER JOIN GruppoFinalita ON Reg_Impianti.Grfi_Cod = GruppoFinalita.Grfi_Cod ")
            StbSQL.AppendLine(" LEFT OUTER JOIN GruppoVarietale ON Reg_Impianti.GRVA_Cod_VEG  = GruppoVarietale.Grva_Cod ")
            StbSQL.AppendLine("")

            StbSQL.AppendLine(" WHERE       (Mov_Dettagli_Riferimenti.Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_ACCETTAZIONE_DIVERSI) & ")  ")
            StbSQL.AppendLine(" AND         (Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(LAVCOD_RACCOLTA) & ") ")
            StbSQL.AppendLine(" AND	        Movimenti_RaccoltaImpianti.Data_Movimento >= Imprese_Progetti.Validita_Inizio ")
            StbSQL.AppendLine(" AND	        Movimenti_RaccoltaImpianti.Data_Movimento <= Imprese_Progetti.Validita_Fine ")

            If Piva <> "" Then
                StbSQL.AppendLine(" AND     Mov_Dettagli_Riferimenti.Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            End If

            If Sa_Cod <> 0 Then
                StbSQL.AppendLine(" AND     Mov_Dettagli_Riferimenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Id_Agenda <> 0 Then
                StbSQL.AppendLine(" AND     Mov_Dettagli_Riferimenti.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            End If

            If Piva_Rif <> "" Then
                StbSQL.AppendLine(" AND     Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(Piva_Rif) & "'  ")
            End If

            If Sa_Cod_Rif <> 0 Then
                StbSQL.AppendLine(" AND     Mov_Dettagli_Riferimenti.Sa_Cod_Rif = " & Agro_SQL_SaveNum(Sa_Cod_Rif) & "  ")
            End If

            If Id_Agenda_Rif <> 0 Then
                StbSQL.AppendLine(" AND     Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_Rif) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.AppendLine(" ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge le bolle di accettazione, in base al filtro impostato, per l'esportazione excel
    ''' Default:
    ''' Mat_Cod As Integer = 0, FiltroSpecie As String = "", FiltroConferenti As String = ""
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Bolle_XLS(ByVal Piva As String,
                              ByVal Sa_Cod As Integer,
                              ByVal Fabbricato_Cod As Integer,
                              ByVal Codice_Specie As String,
                              ByVal Codice_Conferente As String,
                              ByVal Piva_Produttore As String,
                              ByVal Piva_Coop1 As String,
                              ByVal Piva_Coop2 As String,
                              ByVal Data_Inizio As Date,
                              ByVal Data_Fine As Date,
                              ByVal Mat_Cod As Integer,
                              ByVal FiltroSpecie As String,
                              ByVal FiltroConferenti As String,
                              ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.Bolle_XLS"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine("SELECT   ")
            StbSQL.AppendLine(" Agenda.Id_Agenda, Fabbr_Raccolta.Fabbricato_Des AS Stabilimento, ")
            'StbSQL.AppendLine(" Agenda.des_lib, ")

            StbSQL.AppendLine(" Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, ")
            StbSQL.AppendLine(" Risorse_Umane_Conferenti.Cod_Contatto AS Piva_Conferente, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Conferenti.Rag_Soc, ' ') AS RagSoc_Conferente, ")

            StbSQL.AppendLine(" Risorse_Umane_Coop1.Cod_Contatto AS Piva_Coop1, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Coop1.Rag_Soc, ' ') AS RagSoc_Coop1, ")

            StbSQL.AppendLine(" Risorse_Umane_Coop2.Cod_Contatto AS Piva_Coop2, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Coop2.Rag_Soc, ' ') AS RagSoc_Coop2, ")

            StbSQL.AppendLine(" ISNULL(Contatti_Produttori.Cod_Contatto, ' ') AS Piva_Produttore, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Produttori.Rag_Soc, ' ') AS RagSoc_Produttore, ")

            StbSQL.AppendLine(" Mov_Accett.Peso, 0.0 AS Peso_Totale, Mov_Accett.Tara_Veicolo, 0.0 AS Peso_Lordo, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso, ")
            StbSQL.AppendLine(" 0.0 AS Peso_Netto, 0.0 AS Degrado_Perc, 0.0 AS Degrado, 0.0 AS Netto_Pagamento, ")
            StbSQL.AppendLine(" ISNULL( ")
            StbSQL.AppendLine(" ( ")
            StbSQL.AppendLine("     SELECT  Listino_Des ")
            StbSQL.AppendLine("     FROM    Listini_Prezzi ")
            StbSQL.AppendLine("     WHERE   Listini_Prezzi.Listino_Cod = Mov_Dett_Raccolta.Listino_Cod ")
            StbSQL.AppendLine("     AND     Listini_Prezzi.Piva = Mov_Dett_Raccolta.Piva ")
            StbSQL.AppendLine("     AND     Listini_Prezzi.Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StbSQL.AppendLine(" ) , '' ) AS Listino, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Prezzo_Unitario_Netto, ")

            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, '' AS Numero_Bolla, ")
            StbSQL.AppendLine(" Sequenza_Progressivi.Lunghezza_Sin, Sequenza_Progressivi.Lunghezza_Centro, Sequenza_Progressivi.Lunghezza_Des, Sequenza_Progressivi.CarattereFormattazione, ")
            StbSQL.AppendLine(" Mov_Accett.Data_Movimento AS Data_Bolla, ")

            StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf, ")
            StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf, '' AS Numero_Conf,  Mov_Conf.Data_Movimento AS Data_Conf,  ")

            StbSQL.AppendLine(" Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta, Mov_Dett_Raccolta.Tara AS Tara, ")
            StbSQL.AppendLine(" MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta, ' ' AS Descr_Specie, ' ' AS Descr_Varieta, ")
            StbSQL.AppendLine(" MP_Raccolta.Mat_Cod AS Codice_Prodotto, MP_Raccolta.Mat_Des AS Mat_Des_Raccolta ")
            StbSQL.AppendLine(" , ISNULL( (SELECT TOP 1 VAL_COD  ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature MP_Camp_Indice  ")
            StbSQL.AppendLine("             WHERE MP_Camp_Indice.progressivo = Mov_Dett_Raccolta.cal_cod  ")
            StbSQL.AppendLine("             AND MP_Camp_Indice.tipo = 'indice' ")
            'StbSQL.AppendLine("             AND MP_Camp_Indice.tipo_cod = " & CStr(FRUTTAGEL_INDMATCOD_PUNTEGGIO) & " ), 0 ) AS Punteggio ")
            StbSQL.AppendLine("             AND MP_Camp_Indice.tipo_cod IN ( " & CStr(FRUTTAGEL_INDMATCOD_PUNTEGGIO) & ", " & CStr(FRUTTAGEL_INDMATCOD_GRADOTEND) & ", " & CStr(FRUTTAGEL_INDMATCOD_GRADOBRIX) & " ) ), 0 ) AS Punteggio ")
            StbSQL.AppendLine(" , ISNULL( (SELECT TOP 1 ISNULL( materie_prime_calibri.cal_des , ' ') AS Calibro ")
            StbSQL.AppendLine("         FROM materie_prime_calibri  ")
            StbSQL.AppendLine("         INNER JOIN Materie_Prime_Campionature MP_Camp_Calibro ON MP_Camp_Calibro.tipo_cod =  materie_prime_calibri.cal_cod ")
            StbSQL.AppendLine("         WHERE MP_Camp_Calibro.progressivo = Mov_Dett_Raccolta.cal_cod  ")
            StbSQL.AppendLine("         AND MP_Camp_Calibro.tipo = 'calibro'), 0 ) AS Calibro_Des, ")

            StbSQL.AppendLine(" '0' AS A, '0' AS B, '0' AS C, '0' AS D, '0' AS CREMA, '0' AS FOGLIA, '0' AS S_SFALCIO, '0' AS SUB_STANDARD, '0' AS POMODORO,  ")
            StbSQL.AppendLine(" '0' AS _80, '0' AS _80_85, '0' AS _86_90, '0' AS _91_95, '0' AS _96_100, '0' AS _101_110, '0' AS _111_120, '0' AS _121_130, '0' AS _131_140, '0' AS _140_ ")

            StbSQL.AppendLine(", '' AS Str_Progetto_Nome, '' AS Str_Sup_Imp, '' AS Str_Veg_Des, '' AS Str_Cul_Des,  ")
            StbSQL.AppendLine(" '' AS Str_Grva_Des, '' AS Str_Grfi_Des, '' AS Str_Setup_Cod,  ")
            StbSQL.AppendLine(" '' AS Str_Validita_Inizio_Impianto, '' AS Str_Validita_Fine_Impianto, ")
            StbSQL.AppendLine(" '' AS Str_Validita_Inizio_Distinta, '' AS Str_Validita_Fine_Distinta ")

            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda  ")

            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")

            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  ")

            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop1 ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop1.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Coop1 ON Risorse_Umane_Coop1.Piva = Contatti_Coop1.Piva AND Risorse_Umane_Coop1.Cod_Contatto = Contatti_Coop1.Cod_Contatto  ")

            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop2 ON Mov_Accett.Extra_Int = Risorse_Umane_Coop2.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Coop2 ON Risorse_Umane_Coop2.Piva = Contatti_Coop2.Piva AND Risorse_Umane_Coop2.Cod_Contatto = Contatti_Coop2.Cod_Contatto  ")


            StbSQL.AppendLine(" INNER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Accett.PIVA ")
            StbSQL.AppendLine("             AND Sequenza_Progressivi.Anno = YEAR(Mov_Accett.Data_Movimento)  ")
            StbSQL.AppendLine("             AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Accett.Doc_Numero_Sin ")
            StbSQL.AppendLine("             AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Accett.Doc_Numero_Des  ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda  ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   ")
            StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det   ")
            StbSQL.AppendLine(" INNER JOIN Fabbricati Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Fabbricato_Cod ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")

            StbSQL.AppendLine(" WHERE   Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "")
            StbSQL.AppendLine(" AND     Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StbSQL.AppendLine(" AND     Mov_Accett.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'")
            StbSQL.AppendLine(" AND     Mov_Conf.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) & "'")
            StbSQL.AppendLine(" AND     Mov_Raccolta.Cau_Mov = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'")

            StbSQL.AppendLine(" AND     Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StbSQL.AppendLine(" AND     Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(CStr(enum_SequenzaProgressiviTipi.BolleAccettDaDiversi)) & "  ")

            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            StbSQL.AppendLine(" AND     Mov_Dest_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StbSQL.AppendLine(" AND     Mov_Dest_Raccolta.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.AppendLine(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' ")
            End If

            If FiltroConferenti <> "" Then
                StbSQL.AppendLine(FiltroConferenti)
            End If

            If Piva_Produttore <> "" Then
                StbSQL.AppendLine(" AND Risorse_Umane_Produttori.Cod_contatto = '" & Agro_SQL_SaveText(Piva_Produttore) & "' ")
            End If

            If Piva_Coop1 <> "" Then
                StbSQL.AppendLine(" AND Risorse_Umane_Coop1.Cod_contatto = '" & Agro_SQL_SaveText(Piva_Coop1) & "' ")
            End If

            If Piva_Coop2 <> "" Then
                StbSQL.AppendLine(" AND Risorse_Umane_Coop2.Cod_contatto = '" & Agro_SQL_SaveText(Piva_Coop2) & "' ")
            End If

            If Codice_Specie <> "" Then
                'se è stata selezionata una solo specie
                '(altrimenti c'è il filtro dopo)
                StbSQL.AppendLine(" AND MP_Raccolta.Cod_Articolo = '" & Agro_SQL_SaveText(Codice_Specie) & "' ")
            End If

            If FiltroSpecie <> "" Then
                StbSQL.AppendLine(FiltroSpecie)
            End If

            If Mat_Cod <> 0 Then
                StbSQL.AppendLine(" AND MP_Raccolta.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            End If

            'If Ordinamento = "" Then
            '    StbSQL.AppendLine(" ORDER BY Cod_Articolo_Raccolta, Mat_Des_Raccolta, Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, Mov_Accett.Data_Movimento ")
            'Else
            '    StbSQL.AppendLine(Ordinamento)
            'End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            'questo order by è fondamentale, non va cambiato
            StbSQL.AppendLine(" ORDER BY Cod_Articolo_Raccolta, Mat_Des_Raccolta, Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, Mov_Accett.Data_Movimento  ")


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge le bolle di accettazione, in base al filtro impostato, per l'esportazione excel
    ''' Default:
    ''' Sa_Cod As Integer = 0, Fabbricato_Cod = 0
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function TracciabilitaConferimenti_XLS(ByVal Piva As String,
                                                  ByVal Regolamento_Cod As enum_Cod_Regolamento,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal Fabbricato_Cod As Integer,
                                                  ByVal Data_Inizio As Date,
                                                  ByVal Data_Fine As Date,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As DataTable

        'ByVal Codice_Specie As String,
        'ByVal Codice_Conferente As String,
        'ByVal Piva_Produttore As String,
        'ByVal Piva_Coop1 As String,
        'ByVal Piva_Coop2 As String,
        'ByVal Mat_Cod As Integer,
        'ByVal FiltroSpecie As String,
        'ByVal FiltroConferenti As String,

        Const nomeRoutine = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.TracciabilitaConferimenti_XLS"

        Dim messaggioErrore As String = ""
        Dim StbSQLSelect As New Text.StringBuilder
        Dim StbSQLJoin As New Text.StringBuilder
        Dim StbSQLWhere As New Text.StringBuilder
        Dim StbSQLWhereImpianti As New Text.StringBuilder
        Dim StbSQLGenerale As New Text.StringBuilder
        Dim StbSQLJoinImpianti As New Text.StringBuilder
        Dim StbSQLNotExists As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQLSelect.Length = 0
            StbSQLJoin.Length = 0
            StbSQLWhere.Length = 0
            StbSQLGenerale.Length = 0
            StbSQLJoinImpianti.Length = 0
            StbSQLWhereImpianti.Length = 0
            StbSQLNotExists.Length = 0

            '-----------------------------------------------
            'PEZZO DI QUERY COMUNE ALLE DUE PARTI DI UNION
            StbSQLSelect.AppendLine(" SELECT   ")
            StbSQLSelect.AppendLine(" Agenda.Id_Agenda, Fabbr_Raccolta.Fabbricato_Des AS Stabilimento, ")
            StbSQLSelect.AppendLine(" Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, ")
            StbSQLSelect.AppendLine(" -- Risorse_Umane_Conferenti.Cod_Contatto AS Piva_Conferente, ")
            StbSQLSelect.AppendLine(" ISNULL(Contatti_Conferenti.Rag_Soc, ' ') AS RagSoc_Conferente, ")
            StbSQLSelect.AppendLine("  ")
            StbSQLSelect.AppendLine(" ISNULL( ( ")
            StbSQLSelect.AppendLine(" SELECT rtrim(Rag_Soc) + '     ' + Contatti_Coop1.Cod_Contatto ")
            StbSQLSelect.AppendLine(" FROM  Risorse_Umane Risorse_Umane_Coop1  ")
            StbSQLSelect.AppendLine(" INNER JOIN Contatti Contatti_Coop1 ON Risorse_Umane_Coop1.Piva = Contatti_Coop1.Piva ")
            StbSQLSelect.AppendLine(" AND Risorse_Umane_Coop1.Cod_Contatto = Contatti_Coop1.Cod_Contatto  ")
            StbSQLSelect.AppendLine(" WHERE Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop1.Cod_RisUm  ")
            StbSQLSelect.AppendLine(" ), ' ') AS RagSoc_Coop1,  ")
            StbSQLSelect.AppendLine("  ")
            StbSQLSelect.AppendLine(" ISNULL( (  ")
            StbSQLSelect.AppendLine(" SELECT rtrim(Rag_Soc) + '     ' + Contatti_Coop2.Cod_Contatto ")
            StbSQLSelect.AppendLine(" FROM  Risorse_Umane Risorse_Umane_Coop2  ")
            StbSQLSelect.AppendLine(" INNER JOIN Contatti Contatti_Coop2 ON Risorse_Umane_Coop2.Piva = Contatti_Coop2.Piva ")
            StbSQLSelect.AppendLine(" AND Risorse_Umane_Coop2.Cod_Contatto = Contatti_Coop2.Cod_Contatto   ")
            StbSQLSelect.AppendLine(" WHERE Mov_Accett.Extra_Int = Risorse_Umane_Coop2.Cod_RisUm   ")
            StbSQLSelect.AppendLine("  ), ' ') AS RagSoc_Coop2,  ")
            StbSQLSelect.AppendLine("  ")
            StbSQLSelect.AppendLine(" ISNULL( ( ")
            StbSQLSelect.AppendLine(" SELECT rtrim(Rag_Soc) + '     ' + Contatti_Produttori.Cod_Contatto ")
            StbSQLSelect.AppendLine(" FROM  Risorse_Umane Risorse_Umane_Produttori  ")
            StbSQLSelect.AppendLine(" INNER JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva  ")
            StbSQLSelect.AppendLine(" AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  ")
            StbSQLSelect.AppendLine(" WHERE Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm   ")
            StbSQLSelect.AppendLine("  ), ' ') AS RagSoc_Produttore,  ")
            StbSQLSelect.AppendLine("  ")
            StbSQLSelect.AppendLine(" Mov_Accett.Peso,  Mov_Accett.Tara_Veicolo, 0.0 AS Peso_Lordo, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso, ")
            StbSQLSelect.AppendLine(" 0.0 AS Peso_Netto, 0.0 AS Degrado, 0.0 AS Netto_Pagamento, ")
            StbSQLSelect.AppendLine(" -- 0.0 AS Peso_Totale, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta,  ")
            StbSQLSelect.AppendLine("  ")
            StbSQLSelect.AppendLine(" Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, '' AS Numero_Bolla, ")
            StbSQLSelect.AppendLine(" Sequenza_Progressivi.Lunghezza_Sin, Sequenza_Progressivi.Lunghezza_Centro, Sequenza_Progressivi.Lunghezza_Des, Sequenza_Progressivi.CarattereFormattazione, ")
            StbSQLSelect.AppendLine(" Mov_Accett.Data_Movimento AS Data_Bolla, ")
            StbSQLSelect.AppendLine(" Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf, ")
            StbSQLSelect.AppendLine(" Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf, '' AS Numero_Conf,  Mov_Conf.Data_Movimento AS Data_Conf,  ")
            StbSQLSelect.AppendLine(" --Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta,  ")
            StbSQLSelect.AppendLine(" Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta, ")
            StbSQLSelect.AppendLine(" Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta, Mov_Dett_Raccolta.Tara AS Tara, ")
            StbSQLSelect.AppendLine(" MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta,  ")
            StbSQLSelect.AppendLine(" MP_Raccolta.Mat_Cod AS Mat_Cod_Prodotto, MP_Raccolta.Regolamento AS Regolamento_Prodotto,  '' AS Tipologia_Agricoltura, MP_Raccolta.Mat_Des AS Mat_Des_Raccolta ")

            StbSQLSelect.AppendLine("  ")

            StbSQLJoin.AppendLine(" FROM Agenda ")
            StbSQLJoin.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda  ")

            StbSQLJoin.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQLJoin.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")

            StbSQLJoin.AppendLine(" INNER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Accett.PIVA ")
            StbSQLJoin.AppendLine("             AND Sequenza_Progressivi.Anno = YEAR(Mov_Accett.Data_Movimento)  ")
            StbSQLJoin.AppendLine("             AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Accett.Doc_Numero_Sin ")
            StbSQLJoin.AppendLine("             AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Accett.Doc_Numero_Des  ")

            StbSQLJoin.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda  ")

            StbSQLJoin.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            StbSQLJoin.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   ")
            StbSQLJoin.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det   ")
            StbSQLJoin.AppendLine(" INNER JOIN Fabbricati Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Fabbricato_Cod ")
            StbSQLJoin.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")

            StbSQLWhere.AppendLine(" WHERE   Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "")
            StbSQLWhere.AppendLine(" AND     Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StbSQLWhere.AppendLine(" AND     Mov_Accett.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'")
            StbSQLWhere.AppendLine(" AND     Mov_Conf.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) & "'")
            StbSQLWhere.AppendLine(" AND     Mov_Raccolta.Cau_Mov = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'")

            StbSQLWhere.AppendLine(" AND     Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StbSQLWhere.AppendLine(" AND     Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(CStr(enum_SequenzaProgressiviTipi.BolleAccettDaDiversi)) & "  ")

            StbSQLWhere.AppendLine(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQLWhere.AppendLine(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            If Sa_Cod <> 0 Then
                StbSQLWhere.AppendLine(" AND     Mov_Dest_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If
            If Fabbricato_Cod <> 0 Then
                StbSQLWhere.AppendLine(" AND     Mov_Dest_Raccolta.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")
            End If

            'If Codice_Conferente <> "" Then
            '    'se è stato selezionato un solo conferente
            '    '(altrimenti c'è il filtro dopo)
            '    StbSQLWhere.AppendLine(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' ")
            'End If

            'If FiltroConferenti <> "" Then
            '    StbSQLWhere.AppendLine(FiltroConferenti)
            'End If

            'If Codice_Specie <> "" Then
            '    'se è stata selezionata una solo specie
            '    '(altrimenti c'è il filtro dopo)
            '    StbSQLWhere.AppendLine(" AND MP_Raccolta.Cod_Articolo = '" & Agro_SQL_SaveText(Codice_Specie) & "' ")
            'End If

            'If FiltroSpecie <> "" Then
            '    StbSQLWhere.AppendLine(FiltroSpecie)
            'End If

            'If Mat_Cod <> 0 Then
            '    StbSQLWhere.AppendLine(" AND MP_Raccolta.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            'End If

            If Regolamento_Cod <> enum_Cod_Regolamento.Non_Specificato Then
                StbSQLWhere.AppendLine(" AND MP_Raccolta.Regolamento = " & Agro_SQL_SaveNum(Regolamento_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQLWhere.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            ' FINE PEZZO DI QUERY COMUNE ALLE DUE PARTI DI UNION
            '-----------------------------------------------


            '-- NOT EXISTS PER LA PRIMA PARTE DI UNION
            'per verificare che non ci sia la tracciabilità non basta verificare Mov_Dettagli_Riferimenti
            'devo arrivare fino all'impianto
            'perché una raccolta fittizia viene salvata sempre
            StbSQLNotExists.AppendLine("  ")
            StbSQLNotExists.AppendLine("  AND NOT EXISTS ( ")
            StbSQLNotExists.AppendLine("  SELECT 1 ")
            StbSQLNotExists.AppendLine("  FROM        Mov_Dettagli_Riferimenti  ")
            StbSQLNotExists.AppendLine("  INNER JOIN  Mov_Destinazioni Mov_Destinazioni_RaccoltaImpianti ON ")
            StbSQLNotExists.AppendLine("    Mov_Dettagli_Riferimenti.Piva_Rif = Mov_Destinazioni_RaccoltaImpianti.Piva   ")
            StbSQLNotExists.AppendLine("    AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Mov_Destinazioni_RaccoltaImpianti.Id_Agenda   ")
            StbSQLNotExists.AppendLine("  INNER JOIN Reg_Impianti ON Reg_Impianti.PIVA = Mov_Destinazioni_RaccoltaImpianti.Piva   ")
            StbSQLNotExists.AppendLine("    AND Reg_Impianti.Sa_Cod = Mov_Destinazioni_RaccoltaImpianti.Sa_Cod   ")
            StbSQLNotExists.AppendLine("    AND Reg_Impianti.Appezza = Mov_Destinazioni_RaccoltaImpianti.Appezza  ")
            StbSQLNotExists.AppendLine("    AND Reg_Impianti.Id_Reg = Mov_Destinazioni_RaccoltaImpianti.Id_Destinazione  ")
            StbSQLNotExists.AppendLine("  WHERE     Mov_Dettagli_Riferimenti.Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_ACCETTAZIONE_DIVERSI) & "  ")
            StbSQLNotExists.AppendLine("  AND       Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(LAVCOD_RACCOLTA) & "  ")
            StbSQLNotExists.AppendLine("  AND       Mov_Dettagli_Riferimenti.Piva = Agenda.piva ")
            StbSQLNotExists.AppendLine("  AND       Mov_Dettagli_Riferimenti.Id_Agenda = Agenda.id_agenda   ")
            StbSQLNotExists.AppendLine(" ) ")
            StbSQLNotExists.AppendLine("  ")


            '-- JOIN PER RIFERIMENTI CON IMPIANTI
            StbSQLJoinImpianti.AppendLine("  ")
            StbSQLJoinImpianti.AppendLine(" INNER JOIN  Mov_Dettagli_Riferimenti ON Mov_Dettagli_Riferimenti.Piva = Agenda.piva AND  Mov_Dettagli_Riferimenti.Id_Agenda = Agenda.id_agenda  ")
            'l'accettazione sta in piva sa_cod lav_cod
            StbSQLJoinImpianti.AppendLine(" INNER JOIN  Agenda Agenda_Accettazione ON Mov_Dettagli_Riferimenti.Piva = Agenda_Accettazione.PIVA  ")
            StbSQLJoinImpianti.AppendLine("             AND Mov_Dettagli_Riferimenti.Sa_Cod = Agenda_Accettazione.Sa_Cod  ")
            StbSQLJoinImpianti.AppendLine("             AND Mov_Dettagli_Riferimenti.Id_Agenda = Agenda_Accettazione.Id_Agenda  ")
            'la raccolta sta in piva_rif sa_cod_rif lav_cod_rif
            StbSQLJoinImpianti.AppendLine(" INNER JOIN  Agenda Agenda_RaccoltaImpianti ON Mov_Dettagli_Riferimenti.Piva_Rif = Agenda_RaccoltaImpianti.PIVA  ")
            StbSQLJoinImpianti.AppendLine("             AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = Agenda_RaccoltaImpianti.Sa_Cod  ")
            StbSQLJoinImpianti.AppendLine("             AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Agenda_RaccoltaImpianti.Id_Agenda  ")
            StbSQLJoinImpianti.AppendLine(" INNER JOIN  Movimenti Movimenti_RaccoltaImpianti ON Agenda_RaccoltaImpianti.PIVA = Movimenti_RaccoltaImpianti.PIVA  ")
            StbSQLJoinImpianti.AppendLine("             AND Agenda_RaccoltaImpianti.Id_Agenda = Movimenti_RaccoltaImpianti.Id_Agenda ")
            StbSQLJoinImpianti.AppendLine(" INNER JOIN  Movimenti_dettagli Movimenti_dettagli_RaccoltaImpianti ON Movimenti_RaccoltaImpianti.PIVA = Movimenti_dettagli_RaccoltaImpianti.PIVA  ")
            StbSQLJoinImpianti.AppendLine("             AND Movimenti_RaccoltaImpianti.Id_Agenda = Movimenti_dettagli_RaccoltaImpianti.Id_Agenda  ")
            StbSQLJoinImpianti.AppendLine("             AND Movimenti_RaccoltaImpianti.Id_Mov = Movimenti_dettagli_RaccoltaImpianti.Id_Mov  ")
            StbSQLJoinImpianti.AppendLine(" INNER JOIN  Mov_Destinazioni Mov_Destinazioni_RaccoltaImpianti ON Movimenti_dettagli_RaccoltaImpianti.PIVA = Mov_Destinazioni_RaccoltaImpianti.Piva  ")
            StbSQLJoinImpianti.AppendLine("             AND Movimenti_dettagli_RaccoltaImpianti.Sa_Cod = Mov_Destinazioni_RaccoltaImpianti.Sa_Cod  ")
            StbSQLJoinImpianti.AppendLine("             AND Movimenti_dettagli_RaccoltaImpianti.Id_Agenda = Mov_Destinazioni_RaccoltaImpianti.Id_Agenda  ")
            StbSQLJoinImpianti.AppendLine("             AND Movimenti_dettagli_RaccoltaImpianti.Id_Mov = Mov_Destinazioni_RaccoltaImpianti.Id_Mov  ")
            StbSQLJoinImpianti.AppendLine("             AND Movimenti_dettagli_RaccoltaImpianti.Id_Mov_Det = Mov_Destinazioni_RaccoltaImpianti.Id_Mov_Det  ")
            StbSQLJoinImpianti.AppendLine(" INNER JOIN Reg_Impianti ON Reg_Impianti.PIVA = Mov_Destinazioni_RaccoltaImpianti.Piva  ")
            StbSQLJoinImpianti.AppendLine("             AND Reg_Impianti.Sa_Cod = Mov_Destinazioni_RaccoltaImpianti.Sa_Cod  ")
            StbSQLJoinImpianti.AppendLine("             AND Reg_Impianti.Appezza = Mov_Destinazioni_RaccoltaImpianti.Appezza  ")
            StbSQLJoinImpianti.AppendLine("             AND Reg_Impianti.Id_Reg = Mov_Destinazioni_RaccoltaImpianti.Id_Destinazione ")
            StbSQLJoinImpianti.AppendLine(" INNER JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva ")
            StbSQLJoinImpianti.AppendLine("             AND Reg_Impianti.Sa_Cod = Imprese_Progetti.Sa_Cod  ")
            StbSQLJoinImpianti.AppendLine("             AND Reg_Impianti.Appezza = Imprese_Progetti.Appezza  ")
            StbSQLJoinImpianti.AppendLine("             AND Reg_Impianti.Id_Reg = Imprese_Progetti.Id_reg ")
            StbSQLJoinImpianti.AppendLine(" INNER JOIN Cultivar ON Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod ")
            StbSQLJoinImpianti.AppendLine(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StbSQLJoinImpianti.AppendLine(" INNER JOIN GruppoFinalita ON Reg_Impianti.Grfi_Cod = GruppoFinalita.Grfi_Cod ")
            StbSQLJoinImpianti.AppendLine("")


            '-- WHERE PER RIFERIMENTI CON IMPIANTI
            StbSQLWhereImpianti.AppendLine("  AND     Mov_Dettagli_Riferimenti.Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_ACCETTAZIONE_DIVERSI) & "  ")
            StbSQLWhereImpianti.AppendLine("  AND       Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & Agro_SQL_SaveNum(LAVCOD_RACCOLTA) & "  ")
            StbSQLWhereImpianti.AppendLine("  AND       Movimenti_RaccoltaImpianti.Data_Movimento >= Imprese_Progetti.Validita_Inizio ")
            StbSQLWhereImpianti.AppendLine("  AND       Movimenti_RaccoltaImpianti.Data_Movimento <= Imprese_Progetti.Validita_Fine  ")



            '------------------------------------------
            '-- QUERY COMPLETA
            '------------------------------------------
            StbSQLGenerale.AppendLine(" ( ")
            StbSQLGenerale.AppendLine(" -- PARTE 1: BOLLE SENZA TRACCIABILITA' IMPIANTO ")
            StbSQLGenerale.AppendLine(StbSQLSelect.ToString)
            StbSQLGenerale.AppendLine(", '' AS Progetto_Nome, 0 AS Sup_Imp, '' AS Veg_Des, '' AS Cul_Des,  ")
            StbSQLGenerale.AppendLine(" '' AS Grva_Des, '' AS Grfi_Des, '' AS Setup_Cod,  ")
            StbSQLGenerale.AppendLine(" '01/01/1900' AS Validita_Inizio_Impianto, '31/12/2100' AS Validita_Fine_Impianto, ")
            StbSQLGenerale.AppendLine(" '01/01/1900' AS Validita_Inizio_Distinta, '31/12/2100' AS Validita_Fine_Distinta ")
            StbSQLGenerale.AppendLine(" , '' AS Str_Progetto_Nome, '' AS Str_Sup_Imp, '' AS Str_Veg_Des, '' AS Str_Cul_Des,  ")
            StbSQLGenerale.AppendLine(" '' AS Str_Grva_Des, '' AS Str_Grfi_Des, '' AS Str_Setup_Cod,  ")
            StbSQLGenerale.AppendLine(" '' AS Str_Validita_Inizio_Impianto, '' AS Str_Validita_Fine_Impianto, ")
            StbSQLGenerale.AppendLine(" '' AS Str_Validita_Inizio_Distinta, '' AS Str_Validita_Fine_Distinta ")
            StbSQLGenerale.AppendLine(StbSQLJoin.ToString)
            StbSQLGenerale.AppendLine(StbSQLWhere.ToString)
            StbSQLGenerale.AppendLine(StbSQLNotExists.ToString)
            StbSQLGenerale.AppendLine(" ) ")
            StbSQLGenerale.AppendLine(" UNION ALL ")
            StbSQLGenerale.AppendLine(" (  ")
            StbSQLGenerale.AppendLine(" -- PARTE 2: BOLLE CON TRACCIABILITA' IMPIANTO ")
            StbSQLGenerale.AppendLine(StbSQLSelect.ToString)
            StbSQLGenerale.AppendLine(" , Imprese_Progetti.Progetto_Nome AS Str_Progetto_Nome, Reg_Impianti.Sup_Imp AS Str_Sup_Imp, veg_des AS Str_Veg_Des, cul_des AS Str_Cul_Des,  ")
            StbSQLGenerale.AppendLine(" ISNULL( (SELECT GruppoVarietale.Grva_Des ")
            StbSQLGenerale.AppendLine("         FROM GruppoVarietale ")
            StbSQLGenerale.AppendLine("         WHERE Reg_Impianti.GRVA_Cod_VEG  = GruppoVarietale.Grva_Cod  ")
            StbSQLGenerale.AppendLine("         ), '') AS Grva_Des, ")
            StbSQLGenerale.AppendLine(" GruppoFinalita.Grfi_Des, Reg_Impianti.Setup_Cod,  ")
            StbSQLGenerale.AppendLine(" Reg_Impianti.Validita_Inizio AS Validita_Inizio_Impianto, Reg_Impianti.Validita_Fine AS Validita_Fine_Impianto, ")
            StbSQLGenerale.AppendLine(" Imprese_Progetti.Validita_Inizio AS Validita_Inizio_Distinta, Imprese_Progetti.Validita_Fine AS Validita_Fine_Distinta ")
            StbSQLGenerale.AppendLine(" , '' AS Str_Progetto_Nome, '' AS Str_Sup_Imp, '' AS Str_Veg_Des, '' AS Str_Cul_Des,  ")
            StbSQLGenerale.AppendLine(" '' AS Str_Grva_Des, '' AS Str_Grfi_Des, '' AS Str_Setup_Cod,  ")
            StbSQLGenerale.AppendLine(" '' AS Str_Validita_Inizio_Impianto, '' AS Str_Validita_Fine_Impianto, ")
            StbSQLGenerale.AppendLine(" '' AS Str_Validita_Inizio_Distinta, '' AS Str_Validita_Fine_Distinta ")
            StbSQLGenerale.AppendLine(StbSQLJoin.ToString)
            StbSQLGenerale.AppendLine(StbSQLJoinImpianti.ToString)
            StbSQLGenerale.AppendLine(StbSQLWhere.ToString)
            StbSQLGenerale.AppendLine(StbSQLWhereImpianti.ToString)
            StbSQLGenerale.AppendLine(" ) ")
            StbSQLGenerale.AppendLine("  ")
            StbSQLGenerale.AppendLine(" ORDER BY Stabilimento, Cod_Articolo_Raccolta, Mat_Des_Raccolta, Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, Mov_Accett.Data_Movimento  ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQLGenerale.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''  
    ''' Nota: restituisce una riga per ogni certificato (ha un solo dettaglio)
    ''' Viene utilizzata per l'esportazione conferimenti pomodoro nel tracciato agrea
    ''' Default:
    ''' ByVal Cod_RisUm As Integer = 0
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function CertificatiPomodoro_ExportAgrea(ByVal Anno_Conferimento As Integer,
                                                    ByVal Piva As String,
                                                    ByVal Data_Inizio As Date,
                                                    ByVal Data_Fine As Date,
                                                    ByVal Cod_RisUm As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.CertificatiPomodoro_ExportAgrea"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT  Agenda.Id_Agenda, Agenda.des_lib,  ")

            StbSQL.AppendLine("         Mov_Certificato.Doc_Numero_Sin AS Doc_Numero_Sin_Cert, Mov_Certificato.Doc_Numero AS Doc_Numero_Cert, Mov_Certificato.Doc_Numero_Des AS Doc_Numero_Des_Cert,  ")
            StbSQL.AppendLine("         Mov_Certificato.Data_Movimento AS Data_Certificato, ")
            'OLD
            'StbSQL.AppendLine("         Mov_Accett.extra_int AS Tagliando_Pesa,  Mov_Conf.extra_Str AS Premio_PomoTardivo, ")
            StbSQL.AppendLine("         Mov_Certificato.extra_int AS Tagliando_Pesa,  Mov_Certificato.extra_Str AS Premio_PomoTardivo, ")
            'StbSQL.AppendLine("         Mov_Accett.progr_registrazione AS Numero_Certificato_OLD, Mov_Accett.data_registrazione AS Data_Certificato_OLD, ")
            'aggiunti in data 15/07/2014
            StbSQL.AppendLine("         Mov_Certificato.Username_Note AS Descrizione_Appezzamento, Mov_Certificato.Sezionale_Cod AS CodiceVarieta_Distretto, ")

            StbSQL.AppendLine("         ISNULL(IC.Contratto_Numero, '') AS Contratto_Numero, ISNULL(IC.Contratto_Nome, '') AS Cod_Agrea_Contratto, ISNULL(IC.Data_Stipulazione, '01/01/1900') AS Data_Stipulazione,  ")

            ''modifica del 20/09/2010: la clausola viene salvata nel campo num_protocollo
            'StbSQL.AppendLine("         ISNULL(  (SELECT TOP 1 convert(varchar(250), Clausola_Numero) + '|' + convert(varchar(250), Imprese_Contratti_Clausole.Validita_Inizio, 103) ")
            'StbSQL.AppendLine("                     FROM Imprese_Contratti_Clausole ")
            'StbSQL.AppendLine("                     INNER JOIN Imprese_ContrattixClausole ON Imprese_ContrattixClausole.Piva_SuperUser = Imprese_Contratti_Clausole.Piva_SuperUser ")
            'StbSQL.AppendLine("                     AND Imprese_ContrattixClausole.Piva = Imprese_Contratti_Clausole.Piva AND Imprese_ContrattixClausole.Clausola_Cod = Imprese_Contratti_Clausole.Clausola_Cod ")
            'StbSQL.AppendLine("                     WHERE Imprese_ContrattixClausole.Piva= IC.Piva")
            'StbSQL.AppendLine("                     AND Imprese_ContrattixClausole.Contratto_Cod= IC.Contratto_Cod")
            'StbSQL.AppendLine("                     AND Mov_Certificato.Data_Movimento >= Imprese_Contratti_Clausole.Validita_Inizio ")
            'StbSQL.AppendLine("                     AND Mov_Certificato.Data_Movimento <= Imprese_Contratti_Clausole.Validita_Fine ")
            'StbSQL.AppendLine("                     AND Mov_Certificato.Data_Movimento >= Imprese_ContrattixClausole.Data ")
            'StbSQL.AppendLine("                     ), '|' ) AS Clausola, ")

            'Modifica del 12/07/2011: in caso non esista non devo mettere la pipe, altrimenti il codice va in errore
            StbSQL.AppendLine("         ISNULL(  (SELECT TOP 1 Imprese_Contratti_Clausole.Clausola_Cod_Alternativo + '|' + convert(varchar(250), Imprese_Contratti_Clausole.Validita_Inizio, 103) ")
            StbSQL.AppendLine("                     FROM Imprese_Contratti_Clausole ")
            StbSQL.AppendLine("                     WHERE Mov_Certificato.Num_Protocollo = Imprese_Contratti_Clausole.Clausola_Cod ")
            StbSQL.AppendLine("                     ), '' ) AS Clausola, ")
            StbSQL.AppendLine("  ")

            StbSQL.AppendLine("         Mov_Accett.Data_Movimento AS Data_Accett, Mov_Accett.Ora AS Ora_Accett,   ")
            'modifica del 25/07/2013: leggo anche il numero bolla perché marco salva 0 nel numero del certificato
            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, ")
            StbSQL.AppendLine("         Mov_Accett.Mov_Desc AS Note, ")

            StbSQL.AppendLine("         Mov_Accett.Peso, Mov_Accett.Tara_Veicolo, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso,  ")

            StbSQL.AppendLine("         ISNULL(  (SELECT    VAL_Cod ")
            StbSQL.AppendLine("                     FROM    Imprese_Codici ImpCodici_Conf ")
            StbSQL.AppendLine("                     WHERE   ImpCodici_Conf.Piva = Contatti_Conferenti.Cod_Contatto ")
            StbSQL.AppendLine("                     AND     ImpCodici_Conf.ID_COD = 1125 ")
            StbSQL.AppendLine("                     ), '' ) AS Codice_Unione_OP, ")
            StbSQL.AppendLine("         ISNULL(  (SELECT    VAL_Cod ")
            StbSQL.AppendLine("                     FROM    Imprese_Codici ImpCodici_Conf ")
            StbSQL.AppendLine("                     WHERE   ImpCodici_Conf.Piva = Contatti_Conferenti.Cod_Contatto ")
            StbSQL.AppendLine("                     AND     ImpCodici_Conf.ID_COD = 1126 ")
            StbSQL.AppendLine("                     ), '' ) AS Codice_OP, ")

            StbSQL.AppendLine("         Mov_Accett.Cod_RisUm,")
            StbSQL.AppendLine("         Contatti_Conferenti.Cod_Contatto AS Cod_Contatto_Conferente,  ")
            StbSQL.AppendLine("         Contatti_Conferenti.Rag_Soc AS Rag_Soc_Conferente,  ")

            StbSQL.AppendLine("         ISNULL(Contatti_Coop.Cod_Contatto, '') AS Cod_Contatto_Coop,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Coop.Rag_Soc, '') AS Rag_Soc_Coop,  ")

            StbSQL.AppendLine("         Contatti_Produttori.Cod_Contatto AS Cod_Contatto_Produttore,  ")
            StbSQL.AppendLine("         Contatti_Produttori.Rag_Soc AS Rag_Soc_Produttore, ISNULL(Contatti_Produttori.Codice_Fiscale, '') AS Codice_Fiscale_Produttore,  ")
            StbSQL.AppendLine("         Contatti_Produttori.Nome AS Nome_Produttore, Contatti_Produttori.Cognome AS Cognome_Produttore,  ")

            StbSQL.AppendLine("         ISNULL(  (SELECT    VAL_Cod ")
            StbSQL.AppendLine("                     FROM        Imprese_Codici ImpCodici_Trasf ")
            StbSQL.AppendLine("                     WHERE       ImpCodici_Trasf.Piva = Contatti_Trasf.Cod_Contatto ")
            StbSQL.AppendLine("                     AND         ImpCodici_Trasf.ID_COD = 1127 ")
            StbSQL.AppendLine("                     ), '0' ) AS Codice_Ass_Ind, ")
            StbSQL.AppendLine("         ISNULL(  (SELECT    VAL_Cod ")
            StbSQL.AppendLine("                     FROM        Imprese_Codici ImpCodici_Trasf ")
            StbSQL.AppendLine("                     WHERE       ImpCodici_Trasf.Piva = Contatti_Trasf.Cod_Contatto ")
            StbSQL.AppendLine("                     AND         ImpCodici_Trasf.ID_COD = 1128 ")
            StbSQL.AppendLine("                     ), '0' ) AS Codice_Az_Trasf, ")

            StbSQL.AppendLine("         Contatti_Trasf.cod_contatto AS Cod_Contatto_Trasf, Contatti_Trasf.rag_soc AS Rag_Soc_Trasf,  ")
            StbSQL.AppendLine("         Indirizzi_Trasf.ind_des AS ind_des_Trasf, Indirizzi_Trasf.frz_des AS frz_des_Trasf, Indirizzi_Trasf.CAP AS cap_Trasf,  ")
            StbSQL.AppendLine("         Indirizzi_Trasf.stato AS stato_Trasf, Indirizzi_Trasf.pro_cod_istat AS pro_cod_istat_Trasf, ")
            StbSQL.AppendLine("         Indirizzi_Trasf.com_cod_istat AS com_cod_istat_Trasf, Istat_Trasf.LOCALITA AS localita_Trasf, Istat_Trasf.COMUNI_PROV AS comuni_prov_Trasf,  ")
            StbSQL.AppendLine("         ISNULL(  (SELECT VAL_Cod ")
            StbSQL.AppendLine("                     FROM Fabbricati_Codici ")
            StbSQL.AppendLine("                     WHERE Fabbricati_Codici.Piva = Fabbr_Raccolta.PIVA ")
            StbSQL.AppendLine("                     AND  Fabbricati_Codici.SA_COD = Fabbr_Raccolta.SA_COD ")
            StbSQL.AppendLine("                     AND Fabbricati_Codici.fabbricato_cod = Fabbr_Raccolta.fabbricato_cod ")
            StbSQL.AppendLine("                     AND Fabbricati_Codici.ID_COD = 1129 ")
            StbSQL.AppendLine("                     ), '0' ) AS Codice_Stab_Reg, ")
            'StbSQL.AppendLine("         /* ")
            'StbSQL.AppendLine("         Indirizzi_Fabbricato.ind_des AS ind_des_fabbricato, Indirizzi_Fabbricato.frz_des AS frz_des_Fabbricato, Indirizzi_Fabbricato.CAP AS cap_Fabbricato,   ")
            'StbSQL.AppendLine("         Indirizzi_Fabbricato.stato AS stato_Fabbricato, Indirizzi_Fabbricato.pro_cod_istat AS pro_cod_istat_fabbricato,   ")
            'StbSQL.AppendLine("         Indirizzi_Fabbricato.com_cod_istat AS com_cod_istat_fabbricato, Istat_fabbricato.LOCALITA AS localita_fabbricato, Istat_fabbricato.COMUNI_PROV AS comuni_prov_fabbricato,  ")
            'StbSQL.AppendLine("         */ ")
            StbSQL.AppendLine("         Contatti_Vettore.Rag_Soc AS Rag_Soc_Vettore,  ")
            StbSQL.AppendLine("         Contatti_Vettore.Nome AS Nome_Vettore, Contatti_Vettore.Cognome AS Cognome_Vettore,  ")
            StbSQL.AppendLine("         Mdt_Extra.Targa AS Targa_Automezzo, Mdt_Extra.N_Immatricolazione_Rimorchio AS Targa_Rimorchio, Mdt_Extra.Provvigione AS Premio_Pomo_Bio, ")
            '/*
            'StbSQL.AppendLine("         ISNULL(  (SELECT Automezzo_Vettore.targa ")
            'StbSQL.AppendLine("                     FROM Parco_Macchine Automezzo_Vettore ")
            'StbSQL.AppendLine("                     WHERE Contatti_Vettore.Cod_Contatto = Automezzo_Vettore.Cod_Contatto ")
            'StbSQL.AppendLine("                     AND Automezzo_Vettore.tipo = 2 ")
            'StbSQL.AppendLine("                     AND Automezzo_Vettore.mac_des like '%automezzo%' ")
            'StbSQL.AppendLine("                     ), '' ) AS Targa_Automezzo, ")
            'StbSQL.AppendLine("         ISNULL(  (SELECT Rimorchio_Vettore.targa ")
            'StbSQL.AppendLine("                     FROM Parco_Macchine Rimorchio_Vettore  ")
            'StbSQL.AppendLine("                     WHERE Contatti_Vettore.Cod_Contatto = Rimorchio_Vettore.Cod_Contatto ")
            'StbSQL.AppendLine("                     AND Rimorchio_Vettore.tipo = 2 ")
            'StbSQL.AppendLine("                     AND Rimorchio_Vettore.mac_des like '%rimorchio%' ")
            'StbSQL.AppendLine("                     ), '' ) AS Targa_Rimorchio, ")
            '*/

            StbSQL.AppendLine("         Mov_Conf.Mov_Desc AS Mov_Desc_Conf,  ")
            StbSQL.AppendLine("         Mov_Conf.Data_Movimento AS Data_Conf, Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf,  ")
            StbSQL.AppendLine("         Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf, ")

            'StbSQL.AppendLine("         Mov_Raccolta.Id_Mov AS Id_Mov_Raccolta,  ")
            'StbSQL.AppendLine("         Mov_Raccolta.Cau_Mov AS Cau_Mov_Raccolta, Mov_Raccolta.Mov_Desc AS Mov_Desc_Raccolta, Mov_Raccolta.Data_Movimento AS Data_Raccolta,  ")
            'StbSQL.AppendLine("         Mov_Raccolta.Ora AS Ora_Raccolta, C_Az_Raccolta.sa_cod AS Sa_Cod_Raccolta, Fabbr_Raccolta.Fabbricato_Cod AS Fabbricato_Cod_Raccolta, ")
            'StbSQL.AppendLine("         C_Az_Raccolta.sa_nome AS Sa_Nome_Raccolta, Fabbr_Raccolta.Fabbricato_Des AS Fabbricato_Des_Raccolta,  ")
            'StbSQL.AppendLine("         Mov_Dett_Raccolta.Id_Mov_Det AS Id_Mov_Det_Raccolta, Mov_Dest_Raccolta.Id_Destinazione AS Id_Destinazione_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Elem_Cod AS Elem_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Pro_Cod AS Pro_Cod_Raccolta, Mov_Dett_Raccolta.Mat_Cod AS Mat_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Cod_Progetto AS Cod_Progetto_Raccolta, Mov_Dett_Raccolta.Fase_Cod AS Fase_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta,  ")
            'StbSQL.AppendLine("        -- UDM_Qta.Udm_Sim AS Udm_Sim_Raccolta, UDM_Qta.Udm_des AS Udm_des_Raccolta, ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Listino_Cod AS Listino_Cod_Raccolta, Mov_Dett_Raccolta.Tara AS Tara, Mov_Dett_Raccolta.Prezzo_Unitario, Mov_Dett_Raccolta.Prezzo_Unitario_Netto,  ")
            'StbSQL.AppendLine("        -- Mov_Dett_Raccolta.Imponibile, Mov_Dett_Raccolta.Imponibile_Netto,  ")
            'StbSQL.AppendLine("         Mov_Dett_Raccolta.Jolly_Int AS Jolly_Int_Raccolta, Mov_Dett_Raccolta.Contabilizzato AS Contabilizzato_Raccolta,  ")
            'StbSQL.AppendLine("         Mov_Dett_Raccolta.Pendente AS Pendente_Raccolta, ")
            StbSQL.AppendLine("         MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta,  ")
            StbSQL.AppendLine("         MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, MP_Raccolta.Cul_Cod, MP_Raccolta.Veg_Cod, SpecieVegetali.Veg_Des, ISNULL(MP_Raccolta.GRVA_COD_VEG, 0) AS GRVA_COD_VEG, ISNULL(GruppoVarietale.Grva_Des, '') AS Grva_Des,  ")
            StbSQL.AppendLine("         MP_Raccolta.Regolamento,  ")

            StbSQL.AppendLine("         ISNULL( (SELECT TOP 1 VAL_COD + '_' + REPLACE( CONVERT(Varchar(500),  MPXPrezzi.Prezzo	), '.',',' )  ")
            StbSQL.AppendLine("                     FROM Materie_Prime_Campionature MP_Camp_Indice ")
            StbSQL.AppendLine("                     INNER JOIN ParametriQualitativiXPrezzi MPXPrezzi ")
            StbSQL.AppendLine("                     ON    MP_Camp_Indice.tipo = MPXPrezzi.tipo ")
            StbSQL.AppendLine("                     AND MP_Camp_Indice.tipo_cod = MPXPrezzi.tipo_cod ")
            StbSQL.AppendLine("                     AND MP_Camp_Indice.udm_cod = MPXPrezzi.udm_cod ")
            StbSQL.AppendLine("                     WHERE MP_Camp_Indice.progressivo = Mov_Dett_Raccolta.cal_cod ")
            StbSQL.AppendLine("                     AND MPXPrezzi.Piva_SuperUser = '01271980391' ")
            StbSQL.AppendLine("                     AND MPXPrezzi.Piva = Mov_Dett_Raccolta.Piva  ")
            StbSQL.AppendLine("                     AND MP_Camp_Indice.tipo = 'indice' ")
            StbSQL.AppendLine("                     AND MPXPrezzi.validita_inizio <= Mov_Accett.Data_Movimento ")
            StbSQL.AppendLine("                     AND MPXPrezzi.validita_fine >= Mov_Accett.Data_Movimento ")
            StbSQL.AppendLine("                     AND CONVERT(FLOAT, replace( MP_Camp_Indice.val_cod ,',', '.') )  >= MPXPrezzi.VALORE_MIN ")
            StbSQL.AppendLine("                     AND CONVERT(FLOAT,replace( MP_Camp_Indice.val_cod ,',', '.') )  <= MPXPrezzi.VALORE_Max ")
            StbSQL.AppendLine("                     ), '_' ) AS GradoBrix_IndicePrezzo, ")

            StbSQL.AppendLine("         ISNULL(ICF.Valore5, 3) AS Franchigia_DifMaggiori, ISNULL(ICF.Valore7, 0.5) AS Coefficiente_DifMinori, ISNULL(ICF.Valore9, 0) AS Premio_EuroTon_PomoBio ")

            'StbSQL.AppendLine("        , Materie_Prime_Campionature.tipo, Materie_Prime_Campionature.tipo_cod, Materie_Prime_Campionature.val_cod, Materie_Prime_Campionature.descrizione ")
            'StbSQL.AppendLine(" , CASE WHEN Materie_Prime_Campionature.tipo = 'danno' AND Materie_Prime_Campionature.tipo_cod IN (1,2,3) THEN SUM(CONVERT(float, Materie_Prime_Campionature.val_cod)) AS Tot_Dif_Magg, ")
            'StbSQL.AppendLine(" CASE WHEN Materie_Prime_Campionature.tipo = 'danno' AND Materie_Prime_Campionature.tipo_cod IN (4,5,6,7) THEN SUM(CONVERT(float, Materie_Prime_Campionature.val_cod)) AS Tot_Dif_Min ")

            Select Case Anno_Conferimento
                Case Is < 2015
                    StbSQL.AppendLine(" , MPC_1.Tot_Dif_Magg")

                Case Else
                    'dal 2015 serve il valore di ciascun difetto maggiore
                    StbSQL.AppendLine(" , MPC_1a.Dif_Magg_Marcio, MPC_1b.Dif_Magg_Verde , MPC_1c.Dif_Magg_Inerti   ")
            End Select

            'introdotto il 30/07/2012
            StbSQL.AppendLine(" , MPC_2.Tot_Dif_Min , MPC_3.Frutti_Schiacciati ")

            'introdotto il 27/07/2012
            StbSQL.AppendLine(" , Sequenza_Progressivi.Lunghezza_Sin, Sequenza_Progressivi.Lunghezza_Centro, Sequenza_Progressivi.Lunghezza_Des, Sequenza_Progressivi.CarattereFormattazione ")

            StbSQL.AppendLine(" FROM Agenda ")

            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Trasf ON Agenda.PIVA = Contatti_Trasf.Piva and agenda.piva= Contatti_Trasf.cod_contatto ")
            StbSQL.AppendLine(" INNER JOIN ImpresexIndirizzi ImpresexIndirizzi_Trasf ON Contatti_Trasf.Cod_Contatto = ImpresexIndirizzi_Trasf.piva ")
            StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Trasf ON ImpresexIndirizzi_Trasf.cod_indirizzo = Indirizzi_Trasf.cod_indirizzo ")
            StbSQL.AppendLine(" INNER JOIN ISTAT Istat_trasf ON Indirizzi_Trasf.pro_cod_istat = Istat_trasf.PROV AND Indirizzi_Trasf.com_cod_istat = Istat_trasf.COM  ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   ")
            StbSQL.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Mdt_Extra ON Mdt_Extra.PIVA = Mov_Certificato.PIVA AND Mdt_Extra.Sa_Cod = Mov_Certificato.Sa_Cod  ")
            StbSQL.AppendLine(" AND Mdt_Extra.Id_Agenda = Mov_Certificato.Id_Agenda AND Mdt_Extra.Id_Mov = Mov_Certificato.Id_Mov ")

            'StbSQL.AppendLine(" ")
            'StbSQL.AppendLine(" INNER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Accett.PIVA  ")
            'StbSQL.AppendLine(" AND Sequenza_Progressivi.Anno = YEAR(Mov_Accett.Data_Movimento)  ")
            'StbSQL.AppendLine(" AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Accett.Doc_Numero_Sin  ")
            'StbSQL.AppendLine(" AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Accett.Doc_Numero_Des  ")
            'StbSQL.AppendLine("")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")
            'StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Conferenti ON  Mov_Accett.Cod_IndirizzoRisUm = Indirizzi_Conferenti.cod_indirizzo ")
            'StbSQL.AppendLine(" INNER JOIN ISTAT Istat_Conferenti ON Indirizzi_Conferenti.pro_cod_istat = Istat_Conferenti.PROV AND Indirizzi_Conferenti.com_cod_istat = Istat_Conferenti.COM ")
            StbSQL.AppendLine("")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Coop ON Risorse_Umane_Coop.Piva = Contatti_Coop.Piva AND Risorse_Umane_Coop.Cod_Contatto = Contatti_Coop.Cod_Contatto  ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN ImpreseXIndirizzi ImpreseXIndirizzi_Coop ON  Contatti_Coop.Cod_Contatto = ImpreseXIndirizzi_Coop.Piva ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Coop ON  ImpreseXIndirizzi_Coop.Cod_Indirizzo = Indirizzi_Coop.cod_indirizzo ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Coop ON Indirizzi_Coop.pro_cod_istat = Istat_Coop.PROV AND Indirizzi_Coop.com_cod_istat = Istat_Coop.COM ")
            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Produttori ON  Mov_Accett.Cod_IndirizzoDestinazione = Indirizzi_Produttori.cod_indirizzo ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Produttori ON Indirizzi_Produttori.pro_cod_istat = Istat_Produttori.PROV AND Indirizzi_Produttori.com_cod_istat = Istat_Produttori.COM ")
            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Vettore ON Mov_Accett.Cod_Vettore = Risorse_Umane_Vettore.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Vettore ON Risorse_Umane_Vettore.Piva = Contatti_Vettore.Piva AND Risorse_Umane_Vettore.Cod_Contatto = Contatti_Vettore.Cod_Contatto  ")
            'StbSQL.AppendLine("         /* ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Vettore ON  Mov_Accett.Cod_IndirizzoVettore = Indirizzi_Vettore.cod_indirizzo ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Vettore ON Indirizzi_Vettore.pro_cod_istat = Istat_Vettore.PROV AND Indirizzi_Vettore.com_cod_istat = Istat_Vettore.COM ")
            'StbSQL.AppendLine("         */ ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")

            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  ")
            StbSQL.AppendLine(" INNER JOIN SpecieVegetali ON SpecieVegetali.veg_cod = MP_Raccolta.Veg_Cod  ")
            'MODIFICA DEL 22/07/2010: ci sono prodotti che non hanno la tipologia varietale
            StbSQL.AppendLine(" LEFT OUTER JOIN gruppovarietale ON gruppovarietale.grva_cod = MP_Raccolta.grva_Cod_Veg ")

            StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det ")
            StbSQL.AppendLine(" INNER JOIN Centri_Aziendali C_Az_Raccolta ON Mov_Dett_Raccolta.PIVA = C_Az_Raccolta.PIVA AND Mov_Dett_Raccolta.Sa_Cod = C_Az_Raccolta.sa_cod ")
            StbSQL.AppendLine(" INNER JOIN Fabbricati Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Fabbricato_Cod ")
            'StbSQL.AppendLine("         /* ")
            'StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Fabbricato ON Fabbr_Raccolta.indirizzo_cod = Indirizzi_Fabbricato.cod_indirizzo ")
            'StbSQL.AppendLine(" INNER JOIN ISTAT Istat_Fabbricato ON Indirizzi_Fabbricato.pro_cod_istat = Istat_Fabbricato.PROV AND Indirizzi_Fabbricato.com_cod_istat = Istat_Fabbricato.COM  ")
            'StbSQL.AppendLine(" INNER JOIN UnitaMisura UDM_Qta ON Mov_Dett_Raccolta.Udm_Cod = UDM_Qta.Udm_Cod ")
            'StbSQL.AppendLine("         */ ")
            'StbSQL.AppendLine(" -- INNER JOIN UnitaMisura UDM_Qta ON Mov_Dett_Raccolta.Udm_Cod = UDM_Qta.Udm_Cod ")

            StbSQL.AppendLine(" LEFT OUTER JOIN Imprese_Contratto_Fasi ICF ON ICF.PIVA = Mov_Dett_Raccolta.PIVA AND ICF.Elem_Cod = Mov_Dett_Raccolta.Elem_Cod  ")
            StbSQL.AppendLine(" AND ICF.pro_Cod = Mov_Dett_Raccolta.pro_Cod AND ICF.mat_Cod = Mov_Dett_Raccolta.mat_Cod AND ICF.Progetto_Cod = Mov_Dett_Raccolta.cod_progetto ")
            StbSQL.AppendLine(" AND ICF.fase_Cod = Mov_Dett_Raccolta.fase_Cod AND ICF.lotto = Mov_Dett_Raccolta.Lotto  ")
            'StbSQL.AppendLine(" -- non join sul cal_cod! in ICF è 0, mentre nei dettagli è valorizzato con il progressivo negativo ")
            'StbSQL.AppendLine(" -- AND ICF.cal_cod = Mov_Dett_Raccolta.cal_cod")
            'StbSQL.AppendLine(" -- non join su udm_cod! in ICF è 4 (tonnellate), mentre nei dettagli è 2 (kg) e io ho bisogno di recuperare il premio euro/ton del pomo bio")
            'StbSQL.AppendLine(" -- AND ICF.udm_cod= Mov_Dett_Raccolta.udm_cod ")

            StbSQL.AppendLine(" LEFT OUTER JOIN Imprese_Contratti IC ON ICF.PIVA = IC.PIVA AND ICF.Contratto_Cod = IC.Contratto_Cod ")
            StbSQL.AppendLine(" ")
            'StbSQL.AppendLine(" INNER JOIN Materie_Prime_Campionature ")
            'StbSQL.AppendLine(" ON Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.cal_cod ")
            'StbSQL.AppendLine(" AND Materie_Prime_Campionature.tipo = 'danno' ")

            Select Case Anno_Conferimento
                Case Is < 2015
                    StbSQL.AppendLine(" INNER JOIN (SELECT progressivo, SUM(CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.'))) AS Tot_Dif_Magg  ")
                    StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
                    StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'danno'  ")
                    StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod IN (1,2,3) ")
                    StbSQL.AppendLine("             GROUP BY progressivo) AS MPC_1 on MPC_1.progressivo = Mov_Dett_Raccolta.cal_cod  ")

                Case Else
                    'dal 2015 serve il valore di ciascun difetto maggiore

                    StbSQL.AppendLine(" INNER JOIN (SELECT progressivo, CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.')) AS Dif_Magg_Marcio  ")
                    StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
                    StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'danno'  ")
                    StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 1 ")
                    StbSQL.AppendLine("             ) AS MPC_1a on MPC_1a.progressivo = Mov_Dett_Raccolta.cal_cod  ")

                    StbSQL.AppendLine(" INNER JOIN (SELECT progressivo, CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.')) AS Dif_Magg_Verde  ")
                    StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
                    StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'danno'  ")
                    StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 2 ")
                    StbSQL.AppendLine("             ) AS MPC_1b on MPC_1b.progressivo = Mov_Dett_Raccolta.cal_cod  ")

                    StbSQL.AppendLine(" INNER JOIN (SELECT progressivo, CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.')) AS Dif_Magg_Inerti  ")
                    StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
                    StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'danno'  ")
                    StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 3 ")
                    StbSQL.AppendLine("             ) AS MPC_1c on MPC_1c.progressivo = Mov_Dett_Raccolta.cal_cod  ")

            End Select

            StbSQL.AppendLine(" INNER JOIN (SELECT progressivo, SUM(CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.'))) AS Tot_Dif_Min  ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'danno'  ")
            StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod IN (4,5,6,7) ")
            StbSQL.AppendLine("             GROUP BY progressivo) AS MPC_2 on MPC_2.progressivo = Mov_Dett_Raccolta.cal_cod  ")

            'introdotto il 30/07/2012
            StbSQL.AppendLine(" INNER JOIN (SELECT progressivo, Materie_Prime_Campionature.val_cod AS Frutti_Schiacciati ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'danno'  ")
            StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 4 ") '4=frutti schiacciati
            StbSQL.AppendLine("             ) AS MPC_3 ON MPC_3.progressivo = Mov_Dett_Raccolta.cal_cod  ")

            'inserito il 27/07/2012
            StbSQL.AppendLine(" INNER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Accett.PIVA  ")
            StbSQL.AppendLine(" AND Sequenza_Progressivi.Anno = YEAR(Mov_Accett.Data_Movimento)  ")
            StbSQL.AppendLine(" AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Accett.Doc_Numero_Sin  ")
            StbSQL.AppendLine(" AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Accett.Doc_Numero_Des  ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione = -1 ")
            '                                                           0 = NO SURGELATO
            StbSQL.AppendLine(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  ")
            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov      = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'")
            StbSQL.AppendLine(" AND Mov_Certificato.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) & "'")
            StbSQL.AppendLine(" AND Mov_Conf.Cau_Mov        = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) & "'")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov    = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'")

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StbSQL.AppendLine(" AND Mov_Certificato.Data_Movimento <=" & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine(" AND Mov_Certificato.Data_Movimento >=" & Agro_SQL_SaveDate(Data_Inizio) & " ")

            If Cod_RisUm <> 0 Then
                StbSQL.AppendLine(" AND Mov_Accett.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            End If

            'StbSQL.AppendLine(" AND Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(CStr(objSession("ASG_SuperUser_CodFiscale"))) & "'  ")
            'StbSQL.AppendLine(" AND Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(CStr(enum_SequenzaProgressiviTipi.BolleAccettDaDiversi)) & "  ")

            '28/08/2018: modificato di nuovo l'ordinamento: eliminato il numero certificato che a volte veniva salvato e comprometteva l'ordine
            ''31/07/2018: nell'ordinamento introdotto il numero bolla perché il numero certificato è sempre a 0 (da quando non esiste più il certificato esterno)
            ''è capitato un caso in cui le righe non venivano ordinate bene (avevano stessa data e ora)
            StbSQL.AppendLine(" ORDER BY Codice_OP, ")
            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero,Mov_Accett.Doc_Numero_Des,  ")
            StbSQL.AppendLine("         Mov_Certificato.Data_Movimento, Mov_Accett.Ora ")
            'StbSQL.AppendLine(" ORDER BY Codice_OP, ")
            'StbSQL.AppendLine("         Mov_Certificato.Doc_Numero_Sin, Mov_Certificato.Doc_Numero, Mov_Certificato.Doc_Numero_Des, Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero,Mov_Accett.Doc_Numero_Des,  ")
            'StbSQL.AppendLine("         Mov_Certificato.Data_Movimento, Mov_Accett.Ora ")
            'StbSQL.AppendLine(" ORDER BY Codice_OP, ")
            'StbSQL.AppendLine("         Mov_Certificato.Doc_Numero_Sin, Mov_Certificato.Doc_Numero, Mov_Certificato.Doc_Numero_Des,  ")
            'StbSQL.AppendLine("         Mov_Certificato.Data_Movimento ")


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.AppendLine(" ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' In base al filtro impostato legge gli id_agenda dei certificati di pomodoro,
    ''' utilizzata da Leggi_Range_IdAgenda_Bolle per la composizione degli id_agenda per la stampa massiva
    ''' Default:
    ''' ByVal Data_Inizio As Date = AGRODATAINIZIO
    ''' ByVal Data_Fine As Date = AGRODATAFINE
    ''' ByVal Codice_Specie As String = ""
    ''' ByVal FiltroSpecie As String = ""
    ''' ByVal Codice_Conferente As String = ""
    ''' ByVal FiltroConferenti As String = ""
    ''' ByVal Piva_Produttore As String = ""
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function CertificatiPomodoro_IdAgenda_Leggi(ByVal Piva As String,
                                                       ByVal Tipo_Certificato As enum_CodificaStampe,
                                                       ByVal Data_Inizio As Date,
                                                       ByVal Data_Fine As Date,
                                                       ByVal Codice_Specie As String,
                                                       ByVal FiltroSpecie As String,
                                                       ByVal Codice_Conferente As String,
                                                       ByVal FiltroConferenti As String,
                                                       ByVal Piva_Produttore As String,
                                                       ByVal Piva_Coop1 As String,
                                                       ByVal Piva_Coop2 As String,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByVal xOrderBy As String,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.CertificatiPomodoro_IdAgenda_Leggi"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.AppendLine("SELECT   Agenda.Id_Agenda, Agenda.des_lib, ")
            StbSQL.AppendLine("         Mov_Certificato.Data_Movimento, ")
            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin AS Doc_Numero_Sin_Bolla, Mov_Accett.Doc_Numero AS Doc_Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Doc_Numero_Des_Bolla,  ")
            StbSQL.AppendLine("         Mov_Certificato.Doc_Numero_Sin AS Doc_Numero_Sin_Cert, Mov_Certificato.Doc_Numero AS Doc_Numero_Cert, Mov_Certificato.Doc_Numero_Des AS Doc_Numero_Des_Cert  ")

            StbSQL.AppendLine(" FROM    Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   ")

            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")

            If Piva_Produttore <> "" Then
                StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  ")
            End If

            If Piva_Coop1 <> "" Then
                StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop1 ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop1.Cod_RisUm  ")
            End If

            If Piva_Coop2 <> "" Then
                StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop2 ON Mov_Accett.Extra_Int = Risorse_Umane_Coop2.Cod_RisUm  ")
            End If

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")

            If Tipo_Certificato = enum_CodificaStampe.Certificato_Pomodoro_Esterno Then
                'il certificato esterno va stampato solo per i non surgelati
                StbSQL.AppendLine(" INNER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  ")
            End If

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod            = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione    = -1 ")
            StbSQL.AppendLine(" AND Agenda.Piva                 = '" & Agro_SQL_SaveText(Piva) & "' ")
            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov          = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'")
            StbSQL.AppendLine(" AND Mov_Certificato.Cau_Mov     = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) & "'")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov        = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "' ")
            'StbSQL.AppendLine(" AND Mov_Certificato.Data_Movimento   <=" & Agro_SQL_SaveDate(Validita_Fine) & " ")
            'StbSQL.AppendLine(" AND Mov_Certificato.Data_Movimento   >=" & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento   <=" & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento   >=" & Agro_SQL_SaveDate(Data_Inizio) & " ")

            If Tipo_Certificato = enum_CodificaStampe.Certificato_Pomodoro_Esterno Then
                'il certificato esterno va stampato solo per i non surgelati
                StbSQL.AppendLine(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  ")
            End If

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.AppendLine(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' ")
            End If

            If FiltroConferenti <> "" Then
                StbSQL.AppendLine(FiltroConferenti)
            End If

            If Piva_Produttore <> "" Then
                StbSQL.AppendLine(" AND Risorse_Umane_Produttori.Cod_contatto = '" & Agro_SQL_SaveText(Piva_Produttore) & "' ")
            End If

            If Piva_Coop1 <> "" Then
                StbSQL.AppendLine(" AND Risorse_Umane_Coop1.Cod_contatto = '" & Agro_SQL_SaveText(Piva_Coop1) & "' ")
            End If

            If Piva_Coop2 <> "" Then
                StbSQL.AppendLine(" AND Risorse_Umane_Coop2.Cod_contatto = '" & Agro_SQL_SaveText(Piva_Coop2) & "' ")
            End If

            If Codice_Specie <> "" Then
                'se è stata selezionata una solo specie
                '(altrimenti c'è il filtro dopo)
                StbSQL.AppendLine(" AND MP_Raccolta.Cod_Articolo = '" & Agro_SQL_SaveText(Codice_Specie) & "' ")
            End If

            If FiltroSpecie <> "" Then
                StbSQL.AppendLine(FiltroSpecie)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.AppendLine(" ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' chiamata da Recupera_UltimoNumCertificatoEBolla per la valorizzazione del numero dei certificati
    ''' Default:
    '''ByVal Data_Inizio As Date = AGRODATAINIZIO
    '''ByVal Data_Fine As Date = AGRODATAFINE
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function CertificatiPomodoro_MaxNumCertificato_Leggi(ByVal Piva As String,
                                                                ByVal Anno As Integer,
                                                                ByVal Data_Inizio As Date,
                                                                ByVal Data_Fine As Date,
                                                                ByVal xFiltroAggiuntivo As String,
                                                                ByVal xOrderBy As String,
                                                                ByRef objParametri As AgronicaCoreParametri
                                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.CertificatiPomodoro_MaxNumCertificato_Leggi"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine("SELECT   Agenda.Id_Agenda, Agenda.des_lib, ")
            StbSQL.AppendLine("         Mov_Certificato.Data_Movimento, ")
            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin AS Doc_Numero_Sin_Bolla, Mov_Accett.Doc_Numero AS Doc_Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Doc_Numero_Des_Bolla,  ")
            StbSQL.AppendLine("         MAX(Mov_Certificato.Doc_Numero) AS Max_Doc_Numero_Cert ")
            StbSQL.AppendLine(" FROM    Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod            = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & " ")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione    = -1 ")
            StbSQL.AppendLine(" AND Agenda.Piva                 = '" & Agro_SQL_SaveText(Piva) & "' ")
            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov          = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "' ")
            StbSQL.AppendLine(" AND Mov_Certificato.Cau_Mov     = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) & "' ")
            StbSQL.AppendLine(" AND YEAR(Mov_Certificato.Data_Movimento) = " & Agro_SQL_SaveNum(Anno) & " ")
            StbSQL.AppendLine(" AND Mov_Certificato.Data_Movimento   <=" & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine(" AND Mov_Certificato.Data_Movimento   >=" & Agro_SQL_SaveDate(Data_Inizio) & " ")

            StbSQL.AppendLine("GROUP BY   Agenda.Id_Agenda, Agenda.des_lib, ")
            StbSQL.AppendLine("         Mov_Certificato.Data_Movimento, ")
            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des ")

            StbSQL.AppendLine(" ORDER BY Max_Doc_Numero_Cert desc ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.AppendLine(" ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge il peso netto e il degrado dei certificati
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function CertificatiPomodoro_NettoDegrado_Leggi(ByVal Tipo_Registro As enum_TipoRegistroCaricoScaricoPomodoro,
                                                           ByVal Piva As String,
                                                           ByVal Data_Inizio As Date,
                                                           ByVal Data_Fine As Date,
                                                           ByVal xFiltroAggiuntivo As String,
                                                           ByVal xOrderBy As String,
                                                           ByRef objParametri As AgronicaCoreParametri
                                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.CertificatiPomodoro_NettoDegrado_Leggi"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine("SELECT Mov_Dett_Raccolta.Udm_Cod, Mov_Dett_Raccolta.Qta AS Peso_Netto, Mov_Dett_Raccolta.Variazione AS Degrado_Perc ")

            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda  ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   ")

            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  ")

            StbSQL.AppendLine(" WHERE   Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "")
            '-1 è la modalità di accettazione del pomodoro
            StbSQL.AppendLine(" AND     Agenda.tipo_accettazione = -1")
            StbSQL.AppendLine(" AND     Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StbSQL.AppendLine(" AND     Mov_Accett.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'")
            StbSQL.AppendLine(" AND     Mov_Raccolta.Cau_Mov = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'")

            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            StbSQL.AppendLine(" AND     MP_Dettagli_Raccolta.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")

            If Tipo_Registro = enum_TipoRegistroCaricoScaricoPomodoro.Contrattato Then
                '                                                           0 = NO SURGELATO
                StbSQL.AppendLine(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  ")
            Else
                '                                                           1 = SURGELATO
                StbSQL.AppendLine(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 1  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            'dalla funzione non viene passato, verificare se è fondamentale l'ordinamento bloccato
            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.AppendLine(" ORDER BY Mov_Accett.Data_Movimento ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Leggi id_agenda accettazione pomodoro 
    ''' con materie prime contrattate (no surgelate)
    ''' con doc_numero certificato =0
    ''' con doc_numero bolla > ultimo doc_numero bolla valorizzato con certificato
    ''' con doc_numero bolla < = doc_numero bolla da valorizzare
    ''' con doc_numero_sin bolla = ultimo doc_numero_sin bolla valorizzato con certificato
    ''' con doc_numero_des bolla = ultimo doc_numero_des bolla valorizzato con certificato
    ''' ordinati per doc_numero_sin, doc_numero, doc_numero des (della bolla), data bolla ASC
    ''' Default:
    ''' ByVal Data_Inizio As Date = AGRODATAINIZIO, ByVal Data_Fine As Date = AGRODATAFINE
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function CertificatiPomodoroContrattatoXNumerazione_Leggi(ByVal Piva As String,
                                                                     ByVal Anno As Integer,
                                                                     ByVal UltimoNumBollaValorizzato_Prefisso As String,
                                                                     ByVal UltimoNumBollaValorizzato As Decimal,
                                                                     ByVal UltimoNumBollaValorizzato_Suffisso As String,
                                                                     ByVal UltimoNumBollaDaValorizzare As Decimal,
                                                                     ByVal Data_Inizio As Date,
                                                                     ByVal Data_Fine As Date,
                                                                     ByVal xFiltroAggiuntivo As String,
                                                                     ByRef objParametri As AgronicaCoreParametri
                                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.CertificatiPomodoroContrattatoXNumerazione_Leggi"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.AppendLine("SELECT   Agenda.Id_Agenda, Agenda.des_lib, ")
            StbSQL.AppendLine("         Mov_Certificato.Data_Movimento, ")
            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin AS Doc_Numero_Sin_Bolla, Mov_Accett.Doc_Numero AS Doc_Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Doc_Numero_Des_Bolla,  ")
            StbSQL.AppendLine("         Mov_Certificato.Doc_Numero_Sin AS Doc_Numero_Sin_Cert, Mov_Certificato.Doc_Numero AS Doc_Numero_Cert, Mov_Certificato.Doc_Numero_Des AS Doc_Numero_Des_Cert  ")
            StbSQL.AppendLine(" FROM    Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod            = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & " ")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione    = -1 ")
            '                                                   0 = NO SURGELATO
            StbSQL.AppendLine(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  ")
            StbSQL.AppendLine(" AND Agenda.Piva                 = '" & Agro_SQL_SaveText(Piva) & "' ")
            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov          = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "' ")
            StbSQL.AppendLine(" AND Mov_Certificato.Cau_Mov     = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) & "' ")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov        = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "' ")
            StbSQL.AppendLine(" AND Mov_Certificato.Doc_Numero  = 0 ")
            StbSQL.AppendLine(" AND YEAR(Mov_Certificato.Data_Movimento) = " & Agro_SQL_SaveNum(Anno) & " ")
            StbSQL.AppendLine(" AND Mov_Accett.Doc_Numero > " & Agro_SQL_SaveNum(UltimoNumBollaValorizzato) & " ")
            StbSQL.AppendLine(" AND Mov_Accett.Doc_Numero <= " & Agro_SQL_SaveNum(UltimoNumBollaDaValorizzare) & " ")
            StbSQL.AppendLine(" AND Mov_Accett.Doc_Numero_sin = '" & Agro_SQL_SaveNum(UltimoNumBollaValorizzato_Prefisso) & "' ")
            StbSQL.AppendLine(" AND Mov_Accett.Doc_Numero_Des = '" & Agro_SQL_SaveNum(UltimoNumBollaValorizzato_Suffisso) & "' ")
            StbSQL.AppendLine(" AND Mov_Certificato.Data_Movimento   <=" & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine(" AND Mov_Certificato.Data_Movimento   >=" & Agro_SQL_SaveDate(Data_Inizio) & " ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StbSQL.AppendLine(" ORDER BY Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, Mov_Accett.Data_movimento  ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge un certificato specifico o più certificati
    ''' utilizzata da:
    ''' stampa del report del certificato del pomodoro
    ''' esportazione excel dei certificati
    ''' Default:
    '''Optional ByVal FiltroSpecie As String = "",
    '''Optional ByVal FiltroConferenti As String = "",
    '''Optional ByVal FiltroAggiuntivo As String = "",
    '''Optional ByVal Ordinamento As String = "",
    '''Optional ByVal Data_Inizio As Date = AGRODATAINIZIO,
    '''Optional ByVal Data_Fine As Date = AGRODATAFINE,
    '''Optional ByVal Flag_IndirizzoCoop As Boolean = False,
    '''Optional ByVal Flag_IndirizzoProd As Boolean = False) As DataTable
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function CertificatoPomodoro_Stampa(ByVal Piva As String,
                                               ByVal Id_Agenda As Integer,
                                               ByVal Sa_Cod As Integer,
                                               ByVal Fabbricato_Cod As Integer,
                                               ByVal Codice_Specie As String,
                                               ByVal Codice_Conferente As String,
                                               ByVal FiltroSpecie As String,
                                               ByVal FiltroConferenti As String,
                                               ByVal Data_Inizio As Date,
                                               ByVal Data_Fine As Date,
                                               ByVal Flag_IndirizzoCoop As Boolean,
                                               ByVal Flag_IndirizzoProd As Boolean,
                                               ByVal Piva_Produttore As String,
                                               ByVal Piva_Coop1 As String,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.CertificatoPomodoro_Stampa"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT  Agenda.Id_Agenda, Agenda.des_lib,  ")

            StbSQL.AppendLine("         Mov_Certificato.Doc_Numero_Sin AS Doc_Numero_Sin_Cert, Mov_Certificato.Doc_Numero AS Doc_Numero_Cert, Mov_Certificato.Doc_Numero_Des AS Doc_Numero_Des_Cert,  ")
            StbSQL.AppendLine("         Mov_Certificato.Data_Movimento AS Data_Certificato, ")
            'aggiunti in data 15/07/2014
            StbSQL.AppendLine("         Mov_Certificato.Username_Note AS Descrizione_Appezzamento, Mov_Certificato.Sezionale_Cod AS CodiceVarieta_Distretto, ")
            StbSQL.AppendLine("         ISNULL(  (SELECT TOP 1 Descrizione ")
            StbSQL.AppendLine("                     FROM CAC_Codifica_Cultivar ")
            StbSQL.AppendLine("                     WHERE CONVERT(int,CAC_Codifica_Cultivar.Cultivar_Coltiva) = Mov_Certificato.Sezionale_Cod ")
            StbSQL.AppendLine("                     ), '' ) AS Varieta_Distretto, ")
            'OLD
            'StbSQL.AppendLine("         Mov_Accett.extra_int AS Tagliando_Pesa,  Mov_Conf.extra_Str AS Premio_PomoTardivo, ")
            StbSQL.AppendLine("         Mov_Certificato.extra_int AS Tagliando_Pesa,  Mov_Certificato.extra_Str AS Premio_PomoTardivo, ")

            StbSQL.AppendLine("         Mov_Accett.progr_registrazione AS Numero_Certificato_OLD, Mov_Accett.data_registrazione AS Data_Certificato_OLD, ")

            StbSQL.AppendLine("         ISNULL(IC.Contratto_Numero, '') AS Contratto_Numero, ISNULL(IC.Contratto_Nome, '') AS Cod_Agrea_Contratto, ISNULL(IC.Data_Stipulazione, '01/01/1900') AS Data_Stipulazione,  ")

            ''modifica del 20/09/2010: la clausola viene salvata nel campo num_protocollo
            'StbSQL.AppendLine("         ISNULL(  (SELECT TOP 1 convert(varchar(250), Clausola_Numero) + '|' + convert(varchar(250), Imprese_Contratti_Clausole.Validita_Inizio, 103) ")
            'StbSQL.AppendLine("                     FROM Imprese_Contratti_Clausole ")
            'StbSQL.AppendLine("                     INNER JOIN Imprese_ContrattixClausole ON Imprese_ContrattixClausole.Piva_SuperUser = Imprese_Contratti_Clausole.Piva_SuperUser ")
            'StbSQL.AppendLine("                     AND Imprese_ContrattixClausole.Piva = Imprese_Contratti_Clausole.Piva AND Imprese_ContrattixClausole.Clausola_Cod = Imprese_Contratti_Clausole.Clausola_Cod ")
            'StbSQL.AppendLine("                     WHERE Imprese_ContrattixClausole.Piva= IC.Piva")
            'StbSQL.AppendLine("                     AND Imprese_ContrattixClausole.Contratto_Cod= IC.Contratto_Cod")
            'StbSQL.AppendLine("                     AND Mov_Certificato.Data_Movimento >= Imprese_Contratti_Clausole.Validita_Inizio ")
            'StbSQL.AppendLine("                     AND Mov_Certificato.Data_Movimento <= Imprese_Contratti_Clausole.Validita_Fine ")
            'StbSQL.AppendLine("                     AND Mov_Certificato.Data_Movimento >= Imprese_ContrattixClausole.Data ")
            'StbSQL.AppendLine("                     ), '|' ) AS Clausola, ")
            StbSQL.AppendLine("         ISNULL(  (SELECT TOP 1 Imprese_Contratti_Clausole.Clausola_Cod_Alternativo + '|' + convert(varchar(250), Imprese_Contratti_Clausole.Validita_Inizio, 103) ")
            StbSQL.AppendLine("                     FROM Imprese_Contratti_Clausole ")
            StbSQL.AppendLine("                     WHERE Mov_Certificato.Num_Protocollo = Imprese_Contratti_Clausole.Clausola_Cod ")
            StbSQL.AppendLine("                     ), '|' ) AS Clausola, ")
            StbSQL.AppendLine("  ")

            StbSQL.AppendLine("         Mov_Accett.Data_Movimento AS Data_Accett, Mov_Accett.Ora AS Ora_Accett, YEAR(Mov_Accett.Data_Movimento) AS Anno_raccolto,  ")
            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero,Mov_Accett.Doc_Numero_Des, Mov_Accett.Mov_Desc AS Note, ")
            StbSQL.AppendLine("         Sequenza_Progressivi.Lunghezza_Sin, Sequenza_Progressivi.Lunghezza_Centro, Sequenza_Progressivi.Lunghezza_Des, Sequenza_Progressivi.CarattereFormattazione, ")

            StbSQL.AppendLine("         Mov_Accett.Peso, Mov_Accett.Tara_Veicolo, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso,  ")

            StbSQL.AppendLine("         ISNULL(  (SELECT    VAL_Cod ")
            StbSQL.AppendLine("                     FROM    Imprese_Codici ImpCodici_Conf ")
            StbSQL.AppendLine("                     WHERE   ImpCodici_Conf.Piva = Contatti_Conferenti.Cod_Contatto ")
            StbSQL.AppendLine("                     AND     ImpCodici_Conf.ID_COD = 1125 ")
            StbSQL.AppendLine("                     ), '' ) AS Codice_Unione_OP, ")
            StbSQL.AppendLine("         ISNULL(  (SELECT    VAL_Cod ")
            StbSQL.AppendLine("                     FROM    Imprese_Codici ImpCodici_Conf ")
            StbSQL.AppendLine("                     WHERE   ImpCodici_Conf.Piva = Contatti_Conferenti.Cod_Contatto ")
            StbSQL.AppendLine("                     AND     ImpCodici_Conf.ID_COD = 1126 ")
            StbSQL.AppendLine("                     ), '' ) AS Codice_OP, ")

            StbSQL.AppendLine("         Mov_Accett.Cod_RisUm,")
            StbSQL.AppendLine("         Risorse_Umane_Conferenti.Cod_Rapporto AS Cod_Rapporto_Conferente, Contatti_Conferenti.Cod_Contatto AS Cod_Contatto_Conferente,  ")
            StbSQL.AppendLine("         Contatti_Conferenti.Rag_Soc AS Rag_Soc_Conferente, ISNULL(Contatti_Conferenti.Codice_Fiscale, '') AS Codice_Fiscale_Conferente, ")
            StbSQL.AppendLine("         Contatti_Conferenti.Nome AS Nome_Conferente, Contatti_Conferenti.Cognome AS Cognome_Conferente,  ")
            StbSQL.AppendLine("         Mov_Accett.Cod_IndirizzoRisUm,   Indirizzi_Conferenti.ind_des AS ind_des_Conferente, Indirizzi_Conferenti.frz_des AS frz_des_Conferente, Indirizzi_Conferenti.CAP AS cap_Conferente,  ")
            StbSQL.AppendLine("         Indirizzi_Conferenti.stato AS stato_Conferente, Indirizzi_Conferenti.pro_cod_istat AS pro_cod_istat_Conferente,  ")
            StbSQL.AppendLine("         Indirizzi_Conferenti.com_cod_istat AS com_cod_istat_Conferente, Istat_Conferenti.LOCALITA AS localita_Conferente, Istat_Conferenti.COMUNI_PROV AS comuni_prov_Conferente, ")

            StbSQL.AppendLine("         Mov_Accett.Cod_RisUm_Altro,  ")
            StbSQL.AppendLine("         Risorse_Umane_Coop.Cod_Rapporto AS Cod_Rapporto_Coop, ISNULL(Contatti_Coop.Cod_Contatto, '') AS Cod_Contatto_Coop,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Coop.Rag_Soc, '') AS Rag_Soc_Coop, ISNULL(Contatti_Coop.Codice_Fiscale, '') AS Codice_Fiscale_Coop,  ")
            StbSQL.AppendLine("         Contatti_Coop.Nome AS Nome_Coop, Contatti_Coop.Cognome AS Cognome_Coop,  ")
            If Flag_IndirizzoCoop Then
                StbSQL.AppendLine("         ImpreseXIndirizzi_Coop.Cod_Indirizzo AS Cod_Indirizzo_Coop, Indirizzi_Coop.ind_des AS ind_des_Coop, Indirizzi_Coop.frz_des AS frz_des_Coop, Indirizzi_Coop.CAP AS cap_Coop, ")
                StbSQL.AppendLine("         Indirizzi_Coop.stato AS stato_Coop, Indirizzi_Coop.pro_cod_istat AS pro_cod_istat_Coop,  ")
                StbSQL.AppendLine("         Indirizzi_Coop.com_cod_istat AS com_cod_istat_Coop, Istat_Coop.LOCALITA AS localita_Coop, Istat_Coop.COMUNI_PROV AS comuni_prov_Coop, ")
            End If
            StbSQL.AppendLine("         Mov_Accett.Cod_Destinazione, ")
            StbSQL.AppendLine("         Risorse_Umane_Produttori.Cod_Rapporto AS Cod_Rapporto_Produttore, ISNULL(Contatti_Produttori.Cod_Contatto, '') AS Cod_Contatto_Produttore,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Produttori.Rag_Soc, '') AS Rag_Soc_Produttore, ISNULL(Contatti_Produttori.Codice_Fiscale, '') AS Codice_Fiscale_Produttore,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Produttori.Nome, '') AS Nome_Produttore, ISNULL(Contatti_Produttori.Cognome, '') AS Cognome_Produttore,  ")
            If Flag_IndirizzoProd Then
                StbSQL.AppendLine("         Mov_Accett.Cod_IndirizzoDestinazione, Indirizzi_Produttori.ind_des AS ind_des_Produttore, Indirizzi_Produttori.frz_des AS frz_des_Produttore, Indirizzi_Produttori.CAP AS cap_Produttore,  ")
                StbSQL.AppendLine("         Indirizzi_Produttori.stato AS stato_Produttore, Indirizzi_Produttori.pro_cod_istat AS pro_cod_istat_Produttore,  ")
                StbSQL.AppendLine("         Indirizzi_Produttori.com_cod_istat AS com_cod_istat_Produttore, Istat_Produttori.LOCALITA AS localita_Produttore, Istat_Produttori.COMUNI_PROV AS comuni_prov_Produttore, ")
            End If
            StbSQL.AppendLine("         ISNULL(  (SELECT    VAL_Cod ")
            StbSQL.AppendLine("                     FROM        Imprese_Codici ImpCodici_Trasf ")
            StbSQL.AppendLine("                     WHERE       ImpCodici_Trasf.Piva = Contatti_Trasf.Cod_Contatto ")
            StbSQL.AppendLine("                     AND         ImpCodici_Trasf.ID_COD = 1127 ")
            StbSQL.AppendLine("                     ), '0' ) AS Codice_Ass_Ind, ")
            StbSQL.AppendLine("         ISNULL(  (SELECT    VAL_Cod ")
            StbSQL.AppendLine("                     FROM        Imprese_Codici ImpCodici_Trasf ")
            StbSQL.AppendLine("                     WHERE       ImpCodici_Trasf.Piva = Contatti_Trasf.Cod_Contatto ")
            StbSQL.AppendLine("                     AND         ImpCodici_Trasf.ID_COD = 1128 ")
            StbSQL.AppendLine("                     ), '0' ) AS Codice_Az_Trasf, ")

            StbSQL.AppendLine("         Contatti_Trasf.cod_contatto AS Cod_Contatto_Trasf, Contatti_Trasf.codice_fiscale AS Codice_Fiscale_Trasf, Contatti_Trasf.rag_soc AS Rag_Soc_Trasf,  ")
            StbSQL.AppendLine("         Indirizzi_Trasf.ind_des AS ind_des_Trasf, Indirizzi_Trasf.frz_des AS frz_des_Trasf, Indirizzi_Trasf.CAP AS cap_Trasf,  ")
            StbSQL.AppendLine("         Indirizzi_Trasf.stato AS stato_Trasf, Indirizzi_Trasf.pro_cod_istat AS pro_cod_istat_Trasf, ")
            StbSQL.AppendLine("         Indirizzi_Trasf.com_cod_istat AS com_cod_istat_Trasf, Istat_Trasf.LOCALITA AS localita_Trasf, Istat_Trasf.COMUNI_PROV AS comuni_prov_Trasf,  ")
            StbSQL.AppendLine("         ISNULL(  (SELECT VAL_Cod ")
            StbSQL.AppendLine("                     FROM Fabbricati_Codici ")
            StbSQL.AppendLine("                     WHERE Fabbricati_Codici.Piva = Fabbr_Raccolta.PIVA ")
            StbSQL.AppendLine("                     AND  Fabbricati_Codici.SA_COD = Fabbr_Raccolta.SA_COD ")
            StbSQL.AppendLine("                     AND Fabbricati_Codici.fabbricato_cod = Fabbr_Raccolta.fabbricato_cod ")
            StbSQL.AppendLine("                     AND Fabbricati_Codici.ID_COD = 1129 ")
            StbSQL.AppendLine("                     ), '0' ) AS Codice_Stab_Reg, ")
            'StbSQL.AppendLine("         /* ")
            'StbSQL.AppendLine("         Indirizzi_Fabbricato.ind_des AS ind_des_fabbricato, Indirizzi_Fabbricato.frz_des AS frz_des_Fabbricato, Indirizzi_Fabbricato.CAP AS cap_Fabbricato,   ")
            'StbSQL.AppendLine("         Indirizzi_Fabbricato.stato AS stato_Fabbricato, Indirizzi_Fabbricato.pro_cod_istat AS pro_cod_istat_fabbricato,   ")
            'StbSQL.AppendLine("         Indirizzi_Fabbricato.com_cod_istat AS com_cod_istat_fabbricato, Istat_fabbricato.LOCALITA AS localita_fabbricato, Istat_fabbricato.COMUNI_PROV AS comuni_prov_fabbricato,  ")
            'StbSQL.AppendLine("         */ ")
            StbSQL.AppendLine("         Mov_Accett.Mezzo, Mov_Accett.Cod_Vettore, ")
            StbSQL.AppendLine("         Risorse_Umane_Vettore.Cod_Rapporto AS Cod_Rapporto_Vettore, Contatti_Vettore.Cod_Contatto AS Cod_Contatto_Vettore,  ")
            StbSQL.AppendLine("         Contatti_Vettore.Rag_Soc AS Rag_Soc_Vettore, ISNULL(Contatti_Vettore.Codice_Fiscale, '') AS Codice_Fiscale_Vettore,  ")
            StbSQL.AppendLine("         Contatti_Vettore.Nome AS Nome_Vettore, Contatti_Vettore.Cognome AS Cognome_Vettore,  ")
            'StbSQL.AppendLine("         /* ")
            'StbSQL.AppendLine("         Mov_Accett.Cod_IndirizzoVettore, Indirizzi_Vettore.ind_des AS ind_des_Vettore, Indirizzi_Vettore.frz_des AS frz_des_Vettore, Indirizzi_Vettore.CAP AS cap_Vettore,  ")
            'StbSQL.AppendLine("         Indirizzi_Vettore.stato AS stato_Vettore, Indirizzi_Vettore.pro_cod_istat AS pro_cod_istat_Vettore,  ")
            'StbSQL.AppendLine("         Indirizzi_Vettore.com_cod_istat AS com_cod_istat_Vettore, Istat_Vettore.LOCALITA AS localita_Vettore, Istat_Vettore.COMUNI_PROV AS comuni_prov_Vettore, ")
            'StbSQL.AppendLine("         */ ")

            StbSQL.AppendLine("         Mdt_Extra.Targa AS Targa_Automezzo, Mdt_Extra.N_Immatricolazione_Rimorchio AS Targa_Rimorchio, Mdt_Extra.Provvigione AS Premio_Pomo_Bio, ")
            '/*
            'StbSQL.AppendLine("         ISNULL(  (SELECT Automezzo_Vettore.targa ")
            'StbSQL.AppendLine("                     FROM Parco_Macchine Automezzo_Vettore ")
            'StbSQL.AppendLine("                     WHERE Contatti_Vettore.Cod_Contatto = Automezzo_Vettore.Cod_Contatto ")
            'StbSQL.AppendLine("                     AND Automezzo_Vettore.tipo = 2 ")
            'StbSQL.AppendLine("                     AND Automezzo_Vettore.mac_des like '%automezzo%' ")
            'StbSQL.AppendLine("                     ), '' ) AS Targa_Automezzo, ")
            'StbSQL.AppendLine("         ISNULL(  (SELECT Rimorchio_Vettore.targa ")
            'StbSQL.AppendLine("                     FROM Parco_Macchine Rimorchio_Vettore  ")
            'StbSQL.AppendLine("                     WHERE Contatti_Vettore.Cod_Contatto = Rimorchio_Vettore.Cod_Contatto ")
            'StbSQL.AppendLine("                     AND Rimorchio_Vettore.tipo = 2 ")
            'StbSQL.AppendLine("                     AND Rimorchio_Vettore.mac_des like '%rimorchio%' ")
            'StbSQL.AppendLine("                     ), '' ) AS Targa_Rimorchio, ")
            '*/

            StbSQL.AppendLine("         Mov_Conf.Id_Mov AS Id_Mov_Conf, Mov_Conf.Cau_Mov AS Cau_Mov_Conf, Mov_Conf.Mov_Desc AS Mov_Desc_Conf,  ")
            StbSQL.AppendLine("         Mov_Conf.Data_Movimento AS Data_Conf, Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf,  ")
            StbSQL.AppendLine("         Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf,  ")

            StbSQL.AppendLine("         Mov_Raccolta.Id_Mov AS Id_Mov_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Raccolta.Cau_Mov AS Cau_Mov_Raccolta, Mov_Raccolta.Mov_Desc AS Mov_Desc_Raccolta, Mov_Raccolta.Data_Movimento AS Data_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Raccolta.Ora AS Ora_Raccolta, C_Az_Raccolta.sa_cod AS Sa_Cod_Raccolta, Fabbr_Raccolta.Fabbricato_Cod AS Fabbricato_Cod_Raccolta, ")
            StbSQL.AppendLine("         C_Az_Raccolta.sa_nome AS Sa_Nome_Raccolta, Fabbr_Raccolta.Fabbricato_Des AS Fabbricato_Des_Raccolta,  ")

            StbSQL.AppendLine("         Mov_Dett_Raccolta.Id_Mov_Det AS Id_Mov_Det_Raccolta, Mov_Dest_Raccolta.Id_Destinazione AS Id_Destinazione_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Elem_Cod AS Elem_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Pro_Cod AS Pro_Cod_Raccolta, Mov_Dett_Raccolta.Mat_Cod AS Mat_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Cod_Progetto AS Cod_Progetto_Raccolta, Mov_Dett_Raccolta.Fase_Cod AS Fase_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta,  ")
            'StbSQL.AppendLine("        -- UDM_Qta.Udm_Sim AS Udm_Sim_Raccolta, UDM_Qta.Udm_des AS Udm_des_Raccolta, ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Listino_Cod AS Listino_Cod_Raccolta, Mov_Dett_Raccolta.Tara AS Tara, Mov_Dett_Raccolta.Prezzo_Unitario, Mov_Dett_Raccolta.Prezzo_Unitario_Netto,  ")
            'StbSQL.AppendLine("        -- Mov_Dett_Raccolta.Imponibile, Mov_Dett_Raccolta.Imponibile_Netto,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Jolly_Int AS Jolly_Int_Raccolta, Mov_Dett_Raccolta.Contabilizzato AS Contabilizzato_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Pendente AS Pendente_Raccolta, ")
            StbSQL.AppendLine("         MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta,  ")
            StbSQL.AppendLine("         MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, MP_Raccolta.Cul_Cod, MP_Raccolta.Veg_Cod, SpecieVegetali.Veg_Des, ISNULL(MP_Raccolta.GRVA_COD_VEG, 0) AS GRVA_COD_VEG, ISNULL(GruppoVarietale.Grva_Des, '') AS Grva_Des,  ")
            StbSQL.AppendLine("         MP_Raccolta.Regolamento, MP_Raccolta.ChkListino, MP_Dettagli_Raccolta.Extra_Smallint4 AS Flag_Surgelato, ")

            StbSQL.AppendLine("         ISNULL( (SELECT TOP 1 VAL_COD + '_' + REPLACE( CONVERT(Varchar(500),  MPXPrezzi.Prezzo	), '.',',' )  ")
            StbSQL.AppendLine("                     FROM Materie_Prime_Campionature MP_Camp_Indice ")
            StbSQL.AppendLine("                     INNER JOIN ParametriQualitativiXPrezzi MPXPrezzi ")
            StbSQL.AppendLine("                     ON    MP_Camp_Indice.tipo = MPXPrezzi.tipo ")
            StbSQL.AppendLine("                     AND MP_Camp_Indice.tipo_cod = MPXPrezzi.tipo_cod ")
            StbSQL.AppendLine("                     AND MP_Camp_Indice.udm_cod = MPXPrezzi.udm_cod ")
            StbSQL.AppendLine("                     WHERE MP_Camp_Indice.progressivo = Mov_Dett_Raccolta.cal_cod ")
            StbSQL.AppendLine("                     AND MPXPrezzi.Piva_SuperUser = '01271980391' ")
            StbSQL.AppendLine("                     AND MPXPrezzi.Piva = Mov_Dett_Raccolta.Piva  ")
            StbSQL.AppendLine("                     AND MP_Camp_Indice.tipo = 'indice' ")
            StbSQL.AppendLine("                     AND MPXPrezzi.validita_inizio <= Mov_Accett.Data_Movimento ")
            StbSQL.AppendLine("                     AND MPXPrezzi.validita_fine >= Mov_Accett.Data_Movimento ")
            StbSQL.AppendLine("                     AND CONVERT(FLOAT, replace( MP_Camp_Indice.val_cod ,',', '.') )  >= MPXPrezzi.VALORE_MIN ")
            StbSQL.AppendLine("                     AND CONVERT(FLOAT,replace( MP_Camp_Indice.val_cod ,',', '.') )  <= MPXPrezzi.VALORE_Max ")
            StbSQL.AppendLine("                     ), '_' ) AS GradoBrix_IndicePrezzo, ")

            StbSQL.AppendLine("         ISNULL(ICF.Valore5, 3) AS Franchigia_DifMaggiori, ISNULL(ICF.Valore7, 0.5) AS Coefficiente_DifMinori, ISNULL(ICF.Valore9, 0) AS Premio_EuroTon_PomoBio, ")
            StbSQL.AppendLine("         Materie_Prime_Campionature.tipo, Materie_Prime_Campionature.tipo_cod, Materie_Prime_Campionature.val_cod, Materie_Prime_Campionature.descrizione ")

            StbSQL.AppendLine(" FROM Agenda ")

            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Trasf ON Agenda.PIVA = Contatti_Trasf.Piva and agenda.piva= Contatti_Trasf.cod_contatto ")
            StbSQL.AppendLine(" INNER JOIN ImpresexIndirizzi ImpresexIndirizzi_Trasf ON Contatti_Trasf.Cod_Contatto = ImpresexIndirizzi_Trasf.piva ")
            StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Trasf ON ImpresexIndirizzi_Trasf.cod_indirizzo = Indirizzi_Trasf.cod_indirizzo ")
            StbSQL.AppendLine(" INNER JOIN ISTAT Istat_trasf ON Indirizzi_Trasf.pro_cod_istat = Istat_trasf.PROV AND Indirizzi_Trasf.com_cod_istat = Istat_trasf.COM  ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   ")
            StbSQL.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Mdt_Extra ON Mdt_Extra.PIVA = Mov_Certificato.PIVA AND Mdt_Extra.Sa_Cod = Mov_Certificato.Sa_Cod  ")
            StbSQL.AppendLine(" AND Mdt_Extra.Id_Agenda = Mov_Certificato.Id_Agenda AND Mdt_Extra.Id_Mov = Mov_Certificato.Id_Mov ")

            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" INNER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Accett.PIVA  ")
            StbSQL.AppendLine(" AND Sequenza_Progressivi.Anno = YEAR(Mov_Accett.Data_Movimento)  ")
            StbSQL.AppendLine(" AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Accett.Doc_Numero_Sin  ")
            StbSQL.AppendLine(" AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Accett.Doc_Numero_Des  ")
            StbSQL.AppendLine("")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")
            StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Conferenti ON  Mov_Accett.Cod_IndirizzoRisUm = Indirizzi_Conferenti.cod_indirizzo ")
            StbSQL.AppendLine(" INNER JOIN ISTAT Istat_Conferenti ON Indirizzi_Conferenti.pro_cod_istat = Istat_Conferenti.PROV AND Indirizzi_Conferenti.com_cod_istat = Istat_Conferenti.COM ")
            StbSQL.AppendLine("")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Coop ON Risorse_Umane_Coop.Piva = Contatti_Coop.Piva AND Risorse_Umane_Coop.Cod_Contatto = Contatti_Coop.Cod_Contatto  ")
            If Flag_IndirizzoCoop Then
                StbSQL.AppendLine(" LEFT OUTER JOIN ImpreseXIndirizzi ImpreseXIndirizzi_Coop ON  Contatti_Coop.Cod_Contatto = ImpreseXIndirizzi_Coop.Piva ")
                StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Coop ON  ImpreseXIndirizzi_Coop.Cod_Indirizzo = Indirizzi_Coop.cod_indirizzo ")
                StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Coop ON Indirizzi_Coop.pro_cod_istat = Istat_Coop.PROV AND Indirizzi_Coop.com_cod_istat = Istat_Coop.COM ")
            End If
            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  ")
            If Flag_IndirizzoProd Then
                StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Produttori ON  Mov_Accett.Cod_IndirizzoDestinazione = Indirizzi_Produttori.cod_indirizzo ")
                StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Produttori ON Indirizzi_Produttori.pro_cod_istat = Istat_Produttori.PROV AND Indirizzi_Produttori.com_cod_istat = Istat_Produttori.COM ")
            End If
            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Vettore ON Mov_Accett.Cod_Vettore = Risorse_Umane_Vettore.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Vettore ON Risorse_Umane_Vettore.Piva = Contatti_Vettore.Piva AND Risorse_Umane_Vettore.Cod_Contatto = Contatti_Vettore.Cod_Contatto  ")
            'StbSQL.AppendLine("         /* ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Vettore ON  Mov_Accett.Cod_IndirizzoVettore = Indirizzi_Vettore.cod_indirizzo ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Vettore ON Indirizzi_Vettore.pro_cod_istat = Istat_Vettore.PROV AND Indirizzi_Vettore.com_cod_istat = Istat_Vettore.COM ")
            'StbSQL.AppendLine("         */ ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")

            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  ")
            StbSQL.AppendLine(" INNER JOIN SpecieVegetali ON SpecieVegetali.veg_cod = MP_Raccolta.Veg_Cod  ")
            'MODIFICA DEL 22/07/2010: ci sono prodotti che non hanno la tipologia varietale
            StbSQL.AppendLine(" LEFT OUTER JOIN gruppovarietale ON gruppovarietale.grva_cod = MP_Raccolta.grva_Cod_Veg ")

            StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det ")
            StbSQL.AppendLine(" INNER JOIN Centri_Aziendali C_Az_Raccolta ON Mov_Dett_Raccolta.PIVA = C_Az_Raccolta.PIVA AND Mov_Dett_Raccolta.Sa_Cod = C_Az_Raccolta.sa_cod ")
            StbSQL.AppendLine(" INNER JOIN Fabbricati Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Fabbricato_Cod ")

            'StbSQL.AppendLine("         /* ")
            'StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Fabbricato ON Fabbr_Raccolta.indirizzo_cod = Indirizzi_Fabbricato.cod_indirizzo ")
            'StbSQL.AppendLine(" INNER JOIN ISTAT Istat_Fabbricato ON Indirizzi_Fabbricato.pro_cod_istat = Istat_Fabbricato.PROV AND Indirizzi_Fabbricato.com_cod_istat = Istat_Fabbricato.COM  ")
            'StbSQL.AppendLine(" INNER JOIN UnitaMisura UDM_Qta ON Mov_Dett_Raccolta.Udm_Cod = UDM_Qta.Udm_Cod ")
            'StbSQL.AppendLine("         */ ")

            'StbSQL.AppendLine(" -- INNER JOIN UnitaMisura UDM_Qta ON Mov_Dett_Raccolta.Udm_Cod = UDM_Qta.Udm_Cod ")

            StbSQL.AppendLine(" LEFT OUTER JOIN Imprese_Contratto_Fasi ICF ON ICF.PIVA = Mov_Dett_Raccolta.PIVA AND ICF.Elem_Cod = Mov_Dett_Raccolta.Elem_Cod  ")
            StbSQL.AppendLine(" AND ICF.pro_Cod = Mov_Dett_Raccolta.pro_Cod AND ICF.mat_Cod = Mov_Dett_Raccolta.mat_Cod AND ICF.Progetto_Cod = Mov_Dett_Raccolta.cod_progetto ")
            StbSQL.AppendLine(" AND ICF.fase_Cod = Mov_Dett_Raccolta.fase_Cod AND ICF.lotto = Mov_Dett_Raccolta.Lotto  ")
            'StbSQL.AppendLine(" -- non join sul cal_cod! in ICF è 0, mentre nei dettagli è valorizzato con il progressivo negativo ")
            'StbSQL.AppendLine(" -- AND ICF.cal_cod = Mov_Dett_Raccolta.cal_cod")
            'StbSQL.AppendLine(" -- non join su udm_cod! in ICF è 4 (tonnellate), mentre nei dettagli è 2 (kg) e io ho bisogno di recuperare il premio euro/ton del pomo bio")
            'StbSQL.AppendLine(" -- AND ICF.udm_cod= Mov_Dett_Raccolta.udm_cod ")

            StbSQL.AppendLine(" LEFT OUTER JOIN Imprese_Contratti IC ON ICF.PIVA = IC.PIVA AND ICF.Contratto_Cod = IC.Contratto_Cod ")
            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime_Campionature ")
            StbSQL.AppendLine(" ON Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.cal_cod ")
            StbSQL.AppendLine(" AND Materie_Prime_Campionature.tipo = 'danno' ")
            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione = -1 ")

            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov      = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'")
            StbSQL.AppendLine(" AND Mov_Certificato.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) & "'")
            StbSQL.AppendLine(" AND Mov_Conf.Cau_Mov        = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) & "'")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov    = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'")

            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            'Nel caso dell'export excel id_agenda è 0
            If Id_Agenda <> 0 Then
                StbSQL.AppendLine(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            End If

            If Sa_Cod <> 0 Then
                StbSQL.AppendLine(" AND     Mov_Dest_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Fabbricato_Cod <> 0 Then
                StbSQL.AppendLine(" AND     Mov_Dest_Raccolta.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")
            End If

            StbSQL.AppendLine(" AND Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StbSQL.AppendLine(" AND Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(CStr(enum_SequenzaProgressiviTipi.BolleAccettDaDiversi)) & "  ")

            If Piva_Produttore <> "" Then
                StbSQL.AppendLine(" AND Risorse_Umane_Produttori.Cod_contatto = '" & Agro_SQL_SaveText(Piva_Produttore) & "' ")
            End If

            If Piva_Coop1 <> "" Then
                StbSQL.AppendLine(" AND Risorse_Umane_Coop.Cod_contatto = '" & Agro_SQL_SaveText(Piva_Coop1) & "' ")
            End If

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.AppendLine(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' ")
            End If

            If FiltroConferenti <> "" Then
                StbSQL.AppendLine(FiltroConferenti)
            End If

            If Codice_Specie <> "" Then
                'se è stata selezionata una solo specie
                '(altrimenti c'è il filtro dopo)
                StbSQL.AppendLine(" AND MP_Raccolta.Cod_Articolo = '" & Agro_SQL_SaveText(Codice_Specie) & "' ")
            End If

            If FiltroSpecie <> "" Then
                StbSQL.AppendLine(FiltroSpecie)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.AppendLine(" ORDER BY Mov_Certificato.Doc_Numero_Sin, Mov_Certificato.Doc_Numero, Mov_Certificato.Doc_Numero_Des, Mov_Certificato.Data_Movimento, Mov_Accett.Ora ")
            End If

            'If Ordinamento <> "" Then
            '    StbSQL.AppendLine(Ordinamento)
            'Else
            '    StbSQL.AppendLine(" ORDER BY Mov_Certificato.Doc_Numero_Sin, Mov_Certificato.Doc_Numero, Mov_Certificato.Doc_Numero_Des, Mov_Certificato.Data_Movimento, Mov_Accett.Ora  ")
            'End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' utilizzata dal report estratto conto bolle
    ''' Default:
    '''ByVal Mat_Cod As Integer = 0, _
    '''ByVal FiltroSpecie As String = "", _
    '''ByVal FiltroConferenti As String = "", _
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function EstrattoConto_Bolle(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Fabbricato_Cod As Integer,
                                        ByVal Codice_Specie As String,
                                        ByVal Codice_Conferente As String,
                                        ByVal Piva_Produttore As String,
                                        ByVal Data_Inizio As Date,
                                        ByVal Data_Fine As Date,
                                        ByVal Mat_Cod As Integer,
                                        ByVal FiltroSpecie As String,
                                        ByVal FiltroConferenti As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.EstrattoConto_Bolle"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append("SELECT   " & vbCrLf)
            'StbSQL.Append(" Agenda.Id_Agenda,  Agenda.des_lib, " + vbCrLf)

            StbSQL.Append(" Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, " & vbCrLf)
            StbSQL.Append(" ISNULL(Contatti_Conferenti.Rag_Soc, ' ') AS RagSoc_Conferente, " & vbCrLf)

            'StbSQL.Append(" ISNULL(Contatti_Produttori.Cod_Contatto, ' ') AS Piva_Produttore, " + vbCrLf)
            'StbSQL.Append(" ISNULL(Contatti_Produttori.Rag_Soc, ' ') AS RagSoc_Produttore, " + vbCrLf)

            StbSQL.Append(" MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta, ' ' AS Descr_Specie, ' ' AS Descr_Varieta, " & vbCrLf)
            StbSQL.Append(" MP_Raccolta.Mat_Cod AS Codice_Prodotto, MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, " & vbCrLf)

            StbSQL.Append(" Mov_Accett.Data_Movimento AS Data_Bolla, " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, '' AS Numero_Bolla, " & vbCrLf)
            StbSQL.Append(" Sequenza_Progressivi.Lunghezza_Sin, Sequenza_Progressivi.Lunghezza_Centro, Sequenza_Progressivi.Lunghezza_Des, Sequenza_Progressivi.CarattereFormattazione, " & vbCrLf)

            StbSQL.Append(" Mov_Conf.Mov_Desc AS Mov_Desc_Conf, Mov_Conf.Data_Movimento AS Data_Conf, " & vbCrLf)
            StbSQL.Append(" Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf, " & vbCrLf)
            StbSQL.Append(" Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf,  Mov_Conf.Colli, " & vbCrLf)

            StbSQL.Append(" Mov_Accett.Peso, Mov_Accett.Tara_Veicolo,  Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso, " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta, " & vbCrLf)
            StbSQL.Append(" 0 AS Degrado, 0 AS Netto_Pagamento, " & vbCrLf)

            StbSQL.Append(" Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta " & vbCrLf)

            StbSQL.Append(" , ISNULL( (SELECT TOP 1 ISNULL( materie_prime_calibri.cal_des , ' ') AS Calibro " & vbCrLf)
            StbSQL.Append("         FROM materie_prime_calibri  " & vbCrLf)
            StbSQL.Append("         INNER JOIN Materie_Prime_Campionature MP_Camp_Calibro ON MP_Camp_Calibro.tipo_cod =  materie_prime_calibri.cal_cod " & vbCrLf)
            StbSQL.Append("         WHERE MP_Camp_Calibro.progressivo = Mov_Dett_Raccolta.cal_cod  " & vbCrLf)
            StbSQL.Append("         AND MP_Camp_Calibro.tipo = 'calibro'), 0 ) AS Calibro_Des " & vbCrLf)

            StbSQL.Append(" , ISNULL( (SELECT TOP 1 VAL_COD  " & vbCrLf)
            StbSQL.Append("             FROM Materie_Prime_Campionature MP_Camp_Indice  " & vbCrLf)
            StbSQL.Append("             WHERE MP_Camp_Indice.progressivo = Mov_Dett_Raccolta.cal_cod  " & vbCrLf)
            StbSQL.Append("             AND MP_Camp_Indice.tipo = 'indice' " & vbCrLf)
            'StbSQL.Append("             AND MP_Camp_Indice.tipo_cod = " + CStr(FRUTTAGEL_INDMATCOD_PUNTEGGIO) + " ), 0 ) AS Punteggio " + vbCrLf)
            StbSQL.Append("             AND MP_Camp_Indice.tipo_cod IN ( " & CStr(FRUTTAGEL_INDMATCOD_PUNTEGGIO) & ", " & CStr(FRUTTAGEL_INDMATCOD_GRADOTEND) & ", " & CStr(FRUTTAGEL_INDMATCOD_GRADOBRIX) & " ) ), 0 ) AS Punteggio " & vbCrLf)

            StbSQL.Append(" FROM Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  " & vbCrLf)

            StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Accett.PIVA " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Anno = YEAR(Mov_Accett.Data_Movimento)  " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Accett.Doc_Numero_Sin " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Accett.Doc_Numero_Des  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " & vbCrLf)

            StbSQL.Append(" WHERE   Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "" & vbCrLf)
            StbSQL.Append(" AND     Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL.Append(" AND     Mov_Accett.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Conf.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Raccolta.Cau_Mov = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'" & vbCrLf)

            StbSQL.Append(" AND     Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  " & vbCrLf)
            StbSQL.Append(" AND     Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(CStr(enum_SequenzaProgressiviTipi.BolleAccettDaDiversi)) & "  " & vbCrLf)

            StbSQL.Append(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL.Append(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dest_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Raccolta.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.Append(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' " & vbCrLf)
            End If

            If FiltroConferenti <> "" Then
                StbSQL.Append(FiltroConferenti)
            End If

            If Piva_Produttore <> "" Then
                StbSQL.Append(" AND Contatti_Produttori.Cod_contatto = '" & Agro_SQL_SaveText(Piva_Produttore) & "' " & vbCrLf)
            End If

            If Codice_Specie <> "" Then
                'se è stata selezionata una solo specie
                '(altrimenti c'è il filtro dopo)
                StbSQL.Append(" AND MP_Raccolta.Cod_Articolo = '" & Agro_SQL_SaveText(Codice_Specie) & "' " & vbCrLf)
            End If

            If FiltroSpecie <> "" Then
                StbSQL.Append(FiltroSpecie)
            End If

            If Mat_Cod <> 0 Then
                StbSQL.Append(" AND MP_Raccolta.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Codice_Conferente, Cod_Articolo_Raccolta, Mat_Des_Raccolta ")
            End If

            'If Ordinamento = "" Then
            '    StbSQL.Append("  ORDER BY Codice_Conferente, Cod_Articolo_Raccolta, Mat_Des_Raccolta " + vbCrLf)
            'Else
            '    StbSQL.Append(Ordinamento)
            'End If



            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' report estratto conto imballi: nuova query ottimizzata
    ''' Default:
    ''' Optional ByVal FiltroConferenti As String = ""
    ''' Optional ByVal FinestraTemp_Inizio As Date = agrodatainizio
    '''Optional ByVal FinestraTemp_Fine As Date = agrodatafine
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function EstrattoConto_Imballi(ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Fabbricato_Cod As Integer,
                                            ByVal Mat_Cod As Integer,
                                            ByVal Data_Inizio As Date,
                                            ByVal Data_Fine As Date,
                                            ByVal Data_Saldo_Precedente As Date,
                                            ByVal Codice_Conferente As String,
                                            ByVal FiltroConferenti As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.EstrattoConto_Imballi"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0


            StbSQL.Append(" ( ")
            StbSQL.Append(" SELECT  Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, Contatti_Conferenti.Rag_Soc AS RagSoc_Conferente, " & vbCrLf)
            StbSQL.Append("         MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des, Agenda.lav_cod, " & vbCrLf)
            StbSQL.Append("         Mov_Cont.Doc_Numero_Sin ,Mov_Cont.Doc_Numero, Mov_Cont.Doc_Numero_Des, '' AS Numero_Completo, " & vbCrLf)
            StbSQL.Append("         ISNULL(Sequenza_Progressivi.Lunghezza_Sin, -1 ) AS Lunghezza_Sin, ISNULL(Sequenza_Progressivi.Lunghezza_Centro, -1 ) AS Lunghezza_Centro, " & vbCrLf)
            StbSQL.Append("         ISNULL(Sequenza_Progressivi.Lunghezza_Des, -1 ) AS Lunghezza_Des, ISNULL(Sequenza_Progressivi.CarattereFormattazione, '' ) AS CarattereFormattazione, " & vbCrLf)
            StbSQL.Append("         Mov_Cont.Data_Movimento AS Data_Doc, Mov_Magazzino.CAU_MOV AS Causale, Mov_Dest_Magazzino.Qta   " & vbCrLf)

            StbSQL.Append(" FROM Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Cont ON Agenda.PIVA = Mov_Cont.PIVA AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" LEFT OUTER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Cont.PIVA " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Anno = YEAR(Mov_Cont.Data_Movimento)  " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Cont.Doc_Numero_Sin " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Cont.Doc_Numero_Des  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Cont.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Magazzino ON Mov_Dett_Magazzino.PIVA = Mov_Dest_Magazzino.Piva AND Mov_Dett_Magazzino.Sa_Cod = Mov_Dest_Magazzino.Sa_Cod AND  Mov_Dett_Magazzino.Id_Agenda = Mov_Dest_Magazzino.Id_Agenda AND Mov_Dett_Magazzino.Id_Mov = Mov_Dest_Magazzino.Id_Mov AND Mov_Dett_Magazzino.Id_Mov_Det = Mov_Dest_Magazzino.Id_Mov_Det   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Imballi ON Mov_Dett_Magazzino.Elem_Cod = MP_Imballi.Elem_Cod AND Mov_Dett_Magazzino.Mat_Cod = MP_Imballi.Mat_Cod " & vbCrLf)

            StbSQL.Append(" WHERE   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dett_Magazzino.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dett_Magazzino.Contabilizzato >= 0  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Magazzino.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)

            StbSQL.Append(" AND     Mov_Cont.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Magazzino.Cau_Mov IN ( '" & Agro_SQL_SaveText(CAU_CARICO) & "', '" & Agro_SQL_SaveText(CAU_SCARICO) & "') " & vbCrLf)

            StbSQL.Append(" AND     Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL.Append(" AND     Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dest_Magazzino.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Magazzino.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.Append(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' " & vbCrLf)
            End If

            If FiltroConferenti <> "" Then
                StbSQL.Append(FiltroConferenti)
            End If

            StbSQL.Append(" AND Mov_Dett_Magazzino.Elem_Cod = " & Agro_SQL_SaveNum(BENI_CONFEZ_VEGETALE) & "   " & vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Pro_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            If Mat_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Dett_Magazzino.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If

            StbSQL.Append(" AND Mov_Dett_Magazzino.Cod_Progetto = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Fase_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            'StbSQL.Append(" AND Mov_Dett_Magazzino.Lotto = '" & Agro_SQL_SaveText("") & "'   " + vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Cal_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            'StbSQL.Append(" AND Int_Mov_Dett.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   " + vbCrLf)


            StbSQL.Append(" ) " & vbCrLf)
            StbSQL.Append(" UNION ALL " & vbCrLf)
            StbSQL.Append(" ( " & vbCrLf)



            '**********************************************************
            '***************** GIACENZA ATTUALE ***********************
            '**********************************************************
            StbSQL.Append(" SELECT DISTINCT    Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, " & vbCrLf)
            StbSQL.Append(" Contatti_Conferenti.Rag_Soc AS RagSoc_Conferente, " & vbCrLf)
            StbSQL.Append(" MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des, -1 AS lav_cod, " & vbCrLf)
            StbSQL.Append(" '' AS Doc_Numero_Sin, 0 AS Doc_Numero, '' AS Doc_Numero_Des, '' AS Numero_Completo, " & vbCrLf)
            StbSQL.Append(" -1 AS Lunghezza_Sin, -1 AS Lunghezza_Centro, -1 AS Lunghezza_Des, '' AS CarattereFormattazione, " & vbCrLf)
            StbSQL.Append(" '01/01/1900' AS Data_Doc, '-1' AS Causale " & vbCrLf)

            StbSQL.Append(" , SUM( CASE WHEN    Mov_Magazzino.CAU_MOV='" & Agro_SQL_SaveText(CAU_SCARICO) & "' " & vbCrLf)
            StbSQL.Append("         THEN        -(Mov_Dest_Magazzino.qta) " & vbCrLf)
            StbSQL.Append("         ELSE        Mov_Dest_Magazzino.qta " & vbCrLf)
            StbSQL.Append("         END) AS Giacenza " & vbCrLf)

            StbSQL.Append(" FROM Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Cont ON Agenda.PIVA = Mov_Cont.PIVA AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Cont.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Magazzino ON Mov_Dett_Magazzino.PIVA = Mov_Dest_Magazzino.Piva AND Mov_Dett_Magazzino.Sa_Cod = Mov_Dest_Magazzino.Sa_Cod AND  Mov_Dett_Magazzino.Id_Agenda = Mov_Dest_Magazzino.Id_Agenda AND Mov_Dett_Magazzino.Id_Mov = Mov_Dest_Magazzino.Id_Mov AND Mov_Dett_Magazzino.Id_Mov_Det = Mov_Dest_Magazzino.Id_Mov_Det   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Imballi ON Mov_Dett_Magazzino.Elem_Cod = MP_Imballi.Elem_Cod AND Mov_Dett_Magazzino.Mat_Cod = MP_Imballi.Mat_Cod " & vbCrLf)

            StbSQL.Append(" WHERE   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dett_Magazzino.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dett_Magazzino.Contabilizzato >= 0  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Magazzino.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)

            StbSQL.Append(" AND     Mov_Cont.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Magazzino.Cau_Mov IN ( '" & Agro_SQL_SaveText(CAU_CARICO) & "', '" & Agro_SQL_SaveText(CAU_SCARICO) & "') " & vbCrLf)

            StbSQL.Append(" AND     Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Saldo_Precedente) & " " & vbCrLf)
            StbSQL.Append(" AND     Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dest_Magazzino.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Magazzino.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.Append(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' " & vbCrLf)
            End If

            If FiltroConferenti <> "" Then
                StbSQL.Append(FiltroConferenti)
            End If

            StbSQL.Append(" AND Mov_Dett_Magazzino.Elem_Cod = " & Agro_SQL_SaveNum(BENI_CONFEZ_VEGETALE) & "   " & vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Pro_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            If Mat_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Dett_Magazzino.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If

            StbSQL.Append(" AND Mov_Dett_Magazzino.Cod_Progetto = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Fase_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            'StbSQL.Append(" AND Mov_Dett_Magazzino.Lotto = '" & Agro_SQL_SaveText("") & "'   " + vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Cal_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            'StbSQL.Append(" AND Int_Mov_Dett.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   " + vbCrLf)


            ''StbSQL.Append(" AND EXISTS ( " + vbCrLf)
            'StbSQL.Append(" AND Mov_Dett_Magazzino.mat_Cod IN ( " + vbCrLf)
            'StbSQL.Append("             SELECT      mat_cod  " + vbCrLf)
            'StbSQL.Append("             FROM        Movimenti_Dettagli Mov_Dett_Int  " + vbCrLf)
            'StbSQL.Append("             INNER JOIN  Movimenti  Mov_Int ON Mov_Int.PIVA = Mov_Dett_Int.PIVA AND Mov_Int.Id_Agenda = Mov_Dett_Int.Id_Agenda AND Mov_Int.Id_Mov = Mov_Dett_Int.Id_Mov   " + vbCrLf)
            'StbSQL.Append("             INNER JOIN  Mov_Destinazioni Mov_Dest_Int ON Mov_Dett_Int.PIVA = Mov_Dest_Int.Piva AND Mov_Dett_Int.Sa_Cod = Mov_Dest_Int.Sa_Cod AND  Mov_Dett_Int.Id_Agenda = Mov_Dest_Int.Id_Agenda AND Mov_Dett_Int.Id_Mov = Mov_Dest_Int.Id_Mov AND Mov_Dett_Int.Id_Mov_Det = Mov_Dest_Int.Id_Mov_Des " + vbCrLf)
            'StbSQL.Append("             WHERE       Mov_Dett_Int.Piva = Agenda.Piva " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Int.Cau_Mov IN ( '" + Agro_SQL_SaveText(CAU_CARICO) + "', '" + Agro_SQL_SaveText(CAU_SCARICO) + "') " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Int.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Saldo_Precedente) & " " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Int.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Dest_Int.Sa_Cod =  Mov_Dest_Magazzino.Sa_Cod   " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Dest_Int.Id_Destinazione = Mov_Dest_Magazzino.Id_Destinazione " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Dett_Int.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Dett_Int.Pro_Cod = Mov_Dett_Magazzino.Pro_Cod " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Dett_Int.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod  " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Dett_Int.Cod_Progetto = Mov_Dett_Magazzino.Cod_Progetto " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Dett_Int.Fase_Cod = Mov_Dett_Magazzino.Fase_Cod  " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Dett_Int.Lotto = Mov_Dett_Magazzino.Lotto " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Dett_Int.Cal_Cod = Mov_Dett_Magazzino.Cal_Cod " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Dett_Int.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Dett_Int.Jolly_Int = 0 " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Dett_Int.Contabilizzato >= 0  " + vbCrLf)
            'StbSQL.Append("             AND         Mov_Dest_Int.Tipo_Destinazione = 20  " + vbCrLf)
            'StbSQL.Append("             )  " + vbCrLf)

            StbSQL.Append(" GROUP BY Risorse_Umane_Conferenti.Settore_Des,  Contatti_Conferenti.Rag_Soc, " & vbCrLf)
            StbSQL.Append("           MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des  " & vbCrLf)

            '**********************************************************
            '**************** FINE GIACENZA ATTUALE *******************
            '**********************************************************

            StbSQL.Append(" ) " & vbCrLf)

            ''StbSQL.Append(" ORDER BY  Codice_Conferente, Cod_articolo, Causale, Mov_cont.Doc_Numero " + vbCrLf)
            'StbSQL.Append(" ORDER BY  Codice_Conferente, Cod_articolo, Data_Doc, Causale, Mov_Cont.Doc_Numero " + vbCrLf)
            StbSQL.Append(" ORDER BY  RagSoc_Conferente, MP_Imballi.Mat_Des, Data_Doc, Causale, Mov_Cont.Doc_Numero ")


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


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' report estratto conto imballi: vecchia query che va in timeout
    ''' Default:
    ''' Optional ByVal FiltroConferenti As String = ""
    ''' Optional ByVal FinestraTemp_Inizio As Date = agrodatainizio
    '''Optional ByVal FinestraTemp_Fine As Date = agrodatafine
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Private Function EstrattoConto_Imballi_OLD(ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Fabbricato_Cod As Integer,
                                            ByVal Mat_Cod As Integer,
                                            ByVal Data_Inizio As Date,
                                            ByVal Data_Fine As Date,
                                            ByVal Data_Saldo_Precedente As Date,
                                            ByVal Codice_Conferente As String,
                                            ByVal FiltroConferenti As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.EstrattoConto_Imballi"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0


            StbSQL.Append(" ( ")
            StbSQL.Append(" SELECT  Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, Contatti_Conferenti.Rag_Soc AS RagSoc_Conferente, " & vbCrLf)
            StbSQL.Append("         MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des, Agenda.lav_cod, " & vbCrLf)
            StbSQL.Append("         Mov_Cont.Doc_Numero_Sin ,Mov_Cont.Doc_Numero, Mov_Cont.Doc_Numero_Des, '' AS Numero_Completo, " & vbCrLf)
            StbSQL.Append("         ISNULL(Sequenza_Progressivi.Lunghezza_Sin, -1 ) AS Lunghezza_Sin, ISNULL(Sequenza_Progressivi.Lunghezza_Centro, -1 ) AS Lunghezza_Centro, " & vbCrLf)
            StbSQL.Append("         ISNULL(Sequenza_Progressivi.Lunghezza_Des, -1 ) AS Lunghezza_Des, ISNULL(Sequenza_Progressivi.CarattereFormattazione, '' ) AS CarattereFormattazione, " & vbCrLf)
            StbSQL.Append("         Mov_Cont.Data_Movimento AS Data_Doc, Mov_Magazzino.CAU_MOV AS Causale, Mov_Dest_Magazzino.Qta   " & vbCrLf)

            StbSQL.Append(" FROM Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Cont ON Agenda.PIVA = Mov_Cont.PIVA AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" LEFT OUTER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Cont.PIVA " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Anno = YEAR(Mov_Cont.Data_Movimento)  " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Cont.Doc_Numero_Sin " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Cont.Doc_Numero_Des  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Cont.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Magazzino ON Mov_Dett_Magazzino.PIVA = Mov_Dest_Magazzino.Piva AND Mov_Dett_Magazzino.Sa_Cod = Mov_Dest_Magazzino.Sa_Cod AND  Mov_Dett_Magazzino.Id_Agenda = Mov_Dest_Magazzino.Id_Agenda AND Mov_Dett_Magazzino.Id_Mov = Mov_Dest_Magazzino.Id_Mov AND Mov_Dett_Magazzino.Id_Mov_Det = Mov_Dest_Magazzino.Id_Mov_Det   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Imballi ON Mov_Dett_Magazzino.Elem_Cod = MP_Imballi.Elem_Cod AND Mov_Dett_Magazzino.Mat_Cod = MP_Imballi.Mat_Cod " & vbCrLf)

            StbSQL.Append(" WHERE   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dett_Magazzino.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dett_Magazzino.Contabilizzato >= 0  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Magazzino.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)

            StbSQL.Append(" AND     Mov_Cont.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Magazzino.Cau_Mov IN ( '" & Agro_SQL_SaveText(CAU_CARICO) & "', '" & Agro_SQL_SaveText(CAU_SCARICO) & "') " & vbCrLf)

            StbSQL.Append(" AND     Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL.Append(" AND     Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dest_Magazzino.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Magazzino.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.Append(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' " & vbCrLf)
            End If

            If FiltroConferenti <> "" Then
                StbSQL.Append(FiltroConferenti)
            End If

            StbSQL.Append(" AND Mov_Dett_Magazzino.Elem_Cod = " & Agro_SQL_SaveNum(BENI_CONFEZ_VEGETALE) & "   " & vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Pro_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            If Mat_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Dett_Magazzino.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If

            StbSQL.Append(" AND Mov_Dett_Magazzino.Cod_Progetto = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Fase_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            'StbSQL.Append(" AND Mov_Dett_Magazzino.Lotto = '" & Agro_SQL_SaveText("") & "'   " + vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Cal_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            'StbSQL.Append(" AND Int_Mov_Dett.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   " + vbCrLf)


            StbSQL.Append(" ) ")
            StbSQL.Append(" UNION ALL " & vbCrLf)
            StbSQL.Append(" ( ")



            '**********************************************************
            '***************** GIACENZA ATTUALE ***********************
            '**********************************************************
            StbSQL.Append(" SELECT DISTINCT    Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, " & vbCrLf)
            StbSQL.Append(" Contatti_Conferenti.Rag_Soc AS RagSoc_Conferente, " & vbCrLf)
            StbSQL.Append(" MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des, -1 AS lav_cod, " & vbCrLf)
            StbSQL.Append(" '' AS Doc_Numero_Sin, 0 AS Doc_Numero, '' AS Doc_Numero_Des, '' AS Numero_Completo, " & vbCrLf)
            StbSQL.Append(" -1 AS Lunghezza_Sin, -1 AS Lunghezza_Centro, -1 AS Lunghezza_Des, '' AS CarattereFormattazione, " & vbCrLf)
            StbSQL.Append(" '01/01/1900' AS Data_Doc, '-1' AS Causale, " & vbCrLf)

            StbSQL.Append(" ISNULL(( SELECT ( " & vbCrLf)
            StbSQL.Append("           SELECT ISNULL(SUM(Int_Mov_Dest.Qta) ,0) AS Qta_Totale_Plus  " & vbCrLf)

            StbSQL.Append("           FROM    Agenda Int_Ag " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti Int_Mov ON Int_Ag.PIVA = Int_Mov.PIVA AND Int_Ag.Id_Agenda = Int_Mov.Id_Agenda " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti Int_Mov_Contatti ON Int_Ag.PIVA = Int_Mov_Contatti.PIVA AND Int_Ag.Id_Agenda = Int_Mov_Contatti.Id_Agenda " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti_dettagli Int_Mov_Dett ON Int_Mov.PIVA = Int_Mov_Dett.PIVA AND Int_Mov.Id_Agenda = Int_Mov_Dett.Id_Agenda AND Int_Mov.Id_Mov = Int_Mov_Dett.Id_Mov  " & vbCrLf)
            StbSQL.Append("           INNER JOIN Mov_Destinazioni Int_Mov_Dest ON Int_Mov_Dett.PIVA = Int_Mov_Dest.Piva AND Int_Mov_Dett.Id_Agenda = Int_Mov_Dest.Id_Agenda AND Int_Mov_Dett.Id_Mov = Int_Mov_Dest.Id_Mov AND Int_Mov_Dett.Id_Mov_Det = Int_Mov_Dest.Id_Mov_Det " & vbCrLf)

            StbSQL.Append("           WHERE   Int_Mov_Dett.Validita_inizio <= " & Agro_SQL_SaveDate(AGRODATAFINE) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Validita_Fine >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Contabilizzato >= 0  " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dest.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)
            '                                                               ( '7300', '4100', '7900', '7920')
            StbSQL.Append("           AND     Int_Mov.Cau_Mov IN ( '" & CAU_CARICO & "', '" & CAU_CONFERIMENTO & "', '" & CAU_ACCETTAZIONE_BENI & "', '" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "' )  " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Contatti.Cau_Mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)

            StbSQL.Append("           AND     Int_Mov.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Saldo_Precedente) & "   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dest.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbSQL.Append(" AND Int_Mov_Dest.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
            End If

            StbSQL.Append(" AND Int_Mov_Dest.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Pro_Cod = Mov_Dett_Magazzino.Pro_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Cod_Progetto = Mov_Dett_Magazzino.Cod_Progetto   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Fase_Cod = Mov_Dett_Magazzino.Fase_Cod  " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Lotto = Mov_Dett_Magazzino.Lotto   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Cal_Cod = Mov_Dett_Magazzino.Cal_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Contatti.Cod_RisUm = Mov_Cont.Cod_RisUm  " & vbCrLf)

            StbSQL.Append(" )" & vbCrLf)
            StbSQL.Append(" + " & vbCrLf)
            StbSQL.Append(" ( " & vbCrLf)

            StbSQL.Append(" SELECT ISNULL(-SUM(Int_Mov_Dest.Qta), 0) AS Qta_Totale_Minus  " & vbCrLf)

            StbSQL.Append("           FROM    Agenda Int_Ag " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti Int_Mov ON Int_Ag.PIVA = Int_Mov.PIVA AND Int_Ag.Id_Agenda = Int_Mov.Id_Agenda " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti Int_Mov_Contatti ON Int_Ag.PIVA = Int_Mov_Contatti.PIVA AND Int_Ag.Id_Agenda = Int_Mov_Contatti.Id_Agenda " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti_dettagli Int_Mov_Dett ON Int_Mov.PIVA = Int_Mov_Dett.PIVA AND Int_Mov.Id_Agenda = Int_Mov_Dett.Id_Agenda AND Int_Mov.Id_Mov = Int_Mov_Dett.Id_Mov  " & vbCrLf)
            StbSQL.Append("           INNER JOIN Mov_Destinazioni Int_Mov_Dest ON Int_Mov_Dett.PIVA = Int_Mov_Dest.Piva AND Int_Mov_Dett.Id_Agenda = Int_Mov_Dest.Id_Agenda AND Int_Mov_Dett.Id_Mov = Int_Mov_Dest.Id_Mov AND Int_Mov_Dett.Id_Mov_Det = Int_Mov_Dest.Id_Mov_Det " & vbCrLf)

            StbSQL.Append("           WHERE   Int_Mov_Dett.Validita_inizio <= " & Agro_SQL_SaveDate(AGRODATAFINE) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Validita_Fine >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Contabilizzato >= 0  " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dest.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)
            '( '7350', '4200' )
            StbSQL.Append("           AND     Int_Mov.Cau_Mov IN ( '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO_DIVERSI & "' )  " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Contatti.Cau_Mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)

            StbSQL.Append("           AND     Int_Mov.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Saldo_Precedente) & "   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dest.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbSQL.Append(" AND Int_Mov_Dest.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
            End If

            StbSQL.Append(" AND Int_Mov_Dest.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Pro_Cod = Mov_Dett_Magazzino.Pro_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Cod_Progetto = Mov_Dett_Magazzino.Cod_Progetto   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Fase_Cod = Mov_Dett_Magazzino.Fase_Cod  " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Lotto = Mov_Dett_Magazzino.Lotto   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Cal_Cod = Mov_Dett_Magazzino.Cal_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Contatti.Cod_RisUm = Mov_Cont.Cod_RisUm  " & vbCrLf)

            StbSQL.Append(" ) " & vbCrLf)

            StbSQL.Append("  AS Qta), 0) AS Qta  " & vbCrLf)
            '**********************************************************
            '**************** FINE GIACENZA ATTUALE *******************
            '**********************************************************

            StbSQL.Append(" FROM Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Cont ON Agenda.PIVA = Mov_Cont.PIVA AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Cont.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Magazzino ON Mov_Dett_Magazzino.PIVA = Mov_Dest_Magazzino.Piva AND Mov_Dett_Magazzino.Sa_Cod = Mov_Dest_Magazzino.Sa_Cod AND  Mov_Dett_Magazzino.Id_Agenda = Mov_Dest_Magazzino.Id_Agenda AND Mov_Dett_Magazzino.Id_Mov = Mov_Dest_Magazzino.Id_Mov AND Mov_Dett_Magazzino.Id_Mov_Det = Mov_Dest_Magazzino.Id_Mov_Det   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Imballi ON Mov_Dett_Magazzino.Elem_Cod = MP_Imballi.Elem_Cod AND Mov_Dett_Magazzino.Mat_Cod = MP_Imballi.Mat_Cod " & vbCrLf)

            StbSQL.Append(" WHERE   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dett_Magazzino.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dett_Magazzino.Contabilizzato >= 0  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Magazzino.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)

            StbSQL.Append(" AND     Mov_Cont.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Magazzino.Cau_Mov IN ( '" & Agro_SQL_SaveText(CAU_CARICO) & "', '" & Agro_SQL_SaveText(CAU_SCARICO) & "') " & vbCrLf)

            StbSQL.Append(" AND     Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL.Append(" AND     Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dest_Magazzino.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Magazzino.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.Append(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' " & vbCrLf)
            End If

            If FiltroConferenti <> "" Then
                StbSQL.Append(FiltroConferenti)
            End If

            StbSQL.Append(" AND Mov_Dett_Magazzino.Elem_Cod = " & Agro_SQL_SaveNum(BENI_CONFEZ_VEGETALE) & "   " & vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Pro_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            If Mat_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Dett_Magazzino.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If

            StbSQL.Append(" AND Mov_Dett_Magazzino.Cod_Progetto = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Fase_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            'StbSQL.Append(" AND Mov_Dett_Magazzino.Lotto = '" & Agro_SQL_SaveText("") & "'   " + vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Cal_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            'StbSQL.Append(" AND Int_Mov_Dett.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   " + vbCrLf)

            StbSQL.Append(" ) " & vbCrLf)

            'StbSQL.Append(" ORDER BY  Codice_Conferente, Cod_articolo, Causale, Mov_cont.Doc_Numero " + vbCrLf)
            StbSQL.Append(" ORDER BY  Codice_Conferente, Cod_articolo, Data_Doc, Causale, Mov_Cont.Doc_Numero " & vbCrLf)


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


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' legge le bolle da esportare nel formato xml pubblico
    ''' attualmente esportazione da fruttagel x terremerse
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function BolleXExportDocPubblico(ByVal Piva As String,
                                                ByVal Filtro_Id_Agenda As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.BolleXExportDocPubblico"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append("SELECT   " & vbCrLf)
            StbSQL.Append(" Agenda.Id_Agenda, Agenda.Stato_Export_2, " & vbCrLf)

            StbSQL.Append(" Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, " & vbCrLf)
            StbSQL.Append(" Sequenza_Progressivi.Lunghezza_Sin, Sequenza_Progressivi.Lunghezza_Centro, Sequenza_Progressivi.Lunghezza_Des, Sequenza_Progressivi.CarattereFormattazione, " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Data_Movimento AS Data_Bolla, Mov_Accett.Ora AS Ora_Bolla,  " & vbCrLf)

            StbSQL.Append(" Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf, " & vbCrLf)
            StbSQL.Append(" Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf, Mov_Conf.Data_Movimento AS Data_Conf,  Mov_Conf.Colli, " & vbCrLf)

            StbSQL.Append(" Mov_Accett.Mov_Desc AS Note, " & vbCrLf)

            StbSQL.Append(" Agenda.PIVA AS Piva_Conferitore, ISNULL(Risorse_Umane_Conferenti.Cod_Contatto, '') AS Piva_Conferente, " & vbCrLf)
            StbSQL.Append(" ISNULL(Risorse_Umane_Coop.Cod_Contatto, '') AS Piva_Coop, ISNULL(Risorse_Umane_Coop2.Cod_Contatto, '') AS Piva_Coop_2,  " & vbCrLf)
            StbSQL.Append(" ISNULL(Risorse_Umane_Produttori.Cod_Contatto, '') AS Piva_Produttore," & vbCrLf)

            StbSQL.Append(" Mov_Accett.Peso, Mov_Accett.Tara_Veicolo,  Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso, " & vbCrLf)
            StbSQL.Append(" 0 AS Degrado, 0 AS Netto_Pagamento, " & vbCrLf)

            StbSQL.Append(" Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta, " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta, Mov_Dett_Raccolta.Tara AS Tara, " & vbCrLf)
            StbSQL.Append(" UDM_Qta.Udm_Sim AS Udm_Sim_Raccolta, UDM_Qta.Udm_des AS Udm_des_Raccolta, " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Prezzo_Unitario_Netto AS Prezzo_unitario, " & vbCrLf)

            StbSQL.Append(" ISNULL( (SELECT TOP 1 CONVERT(VARCHAR(50), tipo_cod) + '|' + VAL_COD  " & vbCrLf)
            StbSQL.Append("             FROM Materie_Prime_Campionature MP_Camp_Indice  " & vbCrLf)
            StbSQL.Append("             WHERE MP_Camp_Indice.progressivo = Mov_Dett_Raccolta.cal_cod  " & vbCrLf)
            StbSQL.Append("             AND MP_Camp_Indice.tipo = 'indice' " & vbCrLf)
            StbSQL.Append("             AND MP_Camp_Indice.tipo_cod IN ( " & CStr(FRUTTAGEL_INDMATCOD_PUNTEGGIO) & ", " & CStr(FRUTTAGEL_INDMATCOD_GRADOTEND) & ", " & CStr(FRUTTAGEL_INDMATCOD_GRADOBRIX) & " ) ), '0|0' ) AS Codice_Valore_ParamQualit, " & vbCrLf)

            StbSQL.Append(" ISNULL( (SELECT TOP 1 ISNULL( MP_Camp_Calibro.tipo_cod , '0') AS Cod_Calibro " & vbCrLf)
            StbSQL.Append("         FROM Materie_Prime_Campionature MP_Camp_Calibro " & vbCrLf)
            StbSQL.Append("         WHERE MP_Camp_Calibro.progressivo = Mov_Dett_Raccolta.cal_cod  " & vbCrLf)
            StbSQL.Append("         AND MP_Camp_Calibro.tipo = 'calibro'), 0 ) AS Cod_Calibro, " & vbCrLf)

            StbSQL.Append(" MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta, MP_Raccolta.Mat_Cod AS Codice_Prodotto, MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, " & vbCrLf)
            StbSQL.Append(" MP_Raccolta.Veg_Cod AS Codice_Specie_Gias, MP_Raccolta.Cul_Cod AS Codice_Varieta_Gias," & vbCrLf)
            StbSQL.Append(" MP_Raccolta.Grva_Cod_Veg AS Codice_TipologiaVarietale_Gias, MP_Raccolta.Regolamento " & vbCrLf)

            '02/02/2016: modifica x richiesta di terremerse:
            'aggiunta info sul magazzino e sul trasportatore
            StbSQL.Append(" , Mov_Destinazioni.Sa_Cod as SaCod_Magazzino, Mov_Destinazioni.id_destinazione as FabbricatoCod_Magazzino, Fabbricato_Des " & vbCrLf)
            StbSQL.Append(" , Contatti_Vettore.Cod_Contatto AS CodContatto_Vettore, (Contatti_Vettore.rag_soc + Contatti_Vettore.Nome + ' ' + Contatti_Vettore.Cognome) AS Vettore_RagSoc " & vbCrLf)

            StbSQL.Append(" FROM Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Accett.PIVA " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Anno = YEAR(Mov_Accett.Data_Movimento)  " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Accett.Doc_Numero_Sin " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Accett.Doc_Numero_Des  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop.Cod_RisUm    " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop2 ON Mov_Accett.Extra_Int = Risorse_Umane_Coop2.Cod_RisUm    " & vbCrLf)

            '02/02/2016: modifica x richiesta di terremerse:
            'aggiunta info sul trasportatore
            StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Vettore ON Mov_Accett.Cod_Vettore = Risorse_Umane_Vettore.Cod_RisUm     " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Contatti Contatti_Vettore ON Risorse_Umane_Vettore.Cod_Contatto =  Contatti_Vettore.cod_contatto AND Risorse_Umane_Vettore.piva =  Contatti_Vettore.piva   " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " & vbCrLf)
            StbSQL.Append(" INNER JOIN UnitaMisura UDM_Qta ON Mov_Dett_Raccolta.Udm_Cod = UDM_Qta.Udm_Cod " & vbCrLf)

            '02/02/2016: modifica x richiesta di terremerse:
            'aggiunta info sul magazzino
            StbSQL.Append(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Destinazioni.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Destinazioni.Id_Mov = Mov_Dett_Raccolta.Id_Mov    AND Mov_Destinazioni.Id_Mov_Det = Mov_Dett_Raccolta.Id_Mov_Det     " & vbCrLf)
            StbSQL.Append(" INNER JOIN Fabbricati ON Mov_Destinazioni.PIVA = Fabbricati.PIVA AND Mov_Destinazioni.sa_cod = Fabbricati.sa_cod AND Mov_Destinazioni.id_destinazione = fabbricati.fabbricato_cod   " & vbCrLf)

            StbSQL.Append(" WHERE   Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "" & vbCrLf)
            StbSQL.Append(" AND     Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL.Append(" AND     Mov_Accett.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Conf.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Raccolta.Cau_Mov = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'" & vbCrLf)

            StbSQL.Append(" AND     Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  " & vbCrLf)
            StbSQL.Append(" AND     Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(CStr(enum_SequenzaProgressiviTipi.BolleAccettDaDiversi)) & "  " & vbCrLf)

            StbSQL.Append(Agro_SQL_Save_xFiltroAggiuntivo(Filtro_Id_Agenda,, objParametri))

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            'If Ordinamento = "" Then
            '    StbSQL.Append(" ORDER BY Mov_Accett.Data_Movimento, Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des " + vbCrLf)
            'Else
            '    StbSQL.Append(Ordinamento)
            'End If

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Mov_Accett.Data_Movimento, Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des ")
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


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge le bolle, in base al filtro impostato, da visualizzare a video
    ''' per far scegliere all'utente quale esportare
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function BolleXExportDocPubblico_Filtro(ByVal Piva As String,
                                                    ByVal Cod_Risum As Integer,
                                                    ByVal Cod_Risum_Altro As Integer,
                                                    ByVal Stato_Export_2 As Integer,
                                                    ByVal Data_Inizio As Date,
                                                    ByVal Data_Fine As Date,
                                                    ByVal Filtro_Id_Agenda As String,
                                                    ByVal Filtro_Veg_Cod As String,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.BolleXExportDocPubblico_Filtro"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.Append(" SELECT " & vbCrLf)
            StbSQL.Append(" Agenda.Piva, Agenda.Sa_Cod, Agenda.ID_Agenda, Agenda.Stato_Export, Agenda.Stato_Export_2, Agenda.Blocco_Flag, Agenda.Blocco_Data, Agenda.Blocco_Username, " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Data_Movimento AS Data_Bolla, ")
            StbSQL.Append(" Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, " & vbCrLf)
            StbSQL.Append(" Sequenza_Progressivi.Lunghezza_Sin, Sequenza_Progressivi.Lunghezza_Centro, Sequenza_Progressivi.Lunghezza_Des, Sequenza_Progressivi.CarattereFormattazione, " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Cod_RisUm, Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, " & vbCrLf)
            StbSQL.Append(" ISNULL(Contatti_Conferenti.Rag_Soc, ' ') AS RagSoc_Conferente, " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Cod_RisUm_Altro, " & vbCrLf)
            StbSQL.Append(" ISNULL(Contatti_Coop.Rag_Soc, ' ') AS RagSoc_Coop, " & vbCrLf)

            StbSQL.Append(" MP_Raccolta.Cod_Articolo AS Codice_Articolo, MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Udm_Cod, Mov_Dett_Raccolta.Qta, Mov_Dett_Raccolta.Variazione AS Degrado_Perc," & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Prezzo_Unitario_Netto AS Prezzo_Unitario " & vbCrLf)

            StbSQL.Append(" FROM Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Accett.PIVA " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Anno = YEAR(Mov_Accett.Data_Movimento)  " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Accett.Doc_Numero_Sin " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Accett.Doc_Numero_Des  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  " & vbCrLf)

            StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Contatti Contatti_Coop ON Risorse_Umane_Coop.Piva = Contatti_Coop.Piva AND Risorse_Umane_Coop.Cod_Contatto = Contatti_Coop.Cod_Contatto  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " & vbCrLf)
            StbSQL.Append(" INNER JOIN specievegetali ON specievegetali.veg_cod = MP_Raccolta.veg_cod " & vbCrLf)

            StbSQL.Append(" WHERE Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "" & vbCrLf)

            StbSQL.Append(" AND Mov_Accett.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND Mov_Raccolta.Cau_Mov = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'" & vbCrLf)

            StbSQL.Append(" AND Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  " & vbCrLf)
            StbSQL.Append(" AND Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(CStr(enum_SequenzaProgressiviTipi.BolleAccettDaDiversi)) & "  " & vbCrLf)

            StbSQL.Append(" AND Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL.Append(" AND Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

            StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL.Append(" AND Agenda.Stato_Export_2 = " & Agro_SQL_SaveNum(Stato_Export_2) & " " & vbCrLf)

            'If Cod_Risum <> 0 Then
            '    StbSQL.Append(" AND Mov_Accett.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_Risum) & " " + vbCrLf)
            'End If

            StbSQL.Append(" AND ( " & vbCrLf)
            StbSQL.Append(" (Mov_Accett.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_Risum) & " " & vbCrLf)
            StbSQL.Append(" AND Mov_Accett.Cod_RisUm_Altro = " & Agro_SQL_SaveNum(Cod_Risum_Altro) & ") " & vbCrLf)
            StbSQL.Append(" OR (Mov_Accett.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_Risum_Altro) & ") " & vbCrLf)
            StbSQL.Append(" )" & vbCrLf)


            If Filtro_Id_Agenda <> "" Then
                StbSQL.Append(Filtro_Id_Agenda)
            End If

            If Filtro_Veg_Cod <> "" Then
                StbSQL.Append(Filtro_Veg_Cod)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Data_Bolla, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero_des ASC ")
            End If

            'If Ordinamento = "" Then
            '    StbSQL.Append(" ORDER BY Data_Bolla, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero_des ASC " + vbCrLf)
            'Else
            '    StbSQL.Append(Ordinamento)
            'End If

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


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''Flag_FiltraPomodoro indica se devo filtrare i certificati di pomodoro
    '''quindi leggere solo le bolle normali.
    '''Viene utilizzata dalla stampa massiva bolle (quindi deve filtrare i cert. di pomo)
    '''e dalla stampa singola, che deve ricavare l'id_agenda (quindi NON deve filtrare il pomo)
    ''' Tipo_Select ->     0 = select normale      1 = max num bolla
    ''' 
    ''' In base al filtro impostato legge gli id_agenda delle bolle,
    ''' utilizzata da Leggi_Range_IdAgenda_Bolle per la composizione degli id_agenda per la stampa massiva
    ''' e da NumeroBolla_from_IdAgenda
    ''' Default:
    '''Optional ByVal Doc_Numero_Sin As String = "-999", _
    '''Optional ByVal FiltroAggiuntivo As String = "", _
    '''Optional ByVal Ordinamento As String = "", _
    '''Optional ByVal Data_Inizio As Date = AGRODATAINIZIO, _
    '''Optional ByVal Data_Fine As Date = AGRODATAFINE, _
    '''Optional ByVal Codice_Specie As String = "", _
    '''Optional ByVal FiltroSpecie As String = "", _
    '''Optional ByVal Codice_Conferente As String = "", _
    '''Optional ByVal FiltroConferenti As String = "", _
    '''Optional ByVal Piva_Produttore As String = "", _
    '''Optional ByVal Tipo_Select As Integer = 0
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Bolle_IdAgenda_DocNumero_Leggi(ByVal Piva As String,
                                                    ByVal Id_Agenda As Integer,
                                                    ByVal Flag_FiltraPomodoro As Boolean,
                                                    ByVal Doc_Numero_Sin As String,
                                                    ByVal Data_Inizio As Date,
                                                    ByVal Data_Fine As Date,
                                                    ByVal Codice_Specie As String,
                                                    ByVal FiltroSpecie As String,
                                                    ByVal Codice_Conferente As String,
                                                    ByVal FiltroConferenti As String,
                                                    ByVal Piva_Produttore As String,
                                                    ByVal Piva_Coop1 As String,
                                                    ByVal Piva_Coop2 As String,
                                                    ByVal Tipo_Select As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.IdAgenda_DocNumero_Leggi"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            Select Case Tipo_Select

                Case 0
                    StbSQL.Append("SELECT   Agenda.Id_Agenda, Agenda.des_lib, " & vbCrLf)
                    StbSQL.Append("         Mov_Accett.Data_Movimento, " & vbCrLf)
                    StbSQL.Append("         Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des  " & vbCrLf)

                Case 1
                    StbSQL.Append("SELECT   ISNULL(MAX(Mov_Accett.Doc_Numero), 0) AS Doc_Numero_Max, ISNULL(MIN(Mov_Accett.Doc_Numero), 0) AS Doc_Numero_Min " & vbCrLf)

                Case Else
                    Throw New Exception("Modulo NewCom : " & NomeRoutine & " : " & "Tipo Select non impostato")
                    Return Nothing

            End Select

            StbSQL.Append(" FROM Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda " & vbCrLf)

            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)

            If Piva_Produttore <> "" Then
                StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  " & vbCrLf)
            End If

            If Piva_Coop1 <> "" Then
                StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop1 ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop1.Cod_RisUm  " & vbCrLf)
            End If

            If Piva_Coop2 <> "" Then
                StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop2 ON Mov_Accett.Extra_Int = Risorse_Umane_Coop2.Cod_RisUm  " & vbCrLf)
            End If

            StbSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " & vbCrLf)

            StbSQL.Append(" WHERE Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "" & vbCrLf)
            'bolle non di pomodoro
            If Flag_FiltraPomodoro Then
                StbSQL.Append(" AND Agenda.Tipo_Accettazione  <> -1 " & vbCrLf)
            End If
            StbSQL.Append(" AND Mov_Accett.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND Mov_Raccolta.Cau_Mov = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "' " & vbCrLf)

            StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            If Id_Agenda <> 0 Then
                StbSQL.Append(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " " & vbCrLf)
            End If
            StbSQL.Append(" AND Mov_Accett.Data_Movimento <=" & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL.Append(" AND Mov_Accett.Data_Movimento >=" & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

            If Doc_Numero_Sin <> "-999" Then
                StbSQL.Append(" AND Mov_Accett.Doc_Numero_Sin = '" & Agro_SQL_SaveText(Doc_Numero_Sin) & "' " & vbCrLf)
            End If

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.Append(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' " & vbCrLf)
            End If

            If FiltroConferenti <> "" Then
                StbSQL.Append(FiltroConferenti)
            End If

            If Piva_Produttore <> "" Then
                StbSQL.Append(" AND Risorse_Umane_Produttori.Cod_contatto = '" & Agro_SQL_SaveText(Piva_Produttore) & "' " & vbCrLf)
            End If

            If Piva_Coop1 <> "" Then
                StbSQL.Append(" AND Risorse_Umane_Coop1.Cod_contatto = '" & Agro_SQL_SaveText(Piva_Coop1) & "' " & vbCrLf)
            End If

            If Piva_Coop2 <> "" Then
                StbSQL.Append(" AND Risorse_Umane_Coop2.Cod_contatto = '" & Agro_SQL_SaveText(Piva_Coop2) & "' " & vbCrLf)
            End If

            If Codice_Specie <> "" Then
                'se è stata selezionata una solo specie
                '(altrimenti c'è il filtro dopo)
                StbSQL.Append(" AND MP_Raccolta.Cod_Articolo = '" & Agro_SQL_SaveText(Codice_Specie) & "' " & vbCrLf)
            End If

            If FiltroSpecie <> "" Then
                StbSQL.Append(FiltroSpecie)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ")
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


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' report su carta vidimata: registro di carico e scarico
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RegistroCaricoScaricoPomodoro(ByVal Piva As String,
                                                    ByVal Mat_Cod As Integer,
                                                    ByVal Tipo_Registro As enum_TipoRegistroCaricoScaricoPomodoro,
                                                    ByVal Data_Stampa As Date,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.RegistroCaricoScaricoPomodoro"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.Append("SELECT   " & vbCrLf)

            If Tipo_Registro = enum_TipoRegistroCaricoScaricoPomodoro.Contrattato Then

                StbSQL.Append("         Mov_Certificato.Doc_Numero_Sin AS Doc_Numero_Sin_Cert, Mov_Certificato.Doc_Numero AS Doc_Numero_Cert, Mov_Certificato.Doc_Numero_Des AS Doc_Numero_Des_Cert,  " & vbCrLf)
                StbSQL.Append("         Mov_Certificato.Data_Movimento AS Data_Certificato, " & vbCrLf)

                StbSQL.Append("         Mov_Accett.progr_registrazione AS Numero_Certificato_OLD, Mov_Accett.data_registrazione AS Data_Certificato_OLD, " & vbCrLf)
                StbSQL.Append("         Mov_Accett.extra_int AS Tagliando_Pesa,  Mov_Conf.extra_Str AS Premio_PomoTardivo, " & vbCrLf)

                StbSQL.Append("         IC.Contratto_Numero, IC.Contratto_Nome AS Cod_Agrea_Contratto, IC.Data_Stipulazione,  " & vbCrLf)

            End If

            StbSQL.Append(" MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta, " & vbCrLf)
            StbSQL.Append(" MP_Raccolta.Mat_Cod, MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, " & vbCrLf)

            StbSQL.Append(" Mov_Accett.Data_Movimento AS Data_Bolla, " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des,  " & vbCrLf)
            StbSQL.Append(" Sequenza_Progressivi.Lunghezza_Sin, Sequenza_Progressivi.Lunghezza_Centro, Sequenza_Progressivi.Lunghezza_Des, Sequenza_Progressivi.CarattereFormattazione, " & vbCrLf)

            StbSQL.Append(" Mov_Conf.Mov_Desc AS Mov_Desc_Conf, Mov_Conf.Data_Movimento AS Data_Conf, " & vbCrLf)
            StbSQL.Append(" Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf, " & vbCrLf)
            StbSQL.Append(" Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf,  " & vbCrLf)

            StbSQL.Append(" Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, " & vbCrLf)
            StbSQL.Append(" ISNULL(Contatti_Conferenti.Rag_Soc, ' ') AS RagSoc_Conferente, " & vbCrLf)
            StbSQL.Append(" Indirizzi.ind_des AS ind_des_conferente, Indirizzi.frz_des AS frz_des_conferente, Indirizzi.CAP AS CAP_Conferente, ISTAT.localita AS Comune_Conferente, ISTAT.comuni_prov AS prov_conferente, " & vbCrLf)

            StbSQL.Append(" Mov_Accett.Peso, Mov_Accett.Tara_Veicolo,  Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso, " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta, " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta " & vbCrLf)

            StbSQL.Append(" FROM Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   " & vbCrLf)

            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  " & vbCrLf)

            StbSQL.Append(" INNER JOIN ImpresexIndirizzi ON ImpresexIndirizzi.Piva  = Contatti_Conferenti.Cod_Contatto  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Indirizzi ON ImpresexIndirizzi.Cod_indirizzo = Indirizzi.Cod_indirizzo " & vbCrLf)
            StbSQL.Append(" INNER JOIN ISTAT ON ISTAT.PROV = Indirizzi.pro_cod_istat AND ISTAT.COM= Indirizzi.com_cod_istat " & vbCrLf)

            StbSQL.Append(" INNER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Accett.PIVA " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Anno = YEAR(Mov_Accett.Data_Movimento)  " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Accett.Doc_Numero_Sin " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Accett.Doc_Numero_Des  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " & vbCrLf)

            StbSQL.Append(" INNER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  " & vbCrLf)

            If Tipo_Registro = enum_TipoRegistroCaricoScaricoPomodoro.Contrattato Then
                StbSQL.Append(" INNER JOIN Imprese_Contratto_Fasi ICF ON ICF.PIVA = Mov_Dett_Raccolta.PIVA AND ICF.Elem_Cod = Mov_Dett_Raccolta.Elem_Cod   " & vbCrLf)
                StbSQL.Append(" AND ICF.pro_Cod = Mov_Dett_Raccolta.pro_Cod AND ICF.mat_Cod = Mov_Dett_Raccolta.mat_Cod AND ICF.Progetto_Cod = Mov_Dett_Raccolta.cod_progetto   " & vbCrLf)
                StbSQL.Append(" AND ICF.fase_Cod = Mov_Dett_Raccolta.fase_Cod AND ICF.lotto = Mov_Dett_Raccolta.Lotto    " & vbCrLf)
                'non join sul cal_cod! in ICF è 0, mentre nei dettagli è valorizzato con il progressivo negativo
                'StbSQL.Append(" -- AND ICF.cal_cod = Mov_Dett_Raccolta.cal_cod  " + vbCrLf)
                StbSQL.Append("  AND ICF.udm_cod= Mov_Dett_Raccolta.udm_cod  " & vbCrLf)
                StbSQL.Append("  INNER JOIN Imprese_Contratti IC ON ICF.PIVA = IC.PIVA AND ICF.Contratto_Cod = IC.Contratto_Cod  " & vbCrLf)
            End If


            StbSQL.Append(" WHERE   Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "" & vbCrLf)
            '-1 è la modalità di accettazione del pomodoro
            StbSQL.Append(" AND     Agenda.tipo_accettazione = -1" & vbCrLf)
            StbSQL.Append(" AND     Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL.Append(" AND     Mov_Accett.Cau_Mov      = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Certificato.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Conf.Cau_Mov        = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Raccolta.Cau_Mov    = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'" & vbCrLf)

            StbSQL.Append(" AND     Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  " & vbCrLf)
            StbSQL.Append(" AND     Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(CStr(enum_SequenzaProgressiviTipi.BolleAccettDaDiversi)) & "  " & vbCrLf)

            StbSQL.Append(" AND     Mov_Accett.Data_Movimento = " & Agro_SQL_SaveDate(Data_Stampa) & " " & vbCrLf)

            If Mat_Cod <> 0 Then
                StbSQL.Append(" AND MP_Raccolta.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  " & vbCrLf)
            End If

            StbSQL.Append(" AND     MP_Dettagli_Raccolta.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  " & vbCrLf)

            If Tipo_Registro = enum_TipoRegistroCaricoScaricoPomodoro.Contrattato Then
                '                                                           0 = NO SURGELATO
                StbSQL.Append(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  " & vbCrLf)
            Else
                '                                                           1 = SURGELATO
                StbSQL.Append(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 1  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy = "" Then
                If Tipo_Registro = enum_TipoRegistroCaricoScaricoPomodoro.Contrattato Then
                    StbSQL.Append("   ORDER BY Mov_Certificato.Doc_Numero_Sin, Mov_Certificato.Doc_Numero, Mov_Certificato.Doc_Numero_Des " & vbCrLf)
                Else
                    StbSQL.Append("   ORDER BY Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero_Des " & vbCrLf)
                End If
            Else
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' report RiepilogoConferimentixSpecie
    ''' Default:
    '''Optional ByVal FiltroSpecie As String = ""
    '''Optional ByVal FiltroConferenti As String = ""
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function RiepilogoConferimentixSpecie(ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Fabbricato_Cod As Integer,
                                                ByVal Codice_Specie As String,
                                                ByVal Codice_Conferente As String,
                                                ByVal Data_Inizio As Date,
                                                ByVal Data_Fine As Date,
                                                ByVal FiltroSpecie As String,
                                                ByVal FiltroConferenti As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.RiepilogoConferimentixSpecie"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.Append("SELECT Agenda.Id_Agenda,  Agenda.des_lib,  " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Id_Mov AS Id_Mov_Accett, Mov_Accett.Cau_Mov AS Cau_Mov_Accett, Mov_Accett.Cod_RisUm, " & vbCrLf)
            StbSQL.Append(" Risorse_Umane_Conferenti.settore_des AS Codice_Conferente, Contatti_Conferenti.Rag_Soc AS Rag_Soc_Conferente, " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Peso, Mov_Accett.Tara_Veicolo, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso,  " & vbCrLf)
            StbSQL.Append(" Mov_Raccolta.Id_Mov AS Id_Mov_Raccolta, Mov_Raccolta.Cau_Mov AS Cau_Mov_Raccolta,  " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Id_Mov_Det AS Id_Mov_Det_Raccolta, Mov_Dett_Raccolta.Elem_Cod AS Elem_Cod_Raccolta, " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Pro_Cod AS Pro_Cod_Raccolta, Mov_Dett_Raccolta.Mat_Cod AS Mat_Cod_Raccolta,  " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Cod_Progetto AS Cod_Progetto_Raccolta, Mov_Dett_Raccolta.Fase_Cod AS Fase_Cod_Raccolta,  " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta, " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta, Mov_Dett_Raccolta.Tara AS Tara, " & vbCrLf)
            StbSQL.Append(" MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta, MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, " & vbCrLf)
            StbSQL.Append(" ISNULL( (SELECT VAL_COD " & vbCrLf)
            StbSQL.Append("         FROM Materie_Prime_Campionature " & vbCrLf)
            StbSQL.Append("         WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.cal_cod " & vbCrLf)
            StbSQL.Append("         AND materie_Prime_Campionature.tipo = 'indice' " & vbCrLf)
            'StbSQL.Append("         AND materie_Prime_Campionature.tipo_cod = " + CStr(FRUTTAGEL_INDMATCOD_PUNTEGGIO) + " ), 0 ) AS Punteggio " + vbCrLf)
            StbSQL.Append("         AND materie_Prime_Campionature.tipo_cod IN ( " & CStr(FRUTTAGEL_INDMATCOD_PUNTEGGIO) & ", " & CStr(FRUTTAGEL_INDMATCOD_GRADOTEND) & ", " & CStr(FRUTTAGEL_INDMATCOD_GRADOBRIX) & " ) ), 0 ) AS Punteggio " & vbCrLf)
            StbSQL.Append(" FROM Agenda " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det " & vbCrLf)

            StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " & vbCrLf)

            StbSQL.Append(" WHERE Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "" & vbCrLf)
            StbSQL.Append(" AND Mov_Accett.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND Mov_Raccolta.Cau_Mov = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'" & vbCrLf)
            StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL.Append(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL.Append(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Raccolta.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)

            If Codice_Specie <> "" Then
                'se è stata selezionata una solo specie
                '(altrimenti c'è il filtro dopo)
                StbSQL.Append(" AND MP_Raccolta.Cod_Articolo = '" & Agro_SQL_SaveText(Codice_Specie) & "' " & vbCrLf)
            End If

            If FiltroSpecie <> "" Then
                StbSQL.Append(FiltroSpecie)
            End If

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.Append(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' " & vbCrLf)
            End If

            If FiltroConferenti <> "" Then
                StbSQL.Append(FiltroConferenti)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY MP_Raccolta.Cod_Articolo, Risorse_Umane_Conferenti.settore_Des ")
            End If

            'If Ordinamento = "" Then
            '    StbSQL.Append(" ORDER BY MP_Raccolta.Cod_Articolo, Risorse_Umane_Conferenti.settore_Des " + vbCrLf)
            'Else
            '    StbSQL.Append(Ordinamento)
            'End If


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


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' report saldo imballi: nuova versione ottimizzata
    '''Default:
    '''Optional ByVal FiltroConferenti As String = "", _
    '''Optional ByVal Ordinamento As String = "", _
    '''Optional ByVal FinestraTemp_Inizio As Date = #1/1/1900#, _
    '''Optional ByVal FinestraTemp_Fine As Date = #12/31/2100# _
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function SaldoImballi(ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Fabbricato_Cod As Integer,
                                ByVal Mat_Cod As Integer,
                                ByVal Data_Inizio As Date,
                                ByVal Data_Fine As Date,
                                ByVal Data_Giacenza As Date,
                                ByVal Codice_Conferente As String,
                                ByVal FiltroConferenti As String,
                                ByVal FinestraTemp_Inizio As Date,
                                ByVal FinestraTemp_Fine As Date,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.SaldoImballi"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.Append(" SELECT  Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, " & vbCrLf)
            StbSQL.Append("         Contatti_Conferenti.Rag_Soc AS RagSoc_Conferente, " & vbCrLf)
            StbSQL.Append("         MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des " & vbCrLf)

            StbSQL.Append("         , SUM( CASE WHEN    Mov_Magazzino.CAU_MOV='" & Agro_SQL_SaveText(CAU_CARICO) & "' " & vbCrLf)
            StbSQL.Append("                 AND         Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL.Append("                 AND         Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)
            StbSQL.Append("                 THEN        Mov_Dest_Magazzino.qta " & vbCrLf)
            StbSQL.Append("                 ELSE 0 " & vbCrLf)
            StbSQL.Append("                 END) AS Qta_Totale_Carichi " & vbCrLf)

            StbSQL.Append("         , SUM( CASE WHEN    Mov_Magazzino.CAU_MOV='" & Agro_SQL_SaveText(CAU_SCARICO) & "' " & vbCrLf)
            StbSQL.Append("                 AND         Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL.Append("                 AND         Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)
            StbSQL.Append("                 THEN        Mov_Dest_Magazzino.qta " & vbCrLf)
            StbSQL.Append("                 ELSE 0 " & vbCrLf)
            StbSQL.Append("                 END) AS Qta_Totale_Scarichi " & vbCrLf)

            StbSQL.Append("         , SUM( CASE WHEN    Mov_Magazzino.CAU_MOV='" & Agro_SQL_SaveText(CAU_SCARICO) & "' " & vbCrLf)
            StbSQL.Append("                 THEN        -(Mov_Dest_Magazzino.qta) " & vbCrLf)
            StbSQL.Append("                 ELSE        Mov_Dest_Magazzino.qta " & vbCrLf)
            StbSQL.Append("                 END) AS Giacenza " & vbCrLf)

            StbSQL.Append(" FROM Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Cont ON Agenda.PIVA = Mov_Cont.PIVA AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Cont.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Magazzino ON Mov_Dett_Magazzino.PIVA = Mov_Dest_Magazzino.Piva AND Mov_Dett_Magazzino.Sa_Cod = Mov_Dest_Magazzino.Sa_Cod AND  Mov_Dett_Magazzino.Id_Agenda = Mov_Dest_Magazzino.Id_Agenda AND Mov_Dett_Magazzino.Id_Mov = Mov_Dest_Magazzino.Id_Mov AND Mov_Dett_Magazzino.Id_Mov_Det = Mov_Dest_Magazzino.Id_Mov_Det   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Imballi ON Mov_Dett_Magazzino.Elem_Cod = MP_Imballi.Elem_Cod AND Mov_Dett_Magazzino.Mat_Cod = MP_Imballi.Mat_Cod " & vbCrLf)

            StbSQL.Append(" WHERE   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dett_Magazzino.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dett_Magazzino.Contabilizzato >= 0  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Magazzino.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)

            StbSQL.Append(" AND     Mov_Cont.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Magazzino.Cau_Mov IN ( '" & Agro_SQL_SaveText(CAU_CARICO) & "', '" & Agro_SQL_SaveText(CAU_SCARICO) & "') " & vbCrLf)

            StbSQL.Append(" AND     Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Giacenza) & " " & vbCrLf)
            StbSQL.Append(" AND     Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dest_Magazzino.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Magazzino.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.Append(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' " & vbCrLf)
            End If

            If FiltroConferenti <> "" Then
                StbSQL.Append(FiltroConferenti)
            End If

            StbSQL.Append(" AND Mov_Dett_Magazzino.Elem_Cod = " & Agro_SQL_SaveNum(BENI_CONFEZ_VEGETALE) & "   " & vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Pro_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            If Mat_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Dett_Magazzino.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If

            StbSQL.Append(" AND Mov_Dett_Magazzino.Cod_Progetto = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Fase_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            'StbSQL.Append(" AND Mov_Dett_Magazzino.Lotto = '" & Agro_SQL_SaveText("") & "'   " + vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Cal_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            'StbSQL.Append(" AND Int_Mov_Dett.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   " + vbCrLf)

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StbSQL.Append(" GROUP BY  Risorse_Umane_Conferenti.Settore_Des, " & vbCrLf)
            StbSQL.Append("         Contatti_Conferenti.Rag_Soc, " & vbCrLf)
            StbSQL.Append("         MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des " & vbCrLf)

            'condizione per filtrare le giacenze=0
            StbSQL.Append("  HAVING SUM(CASE WHEN Mov_Magazzino.Cau_Mov= '" & Agro_SQL_SaveText(CAU_SCARICO) & "' THEN -(Mov_Dest_Magazzino.qta) ELSE Mov_Dest_Magazzino.qta END) <> 0 " & vbCrLf)

            'If Ordinamento = "" Then
            '    StbSQL.Append(" ORDER BY  RagSoc_Conferente, MP_Imballi.Mat_Des " + vbCrLf)
            'Else
            '    StbSQL.Append(Ordinamento)
            'End If

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY  RagSoc_Conferente, MP_Imballi.Mat_Des ")
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

    Public Function SaldoImballixTransfertGiacenze(ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Fabbricato_Cod As Integer,
                                                    ByVal Mat_Cod As Integer,
                                                    ByVal Data_Giacenza As Date,
                                                    ByVal Codice_Conferente As String,
                                                    ByVal Cod_Contatto_Conferente As String,
                                                    ByVal FiltroConferenti As String,
                                                    ByVal FinestraTemp_Inizio As Date,
                                                    ByVal FinestraTemp_Fine As Date,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.SaldoImballixTransfertGiacenze"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0
            StbSQL.Append(" SELECT  Agenda.Piva, Mov_Dest_Magazzino.Sa_Cod, Mov_Dest_Magazzino.Id_destinazione, " & vbCrLf)
            StbSQL.Append("         Risorse_Umane_Conferenti.Cod_RisUm, Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, " & vbCrLf)
            StbSQL.Append("         Contatti_Conferenti.Rag_Soc AS RagSoc_Conferente, " & vbCrLf)
            StbSQL.Append("         MP_Imballi.Elem_Cod, MP_Imballi.Mat_Cod, MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des, " & vbCrLf)
            StbSQL.Append("         Mov_Dett_Magazzino.Lotto, Mov_Dett_Magazzino.Udm_Cod, " & vbCrLf)

            StbSQL.Append("         SUM( CASE WHEN    Mov_Magazzino.CAU_MOV='" & Agro_SQL_SaveText(CAU_SCARICO) & "' " & vbCrLf)
            StbSQL.Append("                 THEN        -(Mov_Dest_Magazzino.qta) " & vbCrLf)
            StbSQL.Append("                 ELSE        Mov_Dest_Magazzino.qta " & vbCrLf)
            StbSQL.Append("                 END) AS Giacenza, " & vbCrLf)

            StbSQL.Append("         ISNULL(" & vbCrLf)
            StbSQL.Append("                  (SELECT TOP 1 Cod_Indirizzo  FROM " & vbCrLf)
            StbSQL.Append("                 (" & vbCrLf) 'parentesi della UNION
            StbSQL.Append("                 (" & vbCrLf) 'parentesi della prima query
            StbSQL.Append("                 -- LEGGE L'INDIRIZZO DEL CONTATTO " & vbCrLf)
            StbSQL.Append("                 SELECT Cod_Indirizzo " & vbCrLf)
            StbSQL.Append("                 FROM ContattiXIndirizzi " & vbCrLf)
            StbSQL.Append("                 WHERE Contatti_Conferenti.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto AND ContattiXIndirizzi.Piva = Contatti_Conferenti.Piva  " & vbCrLf)
            StbSQL.Append("                 AND ContattiXIndirizzi.Tipo_Indirizzo IN (1,3) " & vbCrLf)
            StbSQL.Append("                 )" & vbCrLf) 'parentesi della prima query
            StbSQL.Append("         UNION ALL " & vbCrLf)
            StbSQL.Append("                 (" & vbCrLf) 'parentesi della seconda query
            StbSQL.Append("                 -- LEGGE L'INDIRIZZO DELL'IMPRESA GIAS " & vbCrLf)
            StbSQL.Append("                 SELECT TOP 1 Cod_Indirizzo  " & vbCrLf)
            StbSQL.Append("                 FROM ImpreseXIndirizzi  " & vbCrLf)
            StbSQL.Append("                 WHERE Contatti_Conferenti.Cod_Contatto = ImpreseXIndirizzi.Piva " & vbCrLf)
            StbSQL.Append("                 )" & vbCrLf) 'parentesi della seconda query
            StbSQL.Append("         )" & vbCrLf) 'parentesi della union
            StbSQL.Append("                 AS Cod_Indirizzo_Conferente ) " & vbCrLf) 'parentesi del TOP 1
            StbSQL.Append("         , 0 ) AS Cod_Indirizzo_Conferente " & vbCrLf)

            StbSQL.Append(" FROM Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Cont ON Agenda.PIVA = Mov_Cont.PIVA AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Cont.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Magazzino ON Mov_Dett_Magazzino.PIVA = Mov_Dest_Magazzino.Piva AND Mov_Dett_Magazzino.Sa_Cod = Mov_Dest_Magazzino.Sa_Cod AND  Mov_Dett_Magazzino.Id_Agenda = Mov_Dest_Magazzino.Id_Agenda AND Mov_Dett_Magazzino.Id_Mov = Mov_Dest_Magazzino.Id_Mov AND Mov_Dett_Magazzino.Id_Mov_Det = Mov_Dest_Magazzino.Id_Mov_Det   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Imballi ON Mov_Dett_Magazzino.Elem_Cod = MP_Imballi.Elem_Cod AND Mov_Dett_Magazzino.Mat_Cod = MP_Imballi.Mat_Cod " & vbCrLf)

            StbSQL.Append(" WHERE   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dett_Magazzino.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dett_Magazzino.Contabilizzato >= 0  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Magazzino.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)

            StbSQL.Append(" AND     Mov_Cont.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Magazzino.Cau_Mov IN ( '" & Agro_SQL_SaveText(CAU_CARICO) & "', '" & Agro_SQL_SaveText(CAU_SCARICO) & "') " & vbCrLf)

            StbSQL.Append(" AND     Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Giacenza) & " " & vbCrLf)
            StbSQL.Append(" AND     Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbSQL.Append(" AND     Mov_Dest_Magazzino.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If
            If Fabbricato_Cod <> 0 Then
                StbSQL.Append(" AND     Mov_Dest_Magazzino.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)
            End If

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.Append(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' " & vbCrLf)
            End If

            If Cod_Contatto_Conferente <> "" Then
                StbSQL.Append(" AND Risorse_Umane_Conferenti.Cod_contatto = '" & Agro_SQL_SaveText(Cod_Contatto_Conferente) & "' " & vbCrLf)
            End If

            If FiltroConferenti <> "" Then
                StbSQL.Append(FiltroConferenti)
            End If

            StbSQL.Append(" AND Mov_Dett_Magazzino.Elem_Cod = " & Agro_SQL_SaveNum(BENI_CONFEZ_VEGETALE) & "   " & vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Pro_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            If Mat_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Dett_Magazzino.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If

            StbSQL.Append(" AND Mov_Dett_Magazzino.Cod_Progetto = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Fase_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            'StbSQL.Append(" AND Mov_Dett_Magazzino.Lotto = '" & Agro_SQL_SaveText("") & "'   " + vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Cal_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            'StbSQL.Append(" AND Int_Mov_Dett.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   " + vbCrLf)

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StbSQL.Append(" GROUP BY  Risorse_Umane_Conferenti.Settore_Des, Agenda.Piva, Mov_Dest_Magazzino.Sa_Cod, Mov_Dest_Magazzino.Id_destinazione, " & vbCrLf)
            StbSQL.Append("         Contatti_Conferenti.Rag_Soc, " & vbCrLf)
            StbSQL.Append("         MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des, MP_Imballi.Elem_Cod, MP_Imballi.Mat_Cod,  " & vbCrLf)
            StbSQL.Append("         Mov_Dett_Magazzino.Lotto, Mov_Dett_Magazzino.Udm_cod, " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            StbSQL.Append("         Risorse_Umane_Conferenti.Cod_Risum , Contatti_Conferenti.Cod_Contatto, Contatti_Conferenti.Piva " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'StbSQL.Append("  " & xOrderBy)
            'StbSQL.Append("  " & xOrderBy)
            'StbSQL.Append("  " & xOrderBy)
            'StbSQL.Append("  " & xOrderBy)

            'condizione per filtrare le giacenze=0
            StbSQL.Append("  HAVING SUM(CASE WHEN Mov_Magazzino.Cau_Mov= '" & Agro_SQL_SaveText(CAU_SCARICO) & "' THEN -(Mov_Dest_Magazzino.qta) ELSE Mov_Dest_Magazzino.qta END) <> 0 " & vbCrLf)

            'If Ordinamento = "" Then
            '    StbSQL.Append(" ORDER BY  RagSoc_Conferente, MP_Imballi.Mat_Des " + vbCrLf)
            'Else
            '    StbSQL.Append(Ordinamento)
            'End If

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY  RagSoc_Conferente, MP_Imballi.Mat_Des ")
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


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' vecchia query non ottimizzata del reort saldo imballi
    '''Questa è la query della prima versione del report:
    '''vengono letti i movimenti (carichi e scarichi) degli imballi in base all'intervallo temporale selezionato
    '''(eventualmente filtrati x conferente e imballo)
    '''e di questi viene presentata la giacenza alla data attuale.
    'PROBLEMA: se alla data attuale c'è una giacenza di un imballo non movimentato nell'intervallo temporale specificato, tale giacenza non viene visualizzata
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Private Function SaldoImballi_OLD(ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Fabbricato_Cod As Integer,
                                ByVal Mat_Cod As Integer,
                                ByVal Data_Inizio As Date,
                                ByVal Data_Fine As Date,
                                ByVal Data_Giacenza As Date,
                                ByVal Codice_Conferente As String,
                                ByVal FiltroConferenti As String,
                                ByVal FinestraTemp_Inizio As Date,
                                ByVal FinestraTemp_Fine As Date,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.SaldoImballi_OLD"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.Append(" SELECT DISTINCT ")
            StbSQL.Append("           Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, " & vbCrLf)
            StbSQL.Append("           Contatti_Conferenti.Rag_Soc AS RagSoc_Conferente, " & vbCrLf)
            StbSQL.Append("           MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des, " & vbCrLf)


            '**********************************************************
            '******************* TOTALE CARICHI ***********************
            '**********************************************************
            StbSQL.Append("  ISNULL(( SELECT ( " & vbCrLf)

            StbSQL.Append("           SELECT ISNULL(SUM(Int_Mov_Dest.Qta) ,0) AS Qta_Totale_Carichi  " & vbCrLf)

            StbSQL.Append("           FROM    Agenda Int_Ag " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti Int_Mov ON Int_Ag.PIVA = Int_Mov.PIVA AND Int_Ag.Id_Agenda = Int_Mov.Id_Agenda " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti Int_Mov_Contatti ON Int_Ag.PIVA = Int_Mov_Contatti.PIVA AND Int_Ag.Id_Agenda = Int_Mov_Contatti.Id_Agenda " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti_dettagli Int_Mov_Dett ON Int_Mov.PIVA = Int_Mov_Dett.PIVA AND Int_Mov.Id_Agenda = Int_Mov_Dett.Id_Agenda AND Int_Mov.Id_Mov = Int_Mov_Dett.Id_Mov  " & vbCrLf)
            StbSQL.Append("           INNER JOIN Mov_Destinazioni Int_Mov_Dest ON Int_Mov_Dett.PIVA = Int_Mov_Dest.Piva AND Int_Mov_Dett.Id_Agenda = Int_Mov_Dest.Id_Agenda AND Int_Mov_Dett.Id_Mov = Int_Mov_Dest.Id_Mov AND Int_Mov_Dett.Id_Mov_Det = Int_Mov_Dest.Id_Mov_Det " & vbCrLf)

            StbSQL.Append("           WHERE   Int_Mov_Dett.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Contabilizzato >= 0  " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dest.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)
            '                                                               ( '7300', '4100', '7900', '7920')
            StbSQL.Append("           AND     Int_Mov.Cau_Mov IN ( '" & CAU_CARICO & "', '" & CAU_CONFERIMENTO & "', '" & CAU_ACCETTAZIONE_BENI & "', '" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "' )  " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Contatti.Cau_Mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)

            StbSQL.Append("           AND     Int_Mov.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & "   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dest.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbSQL.Append(" AND Int_Mov_Dest.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
            End If

            StbSQL.Append(" AND Int_Mov_Dest.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Pro_Cod = Mov_Dett_Magazzino.Pro_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Cod_Progetto = Mov_Dett_Magazzino.Cod_Progetto   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Fase_Cod = Mov_Dett_Magazzino.Fase_Cod  " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Lotto = Mov_Dett_Magazzino.Lotto   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Cal_Cod = Mov_Dett_Magazzino.Cal_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Contatti.Cod_RisUm = Mov_Cont.Cod_RisUm  " & vbCrLf)


            StbSQL.Append("  ) AS Qta_Totale_Carichi), 0) AS Qta_Totale_Carichi, " & vbCrLf)
            '**********************************************************
            '***************** FINE TOTALE CARICHI ********************
            '**********************************************************


            '**********************************************************
            '******************* TOTALE SCARICHI **********************
            '**********************************************************
            StbSQL.Append("  ISNULL(( SELECT ( " & vbCrLf)

            StbSQL.Append("           SELECT ISNULL(SUM(Int_Mov_Dest.Qta), 0) AS Qta_Totale_Minus  " & vbCrLf)

            StbSQL.Append("           FROM    Agenda Int_Ag " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti Int_Mov ON Int_Ag.PIVA = Int_Mov.PIVA AND Int_Ag.Id_Agenda = Int_Mov.Id_Agenda " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti Int_Mov_Contatti ON Int_Ag.PIVA = Int_Mov_Contatti.PIVA AND Int_Ag.Id_Agenda = Int_Mov_Contatti.Id_Agenda " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti_dettagli Int_Mov_Dett ON Int_Mov.PIVA = Int_Mov_Dett.PIVA AND Int_Mov.Id_Agenda = Int_Mov_Dett.Id_Agenda AND Int_Mov.Id_Mov = Int_Mov_Dett.Id_Mov  " & vbCrLf)
            StbSQL.Append("           INNER JOIN Mov_Destinazioni Int_Mov_Dest ON Int_Mov_Dett.PIVA = Int_Mov_Dest.Piva AND Int_Mov_Dett.Id_Agenda = Int_Mov_Dest.Id_Agenda AND Int_Mov_Dett.Id_Mov = Int_Mov_Dest.Id_Mov AND Int_Mov_Dett.Id_Mov_Det = Int_Mov_Dest.Id_Mov_Det " & vbCrLf)

            StbSQL.Append("           WHERE   Int_Mov_Dett.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Contabilizzato >= 0  " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dest.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)
            '( '7350', '4200' )
            StbSQL.Append("           AND     Int_Mov.Cau_Mov IN ( '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO_DIVERSI & "' )  " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Contatti.Cau_Mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)

            StbSQL.Append("           AND     Int_Mov.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & "   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dest.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbSQL.Append(" AND Int_Mov_Dest.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
            End If

            StbSQL.Append(" AND Int_Mov_Dest.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Pro_Cod = Mov_Dett_Magazzino.Pro_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Cod_Progetto = Mov_Dett_Magazzino.Cod_Progetto   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Fase_Cod = Mov_Dett_Magazzino.Fase_Cod  " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Lotto = Mov_Dett_Magazzino.Lotto   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Cal_Cod = Mov_Dett_Magazzino.Cal_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Contatti.Cod_RisUm = Mov_Cont.Cod_RisUm  " & vbCrLf)

            StbSQL.Append(" ) " & vbCrLf)

            StbSQL.Append(" AS  Qta_Totale_Scarichi) ,0) AS Qta_Totale_Scarichi, " & vbCrLf)
            '**********************************************************
            '**************** FINE TOTALE SCARICHI ********************
            '**********************************************************

            StbSQL.Append(" 0 AS Differenza, " & vbCrLf)

            '**********************************************************
            '***************** GIACENZA ATTUALE ***********************
            '**********************************************************
            StbSQL.Append("  ISNULL(( SELECT ( " & vbCrLf)

            StbSQL.Append("           SELECT ISNULL(SUM(Int_Mov_Dest.Qta) ,0) AS Qta_Totale_Plus  " & vbCrLf)

            StbSQL.Append("           FROM    Agenda Int_Ag " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti Int_Mov ON Int_Ag.PIVA = Int_Mov.PIVA AND Int_Ag.Id_Agenda = Int_Mov.Id_Agenda " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti Int_Mov_Contatti ON Int_Ag.PIVA = Int_Mov_Contatti.PIVA AND Int_Ag.Id_Agenda = Int_Mov_Contatti.Id_Agenda " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti_dettagli Int_Mov_Dett ON Int_Mov.PIVA = Int_Mov_Dett.PIVA AND Int_Mov.Id_Agenda = Int_Mov_Dett.Id_Agenda AND Int_Mov.Id_Mov = Int_Mov_Dett.Id_Mov  " & vbCrLf)
            StbSQL.Append("           INNER JOIN Mov_Destinazioni Int_Mov_Dest ON Int_Mov_Dett.PIVA = Int_Mov_Dest.Piva AND Int_Mov_Dett.Id_Agenda = Int_Mov_Dest.Id_Agenda AND Int_Mov_Dett.Id_Mov = Int_Mov_Dest.Id_Mov AND Int_Mov_Dett.Id_Mov_Det = Int_Mov_Dest.Id_Mov_Det " & vbCrLf)

            StbSQL.Append("           WHERE   Int_Mov_Dett.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Contabilizzato >= 0  " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dest.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)
            '                                                               ( '7300', '4100', '7900', '7920')
            StbSQL.Append("           AND     Int_Mov.Cau_Mov IN ( '" & CAU_CARICO & "', '" & CAU_CONFERIMENTO & "', '" & CAU_ACCETTAZIONE_BENI & "', '" & CAU_ACCETTAZIONE_BENI_DA_DIVERSI & "' )  " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Contatti.Cau_Mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)

            StbSQL.Append("           AND     Int_Mov.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov.Data_Movimento <= " & Agro_SQL_SaveDate(Date.Today) & "   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dest.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbSQL.Append(" AND Int_Mov_Dest.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
            End If

            StbSQL.Append(" AND Int_Mov_Dest.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Pro_Cod = Mov_Dett_Magazzino.Pro_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Cod_Progetto = Mov_Dett_Magazzino.Cod_Progetto   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Fase_Cod = Mov_Dett_Magazzino.Fase_Cod  " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Lotto = Mov_Dett_Magazzino.Lotto   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Cal_Cod = Mov_Dett_Magazzino.Cal_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Contatti.Cod_RisUm = Mov_Cont.Cod_RisUm  " & vbCrLf)


            StbSQL.Append(" )" & vbCrLf)
            StbSQL.Append(" + " & vbCrLf)
            StbSQL.Append(" ( " & vbCrLf)


            StbSQL.Append(" SELECT ISNULL(-SUM(Int_Mov_Dest.Qta), 0) AS Qta_Totale_Minus  " & vbCrLf)

            StbSQL.Append("           FROM    Agenda Int_Ag " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti Int_Mov ON Int_Ag.PIVA = Int_Mov.PIVA AND Int_Ag.Id_Agenda = Int_Mov.Id_Agenda " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti Int_Mov_Contatti ON Int_Ag.PIVA = Int_Mov_Contatti.PIVA AND Int_Ag.Id_Agenda = Int_Mov_Contatti.Id_Agenda " & vbCrLf)
            StbSQL.Append("           INNER JOIN Movimenti_dettagli Int_Mov_Dett ON Int_Mov.PIVA = Int_Mov_Dett.PIVA AND Int_Mov.Id_Agenda = Int_Mov_Dett.Id_Agenda AND Int_Mov.Id_Mov = Int_Mov_Dett.Id_Mov  " & vbCrLf)
            StbSQL.Append("           INNER JOIN Mov_Destinazioni Int_Mov_Dest ON Int_Mov_Dett.PIVA = Int_Mov_Dest.Piva AND Int_Mov_Dett.Id_Agenda = Int_Mov_Dest.Id_Agenda AND Int_Mov_Dett.Id_Mov = Int_Mov_Dest.Id_Mov AND Int_Mov_Dett.Id_Mov_Det = Int_Mov_Dest.Id_Mov_Det " & vbCrLf)

            StbSQL.Append("           WHERE   Int_Mov_Dett.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dett.Contabilizzato >= 0  " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Dest.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)
            '( '7350', '4200' )
            StbSQL.Append("           AND     Int_Mov.Cau_Mov IN ( '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO_DIVERSI & "' )  " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov_Contatti.Cau_Mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)

            StbSQL.Append("           AND     Int_Mov.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "   " & vbCrLf)
            StbSQL.Append("           AND     Int_Mov.Data_Movimento <= " & Agro_SQL_SaveDate(Date.Today) & "   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dest.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbSQL.Append(" AND Int_Mov_Dest.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   " & vbCrLf)
            End If

            StbSQL.Append(" AND Int_Mov_Dest.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Pro_Cod = Mov_Dett_Magazzino.Pro_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Cod_Progetto = Mov_Dett_Magazzino.Cod_Progetto   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Fase_Cod = Mov_Dett_Magazzino.Fase_Cod  " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Lotto = Mov_Dett_Magazzino.Lotto   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Cal_Cod = Mov_Dett_Magazzino.Cal_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Dett.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod   " & vbCrLf)

            StbSQL.Append(" AND Int_Mov_Contatti.Cod_RisUm = Mov_Cont.Cod_RisUm  " & vbCrLf)

            StbSQL.Append(" ) " & vbCrLf)

            StbSQL.Append(" AS Giacenza_Attuale), 0) AS Giacenza_Attuale " & vbCrLf)
            '**********************************************************
            '**************** FINE GIACENZA ATTUALE *******************
            '**********************************************************


            StbSQL.Append(" FROM Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Cont ON Agenda.PIVA = Mov_Cont.PIVA AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Cont.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Magazzino ON Mov_Dett_Magazzino.PIVA = Mov_Dest_Magazzino.Piva AND Mov_Dett_Magazzino.Sa_Cod = Mov_Dest_Magazzino.Sa_Cod AND  Mov_Dett_Magazzino.Id_Agenda = Mov_Dest_Magazzino.Id_Agenda AND Mov_Dett_Magazzino.Id_Mov = Mov_Dest_Magazzino.Id_Mov AND Mov_Dett_Magazzino.Id_Mov_Det = Mov_Dest_Magazzino.Id_Mov_Det   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Imballi ON Mov_Dett_Magazzino.Elem_Cod = MP_Imballi.Elem_Cod AND Mov_Dett_Magazzino.Mat_Cod = MP_Imballi.Mat_Cod " & vbCrLf)

            StbSQL.Append(" WHERE   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dett_Magazzino.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dett_Magazzino.Contabilizzato >= 0  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Magazzino.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    " & vbCrLf)

            StbSQL.Append(" AND     Mov_Cont.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Magazzino.Cau_Mov IN ( '" & Agro_SQL_SaveText(CAU_CARICO) & "', '" & Agro_SQL_SaveText(CAU_SCARICO) & "') " & vbCrLf)

            StbSQL.Append(" AND     Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL.Append(" AND     Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dest_Magazzino.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Magazzino.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.Append(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' " & vbCrLf)
            End If

            If FiltroConferenti <> "" Then
                StbSQL.Append(FiltroConferenti)
            End If

            StbSQL.Append(" AND Mov_Dett_Magazzino.Elem_Cod = " & Agro_SQL_SaveNum(BENI_CONFEZ_VEGETALE) & "   " & vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Pro_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            If Mat_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Dett_Magazzino.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   " & vbCrLf)
            End If

            StbSQL.Append(" AND Mov_Dett_Magazzino.Cod_Progetto = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Fase_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            'StbSQL.Append(" AND Mov_Dett_Magazzino.Lotto = '" & Agro_SQL_SaveText("") & "'   " + vbCrLf)

            StbSQL.Append(" AND Mov_Dett_Magazzino.Cal_Cod = " & Agro_SQL_SaveNum(0) & "   " & vbCrLf)

            'StbSQL.Append(" AND Int_Mov_Dett.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   " + vbCrLf)

            If xOrderBy = "" Then
                StbSQL.Append(" ORDER BY  RagSoc_Conferente, MP_Imballi.Mat_Des " & vbCrLf)
            Else
                StbSQL.Append(Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ")
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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' report excel dei trasportatori
    '''default:
    '''Optional ByVal FiltroSpecie As String = ""
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Trasportatori_XLS(ByVal ProgressivoGIAS As Integer,
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Fabbricato_Cod As Integer,
                                        ByVal Codice_Specie As String,
                                        ByVal Data_Inizio As Date,
                                        ByVal Data_Fine As Date,
                                        ByVal FiltroSpecie As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.Trasportatori_XLS"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.Append("SELECT   " & vbCrLf)
            'StbSQL.Append(" Agenda.Id_Agenda,  Agenda.des_lib, " + vbCrLf)

            StbSQL.Append(" Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, '' AS Numero_Bolla, " & vbCrLf)
            StbSQL.Append(" Sequenza_Progressivi.Lunghezza_Sin, Sequenza_Progressivi.Lunghezza_Centro, Sequenza_Progressivi.Lunghezza_Des, Sequenza_Progressivi.CarattereFormattazione, " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Data_Movimento AS Data_Bolla, " & vbCrLf)

            StbSQL.Append(" Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf, " & vbCrLf)
            StbSQL.Append(" Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf, '' AS Numero_Conf,  Mov_Conf.Data_Movimento AS Data_Conf,  " & vbCrLf)

            StbSQL.Append(" Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, " & vbCrLf)
            StbSQL.Append(" ISNULL(Contatti_Conferenti.Rag_Soc, ' ') AS RagSoc_Conferente, " & vbCrLf)
            StbSQL.Append(" ISNULL(Contatti_Produttori.Cod_Contatto, ' ') AS Piva_Produttore, " & vbCrLf)
            StbSQL.Append(" ISNULL(Contatti_Produttori.Rag_Soc, ' ') AS RagSoc_Produttore, " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Peso, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso,  0 AS Netto_Trasportato, " & vbCrLf)
            StbSQL.Append(" ' ' AS Codice_Zona, Contatti_Trasportatori.Rag_Soc AS RagSoc_Trasportatore, " & vbCrLf)

            StbSQL.Append("         ISNULL(  (SELECT Mdt_Extra.Targa + '_' + Mdt_Extra.N_Immatricolazione_Rimorchio " & vbCrLf)
            StbSQL.Append("                     FROM Movimenti Mov_Certificato  " & vbCrLf)
            StbSQL.Append("                     INNER JOIN Mov_Dettaglio_Tecnico_Extra Mdt_Extra ON Mdt_Extra.PIVA = Mov_Certificato.PIVA AND Mdt_Extra.Sa_Cod = Mov_Certificato.Sa_Cod " & vbCrLf)
            StbSQL.Append("                     AND Mdt_Extra.Id_Agenda = Mov_Certificato.Id_Agenda AND Mdt_Extra.Id_Mov = Mov_Certificato.Id_Mov " & vbCrLf)
            StbSQL.Append("                     WHERE Agenda.PIVA = Mov_Certificato.PIVA  " & vbCrLf)
            StbSQL.Append("                     AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod  " & vbCrLf)
            StbSQL.Append("                     AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda  " & vbCrLf)
            StbSQL.Append("                     ), '_' ) AS Targhe, '' AS Targa_Automezzo, '' AS Targa_Rimorchio " & vbCrLf)

            StbSQL.Append(" FROM Agenda " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda  " & vbCrLf)

            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Trasportatori ON Mov_Accett.Cod_Vettore = Risorse_Umane_Trasportatori.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Trasportatori ON Risorse_Umane_Trasportatori.Piva = Contatti_Trasportatori.Piva AND Risorse_Umane_Trasportatori.Cod_Contatto = Contatti_Trasportatori.Cod_Contatto  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Accett.PIVA " & vbCrLf)
            StbSQL.Append("             AND  Sequenza_Progressivi.Anno = YEAR(Mov_Accett.Data_Movimento)  " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Accett.Doc_Numero_Sin " & vbCrLf)
            StbSQL.Append("             AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Accett.Doc_Numero_Des  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " & vbCrLf)

            StbSQL.Append(" WHERE   Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "" & vbCrLf)
            StbSQL.Append(" AND     Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            StbSQL.Append(" AND     Mov_Accett.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Conf.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) & "'" & vbCrLf)
            StbSQL.Append(" AND     Mov_Raccolta.Cau_Mov = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'" & vbCrLf)

            StbSQL.Append(" AND     Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  " & vbCrLf)
            StbSQL.Append(" AND     Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(CStr(enum_SequenzaProgressiviTipi.BolleAccettDaDiversi)) & "  " & vbCrLf)

            StbSQL.Append(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " " & vbCrLf)
            StbSQL.Append(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " " & vbCrLf)

            StbSQL.Append(" AND     Mov_Dest_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            StbSQL.Append(" AND     Mov_Dest_Raccolta.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)

            'Select Case ProgressivoGIAS
            '    Case enum_CodiceGIAS_Clienti.Fruttagel
            '        'filtro il TRASPORTATORE C/PROPRIO che è un vettore fittizio
            '        StbSQL.Append(" AND Contatti_Trasportatori.cod_contatto <> '-1999999998' " + vbCrLf)
            'End Select

            If Codice_Specie <> "" Then
                'se è stata selezionata una solo specie
                '(altrimenti c'è il filtro dopo)
                StbSQL.Append(" AND MP_Raccolta.Cod_Articolo = '" & Agro_SQL_SaveText(Codice_Specie) & "' " & vbCrLf)
            End If

            If FiltroSpecie <> "" Then
                StbSQL.Append(FiltroSpecie)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            'If Ordinamento = "" Then
            '    StbSQL.Append(" ORDER BY RagSoc_Trasportatore, Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des,Mov_Accett.Data_Movimento " + vbCrLf)
            'Else
            '    StbSQL.Append(Ordinamento)
            'End If

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY RagSoc_Trasportatore, Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des,Mov_Accett.Data_Movimento ")
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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' default:
    '''Optional ByVal Sa_Cod As Integer = 0, _
    '''Optional ByVal Fabbricato_Cod As Integer = 0, _
    '''Optional ByVal FiltroAggiuntivo As String = "", _
    '''Optional ByVal Ordinamento As String = "", _
    '''Optional ByVal Data_Inizio_Accett As Date = AGRODATAINIZIO, _
    '''Optional ByVal Data_Fine_Accett As Date = AGRODATAFINE
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function ExportJDEdwards_Leggi(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Fabbricato_Cod As Integer,
                                        ByVal FiltroAggiuntivo As String,
                                        ByVal Ordinamento As String,
                                        ByVal Data_Inizio_Accett As Date,
                                        ByVal Data_Fine_Accett As Date,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.ExportJDEdwards_Leggi"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0


            'MODIFICA AL 02/12/2009: si legge il cod_conferente dal campo settore_des di Risorse_Umane
            'invece che dal codice legato all'impresa

            'MODIFICA DELL' 08/03/2010: si leggono anche i dati di blocco

            '/*     INIZIO SPECIE NO POMODORO   */
            StbSQL.Append(" ( " & vbCrLf)
            StbSQL.Append(" SELECT " & vbCrLf)
            StbSQL.Append(" Agenda.Piva, Agenda.Sa_Cod, Agenda.ID_Agenda, Agenda.Stato_Export, Agenda.Blocco_Flag, Agenda.Blocco_Data, Agenda.Blocco_Username, Agenda.Tipo_Accettazione, " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Doc_Numero_Sin AS Prefisso_Bolla, Mov_Accett.Doc_Numero AS Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Suffisso_Bolla,  " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Data_Movimento AS Data_Bolla, ")
            'StbSQL.Append(" ImpCod_Conf.val_cod AS Codice_Conferente, " + vbCrLf)
            StbSQL.Append(" RisUm_Conf.Settore_Des AS Codice_Conferente, " & vbCrLf)
            StbSQL.Append(" MP_Raccolta.Cod_Articolo AS Codice_Articolo, ISNULL(MP_Raccolta.Codice_Esterno, '') AS Codice_Esterno, Mov_Dett_Raccolta.Lotto, " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Udm_Cod, Mov_Dett_Raccolta.Qta AS Peso_Netto, Mov_Dett_Raccolta.Variazione AS Degrado_Perc," & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Prezzo_Unitario_Netto AS Prezzo, 0 AS Prezzo_Contratto, " & vbCrLf)
            StbSQL.Append(" Mov_Conf.Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Mov_Conf.Doc_Numero)) + Mov_Conf.Doc_Numero_Des AS Numero_DDT,   ")
            StbSQL.Append(" Mov_Conf.Data_Movimento AS Data_DDT, " & vbCrLf)
            StbSQL.Append(" Fabbr_Raccolta.Fabbricato_Des AS Stabilimento, Fabbr_Raccolta_Codici.val_cod AS Codice_Stabilimento " & vbCrLf)

            StbSQL.Append(" , '_' AS GradoBrix_IndicePrezzo, 0 AS Franchigia_DifMaggiori, 0 AS Coefficiente_DifMinori, 0 AS Tot_Dif_Magg, 0 AS Tot_Dif_Min, '' AS Premio_PomoTardivo " & vbCrLf)

            StbSQL.Append(" FROM Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det " & vbCrLf)
            StbSQL.Append(" INNER JOIN Fabbricati Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Fabbricato_Cod " & vbCrLf)
            StbSQL.Append(" INNER JOIN Fabbricati_Codici Fabbr_Raccolta_Codici ON Fabbr_Raccolta_Codici.Piva = Fabbr_Raccolta.PIVA AND Fabbr_Raccolta_Codici.Sa_Cod = Fabbr_Raccolta.SA_COD AND Fabbr_Raccolta_Codici.Fabbricato_Cod = Fabbr_Raccolta.Fabbricato_Cod " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane RisUm_Conf ON RisUm_Conf.Cod_RisUm = Mov_Accett.Cod_RisUm" & vbCrLf)
            'StbSQL.Append(" INNER JOIN Imprese ImpreseConf ON RisUm_Conf.Cod_Contatto = ImpreseConf.PIVA " + vbCrLf)
            'StbSQL.Append(" INNER JOIN Imprese_Codici ImpCod_Conf ON ImpreseConf.PIVA = ImpCod_Conf.PIVA " + vbCrLf)

            StbSQL.Append(" WHERE Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "" & vbCrLf)
            StbSQL.Append(" AND Agenda.Tipo_Accettazione <> -1 " & vbCrLf)

            StbSQL.Append(" AND Mov_Accett.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND Mov_Conf.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) & "'" & vbCrLf)
            StbSQL.Append(" AND Mov_Raccolta.Cau_Mov = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'" & vbCrLf)

            StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            'StbSQL.Append(" AND ImpCod_Conf.Id_Cod = " & Agro_SQL_SaveNum(CA_COD_SOCIO) & " " + vbCrLf)

            StbSQL.Append(" AND Fabbr_Raccolta_Codici.Id_Cod = " & Agro_SQL_SaveNum(CA_COD_STABILIMENTO) & " " & vbCrLf)

            StbSQL.Append(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine_Accett) & " " & vbCrLf)
            StbSQL.Append(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio_Accett) & " " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If

            If Fabbricato_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Dest_Raccolta.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StbSQL.Append(" ) " & vbCrLf)
            '/*     FINE SPECIE NO POMODORO   */

            StbSQL.Append(" UNION ALL " & vbCrLf)

            '/*     INIZIO POMODORO   */
            StbSQL.Append(" ( " & vbCrLf)


            StbSQL.Append(" SELECT " & vbCrLf)
            StbSQL.Append(" Agenda.Piva, Agenda.Sa_Cod, Agenda.ID_Agenda, Agenda.Stato_Export, Agenda.Blocco_Flag, Agenda.Blocco_Data, Agenda.Blocco_Username, Agenda.Tipo_Accettazione, " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Doc_Numero_Sin AS Prefisso_Bolla, Mov_Accett.Doc_Numero AS Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Suffisso_Bolla,  " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Data_Movimento AS Data_Bolla, ")
            'StbSQL.Append(" ImpCod_Conf.val_cod AS Codice_Conferente, " + vbCrLf)
            StbSQL.Append(" RisUm_Conf.Settore_Des AS Codice_Conferente, " & vbCrLf)
            StbSQL.Append(" MP_Raccolta.Cod_Articolo AS Codice_Articolo, ISNULL(MP_Raccolta.Codice_Esterno, '') AS Codice_Esterno, Mov_Dett_Raccolta.Lotto, " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Udm_Cod, Mov_Dett_Raccolta.Qta AS Peso_Netto, Mov_Dett_Raccolta.Variazione AS Degrado_Perc," & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Prezzo_Unitario_Netto AS Prezzo, Mov_Dett_Raccolta.Prezzo_Unitario AS Prezzo_Contratto,  " & vbCrLf)
            StbSQL.Append(" Mov_Conf.Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Mov_Conf.Doc_Numero)) + Mov_Conf.Doc_Numero_Des AS Numero_DDT,   ")
            StbSQL.Append(" Mov_Conf.Data_Movimento AS Data_DDT, " & vbCrLf)
            StbSQL.Append(" Fabbr_Raccolta.Fabbricato_Des AS Stabilimento, Fabbr_Raccolta_Codici.val_cod AS Codice_Stabilimento, " & vbCrLf)

            StbSQL.Append("         ISNULL( (SELECT TOP 1 VAL_COD + '_' + REPLACE( CONVERT(Varchar(500),  MPXPrezzi.Prezzo	), '.',',' )  " & vbCrLf)
            StbSQL.Append("                     FROM Materie_Prime_Campionature MP_Camp_Indice " & vbCrLf)
            StbSQL.Append("                     INNER JOIN ParametriQualitativiXPrezzi MPXPrezzi " & vbCrLf)
            StbSQL.Append("                     ON    MP_Camp_Indice.tipo = MPXPrezzi.tipo " & vbCrLf)
            StbSQL.Append("                     AND MP_Camp_Indice.tipo_cod = MPXPrezzi.tipo_cod " & vbCrLf)
            StbSQL.Append("                     AND MP_Camp_Indice.udm_cod = MPXPrezzi.udm_cod " & vbCrLf)
            StbSQL.Append("                     WHERE MP_Camp_Indice.progressivo = Mov_Dett_Raccolta.cal_cod " & vbCrLf)
            StbSQL.Append("                     AND MPXPrezzi.Piva_SuperUser = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StbSQL.Append("                     AND MPXPrezzi.Piva = Mov_Dett_Raccolta.Piva  " & vbCrLf)
            StbSQL.Append("                     AND MP_Camp_Indice.tipo = 'indice' " & vbCrLf)
            StbSQL.Append("                     AND MPXPrezzi.validita_inizio <= Mov_Accett.Data_Movimento " & vbCrLf)
            StbSQL.Append("                     AND MPXPrezzi.validita_fine >= Mov_Accett.Data_Movimento " & vbCrLf)
            StbSQL.Append("                     AND CONVERT(FLOAT, replace( MP_Camp_Indice.val_cod ,',', '.') )  >= MPXPrezzi.VALORE_MIN " & vbCrLf)
            StbSQL.Append("                     AND CONVERT(FLOAT,replace( MP_Camp_Indice.val_cod ,',', '.') )  <= MPXPrezzi.VALORE_Max " & vbCrLf)
            StbSQL.Append("                     ), '_' ) AS GradoBrix_IndicePrezzo, " & vbCrLf)

            StbSQL.Append("         ISNULL(ICF.Valore5, 3) AS Franchigia_DifMaggiori, ISNULL(ICF.Valore7, 0.5) AS Coefficiente_DifMinori " & vbCrLf)
            StbSQL.Append(" , MPC_1.Tot_Dif_Magg, MPC_2.Tot_Dif_Min, Mov_Certificato.extra_Str AS Premio_PomoTardivo " & vbCrLf)

            StbSQL.Append(" FROM Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det " & vbCrLf)
            StbSQL.Append(" INNER JOIN Fabbricati Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Fabbricato_Cod " & vbCrLf)
            StbSQL.Append(" INNER JOIN Fabbricati_Codici Fabbr_Raccolta_Codici ON Fabbr_Raccolta_Codici.Piva = Fabbr_Raccolta.PIVA AND Fabbr_Raccolta_Codici.Sa_Cod = Fabbr_Raccolta.SA_COD AND Fabbr_Raccolta_Codici.Fabbricato_Cod = Fabbr_Raccolta.Fabbricato_Cod " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane RisUm_Conf ON RisUm_Conf.Cod_RisUm = Mov_Accett.Cod_RisUm" & vbCrLf)

            'TEMP
            'StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " + vbCrLf)
            'StbSQL.Append(" INNER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  " + vbCrLf)
            'StbSQL.Append(" INNER JOIN SpecieVegetali ON SpecieVegetali.veg_cod = MP_Raccolta.Veg_Cod  " + vbCrLf)

            StbSQL.Append(" LEFT OUTER JOIN Imprese_Contratto_Fasi ICF ON ICF.PIVA = Mov_Dett_Raccolta.PIVA AND ICF.Elem_Cod = Mov_Dett_Raccolta.Elem_Cod  " & vbCrLf)
            StbSQL.Append(" AND ICF.pro_Cod = Mov_Dett_Raccolta.pro_Cod AND ICF.mat_Cod = Mov_Dett_Raccolta.mat_Cod AND ICF.Progetto_Cod = Mov_Dett_Raccolta.cod_progetto " & vbCrLf)
            StbSQL.Append(" AND ICF.fase_Cod = Mov_Dett_Raccolta.fase_Cod AND ICF.lotto = Mov_Dett_Raccolta.Lotto  " & vbCrLf)
            'StbSQL.Append(" -- non join sul cal_cod! in ICF è 0, mentre nei dettagli è valorizzato con il progressivo negativo " + vbCrLf)
            'StbSQL.Append(" -- AND ICF.cal_cod = Mov_Dett_Raccolta.cal_cod" + vbCrLf)
            StbSQL.Append(" AND ICF.udm_cod= Mov_Dett_Raccolta.udm_cod " & vbCrLf)

            StbSQL.Append(" INNER JOIN (SELECT progressivo, SUM(CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.'))) AS Tot_Dif_Magg  " & vbCrLf)
            StbSQL.Append("             FROM Materie_Prime_Campionature " & vbCrLf)
            StbSQL.Append("             WHERE  Materie_Prime_Campionature.tipo = 'danno'  " & vbCrLf)
            StbSQL.Append("             AND Materie_Prime_Campionature.tipo_cod IN (1,2,3) " & vbCrLf)
            StbSQL.Append("             GROUP BY progressivo) AS MPC_1 on MPC_1.progressivo = Mov_Dett_Raccolta.cal_cod  " & vbCrLf)

            StbSQL.Append(" INNER JOIN (SELECT progressivo, SUM(CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.'))) AS Tot_Dif_Min  " & vbCrLf)
            StbSQL.Append("             FROM Materie_Prime_Campionature " & vbCrLf)
            StbSQL.Append("             WHERE  Materie_Prime_Campionature.tipo = 'danno'  " & vbCrLf)
            StbSQL.Append("             AND Materie_Prime_Campionature.tipo_cod IN (4,5,6,7) " & vbCrLf)
            StbSQL.Append("             GROUP BY progressivo) AS MPC_2 on MPC_2.progressivo = Mov_Dett_Raccolta.cal_cod  " & vbCrLf)

            StbSQL.Append(" WHERE Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "" & vbCrLf)
            StbSQL.Append(" AND Agenda.Tipo_Accettazione = -1 " & vbCrLf)
            ''                                                           0 = NO SURGELATO
            'StbSQL.Append(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  " + vbCrLf)

            StbSQL.Append(" AND Mov_Accett.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND Mov_Certificato.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) & "'" & vbCrLf)
            StbSQL.Append(" AND Mov_Conf.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) & "'" & vbCrLf)
            StbSQL.Append(" AND Mov_Raccolta.Cau_Mov = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'" & vbCrLf)

            StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            'StbSQL.Append(" AND ImpCod_Conf.Id_Cod = " & Agro_SQL_SaveNum(CA_COD_SOCIO) & " " + vbCrLf)

            StbSQL.Append(" AND Fabbr_Raccolta_Codici.Id_Cod = " & Agro_SQL_SaveNum(CA_COD_STABILIMENTO) & " " & vbCrLf)

            StbSQL.Append(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine_Accett) & " " & vbCrLf)
            StbSQL.Append(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio_Accett) & " " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If

            If Fabbricato_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Dest_Raccolta.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StbSQL.Append(" ) " & vbCrLf)
            '/*     FINE POMODORO   */

            'If Ordinamento = "" Then
            '    StbSQL.Append(" ORDER BY Data_Bolla, Numero_Bolla ASC  " + vbCrLf)
            'Else
            '    StbSQL.Append(Ordinamento)
            'End If

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Data_Bolla, Numero_Bolla ASC ")
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



    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Private Function ExportJDEdwards_Leggi_OLD(ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Fabbricato_Cod As Integer,
                                                ByVal FiltroAggiuntivo As String,
                                                ByVal Ordinamento As String,
                                                ByVal Data_Inizio_Accett As Date,
                                                ByVal Data_Fine_Accett As Date,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.ExportJDEdwards_Leggi_OLD"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            'MODIFICA AL 02/12/2009: si legge il cod_conferente dal campo settore_des di Risorse_Umane
            'invece che dal codice legato all'impresa

            'MODIFICA DELL' 08/03/2010: si leggono anche i dati di blocco

            StbSQL.Append(" SELECT " & vbCrLf)
            StbSQL.Append(" Agenda.Piva, Agenda.Sa_Cod, Agenda.ID_Agenda, Agenda.Stato_Export, Agenda.Blocco_Flag, Agenda.Blocco_Data, Agenda.Blocco_Username, " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Doc_Numero_Sin AS Prefisso_Bolla, Mov_Accett.Doc_Numero AS Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Suffisso_Bolla,  " & vbCrLf)
            StbSQL.Append(" Mov_Accett.Data_Movimento AS Data_Bolla, ")
            'StbSQL.Append(" ImpCod_Conf.val_cod AS Codice_Conferente, " + vbCrLf)
            StbSQL.Append(" RisUm_Conf.Settore_Des AS Codice_Conferente, " & vbCrLf)
            StbSQL.Append(" MP_Raccolta.Cod_Articolo AS Codice_Articolo, Mov_Dett_Raccolta.Lotto, " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Udm_Cod, Mov_Dett_Raccolta.Qta AS Peso_Netto, Mov_Dett_Raccolta.Variazione AS Degrado_Perc," & vbCrLf)
            StbSQL.Append(" Mov_Dett_Raccolta.Prezzo_Unitario AS Prezzo,  " & vbCrLf)
            StbSQL.Append(" Mov_Conf.Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Mov_Conf.Doc_Numero)) + Mov_Conf.Doc_Numero_Des AS Numero_DDT,   ")
            StbSQL.Append(" Mov_Conf.Data_Movimento AS Data_DDT, " & vbCrLf)
            StbSQL.Append(" Fabbr_Raccolta.Fabbricato_Des AS Stabilimento, Fabbr_Raccolta_Codici.val_cod AS Codice_Stabilimento " & vbCrLf)

            StbSQL.Append(" FROM Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det " & vbCrLf)
            StbSQL.Append(" INNER JOIN Fabbricati Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Fabbricato_Cod " & vbCrLf)
            StbSQL.Append(" INNER JOIN Fabbricati_Codici Fabbr_Raccolta_Codici ON Fabbr_Raccolta_Codici.Piva = Fabbr_Raccolta.PIVA AND Fabbr_Raccolta_Codici.Sa_Cod = Fabbr_Raccolta.SA_COD AND Fabbr_Raccolta_Codici.Fabbricato_Cod = Fabbr_Raccolta.Fabbricato_Cod " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane RisUm_Conf ON RisUm_Conf.Cod_RisUm = Mov_Accett.Cod_RisUm" & vbCrLf)
            'StbSQL.Append(" INNER JOIN Imprese ImpreseConf ON RisUm_Conf.Cod_Contatto = ImpreseConf.PIVA " + vbCrLf)
            'StbSQL.Append(" INNER JOIN Imprese_Codici ImpCod_Conf ON ImpreseConf.PIVA = ImpCod_Conf.PIVA " + vbCrLf)

            StbSQL.Append(" WHERE Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "" & vbCrLf)

            StbSQL.Append(" AND Mov_Accett.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND Mov_Conf.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) & "'" & vbCrLf)
            StbSQL.Append(" AND Mov_Raccolta.Cau_Mov = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'" & vbCrLf)

            StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            'StbSQL.Append(" AND ImpCod_Conf.Id_Cod = " & Agro_SQL_SaveNum(CA_COD_SOCIO) & " " + vbCrLf)

            StbSQL.Append(" AND Fabbr_Raccolta_Codici.Id_Cod = " & Agro_SQL_SaveNum(CA_COD_STABILIMENTO) & " " & vbCrLf)

            StbSQL.Append(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine_Accett) & " " & vbCrLf)
            StbSQL.Append(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio_Accett) & " " & vbCrLf)

            If Sa_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " & vbCrLf)
            End If

            If Fabbricato_Cod <> 0 Then
                StbSQL.Append(" AND Mov_Dest_Raccolta.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            'If Ordinamento = "" Then
            '    StbSQL.Append(" ORDER BY Data_Bolla, Numero_Bolla ASC  " + vbCrLf)
            'Else
            '    StbSQL.Append(Ordinamento)
            'End If

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Data_Bolla, Numero_Bolla ASC ")
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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function CertificatiPomodoro_DistinctAnni_Leggi(ByVal Piva As String,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AccettazioneDaDiversi.CertificatiPomodoro_DistinctAnni_Leggi"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.Length = 0

            'StbSQL.Append("SELECT   Agenda.Id_Agenda, Agenda.des_lib, " + vbCrLf)
            'StbSQL.Append("         Mov_Certificato.Data_Movimento, " + vbCrLf)
            'StbSQL.Append("         Mov_Accett.Doc_Numero_Sin AS Doc_Numero_Sin_Bolla, Mov_Accett.Doc_Numero AS Doc_Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Doc_Numero_Des_Bolla,  " + vbCrLf)
            'StbSQL.Append("         Mov_Certificato.Doc_Numero_Sin AS Doc_Numero_Sin_Cert, Mov_Certificato.Doc_Numero AS Doc_Numero_Cert, Mov_Certificato.Doc_Numero_Des AS Doc_Numero_Des_Cert  " + vbCrLf)

            StbSQL.Append(" SELECT   DISTINCT YEAR(Mov_Certificato.Data_Movimento) AS Anno_Certificato " & vbCrLf)

            StbSQL.Append(" FROM    Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   " & vbCrLf)

            'StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " + vbCrLf)
            'StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  " + vbCrLf)

            'StbSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  " + vbCrLf)
            'StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   " + vbCrLf)
            'StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " + vbCrLf)

            'If Tipo_Certificato = enum_CodificaStampe.Certificato_Pomodoro_Esterno Then
            '    'il certificato esterno va stampato solo per i non surgelati
            '    StbSQL.Append(" INNER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  " + vbCrLf)
            'End If

            StbSQL.Append(" WHERE Agenda.Lav_Cod            = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "" & vbCrLf)
            StbSQL.Append(" AND Agenda.Tipo_Accettazione    = -1 " & vbCrLf)
            StbSQL.Append(" AND Agenda.Piva                 = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StbSQL.Append(" AND Mov_Accett.Cau_Mov          = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND Mov_Certificato.Cau_Mov     = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) & "'" & vbCrLf)
            'StbSQL.Append(" AND Mov_Raccolta.Cau_Mov        = '" + Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) + "' " + vbCrLf)
            ''StbSQL.Append(" AND Mov_Certificato.Data_Movimento   <=" & Agro_SQL_SaveDate(Validita_Fine) & " " + vbCrLf)
            ''StbSQL.Append(" AND Mov_Certificato.Data_Movimento   >=" & Agro_SQL_SaveDate(Validita_Inizio) & " " + vbCrLf)
            'StbSQL.Append(" AND Mov_Accett.Data_Movimento   <=" & Agro_SQL_SaveDate(Data_Fine) & " " + vbCrLf)
            ' StbSQL.Append(" AND Mov_Accett.Data_Movimento   >=" & Agro_SQL_SaveDate(Data_Inizio) & " " + vbCrLf)

            'If Tipo_Certificato = enum_CodificaStampe.Certificato_Pomodoro_Esterno Then
            '    'il certificato esterno va stampato solo per i non surgelati
            '    StbSQL.Append(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  " + vbCrLf)
            'End If

            'If Id_Agenda <> 0 Then
            '    StbSQL.Append(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " " + vbCrLf)
            'End If

            If Piva <> "" Then
                StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ")
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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' In base al filtro impostato legge gli id_agenda dei certificati di pomodoro,
    ''' utilizzata da Leggi_Range_IdAgenda_Bolle per la composizione degli id_agenda per la stampa massiva
    ''' Default:
    ''' ByVal Data_Inizio As Date = AGRODATAINIZIO
    ''' ByVal Data_Fine As Date = AGRODATAFINE
    ''' ByVal Codice_Specie As String = ""
    ''' ByVal FiltroSpecie As String = ""
    ''' ByVal Codice_Conferente As String = ""
    ''' ByVal FiltroConferenti As String = ""
    ''' ByVal Piva_Produttore As String = ""
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function CertificatiPomodoro_DatiMinimi_Leggi(ByVal Piva As String,
                                                        ByVal Id_Agenda As Integer,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AccettazioneDaDiversi.CertificatiPomodoro_SoloAgenda_Leggi"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.Append("SELECT   Agenda.Id_Agenda, Agenda.des_lib, " & vbCrLf)
            StbSQL.Append("         Mov_Certificato.Data_Movimento, " & vbCrLf)
            StbSQL.Append("         Mov_Accett.Doc_Numero_Sin AS Doc_Numero_Sin_Bolla, Mov_Accett.Doc_Numero AS Doc_Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Doc_Numero_Des_Bolla,  " & vbCrLf)
            StbSQL.Append("         Mov_Certificato.Doc_Numero_Sin AS Doc_Numero_Sin_Cert, Mov_Certificato.Doc_Numero AS Doc_Numero_Cert, Mov_Certificato.Doc_Numero_Des AS Doc_Numero_Des_Cert  " & vbCrLf)

            StbSQL.Append(" FROM    Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   " & vbCrLf)

            'StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " + vbCrLf)
            'StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  " + vbCrLf)

            'StbSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  " + vbCrLf)
            'StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   " + vbCrLf)
            'StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " + vbCrLf)

            'If Tipo_Certificato = enum_CodificaStampe.Certificato_Pomodoro_Esterno Then
            '    'il certificato esterno va stampato solo per i non surgelati
            '    StbSQL.Append(" INNER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  " + vbCrLf)
            'End If

            StbSQL.Append(" WHERE Agenda.Lav_Cod            = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "" & vbCrLf)
            StbSQL.Append(" AND Agenda.Tipo_Accettazione    = -1 " & vbCrLf)
            StbSQL.Append(" AND Agenda.Piva                 = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            StbSQL.Append(" AND Mov_Accett.Cau_Mov          = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'" & vbCrLf)
            StbSQL.Append(" AND Mov_Certificato.Cau_Mov     = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) & "'" & vbCrLf)
            'StbSQL.Append(" AND Mov_Raccolta.Cau_Mov        = '" + Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) + "' " + vbCrLf)
            ''StbSQL.Append(" AND Mov_Certificato.Data_Movimento   <=" & Agro_SQL_SaveDate(Validita_Fine) & " " + vbCrLf)
            ''StbSQL.Append(" AND Mov_Certificato.Data_Movimento   >=" & Agro_SQL_SaveDate(Validita_Inizio) & " " + vbCrLf)
            'StbSQL.Append(" AND Mov_Accett.Data_Movimento   <=" & Agro_SQL_SaveDate(Data_Fine) & " " + vbCrLf)
            ' StbSQL.Append(" AND Mov_Accett.Data_Movimento   >=" & Agro_SQL_SaveDate(Data_Inizio) & " " + vbCrLf)

            'If Tipo_Certificato = enum_CodificaStampe.Certificato_Pomodoro_Esterno Then
            '    'il certificato esterno va stampato solo per i non surgelati
            '    StbSQL.Append(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  " + vbCrLf)
            'End If

            If Id_Agenda <> 0 Then
                StbSQL.Append(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " " & vbCrLf)
            End If

            If Piva <> "" Then
                StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ")
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

    '##################################################################################
    Public Function AnnoCertificatoPomodoro_from_IdAgenda(ByVal Piva As String,
                                                          ByVal Id_Agenda As Integer,
                                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                          ) As Integer

        Dim dt As DataTable
        Dim Anno As Integer = 0

        dt = CertificatiPomodoro_DatiMinimi_Leggi(Piva, Id_Agenda, "", "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Anno = CDate(dt.Rows(0).Item("Data_Movimento")).Year
        End If

        Return Anno

    End Function

    '##################################################################################
    Public Function AnnoCertificatiPomodoro_from_Str_Id_Agenda(ByVal Piva As String,
                                                               ByVal Str_Id_Agenda_Cert As String,
                                                               ByRef MessaggioErrore As String,
                                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                               ) As Integer

        Dim dt As DataTable
        Dim Anno As Integer = 0
        Dim Filtro As String

        MessaggioErrore = ""

        If Str_Id_Agenda_Cert <> "" Then

            Str_Id_Agenda_Cert = Str_Id_Agenda_Cert.Replace("|", ",")

            Filtro = " Agenda.Id_Agenda IN (" & Agro_SQL_Save_Clausola_IN(Str_Id_Agenda_Cert) & ")"

            dt = CertificatiPomodoro_DistinctAnni_Leggi(Piva, Filtro, "", objParametri)

            If Not IsNothing(dt) Then

                Select Case dt.Rows.Count
                    Case 0 'ERRORE
                        MessaggioErrore = "Errore, anno non restituito."
                    Case 1 'CASO OK
                        Anno = dt.Rows(0).Item("Anno_Certificato")
                    Case Else 'ERRORE
                        MessaggioErrore = "Errore, sono stati selezionati certificati di anni diversi, occorre selezionare certificati dello stesso anno."
                End Select
            Else
                MessaggioErrore = "Errore, dati non restituiti."
            End If

        Else
            MessaggioErrore = "Filtro id_agenda vuoto."
        End If

        Return Anno

    End Function

    Public Function CertificatiPomodoro_Export_NON_USARE(ByVal Piva As String,
                                                        ByVal Data_Inizio As Date,
                                                        ByVal Data_Fine As Date,
                                                        ByVal Cod_RisUm As Integer,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.AccettazioneDaDiversi.CertificatiPomodoro_Export"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.AppendLine(" SELECT  Agenda.Id_Agenda, Agenda.des_lib,  ")
            StbSQL.AppendLine("         Mov_Certificato.Doc_Numero_Sin AS Doc_Numero_Sin_Cert, Mov_Certificato.Doc_Numero AS Doc_Numero_Cert, Mov_Certificato.Doc_Numero_Des AS Doc_Numero_Des_Cert,  ")
            StbSQL.AppendLine("         Mov_Certificato.Data_Movimento AS Data_Certificato, ")
            StbSQL.AppendLine("         Mov_Dett_Conf.Tagliando_Pesa, Mov_Dett_Conf.Premio_Complessivo AS Premio_PomoTardivo, ")
            StbSQL.AppendLine("         Mov_Dett_Conf.Desc_Appezzamenti AS Descrizione_Appezzamento, Mov_Dett_Conf.Cod_Varieta AS CodiceVarieta_Distretto, ")
            'StbSQL.AppendLine("         Mov_Certificato.extra_int AS Tagliando_Pesa,  Mov_Certificato.extra_Str AS Premio_PomoTardivo, ")
            'StbSQL.AppendLine("         Mov_Certificato.Username_Note AS Descrizione_Appezzamento, Mov_Certificato.Sezionale_Cod AS CodiceVarieta_Distretto, ")
            StbSQL.AppendLine("         ISNULL(IC.Contratto_Numero, '0') AS Contratto_Numero, ISNULL(IC.Contratto_Nome, '') AS Cod_Agrea_Contratto, ISNULL(IC.Data_Stipulazione, '01/01/1900') AS Data_Stipulazione,  ")
            StbSQL.AppendLine("         ISNULL(  (SELECT TOP 1 Imprese_Contratti_Clausole.Clausola_Cod_Alternativo + '|' + convert(varchar(250), Imprese_Contratti_Clausole.Validita_Inizio, 103) ")
            StbSQL.AppendLine("                     FROM Imprese_Contratti_Clausole ")
            StbSQL.AppendLine("                     WHERE Mov_Certificato.Num_Protocollo = Imprese_Contratti_Clausole.Clausola_Cod ")
            StbSQL.AppendLine("                     ), '' ) AS Clausola, ")
            StbSQL.AppendLine("         Mov_Accett.Data_Movimento AS Data_Accett, Mov_Accett.Ora AS Ora_Accett,   ")
            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, ")
            StbSQL.AppendLine("         Mov_Accett.Mov_Desc AS Note, ")
            StbSQL.AppendLine("         Mov_Accett.Peso, Mov_Accett.Tara_Veicolo, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso,  ")
            StbSQL.AppendLine("         ISNULL(  (SELECT    VAL_Cod ")
            StbSQL.AppendLine("                     FROM    Imprese_Codici ImpCodici_Conf ")
            StbSQL.AppendLine("                     WHERE   ImpCodici_Conf.Piva = Contatti_Conferenti.Cod_Contatto ")
            StbSQL.AppendLine("                     AND     ImpCodici_Conf.ID_COD = 1125 ")
            StbSQL.AppendLine("                     ), '' ) AS Codice_Unione_OP, ")
            StbSQL.AppendLine("         ISNULL(  (SELECT    VAL_Cod ")
            StbSQL.AppendLine("                     FROM    Imprese_Codici ImpCodici_Conf ")
            StbSQL.AppendLine("                     WHERE   ImpCodici_Conf.Piva = Contatti_Conferenti.Cod_Contatto ")
            StbSQL.AppendLine("                     AND     ImpCodici_Conf.ID_COD = 1126 ")
            StbSQL.AppendLine("                     ), '' ) AS Codice_OP, ")
            StbSQL.AppendLine("         Mov_Accett.Cod_RisUm,Mov_Accett.Doc_Numero_Visualizzato,")
            StbSQL.AppendLine("         Contatti_Conferenti.Cod_Contatto AS Cod_Contatto_Conferente,  ")
            StbSQL.AppendLine("         Contatti_Conferenti.Rag_Soc AS Rag_Soc_Conferente,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Coop.Cod_Contatto, '') AS Cod_Contatto_Coop,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Coop.Rag_Soc, '') AS Rag_Soc_Coop,  ")
            StbSQL.AppendLine("         Contatti_Produttori.Cod_Contatto AS Cod_Contatto_Produttore,  ")
            StbSQL.AppendLine("         Contatti_Produttori.Rag_Soc AS Rag_Soc_Produttore, ISNULL(Contatti_Produttori.Codice_Fiscale, '') AS Codice_Fiscale_Produttore,  ")
            StbSQL.AppendLine("         Contatti_Produttori.Nome AS Nome_Produttore, Contatti_Produttori.Cognome AS Cognome_Produttore,  ")
            StbSQL.AppendLine("         ISNULL(  (SELECT    VAL_Cod ")
            StbSQL.AppendLine("                     FROM        Imprese_Codici ImpCodici_Trasf ")
            StbSQL.AppendLine("                     WHERE       ImpCodici_Trasf.Piva = Contatti_Trasf.Cod_Contatto ")
            StbSQL.AppendLine("                     AND         ImpCodici_Trasf.ID_COD = 1127 ")
            StbSQL.AppendLine("                     ), '0' ) AS Codice_Ass_Ind, ")
            StbSQL.AppendLine("         ISNULL(  (SELECT    VAL_Cod ")
            StbSQL.AppendLine("                     FROM        Imprese_Codici ImpCodici_Trasf ")
            StbSQL.AppendLine("                     WHERE       ImpCodici_Trasf.Piva = Contatti_Trasf.Cod_Contatto ")
            StbSQL.AppendLine("                     AND         ImpCodici_Trasf.ID_COD = 1128 ")
            StbSQL.AppendLine("                     ), '0' ) AS Codice_Az_Trasf, ")
            StbSQL.AppendLine("         Contatti_Trasf.cod_contatto AS Cod_Contatto_Trasf, Contatti_Trasf.rag_soc AS Rag_Soc_Trasf,  ")
            StbSQL.AppendLine("         Indirizzi_Trasf.ind_des AS ind_des_Trasf, Indirizzi_Trasf.frz_des AS frz_des_Trasf, Indirizzi_Trasf.CAP AS cap_Trasf,  ")
            StbSQL.AppendLine("         Indirizzi_Trasf.stato AS stato_Trasf, Indirizzi_Trasf.pro_cod_istat AS pro_cod_istat_Trasf, ")
            StbSQL.AppendLine("         Indirizzi_Trasf.com_cod_istat AS com_cod_istat_Trasf, Istat_Trasf.LOCALITA AS localita_Trasf, Istat_Trasf.COMUNI_PROV AS comuni_prov_Trasf,  ")
            StbSQL.AppendLine("         ISNULL(  (SELECT VAL_Cod ")
            StbSQL.AppendLine("                     FROM Centri_Aziendali_Codici ")
            StbSQL.AppendLine("                     WHERE Centri_Aziendali_Codici.Piva = C_Az_Raccolta.PIVA ")
            StbSQL.AppendLine("                     AND  Centri_Aziendali_Codici.SA_COD = C_Az_Raccolta.SA_COD ")
            'StbSQL.AppendLine("                     AND Fabbricati_Codici.fabbricato_cod = Fabbr_Raccolta.vas_cod ")
            StbSQL.AppendLine("                     AND Centri_Aziendali_Codici.ID_COD = 1129 ")
            StbSQL.AppendLine("                     ), '0' ) AS Codice_Stab_Reg, ")
            StbSQL.AppendLine("         ISNULL(Contatti_Vettore.Rag_Soc,'') AS Rag_Soc_Vettore,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Vettore.Nome,'') AS Nome_Vettore, ISNULL(Contatti_Vettore.Cognome,'') AS Cognome_Vettore,  ")
            StbSQL.AppendLine("         Mdt_Extra.Targa AS Targa_Automezzo, Mdt_Extra.N_Immatricolazione_Rimorchio AS Targa_Rimorchio, Mdt_Extra.Provvigione AS Premio_Pomo_Bio, ")
            StbSQL.AppendLine("         Mov_Conf.Mov_Desc AS Mov_Desc_Conf,  ")
            StbSQL.AppendLine("         Mov_Conf.Data_Movimento AS Data_Conf, Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf,  ")
            StbSQL.AppendLine("         Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf, ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Elem_Cod AS Elem_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Pro_Cod AS Pro_Cod_Raccolta, Mov_Dett_Raccolta.Mat_Cod AS Mat_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Cod_Progetto AS Cod_Progetto_Raccolta, Mov_Dett_Raccolta.Fase_Cod AS Fase_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Listino_Cod AS Listino_Cod_Raccolta, Mov_Dett_Raccolta.Tara AS Tara, Mov_Dett_Raccolta.Prezzo_Unitario, Mov_Dett_Raccolta.Prezzo_Unitario_Netto,  ")
            StbSQL.AppendLine("         MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta,  ")
            StbSQL.AppendLine("         MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, MP_Raccolta.Cul_Cod, MP_Raccolta.Veg_Cod, SpecieVegetali.Veg_Des, ISNULL(MP_Raccolta.GRVA_COD_VEG, 0) AS GRVA_COD_VEG, ISNULL(GruppoVarietale.Grva_Des, '') AS Grva_Des,  ")
            StbSQL.AppendLine("         MP_Raccolta.Regolamento,  ")
            'StbSQL.AppendLine("         ISNULL( (SELECT TOP 1 VAL_COD + '_' + REPLACE( CONVERT(Varchar(500),  MPXPrezzi.Prezzo	), '.',',' )  ")
            'StbSQL.AppendLine("                     FROM Materie_Prime_Campionature MP_Camp_Indice ")
            'StbSQL.AppendLine("                     INNER JOIN ParametriQualitativiXPrezzi MPXPrezzi ")
            'StbSQL.AppendLine("                     ON    MP_Camp_Indice.tipo = MPXPrezzi.tipo ")
            'StbSQL.AppendLine("                     AND MP_Camp_Indice.tipo_cod = MPXPrezzi.tipo_cod ")
            'StbSQL.AppendLine("                     AND MP_Camp_Indice.udm_cod = MPXPrezzi.udm_cod ")
            'StbSQL.AppendLine("                     WHERE MP_Camp_Indice.progressivo = Mov_Dett_Raccolta.cal_cod ")
            'StbSQL.AppendLine("                     AND MPXPrezzi.Piva_SuperUser = '01271980391' ")
            'StbSQL.AppendLine("                     AND MPXPrezzi.Piva = Mov_Dett_Raccolta.Piva  ")
            'StbSQL.AppendLine("                     AND MP_Camp_Indice.tipo = 'indice' ")
            'StbSQL.AppendLine("                     AND MPXPrezzi.validita_inizio <= Mov_Accett.Data_Movimento ")
            'StbSQL.AppendLine("                     AND MPXPrezzi.validita_fine >= Mov_Accett.Data_Movimento ")
            'StbSQL.AppendLine("                     AND CONVERT(FLOAT, replace( MP_Camp_Indice.val_cod ,',', '.') )  >= MPXPrezzi.VALORE_MIN ")
            'StbSQL.AppendLine("                     AND CONVERT(FLOAT,replace( MP_Camp_Indice.val_cod ,',', '.') )  <= MPXPrezzi.VALORE_Max ")
            'StbSQL.AppendLine("                     ), '_' ) AS GradoBrix_IndicePrezzo, ")
            StbSQL.AppendLine("         ISNULL(ICF.Valore5, 3) AS Franchigia_DifMaggiori, ISNULL(ICF.Valore7, 0.5) AS Coefficiente_DifMinori, ISNULL(ICF.Valore9, 0) AS Premio_EuroTon_PomoBio ")
            StbSQL.AppendLine(" , MPC_1a.Dif_Magg_Marcio, MPC_1b.Dif_Magg_Verde , MPC_1c.Dif_Magg_Inerti   ")
            StbSQL.AppendLine(" , MPC_2.Tot_Dif_Min , MPC_3.Frutti_Schiacciati, MPC_4.Grado_Brix ")

            StbSQL.AppendLine(" FROM Agenda ") 'Agenda.PIVA = Contatti_Trasf.Piva
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Trasf ON agenda.piva= Contatti_Trasf.cod_contatto ")
            'StbSQL.AppendLine(" INNER JOIN ImpresexIndirizzi ImpresexIndirizzi_Trasf ON Contatti_Trasf.Cod_Contatto = ImpresexIndirizzi_Trasf.piva ")
            'StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Trasf ON ImpresexIndirizzi_Trasf.cod_indirizzo = Indirizzi_Trasf.cod_indirizzo ")
            'StbSQL.AppendLine(" INNER JOIN ISTAT Istat_trasf ON Indirizzi_Trasf.pro_cod_istat = Istat_trasf.PROV AND Indirizzi_Trasf.com_cod_istat = Istat_trasf.COM  ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   ")
            StbSQL.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Mdt_Extra ON Mdt_Extra.PIVA = Mov_Certificato.PIVA AND Mdt_Extra.Sa_Cod = Mov_Certificato.Sa_Cod  ")
            StbSQL.AppendLine(" AND Mdt_Extra.Id_Agenda = Mov_Certificato.Id_Agenda AND Mdt_Extra.Id_Mov = Mov_Certificato.Id_Mov ")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")
            StbSQL.AppendLine(" LEFT JOIN Risorse_Umane Risorse_Umane_Coop ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT JOIN Contatti Contatti_Coop ON Risorse_Umane_Coop.Piva = Contatti_Coop.Piva AND Risorse_Umane_Coop.Cod_Contatto = Contatti_Coop.Cod_Contatto  ")
            StbSQL.AppendLine(" LEFT JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  ")
            StbSQL.AppendLine(" LEFT JOIN Risorse_Umane Risorse_Umane_Vettore ON Mov_Accett.Cod_Vettore = Risorse_Umane_Vettore.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT JOIN Contatti Contatti_Vettore ON Risorse_Umane_Vettore.Piva = Contatti_Vettore.Piva AND Risorse_Umane_Vettore.Cod_Contatto = Contatti_Vettore.Cod_Contatto  ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")
            StbSQL.AppendLine(" LEFT JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  ")
            StbSQL.AppendLine(" LEFT JOIN SpecieVegetali ON SpecieVegetali.veg_cod = MP_Raccolta.Veg_Cod  ")
            StbSQL.AppendLine(" LEFT JOIN gruppovarietale ON gruppovarietale.grva_cod = MP_Raccolta.grva_Cod_Veg ")
            StbSQL.AppendLine(" LEFT JOIN Mov_Dettaglio_Conferimento Mov_Dett_Conf ON Mov_Dett_Conf.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Dett_Conf.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Dett_Conf.Id_Mov = Mov_Dett_Raccolta.Id_Mov AND Mov_Dett_Conf.Id_Mov_Det = Mov_Dett_Raccolta.Id_Mov_Det ")
            StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det ")
            StbSQL.AppendLine(" INNER JOIN Centri_Aziendali C_Az_Raccolta ON Mov_Dett_Raccolta.PIVA = C_Az_Raccolta.PIVA AND Mov_Dett_Raccolta.Sa_Cod = C_Az_Raccolta.sa_cod ")
            StbSQL.AppendLine(" LEFT JOIN CentrixIndirizzi ImpresexIndirizzi_Trasf ON C_Az_Raccolta.PIVA = ImpresexIndirizzi_Trasf.Piva AND C_Az_Raccolta.Sa_Cod = ImpresexIndirizzi_Trasf.Sa_Cod ")
            StbSQL.AppendLine(" LEFT JOIN Indirizzi Indirizzi_Trasf ON ImpresexIndirizzi_Trasf.cod_indirizzo = Indirizzi_Trasf.cod_indirizzo ")
            StbSQL.AppendLine(" LEFT JOIN ISTAT Istat_trasf ON Indirizzi_Trasf.pro_cod_istat = Istat_trasf.PROV AND Indirizzi_Trasf.com_cod_istat = Istat_trasf.COM  ")
            'StbSQL.AppendLine(" LEFT JOIN Fabbricati Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Fabbricato_Cod ")
            StbSQL.AppendLine(" LEFT JOIN Cantina_Vasche Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Vas_Cod ")
            StbSQL.AppendLine(" LEFT JOIN Imprese_Contratto_Fasi ICF ON ICF.PIVA = Mov_Dett_Raccolta.PIVA AND ICF.Elem_Cod = Mov_Dett_Raccolta.Elem_Cod  ")
            StbSQL.AppendLine(" AND ICF.pro_Cod = Mov_Dett_Raccolta.pro_Cod AND ICF.mat_Cod = Mov_Dett_Raccolta.mat_Cod AND ICF.Progetto_Cod = Mov_Dett_Raccolta.cod_progetto ")
            StbSQL.AppendLine(" AND ICF.fase_Cod = Mov_Dett_Raccolta.fase_Cod AND ICF.lotto = Mov_Dett_Raccolta.Lotto  ")
            StbSQL.AppendLine(" LEFT JOIN Imprese_Contratti IC ON ICF.PIVA = IC.PIVA AND ICF.Contratto_Cod = IC.Contratto_Cod ")
            StbSQL.AppendLine(" LEFT JOIN (SELECT progressivo, CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.')) AS Dif_Magg_Marcio  ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'omarcio'  ")
            StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 0 ")
            StbSQL.AppendLine("             ) AS MPC_1a on MPC_1a.progressivo = Mov_Dett_Raccolta.cal_cod  ")
            StbSQL.AppendLine(" LEFT JOIN (SELECT progressivo, CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.')) AS Dif_Magg_Verde  ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'overde'  ")
            StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 0 ")
            StbSQL.AppendLine("             ) AS MPC_1b on MPC_1b.progressivo = Mov_Dett_Raccolta.cal_cod  ")
            StbSQL.AppendLine(" LEFT JOIN (SELECT progressivo, CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.')) AS Dif_Magg_Inerti  ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'oinerti'  ")
            StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 0 ")
            StbSQL.AppendLine("             ) AS MPC_1c on MPC_1c.progressivo = Mov_Dett_Raccolta.cal_cod  ")
            StbSQL.AppendLine(" LEFT JOIN (SELECT progressivo, SUM(CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.'))) AS Tot_Dif_Min  ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo IN ('ofruttiimmaturi','ofruttilesionati','ofruttischiacciati','ofruttiscottati')  ")
            StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 0 ")
            StbSQL.AppendLine("             GROUP BY progressivo) AS MPC_2 on MPC_2.progressivo = Mov_Dett_Raccolta.cal_cod  ")
            StbSQL.AppendLine(" LEFT JOIN (SELECT progressivo, CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.')) AS Frutti_Schiacciati ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'ofruttischiacciati'  ")
            StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 0 ")
            StbSQL.AppendLine("             ) AS MPC_3 ON MPC_3.progressivo = Mov_Dett_Raccolta.cal_cod  ")
            StbSQL.AppendLine(" LEFT JOIN (SELECT progressivo, CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.')) AS Grado_Brix ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'ogradobrix'  ")
            StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 0 ")
            StbSQL.AppendLine("             ) AS MPC_4 ON MPC_4.progressivo = Mov_Dett_Raccolta.cal_cod  ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod = " & Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) & "")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione = -1 ")
            'StbSQL.AppendLine(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  ") ' 0 = NO SURGELATO
            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov      = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'")
            'Mov_Certificato non esiste nel nuovo conferimento
            'StbSQL.AppendLine(" AND Mov_Certificato.Cau_Mov = '" + Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) + "'")
            StbSQL.AppendLine(" AND Mov_Conf.Cau_Mov        = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) & "'")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov    = '" & Agro_SQL_SaveText(CAU_CARICO) & "'")

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine(" AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " " & vbCrLf)

            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento <=" & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento >=" & Agro_SQL_SaveDate(Data_Inizio) & " ")

            If Cod_RisUm <> 0 Then
                StbSQL.AppendLine(" AND Mov_Accett.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.AppendLine(" ORDER BY Codice_OP, ")
                StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero,Mov_Accett.Doc_Numero_Des,  ")
                StbSQL.AppendLine("         Mov_Accett.Data_Movimento, Mov_Accett.Ora ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


    Public Function CertificatiPomodoro_LeggiContatto(ByVal Piva As String, ByVal Cod_Rapporto As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AccettazioneDaDiversi.CertificatiPomodoro_LeggiContatto"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.AppendLine("SELECT c.Piva, c.Cod_Contatto, c.Rag_Soc, ru.Cod_RisUm, ru.Cod_Rapporto, ii.Cod_Indirizzo,")
            StbSQL.AppendLine("  ISNULL((SELECT VAL_Cod FROM Imprese_Codici WHERE Piva = c.Cod_Contatto AND ID_COD = 1126),'') AS Codice_OP,")
            StbSQL.AppendLine("  ISNULL((SELECT VAL_Cod FROM Imprese_Codici WHERE Piva = c.Cod_Contatto AND ID_COD = 1128),'') AS Codice_AzTrasf")
            'StbSQL.AppendLine("  ISNULL((SELECT VAL_Cod FROM Imprese_Codici WHERE Piva = c.Cod_Contatto AND ID_COD = 1125),'') AS Codice_UnioneOP")
            'StbSQL.AppendLine("  ISNULL((SELECT VAL_Cod FROM Imprese_Codici WHERE Piva = c.Cod_Contatto AND ID_COD = 1127),'') AS Codice_AssInd")
            StbSQL.AppendLine("FROM Contatti c INNER JOIN Risorse_Umane ru ON ru.Piva = c.Piva AND ru.Cod_Contatto = c.Cod_Contatto")
            StbSQL.AppendLine("INNER JOIN ImpresexIndirizzi ii ON c.Cod_Contatto = ii.piva")
            StbSQL.AppendLine("WHERE c.Cod_Contatto = '" & Agro_SQL_SaveText(Piva) & "'")

            If Cod_Rapporto <> 0 Then
                StbSQL.AppendLine("  AND ru.Cod_Rapporto = " & Agro_SQL_SaveNum(Cod_Rapporto))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function CertificatiPomodoro_LeggiVettore(ByVal Vettore As String, ByVal Targa As String, ByVal Tipo_Indirizzo As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AccettazioneDaDiversi.CertificatiPomodoro_LeggiVettore"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.AppendLine("SELECT c.Piva, c.Cod_Contatto, c.Rag_Soc, c.Nome, c.Cognome, ru.Cod_RisUm, ru.Cod_Rapporto, ci.Cod_Indirizzo, ci.Tipo_Indirizzo, pm.Mac_Cod, pm.Targa")
            StbSQL.AppendLine("FROM Contatti c INNER JOIN Risorse_Umane ru ON ru.Piva = c.Piva AND ru.Cod_Contatto = c.Cod_Contatto")
            StbSQL.AppendLine("INNER JOIN ContattixIndirizzi ci ON c.Cod_Contatto = ci.Cod_Contatto")
            StbSQL.AppendLine("INNER JOIN Parco_Macchine pm ON c.Cod_Contatto = pm.Cod_Contatto")
            StbSQL.AppendLine("WHERE ru.Cod_Rapporto IN (-5,-10,-16)")

            If Not String.IsNullOrEmpty(Vettore) Then
                StbSQL.AppendLine("  AND c.Rag_Soc = '" & Agro_SQL_SaveText(Vettore) & "'")
            End If

            If Not String.IsNullOrEmpty(Targa) Then
                StbSQL.AppendLine("  AND pm.Targa = '" & Agro_SQL_SaveText(Targa) & "'")
            End If

            If Tipo_Indirizzo <> 0 Then
                StbSQL.AppendLine("  AND ci.Tipo_Indirizzo = " & Agro_SQL_SaveNum(Tipo_Indirizzo))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function CertificatiPomodoro_LeggiContratto(ByVal Cod_RisUm As Integer, ByVal Elem_Cod As Integer, ByVal Mat_Cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AccettazioneDaDiversi.CertificatiPomodoro_LeggiContratto"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.AppendLine("SELECT * ")
            StbSQL.AppendLine("FROM Imprese_Contratto_Fasi ICF")
            StbSQL.AppendLine("INNER JOIN Imprese_Contratti IC ON ICF.PIVA = IC.PIVA AND ICF.Contratto_Cod = IC.Contratto_Cod")
            StbSQL.AppendLine("WHERE IC.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

            If Cod_RisUm <> 0 Then
                StbSQL.AppendLine("AND IC.Cod_Risum = " & Agro_SQL_SaveNum(Cod_RisUm))
            End If

            If Elem_Cod <> 0 Then
                StbSQL.AppendLine("AND ICF.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod))
            End If

            If Mat_Cod <> 0 Then
                StbSQL.AppendLine("AND ICF.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function CertificatiPomodoro_ScriviContatto(ByVal PivaPadre As String, ByVal Piva As String, ByVal RagSoc As String, ByVal CodFis As String,
                                                       ByRef CodRisUm As Integer, ByRef CodIndirizzo As Integer,
                                                       ByRef objParametri As AgronicaCoreParametri,
                                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                       Optional ByVal TipoImpresa As Integer = enum_TipoImpresaGerarchia.Impresa,
                                                       Optional ByVal CodRapporto As Integer = enum_Rapporti_Contabili_Standard.Conferente) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AccettazioneDaDiversi.CertificatiPomodoro_ScriviContatto"
        Dim MessaggioErrore As String = ""

        Dim username = objParametri.UsernameOperazione

        'Dim transactionOptions As New TransactionOptions()
        'transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        'transactionOptions.Timeout = TransactionManager.MaximumTimeout

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            'Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Dim impresa = (From i In GiasContext.Imprese Where i.PIVA = Piva).FirstOrDefault()

                If IsNothing(impresa) Then
                    impresa = EFImprese.CreateImpreseEF(GiasContext, objParametri, Piva, RagSoc, username)
                    EFImprese.CreateGerarchiaImprese(GiasContext, objParametri, PivaPadre, Piva, 1, 4, username, objParametri_Utenti)
                    EFImprese.CreateUtentixImprese(GiasContext, objParametri, objParametri.PivaSuperUser, Piva, username)
                    EFImprese.CreateImprese_CodiciEF(GiasContext, objParametri, impresa, enum_CodiciAnagrafe.CodiceCUAA, CodFis, username)

                End If

                Dim indirizzo = EFImprese.CreateIndirizzoImpresa(GiasContext, objParametri, impresa, enum_TipiIndirizzi.TipoIndirizzoDefaultAnagrafica, username)
                indirizzo.com_cod_istat = "000"
                indirizzo.pro_cod_istat = "000"

                Dim contatto = EFContatti.CreateContattiEF(GiasContext, objParametri, objParametri.PivaSuperUser, -1, Piva, 1, username)
                contatto.Rag_Soc = RagSoc
                contatto.Codice_Fiscale = CodFis

                Dim risorsa = EFContatti.CreateRisorse_Umane(GiasContext, objParametri, contatto, username)
                risorsa.Cod_Rapporto = CodRapporto

                CodRisUm = risorsa.Cod_RisUm
                CodIndirizzo = indirizzo.cod_indirizzo

                GiasContext.SaveChanges()

            End Using

            'scope.Complete()
            'End Using

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return MessaggioErrore

    End Function

    Public Function CertificatiPomodoro_ScriviConferente(ByVal Piva As String, ByRef Cod_RisUm As Integer, ByRef objParametri As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AccettazioneDaDiversi.CertificatiPomodoro_ScriviConferente"
        Dim username = objParametri.UsernameOperazione
        Dim MessaggioErrore As String = ""

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Try

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Dim idGen As New AgronicaCoreDataProvider.Agro_Sequenze
                Cod_RisUm = idGen.NuovoId_Tabella_EF(GiasContext, "Risorse_Umane", 0, 2000000, objParametri)

                Dim risUm As New Risorse_Umane

                risUm.Piva = objParametri.PivaSuperUser
                risUm.Sa_Cod = -1
                risUm.Cod_RisUm = Cod_RisUm
                risUm.Cod_Contatto = Piva
                risUm.Cod_Rapporto = enum_Rapporti_Contabili_Standard.Conferente
                risUm.Validita_Inizio = AGRODATAINIZIO
                risUm.Validita_Fine = AGRODATAFINE
                risUm.Settore_Des = ""
                risUm.Attivita_Des = ""
                risUm.Corrispettivo_Mensile = 0
                risUm.Corrispettivo_Orario = 0
                risUm.Occasionale = 0
                risUm.Ore_Settimanali = 0
                risUm.Giorni_Ferie = 0
                risUm.Ferie_Godute = 0
                risUm.Giorni_Malattia = 0
                risUm.Inviato = 0
                risUm.Data_Creazione = DateTime.Now
                risUm.Data_Modifica = DateTime.Now
                risUm.Username_Creazione = username
                risUm.Username_Modifica = username
                risUm.Patentino = ""
                risUm.Data_Rilascio_Patentino = AGRODATAINIZIO
                risUm.Data_Scadenza_Patentino = AGRODATAFINE
                risUm.Cod_RisUm_Origine = 0
                risUm.Piva_SuperUser_Origine = ""
                risUm.Ente_di_rilascio = ""
                risUm.Saldo_Iniziale_Crediti = 0
                risUm.Saldo_Iniziale_Debiti = 0
                risUm.ChkSpesometro = 1
                risUm.ChkBlocco = 0
                risUm.Blocco_Des = ""
                risUm.Qualifica_Cod = 0
                risUm.Mansione_Cod = 0
                risUm.Info_Famiglia = ""
                risUm.Classificazione_Cod = 0
                risUm.Ra_Cod = ""

                GiasContext.Risorse_Umane.Add(risUm)
                GiasContext.SaveChanges()

            End Using

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return MessaggioErrore


    End Function


End Class
