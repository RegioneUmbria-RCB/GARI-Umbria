Imports System.Globalization
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Statistiche_R
    Inherits AgronicaCoreDataProvider.DataProvider

    'Report Vendite
    Public Const Report_Ordini_Vendita As String = "Report_Ordini_Vendita"
    Public Const Report_Vendite As String = "Report_Vendite"
    Public Const Report_Pdf_Vendita As String = "Report_Pdf_Vendita"
    Public Const Report_DDTVend_OrdLav As String = "Report_DDTVend_OrdLav"
    Public Const Report_OrdVend_OrdLav As String = "Report_OrdVend_OrdLav"

    'Report Acquisti
    Public Const Report_Ordini_Acquisto As String = "Report_Ordini_Acquisto"
    Public Const Report_Acquisti As String = "Report_Acquisti"
    Public Const Report_Pdf_Acquisto As String = "Report_Pdf_Acquisto"

    Private _gestioneGruppiMerce As Boolean
    Private _listaImprese As List(Of String) = New List(Of String)()

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Leggi oggetto database
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Private Function Leggi_Oggetto_Database(ByVal Nome As String, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Statistiche.Leggi_Tabella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Append(" SELECT * FROM sysobjects ")
            If Not String.IsNullOrEmpty(Nome) Then
                StrSQL.Append("WHERE name = '" & Nome & "' ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
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
    ''' Controlla esistenza tabella/vista report
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Esiste_Report(ByVal Nome As String, ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim dt = Leggi_Oggetto_Database(Nome, objParametri)
        Return dt.Rows.Count <> 0

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge parametri qualitativi
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi_Parametri_Qualitativi(ByVal piva As String, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim objConfigDettagli As AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R = New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
        Dim DTParamQual = objConfigDettagli.Leggi(piva, 0, False, "Tipo IN (1,3,4,5) AND Tabella_Key NOT IN ('cliente','fornitore')", "", objParametri)
        Return DTParamQual

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Crea query movimenti di vendita per report statistico
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi_Query_Report_Vendite(
            ByVal piva As String,
            ByVal report As String,
            ByVal tipoValore As String,
            ByVal tipoValore2 As String,
            ByVal selectPrincipale As String,
            ByRef objParametri As AgronicaCoreParametri
            ) As String

        Dim DTParamQual = Leggi_Parametri_Qualitativi(piva, objParametri)

        Dim StrSQL As New System.Text.StringBuilder

        Dim arrLavCodTabCTE As Integer() = New Integer() {}
        Dim cauMovTabCTE As Integer
        If report = Report_Ordini_Vendita OrElse report = Report_Vendite Then
            arrLavCodTabCTE = New Integer() {LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA, LAVCOD_DAA_EMESSO, LAVCOD_MVV_EMESSO}

            cauMovTabCTE = CAU_SCARICO
        End If
        If report = Report_Ordini_Acquisto OrElse report = Report_Acquisti Then
            'Nota: non esiste un corrispettivo di acquisto di LAVCOD_DAA_EMESSO
            arrLavCodTabCTE = New Integer() {LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_MVV_RICEVUTO}

            cauMovTabCTE = CAU_CARICO
        End If

        If Not IsNothing(report) Then

            If report = Report_Acquisti OrElse report = Report_Ordini_Acquisto Then
                '----------------------------------------------------------------
                '--- Filtro associato all'utente 
                '----------------------------------------------------------------
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim UtenteProfiloImpreseSql As String = ""

                Dim DtImpreseVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)
                If Not IsNothing(DtImpreseVisibili) AndAlso DtImpreseVisibili.Rows.Count > 0 Then
                    For Each dr In DtImpreseVisibili.Rows
                        UtenteProfiloImpreseSql &= "'" & dr.Item("Piva") & "',"
                        _listaImprese.Add(dr.Item("Piva"))
                    Next
                End If
            Else
                _listaImprese.Add(piva)
            End If

        Else
            _listaImprese.Add(piva)
        End If

        'Calcolo la gestione dei gruppi merce
        Dim objGruppiMerce As New AgronicaCoreAnagrafeDAL.Gruppi_Merce_R
        Dim ListaGruppiMercePerCategoria = objGruppiMerce.GetListGruppiMerceDefault_MultiAzienda(_listaImprese, objParametri)

        If Not IsNothing(ListaGruppiMercePerCategoria) AndAlso ListaGruppiMercePerCategoria.Count > 0 Then
            _gestioneGruppiMerce = True

            If report <> Report_Pdf_Acquisto AndAlso report <> Report_Pdf_Vendita Then
                Dim sqlCreaTempGruppiMerce As StringBuilder = objGruppiMerce.ComponiSql_CreaTempDefaultGruppiMerce(_listaImprese, objParametri, ListaGruppiMercePerCategoria)

                StrSQL.AppendLine(sqlCreaTempGruppiMerce.ToString() & ";")
                StrSQL.AppendLine("")
            End If
        End If

        '<GBEL> 2020-07-30 Tabelle CTE per date consegna e ultima consegna 
        If report = Report_Ordini_Vendita OrElse report = Report_Vendite OrElse report = Report_Ordini_Acquisto OrElse report = Report_Acquisti Then
            StrSQL.AppendLine(" With Spedizioni_CTE( ")
            StrSQL.AppendLine(" piva, id_agenda, Lav_Cod, des_lib, ")
            StrSQL.AppendLine(" Cau_Mov, Cod_RisUm, Id_Mov, Id_Mov_Det, Mat_Cod, Mov_Det_Des, Pro_Cod, Elem_Cod, Data_Movimento) ")
            StrSQL.AppendLine(" AS ")
            StrSQL.AppendLine(" (   ")
            StrSQL.AppendLine(" Select a.PIVA, a.id_agenda, a.Lav_Cod, a.des_lib, ")
            StrSQL.AppendLine(" mov.Cau_Mov, mov.Cod_RisUm, mov_det.Id_Mov, mov_det.Id_Mov_Det, ")
            StrSQL.AppendLine(" mov_det.Mat_Cod, mov_det.Mov_Det_Des, mov_det.Pro_Cod, mov_det.Elem_Cod, ")
            StrSQL.AppendLine(" mov.Data_Movimento")
            StrSQL.AppendLine(" From agenda a ")
            StrSQL.AppendLine(" inner Join movimenti mov ")
            StrSQL.AppendLine(" On a.PIVA = mov.PIVA And a.Id_Agenda = mov.Id_Agenda And a.Lav_Cod in (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodTabCTE)) & ") ")
            StrSQL.AppendLine(" inner Join movimenti_dettagli mov_det ")
            StrSQL.AppendLine(" On mov.PIVA = mov_det.PIVA And mov.Id_Agenda = mov_det.Id_Agenda And mov.Cau_Mov = '" & cauMovTabCTE & "' ")
            If report = Report_Ordini_Vendita OrElse report = Report_Vendite Then
                StrSQL.AppendLine(" where a.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            Else
                'TODO
            End If
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" ,  ")


            StrSQL.AppendLine(" Consegna_CTE(piva, id_agenda, id_agenda_rif, Id_Mov_Rif, Id_Mov_Det_Rif, Data_Movimento) ")
            StrSQL.AppendLine(" as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select mdr.piva, mdr.id_agenda,  mdr.id_agenda_rif, mdr.Id_Mov_Rif, mdr.Id_Mov_Det_Rif, Data_Movimento ")
            StrSQL.AppendLine(" From Mov_Dettagli_Riferimenti  mdr ")
            StrSQL.AppendLine(" inner Join Movimenti mov ")
            StrSQL.AppendLine(" On mdr.Piva = mov.PIVA And mdr.Id_Agenda = mov.Id_Agenda And mov.id_mov = mdr.Id_Mov ")
            StrSQL.AppendLine(" And mdr.Lav_Cod in (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodTabCTE)) & ")  And mdr.Cau_Mov = '" & cauMovTabCTE & "' ")
            If report = Report_Ordini_Vendita OrElse report = Report_Vendite Then
                StrSQL.AppendLine(" where mdr.piva = '" & Agro_SQL_SaveText(piva) & "' ")
            Else
                'TODO
            End If

            StrSQL.AppendLine(" ) ")
        End If


        ' Dati Principali
        StrSQL.AppendLine(selectPrincipale)
        'StrSQL.AppendLine(" ( ")
        StrSQL.AppendLine(" SELECT ag.PIVA,ag.Id_Agenda,mov_det.Id_Mov,mov_det.Id_Mov_Det,ag.Lav_Cod,ag.Tipo_Accettazione,mov_det.Sa_Cod,Centri_Aziendali.Sa_Nome,ag.Des_Lib,ag.Modulo,movNrDDT.Causale_Trasporto,movNrDDT.Causale_Trasporto_Cod,movNrDDT.Scadenza,movNrDDT.Extra_Int,mov_det.Contabilizzato, ")
        StrSQL.AppendLine(" mov_det.Anno AS Anno_Contabile, ")

        If report = Report_DDTVend_OrdLav OrElse
           report = Report_OrdVend_OrdLav Then
            StrSQL.AppendLine(" mov_det.Cal_Cod, CASE WHEN ISNULL(num_lav,0)>0 THEN 'SI' ELSE 'NO' END AS Lavorato, ")
        End If

        If report = Report_Ordini_Vendita OrElse report = Report_Vendite OrElse report = Report_Ordini_Acquisto OrElse report = Report_Acquisti Then
            '<GBEL> 2020-07-30 Tabelle CTE per date consegna e ultima consegna 
            StrSQL.AppendLine(" sped.Data_Ultima_Consegna as Data_Ultima_Consegna, ")
            StrSQL.AppendLine(" cons.Data_Ultima_Spedizione, ")
            StrSQL.AppendLine(" cons.Data_Prima_Spedizione, ")
        End If

        If report = Report_Ordini_Vendita OrElse
           report = Report_OrdVend_OrdLav Then
            StrSQL.AppendLine(" 'Ordine di vendita' AS Tipo_Documento, ")
        End If

        If report = Report_Vendite OrElse
           report = Report_Pdf_Vendita OrElse
           report = Report_DDTVend_OrdLav Then
            StrSQL.AppendLine(" CASE WHEN ag.Lav_Cod = 1001 THEN CASE WHEN movNrDDT.Extra_Int = 1 THEN 'Fattura accompagnatoria' ELSE 'Fattura' END ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 1020 THEN 'Corrispettivo di vendita' ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 1053 THEN 'Ricevuta fiscale' ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 1028 THEN 'Autoconsumo' ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 1069 THEN 'DDT Contabilizzato Emesso' ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 1031 THEN 'DDT non fatturato' ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 2002 THEN 'Ordine da evadere' ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 1071 THEN 'MVV non fatturato' ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 1003 THEN 'Nota Accredito Emessa' ")
            StrSQL.AppendLine(" ELSE '' END AS Tipo_Documento, ")
        End If

        If report = Report_Ordini_Acquisto Then
            StrSQL.AppendLine(" 'Ordine di acquisto' AS Tipo_Documento, ")
        End If

        If report = Report_Acquisti OrElse report = Report_Pdf_Acquisto Then
            StrSQL.AppendLine(" CASE WHEN ag.Lav_Cod = 1000 THEN CASE WHEN movNrDDT.Extra_Int = 1 THEN 'Fattura accompagnatoria' ELSE 'Fattura' END ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 1002 THEN 'Nota accredito ricevuta' ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 1025 THEN 'DDT ricevuto' ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 1054 THEN 'Accettazione DDT ricevuto' ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 1075 THEN 'Distinta di carico' ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 1076 THEN 'Distinta di carico da accettazione' ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 1077 THEN 'Auto DDT Emesso' ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 1078 THEN 'DDT emesso in accettazione' ")
            StrSQL.AppendLine(" WHEN ag.Lav_Cod = 2004 THEN 'Ordine di acquisto' ")
            StrSQL.AppendLine(" ELSE '' END AS Tipo_Documento, ")
        End If

        ' Dati Intestatario
        StrSQL.AppendLine(" ISNULL(r_u.Settore_Des, '') AS Soggetto_Codice,	")
        StrSQL.AppendLine(" ISNULL(cont.Cod_Contatto, '') AS Soggetto_Piva,	")
        StrSQL.AppendLine(" Case when ag.lav_cod = 1028 then 'Autoconsumo' ")
        StrSQL.AppendLine("      when ag.lav_cod = 1020 then 'Corrispettivo' ")
        StrSQL.AppendLine(" Else ")
        StrSQL.AppendLine(" Case ")
        StrSQL.AppendLine(" when cont.Cod_Contatto Is null then	'Corrispettivo' ")
        StrSQL.AppendLine(" when cont.Cod_Contatto = '' then 'Corrispettivo' ")
        StrSQL.AppendLine(" Else ISNULL(cont.Rag_Soc, '') + LTRIM(' ' + ISNULL(cont.Cognome, '') + ' ' + ISNULL(cont.Nome, ''))  ")
        StrSQL.AppendLine(" End End  As Soggetto_RagioneSociale,  ")
        StrSQL.AppendLine(" ISNULL(r_c.Rapporto_Des, '') as Soggetto_Rapporto,  ")
        StrSQL.AppendLine(" ISNULL(r_c.Cod_rapporto, 0) as Soggetto_Cod_Rapporto,  ")

        ' Dati Destinazione
        StrSQL.AppendLine(" ISNULL(r_u_destinazione.Settore_Des, '') AS Destinazione_Codice, ")

        StrSQL.AppendLine(" Case WHEN movNrDDT.Cod_RisUm = movNrDDT.Cod_destinazione THEN ")
        StrSQL.AppendLine(" Case when ISNULL( IndirizzoTipo_dest.Descrizione, '') = '' Then ")
        StrSQL.AppendLine(" Case WHEN ISNULL(indirizzi_dest.ind_des, '') = '' THEN  ISNULL(cont_destinazione.Rag_Soc, '') ELSE indirizzi_dest.ind_des END ")
        StrSQL.AppendLine(" Else IndirizzoTipo_dest.Descrizione End ")
        StrSQL.AppendLine(" Else  ISNULL(cont_destinazione.Rag_Soc, '') END ")
        StrSQL.AppendLine(" AS Destinazione_OLD, ")

        ' Nuova Destinazione
        'StrSQL.AppendLine(" Case when movNrDDT.Cod_destinazione <> 0 then ")
        'StrSQL.AppendLine(" ISNULL(cont_destinazione.Rag_Soc, '') + ' ' + ISNULL(cont_destinazione.Cognome, '') + ' ' + ISNULL(cont_destinazione.Nome, '')  + ' - ' + ")
        'StrSQL.AppendLine(" isnull(Indirizzi_dest.ind_des,'') + ' - ' + isnull( indirizzi_dest.cap,'') + ' ' + isnull( province_dest.SIGLA,'') + ' ' + isnull(province_dest.PROVINCIA,'') + ' (' + isnull(Indirizzi_dest.stato,'') + ')' ")
        'StrSQL.AppendLine(" Else  ")
        'StrSQL.AppendLine(" ISNULL(cont.Rag_Soc, '') + ' ' + ISNULL(cont.Cognome, '') + ' ' + ISNULL(cont.Nome, '')  + ' - ' + ")
        'StrSQL.AppendLine(" isnull(Indirizzi.ind_des,'') + ' - ' + isnull(indirizzi.cap,'') + ' ' + isnull(Lista_Province.SIGLA,'') + ' ' + isnull(Lista_Province.PROVINCIA,'') + ' (' + isnull(Indirizzi.stato,'') + ')' ")
        'StrSQL.AppendLine(" End As Destinazione, ")

        StrSQL.AppendLine(" Case when movNrDDT.Cod_destinazione <> 0 then  ")
        StrSQL.AppendLine(" Case when isnull(IndirizzoTipo_dest.Descrizione, '') = '' then ")
        StrSQL.AppendLine(" ISNULL(cont_destinazione.Rag_Soc, '') + ' ' + ISNULL(cont_destinazione.Cognome, '') + ' ' + ISNULL(cont_destinazione.Nome, '')  + ' - ' + ")
        StrSQL.AppendLine(" isnull(Indirizzi_dest.ind_des,'') + ' - ' + isnull( indirizzi_dest.cap,'') + ' ' + isnull( province_dest.SIGLA,'') + ' ' + isnull(province_dest.PROVINCIA,'') + ' (' + isnull(Indirizzi_dest.stato,'') + ')'  ")
        StrSQL.AppendLine(" Else ")
        StrSQL.AppendLine(" IndirizzoTipo_dest.Descrizione + ': ' + ")
        StrSQL.AppendLine(" ISNULL(cont_destinazione.Rag_Soc, '') + ' ' + ISNULL(cont_destinazione.Cognome, '') + ' ' + ISNULL(cont_destinazione.Nome, '')  + ' - ' + ")
        StrSQL.AppendLine(" isnull(Indirizzi_dest.ind_des,'') + ' - ' + isnull( indirizzi_dest.cap,'') + ' ' + isnull( province_dest.SIGLA,'') + ' ' + isnull(province_dest.PROVINCIA,'') + ' (' + isnull(Indirizzi_dest.stato,'') + ')' ")
        StrSQL.AppendLine(" End ")
        StrSQL.AppendLine(" Else ")
        StrSQL.AppendLine(" case when movNrDDT.Cod_RisUm = 0 and movNrDDT.Cod_Destinazione = 0 then	'' else")
        StrSQL.AppendLine(" ISNULL(cont.Rag_Soc, '') + ' ' + ISNULL(cont.Cognome, '') + ' ' + ISNULL(cont.Nome, '')  + ' - ' + ")
        StrSQL.AppendLine(" isnull(Indirizzi.ind_des,'') + ' - ' + isnull(indirizzi.cap,'') + ' ' + isnull(Lista_Province.SIGLA,'') + ' ' + isnull(Lista_Province.PROVINCIA,'') + ' (' + isnull(Indirizzi.stato,'') + ')'  ")
        StrSQL.AppendLine(" end ")
        StrSQL.AppendLine(" End As Destinazione, ")

        ' Dati Documento
        StrSQL.AppendLine(" movNrDDT.Data_Movimento, ")
        StrSQL.AppendLine(" movNrDDT.Doc_Numero_Sin + ")
        StrSQL.AppendLine("   CASE WHEN LEN(LTRIM(STR(movNrDDT.doc_numero,10))) > 5 THEN LTRIM(STR(movNrDDT.doc_numero,10)) ")
        StrSQL.AppendLine("   ELSE REPLICATE('0', 5 - LEN(LTRIM(STR(movNrDDT.doc_numero,10)))) + LTRIM(STR(movNrDDT.doc_numero,10))  END ")
        StrSQL.AppendLine("   + movNrDDT.Doc_Numero_Des AS Numero_Movimento, ")

        StrSQL.AppendLine(" CONVERT(varchar, movNrDDT.Data_Movimento, 103) AS Data_Documento, movNrDDT.Doc_Numero_Sin, movNrDDT.Doc_Numero, movNrDDT.Doc_Numero_Des, mov_det.Ordine_Det AS Riga, ")
        StrSQL.AppendLine(" YEAR(movNrDDT.Data_Movimento) AS Anno_Movimento, SUBSTRING(CONVERT(varchar,movNrDDT.Data_Movimento,120),1,7) AS Mese_Movimento ")

        'Settimana Documenti
        StrSQL.AppendLine(" , case when movNrDDT.Data_Movimento Is null then 0 ")
        StrSQL.AppendLine(" Else datename(ww, movNrDDT.Data_Movimento) ")
        StrSQL.AppendLine(" End As Settimana_Documento, ")

        ' Mese Senza Anno
        StrSQL.AppendLine(" REPLACE(REPLACE(SUBSTRING(CONVERT(varchar,movNrDDT.Data_Movimento,120),1,7), YEAR(movNrDDT.Data_Movimento) , ''), '-', '') As MeseNoAnno, ")

        If report = Report_Acquisti Then
            'Dati Documento registrazione extra
            StrSQL.AppendLine(" Case when movNrBolla.Data_Movimento Is NULL then '' ")
            StrSQL.AppendLine(" Else CONVERT(varchar,movNrBolla.Data_Movimento ,120) End As Data_Movimento_Bolla, ")
            StrSQL.AppendLine("       Case when movNrBolla.Doc_Numero Is NULL then '' ")
            StrSQL.AppendLine(" Else movNrBolla.Doc_Numero_Sin + ")
            StrSQL.AppendLine(" Case WHEN LEN(LTRIM(STR(movNrBolla.doc_numero,10))) > 5 THEN LTRIM(STR(movNrBolla.doc_numero,10)) ")
            StrSQL.AppendLine(" Else REPLICATE('0', 5 - LEN(LTRIM(STR(movNrBolla.doc_numero,10)))) + LTRIM(STR(movNrBolla.doc_numero,10))  END ")
            StrSQL.AppendLine(" + movNrBolla.Doc_Numero_Des end AS Numero_Movimento_Bolla, ")
            StrSQL.AppendLine(" ISNULL(movNrBolla.Doc_Numero_Sin, '') AS Doc_Numero_Sin_Bolla, ")
            StrSQL.AppendLine(" ISNULL(movNrBolla.Doc_Numero, 0) AS Doc_Numero_Bolla, ")
            StrSQL.AppendLine(" ISNULL(movNrBolla.Doc_Numero_Des, '') AS Doc_Numero_Des_Bolla, ")
        End If

        ' Dati Vettore
        StrSQL.AppendLine(" ISNULL(r_u_vettore.Settore_Des, '') AS Vettore_Codice, ISNULL(cont_vettore.Rag_Soc, '') + LTRIM(' ' + ISNULL(cont_vettore.Cognome, '') + ' ' + ISNULL(cont_vettore.Nome, '')) AS Vettore_RagioneSociale, ISNULL(Movimento_Extra_0.Targa, '') AS TargaMezzoVettore, ")

        ' Dati Agente
        StrSQL.AppendLine(" ISNULL(r_u_agente.Settore_Des, '') AS Agente_Codice, ISNULL(cont_agente.Cod_Contatto, '') AS Agente_Piva, ISNULL(cont_agente.Rag_Soc, '') + LTRIM(' ' + ISNULL(cont_agente.Cognome, '') + ' ' + ISNULL(cont_agente.Nome, ''))  AS Agente_RagioneSociale, ")

        ' Dati Capo Area
        StrSQL.AppendLine(" ISNULL(r_u_capoarea.Settore_Des, '') AS Capoarea_Codice, ISNULL(cont_capoarea.Rag_Soc, '') + LTRIM(' ' + ISNULL(cont_capoarea.Cognome, '') + ' ' + ISNULL(cont_capoarea.Nome, ''))  AS Capoarea_RagioneSociale, ")

        ' Dati Materie Prime
        StrSQL.AppendLine(" mat_prima.Mat_Cod AS Referenza_Codice, ")

        StrSQL.AppendLine(" (CASE WHEN ISNULL(mat_prima.Cod_articolo,'') = '' THEN ISNULL(mat_prima.Mat_Des,mov_det.Mov_Det_Des) ELSE ")
        StrSQL.AppendLine(" ISNULL(mat_prima.Mat_Des,mov_det.Mov_Det_Des) + ' (' + mat_prima.Cod_articolo + ')' END)     AS Referenza_Descr,")

        StrSQL.AppendLine(" REPLACE( ")
        StrSQL.AppendLine("   CASE WHEN movNrDDT.Extra_Str IS NOT NULL AND movNrDDT.Extra_Str <> '' THEN")
        StrSQL.AppendLine("   movNrDDT.Extra_Str ELSE")
        StrSQL.AppendLine("   movNrDDT.Mov_Desc END,")
        StrSQL.AppendLine(" '§', ' ') AS Note, ")
        StrSQL.AppendLine(" REPLACE(CASE WHEN mov_det.Elem_Cod = 555 THEN mov_det.mov_det_des ELSE mov_det.Extra_Str END,'§',' ') AS Note_Prodotto, ")
        StrSQL.AppendLine(" REPLACE(ISNULL(mov.Extra_Str,''), '§',' ') AS Note_Documento, ")
        StrSQL.AppendLine(" mov_det.Elem_Cod, mov_det.Pro_Cod, mov_det.Mat_Cod, mat_prima.Veg_Cod, mat_prima.Cul_Cod, mat_prima.Cat_Cod, ")
        StrSQL.AppendLine(" ISNULL(cat_mag.NomeComune,'') AS Categoria_Prodotto, ISNULL(cat_com.Linea_Classe_Des,'') AS Categoria_Commerciale, ")

        If _gestioneGruppiMerce Then
            StrSQL.AppendLine(" COALESCE(prodExtraPriv.Id_Gruppo_Merce, #DefaultGruppiMerce.Id_Gruppo_Merce, 0) AS Id_Gruppo_Merce ")
            StrSQL.AppendLine(" ,CASE ")
            StrSQL.AppendLine(" WHEN grpMerce.Codice IS NOT NULL THEN grpMerce.Codice + ' ' + grpMerce.Descrizione ")
            StrSQL.AppendLine(" WHEN #DefaultGruppiMerce.Elem_Cod IS NOT NULL THEN #DefaultGruppiMerce.Codice + ' ' + #DefaultGruppiMerce.Descrizione COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" ELSE '' END AS Gruppo_Merce, ")
        End If

        ' Dati Lotto, UM
        StrSQL.AppendLine(" mov_det.Lotto, mov_det.UDM_COD AS Unita_Misura, unitamisura.udm_sim AS Unita_Misura_Sigla, unitamisura.udm_des AS Unita_Misura_Des, ")
        StrSQL.AppendLine(" mov_det.UDM_COD_EXTRA AS Unita_Misura_Secondaria, udm_sec.udm_sim AS Unita_Misura_Secondaria_Sigla, udm_sec.udm_des AS Unita_Misura_Secondaria_Des, ")

        Dim lavCodRestiAbbuoni As Integer

        If report = Report_Ordini_Vendita OrElse
           report = Report_Vendite OrElse
           report = Report_Pdf_Vendita OrElse
           report = Report_DDTVend_OrdLav OrElse
           report = Report_OrdVend_OrdLav Then
            lavCodRestiAbbuoni = LAVCOD_NOTA_ACCREDITO_EMESSA
        End If

        If report = Report_Ordini_Acquisto OrElse
           report = Report_Acquisti OrElse
           report = Report_Pdf_Acquisto Then
            lavCodRestiAbbuoni = LAVCOD_NOTA_ACCREDITO_RICEVUTA
        End If

        ' Dati Confezionamento
        StrSQL.AppendLine(" (CASE WHEN ag.Lav_Cod = " & lavCodRestiAbbuoni & " THEN -1 ELSE 1 END) * CASE WHEN mov_det.Udm_Cod NOT IN (0,2) THEN ISNULL(mov_det_rif.Qta,mov_det.Qta) ELSE 0 END AS Nr_Confezioni, ")
        StrSQL.AppendLine(" (CASE WHEN ag.Lav_Cod = " & lavCodRestiAbbuoni & " THEN -1 ELSE 1 END) * ISNULL(mov_det_rif.Qta_Dettaglio1, mov_det.Qta_Dettaglio1) AS Nr_Contenitori, ")
        StrSQL.AppendLine(" (CASE WHEN ag.Lav_Cod = " & lavCodRestiAbbuoni & " THEN -1 ELSE 1 END) * ISNULL(mov_det_rif.Qta_Dettaglio2, mov_det.Qta_Dettaglio2) AS Nr_Imballi, ")
        StrSQL.AppendLine(" ISNULL(Movimento_Extra_Dettagli.des_contenitori, '') as Tipo_Contenitore, ")
        StrSQL.AppendLine(" ISNULL(Movimento_Extra_Dettagli.marche_contenitori, '') As Tipo_Imballo, ")

        StrSQL.AppendLine(Componi_Query_Su_Tipo_Valore(report, tipoValore, "Qta"))

        StrSQL.AppendLine(" mov_det.Qta_Extra,")
        StrSQL.AppendLine(" (Case When ag.Lav_Cod = " & lavCodRestiAbbuoni & " Then -1 Else 1 End) * ISNULL(mov_det_rif.Qta_Extra_Totale, mov_det.Qta_Extra_Totale) As Kg_Netti, ")
        StrSQL.AppendLine(" (Case When ag.Lav_Cod = " & lavCodRestiAbbuoni & " Then -1 Else 1 End) * ISNULL(mov_det_rif.Tara, mov_det.Tara) As Tara_Totale, ")
        StrSQL.AppendLine(" (Case When ag.Lav_Cod = " & lavCodRestiAbbuoni & " Then -1 Else 1 End) * (ISNULL(mov_det_rif.Qta_Extra_Totale, mov_det.Qta_Extra_Totale) + ISNULL(mov_det_rif.Tara, mov_det.Tara)) As Kg_Lordi, ")

        ' Quantità Evase / Residue per Ordini Vendita
        If report = Report_Ordini_Vendita OrElse report = Report_Ordini_Acquisto Then
            StrSQL.AppendLine(" ISNULL(mov_det_rif.Qta_Evasa, 0) As Qta_Evasa, Case When mov_det_rif.Qta_Residua Is Not NULL And mov_det.Contabilizzato In (3,-3) Then 0 Else ISNULL(mov_det_rif.Qta_Residua, mov_det.Qta) End As Qta_Residua, ")
            StrSQL.AppendLine(" CASE WHEN mat_prima.Elem_Cod = " & RIGA_DESCRIZIONE_LIBERA & " THEN '' ")
            StrSQL.AppendLine("   WHEN mov_det.Contabilizzato In (3,-3) THEN 'Evaso Forzatamente' ")
            StrSQL.AppendLine("   WHEN ISNULL(mov_det_rif.Qta_Evasa, 0) = 0 THEN 'Non Evaso' ")
            StrSQL.AppendLine("   WHEN ISNULL(mov_det_rif.Qta_Evasa, 0) < mov_det.Qta THEN 'Evaso Parzialmente' ")
            StrSQL.AppendLine("   WHEN ISNULL(mov_det_rif.Qta_Evasa, 0) >= mov_det.Qta THEN 'Evaso' ")
            StrSQL.AppendLine(" END AS StatoEvasione_Des, ")
            StrSQL.AppendLine(" ISNULL(mov_det_rif.Qta_Netta_Evasa, 0) As Qta_Netta_Evasa, Case When mov_det_rif.Qta_Residua Is Not NULL And mov_det.Contabilizzato In (3,-3) Then 0 Else ISNULL(mov_det_rif.Qta_Netta_Residua, 0) End As Qta_Netta_Residua, ")
            StrSQL.AppendLine(" ISNULL(mov_det_rif.Qta_Tara_Evasa, 0) As Qta_Tara_Evasa, Case When mov_det_rif.Qta_Residua Is Not NULL And mov_det.Contabilizzato In (3,-3) Then 0 Else ISNULL(mov_det_rif.Qta_Tara_Residua, 0) End As Qta_Tara_Residua, ")
            StrSQL.AppendLine(" ISNULL(mov_det_rif.Qta_Lorda_Evasa, 0) As Qta_Lorda_Evasa, Case When mov_det_rif.Qta_Residua Is Not NULL And mov_det.Contabilizzato In (3,-3) Then 0 Else ISNULL(mov_det_rif.Qta_Lorda_Residua, 0) End As Qta_Lorda_Residua, ")
            StrSQL.AppendLine(" ISNULL(mov_det_rif.Qta_Contenitori_Evasa, 0) As Qta_Contenitori_Evasa, Case When mov_det_rif.Qta_Residua Is Not NULL And mov_det.Contabilizzato In (3,-3) Then 0 Else ISNULL(mov_det_rif.Qta_Contenitori_Residua, 0) End As Qta_Contenitori_Residua, ")
            StrSQL.AppendLine(" ISNULL(mov_det_rif.Qta_Imballi_Evasa, 0) As Qta_Imballi_Evasa, Case When mov_det_rif.Qta_Residua Is Not NULL And mov_det.Contabilizzato In (3,-3) Then 0 Else ISNULL(mov_det_rif.Qta_Imballi_Residua, 0) End As Qta_Imballi_Residua, ")
        End If

        ' Data Evasione Prevista / Data Spedizione Prevista
        If report = Report_Ordini_Vendita OrElse
           report = Report_Vendite OrElse
           report = Report_Ordini_Acquisto OrElse
           report = Report_Acquisti Then
            Dim lavCodOrdine As Integer
            If report = Report_Ordini_Vendita OrElse report = Report_Vendite Then
                lavCodOrdine = LAVCOD_ORDINE_VENDITA
            End If
            If report = Report_Ordini_Acquisto OrElse report = Report_Acquisti Then
                lavCodOrdine = LAVCOD_ORDINE_ACQUISTO
            End If

            StrSQL.AppendLine(" Case when ag.lav_cod = " & lavCodOrdine & " then ")
            StrSQL.AppendLine(" Case when movNrDDT.Ora =  CONVERT(DateTime,'2100/12/31',120) then null else movNrDDT.Ora end ")
            StrSQL.AppendLine(" Else null end as Data_Spedizione_Prevista, ")
            StrSQL.AppendLine(" Case when ag.lav_cod = " & lavCodOrdine & " then  ")
            StrSQL.AppendLine(" Case when movNrDDT.Extra_Date =  CONVERT(DateTime,'2100/12/31',120) then null else movNrDDT.Extra_Date end ")
            StrSQL.AppendLine(" Else null end as Data_Evasione_Prevista, ")
        End If

        ' Dati Prezzo Netto / Imponibile / Provvigione
        'Emesso → imponibile (prezzo*quantità) positivo e iva negativa
        'Acquisto → imponibile negativo e iva positiva
        Dim impNegativoSuDB As Integer 'segnoImponibile
        Dim ivaNegativaSuDB As Integer 'segnoIva
        If report = Report_Ordini_Vendita OrElse
           report = Report_Vendite OrElse
           report = Report_Pdf_Vendita OrElse
           report = Report_DDTVend_OrdLav OrElse
           report = Report_OrdVend_OrdLav Then
            impNegativoSuDB = 1
            ivaNegativaSuDB = -1
        End If
        If report = Report_Ordini_Acquisto OrElse
           report = Report_Acquisti OrElse
           report = Report_Pdf_Acquisto Then
            impNegativoSuDB = -1
            ivaNegativaSuDB = 1
        End If

        StrSQL.AppendLine(" mov_det.Prezzo_Unitario_Netto As Prezzo_Netto, ")
        StrSQL.AppendLine(" Case When mov_det.prezzo_livello = 4 Then 'Imballo' WHEN mov_det.prezzo_livello = 8 THEN 'Contenitore' WHEN mov_det.prezzo_livello = 5 THEN 'Confezione' ELSE 'KG' END AS Prezzo_Riferito_A, ")
        StrSQL.AppendLine(" mov_det.sconto * -1 as Sconto_Perc, ")
        StrSQL.AppendLine(" ROUND(ISNULL(mov_det_rif.Imponibile, mov_det.Imponibile) - ISNULL(mov_det_rif.Imponibile_Netto, mov_det.Imponibile_Netto),4) * " & impNegativoSuDB & "  as Sconto, ")

        StrSQL.AppendLine(" Case when mov_det.Sconto_Modalita = 0 then 'Sconto % su Prezzo Unitario' ")
        StrSQL.AppendLine(" when mov_det.Sconto_Modalita = 1 then 'Sconto Merce' ")
        StrSQL.AppendLine(" when mov_det.Sconto_Modalita = 2 then 'Campioni Omaggio Senza Rivalsa IVA' ")
        StrSQL.AppendLine(" when mov_det.Sconto_Modalita = 3 then 'Campioni Gratuiti' ")
        StrSQL.AppendLine(" when mov_det.Sconto_Modalita = 4 then 'Campioni Omaggio Con Rivalsa IVA' end as Sconto_Modalita, ")

        StrSQL.AppendLine(" Case when mov_det.Sconto_Modalita = 2 then 0 ")
        StrSQL.AppendLine(" when mov_det.Sconto_Modalita = 4 then 0 ")
        StrSQL.AppendLine(" Else ISNULL(mov_det_rif.Imponibile_Netto, mov_det.Imponibile_Netto) * " & impNegativoSuDB & " End As Imponibile_Netto, ")

        StrSQL.AppendLine(" Case when mov_det.Sconto_Modalita = 2 then 0 ")
        StrSQL.AppendLine(" when mov_det.Sconto_Modalita = 4 then 0 ")
        StrSQL.AppendLine(" Else ISNULL(mov_det_rif.Imponibile, mov_det.Imponibile) * " & impNegativoSuDB & " End As Imponibile,  ")

        StrSQL.AppendLine(" Case when mov_det.Sconto_Modalita = 2 then 0 ")
        StrSQL.AppendLine(" Else ISNULL(mov_det_rif.Iva, mov_det.Iva) * " & ivaNegativaSuDB & " End As Iva,  ")

        StrSQL.AppendLine(" ISNULL(mov_det_rif.Imponibile_Netto, mov_det.Imponibile_Netto) * " & impNegativoSuDB & " + ISNULL(mov_det_rif.Iva, mov_det.Iva) * " & ivaNegativaSuDB & " as Importo, ")
        StrSQL.AppendLine(" ISNULL(Movimento_Extra_Dettagli.Provvigione, 0) AS Provvigione, ")
        StrSQL.AppendLine(" ( (ISNULL(mov_det_rif.Imponibile_Netto, mov_det.Imponibile_Netto) * " & impNegativoSuDB & ") / 100) * ISNULL(Movimento_Extra_Dettagli.Provvigione, 0) as Provvigione_Calcolata, ")

        ' Dati CAP, comune, provincia, regione, stato
        StrSQL.AppendLine(" ISNULL(Indirizzi.CAP, '') AS CAP, ISNULL(Indirizzi.com_des, '') AS Comune, ISNULL(Indirizzi.pro_cod, '') AS Sigla, ")
        StrSQL.AppendLine(" ISNULL(Lista_Province.PROVINCIA, '') AS Provincia, ISNULL(Lista_Regioni.Regione_Des, '') AS Regione, ")

        StrSQL.AppendLine(" Case when ag.lav_cod IN (" & LAVCOD_VENDITA & "," & LAVCOD_AUTOCONSUMO & ") Then ")
        StrSQL.AppendLine(" 'Italia' else ISNULL(Lista_Nazioni.Descrizione,ISNULL(Indirizzi.stato, '')) end AS Stato, ")

        ' Dati CAP, comune, provincia, regione, stato destinazione
        StrSQL.AppendLine(" ISNULL(indirizzi_dest.CAP, '') AS CAP_Dest, ")
        StrSQL.AppendLine(" CASE WHEN ISNULL(indirizzi_dest.com_des, '') = '' THEN ISNULL(indirizzi_dest.frz_des, '') ELSE ISNULL(indirizzi_dest.com_des, '') END AS Comune_Dest, ")
        StrSQL.AppendLine(" ISNULL(indirizzi_dest.pro_cod, '') AS Sigla_Dest, ")
        StrSQL.AppendLine(" ISNULL(province_dest.PROVINCIA, '') AS Provincia_Dest, ISNULL(regioni_dest.Regione_Des, '') AS Regione_Dest, ")
        StrSQL.AppendLine(" ISNULL(nazioni_dest.Descrizione, ISNULL(indirizzi_dest.stato, '')) AS Stato_Dest , ")
        StrSQL.AppendLine(" ISNULL(nazioni_dest.Codice, '') AS Codice_Stato_Dest , ")

        'Stato fatturazione
        StrSQL.AppendLine(" Case when ag.lav_cod IN (" & LAVCOD_VENDITA & "," & LAVCOD_AUTOCONSUMO & ") Then ")
        StrSQL.AppendLine(" 'Italia' else ISNULL(nazioni_fatt.Descrizione, ISNULL(Indirizzi.stato, '')) end AS Stato_Fatt , ")
        StrSQL.AppendLine(" Case when ag.lav_cod IN (" & LAVCOD_VENDITA & "," & LAVCOD_AUTOCONSUMO & ") Then ")
        StrSQL.AppendLine(" 'IT' else ISNULL(nazioni_fatt.Codice, '') end AS Codice_Stato_Fatt, ")

        ' Ragione Sociale Impresa
        StrSQL.AppendLine(" i.Rag_Soc as Ragione_Sociale_Impresa, i.Piva as Piva_Impresa ")

        StrSQL.AppendLine(" , CASE WHEN RicXConti.Id_Riclassificazione IS NULL THEN '' ELSE RicXConti.Id_Riclassificazione + ' - ' + Conti.Conto_Descr END AS Conto_Economico_Descr")

        ' Parametri Qualitativi
        If DTParamQual IsNot Nothing Then
            For Each paramQual In DTParamQual.Rows
                If paramQual("Tipo") = 1 Then
                    StrSQL.AppendLine(" , ISNULL(OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Sigla, '') AS " & paramQual("Tabella_Key") & "_Sigla  ")
                    StrSQL.AppendLine(" , ISNULL(OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Descrizione, '') AS " & paramQual("Tabella_Key") & "_Descrizione ")
                End If
                If paramQual("Tipo") = 3 Then
                    StrSQL.AppendLine(" , ISNULL(Convert(float,Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod), '') AS " & paramQual("Tabella_Key") & "_Val_Cod  ")
                End If
                If paramQual("Tipo") = 4 Then
                    StrSQL.AppendLine(" , ISNULL(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod, '') AS " & paramQual("Tabella_Key") & "_Val_Cod  ")
                End If
                If paramQual("Tipo") = 5 Then
                    StrSQL.AppendLine(" , Case  when ISNULL(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod, '') = '' THEN null ")
                    StrSQL.AppendLine("         when ISNULL(Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod, '') = '0' THEN null ")
                    StrSQL.AppendLine("        Else Convert(DateTime, Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Val_Cod , 120)   ")
                    StrSQL.AppendLine(" End  ")
                    StrSQL.AppendLine(" AS " & paramQual("Tabella_Key") & "_Val_Cod")
                End If
            Next
        End If

        If report = Report_Acquisti Then

            'Degrado %
            StrSQL.AppendLine(" , ISNULL(mov_det.Variazione,0) as Degrado_Perc  ")

            'Degrado
            StrSQL.AppendLine(" ,CASE WHEN mov_det.Variazione Is NULL THEN 0 ")
            StrSQL.AppendLine(" WHEN mov_det.Variazione = 0 THEN 0 ")
            StrSQL.AppendLine(" Else Convert(Int, ROUND(ISNULL(mov_det.Qta_Extra_Totale, 0) / 100 * mov_det.Variazione, 0)) ")
            StrSQL.AppendLine(" End As Degrado  ")

            'Netto a Pagamento
            StrSQL.AppendLine(" , CASE WHEN mov_det.Variazione Is NULL THEN Convert(Int, ISNULL(mov_det.Qta_Extra_Totale, 0)) ")
            StrSQL.AppendLine(" WHEN mov_det.Variazione = 0 THEN Convert(Int, ISNULL(mov_det.Qta_Extra_Totale, 0)) ")
            StrSQL.AppendLine(" Else Convert(Int, ISNULL(mov_det.Qta_Extra_Totale, 0)) - convert(int, round((isnull(mov_det.Qta_Extra_Totale,0) / 100 * mov_det.Variazione),0)) ")
            StrSQL.AppendLine(" End As Netto_Pagamento  ")

            ' Desc_appezzamenti
            StrSQL.AppendLine(" , COALESCE(mdc.Desc_Appezzamenti, '') AS Desc_Appezzamenti ")

            'Desc Varietà
            StrSQL.AppendLine(" , coalesce(cvp.Desc_Varieta, '') as Desc_Varieta")

        End If

        If report = Report_Vendite OrElse
           report = Report_Acquisti OrElse
           report = Report_Pdf_Vendita OrElse
           report = Report_Pdf_Acquisto OrElse
           report = Report_DDTVend_OrdLav Then

            StrSQL.AppendLine(" , CASE WHEN Movimenti_Ordini.Id_Mov IS NOT NULL THEN ")
            StrSQL.AppendLine("     Movimenti_Ordini.Doc_Numero_Sin + ")
            StrSQL.AppendLine("     CASE WHEN LEN(LTRIM(STR(Movimenti_Ordini.doc_numero,10))) > 5 THEN LTRIM(STR(Movimenti_Ordini.doc_numero,10)) ")
            StrSQL.AppendLine("     ELSE REPLICATE('0', 5 - LEN(LTRIM(STR(Movimenti_Ordini.doc_numero,10)))) + LTRIM(STR(Movimenti_Ordini.doc_numero,10))  END ")
            StrSQL.AppendLine("     + Movimenti_Ordini.Doc_Numero_Des ")
            StrSQL.AppendLine("   ELSE '' END AS Numero_Ordine ")
            StrSQL.AppendLine(" , CASE WHEN Movimenti_Ordini.Id_Mov IS NOT NULL THEN ")
            StrSQL.AppendLine("     CONVERT(varchar, Movimenti_Ordini.Data_Movimento, 103) ")
            StrSQL.AppendLine("   ELSE NULL END AS Data_Ordine ")
            StrSQL.AppendLine(" , ISNULL(Movimenti_Ordini.Doc_Numero_Sin, '') AS Doc_Numero_Sin_Ordine ")
            StrSQL.AppendLine(" , ISNULL(Movimenti_Ordini.Doc_Numero, '') AS Doc_Numero_Ordine ")
            StrSQL.AppendLine(" , ISNULL(Movimenti_Ordini.Doc_Numero_Des, '') AS Doc_Numero_Des_Ordine ")

        End If

        ' Join Testata
        StrSQL.AppendLine(" FROM Agenda As ag With (nolock) ")
        StrSQL.AppendLine(" INNER JOIN Movimenti As mov With (nolock) On ag.PIVA = mov.PIVA And ag.Id_Agenda = mov.Id_Agenda ")
        StrSQL.AppendLine(" INNER JOIN Movimenti As movNrDDT With (nolock) On ag.PIVA = movNrDDT.PIVA And ag.Id_Agenda = movNrDDT.Id_Agenda ")

        If report = Report_Acquisti Then
            StrSQL.AppendLine(" LEFT JOIN Movimenti AS movNrBolla WITH (nolock) ")
            StrSQL.AppendLine(" ON ag.PIVA = movNrBolla.PIVA AND ag.Id_Agenda = movNrBolla.Id_Agenda AND movNrBolla.Cau_Mov = '" & CAU_REGISTRAZIONI_ALLEGATE & "'")
        End If

        ' Join x Intestazione
        StrSQL.AppendLine(" LEFT JOIN Risorse_Umane As r_u With (nolock) On mov.Cod_RisUm = r_u.Cod_RisUm ")
        StrSQL.AppendLine(" LEFT JOIN Rapporti_Contabili As r_c With (nolock) On r_u.Cod_Rapporto = r_c.Cod_Rapporto ")
        StrSQL.AppendLine(" LEFT JOIN Contatti As cont With (nolock) On r_u.Piva = cont.Piva And r_u.Cod_Contatto = cont.Cod_Contatto ")

        ' Join x Dettagli e Qta Residue
        StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli As mov_det With (nolock) On ag.PIVA = mov_det.PIVA And mov.Id_Agenda = mov_det.Id_Agenda And mov.Id_Mov = mov_det.Id_Mov ")

        'Join Centri Aziendali
        StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali With (nolock) On mov_det.Piva = Centri_Aziendali.Piva And mov_det.Sa_Cod = Centri_Aziendali.Sa_Cod ")

        If report = Report_Ordini_Vendita OrElse
           report = Report_Ordini_Acquisto OrElse
           report = Report_OrdVend_OrdLav Then
            Dim listPairOrdiniMovDetRif As New List(Of KeyValuePair(Of Integer, Integer))()

            If report = Report_Ordini_Vendita OrElse
               report = Report_OrdVend_OrdLav Then
                listPairOrdiniMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_ORDINE_VENDITA, LAVCOD_FATTURA_EMESSA))
                listPairOrdiniMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_ORDINE_VENDITA, LAVCOD_BOLLA_EMESSA))
                listPairOrdiniMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_ORDINE_VENDITA, LAVCOD_MVV_EMESSO))
            Else
                listPairOrdiniMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_FATTURA_RICEVUTA))
                listPairOrdiniMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_BOLLA_RICEVUTA))
                listPairOrdiniMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_MVV_RICEVUTO))
            End If

            Dim listClausoleOrdiniMovDetRif As New List(Of String)
            For Each pairOrdiniMovDetRif In listPairOrdiniMovDetRif
                listClausoleOrdiniMovDetRif.Add(String.Format("Lav_Cod_Rif = {0} And Lav_Cod = {1}", pairOrdiniMovDetRif.Key, pairOrdiniMovDetRif.Value))
            Next
            Dim strClausoleOrdiniMovDetRif = "(" & String.Join(") Or (", listClausoleOrdiniMovDetRif) & ")"

            StrSQL.AppendLine(" LEFT JOIN (Select a.id_mov_det_rif, sum(c.qta) As Qta_Evasa, avg(b.qta) As Qta_Originale, avg(b.Qta)-sum(a.Qta) As Qta_Residua, ")
            StrSQL.AppendLine(" sum(c.Qta_Extra_Totale) As Qta_Netta_Evasa, avg(b.Qta_Extra_Totale)-sum(c.Qta_Extra_Totale) As Qta_Netta_Residua, ")
            StrSQL.AppendLine(" sum(c.Tara) As Qta_Tara_Evasa, avg(b.Tara)-sum(c.Tara) As Qta_Tara_Residua, ")
            StrSQL.AppendLine(" sum(c.Qta_Extra_Totale)+sum(c.Tara) As Qta_Lorda_Evasa, avg(b.Qta_Extra_Totale)-sum(c.Qta_Extra_Totale)+avg(b.Tara)-sum(c.Tara) As Qta_Lorda_Residua, ")
            StrSQL.AppendLine(" sum(c.Qta_Dettaglio1) As Qta_Contenitori_Evasa, avg(b.Qta_Dettaglio1)-sum(c.Qta_Dettaglio1) As Qta_Contenitori_Residua, ")
            StrSQL.AppendLine(" sum(c.Qta_Dettaglio2) As Qta_Imballi_Evasa, avg(b.Qta_Dettaglio2)-sum(c.Qta_Dettaglio2) As Qta_Imballi_Residua, ")
            StrSQL.AppendLine(" null As Qta, null As Qta_Extra_Totale, null As Tara, null As Qta_Dettaglio1, null As Qta_Dettaglio2, null As imponibile, null As imponibile_netto, null As iva ")
            StrSQL.AppendLine(" FROM mov_dettagli_riferimenti a ")
            StrSQL.AppendLine(" INNER JOIN movimenti_dettagli b On a.id_mov_det_rif = b.id_mov_det ")
            StrSQL.AppendLine(" INNER JOIN movimenti_dettagli c On a.id_mov_det = c.id_mov_det ")
            StrSQL.AppendLine(" WHERE " & strClausoleOrdiniMovDetRif)
            StrSQL.AppendLine(" GROUP BY a.id_mov_det_rif) As mov_det_rif On mov_det_rif.Id_Mov_Det_Rif = mov_det.Id_Mov_Det ")
        End If

        Dim strClausoleVenAcqMovDetRif As String = ""

        If report = Report_Vendite OrElse
           report = Report_Acquisti OrElse
           report = Report_Pdf_Vendita OrElse
           report = Report_Pdf_Acquisto OrElse
           report = Report_DDTVend_OrdLav Then
            Dim listPairVenAcqMovDetRif As New List(Of KeyValuePair(Of Integer, Integer))()

            If report = Report_Vendite OrElse
               report = Report_Pdf_Vendita OrElse
               report = Report_DDTVend_OrdLav Then
                listPairVenAcqMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_BOLLA_EMESSA, LAVCOD_FATTURA_EMESSA))
                listPairVenAcqMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_MVV_EMESSO, LAVCOD_FATTURA_EMESSA))
                listPairVenAcqMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_ORDINE_VENDITA, LAVCOD_FATTURA_EMESSA))
                listPairVenAcqMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_ORDINE_VENDITA, LAVCOD_BOLLA_EMESSA))
                listPairVenAcqMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_ORDINE_VENDITA, LAVCOD_MVV_EMESSO))
            Else
                listPairVenAcqMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA))
                listPairVenAcqMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_MVV_RICEVUTO, LAVCOD_FATTURA_RICEVUTA))
                listPairVenAcqMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_FATTURA_RICEVUTA))
                listPairVenAcqMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_BOLLA_RICEVUTA))
                listPairVenAcqMovDetRif.Add(New KeyValuePair(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_MVV_RICEVUTO))
            End If

            Dim listClausoleVenAcqMovDetRif As New List(Of String)
            For Each pairVenAcqMovDetRif In listPairVenAcqMovDetRif
                listClausoleVenAcqMovDetRif.Add(String.Format("Lav_Cod_Rif = {0} And Lav_Cod = {1}", pairVenAcqMovDetRif.Key, pairVenAcqMovDetRif.Value))
            Next
            strClausoleVenAcqMovDetRif = "(" & String.Join(") Or (", listClausoleVenAcqMovDetRif) & ")"

            StrSQL.AppendLine(" LEFT JOIN (Select a.id_mov_det_rif, sum(c.qta) As Qta_Evasa, avg(b.qta) As Qta_Originale, avg(b.Qta)-sum(a.Qta) As Qta_Residua, ")
            StrSQL.AppendLine(" avg(b.Qta)-sum(a.Qta) As Qta, avg(b.Qta_Extra_Totale)-sum(c.Qta_Extra_Totale) As Qta_Extra_Totale, avg(b.Tara)-sum(c.Tara) As Tara, ")
            StrSQL.AppendLine(" avg(b.Qta_Dettaglio1)-sum(c.Qta_Dettaglio1) As Qta_Dettaglio1, avg(b.Qta_Dettaglio2)-sum(c.Qta_Dettaglio2) As Qta_Dettaglio2, ")
            StrSQL.AppendLine(" avg(b.imponibile)-sum(c.imponibile) As imponibile, avg(b.imponibile_netto)-sum(c.imponibile_netto) As imponibile_netto, avg(b.iva)-sum(c.iva) As iva ")
            StrSQL.AppendLine(" FROM mov_dettagli_riferimenti a ")
            StrSQL.AppendLine(" INNER JOIN movimenti_dettagli b On a.id_mov_det_rif = b.id_mov_det ")
            StrSQL.AppendLine(" INNER JOIN movimenti_dettagli c On a.id_mov_det = c.id_mov_det ")
            StrSQL.AppendLine(" WHERE " & strClausoleVenAcqMovDetRif)
            StrSQL.AppendLine(" GROUP BY a.id_mov_det_rif HAVING sum(a.qta) < avg(b.qta)) As mov_det_rif On mov_det_rif.Id_Mov_Det_Rif = mov_det.Id_Mov_Det ")

            'Per ricavare gli ordini collegati alla riga del DDT
            StrSQL.AppendLine(" left join Mov_Dettagli_Riferimenti Riferimenti_Ordini ")
            StrSQL.AppendLine(" on Riferimenti_Ordini.Piva = mov_det.PIVA")
            StrSQL.AppendLine(" and Riferimenti_Ordini.Id_Agenda = mov_det.Id_Agenda")
            StrSQL.AppendLine(" and Riferimenti_Ordini.Id_Mov = mov_det.Id_Mov")
            StrSQL.AppendLine(" and Riferimenti_Ordini.Id_Mov_Det = mov_det.Id_Mov_Det")
            StrSQL.AppendLine(" and Riferimenti_Ordini.Lav_Cod_Rif IN (" & String.Join(", ", {LAVCOD_ORDINE_ACQUISTO, LAVCOD_ORDINE_VENDITA}.ToArray()) & ")")
            StrSQL.AppendLine(" left join Movimenti Movimenti_Ordini")
            StrSQL.AppendLine(" on Movimenti_Ordini.PIVA = Riferimenti_Ordini.PIVA_Rif")
            StrSQL.AppendLine(" and Movimenti_Ordini.Id_Agenda = Riferimenti_Ordini.Id_Agenda_Rif")
            StrSQL.AppendLine(" and Movimenti_Ordini.Id_Mov = Riferimenti_Ordini.Id_Mov_Rif")

        End If

        If report = Report_DDTVend_OrdLav Then
            StrSQL.AppendLine(" LEFT JOIN (select cal_cod, count(*) as num_lav from movimenti_dettagli d inner join agenda a on d.piva=a.piva and d.id_agenda=a.id_Agenda ")
            StrSQL.AppendLine("where a.lav_cod=" & LAVCOD_TRASFORMAZIONI & " group by cal_cod) AS mov_det_lav on mov_det_lav.cal_cod=mov_det.cal_cod ")
        End If

        If report = Report_OrdVend_OrdLav Then
            StrSQL.AppendLine(" LEFT JOIN (SELECT piva_rif, id_agenda_rif, COUNT(*) AS num_lav FROM mov_dettagli_riferimenti mdrif ")
            StrSQL.AppendLine("WHERE mdrif.lav_cod_rif = " & LAVCOD_ORDINE_VENDITA)
            StrSQL.AppendLine("AND mdrif.lav_cod = " & LAVCOD_TESTATE_ORDINE_LAVORAZIONE)
            StrSQL.AppendLine("GROUP BY piva_rif, id_agenda_rif) AS mov_det_lav")
            StrSQL.AppendLine("ON mov_det_lav.piva_rif = ag.piva AND mov_det_lav.id_agenda_rif = ag.id_agenda")
        End If

        ' StrSQL.AppendLine(" LEFT JOIN Materie_Prime As mat_prima With (nolock) On mov_det.PIVA = mat_prima.Piva And mov_det.Elem_Cod = mat_prima.Elem_Cod And mov_det.Mat_Cod = mat_prima.Mat_Cod ")
        StrSQL.AppendLine(" LEFT JOIN (Select Piva, Elem_Cod, Mat_Cod, 0 As Pro_Cod, Mat_Des, Cod_Articolo, Veg_Cod, Cul_Cod, Cat_Cod FROM Materie_Prime With (nolock) ")
        ' StrSQL.AppendLine(" UNION ALL Select null As Piva, 1 As Elem_Cod, 0 As Mat_Cod, cast(replace(Class_Code,'.','') as int) AS Pro_Cod, Class_Desc AS Mat_Des, '' As Cod_Articolo, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Macchine WITH (nolock) ")
        StrSQL.AppendLine(" UNION ALL SELECT null AS Piva, 3 AS Elem_Cod, 0 AS Mat_Cod, Fer_Cod AS Pro_Cod, Fer_Des AS Mat_Des, '' As Cod_Articolo, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Fertilizzanti WITH (nolock) ")
        StrSQL.AppendLine(" UNION ALL SELECT null AS Piva, 10 AS Elem_Cod, 0 AS Mat_Cod, Sem_Cod AS Pro_Cod, Sem_Des AS Mat_Des, '' As Cod_Articolo, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM TipologieSementi WITH (nolock) ")
        StrSQL.AppendLine(" UNION ALL SELECT null AS Piva, 191 AS Elem_Cod, 0 AS Mat_Cod, Fr_Cod AS Pro_Cod, Fr_Des AS Mat_Des, '' As Cod_Articolo, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Formulati WITH (nolock) ")
        StrSQL.AppendLine(" UNION ALL SELECT null AS Piva, 195 AS Elem_Cod, 0 AS Mat_Cod, Coad_Cod AS Pro_Cod, Coad_Des AS Mat_Des, '' As Cod_Articolo, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Coadiuvante WITH (nolock) ")
        StrSQL.AppendLine(" UNION ALL SELECT null AS Piva, 196 AS Elem_Cod, 0 AS Mat_Cod, Ins_Cod AS Pro_Cod, Ins_Des AS Mat_Des, '' As Cod_Articolo, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM InsettiUtili WITH (nolock) ")
        StrSQL.AppendLine(" UNION ALL SELECT null AS Piva, 197 AS Elem_Cod, 0 AS Mat_Cod, Trap_Cod AS Pro_Cod, Trap_Des AS Mat_Des, '' As Cod_Articolo, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Trappole WITH (nolock) ")
        StrSQL.AppendLine(" UNION ALL SELECT null AS Piva, 198 AS Elem_Cod, 0 AS Mat_Cod, Av_Cod AS Pro_Cod, Av_Des_Vol AS Mat_Des, '' As Cod_Articolo, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Avversita WITH (nolock) ")
        StrSQL.AppendLine(" UNION ALL SELECT null AS Piva, 555 AS Elem_Cod, 0 AS Mat_Cod, cast(replace(COD,'S','') as int) AS Pro_Cod, DESCR AS Mat_Des, '' As Cod_Articolo, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Categorie WITH (nolock) WHERE COD LIKE 'S%' AND Padre = 'S000029' ")
        StrSQL.AppendLine(" ) AS mat_prima ON mov_det.PIVA = ISNULL(mat_prima.Piva,mov_det.PIVA) AND mov_det.Elem_Cod = mat_prima.Elem_Cod AND mov_det.Mat_Cod = mat_prima.Mat_Cod AND mov_det.Pro_Cod = mat_prima.Pro_Cod")

        ' Join x Categorie Prodotto/Commerciale
        StrSQL.AppendLine(" LEFT JOIN CategorieMagazzino AS cat_mag WITH (nolock) ON cat_mag.Elem_Cod = mov_det.Elem_Cod ")
        StrSQL.AppendLine(" LEFT JOIN Linee_Classi_Produzioni AS cat_com WITH (nolock) ON cat_com.Linea_Classe_Cod = mat_prima.Cat_Cod ")

        'Join x Gruppi Merce
        If _gestioneGruppiMerce Then
            StrSQL.AppendLine(" LEFT JOIN Prodotti_Extra_Privata as prodExtraPriv WITH (nolock)")
            StrSQL.AppendLine("              ON prodExtraPriv.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine("       AND prodExtraPriv.Elem_Cod = mov_det.Elem_Cod")
            StrSQL.AppendLine("       AND prodExtraPriv.Mat_Cod = mov_det.Mat_Cod")
            StrSQL.AppendLine("       AND prodExtraPriv.Pro_Cod = mov_det.Pro_Cod")
            StrSQL.AppendLine("             AND (prodExtraPriv.Piva = mov_det.Piva OR mov_det.Pro_Cod = 0)")
            StrSQL.AppendLine(" LEFT JOIN Gruppi_Merce as grpMerce WITH (nolock)")
            StrSQL.AppendLine("              ON grpMerce.Id_Gruppo_Merce = prodExtraPriv.Id_Gruppo_Merce")
            StrSQL.AppendLine(" LEFT JOIN #DefaultGruppiMerce")
            StrSQL.AppendLine("              ON #DefaultGruppiMerce.Piva = mov_det.Piva COLLATE DATABASE_DEFAULT")
            StrSQL.AppendLine("              AND #DefaultGruppiMerce.Elem_Cod = mov_det.Elem_Cod")
        End If

        ' Join x Destinazione
        StrSQL.AppendLine(" LEFT JOIN Risorse_Umane AS r_u_destinazione WITH (nolock) ON movNrDDT.Cod_Destinazione = r_u_destinazione.Cod_RisUm ")
        StrSQL.AppendLine(" LEFT JOIN Contatti AS cont_destinazione WITH (nolock) ON r_u_destinazione.Piva = cont_destinazione.Piva AND r_u_destinazione.Cod_Contatto = cont_destinazione.Cod_Contatto ")

        ' Join x Vettore
        StrSQL.AppendLine(" LEFT JOIN Risorse_Umane AS r_u_vettore WITH (nolock) ON movNrDDT.Cod_Vettore = r_u_vettore.Cod_RisUm ")
        StrSQL.AppendLine(" LEFT JOIN Contatti AS cont_vettore WITH (nolock) ON r_u_vettore.Piva = cont_vettore.Piva AND r_u_vettore.Cod_Contatto = cont_vettore.Cod_Contatto ")
        StrSQL.AppendLine(" LEFT JOIN Mov_Dettaglio_Tecnico_Extra AS Movimento_Extra_0 WITH (nolock) ON Movimento_Extra_0.Piva = ag.PIVA AND Movimento_Extra_0.Id_Agenda = ag.Id_Agenda AND Movimento_Extra_0.Id_Mov_Det = 0 ")

        ' Join x Agente + Provvigione
        StrSQL.AppendLine(" LEFT JOIN Risorse_Umane AS r_u_agente WITH (nolock) ON Movimento_Extra_0.Agente_Cod = r_u_agente.Cod_RisUm ")
        StrSQL.AppendLine(" LEFT JOIN Contatti AS cont_agente WITH (nolock) ON r_u_agente.Piva = cont_agente.Piva AND r_u_agente.Cod_Contatto = cont_agente.Cod_Contatto ")
        StrSQL.AppendLine(" LEFT JOIN Risorse_Umane AS r_u_capoarea WITH (nolock) ON Movimento_Extra_0.CapoArea_Cod = r_u_capoarea.Cod_RisUm ")
        StrSQL.AppendLine(" LEFT JOIN Contatti AS cont_capoarea WITH (nolock) ON r_u_capoarea.Piva = cont_capoarea.Piva AND r_u_capoarea.Cod_Contatto = cont_capoarea.Cod_Contatto ")
        StrSQL.AppendLine(" LEFT JOIN Mov_Dettaglio_Tecnico_Extra AS Movimento_Extra_Dettagli WITH (nolock) ON Movimento_Extra_Dettagli.Piva = ag.PIVA AND Movimento_Extra_Dettagli.Id_Agenda = ag.Id_Agenda AND Movimento_Extra_Dettagli.Id_Mov_Det = mov_det.Id_Mov_Det ")

        ' Join x Unità Misura
        StrSQL.AppendLine(" LEFT JOIN unitamisura WITH (nolock) ON mov_det.udm_cod = unitamisura.udm_cod ")
        StrSQL.AppendLine(" LEFT JOIN unitamisura AS udm_sec WITH (nolock) ON mov_det.udm_cod_extra = udm_sec.udm_cod ")

        ' Join x Comune/Regione
        'StrSQL.AppendLine(" LEFT JOIN ContattiXIndirizzi WITH (nolock) ON cont.Piva = ContattiXIndirizzi.Piva AND cont.Cod_Contatto = ContattiXIndirizzi.Cod_Contatto AND ContattiXIndirizzi.Tipo_Indirizzo IN (2, 101) ")
        'StrSQL.AppendLine(" LEFT JOIN Indirizzi WITH (nolock) ON ContattiXIndirizzi.Cod_Indirizzo = Indirizzi.cod_indirizzo ")
        StrSQL.AppendLine(" LEFT JOIN Indirizzi WITH (nolock) ON movNrDDT.Cod_IndirizzoRisUm = Indirizzi.cod_indirizzo ")
        StrSQL.AppendLine(" LEFT JOIN Lista_Province WITH (nolock) ON Indirizzi.pro_cod_istat = Lista_Province.PROV ")
        StrSQL.AppendLine(" LEFT JOIN Lista_Regioni WITH (nolock) ON Lista_Province.REG = Lista_Regioni.REG ")
        StrSQL.AppendLine(" LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 AS Lista_Nazioni WITH (nolock) ON Indirizzi.stato = Lista_Nazioni.Codice ")

        ' Join x Comune/Regione Destinazione
        'StrSQL.AppendLine(" LEFT JOIN ContattiXIndirizzi as cont_indirizzi_dest WITH (nolock) ON cont_destinazione.Piva = cont_indirizzi_dest.Piva AND cont_destinazione.Cod_Contatto = cont_indirizzi_dest.Cod_Contatto AND cont_indirizzi_dest.Tipo_Indirizzo IN (2, 101) ")
        'StrSQL.AppendLine(" LEFT JOIN Indirizzi as indirizzi_dest WITH (nolock) ON cont_indirizzi_dest.Cod_Indirizzo = indirizzi_dest.cod_indirizzo ")
        StrSQL.AppendLine(" LEFT JOIN Indirizzi as indirizzi_dest WITH (nolock) ON movNrDDT.Cod_IndirizzoDestinazione = indirizzi_dest.cod_indirizzo ")
        StrSQL.AppendLine(" LEFT JOIN Lista_Province as province_dest WITH (nolock) ON indirizzi_dest.pro_cod_istat = province_dest.PROV ")
        StrSQL.AppendLine(" LEFT JOIN Lista_Regioni as regioni_dest WITH (nolock) ON province_dest.REG = regioni_dest.REG ")
        StrSQL.AppendLine(" LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 AS nazioni_dest WITH (nolock) ON indirizzi_dest.stato = nazioni_dest.Codice ")

        StrSQL.AppendLine(" LEFT JOIN ContattiXIndirizzi as cont_indirizzi_dest WITH (nolock) ON cont_destinazione.Piva = cont_indirizzi_dest.Piva And cont_destinazione.Cod_Contatto = cont_indirizzi_dest.Cod_Contatto  And indirizzi_dest.cod_indirizzo = cont_indirizzi_dest.Cod_Indirizzo  ")
        StrSQL.AppendLine(" LEFT JOIN IndirizzoTipo as IndirizzoTipo_dest WITH (nolock) ON IndirizzoTipo_dest.Piva = cont_indirizzi_dest.Piva And IndirizzoTipo_dest.IndirizzoTipo_Cod = cont_indirizzi_dest.Tipo_Indirizzo And IndirizzoTipo_dest.Cod_Contatto = cont_indirizzi_dest.Cod_Contatto  ")

        ' Join x stato fatturazione
        StrSQL.AppendLine(" Left Join ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 AS nazioni_fatt WITH (nolock) ON indirizzi.stato = nazioni_fatt.Codice ")

        If tipoValore = "4" OrElse tipoValore = "5" Then
            StrSQL.AppendLine(" Left Join Materie_PRime AS m_p WITH (nolock) ON m_p.piva = mov_det.piva And  m_p.Elem_Cod = mov_det.Elem_Cod And  m_p.Mat_Cod = mov_det.Mat_Cod ")
        End If

        ' Join x Parametri Qualitativi
        If DTParamQual IsNot Nothing Then
            For Each paramQual In DTParamQual.Rows
                StrSQL.AppendLine(" LEFT JOIN Materie_Prime_Campionature AS Materie_Prime_Campionature_" & paramQual("Tabella_Key") &
                    " WITH (nolock) ON mov_det.Cal_Cod = Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Progressivo " &
                    " And Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo = 'o" & paramQual("Tabella_Key") & "'")
                StrSQL.AppendLine(" LEFT JOIN OTabelle_Parametri AS OTabelle_Parametri_" & paramQual("Tabella_Key") &
                        " WITH (nolock) ON Materie_Prime_Campionature_" & paramQual("Tabella_Key") & ".Tipo_Cod =  OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Tabella_Par_Cod " &
                        " AND OTabelle_Parametri_" & paramQual("Tabella_Key") & ".Tabella_Cod = '" & paramQual("Tabella_ID") & "'")
            Next
        End If

        ' Join con Imprese
        StrSQL.AppendLine(" LEFT JOIN Imprese i WITH (nolock) on ag.Piva = i.Piva ")

        '<GBEL> 2020-07-30 Tabelle CTE per date consegna e ultima consegna 
        If report = Report_Ordini_Vendita OrElse report = Report_Vendite OrElse report = Report_Ordini_Acquisto OrElse report = Report_Acquisti Then
            ' data ultima consegna stesso prodotto al cliente
            StrSQL.AppendLine(" Left Join ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select sped.piva, sped.Cod_RisUm, ")
            StrSQL.AppendLine(" sped.Elem_Cod, sped.Mat_Cod, sped.Pro_Cod, max(Data_Movimento) as Data_Ultima_Consegna ")
            StrSQL.AppendLine(" From Spedizioni_CTE sped ")
            StrSQL.AppendLine(" Group By sped.piva, sped.Cod_RisUm, sped.Elem_Cod, sped.Mat_Cod, sped.Pro_Cod ")
            StrSQL.AppendLine(" ) sped ")
            StrSQL.AppendLine(" On sped.Cod_RisUm = r_u.Cod_RisUm And sped.Elem_Cod = mov_det.Elem_Cod ")
            StrSQL.AppendLine(" And sped.piva = ag.PIVA And sped.mat_cod = mov_det.Mat_Cod  ")
            StrSQL.AppendLine(" And sped.Pro_Cod = mov_det.Pro_Cod ")
            ' data prima ed ultima consegna legate allo specifico ordine
            StrSQL.AppendLine(" Left Join ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select piva, Id_agenda_rif, id_mov_rif, id_mov_det_rif, max(Data_Movimento) AS Data_Ultima_Spedizione, min(Data_Movimento) As Data_Prima_Spedizione ")
            StrSQL.AppendLine(" From Consegna_CTE c ")
            StrSQL.AppendLine(" group by c.piva, c.id_agenda_rif, c.Id_Mov_Det_Rif, c.Id_Mov_Rif ")
            StrSQL.AppendLine(" ) cons ")
            StrSQL.AppendLine(" On cons.piva = ag.PIVA And cons.id_agenda_rif = ag.Id_Agenda ")
            StrSQL.AppendLine(" And cons.id_mov_det_rif = mov_det.Id_Mov_Det ")
        End If

        If report = Report_Acquisti Then
            StrSQL.AppendLine(" left join Mov_Dettaglio_Conferimento mdc on mdc.Id_Agenda = ag.Id_Agenda and mdc.Id_Mov_Det = mov_det.Id_Mov_det ")
            StrSQL.AppendLine(" Left join Codifica_Varieta_OIPomodorodaIndustriaNordItalia cvp on cvp.Cod_Varieta = mdc.Cod_Varieta ")
        End If

        'Join per conto economico
        StrSQL.AppendLine(" Left Join RicXConti WITH (nolock) ON RicXConti.Piva = mov_det.Piva AND RicXConti.Ric_Cod = mov_det.Ric_Cod AND RicXConti.Cod_Conto = mov_det.Cod_Conto AND RicXConti.Anno = mov_det.Anno ")
        StrSQL.AppendLine(" Left Join Conti WITH (nolock) ON Conti.Cod_Conto = mov_det.Cod_Conto ")

        ' Filtri Principali
        StrSQL.AppendLine(" WHERE (mov.Cau_Mov IN ('7300','7350')) ")


        StrSQL.AppendLine(" AND movNrDDT.Cau_Mov = ")
        StrSQL.AppendLine("  case when ag.lav_cod = " & LAVCOD_AUTOCONSUMO & " then '7350' else '4000' end")

        'StrSQL.AppendLine(" AND (movNrDDT.Cau_Mov = '4000') ")

        If Not String.IsNullOrEmpty(piva) Then
            If report <> Report_Ordini_Acquisto AndAlso report <> Report_Acquisti AndAlso report <> Report_Pdf_Acquisto Then
                StrSQL.AppendLine(" AND (ag.PIVA = '" & Agro_SQL_SaveText(piva) & "') ")
            Else
                ' TODO
            End If
        End If

        ' scarto gli imballi dalle righe DDT (da verificare)
        Dim arrLavCodImballiScartare As Integer() = New Integer() {}

        If report = Report_Ordini_Vendita OrElse
           report = Report_Vendite OrElse
           report = Report_Pdf_Vendita OrElse
           report = Report_DDTVend_OrdLav OrElse
           report = Report_OrdVend_OrdLav Then
            arrLavCodImballiScartare = New Integer() {LAVCOD_BOLLA_EMESSA, LAVCOD_MVV_EMESSO}
        End If

        If report = Report_Ordini_Acquisto OrElse
           report = Report_Acquisti OrElse
           report = Report_Pdf_Acquisto Then
            arrLavCodImballiScartare = New Integer() {LAVCOD_BOLLA_RICEVUTA, LAVCOD_MVV_RICEVUTO}
        End If

        StrSQL.AppendLine(" AND (mov_det.Mat_Cod <> 0 OR mov_det.Pro_Cod <> 0 OR mov_det.Elem_Cod = 501) ")
        If arrLavCodImballiScartare.Any Then
            StrSQL.AppendLine(" AND (mat_prima.Elem_Cod <> 205 OR mov_det.ordine_det <> 1000 or ag.Lav_Cod NOT IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodImballiScartare)) & ")) ")
        Else
            StrSQL.AppendLine(" AND (mat_prima.Elem_Cod <> 205 OR mov_det.ordine_det <> 1000) ")
        End If


        If report = Report_Ordini_Vendita OrElse
           report = Report_OrdVend_OrdLav Then
            ' Filtro su tipi documenti
            StrSQL.AppendLine(" AND (ag.Lav_Cod IN (" & LAVCOD_ORDINE_VENDITA & ")) ")
        End If

        If report = Report_Ordini_Acquisto Then
            ' Filtro su tipi documenti
            StrSQL.AppendLine(" AND (ag.Lav_Cod IN (" & LAVCOD_ORDINE_ACQUISTO & ")) ")
        End If

        If report = Report_Vendite OrElse
           report = Report_Acquisti OrElse
           report = Report_Pdf_Vendita OrElse
           report = Report_Pdf_Acquisto OrElse
           report = Report_DDTVend_OrdLav Then

            Dim arrLavCodVenAcq As Integer()
            Dim arrLavCodDaEscl As Integer()

            If report = Report_Vendite OrElse
                report = Report_Pdf_Vendita OrElse
                report = Report_DDTVend_OrdLav Then

                arrLavCodVenAcq = New Integer() {LAVCOD_FATTURA_EMESSA, LAVCOD_VENDITA, LAVCOD_RICEVUTA_EMESSA, LAVCOD_AUTOCONSUMO,
                    LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_BOLLA_EMESSA, LAVCOD_MVV_EMESSO, LAVCOD_ORDINE_VENDITA, LAVCOD_NOTA_ACCREDITO_EMESSA}

                arrLavCodDaEscl = New Integer() {LAVCOD_ORDINE_VENDITA}
            Else
                arrLavCodVenAcq = New Integer() {LAVCOD_BOLLA_RICEVUTA, LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
                    LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, LAVCOD_DISTINTA_CARICO, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_FATTURA_RICEVUTA,
                    LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_ORDINE_ACQUISTO}

                arrLavCodDaEscl = New Integer() {LAVCOD_ORDINE_ACQUISTO}
            End If

            ' Filtro su tipi documenti
            StrSQL.AppendLine(" AND (ag.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodVenAcq)) & ")) ")

            ' escludo le righe ordine contabilizzate
            ' 3 e -3 corrispondono (il secondo è per una contabilizzazione in una data futura) ed indicano le righe di ordine forzatamente contabilizzate
            ' perché magari erano state create slegate dal ddt
            StrSQL.AppendLine(" AND (mov_det.Contabilizzato NOT IN (3,-3) OR ag.Lav_Cod NOT IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", arrLavCodDaEscl)) & ")) ")

            ' Filtro per escludere righe dei documenti collegato ad eccezione di quelle che hanno dei residui
            ' è possibile avere degli ordini che vengono poi spediti in più consegne e quindi ognuna di queste avrà un proprio ddt, ma tutti questi collegati allo stesso ordine
            StrSQL.AppendLine(" AND (mov_det.Id_Mov_Det NOT IN ( ")
            StrSQL.AppendLine(" SELECT Id_Mov_Det_Rif FROM Mov_Dettagli_Riferimenti ")
            StrSQL.AppendLine(" WHERE " & strClausoleVenAcqMovDetRif & " )") 'Con la tonda chiudo il NOT IN
            StrSQL.AppendLine(" OR mov_det.Id_Mov_Det IN ( ")
            StrSQL.AppendLine(" SELECT a.id_mov_det_rif FROM mov_dettagli_riferimenti a INNER JOIN movimenti_dettagli b ON a.id_mov_det_rif = b.id_mov_det ")
            StrSQL.AppendLine(" WHERE " & strClausoleVenAcqMovDetRif)
            StrSQL.AppendLine(" GROUP BY a.id_mov_det_rif HAVING sum(a.qta) < avg(b.qta) )) ") 'Con le due tonde chiudo l'IN e le due condizioni in OR su mov_det.Id_Mov_Det

        End If

        'Chiudo la parentesi aperta dalla variabile selectPrincipale
        'StrSQL.Append(")")

        Return StrSQL.ToString

    End Function

    Private Function Componi_Query_Su_Tipo_Valore(ByVal report As String, ByVal tipoValore As String, ByVal nomeCampo As String) As String

        Dim StrSQL As New StringBuilder
        Dim fieldName As String = nomeCampo

        Dim lavCodNotaAccredito As Integer
        If report = Report_Ordini_Vendita OrElse
           report = Report_Vendite OrElse
           report = Report_Pdf_Vendita OrElse
           report = Report_DDTVend_OrdLav OrElse
           report = Report_OrdVend_OrdLav Then
            lavCodNotaAccredito = LAVCOD_NOTA_ACCREDITO_EMESSA
        End If
        If report = Report_Ordini_Acquisto OrElse
           report = Report_Acquisti OrElse
           report = Report_Pdf_Acquisto Then
            lavCodNotaAccredito = LAVCOD_NOTA_ACCREDITO_RICEVUTA
        End If

        'Dati Quantità / Peso
        If tipoValore <> "4" AndAlso tipoValore <> "5" Then
            StrSQL.AppendLine(" (CASE WHEN ag.Lav_Cod = " & lavCodNotaAccredito & "  THEN -1 ELSE 1 END) * ISNULL(mov_det_rif.Qta,mov_det.Qta) AS " & fieldName & ",")
        Else
            ' Kg / Lt
            If tipoValore = "4" Then
                ' Se sono già in Kg / Lt sono a posto
                StrSQL.AppendLine(" (CASE WHEN ag.Lav_Cod = " & lavCodNotaAccredito & " THEN -1 ELSE 1 END) * (CASE WHEN mov_det.udm_cod = 2 OR mov_det.udm_cod = 29 THEN ISNULL(mov_det_rif.Qta,mov_det.Qta) ELSE  ")
                ' altrimenti Se sono in confezioni e sulla riga c'è già la conversione in U.M. secondaria ... 
                StrSQL.AppendLine("    (CASE WHEN mov_det.udm_cod = 38 And ISNULL(mov_det_rif.Qta_Extra_Totale, mov_det.Qta_Extra_Totale) != 0 And mov_det.udm_cod != mov_det.udm_cod_extra  THEN   ")
                ' ... prendo la secondaria totale
                StrSQL.AppendLine("         ISNULL(mov_det_rif.Qta_Extra_Totale, mov_det.Qta_Extra_Totale) ELSE ")
                ' altrimenti Se sono in confezioni e sull'anagrafica c'è un fattore di conversione moltiplico le confezioni per il fattore di conversione
                StrSQL.AppendLine("             (CASE WHEN ISNULL(m_p.Qta_Extra,0) != 0 THEN ISNULL(mov_det_rif.Qta, mov_det.Qta) * ISNULL(m_p.Qta_Extra,0) ")
                ' altrimenti 0 in tutti gli altri casi
                StrSQL.AppendLine("                                                     Else 0 End) ")
                StrSQL.AppendLine("         End) ")
                StrSQL.AppendLine("  End) As " & fieldName & ",")
            Else
                ' Pezzi / Nr
                If tipoValore = "5" Then
                    ' Se sono già in Confezioni sono a posto
                    StrSQL.AppendLine(" (Case When ag.Lav_Cod = " & lavCodNotaAccredito & " Then -1 Else 1 End) * (Case When mov_det.udm_cod = 38 Then ISNULL(mov_det_rif.Qta,mov_det.Qta) Else  ")
                    ' altrimenti Se sono in kg / lt e sull'anagrafica c'è un fattore di conversione divido kg / lt per il fattore di conversione trovando le confezioni
                    StrSQL.AppendLine("   (Case When (mov_det.udm_cod = 2 Or mov_det.udm_cod = 29) And ISNULL(m_p.Qta_Extra,0) != 0 Then   ")
                    StrSQL.AppendLine("       ISNULL(mov_det_rif.Qta, mov_det.Qta) / ISNULL(m_p.Qta_Extra,0) ")
                    ' altrimenti 0 in tutti gli altri casi
                    StrSQL.AppendLine("         Else 0 End) ")
                    StrSQL.AppendLine("  End) As " & fieldName & ",")
                End If
            End If
        End If

        Return StrSQL.ToString()

    End Function

    ''' ----------------------------------------------------------------------------
    ''' <summary>
    ''' Crea query movimenti di vendita per report statistico
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Sub Filtra_Query_Report_Vendite(ByRef StrSQL As StringBuilder, ByVal objParametri As AgronicaCoreParametri, ByVal objParametriUtenti As AgronicaCoreParametri,
                                           ByVal piva As String, ByVal _docNumeroSin As String, ByVal _docNumero As Integer,
                                            ByVal _docNumeroDes As String, ByVal _nrRiga As String,
                                            ByVal _dataMovDal As String, ByVal _dataMovAl As String,
                                            ByVal _clienti As String, ByVal _agenti As String, ByVal _causali As String,
                                            ByVal _specie As String, ByVal _varieta As String,
                                            ByVal _prodotti As String, ByVal _categorie As String, ByVal _categcommerciali As String,
                                            ByVal _causali_trasp As String,
                                            Optional ByVal _rapportiContabili As String = Nothing,
                                            Optional ByVal _nazioniFatturazione As String = Nothing,
                                            Optional ByVal _dataEvasionePrevDal As String = Nothing,
                                            Optional ByVal _dataEvasionePrevAl As String = Nothing,
                                            Optional ByVal _includiVenditeSenzaClienteInt As Boolean = Nothing,
                                            Optional ByVal _report As String = Nothing,
                                            Optional ByVal _daLavorare As Boolean = False
                                            )

        ' Filtro Principale
        If Not IsNothing(_report) Then

            If _report = Report_Ordini_Vendita OrElse _report = Report_Vendite Then
                StrSQL.AppendLine(" WHERE (PIVA = '" & Agro_SQL_SaveText(piva) & "') ")
            Else

                '----------------------------------------------------------------
                '--- Filtro associato all'utente 
                '----------------------------------------------------------------

                If _listaImprese.Count > 0 Then
                    StrSQL.AppendLine(" WHERE Piva_Impresa IN (" & Agro_SQL_Save_Clausola_IN("'" & String.Join("', '", _listaImprese) & "'", True) & ") ")
                Else
                    StrSQL.AppendLine(" WHERE 1=1 ")
                End If

            End If

        Else
            StrSQL.AppendLine(" WHERE (PIVA = '" & Agro_SQL_SaveText(piva) & "') ")
        End If


        ' Filtri Documento
        If _docNumero <> 0 Then
            StrSQL.AppendLine(" AND (Doc_Numero = " & Agro_SQL_SaveNum(_docNumero) & " ) ")
        End If

        If Not String.IsNullOrEmpty(_docNumeroSin) Then
            StrSQL.AppendLine(" AND (Doc_Numero_Sin = '" & Agro_SQL_SaveText(_docNumeroSin) & "' ) ")
        End If

        If Not String.IsNullOrEmpty(_docNumeroDes) Then
            StrSQL.AppendLine(" AND (Doc_Numero_Des = '" & Agro_SQL_SaveText(_docNumeroDes) & "' ) ")
        End If

        If Not String.IsNullOrEmpty(_nrRiga) Then
            StrSQL.AppendLine(" AND (Ordine_Det = " & Agro_SQL_SaveNum(_nrRiga) & " ) ")
        End If

        ' Filtro Tipo Documento
        If Not String.IsNullOrEmpty(_causali) Then

            Dim causaliDaConsiderare As List(Of String) = _causali.Split("|").Where(Function(s) s <> "").ToList
            If Not IsNothing(_includiVenditeSenzaClienteInt) Then
                If Not _includiVenditeSenzaClienteInt Then
                    If causaliDaConsiderare.Contains(LAVCOD_VENDITA) Then
                        causaliDaConsiderare.Remove(LAVCOD_VENDITA)
                    End If
                    If causaliDaConsiderare.Contains(LAVCOD_AUTOCONSUMO) Then
                        causaliDaConsiderare.Remove(LAVCOD_AUTOCONSUMO)
                    End If
                End If

            End If

            If causaliDaConsiderare.Any Then
                StrSQL.Append(" AND (Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", causaliDaConsiderare)) & ") ) ")
            End If
        Else
            If Not IsNothing(_includiVenditeSenzaClienteInt) Then
                If Not _includiVenditeSenzaClienteInt Then
                    StrSQL.Append(" AND Lav_Cod NOT IN (" & LAVCOD_VENDITA & "," & LAVCOD_AUTOCONSUMO & ")")
                End If
            End If
        End If


        ' Filtro Causali Trasporto
        If Not String.IsNullOrEmpty(_causali_trasp) Then
            StrSQL.Append(" AND (Causale_Trasporto_Cod IN (0," & Replace(_causali_trasp, "|", ",") & ") ) ")
        End If

        ' Filtro Data Movimento
        If Not String.IsNullOrEmpty(_dataMovDal) Then
            StrSQL.Append(" AND (Data_Movimento >= " & Agro_SQL_SaveDate(Convert.ToDateTime(_dataMovDal)) & " ) ")
        End If

        If Not String.IsNullOrEmpty(_dataMovAl) Then
            StrSQL.Append(" AND (Data_Movimento <= " & Agro_SQL_SaveDate(Convert.ToDateTime(_dataMovAl)) & " ) ")
        End If

        ' Filtro Data Evasione Prevista Dal
        If _dataEvasionePrevDal IsNot Nothing AndAlso Not String.IsNullOrEmpty(_dataEvasionePrevDal) Then
            StrSQL.Append(" AND Data_Evasione_Prevista >= " & Agro_SQL_SaveDate(Convert.ToDateTime(_dataEvasionePrevDal)) & "  ")
        End If

        ' Filtro Data Evasione Prevista Dal
        If _dataEvasionePrevAl IsNot Nothing AndAlso Not String.IsNullOrEmpty(_dataEvasionePrevAl) Then
            StrSQL.Append(" AND Data_Evasione_Prevista <= " & Agro_SQL_SaveDate(Convert.ToDateTime(_dataEvasionePrevAl)) & "  ")
        End If

        ' Filtro Clienti
        If Not String.IsNullOrEmpty(_clienti) Then
            If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                StrSQL.Append(" AND (Soggetto_Piva IN ('" & Agro_SQL_Save_Clausola_IN(Replace(_clienti, "|", "','"), True) & "') ) ")
            Else
                If DataProviderFactory.Instance.ParametrizzaQuery Then
                    StrSQL.Append(" AND (Soggetto_Piva IN (" & Agro_SQL_Save_Clausola_IN(Replace(_clienti, "|", "','"), True) & ") ) ")
                Else
                    StrSQL.Append(" AND (Soggetto_Piva IN ('" & Agro_SQL_Save_Clausola_IN(Replace(_clienti, "|", "','"), True) & "') ) ")
                End If

            End If
        End If

        ' Filtro Agenti
        If Not String.IsNullOrEmpty(_agenti) Then
            If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                StrSQL.Append(" AND (Agente_Piva IN ('" & Agro_SQL_Save_Clausola_IN(Replace(_agenti, "|", "','"), True) & "') ) ")
            Else
                If DataProviderFactory.Instance.ParametrizzaQuery Then
                    StrSQL.Append(" AND (Agente_Piva IN (" & Agro_SQL_Save_Clausola_IN(Replace(_agenti, "|", "','"), True) & ") ) ")
                Else
                    StrSQL.Append(" AND (Agente_Piva IN ('" & Agro_SQL_Save_Clausola_IN(Replace(_agenti, "|", "','"), True) & "') ) ")
                End If

            End If
        End If

        ' Filtro Specie
        If Not String.IsNullOrEmpty(_specie) Then
            StrSQL.Append(" AND (Veg_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(_specie, "|", ",")) & ") ) ")
        End If

        ' Filtro Varietà
        If Not String.IsNullOrEmpty(_varieta) Then
            StrSQL.Append(" AND (Cul_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(_varieta, "|", ",")) & ") ) ")
        End If

        ' Filtro Categorie
        If Not String.IsNullOrEmpty(_categorie) Then
            StrSQL.Append(" AND (Elem_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(_categorie, "|", ",")) & ") ) ")
        End If

        ' Filtro Categorie Commerciali  TODO
        If Not String.IsNullOrEmpty(_categcommerciali) Then
            StrSQL.Append(" AND (Cat_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(_categcommerciali, "|", ",")) & ") ) ")
        End If

        ' Filtro Prodotti
        If Not String.IsNullOrEmpty(_prodotti) Then
            StrSQL.Append(" AND ( ")
            Dim elenco As New System.Text.StringBuilder("")
            Dim prodArray As String() = _prodotti.Split("|")
            For Each p In prodArray
                If elenco.ToString() <> "" Then
                    elenco.Append(" OR ")
                End If
                elenco.Append(" ( ")
                Dim dueParti As String() = p.Split("_")
                elenco.Append(" Elem_Cod =  " & CInt(dueParti(0)) & " AND ")
                If CInt(dueParti(1)) > 0 Then
                    elenco.Append(" Pro_Cod =  " & CInt(dueParti(1)))
                Else
                    elenco.Append(" Mat_Cod =  " & CInt(dueParti(1)) * -1)
                End If
                elenco.Append(" ) ")
            Next
            StrSQL.Append(elenco.ToString())
            StrSQL.Append(" ) ")
        End If

        ' Rapporti contabili
        If _rapportiContabili IsNot Nothing AndAlso Not String.IsNullOrEmpty(_rapportiContabili) Then
            StrSQL.Append(" AND (Soggetto_Cod_Rapporto IN (" & Agro_SQL_Save_Clausola_IN(Replace(_rapportiContabili, "|", ",")) & ") ) ")
        End If

        ' Nazioni Fatturazione
        If _nazioniFatturazione IsNot Nothing AndAlso Not String.IsNullOrEmpty(_nazioniFatturazione) Then
            If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                StrSQL.Append(" AND (Codice_Stato_Fatt IN ( '" & Agro_SQL_Save_Clausola_IN(Replace(_nazioniFatturazione, "|", "','"), True) & "') ) ")
            Else
                If DataProviderFactory.Instance.ParametrizzaQuery Then
                    StrSQL.Append(" AND (Codice_Stato_Fatt IN ( " & Agro_SQL_Save_Clausola_IN(Replace(_nazioniFatturazione, "|", "','"), True) & ") ) ")
                Else
                    StrSQL.Append(" AND (Codice_Stato_Fatt IN ( '" & Agro_SQL_Save_Clausola_IN(Replace(_nazioniFatturazione, "|", "','"), True) & "') ) ")
                End If

            End If
        End If

        ' Escludo le righe collegate a lavorazioni
        If _daLavorare Then
            StrSQL.Append(" AND Lavorato = 'NO' ")
        End If

        ' Gestione Gruppi Utenti X Gruppi Merce: indica se i gruppi degli utenti vengono limitati a utilizzare solo determinati gruppi merce
        Dim GestioneGruppiUtenteMerce As Boolean = False
        Dim objGruppiUtenteMerce As New AgronicaCoreAnagrafeDAL.Gruppi_UtenteXGruppi_Merce_R(objParametri, objParametriUtenti)
        Dim dtGruppiUtenteMerce As New DataTable()
        If _gestioneGruppiMerce Then

            Dim filtroAggiuntivo As String = ""
            If _listaImprese.Count > 0 Then
                filtroAggiuntivo = "Gruppi_UtenteXGruppi_Merce.Piva IN('" & String.Join("', '", _listaImprese) & "')"
            End If

            dtGruppiUtenteMerce = objGruppiUtenteMerce.Leggi(filtroAggiuntivo, "")
            GestioneGruppiUtenteMerce = dtGruppiUtenteMerce.Rows.Count > 0
        End If

        If GestioneGruppiUtenteMerce AndAlso objParametri.UtenteUsername <> objParametri.SuperUserUsername Then

            StrSQL.AppendLine("")

            If _listaImprese.Count = 1 Then
                'In questo caso sto cercando i record di un'unica impresa
                StrSQL.AppendLine(" AND Report_Vendite.Id_Gruppo_Merce IN (")
                StrSQL.Append(objGruppiUtenteMerce.ComponiSql_DistinctGruppiMerce_X_GruppiUtente("", _listaImprese.Item(0)))
                StrSQL.AppendLine(" )")
            Else
                Dim azVisibiliGruppiMerce = dtGruppiUtenteMerce.AsEnumerable().Where(Function(dr) dr.Field(Of String)("piva") <> "").Select(Function(dr) dr.Field(Of String)("piva"))
                Dim azDistinctGruppiMerce = azVisibiliGruppiMerce.Distinct()

                'Mostro tutti i record delle imprese per le quali non gestisco i gruppi mentre filtro le altre
                StrSQL.AppendLine(" AND ( Report_Vendite.Piva NOT IN (" & Agro_SQL_Save_Clausola_IN("'" & String.Join("', '", azDistinctGruppiMerce) & "'", True) & ")")
                StrSQL.AppendLine("  OR Report_Vendite.Id_Gruppo_Merce IN (")
                StrSQL.Append(objGruppiUtenteMerce.ComponiSql_DistinctGruppiMerce_X_GruppiUtente("Report_Vendite", ""))
                StrSQL.AppendLine("  )")
                StrSQL.AppendLine(" )")
            End If

        End If

    End Sub

    Private Function Crea_Query_Report_Vendite(ByVal piva As String, ByVal report As String, ByVal cubo As Integer,
                                    ByVal _docNumeroSin As String, ByVal _docNumero As Integer,
                                    ByVal _docNumeroDes As String, ByVal _nrRiga As String,
                                    ByVal _dataMovDal As String, ByVal _dataMovAl As String,
                                    ByVal _clienti As String, ByVal _agenti As String, ByVal _causali As String,
                                    ByVal _specie As String, ByVal _varieta As String,
                                    ByVal _prodotti As String, ByVal _categorie As String, ByVal _categcommerciali As String, ByVal _causali_trasp As String,
                                    ByVal _rapportiCOntabili As String, ByVal _nazioniFatturazione As String,
                                    ByVal xOrderBy As String,
                                    ByVal tipoValore As String, ByVal tipoValore2 As String, ByVal includiCorrispettivi As Boolean,
                                    ByRef objParametri As AgronicaCoreParametri, ByRef objParametriUtenti As AgronicaCoreParametri) As String

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Statistiche.Crea_Query_Report_Vendite()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder

        Try

            If cubo AndAlso report = Report_Vendite Then

                Dim esisteTabella = Esiste_Report(Report_Vendite, objParametri)
                If Not esisteTabella Then
                    Dim scrivi As New Statistiche_W
                    Dim risposta = scrivi.Aggiorna_Cubo_Report_Vendite(piva, _dataMovDal, _dataMovAl, objParametri)
                End If

                ' Query Principale
                StrSQL.AppendLine(" SELECT * FROM Report_Vendite ")

            Else

                Dim QuerySQL = Leggi_Query_Report_Vendite(piva, report, tipoValore, tipoValore2, " Select * from (", objParametri)

                ' Query Principale
                StrSQL.Append(QuerySQL & " ) AS Report_Vendite ")

            End If

            ' Applica Filtri
            Filtra_Query_Report_Vendite(StrSQL, objParametri, objParametriUtenti, piva,
                                        _docNumeroSin, _docNumero, _docNumeroDes, _nrRiga,
                                        _dataMovDal, _dataMovAl, _clienti, _agenti, _causali,
                                        _specie, _varieta, _prodotti, _categorie, _categcommerciali, _causali_trasp, _rapportiCOntabili, _nazioniFatturazione,
                                        _report:=report, _includiVenditeSenzaClienteInt:=includiCorrispettivi)

            If _gestioneGruppiMerce AndAlso report <> Report_Pdf_Acquisto AndAlso report <> Report_Pdf_Vendita Then
                GruppiMerce_SqlCancellaTabellaTempDefault(objParametri, StrSQL)
            End If

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return StrSQL.ToString()


    End Function

    Private Function Crea_Stringua_Query_Selezione_CTE(ByVal livelli As List(Of String))

        Dim strSql As New StringBuilder
        Dim strSqlCampiRaggruppamento As New StringBuilder

        strSql.AppendLine(" piva, Anno_Movimento, Mese_Movimento, ")
        strSql.AppendLine(" Esercizio,mese_qta, ")
        strSql.AppendLine(" Mese_Imponibile_Netto, Mese_Importo, Mese_Provvigione, ")
        strSql.AppendLine(Crea_Stringa_Sql_Select_Campi_Raggruppamento(livelli))
        strSql.Append(" qta, Imponibile_Netto, Importo, provvigione, Unita_Misura_Sigla ")

        Return strSql.ToString

    End Function


    Private Function Crea_Stringa_Pivot_Selezione_Query_ReportVendite_Due_Valori(
                                        ByVal tipoValore As Integer,
                                        ByVal livelli As List(Of String),
                                        ByVal dataDal As String,
                                        ByVal dataAl As String,
                                        ByVal meseInizio As Integer,
                                        ByVal meseFine As Integer) As String

        Dim strSql As New StringBuilder
        Dim strSqlCampiRaggruppamento As New StringBuilder

        Dim prefissoColonnaMese As String = Dammi_Prefisso_Colonna_Mese_Per_TipoValore(tipoValore)

        ' -------------------------------------------------------------------------
        ' UPPER QUERY
        ' -------------------------------------------------------------------------
        strSql.AppendLine("-- UPPER QUERY")
        strSql.AppendLine("Select ")
        strSql.AppendLine(" piva, Anno_Movimento, 0 as TipoRecord, Esercizio, ")

        Dim campiRag As Tuple(Of String, String)
        For i As Integer = 0 To livelli.Count() - 1
            campiRag = OttieniCampiDaLivelloRaggruppamento(livelli(i), i + 1, True, tipoValore)
            If Not String.IsNullOrEmpty(campiRag.Item1) Then
                strSqlCampiRaggruppamento.AppendLine(campiRag.Item1 & ", ")
            End If
            If Not String.IsNullOrEmpty(campiRag.Item2) Then
                strSqlCampiRaggruppamento.AppendLine(campiRag.Item2 & ", ")
            End If
        Next
        Dim strSqlCampiRag = strSqlCampiRaggruppamento.ToString()
        strSql.AppendLine(strSqlCampiRag)

        Dim tuttiMesi = MonthsBetween(Convert.ToDateTime(dataDal), Convert.ToDateTime(dataAl)).ToList()
        Dim piuEsercizi As Boolean = tuttiMesi.Count() > 12

        Dim mesiAnnoPrincipale As List(Of Tuple(Of Integer, Integer))
        If piuEsercizi Then
            mesiAnnoPrincipale = tuttiMesi.Skip(12).Take(12).ToList()
        Else
            mesiAnnoPrincipale = tuttiMesi
        End If

        'dettaglio prendo stessi campi ultimo livello
        Dim ultimoLivello = livelli.LastOrDefault(Function(s) s.ToLower() <> "empty")
        campiRag = OttieniCampiDaLivelloRaggruppamento(ultimoLivello, livelli.Count(), False, tipoValore)
        If Not String.IsNullOrEmpty(campiRag.Item1) Then
            strSql.AppendLine(String.Format("COALESCE({0}, '') as {1}", campiRag.Item1, "CodDet, "))
        End If
        If Not String.IsNullOrEmpty(campiRag.Item2) Then
            Dim campoDescrizione = campiRag.Item2
            If ultimoLivello.ToLower() = "prodotto" AndAlso tipoValore = 0 Then
                campoDescrizione = " COALESCE(cast (COALESCE(referenza_descr COLLATE Latin1_General_CI_AS,'') as varchar(MAX) ) + ' (' + cast(coalesce(Unita_MIsura_Sigla,'') as varchar) + ')' , '') "
            End If
            strSql.AppendLine(String.Format("COALESCE({0}, '') as {1}", campoDescrizione, "DesDet, "))
        End If


        ' TopoValore1
        For i As Integer = 1 To 12

            Dim colMese As String = " SUM({0}{1}) as VAL_{2} "
            Dim meseEffettivo = mesiAnnoPrincipale(i - 1).Item1.ToString().PadLeft(2, "0")

            Dim mese = mesiAnnoPrincipale(i - 1).Item1
            If mese >= meseInizio AndAlso mese <= meseFine Then
                strSql.Append(String.Format(colMese, prefissoColonnaMese, meseEffettivo, i.ToString().PadLeft(2, "0")))
            Else
                strSql.Append(String.Format(colMese, "0", "", i.ToString().PadLeft(2, "0")))
            End If
            strSql.Append(", ")

        Next
        strSql.Append(" Unita_Misura_Sigla ")

        ' -------------------------------------------------------------------------
        ' MIDDLE QUERY
        ' -------------------------------------------------------------------------
        strSql.Append(Environment.NewLine)
        strSql.AppendLine("-- MIDDLE QUERY")
        strSql.AppendLine(" from ( ")
        strSql.AppendLine(" Select piva, Anno_Movimento, Esercizio, ")
        strSql.AppendLine(Crea_Stringa_Sql_Select_Campi_Raggruppamento(livelli))

        ' TipoValore1
        For i As Integer = 1 To 12
            Dim colMese As String = " ISNULL([{0}{1}], 0) as {0}{1} "
            strSql.Append(String.Format(colMese, prefissoColonnaMese, i.ToString().PadLeft(2, "0")))
            strSql.Append(", ")
        Next
        strSql.Append(" Unita_Misura_Sigla ")
        strSql.AppendLine(" from Report_Vendite  ")

        Return strSql.ToString()

    End Function

    Private Function Crea_Stringa_Pivot_Selezione_Query_ReportVendite(
                                        ByVal tipoValore As Integer,
                                        ByVal tipoValore2 As Integer,
                                        ByVal livelli As List(Of String),
                                        ByVal dataDal As String,
                                        ByVal dataAl As String,
                                        ByVal meseInizio As Integer,
                                        ByVal meseFine As Integer) As String

        Dim strSql As New StringBuilder
        Dim strSqlCampiRaggruppamento As New StringBuilder

        Dim prefissoColonnaMese As String = Dammi_Prefisso_Colonna_Mese_Per_TipoValore(tipoValore)

        ' -------------------------------------------------------------------------
        ' UPPER QUERY
        ' -------------------------------------------------------------------------
        strSql.AppendLine("-- UPPER QUERY")
        strSql.AppendLine("Select ")
        strSql.AppendLine(" piva, Anno_Movimento, 0 as TipoRecord, Esercizio, ")

        Dim campiRag As Tuple(Of String, String)
        For i As Integer = 0 To livelli.Count() - 1
            campiRag = OttieniCampiDaLivelloRaggruppamento(livelli(i), i + 1, True, tipoValore)
            If Not String.IsNullOrEmpty(campiRag.Item1) Then
                strSqlCampiRaggruppamento.AppendLine(campiRag.Item1 & ", ")
            End If
            If Not String.IsNullOrEmpty(campiRag.Item2) Then
                strSqlCampiRaggruppamento.AppendLine(campiRag.Item2 & ", ")
            End If
        Next
        Dim strSqlCampiRag = strSqlCampiRaggruppamento.ToString()
        strSql.AppendLine(strSqlCampiRag)

        Dim tuttiMesi = MonthsBetween(Convert.ToDateTime(dataDal), Convert.ToDateTime(dataAl)).ToList()
        Dim piuEsercizi As Boolean = tuttiMesi.Count() > 12

        Dim mesiAnnoPrincipale As List(Of Tuple(Of Integer, Integer))
        If piuEsercizi Then
            mesiAnnoPrincipale = tuttiMesi.Skip(12).Take(12).ToList()
        Else
            mesiAnnoPrincipale = tuttiMesi
        End If

        'dettaglio prendo stessi campi ultimo livello
        Dim ultimoLivello = livelli.LastOrDefault(Function(s) s.ToLower() <> "empty")
        campiRag = OttieniCampiDaLivelloRaggruppamento(ultimoLivello, livelli.Count(), False, tipoValore)
        If Not String.IsNullOrEmpty(campiRag.Item1) Then
            strSql.AppendLine(String.Format("COALESCE({0}, '') as {1}", campiRag.Item1, "CodDet, "))
        End If
        If Not String.IsNullOrEmpty(campiRag.Item2) Then
            Dim campoDescrizione = campiRag.Item2
            If ultimoLivello.ToLower() = "prodotto" AndAlso tipoValore = 0 Then
                campoDescrizione = " COALESCE(cast (COALESCE(referenza_descr COLLATE Latin1_General_CI_AS,'') as varchar(MAX) ) + ' (' + cast(coalesce(Unita_MIsura_Sigla,'') as varchar) + ')' , '') "
            End If
            strSql.AppendLine(String.Format("COALESCE({0}, '') as {1}", campoDescrizione, "DesDet, "))
        End If


        For i As Integer = 1 To 12

            Dim colMese As String = " SUM({0}{1}) as VAL_{2} "
            Dim meseEffettivo = mesiAnnoPrincipale(i - 1).Item1.ToString().PadLeft(2, "0")

            Dim mese = mesiAnnoPrincipale(i - 1).Item1
            If mese >= meseInizio AndAlso mese <= meseFine Then
                strSql.Append(String.Format(colMese, prefissoColonnaMese, meseEffettivo, i.ToString().PadLeft(2, "0")))
            Else
                strSql.Append(String.Format(colMese, "0", "", i.ToString().PadLeft(2, "0")))
            End If
            strSql.Append(", ")

        Next
        For i As Integer = 1 To 12
            Dim colMese As String = " SUM({0}{1}) as VAL_A_{2} "
            Dim meseEffettivo = mesiAnnoPrincipale(i - 1).Item1.ToString().PadLeft(2, "0")

            Dim mese = mesiAnnoPrincipale(i - 1).Item1
            strSql.Append(String.Format(colMese, prefissoColonnaMese, meseEffettivo, i.ToString().PadLeft(2, "0")))
            strSql.Append(", ")
        Next

        strSql.Append(" Unita_Misura_Sigla ")

        ' -------------------------------------------------------------------------
        ' MIDDLE QUERY
        ' -------------------------------------------------------------------------
        strSql.Append(Environment.NewLine)
        strSql.AppendLine("-- MIDDLE QUERY")
        strSql.AppendLine(" from ( ")
        strSql.AppendLine(" Select piva, Anno_Movimento, Esercizio, ")
        strSql.AppendLine(Crea_Stringa_Sql_Select_Campi_Raggruppamento(livelli))

        ' TipoValore1
        For i As Integer = 1 To 12
            Dim colMese As String = " ISNULL([{0}{1}], 0) as {0}{1} "
            strSql.Append(String.Format(colMese, prefissoColonnaMese, i.ToString().PadLeft(2, "0")))
            strSql.Append(", ")
        Next
        strSql.Append(" Unita_Misura_Sigla ")

        ' -------------------------------------------------------------------------
        ' LOWER QUERY
        ' -------------------------------------------------------------------------
        strSql.Append(Environment.NewLine)
        strSql.AppendLine("-- LOWER QUERY")
        strSql.AppendLine(" from ( ")
        strSql.AppendLine(" Select piva, Anno_Movimento, Mese_Movimento, ")
        strSql.AppendLine(" CASE ")

        ' Esercizio 1
        Dim mesiAnniEsercizio = (From m In tuttiMesi.Take(12)
                                 Order By m.Item2 Ascending
                                 Group By anno = m.Item2
                                      Into mesi = Group, Count()
                                 Order By anno).ToList()

        For Each rag In mesiAnniEsercizio
            Dim descrizioneEsercizio = If(piuEsercizi, "Secondario", "Primario")
            Dim mesi = rag.mesi.Select(Function(t) "'" & t.Item1.ToString().PadLeft(2, "0") & "'")
            Dim strInMesi As String = "IN (" & String.Join(",", mesi) & ") "
            strSql.Append("when Anno_Movimento = " & rag.anno.ToString() & " and MeseNoAnno " & strInMesi & " THEN '" & descrizioneEsercizio & "' ")
        Next

        ' Esercizio 2
        If piuEsercizi Then
            mesiAnniEsercizio = (From m In tuttiMesi.Skip(12).Take(12)
                                 Order By m.Item2 Ascending
                                 Group By anno = m.Item2
                                          Into mesi = Group, Count()
                                 Order By anno).ToList()
        Else
            mesiAnniEsercizio.Clear()
        End If

        If piuEsercizi Then
            For Each rag In mesiAnniEsercizio
                Dim mesi = rag.mesi.Select(Function(t) "'" & t.Item1.ToString().PadLeft(2, "0") & "'")
                Dim strInMesi As String = "IN (" & String.Join(",", mesi) & ") "
                strSql.Append("when Anno_Movimento = " & rag.anno.ToString() & " and MeseNoAnno " & strInMesi & " THEN 'Primario' ")
            Next
        End If

        strSql.AppendLine(" end as Esercizio, ")
        strSql.AppendLine(" 'q' + MeseNoAnno as mese_qta, ")

        strSql.AppendLine(" 'in' + MeseNoAnno as Mese_Imponibile_Netto, ")
        strSql.AppendLine(" 'imp' + MeseNoAnno as Mese_Importo, 'pro' + MeseNoAnno as Mese_Provvigione, ")
        strSql.AppendLine(Crea_Stringa_Sql_Select_Campi_Raggruppamento(livelli))
        strSql.AppendLine(" qta, Imponibile_Netto, Importo, provvigione, ")
        If tipoValore = 0 Then
            strSql.Append(" COALESCE(Unita_Misura_Sigla,'') as Unita_Misura_Sigla ")
        Else
            strSql.Append(" '' AS Unita_Misura_Sigla ")
        End If
        strSql.AppendLine(" from ( ")

        Return strSql.ToString()

    End Function

    Private Function Crea_Stringa_Sql_Select_Campi_Raggruppamento(ByVal livelli As List(Of String)) As String

        Dim strSql As New StringBuilder

        For i As Integer = 0 To livelli.Count() - 1
            Dim campiRag = OttieniCampiDaLivelloRaggruppamento(livelli(i), i + 1, False, 0)
            If Not String.IsNullOrEmpty(campiRag.Item1) Then
                strSql.AppendLine(campiRag.Item1 & ", ")
            End If
            If Not String.IsNullOrEmpty(campiRag.Item2) AndAlso campiRag.Item1 <> campiRag.Item2 Then
                strSql.AppendLine(campiRag.Item2 & ", ")
            End If
        Next

        Return strSql.ToString()

    End Function

    Private Function Dammi_Prefisso_Colonna_Mese_Per_TipoValore(ByVal tipoValore As Integer) As String

        Dim prefissoColonnaMese As String

        Select Case tipoValore
            Case 1
                prefissoColonnaMese = "in"
            Case 2
                prefissoColonnaMese = "Imp"
            Case 3
                prefissoColonnaMese = "pro"
            Case Else
                prefissoColonnaMese = "q"
        End Select

        Return prefissoColonnaMese

    End Function
    Private Function Crea_Stringa_Raggruppamento_Query_ReportVendite(
                                                                    ByVal tipoValore As Integer,
                                                                    ByVal livelli As List(Of String)) As String

        Dim strSql As New StringBuilder
        Dim strSqlCampiRaggruppamento As New StringBuilder

        Dim prefissoColonnaMese As String = Dammi_Prefisso_Colonna_Mese_Per_TipoValore(tipoValore)
        Dim campoDaMostrare As String = Dammi_Nome_Campo_Da_Mostrare(tipoValore)

        Dim stringSqlFor = "([{0}01], [{0}02],[{0}03], [{0}04],[{0}05], [{0}06],[{0}07], [{0}08],[{0}09], [{0}10],[{0}11], [{0}12])"
        strSql.AppendLine(") T1 ")
        strSql.AppendLine(" ) T2  ")
        strSql.AppendLine(" Pivot (   ")
        strSql.AppendLine(" sum(" & campoDaMostrare & ") For mese_" & campoDaMostrare & " In " & String.Format(stringSqlFor, prefissoColonnaMese) & " ) AS Pivot_" & campoDaMostrare)
        strSql.AppendLine(" ) ultima ")
        strSql.AppendLine(" group by  ")
        strSql.AppendLine(" piva, ")
        strSql.AppendLine(Crea_Stringa_Sql_Select_Campi_Raggruppamento(livelli))
        strSql.AppendLine(" Esercizio, Anno_Movimento, Unita_Misura_Sigla")

        Return strSql.ToString()

    End Function

    Private Function Crea_Stringa_Raggruppamento_Query_ReportVendite_Due_Valori(
                                                                    ByVal tipoValore As Integer,
                                                                    ByVal livelli As List(Of String)) As String

        Dim strSql As New StringBuilder
        Dim strSqlCampiRaggruppamento As New StringBuilder

        Dim prefissoColonnaMese As String = Dammi_Prefisso_Colonna_Mese_Per_TipoValore(tipoValore)
        Dim campoDaMostrare As String = Dammi_Nome_Campo_Da_Mostrare(tipoValore)

        Dim stringSqlFor = "([{0}01], [{0}02],[{0}03], [{0}04],[{0}05], [{0}06],[{0}07], [{0}08],[{0}09], [{0}10],[{0}11], [{0}12])"
        strSql.AppendLine(" Pivot (   ")
        strSql.AppendLine(" sum(" & campoDaMostrare & ") For mese_" & campoDaMostrare & " In " & String.Format(stringSqlFor, prefissoColonnaMese) & " ) AS Pivot_" & campoDaMostrare)
        strSql.AppendLine(" ) ultima ")
        strSql.AppendLine(" group by  ")
        strSql.AppendLine(" piva, ")
        strSql.AppendLine(Crea_Stringa_Sql_Select_Campi_Raggruppamento(livelli))
        strSql.AppendLine(" Esercizio, Anno_Movimento, Unita_Misura_Sigla")

        Return strSql.ToString()

    End Function

    Private Function Dammi_Nome_Campo_Da_Mostrare(ByVal tipoValore As Integer) As String

        Dim campoDaMostrare As String

        Select Case tipoValore
            Case 1
                campoDaMostrare = "Imponibile_Netto"
            Case 2
                campoDaMostrare = "Importo"
            Case 3
                campoDaMostrare = "provvigione"
            Case Else
                campoDaMostrare = "qta"
        End Select

        Return campoDaMostrare

    End Function

    Public Function Precheck_Righe_Report_Vendite(ByVal piva As String, ByVal report As String, ByVal cubo As Integer,
                                    ByVal docNumeroSin As String, ByVal docNumero As Integer,
                                    ByVal docNumeroDes As String, ByVal nrRiga As String,
                                    ByVal dataMovDal As String, ByVal dataMovAl As String,
                                    ByVal clienti As String, ByVal agenti As String, ByVal causali As String,
                                    ByVal specie As String, ByVal varieta As String,
                                    ByVal prodotti As String, ByVal categorie As String, ByVal categcommerciali As String, ByVal causali_trasp As String,
                                    ByVal xOrderBy As String, ByVal tipoValore As String, ByVal livelli As String,
                                    ByVal rapportiCOntabili As String, ByVal nazioniFatturazione As String,
                                    ByVal tipoValore2 As String, ByVal includiCorrispettivi As Boolean,
                                    ByRef objParametri As AgronicaCoreParametri, ByRef objParametriUtenti As AgronicaCoreParametri) As DataTable

        Dim strSql As New StringBuilder

        If report = Report_Ordini_Vendita OrElse report = Report_Vendite Then
            report = Report_Pdf_Vendita
        End If
        If report = Report_Ordini_Acquisto OrElse report = Report_Acquisti Then
            report = Report_Pdf_Acquisto
        End If

        Dim queryBaseReportVendite = Crea_Query_Report_Vendite(
                   piva,
                   report,
                   False,
                   docNumeroSin, docNumero, docNumeroDes,
                   nrRiga,
                   dataMovDal, dataMovAl,
                   clienti,
                   agenti,
                   causali,
                   specie,
                   varieta,
                   prodotti,
                   categorie,
                   categcommerciali,
                   causali_trasp,
                   rapportiCOntabili,
                   nazioniFatturazione,
                   "",
                   tipoValore,
                   tipoValore2, includiCorrispettivi,
                   objParametri, objParametriUtenti)

        If _gestioneGruppiMerce Then
            GruppiMerce_SqlCreaTabellaTempDefault(objParametri, strSql)
        End If

        strSql.AppendLine(" select distinct ")
        strSql.AppendLine("  COALESCE(querybase.Referenza_Codice, '') as Referenza_Codice ")
        strSql.AppendLine(", COALESCE(querybase.Referenza_Descr, '') as Referenza_Descr from ( ")
        strSql.Append(queryBaseReportVendite)
        strSql.Append(" ) querybase ")
        strSql.Append("  where qta = 0 ")
        strSql.Append(" group by Referenza_Codice, Referenza_Descr")

        If _gestioneGruppiMerce Then
            GruppiMerce_SqlCancellaTabellaTempDefault(objParametri, strSql)
        End If

        Dim dati = Leggi_Righe_Report_Vendite(strSql.ToString, "", "", "", objParametri)

        Return dati

    End Function

    ''' <summary>
    ''' Utilizzata esclusivamente per la generazione dei report PDF
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="report"></param>
    ''' <param name="cubo"></param>
    ''' <param name="docNumeroSin"></param>
    ''' <param name="docNumero"></param>
    ''' <param name="docNumeroDes"></param>
    ''' <param name="nrRiga"></param>
    ''' <param name="dataMovDal"></param>
    ''' <param name="dataMovAl"></param>
    ''' <param name="clienti"></param>
    ''' <param name="agenti"></param>
    ''' <param name="causali"></param>
    ''' <param name="specie"></param>
    ''' <param name="varieta"></param>
    ''' <param name="prodotti"></param>
    ''' <param name="categorie"></param>
    ''' <param name="categcommerciali"></param>
    ''' <param name="causali_trasp"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="tipoValore"></param>
    ''' <param name="livelli"></param>
    ''' <param name="rapportiContabili"></param>
    ''' <param name="nazioniFatturazione"></param>
    ''' <param name="tipoValore2"></param>
    ''' <param name="meseInizio"></param>
    ''' <param name="meseFine"></param>
    ''' <param name="tipo_report"></param>
    ''' <param name="includiCorrispettivi"></param>
    ''' <param name="objParametri"></param>
    ''' <param name="objParametriUtenti"></param>
    ''' <returns></returns>
    Public Function Leggi_Righe_Report_Vendite(ByVal piva As String, ByVal report As String, ByVal cubo As Integer,
                                    ByVal docNumeroSin As String, ByVal docNumero As Integer,
                                    ByVal docNumeroDes As String, ByVal nrRiga As String,
                                    ByVal dataMovDal As String, ByVal dataMovAl As String,
                                    ByVal clienti As String, ByVal agenti As String, ByVal causali As String,
                                    ByVal specie As String, ByVal varieta As String,
                                    ByVal prodotti As String, ByVal categorie As String, ByVal categcommerciali As String, ByVal causali_trasp As String,
                                    ByVal xOrderBy As String, ByVal tipoValore As String, ByVal livelli As String,
                                    ByVal rapportiContabili As String, ByVal nazioniFatturazione As String,
                                    ByVal tipoValore2 As String,
                                    ByVal meseInizio As Integer, ByVal meseFine As Integer, ByVal tipo_report As String,
                                    ByVal includiCorrispettivi As Boolean,
                                    ByRef objParametri As AgronicaCoreParametri, ByRef objParametriUtenti As AgronicaCoreParametri) As DataTable

        Dim livelliRaggruppamento = livelli.Split("|").ToList()
        livelliRaggruppamento.AddRange({"empty", "empty", "empty"})

        Dim tvalore1 As Integer = CInt(tipoValore)
        Dim tvalore2 As Integer = If(String.IsNullOrEmpty(tipoValore2), -1, CInt(tipoValore2))
        Dim dati As DataTable = Nothing

        If report = Report_Ordini_Vendita OrElse report = Report_Vendite Then
            report = Report_Pdf_Vendita
        End If
        If report = Report_Ordini_Acquisto OrElse report = Report_Acquisti Then
            report = Report_Pdf_Acquisto
        End If

        Dim queryBaseReportVendite = Crea_Query_Report_Vendite(
                   piva,
                   report,
                   False,
                   docNumeroSin, docNumero, docNumeroDes,
                   nrRiga,
                   dataMovDal, dataMovAl,
                   clienti,
                   agenti,
                   causali,
                   specie,
                   varieta,
                   prodotti,
                   categorie,
                   categcommerciali,
                   causali_trasp,
                   rapportiContabili,
                   nazioniFatturazione,
                   "",
                   tipoValore,
                   tipoValore2, includiCorrispettivi,
                   objParametri, objParametriUtenti)

        Select Case CInt(tipo_report)
            Case enum_Tipo_Stampa_Statistica.Statistica_mese_anno_confronto_fra_anni
                dati = Statistica_mese_anno_confronto_fra_anni(tipoValore, tipoValore2, livelliRaggruppamento, dataMovDal, dataMovAl, meseInizio, meseFine,
                                                        queryBaseReportVendite, objParametri)

            Case enum_Tipo_Stampa_Statistica.Statistica_mese_singolo_anno_su_quantità_altro_Valore
                dati = Statistica_mese_singolo_anno_su_quantità_altro_Valore(tipoValore, tipoValore2, livelliRaggruppamento, dataMovDal, dataMovAl, meseInizio, meseFine,
                                                        queryBaseReportVendite, objParametri)

            Case enum_Tipo_Stampa_Statistica.Statistica_anno_con_scostamento_e_previsioni
                dati = Statistica_anno_con_scostamento_e_previsioni(tipoValore, tipoValore2, livelliRaggruppamento, dataMovDal, dataMovAl, meseInizio, meseFine,
                                                        queryBaseReportVendite, objParametri)

        End Select

        Return dati

    End Function

    Private Function Statistica_mese_anno_confronto_fra_anni(ByVal tipoValore As Integer,
                                        ByVal tipoValore2 As Integer,
                                        ByVal livelli As List(Of String),
                                        ByVal dataDal As String,
                                        ByVal dataAl As String,
                                        ByVal meseInizio As Integer,
                                        ByVal meseFine As Integer,
                                        ByVal queryBaseReportVendite As String,
                                        ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim dati As DataTable = Nothing

        Dim queyPivotSelezione = Crea_Stringa_Pivot_Selezione_Query_ReportVendite(tipoValore, tipoValore2,
                                                                                  livelli.Take(3).ToList(),
                                                                                  dataDal, dataAl,
                                                                                  meseInizio, meseFine)

        Dim queryPivotRaggruppamento = Crea_Stringa_Raggruppamento_Query_ReportVendite(tipoValore, livelli.Take(3).ToList())
        Dim queryOrdinamento As String = " order by CodLiv1, CodLiv2, Coddet "

        dati = Leggi_Righe_Report_Vendite(queyPivotSelezione, queryBaseReportVendite, queryPivotRaggruppamento, queryOrdinamento, objParametri)

        Return dati

    End Function

    Private Function Statistica_mese_singolo_anno_su_quantità_altro_Valore(ByVal tipoValore As Integer,
                                        ByVal tipoValore2 As Integer,
                                        ByVal livelli As List(Of String),
                                        ByVal dataDal As String,
                                        ByVal dataAl As String,
                                        ByVal meseInizio As Integer,
                                        ByVal meseFine As Integer,
                                        ByVal queryBaseReportVendite As String,
                                        ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim dati As DataTable = Nothing
        Dim strSql As New StringBuilder

        If _gestioneGruppiMerce Then
            GruppiMerce_SqlCreaTabellaTempDefault(objParametri, strSql)
        End If

        Dim cteTable As String = Crea_CTE_Report_Due_Valori(tipoValore, tipoValore2,
                                                   livelli.Take(3).ToList(),
                                                   dataDal, dataAl, meseInizio, meseFine,
                                                   queryBaseReportVendite)

        strSql.AppendLine(cteTable)

        strSql.AppendLine(" select q1.*, ")
        strSql.AppendLine(" q2.val_01 as Val_01_2,  q2.val_02 as Val_02_2, ")
        strSql.AppendLine(" q2.val_03 as Val_03_2,  q2.val_04 as Val_04_2, ")
        strSql.AppendLine(" q2.val_05 as Val_05_2,  q2.val_06 as Val_06_2, ")
        strSql.AppendLine(" q2.val_07 as Val_07_2,  q2.val_08 as Val_08_2, ")
        strSql.AppendLine(" q2.val_09 as Val_09_2,  q2.val_10 as Val_10_2, ")
        strSql.AppendLine(" q2.val_11 as Val_11_2,  q2.val_12 as Val_12_2 ")
        strSql.AppendLine(" FROM ")
        strSql.AppendLine(" ( ")

        Dim strPivotValore1 As String = Crea_Stringa_Pivot_Selezione_Query_ReportVendite_Due_Valori(tipoValore,
                                                                          livelli.Take(3).ToList(),
                                                                          dataDal, dataAl,
                                                                          meseInizio, meseFine)
        Dim queryPivotRaggruppamento1 = Crea_Stringa_Raggruppamento_Query_ReportVendite_Due_Valori(tipoValore, livelli.Take(3).ToList())

        strSql.Append(strPivotValore1)
        strSql.Append(queryPivotRaggruppamento1)
        strSql.AppendLine(" ) q1")
        strSql.AppendLine(" INNER JOIN ")
        strSql.AppendLine(" ( ")

        Dim strPivotValore2 As String = Crea_Stringa_Pivot_Selezione_Query_ReportVendite_Due_Valori(tipoValore2,
                                                                         livelli.Take(3).ToList(),
                                                                         dataDal, dataAl,
                                                                         meseInizio, meseFine)
        Dim queryPivotRaggruppamento2 = Crea_Stringa_Raggruppamento_Query_ReportVendite_Due_Valori(tipoValore2, livelli.Take(3).ToList())

        strSql.Append(strPivotValore2)
        strSql.AppendLine(queryPivotRaggruppamento2)
        strSql.AppendLine(" ) q2")

        strSql.AppendLine("ON q1.piva = q2.piva ")
        strSql.AppendLine(" And q1.Anno_Movimento = q2.Anno_Movimento ")
        strSql.AppendLine(" And q1.TipoRecord = q2.TipoRecord ")
        'strSql.AppendLine(" And q1.Esercizio = q2.Esercizio ")
        strSql.AppendLine(" And q1.CodLiv1 = q2.CodLiv1 ")
        strSql.AppendLine(" And q1.CodLiv2 = q2.CodLiv2 ")
        strSql.AppendLine(" And q1.CodLiv3 = q2.CodLiv3")
        strSql.AppendLine(" And q1.Unita_Misura_Sigla = q2.Unita_Misura_Sigla")

        Dim queryOrdinamento As String = " order by CodLiv1, CodLiv2, Coddet "
        strSql.AppendLine(queryOrdinamento)

        If _gestioneGruppiMerce Then
            GruppiMerce_SqlCancellaTabellaTempDefault(objParametri, strSql)
        End If

        dati = Leggi_Righe_Report_Vendite_Da_Stringa_Sql_Completa(strSql.ToString, objParametri)

        Return dati

    End Function

    Private Function Statistica_anno_con_scostamento_e_previsioni(ByVal tipoValore As Integer,
                                        ByVal tipoValore2 As Integer,
                                        ByVal livelli As List(Of String),
                                        ByVal dataDal As String,
                                        ByVal dataAl As String,
                                        ByVal meseInizio As Integer,
                                        ByVal meseFine As Integer,
                                        ByVal queryBaseReportVendite As String,
                                        ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim dati As DataTable = Nothing
        Dim strSql As New StringBuilder

        If _gestioneGruppiMerce Then
            GruppiMerce_SqlCreaTabellaTempDefault(objParametri, strSql)
        End If

        strSql.AppendLine("select tutto.piva, tutto.Anno_Movimento, tutto.TipoRecord, tutto.Esercizio, ")
        strSql.AppendLine("tutto.CodLiv1, tutto.DesLiv1 , tutto.CodLiv2, tutto.desliv2, tutto.CodLiv3, tutto.DesLiv3, ")
        strSql.AppendLine(" tutto.CodDet, tutto.DesDet, tutto.unita_misura_sigla, ")

        Dim queryTotali = Crea_Stringa_Query_Totali_X_Anno_Periodo(dataDal, dataAl,
                                                                   meseInizio, meseFine)
        strSql.AppendLine(queryTotali)
        strSql.AppendLine("From ( ")


        Dim queyPivotSelezione = Crea_Stringa_Pivot_Selezione_Query_ReportVendite(tipoValore, tipoValore2,
                                                                                  livelli.Take(3).ToList(),
                                                                                  dataDal, dataAl,
                                                                                  meseInizio, meseFine)
        strSql.AppendLine(queyPivotSelezione)
        strSql.AppendLine(queryBaseReportVendite)
        Dim queryPivotRaggruppamento = Crea_Stringa_Raggruppamento_Query_ReportVendite(tipoValore, livelli.Take(3).ToList())

        strSql.AppendLine(queryPivotRaggruppamento)
        strSql.AppendLine(" ) tutto")

        Dim queryOrdinamento As String = " order by CodLiv1, CodLiv2, Coddet "
        strSql.AppendLine(queryOrdinamento)

        If _gestioneGruppiMerce Then
            GruppiMerce_SqlCancellaTabellaTempDefault(objParametri, strSql)
        End If

        dati = Leggi_Righe_Report_Vendite_Da_Stringa_Sql_Completa(strSql.ToString(), objParametri)

        Return dati



    End Function

    Private Function Crea_Stringa_Query_Totali_X_Anno_Periodo(ByVal dataDal As String,
                                        ByVal dataAl As String,
                                        ByVal meseInizio As Integer,
                                        ByVal meseFine As Integer) As String

        Dim strSql As New StringBuilder

        Dim tuttiMesi = MonthsBetween(Convert.ToDateTime(dataDal), Convert.ToDateTime(dataAl)).ToList()
        Dim piuEsercizi As Boolean = tuttiMesi.Count() > 12

        Dim mesiAnnoPrincipale As List(Of Tuple(Of Integer, Integer))
        If piuEsercizi Then
            mesiAnnoPrincipale = tuttiMesi.Skip(12).Take(12).ToList()
        Else
            mesiAnnoPrincipale = tuttiMesi
        End If

        strSql.Append("( ")
        Dim stringaTotali = ""
        For i As Integer = 1 To 12

            Dim colMese As String = "VAL_{0} "
            Dim mese = mesiAnnoPrincipale(i - 1).Item1
            If mese >= meseInizio AndAlso mese <= meseFine Then
                stringaTotali = stringaTotali + String.Format(colMese, i.ToString().PadLeft(2, "0") + " + ")
            End If

        Next
        stringaTotali = stringaTotali.Remove(stringaTotali.LastIndexOf("+"))
        strSql.AppendLine(stringaTotali)
        strSql.Append(") AS TotalePerPeriodo, ")

        strSql.Append("( ")
        stringaTotali = ""
        For i As Integer = 1 To 12
            Dim colMese As String = "VAL_A_{0} "
            Dim mese = mesiAnnoPrincipale(i - 1).Item1
            stringaTotali = stringaTotali + String.Format(colMese, i.ToString().PadLeft(2, "0") + " + ")
        Next
        stringaTotali = stringaTotali.Remove(stringaTotali.LastIndexOf("+"))
        strSql.AppendLine(stringaTotali)
        strSql.Append(") AS Totale12Mesi ")

        Return strSql.ToString

    End Function

    Private Function Crea_CTE_Report_Due_Valori(ByVal tipoValore As Integer,
                                        ByVal tipoValore2 As Integer,
                                        ByVal livelli As List(Of String),
                                        ByVal dataDal As String,
                                        ByVal dataAl As String,
                                        ByVal meseInizio As Integer,
                                        ByVal meseFine As Integer, ByVal queryBaseReportVendite As String) As String

        Dim strSql As New StringBuilder
        strSql.AppendLine(" WITH Report_Vendite( ")

        strSql.AppendLine(Crea_Stringua_Query_Selezione_CTE(livelli))
        strSql.AppendLine(" )")

        strSql.AppendLine(" AS ")
        strSql.AppendLine(" ( ")
        strSql.Append(Environment.NewLine)
        strSql.AppendLine(" Select piva, Anno_Movimento, Mese_Movimento, ")
        strSql.AppendLine(" CASE ")

        Dim tuttiMesi = MonthsBetween(Convert.ToDateTime(dataDal), Convert.ToDateTime(dataAl)).ToList()
        Dim piuEsercizi As Boolean = tuttiMesi.Count() > 12

        Dim mesiAnnoPrincipale As List(Of Tuple(Of Integer, Integer))
        If piuEsercizi Then
            mesiAnnoPrincipale = tuttiMesi.Skip(12).Take(12).ToList()
        Else
            mesiAnnoPrincipale = tuttiMesi
        End If

        ' Esercizio 1
        Dim mesiAnniEsercizio = (From m In tuttiMesi.Take(12)
                                 Order By m.Item2 Ascending
                                 Group By anno = m.Item2
                                      Into mesi = Group, Count()
                                 Order By anno).ToList()

        For Each rag In mesiAnniEsercizio
            Dim descrizioneEsercizio = IIf(piuEsercizi, "Secondario", "Primario")
            Dim mesi = rag.mesi.Select(Function(t) "'" & t.Item1.ToString().PadLeft(2, "0") & "'")
            Dim strInMesi As String = "IN (" & String.Join(",", mesi) & ") "
            strSql.Append("when Anno_Movimento = " & rag.anno.ToString() & " and MeseNoAnno " & strInMesi & " THEN '" & descrizioneEsercizio & "' ")
        Next

        ' Esercizio 2
        If piuEsercizi Then
            mesiAnniEsercizio = (From m In tuttiMesi.Skip(12).Take(12)
                                 Order By m.Item2 Ascending
                                 Group By anno = m.Item2
                                          Into mesi = Group, Count()
                                 Order By anno).ToList()
        Else
            mesiAnniEsercizio.Clear()
        End If

        If piuEsercizi Then
            For Each rag In mesiAnniEsercizio
                Dim mesi = rag.mesi.Select(Function(t) "'" & t.Item1.ToString().PadLeft(2, "0") & "'")
                Dim strInMesi As String = "IN (" & String.Join(",", mesi) & ") "
                strSql.Append("when Anno_Movimento = " & rag.anno.ToString() & " and MeseNoAnno " & strInMesi & " THEN 'Primario' ")
            Next
        End If

        strSql.AppendLine(" end as Esercizio, ")
        strSql.AppendLine(" 'q' + MeseNoAnno as mese_qta, ")

        strSql.AppendLine(" 'in' + MeseNoAnno as Mese_Imponibile_Netto, ")
        strSql.AppendLine(" 'imp' + MeseNoAnno as Mese_Importo, 'pro' + MeseNoAnno as Mese_Provvigione, ")
        strSql.AppendLine(Crea_Stringa_Sql_Select_Campi_Raggruppamento(livelli))
        strSql.AppendLine(" qta, Imponibile_Netto, Importo, provvigione, ")
        If tipoValore = 0 Then
            strSql.Append(" COALESCE(Unita_Misura_Sigla,'') as Unita_Misura_Sigla ")
        Else
            strSql.Append(" '' AS Unita_Misura_Sigla ")
        End If
        strSql.AppendLine(" from ( ")

        strSql.AppendLine(" -- query base ")
        strSql.Append(queryBaseReportVendite)
        strSql.AppendLine(" ) T1 ")
        strSql.AppendLine(" )")

        Return strSql.ToString()

    End Function

    Public Function MonthsBetween(ByVal startDate As DateTime, ByVal endDate As DateTime) As List(Of Tuple(Of Integer, Integer))

        Dim retVal = New List(Of Tuple(Of Integer, Integer))


        Dim iterator As DateTime
        Dim limit As DateTime

        If endDate > startDate Then
            iterator = New DateTime(startDate.Year, startDate.Month, 1)
            limit = endDate
        Else
            iterator = New DateTime(endDate.Year, endDate.Month, 1)
            limit = startDate
        End If

        Dim dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat

        While iterator <= limit
            retVal.Add(Tuple.Create(iterator.Month, iterator.Year))
            iterator = iterator.AddMonths(1)
        End While

        Return retVal

    End Function

    Private Function Leggi_Righe_Report_Vendite_Da_Stringa_Sql_Completa(ByVal stringaSqlCompleta As String, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Statistiche.()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Try
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stringaSqlCompleta, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT


    End Function

    Private Function Leggi_Righe_Report_Vendite(
        ByVal queryPivotSelect As String,
        ByVal queryBase As String,
        ByVal queryPivotRaggruppamento As String,
        ByVal queryOrdinamento As String,
        ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Statistiche.Leggi_Righe_Report_Vendite()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            If _gestioneGruppiMerce AndAlso queryBase <> "" Then
                GruppiMerce_SqlCreaTabellaTempDefault(objParametri, StrSQL)
            End If

            StrSQL.Append(queryPivotSelect)
            StrSQL.Append(queryBase)
            StrSQL.Append(queryPivotRaggruppamento)
            StrSQL.Append(queryOrdinamento)

            If _gestioneGruppiMerce AndAlso queryBase <> "" Then
                GruppiMerce_SqlCancellaTabellaTempDefault(objParametri, StrSQL)
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

    Private Function OttieniCampiDaLivelloRaggruppamento(ByVal livelloRag As String,
                                                         ByVal indiceLivello As Integer,
                                                         ByVal aliasCampo As Boolean,
                                                         ByVal tipoValore As Integer) As Tuple(Of String, String)

        Dim codCampoLivello As String = ""
        Dim desCampoLivello As String = ""

        Select Case livelloRag.ToLower()
            Case "cliente", "fornitore"
                codCampoLivello = "Soggetto_Piva "
                desCampoLivello = "soggetto_RagioneSociale "
            Case "agente"
                codCampoLivello = "Agente_Piva "
                desCampoLivello = "agente_ragionesociale "
            Case "destinazione", "provenienza"
                codCampoLivello = "destinazione "
                desCampoLivello = "destinazione "
            Case "prodotto"
                codCampoLivello = "Referenza_Codice "
                desCampoLivello = "referenza_descr "
            Case "capo_area"
                codCampoLivello = "capoarea_codice "
                desCampoLivello = "capoarea_ragionesociale "
            Case "categoria_commerciale"
                codCampoLivello = "categoria_commerciale "
                desCampoLivello = "categoria_commerciale "
            Case "categoria_prodotto"
                codCampoLivello = "categoria_prodotto "
                desCampoLivello = "categoria_prodotto "
            Case "stato"
                codCampoLivello = "stato "
                desCampoLivello = "stato "
            Case "stato_dest", "stato_proven"
                codCampoLivello = "stato_dest"
                desCampoLivello = "stato_dest"
            Case "rapporto"
                codCampoLivello = "soggetto_rapporto"
                desCampoLivello = "soggetto_rapporto"
            Case "impresa"
                codCampoLivello = "Piva_Impresa"
                desCampoLivello = "Ragione_Sociale_Impresa"
            Case "empty"
                codCampoLivello = "'' "
                desCampoLivello = "'' "
        End Select

        If String.IsNullOrEmpty(codCampoLivello) AndAlso String.IsNullOrEmpty(desCampoLivello) Then
            Return New Tuple(Of String, String)("", "")
        End If

        If aliasCampo Then
            Dim campoDescrizione = desCampoLivello
            If livelloRag.ToLower() = "prodotto" AndAlso tipoValore = 0 Then
                campoDescrizione = "cast (COALESCE(referenza_descr COLLATE Latin1_General_CI_AS,'') as varchar(MAX) ) + ' (' + cast(coalesce(Unita_MIsura_Sigla,'') as varchar) + ')' "
            End If
            Return New Tuple(Of String, String)(String.Format("COALESCE({0}, '') as {1}", codCampoLivello, "CodLiv" & indiceLivello.ToString()),
                                                    String.Format("COALESCE({0}, '') as {1}", campoDescrizione, "DesLiv" & indiceLivello.ToString()))
        Else
            If livelloRag.ToLower() = "empty" Then
                Return New Tuple(Of String, String)("", "")
            Else
                Return New Tuple(Of String, String)(codCampoLivello, desCampoLivello)
            End If
        End If

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge righe movimenti di vendita per report statistico
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi_Righe_Report_Vendite(ByVal piva As String, ByVal report As String, ByVal cubo As Integer,
                                    ByVal _docNumeroSin As String, ByVal _docNumero As Integer,
                                    ByVal _docNumeroDes As String, ByVal _nrRiga As String,
                                    ByVal _dataMovDal As String, ByVal _dataMovAl As String,
                                    ByVal _dataEvasionePrevDal As String, ByVal _dataEvasionePrevAl As String,
                                    ByVal _clienti As String, ByVal _agenti As String, ByVal _causali As String,
                                    ByVal _specie As String, ByVal _varieta As String,
                                    ByVal _prodotti As String, ByVal _categorie As String, ByVal _categcommerciali As String, ByVal _causali_trasp As String,
                                    ByVal _rapportiContabili As String,
                                    ByVal _nazioniFatturazione As String,
                                    ByVal _includiCorrispettivi As Boolean,
                                    ByVal xOrderBy As String, ByRef objParametri As AgronicaCoreParametri, ByRef objParametriUtenti As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Statistiche.Leggi_Righe_Report_Vendite()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim sortedDT As DataTable

        Try

            StrSQL.Length = 0

            If cubo AndAlso report = Report_Vendite Then

                Dim esisteTabella = Esiste_Report(Report_Vendite, objParametri)
                If Not esisteTabella Then
                    Dim scrivi As New Statistiche_W
                    Dim risposta = scrivi.Aggiorna_Cubo_Report_Vendite(piva, _dataMovDal, _dataMovAl, objParametri)
                End If

                ' Query Principale
                StrSQL.AppendLine(" SELECT * FROM Report_Vendite ")

            Else

                Dim QuerySQL = Leggi_Query_Report_Vendite(piva, report, Nothing, Nothing, " Select * from (", objParametri)

                'If report = Report_Ordini_Vendita Then
                StrSQL.Append(QuerySQL & " ) AS Report_Vendite ")
                'Else
                'StrSQL.Append("SELECT * FROM ( " & QuerySQL & " ) AS Report_Vendite ")
                '   End If

            End If

            '''''''''''''' GRIGLIA '''''''''''''''

            ' Applica Filtri
            Filtra_Query_Report_Vendite(StrSQL, objParametri, objParametriUtenti, piva,
                                        _docNumeroSin, _docNumero, _docNumeroDes, _nrRiga,
                                        _dataMovDal, _dataMovAl, _clienti, _agenti, _causali,
                                        _specie, _varieta, _prodotti, _categorie, _categcommerciali, _causali_trasp,
                                        _rapportiContabili, _nazioniFatturazione, _dataEvasionePrevDal, _dataEvasionePrevAl, _includiCorrispettivi, report)

            'If xOrderBy <> "" Then
            '    StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            'Else
            '    StrSQL.AppendLine(" ORDER BY Data_Movimento, Numero_Movimento, Riga ")
            'End If

            If _gestioneGruppiMerce AndAlso report <> Report_Pdf_Acquisto AndAlso report <> Report_Pdf_Vendita Then
                GruppiMerce_SqlCancellaTabellaTempDefault(objParametri, StrSQL)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Dim dv As DataView = DT.DefaultView
            dv.Sort = If(String.IsNullOrEmpty(xOrderBy), "Data_Movimento DESC, Numero_Movimento, Riga", xOrderBy)
            sortedDT = dv.ToTable()

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return sortedDT

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge righe movimenti ddt di vendita per uscite lavorazioni
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi_Righe_DDT_Vendite(ByVal piva As String, ByVal report As String, ByVal da_lavorare As Boolean,
                                    ByVal _dataMovDal As String, ByVal _dataMovAl As String,
                                    ByVal _clienti As String, ByVal _agenti As String, ByVal _causali As String,
                                    ByVal _specie As String, ByVal _varieta As String,
                                    ByVal _prodotti As String, ByVal _categorie As String, ByVal _categcommerciali As String, ByVal _causali_trasp As String,
                                    ByVal xOrderBy As String, ByRef objParametri As AgronicaCoreParametri, ByRef objParametriUtenti As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Statistiche.Leggi_Righe_DDT_Vendite()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim QuerySQL = Leggi_Query_Report_Vendite(piva,
                                                      report,
                                                      Nothing,
                                                      Nothing,
                                                      " Select * from (",
                                                      objParametri)

            ' Query Principale
            StrSQL.Append(QuerySQL & " ) AS Report_Vendite ")

            ' Applica Filtri
            Filtra_Query_Report_Vendite(StrSQL, objParametri, objParametriUtenti, piva,
                                        "", 0, "", "",
                                        _dataMovDal, _dataMovAl, _clienti, _agenti, _causali,
                                        _specie, _varieta, _prodotti, _categorie, _categcommerciali, _causali_trasp, _daLavorare:=da_lavorare)

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Data_Movimento, Numero_Movimento, Riga ")
            End If

            If _gestioneGruppiMerce AndAlso report <> Report_Pdf_Acquisto AndAlso report <> Report_Pdf_Vendita Then
                GruppiMerce_SqlCancellaTabellaTempDefault(objParametri, StrSQL)
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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge righe movimenti ordini di vendita per uscite ordini lavorazione
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi_Righe_Ordini_Vendita(ByVal piva As String,
                                               ByVal report As String,
                                               ByVal da_lavorare As Boolean,
                                               ByVal _dataMovDal As String,
                                               ByVal _dataMovAl As String,
                                               ByVal _clienti As String,
                                               ByVal _agenti As String,
                                               ByVal _causali As String,
                                               ByVal _specie As String,
                                               ByVal _varieta As String,
                                               ByVal _prodotti As String,
                                               ByVal _categorie As String,
                                               ByVal _categcommerciali As String,
                                               ByVal _causali_trasp As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreParametri,
                                               ByRef objParametriUtenti As AgronicaCoreParametri
                                               ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Statistiche.Leggi_Righe_DDT_Vendite()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Dim QuerySQL = Leggi_Query_Report_Vendite(piva,
                                                      report,
                                                      Nothing,
                                                      Nothing,
                                                      " Select * from (",
                                                      objParametri)

            ' Query Principale
            StrSQL.Append(QuerySQL & " ) AS Report_Vendite ")

            ' Applica Filtri
            Filtra_Query_Report_Vendite(StrSQL, objParametri, objParametriUtenti, piva,
                                        "", 0, "", "",
                                        _dataMovDal, _dataMovAl, _clienti, _agenti, _causali,
                                        _specie, _varieta, _prodotti, _categorie,
                                        _categcommerciali, _causali_trasp, _daLavorare:=da_lavorare)

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Data_Movimento, Numero_Movimento, Riga ")
            End If

            If _gestioneGruppiMerce AndAlso report <> Report_Pdf_Acquisto AndAlso report <> Report_Pdf_Vendita Then
                GruppiMerce_SqlCancellaTabellaTempDefault(objParametri, StrSQL)
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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge grafico report vendite
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi_Grafico_Report_Vendite(ByVal piva As String, ByVal tipoReport As String, ByVal cubo As Boolean, ByVal tipo As String,
                                                 ByVal _dataMovDal As String, ByVal _dataMovAl As String,
                                                 ByVal _clienti As String, ByVal _agenti As String, ByVal _causali As String,
                                                 ByVal _specie As String, ByVal _varieta As String,
                                                 ByVal _prodotti As String,
                                                 ByVal _categorie As String,
                                                 ByVal _categcommerciali As String,
                                                 ByVal _causali_trasp As String,
                                                 ByVal _includiCorrispettivi As Boolean,
                                                 ByRef objParametri As AgronicaCoreParametri,
                                                 ByRef objParametriUtenti As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Statistiche.Leggi_Grafico_Report_Vendite()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim selectPrincipale As String = ""

        Try


            If tipo = "Anno" Then
                selectPrincipale = "SELECT Anno_Movimento AS Anno, SUM(Imponibile_Netto) AS Imponibile_Netto, SUM(Importo) AS Importo FROM ( "
            ElseIf tipo = "Regione" Then
                selectPrincipale = "SELECT Regione, SUM(Imponibile_Netto) AS Imponibile_Netto, SUM(Importo) AS Importo FROM ( "
            End If

            If cubo Then

                ' Query Principale
                StrSQL.AppendLine(" SELECT * FROM Report_Vendite ")

            Else

                Dim QuerySQL = Leggi_Query_Report_Vendite(piva, tipoReport, Nothing, Nothing, selectPrincipale, objParametri)

                ' Query Principale
                'StrSQL.Append("SELECT * FROM ( " & QuerySQL & " ) AS Report_Vendite ")
                StrSQL.Append(QuerySQL & " ) AS Report_Vendite ")

            End If

            ' Applica Filtri
            Filtra_Query_Report_Vendite(StrSQL, objParametri, objParametriUtenti,
                                        piva, "", 0, "", "",
                                        _dataMovDal, _dataMovAl,
                                        _clienti, _agenti, _causali, _specie, _varieta, _prodotti,
                                        _categorie, _categcommerciali, _causali_trasp,
                                        Nothing, Nothing, Nothing, Nothing,
                                        _includiCorrispettivi)

            If tipo = "Anno" Then
                StrSQL.AppendLine(" GROUP BY Anno_Movimento ORDER BY Anno_Movimento")
            ElseIf tipo = "Regione" Then
                StrSQL.AppendLine(" GROUP BY Regione HAVING SUM(Imponibile_Netto)>0 ORDER BY Regione")
            End If

            If _gestioneGruppiMerce AndAlso tipoReport <> Report_Pdf_Acquisto AndAlso tipoReport <> Report_Pdf_Vendita Then
                GruppiMerce_SqlCancellaTabellaTempDefault(objParametri, StrSQL)
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

    Private Function CreaTabellaTemp_FiltroProgetti() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#TempProgetti') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #TempProgetti ( ")
        stb.AppendLine("        Progetto_Cod int NULL")
        stb.AppendLine("    )")
        stb.AppendLine()
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

    Private Function EliminaTabellaTemp_FiltroProgetti() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF NOT OBJECT_ID('tempdb.dbo.#TempProgetti') IS NULL BEGIN ")
        stb.AppendLine("    DROP TABLE #TempProgetti ")
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

    Private Iterator Function ChunkBy(Of TSource)(ByVal source As IEnumerable(Of TSource), ByVal chunkSize As Integer) As IEnumerable(Of IEnumerable(Of TSource))
        While source.Any()
            Yield source.Take(chunkSize)
            source = source.Skip(chunkSize)
        End While
    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge rilievi stime di produzione
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi_Rilievi_Stime_Produzione(ByVal filtroProgetti As String,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                   Optional ByVal data As Date = AGRODATAINIZIO) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Statistiche.Leggi_Rilievi_Stime_Produzione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("   imprese_progetti.Piva, imprese_progetti.Sa_Cod, imprese_progetti.Appezza, imprese_progetti.Id_Reg, imprese_progetti.Progetto_Cod, ")
            StrSQL.AppendLine("   agenda.id_agenda, mov_destinazioni.qta as resa_rilievo, movimenti.Data_Movimento as data_rilievo, ")
            StrSQL.AppendLine("   IIF(ud.Cognome != '', ud.Cognome + ' ' + ud.Nome, ud.Rag_Soc) as utente_rilievo, ")
            StrSQL.AppendLine("   ud.UserName, ud.CodFisc, ud.Nome, ud.Cognome, ud.Rag_Soc ")
            StrSQL.AppendLine(" FROM movimenti_dettagli ")
            StrSQL.AppendLine("   INNER JOIN movimenti ")
            StrSQL.AppendLine("      ON movimenti_dettagli.piva = movimenti.piva ")
            StrSQL.AppendLine("      AND movimenti_dettagli.sa_cod = movimenti.sa_cod ")
            StrSQL.AppendLine("      AND movimenti_dettagli.id_agenda = movimenti.id_agenda ")
            StrSQL.AppendLine("      AND movimenti_dettagli.id_mov = movimenti.id_mov ")
            StrSQL.AppendLine("   INNER JOIN agenda ")
            StrSQL.AppendLine("      ON movimenti.piva = agenda.piva ")
            StrSQL.AppendLine("      AND movimenti.sa_cod = agenda.sa_cod ")
            StrSQL.AppendLine("      AND movimenti.id_agenda = agenda.id_agenda ")
            StrSQL.AppendLine("   INNER JOIN mov_destinazioni ")
            StrSQL.AppendLine("      ON movimenti_dettagli.piva = mov_destinazioni.piva ")
            StrSQL.AppendLine("      AND movimenti_dettagli.sa_cod = mov_destinazioni.sa_cod ")
            StrSQL.AppendLine("      AND movimenti_dettagli.id_agenda = mov_destinazioni.id_agenda ")
            StrSQL.AppendLine("      AND movimenti_dettagli.id_mov = mov_destinazioni.id_mov ")
            StrSQL.AppendLine("      AND movimenti_dettagli.id_mov_det = mov_destinazioni.id_mov_det ")
            StrSQL.AppendLine("   INNER JOIN mov_dettaglio_tecnico ")
            StrSQL.AppendLine("      ON movimenti_dettagli.piva = mov_dettaglio_tecnico.piva ")
            StrSQL.AppendLine("      AND movimenti_dettagli.sa_cod = mov_dettaglio_tecnico.sa_cod ")
            StrSQL.AppendLine("      AND movimenti_dettagli.id_agenda = mov_dettaglio_tecnico.id_agenda ")
            StrSQL.AppendLine("      AND movimenti_dettagli.id_mov = mov_dettaglio_tecnico.id_mov ")
            StrSQL.AppendLine("      AND movimenti_dettagli.id_mov_det = mov_dettaglio_tecnico.id_mov_det ")
            StrSQL.AppendLine("   INNER JOIN Imprese_Progetti ")
            StrSQL.AppendLine("      ON Imprese_Progetti.Piva = mov_destinazioni.Piva ")
            StrSQL.AppendLine("      AND Imprese_Progetti.SA_Cod = mov_destinazioni.Sa_Cod ")
            StrSQL.AppendLine("      AND Imprese_Progetti.Appezza = mov_destinazioni.Appezza ")
            StrSQL.AppendLine("      AND Imprese_Progetti.Id_Reg = mov_destinazioni.Id_Destinazione ")

            StrSQL.AppendLine("	LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Dettagli ud ON ud.CodFisc=movimenti.username_modifica ")

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine("   agenda.lav_cod = " & LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA)
            StrSQL.AppendLine("   AND movimenti.cau_mov = '2200' ")
            StrSQL.AppendLine("   AND mov_dettaglio_tecnico.ff_classe = 34 ")
            StrSQL.AppendLine("   AND movimenti.data_movimento >= imprese_progetti.validita_inizio  ")

            If data = AGRODATAINIZIO Then
                StrSQL.AppendLine("   AND movimenti.data_movimento <= imprese_progetti.validita_fine  ")
            Else
                StrSQL.AppendLine("   AND movimenti.data_movimento <= " & Agro_SQL_SaveDate(data))
            End If

            If Not String.IsNullOrEmpty(filtroProgetti) Then
                StrSQL.AppendLine("   AND imprese_progetti.progetto_cod in (" & Agro_SQL_Save_Clausola_IN(filtroProgetti) & ") ")
            End If

            StrSQL.AppendLine(" ORDER BY ")
            StrSQL.AppendLine("   mov_destinazioni.validita_inizio DESC ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] :   " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function Leggi_Rilievi_Stime_Produzione_NEW(filtroProgetti As List(Of Integer),
                                                       ByRef objParametri As AgronicaCoreParametri,
                                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                       data As Date) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Statistiche.Leggi_Rilievi_Stime_Produzione_NEW()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            EseguiQuery_Scrittura(objParametri, CreaTabellaTemp_FiltroProgetti(), NomeRoutine)

            If Not IsNothing(filtroProgetti) AndAlso filtroProgetti.Any Then

                Dim chunks = ChunkBy(Of Integer)(filtroProgetti, 1000)
                For Each chunk In chunks
                    StrSQL.Append(" INSERT INTO #TempProgetti (Progetto_Cod) VALUES ")
                    For Each p As Integer In chunk
                        StrSQL.AppendLine(String.Format("({0}),", p))
                    Next
                    Dim strSqlInsert As String = StrSQL.ToString
                    strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                    StrSQL.Clear()
                    EseguiQuery_Scrittura(objParametri, strSqlInsert, NomeRoutine)
                Next

            End If

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("   Imprese_Progetti.Piva, Imprese_Progetti.Sa_Cod, Imprese_Progetti.Appezza, Imprese_Progetti.Id_Reg, Imprese_Progetti.Progetto_Cod, ")
            StrSQL.AppendLine("   Agenda.id_Agenda, Mov_Destinazioni.qta as resa_rilievo, Movimenti.Data_Movimento as data_rilievo, ")
            StrSQL.AppendLine("   IIF(ud.Cognome != '', ud.Cognome + ' ' + ud.Nome, ud.Rag_Soc) as utente_rilievo, ")
            StrSQL.AppendLine("   ud.UserName, ud.CodFisc, ud.Nome, ud.Cognome, ud.Rag_Soc ")
            StrSQL.AppendLine(" FROM Movimenti_Dettagli ")
            StrSQL.AppendLine(" INNER JOIN Movimenti ON ")
            StrSQL.AppendLine("     Movimenti_Dettagli.piva = Movimenti.piva ")
            StrSQL.AppendLine(" AND Movimenti_Dettagli.sa_cod = Movimenti.sa_cod ")
            StrSQL.AppendLine(" AND Movimenti_Dettagli.id_Agenda = Movimenti.id_Agenda ")
            StrSQL.AppendLine(" AND Movimenti_Dettagli.id_mov = Movimenti.id_mov ")
            StrSQL.AppendLine(" INNER JOIN Agenda ON ")
            StrSQL.AppendLine("     Movimenti.piva = Agenda.piva ")
            StrSQL.AppendLine(" AND Movimenti.sa_cod = Agenda.sa_cod ")
            StrSQL.AppendLine(" AND Movimenti.id_Agenda = Agenda.id_Agenda ")
            StrSQL.AppendLine(" INNER JOIN Mov_Destinazioni ON ")
            StrSQL.AppendLine("     Movimenti_Dettagli.piva = Mov_Destinazioni.piva ")
            StrSQL.AppendLine(" AND Movimenti_Dettagli.sa_cod = Mov_Destinazioni.sa_cod ")
            StrSQL.AppendLine(" AND Movimenti_Dettagli.id_Agenda = Mov_Destinazioni.id_Agenda ")
            StrSQL.AppendLine(" AND Movimenti_Dettagli.id_mov = Mov_Destinazioni.id_mov ")
            StrSQL.AppendLine(" AND Movimenti_Dettagli.id_mov_det = Mov_Destinazioni.id_mov_det ")
            StrSQL.AppendLine(" INNER JOIN Mov_Dettaglio_Tecnico ON ")
            StrSQL.AppendLine("     Movimenti_Dettagli.piva = Mov_Dettaglio_Tecnico.piva ")
            StrSQL.AppendLine(" AND Movimenti_Dettagli.sa_cod = Mov_Dettaglio_Tecnico.sa_cod ")
            StrSQL.AppendLine(" AND Movimenti_Dettagli.id_Agenda = Mov_Dettaglio_Tecnico.id_Agenda ")
            StrSQL.AppendLine(" AND Movimenti_Dettagli.id_mov = Mov_Dettaglio_Tecnico.id_mov ")
            StrSQL.AppendLine(" AND Movimenti_Dettagli.id_mov_det = Mov_Dettaglio_Tecnico.id_mov_det ")
            StrSQL.AppendLine(" INNER JOIN Imprese_Progetti ON ")
            StrSQL.AppendLine("     Imprese_Progetti.Piva = Mov_Destinazioni.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.SA_Cod = Mov_Destinazioni.Sa_Cod ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = Mov_Destinazioni.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = Mov_Destinazioni.Id_Destinazione ")

            StrSQL.AppendLine("	LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Utenti_Dettagli ud ON ud.CodFisc=Movimenti.username_modifica ")

            StrSQL.AppendLine(" INNER JOIN #TempProgetti tempP ON ")
            StrSQL.AppendLine("     Imprese_Progetti.Progetto_Cod = tempP.Progetto_Cod ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")
            StrSQL.AppendLine(" AND Agenda.lav_cod = " & LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA & " ")
            StrSQL.AppendLine(" AND Movimenti.CAU_MOV = '" & CAU_RILIEVO_RACCOLTA & "' ")
            StrSQL.AppendLine(" AND Mov_Dettaglio_Tecnico.ff_classe = 34 ")
            StrSQL.AppendLine(" AND Movimenti.Data_Movimento >= Imprese_Progetti.validita_inizio ")

            If data = AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND Movimenti.Data_Movimento <= Imprese_Progetti.validita_fine ")
            Else
                StrSQL.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(data))
            End If

            StrSQL.AppendLine(" ORDER BY Mov_Destinazioni.validita_inizio DESC ")


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            EseguiQuery_Scrittura(objParametri, EliminaTabellaTemp_FiltroProgetti, NomeRoutine)

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception

            ' Rollback
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)

        Finally

            ConnessioniTransazioni.ChiudiConnessione(objParametri)
            ''Utility.VerificaChiudiConnessione(objParametri, False)

        End Try

        Return DT

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge report stime di produzione
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi_Report_Stime_Produzione(ByVal filtroProgetti As List(Of Integer),
                                                  ByVal ordinamento As String,
                                                  ByRef objParametri As AgronicaCoreParametri,
                                                  Optional ByVal dataReport As Date = AGRODATAINIZIO,
                                                  Optional ByVal sommaQta As Boolean = True) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Statistiche.Leggi_Report_Stime_Produzione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        ' Anna: AGGIUNTE NUOVE INFORMAZIONI CHE RICHIEDONO L'USO DI SUBQUERYS RIPETUTE PIU' E PIU' VOLTE ALL'INTERNO DELLA QUERY PRINCIPALE
        Dim Sup_Abbattuta As String = "ISNULL((SELECT " & If(sommaQta, "SUM(mov_destinazioni.qta2)", "TOP 1 mov_destinazioni.qta2") &
                                                " FROM movimenti_dettagli
                                                     INNER JOIN movimenti

                                                        INNER JOIN agenda
                                                            ON movimenti.piva = agenda.piva
                                                            AND movimenti.sa_cod = agenda.sa_cod
                                                            AND movimenti.id_agenda = agenda.id_agenda
                                                        ON movimenti_dettagli.piva = movimenti.piva
                                                        AND movimenti_dettagli.sa_cod = movimenti.sa_cod
                                                        AND movimenti_dettagli.id_agenda = movimenti.id_agenda
                                                        AND movimenti_dettagli.id_mov = movimenti.id_mov

                                                        INNER JOIN mov_destinazioni
                                                            ON movimenti_dettagli.piva = mov_destinazioni.piva
                                                            AND movimenti_dettagli.sa_cod = mov_destinazioni.sa_cod
                                                            AND movimenti_dettagli.id_agenda = mov_destinazioni.id_agenda
                                                            AND movimenti_dettagli.id_mov = mov_destinazioni.id_mov
                                                            AND movimenti_dettagli.id_mov_det = mov_destinazioni.id_mov_det

                                                WHERE agenda.lav_cod = 170 
                                                AND movimenti.cau_mov = '2300' 
                                                AND mov_destinazioni.piva = reg_impianti.piva 
                                                AND mov_destinazioni.sa_cod = reg_impianti.sa_cod
                                                AND mov_destinazioni.appezza = reg_impianti.appezza
                                                AND mov_destinazioni.id_destinazione = reg_impianti.id_reg
                                                AND movimenti.data_movimento >= imprese_progetti.validita_inizio
                                                AND movimenti.data_movimento <= " & If(dataReport = AGRODATAINIZIO, "imprese_progetti.validita_fine", Agro_SQL_SaveDate(dataReport, False)) &
                                                If(sommaQta, "", " ORDER BY mov_destinazioni.validita_inizio DESC") & "), 0)"

        Dim Perc_Piante_Morte As String = "ISNULL((SELECT " & If(sommaQta, "SUM(mov_destinazioni.qta)", "TOP 1 mov_destinazioni.qta") &
                                                     " FROM movimenti_dettagli
                                                        INNER JOIN movimenti

                                                            INNER JOIN agenda
                                                                ON movimenti.piva = agenda.piva
                                                                AND movimenti.sa_cod = agenda.sa_cod
                                                                AND movimenti.id_agenda = agenda.id_agenda
                                                            ON movimenti_dettagli.piva = movimenti.piva
                                                            AND movimenti_dettagli.sa_cod = movimenti.sa_cod
                                                            AND movimenti_dettagli.id_agenda = movimenti.id_agenda
                                                            AND movimenti_dettagli.id_mov = movimenti.id_mov

                                                            INNER JOIN mov_destinazioni
                                                                ON movimenti_dettagli.piva = mov_destinazioni.piva                                                   
												                AND movimenti_dettagli.sa_cod = mov_destinazioni.sa_cod
                                                                AND movimenti_dettagli.id_agenda = mov_destinazioni.id_agenda
                                                                AND movimenti_dettagli.id_mov = mov_destinazioni.id_mov
                                                                AND movimenti_dettagli.id_mov_det = mov_destinazioni.id_mov_det
                                                            INNER JOIN mov_dettaglio_tecnico
                                                                ON movimenti_dettagli.piva = mov_dettaglio_tecnico.piva
                                                                AND movimenti_dettagli.id_mov_det = mov_dettaglio_tecnico.id_mov_det
                                                                AND movimenti_dettagli.id_mov = mov_dettaglio_tecnico.id_mov
												                AND movimenti_dettagli.id_agenda = mov_dettaglio_tecnico.id_agenda
												                AND movimenti_dettagli.sa_cod = mov_dettaglio_tecnico.sa_cod

								                     WHERE agenda.lav_cod = 108 
								                     AND movimenti.cau_mov = '2200' 
								                     AND mov_dettaglio_tecnico.ff_classe IN (28,29)
								                     AND mov_destinazioni.piva = reg_impianti.piva 
								                     AND mov_destinazioni.sa_cod = reg_impianti.sa_cod
								                     AND mov_destinazioni.appezza = reg_impianti.appezza
								                     AND mov_destinazioni.id_destinazione = reg_impianti.id_reg
								                     AND movimenti.data_movimento >= imprese_progetti.validita_inizio
								                     AND movimenti.data_movimento <= " & If(dataReport = AGRODATAINIZIO, "imprese_progetti.validita_fine", Agro_SQL_SaveDate(dataReport, False)) &
                                                     If(sommaQta, "", " ORDER BY mov_destinazioni.validita_inizio DESC") & "), 0)"

        Dim ImpiantoAttivo As String = "ISNULL((SELECT TOP 1 mov_destinazioni.qta2
						                            FROM movimenti_dettagli
						                                INNER JOIN movimenti

						                                    INNER JOIN agenda
						                                        ON movimenti.piva = agenda.piva
						                                        AND movimenti.sa_cod = agenda.sa_cod
						                                        AND movimenti.id_agenda = agenda.id_agenda
						                                    ON movimenti_dettagli.piva = movimenti.piva
						                                    AND movimenti_dettagli.sa_cod = movimenti.sa_cod
						                                    AND movimenti_dettagli.id_agenda = movimenti.id_agenda
						                                    AND movimenti_dettagli.id_mov = movimenti.id_mov

						                                    INNER JOIN mov_destinazioni
						                                        ON movimenti_dettagli.piva = mov_destinazioni.piva
						                                        AND movimenti_dettagli.sa_cod = mov_destinazioni.sa_cod
						                                        AND movimenti_dettagli.id_agenda = mov_destinazioni.id_agenda
						                                        AND movimenti_dettagli.id_mov = mov_destinazioni.id_mov
						                                        AND movimenti_dettagli.id_mov_det = mov_destinazioni.id_mov_det

						                            WHERE  ( agenda.lav_cod = 170 )
						                            AND ( movimenti.cau_mov = '2300' )
						                            AND ( mov_destinazioni.piva = reg_impianti.piva )
						                            AND mov_destinazioni.sa_cod = reg_impianti.sa_cod
						                            AND mov_destinazioni.appezza = reg_impianti.appezza
						                            AND mov_destinazioni.id_destinazione = reg_impianti.id_reg
						                            AND movimenti.data_movimento >= imprese_progetti.validita_inizio
						                            AND movimenti.data_movimento <= imprese_progetti.validita_fine

						                            ORDER  BY mov_destinazioni.validita_inizio DESC ), 0)"

        Try

            ConnessioniTransazioni.ApriConnessione(True, objParametri)
            ''Utility.VerificaApriConnessione(objParametri, False)

            EseguiQuery_Scrittura(objParametri, CreaTabellaTemp_FiltroProgetti(), NomeRoutine)

            If Not IsNothing(filtroProgetti) AndAlso filtroProgetti.Any Then

                Dim chunks = ChunkBy(Of Integer)(filtroProgetti, 1000)
                For Each chunk In chunks
                    StrSQL.Append(" insert into #TempProgetti (Progetto_Cod) VALUES ")
                    For Each p As Integer In chunk
                        StrSQL.AppendLine(String.Format("({0}),", p))
                    Next
                    Dim strSqlInsert As String = StrSQL.ToString
                    strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                    StrSQL.Clear()
                    EseguiQuery_Scrittura(objParametri, strSqlInsert, NomeRoutine)
                Next

            End If


            ' Query Principale
            StrSQL.AppendLine("SELECT * FROM ( ")
            StrSQL.AppendLine("SELECT DISTINCT ")
            StrSQL.AppendLine("    Imprese.PIVA + '_' + cast(Centri_Aziendali.sa_cod  as varchar(20))+ '_' + cast(Appezzamento.Appezza  as varchar(20)) + '_' + cast(Reg_Impianti.id_reg  as varchar(20)) + '_' + cast(isnull(SpecieVegetali.veg_cod ,'0')  as varchar(20)) + '_' + cast(isnull(Imprese_Progetti.Progetto_Cod,'0')  as varchar(20)) as chiave  ")
            StrSQL.AppendLine("  , Imprese.PIVA  ")
            StrSQL.AppendLine("  , Imprese.rag_soc  ")
            StrSQL.AppendLine("  , IC.val_cod AS Codice_Socio  ")
            StrSQL.AppendLine("  , IC_cuaa.val_cod AS CUAA  ")
            StrSQL.AppendLine("  , IC1324.val_cod AS Contratto_Produzione  ")
            StrSQL.AppendLine("  , ISNULL ((SELECT TOP 1 Contatti.Cognome + ' ' + Contatti.Nome FROM Imprese_Codici INNER JOIN Contatti ON Imprese_Codici.val_cod = Contatti.Cod_Contatto WHERE Imprese_Codici.id_cod = 1088 AND Imprese_Codici.piva=Imprese.piva), ' ') AS Tecnico_Referente ")
            StrSQL.AppendLine("  , ISNULL ((SELECT TOP 1 Codice_Fiscale FROM Contatti WHERE Imprese.PIVA = Contatti.Cod_Contatto), ' ') AS Codice_Fiscale ")
            StrSQL.AppendLine("  , Indirizzi.ind_des + ' ' + Indirizzi.Cap + ' ' + ISNULL(Istat.LOCALITA, '') + ' ' + ISNULL(Istat.COMUNI_PROV, '') + ' ' + Indirizzi.Stato AS Indirizzo ")
            'StrSQL.AppendLine("  , Lista_Province.PROVINCIA  ")
            StrSQL.AppendLine("  , Centri_Aziendali.sa_cod  ")
            StrSQL.AppendLine("  , Centri_Aziendali.sa_nome  ")
            'StrSQL.AppendLine("  , Indirizzi_Centro.ind_des + ' ' + Indirizzi_Centro.Cap + ' ' + ISNULL(Istat_Centro.LOCALITA, '') + ' ' + ISNULL(Istat_Centro.COMUNI_PROV, '') + ' ' + Indirizzi_Centro.Stato AS Indirizzo_Centro ")
            StrSQL.AppendLine("  , Indirizzi_Centro.ind_des AS Indirizzo_Centro, ISNULL(Istat_Centro.LOCALITA, '') AS Comune_Centro, ISNULL(Istat_Centro.COMUNI_PROV, '') AS Provincia_Centro, Indirizzi_Centro.Cap AS CAP_Centro, Indirizzi_Centro.Stato AS Stato_Centro ")
            StrSQL.AppendLine("  , SpecieVegetali.Veg_Cod  ")
            StrSQL.AppendLine("  , Reg_Impianti.Cul_Cod  ")
            StrSQL.AppendLine("  , SpecieVegetali.Veg_Des  ")
            StrSQL.AppendLine("  , Cultivar.Cul_Des  ")
            StrSQL.AppendLine("  , GruppoVarietale.GRVA_DES  ")
            StrSQL.AppendLine("  , Campi.Campo_Cod  ")
            StrSQL.AppendLine("  , Campi.Campo_Des  ")
            StrSQL.AppendLine("  , CC1279.val_cod As Codice_Campo  ")
            StrSQL.AppendLine("  , Appezzamento.APPEZZA  ")

            StrSQL.AppendLine("  , Appezzamento.SUP_APP                         ")
            StrSQL.AppendLine("  , Appezzamento.APP_NOME                        ")
            ' Anna 20/07/21 - Aggiunte info extra per gli appezzamenti: Nazione, Regione, Città
            StrSQL.AppendLine("  , CASE WHEN AppIndirizzi.stato IN ('ITALIA','Italy','It','Italia','italiano') ")
            StrSQL.AppendLine("         THEN 'IT'                               ")
            StrSQL.AppendLine("     ELSE AppIndirizzi.stato                     ")
            StrSQL.AppendLine("     END AS nazione                              ")
            StrSQL.AppendLine("  , AppIndirizzi.frz_des AS citta                ")
            StrSQL.AppendLine("  , lista_regioni.Regione_Des AS regione         ")

            StrSQL.AppendLine("  , Reg_Impianti.ID_REG  ")
            StrSQL.AppendLine("  , Imprese_Progetti.Validita_Inizio  ")
            StrSQL.AppendLine("  , Imprese_Progetti.Validita_Fine  ")
            StrSQL.AppendLine("  , Reg_Impianti.Sup_Imp  ")
            StrSQL.AppendLine("  , Imprese_Progetti.Regolamento_Cod AS Regolamento  ")
            StrSQL.AppendLine("  , Imprese_Progetti.Disciplinare_Cod AS Finanziamento  ")
            StrSQL.AppendLine("  , Reg_Impianti.Grfi_Cod  ")
            StrSQL.AppendLine("  , Reg_Impianti.Grva_Cod_Veg  ")
            StrSQL.AppendLine("  , Reg_Impianti.Cop_Cod  ")
            StrSQL.AppendLine("  , Imprese_Progetti.Progetto_Cod  ")
            StrSQL.AppendLine("  , Imprese_Progetti.Progetto_Nome as Lotto  ")
            StrSQL.AppendLine("  , Imprese_Progetti.Progetto_Des as Descrizione  ")
            StrSQL.AppendLine("  , Imprese_Progetti.Validita_Inizio AS Progetto_Validita_Inizio  ")
            StrSQL.AppendLine("  , Imprese_Progetti.Validita_Fine AS Progetto_Validita_Fine  ")
            StrSQL.AppendLine("  , IC.val_cod AS CodiceSocio  ")
            StrSQL.AppendLine("  , IC_Cuaa.val_cod AS CodiceCuaa  ")
            StrSQL.AppendLine("  , kpin.val_Cod as kpin  ")
            StrSQL.AppendLine("  , blockName.val_Cod as blockName ")
            StrSQL.AppendLine("  , growernumber.val_Cod as GrowerNumber ")
            StrSQL.AppendLine("  , Imprese_Progetti.Produzione_Prevista ")
            'StrSQL.AppendLine("  , Imprese_Progetti.Produzione_Prevista*Reg_Impianti.Sup_Imp As Stima_Produzione ")
            StrSQL.AppendLine("  , Imprese_Progetti.Username_Creazione, Imprese_Progetti.Username_Modifica, Imprese_Progetti.Data_Creazione, Imprese_Progetti.Data_Modifica  ")

            ' Anna 30/07/21 - Aggiunte info extra:
            StrSQL.AppendLine("  , Data_Inizio_Produzione AS AnnoPrimaProduzione")
            StrSQL.AppendLine("  , OTabelle_Parametri.descrizione AS Licenza                                ")
            StrSQL.AppendLine("  , CASE WHEN (Data_Inizio_Produzione) IS NULL                                 ")
            StrSQL.AppendLine("            THEN 'NO'                                                    ")
            StrSQL.AppendLine("       ELSE CASE WHEN (YEAR(Data_Inizio_Produzione)<=YEAR(getdate()) AND YEAR(Data_Inizio_Produzione)>1900)        ")
            StrSQL.AppendLine("            THEN 'SI'                                                        ")
            StrSQL.AppendLine("       ELSE 'NO'                                                         ")
            StrSQL.AppendLine("       END                                                                          ")
            StrSQL.AppendLine("    END AS Produzione                                                                 ")

            StrSQL.AppendLine("  , ISNULL  ((SELECT  TOP 1 CONVERT(VARCHAR,Mov_Destinazioni.Validita_Inizio, 103)   ")
            StrSQL.AppendLine("              FROM Movimenti_dettagli INNER JOIN  ")
            StrSQL.AppendLine("                     Movimenti INNER JOIN  ")
            StrSQL.AppendLine("                     Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND Movimenti.Id_Agenda = Agenda.Id_Agenda ON   ")
            StrSQL.AppendLine("                     Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND   ")
            StrSQL.AppendLine("                     Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov INNER JOIN  ")
            StrSQL.AppendLine("                     Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND   ")
            StrSQL.AppendLine("                     Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda And Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov And  ")
            StrSQL.AppendLine("                     Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det  ")
            StrSQL.AppendLine("                 INNER Join ")
            StrSQL.AppendLine("                     Mov_Dettaglio_Tecnico ON Movimenti_dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva AND Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det AND Movimenti_dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov AND Movimenti_dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod  ")
            StrSQL.AppendLine("              WHERE (Agenda.Lav_Cod = 79) AND (Movimenti.Cau_Mov = '2100') AND (Mov_Destinazioni.PIVA = Reg_Impianti.PIVA) AND  ")
            StrSQL.AppendLine("                     Mov_Destinazioni.Sa_Cod = Reg_Impianti.sa_cod AND Mov_Destinazioni.APPEZZA = Reg_Impianti.APPEZZA AND  ")
            StrSQL.AppendLine("                     Mov_Destinazioni.ID_destinazione = Reg_Impianti.ID_REG   ")
            StrSQL.AppendLine("              AND Movimenti.Data_Movimento >= Imprese_Progetti.Validita_Inizio  ")
            StrSQL.AppendLine("              AND Movimenti.Data_Movimento <= Imprese_Progetti.Validita_Fine  ")
            StrSQL.AppendLine("             ORDER BY Mov_Destinazioni.validita_inizio), '') AS Data_Fioritura ")


            StrSQL.AppendLine(" , " & Sup_Abbattuta & " AS Sup_Abbattuta")
            StrSQL.AppendLine(" , " & Perc_Piante_Morte & " AS Perc_Piante_Morte")
            StrSQL.AppendLine(" , CASE WHEN " & ImpiantoAttivo & " = reg_impianti.sup_imp ")
            StrSQL.AppendLine("     THEN 'NO'")
            StrSQL.AppendLine("     ELSE 'SI'")
            StrSQL.AppendLine("   END AS ImpiantoAttivo")

            StrSQL.AppendLine(" , CASE WHEN " & ImpiantoAttivo & " != reg_impianti.sup_imp") '1) ATTIVO
            StrSQL.AppendLine("     THEN ")

            StrSQL.AppendLine("        CASE WHEN (YEAR(Data_Inizio_Produzione)<=YEAR(Getdate()))") '2) PRODUCING
            StrSQL.AppendLine("             THEN")

            'Se ABBATTUTO PARZIALE + RILIEVO DANNI allora il calcolo diventa:
            'Stima prod = ((Ha impianto – Ha abbattimento) - (Ha impianto – Ha abbattimento) x %Danno) x Resa Impianto 
            StrSQL.AppendLine("                 CASE WHEN " & Sup_Abbattuta & " < sup_imp") '3A) MORIA + ABBATTIMENTO PARZIALE
            StrSQL.AppendLine("                 AND " & Perc_Piante_Morte & " > 0 ")
            StrSQL.AppendLine("                     THEN ((sup_imp - " & Sup_Abbattuta & ") - ((sup_imp - " & Sup_Abbattuta & ") * " & Perc_Piante_Morte & "/100))  * produzione_prevista ")
            'StrSQL.AppendLine("                 END ")

            'Se RILIEVO DANNI, che è espressa in percentuale, allora il calcolo diventa:
            'Stima prod = (Ha impianto – Ha Impianto x %Danno) x Resa Impianto 
            StrSQL.AppendLine("                 WHEN " & Perc_Piante_Morte & " > 0") '3B) MORIA
            StrSQL.AppendLine("                     THEN (sup_imp - (sup_imp * " & Perc_Piante_Morte & ") /100 * produzione_prevista)")

            'Se ABBATTUTO PARZIALE allora il calcolo diventa:
            'Stima prod = (Ha impianto – Ha abbattimento) x Resa Impianto  
            StrSQL.AppendLine("                 WHEN " & Sup_Abbattuta & " < sup_imp") '3C) ABBATTIMENTO PARZIALE
            StrSQL.AppendLine("                     THEN (sup_imp - " & Sup_Abbattuta & ") * produzione_prevista")

            StrSQL.AppendLine("                 ELSE  ") '3D) NESSUN DANNO - TUTTO OK 
            StrSQL.AppendLine("                      imprese_progetti.produzione_prevista * reg_impianti.sup_imp")
            StrSQL.AppendLine("                 END ")
            'StrSQL.AppendLine("         END  ")
            'StrSQL.AppendLine("                 END")
            StrSQL.AppendLine("         ELSE ") '2) NON PRODUCING
            'StrSQL.AppendLine("              CASE WHEN ( data_inizio_produzione ) IS NULL OR ( Year(data_inizio_produzione) ) >= Year(Getdate()) THEN 0")
            StrSQL.AppendLine("              0")
            StrSQL.AppendLine("         END  ")
            'StrSQL.AppendLine("    ELSE WHEN " & ImpiantoAttivo & " = reg_impianti.sup_imp THEN 0") '1) NON ATTIVO
            StrSQL.AppendLine("  ELSE 0") '1) NON ATTIVO
            StrSQL.AppendLine("  END AS Stime_Produzione")
            'StrSQL.AppendLine(" , " & Perc_Piante_Morte & " AS MoriaPiante")









            StrSQL.AppendLine(" FROM Imprese  ")

            StrSQL.AppendLine(" INNER JOIN GerarchiaImprese                                         ")
            StrSQL.AppendLine(" ON Imprese.Piva = GerarchiaImprese.Figlio                           ")

            StrSQL.AppendLine(" INNER JOIN ImpresexIndirizzi                                        ")
            StrSQL.AppendLine(" ON Imprese.PIVA = ImpresexIndirizzi.PIVA                            ")
            StrSQL.AppendLine(" AND ImpresexIndirizzi.Tipo_Indirizzo = 1                            ")

            StrSQL.AppendLine(" LEFT JOIN Indirizzi                                                 ")
            StrSQL.AppendLine(" ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo        ")

            StrSQL.AppendLine(" LEFT JOIN ISTAT                                                     ")
            StrSQL.AppendLine(" ON Indirizzi.pro_cod_istat = ISTAT.PROV                             ")
            StrSQL.AppendLine(" AND Indirizzi.com_cod_istat = ISTAT.COM                             ")

            'StrSQL.AppendLine(" INNER JOIN Lista_Province ON Lista_Province.Sigla = ISTAT.COMUNI_PROV ")
            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali                                          ")
            StrSQL.AppendLine(" ON Imprese.Piva=Centri_Aziendali.Piva                               ")

            StrSQL.AppendLine(" LEFT JOIN CentrixIndirizzi                                          ")
            StrSQL.AppendLine(" ON CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA                    ")
            StrSQL.AppendLine(" AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod               ")
            StrSQL.AppendLine(" AND CentrixIndirizzi.Tipo_Indirizzo = 1                             ")

            StrSQL.AppendLine(" LEFT JOIN Indirizzi AS Indirizzi_Centro                             ")
            StrSQL.AppendLine(" ON Indirizzi_Centro.cod_indirizzo = CentrixIndirizzi.cod_indirizzo  ")

            StrSQL.AppendLine(" LEFT JOIN ISTAT AS Istat_Centro                                     ")
            StrSQL.AppendLine(" ON Indirizzi_Centro.pro_cod_istat = Istat_Centro.PROV               ")
            StrSQL.AppendLine(" AND Indirizzi_Centro.com_cod_istat = Istat_Centro.COM               ")

            StrSQL.AppendLine(" LEFT JOIN Appezzamento                                              ")
            StrSQL.AppendLine(" ON Centri_Aziendali.Piva = Appezzamento.Piva                        ")
            StrSQL.AppendLine(" AND Centri_Aziendali.Sa_Cod = Appezzamento.Sa_Cod                   ")

            'StrSQL.AppendLine(" LEFT JOIN AppezzamentiXParticelle ON Appezzamento.Piva = AppezzamentiXParticelle.Piva And Appezzamento.SA_COD = AppezzamentiXParticelle.sa_cod And Appezzamento.APPEZZA = AppezzamentiXParticelle.APPEZZA  ")
            'StrSQL.AppendLine(" LEFT JOIN ZonexParticelle ON AppezzamentiXParticelle.PROV = ZonexParticelle.PROV And AppezzamentiXParticelle.COM = ZonexParticelle.COM And AppezzamentiXParticelle.SEZIONE = ZonexParticelle.SEZIONE And AppezzamentiXParticelle.FOGLIO = ZonexParticelle.FOGLIO And AppezzamentiXParticelle.NUMERO = ZonexParticelle.NUMERO And AppezzamentiXParticelle.SUBALTERNO = ZonexParticelle.SUBALTERNO And ZonexParticelle.Zona_Cod = -17  ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti                                              ")
            StrSQL.AppendLine(" ON Appezzamento.Piva = Reg_Impianti.Piva                            ")
            StrSQL.AppendLine(" AND Appezzamento.SA_Cod = Reg_Impianti.Sa_Cod                       ")
            StrSQL.AppendLine(" AND Appezzamento.Appezza = Reg_Impianti.Appezza                     ")

            StrSQL.AppendLine(" INNER JOIN Imprese_Progetti                                         ")
            StrSQL.AppendLine(" ON Imprese_Progetti.Piva = Reg_Impianti.Piva                        ")
            StrSQL.AppendLine(" AND Imprese_Progetti.SA_Cod = Reg_Impianti.Sa_Cod                   ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza                 ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg                   ")

            StrSQL.AppendLine(" INNER JOIN #TempProgetti tempP ")
            StrSQL.AppendLine(" ON  Imprese_Progetti.Progetto_Cod = tempP.Progetto_Cod")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici kpin                                  ")
            StrSQL.AppendLine(" ON Imprese_Progetti.Piva = kpin.Piva                                ")
            StrSQL.AppendLine(" AND Imprese_Progetti.SA_Cod = kpin.Sa_Cod                           ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = kpin.Appezza                         ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = kpin.Id_Reg                           ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = kpin.Progetto_Cod               ")
            StrSQL.AppendLine(" AND kpin.id_Cod = 1287                                              ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici blockName                             ")
            StrSQL.AppendLine(" ON Imprese_Progetti.Piva = blockName.Piva                           ")
            StrSQL.AppendLine(" AND Imprese_Progetti.SA_Cod = blockName.Sa_Cod                      ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = blockName.Appezza                    ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = blockName.Id_Reg                      ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = blockName.Progetto_Cod          ")
            StrSQL.AppendLine(" AND blockName.id_Cod = 1288                                         ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici growerNumber                          ")
            StrSQL.AppendLine(" ON Imprese_Progetti.Piva = growerNumber.Piva                        ")
            StrSQL.AppendLine(" AND Imprese_Progetti.SA_Cod = growerNumber.Sa_Cod                   ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = growerNumber.Appezza                 ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = growerNumber.Id_Reg                   ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = growerNumber.Progetto_Cod       ")
            StrSQL.AppendLine(" AND growerNumber.id_Cod = 1317                                      ")

            StrSQL.AppendLine(" LEFT OUTER JOIN Imprese_Codici IC                                   ")
            StrSQL.AppendLine(" ON IC.Piva = Imprese.Piva AND IC.id_cod = 1033                      ")

            StrSQL.AppendLine(" LEFT OUTER JOIN Imprese_Codici IC_cuaa                              ")
            StrSQL.AppendLine(" ON IC_cuaa.Piva = Imprese.Piva                                      ")
            StrSQL.AppendLine(" AND IC_cuaa.id_cod = 1010                                           ")

            StrSQL.AppendLine(" LEFT OUTER JOIN Campi                                               ")
            StrSQL.AppendLine(" ON Campi.Piva = Appezzamento.Piva                                   ")
            StrSQL.AppendLine(" AND Campi.Sa_Cod = Appezzamento.Sa_Cod                              ")
            StrSQL.AppendLine(" AND Campi.Campo_Cod = Appezzamento.Campo_Cod                        ")

            StrSQL.AppendLine(" LEFT JOIN Campi_Codici CC1279                                       ")
            StrSQL.AppendLine(" ON CC1279.Piva = Campi.Piva                                         ")
            StrSQL.AppendLine(" AND CC1279.SA_Cod = Campi.Sa_Cod                                    ")
            StrSQL.AppendLine(" AND CC1279.Campo_Cod = Campi.Campo_Cod                              ")
            StrSQL.AppendLine(" AND CC1279.id_Cod = 1279                                            ")

            StrSQL.AppendLine(" LEFT JOIN Campi_Codici CC1325                                       ")
            StrSQL.AppendLine(" ON CC1325.Piva = Campi.Piva                                         ")
            StrSQL.AppendLine(" AND CC1325.SA_Cod = Campi.Sa_Cod                                    ")
            StrSQL.AppendLine(" AND CC1325.Campo_Cod = Campi.Campo_Cod                              ")
            StrSQL.AppendLine(" AND CC1325.id_Cod = 1325                                            ")

            StrSQL.AppendLine(" LEFT JOIN Campi_Codici CC1326                                       ")
            StrSQL.AppendLine(" ON CC1326.Piva = Campi.Piva                                         ")
            StrSQL.AppendLine(" AND CC1326.SA_Cod = Campi.Sa_Cod                                    ")
            StrSQL.AppendLine(" AND CC1326.Campo_Cod = Campi.Campo_Cod                              ")
            StrSQL.AppendLine(" AND CC1326.id_Cod = 1326                                            ")

            StrSQL.AppendLine(" LEFT JOIN Imprese_Codici IC1324                                     ")
            StrSQL.AppendLine(" ON IC1324.Piva = Imprese.Piva                                       ")
            StrSQL.AppendLine(" AND IC1324.id_Cod = 1324                                            ")

            StrSQL.AppendLine(" LEFT OUTER JOIN GruppoVarietale                                     ")
            StrSQL.AppendLine(" ON abs(Reg_Impianti.GRVA_Cod_VEG) = GruppoVarietale.GRVA_COD        ")

            StrSQL.AppendLine(" LEFT OUTER JOIN Cultivar                                            ")
            StrSQL.AppendLine(" ON Reg_Impianti.CUL_COD = Cultivar.CUL_COD                          ")

            StrSQL.AppendLine(" LEFT OUTER JOIN SpecieVegetali                                      ")
            StrSQL.AppendLine(" ON Cultivar.VEG_COD = SpecieVegetali.VEG_COD                        ")

            ' Anna 20/07/21 - Aggiunte info extra per gli appezzamenti: Nazione, Regione, Città
            StrSQL.AppendLine(" LEFT JOIN AppezzamentixIndirizzi                                        ")
            StrSQL.AppendLine(" ON Appezzamento.PIVA = AppezzamentixIndirizzi.PIVA                      ")
            StrSQL.AppendLine(" AND Appezzamento.SA_COD = AppezzamentixIndirizzi.SA_COD                 ")
            StrSQL.AppendLine(" AND Appezzamento.APPEZZA = AppezzamentixIndirizzi.APPEZZA               ")

            StrSQL.AppendLine(" LEFT JOIN Indirizzi AppIndirizzi                                        ")
            StrSQL.AppendLine(" ON AppezzamentixIndirizzi.cod_indirizzo = AppIndirizzi.cod_indirizzo    ")

            StrSQL.AppendLine(" LEFT JOIN Lista_Province                                                ")
            StrSQL.AppendLine(" ON AppIndirizzi.pro_cod_istat = Lista_Province.PROV                     ")

            StrSQL.AppendLine(" LEFT JOIN Lista_Regioni                                                 ")
            StrSQL.AppendLine(" ON Lista_Province.REG = Lista_Regioni.REG                               ")
            StrSQL.AppendLine(" AND Lista_Province.REG <> '000'                                         ")

            StrSQL.AppendLine("  LEFT JOIN Reg_Impianti_Codici LicenzaCod    ")
            StrSQL.AppendLine("  ON Imprese_Progetti.Piva = LicenzaCod.Piva      ")
            StrSQL.AppendLine("  AND Imprese_Progetti.SA_Cod = LicenzaCod.Sa_Cod ")
            StrSQL.AppendLine("  AND Imprese_Progetti.Appezza = LicenzaCod.Appezza    ")
            StrSQL.AppendLine("  AND Imprese_Progetti.Id_Reg = LicenzaCod.Id_Reg      ")
            StrSQL.AppendLine("  AND Imprese_Progetti.Progetto_Cod = LicenzaCod.Progetto_Cod     ")
            StrSQL.AppendLine("  AND LicenzaCod.id_cod IN ( " & enum_CodiciAnagrafe.Zespri_Fasi_Fase & " ) ")

            'StrSQL.AppendLine(" LEFT JOIN OTabelle ")
            'StrSQL.AppendLine(" ON LicenzaCod.id_cod = OTabelle.Tabella_Cod ")
            'StrSQL.AppendLine(" AND LicenzaCod.id_cod IN ( " & enum_CodiciAnagrafe.Zespri_Fasi_Fase & " ) ")
            'StrSQL.AppendLine(" LEFT JOIN OTabelle_Parametri ")
            'StrSQL.AppendLine(" ON OTabelle.Tabella_Cod = OTabelle_Parametri.Tabella_Cod  ")

            StrSQL.AppendLine(" left join OTabelle_Parametri on OTabelle_Parametri.tabella_cod =LicenzaCod.id_cod and OTabelle_Parametri.tabella_par_cod =LicenzaCod.val_cod  " & vbCrLf)


            StrSQL.AppendLine(" ")

            StrSQL.AppendLine(" ) AS REPORT_STIME_PRODUZIONE")

            ' filtro esercizi selezionati
            'If filtro <> "" Then
            '    StrSQL.AppendLine(" WHERE " & filtro)
            'End If

            If ordinamento <> "" Then
                StrSQL.AppendLine(" ORDER BY " & ordinamento)
            Else
                StrSQL.AppendLine(" ORDER BY piva, sa_cod, Appezza, Id_Reg DESC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            EseguiQuery_Scrittura(objParametri, EliminaTabellaTemp_FiltroProgetti, NomeRoutine)

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception

            ' Rollback
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)

        Finally

            ConnessioniTransazioni.ChiudiConnessione(objParametri)
            ''Utility.VerificaChiudiConnessione(objParametri, False)

        End Try

        Return DT

    End Function

    Public Function Leggi_Report_Stime_Produzione_NEW(filtroProgetti As List(Of Integer),
                                                      ordinamento As String,
                                                      ByRef objParametri As AgronicaCoreParametri,
                                                      dataReport As Date) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Statistiche.Leggi_Report_Stime_Produzione_NEW()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            ConnessioniTransazioni.ApriConnessione(True, objParametri)

            EseguiQuery_Scrittura(objParametri, CreaTabellaTemp_FiltroProgetti(), NomeRoutine)

            If Not IsNothing(filtroProgetti) AndAlso filtroProgetti.Any Then

                Dim chunks = ChunkBy(Of Integer)(filtroProgetti, 1000)
                For Each chunk In chunks
                    StrSQL.Append(" INSERT INTO #TempProgetti (Progetto_Cod) VALUES ")
                    For Each p As Integer In chunk
                        StrSQL.AppendLine(String.Format("({0}),", p))
                    Next
                    Dim strSqlInsert As String = StrSQL.ToString
                    strSqlInsert = strSqlInsert.Remove(strSqlInsert.LastIndexOf(","))
                    StrSQL.Clear()
                    EseguiQuery_Scrittura(objParametri, strSqlInsert, NomeRoutine)
                Next

            End If

            StrSQL.AppendLine(" WITH CTE_Data_Fioritura AS ( ")
            StrSQL.AppendLine(" 	SELECT MIN(CONVERT(VARCHAR, Mov_Destinazioni.Validita_Inizio, 103)) AS Data_Fioritura ")
            StrSQL.AppendLine(" 		 , Mov_Destinazioni.PIVA, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza,  Mov_Destinazioni.Id_Destinazione AS ID_Reg, Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine(" 	FROM Agenda ")
            StrSQL.AppendLine(" 	JOIN Movimenti ON ")
            StrSQL.AppendLine(" 		Movimenti.PIVA = Agenda.PIVA ")
            StrSQL.AppendLine(" 	AND Movimenti.Sa_Cod = Agenda.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Movimenti.Id_Agenda = Agenda.Id_Agenda ")

            StrSQL.AppendLine(" 	JOIN Movimenti_Dettagli ON ")
            StrSQL.AppendLine(" 		Movimenti_Dettagli.PIVA = Movimenti.PIVA ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Sa_Cod = Movimenti.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov ")

            StrSQL.AppendLine(" 	JOIN Mov_Destinazioni ON ")
            StrSQL.AppendLine(" 		Movimenti_Dettagli.PIVA = Mov_Destinazioni.Piva ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

            StrSQL.AppendLine(" 	JOIN Mov_Dettaglio_Tecnico ON ")
            StrSQL.AppendLine(" 		Movimenti_Dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod ")

            StrSQL.AppendLine(" 	JOIN Imprese_Progetti ON ")
            StrSQL.AppendLine(" 		Mov_Destinazioni.PIVA = Imprese_Progetti.PIVA ")
            StrSQL.AppendLine("   AND Mov_Destinazioni.Sa_Cod = Imprese_Progetti.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Mov_Destinazioni.APPEZZA = Imprese_Progetti.APPEZZA ")
            StrSQL.AppendLine("   AND Mov_Destinazioni.ID_destinazione = Imprese_Progetti.ID_REG ")
            StrSQL.AppendLine(" 	AND Movimenti.Data_Movimento >=  Imprese_Progetti.Validita_Inizio ")
            StrSQL.AppendLine("   AND Movimenti.Data_Movimento <=  Imprese_Progetti.Validita_Fine ")
            StrSQL.AppendLine("   WHERE 1 = 1 ")
            StrSQL.AppendLine("   AND Agenda.Lav_Cod = " & LAVCOD_FASI_FENOLOGICHE & " ")
            StrSQL.AppendLine("   AND Movimenti.Cau_Mov = '" & CAU_RILIEVO_CAMPO & "' ")
            StrSQL.AppendLine("   GROUP BY Mov_Destinazioni.PIVA, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza,  Mov_Destinazioni.Id_Destinazione, Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine(" ), ")

            StrSQL.AppendLine(" CTE_Sup_Abbattuta AS ( ")
            StrSQL.AppendLine(" 	SELECT SUM(Mov_Destinazioni.qta2) AS Sup_Abbattuta ")
            StrSQL.AppendLine(" 		 , Mov_Destinazioni.PIVA, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza,  Mov_Destinazioni.Id_Destinazione AS ID_Reg, Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine(" 	FROM Agenda ")
            StrSQL.AppendLine(" 	JOIN Movimenti ON ")
            StrSQL.AppendLine(" 		Movimenti.PIVA = Agenda.PIVA ")
            StrSQL.AppendLine(" 	AND Movimenti.Sa_Cod = Agenda.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Movimenti.Id_Agenda = Agenda.Id_Agenda ")

            StrSQL.AppendLine(" 	JOIN Movimenti_Dettagli ON ")
            StrSQL.AppendLine(" 		Movimenti_Dettagli.PIVA = Movimenti.PIVA ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Sa_Cod = Movimenti.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov ")

            StrSQL.AppendLine(" 	JOIN Mov_Destinazioni ON ")
            StrSQL.AppendLine(" 		Movimenti_Dettagli.PIVA = Mov_Destinazioni.Piva ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

            StrSQL.AppendLine(" 	JOIN Imprese_Progetti ON ")
            StrSQL.AppendLine(" 		Mov_Destinazioni.PIVA = Imprese_Progetti.PIVA ")
            StrSQL.AppendLine("   AND Mov_Destinazioni.Sa_Cod = Imprese_Progetti.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Mov_Destinazioni.APPEZZA = Imprese_Progetti.APPEZZA ")
            StrSQL.AppendLine("   AND Mov_Destinazioni.ID_destinazione = Imprese_Progetti.ID_REG ")
            StrSQL.AppendLine(" 	AND Movimenti.Data_Movimento >=  Imprese_Progetti.Validita_Inizio ")
            StrSQL.AppendLine("   AND Movimenti.Data_Movimento <=  Imprese_Progetti.Validita_Fine ")

            StrSQL.AppendLine("    WHERE 1 = 1 ")
            StrSQL.AppendLine("    AND Agenda.Lav_Cod = " & LAVCOD_ABBATTIMENTOIMPIANTI & " ")
            StrSQL.AppendLine("    AND Movimenti.Cau_Mov = '" & CAU_LAVORAZIONE & "' ")
            StrSQL.AppendLine("    AND Movimenti.Data_Movimento <=  " & Agro_SQL_SaveDate(dataReport, False) & " ")
            StrSQL.AppendLine("    GROUP BY Mov_Destinazioni.PIVA, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza,  Mov_Destinazioni.Id_Destinazione, Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" CTE_Perc_Piante_Morte AS ( ")
            StrSQL.AppendLine(" 	SELECT SUM(Mov_Destinazioni.qta) AS Perc_Piante_Morte, STRING_AGG(DR_DES, ', ') AS DanniDescrizione ")
            StrSQL.AppendLine(" 		 , Mov_Destinazioni.PIVA, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza,  Mov_Destinazioni.Id_Destinazione AS ID_Reg, Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine(" 	FROM Agenda ")
            StrSQL.AppendLine(" 	JOIN Movimenti ON ")
            StrSQL.AppendLine(" 		Movimenti.PIVA = Agenda.PIVA ")
            StrSQL.AppendLine(" 	AND Movimenti.Sa_Cod = Agenda.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Movimenti.Id_Agenda = Agenda.Id_Agenda ")

            StrSQL.AppendLine(" 	JOIN Movimenti_Dettagli ON ")
            StrSQL.AppendLine(" 		Movimenti_Dettagli.PIVA = Movimenti.PIVA ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Sa_Cod = Movimenti.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov ")

            StrSQL.AppendLine(" 	JOIN Mov_Destinazioni ON ")
            StrSQL.AppendLine(" 		Movimenti_Dettagli.PIVA = Mov_Destinazioni.Piva ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

            StrSQL.AppendLine(" 	JOIN Mov_Dettaglio_Tecnico ON ")
            StrSQL.AppendLine(" 		Movimenti_Dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda ")
            StrSQL.AppendLine(" 	AND Movimenti_Dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod ")

            StrSQL.AppendLine(" 	JOIN Imprese_Progetti ON ")
            StrSQL.AppendLine(" 		Mov_Destinazioni.PIVA = Imprese_Progetti.PIVA ")
            StrSQL.AppendLine("     AND Mov_Destinazioni.Sa_Cod = Imprese_Progetti.Sa_Cod ")
            StrSQL.AppendLine(" 	AND Mov_Destinazioni.APPEZZA = Imprese_Progetti.APPEZZA ")
            StrSQL.AppendLine("     AND Mov_Destinazioni.ID_destinazione = Imprese_Progetti.ID_REG ")
            StrSQL.AppendLine(" 	AND Movimenti.Data_Movimento >=  Imprese_Progetti.Validita_Inizio ")
            StrSQL.AppendLine("     AND Movimenti.Data_Movimento <=  Imprese_Progetti.Validita_Fine ")

            StrSQL.AppendLine("   LEFT JOIN DanniRaccolta ON")
            StrSQL.AppendLine("       Mov_Dettaglio_Tecnico.FF_Classe = DanniRaccolta.DR_COD")

            StrSQL.AppendLine("    WHERE 1 = 1 ")
            StrSQL.AppendLine("    AND Agenda.Lav_Cod = " & LAVCOD_DANNI_RACCOLTA & " ")
            StrSQL.AppendLine("    AND Movimenti.Cau_Mov = '" & CAU_RILIEVO_RACCOLTA & "' ")
            'StrSQL.AppendLine("    AND Mov_Dettaglio_Tecnico.ff_clASse IN (28,29) ")
            StrSQL.AppendLine("    AND Movimenti.Data_Movimento <=  " & Agro_SQL_SaveDate(dataReport, False) & " ")
            StrSQL.AppendLine("    GROUP BY Mov_Destinazioni.PIVA, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza,  Mov_Destinazioni.Id_Destinazione, Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine(" ) ")


            StrSQL.AppendLine("SELECT * FROM ( ")
            StrSQL.AppendLine("SELECT DISTINCT ")
            StrSQL.AppendLine("    Imprese.PIVA + '_' + CAST(Centri_Aziendali.sa_cod AS VARCHAR(20)) + '_' + CAST(Appezzamento.Appezza  AS VARCHAR(20)) + '_' + CAST(Reg_Impianti.id_reg AS VARCHAR(20)) + '_' + CAST(ISNULL(SpecieVegetali.veg_cod ,'0') AS VARCHAR(20)) + '_' + CAST(ISNULL(Imprese_Progetti.Progetto_Cod,'0') AS VARCHAR(20)) AS chiave ")
            StrSQL.AppendLine("  , Imprese.PIVA ")
            StrSQL.AppendLine("  , Imprese.rag_soc ")
            StrSQL.AppendLine("  , IC.val_cod AS Codice_Socio ")
            StrSQL.AppendLine("  , IC_cuaa.val_cod AS CUAA ")
            StrSQL.AppendLine("  , IC1324.val_cod AS Contratto_Produzione ")
            StrSQL.AppendLine("  , ISNULL ((SELECT TOP 1 Contatti.Cognome + ' ' + Contatti.Nome FROM Imprese_Codici INNER JOIN Contatti ON Imprese_Codici.val_cod = Contatti.Cod_Contatto WHERE Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.Tecnico & " AND Imprese_Codici.piva = Imprese.piva), '') AS Tecnico_Referente ")
            StrSQL.AppendLine("  , ISNULL ((SELECT TOP 1 Codice_Fiscale FROM Contatti WHERE Imprese.PIVA = Contatti.Cod_Contatto), ' ') AS Codice_Fiscale ")
            StrSQL.AppendLine("  , Indirizzi.ind_des + ' ' + Indirizzi.Cap + ' ' + ISNULL(Istat.LOCALITA, '') + ' ' + ISNULL(Istat.COMUNI_PROV, '') + ' ' + Indirizzi.Stato AS Indirizzo ")
            StrSQL.AppendLine("  , Centri_Aziendali.sa_cod ")
            StrSQL.AppendLine("  , Centri_Aziendali.sa_nome ")
            StrSQL.AppendLine("  , Indirizzi_Centro.ind_des AS Indirizzo_Centro, ISNULL(Istat_Centro.LOCALITA, '') AS Comune_Centro, ISNULL(Istat_Centro.COMUNI_PROV, '') AS Provincia_Centro, Indirizzi_Centro.Cap AS CAP_Centro, Indirizzi_Centro.Stato AS Stato_Centro ")
            StrSQL.AppendLine("  , SpecieVegetali.Veg_Cod ")
            StrSQL.AppendLine("  , Reg_Impianti.Cul_Cod ")
            StrSQL.AppendLine("  , SpecieVegetali.Veg_Des ")
            StrSQL.AppendLine("  , Cultivar.Cul_Des ")
            StrSQL.AppendLine("  , GruppoVarietale.GRVA_DES ")
            StrSQL.AppendLine("  , Campi.Campo_Cod ")
            StrSQL.AppendLine("  , Campi.Campo_Des ")
            StrSQL.AppendLine("  , CC1279.val_cod AS Codice_Campo ")
            StrSQL.AppendLine("  , Appezzamento.APPEZZA ")

            StrSQL.AppendLine("  , Appezzamento.SUP_APP ")
            StrSQL.AppendLine("  , Appezzamento.APP_NOME ")

            ' Anna 20/07/21 - Aggiunte info extra per gli appezzamenti: Nazione, Regione, Città
            StrSQL.AppendLine("  , CASE WHEN AppIndirizzi.stato IN ('ITALIA','Italy','It','Italia','italiano') ")
            StrSQL.AppendLine("         THEN 'IT' ")
            StrSQL.AppendLine("     ELSE AppIndirizzi.stato ")
            StrSQL.AppendLine("     END AS nazione ")
            StrSQL.AppendLine("  , AppIndirizzi.frz_des AS citta ")
            StrSQL.AppendLine("  , lista_regioni.Regione_Des AS regione ")

            StrSQL.AppendLine("  , Reg_Impianti.ID_REG ")
            StrSQL.AppendLine("  , Imprese_Progetti.Validita_Inizio ")
            StrSQL.AppendLine("  , Imprese_Progetti.Validita_Fine ")
            StrSQL.AppendLine("  , Reg_Impianti.Sup_Imp ")
            StrSQL.AppendLine("  , Imprese_Progetti.Regolamento_Cod AS Regolamento ")
            StrSQL.AppendLine("  , Imprese_Progetti.Disciplinare_Cod AS Finanziamento ")
            StrSQL.AppendLine("  , Reg_Impianti.Grfi_Cod ")
            StrSQL.AppendLine("  , Reg_Impianti.Grva_Cod_Veg ")
            StrSQL.AppendLine("  , Reg_Impianti.Cop_Cod ")
            StrSQL.AppendLine("  , Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine("  , Imprese_Progetti.Progetto_Nome AS Lotto ")
            StrSQL.AppendLine("  , Imprese_Progetti.Progetto_Des AS Descrizione ")
            StrSQL.AppendLine("  , Imprese_Progetti.Validita_Inizio AS Progetto_Validita_Inizio ")
            StrSQL.AppendLine("  , Imprese_Progetti.Validita_Fine AS Progetto_Validita_Fine ")
            StrSQL.AppendLine("  , IC.val_cod AS CodiceSocio ")
            StrSQL.AppendLine("  , IC_Cuaa.val_cod AS CodiceCuaa ")
            StrSQL.AppendLine("  , kpin.val_Cod AS kpin ")
            StrSQL.AppendLine("  , blockName.val_Cod AS blockName ")
            StrSQL.AppendLine("  , growernumber.val_Cod AS GrowerNumber ")
            StrSQL.AppendLine("  , Imprese_Progetti.Produzione_Prevista ")
            StrSQL.AppendLine("  , Imprese_Progetti.Username_Creazione, Imprese_Progetti.Username_Modifica, Imprese_Progetti.Data_Creazione, Imprese_Progetti.Data_Modifica ")

            ' Anna 30/07/21 - Aggiunte info extra:
            StrSQL.AppendLine("  , Data_Inizio_Produzione AS AnnoPrimaProduzione ")
            StrSQL.AppendLine("  , OTabelle_Parametri.descrizione AS Licenza ")
            StrSQL.AppendLine("  , CASE WHEN (Data_Inizio_Produzione) IS NULL ")
            StrSQL.AppendLine("            THEN 'NO' ")
            StrSQL.AppendLine("       ELSE CASE WHEN (YEAR(Data_Inizio_Produzione)<= YEAR(getdate()) AND YEAR(Data_Inizio_Produzione)>1900) ")
            StrSQL.AppendLine("            THEN 'SI' ")
            StrSQL.AppendLine("       ELSE 'NO' ")
            StrSQL.AppendLine("       END ")
            StrSQL.AppendLine("    END AS Produzione ")

            StrSQL.AppendLine("  , ISNULL(CTE_Data_Fioritura.Data_Fioritura, '') AS Data_Fioritura ")


            StrSQL.AppendLine(" , ISNULL(CTE_Sup_Abbattuta.Sup_Abbattuta, 0) AS Sup_Abbattuta ")
            StrSQL.AppendLine(" , ISNULL(CTE_Perc_Piante_Morte.Perc_Piante_Morte, NULL) AS Perc_Piante_Morte ")
            StrSQL.AppendLine(" , CTE_Perc_Piante_Morte.DanniDescrizione")
            StrSQL.AppendLine(" , CASE WHEN ISNULL(CTE_Sup_Abbattuta.Sup_Abbattuta, 0) >=  reg_impianti.sup_imp ")
            StrSQL.AppendLine("     THEN 'NO' ")
            StrSQL.AppendLine("     ELSE 'SI' ")
            StrSQL.AppendLine("   END AS ImpiantoAttivo ")

            StrSQL.AppendLine(" , CASE WHEN ISNULL(CTE_Sup_Abbattuta.Sup_Abbattuta, 0) != reg_impianti.sup_imp ") '1) ATTIVO
            StrSQL.AppendLine("     THEN ")

            StrSQL.AppendLine("        CASE WHEN (YEAR(Data_Inizio_Produzione) <= YEAR(Getdate())) ") '2) PRODUCING
            StrSQL.AppendLine("             THEN ")

            'Se ABBATTUTO PARZIALE + RILIEVO DANNI allora il calcolo diventa:
            'Stima prod = ((Ha impianto – Ha abbattimento) - (Ha impianto – Ha abbattimento) x %Danno) x Resa Impianto 
            StrSQL.AppendLine("                 CASE WHEN ISNULL(CTE_Sup_Abbattuta.Sup_Abbattuta, 0) < sup_imp ") '3A) MORIA + ABBATTIMENTO PARZIALE
            StrSQL.AppendLine("                 AND ISNULL(CTE_Perc_Piante_Morte.Perc_Piante_Morte, 0) > 0 ")
            StrSQL.AppendLine("                     THEN ((sup_imp - ISNULL(CTE_Sup_Abbattuta.Sup_Abbattuta, 0)) - ((sup_imp - ISNULL(CTE_Sup_Abbattuta.Sup_Abbattuta, 0)) * ISNULL(CTE_Perc_Piante_Morte.Perc_Piante_Morte, 0) / 100)) * produzione_prevista ")

            'Se RILIEVO DANNI, che è espressa in percentuale, allora il calcolo diventa:
            'Stima prod = (Ha impianto – Ha Impianto x %Danno) x Resa Impianto 
            StrSQL.AppendLine("                 WHEN ISNULL(CTE_Perc_Piante_Morte.Perc_Piante_Morte, 0) > 0 ") '3B) MORIA
            StrSQL.AppendLine("                     THEN (sup_imp - (sup_imp * ISNULL(CTE_Perc_Piante_Morte.Perc_Piante_Morte, 0)) /100 * produzione_prevista) ")

            'Se ABBATTUTO PARZIALE allora il calcolo diventa:
            'Stima prod = (Ha impianto – Ha abbattimento) x Resa Impianto  
            StrSQL.AppendLine("                 WHEN ISNULL(CTE_Sup_Abbattuta.Sup_Abbattuta, 0) < sup_imp ") '3C) ABBATTIMENTO PARZIALE
            StrSQL.AppendLine("                     THEN (sup_imp - ISNULL(CTE_Sup_Abbattuta.Sup_Abbattuta, 0)) * produzione_prevista ")

            StrSQL.AppendLine("                 ELSE ") '3D) NESSUN DANNO - TUTTO OK 
            StrSQL.AppendLine("                      imprese_progetti.produzione_prevista * reg_impianti.sup_imp ")
            StrSQL.AppendLine("                 END ")
            StrSQL.AppendLine("         ELSE ") '2) NON PRODUCING
            StrSQL.AppendLine("              0 ")
            StrSQL.AppendLine("         END ")
            StrSQL.AppendLine("  ELSE 0 ") '1) NON ATTIVO
            StrSQL.AppendLine("  END AS Stime_Produzione ")
            StrSQL.AppendLine("  , Imprese_Progetti.Data_Fine_Prevista AS Data_Raccolta_Prevista ")

            StrSQL.AppendLine(" FROM Imprese ")

            StrSQL.AppendLine(" INNER JOIN GerarchiaImprese ON ")
            StrSQL.AppendLine("     Imprese.Piva = GerarchiaImprese.Figlio ")

            StrSQL.AppendLine(" INNER JOIN ImpresexIndirizzi ON ")
            StrSQL.AppendLine("     Imprese.PIVA = ImpresexIndirizzi.PIVA ")
            StrSQL.AppendLine(" AND ImpresexIndirizzi.Tipo_Indirizzo = 1 ")

            StrSQL.AppendLine(" LEFT JOIN Indirizzi ON ")
            StrSQL.AppendLine("     ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo ")

            StrSQL.AppendLine(" LEFT JOIN ISTAT ON ")
            StrSQL.AppendLine("     Indirizzi.pro_cod_istat = ISTAT.PROV ")
            StrSQL.AppendLine(" AND Indirizzi.com_cod_istat = ISTAT.COM ")

            'StrSQL.AppendLine(" INNER JOIN Lista_Province ON Lista_Province.Sigla = ISTAT.COMUNI_PROV ")
            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali ON ")
            StrSQL.AppendLine("     Imprese.Piva = Centri_Aziendali.Piva ")

            StrSQL.AppendLine(" LEFT JOIN CentrixIndirizzi ON ")
            StrSQL.AppendLine("     CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA ")
            StrSQL.AppendLine(" AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
            StrSQL.AppendLine(" AND CentrixIndirizzi.Tipo_Indirizzo = 1 ")

            StrSQL.AppendLine(" LEFT JOIN Indirizzi AS Indirizzi_Centro ON ")
            StrSQL.AppendLine("     Indirizzi_Centro.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")

            StrSQL.AppendLine(" LEFT JOIN ISTAT AS Istat_Centro ON ")
            StrSQL.AppendLine("     Indirizzi_Centro.pro_cod_istat = Istat_Centro.PROV ")
            StrSQL.AppendLine(" AND Indirizzi_Centro.com_cod_istat = Istat_Centro.COM ")

            StrSQL.AppendLine(" LEFT JOIN Appezzamento ON ")
            StrSQL.AppendLine("     Centri_Aziendali.Piva = Appezzamento.Piva ")
            StrSQL.AppendLine(" AND Centri_Aziendali.Sa_Cod = Appezzamento.Sa_Cod ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti ON ")
            StrSQL.AppendLine("     Appezzamento.Piva = Reg_Impianti.Piva ")
            StrSQL.AppendLine(" AND Appezzamento.SA_Cod = Reg_Impianti.Sa_Cod ")
            StrSQL.AppendLine(" AND Appezzamento.Appezza = Reg_Impianti.Appezza ")

            StrSQL.AppendLine(" INNER JOIN Imprese_Progetti ON ")
            StrSQL.AppendLine("     Imprese_Progetti.Piva = Reg_Impianti.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.SA_Cod = Reg_Impianti.Sa_Cod ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg ")

            StrSQL.AppendLine(" INNER JOIN #TempProgetti tempP ON ")
            StrSQL.AppendLine("     Imprese_Progetti.Progetto_Cod = tempP.Progetto_Cod ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici kpin  ON ")
            StrSQL.AppendLine("     Imprese_Progetti.Piva = kpin.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.SA_Cod = kpin.Sa_Cod ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = kpin.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = kpin.Id_Reg ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = kpin.Progetto_Cod ")
            StrSQL.AppendLine(" AND kpin.id_Cod = " & enum_CodiciAnagrafe.Zespri_Codice_kPIN & " ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici blockName ON ")
            StrSQL.AppendLine("     Imprese_Progetti.Piva = blockName.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.SA_Cod = blockName.Sa_Cod ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = blockName.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = blockName.Id_Reg ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = blockName.Progetto_Cod ")
            StrSQL.AppendLine(" AND blockName.id_Cod = " & enum_CodiciAnagrafe.Zespri_Block_Name & " ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici growerNumber ON ")
            StrSQL.AppendLine("     Imprese_Progetti.Piva = growerNumber.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.SA_Cod = growerNumber.Sa_Cod ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = growerNumber.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = growerNumber.Id_Reg ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = growerNumber.Progetto_Cod ")
            StrSQL.AppendLine(" AND growerNumber.id_Cod = " & enum_CodiciAnagrafe.Zepri_Grower_Number & " ")

            StrSQL.AppendLine(" LEFT OUTER JOIN Imprese_Codici IC ON ")
            StrSQL.AppendLine("     IC.Piva = Imprese.Piva AND IC.id_cod = " & enum_CodiciAnagrafe.Codice_Socio & " ")

            StrSQL.AppendLine(" LEFT OUTER JOIN Imprese_Codici IC_cuaa ON ")
            StrSQL.AppendLine("     IC_cuaa.Piva = Imprese.Piva ")
            StrSQL.AppendLine(" AND IC_cuaa.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA & " ")

            StrSQL.AppendLine(" LEFT OUTER JOIN Campi ON ")
            StrSQL.AppendLine("     Campi.Piva = Appezzamento.Piva ")
            StrSQL.AppendLine(" AND Campi.Sa_Cod = Appezzamento.Sa_Cod ")
            StrSQL.AppendLine(" AND Campi.Campo_Cod = Appezzamento.Campo_Cod ")

            StrSQL.AppendLine(" LEFT JOIN Campi_Codici CC1279 ON ")
            StrSQL.AppendLine("     CC1279.Piva = Campi.Piva ")
            StrSQL.AppendLine(" AND CC1279.SA_Cod = Campi.Sa_Cod ")
            StrSQL.AppendLine(" AND CC1279.Campo_Cod = Campi.Campo_Cod ")
            StrSQL.AppendLine(" AND CC1279.id_Cod = " & enum_CodiciAnagrafe.Riferimento_Alfanumerico_Campo & " ")

            StrSQL.AppendLine(" LEFT JOIN Campi_Codici CC1325 ON ")
            StrSQL.AppendLine("     CC1325.Piva = Campi.Piva ")
            StrSQL.AppendLine(" AND CC1325.SA_Cod = Campi.Sa_Cod ")
            StrSQL.AppendLine(" AND CC1325.Campo_Cod = Campi.Campo_Cod ")
            StrSQL.AppendLine(" AND CC1325.id_Cod = " & enum_CodiciAnagrafe.Sup_Contratto & " ")

            StrSQL.AppendLine(" LEFT JOIN Campi_Codici CC1326 ON ")
            StrSQL.AppendLine("     CC1326.Piva = Campi.Piva ")
            StrSQL.AppendLine(" AND CC1326.SA_Cod = Campi.Sa_Cod ")
            StrSQL.AppendLine(" AND CC1326.Campo_Cod = Campi.Campo_Cod ")
            StrSQL.AppendLine(" AND CC1326.id_Cod = " & enum_CodiciAnagrafe.Filiera & " ")

            StrSQL.AppendLine(" LEFT JOIN Imprese_Codici IC1324 ON ")
            StrSQL.AppendLine("     IC1324.Piva = Imprese.Piva ")
            StrSQL.AppendLine(" AND IC1324.id_Cod = " & enum_CodiciAnagrafe.Contratto_Produzione & " ")

            StrSQL.AppendLine(" LEFT OUTER JOIN GruppoVarietale ON ")
            StrSQL.AppendLine("      abs(Reg_Impianti.GRVA_Cod_VEG) = GruppoVarietale.GRVA_COD ")

            StrSQL.AppendLine(" LEFT OUTER JOIN Cultivar ON ")
            StrSQL.AppendLine("     Reg_Impianti.CUL_COD = Cultivar.CUL_COD ")

            StrSQL.AppendLine(" LEFT OUTER JOIN SpecieVegetali ON ")
            StrSQL.AppendLine("     Cultivar.VEG_COD = SpecieVegetali.VEG_COD ")

            ' Anna 20/07/21 - Aggiunte info extra per gli appezzamenti: Nazione, Regione, Città
            StrSQL.AppendLine(" LEFT JOIN AppezzamentixIndirizzi ON ")
            StrSQL.AppendLine("     Appezzamento.PIVA = AppezzamentixIndirizzi.PIVA ")
            StrSQL.AppendLine(" AND Appezzamento.SA_COD = AppezzamentixIndirizzi.SA_COD ")
            StrSQL.AppendLine(" AND Appezzamento.APPEZZA = AppezzamentixIndirizzi.APPEZZA ")

            StrSQL.AppendLine(" LEFT JOIN Indirizzi AppIndirizzi ON ")
            StrSQL.AppendLine("     AppezzamentixIndirizzi.cod_indirizzo = AppIndirizzi.cod_indirizzo ")

            StrSQL.AppendLine(" LEFT JOIN Lista_Province ON ")
            StrSQL.AppendLine("     AppIndirizzi.pro_cod_istat = Lista_Province.PROV ")

            StrSQL.AppendLine(" LEFT JOIN Lista_Regioni ON ")
            StrSQL.AppendLine("     Lista_Province.REG = Lista_Regioni.REG ")
            StrSQL.AppendLine(" AND Lista_Province.REG <> '000' ")

            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti_Codici LicenzaCod ON ")
            StrSQL.AppendLine("     Imprese_Progetti.Piva = LicenzaCod.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.SA_Cod = LicenzaCod.Sa_Cod ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = LicenzaCod.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = LicenzaCod.Id_Reg ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = LicenzaCod.Progetto_Cod ")
            StrSQL.AppendLine(" AND LicenzaCod.id_cod IN ( " & enum_CodiciAnagrafe.Zespri_Fasi_Fase & " ) ")

            StrSQL.AppendLine(" LEFT JOIN OTabelle_Parametri ON OTabelle_Parametri.tabella_cod  = LicenzaCod.id_cod AND OTabelle_Parametri.tabella_par_cod  = LicenzaCod.val_cod  " & vbCrLf)

            StrSQL.AppendLine(" LEFT JOIN CTE_Data_Fioritura ON ")
            StrSQL.AppendLine("	 CTE_Data_Fioritura.Piva = Imprese_Progetti.Piva ")
            StrSQL.AppendLine(" AND CTE_Data_Fioritura.Sa_Cod = Imprese_Progetti.Sa_Cod ")
            StrSQL.AppendLine(" AND CTE_Data_Fioritura.Appezza = Imprese_Progetti.Appezza ")
            StrSQL.AppendLine(" AND CTE_Data_Fioritura.ID_Reg = Imprese_Progetti.Id_Reg ")
            StrSQL.AppendLine(" AND CTE_Data_Fioritura.Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine(" LEFT JOIN CTE_Sup_Abbattuta ON ")
            StrSQL.AppendLine("	 CTE_Sup_Abbattuta.Piva = Imprese_Progetti.Piva ")
            StrSQL.AppendLine(" AND CTE_Sup_Abbattuta.Sa_Cod = Imprese_Progetti.Sa_Cod ")
            StrSQL.AppendLine(" AND CTE_Sup_Abbattuta.Appezza = Imprese_Progetti.Appezza ")
            StrSQL.AppendLine(" AND CTE_Sup_Abbattuta.ID_Reg = Imprese_Progetti.Id_Reg ")
            StrSQL.AppendLine(" AND CTE_Sup_Abbattuta.Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine(" LEFT JOIN CTE_Perc_Piante_Morte ON ")
            StrSQL.AppendLine("	 CTE_Perc_Piante_Morte.Piva = Imprese_Progetti.Piva ")
            StrSQL.AppendLine(" AND CTE_Perc_Piante_Morte.Sa_Cod = Imprese_Progetti.Sa_Cod ")
            StrSQL.AppendLine(" AND CTE_Perc_Piante_Morte.Appezza = Imprese_Progetti.Appezza ")
            StrSQL.AppendLine(" AND CTE_Perc_Piante_Morte.ID_Reg = Imprese_Progetti.Id_Reg ")
            StrSQL.AppendLine(" AND CTE_Perc_Piante_Morte.Progetto_Cod = Imprese_Progetti.Progetto_Cod ")


            StrSQL.AppendLine(" ")

            StrSQL.AppendLine(" ) AS REPORT_STIME_PRODUZIONE ")

            ' filtro esercizi selezionati
            'If filtro <> "" Then
            '    StrSQL.AppendLine(" WHERE " & filtro)
            'End If

            If ordinamento <> "" Then
                StrSQL.AppendLine(" ORDER BY " & ordinamento)
            Else
                StrSQL.AppendLine(" ORDER BY piva, sa_cod, Appezza, Id_Reg DESC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' Eliminazione tabella temporanea
            EseguiQuery_Scrittura(objParametri, EliminaTabellaTemp_FiltroProgetti, NomeRoutine)

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri)

        Catch ex As Exception

            ' Rollback
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)

        Finally

            ConnessioniTransazioni.ChiudiConnessione(objParametri)
            ''Utility.VerificaChiudiConnessione(objParametri, False)

        End Try

        Return DT

    End Function

    Public Function Leggi_Statistiche_PrevisioniAI(Piva As String,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Statistiche.Leggi_Statistiche_PrevisioniAI()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.AppendLine(" SELECT Istat_Centro.LOCALITA, Istat_Centro.Stato_Country, SpecieVegetali.Veg_Des, ")
            StrSQL.AppendLine(" Cultivar.Cul_Des, Appezzamento.APP_NOME, Appezzamento.SUP_APP,  ")
            StrSQL.AppendLine(" CASE when Imprese_Progetti.Regolamento_Cod = 4 THEN 1 ELSE 0 END AS AgrBio ")
            StrSQL.AppendLine(" FROM Imprese ")
            'StrSQL.AppendLine(" INNER JOIN ImpresexIndirizzi ON ")
            'StrSQL.AppendLine(" Imprese.PIVA = ImpresexIndirizzi.PIVA ")
            'StrSQL.AppendLine(" AND ImpresexIndirizzi.Tipo_Indirizzo = 1 ")
            'StrSQL.AppendLine(" LEFT JOIN Indirizzi ON ")
            'StrSQL.AppendLine(" ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo ")
            'StrSQL.AppendLine(" LEFT JOIN ISTAT ON ")
            'StrSQL.AppendLine(" Indirizzi.pro_cod_istat = ISTAT.PROV ")
            'StrSQL.AppendLine(" AND Indirizzi.com_cod_istat = ISTAT.COM ")
            StrSQL.AppendLine(" LEFT JOIN Centri_Aziendali ON ")
            StrSQL.AppendLine(" Imprese.Piva = Centri_Aziendali.Piva ")
            StrSQL.AppendLine(" LEFT JOIN CentrixIndirizzi ON ")
            StrSQL.AppendLine(" CentrixIndirizzi.PIVA = Centri_Aziendali.PIVA ")
            StrSQL.AppendLine(" AND CentrixIndirizzi.sa_cod = Centri_Aziendali.sa_cod ")
            StrSQL.AppendLine(" AND CentrixIndirizzi.Tipo_Indirizzo = 1 ")
            StrSQL.AppendLine(" LEFT JOIN Indirizzi AS Indirizzi_Centro ON ")
            StrSQL.AppendLine(" Indirizzi_Centro.cod_indirizzo = CentrixIndirizzi.cod_indirizzo ")
            StrSQL.AppendLine(" LEFT JOIN ISTAT AS Istat_Centro ON ")
            StrSQL.AppendLine(" Indirizzi_Centro.pro_cod_istat = Istat_Centro.PROV ")
            StrSQL.AppendLine(" AND Indirizzi_Centro.com_cod_istat = Istat_Centro.COM ")
            StrSQL.AppendLine(" LEFT JOIN Appezzamento ON ")
            StrSQL.AppendLine(" Centri_Aziendali.Piva = Appezzamento.Piva ")
            StrSQL.AppendLine(" AND Centri_Aziendali.Sa_Cod = Appezzamento.Sa_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Reg_Impianti ON ")
            StrSQL.AppendLine(" Appezzamento.Piva = Reg_Impianti.Piva ")
            StrSQL.AppendLine(" AND Appezzamento.SA_Cod = Reg_Impianti.Sa_Cod ")
            StrSQL.AppendLine(" AND Appezzamento.Appezza = Reg_Impianti.Appezza ")
            StrSQL.AppendLine(" INNER JOIN Imprese_Progetti ON ")
            StrSQL.AppendLine(" Imprese_Progetti.Piva = Reg_Impianti.Piva ")
            StrSQL.AppendLine(" AND Imprese_Progetti.SA_Cod = Reg_Impianti.Sa_Cod ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Cultivar ON ")
            StrSQL.AppendLine(" Reg_Impianti.CUL_COD = Cultivar.CUL_COD ")
            StrSQL.AppendLine(" LEFT OUTER JOIN SpecieVegetali ON ")
            StrSQL.AppendLine(" Cultivar.VEG_COD = SpecieVegetali.VEG_COD ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Imprese.PIVA = " & Agro_SQL_SaveText_NULL(Piva))
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Private Sub GruppiMerce_SqlCreaTabellaTempDefault(ByRef objParametri As AgronicaCoreParametri, ByRef queryGenerale As StringBuilder)

        Dim objGruppiMerce As New AgronicaCoreAnagrafeDAL.Gruppi_Merce_R

        Dim ListaGruppiMercePerCategoria = objGruppiMerce.GetListGruppiMerceDefault_MultiAzienda(_listaImprese, objParametri)
        Dim sqlCreaTempGruppiMerce As StringBuilder = objGruppiMerce.ComponiSql_CreaTempDefaultGruppiMerce(_listaImprese, objParametri, ListaGruppiMercePerCategoria)

        queryGenerale.AppendLine(sqlCreaTempGruppiMerce.ToString() & ";")
        queryGenerale.AppendLine("")

    End Sub

    Private Sub GruppiMerce_SqlCancellaTabellaTempDefault(ByRef objParametri As AgronicaCoreParametri, ByRef queryGenerale As StringBuilder)

        Dim objGruppiMerce As New AgronicaCoreAnagrafeDAL.Gruppi_Merce_R

        Dim sqlCancellaTempGruppiMerce As StringBuilder = objGruppiMerce.ComponiSql_CancellaTempDefaultGruppiMerce()
        queryGenerale.AppendLine("")
        queryGenerale.AppendLine(sqlCancellaTempGruppiMerce.ToString())

    End Sub

End Class

Public Class Statistiche_W
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Aggiorna cubo report vendite
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Crea_Cubo_Report_Vendite(ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Statistiche.Crea_Cubo_Report_Vendite()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            ' Crea cubo report vendite
            Dim leggi As New Statistiche_R
            StrSQL.AppendLine("CREATE VIEW V_Report_Vendite AS ")
            Dim QuerySQL = leggi.Leggi_Query_Report_Vendite("",
                                                            Statistiche_R.Report_Vendite,
                                                            Nothing,
                                                            Nothing,
                                                            "select * from (",
                                                            objParametri)
            StrSQL.AppendLine(QuerySQL & ")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "]  " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Aggiorna cubo report vendite
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Aggiorna_Cubo_Report_Vendite(ByVal piva As String, ByVal dataDal As String, ByVal dataAl As String, ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.Statistiche.Aggiorna_Cubo_Report_Vendite()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            ' Verifico se è presente la tabella Report_Vendite
            Dim leggi As New Statistiche_R
            Dim esisteTabella = leggi.Esiste_Report(Statistiche_R.Report_Vendite, objParametri)

            ' Crea / Aggiorna Tabella Cubo
            If Not esisteTabella OrElse String.IsNullOrEmpty(piva) Then

                Dim esisteVista = leggi.Esiste_Report(Statistiche_R.Report_Vendite, objParametri)

                If Not esisteVista Then
                    Crea_Cubo_Report_Vendite(objParametri)
                End If

                If esisteTabella Then
                    StrSQL.AppendLine("DROP TABLE Report_Vendite ")
                End If

                StrSQL.AppendLine("SELECT * INTO Report_Vendite FROM V_Report_Vendite WHERE 0= 0 ")

            Else

                ' Cancella dati presenti
                StrSQL.AppendLine("DELETE FROM Report_Vendite WHERE 0=0 ")
                If Not String.IsNullOrEmpty(piva) Then
                    StrSQL.AppendLine(" And (PIVA = '" & Agro_SQL_SaveText(piva) & "') ")
                End If
                If Not String.IsNullOrEmpty(dataDal) Then
                    StrSQL.AppendLine(" AND (Data_Movimento >= " & Agro_SQL_SaveDate(Convert.ToDateTime(dataDal)) & " ) ")
                End If
                If Not String.IsNullOrEmpty(dataAl) Then
                    StrSQL.AppendLine(" AND (Data_Movimento <= " & Agro_SQL_SaveDate(Convert.ToDateTime(dataAl)) & " ) ")
                End If

                ' Inserisce dati aggiornati
                StrSQL.AppendLine("INSERT INTO Report_Vendite SELECT * FROM V_Report_Vendite WHERE 0=0 ")

            End If

            ' Filtro PIVA
            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine(" AND (PIVA = '" & Agro_SQL_SaveText(piva) & "') ")
            End If

            ' Filtro Date
            If Not String.IsNullOrEmpty(dataDal) Then
                StrSQL.AppendLine(" AND (Data_Movimento >= " & Agro_SQL_SaveDate(Convert.ToDateTime(dataDal)) & " ) ")
            End If
            If Not String.IsNullOrEmpty(dataAl) Then
                StrSQL.AppendLine(" AND (Data_Movimento <= " & Agro_SQL_SaveDate(Convert.ToDateTime(dataAl)) & " ) ")
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

End Class
