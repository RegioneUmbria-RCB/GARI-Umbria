Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json

Public Class FreshAndFood
    Inherits AgronicaCoreDataProvider.DataProvider

    Private arrLavCodAccettazioniConf As Integer() = New Integer() {LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE} 'LAVCOD_DISTINTA_CARICO, LAVCOD_AUTO_DDT_EMESSO Non usate per i conferimenti

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' stampa del report della bolla F&F by giaslan 
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function StampaBolla(ByVal Piva As String,
                                ByVal Lav_Cod As Integer,
                                ByVal Id_Agenda As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal NomeDBUtenti As String,
                                ByVal aggregaRighe As Boolean,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.FreshAndFood.StampaBolla"

        Dim messaggioErrore As String = ""
        Dim StbSQL As New StringBuilder
        Dim dt As DataTable
        Dim Flag_Accettazione As Boolean
        Dim Flag_Carico As Boolean

        Try

            Select Case Lav_Cod
                Case LAVCOD_ACCETTAZIONE_DIVERSI,
                    LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
                    LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                    Flag_Accettazione = True
                Case Else
                    Flag_Accettazione = False
            End Select

            'nel caso del carico NON ci sono tutta una serie di informazioni
            '(bolla di conferimento collegata, conferente, ecc...)
            Select Case Lav_Cod
                Case LAVCOD_DISTINTA_CARICO,
                    LAVCOD_DISTINTA_CARICO_ACCETTAZIONE
                    Flag_Carico = True
                Case Else
                    Flag_Carico = False
            End Select

            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT  Agenda.username_creazione, UD.nome, UD.cognome, Agenda.piva ,Agenda.des_lib ,Agenda.id_agenda, Agenda.lav_cod, ")
            StbSQL.AppendLine("         Mov_Accett.ChkLayout_Join_Prodotti, Mov_Accett.Peso, Mov_Accett.Extra_Str, Mov_Accett.Natura_Beni, ")
            StbSQL.AppendLine("         Mov_Accett.Tara_Veicolo, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso,  ")
            StbSQL.AppendLine("         Mov_Accett.Data_Movimento AS Data_Accett, Mov_Accett.Ora AS Ora_Accett, ")
            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero,Mov_Accett.Doc_Numero_Des, Mov_Accett.Mov_Desc AS Note, ")

            'StbSQL.AppendLine("         -- Sequenza_Progressivi.Lunghezza_Sin, Sequenza_Progressivi.Lunghezza_Centro, Sequenza_Progressivi.Lunghezza_Des, Sequenza_Progressivi.CarattereFormattazione, ")

            StbSQL.AppendLine("         Mov_Accett.Cod_RisUm,")
            'If Flag_Carico = False Then
            StbSQL.AppendLine("         Risorse_Umane_Conferenti.Cod_Rapporto AS Cod_Rapporto_Conferente, Contatti_Conferenti.Cod_Contatto AS Cod_Contatto_Conferente,  ")
            StbSQL.AppendLine("         Contatti_Conferenti.Rag_Soc AS Rag_Soc_Conferente, ISNULL(Contatti_Conferenti.Codice_Fiscale, '') AS Codice_Fiscale_Conferente, ")
            StbSQL.AppendLine("         Contatti_Conferenti.Nome AS Nome_Conferente, Contatti_Conferenti.Cognome AS Cognome_Conferente,  ")
            StbSQL.AppendLine("         Mov_Accett.Cod_IndirizzoRisUm,   Indirizzi_Conferenti.ind_des AS ind_des_Conferente, Indirizzi_Conferenti.frz_des AS frz_des_Conferente, Indirizzi_Conferenti.CAP AS cap_Conferente,  ")
            StbSQL.AppendLine("         Indirizzi_Conferenti.stato AS stato_Conferente, Indirizzi_Conferenti.pro_cod_istat AS pro_cod_istat_Conferente,  ")
            StbSQL.AppendLine("         Indirizzi_Conferenti.com_cod_istat AS com_cod_istat_Conferente, Istat_Conferenti.LOCALITA AS localita_Conferente, Istat_Conferenti.COMUNI_PROV AS comuni_prov_Conferente, ")
            'Else
            '    StbSQL.AppendLine("         0 AS Cod_Rapporto_Conferente, '' AS Cod_Contatto_Conferente,  ")
            '    StbSQL.AppendLine("         '' AS Rag_Soc_Conferente, '' AS Codice_Fiscale_Conferente, ")
            '    StbSQL.AppendLine("         '' AS Nome_Conferente, '' AS Cognome_Conferente,  ")
            '    StbSQL.AppendLine("         0 AS Cod_IndirizzoRisUm,   '' AS ind_des_Conferente, '' AS frz_des_Conferente, '' AS cap_Conferente,  ")
            '    StbSQL.AppendLine("         '' AS stato_Conferente, '' AS pro_cod_istat_Conferente,  ")
            '    StbSQL.AppendLine("         '' AS com_cod_istat_Conferente, '' AS localita_Conferente, '' AS comuni_prov_Conferente, ")
            'End If
            StbSQL.AppendLine("         Risorse_Umane_Conferenti.Settore_des as Codice_Conferente , ")
            StbSQL.AppendLine("         Mov_Accett.Cod_RisUm_Altro,  ")
            'StbSQL.AppendLine("         Risorse_Umane_Coop.Cod_Rapporto AS Cod_Rapporto_Coop, Contatti_Coop.Cod_Contatto AS Cod_Contatto_Coop,  ")
            'StbSQL.AppendLine("         Contatti_Coop.Rag_Soc AS Rag_Soc_Coop, ISNULL(Contatti_Coop.Codice_Fiscale, '') AS Codice_Fiscale_Coop,  ")
            'StbSQL.AppendLine("         Contatti_Coop.Nome AS Nome_Coop, Contatti_Coop.Cognome AS Cognome_Coop,  ")
            'StbSQL.AppendLine("         ImpreseXIndirizzi_Coop.Cod_Indirizzo AS Cod_Indirizzo_Coop, Indirizzi_Coop.ind_des AS ind_des_Coop, Indirizzi_Coop.frz_des AS frz_des_Coop, Indirizzi_Coop.CAP AS cap_Coop, ")
            'StbSQL.AppendLine("         Indirizzi_Coop.stato AS stato_Coop, Indirizzi_Coop.pro_cod_istat AS pro_cod_istat_Coop,  ")
            'StbSQL.AppendLine("         Indirizzi_Coop.com_cod_istat AS com_cod_istat_Coop, Istat_Coop.LOCALITA AS localita_Coop, Istat_Coop.COMUNI_PROV AS comuni_prov_Coop, ")
            StbSQL.AppendLine("         Mov_Accett.Extra_Int,  ")
            'StbSQL.AppendLine("         Risorse_Umane_Coop_2.Cod_Rapporto AS Cod_Rapporto_Coop_2, Contatti_Coop_2.Cod_Contatto AS Cod_Contatto_Coop_2,  ")
            'StbSQL.AppendLine("         Contatti_Coop_2.Rag_Soc AS Rag_Soc_Coop_2, ISNULL(Contatti_Coop_2.Codice_Fiscale, '') AS Codice_Fiscale_Coop_2,  ")
            'StbSQL.AppendLine("         Contatti_Coop_2.Nome AS Nome_Coop_2, Contatti_Coop_2.Cognome AS Cognome_Coop_2,  ")
            'StbSQL.AppendLine("         ImpreseXIndirizzi_Coop_2.Cod_Indirizzo AS Cod_Indirizzo_Coop_2, Indirizzi_Coop_2.ind_des AS ind_des_Coop_2, Indirizzi_Coop_2.frz_des AS frz_des_Coop_2, Indirizzi_Coop_2.CAP AS cap_Coop_2, ")
            'StbSQL.AppendLine("         Indirizzi_Coop_2.stato AS stato_Coop_2, Indirizzi_Coop_2.pro_cod_istat AS pro_cod_istat_Coop_2,  ")
            'StbSQL.AppendLine("         Indirizzi_Coop_2.com_cod_istat AS com_cod_istat_Coop_2, Istat_Coop_2.LOCALITA AS localita_Coop_2, Istat_Coop_2.COMUNI_PROV AS comuni_prov_Coop_2, ")
            StbSQL.AppendLine("         Mov_Accett.Cod_Destinazione, ")
            'StbSQL.AppendLine("         Risorse_Umane_Produttori.Cod_Rapporto AS Cod_Rapporto_Produttore, Contatti_Produttori.Cod_Contatto AS Cod_Contatto_Produttore,  ")
            'StbSQL.AppendLine("         Contatti_Produttori.Rag_Soc AS Rag_Soc_Produttore, ISNULL(Contatti_Produttori.Codice_Fiscale, '') AS Codice_Fiscale_Produttore,  ")
            'StbSQL.AppendLine("         Contatti_Produttori.Nome AS Nome_Produttore, Contatti_Produttori.Cognome AS Cognome_Produttore,  ")
            'StbSQL.AppendLine("         Mov_Accett.Cod_IndirizzoDestinazione, Indirizzi_Produttori.ind_des AS ind_des_Produttore, Indirizzi_Produttori.frz_des AS frz_des_Produttore, Indirizzi_Produttori.CAP AS cap_Produttore,  ")
            'StbSQL.AppendLine("         Indirizzi_Produttori.stato AS stato_Produttore, Indirizzi_Produttori.pro_cod_istat AS pro_cod_istat_Produttore,  ")
            'StbSQL.AppendLine("         Indirizzi_Produttori.com_cod_istat AS com_cod_istat_Produttore, Istat_Produttori.LOCALITA AS localita_Produttore, Istat_Produttori.COMUNI_PROV AS comuni_prov_Produttore, ")
            StbSQL.AppendLine("         Mov_Accett.causale_trasporto, Mov_Accett.aspetto, Mov_Accett.natura_beni, ")
            StbSQL.AppendLine("         Mov_Accett.Mezzo, Mov_Accett.Cod_Vettore, Mov_Accett.Cod_IndirizzoVettore,  ")
            StbSQL.AppendLine("         Movimento_Extra_0.Targa as TargaMezzoVettore, ")
            If Not Flag_Carico Then
                StbSQL.AppendLine("         Risorse_Umane_Vettore.Cod_Rapporto AS Cod_Rapporto_Produttore, Contatti_Vettore.Cod_Contatto AS Cod_Contatto_Vettore,  ")
                StbSQL.AppendLine("         Contatti_Vettore.Rag_Soc AS Rag_Soc_Vettore, ISNULL(Contatti_Vettore.Codice_Fiscale, '') AS Codice_Fiscale_Vettore,  ")
                StbSQL.AppendLine("         Contatti_Vettore.Nome AS Nome_Vettore, Contatti_Vettore.Cognome AS Cognome_Vettore,  ")
                StbSQL.AppendLine("         Mov_Accett.Cod_IndirizzoVettore, Indirizzi_Vettore.ind_des AS ind_des_Vettore, Indirizzi_Vettore.frz_des AS frz_des_Vettore, Indirizzi_Vettore.CAP AS cap_Vettore,  ")
                StbSQL.AppendLine("         Indirizzi_Vettore.stato AS stato_Vettore, Indirizzi_Vettore.pro_cod_istat AS pro_cod_istat_Vettore,  ")
                StbSQL.AppendLine("         Indirizzi_Vettore.com_cod_istat AS com_cod_istat_Vettore, Istat_Vettore.LOCALITA AS localita_Vettore, Istat_Vettore.COMUNI_PROV AS comuni_prov_Vettore, ")
            Else
                StbSQL.AppendLine("         0 AS Cod_Rapporto_Produttore, '' AS Cod_Contatto_Vettore,  ")
                StbSQL.AppendLine("         '' AS Rag_Soc_Vettore, '' AS Codice_Fiscale_Vettore,  ")
                StbSQL.AppendLine("         '' AS Nome_Vettore, '' AS Cognome_Vettore,  ")
                StbSQL.AppendLine("         0 AS Cod_IndirizzoVettore, '' AS ind_des_Vettore, '' AS frz_des_Vettore, '' AS cap_Vettore,  ")
                StbSQL.AppendLine("         '' AS stato_Vettore, '' AS pro_cod_istat_Vettore,  ")
                StbSQL.AppendLine("         '' AS com_cod_istat_Vettore, '' AS localita_Vettore, '' AS comuni_prov_Vettore, ")
            End If

            StbSQL.AppendLine("")
            If Not Flag_Carico Then
                StbSQL.AppendLine("         Mov_Conf.Id_Mov AS Id_Mov_Conf, Mov_Conf.Cau_Mov AS Cau_Mov_Conf, Mov_Conf.Mov_Desc AS Mov_Desc_Conf,  ")
                StbSQL.AppendLine("         Mov_Conf.Data_Movimento AS Data_Conf, Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf,  ")
                StbSQL.AppendLine("         Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf,  Mov_Conf.Colli, ")
            Else
                StbSQL.AppendLine("         0 AS Id_Mov_Conf, '' AS Cau_Mov_Conf, '' AS Mov_Desc_Conf,  ")
                StbSQL.AppendLine("         '01/01/1900' AS Data_Conf, '' AS Doc_Numero_Sin_Conf, 0 AS Doc_Numero_Conf,  ")
                StbSQL.AppendLine("         '' AS Doc_Numero_Des_Conf,  0 AS Colli, ")
            End If

            StbSQL.AppendLine("")
            StbSQL.AppendLine("         Mov_Raccolta.Id_Mov AS Id_Mov_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Raccolta.Cau_Mov AS Cau_Mov_Raccolta, Mov_Raccolta.Mov_Desc AS Mov_Desc_Raccolta, Mov_Raccolta.Data_Movimento AS Data_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Raccolta.ora AS DataOra_Ingresso,  ")
            'StbSQL.AppendLine("         -- C_Az_Raccolta.sa_cod AS Sa_Cod_Raccolta, C_Az_Raccolta.sa_nome AS Sa_Nome_Raccolta,   ")
            'StbSQL.AppendLine("         -- Mov_Dest_Raccolta.Id_Destinazione AS Id_Destinazione_Raccolta, Cantina_Vasche.Identificativo AS Id_vasca, ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Id_Mov_Det AS Id_Mov_Det_Raccolta, Mov_Dett_Raccolta.sa_cod AS Sa_cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Mov_Det_Des AS Mov_Det_Des_Raccolta, Mov_Dett_Raccolta.Elem_Cod AS Elem_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Pro_Cod AS Pro_Cod_Raccolta, Mov_Dett_Raccolta.Mat_Cod AS Mat_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Cod_Progetto AS Cod_Progetto_Raccolta, Mov_Dett_Raccolta.Fase_Cod AS Fase_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Qta, Mov_Dett_Raccolta.Tara AS Tara,    ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Qta_Extra, Mov_Dett_Raccolta.Udm_Cod_Extra, Mov_Dett_Raccolta.Qta_Extra_totale, Mov_Dett_Raccolta.Tara, ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Qta_Dettaglio1 AS Qta_sottoconfezioni, Mov_Dett_Raccolta.Qta_Dettaglio2 AS Qta_Imballaggi, ")

            StbSQL.AppendLine("         UDM_Qta.Udm_Sim AS Udm_Sim_Raccolta, UDM_Qta.Udm_des AS Udm_des_Raccolta, ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta, Mov_Dett_Raccolta.Listino_Cod AS Listino_Cod_Raccolta, Mov_Dett_Raccolta.Prezzo_Unitario,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Prezzo_Unitario_Netto, Mov_Dett_Raccolta.Imponibile, Mov_Dett_Raccolta.Imponibile_Netto,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Jolly_Int AS Jolly_Int_Raccolta, Mov_Dett_Raccolta.Contabilizzato AS Contabilizzato_Raccolta,  ")
            StbSQL.AppendLine("         -- Mov_Dett_Raccolta.Pendente AS Pendente_Raccolta, Mov_Dett_Raccolta.Prezzo_Effettivo, Mov_Dett_Raccolta.ChkLayOut_Hide, ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.extra_str as PrgRiga, ")
            StbSQL.AppendLine("         MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta,  ")
            StbSQL.AppendLine("         MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, MP_Raccolta.Cul_Cod, MP_Raccolta.Veg_Cod, MP_Raccolta.GRVA_COD_VEG,  ")
            StbSQL.AppendLine("         MP_Raccolta.Regolamento, MP_Raccolta.ChkListino, ")

            StbSQL.AppendLine(" Movimento_Extra_0.Agente_Cod ")

            '22/05/2017 MAGA: si leggono dopo, non più in questa query,
            'non va bene leggerle da qui perché non tiene conto della configurazione del veg_cod
            'e non è dinamico, ovvero se si aggiungono nuove tabelle, la query rimane disallineata fino a nostro intervento
            'OTabelle_Parametri(StbSQL, "ocalibro", enum_OTabelle.Calibro)
            'OTabelle_Parametri(StbSQL, "oqualità", enum_OTabelle.Qualita)
            'OTabelle_Parametri(StbSQL, "odeclassamento", enum_OTabelle.Declassamento)
            'OTabelle_Parametri(StbSQL, "omarca", enum_OTabelle.Marca)
            'OTabelle_Parametri(StbSQL, "oconfezione", enum_OTabelle.Confezione)
            'OTabelle_Parametri(StbSQL, "ocontenitore", enum_OTabelle.Contenitore)
            'OTabelle_Parametri(StbSQL, "oimballaggio", enum_OTabelle.Imballaggio)

            'Contatti(StbSQL, "ocliente")
            'Contatti(StbSQL, "ofornitore")
            StbSQL_Note(StbSQL)

            StbSQL.AppendLine("")
            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            StbSQL.AppendLine("")
            'StbSQL.AppendLine(" -- INNER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Accett.PIVA  ")
            'StbSQL.AppendLine(" -- AND Sequenza_Progressivi.Anno = YEAR(Mov_Accett.Data_Movimento)  ")
            'StbSQL.AppendLine(" -- AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Accett.Doc_Numero_Sin  ")
            'StbSQL.AppendLine(" -- AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Accett.Doc_Numero_Des  ")

            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")
            StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Conferenti ON  Mov_Accett.Cod_IndirizzoRisUm = Indirizzi_Conferenti.cod_indirizzo ")
            StbSQL.AppendLine(" INNER JOIN ISTAT Istat_Conferenti ON Indirizzi_Conferenti.pro_cod_istat = Istat_Conferenti.PROV AND Indirizzi_Conferenti.com_cod_istat = Istat_Conferenti.COM ")

            If Not Flag_Carico Then

                StbSQL.AppendLine("")
                StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Vettore ON Mov_Accett.Cod_Vettore = Risorse_Umane_Vettore.Cod_RisUm  ")
                StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Vettore ON Risorse_Umane_Vettore.Piva = Contatti_Vettore.Piva AND Risorse_Umane_Vettore.Cod_Contatto = Contatti_Vettore.Cod_Contatto  ")
                StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Vettore ON  Mov_Accett.Cod_IndirizzoVettore = Indirizzi_Vettore.cod_indirizzo ")
                StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Vettore ON Indirizzi_Vettore.pro_cod_istat = Istat_Vettore.PROV AND Indirizzi_Vettore.com_cod_istat = Istat_Vettore.COM ")

                StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda ")

                'StbSQL.AppendLine("")
                'StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop.Cod_RisUm  ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Coop ON Risorse_Umane_Coop.Piva = Contatti_Coop.Piva AND Risorse_Umane_Coop.Cod_Contatto = Contatti_Coop.Cod_Contatto  ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN ImpreseXIndirizzi ImpreseXIndirizzi_Coop ON  Contatti_Coop.Cod_Contatto = ImpreseXIndirizzi_Coop.Piva ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Coop ON  ImpreseXIndirizzi_Coop.Cod_Indirizzo = Indirizzi_Coop.cod_indirizzo ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Coop ON Indirizzi_Coop.pro_cod_istat = Istat_Coop.PROV AND Indirizzi_Coop.com_cod_istat = Istat_Coop.COM ")
                'StbSQL.AppendLine("")
                'StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop_2 ON Mov_Accett.Extra_Int = Risorse_Umane_Coop_2.Cod_RisUm  ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Coop_2 ON Risorse_Umane_Coop_2.Piva = Contatti_Coop_2.Piva AND Risorse_Umane_Coop_2.Cod_Contatto = Contatti_Coop_2.Cod_Contatto  ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN ImpreseXIndirizzi ImpreseXIndirizzi_Coop_2 ON  Contatti_Coop_2.Cod_Contatto = ImpreseXIndirizzi_Coop_2.Piva ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Coop_2 ON  ImpreseXIndirizzi_Coop_2.Cod_Indirizzo = Indirizzi_Coop_2.cod_indirizzo ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Coop_2 ON Indirizzi_Coop_2.pro_cod_istat = Istat_Coop_2.PROV AND Indirizzi_Coop_2.com_cod_istat = Istat_Coop_2.COM ")
                'StbSQL.AppendLine("")
                'StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Produttori ON  Mov_Accett.Cod_IndirizzoDestinazione = Indirizzi_Produttori.cod_indirizzo ")
                'StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Produttori ON Indirizzi_Produttori.pro_cod_istat = Istat_Produttori.PROV AND Indirizzi_Produttori.com_cod_istat = Istat_Produttori.COM ")
                'StbSQL.AppendLine("")
            End If
            StbSQL.AppendLine("")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda ")

            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")

            'leggo il movimento dettaglio di riepilogo, non c'è destinazione
            'StbSQL.AppendLine(" -- INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det ")
            'StbSQL.AppendLine(" -- INNER JOIN Centri_Aziendali C_Az_Raccolta ON Mov_Dett_Raccolta.PIVA = C_Az_Raccolta.PIVA AND Mov_Dett_Raccolta.Sa_Cod = C_Az_Raccolta.sa_cod ")
            'StbSQL.AppendLine(" -- INNER JOIN Cantina_Vasche ON Mov_Dest_Raccolta.Piva = Cantina_Vasche.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Cantina_Vasche.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Cantina_Vasche.vas_Cod   ")
            StbSQL.AppendLine("")
            StbSQL.AppendLine(" INNER JOIN UnitaMisura UDM_Qta ON Mov_Dett_Raccolta.Udm_Cod = UDM_Qta.Udm_Cod ")
            StbSQL.AppendLine("")

            StbSQL.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra_0 ON Movimento_Extra_0.PIVA = Agenda.PIVA AND Movimento_Extra_0.Sa_Cod = Agenda.Sa_Cod AND Movimento_Extra_0.Id_Agenda = Agenda.Id_Agenda AND Movimento_Extra_0.Id_Mov_Det=0 ")

            StbSQL.AppendLine(" INNER JOIN " & NomeDBUtenti & ".dbo.Utenti_Dettagli UD on  UD.CodFisc = agenda.username_Creazione ")

            StbSQL.AppendLine("")
            StbSQL.AppendLine("")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione = " & Agro_SQL_SaveNum(CStr(enum_Accettazione_Tipo.POSTcampionatura)) & "")

            '13/01/2021 Giulia: il modulo non viene più memorizzato in Agenda.Modulo
            'StbSQL.AppendLine(" AND Agenda.Modulo = " & Agro_SQL_SaveNum(CStr(enum_Omni_Modulo_Generazione.FreshFood)) & "")

            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov      = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'")
            If Not Flag_Carico Then
                StbSQL.AppendLine(" AND Mov_Conf.Cau_Mov        = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONE_SECONDARIA) & "'")
            End If

            '---------------
            '11/12/2019: modifica per gestire la stampa della bolla di conferimento web
            ''StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov    = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'")
            'StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov    = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) & "'")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov    IN ('" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) & "', '" & Agro_SQL_SaveText(CAU_CARICO) & "')")

            'StbSQL.AppendLine(" AND Mov_Dett_Raccolta.elem_cod = " & Agro_SQL_SaveNum(TRASFORMATI_VEGETALI) & "  ")
            StbSQL.AppendLine(" AND Mov_Dett_Raccolta.elem_cod IN ( " & TRASFORMATI_VEGETALI & ", " & TRASFORMATI_ANIMALI & ")  ")

            '---------------

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StbSQL.AppendLine(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "  ")

            'StbSQL.AppendLine(" --AND Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(CStr(objParametri.PivaSuperUser)) & "'  ")
            'StbSQL.AppendLine(" --AND Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(CStr(enum_SequenzaProgressiviTipi.BolleAccettDaDiversi)) & "  ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            'aggregazione per prodotto: 
            'occorre stampare una sola riga a parità di 
            'Prodotto, DettagliProdotto(Calibro, Qualità, Certificazioni,ecc), Lotto, Note, Imballo, Contenitore, Confezione
            'questo perché quando hanno grossi carichi fanno diverse pesate ma non vogliono dare al socio una riga per ogni pesata
            '----------> quindi l'ordinamento è importante
            'dalla query non ho DettagliProdotto(Calibro, Qualità, Certificazioni,ecc), Imballo, Contenitore, Confezione perché li leggo dopo dal cal_cod
            'quindi metto davanti nell'ordinamento Mat_Des_Raccolta, Lotto_Raccolta, Cal_Cod_Raccolta, ONote

            ' Stefano - 7/9/2017  - Cofruta non vuole l'aggregazione delle righe, quindi le ordiniamo in ordine di inserimento a video
            If aggregaRighe Then
                StbSQL.AppendLine(" ORDER BY Lotto_Raccolta, Mat_Des_Raccolta, UDM_Qta.Udm_Sim, Cal_Cod_Raccolta, ONote, Mov_Dett_Raccolta.extra_str, Mov_Dett_Raccolta.Ordine_Det, Mov_Dett_Raccolta.id_mov_det ")
            Else
                StbSQL.AppendLine(" ORDER BY Mov_Dett_Raccolta.extra_str, Mov_Dett_Raccolta.Ordine_Det, Mov_Dett_Raccolta.id_mov_det ")
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
    ''' stampa del report della bolla di accettazione salvata con il giasonline
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function StampaBolla_NuovoConf(ByVal Piva As String,
                                           ByVal Id_Agenda As Integer,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.StampaBolla_NuovoConf"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine("SELECT  Agenda.des_lib, ")
            StbSQL.AppendLine("         Mov_Accett.ChkLayout_Join_Prodotti, Mov_Accett.Peso, Mov_Accett.Extra_Str, Mov_Accett.Natura_Beni, ")
            StbSQL.AppendLine("         Mov_Accett.Tara_Veicolo, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso,  ")
            StbSQL.AppendLine("         Mov_Accett.Data_Movimento AS Data_Accett, Mov_Accett.Ora AS Ora_Accett, ")
            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, Mov_Accett.Doc_Numero_Visualizzato, Mov_Accett.Mov_Desc AS Note, ")
            'StbSQL.AppendLine("         Sequenza_Progressivi.Lunghezza_Sin, Sequenza_Progressivi.Lunghezza_Centro, Sequenza_Progressivi.Lunghezza_Des, Sequenza_Progressivi.CarattereFormattazione, " )
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
            StbSQL.AppendLine("         Mov_Raccolta.Ora AS Ora_Raccolta, C_Az_Raccolta.sa_cod AS Sa_Cod_Raccolta,  ")
            StbSQL.AppendLine("         C_Az_Raccolta.sa_nome AS Sa_Nome_Raccolta,   ")
            'StbSQL.AppendLine("         Fabbr_Raccolta.Fabbricato_Cod AS Fabbricato_Cod_Raccolta, Fabbr_Raccolta.Fabbricato_Des AS Fabbricato_Des_Raccolta, " )
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
            StbSQL.AppendLine("         , ISNULL(   ")
            StbSQL.AppendLine("                 (SELECT TOP 1 OTabelle_Parametri.descrizione ")
            StbSQL.AppendLine("                 FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("                 INNER JOIN OTabelle_Parametri ON Materie_Prime_Campionature.tipo_cod =  OTabelle_Parametri.tabella_par_cod ")
            StbSQL.AppendLine("                 WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.cal_cod ")
            StbSQL.AppendLine("                 AND tipo = 'ocalibro' ")
            StbSQL.AppendLine("                 AND tabella_cod = " & CStr(enum_OTabelle.Calibro) & " ")
            StbSQL.AppendLine("                 ) ,   '' ) AS Calibro  ")
            StbSQL.AppendLine("         , ISNULL(   ")
            StbSQL.AppendLine("                 (SELECT TOP 1 val_cod ")
            StbSQL.AppendLine("                 FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("                 WHERE Materie_Prime_Campionature.progressivo=Mov_Dett_Raccolta.cal_cod ")
            '17:05/2021: gestita lettura anche del grado tenderometrico
            'StbSQL.AppendLine("                 AND tipo = 'opunteggio' ")
            StbSQL.AppendLine("                 AND tipo IN ('opunteggio', 'ogradotenderom') ")
            StbSQL.AppendLine("                 AND val_cod <> 0  ")
            StbSQL.AppendLine("                 ) ,   '' ) AS Punteggio  ")
            'novità new conf
            StbSQL.AppendLine(" , Mov_Dett_Raccolta.ordine_det, Mov_Dett_Raccolta.qta_extra_totale AS qta_extra_totale_Raccolta ")

            StbSQL.AppendLine("")
            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            'StbSQL.AppendLine(" " )
            'StbSQL.AppendLine(" INNER JOIN Sequenza_Progressivi ON Sequenza_Progressivi.Piva = Mov_Accett.PIVA  " )
            'StbSQL.AppendLine(" AND Sequenza_Progressivi.Anno = YEAR(Mov_Accett.Data_Movimento)  " )
            'StbSQL.AppendLine(" AND Sequenza_Progressivi.Doc_Numero_Sin = Mov_Accett.Doc_Numero_Sin  " )
            'StbSQL.AppendLine(" AND Sequenza_Progressivi.Doc_Numero_Des = Mov_Accett.Doc_Numero_Des  " )
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
            ' StbSQL.AppendLine(" INNER JOIN Fabbricati Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Fabbricato_Cod " )
            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" INNER JOIN UnitaMisura UDM_Qta ON Mov_Dett_Raccolta.Udm_Cod = UDM_Qta.Udm_Cod ")
            StbSQL.AppendLine(" ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")

            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov      = '" & CAU_REGISTRAZIONI & "'")
            StbSQL.AppendLine(" AND Mov_Conf.Cau_Mov        = '" & CAU_REGISTRAZIONI_ALLEGATE & "'")

            'modifica rispetto a vecchio conf
            'StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov    = '" + Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) + "'" )
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")

            'novità new conf
            StbSQL.AppendLine(" AND Mov_Dett_Raccolta.elem_cod <> " & BENI_CONFEZ_VEGETALE.ToString & " ")

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StbSQL.AppendLine(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "  ")

            'StbSQL.AppendLine(" AND Sequenza_Progressivi.Piva_SuperUser = '" & Agro_SQL_SaveText(CStr(objParametri.PivaSuperUser)) & "'  " )
            'StbSQL.AppendLine(" AND Sequenza_Progressivi.Tipo_Progressivo = " & Agro_SQL_SaveNum(CStr(enum_SequenzaProgressiviTipi.BolleAccettDaDiversi)) & "  " )

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                StbSQL.AppendLine(" ")
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
    ''' Legge le bolle di accettazione, in base al filtro impostato, per l'esportazione excel
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Bolle_XLS(ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Cod_Articolo As String,
                                ByVal Codice_Esterno As String,
                                ByVal Codice_Conferente As String,
                                ByVal Cod_RisUm As Integer,
                                ByVal Piva_Produttore As String,
                                ByVal Piva_Coop1 As String,
                                ByVal Piva_Coop2 As String,
                                ByVal Data_Inizio As Date,
                                ByVal Data_Fine As Date,
                                ByVal Mat_Cod As Integer,
                                ByVal Veg_Cod As Integer,
                                ByVal Cul_Cod As Integer,
                                ByVal FiltroAggArticoli As String,
                                ByVal FiltroAggConferenti As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal HT_OTab As Hashtable,
                                ByVal HT_OTabParDes As SortedSet(Of String),
                                ByVal Flag_Tracciabilita_Impianti As Integer,
                                ByVal ProgressivoGIAS As Integer,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.Bolle_XLS"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine("SELECT   ")
            StbSQL.AppendLine(" Agenda.Id_Agenda, CA_Raccolta.Sa_Nome , ")
            'StbSQL.AppendLine(" Agenda.des_lib, " )

            StbSQL.AppendLine(" Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, ")
            StbSQL.AppendLine(" Risorse_Umane_Conferenti.Cod_Contatto AS Piva_Conferente, ")
            StbSQL.AppendLine(" Contatti_Conferenti.Codice_Fiscale AS CF_Conferente, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Conferenti.Rag_Soc, ' ') AS RagSoc_Conferente, ")
            StbSQL.AppendLine(" lista_regioni_conferenti.regione_des AS Regione_Conferente, ")

            StbSQL.AppendLine(" ISNULL(Risorse_Umane_Coop1.Cod_Contatto, ' ') AS Piva_Coop1, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Coop1.Rag_Soc, ' ') AS RagSoc_Coop1, ")

            StbSQL.AppendLine(" ISNULL(Risorse_Umane_Coop2.Cod_Contatto, ' ') AS Piva_Coop2, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Coop2.Rag_Soc, ' ') AS RagSoc_Coop2, ")

            StbSQL.AppendLine(" ISNULL(Contatti_Produttori.Cod_Contatto, ' ') AS Piva_Produttore, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Produttori.Codice_Fiscale, ' ') AS CF_Produttore, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Produttori.Rag_Soc, ' ') AS RagSoc_Produttore, ")

            StbSQL.AppendLine(" ISNULL((SELECT regione_des ")
            StbSQL.AppendLine("         FROM Indirizzi  ")
            StbSQL.AppendLine("         INNER Join Lista_Province ON  Lista_Province.prov=Indirizzi.pro_cod_istat ")
            StbSQL.AppendLine("         INNER Join lista_regioni ON  Lista_Province.reg=lista_regioni.reg ")
            StbSQL.AppendLine("         WHERE cod_indirizzo = Mov_Accett.Cod_indirizzodestinazione ")
            StbSQL.AppendLine("         ),'') AS Regione_Produttore,  ")

            StbSQL.AppendLine(" Mov_Accett.Peso AS Peso_Totale, Mov_Accett.Tara_Veicolo, 0.0 AS Peso_Lordo,  ")
            StbSQL.AppendLine(" -- Mov_Accett.Tara_Imballi AS Tara_Imballi_Testata  , ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Tara AS Tara_Dettaglio, 0.0 AS Tara_Imballi, ")
            StbSQL.AppendLine(" ISNULL(  ")
            StbSQL.AppendLine("         (SELECT count(*) AS num_righe ")
            StbSQL.AppendLine("         FROM Movimenti_dettagli MD_righe  ")
            StbSQL.AppendLine("         WHERE MD_righe.piva = agenda.piva  ")
            StbSQL.AppendLine("         AND MD_righe.id_agenda = agenda.id_agenda  ")
            StbSQL.AppendLine("         AND MD_righe.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")
            StbSQL.AppendLine("         ), '0' ) AS num_righe, ")
            StbSQL.AppendLine(" ISNULL(  ")
            StbSQL.AppendLine("         (SELECT sum(Qta_Extra_Totale) as tara_totale_imb_entrata  ")
            StbSQL.AppendLine("            FROM Movimenti_dettagli MD_righe ")
            StbSQL.AppendLine("             WHERE MD_righe.piva = agenda.piva  ")
            StbSQL.AppendLine("             AND MD_righe.id_agenda = agenda.id_agenda    ")
            StbSQL.AppendLine("             AND MD_righe.elem_cod = " & BENI_CONFEZ_VEGETALE.ToString & " ")
            StbSQL.AppendLine("             AND MD_righe.ordine_det = 30000 ")
            StbSQL.AppendLine("             ), '0' ) as tara_totale_imb_entrata, ")

            StbSQL.AppendLine(" Mov_Dett_Raccolta.qta_extra_totale AS Peso_Netto, 0.0 AS Degrado_Perc, 0.0 AS Degrado, 0.0 AS Netto_Pagamento, ")
            'StbSQL.AppendLine(" ISNULL( " )
            'StbSQL.AppendLine(" ( " )
            'StbSQL.AppendLine("     SELECT  Listino_Des " )
            'StbSQL.AppendLine("     FROM    Listini_Prezzi " )
            'StbSQL.AppendLine("     WHERE   Listini_Prezzi.Listino_Cod = Mov_Dett_Raccolta.Listino_Cod " )
            'StbSQL.AppendLine("     AND     Listini_Prezzi.Piva = Mov_Dett_Raccolta.Piva " )
            'StbSQL.AppendLine("     AND     Listini_Prezzi.Piva_SuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " )
            'StbSQL.AppendLine(" ) , '' ) AS Listino, " )
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Prezzo_Unitario_Netto, ")

            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, Mov_Accett.Doc_Numero_Visualizzato AS Numero_Bolla, ")
            StbSQL.AppendLine(" Mov_Accett.Data_Movimento AS Data_Bolla, ")

            StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf, ")
            StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf, '' AS Numero_Conf,  Mov_Conf.Data_Movimento AS Data_Conf,  ")

            StbSQL.AppendLine(" Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta, ")
            StbSQL.AppendLine(" MP_Raccolta.Cod_Articolo, MP_Raccolta.Mat_Cod, MP_Raccolta.Mat_Des, MP_Raccolta.Codice_Esterno, ")

            If ProgressivoGIAS = enum_CodiceGIAS_Clienti.Fruttagel Then
                StbSQL.AppendLine(" '' AS Descrizione_JDE, '' AS Varieta_FRG,  ")
            End If
            StbSQL.AppendLine(" SpecieVegetali.Veg_Cod, Veg_Des AS Descr_Specie, cultivar.cul_cod, Cul_Des AS Descr_Varieta,  ")
            'il regolamento viene sempre salvato (non si dovrebbe quindi mai trovare 0 che nella tabella di metaschema non esiste) 1 = nessun regolamento
            'quindi dal join dovrebbe sempre venire fuori un reg_des
            StbSQL.AppendLine(" MP_Raccolta.Regolamento AS Reg_cod, Regolamenti.Reg_des, ISNULL(grva_des, '') AS grva_des  ")

            StbSQL.AppendLine("         , ISNULL(   ")
            StbSQL.AppendLine("                 (SELECT TOP 1 tipo_cod ")
            StbSQL.AppendLine("                 FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("                 WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.cal_cod ")
            StbSQL.AppendLine("                 AND tipo = 'ocalibro' ")
            StbSQL.AppendLine("                 ), '0' ) AS tipo_cod  ")

            If Not IsNothing(HT_OTabParDes) Then
                StbSQL.AppendLine("         -- aggiungo una colonna vuota col nome del calibro (per ogni calibro gestito nella configurazione oModuli)   ")
                StbSQL.AppendLine("         , 0 AS 'Nessuna_Class'  ")
                For Each key In HT_OTabParDes
                    StbSQL.AppendLine("         , 0 AS '" & Agro_SQL_SaveText(key.ToString, False) & "'  ")
                Next
            End If

            If Not IsNothing(HT_OTab) Then
                StbSQL.AppendLine(" -- le OTabelle che hanno tipo = 1 -> usano i valori di tabella OTabelle_Parametri   ")
                StbSQL.AppendLine(" -- le OTabelle che hanno tipo = 3 -> usano i valori puntuali, val_cod sulle Materie_Prime_Campionature   ")
                For Each key In HT_OTab.Keys
                    StbSQL.AppendLine("         , ISNULL(   ")
                    StbSQL.AppendLine("                 (SELECT TOP 1  ")
                    StbSQL.AppendLine("                         CASE WHEN val_cod <> 0 THEN val_cod  ")
                    StbSQL.AppendLine("                         ELSE OTabelle_Parametri.descrizione ")
                    StbSQL.AppendLine("                         END ")
                    StbSQL.AppendLine("                 FROM Materie_Prime_Campionature ")
                    StbSQL.AppendLine("                 LEFT OUTER JOIN OTabelle_Parametri ON Materie_Prime_Campionature.tipo_cod =  OTabelle_Parametri.tabella_par_cod AND tabella_cod = " & CStr(key) & " ")
                    StbSQL.AppendLine("                 WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.cal_cod ")
                    StbSQL.AppendLine("                 AND tipo = '" & Agro_SQL_SaveText(HT_OTab(key).ToString) & "' ")
                    StbSQL.AppendLine("                 ) ,   '_' ) AS REPLACE_" & HT_OTab(key).ToString & "  ")
                Next
            End If

            'StbSQL.AppendLine("         , ISNULL(   " )
            'StbSQL.AppendLine("                 (SELECT TOP 1 OTabelle_Parametri.descrizione " )
            'StbSQL.AppendLine("                 FROM Materie_Prime_Campionature " )
            'StbSQL.AppendLine("                 INNER JOIN OTabelle_Parametri ON Materie_Prime_Campionature.tipo_cod =  OTabelle_Parametri.tabella_par_cod " )
            'StbSQL.AppendLine("                 WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.cal_cod " )
            'StbSQL.AppendLine("                 AND tipo = 'ocalibro' " )
            'StbSQL.AppendLine("                 AND tabella_cod = " & CStr(enum_OTabelle.Calibro) & " " )
            'StbSQL.AppendLine("                 ) ,   '' ) AS Calibro_Des  " )
            'StbSQL.AppendLine("         , ISNULL(   " )
            'StbSQL.AppendLine("                 (SELECT TOP 1 val_cod " )
            'StbSQL.AppendLine("                 FROM Materie_Prime_Campionature " )
            'StbSQL.AppendLine("                 WHERE Materie_Prime_Campionature.progressivo=Mov_Dett_Raccolta.cal_cod " )
            'StbSQL.AppendLine("                 AND tipo = 'opunteggio' " )
            'StbSQL.AppendLine("                 ) ,   '' ) AS Punteggio  " )
            'StbSQL.AppendLine("         , ISNULL(   " )
            'StbSQL.AppendLine("                 (SELECT TOP 1 val_cod " )
            'StbSQL.AppendLine("                 FROM Materie_Prime_Campionature " )
            'StbSQL.AppendLine("                 WHERE Materie_Prime_Campionature.progressivo=Mov_Dett_Raccolta.cal_cod " )
            'StbSQL.AppendLine("                 AND tipo = 'ogradotenderom' " )
            'StbSQL.AppendLine("                 ) ,   '' ) AS GradoTender  " )
            'StbSQL.AppendLine("         , ISNULL(   " )
            'StbSQL.AppendLine("                 (SELECT TOP 1 val_cod " )
            'StbSQL.AppendLine("                 FROM Materie_Prime_Campionature " )
            'StbSQL.AppendLine("                 WHERE Materie_Prime_Campionature.progressivo=Mov_Dett_Raccolta.cal_cod " )
            'StbSQL.AppendLine("                 AND tipo = 'oresiduosecco' " )
            'StbSQL.AppendLine("                 ) ,   '' ) AS ResiduoSecco  " )
            'novità new conf
            StbSQL.AppendLine(" , Mov_Dett_Raccolta.ordine_det ")

            'colonne per tracciabilità
            If Flag_Tracciabilita_Impianti = 1 Then
                StbSQL.AppendLine(", '' AS Str_Progetto_Nome, '' AS Str_Sup_Imp, '' AS Str_Veg_Des, '' AS Str_Cul_Des,  ")
                StbSQL.AppendLine(" '' AS Str_Grva_Des, '' AS Str_Grfi_Des, '' AS Str_Setup_Cod,  ")
                StbSQL.AppendLine(" '' AS Str_Validita_Inizio_Impianto, '' AS Str_Validita_Fine_Impianto, ")
                StbSQL.AppendLine(" '' AS Str_Validita_Inizio_Distinta, '' AS Str_Validita_Fine_Distinta ")
            End If

            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda  ")

            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")

            StbSQL.AppendLine(" INNER JOIN ImpresexIndirizzi II_Conferenti ON Contatti_Conferenti.cod_contatto=II_Conferenti.piva ")
            StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Conferenti ON Indirizzi_Conferenti.cod_indirizzo = II_Conferenti.Cod_indirizzo ")
            StbSQL.AppendLine(" INNER JOIN Lista_Province Lista_Province_Conferenti ON Lista_Province_Conferenti.prov=Indirizzi_Conferenti.pro_cod_istat ")
            StbSQL.AppendLine(" INNER JOIN lista_regioni lista_regioni_conferenti ON Lista_Province_Conferenti.reg = lista_regioni_conferenti.reg ")

            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  ")

            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop1 ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop1.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Coop1 ON Risorse_Umane_Coop1.Piva = Contatti_Coop1.Piva AND Risorse_Umane_Coop1.Cod_Contatto = Contatti_Coop1.Cod_Contatto  ")

            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop2 ON Mov_Accett.Extra_Int = Risorse_Umane_Coop2.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Coop2 ON Risorse_Umane_Coop2.Piva = Contatti_Coop2.Piva AND Risorse_Umane_Coop2.Cod_Contatto = Contatti_Coop2.Cod_Contatto  ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda  ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   ")
            StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det   ")
            StbSQL.AppendLine(" INNER JOIN Centri_Aziendali CA_Raccolta ON Mov_Dest_Raccolta.Piva = CA_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = CA_Raccolta.SA_COD ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")
            StbSQL.AppendLine(" INNER JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = MP_Raccolta.Veg_Cod ")
            StbSQL.AppendLine(" INNER JOIN Regolamenti ON Regolamenti.Reg_Cod = MP_Raccolta.Regolamento ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Cultivar ON Cultivar.cul_Cod = MP_Raccolta.Cul_Cod ")
            StbSQL.AppendLine(" Left OUTER JOIN GruppoVarietale ON GruppoVarietale.grva_Cod = MP_Raccolta.grva_Cod_veg ")

            StbSQL.AppendLine(" WHERE   Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")
            StbSQL.AppendLine(" AND     Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StbSQL.AppendLine(" AND     Mov_Accett.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            StbSQL.AppendLine(" AND     Mov_Conf.Cau_Mov = '" & CAU_REGISTRAZIONI_ALLEGATE & "'")
            StbSQL.AppendLine(" AND     Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")

            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine("             AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")

            If Sa_Cod <> 0 Then
                StbSQL.AppendLine("         AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Cod_RisUm <> 0 Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro FiltroAggConferenti)
                StbSQL.AppendLine(" AND Mov_Accett.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            End If

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro FiltroAggConferenti)
                StbSQL.AppendLine(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' ")
            End If

            If FiltroAggConferenti <> "" Then
                StbSQL.AppendLine(FiltroAggConferenti)
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

            If Codice_Esterno <> "" Then
                StbSQL.AppendLine(" AND MP_Raccolta.Codice_Esterno = '" & Agro_SQL_SaveText(Codice_Esterno) & "' ")
            End If

            If Cod_Articolo <> "" Then
                'se è stato selezionato un solo articolo
                '(altrimenti c'è il filtro dopo)
                StbSQL.AppendLine(" AND MP_Raccolta.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' ")
            End If

            If FiltroAggArticoli <> "" Then
                StbSQL.AppendLine(FiltroAggArticoli)
            End If

            If Mat_Cod <> 0 Then
                StbSQL.AppendLine(" AND MP_Raccolta.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            If Cul_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            'questo order by è fondamentale, non va cambiato
            StbSQL.AppendLine(" ORDER BY Cod_Articolo, Mat_Des, Mov_Accett.Doc_Numero_Visualizzato, Mov_Accett.Data_Movimento   ")


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
    ''' Legge gli impianti colturali (per la tracciabilità) legati alla raccolta sul produttore, che è agganciata con i riferimenti alla bolla
    ''' Attualmente usata da: report excel conferimenti (agganciargli anche esportazione bolle a terremerse)
    ''' Revisionata il 16/01/2021 per togliere join sul sa_cod_rif che non produceva risultato
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
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.RifImpiantiDaRaccolta_Leggi"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT  Mov_Dettagli_Riferimenti.Piva_Rif, Mov_Dettagli_Riferimenti.Sa_Cod_Rif, Agenda_RaccoltaImpianti.Sa_Cod  AS Sa_CodRaccolta, Mov_Dettagli_Riferimenti.Id_Agenda_Rif, ")
            StbSQL.AppendLine("         Mov_Dettagli_Riferimenti.Lav_Cod_Rif, Agenda_RaccoltaImpianti.des_lib AS Des_Lib_Rif, Mov_Dettagli_Riferimenti.Cau_Mov_Rif,  ")
            StbSQL.AppendLine("         Mov_Dettagli_Riferimenti.Piva, Mov_Dettagli_Riferimenti.Sa_Cod, Mov_Dettagli_Riferimenti.Id_Agenda, Mov_Dettagli_Riferimenti.Lav_Cod, Mov_Dettagli_Riferimenti.Cau_Mov, ")

            StbSQL.AppendLine("         Agenda_Accettazione.des_lib, Movimenti_RaccoltaImpianti.Data_Movimento,    ")
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
            'la bolla di conferimento sta in piva sa_cod lav_cod
            StbSQL.AppendLine(" -- la bolla di conferimento sta in piva sa_cod lav_cod ")
            StbSQL.AppendLine(" INNER JOIN  Agenda Agenda_Accettazione ON Mov_Dettagli_Riferimenti.Piva = Agenda_Accettazione.PIVA  ")
            StbSQL.AppendLine("             AND Mov_Dettagli_Riferimenti.Sa_Cod = Agenda_Accettazione.Sa_Cod  ")
            StbSQL.AppendLine("             AND Mov_Dettagli_Riferimenti.Id_Agenda = Agenda_Accettazione.Id_Agenda  ")
            'la raccolta sta in piva_rif sa_cod_rif lav_cod_rif
            StbSQL.AppendLine(" -- la raccolta sta in piva_rif sa_cod_rif lav_cod_rif ")

            StbSQL.AppendLine(" INNER JOIN  Movimenti_dettagli Movimenti_dettagli_RaccoltaImpianti ")
            StbSQL.AppendLine("             ON Movimenti_dettagli_RaccoltaImpianti.PIVA = Mov_Dettagli_Riferimenti.Piva_Rif ")
            StbSQL.AppendLine("             AND Movimenti_dettagli_RaccoltaImpianti.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda_Rif ")
            StbSQL.AppendLine("             AND (Movimenti_dettagli_RaccoltaImpianti.Id_Mov = Mov_Dettagli_Riferimenti.Id_Mov_Rif OR Mov_Dettagli_Riferimenti.Id_Mov_Rif = -1) ")
            StbSQL.AppendLine("             AND (Movimenti_dettagli_RaccoltaImpianti.Id_Mov_Det = Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif OR Mov_Dettagli_Riferimenti.Id_Mov_Det_Rif = -1) ")

            StbSQL.AppendLine(" INNER JOIN  Movimenti Movimenti_RaccoltaImpianti ")
            StbSQL.AppendLine("             ON Movimenti_RaccoltaImpianti.Piva = Movimenti_dettagli_RaccoltaImpianti.PIVA ")
            StbSQL.AppendLine("             AND Movimenti_RaccoltaImpianti.Id_Agenda = Movimenti_dettagli_RaccoltaImpianti.Id_Agenda ")
            StbSQL.AppendLine("             AND Movimenti_RaccoltaImpianti.Id_Mov = Movimenti_dettagli_RaccoltaImpianti.Id_Mov ")

            StbSQL.AppendLine(" INNER JOIN  Agenda Agenda_RaccoltaImpianti ")
            StbSQL.AppendLine("             ON Agenda_RaccoltaImpianti.Piva = Movimenti_RaccoltaImpianti.Piva ")
            StbSQL.AppendLine("             AND Agenda_RaccoltaImpianti.Id_Agenda = Movimenti_RaccoltaImpianti.Id_Agenda ")

            StbSQL.AppendLine(" INNER JOIN  Mov_Destinazioni Mov_Destinazioni_RaccoltaImpianti ON Movimenti_dettagli_RaccoltaImpianti.PIVA = Mov_Destinazioni_RaccoltaImpianti.Piva  ")
            StbSQL.AppendLine("             AND Movimenti_dettagli_RaccoltaImpianti.Sa_Cod = Mov_Destinazioni_RaccoltaImpianti.Sa_Cod  ")
            StbSQL.AppendLine("             AND Movimenti_dettagli_RaccoltaImpianti.Id_Agenda = Mov_Destinazioni_RaccoltaImpianti.Id_Agenda  ")
            StbSQL.AppendLine("             AND Movimenti_dettagli_RaccoltaImpianti.Id_Mov = Mov_Destinazioni_RaccoltaImpianti.Id_Mov  ")
            StbSQL.AppendLine("             AND Movimenti_dettagli_RaccoltaImpianti.Id_Mov_Det = Mov_Destinazioni_RaccoltaImpianti.Id_Mov_Det  ")
            StbSQL.AppendLine(" INNER JOIN Reg_Impianti ON Reg_Impianti.PIVA = Mov_Destinazioni_RaccoltaImpianti.Piva  ")
            StbSQL.AppendLine("             AND Reg_Impianti.Sa_Cod = Mov_Destinazioni_RaccoltaImpianti.Sa_Cod  ")
            StbSQL.AppendLine("             AND Reg_Impianti.Appezza = Mov_Destinazioni_RaccoltaImpianti.Appezza  ")
            StbSQL.AppendLine("             AND Reg_Impianti.Id_Reg = Mov_Destinazioni_RaccoltaImpianti.Id_Destinazione ")
            StbSQL.AppendLine(" INNER JOIN Imprese_Progetti ON Reg_Impianti.PIVA = Imprese_Progetti.Piva ")
            StbSQL.AppendLine("             AND Reg_Impianti.Sa_Cod = Imprese_Progetti.Sa_Cod  ")
            StbSQL.AppendLine("             AND Reg_Impianti.Appezza = Imprese_Progetti.Appezza  ")
            StbSQL.AppendLine("             AND Reg_Impianti.Id_Reg = Imprese_Progetti.Id_reg ")
            StbSQL.AppendLine(" INNER JOIN Cultivar ON Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod ")
            StbSQL.AppendLine(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
            StbSQL.AppendLine(" INNER JOIN GruppoFinalita ON Reg_Impianti.Grfi_Cod = GruppoFinalita.Grfi_Cod ")
            StbSQL.AppendLine(" LEFT OUTER JOIN GruppoVarietale ON Reg_Impianti.GRVA_Cod_VEG  = GruppoVarietale.Grva_Cod ")
            StbSQL.AppendLine("")
            StbSQL.AppendLine(" WHERE       (Mov_Dettagli_Riferimenti.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & "))  ")
            StbSQL.AppendLine(" AND         (Mov_Dettagli_Riferimenti.Lav_Cod_Rif = " & LAVCOD_RACCOLTA & ") ")
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
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                StbSQL.AppendLine(" ")
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
    Public Function Trasportatori_XLS(ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                         ByVal Veg_Cod As Integer,
                                        ByVal Cul_Cod As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Cod_Articolo As String,
                                        ByVal Codice_Esterno As String,
                                        ByVal Codice_Conferente As String,
                                        ByVal Data_Inizio As Date,
                                        ByVal Data_Fine As Date,
                                        ByVal FiltroAggArticoli As String,
                                        ByVal FiltroAggConferenti As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.Trasportatori_XLS"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine("SELECT   ")
            'StbSQL.AppendLine(" Agenda.Id_Agenda,  Agenda.des_lib, " )

            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Visualizzato AS Numero_Bolla, ")
            StbSQL.AppendLine(" Mov_Accett.Data_Movimento AS Data_Bolla, ")

            StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf, ")
            StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf, '' AS Numero_Conf,  Mov_Conf.Data_Movimento AS Data_Conf,  ")

            StbSQL.AppendLine(" Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Conferenti.Rag_Soc, ' ') AS RagSoc_Conferente, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Prima_Coop.Cod_Contatto, ' ') AS Piva_Prima_Cooperativa, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Prima_Coop.Rag_Soc, ' ') AS RagSoc_Prima_Cooperativa, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Sec_Coop.Cod_Contatto, ' ') AS Piva_Sec_Cooperativa, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Sec_Coop.Rag_Soc, ' ') AS RagSoc_Sec_Cooperativa, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Produttori.Cod_Contatto, ' ') AS Piva_Produttore, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Produttori.Rag_Soc, ' ') AS RagSoc_Produttore, ")



            ' StbSQL.AppendLine(" Mov_Accett.Peso, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso,  " )
            StbSQL.AppendLine(" ISNULL(mp.Mat_Des, '') AS Codice_Zona, lpd.prezzo AS Listino, lpd.Udm_Cod AS Listino_Udm, dest.Qta As Qta,Mov_Dett_Raccolta.Id_Mov_Det AS riga_Conferimento,dest.Appezza as Appezza,dest.Piva as Piva,dest.Sa_Cod as Sa_Cod,dest.Id_Destinazione as Id_Destinazione, Contatti_Trasportatori.Rag_Soc + Contatti_Trasportatori.Cognome + ' ' + Contatti_Trasportatori.Nome AS RagSoc_Trasportatore ")
            'novità new conf
            StbSQL.AppendLine(" , Mov_Dett_Raccolta.ordine_det, Mov_Dett_Raccolta.qta_extra_totale AS Netto_Trasportato ")
            StbSQL.AppendLine(" , Mov_Dett_Raccolta.Variazione AS Degrado_Perc, Mov_Dett_Raccolta.Udm_Cod ")
            StbSQL.AppendLine(" , 0.0 AS Degrado, 0.0 AS Netto_Pagamento ")
            StbSQL.AppendLine(" , '' AS Costo_Totale ")

            StbSQL.AppendLine("  ,       ISNULL(  (SELECT Mdt_Extra.Targa + '_' + Mdt_Extra.N_Immatricolazione_Rimorchio ")
            StbSQL.AppendLine("                     FROM Movimenti Mov_Extra  ")
            StbSQL.AppendLine("                     INNER JOIN Mov_Dettaglio_Tecnico_Extra Mdt_Extra ON Mdt_Extra.PIVA = Mov_Extra.PIVA AND Mdt_Extra.Sa_Cod = Mov_Extra.Sa_Cod ")
            StbSQL.AppendLine("                     AND Mdt_Extra.Id_Agenda = Mov_Extra.Id_Agenda AND Mdt_Extra.Id_Mov = Mov_Extra.Id_Mov ")
            StbSQL.AppendLine("                     WHERE Mov_Accett.PIVA = Mov_Extra.PIVA  ")
            StbSQL.AppendLine("                     AND Mov_Accett.Sa_Cod = Mov_Extra.Sa_Cod  ")
            StbSQL.AppendLine("                     AND Mov_Accett.Id_Agenda = Mov_Extra.Id_Agenda  ")
            StbSQL.AppendLine("                     AND Mov_Accett.Id_mov = Mov_Extra.Id_mov  ")
            StbSQL.AppendLine("                     ), '_' ) AS Targhe, '' AS Targa_Automezzo, '' AS Targa_Rimorchio ")

            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda  ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda  ")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Trasportatori ON Mov_Accett.Cod_Vettore = Risorse_Umane_Trasportatori.Cod_RisUm  ")
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Trasportatori ON Risorse_Umane_Trasportatori.Piva = Contatti_Trasportatori.Piva AND Risorse_Umane_Trasportatori.Cod_Contatto = Contatti_Trasportatori.Cod_Contatto  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti_Codici Listino_Trasportatori ON Contatti_Trasportatori.piva = Listino_Trasportatori.piva AND Contatti_Trasportatori.Sa_Cod = Listino_Trasportatori.Sa_Cod AND Contatti_Trasportatori.Cod_Contatto = Listino_Trasportatori.Cod_Contatto AND  Listino_Trasportatori.Id_cod = 4001 AND Listino_Trasportatori.Val_cod <> 0 ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane RisUma_Prima_Coop ON Mov_Accett.Cod_RisUm_Altro = RisUma_Prima_Coop.Cod_RisUm")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Prima_Coop ON Contatti_Prima_Coop.Piva = RisUma_Prima_Coop.Piva AND Contatti_Prima_Coop.Cod_Contatto = RisUma_Prima_Coop.Cod_Contatto ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane RisUma_Sec_Coop ON Mov_Accett.Extra_Int = RisUma_Sec_Coop.Cod_RisUm")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Sec_Coop ON Contatti_Sec_Coop.Piva = RisUma_Sec_Coop.Piva AND Contatti_Sec_Coop.Cod_Contatto = RisUma_Sec_Coop.Cod_Contatto ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   ")
            StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det   ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")
            StbSQL.AppendLine(" Left Join mov_dettagli_riferimenti rif on Mov_Dett_Raccolta.piva = rif.piva And Mov_Dett_Raccolta.Id_Agenda = rif.Id_Agenda and Mov_Dett_Raccolta.Id_Mov_Det = rif.Id_Mov_Det And rif.lav_cod_rif = 125  ")
            StbSQL.AppendLine(" Left Join Mov_Destinazioni dest on rif.Piva_Rif = dest.piva And rif.Id_Agenda_Rif = dest.Id_Agenda And dest.Tipo_Destinazione = 0 And dest.Appezza <> 0 ")
            StbSQL.AppendLine(" Left Join Reg_Impianti_Codici imp on imp.piva = dest.piva And imp.SA_COD = dest.Sa_Cod And imp.APPEZZA = dest.Appezza And imp.ID_REG = dest.Id_Destinazione And imp.id_cod= 1350 And imp.Progetto_Cod = 0 ")
            StbSQL.AppendLine(" Left Join materie_prime mp on  mp.elem_cod = 700 And mp.Mat_Cod = imp.val_cod  ")
            StbSQL.AppendLine(" Left Join Listini_Prezzi_Dettagli lpd on Listino_Trasportatori.val_cod= lpd.Listino_Cod And lpd.Mat_Cod = imp.val_cod And Mov_Accett.Data_Movimento <= lpd.Validita_Fine And Mov_Accett.Data_Movimento >= lpd.Validita_Inizio ")


            StbSQL.AppendLine(" WHERE   Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")
            StbSQL.AppendLine(" AND     Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StbSQL.AppendLine(" AND     Mov_Accett.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            StbSQL.AppendLine(" AND     Mov_Conf.Cau_Mov = '" & CAU_REGISTRAZIONI_ALLEGATE & "'")
            StbSQL.AppendLine(" AND     Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")
            'StbSQL.AppendLine(" And agenda.id_agenda =  '" & 257325 & "'")

            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine("             AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")

            If Sa_Cod <> 0 Then
                StbSQL.AppendLine("         AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            'Select Case ProgressivoGIAS
            '    Case enum_CodiceGIAS_Clienti.Fruttagel
            '        'filtro il TRASPORTATORE C/PROPRIO che è un vettore fittizio
            '        StbSQL.AppendLine(" AND Contatti_Trasportatori.cod_contatto <> '-1999999998' " )
            'End Select

            If Mat_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            If Cul_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
            End If

            If Cod_Articolo <> "" Then
                'se è stata selezionato un solo cod articolo
                '(altrimenti c'è il filtro dopo)
                StbSQL.AppendLine(" AND MP_Raccolta.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' ")
            End If

            If Codice_Esterno <> "" Then
                StbSQL.AppendLine(" AND MP_Raccolta.Codice_Esterno = '" & Agro_SQL_SaveText(Codice_Esterno) & "' ")
            End If

            If FiltroAggArticoli <> "" Then
                StbSQL.AppendLine(FiltroAggArticoli)
            End If

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.AppendLine(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' ")
            End If

            If FiltroAggConferenti <> "" Then
                StbSQL.AppendLine(FiltroAggConferenti)
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                StbSQL.AppendLine(" ORDER BY RagSoc_Trasportatore, Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des,Mov_Accett.Data_Movimento, Mov_Dett_Raccolta.Id_Mov_Det  ")
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
    ''' DA MODIFICARE LEGGENDO IL NUOVO CONFERIMENTO
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function BolleXExportXML_Filtro(ByVal Piva As String,
                                                    ByVal Cod_Risum As Integer,
                                                    ByVal Cod_Risum_Altro As Integer,
                                                    ByVal Stato_Export_2 As Integer,
                                                    ByVal Data_Inizio As Date,
                                                    ByVal Data_Fine As Date,
                                                    ByVal Filtro_Id_Agenda As String,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.BolleXExportXML_Filtro"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT ")
            StbSQL.AppendLine(" Agenda.Piva, Agenda.Sa_Cod, Agenda.ID_Agenda, Mov_Dett_Raccolta.Id_Mov_Det, Agenda.Stato_Export, Agenda.Stato_Export_2, Agenda.Blocco_Flag, Agenda.Blocco_Data, Agenda.Blocco_Username, ")
            StbSQL.AppendLine(" Mov_Accett.Data_Movimento AS Data_Bolla, ")
            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, Mov_Accett.Doc_Numero_Visualizzato, ")
            StbSQL.AppendLine(" Mov_Accett.Cod_RisUm, Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Conferenti.Rag_Soc, ' ') AS RagSoc_Conferente, ")
            StbSQL.AppendLine(" Mov_Accett.Cod_RisUm_Altro, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Coop.Rag_Soc, ' ') AS RagSoc_Coop, ")

            StbSQL.AppendLine(" MP_Raccolta.Cod_Articolo AS Codice_Articolo, MP_Raccolta.Codice_Esterno, MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Udm_Cod, Mov_Dett_Raccolta.Qta, Mov_Dett_Raccolta.Variazione AS Degrado_Perc,")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Prezzo_Unitario_Netto AS Prezzo_Unitario ")

            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")

            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")

            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Coop ON Risorse_Umane_Coop.Piva = Contatti_Coop.Piva AND Risorse_Umane_Coop.Cod_Contatto = Contatti_Coop.Cod_Contatto  ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")
            StbSQL.AppendLine(" INNER JOIN specievegetali ON specievegetali.veg_cod = MP_Raccolta.veg_cod ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")

            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")

            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StbSQL.AppendLine(" AND Agenda.Stato_Export_2 = " & Agro_SQL_SaveNum(Stato_Export_2) & " ")

            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine(" AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")

            'If Cod_Risum <> 0 Then
            '    StbSQL.AppendLine(" AND Mov_Accett.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_Risum) & " ")
            'End If

            'If Cod_Risum <> 0 Then
            '    StbSQL.AppendLine(" AND ( ")
            '    StbSQL.AppendLine(" (Mov_Accett.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_Risum) & " ")
            '    StbSQL.AppendLine(" AND Mov_Accett.Cod_RisUm_Altro = " & Agro_SQL_SaveNum(Cod_Risum_Altro) & ") ")
            '    StbSQL.AppendLine(" OR (Mov_Accett.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_Risum_Altro) & ") ")
            '    StbSQL.AppendLine(" )")
            'End If

            If Filtro_Id_Agenda <> "" Then
                StbSQL.AppendLine(Filtro_Id_Agenda)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                StbSQL.AppendLine(" ORDER BY Data_Bolla, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero_des ASC ")
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
    ''' legge le bolle da esportare nel formato xml pubblico
    ''' attualmente esportazione da fruttagel x terremerse
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function BolleXExportXML(ByVal Piva As String,
                                                ByVal Filtro_Id_Agenda As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.BolleXExportXML"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.AppendLine("SELECT   ")
            StbSQL.AppendLine(" Agenda.Id_Agenda, Agenda.Stato_Export_2, Centri_Aziendali.Sa_Nome, Mov_Dett_Raccolta.Id_Mov_Det, Mov_Dett_Raccolta.Ordine_Det, ")

            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Sin ,Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, Mov_Accett.Doc_Numero_Visualizzato, ")
            StbSQL.AppendLine(" Mov_Accett.Data_Movimento AS Data_Bolla, Mov_Accett.Ora AS Ora_Bolla,  ")

            StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf, ")
            StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf, Mov_Conf.Data_Movimento AS Data_Conf,  Mov_Conf.Colli, ")

            StbSQL.AppendLine(" Mov_Accett.Mov_Desc AS Note, ")

            StbSQL.AppendLine(" Agenda.PIVA AS Piva_Conferitore, ISNULL(Risorse_Umane_Conferenti.Cod_Contatto, '') AS Piva_Conferente, ")
            StbSQL.AppendLine(" ISNULL(Risorse_Umane_Coop.Cod_Contatto, '') AS Piva_Coop, ISNULL(Risorse_Umane_Coop2.Cod_Contatto, '') AS Piva_Coop_2,  ")
            StbSQL.AppendLine(" ISNULL(Risorse_Umane_Produttori.Cod_Contatto, '') AS Piva_Produttore,")

            StbSQL.AppendLine(" Mov_Accett.Peso, Mov_Accett.Tara_Veicolo,  Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso, ")
            StbSQL.AppendLine(" 0 AS Degrado, 0 AS Netto_Pagamento, ")

            StbSQL.AppendLine(" Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta, Mov_Dett_Raccolta.Tara AS Tara, ")
            StbSQL.AppendLine(" UDM_Qta.Udm_Sim AS Udm_Sim_Raccolta, UDM_Qta.Udm_des AS Udm_des_Raccolta, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Prezzo_Unitario_Netto AS Prezzo_unitario, ")

            StbSQL.AppendLine(" ISNULL( (SELECT TOP 1 ISNULL( CONVERT(VARCHAR(25),MP_Camp_Calibro.tipo_cod) , '0')  + '|' + OTabelle_Parametri.descrizione  ")
            StbSQL.AppendLine("         FROM Materie_Prime_Campionature MP_Camp_Calibro ")
            StbSQL.AppendLine("         INNER JOIN OTabelle_Parametri ON MP_Camp_Calibro.tipo_cod =  OTabelle_Parametri.tabella_par_cod  ")
            StbSQL.AppendLine("         WHERE MP_Camp_Calibro.progressivo = Mov_Dett_Raccolta.cal_cod  ")
            StbSQL.AppendLine("         AND tabella_cod= " & CStr(enum_OTabelle.Calibro) & "  ")
            StbSQL.AppendLine("         AND MP_Camp_Calibro.tipo = 'ocalibro'), 0 ) AS Calibro, ")

            StbSQL.AppendLine("  ISNULL( (SELECT TOP 1 ISNULL( MP_Camp.val_cod , '0')  ")
            StbSQL.AppendLine("          FROM Materie_Prime_Campionature MP_Camp ")
            StbSQL.AppendLine("         WHERE MP_Camp.progressivo = Mov_Dett_Raccolta.cal_cod   ")
            StbSQL.AppendLine("         AND MP_Camp.tipo = 'opunteggio'), 0 ) AS Punteggio,  ")

            StbSQL.AppendLine("  ISNULL( (SELECT TOP 1 ISNULL( MP_Camp.val_cod , '0')   ")
            StbSQL.AppendLine("          FROM Materie_Prime_Campionature MP_Camp ")
            StbSQL.AppendLine("         WHERE MP_Camp.progressivo = Mov_Dett_Raccolta.cal_cod   ")
            StbSQL.AppendLine("         AND MP_Camp.tipo = 'ogradotenderom'), 0 ) AS GradoTenderom,   ")

            StbSQL.AppendLine("  ISNULL( (SELECT TOP 1 ISNULL( MP_Camp.val_cod , '0')  ")
            StbSQL.AppendLine("          FROM Materie_Prime_Campionature MP_Camp ")
            StbSQL.AppendLine("         WHERE MP_Camp.progressivo = Mov_Dett_Raccolta.cal_cod   ")
            StbSQL.AppendLine("         AND MP_Camp.tipo = 'oresiduosecco'), 0 ) AS ResiduoSecco,   ")

            StbSQL.AppendLine(" MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta, MP_Raccolta.Mat_Cod AS Codice_Prodotto, MP_Raccolta.Codice_Esterno, MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, ")
            StbSQL.AppendLine(" MP_Raccolta.Veg_Cod AS Codice_Specie_Gias, MP_Raccolta.Cul_Cod AS Codice_Varieta_Gias,")
            StbSQL.AppendLine(" MP_Raccolta.Grva_Cod_Veg AS Codice_TipologiaVarietale_Gias, MP_Raccolta.Regolamento ")

            '02/02/2016: modifica x richiesta di terremerse:
            'aggiunta info sul magazzino e sul trasportatore
            StbSQL.AppendLine(" , Mov_Destinazioni.Sa_Cod as SaCod_Magazzino, Mov_Destinazioni.id_destinazione as FabbricatoCod_Magazzino ")
            StbSQL.AppendLine(" , Contatti_Vettore.Cod_Contatto AS CodContatto_Vettore, (Contatti_Vettore.rag_soc + Contatti_Vettore.Nome + ' ' + Contatti_Vettore.Cognome) AS Vettore_RagSoc ")

            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda  ")

            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop.Cod_RisUm    ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop2 ON Mov_Accett.Extra_Int = Risorse_Umane_Coop2.Cod_RisUm    ")

            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Vettore ON Mov_Accett.Cod_Vettore = Risorse_Umane_Vettore.Cod_RisUm     ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Vettore ON Risorse_Umane_Vettore.Cod_Contatto =  Contatti_Vettore.cod_contatto AND Risorse_Umane_Vettore.piva =  Contatti_Vettore.piva   ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda  ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")
            StbSQL.AppendLine(" INNER JOIN specievegetali ON specievegetali.veg_cod = MP_Raccolta.veg_cod ")
            StbSQL.AppendLine(" INNER JOIN UnitaMisura UDM_Qta ON Mov_Dett_Raccolta.Udm_Cod = UDM_Qta.Udm_Cod ")

            '02/02/2016: modifica x richiesta di terremerse:
            'aggiunta info sul magazzino
            StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Destinazioni.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Destinazioni.Id_Mov = Mov_Dett_Raccolta.Id_Mov    AND Mov_Destinazioni.Id_Mov_Det = Mov_Dett_Raccolta.Id_Mov_Det     ")
            'StbSQL.AppendLine(" INNER JOIN Fabbricati ON Mov_Destinazioni.PIVA = Fabbricati.PIVA AND Mov_Destinazioni.sa_cod = Fabbricati.sa_cod AND Mov_Destinazioni.id_destinazione = fabbricati.fabbricato_cod   " )
            StbSQL.AppendLine(" INNER JOIN Centri_Aziendali ON Mov_Dett_Raccolta.piva = Centri_Aziendali.piva AND  Mov_Dett_Raccolta.sa_cod = Centri_Aziendali.sa_cod  ")


            StbSQL.AppendLine(" WHERE   Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")
            StbSQL.AppendLine(" AND     Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StbSQL.AppendLine(" AND     Mov_Accett.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            StbSQL.AppendLine(" AND     Mov_Conf.Cau_Mov = '" & CAU_REGISTRAZIONI_ALLEGATE & "'")
            StbSQL.AppendLine(" AND     Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")

            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine(" AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")

            StbSQL.AppendLine(Filtro_Id_Agenda)

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                StbSQL.AppendLine(" ORDER BY Mov_Accett.Data_Movimento, Mov_Accett.Doc_Numero_Visualizzato, Agenda.Id_Agenda, ordine_det, id_mov_det ASC ")
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
    Public Function RiepilogoConferimentixArticolo(ByVal Flag01_GestioneCodEsterno As Integer,
                                                   ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Veg_Cod As Integer,
                                                ByVal Cul_Cod As Integer,
                                                ByVal Mat_Cod As Integer,
                                                ByVal Cod_Articolo As String,
                                                ByVal Codice_Conferente As String,
                                                ByVal Cod_RisUm As Integer,
                                                ByVal Data_Inizio As Date,
                                                ByVal Data_Fine As Date,
                                                ByVal FiltroAggArticoli As String,
                                                ByVal FiltroAggConferenti As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.RiepilogoConferimentixArticolo"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Codice_Esterno As String = ""
        'Dim FiltroCodEsterno As String = ""

        Try

            ''imposto i filtri in abse alla gestione dei codici
            'If Flag01_GestioneCodEsterno = 1 Then
            '    Codice_Esterno = Cod_Articolo
            '    Cod_Articolo = ""
            '    FiltroCodEsterno = Replace(FiltroCodArticolo, "Cod_Articolo", "Codice_Esterno")
            '    FiltroCodArticolo = ""
            'Else
            '    Codice_Esterno = ""
            '    FiltroCodEsterno = ""
            'End If


            StbSQL.Length = 0

            StbSQL.AppendLine("SELECT Imprese.rag_soc, Agenda.Id_Agenda, Mov_Dett_Raccolta.Sa_Cod, Agenda.des_lib,  ")
            StbSQL.AppendLine(" Mov_Accett.Id_Mov AS Id_Mov_Accett, Mov_Accett.Cau_Mov AS Cau_Mov_Accett, Mov_Accett.Cod_RisUm, ")
            StbSQL.AppendLine(" Risorse_Umane_Conferenti.settore_des AS Codice_Conferente, Contatti_Conferenti.Rag_Soc AS Rag_Soc_Conferente, ")

            StbSQL.AppendLine(" Mov_Accett.Peso, Mov_Accett.Tara_Veicolo, ")
            StbSQL.AppendLine(" Mov_Accett.Tara_Imballi AS Tara_Imballi_Testata, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Tara AS Tara_Dettaglio, ")
            StbSQL.AppendLine(" ISNULL(  ")
            StbSQL.AppendLine("         (SELECT count(*) AS num_righe ")
            StbSQL.AppendLine("         FROM Movimenti_dettagli MD_righe  ")
            StbSQL.AppendLine("         WHERE MD_righe.piva = agenda.piva  ")
            StbSQL.AppendLine("         AND MD_righe.id_agenda = agenda.id_agenda  ")
            StbSQL.AppendLine("         AND MD_righe.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")
            StbSQL.AppendLine("         ), 0 ) AS num_righe, ")
            StbSQL.AppendLine(" ISNULL(  ")
            StbSQL.AppendLine("         (SELECT sum(Qta_Extra_Totale) as tara_totale_imb_entrata  ")
            StbSQL.AppendLine("            FROM Movimenti_dettagli MD_righe ")
            StbSQL.AppendLine("             WHERE MD_righe.piva = agenda.piva  ")
            StbSQL.AppendLine("             AND MD_righe.id_agenda = agenda.id_agenda    ")
            StbSQL.AppendLine("             AND MD_righe.elem_cod = " & BENI_CONFEZ_VEGETALE.ToString & " ")
            StbSQL.AppendLine("             AND MD_righe.ordine_det = 30000 ")
            StbSQL.AppendLine("             ), 0 ) as tara_totale_imb_entrata, ")

            'novità new conf
            StbSQL.AppendLine(" Mov_Dett_Raccolta.qta_extra_totale AS qta_extra_totale_Raccolta, ")
            StbSQL.AppendLine(" Mov_Raccolta.Id_Mov AS Id_Mov_Raccolta, Mov_Raccolta.Cau_Mov AS Cau_Mov_Raccolta,  ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Id_Mov_Det AS Id_Mov_Det_Raccolta, Mov_Dett_Raccolta.Elem_Cod AS Elem_Cod_Raccolta, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Pro_Cod AS Pro_Cod_Raccolta, Mov_Dett_Raccolta.Mat_Cod AS Mat_Cod_Raccolta,  ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Cod_Progetto AS Cod_Progetto_Raccolta, Mov_Dett_Raccolta.Fase_Cod AS Fase_Cod_Raccolta,  ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta, Mov_Dett_Raccolta.Tara AS Tara, ")

            StbSQL.AppendLine(" MP_Raccolta.Cod_Articolo, MP_Raccolta.Codice_Esterno, MP_Raccolta.Mat_Des, ")
            StbSQL.AppendLine(" CAST(ISNULL( (SELECT TOP 1 VAL_COD ")
            StbSQL.AppendLine("         FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("         WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.cal_cod ")
            StbSQL.AppendLine("         AND materie_Prime_Campionature.tipo IN ('opunteggio','ogradotenderom','oresiduosecco','ogradobrix') ")
            StbSQL.AppendLine("         AND materie_Prime_Campionature.val_cod NOT IN ('0', '') ")
            'StbSQL.AppendLine("         AND materie_Prime_Campionature.tipo_cod IN ( " + CStr(FRUTTAGEL_INDMATCOD_PUNTEGGIO) + ", " + CStr(FRUTTAGEL_INDMATCOD_GRADOTEND) + ", " + CStr(FRUTTAGEL_INDMATCOD_GRADOBRIX) + " ) ")
            StbSQL.AppendLine("         ), '0' ) AS FLOAT) AS Punteggio ")
            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN imprese ON Agenda.PIVA = Imprese.PIVA ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det ")

            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")
            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")
            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine("     AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")

            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            StbSQL.AppendLine(" AND     Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")

            If Mat_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            If Cul_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
            End If

            If Cod_Articolo <> "" Then
                'se è stata selezionato un solo cod articolo
                '(altrimenti c'è il filtro dopo)
                StbSQL.AppendLine(" AND MP_Raccolta.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' ")
            End If

            If Codice_Esterno <> "" Then
                'se è stata selezionato un solo cod articolo
                '(altrimenti c'è il filtro dopo)
                StbSQL.AppendLine(" AND MP_Raccolta.Codice_Esterno = '" & Agro_SQL_SaveText(Codice_Esterno) & "' ")
            End If

            If FiltroAggArticoli <> "" Then
                StbSQL.AppendLine(FiltroAggArticoli)
            End If

            'If FiltroCodArticolo <> "" Then
            '    StbSQL.AppendLine(FiltroCodArticolo)
            'End If

            'If FiltroCodEsterno <> "" Then
            '    StbSQL.AppendLine(FiltroCodEsterno)
            'End If

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.AppendLine(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' ")
            End If

            If Cod_RisUm <> 0 Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro FiltroAggConferenti)
                StbSQL.AppendLine(" AND Mov_Accett.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            End If

            If FiltroAggConferenti <> "" Then
                StbSQL.AppendLine(FiltroAggConferenti)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                If Flag01_GestioneCodEsterno = 1 Then
                    StbSQL.AppendLine(" ORDER BY MP_Raccolta.Codice_Esterno, Risorse_Umane_Conferenti.settore_Des ")
                Else
                    StbSQL.AppendLine(" ORDER BY MP_Raccolta.Cod_Articolo, Risorse_Umane_Conferenti.settore_Des ")
                End If
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
    Public Function ComunicazioneCredito(ByVal Flag01_GestioneCodEsterno As Integer,
                                                   ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Veg_Cod As Integer,
                                                ByVal Cul_Cod As Integer,
                                                ByVal Mat_Cod As Integer,
                                                ByVal Codice_Conferente As String,
                                                ByVal Cod_RisUm As Integer,
                                                ByVal Data_Inizio As Date,
                                                ByVal Data_Fine As Date,
                                                ByVal FiltroAggArticoli As String,
                                                ByVal FiltroAggConferenti As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.ComunicazioneCredito"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StbSQL.Length = 0

            StbSQL.AppendLine("SELECT Imprese.rag_soc, Agenda.Id_Agenda, Mov_Dett_Raccolta.Sa_Cod, ISTAT.LOCALITA AS CentroAz_Municipio, ISTAT.Comuni_PROV AS CentroAz_Provincia, Agenda.des_lib,  ")
            StbSQL.AppendLine(" Mov_Accett.Id_Mov AS Id_Mov_Accett, Mov_Accett.Cau_Mov AS Cau_Mov_Accett, Mov_Accett.Cod_RisUm AS Cod_RisUm_Conferente, ")
            
            StbSQL.AppendLine(" Mov_Accett.Data_Movimento AS Accettazione_Data, ")
            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Sin + ")
            StbSQL.AppendLine("   CASE WHEN LEN(LTRIM(STR(Mov_Accett.doc_numero,10))) > 5 THEN LTRIM(STR(Mov_Accett.doc_numero,10)) ")
            StbSQL.AppendLine("   ELSE REPLICATE('0', 5 - LEN(LTRIM(STR(Mov_Accett.doc_numero,10)))) + LTRIM(STR(Mov_Accett.doc_numero,10))  END ")
            StbSQL.AppendLine("   + Mov_Accett.Doc_Numero_Des AS Accettazione_NumeroVis, ")
            
            StbSQL.AppendLine(" Mov_DDTConferente.Data_Movimento AS DDTConferente_Data,")
            StbSQL.AppendLine(" Mov_DDTConferente.Doc_Numero_Sin + ")
            StbSQL.AppendLine("   CASE WHEN LEN(LTRIM(STR(Mov_DDTConferente.doc_numero,10))) > 5 THEN LTRIM(STR(Mov_DDTConferente.doc_numero,10)) ")
            StbSQL.AppendLine("   ELSE REPLICATE('0', 5 - LEN(LTRIM(STR(Mov_DDTConferente.doc_numero,10)))) + LTRIM(STR(Mov_DDTConferente.doc_numero,10))  END ")
            StbSQL.AppendLine("   + Mov_DDTConferente.Doc_Numero_Des AS DDTConferente_NumeroVis, ")

            StbSQL.AppendLine(" Risorse_Umane_Conferenti.settore_des AS Codice_Conferente, Contatti_Conferenti.Rag_Soc AS Rag_Soc_Conferente, ")
            StbSQL.AppendLine(" ISNULL(Conf_Des_Pagamenti_Causali.Cau_Pagamento_Des, '') AS Cau_Pagamento_Des, ")

            StbSQL.AppendLine(" Contatto_Referente_Conf.Cognome + ' ' + Contatto_Referente_Conf.Nome AS Conferente_Referente, ")
            StbSQL.AppendLine(" Rubrica_Referente_Conf.numero AS Conferente_Referente_Mail, ")

            'StbSQL.AppendLine(" Mov_Accett.Peso, Mov_Accett.Tara_Veicolo, ")
            'StbSQL.AppendLine(" Mov_Accett.Tara_Imballi AS Tara_Imballi_Testata, ")
            'StbSQL.AppendLine(" Mov_Dett_Raccolta.Tara AS Tara_Dettaglio, ")
            'StbSQL.AppendLine(" ISNULL(  ")
            'StbSQL.AppendLine("         (SELECT count(*) AS num_righe ")
            'StbSQL.AppendLine("         FROM Movimenti_dettagli MD_righe  ")
            'StbSQL.AppendLine("         WHERE MD_righe.piva = agenda.piva  ")
            'StbSQL.AppendLine("         AND MD_righe.id_agenda = agenda.id_agenda  ")
            'StbSQL.AppendLine("         AND MD_righe.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")
            'StbSQL.AppendLine("         ), 0 ) AS num_righe, ")
            'StbSQL.AppendLine(" ISNULL(  ")
            'StbSQL.AppendLine("         (SELECT sum(Qta_Extra_Totale) as tara_totale_imb_entrata  ")
            'StbSQL.AppendLine("            FROM Movimenti_dettagli MD_righe ")
            'StbSQL.AppendLine("             WHERE MD_righe.piva = agenda.piva  ")
            'StbSQL.AppendLine("             AND MD_righe.id_agenda = agenda.id_agenda    ")
            'StbSQL.AppendLine("             AND MD_righe.elem_cod = " & BENI_CONFEZ_VEGETALE.ToString & " ")
            'StbSQL.AppendLine("             AND MD_righe.ordine_det = 30000 ")
            'StbSQL.AppendLine("             ), 0 ) as tara_totale_imb_entrata, ")

            'novità new conf
            StbSQL.AppendLine(" Mov_Dett_Raccolta.qta_extra_totale AS qta_extra_totale_Raccolta, ")
            StbSQL.AppendLine(" Mov_Raccolta.Id_Mov AS Id_Mov_Raccolta, Mov_Raccolta.Cau_Mov AS Cau_Mov_Raccolta,  ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Id_Mov_Det AS Id_Mov_Det_Raccolta, Mov_Dett_Raccolta.Elem_Cod AS Elem_Cod_Raccolta, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Pro_Cod AS Pro_Cod_Raccolta, Mov_Dett_Raccolta.Mat_Cod AS Mat_Cod_Raccolta,  ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Cod_Progetto AS Cod_Progetto_Raccolta, Mov_Dett_Raccolta.Fase_Cod AS Fase_Cod_Raccolta,  ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta, Mov_Dett_Raccolta.Tara AS Tara, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Prezzo_Effettivo, ")

            StbSQL.AppendLine(" CASE WHEN Mov_Dett_Raccolta.Variazione IS NULL THEN Convert(Int, ISNULL(Mov_Dett_Raccolta.Qta_Extra_Totale, 0)) ")
            StbSQL.AppendLine("       WHEN Mov_Dett_Raccolta.Variazione = 0 THEN Convert(Int, ISNULL(Mov_Dett_Raccolta.Qta_Extra_Totale, 0)) ")
            StbSQL.AppendLine("       ELSE Convert(Int, ISNULL(Mov_Dett_Raccolta.Qta_Extra_Totale, 0)) ")
            StbSQL.AppendLine("       - convert(int, round((isnull(Mov_Dett_Raccolta.Qta_Extra_Totale,0) / 100 * Mov_Dett_Raccolta.Variazione),0)) ")
            StbSQL.AppendLine("  END AS Netto_Pagamento, ")

            StbSQL.AppendLine(" MP_Raccolta.Cod_Articolo, MP_Raccolta.Codice_Esterno, MP_Raccolta.Mat_Des, ")
            StbSQL.AppendLine(" SpecieVegetali.Veg_Des, Cultivar.Cul_Des, ")
            StbSQL.AppendLine(" Contatti_Produttori.Rag_Soc AS Rag_Soc_Produttore ")
            'StbSQL.AppendLine(",CAST(ISNULL( (SELECT TOP 1 VAL_COD ")
            'StbSQL.AppendLine("         FROM Materie_Prime_Campionature ")
            'StbSQL.AppendLine("         WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.cal_cod ")
            'StbSQL.AppendLine("         AND materie_Prime_Campionature.tipo IN ('opunteggio','ogradotenderom','oresiduosecco','ogradobrix') ")
            'StbSQL.AppendLine("         AND materie_Prime_Campionature.val_cod NOT IN ('0', '') ")
            ''StbSQL.AppendLine("         AND materie_Prime_Campionature.tipo_cod IN ( " + CStr(FRUTTAGEL_INDMATCOD_PUNTEGGIO) + ", " + CStr(FRUTTAGEL_INDMATCOD_GRADOTEND) + ", " + CStr(FRUTTAGEL_INDMATCOD_GRADOBRIX) + " ) ")
            'StbSQL.AppendLine("         ), '0' ) AS FLOAT) AS Punteggio ")
            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN imprese ON Agenda.PIVA = Imprese.PIVA ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_DDTConferente ON Agenda.PIVA = Mov_DDTConferente.PIVA AND Agenda.Id_Agenda = Mov_DDTConferente.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")
            StbSQL.AppendLine(" LEFT JOIN Contatti_Codici Conf_Cod_Pagamenti_Causali ON Conf_Cod_Pagamenti_Causali.Piva = Contatti_Conferenti.Piva AND Conf_Cod_Pagamenti_Causali.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")
            StbSQL.AppendLine(" AND Conf_Cod_Pagamenti_Causali.Id_Cod = " & enum_CodiciAnagrafe.ModalitaPagamentoDefault)
            StbSQL.AppendLine(" LEFT JOIN Pagamenti_Causali Conf_Des_Pagamenti_Causali ON Conf_Des_Pagamenti_Causali.Cau_Pagamento = Conf_Cod_Pagamenti_Causali.Val_Cod ")

            StbSQL.AppendLine(" LEFT JOIN Contatti_Codici Codici_Referente_Conf ON Codici_Referente_Conf.Piva = Contatti_Conferenti.Piva AND Codici_Referente_Conf.Cod_Contatto = Contatti_Conferenti.Cod_Contatto ")
            StbSQL.AppendLine(" AND Codici_Referente_Conf.Id_Cod = " & enum_CodiciAnagrafe.ReferenteConferimento)
            StbSQL.AppendLine(" LEFT JOIN Risorse_Umane RisUm_Referente_Conf ON RisUm_Referente_Conf.Cod_RisUm = Codici_Referente_Conf.Val_Cod ")
            StbSQL.AppendLine(" LEFT JOIN Contatti Contatto_Referente_Conf ON Contatto_Referente_Conf.Piva = RisUm_Referente_Conf.Piva AND Contatto_Referente_Conf.Cod_Contatto = RisUm_Referente_Conf.Cod_Contatto ")
            StbSQL.AppendLine(" LEFT JOIN ContattiXRubrica CR_Referente_Conf ON CR_Referente_Conf.Piva = Contatto_Referente_Conf.Piva AND CR_Referente_Conf.Cod_Contatto = Contatto_Referente_Conf.Cod_Contatto ")
            StbSQL.AppendLine(" LEFT JOIN Rubrica Rubrica_Referente_Conf ON Rubrica_Referente_Conf.Cod_Rubrica = CR_Referente_Conf.Cod_Rubrica AND Rubrica_Referente_Conf.descr like '%mail%' ")

            StbSQL.AppendLine(" LEFT JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm ")
            StbSQL.AppendLine(" LEFT JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det ")

            StbSQL.AppendLine(" INNER JOIN Centri_Aziendali ON Mov_Dett_Raccolta.Piva = Centri_Aziendali.Piva AND Mov_Dett_Raccolta.Sa_Cod = Centri_Aziendali.sa_cod ")
            StbSQL.AppendLine(" INNER JOIN CentrixIndirizzi ON Centri_Aziendali.Piva = CentrixIndirizzi.Piva AND Centri_Aziendali.sa_cod = CentrixIndirizzi.sa_cod AND CentrixIndirizzi.Tipo_Indirizzo = " & enum_TipiIndirizzi.SedeOperativa)
            StbSQL.AppendLine(" INNER JOIN Indirizzi ON CentrixIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo ")
            StbSQL.AppendLine(" LEFT JOIN ISTAT ON Indirizzi.com_cod_istat = ISTAT.COM AND Indirizzi.pro_cod_istat = ISTAT.PROV ")



            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")
            
            StbSQL.AppendLine(" LEFT JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = MP_Raccolta.Veg_Cod ")
            StbSQL.AppendLine(" LEFT JOIN Cultivar ON Cultivar.Cul_Cod = MP_Raccolta.Cul_Cod ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")
            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            StbSQL.AppendLine(" AND Mov_DDTConferente.Cau_Mov = '" & CAU_REGISTRAZIONI_ALLEGATE & "'")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")
            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine("     AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")

            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            StbSQL.AppendLine(" AND     Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")

            If Mat_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            If Cul_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
            End If

            If FiltroAggArticoli <> "" Then
                StbSQL.AppendLine(FiltroAggArticoli)
            End If

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.AppendLine(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' ")
            End If

            If Cod_RisUm <> 0 Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro FiltroAggConferenti)
                StbSQL.AppendLine(" AND Mov_Accett.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            End If

            If FiltroAggConferenti <> "" Then
                StbSQL.AppendLine(FiltroAggConferenti)
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                If Flag01_GestioneCodEsterno = 1 Then
                    StbSQL.AppendLine(" ORDER BY Risorse_Umane_Conferenti.Cod_RisUm, MP_Raccolta.Codice_Esterno, Mov_Accett.Data_Movimento, Mov_Accett.Doc_Numero ")
                Else
                    StbSQL.AppendLine(" ORDER BY Risorse_Umane_Conferenti.Cod_RisUm, MP_Raccolta.Cod_Articolo, Mov_Accett.Data_Movimento, Mov_Accett.Doc_Numero ")
                End If
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




    '22/05/2017: non più usata
    'viene utilizzata: AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli.ElencoOTabelleConfigurateSulVegCod()
    Private Sub StbSQL_OTabelle_Parametri(ByRef stb As StringBuilder,
                                   ByVal Parametro As String,
                                   ByVal Tabella_Cod As Integer)

        stb.AppendLine(vbCrLf)
        stb.AppendLine(" , ISNULL ( ")
        stb.AppendLine("            ( ")
        stb.AppendLine("            SELECT OTabelle_Parametri.Descrizione ")
        stb.AppendLine("            FROM Materie_Prime_Campionature  ")
        stb.AppendLine("            INNER JOIN OTabelle_Parametri ON  Materie_Prime_Campionature.tipo_cod = OTabelle_Parametri.Tabella_Par_Cod ")
        stb.AppendLine("            WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.Cal_Cod ")
        stb.AppendLine("            AND Materie_Prime_Campionature.tipo ='" & Agro_SQL_SaveText(Parametro) & "' ")
        stb.AppendLine("            AND OTabelle_Parametri.Tabella_Cod = " & Agro_SQL_SaveNum(Tabella_Cod) & " ")
        stb.AppendLine("            AND Materie_Prime_Campionature.tipo_cod <> 0 ")
        stb.AppendLine("             ), '') as " & Parametro & " ")
        stb.AppendLine(vbCrLf)

    End Sub

    Private Sub StbSQL_Contatti(ByRef stb As StringBuilder,
                         ByVal Parametro As String)

        stb.AppendLine(vbCrLf)
        stb.AppendLine(" , ISNULL ( ")
        stb.AppendLine("            ( ")
        stb.AppendLine("            SELECT contatti.rag_soc + contatti.nome + ' ' + contatti.cognome ")
        stb.AppendLine("            FROM Materie_Prime_Campionature  ")
        stb.AppendLine("            INNER JOIN Risorse_Umane ON  Materie_Prime_Campionature.tipo_cod = Risorse_Umane.Cod_RisUm ")
        stb.AppendLine("            INNER JOIN Contatti ON  Contatti.piva = Risorse_Umane.piva AND Contatti.cod_contatto = Risorse_Umane.cod_contatto ")
        stb.AppendLine("            WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.Cal_Cod  ")
        stb.AppendLine("            AND Materie_Prime_Campionature.tipo ='" & Agro_SQL_SaveText(Parametro) & "' ")
        stb.AppendLine("            AND Materie_Prime_Campionature.tipo_cod <> 0 ")
        stb.AppendLine("             ), '') as " & Parametro & " ")
        stb.AppendLine(vbCrLf)

    End Sub

    Private Sub StbSQL_Note(ByRef stb As StringBuilder)

        stb.AppendLine(vbCrLf)
        stb.AppendLine(" , ISNULL ( ")
        stb.AppendLine("            ( ")
        stb.AppendLine("            SELECT Materie_Prime_Campionature.Descrizione ")
        stb.AppendLine("            FROM Materie_Prime_Campionature  ")
        stb.AppendLine("            WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.Cal_Cod  ")
        stb.AppendLine("            AND Materie_Prime_Campionature.tipo ='onote' ")
        stb.AppendLine("             ), '') AS ONote ")
        stb.AppendLine(vbCrLf)

    End Sub


    '' -----------------------------------------------------------------------------
    '' <summary>
    '' report saldo imballi: nuova versione ottimizzata
    ''Default:
    ''Optional ByVal FiltroConferenti As String = "", _
    ''Optional ByVal Ordinamento As String = "", _
    ''Optional ByVal FinestraTemp_Inizio As Date = #1/1/1900#, _
    ''Optional ByVal FinestraTemp_Fine As Date = #12/31/2100# _
    '' </summary>
    '' -----------------------------------------------------------------------------
    Public Function SaldoImballi_FF(ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Fabbricato_Cod As Integer,
                                ByVal Mat_Cod As Integer,
                                ByVal Cod_RisUm As Integer,
                                ByVal Data_Inizio As Date,
                                ByVal Data_Fine As Date,
                                ByVal Data_Giacenza As Date,
                                ByVal Codice_Conferente As String,
                                ByVal FinestraTemp_Inizio As Date,
                                ByVal FinestraTemp_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.SaldoImballi_FF"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT  Contatti_Conferenti.Cod_Contatto AS Codice_Conferente, ")
            StbSQL.AppendLine("         Contatti_Conferenti.Rag_Soc AS RagSoc_Conferente, ")
            StbSQL.AppendLine("         MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des ")

            StbSQL.AppendLine("         , SUM( CASE WHEN    Mov_Magazzino.CAU_MOV='" & CAU_CARICO & "' ")
            StbSQL.AppendLine("                 AND         Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine("                 AND         Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
            StbSQL.AppendLine("                 THEN        Mov_Dest_Magazzino.qta ")
            StbSQL.AppendLine("                 ELSE 0 ")
            StbSQL.AppendLine("                 END) AS Qta_Totale_Carichi ")

            StbSQL.AppendLine("         , SUM( CASE WHEN    Mov_Magazzino.CAU_MOV='" & CAU_SCARICO & "' ")
            StbSQL.AppendLine("                 AND         Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine("                 AND         Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")
            StbSQL.AppendLine("                 THEN        Mov_Dest_Magazzino.qta ")
            StbSQL.AppendLine("                 ELSE 0 ")
            StbSQL.AppendLine("                 END) AS Qta_Totale_Scarichi ")

            StbSQL.AppendLine("         , SUM( CASE WHEN    Mov_Magazzino.CAU_MOV='" & CAU_SCARICO & "' ")
            StbSQL.AppendLine("                 THEN        -(Mov_Dest_Magazzino.qta) ")
            StbSQL.AppendLine("                 ELSE        Mov_Dest_Magazzino.qta ")
            StbSQL.AppendLine("                 END) AS Giacenza ")

            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Cont ON Agenda.PIVA = Mov_Cont.PIVA AND Agenda.Id_Agenda = Mov_Cont.Id_Agenda  ")

            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Cont.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov   ")
            StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Magazzino ON Mov_Dett_Magazzino.PIVA = Mov_Dest_Magazzino.Piva AND Mov_Dett_Magazzino.Sa_Cod = Mov_Dest_Magazzino.Sa_Cod AND  Mov_Dett_Magazzino.Id_Agenda = Mov_Dest_Magazzino.Id_Agenda AND Mov_Dett_Magazzino.Id_Mov = Mov_Dest_Magazzino.Id_Mov AND Mov_Dett_Magazzino.Id_Mov_Det = Mov_Dest_Magazzino.Id_Mov_Det   ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Imballi ON Mov_Dett_Magazzino.Elem_Cod = MP_Imballi.Elem_Cod AND Mov_Dett_Magazzino.Mat_Cod = MP_Imballi.Mat_Cod ")

            StbSQL.AppendLine(" WHERE   Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StbSQL.AppendLine(" AND     Mov_Dett_Magazzino.Jolly_Int = " & CStr(MagazzinoMovimentato) & "   ")
            StbSQL.AppendLine(" AND     Mov_Dett_Magazzino.Contabilizzato >= 0  ")
            StbSQL.AppendLine(" AND     Mov_Dest_Magazzino.Tipo_Destinazione = " & CStr(TIPO_DESTINAZIONE_MAGAZZINO) & "    ")

            StbSQL.AppendLine(" AND     Mov_Cont.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            StbSQL.AppendLine(" AND     Mov_Magazzino.Cau_Mov IN ( '" & CAU_CARICO & "', '" & CAU_SCARICO & "') ")

            StbSQL.AppendLine(" AND     Mov_Magazzino.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Giacenza) & " ")
            StbSQL.AppendLine(" AND     Mov_Magazzino.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")

            StbSQL.AppendLine(" AND     Mov_Dest_Magazzino.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StbSQL.AppendLine(" AND     Mov_Dest_Magazzino.Id_Destinazione = " & Agro_SQL_SaveNum(Fabbricato_Cod) & "  ")

            StbSQL.AppendLine(" AND     Risorse_Umane_Conferenti.Cod_Rapporto IN (" & COD_CONFERENTE & "," & COD_FORNITORE_ORTOFRUTTA & ")  ")

            If Cod_RisUm <> 0 Then
                StbSQL.AppendLine(" AND Risorse_Umane_Conferenti.Cod_RisUm = " & Agro_SQL_SaveText(Cod_RisUm) & " ")
            End If

            If Codice_Conferente <> "" Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro dopo)
                StbSQL.AppendLine(" AND Risorse_Umane_Conferenti.settore_Des = '" & Agro_SQL_SaveText(Codice_Conferente) & "' ")
            End If

            StbSQL.AppendLine(" AND Mov_Dett_Magazzino.Elem_Cod = " & Agro_SQL_SaveNum(BENI_CONFEZ_VEGETALE) & "   ")

            StbSQL.AppendLine(" AND Mov_Dett_Magazzino.Pro_Cod = " & Agro_SQL_SaveNum(0) & "   ")

            If Mat_Cod <> 0 Then
                StbSQL.AppendLine(" AND Mov_Dett_Magazzino.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            StbSQL.AppendLine(" AND Mov_Dett_Magazzino.Cod_Progetto = " & Agro_SQL_SaveNum(0) & "   ")

            StbSQL.AppendLine(" AND Mov_Dett_Magazzino.Fase_Cod = " & Agro_SQL_SaveNum(0) & "   ")

            StbSQL.AppendLine(" AND Mov_Dett_Magazzino.Cal_Cod = " & Agro_SQL_SaveNum(0) & "   ")


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            StbSQL.AppendLine(" AND MP_Imballi.chkimballaggio=1 ")
            '--------------------------------------------------------------------------

            StbSQL.AppendLine(" GROUP BY  Contatti_Conferenti.Cod_Contatto, ")
            StbSQL.AppendLine("         Contatti_Conferenti.Rag_Soc, ")
            StbSQL.AppendLine("         MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des ")

            'condizione per filtrare le giacenze=0
            StbSQL.AppendLine("  HAVING SUM(CASE WHEN Mov_Magazzino.Cau_Mov= '" & CAU_SCARICO & "' THEN -(Mov_Dest_Magazzino.qta) ELSE Mov_Dest_Magazzino.qta END) <> 0 ")



            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                StbSQL.AppendLine(" ORDER BY  RagSoc_Conferente, MP_Imballi.Mat_Des ")
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


    '######################################################################
    Public Function RiepilogoImballi_BollaConferimento(ByVal Lista_Progressivo As String,
                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.RiepilogoImballi_BollaConferimento"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim Tara As String = ""

        Try
            StbSQL.AppendLine(" SELECT OTabelle_Parametri.Descrizione, Materie_Prime_Campionature.Tara_Campionatura, SUM(CONVERT(int,val_cod)) AS Numero ")
            StbSQL.AppendLine(" FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine(" INNER JOIN OTabelle_Parametri ON  Materie_Prime_Campionature.tipo_cod = OTabelle_Parametri.Tabella_Par_Cod ")
            '08/06/2018: riattivata visualizzazione confezioni x richiesta di Cofruta
            StbSQL.AppendLine(" WHERE Tipo IN ( 'oimballaggio', 'ocontenitore', 'oconfezione') ")
            'StbSQL.AppendLine(" WHERE Tipo IN ( 'oimballaggio', 'ocontenitore') ")
            StbSQL.AppendLine(" AND progressivo in (" & Agro_SQL_Save_Clausola_IN(Lista_Progressivo) & ") ")
            StbSQL.AppendLine(" AND tipo_cod <> 0  ")
            StbSQL.AppendLine(" AND ChkTara_Campionatura = 1 ")
            StbSQL.AppendLine(" GROUP BY OTabelle_Parametri.Descrizione, Materie_Prime_Campionature.Tara_Campionatura, Tipo ")
            StbSQL.AppendLine(" ORDER BY Tipo DESC, Descrizione ASC, Tara_Campionatura ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StbSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Tara = ""
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function



    '##############################################################################################
    Public Function LeggiConferimenti_xExportCSV(ByVal piva As String,
                                                  ByVal List_Id_Agenda As Integer(),
                                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.LeggiConferimenti_xExportCSV"
        Dim MessaggioErrore As String = ""

        'Dim risposta As String = ""
        Dim DT As DataTable

        Dim gefutils As New Gias_EF_Utility

        Try

            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                Dim movimenti_dettagli As DbSet(Of Movimenti_dettagli) = GiasContext.Movimenti_dettagli
                Dim movimenti As DbSet(Of Movimenti) = GiasContext.Movimenti
                Dim agenda As DbSet(Of Agenda) = GiasContext.Agenda
                Dim contatti As DbSet(Of Contatti) = GiasContext.Contatti
                Dim risorse_umane As DbSet(Of Risorse_Umane) = GiasContext.Risorse_Umane
                Dim matPrima As DbSet(Of Materie_Prime) = GiasContext.Materie_Prime
                Dim matPrimeCampionature As DbSet(Of Materie_Prime_Campionature) = GiasContext.Materie_Prime_Campionature
                Dim tabelleParametri As DbSet(Of OTabelle_Parametri) = GiasContext.OTabelle_Parametri
                Dim LineeProd As DbSet(Of Linee_Produzioni) = GiasContext.Linee_Produzioni

                Dim lavCodConf As Integer?() = {LAVCOD_DISTINTA_CARICO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_BOLLA_RICEVUTA, LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE}

                'Join mov_det_extra In Mov_Dettaglio_Tecnico_Extra
                '        On mov_det_extra.Piva Equals mov_det.PIVA And
                '         mov_det_extra.Id_Agenda Equals mov_det.Id_Agenda And
                '         mov_det_extra.Id_Mov Equals mov_det.Id_Mov And
                '        mov_det_extra.Id_Mov_Det Equals mov_det.Id_Mov
                'Join cont In contatti
                '    On
                '     r_u.Piva Equals cont.Piva And
                '     r_u.Cod_Contatto Equals cont.Cod_Contatto
                'Join mat_prima In matPrima
                '        On
                '         mov_det.Elem_Cod Equals mat_prima.Elem_Cod And
                '         mov_det.Mat_Cod Equals mat_prima.Mat_Cod
                '    Group Join mat_prima_dett In matprimadett
                '        On mat_prima_dett.Piva Equals mat_prima.Piva And
                '            mat_prima_dett.Mat_Cod Equals mat_prima.Mat_Cod
                '        Into mat_prima_dett_group = Group
                '    From _mat_prima_dett In mat_prima_dett_group.DefaultIfEmpty()
                '    Group Join otab_param_grpfatt In tabelleParametri.Where(Function(x) x.Tabella_Cod = 20 And (x.Piva = piva Or x.Piva = "AAAAAAAAAAA") And x.Modulo_Generazione = 2)
                '        On otab_param_grpfatt.Tabella_Par_Cod Equals _mat_prima_dett.Extra_Int1
                '        Into otab_param_grpfatt_group = Group
                '    From _otab_param_grpfatt In otab_param_grpfatt_group.DefaultIfEmpty()

                '(String.IsNullOrEmpty(_nrRiga) Or mov_det.Extra_Str.Equals(_nrRiga)) AndAlso
                '     (_specie = -1 Or mat_prima.Veg_Cod = _specie) AndAlso
                '     ((_filtroSuVarieta = False) Or _varieta.Contains(mat_prima.Cul_Cod)) AndAlso
                '     ((_filtroSuOperazioni = False) Or _operazioni.Contains(ag.Lav_Cod)) AndAlso
                '     ((_filtroSuFornitori = False) Or _fornitori.Contains(cont.Cod_Contatto)) AndAlso
                '     ((_filtroSuGruppoFatturazione = False) Or gruppoFatturazione.Contains(_mat_prima_dett.Extra_Int1)) AndAlso

                '((Id_Mov_Det = 0) Or (mov_det.Id_Mov_Det = Id_Mov_Det)) AndAlso
                '       (String.IsNullOrEmpty(_dataInizioCampDal) Or _cm.Dt_Inizio_Campionamento >= inizioCampDalDateTime) And
                '       (String.IsNullOrEmpty(_dataInizioCampAl) Or _cm.Dt_Inizio_Campionamento <= inizioCampAlDateTime) And
                '       (String.IsNullOrEmpty(_bollaCampDal) Or _cm.NrBolla_Campionamento >= _bollaCampDal) And
                '       (String.IsNullOrEmpty(_bollaCampAl) Or _cm.NrBolla_Campionamento <= _bollaCampAl)

                'Dim datada As DateTime
                'Dim dataa As DateTime

                'datada = Convert.ToDateTime("01/10/2017")
                'dataa = Convert.ToDateTime("31/10/2017")

                ' mov.Data_Movimento >= datada AndAlso
                '  mov.Data_Movimento <= dataa

                Dim RigheConferimento =
                    From ag In agenda
                    Join mov In movimenti
                        On ag.PIVA Equals mov.PIVA And
                         ag.Id_Agenda Equals mov.Id_Agenda
                    Join mov_for In movimenti
                        On ag.PIVA Equals mov_for.PIVA And
                         ag.Id_Agenda Equals mov_for.Id_Agenda
                    Join r_u In risorse_umane
                        On mov.Cod_RisUm Equals r_u.Cod_RisUm
                    Join mov_det In movimenti_dettagli
                        On mov.PIVA Equals mov_det.PIVA And
                         mov.Id_Agenda Equals mov_det.Id_Agenda And
                         mov.Id_Mov Equals mov_det.Id_Mov
                    Join mat_prima In matPrima
                            On mov_det.Elem_Cod Equals mat_prima.Elem_Cod And
                             mov_det.Mat_Cod Equals mat_prima.Mat_Cod
                    Join linee_prod In LineeProd
                            On linee_prod.Linea_Cod Equals mat_prima.Linea_Cod
                    Group Join mat_prime_camp_imballo In matPrimeCampionature.Where(Function(x) x.Tipo = "oimballaggio")
                        On mat_prime_camp_imballo.Progressivo Equals mov_det.Cal_Cod
                        Into mat_prime_camp_imballo_group = Group
                    From _mat_prime_camp_imballo In mat_prime_camp_imballo_group.DefaultIfEmpty()
                    Group Join otab_param_imballo In tabelleParametri.Where(Function(x) x.Tabella_Cod = enum_OTabelle.Imballaggio AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = enum_Omni_Modulo_Generazione.FreshFood)
                        On otab_param_imballo.Tabella_Par_Cod Equals _mat_prime_camp_imballo.Tipo_Cod
                        Into otab_param_imballo_group = Group
                    From _otp_imballo In otab_param_imballo_group.DefaultIfEmpty()
                    Group Join mat_prime_camp_cont In matPrimeCampionature.Where(Function(x) x.Tipo = "ocontenitore")
                        On mat_prime_camp_cont.Progressivo Equals mov_det.Cal_Cod
                        Into mat_prime_camp_cont_group = Group
                    From _mat_prime_camp_cont In mat_prime_camp_cont_group.DefaultIfEmpty()
                    Group Join otab_param_cont In tabelleParametri.Where(Function(x) x.Tabella_Cod = enum_OTabelle.Contenitore AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = enum_Omni_Modulo_Generazione.FreshFood)
                        On otab_param_cont.Tabella_Par_Cod Equals _mat_prime_camp_cont.Tipo_Cod
                        Into otab_param_cont_group = Group
                    From _otp_contenitore In otab_param_cont_group.DefaultIfEmpty()
                    Group Join mat_prime_camp_conf In matPrimeCampionature.Where(Function(x) x.Tipo = "oconfezione")
                        On mat_prime_camp_conf.Progressivo Equals mov_det.Cal_Cod
                          Into mat_prime_camp_conf_group = Group
                    From _mat_prime_camp_conf In mat_prime_camp_conf_group.DefaultIfEmpty()
                    Group Join otab_param_conf In tabelleParametri.Where(Function(x) x.Tabella_Cod = enum_OTabelle.Confezione AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = enum_Omni_Modulo_Generazione.FreshFood)
                        On otab_param_conf.Tabella_Par_Cod Equals _mat_prime_camp_conf.Tipo_Cod
                        Into otab_param_conf_group = Group
                    From _otp_confez In otab_param_conf_group.DefaultIfEmpty()
                    Group Join mat_prime_camp_caratteristica In matPrimeCampionature.Where(Function(x) x.Tipo = "ocaratteristica")
                        On mat_prime_camp_caratteristica.Progressivo Equals mov_det.Cal_Cod
                         Into mat_prime_camp_caratteristica_group = Group
                    From _mat_prime_camp_caratteristica In mat_prime_camp_caratteristica_group.DefaultIfEmpty()
                    Group Join otab_param_caratter In tabelleParametri.Where(Function(x) x.Tabella_Cod = enum_OTabelle.Caratteristica AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = enum_Omni_Modulo_Generazione.FreshFood)
                        On otab_param_caratter.Tabella_Par_Cod Equals _mat_prime_camp_caratteristica.Tipo_Cod
                        Into otab_param_caratter_group = Group
                    From _otp_caratter In otab_param_caratter_group.DefaultIfEmpty()
                    Group Join mat_prime_camp_altrecaratter In matPrimeCampionature.Where(Function(x) x.Tipo = "oaltrecaratter")
                        On mat_prime_camp_altrecaratter.Progressivo Equals mov_det.Cal_Cod
                        Into mat_prime_camp_altrecaratter_group = Group
                    From _mat_prime_camp_altrecaratter In mat_prime_camp_altrecaratter_group.DefaultIfEmpty()
                    Group Join otab_param_altrecaratter In tabelleParametri.Where(Function(x) x.Tabella_Cod = enum_OTabelle.AltreCaratteristiche AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = enum_Omni_Modulo_Generazione.FreshFood)
                        On otab_param_altrecaratter.Tabella_Par_Cod Equals _mat_prime_camp_altrecaratter.Tipo_Cod
                        Into otab_param_altrecaratter_group = Group
                    From _otp_altrecaratter In otab_param_altrecaratter_group.DefaultIfEmpty()
                    Join mat_prime_camp_calibri In matPrimeCampionature.Where(Function(x) x.Tipo = "ocalibro")
                        On mat_prime_camp_calibri.Progressivo Equals mov_det.Cal_Cod
                    Group Join otab_param_calibro In tabelleParametri.Where(Function(x) x.Tabella_Cod = enum_OTabelle.Calibro AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = enum_Omni_Modulo_Generazione.FreshFood)
                        On
                         otab_param_calibro.Tabella_Par_Cod Equals mat_prime_camp_calibri.Tipo_Cod
                        Into otab_param_cal_group = Group
                    From _otp_cal In otab_param_cal_group.DefaultIfEmpty()
                    Join mat_prime_camp_qualita In matPrimeCampionature.Where(Function(x) x.Tipo = "oqualità")
                        On mat_prime_camp_qualita.Progressivo Equals mov_det.Cal_Cod
                    Group Join otab_param_qual In tabelleParametri.Where(Function(x) x.Tabella_Cod = enum_OTabelle.Qualita AndAlso (x.Piva = piva OrElse x.Piva = "AAAAAAAAAAA") AndAlso x.Modulo_Generazione = enum_Omni_Modulo_Generazione.FreshFood)
                        On
                         otab_param_qual.Tabella_Par_Cod Equals mat_prime_camp_qualita.Tipo_Cod
                        Into otab_param_qual_group = Group
                    From _otp_qual In otab_param_qual_group.DefaultIfEmpty()
                    Where
                        ag.PIVA.Equals(piva) AndAlso
                        ag.Modulo = enum_Omni_Modulo_Generazione.FreshFood AndAlso
                        List_Id_Agenda.Contains(ag.Id_Agenda) AndAlso
                        lavCodConf.Contains(ag.Lav_Cod) AndAlso
                        mov.Cau_Mov = CAU_REGISTRAZIONI_TERZIARIA AndAlso
                        mov_for.Cau_Mov = CAU_REGISTRAZIONE_SECONDARIA AndAlso
                        linee_prod.Piva <> "AAAAAAAAAAA"
                    Group By x = New With {
                                 Key .data_accettazione = mov.Data_Movimento,
                                 Key .ora = mov.Ora,
                                 Key .data_ddt_fornitore = mov_for.Data_Movimento,
                                 Key .nr_interno = mov.Doc_Numero,
                                 Key .codice_fornitore = r_u.Settore_Des,
                                 Key .num_ddt_fornitore = mov_for.Doc_Numero_Sin & CStr(mov_for.Doc_Numero) & mov_for.Doc_Numero_Des,
                                 Key linee_prod.Linea_Cod_Des,
                                Key .ocaratteristica = If(_otp_caratter.Codice_Origine Is Nothing, "", _otp_caratter.Codice_Origine),
                                Key .oaltrecaratteristiche = If(_otp_altrecaratter Is Nothing, "", _otp_altrecaratter.Codice_Origine),
                                Key .ocalibro = If(_otp_cal Is Nothing, "", _otp_cal.Codice_Origine),
                                Key .lotto = mov_det.Lotto,
                                Key .cod_imballaggio = If(_otp_imballo Is Nothing, "", _otp_imballo.Codice_Origine),
                                Key .cod_contenitore = If(_otp_contenitore Is Nothing, "", _otp_contenitore.Codice_Origine),
                                Key .cod_confezione = If(_otp_confez Is Nothing, "", _otp_confez.Codice_Origine),
                                Key .qualita_codiceorigine = If(_otp_qual Is Nothing, "0", _otp_qual.Codice_Origine),
                                Key .udm_cod = mov_det.Udm_Cod
                             } Into g = Group
                    Order By x.data_accettazione, x.nr_interno, g.Min(Function(r) r.mov_det.Extra_Str)
                    Select New With
                        {
                            .data_accettazione = x.data_accettazione,
                            .ora = x.ora,
                            .ora_accettazione = "",
                            .data_ddt_fornitore = x.data_ddt_fornitore,
                            .nr_interno = x.nr_interno,
                            .codice_fornitore = x.codice_fornitore,
                            .num_ddt_fornitore = x.num_ddt_fornitore,
                            .Linea_Cod_Des = x.Linea_Cod_Des,
                            .tipo = 0,
                            .specie = "",
                            .varieta = "",
                            .caratteristica = "",
                            .ocaratteristica = x.ocaratteristica,
                            .oaltrecaratteristiche = x.oaltrecaratteristiche,
                            .ocalibro = x.ocalibro,
                             .Lotto = x.lotto,
                            .kg_lordi = g.Sum(Function(r) r.mov_det.Qta_Extra_Totale + r.mov_det.Tara),
                            .cod_imballaggio = x.cod_imballaggio,
                            .cod_contenitore = x.cod_contenitore,
                            .cod_confezione = x.cod_confezione,
                            .num_confezioni = 0,
                            .num_contenitori = g.Sum(Function(r) r.mov_det.Qta_Dettaglio1),
                            .num_imballi = g.Sum(Function(r) r.mov_det.Qta_Dettaglio2),
                            .qualita = "",
                            .qualita_codiceorigine = x.qualita_codiceorigine,
                            .qta = g.Sum(Function(r) r.mov_det.Qta),
                            .udm_cod = x.udm_cod
                              }


                'Order By mov.Data_Movimento, mov.Doc_Numero, mov_det.Extra_Str
                'Select New With
                '    {
                '        .data_accettazione = mov.Data_Movimento,
                '        .ora = mov.Ora,
                '        .ora_accettazione = "",
                '        .data_ddt_fornitore = mov_for.Data_Movimento,
                '        .nr_interno = mov.Doc_Numero,
                '        .codice_fornitore = r_u.Settore_Des,
                '        .num_ddt_fornitore = mov_for.Doc_Numero_Sin & CStr(mov_for.Doc_Numero) & mov_for.Doc_Numero_Des,
                '        .Linea_Cod_Des = linee_prod.Linea_Cod_Des,
                '        .tipo = 0,
                '        .specie = "",
                '        .varieta = "",
                '        .caratteristica = "",
                '        .ocaratteristica = If(_otp_caratter Is Nothing, "", _otp_caratter.Codice_Origine),
                '        .oaltrecaratteristiche = If(_otp_altrecaratter Is Nothing, "", _otp_altrecaratter.Codice_Origine),
                '        .ocalibro = If(_otp_cal Is Nothing, "", _otp_cal.Codice_Origine),
                '         .Lotto = mov_det.Lotto,
                '        .kg_lordi = mov_det.Qta_Extra_Totale + mov_det.Tara,
                '        .cod_imballaggio = If(_otp_imballo Is Nothing, "", _otp_imballo.Codice_Origine),
                '        .cod_contenitore = If(_otp_contenitore Is Nothing, "", _otp_contenitore.Codice_Origine),
                '        .cod_confezione = If(_otp_confez Is Nothing, "", _otp_confez.Codice_Origine),
                '        .num_confezioni = 0,
                '        .num_contenitori = mov_det.Qta_Dettaglio1,
                '        .num_imballi = mov_det.Qta_Dettaglio2,
                '        .qualita = "",
                '        .qualita_codiceorigine = If(_otp_qual Is Nothing, "0", _otp_qual.Codice_Origine),
                '        .qta = mov_det.Qta,
                '        .udm_cod = mov_det.Udm_Cod,
                '        .Extra_Str = mov_det.Extra_Str
                '          }



                'Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                'risposta = JsonConvert.SerializeObject(RigheConferimento.ToList(), Formatting.None, serializerSettings)

                Dim ut As New Gias_EF_Utility
                DT = ut.ObjectQueryToDataTable(RigheConferimento.ToList())


            End Using


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        'Return risposta
        Return DT

    End Function

    '##############################################################################################
    Public Function LeggiTestataConferimenti(ByVal piva As String,
                                             ByVal _dataMovDal As String,
                                             ByVal _dataMovAl As String,
                                             ByVal _fornitori As String(),
                                             ByVal _operazioni As Integer(),
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As String

        Dim risposta As String = ""
        'Dim DT As DataTable

        Dim _filtroSuFornitori As Boolean = False
        If _fornitori IsNot Nothing AndAlso _fornitori.Length > 0 Then
            _filtroSuFornitori = True
        End If

        Dim _filtroSuOperazioni As Boolean = False
        If _operazioni IsNot Nothing AndAlso _operazioni.Length > 0 Then
            _filtroSuOperazioni = True
        End If

        Dim dataMovDalDateTime As Nullable(Of DateTime)
        dataMovDalDateTime = Nothing
        If Not String.IsNullOrEmpty(_dataMovDal) Then
            dataMovDalDateTime = Convert.ToDateTime(_dataMovDal)
        End If

        Dim dataMovAlDateTime As Nullable(Of DateTime)
        dataMovAlDateTime = Nothing
        If Not String.IsNullOrEmpty(_dataMovAl) Then
            dataMovAlDateTime = Convert.ToDateTime(_dataMovAl)
        End If

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.LeggiTestataConferimenti()"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim movimenti As DbSet(Of Movimenti) = GiasContext.Movimenti
            Dim agenda As DbSet(Of Agenda) = GiasContext.Agenda
            Dim contatti As DbSet(Of Contatti) = GiasContext.Contatti
            Dim risorse_umane As DbSet(Of Risorse_Umane) = GiasContext.Risorse_Umane

            Dim lavCodConf As Integer?() = {LAVCOD_DISTINTA_CARICO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_BOLLA_RICEVUTA, LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE}

            Dim RigheConferimento =
                From ag In agenda
                Join mov In movimenti
                    On ag.PIVA Equals mov.PIVA And
                     ag.Id_Agenda Equals mov.Id_Agenda
                Join mov_for In movimenti
                    On ag.PIVA Equals mov_for.PIVA And
                     ag.Id_Agenda Equals mov_for.Id_Agenda
                Join r_u In risorse_umane
                    On mov.Cod_RisUm Equals r_u.Cod_RisUm
                Join co In contatti
                    On co.Cod_Contatto Equals r_u.Cod_Contatto And co.Piva Equals r_u.Piva
                Where ag.PIVA.Equals(piva) AndAlso
                    ag.Modulo = enum_Omni_Modulo_Generazione.FreshFood AndAlso
                    ag.Stato_Export = 0 AndAlso
                    mov.Cau_Mov = CAU_REGISTRAZIONI_TERZIARIA AndAlso
                    mov_for.Cau_Mov = CAU_REGISTRAZIONE_SECONDARIA AndAlso
                    (String.IsNullOrEmpty(_dataMovDal) OrElse mov.Data_Movimento >= dataMovDalDateTime) AndAlso
                    (String.IsNullOrEmpty(_dataMovAl) OrElse mov.Data_Movimento <= dataMovAlDateTime) AndAlso
                    ((_filtroSuOperazioni = False) OrElse _operazioni.Contains(ag.Lav_Cod)) AndAlso
                    ((_filtroSuFornitori = False) OrElse _fornitori.Contains(co.Cod_Contatto))
                Order By mov.Data_Movimento, mov.Doc_Numero
                Select New With
                    {
                        ag.Id_Agenda,
                         mov.Data_Movimento,
                        .nr_interno = mov.Doc_Numero,
                        .data_ddt_fornitore = mov_for.Data_Movimento,
                        .num_ddt_fornitore = mov_for.Doc_Numero_Sin & CStr(mov_for.Doc_Numero) & mov_for.Doc_Numero_Des,
                        .ragsoc_fornitore = co.Rag_Soc,
                        .cod_fornitore = r_u.Settore_Des,
                        .ConferimentoOAcquisto = If(r_u.Cod_Rapporto = CInt(enum_Rapporti_Contabili_Standard.Conferente), "Conferimento", "Acquisto")
                     }

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            risposta = JsonConvert.SerializeObject(RigheConferimento.ToList(), Formatting.None, serializerSettings)

            'Dim ut As New Gias_EF_Utility
            'DT = ut.ObjectQueryToDataTable(RigheConferimento)


        End Using

        Return risposta
        'Return DT

    End Function



    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function ExportJDEdwards_Leggi_NewConf(ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Data_Inizio_Accett As Date,
                                                ByVal Data_Fine_Accett As Date,
                                                ByVal Str_filtro_IdAgenda As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.ExportJDEdwards_Leggi_NewConf"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        If Str_filtro_IdAgenda = "" Then
            Throw New Exception("Str_filtro_IdAgenda parametro obbligatorio")
        End If

        Try
            StbSQL.Length = 0

            '/*     INIZIO SPECIE NO POMODORO   */
            StbSQL.AppendLine(" ( ")
            StbSQL.AppendLine(" SELECT ")
            StbSQL.AppendLine(" Agenda.Piva, Agenda.Sa_Cod, Agenda.ID_Agenda, Agenda.Stato_Export, Agenda.Blocco_Flag, Agenda.Blocco_Data, Agenda.Blocco_Username, Agenda.Tipo_Accettazione, ")
            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Sin AS Prefisso_Bolla, Mov_Accett.Doc_Numero AS Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Suffisso_Bolla,   ")
            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Visualizzato, Mov_Accett.Data_Movimento AS Data_Bolla, ")
            StbSQL.AppendLine(" RisUm_Conf.Settore_Des AS Codice_Conferente, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Id_Mov_Det, MP_Raccolta.codice_esterno AS Codice_Articolo, Mov_Dett_Raccolta.Lotto, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Udm_Cod, Mov_Dett_Raccolta.Qta AS Peso_Netto, Mov_Dett_Raccolta.Variazione AS Degrado_Perc,")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Prezzo_Unitario_Netto AS Prezzo, 0 AS Prezzo_Contratto, ")
            StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Mov_Conf.Doc_Numero)) + Mov_Conf.Doc_Numero_Des AS Numero_DDT,   ")
            StbSQL.AppendLine(" Mov_Conf.Data_Movimento AS Data_DDT, ")

            StbSQL.AppendLine(" CA_Codici.val_cod AS Codice_Stabilimento ")

            StbSQL.AppendLine(" , 0 AS GradoBrix, 0 AS Franchigia_DifMaggiori, 0 AS Coefficiente_DifMinori, 0 AS Tot_Dif_Magg, 0 AS Tot_Dif_Min, '0' AS Premio_PomoTardivo ")
            StbSQL.AppendLine(" , Mov_Dett_Raccolta.ordine_det ")

            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane RisUm_Conf ON RisUm_Conf.Cod_RisUm = Mov_Accett.Cod_RisUm")
            StbSQL.AppendLine(" INNER JOIN Centri_Aziendali_Codici CA_Codici ON CA_Codici.Piva = Mov_Dett_Raccolta.PIVA AND CA_Codici.Sa_Cod = Mov_Dett_Raccolta.SA_COD  ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione <> -1 ")

            'modifica rispetto a vecchio conf
            'StbSQL.AppendLine(" AND Fabbr_Raccolta_Codici.Id_Cod = " & Agro_SQL_SaveNum(CA_COD_STABILIMENTO) & " " )
            StbSQL.AppendLine(" AND CA_Codici.Id_Cod = " & CInt(enum_CodiciAnagrafe.Centro_CodiceStabilimento) & " ")

            'bisogna escludere i beni confezionamento e la riga descrizione libera
            StbSQL.AppendLine(" AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")

            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            StbSQL.AppendLine(" AND Mov_Conf.Cau_Mov = '" & CAU_REGISTRAZIONI_ALLEGATE & "'")

            'modifica rispetto a vecchio conf
            'StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" + Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) + "'" )
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine_Accett) & " ")
            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio_Accett) & " ")

            If Sa_Cod <> 0 Then
                StbSQL.AppendLine(" AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Str_filtro_IdAgenda <> "" Then
                StbSQL.AppendLine(" AND Agenda.Id_agenda IN ( " & Agro_SQL_Save_Clausola_IN(Str_filtro_IdAgenda) & " )  ")
            End If

            ''--------------------------------------------------------------------------
            'If xFiltroAggiuntivo <> "" Then
            '    StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If
            ''--------------------------------------------------------------------------

            StbSQL.AppendLine(" ) ")
            '/*     FINE SPECIE NO POMODORO   */

            StbSQL.AppendLine(" UNION ALL ")

            '/*     INIZIO POMODORO  (sarà da modificare) */
            StbSQL.AppendLine(" ( ")


            StbSQL.AppendLine(" SELECT ")
            StbSQL.AppendLine(" Agenda.Piva, Agenda.Sa_Cod, Agenda.ID_Agenda, Agenda.Stato_Export, Agenda.Blocco_Flag, Agenda.Blocco_Data, Agenda.Blocco_Username, Agenda.Tipo_Accettazione, ")
            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Sin AS Prefisso_Bolla, Mov_Accett.Doc_Numero AS Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Suffisso_Bolla,  ")
            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Visualizzato, Mov_Accett.Data_Movimento AS Data_Bolla, ")
            StbSQL.AppendLine(" RisUm_Conf.Settore_Des AS Codice_Conferente, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Id_Mov_Det, MP_Raccolta.codice_esterno AS Codice_Articolo, Mov_Dett_Raccolta.Lotto, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Udm_Cod, Mov_Dett_Raccolta.Qta AS Peso_Netto, Mov_Dett_Raccolta.Variazione AS Degrado_Perc,")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Prezzo_Unitario_Netto AS Prezzo, Mov_Dett_Raccolta.Prezzo_Unitario AS Prezzo_Contratto,  ")
            StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Mov_Conf.Doc_Numero)) + Mov_Conf.Doc_Numero_Des AS Numero_DDT,   ")
            StbSQL.AppendLine(" Mov_Conf.Data_Movimento AS Data_DDT, ")

            StbSQL.AppendLine(" CA_Codici.val_cod AS Codice_Stabilimento ")

            StbSQL.AppendLine(" , MPC_brix.gradobrix ")
            StbSQL.AppendLine(" , ISNULL(ICF.Valore5, 3) AS Franchigia_DifMaggiori, ISNULL(ICF.Valore7, 0.5) AS Coefficiente_DifMinori ")
            StbSQL.AppendLine(" , MPC_1.Tot_Dif_Magg, MPC_2.Tot_Dif_Min, Mov_Dett_Conf.Premio_Complessivo AS Premio_PomoTardivo ")
            StbSQL.AppendLine(" , Mov_Dett_Raccolta.ordine_det ")

            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            'Mov_Certificato non esiste nel nuovo conferimento
            'StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   ")
            'cambiato join con la Mov_Dettaglio_Tecnico_Extra
            StbSQL.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Mdt_Extra ON Mdt_Extra.PIVA = Mov_Accett.PIVA AND Mdt_Extra.Sa_Cod = Mov_Accett.Sa_Cod  ")
            StbSQL.AppendLine(" AND Mdt_Extra.Id_Agenda = Mov_Accett.Id_Agenda AND Mdt_Extra.Id_Mov = Mov_Accett.Id_Mov ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine(" INNER JOIN Mov_Dettaglio_Conferimento Mov_Dett_Conf ON Mov_Dett_Conf.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Dett_Conf.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Dett_Conf.Id_Mov = Mov_Dett_Raccolta.Id_Mov AND Mov_Dett_Conf.Id_Mov_Det = Mov_Dett_Raccolta.Id_Mov_Det ")
            StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det ")
            ' StbSQL.AppendLine(" INNER JOIN Fabbricati Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Fabbricato_Cod " )
            'StbSQL.AppendLine(" INNER JOIN Fabbricati_Codici Fabbr_Raccolta_Codici ON Fabbr_Raccolta_Codici.Piva = Fabbr_Raccolta.PIVA AND Fabbr_Raccolta_Codici.Sa_Cod = Fabbr_Raccolta.SA_COD AND Fabbr_Raccolta_Codici.Fabbricato_Cod = Fabbr_Raccolta.Fabbricato_Cod " )
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane RisUm_Conf ON RisUm_Conf.Cod_RisUm = Mov_Accett.Cod_RisUm")

            'TEMP
            'StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " )
            'StbSQL.AppendLine(" INNER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  " )
            'StbSQL.AppendLine(" INNER JOIN SpecieVegetali ON SpecieVegetali.veg_cod = MP_Raccolta.Veg_Cod  " )

            StbSQL.AppendLine(" LEFT OUTER JOIN Imprese_Contratto_Fasi ICF ON ICF.PIVA = Mov_Dett_Raccolta.PIVA AND ICF.Elem_Cod = Mov_Dett_Raccolta.Elem_Cod  ")
            StbSQL.AppendLine(" AND ICF.pro_Cod = Mov_Dett_Raccolta.pro_Cod AND ICF.mat_Cod = Mov_Dett_Raccolta.mat_Cod AND ICF.Progetto_Cod = Mov_Dett_Raccolta.cod_progetto ")
            StbSQL.AppendLine(" AND ICF.fase_Cod = Mov_Dett_Raccolta.fase_Cod AND ICF.lotto = Mov_Dett_Raccolta.Lotto  ")
            'StbSQL.AppendLine(" -- non join sul cal_cod! in ICF è 0, mentre nei dettagli è valorizzato con il progressivo negativo " )
            'StbSQL.AppendLine(" -- AND ICF.cal_cod = Mov_Dett_Raccolta.cal_cod" )
            StbSQL.AppendLine(" AND ICF.udm_cod= Mov_Dett_Raccolta.udm_cod ")

            StbSQL.AppendLine(" INNER JOIN (SELECT progressivo, CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.')) AS gradobrix  ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'ogradobrix'  ")
            'StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod IN (0) ")
            StbSQL.AppendLine("             ) AS MPC_brix ON MPC_brix.progressivo = Mov_Dett_Raccolta.cal_cod  ")

            StbSQL.AppendLine(" INNER JOIN (SELECT progressivo, SUM(CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.'))) AS Tot_Dif_Magg  ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo IN ('oinerti','omarcio','overde') ")
            'StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod IN (0) ")
            StbSQL.AppendLine("             GROUP BY progressivo) AS MPC_1 on MPC_1.progressivo = Mov_Dett_Raccolta.cal_cod  ")

            StbSQL.AppendLine(" INNER JOIN (SELECT progressivo, SUM(CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.'))) AS Tot_Dif_Min  ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo IN ('ofruttiimmaturi','ofruttilesionati','ofruttischiacciati','ofruttiscottati')  ")
            'StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod IN (0) ")
            StbSQL.AppendLine("             GROUP BY progressivo) AS MPC_2 on MPC_2.progressivo = Mov_Dett_Raccolta.cal_cod  ")

            StbSQL.AppendLine(" INNER JOIN Centri_Aziendali_Codici CA_Codici ON CA_Codici.Piva = Mov_Dett_Raccolta.PIVA AND CA_Codici.Sa_Cod = Mov_Dett_Raccolta.SA_COD  ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione = -1 ")

            '0 = NO SURGELATO
            'StbSQL.AppendLine(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  " )

            'modifica rispetto a vecchio conf
            'StbSQL.AppendLine(" AND Fabbr_Raccolta_Codici.Id_Cod = " & Agro_SQL_SaveNum(CA_COD_STABILIMENTO) & " " )
            StbSQL.AppendLine(" AND CA_Codici.Id_Cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.Centro_CodiceStabilimento) & " ")

            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine(" AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")

            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            'StbSQL.AppendLine(" AND Mov_Certificato.Cau_Mov = '" & CAU_REGISTRAZIONI_TERZIARIA & "'")
            StbSQL.AppendLine(" AND Mov_Conf.Cau_Mov = '" & CAU_REGISTRAZIONI_ALLEGATE & "'")

            'modifica rispetto a vecchio conf
            'StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" + Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) + "'" )
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine_Accett) & " ")
            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio_Accett) & " ")

            If Sa_Cod <> 0 Then
                StbSQL.AppendLine(" AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Str_filtro_IdAgenda <> "" Then
                StbSQL.AppendLine(" AND Agenda.Id_agenda IN ( " & Agro_SQL_Save_Clausola_IN(Str_filtro_IdAgenda) & " )  ")
            End If

            ''--------------------------------------------------------------------------
            'If xFiltroAggiuntivo <> "" Then
            '    StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If
            ''--------------------------------------------------------------------------

            StbSQL.AppendLine(" ) ")
            '/*     FINE POMODORO   */

            'If xOrderBy <> "" Then
            'StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'Else
            StbSQL.AppendLine(" ORDER BY Data_Bolla, Numero_Bolla, ordine_det ASC ")
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


    '#####################################################################
    'OBSOLETA: prima versione della query di ricerca bolle per export JDE
    'se non si vuole filtrare per prezzo_valorizzato o per degrado_valorizzato, passare -1
    Public Function ExportJDEdwards_LeggiTestata_NewConf(ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal Data_Inizio_Accett As Date,
                                                        ByVal Data_Fine_Accett As Date,
                                                        ByVal ElencoStatiEsportazione As String,
                                                         ByVal prezzo_valorizzato As Integer,
                                                         ByVal degrado_valorizzato As Integer,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.ExportJDEdwards_LeggiTestata_NewConf"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT * FROM  ")

            '/*     INIZIO SPECIE NO POMODORO   */
            StbSQL.AppendLine(" ( ")
            StbSQL.AppendLine(" SELECT ")
            StbSQL.AppendLine(" Agenda.Piva, Agenda.Des_lib, Agenda.ID_Agenda, Agenda.Stato_Export, Agenda.Blocco_Flag, Agenda.Blocco_Data, Agenda.Blocco_Username, Agenda.Tipo_Accettazione, ")
            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Sin AS Prefisso_Bolla, Mov_Accett.Doc_Numero AS Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Suffisso_Bolla,  ")
            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Visualizzato, Mov_Accett.Data_Movimento AS Data_Bolla, ")
            StbSQL.AppendLine(" RisUm_Conf.Settore_Des AS Codice_Conferente, contatti_Conf.rag_soc AS conferente, ")
            StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Mov_Conf.Doc_Numero)) + Mov_Conf.Doc_Numero_Des AS Numero_DDT,   ")
            StbSQL.AppendLine(" Mov_Conf.Data_Movimento AS Data_DDT ")

            ''  NON SERVER IL COD STABILIMENTO QUI
            ''in attesa di sapere l'id_cod metto 1500 per test
            ''StbSQL.AppendLine(" , 1500 AS Codice_Stabilimento " )
            'StbSQL.AppendLine(" , CA_Codici.val_cod AS Codice_Stabilimento " )
            'StbSQL.AppendLine(" INNER JOIN Centri_Aziendali_Codici CA_Codici ON CA_Codici.Piva = Mov_Dett_Raccolta.PIVA AND CA_Codici.Sa_Cod = Mov_Dett_Raccolta.SA_COD  " )
            'StbSQL.AppendLine(" AND CA_Codici.Id_Cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.Centro_CodiceStabilimento) & " " )

            StbSQL.AppendLine(" , ISNULL((select TOP 1  sa_nome  ")
            StbSQL.AppendLine("            FROM Centri_Aziendali  ")
            StbSQL.AppendLine("             INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Dett_Raccolta.piva = Centri_Aziendali.piva AND  Mov_Dett_Raccolta.sa_cod = Centri_Aziendali.sa_cod  ")
            StbSQL.AppendLine("             INNER JOIN  Movimenti Mov_Raccolta  ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine("             WHERE Agenda.PIVA = Mov_Raccolta.PIVA  ")
            StbSQL.AppendLine("              AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod ")
            StbSQL.AppendLine("             AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            StbSQL.AppendLine("             AND Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")
            StbSQL.AppendLine("             AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")
            If Sa_Cod <> 0 Then
                StbSQL.AppendLine("         AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If
            StbSQL.AppendLine("  ), '' ) as Sa_nome ")

            StbSQL.AppendLine("  , isnull( (SELECT TOP 1 codice_esterno + ' ' + mat_des  ")
            StbSQL.AppendLine("             FROM Movimenti Mov_Raccolta  ")
            StbSQL.AppendLine("             INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine("             INNER JOIN Materie_Prime ON Materie_Prime.mat_cod = Mov_Dett_Raccolta.mat_cod AND Materie_Prime.elem_cod = Mov_Dett_Raccolta.elem_cod             ")
            StbSQL.AppendLine("             WHERE Agenda.PIVA = Mov_Raccolta.PIVA  ")
            StbSQL.AppendLine("              AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod ")
            StbSQL.AppendLine("             AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            StbSQL.AppendLine("             AND Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")
            StbSQL.AppendLine("             AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")
            If Sa_Cod <> 0 Then
                StbSQL.AppendLine("         AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If
            'devo applicare lo stesso order by a tutte le query interne che vanno sui dettagli, per avere i dati riferiti allo stesso dettaglio
            'nel caso ci sia un dettaglio solo non cambia nulla, se ci sono più dettagli visualizza il primo (x mat_cod) che ha il degrado inferiore 
            'per dare risalto a se c'è qualcosa con degrado = 0
            StbSQL.AppendLine("             ORDER BY variazione, Mov_Dett_Raccolta.mat_cod ASC  ")
            StbSQL.AppendLine(" ), 0   ) as articolo ")
            StbSQL.AppendLine(vbCrLf)

            StbSQL.AppendLine("  , isnull( (SELECT TOP 1 CASE WHEN Prezzo_Unitario_Netto = 0 THEN 0  ")
            StbSQL.AppendLine("                         WHEN Prezzo_Unitario_Netto > 0 THEN 1   ")
            StbSQL.AppendLine("                         END ")
            StbSQL.AppendLine("             FROM Movimenti Mov_Raccolta  ")
            StbSQL.AppendLine("             INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine("             WHERE Agenda.PIVA = Mov_Raccolta.PIVA  ")
            StbSQL.AppendLine("              AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod ")
            StbSQL.AppendLine("             AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            StbSQL.AppendLine("             AND Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")
            StbSQL.AppendLine("             AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")
            If Sa_Cod <> 0 Then
                StbSQL.AppendLine("         AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If
            'devo applicare lo stesso order by a tutte le query interne che vanno sui dettagli, per avere i dati riferiti allo stesso dettaglio
            'nel caso ci sia un dettaglio solo non cambia nulla, se ci sono più dettagli visualizza il primo (x mat_cod) che ha il degrado inferiore 
            'per dare risalto a se c'è qualcosa con degrado = 0
            StbSQL.AppendLine("             ORDER BY variazione, Mov_Dett_Raccolta.mat_cod ASC  ")
            StbSQL.AppendLine(" ), 0   ) as prezzo_valorizzato ")
            StbSQL.AppendLine(vbCrLf)

            StbSQL.AppendLine("  , isnull( (SELECT TOP 1 variazione  ")
            StbSQL.AppendLine("             FROM Movimenti Mov_Raccolta  ")
            StbSQL.AppendLine("             INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine("             WHERE Agenda.PIVA = Mov_Raccolta.PIVA  ")
            StbSQL.AppendLine("              AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod ")
            StbSQL.AppendLine("             AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            StbSQL.AppendLine("             AND Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")
            StbSQL.AppendLine("             AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")
            If Sa_Cod <> 0 Then
                StbSQL.AppendLine("         AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If
            'devo applicare lo stesso order by a tutte le query interne che vanno sui dettagli, per avere i dati riferiti allo stesso dettaglio
            'nel caso ci sia un dettaglio solo non cambia nulla, se ci sono più dettagli visualizza il primo (x mat_cod) che ha il degrado inferiore 
            'per dare risalto a se c'è qualcosa con degrado = 0
            StbSQL.AppendLine("             ORDER BY variazione, Mov_Dett_Raccolta.mat_cod ASC  ")
            StbSQL.AppendLine(" ), 0   ) as degrado_perc ")
            StbSQL.AppendLine(vbCrLf)

            'StbSQL.AppendLine("                          ")
            'StbSQL.AppendLine("                          ")
            'StbSQL.AppendLine("                          ")


            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane RisUm_Conf ON RisUm_Conf.Cod_RisUm = Mov_Accett.Cod_RisUm ")
            StbSQL.AppendLine(" INNER JOIN contatti contatti_Conf ON RisUm_Conf.piva = contatti_Conf.piva AND RisUm_Conf.cod_contatto = contatti_Conf.cod_contatto ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione <> -1 ")

            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            StbSQL.AppendLine(" AND Mov_Conf.Cau_Mov = '" & CAU_REGISTRAZIONI_ALLEGATE & "'")

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine_Accett) & " ")
            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio_Accett) & " ")

            If Not String.IsNullOrEmpty(ElencoStatiEsportazione) Then
                StbSQL.AppendLine(" AND Agenda.Stato_Export IN (" & Agro_SQL_Save_Clausola_IN(ElencoStatiEsportazione) & ") ")
            End If

            StbSQL.AppendLine(" AND EXISTS ( ")
            StbSQL.AppendLine("             SELECT 1 ")
            StbSQL.AppendLine("             FROM Movimenti Mov_Raccolta ")
            StbSQL.AppendLine("             INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov  ")
            StbSQL.AppendLine("             WHERE Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine("             AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")
            If Sa_Cod <> 0 Then
                StbSQL.AppendLine("         AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If
            'modifica rispetto a vecchio conf
            'StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" + Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) + "'" )
            StbSQL.AppendLine("             AND Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")

            StbSQL.AppendLine("             ) ")

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StbSQL.AppendLine(" ) AS Bolle_Conf_Testata ")
            '/*     FINE SPECIE NO POMODORO   */

            'StbSQL.AppendLine(" UNION ALL " )

            ''/*     INIZIO POMODORO  (sarà da modificare) */
            'StbSQL.AppendLine(" ( " )


            'StbSQL.AppendLine(" SELECT " )
            'StbSQL.AppendLine(" Agenda.Piva, Agenda.Sa_Cod, Agenda.ID_Agenda, Agenda.Stato_Export, Agenda.Blocco_Flag, Agenda.Blocco_Data, Agenda.Blocco_Username, Agenda.Tipo_Accettazione, " )
            'StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Sin AS Prefisso_Bolla, Mov_Accett.Doc_Numero AS Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Suffisso_Bolla,  " )
            'StbSQL.AppendLine(" Mov_Accett.Data_Movimento AS Data_Bolla, ")
            ''StbSQL.AppendLine(" ImpCod_Conf.val_cod AS Codice_Conferente, " )
            'StbSQL.AppendLine(" RisUm_Conf.Settore_Des AS Codice_Conferente, " )
            'StbSQL.AppendLine(" MP_Raccolta.Cod_Articolo AS Codice_Articolo, Mov_Dett_Raccolta.Lotto, " )
            'StbSQL.AppendLine(" Mov_Dett_Raccolta.Udm_Cod, Mov_Dett_Raccolta.Qta AS Peso_Netto, Mov_Dett_Raccolta.Variazione AS Degrado_Perc," )
            'StbSQL.AppendLine(" Mov_Dett_Raccolta.Prezzo_Unitario_Netto AS Prezzo, Mov_Dett_Raccolta.Prezzo_Unitario AS Prezzo_Contratto,  " )
            'StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Mov_Conf.Doc_Numero)) + Mov_Conf.Doc_Numero_Des AS Numero_DDT,   ")
            'StbSQL.AppendLine(" Mov_Conf.Data_Movimento AS Data_DDT, " )

            ''in attesa di sapere l'id_cod metto 1500 per test
            'StbSQL.AppendLine(" 1500 AS Codice_Stabilimento, " )
            ''StbSQL.AppendLine(" CA_Codici.val_cod AS Codice_Stabilimento, " )

            'StbSQL.AppendLine("         ISNULL( (SELECT TOP 1 VAL_COD + '_' + REPLACE( CONVERT(Varchar(500),  MPXPrezzi.Prezzo	), '.',',' )  " )
            'StbSQL.AppendLine("                     FROM Materie_Prime_Campionature MP_Camp_Indice " )
            'StbSQL.AppendLine("                     INNER JOIN ParametriQualitativiXPrezzi MPXPrezzi " )
            'StbSQL.AppendLine("                     ON    MP_Camp_Indice.tipo = MPXPrezzi.tipo " )
            'StbSQL.AppendLine("                     AND MP_Camp_Indice.tipo_cod = MPXPrezzi.tipo_cod " )
            'StbSQL.AppendLine("                     AND MP_Camp_Indice.udm_cod = MPXPrezzi.udm_cod " )
            'StbSQL.AppendLine("                     WHERE MP_Camp_Indice.progressivo = Mov_Dett_Raccolta.cal_cod " )
            'StbSQL.AppendLine("                     AND MPXPrezzi.Piva_SuperUser = '" & Agro_SQL_SaveText(Piva) & "' " )
            'StbSQL.AppendLine("                     AND MPXPrezzi.Piva = Mov_Dett_Raccolta.Piva  " )
            'StbSQL.AppendLine("                     AND MP_Camp_Indice.tipo = 'indice' " )
            'StbSQL.AppendLine("                     AND MPXPrezzi.validita_inizio <= Mov_Accett.Data_Movimento " )
            'StbSQL.AppendLine("                     AND MPXPrezzi.validita_fine >= Mov_Accett.Data_Movimento " )
            'StbSQL.AppendLine("                     AND CONVERT(FLOAT, replace( MP_Camp_Indice.val_cod ,',', '.') )  >= MPXPrezzi.VALORE_MIN " )
            'StbSQL.AppendLine("                     AND CONVERT(FLOAT,replace( MP_Camp_Indice.val_cod ,',', '.') )  <= MPXPrezzi.VALORE_Max " )
            'StbSQL.AppendLine("                     ), '_' ) AS GradoBrix_IndicePrezzo, " )

            'StbSQL.AppendLine("         ISNULL(ICF.Valore5, 3) AS Franchigia_DifMaggiori, ISNULL(ICF.Valore7, 0.5) AS Coefficiente_DifMinori " )
            'StbSQL.AppendLine(" , MPC_1.Tot_Dif_Magg, MPC_2.Tot_Dif_Min, Mov_Certificato.extra_Str AS Premio_PomoTardivo " )

            'StbSQL.AppendLine(" FROM Agenda " )
            'StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda " )
            'StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   " )
            'StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda " )
            'StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda " )
            'StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov " )
            'StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det " )
            '' StbSQL.AppendLine(" INNER JOIN Fabbricati Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Fabbricato_Cod " )
            ''StbSQL.AppendLine(" INNER JOIN Fabbricati_Codici Fabbr_Raccolta_Codici ON Fabbr_Raccolta_Codici.Piva = Fabbr_Raccolta.PIVA AND Fabbr_Raccolta_Codici.Sa_Cod = Fabbr_Raccolta.SA_COD AND Fabbr_Raccolta_Codici.Fabbricato_Cod = Fabbr_Raccolta.Fabbricato_Cod " )
            'StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " )
            'StbSQL.AppendLine(" INNER JOIN Risorse_Umane RisUm_Conf ON RisUm_Conf.Cod_RisUm = Mov_Accett.Cod_RisUm" )

            ''TEMP
            ''StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " )
            ''StbSQL.AppendLine(" INNER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  " )
            ''StbSQL.AppendLine(" INNER JOIN SpecieVegetali ON SpecieVegetali.veg_cod = MP_Raccolta.Veg_Cod  " )

            'StbSQL.AppendLine(" LEFT OUTER JOIN Imprese_Contratto_Fasi ICF ON ICF.PIVA = Mov_Dett_Raccolta.PIVA AND ICF.Elem_Cod = Mov_Dett_Raccolta.Elem_Cod  " )
            'StbSQL.AppendLine(" AND ICF.pro_Cod = Mov_Dett_Raccolta.pro_Cod AND ICF.mat_Cod = Mov_Dett_Raccolta.mat_Cod AND ICF.Progetto_Cod = Mov_Dett_Raccolta.cod_progetto " )
            'StbSQL.AppendLine(" AND ICF.fase_Cod = Mov_Dett_Raccolta.fase_Cod AND ICF.lotto = Mov_Dett_Raccolta.Lotto  " )
            ''StbSQL.AppendLine(" -- non join sul cal_cod! in ICF è 0, mentre nei dettagli è valorizzato con il progressivo negativo " )
            ''StbSQL.AppendLine(" -- AND ICF.cal_cod = Mov_Dett_Raccolta.cal_cod" )
            'StbSQL.AppendLine(" AND ICF.udm_cod= Mov_Dett_Raccolta.udm_cod " )

            'StbSQL.AppendLine(" INNER JOIN (SELECT progressivo, SUM(CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.'))) AS Tot_Dif_Magg  " )
            'StbSQL.AppendLine("             FROM Materie_Prime_Campionature " )
            'StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'danno'  " )
            'StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod IN (1,2,3) " )
            'StbSQL.AppendLine("             GROUP BY progressivo) AS MPC_1 on MPC_1.progressivo = Mov_Dett_Raccolta.cal_cod  " )

            'StbSQL.AppendLine(" INNER JOIN (SELECT progressivo, SUM(CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.'))) AS Tot_Dif_Min  " )
            'StbSQL.AppendLine("             FROM Materie_Prime_Campionature " )
            'StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'danno'  " )
            'StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod IN (4,5,6,7) " )
            'StbSQL.AppendLine("             GROUP BY progressivo) AS MPC_2 on MPC_2.progressivo = Mov_Dett_Raccolta.cal_cod  " )

            ''StbSQL.AppendLine(" INNER JOIN Centri_Aziendali_Codici CA_Codici ON CA_Codici.Piva = Mov_Dett_Raccolta.PIVA AND CA_Codici.Sa_Cod = Mov_Dett_Raccolta.SA_COD  " )

            'StbSQL.AppendLine(" WHERE Agenda.Lav_Cod IN (" & String.Join(", ", arrLavCodAccettazioniConf) & ")" )
            'StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione = -1 " )
            '''                                                           0 = NO SURGELATO
            ''StbSQL.AppendLine(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  " )

            ''modifica rispetto a vecchio conf
            '''StbSQL.AppendLine(" AND Fabbr_Raccolta_Codici.Id_Cod = " & Agro_SQL_SaveNum(CA_COD_STABILIMENTO) & " " )
            ''StbSQL.AppendLine(" AND CA_Codici.Id_Cod = " & Agro_SQL_SaveNum(????) & " " )

            ''novità new conf
            'StbSQL.AppendLine(" AND Mov_Dett_Raccolta.elem_cod <> " & BENI_CONFEZ_VEGETALE.ToString & " " )

            'StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov = '" + Agro_SQL_SaveText(CAU_REGISTRAZIONI) + "'" )
            'StbSQL.AppendLine(" AND Mov_Certificato.Cau_Mov = '" + Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) + "'" )
            'StbSQL.AppendLine(" AND Mov_Conf.Cau_Mov = '" + Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) + "'" )

            ''modifica rispetto a vecchio conf
            ''StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" + Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) + "'" )
            'StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" + Agro_SQL_SaveText(CAU_CARICO) + "'" )

            'StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' " )

            'StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine_Accett) & " " )
            'StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio_Accett) & " " )

            'If Sa_Cod <> 0 Then
            '    StbSQL.AppendLine(" AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " )
            'End If

            ''--------------------------------------------------------------------------
            'If xFiltroAggiuntivo <> "" Then
            '    StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            'End If
            ''--------------------------------------------------------------------------

            'StbSQL.AppendLine(" ) " )
            ''/*     FINE POMODORO   */

            StbSQL.AppendLine(" WHERE 1 = 1 ")

            If prezzo_valorizzato <> -1 Then
                StbSQL.AppendLine(" AND prezzo_valorizzato = " & Agro_SQL_SaveNum(CStr(prezzo_valorizzato)) & "")
            End If

            Select Case degrado_valorizzato
                Case 1
                    StbSQL.AppendLine(" AND degrado_perc > 0 ")
                Case 0
                    StbSQL.AppendLine(" AND degrado_perc = 0 ")
            End Select

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                StbSQL.AppendLine(" ORDER BY Data_Bolla, Numero_Bolla ASC ")
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

    '#####################################################################
    'se non si vuole filtrare per prezzo_valorizzato o per degrado_valorizzato, passare -1
    Public Function ExportJDEdwards_LeggiTestataDettagli_NewConf(ByVal Piva As String,
                                                                ByVal Sa_Cod As Integer,
                                                                ByVal Data_Inizio_Accett As Date,
                                                                ByVal Data_Fine_Accett As Date,
                                                                ByVal ElencoStatiEsportazione As String,
                                                                ByVal flag_invio_bolle_senza_degrado As Boolean,
                                                                ByVal xFiltroAggiuntivo As String,
                                                                ByVal xOrderBy As String,
                                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.ExportJDEdwards_LeggiTestataDettagli_NewConf"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        '     ByVal prezzo_valorizzato As Integer,

        Try
            StbSQL.Length = 0

            ''StbSQL.AppendLine(" SELECT * FROM  " )

            '/*     INIZIO SPECIE NO POMODORO   */
            StbSQL.AppendLine(" ( ")
            StbSQL.AppendLine(" -- BOLLE CONFERIMENTO NO POMODORO ")
            StbSQL.AppendLine(" SELECT ")
            StbSQL.AppendLine(" Agenda.Piva, Agenda.Des_lib, Agenda.ID_Agenda, Mov_Dett_Raccolta.Id_Mov_Det, Mov_Dett_Raccolta.Ordine_Det, Agenda.Stato_Export, Agenda.Blocco_Flag, Agenda.Blocco_Data, Agenda.Blocco_Username, Agenda.Tipo_Accettazione, ")
            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Sin AS Prefisso_Bolla, Mov_Accett.Doc_Numero AS Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Suffisso_Bolla,  ")
            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Visualizzato, Mov_Accett.Data_Movimento AS Data_Bolla, ")
            StbSQL.AppendLine(" RisUm_Conf.Settore_Des AS Codice_Conferente, contatti_Conf.rag_soc AS conferente, ")
            StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Mov_Conf.Doc_Numero)) + Mov_Conf.Doc_Numero_Des AS Numero_DDT,   ")
            StbSQL.AppendLine(" Mov_Conf.Data_Movimento AS Data_DDT ")
            StbSQL.AppendLine(" , Sa_nome ")
            StbSQL.AppendLine(" , codice_esterno + ' ' + mat_des AS articolo ")
            StbSQL.AppendLine(" , CASE WHEN Mov_Dett_Raccolta.Prezzo_Unitario_Netto = 0 THEN 0  ")
            StbSQL.AppendLine("         WHEN Mov_Dett_Raccolta.Prezzo_Unitario_Netto > 0 THEN 1   ")
            StbSQL.AppendLine("         END AS prezzo_valorizzato ")
            StbSQL.AppendLine(" , variazione AS degrado_perc ")

            StbSQL.AppendLine(" ,    (   SELECT COUNT(distinct  prezzo_valorizzato )  ")
            StbSQL.AppendLine("          FROM ( ")
            StbSQL.AppendLine("                 SELECT CASE WHEN Movimenti_Dettagli.Prezzo_Unitario_Netto = 0 THEN 0  ")
            StbSQL.AppendLine("                             WHEN Movimenti_Dettagli.Prezzo_Unitario_Netto > 0 THEN 1  ")
            StbSQL.AppendLine("                         END AS prezzo_valorizzato ")
            StbSQL.AppendLine("                 FROM   Movimenti_Dettagli ")
            StbSQL.AppendLine("                 WHERE Mov_Raccolta.PIVA = Movimenti_Dettagli.PIVA AND Mov_Raccolta.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Mov_Raccolta.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            StbSQL.AppendLine("                 AND Movimenti_Dettagli.elem_cod = " & TRASFORMATI_VEGETALI.ToString & "    ")
            StbSQL.AppendLine("                ) AS count_prezzo_dettagli ")
            StbSQL.AppendLine("      ) AS count_prezzo_dettagli     ")

            StbSQL.AppendLine(" , 0 AS grado_brix ")
            StbSQL.AppendLine(" , 0 AS Franchigia_DifMaggiori, 0 AS Coefficiente_DifMinori ")
            StbSQL.AppendLine(" , 0 AS Tot_Dif_Magg, 0 AS Tot_Dif_Min, 0 AS Premio_PomoTardivo ")
            StbSQL.AppendLine(" , Mov_Dett_Raccolta.ordine_det ")


            ''  NON SERVE IL COD STABILIMENTO QUI
            'StbSQL.AppendLine(" , CA_Codici.val_cod AS Codice_Stabilimento " )
            'StbSQL.AppendLine(" INNER JOIN Centri_Aziendali_Codici CA_Codici ON CA_Codici.Piva = Mov_Dett_Raccolta.PIVA AND CA_Codici.Sa_Cod = Mov_Dett_Raccolta.SA_COD  " )
            'StbSQL.AppendLine(" AND CA_Codici.Id_Cod = " & Agro_SQL_SaveNum(enum_CodiciAnagrafe.Centro_CodiceStabilimento) & " " )

            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane RisUm_Conf ON RisUm_Conf.Cod_RisUm = Mov_Accett.Cod_RisUm ")
            StbSQL.AppendLine(" INNER JOIN contatti contatti_Conf ON RisUm_Conf.piva = contatti_Conf.piva AND RisUm_Conf.cod_contatto = contatti_Conf.cod_contatto ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA  ")
            StbSQL.AppendLine(" AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime ON Materie_Prime.mat_cod = Mov_Dett_Raccolta.mat_cod AND Materie_Prime.elem_cod = Mov_Dett_Raccolta.elem_cod             ")
            StbSQL.AppendLine(" INNER JOIN Centri_Aziendali ON Mov_Dett_Raccolta.piva = Centri_Aziendali.piva AND  Mov_Dett_Raccolta.sa_cod = Centri_Aziendali.sa_cod  ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione <> -1 ")

            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            StbSQL.AppendLine(" AND Mov_Conf.Cau_Mov = '" & CAU_REGISTRAZIONI_ALLEGATE & "'")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine_Accett) & " ")
            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio_Accett) & " ")

            If Not String.IsNullOrEmpty(ElencoStatiEsportazione) Then
                StbSQL.AppendLine(" AND Agenda.Stato_Export IN (" & Agro_SQL_Save_Clausola_IN(ElencoStatiEsportazione) & ") ")
            End If

            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine("  AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")

            '20/01/2021: aggiunto per evitare che esportino bolle che contengono almeno un dettaglio con udm_cod sbagliata (udm <> kg)
            'questo non va bene nel caso di bolla con più dettagli: StbSQL.AppendLine("  AND Mov_Dett_Raccolta.udm_cod = " & CStr(enum_UnitaMisura.KG) & " " )
            StbSQL.AppendLine(" AND NOT EXISTS (           ")
            StbSQL.AppendLine("                 SELECT 1         ")
            StbSQL.AppendLine("                 FROM   Movimenti_Dettagli          ")
            StbSQL.AppendLine("                 WHERE Mov_Raccolta.PIVA = Movimenti_Dettagli.PIVA AND Mov_Raccolta.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Mov_Raccolta.Id_Mov = Movimenti_Dettagli.Id_Mov          ")
            StbSQL.AppendLine("                 AND Movimenti_Dettagli.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")
            StbSQL.AppendLine("                 AND Movimenti_Dettagli.udm_cod <> " & CStr(enum_UnitaMisura.KG) & " ")
            StbSQL.AppendLine("                 )         ")

            If Sa_Cod <> 0 Then
                StbSQL.AppendLine(" AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Not flag_invio_bolle_senza_degrado Then
                'devono essere visualizzate solo le bolle che hanno tutti i dettagli con degrado valorizzato
                StbSQL.AppendLine(" -- richiesta solo bolle con degrado valorizzato (flag_invio_bolle_senza_degrado = False )         ")
                StbSQL.AppendLine("     AND EXISTS (           ")
                StbSQL.AppendLine("                 SELECT 1         ")
                StbSQL.AppendLine("                 FROM   Movimenti_Dettagli          ")
                StbSQL.AppendLine("                 WHERE Mov_Raccolta.PIVA = Movimenti_Dettagli.PIVA AND Mov_Raccolta.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Mov_Raccolta.Id_Mov = Movimenti_Dettagli.Id_Mov          ")
                StbSQL.AppendLine("                 AND Movimenti_Dettagli.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")
                StbSQL.AppendLine("                 and variazione > 0         ")
                StbSQL.AppendLine("                 )         ")
                StbSQL.AppendLine("     AND NOT EXISTS (           ")
                StbSQL.AppendLine("                 SELECT 1         ")
                StbSQL.AppendLine("                 FROM   Movimenti_Dettagli          ")
                StbSQL.AppendLine("                 WHERE Mov_Raccolta.PIVA = Movimenti_Dettagli.PIVA AND Mov_Raccolta.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Mov_Raccolta.Id_Mov = Movimenti_Dettagli.Id_Mov          ")
                StbSQL.AppendLine("                 AND Movimenti_Dettagli.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")
                StbSQL.AppendLine("                 and variazione = 0         ")
                StbSQL.AppendLine("                 )         ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StbSQL.AppendLine(" ) --AS Bolle_Conf_Testata ")
            '/*     FINE SPECIE NO POMODORO   */

            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" UNION ALL ")
            StbSQL.AppendLine(" ")

            '/*     INIZIO POMODORO */
            StbSQL.AppendLine(" ( ")

            StbSQL.AppendLine(" SELECT ")
            StbSQL.AppendLine(" Agenda.Piva, Agenda.Des_lib, Agenda.ID_Agenda, Mov_Dett_Raccolta.Id_Mov_Det, Mov_Dett_Raccolta.Ordine_Det, Agenda.Stato_Export, Agenda.Blocco_Flag, Agenda.Blocco_Data, Agenda.Blocco_Username, Agenda.Tipo_Accettazione, ")
            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Sin AS Prefisso_Bolla, Mov_Accett.Doc_Numero AS Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Suffisso_Bolla,  ")
            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Visualizzato, Mov_Accett.Data_Movimento AS Data_Bolla, ")
            StbSQL.AppendLine(" RisUm_Conf.Settore_Des AS Codice_Conferente, contatti_Conf.rag_soc AS conferente, ")
            StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Sin + CONVERT(varchar(50), CONVERT(int, Mov_Conf.Doc_Numero)) + Mov_Conf.Doc_Numero_Des AS Numero_DDT,   ")
            StbSQL.AppendLine(" Mov_Conf.Data_Movimento AS Data_DDT ")
            StbSQL.AppendLine(" , Sa_nome ")
            StbSQL.AppendLine(" , codice_esterno + ' ' + mat_des AS articolo ")
            StbSQL.AppendLine(" , CASE WHEN Mov_Dett_Raccolta.Prezzo_Unitario_Netto = 0 THEN 0  ")
            StbSQL.AppendLine("         WHEN Mov_Dett_Raccolta.Prezzo_Unitario_Netto > 0 THEN 1   ")
            StbSQL.AppendLine("         END AS prezzo_valorizzato ")
            StbSQL.AppendLine(" , variazione AS degrado_perc ")

            StbSQL.AppendLine(" ,    (   SELECT COUNT(distinct  prezzo_valorizzato )  ")
            StbSQL.AppendLine("          FROM ( ")
            StbSQL.AppendLine("                 SELECT CASE WHEN Movimenti_Dettagli.Prezzo_Unitario_Netto = 0 THEN 0  ")
            StbSQL.AppendLine("                             WHEN Movimenti_Dettagli.Prezzo_Unitario_Netto > 0 THEN 1  ")
            StbSQL.AppendLine("                         END AS prezzo_valorizzato ")
            StbSQL.AppendLine("                 FROM   Movimenti_Dettagli ")
            StbSQL.AppendLine("                 WHERE Mov_Raccolta.PIVA = Movimenti_Dettagli.PIVA AND Mov_Raccolta.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Mov_Raccolta.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            StbSQL.AppendLine("                 AND Movimenti_Dettagli.elem_cod = " & TRASFORMATI_VEGETALI.ToString & "    ")
            StbSQL.AppendLine("                ) AS count_prezzo_dettagli ")
            StbSQL.AppendLine("      ) AS count_prezzo_dettagli     ")

            '  NON SERVE IL COD STABILIMENTO QUI
            'StbSQL.AppendLine(" CA_Codici.val_cod AS Codice_Stabilimento, " )

            StbSQL.AppendLine(" , MPC_brix.gradobrix  ")
            StbSQL.AppendLine(" , ISNULL(ICF.Valore5, 3) AS Franchigia_DifMaggiori, ISNULL(ICF.Valore7, 0.5) AS Coefficiente_DifMinori ")
            StbSQL.AppendLine(" , MPC_1.Tot_Dif_Magg, MPC_2.Tot_Dif_Min, Mov_Dett_Conf.Premio_Complessivo AS Premio_PomoTardivo ")
            StbSQL.AppendLine(" , Mov_Dett_Raccolta.ordine_det ")

            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            'Mov_Certificato non esiste nel nuovo conferimento
            'StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine(" INNER JOIN Mov_Dettaglio_Conferimento Mov_Dett_Conf ON Mov_Dett_Conf.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Dett_Conf.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Dett_Conf.Id_Mov = Mov_Dett_Raccolta.Id_Mov AND Mov_Dett_Conf.Id_Mov_Det = Mov_Dett_Raccolta.Id_Mov_Det ")
            StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det ")
            ' StbSQL.AppendLine(" INNER JOIN Fabbricati Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Fabbricato_Cod " )
            'StbSQL.AppendLine(" INNER JOIN Fabbricati_Codici Fabbr_Raccolta_Codici ON Fabbr_Raccolta_Codici.Piva = Fabbr_Raccolta.PIVA AND Fabbr_Raccolta_Codici.Sa_Cod = Fabbr_Raccolta.SA_COD AND Fabbr_Raccolta_Codici.Fabbricato_Cod = Fabbr_Raccolta.Fabbricato_Cod " )
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")
            StbSQL.AppendLine(" INNER JOIN Centri_Aziendali ON Mov_Dett_Raccolta.piva = Centri_Aziendali.piva AND  Mov_Dett_Raccolta.sa_cod = Centri_Aziendali.sa_cod  ")

            StbSQL.AppendLine(" INNER JOIN Risorse_Umane RisUm_Conf ON RisUm_Conf.Cod_RisUm = Mov_Accett.Cod_RisUm")
            StbSQL.AppendLine(" INNER JOIN contatti contatti_Conf ON RisUm_Conf.piva = contatti_Conf.piva AND RisUm_Conf.cod_contatto = contatti_Conf.cod_contatto ")

            'TEMP
            'StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod " )
            'StbSQL.AppendLine(" INNER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  " )
            'StbSQL.AppendLine(" INNER JOIN SpecieVegetali ON SpecieVegetali.veg_cod = MP_Raccolta.Veg_Cod  " )

            StbSQL.AppendLine(" LEFT OUTER JOIN Imprese_Contratto_Fasi ICF ON ICF.PIVA = Mov_Dett_Raccolta.PIVA AND ICF.Elem_Cod = Mov_Dett_Raccolta.Elem_Cod  ")
            StbSQL.AppendLine(" AND ICF.pro_Cod = Mov_Dett_Raccolta.pro_Cod AND ICF.mat_Cod = Mov_Dett_Raccolta.mat_Cod AND ICF.Progetto_Cod = Mov_Dett_Raccolta.cod_progetto ")
            StbSQL.AppendLine(" AND ICF.fase_Cod = Mov_Dett_Raccolta.fase_Cod AND ICF.lotto = Mov_Dett_Raccolta.Lotto  ")
            'StbSQL.AppendLine(" -- non join sul cal_cod! in ICF è 0, mentre nei dettagli è valorizzato con il progressivo negativo " )
            'StbSQL.AppendLine(" -- AND ICF.cal_cod = Mov_Dett_Raccolta.cal_cod" )
            StbSQL.AppendLine(" AND ICF.udm_cod= Mov_Dett_Raccolta.udm_cod ")

            StbSQL.AppendLine(" INNER JOIN (SELECT progressivo, CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.')) AS gradobrix  ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'ogradobrix'  ")
            StbSQL.AppendLine("             ) AS MPC_brix ON MPC_brix.progressivo = Mov_Dett_Raccolta.cal_cod  ")

            StbSQL.AppendLine(" INNER JOIN (SELECT progressivo, SUM(CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.'))) AS Tot_Dif_Magg  ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo IN ('oinerti','omarcio','overde') ")
            StbSQL.AppendLine("             GROUP BY progressivo) AS MPC_1 on MPC_1.progressivo = Mov_Dett_Raccolta.cal_cod  ")

            StbSQL.AppendLine(" INNER JOIN (SELECT progressivo, SUM(CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.'))) AS Tot_Dif_Min  ")
            StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo IN ('ofruttiimmaturi','ofruttilesionati','ofruttischiacciati','ofruttiscottati')  ")
            StbSQL.AppendLine("             GROUP BY progressivo) AS MPC_2 on MPC_2.progressivo = Mov_Dett_Raccolta.cal_cod  ")

            'StbSQL.AppendLine(" INNER JOIN Centri_Aziendali_Codici CA_Codici ON CA_Codici.Piva = Mov_Dett_Raccolta.PIVA AND CA_Codici.Sa_Cod = Mov_Dett_Raccolta.SA_COD  " )

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione = -1 ")
            ''                                                           0 = NO SURGELATO
            'StbSQL.AppendLine(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  " )

            'modifica rispetto a vecchio conf
            ''StbSQL.AppendLine(" AND Fabbr_Raccolta_Codici.Id_Cod = " & Agro_SQL_SaveNum(CA_COD_STABILIMENTO) & " " )
            'StbSQL.AppendLine(" AND CA_Codici.Id_Cod = " & Agro_SQL_SaveNum(????) & " " )

            'novità new conf
            StbSQL.AppendLine(" AND Mov_Dett_Raccolta.elem_cod <> " & BENI_CONFEZ_VEGETALE.ToString & " ")

            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI) & "'")
            'StbSQL.AppendLine(" AND Mov_Certificato.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) & "'")
            StbSQL.AppendLine(" AND Mov_Conf.Cau_Mov = '" & Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) & "'")

            'modifica rispetto a vecchio conf
            'StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" & Agro_SQL_SaveText(CAU_ACCETTAZIONE_BENI_DA_DIVERSI) & "'" )
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" & Agro_SQL_SaveText(CAU_CARICO) & "'")

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine_Accett) & " ")
            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio_Accett) & " ")

            If Sa_Cod <> 0 Then
                StbSQL.AppendLine(" AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StbSQL.AppendLine(" ) ")
            '/*     FINE POMODORO   */

            'prima versione della query
            ''StbSQL.AppendLine(" WHERE 1 = 1 ")
            ''If prezzo_valorizzato <> -1 Then
            ''StbSQL.AppendLine(" AND prezzo_valorizzato = " + Agro_SQL_SaveNum(CStr(prezzo_valorizzato)) + "" )
            ''End If
            ''Select Case degrado_valorizzato
            ''    Case 1
            ''        StbSQL.AppendLine(" AND degrado_perc > 0 ")
            ''    Case 0
            ''        StbSQL.AppendLine(" AND degrado_perc = 0 ")
            ''End Select

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                StbSQL.AppendLine(" ORDER BY Data_Bolla, Doc_Numero_Visualizzato, Mov_Dett_Raccolta.ordine_det, id_mov_det ASC ")
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

    '#####################################################################
    Public Function ExportJDEdwards_Verifica_UdmCod_Errata(ByVal Piva As String,
                                                        ByVal Data_Inizio_Accett As Date,
                                                        ByVal Data_Fine_Accett As Date,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable


        'LEGGIMI:
        'non appplico tutti i filtri 
        'ByVal Sa_Cod As Integer,
        'ByVal ElencoStatiEsportazione As String,

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.ExportJDEdwards_Verifica_UdmCod_Errata"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.Length = 0

            '/*     INIZIO SPECIE NO POMODORO   */
            StbSQL.AppendLine(" ( ")
            StbSQL.AppendLine(" -- BOLLE CONFERIMENTO NO POMODORO ")
            StbSQL.AppendLine(" SELECT ")
            StbSQL.AppendLine(" Agenda.Piva, Agenda.Des_lib, Agenda.ID_Agenda, ")
            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Visualizzato AS Numero_Bolla, Mov_Accett.Data_Movimento AS Data_Bolla, ")
            StbSQL.AppendLine(" codice_esterno + ' ' + mat_des AS articolo ")

            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA  ")
            StbSQL.AppendLine(" AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime ON Materie_Prime.mat_cod = Mov_Dett_Raccolta.mat_cod AND Materie_Prime.elem_cod = Mov_Dett_Raccolta.elem_cod             ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione <> -1 ")
            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")

            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine_Accett) & " ")
            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio_Accett) & " ")

            'If Not String.IsNullOrEmpty(ElencoStatiEsportazione) Then
            '    StbSQL.AppendLine(" AND Agenda.Stato_Export IN (" & Agro_SQL_SaveText(ElencoStatiEsportazione) & ") " )
            'End If
            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine("  AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")

            StbSQL.AppendLine(" AND Mov_Dett_Raccolta.udm_cod <> " & CStr(enum_UnitaMisura.KG) & " ")

            'If Sa_Cod <> 0 Then
            '    StbSQL.AppendLine(" AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  " )
            'End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            StbSQL.AppendLine(" )  ")
            StbSQL.AppendLine(" -- ** FINE BOLLE CONFERIMENTO NO POMODORO ** ")
            '/*     FINE SPECIE NO POMODORO   */

            'StbSQL.AppendLine(" UNION ALL " )

            ''/*     INIZIO POMODORO  (sarà da modificare) */
            'StbSQL.AppendLine(" ( " )
            '....
            'StbSQL.AppendLine(" ) " )
            ''/*     FINE POMODORO   */


            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                'x visualizzare le bolle ordinate x stabilimento metto davanti il numero bolla
                StbSQL.AppendLine(" ORDER BY Numero_Bolla, Data_Bolla, ordine_det, id_mov_det ASC ")
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


    '###############################################################################
    Public Function RecuperaDatiBollaxMonitoraggioCE_NEW(ByVal Piva As String,
                                                        ByVal Doc_Numero_Sin_Bolla As String,
                                                        ByVal Doc_Numero_Bolla As Decimal,
                                                        ByVal Doc_Numero_Des_Bolla As String,
                                                        ByVal Doc_Numero_Sin_DDT As String,
                                                        ByVal Doc_Numero_DDT As Decimal,
                                                        ByVal Doc_Numero_Des_DDT As String,
                                                        ByVal Anno_Bolla As Integer,
                                                        ByVal Data_DDT As String,
                                                        ByVal Cod_Contatto_Produttore As String,
                                                        ByVal Sa_Cod As Integer,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByVal xOrderBy As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.RecuperaDatiBollaxMonitoraggioCE_NEW()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------


        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT   -- Mov_Accett.Tara_Veicolo, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso,  ")
            StrSQL.AppendLine("         Mov_Accett.Data_Movimento AS Data_Accett_Bolla, Mov_Accett.Ora AS Ora_Accett, ")
            StrSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin as Doc_Numero_Sin_Bolla, Mov_Accett.Doc_Numero as Doc_Numero_Bolla,Mov_Accett.Doc_Numero_Des as Doc_Numero_Des_Bolla, Mov_Accett.Mov_Desc AS Note, ")
            StrSQL.AppendLine("         ISNULL(Contatti_Conferenti.Rag_Soc, '') AS Rag_Soc_Conferente, ")
            StrSQL.AppendLine("         ISNULL(Contatti_Produttori.Cod_Contatto,'') AS Cod_Contatto_Produttore,  ")
            StrSQL.AppendLine("         ISNULL(Contatti_Produttori.Rag_Soc, '') AS Rag_Soc_Produttore, ISNULL(Contatti_Produttori.Codice_Fiscale,'') AS Codice_Fiscale_Produttore,  ")
            StrSQL.AppendLine("         ISNULL(Contatti_Produttori.Nome,'') AS Nome_Produttore, ISNULL(Contatti_Produttori.Cognome,'') AS Cognome_Produttore,  ")
            StrSQL.AppendLine("         -- Mov_Accett.Cod_IndirizzoDestinazione, Indirizzi_Produttori.ind_des AS ind_des_Produttore, Indirizzi_Produttori.frz_des AS frz_des_Produttore, Indirizzi_Produttori.CAP AS cap_Produttore,  ")
            StrSQL.AppendLine("         -- Indirizzi_Produttori.stato AS stato_Produttore, Indirizzi_Produttori.pro_cod_istat AS pro_cod_istat_Produttore,  ")
            StrSQL.AppendLine("         -- Indirizzi_Produttori.com_cod_istat AS com_cod_istat_Produttore, Istat_Produttori.LOCALITA AS localita_Produttore, Istat_Produttori.COMUNI_PROV AS comuni_prov_Produttore, ")
            StrSQL.AppendLine("         Mov_Conf.Data_Movimento AS Data_Conf, Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf,  ")
            StrSQL.AppendLine("         Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf,   ")
            StrSQL.AppendLine("         Mov_Raccolta.Data_Movimento AS Data_Raccolta,  ")
            StrSQL.AppendLine("         Mov_Raccolta.Ora AS Ora_Raccolta,")
            StrSQL.AppendLine("         Mov_Dett_Raccolta.Elem_Cod AS Elem_Cod_Raccolta,  ")
            StrSQL.AppendLine("         Mov_Dett_Raccolta.Pro_Cod AS Pro_Cod_Raccolta, Mov_Dett_Raccolta.Mat_Cod AS Mat_Cod_Raccolta,  ")
            StrSQL.AppendLine("         Mov_Dett_Raccolta.Cod_Progetto AS Cod_Progetto_Raccolta, Mov_Dett_Raccolta.Fase_Cod AS Fase_Cod_Raccolta,  ")
            StrSQL.AppendLine("         Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta,  ")
            StrSQL.AppendLine("         Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta,  ")
            StrSQL.AppendLine("         MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta,  ")
            StrSQL.AppendLine("         MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, MP_Raccolta.Cul_Cod, MP_Raccolta.Veg_Cod, MP_Raccolta.GRVA_COD_VEG,  ")
            StrSQL.AppendLine("         MP_Raccolta.Regolamento ")
            StrSQL.AppendLine("         , ISNULL(   ")
            StrSQL.AppendLine("                 (SELECT TOP 1 OTabelle_Parametri.descrizione ")
            StrSQL.AppendLine("                 FROM Materie_Prime_Campionature ")
            StrSQL.AppendLine("                 INNER JOIN OTabelle_Parametri ON Materie_Prime_Campionature.tipo_cod =  OTabelle_Parametri.tabella_par_cod ")
            StrSQL.AppendLine("                 WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.cal_cod ")
            StrSQL.AppendLine("                 AND tipo = 'ocalibro' ")
            StrSQL.AppendLine("                 AND tabella_cod = " & CStr(enum_OTabelle.Calibro) & " ")
            StrSQL.AppendLine("                 ) ,   '' ) AS Calibro  ")
            StrSQL.AppendLine("         , ISNULL(   ")
            StrSQL.AppendLine("                 (SELECT TOP 1 val_cod ")
            StrSQL.AppendLine("                 FROM Materie_Prime_Campionature ")
            StrSQL.AppendLine("                 WHERE Materie_Prime_Campionature.progressivo=Mov_Dett_Raccolta.cal_cod ")
            StrSQL.AppendLine("                 AND tipo = 'opunteggio' ")
            StrSQL.AppendLine("                 ) ,   '' ) AS Punteggio  ")
            StrSQL.AppendLine(" , CA_Raccolta.Sa_Nome AS Stabilimento ")

            StrSQL.AppendLine(" FROM Agenda ")

            StrSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")

            StrSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StrSQL.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")

            StrSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  ")
            'StrSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Produttori ON  Mov_Accett.Cod_IndirizzoDestinazione = Indirizzi_Produttori.cod_indirizzo " )
            'StrSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Produttori ON Indirizzi_Produttori.pro_cod_istat = Istat_Produttori.PROV AND Indirizzi_Produttori.com_cod_istat = Istat_Produttori.COM " )

            StrSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda ")

            StrSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda ")
            StrSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StrSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")

            StrSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det ")
            StrSQL.AppendLine(" INNER JOIN Centri_Aziendali CA_Raccolta ON Mov_Dest_Raccolta.Piva = CA_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = CA_Raccolta.SA_COD ")

            StrSQL.AppendLine(" WHERE Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")

            StrSQL.AppendLine(" AND Mov_Accett.Cau_Mov      = '" & CAU_REGISTRAZIONI & "'")
            StrSQL.AppendLine(" AND Mov_Conf.Cau_Mov        = '" & CAU_REGISTRAZIONI_ALLEGATE & "'")
            StrSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov    = '" & CAU_CARICO & "'")

            StrSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            'per scartare beni confez e riga desc libera
            StrSQL.AppendLine(" AND Mov_Dett_Raccolta.elem_cod = " & CStr(TRASFORMATI_VEGETALI) & " ")

            If Doc_Numero_Sin_Bolla <> "" Then
                StrSQL.AppendLine(" AND Mov_Accett.Doc_Numero_Sin = '" & Agro_SQL_SaveText(Doc_Numero_Sin_Bolla) & "'  ")
            End If

            If Doc_Numero_Bolla <> 0 Then
                StrSQL.AppendLine(" AND Mov_Accett.Doc_Numero = " & Agro_SQL_SaveNum(Doc_Numero_Bolla) & "  ")
            End If

            If Doc_Numero_Des_Bolla <> "" Then
                StrSQL.AppendLine(" AND Mov_Accett.Doc_Numero_Des = '" & Agro_SQL_SaveText(Doc_Numero_Des_Bolla) & "'  ")
            End If

            If Anno_Bolla <> 0 Then
                StrSQL.AppendLine(" AND Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(CStr("01/01/" & Anno_Bolla)) & "  ")
                StrSQL.AppendLine(" AND Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(CStr("31/12/" & Anno_Bolla)) & "  ")
            End If

            If Doc_Numero_Sin_DDT <> "" Then
                StrSQL.AppendLine(" AND Mov_Conf.Doc_Numero_Sin = '" & Agro_SQL_SaveText(Doc_Numero_Sin_DDT) & "'  ")
            End If

            If Doc_Numero_DDT <> 0 Then
                StrSQL.AppendLine(" AND Mov_Conf.Doc_Numero = " & Agro_SQL_SaveNum(Doc_Numero_DDT) & "  ")
            End If

            If Doc_Numero_Des_DDT <> "" Then
                StrSQL.AppendLine(" AND Mov_Conf.Doc_Numero_Des = '" & Agro_SQL_SaveText(Doc_Numero_Des_DDT) & "'  ")
            End If

            'If Anno_DDT <> 0 Then
            '    StrSQL.AppendLine(" AND Mov_Conf.Data_Movimento >= " & Agro_SQL_SaveDate(CStr("01/01/" & Anno_Bolla)) & "  ")
            '    StrSQL.AppendLine(" AND Mov_Conf.Data_Movimento <= " & Agro_SQL_SaveDate(CStr("31/12/" & Anno_Bolla)) & "  ")
            'End If

            If Data_DDT <> "" Then
                StrSQL.AppendLine(" AND Mov_Conf.Data_Movimento = " & Agro_SQL_SaveDate(Data_DDT) & " ")
            End If

            If Cod_Contatto_Produttore <> "" Then
                StrSQL.AppendLine(" AND Contatti_Produttori.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto_Produttore) & "'  ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND     Mov_Dest_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Mov_Accett.Inviato >=0 ")
                Case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Mov_Accett.Inviato =-1 ")
                Case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
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



    '#####################
    'Tipo_Select = 0 -> select normale
    'Tipo_Select = 1 -> ISNULL(MAX(Mov_Accett.Doc_Numero), 0) AS Doc_Numero_Max, ISNULL(MIN(Mov_Accett.Doc_Numero), 0) AS Doc_Numero_Min 
    'query utilizzata per ricavare gli id_agenda delle bolle richiedendo la stampa massiva da numero bolla a numero bolla
    Public Function Bolle_IdAgenda_DocNumero_Leggi(ByVal Piva As String,
                                                   ByVal Sa_Cod As Integer,
                                                   ByVal Id_Agenda As Integer,
                                                   ByVal Tipo_Select As Integer,
                                                   ByVal Flag_EscludiPomodoro As Boolean,
                                                   ByVal Doc_Numero_Sin As String,
                                                   ByVal Doc_Numero_Des As String,
                                                   ByVal ElencoNumeriBolla As String,
                                                   ByVal Data_Inizio As Date,
                                                   ByVal Data_Fine As Date,
                                                  ByVal Cod_RisUm As Integer,
                                                    ByVal Mat_Cod As Integer,
                                                    ByVal Veg_Cod As Integer,
                                                    ByVal Cul_Cod As Integer,
                                                    ByVal FiltroAggArticoli As String,
                                                    ByVal FiltroAggConferenti As String,
                                                   ByVal Piva_Produttore As String,
                                                   ByVal Piva_Coop1 As String,
                                                   ByVal Piva_Coop2 As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                       ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.Bolle_IdAgenda_DocNumero_Leggi"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StbSQL.Length = 0

            Select Case Tipo_Select
                Case 0
                    StbSQL.AppendLine("SELECT   DISTINCT Agenda.Id_Agenda, Agenda.des_lib, ")
                    StbSQL.AppendLine("         Mov_Accett.Data_Movimento, ")
                    StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des  ")
                Case 1
                    StbSQL.AppendLine("SELECT   ISNULL(MAX(Mov_Accett.Doc_Numero), 0) AS Doc_Numero_Max, ISNULL(MIN(Mov_Accett.Doc_Numero), 0) AS Doc_Numero_Min ")
            End Select

            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")

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

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")

            If Flag_EscludiPomodoro Then
                StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione  <> -1 ")
            End If
            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "' ")

            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine(" AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " ")

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            If Sa_Cod <> 0 Then
                StbSQL.AppendLine(" AND Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Id_Agenda <> 0 Then
                StbSQL.AppendLine(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            End If
            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento <=" & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento >=" & Agro_SQL_SaveDate(Data_Inizio) & " ")

            If ElencoNumeriBolla <> "" Then
                StbSQL.AppendLine(" AND Mov_Accett.Doc_Numero IN ( " & Agro_SQL_Save_Clausola_IN(ElencoNumeriBolla) & ") ")
            End If

            If Doc_Numero_Sin <> "-999" Then
                StbSQL.AppendLine(" AND Mov_Accett.Doc_Numero_Sin = '" & Agro_SQL_SaveText(Doc_Numero_Sin) & "' ")
            End If
            If Doc_Numero_Des <> "-999" Then
                StbSQL.AppendLine(" AND Mov_Accett.Doc_Numero_Des = '" & Agro_SQL_SaveText(Doc_Numero_Des) & "' ")
            End If

            If Cod_RisUm <> 0 Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro FiltroAggConferenti)
                StbSQL.AppendLine(" AND Mov_Accett.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            End If

            If FiltroAggConferenti <> "" Then
                StbSQL.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggConferenti, , objParametri))
            End If

            If FiltroAggArticoli <> "" Then
                StbSQL.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggArticoli, , objParametri))
            End If

            If Mat_Cod <> 0 Then
                StbSQL.AppendLine(" AND MP_Raccolta.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            If Cul_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
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

            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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
    ''' utilizzata dal report estratto conto bolle
    ''' Default:
    '''ByVal Mat_Cod As Integer = 0, _
    '''ByVal FiltroSpecie As String = "", _
    '''ByVal FiltroConferenti As String = "", _
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function EstrattoConto_BolleConferimento(ByVal Flag01_GestioneCodEsterno As Integer,
                                                    ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Data_Inizio As Date,
                                                    ByVal Data_Fine As Date,
                                                    ByVal Cod_RisUm As Integer,
                                                    ByVal Veg_Cod As Integer,
                                                    ByVal Cul_Cod As Integer,
                                                    ByVal Mat_Cod As Integer,
                                                    ByVal FiltroAggArticoli As String,
                                                    ByVal FiltroAggConferenti As String,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.FreshAndFood.EstrattoConto_BolleConferimento"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.AppendLine("SELECT   ")
            StbSQL.AppendLine(" Risorse_Umane_Conferenti.Cod_RisUm AS Cod_RisUm_Conferente, ")
            StbSQL.AppendLine(" Risorse_Umane_Conferenti.Settore_Des AS Codice_Conferente, ")
            StbSQL.AppendLine(" ISNULL(Contatti_Conferenti.Rag_Soc, ' ') AS RagSoc_Conferente, ")

            StbSQL.AppendLine(" MP_Raccolta.Cod_Articolo, MP_Raccolta.Codice_Esterno,   ")
            StbSQL.AppendLine(" MP_Raccolta.Mat_Cod, MP_Raccolta.Mat_Des, MP_Raccolta.Veg_Cod, ")
            StbSQL.AppendLine(" MP_Raccolta.Cul_Cod, SpecieVegetali.Veg_Des, Cultivar.Cul_Des, ")

            StbSQL.AppendLine(" Mov_Accett.Data_Movimento AS Data_Bolla, ")
            StbSQL.AppendLine(" Mov_Accett.Doc_Numero_Visualizzato AS Numero_Bolla, ")

            StbSQL.AppendLine(" Mov_Conf.Mov_Desc AS Mov_Desc_Conf, Mov_Conf.Data_Movimento AS Data_Conf, ")
            StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf, ")
            StbSQL.AppendLine(" Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf,  Mov_Conf.Colli, ")

            StbSQL.AppendLine(" Mov_Accett.Tipo_Peso, Mov_Accett.Peso AS Peso_Totale, Mov_Accett.Tara_Veicolo, 0.0 AS Peso_Lordo,  ")
            StbSQL.AppendLine(" -- Mov_Accett.Tara_Imballi AS Tara_Imballi_Testata  , ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.Tara AS Tara_Dettaglio, 0.0 AS Tara_Imballi, ")
            StbSQL.AppendLine(" ISNULL(  ")
            StbSQL.AppendLine("         (SELECT count(*) AS num_righe ")
            StbSQL.AppendLine("         FROM Movimenti_dettagli MD_righe  ")
            StbSQL.AppendLine("         WHERE MD_righe.piva = agenda.piva  ")
            StbSQL.AppendLine("         AND MD_righe.id_agenda = agenda.id_agenda  ")
            StbSQL.AppendLine("         AND MD_righe.elem_cod = " & TRASFORMATI_VEGETALI & " ")
            StbSQL.AppendLine("         ), '0' ) AS num_righe, ")
            StbSQL.AppendLine(" ISNULL(  ")
            StbSQL.AppendLine("         (SELECT sum(Qta_Extra_Totale) as tara_totale_imb_entrata  ")
            StbSQL.AppendLine("            FROM Movimenti_dettagli MD_righe ")
            StbSQL.AppendLine("             WHERE MD_righe.piva = agenda.piva  ")
            StbSQL.AppendLine("             AND MD_righe.id_agenda = agenda.id_agenda    ")
            StbSQL.AppendLine("             AND MD_righe.elem_cod = " & BENI_CONFEZ_VEGETALE & " ")
            StbSQL.AppendLine("             AND MD_righe.ordine_det = 30000 ")
            StbSQL.AppendLine("             ), '0' ) as tara_totale_imb_entrata, ")

            StbSQL.AppendLine(" Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta, ")
            StbSQL.AppendLine(" Mov_Dett_Raccolta.qta_extra_totale AS qta_extra_totale_Raccolta, ")
            StbSQL.AppendLine(" 0 AS Degrado, 0 AS Netto_Pagamento, ")

            StbSQL.AppendLine(" Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta ")

            StbSQL.AppendLine(" , ISNULL(   ")
            StbSQL.AppendLine("                 (SELECT TOP 1 OTabelle_Parametri.SIGLA ")
            StbSQL.AppendLine("                 FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("                 INNER JOIN OTabelle_Parametri ON Materie_Prime_Campionature.tipo_cod =  OTabelle_Parametri.tabella_par_cod AND tabella_cod = " & CStr(enum_OTabelle.Calibro) & " ")
            StbSQL.AppendLine("                 WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.cal_cod ")
            StbSQL.AppendLine("                 AND tipo = 'ocalibro' ")
            StbSQL.AppendLine("                 ) ,   '' ) AS calibro_des  ")

            StbSQL.AppendLine(" , ISNULL(CONVERT(float, (SELECT TOP 1 VAL_COD ")
            StbSQL.AppendLine("         FROM Materie_Prime_Campionature ")
            StbSQL.AppendLine("         WHERE Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.cal_cod ")
            StbSQL.AppendLine("         AND materie_Prime_Campionature.tipo IN ('opunteggio','ogradotenderom','oresiduosecco','ogradobrix') ")
            StbSQL.AppendLine("         AND materie_Prime_Campionature.val_cod <> '' AND materie_Prime_Campionature.val_cod <> '0' ")
            StbSQL.AppendLine("         )), 0 ) AS Punteggio ")

            StbSQL.AppendLine(" FROM Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda  ")

            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda  ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")
            StbSQL.AppendLine(" LEFT JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = MP_Raccolta.Veg_Cod ")
            StbSQL.AppendLine(" LEFT JOIN Cultivar ON Cultivar.Cul_Cod = MP_Raccolta.Cul_Cod ")

            StbSQL.AppendLine(" WHERE   Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodAccettazioniConf)) & ")")
            StbSQL.AppendLine(" AND     Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            StbSQL.AppendLine(" AND     Mov_Accett.Cau_Mov = '" & CAU_REGISTRAZIONI & "'")
            StbSQL.AppendLine(" AND     Mov_Conf.Cau_Mov = '" & CAU_REGISTRAZIONI_ALLEGATE & "'")
            StbSQL.AppendLine(" AND     Mov_Raccolta.Cau_Mov = '" & CAU_CARICO & "'")

            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine("     AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI & " ")

            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            StbSQL.AppendLine(" AND     Mov_Accett.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            StbSQL.AppendLine(" AND     Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")

            If Mat_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            End If

            If Veg_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            End If

            If Cul_Cod <> 0 Then
                StbSQL.AppendLine(" AND     MP_Raccolta.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
            End If

            If Cod_RisUm <> 0 Then
                'se è stato selezionato un solo conferente
                '(altrimenti c'è il filtro FiltroAggConferenti)
                StbSQL.AppendLine(" AND Mov_Accett.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            End If

            If FiltroAggArticoli <> "" Then
                StbSQL.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggArticoli, , objParametri))
            End If

            If FiltroAggConferenti <> "" Then
                StbSQL.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggConferenti, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If Flag01_GestioneCodEsterno = 1 Then
                StbSQL.AppendLine(" ORDER BY Risorse_Umane_Conferenti.Cod_RisUm, MP_Raccolta.Codice_Esterno, MP_Raccolta.Cod_Articolo, Mov_Accett.Data_Movimento ")
            Else
                StbSQL.AppendLine(" ORDER BY Risorse_Umane_Conferenti.Cod_RisUm, MP_Raccolta.Veg_Cod, MP_Raccolta.Cod_Articolo, Mov_Accett.Data_Movimento ")
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




