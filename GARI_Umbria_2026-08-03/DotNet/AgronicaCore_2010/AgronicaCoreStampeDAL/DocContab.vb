Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class DocContab
    Inherits DataProvider

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge i seguenti documenti:
    ''' fattura, ddt, ricevuta, nota di accredito, bolla di conferimento
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function DocumentiContabili(ByVal PivaImpresa As String,
                                       ByVal PivaProprietaria As String,
                                       ByVal Lav_Cod As Integer,
                                       ByVal Id_Agenda As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByVal Cod_Report As enum_CodificaStampe,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocContab.DocumentiContabili"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable
        'Dim i As Integer

        Try

            stbSql.Length = 0

            stbSql.AppendLine(" SELECT * ")
            stbSql.AppendLine(" FROM ")

            stbSql.AppendLine(" ( ")


            '------------------------------------------------------------------------
            '---- PRIMA PARTE DELL'UNION: dettagli con magazzino --------------------
            '------------------------------------------------------------------------
            stbSql.AppendLine(" ( ")
            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" SELECT Agenda.PIVA, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, ")
            stbSql.AppendLine(" Contatti_Impresa.Rag_Soc AS Rag_Soc_Impresa, Contatti_Impresa.Codice_Fiscale AS Codice_Fiscale_Impresa,  ")

            stbSql.AppendLine(" Mov_Contabile.Id_Mov AS Id_Mov_Contabile, Mov_Contabile.Data_Movimento, Mov_Contabile.Ora , ")
            stbSql.AppendLine(" Mov_Contabile.Doc_Numero_Sin, Mov_Contabile.Doc_Numero,Mov_Contabile.Doc_Numero_Des, Mov_Contabile.Mov_Desc ,  ")
            stbSql.AppendLine(" Mov_Contabile.Scadenza, Mov_Contabile.Num_Protocollo, Mov_Contabile.Causale_Trasporto, Mov_Contabile.Aspetto, Mov_Contabile.Peso,  ")
            stbSql.AppendLine(" Mov_Contabile.Colli, Mov_Contabile.Extra_Str, Mov_Contabile.Extra_Int, Mov_Contabile.Extra_Date,  ")
            stbSql.AppendLine(" Mov_Contabile.Tipo_Sconto, Mov_Contabile.Natura_Beni, Mov_Contabile.Tara_Veicolo, Mov_Contabile.Tara_Imballi, Mov_Contabile.Tipo_Peso, Mov_Contabile.Modalita, Mov_Contabile.Username_Note,  ")
            stbSql.AppendLine(" Mov_Contabile.Scadenza_Extra, Mov_Contabile.Progr_Protocollo, Mov_Contabile.Progr_Registrazione, Mov_Contabile.Data_Registrazione,  ")
            stbSql.AppendLine(" Mov_Contabile.ChkLayOut_Join_Prodotti, Mov_Contabile.chklayout_bypass_fatturato, Mov_Contabile.ChkLayOut_Peso, Mov_Contabile.ChkLayOut_Prezzo, Mov_Contabile.ChkLayOut_Riscontrato, Mov_Contabile.ChkLayOut_Litri,  ")
            stbSql.AppendLine(" Mov_Contabile.chkfiltro_varietale AS Flag_UveDiraspate,")
            stbSql.AppendLine(" Mov_Contabile.Cod_RisUm, Contatti_Cliente.Cod_Contatto AS Cod_Contatto_Cliente, Contatti_Cliente.Rag_Soc AS Rag_Soc_Cliente, Contatti_Cliente.Codice_Fiscale AS Codice_Fiscale_Cliente,  ")
            stbSql.AppendLine(" Contatti_Cliente.Id_Cf AS Id_Cf_Cliente, Contatti_Cliente.Nome AS Nome_Cliente, Contatti_Cliente.Cognome AS Cognome_Cliente, ISNULL(Contatti_Cliente.ChkFittizio, 0) AS ChkFittizio, ")
            stbSql.AppendLine("  Mov_Contabile.Cod_IndirizzoRisUm,  ")
            'stbSql.AppendLine(" Indirizzi_Cliente.ind_des AS ind_des_Cliente, Indirizzi_Cliente.frz_des AS frz_des_Cliente, Indirizzi_Cliente.CAP AS cap_Cliente,  ")
            'stbSql.AppendLine(" Indirizzi_Cliente.stato AS stato_Cliente, Indirizzi_Cliente.pro_cod_istat AS pro_cod_istat_Cliente,  ")
            'stbSql.AppendLine(" Indirizzi_Cliente.com_cod_istat AS com_cod_istat_Cliente, Istat_Cliente.LOCALITA AS localita_Cliente, Istat_Cliente.COMUNI_PROV AS comuni_prov_Cliente, ")
            'stbSql.AppendLine("  ")
            stbSql.AppendLine(" Mov_Contabile.Cod_Destinazione, Mov_Contabile.Cod_IndirizzoDestinazione,  ")

            '  Giulia, 28/02/2017 16:16:43: Aggiunta di Cessionario Aggiuntivo
            stbSql.AppendLine(" Mov_Contabile.Cod_RisUm_Aggiuntivo, Mov_Contabile.Cod_Indirizzo_Aggiuntivo,  ")

            '27/03/2019: x split payment
            stbSql.AppendLine(" Mov_Contabile.Sezionale_Cod,  ")

            'stbSql.AppendLine(" ISNULL(Contatti_DestDiv.Cod_Contatto, '') AS Cod_Contatto_DestDiv, ISNULL(Contatti_DestDiv.Rag_Soc, '') AS Rag_Soc_DestDiv, ISNULL(Contatti_DestDiv.Codice_Fiscale, '') AS Codice_Fiscale_DestDiv,  ")
            'stbSql.AppendLine(" ISNULL(Contatti_DestDiv.Id_Cf, 0) AS Id_Cf_DestDiv, ISNULL(Contatti_DestDiv.Nome, '') AS Nome_DestDiv, ISNULL(Contatti_DestDiv.Cognome, '') AS Cognome_DestDiv,  ")
            'stbSql.AppendLine(" ISNULL(Indirizzi_DestDiv.ind_des, '') AS ind_des_DestDiv, ISNULL(Indirizzi_DestDiv.frz_des, '') AS frz_des_DestDiv, ISNULL(Indirizzi_DestDiv.CAP, '') AS cap_DestDiv,  ")
            'StbSQL.AppendLine(" ISNULL(Indirizzi_DestDiv.stato, '') AS stato_DestDiv, ISNULL(Indirizzi_DestDiv.pro_cod_istat, '') AS pro_cod_istat_DestDiv,  ")
            'stbSql.AppendLine(" ISNULL(Indirizzi_DestDiv.com_cod_istat, '') AS com_cod_istat_DestDiv, ISNULL(Istat_DestDiv.LOCALITA, '') AS localita_DestDiv, ISNULL(Istat_DestDiv.COMUNI_PROV, '') AS comuni_prov_DestDiv, ")
            'stbSql.AppendLine("  ")
            stbSql.AppendLine(" Mov_Contabile.Mezzo, Mov_Contabile.Cod_Vettore, Mov_Contabile.Cod_IndirizzoVettore,  ")
            '17/05/2017 maga: aggiunto N_Autorizzazione_Trasporto
            stbSql.AppendLine(" ISNULL(Movimento_Extra_0.Targa, '') AS TargaMezzoVettore, ISNULL(Movimento_Extra_0.N_Autorizzazione_Trasporto, '' ) AS N_Autorizzazione_Trasporto, ")
            stbSql.AppendLine(" ISNULL(Movimento_Extra_0.Id_Gestione_Vettore, 0) AS Id_Gestione_Vettore,   ")
            '  Giulia, 15/11/2016 10.11.00: Aggiunta di codice risUm agente
            stbSql.AppendLine(" Movimento_Extra_0.Agente_Cod, ")

            'stbSql.AppendLine(" ISNULL(Contatti_Vettore.Cod_Contatto, '') AS Cod_Contatto_Vettore, ISNULL(Contatti_Vettore.Rag_Soc, '') AS Rag_Soc_Vettore, ISNULL(Contatti_Vettore.Codice_Fiscale, '') AS Codice_Fiscale_Vettore,  ")
            'stbSql.AppendLine(" ISNULL(Contatti_Vettore.Id_Cf, 0) AS Id_Cf_Vettore, ISNULL(Contatti_Vettore.Nome, '') AS Nome_Vettore, ISNULL(Contatti_Vettore.Cognome, '') AS Cognome_Vettore,  ")
            'stbSql.AppendLine(" ISNULL(Indirizzi_Vettore.ind_des, '') AS ind_des_Vettore, ISNULL(Indirizzi_Vettore.frz_des, '') AS frz_des_Vettore, ISNULL(Indirizzi_Vettore.CAP, '') AS cap_Vettore,  ")
            'stbSql.AppendLine(" ISNULL(Indirizzi_Vettore.stato, '') AS stato_Vettore, Indirizzi_Vettore.pro_cod_istat AS pro_cod_istat_Vettore,  ")
            'stbSql.AppendLine(" Indirizzi_Vettore.com_cod_istat AS com_cod_istat_Vettore, ISNULL(Istat_Vettore.LOCALITA, '') AS localita_Vettore, ISNULL(Istat_Vettore.COMUNI_PROV, '') AS comuni_prov_Vettore,  ")
            'stbSql.AppendLine("  ")
            ''stbSql.AppendLine(" Parco_Macchine.Class_Code, Parco_Macchine.Mac_Des, Parco_Macchine.Targa, Parco_Macchine.Telaio, Parco_Macchine.Modello,  ")
            ''stbSql.AppendLine(" Parco_Macchine.Potenza, Parco_Macchine.Data_Immatricolazione, Parco_Macchine.N_Immatricolazione,  ")
            ''stbSql.AppendLine(" Parco_Macchine.N_Immatricolazione_Rimorchio, Parco_Macchine.N_Autorizzazione_Trasporto, Parco_Macchine.Data_Rilascio_Autorizzazione, Parco_Macchine.Peso, ")
            ''stbSql.AppendLine("  ")
            'stbSql.AppendLine(" Movimento_Extra.Mac_Cod,  Movimento_Extra.Mezzo_Trasporto, Movimento_Extra.Targa AS Targa_2, Movimento_Extra.N_Immatricolazione AS N_Immatricolazione_2, Movimento_Extra.N_Immatricolazione_Rimorchio AS N_Immatricolazione_Rimorchio_2, ")
            'stbSql.AppendLine(" Movimento_Extra.N_Autorizzazione_Trasporto AS N_Autorizzazione_Trasporto_2, Movimento_Extra.Data_Rilascio_Autorizzazione AS Data_Rilascio_Autorizzazione_2, Movimento_Extra.Peso AS Peso_2, ")
            'stbSql.AppendLine("  ")
            'stbSql.AppendLine(" Movimento_Extra.Id_Reg_Dettaglio AS Id_Movimento_Extra, Movimento_Extra.Indicazioni_Complementari, ")
            'stbSql.AppendLine(" Movimento_Extra.Tipo_Documento, Movimento_Extra.Luogo_Partenza, Movimento_Extra.Luogo_Consegna, Movimento_Extra.Id_Reg_Dettaglio AS Id_Movimento_Extra, Movimento_Extra.Indicazioni_Complementari, ")
            'stbSql.AppendLine(" Movimento_Extra.Data_Spedizione,  Movimento_Extra.Precisazioni, Movimento_Extra.Annotazioni,  ")
            'stbSql.AppendLine("  ")
            stbSql.AppendLine(" Mov_Magazzino.Id_Mov AS Id_Mov_Magazzino, Mov_Magazzino.Mov_Desc AS Mov_Desc_Mag, Mov_Magazzino.Data_Movimento AS Data_Mag,Mov_Magazzino.Ora AS Ora_Mag,  ")
            'stbSql.AppendLine("  ")
            'stbSql.AppendLine(" Dettagli_Extra.Id_Reg_Dettaglio AS Id_Dettagli_Extra, ISNULL(Dettagli_Extra.Marche_Contenitori, '') AS Marche_Contenitori,  ")
            'stbSql.AppendLine(" ISNULL(Dettagli_Extra.Num_Contenitori, 0) AS Num_Contenitori, ISNULL(Dettagli_Extra.Des_Contenitori, '') AS Des_Contenitori, ISNULL(Dettagli_Extra.Num_Colli, 0) AS Num_Colli, Dettagli_Extra.Titolo_Alcol, ")
            'stbSql.AppendLine("  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Id_Mov_Det, Mov_Dett_Magazzino.Mov_Det_Des, Mov_Dett_Magazzino.Elem_Cod, Mov_Dett_Magazzino.Pro_Cod, Mov_Dett_Magazzino.Mat_Cod,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Cod_Progetto, Mov_Dett_Magazzino.Fase_Cod, Mov_Dett_Magazzino.Lotto, Mov_Dett_Magazzino.Cal_Cod, Mov_Dett_Magazzino.Udm_Cod, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Qta AS Qta, Mov_Dett_Magazzino.Variazione AS Variazione,Mov_Dett_Magazzino.Listino_Cod, Mov_Dett_Magazzino.Tara,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Prezzo_Unitario, Mov_Dett_Magazzino.Prezzo_Unitario_Netto, Mov_Dett_Magazzino.Imponibile, Mov_Dett_Magazzino.Imponibile_Netto,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Jolly_Int, Mov_Dett_Magazzino.Contabilizzato,Mov_Dett_Magazzino.Pendente, Mov_Dett_Magazzino.Prezzo_Effettivo, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Cod_Iva, Mov_Dett_Magazzino.Sconto, Mov_Dett_Magazzino.Cod_Conto, Mov_Dett_Magazzino.Extra_Str AS Extra_Str_Dett, Mov_Dett_Magazzino.Extra_Int AS Extra_Int_Dett, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Extra_Date AS Extra_Date_Dett, Mov_Dett_Magazzino.Anno, Mov_Dett_Magazzino.Ric_Cod, Mov_Dett_Magazzino.Iva, Mov_Dett_Magazzino.UDM_COD_EXTRA, Mov_Dett_Magazzino.QTA_EXTRA,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Cod_IvaIndetraibile, Mov_Dett_Magazzino.Qta_Extra_Totale, Mov_Dett_Magazzino.ChkLayOut_Hide, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.sconto_listino, ISNULL(Mov_Dett_Magazzino.sconto_Testo, '') AS Sconto_Testo, Mov_Dett_Magazzino.Sconto_modalita, Mov_Dett_Magazzino.chkiva_manuale, Mov_Dett_Magazzino.Mat_Cod_Alias, Mov_Dett_Magazzino.Mezzo_Det, ")
            stbSql.AppendLine(" IVA_Aliquote.Sigla AS Sigla_IVA, IVA_Aliquote.Aliquota, ")
            stbSql.AppendLine(" Mov_Destinazioni.Sa_cod AS Sa_Cod_Dest, Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Qta AS Qta_Dest ")
            stbSql.AppendLine(" , Mov_Dett_Magazzino.Sa_cod AS Sa_Cod_Dett ")

            '  Giulia, 16/01/2017 18:24:04: Aggiunta di Ordine_Det per ordinamento dettagli
            stbSql.AppendLine(" , CASE Mov_Dett_Magazzino.Ordine_Det WHEN 0 THEN Mov_Dett_Magazzino.Ordine_Det + 90000 ELSE Mov_Dett_Magazzino.Ordine_Det END as Ordine_Det ")

            stbSql.AppendLine(" , Mov_Dett_Magazzino.Qta_Dettaglio1, Mov_Dett_Magazzino.Qta_Dettaglio2 ")

            '  Giulia, 18/01/2017 09:25:55: Aggiunto livello prezzo imputato (contenitore, confezione, imballaggio)
            stbSql.AppendLine(" , Mov_Dett_Magazzino.Prezzo_Livello ")

            stbSql.AppendLine(" , Mov_Dett_Magazzino.PrincipiAttivi AS Perc_UveDiraspate ")

            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Num_Conf_Riscontrate,-1) AS Num_Conf_Riscontrate ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Num_Colli_Riscontrati, -1) AS Num_Colli_Riscontrati ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Num_Imballi_Riscontrati, -1) AS Num_Imballi_Riscontrati ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Peso_Netto_Riscontrato, -1) AS Peso_Netto_Riscontrato ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Peso_Lordo_Riscontrato, -1) AS Peso_Lordo_Riscontrato ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Tara_Unit_Conf_Riscontrata, -1) AS Tara_Unit_Conf_Riscontrata ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Tara_Unit_Collo_Riscontrata, -1) AS Tara_Unit_Collo_Riscontrata ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Tara_Unit_Imballo_Riscontrata, -1) AS Tara_Unit_Imballo_Riscontrata ")

            stbSql.AppendLine(" , ISNULL(Movimento_Extra.N_Doc_Cliente, '') AS N_Doc_Cliente ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Data_Doc_Cliente, '01/01/1900') AS Data_Doc_Cliente ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.N_Nota_Fattura, '') AS N_Nota_Fattura ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Data_Nota_Fattura, '01/01/1900') AS Data_Nota_Fattura ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.N_Nota_DDT, '') AS N_Nota_DDT ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.N_Nota_Riga_DDT, '') AS N_Nota_Riga_DDT ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Data_Nota_DDT, '01/01/1900') AS Data_Nota_DDT ")

            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Titolo_Alcol, 0) AS Titolo_Alcol ")


            'stbSql.AppendLine(" ,UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim,   ")
            ''Dettagli_Extra.Codice_Prodotto, Dettagli_Extra.Colore, Dettagli_Extra.Zona_Viticola, Dettagli_Extra.Manipolazioni, 
            'stbSql.AppendLine(" Materie_Prime.codice_prodotto, Materie_Prime.colore, Materie_Prime.codice_nc, Materie_Prime.manipolazioni ")
            'stbSql.AppendLine("  ")

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" FROM Agenda  ")
            'MODIFICA DEL 03/06/2014: aggiunta clausola di join, altrimenti per le aziende non superuser (ma contatti del superuser), venivano sdoppiati gli articoli
            stbSql.AppendLine(" INNER JOIN Contatti Contatti_Impresa ON Agenda.Piva = Contatti_Impresa.Cod_Contatto  ")
            'modifica del 30/06/14: verifico che la piva padre del contatto sia quella del superuser
            '(fabrizio non risuciva a stampare le fatture di net-agree)
            'StbSQL.AppendLine(" AND Agenda.Piva = Contatti_Impresa.Piva ")
            'stbSql.AppendLine(" AND Contatti_Impresa.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            If PivaProprietaria <> "" Then
                stbSql.AppendLine(" AND Contatti_Impresa.Piva = '" & Agro_SQL_SaveText(PivaProprietaria) & "'  ")
            End If

            stbSql.AppendLine(" INNER JOIN Movimenti Mov_Contabile ON Agenda.PIVA = Mov_Contabile.PIVA AND Agenda.Sa_Cod = Mov_Contabile.Sa_Cod AND Agenda.Id_Agenda = Mov_Contabile.Id_Agenda  ")
            stbSql.AppendLine("  ")
            stbSql.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Cliente ON Mov_Contabile.Cod_RisUm = Risorse_Umane_Cliente.Cod_RisUm  ")
            stbSql.AppendLine(" INNER JOIN Contatti Contatti_Cliente ON Risorse_Umane_Cliente.Piva = Contatti_Cliente.Piva AND Risorse_Umane_Cliente.Cod_Contatto = Contatti_Cliente.Cod_Contatto  ")
            'StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Cliente ON  Mov_Contabile.Cod_IndirizzoRisUm = Indirizzi_Cliente.cod_indirizzo ")
            'StbSQL.AppendLine(" INNER JOIN ISTAT Istat_Cliente ON Indirizzi_Cliente.pro_cod_istat = Istat_Cliente.PROV AND Indirizzi_Cliente.com_cod_istat = Istat_Cliente.COM ")
            stbSql.AppendLine("  ")
            'stbSql.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_DestDiv ON Mov_Contabile.Cod_Destinazione = Risorse_Umane_DestDiv.Cod_RisUm  ")
            'stbSql.AppendLine(" LEFT OUTER JOIN Contatti Contatti_DestDiv ON Risorse_Umane_DestDiv.Piva = Contatti_DestDiv.Piva AND Risorse_Umane_DestDiv.Cod_Contatto = Contatti_DestDiv.Cod_Contatto  ")
            'stbSql.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_DestDiv ON  Mov_Contabile.Cod_IndirizzoDestinazione= Indirizzi_DestDiv.cod_indirizzo ")
            'stbSql.AppendLine(" LEFT OUTER JOIN ISTAT Istat_DestDiv ON Indirizzi_DestDiv.pro_cod_istat = Istat_DestDiv.PROV AND Indirizzi_DestDiv.com_cod_istat = Istat_DestDiv.COM ")
            'stbSql.AppendLine("  ")
            'stbSql.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Vettore ON Mov_Contabile.Cod_Vettore = Risorse_Umane_Vettore.Cod_RisUm  ")
            'stbSql.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Vettore ON Risorse_Umane_Vettore.Piva = Contatti_Vettore.Piva AND Risorse_Umane_Vettore.Cod_Contatto = Contatti_Vettore.Cod_Contatto  ")
            'stbSql.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Vettore ON  Mov_Contabile.Cod_IndirizzoVettore = Indirizzi_Vettore.cod_indirizzo ")
            'stbSql.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Vettore ON Indirizzi_Vettore.pro_cod_istat = Istat_Vettore.PROV AND Indirizzi_Vettore.com_cod_istat = Istat_Vettore.COM ")
            'stbSql.AppendLine("  ")
            'stbSql.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra ON Movimento_Extra.PIVA = Mov_Contabile.PIVA AND Movimento_Extra.Sa_Cod = Mov_Contabile.Sa_Cod AND Movimento_Extra.Id_Agenda = Mov_Contabile.Id_Agenda AND Movimento_Extra.Id_Mov = Mov_Contabile.Id_Mov ")
            'stbSql.AppendLine("  ")
            stbSql.AppendLine(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Sa_Cod = Mov_Magazzino.Sa_Cod AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  ")
            stbSql.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov  ")
            stbSql.AppendLine(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Destinazioni.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Destinazioni.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det ")

            stbSql.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Mov_Dett_Magazzino.Cod_Iva   ")

            stbSql.AppendLine(" LEFT OUTER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra_0 ON Movimento_Extra_0.PIVA = Mov_Contabile.PIVA AND Movimento_Extra_0.Sa_Cod = Mov_Contabile.Sa_Cod AND Movimento_Extra_0.Id_Agenda = Mov_Contabile.Id_Agenda AND Movimento_Extra_0.Id_Mov = Mov_Contabile.Id_Mov AND Movimento_Extra_0.Id_Mov_Det=0 ")
            stbSql.AppendLine(" LEFT OUTER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra ON Movimento_Extra.PIVA = Mov_Dett_Magazzino.PIVA AND Movimento_Extra.Sa_Cod = Mov_Dett_Magazzino.Sa_Cod AND Movimento_Extra.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Movimento_Extra.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Movimento_Extra.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det ")
            stbSql.AppendLine("  ")

            'stbSql.AppendLine("  ")
            'stbSql.AppendLine(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod AND Materie_Prime.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod   ")
            'stbSql.AppendLine("  ")
            'stbSql.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Dettagli_Extra ON Dettagli_Extra.PIVA = Mov_Dett_Magazzino.PIVA AND Dettagli_Extra.Sa_Cod = Mov_Dett_Magazzino.Sa_Cod AND Dettagli_Extra.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Dettagli_Extra.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Dettagli_Extra.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det ")
            'stbSql.AppendLine("  ")
            'stbSql.AppendLine(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod ")
            'stbSql.AppendLine("  ")


            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" WHERE Mov_Contabile.Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")

            'modifica del 24/08/2015: per il conferimento uva gestita la lettura del dettaglio con il totale delle uve
            Select Case Cod_Report

                Case enum_CodificaStampe.ConferimentoUva_DDTRicevuto,
                    enum_CodificaStampe.ConferimentoUva_DistintaCarico,
                    enum_CodificaStampe.ConferimentoUva_AutoDDT

                    stbSql.AppendLine(" AND    Mov_Magazzino.Cau_Mov = '" & CAU_REGISTRAZIONI_TERZIARIA & "'  ")

                Case Else

                    'i documenti in uscita hanno il cau_mov dello scarico, ma i resi hanno quello del carico 
                    'le bolle di conferimento hanno il loro cau_mov
                    stbSql.AppendLine(" AND    Mov_Magazzino.Cau_Mov IN ( '" &
                                            CAU_SCARICO & "', '" &
                                            CAU_CARICO & "', '" &
                                            CAU_ABBUONI & "', '" &
                                            CAU_CONFERIMENTO & "', '" &
                                            CAU_CONFERIMENTO_DIVERSI & "', '" &
                                            CAU_ACCETTAZIONE_BENI & "', '" &
                                            CAU_ACCETTAZIONE_BENI_DA_DIVERSI &
                                            "'  ) ")

            End Select

            If Lav_Cod <> 0 Then
                stbSql.AppendLine(" AND Agenda.Lav_Cod = " & CStr(Lav_Cod) & " ")
            End If

            If PivaImpresa <> "" Then
                stbSql.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(PivaImpresa) & "' ")
            End If

            If Id_Agenda <> 0 Then
                stbSql.AppendLine(" AND Agenda.Id_Agenda = " & CStr(Id_Agenda) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stbSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------


            stbSql.AppendLine(" ) ")
            stbSql.AppendLine(" UNION ALL ")

            '------------------------------------------------------------------------
            '---- SECONDA PARTE DELL'UNION: dettagli altri beni strumentali ---------
            '------------------------------------------------------------------------
            stbSql.AppendLine(" ( ")
            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" SELECT Agenda.PIVA, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, ")
            stbSql.AppendLine(" Contatti_Impresa.Rag_Soc AS Rag_Soc_Impresa, Contatti_Impresa.Codice_Fiscale AS Codice_Fiscale_Impresa,  ")

            stbSql.AppendLine(" Mov_Contabile.Id_Mov AS Id_Mov_Contabile, Mov_Contabile.Data_Movimento, Mov_Contabile.Ora , ")
            stbSql.AppendLine(" Mov_Contabile.Doc_Numero_Sin, Mov_Contabile.Doc_Numero,Mov_Contabile.Doc_Numero_Des, Mov_Contabile.Mov_Desc ,  ")
            stbSql.AppendLine(" Mov_Contabile.Scadenza, Mov_Contabile.Num_Protocollo, Mov_Contabile.Causale_Trasporto, Mov_Contabile.Aspetto, Mov_Contabile.Peso,  ")
            stbSql.AppendLine(" Mov_Contabile.Colli, Mov_Contabile.Extra_Str, Mov_Contabile.Extra_Int, Mov_Contabile.Extra_Date,  ")
            stbSql.AppendLine(" Mov_Contabile.Tipo_Sconto, Mov_Contabile.Natura_Beni, Mov_Contabile.Tara_Veicolo, Mov_Contabile.Tara_Imballi, Mov_Contabile.Tipo_Peso, Mov_Contabile.Modalita, Mov_Contabile.Username_Note,  ")
            stbSql.AppendLine(" Mov_Contabile.Scadenza_Extra, Mov_Contabile.Progr_Protocollo, Mov_Contabile.Progr_Registrazione, Mov_Contabile.Data_Registrazione,  ")
            stbSql.AppendLine(" Mov_Contabile.ChkLayOut_Join_Prodotti, Mov_Contabile.chklayout_bypass_fatturato, Mov_Contabile.ChkLayOut_Peso, Mov_Contabile.ChkLayOut_Prezzo, Mov_Contabile.ChkLayOut_Riscontrato, Mov_Contabile.ChkLayOut_Litri,   ")
            stbSql.AppendLine(" Mov_Contabile.chkfiltro_varietale AS Flag_UveDiraspate,")
            stbSql.AppendLine(" Mov_Contabile.Cod_RisUm, Contatti_Cliente.Cod_Contatto AS Cod_Contatto_Cliente, Contatti_Cliente.Rag_Soc AS Rag_Soc_Cliente, Contatti_Cliente.Codice_Fiscale AS Codice_Fiscale_Cliente,  ")
            stbSql.AppendLine(" Contatti_Cliente.Id_Cf AS Id_Cf_Cliente, Contatti_Cliente.Nome AS Nome_Cliente, Contatti_Cliente.Cognome AS Cognome_Cliente, ISNULL(Contatti_Cliente.ChkFittizio, 0) AS ChkFittizio,  ")
            stbSql.AppendLine("  Mov_Contabile.Cod_IndirizzoRisUm,  ")
            'stbSql.AppendLine(" Indirizzi_Cliente.ind_des AS ind_des_Cliente, Indirizzi_Cliente.frz_des AS frz_des_Cliente, Indirizzi_Cliente.CAP AS cap_Cliente,  ")
            'stbSql.AppendLine(" Indirizzi_Cliente.stato AS stato_Cliente, Indirizzi_Cliente.pro_cod_istat AS pro_cod_istat_Cliente,  ")
            'stbSql.AppendLine(" Indirizzi_Cliente.com_cod_istat AS com_cod_istat_Cliente, Istat_Cliente.LOCALITA AS localita_Cliente, Istat_Cliente.COMUNI_PROV AS comuni_prov_Cliente, ")
            'stbSql.AppendLine("  ")
            stbSql.AppendLine(" Mov_Contabile.Cod_Destinazione, Mov_Contabile.Cod_IndirizzoDestinazione,  ")

            '  Giulia, 28/02/2017 16:16:43: Aggiunta di Cessionario Aggiuntivo
            stbSql.AppendLine(" Mov_Contabile.Cod_RisUm_Aggiuntivo, Mov_Contabile.Cod_Indirizzo_Aggiuntivo,  ")

            '27/03/2019: x split payment
            stbSql.AppendLine(" Mov_Contabile.Sezionale_Cod,  ")

            'stbSql.AppendLine(" ISNULL(Contatti_DestDiv.Cod_Contatto, '') AS Cod_Contatto_DestDiv, ISNULL(Contatti_DestDiv.Rag_Soc, '') AS Rag_Soc_DestDiv, ISNULL(Contatti_DestDiv.Codice_Fiscale, '') AS Codice_Fiscale_DestDiv,  ")
            'stbSql.AppendLine(" ISNULL(Contatti_DestDiv.Id_Cf, 0) AS Id_Cf_DestDiv, ISNULL(Contatti_DestDiv.Nome, '') AS Nome_DestDiv, ISNULL(Contatti_DestDiv.Cognome, '') AS Cognome_DestDiv,  ")
            'stbSql.AppendLine(" ISNULL(Indirizzi_DestDiv.ind_des, '') AS ind_des_DestDiv, ISNULL(Indirizzi_DestDiv.frz_des, '') AS frz_des_DestDiv, ISNULL(Indirizzi_DestDiv.CAP, '') AS cap_DestDiv,  ")
            'stbSql.AppendLine(" ISNULL(Indirizzi_DestDiv.stato, '') AS stato_DestDiv, ISNULL(Indirizzi_DestDiv.pro_cod_istat, '') AS pro_cod_istat_DestDiv,  ")
            'stbSql.AppendLine(" ISNULL(Indirizzi_DestDiv.com_cod_istat, '') AS com_cod_istat_DestDiv, ISNULL(Istat_DestDiv.LOCALITA, '') AS localita_DestDiv, ISNULL(Istat_DestDiv.COMUNI_PROV, '') AS comuni_prov_DestDiv, ")
            'stbSql.AppendLine("  ")
            stbSql.AppendLine(" Mov_Contabile.Mezzo, Mov_Contabile.Cod_Vettore, Mov_Contabile.Cod_IndirizzoVettore,  ")
            '17/05/2017 maga: aggiunto N_Autorizzazione_Trasporto
            stbSql.AppendLine(" ISNULL(Movimento_Extra_0.Targa, '') AS TargaMezzoVettore, ISNULL(Movimento_Extra_0.N_Autorizzazione_Trasporto, '' ) AS N_Autorizzazione_Trasporto, ")
            stbSql.AppendLine(" ISNULL(Movimento_Extra_0.Id_Gestione_Vettore, 0) AS Id_Gestione_Vettore,   ")
            '  Giulia, 15/11/2016 10.11.00: Aggiunta di codice risUm agente
            stbSql.AppendLine(" Movimento_Extra_0.Agente_Cod, ")

            'stbSql.AppendLine(" ISNULL(Contatti_Vettore.Cod_Contatto, '') AS Cod_Contatto_Vettore, ISNULL(Contatti_Vettore.Rag_Soc, '') AS Rag_Soc_Vettore, ISNULL(Contatti_Vettore.Codice_Fiscale, '') AS Codice_Fiscale_Vettore,  ")
            'stbSql.AppendLine(" ISNULL(Contatti_Vettore.Id_Cf, 0) AS Id_Cf_Vettore, ISNULL(Contatti_Vettore.Nome, '') AS Nome_Vettore, ISNULL(Contatti_Vettore.Cognome, '') AS Cognome_Vettore,  ")
            'stbSql.AppendLine(" ISNULL(Indirizzi_Vettore.ind_des, '') AS ind_des_Vettore, ISNULL(Indirizzi_Vettore.frz_des, '') AS frz_des_Vettore, ISNULL(Indirizzi_Vettore.CAP, '') AS cap_Vettore,  ")
            'stbSql.AppendLine(" ISNULL(Indirizzi_Vettore.stato, '') AS stato_Vettore, Indirizzi_Vettore.pro_cod_istat AS pro_cod_istat_Vettore,  ")
            'stbSql.AppendLine(" Indirizzi_Vettore.com_cod_istat AS com_cod_istat_Vettore, ISNULL(Istat_Vettore.LOCALITA, '') AS localita_Vettore, ISNULL(Istat_Vettore.COMUNI_PROV, '') AS comuni_prov_Vettore,  ")
            'stbSql.AppendLine("  ")
            ''stbSql.AppendLine(" Parco_Macchine.Class_Code, Parco_Macchine.Mac_Des, Parco_Macchine.Targa, Parco_Macchine.Telaio, Parco_Macchine.Modello,  ")
            ''stbSql.AppendLine(" Parco_Macchine.Potenza, Parco_Macchine.Data_Immatricolazione, Parco_Macchine.N_Immatricolazione,  ")
            ''stbSql.AppendLine(" Parco_Macchine.N_Immatricolazione_Rimorchio, Parco_Macchine.N_Autorizzazione_Trasporto, Parco_Macchine.Data_Rilascio_Autorizzazione, Parco_Macchine.Peso, ")
            ''stbSql.AppendLine("  ")
            'stbSql.AppendLine(" Movimento_Extra.Mac_Cod,  Movimento_Extra.Mezzo_Trasporto, Movimento_Extra.Targa AS Targa_2, Movimento_Extra.N_Immatricolazione AS N_Immatricolazione_2, Movimento_Extra.N_Immatricolazione_Rimorchio AS N_Immatricolazione_Rimorchio_2, ")
            'stbSql.AppendLine(" Movimento_Extra.N_Autorizzazione_Trasporto AS N_Autorizzazione_Trasporto_2, Movimento_Extra.Data_Rilascio_Autorizzazione AS Data_Rilascio_Autorizzazione_2, Movimento_Extra.Peso AS Peso_2, ")
            'stbSql.AppendLine("  ")
            'stbSql.AppendLine(" Movimento_Extra.Id_Reg_Dettaglio AS Id_Movimento_Extra, Movimento_Extra.Indicazioni_Complementari, ")
            'stbSql.AppendLine(" Movimento_Extra.Tipo_Documento, Movimento_Extra.Luogo_Partenza, Movimento_Extra.Luogo_Consegna, Movimento_Extra.Id_Reg_Dettaglio AS Id_Movimento_Extra, Movimento_Extra.Indicazioni_Complementari, ")
            'stbSql.AppendLine(" Movimento_Extra.Data_Spedizione,  Movimento_Extra.Precisazioni, Movimento_Extra.Annotazioni,  ")
            'stbSql.AppendLine("  ")
            stbSql.AppendLine(" Mov_Magazzino.Id_Mov AS Id_Mov_Magazzino, Mov_Magazzino.Mov_Desc AS Mov_Desc_Mag, Mov_Magazzino.Data_Movimento AS Data_Mag,Mov_Magazzino.Ora AS Ora_Mag,  ")
            'stbSql.AppendLine("  ")
            'stbSql.AppendLine(" Dettagli_Extra.Id_Reg_Dettaglio AS Id_Dettagli_Extra, ISNULL(Dettagli_Extra.Marche_Contenitori, '') AS Marche_Contenitori,  ")
            'stbSql.AppendLine(" ISNULL(Dettagli_Extra.Num_Contenitori, 0) AS Num_Contenitori, ISNULL(Dettagli_Extra.Des_Contenitori, '') AS Des_Contenitori, ISNULL(Dettagli_Extra.Num_Colli, 0) AS Num_Colli, Dettagli_Extra.Titolo_Alcol, ")
            'stbSql.AppendLine("  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Id_Mov_Det, Mov_Dett_Magazzino.Mov_Det_Des, Mov_Dett_Magazzino.Elem_Cod, Mov_Dett_Magazzino.Pro_Cod, Mov_Dett_Magazzino.Mat_Cod,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Cod_Progetto, Mov_Dett_Magazzino.Fase_Cod, Mov_Dett_Magazzino.Lotto, Mov_Dett_Magazzino.Cal_Cod, Mov_Dett_Magazzino.Udm_Cod, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Qta AS Qta, Mov_Dett_Magazzino.Variazione AS Variazione,Mov_Dett_Magazzino.Listino_Cod, Mov_Dett_Magazzino.Tara,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Prezzo_Unitario, Mov_Dett_Magazzino.Prezzo_Unitario_Netto, Mov_Dett_Magazzino.Imponibile, Mov_Dett_Magazzino.Imponibile_Netto,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Jolly_Int, Mov_Dett_Magazzino.Contabilizzato,Mov_Dett_Magazzino.Pendente, Mov_Dett_Magazzino.Prezzo_Effettivo, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Cod_Iva, Mov_Dett_Magazzino.Sconto, Mov_Dett_Magazzino.Cod_Conto, Mov_Dett_Magazzino.Extra_Str AS Extra_Str_Dett, Mov_Dett_Magazzino.Extra_Int AS Extra_Int_Dett, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Extra_Date AS Extra_Date_Dett, Mov_Dett_Magazzino.Anno, Mov_Dett_Magazzino.Ric_Cod, Mov_Dett_Magazzino.Iva, Mov_Dett_Magazzino.UDM_COD_EXTRA, Mov_Dett_Magazzino.QTA_EXTRA,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Cod_IvaIndetraibile, Mov_Dett_Magazzino.Qta_Extra_Totale, Mov_Dett_Magazzino.ChkLayOut_Hide, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.sconto_listino, ISNULL(Mov_Dett_Magazzino.sconto_Testo, '') AS Sconto_Testo, Mov_Dett_Magazzino.Sconto_modalita, Mov_Dett_Magazzino.chkiva_manuale, Mov_Dett_Magazzino.Mat_Cod_Alias, Mov_Dett_Magazzino.Mezzo_Det, ")
            stbSql.AppendLine(" IVA_Aliquote.Sigla AS Sigla_IVA, IVA_Aliquote.Aliquota, ")
            '02/05/2012: gli altri beni strumentali non hanno il magazzino
            stbSql.AppendLine(" 0 AS Sa_Cod_Dest, 0 AS Id_Destinazione, 0 AS Tipo_Destinazione, 0 AS Qta_Dest ")
            stbSql.AppendLine(" , Mov_Dett_Magazzino.Sa_cod AS Sa_Cod_Dett ")

            '  Giulia, 16/01/2017 18:24:04: Aggiunta di Ordine_Det per ordinamento dettagli
            stbSql.AppendLine(" , CASE Mov_Dett_Magazzino.Ordine_Det WHEN 0 THEN Mov_Dett_Magazzino.Ordine_Det + 90000 ELSE Mov_Dett_Magazzino.Ordine_Det END as Ordine_Det ")

            stbSql.AppendLine(" , Mov_Dett_Magazzino.Qta_Dettaglio1, Mov_Dett_Magazzino.Qta_Dettaglio2 ")

            '  Giulia, 18/01/2017 09:25:55: Aggiunto livello prezzo imputato (contenitore, confezione, imballaggio)
            stbSql.AppendLine(" , Mov_Dett_Magazzino.Prezzo_Livello ")

            stbSql.AppendLine(" , Mov_Dett_Magazzino.PrincipiAttivi AS Perc_UveDiraspate ")

            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Num_Conf_Riscontrate,-1) AS Num_Conf_Riscontrate ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Num_Colli_Riscontrati, -1) AS Num_Colli_Riscontrati ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Num_Imballi_Riscontrati, -1) AS Num_Imballi_Riscontrati ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Peso_Netto_Riscontrato, -1) AS Peso_Netto_Riscontrato ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Peso_Lordo_Riscontrato, -1) AS Peso_Lordo_Riscontrato ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Tara_Unit_Conf_Riscontrata, -1) AS Tara_Unit_Conf_Riscontrata ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Tara_Unit_Collo_Riscontrata, -1) AS Tara_Unit_Collo_Riscontrata ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Tara_Unit_Imballo_Riscontrata, -1) AS Tara_Unit_Imballo_Riscontrata ")

            stbSql.AppendLine(" , ISNULL(Movimento_Extra.N_Doc_Cliente, '') AS N_Doc_Cliente ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Data_Doc_Cliente, '01/01/1900') AS Data_Doc_Cliente ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.N_Nota_Fattura, '') AS N_Nota_Fattura ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Data_Nota_Fattura, '01/01/1900') AS Data_Nota_Fattura ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.N_Nota_DDT, '') AS N_Nota_DDT ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.N_Nota_Riga_DDT, '') AS N_Nota_Riga_DDT ")
            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Data_Nota_DDT, '01/01/1900') AS Data_Nota_DDT ")

            stbSql.AppendLine(" , ISNULL(Movimento_Extra.Titolo_Alcol, 0) AS Titolo_Alcol ")

            stbSql.AppendLine("  ")
            stbSql.AppendLine("  ")
            'stbSql.AppendLine(" ,UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim,   ")
            ''Dettagli_Extra.Codice_Prodotto, Dettagli_Extra.Colore, Dettagli_Extra.Zona_Viticola, Dettagli_Extra.Manipolazioni, 
            'stbSql.AppendLine(" Materie_Prime.codice_prodotto, Materie_Prime.colore, Materie_Prime.codice_nc, Materie_Prime.manipolazioni ")
            'stbSql.AppendLine("  ")

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" FROM Agenda  ")
            'MODIFICA DEL 03/06/2014: aggiunta clausola di join, altrimenti per le aziende non superuser (ma contatti del superuser), venivano sdoppiati gli articoli
            stbSql.AppendLine(" INNER JOIN Contatti Contatti_Impresa ON Agenda.Piva = Contatti_Impresa.Cod_Contatto  ")
            'modifica del 30/06/14: verifico che la piva padre del contatto sia quella del superuser
            '(fabrizio non risuciva a stampare le fatture di net-agree)
            'stbSql.AppendLine(" AND Agenda.Piva = Contatti_Impresa.Piva ")
            'stbSql.AppendLine(" AND Contatti_Impresa.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            If PivaProprietaria <> "" Then
                stbSql.AppendLine(" AND Contatti_Impresa.Piva = '" & Agro_SQL_SaveText(PivaProprietaria) & "'  ")
            End If

            stbSql.AppendLine(" INNER JOIN Movimenti Mov_Contabile ON Agenda.PIVA = Mov_Contabile.PIVA AND Agenda.Sa_Cod = Mov_Contabile.Sa_Cod AND Agenda.Id_Agenda = Mov_Contabile.Id_Agenda  ")
            stbSql.AppendLine("  ")
            stbSql.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Cliente ON Mov_Contabile.Cod_RisUm = Risorse_Umane_Cliente.Cod_RisUm  ")
            stbSql.AppendLine(" INNER JOIN Contatti Contatti_Cliente ON Risorse_Umane_Cliente.Piva = Contatti_Cliente.Piva AND Risorse_Umane_Cliente.Cod_Contatto = Contatti_Cliente.Cod_Contatto  ")
            'stbSql.AppendLine(" INNER JOIN Indirizzi Indirizzi_Cliente ON  Mov_Contabile.Cod_IndirizzoRisUm = Indirizzi_Cliente.cod_indirizzo ")
            'stbSql.AppendLine(" INNER JOIN ISTAT Istat_Cliente ON Indirizzi_Cliente.pro_cod_istat = Istat_Cliente.PROV AND Indirizzi_Cliente.com_cod_istat = Istat_Cliente.COM ")
            stbSql.AppendLine("  ")
            'stbSql.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_DestDiv ON Mov_Contabile.Cod_Destinazione = Risorse_Umane_DestDiv.Cod_RisUm  ")
            'stbSql.AppendLine(" LEFT OUTER JOIN Contatti Contatti_DestDiv ON Risorse_Umane_DestDiv.Piva = Contatti_DestDiv.Piva AND Risorse_Umane_DestDiv.Cod_Contatto = Contatti_DestDiv.Cod_Contatto  ")
            'stbSql.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_DestDiv ON  Mov_Contabile.Cod_IndirizzoDestinazione= Indirizzi_DestDiv.cod_indirizzo ")
            'stbSql.AppendLine(" LEFT OUTER JOIN ISTAT Istat_DestDiv ON Indirizzi_DestDiv.pro_cod_istat = Istat_DestDiv.PROV AND Indirizzi_DestDiv.com_cod_istat = Istat_DestDiv.COM ")
            'stbSql.AppendLine("  ")
            'stbSql.AppendLine(" LEFT OUTER JOIN Risorse_Umane Risorse_Umane_Vettore ON Mov_Contabile.Cod_Vettore = Risorse_Umane_Vettore.Cod_RisUm  ")
            'stbSql.AppendLine(" LEFT OUTER JOIN Contatti Contatti_Vettore ON Risorse_Umane_Vettore.Piva = Contatti_Vettore.Piva AND Risorse_Umane_Vettore.Cod_Contatto = Contatti_Vettore.Cod_Contatto  ")
            'stbSql.AppendLine(" LEFT OUTER JOIN Indirizzi Indirizzi_Vettore ON  Mov_Contabile.Cod_IndirizzoVettore = Indirizzi_Vettore.cod_indirizzo ")
            'stbSql.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Vettore ON Indirizzi_Vettore.pro_cod_istat = Istat_Vettore.PROV AND Indirizzi_Vettore.com_cod_istat = Istat_Vettore.COM ")
            'stbSql.AppendLine("  ")
            'stbSql.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra ON Movimento_Extra.PIVA = Mov_Contabile.PIVA AND Movimento_Extra.Sa_Cod = Mov_Contabile.Sa_Cod AND Movimento_Extra.Id_Agenda = Mov_Contabile.Id_Agenda AND Movimento_Extra.Id_Mov = Mov_Contabile.Id_Mov ")
            'stbSql.AppendLine("  ")
            stbSql.AppendLine(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Sa_Cod = Mov_Magazzino.Sa_Cod AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  ")
            stbSql.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov  ")
            '02/05/2012: gli altri beni strumentali non hanno il magazzino
            ' stbSql.AppendLine(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Destinazioni.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Destinazioni.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det ")

            stbSql.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Mov_Dett_Magazzino.Cod_Iva   ")

            stbSql.AppendLine(" LEFT OUTER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra_0 ON Movimento_Extra_0.PIVA = Mov_Contabile.PIVA AND Movimento_Extra_0.Sa_Cod = Mov_Contabile.Sa_Cod AND Movimento_Extra_0.Id_Agenda = Mov_Contabile.Id_Agenda AND Movimento_Extra_0.Id_Mov = Mov_Contabile.Id_Mov AND Movimento_Extra_0.Id_Mov_Det=0 ")
            stbSql.AppendLine(" LEFT OUTER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra ON Movimento_Extra.PIVA = Mov_Dett_Magazzino.PIVA AND Movimento_Extra.Sa_Cod = Mov_Dett_Magazzino.Sa_Cod AND Movimento_Extra.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Movimento_Extra.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Movimento_Extra.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det ")
            stbSql.AppendLine("  ")

            'stbSql.AppendLine("  ")
            'stbSql.AppendLine(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod AND Materie_Prime.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod   ")
            'stbSql.AppendLine("  ")
            'stbSql.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Dettagli_Extra ON Dettagli_Extra.PIVA = Mov_Dett_Magazzino.PIVA AND Dettagli_Extra.Sa_Cod = Mov_Dett_Magazzino.Sa_Cod AND Dettagli_Extra.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Dettagli_Extra.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Dettagli_Extra.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det ")
            'stbSql.AppendLine("  ")
            'stbSql.AppendLine(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod ")
            'stbSql.AppendLine("  ")


            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" WHERE Mov_Contabile.Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")

            'modifica del 24/08/2015: per il conferimento uva gestita la lettura del dettaglio con il totale delle uve
            Select Case Cod_Report

                Case enum_CodificaStampe.ConferimentoUva_DDTRicevuto,
                    enum_CodificaStampe.ConferimentoUva_DistintaCarico,
                    enum_CodificaStampe.ConferimentoUva_AutoDDT

                    stbSql.AppendLine(" AND    Mov_Magazzino.Cau_Mov = '" & CAU_REGISTRAZIONI_TERZIARIA & "'  ")

                Case Else

                    'i documenti in uscita hanno il cau_mov dello scarico, ma i resi hanno quello del carico 
                    'le bolle di conferimento hanno il loro cau_mov
                    stbSql.AppendLine(" AND    Mov_Magazzino.Cau_Mov IN ( '" &
                                            CAU_SCARICO & "', '" &
                                            CAU_CARICO & "', '" &
                                            CAU_ABBUONI & "', '" &
                                            CAU_CONFERIMENTO & "', '" &
                                            CAU_CONFERIMENTO_DIVERSI & "', '" &
                                            CAU_ACCETTAZIONE_BENI & "', '" &
                                            CAU_ACCETTAZIONE_BENI_DA_DIVERSI &
                                            "'  ) ")

            End Select

            stbSql.AppendLine(" AND NOT EXISTS ( ")
            stbSql.AppendLine("                 SELECT * ")
            stbSql.AppendLine("                 FROM Mov_Destinazioni  ")
            stbSql.AppendLine("                 WHERE Mov_Destinazioni.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Destinazioni.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Destinazioni.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det ")
            stbSql.AppendLine("                 )")

            If Lav_Cod <> 0 Then
                stbSql.AppendLine(" AND Agenda.Lav_Cod = " & CStr(Lav_Cod) & " ")
            End If

            If PivaImpresa <> "" Then
                stbSql.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(PivaImpresa) & "' ")
            End If

            If Id_Agenda <> 0 Then
                stbSql.AppendLine(" AND Agenda.Id_Agenda = " & CStr(Id_Agenda) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stbSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            stbSql.AppendLine(" ) ")

            '------------------------------------------------------------------------
            '---- FINE UNION ---------
            '------------------------------------------------------------------------
            stbSql.AppendLine(" ) DOCUMENTI ")

            If xOrderBy <> "" Then
                stbSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stbSql.AppendLine(" ORDER BY Ordine_Det ASC, Id_Mov_Det ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Public Function Intestazione_Documento(ByVal Piva As String,
                                           ByVal Flag_Fabbricati As Boolean,
                                           ByVal Flag_DOCO As Boolean,
                                           ByVal Flag_DAA As Boolean,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocContab.Intestazione_Documento"

        Dim messaggioErrore As String = ""
        Dim stbQuery As New StringBuilder
        Dim dt As DataTable

        Try

            stbQuery.Length = 0

            stbQuery.AppendLine(" SELECT    Imprese.PIVA, Imprese.rag_soc, ImpresexIndirizzi.cod_indirizzo AS cod_indirizzo_impresa,   ")
            stbQuery.AppendLine("           ImpresexIndirizzi.Tipo_Indirizzo AS Tipo_Indirizzo_impresa, Indirizzo_Impresa.ind_des AS ind_des_impresa,  ")
            stbQuery.AppendLine("           Indirizzo_Impresa.frz_des AS frz_des_impresa, Indirizzo_Impresa.CAP AS cap_impresa, Indirizzo_Impresa.stato AS stato_impresa,  ")
            stbQuery.AppendLine("           Indirizzo_Impresa.note AS note_impresa, Indirizzo_Impresa.pro_cod_istat AS pro_cod_istat_impresa,  ")
            stbQuery.AppendLine("           Indirizzo_Impresa.com_cod_istat AS com_cod_istat_impresa, ISTAT_Impresa.LOCALITA AS localita_impresa, ISTAT_Impresa.COMUNI_PROV  AS comuni_prov_impresa,  ")
            stbQuery.AppendLine("           Lista_Province_Impresa.REG AS Reg_Impresa, Lista_Province_Impresa.COM AS com_istat_provincia_impresa, Lista_Province_Impresa.PROVINCIA AS provincia_impresa, ")
            stbQuery.AppendLine("           Contatto_Impresa.Cod_Contatto, Contatto_Impresa.Codice_Fiscale, Imprese_Codici.id_cod AS id_cod_impresa, Imprese_Codici.val_cod AS val_cod_impresa,  ")
            stbQuery.AppendLine("           Centri_Aziendali.sa_cod AS sa_cod_SedeLegale, Centri_Aziendali.sa_nome AS sa_nome_SedeLegale, CentrixRubrica.cod_rubrica, Rubrica.numero,  ")
            stbQuery.AppendLine("           Rubrica.descr, Centri_Aziendali_SedeLegale.id_cod AS id_cod_SedeLegale, Centri_Aziendali_SedeLegale.val_cod AS val_Cod_SedeLegale  ")
            If Flag_Fabbricati Then
                stbQuery.AppendLine("           , Fabbricati.SA_COD AS Sa_Cod_Fabbricato, Fabbricati.Fabbricato_Cod, Fabbricati.Fabbricato_Des, Fabbricati.Indirizzo_Cod,  ")
                stbQuery.AppendLine("           Indirizzo_Fabbricato.ind_des AS ind_des_fabbricato, Indirizzo_Fabbricato.frz_des AS frz_des_fabbricato, Indirizzo_Fabbricato.CAP AS cap_fabbricato,  ")
                stbQuery.AppendLine("           Indirizzo_Fabbricato.stato AS stato_fabbricato, Indirizzo_Fabbricato.note AS note_fabbricato,  ")
                stbQuery.AppendLine("           Indirizzo_Fabbricato.pro_cod_istat AS pro_cod_istat_fabbricato, Indirizzo_Fabbricato.com_cod_istat AS com_cod_istat_fabbricato,  ")
                stbQuery.AppendLine("           ISTAT_Fabbricato.LOCALITA AS localita_fabbricato, ISTAT_Fabbricato.COMUNI_PROV AS comuni_prov_fabbricato ")
            End If
            If Flag_DOCO Then
                stbQuery.AppendLine(" , Centri_Aziendali_Codici.val_cod AS CodIndirizzo_MinisteroPoliticheAgricole,  ")
                stbQuery.AppendLine("           Indirizzo_Ministero.ind_des AS ind_des_ministero,  ")
                stbQuery.AppendLine("           Indirizzo_Ministero.frz_des AS frz_des_ministero, Indirizzo_Ministero.CAP AS cap_ministero, Indirizzo_Ministero.stato AS stato_ministero,  ")
                stbQuery.AppendLine("           Indirizzo_Ministero.note AS note_ministero, Indirizzo_Ministero.pro_cod_istat AS pro_cod_istat_ministero,  ")
                stbQuery.AppendLine("           Indirizzo_Ministero.com_cod_istat AS com_cod_istat_ministero, ISTAT_Ministero.LOCALITA AS localita_ministero, ISTAT_Ministero.COMUNI_PROV  AS comuni_prov_ministero ")
            End If
            If Flag_DAA Then
                stbQuery.AppendLine(" , Centri_Aziendali_Codici.val_cod AS CodIndirizzo_Autorita,  ")
                stbQuery.AppendLine("           Indirizzo_Autorita.ind_des AS ind_des_Autorita,  ")
                stbQuery.AppendLine("           Indirizzo_Autorita.frz_des AS frz_des_Autorita, Indirizzo_Autorita.CAP AS cap_Autorita, Indirizzo_Autorita.stato AS stato_Autorita,  ")
                stbQuery.AppendLine("           Indirizzo_Autorita.pro_cod_istat AS pro_cod_istat_Autorita,  ")
                stbQuery.AppendLine("           Indirizzo_Autorita.com_cod_istat AS com_cod_istat_Autorita, ISTAT_Autorita.LOCALITA AS localita_Autorita, ISTAT_Autorita.COMUNI_PROV  AS comuni_prov_Autorita, ")
                stbQuery.AppendLine("           Indirizzo_Autorita.note AS RagSoc_Autorita,  ")

                stbQuery.AppendLine("           Contatti_Codici.val_cod AS Codice_Accisa_Speditore  ")
            End If
            stbQuery.AppendLine("  ")
            stbQuery.AppendLine(" FROM Imprese  ")
            stbQuery.AppendLine(" INNER JOIN ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA ")
            stbQuery.AppendLine(" INNER JOIN Indirizzi Indirizzo_Impresa ON ImpresexIndirizzi.cod_indirizzo = Indirizzo_Impresa.cod_indirizzo ")
            stbQuery.AppendLine(" INNER JOIN ISTAT ISTAT_Impresa ON Indirizzo_Impresa.pro_cod_istat = ISTAT_Impresa.PROV AND Indirizzo_Impresa.com_cod_istat = ISTAT_Impresa.COM   ")
            stbQuery.AppendLine(" INNER JOIN Lista_Province AS Lista_Province_Impresa ON Lista_Province_Impresa.SIGLA = ISTAT_Impresa.COMUNI_PROV ")
            stbQuery.AppendLine(" INNER JOIN Contatti Contatto_Impresa ON Imprese.PIVA = Contatto_Impresa.Cod_Contatto ")
            stbQuery.AppendLine(" INNER JOIN Imprese_Codici ON Imprese.PIVA = Imprese_Codici.PIVA  ")
            stbQuery.AppendLine(" INNER JOIN Centri_Aziendali ON Imprese.PIVA = Centri_Aziendali.PIVA ")
            stbQuery.AppendLine(" INNER JOIN CentrixRubrica ON CentrixRubrica.PIVA = Centri_Aziendali.PIVA AND CentrixRubrica.sa_cod = Centri_Aziendali.sa_cod  ")
            stbQuery.AppendLine(" INNER JOIN Rubrica ON  Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica  ")
            stbQuery.AppendLine(" INNER JOIN Centri_Aziendali_Codici Centri_Aziendali_SedeLegale ON Centri_Aziendali.PIVA = Centri_Aziendali_SedeLegale.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_SedeLegale.sa_cod  ")
            stbQuery.AppendLine("  ")
            If Flag_Fabbricati Then
                stbQuery.AppendLine(" INNER JOIN Fabbricati ON Imprese.PIVA = Fabbricati.PIVA ")
                stbQuery.AppendLine(" INNER JOIN Indirizzi Indirizzo_Fabbricato ON Fabbricati.Indirizzo_Cod = Indirizzo_Fabbricato.cod_indirizzo ")
                stbQuery.AppendLine(" INNER JOIN ISTAT ISTAT_Fabbricato ON Indirizzo_Fabbricato.pro_cod_istat = ISTAT_Fabbricato.PROV AND Indirizzo_Fabbricato.com_cod_istat = ISTAT_Fabbricato.COM ")
            End If
            If Flag_DOCO Then
                stbQuery.AppendLine(" INNER JOIN Centri_Aziendali_Codici ON Centri_Aziendali.PIVA = Centri_Aziendali_Codici.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod  ")
                stbQuery.AppendLine(" INNER JOIN Indirizzi Indirizzo_Ministero ON Centri_Aziendali_Codici.val_cod = Indirizzo_Ministero.cod_indirizzo ")
                stbQuery.AppendLine(" INNER JOIN ISTAT ISTAT_Ministero ON Indirizzo_Ministero.pro_cod_istat = ISTAT_Ministero.PROV AND Indirizzo_Ministero.com_cod_istat = ISTAT_Ministero.COM   ")
            End If
            If Flag_DAA Then
                stbQuery.AppendLine(" INNER JOIN Centri_Aziendali_Codici ON Centri_Aziendali.PIVA = Centri_Aziendali_Codici.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod  ")
                stbQuery.AppendLine(" INNER JOIN Indirizzi Indirizzo_Autorita ON Centri_Aziendali_Codici.val_cod = Indirizzo_Autorita.cod_indirizzo ")
                stbQuery.AppendLine(" INNER JOIN ISTAT ISTAT_Autorita ON Indirizzo_Autorita.pro_cod_istat = ISTAT_Autorita.PROV AND Indirizzo_Autorita.com_cod_istat = ISTAT_Autorita.COM   ")

                stbQuery.AppendLine(" INNER JOIN Contatti_Codici ON Contatto_Impresa.PIVA = Contatti_Codici.PIVA AND  Contatto_Impresa.Cod_contatto = Contatti_Codici.Cod_Contatto  ")
            End If
            stbQuery.AppendLine("  ")
            stbQuery.AppendLine(" WHERE (Imprese.PIVA = '" & Agro_SQL_SaveText(Piva) & "')  ")
            stbQuery.AppendLine(" AND   (Imprese_Codici.id_cod IN (" & CStr(enum_CodiciAnagrafe.CodiceISO) &
                                                            ", " & CStr(enum_CodiciAnagrafe.CodiceREA) &
                                                            ", " & CStr(enum_CodiciAnagrafe.CapitaleSociale) &
                                                            ", " & CStr(enum_CodiciAnagrafe.NumIscrAlboSocCoop) &
                                                            ", " & CStr(enum_CodiciAnagrafe.NumRegImprese) & "))  ")

            stbQuery.AppendLine(" AND   Centri_Aziendali_SedeLegale.id_cod = 101 ")

            If Flag_DOCO Then
                stbQuery.AppendLine(" AND   Centri_Aziendali_Codici.id_cod = 1120 ")
            End If

            If Flag_DAA Then
                stbQuery.AppendLine(" AND   Centri_Aziendali_Codici.id_cod = 1121 ")
                stbQuery.AppendLine(" AND   Contatti_Codici.id_cod = 4003 ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stbQuery.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stbQuery.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, nomeRoutine)
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
    ''' Intestazione versione 2: legge indirizzo sede operativa e i numeri della rubrica in base alla sede
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Intestazione_Documento_2(ByVal PivaImpresa As String,
                                             ByVal PivaProprietaria As String,
                                             ByVal Flag_Fabbricati As Boolean,
                                             ByVal Flag_DOCO As Boolean,
                                             ByVal Flag_DAA As Boolean,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocContab.Intestazione_Documento_2"

        Dim messaggioErrore As String = ""
        Dim stbQuery As New StringBuilder
        Dim dt As DataTable

        Try

            stbQuery.Length = 0

            stbQuery.AppendLine(" SELECT    Imprese.PIVA, Contatti.rag_soc, Contatti.Codice_Fiscale, ImpresexIndirizzi.cod_indirizzo AS cod_indirizzo_impresa,   ")
            stbQuery.AppendLine("           ImpresexIndirizzi.Tipo_Indirizzo AS Tipo_Indirizzo_impresa, Indirizzo_Impresa.ind_des AS ind_des_impresa,  ")
            stbQuery.AppendLine("           Indirizzo_Impresa.frz_des AS frz_des_impresa, Indirizzo_Impresa.CAP AS cap_impresa, Indirizzo_Impresa.stato AS stato_impresa,  ")
            stbQuery.AppendLine("           Indirizzo_Impresa.note AS note_impresa, Indirizzo_Impresa.pro_cod_istat AS pro_cod_istat_impresa,  ")
            stbQuery.AppendLine("           Indirizzo_Impresa.com_cod_istat AS com_cod_istat_impresa, ISTAT_Impresa.LOCALITA AS localita_impresa, ISTAT_Impresa.COMUNI_PROV  AS comuni_prov_impresa,  ")
            stbQuery.AppendLine("           Lista_Province_Impresa.REG AS Reg_Impresa, Lista_Province_Impresa.COM AS com_istat_provincia_impresa, Lista_Province_Impresa.PROVINCIA AS provincia_impresa, ")
            stbQuery.AppendLine("           Imprese_Codici.id_cod AS id_cod_impresa, Imprese_Codici.val_cod AS val_cod_impresa  ")
            'stbQuery.AppendLine("           Contatto_Impresa.Cod_Contatto, Contatto_Impresa.Codice_Fiscale, Imprese_Codici.id_cod AS id_cod_impresa, Imprese_Codici.val_cod AS val_cod_impresa  ")

            'stbQuery.AppendLine("           Centri_Aziendali.sa_cod AS sa_cod_SedeLegale, Centri_Aziendali.sa_nome AS sa_nome_SedeLegale, CentrixRubrica.cod_rubrica, Rubrica.numero,  ")
            'stbQuery.AppendLine("           Rubrica.descr, Centri_Aziendali_SedeLegale.id_cod AS id_cod_SedeLegale, Centri_Aziendali_SedeLegale.val_cod AS val_Cod_SedeLegale  ")

            stbQuery.AppendLine("       , ISNULL( ( SELECT TOP 1 Rubrica.numero ")
            stbQuery.AppendLine("               FROM Centri_Aziendali ")
            stbQuery.AppendLine("               INNER JOIN Centri_Aziendali_Codici ON Centri_Aziendali.PIVA = Centri_Aziendali_Codici.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN CentrixRubrica ON CentrixRubrica.PIVA = Centri_Aziendali.PIVA AND CentrixRubrica.sa_cod = Centri_Aziendali.sa_cod   ")
            stbQuery.AppendLine("               INNER JOIN Rubrica ON  Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica  ")
            stbQuery.AppendLine("               WHERE Imprese.PIVA = Centri_Aziendali.PIVA   ")
            stbQuery.AppendLine("               AND   Centri_Aziendali_Codici.id_cod = " & CStr(enum_TipoCentro.Sede_Legale) & "  ")
            stbQuery.AppendLine("               AND Rubrica.descr LIKE '%tel%'  ")
            stbQuery.AppendLine("               ), '' ) AS Tel_Sede_Legale  ")

            stbQuery.AppendLine("       , ISNULL( ( SELECT TOP 1 Rubrica.numero ")
            stbQuery.AppendLine("               FROM Centri_Aziendali ")
            stbQuery.AppendLine("               INNER JOIN Centri_Aziendali_Codici ON Centri_Aziendali.PIVA = Centri_Aziendali_Codici.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN CentrixRubrica ON CentrixRubrica.PIVA = Centri_Aziendali.PIVA AND CentrixRubrica.sa_cod = Centri_Aziendali.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN Rubrica ON  Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica  ")
            stbQuery.AppendLine("               WHERE Imprese.PIVA = Centri_Aziendali.PIVA   ")
            stbQuery.AppendLine("               AND   Centri_Aziendali_Codici.id_cod = " & CStr(enum_TipoCentro.Sede_Legale) & "  ")
            stbQuery.AppendLine("               AND Rubrica.descr LIKE '%Fax%'  ")
            stbQuery.AppendLine("               ), '' ) AS Fax_Sede_Legale  ")

            stbQuery.AppendLine("       , ISNULL( ( SELECT TOP 1 Rubrica.numero ")
            stbQuery.AppendLine("               FROM Centri_Aziendali ")
            stbQuery.AppendLine("               INNER JOIN Centri_Aziendali_Codici ON Centri_Aziendali.PIVA = Centri_Aziendali_Codici.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN CentrixRubrica ON CentrixRubrica.PIVA = Centri_Aziendali.PIVA AND CentrixRubrica.sa_cod = Centri_Aziendali.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN Rubrica ON  Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica  ")
            stbQuery.AppendLine("               WHERE Imprese.PIVA = Centri_Aziendali.PIVA   ")
            stbQuery.AppendLine("               AND   Centri_Aziendali_Codici.id_cod = " & CStr(enum_TipoCentro.Sede_Legale) & "  ")
            stbQuery.AppendLine("               AND Rubrica.descr LIKE '%Cell%'  ")
            stbQuery.AppendLine("               ), '' ) AS Cell_Sede_Legale  ")

            stbQuery.AppendLine("       , ISNULL( ( SELECT TOP 1 Rubrica.numero ")
            stbQuery.AppendLine("               FROM Centri_Aziendali ")
            stbQuery.AppendLine("               INNER JOIN Centri_Aziendali_Codici ON Centri_Aziendali.PIVA = Centri_Aziendali_Codici.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN CentrixRubrica ON CentrixRubrica.PIVA = Centri_Aziendali.PIVA AND CentrixRubrica.sa_cod = Centri_Aziendali.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN Rubrica ON  Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica  ")
            stbQuery.AppendLine("               WHERE Imprese.PIVA = Centri_Aziendali.PIVA   ")
            stbQuery.AppendLine("               AND   Centri_Aziendali_Codici.id_cod = " & CStr(enum_TipoCentro.Sede_Aziendale) & "  ")
            stbQuery.AppendLine("               AND Rubrica.descr LIKE '%tel%'  ")
            stbQuery.AppendLine("               ), '' ) AS Tel_Sede_Operativa  ")

            stbQuery.AppendLine("       , ISNULL( ( SELECT TOP 1 Rubrica.numero ")
            stbQuery.AppendLine("               FROM Centri_Aziendali ")
            stbQuery.AppendLine("               INNER JOIN Centri_Aziendali_Codici ON Centri_Aziendali.PIVA = Centri_Aziendali_Codici.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN CentrixRubrica ON CentrixRubrica.PIVA = Centri_Aziendali.PIVA AND CentrixRubrica.sa_cod = Centri_Aziendali.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN Rubrica ON  Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica  ")
            stbQuery.AppendLine("               WHERE Imprese.PIVA = Centri_Aziendali.PIVA   ")
            stbQuery.AppendLine("               AND   Centri_Aziendali_Codici.id_cod = " & CStr(enum_TipoCentro.Sede_Aziendale) & "  ")
            stbQuery.AppendLine("               AND Rubrica.descr LIKE '%Fax%'  ")
            stbQuery.AppendLine("               ), '' ) AS Fax_Sede_Operativa  ")

            stbQuery.AppendLine("       , ISNULL( ( SELECT TOP 1 Rubrica.numero ")
            stbQuery.AppendLine("               FROM Centri_Aziendali ")
            stbQuery.AppendLine("               INNER JOIN Centri_Aziendali_Codici ON Centri_Aziendali.PIVA = Centri_Aziendali_Codici.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN CentrixRubrica ON CentrixRubrica.PIVA = Centri_Aziendali.PIVA AND CentrixRubrica.sa_cod = Centri_Aziendali.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN Rubrica ON  Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica  ")
            stbQuery.AppendLine("               WHERE Imprese.PIVA = Centri_Aziendali.PIVA   ")
            stbQuery.AppendLine("               AND   Centri_Aziendali_Codici.id_cod = " & CStr(enum_TipoCentro.Sede_Aziendale) & "  ")
            stbQuery.AppendLine("               AND Rubrica.descr LIKE '%Cell%'  ")
            stbQuery.AppendLine("               ), '' ) AS Cell_Sede_Operativa  ")

            stbQuery.AppendLine("       , ISNULL( ( SELECT TOP 1 ind_des + ' '  + frz_des  ")
            stbQuery.AppendLine("               FROM Centri_Aziendali ")
            stbQuery.AppendLine("               INNER JOIN Centri_Aziendali_Codici ON Centri_Aziendali.PIVA = Centri_Aziendali_Codici.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN CentrixIndirizzi ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN Indirizzi ON  Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")
            stbQuery.AppendLine("               INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
            stbQuery.AppendLine("               WHERE Imprese.PIVA = Centri_Aziendali.PIVA   ")
            stbQuery.AppendLine("               AND   Centri_Aziendali_Codici.id_cod = " & CStr(enum_TipoCentro.Sede_Aziendale) & "  ")
            stbQuery.AppendLine("               AND   CentrixIndirizzi.Tipo_Indirizzo = 1 ")
            stbQuery.AppendLine("               ), '' ) AS IndirizzoRiga1_Sede_Operativa  ")

            stbQuery.AppendLine("       , ISNULL( ( SELECT TOP 1 Indirizzi.CAP + ' ' + LOCALITA + ' (' + COMUNI_PROV      + ')' ")
            stbQuery.AppendLine("               FROM Centri_Aziendali ")
            stbQuery.AppendLine("               INNER JOIN Centri_Aziendali_Codici ON Centri_Aziendali.PIVA = Centri_Aziendali_Codici.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN CentrixIndirizzi ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN Indirizzi ON  Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")
            stbQuery.AppendLine("               INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM ")
            stbQuery.AppendLine("               WHERE Imprese.PIVA = Centri_Aziendali.PIVA   ")
            stbQuery.AppendLine("               AND   Centri_Aziendali_Codici.id_cod = " & CStr(enum_TipoCentro.Sede_Aziendale) & "  ")
            stbQuery.AppendLine("               AND   CentrixIndirizzi.Tipo_Indirizzo = 1 ")
            stbQuery.AppendLine("               ), '' ) AS IndirizzoRiga2_Sede_Operativa  ")

            stbQuery.AppendLine("       , ISNULL( ( SELECT TOP 1 Rubrica.numero ")
            stbQuery.AppendLine("               FROM Centri_Aziendali ")
            stbQuery.AppendLine("               INNER JOIN CentrixRubrica ON CentrixRubrica.PIVA = Centri_Aziendali.PIVA AND CentrixRubrica.sa_cod = Centri_Aziendali.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN Rubrica ON  Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica  ")
            stbQuery.AppendLine("               WHERE Imprese.PIVA = Centri_Aziendali.PIVA   ")
            stbQuery.AppendLine("               AND Rubrica.descr LIKE '%Mail%'  ")
            stbQuery.AppendLine("               ), '' ) AS Mail  ")

            stbQuery.AppendLine("       , ISNULL( ( SELECT TOP 1 Rubrica.numero ")
            stbQuery.AppendLine("               FROM Centri_Aziendali ")
            stbQuery.AppendLine("               INNER JOIN CentrixRubrica ON CentrixRubrica.PIVA = Centri_Aziendali.PIVA AND CentrixRubrica.sa_cod = Centri_Aziendali.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN Rubrica ON  Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica  ")
            stbQuery.AppendLine("               WHERE Imprese.PIVA = Centri_Aziendali.PIVA   ")
            stbQuery.AppendLine("               AND Rubrica.descr LIKE '%PEC%'  ")
            stbQuery.AppendLine("               ), '' ) AS PEC  ")

            stbQuery.AppendLine("       , ISNULL( ( SELECT TOP 1 Rubrica.numero ")
            stbQuery.AppendLine("               FROM Centri_Aziendali ")
            stbQuery.AppendLine("               INNER JOIN CentrixRubrica ON CentrixRubrica.PIVA = Centri_Aziendali.PIVA AND CentrixRubrica.sa_cod = Centri_Aziendali.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN Rubrica ON  Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica  ")
            stbQuery.AppendLine("               WHERE Imprese.PIVA = Centri_Aziendali.PIVA ")
            stbQuery.AppendLine("               AND Rubrica.descr LIKE '%Sito%'  ")
            stbQuery.AppendLine("               ), '' ) AS SitoWeb  ")

            stbQuery.AppendLine("       , ISNULL( ( SELECT TOP 1 Rubrica.numero ")
            stbQuery.AppendLine("               FROM Centri_Aziendali ")
            stbQuery.AppendLine("               INNER JOIN CentrixRubrica ON CentrixRubrica.PIVA = Centri_Aziendali.PIVA AND CentrixRubrica.sa_cod = Centri_Aziendali.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN Rubrica ON  Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica  ")
            stbQuery.AppendLine("               WHERE Imprese.PIVA = Centri_Aziendali.PIVA   ")
            stbQuery.AppendLine("               AND Rubrica.descr LIKE '%Social%'  ")
            stbQuery.AppendLine("               ), '' ) AS Social  ")

            stbQuery.AppendLine("       , ISNULL( ( SELECT TOP 1 Rubrica.numero ")
            stbQuery.AppendLine("               FROM Centri_Aziendali ")
            stbQuery.AppendLine("               INNER JOIN CentrixRubrica ON CentrixRubrica.PIVA = Centri_Aziendali.PIVA AND CentrixRubrica.sa_cod = Centri_Aziendali.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN Rubrica ON  Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica  ")
            stbQuery.AppendLine("               WHERE Imprese.PIVA = Centri_Aziendali.PIVA   ")
            stbQuery.AppendLine("               AND Rubrica.descr LIKE '%jolly1%'  ")
            stbQuery.AppendLine("               ), '' ) AS jolly1  ")

            stbQuery.AppendLine("       , ISNULL( ( SELECT TOP 1 Rubrica.numero ")
            stbQuery.AppendLine("               FROM Centri_Aziendali ")
            stbQuery.AppendLine("               INNER JOIN CentrixRubrica ON CentrixRubrica.PIVA = Centri_Aziendali.PIVA AND CentrixRubrica.sa_cod = Centri_Aziendali.sa_cod ")
            stbQuery.AppendLine("               INNER JOIN Rubrica ON  Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica  ")
            stbQuery.AppendLine("               WHERE Imprese.PIVA = Centri_Aziendali.PIVA   ")
            stbQuery.AppendLine("               AND Rubrica.descr LIKE '%jolly2%'  ")
            stbQuery.AppendLine("               ), '' ) AS jolly2  ")

            stbQuery.AppendLine("       , ISNULL( ( SELECT TOP 1 val_cod ")
            stbQuery.AppendLine("               FROM Centri_Aziendali ")
            stbQuery.AppendLine("               INNER JOIN Centri_Aziendali_Codici ON Centri_Aziendali.PIVA = Centri_Aziendali_Codici.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod ")
            stbQuery.AppendLine("               WHERE Imprese.PIVA = Centri_Aziendali.PIVA   ")
            stbQuery.AppendLine("               AND   Centri_Aziendali_Codici.id_cod = " & CStr(enum_CodiciAnagrafe.CodiceCentro_Attuale) & "  ")
            stbQuery.AppendLine("               ), '' ) AS CodiceOperatoreBio  ")

            stbQuery.AppendLine("                 ")
            stbQuery.AppendLine("                 ")


            If Flag_Fabbricati Then
                stbQuery.AppendLine("           , Fabbricati.SA_COD AS Sa_Cod_Fabbricato, Fabbricati.Fabbricato_Cod, Fabbricati.Fabbricato_Des, Fabbricati.Indirizzo_Cod, ")
                stbQuery.AppendLine("           Indirizzo_Fabbricato.ind_des AS ind_des_fabbricato, Indirizzo_Fabbricato.frz_des AS frz_des_fabbricato, Indirizzo_Fabbricato.CAP AS cap_fabbricato, ")
                stbQuery.AppendLine("           Indirizzo_Fabbricato.stato AS stato_fabbricato, Indirizzo_Fabbricato.note AS note_fabbricato,  ")
                stbQuery.AppendLine("           Indirizzo_Fabbricato.pro_cod_istat AS pro_cod_istat_fabbricato, Indirizzo_Fabbricato.com_cod_istat AS com_cod_istat_fabbricato, ")
                stbQuery.AppendLine("           ISTAT_Fabbricato.LOCALITA AS localita_fabbricato, ISTAT_Fabbricato.COMUNI_PROV AS comuni_prov_fabbricato ")
            End If
            If Flag_DOCO Then
                stbQuery.AppendLine(" , Centri_Aziendali_Codici.val_cod AS CodIndirizzo_MinisteroPoliticheAgricole,  ")
                stbQuery.AppendLine("           Indirizzo_Ministero.ind_des AS ind_des_ministero,  ")
                stbQuery.AppendLine("           Indirizzo_Ministero.frz_des AS frz_des_ministero, Indirizzo_Ministero.CAP AS cap_ministero, Indirizzo_Ministero.stato AS stato_ministero, ")
                stbQuery.AppendLine("           Indirizzo_Ministero.note AS note_ministero, Indirizzo_Ministero.pro_cod_istat AS pro_cod_istat_ministero,  ")
                stbQuery.AppendLine("           Indirizzo_Ministero.com_cod_istat AS com_cod_istat_ministero, ISTAT_Ministero.LOCALITA AS localita_ministero, ISTAT_Ministero.COMUNI_PROV  AS comuni_prov_ministero ")
            End If
            If Flag_DAA Then
                stbQuery.AppendLine(" , Centri_Aziendali_Codici.val_cod AS CodIndirizzo_Autorita,  ")
                stbQuery.AppendLine("           Indirizzo_Autorita.ind_des AS ind_des_Autorita,  ")
                stbQuery.AppendLine("           Indirizzo_Autorita.frz_des AS frz_des_Autorita, Indirizzo_Autorita.CAP AS cap_Autorita, Indirizzo_Autorita.stato AS stato_Autorita,  ")
                stbQuery.AppendLine("           Indirizzo_Autorita.pro_cod_istat AS pro_cod_istat_Autorita,  ")
                stbQuery.AppendLine("           Indirizzo_Autorita.com_cod_istat AS com_cod_istat_Autorita, ISTAT_Autorita.LOCALITA AS localita_Autorita, ISTAT_Autorita.COMUNI_PROV  AS comuni_prov_Autorita, ")
                stbQuery.AppendLine("           Indirizzo_Autorita.note AS RagSoc_Autorita,  ")

                stbQuery.AppendLine("           Contatti_Codici.val_cod AS Codice_Accisa_Speditore  ")
            End If
            stbQuery.AppendLine("  ")
            stbQuery.AppendLine(" FROM Imprese  ")
            stbQuery.AppendLine(" INNER JOIN Contatti ON Imprese.Piva = Contatti.Cod_Contatto  ")
            If PivaProprietaria <> "" Then
                stbQuery.AppendLine(" AND Contatti.Piva = '" & Agro_SQL_SaveText(PivaProprietaria) & "'  ")
            End If

            stbQuery.AppendLine(" INNER JOIN ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA ")
            stbQuery.AppendLine(" INNER JOIN Indirizzi Indirizzo_Impresa ON ImpresexIndirizzi.cod_indirizzo = Indirizzo_Impresa.cod_indirizzo ")
            stbQuery.AppendLine(" INNER JOIN ISTAT ISTAT_Impresa ON Indirizzo_Impresa.pro_cod_istat = ISTAT_Impresa.PROV AND Indirizzo_Impresa.com_cod_istat = ISTAT_Impresa.COM ")
            stbQuery.AppendLine(" INNER JOIN Lista_Province AS Lista_Province_Impresa ON Lista_Province_Impresa.SIGLA = ISTAT_Impresa.COMUNI_PROV ")
            'stbQuery.AppendLine(" INNER JOIN Contatti Contatto_Impresa ON Imprese.PIVA = Contatto_Impresa.Cod_Contatto ")
            stbQuery.AppendLine(" INNER JOIN Imprese_Codici ON Imprese.PIVA = Imprese_Codici.PIVA  ")
            'stbQuery.AppendLine(" INNER JOIN Centri_Aziendali ON Imprese.PIVA = Centri_Aziendali.PIVA ")
            'stbQuery.AppendLine(" INNER JOIN CentrixRubrica ON CentrixRubrica.PIVA = Centri_Aziendali.PIVA AND CentrixRubrica.sa_cod = Centri_Aziendali.sa_cod  ")
            'stbQuery.AppendLine(" INNER JOIN Rubrica ON  Rubrica.cod_rubrica = CentrixRubrica.cod_rubrica  ")
            'stbQuery.AppendLine(" INNER JOIN Centri_Aziendali_Codici Centri_Aziendali_SedeLegale ON Centri_Aziendali.PIVA = Centri_Aziendali_SedeLegale.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_SedeLegale.sa_cod  ")
            stbQuery.AppendLine("  ")
            If Flag_Fabbricati Then
                stbQuery.AppendLine(" INNER JOIN Fabbricati ON Imprese.PIVA = Fabbricati.PIVA ")
                stbQuery.AppendLine(" INNER JOIN Indirizzi Indirizzo_Fabbricato ON Fabbricati.Indirizzo_Cod = Indirizzo_Fabbricato.cod_indirizzo ")
                stbQuery.AppendLine(" INNER JOIN ISTAT ISTAT_Fabbricato ON Indirizzo_Fabbricato.pro_cod_istat = ISTAT_Fabbricato.PROV AND Indirizzo_Fabbricato.com_cod_istat = ISTAT_Fabbricato.COM ")
            End If
            If Flag_DOCO Then
                stbQuery.AppendLine(" INNER JOIN Centri_Aziendali_Codici ON Centri_Aziendali.PIVA = Centri_Aziendali_Codici.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod  ")
                stbQuery.AppendLine(" INNER JOIN Indirizzi Indirizzo_Ministero ON Centri_Aziendali_Codici.val_cod = Indirizzo_Ministero.cod_indirizzo ")
                stbQuery.AppendLine(" INNER JOIN ISTAT ISTAT_Ministero ON Indirizzo_Ministero.pro_cod_istat = ISTAT_Ministero.PROV AND Indirizzo_Ministero.com_cod_istat = ISTAT_Ministero.COM   ")
            End If
            If Flag_DAA Then
                stbQuery.AppendLine(" INNER JOIN Centri_Aziendali_Codici ON Centri_Aziendali.PIVA = Centri_Aziendali_Codici.PIVA AND  Centri_Aziendali.sa_cod = Centri_Aziendali_Codici.sa_cod ")
                stbQuery.AppendLine(" INNER JOIN Indirizzi Indirizzo_Autorita ON Centri_Aziendali_Codici.val_cod = Indirizzo_Autorita.cod_indirizzo ")
                stbQuery.AppendLine(" INNER JOIN ISTAT ISTAT_Autorita ON Indirizzo_Autorita.pro_cod_istat = ISTAT_Autorita.PROV AND Indirizzo_Autorita.com_cod_istat = ISTAT_Autorita.COM ")

                stbQuery.AppendLine(" INNER JOIN Contatti_Codici ON Contatti.PIVA = Contatti_Codici.PIVA AND  Contatti.Cod_contatto = Contatti_Codici.Cod_Contatto ")
                'stbQuery.AppendLine(" INNER JOIN Contatti_Codici ON Contatto_Impresa.PIVA = Contatti_Codici.PIVA AND  Contatto_Impresa.Cod_contatto = Contatti_Codici.Cod_Contatto ")
            End If
            stbQuery.AppendLine("  ")
            stbQuery.AppendLine(" WHERE (Imprese.PIVA = '" & Agro_SQL_SaveText(PivaImpresa) & "')  ")
            stbQuery.AppendLine(" AND   (Imprese_Codici.id_cod IN (" & CStr(enum_CodiciAnagrafe.CodiceISO) &
                                                            ", " & CStr(enum_CodiciAnagrafe.CodiceREA) &
                                                            ", " & CStr(enum_CodiciAnagrafe.CodiceCUAA) &
                                                            ", " & CStr(enum_CodiciAnagrafe.BNDOO_BancaDatiOperatoriOrtofrutticoli) &
                                                            ", " & CStr(enum_CodiciAnagrafe.Codice_GlobalGap) &
                                                            ", " & CStr(enum_CodiciAnagrafe.NumRegImprese) & "))  ")

            'stbQuery.AppendLine(" AND   Centri_Aziendali_SedeLegale.id_cod = 101 ")
            '                                                            ", " & CStr(enum_CodiciAnagrafe.NumIscrAlboSocCoop) &

            If Flag_DOCO Then
                stbQuery.AppendLine(" AND   Centri_Aziendali_Codici.id_cod = 1120 ")
            End If

            If Flag_DAA Then
                stbQuery.AppendLine(" AND   Centri_Aziendali_Codici.id_cod = 1121 ")
                stbQuery.AppendLine(" AND   Contatti_Codici.id_cod = 4003 ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stbQuery.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stbQuery.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Public Function DatiIntestazioneImpresa(ByVal piva As String,
                                            ByRef log As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocContab.DatiIntestazioneImpresa"

        Dim messaggioErrore As String = ""
        Dim stbQuery As New StringBuilder
        Dim dt As DataTable

        Try

            stbQuery.Length = 0

            stbQuery.AppendLine("  SELECT       Impresa.PIVA, Impresa.rag_soc, Impresa.TipoImpresaGerarchia, Impresa.Validita_Inizio, Impresa.Validita_Fine,        ")
            stbQuery.AppendLine("               ImpresexIndirizzo_Impresa.Tipo_Indirizzo AS tipo_indirizzo_impresa, Indirizzo_Impresa.ind_des AS ind_impresa, Indirizzo_Impresa.frz_des, ISTAT_Impresa.LOCALITA, ISTAT_Impresa.COMUNI_PROV, ISTAT_Impresa.CAP, ")
            stbQuery.AppendLine("               CASE WHEN ISNULL(Impresa.partitaIvaReale, '') = '' THEN Impresa.PIVA ELSE Impresa.partitaIvaReale END PivaReale, ")

            stbQuery.AppendLine("             ISNULL  ((  SELECT    TOP 1 val_cod  ")
            stbQuery.AppendLine("                         FROM    Imprese_Codici AS Imprese_Codici_1  ")
            stbQuery.AppendLine("                         WHERE   Imprese_Codici_1.piva = Impresa.piva AND Imprese_Codici_1.id_cod = 1086), ' ') AS codice_libro_soci, ")
            stbQuery.AppendLine("             ISNULL  ((  SELECT    TOP 1 val_cod  ")
            stbQuery.AppendLine("                         FROM    Imprese_Codici AS Imprese_Codici_2  ")
            stbQuery.AppendLine("                         WHERE   Imprese_Codici_2.piva = Impresa.piva AND Imprese_Codici_2.id_cod = 1087), '01/01/1900') AS data_libro_soci, ")
            stbQuery.AppendLine("             ISNULL  ((  SELECT    TOP 1 val_cod  ")
            stbQuery.AppendLine("                         FROM    Imprese_Codici AS Imprese_Codici_3  ")
            stbQuery.AppendLine("                         WHERE   Imprese_Codici_3.piva = Impresa.piva AND Imprese_Codici_3.id_cod = 1033), ' ') AS codice_socio, ")
            stbQuery.AppendLine("             ISNULL  ((  SELECT    TOP 1 val_cod  ")
            stbQuery.AppendLine("                         FROM    Imprese_Codici AS Imprese_Codici_4  ")
            stbQuery.AppendLine("                         WHERE   Imprese_Codici_4.piva = Impresa.piva AND Imprese_Codici_4.id_cod = 1010), ' ') AS codice_cuaa, ")
            stbQuery.AppendLine("             ISNULL  ((  SELECT    TOP 1 val_cod  ")
            stbQuery.AppendLine("                         FROM    Imprese_Codici AS Imprese_Codici_5  ")
            stbQuery.AppendLine("                         WHERE   Imprese_Codici_5.piva = Impresa.piva AND Imprese_Codici_5.id_cod = 4), ' ') AS codice_ausl, ")

            stbQuery.AppendLine("             ISNULL(Contatti.Codice_Fiscale, '') AS Codice_Fiscale, ISNULL(CooperativaPadre.PIVA, '') AS piva_padre,  ISNULL(CooperativaPadre.rag_soc, '') AS Cooperativa, ISNULL(ImpresexIndirizzo_Coop.Tipo_Indirizzo, 0) AS tipo_indirizzo_coop, ")
            stbQuery.AppendLine("              ISNULL(Indirizzo_Coop.ind_des, '') AS ind_coop, ISNULL(ISTAT_Coop.LOCALITA, '') AS com_coop, ISNULL(ISTAT_Coop.COMUNI_PROV, '')  AS prov_coop, ISNULL(ISTAT_Coop.CAP, '') AS cap_coop   ")

            stbQuery.AppendLine(" FROM    Imprese Impresa ")
            stbQuery.AppendLine("         LEFT OUTER JOIN ImpresexIndirizzi ImpresexIndirizzo_Impresa ON  Impresa.PIVA = ImpresexIndirizzo_Impresa.PIVA ")
            stbQuery.AppendLine("         LEFT OUTER JOIN Indirizzi Indirizzo_Impresa ON Indirizzo_Impresa.cod_indirizzo = ImpresexIndirizzo_Impresa.cod_indirizzo ")
            stbQuery.AppendLine("         LEFT OUTER JOIN ISTAT ISTAT_Impresa ON Indirizzo_Impresa.pro_cod_istat = ISTAT_Impresa.PROV AND Indirizzo_Impresa.com_cod_istat = ISTAT_Impresa.COM ")
            stbQuery.AppendLine("         LEFT OUTER JOIN Contatti ON Impresa.PIVA = Contatti.Cod_Contatto ")

            stbQuery.AppendLine("         LEFT OUTER JOIN GerarchiaImprese ON Impresa.PIVA = GerarchiaImprese.Figlio ")
            stbQuery.AppendLine("         LEFT OUTER JOIN Imprese CooperativaPadre ON GerarchiaImprese.Padre = CooperativaPadre.PIVA ")
            stbQuery.AppendLine("         LEFT OUTER JOIN ImpresexIndirizzi ImpresexIndirizzo_Coop ON  ImpresexIndirizzo_Coop.PIVA = CooperativaPadre.PIVA ")
            stbQuery.AppendLine("         LEFT OUTER JOIN Indirizzi Indirizzo_Coop ON ImpresexIndirizzo_Coop.cod_indirizzo = Indirizzo_Coop.cod_indirizzo ")
            stbQuery.AppendLine("         LEFT OUTER JOIN ISTAT ISTAT_Coop ON ISTAT_Coop.PROV = Indirizzo_Coop.pro_cod_istat AND ISTAT_Coop.COM = Indirizzo_Coop.com_cod_istat ")

            stbQuery.AppendLine(" WHERE       Impresa.Piva = '" & Agro_SQL_SaveText(piva) & "' ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    'legge solo i dati necessari per l'intestazione dei report della contabilità
    Public Function DatiIntestazioneImpresa_ReportContab(ByVal piva As String,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri As AgronicaCoreParametri
                                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocContab.DatiIntestazioneImpresa_ReportContab"

        Dim messaggioErrore As String = ""
        Dim stbQuery As New StringBuilder
        Dim dt As DataTable

        Try

            stbQuery.Length = 0

            stbQuery.AppendLine("  SELECT       Impresa.PIVA, CASE WHEN ISNULL(Impresa.partitaIvaReale, '') = '' THEN Impresa.PIVA ELSE Impresa.partitaIvaReale END AS PivaReale, Impresa.rag_soc,       ")
            stbQuery.AppendLine("               ImpresexIndirizzo_Impresa.Tipo_Indirizzo, Indirizzo_Impresa.ind_des, Indirizzo_Impresa.frz_des, ISTAT_Impresa.LOCALITA, ISTAT_Impresa.COMUNI_PROV, ISTAT_Impresa.CAP, ")

            stbQuery.AppendLine("             ISNULL  ((  SELECT    TOP 1 val_cod  ")
            stbQuery.AppendLine("                         FROM    Imprese_Codici AS Imprese_Codici_4  ")
            stbQuery.AppendLine("                         WHERE   Imprese_Codici_4.piva = Impresa.piva AND Imprese_Codici_4.id_cod = 1010), ' ') AS codice_cuaa, ")

            stbQuery.AppendLine("             ISNULL(Contatti.Codice_Fiscale, '') AS Codice_Fiscale  ")

            stbQuery.AppendLine(" FROM    Imprese Impresa ")
            stbQuery.AppendLine("         LEFT OUTER JOIN ImpresexIndirizzi ImpresexIndirizzo_Impresa ON  Impresa.PIVA = ImpresexIndirizzo_Impresa.PIVA ")
            stbQuery.AppendLine("         LEFT OUTER JOIN Indirizzi Indirizzo_Impresa ON Indirizzo_Impresa.cod_indirizzo = ImpresexIndirizzo_Impresa.cod_indirizzo ")
            stbQuery.AppendLine("         LEFT OUTER JOIN ISTAT ISTAT_Impresa ON Indirizzo_Impresa.pro_cod_istat = ISTAT_Impresa.PROV AND Indirizzo_Impresa.com_cod_istat = ISTAT_Impresa.COM ")
            stbQuery.AppendLine("         LEFT OUTER JOIN Contatti ON Impresa.PIVA = Contatti.Cod_Contatto ")

            stbQuery.AppendLine(" WHERE       Impresa.Piva = '" & Agro_SQL_SaveText(piva) & "' ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    '###############################################################################
    Public Sub Prepara_Parametri_Intestazione_ReportContab(ByVal Piva As String,
                                                           ByVal Data_inizio As Date,
                                                           ByVal Data_fine As Date,
                                                           ByRef Param_Rag_Soc As String,
                                                           ByRef Param_Piva_CodFiscale As String,
                                                           ByRef Param_Indirizzo As String,
                                                           ByRef Param_Intervallo_Date As String,
                                                           ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocContab.Prepara_Parametri_Intestazione_ReportContab()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Param_Rag_Soc = ""
        Param_Piva_CodFiscale = ""
        Param_Indirizzo = ""
        Param_Intervallo_Date = ""

        Try

            dt = DatiIntestazioneImpresa_ReportContab(Piva, "", "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                Param_Rag_Soc = dt.Rows(0).Item("Rag_Soc")
                Param_Piva_CodFiscale = "Piva: " & Piva & " - Cod. fiscale: " & dt.Rows(0).Item("Codice_Fiscale")
                Param_Indirizzo = dt.Rows(0).Item("ind_des") & " - " & dt.Rows(0).Item("cap") & " " & dt.Rows(0).Item("frz_des") & " " & dt.Rows(0).Item("LOCALITA") & " (" & dt.Rows(0).Item("COMUNI_PROV") & ") "

                Param_Rag_Soc = UtilityProvider.QS_SaveText(Param_Rag_Soc)
                Param_Piva_CodFiscale = UtilityProvider.QS_SaveText(Param_Piva_CodFiscale)
                Param_Indirizzo = UtilityProvider.QS_SaveText(Param_Indirizzo)
            End If

            Param_Intervallo_Date = "Dal " & UtilityProvider.Sistema_ValiditaInizio(Data_inizio) & _
                                    " al " & UtilityProvider.Sistema_ValiditaInizio(Data_fine)


        Catch ex As Exception
            Param_Rag_Soc = ""
            Param_Piva_CodFiscale = ""
            Param_Indirizzo = ""
            Param_Intervallo_Date = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub



    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' report excel agenti provvigioni
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function AgentiProvvigioni(ByVal Piva As String,
                                      ByVal Lav_Cod As Integer,
                                      ByVal mat_cod As Integer,
                                      ByVal cod_risum_agente As Integer,
                                      ByVal cod_risum_cliente As Integer,
                                      ByVal Data_Inizio As Date,
                                      ByVal Data_Fine As Date,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As DataTable

        'ByVal chk_incassato As Integer,
        '            ByVal chk_nonincassato As Integer,
        '            ByVal chk_parzialmenteincassato As Integer,


        Const nomeRoutine = "AgronicaCoreStampeDAL.DocContab.AgentiProvvigioni"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable
        'Dim i As Integer

        Try

            stbSql.Length = 0

            'StbSQL.AppendLine(" SELECT * ")
            'StbSQL.AppendLine(" FROM ")

            'StbSQL.AppendLine(" ( ")


            ''------------------------------------------------------------------------
            ''---- PRIMA PARTE DELL'UNION: dettagli con magazzino --------------------
            ''------------------------------------------------------------------------
            'StbSQL.AppendLine(" ( ")



            '------------------------------------------------------ 
            '------------------- SELECT ---------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" SELECT Agenda.PIVA,Agenda.sa_cod, Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, ")

            stbSql.AppendLine(" Mov_Contabile.Id_Mov AS Id_Mov_Contabile, Mov_Contabile.Data_Movimento, Mov_Contabile.Ora , ")
            stbSql.AppendLine(" Mov_Contabile.Doc_Numero_Sin, Mov_Contabile.Doc_Numero,Mov_Contabile.Doc_Numero_Des, Mov_Contabile.Mov_Desc ,  ")
            stbSql.AppendLine(" Mov_Contabile.Scadenza, Mov_Contabile.Num_Protocollo, Mov_Contabile.Causale_Trasporto, Mov_Contabile.Aspetto, Mov_Contabile.Peso,  ")
            stbSql.AppendLine(" Mov_Contabile.Colli, Mov_Contabile.Extra_Str, Mov_Contabile.Extra_Int, Mov_Contabile.Extra_Date,  ")
            stbSql.AppendLine(" Mov_Contabile.Tipo_Sconto, Mov_Contabile.Natura_Beni, Mov_Contabile.Tara_Veicolo, Mov_Contabile.Tara_Imballi, Mov_Contabile.Tipo_Peso, Mov_Contabile.Modalita, Mov_Contabile.Username_Note,  ")
            stbSql.AppendLine(" Mov_Contabile.Scadenza_Extra, Mov_Contabile.Progr_Protocollo, Mov_Contabile.Progr_Registrazione, Mov_Contabile.Data_Registrazione,  ")
            stbSql.AppendLine("  ")
            stbSql.AppendLine(" Mov_Contabile.Cod_RisUm, Contatti_Cliente.Cod_Contatto AS Cod_Contatto_Cliente, Contatti_Cliente.Codice_Fiscale AS Codice_Fiscale_Cliente,  ")
            stbSql.AppendLine(" Contatti_Cliente.Id_Cf AS Id_Cf_Cliente, Contatti_Cliente.Rag_Soc + ' ' + Contatti_Cliente.Nome + ' ' + Contatti_Cliente.Cognome AS Cliente,  ")
            stbSql.AppendLine("  Mov_Contabile.Cod_IndirizzoRisUm,  ")
            'StbSQL.AppendLine(" Indirizzi_Cliente.ind_des AS ind_des_Cliente, Indirizzi_Cliente.frz_des AS frz_des_Cliente, Indirizzi_Cliente.CAP AS cap_Cliente,  ")
            'StbSQL.AppendLine(" Indirizzi_Cliente.stato AS stato_Cliente, Indirizzi_Cliente.pro_cod_istat AS pro_cod_istat_Cliente,  ")
            'StbSQL.AppendLine(" Indirizzi_Cliente.com_cod_istat AS com_cod_istat_Cliente, Istat_Cliente.LOCALITA AS localita_Cliente, Istat_Cliente.COMUNI_PROV AS comuni_prov_Cliente, ")
            stbSql.AppendLine("  ")
            stbSql.AppendLine(" Movimento_Extra.Agente_Cod, Movimento_Extra.Provvigione, Contatti_Agente.Rag_Soc + ' ' + Contatti_Agente.Nome + ' ' + Contatti_Agente.Cognome AS Nome_Agente, ")
            stbSql.AppendLine("  ")
            stbSql.AppendLine(" Mov_Magazzino.Id_Mov AS Id_Mov_Magazzino, Mov_Magazzino.Mov_Desc AS Mov_Desc_Mag, Mov_Magazzino.Data_Movimento AS Data_Mag,Mov_Magazzino.Ora AS Ora_Mag,  ")
            stbSql.AppendLine("  ")
            stbSql.AppendLine(" Dettagli_Extra.Id_Reg_Dettaglio AS Id_Dettagli_Extra, Dettagli_Extra.Agente_Cod AS Agente_Cod_Dett , Dettagli_Extra.Provvigione AS Provvigione_Dett ,   ")
            stbSql.AppendLine("  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Id_Mov_Det, Mov_Dett_Magazzino.Mov_Det_Des, Mov_Dett_Magazzino.Elem_Cod, Mov_Dett_Magazzino.Pro_Cod, Mov_Dett_Magazzino.Mat_Cod,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Cod_Progetto, Mov_Dett_Magazzino.Fase_Cod, Mov_Dett_Magazzino.Lotto, Mov_Dett_Magazzino.Cal_Cod, Mov_Dett_Magazzino.Udm_Cod, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Qta AS Qta, Mov_Dett_Magazzino.Variazione AS Variazione,Mov_Dett_Magazzino.Listino_Cod, Mov_Dett_Magazzino.Tara,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Prezzo_Unitario, Mov_Dett_Magazzino.Prezzo_Unitario_Netto, Mov_Dett_Magazzino.Imponibile, Mov_Dett_Magazzino.Imponibile_Netto,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Jolly_Int, Mov_Dett_Magazzino.Contabilizzato,Mov_Dett_Magazzino.Pendente, Mov_Dett_Magazzino.Prezzo_Effettivo, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Cod_Iva, Mov_Dett_Magazzino.Sconto, Mov_Dett_Magazzino.Cod_Conto, Mov_Dett_Magazzino.Extra_Str AS Extra_Str_Dett, Mov_Dett_Magazzino.Extra_Int AS Extra_Int_Dett, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Extra_Date AS Extra_Date_Dett, Mov_Dett_Magazzino.Anno, Mov_Dett_Magazzino.Ric_Cod, Mov_Dett_Magazzino.Iva, Mov_Dett_Magazzino.UDM_COD_EXTRA, Mov_Dett_Magazzino.QTA_EXTRA,  ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.Cod_IvaIndetraibile, Mov_Dett_Magazzino.Qta_Extra_Totale, Mov_Dett_Magazzino.ChkLayOut_Hide, ")
            stbSql.AppendLine(" Mov_Dett_Magazzino.sconto_listino, ISNULL(Mov_Dett_Magazzino.sconto_Testo, '') AS Sconto_Testo, Mov_Dett_Magazzino.Sconto_modalita, Mov_Dett_Magazzino.chkiva_manuale, Mov_Dett_Magazzino.Mat_Cod_Alias, Mov_Dett_Magazzino.Mezzo_Det, ")
            stbSql.AppendLine(" IVA_Aliquote.Sigla AS Sigla_IVA ")
            'modifica del 31/10/2014: tolgo il magazzino
            'StbSQL.AppendLine(" ,Mov_Destinazioni.Sa_cod AS Sa_Cod_Dest, Id_Destinazione, Mov_Destinazioni.Tipo_Destinazione, Mov_Destinazioni.Qta AS Qta_Dest ")
            stbSql.AppendLine("  ")

            'StbSQL.AppendLine(" ,UnitaMisura.Udm_Des, UnitaMisura.Udm_Sim,   ")
            ''Dettagli_Extra.Codice_Prodotto, Dettagli_Extra.Colore, Dettagli_Extra.Zona_Viticola, Dettagli_Extra.Manipolazioni, 
            'StbSQL.AppendLine(" Materie_Prime.codice_prodotto, Materie_Prime.colore, Materie_Prime.codice_nc, Materie_Prime.manipolazioni ")
            'StbSQL.AppendLine("  ")

            '------------------------------------------------------ 
            '-------------------- FROM ----------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" FROM Agenda  ")

            stbSql.AppendLine(" INNER JOIN Movimenti Mov_Contabile ON Agenda.PIVA = Mov_Contabile.PIVA AND Agenda.Sa_Cod = Mov_Contabile.Sa_Cod AND Agenda.Id_Agenda = Mov_Contabile.Id_Agenda  ")
            stbSql.AppendLine("  ")
            stbSql.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Cliente ON Mov_Contabile.Cod_RisUm = Risorse_Umane_Cliente.Cod_RisUm  ")
            stbSql.AppendLine(" INNER JOIN Contatti Contatti_Cliente ON Risorse_Umane_Cliente.Piva = Contatti_Cliente.Piva AND Risorse_Umane_Cliente.Cod_Contatto = Contatti_Cliente.Cod_Contatto  ")
            'StbSQL.AppendLine(" INNER JOIN Indirizzi Indirizzi_Cliente ON  Mov_Contabile.Cod_IndirizzoRisUm = Indirizzi_Cliente.cod_indirizzo ")
            'StbSQL.AppendLine(" INNER JOIN ISTAT Istat_Cliente ON Indirizzi_Cliente.pro_cod_istat = Istat_Cliente.PROV AND Indirizzi_Cliente.com_cod_istat = Istat_Cliente.COM ")
            stbSql.AppendLine("  ")
            stbSql.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Movimento_Extra ON Movimento_Extra.PIVA = Mov_Contabile.PIVA AND Movimento_Extra.Sa_Cod = Mov_Contabile.Sa_Cod AND Movimento_Extra.Id_Agenda = Mov_Contabile.Id_Agenda AND Movimento_Extra.Id_Mov = Mov_Contabile.Id_Mov ")
            stbSql.AppendLine(" INNER JOIN Risorse_Umane Risorse_Umane_Agente ON Movimento_Extra.Agente_Cod = Risorse_Umane_Agente.Cod_RisUm  ")
            stbSql.AppendLine(" INNER JOIN Contatti Contatti_Agente ON Risorse_Umane_Agente.Piva = Contatti_Agente.Piva AND Risorse_Umane_Agente.Cod_Contatto = Contatti_Agente.Cod_Contatto  ")
            stbSql.AppendLine("  ")
            'StbSQL.AppendLine("  ")
            stbSql.AppendLine(" INNER JOIN Movimenti Mov_Magazzino ON Agenda.PIVA = Mov_Magazzino.PIVA AND Agenda.Sa_Cod = Mov_Magazzino.Sa_Cod AND Agenda.Id_Agenda = Mov_Magazzino.Id_Agenda  ")
            stbSql.AppendLine(" INNER JOIN Movimenti_Dettagli Mov_Dett_Magazzino ON Mov_Magazzino.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Magazzino.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Magazzino.Id_Mov = Mov_Dett_Magazzino.Id_Mov  ")
            'modifica del 31/10/2013: tolgo il magazzino
            'StbSQL.AppendLine(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.PIVA = Mov_Dett_Magazzino.PIVA AND Mov_Destinazioni.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Mov_Destinazioni.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det ")
            stbSql.AppendLine(" INNER JOIN IVA_Aliquote ON IVA_Aliquote.Codice = Mov_Dett_Magazzino.Cod_Iva ")
            'StbSQL.AppendLine("  ")
            'StbSQL.AppendLine(" INNER JOIN Materie_Prime ON Materie_Prime.Elem_Cod = Mov_Dett_Magazzino.Elem_Cod AND Materie_Prime.Mat_Cod = Mov_Dett_Magazzino.Mat_Cod   ")
            'StbSQL.AppendLine("  ")
            stbSql.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico_Extra Dettagli_Extra ON Dettagli_Extra.PIVA = Mov_Dett_Magazzino.PIVA  ")
            stbSql.AppendLine(" -- commentato join su sa_cod perchè il giaslan molto spesso salva sa_cod=0 su dettagli_extra  ")
            stbSql.AppendLine(" -- AND Dettagli_Extra.Sa_Cod = Mov_Dett_Magazzino.Sa_Cod  ")
            stbSql.AppendLine(" AND Dettagli_Extra.Id_Agenda = Mov_Dett_Magazzino.Id_Agenda AND Dettagli_Extra.Id_Mov = Mov_Dett_Magazzino.Id_Mov AND Dettagli_Extra.Id_Mov_Det = Mov_Dett_Magazzino.Id_Mov_Det ")
            stbSql.AppendLine("  ")
            'StbSQL.AppendLine(" INNER JOIN UnitaMisura ON UnitaMisura.Udm_Cod = Mov_Dett_Magazzino.Udm_Cod ")
            'StbSQL.AppendLine("  ")

            '------------------------------------------------------ 
            '-------------------- WHERE ---------------------------
            '------------------------------------------------------
            stbSql.AppendLine(" WHERE Mov_Contabile.Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")
            'i documenti in uscita hanno il cau_mov dello scarico, ma i resi hanno quello del carico 
            'le bolle di conferimento hanno il loro cau_mov
            stbSql.AppendLine(" AND    Mov_Magazzino.Cau_Mov IN ( '" &
                                    CAU_SCARICO & "', '" &
                                    CAU_CARICO & "', '" &
                                    CAU_ABBUONI & "', '" &
                                    CAU_CONFERIMENTO & "', '" &
                                    CAU_CONFERIMENTO_DIVERSI & "', '" &
                                    CAU_ACCETTAZIONE_BENI & "', '" &
                                    CAU_ACCETTAZIONE_BENI_DA_DIVERSI &
                                    "'  ) ")

            stbSql.AppendLine(" AND    Agenda.Lav_Cod IN ( " &
                        LAVCOD_RICEVUTA_EMESSA & ", " &
                        LAVCOD_DDT_CONTABILIZZATO_EMESSO & ", " &
                        LAVCOD_NOTA_ACCREDITO_EMESSA & ", " &
                        LAVCOD_FATTURA_EMESSA & " " &
                                                        "  ) ")

            stbSql.AppendLine(" AND Mov_Contabile.Data_Movimento <= " & Agro_SQL_SaveDate(Data_Fine) & " ")
            stbSql.AppendLine(" AND Mov_Contabile.Data_Movimento >= " & Agro_SQL_SaveDate(Data_Inizio) & " ")

            If Piva <> "" Then
                stbSql.AppendLine(" AND Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Lav_Cod <> 0 Then
                stbSql.AppendLine(" AND Agenda.Lav_Cod = " & CStr(Lav_Cod) & " ")
            End If

            If mat_cod <> 0 Then
                stbSql.AppendLine(" AND Mov_Dett_Magazzino.mat_cod = " & CStr(mat_cod) & " ")
            End If

            If cod_risum_agente <> 0 Then
                stbSql.AppendLine(" AND Movimento_Extra.Agente_Cod = " & CStr(cod_risum_agente) & " ")
            Else
                'deve essere comunque valorizzato un agente, altrimenti vengono fuori tutti i documenti
                stbSql.AppendLine(" AND Movimento_Extra.Agente_Cod <> 0 ")
            End If

            If cod_risum_cliente <> 0 Then
                stbSql.AppendLine(" AND Risorse_Umane_Cliente.Cod_RisUm = " & CStr(cod_risum_cliente) & " ")
            End If

            'If (chk_incassato = 0 And chk_nonincassato = 0 And chk_parzialmenteincassato = 0) _
            'Or (chk_incassato = 1 And chk_nonincassato = 1 And chk_parzialmenteincassato = 1) Then
            '    'nessun filtro sui pagamenti, tutto
            'End If

            'If chk_incassato = 1 And chk_nonincassato = 0 And chk_parzialmenteincassato = 0 Then
            '    'solo incassati


            'End If

            'If chk_incassato = 0 And chk_nonincassato = 1 And chk_parzialmenteincassato = 0 Then
            '    'solo non incassati


            'End If

            'If chk_incassato = 0 And chk_nonincassato = 0 And chk_parzialmenteincassato = 1 Then
            '    'solo parzialmente incassati


            'End If

            'If chk_incassato = 1 And chk_nonincassato = 1 And chk_parzialmenteincassato = 0 Then
            '    ' incassati e non incassati


            'End If

            'If chk_incassato = 1 And chk_nonincassato = 0 And chk_parzialmenteincassato = 1 Then
            '    ' incassati e parzialmente incassati


            'End If

            'If chk_incassato = 0 And chk_nonincassato = 1 And chk_parzialmenteincassato = 1 Then
            '    'non incassati e parzialmente incassati


            'End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                stbSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stbSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stbSql.AppendLine(" ORDER BY Nome_Agente, Mov_Contabile.Data_Movimento, Mov_Contabile.Doc_numero ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function

    Public Function Tare_Confez_Contenitore_Imballo_FF(ByVal Id_Mov_Det As Integer,
                                                       ByVal tipo As String,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As String

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocContab.Tare_Confez_Contenitore_Imballo_FF"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable

        Dim tara As String = ""

        Try
            stbSql.AppendLine("SELECT Tara_Campionatura ")
            stbSql.AppendLine(" FROM Materie_Prime_Campionature ")
            stbSql.AppendLine(" INNER JOIN Movimenti_Dettagli ON Materie_Prime_Campionature.progressivo = Movimenti_Dettagli.Cal_Cod ")
            stbSql.AppendLine(" WHERE Movimenti_Dettagli.Id_Mov_Det = " & Id_Mov_Det & " ")
            stbSql.AppendLine(" AND Materie_Prime_Campionature.tipo = '" & Agro_SQL_SaveText(tipo) & "'")
            stbSql.AppendLine(" AND Materie_Prime_Campionature.tipo_cod <> 0 ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                tara = CStr(dt.Rows(0).Item("Tara_Campionatura")).ToString()
            Else
                tara = ""
            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            tara = ""
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return tara

    End Function

    Public Function Descrizione_Confezione_FF(ByVal Id_Mov_Det As Integer,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As String

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocContab.Descrizione_Confezione_FF"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable

        Dim descrizione As String = ""

        Try
            stbSql.AppendLine("SELECT OTabelle_Parametri.Descrizione ")
            stbSql.AppendLine(" FROM Materie_Prime_Campionature ")
            stbSql.AppendLine(" INNER JOIN OTabelle_Parametri ON  Materie_Prime_Campionature.tipo_cod = OTabelle_Parametri.Tabella_Par_Cod ")
            stbSql.AppendLine(" INNER JOIN Movimenti_Dettagli ON Materie_Prime_Campionature.progressivo = Movimenti_Dettagli.Cal_Cod ")
            stbSql.AppendLine(" WHERE Movimenti_Dettagli.Id_Mov_Det = " & Id_Mov_Det & " ")
            stbSql.AppendLine(" AND Materie_Prime_Campionature.tipo = 'oconfezione' ")
            stbSql.AppendLine(" AND OTabelle_Parametri.Tabella_Cod = " & enum_OTabelle.Confezione & " ")
            stbSql.AppendLine(" AND Materie_Prime_Campionature.tipo_cod <> 0 ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                descrizione = CStr(dt.Rows(0).Item("Descrizione"))
            Else
                descrizione = ""
            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            descrizione = ""
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return descrizione

    End Function

    Public Function Descrizione_Certificazione_FF(ByVal Id_Mov_Det As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As String

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocContab.Descrizione_Certificazione_FF"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable

        Dim descrizione As String = ""

        Try
            stbSql.AppendLine("SELECT OTabelle_Parametri.Descrizione ")
            stbSql.AppendLine(" FROM Materie_Prime_Campionature ")
            stbSql.AppendLine(" INNER JOIN OTabelle_Parametri ON  Materie_Prime_Campionature.tipo_cod = OTabelle_Parametri.Tabella_Par_Cod ")
            stbSql.AppendLine(" INNER JOIN Movimenti_Dettagli ON Materie_Prime_Campionature.progressivo = Movimenti_Dettagli.Cal_Cod ")
            stbSql.AppendLine(" WHERE Movimenti_Dettagli.Id_Mov_Det = " & Id_Mov_Det & " ")
            stbSql.AppendLine(" AND OTabelle_Parametri.Tabella_Cod = " & enum_OTabelle.Certificazioni & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                descrizione = CStr(dt.Rows(0).Item("Descrizione"))
            Else
                descrizione = ""
            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            descrizione = ""
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return descrizione

    End Function

    Public Function Descrizione_Certificazione_FF_from_Cal_Cod(ByVal Cal_Cod As Integer,
                                                               ByRef objParametri As AgronicaCoreParametri
                                                               ) As String

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocContab.Descrizione_Certificazione_FF_from_Cal_Cod"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable

        Dim descrizione As String = ""

        Try
            stbSql.AppendLine("SELECT OTabelle_Parametri.Descrizione ")
            stbSql.AppendLine(" FROM Materie_Prime_Campionature ")
            stbSql.AppendLine(" INNER JOIN OTabelle_Parametri ON  Materie_Prime_Campionature.tipo_cod = OTabelle_Parametri.Tabella_Par_Cod ")
            stbSql.AppendLine(" WHERE Materie_Prime_Campionature.progressivo = " & Cal_Cod & " ")
            stbSql.AppendLine(" AND OTabelle_Parametri.Tabella_Cod = " & enum_OTabelle.Certificazioni & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                descrizione = CStr(dt.Rows(0).Item("Descrizione"))
            Else
                descrizione = ""
            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            descrizione = ""
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return descrizione

    End Function

    Public Function DocumentiImputatiAnnoSbagliato(ByVal piva As String,
                                                   ByVal anno As Integer,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocContab.DocumentiImputatiAnnoSbagliato"

        Dim messaggioErrore As String = ""
        Dim stbG As New StringBuilder
        Dim stbSql As New StringBuilder
        Dim dt As DataTable

        'Dim descrizione As String = ""

        Try
            stbSql.AppendLine("SELECT ag.Des_Lib AS Documento, ")
            stbSql.AppendLine(" CAST(m_intest.data_movimento AS date) AS Data_Emissione, ")
            stbSql.AppendLine(" CAST(m_intest.Data_Registrazione AS date) AS Data_Registrazione, ")
            stbSql.AppendLine(" md.Anno AS Anno_Imputato, ")
            stbSql.AppendLine(" md.*, m_intest.* ")

            stbSql.AppendLine(" FROM movimenti_dettagli md ")
            stbSql.AppendLine(" INNER JOIN Movimenti m_intest ON md.piva = m_intest.piva AND md.id_agenda = m_intest.id_agenda AND m_intest.cau_mov = '4000' ")
            stbSql.AppendLine(" INNER JOIN Agenda ag ON ag.id_agenda = md.Id_agenda AND ag.id_agenda = m_intest.id_agenda ")

            stbSql.AppendLine(" WHERE md.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            stbSql.AppendLine(" AND md.anno <> 0 ")

            '21/01/2019: aggiunto anche cod_conto
            stbSql.AppendLine(" AND ( md.cod_conto_pat <> 0 OR md.cod_conto <> 0 )")

            '21/01/2019: commentato perché ci sono i casi di fatture ricevute emesse in un anno e registrate nell'altro
            'stbSql.AppendLine(" AND YEAR (m_intest.data_registrazione) = YEAR (m_intest.Data_Movimento) ")

            '21/01/2019: nuovo filtro, il controllo deve essere fatto solo sui documenti della vecchia contabilizzazione automatica
            stbSql.AppendLine(" AND ag.chkcoge_manuale = " & Agro_SQL_SaveNum(enum_ChkCoGe.CoGe_VECCHIA_CONTABILIZZAZIONE_AUTOMATICA))

            '21/01/2019: messi solo i lav_cod gestiti e che riguardano la contabilità, in base al filtro sulla data (vedi sotto)
            'stbSql.AppendLine(" AND ag.lav_cod IN (1008, 1009, 1010, 1011, 1012, 1013, 1014, 1015, 1024, ")
            'stbSql.AppendLine(" 				   1017, 1016, 1018, 1019, 1031, 1025, 1069, 1001, 1000, ")
            'stbSql.AppendLine(" 				   1070, 1064, 1055, 1056, 1057, 1058, 1053, 1027, 1026, ")
            'stbSql.AppendLine(" 				   1002, 1003, 1032, 1005, 1004, 1006, 1007, 1060, 1079, 1020) ")


            '========================================================================

            '------------------------------------
            '------- DOCUMENTI RICEVUTI ---------
            '------------------------------------
            stbG.Append(stbSql)
            stbG.AppendLine(" AND Ag.Lav_Cod IN ( " &
                                               CStr(LAVCOD_FATTURA_RICEVUTA) & ", " &
                                               CStr(LAVCOD_FATTURA_PROFESSIONISTI) & ", " &
                                               CStr(LAVCOD_FATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                               CStr(LAVCOD_NOTA_ACCREDITO_RICEVUTA) & ", " &
                                               CStr(LAVCOD_ACQUISTO) & ", " &
                                               CStr(LAVCOD_MOV_FINANZIARIO) & ", " &
                                               CStr(LAVCOD_ALTRI_COSTI) &
                                               " ) ")

            stbG.AppendLine(" AND YEAR (m_intest.data_registrazione) <> 1900 ")
            stbG.AppendLine(" AND md.Anno <> YEAR (m_intest.data_registrazione) ")
            If anno <> 0 Then
                stbG.AppendLine(" AND YEAR (m_intest.data_registrazione) = " & Agro_SQL_SaveNum(anno) & " ")
            End If

            stbG.Append(vbCrLf)
            stbG.Append(" UNION ALL ")
            stbG.Append(vbCrLf)

            '------------------------------------
            '------- DOCUMENTI EMESSI ---------
            '------------------------------------

            stbG.Append(stbSql)
            stbG.AppendLine(" AND Ag.Lav_Cod IN ( " &
                                                       CStr(LAVCOD_FATTURA_EMESSA) & ", " &
                                                       CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) & ", " &
                                                       CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) & ", " &
                                                       CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA) & ", " &
                                                       CStr(LAVCOD_NOTA_ACCREDITO_EMESSA) & ", " &
                                                       CStr(LAVCOD_RICEVUTA_EMESSA) & ", " &
                                                       CStr(LAVCOD_CORRISPETTIVO_VENDITA_SFUSO) & ", " &
                                                       CStr(LAVCOD_DDT_CONTABILIZZATO_EMESSO) & ", " &
                                                       CStr(LAVCOD_AUTOCONSUMO) & ", " &
                                                       CStr(LAVCOD_AUTOCONSUMO_VINO_SFUSO) & ", " &
                                                       CStr(LAVCOD_VENDITA) & ", " &
                                                       CStr(LAVCOD_ALTRI_RICAVI) & " " &
                                                       " ) ")

            stbG.AppendLine(" AND YEAR (m_intest.Data_Movimento) <> 1900 ")
            stbG.AppendLine(" AND md.Anno <> YEAR (m_intest.Data_Movimento) ")
            If anno <> 0 Then
                stbG.AppendLine(" AND YEAR (m_intest.Data_Movimento) = " & Agro_SQL_SaveNum(anno) & " ")
            End If


            If xOrderBy <> "" Then
                stbG.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stbG.AppendLine(" ORDER BY m_intest.Data_Movimento DESC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbG.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            'If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            '    descrizione = CStr(dt.Rows(0).Item("Descrizione"))
            'Else
            '    descrizione = ""
            'End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            'descrizione = ""
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return dt

    End Function


    '###########################################################################
    Public Function DocumentiSezionaleCod_DiversoDa_SezionaleCodContabilizzazione(ByVal piva As String,
                                                                                  ByVal DataDa As Date,
                                                                                  ByVal xOrderBy As String,
                                                                                  ByRef objParametri As AgronicaCoreParametri
                                                                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocContab.DocumentiSezionaleCod_DiversoDa_SezionaleCodContabilizzazione"

        Dim messaggioErrore As String = ""
        Dim stbSql As New StringBuilder
        Dim dt As DataTable

        Try

            stbSql.Length = 0
            stbSql.AppendLine(" SELECT  m.sezionale_cod as sezionale_cod_documento, IS_doc.Sezionale_Des AS sezionale_des_documento, mfin.sezionale_cod as sezionale_cod_contabil, IS_contabil.Sezionale_Des AS sezionale_des_contabil, " & vbCrLf)
            stbSql.AppendLine(" a.id_agenda as id_agenda_doc,a.lav_cod AS lav_cod,a.des_lib, m.data_movimento, a.Data_Modifica, mfin.id_agenda as id_agenda_contabil " & vbCrLf)
            stbSql.AppendLine(" FROM Movimenti m  " & vbCrLf)
            stbSql.AppendLine(" INNER JOIN Imprese_Sezionali IS_doc ON IS_doc.piva = m.piva AND IS_doc.Sezionale_Cod = m.Sezionale_Cod ")
            stbSql.AppendLine(" INNER JOIN agenda a ON a.piva=m.piva AND a.id_agenda=m.id_agenda " & vbCrLf)
            stbSql.AppendLine(" INNER JOIN Mov_Dettagli_Riferimenti mdr ON a.piva=mdr.piva_rif AND a.id_agenda = mdr.id_agenda_rif " & vbCrLf)
            stbSql.AppendLine(" INNER JOIN movimenti mfin ON mfin.piva=mdr.piva AND mfin.id_agenda = mdr.id_agenda " & vbCrLf)
            stbSql.AppendLine(" INNER JOIN Imprese_Sezionali IS_contabil ON IS_contabil.piva = mfin.piva AND IS_contabil.Sezionale_Cod = mfin.Sezionale_Cod " & vbCrLf)
            stbSql.AppendLine(" WHERE mdr.Lav_Cod = " & CStr(LAVCOD_MOV_FINANZIARIO) & vbCrLf)
            stbSql.AppendLine(" AND  a.piva = '" & Agro_SQL_SaveText(piva) & "' " & vbCrLf)
            stbSql.AppendLine(" AND m.cau_mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)
            stbSql.AppendLine(" AND m.sezionale_cod <> mfin.sezionale_cod " & vbCrLf)
            stbSql.AppendLine(" AND m.data_movimento >= " & Agro_SQL_SaveDate(DataDa) & " " & vbCrLf)
            stbSql.AppendLine(" ORDER BY m.Data_Movimento DESC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '###########################################################################
    Public Function DocumentiSezionaleCod_DiversoDa_SezionaleCodConto(ByVal piva As String,
                                                                      ByVal DataDa As Date,
                                                                      ByVal xOrderBy As String,
                                                                      ByRef objParametri As AgronicaCoreParametri
                                                                      ) As DataTable

        Const nomeRoutine = "AgronicaCoreStampeDAL.DocContab.DocumentiSezionaleCod_DiversoDa_SezionaleCodConto"

        Dim messaggioErrore As String = ""
        Dim stbSqlG As New StringBuilder
        Dim stbSelectEco As New StringBuilder
        Dim stbSelectPat As New StringBuilder
        Dim stbJoinEco1 As New StringBuilder
        Dim stbJoinEco2 As New StringBuilder
        Dim stbJoinPat1 As New StringBuilder
        Dim stbJoinPat2 As New StringBuilder
        Dim stbWhereEco As New StringBuilder
        Dim stbWherePat As New StringBuilder
        Dim dt As DataTable

        Try

            stbSqlG.Length = 0

            '--- PARTE ECONOMICA

            stbSelectEco.AppendLine(" SELECT Agenda.piva, Agenda.id_agenda, Agenda.lav_cod, des_lib, Movimenti_Contab.data_movimento, Conti_Eco.Cod_Conto, Conti_Eco.Conto_Descr,  " & vbCrLf)
            stbSelectEco.AppendLine(" RicXConti_Eco.anno, RicXConti_Eco.Id_Riclassificazione,RicXConti_Eco.Sezionale_Cod_Conto, Sezionali_conto.Sezionale_Des as sezionale_des_conto, " & vbCrLf)
            stbSelectEco.AppendLine("  Movimenti_Contab.Sezionale_Cod, Sezionali_op.Sezionale_Des as sezionale_des_documento " & vbCrLf)

            stbJoinEco1.AppendLine(" FROM Agenda  " & vbCrLf)
            stbJoinEco1.AppendLine("  INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD " & vbCrLf)
            stbJoinEco1.AppendLine("  INNER JOIN Movimenti Movimenti_Contab  " & vbCrLf)
            stbJoinEco1.AppendLine("  ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda " & vbCrLf)
            stbJoinEco1.AppendLine("  INNER JOIN Pagamenti   " & vbCrLf)
            stbJoinEco1.AppendLine("  ON Pagamenti.PIVA = Movimenti_Contab.PIVA AND Pagamenti.Sa_Cod = Movimenti_Contab.Sa_Cod AND Pagamenti.Id_Agenda = Movimenti_Contab.Id_Agenda  AND Pagamenti.Id_MOV = Movimenti_Contab.Id_MOV    " & vbCrLf)
            stbJoinEco1.AppendLine(" INNER JOIN RicXConti RicXConti_Eco ON Pagamenti.PIVA = RicXConti_Eco.Piva AND Pagamenti.Anno = RicXConti_Eco.Anno AND Pagamenti.Ric_Cod = RicXConti_Eco.Ric_Cod    " & vbCrLf)

            stbJoinEco2.AppendLine("  INNER JOIN Conti Conti_Eco ON Conti_Eco.Cod_Conto = RicXConti_Eco.Cod_Conto    " & vbCrLf)
            stbJoinEco2.AppendLine(" INNER JOIN Riclassificazioni Riclassificazioni_Eco ON Riclassificazioni_Eco.Ric_Cod = RicXConti_Eco.Ric_Cod AND Riclassificazioni_Eco.Piva = RicXConti_Eco.Piva    " & vbCrLf)
            stbJoinEco2.AppendLine(" INNER JOIN Imprese Imprese_Ric ON RicXConti_Eco.Piva = Imprese_Ric.PIVA    " & vbCrLf)
            stbJoinEco2.AppendLine(" INNER JOIN Imprese_Sezionali Sezionali_conto ON  Sezionali_conto.Piva = RicXConti_Eco.piva AND  Sezionali_conto.Sezionale_Cod = RicXConti_Eco.Sezionale_Cod_Conto   " & vbCrLf)
            stbJoinEco2.AppendLine(" INNER JOIN Imprese_Sezionali Sezionali_op ON  Sezionali_op.Piva = Movimenti_Contab.piva AND  Sezionali_op.Sezionale_Cod = Movimenti_Contab.Sezionale_Cod   " & vbCrLf)

            stbWhereEco.AppendLine(" WHERE Agenda.Lav_Cod = " & CStr(LAVCOD_MOV_FINANZIARIO) & vbCrLf)
            stbWhereEco.AppendLine(" AND Agenda.piva = '" & Agro_SQL_SaveText(CStr(piva)) & "' " & vbCrLf)
            stbWhereEco.AppendLine(" AND Movimenti_Contab.cau_mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)
            stbWhereEco.AppendLine("  AND Riclassificazioni_Eco.Ric_Cod = 2   ")
            'stbWhereEco.AppendLine(" AND Movimenti_Contab.data_movimento >= " & Agro_SQL_SaveDate(DataDa) & " ")
            stbWhereEco.AppendLine("  AND RicXConti_Eco.Sezionale_Cod_Conto <> -1  ")
            stbWhereEco.AppendLine("  AND  RicXConti_Eco.Sezionale_Cod_Conto <> Movimenti_Contab.Sezionale_Cod    ")

            '--- PARTE PATRIMONIALE

            stbSelectPat.AppendLine(" select Agenda.piva, Agenda.id_agenda, Agenda.lav_cod, des_lib, Movimenti_Contab.data_movimento, Conti_Pat.Cod_Conto_Pat, Conti_Pat.Conto_Pat_Descr,  " & vbCrLf)
            stbSelectPat.AppendLine(" RicXConti_Pat.anno, RicXConti_Pat.Id_Riclassificazione,RicXConti_Pat.Sezionale_Cod_Conto_Pat, Sezionali_conto.Sezionale_Des as sezionale_des_conto, " & vbCrLf)
            stbSelectPat.AppendLine("  Movimenti_Contab.Sezionale_Cod, Sezionali_op.Sezionale_Des as sezionale_des_documento " & vbCrLf)

            stbJoinPat1.AppendLine(" FROM Agenda  " & vbCrLf)
            stbJoinPat1.AppendLine(" INNER JOIN Operazioni ON Agenda.Lav_Cod = Operazioni.LAV_COD  " & vbCrLf)
            stbJoinPat1.AppendLine(" INNER JOIN Movimenti Movimenti_Contab  " & vbCrLf)
            stbJoinPat1.AppendLine(" ON Agenda.PIVA = Movimenti_Contab.PIVA AND Agenda.Sa_Cod = Movimenti_Contab.Sa_Cod AND Agenda.Id_Agenda = Movimenti_Contab.Id_Agenda " & vbCrLf)
            stbJoinPat1.AppendLine(" INNER JOIN Pagamenti  " & vbCrLf)
            stbJoinPat1.AppendLine(" ON Pagamenti.PIVA = Movimenti_Contab.PIVA AND Pagamenti.Sa_Cod = Movimenti_Contab.Sa_Cod AND Pagamenti.Id_Agenda = Movimenti_Contab.Id_Agenda  AND Pagamenti.Id_MOV = Movimenti_Contab.Id_MOV   " & vbCrLf)
            stbJoinPat1.AppendLine(" INNER JOIN RicxConti_Patrimonio RicXConti_Pat ON Pagamenti.PIVA = RicXConti_Pat.Piva AND Pagamenti.Anno = RicXConti_Pat.Anno AND Pagamenti.Ric_Cod = RicXConti_Pat.Ric_Cod_Pat  " & vbCrLf)

            stbJoinPat2.AppendLine(" INNER JOIN Conti_Patrimonio Conti_Pat ON Conti_Pat.Cod_Conto_Pat = RicXConti_Pat.Cod_Conto_Pat  " & vbCrLf)
            stbJoinPat2.AppendLine(" INNER JOIN Riclassificazioni_Patrimonio Riclassificazioni_Pat ON Riclassificazioni_Pat.Ric_Cod_Pat = RicXConti_Pat.Ric_Cod_Pat AND Riclassificazioni_Pat.Piva = RicXConti_Pat.Piva  " & vbCrLf)
            stbJoinPat2.AppendLine(" INNER JOIN Imprese Imprese_Ric ON RicXConti_Pat.Piva = Imprese_Ric.PIVA  " & vbCrLf)
            stbJoinPat2.AppendLine(" INNER JOIN Imprese_Sezionali Sezionali_conto ON  Sezionali_conto.Piva = RicXConti_Pat.piva AND  Sezionali_conto.Sezionale_Cod = RicXConti_Pat.Sezionale_Cod_Conto_Pat " & vbCrLf)
            stbJoinPat2.AppendLine(" INNER JOIN Imprese_Sezionali Sezionali_op ON  Sezionali_op.Piva = Movimenti_Contab.piva AND  Sezionali_op.Sezionale_Cod = Movimenti_Contab.Sezionale_Cod" & vbCrLf)

            stbWherePat.AppendLine(" WHERE Agenda.Lav_Cod = " & CStr(LAVCOD_MOV_FINANZIARIO) & vbCrLf)
            stbWherePat.AppendLine(" AND Agenda.piva = '" & Agro_SQL_SaveText(CStr(piva)) & "' " & vbCrLf)
            stbWherePat.AppendLine(" AND Movimenti_Contab.cau_mov = '" & CAU_REGISTRAZIONI & "' " & vbCrLf)
            stbWherePat.AppendLine("  AND Riclassificazioni_Pat.Ric_Cod_Pat = 2     ")
            'stbWherepat.AppendLine(" AND Movimenti_Contab.data_movimento >= " & Agro_SQL_SaveDate(DataDa) & " ")
            stbWherePat.AppendLine("  AND RicXConti_Pat.Sezionale_Cod_Conto_Pat  <> -1   ")
            stbWherePat.AppendLine("  AND  RicXConti_Pat.Sezionale_Cod_Conto_Pat <> Movimenti_Contab.Sezionale_Cod    ")


            '--- COMPOSIZIONE QUERY GENERALE

            stbSqlG.AppendLine(" -- CONTI ECONOMICI DARE" & vbCrLf)
            stbSqlG.AppendLine(stbSelectEco.ToString & vbCrLf)
            stbSqlG.AppendLine(stbJoinEco1.ToString & vbCrLf)
            stbSqlG.AppendLine(" AND Pagamenti.Cod_Conto_Dare = RicXConti_Eco.Cod_Conto  " & vbCrLf)
            stbSqlG.AppendLine(stbJoinEco2.ToString & vbCrLf)
            stbSqlG.AppendLine(stbWhereEco.ToString & vbCrLf)

            stbSqlG.AppendLine(" UNION ALL " & vbCrLf)

            stbSqlG.AppendLine(" -- CONTI ECONOMICI AVERE" & vbCrLf)
            stbSqlG.AppendLine(stbSelectEco.ToString & vbCrLf)
            stbSqlG.AppendLine(stbJoinEco1.ToString & vbCrLf)
            stbSqlG.AppendLine(" AND Pagamenti.Cod_Conto_Avere = RicXConti_Eco.Cod_Conto   " & vbCrLf)
            stbSqlG.AppendLine(stbJoinEco2.ToString & vbCrLf)
            stbSqlG.AppendLine(stbWhereEco.ToString & vbCrLf)

            stbSqlG.AppendLine(" UNION ALL " & vbCrLf)

            stbSqlG.AppendLine(" -- CONTI PATRIMONIALI DARE " & vbCrLf)
            stbSqlG.AppendLine(stbSelectPat.ToString & vbCrLf)
            stbSqlG.AppendLine(stbJoinPat1.ToString & vbCrLf)
            stbSqlG.AppendLine(" AND Pagamenti.Cod_Conto_Pat_Dare = RicXConti_Pat.Cod_Conto_Pat " & vbCrLf)
            stbSqlG.AppendLine(stbJoinPat2.ToString & vbCrLf)
            stbSqlG.AppendLine(stbWherePat.ToString & vbCrLf)

            stbSqlG.AppendLine(" UNION ALL " & vbCrLf)

            stbSqlG.AppendLine(" -- CONTI PATRIMONIALI AVERE" & vbCrLf)
            stbSqlG.AppendLine(stbSelectPat.ToString & vbCrLf)
            stbSqlG.AppendLine(stbJoinPat1.ToString & vbCrLf)
            stbSqlG.AppendLine(" AND Pagamenti.Cod_Conto_Pat_Avere = RicXConti_Pat.Cod_Conto_Pat  " & vbCrLf)
            stbSqlG.AppendLine(stbJoinPat2.ToString & vbCrLf)
            stbSqlG.AppendLine(stbWherePat.ToString & vbCrLf)

            stbSqlG.AppendLine(" ORDER BY Movimenti_Contab.Data_Movimento DESC ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbSqlG.ToString, nomeRoutine)
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
