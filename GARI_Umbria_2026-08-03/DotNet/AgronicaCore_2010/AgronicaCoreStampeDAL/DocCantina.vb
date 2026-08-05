Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class DocCantina
    Inherits AgronicaCoreDataProvider.DataProvider


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' stampa del report della bolla F&F
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Stampa_DDt_Conferimento_Uva(ByVal Piva As String,
                                                ByVal Lav_Cod As Integer,
                                                ByVal Id_Agenda As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocCantina.Stampa_DDt_Conferimento_Uva"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim Flag_Accettazione As Boolean
        Dim Flag_Carico As Boolean

        Try

            '    Select Case Lav_Cod
            '        Case LAVCOD_ACCETTAZIONE_DIVERSI,
            '             LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
            '             LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
            '            Flag_Accettazione = True
            '        Case Else
            '            Flag_Accettazione = False
            '    End Select

            '    'nel caso del carico NON ci sono tutta una serie di informazioni
            '    '(bolla di conferimento collegata, conferente, ecc...)
            '    Select Case Lav_Cod
            '        Case LAVCOD_DISTINTA_CARICO,
            '             LAVCOD_DISTINTA_CARICO_ACCETTAZIONE
            '            Flag_Carico = True
            '        Case Else
            '            Flag_Carico = False
            '    End Select

            '    StbSQL.Length = 0

            '    StbSQL.Append(" SELECT  Agenda.piva ,Agenda.des_lib ,Agenda.id_agenda, Agenda.lav_cod, " + vbCrLf)
            '    StbSQL.Append("         -- Mov_Accett.ChkLayout_Join_Prodotti, Mov_Accett.Peso, Mov_Accett.Extra_Str, Mov_Accett.Natura_Beni, " + vbCrLf)
            '    StbSQL.Append("         -- Mov_Accett.Tara_Veicolo, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso,  " + vbCrLf)
            '    StbSQL.Append("         Mov_Accett.Data_Movimento AS Data_Accett, Mov_Accett.Ora AS Ora_Accett, " + vbCrLf)
            '    StbSQL.Append("         Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero,Mov_Accett.Doc_Numero_Des, Mov_Accett.Mov_Desc AS Note, " + vbCrLf)

            '    'StbSQL.Append("         -- Sequenza_Progressivi.Lunghezza_Sin, Sequenza_Progressivi.Lunghezza_Centro, Sequenza_Progressivi.Lunghezza_Des, Sequenza_Progressivi.CarattereFormattazione, " + vbCrLf)

            '    StbSQL.Append("         Mov_Accett.Cod_RisUm," + vbCrLf)
            '    'If Not Flag_Carico Then
            '    StbSQL.Append("         Risorse_Umane_Conferenti.Cod_Rapporto AS Cod_Rapporto_Conferente, Contatti_Conferenti.Cod_Contatto AS Cod_Contatto_Conferente,  " + vbCrLf)
            '    StbSQL.Append("         Contatti_Conferenti.Rag_Soc AS Rag_Soc_Conferente, ISNULL(Contatti_Conferenti.Codice_Fiscale, '') AS Codice_Fiscale_Conferente, " + vbCrLf)
            '    StbSQL.Append("         Contatti_Conferenti.Nome AS Nome_Conferente, Contatti_Conferenti.Cognome AS Cognome_Conferente,  " + vbCrLf)
            '    StbSQL.Append("         Mov_Accett.Cod_IndirizzoRisUm,   Indirizzi_Conferenti.ind_des AS ind_des_Conferente, Indirizzi_Conferenti.frz_des AS frz_des_Conferente, Indirizzi_Conferenti.CAP AS cap_Conferente,  " + vbCrLf)
            '    StbSQL.Append("         Indirizzi_Conferenti.stato AS stato_Conferente, Indirizzi_Conferenti.pro_cod_istat AS pro_cod_istat_Conferente,  " + vbCrLf)
            '    StbSQL.Append("         Indirizzi_Conferenti.com_cod_istat AS com_cod_istat_Conferente, Istat_Conferenti.LOCALITA AS localita_Conferente, Istat_Conferenti.COMUNI_PROV AS comuni_prov_Conferente, " + vbCrLf)
            '    'Else
            '    '    StbSQL.Append("         0 AS Cod_Rapporto_Conferente, '' AS Cod_Contatto_Conferente,  " + vbCrLf)
            '    '    StbSQL.Append("         '' AS Rag_Soc_Conferente, '' AS Codice_Fiscale_Conferente, " + vbCrLf)
            '    '    StbSQL.Append("         '' AS Nome_Conferente, '' AS Cognome_Conferente,  " + vbCrLf)
            '    '    StbSQL.Append("         0 AS Cod_IndirizzoRisUm,   '' AS ind_des_Conferente, '' AS frz_des_Conferente, '' AS cap_Conferente,  " + vbCrLf)
            '    '    StbSQL.Append("         '' AS stato_Conferente, '' AS pro_cod_istat_Conferente,  " + vbCrLf)
            '    '    StbSQL.Append("         '' AS com_cod_istat_Conferente, '' AS localita_Conferente, '' AS comuni_prov_Conferente, " + vbCrLf)
            '    'End If

            '    StbSQL.Append("         Mov_Accett.Cod_RisUm_Altro,  " + vbCrLf)
            '    'StbSQL.Append("         Risorse_Umane_Coop.Cod_Rapporto AS Cod_Rapporto_Coop, Contatti_Coop.Cod_Contatto AS Cod_Contatto_Coop,  " + vbCrLf)
            '    'StbSQL.Append("         Contatti_Coop.Rag_Soc AS Rag_Soc_Coop, ISNULL(Contatti_Coop.Codice_Fiscale, '') AS Codice_Fiscale_Coop,  " + vbCrLf)
            '    'StbSQL.Append("         Contatti_Coop.Nome AS Nome_Coop, Contatti_Coop.Cognome AS Cognome_Coop,  " + vbCrLf)
            '    'StbSQL.Append("         ImpreseXIndirizzi_Coop.Cod_Indirizzo AS Cod_Indirizzo_Coop, Indirizzi_Coop.ind_des AS ind_des_Coop, Indirizzi_Coop.frz_des AS frz_des_Coop, Indirizzi_Coop.CAP AS cap_Coop, " + vbCrLf)
            '    'StbSQL.Append("         Indirizzi_Coop.stato AS stato_Coop, Indirizzi_Coop.pro_cod_istat AS pro_cod_istat_Coop,  " + vbCrLf)
            '    'StbSQL.Append("         Indirizzi_Coop.com_cod_istat AS com_cod_istat_Coop, Istat_Coop.LOCALITA AS localita_Coop, Istat_Coop.COMUNI_PROV AS comuni_prov_Coop, " + vbCrLf)
            '    StbSQL.Append("         Mov_Accett.Extra_Int,  " + vbCrLf)
            '    'StbSQL.Append("         Risorse_Umane_Coop_2.Cod_Rapporto AS Cod_Rapporto_Coop_2, Contatti_Coop_2.Cod_Contatto AS Cod_Contatto_Coop_2,  " + vbCrLf)
            '    'StbSQL.Append("         Contatti_Coop_2.Rag_Soc AS Rag_Soc_Coop_2, ISNULL(Contatti_Coop_2.Codice_Fiscale, '') AS Codice_Fiscale_Coop_2,  " + vbCrLf)
            '    'StbSQL.Append("         Contatti_Coop_2.Nome AS Nome_Coop_2, Contatti_Coop_2.Cognome AS Cognome_Coop_2,  " + vbCrLf)
            '    'StbSQL.Append("         ImpreseXIndirizzi_Coop_2.Cod_Indirizzo AS Cod_Indirizzo_Coop_2, Indirizzi_Coop_2.ind_des AS ind_des_Coop_2, Indirizzi_Coop_2.frz_des AS frz_des_Coop_2, Indirizzi_Coop_2.CAP AS cap_Coop_2, " + vbCrLf)
            '    'StbSQL.Append("         Indirizzi_Coop_2.stato AS stato_Coop_2, Indirizzi_Coop_2.pro_cod_istat AS pro_cod_istat_Coop_2,  " + vbCrLf)
            '    'StbSQL.Append("         Indirizzi_Coop_2.com_cod_istat AS com_cod_istat_Coop_2, Istat_Coop_2.LOCALITA AS localita_Coop_2, Istat_Coop_2.COMUNI_PROV AS comuni_prov_Coop_2, " + vbCrLf)
            '    StbSQL.Append("         Mov_Accett.Cod_Destinazione, " + vbCrLf)
            '    'StbSQL.Append("         Risorse_Umane_Produttori.Cod_Rapporto AS Cod_Rapporto_Produttore, Contatti_Produttori.Cod_Contatto AS Cod_Contatto_Produttore,  " + vbCrLf)
            '    'StbSQL.Append("         Contatti_Produttori.Rag_Soc AS Rag_Soc_Produttore, ISNULL(Contatti_Produttori.Codice_Fiscale, '') AS Codice_Fiscale_Produttore,  " + vbCrLf)
            '    'StbSQL.Append("         Contatti_Produttori.Nome AS Nome_Produttore, Contatti_Produttori.Cognome AS Cognome_Produttore,  " + vbCrLf)
            '    'StbSQL.Append("         Mov_Accett.Cod_IndirizzoDestinazione, Indirizzi_Produttori.ind_des AS ind_des_Produttore, Indirizzi_Produttori.frz_des AS frz_des_Produttore, Indirizzi_Produttori.CAP AS cap_Produttore,  " + vbCrLf)
            '    'StbSQL.Append("         Indirizzi_Produttori.stato AS stato_Produttore, Indirizzi_Produttori.pro_cod_istat AS pro_cod_istat_Produttore,  " + vbCrLf)
            '    'StbSQL.Append("         Indirizzi_Produttori.com_cod_istat AS com_cod_istat_Produttore, Istat_Produttori.LOCALITA AS localita_Produttore, Istat_Produttori.COMUNI_PROV AS comuni_prov_Produttore, " + vbCrLf)
            '    StbSQL.Append("" + vbCrLf)
            '    StbSQL.Append("         Mov_Accett.Mezzo, Mov_Accett.Cod_Vettore, Mov_Accett.Cod_IndirizzoVettore,  " + vbCrLf)
            '    If Not Flag_Carico Then
            '        StbSQL.Append("         Risorse_Umane_Vettore.Cod_Rapporto AS Cod_Rapporto_Produttore, Contatti_Vettore.Cod_Contatto AS Cod_Contatto_Vettore,  " + vbCrLf)
            '        StbSQL.Append("         Contatti_Vettore.Rag_Soc AS Rag_Soc_Vettore, ISNULL(Contatti_Vettore.Codice_Fiscale, '') AS Codice_Fiscale_Vettore,  " + vbCrLf)
            '        StbSQL.Append("         Contatti_Vettore.Nome AS Nome_Vettore, Contatti_Vettore.Cognome AS Cognome_Vettore,  " + vbCrLf)
            '        StbSQL.Append("         Mov_Accett.Cod_IndirizzoVettore, Indirizzi_Vettore.ind_des AS ind_des_Vettore, Indirizzi_Vettore.frz_des AS frz_des_Vettore, Indirizzi_Vettore.CAP AS cap_Vettore,  " + vbCrLf)
            '        StbSQL.Append("         Indirizzi_Vettore.stato AS stato_Vettore, Indirizzi_Vettore.pro_cod_istat AS pro_cod_istat_Vettore,  " + vbCrLf)
            '        StbSQL.Append("         Indirizzi_Vettore.com_cod_istat AS com_cod_istat_Vettore, Istat_Vettore.LOCALITA AS localita_Vettore, Istat_Vettore.COMUNI_PROV AS comuni_prov_Vettore, " + vbCrLf)
            '    Else
            '        StbSQL.Append("         0 AS Cod_Rapporto_Produttore, '' AS Cod_Contatto_Vettore,  " + vbCrLf)
            '        StbSQL.Append("         '' AS Rag_Soc_Vettore, '' AS Codice_Fiscale_Vettore,  " + vbCrLf)
            '        StbSQL.Append("         '' AS Nome_Vettore, '' AS Cognome_Vettore,  " + vbCrLf)
            '        StbSQL.Append("         0 AS Cod_IndirizzoVettore, '' AS ind_des_Vettore, '' AS frz_des_Vettore, '' AS cap_Vettore,  " + vbCrLf)
            '        StbSQL.Append("         '' AS stato_Vettore, '' AS pro_cod_istat_Vettore,  " + vbCrLf)
            '        StbSQL.Append("         '' AS com_cod_istat_Vettore, '' AS localita_Vettore, '' AS comuni_prov_Vettore, " + vbCrLf)
            '    End If

            '    StbSQL.Append("" + vbCrLf)
            '    If Not Flag_Carico Then
            '        StbSQL.Append("         Mov_Conf.Id_Mov AS Id_Mov_Conf, Mov_Conf.Cau_Mov AS Cau_Mov_Conf, Mov_Conf.Mov_Desc AS Mov_Desc_Conf,  " + vbCrLf)
            '        StbSQL.Append("         Mov_Conf.Data_Movimento AS Data_Conf, Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf,  " + vbCrLf)
            '        StbSQL.Append("         Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf,  Mov_Conf.Colli, " + vbCrLf)
            '    Else
            '        StbSQL.Append("         0 AS Id_Mov_Conf, '' AS Cau_Mov_Conf, '' AS Mov_Desc_Conf,  " + vbCrLf)
            '        StbSQL.Append("         '01/01/1900' AS Data_Conf, '' AS Doc_Numero_Sin_Conf, 0 AS Doc_Numero_Conf,  " + vbCrLf)
            '        StbSQL.Append("         '' AS Doc_Numero_Des_Conf,  0 AS Colli, " + vbCrLf)
            '    End If

            '    StbSQL.Append("" + vbCrLf)
            '    StbSQL.Append("         Mov_Raccolta.Id_Mov AS Id_Mov_Raccolta,  " + vbCrLf)
            '    StbSQL.Append("         Mov_Raccolta.Cau_Mov AS Cau_Mov_Raccolta, Mov_Raccolta.Mov_Desc AS Mov_Desc_Raccolta, Mov_Raccolta.Data_Movimento AS Data_Raccolta,  " + vbCrLf)
            '    StbSQL.Append("         Mov_Raccolta.ora AS DataOra_Ingresso,  " + vbCrLf)
            '    'StbSQL.Append("         -- C_Az_Raccolta.sa_cod AS Sa_Cod_Raccolta, C_Az_Raccolta.sa_nome AS Sa_Nome_Raccolta,   " + vbCrLf)
            '    'StbSQL.Append("         -- Mov_Dest_Raccolta.Id_Destinazione AS Id_Destinazione_Raccolta, Cantina_Vasche.Identificativo AS Id_vasca, " + vbCrLf)
            '    StbSQL.Append("         Mov_Dett_Raccolta.Id_Mov_Det AS Id_Mov_Det_Raccolta, Mov_Dett_Raccolta.sa_cod AS Sa_cod_Raccolta,  " + vbCrLf)
            '    StbSQL.Append("         Mov_Dett_Raccolta.Mov_Det_Des AS Mov_Det_Des_Raccolta, Mov_Dett_Raccolta.Elem_Cod AS Elem_Cod_Raccolta,  " + vbCrLf)
            '    StbSQL.Append("         Mov_Dett_Raccolta.Pro_Cod AS Pro_Cod_Raccolta, Mov_Dett_Raccolta.Mat_Cod AS Mat_Cod_Raccolta,  " + vbCrLf)
            '    StbSQL.Append("         Mov_Dett_Raccolta.Cod_Progetto AS Cod_Progetto_Raccolta, Mov_Dett_Raccolta.Fase_Cod AS Fase_Cod_Raccolta,  " + vbCrLf)
            '    StbSQL.Append("         Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta,  " + vbCrLf)
            '    StbSQL.Append("         Mov_Dett_Raccolta.Qta, Mov_Dett_Raccolta.Tara AS Tara,    " + vbCrLf)
            '    StbSQL.Append("         Mov_Dett_Raccolta.Qta_Extra, Mov_Dett_Raccolta.Udm_Cod_Extra, Mov_Dett_Raccolta.Qta_Extra_totale, Mov_Dett_Raccolta.Tara, " + vbCrLf)
            '    StbSQL.Append("         Mov_Dett_Raccolta.Qta_Dettaglio1 AS Qta_sottoconfezioni, Mov_Dett_Raccolta.Qta_Dettaglio2 AS Qta_Imballaggi, " + vbCrLf)

            '    StbSQL.Append("         UDM_Qta.Udm_Sim AS Udm_Sim_Raccolta, UDM_Qta.Udm_des AS Udm_des_Raccolta, " + vbCrLf)
            '    StbSQL.Append("         Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta, Mov_Dett_Raccolta.Listino_Cod AS Listino_Cod_Raccolta, Mov_Dett_Raccolta.Prezzo_Unitario,  " + vbCrLf)
            '    StbSQL.Append("         Mov_Dett_Raccolta.Prezzo_Unitario_Netto, Mov_Dett_Raccolta.Imponibile, Mov_Dett_Raccolta.Imponibile_Netto,  " + vbCrLf)
            '    StbSQL.Append("         Mov_Dett_Raccolta.Jolly_Int AS Jolly_Int_Raccolta, Mov_Dett_Raccolta.Contabilizzato AS Contabilizzato_Raccolta,  " + vbCrLf)
            '    StbSQL.Append("         -- Mov_Dett_Raccolta.Pendente AS Pendente_Raccolta, Mov_Dett_Raccolta.Prezzo_Effettivo, Mov_Dett_Raccolta.ChkLayOut_Hide, " + vbCrLf)
            '    StbSQL.Append("         MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta,  " + vbCrLf)
            '    StbSQL.Append("         MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, MP_Raccolta.Cul_Cod, MP_Raccolta.Veg_Cod, MP_Raccolta.GRVA_COD_VEG,  " + vbCrLf)
            '    StbSQL.Append("         MP_Raccolta.Regolamento, MP_Raccolta.ChkListino " + vbCrLf)

            '    StbSQL.Append("" + vbCrLf)
            '    StbSQL.Append(" FROM Agenda " + vbCrLf)
            '    StbSQL.Append(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda " + vbCrLf)
            '    StbSQL.Append(" " + vbCrLf)
            '    'StbSQL.Append(" -- INNER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Accett.PIVA  " + vbCrLf)
            '    'StbSQL.Append(" -- AND Sequenza_Progressivi.Anno = YEAR(Mov_Accett.Data_Movimento)  " + vbCrLf)
            '    'StbSQL.Append(" -- AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Accett.Doc_Numero_Sin  " + vbCrLf)
            '    'StbSQL.Append(" -- AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Accett.Doc_Numero_Des  " + vbCrLf)

            '    StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  " + vbCrLf)
            '    StbSQL.Append(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  " + vbCrLf)
            '    StbSQL.Append(" INNER JOIN Indirizzi Indirizzi_Conferenti ON  Mov_Accett.Cod_IndirizzoRisUm = Indirizzi_Conferenti.cod_indirizzo " + vbCrLf)
            '    StbSQL.Append(" INNER JOIN ISTAT Istat_Conferenti ON Indirizzi_Conferenti.pro_cod_istat = Istat_Conferenti.PROV AND Indirizzi_Conferenti.com_cod_istat = Istat_Conferenti.COM " + vbCrLf)

            '    If Not Flag_Carico Then

            '        StbSQL.Append("" + vbCrLf)
            '        StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Vettore ON Mov_Accett.Cod_Vettore = Risorse_Umane_Vettore.Cod_RisUm  " + vbCrLf)
            '        StbSQL.Append(" LEFT OUTER JOIN Contatti Contatti_Vettore ON Risorse_Umane_Vettore.Piva = Contatti_Vettore.Piva AND Risorse_Umane_Vettore.Cod_Contatto = Contatti_Vettore.Cod_Contatto  " + vbCrLf)
            '        StbSQL.Append(" LEFT OUTER JOIN Indirizzi Indirizzi_Vettore ON  Mov_Accett.Cod_IndirizzoVettore = Indirizzi_Vettore.cod_indirizzo " + vbCrLf)
            '        StbSQL.Append(" LEFT OUTER JOIN ISTAT Istat_Vettore ON Indirizzi_Vettore.pro_cod_istat = Istat_Vettore.PROV AND Indirizzi_Vettore.com_cod_istat = Istat_Vettore.COM " + vbCrLf)

            '        StbSQL.Append(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda " + vbCrLf)

            '        '  StbSQL.Append("" + vbCrLf)
            '        'StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop.Cod_RisUm  " + vbCrLf)
            '        'StbSQL.Append(" LEFT OUTER JOIN Contatti Contatti_Coop ON Risorse_Umane_Coop.Piva = Contatti_Coop.Piva AND Risorse_Umane_Coop.Cod_Contatto = Contatti_Coop.Cod_Contatto  " + vbCrLf)
            '        'StbSQL.Append(" LEFT OUTER JOIN ImpreseXIndirizzi ImpreseXIndirizzi_Coop ON  Contatti_Coop.Cod_Contatto = ImpreseXIndirizzi_Coop.Piva " + vbCrLf)
            '        'StbSQL.Append(" LEFT OUTER JOIN Indirizzi Indirizzi_Coop ON  ImpreseXIndirizzi_Coop.Cod_Indirizzo = Indirizzi_Coop.cod_indirizzo " + vbCrLf)
            '        'StbSQL.Append(" LEFT OUTER JOIN ISTAT Istat_Coop ON Indirizzi_Coop.pro_cod_istat = Istat_Coop.PROV AND Indirizzi_Coop.com_cod_istat = Istat_Coop.COM " + vbCrLf)
            '        'StbSQL.Append(" " + vbCrLf)
            '        'StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop_2 ON Mov_Accett.Extra_Int = Risorse_Umane_Coop_2.Cod_RisUm  " + vbCrLf)
            '        'StbSQL.Append(" LEFT OUTER JOIN Contatti Contatti_Coop_2 ON Risorse_Umane_Coop_2.Piva = Contatti_Coop_2.Piva AND Risorse_Umane_Coop_2.Cod_Contatto = Contatti_Coop_2.Cod_Contatto  " + vbCrLf)
            '        'StbSQL.Append(" LEFT OUTER JOIN ImpreseXIndirizzi ImpreseXIndirizzi_Coop_2 ON  Contatti_Coop_2.Cod_Contatto = ImpreseXIndirizzi_Coop_2.Piva " + vbCrLf)
            '        'StbSQL.Append(" LEFT OUTER JOIN Indirizzi Indirizzi_Coop_2 ON  ImpreseXIndirizzi_Coop_2.Cod_Indirizzo = Indirizzi_Coop_2.cod_indirizzo " + vbCrLf)
            '        'StbSQL.Append(" LEFT OUTER JOIN ISTAT Istat_Coop_2 ON Indirizzi_Coop_2.pro_cod_istat = Istat_Coop_2.PROV AND Indirizzi_Coop_2.com_cod_istat = Istat_Coop_2.COM " + vbCrLf)
            '        'StbSQL.Append(" " + vbCrLf)
            '        'StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  " + vbCrLf)
            '        'StbSQL.Append(" LEFT OUTER JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  " + vbCrLf)
            '        'StbSQL.Append(" LEFT OUTER JOIN Indirizzi Indirizzi_Produttori ON  Mov_Accett.Cod_IndirizzoDestinazione = Indirizzi_Produttori.cod_indirizzo " + vbCrLf)
            '        'StbSQL.Append(" LEFT OUTER JOIN ISTAT Istat_Produttori ON Indirizzi_Produttori.pro_cod_istat = Istat_Produttori.PROV AND Indirizzi_Produttori.com_cod_istat = Istat_Produttori.COM " + vbCrLf)
            '        'StbSQL.Append(" " + vbCrLf)
            '    End If
            '    StbSQL.Append(" " + vbCrLf)

            '    StbSQL.Append(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda " + vbCrLf)

            '    StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov " + vbCrLf)
            '    StbSQL.Append(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " + vbCrLf)

            '    'leggo il movimento dettaglio di riepilogo, non c'è destinazione
            '    'StbSQL.Append(" -- INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det " + vbCrLf)
            '    'StbSQL.Append(" -- INNER JOIN Centri_Aziendali C_Az_Raccolta ON Mov_Dett_Raccolta.PIVA = C_Az_Raccolta.PIVA AND Mov_Dett_Raccolta.Sa_Cod = C_Az_Raccolta.sa_cod " + vbCrLf)
            '    'StbSQL.Append(" -- INNER JOIN Cantina_Vasche ON Mov_Dest_Raccolta.Piva = Cantina_Vasche.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Cantina_Vasche.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Cantina_Vasche.vas_Cod   " + vbCrLf)
            '    StbSQL.Append(" " + vbCrLf)
            '    StbSQL.Append(" INNER JOIN UnitaMisura UDM_Qta ON Mov_Dett_Raccolta.Udm_Cod = UDM_Qta.Udm_Cod " + vbCrLf)
            '    StbSQL.Append(" " + vbCrLf)

            '    StbSQL.Append(" WHERE Agenda.Lav_Cod = " + Agro_SQL_SaveNum(CStr(Lav_Cod)) + "" + vbCrLf)
            '    StbSQL.Append(" AND Agenda.Tipo_Accettazione = " + Agro_SQL_SaveNum(CStr(enum_Accettazione_Tipo.POSTcampionatura)) + "" + vbCrLf)
            '    StbSQL.Append(" AND Agenda.Modulo = " + Agro_SQL_SaveNum(CStr(enum_Omni_Modulo_Generazione.FreshFood)) + "" + vbCrLf)

            '    StbSQL.Append(" AND Mov_Accett.Cau_Mov      = '" + Agro_SQL_SaveText(CAU_REGISTRAZIONI) + "'" + vbCrLf)
            '    If Not Flag_Carico Then
            '        StbSQL.Append(" AND Mov_Conf.Cau_Mov        = '" + Agro_SQL_SaveText(CAU_REGISTRAZIONE_SECONDARIA) + "'" + vbCrLf)
            '    End If
            '    'StbSQL.Append(" AND Mov_Raccolta.Cau_Mov    = '" + Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) + "'" + vbCrLf)
            '    StbSQL.Append(" AND Mov_Raccolta.Cau_Mov    = '" + Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) + "'" + vbCrLf)

            '    StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " + vbCrLf)
            '    StbSQL.Append(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "  " + vbCrLf)

            '    'StbSQL.Append(" --AND Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(CStr(objParametri.PivaSuperUser)) & "'  " + vbCrLf)
            '    'StbSQL.Append(" --AND Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(CStr(enum_SequenzaProgressiviTipi.BolleAccettDaDiversi)) & "  " + vbCrLf)

            '    '--------------------------------------------------------------------------
            '    If xFiltroAggiuntivo <> "" Then
            '        StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            '    End If
            '    '--------------------------------------------------------------------------

            '    If xOrderBy <> "" Then
            '        StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            '    Else
            '        StbSQL.Append(" ")
            '    End If

            '    '--------------------------------------------------------------------------
            '    dt = EseguiQuery_Lettura(objParametri, StbSQL.ToString, nomeRoutine)
            '    '--------------------------------------------------------------------------

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
    ''' legge le consistenze enologiche alla data passata
    ''' fa un sum della qta dei movimenti fino alla data specificata
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function ConsistenzeEnologiche(ByVal Piva As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal Piano_Cod As Integer,
                                          ByVal Vas_Cod As Integer,
                                          ByVal Linea_Cod As Integer,
                                          ByVal Categoria_Cod As Integer,
                                          ByVal Semilavorato_Cod As Integer,
                                          ByVal Data_Consistenza As Date,
                                          ByVal Flag_QtaNoZero As Boolean,
                                          ByVal QS_Flag_VascheNoMov As Boolean,
                                          ByVal Ordinamento As enum_OrdinamentoStampaConsEnologiche,
                                          ByVal xFiltroAggiuntivo1 As String,
                                          ByVal xFiltroAggiuntivo2 As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocCantina.ConsistenzeEnologiche"

        Dim messaggioErrore As String = ""
        Dim stbQ As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim xOrderBy As String = ""

        Try

            Select Case Ordinamento

                Case enum_OrdinamentoStampaConsEnologiche.IdentificativoVasca
                    'str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, Identificativo  "
                    xOrderBy = " Piano_Cod, Identificativo  "
                Case enum_OrdinamentoStampaConsEnologiche.LineaProduttiva
                    'str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, LineaProduttiva "
                    xOrderBy = " Piano_Cod, LineaProduttiva "
                Case enum_OrdinamentoStampaConsEnologiche.Lotto
                    ' str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, Lotto "
                    xOrderBy = " Piano_Cod, Lotto "
                Case enum_OrdinamentoStampaConsEnologiche.Materiale
                    ' str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, Materiale  "
                    xOrderBy = " Piano_Cod, Materiale  "
                Case enum_OrdinamentoStampaConsEnologiche.NumeroSerie
                    ' str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, NumeroSerie "
                    xOrderBy = " Piano_Cod, Numero_Serie "
                Case enum_OrdinamentoStampaConsEnologiche.Prodotto
                    ' str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, Mat_des "
                    xOrderBy = " Piano_Cod, Mat_des "
            End Select

            stbQ.Length = 0

            stbQ.Append(" SELECT * " & vbCrLf)
            stbQ.Append(" FROM " & vbCrLf)
            stbQ.Append(" ( " & vbCrLf)

            '---------------------------------------------------------------------
            ' la prima query della union  seleziona le vasche che hanno il vino
            '---------------------------------------------------------------------
            stbQ.Append(" ( " & vbCrLf)
            stbQ.Append(" SELECT  Rag_Soc, Sa_Nome, Cantina_Caratteristiche.Piano_Cod, Cantina_Vasche.Vas_Cod, Cantina_Caratteristiche.Piano_Des, " & vbCrLf)
            stbQ.Append(" Cantina_Vasche.Identificativo, Cantina_Vasche.Numero_Serie, " & vbCrLf)

            stbQ.Append(" CASE Materiale_Cod " & vbCrLf)
            'stbQ.Append("       WHEN 0 THEN 'Altro' " + vbCrLf)
            'stbQ.Append("       WHEN 1 THEN 'Acciaio Inox' " + vbCrLf)
            'stbQ.Append("       WHEN 2 THEN 'Acciaio Smaltato' " + vbCrLf)
            'stbQ.Append("       WHEN 3 THEN 'Fiberglass' " + vbCrLf)
            'stbQ.Append("       WHEN 4 THEN 'Cemento Armato' " + vbCrLf)
            'stbQ.Append("       WHEN 5 THEN 'Plastica Alimentare' " + vbCrLf)
            'stbQ.Append("       WHEN 6 THEN 'Rovere' " + vbCrLf)
            'stbQ.Append("       WHEN 7 THEN 'Castagno' " + vbCrLf)
            'stbQ.Append("       WHEN 8 THEN 'Ginepro' " + vbCrLf)
            'stbQ.Append("       WHEN 9 THEN 'Gelso' " + vbCrLf)
            'stbQ.Append("       WHEN 10 THEN 'Ciliegio' " + vbCrLf)
            'stbQ.Append("       WHEN 2222 THEN 'Cemento Fuori Terra Rivestita con Resine Epossidiche' " + vbCrLf)
            'stbQ.Append("       WHEN 3333 THEN 'Cemento Fuori Terra (Frigor)' " + vbCrLf)
            'stbQ.Append("       WHEN 4444 THEN 'Acciaio o Ferro Rivestito con Resine Epossidiche' " + vbCrLf)
            'stbQ.Append("       WHEN 5555 THEN 'Acciaio o Ferro Rivestito (Frigor)' " + vbCrLf)
            'stbQ.Append("       WHEN 6666 THEN 'Vetroresina' " + vbCrLf)
            'stbQ.Append("       WHEN 7777 THEN 'Acciaio per Elaborazione Vini Frizzanti' " + vbCrLf)
            'stbQ.Append("       ELSE 'Altro' END AS Materiale," + vbCrLf)

            '2016/06/24
            'Nuova classificazione
            'Il campo Materiale_cod è diventato varchar 
            stbQ.Append("       WHEN 'A' THEN 'Acciaio' " & vbCrLf)
            stbQ.Append("       WHEN 'B' THEN 'Cemento' " & vbCrLf)
            stbQ.Append("       WHEN 'C' THEN 'Legno' " & vbCrLf)
            stbQ.Append("       WHEN 'D' THEN 'Vetroresina' " & vbCrLf)
            stbQ.Append("       WHEN 'E' THEN 'Altro' " & vbCrLf)
            stbQ.Append("       ELSE 'Altro' END AS Materiale," & vbCrLf)

            stbQ.Append(" UDM_Capacita.Udm_SIM AS Udm_Capacita, Capacita_Nominale, Capacita_Effettiva, Modello, " & vbCrLf)
            stbQ.Append(" UDM_Altezza.Udm_SIM AS Udm_Altezza, Altezza_Cilindro, UDM.Udm_Sim,  Mat_Des, Movimenti_Dettagli.Lotto, " & vbCrLf)
            stbQ.Append(" Trasformazioni.linea_cod, Movimenti_Dettagli.Elem_Cod, Movimenti_Dettagli.Pro_Cod, Movimenti_Dettagli.Mat_Cod, Movimenti_Dettagli.Udm_Cod, " & vbCrLf)

            ' stbQ.Append(" (Mat_Des + ' (' + Movimenti_Dettagli.Lotto + ')' )  AS LineaProduttiva  " + vbCrLf)

            'modifica del 17/09/2015: i join li metto sempre
            'If Linea_Cod <> 0 Then
            '    stbQ.Append(" (Linee_Produzioni.Denominazione + ' ' + Linee_Produzioni.linea_des + '§' + CONVERT(varchar(50),Linee_Produzioni.Reg_Cod) ) AS LineaProduttiva  ")
            'Else
            '    stbQ.Append(" ISNULL ( ( SELECT (Linee_Produzioni.Denominazione + ' ' + Linee_Produzioni.linea_des + '§' + CONVERT(varchar(50),Linee_Produzioni.Reg_Cod)  ) AS LineaProduttiva ")
            '    stbQ.Append("            FROM     Trasformazioni   ")
            '    stbQ.Append("            INNER JOIN Linee_Produzioni on Linee_Produzioni.Linea_Cod = Trasformazioni.linea_cod " + vbCrLf)
            '    stbQ.Append("           AND Linee_Produzioni.piva = Trasformazioni.piva " + vbCrLf)
            '    stbQ.Append("            WHERE Movimenti_Dettagli.lotto= Trasformazioni.Trasformazione_des " + vbCrLf)
            '    stbQ.Append("           ), '§0' ) AS LineaProduttiva ")
            'End If

            'stbQ.Append(" (Linee_Produzioni.Denominazione + ' ' + Linee_Produzioni.linea_des + '§' + CONVERT(varchar(50),Linee_Produzioni.Reg_Cod) ) AS LineaProduttiva  ")

            stbQ.Append(" ( Linee_Produzioni.linea_des + ' ' + Linee_Produzioni.Denominazione  ")
            stbQ.Append("   + ISNULL( " & vbCrLf)
            stbQ.Append("                   ( SELECT TOP 1 ' - ' + Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome  " & vbCrLf)
            stbQ.Append("                   FROM Contatti " & vbCrLf)
            stbQ.Append("                   WHERE Linee_Produzioni.Cod_Contatto_Terzi = Contatti.Cod_Contatto " & vbCrLf)
            stbQ.Append("                   AND Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            stbQ.Append("                   AND Linee_Produzioni.Cod_Contatto_Terzi <> '' " & vbCrLf)
            stbQ.Append("                   AND Linee_Produzioni.Cod_Contatto_Terzi <> Linee_Produzioni.Piva " & vbCrLf)
            stbQ.Append("                   ) , '') " & vbCrLf)
            stbQ.Append(" ) AS LineaProduttiva,  ")
            stbQ.Append(" Linee_Produzioni.Reg_Cod  ")

            'Mov_Destinazioni.Qta,
            stbQ.Append(" , SUM( CASE WHEN Movimenti.CAU_MOV IN ('" & CStr(CAU_SCARICO) & "', '" & CStr(CAU_CONFERIMENTO_DIVERSI) & "') " & vbCrLf)
            stbQ.Append("           THEN -(Mov_Destinazioni.qta)" & vbCrLf)
            stbQ.Append("           ELSE Mov_Destinazioni.qta" & vbCrLf)
            stbQ.Append("           END) AS Qta " & vbCrLf)

            stbQ.Append(" FROM Cantina_Vasche " & vbCrLf)

            stbQ.Append(" INNER JOIN Imprese ON Cantina_Vasche.Piva = Imprese.Piva " & vbCrLf)
            stbQ.Append(" INNER JOIN Centri_Aziendali ON Centri_Aziendali.Piva = Cantina_Vasche.Piva   AND Centri_Aziendali.sa_cod = Cantina_Vasche.sa_cod  " & vbCrLf)

            stbQ.Append(" LEFT JOIN UnitaMisura UDM_Capacita ON UDM_Capacita.Udm_Cod = Udm_Cod_Capacita " & vbCrLf)
            stbQ.Append(" LEFT JOIN UnitaMisura UDM_Altezza ON UDM_ALtezza.Udm_Cod = Udm_Cod_Altezza " & vbCrLf)

            stbQ.Append(" INNER JOIN Cantina_Caratteristiche ON Cantina_Vasche.Piva = Cantina_Caratteristiche.PIVA AND Cantina_Vasche.Sa_Cod = Cantina_Caratteristiche.sa_cod AND Cantina_Vasche.Piano_Cod = Cantina_Caratteristiche.Piano_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Cantina_Vasche.PIVA = Mov_Destinazioni.PIVA AND Cantina_Vasche.Sa_Cod= Mov_Destinazioni.Sa_Cod AND  Cantina_Vasche.Vas_Cod = Mov_Destinazioni.Id_Destinazione " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND " & vbCrLf)
            stbQ.Append("               Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND " & vbCrLf)
            stbQ.Append("               Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura UDM ON UDM.Udm_Cod = Movimenti_Dettagli.Udm_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda And Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            '06/12/2017: eliminato il join sul sa_cod perché altrimenti non venivano letti i trasferimenti da un ICQRF (SA_COD) all'altro (ALTRO SA_COD) es: podere palazzo
            'stbQ.Append(" INNER JOIN Agenda ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda " + vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Movimenti_Dettagli.PIVA = Materie_Prime.PIVA AND Movimenti_Dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND  Movimenti_Dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)

            'modifica del 17/09/2015: i join li metto sempre
            'If Linea_Cod <> 0 Then
            stbQ.Append(" INNER JOIN Trasformazioni ON Movimenti_Dettagli.lotto= Trasformazioni.Trasformazione_des AND Movimenti_Dettagli.piva = Trasformazioni.piva " & vbCrLf)
            stbQ.Append(" INNER JOIN Linee_Produzioni on Linee_Produzioni.Linea_Cod = Trasformazioni.linea_cod " & vbCrLf)
            stbQ.Append(" AND Linee_Produzioni.piva = Trasformazioni.piva " & vbCrLf)
            'End If

            'NOTA DEL 07/09/2015: la tabella OGenerazioni_Anagrafe_Log può contenere tanti record uguali,
            'nei quali cambia solo id_generazione (a volte la rigenerazione non cancella i record vecchi)
            'per cui non si può usare in inner join
            'If Semilavorato_Cod <> 0 Or Categoria_Cod <> 0 Then
            '    'la materia prima è loggata su ogni linea, bisogna filtrare il linea-cod!
            '    stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON Movimenti_Dettagli.Mat_Cod = OGenerazioni_Anagrafe_Log.Mat_Cod AND Movimenti_Dettagli.elem_Cod = OGenerazioni_Anagrafe_Log.elem_Cod  " + vbCrLf)
            '    stbQ.Append("               AND Agenda.linea_cod = OGenerazioni_Anagrafe_Log.linea_cod " + vbCrLf)
            'End If

            stbQ.Append(" WHERE     Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Consistenza) & " " & vbCrLf)
            stbQ.Append(" AND       Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " & vbCrLf)

            stbQ.Append(" AND Movimenti.Cau_Mov IN ('" & CAU_CARICO & "', '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO & "', '" & CAU_CONFERIMENTO_DIVERSI & "')   " & vbCrLf)

            '--------------
            ' altrimenti i carichi e gli scarichi vengono sdoppiati nel caso di bolle agganciate a fatture
            'e vengono visualizzati anche i carichi/scarichi di operazioni pendenti
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            stbQ.Append(" AND   Mov_Destinazioni.Tipo_Destinazione = " & CStr(VASCA_ENOLOGICA) & "" & vbCrLf)

            stbQ.Append(" AND   Cantina_Vasche.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            'NOTA DEL 07/09/2015: la tabella OGenerazioni_Anagrafe_Log può contenere tanti record uguali,
            'nei quali cambia solo id_generazione (a volte la rigenerazione non cancella i record vecchi)
            'per cui non si può usare in inner join
            'If Semilavorato_Cod <> 0 OrElse Categoria_Cod <> 0 Then
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " + vbCrLf)
            '    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Tipo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.SemilavoratiMateriePrime) & " " + vbCrLf)

            '    If Categoria_Cod <> 0 Then
            '        stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Categoria_Gias_Cod_Log = " & Agro_SQL_SaveText(Categoria_Cod) & " " + vbCrLf)
            '    End If

            '    If Semilavorato_Cod <> 0 Then
            '        stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Codice_Generazione = " & Agro_SQL_SaveText(Semilavorato_Cod) & " " + vbCrLf)
            '    End If
            'End If


            If Semilavorato_Cod <> 0 OrElse Categoria_Cod <> 0 Then
                stbQ.Append(" AND  EXISTS ( ")
                stbQ.Append("             SELECT 1  ")
                stbQ.Append("             FROM OGenerazioni_Anagrafe_Log  ")
                stbQ.Append("             WHERE Movimenti_Dettagli.Mat_Cod = OGenerazioni_Anagrafe_Log.Mat_Cod AND Movimenti_Dettagli.elem_Cod = OGenerazioni_Anagrafe_Log.elem_Cod  " & vbCrLf)
                stbQ.Append("              AND Trasformazioni.linea_cod = OGenerazioni_Anagrafe_Log.linea_cod " & vbCrLf)
                stbQ.Append("             AND OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
                stbQ.Append("             AND OGenerazioni_Anagrafe_Log.Tipo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.SemilavoratiMateriePrime) & " " & vbCrLf)
                If Categoria_Cod <> 0 Then
                    stbQ.Append("         AND OGenerazioni_Anagrafe_Log.Categoria_Gias_Cod_Log = " & Agro_SQL_SaveNum(Categoria_Cod) & " " & vbCrLf)
                End If
                If Semilavorato_Cod <> 0 Then
                    stbQ.Append("         AND OGenerazioni_Anagrafe_Log.Codice_Generazione = " & Agro_SQL_SaveNum(Semilavorato_Cod) & " " & vbCrLf)
                End If
                stbQ.Append("            )")
            End If

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND   Cantina_Vasche.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)
            End If

            If Piano_Cod <> 0 Then
                stbQ.Append(" AND   Cantina_Caratteristiche.Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & " " & vbCrLf)
            End If

            If Vas_Cod <> 0 Then
                stbQ.Append(" AND   Cantina_Vasche.Vas_Cod = " & Agro_SQL_SaveNum(Vas_Cod) & " " & vbCrLf)
            End If

            If Linea_Cod <> 0 Then
                stbQ.Append(" AND   Trasformazioni.linea_cod = " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo1 <> "" Then
                stbQ.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo1, , objParametri) & vbCrLf)
            End If
            '--------------------------------------------------------------------------

            '------------------------------------------------------ 
            '----------------- GROUP BY ---------------------------
            '-----------------------------------------------------
            stbQ.Append(" GROUP BY  " & vbCrLf)
            'If Linea_Cod <> 0 Then
            stbQ.Append(" Linee_Produzioni.Denominazione, Linee_Produzioni.linea_des, Linee_Produzioni.Reg_Cod, " & vbCrLf)
            'End If
            stbQ.Append(" Rag_Soc, Sa_Nome, Cantina_Caratteristiche.Piano_Cod, Cantina_Vasche.Vas_Cod, Cantina_Caratteristiche.Piano_Des, " & vbCrLf)
            stbQ.Append(" Cantina_Vasche.Identificativo, Cantina_Vasche.Numero_Serie, Materiale_Cod, " & vbCrLf)
            stbQ.Append(" UDM_Capacita.Udm_SIM, Capacita_Nominale, Capacita_Effettiva, Modello, " & vbCrLf)
            stbQ.Append(" UDM_Altezza.Udm_SIM, Altezza_Cilindro, UDM.Udm_Sim,  Mat_Des, Movimenti_Dettagli.Lotto, " & vbCrLf)
            stbQ.Append(" Trasformazioni.linea_cod, Movimenti_Dettagli.Elem_Cod, Movimenti_Dettagli.Pro_Cod, Movimenti_Dettagli.Mat_Cod, Movimenti_Dettagli.Udm_Cod " & vbCrLf)
            stbQ.Append(" , Linee_Produzioni.Cod_Contatto_Terzi,Linee_Produzioni.Piva  " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- HAVING --------------------------
            '------------------------------------------------------
            If Flag_QtaNoZero Then
                'condizione per filtrare le giacenze=0
                stbQ.Append("  HAVING SUM(CASE WHEN Movimenti.Cau_Mov IN ('" & CStr(CAU_SCARICO) & "', '" & CStr(CAU_CONFERIMENTO_DIVERSI) & "') THEN -(Mov_Destinazioni.qta) ELSE Mov_Destinazioni.qta END) <> 0 " & vbCrLf)
            End If

            stbQ.Append(" ) " & vbCrLf)


            'x vedere anche le vasche che non sono state movimentate
            If QS_Flag_VascheNoMov Then

                '/*************************************************************************************
                '/******************            UNION ALL                   *************************
                '/*************************************************************************************

                stbQ.Append(" UNION ALL " & vbCrLf)

                '---------------------------------------------------------------------
                ' la seconda query della union seleziona le vasche vuote
                '---------------------------------------------------------------------
                stbQ.Append(" (" & vbCrLf)
                stbQ.Append(" SELECT  Rag_Soc, Sa_Nome, Cantina_Caratteristiche.Piano_Cod, Cantina_Vasche.Vas_Cod, Cantina_Caratteristiche.Piano_Des, " & vbCrLf)
                stbQ.Append(" Cantina_Vasche.Identificativo, Cantina_Vasche.Numero_Serie, " & vbCrLf)

                stbQ.Append(" CASE Materiale_Cod " & vbCrLf)
                'stbQ.Append("       WHEN 0 THEN 'Altro' " + vbCrLf)
                'stbQ.Append("       WHEN 1 THEN 'Acciaio Inox' " + vbCrLf)
                'stbQ.Append("       WHEN 2 THEN 'Acciaio Smaltato' " + vbCrLf)
                'stbQ.Append("       WHEN 3 THEN 'Fiberglass' " + vbCrLf)
                'stbQ.Append("       WHEN 4 THEN 'Cemento Armato' " + vbCrLf)
                'stbQ.Append("       WHEN 5 THEN 'Plastica Alimentare' " + vbCrLf)
                'stbQ.Append("       WHEN 6 THEN 'Rovere' " + vbCrLf)
                'stbQ.Append("       WHEN 7 THEN 'Castagno' " + vbCrLf)
                'stbQ.Append("       WHEN 8 THEN 'Ginepro' " + vbCrLf)
                'stbQ.Append("       WHEN 9 THEN 'Gelso' " + vbCrLf)
                'stbQ.Append("       WHEN 10 THEN 'Ciliegio' " + vbCrLf)
                'stbQ.Append("       WHEN 2222 THEN 'Cemento Fuori Terra Rivestita con Resine Epossidiche' " + vbCrLf)
                'stbQ.Append("       WHEN 3333 THEN 'Cemento Fuori Terra (Frigor)' " + vbCrLf)
                'stbQ.Append("       WHEN 4444 THEN 'Acciaio o Ferro Rivestito con Resine Epossidiche' " + vbCrLf)
                'stbQ.Append("       WHEN 5555 THEN 'Acciaio o Ferro Rivestito (Frigor)' " + vbCrLf)
                'stbQ.Append("       WHEN 6666 THEN 'Vetroresina' " + vbCrLf)
                'stbQ.Append("       WHEN 7777 THEN 'Acciaio per Elaborazione Vini Frizzanti' " + vbCrLf)
                'stbQ.Append("       ELSE 'Altro' END AS Materiale," + vbCrLf)

                '2016/06/24
                'Nuova classificazione
                'Il campo Materiale_cod è diventato varchar 
                stbQ.Append("       WHEN 'A' THEN 'Acciaio' " & vbCrLf)
                stbQ.Append("       WHEN 'B' THEN 'Cemento' " & vbCrLf)
                stbQ.Append("       WHEN 'C' THEN 'Legno' " & vbCrLf)
                stbQ.Append("       WHEN 'D' THEN 'Vetroresina' " & vbCrLf)
                stbQ.Append("       WHEN 'E' THEN 'Altro' " & vbCrLf)
                stbQ.Append("       ELSE 'Altro' END AS Materiale," & vbCrLf)


                stbQ.Append(" UDM_Capacita.Udm_SIM AS Udm_Capacita, Capacita_Nominale, Capacita_Effettiva, Modello, " & vbCrLf)
                stbQ.Append(" UDM_Altezza.Udm_SIM as Udm_Altezza, Altezza_Cilindro, '' AS Udm_Sim, '' AS Mat_Des, '' AS Lotto,   " & vbCrLf)
                stbQ.Append(" 0 AS linea_cod, 0 AS Elem_Cod, 0 AS Pro_Cod, 0 AS Mat_Cod, 0 AS Udm_Cod, " & vbCrLf)
                'stbQ.Append(" '§0' AS LineaProduttiva " + vbCrLf)
                stbQ.Append(" '' AS LineaProduttiva, 0 AS Reg_Cod " & vbCrLf)

                stbQ.Append(" , 0 AS Qta " & vbCrLf)

                stbQ.Append(" FROM Cantina_Vasche " & vbCrLf)

                stbQ.Append(" INNER JOIN Imprese ON Cantina_Vasche.Piva = Imprese.Piva " & vbCrLf)
                stbQ.Append(" INNER JOIN Centri_Aziendali ON Centri_Aziendali.Piva = Cantina_Vasche.Piva   AND Centri_Aziendali.sa_cod = Cantina_Vasche.sa_cod  " & vbCrLf)

                stbQ.Append(" LEFT JOIN UnitaMisura UDM_Capacita ON UDM_Capacita.Udm_Cod = Udm_Cod_Capacita " & vbCrLf)
                stbQ.Append(" LEFT JOIN UnitaMisura UDM_Altezza ON UDM_ALtezza.Udm_Cod = Udm_Cod_Altezza " & vbCrLf)
                stbQ.Append(" INNER JOIN Cantina_Caratteristiche ON Cantina_Vasche.Piva = Cantina_Caratteristiche.PIVA AND Cantina_Vasche.Sa_Cod = Cantina_Caratteristiche.sa_cod AND Cantina_Vasche.Piano_Cod = Cantina_Caratteristiche.Piano_Cod " & vbCrLf)

                stbQ.Append(" WHERE NOT EXISTS (    SELECT 1  " & vbCrLf)
                stbQ.Append("                       FROM Mov_Destinazioni " & vbCrLf)
                stbQ.Append("                       INNER JOIN Movimenti_dettagli ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND " & vbCrLf)
                stbQ.Append("                                   Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND " & vbCrLf)
                stbQ.Append("                                   Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)
                stbQ.Append("                       INNER JOIN Movimenti ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND " & vbCrLf)
                stbQ.Append("                                   Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda And Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
                stbQ.Append("                       INNER JOIN Agenda ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda " & vbCrLf)
                stbQ.Append("                       WHERE     Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Consistenza) & " " & vbCrLf)
                stbQ.Append("                       AND       Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " & vbCrLf)
                stbQ.Append("                       AND         Movimenti.Cau_Mov IN ('" & CAU_CARICO & "', '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO & "', '" & CAU_CONFERIMENTO_DIVERSI & "')   " & vbCrLf)
                '--------------
                ' altrimenti i carichi e gli scarichi vengono sdoppiati nel caso di bolle agganciate a fatture
                'e vengono visualizzati anche i carichi/scarichi di operazioni pendenti
                stbQ.Append("                       AND         Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
                stbQ.Append("                       AND         Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
                '--------------
                stbQ.Append("                       AND         Mov_Destinazioni.Tipo_Destinazione = " & CStr(VASCA_ENOLOGICA) & "" & vbCrLf)
                stbQ.Append("                       AND     Cantina_Vasche.PIVA = Mov_Destinazioni.PIVA AND Cantina_Vasche.Sa_Cod= Mov_Destinazioni.Sa_Cod AND  Cantina_Vasche.Vas_Cod = Mov_Destinazioni.Id_Destinazione " & vbCrLf)
                stbQ.Append("               ) " & vbCrLf)

                stbQ.Append(" AND   Cantina_Vasche.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
                If Sa_Cod <> 0 Then
                    stbQ.Append(" AND   Cantina_Vasche.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)
                End If

                If Piano_Cod <> 0 Then
                    stbQ.Append(" AND   Cantina_Caratteristiche.Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & " " & vbCrLf)
                End If

                If Vas_Cod <> 0 Then
                    stbQ.Append(" AND   Cantina_Vasche.Vas_Cod = " & Agro_SQL_SaveNum(Vas_Cod) & " " & vbCrLf)
                End If

                '--------------------------------------------------------------------------
                If xFiltroAggiuntivo2 <> "" Then
                    stbQ.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo2, , objParametri) & vbCrLf)
                End If
                '--------------------------------------------------------------------------
                stbQ.Append(" )" & vbCrLf)


            End If 'QS_Flag_VascheNoMov
            '/*************************************************************************************

            stbQ.Append(" ) ENOL " & vbCrLf)

            If xOrderBy <> "" Then
                stbQ.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & vbCrLf)
            Else
                stbQ.Append(" ORDER BY Piano_Cod, Piano_Des, identificativo " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
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
    ''' legge le consistenze enologiche alla data passata
    ''' fa un sum della qta dei movimenti fino alla data specificata
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function ConsistenzeEnologiche_OLD(ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Piano_Cod As Integer,
                                              ByVal Vas_Cod As Integer,
                                              ByVal Linea_Cod As Integer,
                                              ByVal Categoria_Cod As Integer,
                                              ByVal Semilavorato_Cod As Integer,
                                              ByVal Data_Consistenza As Date,
                                              ByVal Flag_QtaNoZero As Boolean,
                                              ByVal QS_Flag_VascheNoMov As Boolean,
                                              ByVal Ordinamento As enum_OrdinamentoStampaConsEnologiche,
                                              ByVal xFiltroAggiuntivo1 As String,
                                              ByVal xFiltroAggiuntivo2 As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocCantina.ConsistenzeEnologiche_OLD"

        Dim messaggioErrore As String = ""
        Dim stbQ As New Text.StringBuilder
        Dim dt As DataTable
        Dim xOrderBy As String = ""

        Try

            Select Case Ordinamento

                Case enum_OrdinamentoStampaConsEnologiche.IdentificativoVasca
                    'str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, Identificativo  "
                    xOrderBy = " Piano_Cod, Identificativo  "
                Case enum_OrdinamentoStampaConsEnologiche.LineaProduttiva
                    'str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, LineaProduttiva "
                    xOrderBy = " Piano_Cod, LineaProduttiva "
                Case enum_OrdinamentoStampaConsEnologiche.Lotto
                    ' str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, Lotto "
                    xOrderBy = " Piano_Cod, Lotto "
                Case enum_OrdinamentoStampaConsEnologiche.Materiale
                    ' str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, Materiale  "
                    xOrderBy = " Piano_Cod, Materiale  "
                Case enum_OrdinamentoStampaConsEnologiche.NumeroSerie
                    ' str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, NumeroSerie "
                    xOrderBy = " Piano_Cod, Numero_Serie "
                Case enum_OrdinamentoStampaConsEnologiche.Prodotto
                    ' str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, Mat_des "
                    xOrderBy = " Piano_Cod, Mat_des "
            End Select

            stbQ.Length = 0

            stbQ.Append(" SELECT * " & vbCrLf)
            stbQ.Append(" FROM " & vbCrLf)
            stbQ.Append(" ( " & vbCrLf)

            '---------------------------------------------------------------------
            ' la prima query della union  seleziona le vasche che hanno il vino
            '---------------------------------------------------------------------
            stbQ.Append(" ( " & vbCrLf)
            stbQ.Append(" SELECT  Rag_Soc, Sa_Nome, Cantina_Caratteristiche.Piano_Cod, Cantina_Vasche.Vas_Cod, Cantina_Caratteristiche.Piano_Des, " & vbCrLf)
            stbQ.Append(" Cantina_Vasche.Identificativo, Cantina_Vasche.Numero_Serie, " & vbCrLf)

            stbQ.Append(" CASE Materiale_Cod " & vbCrLf)
            stbQ.Append("       WHEN 0 THEN 'Altro' " & vbCrLf)
            stbQ.Append("       WHEN 1 THEN 'Acciaio Inox' " & vbCrLf)
            stbQ.Append("       WHEN 2 THEN 'Acciaio Smaltato' " & vbCrLf)
            stbQ.Append("       WHEN 3 THEN 'Fiberglass' " & vbCrLf)
            stbQ.Append("       WHEN 4 THEN 'Cemento Armato' " & vbCrLf)
            stbQ.Append("       WHEN 5 THEN 'Plastica Alimentare' " & vbCrLf)
            stbQ.Append("       WHEN 6 THEN 'Rovere' " & vbCrLf)
            stbQ.Append("       WHEN 7 THEN 'Castagno' " & vbCrLf)
            stbQ.Append("       WHEN 8 THEN 'Ginepro' " & vbCrLf)
            stbQ.Append("       WHEN 9 THEN 'Gelso' " & vbCrLf)
            stbQ.Append("       WHEN 10 THEN 'Ciliegio' " & vbCrLf)
            stbQ.Append("       WHEN 2222 THEN 'Cemento Fuori Terra Rivestita con Resine Epossidiche' " & vbCrLf)
            stbQ.Append("       WHEN 3333 THEN 'Cemento Fuori Terra (Frigor)' " & vbCrLf)
            stbQ.Append("       WHEN 4444 THEN 'Acciaio o Ferro Rivestito con Resine Epossidiche' " & vbCrLf)
            stbQ.Append("       WHEN 5555 THEN 'Acciaio o Ferro Rivestito (Frigor)' " & vbCrLf)
            stbQ.Append("       WHEN 6666 THEN 'Vetroresina' " & vbCrLf)
            stbQ.Append("       WHEN 7777 THEN 'Acciaio per Elaborazione Vini Frizzanti' " & vbCrLf)
            stbQ.Append("       ELSE 'Altro' END AS Materiale," & vbCrLf)

            stbQ.Append(" UDM_Capacita.Udm_SIM AS Udm_Capacita, Capacita_Nominale, Capacita_Effettiva, Modello, " & vbCrLf)
            stbQ.Append(" UDM_Altezza.Udm_SIM AS Udm_Altezza, Altezza_Cilindro, UDM.Udm_Sim,  Mat_Des, Movimenti_Dettagli.Lotto, " & vbCrLf)
            stbQ.Append(" Agenda.linea_cod, Movimenti_Dettagli.Elem_Cod, Movimenti_Dettagli.Pro_Cod, Movimenti_Dettagli.Mat_Cod, Movimenti_Dettagli.Udm_Cod, " & vbCrLf)

            ' stbQ.Append(" (Mat_Des + ' (' + Movimenti_Dettagli.Lotto + ')' )  AS LineaProduttiva  " + vbCrLf)
            If Linea_Cod <> 0 Then
                stbQ.Append(" (Linee_Produzioni.Denominazione + ' ' + Linee_Produzioni.linea_des + '§' + CONVERT(varchar(50),Linee_Produzioni.Reg_Cod) ) AS LineaProduttiva  ")
            Else
                stbQ.Append(" ISNULL ( ( SELECT (Linee_Produzioni.Denominazione + ' ' + Linee_Produzioni.linea_des + '§' + CONVERT(varchar(50),Linee_Produzioni.Reg_Cod)  ) AS LineaProduttiva ")
                stbQ.Append("            FROM     Trasformazioni   ")
                stbQ.Append("            INNER JOIN Linee_Produzioni on Linee_Produzioni.Linea_Cod = Trasformazioni.linea_cod " & vbCrLf)
                stbQ.Append("           AND Linee_Produzioni.piva = Trasformazioni.piva " & vbCrLf)
                stbQ.Append("            WHERE Movimenti_Dettagli.lotto= Trasformazioni.Trasformazione_des " & vbCrLf)
                stbQ.Append("           ), '§0' ) AS LineaProduttiva ")
            End If

            'Mov_Destinazioni.Qta,
            stbQ.Append(" , SUM( CASE WHEN Movimenti.CAU_MOV IN ('" & CStr(CAU_SCARICO) & "', '" & CStr(CAU_CONFERIMENTO_DIVERSI) & "') " & vbCrLf)
            stbQ.Append("           THEN -(Mov_Destinazioni.qta)" & vbCrLf)
            stbQ.Append("           ELSE Mov_Destinazioni.qta" & vbCrLf)
            stbQ.Append("           END) AS Qta " & vbCrLf)

            stbQ.Append(" FROM Cantina_Vasche " & vbCrLf)

            stbQ.Append(" INNER JOIN Imprese ON Cantina_Vasche.Piva = Imprese.Piva " & vbCrLf)
            stbQ.Append(" INNER JOIN Centri_Aziendali ON Centri_Aziendali.Piva = Cantina_Vasche.Piva   AND Centri_Aziendali.sa_cod = Cantina_Vasche.sa_cod  " & vbCrLf)

            stbQ.Append(" LEFT JOIN UnitaMisura UDM_Capacita ON UDM_Capacita.Udm_Cod = Udm_Cod_Capacita " & vbCrLf)
            stbQ.Append(" LEFT JOIN UnitaMisura UDM_Altezza ON UDM_ALtezza.Udm_Cod = Udm_Cod_Altezza " & vbCrLf)

            stbQ.Append(" INNER JOIN Cantina_Caratteristiche ON Cantina_Vasche.Piva = Cantina_Caratteristiche.PIVA AND Cantina_Vasche.Sa_Cod = Cantina_Caratteristiche.sa_cod AND Cantina_Vasche.Piano_Cod = Cantina_Caratteristiche.Piano_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Cantina_Vasche.PIVA = Mov_Destinazioni.PIVA AND Cantina_Vasche.Sa_Cod= Mov_Destinazioni.Sa_Cod AND  Cantina_Vasche.Vas_Cod = Mov_Destinazioni.Id_Destinazione " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND " & vbCrLf)
            stbQ.Append("               Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND " & vbCrLf)
            stbQ.Append("               Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura UDM ON UDM.Udm_Cod = Movimenti_Dettagli.Udm_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda And Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda " & vbCrLf)
            stbQ.Append(" INNER JOIN Materie_Prime ON Movimenti_Dettagli.PIVA = Materie_Prime.PIVA AND Movimenti_Dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND  Movimenti_Dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)

            If Linea_Cod <> 0 Then
                stbQ.Append(" INNER JOIN Trasformazioni ON Movimenti_Dettagli.lotto= Trasformazioni.Trasformazione_des AND Movimenti_Dettagli.piva= Trasformazioni.piva " & vbCrLf)
                stbQ.Append(" INNER JOIN Linee_Produzioni on Linee_Produzioni.Linea_Cod = Trasformazioni.linea_cod " & vbCrLf)
                stbQ.Append(" AND Linee_Produzioni.piva = Trasformazioni.piva " & vbCrLf)
            End If

            If Semilavorato_Cod <> 0 OrElse Categoria_Cod <> 0 Then
                'la materia prima è loggata su ogni linea, bisogna filtrare il linea-cod!
                stbQ.Append(" INNER JOIN OGenerazioni_Anagrafe_Log ON Movimenti_Dettagli.Mat_Cod = OGenerazioni_Anagrafe_Log.Mat_Cod AND Movimenti_Dettagli.elem_Cod = OGenerazioni_Anagrafe_Log.elem_Cod  " & vbCrLf)
                stbQ.Append("               AND Agenda.linea_cod = OGenerazioni_Anagrafe_Log.linea_cod " & vbCrLf)
            End If

            stbQ.Append(" WHERE     Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Consistenza) & " " & vbCrLf)
            stbQ.Append(" AND       Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " & vbCrLf)

            stbQ.Append(" AND Movimenti.Cau_Mov IN ('" & CAU_CARICO & "', '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO & "', '" & CAU_CONFERIMENTO_DIVERSI & "')   " & vbCrLf)

            '--------------
            ' altrimenti i carichi e gli scarichi vengono sdoppiati nel caso di bolle agganciate a fatture
            'e vengono visualizzati anche i carichi/scarichi di operazioni pendenti
            stbQ.Append(" AND   Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
            stbQ.Append(" AND   Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
            '--------------

            stbQ.Append(" AND   Mov_Destinazioni.Tipo_Destinazione = " & CStr(VASCA_ENOLOGICA) & "" & vbCrLf)

            stbQ.Append(" AND   Cantina_Vasche.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)

            If Semilavorato_Cod <> 0 OrElse Categoria_Cod <> 0 Then

                stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " & vbCrLf)
                stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Tipo_Generazione = " & Agro_SQL_SaveNum(enum_Omni_Tipo_Generazione.SemilavoratiMateriePrime) & " " & vbCrLf)

                If Categoria_Cod <> 0 Then
                    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Categoria_Gias_Cod_Log = " & Agro_SQL_SaveText(Categoria_Cod) & " " & vbCrLf)
                End If

                If Semilavorato_Cod <> 0 Then
                    stbQ.Append(" AND OGenerazioni_Anagrafe_Log.Codice_Generazione = " & Agro_SQL_SaveText(Semilavorato_Cod) & " " & vbCrLf)
                End If

            End If

            If Sa_Cod <> 0 Then
                stbQ.Append(" AND   Cantina_Vasche.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)
            End If

            If Piano_Cod <> 0 Then
                stbQ.Append(" AND   Cantina_Caratteristiche.Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & " " & vbCrLf)
            End If

            If Vas_Cod <> 0 Then
                stbQ.Append(" AND   Cantina_Vasche.Vas_Cod = " & Agro_SQL_SaveNum(Vas_Cod) & " " & vbCrLf)
            End If

            If Linea_Cod <> 0 Then
                stbQ.Append(" AND   Trasformazioni.linea_cod = " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo1 <> "" Then
                stbQ.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo1, , objParametri) & vbCrLf)
            End If
            '--------------------------------------------------------------------------

            '------------------------------------------------------ 
            '----------------- GROUP BY ---------------------------
            '-----------------------------------------------------
            stbQ.Append(" GROUP BY  " & vbCrLf)
            If Linea_Cod <> 0 Then
                stbQ.Append(" Linee_Produzioni.Denominazione, Linee_Produzioni.linea_des, Linee_Produzioni.Reg_Cod, " & vbCrLf)
                'Else
                '    stbQ.Append(" Agenda.Piva, Movimenti_dettagli.elem_cod, Movimenti_dettagli.mat_cod, " + vbCrLf)
            End If
            stbQ.Append(" Rag_Soc, Sa_Nome, Cantina_Caratteristiche.Piano_Cod, Cantina_Vasche.Vas_Cod, Cantina_Caratteristiche.Piano_Des, " & vbCrLf)
            stbQ.Append(" Cantina_Vasche.Identificativo, Cantina_Vasche.Numero_Serie, Materiale_Cod, " & vbCrLf)
            stbQ.Append(" UDM_Capacita.Udm_SIM, Capacita_Nominale, Capacita_Effettiva, Modello, " & vbCrLf)
            stbQ.Append(" UDM_Altezza.Udm_SIM, Altezza_Cilindro, UDM.Udm_Sim,  Mat_Des, Movimenti_Dettagli.Lotto, " & vbCrLf)
            stbQ.Append(" Agenda.linea_cod, Movimenti_Dettagli.Elem_Cod, Movimenti_Dettagli.Pro_Cod, Movimenti_Dettagli.Mat_Cod, Movimenti_Dettagli.Udm_Cod " & vbCrLf)

            stbQ.Append("   " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- HAVING --------------------------
            '------------------------------------------------------
            If Flag_QtaNoZero Then
                'condizione per filtrare le giacenze=0
                stbQ.Append("  HAVING SUM(CASE WHEN Movimenti.Cau_Mov IN ('" & CStr(CAU_SCARICO) & "', '" & CStr(CAU_CONFERIMENTO_DIVERSI) & "') THEN -(Mov_Destinazioni.qta) ELSE Mov_Destinazioni.qta END) <> 0 " & vbCrLf)
            End If

            stbQ.Append(" ) " & vbCrLf)


            'x vedere anche le vasche che non sono state movimentate
            If QS_Flag_VascheNoMov Then

                '/*************************************************************************************
                '/******************            UNION ALL                   *************************
                '/*************************************************************************************

                stbQ.Append(" UNION ALL " & vbCrLf)

                '---------------------------------------------------------------------
                ' la seconda query della union seleziona le vasche vuote
                '---------------------------------------------------------------------
                stbQ.Append(" (" & vbCrLf)
                stbQ.Append(" SELECT  Rag_Soc, Sa_Nome, Cantina_Caratteristiche.Piano_Cod, Cantina_Vasche.Vas_Cod, Cantina_Caratteristiche.Piano_Des, " & vbCrLf)
                stbQ.Append(" Cantina_Vasche.Identificativo, Cantina_Vasche.Numero_Serie, " & vbCrLf)

                stbQ.Append(" CASE Materiale_Cod " & vbCrLf)
                stbQ.Append("       WHEN 0 THEN 'Altro' " & vbCrLf)
                stbQ.Append("       WHEN 1 THEN 'Acciaio Inox' " & vbCrLf)
                stbQ.Append("       WHEN 2 THEN 'Acciaio Smaltato' " & vbCrLf)
                stbQ.Append("       WHEN 3 THEN 'Fiberglass' " & vbCrLf)
                stbQ.Append("       WHEN 4 THEN 'Cemento Armato' " & vbCrLf)
                stbQ.Append("       WHEN 5 THEN 'Plastica Alimentare' " & vbCrLf)
                stbQ.Append("       WHEN 6 THEN 'Rovere' " & vbCrLf)
                stbQ.Append("       WHEN 7 THEN 'Castagno' " & vbCrLf)
                stbQ.Append("       WHEN 8 THEN 'Ginepro' " & vbCrLf)
                stbQ.Append("       WHEN 9 THEN 'Gelso' " & vbCrLf)
                stbQ.Append("       WHEN 10 THEN 'Ciliegio' " & vbCrLf)
                stbQ.Append("       WHEN 2222 THEN 'Cemento Fuori Terra Rivestita con Resine Epossidiche' " & vbCrLf)
                stbQ.Append("       WHEN 3333 THEN 'Cemento Fuori Terra (Frigor)' " & vbCrLf)
                stbQ.Append("       WHEN 4444 THEN 'Acciaio o Ferro Rivestito con Resine Epossidiche' " & vbCrLf)
                stbQ.Append("       WHEN 5555 THEN 'Acciaio o Ferro Rivestito (Frigor)' " & vbCrLf)
                stbQ.Append("       WHEN 6666 THEN 'Vetroresina' " & vbCrLf)
                stbQ.Append("       WHEN 7777 THEN 'Acciaio per Elaborazione Vini Frizzanti' " & vbCrLf)
                stbQ.Append("       ELSE 'Altro' END AS Materiale," & vbCrLf)


                stbQ.Append(" UDM_Capacita.Udm_SIM AS Udm_Capacita, Capacita_Nominale, Capacita_Effettiva, Modello, " & vbCrLf)
                stbQ.Append(" UDM_Altezza.Udm_SIM as Udm_Altezza, Altezza_Cilindro, '' AS Udm_Sim, '' AS Mat_Des, '' AS Lotto,   " & vbCrLf)
                stbQ.Append(" 0 AS linea_cod, 0 AS Elem_Cod, 0 AS Pro_Cod, 0 AS Mat_Cod, 0 AS Udm_Cod, " & vbCrLf)
                stbQ.Append(" '§0' AS LineaProduttiva " & vbCrLf)

                stbQ.Append(" , 0 AS Qta " & vbCrLf)

                stbQ.Append(" FROM Cantina_Vasche " & vbCrLf)

                stbQ.Append(" INNER JOIN Imprese ON Cantina_Vasche.Piva = Imprese.Piva " & vbCrLf)
                stbQ.Append(" INNER JOIN Centri_Aziendali ON Centri_Aziendali.Piva = Cantina_Vasche.Piva   AND Centri_Aziendali.sa_cod = Cantina_Vasche.sa_cod  " & vbCrLf)

                stbQ.Append(" LEFT JOIN UnitaMisura UDM_Capacita ON UDM_Capacita.Udm_Cod = Udm_Cod_Capacita " & vbCrLf)
                stbQ.Append(" LEFT JOIN UnitaMisura UDM_Altezza ON UDM_ALtezza.Udm_Cod = Udm_Cod_Altezza " & vbCrLf)
                stbQ.Append(" INNER JOIN Cantina_Caratteristiche ON Cantina_Vasche.Piva = Cantina_Caratteristiche.PIVA AND Cantina_Vasche.Sa_Cod = Cantina_Caratteristiche.sa_cod AND Cantina_Vasche.Piano_Cod = Cantina_Caratteristiche.Piano_Cod " & vbCrLf)

                stbQ.Append(" WHERE NOT EXISTS (    SELECT 1  " & vbCrLf)
                stbQ.Append("                       FROM Mov_Destinazioni " & vbCrLf)
                stbQ.Append("                       INNER JOIN Movimenti_dettagli ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND " & vbCrLf)
                stbQ.Append("                                   Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND " & vbCrLf)
                stbQ.Append("                                   Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)
                stbQ.Append("                       INNER JOIN Movimenti ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND " & vbCrLf)
                stbQ.Append("                                   Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda And Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
                stbQ.Append("                       INNER JOIN Agenda ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda " & vbCrLf)
                stbQ.Append("                       WHERE     Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Consistenza) & " " & vbCrLf)
                stbQ.Append("                       AND       Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " " & vbCrLf)
                stbQ.Append("                       AND         Movimenti.Cau_Mov IN ('" & CAU_CARICO & "', '" & CAU_SCARICO & "', '" & CAU_CONFERIMENTO & "', '" & CAU_CONFERIMENTO_DIVERSI & "')   " & vbCrLf)
                '--------------
                ' altrimenti i carichi e gli scarichi vengono sdoppiati nel caso di bolle agganciate a fatture
                'e vengono visualizzati anche i carichi/scarichi di operazioni pendenti
                stbQ.Append("                       AND         Movimenti_Dettagli.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   " & vbCrLf)
                stbQ.Append("                       AND         Movimenti_Dettagli.Contabilizzato >= 0  " & vbCrLf)
                '--------------
                stbQ.Append("                       AND         Mov_Destinazioni.Tipo_Destinazione = " & CStr(VASCA_ENOLOGICA) & "" & vbCrLf)
                stbQ.Append("                       AND     Cantina_Vasche.PIVA = Mov_Destinazioni.PIVA AND Cantina_Vasche.Sa_Cod= Mov_Destinazioni.Sa_Cod AND  Cantina_Vasche.Vas_Cod = Mov_Destinazioni.Id_Destinazione " & vbCrLf)
                stbQ.Append("               ) " & vbCrLf)

                stbQ.Append(" AND   Cantina_Vasche.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
                If Sa_Cod <> 0 Then
                    stbQ.Append(" AND   Cantina_Vasche.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)
                End If

                If Piano_Cod <> 0 Then
                    stbQ.Append(" AND   Cantina_Caratteristiche.Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & " " & vbCrLf)
                End If

                If Vas_Cod <> 0 Then
                    stbQ.Append(" AND   Cantina_Vasche.Vas_Cod = " & Agro_SQL_SaveNum(Vas_Cod) & " " & vbCrLf)
                End If

                '--------------------------------------------------------------------------
                If xFiltroAggiuntivo2 <> "" Then
                    stbQ.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo2, , objParametri) & vbCrLf)
                End If
                '--------------------------------------------------------------------------
                stbQ.Append(" )" & vbCrLf)


            End If 'QS_Flag_VascheNoMov
            '/*************************************************************************************

            stbQ.Append(" ) ENOL " & vbCrLf)

            If xOrderBy <> "" Then
                stbQ.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & vbCrLf)
            Else
                stbQ.Append(" ORDER BY Piano_Cod, Piano_Des, identificativo " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
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
    ''' legge le consistenze alla data odierna 
    ''' legge il record statico della consistenza -3 (sperando che la qta sia salvata bene, se non lo è bisogna lanciare il verificatore)
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function ConsistenzeEnologiche_RecordStatico(ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal Piano_Cod As Integer,
                                                        ByVal Vas_Cod As Integer,
                                                        ByVal Linea_Cod As Integer,
                                                        ByVal Flag_QtaNoZero As Boolean,
                                                        ByVal QS_Flag_VascheNoMov As Boolean,
                                                        ByVal Ordinamento As enum_OrdinamentoStampaConsEnologiche,
                                                        ByVal xFiltroAggiuntivo1 As String,
                                                        ByVal xFiltroAggiuntivo2 As String,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocCantina.ConsistenzeEnologiche_RecordStatico"

        Dim messaggioErrore As String = ""
        Dim stbQ As New Text.StringBuilder
        Dim dt As DataTable
        Dim xOrderBy As String = ""

        Try

            Select Case Ordinamento

                Case enum_OrdinamentoStampaConsEnologiche.IdentificativoVasca
                    'str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, Identificativo  "
                    xOrderBy = " Piano_Cod, Identificativo  "
                Case enum_OrdinamentoStampaConsEnologiche.LineaProduttiva
                    'str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, LineaProduttiva "
                    xOrderBy = " Piano_Cod, LineaProduttiva "
                Case enum_OrdinamentoStampaConsEnologiche.Lotto
                    ' str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, Lotto "
                    xOrderBy = " Piano_Cod, Lotto "
                Case enum_OrdinamentoStampaConsEnologiche.Materiale
                    ' str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, Materiale  "
                    xOrderBy = " Piano_Cod, Materiale  "
                Case enum_OrdinamentoStampaConsEnologiche.NumeroSerie
                    ' str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, NumeroSerie "
                    xOrderBy = " Piano_Cod, Numero_Serie "
                Case enum_OrdinamentoStampaConsEnologiche.Prodotto
                    ' str_OrderBy = " Cantina_Caratteristiche.Piano_Cod, Mat_des "
                    xOrderBy = " Piano_Cod, Mat_des "
            End Select

            stbQ.Length = 0

            stbQ.Append(" SELECT * " & vbCrLf)
            stbQ.Append(" FROM " & vbCrLf)
            stbQ.Append(" ( " & vbCrLf)

            '---------------------------------------------------------------------
            ' la prima query della union  seleziona le vasche che hanno del vino
            '---------------------------------------------------------------------
            stbQ.Append(" ( " & vbCrLf)
            stbQ.Append(" SELECT  Rag_Soc, Sa_Nome, Cantina_Caratteristiche.Piano_Cod, Cantina_Vasche.Vas_Cod, Cantina_Caratteristiche.Piano_Des, " & vbCrLf)
            stbQ.Append(" Cantina_Vasche.Identificativo, Cantina_Vasche.Numero_Serie, " & vbCrLf)

            stbQ.Append(" CASE Materiale_Cod " & vbCrLf)
            stbQ.Append("       WHEN 0 THEN 'Altro' " & vbCrLf)
            stbQ.Append("       WHEN 1 THEN 'Acciaio Inox' " & vbCrLf)
            stbQ.Append("       WHEN 2 THEN 'Acciaio Smaltato' " & vbCrLf)
            stbQ.Append("       WHEN 3 THEN 'Fiberglass' " & vbCrLf)
            stbQ.Append("       WHEN 4 THEN 'Cemento Armato' " & vbCrLf)
            stbQ.Append("       WHEN 5 THEN 'Plastica Alimentare' " & vbCrLf)
            stbQ.Append("       WHEN 6 THEN 'Rovere' " & vbCrLf)
            stbQ.Append("       WHEN 7 THEN 'Castagno' " & vbCrLf)
            stbQ.Append("       WHEN 8 THEN 'Ginepro' " & vbCrLf)
            stbQ.Append("       WHEN 9 THEN 'Gelso' " & vbCrLf)
            stbQ.Append("       WHEN 10 THEN 'Ciliegio' " & vbCrLf)
            stbQ.Append("       WHEN 2222 THEN 'Cemento Fuori Terra Rivestita con Resine Epossidiche' " & vbCrLf)
            stbQ.Append("       WHEN 3333 THEN 'Cemento Fuori Terra (Frigor)' " & vbCrLf)
            stbQ.Append("       WHEN 4444 THEN 'Acciaio o Ferro Rivestito con Resine Epossidiche' " & vbCrLf)
            stbQ.Append("       WHEN 5555 THEN 'Acciaio o Ferro Rivestito (Frigor)' " & vbCrLf)
            stbQ.Append("       WHEN 6666 THEN 'Vetroresina' " & vbCrLf)
            stbQ.Append("       WHEN 7777 THEN 'Acciaio per Elaborazione Vini Frizzanti' " & vbCrLf)
            stbQ.Append("       ELSE 'Altro' END AS Materiale," & vbCrLf)

            stbQ.Append(" UDM_Capacita.Udm_SIM AS Udm_Capacita, Capacita_Nominale, Capacita_Effettiva, Modello, " & vbCrLf)
            stbQ.Append(" UDM_Altezza.Udm_SIM AS Udm_Altezza, Altezza_Cilindro, UDM.Udm_Sim, Mov_Destinazioni.Qta, Mat_Des, Movimenti_Dettagli.Lotto, " & vbCrLf)

            'stbQ.Append(" (Mat_Des + ' (' + Movimenti_Dettagli.Lotto + ')' )  AS LineaProduttiva  " & vbCrLf)
            If Linea_Cod <> 0 Then
                stbQ.Append(" (Linee_Produzioni.Denominazione + ' ' + Linee_Produzioni.linea_des) AS LineaProduttiva ")
            Else
                stbQ.Append(" ISNULL ( ( SELECT (Linee_Produzioni.Denominazione + ' ' + Linee_Produzioni.linea_des) AS LineaProduttiva ")
                stbQ.Append("            FROM     Trasformazioni   ")
                stbQ.Append("            INNER JOIN Linee_Produzioni on Linee_Produzioni.Linea_Cod = Trasformazioni.linea_cod " & vbCrLf)
                stbQ.Append("            AND Linee_Produzioni.piva = Trasformazioni.piva " & vbCrLf)
                stbQ.Append("            WHERE Movimenti_Dettagli.lotto= Trasformazioni.Trasformazione_des " & vbCrLf)
                stbQ.Append("           ), '' ) AS LineaProduttiva ")
            End If

            stbQ.Append(" FROM Cantina_Vasche " & vbCrLf)

            stbQ.Append(" INNER JOIN Imprese ON Cantina_Vasche.Piva = Imprese.Piva " & vbCrLf)
            stbQ.Append(" INNER JOIN Centri_Aziendali ON Centri_Aziendali.Piva = Cantina_Vasche.Piva   AND Centri_Aziendali.sa_cod = Cantina_Vasche.sa_cod  " & vbCrLf)

            stbQ.Append(" LEFT JOIN UnitaMisura UDM_Capacita ON UDM_Capacita.Udm_Cod = Udm_Cod_Capacita " & vbCrLf)
            stbQ.Append(" LEFT JOIN UnitaMisura UDM_Altezza ON UDM_ALtezza.Udm_Cod = Udm_Cod_Altezza " & vbCrLf)

            stbQ.Append(" INNER JOIN Cantina_Caratteristiche ON Cantina_Vasche.Piva = Cantina_Caratteristiche.PIVA AND Cantina_Vasche.Sa_Cod = Cantina_Caratteristiche.sa_cod AND Cantina_Vasche.Piano_Cod = Cantina_Caratteristiche.Piano_Cod " & vbCrLf)

            stbQ.Append(" INNER JOIN Mov_Destinazioni ON Cantina_Vasche.PIVA = Mov_Destinazioni.PIVA AND Cantina_Vasche.Sa_Cod= Mov_Destinazioni.Sa_Cod AND  Cantina_Vasche.Vas_Cod = Mov_Destinazioni.Id_Destinazione " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti_dettagli ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND " & vbCrLf)
            stbQ.Append(" Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)
            stbQ.Append(" INNER JOIN UnitaMisura UDM ON UDM.Udm_Cod = Movimenti_Dettagli.Udm_Cod " & vbCrLf)
            stbQ.Append(" INNER JOIN Movimenti ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda And Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
            stbQ.Append(" INNER JOIN Agenda ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda " & vbCrLf)
            stbQ.Append(" INNER Join Materie_Prime ON Movimenti_Dettagli.PIVA = Materie_Prime.PIVA AND Movimenti_Dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND  Movimenti_Dettagli.Mat_Cod = Materie_Prime.Mat_Cod " & vbCrLf)

            If Linea_Cod <> 0 Then
                stbQ.Append(" INNER JOIN Trasformazioni ON Movimenti_Dettagli.lotto= Trasformazioni.Trasformazione_des " & vbCrLf)
                stbQ.Append(" INNER JOIN Linee_Produzioni on Linee_Produzioni.Linea_Cod = Trasformazioni.linea_cod " & vbCrLf)
                stbQ.Append(" AND Linee_Produzioni.piva = Trasformazioni.piva " & vbCrLf)
            End If

            stbQ.Append(" WHERE Agenda.Lav_Cod = -3 " & vbCrLf)
            stbQ.Append(" AND   Movimenti.Cau_Mov = 'MOSTO' " & vbCrLf)  ' record relativo al rilevamento delle consistenze enologiche
            stbQ.Append(" AND   Mov_Destinazioni.Tipo_Destinazione = " & CStr(VASCA_ENOLOGICA) & " " & vbCrLf) ' La destinazione è una vasca

            If Flag_QtaNoZero Then
                stbQ.Append(" AND   Mov_Destinazioni.Qta <> 0 " & vbCrLf)
            End If

            stbQ.Append(" AND   Cantina_Vasche.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            stbQ.Append(" AND   Cantina_Vasche.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)

            If Piano_Cod <> 0 Then
                stbQ.Append(" AND   Cantina_Caratteristiche.Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & " " & vbCrLf)
            End If

            If Vas_Cod <> 0 Then
                stbQ.Append(" AND   Cantina_Vasche.Vas_Cod = " & Agro_SQL_SaveNum(Vas_Cod) & " " & vbCrLf)
            End If

            If Linea_Cod <> 0 Then
                stbQ.Append(" AND   Trasformazioni.linea_cod = " & Agro_SQL_SaveNum(Linea_Cod) & " " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo1 <> "" Then
                stbQ.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo1, , objParametri))
            End If
            '--------------------------------------------------------------------------
            stbQ.Append(" ) " & vbCrLf)


            'x vedere anche le vasche che non sono state movimentate
            If QS_Flag_VascheNoMov Then

                '/*************************************************************************************
                '/******************            UNION ALL                   *************************
                '/*************************************************************************************
                stbQ.Append(" UNION ALL " & vbCrLf)

                '---------------------------------------------------------------------
                ' la seconda query della union seleziona le vasche vuote
                '---------------------------------------------------------------------
                stbQ.Append(" (" & vbCrLf)
                stbQ.Append(" SELECT  Rag_Soc, Sa_Nome, Cantina_Caratteristiche.Piano_Cod, Cantina_Vasche.Vas_Cod, Cantina_Caratteristiche.Piano_Des, " & vbCrLf)
                stbQ.Append(" Cantina_Vasche.Identificativo, Cantina_Vasche.Numero_Serie, " & vbCrLf)

                stbQ.Append(" CASE Materiale_Cod " & vbCrLf)
                stbQ.Append("       WHEN 0 THEN 'Altro' " & vbCrLf)
                stbQ.Append("       WHEN 1 THEN 'Acciaio Inox' " & vbCrLf)
                stbQ.Append("       WHEN 2 THEN 'Acciaio Smaltato' " & vbCrLf)
                stbQ.Append("       WHEN 3 THEN 'Fiberglass' " & vbCrLf)
                stbQ.Append("       WHEN 4 THEN 'Cemento Armato' " & vbCrLf)
                stbQ.Append("       WHEN 5 THEN 'Plastica Alimentare' " & vbCrLf)
                stbQ.Append("       WHEN 6 THEN 'Rovere' " & vbCrLf)
                stbQ.Append("       WHEN 7 THEN 'Castagno' " & vbCrLf)
                stbQ.Append("       WHEN 8 THEN 'Ginepro' " & vbCrLf)
                stbQ.Append("       WHEN 9 THEN 'Gelso' " & vbCrLf)
                stbQ.Append("       WHEN 10 THEN 'Ciliegio' " & vbCrLf)
                stbQ.Append("       WHEN 2222 THEN 'Cemento Fuori Terra Rivestita con Resine Epossidiche' " & vbCrLf)
                stbQ.Append("       WHEN 3333 THEN 'Cemento Fuori Terra (Frigor)' " & vbCrLf)
                stbQ.Append("       WHEN 4444 THEN 'Acciaio o Ferro Rivestito con Resine Epossidiche' " & vbCrLf)
                stbQ.Append("       WHEN 5555 THEN 'Acciaio o Ferro Rivestito (Frigor)' " & vbCrLf)
                stbQ.Append("       WHEN 6666 THEN 'Vetroresina' " & vbCrLf)
                stbQ.Append("       WHEN 7777 THEN 'Acciaio per Elaborazione Vini Frizzanti' " & vbCrLf)
                stbQ.Append("       ELSE 'Altro' END AS Materiale," & vbCrLf)

                stbQ.Append(" UDM_Capacita.Udm_SIM AS Udm_Capacita, Capacita_Nominale, Capacita_Effettiva, Modello, " & vbCrLf)
                stbQ.Append(" UDM_Altezza.Udm_SIM as Udm_Altezza, Altezza_Cilindro, '' AS Udm_Sim, 0 AS Qta, '' AS Mat_Des, '' AS Lotto, '' AS LineaProduttiva " & vbCrLf)

                stbQ.Append(" FROM Cantina_Vasche " & vbCrLf)

                stbQ.Append(" INNER JOIN Imprese ON Cantina_Vasche.Piva = Imprese.Piva " & vbCrLf)
                stbQ.Append(" INNER JOIN Centri_Aziendali ON Centri_Aziendali.Piva = Cantina_Vasche.Piva   AND Centri_Aziendali.sa_cod = Cantina_Vasche.sa_cod  " & vbCrLf)

                stbQ.Append(" LEFT JOIN UnitaMisura UDM_Capacita ON UDM_Capacita.Udm_Cod = Udm_Cod_Capacita " & vbCrLf)
                stbQ.Append(" LEFT JOIN UnitaMisura UDM_Altezza ON UDM_ALtezza.Udm_Cod = Udm_Cod_Altezza " & vbCrLf)

                stbQ.Append(" INNER JOIN Cantina_Caratteristiche ON Cantina_Vasche.Piva = Cantina_Caratteristiche.PIVA AND Cantina_Vasche.Sa_Cod = Cantina_Caratteristiche.sa_cod AND Cantina_Vasche.Piano_Cod = Cantina_Caratteristiche.Piano_Cod " & vbCrLf)

                stbQ.Append(" WHERE NOT EXISTS ( SELECT 1 FROM " & vbCrLf)
                stbQ.Append("                    Mov_Destinazioni " & vbCrLf)
                stbQ.Append("                    INNER JOIN Movimenti_dettagli ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND " & vbCrLf)
                stbQ.Append("                    Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND " & vbCrLf)
                stbQ.Append("                    Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det " & vbCrLf)
                stbQ.Append("                    INNER JOIN Movimenti ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND " & vbCrLf)
                stbQ.Append("                    Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda And Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov " & vbCrLf)
                stbQ.Append("                    INNER JOIN Agenda ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda " & vbCrLf)
                stbQ.Append("                    WHERE Agenda.Lav_Cod = -3 " & vbCrLf)
                stbQ.Append("                    AND   Movimenti.Cau_Mov = 'MOSTO' " & vbCrLf)
                stbQ.Append("                    AND   Mov_Destinazioni.Tipo_Destinazione = " & CStr(VASCA_ENOLOGICA) & " " & vbCrLf)
                stbQ.Append("                    AND   Mov_Destinazioni.Qta <> 0 " & vbCrLf)
                stbQ.Append("                    AND   Cantina_Vasche.PIVA = Mov_Destinazioni.PIVA AND Cantina_Vasche.Sa_Cod= Mov_Destinazioni.Sa_Cod AND  Cantina_Vasche.Vas_Cod = Mov_Destinazioni.Id_Destinazione " & vbCrLf)
                stbQ.Append("                    ) " & vbCrLf)

                stbQ.Append(" AND   Cantina_Vasche.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
                stbQ.Append(" AND   Cantina_Vasche.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " " & vbCrLf)

                If Piano_Cod <> 0 Then
                    stbQ.Append(" AND   Cantina_Caratteristiche.Piano_Cod = " & Agro_SQL_SaveNum(Piano_Cod) & " " & vbCrLf)
                End If

                If Vas_Cod <> 0 Then
                    stbQ.Append(" AND   Cantina_Vasche.Vas_Cod = " & Agro_SQL_SaveNum(Vas_Cod) & " " & vbCrLf)
                End If

                '--------------------------------------------------------------------------
                If xFiltroAggiuntivo2 <> "" Then
                    stbQ.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo2, , objParametri))
                End If
                '--------------------------------------------------------------------------
                stbQ.Append(" )" & vbCrLf)

            End If 'QS_Flag_VascheNoMov

            '/*************************************************************************************

            stbQ.Append(" ) ENOL " & vbCrLf)

            If xOrderBy <> "" Then
                stbQ.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stbQ.Append(" ORDER BY Piano_Cod, Piano_Des, identificativo " & vbCrLf)
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQ.ToString, nomeRoutine)
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
    ''' DOCO
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function DOCO_Leggi(ByVal Piva As String,
                               ByVal Id_Agenda As Integer,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreParametri
                               ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocCantina.DOCO_Leggi"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StbSQL.Length = 0


            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            StbSQL.Append(" SELECT Agenda.PIVA, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Id_Mov AS Id_Mov_Contabile, Mov_Contabile.Data_Movimento, Mov_Contabile.Ora , " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Doc_Numero_Sin, Mov_Contabile.Doc_Numero,Mov_Contabile.Doc_Numero_Des, Mov_Contabile.Mov_Desc ,  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Scadenza, Mov_Contabile.Num_Protocollo, Mov_Contabile.Causale_Trasporto, Mov_Contabile.Aspetto, Mov_Contabile.Peso,  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Ora, Mov_Contabile.Colli, Mov_Contabile.Extra_Str, Mov_Contabile.Extra_Int, Mov_Contabile.Extra_Date,  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Tipo_Sconto, Mov_Contabile.Natura_Beni, Mov_Contabile.Tara_Veicolo, Mov_Contabile.Tara_Imballi, Mov_Contabile.Tipo_Peso, Mov_Contabile.Modalita, Mov_Contabile.Username_Note,  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Scadenza_Extra, Mov_Contabile.Progr_Protocollo, Mov_Contabile.Progr_Registrazione, Mov_Contabile.Data_Registrazione, Mov_Contabile.ChkLayOut_Join_Prodotti, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Contatti_Legale.Nome AS Nome_Legale, Contatti_Legale.Cognome AS Cognome_Legale, Contatti_Legale.Rag_Soc AS Rag_Soc_Legale, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Cod_RisUm, Contatti_Cliente.Cod_Contatto AS Cod_Contatto_Cliente, Contatti_Cliente.Rag_Soc AS Rag_Soc_Cliente, Contatti_Cliente.Codice_Fiscale AS Codice_Fiscale_Cliente,  " & vbCrLf)
            StbSQL.Append(" Contatti_Cliente.Id_Cf AS Id_Cf_Cliente, Contatti_Cliente.Nome AS Nome_Cliente, Contatti_Cliente.Cognome AS Cognome_Cliente,  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Cod_IndirizzoRisUm, Indirizzi_Cliente.ind_des AS ind_des_Cliente, Indirizzi_Cliente.frz_des AS frz_des_Cliente, Indirizzi_Cliente.CAP AS cap_Cliente,  " & vbCrLf)
            StbSQL.Append(" Indirizzi_Cliente.stato AS stato_Cliente, Indirizzi_Cliente.pro_cod_istat AS pro_cod_istat_Cliente,  " & vbCrLf)
            StbSQL.Append(" Indirizzi_Cliente.com_cod_istat AS com_cod_istat_Cliente, Istat_Cliente.LOCALITA AS localita_Cliente, Istat_Cliente.COMUNI_PROV AS comuni_prov_Cliente, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Cod_Destinazione, ISNULL(Contatti_DestDiv.Cod_Contatto, '') AS Cod_Contatto_DestDiv, ISNULL(Contatti_DestDiv.Rag_Soc, '') AS Rag_Soc_DestDiv, ISNULL(Contatti_DestDiv.Codice_Fiscale, '') AS Codice_Fiscale_DestDiv,  " & vbCrLf)
            StbSQL.Append(" ISNULL(Contatti_DestDiv.Id_Cf, 0) AS Id_Cf_DestDiv, ISNULL(Contatti_DestDiv.Nome, '') AS Nome_DestDiv, ISNULL(Contatti_DestDiv.Cognome, '') AS Cognome_DestDiv,  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Cod_IndirizzoDestinazione,   ISNULL(Indirizzi_DestDiv.ind_des, '') AS ind_des_DestDiv, ISNULL(Indirizzi_DestDiv.frz_des, '') AS frz_des_DestDiv, ISNULL(Indirizzi_DestDiv.CAP, '') AS cap_DestDiv,  " & vbCrLf)
            StbSQL.Append(" ISNULL(Indirizzi_DestDiv.stato, '') AS stato_DestDiv, ISNULL(Indirizzi_DestDiv.pro_cod_istat, '') AS pro_cod_istat_DestDiv,  " & vbCrLf)
            StbSQL.Append(" ISNULL(Indirizzi_DestDiv.com_cod_istat, '') AS com_cod_istat_DestDiv, ISNULL(Istat_DestDiv.LOCALITA, '') AS localita_DestDiv, ISNULL(Istat_DestDiv.COMUNI_PROV, '') AS comuni_prov_DestDiv, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Mezzo, Mov_Contabile.Cod_Vettore, " & vbCrLf)
            StbSQL.Append(" ISNULL(Contatti_Vettore.Cod_Contatto, '') AS Cod_Contatto_Vettore, ISNULL(Contatti_Vettore.Rag_Soc, '') AS Rag_Soc_Vettore, ISNULL(Contatti_Vettore.Codice_Fiscale, '') AS Codice_Fiscale_Vettore,  " & vbCrLf)
            StbSQL.Append(" ISNULL(Contatti_Vettore.Id_Cf, 0) AS Id_Cf_Vettore, ISNULL(Contatti_Vettore.Nome, '') AS Nome_Vettore, ISNULL(Contatti_Vettore.Cognome, '') AS Cognome_Vettore,  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Cod_IndirizzoVettore, ISNULL(Indirizzi_Vettore.ind_des, '') AS ind_des_Vettore, ISNULL(Indirizzi_Vettore.frz_des, '') AS frz_des_Vettore, ISNULL(Indirizzi_Vettore.CAP, '') AS cap_Vettore,  " & vbCrLf)
            StbSQL.Append(" ISNULL(Indirizzi_Vettore.stato, '') AS stato_Vettore, Indirizzi_Vettore.pro_cod_istat AS pro_cod_istat_Vettore,  " & vbCrLf)
            StbSQL.Append(" Indirizzi_Vettore.com_cod_istat AS com_cod_istat_Vettore, ISNULL(Istat_Vettore.LOCALITA, '') AS localita_Vettore, ISNULL(Istat_Vettore.COMUNI_PROV, '') AS comuni_prov_Vettore,  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            'StbSQL.Append(" Parco_Macchine.Class_Code, Parco_Macchine.Mac_Des, Parco_Macchine.Targa, Parco_Macchine.Telaio, Parco_Macchine.Modello,  " + vbCrLf)
            'StbSQL.Append(" Parco_Macchine.Potenza, Parco_Macchine.Data_Immatricolazione, Parco_Macchine.N_Immatricolazione,  " + vbCrLf)
            'StbSQL.Append(" Parco_Macchine.N_Immatricolazione_Rimorchio, Parco_Macchine.N_Autorizzazione_Trasporto, Parco_Macchine.Data_Rilascio_Autorizzazione, Parco_Macchine.Peso, " + vbCrLf)
            'StbSQL.Append("  " + vbCrLf)
            StbSQL.Append(" Movimento_Extra.Mac_Cod,  Movimento_Extra.Mezzo_Trasporto, Movimento_Extra.Targa AS Targa_2, Movimento_Extra.N_Immatricolazione AS N_Immatricolazione_2, Movimento_Extra.N_Immatricolazione_Rimorchio AS N_Immatricolazione_Rimorchio_2, " & vbCrLf)
            StbSQL.Append(" Movimento_Extra.N_Autorizzazione_Trasporto AS N_Autorizzazione_Trasporto_2, Movimento_Extra.Data_Rilascio_Autorizzazione AS Data_Rilascio_Autorizzazione_2, Movimento_Extra.Peso AS Peso_2, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Movimento_Extra.Id_Reg_Dettaglio AS Id_Movimento_Extra, Movimento_Extra.Indicazioni_Complementari, " & vbCrLf)
            StbSQL.Append(" Movimento_Extra.Tipo_Documento, Movimento_Extra.Luogo_Partenza, Movimento_Extra.Luogo_Consegna, Movimento_Extra.Id_Reg_Dettaglio AS Id_Movimento_Extra, Movimento_Extra.Indicazioni_Complementari, " & vbCrLf)
            StbSQL.Append(" Movimento_Extra.Data_Spedizione,  Movimento_Extra.Precisazioni, Movimento_Extra.Annotazioni,  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Mov_Magazzino.Id_Mov AS Id_Mov_Magazzino, Mov_Magazzino.Mov_Desc AS Mov_Desc_Mag, Mov_Magazzino.Data_Movimento AS Data_Mag,Mov_Magazzino.Ora AS Ora_Mag,  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Dettagli_Extra.Id_Reg_Dettaglio AS Id_Dettagli_Extra, ISNULL(Dettagli_Extra.Marche_Contenitori, '') AS Marche_Contenitori,  " & vbCrLf)
            StbSQL.Append(" ISNULL(Dettagli_Extra.Num_Contenitori, 0) AS Num_Contenitori, ISNULL(Dettagli_Extra.Des_Contenitori, '') AS Des_Contenitori, ISNULL(Dettagli_Extra.Num_Colli, 0) AS Num_Colli, Dettagli_Extra.Titolo_Alcol, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Id_Mov_Det, Mov_Dett_Magazzino.Mov_Det_Des, Mov_Dett_Magazzino.Elem_Cod, Mov_Dett_Magazzino.Pro_Cod, Mov_Dett_Magazzino.Mat_Cod,  " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Cod_Progetto, Mov_Dett_Magazzino.Fase_Cod, Mov_Dett_Magazzino.Lotto, Mov_Dett_Magazzino.Cal_Cod, Mov_Dett_Magazzino.Udm_Cod, UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim,  " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Qta AS Qta, Mov_Dett_Magazzino.Variazione AS Variazione,Mov_Dett_Magazzino.Listino_Cod, Mov_Dett_Magazzino.Tara,  " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Prezzo_Unitario, Mov_Dett_Magazzino.Prezzo_Unitario_Netto, Mov_Dett_Magazzino.Imponibile, Mov_Dett_Magazzino.Imponibile_Netto,  " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Jolly_Int, Mov_Dett_Magazzino.Contabilizzato,Mov_Dett_Magazzino.Pendente, Mov_Dett_Magazzino.Prezzo_Effettivo, " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Cod_Iva, Mov_Dett_Magazzino.Sconto, Mov_Dett_Magazzino.Cod_Conto, Mov_Dett_Magazzino.Extra_Str, Mov_Dett_Magazzino.Extra_Int, " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Extra_Date, Mov_Dett_Magazzino.Anno, Mov_Dett_Magazzino.Ric_Cod, Mov_Dett_Magazzino.Iva, Mov_Dett_Magazzino.UDM_COD_EXTRA, Mov_Dett_Magazzino.QTA_EXTRA,  " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Cod_IvaIndetraibile, Mov_Dett_Magazzino.Qta_Extra_Totale, Mov_Dett_Magazzino.Tara, Mov_Dett_Magazzino.ChkLayOut_Hide, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            'Dettagli_Extra.Codice_Prodotto, Dettagli_Extra.Colore, Dettagli_Extra.Zona_Viticola, Dettagli_Extra.Manipolazioni, 
            StbSQL.Append(" Materie_Prime.codice_prodotto, Materie_Prime.colore, Materie_Prime.codice_nc, Materie_Prime.manipolazioni, " & vbCrLf)
            StbSQL.Append(" IVA_Aliquote.Sigla AS Sigla_IVA " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            StbSQL.Append(" FROM Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Contabile ON Agenda.PIVA = Mov_Contabile.PIVA AND Agenda.Sa_Cod = Mov_Contabile.Sa_Cod AND Agenda.Id_Agenda = Mov_Contabile.Id_Agenda  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Cliente ON Mov_Contabile.Cod_RisUm = Risorse_Umane_Cliente.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Cliente ON Risorse_Umane_Cliente.Piva = Contatti_Cliente.Piva AND Risorse_Umane_Cliente.Cod_Contatto = Contatti_Cliente.Cod_Contatto  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Indirizzi Indirizzi_Cliente ON  Mov_Contabile.Cod_IndirizzoRisUm = Indirizzi_Cliente.cod_indirizzo " & vbCrLf)
            StbSQL.Append(" INNER JOIN ISTAT Istat_Cliente ON Indirizzi_Cliente.pro_cod_istat = Istat_Cliente.PROV AND Indirizzi_Cliente.com_cod_istat = Istat_Cliente.COM " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_DestDiv ON Mov_Contabile.Cod_Destinazione = Risorse_Umane_DestDiv.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Contatti Contatti_DestDiv ON Risorse_Umane_DestDiv.Piva = Contatti_DestDiv.Piva AND Risorse_Umane_DestDiv.Cod_Contatto = Contatti_DestDiv.Cod_Contatto  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Indirizzi Indirizzi_DestDiv ON  Mov_Contabile.Cod_IndirizzoDestinazione= Indirizzi_DestDiv.cod_indirizzo " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN ISTAT Istat_DestDiv ON Indirizzi_DestDiv.pro_cod_istat = Istat_DestDiv.PROV AND Indirizzi_DestDiv.com_cod_istat = Istat_DestDiv.COM " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Vettore ON Mov_Contabile.Cod_Vettore = Risorse_Umane_Vettore.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Contatti Contatti_Vettore ON Risorse_Umane_Vettore.Piva = Contatti_Vettore.Piva AND Risorse_Umane_Vettore.Cod_Contatto = Contatti_Vettore.Cod_Contatto  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Indirizzi Indirizzi_Vettore ON  Mov_Contabile.Cod_IndirizzoVettore = Indirizzi_Vettore.cod_indirizzo " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN ISTAT Istat_Vettore ON Indirizzi_Vettore.pro_cod_istat = Istat_Vettore.PROV AND Indirizzi_Vettore.com_cod_istat = Istat_Vettore.COM " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Legale ON Mov_Contabile.Piva = Risorse_Umane_Legale.Piva  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Legale ON Risorse_Umane_Legale.Piva = Contatti_Legale.Piva AND Risorse_Umane_Legale.Cod_Contatto = Contatti_Legale.Cod_Contatto  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra ON Movimento_Extra.PIVA = Mov_Contabile.PIVA AND Movimento_Extra.Sa_Cod = Mov_Contabile.Sa_Cod AND Movimento_Extra.Id_Agenda = Mov_Contabile.Id_Agenda AND Movimento_Extra.Id_Mov = Mov_Contabile.Id_Mov " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            'StbSQL.Append(" LEFT OUTER JOIN Parco_Macchine ON Movimento_Extra.Piva = Parco_Macchine.Piva AND Movimento_Extra.Mac_Cod = Parco_Macchine.Mac_Cod " + vbCrLf)
            'StbSQL.Append("  " + vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Sa_Cod = Mov_Magazzino.Sa_Cod AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod AND Materie_Prime.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod   " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Dettagli_Extra ON Dettagli_Extra.PIVA = Mov_Dett_Magazzino.PIVA AND Dettagli_Extra.Sa_Cod = Mov_Dett_Magazzino.Sa_Cod AND Dettagli_Extra.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Dettagli_Extra.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Dettagli_Extra.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Mov_Dett_Magazzino.Cod_Iva   " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            StbSQL.Append(" WHERE Agenda.Lav_Cod = " & CStr(LAVCOD_DOCO_EMESSO) & " " & vbCrLf)

            If Piva <> "" Then
                StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            End If

            If Id_Agenda <> 0 Then
                StbSQL.Append(" AND Agenda.Id_Agenda = " & CStr(Id_Agenda) & " " & vbCrLf)
            End If

            StbSQL.Append(" AND Mov_Contabile.Cau_Mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)
            StbSQL.Append(" AND Mov_Magazzino.Cau_Mov = '" & CAU_SCARICO & "' " & vbCrLf)
            StbSQL.Append(" AND Risorse_Umane_Legale.Cod_Rapporto= " & CStr(COD_LEGALE) & " " & vbCrLf)

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" " & vbCrLf)
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
    ''' DAA
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function DAA_Leggi(ByVal Piva As String,
                              ByVal Id_Agenda As Integer,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocCantina.DAA_Leggi"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StbSQL.Length = 0

            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            StbSQL.Append(" SELECT Agenda.PIVA, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Id_Mov AS Id_Mov_Contabile, Mov_Contabile.Data_Movimento, Mov_Contabile.Ora , " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Doc_Numero_Sin, Mov_Contabile.Doc_Numero,Mov_Contabile.Doc_Numero_Des, Mov_Contabile.Mov_Desc ,  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Scadenza, Mov_Contabile.Num_Protocollo, Mov_Contabile.Causale_Trasporto, Mov_Contabile.Aspetto, Mov_Contabile.Peso,  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Ora, Mov_Contabile.Colli, Mov_Contabile.Extra_Str, Mov_Contabile.Extra_Int, Mov_Contabile.Extra_Date,  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Tipo_Sconto, Mov_Contabile.Natura_Beni, Mov_Contabile.Tara_Veicolo, Mov_Contabile.Tara_Imballi, Mov_Contabile.Tipo_Peso, Mov_Contabile.Modalita, Mov_Contabile.Username_Note,  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Scadenza_Extra, Mov_Contabile.Progr_Protocollo, Mov_Contabile.Progr_Registrazione, Mov_Contabile.Data_Registrazione, Mov_Contabile.ChkLayOut_Join_Prodotti, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Contatti_Legale.Nome AS Nome_Legale, Contatti_Legale.Cognome AS Cognome_Legale, Contatti_Legale.Rag_Soc AS Rag_Soc_Legale, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Cod_RisUm, Contatti_Cliente.Cod_Contatto AS Cod_Contatto_Cliente, Contatti_Cliente.Rag_Soc AS Rag_Soc_Cliente, Contatti_Cliente.Codice_Fiscale AS Codice_Fiscale_Cliente,  " & vbCrLf)
            StbSQL.Append(" Contatti_Cliente.Id_Cf AS Id_Cf_Cliente, Contatti_Cliente.Nome AS Nome_Cliente, Contatti_Cliente.Cognome AS Cognome_Cliente,  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Cod_IndirizzoRisUm, Indirizzi_Cliente.ind_des AS ind_des_Cliente, ISNULL(Indirizzi_Cliente.frz_des, '') AS frz_des_Cliente, ISNULL(Indirizzi_Cliente.CAP, '') AS cap_Cliente,  " & vbCrLf)
            StbSQL.Append(" ISNULL(Indirizzi_Cliente.stato, '') AS stato_Cliente, Indirizzi_Cliente.pro_cod_istat AS pro_cod_istat_Cliente,  " & vbCrLf)
            StbSQL.Append(" Indirizzi_Cliente.com_cod_istat AS com_cod_istat_Cliente, Istat_Cliente.LOCALITA AS localita_Cliente, Istat_Cliente.COMUNI_PROV AS comuni_prov_Cliente, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Contatti_Codici.Val_Cod AS Codice_Accisa_Destinatario, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Cod_Destinazione, Contatti_DestDiv.Cod_Contatto AS Cod_Contatto_DestDiv, Contatti_DestDiv.Rag_Soc AS Rag_Soc_DestDiv, Contatti_DestDiv.Codice_Fiscale AS Codice_Fiscale_DestDiv,  " & vbCrLf)
            StbSQL.Append(" Contatti_DestDiv.Id_Cf AS Id_Cf_DestDiv, Contatti_DestDiv.Nome AS Nome_DestDiv, Contatti_DestDiv.Cognome AS Cognome_DestDiv,  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Cod_IndirizzoDestinazione,   Indirizzi_DestDiv.ind_des AS ind_des_DestDiv, Indirizzi_DestDiv.frz_des AS frz_des_DestDiv, Indirizzi_DestDiv.CAP AS cap_DestDiv,  " & vbCrLf)
            StbSQL.Append(" Indirizzi_DestDiv.stato AS stato_DestDiv, Indirizzi_DestDiv.pro_cod_istat AS pro_cod_istat_DestDiv,  " & vbCrLf)
            StbSQL.Append(" Indirizzi_DestDiv.com_cod_istat AS com_cod_istat_DestDiv, Istat_DestDiv.LOCALITA AS localita_DestDiv, Istat_DestDiv.COMUNI_PROV AS comuni_prov_DestDiv, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Mezzo, Mov_Contabile.Cod_Vettore, " & vbCrLf)
            StbSQL.Append(" ISNULL(Contatti_Vettore.Cod_Contatto, '') AS Cod_Contatto_Vettore, ISNULL(Contatti_Vettore.Rag_Soc, '') AS Rag_Soc_Vettore, ISNULL(Contatti_Vettore.Codice_Fiscale, '') AS Codice_Fiscale_Vettore,  " & vbCrLf)
            StbSQL.Append(" ISNULL(Contatti_Vettore.Id_Cf, 0) AS Id_Cf_Vettore, ISNULL(Contatti_Vettore.Nome, '') AS Nome_Vettore, ISNULL(Contatti_Vettore.Cognome, '') AS Cognome_Vettore,  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile.Cod_IndirizzoVettore, ISNULL(Indirizzi_Vettore.ind_des, '') AS ind_des_Vettore, ISNULL(Indirizzi_Vettore.frz_des, '') AS frz_des_Vettore, ISNULL(Indirizzi_Vettore.CAP, '') AS cap_Vettore,  " & vbCrLf)
            StbSQL.Append(" ISNULL(Indirizzi_Vettore.stato, '') AS stato_Vettore, Indirizzi_Vettore.pro_cod_istat AS pro_cod_istat_Vettore,  " & vbCrLf)
            StbSQL.Append(" Indirizzi_Vettore.com_cod_istat AS com_cod_istat_Vettore, ISNULL(Istat_Vettore.LOCALITA, '') AS localita_Vettore, ISNULL(Istat_Vettore.COMUNI_PROV, '') AS comuni_prov_Vettore,  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            'StbSQL.Append(" Parco_Macchine.Class_Code, Parco_Macchine.Mac_Des, Parco_Macchine.Targa, Parco_Macchine.Telaio, Parco_Macchine.Modello,  " + vbCrLf)
            'StbSQL.Append(" Parco_Macchine.Potenza, Parco_Macchine.Data_Immatricolazione, Parco_Macchine.N_Immatricolazione,  " + vbCrLf)
            'StbSQL.Append(" Parco_Macchine.N_Immatricolazione_Rimorchio, Parco_Macchine.N_Autorizzazione_Trasporto, Parco_Macchine.Data_Rilascio_Autorizzazione, Parco_Macchine.Peso, " + vbCrLf)
            'StbSQL.Append("  " + vbCrLf)
            StbSQL.Append(" Movimento_Extra.Mac_Cod, Movimento_Extra.Mezzo_Trasporto, Movimento_Extra.Targa AS Targa_2, Movimento_Extra.N_Immatricolazione AS N_Immatricolazione_2, Movimento_Extra.N_Immatricolazione_Rimorchio AS N_Immatricolazione_Rimorchio_2, " & vbCrLf)
            StbSQL.Append(" Movimento_Extra.N_Autorizzazione_Trasporto AS N_Autorizzazione_Trasporto_2, Movimento_Extra.Data_Rilascio_Autorizzazione AS Data_Rilascio_Autorizzazione_2, Movimento_Extra.Peso AS Peso_2, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Movimento_Extra.Id_Reg_Dettaglio AS Id_Movimento_Extra, Movimento_Extra.Luogo_Partenza, Movimento_Extra.Luogo_Consegna, Movimento_Extra.Data_Spedizione, " & vbCrLf)
            StbSQL.Append(" Movimento_Extra.Num_Riferimento, Movimento_Extra.Data_Dichiarazione, Movimento_Extra.Garanzia,  " & vbCrLf)
            StbSQL.Append(" Movimento_Extra.Certificati, Movimento_Extra.Durata_Viaggio, ISNULL(Movimento_Extra.Codice_Alternativo, '') AS Codice_Alternativo,  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Mov_Magazzino.Id_Mov AS Id_Mov_Magazzino, Mov_Magazzino.Mov_Desc AS Mov_Desc_Mag, Mov_Magazzino.Data_Movimento AS Data_Mag,Mov_Magazzino.Ora AS Ora_Mag,  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Dettagli_Extra.Id_Reg_Dettaglio AS Id_Dettagli_Extra, ISNULL(Dettagli_Extra.Marche_Contenitori, '') AS Marche_Contenitori,  " & vbCrLf)
            'StbSQL.Append(" --Dettagli_Extra.Codice_Prodotto, Dettagli_Extra.Codice_NC, Dettagli_Extra.Colore, Dettagli_Extra.Manipolazioni, Dettagli_Extra.Zona_Viticola, " + vbCrLf)
            StbSQL.Append(" ISNULL(Dettagli_Extra.Num_Contenitori, 0) AS Num_Contenitori, ISNULL(Dettagli_Extra.Des_Contenitori, '') AS Des_Contenitori, Dettagli_Extra.Precisazioni,  " & vbCrLf)
            StbSQL.Append(" Dettagli_Extra.Titolo_Alcol, Dettagli_Extra.Peso_Lordo, ISNULL(Dettagli_Extra.Num_Colli, 0) AS Num_Colli, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Id_Mov_Det, Mov_Dett_Magazzino.Mov_Det_Des, Mov_Dett_Magazzino.Elem_Cod, Mov_Dett_Magazzino.Pro_Cod, Mov_Dett_Magazzino.Mat_Cod,  " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Cod_Progetto, Mov_Dett_Magazzino.Fase_Cod, Mov_Dett_Magazzino.Lotto, Mov_Dett_Magazzino.Cal_Cod, Mov_Dett_Magazzino.Udm_Cod, UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim,  " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Qta AS Qta, Mov_Dett_Magazzino.Variazione AS Variazione,Mov_Dett_Magazzino.Listino_Cod, Mov_Dett_Magazzino.Tara,  " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Prezzo_Unitario, Mov_Dett_Magazzino.Prezzo_Unitario_Netto, Mov_Dett_Magazzino.Imponibile, Mov_Dett_Magazzino.Imponibile_Netto,  " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Jolly_Int, Mov_Dett_Magazzino.Contabilizzato,Mov_Dett_Magazzino.Pendente, Mov_Dett_Magazzino.Prezzo_Effettivo, " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Cod_Iva, Mov_Dett_Magazzino.Sconto, Mov_Dett_Magazzino.Cod_Conto, Mov_Dett_Magazzino.Extra_Str, Mov_Dett_Magazzino.Extra_Int, " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Extra_Date, Mov_Dett_Magazzino.Anno, Mov_Dett_Magazzino.Ric_Cod, Mov_Dett_Magazzino.Iva, Mov_Dett_Magazzino.UDM_COD_EXTRA, Mov_Dett_Magazzino.QTA_EXTRA,  " & vbCrLf)
            StbSQL.Append(" Mov_Dett_Magazzino.Cod_IvaIndetraibile, Mov_Dett_Magazzino.Qta_Extra_Totale, Mov_Dett_Magazzino.Tara, Mov_Dett_Magazzino.ChkLayOut_Hide, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Materie_Prime.codice_prodotto, Materie_Prime.colore, Materie_Prime.codice_nc, Materie_Prime.manipolazioni, " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" Mov_Contabile_Fattura.Data_Movimento AS Data_Fattura, Mov_Contabile_Fattura.Doc_Numero_Sin AS Doc_Numero_Sin_Fattura, " & vbCrLf)
            StbSQL.Append(" Mov_Contabile_Fattura.Doc_Numero AS Doc_Numero_Fattura, Mov_Contabile_Fattura.Doc_Numero_Des AS Doc_Numero_Des_Fattura " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            StbSQL.Append(" FROM Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Contabile ON Agenda.PIVA = Mov_Contabile.PIVA AND Agenda.Sa_Cod = Mov_Contabile.Sa_Cod AND Agenda.Id_Agenda = Mov_Contabile.Id_Agenda  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Cliente ON Mov_Contabile.Cod_RisUm = Risorse_Umane_Cliente.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Cliente ON Risorse_Umane_Cliente.Piva = Contatti_Cliente.Piva AND Risorse_Umane_Cliente.Cod_Contatto = Contatti_Cliente.Cod_Contatto  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Indirizzi Indirizzi_Cliente ON  Mov_Contabile.Cod_IndirizzoRisUm = Indirizzi_Cliente.cod_indirizzo " & vbCrLf)
            StbSQL.Append(" INNER JOIN ISTAT Istat_Cliente ON Indirizzi_Cliente.pro_cod_istat = Istat_Cliente.PROV AND Indirizzi_Cliente.com_cod_istat = Istat_Cliente.COM " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti_Codici ON Contatti_Codici.Piva = Contatti_Cliente.Piva AND Contatti_Codici.Cod_Contatto = Contatti_Cliente.Cod_Contatto  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_DestDiv ON Mov_Contabile.Cod_Destinazione = Risorse_Umane_DestDiv.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Contatti Contatti_DestDiv ON Risorse_Umane_DestDiv.Piva = Contatti_DestDiv.Piva AND Risorse_Umane_DestDiv.Cod_Contatto = Contatti_DestDiv.Cod_Contatto  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Indirizzi Indirizzi_DestDiv ON  Mov_Contabile.Cod_IndirizzoDestinazione= Indirizzi_DestDiv.cod_indirizzo " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN ISTAT Istat_DestDiv ON Indirizzi_DestDiv.pro_cod_istat = Istat_DestDiv.PROV AND Indirizzi_DestDiv.com_cod_istat = Istat_DestDiv.COM " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Vettore ON Mov_Contabile.Cod_Vettore = Risorse_Umane_Vettore.Cod_RisUm  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Contatti Contatti_Vettore ON Risorse_Umane_Vettore.Piva = Contatti_Vettore.Piva AND Risorse_Umane_Vettore.Cod_Contatto = Contatti_Vettore.Cod_Contatto  " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN Indirizzi Indirizzi_Vettore ON  Mov_Contabile.Cod_IndirizzoVettore = Indirizzi_Vettore.cod_indirizzo " & vbCrLf)
            StbSQL.Append(" LEFT OUTER JOIN ISTAT Istat_Vettore ON Indirizzi_Vettore.pro_cod_istat = Istat_Vettore.PROV AND Indirizzi_Vettore.com_cod_istat = Istat_Vettore.COM " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Risorse_Umane Risorse_Umane_Legale ON Mov_Contabile.Piva = Risorse_Umane_Legale.Piva  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Contatti Contatti_Legale ON Risorse_Umane_Legale.Piva = Contatti_Legale.Piva AND Risorse_Umane_Legale.Cod_Contatto = Contatti_Legale.Cod_Contatto  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra ON Movimento_Extra.PIVA = Mov_Contabile.PIVA AND Movimento_Extra.Sa_Cod = Mov_Contabile.Sa_Cod AND Movimento_Extra.Id_Agenda = Mov_Contabile.Id_Agenda AND Movimento_Extra.Id_Mov = Mov_Contabile.Id_Mov " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            'StbSQL.Append(" LEFT OUTER JOIN Parco_Macchine ON Movimento_Extra.Piva = Parco_Macchine.Piva AND Movimento_Extra.Mac_Cod = Parco_Macchine.Mac_Cod " + vbCrLf)
            'StbSQL.Append("  " + vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Sa_Cod = Mov_Magazzino.Sa_Cod AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod AND Materie_Prime.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod   " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Dettagli_Extra ON Dettagli_Extra.PIVA = Mov_Dett_Magazzino.PIVA AND Dettagli_Extra.Sa_Cod = Mov_Dett_Magazzino.Sa_Cod AND Dettagli_Extra.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Dettagli_Extra.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Dettagli_Extra.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Mov_Dettagli_Riferimenti ON Mov_Dettagli_Riferimenti.PIVA = Mov_Dett_Magazzino.PIVA " & vbCrLf)
            StbSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Dettagli_Riferimenti.Id_Mov = Mov_Dett_Magazzino.Id_Mov  " & vbCrLf)
            StbSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Mov_det = Mov_Dett_Magazzino.Id_Mov_det " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Contabile_Fattura  ON Mov_Dettagli_Riferimenti.PIVA_rif = Mov_Contabile_Fattura.PIVA " & vbCrLf)
            StbSQL.Append(" AND Mov_Dettagli_Riferimenti.Sa_Cod_Rif = Mov_Contabile_Fattura.Sa_Cod  " & vbCrLf)
            StbSQL.Append(" AND Mov_Dettagli_Riferimenti.Id_Agenda_Rif = Mov_Contabile_Fattura.Id_Agenda  " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            StbSQL.Append(" WHERE Agenda.Lav_Cod = " & CStr(LAVCOD_DAA_EMESSO) & " " & vbCrLf)

            If Piva <> "" Then
                StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            End If

            If Id_Agenda <> 0 Then
                StbSQL.Append(" AND Agenda.Id_Agenda = " & CStr(Id_Agenda) & " " & vbCrLf)
            End If

            StbSQL.Append(" AND Mov_Contabile.Cau_Mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)
            StbSQL.Append(" AND Mov_Magazzino.Cau_Mov = '" & CAU_SCARICO & "' " & vbCrLf)
            StbSQL.Append(" AND Risorse_Umane_Legale.Cod_Rapporto = " & CStr(COD_LEGALE) & " " & vbCrLf)
            StbSQL.Append(" AND Contatti_Codici.id_cod = " & CStr(4003) & " " & vbCrLf)
            StbSQL.Append(" AND Mov_Contabile_Fattura.Cau_Mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" ORDER BY Mov_Dett_Magazzino.Id_Mov_Det " & vbCrLf)
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

End Class


'#####################
Public Class VerificaDBxCantine
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' movimenti di condizionati in cui la qta_extra è diversa da quella nominale (anagrafica materie prime)
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Condizionati_QtaExtra_NominaleEffettivaDiversa(ByVal Piva As String,
                                                                   ByVal xFiltroAggiuntivo As String,
                                                                   ByVal xOrderBy As String,
                                                                   ByRef objParametri As AgronicaCoreParametri
                                                                   ) As DataTable

        Const nomeRoutine As String = "AgronicaCoreStampeDAL.VerificaDBxCantine.Condizionati_QtaExtra_NominaleEffettivaDiversa"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.Append(" -- movimenti di condizionati in cui la qta_extra è diversa da quella nominale (anagrafica materie prime) " & vbCrLf)
            StbSQL.Append(" SELECT Agenda.PIVA, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, Mov_Magazzino.data_movimento, " & vbCrLf)
            StbSQL.Append(" Movimenti_dettagli.UDM_COD_EXTRA, MMovimenti_dettagli.QTA_EXTRA as qta_extra_dettagli, Materie_Prime.QTA_EXTRA as qta_extra_mp, Qta,Movimenti_dettagli.Udm_Cod,  " & vbCrLf)
            StbSQL.Append(" Movimenti_dettagli.mat_cod, mat_des " & vbCrLf)

            StbSQL.Append(" FROM Agenda  " & vbCrLf)
            'StbSQL.Append(" INNER JOIN Movimenti Mov_Contabile ON Agenda.PIVA = Mov_Contabile.PIVA AND Agenda.Sa_Cod = Mov_Contabile.Sa_Cod AND Agenda.Id_Agenda = Mov_Contabile.Id_Agenda  " + vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA  AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod AND Materie_Prime.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod   " & vbCrLf)
            StbSQL.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod " & vbCrLf)

            StbSQL.Append(" WHERE Movimenti_dettagli.elem_cod= " & Agro_SQL_SaveNum(TRASFORMATI_VEGETALI) & "" & vbCrLf)
            StbSQL.Append(" AND Movimenti_dettagli.udm_cod= " & Agro_SQL_SaveNum(enum_UnitaMisura.Litri) & "" & vbCrLf)

            If Piva <> "" Then
                StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            End If

            StbSQL.Append(" AND (Movimenti_dettagli.udm_cod_extra =0 OR Movimenti_dettagli.QTA_EXTRA=0) " & vbCrLf)
            StbSQL.Append(" AND peso_Set=1 " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)

            ' StbSQL.Append(" AND Mov_Contabile.Cau_Mov = '" + CAU_REGISTRAZIONI + "' " + vbCrLf)

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" -- ORDER BY  " & vbCrLf)
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
    ''' movimenti (sbagliati) di condizionati dove qta_extra o udm_cod_extra sono nulle
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Condizionati_QtaExtra_UdmExtra_Sbagliata(ByVal Piva As String,
                                                             ByVal xFiltroAggiuntivo As String,
                                                             ByVal xOrderBy As String,
                                                             ByRef objParametri As AgronicaCoreParametri
                                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.VerificaDBxCantine.Condizionati_QtaUdmSbagliata"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.Append(" --movimenti (sbagliati) di condizionati dove qta_extra o udm_cod_extra sono nulle " & vbCrLf)
            StbSQL.Append(" SELECT Agenda.PIVA, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, Mov_Magazzino.data_movimento " & vbCrLf)
            StbSQL.Append(" Movimenti_dettagli.UDM_COD_EXTRA,Movimenti_dettagli.QTA_EXTRA,Qta,Movimenti_dettagli.Udm_Cod,  " & vbCrLf)
            StbSQL.Append(" Movimenti_dettagli.mat_cod, mat_des " & vbCrLf)

            StbSQL.Append(" FROM Agenda  " & vbCrLf)
            'StbSQL.Append(" INNER JOIN Movimenti Mov_Contabile ON Agenda.PIVA = Mov_Contabile.PIVA AND Agenda.Sa_Cod = Mov_Contabile.Sa_Cod AND Agenda.Id_Agenda = Mov_Contabile.Id_Agenda  " + vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA  AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov  " & vbCrLf)
            StbSQL.Append(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod AND Materie_Prime.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod   " & vbCrLf)
            StbSQL.Append(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod " & vbCrLf)

            StbSQL.Append(" WHERE Movimenti_dettagli.elem_cod= " & Agro_SQL_SaveNum(TRASFORMATI_VEGETALI) & "" & vbCrLf)
            StbSQL.Append(" AND Movimenti_dettagli.udm_cod= " & Agro_SQL_SaveNum(enum_UnitaMisura.Litri) & "" & vbCrLf)

            If Piva <> "" Then
                StbSQL.Append(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & vbCrLf)
            End If

            StbSQL.Append(" AND (Movimenti_dettagli.udm_cod_extra =0 OR Movimenti_dettagli.QTA_EXTRA=0) " & vbCrLf)
            StbSQL.Append(" AND peso_Set=1 " & vbCrLf)
            StbSQL.Append("  " & vbCrLf)

            ' StbSQL.Append(" AND Mov_Contabile.Cau_Mov = '" + CAU_REGISTRAZIONI + "' " + vbCrLf)

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.Append(" -- ORDER BY  " & vbCrLf)
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

End Class
