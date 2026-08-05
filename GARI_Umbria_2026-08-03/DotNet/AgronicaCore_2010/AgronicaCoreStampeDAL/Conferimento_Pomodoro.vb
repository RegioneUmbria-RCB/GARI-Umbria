Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports Newtonsoft.Json

Public Class Conferimento_Pomodoro
    Inherits AgronicaCoreDataProvider.DataProvider

    Private arrLavCodAccettazioniConf As Integer() = New Integer() {LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE} 'LAVCOD_DISTINTA_CARICO, LAVCOD_AUTO_DDT_EMESSO Non usate per i conferimenti


    '#####################################
    'richiamata dalla stampa pdf e dalla stampa excel di riepilogo
    Public Function CertificatoPomodoro_Stampa_PDF_XLS(ByVal Piva As String,
                                                        ByVal Id_Agenda As Integer,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal Cod_Articolo As String,
                                                        ByVal Codice_Esterno As String,
                                                        ByVal Codice_Conferente As String,
                                                        ByVal Cod_RisUm As Integer,
                                                        ByVal Piva_Produttore As String,
                                                        ByVal Piva_Coop1 As String,
                                                        ByVal Data_Inizio As Date,
                                                        ByVal Data_Fine As Date,
                                                        ByVal Mat_Cod As Integer,
                                                        ByVal Veg_Cod As Integer,
                                                        ByVal Cul_Cod As Integer,
                                                        ByVal FiltroAggArticoli As String,
                                                        ByVal FiltroAggConferenti As String,
                                                        ByVal Flag_IndirizzoCoop As Boolean,
                                                        ByVal Flag_IndirizzoProd As Boolean,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                        ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Conferimento_Pomodoro.CertificatoPomodoro_Stampa_PDF_XLS"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.Length = 0

            StbSQL.AppendLine(" SELECT  Agenda.Id_Agenda, Agenda.des_lib,  ")

            'VECCHIO CONF
            'StbSQL.AppendLine("         Mov_Certificato.Doc_Numero_Sin AS Doc_Numero_Sin_Cert, Mov_Certificato.Doc_Numero AS Doc_Numero_Cert, Mov_Certificato.Doc_Numero_Des AS Doc_Numero_Des_Cert,  ")
            'StbSQL.AppendLine("         Mov_Certificato.Data_Movimento AS Data_Certificato, ")
            'StbSQL.AppendLine("         Mov_Certificato.Username_Note AS Descrizione_Appezzamento, Mov_Certificato.Sezionale_Cod AS CodiceVarieta_Distretto, ")
            'StbSQL.AppendLine("         ISNULL(  (SELECT TOP 1 Descrizione ")
            'StbSQL.AppendLine("                     FROM CAC_Codifica_Cultivar ")
            'StbSQL.AppendLine("                     WHERE CONVERT(int,CAC_Codifica_Cultivar.Cultivar_Coltiva) = Mov_Certificato.Sezionale_Cod ")
            'StbSQL.AppendLine("                     ), '' ) AS Varieta_Distretto, ")
            'StbSQL.AppendLine("         Mov_Certificato.extra_int AS Tagliando_Pesa,  Mov_Certificato.extra_Str AS Premio_PomoTardivo, ")
            'StbSQL.AppendLine("         Mov_Accett.progr_registrazione AS Numero_Certificato_OLD, Mov_Accett.data_registrazione AS Data_Certificato_OLD, ")

            StbSQL.AppendLine("         Mov_Dett_Conf.Tagliando_Pesa, Mov_Dett_Conf.Premio_Complessivo AS Premio_PomoTardivo, ")
            StbSQL.AppendLine("         Mov_Dett_Conf.Desc_Appezzamenti AS Descrizione_Appezzamento, Mov_Dett_Conf.Cod_Varieta AS CodiceVarieta_OI, Varieta_OI.Desc_Varieta AS DescVarieta_OI, ")

            StbSQL.AppendLine("         ISNULL(IC.Contratto_Numero, '') AS Contratto_Numero, ISNULL(IC.Contratto_Nome, '') AS Cod_Agrea_Contratto, ISNULL(IC.Data_Stipulazione, '01/01/1900') AS Data_Stipulazione,  ")

            ''modifica del 20/09/2010: la clausola viene salvata nel campo num_protocollo
            StbSQL.AppendLine("         ISNULL(  (SELECT TOP 1 Imprese_Contratti_Clausole.Clausola_Cod_Alternativo + '|' + convert(varchar(250), Imprese_Contratti_Clausole.Validita_Inizio, 103) ")
            StbSQL.AppendLine("                     FROM Imprese_Contratti_Clausole ")
            StbSQL.AppendLine("                     WHERE Mov_Accett.Num_Protocollo = Imprese_Contratti_Clausole.Clausola_Cod ")
            StbSQL.AppendLine("                     ), '|' ) AS Clausola, ")
            StbSQL.AppendLine("  ")

            StbSQL.AppendLine("         Mov_Accett.Data_Movimento AS Data_Accett, Mov_Accett.Ora AS Ora_Accett, YEAR(Mov_Accett.Data_Movimento) AS Anno_raccolto,  ")
            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Visualizzato AS Numero_Bolla, " + vbCrLf)
            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero,Mov_Accett.Doc_Numero_Des,  Mov_Accett.Mov_Desc AS Note, ")

            'nuovo conf: pesi
            ' StbSQL.Appendline(" Mov_Accett.Peso, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso,  " + vbCrLf)
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

            'novità new conf
            StbSQL.AppendLine(" Mov_Dett_Raccolta.ordine_det, Mov_Dett_Raccolta.qta_extra_totale AS peso_netto, " + vbCrLf)

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
            If Flag_IndirizzoCoop = True Then
                StbSQL.AppendLine("         ImpreseXIndirizzi_Coop.Cod_Indirizzo AS Cod_Indirizzo_Coop, Indirizzi_Coop.ind_des AS ind_des_Coop, Indirizzi_Coop.frz_des AS frz_des_Coop, Indirizzi_Coop.CAP AS cap_Coop, ")
                StbSQL.AppendLine("         Indirizzi_Coop.stato AS stato_Coop, Indirizzi_Coop.pro_cod_istat AS pro_cod_istat_Coop,  ")
                StbSQL.AppendLine("         Indirizzi_Coop.com_cod_istat AS com_cod_istat_Coop, Istat_Coop.LOCALITA AS localita_Coop, Istat_Coop.COMUNI_PROV AS comuni_prov_Coop, ")
            End If
            StbSQL.AppendLine("         Mov_Accett.Cod_Destinazione, ")
            StbSQL.AppendLine("         Risorse_Umane_Produttori.Cod_Rapporto AS Cod_Rapporto_Produttore, ISNULL(Contatti_Produttori.Cod_Contatto, '') AS Cod_Contatto_Produttore,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Produttori.Rag_Soc, '') AS Rag_Soc_Produttore, ISNULL(Contatti_Produttori.Codice_Fiscale, '') AS Codice_Fiscale_Produttore,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Produttori.Nome, '') AS Nome_Produttore, ISNULL(Contatti_Produttori.Cognome, '') AS Cognome_Produttore,  ")
            If Flag_IndirizzoProd = True Then
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
            'StbSQL.AppendLine("                     AND Fabbricati_Codici.fabbricato_cod = Fabbr_Raccolta.Vas_Cod  ")
            StbSQL.AppendLine("                     AND Fabbricati_Codici.ID_COD = 1129 ")
            StbSQL.AppendLine("                     ), '0' ) AS Codice_Stab_Reg, ")
            'StbSQL.appendline("         /* " )
            'StbSQL.appendline("         Indirizzi_Fabbricato.ind_des AS ind_des_fabbricato, Indirizzi_Fabbricato.frz_des AS frz_des_Fabbricato, Indirizzi_Fabbricato.CAP AS cap_Fabbricato,   " )
            'StbSQL.appendline("         Indirizzi_Fabbricato.stato AS stato_Fabbricato, Indirizzi_Fabbricato.pro_cod_istat AS pro_cod_istat_fabbricato,   " )
            'StbSQL.appendline("         Indirizzi_Fabbricato.com_cod_istat AS com_cod_istat_fabbricato, Istat_fabbricato.LOCALITA AS localita_fabbricato, Istat_fabbricato.COMUNI_PROV AS comuni_prov_fabbricato,  " )
            'StbSQL.appendline("         */ " )
            StbSQL.AppendLine("         Mov_Accett.Mezzo, Mov_Accett.Cod_Vettore, ")
            StbSQL.AppendLine("         Risorse_Umane_Vettore.Cod_Rapporto AS Cod_Rapporto_Vettore, Contatti_Vettore.Cod_Contatto AS Cod_Contatto_Vettore,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Vettore.Rag_Soc, '') + ISNULL(Contatti_Vettore.Cognome, '') + ' ' + ISNULL(Contatti_Vettore.Nome, '') AS Rag_Soc_Vettore,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Vettore.Codice_Fiscale, '') AS Codice_Fiscale_Vettore,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Vettore.Nome, '') AS Nome_Vettore, ISNULL(Contatti_Vettore.Cognome, '') AS Cognome_Vettore,  ")
            'StbSQL.appendline("         /* " )
            'StbSQL.appendline("         Mov_Accett.Cod_IndirizzoVettore, Indirizzi_Vettore.ind_des AS ind_des_Vettore, Indirizzi_Vettore.frz_des AS frz_des_Vettore, Indirizzi_Vettore.CAP AS cap_Vettore,  " )
            'StbSQL.appendline("         Indirizzi_Vettore.stato AS stato_Vettore, Indirizzi_Vettore.pro_cod_istat AS pro_cod_istat_Vettore,  " )
            'StbSQL.appendline("         Indirizzi_Vettore.com_cod_istat AS com_cod_istat_Vettore, Istat_Vettore.LOCALITA AS localita_Vettore, Istat_Vettore.COMUNI_PROV AS comuni_prov_Vettore, " )
            'StbSQL.appendline("         */ " )

            StbSQL.AppendLine("         Mdt_Extra.Targa AS Targa_Automezzo, Mdt_Extra.N_Immatricolazione_Rimorchio AS Targa_Rimorchio, Mdt_Extra.Provvigione AS Premio_Pomo_Bio, ")

            StbSQL.AppendLine("         Mov_Conf.Id_Mov AS Id_Mov_Conf, Mov_Conf.Cau_Mov AS Cau_Mov_Conf, Mov_Conf.Mov_Desc AS Mov_Desc_Conf,  ")
            StbSQL.AppendLine("         Mov_Conf.Data_Movimento AS Data_Conf, Mov_Conf.Doc_Numero_Sin AS Doc_Numero_Sin_Conf, Mov_Conf.Doc_Numero AS Doc_Numero_Conf,  ")
            StbSQL.AppendLine("         Mov_Conf.Doc_Numero_Des AS Doc_Numero_Des_Conf,  ")

            StbSQL.AppendLine("         Mov_Raccolta.Id_Mov AS Id_Mov_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Raccolta.Cau_Mov AS Cau_Mov_Raccolta, Mov_Raccolta.Mov_Desc AS Mov_Desc_Raccolta, Mov_Raccolta.Data_Movimento AS Data_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Raccolta.Ora AS Ora_Raccolta, C_Az_Raccolta.sa_cod AS Sa_Cod_Raccolta,  ")
            StbSQL.AppendLine("         C_Az_Raccolta.sa_nome AS Sa_Nome_Raccolta,  ")

            StbSQL.AppendLine("         Mov_Dett_Raccolta.Id_Mov_Det AS Id_Mov_Det_Raccolta, Mov_Dest_Raccolta.Id_Destinazione AS Id_Destinazione_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Elem_Cod AS Elem_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Pro_Cod AS Pro_Cod_Raccolta, Mov_Dett_Raccolta.Mat_Cod AS Mat_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Cod_Progetto AS Cod_Progetto_Raccolta, Mov_Dett_Raccolta.Fase_Cod AS Fase_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Lotto AS Lotto_Raccolta, Mov_Dett_Raccolta.Cal_Cod AS Cal_Cod_Raccolta, Mov_Dett_Raccolta.Udm_Cod AS Udm_Cod_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Qta AS Qta_Raccolta, Mov_Dett_Raccolta.Variazione AS Variazione_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Listino_Cod AS Listino_Cod_Raccolta, Mov_Dett_Raccolta.Tara AS Tara, Mov_Dett_Raccolta.Prezzo_Unitario, Mov_Dett_Raccolta.Prezzo_Unitario_Netto,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Jolly_Int AS Jolly_Int_Raccolta, Mov_Dett_Raccolta.Contabilizzato AS Contabilizzato_Raccolta,  ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Pendente AS Pendente_Raccolta, ")
            StbSQL.AppendLine("         MP_Raccolta.Cod_Articolo AS Cod_Articolo_Raccolta,  ")
            StbSQL.AppendLine("         MP_Raccolta.Mat_Des AS Mat_Des_Raccolta, MP_Raccolta.Cul_Cod, MP_Raccolta.Veg_Cod, SpecieVegetali.Veg_Des, ISNULL(MP_Raccolta.GRVA_COD_VEG, 0) AS GRVA_COD_VEG, ISNULL(GruppoVarietale.Grva_Des, '') AS Grva_Des,  ")
            StbSQL.AppendLine("         MP_Raccolta.Regolamento, MP_Raccolta.ChkListino,  ")
            'vecchio conferimento
            'StbSQL.AppendLine("         MP_Dettagli_Raccolta.Extra_Smallint4 AS Flag_Surgelato, ")

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

            StbSQL.AppendLine("         ISNULL(ICF.Valore5, 3) AS Franchigia_DifMaggiori, ISNULL(ICF.Valore7, 0.5) AS Coefficiente_DifMinori, ISNULL(ICF.Valore9, 0) AS Premio_EuroTon_PomoBio, ")
            'StbSQL.AppendLine("         , MPC_1a.Dif_Magg_Marcio, MPC_1b.Dif_Magg_Verde , MPC_1c.Dif_Magg_Inerti   ")
            'StbSQL.AppendLine("         , MPC_2.Tot_Dif_Min , MPC_3.Frutti_Schiacciati, MPC_4.Grado_Brix ")
            StbSQL.AppendLine("         Materie_Prime_Campionature.tipo, Materie_Prime_Campionature.tipo_cod, CONVERT(float, Materie_Prime_Campionature.val_cod) as val_cod, Materie_Prime_Campionature.descrizione ")

            StbSQL.AppendLine(" FROM Agenda ")

            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Trasf ON agenda.piva= Contatti_Trasf.cod_contatto ")
            StbSQL.AppendLine(" INNER JOIN ImpresexIndirizzi ImpresexIndirizzi_Trasf ON Contatti_Trasf.Cod_Contatto = ImpresexIndirizzi_Trasf.piva ")
            StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Trasf ON ImpresexIndirizzi_Trasf.cod_indirizzo = Indirizzi_Trasf.cod_indirizzo ")
            StbSQL.AppendLine(" INNER JOIN ISTAT Istat_trasf ON Indirizzi_Trasf.pro_cod_istat = Istat_trasf.PROV AND Indirizzi_Trasf.com_cod_istat = Istat_trasf.COM  ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")

            'Mov_Certificato non esiste nel nuovo conferimento
            'StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   ")
            'cambiato join con la Mov_Dettaglio_Tecnico_Extra
            StbSQL.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Mdt_Extra ON Mdt_Extra.PIVA = Mov_Accett.PIVA AND Mdt_Extra.Sa_Cod = Mov_Accett.Sa_Cod  ")
            StbSQL.AppendLine(" AND Mdt_Extra.Id_Agenda = Mov_Accett.Id_Agenda AND Mdt_Extra.Id_Mov = Mov_Accett.Id_Mov ")

            StbSQL.AppendLine("")
            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")
            StbSQL.AppendLine(" INNER JOIN Contatti Contatti_Conferenti ON Risorse_Umane_Conferenti.Piva = Contatti_Conferenti.Piva AND Risorse_Umane_Conferenti.Cod_Contatto = Contatti_Conferenti.Cod_Contatto  ")
            StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Conferenti ON  Mov_Accett.Cod_IndirizzoRisUm = Indirizzi_Conferenti.cod_indirizzo ")
            StbSQL.AppendLine(" INNER JOIN ISTAT Istat_Conferenti ON Indirizzi_Conferenti.pro_cod_istat = Istat_Conferenti.PROV AND Indirizzi_Conferenti.com_cod_istat = Istat_Conferenti.COM ")
            StbSQL.AppendLine("")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Coop ON Risorse_Umane_Coop.Piva = Contatti_Coop.Piva AND Risorse_Umane_Coop.Cod_Contatto = Contatti_Coop.Cod_Contatto  ")
            If Flag_IndirizzoCoop = True Then
                StbSQL.AppendLine(" LEFT OUTER JOIN ImpreseXIndirizzi ImpreseXIndirizzi_Coop ON  Contatti_Coop.Cod_Contatto = ImpreseXIndirizzi_Coop.Piva ")
                StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Coop ON  ImpreseXIndirizzi_Coop.Cod_Indirizzo = Indirizzi_Coop.cod_indirizzo ")
                StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Coop ON Indirizzi_Coop.pro_cod_istat = Istat_Coop.PROV AND Indirizzi_Coop.com_cod_istat = Istat_Coop.COM ")
            End If
            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Produttori ON Risorse_Umane_Produttori.Piva = Contatti_Produttori.Piva AND Risorse_Umane_Produttori.Cod_Contatto = Contatti_Produttori.Cod_Contatto  ")
            If Flag_IndirizzoProd = True Then
                StbSQL.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Produttori ON  Mov_Accett.Cod_IndirizzoDestinazione = Indirizzi_Produttori.cod_indirizzo ")
                StbSQL.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Produttori ON Indirizzi_Produttori.pro_cod_istat = Istat_Produttori.PROV AND Indirizzi_Produttori.com_cod_istat = Istat_Produttori.COM ")
            End If
            StbSQL.AppendLine(" ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Vettore ON Mov_Accett.Cod_Vettore = Risorse_Umane_Vettore.Cod_RisUm  ")
            StbSQL.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Vettore ON Risorse_Umane_Vettore.Piva = Contatti_Vettore.Piva AND Risorse_Umane_Vettore.Cod_Contatto = Contatti_Vettore.Cod_Contatto  ")
            'StbSQL.appendline("         /* " )
            'StbSQL.appendline(" LEFT OUTER JOIN Indirizzi Indirizzi_Vettore ON  Mov_Accett.Cod_IndirizzoVettore = Indirizzi_Vettore.cod_indirizzo " )
            'StbSQL.appendline(" LEFT OUTER JOIN ISTAT Istat_Vettore ON Indirizzi_Vettore.pro_cod_istat = Istat_Vettore.PROV AND Indirizzi_Vettore.com_cod_istat = Istat_Vettore.COM " )
            'StbSQL.appendline("         */ " )
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Conf ON Agenda.PIVA = Mov_Conf.PIVA AND Agenda.Sa_Cod = Mov_Conf.Sa_Cod AND Agenda.Id_Agenda = Mov_Conf.Id_Agenda ")

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov ")
            StbSQL.AppendLine(" INNER JOIN Mov_Dettaglio_Conferimento Mov_Dett_Conf ON Mov_Dett_Conf.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Dett_Conf.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Dett_Conf.Id_Mov = Mov_Dett_Raccolta.Id_Mov AND Mov_Dett_Conf.Id_Mov_Det = Mov_Dett_Raccolta.Id_Mov_Det ")
            StbSQL.AppendLine(" INNER JOIN Codifica_Varieta_OIPomodorodaIndustriaNordItalia Varieta_OI ON Mov_Dett_Conf.cod_varieta = Varieta_OI.cod_varieta ")

            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")
            'StbSQL.AppendLine(" LEFT OUTER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  ")
            StbSQL.AppendLine(" INNER JOIN SpecieVegetali ON SpecieVegetali.veg_cod = MP_Raccolta.Veg_Cod  ")
            'MODIFICA DEL 22/07/2010: ci sono prodotti che non hanno la tipologia varietale
            StbSQL.AppendLine(" LEFT OUTER JOIN gruppovarietale ON gruppovarietale.grva_cod = MP_Raccolta.grva_Cod_Veg ")

            StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni Mov_Dest_Raccolta ON Mov_Dett_Raccolta.PIVA = Mov_Dest_Raccolta.Piva AND Mov_Dett_Raccolta.Sa_Cod = Mov_Dest_Raccolta.Sa_Cod AND  Mov_Dett_Raccolta.Id_Agenda = Mov_Dest_Raccolta.Id_Agenda AND Mov_Dett_Raccolta.Id_Mov = Mov_Dest_Raccolta.Id_Mov AND Mov_Dett_Raccolta.Id_Mov_Det = Mov_Dest_Raccolta.Id_Mov_Det ")
            StbSQL.AppendLine(" INNER JOIN Centri_Aziendali C_Az_Raccolta ON Mov_Dett_Raccolta.PIVA = C_Az_Raccolta.PIVA AND Mov_Dett_Raccolta.Sa_Cod = C_Az_Raccolta.sa_cod ")
            StbSQL.AppendLine(" LEFT JOIN Cantina_Vasche Fabbr_Raccolta ON Mov_Dest_Raccolta.Piva = Fabbr_Raccolta.PIVA AND Mov_Dest_Raccolta.Sa_Cod = Fabbr_Raccolta.SA_COD AND Mov_Dest_Raccolta.Id_Destinazione = Fabbr_Raccolta.Vas_Cod ")

            StbSQL.AppendLine(" LEFT OUTER JOIN Imprese_Contratto_Fasi ICF ON ICF.PIVA = Mov_Dett_Raccolta.PIVA AND ICF.Elem_Cod = Mov_Dett_Raccolta.Elem_Cod  ")
            StbSQL.AppendLine(" AND ICF.pro_Cod = Mov_Dett_Raccolta.pro_Cod AND ICF.mat_Cod = Mov_Dett_Raccolta.mat_Cod AND ICF.Progetto_Cod = Mov_Dett_Raccolta.cod_progetto ")
            StbSQL.AppendLine(" AND ICF.fase_Cod = Mov_Dett_Raccolta.fase_Cod AND ICF.lotto = Mov_Dett_Raccolta.Lotto  ")
            'StbSQL.appendline(" -- non join sul cal_cod! in ICF è 0, mentre nei dettagli è valorizzato con il progressivo negativo " )
            'StbSQL.appendline(" -- AND ICF.cal_cod = Mov_Dett_Raccolta.cal_cod" )
            'StbSQL.appendline(" -- non join su udm_cod! in ICF è 4 (tonnellate), mentre nei dettagli è 2 (kg) e io ho bisogno di recuperare il premio euro/ton del pomo bio" )
            'StbSQL.appendline(" -- AND ICF.udm_cod= Mov_Dett_Raccolta.udm_cod " )

            StbSQL.AppendLine(" LEFT OUTER JOIN Imprese_Contratti IC ON ICF.PIVA = IC.PIVA AND ICF.Contratto_Cod = IC.Contratto_Cod ")
            StbSQL.AppendLine(" ")

            'StbSQL.AppendLine(" LEFT JOIN (SELECT progressivo, CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.')) AS Dif_Magg_Marcio  ")
            'StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            'StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'omarcio'  ")
            'StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 0 ")
            'StbSQL.AppendLine("             ) AS MPC_1a on MPC_1a.progressivo = Mov_Dett_Raccolta.cal_cod  ")
            'StbSQL.AppendLine(" LEFT JOIN (SELECT progressivo, CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.')) AS Dif_Magg_Verde  ")
            'StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            'StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'overde'  ")
            'StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 0 ")
            'StbSQL.AppendLine("             ) AS MPC_1b on MPC_1b.progressivo = Mov_Dett_Raccolta.cal_cod  ")
            'StbSQL.AppendLine(" LEFT JOIN (SELECT progressivo, CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.')) AS Dif_Magg_Inerti  ")
            'StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            'StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'oinerti'  ")
            'StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 0 ")
            'StbSQL.AppendLine("             ) AS MPC_1c on MPC_1c.progressivo = Mov_Dett_Raccolta.cal_cod  ")
            'StbSQL.AppendLine(" LEFT JOIN (SELECT progressivo, SUM(CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.'))) AS Tot_Dif_Min  ")
            'StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            'StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo IN ('ofruttiimmaturi','ofruttilesionati','ofruttischiacciati','ofruttiscottati')  ")
            'StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 0 ")
            'StbSQL.AppendLine("             GROUP BY progressivo) AS MPC_2 on MPC_2.progressivo = Mov_Dett_Raccolta.cal_cod  ")
            'StbSQL.AppendLine(" LEFT JOIN (SELECT progressivo, CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.')) AS Frutti_Schiacciati ")
            'StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            'StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'ofruttischiacciati'  ")
            'StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 0 ")
            'StbSQL.AppendLine("             ) AS MPC_3 ON MPC_3.progressivo = Mov_Dett_Raccolta.cal_cod  ")
            'StbSQL.AppendLine(" LEFT JOIN (SELECT progressivo, CONVERT(float, REPLACE(Materie_Prime_Campionature.val_cod,',','.')) AS Grado_Brix ")
            'StbSQL.AppendLine("             FROM Materie_Prime_Campionature ")
            'StbSQL.AppendLine("             WHERE  Materie_Prime_Campionature.tipo = 'ogradobrix'  ")
            'StbSQL.AppendLine("             AND Materie_Prime_Campionature.tipo_cod = 0 ")
            'StbSQL.AppendLine("             ) AS MPC_4 ON MPC_4.progressivo = Mov_Dett_Raccolta.cal_cod  ")

            StbSQL.AppendLine(" INNER JOIN Materie_Prime_Campionature ")
            StbSQL.AppendLine(" ON Materie_Prime_Campionature.progressivo = Mov_Dett_Raccolta.cal_cod ")
            StbSQL.AppendLine(" AND Materie_Prime_Campionature.tipo_cod = 0 AND Materie_Prime_Campionature.tipo IN ('ogradobrix','omarcio','overde','oinerti','ofruttiimmaturi','ofruttilesionati','ofruttischiacciati','ofruttiscottati') ")

            StbSQL.AppendLine(" ")

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod = " + Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) + "")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione = -1 ")
            'StbSQL.AppendLine(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  ") ' 0 = NO SURGELATO

            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov      = '" + Agro_SQL_SaveText(CAU_REGISTRAZIONI) + "'")
            'Mov_Certificato non esiste nel nuovo conferimento
            'StbSQL.AppendLine(" AND Mov_Certificato.Cau_Mov = '" + Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) + "'")
            StbSQL.AppendLine(" AND Mov_Conf.Cau_Mov        = '" + Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) + "'")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov    = '" + Agro_SQL_SaveText(CAU_CARICO) + "'")

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine(" AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " " + vbCrLf)

            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento <= " + Agro_SQL_SaveDate(Data_Fine) + " ")
            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento >= " + Agro_SQL_SaveDate(Data_Inizio) + " ")

            'Nel caso dell'export excel id_agenda è 0
            If Id_Agenda <> 0 Then
                StbSQL.AppendLine(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            End If

            If Sa_Cod <> 0 Then
                StbSQL.AppendLine(" AND     Mov_Dest_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
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
                StbSQL.AppendLine(" AND Risorse_Umane_Coop.Cod_contatto = '" & Agro_SQL_SaveText(Piva_Coop1) & "' ")
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

            '28/08/2018: eliminato dall'ordinamento il numero certificato che a volte viene salvato
            ''31/07/2018: nell'ordinamento introdotto il numero bolla perchè il numero certificato è sempre a 0  (da quando non esiste più il certificato esterno)
            ''è capitato un caso in cui nell'excel le righe non venivano ordinate bene (avevano stessa data e ora)
            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'StbSQL.appendline(" ORDER BY Mov_Certificato.Doc_Numero_Sin, Mov_Certificato.Doc_Numero, Mov_Certificato.Doc_Numero_Des, Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero,Mov_Accett.Doc_Numero_Des, Mov_Certificato.Data_Movimento, Mov_Accett.Ora ")
                'StbSQL.AppendLine(" ORDER BY Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero,Mov_Accett.Doc_Numero_Des, Mov_Certificato.Data_Movimento, Mov_Accett.Ora ")
                StbSQL.AppendLine(" ORDER BY Mov_Accett.Doc_Numero_Visualizzato, Mov_Accett.Data_Movimento, Mov_Accett.Ora ")
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
    ''' '''  
    ''' Viene utilizzata per l'esportazione conferimenti pomodoro nel tracciato OI POMODORO
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function CertificatiPomodoro_OIPomodoro_Export(ByVal Piva As String,
                                                        ByVal Data_Inizio As Date,
                                                        ByVal Data_Fine As Date,
                                                        ByVal Cod_RisUm As Integer,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Conferimento_Pomodoro.CertificatiPomodoro_OIPomodoro_Export"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.AppendLine(" SELECT  Agenda.Id_Agenda, Agenda.des_lib,  ")

            StbSQL.AppendLine("         Mov_Dett_Conf.Tagliando_Pesa, Mov_Dett_Conf.Premio_Complessivo AS Premio_PomoTardivo, ")
            StbSQL.AppendLine("         Mov_Dett_Conf.Desc_Appezzamenti AS Descrizione_Appezzamento, Mov_Dett_Conf.Cod_Varieta AS CodiceVarieta_OI, ")

            'StbSQL.AppendLine("         Mov_Certificato.Data_Movimento AS Data_Certificato, ")
            'StbSQL.AppendLine("         Mov_Certificato.extra_int AS Tagliando_Pesa,  Mov_Certificato.extra_Str AS Premio_PomoTardivo, ")
            'StbSQL.AppendLine("         Mov_Certificato.Username_Note AS Descrizione_Appezzamento, Mov_Certificato.Sezionale_Cod AS CodiceVarieta_Distretto, ")
            StbSQL.AppendLine("         ISNULL(IC.Contratto_Numero, '0') AS Contratto_Numero, ISNULL(IC.Contratto_Nome, '') AS Cod_Agrea_Contratto, ISNULL(IC.Data_Stipulazione, '01/01/1900') AS Data_Stipulazione,  ")
            StbSQL.AppendLine("         ISNULL(  (SELECT TOP 1 Imprese_Contratti_Clausole.Clausola_Cod_Alternativo + '|' + convert(varchar(250), Imprese_Contratti_Clausole.Validita_Inizio, 103) ")
            StbSQL.AppendLine("                     FROM Imprese_Contratti_Clausole ")
            StbSQL.AppendLine("                     WHERE Mov_Accett.Num_Protocollo = Imprese_Contratti_Clausole.Clausola_Cod ")
            StbSQL.AppendLine("                     ), '' ) AS Clausola, ")
            StbSQL.AppendLine("         Mov_Accett.Data_Movimento AS Data_Accett, Mov_Accett.Ora AS Ora_Accett,   ")
            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des, ")
            StbSQL.AppendLine("         Mov_Accett.Mov_Desc AS Note, ")
            'nuovo conf: pesi
            ' StbSQL.Appendline(" Mov_Accett.Peso, Mov_Accett.Tara_Imballi, Mov_Accett.Tipo_Peso,  " + vbCrLf)
            StbSQL.AppendLine("         Mov_Accett.Peso AS Peso_Totale, Mov_Accett.Tara_Veicolo,   ")
            StbSQL.AppendLine("         -- Mov_Accett.Tara_Imballi AS Tara_Imballi_Testata  , ")
            StbSQL.AppendLine("         Mov_Dett_Raccolta.Tara AS Tara_Dettaglio, 0.0 AS Tara_Imballi, ")
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

            'novità new conf
            StbSQL.AppendLine(" Mov_Dett_Raccolta.ordine_det, Mov_Dett_Raccolta.qta_extra_totale AS peso_netto, " + vbCrLf)

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
            StbSQL.AppendLine("         Mov_Accett.Cod_RisUm, Mov_Accett.Doc_Numero_Visualizzato as Numero_Bolla,")
            StbSQL.AppendLine("         Contatti_Conferenti.Cod_Contatto AS Cod_Contatto_Conferente,  ")
            StbSQL.AppendLine("         Contatti_Conferenti.Rag_Soc AS Rag_Soc_Conferente,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Coop.Cod_Contatto, '') AS Cod_Contatto_Coop,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Coop.Rag_Soc, '') AS Rag_Soc_Coop,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Produttori.Cod_Contatto, '') AS Cod_Contatto_Produttore,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Produttori.Rag_Soc, '') AS Rag_Soc_Produttore, ISNULL(Contatti_Produttori.Codice_Fiscale, '') AS Codice_Fiscale_Produttore,  ")
            StbSQL.AppendLine("         ISNULL(Contatti_Produttori.Nome, '') AS Nome_Produttore, ISNULL(Contatti_Produttori.Cognome, '') AS Cognome_Produttore,  ")
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
            'StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   ")
            StbSQL.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Mdt_Extra ON Mdt_Extra.PIVA = Mov_Accett.PIVA AND Mdt_Extra.Sa_Cod = Mov_Accett.Sa_Cod  ")
            StbSQL.AppendLine(" AND Mdt_Extra.Id_Agenda = Mov_Accett.Id_Agenda AND Mdt_Extra.Id_Mov = Mov_Accett.Id_Mov ")
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
            StbSQL.AppendLine(" Left Join CentrixIndirizzi CentrixIndirizzi_Trasf ON C_Az_Raccolta.PIVA = CentrixIndirizzi_Trasf.Piva And C_Az_Raccolta.Sa_Cod = CentrixIndirizzi_Trasf.Sa_Cod And CentrixIndirizzi_Trasf.Tipo_Indirizzo=1 ")
            StbSQL.AppendLine(" Left Join Indirizzi Indirizzi_Trasf ON CentrixIndirizzi_Trasf.cod_indirizzo = Indirizzi_Trasf.cod_indirizzo ")
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

            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod = " + Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) + "")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione = -1 ")
            'StbSQL.AppendLine(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  ") ' 0 = NO SURGELATO
            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov      = '" + Agro_SQL_SaveText(CAU_REGISTRAZIONI) + "'")
            'Mov_Certificato non esiste nel nuovo conferimento
            'StbSQL.AppendLine(" AND Mov_Certificato.Cau_Mov = '" + Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) + "'")
            StbSQL.AppendLine(" AND Mov_Conf.Cau_Mov        = '" + Agro_SQL_SaveText(CAU_REGISTRAZIONI_ALLEGATE) + "'")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov    = '" + Agro_SQL_SaveText(CAU_CARICO) + "'")

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine(" AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " " + vbCrLf)

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
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal Doc_Numero_Sin As String,
                                                       ByVal Doc_Numero_Des As String,
                                                       ByVal ElencoNumeriBolla As String,
                                                       ByVal Data_Inizio As Date,
                                                       ByVal Data_Fine As Date,
                                                      ByVal Cod_RisUm As Integer,
                                                       ByVal Piva_Produttore As String,
                                                       ByVal Piva_Coop1 As String,
                                                        ByVal Mat_Cod As Integer,
                                                        ByVal Veg_Cod As Integer,
                                                        ByVal Cul_Cod As Integer,
                                                        ByVal FiltroAggArticoli As String,
                                                        ByVal FiltroAggConferenti As String,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByVal xOrderBy As String,
                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                       ) As DataTable

        'ByVal Tipo_Certificato As enum_CodificaStampe,

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Conferimento_Pomodoro.CertificatiPomodoro_IdAgenda_Leggi"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.Length = 0

            'non metto il DISTINCT perchè ad oggi i certificati di pomodoro hanno un solo dettaglio
            StbSQL.AppendLine("SELECT Agenda.Id_Agenda, Agenda.des_lib, ")
            ' StbSQL.AppendLine("         Mov_Certificato.Data_Movimento, ")
            'StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin AS Doc_Numero_Sin_Bolla, Mov_Accett.Doc_Numero AS Doc_Numero_Bolla, Mov_Accett.Doc_Numero_Des AS Doc_Numero_Des_Bolla,  ")
            'StbSQL.AppendLine("         Mov_Certificato.Doc_Numero_Sin AS Doc_Numero_Sin_Cert, Mov_Certificato.Doc_Numero AS Doc_Numero_Cert, Mov_Certificato.Doc_Numero_Des AS Doc_Numero_Des_Cert  ")

            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Visualizzato AS Numero_Bolla, " + vbCrLf)
            StbSQL.AppendLine("         Mov_Accett.Doc_Numero_Sin, Mov_Accett.Doc_Numero, Mov_Accett.Doc_Numero_Des ")

            StbSQL.AppendLine(" FROM    Agenda ")
            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Accett ON Agenda.PIVA = Mov_Accett.PIVA AND Agenda.Sa_Cod = Mov_Accett.Sa_Cod AND Agenda.Id_Agenda = Mov_Accett.Id_Agenda ")
            'StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Certificato ON Agenda.PIVA = Mov_Certificato.PIVA AND Agenda.Sa_Cod = Mov_Certificato.Sa_Cod AND Agenda.Id_Agenda = Mov_Certificato.Id_Agenda   ")

            StbSQL.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Conferenti ON Mov_Accett.Cod_RisUm = Risorse_Umane_Conferenti.Cod_RisUm  ")

            If Piva_Produttore <> "" Then
                StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Produttori ON Mov_Accett.Cod_Destinazione = Risorse_Umane_Produttori.Cod_RisUm  ")
            End If

            If Piva_Coop1 <> "" Then
                StbSQL.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Coop1 ON Mov_Accett.Cod_RisUm_Altro = Risorse_Umane_Coop1.Cod_RisUm  ")
            End If

            StbSQL.AppendLine(" INNER JOIN Movimenti Mov_Raccolta ON Agenda.PIVA = Mov_Raccolta.PIVA AND Agenda.Sa_Cod = Mov_Raccolta.Sa_Cod AND Agenda.Id_Agenda = Mov_Raccolta.Id_Agenda  ")
            StbSQL.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Raccolta ON Mov_Raccolta.PIVA = Mov_Dett_Raccolta.PIVA AND Mov_Raccolta.Id_Agenda = Mov_Dett_Raccolta.Id_Agenda AND Mov_Raccolta.Id_Mov = Mov_Dett_Raccolta.Id_Mov   ")
            StbSQL.AppendLine(" INNER JOIN Materie_Prime MP_Raccolta ON Mov_Dett_Raccolta.Elem_Cod = MP_Raccolta.Elem_Cod AND Mov_Dett_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod ")

            'gestione conferimento pomodoro 2010
            'If Tipo_Certificato = enum_CodificaStampe.Certificato_Pomodoro_Esterno Then
            '    'il certificato esterno va stampato solo per i non surgelati
            '    StbSQL.AppendLine(" INNER JOIN Materie_Prime_Dettagli MP_Dettagli_Raccolta ON MP_Dettagli_Raccolta.Piva = MP_Raccolta.Piva AND MP_Dettagli_Raccolta.Mat_Cod = MP_Raccolta.Mat_Cod  ")
            'End If


            StbSQL.AppendLine(" WHERE Agenda.Lav_Cod = " + Agro_SQL_SaveNum(CStr(LAVCOD_ACCETTAZIONE_DIVERSI)) + "")
            StbSQL.AppendLine(" AND Agenda.Tipo_Accettazione = -1 ")
            StbSQL.AppendLine(" AND Mov_Accett.Cau_Mov      = '" + Agro_SQL_SaveText(CAU_REGISTRAZIONI) + "'")
            'Mov_Certificato non esiste nel nuovo conferimento
            'StbSQL.AppendLine(" AND Mov_Certificato.Cau_Mov = '" + Agro_SQL_SaveText(CAU_REGISTRAZIONI_TERZIARIA) + "'")
            StbSQL.AppendLine(" AND Mov_Raccolta.Cau_Mov    = '" + Agro_SQL_SaveText(CAU_CARICO) + "'")

            StbSQL.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            'per scartare beni confez e riga desc libera
            StbSQL.AppendLine(" AND Mov_Dett_Raccolta.elem_cod = " & TRASFORMATI_VEGETALI.ToString & " " + vbCrLf)

            'StbSQL.AppendLine(" AND Mov_Certificato.Data_Movimento   <=" & Agro_SQL_SaveDate(Validita_Fine) & " " )
            'StbSQL.AppendLine(" AND Mov_Certificato.Data_Movimento   >=" & Agro_SQL_SaveDate(Validita_Inizio) & " " )
            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento <= " + Agro_SQL_SaveDate(Data_Fine) + " ")
            StbSQL.AppendLine(" AND Mov_Accett.Data_Movimento >= " + Agro_SQL_SaveDate(Data_Inizio) + " ")

            If Sa_Cod <> 0 Then
                StbSQL.AppendLine(" AND     Mov_Dett_Raccolta.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

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
                StbSQL.AppendLine(FiltroAggConferenti)
            End If

            If Piva_Produttore <> "" Then
                StbSQL.AppendLine(" AND Risorse_Umane_Produttori.Cod_contatto = '" & Agro_SQL_SaveText(Piva_Produttore) & "' ")
            End If

            If Piva_Coop1 <> "" Then
                StbSQL.AppendLine(" AND Risorse_Umane_Coop1.Cod_contatto = '" & Agro_SQL_SaveText(Piva_Coop1) & "' ")
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

            'If Tipo_Certificato = enum_CodificaStampe.Certificato_Pomodoro_Esterno Then
            '    'il certificato esterno va stampato solo per i non surgelati
            '    StbSQL.AppendLine(" AND MP_Dettagli_Raccolta.Extra_Smallint4 = 0  ")
            'End If

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

    Public Function CertificatiPomodoro_LeggiReferenti(
        ByVal Piva As String, ByVal Cod_Rapporto As String, ByVal Id_Cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal xFiltroAggiuntivo As String = "", Optional ByVal xOrderBy As String = "") As DataTable

        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Conferimento_Pomodoro.CertificatiPomodoro_LeggiReferenti"

        Dim MessaggioErrore As String = ""
        Dim StbSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StbSQL.AppendLine("SELECT c.Piva, c.Cod_Contatto, c.Rag_Soc, ru.Cod_RisUm, ru.Cod_Rapporto, id_cod, val_cod")
            StbSQL.AppendLine("FROM Contatti c INNER JOIN Risorse_Umane ru ON ru.Piva = c.Piva AND ru.Cod_Contatto = c.Cod_Contatto")
            StbSQL.AppendLine("INNER JOIN Imprese_Codici ic ON ic.Piva=c.Cod_Contatto")
            StbSQL.AppendLine("WHERE ic.val_cod<>'' AND ic.val_cod<>'000' ")

            If Not String.IsNullOrEmpty(Piva) Then
                StbSQL.AppendLine("  AND c.Cod_Contatto = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Not String.IsNullOrEmpty(Cod_Rapporto) Then
                StbSQL.AppendLine("  AND ru.Cod_Rapporto IN (" & Agro_SQL_Save_Clausola_IN(Cod_Rapporto) & ")")
            End If

            If Id_Cod <> 0 Then
                StbSQL.AppendLine("  AND ic.id_cod = " & Agro_SQL_SaveNum(Id_Cod))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StbSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StbSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StbSQL.AppendLine(" ORDER BY c.Rag_Soc ")
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


End Class
