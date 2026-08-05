Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreStampeDAL
Imports AgronicaCoreUtility
Imports CrystalDecisions.Shared

Partial Class Fattura_NotaAccredito
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private _rptFattura As Object
    Private _rptIva As Iva
    Private _logErrori As String = ""

    Private _moduliCliente As List(Of Integer)
    Private _moduloCantine As Boolean
    Private _moduloFreshFood As Boolean
    Private _moduloTabacco As Boolean
    Private _moduloZoo As Boolean

    Private _piva As String
    Private _pivaReale As String
    Private _lavCod As Integer
    Private _idAgenda As Integer
    Private _codReport As enum_CodificaStampe
    Private _identNumero As String = ""
    Private _identData As String = ""
    Private _identContatto As String = ""
    Private _flagStampaNumeroVasca As Boolean = False
    Private _tabellaCodBase As Integer = enum_OTabelle.Nessuno
    Private _dataInizioAllegato, _dataFineAllegato As Date

    Private _objConfigStampe As ConfigurazioneStampe = Nothing
    Private _usoConfigStampe As Boolean = False
    Private _nuoviArrotondamenti As Boolean = False
    Private _tipoArrotondamentoFF As enum_TipoArrotondamentoFF = enum_TipoArrotondamentoFF.Nessuno

    Private _objTradStampe As Traduzioni_Stampe = Nothing

    'Parametri per Stampa personalizzata
    'Passa dal filtro : Filtro_Fattura
    Private _qsTipoView As String = "" 'N - Normale , P - Raggruppata per prodotto 
    Private _qsTipoOutput As String = "" 'P - PDF (logo) , C - Carta intestata (No logo, ma ci sono gli spazi sopra e sotto) 

    'Definisce se al termine deve essere visualizzato l'anteprima del report (1) oppure direttamente il pdf (1)
    '*** Importante: anche se c'è _qsAnteprima = 1, poi il visualizzatore report va a vedere l'impostazione utente di enum_Impostazioni_Utenti.UTENTE_COD_MODALITA_STAMPA
    '                quindi, in base a quest'ultima opzione, potrebbe cmq restituire il pdf
    'Questo nuovo parametro si è reso necessario per la stampa del documento in background che serve per includerlo nel file XML della fattura elettronica
    Private _qsAnteprima As Integer = 1         'per mantenere il pregresso il default qui è 1 = Con Anteprima

    Private _objParametriServer As AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreParametri
    Private _progressivoGias As Integer

    Private _CodiceLingua As String = "IT" 'default

    Private _EsigibilitaIva As Integer

    '----------------------
    'PARAMETRI
    Private parametro As New Parametri

    '####################################################################
    Private Class Parametri
        Public Property TotaleFattura As String
        Public Property Conai As String
        Public Property TitoloDocumento As String
        Public Property RiferimentoDocumento As String
        Public Property IntestazioneRiga1 As String
        Public Property IntestazioneRiga2 As String
        Public Property IntestazioneRiga3 As String
        Public Property IntestazioneRiga4 As String
        Public Property IntestazioneRiga5 As String
        Public Property IntestazioneRiga6 As String
        Public Property IntestazioneRiga7 As String
        Public Property IntestazioneRiga8 As String
        Public Property IntestazioneRiga9 As String
        Public Property IntestazioneRiga10 As String
        Public Property IntestazioneRiga11 As String
        Public Property IntestazioneRiga12 As String
        Public Property IntestazioneRiga13 As String
        Public Property IntestazioneRiga14 As String
        Public Property IntestazioneRiga15 As String
        Public Property LblPivaCliente As String
        Public Property LblCfCliente As String
        Public Property TxtPivaCliente As String
        Public Property TxtCfCliente As String
        Public Property LblScadenza As String
        Public Property Litri As String
        Public Property AgenteInfo As String
        Public Property DescAggiuntivo As String
        Public Property RagSocAggiuntivo As String
        Public Property IndirizzoAggiuntivo As String
        Public Property CapAggiuntivo As String
        Public Property FrazioneAggiuntivo As String
        Public Property ComuneAggiuntivo As String
        Public Property ProvAggiuntivo As String
        Public Property TxtPivaAgg As String
        Public Property TxtCfAgg As String

        Public Property T_FatturatoA As String
        Public Property T_Destinatario As String
        Public Property T_DataEmissione As String
        Public Property T_Numero As String
        Public Property T_Valuta As String
        Public Property T_Pagina As String
        Public Property T_ModalitaPagamento As String
        Public Property T_Note As String
        Public Property T_DataSpedizione As String
        Public Property T_Colli As String
        Public Property T_Peso As String
        'non gestito, lasciato LitriLabel1 con dicitura fissa LT
        'Public Property T_Litri As String 
        Public Property T_Aspetto As String
        Public Property T_Causale As String
        Public Property T_Trasporto As String
        Public Property T_GestioneVettore As String
        Public Property T_Vettore As String
        Public Property T_DataConsegna As String
        Public Property T_FirmaDestinatario As String
        Public Property T_FirmaConducente As String
        Public Property T_DescrizioneBeni As String
        Public Property T_UM As String
        Public Property T_Qta As String
        Public Property T_Prezzo As String
        Public Property T_Sconto As String
        Public Property T_Importo As String
        Public Property T_AliqIVA As String
        Public Property T_TotImplordo As String
        Public Property T_TotSconti As String
        Public Property T_TotImpNetto As String
        Public Property T_TotImposta As String
        Public Property T_Tot As String 'TxtTotale
        Public Property T_TotDocumento 'TxtTotaleDaPagare non è gestita
        Public Property T_CalcoloImposta As String
        Public Property T_IVA_imponibile As String
        Public Property T_IVA_aliquota As String
        Public Property T_IVA_imposta As String
        Public Property T_Articolo62 As String
        Public Property T_Privacy As String
        Public Property T_SEO As String
        Public Property CodiceSDI As String


        Public Sub New()
            TotaleFattura = ""
            Conai = ""
            TitoloDocumento = ""
            RiferimentoDocumento = ""
            IntestazioneRiga1 = ""
            IntestazioneRiga2 = ""
            IntestazioneRiga3 = ""
            IntestazioneRiga4 = ""
            IntestazioneRiga5 = ""
            IntestazioneRiga6 = ""
            IntestazioneRiga7 = ""
            IntestazioneRiga8 = ""
            IntestazioneRiga9 = ""
            IntestazioneRiga10 = ""
            IntestazioneRiga11 = ""
            IntestazioneRiga12 = ""
            IntestazioneRiga13 = ""
            IntestazioneRiga14 = ""
            IntestazioneRiga15 = ""
            LblPivaCliente = "Partita Iva:"
            LblCfCliente = "Codice Fiscale:"
            TxtPivaCliente = ""
            TxtCfCliente = ""
            LblScadenza = "Scadenza"
            Litri = ""
            AgenteInfo = ""
            DescAggiuntivo = ""
            RagSocAggiuntivo = ""
            IndirizzoAggiuntivo = ""
            CapAggiuntivo = ""
            FrazioneAggiuntivo = ""
            ComuneAggiuntivo = ""
            ProvAggiuntivo = ""
            TxtPivaAgg = ""
            TxtCfAgg = ""
            CodiceSDI = ""

        End Sub

    End Class
    '####################################################################


    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object


    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()

        '  Giulia, 18/01/2017 18:15:31: ricavo piva ed id_agenda per ricavare il modulo usato e se sto usando un cod_risum che ha una personalizzazione
        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        _idAgenda = CInt(Stringa_Decodifica(CStr(Request.QueryString("i")), AgroKey_EncoderDecoder, Server))

        _codReport = CInt(Stringa_Decodifica(CStr(Request.QueryString("rep")), AgroKey_EncoderDecoder, Server))

        _lavCod = CInt(Stringa_Decodifica(CStr(Request.QueryString("l")), AgroKey_EncoderDecoder, Server))

        _objParametriServer = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        _objParametriUtenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        _progressivoGias = CInt(Session("ASG_ProgressivoGIAS"))

        'Verifica Tipologia clienti Cantine/Fresh&Food/ecc
        Dim objOmni As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
        _moduliCliente = objOmni.Recupera_Moduli_Cliente(_piva, _objParametriServer)

        _moduloCantine = _moduliCliente.Contains(enum_Omni_Modulo_Generazione.Cantine)
        _moduloFreshFood = _moduliCliente.Contains(enum_Omni_Modulo_Generazione.FreshFood)
        _moduloTabacco = _moduliCliente.Contains(enum_Omni_Modulo_Generazione.Tabacco)
        _moduloZoo = _moduliCliente.Contains(enum_Omni_Modulo_Generazione.Zoo)


        '  Giulia, 18/01/2017 18:16:20: devo ricavare il CodRisUm principale
        Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R
        Dim cod_Indirizzo As Integer
        Dim cod_risUm As Integer = objMovimenti.Leggi_CodRisUm_CodIndirizzoRisUm_Cessionario_From_idAgenda(_piva, _idAgenda, "", _
                                                                                                           cod_Indirizzo, _
                                                                                                           _objParametriServer)



        '------ GESTIONE TRADUZIONE LAYOUT -------------
        '19/10/2020: utilizzata la funzione generale dei documenti contab, che contempla l'impostazione utente
        _CodiceLingua = OttieniLingua_ReportContabilita(_objParametriServer, _objParametriUtenti, _lavCod, _piva, _idAgenda)

        ''introdotta il 18/03/2019
        'Select Case _lavCod
        '    'LAVCOD_NOTA_ACCREDITO_EMESSA
        '    'al momento attivo solo per le fatture
        '    Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_PROFORMA
        '        Dim cod_Indirizzo_Destinazione As Integer
        '        Dim cod_risUm_Destinazione As Integer = objMovimenti.Leggi_CodRisUm_CodIndirizzoRisUm_CessionarioDiverso_From_idAgenda(_piva, _idAgenda, "",
        '                                                                                                                               cod_Indirizzo_Destinazione, _
        '                                                                                                                               _objParametriServer)

        '        Dim objContIndir As New AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R

        '        If cod_risUm_Destinazione <> 0 Then
        '            'sulla fattura è impostata una destinazione diversa

        '            'la lingua  si prende dal cessionario diverso
        '            _CodiceLingua = objContIndir.CodiceLingua_from_CodRisUm_o_CodIndirizzoRisUm(cod_risUm_Destinazione, cod_Indirizzo_Destinazione, "", "", _objParametriServer)
        '            If _CodiceLingua = "" Then
        '                _CodiceLingua = "IT"
        '            End If
        '        Else
        '            'sulla fattura non c'è destinazione diversa

        '            'la lingua si prende dal cessionario
        '            _CodiceLingua = objContIndir.CodiceLingua_from_CodRisUm_o_CodIndirizzoRisUm(cod_risUm, cod_Indirizzo, "", "", _objParametriServer)
        '            If _CodiceLingua = "" Then
        '                _CodiceLingua = "IT"
        '            End If

        '        End If
        'End Select

        If _CodiceLingua <> "" AndAlso _CodiceLingua <> "IT" Then

            _objTradStampe = New Traduzioni_Stampe(_piva, _
                                                  enum_CodificaStampe.Fatture, _
                                                   _CodiceLingua, _
                                                   0, _
                                                   "", "", _
                                                   _objParametriServer)

            If _objTradStampe.Fattura_TraduzioneValorizzata = True Then
                'sull'installazione non sono presenti traduzioni
                '-> stampo la fattura in ITALIANO anche se la destinazione/cliente è straniero
                Imposta_TextReport_STRANIERE()
            Else
                'sul db non sono installate le traduzioni
                'redirigo su lingua italiana
                _CodiceLingua = "IT"
                Imposta_TextReport_ITALIANE()
            End If

        Else
            Imposta_TextReport_ITALIANE()
        End If
        '----------------------------------------------------------


        ' Giulia: 2/11/2017:se mi è arrivato un ddt con dettagli economici, dalla tabella devo andare a leggere le informazioni
        'come se mi fosse arrivato direttamente una fattura, perché devo istanziare la classe fattura2016, non bolla2016
        Dim codReportLocale As enum_CodificaStampe = _codReport

        If codReportLocale = enum_CodificaStampe.DDT_Contabilizzato_Emesso OrElse codReportLocale = enum_CodificaStampe.Bolle Then
            Dim checkPrezzo As Integer = 0
            Dim objMov As New AgronicaCoreContabDAL.Movimenti_R
            checkPrezzo = objMov.ChkLayOutPrezzo_from_id_Agenda(_piva,
                                                                _idAgenda,
                                                                0,
                                                                "",
                                                                _objParametriServer)
            objMov = Nothing
            If checkPrezzo = 1 AndAlso _lavCod <> LAVCOD_CONFERIMENTO AndAlso _lavCod <> LAVCOD_CONFERIMENTO_DIVERSI Then
                codReportLocale = enum_CodificaStampe.Fatture
            End If
        End If

        _nuoviArrotondamenti = UsaNuoviArrotondamenti(_objParametriServer)

        Dim objConfStampe = New Configurazione_Stampe_R
        Dim dt As DataTable = objConfStampe.Leggi(_piva, codReportLocale, -1, cod_risUm, "", "", _objParametriServer)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

            _usoConfigStampe = True
            _objConfigStampe = New ConfigurazioneStampe(dt.Rows(0))

            Dim nomeReportCrystal As String = _objConfigStampe.NomeFileRpt
            Dim classeReportCrystal As String = nomeReportCrystal.Remove((nomeReportCrystal.Length - 4), 4)

            '  Giulia, 18/01/2017 18:18:27: permette di creare dinamicamente il giusto report
            _rptFattura = Activator.CreateInstance(Type.GetType("AgronicaStampe_2010." & classeReportCrystal))

            'Select Case classeReportCrystal
            '    Case "CRFattura_Trombin"
            '        rptFattura = New CRFattura_Trombin
            '    Case "CRFattura2016"
            '        rptFattura = New CRBFattura2016
            '    Case "CRFattura2016_LB"
            '        rptFattura = New CRFattura2016_LB
            '    Case "CRFattura"
            '        rptFattura = New CRFattura
            '    Case Else
            '        rptFattura = New CRFattura
            'End Select

        Else

            'non presente in tabella configurazione stampe oppure tabella nn è stata letta
            _usoConfigStampe = False
            _objConfigStampe = New ConfigurazioneStampe()

            Select Case _progressivoGias
                Case enum_CodiceGIAS_Clienti.Trombin,
                    enum_CodiceGIAS_Clienti.SBTF,
                    enum_CodiceGIAS_Clienti.CoFruTa

                    _rptFattura = New CRFattura_Trombin

                Case enum_CodiceGIAS_Clienti.AlCanevon,
                    enum_CodiceGIAS_Clienti.LaRizzola,
                    enum_CodiceGIAS_Clienti.Lorenzato,
                    enum_CodiceGIAS_Clienti.FattoriaMonticinoRosso,
                    enum_CodiceGIAS_Clienti.PoderePalazzo,
                    enum_CodiceGIAS_Clienti.Zuffa,
                    enum_CodiceGIAS_Clienti.FiorentinaDiSopra

                    _rptFattura = New CRFattura2016


                Case enum_CodiceGIAS_Clienti.MaioranoFormaggio,
                         enum_CodiceGIAS_Clienti.MaioranoRaffaele


                    'PERSONALIZZAZIONE MAIORANO
                    'Se in PDF deve stampare i loghi (fatto con la stampa nuova dove i loghi sono su FileSystem)
                    'Se su carta o non specificato : la sezione deve rimanere vuota --> quindi ho tenuto la stampa vecchia (*)
                    '(*) Non si è usata quella nuova perché nella sezione del logo, avendo messo il file blob, omettendo il logo rimane comunque
                    ' la sezione con la scritta Image

                    _qsTipoOutput = Stringa_Decodifica(CStr(Request.QueryString("to")), AgroKey_EncoderDecoder, Server)

                    If _qsTipoOutput = "P" Then
                        _rptFattura = New CRFattura2016
                    Else
                        _rptFattura = New CRFattura
                    End If

                Case enum_CodiceGIAS_Clienti.Guarini,
                     enum_CodiceGIAS_Clienti.Randi,
                     enum_CodiceGIAS_Clienti.Bartolini,
                     enum_CodiceGIAS_Clienti.DeFaveri,
                     enum_CodiceGIAS_Clienti.Sandrin

                    _rptFattura = New CRFattura2016_LB


                Case Else

                    'si rimanda a CRFattura perchè tanto il layout rimane attivo per via di Maiorano
                    _rptFattura = New CRFattura

            End Select

        End If

        _rptIva = New Iva

    End Sub

    Private Sub Imposta_TextReport_STRANIERE()

        'parametri gestiti dopo:
        'titolo_documento e lblscadenza 
        ' parametro.T_Articolo62 = _objTradStampe.Fattura_Articolo62

        parametro.T_FatturatoA = _objTradStampe.Fattura_FatturatoA
        parametro.T_Destinatario = _objTradStampe.Fattura_Destinatario
        parametro.T_DataEmissione = _objTradStampe.Fattura_DataEmissione
        parametro.T_Numero = _objTradStampe.Fattura_Numero
        parametro.T_Valuta = _objTradStampe.Fattura_Valuta
        parametro.T_Pagina = _objTradStampe.Fattura_Pagina
        parametro.T_ModalitaPagamento = _objTradStampe.Fattura_ModalitaPagamento
        parametro.T_Note = _objTradStampe.Fattura_Note
        parametro.T_DataSpedizione = _objTradStampe.Fattura_DataSpedizione
        parametro.T_Colli = _objTradStampe.Fattura_Colli
        parametro.T_Peso = _objTradStampe.Fattura_Peso
        parametro.T_Aspetto = _objTradStampe.Fattura_Aspetto
        parametro.T_Causale = _objTradStampe.Fattura_Causale
        parametro.T_Trasporto = _objTradStampe.Fattura_Trasporto
        parametro.T_GestioneVettore = _objTradStampe.Fattura_GestioneVettore
        parametro.T_Vettore = _objTradStampe.Fattura_Vettore
        parametro.T_DataConsegna = _objTradStampe.Fattura_DataConsegna
        parametro.T_FirmaDestinatario = _objTradStampe.Fattura_FirmaDestinatario
        parametro.T_FirmaConducente = _objTradStampe.Fattura_FirmaConducente
        parametro.T_DescrizioneBeni = _objTradStampe.Fattura_DescrizioneBeni
        parametro.T_UM = _objTradStampe.Fattura_UM
        parametro.T_Qta = _objTradStampe.Fattura_Qta
        parametro.T_Prezzo = _objTradStampe.Fattura_Prezzo
        parametro.T_Sconto = _objTradStampe.Fattura_Sconto
        parametro.T_Importo = _objTradStampe.Fattura_Importo
        parametro.T_AliqIVA = _objTradStampe.Fattura_AliqIVA
        parametro.T_TotImplordo = _objTradStampe.Fattura_TotImplordo
        parametro.T_TotSconti = _objTradStampe.Fattura_TotSconti
        parametro.T_TotImpNetto = _objTradStampe.Fattura_TotImpNetto
        parametro.T_TotImposta = _objTradStampe.Fattura_TotImposta
        parametro.T_Tot = _objTradStampe.Fattura_Tot
        'X il totale documento Fattura_TotDocumento
        'parametro.T_TotDocumento = _objTradStampe.Fattura_TotDocumento
        parametro.T_SEO = _objTradStampe.Fattura_SEO
        parametro.T_CalcoloImposta = _objTradStampe.Fattura_CalcoloImposta
        parametro.T_IVA_imponibile = _objTradStampe.Fattura_IVA_imponibile
        parametro.T_IVA_aliquota = _objTradStampe.Fattura_IVA_aliquota
        parametro.T_IVA_imposta = _objTradStampe.Fattura_IVA_imposta
        parametro.T_Privacy = _objTradStampe.Fattura_Privacy

    End Sub

    Private Sub Imposta_TextReport_ITALIANE()

        parametro.T_FatturatoA = "Fatturato a:"
        parametro.T_Destinatario = "Destinatario:"
        parametro.T_DataEmissione = "Data Emissione"
        parametro.T_Numero = "Numero"
        parametro.T_Valuta = "Valuta"
        parametro.T_Pagina = "Pagina"
        parametro.T_ModalitaPagamento = "Modalità di Pagamento:"
        parametro.T_Note = "Note:"
        parametro.T_DataSpedizione = "Data spedizione"
        parametro.T_Colli = "Colli"
        parametro.T_Peso = "Peso complessivo"
        parametro.T_Aspetto = "Aspetto esteriore dei beni"
        parametro.T_Causale = "Causale"
        parametro.T_Trasporto = "Trasporto a cura del"
        parametro.T_GestioneVettore = "Gestione Vettore"
        parametro.T_Vettore = "Vettore"
        parametro.T_DataConsegna = "Data consegna"
        parametro.T_FirmaDestinatario = "Firma destinatario"
        parametro.T_FirmaConducente = "Firma conducente"
        parametro.T_DescrizioneBeni = "Descrizione della merce o servizio"
        parametro.T_UM = "U.M."
        parametro.T_Qta = "Quantità"
        parametro.T_Prezzo = "Prezzo"
        parametro.T_Sconto = "Sconto"
        parametro.T_Importo = "Importo"
        parametro.T_AliqIVA = "Aliq. IVA"
        parametro.T_TotImplordo = "Totale Imponibile Lordo"
        parametro.T_TotSconti = "Totale Sconti/Magg."
        parametro.T_TotImpNetto = "Totale Imponibile Netto"
        parametro.T_TotImposta = "Totale Imposta"
        parametro.T_Tot = "Totale"
        parametro.T_TotDocumento = "Totale Documento"
        parametro.T_SEO = "S.E.& O."
        parametro.T_CalcoloImposta = "Calcolo Imposta"
        parametro.T_IVA_imponibile = "Imponibile"
        parametro.T_IVA_aliquota = "Aliquota o Articolo Esclusione/Non Imponibile Iva"
        parametro.T_IVA_imposta = "Imposta"
        parametro.T_Articolo62 = "Assolve agli obblighi dell'Art. 3 del D.Lgs 198/2021 e s.m.i. Il contratto ha durata per la presente consegna."
        parametro.T_Privacy = "Trattiamo e tuteliamo i Vostri dati esclusivamente per fini amministrativi e contabili in conformità al GDPR - Regolamento UE 2016/679."

    End Sub

    Private Function Recupera_TotaleDocumento(ByVal codice_lingua As String, _
                                             ByVal lav_cod As Integer) As String

        Dim TotaleDocumento As String = ""

        If codice_lingua = "" OrElse codice_lingua = "IT" Then
            '---------------------------
            '---- LINGUA ITA -----

            TotaleDocumento = "Totale Documento"

        Else
            '---------------------------
            '---- LINGUA STRANIERA -----
            Select Case lav_cod
                Case LAVCOD_FATTURA_EMESSA
                    TotaleDocumento = _objTradStampe.Fattura_TotaleFattura

                Case LAVCOD_FATTURA_PROFORMA
                    TotaleDocumento = _objTradStampe.Fattura_TotaleFatturaProforma

                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                    TotaleDocumento = _objTradStampe.Fattura_TotaleNDC

                Case LAVCOD_DDT_CONTABILIZZATO_EMESSO,
                        LAVCOD_BOLLA_EMESSA
                    TotaleDocumento = _objTradStampe.Fattura_TotaleDDT

                Case LAVCOD_ORDINE_VENDITA
                    TotaleDocumento = _objTradStampe.Fattura_TotaleOrdineVendita

                Case LAVCOD_ORDINE_ACQUISTO
                    TotaleDocumento = _objTradStampe.Fattura_TotaleOrdineAcquisto

                Case LAVCOD_PREVENTIVO_VENDITA
                    TotaleDocumento = _objTradStampe.Fattura_TotalePreventivoVendita
            End Select

        End If

        Return TotaleDocumento

    End Function


    Private Function Recupera_TitoloDocumento(ByVal codice_lingua As String, _
                                             ByVal lav_cod As Integer, _
                                             ByVal Tipo_Fattura As enum_TipoStampaFattura) As String

        Dim TitoloDocumento As String = ""

        If codice_lingua = "" OrElse codice_lingua = "IT" Then
            '---------------------------
            '---- LINGUA ITA -----
            Select Case lav_cod

                Case LAVCOD_FATTURA_EMESSA

                    Select Case Tipo_Fattura

                        Case enum_TipoStampaFattura.Autofattura
                            TitoloDocumento = "AUTOFATTURA"

                        Case enum_TipoStampaFattura.Fattura_AcquistiIntracom
                            TitoloDocumento = "FATTURA ESTERA (ACQUISTI INTRACOM.)"

                        Case enum_TipoStampaFattura.Fattura_Acconto, enum_TipoStampaFattura.Fattura_Acconto_Soci
                            TitoloDocumento = "FATTURA D'ACCONTO"

                        Case enum_TipoStampaFattura.Fattura_Differita
                            TitoloDocumento = "FATTURA"

                        Case enum_TipoStampaFattura.Fattura_Immediata
                            TitoloDocumento = "FATTURA ACCOMPAGNATORIA"

                    End Select

                Case LAVCOD_FATTURA_PROFORMA
                    TitoloDocumento = "FATTURA PRO-FORMA"

                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                    TitoloDocumento = "NOTA DI ACCREDITO"

                Case LAVCOD_DDT_CONTABILIZZATO_EMESSO,
                        LAVCOD_BOLLA_EMESSA
                    TitoloDocumento = "Documento di Trasporto Emesso"

                Case LAVCOD_ORDINE_VENDITA
                    TitoloDocumento = "ORDINE DI VENDITA"

                Case LAVCOD_ORDINE_ACQUISTO
                    TitoloDocumento = "ORDINE DI ACQUISTO"

                Case LAVCOD_PREVENTIVO_VENDITA
                    TitoloDocumento = "PREVENTIVO DI VENDITA"
            End Select



        Else
            '---------------------------
            '---- LINGUA STRANIERA -----

            Select Case lav_cod

                Case LAVCOD_FATTURA_EMESSA

                    Select Case Tipo_Fattura

                        Case enum_TipoStampaFattura.Autofattura
                            TitoloDocumento = _objTradStampe.Fattura_TitoloAutoFattura

                        Case enum_TipoStampaFattura.Fattura_AcquistiIntracom
                            TitoloDocumento = _objTradStampe.Fattura_TitoloFatturaIntra

                        Case enum_TipoStampaFattura.Fattura_Acconto, enum_TipoStampaFattura.Fattura_Acconto_Soci
                            TitoloDocumento = _objTradStampe.Fattura_TitoloFatturaAcconto

                        Case enum_TipoStampaFattura.Fattura_Differita
                            TitoloDocumento = _objTradStampe.Fattura_TitoloFatturaDifferita

                        Case enum_TipoStampaFattura.Fattura_Immediata
                            TitoloDocumento = _objTradStampe.Fattura_TitoloFatturaAccomp

                    End Select

                Case LAVCOD_FATTURA_PROFORMA
                    TitoloDocumento = _objTradStampe.Fattura_TitoloFatturaProforma

                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                    TitoloDocumento = _objTradStampe.Fattura_TitoloNDC

                Case LAVCOD_DDT_CONTABILIZZATO_EMESSO,
                        LAVCOD_BOLLA_EMESSA
                    TitoloDocumento = _objTradStampe.Fattura_TitoloDDT

                Case LAVCOD_ORDINE_VENDITA
                    TitoloDocumento = _objTradStampe.Fattura_TitoloOrdineVendita

                Case LAVCOD_ORDINE_ACQUISTO
                    TitoloDocumento = _objTradStampe.Fattura_TitoloOrdineAcquisto

                Case LAVCOD_PREVENTIVO_VENDITA
                    TitoloDocumento = _objTradStampe.Fattura_TitoloPreventivoVendita
            End Select

        End If


        Return TitoloDocumento

    End Function


    '#####################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load

        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        '  Giulia, 18/01/2017 18:21:58: Spostati su Page_Init perché mi servono prima
        '_piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        '_idAgenda = CInt(Stringa_Decodifica(CStr(Request.QueryString("i")), AgroKey_EncoderDecoder, Server))

        '_codReport = CInt(Stringa_Decodifica(CStr(Request.QueryString("rep")), AgroKey_EncoderDecoder, Server))

        '_lavCod = CInt(Stringa_Decodifica(CStr(Request.QueryString("l")), AgroKey_EncoderDecoder, Server))


        '_printToPrinter = Stringa_Decodifica(CStr(Request.QueryString("r")), AgroKey_EncoderDecoder, Server)

        '_printName = Stringa_Decodifica(CStr(Request.QueryString("a")), AgroKey_EncoderDecoder, Server)


        _qsTipoView = Stringa_Decodifica(CStr(Request.QueryString("ts")), AgroKey_EncoderDecoder, Server)

        _qsTipoOutput = Stringa_Decodifica(CStr(Request.QueryString("to")), AgroKey_EncoderDecoder, Server)

        If Request.QueryString("prvw") IsNot Nothing AndAlso Request.QueryString("prvw") <> "" Then
            _qsAnteprima = Stringa_Decodifica(CStr(Request.QueryString("prvw")), AgroKey_EncoderDecoder, Server)
        End If

        '_objParametriServer = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        ' _objParametriUtenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim nomeDocumento As String = "Fattura_NotaAccredito"

        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim ds As New DataSetFattura
            Dim dsIva As New DS_Iva

            Dim catCod As Integer

            Select Case _lavCod
                Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA, LAVCOD_FATTURA_PROFORMA
                    nomeDocumento = "Fattura"
                    catCod = enum_CategorieDocumenti.Fattura_Emessa
                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                    nomeDocumento = "NotaAccredito"
                    catCod = enum_CategorieDocumenti.NotaAccredito_Emessa
                Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
                    nomeDocumento = "DDT_Corrispettivi"
                    catCod = enum_CategorieDocumenti.DDT_Contabilizzato_Emesso
                Case LAVCOD_ORDINE_VENDITA
                    nomeDocumento = "Ordine_Vendita"
                    catCod = enum_CategorieDocumenti.Ordine_Vendita
                Case LAVCOD_ORDINE_ACQUISTO
                    nomeDocumento = "Ordine_Acquisto"
                    catCod = enum_CategorieDocumenti.Ordine_Acquisto
                Case LAVCOD_PREVENTIVO_VENDITA
                    nomeDocumento = "Preventivo"
                    catCod = enum_CategorieDocumenti.Preventivo_Emesso
                Case LAVCOD_BOLLA_EMESSA
                    nomeDocumento = "DDT"
                    catCod = enum_CategorieDocumenti.DDT_Emesso
            End Select

            ''Verifica Tipologia clienti Cantine/Fresh&Food/ecc
            'Dim objOmni As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R

            'objOmni.Recupera_Modulo_Cliente(_piva,
            '                                _moduloCantine,
            '                                _moduloFreshFood,
            '                                _moduloTabacco,
            '                                _objParametriServer)


            If _moduloCantine = True Then

                Try

                    'verifica opzione stampa numero e capacità vasca
                    Dim Flag_StampaCapacitaVasca As Boolean = False
                    Dim OptGestVisualNumVascaRegImbott As Integer = 0
                    Dim Flag_GestioneRegistroVinificazione As Integer = 0

                    Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

                    objUtenti.LeggiOpzioni_RegistriCantina(2,
                                                           _objParametriUtenti,
                                                           _flagStampaNumeroVasca,
                                                           Flag_StampaCapacitaVasca,
                                                           OptGestVisualNumVascaRegImbott,
                                                           Flag_GestioneRegistroVinificazione,
                                                           Nothing)


                Catch ex As Exception
                    _logErrori &= "- Lettura opzioni registri di cantina: " & vbCrLf & ex.Message & vbCrLf
                End Try

            End If

            If _moduloFreshFood = True Then

                Try

                    'verifica udm base calcolo automatico
                    'enum_OTabelle
                    Dim objRef As New AgronicaCoreContabDAL.OModuli_Referenze_Config_Testata_R

                    _tabellaCodBase = objRef.Recupera_Tabella_Cod_Base(enum_Omni_Modulo_Generazione.FreshFood, "", _objParametriServer)

                    Dim objOmni As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                    _tipoArrotondamentoFF = objOmni.TipoArrotondamentoFF(_piva, 0, _objParametriServer)

                Catch ex As Exception
                    _logErrori &= "- Lettura opzioni fresh&food: " & vbCrLf & ex.Message & vbCrLf
                End Try

            End If

            Try

                Stampa_Fattura_NotaAccredito(ds, dsIva)

            Catch exc As Exception
                _logErrori &= "- Stampa_Fattura_NotaAccredito: " & vbCrLf & exc.Message & vbCrLf
            End Try

            Dim identificazioneDocumento As String = ""
            Dim nomeFile As String = ""

            Try

                identificazioneDocumento = nomeDocumento & "_p" & _piva & "_" & _identData & "_" & _identNumero & "_" & Stringhe.TroncaStringa(_identContatto, 40)
                nomeFile = Stringhe.EliminaCaratteriSpecialiFile(identificazioneDocumento) & ".pdf"

                ' leggo la sotto cartella da CategorieDocumenti
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Dim sottoCartella As String = objCatDoc.Sottocartella(catCod, "", "", _objParametriServer)
                objCatDoc = Nothing

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(_rptFattura,
                                           catCod,
                                           sottoCartella,
                                           nomeFile,
                                           _objParametriServer, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim allegatiDocumentiCod As Integer = objAllegati.SalvaAllegato(_piva,
                                                                                catCod,
                                                                                nomeDocumento,
                                                                                nomeFile,
                                                                                sottoCartella,
                                                                                _idAgenda, "", "", "",
                                                                                _dataInizioAllegato,
                                                                                _dataFineAllegato,
                                                                                _objParametriServer)

            Catch ex As Exception
                _logErrori &= "- Gestione allegati: " & vbCrLf & ex.Message & vbCrLf
            End Try

            '17/12/2018
            'Tutti quanti ora salvano il report temporaneo su cartelle e non passano più da sessione

            'MS 25/02/2016 aggiunto nuovo giro per garantire rilascio memoria e thread che provocavano blocco stampe.
            'La vecchia gestione passa in session il riferimento al report per passarlo a VisualizzatoreReport
            'ma non permette distruggere report e DataSet provocando lo sforamento di memoria.

            'Per intervenire progressivamente sui report, in VisualizzatoreReport rimangono le due gestioni con reference e con rpt su 
            'disco fino a quando sarà terminata la migrazione di tutte le stampe e il giro con la session potrà essere eliminato.

            'MS La nuova gestione salva il report con i dati su disco con un nome univoco per ogni esecuzione di stampa e passa a
            'VisualizzatoreReport il nome del pathname da riaprire.
            'Questi files temporanei sono cancellati periodicamente in entrata su GestioneRichieste e non in VisualizzatoreReport perché
            'possono essere eseguite una serie di PostBack es. export su pdf e il report deve rimanere disponibile.
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                _rptFattura.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                _logErrori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            ' Giulia: 2/11/2017:Devo scrivere il file di log anche qui e poi svuotarlo, perché se va in errore il pezzo sotto
            '   esce di qui e di fatto il file di log non lo scrive mai
            SalvaLogErrori_Agenda(_logErrori, nomeDocumento, identificazioneDocumento, "Fattura_NotaAccredito.aspx", "Stampe_Contabilita", _idAgenda, _objParametriServer)
            _logErrori = ""

            Try
                'MS Una volta persistito il report con i dati su file distruggo rpt e DataSet ed eseguo un GC.Collect
                'In questo modo la memoria e i thread non rimangono allocati e non si blocca più dopo alcune stampe.
                ds.Dispose()
                ds = Nothing

                dsIva.Dispose()
                dsIva = Nothing

                _rptIva.Close()
                _rptIva.Dispose()
                _rptIva = Nothing

                _rptFattura.Close()
                _rptFattura.Dispose()
                _rptFattura = Nothing

                GC.Collect()

                'MS Il controllo passa a VisualizzatoreReport passando in QueryString il pathname del report temporaneo su disco in modo che 
                'rimanga disponibile anche nelle PostBack per esportazione ecc.. 
                'Non si può passare sulla session altrimenti l'esecuzione di 2 o più report dalla stessa postazione andrebbe in conflitto
                'Per discriminare fra vecchio e nuovo giro il visualizzatore testa se in QueryString viene passato tmpReportPath

                Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica(CStr(_qsAnteprima), AgroKey_EncoderDecoder, Server) &
                                  "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                                  "&NomePdf=" & Stringa_Codifica(nomeFile, AgroKey_EncoderDecoder, Server))
            Catch ex As Exception
                _logErrori &= "- Distruzione oggetti e redirect report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            SalvaLogErrori_Agenda(_logErrori, nomeDocumento, identificazioneDocumento, "Fattura_NotaAccredito.aspx", "Stampe_Contabilita", _idAgenda, _objParametriServer)

        End If

    End Sub


    '#####################################################################################################
    Private Sub Stampa_Fattura_NotaAccredito(ByRef ds As DataSetFattura,
                                             ByRef dsIva As DS_Iva)


        Dim drIntestazioneNew As DataSetFattura.IntestazioneFatturaRow
        Dim drDescrizioneNew As DataSetFattura.DescrizioneRow
        Dim i As Integer

        '------------------
        'Movimenti
        'Dim x_Tipo_Sconto As Integer = 0
        Dim x_Edit_Importo As enum_EditImporto = enum_EditImporto.PrezzoUnitario

        Dim x_Cod_RisUm_Aggiuntivo As Integer = 0

        Dim x_ChkLayout_Join_Prodotti As Integer = 0
        Dim x_ChkLayOut_Peso As Integer = 0

        Dim x_ChkLayOut_Litri As Integer = 0
        Dim Litri_Totali As Decimal = 0

        Dim Destinatario_Tel As String = ""

        '------------------------------
        'Movimenti Dettagli

        Dim x_Mov_Det_Des As String = ""
        Dim x_Elem_Cod As Integer = 0
        Dim x_Pro_Cod As Integer = 0
        Dim x_Mat_Cod As Integer = 0
        Dim x_Cod_Progetto As Integer = 0
        Dim x_Fase_Cod As Integer = 0
        Dim x_Lotto As String = ""
        Dim x_Cal_Cod As Integer = 0
        Dim x_Udm_Cod As Integer = 0
        Dim x_Udm_Sim As String = ""
        Dim x_Udm_Des As String = ""
        Dim x_Udm_Cod_Extra As Integer = 0
        Dim x_Udm_Sim_Extra As String = ""
        Dim x_Udm_Des_Extra As String = ""
        Dim x_Flag_Extra As Integer = 0
        Dim x_OTabella_Cod_Base As Integer = 0
        Dim x_Qta As Decimal = 0
        Dim x_Qta_Extra As Decimal = 0
        Dim x_Qta_Extra_Totale As Decimal = 0
        Dim x_Descrizione As String = ""
        Dim x_Prezzo_Unitario As Decimal = 0
        Dim x_Prezzo_Unitario_Netto As Decimal = 0
        Dim x_Imponibile As Decimal = 0
        Dim x_Imponibile_Netto As Decimal = 0
        Dim x_Cod_IVA As Integer = 0
        Dim x_Aliquota_Des As String = ""
        Dim x_Aliquota_Val As Integer = 0
        Dim x_IVA As Decimal = 0
        Dim x_ChkIVA_Manuale As Integer = 0
        Dim x_Cod_IVAIndetraibile As Integer = 0
        Dim x_Sconto_Perc As Decimal = 0
        Dim x_Sconto_Perc_2 As Decimal = 0
        Dim x_Sconto_Testo As String = ""
        Dim x_Sconto As Decimal = 0
        Dim x_Sconto_Modalita As Integer = 0
        Dim x_Prezzo_Effettivo As Decimal = 0
        Dim x_Anno As Integer = 0
        Dim x_Ric_Cod As Integer = 0
        Dim x_Cod_Conto As Integer = 0
        Dim x_Conto As String = ""
        Dim x_Contabilizzato As Integer = 0
        Dim x_Pendente As Integer = 0
        Dim x_ChkLayOut_Hide As Integer = 0
        Dim x_Tara As Decimal = 0
        Dim x_Extra_Str_Dettagli As String = ""
        Dim x_Extra_Int_Dettagli As Integer = 0
        Dim x_Extra_Date_Dettagli As Date = #1/1/1900#

        Dim x_Veg_Cod As Integer = 0
        Dim x_Cul_Cod As Integer = 0

        Dim x_N_Conf_Riscontrate As Integer = -1
        Dim x_N_Colli_Riscontrati As Integer = -1
        Dim x_N_Imballi_Riscontrati As Integer = -1
        Dim x_Peso_Netto_Riscontrato As Decimal = 0
        Dim x_Peso_Lordo_Riscontrato As Decimal = 0
        Dim x_Tara_Unit_Collo_Riscontrata As Decimal = -1
        Dim x_Tara_Unit_Imballo_Riscontrata As Decimal = -1


        '--------------------------------
        'Riferimenti Documento Allegato
        Dim Riferimento_DocAllegato As String = ""
        Dim Rif_Ordine = ""
        'Dim Riferimento_DocAllegato_Prec As String = ""

        '------------------------------
        'Dati 
        Dim Riga_Sconto As String = ""
        Dim Riga_Importo As String = ""
        Dim Riga_Sconto_Omaggio As String = ""
        Dim Importo_Riga_DDT As String = ""
        Dim Importo_Dettaglio As Decimal = 0
        'Dim Riga_Qta As String
        'Dim Riga_Prezzo As String
        'Dim Riga_Iva As String
        'Dim Tot_Imp, Tot_ImpNetto As Decimal
        'Dim Tot_IVA As Decimal
        'Dim Tot_Sconto As Decimal

        Dim Flag_Campioni_Omaggio As Boolean = False
        Dim Flag_Campioni_Omaggio_Rivalsa_Iva As Boolean = False
        Dim Flag_Campioni_Gratuiti As Boolean = False
        Dim Flag_Sconto_Merce As Boolean = False

        Dim Flag_AlmenoUnoProdottoAgroAlimentare As Boolean = False
        Dim Flag_ProdottoAgroAlimentare As Boolean = False
        Dim Flag_ClientePrivato As Boolean = False
        Dim Flag_StampaRiepilogoImballi As Boolean = False

        '------------------------------
        ''Report IVA
        'Dim hash_iva As New Hashtable
        'Dim temp_imp, temp_iva, temp_tot As Decimal
        'Dim chiave_iva, valore_iva, old_valore, new_valore As String

        '------------------------------

        Dim Str_NoteIntegrative1 As String = ""
        Dim Str_NoteIntegrative2 As String = ""
        Dim Str_NoteIntegrative3 As String = ""
        Dim Str_Articolo62 As String = ""
        Dim impArticolo62ElemCod As Boolean = False
        Dim stampaArticolo62AltreMaterie As Boolean = False
        Dim Flag_contributoCONAI As Boolean = False
        Dim Flag_StampaRifOrdine As Boolean = False
        Dim Flag_GradoAlcolico As Boolean = False
        Dim Flag_GestMaterialeVivaistico As Boolean = False
        Dim Flag_CentroAziendalePartenza As Boolean = False

        Dim numColli_contenitori As Integer = 0
        Dim contenitore_cod As Integer = 0
        Dim descr_contenitore As String = ""
        Dim numContenitori_imballi As Integer = 0
        Dim imballaggio_cod As Integer = 0
        Dim descr_imballo As String = ""
        Dim Flag_DDTallegati As Boolean = False
        Dim Flag_ORDAllegati As Boolean = False
        Dim ChkConfezione As Integer = 0
        Dim ChkContenitore As Integer = 0
        Dim ChkImballaggio As Integer = 0

        '------------------------------
        Dim x_Cod_Articolo As String = ""
        Dim x_Descr_Prodotto_breve As String = ""
        Dim x_Prezzo_Livello As Integer = 0
        '------------------------------
        Dim Ultimo_DocRiferimento As String = ""
        Dim Dettaglio_Evasione As String = ""
        Dim tipologiaRiga As enum_TipoRigaFattura = enum_TipoRigaFattura.NonSpecificato

        Try

            ds.Clear()
            ds.AcceptChanges()

            Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
            _pivaReale = objImp.Leggi_PivaReale(_piva, _objParametriServer)

            '############################################################################################
            '############################### INTESTAZIONE DELLA FATTURA #################################
            '############################################################################################

            'MODIFICATA GESTIONE INTESTAZIONE IN DATA 31/08/2015

            '########################## Impostazioni Utente ###########################################
            Try

                ImpostazioniUtente_StampaDoc(_objParametriUtenti,
                                             _logErrori,
                                             _lavCod,
                                             Str_NoteIntegrative1,
                                             Str_NoteIntegrative2,
                                             Str_NoteIntegrative3,
                                             Str_Articolo62,
                                             Flag_contributoCONAI,
                                             Flag_StampaRifOrdine,
                                             Flag_GradoAlcolico,
                                             Flag_GestMaterialeVivaistico,
                                             Flag_CentroAziendalePartenza)

                If Flag_contributoCONAI = True Then
                    If _CodiceLingua <> "IT" AndAlso _CodiceLingua <> "" Then
                        parametro.Conai = _objTradStampe.Fattura_Conai
                    Else
                        'Parametro_CONAI = "Contributo ambientale Conai assolto ove dovuto"
                        '29/02/2016 - Modificato label per Trombin
                        parametro.Conai = "CONTRIBUTO CONAI ASSOLTO OVE DOVUTO"
                    End If
                Else
                    parametro.Conai = ""
                End If

            Catch ex As Exception
                _logErrori &= "- Lettura impostazioni utente: " & vbCrLf & ex.Message & vbCrLf
            End Try


            Dim dtDocumento As DataTable
            '####################################################
            '               CREA INTESTAZIONE
            '####################################################

            drIntestazioneNew = CreaDrIntestazione(ds, dtDocumento, x_Edit_Importo,
                                                   x_ChkLayout_Join_Prodotti, x_ChkLayOut_Peso, x_ChkLayOut_Litri,
                                                   x_Cod_RisUm_Aggiuntivo,
                                                   Flag_ClientePrivato, Destinatario_Tel, parametro,
                                                   Riferimento_DocAllegato, Flag_StampaRiepilogoImballi)


            '####################################################

            ds.IntestazioneFattura.Rows.Add(drIntestazioneNew)

            '####################################################

            Dim objLanRound As New AgronicaCoreContabHLP.GiasLan_Round
            Dim Dt_Dettagli_Round As DataTable = Nothing
            Dim Dt_Riepilogo_IVA As DataTable = Nothing
            Dim Riepilogo_ImponibileLordo As Decimal = 0
            Dim Riepilogo_Variazioni As Decimal = 0
            Dim Riepilogo_ImponibileNetto As Decimal = 0
            Dim Riepilogo_Imposta As Decimal = 0
            Dim Riepilogo_Importo As Decimal = 0

            Try

                '====================================================================================
                '---------------- Prelevo le Informazioni sui dettagli ----------------------------

                Dim Flag_Raggruppa As Boolean
                Dim Hash_Gruppo As Hashtable = Nothing
                Dim Nome_Calibro As String = ""

                If _qsTipoView = "P" Then
                    x_ChkLayout_Join_Prodotti = 1
                End If

                If x_ChkLayout_Join_Prodotti = 1 Then
                    Flag_Raggruppa = True
                    Hash_Gruppo = New Hashtable
                Else
                    Flag_Raggruppa = False
                End If


                ImpostazioniImpresaUtente_Articolo62ElemCod(_objParametriServer, _objParametriUtenti, _logErrori, _piva, 0, impArticolo62ElemCod)

                '  Dt_Riepilogo_IVA = objLanRound.CaricaGriglia_DtIva()

                'mi tengo un certo margine per evitare che mi venga un contatore uguale
                Dim numContatoreBasePerRif As Integer = (dtDocumento.Rows.Count - 1) + 10

                For i = 0 To dtDocumento.Rows.Count - 1

                    Try 'dettaglio

                        Try 'leggi dettaglio

                            Leggi_MovimentoDettaglio_DocContabile(_objParametriServer,
                                                                  _objParametriUtenti,
                                                                  _moduliCliente,
                                                                  _logErrori,
                                                                  dtDocumento.Rows(i),
                                                                  _piva,
                                                                  _lavCod,
                                                                  Flag_Raggruppa,
                                                                  x_ChkLayOut_Peso,
                                                                  x_Mov_Det_Des,
                                                                  x_Elem_Cod,
                                                                  x_Pro_Cod,
                                                                  x_Mat_Cod,
                                                                  x_Cod_Progetto,
                                                                  x_Fase_Cod,
                                                                  x_Lotto,
                                                                  x_Cal_Cod,
                                                                  x_Udm_Cod,
                                                                  x_Udm_Sim,
                                                                  x_Udm_Des,
                                                                  x_Udm_Cod_Extra,
                                                                  x_Udm_Sim_Extra,
                                                                  x_Udm_Des_Extra,
                                                                  x_Qta,
                                                                  x_Qta_Extra,
                                                                  x_Qta_Extra_Totale,
                                                                  x_Descrizione,
                                                                  x_Prezzo_Unitario,
                                                                  x_Prezzo_Unitario_Netto,
                                                                  x_Imponibile,
                                                                  x_Imponibile_Netto,
                                                                  x_Cod_IVA,
                                                                  x_Aliquota_Des,
                                                                  x_IVA,
                                                                  x_ChkIVA_Manuale,
                                                                  x_Cod_IVAIndetraibile,
                                                                  x_Sconto_Perc,
                                                                  x_Sconto_Perc_2,
                                                                  x_Sconto_Testo,
                                                                  x_Sconto,
                                                                  x_Sconto_Modalita,
                                                                  x_Prezzo_Effettivo,
                                                                  x_Anno,
                                                                  x_Ric_Cod,
                                                                  x_Cod_Conto,
                                                                  x_Conto,
                                                                  x_Contabilizzato,
                                                                  x_Pendente,
                                                                  x_ChkLayOut_Hide,
                                                                  x_Tara,
                                                                  x_Extra_Str_Dettagli,
                                                                  x_Extra_Int_Dettagli,
                                                                  x_Extra_Date_Dettagli,
                                                                  x_Veg_Cod,
                                                                  x_Cul_Cod,
                                                                  Nome_Calibro,
                                                                  Riferimento_DocAllegato,
                                                                  Dt_Dettagli_Round,
                                                                  Flag_ProdottoAgroAlimentare,
                                                                  _flagStampaNumeroVasca,
                                                                  Flag_DDTallegati,
                                                                  Flag_ORDAllegati,
                                                                  numColli_contenitori,
                                                                  contenitore_cod,
                                                                  descr_contenitore,
                                                                  numContenitori_imballi,
                                                                  imballaggio_cod,
                                                                  descr_imballo,
                                                                  ChkConfezione,
                                                                  ChkContenitore,
                                                                  ChkImballaggio,
                                                                  x_Cod_Articolo,
                                                                  x_Descr_Prodotto_breve,
                                                                  x_Flag_Extra,
                                                                  x_OTabella_Cod_Base,
                                                                  x_Prezzo_Livello,
                                                                  _objConfigStampe,
                                                                  Dettaglio_Evasione,
                                                                  x_N_Conf_Riscontrate, x_N_Colli_Riscontrati, x_N_Imballi_Riscontrati,
                                                                  x_Peso_Netto_Riscontrato, x_Peso_Lordo_Riscontrato,
                                                                  x_Tara_Unit_Collo_Riscontrata, x_Tara_Unit_Imballo_Riscontrata,
                                                                  _qsTipoView,
                                                                  x_Aliquota_Val,
                                                                  Flag_UveDiraspate:=False,
                                                                  flagTareRiscontrateInDesc:=True,
                                                                  Rif_Ordine:=Rif_Ordine,
                                                                  Flag_GradoAlcolico:=Flag_GradoAlcolico)

                            '-------------------------------------------------
                            '------ Identifico il TIPO DI RIGA DETTAGLIO -----
                            '-------------------------------------------------
                            tipologiaRiga = GetTipologiaRiga(x_Elem_Cod, x_Mov_Det_Des, Flag_Raggruppa, ChkConfezione, ChkContenitore, ChkImballaggio, dtDocumento.Rows(i).Item("Ordine_Det"), _logErrori)


                            'se il prodotto è agroalimentare ed è il primo che trovo
                            '(metto il controllo almeno uno, così evito di sovrascriverlo nel caso in fattura ci siano anche articoli non agroalimentari)
                            If Flag_ProdottoAgroAlimentare = True AndAlso Flag_AlmenoUnoProdottoAgroAlimentare = False Then
                                Flag_AlmenoUnoProdottoAgroAlimentare = True
                            End If

                            If impArticolo62ElemCod = True Then
                                'Se l'impostazione è attiva significa che devo controllare che almeno uno dei dettagli del documento
                                'appartenga all'elem_cod che mi interessa e nel caso, mostrare la dicitura.
                                'A 01/2024 la categoria da controllare è solo ALTRE_MATERIE (200). Sviluppo fatto per Inalca
                                stampaArticolo62AltreMaterie = stampaArticolo62AltreMaterie OrElse x_Elem_Cod = ALTRE_MATERIE
                            End If

                            If Flag_ORDAllegati = True AndAlso Not Flag_StampaRifOrdine Then
                                Riferimento_DocAllegato = ""
                                Dettaglio_Evasione = ""
                            End If

                            '  Giulia, 15/01/2018 11:21:23: Dal castelletto iva escludo le voci di riepilogo degli scarichi imballi, visto che non sono valorizzati
                            If _moduloFreshFood = True AndAlso
                                ((ChkContenitore = 1 OrElse ChkImballaggio = 1) AndAlso
                                 ((x_Elem_Cod = BENI_CONFEZ_ANIMALE OrElse x_Elem_Cod = BENI_CONFEZ_VEGETALE) AndAlso x_Mov_Det_Des.StartsWith("Scarico"))) Then
                                'sono le voci di riepilogo imballi, quindi non le devo considerare per l'iva
                            Else

                                If _nuoviArrotondamenti = True Then
                                    Dt_Riepilogo_IVA = objLanRound.FormAggiornaImportoNEW(_objParametriServer,
                                                                                          Dt_Dettagli_Round,
                                                                                          Riepilogo_ImponibileLordo,
                                                                                          Riepilogo_Variazioni,
                                                                                          Riepilogo_ImponibileNetto,
                                                                                          Riepilogo_Imposta,
                                                                                          Riepilogo_Importo,
                                                                                          x_Edit_Importo, _
                                                                                          False, _
                                                                                          False, _
                                                                                          _EsigibilitaIva)
                                Else
                                    'ad ogni dettaglio ricalcolo gli importi e aggiorno il dtiva
                                    Dt_Riepilogo_IVA = objLanRound.FormAggiornaImporto(_objParametriServer,
                                                                                       Dt_Dettagli_Round,
                                                                                       Riepilogo_ImponibileLordo,
                                                                                       Riepilogo_Variazioni,
                                                                                       Riepilogo_ImponibileNetto,
                                                                                       Riepilogo_Imposta,
                                                                                       Riepilogo_Importo,
                                                                                       x_Edit_Importo)
                                End If

                            End If

                            If tipologiaRiga = enum_TipoRigaFattura.DescrizioneLibera Then
                                Importo_Dettaglio = 0
                            Else
                                Importo_Dettaglio = Dt_Dettagli_Round.Select("Id_mov_det = " & Agro_SQL_SaveNum(dtDocumento.Rows(i).Item("id_mov_det")))(0).Item("importo_totale")
                            End If

                        Catch ex As Exception
                            _logErrori &= "- Lettura dei dettagli della Fattura: " & vbCrLf & ex.Message & vbCrLf
                        End Try

                        'se x_ChkLayOut_Hide=1 non si vuole visualizzare il dettaglio
                        If x_ChkLayOut_Hide <> 1 Then

                            '======================================================================
                            '      Riferimento Doc Allegato
                            '======================================================================
                            AggiungiRiferimentoDocAllegato(_logErrori, ds, Flag_Raggruppa, Riferimento_DocAllegato,
                                                           i, Ultimo_DocRiferimento, numContatoreBasePerRif, _piva,
                                                           _objConfigStampe, Flag_StampaRiepilogoImballi, _moduloFreshFood, tipologiaRiga)

                            Select Case x_Sconto_Modalita

                                Case enModalitaSconto.Percentuale
                                    '   CASO NORMALE
                                    'Riga_Importo = Format(Arrotonda_2Decimali(x_Imponibile_Netto), "#,###,##0.00")
                                    'x_Imponibile_Netto già arrotondato in DocumentiContab.vb
                                    Riga_Sconto_Omaggio = ""
                                    Riga_Importo = Format(x_Imponibile_Netto, "#,###,##0.00")

                                    Riga_Sconto = Composizione_Stringa_Sconti(x_Sconto_Perc, x_Sconto_Perc_2, x_Sconto_Testo)

                                    'Tot_Imp += x_Imponibile
                                    'Tot_Imp = Arrotonda_2Decimali(Tot_Imp)

                                    'Tot_ImpNetto += x_Imponibile_Netto
                                    'Tot_ImpNetto = Arrotonda_2Decimali(Tot_ImpNetto)

                                    'Importo_Riga_DDT = Arrotonda_2Decimali(x_Imponibile_Netto + x_IVA)
                                    Importo_Riga_DDT = Format(Importo_Dettaglio, "#,###,##0.00##")

                                    '=============================================

                                Case enModalitaSconto.Sconto_Merce
                                    '   CASO SCONTO MERCE
                                    Riga_Sconto_Omaggio = "Sconto Merce"
                                    Riga_Importo = "Sconto Merce"
                                    Riga_Sconto = ""
                                    Flag_Sconto_Merce = True
                                    Importo_Riga_DDT = ""
                                    '=============================================

                                Case enModalitaSconto.Omaggio_SenzaRivalsaIva
                                    '   CASO CAMPIONI OMAGGIO
                                    Riga_Sconto_Omaggio = "Campione Omaggio senza rivalsa IVA"
                                    Riga_Importo = "Omaggio senza rivalsa IVA"
                                    Riga_Sconto = ""
                                    Flag_Campioni_Omaggio = True

                                    'Tot_Imp += x_Imponibile
                                    'Tot_Imp = Arrotonda_2Decimali(Tot_Imp)

                                    'Tot_ImpNetto += x_Imponibile_Netto
                                    'Tot_ImpNetto = Arrotonda_2Decimali(Tot_ImpNetto)

                                    'Importo_Riga_DDT = Arrotonda_2Decimali(x_Imponibile_Netto + x_IVA)
                                    Importo_Riga_DDT = Format(Importo_Dettaglio, "#,###,##0.00##")

                                    '=============================================

                                Case enModalitaSconto.Omaggio_ConRivalsaIva
                                    '   CASO CAMPIONI OMAGGIO CON RIVALSA DELL'IVA
                                    Riga_Sconto_Omaggio = "Campione Omaggio con rivalsa IVA"
                                    Riga_Importo = "Omaggio con rivalsa IVA"
                                    Riga_Sconto = ""
                                    Flag_Campioni_Omaggio_Rivalsa_Iva = True

                                    Importo_Riga_DDT = Format(Importo_Dettaglio, "#,###,##0.00##")

                                    '=============================================

                                Case enModalitaSconto.Campioni_Gratuiti
                                    '   CASO CAMPIONI GRATUITI
                                    'non esiste x fattura, ma solo per ddt
                                    'modifica del 06/11/2012
                                    'gestisco il caso di stampa di fattura agganciata a ddt con campioni gratuiti
                                    Riga_Sconto_Omaggio = "Campione Gratuito"
                                    Riga_Importo = "Gratuito"
                                    Riga_Sconto = ""
                                    Flag_Campioni_Gratuiti = True
                                    '=============================================

                            End Select


                            '======================================================================
                            '      Personalizzazioni Doc Contabili Riga
                            '======================================================================

                            Try
                                '  Giulia, 15/11/2016 16.29.10: in alcuni layout (trombin, sbtf, ...) la colonna dell'unità di misura è troppo stretta,
                                '               quindi vengono sovrascritte alcune descrizioni di unità di misura
                                Select Case _progressivoGias
                                    Case enum_CodiceGIAS_Clienti.Trombin,
                                        enum_CodiceGIAS_Clienti.SBTF,
                                        enum_CodiceGIAS_Clienti.CoFruTa

                                        If x_Udm_Cod = enum_UnitaMisura.Unita_Seme Then
                                            x_Udm_Sim = "U di Seme"
                                        End If
                                        If x_Udm_Cod_Extra = enum_UnitaMisura.Unita_Seme Then
                                            x_Udm_Sim_Extra = "U di Seme"
                                        End If
                                End Select


                            Catch ex As Exception
                                _logErrori &= "- Personalizzazioni Doc Contabili Riga: " & vbCrLf & ex.Message & vbCrLf
                            End Try

                            '===================================================================

                            'il dettaglio è visibile

                            'se si vuole visualizzare i dettagli così come sono
                            If Flag_Raggruppa = False Then

                                '------------------------------------
                                '------ VISUALIZZAZIONE NORMALE -----
                                '------------------------------------

                                '  Giulia, 08/11/2016 11.47.22: devo stampare anche i litri (qta_extra_totale)
                                If x_ChkLayOut_Litri = 1 Then
                                    Litri_Totali += ConteggioLitriDettaglio(x_Descrizione, x_Udm_Cod, x_Udm_Cod_Extra, x_Qta, x_Qta_Extra_Totale)
                                End If


                                '28/05/2019: aggiunto (era solo nella stampa del DDT prima)
                                If Riga_Sconto_Omaggio <> "" Then
                                    x_Descrizione &= "<br>" & Riga_Sconto_Omaggio
                                End If

                                If Dettaglio_Evasione <> "" Then
                                    x_Descrizione &= " " & Dettaglio_Evasione
                                End If

                                drDescrizioneNew = ds.Descrizione.NewDescrizioneRow

                                If tipologiaRiga = enum_TipoRigaFattura.DescrizioneLibera Then
                                    'riga testo libero
                                    drDescrizioneNew.Udm_Des = ""
                                    drDescrizioneNew.Qta = ""
                                    drDescrizioneNew.Prezzo = ""
                                    drDescrizioneNew.Sconto = ""
                                    drDescrizioneNew.Iva = ""
                                    drDescrizioneNew.IvaImposta = ""
                                    drDescrizioneNew.Importo = ""
                                    drDescrizioneNew.Extra_Str_1 = ""
                                    drDescrizioneNew.Extra_Str_2 = ""
                                    drDescrizioneNew.Extra_Str_3 = ""
                                    drDescrizioneNew.Extra_Str_4 = ""
                                    drDescrizioneNew.Extra_Str_5 = ""
                                Else
                                    '  Giulia, 23/01/2017 12:38:09: Mancava gestione particolare per FF (ripresa da DDT)

                                    '-------------------------------
                                    '---- FRESH & FOOD -------------
                                    '-------------------------------
                                    If _moduloFreshFood = True Then
                                        Dim Prezzo_Unitario_REALE As Decimal = 0
                                        Dim Prezzo_Unitario_Netto_REALE As Decimal = 0


                                        Select Case tipologiaRiga

                                            Case enum_TipoRigaFattura.RiepilogoConfezioni,
                                                enum_TipoRigaFattura.RiepilogoContenitori,
                                                enum_TipoRigaFattura.RiepilogoImballaggi

                                                'per gli imballaggi
                                                'riepilogo contenitori e imballaggi
                                                drDescrizioneNew.Udm_Des = x_Udm_Sim
                                                drDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")
                                                'per gli  imballaggi non visualizzo dettagli economici
                                                drDescrizioneNew.Prezzo = ""
                                                drDescrizioneNew.Prezzo_Netto = ""
                                                drDescrizioneNew.Sconto = ""
                                                drDescrizioneNew.Iva = ""
                                                drDescrizioneNew.IvaImposta = ""
                                                drDescrizioneNew.Importo = ""
                                                drDescrizioneNew.Extra_Str_1 = ""
                                                drDescrizioneNew.Extra_Str_2 = ""
                                                drDescrizioneNew.Extra_Str_3 = ""
                                                drDescrizioneNew.Extra_Str_4 = ""

                                            Case Else

                                                'per tutti i prodotti tranne gli imballaggi

                                                '======================================================================
                                                '      Impostazioni Prezzi e Qta per FF
                                                '======================================================================
                                                If _nuoviArrotondamenti = True Then
                                                    CalcolaPrezziFFLivello_NEW(x_Prezzo_Livello, drDescrizioneNew, Prezzo_Unitario_REALE, Prezzo_Unitario_Netto_REALE,
                                                                               x_Qta, x_Qta_Extra, x_Prezzo_Unitario, x_Prezzo_Unitario_Netto, x_Prezzo_Effettivo,
                                                                               x_Imponibile, x_Imponibile_Netto, x_Udm_Sim, x_Udm_Sim_Extra, x_Flag_Extra,
                                                                               numColli_contenitori, numContenitori_imballi,
                                                                               _tipoArrotondamentoFF, flagPrezzoAlKgUdmKg:=True)
                                                Else
                                                    CalcolaPrezziFFLivello(x_Prezzo_Livello, drDescrizioneNew, Prezzo_Unitario_REALE, Prezzo_Unitario_Netto_REALE,
                                                                           x_Qta, x_Qta_Extra, x_Prezzo_Unitario, x_Prezzo_Unitario_Netto, x_Prezzo_Effettivo,
                                                                           x_Imponibile, x_Imponibile_Netto, x_Udm_Sim, x_Udm_Sim_Extra, x_Flag_Extra,
                                                                           numColli_contenitori, numContenitori_imballi)
                                                End If

                                                drDescrizioneNew.Prezzo = Format(Prezzo_Unitario_REALE, "#,###,##0.00##")
                                                drDescrizioneNew.Prezzo_Netto = Format(Prezzo_Unitario_Netto_REALE, "#,###,##0.00##")

                                                'DrDescrizioneNew.Importo = Format(x_Imponibile_Netto, "#,###,##0.00")
                                                drDescrizioneNew.Importo = Riga_Importo
                                                drDescrizioneNew.Sconto = Riga_Sconto

                                                '31/05/17: legenda degli art escl iva
                                                If x_Aliquota_Val <> 0 Then
                                                    drDescrizioneNew.Iva = x_Aliquota_Des
                                                Else
                                                    drDescrizioneNew.Iva = "#" & CStr(x_Cod_IVA)
                                                End If

                                                drDescrizioneNew.IvaImposta = CStr(x_IVA)
                                                drDescrizioneNew.Extra_Str_1 = Importo_Riga_DDT
                                                'DrDescrizioneNew.Extra_Str_1 = Format(x_Qta_Extra_Totale, "#,###,##0.00##")
                                                drDescrizioneNew.Extra_Str_2 = Format(x_Tara, "#,###,##0.00##")
                                                'DrDescrizioneNew.Extra_Str_3 = Format(Peso_Lordo_Dettaglio, "#,###,##0.00##")

                                        End Select

                                    Else   ' NO FRESH & FOOD

                                        '-------------------------------------------------------
                                        '---- CANTINE, ZOO, OLIO, TABACCO, NO OMNI -------------
                                        '-------------------------------------------------------

                                        If x_Flag_Extra = 1 Then
                                            'anagrafica con impostazione udm aspetto default
                                            'DrDescrizioneNew.Udm_Des = x_Udm_Sim_Extra
                                            drDescrizioneNew.Udm_Des = IIf(x_Udm_Sim_Extra = "", x_Udm_Sim, x_Udm_Sim_Extra)
                                            drDescrizioneNew.Qta = Format((x_Qta * x_Qta_Extra), "#,###,##0.###")
                                            drDescrizioneNew.Prezzo = Format(x_Prezzo_Effettivo, "#,###,##0.00##")
                                        Else
                                            drDescrizioneNew.Udm_Des = x_Udm_Sim
                                            drDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.###")
                                            drDescrizioneNew.Prezzo = Format(x_Prezzo_Unitario, "#,###,##0.00##")
                                        End If

                                        Select Case tipologiaRiga

                                            Case enum_TipoRigaFattura.RiepilogoConfezioni,
                                                enum_TipoRigaFattura.RiepilogoContenitori,
                                                enum_TipoRigaFattura.RiepilogoImballaggi

                                                'per gli imballaggi
                                                drDescrizioneNew.Prezzo = ""
                                                drDescrizioneNew.Importo = ""
                                                drDescrizioneNew.Sconto = ""
                                                drDescrizioneNew.Iva = ""
                                                drDescrizioneNew.Extra_Str_1 = ""
                                                drDescrizioneNew.Extra_Str_2 = ""
                                                drDescrizioneNew.Extra_Str_3 = ""
                                                drDescrizioneNew.Extra_Str_4 = ""

                                            Case Else

                                                'per tutti i prodotti tranne gli imballaggi
                                                'DrDescrizioneNew.Importo = Format(x_Imponibile_Netto, "#,###,##0.00")
                                                drDescrizioneNew.Importo = Riga_Importo
                                                drDescrizioneNew.Sconto = Riga_Sconto

                                                '31/05/17: legenda degli art escl iva
                                                If x_Aliquota_Val <> 0 Then
                                                    drDescrizioneNew.Iva = x_Aliquota_Des
                                                Else
                                                    drDescrizioneNew.Iva = "#" & CStr(x_Cod_IVA)
                                                End If

                                                drDescrizioneNew.IvaImposta = CStr(x_IVA)
                                                'DrDescrizioneNew.Extra_Str_1 = Format(x_Qta_Extra_Totale, "#,###,##0.00##")
                                                drDescrizioneNew.Extra_Str_1 = Importo_Riga_DDT
                                                drDescrizioneNew.Extra_Str_2 = Format(x_Tara, "#,###,##0.00##")
                                                'DrDescrizioneNew.Extra_Str_3 = Format(Peso_Lordo_Dettaglio, "#,###,##0.00##")
                                                drDescrizioneNew.Extra_Str_4 = Format(x_Prezzo_Effettivo, "#,###,##0.00##")

                                        End Select

                                    End If 'FRESH & FOOD

                                End If 'riga descrizione libera

                                drDescrizioneNew.Contatore = i
                                drDescrizioneNew.SuperPiva = _pivaReale

                                Select Case _progressivoGias
                                    Case enum_CodiceGIAS_Clienti.Trombin
                                        '  Giulia, 06/10/2016 15.23.26: aggiunta per dettagli ulteriori
                                        'DrDescrizioneNew.Descrizione = x_Descr_Prodotto_breve
                                        drDescrizioneNew.Descrizione = x_Descr_Prodotto_breve & Replace(x_Extra_Str_Dettagli, "§", "<br>")
                                    Case Else
                                        drDescrizioneNew.Descrizione = x_Descrizione
                                End Select

                                '-----------
                                '26/02/2016
                                'report personalizzato Trombin
                                drDescrizioneNew.Cod_Articolo = x_Cod_Articolo
                                '-----------

                                If _moduloFreshFood = True Then

                                    'Flag_StampaRiepilogoImballi = False quando non serve stampare il riepilogo imballi
                                    '(ha senso solo in DDT e Fattura Accompagnatoria che viaggiano insieme alla merce)

                                    Select Case tipologiaRiga

                                        Case enum_TipoRigaFattura.RiepilogoConfezioni
                                            'Se è voce di riepilogo confezioni ed ho scelto di visualizzarli
                                            If _objConfigStampe.Flag_RiepilogoConfezioni = True AndAlso Flag_StampaRiepilogoImballi = True Then
                                                ds.Descrizione.Rows.Add(drDescrizioneNew)
                                            End If

                                        Case enum_TipoRigaFattura.RiepilogoContenitori
                                            'Se è voce di riepilogo contenitori ed ho scelto di visualizzarli
                                            If _objConfigStampe.Flag_RiepilogoContenitori = True AndAlso Flag_StampaRiepilogoImballi = True Then
                                                ds.Descrizione.Rows.Add(drDescrizioneNew)
                                            End If

                                        Case enum_TipoRigaFattura.RiepilogoImballaggi
                                            'Se è voce di riepilogo imballaggi ed ho scelto di visualizzarli
                                            If _objConfigStampe.Flag_RiepilogoImballi = True AndAlso Flag_StampaRiepilogoImballi = True Then
                                                ds.Descrizione.Rows.Add(drDescrizioneNew)
                                            End If

                                        Case Else
                                            'stampo sempre il dettaglio
                                            ds.Descrizione.Rows.Add(drDescrizioneNew)
                                    End Select

                                    ' If _moduloFreshFood = True AndAlso
                                    '((ChkContenitore = 1 Or ChkImballaggio = 1) And
                                    ' ((x_Elem_Cod = BENI_CONFEZ_ANIMALE Or x_Elem_Cod = BENI_CONFEZ_VEGETALE) AndAlso x_Mov_Det_Des.StartsWith("Scarico"))) AndAlso
                                    'Flag_StampaRiepilogoImballi = False Then
                                    '     'è un FF, questo è imballo o contenitore, ma ho scelto di non stampare il riepilogo imballi 
                                    '     'perché ha senso solo in DDT e Fattura Accompagnatoria
                                Else
                                    ds.Descrizione.Rows.Add(drDescrizioneNew)
                                End If

                            Else 'bisogna raggruppare i dettagli

                                '------------------------------------
                                '------ RAGGRUPPAMENTO DETTAGLI -----
                                '------------------------------------
                                CreaDettaglioRaggruppato(Hash_Gruppo, i, Riferimento_DocAllegato, x_Mat_Cod, x_Udm_Cod, x_Udm_Sim, x_Udm_Sim_Extra,
                                                         x_Qta, x_Qta_Extra, x_Descrizione, x_Prezzo_Unitario, x_Imponibile_Netto, x_Cod_IVA, x_Aliquota_Des, x_IVA,
                                                         x_Sconto_Perc, x_Sconto_Perc_2, x_Sconto_Testo, x_Prezzo_Effettivo, x_Veg_Cod, x_Cul_Cod, Nome_Calibro,
                                                         x_Cod_Articolo, x_Flag_Extra, Importo_Dettaglio, Riga_Sconto)

                            End If

                        End If

                        ''===========================================================
                        ''--------------- Report IVA --------------------------------
                        '05/09/2012: nuova gestione iva = GiasLan

                        'chiave_iva = CStr(x_Cod_IVA) & "|" & x_Aliquota_Des



                        ''===================================================================

                    Catch ex As Exception
                        _logErrori &= "- Errore dettaglio " & CStr(i + 1) & ": " & vbCrLf & ex.Message & vbCrLf
                    End Try

                Next 'dettagli

                '===================================================================

                Try

                    'se impostato il raggruppamento
                    If Flag_Raggruppa = True Then
                        i = 0
                        AggiungiRigheDettaglioRaggruppate(ds, i, Hash_Gruppo)
                    End If

                Catch ex As Exception
                    _logErrori &= "- Gestione raggruppamento dettagli: " & vbCrLf & ex.Message & vbCrLf
                End Try


                '======================================================================
                '----- IMPOSTAZIONE NOTE AGGIUNTIVE E DICITURE SCONTI E OMAGGI --------
                '======================================================================
                AggiungiNoteAggiuntiveOmaggi(_logErrori, ds, i,
                                             Str_NoteIntegrative1, Str_NoteIntegrative2, Str_NoteIntegrative3,
                                             Flag_Campioni_Omaggio, Flag_Campioni_Omaggio_Rivalsa_Iva, Flag_Campioni_Gratuiti, Flag_Sconto_Merce)


            Catch ex As Exception
                _logErrori &= "- Visualizzazione globale dei dettagli della Fattura: " & vbCrLf & ex.Message & vbCrLf
            End Try


            '=============================================================
            '----- Assolve gli obblighi di cui all’articolo 62, comma 1,  del decreto legge 24 gennaio 2012,  n. 1, convertito, con modificazioni, dalla legge 24 marzo 2012, n. 27. ----------
            '----- Assolve gli obblighi di cui all'articolo 3 del decreto legislativo 08 novembre 2021, n. 198. ----------
            Try
                If (Flag_ClientePrivato = False AndAlso Flag_AlmenoUnoProdottoAgroAlimentare = True) OrElse stampaArticolo62AltreMaterie = True Then
                    _rptFattura.Section4.SectionFormat.EnableSuppress = False

                    If _CodiceLingua <> "" AndAlso _CodiceLingua <> "IT" Then
                        parametro.T_Articolo62 = _objTradStampe.Fattura_Articolo62
                    Else
                        If Str_Articolo62 <> "" Then
                            'CType(_rptFattura.Section4.ReportObjects("TxtArticolo62"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Str_Articolo62
                            parametro.T_Articolo62 = Str_Articolo62
                        End If
                    End If
                Else
                    parametro.T_Articolo62 = ""
                End If

            Catch ex As Exception
                _logErrori &= "- Articolo 62: " & vbCrLf & ex.Message & vbCrLf
            End Try


            '======================================================================
            '       Modifiche Layout Stampa Litri
            '======================================================================
            ModificaLayoutStampaLitri(x_ChkLayOut_Litri, Litri_Totali, parametro.Litri)


            '======================================================================
            '       Modifiche Layout Cessionario Aggiuntivo
            '======================================================================
            ModificaLayoutCessionarioAggiuntivo(x_Cod_RisUm_Aggiuntivo)


            '=============================================================
            '---------- RIEPILOGO DOCUMENTO ----------

            'IMPONIBILE LORDO
            'DrIntestazioneNew.Imponibile = Format(Arrotonda_2Decimali(Tot_Imp), "#,###,##0.00")
            drIntestazioneNew.Imponibile = Format(Riepilogo_ImponibileLordo, "#,###,##0.00")

            'VARIAZIONI
            'Dim Tot_Sconto_xDifferenza As Decimal
            'Tot_Sconto_xDifferenza = Arrotonda_2Decimali(CDec(Arrotonda_2Decimali(Tot_Imp) - Arrotonda_2Decimali(Tot_ImpNetto)))
            'DrIntestazioneNew.Sconti = Format(Arrotonda_2Decimali(Tot_Sconto_xDifferenza), "#,###,##0.00")
            drIntestazioneNew.Sconti = Format(Riepilogo_Variazioni, "#,###,##0.00")

            'IMPONIBILE NETTO
            'riciclo il campo SuperFrz_Des, salvandoci il totale degli imponibili, visto che nel campo imponibile viene troncato l'ultimo decimale
            'DrIntestazioneNew.SuperFrz_Des = Format(Arrotonda_2Decimali(Tot_ImpNetto), "#,###,##0.00")
            drIntestazioneNew.SuperFrz_Des = Format(Riepilogo_ImponibileNetto, "#,###,##0.00")

            'IMPOSTA
            'Dim Tot_IVA_xDifferenza As Decimal
            'Tot_IVA_xDifferenza = Arrotonda_2Decimali(CDec(Arrotonda_2Decimali(x_Num_Protocollo) - Arrotonda_2Decimali(Tot_ImpNetto)))
            'DrIntestazioneNew.Iva = Format(Arrotonda_2Decimali(Tot_IVA_xDifferenza), "#,###,##0.00")
            drIntestazioneNew.Iva = Format(Riepilogo_Imposta, "#,###,##0.00")

            'TOTALE FATTURA
            'nel caso di campioni omaggio, l'intera fattura è di campioni omaggio e il totale fattura è 0 (ma imponibile e iva sono valorizzati)
            'per gli sconti merce, il totale è dato dai dettagli non in sconto merce
            'DrIntestazioneNew.Importo = Format(x_Num_Protocollo, "#,###,##0.00")
            ''riciclo il campo SuperInd_Des, salvandoci il totale della fattura, visto che nel campo importo viene troncato l'ultimo decimale
            'DrIntestazioneNew.SuperInd_Des = Format(x_Num_Protocollo, "#,###,##0.00")
            'modifica del 06/09/2012: per coerenza non viene più stampato num_protocollo, ma il risultato dei riepiloghi
            drIntestazioneNew.Importo = Format(Riepilogo_Importo, "#,###,##0.00")
            drIntestazioneNew.SuperInd_Des = Format(Riepilogo_Importo, "#,###,##0.00")

            parametro.TotaleFattura = Format(CDec(Riepilogo_ImponibileNetto + Riepilogo_Imposta), "#,###,##0.00")

            'If Riepilogo_Importo <> x_Num_Protocollo Then
            '    _logErrori &= "Log debug: Riepilogo_Importo = " & CStr(Riepilogo_Importo) & " <> x_Num_Protocollo = " & CStr(x_Num_Protocollo) & " " & vbCrLf & vbCrLf
            'End If

            '=============================================================
            '      REPORT IVA
            '=============================================================
            'modifica del 05/09/2012: stessa gestione dell'iva del GiasLan

            Try
                If Not IsNothing(Dt_Riepilogo_IVA) Then
                    Dim drIva As DS_Iva.DS_IvaRow
                    For Each drRiepilogo As DataRow In Dt_Riepilogo_IVA.Rows
                        'nuova riga
                        drIva = dsIva.DS_Iva.NewDS_IvaRow

                        drIva.Cod_Iva = drRiepilogo.Item("cod_iva") 'cod_iva

                        '31/05/17: introdotta legenda per gli articoli esclusione iva e non imp iva
                        If drRiepilogo.Item("aliquota_iva") <> 0 Then
                            drIva.Cod_Iva_2 = drRiepilogo.Item("aliquota_des") 'aliquota
                        Else
                            'articoli esclusione
                            drIva.Cod_Iva_2 = "#" & CStr(drRiepilogo.Item("cod_iva")) & " - " & drRiepilogo.Item("aliquota_des")
                        End If

                        drIva.Imponibile = Format(drRiepilogo.Item("imponibile_netto"), "#,###,##0.00")

                        drIva.Imponibile2 = Format(drRiepilogo.Item("imponibile_netto"), "#,###,##0.00")

                        drIva.Imposta = Format(drRiepilogo.Item("iva"), "#,###,##0.00")

                        drIva.Imposta2 = Format(drRiepilogo.Item("iva"), "#,###,##0.00")

                        'aggiungo riga
                        dsIva.DS_Iva.Rows.Add(drIva)
                    Next
                Else
                    _logErrori &= "- Dt_Riepilogo_IVA nothing! " & vbCrLf & vbCrLf
                End If

            Catch ex As Exception
                _logErrori &= "- Riepilogo IVA: " & vbCrLf & ex.Message & vbCrLf
            End Try

            '====================================================================================

        Catch ex As Exception
            _logErrori &= "- Visualizzazione dei riepiloghi della Fattura: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try

            'imposto il DataSet sul report
            _rptFattura.SetDataSource(ds)

            'imposto il DataSet sul report
            _rptIva.SetDataSource(dsIva)

        Catch ex As Exception
            _logErrori &= "- Aggancio DataSet: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try

            _rptFattura.OpenSubreport("Iva.rpt").SetDataSource(dsIva)

        Catch exc As Exception
            _logErrori &= "- OpenSubReport: " & vbCrLf & exc.Message & vbCrLf
        End Try

        '=============================================================
        '      Impostazione Parametri
        '=============================================================
        ImpostaParametri(Destinatario_Tel, parametro)

    End Sub

    Private Sub ImpostaParametri(ByVal Destinatario_Tel As String, ByRef parametro As Parametri)

        Try
            _rptFattura.SetParameterValue("Totale_Fattura", parametro.TotaleFattura)
            _rptFattura.SetParameterValue("Contributo_CONAI", parametro.Conai)
            _rptFattura.SetParameterValue("Titolo_Documento", parametro.TitoloDocumento)
            _rptFattura.SetParameterValue("Riferimento_Documento", parametro.RiferimentoDocumento)

            _rptFattura.SetParameterValue("Intestazione_Riga1_RagSocPiccola", parametro.IntestazioneRiga1)
            _rptFattura.SetParameterValue("Intestazione_Riga2", parametro.IntestazioneRiga2)
            _rptFattura.SetParameterValue("Intestazione_Riga3", parametro.IntestazioneRiga3)
            _rptFattura.SetParameterValue("Intestazione_Riga4", parametro.IntestazioneRiga4)
            _rptFattura.SetParameterValue("Intestazione_Riga5", parametro.IntestazioneRiga5)
            _rptFattura.SetParameterValue("Intestazione_Riga6", parametro.IntestazioneRiga6)
            _rptFattura.SetParameterValue("Intestazione_Riga7", parametro.IntestazioneRiga7)
            _rptFattura.SetParameterValue("Intestazione_Riga8", parametro.IntestazioneRiga8)
            _rptFattura.SetParameterValue("Intestazione_Riga9", parametro.IntestazioneRiga9)
            _rptFattura.SetParameterValue("Intestazione_Riga10", parametro.IntestazioneRiga10)
            _rptFattura.SetParameterValue("Intestazione_Riga11", parametro.IntestazioneRiga11)
            _rptFattura.SetParameterValue("Intestazione_Riga12", parametro.IntestazioneRiga12)
            _rptFattura.SetParameterValue("Intestazione_Riga13", parametro.IntestazioneRiga13)
            _rptFattura.SetParameterValue("Intestazione_Riga14", parametro.IntestazioneRiga14)
            _rptFattura.SetParameterValue("Intestazione_Riga15", parametro.IntestazioneRiga15)

            _rptFattura.SetParameterValue("LblPivaCliente", parametro.LblPivaCliente)
            _rptFattura.SetParameterValue("LblCFCliente", parametro.LblCfCliente)
            _rptFattura.SetParameterValue("TxtPivaCliente", parametro.TxtPivaCliente)
            _rptFattura.SetParameterValue("TxtCFCliente", parametro.TxtCfCliente)
            _rptFattura.SetParameterValue("LblScadenza", parametro.LblScadenza)

            _rptFattura.SetParameterValue("Destinazione_Tel", Destinatario_Tel)

            'PARAMETRI SUL SOTTOREPORT DELL'IVA
            _rptFattura.SetParameterValue("T_IVA_imponibile", parametro.T_IVA_imponibile, "Iva.rpt")
            _rptFattura.SetParameterValue("T_IVA_aliquota", parametro.T_IVA_aliquota, "Iva.rpt")
            _rptFattura.SetParameterValue("T_IVA_imposta", parametro.T_IVA_imposta, "Iva.rpt")


            'PARAMETRI OPZIONALI
            For Each param In _rptFattura.DataDefinition.ParameterFields
                'i parametri potrebbero non esistere in alcuni report
                Select Case param.Name
                    Case "Litri_Tot" 'check if parameter exists in report
                        _rptFattura.SetParameterValue("Litri_Tot", parametro.Litri) ' set the parameter value in the report
                    Case "Agente"
                        If parametro.AgenteInfo <> "" Then
                            parametro.AgenteInfo = "Agente: " & parametro.AgenteInfo
                        End If
                        _rptFattura.SetParameterValue("Agente", parametro.AgenteInfo) ' set the parameter value in the report
                    Case "Desc_Aggiuntivo"
                        _rptFattura.SetParameterValue("Desc_Aggiuntivo", parametro.DescAggiuntivo)
                    Case "Rag_Soc_Aggiuntivo"
                        _rptFattura.SetParameterValue("Rag_Soc_Aggiuntivo", parametro.RagSocAggiuntivo)
                    Case "Indirizzo_Aggiuntivo"
                        _rptFattura.SetParameterValue("Indirizzo_Aggiuntivo", parametro.IndirizzoAggiuntivo)
                    Case "CAP_Aggiuntivo"
                        _rptFattura.SetParameterValue("CAP_Aggiuntivo", parametro.CapAggiuntivo)
                    Case "Frazione_Aggiuntivo"
                        _rptFattura.SetParameterValue("Frazione_Aggiuntivo", parametro.FrazioneAggiuntivo)
                    Case "Comune_Aggiuntivo"
                        _rptFattura.SetParameterValue("Comune_Aggiuntivo", parametro.ComuneAggiuntivo)
                    Case "Prov_Aggiuntivo"
                        _rptFattura.SetParameterValue("Prov_Aggiuntivo", parametro.ProvAggiuntivo)
                    Case "TxtPivaAgg"
                        _rptFattura.SetParameterValue("TxtPivaAgg", parametro.TxtPivaAgg)
                    Case "TxtCFAgg"
                        _rptFattura.SetParameterValue("TxtCFAgg", parametro.TxtCfAgg)

                    Case "T_FatturatoA"
                        _rptFattura.SetParameterValue("T_FatturatoA", parametro.T_FatturatoA)
                    Case "T_Destinatario"
                        _rptFattura.SetParameterValue("T_Destinatario", parametro.T_Destinatario)
                    Case "T_DataEmissione"
                        _rptFattura.SetParameterValue("T_DataEmissione", parametro.T_DataEmissione)
                    Case "T_Numero"
                        _rptFattura.SetParameterValue("T_Numero", parametro.T_Numero)
                    Case "T_Valuta"
                        _rptFattura.SetParameterValue("T_Valuta", parametro.T_Valuta)
                    Case "T_Pagina"
                        _rptFattura.SetParameterValue("T_Pagina", parametro.T_Pagina)
                    Case "T_ModalitaPagamento"
                        _rptFattura.SetParameterValue("T_ModalitaPagamento", parametro.T_ModalitaPagamento)
                    Case "T_Note"
                        _rptFattura.SetParameterValue("T_Note", parametro.T_Note)
                    Case "T_DataSpedizione"
                        _rptFattura.SetParameterValue("T_DataSpedizione", parametro.T_DataSpedizione)
                    Case "T_Colli"
                        _rptFattura.SetParameterValue("T_Colli", parametro.T_Colli)
                    Case "T_Peso"
                        _rptFattura.SetParameterValue("T_Peso", parametro.T_Peso)
                    Case "T_Aspetto"
                        _rptFattura.SetParameterValue("T_Aspetto", parametro.T_Aspetto)
                    Case "T_Causale"
                        _rptFattura.SetParameterValue("T_Causale", parametro.T_Causale)
                    Case "T_Trasporto"
                        _rptFattura.SetParameterValue("T_Trasporto", parametro.T_Trasporto)
                    Case "T_GestioneVettore"
                        _rptFattura.SetParameterValue("T_GestioneVettore", parametro.T_GestioneVettore)
                    Case "T_Vettore"
                        _rptFattura.SetParameterValue("T_Vettore", parametro.T_Vettore)
                    Case "T_DataConsegna"
                        _rptFattura.SetParameterValue("T_DataConsegna", parametro.T_DataConsegna)
                    Case "T_FirmaDestinatario"
                        _rptFattura.SetParameterValue("T_FirmaDestinatario", parametro.T_FirmaDestinatario)
                    Case "T_FirmaConducente"
                        _rptFattura.SetParameterValue("T_FirmaConducente", parametro.T_FirmaConducente)
                    Case "T_DescrizioneBeni"
                        _rptFattura.SetParameterValue("T_DescrizioneBeni", parametro.T_DescrizioneBeni)
                    Case "T_UM"
                        _rptFattura.SetParameterValue("T_UM", parametro.T_UM)
                    Case "T_Qta"
                        _rptFattura.SetParameterValue("T_Qta", parametro.T_Qta)
                    Case "T_Prezzo"
                        _rptFattura.SetParameterValue("T_Prezzo", parametro.T_Prezzo)
                    Case "T_Sconto"
                        _rptFattura.SetParameterValue("T_Sconto", parametro.T_Sconto)
                    Case "T_Importo"
                        _rptFattura.SetParameterValue("T_Importo", parametro.T_Importo)
                    Case "T_AliqIVA"
                        _rptFattura.SetParameterValue("T_AliqIVA", parametro.T_AliqIVA)
                    Case "T_TotImplordo"
                        _rptFattura.SetParameterValue("T_TotImplordo", parametro.T_TotImplordo)
                    Case "T_TotSconti"
                        _rptFattura.SetParameterValue("T_TotSconti", parametro.T_TotSconti)
                    Case "T_TotImpNetto"
                        _rptFattura.SetParameterValue("T_TotImpNetto", parametro.T_TotImpNetto)
                    Case "T_TotImposta"
                        _rptFattura.SetParameterValue("T_TotImposta", parametro.T_TotImposta)
                    Case "T_Tot"
                        _rptFattura.SetParameterValue("T_Tot", parametro.T_Tot)
                    Case "T_TotDocumento"
                        _rptFattura.SetParameterValue("T_TotDocumento", parametro.T_TotDocumento)
                    Case "T_CalcoloImposta"
                        _rptFattura.SetParameterValue("T_CalcoloImposta", parametro.T_CalcoloImposta)
                    Case "T_IVA_imponibile"
                        _rptFattura.SetParameterValue("T_IVA_imponibile", parametro.T_IVA_imponibile)
                    Case "T_IVA_aliquota"
                        _rptFattura.SetParameterValue("T_IVA_aliquota", parametro.T_IVA_aliquota)
                    Case "T_IVA_imposta"
                        _rptFattura.SetParameterValue("T_IVA_imposta", parametro.T_IVA_imposta)
                    Case "T_Articolo62"
                        _rptFattura.SetParameterValue("T_Articolo62", parametro.T_Articolo62)
                    Case "T_Privacy"
                        _rptFattura.SetParameterValue("T_Privacy", parametro.T_Privacy)
                    Case "T_SEO"
                        _rptFattura.SetParameterValue("T_SEO", parametro.T_SEO)
                    Case "CodiceSDI"
                        _rptFattura.SetParameterValue("CodiceSDI", parametro.CodiceSDI)
                End Select

            Next


        Catch ex As Exception
            _logErrori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Sub CreaDettaglioRaggruppato(ByRef Hash_Gruppo As Hashtable,
                                         ByVal numRiga As Integer,
                                         ByVal Riferimento_DocAllegato As String,
                                         ByVal x_Mat_Cod As Integer,
                                         ByVal x_Udm_Cod As Integer,
                                         ByVal x_Udm_Sim As String,
                                         ByVal x_Udm_Sim_Extra As String,
                                         ByVal x_Qta As Decimal,
                                         ByVal x_Qta_Extra As Decimal,
                                         ByVal x_Descrizione As String,
                                         ByVal x_Prezzo_Unitario As Decimal,
                                         ByVal x_Imponibile_Netto As Decimal,
                                         ByVal x_Cod_IVA As Integer,
                                         ByVal x_Aliquota_Des As String,
                                         ByVal x_IVA As Decimal,
                                         ByVal x_Sconto_Perc As Decimal,
                                         ByVal x_Sconto_Perc_2 As Decimal,
                                         ByVal x_Sconto_Testo As String,
                                         ByVal x_Prezzo_Effettivo As Decimal,
                                         ByVal x_Veg_Cod As Integer,
                                         ByVal x_Cul_Cod As Integer,
                                         ByVal Nome_Calibro As String,
                                         ByVal x_Cod_Articolo As String,
                                         ByVal x_Flag_Extra As Integer,
                                         ByVal Importo_Dettaglio As Decimal,
                                         ByRef Riga_Sconto As String)

        'TODO: qui dentro fà (x_Qta * x_Qta_Extra)

        Dim chiave As String
        Dim valore As String
        Dim qtaGruppo As Decimal
        Dim importoGruppo As Decimal
        Dim importoRigaGruppo As Decimal
        Dim ivaGruppo As Decimal
        Dim riferimentoDocAllegatoPrec As String
        Dim codArticoloGruppo As String

        Try
            If _qsTipoView = "P" Then
                'raggruppo per x_Mat_Cod (PRODOTTO) - unità misura - prezzo - sconto - cod iva

                If x_Flag_Extra = 1 Then
                    chiave = CStr(x_Mat_Cod) & "|" & CStr(IIf(x_Udm_Sim_Extra = "", x_Udm_Sim, x_Udm_Sim_Extra)) & "|" & CStr(x_Prezzo_Effettivo) & "|" & CStr(Riga_Sconto) & "|" & CStr(x_Cod_IVA)
                Else
                    chiave = CStr(x_Mat_Cod) & "|" & CStr(x_Udm_Sim) & "|" & CStr(x_Prezzo_Unitario) & "|" & CStr(x_Sconto_Perc) & "|" & CStr(x_Cod_IVA)
                End If
            Else
                'raggruppo per specie- varietà - calibro (non posso usare il codice perché è un progressivo) - unità misura - prezzo - sconto - cod iva
                chiave = CStr(x_Veg_Cod) & "|" & CStr(x_Cul_Cod) & "|" & Nome_Calibro & "|" & CStr(x_Udm_Cod) & "|" & CStr(x_Prezzo_Unitario) & "|" & CStr(x_Sconto_Perc) & "|" & CStr(x_Cod_IVA)
            End If


            'se non è già presente
            If Not Hash_Gruppo.Contains(chiave) Then

                'valore composto da 11 campi (da 0 a 10)

                If _qsTipoView = "P" Then
                    'raggruppo per x_Mat_Cod (PRODOTTO) - unità misura - prezzo - sconto - cod iva

                    If x_Flag_Extra = 1 Then
                        'TODO: eliminare il calcolo (x_Qta * x_Qta_Extra)?
                        valore = x_Descrizione & "|" & IIf(x_Udm_Sim_Extra = "", x_Udm_Sim, x_Udm_Sim_Extra) & "|" & CStr(x_Qta * x_Qta_Extra) & "|" &
                                 CStr(x_Prezzo_Effettivo) & "|" & CStr(x_Sconto_Perc) & "|" &
                                 CStr(x_Aliquota_Des) & "|" & CStr(x_IVA) & "|" & CStr(x_Imponibile_Netto) &
                                 "|" & Riferimento_DocAllegato & "|" & CStr(Importo_Dettaglio) & "|" & CStr(x_Cod_Articolo)

                    Else
                        valore = x_Descrizione & "|" & x_Udm_Sim & "|" & CStr(x_Qta) & "|" &
                                 CStr(x_Prezzo_Unitario) & "|" & CStr(x_Sconto_Perc) & "|" &
                                 CStr(x_Aliquota_Des) & "|" & CStr(x_IVA) & "|" & CStr(x_Imponibile_Netto) &
                                 "|" & Riferimento_DocAllegato & "|" & CStr(Importo_Dettaglio) & "|" & CStr(x_Cod_Articolo)
                    End If
                Else
                    valore = x_Descrizione & "|" & x_Udm_Sim & "|" & CStr(x_Qta) & "|" &
                             CStr(x_Prezzo_Unitario) & "|" & CStr(x_Sconto_Perc) & "|" &
                             CStr(x_Aliquota_Des) & "|" & CStr(x_IVA) & "|" & CStr(x_Imponibile_Netto) &
                             "|" & Riferimento_DocAllegato & "|" & CStr(Importo_Dettaglio) & "|" & CStr(x_Cod_Articolo)
                End If

                'inserisco il dettaglio
                Hash_Gruppo.Add(chiave, valore)

            Else
                'il dettaglio è già presente
                'devo incrementare la quantità

                riferimentoDocAllegatoPrec = CStr(Hash_Gruppo.Item(chiave)).Split("|")(8)

                'prelevo la quantità del dettaglio al momento salvata
                qtaGruppo = CDec(CStr(Hash_Gruppo.Item(chiave)).Split("|")(2))

                'aggiungo la qta del dettaglio ripetuto
                If _qsTipoView = "P" Then
                    If x_Flag_Extra = 1 Then
                        'TODO: eliminare il calcolo (x_Qta * x_Qta_Extra)?
                        qtaGruppo += x_Qta * x_Qta_Extra
                    Else
                        qtaGruppo += x_Qta
                    End If
                Else
                    qtaGruppo += x_Qta
                End If

                'prelevo l'importo del dettaglio al momento salvato
                importoGruppo = Agro_Math.ArrotondaVal_2(CDec(CStr(Hash_Gruppo.Item(chiave)).Split("|")(7)))
                'aggiungo l'imponibile del dettaglio ripetuto
                importoGruppo += x_Imponibile_Netto
                importoGruppo = Agro_Math.ArrotondaVal_2(importoGruppo)

                'prelevo l'importo totale del dettaglio 
                importoRigaGruppo = Agro_Math.ArrotondaVal_2(CDec(CStr(Hash_Gruppo.Item(chiave)).Split("|")(9)))
                importoRigaGruppo += Importo_Dettaglio
                importoRigaGruppo = Agro_Math.ArrotondaVal_2(importoRigaGruppo)

                'questo tanto non serve
                ivaGruppo = CDec(CStr(Hash_Gruppo.Item(chiave)).Split("|")(6))
                ivaGruppo += Agro_Math.ArrotondaVal_2(x_IVA)

                Riga_Sconto = Composizione_Stringa_Sconti(x_Sconto_Perc, x_Sconto_Perc_2, x_Sconto_Testo)

                'preparo il nuovo valore
                'valore composto da 11 campi (da 0 a 10)

                Dim udm As String
                Dim prezzo As Decimal
                Dim rifDoc As String

                If _qsTipoView = "P" Then
                    If x_Flag_Extra = 1 Then
                        udm = IIf(x_Udm_Sim_Extra = "", x_Udm_Sim, x_Udm_Sim_Extra)
                        prezzo = x_Prezzo_Effettivo
                    Else
                        udm = x_Udm_Sim
                        prezzo = x_Prezzo_Unitario
                    End If
                    rifDoc = Riferimento_DocAllegato
                Else
                    udm = x_Udm_Sim
                    prezzo = x_Prezzo_Unitario
                    rifDoc = riferimentoDocAllegatoPrec & Riferimento_DocAllegato
                End If

                codArticoloGruppo = CStr(Hash_Gruppo.Item(chiave)).Split("|")(10)
                '  Giulia, 15/11/2016 16.11.03: teoricamente non dovrebbe mai capitare che il codice articolo è diverso per lo stesso prodotto
                If codArticoloGruppo <> x_Cod_Articolo Then
                    _logErrori &= "- Errore dettaglio " & CStr(numRiga + 1) & ": " & vbCrLf & "Sono stati raggruppati articoli con codici articolo diverso" & vbCrLf
                    codArticoloGruppo = x_Cod_Articolo
                End If


                valore = x_Descrizione & "|" & udm & "|" & CStr(qtaGruppo) & "|" &
                         CStr(prezzo) & "|" & CStr(Riga_Sconto) & "|" &
                         CStr(x_Aliquota_Des) & "|" & CStr(ivaGruppo) & "|" & CStr(importoGruppo) &
                         "|" & rifDoc & "|" & CStr(importoRigaGruppo) & "|" & CStr(codArticoloGruppo)

                'aggiorno il valore
                Hash_Gruppo.Item(chiave) = valore

            End If

        Catch ex As Exception
            _logErrori &= "- Crea Dettaglio raggruppato: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Function CreaDrIntestazione(ByRef ds As DataSetFattura,
                                        ByRef dtDocumento As DataTable,
                                        ByRef x_Edit_Importo As enum_EditImporto,
                                        ByRef x_ChkLayout_Join_Prodotti As Integer,
                                        ByRef x_ChkLayOut_Peso As Integer,
                                        ByRef x_ChkLayOut_Litri As Integer,
                                        ByRef x_Cod_RisUm_Aggiuntivo As Integer,
                                        ByRef Flag_ClientePrivato As Boolean,
                                        ByRef Destinatario_Tel As String,
                                        ByRef parametro As Parametri,
                                        ByRef Riferimento_DocAllegato As String,
                                        ByRef Flag_StampaRiepilogoImballi As Boolean
                                        ) As DataSetFattura.IntestazioneFatturaRow

        Dim drIntestazioneNew As DataSetFattura.IntestazioneFatturaRow = ds.IntestazioneFattura.NewIntestazioneFatturaRow
        Dim msgErrore As String = ""

        '------------------
        'Impresa

        'Dim Indirizzo_Impresa1, Indirizzo_Impresa2 As String
        Dim x_RagSoc_Impresa As String = ""
        'Dim x_CodContatto_Impresa As String = ""
        Dim x_CodiceFiscale_Impresa As String = ""
        Dim x_IndDes_Impresa As String = ""
        Dim x_FrzDes_Impresa As String = ""
        Dim x_Cap_Impresa As String = ""
        Dim x_Comune_Impresa As String = ""
        Dim x_Provincia_Impresa As String = ""
        Dim x_Stato_Impresa As String = ""

        Dim Intestazione_Riga3 As String = ""
        Dim Intestazione_Riga4 As String = ""
        Dim Intestazione_Riga5 As String = ""
        Dim Intestazione_Riga6 As String = ""
        Dim Intestazione_Riga7 As String = ""
        Dim Intestazione_Riga8 As String = ""
        Dim Intestazione_Riga9 As String = ""
        Dim Intestazione_Riga10 As String = ""
        Dim Intestazione_Riga11 As String = ""
        Dim Intestazione_Riga12 As String = ""
        Dim Intestazione_Riga13 As String = ""
        Dim Intestazione_Riga14 As String = ""
        Dim Intestazione_Riga15 As String = ""


        '------------------
        'Movimenti
        Dim Note As String = ""
        Dim x_Id_Mov_Contabile As Integer = 0
        Dim x_Mov_Desc_Contabile As String = ""
        Dim x_Data_Movimento As Date = #1/1/1900#
        Dim x_Ora As String = Date.Now.ToString
        Dim x_Scadenza As Date = #12/31/2100#
        Dim x_Scadenza_Extra As Date = #12/31/2100#
        Dim x_Doc_Numero_Sin As String = ""
        Dim x_Doc_Numero As Integer = 0
        Dim x_Doc_Numero_Des As String = ""
        Dim x_Progr_Protocollo As Integer = 0
        Dim x_Progr_Registrazione As Integer = 0
        Dim x_Data_Registrazione As Date = #1/1/1900#
        Dim x_Num_Protocollo As Decimal = 0
        Dim x_Colli As Integer = 0
        Dim x_Peso As Decimal = 0
        Dim Peso_Lordo As Decimal = 0
        Dim x_Aspetto As String = ""
        Dim x_Causale_Trasporto As String = ""
        'Dim x_Tipo_Sconto As Integer = 0
        Dim x_Cod_RisUm As Integer = 0
        Dim x_Cod_Contatto As String = ""
        Dim x_Rag_Soc As String = ""
        Dim x_Codice_Fiscale As String = ""
        Dim x_ChkFittizio As Integer
        Dim x_Cod_IndirizzoRisUm As Integer = 0
        Dim x_Id_Cf_Cliente As Integer = 0
        Dim x_Cod_Destinazione As Integer = 0
        Dim x_CodContatto_Destinazione As String = ""
        Dim x_RagSoc_Destinazione As String = ""
        Dim x_CodiceFiscale_Destinazione As String = ""
        Dim x_chkfittizio_destinazione As Integer
        Dim x_Cod_IndirizzoDestinazione As Integer = 0
        Dim x_Id_Cf_Destinazione As Integer = 0

        Dim x_Cod_Indirizzo_Aggiuntivo As Integer = 0
        Dim x_Id_Cf_Aggiuntivo As Integer = 0
        Dim x_CodContatto_Aggiuntivo As String = ""
        Dim x_RagSoc_Aggiuntivo As String = ""
        Dim x_CodiceFiscale_Aggiuntivo As String = ""
        Dim x_ChkFittizio_Aggiuntivo As Integer

        Dim x_Mezzo As Integer = 0
        Dim x_Cod_Vettore As Integer = 0
        Dim x_Cod_IndirizzoVettore As Integer = 0
        Dim x_Id_Cf_Vettore As Integer = 0
        Dim x_CodContatto_Vettore As String = ""
        Dim x_RagSoc_Vettore As String = ""
        Dim x_CodiceFiscale_Vettore As String = ""
        Dim x_ChkFittizio_Vettore As Integer
        Dim x_NumReg_Vettore As String = ""
        Dim x_TargaMezzo_Vettore As String = ""
        Dim x_N_Autorizzazione_Trasporto As String = ""
        Dim x_Agente_Cod As Integer = 0
        Dim x_Natura_Beni As String = ""
        Dim x_Modalita As Integer = 0
        Dim x_Tara_Veicolo As Decimal = 0
        Dim x_Tara_Imballi As Decimal = 0
        Dim x_Tipo_Peso As Integer = 0
        Dim x_Username_Note As String = ""
        Dim x_Extra_Str As String = ""
        Dim x_Extra_Int As Integer = 0
        Dim x_Extra_Date As Date = #1/1/1900#

        Dim x_ChkLayout_Bypass_Fatturato As Integer = 0
        Dim x_ChkLayOut_Prezzo As Integer = 0
        Dim x_ChkLayOut_Riscontrato As Integer = 0

        Dim x_Gestione_Vettore As String = ""

        Dim Piva_Cliente As String
        Dim Codice_Fiscale_Cliente As String
        Dim x_Ind_Des As String = ""
        Dim x_Frz_Des As String = ""
        Dim x_Cap As String = ""
        Dim x_Comune As String = ""
        Dim x_Provincia As String = ""
        Dim x_Stato As String = ""
        'Dim Flag_StatoMembro As Boolean = True

        Dim Piva_Dest As String
        Dim Codice_Fiscale_Dest As String
        Dim x_IndDes_Destinazione As String = ""
        Dim x_FrzDes_Destinazione As String = ""
        Dim x_Cap_Destinazione As String = ""
        Dim x_Comune_Destinazione As String = ""
        Dim x_Provincia_Destinazione As String = ""
        Dim x_Stato_Destinazione As String = ""
        Dim x_Tel_Destinazione As String = ""
        Dim x_Cell_Destinazione As String = ""
        Dim x_IndirizzoTipoDesc_Destinazione As String = ""

        Dim Destinatario_RagSoc As String
        Dim Destinatario_Ind_Des As String = ""
        Dim Destinatario_Frz_Des As String = ""
        Dim Destinatario_Cap As String = ""
        Dim Destinatario_Comune As String = ""
        Dim Destinatario_Provincia As String = ""

        Dim x_N_Doc_Cliente As String
        Dim x_Data_Doc_Cliente As Date
        Dim x_N_Nota_Fattura As String
        Dim x_Data_Nota_Fattura As Date
        Dim x_N_Nota_DDT As String
        Dim x_N_Nota_Riga_DDT As String
        Dim x_Data_Nota_DDT As Date

        Dim stampaSDIAcquisto As Boolean = False
        Dim codiceSDIAcquisto As String = ""

        '############################################################################################
        '########################## Lettura della Fattura ###########################################
        '############################################################################################

        dtDocumento = Nothing

        Try

            'Leggo i dati relativi al nodo Movimento (contabile)
            Leggi_Movimenti_DocContabile(_objParametriServer,
                                         msgErrore,
                                         _piva,
                                         _idAgenda,
                                         _codReport,
                                         dtDocumento,
                                         x_Id_Mov_Contabile,
                                         x_RagSoc_Impresa,
                                         x_CodiceFiscale_Impresa,
                                         x_Mov_Desc_Contabile,
                                         x_Data_Movimento,
                                         x_Ora,
                                         x_Scadenza,
                                         x_Scadenza_Extra,
                                         x_Doc_Numero_Sin,
                                         x_Doc_Numero,
                                         x_Doc_Numero_Des,
                                         x_Progr_Protocollo,
                                         x_Progr_Registrazione,
                                         x_Data_Registrazione,
                                         x_Num_Protocollo,
                                         x_Colli,
                                         x_Peso,
                                         x_Aspetto,
                                         x_Causale_Trasporto,
                                         x_Edit_Importo,
                                         x_Cod_RisUm,
                                         x_Cod_Contatto,
                                         x_Rag_Soc,
                                         x_Codice_Fiscale,
                                         x_ChkFittizio,
                                         x_Cod_IndirizzoRisUm,
                                         x_Id_Cf_Cliente,
                                         x_Cod_Destinazione,
                                         x_CodContatto_Destinazione,
                                         x_RagSoc_Destinazione,
                                         x_CodiceFiscale_Destinazione,
                                         x_chkfittizio_destinazione,
                                         x_Cod_IndirizzoDestinazione,
                                         x_Id_Cf_Destinazione,
                                         x_Mezzo,
                                         x_Cod_Vettore,
                                         x_Cod_IndirizzoVettore,
                                         x_Id_Cf_Vettore,
                                         x_CodContatto_Vettore,
                                         x_RagSoc_Vettore,
                                         x_CodiceFiscale_Vettore,
                                         x_ChkFittizio_Vettore,
                                         x_NumReg_Vettore,
                                         x_TargaMezzo_Vettore,
                                         x_N_Autorizzazione_Trasporto,
                                         x_Agente_Cod,
                                         x_Natura_Beni,
                                         x_Modalita,
                                         x_Tara_Veicolo,
                                         x_Tara_Imballi,
                                         x_Tipo_Peso,
                                         x_Username_Note,
                                         x_Extra_Str,
                                         x_Extra_Int,
                                         x_Extra_Date,
                                         x_ChkLayout_Bypass_Fatturato,
                                         x_ChkLayout_Join_Prodotti,
                                         x_ChkLayOut_Peso,
                                         x_ChkLayOut_Prezzo,
                                         x_Gestione_Vettore,
                                         Peso_Lordo,
                                         x_ChkLayOut_Litri,
                                         x_Cod_RisUm_Aggiuntivo,
                                         x_Cod_Indirizzo_Aggiuntivo,
                                         x_Id_Cf_Aggiuntivo,
                                         x_CodContatto_Aggiuntivo,
                                         x_RagSoc_Aggiuntivo,
                                         x_CodiceFiscale_Aggiuntivo,
                                         x_ChkFittizio_Aggiuntivo,
                                         Nothing,
                                         x_ChkLayOut_Riscontrato,
                                         x_N_Doc_Cliente,
                                         x_Data_Doc_Cliente,
                                         x_N_Nota_Fattura,
                                         x_Data_Nota_Fattura,
                                         x_N_Nota_DDT,
                                         x_N_Nota_Riga_DDT,
                                         x_Data_Nota_DDT,
                                         _EsigibilitaIva)


            If msgErrore <> "" Then
                _logErrori &= msgErrore
            End If

        Catch ex As Exception
            _logErrori &= "- Lettura della Fattura: " & vbCrLf & ex.Message & vbCrLf
        End Try


        Try

            '############################################################################################
            '############################### INTESTAZIONE DELLA FATTURA #################################
            '############################################################################################

            'MODIFICATA GESTIONE INTESTAZIONE IN DATA 31/08/2015

            'l'intestazione viene letta dopo l'operazione perché serve x_Id_Cf_Cliente

            Leggi_Intestazione_Impresa_2(_objParametriServer,
                                         _progressivoGias,
                                         False,
                                         False,
                                         x_Id_Cf_Cliente,
                                         _logErrori,
                                         _piva,
                                         x_CodiceFiscale_Impresa,
                                         x_RagSoc_Impresa,
                                         x_IndDes_Impresa,
                                         x_FrzDes_Impresa,
                                         x_Cap_Impresa,
                                         x_Comune_Impresa,
                                         x_Provincia_Impresa,
                                         x_Stato_Impresa,
                                         Intestazione_Riga3,
                                         Intestazione_Riga4,
                                         Intestazione_Riga5,
                                         Intestazione_Riga6,
                                         Intestazione_Riga7,
                                         Intestazione_Riga8,
                                         Intestazione_Riga9,
                                         Intestazione_Riga10,
                                         Intestazione_Riga11,
                                         Intestazione_Riga12,
                                         Intestazione_Riga13,
                                         Intestazione_Riga14,
                                         Intestazione_Riga15,
                                         False, Nothing, "", False)

        Catch ex As Exception
            _logErrori &= "- Lettura dei dati dell'intestazione dell'impresa: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Dim documentoSaCod As Integer = 0
        Dim documentoDettagliSaCod = dtDocumento.AsEnumerable().Where(Function(row) row.Item("Sa_Cod_Dett") <> 0 And row.Item("Ordine_Det") <> 1000)
        If documentoDettagliSaCod.Count > 0 Then
            documentoSaCod = documentoDettagliSaCod.First().Item("Sa_Cod_Dett")
        End If

        ImpostazioniImpresaUtente_StampaDoc(_objParametriServer, _objParametriUtenti, _logErrori, _piva, documentoSaCod, stampaSDIAcquisto)

        '=============================================================
        '-------------- IMPRESA CHE EMETTE FATTURA -------------------



        ''modifica del 20/09/2010
        ''introduco la chiamata a questa funzione
        ''per la gestione del taroccamento piva x multi-attività
        'Ricava_Piva_CodiceFiscale(Piva, x_CodiceFiscale_Impresa, Piva, x_CodiceFiscale_Impresa, Nothing)



        '=============================================================
        '-------------- CLIENTE -------------------

        If x_Cod_IndirizzoRisUm <> 0 Then

            Try

                Leggi_Indirizzi(_objParametriServer,
                                x_Cod_Contatto,
                                x_Cod_RisUm,
                                x_Cod_IndirizzoRisUm,
                                x_Ind_Des,
                                x_Frz_Des,
                                x_Cap,
                                x_Comune,
                                x_Provincia,
                                x_Stato,
                                Nothing,
                                Nothing,
                                "")

            Catch ex As Exception
                'dovrebbe verificarsi nel caso di dati importati tramite g2g (non vengono rimappati gli indirizzi)
                _logErrori &= "- Lettura dell'indirizzo del contatto cliente: " & vbCrLf & ex.Message & vbCrLf
            End Try

        End If

        Dim Cliente_RagSoc As String
        Dim Cliente_Indirizzo As String
        Dim Cliente_Cap As String
        Dim Cliente_Frazione As String
        Dim Cliente_Comune As String
        Dim Cliente_Provincia As String
        Dim Cliente_Piva As String
        Dim Cliente_CF As String

        Dim Fornitore_RagSoc As String = ""
        Dim Fornitore_Indirizzo As String = ""
        Dim Fornitore_Cap As String = ""
        Dim Fornitore_Frazione As String = ""
        Dim Fornitore_Comune As String = ""
        Dim Fornitore_Provincia As String = ""
        Dim Fornitore_Piva As String = ""
        Dim Fornitore_CF As String = ""

        Select Case _lavCod

            Case LAVCOD_ORDINE_ACQUISTO, LAVCOD_BOLLA_RICEVUTA, LAVCOD_ACCETTAZIONE_DIVERSI
                'IL CLIENTE E' L'AZIENDA
                Cliente_RagSoc = x_RagSoc_Impresa
                Cliente_Indirizzo = x_IndDes_Impresa
                Cliente_Cap = x_Cap_Impresa
                Cliente_Frazione = x_FrzDes_Impresa
                Cliente_Comune = x_Comune_Impresa
                Cliente_Provincia = "(" & x_Provincia_Impresa & ")"
                Cliente_Piva = _piva
                Cliente_CF = x_CodiceFiscale_Impresa

                Fornitore_RagSoc = x_Rag_Soc

                codiceSDIAcquisto = If(stampaSDIAcquisto, Intestazione_Riga10, "") 'Il codice è salvato in CentriXRubrica nel campo "social"

                Select Case x_Id_Cf_Cliente
                    Case enum_Contatti_IdCf.ContattoEstero
                        '----------------
                        '-----ESTERO
                        '----------------

                        'Se il contatto è estero, il comune potrebbe essere esplicitato nella x_Comune dalla tabella ISTAT oppure
                        'come campo libero nella x_Frz_Des della Indirizzi. Nel primo caso, x_Frz_Des potrebbe contenere una effettiva frazione
                        'inserita dall'utente
                        If String.IsNullOrEmpty(x_Comune) Then
                            Fornitore_Indirizzo = x_Ind_Des
                            Fornitore_Frazione = x_Frz_Des 'Imposto qua la città così viene stampata a fianco del CAP
                        Else
                            Fornitore_Indirizzo = x_Ind_Des & " " & x_Frz_Des
                            Fornitore_Frazione = x_Comune
                        End If

                        Fornitore_Cap = x_Cap
                        Fornitore_Comune = x_Stato
                        Fornitore_Provincia = ""
                        Fornitore_CF = ""

                        '05/11/2019: i contatti esteri sono passati alla gestione chkfittizio

                        'If Flag_StatoMembro = True Then
                        '    Fornitore_Piva = "VAT: " & x_Cod_Contatto
                        'Else
                        '    Fornitore_Piva = ""
                        'End If

                        Ricava_Piva_Codicefiscale(x_Id_Cf_Cliente,
                                                  x_Cod_Contatto,
                                                  "",
                                                    x_ChkFittizio,
                                                  Piva_Cliente,
                                                  Codice_Fiscale_Cliente,
                                                  Nothing)

                        If x_ChkFittizio = 1 Then
                            Fornitore_Piva = ""
                        Else
                            Fornitore_Piva = "VAT: " & x_Cod_Contatto
                        End If


                    Case Else
                        '----------------
                        '-----ITALIANO
                        '----------------
                        Ricava_Piva_Codicefiscale(x_Id_Cf_Cliente,
                                                  x_Cod_Contatto,
                                                  x_Codice_Fiscale,
                                                    x_ChkFittizio,
                                                  Fornitore_Piva,
                                                  Fornitore_CF,
                                                  Flag_ClientePrivato)

                        Fornitore_Piva = "Partita IVA: " & Fornitore_Piva
                        Fornitore_CF = "   Codice Fiscale: " & Fornitore_CF
                        Fornitore_Indirizzo = x_Ind_Des
                        Fornitore_Frazione = x_Frz_Des
                        Fornitore_Cap = x_Cap
                        Fornitore_Comune = x_Comune
                        Fornitore_Provincia = "(" & x_Provincia & ")"

                End Select

            Case Else
                'IL CLIENTE E' IL CLIENTE
                Cliente_RagSoc = x_Rag_Soc

                Piva_Cliente = ""
                Codice_Fiscale_Cliente = ""
                Select Case x_Id_Cf_Cliente
                    Case enum_Contatti_IdCf.ContattoEstero
                        '----------------
                        '-----ESTERO
                        '----------------

                        'Se il contatto è estero, il comune potrebbe essere esplicitato nella x_Comune dalla tabella ISTAT oppure
                        'come campo libero nella x_Frz_Des della Indirizzi. Nel primo caso, x_Frz_Des potrebbe contenere una effettiva frazione
                        'inserita dall'utente
                        If String.IsNullOrEmpty(x_Comune) Then
                            Cliente_Indirizzo = x_Ind_Des
                            Cliente_Frazione = x_Frz_Des 'Imposto qua la città così viene stampata a fianco del CAP
                        Else
                            Cliente_Indirizzo = x_Ind_Des & " " & x_Frz_Des
                            Cliente_Frazione = x_Comune
                        End If

                        Cliente_Cap = x_Cap
                        Cliente_Comune = x_Stato 'Imposto qua lo stato così viene stampato sotto la città
                        Cliente_Provincia = ""

                        Piva_Cliente = x_Cod_Contatto
                        Codice_Fiscale_Cliente = ""

                        '05/11/2019: i contatti esteri sono passati alla gestione chkfittizio

                        'If Flag_StatoMembro = True Then
                        '    parametro.LblPivaCliente = "VAT:"
                        'Else
                        '    parametro.LblPivaCliente = ""
                        '    Piva_Cliente = ""
                        'End If

                        Ricava_Piva_Codicefiscale(x_Id_Cf_Cliente,
                                                  x_Cod_Contatto,
                                                  "",
                                                    x_ChkFittizio,
                                                  Piva_Cliente,
                                                  Codice_Fiscale_Cliente,
                                                  Nothing)

                        If x_ChkFittizio = 1 Then
                            parametro.LblPivaCliente = ""
                        Else
                            parametro.LblPivaCliente = "VAT:"
                        End If

                        parametro.LblCfCliente = ""
                    Case Else
                        '----------------
                        '-----ITALIANO
                        '----------------
                        Ricava_Piva_Codicefiscale(x_Id_Cf_Cliente,
                                                  x_Cod_Contatto,
                                                  x_Codice_Fiscale,
                                                  x_ChkFittizio,
                                                  Piva_Cliente,
                                                  Codice_Fiscale_Cliente,
                                                  Flag_ClientePrivato)

                        Cliente_Indirizzo = x_Ind_Des
                        Cliente_Frazione = x_Frz_Des
                        Cliente_Cap = x_Cap
                        Cliente_Comune = x_Comune
                        Cliente_Provincia = "(" & x_Provincia & ")"

                        '14/03/2019: commentato per lasciare quanto impostato da Ricava_Piva_Codicefiscale
                        'If x_Id_Cf_Cliente = 0 Then
                        '    Piva_Cliente = ""
                        'End If
                End Select

                Cliente_Piva = CStr(Piva_Cliente)
                Cliente_CF = Codice_Fiscale_Cliente

        End Select

        '=============================================================
        '-------------- DESTINATARIO DOCUMENTO -------------------

        'Destinazione Diversa impostata
        If x_Cod_Destinazione <> 0 Then

            Destinatario_RagSoc = x_RagSoc_Destinazione

            Try

                Leggi_Indirizzi(_objParametriServer,
                                x_CodContatto_Destinazione,
                                x_Cod_Destinazione,
                                x_Cod_IndirizzoDestinazione,
                                x_IndDes_Destinazione,
                                x_FrzDes_Destinazione,
                                x_Cap_Destinazione,
                                x_Comune_Destinazione,
                                x_Provincia_Destinazione,
                                x_Stato_Destinazione,
                                Nothing,
                                x_IndirizzoTipoDesc_Destinazione,
                                x_Cod_Contatto)

            Catch ex As Exception
                _logErrori &= "- Lettura dell'indirizzo del destinatario: " & vbCrLf & ex.Message & vbCrLf
                'dovrebbe verificarsi nel caso di dati importati tramite g2g (non vengono rimappati gli indirizzi)
            End Try


            Try

                Leggi_Rubrica(_objParametriServer,
                              x_CodContatto_Destinazione,
                              x_Cod_Destinazione,
                              0,
                              x_Tel_Destinazione,
                              x_Cell_Destinazione)

                If x_Tel_Destinazione <> "" Then
                    Destinatario_Tel = "Tel: " & x_Tel_Destinazione
                End If
                If x_Cell_Destinazione <> "" Then
                    Destinatario_Tel &= " Cell: " & x_Cell_Destinazione
                End If

            Catch ex As Exception
                _logErrori &= "- Lettura della rubrica del destinatario: " & vbCrLf & ex.Message & vbCrLf
            End Try

        Else

            Select Case _lavCod

                Case LAVCOD_ORDINE_ACQUISTO, LAVCOD_BOLLA_RICEVUTA, LAVCOD_ACCETTAZIONE_DIVERSI
                    'il destinatario coincide con l'azienda, in questo caso occorre ricavare l'indirizzo specifico
                    'del centro aziendale scelto sul documento a cui il/i magazzino/i di destinazione appartengono

                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
                    Dim dtCentro As DataTable
                    dtCentro = objCentri.Leggi(_piva, documentoSaCod, 0, 1,
                          AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                        "", "", _objParametriServer)

                    Destinatario_RagSoc = x_RagSoc_Impresa

                    Destinatario_Ind_Des = dtCentro.Rows(0).Item("ind_des")
                    Destinatario_Frz_Des = dtCentro.Rows(0).Item("frz_des")
                    Destinatario_Cap = dtCentro.Rows(0).Item("cap")
                    Destinatario_Comune = dtCentro.Rows(0).Item("com_des")
                    Destinatario_Provincia = "(" & dtCentro.Rows(0).Item("pro_cod") & ")"

                    Destinatario_Tel = Leggi_RubricaTel_CentroAziendale(_objParametriServer, _logErrori, _piva, documentoSaCod)

                Case Else
                    'il destinatario coincide con il cliente
                    Destinatario_RagSoc = x_Rag_Soc

                    x_Id_Cf_Destinazione = x_Id_Cf_Cliente
                    x_RagSoc_Destinazione = x_Rag_Soc
                    x_CodContatto_Destinazione = x_Cod_Contatto
                    x_CodiceFiscale_Destinazione = x_Codice_Fiscale

                    x_IndDes_Destinazione = x_Ind_Des
                    x_FrzDes_Destinazione = x_Frz_Des
                    x_Cap_Destinazione = x_Cap
                    x_Comune_Destinazione = x_Comune
                    x_Provincia_Destinazione = x_Provincia
                    x_Stato_Destinazione = x_Stato

                    Try

                        Leggi_Rubrica(_objParametriServer,
                                      x_Cod_Contatto,
                                      x_Cod_RisUm,
                                      0,
                                      x_Tel_Destinazione,
                                      x_Cell_Destinazione)

                    Catch ex As Exception
                        _logErrori &= "- Lettura della rubrica del cliente: " & vbCrLf & ex.Message & vbCrLf
                    End Try

                    If x_Tel_Destinazione <> "" Then
                        Destinatario_Tel = "Tel: " & x_Tel_Destinazione
                    End If
                    If x_Cell_Destinazione <> "" Then
                        Destinatario_Tel &= " Cell: " & x_Cell_Destinazione
                    End If

            End Select

        End If 'If x_Cod_Destinazione <> 0

        'nel caso in cui la destinazione non sia l'azienda
        If x_Cod_Destinazione <> 0 OrElse Not {LAVCOD_ORDINE_ACQUISTO, LAVCOD_BOLLA_RICEVUTA, LAVCOD_ACCETTAZIONE_DIVERSI}.Contains(_lavCod) Then

            Piva_Dest = ""
            Codice_Fiscale_Dest = ""

            Select Case x_Id_Cf_Destinazione
                Case enum_Contatti_IdCf.ContattoEstero

                    'Se il contatto è estero, il comune potrebbe essere esplicitato nella x_Comune dalla tabella ISTAT oppure
                    'come campo libero nella x_Frz_Des della Indirizzi. Nel primo caso, x_Frz_Des potrebbe contenere una effettiva frazione
                    'inserita dall'utente
                    If String.IsNullOrEmpty(x_Comune_Destinazione) Then
                        Destinatario_Ind_Des = x_IndDes_Destinazione
                        Destinatario_Frz_Des = x_FrzDes_Destinazione 'Imposto qua la città così viene stampata a fianco del CAP
                    Else
                        Destinatario_Ind_Des = x_IndDes_Destinazione & " " & x_FrzDes_Destinazione
                        Destinatario_Frz_Des = x_Comune_Destinazione
                    End If

                    Destinatario_Cap = x_Cap_Destinazione
                    Destinatario_Comune = x_Stato_Destinazione 'Imposto qua lo stato così viene stampato sotto la città
                    Destinatario_Provincia = ""

                    Piva_Dest = x_CodContatto_Destinazione
                    Codice_Fiscale_Dest = ""

                Case Else

                    Ricava_Piva_Codicefiscale(x_Id_Cf_Destinazione,
                                              x_CodContatto_Destinazione,
                                              x_CodiceFiscale_Destinazione,
                                              x_chkfittizio_destinazione,
                                              Piva_Dest,
                                              Codice_Fiscale_Dest,
                                              Nothing)

                    Destinatario_Ind_Des = x_IndDes_Destinazione
                    Destinatario_Frz_Des = x_FrzDes_Destinazione
                    Destinatario_Cap = x_Cap_Destinazione
                    Destinatario_Comune = x_Comune_Destinazione
                    Destinatario_Provincia = "(" & x_Provincia_Destinazione & ")"

                    '14/03/2019: commentato per lasciare quanto impostato da Ricava_Piva_Codicefiscale
                    'If x_Id_Cf_Destinazione = enum_Contatti_IdCf.PersonaFisica Then
                    '    Piva_Dest = ""
                    'End If

            End Select

        End If 'nel caso in cui la destinazione non sia l'azienda


        '=============================================================
        '-------------- CESSIONARIO AGGIUNTIVO -------------------

        'se presente un cessionario diverso
        If x_Cod_RisUm_Aggiuntivo <> 0 Then
            ValorizzaRisUmAggiuntivo(_logErrori, x_Cod_RisUm_Aggiuntivo, x_Cod_Indirizzo_Aggiuntivo, x_Id_Cf_Aggiuntivo,
                                     x_CodContatto_Aggiuntivo, x_RagSoc_Aggiuntivo, x_CodiceFiscale_Aggiuntivo, x_ChkFittizio_Aggiuntivo,
                                     parametro.RagSocAggiuntivo, parametro.IndirizzoAggiuntivo, parametro.CapAggiuntivo, parametro.FrazioneAggiuntivo,
                                     parametro.ComuneAggiuntivo, parametro.ProvAggiuntivo, parametro.TxtPivaAgg, parametro.TxtCfAgg, _objParametriServer)

            If _lavCod = LAVCOD_ORDINE_ACQUISTO Then
                parametro.DescAggiuntivo = "Cedente:"
            Else
                parametro.DescAggiuntivo = "Spettabile:"  ' "Cessionario:"
            End If

        End If


        '=============================================================
        '-------------- INTESTAZIONE DOCUMENTO -------------------

        'Dim Intestazione_RagSoc As String
        'Dim Intestazione_PivaCF As String
        'Dim Intestazione_Indirizzo1 As String
        'Dim Intestazione_Indirizzo2 As String
        'Dim Intestazione_RegImprese As String
        'Dim Intestazione_CapitaleSociale As String
        'Dim Intestazione_REA As String
        'Dim Intestazione_TelFaxCell As String
        'Dim Intestazione_SitoEmail As String
        'Dim Intestazione_Social As String

        Select Case _lavCod

            Case LAVCOD_ORDINE_ACQUISTO, LAVCOD_BOLLA_RICEVUTA, LAVCOD_ACCETTAZIONE_DIVERSI
                'CHI EMETTE E' IL FORNITORE
                'Intestazione_RagSoc = Fornitore_RagSoc
                'Intestazione_PivaCF = Fornitore_Piva & Fornitore_CF
                'Intestazione_Indirizzo1 = Fornitore_Indirizzo & " " & Fornitore_Frazione
                'Intestazione_Indirizzo2 = Fornitore_Cap & " " & Fornitore_Comune & " " & Fornitore_Provincia
                'Intestazione_RegImprese = ""
                'Intestazione_CapitaleSociale = ""
                'Intestazione_REA = ""
                'Intestazione_TelFaxCell = ""
                'Intestazione_SitoEmail = ""
                'Intestazione_Social = ""
                parametro.IntestazioneRiga1 = Fornitore_RagSoc
                parametro.IntestazioneRiga2 = Fornitore_Piva & Fornitore_CF
                parametro.IntestazioneRiga5 = Fornitore_Indirizzo & " " & Fornitore_Frazione
                parametro.IntestazioneRiga6 = Fornitore_Cap & " " & Fornitore_Comune & " " & Fornitore_Provincia

            Case Else
                ''CHI EMETTE E' L'AZIENDA
                parametro.IntestazioneRiga1 = x_RagSoc_Impresa

                If x_Id_Cf_Cliente = enum_Contatti_IdCf.ContattoEstero Then
                    parametro.IntestazioneRiga2 = "VAT: IT" & _pivaReale
                Else
                    parametro.IntestazioneRiga2 = "Partita IVA: " & _pivaReale
                End If
                If x_CodiceFiscale_Impresa <> "" Then
                    parametro.IntestazioneRiga2 &= "   Codice Fiscale: " & x_CodiceFiscale_Impresa
                End If

                parametro.IntestazioneRiga3 = Intestazione_Riga3
                parametro.IntestazioneRiga4 = Intestazione_Riga4
                parametro.IntestazioneRiga5 = Intestazione_Riga5
                parametro.IntestazioneRiga6 = Intestazione_Riga6
                parametro.IntestazioneRiga7 = Intestazione_Riga7
                parametro.IntestazioneRiga8 = Intestazione_Riga8
                parametro.IntestazioneRiga9 = Intestazione_Riga9
                parametro.IntestazioneRiga10 = Intestazione_Riga10
                parametro.IntestazioneRiga11 = Intestazione_Riga11
                parametro.IntestazioneRiga12 = Intestazione_Riga12
                parametro.IntestazioneRiga13 = Intestazione_Riga13
                parametro.IntestazioneRiga14 = Intestazione_Riga14
                parametro.IntestazioneRiga15 = Intestazione_Riga15

                'Intestazione_RagSoc = x_RagSoc_Impresa
                'Intestazione_PivaCF = str_Piva_CF
                'Intestazione_Indirizzo1 = "Sede legale: " & Indirizzo_Impresa1
                'Intestazione_Indirizzo2 = Indirizzo_Impresa2
                'Intestazione_RegImprese = "Iscr. al n. " & x_RegImprese & " del Reg. Imprese di " & x_Provincia_RegImprese
                'Intestazione_CapitaleSociale = x_CapitaleSociale
                'Intestazione_REA = str_REA_ISO
                'Intestazione_TelFaxCell = TelFaxCell
                'Intestazione_SitoEmail = Sito_Email
                'Intestazione_Social = x_SocialNetwork
        End Select

        'parametri spostati a fine sub


        drIntestazioneNew.Lav_Cod = _lavCod
        drIntestazioneNew.Extra_Int = x_Extra_Int

        drIntestazioneNew.SuperPiva = _piva


        '===============================================================
        '----------------------- DATI FATTURA --------------------------


        '=======================================================================================
        '------------------ PERSONALIZZAZIONI  -------------------------------------------------
        '=======================================================================================

        ModificaLayoutPersonalizzazioni(drIntestazioneNew, parametro.IntestazioneRiga1)


        Select Case _lavCod
            'PER LA SEZIONE DEL VETTORE, VERIFICO DOPO SE VISUALIZZARLO O MENO

            Case LAVCOD_FATTURA_EMESSA
                'CType(rptFattura.Section17.ReportObjects("TxtTotale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Totale Fattura"

                parametro.T_TotDocumento = Recupera_TotaleDocumento(_CodiceLingua, _lavCod)

                If x_Cod_Contatto = _piva Then
                    parametro.TitoloDocumento = Recupera_TitoloDocumento(_CodiceLingua, _lavCod, enum_TipoStampaFattura.Autofattura)
                Else
                    Select Case x_Modalita
                        Case enum_ModalitaFattura.Fattura_AcquistiIntracom
                            parametro.TitoloDocumento = Recupera_TitoloDocumento(_CodiceLingua, _lavCod, enum_TipoStampaFattura.Fattura_AcquistiIntracom)

                            '  Giulia, 15/01/2018 10:38:58: stampa di riepilogo imballi anche in fattura
                            Flag_StampaRiepilogoImballi = True

                        Case enum_ModalitaFattura.Fattura_Acconto
                            parametro.TitoloDocumento = Recupera_TitoloDocumento(_CodiceLingua, _lavCod, enum_TipoStampaFattura.Fattura_Acconto)

                        Case enum_ModalitaFattura.Fattura_Acconto_Soci
                            parametro.TitoloDocumento = Recupera_TitoloDocumento(_CodiceLingua, _lavCod, enum_TipoStampaFattura.Fattura_Acconto_Soci)

                        Case Else
                            Select Case x_Extra_Int
                                Case enum_FatturaTipo.Differita ' 0 ' differita
                                    parametro.TitoloDocumento = Recupera_TitoloDocumento(_CodiceLingua, _lavCod, enum_TipoStampaFattura.Fattura_Differita)

                                    '  Giulia, 15/01/2018 10:38:58: stampa di riepilogo imballi anche in fattura
                                    Flag_StampaRiepilogoImballi = True

                                Case enum_FatturaTipo.Immediata '1 'immediata
                                    parametro.TitoloDocumento = Recupera_TitoloDocumento(_CodiceLingua, _lavCod, enum_TipoStampaFattura.Fattura_Immediata)
                                    ''colli e aspetto
                                    'rptFattura.Section9.SectionFormat.EnableSuppress = False
                                    'firme
                                    _rptFattura.Section10.SectionFormat.EnableSuppress = False
                                    ' CType(rptFattura.Section17.ReportObjects("TxtFirma"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Firma Destinatario"
                                    Flag_StampaRiepilogoImballi = True

                            End Select
                    End Select

                End If 'autofattura

                'If _CodiceLingua <> "IT" And _CodiceLingua <> "" Then
                '    parametro.TitoloDocumento = _objTradStampe.Fattura_Titolo
                'End If

            Case LAVCOD_FATTURA_PROFORMA
                parametro.TitoloDocumento = Recupera_TitoloDocumento(_CodiceLingua, _lavCod, enum_TipoStampaFattura.NonDefinito)
                parametro.T_TotDocumento = Recupera_TotaleDocumento(_CodiceLingua, _lavCod)

            Case LAVCOD_NOTA_ACCREDITO_EMESSA
                parametro.TitoloDocumento = Recupera_TitoloDocumento(_CodiceLingua, _lavCod, enum_TipoStampaFattura.NonDefinito)
                parametro.T_TotDocumento = Recupera_TotaleDocumento(_CodiceLingua, _lavCod)

            Case LAVCOD_DDT_CONTABILIZZATO_EMESSO,
                    LAVCOD_BOLLA_EMESSA

                parametro.TitoloDocumento = Recupera_TitoloDocumento(_CodiceLingua, _lavCod, enum_TipoStampaFattura.NonDefinito)
                parametro.RiferimentoDocumento = "D.P.R. 472 del 14.08.1996 - D.P.R. 696 del 21.12.1996"
                parametro.T_TotDocumento = Recupera_TotaleDocumento(_CodiceLingua, _lavCod)

                'intestazione ddt
                _rptFattura.PageHeaderSection1.SectionFormat.EnableSuppress = False
                'intestazione fattura
                _rptFattura.Section8.SectionFormat.EnableSuppress = True
                'dettagli ddt
                _rptFattura.DetailSection1.SectionFormat.EnableSuppress = False
                'dettagli fattura
                _rptFattura.Section3.SectionFormat.EnableSuppress = True
                'totale ddt
                _rptFattura.Section21.SectionFormat.EnableSuppress = False
                'totale fattura
                _rptFattura.Section17.SectionFormat.EnableSuppress = True
                ''colli e aspetto

                ''pagamenti

                Flag_StampaRiepilogoImballi = True

            Case LAVCOD_ORDINE_VENDITA
                _rptFattura.Section6.SectionFormat.EnableSuppress = True 'sopprimo il riquadro con la normativa fiscale
                _rptFattura.ReportFooterSection1.SectionFormat.EnableSuppress = False 'visualizzo firma per accettazione

                parametro.TitoloDocumento = Recupera_TitoloDocumento(_CodiceLingua, _lavCod, enum_TipoStampaFattura.NonDefinito)
                parametro.T_TotDocumento = Recupera_TotaleDocumento(_CodiceLingua, _lavCod)
                parametro.LblScadenza = "Spedizione prev."

            Case LAVCOD_ORDINE_ACQUISTO
                _rptFattura.Section6.SectionFormat.EnableSuppress = True 'sopprimo il riquadro con la normativa fiscale
                _rptFattura.ReportFooterSection1.SectionFormat.EnableSuppress = False 'visualizzo firma per accettazione

                parametro.TitoloDocumento = Recupera_TitoloDocumento(_CodiceLingua, _lavCod, enum_TipoStampaFattura.NonDefinito)
                parametro.T_TotDocumento = Recupera_TotaleDocumento(_CodiceLingua, _lavCod)
                parametro.LblScadenza = "Spedizione prev."
            Case LAVCOD_PREVENTIVO_VENDITA
                _rptFattura.Section6.SectionFormat.EnableSuppress = True 'sopprimo il riquadro con la normativa fiscale
                _rptFattura.ReportFooterSection1.SectionFormat.EnableSuppress = False 'visualizzo firma per accettazione

                parametro.TitoloDocumento = Recupera_TitoloDocumento(_CodiceLingua, _lavCod, enum_TipoStampaFattura.NonDefinito)
                parametro.T_TotDocumento = Recupera_TotaleDocumento(_CodiceLingua, _lavCod)
                parametro.LblScadenza = "Spedizione prev."
        End Select

        '20/03/2019: in attesa di gestire su tabella anche la lblscadenza
        '(si può fare ma non c'è stato tempo)
        If _CodiceLingua <> "IT" AndAlso _CodiceLingua <> "" Then
            parametro.LblScadenza = _objTradStampe.Fattura_Scadenza
        End If

        drIntestazioneNew.Extra_Str_2 = _lavCod

        drIntestazioneNew.Doc_Numero = x_Doc_Numero_Sin & CStr(x_Doc_Numero) & x_Doc_Numero_Des

        _identNumero = "n" & drIntestazioneNew.Doc_Numero

        Select Case _lavCod
            Case LAVCOD_ORDINE_VENDITA, LAVCOD_ORDINE_ACQUISTO, LAVCOD_PREVENTIVO_VENDITA
                If CDate(CDate(x_Ora).ToShortDateString) = AGRODATAFINE OrElse CDate(CDate(x_Ora).ToShortDateString) = AGRODATAINIZIO Then
                    drIntestazioneNew.Scadenza = ""
                Else
                    drIntestazioneNew.Scadenza = CDate(x_Ora).ToShortDateString
                End If
                x_Scadenza = CDate(x_Ora).ToShortDateString

            Case Else
                If x_Scadenza = AGRODATAFINE OrElse x_Scadenza = AGRODATAINIZIO Then
                    drIntestazioneNew.Scadenza = ""
                Else
                    drIntestazioneNew.Scadenza = x_Scadenza
                End If
        End Select


        drIntestazioneNew.Data_Movimento = CType(x_Data_Movimento, DateTime)
        _identData = Format(x_Data_Movimento, "yyyy-MM-dd")

        'mi servono nel salvataggio dell'allegato
        _dataInizioAllegato = x_Data_Movimento
        _dataFineAllegato = x_Scadenza

        Select Case _lavCod
            Case LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_BOLLA_EMESSA
                'nei ddt le note sono salvate diversamente
                drIntestazioneNew.Extra_Str_1 = ""
                'DrIntestazioneNew.Note = x_Mov_Desc_Contabile 'note
                Note = If(String.IsNullOrWhiteSpace(x_Extra_Str), x_Mov_Desc_Contabile, x_Extra_Str) 'note

            Case Else
                'in mov_desc c'è la descrizione della fattura, non le note
                drIntestazioneNew.Extra_Str_1 = x_Mov_Desc_Contabile

                'DrIntestazioneNew.Note = x_Extra_Str 'note
                Note = x_Extra_Str 'note
        End Select

        Note = Replace(Note, "?", "€")

        drIntestazioneNew.Note = Note

        drIntestazioneNew.Aspetto = x_Aspetto
        drIntestazioneNew.Colli = x_Colli
        drIntestazioneNew.Causale_Trasporto = x_Causale_Trasporto
        drIntestazioneNew.Gest_Vettore = x_Gestione_Vettore

        'modifica del 22/04/2015: stampata oltre alla data di spedizione anche l'ora (che in passato era stata tolta)
        'riciclo il campo telefono per la data e ora consegna
        If CDate(CDate(x_Ora).ToShortDateString) = AGRODATAFINE OrElse CDate(CDate(x_Ora).ToShortDateString) = AGRODATAINIZIO Then
            drIntestazioneNew.Telefono = ""
        Else
            '05/11/2018: applicata stessa modifica fatta al ddt
            'DrIntestazioneNew.Telefono = Format(CDate(x_Ora), "dd/MM/yyyy HH:mm") 'CDate(x_Ora).ToShortDateString
            drIntestazioneNew.Telefono = Format(CDate(x_Ora), "dd/MM/yyyy")
            If Format(CDate(x_Ora), "HH:mm") <> "00:00" Then
                drIntestazioneNew.Telefono &= " " & Format(CDate(x_Ora), "HH:mm")
            End If
        End If

        '22/04/2015: sostituito x_peso con Peso_Lordo
        Select Case Peso_Lordo
            Case 0 : drIntestazioneNew.Peso = "n.d."
            Case Else : drIntestazioneNew.Peso = Peso_Lordo
        End Select

        '=============================================================
        '------------------------ CLIENTE  ---------------------------

        drIntestazioneNew.Rag_Soc = Cliente_RagSoc
        drIntestazioneNew.Ind_Des = Cliente_Indirizzo
        drIntestazioneNew.Frz_Des = Cliente_Frazione
        drIntestazioneNew.Cap = Cliente_Cap
        drIntestazioneNew.Comune = Cliente_Comune
        drIntestazioneNew.Provincia = Cliente_Provincia

        parametro.TxtPivaCliente = Cliente_Piva
        parametro.TxtCfCliente = Cliente_CF
        parametro.CodiceSDI = codiceSDIAcquisto

        _identContatto = Cliente_RagSoc

        '=============================================================
        '--------------------- DESTINATARIO --------------------------

        '22/01/2019:
        If x_IndirizzoTipoDesc_Destinazione = "" Then
            drIntestazioneNew.DestinazioneRag_Soc = Destinatario_RagSoc
        Else
            drIntestazioneNew.DestinazioneRag_Soc = x_IndirizzoTipoDesc_Destinazione
        End If

        drIntestazioneNew.DestinazioneInd_Des = Destinatario_Ind_Des
        drIntestazioneNew.DestinazioneFrz_Des = Destinatario_Frz_Des
        drIntestazioneNew.DestinazioneCap = Destinatario_Cap
        drIntestazioneNew.DestinazioneComune = Destinatario_Comune
        drIntestazioneNew.DestinazioneProvincia = Destinatario_Provincia


        '=============================================================
        '------------------------ VETTORE --------------------------
        AggiungiVettoreMezzo(drIntestazioneNew,
                             x_Mezzo, x_Cod_Vettore, x_Cod_IndirizzoVettore,
                             x_Id_Cf_Vettore, x_CodContatto_Vettore, x_RagSoc_Vettore,
                             x_NumReg_Vettore, x_TargaMezzo_Vettore, x_ChkFittizio_Vettore)


        '=============================================================
        '------------------------ AGENTE -----------------------------
        If x_Agente_Cod <> 0 Then
            parametro.AgenteInfo = ImpostaAgente(_logErrori, x_Agente_Cod, _objParametriServer)
        End If


        '=============================================================================
        '---------------------- DOCUMENTI COLLEGATI ----------------

        If _lavCod = LAVCOD_NOTA_ACCREDITO_EMESSA Then

            '-------------------------------------------------
            '------------- FATTURA AGGANCIATA ----------------
            '-------------------------------------------------

            Riferimento_DocAllegato = Leggi_RiferimentoFattura_NEW(_objParametriServer, _idAgenda)
            parametro.RiferimentoDocumento = Riferimento_DocAllegato

        End If


        '=============================================================
        '--------------------- PAGAMENTI --------------------------

        drIntestazioneNew.NSBanca = ""
        drIntestazioneNew.VSBanca = ""

        AggiungiDettagliPagamento(drIntestazioneNew, x_Id_Mov_Contabile, x_Num_Protocollo)
        '=============================================================

        NascondiLoghiVuoti(_logErrori, drIntestazioneNew, _rptFattura, _objParametriServer)

        Return drIntestazioneNew

    End Function

    Private Sub AggiungiVettoreMezzo(ByRef drIntestazioneNew As DataSetFattura.IntestazioneFatturaRow,
                                     ByVal x_Mezzo As Integer,
                                     ByVal x_Cod_Vettore As Integer,
                                     ByVal x_Cod_IndirizzoVettore As Integer,
                                     ByVal x_Id_Cf_Vettore As Integer,
                                     ByVal x_CodContatto_Vettore As String,
                                     ByVal x_RagSoc_Vettore As String,
                                     ByVal x_NumReg_Vettore As String,
                                     ByVal x_TargaMezzo_Vettore As String,
                                     ByVal x_ChkFittizio_Vettore As Integer)

        Dim indDesVettore As String = ""
        Dim frzDesVettore As String = ""
        Dim capVettore As String = ""
        Dim comuneVettore As String = ""
        Dim provinciaVettore As String = ""
        Dim statoVettore As String = ""

        Try
            Select Case x_Cod_Vettore

                Case 0

                    If x_Mezzo = 0 Then
                        'la label della data rimane "data spedizione"

                        drIntestazioneNew.Mezzo = "CEDENTE"
                    Else
                        'la label della data diventa "data consegna"

                        drIntestazioneNew.Mezzo = "CESSIONARIO"
                    End If

                Case Else

                    'la label della data rimane "data spedizione"

                    '-> gestito da formatta sezione
                    'If x_Extra_Int = 1 Or Lav_Cod = LAVCOD_DDT_CONTABILIZZATO_EMESSO Or Lav_Cod = LAVCOD_ORDINE Then
                    '    rptFattura.Section19.SectionFormat.EnableSuppress = False
                    'End If

                    drIntestazioneNew.Mezzo = "VETTORE"

                    drIntestazioneNew.VettoreRag_Soc = x_RagSoc_Vettore

                    '04/11/2019: gestita la visibilità o meno del cod_contatto vettore
                    'drIntestazioneNew.VettorePiva = ""
                    'If Not IsDBNull(x_CodContatto_Vettore) Then
                    '    If Len(Trim(x_CodContatto_Vettore)) > 0 Then
                    '        drIntestazioneNew.VettorePiva = "P. Iva: " & x_CodContatto_Vettore
                    '    End If
                    'End If
                    'If IsNumeric(x_CodContatto_Vettore) Then
                    '    If x_CodContatto_Vettore < 0 Then
                    '        drIntestazioneNew.VettorePiva = ""
                    '    End If
                    'End If
                    Ricava_Piva_Codicefiscale(x_Id_Cf_Vettore, _
                                      x_CodContatto_Vettore,
                                      "",
                                      x_ChkFittizio_Vettore,
                                      x_CodContatto_Vettore,
                                      "",
                                      Nothing)

                    If x_CodContatto_Vettore <> "" Then
                        drIntestazioneNew.VettorePiva = "P. Iva: " & x_CodContatto_Vettore
                    Else
                        drIntestazioneNew.VettorePiva = ""
                    End If

                    drIntestazioneNew.VettoreTarga = ""
                    If Not IsDBNull(x_TargaMezzo_Vettore) AndAlso Len(Trim(x_TargaMezzo_Vettore)) > 0 Then
                        drIntestazioneNew.VettoreTarga = "Targa: " & x_TargaMezzo_Vettore
                    End If

                    Try

                        Leggi_Indirizzi(_objParametriServer,
                                        x_CodContatto_Vettore,
                                        x_Cod_Vettore,
                                        x_Cod_IndirizzoVettore,
                                        indDesVettore,
                                        frzDesVettore,
                                        capVettore,
                                        comuneVettore,
                                        provinciaVettore,
                                        statoVettore,
                                        Nothing,
                                        Nothing, _
                                        "")

                    Catch ex As Exception
                        _logErrori &= "- Lettura dell'indirizzo del vettore: " & vbCrLf & ex.Message & vbCrLf
                    End Try

                    Select Case x_Id_Cf_Vettore
                        Case 2
                            drIntestazioneNew.VettoreInd_Des = "" 'x_IndDes_Vettore
                            drIntestazioneNew.VettoreFrz_Des = "" ' x_FrzDes_Vettore
                            drIntestazioneNew.VettoreCap = "" 'x_Cap_Vettore
                            drIntestazioneNew.VettoreComune = indDesVettore & " " & capVettore & " " & frzDesVettore & " " & statoVettore
                            drIntestazioneNew.VettoreProvincia = ""
                            drIntestazioneNew.VettoreNumReg = ""
                            If Not IsDBNull(x_NumReg_Vettore) AndAlso
                               Trim(x_NumReg_Vettore) <> "0" AndAlso Trim(x_NumReg_Vettore) <> "" Then
                                drIntestazioneNew.VettoreNumReg = "Albo: " & x_NumReg_Vettore
                            End If

                        Case Else
                            drIntestazioneNew.VettoreInd_Des = "" '& x_IndDes_Vettore & " " & x_FrzDes_Vettore
                            drIntestazioneNew.VettoreFrz_Des = "" 'x_FrzDes_Vettore
                            drIntestazioneNew.VettoreCap = "" 'x_Cap_Vettore
                            drIntestazioneNew.VettoreComune = indDesVettore & " " & frzDesVettore & capVettore & " " & comuneVettore & " (" & provinciaVettore & ")"
                            drIntestazioneNew.VettoreProvincia = "" '"(" & x_Provincia_Vettore & ")"
                            drIntestazioneNew.VettoreNumReg = ""
                            If Not IsDBNull(x_NumReg_Vettore) AndAlso
                               Trim(x_NumReg_Vettore) <> "0" AndAlso Trim(x_NumReg_Vettore) <> "" Then
                                drIntestazioneNew.VettoreNumReg = "Albo: " & x_NumReg_Vettore
                            End If
                    End Select

            End Select

        Catch ex As Exception
            _logErrori &= "- Impostazione del vettore: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Sub AggiungiDettagliPagamento(ByRef drIntestazioneNew As DataSetFattura.IntestazioneFatturaRow,
                                          ByVal x_Id_Mov_Contabile As Integer,
                                          ByVal x_Num_Protocollo As Decimal)

        Dim pagamentoDesc As String = ""
        Dim nsBanca As String = ""
        Dim vsBanca As String = ""

        Dim x_RisorsaFinanziaria_Dare As String = ""
        Dim x_RisorsaFinanziaria_Avere As String = ""

        Try
            Leggi_Pagamenti(_objParametriServer,
                            _piva,
                            _lavCod,
                            _idAgenda,
                            x_Id_Mov_Contabile,
                            x_Num_Protocollo,
                            x_RisorsaFinanziaria_Dare,
                            x_RisorsaFinanziaria_Avere,
                            pagamentoDesc)


            Select Case _lavCod
                Case LAVCOD_NOTA_ACCREDITO_EMESSA
                    If x_RisorsaFinanziaria_Dare <> "" Then
                        nsBanca = " - VS banca: " & x_RisorsaFinanziaria_Dare
                    End If
                    If x_RisorsaFinanziaria_Avere <> "" Then
                        vsBanca = " - NS banca: " & x_RisorsaFinanziaria_Avere
                    End If

                Case Else
                    If x_RisorsaFinanziaria_Dare <> "" Then
                        nsBanca = " - NS banca: " & x_RisorsaFinanziaria_Dare
                    End If
                    If x_RisorsaFinanziaria_Avere <> "" Then
                        vsBanca = " - VS banca: " & x_RisorsaFinanziaria_Avere
                    End If
            End Select

            'modifica del 14/11/2011: tutti i dettagli del pagamento li visualizzo in questo campo del DataSet
            drIntestazioneNew.NSBanca = pagamentoDesc & nsBanca & vsBanca

        Catch ex As Exception
            _logErrori &= "- Lettura dei pagamenti della fattura: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Sub ModificaLayoutCessionarioAggiuntivo(ByVal x_Cod_RisUm_Aggiuntivo As Integer)

        Try
            'se c'è il cessionario aggiuntivo ==> mostro i suoi dati e nascondo il logo
            'altrimenti i dati sono già nascosti di default (nell'intestazione standard x Cofruta)
            'e il logo è visibile di default
            If x_Cod_RisUm_Aggiuntivo <> 0 Then
                'se è presente quel box, allora sono in un report che contempla la possibilità del cessionario aggiuntivo
                If _rptFattura.ReportDefinition.Sections("Section2").ReportObjects.Item("BoxAggiuntivoSTD") IsNot Nothing Then
                    _rptFattura.ReportDefinition.Sections("Section2").ReportObjects.Item("BoxAggiuntivoSTD").ObjectFormat.EnableSuppress = False
                    _rptFattura.ReportDefinition.Sections("Section2").ReportObjects.Item("DescAggiuntivoSTD").ObjectFormat.EnableSuppress = False
                    _rptFattura.ReportDefinition.Sections("Section2").ReportObjects.Item("RagSocAggiuntivoSTD").ObjectFormat.EnableSuppress = False
                    _rptFattura.ReportDefinition.Sections("Section2").ReportObjects.Item("IndirizzoAggiuntivoSTD").ObjectFormat.EnableSuppress = False
                    _rptFattura.ReportDefinition.Sections("Section2").ReportObjects.Item("CAPAggiuntivoSTD").ObjectFormat.EnableSuppress = False
                    _rptFattura.ReportDefinition.Sections("Section2").ReportObjects.Item("FrazioneAggiuntivoSTD").ObjectFormat.EnableSuppress = False
                    _rptFattura.ReportDefinition.Sections("Section2").ReportObjects.Item("ComuneAggiuntivoSTD").ObjectFormat.EnableSuppress = False
                    _rptFattura.ReportDefinition.Sections("Section2").ReportObjects.Item("ProvAggiuntivoSTD").ObjectFormat.EnableSuppress = False
                    _rptFattura.ReportDefinition.Sections("Section2").ReportObjects.Item("LblPivaClienteAggSTD").ObjectFormat.EnableSuppress = False
                    _rptFattura.ReportDefinition.Sections("Section2").ReportObjects.Item("TxtPivaAggSTD").ObjectFormat.EnableSuppress = False
                    _rptFattura.ReportDefinition.Sections("Section2").ReportObjects.Item("LblCFClienteAggSTD").ObjectFormat.EnableSuppress = False
                    _rptFattura.ReportDefinition.Sections("Section2").ReportObjects.Item("TxtCFAggSTD").ObjectFormat.EnableSuppress = False

                    _rptFattura.ReportDefinition.Sections("Section2").ReportObjects.Item("LogoSTD").ObjectFormat.EnableSuppress = True
                End If
            End If

        Catch ex As Exception
            _logErrori &= "- Modifiche Layout Cessionario Aggiuntivo: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Sub ModificaLayoutStampaLitri(ByVal x_ChkLayOut_Litri As Integer,
                                          ByVal litriTotali As Decimal,
                                          ByRef Parametro_Litri As String)

        Dim hideObjectNamesField As New List(Of String)
        Dim hideObjectNamesText As New List(Of String)
        Dim hideObjectNamesLine As New List(Of String)
        Dim sectionName As String
        Dim objStampeUtility As New AgronicaCoreStampeDAL.Utility

        Try

            If x_ChkLayOut_Litri = 1 AndAlso litriTotali <> 0 Then

                Parametro_Litri = CStr(litriTotali)

            Else
                'Se la Sezione 9 è visibile
                'If rptFattura.ReportDefinition.Sections("Section9").SectionFormat.EnableSuppress = False Then
                sectionName = "Section9"

                hideObjectNamesField.Add("Litri_Tot1")
                hideObjectNamesText.Add("LitriLabel1")
                hideObjectNamesLine.Add("LinePeso1")

                'se quei campi effettivamente esistono li nascondo e faccio le modifiche, sennò potrebbe andare in errore
                For Each ro In _rptFattura.ReportDefinition.Sections(sectionName).ReportObjects
                    If ro.Name.Equals("Litri_Tot1") OrElse ro.Name.Equals("LitriLabel1") OrElse ro.Name.Equals("LinePeso1") OrElse ro.Name.Equals("PesoComplessivo1") Then
                        objStampeUtility.HideShowObjCrystal(_rptFattura, sectionName, ReportObjectKind.LineObject, hideObjectNamesLine, False, _objParametriServer)
                        objStampeUtility.HideShowObjCrystal(_rptFattura, sectionName, ReportObjectKind.TextObject, hideObjectNamesText, False, _objParametriServer)
                        objStampeUtility.HideShowObjCrystal(_rptFattura, sectionName, ReportObjectKind.FieldObject, hideObjectNamesField, False, _objParametriServer)

                        'Dim pesoComplessivoMove = _rptFattura.ReportDefinition.Sections(sectionName).ReportObjects.Item("PesoComplessivo1")
                        Dim pesoComplessivoMove = _rptFattura.ReportDefinition.Sections(sectionName).ReportObjects.Item("TPeso1")
                        pesoComplessivoMove.Left = pesoComplessivoMove.Left + 100
                    End If
                Next

                'End If

            End If

        Catch ex As Exception
            _logErrori &= "- Modifiche Layout Stampa Litri: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Sub ModificaLayoutPersonalizzazioni(ByRef drIntestazioneNew As DataSetFattura.IntestazioneFatturaRow,
                                                ByRef Parametro_Intestazione_Riga1 As String)

        Try
            '  Giulia, 19/01/2017 10:34:43: se viene usata la tabella configurazione_stampe è possibile gestire le personalizzazioni in questo modo

            If _usoConfigStampe = True Then

                '------------------ PERSONALIZZAZIONI / SEZIONI DA NASCONDERE  -------------------------
                If _objConfigStampe.SezioniDaNascondereArray IsNot Nothing Then
                    For Each sec In _rptFattura.ReportDefinition.Sections
                        If _objConfigStampe.SezioniDaNascondereArray.Contains(sec.Name) Then
                            sec.SectionFormat.EnableSuppress = True
                        End If
                    Next
                End If

                '------------------ PERSONALIZZAZIONI / CARICAMENTO LOGO -------------------------------
                ' Possibilità: LogoInAlto, Logo (= logo in basso), LogoHeader, LogoFooter
                '---------------------------------------------------------------------------------------
                If _objConfigStampe.PosizioneLogo IsNot Nothing AndAlso Not String.IsNullOrEmpty(_objConfigStampe.PosizioneLogo) Then
                    CaricaLogoInCampoBlobFattura(_logErrori, drIntestazioneNew, _piva, _objConfigStampe.PosizioneLogo)

                    If _objConfigStampe.PosizioneLogo = STAMPE_CONTAB_LOGO_IN_ALTO Then
                        'svuoto la ragione sociale grande in alto 
                        drIntestazioneNew.SuperRag_Soc = ""
                    ElseIf _objConfigStampe.PosizioneLogo = STAMPE_CONTAB_LOGO_IN_BASSO Then
                        'imposto la ragione sociale grande in alto 
                        drIntestazioneNew.SuperRag_Soc = Parametro_Intestazione_Riga1
                        'svuoto il parametro ragione sociale piccola in basso
                        Parametro_Intestazione_Riga1 = ""
                    End If
                End If

                '------------------ PERSONALIZZAZIONI / LIVELLO CONFEZIONAMENTO / CERTIFICAZIONI IN DESCRIZIONE  -------------------------
                'Non serve più perché passerò direttamente i valori di _objConfigStampe
                'Flag_ConfezioniDesc = _objConfigStampe.Flag_ConfezioniDesc
                'Flag_ContenitoriDesc = _objConfigStampe.Flag_ContenitoriDesc
                'Flag_ImballaggiDesc = _objConfigStampe.Flag_ImballaggiDesc
                'Flag_CertificazioniDesc = _objConfigStampe.Flag_CertificazioniDesc

            Else
                '  Giulia, 19/01/2017 10:36:14: se non ho il configStampe, allora faccio il giro vecchio verificando il codice cliente

                '=============================================================
                '------------------ LOGO / INTESTAZIONE  --------------------------

                'ragione sociale sotto al logo
                Select Case _progressivoGias

                    '=========================================
                    '===== LOGO IN ALTO, RAG_SOC IN BASSO =====
                    '=========================================
                    Case enum_CodiceGIAS_Clienti.TenutaCroci,
                        enum_CodiceGIAS_Clienti.Diamanti,
                        enum_CodiceGIAS_Clienti.Pelliconi,
                        enum_CodiceGIAS_Clienti.Molinelli,
                        enum_CodiceGIAS_Clienti.Montegrande,
                        enum_CodiceGIAS_Clienti.FattoriaMonticinoRosso,
                        enum_CodiceGIAS_Clienti.SantaLucia,
                        enum_CodiceGIAS_Clienti.Zuffa,
                        enum_CodiceGIAS_Clienti.PoderePalazzo,
                        enum_CodiceGIAS_Clienti.TenutaColleAngeli,
                        enum_CodiceGIAS_Clienti.PodereBianchi,
                        enum_CodiceGIAS_Clienti.CaPruccolo,
                        enum_CodiceGIAS_Clienti.CasaZanni,
                        enum_CodiceGIAS_Clienti.AlCanevon,
                        enum_CodiceGIAS_Clienti.FragolaDeBosc,
                        enum_CodiceGIAS_Clienti.PaoloBea,
                        enum_CodiceGIAS_Clienti.CollinaDeiPoeti,
                        enum_CodiceGIAS_Clienti.TenutaNeri,
                        enum_CodiceGIAS_Clienti.LaCastellana,
                        enum_CodiceGIAS_Clienti.Qualitoscana,
                        enum_CodiceGIAS_Clienti.PoggioRegini,
                        enum_CodiceGIAS_Clienti.DeRiz,
                        enum_CodiceGIAS_Clienti.TenutaPalazzona,
                        enum_CodiceGIAS_Clienti.CampanacciSanMamante,
                        enum_CodiceGIAS_Clienti.LaRizzola,
                        enum_CodiceGIAS_Clienti.TenutaGodenza,
                        enum_CodiceGIAS_Clienti.GalloNero,
                        enum_CodiceGIAS_Clienti.Quarticello,
                        enum_CodiceGIAS_Clienti.OrtofruttaGrosseto,
                        enum_CodiceGIAS_Clienti.LaBuonaRomagna,
                        enum_CodiceGIAS_Clienti.AzAgrAndreola,
                        enum_CodiceGIAS_Clienti.Colombarda,
                        enum_CodiceGIAS_Clienti.Lorenzato,
                        enum_CodiceGIAS_Clienti.FiorentinaDiSopra



                        _rptFattura.Section2.SectionFormat.EnableSuppress = False
                        _rptFattura.PageHeaderSection2.SectionFormat.EnableSuppress = True
                        _rptFattura.PageHeaderSection3.SectionFormat.EnableSuppress = True

                        ', _  'enum_CodiceGIAS_Clienti.MagaBetaTester()
                        'CType(rptFattura.Section2.ReportObjects("TxtRagSocZeoli"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Intestazione_RagSoc
                        drIntestazioneNew.SuperRag_Soc = ""


                        '*************************************************************************************************
                        'RB x CRFattura2016.rpt
                        'GESTIONE CARICAMENTO LOGO DINAMICO (FATTO PER I CLIENTI CHE ATTUALMENTE SONO SU FOSFORO)
                        Select Case _progressivoGias
                            Case enum_CodiceGIAS_Clienti.AlCanevon,
                                enum_CodiceGIAS_Clienti.LaRizzola,
                                enum_CodiceGIAS_Clienti.Lorenzato,
                                enum_CodiceGIAS_Clienti.FattoriaMonticinoRosso,
                                enum_CodiceGIAS_Clienti.PoderePalazzo,
                                enum_CodiceGIAS_Clienti.Zuffa,
                                enum_CodiceGIAS_Clienti.FiorentinaDiSopra

                                CaricaLogoInCampoBlobFattura(_logErrori, drIntestazioneNew, _piva, STAMPE_CONTAB_LOGO_IN_ALTO)

                        End Select
                        '*************************************************************************************************


                        '=========================================
                        '===== LOGO IN BASSO, RAG_SOC IN ALTO=====
                        '=========================================
                    Case enum_CodiceGIAS_Clienti.Altavita,
                        enum_CodiceGIAS_Clienti.Bonzara,
                        enum_CodiceGIAS_Clienti.FattoriaIlMonte,
                        enum_CodiceGIAS_Clienti.FerrariFranco,
                        enum_CodiceGIAS_Clienti.Fisiomed,
                        enum_CodiceGIAS_Clienti.Folicello,
                        enum_CodiceGIAS_Clienti.Fruttagel,
                        enum_CodiceGIAS_Clienti.Guarini,
                        enum_CodiceGIAS_Clienti.IlPozzo,
                        enum_CodiceGIAS_Clienti.Pedroni,
                        enum_CodiceGIAS_Clienti.PodereVecciano,
                        enum_CodiceGIAS_Clienti.ChieregatoProtti,
                        enum_CodiceGIAS_Clienti.Tomasini,
                        enum_CodiceGIAS_Clienti.Fiorini,
                        enum_CodiceGIAS_Clienti.Gandolfi,
                        enum_CodiceGIAS_Clienti.Randi,
                        enum_CodiceGIAS_Clienti.Bartolini,
                        enum_CodiceGIAS_Clienti.DeFaveri,
                        enum_CodiceGIAS_Clienti.CoFruTa,
                        enum_CodiceGIAS_Clienti.Sandrin

                        _rptFattura.Section2.SectionFormat.EnableSuppress = False
                        _rptFattura.PageHeaderSection2.SectionFormat.EnableSuppress = True
                        _rptFattura.PageHeaderSection3.SectionFormat.EnableSuppress = True
                        _rptFattura.PageHeaderSection5.SectionFormat.EnableSuppress = True


                        'imposto la ragione sociale grande in alto 
                        drIntestazioneNew.SuperRag_Soc = Parametro_Intestazione_Riga1
                        'svuoto il parametro ragione sociale piccola in basso
                        Parametro_Intestazione_Riga1 = ""

                        '*************************************************************************************************
                        'RB x CRFattura2016_LB.rpt
                        'GESTIONE CARICAMENTO LOGO DINAMICO (FATTO PER I CLIENTI CHE ATTUALMENTE SONO SU FOSFORO)
                        Select Case _progressivoGias
                            Case enum_CodiceGIAS_Clienti.Guarini,
                                enum_CodiceGIAS_Clienti.Randi,
                                enum_CodiceGIAS_Clienti.Bartolini,
                                enum_CodiceGIAS_Clienti.DeFaveri,
                                enum_CodiceGIAS_Clienti.CoFruTa,
                                enum_CodiceGIAS_Clienti.Sandrin

                                CaricaLogoInCampoBlobFattura(_logErrori, drIntestazioneNew, _piva, STAMPE_CONTAB_LOGO_IN_BASSO)

                        End Select
                        '*************************************************************************************************


                    Case enum_CodiceGIAS_Clienti.OminaRomana

                        _rptFattura.Section2.SectionFormat.EnableSuppress = True
                        _rptFattura.PageHeaderSection2.SectionFormat.EnableSuppress = False
                        _rptFattura.PageHeaderSection3.SectionFormat.EnableSuppress = True
                        _rptFattura.PageHeaderSection5.SectionFormat.EnableSuppress = True

                    Case enum_CodiceGIAS_Clienti.Trombin

                        _rptFattura.Section2.SectionFormat.EnableSuppress = True
                        _rptFattura.PageHeaderSection2.SectionFormat.EnableSuppress = True
                        _rptFattura.PageHeaderSection3.SectionFormat.EnableSuppress = False
                        _rptFattura.PageHeaderSection5.SectionFormat.EnableSuppress = True

                    Case enum_CodiceGIAS_Clienti.SBTF

                        _rptFattura.Section2.SectionFormat.EnableSuppress = False
                        _rptFattura.PageHeaderSection2.SectionFormat.EnableSuppress = True
                        _rptFattura.PageHeaderSection3.SectionFormat.EnableSuppress = True
                        _rptFattura.PageHeaderSection5.SectionFormat.EnableSuppress = True

                        drIntestazioneNew.SuperRag_Soc = ""

                        'GESTIONE CARICAMENTO LOGO DINAMICO
                        CaricaLogoInCampoBlobFattura(_logErrori, drIntestazioneNew, _piva, STAMPE_CONTAB_LOGO_IN_ALTO)

                    Case enum_CodiceGIAS_Clienti.MaioranoFormaggio,
                        enum_CodiceGIAS_Clienti.MaioranoRaffaele

                        _rptFattura.Section2.SectionFormat.EnableSuppress = True
                        _rptFattura.PageHeaderSection2.SectionFormat.EnableSuppress = True
                        _rptFattura.PageHeaderSection3.SectionFormat.EnableSuppress = True
                        _rptFattura.PageHeaderSection5.SectionFormat.EnableSuppress = False


                        'Per il PDF ci vuole il logo che riproduce quello della carta intestata
                        If _qsTipoOutput = "P" Then
                            CaricaLogoInCampoBlobFattura(_logErrori, drIntestazioneNew, _piva, STAMPE_CONTAB_LOGO_HEADER)
                        Else
                            'Sezione vuota
                        End If

                    'Case enum_CodiceGIAS_Clienti.BeleCasel
                    '    _rptFattura.Section2.SectionFormat.EnableSuppress = True
                    '    _rptFattura.PageHeaderSection2.SectionFormat.EnableSuppress = True
                    '    _rptFattura.PageHeaderSection3.SectionFormat.EnableSuppress = True
                    '    _rptFattura.PageHeaderSection5.SectionFormat.EnableSuppress = False


                    Case Else

                        _rptFattura.Section2.SectionFormat.EnableSuppress = False
                        _rptFattura.PageHeaderSection2.SectionFormat.EnableSuppress = True
                        _rptFattura.PageHeaderSection3.SectionFormat.EnableSuppress = True
                        _rptFattura.PageHeaderSection5.SectionFormat.EnableSuppress = True


                        'imposto la ragione sociale grande in alto 
                        drIntestazioneNew.SuperRag_Soc = Parametro_Intestazione_Riga1
                        'svuoto il parametro ragione sociale piccola in basso
                        Parametro_Intestazione_Riga1 = ""


                End Select



                'Gestione PièDiPagina
                Select Case _progressivoGias
                    'Case enum_CodiceGIAS_Clienti.BeleCasel

                    '    _rptFattura.Section18.SectionFormat.EnableSuppress = True 'Agronica
                    '    _rptFattura.PageFooterSection3.SectionFormat.EnableSuppress = False 'BeleCasel
                    '    _rptFattura.PageFooterSection4.SectionFormat.EnableSuppress = True 'Maiorano

                    Case enum_CodiceGIAS_Clienti.MaioranoFormaggio,
                        enum_CodiceGIAS_Clienti.MaioranoRaffaele

                        _rptFattura.Section18.SectionFormat.EnableSuppress = True 'Agronica
                        _rptFattura.PageFooterSection3.SectionFormat.EnableSuppress = True 'BeleCasel
                        _rptFattura.PageFooterSection4.SectionFormat.EnableSuppress = False 'Maiorano

                        'Per il PDF ci vuole il logo che riproduce quello della carta intestata
                        If _qsTipoOutput = "P" Then
                            CaricaLogoInCampoBlobFattura(_logErrori, drIntestazioneNew, _piva, STAMPE_CONTAB_LOGO_FOOTER)
                        Else
                            'Sezione vuota
                        End If

                    Case Else

                        _rptFattura.Section18.SectionFormat.EnableSuppress = False 'Agronica
                        _rptFattura.PageFooterSection3.SectionFormat.EnableSuppress = True 'BeleCasel
                        _rptFattura.PageFooterSection4.SectionFormat.EnableSuppress = True 'Maiorano

                End Select

            End If

        Catch ex As Exception
            _logErrori &= "- Personalizzazioni Layout: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Sub AggiungiRigheDettaglioRaggruppate(ByRef ds As DataSetFattura, ByRef contatore As Integer, ByRef hashGruppo As Hashtable)

        Dim drDescrizioneNew As DataSetFattura.DescrizioneRow

        Try

            For Each key As Object In hashGruppo.Keys

                drDescrizioneNew = ds.Descrizione.NewDescrizioneRow

                drDescrizioneNew.Contatore = contatore
                drDescrizioneNew.SuperPiva = _piva
                drDescrizioneNew.Descrizione = CStr(hashGruppo.Item(key)).Split("|")(0) & " " & CStr(hashGruppo.Item(key)).Split("|")(8)
                drDescrizioneNew.Udm_Des = CStr(hashGruppo.Item(key)).Split("|")(1)

                drDescrizioneNew.Qta = CStr(hashGruppo.Item(key)).Split("|")(2) 'somma delle qta
                drDescrizioneNew.Prezzo = Format(CDec(CStr(hashGruppo.Item(key)).Split("|")(3)), "#,###,##0.00##")
                drDescrizioneNew.Sconto = CStr(hashGruppo.Item(key)).Split("|")(4)
                drDescrizioneNew.Iva = CStr(hashGruppo.Item(key)).Split("|")(5)
                drDescrizioneNew.IvaImposta = CStr(hashGruppo.Item(key)).Split("|")(6) 'non serve
                drDescrizioneNew.Importo = Format(CDec(CStr(hashGruppo.Item(key)).Split("|")(7)), "#,###,##0.00##")

                drDescrizioneNew.Extra_Str_1 = Format(CDec(CStr(hashGruppo.Item(key)).Split("|")(9)), "#,###,##0.00##")
                drDescrizioneNew.Cod_Articolo = CStr(hashGruppo.Item(key)).Split("|")(10)

                ds.Descrizione.Rows.Add(drDescrizioneNew)

                contatore += 1

            Next

        Catch ex As Exception
            _logErrori &= "- Aggiungi Righe Dettaglio Raggruppate: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

End Class
