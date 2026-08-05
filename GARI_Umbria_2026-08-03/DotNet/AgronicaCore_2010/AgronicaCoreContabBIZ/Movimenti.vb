Imports System.Collections.Concurrent
Imports System.Threading.Tasks
Imports System.Web
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class Qta_Prodotto_X_Impianto

    Private _Lav_Cod As Integer
    Private _Id_Agenda As Integer
    Private _Pro_Cod As Integer
    Private _Piva As String
    Private _Sa_Cod As Integer
    Private _Appezza As Integer
    Private _Id_Destinazione As Integer
    Private _Sup_Impianto As Decimal
    Private _Qta_Prodotto As Decimal
    Private _Qta_Acqua As Decimal

    Public Property Lav_Cod() As Integer
        Get
            Return _Lav_Cod
        End Get
        Set(ByVal value As Integer)
            _Lav_Cod = value
        End Set
    End Property

    Public Property Id_Agenda() As Integer
        Get
            Return _Id_Agenda
        End Get
        Set(ByVal value As Integer)
            _Id_Agenda = value
        End Set
    End Property

    Public Property Pro_Cod() As Integer
        Get
            Return _Pro_Cod
        End Get
        Set(ByVal value As Integer)
            _Pro_Cod = value
        End Set
    End Property

    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property

    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
        End Set
    End Property


    Public Property Appezza() As Integer
        Get
            Return _Appezza
        End Get
        Set(ByVal value As Integer)
            _Appezza = value
        End Set
    End Property

    Public Property Id_Destinazione() As Integer
        Get
            Return _Id_Destinazione
        End Get
        Set(ByVal value As Integer)
            _Id_Destinazione = value
        End Set
    End Property

    Public Property Sup_Impianto() As Decimal
        Get
            Return _Sup_Impianto
        End Get
        Set(ByVal value As Decimal)
            _Sup_Impianto = value
        End Set
    End Property


    Public Property Qta_Prodotto() As Decimal
        Get
            Return _Qta_Prodotto
        End Get
        Set(ByVal value As Decimal)
            _Qta_Prodotto = value
        End Set
    End Property

    Public Property Qta_Acqua() As Decimal
        Get
            Return _Qta_Acqua
        End Get
        Set(ByVal value As Decimal)
            _Qta_Acqua = value
        End Set
    End Property

End Class

Public Class Validita_Rilievi_FF_X_Impianto
    Private _Piva As String
    Private _Sa_Cod As Integer
    Private _Appezza As Integer
    Private _Id_Destinazione As Integer
    Private _Stadi_FF As List(Of Stadio_FF) = New List(Of Stadio_FF)

    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property

    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
        End Set
    End Property


    Public Property Appezza() As Integer
        Get
            Return _Appezza
        End Get
        Set(ByVal value As Integer)
            _Appezza = value
        End Set
    End Property

    Public Property Id_Destinazione() As Integer
        Get
            Return _Id_Destinazione
        End Get
        Set(ByVal value As Integer)
            _Id_Destinazione = value
        End Set
    End Property

    Public Property Stadi_FF() As List(Of Stadio_FF)
        Get
            Return _Stadi_FF
        End Get
        Set(ByVal value As List(Of Stadio_FF))
            _Stadi_FF = value
        End Set
    End Property



End Class

Public Class Stadio_FF
    Private _Id_Agenda_Validita_Inizio As Integer
    Private _Des_Lib_Validita_Inizio As String
    Private _Id_Agenda_Validita_Fine As Integer
    Private _Des_Lib_Validita_Fine As String
    Private _Stadio_Principale As Integer
    Private _Validita_Inizio As Date
    Private _Validita_Fine As Date
    Private _Descrizione_Stadio_Principale As String

    Public Property Id_Agenda_Validita_Inizio() As Integer
        Get
            Return _Id_Agenda_Validita_Inizio
        End Get
        Set(ByVal value As Integer)
            _Id_Agenda_Validita_Inizio = value
        End Set
    End Property

    Public Property Des_Lib_Validita_Inizio() As String
        Get
            Return _Des_Lib_Validita_Inizio
        End Get
        Set(ByVal value As String)
            _Des_Lib_Validita_Inizio = value
        End Set
    End Property

    Public Property Id_Agenda_Validita_Fine() As Integer
        Get
            Return _Id_Agenda_Validita_Fine
        End Get
        Set(ByVal value As Integer)
            _Id_Agenda_Validita_Fine = value
        End Set
    End Property

    Public Property Des_Lib_Validita_Fine() As String
        Get
            Return _Des_Lib_Validita_Fine
        End Get
        Set(ByVal value As String)
            _Des_Lib_Validita_Fine = value
        End Set
    End Property

    Public Property Stadio_Principale() As Integer
        Get
            Return _Stadio_Principale
        End Get
        Set(ByVal value As Integer)
            _Stadio_Principale = value
        End Set
    End Property

    Public Property Validita_Inizio() As Date
        Get
            Return _Validita_Inizio
        End Get
        Set(ByVal value As Date)
            _Validita_Inizio = value
        End Set
    End Property

    Public Property Descrizione_Stadio_Principale() As String
        Get
            Return _Descrizione_Stadio_Principale
        End Get
        Set(ByVal value As String)
            _Descrizione_Stadio_Principale = value
        End Set
    End Property

    Public Property Validita_Fine() As Date
        Get
            Return _Validita_Fine
        End Get
        Set(ByVal value As Date)
            _Validita_Fine = value
        End Set
    End Property
End Class

Public Class Movimenti_R
    Inherits AgronicaCoreDataProvider.LogProvider

    Private Shared _admissibleOperationsWithoutSpecies As IEnumerable(Of Integer) = {
        LAVCOD_RILIEVO_INDICI_MATURITA,
        LAVCOD_DANNI_RACCOLTA,
        LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO,
        LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO
    }

    '============================================================================
    Public Function Movimento_Leggi(ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Id_Agenda As Integer,
                                    ByVal Id_Mov As Integer,
                                    ByVal Cod_RisUm As Integer,
                                    ByVal Cau_Mov As String,
                                    ByVal ForDelete As Boolean,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    Optional ByVal leggiRiferimentiInversi As Boolean = True
                                    ) As String

        Const nomeRoutine = "ContabBIZ.Movimenti_R.Movimento_Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '   Id_Mov=0
        '   Cod_RisUm = 0
        '   Cau_Mov = ""
        '====================================================================================

        Dim messaggioErrore As String = ""

        Dim flagConnessioneLocale As Boolean = False
        Dim i As Integer
        Dim j As Integer

        Dim risultatoFunzione As String = String.Empty

        Dim xmlDoc As XmlDocument

        Dim xmlDatiMovimenti As XmlElement
        Dim xmlMovimenti As XmlElement
        Dim xmlDatiMovDettagliTecnici As XmlElement
        Dim xmlMovDettaglioTecnico As XmlElement
        Dim xmlDatiMovDettagliTecniciExtra As XmlElement
        Dim xmlMovDettaglioTecnicoExtra As XmlElement
        Dim xmlDatiMovimentixReport As XmlElement
        Dim xmlMovimentixReport As XmlElement
        Dim xmlMovRiferimento As XmlElement

        Dim xmlDatiMovimentiDettagli As XmlDocument
        Dim xDatiMovimentiDettagli As XmlNodeList
        Dim xDatiMovimentoDettaglio As XmlElement

        Dim xmlDatiPagamenti As XmlDocument
        Dim xDatiPagamenti As XmlNodeList
        Dim xDatiPagamento As XmlElement

        Dim objPagamenti As AgronicaCoreContabBIZ.Pagamento_R
        Dim objMovimentiDettagli As AgronicaCoreContabBIZ.Movimenti_Dettagli_R

        Dim objMovimenti As AgronicaCoreContabDAL.Movimenti_R
        Dim objMovDettaglioTecnico As AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
        Dim objMovDettaglioTecnicoExtra As AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_R
        Dim objMovimentiRiferimenti As AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
        Dim objMovimentixReport As AgronicaCoreContabDAL.MovimentixReport_R

        Dim DtMovimenti As DataTable
        Dim DtMov_Dettaglio_Tecnico As DataTable
        Dim DtMov_Dettaglio_Tecnico_Extra As DataTable
        Dim DtMovimenti_Riferimenti As DataTable
        Dim DtMovimentixReport As DataTable

        Dim i_DatiPagamenti As Integer

        Dim datiMovimentiDettagli As String
        Dim datiPagamenti As String

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                flagConnessioneLocale = True
                'Creo la connessione localmente
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If
            If objParametri.objConnessione.State = ConnectionState.Closed Then
                'Flag
                flagConnessioneLocale = True
                'Creo la connessione localmente
                objParametri.objConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                objParametri.objConnessione.Open()
            End If

            '------------------------------


            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            'Mi procuro un elenco dei Movimenti associati all'Impresa
            'all'interno della finestra temporale selezionata

            objMovimenti = New AgronicaCoreContabDAL.Movimenti_R

            Dim appFine As Date
            Dim appInizio As Date
            appFine = objParametri.FinestraTemporaleFine
            appInizio = objParametri.FinestraTemporaleInizio

            'Mi procuro il recordset richiesto
            DtMovimenti = objMovimenti.Leggi(CStr(Piva),
                                             CInt(Sa_Cod),
                                             CInt(Id_Agenda),
                                             CInt(Id_Mov),
                                             CInt(Cod_RisUm),
                                             CStr(Cau_Mov),
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             "",
                                             "",
                                             objParametri)

            objParametri.FinestraTemporaleFine = appFine
            objParametri.FinestraTemporaleInizio = appInizio

            'Se ottengo almeno un risultato, creo la struttura XML
            If DtMovimenti.Rows.Count > 0 Then

                '----- < Documento XML > -----
                xmlDoc = New XmlDocument

                xmlDatiMovimenti = xmlDoc.CreateElement("DatiMovimenti")


                'Effettuo un ciclo sui movimenti
                For i = 0 To DtMovimenti.Rows.Count - 1


                    '----- < MOVIMENTO > -----
                    xmlMovimenti = xmlDoc.CreateElement("Movimento")

                    With xmlMovimenti
                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                        .SetAttribute("piva", Agro_SQL_Load(DtMovimenti.Rows(i).Item("PIVA")))
                        .SetAttribute("sa_cod", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Sa_Cod")))
                        .SetAttribute("id_agenda", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Id_Agenda")))
                        .SetAttribute("lav_cod", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Lav_Cod")))
                        .SetAttribute("des_lib", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Des_Lib")))
                        .SetAttribute("tipo_accettazione", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Tipo_Accettazione")))
                        .SetAttribute("linea_cod", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Linea_Cod")))
                        .SetAttribute("preparazione_cod", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Preparazione_Cod")))
                        .SetAttribute("id_trasformazione", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Id_Trasformazione")))
                        .SetAttribute("cod_macchina_lav", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Cod_Macchina_Lav")))
                        .SetAttribute("id_mov", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Id_Mov")))
                        .SetAttribute("cod_risum", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Cod_RisUm")))
                        .SetAttribute("cau_mov", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Cau_Mov")))
                        .SetAttribute("mov_desc", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Mov_Desc")))
                        .SetAttribute("data_movimento", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Data_Movimento")))
                        .SetAttribute("scadenza", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Scadenza")))
                        .SetAttribute("scadenza_extra", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Scadenza_Extra")))
                        .SetAttribute("doc_numero", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Doc_Numero")))
                        .SetAttribute("doc_numero_sin", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Doc_Numero_Sin")))
                        .SetAttribute("doc_numero_des", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Doc_Numero_Des")))
                        .SetAttribute("doc_numero_visualizzato", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Doc_Numero_Visualizzato")))
                        .SetAttribute("num_protocollo", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Num_Protocollo")))
                        .SetAttribute("progr_protocollo", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Progr_Protocollo")))
                        .SetAttribute("progr_registrazione", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Progr_Registrazione")))
                        .SetAttribute("data_registrazione", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Data_Registrazione")))
                        .SetAttribute("cod_indirizzorisum", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Cod_IndirizzoRisUm")))
                        .SetAttribute("cod_destinazione", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Cod_Destinazione")))
                        .SetAttribute("cod_indirizzodestinazione", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Cod_IndirizzoDestinazione")))
                        .SetAttribute("mezzo", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Mezzo")))
                        .SetAttribute("cod_vettore", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Cod_Vettore")))
                        .SetAttribute("cod_indirizzovettore", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Cod_IndirizzoVettore")))
                        .SetAttribute("causale_trasporto", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Causale_Trasporto")))
                        .SetAttribute("aspetto", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Aspetto")))
                        .SetAttribute("peso", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Peso")))
                        .SetAttribute("ora", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Ora")))
                        .SetAttribute("colli", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Colli")))
                        .SetAttribute("tipo_sconto", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Tipo_Sconto")))
                        .SetAttribute("natura_beni", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Natura_Beni")))
                        .SetAttribute("tara_veicolo", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Tara_Veicolo")))
                        .SetAttribute("tara_imballi", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Tara_Imballi")))
                        .SetAttribute("tipo_peso", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Tipo_Peso")))
                        .SetAttribute("modalita", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Modalita")))
                        .SetAttribute("username_note", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Username_Note")))
                        .SetAttribute("extra_str", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Extra_Str")))
                        .SetAttribute("extra_int", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Extra_Int")))
                        .SetAttribute("extra_date", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Extra_Date")))
                        .SetAttribute("chklayout_bypass_fatturato", Agro_SQL_Load(DtMovimenti.Rows(i).Item("ChkLayOut_Bypass_Fatturato")))
                        .SetAttribute("chklayout_join_prodotti", Agro_SQL_Load(DtMovimenti.Rows(i).Item("ChkLayOut_Join_Prodotti")))
                        .SetAttribute("cod_risum_altro", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Cod_RisUm_Altro")))
                        .SetAttribute("chklayout_peso", Agro_SQL_Load(DtMovimenti.Rows(i).Item("ChkLayOut_Peso")))
                        .SetAttribute("chklayout_prezzo", Agro_SQL_Load(DtMovimenti.Rows(i).Item("ChkLayOut_Prezzo")))
                        .SetAttribute("chkfiltro_varietale", Agro_SQL_Load(DtMovimenti.Rows(i).Item("ChkFiltro_varietale")))
                        .SetAttribute("disciplinare_pubblicoprivato", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Disciplinare_PubblicoPrivato")))
                        .SetAttribute("sezionale_cod", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Sezionale_Cod")))
                        .SetAttribute("causale_trasporto_cod", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Causale_Trasporto_Cod")))
                        .SetAttribute("chklayout_litri", Agro_SQL_Load(DtMovimenti.Rows(i).Item("ChkLayOut_litri")))
                        .SetAttribute("cod_risum_aggiuntivo", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Cod_RisUm_Aggiuntivo")))
                        .SetAttribute("cod_indirizzo_aggiuntivo", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Cod_indirizzo_Aggiuntivo")))
                        .SetAttribute("chklayout_riscontrato", Agro_SQL_Load(DtMovimenti.Rows(i).Item("ChkLayOut_Riscontrato")))
                        .SetAttribute("username_creazione", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Username_Creazione")))
                        .SetAttribute("username_modifica", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Username_Modifica")))
                        .SetAttribute("validita_inizio", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Validita_Inizio")))
                        .SetAttribute("validita_fine", Agro_SQL_Load(DtMovimenti.Rows(i).Item("Validita_Fine")))
                    End With


                    '#############################################
                    '##########  RIFERIMENTI MOVIMENTI  ##########
                    '#############################################

                    'Mi procuro un elenco dei riferimenti del Movimento

                    Select Case Agro_SQL_Load(DtMovimenti.Rows(i).Item("Cau_Mov"))

                        Case CAU_CARICO, CAU_SCARICO, CAU_CONFERIMENTO, CAU_CONFERIMENTO_DIVERSI,
                             CAU_ACCETTAZIONE_BENI, CAU_ACCETTAZIONE_BENI_DA_DIVERSI,
                             CAU_IMPUTAZIONE_MANODOPERA, CAU_IMPUTAZIONE_TERZISTI,
                             CAU_IMPUTAZIONE_UTILIZZO_PRODOTTI, CAU_IMPUTAZIONE_PARCOMACCHINE

                            'Escludo i movimenti collaterali a quello base
                            '(carico, scarico, conferimenti, accettazioni, imputazione costi accessori)

                        Case Else

                            objMovimentiRiferimenti = New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R

                            'Mi procuro il recordset richiesto
                            DtMovimenti_Riferimenti = objMovimentiRiferimenti.Leggi(
                                                     "",
                                                     0,
                                                     CInt(Agro_SQL_Load(DtMovimenti.Rows(i).Item("Id_Agenda"))),
                                                     -1,
                                                     -1,
                                                      0,
                                                     "",
                                                     "",
                                                     "",
                                                     objParametri,
                                                     leggiRiferimentiInversi)

                            '                           '************************************************************************************************
                            '                           '***** 2008/07/29 Modifica Mauro: Leggo solo i record che hanno l'id_agenda specificato nella prima metà
                            '                           '************************************************************************************************
                            '
                            '                           Set RsMovimenti_Riferimenti = ObjMovimenti_Riferimenti.LeggiXChiave( _
                            '                                                            , _
                            '                                                            , _
                            '                                                            cint(Agro_SQL_Load(RsMovimenti("Id_Agenda"))), _
                            '                                                            -1, _
                            '                                                            -1, _
                            '                                                            , _
                            '                                                            , _
                            '                                                            , _
                            '                                                            , _
                            '                                                            , _
                            '                                                            , _
                            '                                                            objCnManager, _
                            '                                                            FinestraTemp_Inizio, _
                            '                                                            FinestraTemp_Fine, _
                            '                                                            ConnessioneAlternativa _
                            '                                                            )
                            '                           '*******************************************************************************

                            'Se ottengo almeno un risultato, creo la struttura XML
                            If DtMovimenti_Riferimenti.Rows.Count > 0 Then

                                'Effettuo un ciclo sui riferimenti
                                For j = 0 To DtMovimenti_Riferimenti.Rows.Count - 1

                                    '----- < RIFERIMENTO > -----
                                    xmlMovRiferimento = xmlDoc.CreateElement("Movimento_Riferimento")

                                    With xmlMovRiferimento
                                        .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                        .SetAttribute("piva", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("Piva")))
                                        .SetAttribute("sa_cod", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("Sa_Cod")))
                                        .SetAttribute("id_agenda", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("Id_Agenda")))
                                        .SetAttribute("id_mov", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("Id_Mov")))
                                        .SetAttribute("id_mov_det", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("Id_Mov_Det")))
                                        .SetAttribute("lav_cod", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("Lav_Cod")))
                                        .SetAttribute("cau_mov", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("Cau_Mov")))

                                        .SetAttribute("piva_rif", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("Piva_Rif")))
                                        .SetAttribute("sa_cod_rif", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("Sa_Cod_Rif")))
                                        .SetAttribute("id_agenda_rif", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("Id_Agenda_Rif")))
                                        .SetAttribute("id_mov_rif", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("Id_Mov_Rif")))
                                        .SetAttribute("id_mov_det_rif", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("Id_Mov_Det_Rif")))
                                        .SetAttribute("lav_cod_rif", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("Lav_Cod_Rif")))
                                        .SetAttribute("cau_mov_rif", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("Cau_Mov_Rif")))

                                        .SetAttribute("qta", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("Qta")))

                                        .SetAttribute("validita_inizio", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("validita_inizio")))
                                        .SetAttribute("validita_fine", Agro_SQL_Load(DtMovimenti_Riferimenti.Rows(j).Item("validita_fine")))

                                    End With

                                    xmlMovimenti.AppendChild(xmlMovRiferimento)

                                    '----- < / RIFERIMENTO > -----

                                Next


                            End If

                            xmlMovRiferimento = Nothing
                            DtMovimenti_Riferimenti.Dispose()
                            objMovimentiRiferimenti = Nothing

                    End Select

                    '#################################
                    '#################################
                    '#################################




                    '#########################################################
                    '##########  DETTAGLIO TECNICO DEL MOVIMENTO  ############
                    '#########################################################

                    'Mi procuro un elenco dei dettagli tecnici del movimento

                    objMovDettaglioTecnico = New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R

                    'Mi procuro il RecordSet richiesto
                    DtMov_Dettaglio_Tecnico = objMovDettaglioTecnico.Leggi(CStr(Agro_SQL_Load(DtMovimenti.Rows(i).Item("PIVA"))),
                                                                           0,
                                                                           CInt(Agro_SQL_Load(DtMovimenti.Rows(i).Item("Id_Agenda"))),
                                                                           CInt(Agro_SQL_Load(DtMovimenti.Rows(i).Item("Id_Mov"))),
                                                                           0,
                                                                           0,
                                                                           "",
                                                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                           "",
                                                                           "",
                                                                           objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtMov_Dettaglio_Tecnico.Rows.Count > 0 Then

                        xmlDatiMovDettagliTecnici = xmlDoc.CreateElement("DatiMov_Dettagli_Tecnici")

                        'Effettuo un ciclo sui Dettagli Tecnici del Movimento
                        For j = 0 To DtMov_Dettaglio_Tecnico.Rows.Count - 1

                            '----- < DETTAGLIO TECNICO > -----
                            xmlMovDettaglioTecnico = xmlDoc.CreateElement("Movimento_Dettaglio_Tecnico")

                            With xmlMovDettaglioTecnico
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("id_agenda", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Id_Agenda")))
                                .SetAttribute("id_mov", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Id_Mov")))
                                .SetAttribute("id_mov_det", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Id_Mov_Det")))
                                .SetAttribute("id_reg_dettaglio", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Id_Reg_Dettaglio")))
                                .SetAttribute("qta_ril", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Qta_Ril")))
                                .SetAttribute("data_ril", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Data_Ril")))
                                .SetAttribute("ditta_cod", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Ditta_Cod")))
                                .SetAttribute("dett_cod", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Dett_Cod")))
                                .SetAttribute("id_insetto", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Id_Insetto")))
                                .SetAttribute("ff_classe", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("FF_Classe")))
                                .SetAttribute("dose", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Dose")))
                                .SetAttribute("mg", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Mg")))
                                .SetAttribute("n", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("N")))
                                .SetAttribute("k", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("K")))
                                .SetAttribute("p", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("P")))
                                .SetAttribute("cu", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("cu")))
                                .SetAttribute("parziale", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Parziale")))
                                .SetAttribute("nitrati", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Nitrati")))
                                .SetAttribute("freatimetro", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Freatimetro")))
                                .SetAttribute("piezo1", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Piezo1")))
                                .SetAttribute("piezo2", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Piezo2")))
                                .SetAttribute("piezo3", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Piezo3")))
                                .SetAttribute("piezo4", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Piezo4")))
                                .SetAttribute("sigla_av", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Sigla_Av")))
                                .SetAttribute("trap_num", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Trap_Num")))
                                .SetAttribute("inn1_data", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Inn1_Data")))
                                .SetAttribute("inn2_data", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Inn2_Data")))
                                .SetAttribute("inn3_data", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Inn3_Data")))
                                .SetAttribute("inn4_data", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Inn4_Data")))
                                .SetAttribute("av_cod", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Av_Cod")))
                                .SetAttribute("av_gru", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Av_Gru")))
                                .SetAttribute("lotto", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Lotto")))
                                .SetAttribute("extra_int", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Extra_Int")))
                                .SetAttribute("extra_str", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Extra_Str")))
                                .SetAttribute("extra_date", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Extra_Date")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("validita_fine")))

                                .SetAttribute("soglia_cod", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Soglia_Cod")))
                                .SetAttribute("soglia_quantita", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Soglia_Quantita")))
                                .SetAttribute("soglia_des", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Soglia_Des")))
                                .SetAttribute("efficienza", Agro_SQL_Load(DtMov_Dettaglio_Tecnico.Rows(j).Item("Efficienza")))
                            End With

                            xmlDatiMovDettagliTecnici.AppendChild(xmlMovDettaglioTecnico)
                            '----- < / DETTAGLIO TECNICO > -----

                        Next

                        xmlMovimenti.AppendChild(xmlDatiMovDettagliTecnici)

                    End If

                    xmlDatiMovDettagliTecnici = Nothing
                    xmlMovDettaglioTecnico = Nothing
                    DtMov_Dettaglio_Tecnico.Dispose()
                    objMovDettaglioTecnico = Nothing




                    '#########################################################
                    '##########  DETTAGLIO TECNICO EXTRA DEL MOVIMENTO #######
                    '#########################################################

                    'Mi procuro un elenco dei dettagli tecnici extra del movimento

                    objMovDettaglioTecnicoExtra = New AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_R

                    'Mi procuro il RecordSet richiesto
                    DtMov_Dettaglio_Tecnico_Extra = objMovDettaglioTecnicoExtra.Leggi(
                                            CStr(Agro_SQL_Load(DtMovimenti.Rows(i).Item("PIVA"))),
                                            0,
                                            CInt(Agro_SQL_Load(DtMovimenti.Rows(i).Item("Id_Agenda"))),
                                            CInt(Agro_SQL_Load(DtMovimenti.Rows(i).Item("Id_Mov"))),
                                            0,
                                            0,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtMov_Dettaglio_Tecnico_Extra.Rows.Count > 0 Then

                        xmlDatiMovDettagliTecniciExtra = xmlDoc.CreateElement("DatiMov_Dettagli_Tecnici_Extra")

                        'Effettuo un ciclo sui Dettagli Tecnici Extra del Movimento
                        For j = 0 To DtMov_Dettaglio_Tecnico_Extra.Rows.Count - 1

                            '----- < DETTAGLIO TECNICO EXTRA> -----
                            xmlMovDettaglioTecnicoExtra = xmlDoc.CreateElement("Movimento_Dettaglio_Tecnico_Extra")

                            With xmlMovDettaglioTecnicoExtra
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("id_agenda", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Id_Agenda")))
                                .SetAttribute("id_mov", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Id_Mov")))
                                .SetAttribute("id_mov_det", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Id_Mov_Det")))
                                .SetAttribute("id_reg_dettaglio", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Id_Reg_Dettaglio")))
                                .SetAttribute("regione", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("regione")))
                                .SetAttribute("asl", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("asl")))
                                .SetAttribute("serie", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("serie")))
                                .SetAttribute("numero", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("numero")))
                                .SetAttribute("mac_cod", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("mac_cod")))
                                .SetAttribute("cod_risum", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("cod_risum")))
                                .SetAttribute("trasportatore", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("trasportatore")))
                                .SetAttribute("mezzo_trasporto", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("mezzo_trasporto")))
                                .SetAttribute("targa", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("targa")))
                                .SetAttribute("n_immatricolazione", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("n_immatricolazione")))
                                .SetAttribute("n_immatricolazione_rimorchio", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("n_immatricolazione_rimorchio")))
                                .SetAttribute("n_autorizzazione_trasporto", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("n_autorizzazione_trasporto")))
                                .SetAttribute("data_rilascio_autorizzazione", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("data_rilascio_autorizzazione")))
                                .SetAttribute("peso", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Peso")))
                                .SetAttribute("codice_prodotto", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Codice_Prodotto")))
                                .SetAttribute("colore", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Colore")))
                                .SetAttribute("zona_viticola", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Zona_Viticola")))
                                .SetAttribute("manipolazioni", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Manipolazioni")))
                                .SetAttribute("precisazioni", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Precisazioni")))
                                .SetAttribute("annotazioni", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Annotazioni")))
                                .SetAttribute("num_contenitori", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Num_Contenitori")))
                                .SetAttribute("marche_contenitori", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Marche_Contenitori")))
                                .SetAttribute("des_contenitori", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Des_Contenitori")))
                                .SetAttribute("tipo_documento", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Tipo_Documento")))
                                .SetAttribute("id_cod_autorita", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Id_Cod_Autorita")))
                                .SetAttribute("luogo_partenza", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Luogo_Partenza")))
                                .SetAttribute("luogo_consegna", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Luogo_Consegna")))
                                .SetAttribute("data_spedizione", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Data_Spedizione")))
                                .SetAttribute("indicazioni_complementari", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Indicazioni_Complementari")))
                                .SetAttribute("titolo_alcol", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Titolo_Alcol")))
                                .SetAttribute("codice_nc", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Codice_NC")))
                                .SetAttribute("num_riferimento", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Num_Riferimento")))
                                .SetAttribute("data_dichiarazione", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Data_Dichiarazione")))
                                .SetAttribute("garanzia", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Garanzia")))
                                .SetAttribute("certificati", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Certificati")))
                                .SetAttribute("durata_viaggio", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Durata_Viaggio")))
                                .SetAttribute("peso_lordo", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Peso_Lordo")))
                                .SetAttribute("num_colli", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Num_Colli")))
                                .SetAttribute("contenitore_cod", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Contenitore_Cod")))
                                .SetAttribute("imballaggio_cod", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Imballaggio_Cod")))
                                .SetAttribute("agente_cod", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Agente_Cod")))
                                .SetAttribute("provvigione", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Provvigione")))

                                .SetAttribute("tipo_trasporto", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Tipo_Trasporto")))
                                .SetAttribute("unita_trasporto", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Unita_Trasporto")))
                                .SetAttribute("codice_alternativo", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Codice_Alternativo")))
                                .SetAttribute("id_gestione_vettore", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Id_Gestione_Vettore")))
                                .SetAttribute("ritenuta_acconto_cod", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Ritenuta_Acconto_Cod")))
                                .SetAttribute("ritenuta_acconto", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Ritenuta_Acconto")))
                                .SetAttribute("enasarco_cod", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Enasarco_Cod")))
                                .SetAttribute("enasarco", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Enasarco")))
                                .SetAttribute("accdaa_cod_risum_destinatario", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("ACCDAA_Cod_Risum_Destinatario")))
                                .SetAttribute("accdaa_cod_risum_destinazione", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("ACCDAA_Cod_Risum_Destinazione")))
                                .SetAttribute("accdaa_cod_indirizzorisum_destinatario", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("ACCDAA_Cod_IndirizzoRisum_Destinatario")))
                                .SetAttribute("accdaa_cod_indirizzorisum_destinazione", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("ACCDAA_Cod_IndirizzoRisum_Destinazione")))
                                .SetAttribute("capoarea_cod", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("CapoArea_Cod")))
                                .SetAttribute("provvigione_capoarea", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Provvigione_CapoArea")))
                                .SetAttribute("provvigione_pagata_agente", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Provvigione_Pagata_Agente")))
                                .SetAttribute("provvigione_pagata_capoarea", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Provvigione_Pagata_CapoArea")))
                                .SetAttribute("n_doc_cliente", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("N_Doc_Cliente")))
                                .SetAttribute("data_doc_cliente", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Data_Doc_Cliente")))
                                .SetAttribute("n_doc_ente", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("N_Doc_Ente")))
                                .SetAttribute("anno_doc_ente", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Anno_Doc_Ente")))
                                .SetAttribute("num_conf_riscontrate", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Num_Conf_Riscontrate")))
                                .SetAttribute("num_colli_riscontrati", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Num_Colli_Riscontrati")))
                                .SetAttribute("num_imballi_riscontrati", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Num_Imballi_Riscontrati")))
                                .SetAttribute("peso_netto_riscontrato", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Peso_Netto_Riscontrato")))
                                .SetAttribute("peso_lordo_riscontrato", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Peso_Lordo_Riscontrato")))
                                .SetAttribute("tara_unit_conf_riscontrata", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Tara_Unit_Conf_Riscontrata")))
                                .SetAttribute("tara_unit_collo_riscontrata", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Tara_Unit_Collo_Riscontrata")))
                                .SetAttribute("tara_unit_imballo_riscontrata", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Tara_Unit_Imballo_Riscontrata")))
                                .SetAttribute("n_nota_fattura", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("N_Nota_Fattura")))
                                .SetAttribute("data_nota_fattura", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Data_Nota_Fattura")))
                                .SetAttribute("n_nota_ddt", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("N_Nota_DDT")))
                                .SetAttribute("n_nota_riga_ddt", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("N_Nota_Riga_DDT")))
                                .SetAttribute("data_nota_ddt", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Data_Nota_DDT")))
                                .SetAttribute("causale_fattura", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("Causale_Fattura")))

                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtMov_Dettaglio_Tecnico_Extra.Rows(j).Item("validita_fine")))
                            End With

                            xmlDatiMovDettagliTecniciExtra.AppendChild(xmlMovDettaglioTecnicoExtra)
                            '----- < / DETTAGLIO TECNICO EXTRA> -----

                        Next

                        xmlMovimenti.AppendChild(xmlDatiMovDettagliTecniciExtra)

                    End If

                    xmlDatiMovDettagliTecniciExtra = Nothing
                    xmlMovDettaglioTecnicoExtra = Nothing
                    DtMov_Dettaglio_Tecnico_Extra.Dispose()
                    objMovDettaglioTecnicoExtra = Nothing


                    '#########################################################
                    '###############  MOVIMENTI X REPORT  ####################
                    '#########################################################

                    'Mi procuro un elenco dei dettagli del report associato al movimento

                    objMovimentixReport = New AgronicaCoreContabDAL.MovimentixReport_R

                    'Mi procuro il recordset richiesto
                    DtMovimentixReport = objMovimentixReport.Leggi(
                                            CStr(Agro_SQL_Load(DtMovimenti.Rows(i).Item("PIVA"))),
                                            CInt(Agro_SQL_Load(DtMovimenti.Rows(i).Item("Sa_Cod"))),
                                            CInt(Agro_SQL_Load(DtMovimenti.Rows(i).Item("Id_Agenda"))),
                                            CInt(Agro_SQL_Load(DtMovimenti.Rows(i).Item("Id_Mov"))),
                                            0,
                                            0,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri)

                    'Se ottengo almeno un risultato, creo la struttura XML
                    If DtMovimentixReport.Rows.Count > 0 Then

                        xmlDatiMovimentixReport = xmlDoc.CreateElement("DatiMovimentixReport")

                        'Effettuo un ciclo sui MovimentixReport
                        For j = 0 To DtMovimentixReport.Rows.Count - 1


                            '----- < MOVIMENTOXREPORT> -----
                            xmlMovimentixReport = xmlDoc.CreateElement("MovimentoxReport")

                            With xmlMovimentixReport
                                .SetAttribute("TipoOperazioneDB", IIf(ForDelete, "3", "0"))
                                .SetAttribute("piva", Agro_SQL_Load(DtMovimentixReport.Rows(j).Item("piva")))
                                .SetAttribute("sa_cod", Agro_SQL_Load(DtMovimentixReport.Rows(j).Item("sa_cod")))
                                .SetAttribute("id_agenda", Agro_SQL_Load(DtMovimentixReport.Rows(j).Item("id_agenda")))
                                .SetAttribute("id_mov", Agro_SQL_Load(DtMovimentixReport.Rows(j).Item("id_mov")))
                                .SetAttribute("id_mov_det", Agro_SQL_Load(DtMovimentixReport.Rows(j).Item("id_mov_det")))
                                .SetAttribute("id_report", Agro_SQL_Load(DtMovimentixReport.Rows(j).Item("id_report")))
                                .SetAttribute("descrizione", Agro_SQL_Load(DtMovimentixReport.Rows(j).Item("descrizione")))
                                .SetAttribute("validita_inizio", Agro_SQL_Load(DtMovimentixReport.Rows(j).Item("validita_inizio")))
                                .SetAttribute("validita_fine", Agro_SQL_Load(DtMovimentixReport.Rows(j).Item("validita_fine")))
                            End With

                            xmlDatiMovimentixReport.AppendChild(xmlMovimentixReport)
                            '----- < / MOVIMENTOXREPORT> -----

                        Next

                        xmlMovimenti.AppendChild(xmlDatiMovimentixReport)

                    End If

                    xmlDatiMovimentixReport = Nothing
                    xmlMovimentixReport = Nothing
                    DtMovimentixReport.Dispose()
                    objMovimentixReport = Nothing


                    '#############################################
                    '##############  PAGAMENTI  ##################
                    '#############################################

                    'Mi procuro un elenco dei pagamenti del movimento

                    objPagamenti = New AgronicaCoreContabBIZ.Pagamento_R

                    datiPagamenti = objPagamenti.Pagamento_Leggi(CStr(DtMovimenti.Rows(i).Item("PIVA")),
                                                                 CInt(DtMovimenti.Rows(i).Item("Sa_Cod")),
                                                                 CInt(DtMovimenti.Rows(i).Item("Id_Agenda")),
                                                                 CInt(DtMovimenti.Rows(i).Item("Id_Mov")),
                                                                 0,
                                                                 ForDelete,
                                                                 objParametri)

                    If datiPagamenti <> "" Then

                        xmlDatiPagamenti = New XmlDocument
                        'XmlDatiPagamenti.async = False

                        xmlDatiPagamenti.LoadXml(datiPagamenti)

                        xDatiPagamenti = xmlDatiPagamenti.GetElementsByTagName("DatiPagamenti")

                        i_DatiPagamenti = 0

                        Do While i_DatiPagamenti < xDatiPagamenti.Count

                            xDatiPagamento = xDatiPagamenti.Item(i_DatiPagamenti)

                            Dim xDatiPagamento2 As XmlNode = xmlMovimenti.OwnerDocument.ImportNode(xDatiPagamento, True)

                            xmlMovimenti.AppendChild(xDatiPagamento2)
                            i_DatiPagamenti += 1
                        Loop

                    End If

                    '----- < / PAGAMENTI > -----




                    '#############################################
                    '##########  MOVIMENTI DETTAGLI  #############
                    '#############################################

                    'Mi procuro un elenco dei dettagli del movimento

                    'Dim NuovoNodo As XmlElement = XmlDoc.CreateElement("DatiMovimenti_Dettagli")

                    objMovimentiDettagli = New AgronicaCoreContabBIZ.Movimenti_Dettagli_R

                    datiMovimentiDettagli = objMovimentiDettagli.Movimento_Dettaglio_Leggi(
                                                CStr(DtMovimenti.Rows(i).Item("PIVA")),
                                                0,
                                                CInt(DtMovimenti.Rows(i).Item("Id_Agenda")),
                                                CInt(DtMovimenti.Rows(i).Item("Id_Mov")),
                                                0, 0, 0, 0, 0, 0, 0, 0,
                                                "", 0, 0,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                AGRODATAFINE,
                                                0, 0, 0, 0, 0, 0,
                                                ForDelete,
                                                0,
                                                objParametri,
                                                leggiRiferimentiInversi)

                    If datiMovimentiDettagli <> "" Then

                        xmlMovimenti.InnerXml = xmlMovimenti.InnerXml & datiMovimentiDettagli

                        'xmlDatiMovimentiDettagli = New XmlDocument
                        ''xmlDatiMovimentiDettagli.async = False
                        'xmlDatiMovimentiDettagli.LoadXml(DatiMovimenti_Dettagli)

                        'xDatiMovimentiDettagli = XmlDatiMovimentiDettagli.GetElementsByTagName("DatiMovimenti_Dettagli")

                        'i_DatiMovimenti_Dettagli = 0

                        'Do While i_DatiMovimenti_Dettagli < xDatiMovimentiDettagli.Count
                        '    xDatiMovimentoDettaglio = xDatiMovimentiDettagli.Item(i_DatiMovimenti_Dettagli)


                        '    'xmlMovimenti.AppendChild(xDatiMovimentoDettaglio)
                        '    i_DatiMovimenti_Dettagli = i_DatiMovimenti_Dettagli + 1
                        'Loop

                        xmlDatiMovimentiDettagli = Nothing
                        xDatiMovimentiDettagli = Nothing
                        xDatiMovimentoDettaglio = Nothing
                        objMovimentiDettagli = Nothing

                    End If

                    '----- < / MOVIMENTI DETTAGLI > -----



                    '#################################
                    '#################################
                    '#################################

                    xmlDatiMovimenti.AppendChild(xmlMovimenti)
                    '----- < / MOVIMENTO > -----

                Next

                xmlDoc.AppendChild(xmlDatiMovimenti)

                risultatoFunzione = xmlDoc.OuterXml
                '----- < / Documento XML > -----


                xmlMovimenti = Nothing
                xmlDatiMovimenti = Nothing
                xmlDoc = Nothing

            Else

                'Altrimenti, se non risulta selezionato nessun contatto ...
                risultatoFunzione = ""

            End If


            'Elimino gli oggetti che ho creato
            DtMovimenti.Dispose()
            DtMovimenti = Nothing
            objMovimenti = Nothing


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
                If Not IsNothing(objParametri.objConnessione) Then
                    objParametri.objConnessione.Close()
                    objParametri.objConnessione.Dispose()
                End If
            End If

        End Try

        'Restituisco il risultato
        Return risultatoFunzione

    End Function

    Private Function GetDtRA(
        mode As String,
        ByRef xFiltroAggiuntivo_colturali As String,
        ricetta_cod As Integer,
        objParametri_Server As AgronicaCoreParametri
    ) As DataTable
        Dim DtRA As DataTable = Nothing
        If mode = "AggiungiAlPua" Then
            Dim Lav_Cod_Fertilizzanti = String.Join(",", ({
                LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE,
                LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_CONCIMAZIONE_FOGLIARE,
                LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA
            }))

            If xFiltroAggiuntivo_colturali <> "" Then
                xFiltroAggiuntivo_colturali = " And agenda.lav_cod in (" & Lav_Cod_Fertilizzanti & ") And reg_impianti.cul_cod<>0 "
            Else
                xFiltroAggiuntivo_colturali = " agenda.lav_cod in (" & Lav_Cod_Fertilizzanti & ") And reg_impianti.cul_cod<>0 "
            End If

            Dim objRA As New AgronicaCoreContabDAL.RicettexAgenda_R
            If ricetta_cod > 0 Then
                DtRA = objRA.Leggi(
                    ricetta_cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                    enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                    xFiltroAggiuntivo:="", xOrderBy:="", objParametri_Server
                )
            End If
        End If
        Return DtRA
    End Function

    ''' <summary>
    ''' Nel caso in cui provengo dal pua per ribaltare le fertilizzazioni già registrate visualizzo solamente le
    ''' fertilizzazioni ed escludo i terreni nudi.
    ''' </summary>
    Private Sub RibaltaFertilizzazioniPUA(ByRef dtAgenda As DataTable, mode As String, dtRA As DataTable)
        If mode = "AggiungiAlPua" Then
            dtAgenda.Columns.Add(New DataColumn With {.DataType = GetType(String), .ColumnName = "AggiungiAlPua", .DefaultValue = "true"})
            If dtRA IsNot Nothing AndAlso dtRA.Rows.Count > 0 Then
                For i = 0 To dtAgenda.Rows.Count - 1
                    Dim DrRA = dtRA.Select("id_agenda=" & dtAgenda.Rows(i).Item("id_agenda"))
                    If DrRA IsNot Nothing AndAlso DrRA.Length > 0 Then
                        dtAgenda.Rows(i).Item("AggiungiAlPua") = "false"
                    End If
                Next
            End If
        End If
    End Sub

    ''' <summary>
    ''' Se sto cercando di filtrare le operazioni senza spiecie (veg_cod = 0).
    ''' 
    ''' (2025/03/31): Al momento solo le operazioni di visita generica possono essere senza una specie associata.
    ''' Le visite generiche possono avere come lav_cod quello dei rilievi di indici di maturità o di danni alla raccolta.
    ''' </summary>
    ''' <param name="flag_NessunaSpecieQdC">Se attivo cercare le Operazioni che hanno una Specie vegetale (non hanno impianti selezionati)</param>
    Private Sub FiltraOperazioniSenzaSpecie(ByRef dtAgenda As DataTable, flag_NessunaSpecieQdC As Boolean)

        If flag_NessunaSpecieQdC Then
            Dim lavCodFilter = String.Join(", ", Movimenti_R._admissibleOperationsWithoutSpecies)

            Dim drAgenda As DataRow() = dtAgenda.Select(" Veg_Cod = 0 AND PK_Impianti LIKE '%,0,0' AND lav_cod in (" & lavCodFilter & ") ")

            If Not IsNothing(drAgenda) AndAlso drAgenda.Length > 0 Then
                dtAgenda = drAgenda.CopyToDataTable()
            Else
                dtAgenda.Rows.Clear()
            End If
        End If

    End Sub

    Private Sub HandleGrigliaTipo2(
        ByRef dtAgenda As DataTable,
        ByRef dtRA As DataTable,
        mode As String,
        flag_NessunaSpecieQdC As Boolean
    )
        EstendiDatatable(dtAgenda)
        RibaltaFertilizzazioniPUA(dtAgenda, mode, dtRA)
        FiltraOperazioniSenzaSpecie(dtAgenda, flag_NessunaSpecieQdC)
    End Sub

    Public Function CaricaOperazioni(ByVal filtro As String, ByVal piva As String, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri,
                                     Optional ByVal visualizza_colonna_Ricette As Boolean = False) As RispostaStandard

        Dim jSonDatiTESTATA As JObject = JObject.Parse(filtro)

        Dim r As New RispostaStandard(True)
        Dim sa_cod As Integer = 0
        Dim veg_cod As Integer = 0
        Dim id_cod As Integer = 0
        Dim cul_cod As Integer = 0
        Dim tipo As String = ""
        Dim gru_cod As Integer = 0
        Dim lav_cod As Integer = 0
        Dim mode As String = ""
        Dim ricetta_cod As Integer = 0

        Dim flag_TerrenoNudo As Boolean = True

        'Dim FiltroLavorazioni As String = ""
        'Dim Filtro_Tipo_GruppoOperazioni As String = ""

        Dim sData_Selezionata1 As String
        Dim Data_Selezionata1 As DateTime

        Dim sData_Selezionata2 As String
        Dim Data_Selezionata2 As DateTime

        ''' Griglia operazioni = "2"
        Dim TipoGriglia As String
        Dim xFiltroAggiuntivo_colturali As String

        Dim estraiPkImpianti As Boolean = True

        'Se a true cerco le Operazioni che possono essere salvate senza specie vegetale (quindi senza impianti selezionati)
        Dim flag_NessunaSpecieQdC As Boolean = False

        Try
            TipoGriglia = jSonDatiTESTATA("TipoGriglia").ToString
        Catch ex As Exception
        End Try

        Try
            xFiltroAggiuntivo_colturali = jSonDatiTESTATA("xFiltroAggiuntivo_colturali").ToString
        Catch ex As Exception
        End Try

        Try
            flag_TerrenoNudo = jSonDatiTESTATA("flag_TerrenoNudo").ToString
        Catch ex As Exception
            flag_TerrenoNudo = True
        End Try

        Try
            flag_NessunaSpecieQdC = If(Not IsNothing(jSonDatiTESTATA("flag_NessunaSpecieQdC")) AndAlso
                                        Not String.IsNullOrEmpty(jSonDatiTESTATA("flag_NessunaSpecieQdC")) AndAlso
                                       jSonDatiTESTATA("flag_NessunaSpecieQdC").ToString().ToLower() = "true", True, False)
        Catch ex As Exception
            flag_NessunaSpecieQdC = False
        End Try

        If IsNumeric(jSonDatiTESTATA("sa_cod")) Then
            sa_cod = jSonDatiTESTATA("sa_cod")
        End If

        If IsNumeric(jSonDatiTESTATA("veg_cod")) Then
            veg_cod = jSonDatiTESTATA("veg_cod")
        ElseIf jSonDatiTESTATA("veg_cod") IsNot Nothing AndAlso jSonDatiTESTATA("veg_cod").ToString.Contains("/") Then
            If IsNumeric(jSonDatiTESTATA("veg_cod").ToString.Split("/")(1)) Then
                id_cod = jSonDatiTESTATA("veg_cod").ToString.Split("/")(1)
            End If
        End If

        sData_Selezionata1 = jSonDatiTESTATA("txt_Data1").ToString
        Data_Selezionata1 = If(IsDate(sData_Selezionata1), CDate(sData_Selezionata1), AGRODATAINIZIO)

        sData_Selezionata2 = jSonDatiTESTATA("txt_Data2").ToString
        Data_Selezionata2 = If(IsDate(sData_Selezionata2), CDate(sData_Selezionata2), AGRODATAFINE)

        mode = jSonDatiTESTATA("mode").ToString

        If IsNumeric(jSonDatiTESTATA("ricetta_cod")) Then
            ricetta_cod = jSonDatiTESTATA("ricetta_cod")
        End If

        Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R

        Dim xFiltroAggiuntivo_postRaccolta As String = ""
        Dim xFiltroAggiuntivo_contabili As String = ""
        Dim xFiltroAggiuntivo_contabili_Macchine As String = ""
        Dim xFiltroAggiuntivo_contabili_Audit As String = ""
        Dim xFiltroAggiuntivo_Visite As String = ""

        If xFiltroAggiuntivo_colturali <> "" OrElse mode = "AggiungiAlPua" Then
            xFiltroAggiuntivo_postRaccolta = " (Agenda.ID_Agenda = -1) "
            xFiltroAggiuntivo_contabili = " (Agenda.ID_Agenda = -1) "
            xFiltroAggiuntivo_contabili_Audit = " (Agenda.ID_Agenda = -1) "
            xFiltroAggiuntivo_contabili_Macchine = " (Agenda.ID_Agenda = -1) "
            xFiltroAggiuntivo_Visite = " (Agenda.ID_Agenda = -1) "
        End If

        Dim dtRA = GetDtRA(mode, xFiltroAggiuntivo_colturali, ricetta_cod, objParametri_Server)

        If HttpContext.Current.Session IsNot Nothing Then
            caricaInSessioneImpostazioniutente()

            Dim dtUtente As DataTable
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
            dtUtente = objUtenti.Leggi_Superuser_e_ProgressivoGIAS(objParametri_Utenti.UtenteUsername, "",
                                                            Date.Now,
                                                            CType(Now.Hour, Short),
                                                            0, objParametri_Utenti)
            If Not dtUtente Is Nothing AndAlso dtUtente.Rows.Count > 0 Then
                HttpContext.Current.Session("ASG_ProgressivoGIAS") = dtUtente.Rows(0).Item("ProgressivoGIAS")
                HttpContext.Current.Session("ASG_SuperUser_Password") = dtUtente.Rows(0).Item("Password_SuperUser")
            End If
        End If

        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim visualizza_codici_imp_app_prj As Boolean = False
        Dim visualizza_Kpin_BlockName As Boolean = False

        Dim dtVCodici = objImpost.Leggi2(2, objParametri_Server.SuperUserUsername,
                     enum_Impostazioni_Utenti.SuperUser_Visualizza_Codici_Anagrafici,
                     "", "", objParametri_Utenti)

        If dtVCodici.Rows.Count > 0 AndAlso dtVCodici.Rows(0)("Impostazione_Valore_1") = "1" Then
            visualizza_codici_imp_app_prj = True
        End If

        Dim dtVKPIN = objImpost.Leggi2(2, objParametri_Server.SuperUserUsername,
                     enum_Impostazioni_Utenti.SuperUser_KPIN_BlockName,
                     "", "", objParametri_Utenti)

        If dtVKPIN.Rows.Count > 0 AndAlso dtVKPIN.Rows(0)("Impostazione_Valore_1") = "1" Then
            visualizza_Kpin_BlockName = True
        End If

        Dim TipoOperazioni As New List(Of Integer)
        If jSonDatiTESTATA("tipoOperazione") IsNot Nothing AndAlso jSonDatiTESTATA("tipoOperazione").ToString <> "" Then
            Dim arr = JArray.Parse(jSonDatiTESTATA("tipoOperazione").ToString)
            For Each elem In arr
                TipoOperazioni.Add(CInt(elem))
            Next
        End If

        Dim impianti As New List(Of String)
        If jSonDatiTESTATA("impianti") IsNot Nothing AndAlso jSonDatiTESTATA("impianti").ToString <> "" Then
            Dim arr = JArray.Parse(jSonDatiTESTATA("impianti").ToString)
            For Each elem In arr
                impianti.Add(CStr(elem))
            Next
        End If

        Dim dtAgenda As DataTable = Carica_LavorazioniParallel(
            Piva:=piva, Sa_Cod:=sa_cod,
            DataDa:=Data_Selezionata1, DataA:=Data_Selezionata2,
            Veg_Cod:=veg_cod, id_cod:=id_cod, Cul_Cod:=cul_cod,
            Tipo:=tipo, Gru_Cod:=gru_cod, Lav_Cod:=lav_cod,
            Flag_TerrenoNudo:=flag_TerrenoNudo,
            xFiltroAggiuntivo_colturali:=xFiltroAggiuntivo_colturali,
            xFiltroAggiuntivo_postRaccolta:=xFiltroAggiuntivo_postRaccolta,
            xFiltroAggiuntivo_contabili:=xFiltroAggiuntivo_contabili,
            xFiltroAggiuntivo_contabili_Macchine:=xFiltroAggiuntivo_contabili_Macchine,
            xFiltroAggiuntivo_contabili_Audit:=xFiltroAggiuntivo_contabili_Audit,
            xOrderBy:="",
            objparametri_Server:=objParametri_Server,
            objparametri_Utenti:=objParametri_Utenti,
            FF_TrackedData_Cod:=-1,
            Visualizza_Codici_AppezzaImpianti:=visualizza_codici_imp_app_prj,
            Visualizza_KPIN_BlockName:=visualizza_Kpin_BlockName,
            TipoOperazioni:=TipoOperazioni,
            impianti:=impianti,
            xFiltroAggiuntivo_Visite:=xFiltroAggiuntivo_Visite,
            estraiPkImpianti:=estraiPkImpianti
        )
        r.RispostaOK = True

        If dtAgenda.Rows.Count > 0 OrElse TipoGriglia = "2" Then

            If TipoGriglia = "2" Then

                HandleGrigliaTipo2(dtAgenda, dtRA, mode, flag_NessunaSpecieQdC)

            End If

            If HttpContext.Current.Session IsNot Nothing Then
                HttpContext.Current.Session("dt") = dtAgenda
            End If

            Dim rispostaJson = DT_to_Json_Azienda(dtAgenda, TipoGriglia, objParametri_Server, visualizza_codici_imp_app_prj, visualizza_Kpin_BlockName, visualizza_colonna_Ricette, estraiPkImpianti)
            r.RispostaStringa = rispostaJson

        Else
            r.RispostaStringa = "Zero"

        End If

        Return r

    End Function

    Private Shared Sub caricaInSessioneImpostazioniutente()

        Dim paramUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni") = Leggi_Filtro_Tipo_GruppoOperazioni(paramUtenti)

        HttpContext.Current.Session("Filtro_Utente_Lavorazioni") = Leggi_Filtro_Utente_Lavorazioni(paramUtenti)

    End Sub

    Private Shared Function Leggi_Filtro_Tipo_GruppoOperazioni(paramUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        'FILTRO GRUPPO OPERAZIONI
        Dim Filtro_Tipo_GruppoOperazioni As String = ""
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT As DataTable = objUtentiImpostazioni.Leggi_Utente_Poi_SuperUser(
                                            enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_TIPI_GRUPPI_OPERAZIONI_VISIBILI_MENU_AGENDA, 1,
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "", "", paramUtenti)

        If DT.Rows.Count > 0 Then

            If Not IsDBNull(DT.Rows(0).Item("Impostazione_Valore_1")) AndAlso DT.Rows(0).Item("Impostazione_Valore_1") <> "" Then
                Dim tipi As String = DT.Rows(0).Item("Impostazione_Valore_1")
                Dim tipis() As String = tipi.Split("|")

                For i = 0 To tipis.Count - 1
                    If i > 0 Then
                        Filtro_Tipo_GruppoOperazioni &= " OR "
                    End If

                    Select Case (tipis(i))
                        Case "C", "E", "Z", "P", "V"
                            Filtro_Tipo_GruppoOperazioni &= " GruppoOperazioni.TIPO = '" & tipis(i) & "' "
                        Case "E6"
                            Filtro_Tipo_GruppoOperazioni &= " ( GruppoOperazioni.TIPO = 'E' AND GruppoOperazioni.GRU_COD = 6 ) "
                        Case "E10"
                            Filtro_Tipo_GruppoOperazioni &= " ( GruppoOperazioni.TIPO = 'E' AND GruppoOperazioni.GRU_COD = 10 ) "

                    End Select

                Next
            End If

        End If

        Return Filtro_Tipo_GruppoOperazioni

    End Function

    Private Shared Function Leggi_Filtro_Utente_Lavorazioni(paramUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        'FILTRO LAVORAZIONI
        Dim Filtro_Utente_Lavorazioni As String = ""
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT As DataTable = objUtentiImpostazioni.Leggi(
                                        enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI, 1,
                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                        "", "", paramUtenti)

        If DT.Rows.Count > 0 Then
            Filtro_Utente_Lavorazioni = " agenda.Lav_Cod in ("
            For i = 0 To DT.Rows.Count - 1
                If i <> 0 Then
                    Filtro_Utente_Lavorazioni &= " ,"
                End If
                Filtro_Utente_Lavorazioni &= DT.Rows(i).Item("ID_0")
            Next
            Filtro_Utente_Lavorazioni &= " )  "

        End If

        Return Filtro_Utente_Lavorazioni

    End Function

    ''' <summary>
    ''' Restituisce il formato griglia json
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="TipoGriglia">1 = no tracciabilità, 2 = tracciabilità</param>
    ''' <returns></returns>
    Public Shared Function DT_to_Json_Azienda(ByVal dt As DataTable,
                                              ByVal TipoGriglia As String,
                                              ByVal objParametri_Server As AgronicaCoreParametri,
                                              ByVal visualizza_codiciImp As Boolean,
                                              ByVal visualizza_KPIN_BlockName As Boolean,
                                              ByVal visualizza_colonna_Ricette As Boolean,
                                              ByVal visualizza_PkImpianti As Boolean) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome


        If TipoGriglia = "2" Then
            c = New ColonneNome("id_mov_det", "id_mov_det", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("id_agenda", "Sel.", "string")
            c._Filtrabile = False
            c._ColonnaDiSelezione = True
            c._hidden = True
            l.Add(c)
        End If

        If TipoGriglia = "1" Then
            c = New ColonneNome("FF_Track_Cal_Cod_Padre", "FF_Track_Cal_Cod_Padre", "string")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("FF_Track_Cal_Cod", "FF_Track_Cal_Cod_Padre", "string")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("FF_Track_Lotto_Padre", Gias.LottoPadre, "string")
            'c._hidden = True
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)

            c = New ColonneNome("FF_Track_Lotto", Gias.Lotto, "string")
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)
        End If

        If TipoGriglia = "2" Then

            'Aggiunto questo flag per visualizzare o meno la colonna delle Ricette perchè la Trattamenti_2 la crea lato client per aggiungere il bottone
            'del ribaltamento dell'Agenda in Ricetta
            If visualizza_colonna_Ricette Then
                l.Add(New ColonneNome("Ricetta_Des", Gias.Ricette, "string"))
            End If

            l.Add(New ColonneNome("Data2", Gias.Data, "date"))
            l.Add(New ColonneNome("Lav_Des", Gias.Operazione, "string"))
            l.Add(New ColonneNome("Specie", Gias.Specie, "string") With {._Display = False})
            l.Add(New ColonneNome("cul_Des", Gias.Varieta, "string") With {._Display = False})
            l.Add(New ColonneNome("Specie_Varieta", Gias.Specie & " " & Gias.Varieta, "string")) '"Specie Varietà"
            l.Add(New ColonneNome("Prodotti_Utilizzati", My.Resources.AgronicaCoreContabBIZ.ProdottiUtilizzati, "string") With {._Display = False})
            l.Add(New ColonneNome("Avversita", Gias.Avversita, "string") With {._Display = False})
            l.Add(New ColonneNome("Dettaglio_Tecnico", Gias.DettaglioTecnico, "string"))
            l.Add(New ColonneNome("Centro_Aziendale", Gias.CentroAziendale, "string") With {._Display = False})
            l.Add(New ColonneNome("Centro_Campo", Gias.Campo, "string")) '"Centri Campi"
            l.Add(New ColonneNome("Appezzamenti_Coinvolti", My.Resources.AgronicaCoreContabBIZ.AppezzamentiCoinvolti, "string"))

            If dt.Select("ProdottiMagazzinoTrattati_Coinvolti IS NOT NULL AND ProdottiMagazzinoTrattati_Coinvolti <> ''").FirstOrDefault IsNot Nothing Then
                l.Add(New ColonneNome("ProdottiMagazzinoTrattati_Coinvolti", My.Resources.AgronicaCoreContabBIZ.ProdottiMagazzinoTrattatiCoinvolti, "string") With {._Display = False})
                l.Add(New ColonneNome("ProdottiMagazzinoTrattati_Lotti", My.Resources.AgronicaCoreContabBIZ.ProdottiMagazzinoTrattatiLotti, "string") With {._Display = False})
                l.Add(New ColonneNome("ProdottiMagazzinoTrattati_QuantitaQuintali", My.Resources.AgronicaCoreContabBIZ.ProdottiMagazzinoTrattatiQuantitaTotaleTrattata, "string") With {._Display = False})
            End If

            l.Add(New ColonneNome("LottiImpianto", Gias.LottiImpianto, "string") With {._Display = False})
            l.Add(New ColonneNome("Sup_Trattata", Gias.SuperficieMovimentata, "string") With {._Display = False, ._FormatoParticolare = "#= kendo.format('{0}',Sup_Trattata) #"}) 'kendo.toString(Sup_Trattata,'n')
            'l.Add(New ColonneNome("Sup_Trattata", "Superficie Movimentata", "number") With {._Display = False, ._FormatoParticolare = "#=(Sup_Trattata === 0) ? '' : Sup_Trattata.toString().replace('.', ',')#"}) 'kendo.toString(Sup_Trattata,'n')
            'l.Add(New ColonneNome("Sup_Trattata", "Superficie Movimentata", "number") With {._Display = False}) 'kendo.toString(Sup_Trattata,'n')
            l.Add(New ColonneNome("chiave_composita", "chiave_composita", "string") With {._hidden = True})
            l.Add(New ColonneNome("tipo", "tipo", "string") With {._hidden = True}) 'identifica il tipo di operazione (per colorare le righe)
            l.Add(New ColonneNome("Ricetta_Cod", "Ricetta_Cod", "string") With {._hidden = True})
            l.Add(New ColonneNome("Ricetta_Des", "Ricetta_Des", "string") With {._hidden = True})
            l.Add(New ColonneNome("Data", "Data_Stringa", "string") With {._hidden = True})
            l.Add(New ColonneNome("contabilizzato", "contabilizzato", "number") With {._hidden = True})
            l.Add(New ColonneNome("LottiProduzione", Gias.LottiProduzione, "string") With {._Display = False})
            l.Add(New ColonneNome("Note", Gias.Note, "string") With {._Display = False})
            l.Add(New ColonneNome("Costi_Operatori", Gias.Operatori, "string") With {._Display = False})
            l.Add(New ColonneNome("Costi_Macchine", Gias.Macchine, "string") With {._Display = False})
            l.Add(New ColonneNome("Creatore_Intervento", My.Resources.AgronicaCoreContabBIZ.CreatoreIntervento, "string") With {._Display = False})
            l.Add(New ColonneNome("Data_Ultima_Modifica_Intervento", My.Resources.AgronicaCoreContabBIZ.DataUltimaModificaIntervento, "date") With {._Display = False})
            l.Add(New ColonneNome("Raccoglitore_Cod", Gias.CodMultiAttivita, "string") With {._Display = False})
            l.Add(New ColonneNome("ID", "ID", "string") With {._Display = False})
            'l.Add(New ColonneNome("Tipo_Ricetta", "Tipo_Ricetta", "number") With {._hidden = True})

            l.Add(New ColonneNome("Id_Agenda_Visita", "Id_Agenda_Visita", "number") With {._hidden = True})

            If visualizza_codiciImp Then
                l.Add(New ColonneNome("Codici_Impianto", Gias.CodiciImpianto, "string") With {._Display = False})
                l.Add(New ColonneNome("Codici_Appezzamenti", Gias.CodiciAppezzamento, "string") With {._Display = False})
            End If

            If visualizza_KPIN_BlockName Then
                l.Add(New ColonneNome("KPIN", "KPIN", "string") With {._Display = False})
                l.Add(New ColonneNome("BlockName", "Block Name", "string") With {._Display = False})
            End If

            l.Add(New ColonneNome("Installazione_Trappola_Reinnescata", "Installazione_Trappola_Reinnescata", "number") With {._hidden = True})

            l.Add(New ColonneNome("Cau_Mov", "Cau_Mov", "string") With {._hidden = True})

        Else
            l.Add(New ColonneNome("Data", Gias.Data, "date"))
        End If

        If TipoGriglia = "1" Then
            c = New ColonneNome("Lav_des", Gias.Descrizione, "string") '"Descrizione"
            c._RemoveHtmlEncode = True
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)

            c = New ColonneNome("Dettagli", Gias.Dettagli, "string") '"Dettagli"
            c._RemoveHtmlEncode = True
            c._Filtrabile = True
            l.Add(c)

            c = New ColonneNome("NomeComune", My.Resources.AgronicaCoreContabBIZ.CategoriaProdotto, "string") '"Categoria Prodotto"
            c._RemoveHtmlEncode = True
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)

            c = New ColonneNome("FF_Referenza", My.Resources.AgronicaCoreContabBIZ.Prodotto, "string") ' "Prodotto"
            c._RemoveHtmlEncode = True
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)

            Dim objConfigDettagli As AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R = New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
            Dim DTParamQual As DataTable = objConfigDettagli.Leggi(CStr(dt.Rows(0).Item("piva")), 0, False, "Tipo = 1", "", objParametri_Server)

            For Each paramQual In DTParamQual.Rows
                If paramQual("Tipo") = 1 Then
                    If Not ({"cliente"}).Contains(paramQual("Tabella_Key").ToString.ToLower) Then
                        c = New ColonneNome("FF_" & paramQual("Tabella_Key"), paramQual("Tabella_Key"), "string")
                        c._Filtrabile = True
                        c._FiltrabileConCheck = True
                        l.Add(c)
                    End If
                End If
            Next

        End If

        l.Add(New ColonneNome("Lav_cod", "Lav_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("blocco_flag", "blocco_flag", "string") With {._hidden = True})
        l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
        l.Add(New ColonneNome("Veg_cod", "veg_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("sa_cod", "sa_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("Operazione_DES", "Operazione_DES", "string") With {._hidden = True})
        l.Add(New ColonneNome("gru_des", "gru_des", "string") With {._hidden = True})
        l.Add(New ColonneNome("Origine", "Origine", "string") With {._hidden = True})

        If TipoGriglia = "1" Then
            c = New ColonneNome("FF_Track_Qta_Extra_Totale", Gias.Quantita, "number")
            c._Filtrabile = True
            c._formatNr = "n0"
            l.Add(c)

            c = New ColonneNome("FF_Track_Qta_Contenitori", Gias.Contenitori, "number")
            c._Filtrabile = True
            c._formatNr = "n0"
            l.Add(c)

            c = New ColonneNome("FF_Track_Qta_Imballi", Gias.Imballi, "number")
            c._Filtrabile = True
            c._formatNr = "n0"
            l.Add(c)
        End If

        c = New ColonneNome("Rag_Soc", Gias.Azienda, "string")
        If TipoGriglia = "2" Then
            c._hidden = True
        Else
            c._Filtrabile = True
            c._FiltrabileConCheck = True
        End If
        l.Add(c)

        If TipoGriglia = "1" Then
            c = New ColonneNome("FF_Righe_Aggiunte", Gias.RigheAggiunte, "string")
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)

            c = New ColonneNome("FF_Operazioni_Campagna", Gias.OperazioniCampagna, "string")
            c._Filtrabile = True
            c._RemoveHtmlEncode = True
            c._FiltrabileConCheck = True
            l.Add(c)
        End If

        l.Add(New ColonneNome("PermessoModifica", "PermessoModifica", "string") With {._hidden = True})
        l.Add(New ColonneNome("Descrizione_Unica", Gias.DescrizioneUnica, "string") With {._Display = False, ._RemoveHtmlEncode = True})

        If dt.Columns.Contains("AggiungiAlPua") = True Then
            l.Add(New ColonneNome("AggiungiAlPua", "AggiungiAlPua", "string") With {._hidden = True})
        End If

        l.Add(New ColonneNome("Data_Creazione", Gias.DataCreazione, "date") With {._Display = False, ._width = "97px"})

        If visualizza_PkImpianti Then
            l.Add(New ColonneNome("PK_Impianti", "PK_Impianti", "string") With {._hidden = True})
        End If

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_KendoOpt(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, inParallelo:=False)

        Return risp

    End Function


    Public Shared Function DT_to_Json_Esportazione_Enogis(ByVal dt As DataTable,
                                              ByVal visualizza_codiciImp As Boolean,
                                              ByVal visualizza_KPIN_BlockName As Boolean) As String

        ' TODO Razvan da aggiungere un campo 'inviato' nel dataTable.
        Dim js As New JSON_DataTable

        ' TODO Razvan Da togliere questa parte appena abbiamo i dati veri
        'TemporaryFnFeedData(dt)

        Dim risp As String = js.JSON_DataTable_KendoOpt(dt, EnogisCaricaColonneOperazioniInAttesaDInvio(dt, visualizza_codiciImp, visualizza_KPIN_BlockName), tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

        Return risp

    End Function

    Public Shared Function TemporaryFnFeedData(dt As DataTable)
        dt.Columns.Add("Bloccato", GetType(String))
        dt.Columns.Add("Inviato", GetType(String))

        For Each row In dt.Rows
            ' If dt.Rows.IndexOf(row) Mod 2 = 0 OrElse Not row("GestitoDaSistemiEsterni") Then
            If Not row("GestitoDaSistemiEsterni") Then
                row("Bloccato") = "Si"
                row("Inviato") = "No"
            Else
                row("Bloccato") = "No"
                'row("Inviato") = "No"
            End If
        Next
    End Function

    Public Shared Function EnogisCaricaColonneOperazioniInAttesaDInvio(ByVal dt As DataTable,
                                                                       ByVal visualizza_codiciImp As Boolean,
                                                                       ByVal visualizza_KPIN_BlockName As Boolean) As List(Of ColonneNome)
        Dim cols As New List(Of ColonneNome)
        Dim c As ColonneNome

        cols.Add(New ColonneNome("Inviato", "Inviato", "string"))
        cols.Add(New ColonneNome("Bloccato", "Bloccato", "string"))


        c = New ColonneNome("id_mov_det", "id_mov_det", "number")
        c._hidden = True
        cols.Add(c)

        c = New ColonneNome("id_agenda", "Sel.", "string")
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        c._hidden = True
        cols.Add(c)

        cols.Add(New ColonneNome("Rag_Soc", "Azienda", "string"))
        cols.Add(New ColonneNome("Data2", "Data", "date"))
        cols.Add(New ColonneNome("Lav_Des", "Operazione", "string"))
        cols.Add(New ColonneNome("Specie", Gias.Specie, "string") With {._Display = False})
        cols.Add(New ColonneNome("cul_Des", Gias.Varieta, "string") With {._Display = False})
        cols.Add(New ColonneNome("Specie_Varieta", Gias.Specie & " " & Gias.Varieta, "string")) '"Specie Varietà"
        cols.Add(New ColonneNome("Prodotti_Utilizzati", My.Resources.AgronicaCoreContabBIZ.ProdottiUtilizzati, "string") With {._Display = False})
        cols.Add(New ColonneNome("Avversita", "Avversita", "string") With {._Display = False})
        cols.Add(New ColonneNome("Dettaglio_Tecnico", "Dettaglio Tecnico", "string"))
        cols.Add(New ColonneNome("Centro_Aziendale", My.Resources.AgronicaCoreContabBIZ.CentroAziendale, "string") With {._Display = False})
        cols.Add(New ColonneNome("Centro_Campo", My.Resources.AgronicaCoreContabBIZ.Campo, "string")) '"Centri Campi"
        cols.Add(New ColonneNome("Appezzamenti_Coinvolti", My.Resources.AgronicaCoreContabBIZ.AppezzamentiCoinvolti, "string"))
        cols.Add(New ColonneNome("LottiImpianto", "Lotti Impianto", "string") With {._Display = False})
        cols.Add(New ColonneNome("Sup_Trattata", "Superficie Movimentata", "string") With {._Display = False, ._FormatoParticolare = "#= kendo.format('{0}',Sup_Trattata) #"}) 'kendo.toString(Sup_Trattata,'n')
        cols.Add(New ColonneNome("chiave_composita", "chiave_composita", "string") With {._hidden = True})
        cols.Add(New ColonneNome("tipo", "tipo", "string") With {._hidden = True}) 'identifica il tipo di operazione (per colorare le righe)
        cols.Add(New ColonneNome("Ricetta_Cod", "Ricetta_Cod", "string") With {._hidden = True})
        cols.Add(New ColonneNome("Ricetta_Des", "Ricetta_Des", "string") With {._hidden = True})
        cols.Add(New ColonneNome("Data", "Data_Stringa", "string") With {._hidden = True})
        cols.Add(New ColonneNome("contabilizzato", "contabilizzato", "number") With {._hidden = True})
        cols.Add(New ColonneNome("LottiProduzione", "Lotti di Produzione", "string") With {._Display = False})
        cols.Add(New ColonneNome("Note", "Note", "string") With {._Display = False})
        cols.Add(New ColonneNome("Costi_Operatori", "Operatori", "string") With {._Display = False})
        cols.Add(New ColonneNome("Costi_Macchine", "Macchine", "string") With {._Display = False})
        cols.Add(New ColonneNome("Creatore_Intervento", "Creatore Intervento", "string") With {._Display = False})
        cols.Add(New ColonneNome("Data_Ultima_Modifica_Intervento", "Data Ultima Modifica Intervento", "date") With {._Display = False})
        cols.Add(New ColonneNome("ID", "ID", "string") With {._Display = False})

        cols.Add(New ColonneNome("chiave_sincro_ARTEA", "chiave_sincro_ARTEA", "string") With {._hidden = True})

        If visualizza_codiciImp Then
            cols.Add(New ColonneNome("Codici_Impianto", "Codici Impianto", "string") With {._Display = False})
            cols.Add(New ColonneNome("Codici_Appezzamenti", "Codici Appezzamento", "string") With {._Display = False})
        End If

        If visualizza_KPIN_BlockName Then
            cols.Add(New ColonneNome("KPIN", "KPIN", "string") With {._Display = False})
            cols.Add(New ColonneNome("BlockName", "Block Name", "string") With {._Display = False})
        End If



        cols.Add(New ColonneNome("Lav_cod", "Lav_cod", "string") With {._hidden = True})
        cols.Add(New ColonneNome("blocco_flag", "blocco_flag", "string") With {._hidden = True})
        cols.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
        cols.Add(New ColonneNome("Veg_cod", "veg_cod", "string") With {._hidden = True})
        cols.Add(New ColonneNome("sa_cod", "sa_cod", "string") With {._hidden = True})
        cols.Add(New ColonneNome("Operazione_DES", "Operazione_DES", "string") With {._hidden = True})
        cols.Add(New ColonneNome("gru_des", "gru_des", "string") With {._hidden = True})




        cols.Add(New ColonneNome("PermessoModifica", "PermessoModifica", "string") With {._hidden = True})
        cols.Add(New ColonneNome("Descrizione_Unica", "Descrizione Unica", "string") With {._Display = False, ._RemoveHtmlEncode = True})

        If dt.Columns.Contains("AggiungiAlPua") = True Then
            cols.Add(New ColonneNome("AggiungiAlPua", "AggiungiAlPua", "string") With {._hidden = True})
        End If

        For Each col In cols
            If col._Nome_colonna_DT = "id_agenda" Then
                Continue For
            End If

            col._Filtrabile = True
            If col._Nome_colonna_DT <> "Data2" Then
                col._FiltrabileConCheck = True
            End If
        Next


        Return cols
    End Function

    Private Shared Sub SetCompositeKey(ByRef dr As DataRow)
        If Not IsDBNull(dr.Item("Dettagli")) Then
            Dim delim As String() = New String(0) {"<br>"}
            Dim dettagli As String = dr.Item("Dettagli")
            Dim dett = dettagli.Split(delim, StringSplitOptions.None)

            For count = 0 To dett.Length - 1

                If InStr(dett(count), ":") Then
                    Dim dett2 As String() = dett(count).Split(":")

                    If dett(count) <> "" Then
                        Dim final_s As String = dett2(1).Replace("</b> ", "")

                        Select Case dett2(0)
                            Case "<b>Centro Az."
                                dr.Item("Centro_Aziendale") = final_s
                            Case "<b>Specie"
                                dr.Item("Specie") = final_s
                            Case "<b>Appezzamenti Coinvolti"
                                dr.Item("Appezzamenti_Coinvolti") = final_s
                            Case "<b>Prodotti Utilizzati"
                                dr.Item("Prodotti_Utilizzati") = final_s
                            Case " <b> Avversità"
                                final_s = final_s.Replace("-", "")
                                dr.Item("Avversita") = final_s
                        End Select

                    End If

                End If
            Next
        End If

        dr.Item("chiave_composita") = dr.Item("Data2") & "_" & dr.Item("Id_Agenda") & "_" & dr.Item("Lav_Cod") & "_" &
                                      dr.Item("Piva") & "_" & dr.Item("Sa_Cod") & "_" & dr.Item("Blocco_Flag") & "_" &
                                      dr.Item("Veg_Cod")
    End Sub

    Private Shared Sub SetChiaveSincroARTEA(ByRef dr As DataRow)
        dr.Item("chiave_sincro_ARTEA") = (CDate(dr.Item("Data2")).ToShortDateString()).Replace("/", "-") & "_" &
                                          dr.Item("Id_Agenda") & "_" & dr.Item("Lav_Cod") & "_" &
                                          dr.Item("Piva") & "_" & dr.Item("Sa_Cod") & "_" &
                                          dr.Item("Blocco_Flag") & "_" & dr.Item("Veg_Cod")
    End Sub

    ''' <summary>
    ''' Imposta le descrizioni delle colture per le operazioni senza una specie associata.
    ''' </summary>
    ''' <param name="dr">La riga di dati da aggiornare.</param>
    Private Shared Sub SetDesForOperationsWithoutSpecies(ByRef dr As DataRow)
        Dim isAdmissibleSurvey = Movimenti_R._admissibleOperationsWithoutSpecies.Contains(dr.Item("Lav_Cod"))
        If isAdmissibleSurvey AndAlso dr.Item("Veg_Cod") = 0 AndAlso
            dr.Item("PK_Impianti").ToString.EndsWith(",0,0") Then

            dr.Item("cul_des") = My.Resources.AgronicaCoreContabBIZ.NessunaSpecie
            dr.Item("Specie_Varieta") = My.Resources.AgronicaCoreContabBIZ.NessunaSpecie
            dr.Item("Specie") = My.Resources.AgronicaCoreContabBIZ.NessunaSpecie
        End If
    End Sub

    Public Shared Sub EstendiDatatable(ByRef dt As DataTable)
        Dim shouldSetCompositeKey As Boolean = False
        Dim shouldSetChiaveSincroARTEA As Boolean = False

        If Not dt.Columns.Contains("chiave_composita") Then
            dt.Columns.Add("Centro_Aziendale", GetType(String))
            dt.Columns.Add("Specie", GetType(String))
            dt.Columns.Add("Appezzamenti_Coinvolti", GetType(String))
            dt.Columns.Add("Prodotti_Utilizzati", GetType(String))
            dt.Columns.Add("Avversita", GetType(String))

            dt.Columns.Add("chiave_composita", GetType(String))
            shouldSetCompositeKey = True
        End If

        If Not dt.Columns.Contains("chiave_sincro_ARTEA") Then
            dt.Columns.Add("chiave_sincro_ARTEA", GetType(String))
            shouldSetChiaveSincroARTEA = True
        End If

        'If Not dt.Columns.Contains("Tipo_Ricetta") Then
        '    dt.Columns.Add("Tipo_Ricetta", GetType(Integer))
        '    Dim ricetteCods As New List(Of Integer)
        '    Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
        '    For i = 0 To dt.Rows.Count - 1
        '        Dim codiceRicetta = dt.Rows(i).Item("Ricetta_Cod")
        '        If codiceRicetta <> 0 Then
        '            ricetteCods.Add(codiceRicetta)
        '        End If
        '    Next
        '    Dim codXTipo = objOperazioni.Leggi_TipoXRicettaCod(ricetteCods, objParametri_Server)
        '    Dim hashSet As New Dictionary(Of Integer, Integer)
        '    For i = 0 To codXTipo.Rows.Count - 1
        '        If Not hashSet.ContainsKey(codXTipo.Rows(i).Item("Ricetta_Cod")) Then
        '            hashSet.Add(codXTipo.Rows(i).Item("Ricetta_Cod"), codXTipo.Rows(i).Item("W_Anagrafica_Stati_Cod"))
        '        End If
        '    Next
        '    For i = 0 To dt.Rows.Count - 1
        '        dt.Rows(i).Item("Tipo_Ricetta") = If(hashSet.ContainsKey(dt.Rows(i).Item("Ricetta_Cod")),
        '                                                            hashSet.Item(dt.Rows(i).Item("Ricetta_Cod")),
        '                                                            0)
        '    Next
        'End If

        For i = 0 To dt.Rows.Count - 1
            If shouldSetCompositeKey Then
                SetCompositeKey(dt.Rows(i))
            End If
            If shouldSetChiaveSincroARTEA Then
                SetChiaveSincroARTEA(dt.Rows(i))
            End If

            SetDesForOperationsWithoutSpecies(dt.Rows(i))
        Next

    End Sub

    'Funzione che crea un task per ogni processore
    'A cui viene passata una porzione di Datatable
    'I datatable vengono poi ricongiunti 
    'al termine di tutti i task
    Public Shared Function Carica_LavorazioniParallel(
            ByVal Piva As String,
            ByVal Sa_Cod As Integer,
            ByVal DataDa As Date,
            ByVal DataA As Date,
            ByVal Veg_Cod As Integer,
            ByVal id_cod As Integer,
            ByVal Cul_Cod As Integer,
            ByVal Tipo As String,
            ByVal Gru_Cod As Integer,
            ByVal Lav_Cod As Integer,
            ByVal Flag_TerrenoNudo As Boolean,
            ByVal xFiltroAggiuntivo_colturali As String,
            ByVal xFiltroAggiuntivo_postRaccolta As String,
            ByVal xFiltroAggiuntivo_contabili As String,
            ByVal xFiltroAggiuntivo_contabili_Macchine As String,
            ByVal xFiltroAggiuntivo_contabili_Audit As String,
            ByVal xOrderBy As String,
            ByVal objparametri_Server As AgronicaCoreParametri,
            ByVal objparametri_Utenti As AgronicaCoreParametri,
            ByVal FF_TrackedData_Cod As Integer,
            Optional ByVal FromOutToIn As Boolean = True,
            Optional ByVal cCertificazione As Integer = True,
            Optional ByVal righeAggiunte As String = "",
            Optional ByVal Visualizza_Codici_AppezzaImpianti As Boolean = False,
            Optional ByVal Visualizza_KPIN_BlockName As Boolean = False,
            Optional ByVal TipoOperazioni As List(Of Integer) = Nothing,
            Optional ByVal impianti As List(Of String) = Nothing,
            Optional ByVal expARTEA_1Magazzini_2QdC As Integer = -1,
            Optional ByVal filtroAgende As List(Of Integer) = Nothing,
            Optional ByVal numeroDiRigheDaEstrarre As Integer? = Nothing,
            Optional ByVal estraiPkImpianti As Boolean? = Nothing,
            Optional ByVal xFiltroAggiuntivo_Visite As String = ""
        ) As DataTable

        Dim DtAgenda As New DataTable

        Dim i As Integer

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date

        Dim objConfigDettagli As AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R = New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
        Dim DTParamQual As DataTable = objConfigDettagli.Leggi(Piva, 0, False, "Tipo = 1", "", objparametri_Server)

        Validita_Inizio = If(DataDa >= objparametri_Server.FinestraTemporaleInizio, DataDa, objparametri_Server.FinestraTemporaleInizio)
        Validita_Fine = If(DataA <= objparametri_Server.FinestraTemporaleFine, DataA, objparametri_Server.FinestraTemporaleFine)


        Dim filtro As String = "|"
        If FF_TrackedData_Cod <= 0 Then
            If HttpContext.Current.Session IsNot Nothing AndAlso Not IsNothing(HttpContext.Current.Session("Filtro")) AndAlso HttpContext.Current.Session("Filtro") <> "" Then
                'No un filtro
                filtro = HttpContext.Current.Session("Filtro")
            Else
                ' In Angular non gestiamo i filtri che nell'app precedente veniva gestito
            End If
        End If

        Dim Filtro_Tipo_GruppoOperazioni As String = ""
        Dim Filtro_ElemCod As String = ""
        If FF_TrackedData_Cod <= 0 Then
            If HttpContext.Current.Session IsNot Nothing AndAlso Not IsNothing(HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")) AndAlso HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni") <> "" Then
                Filtro_Tipo_GruppoOperazioni = HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni")
            Else
                ' Utilizzato da NgGias, nel proggetto GiasOnline2010 veniva caricato in caricaInSessioneImpostazioniutente con ASG_objParametri_Utenti
                ' Se ASG_objParametri_Utenti non è objparametri_Utenti allora questa parte è sbagliata.
                Filtro_Tipo_GruppoOperazioni = Leggi_Filtro_Tipo_GruppoOperazioni(objparametri_Utenti)
            End If
        End If


        Dim Filtro_Utente_Lavorazioni As String = ""
        If FF_TrackedData_Cod <= 0 Then
            If HttpContext.Current.Session IsNot Nothing AndAlso Not IsNothing(HttpContext.Current.Session("Filtro_Utente_Lavorazioni")) AndAlso HttpContext.Current.Session("Filtro_Utente_Lavorazioni") <> "" Then
                Filtro_Utente_Lavorazioni = HttpContext.Current.Session("Filtro_Utente_Lavorazioni")

            Else
                ' Razvan Utilizzato da NgGias, nel proggetto GiasOnline2010 veniva caricato in caricaInSessioneImpostazioniutente con ASG_objParametri_Utenti
                ' Se ASG_objParametri_Utenti non è objparametri_Utenti allora questa parte è sbagliata.
                Filtro_Utente_Lavorazioni = Leggi_Filtro_Utente_Lavorazioni(objparametri_Utenti)
            End If

        End If

        Dim filtrolavorazioni = filtro.Split("|")(0)

        If Filtro_Utente_Lavorazioni <> "" Then
            If filtrolavorazioni <> "" Then
                filtrolavorazioni = " ( " & filtrolavorazioni & " ) And (" & Filtro_Utente_Lavorazioni & ") "
            Else
                filtrolavorazioni = Filtro_Utente_Lavorazioni
            End If
        End If

        'Identifico se è abilitata l'operazione di cura
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Tipo_Raccolta_Val As String = ObjUtenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_RACCOLTA_TIPO, objparametri_Utenti)
        Dim bool_isCuraEnabled As Boolean = (IsNumeric(Tipo_Raccolta_Val) AndAlso Tipo_Raccolta_Val = enum_RACCOLTA_TIPO.Raccolta_e_Cura)

        If (Veg_Cod <> 0 AndAlso Veg_Cod <> -1) Or id_cod <> 0 Then
            If Filtro_Tipo_GruppoOperazioni <> "" Then
                Filtro_Tipo_GruppoOperazioni = "( " & Filtro_Tipo_GruppoOperazioni & " ) AND "
            End If
            Filtro_Tipo_GruppoOperazioni += " GruppoOperazioni.Tipo = 'C'  "
        End If

        If expARTEA_1Magazzini_2QdC <> -1 Then
            If expARTEA_1Magazzini_2QdC = 1 Then
                Filtro_Tipo_GruppoOperazioni = "GruppoOperazioni.Gru_Cod IN (6,10) "
                Filtro_ElemCod = "Movimenti_dettagli.Elem_Cod in (" & FORMULATI & ", " & FERTILIZZANTI & ") "
            Else
                Filtro_Tipo_GruppoOperazioni = "GruppoOperazioni.Gru_Cod IN (1,2,3,4)"
            End If
        End If

        If Filtro_Tipo_GruppoOperazioni <> "" AndAlso TipoOperazioni.Count > 0 Then
            Filtro_Tipo_GruppoOperazioni = "( " & Filtro_Tipo_GruppoOperazioni & " ) AND "
            Dim filtroTipoOperazioni_str = " GruppoOperazioni.Gru_Cod IN ("
            For Each Tipos In TipoOperazioni
                filtroTipoOperazioni_str &= Tipos & ","
            Next
            filtroTipoOperazioni_str = filtroTipoOperazioni_str.Substring(0, filtroTipoOperazioni_str.Length - 1)
            filtroTipoOperazioni_str &= ") "
            Filtro_Tipo_GruppoOperazioni &= filtroTipoOperazioni_str
        End If

        Dim strFiltroAgenda As String = ""
        If impianti IsNot Nothing AndAlso impianti.Count > 0 Then
            Dim DT_ID_Agenda As DataTable
            Dim hasValue = Function(k) IsNumeric(k) AndAlso Not Double.IsNaN(k)

            Dim objMov_Destinazioni As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            For Each imp In impianti
                Dim impArr = imp.Split("_")
                Dim dtAgImp = objMov_Destinazioni.Leggi_DistinctID_Agenda_Impianti(
                    objparametri_Server,
                    Piva:=impArr(0),
                    Sa_Cod:=If(hasValue(impArr(1)), impArr(1), 0),
                    Appezza:=If(hasValue(impArr(2)), impArr(2), 0),
                    Id_Reg:=If(hasValue(impArr(3)), impArr(3), 0),
                    Validita_Inizio, Validita_Fine,
                    FiltroAggiuntivo:="", Ordinamento:=""
                )

                If DT_ID_Agenda Is Nothing Then
                    DT_ID_Agenda = dtAgImp.Copy
                Else
                    DT_ID_Agenda.Merge(dtAgImp)
                End If

            Next

            If DT_ID_Agenda.Rows.Count > 0 Then
                DT_ID_Agenda = DT_ID_Agenda.DefaultView.ToTable(True, "ID_Agenda")
                strFiltroAgenda = " Agenda.ID_Agenda IN ( "
                For Each rowAgImp In DT_ID_Agenda.Rows
                    strFiltroAgenda &= rowAgImp(0) & ","
                Next
                strFiltroAgenda = strFiltroAgenda.Substring(0, strFiltroAgenda.Length - 1)
                strFiltroAgenda &= ") "
                If filtrolavorazioni = "" Then
                    filtrolavorazioni = strFiltroAgenda
                Else
                    filtrolavorazioni &= "AND " & strFiltroAgenda
                End If
            Else
                strFiltroAgenda = " Agenda.ID_Agenda = 0 "
                If filtrolavorazioni = "" Then
                    filtrolavorazioni = strFiltroAgenda
                Else
                    filtrolavorazioni &= "AND " & strFiltroAgenda
                End If
            End If

        End If

        Try
            Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
            DtAgenda = objOperazioni.Leggi_x_Grid_Agenda_BS_Fast_Senza_Avversita(
                Piva, Sa_Cod, Validita_Inizio, Validita_Fine,
                Veg_Cod, Cul_Cod, Tipo, Gru_Cod, Lav_Cod, Flag_TerrenoNudo, True,
                filtrolavorazioni, filtro.Split("|")(1),
                Filtro_Tipo_GruppoOperazioni, xFiltroAggiuntivo_colturali,
                xFiltroAggiuntivo_postRaccolta, xFiltroAggiuntivo_contabili,
                xFiltroAggiuntivo_contabili_Macchine, xFiltroAggiuntivo_contabili_Audit,
                xOrderBy,
                objparametri_Utenti, ' Razvan Nella versione vecchia veniva utilizzato HttpContext.Current.Session("ASG_objParametri_Utenti"),
                objparametri_Server, ' Nella versione vecchia veniva utilizzato HttpContext.Current.Session("ASG_objParametri_Server"),
                FF_TrackedData_Cod, FromOutToIn,
                Visualizza_Codici_AppezzaImpianti:=Visualizza_Codici_AppezzaImpianti,
                Visualizza_KPIN_BlockName:=Visualizza_KPIN_BlockName,
                id_cod:=id_cod,
                filtroAgende:=filtroAgende,
                numeroDiRigheDaEstrarre:=numeroDiRigheDaEstrarre,
                Filtro_ElemCod:=Filtro_ElemCod,
                xFiltroAggiuntivo_Visite:=xFiltroAggiuntivo_Visite
            )
        Catch ex As Exception
            Return Nothing
        End Try

        Dim shouldShowContabOp As String = ObjUtenti.LeggiValoreImpostazioneScalare(
            enum_Impostazioni_Utenti.Mostra_DDT_MenuAgenda, objparametri_Utenti.UtenteUsername, objparametri_Utenti)
        If shouldShowContabOp = "0" Then
            Dim rows = DtAgenda.Select("Tipo <> 'E'")
            DtAgenda = If(rows.Any(), rows.CopyToDataTable, DtAgenda.Clone)
        End If

        '----------------------------------
        'Leggo tutti i principi attivi
        Dim HtProdPA As New Hashtable()
        Dim HtPrincAtt As Hashtable = estraiPrincipiAttivi(DtAgenda, HtProdPA, objparametri_Server, objparametri_Utenti)

        '03/01/2018 Grilli: Leggo le Avversità fuori dalla megaLettura
        Dim objMovTec As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
        Dim dtMovDetTec As DataTable = objMovTec.Leggi_x_agenda_Con_Id_Mov_Det(Piva, "", "", objparametri_Server)

        '(12/11/2018 fede) aggiunta indicazione fase fenologica (splittate le operazioni)
        'leggo le fasi via web service
        Dim dtMovDetTecFasi As DataTable = objMovTec.Leggi_x_agenda_fasifenologiche(Piva, "", "", objparametri_Server)


        Dim objParametriUscitaFasiNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
        Dim objParametriUscitaFasiOld As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output

        If Not dtMovDetTecFasi Is Nothing AndAlso dtMovDetTecFasi.Rows.Count > 0 Then

            Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
            Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS

            Dim Filtro_cod_ss As String = ""
            Dim Filtro_ff_cod As String = ""
            Dim Hash_cod_ss As New Hashtable
            Dim Hash_ff_cod As New Hashtable
            Dim Leggi_impostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE, objparametri_Utenti, 2)
            If imp = "1" Then
                objParametriIngresso.Personalizzate = True
            End If
            objParametriIngresso.Lingua_Cod = objparametri_Server.Lingua_Cod

            For f = 0 To dtMovDetTecFasi.Rows.Count - 1
                Select Case dtMovDetTecFasi.Rows(f).Item("ff_classe")
                    Case < 1000
                        If Not Hash_ff_cod.ContainsKey(dtMovDetTecFasi.Rows(f).Item("ff_classe")) Then
                            Hash_ff_cod.Add(dtMovDetTecFasi.Rows(f).Item("ff_classe"), "")
                            Filtro_ff_cod &= dtMovDetTecFasi.Rows(f).Item("ff_classe") & ","
                        End If
                    Case Else
                        If Not Hash_cod_ss.ContainsKey(dtMovDetTecFasi.Rows(f).Item("ff_classe")) Then
                            Hash_cod_ss.Add(dtMovDetTecFasi.Rows(f).Item("ff_classe"), "")
                            Filtro_cod_ss &= dtMovDetTecFasi.Rows(f).Item("ff_classe") & ","
                        End If
                End Select
            Next

            If Filtro_ff_cod <> "" Then
                objParametriIngresso.strFiltro = " fs.ff_cod in (" & Left(Filtro_ff_cod, Filtro_ff_cod.Length - 1) & ")"
                objParametriUscitaFasiOld = objFasi_WS.FasiFenologiche_OLD(objParametriIngresso)
            End If
            If Filtro_cod_ss <> "" Then
                objParametriIngresso.strFiltro = " ss.cod_ss in (" & Left(Filtro_cod_ss, Filtro_cod_ss.Length - 1) & ")"
                objParametriUscitaFasiNew = objFasi_WS.FasiFenologiche(objParametriIngresso)
            End If

        End If

        Dim leggiCategMag As New Categorie_Magazzino_R
        Dim DtCategorieMagazzino As DataTable
        If HttpContext.Current.Cache IsNot Nothing Then
            If HttpContext.Current.Cache("DtCategorieMagazzino") Is Nothing Then
                DtCategorieMagazzino = leggiCategMag.Leggi(0, "", True, "", "", objparametri_Server)
                HttpContext.Current.Cache("DtCategorieMagazzino") = DtCategorieMagazzino
            Else
                DtCategorieMagazzino = HttpContext.Current.Cache("DtCategorieMagazzino")
            End If
        Else
            ' In Angular
            DtCategorieMagazzino = leggiCategMag.Leggi(0, "", True, "", "", objparametri_Server)
        End If

        Dim sa_nome = New AgronicaCoreAnagrafeDAL.CentriAziendali_Read().SaNome_from_SaCod(Piva, Sa_Cod, objparametri_Server)

        Dim ht_Permessi As New Hashtable

        'Se ce l'ho già, lo prendo altrimenti lo leggo da DB
        If Not ht_Permessi.ContainsKey(Lav_Cod) Then
            Dim permesso As Boolean = PermessiOpContabiliEMagazzino(
                                                            Lav_Cod, enum_Security_Operazione.Modifica,
                                                            objparametri_Server, objparametri_Utenti, HttpContext.Current?.Session)

            ht_Permessi.Add(Lav_Cod, permesso)
        End If

        Dim DT As New DataTable
        creaDTAgenda(DT, Visualizza_Codici_AppezzaImpianti, Visualizza_KPIN_BlockName, FF_TrackedData_Cod, DTParamQual, estraiPkImpianti)

        If DtAgenda.Rows.Count > 0 Then
            Dim lockObject As New Object
            Dim taskNumber = Environment.ProcessorCount
            Dim taskList As New List(Of Task)
            Dim dtList As New List(Of DataTable)

            Dim listID_Agenda = (From r In DtAgenda.AsEnumerable() Select If(IsDBNull(r.Item("ID_Agenda")), 0, Convert.ToInt32(r.Item("ID_Agenda")))).Distinct().ToList()
            Dim listaIdXCosti = New List(Of Integer)

            If FF_TrackedData_Cod > 0 Then
                listaIdXCosti.AddRange((From r In DtAgenda.AsEnumerable() Select If(IsDBNull(r.Item("Id_Mov_Det")), 0, Convert.ToInt32(r.Item("Id_Mov_Det")))).Distinct().ToList())
            Else
                listaIdXCosti.AddRange(listID_Agenda)
            End If
            Dim objCostiAccessori As New AgronicaCoreContabDAL.CostiAccessori_R
            Dim DtCosti = objCostiAccessori.CostiAccessori_from_IdAgenda3_Ottimizzata(Piva, listaIdXCosti, "", "", objparametri_Server)

            Dim agende = DtAgenda.ToExpandoObject
            If listID_Agenda.Count < 100 Then
                taskNumber = 1
            End If

            Dim stepAgenda = CInt(listID_Agenda.Count / taskNumber)
            Dim chunks = ChunkBy(Of Integer)(listID_Agenda, stepAgenda).ToList
            For Each c In chunks
                c = c.ToList
            Next

            Dim dtQueue As New ConcurrentQueue(Of DataTable)
            Threading.Tasks.Parallel.ForEach(chunks, Sub(c)
                                                         Dim query = (From agenda In agende
                                                                      Join idAgenda In c On agenda("Id_Agenda") Equals idAgenda
                                                                      Select agenda).ToList

                                                         Dim dtA = query.ToDataTable(DtAgenda)
                                                         dtQueue.Enqueue(dtA)

                                                     End Sub)

            If dtQueue.Count > 0 Then

                For Each q As DataTable In dtQueue

                    Dim t As New Task(Function()
                                          Dim DT_Task = getDTAgenda(q,
                                                                     Piva,
                                                                     Sa_Cod,
                                                                     sa_nome,
                                                                     Veg_Cod,
                                                                     Lav_Cod,
                                                                     righeAggiunte,
                                                                     FF_TrackedData_Cod,
                                                                     objparametri_Server,
                                                                     objparametri_Utenti,
                                                                     bool_isCuraEnabled,
                                                                     Visualizza_Codici_AppezzaImpianti,
                                                                     Visualizza_KPIN_BlockName,
                                                                     HtProdPA,
                                                                     HtPrincAtt,
                                                                     ht_Permessi,
                                                                     DTParamQual,
                                                                     DtCategorieMagazzino,
                                                                     dtMovDetTec,
                                                                     dtMovDetTecFasi,
                                                                     DtCosti,
                                                                     objParametriUscitaFasiOld,
                                                                     objParametriUscitaFasiNew,
                                                                     lockObject,
                                                                     estraiPkImpianti:=estraiPkImpianti
                                                                    )
                                          dtList.Add(DT_Task)
                                      End Function)

                    taskList.Add(t)

                    t.Start()

                Next
            End If

            Task.WaitAll(taskList.ToArray)

            For ii = 0 To dtList.Count - 1

                DT.Merge(dtList(ii))

            Next

        End If



        DT.TableName = "Movimenti"

        'uso il dataview per Riordinare 
        Dim Dv As New DataView(DT)

        If FromOutToIn Then
            Dv.Sort = " Data2 DESC, Ora DESC, Id_Agenda DESC"
        Else
            Dv.Sort = " Data2 ASC, Ora ASC, Id_Agenda ASC"
        End If

        Dim dtOrd As DataTable = Dv.ToTable

        Return dtOrd

    End Function

    Private Shared Iterator Function ChunkBy(Of TSource)(ByVal source As IEnumerable(Of TSource), ByVal chunkSize As Integer) As IEnumerable(Of IEnumerable(Of TSource))
        While source.Any()
            Yield source.Take(chunkSize)
            source = source.Skip(chunkSize)
        End While
    End Function

    Public Shared Function PermessiOpContabiliEMagazzino(ByVal Lav_Cod As String,
                                                         ByVal Operazione As String,
                                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                                         ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                         ByRef session As System.Web.SessionState.HttpSessionState
                                                         ) As Boolean

        Dim olav As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim gruOp As Integer = olav.Gru_Op_from_LavorazioneCod(Lav_Cod, objParametri_Server)

        'Estraggio il permesso da controllare
        Dim attivita As enum_Security_Attivita
        Select Case gruOp
            Case 6
                attivita = enum_Security_Attivita.Gest_Contabilita
            Case 10
                attivita = enum_Security_Attivita.Gest_Magazzino
            Case 20

                Select Case Lav_Cod
                    Case LAVCOD_GESTIONE_RIFIUTI
                        attivita = enum_Security_Attivita.Gestione_Rifiuti
                    Case LAVCOD_MONITORAGGIO_TEMPI_RIENTRO, LAVCOD_VISITA_GENERICA, LAVCOD_VISITA
                        attivita = enum_Security_Attivita.Gest_CartellaAziendale_VisiteIspettive
                    Case LAVCOD_MONITORAGGIO_CE
                        attivita = enum_Security_Attivita.Gest_CartellaAziendale_MonitoraggioCE
                    Case LAVCOD_PRATICA_ECOLOGICA
                        attivita = enum_Security_Attivita.CheckList_Pratiche_Ecologiche_APOT
                    Case LAVCOD_FORMAZIONE
                        attivita = enum_Security_Attivita.CheckList_Formazione
                    Case Else
                        attivita = enum_Security_Attivita.Gest_CartellaAziendale_VisiteIspettive
                End Select

            Case Else
                Return True
        End Select

        'Estraggo il tipo di accesso: lettura/modifica
        Dim tipoOperazione As enum_Security_Operazione
        If IsNumeric(Operazione) Then
            Select Case Operazione
                Case enum_TipoOperazioneDB.Lettura, enum_TipoOperazioneDB.Modifica
                    tipoOperazione = CInt(Operazione)
                Case enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Cancellazione, enum_TipoOperazioneDB.Trasferimento, enum_TipoOperazioneDB.Copia
                    tipoOperazione = enum_TipoOperazioneDB.Modifica
                Case Else
                    Return False
            End Select
        Else
            Return False
        End If

        'Controllo il permesso
        Dim acUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim utenteAbilitato As Boolean
        If session IsNot Nothing Then
            utenteAbilitato = acUtenti.Controlla_Permessi_Utente(session("ASG_Utente_Username"), session("ASG_IdServizio"), attivita, tipoOperazione, Now, "", objParametri_Utenti)
        Else
            ' In Angular. Metto l'id del servzio 5, intanto è questo il valore utilizzato da GIAS 2010
            utenteAbilitato = acUtenti.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername, 5, attivita, tipoOperazione, Now, "", objParametri_Utenti)
        End If

        Return utenteAbilitato

    End Function
    Private Shared Function estraiPrincipiAttivi(ByRef DtAgenda As DataTable, ByRef HtProdPA As Hashtable, objParametri_Server As AgronicaCoreParametri, ngObjParametri_Utenti As AgronicaCoreParametri) As Hashtable

        Dim res As New Hashtable()

        Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility

        'seleziono le righe nella tabella di appoggio relative ai principi attivi
        'Mi interessano solo i principi attivi dei CAU_MOV = '2050' (Trattamenti)
        Dim dtPA As DataTable = DtAgenda.Clone()
        For Each dr As DataRow In DtAgenda.Rows
            If dr.Item("elem_cod") = FORMULATI AndAlso dr.Item("CAU_MOV") = "2050" Then
                dtPA.ImportRow(dr)
            End If
        Next

        If Not IsNothing(dtPA) AndAlso dtPA.Rows.Count > 0 Then
            Dim PrincipiSalvati As Boolean = False

            Dim strPACod() As String = objSqlDis.SelectDistinct(dtPA, "PrincipiAttivi")

            If Not IsNothing(strPACod) AndAlso strPACod.Length > 0 Then

                'se i principi sono salvati tutti (cod1§titolo1|cod2§titolo2)
                'leggo in locale le descrizioni dei principi attivi
                If strPACod(0) <> "" Then
                    PrincipiSalvati = True
                    Dim Principi() As String
                    Dim strElencoPACOD As String = ""
                    For Each strPa_Cod As String In strPACod
                        Principi = Split(strPa_Cod, "|")
                        If Not IsNothing(Principi) Then
                            For Each p As String In Principi
                                strElencoPACOD &= Split(p, "§")(0) & ","
                            Next
                        End If
                    Next
                    If strElencoPACOD <> "" Then
                        Dim objPA As New AgronicaCoreMetaSchemaDAL.PrincipiAttivi_R
                        Dim DtPrincipiDes As DataTable = objPA.Leggi_Da_StrPa_Cod(Left(strElencoPACOD, strElencoPACOD.Length - 1), AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "pa_cod", objParametri_Server)

                        For Each dr As DataRow In DtPrincipiDes.Rows
                            If Not res.ContainsKey(dr.Item("PA_Cod")) Then
                                res.Add(dr.Item("PA_Cod").ToString, dr.Item("PA_Des"))
                            End If
                        Next
                    End If
                End If
            End If

            'se almeno un campo è vuoto
            'leggo i principi da web service (con una sola chiamata per tutti i formulati)
            If PrincipiSalvati = False Then
                'ottengo i formulati distinti
                Dim ElencoFormulati As String = ""
                Dim strFrCod() As String = objSqlDis.SelectDistinct(dtPA, "pro_cod")
                If Not strFrCod Is Nothing Then
                    ElencoFormulati = String.Join(",", strFrCod)
                End If
                If ElencoFormulati <> "" Then
                    Dim objAgroWs As New AgronicaCoreWebService.AgroWs
                    Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session?("ASG_objParametri_Utenti")

                    If objParametri_Utenti Is Nothing Then
                        objParametri_Utenti = ngObjParametri_Utenti ' in Angular
                    End If
                    Dim DtPrincipi As DataTable = objAgroWs.ComposizioneFormulatiRecupera(ElencoFormulati, objParametri_Server, objParametri_Utenti)

                    If IsNothing(DtPrincipi) Then
                        Throw New Exception("Eccezione durante il recupero dei dati sui principi attivi da WS")
                    End If

                    'Estraggo i PrincipiAttivi (cod - des)
                    For Each dr As DataRow In DtPrincipi.Rows
                        Dim testo As String = dr.Item("Elenco_PrincipiAttivi")
                        If testo <> "" Then
                            Dim elenco As String() = Split(testo, "|")
                            For Each elem As String In elenco
                                Dim datiElem As String() = Split(elem, "§")
                                If datiElem.Count > 2 AndAlso Not res.ContainsKey(datiElem(0)) Then
                                    res.Add(datiElem(0).ToString, datiElem(1))
                                End If
                            Next
                        End If
                    Next

                    'Estraggo i PrincipiAttivi (Fr_Cod - principiAttivi)
                    HtProdPA = New Hashtable()
                    For Each dr As DataRow In DtPrincipi.Rows
                        If Not HtProdPA.ContainsKey(dr.Item("Fr_Cod")) Then
                            HtProdPA.Add(dr.Item("Fr_Cod"), dr.Item("Elenco_PrincipiAttivi"))
                        End If
                    Next

                    objAgroWs = Nothing
                End If
            End If
        End If

        Return res

    End Function

    Public Shared Function creaDTAgenda(ByRef dt As DataTable,
                                        Visualizza_Codici_AppezzaImpianti As Boolean,
                                        Visualizza_KPIN_BlockName As Boolean,
                                        FF_TrackedData_Cod As Integer,
                                        DTParamQual As DataTable,
                                        Optional ByVal estraiPkImpianti As Boolean? = Nothing
                                        )

        '----- Definisco la struttura del DataTable

        dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        dt.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Lav_Des", GetType(String)))
        dt.Columns.Add(New DataColumn("Data", GetType(String)))
        dt.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Id_Mov_Det", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Data2", GetType(Date)))   'x ordinare
        dt.Columns.Add(New DataColumn("Ora", GetType(Date)))   'x ordinare
        dt.Columns.Add(New DataColumn("Blocco_Flag", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Raccoglitore_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Info", GetType(String)))
        dt.Columns.Add(New DataColumn("Dettagli", GetType(String)))
        dt.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Ricetta_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Ricetta_Des", GetType(String)))
        dt.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
        dt.Columns.Add(New DataColumn("Operazione_DES", GetType(String)))
        dt.Columns.Add(New DataColumn("gru_des", GetType(String)))
        dt.Columns.Add(New DataColumn("tipo", GetType(String)))
        dt.Columns.Add(New DataColumn("tipo_colore", GetType(String)))
        dt.Columns.Add(New DataColumn("cul_des", GetType(String)))
        dt.Columns.Add(New DataColumn("Specie_Varieta", GetType(String)))
        dt.Columns.Add(New DataColumn("Dettaglio_Tecnico", GetType(String)))
        dt.Columns.Add(New DataColumn("Centro_Campo", GetType(String)))
        dt.Columns.Add(New DataColumn("ID", GetType(String)))
        dt.Columns.Add(New DataColumn("Creatore_Intervento", GetType(String)))
        dt.Columns.Add(New DataColumn("Data_Ultima_Modifica_Intervento", GetType(String)))
        dt.Columns.Add(New DataColumn("Contabilizzato", GetType(Integer)))
        dt.Columns.Add(New DataColumn("LottiProduzione", GetType(String)))
        dt.Columns.Add(New DataColumn("Note", GetType(String)))
        dt.Columns.Add(New DataColumn("Costi_Operatori", GetType(String)))
        dt.Columns.Add(New DataColumn("Costi_Macchine", GetType(String)))
        dt.Columns.Add(New DataColumn("Sup_Trattata", GetType(Decimal)))
        dt.Columns.Add(New DataColumn("LottiImpianto", GetType(String)))
        dt.Columns.Add(New DataColumn("PermessoModifica", GetType(String)))
        dt.Columns.Add(New DataColumn("Descrizione_Unica", GetType(String)))
        dt.Columns.Add(New DataColumn("Elem_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("NomeComune", GetType(String)))
        dt.Columns.Add(New DataColumn("Origine", GetType(String)))

        If Visualizza_Codici_AppezzaImpianti Then
            dt.Columns.Add(New DataColumn("Codici_Appezzamenti", GetType(String)))
            dt.Columns.Add(New DataColumn("Codici_Impianto", GetType(String)))
        End If

        If Visualizza_KPIN_BlockName Then
            dt.Columns.Add(New DataColumn("KPIN", GetType(String)))
            dt.Columns.Add(New DataColumn("BlockName", GetType(String)))
        End If

        dt.Columns.Add(New DataColumn("Centro_Aziendale", GetType(String)))
        dt.Columns.Add(New DataColumn("Specie", GetType(String)))
        dt.Columns.Add(New DataColumn("Appezzamenti_Coinvolti", GetType(String)))
        dt.Columns.Add(New DataColumn("Prodotti_Utilizzati", GetType(String)))
        dt.Columns.Add(New DataColumn("Avversita", GetType(String)))
        dt.Columns.Add(New DataColumn("chiave_composita", GetType(String)))
        dt.Columns.Add(New DataColumn("chiave_sincro_ARTEA", GetType(String)))

        dt.Columns.Add(New DataColumn("ProdottiMagazzinoTrattati_Coinvolti", GetType(String)))
        dt.Columns.Add(New DataColumn("ProdottiMagazzinoTrattati_Lotti", GetType(String)))
        dt.Columns.Add(New DataColumn("ProdottiMagazzinoTrattati_QuantitaQuintali", GetType(Decimal)))

        If FF_TrackedData_Cod > 0 Then
            dt.Columns.Add(New DataColumn("FF_Track_Lotto_Padre", GetType(String)))
            dt.Columns.Add(New DataColumn("FF_Track_Lotto", GetType(String)))
            dt.Columns.Add(New DataColumn("FF_Track_Cal_Cod_Padre", GetType(String)))
            dt.Columns.Add(New DataColumn("FF_Track_Cal_Cod", GetType(String)))
            dt.Columns.Add(New DataColumn("FF_Track_Qta_Extra_Totale", GetType(Decimal)))
            dt.Columns.Add(New DataColumn("FF_Track_Qta_Contenitori", GetType(Decimal)))
            dt.Columns.Add(New DataColumn("FF_Track_Qta_Imballi", GetType(Decimal)))
            dt.Columns.Add(New DataColumn("FF_Mat_Cod", GetType(String)))
            dt.Columns.Add(New DataColumn("FF_Referenza", GetType(String)))
            For Each paramQual In DTParamQual.Rows
                If paramQual("Tipo") = 1 Then
                    dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key") & "_Codice", GetType(String)))
                    dt.Columns.Add(New DataColumn("FF_" & paramQual("Tabella_Key"), GetType(String)))
                End If
            Next
            dt.Columns.Add(New DataColumn("FF_Righe_Aggiunte", GetType(String)))
            dt.Columns.Add(New DataColumn("FF_codice_generazione", GetType(Integer)))
            dt.Columns.Add(New DataColumn("FF_Linea_Cod", GetType(Integer)))
        End If

        If Not IsNothing(estraiPkImpianti) AndAlso estraiPkImpianti.HasValue AndAlso estraiPkImpianti = True Then
            dt.Columns.Add(New DataColumn("PK_Impianti", GetType(String)))
        End If

        dt.Columns.Add(New DataColumn("Id_Agenda_Visita", GetType(Integer)))

        dt.Columns.Add(New DataColumn("Data_Creazione", GetType(Date)))

        dt.Columns.Add(New DataColumn("Installazione_Trappola_Reinnescata", GetType(Integer)))

        dt.Columns.Add(New DataColumn("Cau_Mov", GetType(String)))

    End Function

    ''' <summary>
    ''' Considera un'operazioni con contabilità di magazzino se ha lavCod < 1000 o se fa riferiemnto a una delle seguenti operazioni:
    ''' LAVCOD_ACCETTAZIONE_DIVERSI,
    ''' LAVCOD_BOLLA_RICEVUTA,
    ''' LAVCOD_BOLLA_EMESSA,
    ''' LAVCOD_DISTINTA_CARICO,
    ''' LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
    ''' LAVCOD_AUTO_DDT_EMESSO,
    ''' LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE,
    ''' LAVCOD_TRASFORMAZIONI
    ''' </summary>
    ''' <param name="lavCod">lavCod dell'operazione</param>
    ''' <returns>True se il lavCod appartiene a un'operazione con contabilità di magazzino, False altrimenti.</returns>
    Private Shared Function operazioneLavCodContabMagazzino(ByVal lavCod As Integer) As Boolean

        Dim rval As Boolean
        rval = (lavCod < 1000 Or {
            LAVCOD_ACCETTAZIONE_DIVERSI,
            LAVCOD_BOLLA_RICEVUTA,
            LAVCOD_BOLLA_EMESSA,
            LAVCOD_DISTINTA_CARICO,
            LAVCOD_DISTINTA_CARICO_ACCETTAZIONE,
            LAVCOD_AUTO_DDT_EMESSO,
            LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE,
            LAVCOD_TRASFORMAZIONI
        }.Contains(lavCod))

        Return rval

    End Function

    ''' <summary>
    ''' Considera rileivi i lavCod appartenetni a:
    ''' LAVCOD_FASI_FENOLOGICHE,
    ''' LAVCOD_RILIEVO_AVVERSITA_CAMPO,
    ''' LAVCOD_RILIEVO_ERBE_INFESTANTI,
    ''' LAVCOD_RILIEVO_INDICI_MATURITA,
    ''' LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
    ''' LAVCOD_DANNI_RACCOLTA
    ''' </summary>
    ''' <param name="lavCod"></param>
    ''' <returns></returns>
    Private Shared Function IsRilievo(lavCod As Integer) As Boolean
        Return {
            LAVCOD_FASI_FENOLOGICHE,
            LAVCOD_RILIEVO_AVVERSITA_CAMPO,
            LAVCOD_RILIEVO_ERBE_INFESTANTI,
            LAVCOD_RILIEVO_INDICI_MATURITA,
            LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA,
            LAVCOD_DANNI_RACCOLTA
        }.Contains(lavCod)
    End Function

    Public Shared Function getDTAgenda(DtAgenda As DataTable,
                                       Piva As String,
                                       sa_cod As Integer,
                                       sa_nome As String,
                                       Veg_Cod As Integer,
                                       Lav_Cod As Integer,
                                       righeAggiunte As String,
                                       FF_TrackedData_Cod As Integer,
                                       objparametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       objparametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       bool_isCuraEnabled As Boolean,
                                       Visualizza_Codici_AppezzaImpianti As Boolean,
                                       Visualizza_KPIN_BlockName As Boolean,
                                       HtProdPA As Hashtable,
                                       HtPrincAtt As Hashtable,
                                       ht_Permessi As Hashtable,
                                       DTParamQual As DataTable,
                                       DtCategorieMagazzino As DataTable,
                                       dtMovDetTec As DataTable,
                                       dtMovDetTecFasi As DataTable,
                                       ByVal dtCosti As DataTable,
                                       objParametriUscitaFasiOld As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output,
                                       objParametriUscitaFasiNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output,
                                       lockObject As Object,
                                       Optional ByVal estraiPkImpianti As Boolean? = Nothing
                                       ) As DataTable

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility
        Dim objEti As AgronicaCoreStampeDAL.FF_Etichette_R

        Dim Dt As New DataTable

        creaDTAgenda(Dt, Visualizza_Codici_AppezzaImpianti, Visualizza_KPIN_BlockName, FF_TrackedData_Cod, DTParamQual, estraiPkImpianti:=estraiPkImpianti)

        Dim strProdottiMagazzinoTrattati As String
        Dim strLottiProdottiMagazzinoTrattati As String
        Dim QtaProdottoTrattataTot As Decimal

        Dim strPkImpianti As String
        Dim AppezzamentoNome As String
        Dim strAppezzamenti As String
        Dim strCulDes As String
        Dim strProdotti As String
        Dim strAvversita As String
        Dim strSpecie As String
        Dim strCentro As String
        Dim strSpecieVarieta As String
        Dim strDettaglioTecnico As String
        Dim strCentroCampo As String
        Dim strLottiProduzione As String
        Dim strLottiImpianto As String
        Dim strNote As String
        Dim strCosti_Operatori As String
        Dim strCosti_Macchine As String
        Dim Prodotto As String
        Dim Ricetta As String
        Dim Sup_TrattataTot As Decimal

        Dim strCodici_Appezzamenti As String
        Dim strCodici_Impianto As String
        Dim strKPIN As String
        Dim strBlockName As String

        Dim strId_Agenda() As String

        Dim Testo As String
        Dim strDettagli As String
        Dim Bloccato As String

        Dim Icona_INFO As String = "<img src='../AB_Immagini/Icone16/cI.ico' border='0'>"

        Dim Dr As DataRow
        Dim DrAgenda() As DataRow

        Dim DtOperazione As New DataTable
        Dim DtApp As New DataTable
        Dim DtProdotti As New DataTable
        Dim DtAvversita As New DataTable
        Dim DtAvversitaGru As New DataTable
        Dim DtProdotti1 As New DataTable
        Dim DtSpecie As New DataTable

        If FF_TrackedData_Cod > 0 Then
            strId_Agenda = objSqlDis.SelectDistinct(DtAgenda, "Id_Mov_Det")
        Else
            strId_Agenda = objSqlDis.SelectDistinct(DtAgenda, "Id_Agenda")
        End If

        If Not strId_Agenda Is Nothing Then

            For Each current_Agenda In strId_Agenda

                'AZZERO LE STRINGHE AD OGNI GIRO
                strPkImpianti = ""
                strAppezzamenti = ""
                strProdottiMagazzinoTrattati = ""
                strCulDes = ""
                strProdotti = ""
                strAvversita = ""
                strNote = ""
                strCosti_Operatori = ""
                strCosti_Macchine = ""
                Sup_TrattataTot = 0

                strCodici_Appezzamenti = ""
                strCodici_Impianto = ""
                strKPIN = ""
                strBlockName = ""

                If FF_TrackedData_Cod > 0 Then
                    DrAgenda = DtAgenda.Select("Id_Mov_Det=" & current_Agenda)
                Else
                    DrAgenda = DtAgenda.Select("id_agenda=" & current_Agenda)
                End If

                'DtOperazione = DtAgenda.Clone

                DtOperazione = DrAgenda.CopyToDataTable

                'For Each r As DataRow In DrAgenda
                '    DtOperazione.ImportRow(r)
                'Next


                If DrAgenda.Length > 0 Then

                    Dr = Dt.NewRow
                    impostaDatiBaseOperazione(Dr, DrAgenda)

                    'Se è un altre lavorazioni aggiungo il dettaglio
                    If DrAgenda(0).Item("attivitaDesc") <> "" Then
                        'Dr.Item("Lav_Des") &= " (" & String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s))) & ")"
                        Dr.Item("Lav_Des") = String.Join(" - ", {DrAgenda(0).Item("attivitaSigla").trim(), DrAgenda(0).Item("attivitaDesc").trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                    End If

                    trackLottoPadre(Dr, DrAgenda, DtAgenda)

                    ' @Paolo
                    ' aggiunta colore per tipologia di lavorazione
                    Select Case DrAgenda(0).Item("tipo")
                        Case "C" ' Colturali
                            Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: green'></i>"
                        Case "E" ' Contabili
                            Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: blue'></i>"
                        Case "V" ' Audit / Monitoraggi
                            Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: orange'></i>"
                        Case "F" ' Macchine
                            Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: red'></i>"
                        Case "Z" ' Zootecniche
                            Dr.Item("tipo_colore") = "<i class='fa fa-circle' style='color: yellow'></i>"
                    End Select

                    strAppezzamenti = ""
                    strCulDes = ""
                    strProdotti = ""
                    strAvversita = ""
                    strSpecie = ""
                    strCentro = ""
                    Prodotto = ""
                    Ricetta = ""
                    strSpecieVarieta = ""
                    strDettaglioTecnico = ""
                    strCentroCampo = ""
                    strLottiProduzione = ""
                    strLottiImpianto = ""

                    strProdottiMagazzinoTrattati = ""
                    strLottiProdottiMagazzinoTrattati = ""
                    QtaProdottoTrattataTot = 0

                    If FF_TrackedData_Cod > 0 Then
                        strDettagli = DrAgenda(0)("Des_Lib") & "<br/>" & DrAgenda(0)("Fabbricato_Des") & "<br/>"
                    Else
                        strDettagli = ""
                    End If

                    If DrAgenda(0).Item("ricetta_cod") <> 0 Then
                        Ricetta = "Ricetta n. " & DrAgenda(0).Item("ricetta_numero")
                    End If
                    Dr.Item("Ricetta_Cod") = DrAgenda(0).Item("ricetta_cod")
                    Dr.Item("Ricetta_Des") = DrAgenda(0).Item("ricetta_numero")


                    'modifica per contabilità magazzino
                    If operazioneLavCodContabMagazzino(DrAgenda(0).Item("lav_cod")) Then

                        'specie
                        setSpecieOperazione(Dr, strSpecie, Veg_Cod, objSqlDis, DtOperazione)

                        'appezzamenti
                        DtApp = gestioneAppezzamentiOperazione(Dr, strCentro, strAppezzamenti, AppezzamentoNome,
                                                               strCodici_Appezzamenti, strPkImpianti, strCodici_Impianto,
                                                               strSpecieVarieta, strCulDes, strKPIN, strBlockName,
                                                               Visualizza_Codici_AppezzaImpianti, Visualizza_KPIN_BlockName,
                                                               objSqlDis, DtOperazione, estraiPkImpianti)

                        If DrAgenda(0).Item("Lav_Cod") = LAVCOD_TRATTAMENTO_POST_RACCOLTA OrElse
                           DrAgenda(0).Item("Lav_Cod") = LAVCOD_CONCIA_SEME Then
                            'prodotti magazzino trattati
                            strProdottiMagazzinoTrattati = gestioneProdottiMagazzinoTrattatiOperazione(Dr, DtOperazione, strCulDes, strSpecieVarieta, strLottiProdottiMagazzinoTrattati, objSqlDis)
                        End If

                        'prodotti
                        setProdottiOperazione(Dr, DrAgenda, DtCategorieMagazzino, DtProdotti, DtProdotti1,
                                                  Prodotto, strProdotti, righeAggiunte, FF_TrackedData_Cod, lockObject,
                                                  objSqlDis, DtOperazione, objparametri_Server)

                        strAvversita = getStrAvversita(DrAgenda, dtMovDetTec)

                        'Centri e Campi
                        strCentroCampo = getStrCentroCampo(DtOperazione)

                        'Lotti Produzione
                        strLottiProduzione = getStrLottiProduzione(DtOperazione)

                        'Lotti Impianto
                        strLottiImpianto = getStrLottiImpianto(DtApp)

                        'Note a checkbox
                        strNote = getStrNote(DtOperazione)

                        getStrCostiMacchineOperatoriViaIDAgenda(strCosti_Operatori, strCosti_Macchine, dtCosti, current_Agenda, False)

                        Dim dbUtil As New AgronicaCoreDataProvider.DatatableUtility
                        'Quantità Prodotti Magazzino Trattata
                        Dim fieldList As New List(Of String) From {"Piva", "Sa_Cod", "Id_Destinazione", "Mat_Cod", "LottoProduzione", "Progetto_Cod"}
                        Dim _strProdotti(,) As String = dbUtil.SelectDistincFromFieldList(DtOperazione, fieldList, False)

                        For w = 0 To _strProdotti.Length / 6 - 1
                            'Considero solo le righe dove Id_Destinazione > 0
                            If _strProdotti(w, 2) > 0 Then
                                Dim drProdotto() As DataRow = DtOperazione.Select(" Piva = '" & _strProdotti(w, 0) & "'" &
                                                                                  " AND Sa_Cod = " & _strProdotti(w, 1) &
                                                                                  " AND Id_Destinazione = " & _strProdotti(w, 2) &
                                                                                  " AND Mat_Cod = " & _strProdotti(w, 3) &
                                                                                  " AND LottoProduzione = '" & _strProdotti(w, 4) & "'" &
                                                                                  " AND Progetto_Cod = " & _strProdotti(w, 5) &
                                                                                  " AND Tipo_Destinazione = " & MAGAZZINO &
                                                                                  " AND Elem_Cod <> " & FORMULATI) 'Escludo le righe di scarico formulati

                                If drProdotto.Length > 0 Then
                                    QtaProdottoTrattataTot += CDec(drProdotto(0).Item("Qta"))
                                End If
                            End If
                        Next

                        'Superficie Trattata
                        Dim strID_Reg_Prima_Appezza(,) As String = dbUtil.SelectDistinct(DtOperazione, "APPEZZA", "ID_REG", False)

                        For w = 0 To strID_Reg_Prima_Appezza.Length / 2 - 1
                            Dim drAppezza() As DataRow = DtOperazione.Select("APPEZZA=" & strID_Reg_Prima_Appezza(w, 0) & " AND ID_REG=" & strID_Reg_Prima_Appezza(w, 1) & " ")

                            If drAppezza.Length > 0 Then
                                Sup_TrattataTot += If(drAppezza(0).Item("sup_trattata") <> 0, CDec(drAppezza(0).Item("Sup_Trattata")), CDec(drAppezza(0).Item("sup_app")))
                            End If
                        Next

                        '----------------------------------
                        'Dettaglio Tecnico
                        strDettaglioTecnico = getStrDettaglioTecnicoOperazione(DrAgenda, DtOperazione,
                                                                               dtMovDetTec, dtMovDetTecFasi,
                                                                               HtProdPA, HtPrincAtt,
                                                                               objParametriUscitaFasiOld,
                                                                               objParametriUscitaFasiNew)
                    End If


                    'modifica per magazzino
                    Dim lc As Integer = DrAgenda(0).Item("Lav_Cod")
                    If {LAVCOD_CARICO, LAVCOD_SCARICO, LAVCOD_VENDITA, LAVCOD_ACQUISTO, LAVCOD_TRASFERIMENTO}.Contains(lc) Then
                        If bool_isCuraEnabled Then
                            strProdotti = DrAgenda(0)("Des_Lib")
                        Else
                            '----------------------------------
                            'prodotti
                            strProdotti = GetAllProductsString(DtOperazione, False)
                            If strProdotti <> "" Then
                                Select Case CInt(DrAgenda(0).Item("Elem_Cod"))
                                    Case FERTILIZZANTI
                                        strProdotti = "<b>" & My.Resources.AgronicaCoreContabBIZ.ProdottiUtilizzati & "</b> " & strProdotti

                                    Case FORMULATI
                                        strProdotti = "<b>" & My.Resources.AgronicaCoreContabBIZ.ProdottiUtilizzati & "</b> " & strProdotti

                                    Case TRAPPOLE
                                        strProdotti = "<b>" & My.Resources.AgronicaCoreContabBIZ.ProdottiUtilizzati & "</b> " & strProdotti

                                    Case INSETTI
                                        strProdotti = "<b>" & My.Resources.AgronicaCoreContabBIZ.ProdottiUtilizzati & "</b> " & strProdotti

                                    Case SEMENTI
                                        strProdotti = My.Resources.AgronicaCoreContabBIZ.MaterialeVivaistaUtilizzato & strProdotti

                                    Case SEMILAVORATI_VEGETALI
                                        strProdotti = My.Resources.AgronicaCoreContabBIZ.SemilavoratoRaccolto & strProdotti
                                End Select
                            End If
                        End If

                        If bool_isCuraEnabled Then

                            strDettaglioTecnico = DrAgenda(0)("Des_Lib")

                        Else
                            '----------------------------------
                            'Dettaglio Tecnico
                            Dim listaDetTec As New List(Of String)
                            For Each drDetTec As DataRow In DtOperazione.Rows
                                Dim prod As String = ""

                                'PRODOTTI
                                Select Case CInt(drDetTec.Item("Elem_Cod"))
                                    Case FERTILIZZANTI

                                        If drDetTec.Item("Pro_Cod") <> 0 Then
                                            prod = drDetTec.Item("Fer_Des") & ", "
                                        End If

                                        If drDetTec.Item("Mat_Cod") <> 0 Then
                                            prod = drDetTec.Item("Mat_Des")
                                        Else
                                            If prod.Length > 0 Then
                                                prod = Left(prod, prod.Length - 2)
                                            End If
                                        End If

                                    Case FORMULATI

                                        If drDetTec.Item("Fr_Des") <> "" Then
                                            prod = drDetTec.Item("Fr_Des")
                                        End If

                                    Case TRAPPOLE

                                        If drDetTec.Item("Trap_Des") <> "" Then
                                            prod = drDetTec.Item("Trap_Des")
                                        End If

                                    Case INSETTI

                                        If drDetTec.Item("Ins_Des") <> "" Then
                                            prod = drDetTec.Item("Ins_Des")
                                        End If

                                    Case SEMENTI

                                        If drDetTec.Item("Mat_Des") <> "" Then
                                            Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                                            'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                                            Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                            prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                                        End If

                                    Case SEMILAVORATI_VEGETALI

                                        If drDetTec.Item("Mat_Des") <> "" Then

                                            Select Case drDetTec.Item("Lav_Cod")
                                                Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                                                    prod = "Semilavorato trattato:" & drDetTec.Item("Mat_Des")
                                                Case Else
                                                    prod = My.Resources.AgronicaCoreContabBIZ.SemilavoratoRaccolto & drDetTec.Item("Mat_Des")
                                            End Select
                                        End If

                                    Case Else
                                        If drDetTec.Item("Mat_Des") <> "" Then
                                            Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                                            'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                                            Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                                            prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")

                                        ElseIf drDetTec.Item("Trap_Des") <> "" Then
                                            prod = drDetTec.Item("Trap_Des")

                                        ElseIf drDetTec.Item("Fer_Des") <> "" Then
                                            prod = drDetTec.Item("Fer_Des")
                                        End If

                                End Select


                                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                Dim testoDetTec As String = String.Join(" - ", {prod}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                                'AGGIUNGO L'INDICAZIONE SULLA QUANTITA'
                                testoDetTec = testoDetTec & " (" & Agro_Math.RoundNumber_2Decimali(drDetTec.Item("Qta")) & " " & drDetTec.Item("UDM_SIM_UDM_COD") & " )"

                                If Not listaDetTec.Contains(testoDetTec) Then
                                    listaDetTec.Add(testoDetTec)
                                End If

                            Next

                            strDettaglioTecnico = String.Join(", ", listaDetTec)
                        End If
                    End If

                    'modifica per contabilita
                    Select Case lc
                        Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_MVV_RICEVUTO, LAVCOD_MVV_EMESSO
                            'occorre pescare i prodotti dai dettagli, non vengono su dalla query perche nei doc contabili sacod agenda è 0 probabilmente
                            'per ora lascio stare, leggo la descrizione mov_desc
                            Try

                                Dim strRifDdtFatture As String = ""
                                Dim strMov_Desc As String = ""

                                For Each r As DataRow In DrAgenda

                                    If Not IsDBNull(r.Item("Mov_Desc")) AndAlso r.Item("Mov_Desc") <> "" Then
                                        If Not strDettagli.Contains(r.Item("Mov_Desc")) Then
                                            strDettagli &= r.Item("Mov_Desc") & " <br> "
                                        End If
                                    End If

                                    If Not String.IsNullOrEmpty(r.Item("Mov_Desc")) Then
                                        strMov_Desc = r.Item("Mov_Desc")
                                    End If


                                    If Not String.IsNullOrEmpty(r.Item("RifDdtFatture")) Then
                                        strRifDdtFatture = "Rif: " & r.Item("RifDdtFatture")
                                    End If

                                Next

                                strDettaglioTecnico = String.Join(" ", {strMov_Desc.Trim(), strRifDdtFatture.Trim(), strDettaglioTecnico}.
                                                                  Where(Function(s) Not String.IsNullOrEmpty(s)))

                            Catch ex As Exception

                            End Try
                    End Select

                    'Modifica per operazione di cura
                    If lc = LAVCOD_CURA Then
                        Dim lottoRaccolto As String = (From riga As DataRow In DtOperazione.Rows Where riga.Item("cau_mov") = CAU_SCARICO AndAlso riga.Item("tipo_destinazione") = TIPO_DESTINAZIONE_MAGAZZINO Select riga.Item("lottoProduzione")).FirstOrDefault()
                        strDettaglioTecnico = "Lotto Raccolto: " & lottoRaccolto
                    End If


                    'Controllo i permessi di modifica
                    If Not FF_TrackedData_Cod > 0 Then
                        'Se ce l'ho già, lo prendo altrimenti lo leggo da DB
                        If ht_Permessi.ContainsKey(Lav_Cod) Then
                            Dr.Item("PermessoModifica") = ht_Permessi(Lav_Cod)
                        Else
                            Dim permesso As Boolean = PermessiOpContabiliEMagazzino(
                                                            Lav_Cod, enum_Security_Operazione.Modifica,
                                                            objparametri_Server, objparametri_Utenti, HttpContext.Current?.Session)

                            ht_Permessi.Add(Lav_Cod, permesso)
                        End If
                    End If


                    Dr.Item("Raccoglitore_Cod") = DrAgenda(0).Item("Raccoglitore_Cod")
                    Dr.Item("Blocco_Flag") = DrAgenda(0).Item("Blocco_Flag")

                    Bloccato = If(DrAgenda(0).Item("Blocco_Flag") = 1, My.Resources.AgronicaCoreContabBIZ.Si, My.Resources.AgronicaCoreContabBIZ.No)

                    Testo = "<a " &
                                "title='" & "ID: " & DrAgenda(0).Item("id_agenda") & vbCrLf &
                                My.Resources.AgronicaCoreContabBIZ.CreatoreIntervento & DrAgenda(0).Item("Tecnico") & vbCrLf &
                                My.Resources.AgronicaCoreContabBIZ.InterventoBloccato & Bloccato &
                                "' " &
                                ">" &
                                Icona_INFO &
                                "</a>"

                    Dim riga1 As String = Dr.Item("Data") & " <b> " & Dr.Item("Lav_Des") & "</b>"
                    Dim riga2 As String = strSpecieVarieta & If(String.IsNullOrEmpty(strDettaglioTecnico), "", " <i>" & strDettaglioTecnico & "</i>")
                    Dim riga3 As String = strCentroCampo & If(String.IsNullOrEmpty(strAppezzamenti), "", " <i>" & strAppezzamenti & "</i>")
                    Dr.Item("Descrizione_Unica") = String.Join("<br>", {riga1.Trim(), riga2.Trim(), riga3.Trim()}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                    Dr.Item("ID") = DrAgenda(0).Item("id_agenda")
                    Dr.Item("Creatore_Intervento") = DrAgenda(0).Item("Tecnico")
                    Dr.Item("Data_Ultima_Modifica_Intervento") = DrAgenda(0).Item("Data_Ultima_Modifica_Intervento")
                    Dr.Item("Contabilizzato") = DrAgenda(0).Item("Contabilizzato")

                    If strCentro <> "" Then
                        strDettagli &= "<b>" & My.Resources.AgronicaCoreContabBIZ.CentroAz & "</b> " & strCentro & "<br>"
                    End If

                    If FF_TrackedData_Cod <= 0 And strSpecie <> "" Then
                        strDettagli &= "<b>" & My.Resources.AgronicaCoreContabBIZ.Specie & "</b> " & strSpecie & "<br>"
                    End If

                    'per le operazioni colturali visualizzo gli appezzamenti coinvolti
                    If strAppezzamenti <> "" Then
                        If strProdotti <> "" Then
                            strDettagli &= "<b>" & My.Resources.AgronicaCoreContabBIZ.AppezzamentiCoinvolti & "</b> " & strAppezzamenti & "<br>" & strProdotti
                        Else
                            strDettagli &= "<b>" & My.Resources.AgronicaCoreContabBIZ.AppezzamentiCoinvolti & "</b> " & strAppezzamenti
                        End If
                    Else
                        strDettagli &= strProdotti
                    End If

                    'avversita
                    strDettagli &= If(strAvversita <> "", "<br> <b> Avversità: </b> " & strAvversita & "<br>", "")

                    'If Ricetta <> "" Then
                    '    strDettagli &= If(strDettagli <> "", vbCrLf & Ricetta, Ricetta)

                    '    strDettaglioTecnico &= If(strDettaglioTecnico <> "", ". ", "") & Ricetta
                    'End If

                    Dr.Item("Info") = Testo
                    Dr.Item("Dettagli") = strDettagli

                    Dr.Item("cul_des") = strCulDes
                    Dr.Item("Specie_Varieta") = strSpecieVarieta
                    Dr.Item("Dettaglio_Tecnico") = strDettaglioTecnico
                    Dr.Item("Centro_Campo") = strCentroCampo
                    Dr.Item("LottiProduzione") = strLottiProduzione
                    Dr.Item("LottiImpianto") = strLottiImpianto
                    Dr.Item("Note") = strNote
                    Dr.Item("Costi_Operatori") = strCosti_Operatori
                    Dr.Item("Costi_Macchine") = strCosti_Macchine
                    Dr.Item("Sup_Trattata") = Sup_TrattataTot

                    'PEr Rilievo Piogge
                    If DrAgenda(0).Item("Lav_Cod") = LAVCOD_RILIEVO_PIOGGE Then

                        strDettagli = "<b>" & My.Resources.AgronicaCoreContabBIZ.CentroAz & "</b> " & sa_nome
                        '&= " [ Pioggia: " & Pioggia & " mm; TMin: " & TMin & " °C; TMax: " & TMax & " °C; Umidita': " & Umidita & "% ] "
                        Dr.Item("Dettagli") = strDettagli

                    End If

                    If FF_TrackedData_Cod > 0 Then
                        If String.IsNullOrEmpty(Dr.Item("FF_Referenza").ToString) AndAlso Not String.IsNullOrEmpty(strProdotti) AndAlso DtAgenda.Columns.Contains("FF_Track_Lotto_Padre") Then
                            Dr.Item("FF_Referenza") = strProdotti
                        End If
                    End If
                    'Dr.Item("gru_des") = DrAgenda(0).Item("tipo")

                    Dim specie As String = DrAgenda(0).Item("veg_des")
                    If DrAgenda(0).Item("Lav_Cod") = LAVCOD_TRATTAMENTO_POST_RACCOLTA OrElse
                       DrAgenda(0).Item("Lav_Cod") = LAVCOD_CONCIA_SEME Then
                        specie = strSpecie
                    End If

                    Dr.Item("Centro_Aziendale") = DrAgenda(0).Item("sa_nome")
                    Dr.Item("Specie") = specie
                    Dr.Item("Appezzamenti_Coinvolti") = strAppezzamenti
                    Dr.Item("Prodotti_Utilizzati") = strProdotti
                    Dr.Item("Avversita") = strAvversita

                    Dr.Item("chiave_composita") = CDate(DrAgenda(0).Item("Data_Movimento")) & "_" &
                                                  DrAgenda(0).Item("Id_Agenda") & "_" &
                                                  DrAgenda(0).Item("Lav_Cod") & "_" &
                                                  DrAgenda(0).Item("Piva") & "_" &
                                                  DrAgenda(0).Item("Sa_Cod") & "_" &
                                                  DrAgenda(0).Item("Blocco_Flag") & "_" &
                                                  DrAgenda(0).Item("Veg_Cod")

                    Dr.Item("chiave_sincro_ARTEA") = (CDate(DrAgenda(0).Item("Data_Movimento")).ToShortDateString()).Replace("/", "-") & "_" &
                                                  DrAgenda(0).Item("Id_Agenda") & "_" &
                                                  DrAgenda(0).Item("Lav_Cod") & "_" &
                                                  DrAgenda(0).Item("Piva") & "_" &
                                                  DrAgenda(0).Item("Sa_Cod") & "_" &
                                                  DrAgenda(0).Item("Blocco_Flag") & "_" &
                                                  DrAgenda(0).Item("Veg_Cod")

                    Dr.Item("Id_Agenda_Visita") = DrAgenda(0).Item("Id_Agenda_Visita")

                    Dr.Item("Installazione_Trappola_Reinnescata") = DrAgenda(0).Item("Installazione_Trappola_Reinnescata")

                    Dr.Item("Cau_Mov") = DrAgenda(0).Item("Cau_Mov")

                    Dr.Item("Data_Creazione") = DrAgenda(0).Item("Data_Creazione")
                    Dr.Item("Origine") = DrAgenda(0).Item("Origine")

                    Dr.Item("ProdottiMagazzinoTrattati_Coinvolti") = strProdottiMagazzinoTrattati
                    Dr.Item("ProdottiMagazzinoTrattati_Lotti") = strLottiProdottiMagazzinoTrattati
                    'Espresso in quintali, su db salvato in KG!
                    Dr.Item("ProdottiMagazzinoTrattati_QuantitaQuintali") = If(QtaProdottoTrattataTot > 0, QtaProdottoTrattataTot / 100, QtaProdottoTrattataTot)

                    Dt.Rows.Add(Dr)

                End If

            Next

        End If


        'Elimino l'oggetto
        objSQL = Nothing
        objSqlDis = Nothing
        Return Dt
    End Function

    Public Function List_Qta_Prodotto_X_Impianti(ByVal Piva As String, ByVal Id_Agenda As Integer, ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date, ByVal objParametri_Server As AgronicaCoreParametri) As List(Of Qta_Prodotto_X_Impianto)

        Dim objAgronicaCoreContabDAL As New AgronicaCoreContabDAL.Movimenti_R

        Dim list_prodotti_x_impianti As New List(Of Qta_Prodotto_X_Impianto)

        Dim DT = objAgronicaCoreContabDAL.Ottieni_Qta_per_Impianti(Piva, Id_Agenda, Validita_Inizio, Validita_Fine, "", "", objParametri_Server)

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            For Each Dr In DT.Rows
                If list_prodotti_x_impianti.FindIndex(Function(prodotto_x_impianto) prodotto_x_impianto.Piva = Dr("Piva") AndAlso prodotto_x_impianto.Id_Agenda = Dr("Id_Agenda")) = -1 Then

                    Dim Dr_filtrato = DT.Select("Piva = '" & Dr("Piva") & "' And Id_Agenda = " & Dr("Id_Agenda"))

                    If Not IsNothing(Dr_filtrato) AndAlso Dr_filtrato.Length > 0 Then

                        Dim Sup_Trattata_Totale As Decimal = (From row In Dr_filtrato.CopyToDataTable()
                                                              Group row By g = New With {Key ._Piva = row.Field(Of String)("Piva"), Key ._Sa_Cod = row.Field(Of Integer)("Sa_Cod"),
                                                                   Key ._Appezza = row.Field(Of Integer)("Appezza"),
                                                                 Key ._Id_Destinazione = row.Field(Of Integer)("Id_Destinazione"), Key ._Sup_Trattata = row("Qta2")} Into Group
                                                              Select CDec(g._Sup_Trattata)).Distinct().Sum(Function(sup) sup)


                        If Sup_Trattata_Totale > 0 Then
                            For Each d In Dr_filtrato

                                Dim Acqua_Impianto As Decimal = 0

                                Dim Acqua_Ha As Decimal = 0

                                If d("Qta_Ril") > 0 Then
                                    Dim Acqua_Totale As Decimal = d("Qta_Ril")
                                    Acqua_Ha = Acqua_Totale / Sup_Trattata_Totale
                                    Acqua_Impianto = Acqua_Ha * d("Qta2")
                                Else
                                    Acqua_Ha = -d("Qta_Ril")
                                    Acqua_Impianto = Acqua_Ha * d("Qta2")
                                End If

                                list_prodotti_x_impianti.Add(New Qta_Prodotto_X_Impianto With {
                                       .Lav_Cod = d("Lav_Cod"),
                                       .Id_Agenda = d("Id_Agenda"),
                                       .Pro_Cod = d("Pro_Cod"),
                                       .Piva = d("Piva"),
                                       .Sa_Cod = d("Sa_Cod"),
                                       .Appezza = d("Appezza"),
                                       .Id_Destinazione = d("Id_Destinazione"),
                                       .Sup_Impianto = d("Qta2"),
                                       .Qta_Prodotto = d("Qta"),
                                       .Qta_Acqua = Acqua_Impianto
                                 })
                            Next
                        End If


                    End If

                End If
            Next
        End If

        Return list_prodotti_x_impianti
    End Function

    Public Function List_Validita_Rilievi_Fasi_Fenologiche_X_Impianti(ByVal Piva As String, ByVal IDTestataTemp As Integer, ByVal Validita_Inizio As Date, ByVal Validita_Fine As Date, ByVal objParametri_Server As AgronicaCoreParametri) As List(Of Validita_Rilievi_FF_X_Impianto)

        Dim objAgronicaCoreContabDAL As New AgronicaCoreContabDAL.Movimenti_R

        Dim list_validita_x_impianti As New List(Of Validita_Rilievi_FF_X_Impianto)

        Dim DT = objAgronicaCoreContabDAL.Ottieni_Validita_Rilievi_Fasi_Fenologiche_per_Impianti(Piva, 0, Validita_Inizio, Validita_Fine,
                                                                                                 "", "", objParametri_Server, IDTestataTemp)
        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            For Each Dr In DT.Rows
                If list_validita_x_impianti.FindIndex(Function(validita_x_impianto) validita_x_impianto.Piva = Dr("Piva") AndAlso
                                                          validita_x_impianto.Sa_Cod = Dr("Sa_Cod") AndAlso validita_x_impianto.Appezza = Dr("Appezza") AndAlso
                                                          validita_x_impianto.Id_Destinazione = Dr("Id_Destinazione")) = -1 Then

                    Dim Dr_filtrato = DT.Select("Piva = '" & Dr("Piva") & "' And Sa_Cod = " & Dr("Sa_Cod") & " And Appezza = " & Dr("Appezza") &
                                                " And Id_Destinazione = " & Dr("Id_Destinazione"))

                    If Not IsNothing(Dr_filtrato) AndAlso Dr_filtrato.Length > 0 Then

                        Dim DT_filtrato As DataTable = Dr_filtrato.CopyToDataTable()

                        If Not IsNothing(DT_filtrato) AndAlso DT_filtrato.Rows.Count > 0 Then
                            Dim validita As New Validita_Rilievi_FF_X_Impianto With {
                                               .Piva = Dr_filtrato(0)("Piva"),
                                               .Sa_Cod = Dr_filtrato(0)("Sa_Cod"),
                                               .Appezza = Dr_filtrato(0)("Appezza"),
                                               .Id_Destinazione = Dr_filtrato(0)("Id_Destinazione")
                            }

                            Dim List_Stadi_Principali = (From row In DT_filtrato.Rows
                                                         Select row("Stadio_Principale")).Distinct().ToList()

                            If Not IsNothing(List_Stadi_Principali) AndAlso List_Stadi_Principali.Count > 0 Then

                                For Each stadio_principale In List_Stadi_Principali

                                    Dim Dr_temp = DT_filtrato.Select("Stadio_Principale = " & stadio_principale)

                                    If Not IsNothing(Dr_temp) AndAlso Dr_temp.Count > 0 Then

                                        Dim Dt_temp = Dr_temp.CopyToDataTable()

                                        Dim Descrizione_Stadio_Principale = Dr_temp(0)("Descrizione_StadiCrescita")

                                        Dim Validita_Inizio_Rilievo_FF As Date = Dt_temp.Compute("MIN(Validita_Inizio)", "")

                                        Dim Validita_Fine_Rilievo_FF As Date = Dt_temp.Compute("MAX(Validita_Inizio)", "")

                                        Dim stadio_FF As New Stadio_FF With {
                                            .Stadio_Principale = stadio_principale,
                                            .Descrizione_Stadio_Principale = Descrizione_Stadio_Principale
                                        }

                                        If Validita_Inizio_Rilievo_FF = Validita_Fine_Rilievo_FF Then
                                            Dim Dr_Validita_Inizio = Dt_temp.Select("Validita_Inizio = #" & Validita_Inizio_Rilievo_FF.ToString("yyyy-MM-dd") & "#")

                                            If Not IsNothing(Dr_Validita_Inizio) AndAlso Dr_Validita_Inizio.Count > 0 Then

                                                stadio_FF.Id_Agenda_Validita_Fine = Dr_Validita_Inizio(0)("Id_Agenda")
                                                stadio_FF.Id_Agenda_Validita_Inizio = Dr_Validita_Inizio(0)("Id_Agenda")
                                                stadio_FF.Des_Lib_Validita_Fine = Dr_Validita_Inizio(0)("Des_Lib")
                                                stadio_FF.Des_Lib_Validita_Inizio = Dr_Validita_Inizio(0)("Des_Lib")
                                                stadio_FF.Validita_Inizio = Validita_Inizio_Rilievo_FF
                                                stadio_FF.Validita_Fine = Validita_Inizio_Rilievo_FF

                                                validita.Stadi_FF.Add(stadio_FF)
                                            End If


                                        Else
                                            Dim Dr_Validita_Inizio = Dt_temp.Select("Validita_Inizio = #" & Validita_Inizio_Rilievo_FF.ToString("yyyy-MM-dd") & "#")

                                            If Not IsNothing(Dr_Validita_Inizio) AndAlso Dr_Validita_Inizio.Count > 0 Then
                                                stadio_FF.Id_Agenda_Validita_Inizio = Dr_Validita_Inizio(0)("Id_Agenda")
                                                stadio_FF.Validita_Inizio = Validita_Inizio_Rilievo_FF
                                                stadio_FF.Des_Lib_Validita_Inizio = Dr_Validita_Inizio(0)("Des_Lib")
                                            End If

                                            Dim Dr_Validita_Fine = Dt_temp.Select("Validita_Inizio = #" & Validita_Fine_Rilievo_FF.ToString("yyyy-MM-dd") & "#")

                                            If Not IsNothing(Dr_Validita_Fine) AndAlso Dr_Validita_Fine.Count > 0 Then
                                                stadio_FF.Id_Agenda_Validita_Fine = Dr_Validita_Fine(0)("Id_Agenda")
                                                stadio_FF.Validita_Fine = Validita_Fine_Rilievo_FF
                                                stadio_FF.Des_Lib_Validita_Fine = Dr_Validita_Fine(0)("Des_Lib")
                                            End If

                                            validita.Stadi_FF.Add(stadio_FF)
                                        End If
                                    End If
                                Next
                            End If

                            list_validita_x_impianti.Add(validita)
                        End If

                    End If
                End If
            Next
        End If

        Return list_validita_x_impianti

    End Function

    Public Function CaricaTrappole(ByVal filtro As String, ByVal piva As String,
                                   ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard

        Dim jSonDatiTESTATA As JObject = JObject.Parse(filtro)

        Dim r As New RispostaStandard()

        Dim sa_cod As Integer = 0
        Dim veg_cod As Integer = 0
        Dim id_cod As Integer = 0
        Dim flag_TerrenoNudo As Boolean = True
        Dim Validita_InizioStr As String
        Dim Validita_InizioDate As DateTime
        Dim Validita_FineStr As String
        Dim Validita_FineDate As DateTime

        'Se a true cerco le Operazioni che possono essere salvate senza specie vegetale (quindi senza impianti selezionati)
        Dim flag_NessunaSpecieQdC As Boolean = False

        Try
            flag_TerrenoNudo = jSonDatiTESTATA("flag_TerrenoNudo").ToString
        Catch ex As Exception
            flag_TerrenoNudo = True
        End Try

        Try
            flag_NessunaSpecieQdC = If(Not IsNothing(jSonDatiTESTATA("flag_NessunaSpecieQdC")) AndAlso
                                        Not String.IsNullOrEmpty(jSonDatiTESTATA("flag_NessunaSpecieQdC")) AndAlso
                                       jSonDatiTESTATA("flag_NessunaSpecieQdC").ToString().ToLower() = "true", True, False)
        Catch ex As Exception
            flag_NessunaSpecieQdC = False
        End Try

        If IsNumeric(jSonDatiTESTATA("veg_cod")) Then
            veg_cod = jSonDatiTESTATA("veg_cod")
        ElseIf jSonDatiTESTATA("veg_cod") IsNot Nothing AndAlso jSonDatiTESTATA("veg_cod").ToString.Contains("/") Then
            If IsNumeric(jSonDatiTESTATA("veg_cod").ToString.Split("/")(1)) Then
                id_cod = jSonDatiTESTATA("veg_cod").ToString.Split("/")(1)
            End If
        End If

        If IsNumeric(jSonDatiTESTATA("sa_cod")) Then
            sa_cod = jSonDatiTESTATA("sa_cod")
        End If

        Validita_InizioStr = jSonDatiTESTATA("txt_Data1").ToString
        Validita_InizioDate = If(IsDate(Validita_InizioStr), CDate(Validita_InizioStr), AGRODATAINIZIO)

        Validita_FineStr = jSonDatiTESTATA("txt_Data2").ToString
        Validita_FineDate = If(IsDate(Validita_FineStr), CDate(Validita_FineStr), AGRODATAFINE)

        Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R

        Dim DT = objOperazioni.Leggi_Trappole(Piva:=piva,
                                                Sa_Cod:=sa_cod,
                                                Validita_Inizio:=Validita_InizioDate, Validita_Fine:=Validita_FineDate,
                                                Veg_Cod:=veg_cod,
                                                Id_Cod:=id_cod,
                                                xOrderBy:="",
                                                objParametri_Server:=objParametri_Server,
                                                objParametri_Utenti:=objParametri_Utenti)

        Dim serializerSettings As New JsonSerializerSettings()
        serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

        r.RispostaOK = True
        r.RispostaStringa = JsonConvert.SerializeObject(DT, serializerSettings)

        Return r
    End Function

#Region "Funzioni Utility Crea Tabella Agenda"

    Private Shared Sub impostaDatiBaseOperazione(ByRef Dr As DataRow,
                                                 ByRef DrAgenda() As DataRow)
        '  Vanni, 23/06/2015 16:37:51: imposto piva e sa_cod così come vengono su da query
        Dr.Item("Piva") = DrAgenda(0).Item("Piva")
        Dr.Item("Sa_Cod") = DrAgenda(0).Item("Sa_Cod")
        Dr.Item("Lav_Cod") = DrAgenda(0).Item("Lav_Cod")

        Dr.Item("Data") = CDate(DrAgenda(0).Item("Data_Movimento")).ToShortDateString
        Dr.Item("Data2") = CDate(DrAgenda(0).Item("Data_Movimento"))
        '(05/12/2018) per le fasi visualizzo la data del rilievo (validita_inizio nella destinazione)
        'le fasi nuove creano un id_agenda per centro, fase, data
        'le vecchie ne avevano 1 per centro con fasi e date diverse assieme
        '(per queste ultime visualizzo una data in caso ci siano date diverse nello stesso rilievo)
        Select Case Dr.Item("Lav_Cod")

            Case LAVCOD_FASI_FENOLOGICHE

                Dr.Item("Data") = CDate(DrAgenda(0).Item("validita_inizio_destinazione")).ToShortDateString
                Dr.Item("Data2") = CDate(DrAgenda(0).Item("validita_inizio_destinazione"))

        End Select

        Dr.Item("Lav_Des") = DrAgenda(0).Item("Lav_Des") 'DrAgenda(0).Item("Des_Lib")                                                                                               
        Dr.Item("Ora") = CDate(DrAgenda(0).Item("Ora"))
        Dr.Item("Id_Agenda") = DrAgenda(0).Item("Id_Agenda")
        Dr.Item("Id_Mov_Det") = DrAgenda(0).Item("Id_Mov_Det")
        Dr.Item("Blocco_Flag") = 0
        Dr.Item("Raccoglitore_Cod") = 0
        Dr.Item("Info") = ""
        Dr.Item("Dettagli") = ""
        Dr.Item("Veg_Cod") = 0
        Dr.Item("Ricetta_Cod") = 0
        Dr.Item("Rag_Soc") = DrAgenda(0).Item("Rag_Soc")
        Dr.Item("Operazione_DES") = DrAgenda(0).Item("lav_des")
        Dr.Item("tipo") = DrAgenda(0).Item("tipo")
    End Sub

    Private Shared Sub trackLottoPadre(ByRef Dr As DataRow,
                                       ByRef DrAgenda() As DataRow,
                                       ByRef DtAgenda As DataTable)
        If DtAgenda.Columns.Contains("FF_Track_Lotto_Padre") Then
            Dr.Item("FF_Track_Cal_Cod_Padre") = CStr(DrAgenda(0).Item("FF_Track_Cal_Cod_Padre"))
            Dr.Item("FF_Track_Cal_Cod") = CStr(DrAgenda(0).Item("FF_Track_Cal_Cod"))
            Dr.Item("FF_Track_Lotto_Padre") = DrAgenda(0).Item("FF_Track_Lotto_Padre")
            Dr.Item("FF_Track_Lotto") = DrAgenda(0).Item("FF_Track_Lotto")
            Dr.Item("FF_Track_Qta_Extra_Totale") = DrAgenda(0).Item("FF_Track_Qta_Extra_Totale")
            Dr.Item("FF_Track_Qta_Contenitori") = DrAgenda(0).Item("FF_Track_Qta_Contenitori")
            Dr.Item("FF_Track_Qta_Imballi") = DrAgenda(0).Item("FF_Track_Qta_Imballi")
            Dr.Item("FF_codice_generazione") = DrAgenda(0).Item("FF_codice_generazione")
            Dr.Item("FF_Mat_Cod") = DrAgenda(0).Item("Mat_Cod")
            Dr.Item("FF_Linea_Cod") = DrAgenda(0).Item("FF_Linea_Cod")
            'Else
            '    Dr.Item("FF_Track_Cal_Cod_Padre") = 0
            '    Dr.Item("FF_Track_Cal_Cod") = 0
            '    Dr.Item("FF_Track_Lotto_Padre") = ""
            '    Dr.Item("FF_Track_Lotto") = ""
            '    Dr.Item("FF_Track_Qta_Extra_Totale") = 0.0

        End If
    End Sub

    Private Shared Sub setSpecieOperazione(ByRef Dr As DataRow,
                                 ByRef strSpecie As String,
                                 ByRef Veg_Cod As Integer,
                                 ByRef objSqlDis As AgronicaCoreUtility.DatatableUtility,
                                 ByRef DtOperazione As DataTable)
        Dim DtSpecie = objSqlDis.SelectDistinct("Specie", DtOperazione, "veg_cod", False)
        'DtSpecie = DtOperazione.DefaultView.ToTable(True, "veg_cod", "veg_des", "DestinazioneTerreniNudi_Des", "appezza")

        For Each r As DataRow In DtSpecie.Rows
            Veg_Cod = r.Item("veg_cod")

            If r.Item("veg_cod") <> 0 Then
                'Veg_Cod = DtSpecie.Rows(j).Item("veg_cod")
                Dim Veg_Des As String = r.Item("veg_des")
                If strSpecie <> "" Then
                    strSpecie &= ", " & Veg_Des
                Else
                    strSpecie = Veg_Des
                End If

            ElseIf r.Item("DestinazioneTerreniNudi_Des") <> "" Then
                'Veg_Cod = r.Item("veg_cod")
                Dim DestinazioneTerreniNudi_Des As String = r.Item("DestinazioneTerreniNudi_Des")
                If strSpecie <> "" Then
                    strSpecie &= ", " & DestinazioneTerreniNudi_Des
                Else
                    strSpecie = DestinazioneTerreniNudi_Des
                End If

            ElseIf r.Item("appezza") <> 0 Then
                'Veg_Cod = r.Item("veg_cod")
                If strSpecie <> "" Then
                    strSpecie &= ", " & Str_TerrenoNudo
                Else
                    strSpecie = Str_TerrenoNudo
                End If
            End If

        Next

        Dr.Item("Veg_Cod") = Veg_Cod
    End Sub

    Private Shared Function gestioneAppezzamentiOperazione(ByRef Dr As DataRow,
                                                           ByRef strCentro As String,
                                                           ByRef strAppezzamenti As String,
                                                           ByRef AppezzamentoNome As String,
                                                           ByRef strCodici_Appezzamenti As String,
                                                           ByRef strPkImpianti As String,
                                                           ByRef strCodici_Impianto As String,
                                                           ByRef strSpecieVarieta As String,
                                                           ByRef strCulDes As String,
                                                           ByRef strKPIN As String,
                                                           ByRef strBlockName As String,
                                                           Visualizza_Codici_AppezzaImpianti As Boolean,
                                                           Visualizza_KPIN_BlockName As Boolean,
                                                           ByRef objSqlDis As AgronicaCoreUtility.DatatableUtility,
                                                           ByRef DtOperazione As DataTable,
                                                           Optional estraiPkImpianti As Boolean? = Nothing) As DataTable

        Dim objImpianto_codici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dim DtApp = objSqlDis.SelectDistinct("Appezzamenti", DtOperazione, "appezza", False)
        'DtApp = DtOperazione.DefaultView.ToTable(True, "piva", "sa_cod", "appezza", "Sa_Nome", "App_Nome", "cul_des",
        '                                         "veg_des", "veg_cod", "DestinazioneTerreniNudi_Des", "LottoImpianto", "ID_Reg")

        strCentro = If(IsDBNull(DtApp.Rows(0).Item("Sa_Nome")), "", DtApp.Rows(0).Item("Sa_Nome"))

        For Each r As DataRow In DtApp.Rows

            If estraiPkImpianti Then
                Dim pivaImp As String = If(r.Item("piva") Is DBNull.Value, "", r.Item("piva"))
                Dim sacodImp As String = If(r.Item("sa_cod") Is DBNull.Value, 0, r.Item("sa_cod"))
                Dim appImp As String = If(r.Item("appezza") Is DBNull.Value, 0, r.Item("appezza"))
                Dim idRegImp As String = If(r.Item("id_reg") Is DBNull.Value, 0, r.Item("id_reg"))
                strPkImpianti &= String.Format("{0},{1},{2},{3}| ", pivaImp, sacodImp, appImp, idRegImp)
            End If


            If r.Item("App_Nome") <> "" Then
                AppezzamentoNome = Replace(r.Item("App_Nome"), "'", "")
                strAppezzamenti &= AppezzamentoNome & ", "
            End If
            If r.Item("cul_des") <> "" AndAlso InStr(strCulDes, r.Item("cul_des")) = 0 Then
                strCulDes &= Replace(r.Item("cul_des"), "'", "") & ", "
            End If

            If r.Item("cul_des") <> "" AndAlso InStr(strSpecieVarieta, r.Item("cul_des")) = 0 Then
                Dim specie As String = If(r.Item("veg_cod") <> 0, r.Item("veg_des") & " - ", "")
                Dim varieta As String = Replace(r.Item("cul_des"), "'", "")
                strSpecieVarieta &= specie & varieta & ", "
            End If

            If r.Item("veg_cod") = 0 Then
                If r.Item("DestinazioneTerreniNudi_Des") <> "" Then
                    If InStr(strSpecieVarieta, r.Item("DestinazioneTerreniNudi_Des")) = 0 Then
                        Dim destinazioneTN As String = Replace(r.Item("DestinazioneTerreniNudi_Des"), "'", "")
                        strSpecieVarieta &= destinazioneTN & ", "
                    End If
                ElseIf r.Item("Appezza") <> 0 Then
                    strSpecieVarieta &= Str_TerrenoNudo & ", "
                End If
            End If


            If Visualizza_Codici_AppezzaImpianti AndAlso r("appezza") <> 0 Then

                Dim Riferimento_Alfanumerico_Appezzamento = r("Riferimento_Alfanumerico_Appezzamento")
                If Riferimento_Alfanumerico_Appezzamento <> "" Then
                    strCodici_Appezzamenti &= Riferimento_Alfanumerico_Appezzamento & ", "
                End If

                Dim Codice_Impianto = r("Codice_Impianto")

                If Codice_Impianto <> "" Then
                    strCodici_Impianto &= Codice_Impianto & ", "
                End If

            End If

            If Visualizza_KPIN_BlockName AndAlso r("appezza") <> 0 Then

                Dim impKPIN = r("Zespri_Codice_kPIN")

                If impKPIN <> "" Then
                    strKPIN &= impKPIN & ", "
                End If

                Dim impBlockName = r("Zespri_Block_Name")

                If impBlockName.Trim <> "" Then
                    strBlockName &= impBlockName.Trim & ", "
                End If

            End If

        Next

        If Not IsNothing(estraiPkImpianti) AndAlso estraiPkImpianti.HasValue AndAlso estraiPkImpianti = True Then
            If strPkImpianti <> "" Then
                strPkImpianti = Left(strPkImpianti, strPkImpianti.Length - 2)
            End If
            Dr.Item("PK_Impianti") = strPkImpianti
        End If

        If strCodici_Appezzamenti <> "" Then
            strCodici_Appezzamenti = Left(strCodici_Appezzamenti, strCodici_Appezzamenti.Length - 2)
        End If

        If strCodici_Impianto <> "" Then
            strCodici_Impianto = Left(strCodici_Impianto, strCodici_Impianto.Length - 2)
        End If

        If strKPIN <> "" Then
            strKPIN = Left(strKPIN, strKPIN.Length - 2)
        End If

        If strBlockName <> "" Then
            strBlockName = Left(strBlockName, strBlockName.Length - 2)
        End If

        If strAppezzamenti <> "" Then
            strAppezzamenti = Left(strAppezzamenti, strAppezzamenti.Length - 2)
        End If
        If strCulDes <> "" Then
            strCulDes = Left(strCulDes, strCulDes.Length - 2)
        End If
        If strSpecieVarieta <> "" Then
            strSpecieVarieta = Left(strSpecieVarieta, strSpecieVarieta.Length - 2)
        End If

        If Visualizza_Codici_AppezzaImpianti Then
            Dr.Item("Codici_Appezzamenti") = strCodici_Appezzamenti
            Dr.Item("Codici_Impianto") = strCodici_Impianto
        End If

        If Visualizza_KPIN_BlockName Then
            Dr.Item("KPIN") = strKPIN
            Dr.Item("BlockName") = strBlockName
        End If

        Return DtApp
    End Function

    Private Shared Function gestioneProdottiMagazzinoTrattatiOperazione(ByRef Dr As DataRow,
                                                                        ByRef DtOperazione As DataTable,
                                                                        ByRef strCulDes As String,
                                                                        ByRef strSpecieVarieta As String,
                                                                        ByRef strLottiProdottiMagazzinoTrattati As String,
                                                                        ByRef objSqlDis As AgronicaCoreUtility.DatatableUtility
                                                                        ) As String
        Dim strProdottiMagazzinoTrattati As String = ""

        Dim DtProdotti As New DataTable
        Select Case CInt(DtOperazione(0).Item("Lav_Cod"))
            Case LAVCOD_CONCIA_SEME
                DtProdotti = DtOperazione.AsEnumerable().
                    Where(Function(row) row.Field(Of Integer)("Veg_Cod") <> 0).
                    GroupBy(Function(row) row.Field(Of Integer)("Mat_Cod")).
                    Select(Function(group) group.First()).
                    CopyToDataTable()
            Case Else
                DtProdotti = objSqlDis.SelectDistinct("ProdottiMagazzino", DtOperazione, "mat_cod", False)
        End Select

        Dim listaProdottiMagazzino As New List(Of String)
        Dim listaVarieta As New List(Of String)
        Dim listaSpecieVarieta As New List(Of String)
        Dim listaLotti As New List(Of String)

        For Each r As DataRow In DtProdotti.Rows
            If r.Item("Elem_Cod") = SEMENTI OrElse r.Item("Elem_Cod") = TRASFORMATI_VEGETALI Then
                If r.Item("Mat_Des") <> "" Then
                    Dim prodotto As String = Replace(r.Item("Mat_Des"), "'", "")

                    If r.Item("Cod_Articolo") <> "" Then
                        prodotto &= " (" + Gias.CodArticolo + ": " + Replace(r.Item("Cod_Articolo"), "'", "") & ")"
                    End If

                    listaProdottiMagazzino.Add(prodotto)
                End If

                If r.Item("cul_des") <> "" Then
                    listaVarieta.Add(Replace(r.Item("cul_des"), "'", ""))

                    Dim specie As String = If(r.Item("veg_cod") <> 0, r.Item("veg_des") & " - ", "")
                    Dim varieta As String = Replace(r.Item("cul_des"), "'", "")
                    listaSpecieVarieta.Add(specie & varieta)
                End If

                If CStr(r.Item("LottoProduzione")) <> "" Then
                    listaLotti.Add(Replace(CStr(r.Item("LottoProduzione")), "'", ""))
                End If
            End If
        Next

        If listaProdottiMagazzino.Count > 0 Then
            strProdottiMagazzinoTrattati = String.Join(", ", listaProdottiMagazzino.Distinct.ToList())
        End If
        If listaVarieta.Count > 0 Then
            strCulDes = String.Join(", ", listaVarieta.Distinct.ToList())
        End If
        If listaSpecieVarieta.Count > 0 Then
            strSpecieVarieta = String.Join(", ", listaSpecieVarieta.Distinct.ToList())
        End If
        If listaLotti.Count > 0 Then
            strLottiProdottiMagazzinoTrattati = String.Join(", ", listaLotti.Distinct.ToList())
        End If

        Return strProdottiMagazzinoTrattati

    End Function


    Private Shared Sub setProdottiOperazione(ByRef Dr As DataRow,
                                             ByRef DrAgenda() As DataRow,
                                             ByRef DtCategorieMagazzino As DataTable,
                                             ByRef DtProdotti As DataTable,
                                             ByRef DtProdotti1 As DataTable,
                                             ByRef Prodotto As String,
                                             ByRef strProdotti As String,
                                             ByRef righeAggiunte As String,
                                             ByRef FF_TrackedData_Cod As Integer,
                                             ByRef lockObject As Object,
                                             ByRef objSqlDis As AgronicaCoreUtility.DatatableUtility,
                                             ByRef DtOperazione As DataTable,
                                             ByRef objparametri_Server As AgronicaCoreParametri)

        Dim Elem_Cod As Integer = CInt(DrAgenda(0).Item("Elem_Cod"))
        If (DrAgenda(0).Item("Lav_Cod") = LAVCOD_CONCIA_SEME OrElse DrAgenda(0).Item("Lav_Cod") = LAVCOD_TRATTAMENTO_POST_RACCOLTA) Then
            'Se stiamo analizzando lav_cod di Concia del Seme o Trattamenti Post Raccolta, forzo il prodotto utilizzato a FORMULATO
            Elem_Cod = FORMULATI
        End If

        Dim NomeComune As String = ""
        Dim drCategoriaMagazzino = DtCategorieMagazzino.Select(" Elem_Cod = " & Elem_Cod)
        If drCategoriaMagazzino.Length > 0 Then
            NomeComune = drCategoriaMagazzino(0)("NomeComune")
        End If
        Dr.Item("NomeComune") = NomeComune

        Select Case Elem_Cod

            Case FERTILIZZANTI  'FERTILIZZANTI
                DtProdotti = objSqlDis.SelectDistinct("Fertilizzanti", DtOperazione, "pro_cod", False)
                DtProdotti1 = objSqlDis.SelectDistinct("Fertilizzanti1", DtOperazione, "mat_cod", False)
                For Each r As DataRow In DtProdotti.Rows
                    If r.Item("Pro_Cod") <> 0 Then
                        Prodotto = r.Item("Fer_Des")
                        strProdotti &= Prodotto & ", "
                    End If
                Next
                For Each r As DataRow In DtProdotti1.Rows
                    If r.Item("Mat_Cod") <> 0 Then
                        Prodotto = r.Item("Mat_Des")
                        strProdotti &= Prodotto & ", "
                    End If
                Next
                If strProdotti <> "" Then
                    strProdotti = Left(strProdotti, strProdotti.Length - 2)
                    strProdotti = "<b>" & My.Resources.AgronicaCoreContabBIZ.ProdottiUtilizzati & "</b> " & strProdotti
                End If

            Case FORMULATI    'FORMULATI
                DtProdotti = objSqlDis.SelectDistinct("Formulati", DtOperazione, "pro_cod", False)

                For Each r As DataRow In DtProdotti.Rows
                    If r.Item("Fr_Des") <> "" AndAlso r.Item("Elem_Cod") = FORMULATI Then
                        Prodotto = r.Item("Fr_Des")
                        strProdotti &= Prodotto & ", "
                    End If
                Next
                If strProdotti <> "" Then
                    strProdotti = Left(strProdotti, strProdotti.Length - 2)
                    strProdotti = "<b>" & My.Resources.AgronicaCoreContabBIZ.ProdottiUtilizzati & "</b> " & strProdotti
                End If

            Case TRAPPOLE
                DtProdotti = objSqlDis.SelectDistinct("Trappole", DtOperazione, "pro_cod", False)

                For Each r As DataRow In DtProdotti.Rows
                    If r.Item("Trap_Des") <> "" Then
                        Prodotto = r.Item("Trap_Des")
                        strProdotti &= Prodotto & ", "
                    End If
                Next
                If strProdotti <> "" Then
                    strProdotti = Left(strProdotti, strProdotti.Length - 2)
                    strProdotti = "<b>" & My.Resources.AgronicaCoreContabBIZ.ProdottiUtilizzati & "</b> " & strProdotti
                End If

            Case INSETTI
                DtProdotti = objSqlDis.SelectDistinct("InsettiUtili", DtOperazione, "pro_cod", False)

                For Each r As DataRow In DtProdotti.Rows
                    If r.Item("Ins_Des") <> "" Then
                        Prodotto = r.Item("Ins_Des")
                        strProdotti &= Prodotto & ", "
                    End If
                Next
                If strProdotti <> "" Then
                    strProdotti = Left(strProdotti, strProdotti.Length - 2)
                    strProdotti = "<b>" & My.Resources.AgronicaCoreContabBIZ.ProdottiUtilizzati & "</b> " & strProdotti
                End If

            Case SEMENTI

                DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)

                For Each r As DataRow In DtProdotti.Rows
                    If r.Item("Mat_Des") <> "" Then
                        Dim codart As String = If(r.Item("Cod_Articolo") = "", "", "Articolo: " & r.Item("Cod_Articolo"))
                        'Dim lotto As String = If(r.Item("LottoProduzione") = "", "", "Lotto: " & r.Item("LottoProduzione"))
                        Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                        Prodotto = r.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                        strProdotti &= Prodotto & ", "
                    End If
                Next
                If strProdotti <> "" Then
                    strProdotti = Left(strProdotti, strProdotti.Length - 2)
                    strProdotti = My.Resources.AgronicaCoreContabBIZ.MaterialeVivaistaUtilizzato & strProdotti
                End If

            Case SEMILAVORATI_VEGETALI

                DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)
                For Each r As DataRow In DtProdotti.Rows
                    If r.Item("Mat_Des") <> "" Then
                        Prodotto = r.Item("Mat_Des")
                        strProdotti &= Prodotto & ", "
                    End If
                Next
                If strProdotti <> "" Then
                    strProdotti = Left(strProdotti, strProdotti.Length - 2)
                    If Dr.Item("Lav_Cod") = LAVCOD_TRATTAMENTO_POST_RACCOLTA Then
                        strProdotti = "Semilavorato trattato:" & strProdotti
                    Else
                        strProdotti = My.Resources.AgronicaCoreContabBIZ.SemilavoratoRaccolto & strProdotti
                    End If
                End If

            'vanni, 27/06/2017 gestito per operazioni F&F
            Case TRASFORMATI_VEGETALI
                If FF_TrackedData_Cod > 0 Then

                    Try
                        'Questa serve per dare una colorazione diversa alle righe aggiunte a parità di certificazione
                        Dr.Item("FF_Righe_Aggiunte") = righeAggiunte

                        Dim objEti As New AgronicaCoreStampeDAL.FF_Etichette_R

                        Dim xOrderByFF As String = ""
                        Dim xFiltroAggiuntivoFF As String = " detProd.Id_Mov_Det = " & DrAgenda(0).Item("Id_Mov_Det")
                        Dim DtProdottoFF As New DataTable
                        SyncLock lockObject
                            'etichette
                            DtProdottoFF = objEti.LeggiParametriQualitativi(
                                                DrAgenda(0).Item("id_Agenda"),
                                                1,
                                                FF_Etichette_tipo.Interne,
                                                DrAgenda(0).Item("cau_mov"),
                                                xFiltroAggiuntivoFF,
                                                "",
                                                objparametri_Server
                                            )
                        End SyncLock

                        For Each drrProdotto As DataRow In DtProdottoFF.Rows

                            For Each colProdotto As DataColumn In DtProdottoFF.Columns

                                If Not ({"specie", "varieta", "data", "ora", "qtakg", "note"}.Contains(colProdotto.ColumnName.ToLower)) Then

                                    If Not colProdotto.ColumnName.ToLower.Contains("_sigla") Then

                                        Dim parametroQualitativo As String = ""
                                        If Not drrProdotto(colProdotto.ColumnName) Is DBNull.Value Then
                                            parametroQualitativo = drrProdotto(colProdotto.ColumnName)
                                        End If

                                        If Not String.IsNullOrEmpty(parametroQualitativo) Then
                                            'If colProdotto.ColumnName = "Referenza" Then
                                            '    strProdotti &=
                                            '    "<b>" & colProdotto.ColumnName & "</b>: " & parametroQualitativo & "<br>"
                                            'Else
                                            Dr.Item("FF_" & colProdotto.ColumnName) = parametroQualitativo
                                            'End If
                                        End If
                                    End If
                                End If

                            Next

                        Next

                        If strProdotti <> "" Then
                            strProdotti &= "<br>"
                        End If

                    Catch ex As Exception

                    End Try
                End If


            Case Else

                DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)

                For Each r As DataRow In DtProdotti.Rows
                    If r.Item("Mat_Des") <> "" Then
                        Dim codart As String = If(r.Item("Cod_Articolo") = "", "", "Articolo: " & r.Item("Cod_Articolo"))
                        'Dim lotto As String = If(r.Item("LottoProduzione") = "", "", "Lotto: " & r.Item("LottoProduzione"))
                        Dim desProdotto As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                        Prodotto = r.Item("Mat_Des") & If(desProdotto = "", "", " (" & desProdotto & ")")
                        strProdotti &= Prodotto & ", "
                    End If
                Next
                If strProdotti <> "" Then
                    strProdotti = Left(strProdotti, strProdotti.Length - 2)
                End If
        End Select

    End Sub

    Private Shared Function getStrAvversita(DrAgenda() As DataRow, dtMovDetTec As DataTable)
        Dim drMovDetTec() As DataRow = dtMovDetTec.Select(" ID_Agenda=" & DrAgenda(0).Item("Id_Agenda"))
        Dim listaAvv As New List(Of String)

        For Each drAvv As DataRow In drMovDetTec
            If drAvv.Item("Av_des_vol") <> "" Then
                listaAvv.Add(drAvv.Item("Av_des_vol"))
            End If
            If drAvv.Item("Av_Gru_des") <> "" Then
                listaAvv.Add(drAvv.Item("Av_Gru_des"))
            End If
        Next

        Return String.Join(", ", listaAvv.Distinct)
    End Function

    Private Shared Function getStrCentroCampo(DtOperazione As DataTable)
        Dim listaCentriCampi As New List(Of String)

        For Each drCentriCampi As DataRow In DtOperazione.Rows
            Dim centro As String = If(Not IsDBNull(drCentriCampi.Item("Sa_Nome")) AndAlso Not IsNothing(drCentriCampi.Item("Sa_Nome")), drCentriCampi.Item("Sa_Nome"), "")
            Dim campo As String = If(Not IsDBNull(drCentriCampi.Item("Campo_Des")) AndAlso Not IsNothing(drCentriCampi.Item("Campo_Des")), drCentriCampi.Item("Campo_Des"), "")

            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
            Dim testoCentriCampi As String = String.Join(" - ", {centro, campo}.Where(Function(s) Not String.IsNullOrEmpty(s)))
            If Not listaCentriCampi.Contains(testoCentriCampi) Then
                listaCentriCampi.Add(testoCentriCampi)
            End If
        Next

        Return String.Join(", ", listaCentriCampi)
    End Function

    Private Shared Function getStrLottiProduzione(DtOperazione As DataTable)
        Dim listaLottiProduzione As New List(Of String)
        For Each drLottiProduzione As DataRow In DtOperazione.Rows
            If (drLottiProduzione.Item("Lav_Cod") = LAVCOD_CONCIA_SEME OrElse drLottiProduzione.Item("Lav_Cod") = LAVCOD_TRATTAMENTO_POST_RACCOLTA) AndAlso
                (drLottiProduzione.Item("Elem_Cod") = SEMENTI OrElse drLottiProduzione.Item("Elem_Cod") = TRASFORMATI_VEGETALI) Then
                'Se stiamo analizzando lav_cod di Concia del Seme o Trattamenti Post Raccolta, evito di mettere il lotto del prodotto magazzino 
                Continue For
            End If

            Dim LottoProduzione As String = If(Not IsDBNull(drLottiProduzione.Item("LottoProduzione")), drLottiProduzione.Item("LottoProduzione"), "")

            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
            If LottoProduzione.Trim() <> "" AndAlso Not listaLottiProduzione.Contains(LottoProduzione) Then
                listaLottiProduzione.Add(LottoProduzione)
            End If
        Next

        Return String.Join(", ", listaLottiProduzione)
    End Function

    Private Shared Function getStrLottiImpianto(DtApp As DataTable)
        Dim listaLottiImpianto As New List(Of String)

        For Each drLottiImpianto As DataRow In DtApp.Rows
            Dim LottoImpianto As String = If(Not IsDBNull(drLottiImpianto.Item("LottoImpianto")), drLottiImpianto.Item("LottoImpianto"), "")

            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
            If LottoImpianto.Trim() <> "" Then
                listaLottiImpianto.Add(LottoImpianto)
            End If
        Next

        Return String.Join(", ", listaLottiImpianto)
    End Function

    Private Shared Function getStrNote(DtOperazione As DataTable)
        Dim listaNote As New List(Of String)
        For Each drNote As DataRow In DtOperazione.Rows
            Dim Nota As String = If(Not IsDBNull(drNote.Item("Nota_Des")), drNote.Item("Nota_Des"), "")

            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
            If Nota.Trim() <> "" AndAlso Not listaNote.Contains(Nota) Then
                listaNote.Add(Nota)
            End If
        Next

        'Nota libera
        For Each drNote As DataRow In DtOperazione.Rows
            Dim Nota As String = If(Not IsDBNull(drNote.Item("Mov_desc")), drNote.Item("Mov_desc"), "")

            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
            If Nota.Trim() <> "" AndAlso Not listaNote.Contains(Nota) Then
                listaNote.Add(Nota)
            End If
        Next

        Return String.Join(", ", listaNote)
    End Function

    #Region "descrizione prodotti"

    Private Shared Sub concatenaQtaProdotto(ByRef prod As String, drDetTec As DataRow)
        If drDetTec.Item("Qta_Extra_Totale") <> 0 Then
            prod = prod & " (" & Agro_Math.RoundNumber_2Decimali(drDetTec.Item("Qta_Extra_Totale")) & " " & drDetTec.Item("UDM_SIM_ExtraInt") & " )"
        ElseIf drDetTec.Item("Qta") <> 0 Then
            prod = prod & " (" & Agro_Math.RoundNumber_2Decimali(drDetTec.Item("Qta")) & " " & drDetTec.Item("UDM_SIM_UDM_COD") & " )"
        End If
    End Sub

    ''' <summary>
    ''' Legge il prodotto o il formulato indicato per la riga specificata.
    ''' </summary>
    ''' <param name="drDetTec"></param>
    ''' <returns>Una stringa contenente la descrizione del prodotto o del formulato</returns>
    Private Shared Function getDescrizioneAltriProdotti(drDetTec As DataRow) As String
        Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
        'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
        Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))
        Dim prod As String = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")

        If prod = "" AndAlso Not IsRilievo(drDetTec.Item("Lav_Cod")) Then
            prod = drDetTec.Item("Fr_Des")
        End If
        concatenaQtaProdotto(prod, drDetTec)

        Return prod
    End Function

    Private Shared function GetAllProductsString(table As DataTable, getQuantity As Boolean) As string
        return table.AsEnumerable().
            Select(Function(dr) GetProductString(dr, getQuantity)).
            DefaultIfEmpty(String.Empty).
            Aggregate(Function(acc, str) acc & ", " & str)
    End function

    Private shared function GetProductString(drDetTec As DataRow, getQuantity As boolean) As String
        Dim prod = ""
        Dim nessunProdotto As Integer = 0
        Dim elemCod = CInt(drDetTec.Item("Elem_Cod"))

        Select Case elemCod
            Case FERTILIZZANTI
                If drDetTec.Item("Pro_Cod") <> 0 Then
                    prod = drDetTec.Item("Fer_Des") & ", "
                End If
                If drDetTec.Item("Mat_Cod") <> 0 Then
                    prod = drDetTec.Item("Mat_Des")
                Else
                    If prod.Length > 0 Then
                        prod = Left(prod, prod.Length - 2)
                    End If
                End If
                If (getQuantity) Then
                    concatenaQtaProdotto(prod, drDetTec)
                End If

            Case FORMULATI
                If drDetTec.Item("Fr_Des") <> "" Then
                    prod = drDetTec.Item("Fr_Des")
                    If (getQuantity) Then
                        concatenaQtaProdotto(prod, drDetTec)
                    End If
                End If

            Case TRAPPOLE
                If drDetTec.Item("Trap_Des") <> "" Then
                    prod = drDetTec.Item("Trap_Des")
                    If (getQuantity) Then
                        concatenaQtaProdotto(prod, drDetTec)
                    End If
                End If

            Case INSETTI
                If drDetTec.Item("Ins_Des") <> "" Then
                    prod = drDetTec.Item("Ins_Des")
                    If (getQuantity) Then
                        concatenaQtaProdotto(prod, drDetTec)
                    End If
                End If

            Case SEMENTI

                If drDetTec.Item("Mat_Des") <> "" Then
                    Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                    'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                    Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))

                    prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                    If (getQuantity) Then
                        concatenaQtaProdotto(prod, drDetTec)
                    End If
                End If

            Case SEMILAVORATI_VEGETALI
                If drDetTec.Item("Mat_Des") <> "" Then
                    If drDetTec.Item("Lav_Cod") = LAVCOD_TRATTAMENTO_POST_RACCOLTA Then
                        prod = "Semilavorato trattato:" & drDetTec.Item("Mat_Des")
                    Else
                        prod = My.Resources.AgronicaCoreContabBIZ.SemilavoratoRaccolto & drDetTec.Item("Mat_Des")
                        If drDetTec.Item("Qta") <> 0 AndAlso getQuantity Then
                            prod = prod & " (" & Agro_Math.ArrotondaVal_2(drDetTec.Item("Qta")) & " " & drDetTec.Item("UDM_SIM_UDM_COD") & " )"
                        End If
                    End If
                End If

            Case TRASFORMATI_VEGETALI
                If drDetTec.Item("Mat_Des") <> "" Then
                    Dim codart As String = If(drDetTec.Item("Cod_Articolo") = "", "", "Articolo: " & drDetTec.Item("Cod_Articolo"))
                    'Dim lotto As String = If(drDetTec.Item("LottoProduzione") = "", "", "Lotto: " & drDetTec.Item("LottoProduzione"))
                    Dim desSemente As String = String.Join(" - ", {codart}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                    prod = drDetTec.Item("Mat_Des") & If(desSemente = "", "", " (" & desSemente & ")")
                    If drDetTec.Item("Qta") <> 0 AndAlso getQuantity Then
                        prod = prod & " (" & Agro_Math.ArrotondaVal_2(drDetTec.Item("Qta")) & " " & drDetTec.Item("UDM_SIM_UDM_COD") & " )"
                    End If
                End If

            Case nessunProdotto
                'Nessun prodotto da indicare

            Case Else
                prod = getDescrizioneAltriProdotti(drDetTec)

        End Select

        Return prod
    End function

    #End Region

    Private Shared Function getStrDettaglioTecnicoOperazione(DrAgenda() As DataRow,
                                                             DtOperazione As DataTable,
                                                             dtMovDetTec As DataTable,
                                                             dtMovDetTecFasi As DataTable,
                                                             HtProdPA As Hashtable,
                                                             HtPrincAtt As Hashtable,
                                                             objParametriUscitaFasiOld As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output,
                                                             objParametriUscitaFasiNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output)
        Dim listaDetTec As New List(Of String)
        Dim nessunProdotto As Integer = 0

        For Each drDetTec As DataRow In DtOperazione.Rows

            If (drDetTec.Item("Lav_Cod") = LAVCOD_CONCIA_SEME OrElse drDetTec.Item("Lav_Cod") = LAVCOD_TRATTAMENTO_POST_RACCOLTA) AndAlso
                (drDetTec.Item("Elem_Cod") = SEMENTI OrElse drDetTec.Item("Elem_Cod") = TRASFORMATI_VEGETALI) Then
                'Se stiamo analizzando lav_cod di Concia del Seme o Trattamenti Post Raccolta, evito di mettere il prodotto magazzino trattato nel dettaglio tecnico
                Continue For
            End If

            Dim prod As String = GetProductString(drDetTec, true)
            Dim princAtt As String = ""
            Dim avv As String = ""
            Dim fasifeno As String = ""

            'AVVERSITA'
            Dim listaAvv2 As New List(Of String)

            Dim filtroAvversita As String = " ID_Agenda=" & drDetTec.Item("Id_Agenda")
            If Not IsDBNull(drDetTec.Item("Id_Mov_det")) Then
                filtroAvversita = filtroAvversita & " AND Id_Mov_Det=" & drDetTec.Item("Id_Mov_det")
            End If

            Dim drMovDetTec2() As DataRow = dtMovDetTec.Select(filtroAvversita)

            For Each drAvv2 As DataRow In drMovDetTec2
                If drAvv2.Item("Av_des_vol") <> "" Then
                    listaAvv2.Add(drAvv2.Item("Av_des_vol"))
                End If
                If drAvv2.Item("Av_Gru_des") <> "" Then
                    listaAvv2.Add(drAvv2.Item("Av_Gru_des"))
                End If
            Next
            avv = String.Join(", ", listaAvv2)


            'PRINCIPI ATTIVI / SOSTANZE ATTIVE
            Dim codiciPrincAtt As String = "" 'cod1§titolo1|cod2§titolo2

            If drDetTec.Item("PrincipiAttivi") <> "" Then
                codiciPrincAtt = drDetTec.Item("PrincipiAttivi")
            Else
                If Not IsDBNull(drDetTec.Item("Pro_Cod")) AndAlso drDetTec.Item("Pro_Cod") <> 0 _
                    AndAlso Not IsNothing(HtProdPA(drDetTec.Item("Pro_Cod"))) Then
                    codiciPrincAtt = HtProdPA(drDetTec.Item("Pro_Cod"))
                End If
            End If

            Dim listaPrincAtt() As String = codiciPrincAtt.Split("|")
            Dim listaPrincAttNomi As New List(Of String)
            For Each pa As String In listaPrincAtt
                listaPrincAttNomi.Add(HtPrincAtt(pa.Split("§")(0))) 'estraggo il codice numerico e ricerco la stringa
            Next
            princAtt = String.Join(", ", listaPrincAttNomi)


            'FASI FENOLOGICHE
            Dim drMovDetTec2Fasi() As DataRow = dtMovDetTecFasi.Select(" ID_Agenda=" & DrAgenda(0).Item("Id_Agenda"))
            Dim listaFasi As New List(Of String)

            For Each drFasi As DataRow In drMovDetTec2Fasi
                Dim Fase_Des As String = ""
                If drFasi.Item("ff_classe") <> 0 Then

                    Select Case drFasi.Item("ff_classe")
                        Case < 1000 'caso vecchio av_cod = ff_cod
                            Fase_Des = (From aa In objParametriUscitaFasiOld.ListaFasiFenologiche
                                        Where aa.FF_Cod = drFasi.Item("ff_classe")
                                        Select aa.Descrizione
                                        ).FirstOrDefault

                        Case Else 'caso nuovo av_cod = cod_css
                            Fase_Des = (From aa In objParametriUscitaFasiNew.ListaFasiFenologiche
                                        Where aa.Cod_SS = drFasi.Item("ff_classe")
                                        Select aa.Descrizione & " - BBCH " & aa.Stadio
                                        ).FirstOrDefault

                    End Select
                    If Fase_Des <> "" Then
                        listaFasi.Add(Fase_Des)
                    End If

                End If

            Next

            fasifeno = String.Join(", ", listaFasi)

            'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
            'Dim testoDetTec As String = prod & If(princAtt <> "", " - " & princAtt, "") & If(avv <> "", " - " & avv, "")
            Dim testoDetTec As String = String.Join(" - ", {prod, princAtt, avv, fasifeno}.Where(Function(s) Not String.IsNullOrEmpty(s)))
            If testoDetTec <> "" AndAlso Not listaDetTec.Contains(testoDetTec) Then
                listaDetTec.Add(testoDetTec)
            End If

        Next

        Return String.Join(", ", listaDetTec)
    End Function

    Private Shared Sub getStrCostiMacchineOperatoriViaIDAgenda(ByRef strCosti_Operatori As String, ByRef strCosti_Macchine As String, DtCosti As DataTable, current_Agenda As String, ByVal AggiungiQTA As Boolean)
        '----------------------------------
        'Costi 
        Dim listaOperatori As New List(Of String)
        Dim listaMacchine As New List(Of String)

        If Not IsNothing(DtCosti) AndAlso DtCosti.Rows.Count > 0 Then

            Dim DrCosti() As DataRow = DtCosti.Select("Id_Agenda=" & current_Agenda)

            If Not IsNothing(DrCosti) Then
                For Each dr_costo As DataRow In DrCosti

                    'è un record manodopera
                    If dr_costo.Item("Cod_RisUm") <> 0 Then

                        'Recupero il nome del contatto
                        Dim nomeContatto As String = If(dr_costo.Item("Rag_Soc") <> "", dr_costo.Item("Rag_Soc"), String.Format("{0} {1}", dr_costo.Item("Cognome"), dr_costo.Item("Nome")))

                        If AggiungiQTA Then
                            If dr_costo("qta") <> 0 Then
                                nomeContatto &= " - " & dr_costo("qta") & " "
                                Select Case dr_costo("udm_cod")
                                    Case "-1"
                                        nomeContatto &= "Indefinito"
                                    Case "1"
                                        nomeContatto &= "Ettari"
                                    Case "2"
                                        nomeContatto &= "Ore"
                                End Select
                            End If

                        End If

                        If Not listaOperatori.Contains(nomeContatto) Then
                            listaOperatori.Add(nomeContatto)
                        End If

                    Else
                        'è un record macchinario

                        Dim detMacchina As String = dr_costo.Item("CLASS_DESC")
                        detMacchina &= If(dr_costo.Item("Modello") <> "", " - Modello " & dr_costo.Item("Modello"), "")
                        detMacchina &= If(dr_costo.Item("Ditta_Des") <> "", " - Marca " & dr_costo.Item("Ditta_Des"), "")
                        'detMacchina &= If(dr_costo.Item("Ultima_Manutenzione") <> "01/01/1900", " - Ultima Manutenzione " & dr_costo.Item("Ultima_Manutenzione"), "")


                        If AggiungiQTA Then

                            'al momento "AggiungiQta" vale anche come "aggiungi codice macchina"
                            detMacchina &= " (Codice: " & dr_costo("macchina_codice") & ") "
                            If dr_costo("qta") <> 0 Then
                                detMacchina &= " - " & dr_costo("qta") & " "
                                Select Case dr_costo("udm_cod")
                                    Case "-1"
                                        detMacchina &= "Indefinito"
                                    Case "1"
                                        detMacchina &= "Ettari"
                                    Case "2"
                                        detMacchina &= "Ore"
                                End Select
                            End If
                        End If


                        If Not listaMacchine.Contains(detMacchina) Then
                            listaMacchine.Add(detMacchina)
                        End If

                    End If

                Next
            End If
        End If

        strCosti_Operatori = String.Join(", ", listaOperatori)
        strCosti_Macchine = String.Join(", ", listaMacchine)
    End Sub
#End Region
End Class

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Movimenti_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '============================================================================
    Public Function Movimento_Scrivi(ByVal DatiMovimenti As String,
                                     ByVal Id_Agenda As Integer,
                                     ByVal Lav_Cod As Integer,
                                     ByVal Progressivo_Mirror As Integer,
                                     ByVal Flag_Mirror As Integer,
                                     ByVal Rimappa_Codici As Integer,
                                     ByVal Piva_SuperUser_Origine As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByVal Data_creazione As Date = AgroDataInizializzata,
                                     Optional ByVal Data_modifica As Date = AgroDataInizializzata,
                                     Optional ByVal username_creazione As String = "",
                                     Optional ByVal username_modifica As String = "",
                                     Optional ByRef CodiciRimappati As String = "",
                                     Optional ByVal Flag_Usa_Ora_Reale As Boolean = False,
                                     Optional ByVal G2G As Boolean = False
                                     ) As Boolean


        '----------------------------------------------------------------------

        Dim nomeRoutine As String = "AgronicaCoreContabBIZ.Movimenti_W.Movimento_Scrivi()"

        '----------------------------------------------------------------------


        If Data_creazione = AgroDataInizializzata Then
            Data_creazione = Date.Now
        End If

        If Data_modifica = AgroDataInizializzata Then
            Data_modifica = Date.Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If


        Dim dummy As Boolean

        Dim objSequenze As AgronicaCoreDataProvider.Agro_Sequenze
        Dim objMovimenti As AgronicaCoreContabDAL.Movimenti_W
        Dim objMovimentiDettagli As AgronicaCoreContabBIZ.Movimenti_Dettagli_W
        Dim objPagamenti As AgronicaCoreContabBIZ.Pagamento_W
        Dim objMovDettaglioTecnico As AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W
        Dim objMovDettaglioTecnicoExtra As AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_W
        Dim objMovimentixReport As AgronicaCoreContabDAL.MovimentixReport_W
        Dim objMovRiferimento As AgronicaCoreContabDAL.Mov_Det_Riferimenti_W
        Dim objMovimentoMirrorW As AgronicaCoreContabDAL.Movimenti_Mirror_W
        Dim objMovDettTecnicoMirrorW As AgronicaCoreContabDAL.Mov_DettTecnico_Mirror_W
        Dim objMovDettRiferimentoMirrorW As AgronicaCoreContabDAL.Mov_DettRiferim_Mirror_W
        Dim objComRisUmR As AgronicaCoreAnagrafeDAL.Risorse_Umane_R
        Dim objComMatPrR As AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim objComMatCamR As AgronicaCoreContabDAL.Materie_Prime_Campionature_R
        Dim objComMacR As AgronicaCoreContabDAL.Parco_Macchine_R



        Dim xmlDoc As XmlDocument

        Dim xDatiMovimenti As XmlNodeList
        Dim xDatiMovimento As XmlElement
        Dim xMovimenti As XmlNodeList
        Dim xMovimento As XmlElement
        Dim xMovRiferimenti As XmlNodeList
        Dim xMovRiferimento As XmlElement
        Dim xDatiMateriePC As XmlNodeList
        Dim xMateriaPC As XmlElement
        Dim xmlDatiMovimentiDettagli As XmlNodeList
        Dim xmlDatiMovimentoDettagli As XmlElement
        Dim xMovimentiDettagli As XmlNodeList
        Dim xMovimentoDettagli As XmlElement
        Dim xmlDatiPagamenti As XmlNodeList
        Dim xmlDatiPagamento As XmlElement
        Dim xMovDettagliTecnici As XmlNodeList
        Dim xMovDettaglioTecnico As XmlElement
        Dim xMovDettagliTecniciExtra As XmlNodeList
        Dim xMovDettaglioTecnicoExtra As XmlElement
        Dim xmlDatiMovimentoxReports As XmlNodeList
        Dim xmlDatiMovimentoxReport As XmlElement
        Dim xMovimentoxReports As XmlNodeList
        Dim xMovimentoxReport As XmlElement


        Dim objDtMat As DataTable

        Dim Id_Mov As Integer
        Dim Cod_Id_Reg_Dettaglio As Integer
        Dim intDummy As Integer
        Dim Cod_RisUm_Origine As Integer
        Dim Cod_RisUm As Integer

        Dim Qta_Acqua As String
        Dim Cau_Mov As String
        Dim Piva As String

        Dim bIPNO_DELETE As Boolean

        Dim i_DatiMovimenti As Integer
        Dim i_Movimenti As Integer
        Dim i_Movimenti_Dettagli As Integer
        Dim i_Pagamenti As Integer
        Dim i_Mov_Dettaglio_Tecnico As Integer
        Dim i_Mov_Dettaglio_Tecnico_Extra As Integer
        Dim i_DatiMovimentoxReports As Integer
        Dim i_MovimentoxReport As Integer
        Dim i_Mov_Riferimenti As Integer
        Dim i_Mov_Riferimento As Integer

        Dim OpeDB_Movimento As String
        Dim OpeDB_Mov_Dettaglio_Tecnico As String
        Dim OpeDB_Mov_Dettaglio_Tecnico_Extra As String
        Dim OpeDB_MovimentoxReport As String
        Dim OpeDB_Mov_Riferimento As String
        Dim DatiPagamenti As String
        Dim DatiMovimenti_Dettagli As String


        '------------------------------
        Dim flagTransazioneLocale As Boolean = False
        Dim flagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String = ""
        Dim xRisp As Boolean = True
        '------------------------------

        Dim Extra_Date As Date

        Dim vCodiciRimappati As String() = CodiciRimappati.Split("|")

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

            xmlDoc = New Xml.XmlDocument
            'xmlDoc.async = False
            xmlDoc.LoadXml(DatiMovimenti)

            '------------------------------
            '------------------------------
            '------------------------------



            xDatiMovimenti = xmlDoc.GetElementsByTagName("DatiMovimenti")

            i_DatiMovimenti = 0

            Do While i_DatiMovimenti < xDatiMovimenti.Count

                'Prelevo l'i-esimo blocco di DatiMovimenti (in realtà ne esiste uno solo)
                xDatiMovimento = xDatiMovimenti.Item(i_DatiMovimenti)


                '------------------------------

                xMovimenti = xDatiMovimento.GetElementsByTagName("Movimento")

                i_Movimenti = 0

                Do While i_Movimenti < xMovimenti.Count

                    'Prelevo l' i-esimo Movimento
                    xMovimento = xMovimenti.Item(i_Movimenti)

                    'Prelevo gli attributi del movimento selezionato
                    OpeDB_Movimento = xMovimento.GetAttribute("TipoOperazioneDB")

                    bIPNO_DELETE = False

                    objMovimenti = New AgronicaCoreContabDAL.Movimenti_W

                    'Inizializzo Preventivamente il Id_Mov
                    Id_Mov = CInt(xMovimento.GetAttribute("id_mov"))

                    '-------------------------------------------------------------------------------------------------------


                    If Rimappa_Codici = 1 Then

                        'Cod_RisUm magico, se compare nell'xml,
                        'significa che la rimappatura non è andata a buon fine
                        Cod_RisUm = -666

                        'Nelle operazioni contabili rimappo cod_risum
                        'Bolla emessa e ricevuta
                        'Fattura emessa e ricevuta
                        'Conf a diversi e conferimento
                        If (Lav_Cod = LAVCOD_BOLLA_EMESSA) Or (Lav_Cod = LAVCOD_BOLLA_RICEVUTA) Or
                           (Lav_Cod = LAVCOD_FATTURA_EMESSA) Or (Lav_Cod = LAVCOD_FATTURA_RICEVUTA) Or
                           (Lav_Cod = LAVCOD_CONFERIMENTO_DIVERSI) Or (Lav_Cod = LAVCOD_CONFERIMENTO) Then

                            Cod_RisUm_Origine = CInt(xMovimento.GetAttribute("cod_risum"))

                            objComRisUmR = New AgronicaCoreAnagrafeDAL.Risorse_Umane_R

                            objDtMat = objComRisUmR.LeggiSoloContatto(
                                                "",
                                                0,
                                                "",
                                                0,
                                                Cod_RisUm_Origine,
                                                Piva_SuperUser_Origine,
                                                True,
                                                CDate(xMovimento.GetAttribute("validita_inizio")),
                                                CDate(xMovimento.GetAttribute("validita_fine")),
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "",
                                                "",
                                                objParametri)


                            If objDtMat.Rows.Count > 0 Then

                                Cod_RisUm = CInt(objDtMat.Rows(0).Item("cod_risum"))

                            Else
                                'non dovrebbe verificarsi!
                                'verrà impostato a Cod_RisUm magico
                            End If

                            objComRisUmR = Nothing
                            objDtMat.Dispose()
                            objDtMat = Nothing

                            xMovimento.SetAttribute("cod_risum", CStr(Cod_RisUm))

                        End If 'FINE: If Lav_Cod contabili

                    End If 'FINE: If Rimappa_Codici = 1 Then



                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Movimento

                        Case "0"    'LEGGI -------------------------------------------------------
                            '
                        Case "1"    'SALVA -------------------------------------------------------


                            'NESSUNA OPERAZIONE NELLE TABELLE MIRROR

                            If Id_Mov <= 0 Then

                                'Richiedo un nuovo codice movimento

                                objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                                Id_Mov = objSequenze.NuovoId_Tabella("Movimenti",
                                                                     CInt(xMovimento.GetAttribute("basecode")),
                                                                     CInt(xMovimento.GetAttribute("topcode")),
                                                                     objParametri)

                                objSequenze = Nothing

                            Else

                                'Esportazione in Locale

                            End If

                            Agenda_W.ImpostaRimappaturaCodici(vCodiciRimappati, "Movimento",
                                    objParametri.PivaSuperUser & "," &
                                    CStr(xMovimento.GetAttribute("piva")) & "," &
                                    CStr(xMovimento.GetAttribute("sa_cod")) & "," &
                                    CStr(Id_Agenda) & "," &
                                    CStr(Id_Mov)
                            )



                            Extra_Date = Agro_XML_GetDate(xMovimento, "extra_date", AGRODATAINIZIO)

                            If Extra_Date.Year < 1900 Then

                                Extra_Date = AGRODATAINIZIO

                            End If


                            Dim xOra As Date
                            If xMovimento.HasAttribute("ora") = False Then
                                xOra = New Date
                            Else
                                If xMovimento.GetAttribute("ora").Length = 5 AndAlso
                                    (InStr(xMovimento.GetAttribute("ora"), "12:00") > 0 Or
                                    InStr(xMovimento.GetAttribute("ora"), "12.00") > 0) Then
                                    xOra = New Date
                                Else
                                    Try
                                        xOra = CDate(xMovimento.GetAttribute("ora"))
                                    Catch
                                        xOra = New Date
                                    End Try
                                End If
                            End If

                            Dim xOraFine As Date
                            If xMovimento.HasAttribute("oraFine") = False Then
                                xOraFine = New Date
                            Else
                                If xMovimento.GetAttribute("oraFine").Length = 5 AndAlso
                                    (InStr(xMovimento.GetAttribute("oraFine"), "12:00") > 0 Or
                                    InStr(xMovimento.GetAttribute("oraFine"), "12.00") > 0) Then
                                    xOraFine = New Date
                                Else
                                    Try
                                        xOraFine = CDate(xMovimento.GetAttribute("oraFine"))
                                    Catch
                                        xOraFine = New Date
                                    End Try
                                End If
                            End If

                            dummy = objMovimenti.Scrivi(
                                                CStr(xMovimento.GetAttribute("piva")),
                                                CInt(xMovimento.GetAttribute("sa_cod")),
                                                CInt(Id_Agenda),
                                                CInt(Id_Mov),
                                                CInt(xMovimento.GetAttribute("cod_risum")),
                                                CStr(xMovimento.GetAttribute("cau_mov")),
                                                CStr(xMovimento.GetAttribute("mov_desc")),
                                                CDate(xMovimento.GetAttribute("data_movimento")),
                                                CDate(xMovimento.GetAttribute("scadenza")),
                                                IIf(Not IsDate(xMovimento.GetAttribute("scadenza_extra")), AGRODATAINIZIO, xMovimento.GetAttribute("scadenza_extra")),
                                                CDec(xMovimento.GetAttribute("doc_numero")),
                                                CDec(xMovimento.GetAttribute("num_protocollo")),
                                                IIf(xMovimento.HasAttribute("cod_indirizzorisum") = False, 0, xMovimento.GetAttribute("cod_indirizzorisum")),
                                                IIf(xMovimento.HasAttribute("cod_destinazione") = False, 0, xMovimento.GetAttribute("cod_destinazione")),
                                                IIf(xMovimento.HasAttribute("cod_indirizzodestinazione") = False, 0, xMovimento.GetAttribute("cod_indirizzodestinazione")),
                                                IIf(xMovimento.HasAttribute("mezzo") = False, 0, xMovimento.GetAttribute("mezzo")),
                                                IIf(xMovimento.HasAttribute("cod_vettore") = False, 0, xMovimento.GetAttribute("cod_vettore")),
                                                IIf(xMovimento.HasAttribute("cod_indirizzovettore") = False, 0, xMovimento.GetAttribute("cod_indirizzovettore")),
                                                IIf(xMovimento.HasAttribute("causale_trasporto") = False, "", xMovimento.GetAttribute("causale_trasporto")),
                                                IIf(xMovimento.HasAttribute("aspetto") = False, "", xMovimento.GetAttribute("aspetto")),
                                                IIf(xMovimento.HasAttribute("peso") = False, 0, xMovimento.GetAttribute("peso")),
                                                xOra,
                                                IIf(xMovimento.HasAttribute("colli") = False, 0, xMovimento.GetAttribute("colli")),
                                                IIf(xMovimento.HasAttribute("tipo_sconto") = False, 0, xMovimento.GetAttribute("tipo_sconto")),
                                                IIf(xMovimento.HasAttribute("extra_str") = False, "", xMovimento.GetAttribute("extra_str")),
                                                IIf(xMovimento.HasAttribute("extra_int") = False, 0, xMovimento.GetAttribute("extra_int")),
                                                Extra_Date,
                                                Agro_SQL_SaveText(xMovimento.GetAttribute("doc_numero_sin"), False),
                                                Agro_SQL_SaveText(xMovimento.GetAttribute("doc_numero_des"), False),
                                                Agro_SQL_SaveText(xMovimento.GetAttribute("natura_beni"), False),
                                                IIf(xMovimento.HasAttribute("tara_veicolo") = False, 0, xMovimento.GetAttribute("tara_veicolo")),
                                                IIf(xMovimento.HasAttribute("tara_imballi") = False, 0, xMovimento.GetAttribute("tara_imballi")),
                                                IIf(xMovimento.HasAttribute("tipo_peso") = False, 0, xMovimento.GetAttribute("tipo_peso")),
                                                Agro_SQL_SaveNum(xMovimento.GetAttribute("modalita"), False),
                                                Agro_SQL_SaveText(xMovimento.GetAttribute("username_note"), False),
                                                IIf(xMovimento.HasAttribute("progr_protocollo") = False, 0, xMovimento.GetAttribute("progr_protocollo")),
                                                IIf(xMovimento.HasAttribute("progr_registrazione") = False, 0, xMovimento.GetAttribute("progr_registrazione")),
                                                IIf(xMovimento.HasAttribute("data_registrazione") = False, AGRODATAINIZIO, xMovimento.GetAttribute("data_registrazione")),
                                                Agro_SQL_SaveNum(xMovimento.GetAttribute("chklayout_bypass_fatturato"), False),
                                                Agro_SQL_SaveNum(xMovimento.GetAttribute("chklayout_join_prodotti"), False),
                                                CDate(xMovimento.GetAttribute("validita_inizio")),
                                                CDate(xMovimento.GetAttribute("validita_fine")),
                                                IIf(xMovimento.HasAttribute("disciplinare_pubblicoprivato") = False, 0, xMovimento.GetAttribute("disciplinare_pubblicoprivato")),
                                                objParametri,
                                                Data_creazione,
                                                Data_modifica,
                                                username_creazione,
                                                username_modifica,
                                                IIf(xMovimento.HasAttribute("sezionale_cod") = False, 0, xMovimento.GetAttribute("sezionale_cod")),
                                                IIf(xMovimento.HasAttribute("causale_trasporto_cod") = False, 0, xMovimento.GetAttribute("causale_trasporto_cod")),
                                                IIf(xMovimento.HasAttribute("cod_risum_altro") = False, 0, xMovimento.GetAttribute("cod_risum_altro")),
                                                IIf(xMovimento.HasAttribute("chklayout_peso") = False, 0, xMovimento.GetAttribute("chklayout_peso")),
                                                IIf(xMovimento.HasAttribute("chklayout_prezzo") = False, 0, xMovimento.GetAttribute("chklayout_prezzo")),
                                                IIf(xMovimento.HasAttribute("chkfiltro_varietale") = False, 0, xMovimento.GetAttribute("chkfiltro_varietale")),
                                                IIf(xMovimento.HasAttribute("chklayout_litri") = False, 0, xMovimento.GetAttribute("chklayout_litri")),
                                                Flag_Usa_Ora_Reale,
                                                IIf(xMovimento.HasAttribute("cod_risum_aggiuntivo") = False, 0, xMovimento.GetAttribute("cod_risum_aggiuntivo")),
                                                IIf(xMovimento.HasAttribute("cod_indirizzo_aggiuntivo") = False, 0, xMovimento.GetAttribute("cod_indirizzo_aggiuntivo")),
                                                IIf(xMovimento.HasAttribute("chklayout_riscontrato") = False, 0, xMovimento.GetAttribute("chklayout_riscontrato")),
                                                IIf(xMovimento.HasAttribute("doc_numero_visualizzato") = False, "", xMovimento.GetAttribute("doc_numero_visualizzato")),
                                                IIf(xMovimento.HasAttribute("cod_macchina_lav") = False, "", xMovimento.GetAttribute("cod_macchina_lav")),
                                                OraFine:=xOraFine
                                                )


                            '
                            '
                        Case "2"    'MODIFICA -------------------------------------------------------
                            '

                            'NON CONTEMPLATO

                            objMovimenti.Modifica(CStr(xMovimento.GetAttribute("piva")),
                                                CInt(xMovimento.GetAttribute("sa_cod")),
                                                CInt(xMovimento.GetAttribute("id_agenda")),
                                                CInt(Id_Mov),
                                                CInt(xMovimento.GetAttribute("cod_risum")),
                                                CStr(xMovimento.GetAttribute("cau_mov")),
                                                CStr(xMovimento.GetAttribute("mov_desc")),
                                                CStr(xMovimento.GetAttribute("data_movimento")),
                                                CDate(xMovimento.GetAttribute("scadenza")),
                                                IIf(Not IsDate(xMovimento.GetAttribute("scadenza_extra")), AGRODATAINIZIO, xMovimento.GetAttribute("scadenza_extra")),
                                                CDec(xMovimento.GetAttribute("doc_numero")),
                                                CDec(xMovimento.GetAttribute("num_protocollo")),
                                                IIf(xMovimento.HasAttribute("cod_indirizzorisum") = False, 0, xMovimento.GetAttribute("cod_indirizzorisum")),
                                                IIf(xMovimento.HasAttribute("cod_destinazione") = False, 0, xMovimento.GetAttribute("cod_destinazione")),
                                                IIf(xMovimento.HasAttribute("cod_indirizzodestinazione") = False, 0, xMovimento.GetAttribute("cod_indirizzodestinazione")),
                                                IIf(xMovimento.HasAttribute("mezzo") = False, 0, xMovimento.GetAttribute("mezzo")),
                                                IIf(xMovimento.HasAttribute("cod_vettore") = False, 0, xMovimento.GetAttribute("cod_vettore")),
                                                IIf(xMovimento.HasAttribute("cod_indirizzovettore") = False, 0, xMovimento.GetAttribute("cod_indirizzovettore")),
                                                IIf(xMovimento.HasAttribute("causale_trasporto") = False, "", xMovimento.GetAttribute("causale_trasporto")),
                                                IIf(xMovimento.HasAttribute("aspetto") = False, "", xMovimento.GetAttribute("aspetto")),
                                                IIf(xMovimento.HasAttribute("peso") = False, 0, xMovimento.GetAttribute("peso")),
                                                IIf(xMovimento.HasAttribute("ora") = False, 0, xMovimento.GetAttribute("ora")),
                                                IIf(xMovimento.HasAttribute("colli") = False, 0, xMovimento.GetAttribute("colli")),
                                                IIf(xMovimento.HasAttribute("tipo_sconto") = False, 0, xMovimento.GetAttribute("tipo_sconto")),
                                                IIf(xMovimento.HasAttribute("extra_str") = False, "", xMovimento.GetAttribute("extra_str")),
                                                IIf(xMovimento.HasAttribute("extra_int") = False, 0, xMovimento.GetAttribute("extra_int")),
                                                IIf(IsDate(xMovimento.GetAttribute("extra_date")), xMovimento.GetAttribute("extra_date"), New Date),
                                                Agro_SQL_SaveText(xMovimento.GetAttribute("doc_numero_sin"), False),
                                                Agro_SQL_SaveText(xMovimento.GetAttribute("doc_numero_des"), False),
                                                Agro_SQL_SaveText(xMovimento.GetAttribute("natura_beni"), False),
                                                IIf(xMovimento.HasAttribute("tara_veicolo") = False, 0, xMovimento.GetAttribute("tara_veicolo")),
                                                IIf(xMovimento.HasAttribute("tara_imballi") = False, 0, xMovimento.GetAttribute("tara_imballi")),
                                                IIf(xMovimento.HasAttribute("tipo_peso") = False, 0, xMovimento.GetAttribute("tipo_peso")),
                                                Agro_SQL_SaveNum(xMovimento.GetAttribute("modalita"), False),
                                                Agro_SQL_SaveText(xMovimento.GetAttribute("username_note"), False),
                                                IIf(xMovimento.HasAttribute("progr_protocollo") = False, 0, xMovimento.GetAttribute("progr_protocollo")),
                                                IIf(xMovimento.HasAttribute("progr_registrazione") = False, 0, xMovimento.GetAttribute("progr_registrazione")),
                                                IIf(xMovimento.HasAttribute("data_registrazione") = False, AGRODATAINIZIO, xMovimento.GetAttribute("data_registrazione")),
                                                Agro_SQL_SaveNum(xMovimento.GetAttribute("chklayout_bypass_fatturato"), False),
                                                Agro_SQL_SaveNum(xMovimento.GetAttribute("chklayout_join_prodotti"), False),
                                                CDate(xMovimento.GetAttribute("validita_inizio")),
                                                CDate(xMovimento.GetAttribute("validita_fine")),
                                                IIf(xMovimento.HasAttribute("disciplinare_pubblicoprivato") = False, 0, xMovimento.GetAttribute("disciplinare_pubblicoprivato")),
                                                "",
                                                objParametri)



                        Case "3" 'DELETE

                            '==============================================================================================================================================================================
                            'Marco 28/06/09: La 'Ipno Delete' consente la cancellazione globale di tutte le tabelle collegate a 'Movimenti'
                            'nel caso la causale del movimento non riguardi la gestione giacenze/consistenze/piano dei conti.
                            'Questa soluzione permette di ridurre i tempi di attesa in caso di modifica/cancellazione dei seguenti movimenti (elenco parziale):

                            'Cau_Mov =
                            '1. "2050" 'Scheda Trattamenti
                            '2. "2100" 'Scheda Rilievi in Campo
                            '3. "2200" 'Scheda Raccolta
                            '4. "2300" 'Scheda Lavorazioni

                            'Questi movimenti possono comprendere molti figli-conigli (soprattutto nella tabella 'Mov_Destinazioni').

                            Piva = xMovimento.GetAttribute("piva")
                            Cau_Mov = xMovimento.GetAttribute("cau_mov")

                            If Flag_Mirror = 0 Then

                                Select Case Cau_Mov

                                    Case "2050", "2100", "2200", "2300"

                                        'Controllo la Coerenza dei Parametri
                                        If Trim(Piva) <> "" And Id_Agenda <> 0 And Id_Mov <> 0 Then

                                            '##################################################################################################
                                            '##################################### IPNO DELETE  ###############################################
                                            '##################################################################################################

                                            objMovimenti.IPNO_Delete(Piva,
                                                                     Id_Agenda,
                                                                     Id_Mov,
                                                                     "",
                                                                     objParametri)

                                            bIPNO_DELETE = True

                                        End If

                                    Case Else

                                        'Causale Non Gestibile da Ipno Delete

                                End Select

                            Else

                                'Mirroring --> Ipno Delete Non Praticabile

                            End If
                            '==============================================================================================================================================================================


                    End Select

                    '===========================================================================================================================
                    If Not bIPNO_DELETE Then
                        '---------------------------------------------------------------------------------------------------------------------------

                        '##################################################
                        '##########  MOVIMENTI DETTAGLI TECNICI  ##########
                        '##################################################

                        'Prelevo l'elenco dei dettagli tecnici del movimento
                        xMovDettagliTecnici = xMovimento.GetElementsByTagName("Movimento_Dettaglio_Tecnico")

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

                                        objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                                        Cod_Id_Reg_Dettaglio = objSequenze.NuovoId_Tabella(
                                                "Movimenti_Dettagli_Tecnici",
                                                CInt(xMovDettaglioTecnico.GetAttribute("basecode")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("topcode")),
                                                objParametri)


                                        objSequenze = Nothing

                                    Else

                                        'Esportazione in Locale

                                    End If

                                    'Salvo il dettaglio tecnico
                                    Qta_Acqua = xMovDettaglioTecnico.GetAttribute("qta_ril")

                                    If InStr(Qta_Acqua, ".") <> 0 Then
                                        Qta_Acqua = Split(Qta_Acqua, ".")(0) & "," & Split(Qta_Acqua, ".")(1)
                                    End If

                                    dummy = objMovDettaglioTecnico.Scrivi(
                                                CStr(xMovimento.GetAttribute("piva")),
                                                CInt(xMovimento.GetAttribute("sa_cod")),
                                                CInt(Id_Agenda),
                                                CInt(Id_Mov),
                                                0,
                                                CInt(Cod_Id_Reg_Dettaglio),
                                                CInt(xMovDettaglioTecnico.GetAttribute("av_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("av_gru")),
                                                CStr(xMovDettaglioTecnico.GetAttribute("sigla_av")),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("data_ril")), xMovDettaglioTecnico.GetAttribute("data_ril"), New Date),
                                                Qta_Acqua,
                                                CDec(xMovDettaglioTecnico.GetAttribute("dose")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("ditta_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("dett_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("id_insetto")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("ff_classe")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("mg")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("n")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("k")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("p")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("parziale")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("nitrati")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("freatimetro")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("piezo1")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("piezo2")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("piezo3")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("piezo4")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("trap_num")),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn1_data")), xMovDettaglioTecnico.GetAttribute("inn1_data"), New Date), IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn2_data")), xMovDettaglioTecnico.GetAttribute("inn2_data"), New Date),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn3_data")), xMovDettaglioTecnico.GetAttribute("inn3_data"), New Date), IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn4_data")), xMovDettaglioTecnico.GetAttribute("inn4_data"), New Date),
                                                IIf(xMovDettaglioTecnico.HasAttribute("lotto") = False, "", xMovDettaglioTecnico.GetAttribute("lotto")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("extra_int") = False, 0, xMovDettaglioTecnico.GetAttribute("extra_int")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("extra_str") = False, "", xMovDettaglioTecnico.GetAttribute("extra_str")),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("extra_date")), xMovDettaglioTecnico.GetAttribute("extra_date"), New Date),
                                                IIf(xMovDettaglioTecnico.HasAttribute("soglia_cod") = False, 0, xMovDettaglioTecnico.GetAttribute("soglia_cod")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("soglia_quantita") = False, 0, xMovDettaglioTecnico.GetAttribute("soglia_quantita")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("soglia_des") = False, "", xMovDettaglioTecnico.HasAttribute("soglia_des")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("efficienza") = False, 0, xMovDettaglioTecnico.GetAttribute("efficienza")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("cu") = False, 0, xMovDettaglioTecnico.GetAttribute("cu")),
                                                CDate(xMovDettaglioTecnico.GetAttribute("validita_inizio")),
                                                CDate(xMovDettaglioTecnico.GetAttribute("validita_fine")),
                                                objParametri,
                                                Data_creazione,
                                                Data_modifica,
                                                username_creazione,
                                                username_modifica)


                                Case "2"    'MODIFICA -------------------------------------------------------

                                    Qta_Acqua = xMovDettaglioTecnico.GetAttribute("qta_ril")

                                    If InStr(Qta_Acqua, ".") <> 0 Then
                                        Qta_Acqua = Split(Qta_Acqua, ".")(0) & "," & Split(Qta_Acqua, ".")(1)
                                    End If

                                    objMovDettaglioTecnico.Modifica(
                                                CStr(xMovimento.GetAttribute("piva")),
                                                CInt(xMovimento.GetAttribute("sa_cod")),
                                                CInt(xMovimento.GetAttribute("id_agenda")),
                                                CInt(Id_Mov),
                                                0,
                                                CInt(Cod_Id_Reg_Dettaglio),
                                                CInt(xMovDettaglioTecnico.GetAttribute("av_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("av_gru")),
                                                CStr(xMovDettaglioTecnico.GetAttribute("sigla_av")),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("data_ril")), xMovDettaglioTecnico.GetAttribute("data_ril"), New Date),
                                                Qta_Acqua,
                                                CDec(xMovDettaglioTecnico.GetAttribute("dose")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("ditta_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("dett_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("id_insetto")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("ff_classe")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("mg")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("n")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("k")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("p")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("parziale")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("nitrati")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("freatimetro")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("piezo1")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("piezo2")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("piezo3")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("piezo4")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("trap_num")),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn1_data")), xMovDettaglioTecnico.GetAttribute("inn1_data"), New Date),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn2_data")), xMovDettaglioTecnico.GetAttribute("inn2_data"), New Date),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn3_data")), xMovDettaglioTecnico.GetAttribute("inn3_data"), New Date),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn4_data")), xMovDettaglioTecnico.GetAttribute("inn4_data"), New Date),
                                                IIf(xMovDettaglioTecnico.HasAttribute("lotto") = False, "", xMovDettaglioTecnico.GetAttribute("lotto")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("extra_int") = False, 0, xMovDettaglioTecnico.GetAttribute("extra_int")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("extra_str") = False, "", xMovDettaglioTecnico.GetAttribute("extra_str")),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("extra_date")), xMovDettaglioTecnico.GetAttribute("extra_date"), New Date),
                                                IIf(xMovDettaglioTecnico.HasAttribute("soglia_cod") = False, 0, xMovDettaglioTecnico.GetAttribute("soglia_cod")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("soglia_quantita") = False, 0, xMovDettaglioTecnico.GetAttribute("soglia_quantita")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("soglia_des") = False, "", xMovDettaglioTecnico.HasAttribute("soglia_des")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("efficienza") = False, 0, xMovDettaglioTecnico.GetAttribute("efficienza")),
                                                IIf(xMovDettaglioTecnico.HasAttribute("cu") = False, 0, xMovDettaglioTecnico.GetAttribute("cu")),
                                                CDate(xMovDettaglioTecnico.GetAttribute("validita_inizio")),
                                                CDate(xMovDettaglioTecnico.GetAttribute("validita_fine")),
                                                "",
                                                objParametri)




                                Case "3"    'ELIMINA -------------------------------------------------------

                                    '@MIRROR@

                                    If Flag_Mirror = 1 Then


                                        '************************************************
                                        '************************************************
                                        '*********** INIZIO MIRRORING *******************
                                        '************************************************


                                        'IN CASO DI CANCELLAZIONE, VA INSERITA L'OPERAZIONE NELLA TABELLA MIRROR
                                        'E POI CANCELLATA NELLA TABELLA BUONA


                                        'MOV_DETTTECNICO_MIRROR_W

                                        'Salvo il dettaglio tecnico
                                        Qta_Acqua = xMovDettaglioTecnico.GetAttribute("qta_ril")
                                        If InStr(Qta_Acqua, ".") <> 0 Then
                                            Qta_Acqua = Split(Qta_Acqua, ".")(0) & "," & Split(Qta_Acqua, ".")(1)
                                        End If

                                        objMovDettTecnicoMirrorW = New AgronicaCoreContabDAL.Mov_DettTecnico_Mirror_W

                                        dummy = objMovDettTecnicoMirrorW.Scrivi(
                                                CStr(xMovimento.GetAttribute("piva")),
                                                CInt(xMovimento.GetAttribute("sa_cod")),
                                                CInt(Id_Agenda),
                                                CInt(Id_Mov),
                                                CInt(0),
                                                CInt(Cod_Id_Reg_Dettaglio),
                                                CInt(Progressivo_Mirror),
                                                CInt(xMovDettaglioTecnico.GetAttribute("av_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("av_gru")),
                                                CStr(xMovDettaglioTecnico.GetAttribute("sigla_av")),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("data_ril")), xMovDettaglioTecnico.GetAttribute("data_ril"), New Date),
                                                CDec(Qta_Acqua),
                                                CDec(xMovDettaglioTecnico.GetAttribute("dose")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("ditta_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("dett_cod")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("id_insetto")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("ff_classe")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("mg")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("n")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("k")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("p")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("parziale")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("nitrati")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("freatimetro")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("piezo1")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("piezo2")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("piezo3")),
                                                CDec(xMovDettaglioTecnico.GetAttribute("piezo4")),
                                                CInt(xMovDettaglioTecnico.GetAttribute("trap_num")),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn1_data")), xMovDettaglioTecnico.GetAttribute("inn1_data"), New Date),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn2_data")), xMovDettaglioTecnico.GetAttribute("inn2_data"), New Date),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn3_data")), xMovDettaglioTecnico.GetAttribute("inn3_data"), New Date),
                                                IIf(IsDate(xMovDettaglioTecnico.GetAttribute("inn4_data")), xMovDettaglioTecnico.GetAttribute("inn4_data"), New Date),
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

                                    objMovDettaglioTecnico.Cancella(CStr(xMovimento.GetAttribute("piva")),
                                                                    CInt(xMovimento.GetAttribute("sa_cod")),
                                                                    CInt(xMovimento.GetAttribute("id_agenda")),
                                                                    CInt(Id_Mov),
                                                                    0,
                                                                    CInt(Cod_Id_Reg_Dettaglio),
                                                                    "",
                                                                    objParametri)


                            End Select

                            'Elimino l'oggetto
                            objMovDettaglioTecnico = Nothing
                            'Incremento l'indice
                            i_Mov_Dettaglio_Tecnico += 1

                        Loop

                        '##################################################
                        '##########  MOVIMENTI DETTAGLI TECNICI EXTRA #####
                        '##################################################

                        'Prelevo l'elenco dei dettagli tecnici EXTRA del movimento
                        xMovDettagliTecniciExtra = xMovimento.GetElementsByTagName("Movimento_Dettaglio_Tecnico_Extra")

                        i_Mov_Dettaglio_Tecnico_Extra = 0

                        Do While i_Mov_Dettaglio_Tecnico_Extra < xMovDettagliTecniciExtra.Count

                            'Prelevo l'i-esimo dettaglio tecnico extra
                            xMovDettaglioTecnicoExtra = xMovDettagliTecniciExtra.Item(i_Mov_Dettaglio_Tecnico_Extra)

                            'Prelevo gli attributi del dettaglio tecnico extra selezionato
                            OpeDB_Mov_Dettaglio_Tecnico_Extra = xMovDettaglioTecnicoExtra.GetAttribute("TipoOperazioneDB")

                            'Inizializzo Preventivamente il Codice Dettaglio Tecnico
                            Cod_Id_Reg_Dettaglio = CInt(xMovDettaglioTecnicoExtra.GetAttribute("id_reg_dettaglio"))

                            objMovDettaglioTecnicoExtra = New AgronicaCoreContabDAL.Mov_Dett_Tecnico_Ex_W

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Mov_Dettaglio_Tecnico_Extra

                                Case "0"    'LEGGI -------------------------------------------------------
                                    '
                                Case "1"    'SALVA -------------------------------------------------------


                                    If Cod_Id_Reg_Dettaglio <= 0 Then

                                        'Richiedo un nuovo codice dettaglio tecnico extra
                                        objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                                        Cod_Id_Reg_Dettaglio = objSequenze.NuovoId_Tabella(
                                                "Movimenti_Dettagli_Tecnici_Extra",
                                                CInt(xMovDettaglioTecnicoExtra.GetAttribute("basecode")),
                                                CInt(xMovDettaglioTecnicoExtra.GetAttribute("topcode")),
                                                objParametri)


                                        objSequenze = Nothing

                                    Else

                                        'Esportazione in Locale

                                    End If


                                    dummy = objMovDettaglioTecnicoExtra.ScriviFull(
                                                CStr(xMovimento.GetAttribute("piva")),
                                                CInt(xMovimento.GetAttribute("sa_cod")),
                                                CInt(Id_Agenda),
                                                CInt(Id_Mov),
                                                0,
                                                CInt(Cod_Id_Reg_Dettaglio),
                                                CStr(xMovDettaglioTecnicoExtra.GetAttribute("regione")),
                                                CStr(xMovDettaglioTecnicoExtra.GetAttribute("asl")),
                                                CStr(xMovDettaglioTecnicoExtra.GetAttribute("serie")),
                                                CStr(xMovDettaglioTecnicoExtra.GetAttribute("numero")),
                                                CInt(xMovDettaglioTecnicoExtra.GetAttribute("mac_cod")),
                                                CInt(xMovDettaglioTecnicoExtra.GetAttribute("cod_risum")),
                                                CStr(xMovDettaglioTecnicoExtra.GetAttribute("trasportatore")),
                                                CStr(xMovDettaglioTecnicoExtra.GetAttribute("mezzo_trasporto")),
                                                CStr(xMovDettaglioTecnicoExtra.GetAttribute("targa")),
                                                CStr(xMovDettaglioTecnicoExtra.GetAttribute("n_immatricolazione")),
                                                CStr(xMovDettaglioTecnicoExtra.GetAttribute("n_immatricolazione_rimorchio")),
                                                CStr(xMovDettaglioTecnicoExtra.GetAttribute("n_autorizzazione_trasporto")),
                                                CDate(xMovDettaglioTecnicoExtra.GetAttribute("data_rilascio_autorizzazione")),
                                                CDec(xMovDettaglioTecnicoExtra.GetAttribute("peso")),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "codice_prodotto", 0),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "colore", 0),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "zona_viticola", ""),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "manipolazioni", 0),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "precisazioni", ""),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "annotazioni", ""),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "num_contenitori", 0),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "marche_contenitori", ""),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "des_contenitori", ""),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "tipo_documento", ""),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "id_cod_autorita", 0),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "luogo_partenza", ""),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "luogo_consegna", ""),
                                                Agro_XML_GetDate(xMovDettaglioTecnicoExtra, "data_spedizione", AGRODATAINIZIO),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "indicazioni_complementari", ""),
                                                Agro_XML_GetDecimal(xMovDettaglioTecnicoExtra, "titolo_alcol", 0),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "codice_nc", ""),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "num_riferimento", ""),
                                                Agro_XML_GetDate(xMovDettaglioTecnicoExtra, "data_dichiarazione", AGRODATAINIZIO),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "garanzia", ""),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "certificati", ""),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "durata_viaggio", ""),
                                                Agro_XML_GetDecimal(xMovDettaglioTecnicoExtra, "peso_lordo", 0),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "num_colli", 0),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "contenitore_cod", 0),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "imballaggio_cod", 0),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "agente_cod", 0),
                                                Agro_XML_GetDecimal(xMovDettaglioTecnicoExtra, "provvigione", 0),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "tipo_trasporto", 0),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "unita_trasporto", 0),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "codice_alternativo", ""),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "id_gestione_vettore", 0),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "ritenuta_acconto_cod", 0),
                                                Agro_XML_GetDecimal(xMovDettaglioTecnicoExtra, "ritenuta_acconto", 0),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "enasarco_cod", 0),
                                                Agro_XML_GetDecimal(xMovDettaglioTecnicoExtra, "enasarco", 0),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "accdaa_cod_risum_destinatario", 0),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "accdaa_cod_risum_destinazione", 0),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "accdaa_cod_indirizzorisum_destinatario", 0),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "accdaa_cod_indirizzorisum_destinazione", 0),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "capoarea_cod", 0),
                                                Agro_XML_GetDecimal(xMovDettaglioTecnicoExtra, "provvigione_capoarea", 0),
                                                Agro_XML_GetDecimal(xMovDettaglioTecnicoExtra, "provvigione_pagata_agente", 0),
                                                Agro_XML_GetDecimal(xMovDettaglioTecnicoExtra, "provvigione_pagata_capoarea", 0),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "n_doc_cliente", ""),
                                                Agro_XML_GetDate(xMovDettaglioTecnicoExtra, "data_doc_cliente", AGRODATAINIZIO),
                                                Agro_XML_GetString(xMovDettaglioTecnicoExtra, "n_doc_ente", ""),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "anno_doc_ente", Year(AGRODATAFINE)),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "num_conf_riscontrate", -1),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "num_colli_riscontrati", -1),
                                                Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "num_imballi_riscontrati", -1),
                                                Agro_XML_GetDecimal(xMovDettaglioTecnicoExtra, "peso_netto_riscontrato", 0),
                                                Agro_XML_GetDecimal(xMovDettaglioTecnicoExtra, "peso_lordo_riscontrato", 0),
                                                Agro_XML_GetDate(xMovDettaglioTecnicoExtra, "validita_inizio", AGRODATAINIZIO),
                                                Agro_XML_GetDate(xMovDettaglioTecnicoExtra, "validita_fine", AGRODATAFINE),
                                                objParametri,
                                                Data_creazione,
                                                Data_modifica,
                                                username_creazione,
                                                username_modifica,
                                                Tara_Unit_Conf_Riscontrata:=Agro_XML_GetDecimal(xMovDettaglioTecnicoExtra, "tara_unit_conf_riscontrata", -1),
                                                Tara_Unit_Collo_Riscontrata:=Agro_XML_GetDecimal(xMovDettaglioTecnicoExtra, "tara_unit_collo_riscontrata", -1),
                                                Tara_Unit_Imballo_Riscontrata:=Agro_XML_GetDecimal(xMovDettaglioTecnicoExtra, "tara_unit_imballo_riscontrata", -1),
                                                N_Nota_Fattura:=Agro_XML_GetString(xMovDettaglioTecnicoExtra, "n_nota_fattura", ""),
                                                Data_Nota_Fattura:=Agro_XML_GetDate(xMovDettaglioTecnicoExtra, "data_nota_fattura", AGRODATAINIZIO),
                                                N_Nota_DDT:=Agro_XML_GetString(xMovDettaglioTecnicoExtra, "n_nota_ddt", ""),
                                                N_Nota_Riga_DDT:=Agro_XML_GetString(xMovDettaglioTecnicoExtra, "n_nota_riga_ddt", ""),
                                                Data_Nota_DDT:=Agro_XML_GetDate(xMovDettaglioTecnicoExtra, "data_nota_ddt", AGRODATAINIZIO),
                                                Causale_Fattura:=Agro_XML_GetInteger(xMovDettaglioTecnicoExtra, "causale_fattura", 0)
                                                )

                                    'validita inizio e fine vengono impostati da objParametri.FinestraTemporaleInizio e objParametri.FinestraTemporaleFine
                                    'CDate(xMovDettaglioTecnicoExtra.GetAttribute("validita_inizio")),
                                    'CDate(xMovDettaglioTecnicoExtra.GetAttribute("validita_fine")),


                                Case "2"    'MODIFICA -------------------------------------------------------

                                    '
                                Case "3"    'ELIMINA -------------------------------------------------------

                                    objMovDettaglioTecnicoExtra.Cancella(CStr(xMovimento.GetAttribute("piva")),
                                                                         CInt(xMovimento.GetAttribute("sa_cod")),
                                                                         CInt(xMovimento.GetAttribute("id_agenda")),
                                                                         CInt(Id_Mov),
                                                                         0,
                                                                         CInt(Cod_Id_Reg_Dettaglio),
                                                                         "",
                                                                         objParametri)



                            End Select

                            'Elimino l'oggetto
                            objMovDettaglioTecnicoExtra = Nothing
                            'Incremento l'indice
                            i_Mov_Dettaglio_Tecnico_Extra += 1

                        Loop

                        '##################################################
                        '#############  MOVIMENTI X REPORT  ###############
                        '##################################################

                        xmlDatiMovimentoxReports = xMovimento.GetElementsByTagName("DatiMovimentixReport")

                        i_DatiMovimentoxReports = 0

                        Do While i_DatiMovimentoxReports < xmlDatiMovimentoxReports.Count

                            'Prelevo l'i-esimo blocco di DatiMovimentoxReport (in realtà ne esiste uno solo)
                            xmlDatiMovimentoxReport = xmlDatiMovimentoxReports.Item(i_DatiMovimentoxReports)

                            '------------------------------

                            xMovimentoxReports = xmlDatiMovimentoxReport.GetElementsByTagName("MovimentoxReport")

                            i_MovimentoxReport = 0

                            Do While i_MovimentoxReport < xMovimentoxReports.Count

                                'Prelevo l' i-esimo MovimentoxReport
                                xMovimentoxReport = xMovimentoxReports.Item(i_Movimenti)

                                'Prelevo gli attributi del dettaglio tecnico extra selezionato
                                OpeDB_MovimentoxReport = xMovimentoxReport.GetAttribute("TipoOperazioneDB")

                                objMovimentixReport = New AgronicaCoreContabDAL.MovimentixReport_W

                                'Verifico l'operazione richiesta
                                Select Case OpeDB_MovimentoxReport

                                    Case "0"    'LEGGI -------------------------------------------------------
                                        '
                                    Case "1"    'SALVA -------------------------------------------------------


                                        dummy = objMovimentixReport.Scrivi(
                                                CStr(xMovimento.GetAttribute("piva")),
                                                CInt(xMovimento.GetAttribute("sa_cod")),
                                                CInt(Id_Agenda),
                                                CInt(Id_Mov),
                                                0,
                                                CInt(xMovimentoxReport.GetAttribute("id_report")),
                                                CStr(xMovimentoxReport.GetAttribute("descrizione")),
                                                CDate(xMovimentoxReport.GetAttribute("validita_inizio")),
                                                CDate(xMovimentoxReport.GetAttribute("validita_fine")),
                                                objParametri,
                                                Data_creazione,
                                                Data_modifica,
                                                username_creazione,
                                                username_modifica)



                                    Case "2"    'MODIFICA -------------------------------------------------------

                                        '
                                    Case "3"    'ELIMINA -------------------------------------------------------


                                        objMovimentixReport.Cancella(CStr(xMovimento.GetAttribute("piva")),
                                                                     CInt(xMovimento.GetAttribute("sa_cod")),
                                                                     CInt(xMovimento.GetAttribute("id_agenda")),
                                                                     CInt(Id_Mov),
                                                                     0,
                                                                     CInt(xMovimentoxReport.GetAttribute("id_report")),
                                                                     "",
                                                                     objParametri)



                                End Select

                                'Elimino l'oggetto
                                objMovimentixReport = Nothing
                                'Incremento l'indice
                                i_MovimentoxReport += 1

                            Loop

                            i_DatiMovimentoxReports += 1

                        Loop


                        '#############################################
                        '##########  MOVIMENTI RIFERIMENTI  ##########
                        '#############################################

                        Dim tempPiva As String
                        Dim tempSaCod As String
                        Dim tempIdAgenda As String

                        'Prelevo l'elenco dei riferimenti del movimento
                        xMovRiferimenti = xMovimento.GetElementsByTagName("Movimento_Riferimento")

                        i_Mov_Riferimento = 0

                        Do While i_Mov_Riferimento < xMovRiferimenti.Count

                            'Prelevo l'i-esimo riferimento del movimento riferimento
                            xMovRiferimento = xMovRiferimenti.Item(i_Mov_Riferimento)

                            'Prelevo gli attributi del riferimento selezionato
                            OpeDB_Mov_Riferimento = xMovRiferimento.GetAttribute("TipoOperazioneDB")

                            objMovRiferimento = New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W

                            'Verifico l'operazione richiesta
                            Select Case OpeDB_Mov_Riferimento

                                Case "0"    'LEGGI -------------------------------------------------------
                                    '
                                Case "1"    'SALVA -------------------------------------------------------
                                    '
                                    '

                                    'DEBUG //////////////////
                                    tempPiva = xMovRiferimento.GetAttribute("piva")
                                    tempSaCod = xMovRiferimento.GetAttribute("sa_cod")
                                    tempIdAgenda = xMovRiferimento.GetAttribute("id_agenda")
                                    tempPiva = xMovimento.GetAttribute("piva")
                                    tempSaCod = xMovimento.GetAttribute("sa_cod")
                                    tempIdAgenda = xMovimento.GetAttribute("id_agenda")
                                    tempPiva = Piva
                                    tempSaCod = ""
                                    tempIdAgenda = Id_Agenda
                                    'DEBUG //////////////////



                                    '****** Modifica Mauro 24/07/2008 ****************
                                    ' Passo piva, sa_cod e id_agenda di xMov_Riferimento invece di quelli di xMovimento
                                    ' perché altrimenti può generare errori di chiave duplicata!
                                    '*******************************************************

                                    If CInt(xMovRiferimento.GetAttribute("id_agenda")) <> 0 And
                                       CStr(xMovRiferimento.GetAttribute("piva")) <> "" Then

                                        '
                                        'Controllo che il record non sia già presente

                                        Dim objComRif As AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                                        Dim objDtRif As DataTable
                                        Dim presente As Boolean = True

                                        objComRif = New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R

                                        objDtRif = objComRif.LeggixChiave(
                                                CStr(xMovRiferimento.GetAttribute("piva")),
                                                CInt(xMovRiferimento.GetAttribute("sa_cod")),
                                                CInt(xMovRiferimento.GetAttribute("id_agenda")),
                                                CInt(xMovRiferimento.GetAttribute("id_mov")),
                                                CInt(xMovRiferimento.GetAttribute("id_mov_det")),
                                                CInt(xMovRiferimento.GetAttribute("id_agenda_rif")),
                                                CInt(xMovRiferimento.GetAttribute("id_mov_rif")),
                                                CInt(xMovRiferimento.GetAttribute("id_mov_det_rif")),
                                                0,
                                                0,
                                                "",
                                                objParametri)

                                        If Not IsNothing(objDtRif) Then

                                            If objDtRif.Rows.Count > 0 Then
                                                presente = True
                                            Else
                                                presente = False
                                            End If 'FINE: If objRsRif.State <> 0 Then

                                        End If 'FINE: If Not objRsRif Is Nothing Then

                                        objDtRif.Dispose()
                                        objDtRif = Nothing
                                        objComRif = Nothing

                                        If Not presente Then

                                            dummy = objMovRiferimento.Scrivi(
                                                CStr(xMovRiferimento.GetAttribute("piva")),
                                                CInt(xMovRiferimento.GetAttribute("sa_cod")),
                                                CInt(xMovRiferimento.GetAttribute("id_agenda")),
                                                -1,
                                                -1,
                                                Agro_SQL_SaveNum(xMovRiferimento.GetAttribute("lav_cod"), False),
                                                Agro_SQL_SaveText(xMovRiferimento.GetAttribute("cau_mov"), False),
                                                Agro_SQL_SaveText(xMovRiferimento.GetAttribute("piva_rif"), False),
                                                Agro_SQL_SaveNum(xMovRiferimento.GetAttribute("sa_cod_rif"), False),
                                                CInt(xMovRiferimento.GetAttribute("id_agenda_rif")),
                                                -1,
                                                -1,
                                                Agro_SQL_SaveNum(xMovRiferimento.GetAttribute("lav_cod_rif"), False),
                                                CStr(xMovRiferimento.GetAttribute("cau_mov_rif")),
                                                IIf(xMovRiferimento.HasAttribute("qta") = False, 0, xMovRiferimento.GetAttribute("qta")),
                                                CDate(xMovRiferimento.GetAttribute("validita_inizio")),
                                                CDate(xMovRiferimento.GetAttribute("validita_fine")),
                                                objParametri,
                                                Data_creazione,
                                                Data_modifica,
                                                username_creazione,
                                                username_modifica)



                                        End If

                                    Else 'If cint(xMov_Riferimento.getAttribute("id_agenda")) = 0 And CStr(xMov_Riferimento.getAttribute("piva")) <> "" Then

                                        dummy = objMovRiferimento.Scrivi(
                                                CStr(xMovimento.GetAttribute("piva")),
                                                CInt(xMovimento.GetAttribute("sa_cod")),
                                                CInt(Id_Agenda),
                                                -1,
                                                -1,
                                                Agro_SQL_SaveNum(xMovRiferimento.GetAttribute("lav_cod"), False),
                                                Agro_SQL_SaveText(xMovRiferimento.GetAttribute("cau_mov"), False),
                                                Agro_SQL_SaveText(xMovRiferimento.GetAttribute("piva_rif"), False),
                                                Agro_SQL_SaveNum(xMovRiferimento.GetAttribute("sa_cod_rif"), False),
                                                CInt(xMovRiferimento.GetAttribute("id_agenda_rif")),
                                                -1,
                                                -1,
                                                Agro_SQL_SaveNum(xMovRiferimento.GetAttribute("lav_cod_rif"), False),
                                                CStr(xMovRiferimento.GetAttribute("cau_mov_rif")),
                                                IIf(xMovRiferimento.HasAttribute("qta") = False, 0, xMovRiferimento.GetAttribute("qta")),
                                                CDate(xMovRiferimento.GetAttribute("validita_inizio")),
                                                CDate(xMovRiferimento.GetAttribute("validita_fine")),
                                                objParametri,
                                                Data_creazione,
                                                Data_modifica,
                                                username_creazione,
                                                username_modifica)



                                    End If 'FINE: If cint(xMov_Riferimento.getAttribute("id_agenda")) = 0 And CStr(xMov_Riferimento.getAttribute("piva")) <> "" Then


                                    '
                                Case "2"    'MODIFICA -------------------------------------------------------
                                    '
                                    'Non Gestita

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


                                        'MOV_DETTRIFERIM_MIRROR_W

                                        objMovDettRiferimentoMirrorW = New AgronicaCoreContabDAL.Mov_DettRiferim_Mirror_W

                                        dummy = objMovDettRiferimentoMirrorW.Scrivi(
                                                CStr(xMovimento.GetAttribute("piva")),
                                                CInt(xMovimento.GetAttribute("sa_cod")),
                                                CInt(Id_Agenda),
                                                -1,
                                                -1,
                                                Agro_SQL_SaveNum(xMovRiferimento.GetAttribute("lav_cod"), False),
                                                Agro_SQL_SaveText(xMovRiferimento.GetAttribute("cau_mov"), False),
                                                Agro_SQL_SaveText(xMovRiferimento.GetAttribute("piva_rif"), False),
                                                Agro_SQL_SaveNum(xMovRiferimento.GetAttribute("sa_cod_rif"), False),
                                                CInt(xMovRiferimento.GetAttribute("id_agenda_rif")),
                                                -1,
                                                -1,
                                                Progressivo_Mirror,
                                                Agro_SQL_SaveNum(xMovRiferimento.GetAttribute("lav_cod_rif"), False),
                                                CStr(xMovRiferimento.GetAttribute("cau_mov_rif")),
                                                IIf(xMovRiferimento.HasAttribute("qta") = False, 0, xMovRiferimento.GetAttribute("qta")),
                                                objParametri)



                                        objMovDettRiferimentoMirrorW = Nothing


                                        '************************************************
                                        '*********** FINE MIRRORING *********************
                                        '************************************************
                                        '************************************************

                                    End If 'Flag_Mirror


                                    objMovRiferimento.Cancella(
                                                "",
                                                0,
                                                CInt(xMovimento.GetAttribute("id_agenda")),
                                                -1,
                                                -1,
                                                "",
                                                objParametri)

                            End Select


                            'Elimino l'oggetto
                            objMovRiferimento = Nothing

                            'Incremento l'indice
                            i_Mov_Riferimento += 1

                        Loop




                        '#############################################
                        '###############  PAGAMENTI  #################
                        '#############################################

                        'Prelevo l'elenco dei Pagamenti
                        xmlDatiPagamenti = xMovimento.GetElementsByTagName("DatiPagamenti")

                        i_Pagamenti = 0

                        Do While i_Pagamenti < xmlDatiPagamenti.Count

                            xmlDatiPagamento = xmlDatiPagamenti.Item(i_Pagamenti)

                            DatiPagamenti = xmlDatiPagamento.OuterXml

                            objPagamenti = New AgronicaCoreContabBIZ.Pagamento_W

                            dummy = objPagamenti.Pagamento_Scrivi(
                                                CStr(DatiPagamenti),
                                                CInt(Id_Agenda),
                                                CInt(Id_Mov),
                                                objParametri,
                                                Data_creazione,
                                                Data_modifica,
                                                username_creazione,
                                                username_modifica)


                            i_Pagamenti += 1

                        Loop

                        objPagamenti = Nothing



                        '#############################################
                        '##########  MOVIMENTI DETTAGLI  #############
                        '#############################################

                        'Prelevo l'elenco dei dettagli
                        xmlDatiMovimentiDettagli = xMovimento.GetElementsByTagName("DatiMovimenti_Dettagli")

                        i_Movimenti_Dettagli = 0

                        Do While i_Movimenti_Dettagli < xmlDatiMovimentiDettagli.Count

                            xmlDatiMovimentoDettagli = xmlDatiMovimentiDettagli.Item(i_Movimenti_Dettagli)


                            '**************************************************************************
                            'Verifico se devo rimappare i mat_cod
                            If Rimappa_Codici = 1 Then

                                xMovimentiDettagli = xmlDatiMovimentoDettagli.GetElementsByTagName("Movimento_Dettaglio")

                                Dim i_Dettaglio As Int32

                                i_Dettaglio = 0

                                Dim Mat_Cod As Int32
                                Dim Elem_Cod As Int32
                                Dim Mat_Cod_Origine As Int32
                                Dim Pro_Cod As Int32


                                Do While i_Dettaglio < xMovimentiDettagli.Count

                                    xMovimentoDettagli = xMovimentiDettagli.Item(i_Dettaglio)

                                    Pro_Cod = -1000

                                    Pro_Cod = CInt(xMovimentoDettagli.GetAttribute("pro_cod"))

                                    If Pro_Cod = 0 Then

                                        'Mat_Cod magico, se compare nell'xml,
                                        'significa che la rimappatura non è andata a buon fine
                                        Mat_Cod = -666

                                        Elem_Cod = -1000

                                        Mat_Cod_Origine = CInt(xMovimentoDettagli.GetAttribute("mat_cod"))

                                        Elem_Cod = CInt(xMovimentoDettagli.GetAttribute("elem_cod"))

                                        If Piva_SuperUser_Origine <> "" Then 'Rimappo il mat_cod

                                            Select Case Elem_Cod

                                                Case 1 'Macchine

                                                    objComMacR = New AgronicaCoreContabDAL.Parco_Macchine_R

                                                    objDtMat = objComMacR.LeggiMacchinaDaOrigine(
                                                                            CInt(Mat_Cod_Origine),
                                                                            CStr(Piva_SuperUser_Origine),
                                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                            "",
                                                                            "",
                                                                            objParametri)


                                                    If objDtMat.Rows.Count > 0 Then

                                                        Mat_Cod = CInt(objDtMat.Rows(0).Item("mac_cod").Value)

                                                    Else
                                                        'non dovrebbe verificarsi!
                                                        'verrà impostato a Mat_Cod magico
                                                    End If

                                                    objComMacR = Nothing
                                                    objDtMat = Nothing

                                                    xMovimentoDettagli.SetAttribute("mat_cod", CStr(Mat_Cod))

                                                Case 0

                                                    objComRisUmR = New AgronicaCoreAnagrafeDAL.Risorse_Umane_R

                                                    objDtMat = objComRisUmR.LeggiSoloContatto(
                                                                            "",
                                                                            0,
                                                                            "",
                                                                            0,
                                                                            CInt(Mat_Cod_Origine),
                                                                            CStr(Piva_SuperUser_Origine),
                                                                            True,
                                                                            CDate(xMovimento.GetAttribute("validita_inizio")),
                                                                            CDate(xMovimento.GetAttribute("validita_fine")),
                                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                            "",
                                                                            "",
                                                                            objParametri)

                                                    If objDtMat.Rows.Count > 0 Then

                                                        Mat_Cod = CInt(objDtMat.Rows(0).Item("cod_risum").Value)

                                                    Else
                                                        'non dovrebbe verificarsi!
                                                        'verrà impostato a Mat_Cod magico
                                                    End If

                                                    objComRisUmR = Nothing
                                                    objDtMat.Dispose()
                                                    objDtMat = Nothing

                                                    xMovimentoDettagli.SetAttribute("mat_cod", CStr(Mat_Cod))

                                                    '--------------

                                                Case Else 'gestisco tutti gli altri casi, che per forza sono mat_cod
                                                    'Case 10, 201, 3
                                                    'Case ALTRE_MATERIE, FERTILIZZANTI, _
                                                    '    SEMILAVORATI_VEGETALI, MATERIE_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI, _
                                                    '    SEMILAVORATI_ANIMALI, MATERIE_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI, _
                                                    '    MANGIMI, FARMACI

                                                    objComMatPrR = New AgronicaCoreAnagrafeDAL.Materie_Prime_R

                                                    objDtMat = objComMatPrR.LeggiMateriePrimeDaOrigine(
                                                                    "",
                                                                    CInt(Elem_Cod),
                                                                    0,
                                                                    CInt(Mat_Cod_Origine),
                                                                    CStr(Piva_SuperUser_Origine),
                                                                    True,
                                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                    "",
                                                                    "",
                                                                    objParametri)


                                                    If objDtMat.Rows.Count > 0 Then
                                                        Mat_Cod = CInt(objDtMat.Rows(0).Item("mat_cod").Value)
                                                    Else
                                                        'non dovrebbe verificarsi!
                                                        'verrà impostato a Mat_Cod magico
                                                    End If

                                                    objComMatPrR = Nothing
                                                    objDtMat.Dispose()
                                                    objDtMat = Nothing

                                                    xMovimentoDettagli.SetAttribute("mat_cod", CStr(Mat_Cod))


                                                    '****************************************************
                                                    '**** Materie_Prime_Campionature
                                                    '****************************************************

                                                    If Lav_Cod = 125 Then 'Se è una Raccolta

                                                        Dim Progressivo As Long
                                                        Dim flagPresente As Boolean
                                                        Dim i_MPC As Integer

                                                        flagPresente = False
                                                        Progressivo = 0


                                                        i_MPC = 0

                                                        xDatiMateriePC = xMovimentoDettagli.GetElementsByTagName("Raccolto_Campionatura")

                                                        Do While i_MPC < xDatiMateriePC.Count

                                                            flagPresente = False

                                                            xMateriaPC = xDatiMateriePC.Item(i_MPC)

                                                            objComMatCamR = New AgronicaCoreContabDAL.Materie_Prime_Campionature_R


                                                            objDtMat = objComMatCamR.Leggi(0,
                                                                                           "",
                                                                                           0,
                                                                                           0,
                                                                                           CInt(xMovimentoDettagli.GetAttribute("cal_cod")),
                                                                                           Piva_SuperUser_Origine,
                                                                                           True,
                                                                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                           "",
                                                                                           "",
                                                                                           objParametri)

                                                            If objDtMat.Rows.Count > 0 Then

                                                                flagPresente = True
                                                                Progressivo = CInt(objDtMat.Rows(0).Item("Progressivo"))

                                                            Else
                                                                'può verificarsi!
                                                            End If

                                                            If flagPresente = False Then

                                                                objSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

                                                                Progressivo = objSequenze.NuovoId_Tabella("Materie_Prime_Campionature",
                                                                                                          CInt(0),
                                                                                                          CInt(2000000000),
                                                                                                          objParametri)

                                                                Progressivo = Progressivo * (-1)

                                                                objSequenze = Nothing

                                                            End If

                                                            xMateriaPC.SetAttribute("progressivo", CStr(Progressivo))
                                                            xMateriaPC.SetAttribute("progressivo_origine", CStr(xMovimentoDettagli.GetAttribute("cal_cod")))
                                                            xMateriaPC.SetAttribute("piva_superuser_origine", CStr(Piva_SuperUser_Origine))

                                                            i_MPC += 1

                                                        Loop

                                                        '********************************************************


                                                    End If 'FINE: If Lav_Cod = 125 Then


                                                    objComMatCamR = Nothing
                                                    objDtMat.Dispose()
                                                    objDtMat = Nothing


                                            End Select

                                        End If 'FINE: If Piva_SuperUser_Origine <> "" Then 'Rimappo il mat_cod

                                    End If 'FINE: If Pro_Cod = 0 then


                                    i_Dettaglio += 1

                                Loop 'FINE: Do While i_Dettaglio < xMovimenti_Dettagli.length


                            End If 'FINE: If Rimappa_Codici = 1 Then
                            '**************************************************************************

                            DatiMovimenti_Dettagli = xmlDatiMovimentoDettagli.OuterXml

                            objMovimentiDettagli = New AgronicaCoreContabBIZ.Movimenti_Dettagli_W


                            Dim mCodiciRimappati As String = ""
                            If CodiciRimappati <> "" Then
                                mCodiciRimappati = String.Join("|", vCodiciRimappati)
                            End If

                            '@MIRROR@
                            dummy = objMovimentiDettagli.Movimento_Dettaglio_Scrivi(
                                                CStr(DatiMovimenti_Dettagli),
                                                CInt(Id_Agenda),
                                                CInt(Id_Mov),
                                                CInt(Lav_Cod),
                                                Progressivo_Mirror,
                                                Flag_Mirror,
                                                objParametri,
                                                CodiciRimappati:=mCodiciRimappati,
                                                G2G:=G2G,
                                                DataMovimento:=CDate(xMovimento.GetAttribute("data_movimento")))

                            If CodiciRimappati <> "" Then
                                vCodiciRimappati = mCodiciRimappati.Split("|")
                            End If



                            i_Movimenti_Dettagli += 1

                        Loop

                        objMovimentiDettagli = Nothing


                        Select Case OpeDB_Movimento


                            Case 3 'CANCELLAZIONE MOVIMENTO

                                '@MIRROR@

                                If Flag_Mirror = 1 Then

                                    '************************************************
                                    '************************************************
                                    '*********** INIZIO MIRRORING *******************
                                    '************************************************

                                    'IN CASO DI CANCELLAZIONE, VA INSERITA L'OPERAZIONE NELLA TABELLA MIRROR
                                    'E POI CANCELLATA NELLA TABELLA BUONA


                                    'MOVIMENTO_MIRROR_W

                                    objMovimentoMirrorW = New AgronicaCoreContabDAL.Movimenti_Mirror_W

                                    'scrivo nella tabella mirror
                                    intDummy = objMovimentoMirrorW.Scrivi(
                                                CStr(xMovimento.GetAttribute("piva")),
                                                CInt(xMovimento.GetAttribute("sa_cod")),
                                                CInt(Id_Agenda),
                                                CInt(Id_Mov),
                                                Progressivo_Mirror,
                                                CInt(xMovimento.GetAttribute("cod_risum")),
                                                CStr(xMovimento.GetAttribute("cau_mov")),
                                                CStr(xMovimento.GetAttribute("mov_desc")),
                                                CStr(xMovimento.GetAttribute("data_movimento")),
                                                CDate(xMovimento.GetAttribute("scadenza")),
                                                CDec(xMovimento.GetAttribute("doc_numero")),
                                                CDec(xMovimento.GetAttribute("num_protocollo")),
                                                IIf(xMovimento.HasAttribute("cod_indirizzorisum") = False, 0, xMovimento.GetAttribute("cod_indirizzorisum")),
                                                IIf(xMovimento.HasAttribute("cod_destinazione") = False, 0, xMovimento.GetAttribute("cod_destinazione")),
                                                IIf(xMovimento.HasAttribute("cod_indirizzodestinazione") = False, 0, xMovimento.GetAttribute("cod_indirizzodestinazione")),
                                                IIf(xMovimento.HasAttribute("mezzo") = False, 0, xMovimento.GetAttribute("mezzo")),
                                                IIf(xMovimento.HasAttribute("cod_vettore") = False, 0, xMovimento.GetAttribute("cod_vettore")),
                                                IIf(xMovimento.HasAttribute("cod_indirizzovettore") = False, 0, xMovimento.GetAttribute("cod_indirizzovettore")),
                                                IIf(xMovimento.HasAttribute("causale_trasporto") = False, "", xMovimento.GetAttribute("causale_trasporto")),
                                                IIf(xMovimento.HasAttribute("aspetto") = False, "", xMovimento.GetAttribute("aspetto")),
                                                IIf(xMovimento.HasAttribute("peso") = False, 0, xMovimento.GetAttribute("peso")),
                                                IIf(xMovimento.HasAttribute("ora") = False, 0, xMovimento.GetAttribute("ora")),
                                                IIf(xMovimento.HasAttribute("colli") = False, 0, xMovimento.GetAttribute("colli")),
                                                IIf(xMovimento.HasAttribute("tipo_sconto") = False, 0, xMovimento.GetAttribute("tipo_sconto")),
                                                IIf(xMovimento.HasAttribute("extra_str") = False, "", xMovimento.GetAttribute("extra_str")),
                                                IIf(xMovimento.HasAttribute("extra_int") = False, 0, xMovimento.GetAttribute("extra_int")),
                                                IIf(xMovimento.HasAttribute("extra_date") = False, AGRODATAINIZIO, xMovimento.GetAttribute("extra_date")),
                                                CDate(xMovimento.GetAttribute("validita_inizio")),
                                                CDate(xMovimento.GetAttribute("validita_fine")),
                                                objParametri)


                                    objMovimentoMirrorW = Nothing


                                    '************************************************
                                    '*********** FINE MIRRORING *********************
                                    '************************************************
                                    '************************************************

                                End If 'Flag_Mirror



                                objMovimenti.Cancella(
                                                CStr(xMovimento.GetAttribute("piva")),
                                                CInt(xMovimento.GetAttribute("sa_cod")),
                                                CInt(xMovimento.GetAttribute("id_agenda")),
                                                CInt(xMovimento.GetAttribute("id_mov")),
                                                "",
                                                objParametri)

                        End Select


                        '===============================================================================================
                    End If 'IPNO DELETE
                    '===============================================================================================


                    'Elimino l'oggetto
                    objMovimenti = Nothing


                    '-------------------------------------------------------------

                    'Incremento l'indice
                    i_Movimenti += 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiMovimenti += 1

            Loop


            '------------------------------
            '------------------------------
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            xDatiMovimenti = Nothing
            xDatiMovimento = Nothing
            xMovimenti = Nothing
            xMovimento = Nothing
            xmlDatiMovimentiDettagli = Nothing
            xmlDatiMovimentoDettagli = Nothing
            xmlDoc = Nothing

            '------------------------------

            'Restituisco un valore Dummy
            xRisp = True

            'Se ho la transazione è stata avviata in questa routine faccio il commit
            If flagTransazioneLocale = True Then
                objParametri.objTransazione.Commit()
            End If

            '----------------------------------------------------------------------------

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                objParametri.objTransazione.Rollback()
                objParametri.objTransazione = Nothing
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            If (flagConnessioneLocale = True) AndAlso (Not objParametri.objConnessione Is Nothing) Then
                objParametri.objConnessione.Close()
            End If

        End Try

        CodiciRimappati = String.Join("|", vCodiciRimappati)

        Return xRisp

    End Function

    Public Function Scrivi_Modifica(ByVal Movimenti As AgronicaCoreEntityFramework_POCO.Movimenti,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Integer

        Const nomeRoutine = "AgronicaCoreContabBIZ.Movimenti_W.Scrivi_Modifica()"
        Dim messaggioErrore As String = ""
        Dim idMov As Integer = 0

        Try
            Dim agroDP As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim esiste = False
            If Movimenti.Id_Mov = 0 Then

                idMov = agroDP.NuovoId_Tabella("MOVIMENTI", 0, 200000000, objParametriServer)
                Movimenti.Id_Mov = idMov

            Else

                idMov = Movimenti.Id_Mov

                Dim movimentiCount = From a In GiasContext.Movimenti
                                     Where a.Id_Agenda = Movimenti.Id_Agenda And
                                         a.Id_Mov = idMov
                                     Select a
                If movimentiCount.Count > 0 Then
                    esiste = True
                End If

            End If



            Dim movimentiW As New AgronicaCoreContabDAL.Movimenti_W

            If esiste Then
                movimentiW.Modifica(Movimenti, GiasContext, objParametriServer)
            Else
                movimentiW.Scrivi(Movimenti, GiasContext, objParametriServer)
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return idMov

    End Function

    Public Sub Elimina(ByRef Movimenti As AgronicaCoreEntityFramework_POCO.Movimenti(),
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreContabBIZ.Movimenti_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try

            Dim movimentiW As New AgronicaCoreContabDAL.Movimenti_W
            For Each Movimento In Movimenti
                movimentiW.Elimina(Movimento, GiasContext, objParametriServer)
            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

End Class