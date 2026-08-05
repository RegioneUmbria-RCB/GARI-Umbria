Imports System.Data
Imports System.Data.OleDb
Imports System.Xml
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Liquidita_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Liquidita_Scrivi(ByVal DatiLiquidita As String, _
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        '----------------------------------------------------------------------

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Liquidita_W.Liquidita_Scrivi()"

        '----------------------------------------------------------------------

        Dim XmlDoc As New XmlDocument

        Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim ObjLiquidita As New AgronicaCoreContabDAL.Liquidita_W

        Dim xRisp As Boolean
        Dim Cod_Liquidita As Long

        Dim xDatiLiquiditas As XmlNodeList
        Dim xDatiLiquidita As XmlElement
        Dim xLiquiditas As XmlNodeList
        Dim xLiquidita As XmlElement

        Dim i_DatiLiquidita As Integer
        Dim i_Liquidita As Integer

        Dim OpeDB_Liquidita As String

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim MessaggioErrore As String


        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                    FlagTransazioneLocale, _
                                                                                    objParametri)
            '------------------------------

            XmlDoc.LoadXml(DatiLiquidita)

            '------------------------------

            xDatiLiquiditas = XmlDoc.GetElementsByTagName("DatiLiquidita")

            i_DatiLiquidita = 0

            Do While i_DatiLiquidita < xDatiLiquiditas.Count

                'Prelevo l'i-esimo blocco di Dati Liquidità
                xDatiLiquidita = xDatiLiquiditas.Item(i_DatiLiquidita)


                '------------------------------

                xLiquiditas = xDatiLiquidita.GetElementsByTagName("Liquidita")

                i_Liquidita = 0

                Do While i_Liquidita < xLiquiditas.Count

                    'Prelevo l' i-esima Liquidità/Risorsa Finanziaria
                    xLiquidita = xLiquiditas.Item(i_Liquidita)

                    'Prelevo gli attributi del rapporto selezionato
                    OpeDB_Liquidita = xLiquidita.GetAttribute("TipoOperazioneDB")

                    'Inizializzo Preventivamente il Cod_Liquidita
                    Cod_Liquidita = CLng(xLiquidita.GetAttribute("cod_liquidita"))

                    'Verifico l'operazione richiesta
                    Select Case OpeDB_Liquidita
                        '
                        Case "0"    'LEGGI -------------------------------------------------------
                            '
                        Case "1"    'SALVA -------------------------------------------------------
                            '

                            'Richiedo un nuovo codice liquidità
                            Cod_Liquidita = ObjSequenze.NuovoId_Tabella("Liquidita",
                                                                        CInt(xLiquidita.GetAttribute("basecode")),
                                                                        CInt(xLiquidita.GetAttribute("topcode")),
                                                                        objParametri)

                            xRisp = ObjLiquidita.Scrivi(CStr(xLiquidita.GetAttribute("piva")),
                                                        CInt(xLiquidita.GetAttribute("sa_cod")),
                                                        Cod_Liquidita,
                                                        CStr(xLiquidita.GetAttribute("cod_contatto")),
                                                        CStr(xLiquidita.GetAttribute("riferimento")),
                                                        CStr(xLiquidita.GetAttribute("cau_risorsa")),
                                                        CInt(xLiquidita.GetAttribute("cod_istituto")),
                                                        CStr(xLiquidita.GetAttribute("numero")),
                                                        CStr(xLiquidita.GetAttribute("abi")),
                                                        CStr(xLiquidita.GetAttribute("cab")),
                                                        CStr(xLiquidita.GetAttribute("cin")),
                                                        CStr(xLiquidita.GetAttribute("cifre_controllo")),
                                                        CStr(xLiquidita.GetAttribute("nazione")),
                                                        CStr(xLiquidita.GetAttribute("bic")),
                                                        CStr(xLiquidita.GetAttribute("interbancario")),
                                                        CDbl(xLiquidita.GetAttribute("saldo_attuale")),
                                                        CDbl(xLiquidita.GetAttribute("saldo_iniziale")),
                                                        CInt(xLiquidita.GetAttribute("avviso")),
                                                        CDbl(xLiquidita.GetAttribute("importo_avviso")),
                                                        CStr(xLiquidita.GetAttribute("note")),
                                                        CStr(xLiquidita.GetAttribute(LCase("Rilevamento"))),
                                                        CStr(xLiquidita.GetAttribute(LCase("Data_Rilevamento"))),
                                                        CStr(xLiquidita.GetAttribute(LCase("Offset"))),
                                                        CStr(xLiquidita.GetAttribute(LCase("ChkDefault"))),
                                                        CStr(xLiquidita.GetAttribute(LCase("ChkAbilitazione"))),
                                                        CDate(xLiquidita.GetAttribute("validita_inizio")),
                                                        CDate(xLiquidita.GetAttribute("validita_fine")),
                                                        "", objParametri)


                        Case "2"    'MODIFICA -------------------------------------------------------
                            '

                            xRisp = ObjLiquidita.Modifica(
                                     CStr(xLiquidita.GetAttribute("piva")),
                                     CInt(xLiquidita.GetAttribute("sa_cod")),
                                     Cod_Liquidita,
                                     CStr(xLiquidita.GetAttribute("cod_contatto")),
                                     CStr(xLiquidita.GetAttribute("riferimento")),
                                     CStr(xLiquidita.GetAttribute("cau_risorsa")),
                                     CInt(xLiquidita.GetAttribute("cod_istituto")),
                                     CStr(xLiquidita.GetAttribute("numero")),
                                     CStr(xLiquidita.GetAttribute("abi")),
                                     CStr(xLiquidita.GetAttribute("cab")),
                                     CStr(xLiquidita.GetAttribute("cin")),
                                     CStr(xLiquidita.GetAttribute("cifre_controllo")),
                                     CStr(xLiquidita.GetAttribute("nazione")),
                                     CStr(xLiquidita.GetAttribute("bic")),
                                     CStr(xLiquidita.GetAttribute("interbancario")),
                                     CDbl(xLiquidita.GetAttribute("saldo_attuale")),
                                     CDbl(xLiquidita.GetAttribute("saldo_iniziale")),
                                     CInt(xLiquidita.GetAttribute("avviso")),
                                     CDbl(xLiquidita.GetAttribute("importo_avviso")),
                                     CStr(xLiquidita.GetAttribute("note")),
                                     CDate(xLiquidita.GetAttribute("validita_inizio")),
                                     CDate(xLiquidita.GetAttribute("validita_fine")),
                                     CInt(xLiquidita.GetAttribute("chkdefault")),
                                     "", objParametri)



                        Case "3" 'CANCELLAZIONE

                            'Cancellazione della liquidità
                            xRisp = ObjLiquidita.Cancella(
                                  CStr(xLiquidita.GetAttribute("piva")),
                                  CInt(xLiquidita.GetAttribute("sa_cod")),
                                  CInt(xLiquidita.GetAttribute("cod_liquidita")),
                                  "", "", objParametri)

                    End Select

                    '-------------------------------------------------------------

                    'Incremento l'indice
                    i_Liquidita = i_Liquidita + 1

                Loop

                '------------------------------

                'Incremento l'indice
                i_DatiLiquidita = i_DatiLiquidita + 1

            Loop

            'Elimino l'oggetto
            ObjLiquidita = Nothing
            ObjSequenze = Nothing
            '------------------------------

            'Elimino tutti gli oggetti utilizzati

            xLiquidita = Nothing
            xLiquiditas = Nothing
            xDatiLiquidita = Nothing
            xDatiLiquiditas = Nothing
            XmlDoc = Nothing

            xRisp = True

            '------------------------------

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            '------------------------------

            'Restituisco un valore Dummy
            Return xRisp

        Catch ex As Exception

            'Restituisco un valore Dummy
            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = ex.Message
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)

        End Try

    End Function

    '#########################################################
    'default di Cod_Istituto è -1 (CODISTITUTO_NOFILTRO)
    Public Function Liquidita_Inserisci(ByVal PIVA As String, _
                                        ByVal Sa_Cod As Integer, _
                                        ByVal Cau_Risorsa As String, _
                                        ByVal Cod_Istituto As Integer, _
                                        ByVal Istituto_Des As String, _
                                        ByVal Interbancario As String, _
                                        ByVal Nazione As String, _
                                        ByVal Cifre_Controllo As String, _
                                         ByVal Cin As String, _
                                        ByVal Abi As String, _
                                        ByVal Cab As String, _
                                         ByVal Numero As String, _
                                         ByVal Bic As String, _
                                         ByVal Riferimento As String, _
                                           ByVal Cod_Contatto As String, _
                                        ByVal Saldo_Attuale As Decimal, _
                                        ByVal Saldo_Iniziale As Decimal, _
                                        ByVal Avviso As Integer, _
                                        ByVal Importo_Avviso As Decimal, _
                                        ByVal Note As String, _
                                        ByVal Rilevamento As Double, _
                                        ByVal Data_Rilevamento As Date, _
                                        ByVal Offset As Double, _
                                        ByVal ChkDefault As Integer, _
                                        ByVal ChkAbilitazione As Integer, _
                                        ByVal Validita_Inizio As Date, _
                                        ByVal Validita_Fine As Date, _
                                        ByVal BaseCode As Integer, _
                                        ByVal TopCode As Integer, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        '----------------------------------------------------------------------

        Dim NomeRoutine As String = "AgronicaCoreContabBIZ.Liquidita_W.Liquidita_Inserisci()"
        Dim MessaggioErrore As String
        '----------------------------------------------------------------------

        Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim ObjLiquidita As New AgronicaCoreContabDAL.Liquidita_W

        Dim xRisp As Boolean
        Dim Cod_Liquidita As Long

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        'se l'IBAN è totalmente vuoto
        'e non c'è nemmeno la banca valorizzata
        '---> non salvo nulla, non c'è l'informazione
        If (Nazione = "" And Cifre_Controllo = "" And Cin = "" And Abi = "" And Cab = "" And Numero = "" And Bic = "") _
            And (Cod_Istituto = CODISTITUTO_NOFILTRO And Istituto_Des = "") Then
            Return False
            Exit Function
        End If


        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale, _
                                                                                    FlagTransazioneLocale, _
                                                                                    objParametri)
            '------------------------------

            If Cod_Istituto = CODISTITUTO_NOFILTRO Then

                Dim objIstCred_R As New AgronicaCoreContabDAL.Ist_Credito_R
                If Istituto_Des <> "" Then
                    'se ho passato il nome della banca, recupero il codice
                    Cod_Istituto = objIstCred_R.Recupera_CodIstituto_byDesc(Istituto_Des, objParametri)
                Else
                    'cerco di recuperare il nome della banca dalla tabella degli abi e cab
                    Dim objCore_AbiCab_R As New AgronicaCoreMetaSchemaDAL.Banche_ABI_CAB_R
                    Istituto_Des = objCore_AbiCab_R.Recupera_IstitutoSportello_byABICAB(Abi, Cab, objParametri)
                    If Istituto_Des = "" Then
                        Istituto_Des = "Istituto N.D."
                    End If
                End If

                If Cod_Istituto = CODISTITUTO_NOFILTRO Then
                    'banca non trovata, la creo fittizia
                    'lo ricavo da sequenza tabella
                    Cod_Istituto = ObjSequenze.NuovoId_Tabella("Ist_Credito", _
                                              BaseCode, _
                                              TopCode, _
                                              objParametri)

                    'inserimento della banca
                    Dim objIstCred_W As New AgronicaCoreContabDAL.Ist_Credito_W
                    Dim risp As Boolean

                    risp = objIstCred_W.Scrivi(0, _
                                            Cod_Istituto, _
                                            Istituto_Des, _
                                            0, _
                                            enum_IstCredito_PerRisorsa.ContoCorrente, _
                                            AGRODATAINIZIO, _
                                            AGRODATAFINE, _
                                            objParametri)

                End If

            Else
                'è stato passato, non occorre fare nulla
            End If


            '------------------------------

            Cod_Liquidita = ObjSequenze.NuovoId_Tabella("Liquidita", _
                                                        BaseCode, _
                                                        TopCode, _
                                                        objParametri)


            xRisp = ObjLiquidita.Scrivi(PIVA, _
                                        Sa_Cod, _
                                        Cod_Liquidita, _
                                        Cod_Contatto, _
                                        Riferimento, _
                                        Cau_Risorsa, _
                                        Cod_Istituto, _
                                        Numero, _
                                        Abi, _
                                        Cab, _
                                        Cin, _
                                        Cifre_Controllo, _
                                        Nazione, _
                                        Bic, _
                                        Interbancario, _
                                        Saldo_Attuale, _
                                        Saldo_Iniziale, _
                                        Avviso, _
                                        Importo_Avviso, _
                                        Note, _
                                        Rilevamento, _
                                        Data_Rilevamento, _
                                        Offset, _
                                        ChkDefault, _
                                        ChkAbilitazione, _
                                        Validita_Inizio, _
                                        Validita_Fine, _
                                        "", objParametri)


            '------------------------------

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            '------------------------------

        Catch ex As Exception

            xRisp = False

            'Faccio il rollback della transazione
            If Not objParametri.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            '//////////////////////////////////////////////////////////////////////
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            '//////////////////////////////////////////////////////////////////////

            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)

        Finally
            'Chiudo la connessione se è stata aperta in questa routine
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return xRisp

    End Function




End Class
