Imports System.Data.Common
Imports System.Xml
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports System.Runtime.CompilerServices

'Creo una funzione di aggregazione personalizzata da usare in Linq To Dataset, per mantenere tutti i valori stringa nei record che vengono aggregati.
'Simile alla funzione T-Sql String_Agg con le differenze che il separatore in questa funzione non è modificabile ma permette in più di non ripetere i valori se sono uguali
'https://docs.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2010/bb531251(v=vs.100)#creating-user-defined-aggregate-functions
Module UserDefinedAggregates

    <Extension()>
    Function String_Join(ByVal _values As IEnumerable(Of String)) As String

        Dim firstValue = _values.First()

        'Se tutti i valori sono uguali al primo, allora non effettuo la concatenazione ma lo restituisco solo una volta
        If _values.All(Function(val) val = firstValue) Then
            Return firstValue
        Else
            Return String.Join("<br/>", _values)
        End If

    End Function

    <Extension()>
    Function String_Join(Of T)(ByVal values As IEnumerable(Of T),
                               ByVal selector As Func(Of T, String)) As String
        Return (From element In values Select selector(element)).String_Join()
    End Function

End Module


Public Class Movimenti_Dettagli_R
    Inherits AgronicaCoreDataProvider.LogProvider


    '============================================================================
    Public Function Movimento_Dettaglio_Leggi(ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByRef Id_Agenda As Integer,
                                              ByRef Id_Mov As Integer,
                                              ByRef Id_Mov_Det As Integer,
                                              ByRef Contabilizzato As Integer,
                                              ByRef Pendente As Integer,
                                              ByRef Appezza As Integer,
                                              ByVal Id_Destinazione As Integer,
                                              ByRef Tipo_Destinazione As Integer,
                                              ByVal Id_Reg_Dettaglio As Integer,
                                              ByVal Lav_Cod As Integer,
                                              ByVal Cau_Mov As String,
                                              ByVal Doc_Numero As Decimal,
                                              ByVal Num_Protocollo As Decimal,
                                              ByVal Validita_Inizio_Mov As Date,
                                              ByVal Validita_Fine_Mov As Date,
                                              ByVal Scadenza_Fine As Date,
                                              ByVal Elem_Cod As Integer,
                                              ByVal Pro_Cod As Integer,
                                              ByVal Mat_Cod As Integer,
                                              ByVal Cal_Cod As Integer,
                                              ByVal Cod_Conto As Integer,
                                              ByVal Cod_Progetto As Integer,
                                              ByVal ForDelete As Boolean,
                                              ByVal Fase_Cod As Integer,
                                              ByRef objParametri As AgronicaCoreParametri,
                                              Optional ByVal LeggiRiferimentiInversi As Boolean = True
                                              ) As String

        Const nomeRoutine = "ContabBIZ.Movimenti_Dettagli_R.Movimento_Dettaglio_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese dell'utente
        '====================================================================================

        Dim messaggioErrore As String = ""

        Dim flagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction
        Dim i As Integer
        Dim j As Integer

        Dim risultatoFunzione As String = String.Empty

        Dim xmlDoc As XmlDocument
        Dim xmlDatiMovimentiDettagli As XmlElement
        Dim xmlMovimentiDettagli As XmlElement
        Dim xmlMovDetRiferimenti As XmlElement
        Dim xmlMovDestinazione As XmlElement
        Dim xmlMovDettaglioTecnico As XmlElement
        Dim xmlCampionatura As XmlElement

        Dim objMovimentiDettagli As AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim objMovDetRiferimenti As AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
        Dim objMovDestinazioni As AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim objMovDettaglioTecnico As AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
        Dim objMpCampionature As AgronicaCoreContabDAL.Materie_Prime_Campionature_R

        Dim dtMovimentiDettagli As DataTable
        Dim dtMovDetRiferimenti As DataTable
        Dim dtMovDestinazioni As DataTable
        Dim dtMovDettaglioTecnico As DataTable
        Dim dtCampionature As DataTable

        '------------------------------


        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                flagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                xTransazione = Nothing
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
                xTransazione = objParametri.objTransazione
            End If

            '------------------------------

            'Mi procuro un elenco dei dettagli di movimenti associati all'Impresa
            'all'interno della finestra temporale selezionata

            objMovimentiDettagli = New AgronicaCoreContabDAL.Movimenti_Dettagli_R

            'Mi procuro il RecordSet richiesto
            dtMovimentiDettagli = objMovimentiDettagli.Leggi(CStr(Piva),
                                                             CInt(Sa_Cod),
                                                             CInt(Id_Agenda),
                                                             CInt(Id_Mov),
                                                             CInt(Id_Mov_Det),
                                                             CInt(Elem_Cod),
                                                             CInt(Pro_Cod),
                                                             CInt(Mat_Cod),
                                                             CStr(Cau_Mov),
                                                             CInt(Cal_Cod),
                                                             CInt(Cod_Progetto),
                                                             CInt(Fase_Cod),
                                                             CInt(Contabilizzato),
                                                             CInt(Pendente),
                                                             CInt(Cod_Conto),
                                                             enumSelezioneVariabile.Selezione_JoinCompleta,
                                                             "",
                                                             "",
                                                             objParametri)

            'Se ottengo almeno un risultato, creo la struttura XML
            If dtMovimentiDettagli.Rows.Count > 0 Then

                '----- < Documento XML > -----
                xmlDoc = New XmlDocument

                xmlDatiMovimentiDettagli = xmlDoc.CreateElement("DatiMovimenti_Dettagli")


                For i = 0 To dtMovimentiDettagli.Rows.Count - 1

                    '----- < MOVIMENTO_DETTAGLIO > -----
                    xmlMovimentiDettagli = xmlDoc.CreateElement("Movimento_Dettaglio")

                    With xmlMovimentiDettagli
                        .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                        .SetAttribute("piva", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("PIVA")))
                        .SetAttribute("sa_cod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Sa_Cod")))
                        .SetAttribute("lav_cod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Lav_Cod")))
                        .SetAttribute("id_agenda", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Id_Agenda")))
                        .SetAttribute("id_mov", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Id_Mov")))
                        .SetAttribute("id_mov_det", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Id_Mov_Det")))
                        .SetAttribute("elem_cod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Elem_Cod")))
                        .SetAttribute("pro_cod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Pro_Cod")))
                        .SetAttribute("mat_cod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Mat_Cod")))
                        .SetAttribute("mov_det_des", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Mov_Det_Des")))
                        .SetAttribute("cau_mov", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Cau_Mov")))
                        .SetAttribute("qta", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Qta")))
                        .SetAttribute("old_qta", 0)
                        .SetAttribute("udm_cod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Udm_Cod")))
                        .SetAttribute("cod_iva", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Cod_Iva")))
                        .SetAttribute("sconto", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Sconto")))
                        .SetAttribute("prezzo_unitario", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Prezzo_Unitario")))
                        .SetAttribute("prezzo_unitario_netto", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Prezzo_Unitario_Netto")))
                        .SetAttribute("prezzo_effettivo", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Prezzo_Effettivo")))
                        .SetAttribute("old_prezzo_unitario", 0)
                        .SetAttribute("jolly_int", Agro_SQL_SaveNum(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Jolly_Int"))))
                        .SetAttribute("tipo_sconto", Agro_SQL_SaveNum(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Tipo_Sconto"))))
                        .SetAttribute("cal_cod", Agro_SQL_SaveNum(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Cal_Cod"))))
                        .SetAttribute("cod_conto", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Cod_Conto")))
                        .SetAttribute("cod_progetto", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Cod_Progetto")))
                        .SetAttribute("fase_cod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Fase_Cod")))
                        .SetAttribute("contabilizzato", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Contabilizzato")))
                        .SetAttribute("pendente", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Pendente")))
                        .SetAttribute("extra_str", Agro_SQL_SaveText(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Extra_Str"))))
                        .SetAttribute("extra_int", Agro_SQL_SaveNum(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Extra_Int"))))
                        .SetAttribute("extra_date", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Extra_Date")))
                        .SetAttribute("ric_cod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Ric_Cod")))
                        .SetAttribute("anno", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Anno")))
                        .SetAttribute("imponibile", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Imponibile")))
                        .SetAttribute("imponibile_netto", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Imponibile_Netto")))
                        .SetAttribute("iva", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Iva")))
                        .SetAttribute("lotto", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Lotto")))
                        .SetAttribute("data_movimento", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Data_Movimento")))
                        .SetAttribute("ora", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Ora")))
                        .SetAttribute("udm_cod_extra", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Udm_Cod_Extra")))
                        .SetAttribute("qta_extra", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Qta_Extra")))
                        .SetAttribute("qta_extra_totale", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Qta_Extra_Totale")))
                        .SetAttribute("variazione", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Variazione")))
                        .SetAttribute("tara", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Tara")))
                        .SetAttribute("listino_cod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Listino_Cod")))
                        .SetAttribute("chklayout_hide", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("ChkLayOut_Hide")))
                        .SetAttribute("chkiva_manuale", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("ChkIva_Manuale")))
                        .SetAttribute("cod_ivaindetraibile", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Cod_IvaIndetraibile")))
                        .SetAttribute("doc_numero", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Doc_Numero")))
                        .SetAttribute("doc_numero_sin", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Doc_Numero_Sin")))
                        .SetAttribute("doc_numero_des", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Doc_Numero_Des")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Validita_Fine")))
                        .SetAttribute("tempocarenza", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("tempocarenza")))
                        .SetAttribute("doseetichetta", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("doseetichetta")))
                        .SetAttribute("doseetichetta_value", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("DoseEtichetta_Value")))

                        '  Marco Grilli, 12/09/2014 11:26:08: Aggiungo in quanto mancavano per il G2G
                        .SetAttribute("data_creazione", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("data_creazione")))
                        .SetAttribute("data_modifica", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("data_modifica")))
                        .SetAttribute("username_creazione", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("username_creazione")))
                        .SetAttribute("username_modifica", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("username_modifica")))

                        '  Giulia, 08/05/2017 12:09:50: mancavano molte colonne contabili usate dal LAN
                        .SetAttribute("principiattivi", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("PrincipiAttivi")))
                        .SetAttribute("principiattivipesi", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("PrincipiAttivipesi")))
                        .SetAttribute("buffer", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("buffer")))
                        .SetAttribute("classitossicologiche", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("ClassiTossicologiche")))
                        .SetAttribute("turno_cod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Turno_Cod")))
                        .SetAttribute("id_attivita", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Id_Attivita")))
                        .SetAttribute("qualifica_cod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Qualifica_Cod")))
                        .SetAttribute("tariffa_cod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Tariffa_Cod")))
                        .SetAttribute("sconto_listino", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Sconto_Listino")))
                        .SetAttribute("sconto_modalita", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Sconto_Modalita")))
                        .SetAttribute("mat_cod_alias", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Mat_Cod_Alias")))
                        .SetAttribute("mezzo_det", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Mezzo_Det")))
                        .SetAttribute("sconto_testo", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Sconto_Testo")))
                        .SetAttribute("ric_cod_pat", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Ric_Cod_Pat")))
                        .SetAttribute("cod_conto_pat", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Cod_Conto_Pat")))
                        .SetAttribute("dettaglio_vegcod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Dettaglio_VegCod")))
                        .SetAttribute("iva_indetraibile", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Iva_Indetraibile")))
                        .SetAttribute("iva_indetraibile_perc", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Iva_Indetraibile_Perc")))
                        .SetAttribute("iva_deto_cod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Iva_Deto_Cod")))
                        .SetAttribute("qta_dettaglio1", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Qta_Dettaglio1")))
                        .SetAttribute("qta_dettaglio2", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Qta_Dettaglio2")))
                        .SetAttribute("dettagli_blocco_flag", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Dettagli_Blocco_Flag")))
                        .SetAttribute("dettagli_blocco_username", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Dettagli_Blocco_Username")))
                        .SetAttribute("dettagli_blocco_data", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Dettagli_Blocco_Data")))
                        .SetAttribute("ordine_det", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Ordine_Det")))
                        .SetAttribute("deroga_cod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Deroga_Cod")))
                        .SetAttribute("prezzo_livello", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Prezzo_Livello")))
                        .SetAttribute("rif_esterno", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Rif_Esterno")))
                        .SetAttribute("rif_esterno_2", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Rif_Esterno_2")))
                        .SetAttribute("principiattivipercabb", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("PrincipiAttiviPercAbb")))
                        .SetAttribute("polverulento", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("polverulento")))
                        .SetAttribute("dettaglio_idcod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Dettaglio_IdCod")))
                        .SetAttribute("dettaglio_gencod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Dettaglio_GenCod")))
                        .SetAttribute("dettaglio_specod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Dettaglio_SpeCod")))
                        .SetAttribute("dettaglio_iprocod", Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Dettaglio_IProCod")))

                    End With


                    '#############################################
                    '##########  MOVIMENTI DESTINAZIONI ##########
                    '#############################################

                    'Mi procuro un elenco dei Movimenti dell'Agenda

                    objMovDestinazioni = New AgronicaCoreContabDAL.Mov_Destinazioni_R

                    'Mi procuro il RecordSet richiesto
                    dtMovDestinazioni = objMovDestinazioni.Leggi(CStr(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("PIVA"))),
                                                                 CInt(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Sa_Cod"))),
                                                                 CInt(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Id_Agenda"))),
                                                                 CInt(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Id_Mov"))),
                                                                 CInt(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Id_Mov_Det"))),
                                                                 CInt(Appezza),
                                                                 CInt(Id_Destinazione),
                                                                 CInt(Tipo_Destinazione),
                                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                 "",
                                                                 "",
                                                                 objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If dtMovDestinazioni.Rows.Count > 0 Then

                        'Effettuo un ciclo sulle destinazioni
                        For j = 0 To dtMovDestinazioni.Rows.Count - 1

                            '----- < DESTINAZIONE > -----
                            xmlMovDestinazione = xmlDoc.CreateElement("Movimento_Destinazione")

                            With xmlMovDestinazione
                                .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                                .SetAttribute("piva", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("PIVA")))
                                .SetAttribute("sa_cod", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("Sa_Cod")))
                                .SetAttribute("id_agenda", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("Id_Agenda")))
                                .SetAttribute("id_mov", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("Id_Mov")))
                                .SetAttribute("id_mov_det", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("Id_Mov_Det")))
                                .SetAttribute("appezza", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("Appezza")))
                                .SetAttribute("id_destinazione", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("Id_Destinazione")))
                                .SetAttribute("tipo_destinazione", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("Tipo_Destinazione")))
                                .SetAttribute("qta", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("Qta")))
                                .SetAttribute("qta2", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("Qta2")))
                                .SetAttribute("tipo_scorta", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("Tipo_Scorta")))
                                .SetAttribute("scorta_min", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("Scorta_Min")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("validita_fine")))

                                .SetAttribute("mov_destinazioni_graphickey", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("mov_destinazioni_graphickey")))
                                .SetAttribute("qta_dest1", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("Qta_Dest1")))
                                .SetAttribute("qta_dest2", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("Qta_Dest2")))
                                .SetAttribute("quotadistribuzione", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("QuotaDistribuzione")))
                                .SetAttribute("sa_cod_riferimento", Agro_SQL_Load_ConDefault(dtMovDestinazioni.Rows(j).Item("Sa_Cod_Riferimento"), GetType(Integer)))
                                .SetAttribute("id_destinazione_riferimento", Agro_SQL_Load_ConDefault(dtMovDestinazioni.Rows(j).Item("Id_Destinazione_Riferimento"), GetType(Integer)))
                                .SetAttribute("tipo_destinazione_riferimento", Agro_SQL_Load_ConDefault(dtMovDestinazioni.Rows(j).Item("Tipo_Destinazione_Riferimento"), GetType(Integer)))
                                .SetAttribute("sup_riduzione_bufferzone", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("Sup_Riduzione_BufferZone")))
                                .SetAttribute("perc_riduzione_deriva", Agro_SQL_Load(dtMovDestinazioni.Rows(j).Item("Perc_Riduzione_Deriva")))

                            End With

                            xmlMovimentiDettagli.AppendChild(xmlMovDestinazione)
                            '----- < / DESTINAZIONE > -----

                        Next

                    End If

                    xmlMovDestinazione = Nothing
                    dtMovDestinazioni.Dispose()
                    dtMovDestinazioni = Nothing
                    objMovDestinazioni = Nothing


                    '#########################################################
                    '##########  DETTAGLIO TECNICO DEL MOVIMENTO  ############
                    '#########################################################

                    'Mi procuro un elenco dei dettagli tecnici del movimento

                    objMovDettaglioTecnico = New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R

                    'Mi procuro il RecordSet richiesto
                    dtMovDettaglioTecnico = objMovDettaglioTecnico.Leggi(
                                                        CStr(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("PIVA"))),
                                                        CInt(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Sa_Cod"))),
                                                        CInt(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Id_Agenda"))),
                                                        CInt(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Id_Mov"))),
                                                        CInt(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Id_Mov_Det"))),
                                                        CInt(Id_Reg_Dettaglio),
                                                        "",
                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "",
                                                        "",
                                                        objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If dtMovDettaglioTecnico.Rows.Count > 0 Then

                        'Effettuo un ciclo sui Dettagli Tecnici del Movimento Dettaglio
                        For j = 0 To dtMovDettaglioTecnico.Rows.Count - 1

                            '----- < DETTAGLIO TECNICO > -----
                            xmlMovDettaglioTecnico = xmlDoc.CreateElement("Movimento_Dettaglio_Tecnico_2")

                            With xmlMovDettaglioTecnico
                                .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                                .SetAttribute("id_agenda", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Id_Agenda")))
                                .SetAttribute("id_mov", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Id_Mov")))
                                .SetAttribute("id_mov_det", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Id_Mov_Det")))
                                .SetAttribute("id_reg_dettaglio", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Id_Reg_Dettaglio")))
                                .SetAttribute("qta_ril", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Qta_Ril")))
                                .SetAttribute("data_ril", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Data_Ril")))
                                .SetAttribute("ditta_cod", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Ditta_Cod")))
                                .SetAttribute("dett_cod", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Dett_Cod")))
                                .SetAttribute("id_insetto", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Id_Insetto")))
                                .SetAttribute("ff_classe", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("FF_Classe")))
                                .SetAttribute("dose", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Dose")))
                                .SetAttribute("mg", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Mg")))
                                .SetAttribute("n", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("N")))
                                .SetAttribute("k", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("K")))
                                .SetAttribute("p", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("P")))
                                .SetAttribute("cu", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("cu")))
                                .SetAttribute("parziale", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Parziale")))
                                .SetAttribute("nitrati", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Nitrati")))
                                .SetAttribute("freatimetro", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Freatimetro")))
                                .SetAttribute("piezo1", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Piezo1")))
                                .SetAttribute("piezo2", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Piezo2")))
                                .SetAttribute("piezo3", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Piezo3")))
                                .SetAttribute("piezo4", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Piezo4")))
                                .SetAttribute("sigla_av", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Sigla_Av")))
                                .SetAttribute("trap_num", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Trap_Num")))
                                .SetAttribute("inn1_data", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Inn1_Data")))
                                .SetAttribute("inn2_data", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Inn2_Data")))
                                .SetAttribute("inn3_data", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Inn3_Data")))
                                .SetAttribute("inn4_data", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Inn4_Data")))
                                .SetAttribute("av_cod", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Av_Cod")))
                                .SetAttribute("av_gru", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Av_Gru")))
                                .SetAttribute("lotto", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Lotto")))
                                .SetAttribute("extra_int", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Extra_Int")))
                                .SetAttribute("extra_str", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Extra_Str")))
                                .SetAttribute("extra_date", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Extra_Date")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("validita_fine")))

                                .SetAttribute("soglia_cod", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Soglia_Cod")))
                                .SetAttribute("soglia_quantita", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Soglia_Quantita")))
                                .SetAttribute("soglia_des", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Soglia_Des")))
                                .SetAttribute("efficienza", Agro_SQL_Load(dtMovDettaglioTecnico.Rows(j).Item("Efficienza")))
                            End With

                            xmlMovimentiDettagli.AppendChild(xmlMovDettaglioTecnico)
                            '----- < / DETTAGLIO TECNICO > -----
                        Next

                    End If

                    xmlMovDettaglioTecnico = Nothing

                    dtMovDettaglioTecnico.Dispose()
                    dtMovDettaglioTecnico = Nothing

                    objMovDettaglioTecnico = Nothing

                    '##############################################################
                    '#####################  CAMPIONATURA  RACCOLTO  ###############
                    '##############################################################


                    'Nota Importante: Occorre filtrare le sole operazioni che creano una campionatura.
                    'Questo è necessario poiché, al fine di importare/esportare i dati per il gias standalone,
                    'la classe di lettura del dettaglio-movimento impacchetta anche le campionature.
                    'Senza questo controllo quindi il componente di scrittura tenterebbe di creare un nuovo progressivo per
                    'ogni operazione con cal_cod < 0.

                    'Controllo che sia stata effettuata una campionatura del prodotto aziendale   (Raccolta, Accettazione Beni, Trasformazioni)
                    If Agro_SQL_SaveNum(dtMovimentiDettagli.Rows(i).Item("Cal_Cod")) < 0 AndAlso
                       (Agro_SQL_SaveNum(dtMovimentiDettagli.Rows(i).Item("Lav_Cod")) = LAVCOD_RACCOLTA OrElse
                        Agro_SQL_SaveNum(dtMovimentiDettagli.Rows(i).Item("Lav_Cod")) = 105 OrElse
                        Agro_SQL_SaveNum(dtMovimentiDettagli.Rows(i).Item("Lav_Cod")) = LAVCOD_ACCETTAZIONE_DIVERSI OrElse
                        Agro_SQL_SaveNum(dtMovimentiDettagli.Rows(i).Item("Lav_Cod")) = LAVCOD_PREPARAZIONE) Then

                        objMpCampionature = New AgronicaCoreContabDAL.Materie_Prime_Campionature_R

                        'Mi procuro il RecordSet richiesto
                        dtCampionature = objMpCampionature.Leggi(
                                                 CInt(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Cal_Cod"))),
                                                 "", 0, 0, 0, "", True,
                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                 "",
                                                 "",
                                                 objParametri)

                        'Se ottengo almeno un risultato, creo la struttura XML
                        If dtCampionature.Rows.Count > 0 Then

                            'Effettuo un ciclo sulle Campionature del Movimento Dettaglio
                            For j = 0 To dtCampionature.Rows.Count - 1

                                '----- < CAMPIONATURA > -----
                                xmlCampionatura = xmlDoc.CreateElement("Raccolto_Campionatura")

                                With xmlCampionatura
                                    .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                                    .SetAttribute("progressivo", Agro_SQL_Load(dtCampionature.Rows(j).Item("progressivo")))
                                    .SetAttribute("tipo", Agro_SQL_Load(dtCampionature.Rows(j).Item("tipo")))
                                    .SetAttribute("tipo_cod", Agro_SQL_Load(dtCampionature.Rows(j).Item("tipo_cod")))
                                    .SetAttribute("udm_cod", Agro_SQL_Load(dtCampionature.Rows(j).Item("udm_cod")))
                                    .SetAttribute("val_cod", Agro_SQL_Load(dtCampionature.Rows(j).Item("val_cod")))
                                    .SetAttribute("descrizione", Agro_SQL_Load(dtCampionature.Rows(j).Item("descrizione")))
                                    .SetAttribute("peso_campione", Agro_SQL_Load(dtCampionature.Rows(j).Item("peso_campione")))
                                    .SetAttribute("validita_inizio", Agro_SQL_Load(dtCampionature.Rows(j).Item("validita_inizio")))
                                    .SetAttribute("validita_fine", Agro_SQL_Load(dtCampionature.Rows(j).Item("validita_fine")))
                                    .SetAttribute("progressivo_origine", Agro_SQL_Load(dtCampionature.Rows(j).Item("progressivo_origine")))
                                    .SetAttribute("piva_superuser_origine", Agro_SQL_Load(dtCampionature.Rows(j).Item("piva_superuser_origine")))
                                    .SetAttribute("chkstima", Agro_SQL_Load(dtCampionature.Rows(j).Item("chkstima")))
                                    .SetAttribute("chktara_campionatura", Agro_SQL_Load(dtCampionature.Rows(j).Item("chktara_campionatura")))
                                    .SetAttribute("tara_campionatura", Agro_SQL_Load(dtCampionature.Rows(j).Item("tara_campionatura")))
                                End With

                                xmlMovimentiDettagli.AppendChild(xmlCampionatura)
                                '----- < / CAMPIONATURA > -----
                            Next

                        End If

                        xmlCampionatura = Nothing
                        dtCampionature.Dispose()
                        dtCampionature = Nothing
                        objMpCampionature = Nothing

                    End If



                    '#############################################
                    '##########  RIFERIMENTI DETTAGLI  ###########
                    '#############################################

                    'Mi procuro un elenco dei riferimenti del dettaglio

                    objMovDetRiferimenti = New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R

                    'Mi procuro il RecordSet richiesto
                    dtMovDetRiferimenti = objMovDetRiferimenti.Leggi("",
                                                                     0,
                                                                     CInt(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Id_Agenda"))),
                                                                     CInt(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Id_Mov"))),
                                                                     CInt(Agro_SQL_Load(dtMovimentiDettagli.Rows(i).Item("Id_Mov_Det"))),
                                                                     0,
                                                                     "",
                                                                     "",
                                                                     "",
                                                                     objParametri,
                                                                     leggiRiferimentiInversi:=LeggiRiferimentiInversi)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If dtMovDetRiferimenti.Rows.Count > 0 Then

                        'Effettuo un ciclo sui riferimenti
                        For j = 0 To dtMovDetRiferimenti.Rows.Count - 1

                            '----- < RIFERIMENTO > -----
                            xmlMovDetRiferimenti = xmlDoc.CreateElement("Movimento_Riferimento2")

                            With xmlMovDetRiferimenti
                                .SetAttribute("TipoOperazioneDB", If(ForDelete, "3", "0"))
                                .SetAttribute("piva", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("Piva")))
                                .SetAttribute("sa_cod", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("Sa_Cod")))
                                .SetAttribute("id_agenda", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("Id_Agenda")))
                                .SetAttribute("id_mov", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("Id_Mov")))
                                .SetAttribute("id_mov_det", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("Id_Mov_Det")))
                                .SetAttribute("lav_cod", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("Lav_Cod")))
                                .SetAttribute("cau_mov", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("Cau_Mov")))

                                .SetAttribute("piva_rif", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("Piva_Rif")))
                                .SetAttribute("sa_cod_rif", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("Sa_Cod_Rif")))
                                .SetAttribute("id_agenda_rif", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("Id_Agenda_Rif")))
                                .SetAttribute("id_mov_rif", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("Id_Mov_Rif")))
                                .SetAttribute("id_mov_det_rif", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("Id_Mov_Det_Rif")))
                                .SetAttribute("lav_cod_rif", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("Lav_Cod_Rif")))
                                .SetAttribute("cau_mov_rif", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("Cau_Mov_Rif")))

                                .SetAttribute("qta", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("Qta")))

                                .SetAttribute("validita_inizio", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(dtMovDetRiferimenti.Rows(j).Item("validita_fine")))
                            End With

                            xmlMovimentiDettagli.AppendChild(xmlMovDetRiferimenti)
                            '----- < / RIFERIMENTO > -----


                        Next

                    End If

                    xmlMovDetRiferimenti = Nothing
                    dtMovDetRiferimenti.Dispose()
                    dtMovDetRiferimenti = Nothing
                    objMovDetRiferimenti = Nothing

                    '#################################
                    '#################################
                    '#################################



                    xmlDatiMovimentiDettagli.AppendChild(xmlMovimentiDettagli)
                    '----- < / MOVIMENTO DETTAGLIO > -----


                Next        ' Ciclo DtMovimenti_Dettagli

                xmlDoc.AppendChild(xmlDatiMovimentiDettagli)

                risultatoFunzione = xmlDoc.OuterXml

                '----- < / Documento XML > -----
                xmlDatiMovimentiDettagli = Nothing
                xmlDoc = Nothing

            Else

                'Altrimenti, se non risulta selezionato nessun movimento dettaglio ...
                risultatoFunzione = ""

            End If


            'Elimino gli oggetti che ho creato
            dtMovimentiDettagli.Dispose()
            dtMovimentiDettagli = Nothing
            objMovimentiDettagli = Nothing

        Catch ex As Exception
            risultatoFunzione = ""
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally

            'Pulizia
            'objUtentixImprese = Nothing
            'objImpresexCodici = Nothing
            'objImpresexIndirizzi = Nothing

            If flagConnessioneLocale = True Then
                If Not IsNothing(xConnessione) Then
                    xConnessione.Close()
                    xConnessione.Dispose()
                End If
            Else
                If xConnectionState = ConnectionState.Closed Then
                    xConnessione.Close()
                End If
            End If

        End Try

        Return risultatoFunzione

    End Function


    Public Function LeggiLavorazioni_Da_Principi_Attivi_e_Avversita(ByVal Piva As String,
                                                                    ByVal Sa_Cod As Integer,
                                                                    ByVal strPA As String,
                                                                    ByVal strAvversita As String,
                                                                    ByVal strGruppiAvversita As String,
                                                                    ByVal Appezza As Integer,
                                                                    ByVal Id_Destinazione As Integer,
                                                                    ByVal Validita_Inizio As Date,
                                                                    ByVal Validita_Fine As Date,
                                                                    ByVal xFiltroAggiuntivo As String,
                                                                    ByVal xOrderBy As String,
                                                                    ByRef objParametri_Server As AgronicaCoreParametri,
                                                                    ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                    Optional ByRef Hash_FormulatiPA As Hashtable = Nothing
                                                                   ) As Hashtable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli.LeggiLavorazioni_Da_Principi_Attivi_e_Avversita()"

        Dim messaggioErrore As String = ""

        Dim Hash_IdAgenda As New Hashtable

        Dim ProCod As Integer
        Dim IdAgenda As Integer
        Dim strPrincipi As String
        Dim Principi() As String
        Dim PrincipiDPI() As String

        Dim f, fp, p, p1 As Integer

        Dim Pa() As String

        Dim PrincipioPresente As Boolean
        Dim PaFrCod As Integer
        Dim PaDPI As Integer
        Dim Miscela As Boolean

        If strPA <> "" Then

            strPA = Replace(strPA, "(", "").Trim
            strPA = Replace(strPA, ")", "").Trim
            Pa = Split(strPA, ",")

            'leggo i trattamenti fatti sull'impianto
            Dim ObjOp As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            Dim DtOp As DataTable = ObjOp.Leggi_TrattamentiImpianto_su_Avversita(Piva, Sa_Cod, Appezza, Id_Destinazione,
                                                strAvversita, strGruppiAvversita,
                                                Validita_Inizio, Validita_Fine,
                                                xFiltroAggiuntivo,
                                                "",
                                                objParametri_Server)

            If DtOp IsNot Nothing Then

                For f = 0 To DtOp.Rows.Count - 1

                    IdAgenda = DtOp.Rows(f).Item("id_agenda")
                    ProCod = DtOp.Rows(f).Item("pro_cod")
                    strPrincipi = DtOp.Rows(f).Item("PrincipiAttivi")

                    'se i principi non sono salvati sull'operazione li leggo da ws
                    If strPrincipi = "" Then

                        '(06/09/2017 fede) aggiunta hashtable x tenere memoria dei prodotti 
                        'sia per le diverse operazioni su un impianto sia per gli altri impianti della stessa operazione
                        If Not Hash_FormulatiPA.ContainsKey(ProCod) Then

                            Dim objWs As New AgronicaCoreWebService.AgroWs
                            Dim DtFor As New DataTable
                            DtFor = objWs.ComposizioneFormulatiRecupera(ProCod.ToString, objParametri_Server, objParametri_Utenti, False)
                            If DtFor IsNot Nothing AndAlso DtFor.Rows.Count > 0 Then
                                strPrincipi = DtFor.Rows(0).Item("Elenco_PrincipiAttivi")
                            End If

                            Hash_FormulatiPA.Add(ProCod, strPrincipi)

                        Else

                            strPrincipi = Hash_FormulatiPA(ProCod)

                        End If

                    End If

                    Principi = Split(strPrincipi, "|")

                    For fp = 0 To Principi.Length - 1

                        PrincipioPresente = False
                        PaFrCod = Split(Principi(fp), "§")(0).Trim

                        'PRINCIPI DPI
                        For p = 0 To Pa.Length - 1

                            Miscela = False
                            PrincipiDPI = Split(Pa(p).Trim, "+")

                            If PrincipiDPI IsNot Nothing Then

                                If PrincipiDPI.Length > 1 Then
                                    Miscela = True
                                End If

                                For p1 = 0 To PrincipiDPI.Length - 1
                                    PaDPI = PrincipiDPI(p1).Trim
                                    If PaFrCod = PaDPI Then
                                        PrincipioPresente = True
                                        If Miscela = False Then
                                            If Hash_IdAgenda.ContainsKey(IdAgenda & "|" & PaFrCod) = False Then
                                                Hash_IdAgenda.Add(IdAgenda & "|" & PaFrCod, "")
                                            End If
                                        End If
                                        Exit For
                                    Else
                                        PrincipioPresente = False
                                    End If
                                Next

                            End If

                            If PrincipioPresente = True Then
                                Exit For
                            End If

                        Next

                    Next

                    If PrincipioPresente = True Then
                        If Hash_IdAgenda.ContainsKey(IdAgenda & "|" & PaFrCod) = False Then
                            Hash_IdAgenda.Add(IdAgenda & "|" & PaFrCod, "")
                        End If
                    End If

                Next

            End If

        End If

        Return Hash_IdAgenda

    End Function


    Public Function LeggiLavorazioni_Da_Principi_Attivi(ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal strPA As String,
                                                        ByVal Appezza As Integer,
                                                        ByVal Id_Destinazione As Integer,
                                                        ByVal Validita_Inizio As Date,
                                                        ByVal Validita_Fine As Date,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                                        ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                        Optional ByRef Hash_FormulatiPA As Hashtable = Nothing,
                                                        Optional ByRef Hash_FormulatiPAPesi As Hashtable = Nothing
                                                        ) As DataTable



        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli.LeggiLavorazioni_Da_Principi_Attivi3()"

        Dim messaggioErrore As String = ""

        Dim ProCod As Integer
        Dim IdAgenda As Integer
        Dim strPrincipi As String
        Dim strPrincipiPesi As String
        Dim Principi() As String
        Dim PrincipiP() As String
        Dim Titolo As Decimal
        Dim Peso As Decimal
        Dim Pa_Cod As Integer

        Dim f, fp, p As Integer

        Dim Pa() As String

        Dim PrincipioPresente As Boolean

        Dim DtOpFor As New DataTable


        If strPA <> "" Then

            strPA = Replace(strPA, "(", "").Trim
            strPA = Replace(strPA, ")", "").Trim
            Pa = Split(strPA, ",")

            'leggo i trattamenti fatti sull'impianto
            
            Dim ObjOp As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            Dim DtOp As DataTable = ObjOp.Leggi_TrattamentiImpianto(Piva, Sa_Cod, Appezza, Id_Destinazione,
                                                Validita_Inizio, Validita_Fine,
                                                xFiltroAggiuntivo,
                                                "",
                                                objParametri_Server)

            If DtOp IsNot Nothing Then

                DtOp.Columns.Add(New DataColumn("Pa_Cod", GetType(Integer)))
                DtOp.Columns.Add(New DataColumn("Titolo", GetType(Decimal)))
                DtOp.Columns.Add(New DataColumn("Peso", GetType(Decimal)))

                DtOpFor = DtOp.Clone

                For f = 0 To DtOp.Rows.Count - 1

                    IdAgenda = DtOp.Rows(f).Item("id_agenda")
                    ProCod = DtOp.Rows(f).Item("pro_cod")
                    strPrincipi = DtOp.Rows(f).Item("PrincipiAttivi")
                    strPrincipiPesi = DtOp.Rows(f).Item("PrincipiAttiviPesi")

                    'se i principi non sono salvati sull'operazione li leggo da ws
                    If strPrincipi = "" Then

                        '(06/09/2017 fede) aggiunta hashtable x tenere memoria dei prodotti 
                        'sia per le diverse operazioni su un impianto sia per gli altri impianti della stessa operazione
                        If Not Hash_FormulatiPA.ContainsKey(ProCod) Then

                            Dim objWs As New AgronicaCoreWebService.AgroWs
                            Dim DtFor As New DataTable
                            DtFor = objWs.ComposizioneFormulatiRecupera(ProCod.ToString, objParametri_Server, objParametri_Utenti, False)
                            If DtFor IsNot Nothing AndAlso DtFor.Rows.Count > 0 Then
                                strPrincipi = DtFor.Rows(0).Item("Elenco_PrincipiAttivi")
                                strPrincipiPesi = DtFor.Rows(0).Item("Elenco_PrincipiAttiviPesi")
                            End If

                            Hash_FormulatiPA.Add(ProCod, strPrincipi)

                        Else

                            strPrincipi = Hash_FormulatiPA(ProCod)

                        End If

                    End If

                    'se i principi pesi non sono salvati sull'operazione li leggo da ws
                    If strPrincipiPesi = "" Then

                        '(06/09/2017 fede) aggiunta hashtable x tenere memoria dei prodotti 
                        'sia per le diverse operazioni su un impianto sia per gli altri impianti della stessa operazione
                        If Not Hash_FormulatiPAPesi.ContainsKey(ProCod) Then

                            Dim objWs As New AgronicaCoreWebService.AgroWs
                            Dim DtFor As New DataTable
                            DtFor = objWs.ComposizioneFormulatiRecupera(ProCod.ToString, objParametri_Server, objParametri_Utenti, False)
                            If DtFor IsNot Nothing AndAlso DtFor.Rows.Count > 0 Then
                                strPrincipiPesi = DtFor.Rows(0).Item("Elenco_PrincipiAttiviPesi")
                            End If

                            Hash_FormulatiPAPesi.Add(ProCod, strPrincipiPesi)

                        Else

                            strPrincipiPesi = Hash_FormulatiPAPesi(ProCod)

                        End If

                    End If

                    Principi = Split(strPrincipi, "|")
                    PrincipiP = Split(strPrincipiPesi, "|")

                    For fp = 0 To Principi.Length - 1
                        PrincipioPresente = False
                        For p = 0 To Pa.Length - 1
                            Pa_Cod = Split(Principi(fp), "§")(0)
                            If Pa_Cod = Pa(p) Then
                                If IsNumeric(Split(Principi(fp), "§")(1)) Then
                                    Titolo = CDec(Split(Principi(fp), "§")(1).Replace(".", ","))
                                End If
                                If IsNumeric(Split(PrincipiP(fp), "§")(1)) Then
                                    Peso = CDec(Split(PrincipiP(fp), "§")(1).Replace(".", ","))
                                End If
                                PrincipioPresente = True
                                Exit For
                            End If
                        Next
                        If PrincipioPresente = True Then
                            DtOp.Rows(f).Item("pa_cod") = Pa_Cod
                            DtOp.Rows(f).Item("titolo") = Titolo
                            DtOp.Rows(f).Item("peso") = Peso
                            DtOpFor.ImportRow(DtOp.Rows(f))
                        End If
                    Next
                Next

            End If

        End If

        Return DtOpFor

    End Function

    '===================================================================================================================
    'Restituisce una hashtable con gli id_agenda delle operazioni effettuate sull'impianto con un prodotto
    Public Function LeggiTrattamenti_Da_Formulato(ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal Fr_Cod As Integer,
                                                  ByVal Appezza As Integer,
                                                  ByVal Id_Destinazione As Integer,
                                                  ByVal Validita_Inizio As Date,
                                                  ByVal Validita_Fine As Date,
                                                  ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                                  ByRef objParametri_Utenti As AgronicaCoreParametri
                                                  ) As Hashtable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli.LeggiLavorazioni_Da_Formulato()"

        Dim messaggioErrore As String = ""

        Dim Hash_IdAgenda As New Hashtable

        Dim IdAgenda As Integer

        'leggo i trattamenti fatti sull'impianto
        Dim ObjOp As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim DtOp As DataTable = ObjOp.Leggi_TrattamentiImpianto(Piva, Sa_Cod, Appezza, Id_Destinazione,
                                            Validita_Inizio, Validita_Fine,
                                            " Movimenti_dettagli.Pro_Cod=" & Fr_Cod.ToString & xFiltroAggiuntivo,
                                            "",
                                            objParametri_Server)

        If DtOp IsNot Nothing Then
            For f As Integer = 0 To DtOp.Rows.Count - 1
                IdAgenda = DtOp.Rows(f).Item("id_agenda")
                If Hash_IdAgenda.ContainsKey(IdAgenda) = False Then
                    Hash_IdAgenda.Add(IdAgenda, "")
                End If
            Next
        End If

        Return Hash_IdAgenda

    End Function


    Public Function LeggiDistribuzioneInsetti_Da_Insetti(ByVal Piva As String,
                                                         ByVal Sa_Cod As Integer,
                                                         ByVal strIns_Cod As String,
                                                         ByVal strAv_Cod As String,
                                                         ByVal Appezza As Integer,
                                                         ByVal Id_Destinazione As Integer,
                                                         ByVal Validita_Inizio As Date,
                                                         ByVal Validita_Fine As Date,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                                         ByRef objParametri_Utenti As AgronicaCoreParametri
                                                         ) As Hashtable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli.LeggiDistribuzioneInsetti_Da_Insetti()"

        Dim messaggioErrore As String = ""

        Dim Hash_IdAgenda As New Hashtable

        Dim IdAgenda As Integer

        'leggo i trattamenti fatti sull'impianto
        Dim ObjOp As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim DtOp As DataTable = ObjOp.Leggi_DistribuzioneInsettiImpianto(Piva, Sa_Cod, Appezza, Id_Destinazione,
                                           strIns_Cod, strAv_Cod,
                                           Validita_Inizio, Validita_Fine,
                                            "",
                                            "",
                                            objParametri_Server)

        If DtOp IsNot Nothing Then
            For f As Integer = 0 To DtOp.Rows.Count - 1
                IdAgenda = DtOp.Rows(f).Item("id_agenda")
                If Hash_IdAgenda.ContainsKey(IdAgenda) = False Then
                    Hash_IdAgenda.Add(IdAgenda, "")
                End If
            Next
        End If

        Return Hash_IdAgenda

    End Function

    '############################################
    'passato in formato JSon
    Public Function MovimentiDettagli_Leggi_FF(ByVal Piva As String,
                                               ByVal Sa_Cod As Integer,
                                               ByVal Cod_Contatto As String,
                                               ByVal Id_Agenda As Integer,
                                               ByVal Id_Mov As Integer,
                                               ByVal Id_Mov_Det As Integer,
                                               ByVal Elem_Cod As Integer,
                                               ByVal Pro_Cod As Integer,
                                               ByVal Mat_Cod As Integer,
                                               ByVal Cod_Progetto As Integer,
                                               ByVal Fase_Cod As Integer,
                                               ByVal Lotto As String,
                                               ByVal Cal_Cod As Integer,
                                               ByVal Udm_Cod As Integer,
                                               ByVal Lav_Cod As Integer(),
                                               ByVal Cau_Mov As String(),
                                               ByVal Cau_Agg As Integer,
                                               ByVal Flag_Jolly_Int As Short,
                                               ByVal xSelectAggiuntiva As String,
                                               ByVal xJoinAggiuntiva As String,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByVal fromOutToIn As Boolean,
                                               ByRef objParametri As AgronicaCoreParametri,
                                               Optional ByRef dt As DataTable = Nothing
                                               ) As String

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Movimenti_Dettagli.MovimentiDettagli_Leggi_JSon()"

        Dim messaggioErrore As String = ""

        Dim mdR As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

        Dim DTParamQual As New DataTable
        Dim objConfigDettagli As AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R = New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
        DTParamQual = objConfigDettagli.Leggi(Piva, 0, False, "Tipo = 1", "", objParametri)

        dt = mdR.MovimentiDettagli_Leggi_FF(Piva,
                                            Sa_Cod,
                                            Cod_Contatto,
                                            Id_Agenda,
                                            Id_Mov,
                                            Id_Mov_Det,
                                            Elem_Cod,
                                            Pro_Cod,
                                            Mat_Cod,
                                            Cod_Progetto,
                                            Fase_Cod,
                                            Lotto,
                                            Cal_Cod,
                                            Udm_Cod,
                                            Lav_Cod,
                                            Cau_Mov,
                                            Cau_Agg,
                                            Flag_Jolly_Int,
                                            xSelectAggiuntiva,
                                            xJoinAggiuntiva,
                                            xFiltroAggiuntivo,
                                            xOrderBy,
                                            objParametri
                                            )

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("Id_Mov_Det", "Id_Mov_Det", "number") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Cal_Cod", "Cal_Cod", "number") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Mat_Cod", "Mat_Cod", "number") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Qta_Extra_Totale", "Qta_Extra_Totale", "number") With {
            ._hidden = True
        }
        l.Add(c)

        c = New ColonneNome("Rag_Soc", "Ragione sociale", "string") With {
            ._Editabile = False,
            ._Filtrabile = True,
            ._FiltrabileConCheck = True,
            ._Display = True
        }
        l.Add(c)

        c = New ColonneNome("MCauAgg_Doc_Numero_Sin", "Prefisso Doc.", "string")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = True
        l.Add(c)

        If fromOutToIn Then
            c = New ColonneNome("MCauAgg_Doc_Numero", "Nr. Doc.", "number")
        Else
            c = New ColonneNome("MCauAgg_Doc_Numero", "Nr. Doc.", "string")
        End If
        c._Editabile = False
        c._Filtrabile = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("MCauAgg_Doc_Numero_Des", "Suffisso Doc.", "string")
        c._Editabile = False
        c._Filtrabile = True
        c._Display = True
        l.Add(c)

        'If fromOutToIn Then
        '    c = New ColonneNome("Extra_Str", "Riga", "string")
        'Else
        '    c = New ColonneNome("Extra_Str1", "Riga", "string")
        'End If
        'c._Editabile = False
        'c._Filtrabile = True
        'c._Display = True
        'l.Add(c)

        c = New ColonneNome("Data_Movimento", "Data movimento", "date") With {
            ._Editabile = False,
            ._Filtrabile = True,
            ._Display = True,
            ._formatNr = "{0:dd/MM/yyyy}"
        }
        l.Add(c)

        c = New ColonneNome("Mat_Des", "Prodotto", "string") With {
            ._Editabile = False,
            ._Filtrabile = True,
            ._Display = True
        }
        l.Add(c)

        c = New ColonneNome("Lotto", "Lotto", "string") With {
            ._Editabile = False,
            ._Filtrabile = True,
            ._Display = True
        }
        l.Add(c)

        For Each paramQual In DTParamQual.Rows
            If ({"certificazioni", "qualità", "calibro"}).Contains(paramQual("Tabella_Key")) Then
                Dim descr As String = paramQual("Tabella_Key").ToString.Substring(0, 1).ToUpper() + paramQual("Tabella_Key").ToString.Substring(1)
                c = New ColonneNome(paramQual("Tabella_Key") & "_Sigla", descr, "string") With {
                ._Editabile = False,
                ._Filtrabile = True,
                ._Display = True
            }
                l.Add(c)
            End If
        Next

        c = New ColonneNome("Qta_Extra_Totale", "Kg", "number") With {
            ._Editabile = False,
            ._Filtrabile = True,
            ._Display = True,
            ._formatNr = "n5"
        }
        l.Add(c)

        Dim js As New JSON_DataTable With {
            .Editabile_Deafault = False
        }
        Dim risposta = js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto)

        Return risposta

    End Function

    Public Function BIZ_RaccolteCollegabiliConferimento(ByVal _piva As String, ByVal dataMovimento As Date, ByVal idMovDetConf As Integer, ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreContabBIZ.Movimenti_Dettagli_R.BIZ_RaccolteCollegabiliConferimento()"

        Dim dt As Object

        Try
            Dim fineValImp = dataMovimento
            'TODO Sostituire il calcolo fisso leggendo la relativa impostazione
            Dim inizioValImp = dataMovimento.AddDays(-31)

            Dim movDetDAL As New AgronicaCoreContabDAL.Movimenti_Dettagli_R()
            Dim dtDaQuery As DataTable = movDetDAL.RaccolteCollegabiliConferimento(_piva, inizioValImp, fineValImp, idMovDetConf, objParametri)

            Dim groupDestRaccolte =
                From dest In dtDaQuery.AsEnumerable()
                Group By idAgenda = dest.Field(Of Integer)("Id_Agenda"),
                     idMov = dest.Field(Of Integer)("Id_Mov"),
                     idMovDet = dest.Field(Of Integer)("Id_Mov_Det"),
                     udmCod = dest.Field(Of Integer)("Udm_Cod"),
                     udmSim = dest.Field(Of String)("Udm_Sim")
                    Into gruppo = Group
                Select New With {
                    .Piva = gruppo.String_Join(Function(elem) elem.Field(Of String)("Piva")),
                    .Id_Agenda = idAgenda,
                    .Id_Mov = idMov,
                    .Id_Mov_Det = idMovDet,
                    .Udm_Cod = udmCod,
                    .Udm_Sim = udmSim,
                    .Chiave = gruppo.String_Join(Function(elem) elem.Field(Of String)("Chiave")).Replace("<br/>", ""),
                    .Sa_Nome = gruppo.String_Join(Function(elem) elem.Field(Of String)("Sa_Nome")),
                    .Sa_Cod = gruppo.String_Join(Function(elem) elem.Field(Of Integer)("Sa_Cod")),
                    .Campagna_Sa_Cod = gruppo.String_Join(Function(elem) elem.Field(Of Integer)("Campagna_Sa_Cod")),
                    .Fabbricato_Des = gruppo.String_Join(Function(elem) elem.Field(Of String)("Fabbricato_Des")),
                    .Destinazione_Des = gruppo.String_Join(Function(elem)
                                                               Return "<strong>Centro:</strong> " & elem.Field(Of String)("Sa_Nome") &
                                                                      " <strong>Mag:</strong> " & elem.Field(Of String)("Fabbricato_Des")
                                                           End Function),
                    .Des_Lib = gruppo.String_Join(Function(elem) elem.Field(Of String)("Des_Lib")),
                    .Conf_Id_Mov_Det = gruppo.String_Join(Function(elem) elem.Field(Of Integer)("Conf_Id_Mov_Det")),
                    .Impianto_Cul_Cod = gruppo.String_Join(Function(elem) elem.Field(Of String)("Impianto_Cul_Cod")),
                    .Impianto_Veg_Cod = gruppo.String_Join(Function(elem) elem.Field(Of String)("Impianto_Veg_Cod")),
                    .Impianto_Veg_Des = gruppo.String_Join(Function(elem) elem.Field(Of String)("Impianto_Veg_Des")),
                    .Campagna_Des = gruppo.String_Join(Function(elem) elem.Field(Of String)("Campagna_Des")).Replace("&lt;", "<").Replace("&gt;", ">"),
                    .Mat_Cod = gruppo.String_Join(Function(elem) elem.Field(Of Integer)("Mat_Cod")),
                    .Mat_Des = gruppo.String_Join(Function(elem) elem.Field(Of String)("Mat_Des")),
                    .Cod_Articolo = gruppo.String_Join(Function(elem) elem.Field(Of String)("Cod_Articolo")),
                    .Data_Movimento = gruppo.String_Join(Function(elem) elem.Field(Of Date)("Data_Movimento").ToString("g")),
                    .Destinazioni_Qta = gruppo.Sum(Function(elem) elem.Field(Of Double)("Qta_Dest")),
                    .Tipo_Associazione = gruppo.String_Join(Function(elem) elem.Field(Of Short)("Tipo_Associazione"))
                }

            dt = groupDestRaccolte.ToList()

        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Movimenti_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider
    
    '============================================================================
    Public Function Movimento_Dettaglio_Scrivi(ByVal DatiMovimenti_Dettagli As String,
                                               ByVal Id_Agenda As Integer,
                                               ByVal Id_Mov As Integer,
                                               ByVal Lav_Cod As Integer,
                                               ByVal Progressivo_Mirror As Integer,
                                               ByVal Flag_Mirror As Integer,
                                               ByRef objParametri As AgronicaCoreParametri,
                                               Optional ByVal Data_creazione As Date = #2/1/1900#,
                                               Optional ByVal Data_modifica As Date = #2/1/1900#,
                                               Optional ByVal username_creazione As String = "",
                                               Optional ByVal username_modifica As String = "",
                                               Optional ByRef CodiciRimappati As String = "",
                                               Optional ByVal G2G As Boolean = False,
                                               Optional ByVal DataMovimento As Date = AGRODATAINIZIO
                                               ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabBIZ.Movimenti_Dettagli_W.Movimento_Dettaglio_Scrivi()"

        Dim dummy As Boolean

        Dim objSequenze As New Agro_Sequenze
        Dim objMovimentiDettagli As AgronicaCoreContabDAL.Movimenti_Dettagli_W
        Dim objMovDetRiferimenti As AgronicaCoreContabDAL.Mov_Det_Riferimenti_W
        Dim objMovDestinazioni As AgronicaCoreContabDAL.Mov_Destinazioni_W
        Dim objMovDettaglioTecnico As AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W
        Dim objParcoMacchine As AgronicaCoreContabDAL.Parco_Macchine_W
        Dim objGiacenze As AgronicaCoreContabBIZ.Giacenze_W
        Dim objRicxCod As AgronicaCoreContabDAL.RicxConti_W
        Dim objRicxCodPat As AgronicaCoreContabDAL.RicxConti_Patrimonio_W
        Dim objContabHlp As AgronicaCoreContabHLP.Contabilita
        Dim objProdottiCosti As AgronicaCoreContabBIZ.Prodotti_Costi_W
        Dim objMpCampionature As AgronicaCoreContabDAL.Materie_Prime_Campion_W
        Dim objMovimentoDettaglioMirrorW As AgronicaCoreContabDAL.Mov_Dettagli_Mirror_W
        Dim objMovDestinazioneMirrorW As AgronicaCoreContabDAL.Mov_Destinaz_Mirror_W
        Dim objMovDettRiferimentoMirrorW As AgronicaCoreContabDAL.Mov_DettRiferim_Mirror_W
        Dim objMovDettTecnicoMirrorW As AgronicaCoreContabDAL.Mov_DettTecnico_Mirror_W
        Dim objMatPrimeCampionatureMirrorW As AgronicaCoreContabDAL.MatPrime_Camp_Mirror_W
        Dim objParcoMacchineCodici As AgronicaCoreContabDAL.Parco_Macchine_Codici_W

        Dim xmlDoc As XmlDocument

        Dim dummyProdottiCosti As Object

        Dim Cod_Movimento_Dettaglio As Integer
        Dim Cod_Id_Reg_Dettaglio As Integer
        Dim Mat_Cod As Integer
        Dim Cod_Progetto As Integer
        Dim Progressivo As Integer
        Dim Lotto As String

        'Variabili per la gestione della giacenza

        Dim mId_Destinazione As Integer
        Dim mImponibile As Decimal
        Dim mIva As Decimal

        Dim modalitaSconto As Integer
        Dim importoOmaggio As Decimal
        Dim ivaIndetraibile As Decimal

        Dim mBasecode As Integer
        Dim mTopcode As Integer

        Dim xDatiMovimentiDettagli As XmlNodeList
        Dim xDatiMovimentoDettaglio As XmlElement
        Dim xMovimentiDettagli As XmlNodeList
        Dim xMovimentoDettaglio As XmlElement
        Dim xMovimentiDestinazioni As XmlNodeList
        Dim xMovimentoDestinazione As XmlElement
        Dim xMovDetRiferimenti As XmlNodeList
        Dim xMovDetRiferimento As XmlElement
        Dim xMovDettagliTecnici As XmlNodeList
        Dim xMovDettaglioTecnico As XmlElement
        Dim xRaccoltoCampionature As XmlNodeList
        Dim xRaccoltoCampionatura As XmlElement
        Dim xParcoMacchine As XmlNodeList
        Dim xParcoMacchina As XmlElement
        Dim xProdottiCosti As XmlNodeList
        Dim xProdottoCosti As XmlElement
        Dim xZooAnagrafiche As XmlNodeList
        Dim xCodici As XmlNodeList
        Dim xCodice As XmlElement

        Dim i_DatiMovimenti_Dettagli As Integer
        Dim i_Movimenti_Dettagli As Integer
        Dim i_Movimento_Destinazione As Integer
        Dim i_Mov_Det_Riferimento As Integer
        Dim i_Mov_Dettaglio_Tecnico As Integer
        Dim i_Raccolto_Campionature As Integer
        Dim i_Parco_Macchina As Integer
        Dim i_Codice As Integer

        Dim datiProdottiCosti As String

        Dim OpeDB_Movimento_Dettaglio As String
        Dim OpeDB_Movimento_Destinazione As String
        Dim OpeDB_Mov_Det_Riferimento As String
        Dim OpeDB_Mov_Dettaglio_Tecnico As String
        Dim OpeDB_Parco_Macchina As String
        Dim OpeDB_Campionatura As String
        Dim OpeDB_Codice As String


        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------

        Dim OUTPUT_ID_Agenda As Integer = 0

        Dim extraDate As Date

        Dim vCodiciRimappati As String() = CodiciRimappati.Split("|")

        '  Marco Grilli, 12/09/2014 17:24:53: Variabili per gestire il fatto che un movimento può avere 
        '                                    la data, mentre il successivo no
        Dim dataCreazioneOld, dataModificaOld As Date
        Dim usernameCreazioneOld, usernameModificaOld As String

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Date.Now
        End If
        dataCreazioneOld = Data_creazione

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Date.Now
        End If
        dataModificaOld = Data_modifica

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If
        usernameCreazioneOld = username_creazione

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If
        usernameModificaOld = username_modifica


        Try

            '------------------------------

            'Se la connessione è chiusa la apro
            If objParametri.objConnessione Is Nothing Then
                'Richiedo una connessione
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
                flagConnessioneLocale = True
                objParametri.objTransazione = Nothing
            End If

            If objParametri.objTransazione Is Nothing Then
                'Inizializzo la transazione
                objParametri.objTransazione = objParametri.objConnessione.BeginTransaction
                flagTransazioneLocale = True
            End If

            '------------------------------

            xmlDoc = New XmlDocument
            'xmlDoc.async = False
            xmlDoc.LoadXml(DatiMovimenti_Dettagli)

            '------------------------------
            '------------------------------
            '------------------------------



            xDatiMovimentiDettagli = xmlDoc.GetElementsByTagName("DatiMovimenti_Dettagli")

            i_DatiMovimenti_Dettagli = 0

            Do While i_DatiMovimenti_Dettagli < xDatiMovimentiDettagli.Count

                'Prelevo l'i-esimo blocco di DatiMovimenti_Dettagli (in realtà ne esiste uno solo)
                xDatiMovimentoDettaglio = xDatiMovimentiDettagli.Item(i_DatiMovimenti_Dettagli)


                '------------------------------

                xMovimentiDettagli = xDatiMovimentoDettaglio.GetElementsByTagName("Movimento_Dettaglio")

                i_Movimenti_Dettagli = 0

                Do While i_Movimenti_Dettagli < xMovimentiDettagli.Count

                    'Prelevo l' i-esimo Movimento_Dettaglio
                    xMovimentoDettaglio = xMovimentiDettagli.Item(i_Movimenti_Dettagli)

                    'Inizializzo preventivamente il Mat_Cod
                    Mat_Cod = CInt(xMovimentoDettaglio.GetAttribute("mat_cod"))

                    '  Marco Grilli, 12/09/2014 17:29:40: prelevo, se presenti, i dati di creazione/modifica
                    '                                     altrimenti assegno quelli passati come parametro
                    If xMovimentoDettaglio.GetAttribute("data_creazione") <> "" AndAlso IsDate(xMovimentoDettaglio.GetAttribute("data_creazione")) Then
                        Data_creazione = CDate(xMovimentoDettaglio.GetAttribute("data_creazione"))
                    Else
                        Data_creazione = dataCreazioneOld
                    End If

                    If xMovimentoDettaglio.GetAttribute("data_modifica") <> "" AndAlso IsDate(xMovimentoDettaglio.GetAttribute("data_modifica")) Then
                        Data_modifica = CDate(xMovimentoDettaglio.GetAttribute("data_modifica"))
                    Else
                        Data_modifica = dataModificaOld
                    End If

                    username_creazione = If(xMovimentoDettaglio.GetAttribute("username_creazione") <> "",
                                            xMovimentoDettaglio.GetAttribute("username_creazione"),
                                            usernameCreazioneOld)

                    username_modifica = If(xMovimentoDettaglio.GetAttribute("username_modifica") <> "",
                                           xMovimentoDettaglio.GetAttribute("username_modifica"),
                                           usernameModificaOld)


                    '##################################################
                    '##############  PARCO MACCHINE  ##################
                    '##################################################

                    'Prelevo l'elenco delle macchine/attrezzature
                    xParcoMacchine = xMovimentoDettaglio.GetElementsByTagName("ParcoMacchina")

                    i_Parco_Macchina = 0

                    Do While i_Parco_Macchina < xParcoMacchine.Count

                        'Prelevo l'i-esima macchina/attrezzatura
                        xParcoMacchina = xParcoMacchine.Item(i_Parco_Macchina)

                        'Prelevo gli attributi della macchina/attrezzatura selezionata
                        OpeDB_Parco_Macchina = xParcoMacchina.GetAttribute("TipoOperazioneDB")

                        'Inizializzo Preventivamente il Codice della Macchina/Attrezzatura
                        Mat_Cod = CInt(xParcoMacchina.GetAttribute("mac_cod"))

                        objParcoMacchine = New AgronicaCoreContabDAL.Parco_Macchine_W


                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Parco_Macchina

                            Case "0"    'LEGGI -------------------------------------------------------
                                '
                            Case "1"    'SALVA -------------------------------------------------------
                                '
                                'Richiedo un nuovo codice della macchina/attrezzatura

                                If Mat_Cod <= 0 Then

                                    Mat_Cod = objSequenze.NuovoId_Tabella("Parco_Macchine",
                                                                          CInt(xParcoMacchina.GetAttribute("basecode")),
                                                                          CInt(xParcoMacchina.GetAttribute("topcode")),
                                                                          objParametri)

                                Else

                                    'Esportazione in Locale

                                End If


                                ''TEST
                                'Dim Flag1 As Boolean = xParcoMacchina.HasAttribute("cod_contatto")
                                'Dim Flag2 As Boolean = IsDBNull(xParcoMacchina.HasAttribute("cod_contatto"))
                                'Dim Flag7 As Boolean = IsNothing(xParcoMacchina.HasAttribute("cod_contatto"))


                                'Dim Flag3 As Boolean = xParcoMacchina.HasAttribute("note")
                                'Dim Flag4 As Boolean = IsDBNull(xParcoMacchina.GetAttribute("note"))
                                'Dim Flag8 As Boolean = IsNothing(xParcoMacchina.GetAttribute("note"))

                                'Dim flag5 As Boolean = xParcoMacchina.HasAttribute("nonesiste")
                                'Dim Flag6 As Boolean = IsDBNull(xParcoMacchina.GetAttribute("nonesiste"))
                                'Dim Flag9 As Boolean = IsNothing(xParcoMacchina.GetAttribute("nonesiste"))


                                'Salvo la macchina/attrezzatura
                                dummy = objParcoMacchine.Scrivi(CStr(xParcoMacchina.GetAttribute("piva")),
                                                                CInt(xParcoMacchina.GetAttribute("sa_cod")),
                                                                CInt(Mat_Cod),
                                                                Agro_XML_GetString(xParcoMacchina, "cod_contatto", ""),
                                                                CStr(xParcoMacchina.GetAttribute("class_code")),
                                                                CInt(xParcoMacchina.GetAttribute("tipo")),
                                                                CStr(xParcoMacchina.GetAttribute("mac_des")),
                                                                CDbl(xParcoMacchina.GetAttribute("costo_acquisto")),
                                                                CStr(xParcoMacchina.GetAttribute("targa")),
                                                                CStr(xParcoMacchina.GetAttribute("telaio")),
                                                                CInt(xParcoMacchina.GetAttribute("ditta_cod")),
                                                                CStr(xParcoMacchina.GetAttribute("modello")),
                                                                CStr(xParcoMacchina.GetAttribute("potenza")),
                                                                CDbl(xParcoMacchina.GetAttribute("ammortamento")),
                                                                CDate(xParcoMacchina.GetAttribute("data_immatricolazione")),
                                                                CDate(xParcoMacchina.GetAttribute("ultima_manutenzione")),
                                                                CDate(xParcoMacchina.GetAttribute("ultima_revisione")),
                                                                CStr(xParcoMacchina.GetAttribute("stato_utilizzo")),
                                                                Agro_XML_GetString(xParcoMacchina, "note", ""),
                                                                CStr(xParcoMacchina.GetAttribute("n_immatricolazione")),
                                                                CStr(xParcoMacchina.GetAttribute("n_immatricolazione_rimorchio")),
                                                                CStr(xParcoMacchina.GetAttribute("n_autorizzazione_trasporto")),
                                                                CDate(xParcoMacchina.GetAttribute("data_rilascio_autorizzazione")),
                                                                CDbl(xParcoMacchina.GetAttribute("peso")),
                                                                Agro_XML_GetInteger(xParcoMacchina, "portata_max", 0),
                                                                Agro_XML_GetInteger(xParcoMacchina, "chkdefault", 0),
                                                                CInt(xParcoMacchina.GetAttribute("alimentazione_cod")),
                                                                CInt(xParcoMacchina.GetAttribute("potenza_udm_cod")),
                                                                CInt(xParcoMacchina.GetAttribute("mac_cod_origine")),
                                                                CStr(xParcoMacchina.GetAttribute("piva_superuser_origine")),
                                                                Agro_XML_GetString(xParcoMacchina, "cuaa_proprietario", ""),
                                                                Agro_XML_GetString(xParcoMacchina, "denominazione_proprietario", ""),
                                                                Agro_XML_GetInteger(xParcoMacchina, "tipo_targa_cod", 0),
                                                                Agro_XML_GetInteger(xParcoMacchina, "tipo_trazione", 0),
                                                                Agro_XML_GetString(xParcoMacchina, "n_omologazione", ""),
                                                                Agro_XML_GetInteger(xParcoMacchina, "ditta_cod_motore", 0),
                                                                Agro_XML_GetString(xParcoMacchina, "tipo_motore", ""),
                                                                Agro_XML_GetString(xParcoMacchina, "matricola_motore", ""),
                                                                Agro_XML_GetDate(xParcoMacchina, "data_reimmatricolazione", AGRODATAINIZIO),
                                                                Agro_XML_GetDate(xParcoMacchina, "data_carico", AGRODATAINIZIO),
                                                                Agro_XML_GetDate(xParcoMacchina, "data_scarico", AGRODATAFINE),
                                                                Agro_XML_GetInteger(xParcoMacchina, "titolopossesso", 0),
                                                                Agro_XML_GetString(xParcoMacchina, "flag_attrezzatura_macchina", ""),
                                                                CDate(xParcoMacchina.GetAttribute("validita_inizio")),
                                                                CDate(xParcoMacchina.GetAttribute("validita_fine")),
                                                                objParametri,
                                                                Agro_XML_GetDecimal(xParcoMacchina, "taratura_ugello", 0))



                                ' TipoOperazioneDB="1"   class_code="12.03" mac_des="" costo_acquisto="0" targa="" telaio="" ditta_cod="0" modello="" potenza="" ammortamento="0" 
                                'ammortizzato="0" data_immatricolazione="01/01/1900" ultima_manutenzione="01/01/1900" ultima_revisione="01/01/1900" stato_utilizzo="" validita_inizio="01/01/1900" validita_fine="31/12/2100" basecode="84541440" topcode="84672511" note="" tipo="0" n_immatricolazione="" n_immatricolazione_rimorchio="" n_autorizzazione_trasporto="" data_rilascio_autorizzazione="01/01/1900" peso="0" titolopossesso="0" alimentazione_cod="0" potenza_udm_cod="0" />

                                '

                            Case "2"    'MODIFICA -------------------------------------------------------

                                objParcoMacchine.Modifica(CStr(xParcoMacchina.GetAttribute("piva")),
                                                CInt(xParcoMacchina.GetAttribute("sa_cod")),
                                                CInt(Mat_Cod),
                                                Agro_XML_GetString(xParcoMacchina, "cod_contatto", ""),
                                                CStr(xParcoMacchina.GetAttribute("class_code")),
                                                CInt(xParcoMacchina.GetAttribute("tipo")),
                                                CStr(xParcoMacchina.GetAttribute("mac_des")),
                                                CDbl(xParcoMacchina.GetAttribute("costo_acquisto")),
                                                CStr(xParcoMacchina.GetAttribute("targa")),
                                                CStr(xParcoMacchina.GetAttribute("telaio")),
                                                CInt(xParcoMacchina.GetAttribute("ditta_cod")),
                                                CStr(xParcoMacchina.GetAttribute("modello")),
                                                CStr(xParcoMacchina.GetAttribute("potenza")),
                                                CDbl(xParcoMacchina.GetAttribute("ammortamento")),
                                                CDate(xParcoMacchina.GetAttribute("data_immatricolazione")),
                                                CDate(xParcoMacchina.GetAttribute("ultima_manutenzione")),
                                                CDate(xParcoMacchina.GetAttribute("ultima_revisione")),
                                                CStr(xParcoMacchina.GetAttribute("stato_utilizzo")),
                                                Agro_XML_GetString(xParcoMacchina, "note", ""),
                                                CStr(xParcoMacchina.GetAttribute("n_immatricolazione")),
                                                CStr(xParcoMacchina.GetAttribute("n_immatricolazione_rimorchio")),
                                                CStr(xParcoMacchina.GetAttribute("n_autorizzazione_trasporto")),
                                                CDate(xParcoMacchina.GetAttribute("data_rilascio_autorizzazione")),
                                                CDbl(xParcoMacchina.GetAttribute("peso")),
                                                Agro_XML_GetInteger(xParcoMacchina, "portata_max", 0),
                                                Agro_XML_GetInteger(xParcoMacchina, "chkdefault", 0),
                                                CInt(xParcoMacchina.GetAttribute("alimentazione_cod")),
                                                CInt(xParcoMacchina.GetAttribute("potenza_udm_cod")),
                                                Agro_XML_GetString(xParcoMacchina, "cuaa_proprietario", ""),
                                                Agro_XML_GetString(xParcoMacchina, "denominazione_proprietario", ""),
                                                Agro_XML_GetInteger(xParcoMacchina, "tipo_targa_cod", 0),
                                                Agro_XML_GetInteger(xParcoMacchina, "tipo_trazione", 0),
                                                Agro_XML_GetString(xParcoMacchina, "n_omologazione", ""),
                                                Agro_XML_GetInteger(xParcoMacchina, "ditta_cod_motore", 0),
                                                Agro_XML_GetString(xParcoMacchina, "tipo_motore", ""),
                                                Agro_XML_GetString(xParcoMacchina, "matricola_motore", ""),
                                                Agro_XML_GetDate(xParcoMacchina, "data_reimmatricolazione", AGRODATAINIZIO),
                                                Agro_XML_GetDate(xParcoMacchina, "data_carico", AGRODATAINIZIO),
                                                Agro_XML_GetDate(xParcoMacchina, "data_scarico", AGRODATAFINE),
                                                Agro_XML_GetInteger(xParcoMacchina, "titolopossesso", 0),
                                                Agro_XML_GetString(xParcoMacchina, "flag_attrezzatura_macchina", ""),
                                                CDate(xParcoMacchina.GetAttribute("validita_inizio")),
                                                CDate(xParcoMacchina.GetAttribute("validita_fine")),
                                                "", objParametri,
                                                Agro_XML_GetDecimal(xParcoMacchina, "taratura_ugello", 0))


                            Case "3"    'ELIMINA -------------------------------------------------------
                                '

                                objParcoMacchine.Cancella(CStr(xParcoMacchina.GetAttribute("piva")),
                                                          CInt(Mat_Cod),
                                                          Agro_XML_GetString(xParcoMacchina, "cod_contatto", ""),
                                                          "",
                                                          objParametri)


                                '------------------------------------------
                                '       <<<< INTEGRAZIONE >>>>>
                                '------------------------------------------
                                '
                                'Cancellazione del Movimento di Agenda relativo la Macchina/Attrezzatura
                                '
                                '

                                Dim objDeleteGiacenzaAd As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                                Dim objDeleteGiacenzaR As New AgronicaCoreContabBIZ.Agenda_R
                                Dim objDeleteGiacenzaW As New AgronicaCoreContabBIZ.Agenda_W
                                Dim dtGiacenza As DataTable
                                Dim datiGiacenza As String
                                Dim drGiacenza() As DataRow

                                dtGiacenza = objDeleteGiacenzaAd.Leggi(CStr(xParcoMacchina.GetAttribute("piva")),
                                                                       0,
                                                                       0,
                                                                       0,
                                                                       0,
                                                                       1,
                                                                       0,
                                                                       CInt(Mat_Cod),
                                                                       "",
                                                                       0,
                                                                       0,
                                                                       0,
                                                                       0,
                                                                       0,
                                                                       0,
                                                                       enumSelezioneVariabile.Selezione_JoinCompleta,
                                                                       "",
                                                                       "",
                                                                       objParametri)

                                If dtGiacenza.Rows.Count > 0 Then

                                    drGiacenza = dtGiacenza.Select("Lav_Cod = 1008")

                                    If drGiacenza.Length > 0 Then

                                        datiGiacenza = objDeleteGiacenzaR.Agenda_Leggi(drGiacenza(0).Item("Piva"),
                                                                                       0,
                                                                                       drGiacenza(0).Item("Id_Agenda"),
                                                                                       1008,
                                                                                       True,
                                                                                       objParametri)

                                        objDeleteGiacenzaW.Agenda_Scrivi(datiGiacenza,
                                                                         OUTPUT_ID_Agenda,
                                                                         Flag_Mirror,
                                                                         5,
                                                                         0,
                                                                         "",
                                                                         objParametri)

                                    End If

                                End If

                                objDeleteGiacenzaAd = Nothing
                                objDeleteGiacenzaR = Nothing
                                objDeleteGiacenzaW = Nothing
                                dtGiacenza.Dispose()
                                dtGiacenza = Nothing

                        End Select




                        '#####################################
                        '##########  PRODOTTI COSTI   ########
                        '#####################################


                        'Prelevo l'elenco dei movimenti
                        xProdottiCosti = xParcoMacchina.GetElementsByTagName("DatiProdotti_Costi")

                        If xProdottiCosti.Count > 0 Then

                            xProdottoCosti = xProdottiCosti.Item(0)

                            datiProdottiCosti = xProdottoCosti.OuterXml

                            objProdottiCosti = New AgronicaCoreContabBIZ.Prodotti_Costi_W

                            dummyProdottiCosti = objProdottiCosti.Prodotti_Costi_Scrivi(CStr(datiProdottiCosti),
                                                                                        Mat_Cod,
                                                                                        objParametri)

                            objProdottiCosti = Nothing

                        End If




                        '#####################################
                        '##########  CODICI   ################
                        '#####################################

                        'Prelevo l'elenco dei codici
                        xCodici = xParcoMacchina.GetElementsByTagName("CodiceParcoMacchina")

                        i_Codice = 0

                        Do While i_Codice < xCodici.Count

                            'Prelevo l'i-esimo codice
                            xCodice = xCodici.Item(i_Codice)

                            'Prelevo gli attributi del codice selezionato
                            OpeDB_Codice = xCodice.GetAttribute("TipoOperazioneDB")

                            objParcoMacchineCodici = New AgronicaCoreContabDAL.Parco_Macchine_Codici_W

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Codice

                                Case "0"    'LEGGI -------------------------------------------------------


                                Case "1"    'SALVA -------------------------------------------------------

                                    dummy = objParcoMacchineCodici.Scrivi(CStr(xParcoMacchina.GetAttribute("piva")),
                                                                          CInt(xParcoMacchina.GetAttribute("sa_cod")),
                                                                          CInt(xParcoMacchina.GetAttribute("mac_cod")),
                                                                          CInt(xCodice.GetAttribute("id_cod")),
                                                                          CStr(xCodice.GetAttribute("val_cod")),
                                                                          CDate(xCodice.GetAttribute("validita_inizio")),
                                                                          CDate(xCodice.GetAttribute("validita_fine")),
                                                                          objParametri)

                                Case "2"    'MODIFICA -------------------------------------------------------

                                    dummy = objParcoMacchineCodici.Modifica(CStr(xParcoMacchina.GetAttribute("piva")),
                                                                            CInt(xParcoMacchina.GetAttribute("sa_cod")),
                                                                            CInt(xParcoMacchina.GetAttribute("mac_cod")),
                                                                            CInt(xCodice.GetAttribute("id_cod")),
                                                                            CStr(xCodice.GetAttribute("val_cod")),
                                                                            CDate(xCodice.GetAttribute("validita_inizio")),
                                                                            CDate(xCodice.GetAttribute("validita_fine")),
                                                                            "",
                                                                            objParametri)

                                Case "3"    'ELIMINA -------------------------------------------------------

                                    dummy = objParcoMacchineCodici.Cancella(CStr(xParcoMacchina.GetAttribute("piva")),
                                                                            CInt(xParcoMacchina.GetAttribute("sa_cod")),
                                                                            CInt(xParcoMacchina.GetAttribute("mac_cod")),
                                                                            CInt(xCodice.GetAttribute("id_cod")),
                                                                            "",
                                                                            objParametri)

                            End Select

                            objParcoMacchineCodici = Nothing
                            i_Codice += 1

                        Loop

                        objParcoMacchine = Nothing
                        i_Parco_Macchina += 1

                    Loop



                    'Inizializzo preventivamente il Cod_Progetto (Gestione Zootecnica)
                    Cod_Progetto = CInt(xMovimentoDettaglio.GetAttribute("cod_progetto"))


                    '##################################################
                    '###############  ANAGRAFE ZOOTECNICA  ############
                    '##################################################

                    'Prelevo l'elenco delle Anagrafe Zootecniche
                    xZooAnagrafiche = xMovimentoDettaglio.GetElementsByTagName("Zoo_Animale_Anagrafe")

                    If xZooAnagrafiche.Count = 1 Then

                        Throw New NotImplementedException("Funzione di creazione animale obsoleta.")

                    End If





                    '##############################################################
                    '#####################  CAMPIONATURA  RACCOLTO  ###############
                    '##############################################################


                    'Inizializzazione Progressivo
                    Progressivo = Agro_XML_GetInteger(xMovimentoDettaglio, "cal_cod", 0)

                    'Prelevo gli attributi del movimento dettaglio selezionato
                    OpeDB_Movimento_Dettaglio = xMovimentoDettaglio.GetAttribute("TipoOperazioneDB")

                    'Nota Importante: Occorre filtrare le sole operazioni che creano una campionatura.
                    'Questo è necessario poiché, al fine di importare/esportare i dati per il gias standalone,
                    'la classe di lettura del dettaglio-movimento impacchetta anche le campionature.
                    'Senza questo controllo quindi il componente tenterebbe di creare un nuovo progressivo per
                    'ogni operazione con cal_cod < 0.

                    Select Case Lav_Cod

                        Case LAVCOD_RACCOLTA, LAVCOD_ACCETTAZIONE, LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_TRASFORMAZIONI

                            xRaccoltoCampionature = xMovimentoDettaglio.GetElementsByTagName("Raccolto_Campionatura")

                            If xRaccoltoCampionature.Count > 0 Then

                                If OpeDB_Movimento_Dettaglio <> 1 Then

                                    'Modifica/Cancellazione

                                Else

                                    'Inserimento

                                    If Progressivo = 0 Then
                                        Progressivo = objSequenze.NuovoId_Tabella("Materie_Prime_Campionature",
                                                                                  0, 2000000000,
                                                                                  objParametri)

                                        Progressivo = -Math.Abs(Progressivo) 'valore negativo
                                    End If

                                    i_Raccolto_Campionature = 0

                                    objMpCampionature = New AgronicaCoreContabDAL.Materie_Prime_Campion_W

                                    Do While i_Raccolto_Campionature < xRaccoltoCampionature.Count

                                        'Prelevo l'i-esimo blocco di xRaccolto_Campionature
                                        xRaccoltoCampionatura = xRaccoltoCampionature.Item(i_Raccolto_Campionature)

                                        'Prelevo gli attributi della campionatura selezionata
                                        OpeDB_Campionatura = xRaccoltoCampionatura.GetAttribute("TipoOperazioneDB")

                                        'Verifico l'operazione richiesta
                                        Select Case OpeDB_Campionatura

                                            Case "0"    'LEGGI -------------------------------------------------------
                                            '
                                            Case "1"    'SALVA -------------------------------------------------------

                                                'Devo verificare se il record esiste già, quindi vado in update, altrimenti scrittura vera e propria
                                                Dim objMpCampionatureR As New Materie_Prime_Campionature_R
                                                Dim dtMatCamp As DataTable = objMpCampionatureR.Leggi(Progressivo,
                                                                                                  CStr(xRaccoltoCampionatura.GetAttribute("tipo")),
                                                                                                  CInt(xRaccoltoCampionatura.GetAttribute("tipo_cod")),
                                                                                                  CInt(xRaccoltoCampionatura.GetAttribute("udm_cod")),
                                                                                                  0, "",
                                                                                                  True,
                                                                                                  enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                                  "", "",
                                                                                                  objParametri)

                                                If dtMatCamp IsNot Nothing AndAlso dtMatCamp.Rows.Count > 0 Then
                                                    'Aggiorno la campionatura
                                                    dummy = objMpCampionature.Modifica(Progressivo,
                                                                                   CStr(xRaccoltoCampionatura.GetAttribute("tipo")),
                                                                                   CInt(xRaccoltoCampionatura.GetAttribute("tipo_cod")),
                                                                                   CInt(xRaccoltoCampionatura.GetAttribute("udm_cod")),
                                                                                   objParametri,
                                                                                   Val_Cod:=CStr(xRaccoltoCampionatura.GetAttribute("val_cod")),
                                                                                   Descrizione:=CStr(xRaccoltoCampionatura.GetAttribute("descrizione")),
                                                                                   Validita_Inizio:=CDate(xRaccoltoCampionatura.GetAttribute("validita_inizio")),
                                                                                   Validita_Fine:=CDate(xRaccoltoCampionatura.GetAttribute("validita_fine")),
                                                                                   Progressivo_Origine:=Agro_XML_GetInteger(xRaccoltoCampionatura, "progressivo_origine", 0),
                                                                                   Piva_SuperUser_Origine:=CStr(xRaccoltoCampionatura.GetAttribute("piva_superuser_origine")),
                                                                                   Peso_Campione:=Agro_XML_GetDecimal(xRaccoltoCampionatura, "peso_campione", 0),
                                                                                   ChkStima:=Agro_XML_GetInteger(xRaccoltoCampionatura, "chkstima", 0),
                                                                                   ChkTara_Campionatura:=Agro_XML_GetDecimal(xRaccoltoCampionatura, "chktara_campionatura", 0),
                                                                                   Tara_Campionatura:=Agro_XML_GetDecimal(xRaccoltoCampionatura, "tara_campionatura", 0)
                                                                                   )
                                                Else
                                                    'Salvo la Campionatura
                                                    dummy = objMpCampionature.Scrivi(Progressivo,
                                                                                 CStr(xRaccoltoCampionatura.GetAttribute("tipo")),
                                                                                 CInt(xRaccoltoCampionatura.GetAttribute("tipo_cod")),
                                                                                 CInt(xRaccoltoCampionatura.GetAttribute("udm_cod")),
                                                                                 CStr(xRaccoltoCampionatura.GetAttribute("val_cod")),
                                                                                 CStr(xRaccoltoCampionatura.GetAttribute("descrizione")),
                                                                                 Agro_XML_GetDecimal(xRaccoltoCampionatura, "peso_campione", 0),
                                                                                 Agro_XML_GetInteger(xRaccoltoCampionatura, "progressivo_origine", 0),
                                                                                 CStr(xRaccoltoCampionatura.GetAttribute("piva_superuser_origine")),
                                                                                 CDate(xRaccoltoCampionatura.GetAttribute("validita_inizio")),
                                                                                 CDate(xRaccoltoCampionatura.GetAttribute("validita_fine")),
                                                                                 objParametri,
                                                                                 ChkStima:=Agro_XML_GetInteger(xRaccoltoCampionatura, "chkstima", 0),
                                                                                 ChkTara_Campionatura:=Agro_XML_GetDecimal(xRaccoltoCampionatura, "chktara_campionatura", 0),
                                                                                 Tara_Campionatura:=Agro_XML_GetDecimal(xRaccoltoCampionatura, "tara_campionatura", 0)
                                                                                 )
                                                End If



                                            Case "2"    'MODIFICA -------------------------------------------------------
                                            '

                                            Case "3"    'ELIMINA -------------------------------------------------------

                                                '@MIRROR@

                                                If Flag_Mirror = 1 Then

                                                    '************************************************
                                                    '************************************************
                                                    '*********** INIZIO MIRRORING *******************
                                                    '************************************************

                                                    'IN CASO DI CANCELLAZIONE, VA INSERITA L'OPERAZIONE NELLA TABELLA MIRROR
                                                    'E POI CANCELLATA NELLA TABELLA BUONA


                                                    'MATPRIME_CAMP_MIRROR_W

                                                    objMatPrimeCampionatureMirrorW = New AgronicaCoreContabDAL.MatPrime_Camp_Mirror_W


                                                    'Salvo la Campionatura
                                                    dummy = objMatPrimeCampionatureMirrorW.Scrivi(
                                                            Progressivo,
                                                            CStr(xRaccoltoCampionatura.GetAttribute("tipo")),
                                                            CInt(xRaccoltoCampionatura.GetAttribute("tipo_cod")),
                                                            CInt(xRaccoltoCampionatura.GetAttribute("udm_cod")),
                                                            Progressivo_Mirror,
                                                            CStr(xRaccoltoCampionatura.GetAttribute("val_cod")),
                                                            CStr(xRaccoltoCampionatura.GetAttribute("descrizione")),
                                                            CDate(xRaccoltoCampionatura.GetAttribute("validita_inizio")),
                                                            CDate(xRaccoltoCampionatura.GetAttribute("validita_fine")),
                                                            objParametri)


                                                    objMatPrimeCampionatureMirrorW = Nothing


                                                    '************************************************
                                                    '*********** FINE MIRRORING *********************
                                                    '************************************************
                                                    '************************************************

                                                End If


                                        End Select

                                        i_Raccolto_Campionature += 1

                                    Loop

                                End If

                            End If

                    End Select


                    '=================================================================================================
                    'GESTIONE CATEGORIE SENSIBILI
                    '-------------------------------------------------------------------------------------------------

                    Lotto = UtilityProvider.Agro_SQL_SaveText(xMovimentoDettaglio.GetAttribute("lotto"))

                    '(21/10/2022) Allineamento a vecchia agenda e LAN, queste non utilizzano più il lotto "indefinito",
                    '             quindi scrivo il campo così come già valorizzato su xMovimentoDettaglio
                    'Select Case CInt(xMovimentoDettaglio.GetAttribute("elem_cod"))

                    '    Case ALTRE_MATERIE, SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI,
                    '     SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI,
                    '     MANGIMI, SEMENTI,
                    '     CAT_MAG_SERVIZI_PROFESSIONALI

                    '        If Lotto = "" Then
                    '            Lotto = "Indefinito" '-->Impostazione Forzata Lotto
                    '        End If

                    'End Select
                    '=================================================================================================


                    objMovimentiDettagli = New AgronicaCoreContabDAL.Movimenti_Dettagli_W
                    objRicxCod = New AgronicaCoreContabDAL.RicxConti_W
                    objRicxCodPat = New AgronicaCoreContabDAL.RicxConti_Patrimonio_W
                    objContabHlp = New AgronicaCoreContabHLP.Contabilita


                    'Inizializzo Preventivamente il Cod_Movimento_Dettaglio
                    Cod_Movimento_Dettaglio = CInt(xMovimentoDettaglio.GetAttribute("id_mov_det"))

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Movimento_Dettaglio

                        Case "0"    'LEGGI -------------------------------------------------------
                            '
                        Case "1"    'SALVA -------------------------------------------------------

                            If Cod_Movimento_Dettaglio <= 0 Then

                                'Richiedo un nuovo codice movimento_dettaglio
                                Cod_Movimento_Dettaglio = objSequenze.NuovoId_Tabella("Movimenti_Dettagli",
                                                                                      CInt(xMovimentoDettaglio.GetAttribute("basecode")),
                                                                                      CInt(xMovimentoDettaglio.GetAttribute("topcode")),
                                                                                      objParametri)

                            Else

                                'Esportazione in Locale

                            End If

                            Agenda_W.ImpostaRimappaturaCodici(vCodiciRimappati, "Movimento_Dettaglio",
                                                              objParametri.PivaSuperUser & "," &
                                                              CStr(xMovimentoDettaglio.GetAttribute("piva")) & "," &
                                                              CStr(xMovimentoDettaglio.GetAttribute("sa_cod")) & "," &
                                                              Id_Agenda & "," &
                                                              Id_Mov & "," &
                                                              CStr(Cod_Movimento_Dettaglio)
                                                              )


                            extraDate = Agro_XML_GetDate(xMovimentoDettaglio, "extra_date", AGRODATAINIZIO)

                            If extraDate.Year < 1900 Then
                                extraDate = AGRODATAINIZIO
                            End If

                            Dim contabilizzato As Integer
                            If G2G Then
                                contabilizzato = CInt(xMovimentoDettaglio.GetAttribute("contabilizzato"))
                            Else
                                Dim dataOperazione As Date
                                If CDate(xMovimentoDettaglio.GetAttribute("validita_inizio")) <> AGRODATAINIZIO Then
                                    dataOperazione = CDate(xMovimentoDettaglio.GetAttribute("validita_inizio"))
                                Else
                                    dataOperazione = DataMovimento
                                End If
                                contabilizzato = If(dataOperazione > CDate(Now), -Math.Abs(CInt(xMovimentoDettaglio.GetAttribute("contabilizzato"))), Math.Abs(CInt(xMovimentoDettaglio.GetAttribute("contabilizzato"))))
                            End If


                            dummy = objMovimentiDettagli.Scrivi(
                                            CStr(xMovimentoDettaglio.GetAttribute("piva")),
                                            CInt(xMovimentoDettaglio.GetAttribute("sa_cod")),
                                            CInt(Id_Agenda),
                                            CInt(Id_Mov),
                                            CInt(Cod_Movimento_Dettaglio),
                                            CInt(xMovimentoDettaglio.GetAttribute("elem_cod")),
                                            CInt(xMovimentoDettaglio.GetAttribute("pro_cod")),
                                            CInt(Mat_Cod),
                                            CStr(xMovimentoDettaglio.GetAttribute("mov_det_des")),
                                            CDbl(xMovimentoDettaglio.GetAttribute("qta")),
                                            CInt(xMovimentoDettaglio.GetAttribute("udm_cod")),
                                            CInt(xMovimentoDettaglio.GetAttribute("cod_iva")),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "jolly_int", 0),
                                            CDbl(xMovimentoDettaglio.GetAttribute("sconto")),
                                            CDbl(xMovimentoDettaglio.GetAttribute("prezzo_unitario")),
                                            If(xMovimentoDettaglio.HasAttribute("prezzo_unitario_netto") = False, CDbl(xMovimentoDettaglio.GetAttribute("prezzo_unitario")), xMovimentoDettaglio.GetAttribute("prezzo_unitario_netto")),
                                            CInt(xMovimentoDettaglio.GetAttribute("cod_conto")),
                                            Progressivo,
                                            Cod_Progetto,
                                            CInt(xMovimentoDettaglio.GetAttribute("fase_cod")),
                                            Agro_XML_GetString(xMovimentoDettaglio, "extra_str", ""),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "extra_int", 0),
                                            extraDate,
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "ric_cod", 0),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "anno", 0),
                                            If(xMovimentoDettaglio.HasAttribute("imponibile") = False, 0, xMovimentoDettaglio.GetAttribute("imponibile")),
                                            If(xMovimentoDettaglio.HasAttribute("imponibile_netto") = False, 0, xMovimentoDettaglio.GetAttribute("imponibile_netto")),
                                            If(xMovimentoDettaglio.HasAttribute("iva") = False, 0, xMovimentoDettaglio.GetAttribute("iva")),
                                            Agro_SQL_SaveNum(xMovimentoDettaglio.GetAttribute("listino_cod"), False),
                                            contabilizzato,
                                            CInt(xMovimentoDettaglio.GetAttribute("pendente")),
                                            Lotto,
                                            Agro_SQL_SaveNum(xMovimentoDettaglio.GetAttribute("udm_cod_extra"), False),
                                            If(xMovimentoDettaglio.HasAttribute("qta_extra") = False, 0, xMovimentoDettaglio.GetAttribute("qta_extra")),
                                            If(xMovimentoDettaglio.HasAttribute("qta_extra_totale") = False, 0, xMovimentoDettaglio.GetAttribute("qta_extra_totale")),
                                            If(xMovimentoDettaglio.HasAttribute("prezzo_effettivo") = False, 0, xMovimentoDettaglio.GetAttribute("prezzo_effettivo")),
                                            If(xMovimentoDettaglio.HasAttribute("variazione") = False, 0, xMovimentoDettaglio.GetAttribute("variazione")),
                                            If(xMovimentoDettaglio.HasAttribute("tara") = False, 0, xMovimentoDettaglio.GetAttribute("tara")),
                                            Agro_SQL_SaveNum(xMovimentoDettaglio.GetAttribute("chklayout_hide"), False),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "chkiva_manuale", 0),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "cod_ivaindetraibile", 0),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "tempocarenza", 0),
                                            Agro_XML_GetString(xMovimentoDettaglio, "doseetichetta", ""),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "turno_cod", 0),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "id_attivita", 0),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "dettaglio_vegcod", 0),
                                            Agro_XML_GetString(xMovimentoDettaglio, "principiattivi", ""),
                                            Agro_XML_GetString(xMovimentoDettaglio, "classitossicologiche", ""),
                                            Agro_XML_GetString(xMovimentoDettaglio, "doseetichetta_value", ""),
                                            CDate(xMovimentoDettaglio.GetAttribute("validita_inizio")),
                                            CDate(xMovimentoDettaglio.GetAttribute("validita_fine")),
                                            objParametri,
                                            Data_creazione,
                                            Data_modifica,
                                            username_creazione,
                                            username_modifica,
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "mat_cod_alias", 0),
                                            If(xMovimentoDettaglio.HasAttribute("qta_dettaglio1") = False, 0, xMovimentoDettaglio.GetAttribute("qta_dettaglio1")),
                                            If(xMovimentoDettaglio.HasAttribute("qta_dettaglio2") = False, 0, xMovimentoDettaglio.GetAttribute("qta_dettaglio2")),
                                            If(xMovimentoDettaglio.HasAttribute("sconto_listino") = False, 0, xMovimentoDettaglio.GetAttribute("sconto_listino")),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "sconto_modalita", 0),
                                            Agro_XML_GetString(xMovimentoDettaglio, "sconto_testo", ""),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "qualifica_cod", 0),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "tariffa_cod", 0),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "mezzo_det", -1),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "ric_cod_pat", 0),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "cod_conto_pat", 0),
                                            If(xMovimentoDettaglio.HasAttribute("iva_indetraibile") = False, 0, xMovimentoDettaglio.GetAttribute("iva_indetraibile")),
                                            If(xMovimentoDettaglio.HasAttribute("iva_indetraibile_perc") = False, 0, xMovimentoDettaglio.GetAttribute("iva_indetraibile_perc")),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "iva_deto_cod", 0),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "dettagli_blocco_flag", 0),
                                            If(xMovimentoDettaglio.HasAttribute("dettagli_blocco_username") = False, "0", xMovimentoDettaglio.GetAttribute("dettagli_blocco_username")),
                                            Agro_XML_GetDate(xMovimentoDettaglio, "dettagli_blocco_data", AGRODATAINIZIO),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "ordine_det", 0),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "deroga_cod", 0),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "prezzo_livello", 0),
                                            Agro_XML_GetString(xMovimentoDettaglio, "principiattivipesi", ""),
                                            Agro_XML_GetString(xMovimentoDettaglio, "buffer", ""),
                                            Agro_XML_GetString(xMovimentoDettaglio, "rif_esterno", ""),
                                            Agro_XML_GetString(xMovimentoDettaglio, "rif_esterno_2", ""),
                                            Agro_XML_GetString(xMovimentoDettaglio, "principiattivipercabb", ""),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "polverulento", 0),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "dettaglio_idcod", 0),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "dettaglio_gencod", 0),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "dettaglio_specod", 0),
                                            Agro_XML_GetInteger(xMovimentoDettaglio, "dettaglio_iprocod", 0))

                        Case "2"    'MODIFICA -------------------------------------------------------
                            '
                            'Non Gestito

                    End Select




                    '#############################################
                    '##########  MOVIMENTI DESTINAZIONI ##########
                    '#############################################

                    'Prelevo l'elenco delle destinazioni
                    xMovimentiDestinazioni = xMovimentoDettaglio.GetElementsByTagName("Movimento_Destinazione")

                    i_Movimento_Destinazione = 0

                    Do While i_Movimento_Destinazione < xMovimentiDestinazioni.Count

                        'Prelevo l'i-esima destinazione del movimento dettaglio
                        xMovimentoDestinazione = xMovimentiDestinazioni.Item(i_Movimento_Destinazione)

                        'Prelevo gli attributi della destinazione selezionata
                        OpeDB_Movimento_Destinazione = xMovimentoDestinazione.GetAttribute("TipoOperazioneDB")

                        objMovDestinazioni = New AgronicaCoreContabDAL.Mov_Destinazioni_W


                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Movimento_Destinazione

                            Case "0"    'LEGGI -------------------------------------------------------
                                '
                            Case "1"    'SALVA -------------------------------------------------------

                                'Salvo la destinazione
                                dummy = objMovDestinazioni.Scrivi(CStr(xMovimentoDettaglio.GetAttribute("piva")),
                                                                  CInt(xMovimentoDettaglio.GetAttribute("sa_cod")),
                                                                  CInt(Id_Agenda),
                                                                  CInt(Id_Mov),
                                                                  CInt(Cod_Movimento_Dettaglio),
                                                                  CInt(xMovimentoDestinazione.GetAttribute("appezza")),
                                                                  CInt(xMovimentoDestinazione.GetAttribute("id_destinazione")),
                                                                  CInt(xMovimentoDestinazione.GetAttribute("tipo_destinazione")),
                                                                  CDbl(xMovimentoDestinazione.GetAttribute("qta")),
                                                                  CDbl(xMovimentoDestinazione.GetAttribute("qta2")),
                                                                  Agro_XML_GetInteger(xMovimentoDestinazione, "tipo_scorta", 0),
                                                                  Agro_XML_GetDecimal(xMovimentoDestinazione, "scorta_min", 0),
                                                                  "",
                                                                  Agro_XML_GetDecimal(xMovimentoDestinazione, "quotadistribuzione", 0),
                                                                  CDate(xMovimentoDestinazione.GetAttribute("validita_inizio")),
                                                                  CDate(xMovimentoDestinazione.GetAttribute("validita_fine")),
                                                                  objParametri,
                                                                  Data_creazione,
                                                                  Data_modifica,
                                                                  username_creazione,
                                                                  username_modifica,
                                                                  Qta_Dest1:=Agro_XML_GetDecimal(xMovimentoDestinazione, "qta_dest1", 0),
                                                                  Qta_Dest2:=Agro_XML_GetDecimal(xMovimentoDestinazione, "qta_dest2", 0),
                                                                  Sup_Riduzione_BufferZone:=Agro_XML_GetDecimal(xMovimentoDestinazione, "sup_riduzione_bufferzone", 0),
                                                                  Perc_Riduzione_Deriva:=Agro_XML_GetDecimal(xMovimentoDestinazione, "perc_riduzione_deriva", 0),
                                                                  Sa_Cod_Riferimento:=Agro_XML_GetInteger(xMovimentoDestinazione, "sa_cod_riferimento", 0),
                                                                  Id_Destinazione_Riferimento:=Agro_XML_GetInteger(xMovimentoDestinazione, "id_destinazione_riferimento", 0),
                                                                  Tipo_Destinazione_Riferimento:=Agro_XML_GetInteger(xMovimentoDestinazione, "tipo_destinazione_riferimento", 0)
                                                                  )


                            Case "2"    'MODIFICA -------------------------------------------------------

                                objMovDestinazioni.ModificaPuntuale(CStr(xMovimentoDettaglio.GetAttribute("piva")),
                                                                    CInt(xMovimentoDettaglio.GetAttribute("sa_cod")),
                                                                    CInt(xMovimentoDettaglio.GetAttribute("id_agenda")),
                                                                    CInt(xMovimentoDettaglio.GetAttribute("id_mov")),
                                                                    CInt(Cod_Movimento_Dettaglio),
                                                                    CInt(xMovimentoDestinazione.GetAttribute("appezza")),
                                                                    CInt(xMovimentoDestinazione.GetAttribute("id_destinazione")),
                                                                    objParametri,
                                                                    Tipo_Destinazione:=CInt(xMovimentoDestinazione.GetAttribute("tipo_destinazione")),
                                                                    Qta:=CDbl(xMovimentoDestinazione.GetAttribute("qta")),
                                                                    Qta2:=CDbl(xMovimentoDestinazione.GetAttribute("qta2")),
                                                                    Validita_Inizio:=CDate(xMovimentoDestinazione.GetAttribute("validita_inizio")),
                                                                    Validita_Fine:=CDate(xMovimentoDestinazione.GetAttribute("validita_fine")),
                                                                    Tipo_Scorta:=If(xMovimentoDestinazione.HasAttribute("tipo_scorta") = False, Nothing, CInt(xMovimentoDestinazione.GetAttribute("tipo_scorta"))),
                                                                    Scorta_Min:=If(xMovimentoDestinazione.HasAttribute("scorta_min") = False, Nothing, CDbl(xMovimentoDestinazione.GetAttribute("scorta_min"))),
                                                                    Mov_Destinazioni_Graphickey:="",
                                                                    Qta_Dest1:=If(xMovimentoDestinazione.HasAttribute("qta_dest1") = False, Nothing, CDbl(xMovimentoDestinazione.GetAttribute("qta_dest1"))),
                                                                    Qta_Dest2:=If(xMovimentoDestinazione.HasAttribute("qta_dest2") = False, Nothing, CDbl(xMovimentoDestinazione.GetAttribute("qta_dest2"))),
                                                                    QuotaDistribuzione:=If(xMovimentoDestinazione.HasAttribute("qta_dest2") = False, Nothing, CDbl(xMovimentoDestinazione.GetAttribute("quotadistribuzione"))))

                            Case "3"    'ELIMINA -------------------------------------------------------
                                '
                                '@MIRROR@

                                If Flag_Mirror = 1 Then

                                    '************************************************
                                    '************************************************
                                    '*********** INIZIO MIRRORING *******************
                                    '************************************************

                                    'IN CASO DI CANCELLAZIONE, VA INSERITA L'OPERAZIONE NELLA TABELLA MIRROR
                                    'E POI CANCELLATA NELLA TABELLA BUONA


                                    'MOV_DESTINAZ_MIRROR_W

                                    objMovDestinazioneMirrorW = New AgronicaCoreContabDAL.Mov_Destinaz_Mirror_W

                                    dummy = objMovDestinazioneMirrorW.Scrivi(
                                                CStr(xMovimentoDettaglio.GetAttribute("piva")),
                                                CInt(xMovimentoDettaglio.GetAttribute("sa_cod")),
                                                CInt(Id_Agenda),
                                                CInt(Id_Mov),
                                                CInt(Cod_Movimento_Dettaglio),
                                                CInt(xMovimentoDestinazione.GetAttribute("appezza")),
                                                CInt(xMovimentoDestinazione.GetAttribute("id_destinazione")),
                                                Progressivo_Mirror,
                                                CInt(xMovimentoDestinazione.GetAttribute("tipo_destinazione")),
                                                CDbl(xMovimentoDestinazione.GetAttribute("qta")),
                                                CDbl(xMovimentoDestinazione.GetAttribute("qta2")),
                                                IIf(xMovimentoDestinazione.HasAttribute("tipo_scorta") = False, 0, xMovimentoDestinazione.GetAttribute("tipo_scorta")),
                                                IIf(xMovimentoDestinazione.HasAttribute("scorta_min") = False, 0, xMovimentoDestinazione.GetAttribute("scorta_min")),
                                                CDate(xMovimentoDestinazione.GetAttribute("validita_inizio")),
                                                CDate(xMovimentoDestinazione.GetAttribute("validita_fine")),
                                                objParametri)


                                    objMovDestinazioneMirrorW = Nothing


                                    '************************************************
                                    '*********** FINE MIRRORING *********************
                                    '************************************************
                                    '************************************************

                                End If 'Flag_Mirror

                                objMovDestinazioni.Cancella(CStr(xMovimentoDettaglio.GetAttribute("piva")),
                                                            CInt(xMovimentoDettaglio.GetAttribute("sa_cod")),
                                                            CInt(xMovimentoDettaglio.GetAttribute("id_agenda")),
                                                            CInt(xMovimentoDettaglio.GetAttribute("id_mov")),
                                                            CInt(Cod_Movimento_Dettaglio),
                                                            0,
                                                            0,
                                                            "",
                                                            objParametri)

                        End Select


                        ' Giulia 7/2019: Soppresso, perché il record statico delle giacenze non è più utilizzato

                        ''##############################################################
                        ''#####  GIACENZE DI MAGAZZINO (QUANTITATIVA-QUALITATIVA)  #####
                        ''##############################################################

                        ''Se la Destinazione è 20 -> l'Operazione coinvolge le giacenze di magazzino

                        ''La gestione delle giacenze non deve essere movimentata se la connessione è locale
                        ''poiché in caso di importazione dati verrebbe movimentata 2 volte!

                        ''Le giacenze devono essere aggiornate se l'operazione non è pianificata

                        ''Il campo Jolly_Int se = 1 --> il movimento non riguarda il magazzino

                        ''Se CInt(xMovimento_Dettaglio.getAttribute("contabilizzato")) < 0 --> si sta cancellando una pianificazione

                        ''-----

                        ''If CInt(xMovimento_Destinazione.GetAttribute("tipo_destinazione")) = 20 And _
                        ''   ConnessioneAlternativa <> "CnGIAS_Local" And _
                        ''   CDate(xMovimento_Destinazione.GetAttribute("validita_inizio")) <= CDate(Now) And _
                        ''   CInt(xMovimento_Dettaglio.GetAttribute("contabilizzato")) >= 0 Then

                        'If CInt(xMovimentoDestinazione.GetAttribute("tipo_destinazione")) = 20 And
                        '   CDate(xMovimentoDestinazione.GetAttribute("validita_inizio")) <= CDate(Now) And
                        '   CInt(xMovimentoDettaglio.GetAttribute("contabilizzato")) >= 0 Then


                        '    'Verifico che la causale sia gestita dal modulo "Giacenze"
                        '    Select Case CStr(xMovimentoDettaglio.GetAttribute("cau_mov"))

                        '        Case "7300", "7350", "4100", "4200", "7900", "7920" 'Carico, Scarico, Conferimento, Conferimento a diversi, Accettazione, Accettazione da Diversi

                        '            'Causale Gestita
                        '            objGiacenze = New AgronicaCoreContabBIZ.Giacenze_W

                        '            '===================================================================================
                        '            'Setting dei Parametri quantitativi del movimento dettaglio
                        '            '-----------------------------------------------------------------------------------
                        '            mId_Destinazione = CInt(xMovimentoDestinazione.GetAttribute("id_destinazione"))

                        '            mBasecode = 0
                        '            mTopcode = 2000000000

                        '            dummy = objGiacenze.Giacenza_Scrivi(
                        '                        CStr(xMovimentoDestinazione.GetAttribute("piva")),
                        '                        CInt(xMovimentoDestinazione.GetAttribute("sa_cod")),
                        '                        CInt(xMovimentoDettaglio.GetAttribute("elem_cod")),
                        '                        CInt(xMovimentoDettaglio.GetAttribute("pro_cod")),
                        '                        CInt(Mat_Cod),
                        '                        CInt(xMovimentoDettaglio.GetAttribute("udm_cod")),
                        '                        mId_Destinazione,
                        '                        OpeDB_Movimento_Destinazione,
                        '                        CStr(xMovimentoDettaglio.GetAttribute("cau_mov")),
                        '                        Progressivo,
                        '                        Cod_Progetto,
                        '                        CInt(xMovimentoDettaglio.GetAttribute("fase_cod")),
                        '                        Lotto,
                        '                        CDbl(xMovimentoDettaglio.GetAttribute("qta")),
                        '                        IIf(xMovimentoDettaglio.HasAttribute("prezzo_unitario_netto") = False, xMovimentoDettaglio.GetAttribute("prezzo_unitario"), xMovimentoDettaglio.GetAttribute("prezzo_unitario_netto")),
                        '                        Agro_SQL_SaveNum(xMovimentoDettaglio.GetAttribute("udm_cod_extra")),
                        '                        IIf(xMovimentoDettaglio.HasAttribute("qta_extra") = False, 0, xMovimentoDettaglio.GetAttribute("qta_extra")),
                        '                        IIf(xMovimentoDettaglio.HasAttribute("qta_extra_totale") = False, 0, xMovimentoDettaglio.GetAttribute("qta_extra_totale")),
                        '                        IIf(xMovimentoDettaglio.HasAttribute("variazione") = False, 0, xMovimentoDettaglio.GetAttribute("variazione")),
                        '                        Lav_Cod,
                        '                        Agro_SQL_SaveNum(xMovimentoDettaglio.GetAttribute("jolly_int")),
                        '                        mBasecode,
                        '                        mTopcode,
                        '                        CDate(xMovimentoDettaglio.GetAttribute("validita_inizio")),
                        '                        CDate(xMovimentoDettaglio.GetAttribute("validita_fine")),
                        '                        objParametri)

                        '            objGiacenze = Nothing


                        '        Case Else 'Causale Non Gestita

                        '            'Eccezione

                        '    End Select

                        'End If


                        '#######################################################################
                        '#####  CONSISTENZA ENOLOGICA IN VASCA (QUANTITATIVA-QUALITATIVA)  #####
                        '#######################################################################

                        'Se la Destinazione è 13 -> l'Operazione coinvolge le consistenze enologiche in vasca

                        'La gestione delle consistenze non deve essere movimentata se la connessione è locale
                        'poiché in caso di importazione dati verrebbe movimentata 2 volte!

                        'Le consistenze devono essere aggiornate se l'operazione non è pianificata

                        'Il campo Jolly_Int se = 1 --> il movimento non riguarda la vasca

                        'Se CInt(xMovimento_Dettaglio.getAttribute("contabilizzato")) < 0 --> si sta cancellando una pianificazione


                        'If cint(xMovimento_Destinazione.GetAttribute("tipo_destinazione")) = 13 And _
                        '   ConnessioneAlternativa <> "CnGIAS_Local" And _
                        '   CDate(xMovimento_Destinazione.GetAttribute("validita_inizio")) <= CDate(Now) And _
                        '   CInt(xMovimento_Dettaglio.GetAttribute("contabilizzato")) >= 0 Then

                        If CInt(xMovimentoDestinazione.GetAttribute("tipo_destinazione")) = 13 AndAlso
                           CDate(xMovimentoDestinazione.GetAttribute("validita_inizio")) <= CDate(Now) AndAlso
                           CInt(xMovimentoDettaglio.GetAttribute("contabilizzato")) >= 0 Then

                            'Verifico che la causale sia gestita dal modulo "Consistenze Enologiche"
                            Select Case CStr(xMovimentoDettaglio.GetAttribute("cau_mov"))

                                Case CAU_CARICO, CAU_SCARICO

                                    'Causale Gestita
                                    objGiacenze = New AgronicaCoreContabBIZ.Giacenze_W

                                    '===================================================================================
                                    'Setting dei Parametri quantitativi del movimento dettaglio
                                    '-----------------------------------------------------------------------------------
                                    mId_Destinazione = CInt(xMovimentoDestinazione.GetAttribute("id_destinazione"))

                                    mBasecode = 0
                                    mTopcode = 2000000000

                                    dummy = objGiacenze.ConsistenzeEnologiche_Scrivi(
                                                CStr(xMovimentoDestinazione.GetAttribute("piva")),
                                                CInt(xMovimentoDestinazione.GetAttribute("sa_cod")),
                                                CInt(xMovimentoDettaglio.GetAttribute("elem_cod")),
                                                CInt(xMovimentoDettaglio.GetAttribute("pro_cod")),
                                                CInt(Mat_Cod),
                                                CInt(xMovimentoDettaglio.GetAttribute("udm_cod")),
                                                mId_Destinazione,
                                                OpeDB_Movimento_Destinazione,
                                                CStr(xMovimentoDettaglio.GetAttribute("cau_mov")),
                                                Progressivo,
                                                Cod_Progetto,
                                                CInt(xMovimentoDettaglio.GetAttribute("fase_cod")),
                                                Lotto,
                                                CDbl(xMovimentoDettaglio.GetAttribute("qta")),
                                                IIf(xMovimentoDettaglio.HasAttribute("prezzo_unitario_netto") = False, xMovimentoDettaglio.GetAttribute("prezzo_unitario"), xMovimentoDettaglio.GetAttribute("prezzo_unitario_netto")),
                                                Agro_SQL_SaveNum(xMovimentoDettaglio.GetAttribute("udm_cod_extra"), False),
                                                IIf(xMovimentoDettaglio.HasAttribute("qta_extra") = False, 0, xMovimentoDettaglio.GetAttribute("qta_extra")),
                                                IIf(xMovimentoDettaglio.HasAttribute("qta_extra_totale") = False, 0, xMovimentoDettaglio.GetAttribute("qta_extra_totale")),
                                                IIf(xMovimentoDettaglio.HasAttribute("variazione") = False, 0, xMovimentoDettaglio.GetAttribute("variazione")),
                                                Lav_Cod,
                                                Agro_SQL_SaveNum(xMovimentoDettaglio.GetAttribute("jolly_int"), False),
                                                mBasecode,
                                                mTopcode,
                                                CDate(xMovimentoDettaglio.GetAttribute("validita_inizio")),
                                                CDate(xMovimentoDettaglio.GetAttribute("validita_fine")),
                                                objParametri)


                                    objGiacenze = Nothing


                                Case Else 'Causale Non Gestita

                                    'Eccezione

                            End Select

                        End If


                        '##############################################################
                        '#####  CONSISTENZA DI STALLA (QUANTITATIVA-QUALITATIVA)  #####
                        '##############################################################

                        'Se la Destinazione è 15 -> l'Operazione coinvolge le Consistenze

                        'La gestione delle giacenze non deve essere movimentata se la connessione è locale
                        'poiché in caso di importazione dati verrebbe movimentata 2 volte!

                        'Le consistenze devono essere aggiornate se l'operazione non è pianificata

                        'Il campo Jolly_Int se = 1 --> il movimento non riguarda la stalla

                        'Se CInt(xMovimento_Dettaglio.getAttribute("contabilizzato")) < 0 --> si sta cancellando una pianificazione

                        'If CInt(xMovimento_Destinazione.GetAttribute("tipo_destinazione")) = 15 And _
                        '    ConnessioneAlternativa <> "CnGIAS_Local" And _
                        '    CDate(xMovimento_Destinazione.GetAttribute("validita_inizio")) <= CDate(Now) And _
                        '    CInt(xMovimento_Dettaglio.GetAttribute("contabilizzato")) >= 0 Then

                        If CInt(xMovimentoDestinazione.GetAttribute("tipo_destinazione")) = 15 AndAlso
                           CDate(xMovimentoDestinazione.GetAttribute("validita_inizio")) <= CDate(Now) AndAlso
                           CInt(xMovimentoDettaglio.GetAttribute("contabilizzato")) >= 0 Then

                            'Verifico che la causale sia gestita dal modulo "Consistenze"
                            Select Case CStr(xMovimentoDettaglio.GetAttribute("cau_mov"))

                                Case CAU_CARICO, CAU_SCARICO

                                    'Causale Gestita
                                    objGiacenze = New AgronicaCoreContabBIZ.Giacenze_W

                                    '===================================================================================
                                    'Setting dei Parametri quantitativi del movimento dettaglio
                                    '-----------------------------------------------------------------------------------
                                    mId_Destinazione = CInt(xMovimentoDestinazione.GetAttribute("id_destinazione"))

                                    mBasecode = 0
                                    mTopcode = 2000000000

                                    'Nota: Forzo la chiave per evitare guai
                                    dummy = objGiacenze.Consistenze_Scrivi(
                                                CStr(xMovimentoDestinazione.GetAttribute("piva")),
                                                CInt(xMovimentoDestinazione.GetAttribute("sa_cod")),
                                                300,
                                                0,
                                                0,
                                                enum_UnitaMisura.Numero,
                                                mId_Destinazione,
                                                OpeDB_Movimento_Destinazione,
                                                CStr(xMovimentoDettaglio.GetAttribute("cau_mov")),
                                                0,
                                                Cod_Progetto,
                                                0,
                                                "",
                                                CDbl(xMovimentoDettaglio.GetAttribute("qta")),
                                                IIf(xMovimentoDettaglio.HasAttribute("prezzo_unitario_netto") = False, xMovimentoDettaglio.GetAttribute("prezzo_unitario"), xMovimentoDettaglio.GetAttribute("prezzo_unitario_netto")),
                                                Agro_SQL_SaveNum(xMovimentoDettaglio.GetAttribute("udm_cod_extra"), False),
                                                IIf(xMovimentoDettaglio.HasAttribute("qta_extra") = False, 0, xMovimentoDettaglio.GetAttribute("qta_extra")),
                                                IIf(xMovimentoDettaglio.HasAttribute("qta_extra_totale") = False, 0, xMovimentoDettaglio.GetAttribute("qta_extra_totale")),
                                                IIf(xMovimentoDettaglio.HasAttribute("variazione") = False, 0, xMovimentoDettaglio.GetAttribute("variazione")),
                                                Lav_Cod,
                                                Agro_SQL_SaveNum(xMovimentoDettaglio.GetAttribute("jolly_int"), False),
                                                CDate(xMovimentoDettaglio.GetAttribute("validita_inizio")),
                                                mBasecode, mTopcode,
                                                CDate(xMovimentoDettaglio.GetAttribute("validita_inizio")),
                                                CDate(xMovimentoDettaglio.GetAttribute("validita_fine")),
                                                objParametri)

                                    objGiacenze = Nothing

                            End Select

                        End If

                        objMovDestinazioni = Nothing
                        i_Movimento_Destinazione += 1

                    Loop



                    '#############################################
                    '##########  MOVIMENTI RIFERIMENTI  ##########
                    '#############################################

                    'Prelevo l'elenco dei riferimenti del movimento dettaglio
                    xMovDetRiferimenti = xMovimentoDettaglio.GetElementsByTagName("Movimento_Riferimento2")

                    i_Mov_Det_Riferimento = 0

                    Do While i_Mov_Det_Riferimento < xMovDetRiferimenti.Count

                        'Prelevo l'i-esimo riferimento del movimento dettaglio
                        xMovDetRiferimento = xMovDetRiferimenti.Item(i_Mov_Det_Riferimento)

                        'Prelevo gli attributi del riferimento selezionato
                        OpeDB_Mov_Det_Riferimento = xMovDetRiferimento.GetAttribute("TipoOperazioneDB")

                        objMovDetRiferimenti = New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Mov_Det_Riferimento

                            Case "0"    'LEGGI -------------------------------------------------------
                                '
                            Case "1"    'SALVA -------------------------------------------------------


                                '****** Modifica Mauro 24/07/2008 ****************
                                ' Passo piva, sa_cod e id_agenda di xMov_Riferimento invece di quelli di xMovimento
                                ' perché altrimenti può generare errori di chiave duplicata!
                                '*******************************************************

                                If CInt(xMovDetRiferimento.GetAttribute("id_mov")) <> 0 AndAlso
                                   CInt(xMovDetRiferimento.GetAttribute("id_agenda")) <> 0 AndAlso
                                   CStr(xMovDetRiferimento.GetAttribute("piva")) <> "" Then


                                    'Controllo che il record non sia già presente

                                    Dim objComRif As AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                                    Dim objDtRif As DataTable
                                    Dim Presente As Boolean = True

                                    objComRif = New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R

                                    objDtRif = objComRif.LeggixChiave(
                                                CStr(xMovDetRiferimento.GetAttribute("piva")),
                                                CInt(xMovDetRiferimento.GetAttribute("sa_cod")),
                                                CInt(xMovDetRiferimento.GetAttribute("id_agenda")),
                                                CInt(xMovDetRiferimento.GetAttribute("id_mov")),
                                                CInt(xMovDetRiferimento.GetAttribute("id_mov_det")),
                                                CInt(xMovDetRiferimento.GetAttribute("id_agenda_rif")),
                                                CInt(xMovDetRiferimento.GetAttribute("id_mov_rif")),
                                                CInt(xMovDetRiferimento.GetAttribute("id_mov_det_rif")),
                                                0,
                                                0,
                                                "",
                                                objParametri)


                                    If Not IsNothing(objDtRif) Then
                                        If objDtRif.Rows.Count > 0 Then
                                            Presente = True
                                        Else
                                            Presente = False
                                        End If
                                    End If

                                    objDtRif.Dispose()
                                    objDtRif = Nothing
                                    objComRif = Nothing


                                    If Not Presente Then
                                        'scrivo
                                        dummy = objMovDetRiferimenti.Scrivi(
                                                CStr(xMovDetRiferimento.GetAttribute("piva")),
                                                CInt(xMovDetRiferimento.GetAttribute("sa_cod")),
                                                CInt(xMovDetRiferimento.GetAttribute("id_agenda")),
                                                CInt(xMovDetRiferimento.GetAttribute("id_mov")),
                                                CInt(xMovDetRiferimento.GetAttribute("id_mov_det")),
                                                Agro_SQL_SaveNum(xMovDetRiferimento.GetAttribute("lav_cod"), False),
                                                Agro_SQL_SaveText(xMovDetRiferimento.GetAttribute("cau_mov"), False),
                                                Agro_SQL_SaveText(xMovDetRiferimento.GetAttribute("piva_rif"), False),
                                                Agro_SQL_SaveNum(xMovDetRiferimento.GetAttribute("sa_cod_rif"), False),
                                                CInt(xMovDetRiferimento.GetAttribute("id_agenda_rif")),
                                                CInt(xMovDetRiferimento.GetAttribute("id_mov_rif")),
                                                CInt(xMovDetRiferimento.GetAttribute("id_mov_det_rif")),
                                                Agro_SQL_SaveNum(xMovDetRiferimento.GetAttribute("lav_cod_rif"), False),
                                                CStr(xMovDetRiferimento.GetAttribute("cau_mov_rif")),
                                                IIf(xMovDetRiferimento.HasAttribute("qta") = False, 0, xMovDetRiferimento.GetAttribute("qta")),
                                                CDate(xMovimentoDettaglio.GetAttribute("validita_inizio")),
                                                CDate(xMovimentoDettaglio.GetAttribute("validita_fine")),
                                                objParametri,
                                                Data_creazione,
                                                Data_modifica,
                                                username_creazione,
                                                username_modifica)

                                    End If 'FINE: If Not presente Then


                                Else 'If CInt(xMov_Det_Riferimento.getAttribute("id_mov")) <> 0 And CInt(xMov_Det_Riferimento.getAttribute("id_agenda")) <> 0 And CStr(xMov_Det_Riferimento.getAttribute("piva")) <> "" Then

                                    'Salvo il riferimento
                                    dummy = objMovDetRiferimenti.Scrivi(CStr(xMovDetRiferimento.GetAttribute("piva")),
                                                                        CInt(xMovDetRiferimento.GetAttribute("sa_cod")),
                                                                        CInt(Id_Agenda),
                                                                        CInt(Id_Mov),
                                                                        CInt(Cod_Movimento_Dettaglio),
                                                                        Agro_SQL_SaveNum(xMovDetRiferimento.GetAttribute("lav_cod"), False),
                                                                        Agro_SQL_SaveText(xMovDetRiferimento.GetAttribute("cau_mov"), False),
                                                                        Agro_SQL_SaveText(xMovDetRiferimento.GetAttribute("piva_rif"), False),
                                                                        Agro_SQL_SaveNum(xMovDetRiferimento.GetAttribute("sa_cod_rif"), False),
                                                                        CInt(xMovDetRiferimento.GetAttribute("id_agenda_rif")),
                                                                        CInt(xMovDetRiferimento.GetAttribute("id_mov_rif")),
                                                                        CInt(xMovDetRiferimento.GetAttribute("id_mov_det_rif")),
                                                                        Agro_SQL_SaveNum(xMovDetRiferimento.GetAttribute("lav_cod_rif"), False),
                                                                        CStr(xMovDetRiferimento.GetAttribute("cau_mov_rif")),
                                                                        IIf(xMovDetRiferimento.HasAttribute("qta") = False, 0, xMovDetRiferimento.GetAttribute("qta")),
                                                                        CDate(xMovimentoDettaglio.GetAttribute("validita_inizio")),
                                                                        CDate(xMovimentoDettaglio.GetAttribute("validita_fine")),
                                                                        objParametri,
                                                                        Data_creazione,
                                                                        Data_modifica,
                                                                        username_creazione,
                                                                        username_modifica)

                                End If 'FINE: If CInt(xMov_Det_Riferimento.getAttribute("id_mov")) <> 0 And CInt(xMov_Det_Riferimento.getAttribute("id_agenda")) <> 0 And CStr(xMov_Det_Riferimento.getAttribute("piva")) <> "" Then

                                '
                            Case "2"    'MODIFICA -------------------------------------------------------

                                'Non Gestito
                                '

                            Case "3"    'ELIMINA -------------------------------------------------------
                                '

                                '@MIRROR@

                                If Flag_Mirror = 1 Then

                                    '************************************************
                                    '************************************************
                                    '*********** INIZIO MIRRORING *******************
                                    '************************************************


                                    'IN CASO DI CANCELLAZIONE, VA INSERITA L'OPERAZIONE NELLA TABELLA MIRROR
                                    'E POI CANCELLATA NELLA TABELLA BUONA


                                    'MOV_DETT_RIFERIM_MIRROR_W

                                    objMovDettRiferimentoMirrorW = New AgronicaCoreContabDAL.Mov_DettRiferim_Mirror_W

                                    dummy = objMovDettRiferimentoMirrorW.Scrivi(
                                                CStr(xMovDetRiferimento.GetAttribute("piva")),
                                                CInt(xMovDetRiferimento.GetAttribute("sa_cod")),
                                                CInt(Id_Agenda),
                                                CInt(Id_Mov),
                                                CInt(Cod_Movimento_Dettaglio),
                                                Agro_SQL_SaveNum(xMovDetRiferimento.GetAttribute("lav_cod"), False),
                                                Agro_SQL_SaveText(xMovDetRiferimento.GetAttribute("cau_mov"), False),
                                                Agro_SQL_SaveText(xMovDetRiferimento.GetAttribute("piva_rif"), False),
                                                Agro_SQL_SaveNum(xMovDetRiferimento.GetAttribute("sa_cod_rif"), False),
                                                CInt(xMovDetRiferimento.GetAttribute("id_agenda_rif")),
                                                CInt(xMovDetRiferimento.GetAttribute("id_mov_rif")),
                                                CInt(xMovDetRiferimento.GetAttribute("id_mov_det_rif")),
                                                Progressivo_Mirror,
                                                Agro_SQL_SaveNum(xMovDetRiferimento.GetAttribute("lav_cod_rif"), False),
                                                CStr(xMovDetRiferimento.GetAttribute("cau_mov_rif")),
                                                IIf(xMovDetRiferimento.HasAttribute("qta") = False, 0, xMovDetRiferimento.GetAttribute("qta")),
                                                objParametri)


                                    objMovDettRiferimentoMirrorW = Nothing


                                    '************************************************
                                    '*********** FINE MIRRORING *********************
                                    '************************************************
                                    '************************************************

                                End If 'Flag_Mirror


                                objMovDetRiferimenti.Cancella(CStr(xMovDetRiferimento.GetAttribute("piva")),
                                                              0,
                                                              CInt(xMovDetRiferimento.GetAttribute("id_agenda")),
                                                              CInt(xMovDetRiferimento.GetAttribute("id_mov")),
                                                              CInt(Cod_Movimento_Dettaglio),
                                                              "",
                                                              objParametri)

                        End Select

                        objMovDetRiferimenti = Nothing
                        i_Mov_Det_Riferimento += 1

                    Loop


                    '##################################################
                    '##########  MOVIMENTI DETTAGLI TECNICI  ##########
                    '##################################################

                    'Prelevo l'elenco dei dettagli tecnici del movimento
                    xMovDettagliTecnici = xMovimentoDettaglio.GetElementsByTagName("Movimento_Dettaglio_Tecnico_2")

                    i_Mov_Dettaglio_Tecnico = 0

                    Do While i_Mov_Dettaglio_Tecnico < xMovDettagliTecnici.Count

                        'Prelevo l'i-esimo dettaglio tecnico
                        xMovDettaglioTecnico = xMovDettagliTecnici.Item(i_Mov_Dettaglio_Tecnico)

                        'Prelevo gli attributi del dettaglio tecnico selezionato
                        OpeDB_Mov_Dettaglio_Tecnico = xMovDettaglioTecnico.GetAttribute("TipoOperazioneDB")

                        'Inizializzo Preventivamente il Codice Dettaglio Tecnico
                        Cod_Id_Reg_Dettaglio = CInt(xMovDettaglioTecnico.GetAttribute("id_reg_dettaglio"))

                        objMovDettaglioTecnico = New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W

                        'Verifico l'operazione richiesta
                        Select Case OpeDB_Mov_Dettaglio_Tecnico

                            Case "0"    'LEGGI -------------------------------------------------------
                                '
                            Case "1"    'SALVA -------------------------------------------------------

                                If Cod_Id_Reg_Dettaglio <= 0 Then

                                    'Richiedo un nuovo codice dettaglio tecnico
                                    Cod_Id_Reg_Dettaglio = objSequenze.NuovoId_Tabella("Movimenti_Dettagli_Tecnici",
                                                                                       CInt(xMovDettaglioTecnico.GetAttribute("basecode")),
                                                                                       CInt(xMovDettaglioTecnico.GetAttribute("topcode")),
                                                                                       objParametri)

                                Else

                                    'Esportazione in Locale

                                End If

                                'Salvo il dettaglio tecnico
                                dummy = objMovDettaglioTecnico.Scrivi(
                                                CStr(xMovimentoDettaglio.GetAttribute("piva")),
                                                CInt(xMovimentoDettaglio.GetAttribute("sa_cod")),
                                                CInt(Id_Agenda),
                                                CInt(Id_Mov),
                                                CInt(Cod_Movimento_Dettaglio),
                                                CInt(Cod_Id_Reg_Dettaglio),
                                                CInt(xMovDettaglioTecnico.GetAttribute("av_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("av_gru")),
                                                CStr(xMovDettaglioTecnico.GetAttribute("sigla_av")),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("data_ril")), xMovDettaglioTecnico.GetAttribute("data_ril"), New Date),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("qta_ril")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("dose")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("ditta_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("dett_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("id_insetto")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("ff_classe")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("mg")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("n")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("k")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("p")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("parziale")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("nitrati")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("freatimetro")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("piezo1")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("piezo2")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("piezo3")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("piezo4")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("trap_num")),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn1_data")), xMovDettaglioTecnico.GetAttribute("inn1_data"), New Date), IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn2_data")), xMovDettaglioTecnico.GetAttribute("inn2_data"), New Date),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn3_data")), xMovDettaglioTecnico.GetAttribute("inn3_data"), New Date), IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn4_data")), xMovDettaglioTecnico.GetAttribute("inn4_data"), New Date),
                                                Lotto,
                                                IIf(xMovDettaglioTecnico.HasAttribute("extra_int") = False, 0, xMovDettaglioTecnico.GetAttribute("extra_int")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("extra_str") = False, "", xMovDettaglioTecnico.GetAttribute("extra_str")),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("extra_date")), xMovDettaglioTecnico.GetAttribute("extra_date"), New Date),
                                                IIf(xMovDettaglioTecnico.HasAttribute("soglia_cod") = False, 0, xMovDettaglioTecnico.GetAttribute("soglia_cod")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("soglia_quantita") = False, 0, xMovDettaglioTecnico.GetAttribute("soglia_quantita")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("soglia_des") = False, "", xMovDettaglioTecnico.GetAttribute("soglia_des")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("efficienza") = False, 0, xMovDettaglioTecnico.GetAttribute("efficienza")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("cu") = False, 0, xMovDettaglioTecnico.GetAttribute("cu")),
                                                CDate(xMovDettaglioTecnico.GetAttribute("validita_inizio")),
                                                CDate(xMovDettaglioTecnico.GetAttribute("validita_fine")),
                                                objParametri,
                                                Data_creazione,
                                                Data_modifica,
                                                username_creazione,
                                                username_modifica)

                                '
                            Case "2"    'MODIFICA -------------------------------------------------------
                                '
                                objMovDettaglioTecnico.Modifica(
                                                CStr(xMovimentoDettaglio.GetAttribute("piva")),
                                                CInt(xMovimentoDettaglio.GetAttribute("sa_cod")),
                                                CInt(xMovimentoDettaglio.GetAttribute("id_agenda")),
                                                CInt(xMovimentoDettaglio.GetAttribute("id_mov")),
                                                CInt(Cod_Movimento_Dettaglio),
                                                CInt(Cod_Id_Reg_Dettaglio),
                                                CInt(xMovDettaglioTecnico.GetAttribute("av_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("av_gru")),
                                                CStr(xMovDettaglioTecnico.GetAttribute("sigla_av")),
                                                CDate(IIf(IsDate(xMovDettaglioTecnico.GetAttribute("data_ril")), xMovDettaglioTecnico.GetAttribute("data_ril"), New Date)),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("qta_ril")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("dose")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("ditta_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("dett_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("id_insetto")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("ff_classe")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("mg")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("n")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("k")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("p")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("parziale")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("nitrati")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("freatimetro")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("piezo1")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("piezo2")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("piezo3")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("piezo4")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("trap_num")),
                                                CDate(IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn1_data")), xMovDettaglioTecnico.GetAttribute("inn1_data"), New Date)),
                                                CDate(IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn2_data")), xMovDettaglioTecnico.GetAttribute("inn2_data"), New Date)),
                                                CDate(IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn3_data")), xMovDettaglioTecnico.GetAttribute("inn3_data"), New Date)),
                                                CDate(IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn4_data")), xMovDettaglioTecnico.GetAttribute("inn4_data"), New Date)),
                                                IIf(xMovDettaglioTecnico.HasAttribute("lotto") = False, "", xMovDettaglioTecnico.GetAttribute("lotto")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("extra_int") = False, 0, CInt(xMovDettaglioTecnico.GetAttribute("extra_int"))),
                                                IIf(xMovDettaglioTecnico.HasAttribute("extra_str") = False, "", xMovDettaglioTecnico.GetAttribute("extra_str")),
                                                CDate(IIf(IsDate(xMovDettaglioTecnico.GetAttribute("extra_date")), xMovDettaglioTecnico.GetAttribute("extra_date"), New Date)),
                                                IIf(xMovDettaglioTecnico.HasAttribute("soglia_cod") = False, 0, xMovDettaglioTecnico.GetAttribute("soglia_cod")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("soglia_quantita") = False, 0, xMovDettaglioTecnico.GetAttribute("soglia_quantita")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("soglia_des") = False, "", xMovDettaglioTecnico.HasAttribute("soglia_des")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("efficienza") = False, 0, xMovDettaglioTecnico.GetAttribute("efficienza")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("cu") = False, 0, xMovDettaglioTecnico.GetAttribute("cu")),
                                                CDate(xMovDettaglioTecnico.GetAttribute("validita_inizio")),
                                                CDate(xMovDettaglioTecnico.GetAttribute("validita_fine")),
                                                "",
                                                objParametri)
                                '
                            Case "3"    'ELIMINA -------------------------------------------------------
                                '

                                '@MIRROR@

                                If Flag_Mirror = 1 Then


                                    '************************************************
                                    '************************************************
                                    '*********** INIZIO MIRRORING *******************
                                    '************************************************


                                    'IN CASO DI CANCELLAZIONE, VA INSERITA L'OPERAZIONE NELLA TABELLA MIRROR
                                    'E POI CANCELLATA NELLA TABELLA BUONA


                                    'MOV_DETT_TECNICO_MIRROR_W

                                    objMovDettTecnicoMirrorW = New AgronicaCoreContabDAL.Mov_DettTecnico_Mirror_W

                                    dummy = objMovDettTecnicoMirrorW.Scrivi(
                                                CStr(xMovimentoDettaglio.GetAttribute("piva")),
                                                CInt(xMovimentoDettaglio.GetAttribute("sa_cod")),
                                                CInt(Id_Agenda),
                                                CInt(Id_Mov),
                                                CInt(Cod_Movimento_Dettaglio),
                                                CInt(Cod_Id_Reg_Dettaglio),
                                                Progressivo_Mirror,
                                                CInt(xMovDettaglioTecnico.GetAttribute("av_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("av_gru")),
                                                CStr(xMovDettaglioTecnico.GetAttribute("sigla_av")),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("data_ril")), xMovDettaglioTecnico.GetAttribute("data_ril"), New Date),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("qta_ril")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("dose")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("ditta_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("dett_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("id_insetto")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("ff_classe")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("mg")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("n")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("k")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("p")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("parziale")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("nitrati")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("freatimetro")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("piezo1")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("piezo2")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("piezo3")),
                                                CDbl(xMovDettaglioTecnico.GetAttribute("piezo4")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("trap_num")),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn1_data")), xMovDettaglioTecnico.GetAttribute("inn1_data"), New Date), IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn2_data")), xMovDettaglioTecnico.GetAttribute("inn2_data"), New Date),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn3_data")), xMovDettaglioTecnico.GetAttribute("inn3_data"), New Date), IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn4_data")), xMovDettaglioTecnico.GetAttribute("inn4_data"), New Date),
                                                IIf(xMovDettaglioTecnico.HasAttribute("lotto") = False, "", xMovDettaglioTecnico.GetAttribute("lotto")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("extra_int") = False, 0, xMovDettaglioTecnico.GetAttribute("extra_int")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("extra_str") = False, "", xMovDettaglioTecnico.GetAttribute("extra_str")),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("extra_date")), xMovDettaglioTecnico.GetAttribute("extra_date"), New Date),
                                                CDate(xMovDettaglioTecnico.GetAttribute("validita_inizio")),
                                                CDate(xMovDettaglioTecnico.GetAttribute("validita_fine")),
                                                objParametri)


                                    objMovDettTecnicoMirrorW = Nothing


                                    '************************************************
                                    '*********** FINE MIRRORING *********************
                                    '************************************************
                                    '************************************************

                                End If 'Flag_Mirror

                                objMovDettaglioTecnico.Cancella(CStr(xMovimentoDettaglio.GetAttribute("piva")),
                                                                CInt(xMovimentoDettaglio.GetAttribute("sa_cod")),
                                                                CInt(xMovimentoDettaglio.GetAttribute("id_agenda")),
                                                                CInt(xMovimentoDettaglio.GetAttribute("id_mov")),
                                                                CInt(Cod_Movimento_Dettaglio),
                                                                CInt(Cod_Id_Reg_Dettaglio),
                                                                "",
                                                                objParametri)

                        End Select

                        objMovDettaglioTecnico = Nothing
                        i_Mov_Dettaglio_Tecnico += 1

                    Loop


                    Select Case OpeDB_Movimento_Dettaglio

                        Case "2" '-

                            'NON GESTITA

                        Case "3" 'CANCELLAZIONE MOVIMENTO DETTAGLIO

                            '@MIRROR@

                            If Flag_Mirror = 1 Then

                                '************************************************
                                '************************************************
                                '*********** INIZIO MIRRORING *******************
                                '************************************************

                                'IN CASO DI CANCELLAZIONE, VA INSERITA L'OPERAZIONE NELLA TABELLA MIRROR
                                'E POI CANCELLATA NELLA TABELLA BUONA


                                'MOV_DETTAGLI_MIRROR_W

                                objMovimentoDettaglioMirrorW = New AgronicaCoreContabDAL.Mov_Dettagli_Mirror_W

                                'scrivo nella tabella mirror
                                dummy = objMovimentoDettaglioMirrorW.Scrivi(
                                                CStr(xMovimentoDettaglio.GetAttribute("piva")),
                                                CInt(xMovimentoDettaglio.GetAttribute("sa_cod")),
                                                CInt(Id_Agenda),
                                                CInt(Id_Mov),
                                                CInt(Cod_Movimento_Dettaglio), Progressivo_Mirror,
                                                CInt(xMovimentoDettaglio.GetAttribute("elem_cod")),
                                                CInt(xMovimentoDettaglio.GetAttribute("pro_cod")),
                                                CInt(Mat_Cod),
                                                CStr(xMovimentoDettaglio.GetAttribute("mov_det_des")),
                                                CDbl(xMovimentoDettaglio.GetAttribute("qta")),
                                                CInt(xMovimentoDettaglio.GetAttribute("udm_cod")),
                                                CInt(xMovimentoDettaglio.GetAttribute("cod_iva")),
                                                IIf(xMovimentoDettaglio.HasAttribute("jolly_int") = False, 0, xMovimentoDettaglio.GetAttribute("jolly_int")),
                                                CDbl(xMovimentoDettaglio.GetAttribute("sconto")),
                                                CDbl(xMovimentoDettaglio.GetAttribute("prezzo_unitario")),
                                                IIf(xMovimentoDettaglio.HasAttribute("prezzo_unitario_netto") = False, CDbl(xMovimentoDettaglio.GetAttribute("prezzo_unitario")), xMovimentoDettaglio.GetAttribute("prezzo_unitario_netto")),
                                                CInt(xMovimentoDettaglio.GetAttribute("cod_conto")),
                                                Progressivo,
                                                Cod_Progetto,
                                                CInt(xMovimentoDettaglio.GetAttribute("fase_cod")),
                                                IIf(xMovimentoDettaglio.HasAttribute("extra_str") = False, "", xMovimentoDettaglio.GetAttribute("extra_str")),
                                                IIf(xMovimentoDettaglio.HasAttribute("extra_int") = False, 0, xMovimentoDettaglio.GetAttribute("extra_int")),
                                                IIf(IsDate(xMovimentoDettaglio.GetAttribute("extra_date")), xMovimentoDettaglio.GetAttribute("extra_date"), New Date),
                                                IIf(xMovimentoDettaglio.HasAttribute("ric_cod") = False, 0, xMovimentoDettaglio.GetAttribute("ric_cod")),
                                                IIf(xMovimentoDettaglio.HasAttribute("anno") = False, 0, xMovimentoDettaglio.GetAttribute("anno")),
                                                IIf(xMovimentoDettaglio.HasAttribute("imponibile") = False, 0, xMovimentoDettaglio.GetAttribute("imponibile")),
                                                IIf(xMovimentoDettaglio.HasAttribute("imponibile_netto") = False, 0, xMovimentoDettaglio.GetAttribute("imponibile_netto")),
                                                IIf(xMovimentoDettaglio.HasAttribute("iva") = False, 0, xMovimentoDettaglio.GetAttribute("iva")),
                                                IIf(CDate(xMovimentoDettaglio.GetAttribute("validita_inizio")) > CDate(Now), -Math.Abs(CInt(xMovimentoDettaglio.GetAttribute("contabilizzato"))), Math.Abs(CInt(xMovimentoDettaglio.GetAttribute("contabilizzato")))),
                                                CInt(xMovimentoDettaglio.GetAttribute("pendente")),
                                                Lotto,
                                                Agro_SQL_SaveNum(xMovimentoDettaglio.GetAttribute("udm_cod_extra"), False),
                                                IIf(xMovimentoDettaglio.HasAttribute("qta_extra") = False, 0, xMovimentoDettaglio.GetAttribute("qta_extra")),
                                                CDate(xMovimentoDettaglio.GetAttribute("validita_inizio")),
                                                CDate(xMovimentoDettaglio.GetAttribute("validita_fine")),
                                                objParametri)




                                objMovimentoDettaglioMirrorW = Nothing


                                '************************************************
                                '*********** FINE MIRRORING *********************
                                '************************************************
                                '************************************************

                            End If 'Flag_Mirror

                            objMovimentiDettagli.Cancella(CStr(xMovimentoDettaglio.GetAttribute("piva")),
                                                          CInt(xMovimentoDettaglio.GetAttribute("sa_cod")),
                                                          CInt(xMovimentoDettaglio.GetAttribute("id_agenda")),
                                                          CInt(xMovimentoDettaglio.GetAttribute("id_mov")),
                                                          CInt(Cod_Movimento_Dettaglio),
                                                          "",
                                                          objParametri)

                    End Select


                    '##################################################
                    '############## PIANO DEI CONTI  ##################
                    '##################################################


                    If CInt(xMovimentoDettaglio.GetAttribute("cod_conto")) <> 0 AndAlso
                       IsNumeric(xMovimentoDettaglio.GetAttribute("ric_cod")) AndALso
                       IsNumeric(xMovimentoDettaglio.GetAttribute("anno")) AndAlso
                       IsNumeric(xMovimentoDettaglio.GetAttribute("imponibile")) Then

                        Select Case OpeDB_Movimento_Dettaglio
                            Case "1" : mImponibile = xMovimentoDettaglio.GetAttribute("imponibile")
                            Case "3" : mImponibile = -xMovimentoDettaglio.GetAttribute("imponibile")
                            Case Else : mImponibile = 0
                        End Select


                        objRicxCod.Modifica_Solo_Saldo(CStr(xMovimentoDettaglio.GetAttribute("piva")),
                                                       CInt(xMovimentoDettaglio.GetAttribute("ric_cod")),
                                                       CInt(xMovimentoDettaglio.GetAttribute("cod_conto")),
                                                       CInt(xMovimentoDettaglio.GetAttribute("anno")),
                                                       mImponibile,
                                                       CDate(xMovimentoDettaglio.GetAttribute("validita_inizio")),
                                                       CDate(xMovimentoDettaglio.GetAttribute("validita_fine")),
                                                       "",
                                                       objParametri)

                    End If


                    '  Giulia, 08/05/2017 12:56:19: Aggiunta gestione anche del piano dei conti patrimoniale


                    '###############################################################
                    '############## PIANO DEI CONTI PATRIMONIALE  ##################
                    '###############################################################


                    If xMovimentoDettaglio.HasAttribute("cod_conto_pat") AndAlso CInt(xMovimentoDettaglio.GetAttribute("cod_conto_pat")) <> 0 AndAlso
                       xMovimentoDettaglio.HasAttribute("ric_cod_pat") AndAlso IsNumeric(xMovimentoDettaglio.GetAttribute("ric_cod_pat")) AndAlso
                       xMovimentoDettaglio.HasAttribute("anno") AndAlso IsNumeric(xMovimentoDettaglio.GetAttribute("anno")) AndAlso
                       xMovimentoDettaglio.HasAttribute("imponibile") AndAlso IsNumeric(xMovimentoDettaglio.GetAttribute("imponibile")) AndAlso
                       Not objContabHlp.Bypass_Imputazione_Conto(Lav_Cod) Then


                        'Aggiungo all'imponibile anche l'iva_indetraibile (costo)
                        If xMovimentoDettaglio.HasAttribute("iva_indetraibile") AndAlso IsNumeric(xMovimentoDettaglio.GetAttribute("iva_indetraibile")) Then
                            ivaIndetraibile = xMovimentoDettaglio.GetAttribute("iva_indetraibile")
                        Else
                            ivaIndetraibile = 0
                        End If


                        '======================================================================================================================================================
                        'Controllo Modalita Sconto
                        '------------------------------------------------------------------------------------------------------------------------------------------------------
                        If objContabHlp.IVA_Debito(Lav_Cod, 1) <> 0 Then

                            If xMovimentoDettaglio.HasAttribute("sconto_modalita") AndAlso IsNumeric(xMovimentoDettaglio.GetAttribute("sconto_modalita")) Then
                                modalitaSconto = UtilityProvider.Agro_SQL_SaveNum(xMovimentoDettaglio.GetAttribute("sconto_modalita"))
                            Else
                                modalitaSconto = 0
                            End If

                            importoOmaggio = 0

                            Select Case modalitaSconto

                                Case 2 'enModalitaSconto.Campioni_Omaggio

                                    importoOmaggio = Format(Math.Abs(CDec(IIf(xMovimentoDettaglio.HasAttribute("imponibile_netto") = False, 0, xMovimentoDettaglio.GetAttribute("imponibile_netto")))) -
                                                             (CDec(IIf(xMovimentoDettaglio.HasAttribute("iva") = False, 0, xMovimentoDettaglio.GetAttribute("iva")))), "##,###,###.00")

                                Case 4 'enModalitaSconto.Rivalsa_Iva

                                    importoOmaggio = Format(CDec(IIf(xMovimentoDettaglio.HasAttribute("imponibile_netto") = False, 0, xMovimentoDettaglio.GetAttribute("imponibile_netto"))), "##,###,###.00")

                            End Select

                        End If
                        '===============================================================================================================


                        Select Case OpeDB_Movimento_Dettaglio
                            Case "1"
                                mImponibile = xMovimentoDettaglio.GetAttribute("imponibile") + ivaIndetraibile - importoOmaggio
                                mIva = IIf(xMovimentoDettaglio.HasAttribute("iva") = False, 0, xMovimentoDettaglio.GetAttribute("iva")) + ivaIndetraibile
                            Case "3"
                                mImponibile = -xMovimentoDettaglio.GetAttribute("imponibile") - ivaIndetraibile + importoOmaggio
                                mIva = -IIf(xMovimentoDettaglio.HasAttribute("iva") = False, 0, xMovimentoDettaglio.GetAttribute("iva")) - ivaIndetraibile
                            Case Else
                                mImponibile = 0
                                mIva = 0
                        End Select

                        objRicxCodPat.Modifica_Solo_Saldo(CStr(xMovimentoDettaglio.GetAttribute("piva")),
                                                          CInt(xMovimentoDettaglio.GetAttribute("ric_cod_pat")),
                                                          CInt(xMovimentoDettaglio.GetAttribute("cod_conto_pat")),
                                                          CInt(xMovimentoDettaglio.GetAttribute("anno")),
                                                          mImponibile,
                                                          objContabHlp.IVA_Credito(Lav_Cod, mIva),
                                                          objContabHlp.IVA_Debito(Lav_Cod, mIva),
                                                          CDate(xMovimentoDettaglio.GetAttribute("validita_inizio")),
                                                          CDate(xMovimentoDettaglio.GetAttribute("validita_fine")),
                                                          "",
                                                          objParametri)

                    End If

                    objMovimentiDettagli = Nothing
                    i_Movimenti_Dettagli += 1

                Loop

                i_DatiMovimenti_Dettagli += 1

            Loop



            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            xDatiMovimentiDettagli = Nothing
            xDatiMovimentoDettaglio = Nothing
            xMovimentiDettagli = Nothing
            xMovimentoDettaglio = Nothing
            xMovimentiDestinazioni = Nothing
            xMovimentoDestinazione = Nothing
            xMovDetRiferimenti = Nothing
            xMovDetRiferimento = Nothing
            xMovDettagliTecnici = Nothing
            xMovDettaglioTecnico = Nothing
            xmlDoc = Nothing

            objSequenze = Nothing

            '------------------------------

            'Restituisco un valore Dummy
            xRisp = True

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            If flagTransazioneLocale = True Then
                objParametri.objTransazione.Commit()
            End If

            '----------------------------------------------------------------------------

        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If objParametri.objTransazione IsNot Nothing Then
                objParametri.objTransazione.Rollback()
                objParametri.objTransazione = Nothing
            End If

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            If (flagConnessioneLocale = True) AndAlso (objParametri.objConnessione IsNot Nothing) Then
                objParametri.objConnessione.Close()
            End If

        End Try

        CodiciRimappati = String.Join("|", vCodiciRimappati)

        Return xRisp

    End Function

    Public Function Aggiorna_PrincipiAttiviPesi(ByVal EsisteChiave As Boolean,
                                                ByRef objParametri_Server As AgronicaCoreParametri,
                                                ByRef objParametri_Utenti As AgronicaCoreParametri
                                                ) As Boolean


        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.Movimenti_Dettagli_W.Aggiorna_PrincipiAttiviPesi()"


        '------------------------------
        'Dim flagTransazioneLocale As Boolean = False
        'Dim flagConnessioneLocale As Boolean = False

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------

        Try


            '------------------------------

            ''Se la connessione è chiusa la apro
            'If objParametri_Server.objConnessione Is Nothing Then
            '    'Richiedo una connessione
            '    objParametri_Server.objConnessione =  DataProviderFactory.Instance.CreaNuovaConnessione(objParametri_Server.StringaConnessione)
            '    objParametri_Server.objConnessione.Open()
            '    flagConnessioneLocale = True
            '    objParametri_Server.objTransazione = Nothing
            'End If

            'If objParametri_Server.objTransazione Is Nothing Then
            '    'Inizializzo la transazione
            '    objParametri_Server.objTransazione = objParametri_Server.objConnessione.BeginTransaction
            '    flagTransazioneLocale = True
            'End If

            '------------------------------

            Dim strFrCod As New System.Text.StringBuilder
            Dim HashFito As New Hashtable

            'estraggo i dettagli relativi a prodotti fitosanitari
            Dim objMovDettR As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            Dim DtDettFito As DataTable = objMovDettR.Leggi_ProCod_Utilizzati("", 0, 0, 0, 0, 0, FORMULATI, 0, 0, 0, CAU_TRATTAMENTO, "", "", "", objParametri_Server)
            For i = 0 To DtDettFito.Rows.Count - 1
                strFrCod.Append(DtDettFito.Rows(i).Item("pro_cod") & ",")
            Next

            'DtDettFito = objMovDettR.Leggi("", 0, 0, 0, 0, FORMULATI, 0, 0, CAU_TRATTAMENTO, 0, 0, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            'For i = 0 To DtDettFito.Rows.Count - 1
            '    If Not HashFito.ContainsKey(DtDettFito.Rows(i).Item("pro_cod")) Then
            '        strFrCod.Append(DtDettFito.Rows(i).Item("pro_cod") & ",")
            '        HashFito.Add(DtDettFito.Rows(i).Item("pro_cod"), "")
            '    End If
            'Next

            If strFrCod.Length > 0 Then

                Dim strFC As String = Left(strFrCod.ToString, strFrCod.ToString.Length - 1)

                'ricavo via ws i pesi delle sostanze dei prodotti movimentati
                Dim objWs As New AgronicaCoreWebService.AgroWs
                Dim DtFor As New DataTable
                DtFor = objWs.ComposizioneFormulatiRecupera(strFC, objParametri_Server, objParametri_Utenti, False)

                If DtFor IsNot Nothing AndAlso DtFor.Rows.Count > 0 Then

                    Dim objMovDettW As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
                    objMovDettW.Modifica_PrincipiAttiviPesi_Da_DtFrCod(DtFor, "", objParametri_Server)

                End If

            End If


            Dim xScriviCS As New AgronicaCoreVarieDAL.Configurazione_Siti_W
            Dim bDummy As Boolean
            If EsisteChiave = False Then
                bDummy = xScriviCS.Scrivi(0, "Aggiorna_PesiSA_Operazioni", "false", objParametri_Server)
            Else
                bDummy = xScriviCS.AggiornaConfigurazione(0, "Aggiorna_PesiSA_Operazioni", "false", objParametri_Server)
            End If

            'Restituisco un valore Dummy
            xRisp = True

            ''Se ho la transazione è stata avviata in questa routine faccio il commit
            'If flagTransazioneLocale = True Then
            '    objParametri_Server.objTransazione.Commit()
            'End If

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            ''Faccio il rollback della transazione
            'If Not objParametri_Server.objTransazione Is Nothing Then
            '    objParametri_Server.objTransazione.Rollback()
            '    objParametri_Server.objTransazione = Nothing
            'End If

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        Finally

            ''Chiudo la connessione se è stata aperta in questa routine
            'If (flagConnessioneLocale = True) AndAlso (Not objParametri_Server.objConnessione Is Nothing) Then
            '    objParametri_Server.objConnessione.Close()
            'End If

        End Try

        Return xRisp

    End Function

    Public Function Scrivi_Modifica(ByVal Movimenti_dettagli As AgronicaCoreEntityFramework_POCO.Movimenti_dettagli,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByRef objParametriServer As AgronicaCoreParametri
                                    ) As Integer

        Const nomeRoutine = "ContabBIZ.Movimenti_Dettagli_W.Scrivi_Modifica()"
        Dim messaggioErrore As String = ""
        Dim idMovDet As Integer = 0

        Try

            Dim agroDP As New Agro_Sequenze
            Dim esiste As Boolean = False
            Dim cambioChiave = False
            If Movimenti_dettagli.Id_Mov_Det = 0 Then

                'idMovDet = agroDP.NuovoId_Tabella_EF(GiasContext, "MOVIMENTI_DETTAGLI", 0, 200000000, objParametriServer)
                idMovDet = agroDP.NuovoId_Tabella("MOVIMENTI_DETTAGLI", 0, 200000000, objParametriServer)
                Movimenti_dettagli.Id_Mov_Det = idMovDet

            Else

                idMovDet = Movimenti_dettagli.Id_Mov_Det

                Dim movimentiCount = From a In GiasContext.Movimenti_dettagli
                                     Where a.Id_Agenda = Movimenti_dettagli.Id_Agenda AndAlso
                                           a.Id_Mov = Movimenti_dettagli.Id_Mov AndAlso
                                           a.Id_Mov_Det = idMovDet AndAlso
                                           a.PIVA = Movimenti_dettagli.PIVA AndAlso
                                           a.Sa_Cod = Movimenti_dettagli.Sa_Cod
                                     Select a
                If movimentiCount.Count > 0 Then
                    esiste = True
                Else
                    'Ho mantenuto ID_Agenda, ID_Mov e Id_Mov_Det ma è cambiata la chiave: PIVA o Sa_Cod
                    cambioChiave = True
                End If

            End If

            If cambioChiave Then
                Dim movimentiDel = From a In GiasContext.Movimenti_dettagli
                                   Where a.Id_Agenda = Movimenti_dettagli.Id_Agenda AndAlso
                                         a.Id_Mov = Movimenti_dettagli.Id_Mov AndAlso
                                         a.Id_Mov_Det = idMovDet
                                   Select a

                For Each mov In movimentiDel
                    GiasContext.Movimenti_dettagli.Remove(mov)
                    GiasContext.SaveChanges()
                Next

            End If

            Dim movimentiW As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
            If esiste Then
                movimentiW.Modifica(Movimenti_dettagli, GiasContext, objParametriServer)
            Else
                movimentiW.Scrivi(Movimenti_dettagli, GiasContext, objParametriServer)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return idMovDet

    End Function

    Public Sub Elimina(ByRef Movimenti_dettagli As AgronicaCoreEntityFramework_POCO.Movimenti_dettagli(),
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabBIZ.Movimenti_Dettagli_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try

            Dim movimentiW As New AgronicaCoreContabDAL.Movimenti_Dettagli_W
            For Each Movimento In Movimenti_dettagli
                movimentiW.Elimina(Movimento, GiasContext, objParametriServer)
            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Sub

End Class