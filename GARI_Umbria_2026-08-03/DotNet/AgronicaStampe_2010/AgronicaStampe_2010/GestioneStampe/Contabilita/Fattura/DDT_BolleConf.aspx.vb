Imports AgronicaControlli_2010
Imports AgronicaControlli_2010.UtilityPersonalizzazioniGraficheCliente
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente
Imports AgronicaCoreStampeBIZ
Imports AgronicaCoreStampeDAL
Imports AgronicaCoreUtility
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared

Partial Class DDT_BolleConf
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private _rptBolla As Object
    Private rptFooterLogo As FooterLogo
    Private _logErrori As String = ""

    Private _moduliCliente As List(Of Integer)
    Private _moduloCantine As Boolean
    Private _moduloFreshFood As Boolean
    Private _moduloTabacco As Boolean
    Private _moduloZoo As Boolean

    Private _piva As String
    Private _lavCod As Integer
    Private _idAgenda As Integer
    Private _codReport As enum_CodificaStampe
    Private _flagStampaNumeroVasca As Boolean = False
    Private _tabellaCodBase As Integer = enum_OTabelle.Nessuno

    Private _identNumero As String = ""
    Private _identData As String = ""
    Private _identContatto As String = ""
    Private _dataInizioAllegato, _dataFineAllegato As Date

    Private _objConfigStampe As ConfigurazioneStampe = Nothing
    Private _usoConfigStampe As Boolean = False
    Private _nuoviArrotondamenti As Boolean = False
    Private _tipoArrotondamentoFF As enum_TipoArrotondamentoFF = enum_TipoArrotondamentoFF.Nessuno
    Private _tipoLayout As enum_TipoLayoutDDT = enum_TipoLayoutDDT.Standard

    Private _qsTipoOutput As String = "" 'P - PDF (logo) , C - Carta intestata (No logo, ma ci sono gli spazi sopra e sotto) 

    Private flag_StampaColonnaNumColliDdtFF As Boolean = False

    Private _objParametriServer As AgronicaCoreParametri
    Private _objParametriSuperServer As AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreParametri
    Private _progressivoGias As Integer
    Private _traduttore As Traduttore_DDT = Nothing

    Dim customLoghi As PersonalizzazioniGraficheCliente = Nothing
    Dim dsLogoFooter As DS_LogoFooter

    Private Enum enum_TipoLayoutDDT
        Standard = 0
        Pesi_Reali_piu_Prezzo = 1
        Tutto_Riscontrato_se_disponibile_piu_Prezzo = 2
        Pesi_Reali_piu_Riscontrati = 3
        Dettagli_Economici = 4
    End Enum

    '####################################################################
    Private Class Parametri
        Public Property TotaleDdt As String
        Public Property TipoPeso As String
        Public Property FatturatoCessionario As String
        Public Property Conai As String
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
        Public Property VettoreRiga1 As String
        Public Property VettoreRiga2 As String
        Public Property VettoreRiga3 As String
        Public Property VettoreRiga4 As String
        Public Property TitoloDocumento As String
        Public Property TitoloStabilimento As String
        Public Property LblPivaCliente As String
        Public Property LblCfCliente As String
        Public Property TxtPivaCliente As String
        Public Property TxtCfCliente As String
        Public Property Litri As String
        Public Property AgenteInfo As String
        Public Property LblPrezzoUdmExtra As String
        Public Property RagSocAggiuntivo As String
        Public Property IndirizzoAggiuntivo As String
        Public Property CapAggiuntivo As String
        Public Property FrazioneAggiuntivo As String
        Public Property ComuneAggiuntivo As String
        Public Property ProvAggiuntivo As String
        Public Property TxtPivaAgg As String
        Public Property TxtCfAgg As String
        Public Property LblPrezzoRiscontrato As String
        Public Property QualificaSecondoContatto As String
        Public Property QualificaQuartoContatto As String
        Public Property CodiceSDI As String
        Public Property Articolo62 As String

        Private ReadOnly _traduttore As Traduttore_DDT = Nothing

        Public Sub New(ByVal traduttore As Traduttore_DDT)
            _traduttore = traduttore

            TotaleDdt = ""
            TipoPeso = ""
            FatturatoCessionario = _traduttore.ValoreDizionarioTraduzioneComuni("Cessionario:")
            Conai = ""
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
            VettoreRiga1 = ""
            VettoreRiga2 = ""
            VettoreRiga3 = ""
            VettoreRiga4 = ""
            TitoloDocumento = _traduttore.ValoreDizionarioTraduzioneComuni("DOCUMENTO DI TRASPORTO (D.d.T.)")
            TitoloStabilimento = ""
            LblPivaCliente = _traduttore.ValoreDizionarioTraduzioneComuni("Partita Iva:")
            LblCfCliente = _traduttore.ValoreDizionarioTraduzioneComuni("Codice Fiscale:")
            TxtPivaCliente = ""
            TxtCfCliente = ""
            Litri = ""
            AgenteInfo = ""
            LblPrezzoUdmExtra = ""
            RagSocAggiuntivo = ""
            IndirizzoAggiuntivo = ""
            CapAggiuntivo = ""
            FrazioneAggiuntivo = ""
            ComuneAggiuntivo = ""
            ProvAggiuntivo = ""
            TxtPivaAgg = ""
            TxtCfAgg = ""
            LblPrezzoRiscontrato = _traduttore.ValoreDizionarioTraduzioneComuni("Prezzo")
            QualificaSecondoContatto = "Cessionario:"
            QualificaQuartoContatto = "Spettabile:"
            CodiceSDI = ""

        End Sub
    End Class
    '####################################################################

#Region " DDT E BOLLE CONFERIMENTO "

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

        '  Giulia, 10/01/2017 12.00.14: ricavo piva ed id_agenda per ricavare il modulo usato e se sto usando un cod_risum che ha una personalizzazione
        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        _idAgenda = CInt(Stringa_Decodifica(CStr(Request.QueryString("i")), AgroKey_EncoderDecoder, Server))

        _codReport = CInt(Stringa_Decodifica(CStr(Request.QueryString("rep")), AgroKey_EncoderDecoder, Server))

        _lavCod = CInt(Stringa_Decodifica(CStr(Request.QueryString("l")), AgroKey_EncoderDecoder, Server))

        _objParametriServer = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        _objParametriSuperServer = New AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        _objParametriUtenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        customLoghi = UtilityPersonalizzazioniGraficheCliente.LeggiPersonalizzazioniGraficheCliente(_objParametriServer, _objParametriSuperServer)

        _progressivoGias = CInt(Session("ASG_ProgressivoGIAS"))

        'Verifica Tipologia clienti Cantine/Fresh&Food/ecc
        Dim objOmni As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
        _moduliCliente = objOmni.Recupera_Moduli_Cliente(_piva, _objParametriServer)

        _moduloCantine = _moduliCliente.Contains(enum_Omni_Modulo_Generazione.Cantine)
        _moduloFreshFood = _moduliCliente.Contains(enum_Omni_Modulo_Generazione.FreshFood)
        _moduloTabacco = _moduliCliente.Contains(enum_Omni_Modulo_Generazione.Tabacco)
        _moduloZoo = _moduliCliente.Contains(enum_Omni_Modulo_Generazione.Zoo)

        '  Giulia, 11/01/2017 10.12.44: devo ricavare il CodRisUm principale
        Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R
        Dim codRisUm As Integer = objMovimenti.Leggi_Cod_RisUm_Cessionario_From_idAgenda(_piva, _idAgenda, "", _objParametriServer)

        _nuoviArrotondamenti = UsaNuoviArrotondamenti(_objParametriServer)

        Dim objConfStampe = New Configurazione_Stampe_R
        Dim dt As DataTable = objConfStampe.Leggi(_piva, _codReport, -1, codRisUm, "", "", _objParametriServer)

        If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

            _usoConfigStampe = True
            _objConfigStampe = New ConfigurazioneStampe(dt.Rows(0))

            Dim nomeReportCrystal As String = _objConfigStampe.NomeFileRpt
            Dim classeReportCrystal As String = nomeReportCrystal.Remove((nomeReportCrystal.Length - 4), 4)

            If Not IsNothing(_objConfigStampe.Parametri_Extra_HT) AndAlso _objConfigStampe.Parametri_Extra_HT.ContainsKey("StampaColonnaNumColliDdtFF") Then
                flag_StampaColonnaNumColliDdtFF = _objConfigStampe.Parametri_Extra_HT("StampaColonnaNumColliDdtFF")
            End If

            '  Giulia, 11/01/2017 12.00.39: permette di creare dinamicamente il giusto report
            _rptBolla = Activator.CreateInstance(Type.GetType("AgronicaStampe_2010." & classeReportCrystal))

            'Select Case ClasseReportCrystal
            '    Case "CRBolla_Trombin"
            '        rptBolla = New CRBolla_Trombin
            '    Case "CRBolla2016"
            '        rptBolla = New CRBolla2016
            '    Case "CRBolla2016_LB"
            '        rptBolla = New CRBolla2016_LB
            '    Case "CRBolla"
            '        rptBolla = New CRBolla
            '    Case Else
            '        rptBolla = New CRBolla
            'End Select

        Else
            'non presente in tabella configurazione stampe oppure tabella nn è stata letta
            _usoConfigStampe = False
            _objConfigStampe = New ConfigurazioneStampe()

            Select Case _progressivoGias
                Case enum_CodiceGIAS_Clienti.Trombin,
                    enum_CodiceGIAS_Clienti.SBTF

                    _rptBolla = New CRBolla_Trombin

                Case enum_CodiceGIAS_Clienti.AlCanevon,
                    enum_CodiceGIAS_Clienti.LaRizzola,
                    enum_CodiceGIAS_Clienti.Lorenzato,
                    enum_CodiceGIAS_Clienti.FattoriaMonticinoRosso,
                    enum_CodiceGIAS_Clienti.PoderePalazzo,
                    enum_CodiceGIAS_Clienti.Zuffa,
                    enum_CodiceGIAS_Clienti.FiorentinaDiSopra


                    _rptBolla = New CRBolla2016

                Case enum_CodiceGIAS_Clienti.MaioranoFormaggio,
                    enum_CodiceGIAS_Clienti.MaioranoRaffaele


                    'PERSONALIZZAZIONE MAIORANO
                    'Se in PDF deve stampare i loghi (fatto con la stampa nuova dove i loghi sono su FileSystem)
                    'Se su carta o non specificato : la sezione deve rimanere vuota --> quindi ho tenuto la stampa vecchia (*)
                    '(*) Non si è usata quella nuova perché nella sezione del logo, avendo messo il file blob, omettendo il logo rimane comunque
                    ' la sezione con la scritta Image
                    _qsTipoOutput = Stringa_Decodifica(CStr(Request.QueryString("to")), AgroKey_EncoderDecoder, Server)

                    If _qsTipoOutput = "P" Then
                        _rptBolla = New CRBolla2016
                    Else
                        _rptBolla = New CRBolla
                    End If

                Case enum_CodiceGIAS_Clienti.Guarini,
                     enum_CodiceGIAS_Clienti.Randi,
                     enum_CodiceGIAS_Clienti.Bartolini,
                     enum_CodiceGIAS_Clienti.DeFaveri

                    _rptBolla = New CRBolla2016_LB

                Case Else
                    _rptBolla = New CRBolla

            End Select

        End If

        rptFooterLogo = New FooterLogo()
        dsLogoFooter = New DS_LogoFooter()

        Dim linguaDoc As String = OttieniLingua_ReportContabilita(_objParametriServer, _objParametriUtenti, _lavCod, _piva, _idAgenda)

        Dim tipoReport As Enum_Tipo_Report_DDT = OttieniTipoReportPerTraduttore()
        If tipoReport = Enum_Tipo_Report_DDT.Undefined Then
            linguaDoc = "it"
        End If

        _traduttore = New Traduttore_DDT(_piva,
                                   enum_CodificaStampe.Bolle,
                                   linguaDoc, tipoReport, DirectCast(_rptBolla, ReportClass), _objParametriServer)
        _traduttore.Inizializza()

    End Sub

#End Region


    '#####################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        '  Giulia, 10/01/2017 12.01.38: Spostati su Page_Init perché mi servono prima
        'Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        'Id_Agenda = CInt(Stringa_Decodifica(CStr(Request.QueryString("i")), AgroKey_EncoderDecoder, Server))

        'Cod_Report = CInt(Stringa_Decodifica(CStr(Request.QueryString("rep")), AgroKey_EncoderDecoder, Server))

        ' Gianluca , 13/05/2020 12.01.38: Spostato su Page_Init perché mi serve prima
        '_lavCod = CInt(Stringa_Decodifica(CStr(Request.QueryString("l")), AgroKey_EncoderDecoder, Server))


        'PrintToPrinter = Stringa_Decodifica(CStr(Request.QueryString("r")), AgroKey_EncoderDecoder, Server)

        'PrintName = Stringa_Decodifica(CStr(Request.QueryString("a")), AgroKey_EncoderDecoder, Server)

        _qsTipoOutput = Stringa_Decodifica(CStr(Request.QueryString("to")), AgroKey_EncoderDecoder, Server)


        'objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        '_objParametriUtenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim nomeDocumento As String = "DDT_BolleConferimento"

        If Not Me.IsPostBack Then

            Dim catCod As Integer

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim ds As New DataSetFattura

            _logErrori = ""

            Select Case _lavCod
                Case LAVCOD_BOLLA_EMESSA, LAVCOD_BOLLA_RICEVUTA
                    nomeDocumento = "DDT"
                    catCod = enum_CategorieDocumenti.DDT_Emesso
                Case LAVCOD_CONFERIMENTO
                    nomeDocumento = "Conferimento"
                    catCod = enum_CategorieDocumenti.Conferimento
                Case LAVCOD_CONFERIMENTO_DIVERSI
                    nomeDocumento = "ConferimentoDiversi"
                    catCod = enum_CategorieDocumenti.Conferimento
                Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
                    nomeDocumento = "DDT_Corrispettivi"
                    catCod = enum_CategorieDocumenti.DDT_Contabilizzato_Emesso
                Case LAVCOD_ACCETTAZIONE_DIVERSI
                    nomeDocumento = "DDT_Ricevuto"
                    catCod = enum_CategorieDocumenti.Conferimento
                Case LAVCOD_AUTO_DDT_EMESSO
                    nomeDocumento = "Auto_DDT"
                    catCod = enum_CategorieDocumenti.Conferimento
                Case LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                    nomeDocumento = "Auto_DDT"
                    catCod = enum_CategorieDocumenti.Conferimento
            End Select

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

                Stampa_DDT_BollaConferimento(ds)

            Catch exc As Exception
                _logErrori &= "- Stampa_DDT_BollaConferimento: " & vbCrLf & exc.Message & vbCrLf
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
                objGestFile.SalvaReportPdf(_rptBolla, catCod,
                                           sottoCartella, nomeFile,
                                           _objParametriServer, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim allegatiDocumentiCod As Integer = objAllegati.SalvaAllegato(_piva,
                                                                                catCod,
                                                                                "Fattura",
                                                                                nomeFile, sottoCartella,
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
                _rptBolla.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                _logErrori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            ' Giulia: 2/11/2017:Devo scrivere il file di log anche qui e poi svuotarlo, perché se va in errore il pezzo sotto
            '   esce di qui e di fatto il file di log non lo scrive mai
            SalvaLogErrori_Agenda(_logErrori, nomeDocumento, identificazioneDocumento, "DDT_BolleConf.aspx", "Stampe_Contabilita", _idAgenda, _objParametriServer)
            _logErrori = ""

            Try
                'MS Una volta persistito il report con i dati su file distruggo rpt e DataSet ed eseguo un GC.Collect
                'In questo modo la memoria e i thread non rimangono allocati e non si blocca più dopo alcune stampe.
                ds.Dispose()
                ds = Nothing

                _rptBolla.Close()
                _rptBolla.Dispose()
                _rptBolla = Nothing

                GC.Collect()

                Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                                  "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                                  "&NomePdf=" & Stringa_Codifica(nomeFile, AgroKey_EncoderDecoder, Server))
            Catch ex As Exception
                _logErrori &= "- Distruzione oggetti e redirect report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            SalvaLogErrori_Agenda(_logErrori, nomeDocumento, identificazioneDocumento, "DDT_BolleConf.aspx", "Stampe_Contabilita", _idAgenda, _objParametriServer)

        End If

    End Sub

    '#####################################################################################################
    Private Sub Stampa_DDT_BollaConferimento(ByRef ds As DataSetFattura)

        Dim drIntestazioneNew As DataSetFattura.IntestazioneFatturaRow = ds.IntestazioneFattura.NewIntestazioneFatturaRow
        Dim drDescrizioneNew As DataSetFattura.DescrizioneRow
        Dim i As Integer

        '------------------
        'sommatoria dei valori dei dettagli
        Dim newPesoNettoTotale As Decimal = 0
        Dim newPesoLordoTotale As Decimal = 0
        Dim newTaraTotale As Decimal = 0

        '------------------
        'parametri
        Dim parametro As New Parametri(_traduttore)

        'Dim Cliente_Piva As String = ""
        'Dim Cliente_CF As String = ""
        Dim Destinatario_Tel As String = ""

        '------------------
        'Impresa

        ' Dim Indirizzo_Impresa1, Indirizzo_Impresa2 As String

        'Dim x_RegImprese As String = ""
        'Dim x_Provincia_RegImprese As String = ""
        'Dim x_REA As String = ""
        'Dim x_ISO As String = ""
        'Dim x_AlboCoop As String = ""
        'Dim x_CapitaleSociale As String = ""
        'Dim x_Telefono As String = ""
        'Dim x_Fax As String = ""
        'Dim x_Cell As String = ""
        'Dim x_Email As String = ""
        'Dim x_SitoWeb As String = ""
        'Dim Sito_Email As String = ""

        '------------------
        'Movimenti
        Dim x_Cod_RisUm As Integer = 0
        Dim x_Cod_RisUm_Aggiuntivo As Integer = 0

        Dim x_ChkLayout_Join_Prodotti As Integer = 0
        'Dim x_Tipo_Sconto As Integer = 0
        Dim x_Edit_Importo As enum_EditImporto = enum_EditImporto.PrezzoUnitario


        Dim x_ChkLayOut_Litri As Integer = 0
        Dim Litri_Totali As Decimal = 0

        '------------------------------
        'Movimenti Dettagli
        Dim Riferimento_DocAllegato As String = ""
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
        Dim x_Aliquota As String = ""
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

        Dim Peso_Lordo_Dettaglio As Decimal = 0

        Dim x_Veg_Cod As Integer = 0
        Dim x_Cul_Cod As Integer = 0

        Dim x_N_Conf_Riscontrate As Integer = -1
        Dim x_N_Colli_Riscontrati As Integer = -1
        Dim x_N_Imballi_Riscontrati As Integer = -1
        Dim x_Peso_Netto_Riscontrato As Decimal = 0
        Dim x_Peso_Lordo_Riscontrato As Decimal = 0
        Dim x_Tara_Unit_Collo_Riscontrata As Decimal = -1
        Dim x_Tara_Unit_Imballo_Riscontrata As Decimal = -1


        'Dim Indirizzo_Fabbricato As String
        'Dim x_Fabbricato_Des() As String
        'Dim x_IndDes_Fabbricato() As String
        'Dim x_FrzDes_Fabbricato() As String
        'Dim x_Cap_Fabbricato() As String
        'Dim x_Comune_Fabbricato() As String
        'Dim x_Provincia_Fabbricato() As String

        '------------------------------
        'Dati 
        Dim Riga_Sconto As String = ""
        Dim Riga_Sconto_Omaggio As String = ""
        Dim Riga_Importo As String = ""
        Dim Importo_Riga_DDT As String = ""
        Dim Importo_Dettaglio As Decimal = 0

        Dim Flag_Campioni_Omaggio As Boolean = False
        Dim Flag_Campioni_Omaggio_Rivalsa_Iva As Boolean = False
        Dim Flag_Campioni_Gratuiti As Boolean = False
        Dim Flag_Sconto_Merce As Boolean = False

        Dim Flag_AlmenoUnoProdottoAgroAlimentare As Boolean = False
        Dim Flag_ProdottoAgroAlimentare As Boolean = False
        Dim Flag_ClientePrivato As Boolean = False
        'Dim Flag_StampaRiepilogoImballi As Boolean = False

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

        Dim Flag_UveDiraspate As Boolean = False
        Dim Rif_Ordine As String

        Try

            Select Case _lavCod
                Case LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                    parametro.FatturatoCessionario = _traduttore.ValoreDizionarioTraduzioneComuni("Cessionario: ")
                    parametro.TitoloDocumento = _traduttore.ValoreDizionarioTraduzioneComuni("AUTO - DOCUMENTO DI TRASPORTO (D.d.T.)")
            End Select



            'Try

            '    'MODIFICATA GESTIONE INTESTAZIONE IN DATA 31/08/2015

            '    '############################################################################################
            '    '######################## INTESTAZIONE DEL DOCUMENTO DI TRASPORTO ###########################
            '    '############################################################################################

            '    Leggi_Intestazione_Impresa(objParametri_Server,
            '                                _progressivoGias,
            '                                False,
            '                                False,
            '                                Piva,
            '                                Log_Errori,
            '                                x_RagSoc_Impresa,
            '                                x_CodContatto_Impresa,
            '                                x_CodiceFiscale_Impresa,
            '                                x_IndDes_Impresa,
            '                                x_FrzDes_Impresa,
            '                                x_Cap_Impresa,
            '                                x_Comune_Impresa,
            '                                x_Provincia_Impresa,
            '                                x_RegImprese,
            '                                x_Provincia_RegImprese,
            '                                x_REA,
            '                                x_ISO,
            '                                x_AlboCoop,
            '                                x_CapitaleSociale,
            '                                x_Telefono,
            '                                x_Fax,
            '                                x_Cell,
            '                                x_Email,
            '                                x_SitoWeb,
            '                                x_Fabbricato_Des,
            '                                x_IndDes_Fabbricato,
            '                                x_FrzDes_Fabbricato,
            '                                x_Cap_Fabbricato,
            '                                x_Comune_Fabbricato,
            '                                x_Provincia_Fabbricato,
            '                                Nothing,
            '                                Nothing,
            '                                Nothing,
            '                                Nothing,
            '                                Nothing,
            '                                Nothing,
            '                                Nothing,
            '                                Nothing,
            '                                Nothing,
            '                                Nothing,
            '                                Nothing,
            '                                Nothing,
            '                                Nothing,
            '                                x_Stato_Impresa)

            'Catch ex As Exception
            '    Log_Errori &= "- Lettura dei dati dell'intestazione dell'impresa: " & vbCrLf & ex.Message & vbCrLf
            'End Try

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
                    'Parametro_CONAI = "Contributo ambientale Conai assolto ove dovuto"
                    '29/02/2016 - Modificato label per Trombin
                    parametro.Conai = _traduttore.ValoreDizionarioTraduzioneComuni("CONTRIBUTO CONAI ASSOLTO OVE DOVUTO")
                Else
                    parametro.Conai = ""
                End If

            Catch ex As Exception
                _logErrori &= "- Lettura impostazioni utente: " & vbCrLf & ex.Message & vbCrLf
            End Try

            '/******************* impostazione x ZESPRI *****************
            'Indirizzo di spedizione preso dal centro 
            'togliere kg mettendo numero
            'Per quest'ultima cosa c'è una nuova impostazione superuser 848; se è 1 devi mostrare numero invece di Kg
            'E' stata fatta per utilizzare le piantine come trasformati vegetali





            Dim Dt_Documento As DataTable
            '####################################################
            '               CREA INTESTAZIONE
            '####################################################

            drIntestazioneNew = CreaDrIntestazione(_logErrori, ds, Dt_Documento, x_Edit_Importo,
                                                   x_ChkLayout_Join_Prodotti, x_ChkLayOut_Litri,
                                                   x_Cod_RisUm, x_Cod_RisUm_Aggiuntivo,
                                                   Flag_ClientePrivato, Destinatario_Tel, parametro,
                                                   Flag_UveDiraspate,
                                                   Flag_GestMaterialeVivaistico,
                                                   Flag_CentroAziendalePartenza)

            '####################################################

            'Sono costretta a posticipare l'aggiunta dell'intestazione perché se ho layout totalmente riscontrato
            'potrebbe essere necessario sostituire i valori di peso netto, lordo e tara di testata

            'ds.IntestazioneFattura.Rows.Add(drIntestazioneNew)

            '####################################################

            Dim objLanRound As New AgronicaCoreContabHLP.GiasLan_Round
            Dim Dt_Dettagli_Round As DataTable = Nothing
            Dim Dt_Riepilogo_IVA As DataTable
            Dim Riepilogo_ImponibileLordo As Decimal = 0
            Dim Riepilogo_Variazioni As Decimal = 0
            Dim Riepilogo_ImponibileNetto As Decimal = 0
            Dim Riepilogo_Imposta As Decimal = 0
            Dim Riepilogo_Importo As Decimal = 0

            '====================================================================================

            'Dim flag_Contenitori_Imballi As Boolean
            '====================================================================================
            '---------------- Prelevo le Informazioni sui dettagli ----------------------------

            Dim Flag_Raggruppa As Boolean
            Dim hashGruppo As Hashtable = Nothing
            Dim Nome_Calibro As String = ""


            If x_ChkLayout_Join_Prodotti = 1 Then
                Flag_Raggruppa = True
                hashGruppo = New Hashtable
            Else
                Flag_Raggruppa = False
            End If

            Dim pesiEsplicitatiInCorpo As Integer = 0
            Select Case _tipoLayout
                Case enum_TipoLayoutDDT.Pesi_Reali_piu_Prezzo, enum_TipoLayoutDDT.Pesi_Reali_piu_Riscontrati, enum_TipoLayoutDDT.Tutto_Riscontrato_se_disponibile_piu_Prezzo
                    pesiEsplicitatiInCorpo = 1
                Case Else
                    pesiEsplicitatiInCorpo = 0
            End Select

            'mi tengo un certo margine per evitare che mi venga un contatore uguale
            Dim numContatoreBasePerRif As Integer = (Dt_Documento.Rows.Count - 1) + 10

            'Dim Flag_Partenza_CentroAzi As Boolean
            'Dim Partenza_CentroAzi As String = ""
            Dim Partenza_SaCod As Integer = 0
            'Dim objSaNome As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            ImpostazioniImpresaUtente_Articolo62ElemCod(_objParametriServer, _objParametriUtenti, _logErrori, _piva, 0, impArticolo62ElemCod)

            For i = 0 To Dt_Documento.Rows.Count - 1
                Try
                    'azzero ad ogni giro
                    numColli_contenitori = 0
                    contenitore_cod = 0
                    descr_contenitore = ""
                    numContenitori_imballi = 0
                    imballaggio_cod = 0
                    descr_imballo = ""
                    Flag_DDTallegati = False
                    ChkConfezione = 0
                    ChkContenitore = 0
                    ChkImballaggio = 0
                    x_Prezzo_Livello = 0
                    Dettaglio_Evasione = ""
                    tipologiaRiga = enum_TipoRigaFattura.NonSpecificato

                    If Partenza_SaCod = 0 And Dt_Documento.Rows(i).Item("Sa_Cod_Dett") <> 0 And Dt_Documento.Rows(i).Item("Ordine_Det") <> 1000 Then
                        'Partenza_CentroAzi = objSaNome.SaNome_from_SaCod(_piva, Dt_Documento.Rows(i).Item("Sa_Cod_Dett"), _objParametriServer)
                        Partenza_SaCod = Dt_Documento.Rows(i).Item("Sa_Cod_Dett")
                    End If

                    'se il layout non è il peso, allora passo quello riscontrato (perché anche questo ha già tutti i pesi, quindi è equiparabile a quello peso)
                    Leggi_MovimentoDettaglio_DocContabile(_objParametriServer,
                                                          _objParametriUtenti,
                                                          _moduliCliente,
                                                          _logErrori,
                                                          Dt_Documento.Rows(i),
                                                          _piva,
                                                          _lavCod,
                                                          Flag_Raggruppa,
                                                          pesiEsplicitatiInCorpo,
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
                                                          x_Aliquota,
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
                                                          False,
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
                                                          "", 0,
                                                          Flag_UveDiraspate,
                                                          flagTareRiscontrateInDesc:=If(_tipoLayout = enum_TipoLayoutDDT.Tutto_Riscontrato_se_disponibile_piu_Prezzo, True, False),
                                                          Rif_Ordine:=Rif_Ordine,
                                                          Flag_GradoAlcolico:=Flag_GradoAlcolico,
                                                          Flag_GestMaterialeVivaistico:=Flag_GestMaterialeVivaistico)

                    '-------------------------------------------------
                    '------ Identifico il TIPO DI RIGA DETTAGLIO -----
                    '-------------------------------------------------
                    tipologiaRiga = GetTipologiaRiga(x_Elem_Cod, x_Mov_Det_Des, Flag_Raggruppa, ChkConfezione, ChkContenitore, ChkImballaggio, Dt_Documento.Rows(i).Item("Ordine_Det"), _logErrori)


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


                    '  Giulia, 20/01/2017 09:31:41: uniformata gestione importi come la fattura (vari ricalcoli come fa il lan)
                    '   perché ora (per il momento solo FF) la stampa del ddt con dettagli economici passa di qui, 
                    '   non più con lo stesso giro della fattura accompagnatoria


                    If _nuoviArrotondamenti = True Then
                        Dt_Riepilogo_IVA = objLanRound.FormAggiornaImportoNEW(_objParametriServer,
                                                                              Dt_Dettagli_Round,
                                                                              Riepilogo_ImponibileLordo,
                                                                              Riepilogo_Variazioni,
                                                                              Riepilogo_ImponibileNetto,
                                                                              Riepilogo_Imposta,
                                                                              Riepilogo_Importo,
                                                                              x_Edit_Importo,
                                                                              False,
                                                                              False,
                                                                               enum_EsigibilitaIva.Non_Specificata)
                    Else
                        'ad ogni dettaglio ricalcolo gli importi e aggiorno il dtIva
                        Dt_Riepilogo_IVA = objLanRound.FormAggiornaImporto(_objParametriServer,
                                                                           Dt_Dettagli_Round,
                                                                           Riepilogo_ImponibileLordo,
                                                                           Riepilogo_Variazioni,
                                                                           Riepilogo_ImponibileNetto,
                                                                           Riepilogo_Imposta,
                                                                           Riepilogo_Importo,
                                                                           x_Edit_Importo)
                    End If

                    '16/05/2017
                    If tipologiaRiga = enum_TipoRigaFattura.DescrizioneLibera Then
                        Importo_Dettaglio = 0
                    Else
                        Importo_Dettaglio = Dt_Dettagli_Round.Select("Id_mov_det = " & Agro_SQL_SaveNum(Dt_Documento.Rows(i).Item("id_mov_det")))(0).Item("importo_totale")
                    End If

                    Peso_Lordo_Dettaglio = x_Tara + x_Qta_Extra_Totale

                    '===================================================================

                    Select Case x_Sconto_Modalita

                        Case enModalitaSconto.Percentuale
                            'Riga_Sconto_Omaggio = ""
                            '   CASO NORMALE
                            'If x_Sconto_Perc <> 0 Then
                            '    Riga_Sconto = "-" & CStr(x_Sconto_Perc) & "%"
                            'Else
                            '    Riga_Sconto = "" 'vanni, 02/09/2011, baco su riga fattura con sconto = 0
                            'End If

                            'If Trim(x_Sconto_Testo) = "" Then
                            '    If x_Sconto_Perc_2 <> 0 Then
                            '        Riga_Sconto &= " -" & CStr(x_Sconto_Perc_2) & "%"
                            '    End If
                            'Else
                            '    Riga_Sconto &= CStr(x_Sconto_Testo) '& "%"
                            'End If

                            Riga_Sconto_Omaggio = ""
                            Riga_Importo = Format(x_Imponibile_Netto, "#,###,##0.00")
                            Riga_Sconto = Composizione_Stringa_Sconti(x_Sconto_Perc, x_Sconto_Perc_2, x_Sconto_Testo)
                            Importo_Riga_DDT = Format(Importo_Dettaglio, "#,###,##0.00##")

                        Case enModalitaSconto.Sconto_Merce
                            '   CASO SCONTO MERCE
                            Riga_Importo = _traduttore.ValoreDizionarioTraduzioneComuni("Sconto Merce")
                            Riga_Sconto = ""
                            'Riga_Sconto_Omaggio = "          Sconto Merce"
                            Riga_Sconto_Omaggio = _traduttore.ValoreDizionarioTraduzioneComuni("Sconto Merce")
                            Flag_Sconto_Merce = True
                            Importo_Riga_DDT = ""

                        Case enModalitaSconto.Omaggio_SenzaRivalsaIva
                            '   CASO CAMPIONI OMAGGIO SENZA RIVALSA IVA
                            Riga_Importo = _traduttore.ValoreDizionarioTraduzioneComuni("Omaggio senza rivalsa IVA")
                            Riga_Sconto = ""
                            'Riga_Sconto_Omaggio = "          Campione Omaggio senza rivalsa IVA"
                            Riga_Sconto_Omaggio = _traduttore.ValoreDizionarioTraduzioneComuni("Campione Omaggio senza rivalsa IVA")
                            Flag_Campioni_Omaggio = True
                            Importo_Riga_DDT = Format(Importo_Dettaglio, "#,###,##0.00##")

                        Case enModalitaSconto.Omaggio_ConRivalsaIva
                            '   CASO CAMPIONI OMAGGIO CON RIVALSA IVA
                            Riga_Importo = _traduttore.ValoreDizionarioTraduzioneComuni("Omaggio con rivalsa IVA")
                            Riga_Sconto = ""
                            'Riga_Sconto_Omaggio = "          Campione Omaggio con rivalsa IVA"
                            Riga_Sconto_Omaggio = _traduttore.ValoreDizionarioTraduzioneComuni("Campione Omaggio con rivalsa IVA")
                            Flag_Campioni_Omaggio = True
                            Importo_Riga_DDT = Format(Importo_Dettaglio, "#,###,##0.00##")

                        Case enModalitaSconto.Campioni_Gratuiti
                            '   CASO CAMPIONI GRATUITI
                            'Riga_Sconto_Omaggio = "          Campione Gratuito"
                            Riga_Importo = _traduttore.ValoreDizionarioTraduzioneComuni("Gratuito")
                            Riga_Sconto_Omaggio = _traduttore.ValoreDizionarioTraduzioneComuni("Campione Gratuito")
                            Flag_Campioni_Gratuiti = True

                    End Select

                    '===================================================================


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
                                    x_Udm_Sim = _traduttore.ValoreDizionarioTraduzioneComuni("U di Seme")
                                End If
                                If x_Udm_Cod_Extra = enum_UnitaMisura.Unita_Seme Then
                                    x_Udm_Sim_Extra = _traduttore.ValoreDizionarioTraduzioneComuni("U di Seme")
                                End If
                        End Select

                        '  Giulia, 29/12/2016 16.35.15: Personalizzazione CofruTa - Opera
                        If _progressivoGias = enum_CodiceGIAS_Clienti.CoFruTa AndAlso
                           _usoConfigStampe = True AndAlso
                           Not String.IsNullOrEmpty(_objConfigStampe.CodRisUm_PrimoCessionario) AndAlso
                           _objConfigStampe.CodRisUm_PrimoCessionario.Contains("|" & x_Cod_RisUm & "|") AndAlso
                           _lavCod = LAVCOD_BOLLA_EMESSA Then

                            'reset dei campi prezzo
                            x_Imponibile = 0
                            x_Imponibile_Netto = 0
                            x_Prezzo_Effettivo = 0
                            x_Prezzo_Unitario = 0
                            x_Prezzo_Unitario_Netto = 0

                        End If

                    Catch ex As Exception
                        _logErrori &= "- Personalizzazioni Doc Contabili Riga: " & vbCrLf & ex.Message & vbCrLf
                    End Try


                Catch ex As Exception
                    _logErrori &= "- Lettura dei dettagli del DDT: " & vbCrLf & ex.Message & vbCrLf
                End Try

                '===================================================================

                Try
                    'se x_ChkLayOut_Hide=1 non si vuole visualizzare il dettaglio
                    If x_ChkLayOut_Hide <> 1 Then

                        'se si vuole visualizzare i dettagli così come sono
                        If Flag_Raggruppa = False Then
                            '------------------------------------
                            '------ VISUALIZZAZIONE NORMALE -----
                            '------------------------------------

                            '======================================================================
                            '      Riferimento Doc Allegato
                            '======================================================================
                            AggiungiRiferimentoDocAllegato(_logErrori, ds, Flag_Raggruppa, Riferimento_DocAllegato,
                                                           i, Ultimo_DocRiferimento, numContatoreBasePerRif, _piva,
                                                           _objConfigStampe, True, _moduloFreshFood, tipologiaRiga)

                            '  Giulia, 26/09/2016 11.35.22: devo stampare anche i litri (qta_extra_totale)
                            If x_ChkLayOut_Litri = 1 Then
                                Litri_Totali += ConteggioLitriDettaglio(x_Descrizione, x_Udm_Cod, x_Udm_Cod_Extra, x_Qta, x_Qta_Extra_Totale)
                            End If


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
                                '  Giulia, 20/01/2017 12:19:29: RI-ORGANIZZAZIONE DI QUESTA PARTE, IN MANIERA DA RENDERLA PIù LEGGIBILE
                                '       E NON SCRIVERE E RI-SCRIVERE INUTILMENTE I DATI A SECONDA DELLE CASISTICHE.
                                '       PER SICUREZZA LASCIO TUTTO COMMENTATO SOTTO

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
                                            drDescrizioneNew.Prezzo = ""
                                            drDescrizioneNew.Prezzo_Netto = ""
                                            drDescrizioneNew.Importo = ""
                                            drDescrizioneNew.Sconto = ""
                                            drDescrizioneNew.Iva = ""
                                            drDescrizioneNew.Extra_Str_1 = ""
                                            drDescrizioneNew.Extra_Str_2 = ""
                                            drDescrizioneNew.Extra_Str_3 = ""
                                            drDescrizioneNew.Extra_Str_4 = ""
                                            drDescrizioneNew.Extra_Str_5 = ""

                                        Case Else
                                            'per tutti i prodotti tranne gli imballaggi

                                            '  Giulia, 17/01/2017 18:11:25: ora non devo più basarmi sull'anagrafica, ma sui valori stabiliti direttamente sul prodotto
                                            '           (colonna Livello_Prezzo: 4 = IMBALLAGGIO, 5 = CONFEZIONE, 8 = CONTENITORE)


                                            '======================================================================
                                            '      Impostazioni Prezzi e Qta per FF
                                            '======================================================================
                                            If _nuoviArrotondamenti = True Then
                                                CalcolaPrezziFFLivello_NEW(x_Prezzo_Livello, drDescrizioneNew, Prezzo_Unitario_REALE, Prezzo_Unitario_Netto_REALE,
                                                                           x_Qta, x_Qta_Extra, x_Prezzo_Unitario, x_Prezzo_Unitario_Netto, x_Prezzo_Effettivo,
                                                                           x_Imponibile, x_Imponibile_Netto, x_Udm_Sim, x_Udm_Sim_Extra, x_Flag_Extra,
                                                                           numColli_contenitori, numContenitori_imballi,
                                                                           _tipoArrotondamentoFF, flagPrezzoAlKgUdmKg:=False)
                                            Else
                                                CalcolaPrezziFFLivello(x_Prezzo_Livello, drDescrizioneNew, Prezzo_Unitario_REALE, Prezzo_Unitario_Netto_REALE,
                                                                       x_Qta, x_Qta_Extra, x_Prezzo_Unitario, x_Prezzo_Unitario_Netto, x_Prezzo_Effettivo,
                                                                       x_Imponibile, x_Imponibile_Netto, x_Udm_Sim, x_Udm_Sim_Extra, x_Flag_Extra,
                                                                       numColli_contenitori, numContenitori_imballi)
                                            End If

                                            '======================================================================
                                            '      Sovrascrivi Valori Riscontrati
                                            '======================================================================
                                            If _tipoLayout = enum_TipoLayoutDDT.Pesi_Reali_piu_Riscontrati OrElse
                                               _tipoLayout = enum_TipoLayoutDDT.Tutto_Riscontrato_se_disponibile_piu_Prezzo Then
                                                If x_Prezzo_Livello = enum_OTabelle.Confezione AndAlso x_N_Conf_Riscontrate <> -1 Then
                                                    'ho cambiato il valore delle confezioni e sto stampando i valori riscontrati
                                                    drDescrizioneNew.Qta = Format(x_N_Conf_Riscontrate, "#,###,##0.###")
                                                ElseIf x_Prezzo_Livello = enum_OTabelle.Contenitore AndAlso x_N_Colli_Riscontrati <> -1 Then
                                                    'ho cambiato il valore dei colli e sto stampando i valori riscontrati
                                                    drDescrizioneNew.Qta = Format(x_N_Colli_Riscontrati, "#,###,##0.###")
                                                ElseIf x_Prezzo_Livello = enum_OTabelle.Imballaggio AndAlso x_N_Imballi_Riscontrati <> -1 Then
                                                    'ho cambiato il valore degli imballi e sto stampando i valori riscontrati
                                                    drDescrizioneNew.Qta = Format(x_N_Imballi_Riscontrati, "#,###,##0.###")
                                                End If
                                            End If


                                            '  Giulia, 20/02/2017 11:13:41: se il prezzo è 0 non devo stampare 0/udm ma stringa vuota
                                            If _progressivoGias = enum_CodiceGIAS_Clienti.CoFruTa AndAlso
                                               (Prezzo_Unitario_Netto_REALE = 0 OrElse x_Prezzo_Effettivo = 0) Then
                                                drDescrizioneNew.Extra_Str_4 = ""
                                            End If

                                            If numColli_contenitori <> 0 Then
                                                drDescrizioneNew.Extra_Str_5 = CStr(numColli_contenitori)
                                            ElseIf numContenitori_imballi <> 0 Then
                                                drDescrizioneNew.Extra_Str_5 = CStr(numContenitori_imballi)
                                            Else
                                                drDescrizioneNew.Extra_Str_5 = ""
                                            End If

                                            drDescrizioneNew.Prezzo = Format(Prezzo_Unitario_REALE, "#,###,##0.00##")
                                            drDescrizioneNew.Prezzo_Netto = Format(Prezzo_Unitario_Netto_REALE, "#,###,##0.00##")

                                            'DrDescrizioneNew.Importo = Format(x_Imponibile_Netto, "#,###,##0.00")
                                            drDescrizioneNew.Importo = Riga_Importo
                                            drDescrizioneNew.Sconto = Riga_Sconto
                                            drDescrizioneNew.Iva = x_Aliquota

                                            If x_Elem_Cod = BENI_CONFEZ_VEGETALE Then
                                                drDescrizioneNew.Extra_Str_1 = ""
                                                drDescrizioneNew.Extra_Str_2 = ""
                                                drDescrizioneNew.Extra_Str_3 = ""
                                            Else
                                                If _tipoLayout = enum_TipoLayoutDDT.Tutto_Riscontrato_se_disponibile_piu_Prezzo Then
                                                    drDescrizioneNew.Extra_Str_1 = FormattaArrotondaFF(_tipoArrotondamentoFF, x_Peso_Netto_Riscontrato, "#,###,##0.00#")
                                                    drDescrizioneNew.Extra_Str_3 = FormattaArrotondaFF(_tipoArrotondamentoFF, x_Peso_Lordo_Riscontrato, "#,###,##0.00#")
                                                    'devo fare così sennò rischierei: Lordo = 16, Tara = 4, Netto = 13 a causa dei possibili arrotondamenti
                                                    drDescrizioneNew.Extra_Str_2 = FormattaArrotondaFF(_tipoArrotondamentoFF, CDec(drDescrizioneNew.Extra_Str_3) - CDec(drDescrizioneNew.Extra_Str_1), "#,###,##0.00#")
                                                Else
                                                    drDescrizioneNew.Extra_Str_1 = FormattaArrotondaFF(_tipoArrotondamentoFF, x_Qta_Extra_Totale, "#,###,##0.00#")
                                                    drDescrizioneNew.Extra_Str_2 = FormattaArrotondaFF(_tipoArrotondamentoFF, x_Tara, "#,###,##0.00#")
                                                    drDescrizioneNew.Extra_Str_3 = FormattaArrotondaFF(_tipoArrotondamentoFF, Peso_Lordo_Dettaglio, "#,###,##0.00#")
                                                End If

                                                newPesoNettoTotale += CDec(drDescrizioneNew.Extra_Str_1)
                                                newPesoLordoTotale += CDec(drDescrizioneNew.Extra_Str_3)
                                                newTaraTotale += CDec(drDescrizioneNew.Extra_Str_2)
                                            End If

                                    End Select

                                Else    ' NO FRESH & FOOD

                                    '-------------------------------------------------------
                                    '---- CANTINE, ZOO, OLIO, TABACCO, NO OMNI -------------
                                    '-------------------------------------------------------

                                    'nel caso di layout stampa pesi o anagrafica senza udm aspetto default o stampa personalizzata trombin
                                    If _tipoLayout = enum_TipoLayoutDDT.Pesi_Reali_piu_Prezzo OrElse x_Flag_Extra = 0 Then
                                        drDescrizioneNew.Udm_Des = x_Udm_Sim
                                        drDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")
                                        If x_Udm_Sim_Extra <> "" Then
                                            parametro.LblPrezzoUdmExtra = "/" & x_Udm_Sim_Extra
                                        End If
                                    End If

                                    If x_Flag_Extra = 1 Then
                                        'anagrafica con impostazione udm aspetto default
                                        drDescrizioneNew.Udm_Des = x_Udm_Sim_Extra
                                        drDescrizioneNew.Qta = Format((x_Qta * x_Qta_Extra), "#,###,##0.###")
                                    End If

                                    Select Case tipologiaRiga

                                        Case enum_TipoRigaFattura.RiepilogoConfezioni,
                                            enum_TipoRigaFattura.RiepilogoContenitori,
                                            enum_TipoRigaFattura.RiepilogoImballaggi

                                            'per gli imballaggi
                                            drDescrizioneNew.Prezzo = ""
                                            drDescrizioneNew.Prezzo_Netto = ""
                                            drDescrizioneNew.Importo = ""
                                            drDescrizioneNew.Sconto = ""
                                            drDescrizioneNew.Iva = ""
                                            drDescrizioneNew.Extra_Str_1 = ""
                                            drDescrizioneNew.Extra_Str_2 = ""
                                            drDescrizioneNew.Extra_Str_3 = ""
                                            drDescrizioneNew.Extra_Str_4 = ""
                                            drDescrizioneNew.Extra_Str_5 = ""

                                        Case Else

                                            'per tutti i prodotti tranne gli imballaggi

                                            drDescrizioneNew.Prezzo = Format(x_Prezzo_Unitario, "#,###,##0.00##")
                                            drDescrizioneNew.Prezzo_Netto = Format(x_Prezzo_Unitario_Netto, "#,###,##0.00##")
                                            'DrDescrizioneNew.Importo = Format(x_Imponibile_Netto, "#,###,##0.00")
                                            drDescrizioneNew.Importo = Riga_Importo
                                            drDescrizioneNew.Sconto = Riga_Sconto
                                            drDescrizioneNew.Iva = x_Aliquota
                                            drDescrizioneNew.Extra_Str_1 = Format(x_Qta_Extra_Totale, "#,###,##0.00##")
                                            'DrDescrizioneNew.Extra_Str_1 = Importo_Riga_DDT
                                            drDescrizioneNew.Extra_Str_2 = Format(x_Tara, "#,###,##0.00##")
                                            drDescrizioneNew.Extra_Str_3 = Format(Peso_Lordo_Dettaglio, "#,###,##0.00##")
                                            drDescrizioneNew.Extra_Str_4 = Format(x_Prezzo_Effettivo, "#,###,##0.00##")
                                            drDescrizioneNew.Extra_Str_5 = ""

                                            newPesoNettoTotale += CDec(drDescrizioneNew.Extra_Str_1)
                                            newPesoLordoTotale += CDec(drDescrizioneNew.Extra_Str_3)
                                            newTaraTotale += CDec(drDescrizioneNew.Extra_Str_2)

                                    End Select

                                End If  ' FRESH & FOOD


                                ''-------------------------------
                                ''---- FRESH & FOOD ---------
                                ''-------------------------------
                                'If Modulo_FreshFood = True Then

                                '    If x_Flag_Extra = 1 Then
                                '        'anagrafica con impostazione udm aspetto default
                                '        DrDescrizioneNew.Udm_Des = x_Udm_Sim_Extra
                                '        DrDescrizioneNew.Qta = Format((x_Qta * x_Qta_Extra), "#,###,##0.####")
                                '    End If

                                'Else

                                '    'nel caso di layout stampa pesi o anagrafica senza udm aspetto default o stampa personalizzata trombin
                                '    If _tipoLayout = enum_TipoLayoutDDT.Pesi_Reali_piu_Prezzo Or x_Flag_Extra = 0 Then
                                '        DrDescrizioneNew.Udm_Des = x_Udm_Sim
                                '        DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")
                                '        If x_Udm_Sim_Extra <> "" Then
                                '            Parametro_LblPrezzoUdmExtra = "/" & x_Udm_Sim_Extra
                                '        End If
                                '    End If

                                '    If x_Flag_Extra = 1 Then
                                '        'anagrafica con impostazione udm aspetto default
                                '        DrDescrizioneNew.Udm_Des = x_Udm_Sim_Extra
                                '        DrDescrizioneNew.Qta = Format((x_Qta * x_Qta_Extra), "#,###,##0.####")
                                '    End If

                                'End If



                                '''nel caso di layout stampa pesi o anagrafica senza udm aspetto default o stampa personalizzata trombin
                                ''If _tipoLayout = enum_TipoLayoutDDT.Pesi_Reali_piu_Prezzo Or x_Flag_Extra = 0 Then

                                ''    '-------------------------------
                                ''    '---- FRESH & FOOD ---------
                                ''    '-------------------------------
                                ''    '03/05/2016
                                ''    'CASISTICA GESTITA SOTTO

                                ''    '----------------------------------------
                                ''    '---- CANTINE, TABACCO, ZOO, NO OMNI ---------
                                ''    '-----------------------------------------
                                ''    If Modulo_FreshFood = False Then
                                ''        DrDescrizioneNew.Udm_Des = x_Udm_Sim
                                ''        DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")
                                ''        If x_Udm_Sim_Extra <> "" Then
                                ''            Parametro_LblPrezzoUdmExtra = "/" & x_Udm_Sim_Extra
                                ''        End If
                                ''    End If

                                ''Else
                                ''    'flag_extra = 1

                                ''    'anagrafica con impostazione udm aspetto default
                                ''    DrDescrizioneNew.Udm_Des = x_Udm_Sim_Extra
                                ''    DrDescrizioneNew.Qta = Format((x_Qta * x_Qta_Extra), "#,###,##0.####")
                                ''End If

                                'If flag_Contenitori_Imballi = False Then
                                '    'per tutti i prodotti tranne gli imballaggi
                                '    DrDescrizioneNew.Prezzo = Format(x_Prezzo_Unitario, "#,###,##0.00##")
                                '    DrDescrizioneNew.Prezzo_Netto = Format(x_Prezzo_Unitario_Netto, "#,###,##0.00##")
                                '    DrDescrizioneNew.Importo = Format(x_Imponibile_Netto, "#,###,##0.00")
                                '    DrDescrizioneNew.Sconto = Riga_Sconto
                                '    DrDescrizioneNew.Iva = x_Aliquota
                                '    DrDescrizioneNew.Extra_Str_1 = Format(x_Qta_Extra_Totale, "#,###,##0.00##")
                                '    DrDescrizioneNew.Extra_Str_2 = Format(x_Tara, "#,###,##0.00##")
                                '    DrDescrizioneNew.Extra_Str_3 = Format(Peso_Lordo_Dettaglio, "#,###,##0.00##")
                                '    DrDescrizioneNew.Extra_Str_4 = Format(x_Prezzo_Effettivo, "#,###,##0.00##")
                                'Else
                                '    'per gli imballaggi
                                '    DrDescrizioneNew.Prezzo = ""
                                '    DrDescrizioneNew.Prezzo_Netto = ""
                                '    DrDescrizioneNew.Importo = ""
                                '    DrDescrizioneNew.Sconto = ""
                                '    DrDescrizioneNew.Iva = ""
                                '    DrDescrizioneNew.Extra_Str_1 = ""
                                '    DrDescrizioneNew.Extra_Str_2 = ""
                                '    DrDescrizioneNew.Extra_Str_3 = ""
                                '    DrDescrizioneNew.Extra_Str_4 = ""
                                'End If

                                ''-------------------------------
                                ''---- FRESH & FOOD ---------
                                ''-------------------------------
                                'If Modulo_FreshFood = True Then

                                '    Dim Prezzo_Unitario_REALE As Decimal = 0
                                '    Dim Prezzo_Unitario_Netto_REALE As Decimal = 0

                                '    If ChkContenitore <> 1 And ChkImballaggio <> 1 Then

                                '        '  Giulia, 17/01/2017 18:11:25: ora non devo più basarmi sull'anagrafica, ma sui valori stabiliti direttamente sul prodotto
                                '        '           (colonna Livello_Prezzo: 4 = IMBALLAGGIO, 5 = CONFEZIONE, 8 = CONTENITORE)


                                '        ''verifico l'udm principale, utilizzata come base per i calcoli
                                '        'Select Case x_OTabella_Cod_Base

                                '        '    Case enum_OTabelle.Confezione
                                '        '        DrDescrizioneNew.Udm_Des = "n"
                                '        '        DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")

                                '        '    Case enum_OTabelle.Imballaggio
                                '        '        DrDescrizioneNew.Udm_Des = "n"
                                '        '        DrDescrizioneNew.Qta = Format(numContenitori_imballi, "#,###,##0.####")

                                '        '    Case enum_OTabelle.Contenitore
                                '        '        DrDescrizioneNew.Udm_Des = "n"
                                '        '        DrDescrizioneNew.Qta = Format(numColli_contenitori, "#,###,##0.####")

                                '        '    Case Else
                                '        '        'quando non è specificato nulla
                                '        '        DrDescrizioneNew.Udm_Des = x_Udm_Sim
                                '        '        DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")

                                '        'End Select

                                '        'DrDescrizioneNew.Udm_Des = x_Udm_Sim
                                '        'DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")

                                '        Select Case x_Livello_Prezzo

                                '            Case enum_OTabelle.Confezione

                                '                'è il livello più interno, quindi di fatto è come se avessi 0, non devo ricalcolare nulla
                                '                DrDescrizioneNew.Udm_Des = "n"
                                '                DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")
                                '                Prezzo_Unitario_REALE = x_Prezzo_Unitario
                                '                Prezzo_Unitario_Netto_REALE = x_Prezzo_Unitario_Netto

                                '            Case enum_OTabelle.Contenitore

                                '                DrDescrizioneNew.Udm_Des = "n"
                                '                DrDescrizioneNew.Qta = Format(numColli_contenitori, "#,###,##0.####")
                                '                Prezzo_Unitario_REALE = x_Imponibile / numColli_contenitori
                                '                Prezzo_Unitario_Netto_REALE = x_Imponibile_Netto / numColli_contenitori

                                '            Case enum_OTabelle.Imballaggio

                                '                DrDescrizioneNew.Udm_Des = "n"
                                '                DrDescrizioneNew.Qta = Format(numContenitori_imballi, "#,###,##0.####")
                                '                Prezzo_Unitario_REALE = x_Imponibile / numContenitori_imballi
                                '                Prezzo_Unitario_Netto_REALE = x_Imponibile_Netto / numContenitori_imballi

                                '            Case enum_OTabelle.Nessuno

                                '                'non devo fare nessuna operazione particolare => valori secchi
                                '                DrDescrizioneNew.Udm_Des = x_Udm_Sim
                                '                DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")
                                '                Prezzo_Unitario_REALE = x_Prezzo_Unitario
                                '                Prezzo_Unitario_Netto_REALE = x_Prezzo_Unitario_Netto

                                '            Case -1
                                '                'ho impostato il prezzo al KG
                                '                DrDescrizioneNew.Udm_Des = x_Udm_Sim
                                '                DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")
                                '                Prezzo_Unitario_REALE = x_Prezzo_Effettivo
                                '                Prezzo_Unitario_Netto_REALE = x_Prezzo_Unitario_Netto
                                '        End Select

                                '        If DrDescrizioneNew.Qta = 0 Then
                                '            'movimentato a kg
                                '            DrDescrizioneNew.Udm_Des = x_Udm_Sim
                                '            DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")
                                '        End If

                                '    Else
                                '        'riepilogo contenitori e imballaggi
                                '        DrDescrizioneNew.Udm_Des = x_Udm_Sim
                                '        DrDescrizioneNew.Qta = Format(x_Qta, "#,###,##0.####")
                                '        flag_Contenitori_Imballi = True
                                '    End If

                                '    If flag_Contenitori_Imballi = False Then
                                '        'per tutti i prodotti tranne gli imballaggi

                                '        'DrDescrizioneNew.Prezzo = Format(x_Prezzo_Unitario, "#,###,##0.00##")
                                '        DrDescrizioneNew.Prezzo = Format(Prezzo_Unitario_REALE, "#,###,##0.00##")

                                '        'DrDescrizioneNew.Prezzo_Netto = Format(x_Prezzo_Unitario_Netto, "#,###,##0.00##")
                                '        DrDescrizioneNew.Prezzo_Netto = Format(Prezzo_Unitario_Netto_REALE, "#,###,##0.00##")

                                '        DrDescrizioneNew.Importo = Format(x_Imponibile_Netto, "#,###,##0.00")
                                '        DrDescrizioneNew.Sconto = Riga_Sconto
                                '        DrDescrizioneNew.Iva = x_Aliquota
                                '        DrDescrizioneNew.Extra_Str_1 = Format(x_Qta_Extra_Totale, "#,###,##0.00##")
                                '        DrDescrizioneNew.Extra_Str_2 = Format(x_Tara, "#,###,##0.00##")
                                '        DrDescrizioneNew.Extra_Str_3 = Format(Peso_Lordo_Dettaglio, "#,###,##0.00##")

                                '        If x_Flag_Extra = 1 Then
                                '            DrDescrizioneNew.Extra_Str_4 = Format(x_Prezzo_Effettivo, "#,###,##0.00##") & "/Kg"
                                '        Else
                                '            'DrDescrizioneNew.Extra_Str_4 = Format(x_Prezzo_Unitario_Netto, "#,###,##0.00##") & "/" & DrDescrizioneNew.Udm_Des
                                '            DrDescrizioneNew.Extra_Str_4 = Format(Prezzo_Unitario_Netto_REALE, "#,###,##0.00##") & "/" & DrDescrizioneNew.Udm_Des
                                '        End If
                                '    Else
                                '        'per gli imballaggi
                                '        DrDescrizioneNew.Prezzo = ""
                                '        DrDescrizioneNew.Prezzo_Netto = ""
                                '        DrDescrizioneNew.Importo = ""
                                '        DrDescrizioneNew.Sconto = ""
                                '        DrDescrizioneNew.Iva = ""
                                '        DrDescrizioneNew.Extra_Str_1 = ""
                                '        DrDescrizioneNew.Extra_Str_2 = ""
                                '        DrDescrizioneNew.Extra_Str_3 = ""
                                '        DrDescrizioneNew.Extra_Str_4 = ""
                                '    End If

                                'End If 'If Modulo_FreshFood = True Then

                            End If 'riga descrizione libera


                            drDescrizioneNew.Contatore = i
                            drDescrizioneNew.SuperPiva = _piva

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

                            'se sto stampando il layout con pesi + valori riscontrati, allora in Extra_Str_4 (anziché il prezzo unitario), riporto il peso netto riscontrato
                            'solo se il valore è <> -1 perché è -1 se non risultava dal join (per esempio caso del riepilogo imballi)
                            If _tipoLayout = enum_TipoLayoutDDT.Pesi_Reali_piu_Riscontrati AndAlso x_Peso_Netto_Riscontrato <> -1 Then
                                drDescrizioneNew.Extra_Str_4 = Format(x_Peso_Netto_Riscontrato, "#,###,##0.##")
                            End If

                            If _moduloFreshFood = True Then

                                Select Case tipologiaRiga

                                    Case enum_TipoRigaFattura.RiepilogoConfezioni
                                        'Se è voce di riepilogo confezioni ed ho scelto di visualizzarli
                                        If _objConfigStampe.Flag_RiepilogoConfezioni = True Then
                                            ds.Descrizione.Rows.Add(drDescrizioneNew)
                                        End If

                                    Case enum_TipoRigaFattura.RiepilogoContenitori
                                        'Se è voce di riepilogo contenitori ed ho scelto di visualizzarli
                                        If _objConfigStampe.Flag_RiepilogoContenitori = True Then
                                            ds.Descrizione.Rows.Add(drDescrizioneNew)
                                        End If

                                    Case enum_TipoRigaFattura.RiepilogoImballaggi
                                        'Se è voce di riepilogo imballaggi ed ho scelto di visualizzarli
                                        If _objConfigStampe.Flag_RiepilogoImballi = True Then
                                            ds.Descrizione.Rows.Add(drDescrizioneNew)
                                        End If

                                    Case Else
                                        'stampo sempre il dettaglio
                                        ds.Descrizione.Rows.Add(drDescrizioneNew)
                                End Select


                                'Select Case _progressivoGias

                                '    ' Stefano - 16/5/2017 - tolto il riepilogo contenitori nel F&F (richiesta Cofruta)
                                '    Case enum_CodiceGIAS_Clienti.CoFruTa

                                '        If ChkContenitore <> 1 Or
                                '            (x_Elem_Cod = BENI_CONFEZ_VEGETALE AndAlso Not x_Mov_Det_Des.StartsWith("Scarico")) Then
                                '            ds.Descrizione.Rows.Add(drDescrizioneNew)
                                '        End If

                                '    Case Else

                                '        'Tutti gli altri FF stampo sempre il dettaglio (anche se è voce di riepilogo contenitori)
                                '        ds.Descrizione.Rows.Add(drDescrizioneNew)

                                'End Select

                            Else
                                ds.Descrizione.Rows.Add(drDescrizioneNew)
                            End If

                        Else 'bisogna raggruppare i dettagli
                            '------------------------------------
                            '------ RAGGRUPPAMENTO DETTAGLI -----
                            '------------------------------------
                            CreaDettaglioRaggruppato(hashGruppo, i, x_Udm_Cod, x_Udm_Sim, x_Qta, x_Qta_Extra_Totale, x_Descrizione,
                                                     x_Prezzo_Unitario, x_Prezzo_Unitario_Netto, x_Sconto_Perc, x_Tara, x_Veg_Cod, x_Cul_Cod,
                                                     Nome_Calibro, x_Cod_Articolo, Peso_Lordo_Dettaglio)


                        End If 'raggruppamento dettagli

                    End If 'dettaglio nascosto

                Catch ex As Exception
                    _logErrori &= "- Visualizzazione dei dettagli del DDT: " & vbCrLf & ex.Message & vbCrLf
                End Try

            Next 'dettagli

            '======================================================================
            '----- IMPOSTAZIONE NOTE AGGIUNTIVE E DICITURE SCONTI E OMAGGI --------
            '======================================================================

            If _tipoLayout = enum_TipoLayoutDDT.Tutto_Riscontrato_se_disponibile_piu_Prezzo Then
                If _moduloFreshFood = True Then
                    drIntestazioneNew.Peso_Lordo = FormattaArrotondaFF(_tipoArrotondamentoFF, newPesoLordoTotale, "")
                    drIntestazioneNew.Peso_Netto = FormattaArrotondaFF(_tipoArrotondamentoFF, newPesoNettoTotale, "")
                    drIntestazioneNew.Extra_Str_2 = FormattaArrotondaFF(_tipoArrotondamentoFF, newTaraTotale, "")
                Else
                    drIntestazioneNew.Peso_Lordo = Format(newPesoLordoTotale, "#,###,##0.##")
                    drIntestazioneNew.Peso_Netto = Format(newPesoNettoTotale, "#,###,##0.##")
                    drIntestazioneNew.Extra_Str_2 = Format(newTaraTotale, "#,###,##0.##")
                End If
            End If

            '===================================================================

            '####################################################

            'aggiunta dell'intestazione posticipata perché se ho layout totalmente riscontrato
            'potrei aver sostituito i valori di peso netto, lordo e tara di testata

            ds.IntestazioneFattura.Rows.Add(drIntestazioneNew)

            '####################################################


            '======================================================================
            '----- IMPOSTAZIONE NOTE AGGIUNTIVE E DICITURE SCONTI E OMAGGI --------
            '======================================================================
            AggiungiNoteAggiuntiveOmaggi(_logErrori, ds, i,
                                         Str_NoteIntegrative1, Str_NoteIntegrative2, Str_NoteIntegrative3,
                                         Flag_Campioni_Omaggio, Flag_Campioni_Omaggio_Rivalsa_Iva, Flag_Campioni_Gratuiti, Flag_Sconto_Merce)

            '===================================================================

            'End If

            '  End If

            'se impostato il raggruppamento
            If Flag_Raggruppa = True Then
                i = 0
                AggiungiRigheDettaglioRaggruppate(ds, i, hashGruppo)
            End If

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

            parametro.TotaleDdt = Format(CDec(Riepilogo_ImponibileNetto + Riepilogo_Imposta), "#,###,##0.00")

            '======================================================================

        Catch ex As Exception
            _logErrori &= "- Visualizzazione intestazione del DDT: " & vbCrLf & ex.Message & vbCrLf
        End Try


        '=============================================================
        '----- Assolve gli obblighi di cui all’articolo 62, comma 1,  del decreto legge 24 gennaio 2012,  n. 1, convertito, con modificazioni, dalla legge 24 marzo 2012, n. 27. ----------
        '----- Assolve gli obblighi di cui all'articolo 3 del decreto legislativo 08 novembre 2021, n. 198. ----------
        Try
            'verifico anche che sia una bolla emessa e non una bolla di conferimento

            If (_lavCod = LAVCOD_BOLLA_EMESSA OrElse
                _lavCod = LAVCOD_DDT_CONTABILIZZATO_EMESSO OrElse
                _lavCod = LAVCOD_AUTO_DDT_EMESSO OrElse
                _lavCod = LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE OrElse
                _lavCod = LAVCOD_CONFERIMENTO OrElse
                _lavCod = LAVCOD_CONFERIMENTO_DIVERSI) AndAlso
               ((Flag_ClientePrivato = False AndAlso Flag_AlmenoUnoProdottoAgroAlimentare = True) OrElse stampaArticolo62AltreMaterie = True) Then

                _rptBolla.Section4.SectionFormat.EnableSuppress = False
                'If Str_Articolo62 <> "" Then
                '    parametro.Articolo62 = Str_Articolo62
                '    '05/10/2020: commentato perchè questa txt è solo nei report CRBolla_Cofruta.rpt e CRBolla_Trombin.rpt
                '    'negli altri report c'è LP_Articolo62.Section4
                '    'CType(_rptBolla.Section4.ReportObjects("TxtArticolo62"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Str_Articolo62
                'End If

            End If

            parametro.Articolo62 = Str_Articolo62


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

        Dim log_errori As String = String.Empty
        Dim drLogo = dsLogoFooter.DT_LogoFooter.NewDT_LogoFooterRow
        Dim logo As ConfigurazioneLoghiStampe = TrovaLogoFooterStampe(customLoghi, log_errori, _objParametriServer)
        If logo.LogoStampe IsNot Nothing Then
            drLogo.Logo = logo.LogoStampe
            drLogo.TestoPostLogo = logo.TestoPostLogo
            drLogo.TestoPreLogo = logo.TestoPreLogo
        End If
        dsLogoFooter.DT_LogoFooter.Rows.Add(drLogo)

        Try
            'imposto il DataSet sul report
            _rptBolla.SetDataSource(ds)
            rptFooterLogo.SetDataSource(dsLogoFooter)
            _rptBolla.OpenSubreport("FooterLogo.rpt").SetDataSource(dsLogoFooter)

        Catch ex As Exception
            _logErrori &= "- Aggancio del DataSet: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '=============================================================
        '     Traduttore
        '=============================================================
        Try
            Dim tipoReport As Enum_Tipo_Report_DDT = OttieniTipoReportPerTraduttore()
            If tipoReport <> Enum_Tipo_Report_DDT.Undefined Then
                _traduttore.Traduci("", "")
            End If
        Catch ex As Exception
            _logErrori &= "- Traduttore: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '=============================================================
        '      Impostazione Parametri
        '=============================================================
        ImpostaParametri(Destinatario_Tel, parametro)

    End Sub

    Private Sub AggiungiRigheDettaglioRaggruppate(ByRef ds As DataSetFattura, ByRef contatore As Integer, ByRef hashGruppo As Hashtable)
        Dim drDescrizioneNew As DataSetFattura.DescrizioneRow

        Try
            For Each key As Object In hashGruppo.Keys

                drDescrizioneNew = ds.Descrizione.NewDescrizioneRow

                drDescrizioneNew.Contatore = contatore

                drDescrizioneNew.SuperPiva = _piva

                drDescrizioneNew.Descrizione = CStr(hashGruppo.Item(key)).Split("|")(0)

                drDescrizioneNew.Udm_Des = CStr(hashGruppo.Item(key)).Split("|")(1)

                drDescrizioneNew.Qta = CStr(hashGruppo.Item(key)).Split("|")(2)

                drDescrizioneNew.Prezzo_Netto = CStr(hashGruppo.Item(key)).Split("|")(6)

                drDescrizioneNew.Extra_Str_1 = CStr(hashGruppo.Item(key)).Split("|")(3)
                drDescrizioneNew.Extra_Str_2 = CStr(hashGruppo.Item(key)).Split("|")(4)
                drDescrizioneNew.Extra_Str_3 = CStr(hashGruppo.Item(key)).Split("|")(5)

                drDescrizioneNew.Cod_Articolo = CStr(hashGruppo.Item(key)).Split("|")(7)

                ds.Descrizione.Rows.Add(drDescrizioneNew)

                contatore += 1

            Next

        Catch ex As Exception
            _logErrori &= "- Aggiungi Righe Dettaglio Raggruppate: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Sub CreaDettaglioRaggruppato(ByRef Hash_Gruppo As Hashtable,
                                         ByVal numRiga As Integer,
                                         ByVal x_Udm_Cod As Integer,
                                         ByVal x_Udm_Sim As String,
                                         ByVal x_Qta As Decimal,
                                         ByVal x_Qta_Extra_Totale As Decimal,
                                         ByVal x_Descrizione As String,
                                         ByVal x_Prezzo_Unitario As Decimal,
                                         ByVal x_Prezzo_Unitario_Netto As Decimal,
                                         ByVal x_Sconto_Perc As Decimal,
                                         ByVal x_Tara As Decimal,
                                         ByVal x_Veg_Cod As Integer,
                                         ByVal x_Cul_Cod As Integer,
                                         ByVal Nome_Calibro As String,
                                         ByVal x_Cod_Articolo As String,
                                         ByVal Peso_Lordo_Dettaglio As Decimal)

        Dim chiave As String
        Dim valore As String
        Dim qtaGruppo As Decimal
        Dim nettoGruppo As Decimal
        Dim taraGruppo As Decimal
        Dim lordoGruppo As Decimal
        Dim codArticoloGruppo As String

        Try
            'raggruppo per specie- varietà - calibro (non posso usare il codice perché è un progressivo) - unità misura - prezzo - sconto
            chiave = CStr(x_Veg_Cod) & "|" & CStr(x_Cul_Cod) & "|" & Nome_Calibro & "|" & CStr(x_Udm_Cod) & "|" &
                     CStr(x_Prezzo_Unitario) & "|" & CStr(x_Sconto_Perc)

            'se non è già presente
            If Not Hash_Gruppo.Contains(chiave) Then

                valore = x_Descrizione & "|" & x_Udm_Sim & "|" & CStr(x_Qta) & "|" &
                         CStr(x_Qta_Extra_Totale) & "|" & CStr(x_Tara) & "|" & CStr(Peso_Lordo_Dettaglio) & "|" &
                         CStr(x_Prezzo_Unitario_Netto) & "|" & CStr(x_Cod_Articolo)

                'inserisco il dettaglio
                Hash_Gruppo.Add(chiave, valore)

            Else
                'il dettaglio è già presente
                'devo incrementare la quantità

                'prelevo la quantità del dettaglio al momento salvata
                qtaGruppo = CDec(CStr(Hash_Gruppo.Item(chiave)).Split("|")(2))

                'aggiungo la qta del dettaglio ripetuto
                qtaGruppo += x_Qta

                nettoGruppo = CDec(CStr(Hash_Gruppo.Item(chiave)).Split("|")(3))
                nettoGruppo += x_Qta_Extra_Totale

                taraGruppo = CDec(CStr(Hash_Gruppo.Item(chiave)).Split("|")(4))
                taraGruppo += x_Tara

                lordoGruppo = CDec(CStr(Hash_Gruppo.Item(chiave)).Split("|")(5))
                lordoGruppo += Peso_Lordo_Dettaglio

                codArticoloGruppo = CStr(Hash_Gruppo.Item(chiave)).Split("|")(10)
                '  Giulia, 15/11/2016 16.11.03: teoricamente non dovrebbe mai capitare che il codice articolo è diverso per lo stesso prodotto
                If codArticoloGruppo <> x_Cod_Articolo Then
                    _logErrori &= "- Errore dettaglio " & CStr(numRiga + 1) & ": " & vbCrLf & "Sono stati raggruppati articoli con codici articolo diverso" & vbCrLf
                    codArticoloGruppo = x_Cod_Articolo
                End If


                'preparo il nuovo valore
                valore = x_Descrizione & "|" & x_Udm_Sim & "|" & CStr(qtaGruppo) & "|" &
                         CStr(nettoGruppo) & "|" & CStr(taraGruppo) & "|" & CStr(lordoGruppo) & "|" &
                         CStr(x_Prezzo_Unitario_Netto) & "|" & CStr(codArticoloGruppo)

                'aggiorno il valore
                Hash_Gruppo.Item(chiave) = valore

            End If

        Catch ex As Exception
            _logErrori &= "- Crea Dettaglio raggruppato: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Function CreaDrIntestazione(ByRef Log_Errori As String,
                                        ByRef DS As DataSetFattura,
                                        ByRef Dt_Documento As DataTable,
                                        ByRef x_Edit_Importo As enum_EditImporto,
                                        ByRef x_ChkLayout_Join_Prodotti As Integer,
                                        ByRef x_ChkLayOut_Litri As Integer,
                                        ByRef x_Cod_RisUm As Integer,
                                        ByRef x_Cod_RisUm_Aggiuntivo As Integer,
                                        ByRef Flag_ClientePrivato As Boolean,
                                        ByRef Destinatario_Tel As String,
                                        ByRef parametro As Parametri,
                                        ByRef Flag_UveDiraspate As Boolean,
                                        ByVal Flag_GestMaterialeVivaistico As Boolean,
                                        ByVal Flag_CentroAziendalePartenza As Boolean
                                        ) As DataSetFattura.IntestazioneFatturaRow

        Dim drIntestazioneNew As DataSetFattura.IntestazioneFatturaRow = DS.IntestazioneFattura.NewIntestazioneFatturaRow
        Dim msgErrore As String = ""

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


        Dim Cliente_RagSoc As String = ""
        Dim Cliente_Indirizzo As String = ""
        Dim Cliente_Cap As String = ""
        Dim Cliente_Frazione As String = ""
        Dim Cliente_Comune As String = ""
        Dim Cliente_Provincia As String = ""
        'Dim Cliente_Piva As String = ""
        'Dim Cliente_CF As String = ""
        Dim Destinatario_RagSoc As String = ""
        Dim Destinatario_Ind_Des As String = ""
        Dim Destinatario_Frz_Des As String = ""
        Dim Destinatario_Cap As String = ""
        Dim Destinatario_Comune As String = ""
        Dim Destinatario_Provincia As String = ""

        '------------------
        'Impresa

        ' Dim Indirizzo_Impresa1, Indirizzo_Impresa2 As String
        Dim x_RagSoc_Impresa As String = ""
        Dim x_CodContatto_Impresa As String = ""
        Dim x_CodiceFiscale_Impresa As String = ""
        Dim x_IndDes_Impresa As String = ""
        Dim x_FrzDes_Impresa As String = ""
        Dim x_Cap_Impresa As String = ""
        Dim x_Comune_Impresa As String = ""
        Dim x_Provincia_Impresa As String = ""
        Dim x_Numero As String = ""
        Dim x_Stato_Impresa As String = ""

        'Dim x_RegImprese As String = ""
        'Dim x_Provincia_RegImprese As String = ""
        'Dim x_REA As String = ""
        'Dim x_ISO As String = ""
        'Dim x_AlboCoop As String = ""
        'Dim x_CapitaleSociale As String = ""
        'Dim x_Telefono As String = ""
        'Dim x_Fax As String = ""
        'Dim x_Cell As String = ""
        'Dim x_Email As String = ""
        'Dim x_SitoWeb As String = ""
        'Dim Sito_Email As String = ""

        '------------------
        'Movimenti
        Dim x_Id_Mov_Contabile As Integer = 0
        Dim x_Id_Cf_Cliente As Integer = 0
        Dim x_Id_Cf_Destinazione As Integer = 0
        Dim x_Id_Cf_Vettore As Integer = 0
        Dim x_Mov_Desc_Contabile As String = ""
        Dim x_Data_Movimento As Date = #1/1/1900#
        Dim x_Ora As String = ""
        Dim x_Scadenza As Date = #12/31/2100#
        Dim x_Scadenza_Extra As Date = #12/31/2100#
        Dim x_Doc_Numero_Sin As String = ""
        Dim x_Doc_Numero As Integer = 0
        Dim x_Doc_Numero_Des As String = ""
        Dim x_Lunghezza_Sin As Integer = 0
        Dim x_Lunghezza_Centro As Integer = 0
        Dim x_Lunghezza_Des As Integer = 0
        Dim x_CarattereFormattazione As String = ""
        Dim x_Progr_Protocollo As Integer = 0
        Dim x_Progr_Registrazione As Integer = 0
        Dim x_Data_Registrazione As Date = #1/1/1900#
        Dim x_Num_Protocollo As Decimal = 0
        Dim x_Colli As Integer = 0
        Dim x_Peso As Decimal = 0
        Dim x_Aspetto As String = ""
        Dim x_Causale_Trasporto As String = ""
        Dim x_Cod_Contatto As String = ""
        Dim x_Rag_Soc As String = ""
        Dim x_Codice_Fiscale As String = ""
        Dim x_chk_fittizio As Integer
        Dim x_Cod_IndirizzoRisUm As Integer = 0
        Dim x_Cod_Destinazione As Integer = 0
        Dim x_CodContatto_Destinazione As String = ""
        Dim x_RagSoc_Destinazione As String = ""
        Dim x_CodiceFiscale_Destinazione As String = ""
        Dim x_ChkFittizio_Destinazione As Integer
        Dim x_Cod_IndirizzoDestinazione As Integer = 0

        Dim x_Cod_Indirizzo_Aggiuntivo As Integer = 0
        Dim x_Id_Cf_Aggiuntivo As Integer = 0
        Dim x_CodContatto_Aggiuntivo As String = ""
        Dim x_RagSoc_Aggiuntivo As String = ""
        Dim x_CodiceFiscale_Aggiuntivo As String = ""
        Dim x_ChkFittizio_Aggiuntivo As Integer

        Dim x_Mezzo As Integer = 0
        Dim x_Cod_Vettore As Integer = 0
        Dim x_Cod_IndirizzoVettore As Integer = 0
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
        Dim Peso_Lordo As Decimal = 0
        Dim x_Tipo_Peso As Integer = 0
        Dim x_Username_Note As String = ""
        Dim x_Extra_Str As String = ""
        Dim x_Extra_Int As Integer = 0
        Dim x_Extra_Date As Date = #1/1/1900#

        Dim x_ChkLayout_Bypass_Fatturato As Integer = 0
        Dim x_ChkLayOut_Peso As Integer = 0
        Dim x_ChkLayOut_Prezzo As Integer = 0
        Dim x_ChkLayOut_Riscontrato As Integer = 0

        'Dim x_Tipo_Sconto As Integer = 0
        Dim x_Gestione_Vettore As String = ""

        Dim x_Ind_Des As String = ""
        Dim x_Frz_Des As String = ""
        Dim x_Cap As String = ""
        Dim x_Comune As String = ""
        Dim x_Provincia As String = ""
        Dim x_Stato As String = ""
        'Dim Flag_StatoMembro As Boolean = True

        Dim x_IndDes_Destinazione As String = ""
        Dim x_FrzDes_Destinazione As String = ""
        Dim x_Cap_Destinazione As String = ""
        Dim x_Comune_Destinazione As String = ""
        Dim x_Provincia_Destinazione As String = ""
        Dim x_Stato_Destinazione As String = ""
        Dim x_Tel_Destinazione As String = ""
        Dim x_Cell_Destinazione As String = ""
        Dim x_IndirizzoTipoDesc_Destinazione As String = ""

        Dim x_IndDes_Vettore As String = ""
        Dim x_FrzDes_Vettore As String = ""
        Dim x_Cap_Vettore As String = ""
        Dim x_Comune_Vettore As String = ""
        Dim x_Provincia_Vettore As String = ""
        Dim x_Stato_Vettore As String = ""

        Dim x_N_Doc_Cliente As String
        Dim x_Data_Doc_Cliente As Date
        Dim x_N_Nota_Fattura As String
        Dim x_Data_Nota_Fattura As Date
        Dim x_N_Nota_DDT As String
        Dim x_N_Nota_Riga_DDT As String
        Dim x_Data_Nota_DDT As Date

        Dim Piva_Cliente As String = ""
        Dim Codice_Fiscale_Cliente As String = ""
        Dim Piva_Dest As String = ""
        Dim Codice_Fiscale_Dest As String = ""

        Dim stampaSDIAcquisto As Boolean = False
        Dim codiceSDIAcquisto As String = ""

        '############################################################################################
        '############################ Lettura della Bolla ###########################################
        '############################################################################################

        Dt_Documento = Nothing

        Try

            Leggi_Movimenti_DocContabile(_objParametriServer,
                                         msgErrore,
                                         _piva,
                                         _idAgenda,
                                         _codReport,
                                         Dt_Documento,
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
                                         x_chk_fittizio,
                                         x_Cod_IndirizzoRisUm,
                                         x_Id_Cf_Cliente,
                                         x_Cod_Destinazione,
                                         x_CodContatto_Destinazione,
                                         x_RagSoc_Destinazione,
                                         x_CodiceFiscale_Destinazione,
                                         x_ChkFittizio_Destinazione,
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
                                         Flag_UveDiraspate,
                                         x_ChkLayOut_Riscontrato,
                                             x_N_Doc_Cliente,
                                               x_Data_Doc_Cliente,
                                               x_N_Nota_Fattura,
                                                x_Data_Nota_Fattura,
                                                x_N_Nota_DDT,
                                               x_N_Nota_Riga_DDT,
                                               x_Data_Nota_DDT,
                                               Nothing)


            If msgErrore <> "" Then
                Log_Errori &= msgErrore
            End If

            _tipoLayout = GetTipoLayout(x_ChkLayOut_Peso, x_ChkLayOut_Prezzo, x_ChkLayOut_Riscontrato)

        Catch ex As Exception
            Log_Errori &= "- Lettura del DDT: " & vbCrLf & ex.Message & vbCrLf
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
                                         Log_Errori,
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
                                         Flag_GestMaterialeVivaistico,
                                         _traduttore,
                                         "",
                                         Flag_CentroAziendalePartenza)


        Catch ex As Exception
            Log_Errori &= "- Lettura dei dati dell'intestazione dell'impresa: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Dim documentoSaCod As Integer = 0
        Dim documentoDettagliSaCod = Dt_Documento.AsEnumerable().Where(Function(row) row.Item("Sa_Cod_Dett") <> 0 AndAlso row.Item("Ordine_Det") <> RIGA_IMBALLI_CONTENTI_PRODOTTI)
        If documentoDettagliSaCod.Count > 0 Then
            documentoSaCod = documentoDettagliSaCod.First().Item("Sa_Cod_Dett")
        End If

        ImpostazioniImpresaUtente_StampaDoc(_objParametriServer, _objParametriUtenti, Log_Errori, _piva, documentoSaCod, stampaSDIAcquisto)

        '=============================================================

        Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim pivaReale As String = objImp.Leggi_PivaReale(_piva, _objParametriServer)

        'select case per la valorizzazione dei DATI DELL'AZIENDA
        '(a seconda dei casi va nella sezione intestazione
        'oppure nella sezione cliente/destinatario
        Select Case _lavCod

            'DDT emesso  e conferimento like Agrisfera (da superuser a contatto)
            Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO,
                LAVCOD_CONFERIMENTO_DIVERSI,
                LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE


                ''=============================================================
                ''-------------- IMPRESA CHE EMETTE BOLLA  -------------------
                ''              CHI EMETTE E' L'AZIENDA
                parametro.IntestazioneRiga1 = x_RagSoc_Impresa

                drIntestazioneNew.SuperPiva = _piva

                If x_Id_Cf_Cliente = enum_Contatti_IdCf.ContattoEstero Then
                    parametro.IntestazioneRiga2 = _traduttore.ValoreDizionarioTraduzioneComuni("VAT: IT") & pivaReale
                Else
                    parametro.IntestazioneRiga2 = _traduttore.ValoreDizionarioTraduzioneComuni("Partita IVA: ") & pivaReale
                End If
                If x_CodiceFiscale_Impresa <> "" Then
                    parametro.IntestazioneRiga2 &= _traduttore.ValoreDizionarioTraduzioneComuni("   Codice Fiscale: ") & x_CodiceFiscale_Impresa
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

                ''   = x_IndDes_Impresa & " " & x_FrzDes_Impresa & " " & x_Cap_Impresa & " " & x_Comune_Impresa & " " & "(" & x_Provincia_Impresa & ")"
                'Indirizzo_Impresa1 = x_IndDes_Impresa & " " & x_FrzDes_Impresa
                'Indirizzo_Impresa2 = x_Cap_Impresa & " " & x_Comune_Impresa & " " & "(" & x_Provincia_Impresa & ")"

                ''modifica del 20/09/2010
                ''introduco la chiamata a questa funzione
                ''per la gestione del taroccamento piva x multi-attività
                'Ricava_Piva_CodiceFiscale(Piva, x_CodiceFiscale_Impresa, Piva, x_CodiceFiscale_Impresa, Nothing)

                'Dim str_Piva_CF As String = ""
                'If x_Id_Cf_Cliente = 2 Then
                '    'CType(rptBolla.Section2.ReportObjects("TxtPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "VAT: IT" & Piva
                '    str_Piva_CF = "VAT: IT" & Piva
                'Else
                '    'CType(rptBolla.Section2.ReportObjects("TxtPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Partita IVA: " & Piva
                '    str_Piva_CF = "Partita IVA: " & Piva
                'End If
                ''CType(rptBolla.Section2.ReportObjects("TxtCF"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Codice Fiscale: " & x_CodiceFiscale_Impresa
                'If x_CodiceFiscale_Impresa <> "" Then
                '    str_Piva_CF &= "   Codice Fiscale: " & x_CodiceFiscale_Impresa
                'End If
                'CType(rptBolla.Section2.ReportObjects("TxtPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = str_Piva_CF

                'CType(rptBolla.Section2.ReportObjects("TxtRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = x_RagSoc_Impresa
                'CType(rptBolla.Section2.ReportObjects("TxtIndirizzo1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Sede legale: " & Indirizzo_Impresa1
                'CType(rptBolla.Section2.ReportObjects("TxtIndirizzo2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Indirizzo_Impresa2

                'If x_RegImprese <> "" Then
                '    CType(rptBolla.Section2.ReportObjects("TxtRegImprese"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Iscr. al n. " & x_RegImprese & " del Reg. Imprese di " & x_Provincia_RegImprese
                'End If

                'Dim str_REA_ISO As String = ""
                'If x_REA <> "" Then
                '    'CType(rptBolla.Section2.ReportObjects("TxtREA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "R.E.A.: " & x_REA
                '    str_REA_ISO = "R.E.A.: " & x_REA
                'End If
                'If x_ISO <> "" Then
                '    'CType(rptBolla.Section2.ReportObjects("TxtISO"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Codice ISO: " & x_ISO
                '    str_REA_ISO &= "   Codice ISO: " & x_ISO
                'End If
                'CType(rptBolla.Section2.ReportObjects("TxtREA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = str_REA_ISO

                'If x_AlboCoop <> "" Then
                '    CType(rptBolla.Section2.ReportObjects("TxtAlboCoop"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "N.Iscr. dell'Albo delle Soc.Coop. a Mutualità Prevalente: " & x_AlboCoop
                'End If
                'If x_CapitaleSociale <> "" Then
                '    CType(rptBolla.Section2.ReportObjects("TxtCapitaleSociale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = x_CapitaleSociale
                'End If
                'Dim TelFaxCell As String = ""
                'If x_Telefono <> "" Then
                '    TelFaxCell &= "Telefono: " & x_Telefono
                'End If
                'If x_Fax <> "" Then
                '    TelFaxCell &= " Fax: " & x_Fax
                '    ' CType(rptBolla.Section2.ReportObjects("TxtFax"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Fax: " & x_Fax
                'End If
                'If x_Cell <> "" Then
                '    TelFaxCell &= " Cell: " & x_Cell
                'End If
                'CType(rptBolla.Section2.ReportObjects("TxtTelefono"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TelFaxCell

                ''If x_Email <> "" Then
                ''    CType(rptBolla.Section2.ReportObjects("TxtEmail"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Email: " & x_Email
                ''End If
                ''If x_SitoWeb <> "" Then
                ''   CType(rptBolla.Section2.ReportObjects("TxtSito"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Sito web: " & x_SitoWeb
                ''End If

                'If x_Email <> "" Then
                '    Sito_Email = x_Email
                'End If
                'If x_SitoWeb <> "" Then
                '    If Sito_Email <> "" Then
                '        Sito_Email &= " "
                '    End If
                '    Sito_Email &= x_SitoWeb
                'End If
                'CType(rptBolla.Section2.ReportObjects("TxtSitoEmail"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Sito_Email

                ''Assegnazione Intestatario al DataRow
                'DrIntestazioneNew.SuperRag_Soc = x_RagSoc_Impresa
                'DrIntestazioneNew.SuperPiva = Piva

                ''DrIntestazioneNew.SuperInd_Des = x_IndDes_Impresa
                ''DrIntestazioneNew.SuperFrz_Des = x_FrzDes_Impresa
                ''DrIntestazioneNew.SuperCap = x_Cap_Impresa
                ''DrIntestazioneNew.SuperComune = x_Comune_Impresa
                ''DrIntestazioneNew.SuperProvincia = "(" & x_Provincia_Impresa & ")"

                ''Try
                ''    'cod_contatto= piva
                ''    'piva è quella dell'impresa che ha creato il contatto, quasi certamente il superuser
                ''    Leggi_Rubrica(Server, Session, Page, Piva, x_Cod_RisUm, 0, 0, x_Numero)

                ''Catch ex As Exception
                ''    Log_Errori &= "- Lettura della rubrica dell'impresa: " & vbCrLf & ex.Message & vbCrLf
                ''End Try

                'DrIntestazioneNew.Telefono = x_Numero

                '===================================================================


                'conferimento like Agribologna (superuser che riceve bolla dal socio)
            Case LAVCOD_CONFERIMENTO, LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_BOLLA_RICEVUTA

                '-------------------------------------------------------------
                '------------------ CONFERIMENTO DA SOCI ---------------------
                '-------------------------------------------------------------

                _identContatto = x_RagSoc_Impresa

                '=============================================================
                '------------------------ CLIENTE  ---------------------------
                'in questo caso il cliente è il superuser (esempio AgriBologna)

                'SI VUOLE VISUALIZZARE IL RIQUADRO FATTURATO A:
                If x_ChkLayout_Bypass_Fatturato = 0 Then

                    Cliente_RagSoc = x_RagSoc_Impresa
                    Cliente_Indirizzo = x_IndDes_Impresa
                    Cliente_Frazione = x_FrzDes_Impresa
                    Cliente_Cap = x_Cap_Impresa
                    Cliente_Comune = x_Comune_Impresa
                    Cliente_Provincia = "(" & x_Provincia_Impresa & ")"
                    Piva_Cliente = _piva
                    Codice_Fiscale_Cliente = x_CodiceFiscale_Impresa
                Else
                    Cliente_RagSoc = ""
                    Cliente_Indirizzo = ""
                    Cliente_Frazione = ""
                    Cliente_Cap = ""
                    Cliente_Comune = ""
                    Cliente_Provincia = ""
                    Piva_Cliente = ""
                    Codice_Fiscale_Cliente = ""
                End If


                '=============================================================
                '--------------------- DESTINATARIO --------------------------
                'in questo caso il destinatario è il superuser (esempio AgriBologna)

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

                Destinatario_Tel = Leggi_RubricaTel_CentroAziendale(_objParametriServer, _logErrori, _piva, documentoSaCod, _traduttore)

        End Select

        '=============================================================

        'select case per la valorizzazione dei DATI DEL CONTATTO CLIENTE E CONTATTO DESTINATARIO
        '(a seconda dei casi va nella sezione cliente/destinatario
        'O nella sezione intestazione)
        Select Case _lavCod

            'AutoDDT, usati con il conferimento uva
            Case LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE

                _identContatto = x_Rag_Soc

                '-------------------------------------------------------------
                '---------- AUTODDT ------------
                'nella sezione cliente c'è il fornitore (salvato in cod_risum)
                'nella sezione destinazione c'è l'azienda che emette
                '-------------------------------------------------------------

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
                        Log_Errori &= "- Lettura dell'indirizzo del contatto cliente: " & vbCrLf & ex.Message & vbCrLf
                    End Try

                End If

                Cliente_RagSoc = x_Rag_Soc

                Select Case x_Id_Cf_Cliente

                    Case enum_Contatti_IdCf.ContattoEstero

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
                        '    parametro.LblPivaCliente = "Codice VAT:"
                        'Else
                        '    parametro.LblPivaCliente = ""
                        '    Piva_Cliente = ""
                        'End If

                        Ricava_Piva_Codicefiscale(x_Id_Cf_Cliente,
                                                  x_Cod_Contatto,
                                                  "",
                                                    x_chk_fittizio,
                                                  Piva_Cliente,
                                                  Codice_Fiscale_Cliente,
                                                  Nothing)

                        If x_chk_fittizio = 1 Then
                            parametro.LblPivaCliente = ""
                        Else
                            parametro.LblPivaCliente = _traduttore.ValoreDizionarioTraduzioneComuni("Codice VAT:")
                        End If

                        parametro.LblCfCliente = ""

                    Case Else

                        Ricava_Piva_Codicefiscale(x_Id_Cf_Cliente,
                                                  x_Cod_Contatto,
                                                  x_Codice_Fiscale,
                                                    x_chk_fittizio,
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

                parametro.TxtPivaCliente = Piva_Cliente
                parametro.TxtCfCliente = Codice_Fiscale_Cliente

                '-----------------------------------------------------
                '-------- LA DESTINAZIONE è L'AZIENDA STESSA ---------
                '-----------------------------------------------------

                x_RagSoc_Destinazione = x_RagSoc_Impresa
                x_CodContatto_Destinazione = _piva
                x_CodiceFiscale_Destinazione = x_CodiceFiscale_Impresa

                'TODO Verificare cosa succede in caso di impresa estera, perché il comune è preso sempre dalla ISTAT,
                'ma se la gestione gerarchia indirizzi non è attiva per lo Stato il comune dovrebbe essere salvato nella frz_des della Indirizzi
                x_IndDes_Destinazione = x_IndDes_Impresa
                x_FrzDes_Destinazione = x_FrzDes_Impresa
                x_Cap_Destinazione = x_Cap_Impresa
                x_Comune_Destinazione = x_Comune_Impresa
                x_Provincia_Destinazione = x_Provincia_Impresa

                Destinatario_RagSoc = x_RagSoc_Destinazione
                Destinatario_Ind_Des = x_IndDes_Destinazione
                Destinatario_Frz_Des = x_FrzDes_Destinazione
                Destinatario_Cap = x_Cap_Destinazione
                Destinatario_Comune = x_Comune_Destinazione
                Destinatario_Provincia = "(" & x_Provincia_Destinazione & ")"

                If Intestazione_Riga7 <> "" Then
                    Destinatario_Tel = Intestazione_Riga7
                ElseIf Intestazione_Riga12 <> "" Then
                    Destinatario_Tel = Intestazione_Riga12
                End If

                '####################################################################


                'DDT emesso, ddt corrispettivi e conferimento like Agrisfera (da superuser a contatto)
            Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_CONFERIMENTO_DIVERSI

                '-------------------------------------------------------------
                '---------- BOLLA EMESSA E CONFERIMENTO A DIVERSI ------------
                '-------------------------------------------------------------

                _identContatto = x_Rag_Soc

                '=============================================================
                '------------------- FATTURATO A: (CLIENTE)  ---------------------------
                'in questo caso il cliente è il contatto

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
                        Log_Errori &= "- Lettura dell'indirizzo del contatto cliente: " & vbCrLf & ex.Message & vbCrLf
                    End Try

                End If

                'SI VUOLE VISUALIZZARE IL RIQUADRO FATTURATO A:
                If x_ChkLayout_Bypass_Fatturato = 0 Then

                    Cliente_RagSoc = x_Rag_Soc

                    Select Case x_Id_Cf_Cliente
                        Case enum_Contatti_IdCf.ContattoEstero

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
                            '    parametro.LblPivaCliente = "Codice VAT:"
                            'Else
                            '    parametro.LblPivaCliente = ""
                            '    Piva_Cliente = ""
                            'End If

                            Ricava_Piva_Codicefiscale(x_Id_Cf_Cliente,
                                                      x_Cod_Contatto,
                                                      "",
                                                        x_chk_fittizio,
                                                      Piva_Cliente,
                                                      Codice_Fiscale_Cliente,
                                                      Nothing)

                            If x_chk_fittizio = 1 Then
                                parametro.LblPivaCliente = ""
                            Else
                                parametro.LblPivaCliente = _traduttore.ValoreDizionarioTraduzioneComuni("Codice VAT:")
                            End If

                            parametro.LblCfCliente = ""

                        Case Else

                            Ricava_Piva_Codicefiscale(x_Id_Cf_Cliente,
                                                      x_Cod_Contatto,
                                                      x_Codice_Fiscale,
                                                      x_chk_fittizio,
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

                    parametro.TxtPivaCliente = Piva_Cliente
                    parametro.TxtCfCliente = Codice_Fiscale_Cliente

                Else
                    Cliente_RagSoc = ""
                    Cliente_Indirizzo = ""
                    Cliente_Frazione = ""
                    Cliente_Cap = ""
                    Cliente_Comune = ""
                    Cliente_Provincia = ""

                    parametro.TxtPivaCliente = ""
                    parametro.TxtCfCliente = ""
                End If

                '=============================================================
                '--------------------- DESTINATARIO --------------------------
                'in questo caso il destinatario è il contatto

                'Destinazione Diversa
                If x_Cod_Destinazione <> 0 Then

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
                        Log_Errori &= "- Lettura dell'indirizzo del destinatario: " & vbCrLf & ex.Message & vbCrLf
                        'dovrebbe verificarsi nel caso di dati importati tramite g2g (non vengono rimappati gli indirizzi)
                    End Try

                    Try

                        Leggi_Rubrica(_objParametriServer,
                                      x_CodContatto_Destinazione,
                                      x_Cod_Destinazione,
                                      0,
                                      x_Tel_Destinazione,
                                      x_Cell_Destinazione)

                    Catch ex As Exception
                        Log_Errori &= "- Lettura della rubrica del destinatario: " & vbCrLf & ex.Message & vbCrLf
                    End Try

                    '  Giulia, 29/12/2016 17.11.52: Verifica della piva e cf destinazione
                    '       caso Cofruta - opera, devo visualizzarlo, quindi devo assicurarmi che sia corretto
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
                                                      x_ChkFittizio_Destinazione,
                                                      Piva_Dest,
                                                      Codice_Fiscale_Dest,
                                                      Nothing)

                            Destinatario_Ind_Des = x_IndDes_Destinazione
                            Destinatario_Frz_Des = x_FrzDes_Destinazione
                            Destinatario_Cap = x_Cap_Destinazione
                            Destinatario_Comune = x_Comune_Destinazione
                            Destinatario_Provincia = "(" & x_Provincia_Destinazione & ")"

                            '14/03/2019: commentato per lasciare quanto impostato da Ricava_Piva_Codicefiscale
                            'If x_Id_Cf_Destinazione = 0 Then
                            '    Piva_Dest = ""
                            'End If
                    End Select

                Else
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
                        Log_Errori &= "- Lettura della rubrica del cliente: " & vbCrLf & ex.Message & vbCrLf
                    End Try

                End If

                If x_Tel_Destinazione <> "" Then
                    Destinatario_Tel = _traduttore.ValoreDizionarioTraduzioneComuni("Tel: ") & x_Tel_Destinazione
                End If
                If x_Cell_Destinazione <> "" Then
                    Destinatario_Tel &= _traduttore.ValoreDizionarioTraduzioneComuni(" Cell: ") & x_Cell_Destinazione
                End If
                Destinatario_Tel = Destinatario_Tel.Trim

                If x_IndirizzoTipoDesc_Destinazione = "" Then
                    Destinatario_RagSoc = x_RagSoc_Destinazione
                Else
                    Destinatario_RagSoc = x_IndirizzoTipoDesc_Destinazione
                End If

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
                                                  x_ChkFittizio_Destinazione,
                                                  Piva_Dest,
                                                  Codice_Fiscale_Dest,
                                                  Nothing)

                        Destinatario_Ind_Des = x_IndDes_Destinazione
                        Destinatario_Frz_Des = x_FrzDes_Destinazione
                        Destinatario_Cap = x_Cap_Destinazione
                        Destinatario_Comune = x_Comune_Destinazione
                        Destinatario_Provincia = "(" & x_Provincia_Destinazione & ")"

                        '14/03/2019: commentato per lasciare quanto impostato da Ricava_Piva_Codicefiscale
                        'If x_Id_Cf_Destinazione = 0 Then
                        '    Piva_Dest = ""
                        'End If
                End Select


                '####################################################################

            Case LAVCOD_CONFERIMENTO, LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_BOLLA_RICEVUTA

                '-------------------------------------------------------------
                '------------------ CONFERIMENTO DA SOCI ---------------------
                '                   (modalità AGRIBOLOGNA)
                '-------------------------------------------------------------


                '=============================================================
                '-------------- IMPRESA CHE EMETTE BOLLA  -------------------

                'Assegnazione Intestatario al DataRow
                '   DrIntestazioneNew.SuperRag_Soc = x_Rag_Soc
                parametro.IntestazioneRiga1 = x_Rag_Soc

                Piva_Cliente = pivaReale
                Codice_Fiscale_Cliente = x_CodiceFiscale_Impresa
                codiceSDIAcquisto = If(stampaSDIAcquisto, Intestazione_Riga10, "") 'Il codice è salvato in CentriXRubrica nel campo "social"

                'modifica del 20/09/2010
                'introduco la chiamata a questa funzione
                'per la gestione del taroccamento piva x multi-attività
                Ricava_Piva_Codicefiscale(PERSONA_GIURIDICA, x_Cod_Contatto, x_Codice_Fiscale, 0, x_Cod_Contatto, x_Codice_Fiscale, Nothing)

                drIntestazioneNew.SuperPiva = x_Cod_Contatto

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
                    Log_Errori &= "- Lettura dell'indirizzo dell'impresa: " & vbCrLf & ex.Message & vbCrLf
                End Try

                If x_Cod_Destinazione <> 0 Then
                    'In caso sia selezionato un contatto di destinazione diversa

                    Select Case x_Id_Cf_Destinazione
                        Case enum_Contatti_IdCf.ContattoEstero

                            'Se il contatto è estero, il comune potrebbe essere esplicitato nella x_Comune dalla tabella ISTAT oppure
                            'come campo libero nella x_Frz_Des della Indirizzi. Nel primo caso, x_Frz_Des potrebbe contenere una effettiva frazione
                            'inserita dall'utente
                            If String.IsNullOrWhiteSpace(x_Comune) Then
                                Intestazione_Riga5 = x_Ind_Des
                                Intestazione_Riga6 = x_Cap & " " & x_Frz_Des & " " & x_Stato
                            Else
                                Intestazione_Riga5 = x_Ind_Des & " " & x_Frz_Des
                                Intestazione_Riga6 = x_Cap & " " & x_Comune & " " & x_Stato
                            End If

                        Case Else
                            '  Indirizzo_Impresa1 = x_Ind_Des & " " & x_Frz_Des
                            Intestazione_Riga5 = x_Ind_Des & " " & x_Frz_Des
                            ' Indirizzo_Impresa2 = x_Cap & " " & x_Comune & " " & "(" & x_Provincia & ")"
                            Intestazione_Riga6 = x_Cap & " " & x_Comune & " " & "(" & x_Provincia & ")"
                    End Select

                Else

                    Select Case x_Id_Cf_Cliente
                        Case enum_Contatti_IdCf.ContattoEstero

                            'Se il contatto è estero, il comune potrebbe essere esplicitato nella x_Comune dalla tabella ISTAT oppure
                            'come campo libero nella x_Frz_Des della Indirizzi. Nel primo caso, x_Frz_Des potrebbe contenere una effettiva frazione
                            'inserita dall'utente
                            If String.IsNullOrWhiteSpace(x_Comune) Then
                                Intestazione_Riga5 = x_Ind_Des
                                Intestazione_Riga6 = x_Cap & " " & x_Frz_Des & " " & x_Stato
                            Else
                                Intestazione_Riga5 = x_Ind_Des & " " & x_Frz_Des
                                Intestazione_Riga6 = x_Cap & " " & x_Comune & " " & x_Stato
                            End If

                        Case Else
                            Intestazione_Riga5 = x_Ind_Des & " " & x_Frz_Des
                            Intestazione_Riga6 = x_Cap & " " & x_Comune & " " & "(" & x_Provincia & ")"
                    End Select

                End If


                Dim str_Piva_CF As String
                str_Piva_CF = _traduttore.ValoreDizionarioTraduzioneComuni("Partita IVA: ") & x_Cod_Contatto
                If x_Codice_Fiscale <> "" Then
                    str_Piva_CF &= _traduttore.ValoreDizionarioTraduzioneComuni("   Codice Fiscale: ") & x_Codice_Fiscale
                End If
                parametro.IntestazioneRiga2 = str_Piva_CF
                parametro.IntestazioneRiga1 = x_Rag_Soc
                parametro.IntestazioneRiga5 = _traduttore.ValoreDizionarioTraduzioneComuni("Sede legale: ") & Intestazione_Riga5
                parametro.IntestazioneRiga6 = Intestazione_Riga6


                ' In caso di bolla ricevuta stampo anche il secondo cedente (richiesta Cofruta luglio 2017)
                If _lavCod = LAVCOD_BOLLA_RICEVUTA AndAlso x_Cod_Destinazione <> 0 Then

                    Try

                        Leggi_Indirizzi(_objParametriServer,
                                        x_CodContatto_Destinazione,
                                        0,
                                        x_Cod_IndirizzoDestinazione,
                                        x_Ind_Des,
                                        x_Frz_Des,
                                        x_Cap,
                                        x_Comune,
                                        x_Provincia,
                                        x_Stato,
                                        Nothing,
                                        Nothing,
                                        x_Cod_Contatto)

                    Catch ex As Exception
                        Log_Errori &= "- Lettura dell'indirizzo dell'impresa: " & vbCrLf & ex.Message & vbCrLf
                    End Try

                    Select Case x_Id_Cf_Destinazione
                        Case enum_Contatti_IdCf.ContattoEstero

                            'Se il contatto è estero, il comune potrebbe essere esplicitato nella x_Comune dalla tabella ISTAT oppure
                            'come campo libero nella x_Frz_Des della Indirizzi. Nel primo caso, x_Frz_Des potrebbe contenere una effettiva frazione
                            'inserita dall'utente
                            If String.IsNullOrWhiteSpace(x_Comune) Then
                                Intestazione_Riga10 = x_Ind_Des
                                Intestazione_Riga11 = x_Cap & " " & x_Frz_Des & " " & x_Stato
                            Else
                                Intestazione_Riga10 = x_Ind_Des & " " & x_Frz_Des
                                Intestazione_Riga11 = x_Cap & " " & x_Comune & " " & x_Stato
                            End If

                        Case Else
                            Intestazione_Riga10 = x_Ind_Des & " " & x_Frz_Des
                            Intestazione_Riga11 = x_Cap & " " & x_Comune & " " & "(" & x_Provincia & ")"
                    End Select

                    str_Piva_CF = _traduttore.ValoreDizionarioTraduzioneComuni("Partita IVA: ") & x_CodContatto_Destinazione
                    If x_CodiceFiscale_Destinazione <> "" Then
                        str_Piva_CF &= _traduttore.ValoreDizionarioTraduzioneComuni("   Codice Fiscale: ") & x_CodiceFiscale_Destinazione
                    End If
                    parametro.IntestazioneRiga9 = str_Piva_CF
                    parametro.IntestazioneRiga8 = x_RagSoc_Destinazione
                    parametro.IntestazioneRiga10 = _traduttore.ValoreDizionarioTraduzioneComuni("Stabilimento: ") & Intestazione_Riga10
                    parametro.IntestazioneRiga11 = Intestazione_Riga11
                End If

        End Select

        'Gestioni particolari per parametri intestazione_riga

        ModificaParametriIntestazioneGestioneMaterialeVivaistico(parametro, Flag_GestMaterialeVivaistico, documentoSaCod)
        ModificaParametriIntestazioneCentroAziendalePartenza(parametro, Flag_CentroAziendalePartenza, documentoSaCod)

        Dim impresaEsternaPiva As String = ""
        Dim impresaEsternaSaCod As Integer

        If _lavCod = LAVCOD_BOLLA_EMESSA AndAlso documentoSaCod <> 0 Then
            Dim handleCentriAzCodici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read()

            Dim impresaEsternaValCod As String = handleCentriAzCodici.ValCod_from_SaCodIdCod(_piva, documentoSaCod, enum_CodiciAnagrafe.Centro_Aziendale_Esterno_Collegato, _objParametriServer)

            If impresaEsternaValCod <> "" Then
                Dim arrImpresaEsterna As String() = impresaEsternaValCod.Split("_")
                impresaEsternaPiva = arrImpresaEsterna(0)
                impresaEsternaSaCod = arrImpresaEsterna(1)
            End If

        End If

        ModificaParametriIntestazioneGestioneImpresaEsterna(parametro, Log_Errori, x_Id_Cf_Cliente, Flag_GestMaterialeVivaistico, impresaEsternaPiva, impresaEsternaSaCod)


        '=============================================================
        '-------------- CESSIONARIO AGGIUNTIVO -------------------
        'se presente un cessionario diverso
        If x_Cod_RisUm_Aggiuntivo <> 0 Then
            ValorizzaRisUmAggiuntivo(Log_Errori, x_Cod_RisUm_Aggiuntivo, x_Cod_Indirizzo_Aggiuntivo, x_Id_Cf_Aggiuntivo,
                                     x_CodContatto_Aggiuntivo, x_RagSoc_Aggiuntivo, x_CodiceFiscale_Aggiuntivo, x_ChkFittizio_Aggiuntivo,
                                     parametro.RagSocAggiuntivo, parametro.IndirizzoAggiuntivo, parametro.CapAggiuntivo, parametro.FrazioneAggiuntivo,
                                     parametro.ComuneAggiuntivo, parametro.ProvAggiuntivo, parametro.TxtPivaAgg, parametro.TxtCfAgg, _objParametriServer)
        Else
            'non è presente un cessionario diverso
            If impresaEsternaPiva <> "" Then

                x_Cod_RisUm_Aggiuntivo = 1 'Imposto un valore a questa variabile perché viene usata per decidere se mostrare i dati del contatto oppure il logo del cedente

                'Se c'è una impresa esterna, gestione abilitata nelle bolle emesse, allora questa diventa il cedente principale ed in questi campi imposto l'impresa che emette il doc.
                parametro.RagSocAggiuntivo = x_RagSoc_Impresa
                parametro.IndirizzoAggiuntivo = x_IndDes_Impresa
                parametro.FrazioneAggiuntivo = x_FrzDes_Impresa
                parametro.CapAggiuntivo = x_Cap_Impresa
                parametro.ComuneAggiuntivo = x_Comune_Impresa
                parametro.ProvAggiuntivo = "(" & x_Provincia_Impresa & ")"

                parametro.TxtPivaAgg = _piva
                parametro.TxtCfAgg = x_CodiceFiscale_Impresa
            Else
                'Altrimenti ricopio la destinazione diversa, se presente
                If x_Cod_Destinazione <> 0 Then
                    parametro.RagSocAggiuntivo = Destinatario_RagSoc
                    parametro.IndirizzoAggiuntivo = Destinatario_Ind_Des
                    parametro.FrazioneAggiuntivo = Destinatario_Frz_Des
                    parametro.CapAggiuntivo = Destinatario_Cap
                    parametro.ComuneAggiuntivo = Destinatario_Comune
                    parametro.ProvAggiuntivo = Destinatario_Provincia

                    parametro.TxtPivaAgg = Piva_Dest
                    parametro.TxtCfAgg = Codice_Fiscale_Dest
                End If
            End If

        End If



        '=============================================================
        'Valorizzazione sul DataSet di fatturato a / destinatario

        drIntestazioneNew.Rag_Soc = Cliente_RagSoc
        drIntestazioneNew.Ind_Des = Cliente_Indirizzo
        drIntestazioneNew.Frz_Des = Cliente_Frazione
        drIntestazioneNew.Cap = Cliente_Cap
        drIntestazioneNew.Comune = Cliente_Comune
        drIntestazioneNew.Provincia = Cliente_Provincia


        ''  Giulia, 29/12/2016 16.35.15: Personalizzazione CofruTa - Opera
        'If _progressivoGias = enum_CodiceGIAS_Clienti.CoFruTa _
        '    AndAlso UsoConfigStampe = True _
        '    AndAlso Not String.IsNullOrEmpty(ConfigStampe.CodRisUm_PrimoCessionario) _
        '    AndAlso ConfigStampe.CodRisUm_PrimoCessionario.Contains("|" & x_Cod_RisUm & "|") _
        '    AndAlso Lav_Cod = LAVCOD_BOLLA_EMESSA Then

        '    Parametro_TxtPivaCliente = Piva_Dest
        '    Parametro_TxtCFCliente = Codice_Fiscale_Dest
        'Else
        '    Parametro_TxtPivaCliente = Piva_Cliente
        '    Parametro_TxtCFCliente = Codice_Fiscale_Cliente
        'End If

        parametro.TxtPivaCliente = Piva_Cliente
        parametro.TxtCfCliente = Codice_Fiscale_Cliente
        parametro.CodiceSDI = codiceSDIAcquisto

        drIntestazioneNew.DestinazioneRag_Soc = Destinatario_RagSoc
        drIntestazioneNew.DestinazioneInd_Des = Destinatario_Ind_Des
        drIntestazioneNew.DestinazioneFrz_Des = Destinatario_Frz_Des
        drIntestazioneNew.DestinazioneCap = Destinatario_Cap
        drIntestazioneNew.DestinazioneComune = Destinatario_Comune
        drIntestazioneNew.DestinazioneProvincia = Destinatario_Provincia

        '=============================================================

        '------------------------ VETTORE --------------------------

        Select Case x_Cod_Vettore

            Case 0

                Select Case _lavCod

                    Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_ACCETTAZIONE_DIVERSI

                        If x_Mezzo = 0 Then
                            ' "data spedizione"
                            'nel data entry rimane data consegna
                            ' CType(rptBolla.Section5.ReportObjects("TxtDataSpedizioneConsegna"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Data Spedizione:"
                            drIntestazioneNew.Mezzo = _traduttore.ValoreDizionarioTraduzioneComuni("CEDENTE")
                        Else
                            ' "data consegna"
                            drIntestazioneNew.Mezzo = _traduttore.ValoreDizionarioTraduzioneComuni("CESSIONARIO")
                        End If

                    Case LAVCOD_CONFERIMENTO, LAVCOD_CONFERIMENTO_DIVERSI

                        If x_Mezzo = 0 Then
                            'CType(rptBolla.Section5.ReportObjects("TxtDataSpedizioneConsegna"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Data Spedizione:"
                            ' "data spedizione"
                            drIntestazioneNew.Mezzo = _traduttore.ValoreDizionarioTraduzioneComuni("CONFERENTE")
                        Else
                            '"data consegna"
                            drIntestazioneNew.Mezzo = _traduttore.ValoreDizionarioTraduzioneComuni("CONFERITORE")
                        End If

                End Select

            Case Else
                ' "data spedizione"
                'CType(rptBolla.Section5.ReportObjects("TxtDataSpedizioneConsegna"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Data Spedizione:"

                drIntestazioneNew.Mezzo = _traduttore.ValoreDizionarioTraduzioneComuni("VETTORE")

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

                Ricava_Piva_Codicefiscale(x_Id_Cf_Vettore,
                                        x_CodContatto_Vettore,
                                        "",
                                        x_ChkFittizio_Vettore,
                                        x_CodContatto_Vettore,
                                        "",
                                        Nothing)

                If x_CodContatto_Vettore <> "" Then
                    drIntestazioneNew.VettorePiva = _traduttore.ValoreDizionarioTraduzioneComuni("P. Iva: ") & x_CodContatto_Vettore
                Else
                    drIntestazioneNew.VettorePiva = ""
                End If


                drIntestazioneNew.VettoreTarga = ""
                If Not IsDBNull(x_TargaMezzo_Vettore) AndAlso
                   Len(Trim(x_TargaMezzo_Vettore)) > 0 Then
                    drIntestazioneNew.VettoreTarga = _traduttore.ValoreDizionarioTraduzioneComuni("Targa: ") & x_TargaMezzo_Vettore
                End If

                drIntestazioneNew.VettoreNumReg = ""
                If Not IsDBNull(x_NumReg_Vettore) AndAlso
                   Trim(x_NumReg_Vettore) <> "0" AndAlso Trim(x_NumReg_Vettore) <> "" Then
                    drIntestazioneNew.VettoreNumReg = _traduttore.ValoreDizionarioTraduzioneComuni("Albo: ") & x_NumReg_Vettore
                End If

                Try

                    Leggi_Indirizzi(_objParametriServer,
                                    x_CodContatto_Vettore,
                                    x_Cod_Vettore,
                                    x_Cod_IndirizzoVettore,
                                    x_IndDes_Vettore,
                                    x_FrzDes_Vettore,
                                    x_Cap_Vettore,
                                    x_Comune_Vettore,
                                    x_Provincia_Vettore,
                                    x_Stato_Vettore,
                                    Nothing,
                                    Nothing,
                                    "")

                Catch ex As Exception
                    Log_Errori &= "- Lettura dell'indirizzo del vettore: " & vbCrLf & ex.Message & vbCrLf
                End Try

                Select Case x_Id_Cf_Vettore
                    Case enum_Contatti_IdCf.ContattoEstero

                        'Se il contatto è estero, il comune potrebbe essere esplicitato nella x_Comune dalla tabella ISTAT oppure
                        'come campo libero nella x_Frz_Des della Indirizzi. Nel primo caso, x_Frz_Des potrebbe contenere una effettiva frazione
                        'inserita dall'utente
                        If String.IsNullOrEmpty(x_Comune_Destinazione) Then
                            drIntestazioneNew.VettoreInd_Des = x_IndDes_Vettore
                            drIntestazioneNew.VettoreComune = x_Cap_Vettore & " " & x_FrzDes_Destinazione & " " & x_Stato_Vettore
                        Else
                            drIntestazioneNew.VettoreInd_Des = x_IndDes_Vettore & " " & x_FrzDes_Vettore
                            drIntestazioneNew.VettoreComune = x_Cap_Vettore & " " & x_Comune_Destinazione & " " & x_Stato_Vettore
                        End If

                        drIntestazioneNew.VettoreFrz_Des = "" ' x_FrzDes_Vettore
                        drIntestazioneNew.VettoreCap = "" 'x_Cap_Vettore
                        drIntestazioneNew.VettoreProvincia = ""
                        drIntestazioneNew.VettoreNumReg = ""
                    Case Else
                        drIntestazioneNew.VettoreInd_Des = x_IndDes_Vettore & " " & x_FrzDes_Vettore
                        drIntestazioneNew.VettoreFrz_Des = "" 'x_FrzDes_Vettore
                        drIntestazioneNew.VettoreCap = "" 'x_Cap_Vettore
                        drIntestazioneNew.VettoreComune = x_Cap_Vettore & " " & x_Comune_Vettore & " (" & x_Provincia_Vettore & ")"
                        drIntestazioneNew.VettoreProvincia = "" '"(" & x_Provincia_Vettore & ")"

                End Select

                '17/05/2017 sostituzione dei campi del DataSet con i parametri
                parametro.VettoreRiga1 = drIntestazioneNew.VettoreRag_Soc
                parametro.VettoreRiga2 = drIntestazioneNew.VettoreInd_Des
                parametro.VettoreRiga3 = drIntestazioneNew.VettoreComune
                parametro.VettoreRiga4 = drIntestazioneNew.VettorePiva & " " & drIntestazioneNew.VettoreTarga & " " & drIntestazioneNew.VettoreNumReg
                If x_N_Autorizzazione_Trasporto <> "" Then
                    parametro.VettoreRiga4 &= _traduttore.ValoreDizionarioTraduzioneComuni("N.Autorizz.Trasp.: ") & x_N_Autorizzazione_Trasporto
                End If
                parametro.VettoreRiga4 = parametro.VettoreRiga4.Trim
        End Select


        '=============================================================
        '------------------------ AGENTE -----------------------------
        If x_Agente_Cod <> 0 Then
            parametro.AgenteInfo = ImpostaAgente(Log_Errori, x_Agente_Cod, _objParametriServer)
        End If
        '=============================================================


        '  Giulia, 17/01/2017 10:18:56: se viene usata la tabella configurazione_stampe è possibile gestire le personalizzazioni in questo modo
        '=============================================================
        '------------------ PERSONALIZZAZIONI  -----------------------
        '=============================================================

        ModificaLayoutPersonalizzazioni(drIntestazioneNew, parametro.IntestazioneRiga1)


        '=============================================================
        '----------- SEZIONE DATE / NUMERO / ECC  --------------------
        '=============================================================
        ModificaLayoutTipoVisualizzazione(parametro.LblPrezzoRiscontrato, flag_StampaColonnaNumColliDdtFF)


        Select Case _progressivoGias

            Case enum_CodiceGIAS_Clienti.Fruttagel

                drIntestazioneNew.Doc_Numero = CStr(CStr(x_Doc_Numero_Sin) & Right("00000" & CStr(x_Doc_Numero), 5) & x_Doc_Numero_Des)

                Select Case CStr(x_Doc_Numero_Sin)
                    Case "A"
                        ' CType(rptBolla.Section2.ReportObjects("TxtStabilimento"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "D.d.T. emesso dallo Stabilimento di ALFONSINE"
                        parametro.TitoloStabilimento = _traduttore.ValoreDizionarioTraduzioneComuni("D.d.T. emesso dallo Stabilimento di ALFONSINE")
                    Case "L"
                        'CType(rptBolla.Section2.ReportObjects("TxtStabilimento"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "D.d.T. emesso dallo Stabilimento di LARINO"
                        parametro.TitoloStabilimento = _traduttore.ValoreDizionarioTraduzioneComuni("D.d.T. emesso dallo Stabilimento di LARINO")
                End Select

                'questi indirizzi ora si mettono nei campi opzionali dell'intestazione
                'If Not IsNothing(x_Fabbricato_Des) Then

                '    For i = 0 To x_Fabbricato_Des.Length - 1

                '        Indirizzo_Fabbricato = x_IndDes_Fabbricato(i) & " " & x_FrzDes_Fabbricato(i) & " " & x_Cap_Fabbricato(i) & " " & x_Comune_Fabbricato(i) & " " & "(" & x_Provincia_Fabbricato(i) & ")"

                '        If InStr(1, x_Fabbricato_Des(i).ToLower, "alfonsine") > 0 Then
                '            CType(rptBolla.Section2.ReportObjects("TxtIndirizzoAlfonsine"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Stabilimento di Alfonsine: " & Indirizzo_Fabbricato
                '        End If

                '        If InStr(1, x_Fabbricato_Des(i).ToLower, "larino") > 0 Then
                '            CType(rptBolla.Section2.ReportObjects("TxtIndirizzoLarino"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Stabilimento di Larino: " & Indirizzo_Fabbricato
                '        End If

                '    Next

                'End If

        End Select


        Select Case _lavCod

            Case LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE

                'Numero_Accettazione = x_Doc_Numero_Sin & CStr(x_Doc_Numero) & x_Doc_Numero_Des
                Dim onjContab As New AgronicaCoreContabDAL.Contabilita_R
                Dim numero_ddt As String
                numero_ddt = onjContab.NumeroDoc_from_CauMov(_piva, _idAgenda, CAU_REGISTRAZIONI_ALLEGATE, _objParametriServer)
                drIntestazioneNew.Doc_Numero = numero_ddt

                'Dim objSeq As New AgronicaCoreDataProvider.Sequenza_Progressivi_R

                ''x_Cod_Contatto
                'objSeq.Formattazione_from_Tipo(Piva,
                '                               Year(x_Data_Movimento),
                '                               enum_SequenzaProgressiviTipi.DDTRicevutiAccettazione,
                '                               "",
                '                               "",
                '                               0,
                '                               x_Lunghezza_Sin,
                '                                x_Lunghezza_Centro,
                '                                x_Lunghezza_Des,
                '                                x_CarattereFormattazione,
                '                                objParametri_Server)

                'DrIntestazioneNew.Doc_Numero = Ricava_NumeroDocumento_Con_Sequenza(x_Doc_Numero_Sin,
                '                        x_Doc_Numero,
                '                        x_Doc_Numero_Des,
                '                        x_Lunghezza_Sin,
                '                        x_Lunghezza_Centro,
                '                        x_Lunghezza_Des,
                '                        x_CarattereFormattazione)

            Case LAVCOD_AUTO_DDT_EMESSO, LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA,
                LAVCOD_CONFERIMENTO, LAVCOD_CONFERIMENTO_DIVERSI, LAVCOD_DDT_CONTABILIZZATO_EMESSO
                drIntestazioneNew.Doc_Numero = x_Doc_Numero_Sin & CStr(x_Doc_Numero) & x_Doc_Numero_Des

                If _progressivoGias = enum_CodiceGIAS_Clienti.CoFruTa AndAlso
                   _usoConfigStampe = True AndAlso
                   Not String.IsNullOrEmpty(_objConfigStampe.CodRisUm_PrimoCessionario) AndAlso
                   _objConfigStampe.CodRisUm_PrimoCessionario.Contains("|" & x_Cod_RisUm & "|") AndAlso
                   _lavCod = LAVCOD_BOLLA_EMESSA Then

                    drIntestazioneNew.Doc_Numero = CStr(CStr(x_Doc_Numero_Sin) & Right("00000" & CStr(x_Doc_Numero), 5) & x_Doc_Numero_Des)

                End If

        End Select

        'CType(rptBolla.Section2.ReportObjects("TxtAlfonsine"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        'CType(rptBolla.Section2.ReportObjects("TxtLarino"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""

        'If x_AlboCoop = "" Then
        '    CType(rptBolla.Section2.ReportObjects("TxtNumSocCoop"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        'End If

        '   End Select

        ' mi serve per il salvataggio del pdf
        _identNumero = drIntestazioneNew.Doc_Numero


        drIntestazioneNew.Data_Movimento = CType(x_Data_Movimento, DateTime)

        _dataInizioAllegato = x_Data_Movimento
        _identData = Format(x_Data_Movimento, "yyyy-MM-dd")

        'riciclo questo campo per la data per fare in modo che il crystal report visualizzi la data in italiano
        drIntestazioneNew.NSBanca = CType(x_Data_Movimento, DateTime)

        Dim toNote = ""

        If _moduloCantine AndAlso Not _moduloFreshFood AndAlso _lavCod = LAVCOD_ACCETTAZIONE_DIVERSI Then
            'In questo caso specifico, le note che si vogliono stampare sono nella colonna mov_desc
            toNote = x_Mov_Desc_Contabile
        Else
            toNote = If(String.IsNullOrWhiteSpace(x_Extra_Str), x_Mov_Desc_Contabile, x_Extra_Str)
        End If

        drIntestazioneNew.Note = Replace(toNote, "?", "€")
        drIntestazioneNew.Importo = x_Num_Protocollo
        drIntestazioneNew.Aspetto = x_Aspetto
        drIntestazioneNew.Colli = x_Colli
        drIntestazioneNew.Causale_Trasporto = x_Causale_Trasporto
        drIntestazioneNew.Gest_Vettore = x_Gestione_Vettore



        drIntestazioneNew.Peso = x_Peso
        Select Case x_Tipo_Peso
            Case 0 'peso lordo
                parametro.TipoPeso = _traduttore.ValoreDizionarioTraduzioneComuni("Peso lordo tot")
            Case 1 'peso netto
                parametro.TipoPeso = _traduttore.ValoreDizionarioTraduzioneComuni("Peso netto tot")
        End Select

        Dim taraTotale As Decimal = ArrotondaVal_2(x_Tara_Imballi)
        Dim pesoNettoTotale As Decimal = ArrotondaVal_2(Calcola_PesoNetto(x_Tipo_Peso, x_Peso, taraTotale))
        Dim pesoLordoTotale As Decimal = ArrotondaVal_2(Calcola_PesoLordo(x_Tipo_Peso, x_Peso, taraTotale))

        'TODO: in questo caso devo sostituire tutti i valori di testata con la sommatoria di quello che si vede nel corpo (alcuni potrebbero essere i riscontrati, altri reali) ==> è UN CASINO!
        If _tipoLayout = enum_TipoLayoutDDT.Tutto_Riscontrato_se_disponibile_piu_Prezzo Then

        End If

        If _moduloFreshFood = True Then
            drIntestazioneNew.Peso_Lordo = FormattaArrotondaFF(_tipoArrotondamentoFF, pesoLordoTotale, "")
            drIntestazioneNew.Peso_Netto = FormattaArrotondaFF(_tipoArrotondamentoFF, pesoNettoTotale, "")
            drIntestazioneNew.Extra_Str_2 = FormattaArrotondaFF(_tipoArrotondamentoFF, taraTotale, "")
        Else
            drIntestazioneNew.Peso_Lordo = Format(pesoLordoTotale, "#,###,##0.##")
            drIntestazioneNew.Peso_Netto = Format(pesoNettoTotale, "#,###,##0.##")
            drIntestazioneNew.Extra_Str_2 = Format(taraTotale, "#,###,##0.##")
        End If

        'modifica del 22/04/2015: stampata oltre alla data di spedizione anche l'ora (che in passato era stata tolta)
        If IsDate(x_Ora) Then
            If CDate(CDate(x_Ora).ToShortDateString) = AGRODATAFINE OrElse CDate(CDate(x_Ora).ToShortDateString) = AGRODATAINIZIO Then
                drIntestazioneNew.Scadenza = ""
            Else
                '31/08/2018:
                'drIntestazioneNew.Scadenza = Format(CDate(x_Ora), "dd/MM/yyyy HH:mm") 'CDate(x_Ora).ToShortDateString
                drIntestazioneNew.Scadenza = Format(CDate(x_Ora), "dd/MM/yyyy")
                If Format(CDate(x_Ora), "HH:mm") <> "00:00" Then
                    drIntestazioneNew.Scadenza &= " " & Format(CDate(x_Ora), "HH:mm")
                End If
            End If
            x_Scadenza = CDate(x_Ora).ToShortDateString
        Else
            drIntestazioneNew.Scadenza = ""
        End If

        _dataFineAllegato = x_Scadenza

        '=============================================================
        '--------------------- PAGAMENTI --------------------------

        _rptBolla.PageHeaderSection1.SectionFormat.EnableSuppress = True
        drIntestazioneNew.VSBanca = ""

        '  Giulia, 09/05/2017 12:25:21: DeFaveri lo vuole stampato
        If _progressivoGias = enum_CodiceGIAS_Clienti.DeFaveri Then
            AggiungiDettagliPagamento(drIntestazioneNew, x_Id_Mov_Contabile, x_Num_Protocollo)
        End If

        NascondiLoghiVuoti(Log_Errori, drIntestazioneNew, _rptBolla, _objParametriServer)

        Return drIntestazioneNew

    End Function

    Private Sub ModificaParametriIntestazioneGestioneMaterialeVivaistico(ByRef parametro As Parametri, ByVal Flag_GestMaterialeVivaistico As Boolean, ByVal documentoSaCod As Integer)
        'sviluppo x zespribud
        If Flag_GestMaterialeVivaistico = True AndAlso documentoSaCod <> 0 Then

            Dim Rag_Soc_Centro, Sa_Nome, x_Indirizzo1_Sede_Operativa, x_Indirizzo2_Sede_Operativa, x_eMail As String

            Leggi_CentroAziendale_Partenza(_objParametriServer,
                                           _logErrori,
                                           _piva,
                                           documentoSaCod,
                                           Rag_Soc_Centro,
                                           Sa_Nome,
                                           x_Indirizzo1_Sede_Operativa,
                                           x_Indirizzo2_Sede_Operativa,
                                           x_eMail)

            parametro.IntestazioneRiga12 = _traduttore.ValoreDizionarioTraduzioneComuni("Partenza: ") & Sa_Nome

            '-------------
            'parametro.IntestazioneRiga13 = _piva & " " & Rag_Soc_Centro
            'L’esigenza nasce dal fatto che ci può essere un luogo di partenza diverso (cosa che non abbiamo mai considerato).
            'Per il momento il luogo di partenza coincide con il centro aziendale scelto all’ingresso nel documento. 
            'L'avevamo messo per gestire la numerazione diversa dei documenti (e anche per visibilità magazzini etc.) e nell’occasione ci ha fatto comodo per gestire il luogo di partenza. 
            'In questo momento di fatto non si può proprio scegliere un centro aziendale di un’altra impresa diversa da chi emette il documento. 
            'Quindi se diventa un casino puoi tranquillamente dare per scontato che il centro che trovi nei dettagli (alias centro di ingresso nel documento, alias luogo di partenza) appartenga alla piva di chi emette il doc. 
            'In futuro l’esigenza comprenderà la possibilità di selezionarlo il luogo di partenza (non di dare per scontato che sia il centro aziendale di ingresso).
            'Questo perché ci sono casistiche che non abbiamo mai gestito in cui chi emette il DDT dichiara luogo di partenza un’impresa GIAS diversa (non direi un contatto ma vedremo).
            'Ad esempio lo fanno spesso le OP che emettono DDT dalla loro PIVA come se il prodotto fosse nel loro magazzino ma in realtà indicano l’azienda agricola socia che effettivamente carica il camion nel luogo di partenza. Etc. 
            parametro.IntestazioneRiga13 = "" 'modificheremo quando si gestirà la casistica.
            '-----------------------------------
            parametro.IntestazioneRiga14 = x_Indirizzo1_Sede_Operativa
            parametro.IntestazioneRiga15 = x_Indirizzo2_Sede_Operativa

        End If
    End Sub

    Private Sub ModificaParametriIntestazioneCentroAziendalePartenza(ByRef parametro As Parametri, ByVal Flag_CentroAziendalePartenza As Boolean, ByVal documentoSaCod As Integer)
        'sviluppo x zespribud
        If Flag_CentroAziendalePartenza = True AndAlso documentoSaCod <> 0 Then

            Dim Rag_Soc_Centro, Sa_Nome, x_Indirizzo1_Sede_Operativa, x_Indirizzo2_Sede_Operativa, x_eMail As String

            Leggi_CentroAziendale_Partenza(_objParametriServer,
                                           _logErrori,
                                           _piva,
                                           documentoSaCod,
                                           Rag_Soc_Centro,
                                           Sa_Nome,
                                           x_Indirizzo1_Sede_Operativa,
                                           x_Indirizzo2_Sede_Operativa,
                                           x_eMail)

            parametro.IntestazioneRiga12 = _traduttore.ValoreDizionarioTraduzioneComuni("Partenza: ") & Sa_Nome

            '-------------
            'parametro.IntestazioneRiga13 = _piva & " " & Rag_Soc_Centro
            'L’esigenza nasce dal fatto che ci può essere un luogo di partenza diverso (cosa che non abbiamo mai considerato).
            'Per il momento il luogo di partenza coincide con il centro aziendale scelto all’ingresso nel documento. 
            'L'avevamo messo per gestire la numerazione diversa dei documenti (e anche per visibilità magazzini etc.) e nell’occasione ci ha fatto comodo per gestire il luogo di partenza. 
            'In questo momento di fatto non si può proprio scegliere un centro aziendale di un’altra impresa diversa da chi emette il documento. 
            'Quindi se diventa un casino puoi tranquillamente dare per scontato che il centro che trovi nei dettagli (alias centro di ingresso nel documento, alias luogo di partenza) appartenga alla piva di chi emette il doc. 
            'In futuro l’esigenza comprenderà la possibilità di selezionarlo il luogo di partenza (non di dare per scontato che sia il centro aziendale di ingresso).
            'Questo perché ci sono casistiche che non abbiamo mai gestito in cui chi emette il DDT dichiara luogo di partenza un’impresa GIAS diversa (non direi un contatto ma vedremo).
            'Ad esempio lo fanno spesso le OP che emettono DDT dalla loro PIVA come se il prodotto fosse nel loro magazzino ma in realtà indicano l’azienda agricola socia che effettivamente carica il camion nel luogo di partenza. Etc. 
            parametro.IntestazioneRiga13 = "" 'modificheremo quando si gestirà la casistica.
            '-----------------------------------
            parametro.IntestazioneRiga14 = x_Indirizzo1_Sede_Operativa
            parametro.IntestazioneRiga15 = x_Indirizzo2_Sede_Operativa

            If _lavCod = LAVCOD_BOLLA_EMESSA Then
                parametro.IntestazioneRiga8 = x_eMail
            End If

        End If
    End Sub

    Private Sub ModificaParametriIntestazioneGestioneImpresaEsterna(ByRef parametro As Parametri, ByRef Log_Errori As String, ByVal x_Id_Cf_Cliente As Integer, ByVal Flag_GestMaterialeVivaistico As Boolean, ByVal impresaEsternaPiva As String, ByVal impresaEsternaSaCod As Integer)

        'Caso di impresa esterna, tipico di imprese di Organizzazione Produttori come Lorenzini

        If impresaEsternaPiva = "" Then
            Return
        End If

        parametro.QualificaSecondoContatto = "Destinatario:"
        parametro.QualificaQuartoContatto = "Cessionario:"

        Dim impresaEsternaCF As String = ""
        Dim impresaEsternaRagSoc As String = ""
        'Dim impresaEsternaSaNome As String = ""
        Dim impresaEsternaIndir As String = ""
        Dim impresaEsternaFrazione As String = ""
        Dim impresaEsternaComune As String = ""
        Dim impresaEsternaCAP As String = ""
        Dim impresaEsternaProvincia As String = ""

        Dim impresaEsternaPivaCF As String = ""
        Dim impresaEsternaCodici As String = ""
        Dim impresaEsternaRegImprese As String = ""
        Dim impresaEsternaSedeLegaleIndir As String = ""
        Dim impresaEsternaSedeLegaleComune As String = ""
        Dim impresaEsternaSedeLegaleContatti As String = ""
        Dim impresaEsternaEmailSitoWeb As String = ""
        Dim impresaEsternaPec As String = ""
        Dim impresaEsternaSocial As String = ""
        Dim impresaEsternaSedeOperativaIndir As String = ""
        Dim impresaEsternaSedeOperativaComune As String = ""
        Dim impresaEsternaSedeOperativaContatti As String = ""
        Dim impresaEsternaJolly1 As String = ""
        Dim impresaEsternaJolly2 As String = ""
        Dim impresaEsternaGlobalGap As String = ""

        Leggi_Intestazione_Impresa_2(_objParametriServer,
                                             _progressivoGias,
                                             False,
                                             False,
                                             x_Id_Cf_Cliente, 'Se il cliente (ovvero il cessionario) è estero, specifico la nazione della mia azienda cedente
                                             Log_Errori,
                                             impresaEsternaPiva,
                                             impresaEsternaCF,
                                             impresaEsternaRagSoc,
                                             impresaEsternaIndir,
                                             impresaEsternaFrazione,
                                             impresaEsternaCAP,
                                             impresaEsternaComune,
                                             impresaEsternaProvincia,
                                             "",
                                             impresaEsternaCodici,
                                             impresaEsternaRegImprese,
                                             impresaEsternaSedeLegaleIndir,
                                             impresaEsternaSedeLegaleComune,
                                             impresaEsternaSedeLegaleContatti,
                                             impresaEsternaEmailSitoWeb,
                                             impresaEsternaPec,
                                             impresaEsternaSocial,
                                             impresaEsternaSedeOperativaIndir,
                                             impresaEsternaSedeOperativaComune,
                                             impresaEsternaSedeOperativaContatti,
                                             impresaEsternaJolly1,
                                             impresaEsternaJolly2,
                                             Flag_GestMaterialeVivaistico,
                                             _traduttore,
                                             impresaEsternaGlobalGap,
                                             Flag_CentroAziendalePartenza:=False)


        parametro.IntestazioneRiga1 = impresaEsternaRagSoc

        If x_Id_Cf_Cliente = enum_Contatti_IdCf.ContattoEstero Then
            parametro.IntestazioneRiga2 = _traduttore.ValoreDizionarioTraduzioneComuni("VAT: IT") & impresaEsternaPiva
        Else
            parametro.IntestazioneRiga2 = _traduttore.ValoreDizionarioTraduzioneComuni("Partita IVA: ") & impresaEsternaPiva
        End If

        If impresaEsternaCF <> "" Then
            parametro.IntestazioneRiga2 &= _traduttore.ValoreDizionarioTraduzioneComuni("   Codice Fiscale: ") & impresaEsternaCF
        End If

        'Per questa casistica, questa impresa è l'effettivo cedente del bene, imposto quindi questa come primo contatto e la
        'impresa che emette il documento viene impostata nel riquadro del "cessionario aggiuntivo"
        parametro.IntestazioneRiga3 = impresaEsternaCodici
        parametro.IntestazioneRiga4 = impresaEsternaRegImprese
        'Intestazione_Riga5 = impresaEsternaSedeLegaleIndir
        'Intestazione_Riga6 = impresaEsternaSedeLegaleComune
        'Intestazione_Riga7 = impresaEsternaSedeLegaleContatti
        'Intestazione_Riga8 = impresaEsternaEmailSitoWeb
        'Intestazione_Riga9 = impresaEsternaPec
        'Intestazione_Riga10 = impresaEsternaSocial
        'Intestazione_Riga11 = impresaEsternaSedeOperativaIndir
        'Intestazione_Riga12 = impresaEsternaSedeOperativaComune
        'Intestazione_Riga13 = impresaEsternaSedeOperativaContatti
        'Intestazione_Riga14 = impresaEsternaJolly1
        'Intestazione_Riga15 = impresaEsternaJolly2
        parametro.IntestazioneRiga12 = If(impresaEsternaGlobalGap <> "", "Azienda Certificata Global G.A.P", "")


        Dim Rag_Soc_Centro, Sa_Nome, x_Indirizzo1_Sede_Operativa, x_Indirizzo2_Sede_Operativa, x_eMail As String

        Leggi_CentroAziendale_Partenza(_objParametriServer,
                                           _logErrori,
                                           impresaEsternaPiva,
                                           impresaEsternaSaCod,
                                           Rag_Soc_Centro,
                                           Sa_Nome,
                                           x_Indirizzo1_Sede_Operativa,
                                           x_Indirizzo2_Sede_Operativa,
                                           x_eMail)


        'parametro.IntestazioneRiga5 = _traduttore.ValoreDizionarioTraduzioneComuni("Partenza: ") & Sa_Nome & " " & x_Indirizzo1_Sede_Operativa
        parametro.IntestazioneRiga5 = x_Indirizzo1_Sede_Operativa
        parametro.IntestazioneRiga6 = x_Indirizzo2_Sede_Operativa


        Dim handleRubrica As New AgronicaCoreAnagrafeBIZ.Rubrica_R()
        Dim listaImpresaEsternaCentroRubrica = handleRubrica.Leggi_Rubrica_Centro(impresaEsternaPiva, impresaEsternaSaCod, _objParametriServer)

        If listaImpresaEsternaCentroRubrica IsNot Nothing AndAlso listaImpresaEsternaCentroRubrica.Count > 0 Then
            'Ho una solo contatto per tipo (ovvero ho un solo numero di telefono, una sola mail ecc..)

            Dim listaTelFaxCel As New List(Of String)()

            Dim listaImpresaEsternaCentroTel = listaImpresaEsternaCentroRubrica.Where(Function(r) r.rubrica.tipologia.ToLower().Contains("tel"))

            If listaImpresaEsternaCentroTel.Count > 0 Then
                listaTelFaxCel.Add(_traduttore.ValoreDizionarioTraduzioneComuni("Tel: ") & listaImpresaEsternaCentroTel.First().valore)
            End If

            Dim listaImpresaEsternaCentrofax = listaImpresaEsternaCentroRubrica.Where(Function(r) r.rubrica.tipologia.ToLower().Contains("fax"))

            If listaImpresaEsternaCentrofax.Count > 0 Then
                listaTelFaxCel.Add(_traduttore.ValoreDizionarioTraduzioneComuni(" Fax: ") & listaImpresaEsternaCentrofax.First().valore)
            End If

            Dim listaImpresaEsternaCentrocel = listaImpresaEsternaCentroRubrica.Where(Function(r) r.rubrica.tipologia.ToLower().Contains("cell"))

            If listaImpresaEsternaCentrocel.Count > 0 Then
                listaTelFaxCel.Add(_traduttore.ValoreDizionarioTraduzioneComuni(" Cell: ") & listaImpresaEsternaCentrocel.First().valore)
            End If

            parametro.IntestazioneRiga7 = String.Join("; ", listaTelFaxCel)


            'Dim listaMailSitoWebPec As New List(Of String)()

            Dim listaImpresaEsternaCentromail = listaImpresaEsternaCentroRubrica.Where(Function(r) r.rubrica.tipologia.ToLower().Contains("mail"))

            If listaImpresaEsternaCentromail.Count > 0 Then
                'listaMailSitoWeb.Add(_traduttore.ValoreDizionarioTraduzioneComuni("mail: ") & listaImpresaEsternaCentromail.First().valore)
                'listaMailSitoWebPec.Add(listaImpresaEsternaCentromail.First().valore)
                parametro.IntestazioneRiga8 = listaImpresaEsternaCentromail.First().valore
            Else
                parametro.IntestazioneRiga8 = ""
            End If

            Dim listaImpresaEsternaCentrosito = listaImpresaEsternaCentroRubrica.Where(Function(r) r.rubrica.tipologia.ToLower().Contains("web"))

            If listaImpresaEsternaCentrosito.Count > 0 Then
                'listaMailSitoWeb.Add(_traduttore.ValoreDizionarioTraduzioneComuni("sito: ") & listaImpresaEsternaCentrosito.First().valore)
                'listaMailSitoWebPec.Add(listaImpresaEsternaCentrosito.First().valore)
                parametro.IntestazioneRiga9 = listaImpresaEsternaCentrosito.First().valore
            Else
                parametro.IntestazioneRiga9 = ""
            End If

            Dim listaImpresaEsternaCentroPec = listaImpresaEsternaCentroRubrica.Where(Function(r) r.rubrica.tipologia.ToLower().Contains("pec"))

            If listaImpresaEsternaCentroPec.Count > 0 Then
                'listaMailSitoWebPec.Add(_traduttore.ValoreDizionarioTraduzioneComuni("PEC: ") & listaImpresaEsternaCentroPec.First().valore)
                parametro.IntestazioneRiga10 = _traduttore.ValoreDizionarioTraduzioneComuni("PEC: ") & listaImpresaEsternaCentroPec.First().valore
            Else
                parametro.IntestazioneRiga10 = ""
            End If

            'parametro.IntestazioneRiga8 = String.Join("; ", listaMailSitoWebPec)

            Dim listaImpresaEsternaCentrosocial = listaImpresaEsternaCentroRubrica.Where(Function(r) r.rubrica.tipologia.ToLower().Contains("social"))

            If listaImpresaEsternaCentrosocial.Count > 0 Then
                'parametro.IntestazioneRiga10 = _traduttore.ValoreDizionarioTraduzioneComuni("social: ") & listaImpresaEsternaCentrosocial.First().valore
                parametro.IntestazioneRiga11 = listaImpresaEsternaCentrosocial.First().valore
            Else
                parametro.IntestazioneRiga11 = ""
            End If

            Dim listaImpresaEsternaCentrojolly1 = listaImpresaEsternaCentroRubrica.Where(Function(r) r.rubrica.tipologia.ToLower().Contains("jolly1"))

            If listaImpresaEsternaCentrojolly1.Count > 0 Then
                parametro.IntestazioneRiga13 = listaImpresaEsternaCentrojolly1.First().valore
            Else
                parametro.IntestazioneRiga13 = ""
            End If

        Else
            parametro.IntestazioneRiga7 = ""
            parametro.IntestazioneRiga8 = ""
            parametro.IntestazioneRiga9 = ""
            parametro.IntestazioneRiga10 = ""
            parametro.IntestazioneRiga11 = ""
            parametro.IntestazioneRiga13 = ""
        End If

        parametro.IntestazioneRiga14 = ""
        parametro.IntestazioneRiga15 = ""

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

            'stampiamo solo info generali sul pagamento, non tutti i dettagli della banca

            'If x_RisorsaFinanziaria_Dare <> "" Then
            '    nsBanca = " - NS banca: " & x_RisorsaFinanziaria_Dare
            'End If
            'If x_RisorsaFinanziaria_Avere <> "" Then
            '    vsBanca = " - VS banca: " & x_RisorsaFinanziaria_Avere
            'End If

            drIntestazioneNew.VSBanca = pagamentoDesc & nsBanca & vsBanca

            If drIntestazioneNew.VSBanca <> "" Then
                'abilito la sezione
                _rptBolla.PageHeaderSection1.SectionFormat.EnableSuppress = False
            End If

        Catch ex As Exception
            _logErrori &= "- Lettura dei pagamenti del ddt: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Sub ModificaLayoutTipoVisualizzazione(ByRef Parametro_LblPrezzoRiscontrato As String,
                                                  ByVal flag_StampaColonnaNumColliDdtFF As Boolean)

        Try

            Select Case _tipoLayout

                Case enum_TipoLayoutDDT.Pesi_Reali_piu_Prezzo, enum_TipoLayoutDDT.Tutto_Riscontrato_se_disponibile_piu_Prezzo   'Visualizza Pesi + Tutti i riscontrati, se esistenti

                    'SEZIONE STANDARD
                    _rptBolla.Section6.SectionFormat.EnableSuppress = True
                    _rptBolla.Section3.SectionFormat.EnableSuppress = True

                    'SEZIONE COL PREZZO
                    _rptBolla.Section12.SectionFormat.EnableSuppress = True
                    _rptBolla.Section13.SectionFormat.EnableSuppress = True

                    If flag_StampaColonnaNumColliDdtFF = False Then
                        'SEZIONE  VISUALIZZA PESI
                        _rptBolla.Section7.SectionFormat.EnableSuppress = False
                        _rptBolla.Section8.SectionFormat.EnableSuppress = False

                        'SEZIONE  VISUALIZZA PESI + NUM_COLLI (RICHIESTA MASSEI)
                        _rptBolla.PageHeaderSection6.SectionFormat.EnableSuppress = True
                        _rptBolla.DetailSection2.SectionFormat.EnableSuppress = True
                    Else
                        'SEZIONE  VISUALIZZA PESI
                        _rptBolla.Section7.SectionFormat.EnableSuppress = True
                        _rptBolla.Section8.SectionFormat.EnableSuppress = True

                        'SEZIONE  VISUALIZZA PESI + NUM_COLLI (RICHIESTA MASSEI)
                        _rptBolla.PageHeaderSection6.SectionFormat.EnableSuppress = False
                        _rptBolla.DetailSection2.SectionFormat.EnableSuppress = False

                        Try

                            ' Dim linea As ReportObject = _rptBolla.ReportDefinition.ReportObjects("LineaCodArt")
                            '  Dim objectFormat As ObjectFormat = linea.ObjectFormat
                            ' objectFormat.EnableSuppress = False

                            CType(_rptBolla.DetailSection2.ReportObjects("LineaCodArt"), CrystalDecisions.CrystalReports.Engine.LineObject).ObjectFormat.EnableSuppress = True
                        Catch ex As Exception

                        End Try
                    End If

                    GestioneCampiTotaleDDT(False)

                Case enum_TipoLayoutDDT.Pesi_Reali_piu_Riscontrati  'Visualizza Pesi + valori riscontrati

                    'SEZIONE STANDARD
                    _rptBolla.Section6.SectionFormat.EnableSuppress = True
                    _rptBolla.Section3.SectionFormat.EnableSuppress = True

                    'SEZIONE COL PREZZO
                    _rptBolla.Section12.SectionFormat.EnableSuppress = True
                    _rptBolla.Section13.SectionFormat.EnableSuppress = True

                    If flag_StampaColonnaNumColliDdtFF = False Then
                        'SEZIONE  VISUALIZZA PESI
                        _rptBolla.Section7.SectionFormat.EnableSuppress = False
                        _rptBolla.Section8.SectionFormat.EnableSuppress = False

                        'SEZIONE  VISUALIZZA PESI + NUM_COLLI (RICHIESTA MASSEI)
                        _rptBolla.PageHeaderSection6.SectionFormat.EnableSuppress = True
                        _rptBolla.DetailSection2.SectionFormat.EnableSuppress = True
                    Else
                        'SEZIONE  VISUALIZZA PESI
                        _rptBolla.Section7.SectionFormat.EnableSuppress = True
                        _rptBolla.Section8.SectionFormat.EnableSuppress = True

                        'SEZIONE  VISUALIZZA PESI + NUM_COLLI (RICHIESTA MASSEI)
                        _rptBolla.PageHeaderSection6.SectionFormat.EnableSuppress = False
                        _rptBolla.DetailSection2.SectionFormat.EnableSuppress = False

                        Try

                            ' Dim linea As ReportObject = _rptBolla.ReportDefinition.ReportObjects("LineaCodArt")
                            '  Dim objectFormat As ObjectFormat = linea.ObjectFormat
                            ' objectFormat.EnableSuppress = False

                            CType(_rptBolla.DetailSection2.ReportObjects("LineaCodArt"), CrystalDecisions.CrystalReports.Engine.LineObject).ObjectFormat.EnableSuppress = True
                        Catch ex As Exception

                        End Try
                    End If

                    Parametro_LblPrezzoRiscontrato = _traduttore.ValoreDizionarioTraduzioneComuni("Kg Fattura")

                    GestioneCampiTotaleDDT(False)

                Case enum_TipoLayoutDDT.Dettagli_Economici  ' visualizzazione dettagli economici

                    'SEZIONE STANDARD
                    _rptBolla.Section6.SectionFormat.EnableSuppress = True
                    _rptBolla.Section3.SectionFormat.EnableSuppress = True

                    'SEZIONE COL PREZZO
                    _rptBolla.Section12.SectionFormat.EnableSuppress = False
                    _rptBolla.Section13.SectionFormat.EnableSuppress = False

                    'SEZIONE  VISUALIZZA PESI
                    _rptBolla.Section7.SectionFormat.EnableSuppress = True
                    _rptBolla.Section8.SectionFormat.EnableSuppress = True

                    'SEZIONE  VISUALIZZA PESI + NUM_COLLI (RICHIESTA MASSEI)
                    _rptBolla.PageHeaderSection6.SectionFormat.EnableSuppress = True
                    _rptBolla.DetailSection2.SectionFormat.EnableSuppress = True

                    'TODO: Giulia ri-calcolare il totale DDT e renderlo di nuovo visibile
                    '  Giulia, 17/01/2017 13:09:20: temporaneamente totale DDT nascosto perché non è memorizzato direttamente sul db, ma va ri-calcolato
                    '               come per le fatture ==> sarebbe meglio estrapolare questa parte e fare una funzione apposita, tanto il DataSet usato è lo stesso
                    GestioneCampiTotaleDDT(True)
                    'GestioneCampiTotaleDDT(False)

                Case enum_TipoLayoutDDT.Standard    ' visualizzazione standard

                    'SEZIONE STANDARD
                    _rptBolla.Section6.SectionFormat.EnableSuppress = False
                    _rptBolla.Section3.SectionFormat.EnableSuppress = False

                    'SEZIONE COL PREZZO
                    _rptBolla.Section12.SectionFormat.EnableSuppress = True
                    _rptBolla.Section13.SectionFormat.EnableSuppress = True

                    'SEZIONE  VISUALIZZA PESI
                    _rptBolla.Section7.SectionFormat.EnableSuppress = True
                    _rptBolla.Section8.SectionFormat.EnableSuppress = True

                    'SEZIONE  VISUALIZZA PESI + NUM_COLLI (RICHIESTA MASSEI)
                    _rptBolla.PageHeaderSection6.SectionFormat.EnableSuppress = True
                    _rptBolla.DetailSection2.SectionFormat.EnableSuppress = True

                    GestioneCampiTotaleDDT(False)

            End Select

        Catch ex As Exception
            _logErrori &= "- Tipo visualizzazione Layout: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Sub ModificaLayoutPersonalizzazioni(ByRef DrIntestazioneNew As DataSetFattura.IntestazioneFatturaRow,
                                                ByRef Parametro_Intestazione_Riga1 As String)
        Try

            If _usoConfigStampe = True Then

                '------------------ PERSONALIZZAZIONI / SEZIONI DA NASCONDERE  -------------------------
                If Not _objConfigStampe.SezioniDaNascondereArray Is Nothing Then
                    For Each sec In _rptBolla.ReportDefinition.Sections
                        If _objConfigStampe.SezioniDaNascondereArray.Contains(sec.Name) Then
                            sec.SectionFormat.EnableSuppress = True
                        End If
                    Next
                End If

                '------------------ PERSONALIZZAZIONI / CARICAMENTO LOGO -------------------------------
                ' Possibilità: LogoInAlto, Logo (= logo in basso), LogoHeader, LogoFooter
                '---------------------------------------------------------------------------------------
                If Not _objConfigStampe.PosizioneLogo Is Nothing AndAlso Not String.IsNullOrEmpty(_objConfigStampe.PosizioneLogo) Then
                    CaricaLogoInCampoBlobFattura(_logErrori, DrIntestazioneNew, _piva, _objConfigStampe.PosizioneLogo)

                    If _objConfigStampe.PosizioneLogo = STAMPE_CONTAB_LOGO_IN_ALTO Then
                        'svuoto la ragione sociale grande in alto 
                        DrIntestazioneNew.SuperRag_Soc = ""
                    ElseIf _objConfigStampe.PosizioneLogo = STAMPE_CONTAB_LOGO_IN_BASSO Then
                        'imposto la ragione sociale grande in alto 
                        DrIntestazioneNew.SuperRag_Soc = Parametro_Intestazione_Riga1
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
                '  Giulia, 17/01/2017 11:07:28: se non ho il ConfigStampe, allora faccio il giro vecchio verificando il codice cliente

                '=============================================================
                '------------------ LOGO / INTESTAZIONE  --------------------------

                'ragione sociale sotto al logo
                Select Case _progressivoGias

                    '=========================================
                    '===== LOGO IN ALTO, RAG_SOC IN BASSO  =====
                    '=========================================
                    Case enum_CodiceGIAS_Clienti.FattoriaMonticinoRosso,
                        enum_CodiceGIAS_Clienti.Zuffa,
                        enum_CodiceGIAS_Clienti.TenutaCroci,
                        enum_CodiceGIAS_Clienti.Diamanti,
                        enum_CodiceGIAS_Clienti.Montegrande,
                        enum_CodiceGIAS_Clienti.SantaLucia,
                        enum_CodiceGIAS_Clienti.Pelliconi,
                        enum_CodiceGIAS_Clienti.Molinelli,
                        enum_CodiceGIAS_Clienti.PoderePalazzo,
                        enum_CodiceGIAS_Clienti.TenutaColleAngeli,
                        enum_CodiceGIAS_Clienti.PodereBianchi,
                        enum_CodiceGIAS_Clienti.CaPruccolo,
                        enum_CodiceGIAS_Clienti.CasaZanni,
                        enum_CodiceGIAS_Clienti.AlCanevon,
                        enum_CodiceGIAS_Clienti.FragolaDeBosc,
                        enum_CodiceGIAS_Clienti.PaoloBea,
                        enum_CodiceGIAS_Clienti.CollinaDeiPoeti,
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


                        _rptBolla.Section2.SectionFormat.EnableSuppress = False             'intestazione x tutti
                        'rptBolla.PageHeaderSection2.SectionFormat.EnableSuppress = True    'intestazione Omina Romana
                        'rptBolla.PageHeaderSection3.SectionFormat.EnableSuppress = True    'intestazione Trombin
                        _rptBolla.PageHeaderSection5.SectionFormat.EnableSuppress = True    'intestazione Maiorano

                        ' CType(rptBolla.Section2.ReportObjects("TxtRagSocZeoli"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = x_RagSoc_Impresa
                        DrIntestazioneNew.SuperRag_Soc = ""
                        'già omesso come formula sul report
                        'CType(rptBolla.Section2.ReportObjects("TxtRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""


                        '*************************************************************************************************
                        'RB x CRBolla2016.rpt
                        'GESTIONE CARICAMENTO LOGO DINAMICO (FATTO PER I CLIENTI CHE ATTUALMENTE SONO SU FOSFORO)
                        Select Case _progressivoGias
                            Case enum_CodiceGIAS_Clienti.AlCanevon,
                                enum_CodiceGIAS_Clienti.LaRizzola,
                                enum_CodiceGIAS_Clienti.Lorenzato,
                                enum_CodiceGIAS_Clienti.FattoriaMonticinoRosso,
                                enum_CodiceGIAS_Clienti.PoderePalazzo,
                                enum_CodiceGIAS_Clienti.Zuffa,
                                enum_CodiceGIAS_Clienti.FiorentinaDiSopra

                                CaricaLogoInCampoBlobFattura(_logErrori, DrIntestazioneNew, _piva, STAMPE_CONTAB_LOGO_IN_ALTO)

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
                        enum_CodiceGIAS_Clienti.DeFaveri

                        _rptBolla.Section2.SectionFormat.EnableSuppress = False             'intestazione x tutti
                        'rptBolla.PageHeaderSection2.SectionFormat.EnableSuppress = True    'intestazione Omina Romana
                        'rptBolla.PageHeaderSection3.SectionFormat.EnableSuppress = True    'intestazione Trombin
                        _rptBolla.PageHeaderSection5.SectionFormat.EnableSuppress = True    'intestazione Maiorano

                        ' CType(rptBolla.Section2.ReportObjects("TxtRagSocZeoli"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
                        'DrIntestazioneNew.SuperRag_Soc = Intestazione_RagSoc
                        'imposto la ragione sociale grande in alto 
                        DrIntestazioneNew.SuperRag_Soc = Parametro_Intestazione_Riga1
                        'svuoto il parametro ragione sociale piccola in basso
                        Parametro_Intestazione_Riga1 = ""


                        '*************************************************************************************************
                        'RB x CRBolla2016_LB.rpt
                        'GESTIONE CARICAMENTO LOGO DINAMICO (FATTO PER I CLIENTI CHE ATTUALMENTE SONO SU FOSFORO)
                        Select Case _progressivoGias
                            Case enum_CodiceGIAS_Clienti.Guarini,
                                enum_CodiceGIAS_Clienti.Randi,
                                enum_CodiceGIAS_Clienti.Bartolini,
                                enum_CodiceGIAS_Clienti.DeFaveri

                                CaricaLogoInCampoBlobFattura(_logErrori, DrIntestazioneNew, _piva, STAMPE_CONTAB_LOGO_IN_BASSO)
                                'Prova
                                'DrIntestazioneNew.LogoInAlto = My.Computer.FileSystem.ReadAllBytes("c:\temp\prova\LOGO.jpg")
                        End Select
                        '*************************************************************************************************


                    Case enum_CodiceGIAS_Clienti.OminaRomana

                        _rptBolla.Section2.SectionFormat.EnableSuppress = True              'intestazione x tutti
                        _rptBolla.PageHeaderSection2.SectionFormat.EnableSuppress = False   'intestazione Omina Romana
                        'rptBolla.PageHeaderSection3.SectionFormat.EnableSuppress = True    'intestazione Trombin
                        _rptBolla.PageHeaderSection5.SectionFormat.EnableSuppress = True    'intestazione Maiorano


                    Case enum_CodiceGIAS_Clienti.Trombin

                        _rptBolla.Section2.SectionFormat.EnableSuppress = True              'intestazione x tutti
                        'rptBolla.PageHeaderSection2.SectionFormat.EnableSuppress = True    'intestazione Omina Romana
                        _rptBolla.PageHeaderSection3.SectionFormat.EnableSuppress = False   'intestazione Trombin
                        _rptBolla.PageHeaderSection5.SectionFormat.EnableSuppress = True    'intestazione Maiorano

                    Case enum_CodiceGIAS_Clienti.SBTF

                        _rptBolla.Section2.SectionFormat.EnableSuppress = False
                        'rptBolla.PageHeaderSection2.SectionFormat.EnableSuppress = True
                        'rptBolla.PageHeaderSection3.SectionFormat.EnableSuppress = True
                        _rptBolla.PageHeaderSection5.SectionFormat.EnableSuppress = True

                        DrIntestazioneNew.SuperRag_Soc = ""

                        'GESTIONE CARICAMENTO LOGO DINAMICO
                        CaricaLogoInCampoBlobFattura(_logErrori, DrIntestazioneNew, _piva, STAMPE_CONTAB_LOGO_IN_BASSO)


                        '  Giulia, 17/01/2017 10:29:16: CoFruTa ora gestita con tabella
                        'Case enum_CodiceGIAS_Clienti.CoFruTa

                        'If EsistePersDocContab = True AndAlso Lav_Cod = LAVCOD_BOLLA_EMESSA AndAlso CodRisUm_Pers1.Contains(x_Cod_RisUm) Then
                        '    rptBolla.Section2.SectionFormat.EnableSuppress = True
                        '    rptBolla.PageHeaderSection2.SectionFormat.EnableSuppress = False
                        'Else
                        '    rptBolla.Section2.SectionFormat.EnableSuppress = False
                        '    rptBolla.PageHeaderSection2.SectionFormat.EnableSuppress = True
                        'End If

                        'rptBolla.PageHeaderSection3.SectionFormat.EnableSuppress = True
                        'rptBolla.PageHeaderSection5.SectionFormat.EnableSuppress = True

                        'DrIntestazioneNew.SuperRag_Soc = ""

                        'GESTIONE CARICAMENTO LOGO DINAMICO
                        'CaricaLogoInCampoBlob(Log_Errori, DrIntestazioneNew, objParametri_Server, Piva, "LogoInAlto")

                    Case enum_CodiceGIAS_Clienti.MaioranoFormaggio,
                        enum_CodiceGIAS_Clienti.MaioranoRaffaele


                        _rptBolla.Section2.SectionFormat.EnableSuppress = True              'intestazione x tutti
                        'rptBolla.PageHeaderSection2.SectionFormat.EnableSuppress = True    'intestazione Omina Romana
                        'rptBolla.PageHeaderSection3.SectionFormat.EnableSuppress = True    'intestazione Trombin
                        _rptBolla.PageHeaderSection5.SectionFormat.EnableSuppress = False   'intestazione Maiorano


                        'Per il PDF ci vuole il logo che riproduce quello della carta intestata
                        If _qsTipoOutput = "P" Then
                            CaricaLogoInCampoBlobFattura(_logErrori, DrIntestazioneNew, _piva, STAMPE_CONTAB_LOGO_HEADER)
                        Else
                            'Sezione vuota
                        End If


                        'Case enum_CodiceGIAS_Clienti.BeleCasel

                        '    _rptBolla.Section2.SectionFormat.EnableSuppress = True              'intestazione x tutti
                        '    'rptBolla.PageHeaderSection2.SectionFormat.EnableSuppress = True    'intestazione Omina Romana    
                        '    'rptBolla.PageHeaderSection3.SectionFormat.EnableSuppress = True    'intestazione Trombin
                        '    _rptBolla.PageHeaderSection5.SectionFormat.EnableSuppress = False   'intestazione Maiorano/Bele Casel

                    Case Else

                        _rptBolla.Section2.SectionFormat.EnableSuppress = False             'intestazione x tutti
                        'rptBolla.PageHeaderSection2.SectionFormat.EnableSuppress = True    'intestazione Omina Romana
                        'rptBolla.PageHeaderSection3.SectionFormat.EnableSuppress = True    'intestazione Trombin
                        _rptBolla.PageHeaderSection5.SectionFormat.EnableSuppress = True    'intestazione Maiorano

                        ' CType(rptBolla.Section2.ReportObjects("TxtRagSocZeoli"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
                        'DrIntestazioneNew.SuperRag_Soc = Intestazione_RagSoc
                        'imposto la ragione sociale grande in alto 
                        DrIntestazioneNew.SuperRag_Soc = Parametro_Intestazione_Riga1
                        'svuoto il parametro ragione sociale piccola in basso
                        Parametro_Intestazione_Riga1 = ""
                End Select

            End If

            'Gestione PièDiPagina
            Select Case _progressivoGias
                'Case enum_CodiceGIAS_Clienti.BeleCasel

                '    _rptBolla.Section14.SectionFormat.EnableSuppress = True 'Agronica
                '    _rptBolla.PageFooterSection3.SectionFormat.EnableSuppress = False 'BeleCasel
                '    _rptBolla.PageFooterSection4.SectionFormat.EnableSuppress = True 'Maiorano

                Case enum_CodiceGIAS_Clienti.MaioranoFormaggio,
                    enum_CodiceGIAS_Clienti.MaioranoRaffaele

                    _rptBolla.Section14.SectionFormat.EnableSuppress = True 'Agronica
                    _rptBolla.PageFooterSection3.SectionFormat.EnableSuppress = True 'BeleCasel
                    _rptBolla.PageFooterSection4.SectionFormat.EnableSuppress = False 'Maiorano

                    'Per il PDF ci vuole il logo che riproduce quello della carta intestata
                    If _qsTipoOutput = "P" Then
                        CaricaLogoInCampoBlobFattura(_logErrori, DrIntestazioneNew, _piva, STAMPE_CONTAB_LOGO_FOOTER)
                    Else
                        'Sezione vuota
                    End If


                Case Else

                    'In caso le personalizzazioni siano attive, nascondo il logo e ragione sociale Agronica.
                    'If customLoghi IsNot Nothing Then
                    '    _rptBolla.Section14.SectionFormat.EnableSuppress = True
                    'Else
                    '    _rptBolla.Section14.SectionFormat.EnableSuppress = False 'Agronica
                    'End If

                    _rptBolla.PageFooterSection3.SectionFormat.EnableSuppress = True 'BeleCasel
                    _rptBolla.PageFooterSection4.SectionFormat.EnableSuppress = True 'Maiorano
            End Select

        Catch ex As Exception
            _logErrori &= "- Personalizzazioni Layout: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Sub ModificaLayoutStampaLitri(ByVal x_ChkLayOut_Litri As Integer, ByVal litriTotali As Decimal, ByRef parametroLitri As String)

        Dim objStampeUtility As New AgronicaCoreStampeDAL.Utility
        Dim hideObjectNamesField As New List(Of String)
        Dim hideObjectNamesText As New List(Of String)
        Dim hideObjectNamesLine As New List(Of String)
        Dim sectionName As String

        Try

            If x_ChkLayOut_Litri = 1 AndAlso litriTotali <> 0 Then

                parametroLitri = CStr(litriTotali)

            Else
                'Se la Sezione 2 è visibile
                If _rptBolla.ReportDefinition.Sections("Section2").SectionFormat.EnableSuppress = False Then
                    sectionName = "Section2"

                    hideObjectNamesField.Add("Litri_Tot1")
                    hideObjectNamesField.Add("LitriLabel1")
                    'hideObjectNamesText.Add("LitriLabel1")
                    hideObjectNamesLine.Add("LinePeso1")

                    'se quei campi effettivamente esistono li nascondo e faccio le modifiche, sennò potrebbe andare in errore
                    For Each ro In _rptBolla.ReportDefinition.Sections(sectionName).ReportObjects
                        If ro.Name.Equals("Litri_Tot1") OrElse ro.Name.Equals("LitriLabel1") OrElse ro.Name.Equals("LinePeso1") OrElse ro.Name.Equals("TipoPeso1") Then
                            objStampeUtility.HideShowObjCrystal(_rptBolla, sectionName, ReportObjectKind.LineObject, hideObjectNamesLine, False, _objParametriServer)
                            'objStampeUtility.HideShowObjCrystal(_rptBolla, sectionName, ReportObjectKind.TextObject, hideObjectNamesText, False, _objParametriServer)
                            objStampeUtility.HideShowObjCrystal(_rptBolla, sectionName, ReportObjectKind.FieldObject, hideObjectNamesField, False, _objParametriServer)

                            Dim pesoLordoMove = _rptBolla.ReportDefinition.Sections(sectionName).ReportObjects.Item("TipoPeso1")
                            pesoLordoMove.Left = pesoLordoMove.Left + 150
                        End If
                    Next

                    'objStampeUtility.HideShowObjCrystalParameter(_rptBolla, "LitriLabel1", True, _objParametriServer)

                ElseIf _rptBolla.ReportDefinition.Sections("PageHeaderSection2").SectionFormat.EnableSuppress = False Then
                    sectionName = "PageHeaderSection2"

                    hideObjectNamesField.Add("Litri_Tot2")
                    'hideObjectNamesText.Add("LitriLabel2")
                    hideObjectNamesLine.Add("LinePeso2")

                    'se quei campi effettivamente esistono li nascondo e faccio le modifiche, sennò potrebbe andare in errore
                    For Each ro In _rptBolla.ReportDefinition.Sections(sectionName).ReportObjects
                        If ro.Name.Equals("Litri_Tot2") OrElse ro.Name.Equals("LitriLabel2") OrElse ro.Name.Equals("LinePeso2") OrElse ro.Name.Equals("TipoPeso2") Then
                            objStampeUtility.HideShowObjCrystal(_rptBolla, sectionName, ReportObjectKind.LineObject, hideObjectNamesLine, False, _objParametriServer)
                            'objStampeUtility.HideShowObjCrystal(_rptBolla, sectionName, ReportObjectKind.TextObject, hideObjectNamesText, False, _objParametriServer)
                            objStampeUtility.HideShowObjCrystal(_rptBolla, sectionName, ReportObjectKind.FieldObject, hideObjectNamesField, False, _objParametriServer)

                            Dim pesoLordoMove = _rptBolla.ReportDefinition.Sections(sectionName).ReportObjects.Item("TipoPeso2")
                            pesoLordoMove.Left = pesoLordoMove.Left + 150
                        End If
                    Next

                    'objStampeUtility.HideShowObjCrystalParameter(_rptBolla, "LitriLabel2", True, _objParametriServer)

                End If

            End If

        Catch ex As Exception
            _logErrori &= "- Modifiche Layout Stampa Litri: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Sub ImpostaParametri(ByVal Destinatario_Tel As String, ByRef parametro As Parametri)

        Try
            _rptBolla.SetParameterValue("Tipo_Peso", parametro.TipoPeso)
            _rptBolla.SetParameterValue("FatturatoCessionario", parametro.FatturatoCessionario)

            _rptBolla.SetParameterValue("Intestazione_Riga1_RagSocPiccola", parametro.IntestazioneRiga1)
            _rptBolla.SetParameterValue("Intestazione_Riga2", parametro.IntestazioneRiga2)
            _rptBolla.SetParameterValue("Intestazione_Riga3", parametro.IntestazioneRiga3)
            _rptBolla.SetParameterValue("Intestazione_Riga4", parametro.IntestazioneRiga4)
            _rptBolla.SetParameterValue("Intestazione_Riga5", parametro.IntestazioneRiga5)
            _rptBolla.SetParameterValue("Intestazione_Riga6", parametro.IntestazioneRiga6)
            _rptBolla.SetParameterValue("Intestazione_Riga7", parametro.IntestazioneRiga7)
            _rptBolla.SetParameterValue("Intestazione_Riga8", parametro.IntestazioneRiga8)
            _rptBolla.SetParameterValue("Intestazione_Riga9", parametro.IntestazioneRiga9)
            _rptBolla.SetParameterValue("Intestazione_Riga10", parametro.IntestazioneRiga10)
            _rptBolla.SetParameterValue("Intestazione_Riga11", parametro.IntestazioneRiga11)
            _rptBolla.SetParameterValue("Intestazione_Riga12", parametro.IntestazioneRiga12)
            _rptBolla.SetParameterValue("Intestazione_Riga13", parametro.IntestazioneRiga13)
            _rptBolla.SetParameterValue("Intestazione_Riga14", parametro.IntestazioneRiga14)
            _rptBolla.SetParameterValue("Intestazione_Riga15", parametro.IntestazioneRiga15)

            _rptBolla.SetParameterValue("LblPivaCliente", parametro.LblPivaCliente)
            _rptBolla.SetParameterValue("LblCFCliente", parametro.LblCfCliente)
            _rptBolla.SetParameterValue("TxtPivaCliente", parametro.TxtPivaCliente)
            _rptBolla.SetParameterValue("TxtCFCliente", parametro.TxtCfCliente)

            _rptBolla.SetParameterValue("Titolo_Documento", parametro.TitoloDocumento)
            _rptBolla.SetParameterValue("Titolo_Stabilimento", parametro.TitoloStabilimento)

            _rptBolla.SetParameterValue("LblPrezzoUdmExtra", parametro.LblPrezzoUdmExtra)

            _rptBolla.SetParameterValue("Contributo_CONAI", parametro.Conai)

            _rptBolla.SetParameterValue("Destinazione_Tel", Destinatario_Tel)


            For Each param In _rptBolla.DataDefinition.ParameterFields
                'i parametri potrebbero non esistere in alcuni report
                Select Case param.Name
                    Case "Litri_Tot" 'check if parameter exists in report
                        _rptBolla.SetParameterValue("Litri_Tot", parametro.Litri) ' set the parameter value in the report
                    Case "Agente"
                        If parametro.AgenteInfo <> "" Then
                            parametro.AgenteInfo = _traduttore.ValoreDizionarioTraduzioneComuni("Agente: ") & parametro.AgenteInfo
                        End If
                        _rptBolla.SetParameterValue("Agente", parametro.AgenteInfo) ' set the parameter value in the report
                    Case "Rag_Soc_Aggiuntivo"
                        _rptBolla.SetParameterValue("Rag_Soc_Aggiuntivo", parametro.RagSocAggiuntivo)
                    Case "Indirizzo_Aggiuntivo"
                        _rptBolla.SetParameterValue("Indirizzo_Aggiuntivo", parametro.IndirizzoAggiuntivo)
                    Case "CAP_Aggiuntivo"
                        _rptBolla.SetParameterValue("CAP_Aggiuntivo", parametro.CapAggiuntivo)
                    Case "Frazione_Aggiuntivo"
                        _rptBolla.SetParameterValue("Frazione_Aggiuntivo", parametro.FrazioneAggiuntivo)
                    Case "Comune_Aggiuntivo"
                        _rptBolla.SetParameterValue("Comune_Aggiuntivo", parametro.ComuneAggiuntivo)
                    Case "Prov_Aggiuntivo"
                        _rptBolla.SetParameterValue("Prov_Aggiuntivo", parametro.ProvAggiuntivo)
                    Case "TxtPivaAgg"
                        _rptBolla.SetParameterValue("TxtPivaAgg", parametro.TxtPivaAgg)
                    Case "TxtCFAgg"
                        _rptBolla.SetParameterValue("TxtCFAgg", parametro.TxtCfAgg)

                    Case "Vettore_Riga1"
                        _rptBolla.SetParameterValue("Vettore_Riga1", parametro.VettoreRiga1)
                    Case "Vettore_Riga2"
                        _rptBolla.SetParameterValue("Vettore_Riga2", parametro.VettoreRiga2)
                    Case "Vettore_Riga3"
                        _rptBolla.SetParameterValue("Vettore_Riga3", parametro.VettoreRiga3)
                    Case "Vettore_Riga4"
                        _rptBolla.SetParameterValue("Vettore_Riga4", parametro.VettoreRiga4)

                    Case "Lbl_PrezzoRiscontrato"
                        _rptBolla.SetParameterValue("Lbl_PrezzoRiscontrato", parametro.LblPrezzoRiscontrato)

                    Case "QualificaSecondoContatto"
                        _rptBolla.SetParameterValue("QualificaSecondoContatto", parametro.QualificaSecondoContatto)
                    Case "QualificaQuartoContatto"
                        _rptBolla.SetParameterValue("QualificaQuartoContatto", parametro.QualificaQuartoContatto)
                    Case "CodiceSDI"
                        _rptBolla.SetParameterValue("CodiceSDI", parametro.CodiceSDI)
                    Case "Articolo62"
                        _rptBolla.SetParameterValue("Articolo62", parametro.Articolo62)
                    Case "LP_Articolo62.Section4"
                        _rptBolla.SetParameterValue("LP_Articolo62.Section4", parametro.Articolo62)
                End Select
            Next

        Catch ex As Exception
            _logErrori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Sub ModificaLayoutCessionarioAggiuntivo(ByVal x_Cod_RisUm_Aggiuntivo As Integer)

        Try
            'se c'è il cessionario aggiuntivo ==> mostro i suoi dati
            If x_Cod_RisUm_Aggiuntivo <> 0 Then
                _rptBolla.ReportDefinition.Sections("Section2").ReportObjects.Item("QualificaQuartoContatto").ObjectFormat.EnableSuppress = False
                _rptBolla.ReportDefinition.Sections("Section2").ReportObjects.Item("RagSocAggiuntivoSTD").ObjectFormat.EnableSuppress = False
                _rptBolla.ReportDefinition.Sections("Section2").ReportObjects.Item("IndirizzoAggiuntivoSTD").ObjectFormat.EnableSuppress = False
                _rptBolla.ReportDefinition.Sections("Section2").ReportObjects.Item("CAPAggiuntivoSTD").ObjectFormat.EnableSuppress = False
                _rptBolla.ReportDefinition.Sections("Section2").ReportObjects.Item("FrazioneAggiuntivoSTD").ObjectFormat.EnableSuppress = False
                _rptBolla.ReportDefinition.Sections("Section2").ReportObjects.Item("ComuneAggiuntivoSTD").ObjectFormat.EnableSuppress = False
                _rptBolla.ReportDefinition.Sections("Section2").ReportObjects.Item("ProvAggiuntivoSTD").ObjectFormat.EnableSuppress = False
                _rptBolla.ReportDefinition.Sections("Section2").ReportObjects.Item("LblPivaClienteAggSTD").ObjectFormat.EnableSuppress = False
                _rptBolla.ReportDefinition.Sections("Section2").ReportObjects.Item("TxtPivaAggSTD").ObjectFormat.EnableSuppress = False
                _rptBolla.ReportDefinition.Sections("Section2").ReportObjects.Item("LblCFClienteAggSTD").ObjectFormat.EnableSuppress = False
                _rptBolla.ReportDefinition.Sections("Section2").ReportObjects.Item("TxtCFAggSTD").ObjectFormat.EnableSuppress = False
            End If

        Catch ex As Exception
            _logErrori &= "- Modifiche Layout Cessionario Aggiuntivo: " & vbCrLf & ex.Message & vbCrLf
        End Try
    End Sub

    Private Sub GestioneCampiTotaleDDT(ByVal mostraCampi As Boolean)

        If _rptBolla.Section17 IsNot Nothing Then

            For Each ro In _rptBolla.Section17.ReportObjects
                If ro.Name.Equals("TotaleDDTLabel") OrElse ro.Name.Equals("TotaleDDTVal") OrElse ro.Name.Equals("TotaleDDTBox") Then
                    If mostraCampi = True Then
                        ro.ObjectFormat.EnableSuppress = False
                    Else
                        ro.ObjectFormat.EnableSuppress = True
                    End If
                End If
            Next
        End If

    End Sub

    Private Shared Function GetTipoLayout(ByVal chkLayOutPeso As Integer,
                                          ByVal chkLayOutPrezzo As Integer,
                                          ByVal chkLayOutRiscontrato As Integer
                                          ) As enum_TipoLayoutDDT

        If chkLayOutPeso = 1 Then
            Return enum_TipoLayoutDDT.Pesi_Reali_piu_Prezzo
        ElseIf chkLayOutPrezzo = 1 Then
            Return enum_TipoLayoutDDT.Dettagli_Economici
        ElseIf chkLayOutRiscontrato = 1 Then
            Return enum_TipoLayoutDDT.Pesi_Reali_piu_Riscontrati
        ElseIf chkLayOutRiscontrato = 2 Then
            Return enum_TipoLayoutDDT.Tutto_Riscontrato_se_disponibile_piu_Prezzo
        Else
            Return enum_TipoLayoutDDT.Standard
        End If

    End Function

    Private Function OttieniTipoReportPerTraduttore() As Enum_Tipo_Report_DDT

        Dim reportName As String = _rptBolla.GetType().Name.ToLower()
        Dim tipoReport As Enum_Tipo_Report_DDT = Enum_Tipo_Report_DDT.CRBolla

        Select Case reportName
            Case "crbolla"
                tipoReport = Enum_Tipo_Report_DDT.CRBolla
            Case "crbolla2016"
                tipoReport = Enum_Tipo_Report_DDT.CRBolla2016
            Case "crbolla2016_lb"
                tipoReport = Enum_Tipo_Report_DDT.CRBolla2016_LB
            Case Else
                tipoReport = Enum_Tipo_Report_DDT.Undefined
        End Select

        Return tipoReport

    End Function



End Class


