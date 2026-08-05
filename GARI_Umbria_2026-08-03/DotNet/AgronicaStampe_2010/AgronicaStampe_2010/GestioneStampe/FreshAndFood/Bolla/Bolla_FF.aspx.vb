Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Bolla_FF
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private _rptBolla As Rpt_BollaFF

    Private _piva As String
    Private _lavCod As Integer
    Private _idAgenda As Integer
    Private _dataAccettazione As Date = #1/1/1900#

    Private _objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        _idAgenda = CInt(Stringa_Decodifica(CStr(Request.QueryString("i")), AgroKey_EncoderDecoder, Server))

        _lavCod = CInt(Stringa_Decodifica(CStr(Request.QueryString("l")), AgroKey_EncoderDecoder, Server))


        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        _objParametriUtenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim nomeDocumento As String = "Bolla_FF"
        Dim logErrori As String = ""

        _rptBolla = New Rpt_BollaFF

        If Not Me.IsPostBack Then

            Dim catCod As Integer = enum_CategorieDocumenti.Conferimento

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------

            Try

                Stampa_Bolla_FF(logErrori)

            Catch exc As Exception
                logErrori &= "- Stampa_Bolla_FF: " & vbCrLf & exc.Message & vbCrLf
            End Try

            Dim identificazioneDocumento As String = ""

            Try

                identificazioneDocumento = nomeDocumento & "_p" & _piva

                ' leggo la sotto cartella da CategorieDocumenti
                Dim sottoCartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                sottoCartella = objCatDoc.Sottocartella(catCod, "", "", _objParametriServer)
                objCatDoc = Nothing

                Dim nomeFile As String
                If Not IsNothing(_dataAccettazione) Then
                    nomeFile = identificazioneDocumento & "_n" & Session("Doc_Numero") & "_d" & Format(CDate(Session("Data_Movimento")), "dd-MM-yyyy") & ".pdf"
                Else
                    nomeFile = identificazioneDocumento & "_n" & Session("Doc_Numero") & ".pdf"
                End If

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(_rptBolla,
                                           catCod,
                                           sottoCartella,
                                           nomeFile,
                                           _objParametriServer, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim allegatiDocumentiCod As Integer

                allegatiDocumentiCod = objAllegati.SalvaAllegato(_piva,
                                                                 catCod,
                                                                 "Bolla_FreshFood",
                                                                 nomeFile,
                                                                 sottoCartella,
                                                                 _idAgenda, "", "", "",
                                                                 _dataAccettazione,
                                                                 AGRODATAFINE,
                                                                 _objParametriServer)

            Catch ex As Exception
                logErrori &= "- Gestione allegati: " & vbCrLf & ex.Message & vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            SalvaLogErrori_Agenda(logErrori, nomeDocumento, identificazioneDocumento, "Bolla_FF.aspx", "Stampe_Conferimento", _idAgenda, _objParametriServer)

            ' per visualizzare l'anteprima, ma non c'è nè il bottone di stampa, nè quello di export
            Session("Report") = _rptBolla
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))

        End If

    End Sub


    '#####################################################################################################
    Private Sub Stampa_Bolla_FF(ByRef logErrori As String)

        Dim ds As New DataSetFattura
        Dim i As Integer
        Dim msgErrore As String = ""
        Dim dsRiepilogoImballiEntrata As New DS_Iva 'riciclo il DataSet dell'iva, per gli imballi in entrata, se no fa casino con quelli in uscita
        'Dim dsImballiUscita As New DS_ImballiUscita
        Dim objMPCamp As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
        
        Dim x_Tara_Veicolo As Decimal = 0
        'Dim x_Tara_Imballi As Decimal = 0
        Dim dettagliProdotto As String
        Dim pesoTotale As Decimal = 0
        Dim pesoLordo As Decimal = 0
        Dim pesoNetto As Decimal = 0
        'Dim Tot_Degrado As Decimal = 0
        'Dim Tot_NettoPagamento As Decimal = 0
        
        ''------------------
        ''Dati DDT Imballi
        'Dim x_Doc_Numero_Sin_DDtImballi As String
        'Dim x_Doc_Numero_DDTImballi As Decimal
        'Dim x_Doc_Numero_Des_DDTImballi As String
        'Dim Numero_DDTImballi As String

        '------------------------------
        'Movimenti Dettagli

        Dim descProdotto As String = ""
        Dim confezionamento As String = ""
        Dim dettagliLotto As String = ""
        Dim objLotto As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R

        Dim contatore As Integer = 0
        Dim contatoreRaggruppamento As Integer = 0
        Dim prgRiga As String = "" 'E' salvato in Movimenti_dettagli.extra_str 
        Dim matDes As String
        'Dim Calibro As String
        'Dim Qualita As String
        'Dim Declassamento As String
        'Dim Marca As String
        Dim confezione As String = ""
        Dim contenitore As String = ""
        Dim imballaggio As String = ""
        Dim udmDes As String = ""
        Dim note As String

        Dim x_Qta As Decimal
        'Dim Qta_Extra As Decimal = 0
        Dim x_Qta_Extra_Totale As Decimal
        Dim x_Udm_Cod_Extra As Integer
        Dim x_Tara As Decimal
        Dim x_Qta_Sottoconfezioni As Decimal
        Dim x_Qta_Imballaggi As Decimal = 0
        Dim x_Unita_Misura As String
        Dim x_Lotto As String
        Dim x_Udm_Cod As Integer
        Dim x_Elem_Cod As Integer
        Dim x_Mat_Cod As Integer
        Dim x_Cal_cod As Integer
        Dim listaProgressivo As String = ""

        Dim memoRigaDettaglio As String = ""
        Dim rigaDettaglio As String
        Dim memoPrgRiga As String = ""
        Dim memoDescProdotto As String = ""
        Dim memoUdmDes As String = ""
        Dim memoImballaggio As String = ""
        Dim memoContenitore As String = ""
        Dim memoConfezione As String = ""

        Dim numImballaggi As Integer = 0
        Dim numContenitori As Integer = 0
        Dim numConfezione As Integer = 0
        Dim raggrNumImballaggi As Integer = 0
        Dim raggrNumContenitori As Integer = 0
        Dim raggrNumConfezione As Integer = 0

        Dim qta As Decimal = 0
        Dim raggrQta As Decimal = 0

        Dim raggrPesoNetto As Decimal = 0
        Dim raggrPesoLordo As Decimal = 0
        Dim raggrTaraImballi As Decimal = 0

        Dim totalePesoNetto As Decimal = 0
        Dim totalePesoLordo As Decimal = 0
        Dim totaleTaraImballi As Decimal = 0

        '---------------------------
        Dim flagRaggruppamento As Boolean = False
        Dim Parametro_OperatorePesa As String = ""
        Dim Parametro_Vettore_Riga1 As String = ""
        Dim Parametro_Vettore_Riga2 As String = ""
        Dim Parametro_Vettore_Riga3 As String = ""
        Dim Parametro_Vettore_Riga4 As String = ""
        Dim Parametro_AgenteInfo As String = ""

        '############################################################################################
        '######################## INTESTAZIONE DELLA BOLLA ###########################
        '############################################################################################

        Try

            'Impresa
            Dim indirizzoImpresa As String
            Dim x_RagSoc_Impresa As String = ""
            Dim x_CodContatto_Impresa As String = ""
            Dim x_CodiceFiscale_Impresa As String = ""
            Dim x_IndDes_Impresa As String = ""
            Dim x_FrzDes_Impresa As String = ""
            Dim x_Cap_Impresa As String = ""
            Dim x_Comune_Impresa As String = ""
            Dim x_Provincia_Impresa As String = ""
            Dim x_RegImprese As String = ""
            Dim x_Provincia_RegImprese As String = ""
            Dim x_Stato_Impresa As String = ""
            Dim x_REA As String = ""
            Dim x_ISO As String = ""
            Dim x_AlboCoop As String = ""
            Dim x_CapitaleSociale As String = ""
            Dim x_Telefono As String = ""
            Dim x_Fax As String = ""
            Dim x_Cell As String = ""
            Dim x_Email As String = ""
            Dim x_SitoWeb As String = ""
            Dim x_Fabbricato_Des() As String
            Dim x_IndDes_Fabbricato() As String
            Dim x_FrzDes_Fabbricato() As String
            Dim x_Cap_Fabbricato() As String
            Dim x_Comune_Fabbricato() As String
            Dim x_Provincia_Fabbricato() As String

            Leggi_Intestazione_Impresa(_objParametriServer,
                                       CInt(Session("ASG_ProgressivoGIAS")),
                                       False,
                                       False,
                                       _piva,
                                       logErrori,
                                       x_RagSoc_Impresa,
                                       x_CodContatto_Impresa,
                                       x_CodiceFiscale_Impresa,
                                       x_IndDes_Impresa,
                                       x_FrzDes_Impresa,
                                       x_Cap_Impresa,
                                       x_Comune_Impresa,
                                       x_Provincia_Impresa,
                                       x_RegImprese,
                                       x_Provincia_RegImprese,
                                       x_REA,
                                       x_ISO,
                                       x_AlboCoop,
                                       x_CapitaleSociale,
                                       x_Telefono,
                                       x_Fax,
                                       x_Cell,
                                       x_Email,
                                       x_SitoWeb,
                                       x_Fabbricato_Des,
                                       x_IndDes_Fabbricato,
                                       x_FrzDes_Fabbricato,
                                       x_Cap_Fabbricato,
                                       x_Comune_Fabbricato,
                                       x_Provincia_Fabbricato,
                                       Nothing,
                                       Nothing,
                                       Nothing,
                                       Nothing,
                                       Nothing,
                                       Nothing,
                                       Nothing,
                                       Nothing,
                                       Nothing,
                                       Nothing,
                                       Nothing,
                                       Nothing,
                                       Nothing,
                                       x_Stato_Impresa)

            indirizzoImpresa = x_IndDes_Impresa & " " & x_FrzDes_Impresa & " " & x_Cap_Impresa & " " & x_Comune_Impresa & " " & "(" & x_Provincia_Impresa & ")"

            CType(_rptBolla.Section2.ReportObjects("TxtPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Partita IVA: " & _piva
            CType(_rptBolla.Section2.ReportObjects("TxtRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = x_RagSoc_Impresa
            CType(_rptBolla.Section2.ReportObjects("TxtIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = indirizzoImpresa
            CType(_rptBolla.Section2.ReportObjects("TxtCF"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Codice Fiscale: " & x_CodiceFiscale_Impresa

            If x_RegImprese <> "" Then
                CType(_rptBolla.Section2.ReportObjects("TxtRegImprese"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Iscr. al n. " & x_RegImprese & " del Reg. Imprese Prov. di " & x_Provincia_RegImprese
            End If
            If x_REA <> "" Then
                CType(_rptBolla.Section2.ReportObjects("TxtREA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Codice REA: " & x_REA
            End If
            If x_ISO <> "" Then
                CType(_rptBolla.Section2.ReportObjects("TxtISO"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Codice ISO: " & x_ISO
            End If
            If x_Telefono <> "" Then
                CType(_rptBolla.Section2.ReportObjects("TxtTelefono"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Telefono: " & x_Telefono
            End If
            If x_Fax <> "" Then
                CType(_rptBolla.Section2.ReportObjects("TxtFax"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Fax: " & x_Fax
            End If
            If x_Email <> "" Then
                CType(_rptBolla.Section2.ReportObjects("TxtEmail"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Email: " & x_Email
            End If
            If x_SitoWeb <> "" Then
                CType(_rptBolla.Section2.ReportObjects("TxtSito"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Sito web: " & x_SitoWeb
            End If

        Catch ex As Exception
            logErrori &= "- Lettura dei dati dell'intestazione dell'impresa: " & vbCrLf & ex.Message & vbCrLf
        End Try


        ''############################################################################################
        ''############################ Lettura della Bolla ###########################################
        ''############################################################################################

        Dim dt As DataTable = Nothing
        Dim aggregaRighe As Boolean = True

        Try
            Dim nomeDbUtenti As String = _objParametriUtenti.Recupera_NomeDB
            If CInt(Session("ASG_ProgressivoGIAS")) = enum_CodiceGIAS_Clienti.CoFruTa Then
                aggregaRighe = False
            End If

            Dim objFF As New AgronicaCoreStampeDAL.FreshAndFood
            dt = objFF.StampaBolla(_piva,
                                   _lavCod,
                                   _idAgenda,
                                   "",
                                   nomeDbUtenti,
                                   aggregaRighe,
                                   _objParametriServer)

        Catch ex As Exception
            logErrori &= "- Lettura dei dati: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try


        Try

            If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then

                'Leggo i moduli installati
                Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                Dim moduliCliente = objO.Recupera_Moduli_Cliente(_piva, _objParametriServer)

                For i = 0 To dt.Rows.Count - 1

                    'solo primo giro
                    If i = 0 Then
                        ValorizzazioneDatiTestata(dt(i), ds, logErrori,
                                                  x_Tara_Veicolo, 
                                                  Parametro_Vettore_Riga1, Parametro_Vettore_Riga2, 
                                                  Parametro_Vettore_Riga3, Parametro_Vettore_Riga4,
                                                  Parametro_AgenteInfo, Parametro_OperatorePesa)
                        
                    End If 'primo giro (x intestazione documento)

                    '===================================================================
                    'per ogni dettaglio di bolla
                    rigaDettaglio = ""
                    confezionamento = ""
                    imballaggio = ""
                    contenitore = ""
                    confezione = ""

                    With dt.Rows(i)

                        contatore = i + 1
                        prgRiga = .Item("PrgRiga") 'E' salvato in Movimenti_dettagli.extra_str 

                        matDes = .Item("Mat_Des_Raccolta")

                        note = .Item("ONote")

                        x_Qta = .Item("Qta")
                        'x_Qta_Extra = .Item("Qta_Extra")
                        x_Qta_Sottoconfezioni = .Item("Qta_Sottoconfezioni")
                        x_Qta_Imballaggi = .Item("Qta_Imballaggi")

                        x_Qta_Extra_Totale = .Item("Qta_Extra_totale")
                        x_Udm_Cod_Extra = .Item("Udm_Cod_Extra")
                        x_Tara = .Item("Tara")

                        x_Udm_Cod = .Item("Udm_Cod_Raccolta")
                        x_Unita_Misura = .Item("Udm_Sim_Raccolta")
                        x_Lotto = .Item("Lotto_raccolta")
                        x_Elem_Cod = .Item("Elem_Cod_Raccolta")
                        x_Mat_Cod = .Item("Mat_Cod_Raccolta")
                        x_Cal_cod = .Item("Cal_Cod_Raccolta")
                        listaProgressivo &= CStr(x_Cal_cod) & ","

                        '----------------------------------
                        'Recupera_DettagliEConfezionamento_Prodotto
                        'TODO: da rivedere quando ci saranno i filtri su specie animale per lo zoo
                        dettagliProdotto = objMPCamp.Recupera_DettagliEConfezionamento_Prodotto(_piva, .Item("veg_cod"), x_Cal_cod,
                                                                                                "",
                                                                                                imballaggio,
                                                                                                contenitore,
                                                                                                confezione,
                                                                                                _objParametriServer)

                        '----------------------------------
                        'GESTIONE LOTTO PRODOTTI
                        dettagliLotto = objLotto.Gestione_LottoProdotto(_piva, x_Elem_Cod, x_Mat_Cod, x_Lotto, moduliCliente, _objParametriServer)

                    End With
                    '----------------------------------

                    descProdotto = matDes

                    If dettagliProdotto <> "" Then
                        descProdotto &= " " & dettagliProdotto
                    End If

                    If dettagliLotto <> "" Then
                        descProdotto &= dettagliLotto
                    End If

                    If note <> "" Then
                        descProdotto &= "<br>" & note
                    End If

                    qta = x_Qta
                    udmDes = x_Unita_Misura

                    numImballaggi = x_Qta_Imballaggi
                    numContenitori = x_Qta_Sottoconfezioni
                    numConfezione = x_Qta

                    pesoNetto = x_Qta_Extra_Totale
                    pesoLordo = pesoNetto + x_Tara

                    'i totali del documento li calcolo per ogni riga
                    totalePesoNetto += pesoNetto
                    totaleTaraImballi += x_Tara
                    totalePesoLordo += pesoLordo

                    'aggregazione per prodotto: 
                    'occorre stampare una sola riga a parità di 
                    'Prodotto, DettagliProdotto(Calibro, Qualità, Certificazioni,ecc), Lotto, Note, Imballo, Contenitore, Confezione
                    'questo perché quando hanno grossi carichi fanno diverse pesate ma non vogliono dare al socio una riga per ogni pesata
                    rigaDettaglio = descProdotto & udmDes & imballaggio & contenitore & confezione

                    'Stefano 6/9/2017 - Cofruta non vuole aggregazione
                    If memoRigaDettaglio <> rigaDettaglio OrElse Not aggregaRighe Then

                        'primo caso o nuovo prodotto

                        If memoRigaDettaglio = "" Then
                            'primo prodotto in assoluto della bolla
                            contatoreRaggruppamento = 1
                        Else
                            'nuovo prodotto ==> devo aggiungere al DataSet il precedente prodotto (raggruppato o meno)

                            ' la sfiga che può succedere su flag_Raggruppamento è che il primo dettaglio
                            'ha solo una riga, quindi nel contatore metterà prgRiga
                            'non ho modo di saperlo prima se raggrupperò o meno

                            InserisciRiga_DataSet(ds,
                                                  flagRaggruppamento,
                                                  contatore - 1,
                                                  contatoreRaggruppamento,
                                                  memoPrgRiga,
                                                  memoDescProdotto,
                                                  memoImballaggio,
                                                  memoContenitore,
                                                  memoConfezione,
                                                  raggrNumImballaggi,
                                                  raggrNumContenitori,
                                                  raggrNumConfezione,
                                                  raggrPesoNetto,
                                                  raggrTaraImballi,
                                                  raggrPesoLordo, 
                                                  raggrQta, memoUdmDes)

                            'incrementare contatore prodotti
                            contatoreRaggruppamento += 1

                            'ri-azzero i totali per prodotto (essendo cambiato)
                            raggrQta = 0
                            raggrNumImballaggi = 0
                            raggrNumContenitori = 0
                            raggrNumConfezione = 0
                            raggrPesoNetto = 0
                            raggrPesoLordo = 0
                            raggrTaraImballi = 0

                        End If

                        'inizio a sommare totali per prodotto
                        raggrQta += qta
                        raggrNumImballaggi += numImballaggi
                        raggrNumContenitori += numContenitori
                        raggrNumConfezione += numConfezione
                        raggrPesoNetto += pesoNetto
                        raggrTaraImballi += x_Tara
                        raggrPesoLordo += pesoLordo

                        memoRigaDettaglio = rigaDettaglio 'memorizzo il nuovo prodotto
                        memoDescProdotto = descProdotto
                        memoUdmDes = udmDes
                        memoImballaggio = imballaggio
                        memoContenitore = contenitore
                        memoConfezione = confezione
                        memoPrgRiga = prgRiga

                    Else

                        flagRaggruppamento = True
                        'stesso prodotto
                        '   -> sommare qta imballi/contenitori/confezioni e pesi

                        raggrQta += qta
                        raggrNumImballaggi += numImballaggi
                        raggrNumContenitori += numContenitori
                        raggrNumConfezione += numConfezione

                        raggrPesoNetto += pesoNetto
                        raggrTaraImballi += x_Tara
                        raggrPesoLordo += pesoLordo

                    End If

                    '===================================================================

                Next

                'occorre aggiungere l'ultimo prodotto perché non viene aggiunto dal ciclo
                InserisciRiga_DataSet(ds,
                                      flagRaggruppamento,
                                      contatore,
                                      contatoreRaggruppamento,
                                      prgRiga,
                                      descProdotto,
                                      imballaggio,
                                      contenitore,
                                      confezione,
                                      raggrNumImballaggi,
                                      raggrNumContenitori,
                                      raggrNumConfezione,
                                      raggrPesoNetto,
                                      raggrTaraImballi,
                                      raggrPesoLordo,
                                      raggrQta, udmDes)

            End If

        Catch ex As Exception
            logErrori &= "- Elaborazione dei dati: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try

        ''############################################################################################
        ''############################ Print Dati Bolla ##############################################
        ''############################################################################################

        'Dim DrIntestazioneNew As DataSetFattura.IntestazioneFatturaRow = DS.IntestazioneFattura.NewIntestazioneFatturaRow

        'Try

        '    '=============================================================
        '    '----------------------- DATI BOLLA --------------------------

        '    'per la visualizzazione del logo viene letto questo campo
        '    DrIntestazioneNew.SuperPiva = Piva

        '    'riciclo il campo aspetto, poiché il campo data_Movimento viene visto come data e non come stringa
        '    'e le date vengono visualizzate in formato americano da Fruttagel
        '    DrIntestazioneNew.Aspetto = Format(_dataAccettazione, "dd/MM/yyyy")
        '    DrIntestazioneNew.Doc_Numero = Numero_Bolla
        '    DrIntestazioneNew.NSBanca = CType(x_Data_DDTConf, DateTime)
        '    DrIntestazioneNew.VSBanca = Numero_DDTConf
        '    DrIntestazioneNew.Note = x_Note
        '    DrIntestazioneNew.Colli = x_Colli




        '    '/////////////////////////////////////////////////////////////

        '    Select Case Lav_Cod


        '        Case LAVCOD_ACCETTAZIONE_DIVERSI

        '            '-------------------------------------------------------------
        '            '--------------- ACCETTAZIONE DA DIVERSI ---------------------
        '            '-------------------------------------------------------------
        
        '            '=============================================================
        '            '--------------------- PRODUTTORE --------------------------

        '            If x_Cod_Destinazione <> 0 Then

        '                DrIntestazioneNew.DestinazioneRag_Soc = x_RagSoc_Destinazione
        '                DrIntestazioneNew.DestinazionePiva = "Partita IVA: " & Piva_Produttore
        '                If CodiceFiscale_Produttore <> "" Then
        '                    'riciclo il campo causale_trasporto
        '                    'DrIntestazioneNew.DestinazionePiva &= " - Codice Fiscale: " & CodiceFiscale_Produttore
        '                    DrIntestazioneNew.Causale_Trasporto = "Codice Fiscale: " & CodiceFiscale_Produttore
        '                End If

        '                If x_Cod_IndirizzoDestinazione <> 0 Then

        '                    DrIntestazioneNew.DestinazioneInd_Des = x_IndDes_Destinazione
        '                    DrIntestazioneNew.DestinazioneFrz_Des = x_FrzDes_Destinazione
        '                    DrIntestazioneNew.DestinazioneCap = x_Cap_Destinazione
        '                    DrIntestazioneNew.DestinazioneComune = x_Comune_Destinazione
        '                    If x_Provincia_Destinazione <> "" Then
        '                        DrIntestazioneNew.DestinazioneProvincia = "(" & x_Provincia_Destinazione & ")"
        '                    End If

        '                End If

        '            End If

        '    End Select

        '    '======================================================================

        'Catch ex As Exception
        '    logErrori &= "- Visualizzazione dei dettagli della Bolla di Accettazione: " & vbCrLf & ex.Message & vbCrLf
        'End Try


        ''=============================================================
        ''RIEPILOGO IMBALLAGGI
        ''=============================================================
        ''Riempi DataSet Imballaggi

        Try

            Carica_DSImballaggi_StampaBolla(_piva, _idAgenda,
                                            logErrori,
                                            dsRiepilogoImballiEntrata)

        Catch ex As Exception
            logErrori &= "- Caricamento DataSet Imballaggi: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try

            'imposto il DataSet sul report
            _rptBolla.SetDataSource(ds)

        Catch ex As Exception
            logErrori &= "- Aggancio del DataSet: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try
            _rptBolla.OpenSubreport("Rpt_RiepilogoImballi.rpt").SetDataSource(dsRiepilogoImballiEntrata)
        Catch

        End Try

        Try

            _rptBolla.SetParameterValue("Layout1", "1")

            _rptBolla.SetParameterValue("Par_PesoTotale", Format(totalePesoLordo + x_Tara_Veicolo, "#,###,##0.00"))
            _rptBolla.SetParameterValue("Par_TaraVeicolo", Format(x_Tara_Veicolo, "#,###,##0.00"))
            _rptBolla.SetParameterValue("Par_Totale_PesoLordo", Format(totalePesoLordo, "#,###,##0.00"))
            _rptBolla.SetParameterValue("Par_Totale_TaraImballi", Format(totaleTaraImballi, "#,###,##0.00"))
            _rptBolla.SetParameterValue("Par_Totale_PesoNetto", Format(totalePesoNetto, "#,###,##0.00"))
            _rptBolla.SetParameterValue("Operatore_Pesa", Parametro_OperatorePesa)

            _rptBolla.SetParameterValue("Vettore_Riga1", Parametro_Vettore_Riga1)
            _rptBolla.SetParameterValue("Vettore_Riga2", Parametro_Vettore_Riga2)
            _rptBolla.SetParameterValue("Vettore_Riga3", Parametro_Vettore_Riga3)
            _rptBolla.SetParameterValue("Vettore_Riga4", Parametro_Vettore_Riga4)
            _rptBolla.SetParameterValue("Agente_Info", Parametro_AgenteInfo)

            _rptBolla.SetParameterValue("Dicitura_Cessione", "(CESSIONE CON PREZZO DA DETERMINARE: D.M. 15.11.1975)")

        Catch ex As Exception
            logErrori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Sub ValorizzazioneDatiTestata(ByRef dr As DataRow,
                                          ByRef ds As DataSetFattura,
                                          ByRef logErrori As String,
                                          ByRef x_Tara_Veicolo As Decimal,
                                          ByRef Parametro_Vettore_Riga1 As String, ByRef Parametro_Vettore_Riga2 As String,
                                          ByRef Parametro_Vettore_Riga3 As String,ByRef Parametro_Vettore_Riga4 As String,
                                          ByRef Parametro_AgenteInfo As String, ByRef Parametro_OperatorePesa As String)
        
        Dim drIntestazioneNew As DataSetFattura.IntestazioneFatturaRow

        Dim objContattiCodici As New AgronicaCoreAnagrafeDAL.Contatti_Codici_R
        Dim dtContattiCodici As DataTable

        '------------------
        'Dati Bolla
        Dim x_Ora_Accettazione As DateTime
        Dim x_Doc_Numero_Sin_Accettazione As String
        Dim x_Doc_Numero_Accettazione As Integer
        Dim x_Doc_Numero_Des_Accettazione As String
        Dim numeroBolla As String
        'Dim x_Peso As Decimal = 0
        'Dim x_Tipo_Peso As Integer = 0
        'Dim x_ChkLayout_Join_Prodotti As Integer = 0
        Dim x_Note As String = ""
        Dim x_DataOra_Ingresso As DateTime

        '------------------
        'Dati DDT Conf
        Dim x_Data_DDTConf As Date
        'Dim x_Ora_DDTConf As String 
        'Dim x_Colli As Integer
        Dim x_Doc_Numero_Sin_DDTConf As String
        Dim x_Doc_Numero_DDTConf As Integer
        Dim x_Doc_Numero_Des_DDTConf As String
        Dim Numero_DDTConf As String
        Dim Data_DDTConf As String

        '------------------
        'Conferente
        Dim Piva_Conferente As String = ""
        Dim CodiceFiscale_Conferente As String = ""
        Dim Codice_Conferente As String = ""
        Dim x_Cod_RisUm As Integer
        Dim x_Cod_Contatto As String
        Dim x_Rag_Soc As String = ""
        Dim x_Codice_Fiscale As String
        Dim x_Cod_IndirizzoRisUm As Integer
        Dim x_Ind_Des As String = ""
        Dim x_Frz_Des As String = ""
        Dim x_Cap As String = ""
        Dim x_Comune As String = ""
        Dim x_Provincia As String = ""
        Dim x_Stato As String

        '------------------
        'Vettore
        Dim pivaVettore As String = ""
        Dim codiceFiscaleVettore As String = ""
        Dim x_Mezzo As Integer
        Dim mezzoDes As String = ""
        Dim x_Cod_Vettore As Integer
        Dim x_Cod_IndirizzoVettore As Integer
        Dim x_CodContatto_Vettore As String = ""
        Dim x_RagSoc_Vettore As String = ""
        Dim x_CodiceFiscale_Vettore As String = ""
        Dim x_IndDes_Vettore As String = ""
        Dim x_FrzDes_Vettore As String = ""
        Dim x_Cap_Vettore As String = ""
        Dim x_Comune_Vettore As String = ""
        Dim x_Provincia_Vettore As String = ""
        Dim x_Stato_Vettore As String = ""
        Dim x_NumReg_Vettore As String = ""
        Dim x_TargaMezzo_Vettore As String = ""

        Dim x_Agente_Cod As Integer

        Dim causaleTrasporto As String
        Dim aspettoBeni As String
        Dim naturaBeni As String

        Parametro_OperatorePesa = "Operatore Pesa: " & dr.Item("Nome") & " " & dr.Item("Cognome")
        _dataAccettazione = dr.Item("Data_Accett")
        x_Ora_Accettazione = dr.Item("Ora_Accett")
        x_Doc_Numero_Sin_Accettazione = dr.Item("Doc_Numero_Sin")
        x_Doc_Numero_Accettazione = dr.Item("Doc_Numero")
        x_Doc_Numero_Des_Accettazione = dr.Item("Doc_Numero_Des")

        numeroBolla = Ricava_NumeroDocumento_Senza_Sequenza(x_Doc_Numero_Sin_Accettazione,
                                                            x_Doc_Numero_Accettazione,
                                                            x_Doc_Numero_Des_Accettazione)

        x_DataOra_Ingresso = dr.Item("DataOra_Ingresso")

        x_Note = If(String.IsNullOrWhiteSpace(dr.Item("Extra_Str")), dr.Item("Note"), dr.Item("Extra_Str"))
        If InStr(x_Note, "§") > 0 Then
            x_Note = Replace(x_Note, "§", "<br>")
        End If

        'x_Tipo_Peso = dr.Item("Tipo_Peso")
        'x_Peso = dr.Item("Peso")
        x_Tara_Veicolo = dr.Item("Tara_Veicolo")

        'x_Colli = dr.Item("Colli")

        Data_DDTConf = ""
        Numero_DDTConf = ""

        If dr.Item("lav_cod") = LAVCOD_ACCETTAZIONE_DIVERSI OrElse
           dr.Item("lav_cod") = LAVCOD_BOLLA_RICEVUTA Then

            x_Data_DDTConf = dr.Item("Data_Conf")
            If x_Data_DDTConf = AGRODATAINIZIO Then
                Data_DDTConf = ""
            Else
                Data_DDTConf = x_Data_DDTConf
            End If


            x_Doc_Numero_Sin_DDTConf = dr.Item("Doc_Numero_Sin_Conf")
            x_Doc_Numero_DDTConf = dr.Item("Doc_Numero_Conf")
            x_Doc_Numero_Des_DDTConf = dr.Item("Doc_Numero_Des_Conf")

            If x_Doc_Numero_DDTConf <> 0 Then
                Numero_DDTConf = Ricava_NumeroDocumento_Senza_Sequenza(x_Doc_Numero_Sin_DDTConf,
                                                                       x_Doc_Numero_DDTConf,
                                                                       x_Doc_Numero_Des_DDTConf)
            End If
        End If
        causaleTrasporto = dr.Item("Causale_Trasporto")
        aspettoBeni = dr.Item("Aspetto")
        naturaBeni = dr.Item("Natura_Beni")

        '  x_ChkLayout_Join_Prodotti = dr.Item("ChkLayout_Join_Prodotti")

        '=============================================================
        '------------------- CONFERENTE  ---------------------------

        x_Cod_RisUm = dr.Item("Cod_RisUm")
        If x_Cod_RisUm <> 0 Then

            '22/05/2017
            Codice_Conferente = CStr(dr.Item("Codice_Conferente"))
            x_Rag_Soc = CStr(dr.Item("Rag_Soc_Conferente")) & CStr(dr.Item("Nome_Conferente")) & " " & CStr(dr.Item("Cognome_Conferente"))
            'If bool_Layout1 Then
            '    x_Rag_Soc = CStr(dr.Item("Codice_Conferente")) & " - " & CStr(dr.Item("Rag_Soc_Conferente")) & CStr(dr.Item("Nome_Conferente")) & " " & CStr(dr.Item("Cognome_Conferente"))
            'Else
            '    x_Rag_Soc = CStr(dr.Item("Rag_Soc_Conferente")) & CStr(dr.Item("Nome_Conferente")) & " " & CStr(dr.Item("Cognome_Conferente"))
            'End If

            x_Cod_Contatto = dr.Item("Cod_Contatto_Conferente")
            x_Codice_Fiscale = dr.Item("Codice_Fiscale_Conferente")
            Ricava_Piva_Codicefiscale_VersioneSenzaIDCF(x_Cod_Contatto, x_Codice_Fiscale, Piva_Conferente, CodiceFiscale_Conferente, Nothing)
            Piva_Conferente = "Partita IVA: " & Piva_Conferente
            CodiceFiscale_Conferente = "Codice Fiscale: " & CodiceFiscale_Conferente


            x_Cod_IndirizzoRisUm = dr.Item("Cod_IndirizzoRisUm")
            If x_Cod_IndirizzoRisUm <> 0 Then
                x_Ind_Des = dr.Item("ind_des_Conferente")
                x_Frz_Des = dr.Item("frz_des_Conferente")
                x_Cap = dr.Item("cap_Conferente")
                x_Comune = dr.Item("localita_Conferente")
                x_Provincia = dr.Item("comuni_prov_Conferente")
                x_Stato = dr.Item("stato_Conferente")
                Sistema_Comune_Provincia(x_Comune, x_Provincia)
                If x_Provincia <> "" Then
                    x_Provincia = "(" & x_Provincia & ")"
                End If
            End If
        End If


        '=============================================================
        '------------------------ AGENTE -----------------------------
        x_Agente_Cod = dr.Item("Agente_Cod")
        If x_Agente_Cod <> 0 Then
            Parametro_AgenteInfo = ImpostaAgente(logErrori, x_Agente_Cod, _objParametriServer)
        End If

        '=============================================================
        '------------------------ VETTORE --------------------------

        x_Cod_Vettore = dr.Item("Cod_Vettore")
        x_Mezzo = dr.Item("Mezzo")
        Select Case x_Mezzo
            Case 0
                mezzoDes = "CEDENTE"
            Case 1
                mezzoDes = "CESSIONARIO"
            Case 2
                mezzoDes = "VETTORE"
            Case Else
                mezzoDes = ""
        End Select

        If x_Cod_Vettore <> 0 Then
            x_RagSoc_Vettore = CStr(dr.Item("Rag_Soc_Vettore")) & CStr(dr.Item("Nome_Vettore")) & " " & CStr(dr.Item("Cognome_Vettore"))
            x_CodContatto_Vettore = dr.Item("Cod_Contatto_Vettore")
            x_CodiceFiscale_Vettore = dr.Item("Codice_Fiscale_Vettore")
            Ricava_Piva_Codicefiscale_VersioneSenzaIDCF(x_CodContatto_Vettore, x_CodiceFiscale_Vettore, pivaVettore, codiceFiscaleVettore, Nothing)
            x_Cod_IndirizzoVettore = dr.Item("Cod_IndirizzoVettore")
            x_TargaMezzo_Vettore = dr.Item("TargaMezzoVettore")
            If x_Cod_IndirizzoVettore <> 0 Then
                x_IndDes_Vettore = dr.Item("Ind_Des_Vettore")
                x_FrzDes_Vettore = dr.Item("Frz_Des_Vettore")
                x_Cap_Vettore = dr.Item("Cap_Vettore")
                x_Comune_Vettore = dr.Item("Localita_Vettore")
                x_Provincia_Vettore = dr.Item("Comuni_Prov_Vettore")
                x_Stato_Vettore = dr.Item("Stato_Vettore")
                Sistema_Comune_Provincia(x_Comune_Vettore, x_Provincia_Vettore)
            End If

            dtContattiCodici = objContattiCodici.Leggi(_piva, x_CodContatto_Vettore,
                                                       enum_CodiciAnagrafe.NumeroIscrizioneAlboAutotrasportatori, "",
                                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                       "", "",
                                                       _objParametriServer)

            If dtContattiCodici.Rows.Count <> 0 Then
                If Not IsDBNull(dtContattiCodici.Rows(0).Item("Val_cod")) Then
                    If (Trim(dtContattiCodici.Rows(0).Item("Val_cod")) <> "" And Len(dtContattiCodici.Rows(0).Item("Val_cod")) > 1) Then
                        x_NumReg_Vettore = dtContattiCodici.Rows(0).Item("Val_cod")
                    End If
                End If
            End If
        End If


        'valorizzo sul DataSet i dati
        drIntestazioneNew = ds.IntestazioneFattura.NewIntestazioneFatturaRow

        drIntestazioneNew.SuperPiva = _piva

        drIntestazioneNew.Extra_Str_1 = Format(_dataAccettazione, "dd/MM/yyyy")
        drIntestazioneNew.Extra_Str_2 = Format(x_DataOra_Ingresso, "dd/MM/yyyy HH:mm")
        drIntestazioneNew.Doc_Numero = numeroBolla

        drIntestazioneNew.NSBanca = Data_DDTConf
        If Numero_DDTConf <> "0" Then
            drIntestazioneNew.VSBanca = Numero_DDTConf
        End If
        drIntestazioneNew.Rag_Soc = x_Rag_Soc
        drIntestazioneNew.Imposta = Codice_Conferente
        drIntestazioneNew.Ind_Des = x_Ind_Des
        drIntestazioneNew.Cap = x_Cap
        drIntestazioneNew.Frz_Des = x_Frz_Des
        drIntestazioneNew.Comune = x_Comune
        drIntestazioneNew.Provincia = x_Provincia
        drIntestazioneNew.Piva = Piva_Conferente
        drIntestazioneNew.Scadenza = CodiceFiscale_Conferente

        drIntestazioneNew.Imponibile = causaleTrasporto
        drIntestazioneNew.Iva = aspettoBeni
        drIntestazioneNew.Sconti = naturaBeni

        drIntestazioneNew.Mezzo = mezzoDes
        drIntestazioneNew.Note = x_Note


        drIntestazioneNew.VettorePiva = ""
        If Not IsDBNull(x_CodContatto_Vettore) Then
            If Len(Trim(x_CodContatto_Vettore)) > 0 Then
                drIntestazioneNew.VettorePiva = "P. Iva: " & x_CodContatto_Vettore
            End If
        End If
        If IsNumeric(x_CodContatto_Vettore) Then
            If x_CodContatto_Vettore < 0 Then
                drIntestazioneNew.VettorePiva = ""
            End If
        End If

        drIntestazioneNew.VettoreTarga = ""
        If Not IsDBNull(x_TargaMezzo_Vettore) Then
            If Len(Trim(x_TargaMezzo_Vettore)) > 0 Then
                drIntestazioneNew.VettoreTarga = "Targa: " & x_TargaMezzo_Vettore
            End If
        End If
        drIntestazioneNew.VettoreRag_Soc = x_RagSoc_Vettore
        drIntestazioneNew.VettoreInd_Des = x_IndDes_Vettore
        drIntestazioneNew.VettoreFrz_Des = x_FrzDes_Vettore
        drIntestazioneNew.VettoreCap = x_Cap_Vettore
        drIntestazioneNew.VettoreComune = x_Comune_Vettore
        drIntestazioneNew.VettoreProvincia = x_Provincia_Vettore

        drIntestazioneNew.VettoreNumReg = ""
        If Not IsDBNull(x_NumReg_Vettore) Then
            If Trim(x_NumReg_Vettore) <> "0" AndAlso Trim(x_NumReg_Vettore) <> "" Then
                drIntestazioneNew.VettoreNumReg = "Albo: " & x_NumReg_Vettore
            End If
        End If

        '17/05/2017 sostituzione dei campi del DataSet con i parametri
        Parametro_Vettore_Riga1 = drIntestazioneNew.VettoreRag_Soc
        Parametro_Vettore_Riga2 = drIntestazioneNew.VettoreInd_Des & " " & drIntestazioneNew.VettoreFrz_Des
        Parametro_Vettore_Riga3 = drIntestazioneNew.VettoreCap & " " & drIntestazioneNew.VettoreComune & " " & drIntestazioneNew.VettoreProvincia
        Parametro_Vettore_Riga4 = drIntestazioneNew.VettorePiva & " " & drIntestazioneNew.VettoreTarga & " " & drIntestazioneNew.VettoreNumReg
        'If x_N_Autorizzazione_Trasporto <> "" Then
        '    Parametro_Vettore_Riga4 &= "N.Autorizz.Trasp.: " & x_N_Autorizzazione_Trasporto
        'End If
        Parametro_Vettore_Riga4 = Parametro_Vettore_Riga4.Trim

        '=============================================================
        '------------------------ LOGO -------------------------------
        CaricaLogoInCampoBlobFattura(logErrori, drIntestazioneNew, _piva, STAMPE_CONTAB_LOGO_IN_BASSO)
        
        ds.IntestazioneFattura.Rows.Add(drIntestazioneNew)

    End Sub


    Private Sub InserisciRiga_DataSet(ByRef ds As DataSetFattura,
                                      ByVal flagRaggruppamento As Boolean,
                                      ByVal contatore As Integer,
                                      ByVal contatoreRaggruppamento As Integer,
                                      ByVal prgRiga As String,
                                      ByVal descProdotto As String,
                                      ByVal imballaggio As String,
                                      ByVal contenitore As String,
                                      ByVal confezione As String,
                                      ByVal numImballaggio As Integer,
                                      ByVal numContenitore As Integer,
                                      ByVal numConfezione As Integer,
                                      ByVal pesoNetto As Decimal,
                                      ByVal tara As Decimal,
                                      ByVal pesoLordo As Decimal,
                                      ByVal qta As Decimal,
                                      ByVal udmDes As String)

        Dim confezionamento As String = ""
        Dim spazio As String = ""
        Const spazio2 As String = "  "
        Const spazio3 As String = "   "

        '22/05/2017: ripristinata la visualizzazione completa del confezionamento
        'con aggiunta dell'andata a capo
        If imballaggio <> "" Then
            spazio = If(CStr(numImballaggio).Length = 1, spazio3, spazio2)
            confezionamento &= CStr(numImballaggio) & spazio & imballaggio
        End If

        If contenitore <> "" Then
            If confezionamento <> "" Then
                confezionamento &= "<br>"
            End If
            spazio = If(CStr(numContenitore).Length = 1, spazio3, spazio2)
            confezionamento &= CStr(numContenitore) & spazio & contenitore
        End If

        If confezione <> "" Then
            If confezionamento <> "" Then
                confezionamento &= "<br>"
            End If
            spazio = If(CStr(numConfezione).Length = 1, spazio3, spazio2)
            confezionamento &= CStr(numConfezione) & spazio & confezione
        End If



        Dim drDescrizioneNew As DataSetFattura.DescrizioneRow
        '-----------------------------------------
        '------ inserisco i dati nel DataSet -----
        '-----------------------------------------

        drDescrizioneNew = ds.Descrizione.NewDescrizioneRow

        'la sfiga su flag_Raggruppamento è che al primo dettaglio non si sa se verrà fatto un raggruppamento
        'quindi nel contatore metterà prgRiga
        'che nei successivi dettagli però poi può coincidere con contatore_raggruppamento e dare errore
        '---> uso allora sempre contatore_raggruppamento
        drDescrizioneNew.Contatore = contatoreRaggruppamento
        'If flag_Raggruppamento = False Then
        '    'in questa bolla non c'è raggruppamento in atto
        '    'posso usare il prgRiga salvato
        '    If PrgRiga = "" Then
        '        drDescrizioneNew.Contatore = contatore
        '    Else
        '        drDescrizioneNew.Contatore = prgRiga
        '    End If
        'Else
        '    'è stato fatto il raggruppamento, non posso usare PrgRiga perché altrimenti ci sarebbero dei buchi
        '    drDescrizioneNew.Contatore = contatore_raggruppamento
        'End If

        'If PrgRiga = "" Then
        '    drDescrizioneNew.Contatore = i + 1
        'Else
        '    drDescrizioneNew.Contatore = prgRiga
        'End If

        drDescrizioneNew.Descrizione = descProdotto

        'RICICLO IL CAMPO SCONTO PER IL CONFEZIONAMENTO
        drDescrizioneNew.Sconto = confezionamento

        drDescrizioneNew.Qta = qta 'pesoNetto

        drDescrizioneNew.Extra_Str_1 = Format(pesoNetto, "#,###,##0.00")
        drDescrizioneNew.Extra_Str_2 = Format(tara, "#,###,##0.00")
        drDescrizioneNew.Extra_Str_3 = Format(pesoLordo, "#,###,##0.00")

        drDescrizioneNew.Udm_Des = udmDes ' "kg" ' x_Unita_Misura


        ''questa info è stata messa nel DataSet dei dettagli anche se non ha senso
        ''If bool_Layout1 = True Then
        ''    drDescrizioneNew.Extra_Str_5 = "(CESSIONE CON PREZZO DA DETERMINARE: D.M. 15.11.1975)"
        ''Else
        ''    drDescrizioneNew.Extra_Str_5 = ""
        ''End If
        'drDescrizioneNew.Extra_Str_5 = "(CESSIONE CON PREZZO DA DETERMINARE: D.M. 15.11.1975)"

        ds.Descrizione.Rows.Add(drDescrizioneNew)

    End Sub

    '##################################################################################
    'usa il DataSet dell'iva (per 3 campi non ne è stato creato uno nuovo)
    Public Sub Carica_DSImballaggi_StampaBolla(ByVal piva As String,
                                               ByVal idAgenda As Integer,
                                               ByRef logErrori As String,
                                               ByRef dsRiepilogoImballiEntrata As DS_Iva)

        '====================================================================================
        'RIEPILOGO IMBALLAGGI

        '----------------------
        'IMBALLAGGI IN ENTRATA
        '----------------------

        Dim dtImballiEntrata As DataTable = Nothing

        Try

            Dim AdD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi
            dtImballiEntrata = AdD.ImballiEntrata_Leggi(piva,
                                                        idAgenda,
                                                        False,
                                                        False,
                                                        "", "",
                                                        _objParametriServer)

        Catch ex As Exception
            logErrori &= "- Lettura degli imballaggi in entrata: " & vbCrLf & ex.Message & vbCrLf
        End Try


        Try

            If Not IsNothing(dtImballiEntrata) AndAlso dtImballiEntrata.Rows.Count <> 0 Then

                Dim drImballoEntrata As DS_Iva.DS_IvaRow
                Dim matDesImballo As String
                Dim numImballo As Integer
                Dim taraImballo As Decimal

                For i As Integer = 0 To dtImballiEntrata.Rows.Count - 1

                    matDesImballo = dtImballiEntrata.Rows(i).Item("Mat_Des_ImballiEntrata")
                    numImballo = dtImballiEntrata.Rows(i).Item("Qta_ImballiEntrata")
                    taraImballo = dtImballiEntrata.Rows(i).Item("Qta_Extra_ImballiEntrata")

                    '===================================================================

                    drImballoEntrata = dsRiepilogoImballiEntrata.DS_Iva.NewDS_IvaRow

                    drImballoEntrata.Imponibile2 = matDesImballo
                    drImballoEntrata.Cod_Iva_2 = CStr(numImballo)
                    drImballoEntrata.Imposta2 = CStr(taraImballo)

                    dsRiepilogoImballiEntrata.DS_Iva.Rows.Add(drImballoEntrata)

                    '===================================================================

                Next

            End If

            'TODO: se devo mostrare anche i confezionamenti, visto che non sono caricati separatamente ma insieme al prodotto, devo: 
            'recuperare la descrizione del confezionamento tramite il cal_cod (oconfezione) e qta da mov_det.qta (ovviamente udm = numero)
            ' e tara sempre da cal_cod

        Catch ex As Exception
            logErrori &= "- Riepilogo Imballaggi in Entrata: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

End Class