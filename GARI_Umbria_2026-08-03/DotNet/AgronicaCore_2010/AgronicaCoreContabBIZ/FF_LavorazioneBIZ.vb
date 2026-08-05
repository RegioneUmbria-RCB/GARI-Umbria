Imports System.Text
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class FF_AgendaCollegBIZ

    ''' <value>
    ''' IdAgenda collegata
    ''' </value>
    Public Property IdAgendaColleg As Integer

    ''' <value>
    ''' LavCod agenda collegata
    ''' </value>
    Public Property LavCodColleg As String

    ''' <value>
    ''' IdAgenda lavorazione origine
    ''' </value>
    Public Property IdAgendaLavOrig As Integer

    ''' <value>
    ''' IdAgenda ordine lavorazione che ha generato la lavorazione di origine
    ''' </value>
    Public Property IdAgendaOrdGenLavOrig As Integer

    ''' <value>
    ''' IdAgenda ordine lavorazione collegato a ordine lavorazione che ha generato la lavorazione origine
    ''' </value>
    Public Property IdAgendaOrdCollegOrdGenLavOrig As Integer

    Public Property MessaggioErrore As String

End Class

Public Class FF_LavorazioneBIZ
    Inherits AgronicaCoreDataProvider.LogProvider

    '--------------------------------------------------------------------------------
    'Costanti parametri qualitativi
    '--------------------------------------------------------------------------------
    Private Const FF_ As String = "FF_"
    Private Const Tipo As String = "Tipo"
    Private Const NumDecimali_Maximo As String = "NumDecimali_Maximo"
    Private Const Tabella_Key As String = "Tabella_Key"
    Private Const Tabella_Des As String = "Tabella_Des"
    Private Const _Cod As String = "_Cod"
    Private Const _Sigla As String = "_Sigla"
    Private Const _Tara_Campionatura As String = "_Tara_Campionatura"
    Private Const pq_imballaggio As String = "imballaggio"
    Private Const pq_contenitore As String = "contenitore"
    Private Const pq_confezione As String = "confezione"
    Private Const pq_fornitore As String = "fornitore"
    Private Const pq_cliente As String = "cliente"

    ''' <summary>
    ''' Ricerca la lavorazione collegata ad un'altra lavorazione.<br/><br/>
    ''' La ricerca avviene attraverso i seguenti passi:<br/>
    ''' <list type="number">  
    ''' <item>
    '''     <description>
    '''     Ricerca lavorazione direttamente collegata a lavorazione origine passata in ingresso (<paramref name="IdAgenda"/>);
    '''     </description>  
    ''' </item>  
    ''' <item>
    '''     <description>
    '''     Se lavorazione collegata non trovata, ricerca ordine lavorazione che ha generato lavorazione origine;
    '''     </description>  
    ''' </item>  
    ''' <item>
    '''     <description>
    '''     Se ordine lavorazione trovato, ricerca ordine lavorazione collegato.
    '''     </description>  
    ''' </item>  
    ''' </list>  
    ''' </summary>
    ''' <param name="Piva">Piva lavorazione origine</param>
    ''' <param name="IdAgenda">IdAgenda lavorazione origine</param>
    ''' <param name="objParametriServer">Oggetto parametri connessione</param>
    ''' <returns>
    ''' Oggetto FF_AgendaBIZ che contiene i dati della lavorazione/ordine collegato
    ''' </returns>

    Public Function Ricerca_LavOrd_Colleg(ByVal Piva As String,
                                          ByVal IdAgenda As Integer,
                                          ByRef objParametriServer As AgronicaCoreParametri) As FF_AgendaCollegBIZ

        'Non è stato gestito in ingresso il LavCod.
        'Se serve implementare nuova funzione Ricerca_Ord_Colleg per ricercare un ordine lavorazione collegato ad un altro ordine lavorazione.

        Dim r As New FF_AgendaCollegBIZ

        r.IdAgendaLavOrig = IdAgenda

        'Filtri per considerare solo lavorazioni/ordini aperti
        Dim filtriAggLavOrdAperti = OttieniFiltroLavOrdAperti()
        Dim filtriAggLavOrdApertiRif = OttieniFiltroLavOrdApertiRif()

        'Lettura lavorazione collegata
        Dim objMovDetRif As New Mov_Dettagli_Riferimenti_R
        Dim dtMovDetRif As DataTable = objMovDetRif.LeggiPerAgendaRif(Piva, 0, r.IdAgendaLavOrig, LAVCOD_TRASFORMAZIONI, -1,
                                                                      filtriAggLavOrdAperti, "", objParametriServer,
                                                                      Lav_Cod:=LAVCOD_TRASFORMAZIONI, Cau_Mov:=-1)

        Select Case dtMovDetRif.Rows.Count

            Case 1
                r.IdAgendaColleg = dtMovDetRif.Rows(0).Item("id_agenda")
                r.LavCodColleg = LAVCOD_TRASFORMAZIONI

            Case > 1
                r.MessaggioErrore = "Lavorazione collegata a più lavorazioni (Lavorazione: {0})"
                r.MessaggioErrore = String.Format(r.MessaggioErrore, r.IdAgendaLavOrig)

            Case 0
                'Lettura ordine che ha generato la lavorazione
                Dim dtMDRifLavOrd As DataTable = objMovDetRif.LeggiPerAgenda(Piva, 0, r.IdAgendaLavOrig, LAVCOD_TRASFORMAZIONI, -1,
                                                                             filtriAggLavOrdApertiRif, "", objParametriServer,
                                                                             leggiRiferimentiInversi:=False,
                                                                             LAVCOD_TESTATE_ORDINE_LAVORAZIONE, -1)
                Select Case dtMDRifLavOrd.Rows.Count

                    Case 1
                        r.IdAgendaOrdGenLavOrig = dtMDRifLavOrd.Rows(0).Item("id_agenda_rif")
                        'Lettura ordine collegato a ordine che ha generato la lavorazione
                        Dim dtMDRifOrdOrd As DataTable = objMovDetRif.LeggiPerAgendaRif(Piva, 0, r.IdAgendaOrdGenLavOrig,
                                                                                        LAVCOD_TESTATE_ORDINE_LAVORAZIONE, -1,
                                                                                        filtriAggLavOrdAperti, "", objParametriServer,
                                                                                        Lav_Cod:=LAVCOD_TESTATE_ORDINE_LAVORAZIONE, Cau_Mov:=-1)
                        Select Case dtMDRifOrdOrd.Rows.Count

                            Case 1
                                r.IdAgendaOrdCollegOrdGenLavOrig = dtMDRifOrdOrd.Rows(0).Item("id_agenda")
                                r.IdAgendaColleg = r.IdAgendaOrdCollegOrdGenLavOrig
                                r.LavCodColleg = LAVCOD_TESTATE_ORDINE_LAVORAZIONE

                            Case 0
                                r.MessaggioErrore = "Lavorazione collegata a ordine; ordine non collegato ad altro ordine (Lavorazione: {0} - Ordine: {1})"
                                r.MessaggioErrore = String.Format(r.MessaggioErrore, r.IdAgendaLavOrig, r.IdAgendaOrdGenLavOrig)

                            Case > 1
                                r.MessaggioErrore = "Lavorazione collegata a ordine; ordine collegato a più ordini (Lavorazione: {0} - Ordine: {1})"
                                r.MessaggioErrore = String.Format(r.MessaggioErrore, r.IdAgendaLavOrig, r.IdAgendaOrdGenLavOrig)

                        End Select

                    Case > 1
                        r.MessaggioErrore = "Lavorazione con più ordini collegati (Lavorazione: {0})"
                        r.MessaggioErrore = String.Format(r.MessaggioErrore, r.IdAgendaLavOrig)

                    Case 0
                        r.MessaggioErrore = "Lavorazione collegata non trovata (Lavorazione: {0})"
                        r.MessaggioErrore = String.Format(r.MessaggioErrore, r.IdAgendaLavOrig)

                End Select

        End Select

        Return r

    End Function

    Private Function OttieniFiltroLavOrdAperti() As String

        Dim filtriAggLavOrdAperti As String

        filtriAggLavOrdAperti = " MOV_DETTAGLI_RIFERIMENTI.ID_AGENDA IN ( "
        filtriAggLavOrdAperti += "SELECT MOV2.ID_AGENDA FROM MOVIMENTI MOV2 "
        filtriAggLavOrdAperti += "INNER JOIN AGENDA AGE2 ON AGE2.ID_AGENDA = MOV2.ID_AGENDA "
        filtriAggLavOrdAperti += "WHERE MOV2.ID_AGENDA = MOV_DETTAGLI_RIFERIMENTI.ID_AGENDA AND "
        filtriAggLavOrdAperti += "MOV2.CAU_MOV = {0} AND MOV2.EXTRA_INT = 0"
        filtriAggLavOrdAperti += ") "

        filtriAggLavOrdAperti = String.Format(filtriAggLavOrdAperti, CAU_LINEA_PRODUZIONE)

        Return filtriAggLavOrdAperti

    End Function

    Private Function OttieniFiltroLavOrdApertiRif() As String

        Dim filtriAggLavOrdApertiRif As String

        filtriAggLavOrdApertiRif = " MOV_DETTAGLI_RIFERIMENTI.ID_AGENDA_RIF IN ( "
        filtriAggLavOrdApertiRif += "SELECT MOV2.ID_AGENDA FROM MOVIMENTI MOV2 "
        filtriAggLavOrdApertiRif += "INNER JOIN AGENDA AGE2 ON AGE2.ID_AGENDA = MOV2.ID_AGENDA "
        filtriAggLavOrdApertiRif += "WHERE MOV2.ID_AGENDA = MOV_DETTAGLI_RIFERIMENTI.ID_AGENDA_RIF AND "
        filtriAggLavOrdApertiRif += "MOV2.CAU_MOV = {0} AND MOV2.EXTRA_INT = 0"
        filtriAggLavOrdApertiRif += ") "
        filtriAggLavOrdApertiRif = String.Format(filtriAggLavOrdApertiRif, CAU_LINEA_PRODUZIONE)

        Return filtriAggLavOrdApertiRif

    End Function

    ''' <summary>
    ''' Ricerca l'ordine lavorazione che ha generato la lavorazione.<br/><br/>
    ''' </summary>
    ''' <param name="Piva">Piva lavorazione origine</param>
    ''' <param name="IdAgenda">IdAgenda lavorazione origine</param>
    ''' <param name="objParametriServer">Oggetto parametri connessione</param>
    ''' <returns>
    ''' IdAgenda ordine lavorazione che ha generato la lavorazione
    ''' </returns>

    Public Function Ricerca_Ord_Generatore(ByVal Piva As String,
                                           ByVal IdAgenda As Integer,
                                           ByRef objParametriServer As AgronicaCoreParametri) As Integer

        Dim idAgendaOrd As Integer

        Dim objMovDetRif As New Mov_Dettagli_Riferimenti_R

        Dim filtriAggLavOrdApertiRif = OttieniFiltroLavOrdApertiRif()

        Dim dtMDRifLavOrd As DataTable = objMovDetRif.LeggiPerAgenda(Piva,
                                                                     0,
                                                                     IdAgenda,
                                                                     LAVCOD_TRASFORMAZIONI,
                                                                     -1,
                                                                     filtriAggLavOrdApertiRif,
                                                                     "",
                                                                     objParametriServer,
                                                                     leggiRiferimentiInversi:=False,
                                                                     LAVCOD_TESTATE_ORDINE_LAVORAZIONE,
                                                                     -1)

        If dtMDRifLavOrd.Rows.Count > 0 Then
            idAgendaOrd = dtMDRifLavOrd.Rows(0).Item("id_agenda_rif")
        End If

        Return idAgendaOrd

    End Function

    Public Function Apertura_Nuova_Lavorazione(ByVal Piva As String,
                                               ByVal Sa_Cod As Integer,
                                               ByVal Lav_Cod As Integer,
                                               ByVal Linea_Cod As Integer,
                                               ByVal Preparazione_Cod As Integer,
                                               ByVal des_lib As String,
                                               ByVal Validita_Inizio As Date,
                                               ByVal Validita_Fine As Date,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As Boolean


        'ByVal Elem_Cod As Integer,
        'ByVal Pro_Cod As Integer,
        'ByVal Mat_Cod As Integer,
        'ByVal Udm_Cod As Integer,
        'ByVal Id_Destinazione As Integer,
        'ByVal Operazione As String,
        'ByVal Cau_Mov As String,
        'ByVal Cal_Cod As Integer,
        'ByVal Cod_Progetto As Integer,
        'ByVal Fase_Cod As Integer,
        'ByVal Lotto As String,
        'ByVal Qta As Decimal,
        'ByVal Prezzo_Unitario As Decimal,
        'ByVal Udm_Cod_Extra As Integer,
        'ByVal Qta_Extra As Decimal,
        'ByVal Qta_Extra_Totale As Decimal,
        'ByVal Variazione As Decimal,
        'ByVal Jolly_Int As Integer,
        'ByVal BaseCode As Integer,
        'ByVal TopCode As Integer,

        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.Apertura_Nuova_Lavorazione()"
        Dim messaggioErrore As String = ""

        Dim xmlDoc As New System.Xml.XmlDocument

        '---------------------------
        'nodo AGENDA
        Dim LogErrori As String
        Dim objXmlContab As New AgronicaCoreXML.XML_Contab

        Dim xmlDatiAgenda As System.Xml.XmlElement
        Dim dtAgenda As New DataTable
        xmlDatiAgenda = objXmlContab.XML_Agenda_Agenda__DATI(LogErrori, xmlDoc, dtAgenda)

        dtAgenda = objXmlContab.DtForXml_Genera_Agenda

        objXmlContab.DtForXml_InserisciRiga_Agenda(dtAgenda,
                                                   enum_TipoOperazioneDB.Scrittura,
                                                   Piva, Sa_Cod,
                                                   ,
                                                   LAVCOD_TRASFORMAZIONI, des_lib,
                                                   , Linea_Cod, Preparazione_Cod, , Validita_Inizio, Validita_Fine, , ,
                                                   , , , , , , , , , )

        Dim xmlAgenda As System.Xml.XmlElement
        xmlAgenda = objXmlContab.XML_Agenda_Agenda(LogErrori,
                                                   xmlDoc,
                                                   dtAgenda.Rows(0))

        xmlDatiAgenda.AppendChild(xmlAgenda)

        '---------------------------
        'nodo MOVIMENTO
        Dim xmlDatiMovimenti As System.Xml.XmlElement
        Dim dtMovimenti As New DataTable
        xmlDatiMovimenti = objXmlContab.XML_Agenda_Movimenti__DATI(LogErrori, xmlDoc, dtMovimenti)

        xmlAgenda.AppendChild(xmlDatiMovimenti)


        dtMovimenti = objXmlContab.DtForXml_Genera_Movimenti

        objXmlContab.DtForXml_InserisciRiga_Movimenti(dtMovimenti,
                                                      enum_TipoOperazioneDB.Scrittura,
                                                      Piva,
                                                      , , ,
                                                      CAU_CARICO,
                                                      , , , , , , , , , , , , , , , , , , , , , , , , , , , , , ,
                                                      AGRODATAINIZIO, AGRODATAFINE,
                                                      , )

        Dim xmlMovimenti As System.Xml.XmlElement
        xmlMovimenti = objXmlContab.XML_Agenda_Movimenti(LogErrori,
                                                         xmlDoc,
                                                         dtMovimenti.Rows(0))


        xmlDatiMovimenti.AppendChild(xmlMovimenti)

        '---------------------------
        'nodo DETTAGLIO

        Dim xmlDatiDettagli As System.Xml.XmlElement
        Dim dtDettagli As New DataTable
        xmlDatiDettagli = objXmlContab.XML_Agenda_MovimentiDettagli__DATI(LogErrori, xmlDoc, dtDettagli)

        xmlMovimenti.AppendChild(xmlDatiDettagli)


        dtDettagli = objXmlContab.DtForXml_Genera_MovimentiDettagli

        'objXMLContab.DtForXml_InserisciRiga_MovimentiDettagli(dtDettagli,
        '                                               enum_TipoOperazioneDB.Scrittura,
        '                                               Piva,
        '                                               , , , , ,
        '                                               MACCHINE, ,
        '                                               Mac_Cod, , , , ,
        '                                               38,
        '                                               ,
        '                                               1,
        '                                               , , , , , , , , , , , , ,
        '                                               NONCONTABILE,
        '                                               enum_Pendenza.GiacenzeIniziali,
        '                                               , , ,
        '                                               AGRODATAINIZIO, AGRODATAFINE,
        '                                               , , , , , , ,
        '                                               BaseCode, TopCode)

        Dim xmlDettagli As System.Xml.XmlElement
        xmlDettagli = objXmlContab.XML_Agenda_MovimentiDettagli(LogErrori,
                                                                xmlDoc,
                                                                dtDettagli.Rows(0))

        xmlDatiDettagli.AppendChild(xmlDettagli)

        '-------------------------------------------
        'Rendo l'albero figlio del documento
        xmlDoc.AppendChild(xmlDatiAgenda)

        'Estraggo la stringa XML complessiva
        Dim strXmlLavor As String = xmlDoc.InnerXml


        '------------------------------------------------
        '----- Scrivo nel DB
        '------------------------------------------------

        Dim objAgendaW As New AgronicaCoreContabBIZ.Agenda_W
        Dim OUTPUT_ID_Agenda As Integer
        objAgendaW.Agenda_Scrivi(strXmlLavor,
                                 OUTPUT_ID_Agenda,
                                 0,
                                 Id_Servizio_GiasOnline,
                                 0, "",
                                 objParametri)

    End Function

    '============================================================================
    Public Function Leggi_Lavorazioni(ByVal Piva As String,
                                      ByVal Id_Agenda As Integer,
                                      ByVal Id_Mov As Integer,
                                      ByVal Cau_Mov As String,
                                      ByVal Des_Lib_Agenda As String,
                                      ByVal Validita_Inizio As String,
                                      ByVal Validita_Fine As String,
                                      ByVal AperteChiuseTutte As String,
                                      ByVal CheckCarichiScarichi As Boolean,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional LavCod As Integer = LAVCOD_TRASFORMAZIONI) As String

        Dim risposta As String = ""
        Dim dtDati = New DataTable

        Const nomeRoutine = "FF_LavorazioneBIZ.Leggi_Lavorazioni()"

        Dim messaggioErrore As String = ""

        Dim i As Integer

        Dim objMovimenti As AgronicaCoreContabDAL.Movimenti_R
        Dim objMovimentiDettagli As AgronicaCoreContabDAL.Movimenti_Dettagli_R

        Dim dtMovimenti As DataTable
        Dim dtMovimentiScarico As DataTable
        Dim dtMovimentiCarico As DataTable
        Dim dtMovimentiDettagli As DataTable
        Dim dataRicercaDal As Date
        Dim dataRicercaAl As Date

        If Validita_Inizio = "" Then
            dataRicercaDal = AGRODATAINIZIO
        Else
            dataRicercaDal = CDate(Validita_Inizio)
        End If

        If Validita_Fine = "" Then
            dataRicercaAl = AGRODATAFINE
        Else
            dataRicercaAl = CDate(Validita_Fine)
        End If

        Dim xFiltroAggiuntivo As String = ""
        xFiltroAggiuntivo = xFiltroAggiuntivo & "  Movimenti.Data_Movimento >= '" & dataRicercaDal & "' "
        xFiltroAggiuntivo = xFiltroAggiuntivo & " And Movimenti.Data_Movimento <= '" & dataRicercaAl & "' "
        xFiltroAggiuntivo = xFiltroAggiuntivo & " And Agenda.Lav_Cod = " & LavCod.ToString() & " "

        If Not String.IsNullOrEmpty(Des_Lib_Agenda) Then
            xFiltroAggiuntivo = xFiltroAggiuntivo & " And Agenda.Des_Lib like '%" & Des_Lib_Agenda & "%'"
        End If

        If AperteChiuseTutte = "A" Then
            xFiltroAggiuntivo &= " And Movimenti.Extra_Int = 0 "
        Else
            If AperteChiuseTutte = "C" Then
                xFiltroAggiuntivo &= " And Movimenti.Extra_Int = 1 "
            End If
        End If

        Dim xOrderBy As String = " Movimenti.Data_Movimento DESC, Agenda.Des_Lib "

        Dim DTParamQual As New DataTable

        Try

            Dim objConfigDettagli As AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R = New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
            DTParamQual = objConfigDettagli.Leggi(Piva, 0, False, "Tipo = 1", "", objParametri)

            objMovimenti = New AgronicaCoreContabDAL.Movimenti_R
            dtMovimenti = objMovimenti.Leggi(Piva,
                                             0,
                                             Id_Agenda,
                                             0,
                                             0,
                                             CAU_LINEA_PRODUZIONE,
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             xFiltroAggiuntivo,
                                             xOrderBy,
                                             objParametri)

            If dtMovimenti.Rows.Count > 0 Then

                'Leggo i tipi di preparazione di Unificazione Lotti perché poi li dovrò scartare.
                'Lo faccio solo se non sto leggendo una specifica lavorazione perché in quel caso
                'la voglio selezionare sicuramente.
                Dim obj_Preparazione As New Linee_Preparazioni_R
                Dim DT_Preparazione As New DataTable
                If Id_Agenda = 0 Then
                    DT_Preparazione = obj_Preparazione.Leggi(Piva,
                                                             0,
                                                             enum_Omni_Modulo_Generazione.FreshFood,
                                                             enum_PreparazioneCod.UnificazioneLotto,
                                                             "",
                                                             "",
                                                             objParametri)
                End If

                dtDati.Columns.Add(New DataColumn("Piva", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Sa_Cod", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Id_Mov", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Data_Movimento", GetType(Date)))
                dtDati.Columns.Add(New DataColumn("Ora_Movimento", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Lotto", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Prodotto", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Cat_Cod", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Cul_Cod", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Mat_Cod", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Des_Lib", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Descr_Aggiuntiva", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Extra_Int", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Chiusa", GetType(String)))
                dtDati.Columns.Add(New DataColumn("KgScaricati", GetType(Decimal)))
                dtDati.Columns.Add(New DataColumn("KgCaricati", GetType(Decimal)))
                dtDati.Columns.Add(New DataColumn("KgDifferenza", GetType(Decimal)))
                dtDati.Columns.Add(New DataColumn("NrImballaggiAperturaLav", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("NrContenitoriAperturaLav", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("NrConfezioniAperturaLav", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("IdDestinazioneAperturaLav", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Cod_Macchina_Lav", GetType(String)))
                dtDati.Columns.Add(New DataColumn("Id_Agenda_Collegata", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Id_Ordine_Lavoro", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Num_Lav_Collegate", GetType(Integer)))

                For Each paramQual In DTParamQual.Rows

                    ColumnsAddParamQual(dtDati, paramQual)

                Next

                dtDati.Columns.Add(New DataColumn("Sa_Cod_AperturaLav", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Id_Mov_AperturaLav", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Id_Mov_Det_AperturaLav", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Cal_Cod_AperturaLav", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("DataOraUltimaLettura", GetType(Date)))
                dtDati.Columns.Add(New DataColumn("Preparazione_Cod_AperturaLav", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Linea_Cod_AperturaLav", GetType(Integer)))
                dtDati.Columns.Add(New DataColumn("Tipo_Destinazione_AperturaLav", GetType(Integer)))

                Dim kgScaricati As Decimal = 0
                Dim kgCaricati As Decimal = 0
                Dim kgDifferenza As Decimal = 0
                Dim IdMovScarico As Integer = 0
                Dim IdMovCarico As Integer = 0

                'Effettuo un ciclo sui movimenti
                For i = 0 To dtMovimenti.Rows.Count - 1

                    Dim lavUnifLotti = False
                    If DT_Preparazione.Rows.Count > 0 Then
                        For Each drPrep In DT_Preparazione.Rows
                            If CInt(drPrep("Preparazione_Cod")) = CInt(dtMovimenti.Rows(i).Item("Preparazione_Cod")) Then
                                lavUnifLotti = True
                                Exit For
                            End If
                        Next
                    End If

                    If Not lavUnifLotti Then

                        Dim dr As DataRow

                        'Creo una nuova riga
                        dr = dtDati.NewRow
                        dr.Item("Piva") = dtMovimenti.Rows(i).Item("Piva")
                        dr.Item("Sa_Cod") = dtMovimenti.Rows(i).Item("Sa_Cod")
                        dr.Item("Id_Agenda") = dtMovimenti.Rows(i).Item("Id_Agenda")
                        dr.Item("Id_Mov") = dtMovimenti.Rows(i).Item("Id_Mov")
                        dr.Item("Data_Movimento") = dtMovimenti.Rows(i).Item("Data_Movimento")
                        If dtMovimenti.Rows(i).Item("Ora") IsNot DBNull.Value Then

                            dr.Item("Ora_Movimento") = CDate(dtMovimenti.Rows(i).Item("Ora")).ToString("HH:mm")
                        Else
                            dr.Item("Ora_Movimento") = "00:00"
                        End If

                        'Descrizione lavorazione
                        dr.Item("Des_Lib") = dtMovimenti.Rows(i).Item("Des_Lib")
                        Des_Lib_Agenda = dtMovimenti.Rows(i).Item("Des_Lib")

                        'Stato lavorazione
                        dr.Item("Extra_Int") = dtMovimenti.Rows(i).Item("Extra_Int")

                        'Descrizione stato lavorazione: 0 aperta - 1 chiusa
                        dr.Item("Chiusa") = If(dtMovimenti.Rows(i).Item("Extra_Int") = 1, "Sì", "No")

                        'Macchina lavorazione
                        dr.Item("Cod_Macchina_Lav") = dtMovimenti.Rows(i).Item("Cod_Macchina_Lav")

                        'Lettura lavorazione/ordine collegato

                        Dim idAgenda = CInt(dtMovimenti.Rows(i).Item("Id_Agenda"))

                        dr.Item("Id_Agenda_Collegata") = LeggiAgendaCollegata(Piva, idAgenda, LavCod, objParametri)

                        dr.Item("Id_Ordine_Lavoro") = SeLeggiOrdineLavoro(Piva, idAgenda, LavCod, objParametri)

                        dr.Item("Num_Lav_Collegate") = SeLeggiLavCollegate(Piva, idAgenda, LavCod, objParametri)

                        If CheckCarichiScarichi Then

                            'Leggo i movimenti di carico e scarico per controllare il totale dei kg

                            dtMovimentiScarico = objMovimenti.Leggi(Piva, 0,
                                                                    CInt(dtMovimenti.Rows(i).Item("Id_Agenda")),
                                                                    0, 0, CAU_SCARICO,
                                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                    "", xOrderBy,
                                                                    objParametri)
                            If dtMovimentiScarico.Rows.Count > 0 Then
                                IdMovScarico = CInt(dtMovimentiScarico.Rows(0).Item("Id_Mov"))
                            End If

                            dtMovimentiCarico = objMovimenti.Leggi(Piva, 0,
                                                                   CInt(dtMovimenti.Rows(i).Item("Id_Agenda")),
                                                                   0, 0, CAU_CARICO,
                                                                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                   "", xOrderBy,
                                                                   objParametri)
                            If dtMovimentiCarico.Rows.Count > 0 Then
                                IdMovCarico = CInt(dtMovimentiCarico.Rows(0).Item("Id_Mov"))
                            End If

                            xFiltroAggiuntivo = " Jolly_Int = 0 "

                            If IdMovScarico <> 0 Then
                                objMovimentiDettagli = New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                                dtMovimentiDettagli = objMovimentiDettagli.Leggi(Piva,
                                                                                 0,
                                                                                 Id_Agenda,
                                                                                 IdMovScarico,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                 xFiltroAggiuntivo,
                                                                                 "",
                                                                                 objParametri)

                                kgScaricati = 0
                                If dtMovimentiDettagli.Rows.Count > 0 Then
                                    For j = 0 To dtMovimentiDettagli.Rows.Count - 1
                                        kgScaricati = kgScaricati + (dtMovimentiDettagli.Rows(j).Item("Qta_Extra") * dtMovimentiDettagli.Rows(j).Item("Qta"))
                                    Next
                                End If
                            End If

                            If IdMovCarico <> 0 Then
                                objMovimentiDettagli = New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                                dtMovimentiDettagli = objMovimentiDettagli.Leggi(Piva,
                                                                                 0,
                                                                                 Id_Agenda,
                                                                                 IdMovCarico,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 0,
                                                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                 xFiltroAggiuntivo,
                                                                                 "",
                                                                                 objParametri)

                                kgCaricati = 0
                                If dtMovimentiDettagli.Rows.Count > 0 Then
                                    For j = 0 To dtMovimentiDettagli.Rows.Count - 1
                                        kgCaricati = kgCaricati + (dtMovimentiDettagli.Rows(j).Item("Qta_Extra") * dtMovimentiDettagli.Rows(j).Item("Qta"))
                                    Next
                                End If
                            End If

                        End If

                        dr.Item("KgScaricati") = kgScaricati
                        dr.Item("KgCaricati") = kgCaricati
                        dr.Item("KgDifferenza") = kgCaricati - kgScaricati

                        'Se sto leggendo una singola lavorazione cerco il record di default con cui è stata creata

                        'Cerco lotto e prodotto del movimento di creazione lavorazione
                        dtMovimentiCarico = objMovimenti.Leggi(Piva,
                                                               0,
                                                               CInt(dtMovimenti.Rows(i).Item("Id_Agenda")),
                                                               0,
                                                               0,
                                                               CAU_CARICO,
                                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                               "",
                                                               xOrderBy,
                                                               objParametri)

                        If dtMovimentiCarico IsNot Nothing AndAlso dtMovimentiCarico.Rows.Count > 0 Then
                            IdMovCarico = CInt(dtMovimentiCarico.Rows(0).Item("Id_Mov"))
                        End If

                        'N.B. record con jolly_int = 1
                        If IdMovCarico <> 0 Then
                            objMovimentiDettagli = New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                            Dim filtroAggiuntivo = " AND Movimenti_dettagli.Extra_Str <> '' AND Movimenti_dettagli.Extra_Int > 0 "
                            dtMovimentiDettagli = objMovimentiDettagli.MovimentiDettagli_Leggi_FF(
                                        Piva,
                                        0,
                                        "",
                                        CInt(dtMovimenti.Rows(i).Item("Id_Agenda")),
                                        IdMovCarico,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0,
                                        LOTTO_NONDEFINITO,
                                        0,
                                        0,
                                        Nothing,
                                        Nothing,
                                        0,
                                        1,
                                        Nothing,
                                        Nothing,
                                        filtroAggiuntivo,
                                        Nothing,
                                        objParametri)

                            ' NB  Per il record fittizio jolly_int = 1 l'id destinazione è nel campo Extra_Int1 (ovvero movimenti_dettagli.extra_int)
                            If dtMovimentiDettagli.Rows.Count > 0 Then

                                For Each paramQual In DTParamQual.Rows

                                    CaricaParamQualLavorazioni(paramQual,
                                                               dtMovimentiDettagli.Rows(0),
                                                               dr,
                                                               objParametri)

                                Next

                                dr.Item("Sa_Cod_AperturaLav") = dtMovimentiDettagli.Rows(0).Item("Sa_Cod2")
                                dr.Item("Id_Mov_AperturaLav") = dtMovimentiDettagli.Rows(0).Item("Id_Mov1")
                                dr.Item("Id_Mov_Det_AperturaLav") = dtMovimentiDettagli.Rows(0).Item("Id_Mov_Det")
                                dr.Item("Cal_Cod_AperturaLav") = dtMovimentiDettagli.Rows(0).Item("Cal_Cod")
                                dr.Item("DataOraUltimaLettura") = DateTime.Now
                                dr.Item("Preparazione_Cod_AperturaLav") = dtMovimentiDettagli.Rows(0).Item("PREPARAZIONE_COD")
                                dr.Item("Linea_Cod_AperturaLav") = dtMovimentiDettagli.Rows(0).Item("LINEA_COD")
                                dr.Item("Tipo_Destinazione_AperturaLav") = dtMovimentiDettagli.Rows(0).Item("Extra_Str1")

                                dr.Item("Lotto") = dtMovimentiDettagli.Rows(0).Item("Lotto")
                                dr.Item("Cat_Cod") = dtMovimentiDettagli.Rows(0).Item("Elem_Cod")
                                dr.Item("Veg_Cod") = dtMovimentiDettagli.Rows(0).Item("Veg_Cod")
                                dr.Item("Cul_Cod") = dtMovimentiDettagli.Rows(0).Item("Cul_Cod")
                                dr.Item("Prodotto") = dtMovimentiDettagli.Rows(0).Item("Mat_Des")
                                dr.Item("Mat_Cod") = dtMovimentiDettagli.Rows(0).Item("Mat_Cod")
                                dr.Item("Udm_Cod") = dtMovimentiDettagli.Rows(0).Item("Udm_Cod")
                                dr.Item("IdDestinazioneAperturaLav") = dtMovimentiDettagli.Rows(0).Item("Extra_Str1") & "_" & CStr(dtMovimentiDettagli.Rows(0).Item("Sa_Cod2")) & "_" & CStr(dtMovimentiDettagli.Rows(0).Item("Extra_Int1"))
                                dr.Item("NrImballaggiAperturaLav") = dtMovimentiDettagli.Rows(0).Item("Qta_Dettaglio2")
                                dr.Item("NrContenitoriAperturaLav") = dtMovimentiDettagli.Rows(0).Item("Qta_Dettaglio1")
                                dr.Item("NrConfezioniAperturaLav") = 0
                                If CInt(dtMovimentiDettagli.Rows(0).Item("Udm_Cod")) = enum_UnitaMisura.Numero Then
                                    If dtMovimentiDettagli.Rows(0).Item("Qta_Dettaglio1") <> 0 Then
                                        dr.Item("NrConfezioniAperturaLav") = dtMovimentiDettagli.Rows(0).Item("Qta") / dtMovimentiDettagli.Rows(0).Item("Qta_Dettaglio1")
                                        ' 21/11/2017 - per ora non gestiamo il caso in cui ci sono imballi e confezioni senza contenitore
                                        'Else
                                        '    If dtMovimentiDettagli.Rows(0).Item("Qta_Dettaglio2") <> 0 Then
                                        '        dr.Item("NrConfezioniAperturaLav") = dtMovimentiDettagli.Rows(0).Item("Qta") / dtMovimentiDettagli.Rows(0).Item("Qta_Dettaglio2")
                                        '    End If
                                    End If
                                End If

                                'Descrizione aggiuntiva

                                'La carico solo a fronte di una singola lettura per non appesantire la ricerca

                                If Id_Agenda <> 0 Then
                                    ImpostaDescrizioneAggiuntiva(dr,
                                                                 Piva,
                                                                 dtMovimenti.Rows(i),
                                                                 objParametri)
                                End If

                            End If

                        End If

                        dtDati.Rows.Add(dr)

                    End If

                Next

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        ' VAnni: 15/2/2017: todo: verificare tutte le chiavi commentate (es: tariffa_cod per costi SBTF..)

        c = New ColonneNome("Piva", "Id_Agenda", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Id_Agenda", "Id_Agenda", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Data_Movimento", My.Resources.AgronicaCoreContabBIZ.FF_LavorazioneBIZ_Leggi_Lavorazioni_Data, "date")
        c._Filtrabile = True
        c._Display = True
        c._formatNr = "{0:dd/MM/yyyy}"
        l.Add(c)

        c = New ColonneNome("Ora_Movimento", My.Resources.AgronicaCoreContabBIZ.FF_LavorazioneBIZ_Leggi_Lavorazioni_Ora, "string")
        c._Filtrabile = False
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Des_Lib", My.Resources.AgronicaCoreContabBIZ.FF_LavorazioneBIZ_Leggi_Lavorazioni_Descrizione, "string")
        c._Filtrabile = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Descr_Aggiuntiva", "Descr_Aggiuntiva", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Sa_Cod_AperturaLav", "Sa_Cod_AperturaLav", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Id_Mov_AperturaLav", "Id_Mov_AperturaLav", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Id_Mov_Det_AperturaLav", "Id_Mov_Det_AperturaLav", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Cal_Cod_AperturaLav", "Cal_Cod_AperturaLav", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("DataOraUltimaLettura", "DataOraUltimaLettura", "date")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Preparazione_Cod_AperturaLav", "Preparazione_Cod_AperturaLav", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Linea_Cod_AperturaLav", "Linea_Cod_AperturaLav", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Tipo_Destinazione_AperturaLav", "Tipo_Destinazione_AperturaLav", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Lotto", My.Resources.AgronicaCoreContabBIZ.FF_LavorazioneBIZ_Leggi_Lavorazioni_Lotto, "string")
        c._Filtrabile = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Prodotto", My.Resources.AgronicaCoreContabBIZ.FF_LavorazioneBIZ_Leggi_Lavorazioni_Prodotto, "string")
        c._Filtrabile = True
        c._Display = True
        l.Add(c)

        c = New ColonneNome("Cat_Cod", "Cat_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Veg_Cod", "Veg_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Cul_Cod", "Cul_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Mat_Cod", "Mat_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Udm_Cod", "Udm_Cod", "number")
        c._hidden = True
        l.Add(c)

        For Each paramQual In DTParamQual.Rows

            AggiungiColonneParamQualLavorazioni(l, paramQual)

        Next

        c = New ColonneNome("NrImballaggiAperturaLav", "NrImballaggiAperturaLav", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("NrContenitoriAperturaLav", "NrContenitoriAperturaLav", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("NrConfezioniAperturaLav", "NrConfezioniAperturaLav", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("IdDestinazioneAperturaLav", "IdDestinazioneAperturaLav", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Cod_Macchina_Lav", "Cod_Macchina_Lav", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Id_Agenda_Collegata", "Id_Agenda_Collegata", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Id_Ordine_Lavoro", "Id_Ordine_Lavoro", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Num_Lav_Collegate", "Num_Lav_Collegate", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Extra_Int", "Extra_Int", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Chiusa", My.Resources.AgronicaCoreContabBIZ.FF_LavorazioneBIZ_Leggi_Lavorazioni_Chiusa, "string")
        c._Filtrabile = True
        c._Display = True
        l.Add(c)

        If CheckCarichiScarichi Then
            c = New ColonneNome("KgScaricati", My.Resources.AgronicaCoreContabBIZ.FF_LavorazioneBIZ_Leggi_Lavorazioni_KgIngresso, "number")
            c._Filtrabile = True
            c._FiltrabileConCheck = False
            c._Display = True
            c._formatNr = "n2"
            l.Add(c)

            c = New ColonneNome("KgCaricati", My.Resources.AgronicaCoreContabBIZ.FF_LavorazioneBIZ_Leggi_Lavorazioni_KgUscita, "number")
            c._Filtrabile = True
            c._FiltrabileConCheck = False
            c._Display = True
            c._formatNr = "n2"
            l.Add(c)

            c = New ColonneNome("KgDifferenza", My.Resources.AgronicaCoreContabBIZ.FF_LavorazioneBIZ_Leggi_Lavorazioni_KgDifferenza, "number")
            c._Filtrabile = True
            c._FiltrabileConCheck = False
            c._Display = True
            c._formatNr = "n2"
            l.Add(c)
        End If

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable With {
            .Editabile_Deafault = False
        }
        risposta = js.JSON_DataTable_Kendo(dtDati, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)

        Return risposta

    End Function

    Private Sub ImpostaDescrizioneAggiuntiva(
        ByRef dr As DataRow,
        ByVal piva As String,
        ByVal drMovimenti As DataRow,
        ByRef objParametri As AgronicaCoreParametri)

        Dim preparazioneDes = PreparazioneDesFromPreparazioneCod(
            piva,
            drMovimenti.Item("Preparazione_Cod"),
            objParametri)

        Dim descrizioneBase = CreaDesLibLavorazione(
            preparazioneDes,
            dr.Item("Prodotto"),
            dr.Item("Lotto"),
            "")

        Dim desLib = drMovimenti.Item("Des_Lib").ToString()

        dr.Item("Descr_Aggiuntiva") = desLib.Replace(descrizioneBase, "").Trim()

    End Sub

    Private Sub CaricaParamQualLavorazioni(
        ByVal paramQual As Object,
        ByRef drMovDettagli As DataRow,
        ByRef dr As DataRow,
        ByRef objParametri As AgronicaCoreParametri)

        Dim nomeColonnaCod = FF_ & paramQual(Tabella_Key) & _Cod
        Dim nomeColonnaSigla = FF_ & paramQual(Tabella_Key) & _Sigla
        Dim nomeColonnaTaraCamp = FF_ & paramQual(Tabella_Key) & _Tara_Campionatura

        Dim nomeColonnaCod_MovDett = paramQual(Tabella_Key) & _Cod
        Dim nomeColonnaSigla_MovDett = paramQual(Tabella_Key) & _Sigla
        Dim nomeColonnaTaraCamp_MovDett = paramQual(Tabella_Key) & _Tara_Campionatura

        Select Case paramQual(Tipo)

            Case enum_TipoParamQual.Numero, enum_TipoParamQual.Stringa, enum_TipoParamQual.Data

                Dim drMovDettagliCod = drMovDettagli.Item(nomeColonnaCod_MovDett)

                If paramQual(Tipo) = enum_TipoParamQual.Numero And Not IsNumeric(drMovDettagliCod) Then
                    drMovDettagliCod = "0"
                End If

                If paramQual(Tipo) = enum_TipoParamQual.Numero Then
                    dr.Item(nomeColonnaCod) = CDec(CStr(drMovDettagliCod).Replace(".", ","))
                Else
                    dr.Item(nomeColonnaCod) = drMovDettagliCod
                End If

            Case Else

                dr.Item(nomeColonnaCod) = drMovDettagli.Item(nomeColonnaCod_MovDett)

                If ({pq_imballaggio, pq_contenitore, pq_confezione}).Contains(paramQual(Tabella_Key)) Then
                    dr.Item(nomeColonnaTaraCamp) = drMovDettagli.Item(nomeColonnaTaraCamp_MovDett)
                End If

                If Not ({pq_cliente, pq_fornitore}).Contains(paramQual(Tabella_Key)) Then
                    dr.Item(nomeColonnaSigla) = drMovDettagli.Item(nomeColonnaSigla_MovDett)
                End If

                If ({pq_fornitore}).Contains(paramQual(Tabella_Key)) Then

                    Dim w_fornitore_cod = CStr(drMovDettagli.Item("fornitore_Cod"))

                    If Not String.IsNullOrEmpty(w_fornitore_cod) Then
                        Dim objContattiR = New AgronicaCoreAnagrafeDAL.Contatti_R
                        Dim objContatto As DataTable = objContattiR.RagSoc_Nome_Cognome_RapportoDes_from_Cod_Risum(w_fornitore_cod, objParametri)
                        If objContatto.Rows.Count > 0 Then
                            dr.Item("FF_Fornitore_Sigla") = objContatto.Rows(0)("rag_soc")
                        End If
                    End If

                End If

        End Select

    End Sub

    Private Sub ColumnsAddParamQual(
        ByRef dtDati As DataTable,
        ByVal paramQual As Object)

        Dim nomeColonnaCod = FF_ & paramQual(Tabella_Key) & _Cod
        Dim nomeColonnaSigla = FF_ & paramQual(Tabella_Key) & _Sigla
        Dim nomeColonnaTaraCamp = FF_ & paramQual(Tabella_Key) & _Tara_Campionatura

        Select Case paramQual(Tipo)

            Case enum_TipoParamQual.Numero
                dtDati.Columns.Add(New DataColumn(nomeColonnaCod, GetType(Decimal)))

            Case enum_TipoParamQual.Stringa, enum_TipoParamQual.Data
                dtDati.Columns.Add(New DataColumn(nomeColonnaCod, GetType(String)))

            Case Else
                dtDati.Columns.Add(New DataColumn(nomeColonnaCod, GetType(Integer)))
                If ({pq_imballaggio, pq_contenitore, pq_confezione}).Contains(paramQual(Tabella_Key)) Then
                    dtDati.Columns.Add(New DataColumn(nomeColonnaTaraCamp, GetType(Decimal)))
                End If
                If Not ({pq_cliente}).Contains(paramQual(Tabella_Key)) Then
                    dtDati.Columns.Add(New DataColumn(nomeColonnaSigla, GetType(String)))
                End If

        End Select

    End Sub

    Private Sub AggiungiColonneParamQualLavorazioni(
        ByRef l As List(Of ColonneNome),
        ByVal paramQual As Object)

        Dim nomeColonnaCod = FF_ & paramQual(Tabella_Key) & _Cod
        Dim nomeColonnaSigla = FF_ & paramQual(Tabella_Key) & _Sigla
        Dim nomeColonnaTaraCamp = FF_ & paramQual(Tabella_Key) & _Tara_Campionatura

        Dim c As ColonneNome

        Select Case paramQual(Tipo)

            Case enum_TipoParamQual.Numero, enum_TipoParamQual.Stringa, enum_TipoParamQual.Data

                AggiungiSingolaColonnaParamQualLavorazioni(nomeColonnaCod,
                                                           paramQual(Tabella_Des),
                                                           paramQual,
                                                           l)

            Case Else

                c = New ColonneNome(nomeColonnaCod, nomeColonnaCod, "number")
                c._hidden = True
                l.Add(c)

                Select Case paramQual(Tabella_Key)

                    Case pq_imballaggio, pq_contenitore, pq_confezione

                        c = New ColonneNome(nomeColonnaTaraCamp, nomeColonnaTaraCamp, "number")
                        c._hidden = True
                        l.Add(c)

                    Case pq_fornitore

                        AggiungiSingolaColonnaParamQualLavorazioni(nomeColonnaSigla,
                                                                   Gias.Fornitore,
                                                                   paramQual,
                                                                   l)

                    Case Is <> pq_cliente

                        AggiungiSingolaColonnaParamQualLavorazioni(nomeColonnaSigla,
                                                                   paramQual(Tabella_Des),
                                                                   paramQual,
                                                                   l)

                End Select

        End Select

    End Sub

    Private Sub AggiungiSingolaColonnaParamQualLavorazioni(
        ByVal nomeColonna As String,
        ByVal nomeColonnaVisualizzato As String,
        ByVal paramQual As Object,
        ByRef l As List(Of ColonneNome))

        Dim tipoDatoColonna As String

        Select Case paramQual(Tipo)

            Case enum_TipoParamQual.Numero
                tipoDatoColonna = "number"

            Case enum_TipoParamQual.Stringa
                tipoDatoColonna = "string"

            Case enum_TipoParamQual.Data
                tipoDatoColonna = "date"

            Case Else
                tipoDatoColonna = "string"

        End Select

        Dim c = New ColonneNome(nomeColonna, nomeColonnaVisualizzato, tipoDatoColonna)

        SetFormatNrParamQualNumerico(c, paramQual)

        c._Filtrabile = True
        c._Display = True

        l.Add(c)

    End Sub

    Private Sub SetFormatNrParamQualNumerico(ByRef c As ColonneNome, ByVal paramQual As Object)

        If paramQual(Tipo) = enum_TipoParamQual.Numero Then
            c._formatNr = String.Format("n{0}", paramQual(NumDecimali_Maximo))
        End If

    End Sub

    Private Function LeggiAgendaCollegata(piva As String,
                                          idAgenda As Integer,
                                          lavCod As Integer,
                                          objParametri As AgronicaCoreParametri
                                          ) As Integer

        Dim idAgendaCollegata As Integer = 0

        Dim objMovDetRif As New Mov_Dettagli_Riferimenti_R

        Dim dtMovDetRif = objMovDetRif.LeggiPerAgenda(piva, 0, idAgenda,
                                                      lavCod, -1,
                                                      "", "", objParametri,
                                                      leggiRiferimentiInversi:=False,
                                                      Lav_Cod_Rif:=lavCod, Cau_Mov_Rif:="-1")

        If dtMovDetRif.Rows.Count > 0 Then
            idAgendaCollegata = dtMovDetRif.Rows(0).Item("Id_Agenda_Rif")
        End If

        Return idAgendaCollegata

    End Function

    Private Function SeLeggiOrdineLavoro(piva As String,
                                         idAgenda As Integer,
                                         lavCod As Integer,
                                         objParametri As AgronicaCoreParametri
                                         ) As Integer

        Dim idOrdineLavoro As Integer = 0

        If lavCod = LAVCOD_TRASFORMAZIONI Then

            Dim objMovDetRif As New Mov_Dettagli_Riferimenti_R

            Dim dtMovDetRif = objMovDetRif.LeggiPerAgenda(piva, 0, idAgenda,
                                                          lavCod, -1,
                                                          "", "", objParametri,
                                                          leggiRiferimentiInversi:=False,
                                                          Lav_Cod_Rif:=LAVCOD_TESTATE_ORDINE_LAVORAZIONE, Cau_Mov_Rif:="-1")

            If dtMovDetRif.Rows.Count > 0 Then
                idOrdineLavoro = dtMovDetRif.Rows(0).Item("Id_Agenda_Rif")
            End If

        End If

        Return idOrdineLavoro

    End Function

    Private Function SeLeggiLavCollegate(piva As String,
                                         idAgenda As Integer,
                                         lavCod As Integer,
                                         objParametri As AgronicaCoreParametri
                                         ) As Integer

        Dim lavCollegate As Integer = 0

        If lavCod = LAVCOD_TESTATE_ORDINE_LAVORAZIONE Then

            Dim objMovDetRif As New Mov_Dettagli_Riferimenti_R

            Dim dtMovDetRif = objMovDetRif.LeggiPerAgendaRif(piva, 0, idAgenda,
                                                             lavCod, -1,
                                                             "", "", objParametri,
                                                             Lav_Cod:=LAVCOD_TRASFORMAZIONI,
                                                             Cau_Mov:="-1")

            lavCollegate = dtMovDetRif.Rows.Count

        End If

        Return lavCollegate

    End Function

    Public Shared Function CreaDesLibLavorazione(ByVal tipoLavorazioneDes As String,
                                                 ByVal matDes As String,
                                                 ByVal lotto As String,
                                                 ByVal descrizioneAggiuntiva As String) As String

        Dim stb As New StringBuilder

        If Not IsNothing(tipoLavorazioneDes) AndAlso Not String.IsNullOrEmpty(tipoLavorazioneDes) Then
            stb.Append(tipoLavorazioneDes)
        End If

        If Not IsNothing(matDes) AndAlso Not String.IsNullOrEmpty(matDes) Then
            stb.Append(If(stb.Length > 0, " ", "") & matDes)
        End If

        If Not IsNothing(lotto) AndAlso Not String.IsNullOrEmpty(lotto) Then
            stb.Append(If(stb.Length > 0, " ", "") & lotto)
        End If

        If Not IsNothing(descrizioneAggiuntiva) AndAlso Not String.IsNullOrEmpty(descrizioneAggiuntiva) Then
            stb.Append(If(stb.Length > 0, " ", "") & descrizioneAggiuntiva)
        End If

        Return stb.ToString

    End Function

    Private Function PreparazioneDesFromPreparazioneCod(
        ByVal piva As String,
        ByVal preparazioneCod As Integer,
        ByRef objParametriServer As AgronicaCoreParametri
        ) As String

        Dim lineePrepR As New Linee_Preparazioni_R
        Return lineePrepR.PreparazioneDes_From_PreparazioneCod(piva, preparazioneCod, objParametriServer)

    End Function

End Class
