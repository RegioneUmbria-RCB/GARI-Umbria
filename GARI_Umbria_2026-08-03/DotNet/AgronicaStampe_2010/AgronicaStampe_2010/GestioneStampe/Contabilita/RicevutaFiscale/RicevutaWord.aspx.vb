Imports System.Xml
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreStampeDAL

Public Class RicevutaWord
    Inherits System.Web.UI.Page

    Private _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _objParametriUtenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Private _piva As String
    Private _lavCod As Integer
    Private _idAgenda As Integer
    Private _dataMovimento As Date
    Private _identificazioneDocumento As String = ""

    Private _nuoviArrotondamenti As Boolean = False
    Private _objConfigStampe As ConfigurazioneStampe = Nothing

    Const FILLER As String = "                                                                                                                                                                                                "
    'il prefisso lo metto già nel nome nel file xml                                                                                                        
    'const Str_Inizio As String = "@#"
    Const Str_Inizio As String = ""

    Const NUM_DETTAGLI_GESTITI As Integer = 17
    Const indice_casella_primo_dettaglio As Integer = 12 '27

    '#####################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Faccio scadere subito la pagina memorizzata nella cache
        Response.Expires = 0


        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        Try

            _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            _objParametriUtenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

            _nuoviArrotondamenti = UsaNuoviArrotondamenti(_objParametriServer)
            _objConfigStampe = New ConfigurazioneStampe()

            Dim logErrori As String = ""
            Dim nomeDocumento As String = "RicevutaFiscaleWord"

            If Not Page.IsPostBack Then

                '--- Recupero i dati dalla querystring

                '##############################################################
                '############  Lettura Parametri Query String #################
                '##############################################################

                _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

                _idAgenda = Stringa_Decodifica(CStr(Request.QueryString("i")), AgroKey_EncoderDecoder, Server)

                _lavCod = Stringa_Decodifica(CStr(Request.QueryString("l")), AgroKey_EncoderDecoder, Server)

            End If

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim ds As New DS_RicevutaFiscaleA5

            Try

                logErrori = ""

                Stampa_RicevutaFiscaleWord(ds, logErrori)

            Catch exc As Exception
                logErrori &= "- PageLoad: " & vbCrLf & exc.Message & vbCrLf
            End Try


            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            SalvaLogErrori_Agenda(logErrori, nomeDocumento, _identificazioneDocumento, "RicevutaWord.aspx", "Stampe_Contabilita", _idAgenda, _objParametriServer)

        Catch ex As Exception
            AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Errore: " & ex.Message, Page)
        End Try

    End Sub


    '#####################################################################################################
    Private Sub Stampa_RicevutaFiscaleWord(ByRef DS As DS_RicevutaFiscaleA5,
                                           ByRef logErrori As String)

        Dim PathOrigine As String = ""
        Dim PathDestinazione As String = ""
        Dim NomeFileOrigine As String = ""
        Dim NomeFileDestinazione As String = ""
        Dim NomeFileXML As String = ""

        Dim XmlDoc As New XmlDocument
        Dim XMLricevuta As XmlElement

        '2 colonne: nome casella di testo e valore 
        Dim NUM_COLONNE As Integer = 2

        Dim Flag1 As Integer = Asc("@")
        Dim Flag2 As Integer = Asc("#")

        Dim i As Integer

        '2
        Dim xNumero As String
        Dim xData As String

        ''6
        Dim xIntestazione_RagSoc As String
        Dim xIntestazione_Indirizzo As String
        Dim xIntestazione_PivaCF As String
        Dim xIntestazione_TelCellFax As String
        Dim xIntestazione_SitoMail As String
        'Dim xIntestazione_Piva As String
        'Dim xIntestazione_CF As String

        '9
        Dim xCliente_RagSoc As String
        Dim xCliente_Indirizzo As String
        'Dim xCliente_Piva_CF As String

        Dim xDettagli_Qta As String
        Dim xDettagli_Udm As String
        Dim xDettagli_Desc As String
        Dim xDettagli_Importo As String

        ''57
        'Dim xIva_Imponibile1 As String
        'Dim xIva_Aliquota1 As String
        'Dim xIva_Imposta1 As String

        ''60
        'Dim xIva_Imponibile2 As String
        'Dim xIva_Aliquota2 As String
        'Dim xIva_Imposta2 As String

        ''63
        'Dim xIva_Imponibile3 As String
        'Dim xIva_Aliquota3 As String
        'Dim xIva_Imposta3 As String

        ''66
        'Dim xIva_Imponibile4 As String
        'Dim xIva_Aliquota4 As String
        'Dim xIva_Imposta4 As String

        ''69
        'Dim xIva_Imponibile5 As String
        'Dim xIva_Aliquota5 As String
        'Dim xIva_Imposta5 As String

        '72
        Dim xRiepilogo_CorrPagato As String
        Dim xRiepilogo_CorrNonPagato As String
        Dim xRiepilogo_TotaleDoc As String

        Try

            '--- Definisco i nomi dei due file di ORIGINE e DESTINAZIONE

            PathOrigine = Server.MapPath(_piva)

            PathDestinazione = Server.MapPath("../../../File_Temporanei")


            '--- Aggiungo l'eventuale carattere "\" in fondo al path

            If Not PathOrigine.EndsWith("\") Then
                PathOrigine &= "\"
            End If

            If Not PathDestinazione.EndsWith("\") Then
                PathDestinazione &= "\"
            End If

            NomeFileOrigine = "RicevutaFiscaleBuffetti.doc"
            NomeFileDestinazione = "RicevutaFiscaleBuffetti" & Guid.NewGuid.ToString & ".doc"

            NomeFileXML = PathOrigine & "RicevutaFiscaleConfig.xml"

            'Verifico se esiste la cartella
            If Not IO.File.Exists(NomeFileXML) Then
                Dim messaggio As String
                messaggio = "La stampa della ricevuta fiscale su layout A5 Buffetti non è abilitata." & vbCrLf &
                            "Contattare com@agronica.it per richiederne l'attivazione." & vbCrLf &
                            "In alternativa è possibile stampare la ricevuta A5 in doppia copia (su foglio A4 già numerato dalla tipografia), modificando l'opzione di stampa dal menù Strumenti -> Opzioni."
                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox(messaggio, Page)
                Throw New Exception(messaggio)
            End If


            Dim Segnaposto(0, NUM_COLONNE) As String
            Dim Vet_XML(0) As XmlElement
            Dim Vet_Nome(0) As String
            Dim Vet_NumCaratteri(0) As Integer
            Dim numElementi As Integer

            Try

                XmlDoc.Load(NomeFileXML)

                XMLricevuta = XmlDoc.SelectSingleNode("ricevuta_fiscale")

                Dim List_XML As XmlNodeList = XMLricevuta.ChildNodes

                numElementi = List_XML.Count

                ReDim Segnaposto(numElementi - 1, NUM_COLONNE - 1)
                ReDim Vet_XML(numElementi - 1)
                ReDim Vet_Nome(numElementi - 1)
                ReDim Vet_NumCaratteri(numElementi - 1)

                Dim nomeCasella As String

                For i = 0 To numElementi - 1

                    nomeCasella = "casella" & Right("0" & CStr(i), 2)

                    Vet_XML(i) = XMLricevuta.SelectSingleNode(nomeCasella)

                    Vet_Nome(i) = Vet_XML(i).GetAttribute("nome")

                    Vet_NumCaratteri(i) = Vet_Nome(i).Length

                Next 'elementi xml

            Catch ex As Exception
                Throw New Exception("Errore nella gestione dell'xml: " & ex.Message)
            End Try

            Dim objPrint As New RicevutaFiscale_GestioneStampa
            Dim Flag_SuperatoNumDettagliGestiti As Boolean

            Try

                objPrint.Stampa_RicevutaFiscale(_objParametriServer,
                                                _objParametriUtenti,
                                                DS,
                                                logErrori,
                                                enum_TipoStampaRicevutaFiscale.WordA5Personalizzabile,
                                                _identificazioneDocumento,
                                                _piva,
                                                _idAgenda,
                                                _lavCod,
                                                CInt(Session("ASG_ProgressivoGIAS")),
                                                NUM_DETTAGLI_GESTITI,
                                                Flag_SuperatoNumDettagliGestiti,
                                                _dataMovimento,
                                                False,
                                                _nuoviArrotondamenti,
                                                _objConfigStampe)

            Catch ex As Exception
                Throw New Exception("Errore nella lettura dei dati: " & ex.Message)
            End Try

            If Flag_SuperatoNumDettagliGestiti = True Then
                AgronicaCoreDataProvider.UtilityProvider.AgroMsgBox("Superato il numero massimo di dettagli gestiti da questa modalità di stampa della ricevuta fiscale!" & vbCrLf &
                            "Per la stampa con modulo word, inserire al massimo " & CStr(NUM_DETTAGLI_GESTITI) & " dettagli.", Page)
                Exit Sub
            End If

            Try

                xNumero = DS.Intestazione.Rows(0).Item("Numero")
                xData = DS.Intestazione.Rows(0).Item("Data")

                xIntestazione_RagSoc = DS.Intestazione.Rows(0).Item("Rag_Soc")
                xIntestazione_Indirizzo = DS.Intestazione.Rows(0).Item("Int_Riga1")
                xIntestazione_PivaCF = DS.Intestazione.Rows(0).Item("Int_Riga2")
                xIntestazione_TelCellFax = DS.Intestazione.Rows(0).Item("Int_Riga3")
                xIntestazione_SitoMail = DS.Intestazione.Rows(0).Item("Int_Riga4")

                xCliente_RagSoc = DS.Intestazione.Rows(0).Item("Cliente_Riga1")
                xCliente_Indirizzo = DS.Intestazione.Rows(0).Item("Cliente_Riga2")
                'xCliente_Piva_CF = DS.Intestazione.Rows(0).Item("Data")

                xRiepilogo_CorrPagato = DS.Intestazione.Rows(0).Item("Corrispettivo_Pagato")
                xRiepilogo_CorrNonPagato = DS.Intestazione.Rows(0).Item("Corrispettivo_NonPagato")
                xRiepilogo_TotaleDoc = DS.Intestazione.Rows(0).Item("Totale")

            Catch ex As Exception
                Throw New Exception("Errore nella valorizzazione dei dati intestazione e riepilogo: " & ex.Message)
            End Try

            Try

                '0 =NUMERO 
                SegnapostoFill(Segnaposto,
                               0,
                               Vet_Nome(0),
                               xNumero,
                               Vet_NumCaratteri(0))
                '1=DATA
                SegnapostoFill(Segnaposto,
                               1,
                               Vet_Nome(1),
                               xData,
                               Vet_NumCaratteri(1))

                '2= RAG SOC
                SegnapostoFill(Segnaposto,
                               2,
                               Vet_Nome(2),
                               xIntestazione_RagSoc,
                               Vet_NumCaratteri(2))

                '3 INDIRIZZO
                SegnapostoFill(Segnaposto,
                               3,
                               Vet_Nome(3),
                               xIntestazione_Indirizzo,
                               Vet_NumCaratteri(3))

                '4 PIVA CF
                SegnapostoFill(Segnaposto,
                               4,
                               Vet_Nome(4),
                               xIntestazione_PivaCF,
                               Vet_NumCaratteri(4))

                '5 TEL CELL FAX
                SegnapostoFill(Segnaposto,
                               5,
                               Vet_Nome(5),
                               xIntestazione_TelCellFax,
                               Vet_NumCaratteri(5))

                '5 TEL CELL FAX
                SegnapostoFill(Segnaposto,
                                5,
                                Vet_Nome(5),
                                xIntestazione_TelCellFax,
                                Vet_NumCaratteri(5))


                '6= sito mail
                SegnapostoFill(Segnaposto,
                               6,
                               Vet_Nome(6),
                               xIntestazione_SitoMail,
                               Vet_NumCaratteri(6))

                '7 CLIENTE RAG SOC
                SegnapostoFill(Segnaposto,
                               7,
                               Vet_Nome(7),
                               xCliente_RagSoc,
                               Vet_NumCaratteri(7))

                '8 CLIENTE INDIRIZZO
                SegnapostoFill(Segnaposto,
                               8,
                               Vet_Nome(8),
                               xCliente_Indirizzo,
                               Vet_NumCaratteri(8))

                'non c'è nella nuova versione
                'SegnapostoFill(Segnaposto,
                '               ,
                '               Vet_Nome(),
                '               xCliente_Piva_CF,
                '               Vet_NumCaratteri())

                'RIEPILOGO             
                SegnapostoFill(Segnaposto,
                               9,
                               Vet_Nome(9),
                               xRiepilogo_CorrPagato,
                               Vet_NumCaratteri(9))

                SegnapostoFill(Segnaposto,
                               10,
                               Vet_Nome(10),
                               xRiepilogo_CorrNonPagato,
                               Vet_NumCaratteri(10))

                SegnapostoFill(Segnaposto,
                               11,
                               Vet_Nome(11),
                               xRiepilogo_TotaleDoc,
                               Vet_NumCaratteri(11))

                'MODIFICA DEL 06/09/2012: 
                'nella ricevuta fiscale non è obbligatorio il riepilogo iva
                'lo elimino
                ' '===============================================
                ' '===============================================
                ' 'RIEPILOGO IVA
                ' For i = 0 To DSIva.DS_Iva.Rows.Count - 1

                '     Select Case i

                '         Case 0
                '             xIva_Imponibile1 = DSIva.DS_Iva.Rows(i).Item("Imponibile2")
                '             xIva_Aliquota1 = DSIva.DS_Iva.Rows(i).Item("Cod_Iva_2")
                '             xIva_Imposta1 = DSIva.DS_Iva.Rows(i).Item("Imposta2")

                '         Case 1
                '             xIva_Imponibile2 = DSIva.DS_Iva.Rows(i).Item("Imponibile2")
                '             xIva_Aliquota2 = DSIva.DS_Iva.Rows(i).Item("Cod_Iva_2")
                '             xIva_Imposta2 = DSIva.DS_Iva.Rows(i).Item("Imposta2")

                '         Case 2
                '             xIva_Imponibile3 = DSIva.DS_Iva.Rows(i).Item("Imponibile2")
                '             xIva_Aliquota3 = DSIva.DS_Iva.Rows(i).Item("Cod_Iva_2")
                '             xIva_Imposta3 = DSIva.DS_Iva.Rows(i).Item("Imposta2")

                '         Case 3
                '             xIva_Imponibile4 = DSIva.DS_Iva.Rows(i).Item("Imponibile2")
                '             xIva_Aliquota4 = DSIva.DS_Iva.Rows(i).Item("Cod_Iva_2")
                '             xIva_Imposta4 = DSIva.DS_Iva.Rows(i).Item("Imposta2")

                '         Case 4
                '             xIva_Imponibile5 = DSIva.DS_Iva.Rows(i).Item("Imponibile2")
                '             xIva_Aliquota5 = DSIva.DS_Iva.Rows(i).Item("Cod_Iva_2")
                '             xIva_Imposta5 = DSIva.DS_Iva.Rows(i).Item("Imposta2")

                '         Case Else
                '             Log_Errori &= "Nel riepilogo IVA ci sono più di 5 dettagli, occorre gestire i mancanti." & vbCrLf

                '     End Select

                ' Next 'ciclo iva

                ' '1 riga
                ' SegnapostoFill(Segnaposto,
                '                12,
                '                Vet_Nome(12),
                '                xIva_Imponibile1,
                '                Vet_NumCaratteri(12))

                ' SegnapostoFill(Segnaposto,
                '                13,
                '                Vet_Nome(13),
                '                xIva_Aliquota1,
                '                Vet_NumCaratteri(13))

                ' SegnapostoFill(Segnaposto,
                '                14,
                '                Vet_Nome(14),
                '                xIva_Imposta1,
                '                Vet_NumCaratteri(14))

                ' '2 riga
                'SegnapostoFill(Segnaposto,
                '               15,
                '               Vet_Nome(15),
                '               xIva_Imponibile2,
                '               Vet_NumCaratteri(15))

                ' SegnapostoFill(Segnaposto,
                '                16,
                '                Vet_Nome(16),
                '                xIva_Aliquota2,
                '                Vet_NumCaratteri(16))

                ' SegnapostoFill(Segnaposto,
                '                17,
                '                Vet_Nome(17),
                '                xIva_Imposta2,
                '                Vet_NumCaratteri(17))

                ' '3 riga
                'SegnapostoFill(Segnaposto,
                '               18,
                '               Vet_Nome(18),
                '               xIva_Imponibile3,
                '               Vet_NumCaratteri(18))

                'SegnapostoFill(Segnaposto,
                '               19,
                '               Vet_Nome(19),
                '               xIva_Aliquota3,
                '               Vet_NumCaratteri(19))

                'SegnapostoFill(Segnaposto,
                '               20,
                '               Vet_Nome(20),
                '               xIva_Imposta3,
                '               Vet_NumCaratteri(20))

                ' '4 riga
                'SegnapostoFill(Segnaposto,
                '               21,
                '               Vet_Nome(21),
                '               xIva_Imponibile4,
                '               Vet_NumCaratteri(21))

                'SegnapostoFill(Segnaposto,
                '               22,
                '               Vet_Nome(22),
                '               xIva_Aliquota4,
                '               Vet_NumCaratteri(22))

                'SegnapostoFill(Segnaposto,
                '               23,
                '               Vet_Nome(23),
                '               xIva_Imposta4,
                '               Vet_NumCaratteri(23))

                ' '5 riga
                ' SegnapostoFill(Segnaposto,
                '                24,
                '                Vet_Nome(24),
                '                xIva_Imponibile5,
                '                Vet_NumCaratteri(24))

                'SegnapostoFill(Segnaposto,
                '               25,
                '               Vet_Nome(25),
                '               xIva_Aliquota5,
                '               Vet_NumCaratteri(25))

                ' SegnapostoFill(Segnaposto,
                '                26,
                '                Vet_Nome(26),
                '                xIva_Imposta5,
                '                Vet_NumCaratteri(26))

                ' 'fine RIEPILOGO IVA
                ' '===============================================
                ' '===============================================


                Dim num_dettagli_ricevuta As Integer
                num_dettagli_ricevuta = DS.Dettagli.Rows.Count

                If num_dettagli_ricevuta > NUM_DETTAGLI_GESTITI Then
                    logErrori &= "Ci sono più di 17 dettagli, occorre gestire i mancanti." & vbCrLf
                End If

                Dim contatore_segnaposto_dettagli As Integer
                Dim contatore_segnaposto_dettaglioQTA As Integer

                Const num_casella_primo_dettaglio As Integer = indice_casella_primo_dettaglio

                contatore_segnaposto_dettaglioQTA = num_casella_primo_dettaglio


                Dim num_dettagli As Integer
                num_dettagli = (numElementi - num_casella_primo_dettaglio) / 4
                'inizializzazione della matrice segnaposto (si valorizzano tutti i campi di tutte le righe con stringa vuota)
                '(altrimenti se ci sono meno di 17 dettagli, non si valorizzano le righe successive della matrice, e si inchioda)
                For i = 0 To num_dettagli - 1

                    If i <> 0 Then
                        contatore_segnaposto_dettaglioQTA += 4
                    End If

                    SegnapostoFill(Segnaposto,
                                   contatore_segnaposto_dettaglioQTA,
                                   Vet_Nome(contatore_segnaposto_dettaglioQTA),
                                   "",
                                   Vet_NumCaratteri(contatore_segnaposto_dettaglioQTA))

                    contatore_segnaposto_dettagli = contatore_segnaposto_dettaglioQTA + 1

                    SegnapostoFill(Segnaposto,
                                   contatore_segnaposto_dettagli,
                                   Vet_Nome(contatore_segnaposto_dettagli),
                                   "",
                                   Vet_NumCaratteri(contatore_segnaposto_dettagli))

                    contatore_segnaposto_dettagli += 1

                    SegnapostoFill(Segnaposto,
                                   contatore_segnaposto_dettagli,
                                   Vet_Nome(contatore_segnaposto_dettagli),
                                   "",
                                   Vet_NumCaratteri(contatore_segnaposto_dettagli))

                    contatore_segnaposto_dettagli += 1

                    SegnapostoFill(Segnaposto,
                                   contatore_segnaposto_dettagli,
                                   Vet_Nome(contatore_segnaposto_dettagli),
                                   "",
                                   Vet_NumCaratteri(contatore_segnaposto_dettagli))

                Next

                contatore_segnaposto_dettaglioQTA = num_casella_primo_dettaglio


                'DETTAGLI
                For i = 0 To num_dettagli_ricevuta - 1

                    xDettagli_Qta = DS.Dettagli.Rows(i).Item("Qta")
                    xDettagli_Udm = DS.Dettagli.Rows(i).Item("Udm_Sim")
                    xDettagli_Desc = DS.Dettagli.Rows(i).Item("Descrizione")
                    xDettagli_Importo = DS.Dettagli.Rows(i).Item("Importo")
                    If IsNumeric(xDettagli_Importo) Then
                        xDettagli_Importo = Format(CDec(xDettagli_Importo), "#,###,##0.00")
                    End If

                    If i < NUM_DETTAGLI_GESTITI Then

                        If i <> 0 Then
                            contatore_segnaposto_dettaglioQTA += 4
                        End If

                        SegnapostoFill(Segnaposto,
                                       contatore_segnaposto_dettaglioQTA,
                                       Vet_Nome(contatore_segnaposto_dettaglioQTA),
                                       xDettagli_Qta,
                                       Vet_NumCaratteri(contatore_segnaposto_dettaglioQTA))

                        contatore_segnaposto_dettagli = contatore_segnaposto_dettaglioQTA + 1

                        SegnapostoFill(Segnaposto,
                                       contatore_segnaposto_dettagli,
                                       Vet_Nome(contatore_segnaposto_dettagli),
                                       xDettagli_Udm,
                                       Vet_NumCaratteri(contatore_segnaposto_dettagli))

                        contatore_segnaposto_dettagli += 1

                        SegnapostoFill(Segnaposto,
                                       contatore_segnaposto_dettagli,
                                       Vet_Nome(contatore_segnaposto_dettagli),
                                       xDettagli_Desc,
                                       Vet_NumCaratteri(contatore_segnaposto_dettagli))

                        contatore_segnaposto_dettagli += 1

                        SegnapostoFill(Segnaposto,
                                       contatore_segnaposto_dettagli,
                                       Vet_Nome(contatore_segnaposto_dettagli),
                                       xDettagli_Importo,
                                       Vet_NumCaratteri(contatore_segnaposto_dettagli))

                    Else
                        logErrori &= "Dettaglio in più:" & xDettagli_Desc & " " & xDettagli_Qta & " " & xDettagli_Udm & vbCrLf
                    End If

                Next 'ciclo dettagli

            Catch ex As Exception
                Throw New Exception("Errore nella gestione dei segnaposto dei dettagli: " & ex.Message)
            End Try

            Try

                '--- Recupero il file ORIGINE sottoforma di vettore di BYTE

                Dim ByteArray As Byte()
                Dim LunghezzaOrigine As Long

                Try

                    Dim OrigineFile As New IO.FileInfo(PathOrigine & NomeFileOrigine)
                    Dim OrigineFileStream As IO.FileStream = OrigineFile.OpenRead()
                    LunghezzaOrigine = OrigineFileStream.Length

                    If (LunghezzaOrigine > 0) Then
                        ReDim ByteArray(LunghezzaOrigine - 1)
                        OrigineFileStream.Read(ByteArray, 0, LunghezzaOrigine)
                        OrigineFileStream.Close()
                    End If

                Catch ex As Exception
                    Throw New Exception("Errore nell'elaborazione del file di origine: " & ex.Message)
                End Try

                Try

                    '--- Applico le modifiche richieste al vettore di BYTE (sostituzione dei segnaposto)

                    Dim iByte As Integer = 0
                    Dim iTrovato As Integer

                    For iByte = 0 To UBound(ByteArray)

                        'Se il carattere corrente e il successivo suggeriscono la presenza di un segnaposto ...
                        If (iByte < UBound(ByteArray)) AndAlso
                           (ByteArray(iByte) = Flag1) AndAlso
                           (ByteArray(iByte + 1) = Flag2) Then

                            'Devo verificare i caratteri successivi con i vari segnaposto
                            iTrovato = Verifica_Ciascun_Segnaposto(ByteArray, iByte, Segnaposto)

                            If iTrovato <> -1 Then
                                'Sostituisco il valore del segnaposto
                                Sostituzione_Segnaposto(ByteArray, iByte, Segnaposto, iTrovato)

                            End If

                        End If

                    Next

                Catch ex As Exception
                    Throw New Exception("Errore nella sostituzione dei segnaposto: " & ex.Message)
                End Try

                Try

                    '--- Scrivo il vettore di BYTE in un nuovo file DESTINAZIONE

                    Dim DestinazioneFile As New IO.FileInfo(PathDestinazione & NomeFileDestinazione)
                    Dim DestinazioneFileStream As IO.FileStream = DestinazioneFile.Create

                    DestinazioneFileStream.Write(ByteArray, 0, LunghezzaOrigine)
                    DestinazioneFileStream.Flush()
                    DestinazioneFileStream.Close()

                Catch ex As Exception
                    Throw New Exception("Errore nella scrittura del vettore di BYTE nel nuovo file DESTINAZIONE: " & ex.Message)
                End Try

                Try

                    '--- Apro il file di destinazione

                    Response.ClearContent()
                    Response.ClearHeaders()
                    'Response.ContentType = "application/pdf"
                    Response.ContentType = "application/msword"
                    Response.AddHeader("Content-Disposition", "inline; filename = RicevutaWord.doc")
                    Response.WriteFile(PathDestinazione & NomeFileDestinazione)
                    Response.Flush()
                    Response.Close()

                    ' il file esportato viene eliminato dal disco
                    IO.File.Delete(PathDestinazione & NomeFileDestinazione)


                Catch ex As Exception
                    Throw New Exception("Errore nell'apertura del file di destinazione: " & ex.Message)
                End Try

            Catch ex As Exception
                Throw New Exception("Errore nell'elaborazione dei file: " & ex.Message)
            End Try


            '-------------------------------------------------------------------------------------

        Catch ex As Exception
            logErrori &= ex.Message & vbCrLf
        End Try


    End Sub


    '#####################################################################################################
    Private Sub SegnapostoFill(ByRef Segnaposto(,) As String,
                               ByVal Indice_Riga As Integer,
                               ByVal NomeCasella As String,
                               ByVal Dato As String,
                               ByVal Lunghezza As Integer)


        Segnaposto(Indice_Riga, 0) = Str_Inizio & NomeCasella

        'Segnaposto(Indice_Riga, 1) = Fill(Vet_Nome(0), xNumero, Vet_NumCaratteri(0))

        Select Case Mid(NomeCasella, 3, 1).ToUpper
            Case "X" '->ALLINEAMENTO A SINISTRA SX
                Segnaposto(Indice_Riga, 1) = Left(Dato & FILLER, Lunghezza)
            Case "Y" '->ALLINEAMENTO A DESTRA DX
                Segnaposto(Indice_Riga, 1) = Right(FILLER & Dato, Lunghezza)
            Case Else
                Segnaposto(Indice_Riga, 1) = Left(Dato & FILLER, Lunghezza)
        End Select

    End Sub

    '#####################################################################################################
    Private Function Verifica_Ciascun_Segnaposto(ByRef ByteArray() As Byte,
                                                 ByRef iByte As Integer,
                                                 ByRef Segnaposto(,) As String
                                                 ) As Integer

        Dim iTrovato As Integer
        Dim iSegnaposto As Integer
        Dim iCarattere As Integer

        'Inizializzo
        iTrovato = -1

        'Per ciascun segnaposto ...
        For iSegnaposto = 0 To UBound(Segnaposto, 1)

            'Ho gia' verificato che i primi due caratteri corrispondono ...
            For iCarattere = 3 To Segnaposto(iSegnaposto, 0).Length

                iTrovato = iSegnaposto

                If ByteArray(iByte + iCarattere - 1) <> Asc(Mid(Segnaposto(iSegnaposto, 0), iCarattere, 1)) Then
                    'Basta un carattere diverso per scartare la parola ...
                    iTrovato = -1
                    Exit For
                End If

            Next

            'Se iTrovato e' diverso da -1 allora avevo trovato la parola nel ciclo interno ...
            If iTrovato <> -1 Then
                Exit For
            End If

        Next

        Return iTrovato

    End Function

    '#####################################################################################################
    Private Sub Sostituzione_Segnaposto(ByRef ByteArray() As Byte,
                                        ByRef iByte As Integer,
                                        ByRef Segnaposto(,) As String,
                                        ByVal iTrovato As Integer)

        Dim iCarattere As Integer

        For iCarattere = 1 To Segnaposto(iTrovato, 1).Length
            ByteArray(iByte + iCarattere - 1) = Asc(Mid(Segnaposto(iTrovato, 1), iCarattere, 1))
        Next

    End Sub

End Class