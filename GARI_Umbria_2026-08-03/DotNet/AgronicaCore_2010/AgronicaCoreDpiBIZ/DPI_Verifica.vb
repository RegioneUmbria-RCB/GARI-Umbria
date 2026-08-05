Imports System.Configuration
Imports System.Data
Imports System.Data.Common
Imports System.Data.OleDb
Imports System.Text
Imports System.Web
Imports System.Xml

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello.VerificaDPIMultiAttivita
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL



'DA TERMINARE LA CONVERSIONE   

Public Class DPI_Verifica
    Inherits AgronicaCoreDataProvider.LogProvider

    Protected Elemento_Verifica_Discip As AgronicaCoreModello.Agenda.Util.Elemento_Verifica_Disciplinare

    Private Const ClasseNome = "DPI_Verifica:"          ' #### CLASSE ####

    'Tipi di Mezzo
    Public Enum enTipoMezzo
        Indefinito = -1      'Mezzo per le Materie Prime
        Ettolitro = 0
        Ettaro = 1
        Ora = 2
        Mensile = 3
        Complessivo = 4
    End Enum



    'Definizione Indici Matrice 'Destinazioni'
    Private Const i_Piva As Integer = 0
    Private Const i_Sa_Cod As Integer = 1
    Private Const i_Appezza As Integer = 2
    Private Const i_Id_Reg As Integer = 3
    Private Const i_Lotto As Integer = 4
    Private Const i_Regolamento As Integer = 5
    Private Const i_Cul_Cod As Integer = 6
    Private Const i_Disciplinare_Cod As Integer = 7
    Private Const i_Sup_Trattata As Integer = 8
    Private Const i_Grfi_Cod As Integer = 9
    Private Const i_Stato As Integer = 10
    Private Const i_Foral_Cod As Integer = 11
    Private Const i_Cop_Cod As Integer = 12
    Private Const i_Protetto As Integer = 13
    Private Const i_Id_RcDpi As Integer = 14
    Private Const i_Id_RcDpi_Des As Integer = 15
    Private Const i_Disciplinare_Des As Integer = 16
    Private Const i_FF_Cod As Integer = 17
    Private Const i_Soglia As Integer = 18           'Stato Soglia
    Private Const i_Validita_Inizio As Integer = 19  'Stato Soglia
    Private Const i_Validita_Fine As Integer = 20    'Stato Soglia
    Private Const i_Data_Raccolta As Integer = 21
    Private Const i_ListaComuni As Integer = 22
    Private Const i_Data_Fioritura As Integer = 23
    Private Const i_LunghezzaConfine_BufferZone As Integer = 24
    Private Const i_Sup_Riduzione_BufferZone As Integer = 25
    Private Const i_Perc_Riduzione_Deriva As Integer = 26
    Private Const i_Sup_Imp As Integer = 27
    Private Const i_FF_Stadio As Integer = 28
    Private Const i_Offset_UltimaPianta As Integer = 29 'capezzagna

    'Definizione Indici Matrice 'Dettagli'
    Private Const i_Pro_Cod As Integer = 0
    Private Const i_Av_Cod As Integer = 1
    Private Const i_Av_Gru As Integer = 2
    Private Const i_Udm_Cod As Integer = 3
    Private Const i_Dose As Integer = 4
    Private Const i_for_veg_av_dos_cod As Integer = 5
    Private Const i_formulatixallegatinormative_idriga As Integer = 6
    Private Const i_buffer As Integer = 7
    Private Const i_soglia_cod As Integer = 8
    Private Const i_for_veg_cod As Integer = 9
    Private Const i_Dose_Hl As Integer = 10
    Private Const i_Mezzo_Det As Integer = 11


    '###########################################################################################
    'Public Function DPI_Verifica_Conformita_Intervento(ByRef objParametri_Server As AgronicaCoreParametri,
    '                                                   ByRef objParametri_Utenti As AgronicaCoreParametri,
    '                                                   ByVal DatiAgenda As String,
    '                                                   ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
    '                                                   Optional ByVal Id_Agenda_Escluso As Integer = 0,
    '                                                   Optional ByVal Piva_Riferimento As String = "",
    '                                                   Optional ByVal Sa_Cod_Riferimento As Integer = 0,
    '                                                   Optional ByVal Appezza_Riferimento As Integer = 0,
    '                                                   Optional ByVal Id_Reg_Riferimento As Integer = 0,
    '                                                   Optional ByVal Disciplinare_Cod As Integer = 0) As String


    '    Dim XmlDoc As New XmlDocument

    '    Dim Lav_Cod As Integer

    '    Dim xDatiAgende As XmlNodeList
    '    Dim xDatiAgenda As XmlElement
    '    Dim xAgende As XmlNodeList
    '    Dim xAgenda As XmlElement

    '    Dim i_DatiAgenda As Integer
    '    Dim i_Agenda As Integer

    '    Dim DatiWS As String


    '    '------------------------------

    '    Try

    '        XmlDoc.LoadXml(DatiAgenda)

    '        '############################################################################################
    '        '################# Lettura dei Parametri Base di Agenda     #################################
    '        '############################################################################################

    '        xDatiAgende = XmlDoc.GetElementsByTagName("DatiAgenda")

    '        i_DatiAgenda = 0

    '        Do While i_DatiAgenda < xDatiAgende.Count

    '            'Prelevo l'i-esimo blocco di DatiAgenda (in realtà ne esiste uno solo)
    '            xDatiAgenda = xDatiAgende.Item(i_DatiAgenda)


    '            xAgende = xDatiAgenda.GetElementsByTagName("Agenda")

    '            i_Agenda = 0

    '            Do While i_Agenda < xAgende.Count

    '                'Prelevo l' i-esima Codifica Agenda
    '                xAgenda = xAgende.Item(i_Agenda)
    '                Lav_Cod = CInt(xAgenda.GetAttribute("lav_cod"))


    '                Select Case Lav_Cod

    '                    Case LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO, LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_GEODISINFESTAZIONE, LAVCOD_CONCIA_SEME, LAVCOD_TRATTAMENTO_FITOREGOLATORE

    '                        DPI_Verifica_Conformita_Intervento = DPI_Verifica_DifesaDiserbo(objParametri_Server,
    '                                                                                        objParametri_Utenti,
    '                                                                                        DatiAgenda,
    '                                                                                        Id_Agenda_Escluso,
    '                                                                                        VerificaSoloControlliImpostazioniUtente,
    '                                                                                        Disciplinare_Cod,
    '                                                                                        Piva_Riferimento,
    '                                                                                        Sa_Cod_Riferimento,
    '                                                                                        Appezza_Riferimento,
    '                                                                                        Id_Reg_Riferimento)


    '                    Case LAVCOD_TRATTAMENTO_ANTIBUTTERATURA, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_FERTIRRIGAZIONE, LAVCOD_SARCHIATURA_CONCIMAZIONE

    '                        DPI_Verifica_Conformita_Intervento = DPI_Verifica_Concimazione(objParametri_Server,
    '                                                                                       objParametri_Utenti,
    '                                                                                       DatiAgenda,
    '                                                                                        Id_Agenda_Escluso,
    '                                                                                        VerificaSoloControlliImpostazioniUtente,
    '                                                                                       Disciplinare_Cod,
    '                                                                                       Piva_Riferimento,
    '                                                                                        Sa_Cod_Riferimento,
    '                                                                                        Appezza_Riferimento,
    '                                                                                        Id_Reg_Riferimento)


    '                    Case 125 'Raccolta

    '                        DPI_Verifica_Conformita_Intervento = DPI_Verifica_Raccolta(objParametri_Server,
    '                                                                                   objParametri_Utenti,
    '                                                                                    DatiAgenda,
    '                                                                                    Id_Agenda_Escluso,
    '                                                                                    VerificaSoloControlliImpostazioniUtente,
    '                                                                                   Piva_Riferimento,
    '                                                                                    Sa_Cod_Riferimento,
    '                                                                                    Appezza_Riferimento,
    '                                                                                    Id_Reg_Riferimento)


    '                    Case Else 'Lavorazione non verificabile

    '                        DPI_Verifica_Conformita_Intervento = "0" 'Dummy

    '                End Select



    '                i_Agenda = i_Agenda + 1


    '            Loop

    '            i_DatiAgenda = i_DatiAgenda + 1

    '        Loop

    '        '------------------------------

    '    Catch exc As Exception

    '        '------------------------------------------------
    '        'Si e' verificata una eccezione !!!!!!
    '        '------------------------------------------------

    '        Dim StrDummy As String

    '        'Messaggio di errore
    '        StrDummy = exc.Message.ToString()

    '        'Stampo il messaggio sul client
    '        'AgroMsgBox("Si è verificato un errore durante la Verifica di Conformità : " & StrDummy, objPage)

    '        Return ""
    '        '------------------------------------------------

    '    End Try




    'End Function

    '###########################################################################################
    'Public Function Verifica_Conformita_Intervento(ByRef objParametri_Server As AgronicaCoreParametri,
    '                                                   ByRef objParametri_Utenti As AgronicaCoreParametri,
    '                                                   ByVal DatiAgenda As String,
    '                                                   ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
    '                                                    Optional ByVal Id_Agenda_Escluso As Integer = 0,
    '                                                   Optional ByVal Piva_Riferimento As String = "",
    '                                                   Optional ByVal Sa_Cod_Riferimento As Integer = 0,
    '                                                   Optional ByVal Appezza_Riferimento As Integer = 0,
    '                                                   Optional ByVal Id_Reg_Riferimento As Integer = 0,
    '                                                   Optional ByVal Biologico As Boolean = False) As String


    '    Dim XmlDoc As New XmlDocument

    '    Dim Lav_Cod As Integer

    '    Dim xDatiAgende As XmlNodeList
    '    Dim xDatiAgenda As XmlElement
    '    Dim xAgende As XmlNodeList
    '    Dim xAgenda As XmlElement

    '    Dim i_DatiAgenda As Integer
    '    Dim i_Agenda As Integer

    '    Dim DatiWS As String


    '    '------------------------------

    '    Try

    '        XmlDoc.LoadXml(DatiAgenda)

    '        '############################################################################################
    '        '################# Lettura dei Parametri Base di Agenda     #################################
    '        '############################################################################################

    '        xDatiAgende = XmlDoc.GetElementsByTagName("DatiAgenda")

    '        i_DatiAgenda = 0

    '        Do While i_DatiAgenda < xDatiAgende.Count

    '            'Prelevo l'i-esimo blocco di DatiAgenda (in realtà ne esiste uno solo)
    '            xDatiAgenda = xDatiAgende.Item(i_DatiAgenda)


    '            xAgende = xDatiAgenda.GetElementsByTagName("Agenda")

    '            i_Agenda = 0

    '            Do While i_Agenda < xAgende.Count

    '                'Prelevo l' i-esima Codifica Agenda
    '                xAgenda = xAgende.Item(i_Agenda)
    '                Lav_Cod = CInt(xAgenda.GetAttribute("lav_cod"))


    '                Select Case Lav_Cod

    '                    Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
    '                                    LAVCOD_TRATTAMENTO_FITOREGOLATORE,
    '                                    LAVCOD_CONCIA_SEME,
    '                                    LAVCOD_GEODISINFESTAZIONE,
    '                                    LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO

    '                        Verifica_Conformita_Intervento = Verifica_DifesaDiserbo(objParametri_Server,
    '                                                                                        objParametri_Utenti,
    '                                                                                        DatiAgenda,
    '                                                                                        Id_Agenda_Escluso,
    '                                                                                        VerificaSoloControlliImpostazioniUtente,
    '                                                                                        Piva_Riferimento,
    '                                                                                        Sa_Cod_Riferimento,
    '                                                                                        Appezza_Riferimento,
    '                                                                                        Id_Reg_Riferimento,
    '                                                                                        Biologico)

    '                    Case 106, 123, 124, 14, 26, 156 'TRATTAMENTO ANTIBUTTERATURA, CONCIMAZIONE FOGLIARE,
    '                        'DISTRIBUZIONE AMMENDANTI ORGANICI, DISTRIBUZIONE CONCIME IN PIENO CAMPO,
    '                        'FERTIRRIGAZIONE, SARCHIATURA

    '                        Verifica_Conformita_Intervento = DPI_Verifica_Concimazione(objParametri_Server,
    '                                                                                   objParametri_Utenti,
    '                                                                                   DatiAgenda,
    '                                                                                   Id_Agenda_Escluso,
    '                                                                                   VerificaSoloControlliImpostazioniUtente,
    '                                                                                   0,
    '                                                                                   Piva_Riferimento,
    '                                                                                    Sa_Cod_Riferimento,
    '                                                                                    Appezza_Riferimento,
    '                                                                                    Id_Reg_Riferimento)

    '                    Case 125 'Raccolta

    '                        Verifica_Conformita_Intervento = DPI_Verifica_Raccolta(objParametri_Server,
    '                                                                                   objParametri_Utenti,
    '                                                                                    DatiAgenda,
    '                                                                                    Id_Agenda_Escluso,
    '                                                                                    VerificaSoloControlliImpostazioniUtente,
    '                                                                                    Piva_Riferimento,
    '                                                                                    Sa_Cod_Riferimento,
    '                                                                                    Appezza_Riferimento,
    '                                                                                    Id_Reg_Riferimento)

    '                    Case Else 'Lavorazione non verificabile

    '                        Verifica_Conformita_Intervento = "0" 'Dummy

    '                End Select



    '                i_Agenda = i_Agenda + 1


    '            Loop

    '            i_DatiAgenda = i_DatiAgenda + 1

    '        Loop

    '        '------------------------------

    '    Catch exc As Exception

    '        '------------------------------------------------
    '        'Si e' verificata una eccezione !!!!!!
    '        '------------------------------------------------

    '        Dim StrDummy As String

    '        'Messaggio di errore
    '        StrDummy = exc.Message.ToString()

    '        'Stampo il messaggio sul client
    '        'AgroMsgBox("Si è verificato un errore durante la Verifica di Conformità : " & StrDummy, objPage)

    '        Return ""
    '        '------------------------------------------------

    '    End Try




    'End Function

    ''' <param name="TotCU_Trattamenti_MultiAttivita_xDistinta"> ((PIVA, SA_COD, APPEZZA, ID_REG), CU_xDISTINTA)
    ''' MI SALVO IL TOT DI RAME USATO NEI TRATTAMENTI X OGNI DISTINTA --> DA PASSARE AL VERIFICA CONCIMAZIONE
    ''' </param>
    ''' <param name="TotCU_Fertilizzazione_MultiAttivita_xDistinta"> ((PIVA, SA_COD, APPEZZA, ID_REG), CU_xDISTINTA)
    ''' MI SALVO IL TOT DI RAME USATO NELLE CONCIMAZIONI X OGNI DISTINTA --> DA PASSARE AL VERIFICA TRATTAMENTI
    ''' </param>
    ''' <param name="dic_CUTrattamenti_xDistintaxLavCod"> ((PIVA, SA_COD, APPEZZA, ID_REG), LAV_COD), CU_xDISTINTAxLAV_COD)
    ''' MI SALVO IL TOT DI RAME USATO NELLE CONCIMAZIONI X OGNI DISTINTA X OGNI LAV_COD --> DA PASSARE AL VERIFICA TRATTAMENTI, COSI' DA POTER CONTARE I MULTI TRATTAMENTI
    ''' </param>
    Public Function Verifica_Conformita_Intervento_New(ByRef objParametri_Server As AgronicaCoreParametri,
                                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                       ByVal Piva As String,
                                                       ByVal DatiAgenda As String,
                                                       ByVal Id_Agenda_DaAnalizzare As Integer,
                                                       ByVal Id_Agenda_Escluso As Integer,
                                                       ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
                                                       ByVal Disciplinare_Cod As String,
                                                       ByVal Disciplinare_PubblicoPrivato As String,
                                                       ByVal Piva_Riferimento As String,
                                                       ByVal Sa_Cod_Riferimento As Integer,
                                                       ByVal Appezza_Riferimento As Integer,
                                                       ByVal Id_Reg_Riferimento As Integer,
                                                       Optional ByVal DTCarenze As DataTable = Nothing,
                                                       Optional ByVal dtDosiVuoto As Boolean = False,
                                                       Optional ByVal objParametri_Super_Server As AgronicaCoreParametri = Nothing,
                                                       Optional isFromAgendaNG As Boolean = False,
                                                       Optional paramVerificaDPIMultiAttivita As AgronicaCoreModello.VerificaDPIMultiAttivita = Nothing
                                                       ) As rispostaStandard(Of Verifica_Disciplinare_Intervento)

        Dim r As New rispostaStandard(Of Verifica_Disciplinare_Intervento)
        r.RispostaStringa = New Verifica_Disciplinare_Intervento

        Dim Verifica_Conformita_Intervento As String = ""

        Dim XmlDoc As New XmlDocument

        Dim Lav_Cod As Integer = 0

        Dim xDatiAgende As XmlNodeList
        Dim xDatiAgenda As XmlElement
        Dim xAgende As XmlNodeList
        Dim xAgenda As XmlElement

        Dim i_DatiAgenda As Integer
        Dim i_Agenda As Integer

        Try

            If DatiAgenda = "" AndAlso Id_Agenda_DaAnalizzare = 0 Then
                Throw New Exception(Gias.IndicareIntervento)
            End If

            If DatiAgenda = "" Then

                Dim objAgenda As New AgronicaCoreContabBIZ.Agenda_R
                DatiAgenda = objAgenda.Agenda_Leggi(Piva,
                                                    0,
                                                    Id_Agenda_DaAnalizzare,
                                                    0,
                                                    False,
                                                    objParametri_Server)

            End If


            If DatiAgenda <> "" Then

                XmlDoc.LoadXml(DatiAgenda)

                xDatiAgende = XmlDoc.GetElementsByTagName("DatiAgenda")

                i_DatiAgenda = 0

                Do While i_DatiAgenda < xDatiAgende.Count

                    xDatiAgenda = xDatiAgende.Item(i_DatiAgenda)

                    xAgende = xDatiAgenda.GetElementsByTagName("Agenda")

                    i_Agenda = 0

                    Do While i_Agenda < xAgende.Count

                        'Prelevo l' i-esima Codifica Agenda
                        xAgenda = xAgende.Item(i_Agenda)
                        Lav_Cod = CInt(xAgenda.GetAttribute("lav_cod"))

                        Select Case Lav_Cod

                            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                                 LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                                 LAVCOD_CONCIA_SEME,
                                 LAVCOD_GEODISINFESTAZIONE,
                                 LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO,
                                 LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                                 LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA,
                                 LAVCOD_REINNESCO_TRAPPOLE

                                Dim rVal As New rispostaStandard(Of Verifica_Disciplinare_Intervento)
                                rVal = Verifica_DifesaDiserbo_New(objParametri_Server,
                                                                  objParametri_Utenti,
                                                                  DatiAgenda,
                                                                  Id_Agenda_Escluso,
                                                                  VerificaSoloControlliImpostazioniUtente,
                                                                  Disciplinare_Cod,
                                                                  Disciplinare_PubblicoPrivato,
                                                                  Piva_Riferimento,
                                                                  Sa_Cod_Riferimento,
                                                                  Appezza_Riferimento,
                                                                  Id_Reg_Riferimento, dtDosiVuoto,
                                                                  objParametri_Super_Server,
                                                                  paramVerificaDPIMultiAttivita:=paramVerificaDPIMultiAttivita)

                                If rVal.RispostaOK Then
                                    Verifica_Conformita_Intervento = rVal.RispostaStringa.Risultato
                                Else
                                    Throw New Exception(rVal.Errore)
                                End If

                            Case LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                                 LAVCOD_CONCIMAZIONE_FOGLIARE,
                                 LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                                 LAVCOD_DISTRIBUZIONE_CONCIME,
                                 LAVCOD_FERTIRRIGAZIONE,
                                 LAVCOD_SARCHIATURA_CONCIMAZIONE

                                Dim rVal As New rispostaStandard(Of Verifica_Disciplinare_Intervento)
                                rVal = DPI_Verifica_Concimazione(objParametri_Server,
                                                                 objParametri_Utenti,
                                                                 DatiAgenda,
                                                                 Id_Agenda_Escluso,
                                                                 Disciplinare_Cod,
                                                                 VerificaSoloControlliImpostazioniUtente,
                                                                 Piva_Riferimento,
                                                                 Sa_Cod_Riferimento,
                                                                 Appezza_Riferimento,
                                                                 Id_Reg_Riferimento,
                                                                 objParametri_Super_Server:=objParametri_Super_Server,
                                                                 paramVerificaDPIMultiAttivita:=paramVerificaDPIMultiAttivita)

                                If rVal.RispostaOK Then
                                    Verifica_Conformita_Intervento = rVal.RispostaStringa.Risultato
                                Else
                                    Throw New Exception(rVal.Errore)
                                End If

                            Case LAVCOD_RACCOLTA

                                Dim rVal As New rispostaStandard(Of Verifica_Disciplinare_Intervento)
                                rVal = DPI_Verifica_Raccolta(objParametri_Server,
                                                                objParametri_Utenti,
                                                                DatiAgenda,
                                                                Id_Agenda_Escluso,
                                                                VerificaSoloControlliImpostazioniUtente,
                                                                Piva_Riferimento,
                                                                Sa_Cod_Riferimento,
                                                                Appezza_Riferimento,
                                                                Id_Reg_Riferimento,
                                                                DTCarenze,
                                                                isFromAgendaNG:=isFromAgendaNG)

                                If rVal.RispostaOK Then
                                    Verifica_Conformita_Intervento = rVal.RispostaStringa.Risultato
                                Else
                                    Throw New Exception(rVal.Errore)
                                End If

                            Case Else 'Lavorazione non verificabile

                                Verifica_Conformita_Intervento = "" 'Dummy

                        End Select



                        i_Agenda = i_Agenda + 1


                    Loop

                    i_DatiAgenda = i_DatiAgenda + 1

                Loop

                '------------------------------

            End If

            r.RispostaOK = True

            r.RispostaStringa.Risultato = Verifica_Conformita_Intervento

            Dim strNonConformita As String = ""
            If Verifica_Conformita_Intervento <> "" Then
                r.RispostaStringa.Conforme = VerificaNonConformita(Verifica_Conformita_Intervento, strNonConformita)
                r.RispostaStringa.strNonConformita = strNonConformita
            End If

        Catch exc As Exception

            r.RispostaOK = False
            r.Errore = exc.Message.ToString()

        End Try

        Return r

    End Function

    Private Function VerificaNonConformita(ByVal RisultatoVerifica As String, ByRef strErrore As String) As Boolean

        '------------------------------------------
        '----- Dichiarazione delle Variabili
        '------------------------------------------

        Dim Conforme As Boolean = True

        Dim XmlDoc As New XmlDocument
        Dim XML_DatiRisultati As XmlElement
        Dim XML_DatiGenerali As XmlElement
        Dim XML_DatiNonConformi As XmlElement
        Dim XML_DatoNonConforme As XmlElement
        Dim XMLs_DatoNonConforme As XmlNodeList

        Dim x As Integer = 0

        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(RisultatoVerifica)

        '----- Elemento <DatiRisultati>
        XML_DatiRisultati = XmlDoc.SelectSingleNode("DatiRisultati")

        '----- Elemento <DatiGenerali>
        XML_DatiGenerali = XML_DatiRisultati.SelectSingleNode("DatiGenerali")

        '----- Elemento <DatiNonConformi>
        XML_DatiNonConformi = XML_DatiGenerali.SelectSingleNode("DatiNonConformi")

        XMLs_DatoNonConforme = XML_DatiNonConformi.GetElementsByTagName("DatoNonConforme")

        If Not XML_DatiNonConformi.HasChildNodes Then
            Conforme = True
        Else
            Conforme = False
        End If

        '------------------------------------------
        '----- Data Grid Dettagli
        '------------------------------------------
        For x = 0 To XMLs_DatoNonConforme.Count - 1
            XML_DatoNonConforme = XMLs_DatoNonConforme.Item(x)
            strErrore &= CStr(XML_DatoNonConforme.GetAttribute("err_des")) & "<br>"
        Next

        Return Conforme

    End Function


    '###########################################################################################
    'Public Function DPI_Verifica_Conformita_Impianto(ByRef objParametri_Server As AgronicaCoreParametri,
    '                                                ByRef objParametri_Utenti As AgronicaCoreParametri,
    '                                                ByVal Piva As String,
    '                                                ByVal Sa_Cod As Integer,
    '                                                ByVal Appezza As Integer,
    '                                                ByVal Id_Reg As Integer,
    '                                                ByVal TipoTestata As Integer,
    '                                                   ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
    '                                                        ByRef strRisultato As String,
    '                                                            Optional ByVal Disciplinare_Cod As Integer = 0,
    '                                                            Optional ByVal Validita_Inizio As Date = #1/1/1900#,
    '                                                            Optional ByVal Validita_Fine As Date = #12/31/2100#) As Integer




    '    Dim DatiAgenda As String
    '    Dim Risultato As String = ""
    '    Dim i As Integer

    '    strRisultato = ""

    '    Try

    '        DPI_Verifica_Conformita_Impianto = 0

    '        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Validita_Inizio, Validita_Fine)

    '        Dim ObjAgende As New AgronicaCoreContabDAL.Mov_Destinazioni_R 'Object
    '        Dim ObjAgenda As New AgronicaCoreContabBIZ.Agenda_R 'Object
    '        Dim DtAgende As DataTable
    '        Dim Dr() As DataRow

    '        DtAgende = ObjAgende.LeggiCronologiaMovimenti(CStr(Piva),
    '                                                      CInt(Sa_Cod),
    '                                                      CInt(Appezza),
    '                                                      CInt(Id_Reg),
    '                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
    '                                                      "",
    '                                                      "",
    '                                                      objParametri_Server)

    '        If DtAgende.Rows.Count > 0 Then

    '            'Filtro gli interventi in base alle norme di intervento
    '            Select Case TipoTestata
    '                Case 0 'Difesa        'Trattamenti Antiparassitari o Concia o Geodisinfestazione
    '                    Dr = DtAgende.Select("Lav_Cod = 74 or Lav_Cod = 13 or Lav_Cod = 155")
    '                Case 1 'Diserbo        'Diserbo o Disseccamento
    '                    Dr = DtAgende.Select("Lav_Cod = 18 or Lav_Cod = 158")
    '                Case 10 'Limitazioni Generali
    '                    Dr = DtAgende.Select("Lav_Cod = 125")
    '                Case Else
    '                    'Eccezione
    '                    Dr = DtAgende.Select("Lav_Cod = -1001")   'Uscita morbida
    '            End Select


    '            For i = 0 To Dr.Length - 1

    '                'If DPI_Verifica_Conformita_Impianto = 0 Then

    '                DatiAgenda = ObjAgenda.Agenda_Leggi(CStr(Piva),
    '                                                    CInt(Sa_Cod),
    '                                                    CInt(Dr(i).Item("Id_Agenda")),
    '                                                    0,
    '                                                    False,
    '                                                    objParametri_Server)

    '                Select Case TipoTestata

    '                    Case 0, 1


    '                        'Dim rval As RispostaStandard =Verifica_DifesaDiserbo_New(objParametri_Server,
    '                        '                                   objParametri_Utenti,
    '                        '                                   CStr(DatiAgenda),
    '                        '                                   CInt(Dr(i).Item("Id_Agenda")),
    '                        '                                   VerificaSoloControlliImpostazioniUtente,
    '                        '                                   CInt(Disciplinare_Cod),
    '                        '                                   CStr(Piva),
    '                        '                                   CInt(Sa_Cod),
    '                        '                                   CInt(Appezza),
    '                        '                                   CInt(Id_Reg) )

    '                        Dim rval As RispostaStandard = Verifica_DifesaDiserbo_New(objParametri_Server,
    '                                                                                            objParametri_Utenti,
    '                                                                                            DatiAgenda,
    '                                                                                            CInt(Dr(i).Item("Id_Agenda")),
    '                                                                                            VerificaSoloControlliImpostazioniUtente,
    '                                                                                            Disciplinare_Cod,
    '                                                                                            CStr(Piva),
    '                                                                                            CInt(Sa_Cod),
    '                                                                                            CInt(Appezza),
    '                                                                                            CInt(Id_Reg))
    '                        If rval.RispostaOK = True Then
    '                            Risultato = rval.RispostaStringa
    '                        End If

    '                    Case Else

    '                        Risultato = DPI_Verifica_Raccolta(objParametri_Server,
    '                                                          objParametri_Utenti,
    '                                                          DatiAgenda,
    '                                                          CInt(Dr(i).Item("Id_Agenda")),
    '                                                          VerificaSoloControlliImpostazioniUtente,
    '                                                          Piva,
    '                                                          Sa_Cod,
    '                                                          Appezza,
    '                                                          Id_Reg)

    '                End Select

    '                '========================================================================================
    '                'Verifica criterio di arresto
    '                '----------------------------------------------------------------------------------------
    '                If InStr(1, Risultato, "err_code") > 0 Then
    '                    DPI_Verifica_Conformita_Impianto = DPI_Verifica_Conformita_Impianto + 1
    '                    Risultato = Risultato.Replace("<DatiRisultati>", "")
    '                    Risultato = Risultato.Replace("</DatiRisultati>", "")
    '                    strRisultato = strRisultato & Risultato
    '                End If
    '                'End If
    '            Next
    '        End If

    '        If strRisultato <> "" Then
    '            strRisultato = "<DatiRisultati>" & strRisultato & " </DatiRisultati>"
    '        End If
    '        'strRisultato = Risultato

    '        ObjAgende = Nothing
    '        ObjAgenda = Nothing
    '        DtAgende = Nothing


    '    Catch exc As Exception

    '        '------------------------------------------------
    '        'Si e' verificata una eccezione !!!!!!
    '        '------------------------------------------------

    '        Dim StrDummy As String

    '        'Messaggio di errore
    '        StrDummy = exc.Message.ToString()

    '        Scrivi_LOG(objParametri_Server, "DPI_Verifica_Conformita_Impianto", StrDummy)

    '        'Stampo il messaggio sul client
    '        'AgroMsgBox("Si è verificato un errore durante la Verifica di Conformità : " & StrDummy, objPage)
    '        Return ""

    '    End Try


    'End Function


    ''###########################################################################################
    'Public Function Verifica_Conformita_Impianto(ByRef objParametri_Server As AgronicaCoreParametri,
    '                                                ByRef objParametri_Utenti As AgronicaCoreParametri,
    '                                                ByVal Piva As String,
    '                                                ByVal Sa_Cod As Integer,
    '                                                ByVal Appezza As Integer,
    '                                                ByVal Id_Reg As Integer,
    '                                                ByVal TipoTestata As Integer,
    '                                                ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
    '                                                ByRef strRisultato As String,
    '                                                Optional ByVal Biologico As Boolean = False,
    '                                                Optional ByVal Validita_Inizio As Date = #1/1/1900#,
    '                                                Optional ByVal Validita_Fine As Date = #12/31/2100#) As Integer




    '    Dim DatiAgenda As String
    '    Dim Risultato As String = ""
    '    Dim i As Integer

    '    Try

    '        Verifica_Conformita_Impianto = 0

    '        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Validita_Inizio, Validita_Fine)

    '        Dim ObjAgende As New AgronicaCoreContabDAL.Mov_Destinazioni_R 'Object
    '        Dim ObjAgenda As New AgronicaCoreContabBIZ.Agenda_R 'Object
    '        Dim DtAgende As DataTable
    '        Dim Dr() As DataRow

    '        DtAgende = ObjAgende.LeggiCronologiaMovimenti(CStr(Piva),
    '                                                      CInt(Sa_Cod),
    '                                                      CInt(Appezza),
    '                                                      CInt(Id_Reg),
    '                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
    '                                                      "",
    '                                                      "",
    '                                                      objParametri_Server)

    '        If DtAgende.Rows.Count > 0 Then

    '            'Filtro gli interventi in base alle norme di intervento
    '            Select Case TipoTestata
    '                Case 0 'Difesa        'Trattamenti Antiparassitari o Concia o Geodisinfestazione
    '                    Dr = DtAgende.Select("Lav_Cod = 74 or Lav_Cod = 13 or Lav_Cod = 155")
    '                Case 1 'Diserbo        'Diserbo o Disseccamento
    '                    Dr = DtAgende.Select("Lav_Cod = 18 or Lav_Cod = 158")
    '                Case 10 'Limitazioni Generali
    '                    Dr = DtAgende.Select("Lav_Cod = 125")
    '                Case Else
    '                    'Eccezione
    '                    Dr = DtAgende.Select("Lav_Cod = -1001")   'Uscita morbida
    '            End Select


    '            For i = 0 To Dr.Length - 1

    '                If Verifica_Conformita_Impianto = 0 Then

    '                    DatiAgenda = ObjAgenda.Agenda_Leggi(CStr(Piva),
    '                                                        CInt(Sa_Cod),
    '                                                        CInt(Dr(i).Item("Id_Agenda")),
    '                                                        0,
    '                                                        False,
    '                                                        objParametri_Server)

    '                    Select Case TipoTestata

    '                        Case 0, 1

    '                            Dim Disciplinare_Cod As String = "0" 'verificare BIOLOGICO!!!!!
    '                            Dim rval As RispostaStandard = Verifica_DifesaDiserbo_New(objParametri_Server,
    '                                                                                            objParametri_Utenti,
    '                                                                                            DatiAgenda,
    '                                                                                            CInt(Dr(i).Item("Id_Agenda")),
    '                                                                                            VerificaSoloControlliImpostazioniUtente,
    '                                                                                            Disciplinare_Cod,
    '                                                                                            CStr(Piva),
    '                                                                                            CInt(Sa_Cod),
    '                                                                                            CInt(Appezza),
    '                                                                                            CInt(Id_Reg))
    '                            'Risultato = Verifica_DifesaDiserbo(objParametri_Server,
    '                            '                                       objParametri_Utenti,
    '                            '                                       CStr(DatiAgenda),
    '                            '                                       CInt(Dr(i).Item("Id_Agenda")),
    '                            '                                       VerificaSoloControlliImpostazioniUtente,
    '                            '                                       CStr(Piva),
    '                            '                                       CInt(Sa_Cod),
    '                            '                                       CInt(Appezza),
    '                            '                                       CInt(Id_Reg),
    '                            '                                       Biologico)
    '                        Case Else

    '                            Risultato = DPI_Verifica_Raccolta(objParametri_Server,
    '                                                              objParametri_Utenti,
    '                                                              DatiAgenda,
    '                                                              CInt(Dr(i).Item("Id_Agenda")),
    '                                                              VerificaSoloControlliImpostazioniUtente,
    '                                                              Piva,
    '                                                              Sa_Cod,
    '                                                              Appezza,
    '                                                              Id_Reg)

    '                    End Select

    '                    '========================================================================================
    '                    'Verifica criterio di arresto
    '                    '----------------------------------------------------------------------------------------
    '                    If InStr(1, Risultato, "err_code") > 0 Then

    '                        Verifica_Conformita_Impianto = 1

    '                    End If


    '                End If

    '            Next

    '        End If

    '        strRisultato = Risultato

    '        ObjAgende = Nothing
    '        ObjAgenda = Nothing
    '        DtAgende = Nothing


    '    Catch exc As Exception

    '        '------------------------------------------------
    '        'Si e' verificata una eccezione !!!!!!
    '        '------------------------------------------------

    '        Dim StrDummy As String

    '        'Messaggio di errore
    '        StrDummy = exc.Message.ToString()

    '        Scrivi_LOG(objParametri_Server, "Verifica_Conformita_Impianto", StrDummy)

    '        'Stampo il messaggio sul client
    '        'AgroMsgBox("Si è verificato un errore durante la Verifica di Conformità : " & StrDummy, objPage)
    '        Return ""

    '    End Try


    'End Function


    Public Function Verifica_Conformita_Impianto_New(ByRef objParametri_Server As AgronicaCoreParametri,
                                                     ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                     ByVal Piva As String,
                                                     ByVal Sa_Cod As Integer,
                                                     ByVal Appezza As Integer,
                                                     ByVal Id_Reg As Integer,
                                                     ByVal Disciplinare_Cod As String,
                                                     ByVal Disciplinare_PubblicoPrivato As String,
                                                     ByVal TipoTestata As Integer,
                                                     ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
                                                     ByVal Validita_Inizio As Date,
                                                     ByVal Validita_Fine As Date
                                                     ) As rispostaStandard(Of Verifica_Disciplinare_Impianto)


        Dim r As New rispostaStandard(Of Verifica_Disciplinare_Impianto)
        r.RispostaStringa = New Verifica_Disciplinare_Impianto

        Dim Risultato As String = ""
        Dim i As Integer

        Dim Num_Interventi As Integer = 0
        Dim Num_InterventiSoggetti As Integer = 0
        Dim Num_Interventi_Conformi As Integer = 0
        Dim Num_Interventi_Non_Conformi As Integer = 0

        Dim strRisultato As String = 0

        Try

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Validita_Inizio, Validita_Fine)

            Dim ObjAgende As New AgronicaCoreContabDAL.Mov_Destinazioni_R 'Object
            Dim DtAgende As DataTable

            DtAgende = ObjAgende.LeggiCronologiaMovimenti(CStr(Piva),
                                                          CInt(Sa_Cod),
                                                          CInt(Appezza),
                                                          CInt(Id_Reg),
                                                          enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                          "",
                                                          "",
                                                          objParametri_Server)

            objParametri_Server.ResettaFinestra()

            If DtAgende.Rows.Count > 0 Then

                Num_Interventi = DtAgende.Rows.Count

                'Num_InterventiSoggetti = Dr.Length

                'For i = 0 To Dr.Length - 1
                For i = 0 To DtAgende.Rows.Count - 1

                    Dim objDpiVerifica As New AgronicaCoreDpiBIZ.DPI_Verifica
                    Dim rVal As New rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento)
                    rVal = objDpiVerifica.Verifica_Conformita_Intervento_New(objParametri_Server, objParametri_Utenti,
                                                                             Piva,
                                                                             "", DtAgende.Rows(i).Item("id_agenda"),
                                                                             DtAgende.Rows(i).Item("id_agenda"),
                                                                             VerificaSoloControlliImpostazioniUtente,
                                                                             enum_Disciplinare_Operazione.QuelloDellOperazione, 0,
                                                                             "", 0, 0, 0)

                    If rVal.RispostaOK Then

                        Risultato = rVal.RispostaStringa.Risultato

                        If Risultato <> "" Then

                            If InStr(1, Risultato, "err_code") > 0 Then
                                Risultato = Risultato.Replace("<DatiRisultati>", "")
                                Risultato = Risultato.Replace("</DatiRisultati>", "")
                                strRisultato = strRisultato & Risultato
                            End If

                            Num_InterventiSoggetti += 1

                            If rVal.RispostaStringa.Conforme Then
                                Num_Interventi_Conformi += 1
                            Else
                                Num_Interventi_Non_Conformi += 1
                            End If

                        End If

                    Else
                        Throw New Exception(rVal.Errore)
                    End If

                Next

            End If

            If strRisultato <> "" Then
                strRisultato = "<DatiRisultati>" & strRisultato & " </DatiRisultati>"
            End If


            r.RispostaOK = True
            r.RispostaStringa.Risultato = strRisultato

            If Num_Interventi_Non_Conformi > 0 Then
                r.RispostaStringa.Conforme = False
            Else
                r.RispostaStringa.Conforme = True
            End If

            r.RispostaStringa.Num_Interventi = Num_Interventi
            r.RispostaStringa.Num_InterventiSoggetti = Num_InterventiSoggetti
            r.RispostaStringa.Num_Interventi_Conformi = Num_Interventi_Conformi
            r.RispostaStringa.Num_Interventi_Non_Conformi = Num_Interventi_Non_Conformi


        Catch exc As Exception

            '------------------------------------------------
            'Si è verificata una eccezione !!!!!!
            '------------------------------------------------

            Dim StrDummy As String

            'Messaggio di errore
            StrDummy = exc.Message.ToString()

            Scrivi_LOG(objParametri_Server, "Verifica_Conformita_Impianto_New", StrDummy)

            r.RispostaOK = False
            r.Errore = StrDummy

            Return r

        End Try

    End Function



    '============================================================================
    'Public Function DPI_Verifica_DifesaDiserbo(ByRef objParametri_Server As AgronicaCoreParametri,
    '                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
    '                                           ByVal DatiAgenda As String,
    '                                           ByVal Id_Agenda_Escluso As Integer,
    '                                           ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
    '                                           Optional ByVal Disciplinare_Cod As Integer = 0,
    '                                           Optional ByVal Piva_Riferimento As String = "",
    '                                           Optional ByVal Sa_Cod_Riferimento As Integer = 0,
    '                                           Optional ByVal Appezza_Riferimento As Integer = 0,
    '                                           Optional ByVal Id_Reg_Riferimento As Integer = 0) As String




    '    Dim DatiWS As String
    '    Dim DatiVerifica As String

    '    Dim ASG_Utente_Username_Crypt As String
    '    Dim ASG_Utente_Password_Crypt As String

    '    Try


    '        Dim Livello As Integer
    '        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
    '        Dim DT As DataTable
    '        DT = objUtentiImpostazioni.Leggi(0, 1, enumSelezioneVariabile.Selezione_TabellaCompleta,
    '                                          "", "", objParametri_Utenti)
    '        Dim DR() As DataRow

    '        DR = DT.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_COD_LIVELLO_CHK_DPI)
    '        If DR.Length > 0 Then
    '            '1 = Livello Base
    '            '2 = Livello Avanzato
    '            Livello = DR(0).Item("Impostazione_Valore_1")
    '        Else
    '            'se non ho il livello impostato utilizzo quello di base 
    '            Livello = 1
    '        End If

    '        '######################################################################################################
    '        '################################ DATI WS #############################################################
    '        '######################################################################################################

    '        DatiWS = DPI_DatixWS(CStr(DatiAgenda),
    '                                          CInt(Id_Agenda_Escluso),
    '                                          CInt(Disciplinare_Cod),
    '                                          CStr(Piva_Riferimento),
    '                                          CInt(Sa_Cod_Riferimento),
    '                                          CInt(Appezza_Riferimento),
    '                                          CInt(Id_Reg_Riferimento),
    '                                          objParametri_Server)


    '        '######################################################################################################
    '        '####################### CHIAMATA WEB SERVICE #########################################################
    '        '######################################################################################################

    '        'Scrivi_LOG(objParametri_Server, "DPI_Verifica_DifesaDiserbo", "chiamo il web service")

    '        Dim ObjDownloadWs As New WS_Disciplinari.AgroWS_Disciplinari

    '        Dim agroWs As String
    '        If IsNothing(ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
    '            'Scrivi_LOG(objParametri_Server, "DPI_Verifica_DifesaDiserbo", "creo agrowebconfig")
    '            'creo l'agrowebconfig
    '            Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
    '            agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
    '        Else
    '            'Scrivi_LOG(objParametri_Server, "DPI_Verifica_DifesaDiserbo", "webconfig " & ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString())
    '            agroWs = ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
    '        End If


    '        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
    '        objWs.NewWS(ObjDownloadWs,
    '                        agroWs,
    '                        objParametri_Utenti)


    '        If Not HttpContext.Current Is Nothing Then

    '            ASG_Utente_Username_Crypt = CStr(HttpContext.Current.Session("ASG_Utente_Username_Crypt").ToString)
    '            ASG_Utente_Password_Crypt = CStr(HttpContext.Current.Session("ASG_Utente_Password_Crypt").ToString)

    '        Else

    '            Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Read
    '            Dim DtUtente As DataTable

    '            DtUtente = objUtente.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta,
    '                                       " Utenti.Username='" & Agro_SQL_SaveText(objParametri_Server.SuperUserUsername) & "'",
    '                                       "", objParametri_Utenti)
    '            If DtUtente.Rows.Count > 0 Then
    '                ASG_Utente_Username_Crypt = Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, AgroKey_EncoderDecoder)
    '                ASG_Utente_Password_Crypt = Stringa_Codifica_LANCompatibile(DtUtente.Rows(0).Item("Password").ToString, AgroKey_EncoderDecoder)
    '            End If

    '        End If

    '        'Scrivi_LOG(objParametri_Server, "DPI_Verifica_DifesaDiserbo", "chiamo web service")
    '        DatiVerifica = ObjDownloadWs.DPI_Verifica_DifesaDiserboWS_Livello(ASG_Utente_Username_Crypt,
    '                                                                        ASG_Utente_Password_Crypt,
    '                                                                        CStr(DatiWS),
    '                                                                        Livello)

    '        'Scrivi_LOG(objParametri_Server, "DPI_Verifica_DifesaDiserbo", "chiamato web service")
    '        DPI_Verifica_DifesaDiserbo = XmlFINALE(CStr(DatiVerifica),
    '                                                CInt(Id_Agenda_Escluso),
    '                                               VerificaSoloControlliImpostazioniUtente,
    '                                                objParametri_Server,
    '                                                objParametri_Utenti,
    '                                                DatiAgenda)

    '    Catch ex As Exception

    '        '------------------------------------------------
    '        'Si e' verificata una eccezione !!!!!!
    '        '------------------------------------------------

    '        Dim StrDummy As String

    '        ''Messaggio di errore
    '        StrDummy = ex.Message.ToString()

    '        Scrivi_LOG(objParametri_Server, "DPI_Verifica_DifesaDiserbo", StrDummy)

    '        ''Se la transazione ha avuto esito positivo allora ... stampo il messaggio sul client
    '        'AgroMsgBox("Si è verificato un errore durante la fase di verifica conformità: " & StrDummy, objPage)


    '    End Try


    'End Function

    '============================================================================
    'Public Function Verifica_DifesaDiserbo(ByRef objParametri_Server As AgronicaCoreParametri,
    '                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
    '                                           ByVal DatiAgenda As String,
    '                                           ByVal Id_Agenda_Escluso As Integer,
    '                                           ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
    '                                           Optional ByVal Piva_Riferimento As String = "",
    '                                           Optional ByVal Sa_Cod_Riferimento As Integer = 0,
    '                                           Optional ByVal Appezza_Riferimento As Integer = 0,
    '                                           Optional ByVal Id_Reg_Riferimento As Integer = 0,
    '                                           Optional ByVal Biologico As Boolean = False) As String



    '    Dim DatiWS As String
    '    Dim DatiVerifica As String

    '    Dim ASG_Utente_Username_Crypt As String
    '    Dim ASG_Utente_Password_Crypt As String

    '    Try


    '        '######################################################################################################
    '        '################################ DATI WS #############################################################
    '        '######################################################################################################


    '        DatiWS = DPI_DatixWS(CStr(DatiAgenda),
    '                                          CInt(Id_Agenda_Escluso),
    '                                          CInt(0),
    '                                          CStr(Piva_Riferimento),
    '                                          CInt(Sa_Cod_Riferimento),
    '                                          CInt(Appezza_Riferimento),
    '                                          CInt(Id_Reg_Riferimento),
    '                                          objParametri_Server,
    '                                          Biologico)

    '        '######################################################################################################
    '        '####################### CHIAMATA WEB SERVICE #########################################################
    '        '######################################################################################################

    '        Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci

    '        Dim agroWs As String
    '        If IsNothing(ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci")) Then
    '            'creo l'agrowebconfig
    '            Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
    '            agroWs = objAgroWeb.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
    '        Else
    '            agroWs = ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
    '        End If


    '        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
    '        objWs.NewWS(ObjDownloadWs,
    '                        agroWs,
    '                        objParametri_Utenti)


    '        If Not HttpContext.Current Is Nothing Then

    '            ASG_Utente_Username_Crypt = CStr(HttpContext.Current.Session("ASG_Utente_Username_Crypt").ToString)
    '            ASG_Utente_Password_Crypt = CStr(HttpContext.Current.Session("ASG_Utente_Password_Crypt").ToString)

    '        Else

    '            Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Read
    '            Dim DtUtente As DataTable

    '            DtUtente = objUtente.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta,
    '                                       " Utenti.Username='" & Agro_SQL_SaveText(objParametri_Server.SuperUserUsername) & "'",
    '                                       "", objParametri_Utenti)
    '            If DtUtente.Rows.Count > 0 Then
    '                ASG_Utente_Username_Crypt = Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, AgroKey_EncoderDecoder)
    '                ASG_Utente_Password_Crypt = Stringa_Codifica_LANCompatibile(DtUtente.Rows(0).Item("Password").ToString, AgroKey_EncoderDecoder)
    '            End If

    '        End If

    '        If Biologico = False Then
    '            DatiVerifica = ObjDownloadWs.Verifica_DifesaDiserboWS(ASG_Utente_Username_Crypt,
    '                                                  ASG_Utente_Password_Crypt,
    '                                                  CStr(DatiWS))
    '        Else
    '            DatiVerifica = ObjDownloadWs.Verifica_DifesaDiserboWS_Bio(ASG_Utente_Username_Crypt,
    '                                                  ASG_Utente_Password_Crypt,
    '                                                  CStr(DatiWS))
    '        End If


    '        Verifica_DifesaDiserbo = XmlFINALE(CStr(DatiVerifica),
    '                                           CInt(Id_Agenda_Escluso),
    '                                           VerificaSoloControlliImpostazioniUtente,
    '                                           objParametri_Server,
    '                                           objParametri_Utenti,
    '                                           CStr(DatiAgenda))

    '    Catch ex As Exception

    '        '------------------------------------------------
    '        'Si e' verificata una eccezione !!!!!!
    '        '------------------------------------------------

    '        Dim StrDummy As String

    '        ''Messaggio di errore
    '        StrDummy = ex.Message.ToString()

    '        Scrivi_LOG(objParametri_Server, "Verifica_DifesaDiserbo", StrDummy)

    '        ''Se la transazione ha avuto esito positivo allora ... stampo il messaggio sul client
    '        'AgroMsgBox("Si è verificato un errore durante la fase di verifica conformità: " & StrDummy, objPage)


    '    End Try


    'End Function


    ''' <param name="TotCU_Fertilizzazione_MultiAttivita_xDistinta"> ((PIVA, SA_COD, APPEZZA, ID_REG), CU_xDISTINTA)
    ''' MI SALVO IL TOT DI RAME USATO NELLE CONCIMAZIONI X OGNI DISTINTA --> DA PASSARE AL VERIFICA TRATTAMENTI
    ''' </param>
    ''' <param name="dic_CUTrattamenti_xDistintaxLavCod"> ((PIVA, SA_COD, APPEZZA, ID_REG), LAV_COD), CU_xDISTINTAxLAV_COD)
    ''' MI SALVO IL TOT DI RAME USATO NELLE CONCIMAZIONI X OGNI DISTINTA X OGNI LAV_COD --> DA PASSARE AL VERIFICA TRATTAMENTI, COSI' DA POTER CONTARE I MULTI TRATTAMENTI
    ''' </param>
    Public Function Verifica_DifesaDiserbo_New(ByRef objParametri_Server As AgronicaCoreParametri,
                                               ByRef objParametri_Utenti As AgronicaCoreParametri,
                                               ByVal DatiAgenda As String,
                                               ByVal Id_Agenda_Escluso As Integer,
                                               ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
                                               ByVal Disciplinare_Cod As String,
                                               ByVal Disciplinare_PubblicoPrivato As String,
                                               ByVal Piva_Riferimento As String,
                                               ByVal Sa_Cod_Riferimento As Integer,
                                               ByVal Appezza_Riferimento As Integer,
                                               ByVal Id_Reg_Riferimento As Integer,
                                               ByVal dtDosiVuoto As Boolean,
                                                Optional ByVal objParametri_Super_Server As AgronicaCoreParametri = Nothing,
                                                Optional paramVerificaDPIMultiAttivita As AgronicaCoreModello.VerificaDPIMultiAttivita = Nothing
                                                ) As rispostaStandard(Of Verifica_Disciplinare_Intervento)


        Dim r As New rispostaStandard(Of Verifica_Disciplinare_Intervento)
        r.RispostaStringa = New Verifica_Disciplinare_Intervento

        Dim DatiWS As String
        Dim DatiVerifica As String

        Dim ASG_Utente_Username_Crypt As String
        Dim ASG_Utente_Password_Crypt As String

        Dim xLeggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Try


            Dim Livello As Integer
            'Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            'Dim DT As DataTable
            'DT = objUtentiImpostazioni.Leggi(0, 1, enumSelezioneVariabile.Selezione_TabellaCompleta,
            '                                  "", "", objParametri_Utenti)
            'Dim DR() As DataRow

            'DR = DT.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_COD_LIVELLO_CHK_DPI)
            'If DR.Length > 0 Then
            '    '1 = Livello Base
            '    '2 = Livello Avanzato
            '    Livello = DR(0).Item("Impostazione_Valore_1")
            'Else
            '    'se non ho il livello impostato utilizzo quello di base 
            '    Livello = 1
            'End If

            '(02/11/2017 fede) reintrodotte soglie
            Livello = 2

            '######################################################################################################
            '################################ DATI WS #############################################################
            '######################################################################################################
            Dim Dpi_Cod As Integer = 0
            Dim Id_Rcdpi As Integer = 0
            Dim Dpi_PubblicoPrivato As Integer = 0
            Dim Biologico As Boolean = False

            Dim Dpi_Cod_Op As Integer = 0
            Dim Id_Rcdpi_Op As Integer = 0
            Dim Dpi_PubblicoPrivato_Op As Integer = 0

            If IsNumeric(Disciplinare_Cod) AndAlso CInt(Disciplinare_Cod) = enum_Disciplinare_Operazione.QuelloDellOperazione Then

                Dim XmlDom As New XmlDocument
                Dim xDatiAgende As XmlNodeList
                Dim xDatiAgenda As XmlElement
                Dim xAgende As XmlNodeList
                Dim xAgenda As XmlElement
                Dim xDatiMovimenti As XmlNodeList
                Dim xDatiMovimento As XmlElement
                Dim xMovimenti As XmlNodeList
                Dim xMovimento As XmlElement

                XmlDom.LoadXml(DatiAgenda)
                xDatiAgende = XmlDom.GetElementsByTagName("DatiAgenda")

                For i_DatiAgenda = 0 To xDatiAgende.Count - 1
                    xDatiAgenda = xDatiAgende.Item(i_DatiAgenda)
                    xAgende = xDatiAgenda.GetElementsByTagName("Agenda")
                    For i_Agenda = 0 To xAgende.Count - 1
                        xAgenda = xAgende.Item(i_Agenda)
                        xDatiMovimenti = xAgenda.GetElementsByTagName("DatiMovimenti")
                        For i_DatiMovimento = 0 To xDatiMovimenti.Count - 1
                            xDatiMovimento = xDatiMovimenti.Item(i_DatiMovimento)
                            xMovimenti = xDatiMovimento.GetElementsByTagName("Movimento")
                            For i_Movimento = 0 To xMovimenti.Count - 1
                                xMovimento = xMovimenti.Item(i_Movimento)
                                Select Case xMovimento.GetAttribute("cau_mov")
                                    Case CAU_TRATTAMENTO, CAU_RILIEVO_CAMPO, CAU_RILIEVO_RACCOLTA, CAU_LAVORAZIONE
                                        Dpi_Cod_Op = CInt(xMovimento.GetAttribute("num_protocollo"))
                                        If xMovimento.HasAttribute("disciplinare_pubblicoprivato") AndAlso
                                            xMovimento.GetAttribute("disciplinare_pubblicoprivato") = 2 Then
                                            Dpi_Cod_Op = -CInt(xMovimento.GetAttribute("num_protocollo"))
                                        End If
                                        If xMovimento.HasAttribute("doc_numero") Then
                                            Id_Rcdpi_Op = CInt(xMovimento.GetAttribute("doc_numero"))
                                        End If
                                End Select
                            Next
                        Next
                    Next
                Next

            End If

            Select Case Disciplinare_Cod
                Case enum_Disciplinare_Operazione.Nessuno
                    Dpi_Cod = 0
                Case enum_Disciplinare_Operazione.Biologico
                    Dpi_Cod = 0
                    Biologico = True
                Case enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta
                    Dpi_Cod = 0
                Case enum_Disciplinare_Operazione.QuelloDellOperazione
                    Select Case Dpi_Cod_Op
                        Case enum_Disciplinare_Operazione.Nessuno
                            Dpi_Cod = 0
                        Case enum_Disciplinare_Operazione.Biologico
                            Dpi_Cod = 0
                            Biologico = True
                        Case enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta
                            Dpi_Cod = 0
                        Case Else
                            Dpi_Cod = Dpi_Cod_Op
                            Id_Rcdpi = Id_Rcdpi_Op
                    End Select
                Case Else
                    'dpi
                    Dim Array As String()
                    Array = Split(Disciplinare_Cod, "/")
                    Dpi_Cod = CInt(Array(0))
                    Dpi_PubblicoPrivato = Disciplinare_PubblicoPrivato
                    If Array.Length > 4 AndAlso Array(4) IsNot Nothing Then
                        Dpi_PubblicoPrivato = CInt(Array(4))
                    End If
                    If Dpi_PubblicoPrivato = 2 Then
                        Dpi_Cod = -Dpi_Cod
                    End If
                    If Array.Length > 1 AndAlso Array(1) IsNot Nothing Then
                        Id_Rcdpi = CInt(Array(1))
                    End If

            End Select

            DatiWS = DPI_DatixWS(DatiAgenda,
                                Id_Agenda_Escluso,
                                Dpi_Cod,
                                Id_Rcdpi,
                                Biologico,
                                Piva_Riferimento,
                                Sa_Cod_Riferimento,
                                Appezza_Riferimento,
                                Id_Reg_Riferimento,
                                objParametri_Server, dtDosiVuoto, objParametri_Super_Server)


            '######################################################################################################
            '####################### CHIAMATA WEB SERVICE #########################################################
            '######################################################################################################


            If HttpContext.Current.Session IsNot Nothing AndAlso
               HttpContext.Current.Session("ASG_Utente_Username_Crypt") IsNot Nothing AndAlso
               HttpContext.Current.Session("ASG_Utente_Password_Crypt") IsNot Nothing Then

                ASG_Utente_Username_Crypt = CStr(HttpContext.Current.Session("ASG_Utente_Username_Crypt").ToString)
                ASG_Utente_Password_Crypt = CStr(HttpContext.Current.Session("ASG_Utente_Password_Crypt").ToString)

            Else

                Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Read
                Dim DtUtente As DataTable

                DtUtente = objUtente.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "", "", objParametri_Utenti, objParametri_Server.SuperUserUsername)
                If DtUtente.Rows.Count > 0 Then
                    ASG_Utente_Username_Crypt = Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, AgroKey_EncoderDecoder)
                    ASG_Utente_Password_Crypt = Stringa_Codifica_LANCompatibile(DtUtente.Rows(0).Item("Password").ToString, AgroKey_EncoderDecoder)
                End If

            End If




            Select Case Dpi_Cod

                Case 0

                    Dim ObjDownloadWs As New WS_Fitofarmaci.AgroWS_Fitofarmaci

                    Dim agroWs As String
                    If IsNothing(ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci")) Then
                        'creo l'agrowebconfig
                        Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                        agroWs = objAgroWeb.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci
                    Else
                        agroWs = ConfigurationManager.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString()
                    End If

                    If agroWs Is Nothing OrElse agroWs = "" Then
                        Dim DTConfigurazioneSiti As DataTable = xLeggiConfigurazioneSiti.Leggi(0, "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci", "", "", objParametri_Super_Server)

                        If Not IsNothing(DTConfigurazioneSiti) AndAlso DTConfigurazioneSiti.Rows.Count = 1 Then
                            agroWs = DTConfigurazioneSiti.Rows(0)("Valore")
                        End If
                    End If

                    Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                    objWs.NewWS(ObjDownloadWs,
                            agroWs,
                            objParametri_Utenti)

                    If Not Biologico Then
                        DatiVerifica = ObjDownloadWs.Verifica_DifesaDiserboWS(ASG_Utente_Username_Crypt,
                                                              ASG_Utente_Password_Crypt,
                                                              CStr(DatiWS))
                    Else
                        DatiVerifica = ObjDownloadWs.Verifica_DifesaDiserboWS_Bio(ASG_Utente_Username_Crypt,
                                                              ASG_Utente_Password_Crypt,
                                                              CStr(DatiWS))
                    End If

                Case Else

                    Dim ObjDownloadWs As New WS_Disciplinari.AgroWS_Disciplinari

                    Dim agroWs As String
                    If IsNothing(ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                        Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                        agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
                    Else
                        agroWs = ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
                    End If

                    If agroWs Is Nothing OrElse agroWs = "" Then
                        Dim DTConfigurazioneSiti As DataTable = xLeggiConfigurazioneSiti.Leggi(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Super_Server)

                        If Not IsNothing(DTConfigurazioneSiti) AndAlso DTConfigurazioneSiti.Rows.Count = 1 Then
                            agroWs = DTConfigurazioneSiti.Rows(0)("Valore")
                        End If
                    End If

                    Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                    objWs.NewWS(ObjDownloadWs,
                            agroWs,
                            objParametri_Utenti)

                    DatiVerifica = ObjDownloadWs.DPI_Verifica_DifesaDiserboWS_Livello(ASG_Utente_Username_Crypt,
                                                                                    ASG_Utente_Password_Crypt,
                                                                                    CStr(DatiWS),
                                                                                    Livello)

            End Select

            Dim rRis As New rispostaStandard(Of Verifica_Disciplinare_Intervento)
            rRis = XmlFINALE(CStr(DatiVerifica),
                             CInt(Id_Agenda_Escluso),
                             VerificaSoloControlliImpostazioniUtente,
                             objParametri_Server,
                             objParametri_Utenti,
                             DatiAgenda,
                             Biologico,
                             Dpi_Cod,
                             paramVerificaDPIMultiAttivita:=paramVerificaDPIMultiAttivita)

            If rRis.RispostaOK Then

                r.RispostaOK = True
                r.RispostaStringa.Risultato = rRis.RispostaStringa.Risultato
                r.RispostaStringa.Conforme = rRis.RispostaStringa.Conforme

            Else
                Throw New Exception(rRis.Errore)
            End If



        Catch ex As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            Dim StrDummy As String

            ''Messaggio di errore
            StrDummy = ex.Message.ToString()
            If ex.InnerException IsNot Nothing Then
                StrDummy = StrDummy + " " + ex.InnerException.Message.ToString()
            End If

            Scrivi_LOG(objParametri_Server, "Verifica_DifesaDiserbo_New", StrDummy)

            r.RispostaOK = False
            r.Errore = StrDummy

        End Try

        Return r

    End Function


    'verifica n operazioni
    Public Function Verifica_DifesaDiserbo(ByRef objParametri_Server As AgronicaCoreParametri,
                                               ByRef objParametri_Utenti As AgronicaCoreParametri,
                                               ByVal DatiOperazioni As String, ByVal IDTestataTemp__tmp_FormulatiXPrincipiAttivi As Integer,
                                               ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
                                               ByRef objParametri_SuperServer As AgronicaCoreParametri,
                                               Optional ByVal leggiUrlDaConfigurazioniSiti As Boolean = False) As List(Of rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento))


        Dim Lr As New List(Of rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento))

        Dim rRis As rispostaStandard(Of Verifica_Disciplinare_Intervento)
        Dim r As New rispostaStandard(Of Verifica_Disciplinare_Intervento)
        r.RispostaStringa = New Verifica_Disciplinare_Intervento

        Dim DatiVerifica As String

        Dim ASG_Utente_Username_Crypt As String
        Dim ASG_Utente_Password_Crypt As String

        Try

            If HttpContext.Current IsNot Nothing Then

                ASG_Utente_Username_Crypt = CStr(HttpContext.Current.Session("ASG_Utente_Username_Crypt").ToString)
                ASG_Utente_Password_Crypt = CStr(HttpContext.Current.Session("ASG_Utente_Password_Crypt").ToString)

            Else

                Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Read
                Dim DtUtente As DataTable

                DtUtente = objUtente.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "", "", objParametri_Utenti, objParametri_Server.SuperUserUsername)
                If DtUtente.Rows.Count > 0 Then
                    ASG_Utente_Username_Crypt = Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, AgroKey_EncoderDecoder)
                    ASG_Utente_Password_Crypt = Stringa_Codifica_LANCompatibile(DtUtente.Rows(0).Item("Password").ToString, AgroKey_EncoderDecoder)
                End If

            End If

            Dim ObjDownloadWs As New WS_Disciplinari.AgroWS_Disciplinari

            Dim agroWs As String
            Dim Timeout As Integer = 1000000
            If Not leggiUrlDaConfigurazioniSiti Then
                If IsNothing(ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                    Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                    agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
                    Timeout = objAgroWeb.WS_Timeout * 1000
                Else
                    agroWs = ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
                End If
            Else
                Dim objConfig As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                agroWs = objConfig.Leggi_Valore(0, "GiasOnline.WS_Disciplinari.AgroWS_Disciplinari", "", "", objParametri_Server)
                If agroWs = "" AndAlso objParametri_SuperServer IsNot Nothing Then
                    agroWs = objConfig.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_SuperServer)
                End If
            End If

            Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
            objWs.NewWS(ObjDownloadWs,
                            agroWs,
                            objParametri_Utenti)

            ObjDownloadWs.Timeout = Timeout
            DatiVerifica = ObjDownloadWs.Verifica_DiverseOperazioni(ASG_Utente_Username_Crypt, ASG_Utente_Password_Crypt,
                                                                                    CStr(DatiOperazioni))

            If DatiVerifica <> "" Then

                Dim XmlDom As New XmlDocument
                Dim xDatiOperazioni As XmlElement
                Dim xOperazioni As XmlNodeList
                Dim xOperazione As XmlElement

                Dim xDatiRisultati As XmlElement
                Dim xDatiGenerali As XmlElement

                Dim Dpi_Cod As Integer = 0
                Dim Biologico As Boolean = False
                Dim Id_Agenda As Integer = 0

                Dim Superficie As Decimal
                Dim Appezzamenti As String

                XmlDom.LoadXml(DatiVerifica)
                xDatiOperazioni = XmlDom.SelectSingleNode("DatiOperazioni")

                If xDatiOperazioni IsNot Nothing Then

                    xOperazioni = xDatiOperazioni.GetElementsByTagName("Operazione")

                    If xOperazioni IsNot Nothing AndAlso xOperazioni.Count > 0 Then

                        For i = 0 To xOperazioni.Count - 1

                            xOperazione = xOperazioni(i)

                            Dpi_Cod = 0
                            Biologico = False

                            Superficie = 0
                            Appezzamenti = ""

                            If xOperazione.HasAttribute("dpi_cod") AndAlso IsNumeric(xOperazione.HasAttribute("dpi_cod")) Then
                                Dpi_Cod = xOperazione.GetAttribute("dpi_cod")
                            End If

                            If xOperazione.HasAttribute("biologico") Then
                                Biologico = xOperazione.GetAttribute("biologico")
                            End If

                            If xOperazione.HasAttribute("id_agenda") AndAlso IsNumeric(xOperazione.HasAttribute("id_agenda")) Then
                                Id_Agenda = xOperazione.GetAttribute("id_agenda")
                            End If

                            If xOperazione.HasAttribute("superficie") AndAlso IsNumeric(xOperazione.HasAttribute("superficie")) Then
                                Superficie = xOperazione.GetAttribute("superficie")
                            End If

                            If xOperazione.HasAttribute("appezzamenti") Then
                                Appezzamenti = xOperazione.GetAttribute("appezzamenti")
                            End If

                            xDatiRisultati = xOperazione.SelectSingleNode("DatiRisultati")
                            xDatiGenerali = xDatiRisultati.SelectSingleNode("DatiGenerali")

                            xDatiGenerali.SetAttribute("appezzamenti", Appezzamenti)
                            xDatiGenerali.SetAttribute("superficie", Superficie)


                            rRis = New rispostaStandard(Of Verifica_Disciplinare_Intervento)
                            rRis = XmlFINALE(CStr(xOperazione.InnerXml),
                                                        CInt(Id_Agenda),
                                                        VerificaSoloControlliImpostazioniUtente,
                                                        objParametri_Server,
                                                        objParametri_Utenti,
                                                        "",
                                                            Biologico, Dpi_Cod,
                                                                IDTestataTemp__tmp_FormulatiXPrincipiAttivi)

                            Lr.Add(rRis)

                        Next

                    End If

                End If

            End If

        Catch ex As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            Dim StrDummy As String

            ''Messaggio di errore
            StrDummy = ex.Message.ToString()
            If ex.InnerException IsNot Nothing Then
                StrDummy = StrDummy + " " + ex.InnerException.Message.ToString()
            End If

            Scrivi_LOG(objParametri_Server, "Verifica_DifesaDiserbo", StrDummy)

            r.RispostaOK = False
            r.Errore = StrDummy

        End Try

        Return Lr

    End Function


    '#############################################################################################################
    '#############################################################################################################
    '#############################################################################################################

    Public Function DPI_DatixWS(ByVal DatiAgenda As String,
                                ByVal Id_Agenda As Int32,
                                ByVal Disciplinare_Cod As Integer,
                                ByVal Id_Rcdpi As Integer,
                                ByVal Biologico As Boolean,
                                ByVal Piva_Riferimento As String,
                                ByVal Sa_Cod_Riferimento As Int32,
                                ByVal Appezza_Riferimento As Int32,
                                ByVal Id_Reg_Riferimento As Int32,
                                ByRef objParametri As AgronicaCoreParametri,
                                Optional ByVal dtDosiVuoto As Boolean = False,
                                Optional ByVal objParametri_Super_Server As AgronicaCoreParametri = Nothing
                                ) As String


        Dim NomeRoutine As String = "DpiBIZ.DPI_Verifica.DPI_DatiWS()"
        Dim MessaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction
        Dim i As Int32
        Dim RisultatoFunzione As String = String.Empty

        Dim XmlDoc As XmlDocument
        Dim XmlDom As XmlDocument

        Dim XmlDatiWS As XmlElement
        Dim XmlDatiGenerali As XmlElement
        Dim XmlDatiImpianti As XmlElement
        Dim XmlDatiImpianto As XmlElement
        Dim XmlDatiDettagli As XmlElement
        Dim XmlDatiDettaglio As XmlElement

        Dim Tipo_Distribuzione_H2O As enTipoMezzo

        Dim PIVA As String = ""
        Dim Sa_Cod As Integer
        Dim Lav_Cod As Integer
        Dim Des_Lib As String = ""
        Dim Data As Date
        Dim Av_Cod As Integer
        Dim Av_Gru As Integer

        Dim Gru_Cod As Integer
        Dim Veg_Cod As Integer
        Dim Grfi_Cod As Integer
        Dim Foral_Cod As Integer
        Dim Stato_Impianto As Integer
        Dim Cop_Cod As Integer
        Dim Pro_Cod As Integer
        Dim for_veg_av_dos_cod As String = ""
        Dim for_veg_cod As String = ""
        Dim formulatixallegatinormative_idriga As Integer = 0
        Dim H2O As Decimal
        Dim H2O_Totale As Decimal
        Dim TipoTestata As Integer
        Dim Mezzo As Integer
        Dim Sup_Imp As Decimal
        Dim Sup_Totale As Decimal
        Dim Sup_Operazione As Decimal
        Dim TipoOperazioneDB As Integer

        Dim xDatiAgende As XmlNodeList
        Dim xDatiAgenda As XmlElement
        Dim xAgende As XmlNodeList
        Dim xAgenda As XmlElement

        Dim xDatiMovimenti As XmlNodeList
        Dim xDatiMovimento As XmlElement
        Dim xMovimenti As XmlNodeList
        Dim xMovimento As XmlElement

        Dim xDatiMovimenti_Dettagli As XmlNodeList
        Dim xDatiMovimento_Dettaglio As XmlElement
        Dim xMovimenti_Dettagli As XmlNodeList = Nothing
        Dim xMovimento_Dettaglio As XmlElement

        Dim xMov_Destinazioni As XmlNodeList
        Dim xMov_Destinazione As XmlElement

        Dim xDatiMov_Dettagli_Tecnici As XmlNodeList
        Dim xDatiMov_Dettaglio_Tecnico As XmlElement

        Dim i_DatiAgenda As Integer
        Dim i_Agenda As Integer
        Dim i_DatiMovimento As Integer
        Dim i_Movimento As Integer
        Dim i_DatiMovimenti_Dettagli As Integer
        Dim i_DatiMovimento_Dettaglio As Integer
        Dim i_Mov_Dettaglio_Tecnico As Integer
        Dim i_DatiMov_Destinazioni As Integer

        Dim bGestione_Verifica As Boolean
        Dim bScheda As Boolean = True
        Dim bImpianto_Coerente As Boolean
        Dim bImpianti_Impostati As Boolean
        Dim bAvversita_Generali As Boolean 'Booleano per la gestione generali delle avversità/infestanti

        Dim mModulo As Integer
        Dim mEp_Cod As Integer

        Dim Destinazioni As String(,) = Nothing
        Dim Avversita As String(,)
        Dim Dettagli As String(,) = Nothing

        Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        'Dim Id_Rcdpi As Integer = 0

        'Dim Dpi_Cod As Integer = 0
        'Dim Dpi_PubblicoPrivato As Integer = 0
        'Dim Biologico As Boolean = False

        'Dim FasiOLD As Boolean = True
        Dim FF_Cod_Fioritura_Old As Integer = 0
        Dim FF_Cod_Fioritura_New As Integer = 0
        '------------------------------

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                xTransazione = Nothing
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
                xTransazione = objParametri.objTransazione
            End If

            ' 2022-03-16 Lettura nuova chiave da configurazione siti Flag_Nuovo_Controllo_Riduzione_Diserbo
            ' per aggiunta tag a xml se usare vecchio o nuovo controllo
            Dim nuovoControlloRiduzioneDiserbo As Boolean = Leggi_FlagNuovoControlloRiduzioneDiserbo(objParametri)
            Dim arrayPrincipiAttivi As New List(Of Object)


            '------------------------------
            XmlDom = New XmlDocument
            '------------------------------

            '================================================================================================================

            XmlDom.LoadXml(DatiAgenda)


            XmlDoc = New XmlDocument

            XmlDatiWS = XmlDoc.CreateElement("DatiWS")

            '----- < DatiGenerali > ----
            XmlDatiGenerali = XmlDoc.CreateElement("DatiGenerali")

            '############################################################################################
            '################# Lettura dei Parametri Base di Agenda     #################################
            '############################################################################################

            xDatiAgende = XmlDom.GetElementsByTagName("DatiAgenda")

            i_DatiAgenda = 0

            Do While i_DatiAgenda < xDatiAgende.Count

                'Prelevo l'i-esimo blocco di DatiAgenda (in realtà ne esiste uno solo)
                xDatiAgenda = xDatiAgende.Item(i_DatiAgenda)

                '------------------------------

                xAgende = xDatiAgenda.GetElementsByTagName("Agenda")

                i_Agenda = 0

                Do While i_Agenda < xAgende.Count

                    'Prelevo l' i-esima Codifica Agenda
                    xAgenda = xAgende.Item(i_Agenda)

                    ReDim Dettagli(13, 0)

                    'Parametri agenda
                    TipoOperazioneDB = CInt(xAgenda.GetAttribute("TipoOperazioneDB"))

                    PIVA = xAgenda.GetAttribute("piva")
                    Sa_Cod = CInt(xAgenda.GetAttribute("sa_cod"))
                    Lav_Cod = CInt(xAgenda.GetAttribute("lav_cod"))
                    Des_Lib = xAgenda.GetAttribute("des_lib")
                    Data = CDate(xAgenda.GetAttribute("validita_inizio"))
                    Veg_Cod = -1
                    Gru_Cod = 0
                    bImpianti_Impostati = False


                    ReDim Destinazioni(29, 0)


                    'Verifica della validita dell'intervento
                    Select Case Lav_Cod

                        Case LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO

                            bGestione_Verifica = True
                            TipoTestata = enum_Disciplinare_Tipo_Testata.Diserbo

                        Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                             LAVCOD_GEODISINFESTAZIONE,
                             LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                             LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA,
                             LAVCOD_REINNESCO_TRAPPOLE

                            bGestione_Verifica = True
                            TipoTestata = enum_Disciplinare_Tipo_Testata.Difesa

                        Case LAVCOD_TRATTAMENTO_FITOREGOLATORE

                            bGestione_Verifica = True
                            TipoTestata = enum_Disciplinare_Tipo_Testata.Fitoregolatore


                        Case LAVCOD_TRATTAMENTO_ANTIBUTTERATURA, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                             LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_FERTIRRIGAZIONE, LAVCOD_SARCHIATURA_CONCIMAZIONE

                            bGestione_Verifica = False


                        Case Else 'Lavorazione non verificabile

                            bGestione_Verifica = False

                    End Select


                    If bGestione_Verifica Then

                        '############################################################################################
                        '################# Lettura della Scheda di Lavorazione della Lavorazione     ################
                        '############################################################################################

                        xDatiMovimenti = xAgenda.GetElementsByTagName("DatiMovimenti")

                        i_DatiMovimento = 0

                        'Check di esistenza Informazioni
                        Do While i_DatiMovimento < xDatiMovimenti.Count

                            xDatiMovimento = xDatiMovimenti.Item(i_DatiMovimento)

                            xMovimenti = xDatiMovimento.GetElementsByTagName("Movimento")

                            i_Movimento = 0

                            Do While i_Movimento < xMovimenti.Count

                                xMovimento = xMovimenti.Item(i_Movimento)

                                'Verifico che il movimento sia la scheda di lavorazione

                                Select Case xMovimento.GetAttribute("cau_mov")

                                    Case CAU_TRATTAMENTO, CAU_RILIEVO_CAMPO, CAU_RILIEVO_RACCOLTA, CAU_LAVORAZIONE

                                        '        'Select Case Disciplinare_Cod

                                        '        '    Case enum_Disciplinare_Operazione.QuelloDellOperazione
                                        '        '        Dpi_Cod = CInt(xMovimento.GetAttribute("num_protocollo"))
                                        '        '        If xMovimento.HasAttribute("disciplinare_pubblicoprivato") = True AndAlso
                                        '        '            xMovimento.GetAttribute("disciplinare_pubblicoprivato") = 2 Then
                                        '        '            Dpi_Cod = -CInt(xMovimento.GetAttribute("num_protocollo"))
                                        '        '        End If
                                        '        '        If xMovimento.HasAttribute("doc_numero") = True Then
                                        '        '            Id_Rcdpi = CInt(xMovimento.GetAttribute("doc_numero"))
                                        '        '        End If

                                        '        '    Case enum_Disciplinare_Operazione.Nessuno 'etichetta
                                        '        '        Dpi_Cod = 0

                                        '        '    Case enum_Disciplinare_Operazione.Biologico
                                        '        '        Dpi_Cod = 0
                                        '        '        Biologico = True

                                        '        '    Case enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta
                                        '        '        Dpi_Cod = 0

                                        '        '    Case Else 'dpi

                                        '        '        Dim Array() As String
                                        '        '        Array = Split(Disciplinare_Cod, "/")
                                        '        '        Dpi_Cod = CInt(Array(0))
                                        '        '        If Array.Length > 4 AndAlso Not Array(4) Is Nothing Then
                                        '        '            Dpi_PubblicoPrivato = CInt(Array(4))
                                        '        '            If Dpi_PubblicoPrivato = 2 Then
                                        '        '                Dpi_Cod = -Dpi_Cod
                                        '        '            End If
                                        '        '        End If
                                        '        '        If Array.Length > 1 AndAlso Not Array(1) Is Nothing Then
                                        '        '            Id_Rcdpi = Array(1)
                                        '        '        End If
                                        '        'End Select

                                        '        ''Verifico che sia stato impostato 1 disciplinare
                                        '        'If Disciplinare_Cod = 0 Then
                                        '        '    'Impostazione DPI da Lavorazione
                                        '        '    Disciplinare_Cod = CInt(xMovimento.GetAttribute("num_protocollo"))
                                        '        '    If Disciplinare_Cod = -2 Then
                                        '        '        Biologico = True
                                        '        '    End If
                                        '        '    If xMovimento.HasAttribute("disciplinare_pubblicoprivato") = True AndAlso
                                        '        '        xMovimento.GetAttribute("disciplinare_pubblicoprivato") = 2 Then
                                        '        '        Disciplinare_Cod = -CInt(xMovimento.GetAttribute("num_protocollo"))
                                        '        '    End If
                                        '        '    If xMovimento.HasAttribute("doc_numero") = True Then
                                        '        '        Id_Rcdpi = CInt(xMovimento.GetAttribute("doc_numero"))
                                        '        '    End If
                                        '        'Else
                                        '        '    'DPI impostato da interfaccia
                                        '        '    'Do nothing
                                        '        '    If xMovimento.HasAttribute("doc_numero") = True Then
                                        '        '        Id_Rcdpi = CInt(xMovimento.GetAttribute("doc_numero"))
                                        '        '    End If
                                        '        'End If

                                        '        If Dpi_Cod <> 0 Then
                                        bScheda = True
                                        '        Else
                                        '            bScheda = False 'Disciplinare non impostato
                                        '        End If

                                    Case Else

                                        bScheda = False

                                End Select

                                '====================================================================================


                                If bScheda Then

                                    '------------------------------

                                    '====================================================================================
                                    'Prelevo le Informazioni sulle Note, sulla Data di Operazione e sull'Username creazione
                                    '------------------------------------------------------------------------------------
                                    Data = CDate(xMovimento.GetAttribute("data_movimento"))

                                    Mezzo = CInt(xMovimento.GetAttribute("mezzo"))


                                    'Lettura dell'epoca/modulo
                                    Select Case Lav_Cod

                                        Case LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO
                                            mModulo = 0
                                            mEp_Cod = CInt(xMovimento.GetAttribute("extra_int"))

                                        Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_GEODISINFESTAZIONE
                                            mModulo = CInt(xMovimento.GetAttribute("extra_int"))
                                            mEp_Cod = 0

                                        Case Else
                                            mModulo = 0
                                            mEp_Cod = 0

                                    End Select


                                    '====================================================================================
                                    'Prelevo le Informazioni dell'Acqua associata alla lavorazione
                                    '------------------------------------------------------------------------------------

                                    bAvversita_Generali = False
                                    ReDim Avversita(2, 0)

                                    xDatiMov_Dettagli_Tecnici = xMovimento.GetElementsByTagName("Movimento_Dettaglio_Tecnico")

                                    i_Mov_Dettaglio_Tecnico = 0

                                    Do While i_Mov_Dettaglio_Tecnico < xDatiMov_Dettagli_Tecnici.Count

                                        'Prelevo l'i-esimo dettaglio tecnico
                                        xDatiMov_Dettaglio_Tecnico = xDatiMov_Dettagli_Tecnici.Item(i_Mov_Dettaglio_Tecnico)

                                        If CInt(xDatiMov_Dettaglio_Tecnico.GetAttribute("av_cod")) <> 0 OrElse
                                           CInt(xDatiMov_Dettaglio_Tecnico.GetAttribute("av_gru")) <> 0 Then

                                            'Le avversità sono generali rispetto il movimento
                                            bAvversita_Generali = True
                                            ReDim Preserve Avversita(2, UBound(Avversita, 2) + 1)

                                            Avversita(0, UBound(Avversita, 2) - 1) = CInt(xDatiMov_Dettaglio_Tecnico.GetAttribute("av_cod"))
                                            Avversita(1, UBound(Avversita, 2) - 1) = CInt(xDatiMov_Dettaglio_Tecnico.GetAttribute("av_gru"))

                                            Avversita(2, UBound(Avversita, 2) - 1) = 0
                                            If xDatiMov_Dettaglio_Tecnico.HasAttribute("soglia_cod") AndAlso IsNumeric(xDatiMov_Dettaglio_Tecnico.GetAttribute("soglia_cod")) Then
                                                Avversita(2, UBound(Avversita, 2) - 1) = CInt(xDatiMov_Dettaglio_Tecnico.GetAttribute("soglia_cod"))
                                            End If

                                        ElseIf CDbl(Replace(xDatiMov_Dettaglio_Tecnico.GetAttribute("qta_ril"), ".", ",")) <> 0 Then

                                            Select Case CDbl(Replace(xDatiMov_Dettaglio_Tecnico.GetAttribute("qta_ril"), ".", ","))
                                                Case Is > 0

                                                    Tipo_Distribuzione_H2O = enTipoMezzo.Complessivo
                                                    H2O = 0
                                                    H2O_Totale = CDbl(Replace(xDatiMov_Dettaglio_Tecnico.GetAttribute("qta_ril"), ".", ","))

                                                Case Is < 0

                                                    Tipo_Distribuzione_H2O = enTipoMezzo.Ettaro
                                                    H2O = -CDbl(Replace(xDatiMov_Dettaglio_Tecnico.GetAttribute("qta_ril"), ".", ","))
                                                    H2O_Totale = 0

                                            End Select

                                        End If

                                        i_Mov_Dettaglio_Tecnico = i_Mov_Dettaglio_Tecnico + 1

                                    Loop


                                    '====================================================================================
                                    'Prelevo le Informazioni sugli Impianti, I Prodotti e le Quantità Distribuite
                                    '------------------------------------------------------------------------------------
                                    xDatiMovimenti_Dettagli = xMovimento.GetElementsByTagName("DatiMovimenti_Dettagli")

                                    i_DatiMovimenti_Dettagli = 0

                                    Do While i_DatiMovimenti_Dettagli < xDatiMovimenti_Dettagli.Count

                                        'Prelevo l'i-esimo Blocco di Movimenti Dettagli (ne esiste 1 solo)
                                        xDatiMovimento_Dettaglio = xDatiMovimenti_Dettagli.Item(i_DatiMovimenti_Dettagli)

                                        xMovimenti_Dettagli = xDatiMovimento_Dettaglio.GetElementsByTagName("Movimento_Dettaglio")

                                        i_DatiMovimento_Dettaglio = 0

                                        Do While i_DatiMovimento_Dettaglio < xMovimenti_Dettagli.Count

                                            'Prelevo l'i-esimo Movimento Dettaglio
                                            xMovimento_Dettaglio = xMovimenti_Dettagli.Item(i_DatiMovimento_Dettaglio)

                                            If Not bImpianti_Impostati Then

                                                '==========================================================================
                                                'Lettura della specie vegetale e dei dettagli degli impianti
                                                '-------------------------------------------------------------------------

                                                xMov_Destinazioni = xMovimento_Dettaglio.GetElementsByTagName("Movimento_Destinazione")

                                                i_DatiMov_Destinazioni = 0

                                                bImpianti_Impostati = True

                                                Sup_Totale = 0
                                                Sup_Operazione = 0

                                                Dim ArrayPiva(0) As String
                                                Dim ArraySaCod(0) As Integer
                                                Dim ArrayAppezza(0) As Integer
                                                Dim ArrayIdReg(0) As Integer
                                                Dim N_Imp As Integer = 0

                                                Do While i_DatiMov_Destinazioni < xMov_Destinazioni.Count

                                                    'Prelevo l'i-esimo Movimento Dettaglio
                                                    xMov_Destinazione = xMov_Destinazioni.Item(i_DatiMov_Destinazioni)

                                                    If CInt(xMov_Destinazione.GetAttribute("tipo_destinazione")) = 0 Then

                                                        'Verifico che l'impianto sia coerente con quello eventualmente passato come riferimento
                                                        If Trim(Piva_Riferimento) <> "" AndAlso Sa_Cod_Riferimento <> 0 AndAlso
                                                           Appezza_Riferimento <> 0 AndAlso Id_Reg_Riferimento <> 0 AndAlso
                                                           (Appezza_Riferimento <> CInt(xMov_Destinazione.GetAttribute("appezza")) OrElse
                                                           Id_Reg_Riferimento <> CInt(xMov_Destinazione.GetAttribute("id_destinazione"))) Then

                                                            'è stato impostato un impianto come riferimento ed è diverso da quello di lavorazione
                                                            bImpianto_Coerente = False

                                                        Else 'Ok
                                                            bImpianto_Coerente = True
                                                        End If

                                                        If xMov_Destinazione.HasAttribute("qta2") AndAlso IsNumeric(xMov_Destinazione.GetAttribute("qta2")) AndAlso CDbl(xMov_Destinazione.GetAttribute("qta2")) <> 0 Then
                                                            Sup_Operazione += CDbl(xMov_Destinazione.GetAttribute("qta2"))
                                                        Else
                                                            Sup_Operazione += objImp.LeggiSuperficie(xMov_Destinazione.GetAttribute("piva"), CInt(xMov_Destinazione.GetAttribute("sa_cod")), CInt(xMov_Destinazione.GetAttribute("appezza")), CInt(xMov_Destinazione.GetAttribute("id_destinazione")), objParametri)
                                                        End If

                                                        If bImpianto_Coerente Then

                                                            '===================================================================================================================================
                                                            'Aggiornamento struttura di appoggio delle destinazioni di intervento
                                                            '-----------------------------------------------------------------------------------------------------------------------------------
                                                            ReDim Preserve Destinazioni(29, UBound(Destinazioni, 2) + 1)

                                                            'Destinazioni(i_Piva, UBound(Destinazioni, 2) - 1) = PIVA
                                                            'Destinazioni(i_Sa_Cod, UBound(Destinazioni, 2) - 1) = Sa_Cod
                                                            Destinazioni(i_Piva, UBound(Destinazioni, 2) - 1) = xMov_Destinazione.GetAttribute("piva")
                                                            Destinazioni(i_Sa_Cod, UBound(Destinazioni, 2) - 1) = CInt(xMov_Destinazione.GetAttribute("sa_cod"))
                                                            Destinazioni(i_Appezza, UBound(Destinazioni, 2) - 1) = CInt(xMov_Destinazione.GetAttribute("appezza"))
                                                            Destinazioni(i_Id_Reg, UBound(Destinazioni, 2) - 1) = CInt(xMov_Destinazione.GetAttribute("id_destinazione"))
                                                            Destinazioni(i_Sup_Trattata, UBound(Destinazioni, 2) - 1) = CDec(xMov_Destinazione.GetAttribute("qta2"))

                                                            '(07/01/2019 fede) aggiunto controllo buffer
                                                            Destinazioni(i_Sup_Riduzione_BufferZone, UBound(Destinazioni, 2) - 1) = 0
                                                            If xMov_Destinazione.HasAttribute("sup_riduzione_bufferzone") AndAlso IsNumeric(xMov_Destinazione.GetAttribute("sup_riduzione_bufferzone")) Then
                                                                Destinazioni(i_Sup_Riduzione_BufferZone, UBound(Destinazioni, 2) - 1) = CDec(xMov_Destinazione.GetAttribute("sup_riduzione_bufferzone"))
                                                            End If
                                                            Destinazioni(i_Perc_Riduzione_Deriva, UBound(Destinazioni, 2) - 1) = 0
                                                            If xMov_Destinazione.HasAttribute("perc_riduzione_deriva") AndAlso IsNumeric(xMov_Destinazione.GetAttribute("perc_riduzione_deriva")) Then
                                                                Destinazioni(i_Perc_Riduzione_Deriva, UBound(Destinazioni, 2) - 1) = CDec(xMov_Destinazione.GetAttribute("perc_riduzione_deriva"))
                                                            End If

                                                            '-------
                                                            ReDim Preserve ArrayPiva(N_Imp)
                                                            ReDim Preserve ArraySaCod(N_Imp)
                                                            ReDim Preserve ArrayAppezza(N_Imp)
                                                            ReDim Preserve ArrayIdReg(N_Imp)

                                                            ArrayPiva(N_Imp) = xMov_Destinazione.GetAttribute("piva")
                                                            ArraySaCod(N_Imp) = CInt(xMov_Destinazione.GetAttribute("sa_cod"))
                                                            ArrayAppezza(N_Imp) = CInt(xMov_Destinazione.GetAttribute("appezza"))
                                                            ArrayIdReg(N_Imp) = CInt(xMov_Destinazione.GetAttribute("id_destinazione"))
                                                            N_Imp += 1

                                                            '-------

                                                        End If 'Fine controllo impianto coerente

                                                    Else

                                                        'Eccezione: la destinazione del trattamento non è un impianto

                                                    End If


                                                    i_DatiMov_Destinazioni = i_DatiMov_Destinazioni + 1

                                                Loop


                                                If Destinazioni IsNot Nothing AndAlso UBound(Destinazioni, 2) Then

                                                    Dim DtImp As DataTable
                                                    Dim I_Imp As Integer
                                                    Dim DrImp As DataRow()

                                                    objParametri.ImpostaFinestre_con_SalvataggioTemporale(Data, Data)
                                                    DtImp = objImp.Leggi_Dati_Impianti_Distinte(ArrayPiva,
                                                                                        ArraySaCod,
                                                                                        ArrayAppezza,
                                                                                        ArrayIdReg,
                                                                                        "", "",
                                                                                        objParametri)
                                                    objParametri.ResettaFinestra()

                                                    If DtImp IsNot Nothing Then
                                                        For I_Imp = 0 To UBound(Destinazioni, 2) - 1
                                                            DrImp = DtImp.Select(" Piva='" & Agro_SQL_SaveText(Destinazioni(i_Piva, I_Imp)) & "'" &
                                                                                 " AND Sa_Cod=" & Agro_SQL_SaveNum(Destinazioni(i_Sa_Cod, I_Imp)) &
                                                                                 " AND Appezza=" & Agro_SQL_SaveNum(Destinazioni(i_Appezza, I_Imp)) &
                                                                                 " AND Id_Reg=" & Agro_SQL_SaveNum(Destinazioni(i_Id_Reg, I_Imp)) & "")
                                                            If DrImp IsNot Nothing AndAlso DrImp.Length > 0 Then

                                                                'Impostazione Sup_Imp
                                                                If IsNumeric(DrImp(0).Item("Sup_Imp")) Then
                                                                    Sup_Imp = DrImp(0).Item("Sup_Imp")
                                                                    Sup_Totale = Sup_Totale + DrImp(0).Item("Sup_Imp")
                                                                Else
                                                                    Sup_Imp = 0
                                                                End If

                                                                Veg_Cod = DrImp(0).Item("Veg_Cod")

                                                                If Not IsDBNull(DrImp(0).Item("Grfi_Cod")) Then
                                                                    Grfi_Cod = DrImp(0).Item("Grfi_Cod")
                                                                Else
                                                                    Grfi_Cod = 0
                                                                End If

                                                                If Not IsDBNull(DrImp(0).Item("Gru_Cod")) Then
                                                                    Gru_Cod = DrImp(0).Item("Gru_Cod")
                                                                Else
                                                                    Gru_Cod = 0
                                                                End If

                                                                If Not IsDBNull(DrImp(0).Item("Cop_Cod")) Then
                                                                    Cop_Cod = DrImp(0).Item("Cop_Cod")
                                                                Else
                                                                    Cop_Cod = 0
                                                                End If

                                                                If Not IsDBNull(DrImp(0).Item("Foral_Cod")) Then
                                                                    Foral_Cod = DrImp(0).Item("Foral_Cod")
                                                                Else
                                                                    Foral_Cod = 0
                                                                End If

                                                                If Not IsDBNull(DrImp(0).Item("Stato_Impianto")) Then
                                                                    Stato_Impianto = DrImp(0).Item("Stato_Impianto")
                                                                Else
                                                                    Stato_Impianto = 0
                                                                End If

                                                                'If Destinazioni(i_Sup_Imp, I_Imp) = 0 Then
                                                                Destinazioni(i_Sup_Imp, I_Imp) = Sup_Imp
                                                                'End If

                                                                Destinazioni(i_Cul_Cod, I_Imp) = DrImp(0).Item("Cul_Cod")
                                                                Destinazioni(i_Grfi_Cod, I_Imp) = Grfi_Cod
                                                                Destinazioni(i_Protetto, I_Imp) = Cop_Cod
                                                                Destinazioni(i_Foral_Cod, I_Imp) = Foral_Cod
                                                                Destinazioni(i_Stato, I_Imp) = Stato_Impianto

                                                                Destinazioni(i_Regolamento, I_Imp) = DrImp(0).Item("Regolamento_Cod")
                                                                Destinazioni(i_Validita_Inizio, I_Imp) = DrImp(0).Item("Validita_inizio_Distinta")
                                                                Destinazioni(i_Validita_Fine, I_Imp) = DrImp(0).Item("Validita_fine_Distinta")

                                                                If Disciplinare_Cod <> 0 Then
                                                                    Destinazioni(i_Disciplinare_Cod, I_Imp) = Disciplinare_Cod
                                                                Else
                                                                    Destinazioni(i_Disciplinare_Cod, I_Imp) = DrImp(0).Item("Disciplinare_Cod")
                                                                End If

                                                                '(07/01/2019 fede) aggiunto controllo buffer
                                                                Destinazioni(i_LunghezzaConfine_BufferZone, I_Imp) = DrImp(0).Item("DistBZ_CorpiIdrici") + DrImp(0).Item("DistBZ_AreeResPub") + DrImp(0).Item("DistBZ_Allevamenti") + DrImp(0).Item("DistBZ_VegNatNonColt")

                                                                '(15/10/2020 fede) aggiunto controllo capezzagna
                                                                Destinazioni(i_Offset_UltimaPianta, I_Imp) = DrImp(0).Item("SupBZ_Riduzione")


                                                                'parte per l'aggiunta della data all'xml
                                                                'leggo la raccolta se c'è
                                                                'altrimenti leggo la data presunta di raccolta
                                                                'prima leggo se è registrata un operazione di raccolta
                                                                Dim objMovDest As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                                                                Dim Data_Raccolta_PrevistaoPresunta As Date =
                                                                        objMovDest.Get_DataRaccolta_EfoPre_successiva_data_operazione(
                                                                                    Destinazioni(i_Piva, I_Imp),
                                                                                    CInt(Destinazioni(i_Sa_Cod, I_Imp)),
                                                                                    CInt(Destinazioni(i_Appezza, I_Imp)),
                                                                                    CInt(Destinazioni(i_Id_Reg, I_Imp)),
                                                                                    Data,
                                                                                    "", "", objParametri)
                                                                'mi posso trovare anche il valore nothing
                                                                Destinazioni(i_Data_Raccolta, I_Imp) = Data_Raccolta_PrevistaoPresunta


                                                                '------------------------------------------------

                                                                'lettura fasi fenologiche (da web service sia nuove fasi bbch sia vecchie fasi)
                                                                Dim objParametriUscita_Fasi As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output = Nothing

                                                                If I_Imp = 0 Then

                                                                    Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
                                                                    objParametriIngresso.Veg_Cod = Veg_Cod

                                                                    Dim objAgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig

                                                                    Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS

                                                                    If Not IsNothing(objParametri_Super_Server) AndAlso
                                                                        (IsNothing(objAgroWebConfig.GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali) OrElse
                                                                        objAgroWebConfig.GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali = "") Then

                                                                        Dim xLeggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

                                                                        Dim DTConfigurazioneSiti As DataTable = xLeggiConfigurazioneSiti.Leggi(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)

                                                                        If Not IsNothing(DTConfigurazioneSiti) AndAlso DTConfigurazioneSiti.Rows.Count = 1 AndAlso DTConfigurazioneSiti.Rows(0)("Valore") <> "" Then
                                                                            objParametriIngresso.Url = DTConfigurazioneSiti.Rows(0)("Valore") & "/FasiFenologiche"
                                                                        End If


                                                                    End If

                                                                    'fasi bbch
                                                                    objParametriUscita_Fasi = objFasi_WS.FasiFenologiche(objParametriIngresso)
                                                                    For f = 0 To objParametriUscita_Fasi.ListaFasiFenologiche.Count - 1
                                                                        If objParametriUscita_Fasi.ListaFasiFenologiche(f).Fioritura Then
                                                                            FF_Cod_Fioritura_New = objParametriUscita_Fasi.ListaFasiFenologiche(f).Cod_SS
                                                                            FF_Cod_Fioritura_Old = objParametriUscita_Fasi.ListaFasiFenologiche(f).FF_Cod
                                                                            Exit For
                                                                        End If
                                                                    Next

                                                                End If

                                                                Destinazioni(i_Data_Fioritura, I_Imp) = AGRODATAINIZIO
                                                                Destinazioni(i_FF_Cod, I_Imp) = 0
                                                                Destinazioni(i_FF_Stadio, I_Imp) = ""

                                                                'Impostazione Fase Fenologica
                                                                Dim dtFasiFeno As DataTable
                                                                Dim ObjFasiFeno As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
                                                                dtFasiFeno = ObjFasiFeno.Leggi_FasiFenologiche(Destinazioni(i_Piva, I_Imp),
                                                                                                         CInt(Destinazioni(i_Sa_Cod, I_Imp)),
                                                                                                         CInt(Destinazioni(i_Appezza, I_Imp)),
                                                                                                         CInt(Destinazioni(i_Id_Reg, I_Imp)),
                                                                                        Destinazioni(i_Validita_Inizio, I_Imp), Destinazioni(i_Validita_Fine, I_Imp),
                                                                                                         enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                         "",
                                                                                                         "Data_Ril Desc",
                                                                                                         objParametri)

                                                                If dtFasiFeno.Rows.Count > 0 Then

                                                                    Dim drFioritura As DataRow() = Nothing
                                                                    If FF_Cod_Fioritura_Old <> 0 Then
                                                                        drFioritura = dtFasiFeno.Select("FF_Classe=" & Agro_SQL_SaveNum(FF_Cod_Fioritura_Old))
                                                                    ElseIf FF_Cod_Fioritura_New <> 0 Then
                                                                        drFioritura = dtFasiFeno.Select("FF_Classe=" & Agro_SQL_SaveNum(FF_Cod_Fioritura_New))
                                                                    End If

                                                                    If Not IsNothing(drFioritura) AndAlso drFioritura.Length > 0 Then
                                                                        Destinazioni(i_Data_Fioritura, I_Imp) = drFioritura(0).Item("data_movimento")
                                                                    End If

                                                                    Dim drFaseAttuale As DataRow() = dtFasiFeno.Select("Data_Movimento<='" & Data.ToShortDateString & "' ", "data_movimento desc")
                                                                    If Not IsNothing(drFaseAttuale) AndAlso drFaseAttuale.Length > 0 Then
                                                                        Destinazioni(i_FF_Cod, I_Imp) = drFaseAttuale(0).Item("data_movimento")
                                                                        If objParametriUscita_Fasi IsNot Nothing Then
                                                                            Select Case drFaseAttuale(0).Item("FF_Classe")
                                                                                Case < 1000
                                                                                    Destinazioni(i_FF_Stadio, I_Imp) = (From aa In objParametriUscita_Fasi.ListaFasiFenologiche
                                                                                                                        Where aa.FF_Cod = drFaseAttuale(0).Item("FF_Classe")
                                                                                                                        Select aa.Stadio
                                                                                                                        ).FirstOrDefault
                                                                                Case Else
                                                                                    Destinazioni(i_FF_Stadio, I_Imp) = (From aa In objParametriUscita_Fasi.ListaFasiFenologiche
                                                                                                                        Where aa.Cod_SS = drFaseAttuale(0).Item("FF_Classe")
                                                                                                                        Select aa.Stadio
                                                                                                                        ).FirstOrDefault
                                                                            End Select

                                                                        End If
                                                                    End If

                                                                End If

                                                                If Destinazioni(i_Data_Fioritura, I_Imp) = AGRODATAINIZIO Then
                                                                    'verifico se non ho date di fioritura provo a controllare la data prevista
                                                                    If Not IsDBNull(DrImp(0).Item("data_fioritura_prevista")) AndAlso
                                                                                IsDate(DrImp(0).Item("data_fioritura_prevista")) AndAlso
                                                                                DrImp(0).Item("data_fioritura_prevista") > AGRODATAINIZIO Then
                                                                        Destinazioni(i_Data_Fioritura, I_Imp) = DrImp(0).Item("data_fioritura_prevista")
                                                                    End If
                                                                End If


                                                                '(22/05/2017) aggiunti comuni x verifica deroghe territoriali
                                                                Dim strComuni As String = ""
                                                                Dim DtAppxPart As DataTable
                                                                Dim objAppxPart As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
                                                                DtAppxPart = objAppxPart.LeggiParticelle_Da_Appezzamento(Destinazioni(i_Piva, I_Imp),
                                                                                                                             Destinazioni(i_Sa_Cod, I_Imp),
                                                                                                                             Destinazioni(i_Appezza, I_Imp),
                                                                                                                             enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
                                                                For a = 0 To DtAppxPart.Rows.Count - 1
                                                                    If InStr(strComuni, DtAppxPart.Rows(a).Item("prov") & DtAppxPart.Rows(a).Item("com")) = 0 Then
                                                                        strComuni &= DtAppxPart.Rows(a).Item("prov") & DtAppxPart.Rows(a).Item("com") & ","
                                                                    End If
                                                                Next

                                                                If strComuni = "" Then
                                                                    Dim objCentro As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                                                                    Dim DtCentro As DataTable
                                                                    DtCentro = objCentro.Leggi(Destinazioni(i_Piva, I_Imp), Destinazioni(i_Sa_Cod, I_Imp), enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri)
                                                                    If DtCentro.Rows.Count > 0 Then
                                                                        strComuni &= DtCentro.Rows(0).Item("pro_cod_istat") & DtCentro.Rows(0).Item("com_cod_istat")
                                                                    End If
                                                                Else
                                                                    strComuni = Left(strComuni, strComuni.Length - 1)
                                                                End If

                                                                Destinazioni(i_ListaComuni, I_Imp) = strComuni

                                                            End If
                                                        Next
                                                    End If



                                                End If



                                            End If

                                            'Impostazione Prodotto
                                            Pro_Cod = CInt(xMovimento_Dettaglio.GetAttribute("pro_cod"))

                                            '==================================================================================================================
                                            Select Case bAvversita_Generali
                                                Case True 'Avversità già impostate nella modalità gestita 'nessun disciplinare'

                                                    'Ogni prodotto è imputato ad ogni avversità/infestante

                                                Case False

                                                    'Singole avversità associate ai singoli prodotti (come disciplinare richiede)
                                                    ReDim Avversita(2, 0)

                                                    '====================================================================================
                                                    'Prelevo le informazioni sul dettaglio tecnico associato al singolo dettaglio
                                                    '------------------------------------------------------------------------------------
                                                    xDatiMov_Dettagli_Tecnici = xMovimento_Dettaglio.GetElementsByTagName("Movimento_Dettaglio_Tecnico_2")

                                                    i_Mov_Dettaglio_Tecnico = 0

                                                    Do While i_Mov_Dettaglio_Tecnico < xDatiMov_Dettagli_Tecnici.Count

                                                        'Prelevo l'i-esimo Movimento Dettaglio Tecnico
                                                        xDatiMov_Dettaglio_Tecnico = xDatiMov_Dettagli_Tecnici.Item(i_Mov_Dettaglio_Tecnico)

                                                        ReDim Preserve Avversita(2, UBound(Avversita, 2) + 1)

                                                        Avversita(0, UBound(Avversita, 2) - 1) = CInt(xDatiMov_Dettaglio_Tecnico.GetAttribute("av_cod"))
                                                        Avversita(1, UBound(Avversita, 2) - 1) = CInt(xDatiMov_Dettaglio_Tecnico.GetAttribute("av_gru"))

                                                        Avversita(2, UBound(Avversita, 2) - 1) = 0
                                                        If xDatiMov_Dettaglio_Tecnico.HasAttribute("soglia_cod") AndAlso IsNumeric(xDatiMov_Dettaglio_Tecnico.GetAttribute("soglia_cod")) Then
                                                            Avversita(2, UBound(Avversita, 2) - 1) = CInt(xDatiMov_Dettaglio_Tecnico.GetAttribute("soglia_cod"))
                                                        End If

                                                        i_Mov_Dettaglio_Tecnico = i_Mov_Dettaglio_Tecnico + 1

                                                    Loop

                                            End Select
                                            '==================================================================================================================

                                            i_Mov_Dettaglio_Tecnico = 0

                                            Do While i_Mov_Dettaglio_Tecnico < UBound(Avversita, 2)

                                                '===================================================================
                                                'Lettura dell'avversità associata al prodotto
                                                '===================================================================
                                                Av_Cod = CInt(Avversita(0, i_Mov_Dettaglio_Tecnico))
                                                Av_Gru = CInt(Avversita(1, i_Mov_Dettaglio_Tecnico))


                                                'Inserimento Nuovo Dettaglio
                                                ReDim Preserve Dettagli(13, UBound(Dettagli, 2) + 1)

                                                Dettagli(i_Pro_Cod, UBound(Dettagli, 2) - 1) = Pro_Cod
                                                Dettagli(i_Av_Cod, UBound(Dettagli, 2) - 1) = Av_Cod
                                                Dettagli(i_Av_Gru, UBound(Dettagli, 2) - 1) = Av_Gru
                                                Dettagli(i_soglia_cod, UBound(Dettagli, 2) - 1) = CInt(Avversita(2, i_Mov_Dettaglio_Tecnico))
                                                Dettagli(i_Udm_Cod, UBound(Dettagli, 2) - 1) = CInt(xMovimento_Dettaglio.GetAttribute("extra_int"))
                                                Dettagli(i_Dose, UBound(Dettagli, 2) - 1) = CDbl(xMovimento_Dettaglio.GetAttribute("qta"))

                                                Dettagli(i_Dose_Hl, UBound(Dettagli, 2) - 1) = CDbl(xMovimento_Dettaglio.GetAttribute("qta_extra"))
                                                Dettagli(i_Mezzo_Det, UBound(Dettagli, 2) - 1) = CDbl(xMovimento_Dettaglio.GetAttribute("mezzo_det"))

                                                for_veg_av_dos_cod = "0"
                                                formulatixallegatinormative_idriga = "0"
                                                Dim ArrayDosiValue As String()
                                                Dim d As Integer
                                                If xMovimento_Dettaglio.HasAttribute("doseetichetta_value") AndAlso xMovimento_Dettaglio.GetAttribute("doseetichetta_value") <> "" Then
                                                    ArrayDosiValue = Split(xMovimento_Dettaglio.GetAttribute("doseetichetta_value"), "<br>")
                                                    If ArrayDosiValue IsNot Nothing Then
                                                        For d = 0 To ArrayDosiValue.Length - 1
                                                            for_veg_av_dos_cod &= Split(ArrayDosiValue(d), "$")(0) & ","
                                                            If Split(ArrayDosiValue(d), "$").Length > 19 Then
                                                                formulatixallegatinormative_idriga = Split(ArrayDosiValue(d), "$")(20)
                                                            End If
                                                        Next
                                                    End If
                                                    'elimino la virgola
                                                    If for_veg_av_dos_cod <> "0" Then
                                                        for_veg_av_dos_cod = Left(for_veg_av_dos_cod, for_veg_av_dos_cod.Length - 1)
                                                    End If
                                                    'for_veg_av_dos_cod = Split(xMovimento_Dettaglio.GetAttribute("doseetichetta_value"), "$")(0)
                                                End If
                                                Dettagli(i_for_veg_av_dos_cod, UBound(Dettagli, 2) - 1) = for_veg_av_dos_cod

                                                Dettagli(i_formulatixallegatinormative_idriga, UBound(Dettagli, 2) - 1) = formulatixallegatinormative_idriga

                                                '(07/01/2019 fede) aggiunto controllo buffer
                                                Dettagli(i_buffer, UBound(Dettagli, 2) - 1) = ""
                                                If xMovimento_Dettaglio.HasAttribute("buffer") Then
                                                    Dettagli(i_buffer, UBound(Dettagli, 2) - 1) = xMovimento_Dettaglio.GetAttribute("buffer")
                                                End If

                                                '(18/01/2021 fede) aggiunto controllo della riga di formulatixspecie selezionata
                                                for_veg_cod = "0"
                                                If xMovimento_Dettaglio.HasAttribute("extra_str") AndAlso IsNumeric(xMovimento_Dettaglio.GetAttribute("extra_str")) Then
                                                    for_veg_cod = xMovimento_Dettaglio.GetAttribute("extra_str")
                                                End If
                                                Dettagli(i_for_veg_cod, UBound(Dettagli, 2) - 1) = for_veg_cod

                                                If xMovimento_Dettaglio.HasAttribute("principiattivipercabb") Then
                                                    Dim papa As String = xMovimento_Dettaglio.GetAttribute("principiattivipercabb")
                                                    If Not String.IsNullOrEmpty(papa) Then
                                                        arrayPrincipiAttivi.Add(papa)
                                                    End If

                                                End If

                                                i_Mov_Dettaglio_Tecnico += 1

                                            Loop

                                            i_DatiMovimento_Dettaglio += 1

                                        Loop

                                        i_DatiMovimenti_Dettagli += 1

                                    Loop

                                End If

                                i_Movimento += 1

                            Loop

                            i_DatiMovimento += 1

                        Loop

                    End If

                    i_Agenda += 1

                Loop

                i_DatiAgenda += 1

            Loop


            '######################################################################################################
            '################### CREAZIONE STRINGA XML  ###########################################################
            '######################################################################################################

            '======================================================================================================
            'Dati Generali Intervento
            XmlDatiGenerali.SetAttribute("TipoOperazioneDB", TipoOperazioneDB)
            XmlDatiGenerali.SetAttribute("piva", PIVA)
            XmlDatiGenerali.SetAttribute("sa_cod", Sa_Cod)
            XmlDatiGenerali.SetAttribute("data_movimento", Data.ToShortDateString)
            XmlDatiGenerali.SetAttribute("lav_cod", Lav_Cod)
            XmlDatiGenerali.SetAttribute("id_agenda", Id_Agenda)
            XmlDatiGenerali.SetAttribute("des_lib", Des_Lib)
            XmlDatiGenerali.SetAttribute("tipotestata", TipoTestata)
            XmlDatiGenerali.SetAttribute("modulo", mModulo)
            XmlDatiGenerali.SetAttribute("epoca", mEp_Cod)
            If H2O_Totale <> 0 AndAlso H2O = 0 Then
                H2O = H2O_Totale / Sup_Operazione
            End If
            XmlDatiGenerali.SetAttribute("h2o", H2O)
            XmlDatiGenerali.SetAttribute("h2o_totale", H2O_Totale)
            XmlDatiGenerali.SetAttribute("gru_cod", Gru_Cod)
            XmlDatiGenerali.SetAttribute("veg_cod", Veg_Cod)
            XmlDatiGenerali.SetAttribute("mezzo", Mezzo)
            XmlDatiGenerali.SetAttribute("tipo_distribuzione_h2o", Tipo_Distribuzione_H2O)
            XmlDatiGenerali.SetAttribute("sup_totale", Sup_Totale)

            If Biologico Then
                XmlDatiGenerali.SetAttribute("biologico", 1)
            Else
                XmlDatiGenerali.SetAttribute("biologico", 0)
            End If

            '(06/07/2017 fede) aggiunti dati dpi operazione
            XmlDatiGenerali.SetAttribute("id_rcdpi", Id_Rcdpi)
            XmlDatiGenerali.SetAttribute("disciplinare_cod", Disciplinare_Cod)


            ' 2022-03-16 belmonte - aggiunta flag per chiamata vecchio / nuovo controllo Difesa Diserbo
            If TipoOperazioneDB <> enum_TipoOperazioneDB.Modifica Then
                'vince il flag
                XmlDatiGenerali.SetAttribute("flag_nuovo_controllo_riduzione_diserbo", nuovoControlloRiduzioneDiserbo)
            Else
                If IsNothing(xMovimenti_Dettagli) OrElse (xMovimenti_Dettagli.Count = 1 AndAlso dtDosiVuoto) Then
                    ' non ho dettagli sotto -> vince il flag
                    XmlDatiGenerali.SetAttribute("flag_nuovo_controllo_riduzione_diserbo", nuovoControlloRiduzioneDiserbo)
                Else
                    ' ho dettagli sotto e almeno un principio attivo valorizzato -> True
                    If nuovoControlloRiduzioneDiserbo AndAlso arrayPrincipiAttivi.Any Then
                        XmlDatiGenerali.SetAttribute("flag_nuovo_controllo_riduzione_diserbo", True)
                    Else
                        XmlDatiGenerali.SetAttribute("flag_nuovo_controllo_riduzione_diserbo", False)
                    End If

                End If
            End If

            '======================================================================================================
            'Inserimento Impianti
            XmlDatiImpianti = XmlDoc.CreateElement("DatiReg_Impianti")

            For i = 0 To UBound(Destinazioni, 2) - 1

                XmlDatiImpianto = XmlDoc.CreateElement("DatiReg_Impianto")

                XmlDatiImpianto.SetAttribute("piva", Destinazioni(i_Piva, i))
                XmlDatiImpianto.SetAttribute("sa_cod", Destinazioni(i_Sa_Cod, i))
                XmlDatiImpianto.SetAttribute("appezza", Destinazioni(i_Appezza, i))
                XmlDatiImpianto.SetAttribute("id_reg", Destinazioni(i_Id_Reg, i))
                XmlDatiImpianto.SetAttribute("lotto", Destinazioni(i_Lotto, i))
                XmlDatiImpianto.SetAttribute("regolamento", Destinazioni(i_Regolamento, i))
                XmlDatiImpianto.SetAttribute("cul_cod", Destinazioni(i_Cul_Cod, i))
                XmlDatiImpianto.SetAttribute("disciplinare_cod", Destinazioni(i_Disciplinare_Cod, i))
                XmlDatiImpianto.SetAttribute("sup_trattata", Destinazioni(i_Sup_Trattata, i))
                XmlDatiImpianto.SetAttribute("sup_imp", Destinazioni(i_Sup_Imp, i))
                XmlDatiImpianto.SetAttribute("grfi_cod", Destinazioni(i_Grfi_Cod, i))
                XmlDatiImpianto.SetAttribute("stato_impianto", Destinazioni(i_Stato, i))
                XmlDatiImpianto.SetAttribute("foral_cod", Destinazioni(i_Foral_Cod, i))
                XmlDatiImpianto.SetAttribute("cop_cod", Destinazioni(i_Protetto, i))
                XmlDatiImpianto.SetAttribute("ff_cod", Destinazioni(i_FF_Cod, i))
                XmlDatiImpianto.SetAttribute("ff_stadio", Destinazioni(i_FF_Stadio, i))
                XmlDatiImpianto.SetAttribute("validita_inizio", Destinazioni(i_Validita_Inizio, i))
                XmlDatiImpianto.SetAttribute("validita_fine", Destinazioni(i_Validita_Fine, i))
                XmlDatiImpianto.SetAttribute("data_raccolta", Destinazioni(i_Data_Raccolta, i))
                XmlDatiImpianto.SetAttribute("lista_comuni", Destinazioni(i_ListaComuni, i))
                XmlDatiImpianto.SetAttribute("data_fioritura", Destinazioni(i_Data_Fioritura, i))
                XmlDatiImpianto.SetAttribute("lunghezza_confine_bufferzone", Destinazioni(i_LunghezzaConfine_BufferZone, i))
                XmlDatiImpianto.SetAttribute("sup_riduzione_bufferzone", Destinazioni(i_Sup_Riduzione_BufferZone, i))
                XmlDatiImpianto.SetAttribute("perc_riduzione_deriva", Destinazioni(i_Perc_Riduzione_Deriva, i))
                XmlDatiImpianto.SetAttribute("offset_ultima_pianta", Destinazioni(i_Offset_UltimaPianta, i))

                XmlDatiImpianti.AppendChild(XmlDatiImpianto)

            Next i

            XmlDatiGenerali.AppendChild(XmlDatiImpianti)

            '======================================================================================================
            'Inserimento Dettagli
            XmlDatiDettagli = XmlDoc.CreateElement("DatiDettagli")

            For i = 0 To UBound(Dettagli, 2) - 1

                XmlDatiDettaglio = XmlDoc.CreateElement("Dettaglio")

                XmlDatiDettaglio.SetAttribute("pro_cod", Dettagli(i_Pro_Cod, i))
                XmlDatiDettaglio.SetAttribute("av_cod", Dettagli(i_Av_Cod, i))
                XmlDatiDettaglio.SetAttribute("av_gru", Dettagli(i_Av_Gru, i))
                XmlDatiDettaglio.SetAttribute("udm_cod", Dettagli(i_Udm_Cod, i))
                XmlDatiDettaglio.SetAttribute("dose", Dettagli(i_Dose, i))
                XmlDatiDettaglio.SetAttribute("for_veg_av_dos_cod", Dettagli(i_for_veg_av_dos_cod, i))
                XmlDatiDettaglio.SetAttribute("for_veg_cod", Dettagli(i_for_veg_cod, i))
                XmlDatiDettaglio.SetAttribute("formulatixallegatinormative_idriga", Dettagli(i_formulatixallegatinormative_idriga, i))
                XmlDatiDettaglio.SetAttribute("buffer", Dettagli(i_buffer, i))
                XmlDatiDettaglio.SetAttribute("soglia_cod", Dettagli(i_soglia_cod, i))

                XmlDatiDettaglio.SetAttribute("dose_hl", Dettagli(i_Dose_Hl, i))
                XmlDatiDettaglio.SetAttribute("mezzo_det", Dettagli(i_Mezzo_Det, i))


                XmlDatiDettagli.AppendChild(XmlDatiDettaglio)

            Next i

            XmlDatiGenerali.AppendChild(XmlDatiDettagli)

            '========================================================================

            XmlDatiWS.AppendChild(XmlDatiGenerali)

            XmlDoc.AppendChild(XmlDatiWS)

            Return XmlDoc.OuterXml

        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        Finally

            If FlagConnessioneLocale Then
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

    End Function


    Public Function DPI_Verifica_Concimazione(ByRef objParametri_Server As AgronicaCoreParametri,
                                              ByRef objParametri_Utenti As AgronicaCoreParametri,
                                              ByVal DatiAgenda As String,
                                              ByVal Id_Agenda_Escluso As Long,
                                              ByVal Disciplinare_Cod As Integer,
                                              ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
                                                Optional ByVal Piva_Riferimento As String = "",
                                                Optional ByVal Sa_Cod_Riferimento As Integer = 0,
                                                Optional ByVal Appezza_Riferimento As Integer = 0,
                                                Optional ByVal Id_Reg_Riferimento As Integer = 0,
                                                Optional ByVal objParametri_Super_Server As AgronicaCoreParametri = Nothing,
                                                Optional paramVerificaDPIMultiAttivita As AgronicaCoreModello.VerificaDPIMultiAttivita = Nothing
                                              ) As rispostaStandard(Of Verifica_Disciplinare_Intervento)


        Dim r As New rispostaStandard(Of Verifica_Disciplinare_Intervento)
        r.RispostaStringa = New Verifica_Disciplinare_Intervento

        Dim DatiVerifica As String

        Try

            Dim Dpi_Cod As Integer = 0
            Dim Dpi_Cod_Op As Integer = 0
            Dim Biologico As Boolean

            If IsNumeric(Disciplinare_Cod) AndAlso CInt(Disciplinare_Cod) = enum_Disciplinare_Operazione.QuelloDellOperazione Then

                Dim XmlDom As New XmlDocument
                Dim xDatiAgende As XmlNodeList
                Dim xDatiAgenda As XmlElement
                Dim xAgende As XmlNodeList
                Dim xAgenda As XmlElement
                Dim xDatiMovimenti As XmlNodeList
                Dim xDatiMovimento As XmlElement
                Dim xMovimenti As XmlNodeList
                Dim xMovimento As XmlElement

                XmlDom.LoadXml(DatiAgenda)
                xDatiAgende = XmlDom.GetElementsByTagName("DatiAgenda")

                For i_DatiAgenda = 0 To xDatiAgende.Count - 1
                    xDatiAgenda = xDatiAgende.Item(i_DatiAgenda)
                    xAgende = xDatiAgenda.GetElementsByTagName("Agenda")
                    For i_Agenda = 0 To xAgende.Count - 1
                        xAgenda = xAgende.Item(i_Agenda)
                        xDatiMovimenti = xAgenda.GetElementsByTagName("DatiMovimenti")
                        For i_DatiMovimento = 0 To xDatiMovimenti.Count - 1
                            xDatiMovimento = xDatiMovimenti.Item(i_DatiMovimento)
                            xMovimenti = xDatiMovimento.GetElementsByTagName("Movimento")
                            For i_Movimento = 0 To xMovimenti.Count - 1
                                xMovimento = xMovimenti.Item(i_Movimento)
                                Select Case xMovimento.GetAttribute("cau_mov")
                                    Case CAU_TRATTAMENTO, CAU_RILIEVO_CAMPO, CAU_RILIEVO_RACCOLTA, CAU_LAVORAZIONE
                                        Dpi_Cod_Op = CInt(xMovimento.GetAttribute("num_protocollo"))
                                        If xMovimento.HasAttribute("disciplinare_pubblicoprivato") AndAlso
                                            xMovimento.GetAttribute("disciplinare_pubblicoprivato") = 2 Then
                                            Dpi_Cod_Op = -CInt(xMovimento.GetAttribute("num_protocollo"))
                                        End If
                                End Select
                            Next
                        Next
                    Next
                Next

            End If


            Select Case Disciplinare_Cod
                Case enum_Disciplinare_Operazione.Nessuno
                    Dpi_Cod = 0
                Case enum_Disciplinare_Operazione.Biologico
                    Dpi_Cod = -2
                    Biologico = True
                'Case enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta
                '    Dpi_Cod = 0
                Case enum_Disciplinare_Operazione.QuelloDellOperazione
                    Select Case Dpi_Cod_Op
                        Case enum_Disciplinare_Operazione.Nessuno
                            Dpi_Cod = 0
                        Case enum_Disciplinare_Operazione.Biologico
                            Dpi_Cod = -2
                            Biologico = True
                        Case enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta
                            Dpi_Cod = 0
                        Case Else
                            Dpi_Cod = Dpi_Cod_Op
                    End Select
                Case Else
                    'dpi
                    Dim array As String()
                    array = Split(Disciplinare_Cod, "/")
                    Dpi_Cod = CInt(array(0))
            End Select


            DatiVerifica = Verifica_Concimazione(CStr(DatiAgenda),
                                                 CInt(Id_Agenda_Escluso),
                                                 Dpi_Cod,
                                                 CStr(Piva_Riferimento),
                                                 CInt(Sa_Cod_Riferimento),
                                                 CInt(Appezza_Riferimento),
                                                 CInt(Id_Reg_Riferimento),
                                                 objParametri_Server,
                                                 objParametri_Utenti,
                                                 objParametri_Super_Server:=objParametri_Super_Server,
                                                 paramVerificaDPIMultiAttivita:=paramVerificaDPIMultiAttivita)

            Dim rRis As New rispostaStandard(Of Verifica_Disciplinare_Intervento)
            rRis = XmlFINALE(CStr(DatiVerifica),
                                        CInt(Id_Agenda_Escluso),
                                        VerificaSoloControlliImpostazioniUtente,
                                        objParametri_Server,
                                        objParametri_Utenti,
                                        DatiAgenda,
                                         Biologico,
                                         Disciplinare_Cod)

            If rRis.RispostaOK Then
                r.RispostaOK = True
                r.RispostaStringa.Risultato = rRis.RispostaStringa.Risultato
            Else
                Throw New Exception(rRis.Errore)
            End If


        Catch exc As Exception

            r.RispostaOK = False
            r.Errore = exc.Message

        End Try

        Return r

    End Function

    Public Function Verifica_Concimazione(ByVal DatiAgenda As String,
                                          ByVal Id_Agenda_Concimazione_Escluso As Integer,
                                          ByVal Disciplinare_Cod As Integer,
                                          ByVal Piva_Riferimento As String,
                                          ByVal Sa_Cod_Riferimento As Integer,
                                          ByVal Appezza_Riferimento As Integer,
                                          ByVal Id_Reg_Riferimento As Integer,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreParametri,
                                            Optional ByVal objParametri_Super_Server As AgronicaCoreParametri = Nothing,
                                            Optional paramVerificaDPIMultiAttivita As AgronicaCoreModello.VerificaDPIMultiAttivita = Nothing
                                          ) As String
        '============================================================================

        Dim NomeRoutine As String = "DpiBIZ.DPI_Consultazione.Verifica_Concimazione"

        Dim MessaggioErrore As String = ""

        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction

        Dim i As Int32

        Dim RisultatoFunzione As String

        Dim XmlDoc As New XmlDocument
        Dim XmlDom As New XmlDocument

        Dim XmlDatiRisultati As XmlElement
        Dim XmlDatiGenerali As XmlElement
        Dim XmlDatiNonConformi As XmlElement


        Dim PIVA As String = ""
        Dim Sa_Cod As Integer
        Dim Lav_Cod As Integer
        Dim Id_Agenda As Integer
        Dim Des_Lib As String = ""
        Dim Data As Date
        Dim Veg_Cod As Integer
        Dim Veg_Des As String = ""
        Dim Grfi_Cod As Integer
        Dim Raccoglitore_Cod As Integer
        'Dim H2O As Decimal
        Dim TipoTestata As Integer
        Dim Mezzo As Integer
        Dim TipoOperazioneDB As Integer
        Dim Superficie As Decimal

        Dim xDatiAgenda As XmlElement
        Dim xAgenda As XmlElement
        Dim xDatiAgende As XmlNodeList
        Dim xAgende As XmlNodeList

        Dim xDatiMovimenti As XmlNodeList
        Dim xDatiMovimento As XmlElement
        Dim xMovimenti As XmlNodeList
        Dim xMovimento As XmlElement

        Dim xDatiMovimenti_Dettagli As XmlNodeList
        Dim xDatiMovimento_Dettaglio As XmlElement
        Dim xMovimenti_Dettagli As XmlNodeList
        Dim xMovimento_Dettaglio As XmlElement

        Dim xMov_Destinazioni As XmlNodeList
        Dim xMov_Destinazione As XmlElement

        Dim xMov_Dettagli_Tecnici_2 As XmlNodeList
        Dim xMov_Dettaglio_Tecnico_2 As XmlElement

        'Dim xDatiMov_Dettagli_Tecnici As XmlNodeList
        'Dim xDatiMov_Dettaglio_Tecnico As XmlElement

        Dim i_DatiAgenda As Integer
        Dim i_Agenda As Integer
        Dim i_DatiMovimento As Integer
        Dim i_Movimento As Integer
        Dim i_DatiMovimenti_Dettagli As Integer
        Dim i_DatiMovimento_Dettaglio As Integer
        'Dim i_Mov_Dettaglio_Tecnico As Integer
        Dim i_Mov_Dettaglio_Tecnico_2 As Integer
        Dim i_DatiMov_Destinazioni As Integer

        Dim Destinazioni As String(,) = Nothing
        Dim DatiInseriti As String(,) = Nothing


        Dim bGestione_Verifica As Boolean
        Dim bScheda As Boolean
        Dim bDestinazionePresente As Boolean

        Dim Indice As Integer

        Dim Codice_Piva As Integer = 0
        Dim Codice_SaCod As Integer = 1
        Dim Codice_Appezza As Integer = 2
        Dim Codice_IdReg As Integer = 3

        Dim Codice_ProgettoNome As Integer = 4
        Dim Codice_SupImp As Integer = 5
        Dim Codice_CulCod As Integer = 6

        Dim Codice_N As Integer = 7
        Dim Codice_P As Integer = 8
        Dim Codice_K As Integer = 9
        Dim Codice_Mg As Integer = 10

        Dim Codice_N_Distribuito As Integer = 11
        Dim Codice_P_Distribuito As Integer = 12
        Dim Codice_K_Distribuito As Integer = 13
        Dim Codice_Mg_Distribuito As Integer = 14

        Dim Codice_ProgettoCod As Integer = 15

        Dim Codice_ValiditaInizio As Integer = 16
        Dim Codice_ValiditaFine As Integer = 17

        Dim Codice_Cu_Distribuito As Integer = 18

        Dim Codice_Regolamento_Concimazioni_Cod As Integer = 19

        Dim Codice_GrfiCod As Integer = 20

        Dim N_Prodotto As Decimal
        Dim P_Prodotto As Decimal
        Dim K_Prodotto As Decimal
        Dim Mg_Prodotto As Decimal
        Dim Efficienza As Decimal
        Dim Cu_Prodotto As Decimal

        Dim bImpianto_Coerente As Boolean

        Dim ObjImpianti As AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim DtImpianti As DataTable
        'Dim DtDistinta As DataTable

        Dim Err_Cod As enTipoErrCode_Verifica
        Dim Err_Des As String


        Dim IDTestataTemp As Integer = -1

        Try

            '------------------------------
            'Verifico se è stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
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

            XmlDom.LoadXml(DatiAgenda)

            XmlDatiRisultati = XmlDoc.CreateElement("DatiRisultati")

            '----- < DatiGenerali > ----
            XmlDatiGenerali = XmlDoc.CreateElement("DatiGenerali")
            XmlDatiNonConformi = XmlDoc.CreateElement("DatiNonConformi")


            '############################################################################################
            '################# Lettura dei Parametri Base di Agenda     #################################
            '############################################################################################

            xDatiAgende = XmlDom.GetElementsByTagName("DatiAgenda")

            i_DatiAgenda = 0

            Do While i_DatiAgenda < xDatiAgende.Count

                'Prelevo l'i-esimo blocco di DatiAgenda (in realtà ne esiste uno solo)
                xDatiAgenda = xDatiAgende.Item(i_DatiAgenda)

                '------------------------------

                xAgende = xDatiAgenda.GetElementsByTagName("Agenda")

                i_Agenda = 0

                Do While i_Agenda < xAgende.Count

                    'Prelevo l' i-esima Codifica Agenda
                    xAgenda = xAgende.Item(i_Agenda)

                    ReDim Destinazioni(20, 0)
                    ReDim DatiInseriti(10, 0)

                    'Parametri agenda
                    TipoOperazioneDB = CInt(xAgenda.GetAttribute("TipoOperazioneDB"))

                    PIVA = xAgenda.GetAttribute("piva")
                    Sa_Cod = CInt(xAgenda.GetAttribute("sa_cod"))
                    Lav_Cod = CInt(xAgenda.GetAttribute("lav_cod"))
                    Id_Agenda = CInt(xAgenda.GetAttribute("id_agenda"))
                    Raccoglitore_Cod = CInt(xAgenda.GetAttribute("raccoglitore_cod"))
                    Des_Lib = xAgenda.GetAttribute("des_lib")
                    Veg_Cod = -1


                    Select Case Lav_Cod


                        Case LAVCOD_TRATTAMENTO_ANTIBUTTERATURA, LAVCOD_CONCIMAZIONE_FOGLIARE,
                             LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_FERTIRRIGAZIONE, LAVCOD_SARCHIATURA_CONCIMAZIONE
                            '106 TRATTAMENTO ANTIBUTTERATURA, 123 CONCIMAZIONE FOGLIARE,
                            '124 DISTRIBUZIONE AMMENDANTI ORGANICI, 14 DISTRIBUZIONE CONCIME IN PIENO CAMPO,
                            '26 FERTIRRIGAZIONE, 156 SARCHIATURA

                            bGestione_Verifica = True


                        Case Else 'Lavorazione non verificabile

                            bGestione_Verifica = False

                    End Select


                    If bGestione_Verifica Then

                        '############################################################################################
                        '################# Lettura della Scheda di Lavorazione della Lavorazione     ################
                        '############################################################################################

                        xDatiMovimenti = xAgenda.GetElementsByTagName("DatiMovimenti")

                        i_DatiMovimento = 0

                        'Check di esistenza Informazioni
                        Do While i_DatiMovimento < xDatiMovimenti.Count

                            xDatiMovimento = xDatiMovimenti.Item(i_DatiMovimento)

                            xMovimenti = xDatiMovimento.GetElementsByTagName("Movimento")

                            i_Movimento = 0

                            Do While i_Movimento < xMovimenti.Count

                                xMovimento = xMovimenti.Item(i_Movimento)

                                'Verifico che il movimento sia la scheda di lavorazione

                                Select Case xMovimento.GetAttribute("cau_mov")

                                    Case "2050", "2100", "2200", "2300" 'Trattamenti, Rilievi in campo, Rilievi alla raccolta, Lavorazioni

                                        'Verifico che sia stato impostato 1 disciplinare
                                        'Disciplinare_Cod = CInt(xMovimento.GetAttribute("num_protocollo"))

                                        'If Disciplinare_Cod > 0 Then
                                        bScheda = True

                                        'Else
                                        '   bScheda = False 'Disciplinare non impostato
                                        'End If

                                    Case Else

                                        bScheda = False

                                End Select

                                '====================================================================================

                                If bScheda Then

                                    '------------------------------

                                    '====================================================================================
                                    'Prelevo le Informazioni sulle Note, sulla Data di Operazione e sull'Username creazione
                                    '------------------------------------------------------------------------------------
                                    Data = CDate(xMovimento.GetAttribute("data_movimento")).ToShortDateString

                                    Mezzo = CInt(xMovimento.GetAttribute("mezzo"))

                                    'mNote = xMovimento.getAttribute("mov_desc")

                                    ''====================================================================================
                                    ''Prelevo le Informazioni dell'Acqua associata alla lavorazione
                                    ''------------------------------------------------------------------------------------
                                    'xDatiMov_Dettagli_Tecnici = xMovimento.GetElementsByTagName("Movimento_Dettaglio_Tecnico")

                                    'i_Mov_Dettaglio_Tecnico = 0

                                    'Do While i_Mov_Dettaglio_Tecnico < xDatiMov_Dettagli_Tecnici.Count

                                    '    'Prelevo l'i-esimo dettaglio tecnico
                                    '    xDatiMov_Dettaglio_Tecnico = xDatiMov_Dettagli_Tecnici.Item(i_Mov_Dettaglio_Tecnico)


                                    '    If CInt(xDatiMov_Dettaglio_Tecnico.GetAttribute("av_cod")) <> 0 Or
                                    '       CInt(xDatiMov_Dettaglio_Tecnico.GetAttribute("av_gru")) <> 0 Then

                                    '        'Eccezione: le avversità non dovrebbero trovarsi in una concimazione

                                    '    ElseIf CDbl(Replace(xDatiMov_Dettaglio_Tecnico.GetAttribute("qta_ril"), ".", ",")) <> 0 Then

                                    '        H2O = CDbl(Replace(xDatiMov_Dettaglio_Tecnico.GetAttribute("qta_ril"), ".", ","))

                                    '    End If


                                    '    i_Mov_Dettaglio_Tecnico = i_Mov_Dettaglio_Tecnico + 1

                                    'Loop


                                    Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                                    'IDTestataTemp = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("IDTestataTemp", objParametri)
                                    IDTestataTemp = xAgrosequenze.NuovoId_Tabella("IDTestataTemp", 0, 20000000, objParametri)

                                    '====================================================================================
                                    'Prelevo le Informazioni sugli Impianti, I Prodotti e le Quantità Distribuite
                                    '------------------------------------------------------------------------------------
                                    xDatiMovimenti_Dettagli = xMovimento.GetElementsByTagName("DatiMovimenti_Dettagli")

                                    i_DatiMovimenti_Dettagli = 0

                                    Do While i_DatiMovimenti_Dettagli < xDatiMovimenti_Dettagli.Count

                                        'Prelevo l'i-esimo Blocco di Movimenti Dettagli (ne esiste 1 solo)
                                        xDatiMovimento_Dettaglio = xDatiMovimenti_Dettagli.Item(i_DatiMovimenti_Dettagli)

                                        xMovimenti_Dettagli = xDatiMovimento_Dettaglio.GetElementsByTagName("Movimento_Dettaglio")

                                        i_DatiMovimento_Dettaglio = 0



                                        Do While i_DatiMovimento_Dettaglio < xMovimenti_Dettagli.Count

                                            xMovimento_Dettaglio = xMovimenti_Dettagli.Item(i_DatiMovimento_Dettaglio)

                                            ' fix per escludere i movimenti dettagli irrigazione dalla verifica conformità
                                            If Lav_Cod = CostantiPersonalizzate.LAVCOD_FERTIRRIGAZIONE AndAlso
                                                CInt(xMovimento_Dettaglio.GetAttribute("elem_cod")) = CostantiPersonalizzate.ALTRE_MATERIE AndAlso
                                                CInt(xMovimento_Dettaglio.GetAttribute("mat_cod")) = CostantiPersonalizzate.MAT_COD_ACQUA_IRRIGAZIONE Then
                                                i_DatiMovimento_Dettaglio += 1
                                                Continue Do
                                            End If

                                            '(09/10/2017 fede) aggiunto controllo prodotto bio in caso si voglia verificare il regolamento bio
                                            If Disciplinare_Cod = enum_Disciplinare_Operazione.Biologico Then

                                                Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.Fertilizzanti_input
                                                objParametriIngresso.Codice = CInt(xMovimento_Dettaglio.GetAttribute("pro_cod"))

                                                Dim objAgroWebConfig As New AgroWebConfig
                                                If IsNothing(objAgroWebConfig.GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti) Then
                                                    Dim agroWs As String
                                                    Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                                                    agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti", "", "", objParametri)
                                                    If agroWs = "" Then
                                                        agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti", "", "", objParametri_Super_Server)
                                                    End If
                                                    objParametriIngresso.Url = agroWs & "/Fertilizzanti"
                                                    objConfigurazione_Siti = Nothing
                                                End If

                                                Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output
                                                Dim objFert_WS As New AgronicaCoreWebService.Fertilizzanti_WS

                                                objParametriUscita = objFert_WS.Fertilizzanti(objParametriIngresso)

                                                If objParametriUscita.ListaFertilizzanti.Count > 0 AndAlso objParametriUscita.ListaFertilizzanti(0).Biologico = False Then

                                                    Err_Cod = enTipoErrCode_Verifica.ProdottoNonBiologico
                                                    Err_Des = "Il Fertilizzante utilizzato (" & objParametriUscita.ListaFertilizzanti(0).Descrizione & ") non è Biologico. "

                                                    Inserisci_NonConformita(DatiInseriti,
                                                                             XmlDoc,
                                                                             XmlDatiNonConformi,
                                                                             Err_Cod,
                                                                             Err_Des,
                                                                             "",
                                                                             CInt(xMovimento_Dettaglio.GetAttribute("pro_cod")),
                                                                             0,
                                                                             0,
                                                                             "",
                                                                             "",
                                                                             0,
                                                                             0,
                                                                             "", "", "",
                                                                             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)

                                                End If


                                            End If






                                            i_Mov_Dettaglio_Tecnico_2 = 0

                                            N_Prodotto = 0
                                            P_Prodotto = 0
                                            K_Prodotto = 0
                                            Mg_Prodotto = 0
                                            Efficienza = 1
                                            Cu_Prodotto = 0

                                            xMov_Dettagli_Tecnici_2 = xMovimento_Dettaglio.GetElementsByTagName("Movimento_Dettaglio_Tecnico_2")

                                            If xMov_Dettagli_Tecnici_2 IsNot Nothing Then

                                                Do While i_Mov_Dettaglio_Tecnico_2 < xMov_Dettagli_Tecnici_2.Count

                                                    'Prelevo l'i-esimo Movimento Dettaglio
                                                    xMov_Dettaglio_Tecnico_2 = xMov_Dettagli_Tecnici_2.Item(i_Mov_Dettaglio_Tecnico_2)

                                                    N_Prodotto = xMov_Dettaglio_Tecnico_2.GetAttribute("n")
                                                    P_Prodotto = xMov_Dettaglio_Tecnico_2.GetAttribute("p")
                                                    K_Prodotto = xMov_Dettaglio_Tecnico_2.GetAttribute("k")
                                                    Mg_Prodotto = xMov_Dettaglio_Tecnico_2.GetAttribute("mg")
                                                    Efficienza = xMov_Dettaglio_Tecnico_2.GetAttribute("efficienza")
                                                    Cu_Prodotto = xMov_Dettaglio_Tecnico_2.GetAttribute("cu")

                                                    i_Mov_Dettaglio_Tecnico_2 += 1

                                                Loop

                                            End If

                                            'Caso vecchie concimazioni 
                                            'e concimazioni LAN
                                            'gli apporti non sono editabili nell'operazione
                                            'non sono salvati nel mov_dettaglio_tecnico

                                            If N_Prodotto = 0 AndAlso P_Prodotto = 0 AndAlso K_Prodotto = 0 AndAlso Mg_Prodotto = 0 AndAlso Cu_Prodotto = 0 Then

                                                Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.Fertilizzanti_input
                                                objParametriIngresso.Codice = CInt(xMovimento_Dettaglio.GetAttribute("pro_cod"))
                                                objParametriIngresso.IncludiApporti = True

                                                Dim objAgroWebConfig As New AgroWebConfig
                                                If IsNothing(objAgroWebConfig.GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti) Then
                                                    Dim agroWs As String
                                                    Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                                                    agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti", "", "", objParametri)
                                                    If agroWs = "" Then
                                                        agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti", "", "", objParametri_Super_Server)
                                                    End If
                                                    objParametriIngresso.Url = agroWs & "/Fertilizzanti"
                                                    objConfigurazione_Siti = Nothing
                                                End If

                                                Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output
                                                Dim objFert_WS As New AgronicaCoreWebService.Fertilizzanti_WS
                                                objParametriUscita = objFert_WS.Fertilizzanti(objParametriIngresso)

                                                For i = 0 To objParametriUscita.ListaFertilizzanti.Count - 1
                                                    N_Prodotto = objParametriUscita.ListaFertilizzanti(i).N
                                                    P_Prodotto = objParametriUscita.ListaFertilizzanti(i).P2O5
                                                    K_Prodotto = objParametriUscita.ListaFertilizzanti(i).K2O
                                                    Mg_Prodotto = objParametriUscita.ListaFertilizzanti(i).MgO
                                                    Cu_Prodotto = objParametriUscita.ListaFertilizzanti(i).Cu
                                                Next



                                                'Dim objFert As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
                                                'Dim DT_Fert As DataTable

                                                'DT_Fert = objFert.Leggi_Completa(CInt(xMovimento_Dettaglio.GetAttribute("pro_cod")),
                                                '                                 "",
                                                '                                0,
                                                '                                True,
                                                '                                PIVA,
                                                '                                CInt(xMovimento_Dettaglio.GetAttribute("mat_cod")),
                                                '                                AGRODATAINIZIO,
                                                '                                AGRODATAFINE,
                                                '                                "",
                                                '                                "",
                                                '                                objParametri)

                                                'If Not IsNothing(DT_Fert) AndAlso DT_Fert.Rows.Count > 0 Then
                                                '    N_Prodotto = DT_Fert.Rows(0).Item("N")
                                                '    P_Prodotto = DT_Fert.Rows(0).Item("P2O5")
                                                '    K_Prodotto = DT_Fert.Rows(0).Item("K2O")
                                                '    Mg_Prodotto = DT_Fert.Rows(0).Item("MgO")
                                                'End If

                                            End If

                                            '==========================================================================
                                            'Lettura della specie vegetale e dei dettagli degli impianti
                                            '-------------------------------------------------------------------------

                                            xMov_Destinazioni = xMovimento_Dettaglio.GetElementsByTagName("Movimento_Destinazione")

                                            i_DatiMov_Destinazioni = 0

                                            Do While i_DatiMov_Destinazioni < xMov_Destinazioni.Count

                                                'Prelevo l'i-esimo Movimento Dettaglio
                                                xMov_Destinazione = xMov_Destinazioni.Item(i_DatiMov_Destinazioni)

                                                If CLng(xMov_Destinazione.GetAttribute("tipo_destinazione")) = 0 Then

                                                    'Verifico che l'impianto sia coerente con quello eventualmente passato come riferimento
                                                    If Trim(Piva_Riferimento) <> "" AndAlso Sa_Cod_Riferimento <> 0 AndAlso
                                                       Appezza_Riferimento <> 0 AndAlso Id_Reg_Riferimento <> 0 AndAlso
                                                       (Appezza_Riferimento <> CInt(xMov_Destinazione.GetAttribute("appezza")) OrElse
                                                       Id_Reg_Riferimento <> CInt(xMov_Destinazione.GetAttribute("id_destinazione"))) Then

                                                        'è stato impostato un impianto come riferimento ed è diverso da quello di lavorazione
                                                        bImpianto_Coerente = False

                                                    Else 'Ok
                                                        bImpianto_Coerente = True

                                                    End If

                                                    If bImpianto_Coerente Then

                                                        '========================================================================
                                                        'La prima volta impostazione del mVeg_Cod
                                                        '------------------------------------------------------------------------
                                                        If Veg_Cod = -1 Then

                                                            ObjImpianti = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

                                                            DtImpianti = ObjImpianti.Leggi(CStr(xMov_Destinazione.GetAttribute("piva")),
                                                                                            CInt(xMov_Destinazione.GetAttribute("sa_cod")),
                                                                                            CInt(xMov_Destinazione.GetAttribute("appezza")),
                                                                                            CInt(xMov_Destinazione.GetAttribute("id_destinazione")),
                                                                                            enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                                            "",
                                                                                            "",
                                                                                            objParametri)
                                                            ObjImpianti = Nothing

                                                            If DtImpianti.Rows.Count > 0 Then

                                                                Veg_Cod = DtImpianti.Rows(0).Item("Veg_Cod")
                                                                Veg_Des = DtImpianti.Rows(0).Item("Veg_Des")
                                                                Grfi_Cod = Agro_SQL_SaveNum(DtImpianti.Rows(0).Item("Grfi_Cod"))

                                                            End If

                                                        End If

                                                        'Verifico se s'impianto non sia già presente nella struttura di appoggio
                                                        bDestinazionePresente = False
                                                        For i = 0 To UBound(Destinazioni, 2) - 1
                                                            If Destinazioni(Codice_Piva, i) = xMov_Destinazione.GetAttribute("piva") AndAlso
                                                               Destinazioni(Codice_SaCod, i) = CInt(xMov_Destinazione.GetAttribute("sa_cod")) AndAlso
                                                               CLng(Destinazioni(Codice_Appezza, i)) = CInt(xMov_Destinazione.GetAttribute("appezza")) AndAlso
                                                               CLng(Destinazioni(Codice_IdReg, i)) = CInt(xMov_Destinazione.GetAttribute("id_destinazione")) Then

                                                                bDestinazionePresente = True
                                                                Indice = i

                                                                Exit For

                                                            End If
                                                        Next i


                                                        If Not bDestinazionePresente Then

                                                            OperazioneCorrente_Scrivi___Tmp_Movimenti_Destinazioni_DateDistinta(IDTestataTemp,
                                                                                                                                xMov_Destinazione.GetAttribute("piva"),
                                                                                                                                xMov_Destinazione.GetAttribute("sa_cod"),
                                                                                                                                xMov_Destinazione.GetAttribute("appezza"),
                                                                                                                                xMov_Destinazione.GetAttribute("id_destinazione"),
                                                                                                                                Data,
                                                                                                                                objParametri)



                                                            '===================================================================================================================================
                                                            'Aggiornamento struttura di appoggio delle destinazioni di intervento
                                                            '-----------------------------------------------------------------------------------------------------------------------------------
                                                            ReDim Preserve Destinazioni(20, UBound(Destinazioni, 2) + 1)

                                                            'Destinazioni(Codice_Piva, UBound(Destinazioni, 2) - 1) = PIVA
                                                            'Destinazioni(Codice_SaCod, UBound(Destinazioni, 2) - 1) = Sa_Cod
                                                            Destinazioni(Codice_Piva, UBound(Destinazioni, 2) - 1) = xMov_Destinazione.GetAttribute("piva")
                                                            Destinazioni(Codice_SaCod, UBound(Destinazioni, 2) - 1) = CInt(xMov_Destinazione.GetAttribute("sa_cod"))
                                                            Destinazioni(Codice_Appezza, UBound(Destinazioni, 2) - 1) = CInt(xMov_Destinazione.GetAttribute("appezza"))
                                                            Destinazioni(Codice_IdReg, UBound(Destinazioni, 2) - 1) = CInt(xMov_Destinazione.GetAttribute("id_destinazione"))
                                                            Destinazioni(Codice_SupImp, UBound(Destinazioni, 2) - 1) = CDec(xMov_Destinazione.GetAttribute("qta2"))

                                                            'Dim objDistinta As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                                                            'objParametri.ImpostaFinestre_con_SalvataggioTemporale(Data, Data)
                                                            'DtDistinta = objDistinta.Leggi(CStr(PIVA),
                                                            '                                0,
                                                            '                                "9100",
                                                            '                                0,
                                                            '                                CInt(Sa_Cod),
                                                            '                                CInt(xMov_Destinazione.GetAttribute("appezza")),
                                                            '                                CInt(xMov_Destinazione.GetAttribute("id_destinazione")),
                                                            '                                0,
                                                            '                                0,
                                                            '                                enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                            '                                "", "",
                                                            '                                 objParametri)
                                                            'objParametri.ResettaFinestra()
                                                            'objDistinta = Nothing

                                                            'If DtDistinta.Rows.Count <> 0 Then

                                                            '    Destinazioni(Codice_ProgettoNome, UBound(Destinazioni, 2) - 1) = DtDistinta.Rows(0).Item("Progetto_Nome") 'LottoImpianto(PIVA, Sa_Cod, CLng(xMov_Destinazione.GetAttribute("appezza")), CLng(xMov_Destinazione.GetAttribute("id_destinazione")), CLng(DtImpianti.Rows(0).Item("Cul_Cod")), 0, Data, objParametri)
                                                            '    If Destinazioni(Codice_SupImp, UBound(Destinazioni, 2) - 1) = 0 Then
                                                            '        Destinazioni(Codice_SupImp, UBound(Destinazioni, 2) - 1) = DtDistinta.Rows(0).Item("Sup_Imp")
                                                            '    End If

                                                            '    Destinazioni(Codice_CulCod, UBound(Destinazioni, 2) - 1) = DtDistinta.Rows(0).Item("Cul_Cod")
                                                            '    Destinazioni(Codice_ProgettoCod, UBound(Destinazioni, 2) - 1) = DtDistinta.Rows(0).Item("Progetto_Cod")

                                                            '    Destinazioni(Codice_ValiditaInizio, UBound(Destinazioni, 2) - 1) = DtDistinta.Rows(0).Item("validita_inizio")
                                                            '    Destinazioni(Codice_ValiditaFine, UBound(Destinazioni, 2) - 1) = DtDistinta.Rows(0).Item("validita_fine")

                                                            '    If Not IsDBNull(DtDistinta.Rows(0).Item("Regolamento_Concimazioni_Cod")) Then
                                                            '        Destinazioni(Codice_Regolamento_Concimazioni_Cod, UBound(Destinazioni, 2) - 1) = DtDistinta.Rows(0).Item("Regolamento_Concimazioni_Cod")
                                                            '    Else
                                                            '        Destinazioni(Codice_Regolamento_Concimazioni_Cod, UBound(Destinazioni, 2) - 1) = 0
                                                            '    End If

                                                            '    Destinazioni(Codice_GrfiCod, UBound(Destinazioni, 2) - 1) = DtDistinta.Rows(0).Item("Grfi_Cod_Impianto")


                                                            'Else

                                                            '    'Eccezione: Impianto Non Esistente...
                                                            '    Destinazioni(Codice_ProgettoNome, UBound(Destinazioni, 2) - 1) = ""
                                                            '    Destinazioni(Codice_SupImp, UBound(Destinazioni, 2) - 1) = 0.0001 'Evito il Division By Zero
                                                            '    Destinazioni(Codice_CulCod, UBound(Destinazioni, 2) - 1) = 0
                                                            '    Destinazioni(Codice_ProgettoCod, UBound(Destinazioni, 2) - 1) = 0

                                                            '    Destinazioni(Codice_ValiditaInizio, UBound(Destinazioni, 2) - 1) = AGRODATAINIZIO
                                                            '    Destinazioni(Codice_ValiditaFine, UBound(Destinazioni, 2) - 1) = AGRODATAFINE

                                                            '    Destinazioni(Codice_Regolamento_Concimazioni_Cod, UBound(Destinazioni, 2) - 1) = 0
                                                            '    Destinazioni(Codice_GrfiCod, UBound(Destinazioni, 2) - 1) = 0

                                                            '    'Do Nothing

                                                            'End If

                                                            Destinazioni(Codice_N, UBound(Destinazioni, 2) - 1) = 9999 'Dummy
                                                            Destinazioni(Codice_P, UBound(Destinazioni, 2) - 1) = 9999 'Dummy
                                                            Destinazioni(Codice_K, UBound(Destinazioni, 2) - 1) = 9999 'Dummy
                                                            Destinazioni(Codice_Mg, UBound(Destinazioni, 2) - 1) = 9999 'Dummy

                                                            Destinazioni(Codice_N_Distribuito, UBound(Destinazioni, 2) - 1) = 0
                                                            Destinazioni(Codice_P_Distribuito, UBound(Destinazioni, 2) - 1) = 0
                                                            Destinazioni(Codice_K_Distribuito, UBound(Destinazioni, 2) - 1) = 0
                                                            Destinazioni(Codice_Mg_Distribuito, UBound(Destinazioni, 2) - 1) = 0
                                                            Destinazioni(Codice_Cu_Distribuito, UBound(Destinazioni, 2) - 1) = 0



                                                            Indice = UBound(Destinazioni, 2) - 1

                                                        End If

                                                        'Aggiornamenti Macroelementi Distribuiti
                                                        Superficie = Destinazioni(5, Indice)

                                                        If Superficie <> 0 Then
                                                            If Efficienza < 1 Then
                                                                Destinazioni(Codice_N_Distribuito, Indice) = Destinazioni(Codice_N_Distribuito, Indice) + (CDbl(xMov_Destinazione.GetAttribute("qta")) / Superficie * N_Prodotto * Efficienza / 100)
                                                            Else
                                                                Destinazioni(Codice_N_Distribuito, Indice) = Destinazioni(Codice_N_Distribuito, Indice) + (CDbl(xMov_Destinazione.GetAttribute("qta")) / Superficie * N_Prodotto / 100)
                                                            End If
                                                            Destinazioni(Codice_P_Distribuito, Indice) = Destinazioni(Codice_P_Distribuito, Indice) + (CDbl(xMov_Destinazione.GetAttribute("qta")) / Superficie * P_Prodotto / 100)
                                                            Destinazioni(Codice_K_Distribuito, Indice) = Destinazioni(Codice_K_Distribuito, Indice) + (CDbl(xMov_Destinazione.GetAttribute("qta")) / Superficie * K_Prodotto / 100)
                                                            Destinazioni(Codice_Mg_Distribuito, Indice) = Destinazioni(Codice_Mg_Distribuito, Indice) + (CDbl(xMov_Destinazione.GetAttribute("qta")) / Superficie * Mg_Prodotto / 100)
                                                            Destinazioni(Codice_Cu_Distribuito, Indice) = Destinazioni(Codice_Cu_Distribuito, Indice) + (CDbl(xMov_Destinazione.GetAttribute("qta")) / Superficie * Cu_Prodotto / 100)

                                                            'Destinazioni(Codice_N_Distribuito, Indice) = Destinazioni(Codice_N_Distribuito, Indice) + (CDbl(xMov_Destinazione.GetAttribute("qta")) * N_Prodotto / (100 * Superficie))
                                                            'Destinazioni(Codice_P_Distribuito, Indice) = Destinazioni(Codice_P_Distribuito, Indice) + (CDbl(xMov_Destinazione.GetAttribute("qta")) * P_Prodotto / (100 * Superficie))
                                                            'Destinazioni(Codice_K_Distribuito, Indice) = Destinazioni(Codice_K_Distribuito, Indice) + (CDbl(xMov_Destinazione.GetAttribute("qta")) * K_Prodotto / (100 * Superficie))
                                                            'Destinazioni(Codice_Mg_Distribuito, Indice) = Destinazioni(Codice_Mg_Distribuito, Indice) + (CDbl(xMov_Destinazione.GetAttribute("qta")) * Mg_Prodotto / (100 * Superficie))
                                                        End If

                                                    End If
                                                Else

                                                    'Eccezione: la destinazione del trattamento non è un impianto

                                                End If

                                                i_DatiMov_Destinazioni += 1

                                            Loop


                                            i_DatiMovimento_Dettaglio += 1

                                        Loop

                                        i_DatiMovimenti_Dettagli += 1

                                    Loop


                                End If

                                i_Movimento += 1

                            Loop

                            i_DatiMovimento += 1

                        Loop

                    End If

                    i_Agenda += 1

                Loop

                i_DatiAgenda += 1

            Loop

            '###############################################################################
            '################# CONTROLLI PIANO CONCIMAZIONE ################################
            '###############################################################################

            'Ciclo per impianto colturale
            Dim Hash_FormulatiPA As New Hashtable
            Dim Hash_FormulatiPAPesi As New Hashtable


            '----------------------------------------------------------

            'lista n_max_intervento
            Dim objParametriUscitaFattoriCorrettivi As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output = Nothing
            Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS

            If Disciplinare_Cod > 0 AndAlso Veg_Cod > 0 AndAlso Grfi_Cod > 0 Then

                Dim objParametriIngressoFattoriCorrettivi As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input
                objParametriIngressoFattoriCorrettivi.Regolamento_Cod = Disciplinare_Cod ' 0
                objParametriIngressoFattoriCorrettivi.Veg_Cod = Veg_Cod
                objParametriIngressoFattoriCorrettivi.Grfi_Cod = 0
                objParametriIngressoFattoriCorrettivi.SoloValorizzati = True
                objParametriIngressoFattoriCorrettivi.SoloVisibili = False
                objParametriIngressoFattoriCorrettivi.Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.N_Max_Intervento

                Dim objAgroWebConfig As New AgroWebConfig
                If IsNothing(objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione) Then
                    Dim agroWs As String
                    Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri)
                    If agroWs = "" Then
                        agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Super_Server)
                    End If
                    objParametriIngressoFattoriCorrettivi.url = agroWs
                    objConfigurazione_Siti = Nothing
                End If

                objParametriUscitaFattoriCorrettivi = objPC_WS.FattoriCorrettivi_ConFinalitaGias_Leggi(objParametriIngressoFattoriCorrettivi)

            End If

            Dim obj_Op As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            Dim DTFrCod As DataTable = obj_Op.Leggi_Prodotti_Utilizzati_Su_VegCod_IntervalloTemporale(PIVA, Sa_Cod, Veg_Cod, AGRODATAINIZIO, AGRODATAFINE, FORMULATI, "", "", objParametri)

            Dim strFrCod As String = ""

            Dim IDTestataTemp__tmp_FormulatiXPrincipiAttivi As Integer = -1

            If DTFrCod IsNot Nothing AndAlso DTFrCod.Rows.Count > 0 Then

                For f = 0 To DTFrCod.Rows.Count - 1
                    strFrCod &= DTFrCod.Rows(f).Item("pro_cod") & ","
                Next

                If strFrCod <> "" Then

                    Dim objWs As New AgronicaCoreWebService.AgroWs
                    Dim DtPrincipi As DataTable
                    DtPrincipi = objWs.ComposizioneFormulatiRecupera(Left(strFrCod, strFrCod.Length - 1), objParametri, objParametri_Utenti)

                    If DtPrincipi IsNot Nothing Then

                        Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                        'IDTestataTemp__tmp_FormulatiXPrincipiAttivi = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("IDTestataTemp", objParametri)
                        'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                        IDTestataTemp__tmp_FormulatiXPrincipiAttivi = xAgrosequenze.NuovoId_Tabella("IDTestataTemp", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)
                        Dim filtroRameici As String = PaRameici_str.Replace("(", "").Replace(")", "")

                        For p = 0 To DtPrincipi.Rows.Count - 1
                            PopolaTabellaFormulatiPA(filtroRameici, IDTestataTemp__tmp_FormulatiXPrincipiAttivi, objParametri, DtPrincipi.Rows(p).Item("elenco_PrincipiAttivi"), DtPrincipi.Rows(p).Item("elenco_PrincipiAttiviPesi"), DtPrincipi.Rows(p).Item("fr_cod"))
                        Next

                    End If

                End If

            End If

            DPI_Verifica_Apporto_MacroElementi(DatiInseriti,
                                               XmlDoc,
                                               XmlDatiNonConformi,
                                               PIVA, Id_Agenda_Concimazione_Escluso, IDTestataTemp, Destinazioni,
                                               Veg_Cod, Data, Disciplinare_Cod, IDTestataTemp__tmp_FormulatiXPrincipiAttivi,
                                               objParametriUscitaFattoriCorrettivi,
                                               objParametri,
                                               isVerificaPosteriori:=False,
                                               paramVerificaDPIMultiAttivita:=paramVerificaDPIMultiAttivita,
                                               Raccoglitore_Cod:=Raccoglitore_Cod)

            If IDTestataTemp__tmp_FormulatiXPrincipiAttivi > 0 Then
                PulisciTabella__tmp_FormulatiXPrincipiAttivi(IDTestataTemp__tmp_FormulatiXPrincipiAttivi, objParametri)
            End If
            If IDTestataTemp > 0 Then
                PulisciTabella__Tmp_Movimenti_Destinazioni_DateDistinta(IDTestataTemp, objParametri)
            End If

            '-------------------------------------
            'For i = 0 To UBound(Destinazioni, 2) - 1

            '    DPI_Verifica_Apporto_MacroElementi(DatiInseriti,
            '                                       XmlDoc,
            '                                       XmlDatiNonConformi,
            '                                       CStr(Destinazioni(Codice_Piva, i)),
            '                                       CLng(Destinazioni(Codice_SaCod, i)),
            '                                       CLng(Destinazioni(Codice_Appezza, i)),
            '                                       CLng(Destinazioni(Codice_IdReg, i)),
            '                                       CLng(Destinazioni(Codice_ProgettoCod, i)),
            '                                       CStr(Destinazioni(Codice_ProgettoNome, i)),
            '                                       CStr(Destinazioni(Codice_ValiditaInizio, i)),
            '                                       CStr(Destinazioni(Codice_ValiditaFine, i)),
            '                                       Veg_Cod,
            '                                       CInt(Destinazioni(Codice_GrfiCod, i)),
            '                                       Data,
            '                                       Disciplinare_Cod,
            '                                       Id_Agenda_Concimazione_Escluso,
            '                                       CDec(Destinazioni(Codice_N_Distribuito, i)),
            '                                       CDec(Destinazioni(Codice_P_Distribuito, i)),
            '                                       CDec(Destinazioni(Codice_K_Distribuito, i)),
            '                                       CDec(Destinazioni(Codice_Mg_Distribuito, i)),
            '                                       CDec(Destinazioni(Codice_Cu_Distribuito, i)),
            '                                        objParametri, objParametri_Utenti,
            '                                       Hash_FormulatiPA, Hash_FormulatiPAPesi)


            'Next i

            XmlDatiGenerali.SetAttribute("piva", PIVA)
            XmlDatiGenerali.SetAttribute("sa_cod", Sa_Cod)
            XmlDatiGenerali.SetAttribute("data_movimento", Data.ToShortDateString)
            XmlDatiGenerali.SetAttribute("lav_cod", Lav_Cod)
            XmlDatiGenerali.SetAttribute("id_agenda", Id_Agenda)
            XmlDatiGenerali.SetAttribute("des_lib", Des_Lib)
            XmlDatiGenerali.SetAttribute("veg_cod", Veg_Cod)
            XmlDatiGenerali.SetAttribute("veg_des", Veg_Des)
            XmlDatiGenerali.SetAttribute("disciplinare_cod", Disciplinare_Cod)

            Select Case Disciplinare_Cod
                Case -2
                    XmlDatiGenerali.SetAttribute("disciplinare_des", Descrizione_Regolamento_Bio)
                Case > 0
                    Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input
                    objParametriIngresso.strFiltro = " (Regolamento_Cod=" & Disciplinare_Cod.ToString & ") "

                    Dim objAgroWebConfig As New AgroWebConfig
                    If IsNothing(objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione) Then
                        Dim agroWs As String
                        Dim objConfigurazione_Siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                        agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri)
                        If agroWs = "" Then
                            agroWs = objConfigurazione_Siti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Super_Server)
                        End If
                        objConfigurazione_Siti = Nothing
                        objParametriIngresso.url = agroWs
                    End If

                    Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
                    'Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                    objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso)
                    For i = 0 To objParametriUscita.ListaRegolamenti.Count - 1
                        XmlDatiGenerali.SetAttribute("disciplinare_des", objParametriUscita.ListaRegolamenti(i).Descrizione)
                    Next
                Case Else
                    XmlDatiGenerali.SetAttribute("disciplinare_des", "Nessuno")
            End Select

            XmlDatiGenerali.SetAttribute("id_rcdpi", "0") 'Inutile
            XmlDatiGenerali.SetAttribute("rcdpi_des", "") 'Inutile
            XmlDatiGenerali.SetAttribute("tipotestata", TipoTestata)

            XmlDatiGenerali.AppendChild(XmlDatiNonConformi)
            XmlDatiRisultati.AppendChild(XmlDatiGenerali)

            XmlDoc.AppendChild(XmlDatiRisultati)

            Return XmlDoc.OuterXml

        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            Return "-1"

        Finally

            If FlagConnessioneLocale Then
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

    End Function

    Public Function Verifica_Concimazioni_Old(ByVal Dt_Fertilizzazioni As DataTable,
                                              ByVal Dt_Fertilizzazioni_Impianti As DataTable,
                                              ByVal Disciplinare_Cod As Integer,
                                              ByVal ListaFertilizzanti As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output,
                                              ByVal ListaN_Max_Intervento As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output,
                                              ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
                                              ByRef objParametri As AgronicaCoreParametri,
                                              ByRef objParametri_Utenti As AgronicaCoreParametri
                                              ) As List(Of rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento))

        Dim Lr As New List(Of rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento))

        Dim rRis As rispostaStandard(Of Verifica_Disciplinare_Intervento)
        'Dim r As New rispostaStandard(Of Verifica_Disciplinare_Intervento)
        'r.RispostaStringa = New Verifica_Disciplinare_Intervento

        Dim Id_Agenda As Integer
        Dim Veg_Cod As Integer
        Dim Data As Date
        Dim Piva As String
        Dim Sa_Cod As Integer
        Dim Des_lib As String
        Dim Lav_Cod As Integer

        Dim Err_Cod As enTipoErrCode_Verifica
        Dim Err_Des As String

        Dim Codice_Piva As Integer = 0
        Dim Codice_SaCod As Integer = 1
        Dim Codice_Appezza As Integer = 2
        Dim Codice_IdReg As Integer = 3

        Dim Codice_ProgettoNome As Integer = 4
        Dim Codice_SupImp As Integer = 5
        Dim Codice_CulCod As Integer = 6

        Dim Codice_N As Integer = 7
        Dim Codice_P As Integer = 8
        Dim Codice_K As Integer = 9
        Dim Codice_Mg As Integer = 10

        Dim Codice_N_Distribuito As Integer = 11
        Dim Codice_P_Distribuito As Integer = 12
        Dim Codice_K_Distribuito As Integer = 13
        Dim Codice_Mg_Distribuito As Integer = 14

        Dim Codice_ProgettoCod As Integer = 15

        Dim Codice_ValiditaInizio As Integer = 16
        Dim Codice_ValiditaFine As Integer = 17

        Dim Codice_Cu_Distribuito As Integer = 18

        Dim Codice_Regolamento_Concimazioni_Cod As Integer = 19

        Dim Codice_GrfiCod As Integer = 20


        Try

            For i = 0 To Dt_Fertilizzazioni.Rows.Count - 1

                Id_Agenda = Dt_Fertilizzazioni.Rows(i).Item("id_agenda")
                Veg_Cod = Dt_Fertilizzazioni.Rows(i).Item("Veg_Cod")
                Data = Dt_Fertilizzazioni.Rows(i).Item("data_movimento")
                Piva = Dt_Fertilizzazioni.Rows(i).Item("piva")
                Sa_Cod = Dt_Fertilizzazioni.Rows(i).Item("sa_cod")
                Des_lib = Dt_Fertilizzazioni.Rows(i).Item("des_lib")
                Lav_Cod = Dt_Fertilizzazioni.Rows(i).Item("Lav_Cod")

                Dim Sup_Coinvolta As Decimal = 0
                Dim strAppCoinvolti As String = ""

                Dim Dpi_Cod As Integer = 0
                Dim Dpi_Cod_Op As Integer = 0
                Dim Biologico As Boolean

                If IsNumeric(Disciplinare_Cod) AndAlso CInt(Disciplinare_Cod) = enum_Disciplinare_Operazione.QuelloDellOperazione Then
                    Dpi_Cod_Op = Dt_Fertilizzazioni.Rows(i).Item("num_protocollo")
                End If

                Select Case Disciplinare_Cod
                    Case enum_Disciplinare_Operazione.Nessuno
                        Dpi_Cod = 0
                    Case enum_Disciplinare_Operazione.Biologico
                        Dpi_Cod = -2
                        Biologico = True
                    Case enum_Disciplinare_Operazione.QuelloDellOperazione
                        Select Case Dpi_Cod_Op
                            Case enum_Disciplinare_Operazione.Nessuno
                                Dpi_Cod = 0
                            Case enum_Disciplinare_Operazione.Biologico
                                Dpi_Cod = -2
                                Biologico = True
                            Case enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta
                                Dpi_Cod = 0
                            Case Else
                                Dpi_Cod = Dpi_Cod_Op
                        End Select
                    Case Else
                        'dpi
                        Dim array As String()
                        array = Split(Disciplinare_Cod, "/")
                        Dpi_Cod = CInt(array(0))
                End Select

                Dim HashImpianti As New Hashtable
                Dim HashProdotti As New Hashtable

                Dim XmlDoc As New XmlDocument

                Dim XmlDatiRisultati As XmlElement
                Dim XmlDatiGenerali As XmlElement
                Dim XmlDatiNonConformi As XmlElement

                Dim DatiInseriti(10, 0) As String
                Dim Destinazioni(20, 0) As String

                XmlDatiRisultati = XmlDoc.CreateElement("DatiRisultati")

                XmlDatiGenerali = XmlDoc.CreateElement("DatiGenerali")
                XmlDatiNonConformi = XmlDoc.CreateElement("DatiNonConformi")

                If Dt_Fertilizzazioni_Impianti IsNot Nothing Then

                    Dim DrFertilizzazioniImpianti As DataRow() = Dt_Fertilizzazioni_Impianti.Select("id_agenda=" & Id_Agenda)

                    If DrFertilizzazioniImpianti IsNot Nothing Then

                        'estraggo i prodotti e gli apporti (ed eventualmente verifico se sono bio) 
                        For i_t = 0 To DrFertilizzazioniImpianti.Length - 1

                            Dim Fer_Cod As Integer = DrFertilizzazioniImpianti(i_t).Item("pro_cod")

                            If Not HashProdotti.ContainsKey(Fer_Cod) Then

                                HashProdotti.Add(Fer_Cod, DrFertilizzazioniImpianti(i_t).Item("n") & "|" & DrFertilizzazioniImpianti(i_t).Item("p") & "|" & DrFertilizzazioniImpianti(i_t).Item("k") & "|" & DrFertilizzazioniImpianti(i_t).Item("cu") & "|" & DrFertilizzazioniImpianti(i_t).Item("efficienza"))

                                '--------------------------------------------------------------------
                                'CONTROLLO PRODOTTI BIO
                                '--------------------------------------------------------------------
                                If Dpi_Cod = enum_Disciplinare_Operazione.Biologico Then

                                    Dim fert As AgronicaCoreMetaSchemaBIZ.Fertilizzante = (From ff In ListaFertilizzanti.ListaFertilizzanti
                                                                                           Where ff.Codice = Fer_Cod).FirstOrDefault()

                                    If Not fert.Biologico Then

                                        Err_Cod = enTipoErrCode_Verifica.ProdottoNonBiologico
                                        Err_Des = "Il Fertilizzante utilizzato (" & fert.Descrizione & ") non è Biologico. "

                                        Inserisci_NonConformita(DatiInseriti,
                                                                 XmlDoc,
                                                                 XmlDatiNonConformi,
                                                                 Err_Cod,
                                                                 Err_Des,
                                                                 "",
                                                                 Fer_Cod,
                                                                 0,
                                                                 0,
                                                                 "",
                                                                 "",
                                                                 0,
                                                                 0,
                                                                 "", "", "",
                                                                 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)

                                    End If

                                End If

                            End If

                        Next

                        'estraggo gli impianti
                        For i_t = 0 To DrFertilizzazioniImpianti.Length - 1
                            If Not HashImpianti.ContainsKey(DrFertilizzazioniImpianti(i_t).Item("piva") & "|" & DrFertilizzazioniImpianti(i_t).Item("sa_cod") & "|" & DrFertilizzazioniImpianti(i_t).Item("appezza") & "|" & DrFertilizzazioniImpianti(i_t).Item("id_destinazione") & "|" & DrFertilizzazioniImpianti(i_t).Item("progetto_cod")) Then
                                HashImpianti.Add(DrFertilizzazioniImpianti(i_t).Item("piva") &
                                                 "|" & DrFertilizzazioniImpianti(i_t).Item("sa_cod") &
                                                 "|" & DrFertilizzazioniImpianti(i_t).Item("appezza") &
                                                 "|" & DrFertilizzazioniImpianti(i_t).Item("id_destinazione") &
                                                 "|" & DrFertilizzazioniImpianti(i_t).Item("progetto_cod"),
                                                 DrFertilizzazioniImpianti(i_t).Item("sup_imp") &
                                                 "|" & DrFertilizzazioniImpianti(i_t).Item("qta2") &
                                                 "|" & DrFertilizzazioniImpianti(i_t).Item("cul_cod") &
                                                 "|" & DrFertilizzazioniImpianti(i_t).Item("progetto_nome") &
                                                 "|" & DrFertilizzazioniImpianti(i_t).Item("validita_inizio_esercizio") &
                                                 "|" & DrFertilizzazioniImpianti(i_t).Item("validita_fine_esercizio") &
                                                 "|" & DrFertilizzazioniImpianti(i_t).Item("n_massimo") &
                                                 "|" & DrFertilizzazioniImpianti(i_t).Item("p_massimo") &
                                                 "|" & DrFertilizzazioniImpianti(i_t).Item("k_massimo") &
                                                 "|" & DrFertilizzazioniImpianti(i_t).Item("mg_massimo") &
                                                 "|" & DrFertilizzazioniImpianti(i_t).Item("Regolamento_Concimazioni_Cod") &
                                                 "|" & DrFertilizzazioniImpianti(i_t).Item("Grfi_Cod"))

                                strAppCoinvolti += DrFertilizzazioniImpianti(i_t).Item("app_nome") & ","
                                If Not IsDBNull(DrFertilizzazioniImpianti(i_t).Item("qta2")) AndAlso IsNumeric(DrFertilizzazioniImpianti(i_t).Item("qta2")) AndAlso CDec(DrFertilizzazioniImpianti(i_t).Item("qta2")) > 0 Then
                                    Sup_Coinvolta += CDec(DrFertilizzazioniImpianti(i_t).Item("qta2"))
                                Else
                                    Sup_Coinvolta += CDec(DrFertilizzazioniImpianti(i_t).Item("sup_imp"))
                                End If
                            End If
                        Next
                        If strAppCoinvolti <> "" Then
                            strAppCoinvolti = Left(strAppCoinvolti, strAppCoinvolti.Length - 1)
                        End If


                        For Each key In HashImpianti.Keys

                            ReDim Preserve Destinazioni(20, UBound(Destinazioni, 2) + 1)

                            Destinazioni(Codice_Piva, UBound(Destinazioni, 2) - 1) = Split(key.ToString, "|")(0)
                            Destinazioni(Codice_SaCod, UBound(Destinazioni, 2) - 1) = Split(key.ToString, "|")(1)
                            Destinazioni(Codice_Appezza, UBound(Destinazioni, 2) - 1) = Split(key.ToString, "|")(2)
                            Destinazioni(Codice_IdReg, UBound(Destinazioni, 2) - 1) = Split(key.ToString, "|")(3)
                            Destinazioni(Codice_ProgettoCod, UBound(Destinazioni, 2) - 1) = Split(key.ToString, "|")(4)

                            Destinazioni(Codice_SupImp, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(1)
                            Destinazioni(Codice_ProgettoNome, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(3)
                            If Destinazioni(Codice_SupImp, UBound(Destinazioni, 2) - 1) = 0 Then
                                Destinazioni(Codice_SupImp, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(0)
                            End If

                            Destinazioni(Codice_CulCod, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(2)
                            Destinazioni(Codice_ValiditaInizio, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(4)
                            Destinazioni(Codice_ValiditaFine, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(5)

                            Destinazioni(Codice_N, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(6)
                            Destinazioni(Codice_P, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(7)
                            Destinazioni(Codice_K, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(8)
                            Destinazioni(Codice_Mg, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(9)

                            Destinazioni(Codice_Regolamento_Concimazioni_Cod, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(10)

                            Destinazioni(Codice_GrfiCod, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(11)

                            Destinazioni(Codice_N_Distribuito, UBound(Destinazioni, 2) - 1) = 0
                            Destinazioni(Codice_P_Distribuito, UBound(Destinazioni, 2) - 1) = 0
                            Destinazioni(Codice_K_Distribuito, UBound(Destinazioni, 2) - 1) = 0
                            Destinazioni(Codice_Mg_Distribuito, UBound(Destinazioni, 2) - 1) = 0
                            Destinazioni(Codice_Cu_Distribuito, UBound(Destinazioni, 2) - 1) = 0

                            For i_t = 0 To DrFertilizzazioniImpianti.Length - 1

                                If DrFertilizzazioniImpianti(i_t).Item("piva") = Destinazioni(Codice_Piva, UBound(Destinazioni, 2) - 1) And
                                     DrFertilizzazioniImpianti(i_t).Item("sa_cod") = Destinazioni(Codice_SaCod, UBound(Destinazioni, 2) - 1) And
                                     DrFertilizzazioniImpianti(i_t).Item("appezza") = Destinazioni(Codice_Appezza, UBound(Destinazioni, 2) - 1) And
                                     DrFertilizzazioniImpianti(i_t).Item("id_destinazione") = Destinazioni(Codice_IdReg, UBound(Destinazioni, 2) - 1) And
                                     DrFertilizzazioniImpianti(i_t).Item("progetto_cod") = Destinazioni(Codice_ProgettoCod, UBound(Destinazioni, 2) - 1) And
                                     Destinazioni(Codice_SupImp, UBound(Destinazioni, 2) - 1) <> 0 Then

                                    If DrFertilizzazioniImpianti(i_t).Item("efficienza") < 1 Then
                                        Destinazioni(Codice_N_Distribuito, UBound(Destinazioni, 2) - 1) += DrFertilizzazioniImpianti(i_t).Item("qta_dest") / Destinazioni(Codice_SupImp, UBound(Destinazioni, 2) - 1) * DrFertilizzazioniImpianti(i_t).Item("n") * DrFertilizzazioniImpianti(i_t).Item("efficienza") / 100
                                    Else
                                        Destinazioni(Codice_N_Distribuito, UBound(Destinazioni, 2) - 1) += DrFertilizzazioniImpianti(i_t).Item("qta_dest") / Destinazioni(Codice_SupImp, UBound(Destinazioni, 2) - 1) * DrFertilizzazioniImpianti(i_t).Item("n") / 100
                                    End If

                                    Destinazioni(Codice_P_Distribuito, UBound(Destinazioni, 2) - 1) += DrFertilizzazioniImpianti(i_t).Item("qta_dest") / Destinazioni(Codice_SupImp, UBound(Destinazioni, 2) - 1) * DrFertilizzazioniImpianti(i_t).Item("p") / 100
                                    Destinazioni(Codice_K_Distribuito, UBound(Destinazioni, 2) - 1) += DrFertilizzazioniImpianti(i_t).Item("qta_dest") / Destinazioni(Codice_SupImp, UBound(Destinazioni, 2) - 1) * DrFertilizzazioniImpianti(i_t).Item("k") / 100
                                    Destinazioni(Codice_Mg_Distribuito, UBound(Destinazioni, 2) - 1) += DrFertilizzazioniImpianti(i_t).Item("qta_dest") / Destinazioni(Codice_SupImp, UBound(Destinazioni, 2) - 1) * DrFertilizzazioniImpianti(i_t).Item("mg") / 100
                                    Destinazioni(Codice_Cu_Distribuito, UBound(Destinazioni, 2) - 1) += DrFertilizzazioniImpianti(i_t).Item("qta_dest") / Destinazioni(Codice_SupImp, UBound(Destinazioni, 2) - 1) * DrFertilizzazioniImpianti(i_t).Item("cu") / 100

                                End If

                            Next

                        Next

                    End If

                End If


                'Ciclo per impianto colturale
                Dim Hash_FormulatiPA As New Hashtable
                Dim Hash_FormulatiPAPesi As New Hashtable

                For imp = 0 To UBound(Destinazioni, 2) - 1

                    DPI_Verifica_Apporto_MacroElementi_old(DatiInseriti,
                                                   XmlDoc,
                                                   XmlDatiNonConformi,
                                                   CStr(Destinazioni(Codice_Piva, imp)),
                                                   CLng(Destinazioni(Codice_SaCod, imp)),
                                                   CLng(Destinazioni(Codice_Appezza, imp)),
                                                   CLng(Destinazioni(Codice_IdReg, imp)),
                                                   CLng(Destinazioni(Codice_ProgettoCod, imp)),
                                                   CStr(Destinazioni(Codice_ProgettoNome, imp)),
                                                   CStr(Destinazioni(Codice_ValiditaInizio, imp)),
                                                   CStr(Destinazioni(Codice_ValiditaFine, imp)),
                                                   Veg_Cod,
                                                   CStr(Destinazioni(Codice_GrfiCod, imp)),
                                                   Data,
                                                   Dpi_Cod,
                                                   CLng(Destinazioni(Codice_Regolamento_Concimazioni_Cod, imp)),
                                                   CStr(Destinazioni(Codice_N, imp)),
                                                   CStr(Destinazioni(Codice_P, imp)),
                                                   CStr(Destinazioni(Codice_K, imp)),
                                                   CStr(Destinazioni(Codice_Mg, imp)),
                                                   Id_Agenda,
                                                   CDec(Destinazioni(Codice_N_Distribuito, imp)),
                                                   CDec(Destinazioni(Codice_P_Distribuito, imp)),
                                                   CDec(Destinazioni(Codice_K_Distribuito, imp)),
                                                   CDec(Destinazioni(Codice_Mg_Distribuito, imp)),
                                                   CDec(Destinazioni(Codice_Cu_Distribuito, imp)),
                                                   ListaN_Max_Intervento,
                                                   objParametri, objParametri_Utenti,
                                                   Hash_FormulatiPA)


                Next

                XmlDatiGenerali.SetAttribute("piva", Piva)
                XmlDatiGenerali.SetAttribute("sa_cod", Sa_Cod)
                XmlDatiGenerali.SetAttribute("data_movimento", Data.ToShortDateString)
                XmlDatiGenerali.SetAttribute("lav_cod", Lav_Cod)
                XmlDatiGenerali.SetAttribute("id_agenda", Id_Agenda)
                XmlDatiGenerali.SetAttribute("des_lib", Des_lib)
                XmlDatiGenerali.SetAttribute("veg_cod", Veg_Cod)
                XmlDatiGenerali.SetAttribute("veg_des", "")
                XmlDatiGenerali.SetAttribute("disciplinare_cod", Dpi_Cod)

                Select Case Dpi_Cod
                    Case -2
                        XmlDatiGenerali.SetAttribute("disciplinare_des", Descrizione_Regolamento_Bio)
                    Case > 0
                        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input
                        objParametriIngresso.strFiltro = " (Regolamento_Cod=" & Dpi_Cod.ToString & ") "
                        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
                        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                        objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso)
                        For disc = 0 To objParametriUscita.ListaRegolamenti.Count - 1
                            XmlDatiGenerali.SetAttribute("disciplinare_des", objParametriUscita.ListaRegolamenti(disc).Descrizione)
                        Next
                    Case Else
                        XmlDatiGenerali.SetAttribute("disciplinare_des", "Nessuno")
                End Select

                XmlDatiGenerali.SetAttribute("id_rcdpi", "0") 'Inutile
                XmlDatiGenerali.SetAttribute("rcdpi_des", "") 'Inutile
                XmlDatiGenerali.SetAttribute("tipotestata", enum_Disciplinare_Tipo_Testata.Fertilizzazione)

                XmlDatiGenerali.SetAttribute("appezzamenti", strAppCoinvolti)
                XmlDatiGenerali.SetAttribute("superficie", Sup_Coinvolta)

                XmlDatiGenerali.AppendChild(XmlDatiNonConformi)
                XmlDatiRisultati.AppendChild(XmlDatiGenerali)

                XmlDoc.AppendChild(XmlDatiRisultati)

                Dim DatiVerifica As String = XmlDoc.OuterXml

                rRis = New rispostaStandard(Of Verifica_Disciplinare_Intervento)
                rRis = XmlFINALE(CStr(DatiVerifica),
                                            CInt(Id_Agenda),
                                            VerificaSoloControlliImpostazioniUtente,
                                            objParametri,
                                            objParametri_Utenti,
                                            "",
                                                Biologico,
                                                    Dpi_Cod)

                Lr.Add(rRis)

            Next

        Catch ex As Exception

            Dim StrDummy As String

            StrDummy = ex.Message.ToString()

            Scrivi_LOG(objParametri, "Verifica_Concimazioni", StrDummy)

            rRis = New rispostaStandard(Of Verifica_Disciplinare_Intervento)

            rRis.RispostaOK = False
            rRis.Errore = StrDummy

            Lr.Add(rRis)

        End Try

        Return Lr


    End Function

    Public Function Verifica_Concimazioni(ByVal Dt_Fertilizzazioni As DataTable,
                                          ByVal Disciplinare_Cod As Integer, ByVal IDTestataTemp__tmp_FormulatiXPrincipiAttivi As Integer,
                                          ByVal ListaFertilizzanti As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output,
                                          ByVal ListaN_Max_Intervento As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output,
                                          ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreParametri,
                                          ByRef objParametri_SuperServer As AgronicaCoreParametri
                                          ) As List(Of rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento))

        Dim Lr As New List(Of rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento))

        Dim rRis As rispostaStandard(Of Verifica_Disciplinare_Intervento)

        Dim Id_Agenda As Integer
        Dim Veg_Cod As Integer
        Dim Data As Date
        Dim Piva As String
        Dim Sa_Cod As Integer
        Dim Des_lib As String
        Dim Lav_Cod As Integer

        Dim Err_Cod As enTipoErrCode_Verifica
        Dim Err_Des As String

        Try

            For i = 0 To Dt_Fertilizzazioni.Rows.Count - 1

                Id_Agenda = Dt_Fertilizzazioni.Rows(i).Item("id_agenda")
                Veg_Cod = Dt_Fertilizzazioni.Rows(i).Item("Veg_Cod")
                Data = Dt_Fertilizzazioni.Rows(i).Item("data_movimento")
                Piva = Dt_Fertilizzazioni.Rows(i).Item("piva")
                Sa_Cod = Dt_Fertilizzazioni.Rows(i).Item("sa_cod")
                Des_lib = Dt_Fertilizzazioni.Rows(i).Item("des_lib")
                Lav_Cod = Dt_Fertilizzazioni.Rows(i).Item("Lav_Cod")

                Dim Sup_Coinvolta As Decimal = 0
                Dim strAppCoinvolti As String = ""
                Dim list_strAppCoinvolti As New List(Of String)

                Dim Dpi_Cod As Integer = 0
                Dim Dpi_Cod_Op As Integer = 0
                Dim Biologico As Boolean

                If IsNumeric(Disciplinare_Cod) AndAlso CInt(Disciplinare_Cod) = enum_Disciplinare_Operazione.QuelloDellOperazione Then
                    Dpi_Cod_Op = Dt_Fertilizzazioni.Rows(i).Item("num_protocollo")
                End If

                Select Case Disciplinare_Cod
                    Case enum_Disciplinare_Operazione.Nessuno
                        Dpi_Cod = 0
                    Case enum_Disciplinare_Operazione.Biologico
                        Dpi_Cod = -2
                        Biologico = True
                    Case enum_Disciplinare_Operazione.QuelloDellOperazione
                        Select Case Dpi_Cod_Op
                            Case enum_Disciplinare_Operazione.Nessuno
                                Dpi_Cod = 0
                            Case enum_Disciplinare_Operazione.Biologico
                                Dpi_Cod = -2
                                Biologico = True
                            Case enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta
                                Dpi_Cod = 0
                            Case Else
                                Dpi_Cod = Dpi_Cod_Op
                        End Select
                    Case Else
                        'dpi
                        Dim array As String()
                        array = Split(Disciplinare_Cod, "/")
                        Dpi_Cod = CInt(array(0))
                End Select

                Dim HashImpianti As New Hashtable
                Dim HashProdotti As New Hashtable

                Dim XmlDoc As New XmlDocument

                Dim XmlDatiRisultati As XmlElement
                Dim XmlDatiGenerali As XmlElement
                Dim XmlDatiNonConformi As XmlElement

                Dim DatiInseriti(10, 0) As String
                Dim Destinazioni(20, 0) As String

                XmlDatiRisultati = XmlDoc.CreateElement("DatiRisultati")

                XmlDatiGenerali = XmlDoc.CreateElement("DatiGenerali")
                XmlDatiNonConformi = XmlDoc.CreateElement("DatiNonConformi")

                Dim ObjDett As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                Dim DtFerCod As DataTable
                DtFerCod = ObjDett.Leggi_ProCod_Utilizzati(Piva, 0, 0, Id_Agenda, 0, 0, 0, 0, 0, 0, "", "", "", "", objParametri)

                Dim Prodotti As String = ""

                If DtFerCod IsNot Nothing Then

                    For Each Drfercod As DataRow In DtFerCod.Rows

                        Dim fert As AgronicaCoreMetaSchemaBIZ.Fertilizzante = (From ff In ListaFertilizzanti.ListaFertilizzanti
                                                                               Where ff.Codice = Drfercod.Item("pro_cod")).FirstOrDefault()

                        If fert IsNot Nothing Then

                            Prodotti &= fert.Descrizione & " (" & fert.N & "-" & fert.P2O5 & "-" & fert.K2O & "-" & fert.Cu & ") - "

                            '--------------------------------------------------------------------
                            'CONTROLLO PRODOTTI BIO
                            '--------------------------------------------------------------------
                            If Dpi_Cod = enum_Disciplinare_Operazione.Biologico Then

                                If Not fert.Biologico Then

                                    Err_Cod = enTipoErrCode_Verifica.ProdottoNonBiologico
                                    Err_Des = "Il Fertilizzante utilizzato (" & fert.Descrizione & ") non è Biologico. "
                                    Inserisci_NonConformita(DatiInseriti,
                                                                         XmlDoc,
                                                                         XmlDatiNonConformi,
                                                                         Err_Cod,
                                                                         Err_Des,
                                                                         "",
                                                                         fert.Codice,
                                                                         0,
                                                                         0,
                                                                         "",
                                                                         "",
                                                                         0,
                                                                         0,
                                                                         "", "", "",
                                                                         0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)

                                End If

                            End If

                        End If

                    Next

                End If

                If Prodotti <> "" Then
                    Des_lib &= " - " & Left(Prodotti, Prodotti.Length - 3)
                End If

                DPI_Verifica_Apporto_MacroElementi(DatiInseriti,
                                                   XmlDoc,
                                                   XmlDatiNonConformi,
                                                   Piva, Id_Agenda, 0, Nothing,
                                                   Veg_Cod, Data, Dpi_Cod, IDTestataTemp__tmp_FormulatiXPrincipiAttivi,
                                                   ListaN_Max_Intervento,
                                                   objParametri,
                                                   isVerificaPosteriori:=True,
                                                   list_strAppCoinvolti:=list_strAppCoinvolti,
                                                   Sup_Coinvolta:=Sup_Coinvolta)

                XmlDatiGenerali.SetAttribute("piva", Piva)
                XmlDatiGenerali.SetAttribute("sa_cod", Sa_Cod)
                XmlDatiGenerali.SetAttribute("data_movimento", Data.ToShortDateString)
                XmlDatiGenerali.SetAttribute("lav_cod", Lav_Cod)
                XmlDatiGenerali.SetAttribute("id_agenda", Id_Agenda)
                XmlDatiGenerali.SetAttribute("des_lib", Des_lib)
                XmlDatiGenerali.SetAttribute("veg_cod", Veg_Cod)
                XmlDatiGenerali.SetAttribute("veg_des", "")
                XmlDatiGenerali.SetAttribute("disciplinare_cod", Dpi_Cod)

                Select Case Dpi_Cod
                    Case -2
                        XmlDatiGenerali.SetAttribute("disciplinare_des", Descrizione_Regolamento_Bio)
                    Case > 0
                        Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input
                        objParametriIngresso.strFiltro = " (Regolamento_Cod=" & Dpi_Cod.ToString & ") "
                        Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
                        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                        objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso, objParametri, objParametri_SuperServer, True)
                        For disc = 0 To objParametriUscita.ListaRegolamenti.Count - 1
                            XmlDatiGenerali.SetAttribute("disciplinare_des", objParametriUscita.ListaRegolamenti(disc).Descrizione)
                        Next
                    Case Else
                        XmlDatiGenerali.SetAttribute("disciplinare_des", "Nessuno")
                End Select

                XmlDatiGenerali.SetAttribute("id_rcdpi", "0") 'Inutile
                XmlDatiGenerali.SetAttribute("rcdpi_des", "") 'Inutile
                XmlDatiGenerali.SetAttribute("tipotestata", enum_Disciplinare_Tipo_Testata.Fertilizzazione)

                If list_strAppCoinvolti.Count > 0 Then
                    strAppCoinvolti = String.Join(", ", list_strAppCoinvolti)
                End If
                XmlDatiGenerali.SetAttribute("appezzamenti", strAppCoinvolti)
                XmlDatiGenerali.SetAttribute("superficie", Sup_Coinvolta)

                XmlDatiGenerali.AppendChild(XmlDatiNonConformi)
                XmlDatiRisultati.AppendChild(XmlDatiGenerali)

                XmlDoc.AppendChild(XmlDatiRisultati)

                Dim DatiVerifica As String = XmlDoc.OuterXml

                rRis = New rispostaStandard(Of Verifica_Disciplinare_Intervento)
                rRis = XmlFINALE(CStr(DatiVerifica),
                                            CInt(Id_Agenda),
                                            VerificaSoloControlliImpostazioniUtente,
                                            objParametri,
                                            objParametri_Utenti,
                                            "",
                                                Biologico,
                                                    Dpi_Cod)

                Lr.Add(rRis)

            Next

        Catch ex As Exception

            Dim StrDummy As String

            StrDummy = ex.Message.ToString()

            Scrivi_LOG(objParametri, "Verifica_Concimazioni", StrDummy)

            rRis = New rispostaStandard(Of Verifica_Disciplinare_Intervento)

            rRis.RispostaOK = False
            rRis.Errore = StrDummy

            Lr.Add(rRis)

        End Try

        Return Lr

    End Function

    Public Function Verifica_Trattamenti(ByVal Dt_Trattamenti As DataTable,
                                         ByVal Dt_Trattamenti_Impianti As DataTable,
                                         ByVal Disciplinare_Cod As Integer, ByVal Disciplinare_PubblicoPrivato As Integer,
                                         ByVal IDTestataTemp__tmp_FormulatiXPrincipiAttivi As Integer,
                                         ByVal DtAppxPart As DataTable, ByVal DtCentri As DataTable,
                                         ByVal Dt_Raccolte_Impianti As DataTable, ByVal dtFasiFeno As DataTable,
                                         ByVal FF_Cod_Fioritura_Old As Integer, ByVal FF_Cod_Fioritura_New As Integer,
                                         ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
                                         ByRef objParametri As AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreParametri,
                                         Optional ByRef objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing,
                                         Optional ByVal leggiUrlDaConfigurazioniSiti As Boolean = False) As List(Of rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento))

        Dim Lrval As New List(Of rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento))



        Dim Id_Agenda As Integer
        Dim Veg_Cod As Integer
        Dim Data As Date
        Dim Piva As String
        Dim Sa_Cod As Integer
        Dim Des_lib As String
        Dim Lav_Cod As Integer


        Dim TipoTestata As Integer
        Dim Modulo, Epoca As Integer
        Dim flag_nuovo_controllo_riduzione_diserbo As Boolean



        Try

            flag_nuovo_controllo_riduzione_diserbo = Leggi_FlagNuovoControlloRiduzioneDiserbo(objParametri)

            Dim XmlDoc As New XmlDocument
            Dim XmlDocO As New XmlDocument
            Dim xDatiOperazioni As XmlElement
            Dim xOperazione As XmlElement

            Dim XmlDatiWS As XmlElement
            Dim XmlDatiGenerali As XmlElement
            Dim XmlDatiImpianti As XmlElement
            Dim XmlDatiImpianto As XmlElement
            Dim XmlDatiDettagli As XmlElement
            Dim XmlDatiDettaglio As XmlElement

            Dim objDpiVerifica As New AgronicaCoreDpiBIZ.DPI_Verifica

            XmlDoc = New XmlDocument
            xDatiOperazioni = XmlDoc.CreateElement("DatiOperazioni")

            For i = 0 To Dt_Trattamenti.Rows.Count - 1

                Id_Agenda = Dt_Trattamenti.Rows(i).Item("id_agenda")
                Veg_Cod = Dt_Trattamenti.Rows(i).Item("Veg_Cod")
                Data = Dt_Trattamenti.Rows(i).Item("data_movimento")
                Piva = Dt_Trattamenti.Rows(i).Item("piva")
                Sa_Cod = Dt_Trattamenti.Rows(i).Item("sa_cod")
                Des_lib = Dt_Trattamenti.Rows(i).Item("des_lib")
                Lav_Cod = Dt_Trattamenti.Rows(i).Item("Lav_Cod")


                Dim Sup_Coinvolta As Decimal = 0
                Dim strAppCoinvolti As String = ""


                XmlDocO = New XmlDocument

                XmlDatiWS = XmlDocO.CreateElement("DatiWS")

                XmlDatiGenerali = XmlDocO.CreateElement("DatiGenerali")
                XmlDatiImpianti = XmlDocO.CreateElement("DatiReg_Impianti")
                XmlDatiDettagli = XmlDocO.CreateElement("DatiDettagli")


                XmlDatiGenerali.SetAttribute("TipoOperazioneDB", "0")
                XmlDatiGenerali.SetAttribute("piva", Piva)
                XmlDatiGenerali.SetAttribute("sa_cod", Sa_Cod)
                XmlDatiGenerali.SetAttribute("data_movimento", Dt_Trattamenti.Rows(i).Item("data_movimento"))
                XmlDatiGenerali.SetAttribute("lav_cod", Lav_Cod)
                XmlDatiGenerali.SetAttribute("id_agenda", Id_Agenda)
                XmlDatiGenerali.SetAttribute("des_lib", Dt_Trattamenti.Rows(i).Item("des_lib"))
                XmlDatiGenerali.SetAttribute("mezzo", Dt_Trattamenti.Rows(i).Item("mezzo"))
                XmlDatiGenerali.SetAttribute("gru_cod", Dt_Trattamenti.Rows(i).Item("gru_cod"))
                XmlDatiGenerali.SetAttribute("veg_cod", Dt_Trattamenti.Rows(i).Item("veg_cod"))

                Select Case Lav_Cod
                    Case LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO
                        TipoTestata = enum_Disciplinare_Tipo_Testata.Diserbo
                        Modulo = 0
                        Epoca = Dt_Trattamenti.Rows(i).Item("extra_int")
                    Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                         LAVCOD_GEODISINFESTAZIONE,
                         LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE
                        TipoTestata = enum_Disciplinare_Tipo_Testata.Difesa
                        Modulo = Dt_Trattamenti.Rows(i).Item("extra_int")
                        Epoca = 0
                    Case LAVCOD_TRATTAMENTO_FITOREGOLATORE
                        TipoTestata = enum_Disciplinare_Tipo_Testata.Fitoregolatore
                        Modulo = Dt_Trattamenti.Rows(i).Item("extra_int")
                        Epoca = 0
                End Select

                XmlDatiGenerali.SetAttribute("tipotestata", TipoTestata)
                XmlDatiGenerali.SetAttribute("modulo", Modulo)
                XmlDatiGenerali.SetAttribute("epoca", Epoca)

                Dim Dpi_Cod As Integer = 0
                Dim Dpi_PubblicoPrivato As Integer = 0
                Dim Id_Rcdpi As Integer = 0
                Dim Biologico As Boolean = False

                Dim Dpi_Cod_Op As Integer = 0
                Dim Id_Rcdpi_Op As Integer = 0
                Dim Dpi_PubblicoPrivato_Op As Integer = 0

                If IsNumeric(Disciplinare_Cod) AndAlso CInt(Disciplinare_Cod) = enum_Disciplinare_Operazione.QuelloDellOperazione Then

                    Dpi_Cod_Op = Dt_Trattamenti.Rows(i).Item("num_protocollo")
                    If Dt_Trattamenti.Rows(i).Item("Disciplinare_PubblicoPrivato") = 2 Then
                        Dpi_Cod_Op = -Dt_Trattamenti.Rows(i).Item("num_protocollo")
                    End If
                    Id_Rcdpi_Op = Dt_Trattamenti.Rows(i).Item("doc_numero")

                End If

                Select Case Disciplinare_Cod
                    Case enum_Disciplinare_Operazione.Nessuno
                        Dpi_Cod = 0
                    Case enum_Disciplinare_Operazione.Biologico
                        Dpi_Cod = 0
                        Biologico = True
                    Case enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta
                        Dpi_Cod = 0
                    Case enum_Disciplinare_Operazione.QuelloDellOperazione
                        Select Case Dpi_Cod_Op
                            Case enum_Disciplinare_Operazione.Nessuno
                                Dpi_Cod = 0
                            Case enum_Disciplinare_Operazione.Biologico
                                Dpi_Cod = 0
                                Biologico = True
                            Case enum_Disciplinare_Operazione.NessunDpiNessunaEtichetta
                                Dpi_Cod = 0
                            Case Else
                                Dpi_Cod = Dpi_Cod_Op
                                Id_Rcdpi = Id_Rcdpi_Op
                        End Select
                    Case Else
                        'dpi
                        Dim Array As String()
                        Array = Split(Disciplinare_Cod, "/")
                        Dpi_Cod = CInt(Array(0))
                        Dpi_PubblicoPrivato = Disciplinare_PubblicoPrivato
                        If Array.Length > 4 AndAlso Array(4) IsNot Nothing Then
                            Dpi_PubblicoPrivato = CInt(Array(4))
                        End If
                        If Dpi_PubblicoPrivato = 2 Then
                            Dpi_Cod = -Dpi_Cod
                        End If
                        If Array.Length > 1 AndAlso Array(1) IsNot Nothing Then
                            Id_Rcdpi = CInt(Array(1))
                        End If

                End Select

                If Biologico Then
                    XmlDatiGenerali.SetAttribute("biologico", 1)
                Else
                    XmlDatiGenerali.SetAttribute("biologico", 0)
                End If

                XmlDatiGenerali.SetAttribute("id_rcdpi", Id_Rcdpi)
                XmlDatiGenerali.SetAttribute("disciplinare_cod", Dpi_Cod)



                Dim Sup_Operazione As Decimal = 0
                Dim Sup_Totale As Decimal = 0
                Dim HashImpiantiOp As New Hashtable
                Dim HashProdottiOp As New Hashtable

                Dim strComuni As String
                Dim Data_Raccolta As Date = AGRODATAFINE
                Dim Data_Fioritura As Date = AGRODATAINIZIO
                Dim ff_cod As Integer = 0
                Dim listaPapa As New List(Of String)

                If Dt_Trattamenti_Impianti IsNot Nothing Then

                    Dim DrTrattamentiImpianti As DataRow() = Dt_Trattamenti_Impianti.Select("id_agenda=" & Id_Agenda)
                    listaPapa.Clear()

                    If DrTrattamentiImpianti IsNot Nothing Then

                        For i_t = 0 To DrTrattamentiImpianti.Length - 1

                            If Not HashImpiantiOp.ContainsKey(DrTrattamentiImpianti(i_t).Item("piva") & "|" & DrTrattamentiImpianti(i_t).Item("sa_cod") & "|" & DrTrattamentiImpianti(i_t).Item("appezza") & "|" & DrTrattamentiImpianti(i_t).Item("id_destinazione")) Then

                                HashImpiantiOp.Add(DrTrattamentiImpianti(i_t).Item("piva") & "|" & DrTrattamentiImpianti(i_t).Item("sa_cod") & "|" & DrTrattamentiImpianti(i_t).Item("appezza") & "|" & DrTrattamentiImpianti(i_t).Item("id_destinazione"), "")

                                If IsNumeric(DrTrattamentiImpianti(i_t).Item("qta2")) AndAlso CDec(DrTrattamentiImpianti(i_t).Item("qta2")) <> 0 Then
                                    Sup_Operazione += CDec(DrTrattamentiImpianti(i_t).Item("qta2"))
                                Else
                                    Sup_Operazione += CDec(DrTrattamentiImpianti(i_t).Item("sup_imp"))
                                End If
                                Sup_Totale += CDec(DrTrattamentiImpianti(i_t).Item("sup_imp"))


                                XmlDatiImpianto = XmlDocO.CreateElement("DatiReg_Impianto")

                                XmlDatiImpianto.SetAttribute("piva", DrTrattamentiImpianti(i_t).Item("piva"))
                                XmlDatiImpianto.SetAttribute("sa_cod", DrTrattamentiImpianti(i_t).Item("sa_cod"))
                                XmlDatiImpianto.SetAttribute("appezza", DrTrattamentiImpianti(i_t).Item("appezza"))
                                XmlDatiImpianto.SetAttribute("id_reg", DrTrattamentiImpianti(i_t).Item("id_destinazione"))
                                XmlDatiImpianto.SetAttribute("lotto", DrTrattamentiImpianti(i_t).Item("progetto_nome"))
                                XmlDatiImpianto.SetAttribute("regolamento", DrTrattamentiImpianti(i_t).Item("regolamento_cod"))
                                XmlDatiImpianto.SetAttribute("cul_cod", DrTrattamentiImpianti(i_t).Item("cul_cod"))
                                If Dpi_Cod <> 0 Then
                                    XmlDatiImpianto.SetAttribute("disciplinare_cod", Dpi_Cod)
                                Else
                                    XmlDatiImpianto.SetAttribute("disciplinare_cod", DrTrattamentiImpianti(i_t).Item("disciplinare_cod"))
                                End If

                                '(29/10/2021 fede) modificato per controllo buffer (necessaria sup_imp)
                                XmlDatiImpianto.SetAttribute("sup_imp", DrTrattamentiImpianti(i_t).Item("sup_imp"))
                                XmlDatiImpianto.SetAttribute("sup_trattata", DrTrattamentiImpianti(i_t).Item("qta2"))

                                '(29/10/2021 fede) aggiunto controllo buffer
                                If Not IsDBNull(DrTrattamentiImpianti(i_t).Item("sup_riduzione_bufferzone")) Then
                                    XmlDatiImpianto.SetAttribute("sup_riduzione_bufferzone", DrTrattamentiImpianti(i_t).Item("sup_riduzione_bufferzone"))
                                Else
                                    XmlDatiImpianto.SetAttribute("sup_riduzione_bufferzone", "0")
                                End If
                                If Not IsDBNull(DrTrattamentiImpianti(i_t).Item("perc_riduzione_deriva")) Then
                                    XmlDatiImpianto.SetAttribute("perc_riduzione_deriva", DrTrattamentiImpianti(i_t).Item("perc_riduzione_deriva"))
                                Else
                                    XmlDatiImpianto.SetAttribute("perc_riduzione_deriva", "0")
                                End If

                                If Not IsDBNull(DrTrattamentiImpianti(i_t).Item("SupBZ_Riduzione")) Then
                                    XmlDatiImpianto.SetAttribute("offset_ultima_pianta", DrTrattamentiImpianti(i_t).Item("SupBZ_Riduzione"))
                                Else
                                    XmlDatiImpianto.SetAttribute("offset_ultima_pianta", "0")
                                End If
                                Dim lunghezza_confine_bufferzone As Decimal = DrTrattamentiImpianti(i_t).Item("DistBZ_CorpiIdrici") + DrTrattamentiImpianti(i_t).Item("DistBZ_AreeResPub") + DrTrattamentiImpianti(i_t).Item("DistBZ_Allevamenti") + DrTrattamentiImpianti(i_t).Item("DistBZ_VegNatNonColt")
                                XmlDatiImpianto.SetAttribute("lunghezza_confine_bufferzone", lunghezza_confine_bufferzone)

                                XmlDatiImpianto.SetAttribute("grfi_cod", DrTrattamentiImpianti(i_t).Item("grfi_cod"))
                                XmlDatiImpianto.SetAttribute("stato_impianto", DrTrattamentiImpianti(i_t).Item("stato_impianto"))
                                XmlDatiImpianto.SetAttribute("foral_cod", DrTrattamentiImpianti(i_t).Item("foral_cod"))
                                XmlDatiImpianto.SetAttribute("cop_cod", DrTrattamentiImpianti(i_t).Item("cop_cod"))
                                XmlDatiImpianto.SetAttribute("validita_inizio", DrTrattamentiImpianti(i_t).Item("validita_inizio_esercizio"))
                                XmlDatiImpianto.SetAttribute("validita_fine", DrTrattamentiImpianti(i_t).Item("validita_fine_esercizio"))

                                'AF 07/02/2025 - Spostato sotto, altrimenti se l'ordine delle righe è sfigato vengono saltate gli impiantixprodotti con PrincipiAttiviPercAbb valorizzato
                                'If Not IsDBNull(DrTrattamentiImpianti(i_t).Item("PrincipiAttiviPercAbb")) Then
                                '    Dim papa As String = DrTrattamentiImpianti(i_t).Item("PrincipiAttiviPercAbb").ToString
                                '    If Not String.IsNullOrEmpty(papa) Then
                                '        listaPapa.Add(papa)
                                '    End If
                                'End If

                                strComuni = ""
                                If DtAppxPart IsNot Nothing Then
                                    Dim DrAppxPart As DataRow() = DtAppxPart.Select("piva='" & DrTrattamentiImpianti(i_t).Item("piva") & "' and sa_cod=" & DrTrattamentiImpianti(i_t).Item("sa_cod") & " and appezza =" & DrTrattamentiImpianti(i_t).Item("appezza"))
                                    If DrAppxPart IsNot Nothing Then
                                        For a = 0 To DrAppxPart.Length - 1
                                            If InStr(strComuni, DrAppxPart(a).Item("prov") & DrAppxPart(a).Item("com")) = 0 Then
                                                strComuni &= DrAppxPart(a).Item("prov") & DrAppxPart(a).Item("com") & ","
                                            End If
                                        Next
                                    End If
                                End If

                                If strComuni = "" Then
                                    If DtCentri IsNot Nothing Then
                                        Dim DrCentro As DataRow() = DtCentri.Select("piva='" & DrTrattamentiImpianti(i_t).Item("piva") & "' and sa_cod=" & DrTrattamentiImpianti(i_t).Item("sa_cod"))
                                        If DrCentro IsNot Nothing AndAlso DrCentro.Length > 0 Then
                                            strComuni &= DrCentro(0).Item("pro_cod_istat") & DrCentro(0).Item("com_cod_istat")
                                        End If
                                    End If
                                Else
                                    strComuni = Left(strComuni, strComuni.Length - 1)
                                End If

                                XmlDatiImpianto.SetAttribute("lista_comuni", strComuni)


                                If Not IsDBNull(DrTrattamentiImpianti(i_t).Item("data_fine_prevista")) AndAlso
                                    IsDate(DrTrattamentiImpianti(i_t).Item("data_fine_prevista")) AndAlso
                                    CDate(DrTrattamentiImpianti(i_t).Item("data_fine_prevista")) > AGRODATAINIZIO AndAlso
                                    CDate(DrTrattamentiImpianti(i_t).Item("data_fine_prevista")) > Dt_Trattamenti.Rows(i).Item("data_movimento") Then

                                    Data_Raccolta = CDate(DrTrattamentiImpianti(i_t).Item("data_fine_prevista"))

                                End If


                                If Not IsDBNull(DrTrattamentiImpianti(i_t).Item("data_fioritura_prevista")) AndAlso
                                    IsDate(DrTrattamentiImpianti(i_t).Item("data_fioritura_prevista")) AndAlso
                                    CDate(DrTrattamentiImpianti(i_t).Item("data_fioritura_prevista")) > AGRODATAINIZIO Then

                                    Data_Fioritura = DrTrattamentiImpianti(i_t).Item("data_fioritura_prevista")

                                End If

                                ff_cod = 0

                                If Dt_Raccolte_Impianti IsNot Nothing Then
                                    Dim DrRaccolte As DataRow() = Dt_Raccolte_Impianti.Select("piva='" & DrTrattamentiImpianti(i_t).Item("piva") & "' and sa_cod=" & DrTrattamentiImpianti(i_t).Item("sa_cod") & " and appezza =" & DrTrattamentiImpianti(i_t).Item("appezza") & " and id_destinazione =" & DrTrattamentiImpianti(i_t).Item("id_destinazione") & " and Data_Movimento >'" & CDate(DrTrattamentiImpianti(i_t).Item("data_movimento")).ToShortDateString & "'", " data_movimento asc")
                                    If DrRaccolte IsNot Nothing AndAlso DrRaccolte.Length > 0 Then
                                        Data_Raccolta = DrRaccolte(0).Item("data_movimento")
                                    End If
                                End If

                                If dtFasiFeno IsNot Nothing AndAlso dtFasiFeno.Rows.Count > 0 Then

                                    Dim drFioritura As DataRow() = Nothing
                                    If FF_Cod_Fioritura_Old <> 0 Then
                                        drFioritura = dtFasiFeno.Select("piva='" & DrTrattamentiImpianti(i_t).Item("piva") & "' and sa_cod=" & DrTrattamentiImpianti(i_t).Item("sa_cod") & " and appezza =" & DrTrattamentiImpianti(i_t).Item("appezza") & " and id_destinazione =" & DrTrattamentiImpianti(i_t).Item("id_destinazione") & " and FF_Classe=" & Agro_SQL_SaveNum(FF_Cod_Fioritura_Old))
                                    ElseIf FF_Cod_Fioritura_New <> 0 Then
                                        drFioritura = dtFasiFeno.Select("piva='" & DrTrattamentiImpianti(i_t).Item("piva") & "' and sa_cod=" & DrTrattamentiImpianti(i_t).Item("sa_cod") & " and appezza =" & DrTrattamentiImpianti(i_t).Item("appezza") & " and id_destinazione =" & DrTrattamentiImpianti(i_t).Item("id_destinazione") & " and FF_Classe=" & Agro_SQL_SaveNum(FF_Cod_Fioritura_New))
                                    End If

                                    If drFioritura IsNot Nothing AndAlso drFioritura.Length > 0 Then
                                        Data_Fioritura = drFioritura(0).Item("data_movimento")
                                    End If

                                    Dim drFaseAttuale As DataRow() = dtFasiFeno.Select("piva='" & DrTrattamentiImpianti(i_t).Item("piva") & "' and sa_cod=" & DrTrattamentiImpianti(i_t).Item("sa_cod") & " and appezza =" & DrTrattamentiImpianti(i_t).Item("appezza") & " and id_destinazione =" & DrTrattamentiImpianti(i_t).Item("id_destinazione") & " and Data_Movimento <'" & CDate(DrTrattamentiImpianti(i_t).Item("data_movimento")).ToShortDateString & "'", " data_movimento desc")
                                    If drFaseAttuale IsNot Nothing AndAlso drFaseAttuale.Length > 0 Then
                                        ff_cod = drFaseAttuale(0).Item("FF_Classe")
                                    End If

                                End If


                                XmlDatiImpianto.SetAttribute("ff_cod", ff_cod)
                                XmlDatiImpianto.SetAttribute("data_raccolta", Data_Raccolta)
                                XmlDatiImpianto.SetAttribute("data_fioritura", Data_Fioritura)


                                XmlDatiImpianti.AppendChild(XmlDatiImpianto)


                                'strAppCoinvolti += DrTrattamentiImpianti(i_t).Item("app_nome") & ","

                                '(15/01/2018 fede) aggiunta indicazione se l'appezzamento ha un vincolo più restrittivo di quello scelto per l'intervento
                                'se si è scelto il regolamento bio si possono trattare tutti gli impianti
                                'se si è scelto un dpi si evidenziano quelli bio
                                'se non si sono scelti disciplinari si evidenziano gli impianti con un dpi e quelli bio
                                Select Case Dpi_Cod
                                    Case > 0 'dpi
                                        If DrTrattamentiImpianti(i_t).Item("regolamento_cod") = 4 Then
                                            strAppCoinvolti &= DrTrattamentiImpianti(i_t).Item("app_nome") & " (" & Gias.BIOVincoloPiuRestrittivo & ")" & ","
                                        Else
                                            strAppCoinvolti &= DrTrattamentiImpianti(i_t).Item("app_nome") & ","
                                        End If
                                    Case "0"
                                        If Not Biologico Then
                                            If DrTrattamentiImpianti(i_t).Item("regolamento_cod") = 4 Then
                                                strAppCoinvolti &= DrTrattamentiImpianti(i_t).Item("app_nome") & " (" & Gias.BIOVincoloPiuRestrittivo & ")" & ","
                                            Else
                                                If DrTrattamentiImpianti(i_t).Item("Disciplinare_Cod") <> 0 Then
                                                    strAppCoinvolti &= DrTrattamentiImpianti(i_t).Item("app_nome") & " (" & Gias.DPIVincoloPiuRestrittivo & ")" & ","
                                                Else
                                                    strAppCoinvolti &= DrTrattamentiImpianti(i_t).Item("app_nome") & ","
                                                End If
                                            End If
                                        Else
                                            strAppCoinvolti &= DrTrattamentiImpianti(i_t).Item("app_nome") & ","
                                        End If
                                    Case Else
                                        strAppCoinvolti &= DrTrattamentiImpianti(i_t).Item("app_nome") & ","
                                End Select


                                If Not IsDBNull(DrTrattamentiImpianti(i_t).Item("qta2")) AndAlso IsNumeric(DrTrattamentiImpianti(i_t).Item("qta2")) AndAlso CDec(DrTrattamentiImpianti(i_t).Item("qta2")) > 0 Then
                                    Sup_Coinvolta += CDec(DrTrattamentiImpianti(i_t).Item("qta2"))
                                Else
                                    Sup_Coinvolta += CDec(DrTrattamentiImpianti(i_t).Item("sup_imp"))
                                End If

                            End If



                            Dim for_veg_av_dos_cod As String = ""
                            Dim for_veg_cod As String = ""
                            Dim formulatixallegatinormative_idriga As Integer = 0

                            If Not HashProdottiOp.ContainsKey(DrTrattamentiImpianti(i_t).Item("pro_cod")) Then

                                HashProdottiOp.Add(DrTrattamentiImpianti(i_t).Item("pro_cod"), "")

                                XmlDatiDettaglio = XmlDocO.CreateElement("Dettaglio")

                                XmlDatiDettaglio.SetAttribute("pro_cod", DrTrattamentiImpianti(i_t).Item("pro_cod"))
                                XmlDatiDettaglio.SetAttribute("av_cod", DrTrattamentiImpianti(i_t).Item("av_cod"))
                                XmlDatiDettaglio.SetAttribute("av_gru", DrTrattamentiImpianti(i_t).Item("av_gru"))
                                XmlDatiDettaglio.SetAttribute("udm_cod", DrTrattamentiImpianti(i_t).Item("extra_int"))
                                XmlDatiDettaglio.SetAttribute("dose", DrTrattamentiImpianti(i_t).Item("qta_dett"))

                                XmlDatiDettaglio.SetAttribute("soglia_cod", DrTrattamentiImpianti(i_t).Item("soglia_cod"))

                                for_veg_av_dos_cod = "0"

                                formulatixallegatinormative_idriga = "0"
                                Dim ArrayDosiValue As String()
                                Dim d As Integer
                                If DrTrattamentiImpianti(i_t).Item("doseetichetta_value") <> "" Then
                                    ArrayDosiValue = Split(DrTrattamentiImpianti(i_t).Item("doseetichetta_value"), "<br>")
                                    If ArrayDosiValue IsNot Nothing Then
                                        For d = 0 To ArrayDosiValue.Length - 1
                                            for_veg_av_dos_cod &= Split(ArrayDosiValue(d), "$")(0) & ","
                                            If Split(ArrayDosiValue(d), "$").Length > 19 Then
                                                formulatixallegatinormative_idriga = Split(ArrayDosiValue(d), "$")(20)
                                            End If
                                        Next
                                    End If
                                    'elimino la virgola
                                    If for_veg_av_dos_cod <> "0" Then
                                        for_veg_av_dos_cod = Left(for_veg_av_dos_cod, for_veg_av_dos_cod.Length - 1)
                                    End If
                                    'for_veg_av_dos_cod = Split(xMovimento_Dettaglio.GetAttribute("doseetichetta_value"), "$")(0)
                                End If

                                XmlDatiDettaglio.SetAttribute("for_veg_av_dos_cod", for_veg_av_dos_cod)
                                XmlDatiDettaglio.SetAttribute("formulatixallegatinormative_idriga", formulatixallegatinormative_idriga)

                                for_veg_cod = "0"
                                If Not IsDBNull(DrTrattamentiImpianti(i_t).Item("extra_str")) AndAlso IsNumeric(DrTrattamentiImpianti(i_t).Item("extra_str")) Then
                                    for_veg_cod = DrTrattamentiImpianti(i_t).Item("extra_str")
                                End If
                                XmlDatiDettaglio.SetAttribute("for_veg_cod", for_veg_cod)

                                XmlDatiDettagli.AppendChild(XmlDatiDettaglio)

                            End If

                        Next

                        If strAppCoinvolti <> "" Then
                            strAppCoinvolti = Left(strAppCoinvolti, strAppCoinvolti.Length - 1)
                        End If

                        For i_t = 0 To DrTrattamentiImpianti.Length - 1
                            If Not IsDBNull(DrTrattamentiImpianti(i_t).Item("PrincipiAttiviPercAbb")) Then
                                Dim papa As String = DrTrattamentiImpianti(i_t).Item("PrincipiAttiviPercAbb").ToString
                                If Not String.IsNullOrEmpty(papa) Then
                                    listaPapa.Add(papa)
                                End If
                            End If
                        Next

                    End If
                End If

                Dim Acqua As Decimal
                Dim H2O As Decimal
                Dim H2O_Totale As Decimal
                Dim Tipo_Distribuzione_H2O As enum_TipoMezzo

                Acqua = Dt_Trattamenti.Rows(i).Item("Acqua")
                Select Case Acqua
                    Case < 0
                        H2O = -Acqua
                        H2O_Totale = H2O * Sup_Operazione
                        Tipo_Distribuzione_H2O = enum_TipoMezzo.Ettaro
                    Case Else
                        H2O_Totale = Acqua
                        H2O = H2O_Totale / Sup_Operazione
                        Tipo_Distribuzione_H2O = enum_TipoMezzo.Complessivo
                End Select


                XmlDatiGenerali.SetAttribute("h2o", H2O)
                XmlDatiGenerali.SetAttribute("h2o_totale", H2O_Totale)
                XmlDatiGenerali.SetAttribute("tipo_distribuzione_h2o", Tipo_Distribuzione_H2O)

                XmlDatiGenerali.SetAttribute("sup_totale", Sup_Totale)

                If flag_nuovo_controllo_riduzione_diserbo AndAlso listaPapa.Any Then
                    XmlDatiGenerali.SetAttribute("flag_nuovo_controllo_riduzione_diserbo", True)
                Else
                    XmlDatiGenerali.SetAttribute("flag_nuovo_controllo_riduzione_diserbo", False)
                End If


                XmlDatiGenerali.AppendChild(XmlDatiImpianti)
                XmlDatiGenerali.AppendChild(XmlDatiDettagli)

                XmlDatiWS.AppendChild(XmlDatiGenerali)

                xOperazione = XmlDoc.CreateElement("Operazione")
                xOperazione.SetAttribute("dpi_cod", Dpi_Cod)
                xOperazione.SetAttribute("biologico", Biologico)
                xOperazione.SetAttribute("id_agenda", Id_Agenda)

                xOperazione.SetAttribute("appezzamenti", strAppCoinvolti)
                xOperazione.SetAttribute("superficie", Sup_Coinvolta)


                xOperazione.InnerXml = XmlDatiWS.OuterXml

                xDatiOperazioni.AppendChild(xOperazione)

            Next

            Dim DatiVerifica As String = xDatiOperazioni.OuterXml

            Lrval = objDpiVerifica.Verifica_DifesaDiserbo(objParametri, objParametri_Utenti,
                                                            DatiVerifica, IDTestataTemp__tmp_FormulatiXPrincipiAttivi,
                                                            VerificaSoloControlliImpostazioniUtente, objParametri_SuperServer, leggiUrlDaConfigurazioniSiti)


        Catch ex As Exception

            Dim StrDummy As String

            StrDummy = ex.Message.ToString()

            Scrivi_LOG(objParametri, "Verifica_Trattamenti", StrDummy)

            Dim rRis = New rispostaStandard(Of Verifica_Disciplinare_Intervento)

            rRis.RispostaOK = False
            rRis.Errore = StrDummy

            Lrval.Add(rRis)

        End Try

        Return Lrval

    End Function

    Public Function Verifica_Raccolte(ByVal Dt_Raccolte As DataTable,
                                      ByVal Dt_Raccolte_Impianti As DataTable,
                                      ByVal Dt_Trattamenti_Impianti As DataTable,
                                      ByVal Dt_Carenze As DataTable,
                                      ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      ByRef objParametri_Utenti As AgronicaCoreParametri,
                                      Optional ByVal isFromVerificaCarenzaRaccoltaQdCNG As Boolean = False,
                                      Optional ByRef DtImpiantiNG As DataTable = Nothing
                                      ) As List(Of rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento))

        Dim Lr As New List(Of rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento))
        Dim rRis As rispostaStandard(Of Verifica_Disciplinare_Intervento)

        Dim Id_Agenda As Integer
        Dim Veg_Cod As Integer
        Dim Data As Date
        Dim Piva As String
        Dim Sa_Cod As Integer
        Dim Des_lib As String
        Dim Lav_Cod As Integer

        Dim Err_Cod As enTipoErrCode_Verifica
        Dim Err_Des As String

        Dim Codice_Piva As Integer = 0
        Dim Codice_SaCod As Integer = 1
        Dim Codice_Appezza As Integer = 2
        Dim Codice_IdReg As Integer = 3
        Dim Codice_ProgettoNome As Integer = 4
        Dim Codice_ValiditaInizio As Integer = 5
        Dim Codice_ValiditaFine As Integer = 6
        Dim Codice_GrfiCod As Integer = 7
        Dim Codice_CopCod As Integer = 8

        Try

            For i = 0 To Dt_Raccolte.Rows.Count - 1

                Id_Agenda = Dt_Raccolte.Rows(i).Item("id_agenda")
                Veg_Cod = Dt_Raccolte.Rows(i).Item("Veg_Cod")
                Data = Dt_Raccolte.Rows(i).Item("data_movimento")
                Piva = Dt_Raccolte.Rows(i).Item("piva")
                Sa_Cod = Dt_Raccolte.Rows(i).Item("sa_cod")
                Des_lib = Dt_Raccolte.Rows(i).Item("des_lib")
                Lav_Cod = Dt_Raccolte.Rows(i).Item("Lav_Cod")

                Dim HashImpianti As New Hashtable
                Dim HashProdotti As New Hashtable

                Dim XmlDoc As New XmlDocument

                Dim XmlDatiRisultati As XmlElement
                Dim XmlDatiGenerali As XmlElement
                Dim XmlDatiNonConformi As XmlElement

                Dim DatiInseriti(10, 0) As String
                Dim Destinazioni(9, 0) As String

                Dim Sup_Coinvolta As Decimal = 0
                Dim strAppCoinvolti As String = ""

                XmlDatiRisultati = XmlDoc.CreateElement("DatiRisultati")

                XmlDatiGenerali = XmlDoc.CreateElement("DatiGenerali")
                XmlDatiNonConformi = XmlDoc.CreateElement("DatiNonConformi")

                If Dt_Raccolte_Impianti IsNot Nothing Then

                    Dim DrRaccolteImpianti As DataRow() = Dt_Raccolte_Impianti.Select("id_agenda=" & Id_Agenda)

                    If DrRaccolteImpianti IsNot Nothing Then

                        'estraggo gli impianti
                        For i_t = 0 To DrRaccolteImpianti.Length - 1
                            If Not HashImpianti.ContainsKey(DrRaccolteImpianti(i_t).Item("piva") & "|" & DrRaccolteImpianti(i_t).Item("sa_cod") & "|" & DrRaccolteImpianti(i_t).Item("appezza") & "|" & DrRaccolteImpianti(i_t).Item("id_destinazione") & "|" & DrRaccolteImpianti(i_t).Item("progetto_cod")) Then
                                HashImpianti.Add(DrRaccolteImpianti(i_t).Item("piva") & "|" & DrRaccolteImpianti(i_t).Item("sa_cod") & "|" & DrRaccolteImpianti(i_t).Item("appezza") & "|" & DrRaccolteImpianti(i_t).Item("id_destinazione") & "|" & DrRaccolteImpianti(i_t).Item("progetto_cod"),
                                                 DrRaccolteImpianti(i_t).Item("progetto_nome") & "|" &
                                                 DrRaccolteImpianti(i_t).Item("validita_inizio_esercizio") & "|" & DrRaccolteImpianti(i_t).Item("validita_fine_esercizio") & "|" &
                                                 DrRaccolteImpianti(i_t).Item("grfi_cod") & "|" & DrRaccolteImpianti(i_t).Item("cop_cod"))
                                strAppCoinvolti += DrRaccolteImpianti(i_t).Item("app_nome") & ","
                                If Not IsDBNull(DrRaccolteImpianti(i_t).Item("qta2")) AndAlso IsNumeric(DrRaccolteImpianti(i_t).Item("qta2")) AndAlso CDec(DrRaccolteImpianti(i_t).Item("qta2")) > 0 Then
                                    Sup_Coinvolta += CDec(DrRaccolteImpianti(i_t).Item("qta2"))
                                Else
                                    Sup_Coinvolta += CDec(DrRaccolteImpianti(i_t).Item("sup_imp"))
                                End If

                            End If
                        Next

                        If strAppCoinvolti <> "" Then
                            strAppCoinvolti = Left(strAppCoinvolti, strAppCoinvolti.Length - 1)
                        End If

                        For Each key In HashImpianti.Keys

                            ReDim Preserve Destinazioni(9, UBound(Destinazioni, 2) + 1)

                            Destinazioni(Codice_Piva, UBound(Destinazioni, 2) - 1) = Split(key.ToString, "|")(0)
                            Destinazioni(Codice_SaCod, UBound(Destinazioni, 2) - 1) = Split(key.ToString, "|")(1)
                            Destinazioni(Codice_Appezza, UBound(Destinazioni, 2) - 1) = Split(key.ToString, "|")(2)
                            Destinazioni(Codice_IdReg, UBound(Destinazioni, 2) - 1) = Split(key.ToString, "|")(3)

                            Destinazioni(Codice_ProgettoNome, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(0)
                            Destinazioni(Codice_ValiditaInizio, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(1)
                            Destinazioni(Codice_ValiditaFine, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(2)

                            Destinazioni(Codice_GrfiCod, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(3)
                            Destinazioni(Codice_CopCod, UBound(Destinazioni, 2) - 1) = Split(HashImpianti(key).ToString, "|")(4)

                        Next

                    End If

                End If


                Dim strVerifica As String

                Dim Pro_Cod_Test As Integer
                Dim Pro_Cod As Integer
                Dim DataTest As Date
                Dim Data_Trattamento As Date
                Dim Descrizione As String = ""
                Dim For_Veg_Cod As Integer = 0



                For imp = 0 To UBound(Destinazioni, 2) - 1

                    strVerifica = DPI_Verifica_Carenza(Err_Cod,
                                                        CStr(Destinazioni(Codice_Piva, imp)),
                                                        CInt(Destinazioni(Codice_SaCod, imp)),
                                                        CInt(Destinazioni(Codice_Appezza, imp)),
                                                        CInt(Destinazioni(Codice_IdReg, imp)),
                                                        Veg_Cod,
                                                        CInt(Destinazioni(Codice_GrfiCod, imp)),
                                                        CInt(Destinazioni(Codice_CopCod, imp)),
                                                        CDate(Destinazioni(Codice_ValiditaInizio, UBound(Destinazioni, 2) - 1)),
                                                        CDate(Destinazioni(Codice_ValiditaFine, UBound(Destinazioni, 2) - 1)),
                                                        Data,
                                                        objParametri,
                                                        objParametri_Utenti,
                                                        Dt_Carenze)

                    Dim Verifica_Carenza As String = ""

                    Data_Trattamento = AGRODATAINIZIO

                    If Dt_Trattamenti_Impianti IsNot Nothing AndAlso Dt_Trattamenti_Impianti.Rows.Count > 0 Then

                        Dim DrTrattamentiImpianto As DataRow() = Dt_Trattamenti_Impianti.Select("piva='" & CStr(Destinazioni(Codice_Piva, imp)) & "' and sa_cod=" & CInt(Destinazioni(Codice_SaCod, imp)) & " and appezza =" & CInt(Destinazioni(Codice_Appezza, imp)) & " and id_destinazione =" & CInt(Destinazioni(Codice_IdReg, imp)) & " and Data_Movimento <'" & Data.ToShortDateString & "'", " data_movimento asc")

                        If DrTrattamentiImpianto IsNot Nothing AndAlso DrTrattamentiImpianto.Length > 0 Then

                            For iTra = 0 To DrTrattamentiImpianto.Length - 1

                                ''Verifico che l'intervento sia valido per il controllo
                                ''-- sia antecedente la data di raccolta non oltre 7 mesi
                                'If Data_Raccolta > CDate(DtTraccia.Rows(iTra).Item("Data_Movimento")) And _
                                '   Data_Raccolta < DateAdd("m", 7, CDate(DtTraccia.Rows(iTra).Item("Data_Movimento"))) Then

                                'Verifico che l'intervento sia valido per il controllo
                                '-- sia antecedente la data di raccolta e la fine della distinta

                                Dim Data_Trattamento_Attuale As Date = DrTrattamentiImpianto(iTra).Item("Data_Movimento")

                                If Data > Data_Trattamento_Attuale Then 'And
                                    'Data <Validita_Fine Then

                                    Pro_Cod_Test = DrTrattamentiImpianto(iTra).Item("Pro_Cod")
                                    For_Veg_Cod = 0
                                    If Not IsDBNull(DrTrattamentiImpianto(iTra).Item("extra_str")) AndAlso IsNumeric(DrTrattamentiImpianto(iTra).Item("extra_str")) Then
                                        For_Veg_Cod = DrTrattamentiImpianto(iTra).Item("extra_str")
                                    End If

                                    If Pro_Cod_Test <> 0 Then

                                        Dim Carenza As Integer = -1

                                        If Dt_Carenze IsNot Nothing Then

                                            Dim strFiltroProtetto = ""

                                            Select Case CInt(Destinazioni(Codice_CopCod, imp))
                                                Case 0, 3, 4, 5, 6, 1 'Nessuno
                                                    strFiltroProtetto = " AND flag_protetto<> 1"
                                                Case Else
                                                    strFiltroProtetto = " AND flag_protetto<> 2"
                                            End Select

                                            'Dim DrCarenza() As DataRow = Dt_Carenze.Select("fr_cod=" & Pro_Cod_Test & " AND validita_inizio<='" & Data & "' AND validita_fine>='" & Data & "'" & strFiltroProtetto)
                                            'If Not DrCarenza Is Nothing AndAlso DrCarenza.Length > 0 Then
                                            '    Carenza = DrCarenza(0).Item("tempocarenza")
                                            'End If

                                            'LC: Per recuperare la carenza da rispettare considero la data del trattamento precedentemente registrato e non della raccolta 
                                            Dim DrCarenza As DataRow()
                                            If For_Veg_Cod = 0 Then
                                                DrCarenza = Dt_Carenze.Select("fr_cod=" & Pro_Cod_Test & " AND validita_inizio<='" & Data_Trattamento_Attuale & "' AND validita_fine>='" & Data_Trattamento_Attuale & "'" & strFiltroProtetto)
                                            Else
                                                DrCarenza = Dt_Carenze.Select("fr_cod=" & Pro_Cod_Test & " AND for_veg_cod=" & For_Veg_Cod & " AND validita_inizio<='" & Data_Trattamento_Attuale & "' AND validita_fine>='" & Data_Trattamento_Attuale & "'" & strFiltroProtetto)
                                            End If
                                            If DrCarenza IsNot Nothing AndAlso DrCarenza.Length > 0 Then
                                                Carenza = DrCarenza(0).Item("tempocarenza")
                                            End If

                                        Else

                                            Dim objWs As New AgronicaCoreWebService.AgroWs
                                            Carenza = objWs.TempoCarenza_from_FrCod_VegCod(Pro_Cod_Test,
                                                                               Veg_Cod,
                                                                               For_Veg_Cod,
                                                                               CInt(Destinazioni(Codice_GrfiCod, imp)),
                                                                               CInt(Destinazioni(Codice_CopCod, imp)),
                                                                               Data,
                                                                               objParametri,
                                                                                objParametri_Utenti)
                                            objWs = Nothing
                                        End If

                                        If Carenza <> -1 AndAlso Carenza <> 0 Then

                                            'ai giorni di carenza aggiungo 1gg pe-r evitare controversie con i controllori

                                            DataTest = DateAdd("d", Carenza + 1, Data_Trattamento_Attuale)

                                            If DataTest > Data_Trattamento Then

                                                'Aggiornamento
                                                Pro_Cod = Pro_Cod_Test
                                                Data_Trattamento = DataTest

                                                If isFromVerificaCarenzaRaccoltaQdCNG Then
                                                    Descrizione = "distribuito il " & Data_Trattamento_Attuale.ToShortDateString() & " (Carenza: " & Carenza & " gg)"
                                                Else
                                                    Descrizione = " (Carenza: " & Carenza & " gg) in data " & Data_Trattamento_Attuale.ToShortDateString()
                                                End If

                                            End If

                                        End If

                                    End If

                                End If

                            Next iTra

                        End If

                        Select Case Data

                            Case AGRODATAINIZIO

                                Err_Cod = 0
                                Verifica_Carenza = "Nessun Formulato Distribuito"

                            Case Else

                                'Lettura del Prodotto
                                Dim ObjFormulati As New AgronicaCoreMetaSchemaDAL.Formulati_R
                                Dim DtFormulati As DataTable
                                Dim Pro_Des As String = ""

                                DtFormulati = ObjFormulati.Leggi(Pro_Cod,
                                                     AGRODATAINIZIO,
                                                     AGRODATAFINE,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "",
                                                    "",
                                                    objParametri)
                                ObjFormulati = Nothing

                                If DtFormulati.Rows.Count > 0 Then
                                    Pro_Des = LCase(DtFormulati.Rows(0).Item("Fr_Des"))
                                End If

                                If Data < Data_Trattamento Then
                                    Err_Cod = enTipoErrCode_Verifica.CarenzaNonRispettata
                                Else
                                    Err_Cod = 0 'Tempi di carenza rispettati
                                End If

                                If isFromVerificaCarenzaRaccoltaQdCNG Then
                                    Verifica_Carenza = Data_Trattamento & " - '" & Pro_Des & "' " & Descrizione
                                Else
                                    Verifica_Carenza = Data_Trattamento & " - Riferimento: '" & Pro_Des & "' " & Descrizione
                                End If

                        End Select

                    End If

                    If isFromVerificaCarenzaRaccoltaQdCNG Then
                        'Se arriviamo dalla verifica carenza x la raccolta, dal nuovo quaderno angular vado ad inserire la data carenza nella riga dell'impianto appena controllato
                        Dim DrImpiantoCorrenteNG As DataRow() = DtImpiantiNG.Select(
                                "piva = '" & CStr(Destinazioni(Codice_Piva, imp)) & "'" &
                                " AND sa_cod = " & CInt(Destinazioni(Codice_SaCod, imp)) &
                                " AND appezza = " & CInt(Destinazioni(Codice_Appezza, imp)) &
                                " AND id_reg = " & CInt(Destinazioni(Codice_IdReg, imp)))

                        If DrImpiantoCorrenteNG IsNot Nothing AndAlso DrImpiantoCorrenteNG.Length = 1 Then
                            DrImpiantoCorrenteNG(0).Item("DataCarenza") = Data_Trattamento
                            DrImpiantoCorrenteNG(0).Item("CarenzaStr") = Verifica_Carenza
                        End If

                        Continue For
                    End If

                    If Err_Cod <> 0 Then

                        Err_Des = "La raccolta non rispetta i tempi di carenza. La data di raccolta non è consentita prima del " & Verifica_Carenza & ". "

                        Inserisci_NonConformita(DatiInseriti,
                                                XmlDoc,
                                                XmlDatiNonConformi,
                                                Err_Cod,
                                                Err_Des,
                                                "Rispettare i tempi di carenza. ",
                                                0, 0, 0, "", "", 0, 0, "", "", "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                    End If
                Next

                If isFromVerificaCarenzaRaccoltaQdCNG Then
                    'In questo caso controlliamo una sola raccolta (corrente) e non ci interessa avere l'XML
                    Exit Function
                End If

                XmlDatiGenerali.SetAttribute("piva", Piva)
                XmlDatiGenerali.SetAttribute("sa_cod", Sa_Cod)
                XmlDatiGenerali.SetAttribute("data_movimento", Data.ToShortDateString)
                XmlDatiGenerali.SetAttribute("lav_cod", Lav_Cod)
                XmlDatiGenerali.SetAttribute("id_agenda", Id_Agenda)
                XmlDatiGenerali.SetAttribute("des_lib", Des_lib)

                XmlDatiGenerali.SetAttribute("veg_cod", Veg_Cod)
                XmlDatiGenerali.SetAttribute("veg_des", "")
                XmlDatiGenerali.SetAttribute("disciplinare_cod", "0")
                XmlDatiGenerali.SetAttribute("disciplinare_des", "")
                XmlDatiGenerali.SetAttribute("id_rcdpi", "0")
                XmlDatiGenerali.SetAttribute("rcdpi_des", "")
                XmlDatiGenerali.SetAttribute("tipotestata", "0")

                XmlDatiGenerali.SetAttribute("appezzamenti", strAppCoinvolti)
                XmlDatiGenerali.SetAttribute("superficie", Sup_Coinvolta)

                XmlDatiGenerali.AppendChild(XmlDatiNonConformi)
                XmlDatiRisultati.AppendChild(XmlDatiGenerali)

                XmlDoc.AppendChild(XmlDatiRisultati)

                Dim DatiVerifica As String = XmlDoc.OuterXml

                rRis = New rispostaStandard(Of Verifica_Disciplinare_Intervento)
                rRis = XmlFINALE(CStr(DatiVerifica),
                                            CInt(Id_Agenda),
                                            VerificaSoloControlliImpostazioniUtente,
                                            objParametri,
                                            objParametri_Utenti)

                Lr.Add(rRis)

            Next

        Catch ex As Exception

            Dim StrDummy As String

            StrDummy = ex.Message.ToString()

            Scrivi_LOG(objParametri, "Verifica_Raccolte", StrDummy)

            rRis = New rispostaStandard(Of Verifica_Disciplinare_Intervento)

            rRis.RispostaOK = False
            rRis.Errore = StrDummy

            Lr.Add(rRis)

        End Try

        Return Lr


    End Function

    Public Function DPI_Verifica_Raccolta(ByRef objParametri_Server As AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreParametri,
                                          ByVal DatiAgenda As String,
                                          ByVal Id_Agenda_Escluso As Integer,
                                          ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
                                          Optional ByVal Piva_Riferimento As String = "",
                                          Optional ByVal Sa_Cod_Riferimento As Integer = 0,
                                          Optional ByVal Appezza_Riferimento As Integer = 0,
                                          Optional ByVal Id_Reg_Riferimento As Integer = 0,
                                          Optional ByVal DTCarenze As DataTable = Nothing,
                                          Optional isFromAgendaNG As Boolean = False
                                          ) As rispostaStandard(Of Verifica_Disciplinare_Intervento)


        Dim r As New rispostaStandard(Of Verifica_Disciplinare_Intervento)
        r.RispostaStringa = New Verifica_Disciplinare_Intervento

        Dim DatiVerifica As String

        Try

            DatiVerifica = Verifica_Raccolta(CStr(DatiAgenda),
                                            CInt(Id_Agenda_Escluso),
                                            CStr(Piva_Riferimento),
                                            CInt(Sa_Cod_Riferimento),
                                            CInt(Appezza_Riferimento),
                                            CInt(Id_Reg_Riferimento),
                                            objParametri_Server,
                                            objParametri_Utenti,
                                            DTCarenze,
                                            isFromAgendaNG)

            Dim rRis As New rispostaStandard(Of Verifica_Disciplinare_Intervento)
            rRis = XmlFINALE(CStr(DatiVerifica),
                                        CInt(Id_Agenda_Escluso),
                                        VerificaSoloControlliImpostazioniUtente,
                                        objParametri_Server,
                                        objParametri_Utenti,
                                        DatiAgenda)

            If rRis.RispostaOK Then
                r.RispostaOK = True
                r.RispostaStringa.Risultato = rRis.RispostaStringa.Risultato
            Else
                Throw New Exception(rRis.Errore)
            End If


        Catch exc As Exception

            r.RispostaOK = False
            r.Errore = exc.Message

        End Try

        Return r

    End Function

    '============================================================================
    Public Function Verifica_Raccolta(ByVal DatiAgenda As String,
                                      ByVal Id_Agenda_Escluso As Long,
                                      ByVal Piva_Riferimento As String,
                                      ByVal Sa_Cod_Riferimento As Integer,
                                      ByVal Appezza_Riferimento As Integer,
                                      ByVal Id_Reg_Riferimento As Integer,
                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                      ByRef objParametri_Utenti As AgronicaCoreParametri,
                                      Optional ByVal DTCarenze As DataTable = Nothing,
                                      Optional isFromAgendaNG As Boolean = False
                                      ) As String
        '============================================================================

        Dim NomeRoutine As String = "DpiBIZ.DPI_Verifica.DPI_Verifica_Raccolta()"

        Dim MessaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction

        Dim i As Int32

        Dim RisultatoFunzione As String

        Dim XmlDoc As New XmlDocument
        Dim XmlDom As New XmlDocument

        Dim XmlDatiRisultati As XmlElement
        Dim XmlDatiGenerali As XmlElement
        Dim XmlDatiNonConformi As XmlElement

        Dim PIVA As String = ""
        Dim Sa_Cod As Integer
        Dim Appezza As Integer
        Dim Id_Reg As Integer
        Dim Lav_Cod As Integer
        Dim Id_Agenda As Integer
        Dim Des_Lib As String = ""
        Dim Data As Date
        Dim Veg_Cod As Integer
        Dim Veg_Des As String = ""
        Dim Dummy As String
        Dim bScheda As Boolean

        Dim Err_Code As Integer
        Dim Err_Des As String
        Dim TipoOperazioneDB As Integer

        Dim xDatiAgenda As XmlElement
        Dim xAgenda As XmlElement
        Dim xDatiAgende As XmlNodeList
        Dim xAgende As XmlNodeList

        Dim xDatiMovimenti As XmlNodeList
        Dim xDatiMovimento As XmlElement
        Dim xMovimenti As XmlNodeList
        Dim xMovimento As XmlElement

        Dim xDatiMovimenti_Dettagli As XmlNodeList
        Dim xDatiMovimento_Dettaglio As XmlElement
        Dim xMovimenti_Dettagli As XmlNodeList
        Dim xMovimento_Dettaglio As XmlElement

        Dim xMov_Destinazioni As XmlNodeList
        Dim xMov_Destinazione As XmlElement

        Dim i_DatiAgenda As Integer
        Dim i_Agenda As Integer
        Dim i_DatiMovimento As Integer
        Dim i_Movimento As Integer
        Dim i_DatiMovimenti_Dettagli As Integer
        Dim i_DatiMovimento_Dettaglio As Integer
        Dim i_DatiMov_Destinazioni As Integer

        Dim Destinazioni As String(,) = Nothing
        Dim DatiInseriti As String(,)

        Dim bImpianto_Coerente As Boolean
        Dim bImpianti_Impostati As Boolean

        Try

            '------------------------------
            'Verifico se è stata impostata una connessione
            If IsNothing(objParametri_Server.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri_Server.StringaConnessione)
                xTransazione = Nothing
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri_Server.objConnessione
                xConnectionState = objParametri_Server.objConnessione.State
                xTransazione = objParametri_Server.objTransazione
            End If

            '------------------------------

            '================================================================================================================

            XmlDom.LoadXml(DatiAgenda)

            'Creazione Istanze
            'Dim ObjImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

            XmlDatiRisultati = XmlDoc.CreateElement("DatiRisultati")

            '----- < DatiGenerali > ----
            XmlDatiGenerali = XmlDoc.CreateElement("DatiGenerali")
            XmlDatiNonConformi = XmlDoc.CreateElement("DatiNonConformi")

            '############################################################################################
            '################# Lettura dei Parametri Base di Agenda     #################################
            '############################################################################################

            xDatiAgende = XmlDom.GetElementsByTagName("DatiAgenda")

            i_DatiAgenda = 0

            Do While i_DatiAgenda < xDatiAgende.Count

                'Prelevo l'i-esimo blocco di DatiAgenda (in realtà ne esiste uno solo)
                xDatiAgenda = xDatiAgende.Item(i_DatiAgenda)

                '------------------------------

                xAgende = xDatiAgenda.GetElementsByTagName("Agenda")

                i_Agenda = 0

                Do While i_Agenda < xAgende.Count

                    'Prelevo l' i-esima Codifica Agenda
                    xAgenda = xAgende.Item(i_Agenda)

                    ReDim DatiInseriti(11, 0)

                    'Parametri agenda
                    TipoOperazioneDB = Agro_SQL_SaveNum(xAgenda.GetAttribute("TipoOperazioneDB"))

                    PIVA = xAgenda.GetAttribute("piva")
                    Sa_Cod = CInt(xAgenda.GetAttribute("sa_cod"))
                    Lav_Cod = CInt(xAgenda.GetAttribute("lav_cod"))
                    Id_Agenda = CInt(xAgenda.GetAttribute("id_agenda"))
                    Des_Lib = xAgenda.GetAttribute("des_lib")
                    Veg_Cod = -1
                    bImpianti_Impostati = False


                    '############################################################################################
                    '################# Lettura della Scheda di Lavorazione della Lavorazione     ################
                    '############################################################################################

                    xDatiMovimenti = xAgenda.GetElementsByTagName("DatiMovimenti")

                    i_DatiMovimento = 0

                    'Check di esistenza Informazioni
                    Do While i_DatiMovimento < xDatiMovimenti.Count

                        xDatiMovimento = xDatiMovimenti.Item(i_DatiMovimento)

                        xMovimenti = xDatiMovimento.GetElementsByTagName("Movimento")

                        i_Movimento = 0

                        Do While i_Movimento < xMovimenti.Count

                            xMovimento = xMovimenti.Item(i_Movimento)

                            'Verifico che il movimento sia la scheda di lavorazione
                            Select Case xMovimento.GetAttribute("cau_mov")

                                Case "2200" 'Rilievi alla raccolta

                                    bScheda = True

                                Case Else

                                    bScheda = False

                            End Select

                            '====================================================================================

                            If bScheda Then

                                '------------------------------

                                '====================================================================================
                                'Prelevo le Informazioni sulle Note, sulla Data di Operazione e sull'Username creazione
                                '------------------------------------------------------------------------------------
                                Data = CDate(xMovimento.GetAttribute("data_movimento")).ToShortDateString

                                '====================================================================================
                                'Prelevo le Informazioni sugli Impianti
                                '------------------------------------------------------------------------------------
                                xDatiMovimenti_Dettagli = xMovimento.GetElementsByTagName("DatiMovimenti_Dettagli")

                                i_DatiMovimenti_Dettagli = 0

                                Do While i_DatiMovimenti_Dettagli < xDatiMovimenti_Dettagli.Count

                                    'Prelevo l'i-esimo Blocco di Movimenti Dettagli (ne esiste 1 solo)
                                    xDatiMovimento_Dettaglio = xDatiMovimenti_Dettagli.Item(i_DatiMovimenti_Dettagli)

                                    xMovimenti_Dettagli = xDatiMovimento_Dettaglio.GetElementsByTagName("Movimento_Dettaglio")


                                    i_DatiMovimento_Dettaglio = 0

                                    Do While i_DatiMovimento_Dettaglio < xMovimenti_Dettagli.Count

                                        'Prelevo l'i-esimo Movimento Dettaglio
                                        xMovimento_Dettaglio = xMovimenti_Dettagli.Item(i_DatiMovimento_Dettaglio)

                                        If Not bImpianti_Impostati Then

                                            '==========================================================================
                                            'Lettura della specie vegetale e dei dettagli degli impianti
                                            '-------------------------------------------------------------------------

                                            xMov_Destinazioni = xMovimento_Dettaglio.GetElementsByTagName("Movimento_Destinazione")

                                            i_DatiMov_Destinazioni = 0

                                            ReDim Destinazioni(9, 0)

                                            Dim ArrayPiva(0) As String
                                            Dim ArraySaCod(0) As Integer
                                            Dim ArrayAppezza(0) As Integer
                                            Dim ArrayIdReg(0) As Integer
                                            Dim N_Imp As Integer = 0

                                            Do While i_DatiMov_Destinazioni < xMov_Destinazioni.Count

                                                'Prelevo l'i-esimo Movimento Dettaglio
                                                xMov_Destinazione = xMov_Destinazioni.Item(i_DatiMov_Destinazioni)

                                                If CLng(xMov_Destinazione.GetAttribute("tipo_destinazione")) = 0 Then

                                                    'Verifico che l'impianto sia coerente con quello eventualmente passato come riferimento
                                                    If Trim(Piva_Riferimento) <> "" AndAlso Sa_Cod_Riferimento <> 0 AndAlso
                                                       Appezza_Riferimento <> 0 AndAlso Id_Reg_Riferimento <> 0 AndAlso
                                                       (Appezza_Riferimento <> CInt(xMov_Destinazione.GetAttribute("appezza")) OrElse
                                                       Id_Reg_Riferimento <> CInt(xMov_Destinazione.GetAttribute("id_destinazione"))) Then

                                                        'è stato impostato un impianto come riferimento ed è diverso da quello di lavorazione
                                                        bImpianto_Coerente = False

                                                    Else 'Ok
                                                        bImpianto_Coerente = True

                                                    End If

                                                    If bImpianto_Coerente Then

                                                        '===================================================================================================================================
                                                        'Aggiornamento struttura di appoggio delle destinazioni di intervento
                                                        '-----------------------------------------------------------------------------------------------------------------------------------
                                                        ReDim Preserve Destinazioni(9, UBound(Destinazioni, 2) + 1)

                                                        Destinazioni(0, UBound(Destinazioni, 2) - 1) = CStr(xMov_Destinazione.GetAttribute("piva"))
                                                        Destinazioni(1, UBound(Destinazioni, 2) - 1) = CInt(xMov_Destinazione.GetAttribute("sa_cod"))
                                                        Destinazioni(2, UBound(Destinazioni, 2) - 1) = CInt(xMov_Destinazione.GetAttribute("appezza"))
                                                        Destinazioni(3, UBound(Destinazioni, 2) - 1) = CInt(xMov_Destinazione.GetAttribute("id_destinazione"))

                                                        '-------
                                                        ReDim Preserve ArrayPiva(N_Imp)
                                                        ReDim Preserve ArraySaCod(N_Imp)
                                                        ReDim Preserve ArrayAppezza(N_Imp)
                                                        ReDim Preserve ArrayIdReg(N_Imp)

                                                        ArrayPiva(N_Imp) = PIVA
                                                        ArraySaCod(N_Imp) = Sa_Cod
                                                        ArrayAppezza(N_Imp) = CInt(xMov_Destinazione.GetAttribute("appezza"))
                                                        ArrayIdReg(N_Imp) = CInt(xMov_Destinazione.GetAttribute("id_destinazione"))
                                                        N_Imp += 1

                                                    End If 'Fine controllo impianto coerente

                                                End If

                                                i_DatiMov_Destinazioni = i_DatiMov_Destinazioni + 1

                                            Loop

                                            bImpianti_Impostati = True


                                            If Destinazioni IsNot Nothing AndAlso UBound(Destinazioni, 2) Then

                                                Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                                Dim DtImp As DataTable
                                                Dim I_Imp As Integer
                                                Dim DrImp As DataRow()

                                                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Data, Data)
                                                DtImp = objImp.Leggi_Dati_Impianti_Distinte(ArrayPiva,
                                                                                    ArraySaCod,
                                                                                    ArrayAppezza,
                                                                                    ArrayIdReg,
                                                                                    "", "",
                                                                                    objParametri_Server)
                                                objParametri_Server.ResettaFinestra()

                                                If DtImp IsNot Nothing Then

                                                    For I_Imp = 0 To UBound(Destinazioni, 2) - 1

                                                        DrImp = DtImp.Select(" Piva='" & Agro_SQL_SaveText(Destinazioni(0, I_Imp)) & "'" &
                                                                             " AND Sa_Cod=" & Agro_SQL_SaveNum(Destinazioni(1, I_Imp)) &
                                                                             " AND Appezza=" & Agro_SQL_SaveNum(Destinazioni(2, I_Imp)) &
                                                                             " AND Id_Reg=" & Agro_SQL_SaveNum(Destinazioni(3, I_Imp)) & "")

                                                        If DrImp IsNot Nothing AndAlso DrImp.Length > 0 Then

                                                            Veg_Cod = DrImp(0).Item("Veg_Cod")
                                                            Veg_Des = DrImp(0).Item("Veg_Des")

                                                            Destinazioni(4, UBound(Destinazioni, 2) - 1) = DrImp(0).Item("Progetto_Nome")
                                                            Destinazioni(5, UBound(Destinazioni, 2) - 1) = DrImp(0).Item("Validita_Inizio_Distinta")
                                                            Destinazioni(6, UBound(Destinazioni, 2) - 1) = DrImp(0).Item("Validita_Fine_Distinta")

                                                            Destinazioni(7, UBound(Destinazioni, 2) - 1) = DrImp(0).Item("Grfi_Cod")
                                                            Destinazioni(8, UBound(Destinazioni, 2) - 1) = DrImp(0).Item("Cop_Cod")

                                                        Else
                                                            Destinazioni(4, UBound(Destinazioni, 2) - 1) = ""
                                                            Destinazioni(5, UBound(Destinazioni, 2) - 1) = "01/01/1900"
                                                            Destinazioni(6, UBound(Destinazioni, 2) - 1) = "31/12/2100"

                                                            Destinazioni(7, UBound(Destinazioni, 2) - 1) = "0"
                                                            Destinazioni(8, UBound(Destinazioni, 2) - 1) = "0"

                                                        End If
                                                    Next

                                                End If

                                            End If

                                        End If

                                        '================================================================================================================
                                        'Controllo che esista almeno un impianto coerente per la lavorazione

                                        If UBound(Destinazioni, 2) = 0 AndAlso Not (isFromAgendaNG) Then

                                            Err_Code = 701
                                            Err_Des = "L'intervento colturale non può essere analizzato perché non riferito ad impianti colturali validi. "

                                            Inserisci_NonConformita(DatiInseriti,
                                                                    XmlDoc,
                                                                    XmlDatiNonConformi,
                                                                    Err_Code,
                                                                    Err_Des,
                                                                    "Verificare l'intervento colturale. ",
                                                                    0, 0, 0, "", "", 0, 0, "", "", "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)



                                            'Uscita
                                            Exit Do

                                        Else

                                            '###########################################################################################
                                            '##################### VERIFICA TEMPI DI CARENZA ###########################################
                                            '###########################################################################################

                                            For i = 0 To UBound(Destinazioni, 2) - 1

                                                Appezza = CInt(Destinazioni(2, i))
                                                Id_Reg = CInt(Destinazioni(3, i))

                                                Dummy = DPI_Verifica_Carenza(Err_Code,
                                                                            PIVA, Sa_Cod, Appezza, Id_Reg,
                                                                            Veg_Cod,
                                                                            CInt(Destinazioni(7, i)),
                                                                             CInt(Destinazioni(8, i)),
                                                                            CDate(Destinazioni(5, UBound(Destinazioni, 2) - 1)),
                                                                            CDate(Destinazioni(6, UBound(Destinazioni, 2) - 1)),
                                                                             Data,
                                                                            objParametri_Server,
                                                                            objParametri_Utenti,
                                                                            DTCarenze)

                                                If Err_Code <> 0 Then

                                                    Err_Des = "La raccolta non rispetta i tempi di carenza. La data di raccolta non è consentita prima del " & Dummy & ". "

                                                    Inserisci_NonConformita(DatiInseriti,
                                                                            XmlDoc,
                                                                            XmlDatiNonConformi,
                                                                            Err_Code,
                                                                            Err_Des,
                                                                            "Rispettare i tempi di carenza. ",
                                                                            0, 0, 0, "", "", 0, 0, "", "", "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)

                                                End If

                                            Next i

                                        End If

                                        i_DatiMovimento_Dettaglio = i_DatiMovimento_Dettaglio + 1

                                    Loop

                                    i_DatiMovimenti_Dettagli = i_DatiMovimenti_Dettagli + 1

                                Loop

                            End If

                            i_Movimento = i_Movimento + 1



                        Loop

                        i_DatiMovimento = i_DatiMovimento + 1

                    Loop


                    i_Agenda = i_Agenda + 1


                Loop

                i_DatiAgenda = i_DatiAgenda + 1

            Loop



            XmlDatiGenerali.SetAttribute("piva", PIVA)
            XmlDatiGenerali.SetAttribute("sa_cod", Sa_Cod)
            XmlDatiGenerali.SetAttribute("data_movimento", Data.ToShortDateString)
            XmlDatiGenerali.SetAttribute("lav_cod", Lav_Cod)
            XmlDatiGenerali.SetAttribute("id_agenda", Id_Agenda)
            XmlDatiGenerali.SetAttribute("des_lib", Des_Lib)

            XmlDatiGenerali.SetAttribute("veg_cod", Veg_Cod)
            XmlDatiGenerali.SetAttribute("veg_des", Veg_Des)
            XmlDatiGenerali.SetAttribute("disciplinare_cod", "0") 'Inutile
            XmlDatiGenerali.SetAttribute("disciplinare_des", "") 'Inutile
            XmlDatiGenerali.SetAttribute("id_rcdpi", "0") 'Inutile
            XmlDatiGenerali.SetAttribute("rcdpi_des", "") 'Inutile
            XmlDatiGenerali.SetAttribute("tipotestata", "0") 'Inutile

            XmlDatiGenerali.AppendChild(XmlDatiNonConformi)
            XmlDatiRisultati.AppendChild(XmlDatiGenerali)

            XmlDoc.AppendChild(XmlDatiRisultati)

            Return XmlDoc.OuterXml

        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

            'Restituisco un valore Dummy
            Return "-1"

        Finally

            If FlagConnessioneLocale Then
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

    End Function


    '================================================================================================
    ''Calcolo Apporto Macroelementi Distribuito/Ha
    ''================================================================================================
    'Public Sub DPI_Fertilizzante_Distribuito(ByRef N_Distribuito As Decimal, _
    '                                         ByRef P_Distribuito As Decimal, _
    '                                         ByRef K_Distribuito As Decimal, _
    '                                         ByRef Mg_Distribuito As Decimal, _
    '                                         ByVal PIVA As String, _
    '                                         ByVal Sa_Cod As Int32, _
    '                                         ByVal Appezza As Int32, _
    '                                         ByVal Id_Reg As Int32, _
    '                                         ByVal Sup_Imp As Decimal, _
    '                                         ByVal Id_Agenda_Escluso As Int32, _
    '                                         ByVal Validita_Inizio As Date, _
    '                                         ByVal Validita_Fine As Date, _
    '                                         ByRef objParametri As AgronicaCoreParametri)
    '    '============================================================================

    '    Dim FlagConnessioneLocale As Boolean = False
    '    Dim xConnessione As OleDbConnection

    '    Dim xTransazione As OleDbTransaction
    '    Dim i As Int32
    '    Dim j As Int32
    '    Dim xConnectionState As ConnectionState = ConnectionState.Closed
    '    Dim NomeRoutine As String = "DpiBIZ.DPI_Verifica_.DPI_Fertilizzante_Distribuito()"

    '    Dim MessaggioErrore As String = ""

    '    Dim RisultatoFunzione As String

    '    Dim N_Prodotto As Decimal
    '    Dim P_Prodotto As Decimal
    '    Dim K_Prodotto As Decimal
    '    Dim Mg_Prodotto As Decimal


    '    '------------------------------

    '    Const MetodoNome = "DPI_Fertilizzante_Distribuito:"

    '    '------------------------------

    '    Try

    '        '------------------------------
    '        'Verifico se è stata impostata una connessione
    '        If IsNothing(objParametri.objConnessione) Then
    '            'Flag
    '            FlagConnessioneLocale = True
    '            'Creo la connessione localmente
    '            xConnessione = New OleDbConnection(objParametri.StringaConnessione)
    '            xTransazione = Nothing
    '        Else
    '            'Utilizzo quella passata come parametro
    '            xConnessione = objParametri.objConnessione
    '            xConnectionState = objParametri.objConnessione.State
    '            xTransazione = objParametri.objTransazione
    '        End If


    '        '===============================================================================
    '        'CONTROLLO MASSIMO APPORTO MACROELEMENTI
    '        '-------------------------------------------------------------------------------

    '        Dim ObjTracciabilita As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
    '        Dim DtTracciabilita As DataTable

    '        'ObjTracciabilita = CreateObject("Agro_Contab_AD.Movimenti_Dettagli_R")

    '        'Lettura tracciabilità di fertilizzanti distribuiti sul singolo impianto
    '        DtTracciabilita = ObjTracciabilita.LeggiTracciabilita( _
    '                                        PIVA, _
    '                                        Sa_Cod, _
    '                                         0, 0, 0, 3, 0, 0, 0, _
    '                                        Appezza, _
    '                                        Id_Reg, _
    '                                        0, "", 0, 0, 0, 0, 0, "", _
    '                                        Validita_Inizio, _
    '                                        Validita_Fine, _
    '                                        enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                        "", _
    '                                        "", _
    '                                        objParametri)

    '        If DtTracciabilita.Rows.Count <> 0 Then

    '            'Do While Not RsTracciabilita.EOF
    '            Dim iTra As Integer
    '            For iTra = 0 To DtTracciabilita.Rows.Count - 1

    '                'Controllo che l'Id_Agenda non sia già stato verificato
    '                If CLng(DtTracciabilita.Rows(iTra).Item("Id_Agenda")) <> Id_Agenda_Escluso Then

    '                    MacroElementi_da_Prodotto(PIVA, _
    '                                              CLng(DtTracciabilita.Rows(iTra).Item("Pro_Cod")), _
    '                                              CLng(DtTracciabilita.Rows(iTra).Item("Mat_Cod")), _
    '                                              N_Prodotto, _
    '                                              P_Prodotto, _
    '                                              K_Prodotto, _
    '                                              Mg_Prodotto, _
    '                                              objParametri)

    '                    'Aggiornamenti Macroelementi Distribuiti
    '                    N_Distribuito = N_Distribuito + (CDbl(DtTracciabilita.Rows(iTra).Item("Qta")) * N_Prodotto / (100 * Sup_Imp))
    '                    P_Distribuito = P_Distribuito + (CDbl(DtTracciabilita.Rows(iTra).Item("Qta")) * P_Prodotto / (100 * Sup_Imp))
    '                    K_Distribuito = K_Distribuito + (CDbl(DtTracciabilita.Rows(iTra).Item("Qta")) * K_Prodotto / (100 * Sup_Imp))
    '                    Mg_Distribuito = Mg_Distribuito + (CDbl(DtTracciabilita.Rows(iTra).Item("Qta")) * Mg_Prodotto / (100 * Sup_Imp))


    '                End If

    '            Next iTra

    '        End If

    '        '------------------------------


    '    Catch ex As Exception

    '        RisultatoFunzione = ""
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)


    '    Finally

    '        If FlagConnessioneLocale = True Then
    '            If Not IsNothing(xConnessione) Then
    '                xConnessione.Close()
    '                xConnessione.Dispose()
    '            End If
    '        Else
    '            If xConnectionState = ConnectionState.Closed Then
    '                xConnessione.Close()
    '            End If
    '        End If

    '    End Try

    'End Sub


    '================================================================================================
    '================================================================================================
    '================================================================================================
    'NON METTERE QUESTA ROUTINE QUI!!!!!!!
    'NEI DPI VANNO SOLO LE ROUTINE UTILIZZATE DAL WEB SERVICE!!!!!!
    '================================================================================================
    '================================================================================================
    '================================================================================================

    '    '================================================================================================
    '    'Calcolo Apporto Massimo Macroelementi
    '    '================================================================================================
    '    Public Sub DPI_Apporto_Massimo_MacroElementi(ByRef N_Massimo As Decimal, _
    '                                                 ByRef P_Massimo As Decimal, _
    '                                                 ByRef K_Massimo As Decimal, _
    '                                                 ByRef Mg_Massimo As Decimal, _
    '                                                 ByVal PIVA As String, _
    '                                                 ByVal Sa_Cod As Int32, _
    '                                                 ByVal Appezza As Int32, _
    '                                                 ByVal Id_Reg As Int32, _
    '                                                 ByVal Progetto_Cod As Int32, _
    '                                                 ByRef objParametri As AgronicaCoreParametri)
    '        '============================================================================

    '        Dim NomeRoutine As String = "DpiBIZ.DPI_Apporto_Massimo_MacroElementi()"
    '        Dim MessaggioErrore As String = ""
    '        Dim FlagConnessioneLocale As Boolean = False
    '        Dim xConnessione As OleDbConnection
    '         
    '        Dim xTransazione As OleDbTransaction
    '        Dim i As Int32
    '        Dim j As Int32
    '        Dim RisultatoFunzione As String = String.Empty

    '        '------------------------------

    '        Const MetodoNome = "DPI_Apporto_Massimo_MacroElementi:"

    '        '------------------------------

    '        Try

    '            '------------------------------
    '            'Verifico se e' stata impostata una connessione
    '            If IsNothing(objParametri.objConnessione) Then
    '                'Flag
    '                FlagConnessioneLocale = True
    '                'Creo la connessione localmente
    '                xConnessione = New OleDbConnection(objParametri.StringaConnessione)
    '                xTransazione = Nothing
    '            Else
    '                'Utilizzo quella passata come parametro
    '                xConnessione = objParametri.objConnessione
    '                xConnectionState = objParametri.objConnessione.State
    '                xTransazione = objParametri.objTransazione
    '            End If



    '            '===========================================================================================
    '            'Lettura dei massimi apporti macroelementi
    '            '-------------------------------------------------------------------------------------------

    '            'ObjCodici = CreateObject("Agro_Anagrafe_AD.Reg_Impianti_Codici_R")
    '            Dim ObjCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
    '            Dim DtCodici As DataTable

    '            DtCodici = ObjCodici.LeggixProgetto(PIVA, _
    '                                                    Sa_Cod, _
    '                                                    Appezza, _
    '                                                    Id_Reg, _
    '                                                    Progetto_Cod, _
    '                                                    0, _
    '                                                    0, _
    '                                                    "", _
    '                                                    enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                    "", _
    '                                                    "", _
    '                                                    objParametri)

    '            N_Massimo = 9999 'Dummy
    '            P_Massimo = 9999 'Dummy
    '            K_Massimo = 9999 'Dummy
    '            Mg_Massimo = 9999 'Dummy


    '            If DtCodici.Rows.Count <> 0 Then

    '                Dim Dr() As DataRow

    '                'Azoto
    '                Dr = DtCodici.Select("id_cod = 1050")
    '                If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
    '                    If IsNumeric(Dr(0).Item("val_cod")) Then
    '                        N_Massimo = Dr(0).Item("val_cod")
    '                    End If
    '                End If

    '                'Potassio
    '                Dr = DtCodici.Select("id_cod = 1051")
    '                If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
    '                    If IsNumeric(Dr(0).Item("val_cod")) Then
    '                        P_Massimo = Dr(0).Item("val_cod")
    '                    End If
    '                End If

    '                'Fosforo
    '                Dr = DtCodici.Select("id_cod = 1052")
    '                If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
    '                    If IsNumeric(Dr(0).Item("val_cod")) Then
    '                        K_Massimo = Dr(0).Item("val_cod")
    '                    End If
    '                End If

    '                'Magnesio
    '                Dr = DtCodici.Select("id_cod = 1053")
    '                If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
    '                    If IsNumeric(Dr(0).Item("val_cod")) Then
    '                        Mg_Massimo = Dr(0).Item("val_cod")
    '                    End If
    '                End If

    '            End If


    '            '===========================================================================================
    '        Catch ex As Exception

    '            RisultatoFunzione = ""
    '            MessaggioErrore = ex.Message
    '            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)


    '        Finally

    '            If FlagConnessioneLocale = True Then
    '                If Not IsNothing(xConnessione) Then
    '                    xConnessione.Close()
    '                    xConnessione.Dispose()
    '                End If
    '            Else
    '                If xConnectionState = ConnectionState.Closed Then
    '                    xConnessione.Close()
    '                End If
    '            End If

    '        End Try


    '    End Sub





    '###############################################################################
    Public Function DPI_Verifica_Piano_Concimazione(ByRef objParametri_Server As AgronicaCoreParametri,
                                                    ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                    ByVal UserProfilo_CodFisc As String,
                                                    ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Appezza As Integer,
                                                    ByVal Id_Reg As Integer,
                                                    ByVal Modalita_Xml As Boolean
                                                    ) As String

        Dim StringaXmlDPIVerifica As String

        Try

            StringaXmlDPIVerifica = Verifica_Piano_Concimazione(CStr(UserProfilo_CodFisc),
                                                                           CStr(Piva),
                                                                           CInt(Sa_Cod),
                                                                           CInt(Appezza),
                                                                           CInt(Id_Reg),
                                                                           CBool(Modalita_Xml),
                                                                           objParametri_Server,
                                                                           objParametri_Utenti)
            Return StringaXmlDPIVerifica

        Catch ex As Exception

            Dim StrDummy As String

            'Messaggio di errore
            StrDummy = ex.Message.ToString()

            Scrivi_LOG(objParametri_Server, "DPI_Verifica_Piano_Concimazione", StrDummy)

        End Try

    End Function


    '============================================================================
    Public Function Verifica_Piano_Concimazione(ByVal PIVA As String,
                                                    ByVal Sa_Cod As Int32,
                                                    ByVal Appezza As Int32,
                                                    ByVal Id_Reg As Int32,
                                                    ByVal Progetto_Cod As Int32,
                                                    ByVal Modalita_Xml As Boolean,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    ByRef objParametri_Utenti As AgronicaCoreParametri
                                                    ) As String
        '============================================================================

        Dim NomeRoutine As String = "DpiBIZ.DPI_Verifica_R.Verifica_Piano_Concimazione()"
        Dim MessaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction
        Dim RisultatoFunzione As String = String.Empty

        Dim XmlDoc As New XmlDocument

        Dim XmlDatiRisultati As XmlElement
        Dim XmlDatiGenerali As XmlElement
        Dim XmlDatiNonConformi As XmlElement

        Dim DatiInseriti As String(,)

        Try

            '------------------------------
            'Verifico se è stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
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

            XmlDatiRisultati = XmlDoc.CreateElement("DatiRisultati")

            '----- < DatiGenerali > ----
            XmlDatiGenerali = XmlDoc.CreateElement("DatiGenerali")
            XmlDatiNonConformi = XmlDoc.CreateElement("DatiNonConformi")

            ReDim DatiInseriti(10, 0)

            'Verifica Apporto MacroElementi
            Dim Hash_FormulatiPA As New Hashtable
            Dim Hash_FormulatiPAPesi As New Hashtable

            DPI_Verifica_Apporto_MacroElementi(DatiInseriti,
                                               XmlDoc,
                                               XmlDatiNonConformi,
                                               PIVA,
                                               Sa_Cod,
                                               Appezza,
                                               Id_Reg,
                                               Progetto_Cod,
                                               "",
                                               AGRODATAINIZIO,
                                               AGRODATAFINE,
                                               0, 0,
                                               AGRODATAINIZIO,
                                               0,
                                               0, 0, 0, 0, 0, 0,
                                               objParametri, objParametri_Utenti,
                                               Hash_FormulatiPA, Hash_FormulatiPAPesi)

            XmlDatiGenerali.SetAttribute("piva", PIVA)
            XmlDatiGenerali.SetAttribute("sa_cod", Sa_Cod)
            XmlDatiGenerali.SetAttribute("appezza", Appezza)
            XmlDatiGenerali.SetAttribute("id_reg", Id_Reg)
            XmlDatiGenerali.SetAttribute("progetto_cod", Progetto_Cod)

            XmlDatiGenerali.AppendChild(XmlDatiNonConformi)
            XmlDatiRisultati.AppendChild(XmlDatiGenerali)

            XmlDoc.AppendChild(XmlDatiRisultati)

            Select Case Modalita_Xml

                Case True
                    Return XmlDoc.OuterXml

                Case False
                    If InStr(1, XmlDoc.OuterXml, "err_code") > 0 Then

                        'Impianto non conforme al piano di concimazione
                        Return "1"

                    Else

                        'Impianto conforme al piano di concimazione
                        Return "0"

                    End If

            End Select

        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

            Return "-1"

        Finally

            If FlagConnessioneLocale Then
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

    End Function


    '============================================================================
    Private Sub DPI_Verifica_Apporto_MacroElementi(ByRef DatiInseriti(,) As String,
                                                   ByRef XmlDoc As XmlDocument,
                                                   ByRef XmlDatiNonConformi As XmlElement,
                                                   ByVal PIVA As String,
                                                   ByVal Sa_Cod As Int32,
                                                   ByVal Appezza As Int32,
                                                   ByVal Id_Reg As Int32,
                                                   ByVal Progetto_Cod As Int32,
                                                   ByVal Progetto_Nome As String,
                                                   ByVal Validita_Inizio As Date,
                                                   ByVal Validita_Fine As Date,
                                                   ByVal Veg_Cod As Int32, ByVal Grfi_Cod As Int32,
                                                   ByVal Data_Riferimento As Date,
                                                   ByVal Disciplinare_Cod As Integer,
                                                   ByVal Id_Agenda_Escluso As Int32,
                                                   ByVal N_Distribuito As Decimal,
                                                   ByVal P_Distribuito As Decimal,
                                                   ByVal K_Distribuito As Decimal,
                                                   ByVal Mg_Distribuito As Decimal,
                                                   ByVal Cu_Distribuito As Decimal,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                   Optional ByRef Hash_FormulatiPA As Hashtable = Nothing,
                                                   Optional ByRef Hash_FormulatiPAPesi As Hashtable = Nothing
                                                   )
        '============================================================================



        Dim NomeRoutine As String = "DpiBIZ.DPI_Verifica.DPI_Verifica_Apporto_MacroElementi()"
        Dim MessaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction
        Dim RisultatoFunzione As String = String.Empty

        Dim Err_Code As Int32
        Dim Err_Des As String

        Dim N_Massimo As Decimal = -1
        Dim P_Massimo As Decimal = -1
        Dim K_Massimo As Decimal = -1
        Dim Mg_Massimo As Decimal = -1
        Dim N_Massimo_Org As Decimal = -1


        Dim N_Operazione As Decimal = N_Distribuito
        Dim P_Operazione As Decimal = P_Distribuito
        Dim K_Operazione As Decimal = K_Distribuito
        Dim Mg_Operazione As Decimal = Mg_Distribuito
        Dim Cu_Operazione As Decimal = Cu_Distribuito


        'Dim N_Distribuito_Intervento As Decimal = N_Distribuito

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                xTransazione = Nothing
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
                xTransazione = objParametri.objTransazione
            End If

            '===========================================================================================
            'Lettura dei massimi apporti macroelementi
            '-------------------------------------------------------------------------------------------
            Dim objApporti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim Regolamento_Concimazioni_Cod As Integer = 0
            Dim N_Max_Intervento As Decimal = 0

            objApporti.Leggi_Macroelementi_Impostati(PIVA,
                                                Sa_Cod,
                                                Appezza,
                                                Id_Reg,
                                                Progetto_Cod,
                                                N_Massimo, N_Massimo_Org,
                                                P_Massimo,
                                                K_Massimo,
                                                Mg_Massimo,
                                                Regolamento_Concimazioni_Cod,
                                                objParametri)
            objApporti = Nothing

            If Disciplinare_Cod = 0 AndAlso Regolamento_Concimazioni_Cod <> 0 Then
                Disciplinare_Cod = Regolamento_Concimazioni_Cod
            End If

            If Disciplinare_Cod > 0 AndAlso Veg_Cod > 0 Then

                Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output
                Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input
                objParametriIngresso.Regolamento_Cod = Disciplinare_Cod
                objParametriIngresso.Veg_Cod = Veg_Cod
                objParametriIngresso.Grfi_Cod = 0
                If Grfi_Cod > 0 Then
                    objParametriIngresso.Grfi_Cod_Gias = Grfi_Cod
                End If
                objParametriIngresso.SoloValorizzati = True
                objParametriIngresso.SoloVisibili = False
                objParametriIngresso.Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.N_Max_Intervento

                Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                objParametriUscita = objPC_WS.FattoriCorrettivi_ConFinalitaGias_Leggi(objParametriIngresso)
                If objParametriUscita.ListaFattoriCorrettivi.Count > 0 Then
                    N_Max_Intervento = objParametriUscita.ListaFattoriCorrettivi.Item(0).Valore
                End If

            End If


            '===============================================================================
            'CONTROLLO MASSIMO APPORTO MACROELEMENTI
            '-------------------------------------------------------------------------------

            Dim objFert As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            Dim Dt As DataTable

            Dt = objFert.Leggi_Macroelementi_Distribuiti(N_Distribuito,
                                                        P_Distribuito,
                                                        K_Distribuito,
                                                        Mg_Distribuito,
                                                        Cu_Distribuito,
                                                        0,
                                                        0,
                                                        0,
                                                        0,
                                                        0,
                                                        PIVA,
                                                        Sa_Cod,
                                                        Appezza,
                                                        Id_Reg,
                                                        Progetto_Cod,
                                                        Validita_Inizio,
                                                        Validita_Fine,
                                                        Id_Agenda_Escluso,
                                                        objParametri)
            objFert = Nothing
            Dt = Nothing


            'Azoto Max x Intervento
            If N_Max_Intervento > 0 AndAlso N_Operazione > 0 Then
                If N_Max_Intervento < N_Operazione Then
                    Err_Code = enTipoErrCode_Verifica.Superato_N_Max_Intervento
                    Err_Des = "L'apporto di azoto sull'impianto lotto " & Progetto_Nome & " (" & Format(N_Operazione, "##,###,##0.000") & " kg/ha) supera il massimo consentito per singolo intervento (" & N_Max_Intervento & " kg/ha)"

                    Inserisci_NonConformita(DatiInseriti,
                                         XmlDoc,
                                         XmlDatiNonConformi,
                                         Err_Code,
                                         Err_Des,
                                         "Controllare il massimo apporto di azoto. ",
                                         0,
                                         0,
                                         0,
                                         "",
                                         "",
                                         CLng(Appezza),
                                         CLng(Id_Reg),
                                         "", "", "",
                                         N_Massimo,
                                         N_Distribuito,
                                         0, 0, 0, 0, 0, 0, 0, 0, 0, 0)

                End If
            End If

            'Controllo Residui Piano di Concimazione
            If N_Massimo <> -1 AndAlso N_Operazione > 0 Then
                If N_Massimo < N_Distribuito Then
                    Err_Code = enTipoErrCode_Verifica.Superato_N_Max
                    Err_Des = "L'apporto di azoto sull'impianto lotto " & Progetto_Nome & " (" & Format(N_Distribuito, "##,###,##0.000") & " kg/ha) supera il massimo consentito (" & N_Massimo & " kg/ha)"

                    Inserisci_NonConformita(DatiInseriti,
                                         XmlDoc,
                                         XmlDatiNonConformi,
                                         Err_Code,
                                         Err_Des,
                                         "Controllare il massimo apporto di azoto. ",
                                         0,
                                         0,
                                         0,
                                         "",
                                         "",
                                         CLng(Appezza),
                                         CLng(Id_Reg),
                                         "", "", "",
                                         N_Massimo,
                                         N_Distribuito,
                                         0, 0, 0, 0, 0, 0, 0, 0, 0, 0)

                End If
            End If

            If P_Massimo <> -1 AndAlso P_Operazione > 0 Then
                If P_Massimo < P_Distribuito Then
                    Err_Code = enTipoErrCode_Verifica.Superato_P_Max
                    Err_Des = "L'apporto di anidride fosforosa sull'impianto lotto " & Progetto_Nome & " (" & Format(P_Distribuito, "##,###,##0.000") & " kg/ha) supera il massimo consentito (" & P_Massimo & " kg/ha)"

                    Inserisci_NonConformita(DatiInseriti,
                                         XmlDoc,
                                         XmlDatiNonConformi,
                                         Err_Code,
                                         Err_Des,
                                         "Controllare il massimo apporto di anidride fosforosa. ",
                                         0,
                                         0,
                                         0,
                                         0,
                                         0,
                                         CLng(Appezza),
                                         CLng(Id_Reg),
                                         "", "", "", 0, 0,
                                         P_Massimo,
                                         P_Distribuito,
                                         0, 0, 0, 0, 0, 0, 0, 0)

                End If
            End If

            If K_Massimo <> -1 AndAlso K_Operazione > 0 Then
                If K_Massimo < K_Distribuito Then
                    Err_Code = enTipoErrCode_Verifica.Superato_K_Max
                    Err_Des = "L'apporto di Potassio sull'impianto lotto " & Progetto_Nome & " (" & Format(K_Distribuito, "##,###,##0.000") & " kg/ha) supera il massimo consentito (" & K_Massimo & " kg/ha)"

                    Inserisci_NonConformita(DatiInseriti,
                                         XmlDoc,
                                         XmlDatiNonConformi,
                                         Err_Code,
                                         Err_Des,
                                         "Controllare il massimo apporto di potassio. ",
                                         0,
                                         0,
                                         0,
                                         "", "",
                                         CLng(Appezza),
                                         CLng(Id_Reg),
                                         "", "", "", 0, 0, 0, 0,
                                         K_Massimo,
                                         K_Distribuito,
                                         0, 0, 0, 0, 0, 0)

                End If
            End If

            If Mg_Massimo <> -1 AndAlso Mg_Operazione > 0 Then
                If Mg_Massimo < Mg_Distribuito Then
                    Err_Code = enTipoErrCode_Verifica.Superato_M_Max
                    Err_Des = "L'apporto di ossido di magnesio sull'impianto lotto " & Progetto_Nome & " (" & Format(Mg_Distribuito, "##,###,##0.000") & " kg/ha) supera il massimo consentito (" & Mg_Massimo & " kg/ha)"

                    Inserisci_NonConformita(DatiInseriti,
                                         XmlDoc,
                                         XmlDatiNonConformi,
                                         Err_Code,
                                         Err_Des,
                                         "Controllare il massimo apporto di ossido di magnesio. ",
                                         0,
                                         0,
                                         0,
                                         "", "",
                                         CLng(Appezza),
                                         CLng(Id_Reg),
                                         "", "", "", 0, 0, 0, 0, 0, 0,
                                         Mg_Massimo,
                                         Mg_Distribuito,
                                         0, 0, 0, 0)
                End If
            End If

            '(20/06/2017 fede) aggiunto controllo rame nella concimazione in caso di dpi e bio
            'controllo anche trattamenti rameici
            If (Disciplinare_Cod > 0 OrElse Disciplinare_Cod = -2) AndAlso Cu_Operazione > 0 Then

                'verifico se è stato distribuito del rame nei trattamenti
                Dim dtLavorazioni As DataTable
                Dim ObjContab As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R
                dtLavorazioni = ObjContab.LeggiLavorazioni_Da_Principi_Attivi(PIVA,
                                                                               Sa_Cod,
                                                                                PaRameici_str,
                                                                                Appezza,
                                                                                Id_Reg,
                                                                                Validita_Inizio,
                                                                                Validita_Fine,
                                                                                "",
                                                                                "",
                                                                                objParametri, objParametri_Utenti,
                                                                               Hash_FormulatiPA, Hash_FormulatiPAPesi)

                Dim Qta_Pa_Tot As Decimal = 0
                Dim Qta_Pa_Ha As Decimal = 0
                Dim Qta_Prodotto_Ha As Decimal = 0
                Dim Sup As Decimal
                Dim TitoloP As Decimal
                Dim PesoP As Decimal

                For i = 0 To dtLavorazioni.Rows.Count - 1
                    TitoloP = 0
                    PesoP = 0
                    If Not IsDBNull(dtLavorazioni.Rows(i).Item("titolo")) AndAlso CDec(dtLavorazioni.Rows(i).Item("titolo")) > 0 Then
                        TitoloP = CDec(dtLavorazioni.Rows(i).Item("titolo"))
                    End If
                    If Not IsDBNull(dtLavorazioni.Rows(i).Item("peso")) AndAlso CDec(dtLavorazioni.Rows(i).Item("peso")) > 0 Then
                        PesoP = CDec(dtLavorazioni.Rows(i).Item("peso"))
                    End If
                    If Not IsDBNull(dtLavorazioni.Rows(i).Item("sup_trattata")) AndAlso CDbl(dtLavorazioni.Rows(i).Item("sup_trattata")) > 0 Then
                        Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_trattata"))
                    Else
                        Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_imp"))
                    End If
                    If Sup <> 0 Then
                        Qta_Prodotto_Ha = CDbl(dtLavorazioni.Rows(i).Item("qta")) / Sup
                        If PesoP = 0 Then
                            Qta_Pa_Ha = Qta_Prodotto_Ha * TitoloP / 100
                        Else
                            Qta_Pa_Ha = Qta_Prodotto_Ha * PesoP / 1000
                        End If
                        Qta_Pa_Tot += Qta_Pa_Ha
                    End If
                Next

                Qta_Pa_Tot += Cu_Distribuito

                Dim Cu_Massimo = 6
                If Data_Riferimento >= #1/1/2019# Then
                    Cu_Massimo = 4
                Else
                    Cu_Massimo = 6
                End If

                If Qta_Pa_Tot > Cu_Massimo Then
                    Err_Code = enTipoErrCode_Verifica.DoseEccessivaRame
                    Err_Des = "La dose di rame distribuita (" & Qta_Pa_Tot & " Kg/ha) supera la massima consentita ( " & Cu_Massimo & " Kg/Ha all'anno)"

                    Inserisci_NonConformita(DatiInseriti,
                                     XmlDoc,
                                     XmlDatiNonConformi,
                                     Err_Code,
                                     Err_Des,
                                     "E' consentito utilizzare non più di " & Cu_Massimo & " Kg/ha di rame all'anno. ",
                                     0,
                                     0,
                                     0,
                                     "", "",
                                     CLng(Appezza),
                                     CLng(Id_Reg),
                                     "", "", "", 0, 0, 0, 0, 0, 0, 0, 0,
                                     6,
                                     Qta_Pa_Tot,
                                     0, 0)
                End If

            End If

            '------------------------------

        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

        Finally

            If FlagConnessioneLocale Then
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

    End Sub

    Private Sub DPI_Verifica_Apporto_MacroElementi_old(ByRef DatiInseriti(,) As String,
                                                   ByRef XmlDoc As XmlDocument,
                                                   ByRef XmlDatiNonConformi As XmlElement,
                                                   ByVal PIVA As String,
                                                   ByVal Sa_Cod As Int32,
                                                   ByVal Appezza As Int32,
                                                   ByVal Id_Reg As Int32,
                                                   ByVal Progetto_Cod As Int32,
                                                   ByVal Progetto_Nome As String,
                                                   ByVal Validita_Inizio As Date,
                                                   ByVal Validita_Fine As Date,
                                                   ByVal Veg_Cod As Int32, ByVal Grfi_Cod As Int32,
                                                   ByVal Data_Riferimento As Date,
                                                   ByVal Disciplinare_Cod As Integer,
                                                   ByVal Regolamento_Concimazioni_Cod As Integer,
                                                   ByVal strN_Massimo As String,
                                                   ByVal strP_Massimo As String,
                                                   ByVal strK_Massimo As String,
                                                   ByVal strMg_Massimo As String,
                                                   ByVal Id_Agenda_Escluso As Int32,
                                                   ByVal N_Distribuito As Decimal,
                                                   ByVal P_Distribuito As Decimal,
                                                   ByVal K_Distribuito As Decimal,
                                                   ByVal Mg_Distribuito As Decimal,
                                                   ByVal Cu_Distribuito As Decimal,
                                                    ByVal ListaN_Max_Intervento As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output,
                                                     ByRef objParametri As AgronicaCoreParametri,
                                                     ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                         Optional ByRef Hash_FormulatiPA As Hashtable = Nothing,
                                                            Optional ByRef Hash_FormulatiPAPesi As Hashtable = Nothing
                                                   )
        '============================================================================



        Dim NomeRoutine As String = "DpiBIZ.DPI_Verifica.DPI_Verifica_Apporto_MacroElementi()"
        Dim MessaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction
        Dim RisultatoFunzione As String = String.Empty

        Dim Err_Code As Int32
        Dim Err_Des As String

        'necessario per gestire i volutamente NON IMPOSTATI (caso diverso da = 0)
        Dim N_Massimo As Decimal = -1
        Dim P_Massimo As Decimal = -1
        Dim K_Massimo As Decimal = -1
        Dim Mg_Massimo As Decimal = -1

        If IsNumeric(strN_Massimo) Then
            N_Massimo = CDec(strN_Massimo)
        End If
        If IsNumeric(strP_Massimo) Then
            P_Massimo = CDec(strP_Massimo)
        End If
        If IsNumeric(strK_Massimo) Then
            K_Massimo = CDec(strK_Massimo)
        End If
        If IsNumeric(strMg_Massimo) Then
            Mg_Massimo = CDec(strMg_Massimo)
        End If


        Dim N_Operazione As Decimal = N_Distribuito
        Dim P_Operazione As Decimal = P_Distribuito
        Dim K_Operazione As Decimal = K_Distribuito
        Dim Mg_Operazione As Decimal = Mg_Distribuito
        Dim Cu_Operazione As Decimal = Cu_Distribuito


        'Dim N_Distribuito_Intervento As Decimal = N_Distribuito

        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                xTransazione = Nothing
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
                xTransazione = objParametri.objTransazione
            End If

            ''===========================================================================================
            ''Lettura dei massimi apporti macroelementi
            ''-------------------------------------------------------------------------------------------

            Dim N_Max_Intervento As Decimal = 0

            If Disciplinare_Cod = 0 AndAlso Regolamento_Concimazioni_Cod <> 0 Then
                Disciplinare_Cod = Regolamento_Concimazioni_Cod
            End If

            If Disciplinare_Cod > 0 AndAlso Veg_Cod > 0 AndAlso Grfi_Cod > 0 Then

                Dim fattore As AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo = (From ff In ListaN_Max_Intervento.ListaFattoriCorrettivi
                                                                                     Where ff.Regolamento_Cod = Disciplinare_Cod AndAlso
                                                                                           ff.Veg_Cod = Veg_Cod AndAlso
                                                                                           ff.Grfi_Cod_Gias = Grfi_Cod).FirstOrDefault()
                If fattore IsNot Nothing Then
                    N_Max_Intervento = fattore.Valore
                End If

            End If


            '===============================================================================
            'CONTROLLO MASSIMO APPORTO MACROELEMENTI
            '-------------------------------------------------------------------------------

            Dim objFert As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            Dim Dt As DataTable

            Dt = objFert.Leggi_Macroelementi_Distribuiti(N_Distribuito,
                                                        P_Distribuito,
                                                        K_Distribuito,
                                                        Mg_Distribuito,
                                                        Cu_Distribuito,
                                                        0,
                                                        0,
                                                        0,
                                                        0,
                                                        0,
                                                        PIVA,
                                                        Sa_Cod,
                                                        Appezza,
                                                        Id_Reg,
                                                        Progetto_Cod,
                                                        Validita_Inizio,
                                                        Validita_Fine,
                                                        Id_Agenda_Escluso,
                                                        objParametri)
            objFert = Nothing
            Dt = Nothing


            'Azoto Max x Intervento
            If N_Max_Intervento > 0 Then
                If N_Max_Intervento < N_Operazione Then
                    Err_Code = enTipoErrCode_Verifica.Superato_N_Max_Intervento
                    Err_Des = "L'apporto di azoto sull'impianto lotto " & Progetto_Nome & " (" & Format(N_Operazione, "##,###,##0.000") & " kg/ha) supera il massimo consentito per singolo intervento (" & N_Max_Intervento & " kg/ha)"

                    Inserisci_NonConformita(DatiInseriti,
                                         XmlDoc,
                                         XmlDatiNonConformi,
                                         Err_Code,
                                         Err_Des,
                                         "Controllare il massimo apporto di azoto. ",
                                         0,
                                         0,
                                         0,
                                         "",
                                         "",
                                         CLng(Appezza),
                                         CLng(Id_Reg),
                                         "", "", "",
                                         N_Massimo,
                                         N_Distribuito,
                                         0, 0, 0, 0, 0, 0, 0, 0, 0, 0)

                End If
            End If

            'Controllo Residui Piano di Concimazione
            If N_Massimo <> -1 AndAlso N_Operazione > 0 Then
                If N_Massimo < N_Distribuito Then
                    Err_Code = enTipoErrCode_Verifica.Superato_N_Max
                    Err_Des = "L'apporto di azoto sull'impianto lotto " & Progetto_Nome & " (" & Format(N_Distribuito, "##,###,##0.000") & " kg/ha) supera il massimo consentito (" & N_Massimo & " kg/ha)"

                    Inserisci_NonConformita(DatiInseriti,
                                         XmlDoc,
                                         XmlDatiNonConformi,
                                         Err_Code,
                                         Err_Des,
                                         "Controllare il massimo apporto di azoto. ",
                                         0,
                                         0,
                                         0,
                                         "",
                                         "",
                                         CLng(Appezza),
                                         CLng(Id_Reg),
                                         "", "", "",
                                         N_Massimo,
                                         N_Distribuito,
                                         0, 0, 0, 0, 0, 0, 0, 0, 0, 0)

                End If
            End If

            If P_Massimo <> -1 AndAlso P_Operazione > 0 Then
                If P_Massimo < P_Distribuito Then
                    Err_Code = enTipoErrCode_Verifica.Superato_P_Max
                    Err_Des = "L'apporto di anidride fosforosa sull'impianto lotto " & Progetto_Nome & " (" & Format(P_Distribuito, "##,###,##0.000") & " kg/ha) supera il massimo consentito (" & P_Massimo & " kg/ha)"

                    Inserisci_NonConformita(DatiInseriti,
                                         XmlDoc,
                                         XmlDatiNonConformi,
                                         Err_Code,
                                         Err_Des,
                                         "Controllare il massimo apporto di anidride fosforosa. ",
                                         0,
                                         0,
                                         0,
                                         0,
                                         0,
                                         CLng(Appezza),
                                         CLng(Id_Reg),
                                         "", "", "", 0, 0,
                                         P_Massimo,
                                         P_Distribuito,
                                         0, 0, 0, 0, 0, 0, 0, 0)

                End If
            End If

            If K_Massimo <> -1 AndAlso K_Operazione > 0 Then
                If K_Massimo < K_Distribuito Then
                    Err_Code = enTipoErrCode_Verifica.Superato_K_Max
                    Err_Des = "L'apporto di Potassio sull'impianto lotto " & Progetto_Nome & " (" & Format(K_Distribuito, "##,###,##0.000") & " kg/ha) supera il massimo consentito (" & K_Massimo & " kg/ha)"

                    Inserisci_NonConformita(DatiInseriti,
                                         XmlDoc,
                                         XmlDatiNonConformi,
                                         Err_Code,
                                         Err_Des,
                                         "Controllare il massimo apporto di potassio. ",
                                         0,
                                         0,
                                         0,
                                         "", "",
                                         CLng(Appezza),
                                         CLng(Id_Reg),
                                         "", "", "", 0, 0, 0, 0,
                                         K_Massimo,
                                         K_Distribuito,
                                         0, 0, 0, 0, 0, 0)

                End If
            End If

            If Mg_Massimo <> -1 AndAlso Mg_Operazione > 0 Then
                If Mg_Massimo < Mg_Distribuito Then
                    Err_Code = enTipoErrCode_Verifica.Superato_M_Max
                    Err_Des = "L'apporto di ossido di magnesio sull'impianto lotto " & Progetto_Nome & " (" & Format(Mg_Distribuito, "##,###,##0.000") & " kg/ha) supera il massimo consentito (" & Mg_Massimo & " kg/ha)"

                    Inserisci_NonConformita(DatiInseriti,
                                         XmlDoc,
                                         XmlDatiNonConformi,
                                         Err_Code,
                                         Err_Des,
                                         "Controllare il massimo apporto di ossido di magnesio. ",
                                         0,
                                         0,
                                         0,
                                         "", "",
                                         CLng(Appezza),
                                         CLng(Id_Reg),
                                         "", "", "", 0, 0, 0, 0, 0, 0,
                                         Mg_Massimo,
                                         Mg_Distribuito,
                                         0, 0, 0, 0)
                End If
            End If

            '(20/06/2017 fede) aggiunto controllo rame nella concimazione in caso di dpi e bio
            'controllo anche trattamenti rameici
            If (Disciplinare_Cod > 0 OrElse Disciplinare_Cod = -2) AndAlso Cu_Operazione > 0 Then

                'verifico se è stato distribuito del rame nei trattamenti
                Dim dtLavorazioni As DataTable
                Dim ObjContab As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R
                dtLavorazioni = ObjContab.LeggiLavorazioni_Da_Principi_Attivi(PIVA,
                                                                               Sa_Cod,
                                                                                PaRameici_str,
                                                                                Appezza,
                                                                                Id_Reg,
                                                                                Validita_Inizio,
                                                                                Validita_Fine,
                                                                                "",
                                                                                "",
                                                                                objParametri, objParametri_Utenti,
                                                                               Hash_FormulatiPA, Hash_FormulatiPAPesi)

                Dim Qta_Pa_Tot As Decimal = 0
                Dim Qta_Pa_Ha As Decimal = 0
                Dim Qta_Prodotto_Ha As Decimal = 0
                Dim Sup As Decimal
                Dim TitoloP As Decimal
                Dim PesoP As Decimal

                For i = 0 To dtLavorazioni.Rows.Count - 1
                    TitoloP = 0
                    PesoP = 0
                    If Not IsDBNull(dtLavorazioni.Rows(i).Item("titolo")) AndAlso CDec(dtLavorazioni.Rows(i).Item("titolo")) > 0 Then
                        TitoloP = CDec(dtLavorazioni.Rows(i).Item("titolo"))
                    End If
                    If Not IsDBNull(dtLavorazioni.Rows(i).Item("peso")) AndAlso CDec(dtLavorazioni.Rows(i).Item("peso")) > 0 Then
                        PesoP = CDec(dtLavorazioni.Rows(i).Item("peso"))
                    End If
                    If Not IsDBNull(dtLavorazioni.Rows(i).Item("sup_trattata")) AndAlso CDbl(dtLavorazioni.Rows(i).Item("sup_trattata")) > 0 Then
                        Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_trattata"))
                    Else
                        Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_imp"))
                    End If
                    If Sup <> 0 Then
                        Qta_Prodotto_Ha = CDbl(dtLavorazioni.Rows(i).Item("qta")) / Sup
                        If PesoP = 0 Then
                            Qta_Pa_Ha = Qta_Prodotto_Ha * TitoloP / 100
                        Else
                            Qta_Pa_Ha = Qta_Prodotto_Ha * PesoP / 1000
                        End If
                        Qta_Pa_Tot += Qta_Pa_Ha
                    End If
                Next

                Qta_Pa_Tot += Cu_Distribuito

                Dim Cu_Massimo = 6
                If Data_Riferimento >= #1/1/2019# Then
                    Cu_Massimo = 4
                Else
                    Cu_Massimo = 6
                End If

                If Qta_Pa_Tot > 6 Then
                    Err_Code = enTipoErrCode_Verifica.DoseEccessivaRame
                    Err_Des = "La dose di rame distribuita (" & Qta_Pa_Tot & " Kg/ha) supera la massima consentita ( " & Cu_Massimo & " Kg/Ha all'anno)"
                    Inserisci_NonConformita(DatiInseriti,
                                     XmlDoc,
                                     XmlDatiNonConformi,
                                     Err_Code,
                                     Err_Des,
                                     "E' consentito utilizzare non più di " & Cu_Massimo & " Kg/ha di rame all'anno. ",
                                     0,
                                     0,
                                     0,
                                     "", "",
                                     CLng(Appezza),
                                     CLng(Id_Reg),
                                     "", "", "", 0, 0, 0, 0, 0, 0, 0, 0,
                                     6,
                                     Qta_Pa_Tot,
                                     0, 0)
                End If

            End If

            '------------------------------

        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

        Finally

            If FlagConnessioneLocale Then
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

    End Sub

    Private Sub DPI_Verifica_Apporto_MacroElementi(ByRef DatiInseriti(,) As String,
                                                   ByRef XmlDoc As XmlDocument,
                                                   ByRef XmlDatiNonConformi As XmlElement,
                                                   ByVal PIVA As String,
                                                   ByVal Id_Agenda As Int32, ByVal IDTestataTemp As Integer, ByVal Destinazioni(,) As String,
                                                   ByVal Veg_Cod As Int32,
                                                   ByVal Data_Riferimento As Date,
                                                   ByVal Disciplinare_Cod As Integer,
                                                   ByVal IDTestataTemp__tmp_FormulatiXPrincipiAttivi As Integer,
                                                   ByVal ListaN_Max_Intervento As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                    Optional isVerificaPosteriori As Boolean = True,
                                                    Optional ByRef list_strAppCoinvolti As List(Of String) = Nothing,
                                                    Optional ByRef Sup_Coinvolta As Decimal = 0,
                                                    Optional paramVerificaDPIMultiAttivita As AgronicaCoreModello.VerificaDPIMultiAttivita = Nothing,
                                                    Optional Raccoglitore_Cod As Integer = 0
                                                   )
        '============================================================================



        Dim NomeRoutine As String = "DpiBIZ.DPI_Verifica.DPI_Verifica_Apporto_MacroElementi()"
        Dim MessaggioErrore As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim xConnessione As DbConnection = Nothing
        Dim xConnectionState As ConnectionState = ConnectionState.Closed
        Dim xTransazione As DbTransaction
        Dim RisultatoFunzione As String = String.Empty

        Dim Err_Code As Int32
        Dim Err_Des As String

        'necessario per gestire i volutamente NON IMPOSTATI (caso diverso da = 0)
        Dim N_Massimo As Decimal = -1
        Dim P_Massimo As Decimal = -1
        Dim K_Massimo As Decimal = -1
        Dim Mg_Massimo As Decimal = -1

        Dim N_Operazione As Decimal = 0 'N_Distribuito
        Dim P_Operazione As Decimal = 0 'P_Distribuito
        Dim K_Operazione As Decimal = 0 'K_Distribuito
        Dim Mg_Operazione As Decimal = 0 'Mg_Distribuito
        Dim Cu_Operazione As Decimal = 0 'Cu_Distribuito

        Dim N_Distribuito As Decimal
        Dim P_Distribuito As Decimal
        Dim K_Distribuito As Decimal
        Dim Mg_Distribuito As Decimal
        Dim Cu_Distribuito As Decimal
        Dim Cu_Distribuito_Trattamenti As Decimal

        'Dim N_Distribuito_Intervento As Decimal = N_Distribuito

        Dim app_nome As String = ""
        Dim grfi_cod As Integer = 0
        Dim Regolamento_Concimazioni_Cod As Integer = 0

        Dim N_Max_Intervento As Decimal

        Dim Codice_N_Distribuito As Integer = 11
        Dim Codice_P_Distribuito As Integer = 12
        Dim Codice_K_Distribuito As Integer = 13
        Dim Codice_Mg_Distribuito As Integer = 14
        Dim Codice_Cu_Distribuito As Integer = 18

        Dim Codice_ProgettoCod As Integer = 15
        Dim Codice_ValiditaInizio As Integer = 16
        Dim Codice_ValiditaFine As Integer = 17



        Try

            '------------------------------
            'Verifico se e' stata impostata una connessione
            If IsNothing(objParametri.objConnessione) Then
                'Flag
                FlagConnessioneLocale = True
                'Creo la connessione localmente
                xConnessione = DataProviderFactory.Instance.CreaNuovaConnessione(objParametri.StringaConnessione)
                xTransazione = Nothing
            Else
                'Utilizzo quella passata come parametro
                xConnessione = objParametri.objConnessione
                xConnectionState = objParametri.objConnessione.State
                xTransazione = objParametri.objTransazione
            End If

            Dim objFert As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

            Dim DtDistinte As DataTable
            'se sono in modifica devo escludere l'operazione corrente
            Dim Str_FiltroAggiuntivo As New StringBuilder
            If IDTestataTemp > 0 AndAlso Id_Agenda > 0 Then
                Str_FiltroAggiuntivo.AppendLine(" Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda) & " ")

                If Raccoglitore_Cod <> 0 Then
                    Str_FiltroAggiuntivo.AppendLine(" AND ISNULL(Agenda.Raccoglitore_Cod, 0) <> " & Agro_SQL_SaveNum(Raccoglitore_Cod) & " ")
                End If
            End If
            'If(isVerificaPosteriori, Id_Agenda, 0) --> Perché?
            'Se arriviamo dalla verifica conformità a posteriori ci serve l'Id_Agenda per leggere le distinte ed i loro valori di N-P-K-etc...
            'Quando  arriviamo dalla verifica conformità all'interno dell'operazione
            'ci serviamo delle righe 7435-7437 per leggere il PREGRESSO (in IDTestataTemp) escludendo l'agenda corrente
            DtDistinte = objFert.Leggi_Macroelementi_Distribuiti_suDistinte(PIVA, If(isVerificaPosteriori, Id_Agenda, 0),
                                                                            IDTestataTemp, IDTestataTemp__tmp_FormulatiXPrincipiAttivi,
                                                                            Str_FiltroAggiuntivo.ToString,
                                                                            objParametri, isVerificaPosteriori:=isVerificaPosteriori)


            For Each distinta As DataRow In DtDistinte.Rows

                app_nome = distinta.Item("app_nome")
                If isVerificaPosteriori Then
                    If IsNothing(list_strAppCoinvolti) Then
                        list_strAppCoinvolti = New List(Of String)
                    End If
                    list_strAppCoinvolti.Add(app_nome)

                    Sup_Coinvolta += distinta.Item("Sup_Coinvolta")
                End If

                Regolamento_Concimazioni_Cod = distinta.Item("Regolamento_Concimazioni_Cod")
                grfi_cod = distinta.Item("grfi_cod")

                N_Massimo = -1
                P_Massimo = -1
                K_Massimo = -1
                Mg_Massimo = -1

                N_Distribuito = 0
                P_Distribuito = 0
                K_Distribuito = 0
                Mg_Distribuito = 0
                Cu_Distribuito = 0
                Cu_Distribuito_Trattamenti = 0

                If IsNumeric(distinta.Item("N_Massimo")) Then
                    N_Massimo = CDec(distinta.Item("N_Massimo"))
                End If
                If IsNumeric(distinta.Item("P_Massimo")) Then
                    P_Massimo = CDec(distinta.Item("P_Massimo"))
                End If
                If IsNumeric(distinta.Item("K_Massimo")) Then
                    K_Massimo = CDec(distinta.Item("K_Massimo"))
                End If
                If IsNumeric(distinta.Item("Mg_Massimo")) Then
                    Mg_Massimo = CDec(distinta.Item("Mg_Massimo"))
                End If

                If IsNumeric(distinta.Item("N_Distribuito_Ha")) Then
                    N_Distribuito = distinta.Item("N_Distribuito_Ha")
                End If
                If IsNumeric(distinta.Item("P_Distribuito_Ha")) Then
                    P_Distribuito = distinta.Item("P_Distribuito_Ha")
                End If
                If IsNumeric(distinta.Item("K_Distribuito_Ha")) Then
                    K_Distribuito = distinta.Item("K_Distribuito_Ha")
                End If
                If IsNumeric(distinta.Item("Mg_Distribuito_Ha")) Then
                    Mg_Distribuito = distinta.Item("Mg_Distribuito_Ha")
                End If
                If IsNumeric(distinta.Item("Cu_Distribuito_Ha")) Then
                    Cu_Distribuito = distinta.Item("Cu_Distribuito_Ha")
                End If
                If Not IsDBNull(distinta.Item("Cu_Distribuito_Trattamenti_Ha")) Then
                    Cu_Distribuito_Trattamenti = distinta.Item("Cu_Distribuito_Trattamenti_Ha")
                End If

                'se sono in inserimento/modifica devo aggiungere gli apporti dell'operazione corrente
                'se sono a posteriori li ho già letti nella query
                If IDTestataTemp > 0 AndAlso Destinazioni IsNot Nothing Then

                    For i = 0 To UBound(Destinazioni, 2) - 1
                        If Destinazioni(i_Piva, i) = distinta.Item("piva") AndAlso
                            Destinazioni(i_Sa_Cod, i) = distinta.Item("sa_cod") AndAlso
                            Destinazioni(i_Appezza, i) = distinta.Item("appezza") AndAlso
                            Destinazioni(i_Id_Reg, i) = distinta.Item("id_reg") Then

                            N_Operazione = Destinazioni(Codice_N_Distribuito, i)
                            P_Operazione = Destinazioni(Codice_P_Distribuito, i)
                            K_Operazione = Destinazioni(Codice_K_Distribuito, i)
                            Mg_Operazione = Destinazioni(Codice_Mg_Distribuito, i)
                            Cu_Operazione = Destinazioni(Codice_Cu_Distribuito, i)

                        End If
                    Next

                Else

                    N_Operazione = distinta.Item("N_Operazione_Ha")
                    P_Operazione = distinta.Item("P_Operazione_Ha")
                    K_Operazione = distinta.Item("K_Operazione_Ha")
                    Mg_Operazione = distinta.Item("Mg_Operazione_Ha")
                    Cu_Operazione = distinta.Item("Cu_Operazione_Ha")

                End If


                '----------------
                '   AZOTO                
                '----------------
                'Azoto Max x Intervento
                N_Max_Intervento = 0

                If Disciplinare_Cod = 0 AndAlso Regolamento_Concimazioni_Cod <> 0 Then
                    Disciplinare_Cod = Regolamento_Concimazioni_Cod
                End If

                If Disciplinare_Cod > 0 AndAlso Veg_Cod > 0 AndAlso grfi_cod > 0 AndAlso ListaN_Max_Intervento IsNot Nothing Then

                    Dim fattore As AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo = (From ff In ListaN_Max_Intervento.ListaFattoriCorrettivi
                                                                                         Where ff.Regolamento_Cod = Disciplinare_Cod AndAlso
                                                                                               ff.Veg_Cod = Veg_Cod AndAlso
                                                                                               ff.Grfi_Cod_Gias = grfi_cod).FirstOrDefault()
                    If fattore IsNot Nothing Then
                        N_Max_Intervento = fattore.Valore
                    End If

                End If

                If N_Max_Intervento > 0 Then
                    If (N_Operazione - N_Max_Intervento) > 0.001 Then
                        'If N_Max_Intervento < N_Operazione Then
                        Err_Code = enTipoErrCode_Verifica.Superato_N_Max_Intervento
                        Err_Des = String.Format(Gias.ApportoNSuAppSuperaMaxConsentitoSingoloIntervento, app_nome, Format(N_Operazione, "##,###,##0.000"), N_Max_Intervento)

                        Inserisci_NonConformita(DatiInseriti,
                                                XmlDoc, XmlDatiNonConformi,
                                                Err_Code, Err_Des,
                                                Gias.ControllareMassimoApportoN,
                                                0, 0, 0,
                                                "", "",
                                                distinta.Item("appezza"),
                                                distinta.Item("id_reg"),
                                                "", "", "",
                                                N_Massimo,
                                                N_Distribuito,
                                                0, 0,
                                                0, 0,
                                                0, 0,
                                                0, 0,
                                                0, 0)

                    End If
                End If

                'Controllo Residui Piano di Concimazione
                If N_Massimo <> -1 AndAlso N_Operazione > 0 Then

                    'SE SIAMO IN MODIFICA/INSERIMENTO DI UNA CONCIMAZIONE, DEVO SOMMARE L'N_OPERAZIONE
                    If Not isVerificaPosteriori Then
                        'AGGIUNGO L'N DELL'OPERAZIONE
                        N_Distribuito += N_Operazione
                    End If

                    If (N_Distribuito - N_Massimo) > 0.001 Then
                        'If N_Massimo < N_Distribuito Then
                        Err_Code = enTipoErrCode_Verifica.Superato_N_Max
                        Err_Des = String.Format(Gias.ApportoNSuAppSuperaMaxConsentitoAnno, app_nome, Format(N_Distribuito, "##,###,##0.000"), N_Massimo)

                        Inserisci_NonConformita(DatiInseriti,
                                                XmlDoc, XmlDatiNonConformi,
                                                Err_Code, Err_Des,
                                                Gias.ControllareMassimoApportoN,
                                                0, 0, 0,
                                                "", "",
                                                distinta.Item("appezza"),
                                                distinta.Item("id_reg"),
                                                "", "", "",
                                                N_Massimo, N_Distribuito,
                                                0, 0,
                                                0, 0,
                                                0, 0,
                                                0, 0,
                                                0, 0)

                    End If
                End If


                '----------------
                '   FOSFORO
                '----------------
                If P_Massimo <> -1 AndAlso P_Operazione > 0 Then

                    'SE SIAMO IN MODIFICA/INSERIMENTO DI UNA CONCIMAZIONE, DEVO SOMMARE IL P_OPERAZIONE
                    If Not isVerificaPosteriori Then
                        'AGGIUNGO IL P DELL'OPERAZIONE
                        P_Distribuito += P_Operazione
                    End If

                    'If P_Massimo < P_Distribuito Then
                    If (P_Distribuito - P_Massimo) > 0.001 Then
                        Err_Code = enTipoErrCode_Verifica.Superato_P_Max
                        Err_Des = String.Format(Gias.ApportoPSuAppSuperaMaxConsentitoAnno, app_nome, Format(P_Distribuito, "##,###,##0.000"), P_Massimo)

                        Inserisci_NonConformita(DatiInseriti,
                                                XmlDoc, XmlDatiNonConformi,
                                                Err_Code, Err_Des,
                                                Gias.ControllareMassimoApportoP,
                                                0, 0, 0,
                                                0, 0,
                                                distinta.Item("appezza"),
                                                distinta.Item("id_reg"),
                                                "", "", "",
                                                0, 0,
                                                P_Massimo, P_Distribuito,
                                                0, 0,
                                                0, 0,
                                                0, 0,
                                                0, 0)

                    End If
                End If


                '----------------
                '   POTASSIO
                '----------------
                If K_Massimo <> -1 AndAlso K_Operazione > 0 Then

                    'SE SIAMO IN MODIFICA/INSERIMENTO DI UNA CONCIMAZIONE, DEVO SOMMARE IL K_OPERAZIONE
                    If Not isVerificaPosteriori Then
                        'AGGIUNGO IL K DELL'OPERAZIONE
                        K_Distribuito += K_Operazione
                    End If

                    'If K_Massimo < K_Distribuito Then
                    If (K_Distribuito - K_Massimo) > 0.001 Then
                        Err_Code = enTipoErrCode_Verifica.Superato_K_Max
                        Err_Des = String.Format(Gias.ApportoKSuAppSuperaMaxConsentitoAnno, app_nome, Format(K_Distribuito, "##,###,##0.000"), K_Massimo)

                        Inserisci_NonConformita(DatiInseriti,
                                                XmlDoc, XmlDatiNonConformi,
                                                Err_Code, Err_Des,
                                                Gias.ControllareMassimoApportoK,
                                                0, 0, 0,
                                                "", "",
                                                distinta.Item("appezza"),
                                                distinta.Item("id_reg"),
                                                "", "", "",
                                                0, 0,
                                                0, 0,
                                                K_Massimo, K_Distribuito,
                                                0, 0,
                                                0, 0,
                                                0, 0)

                    End If
                End If


                '----------------
                '  MAGNESIO
                '----------------
                If Mg_Massimo <> -1 AndAlso Mg_Operazione > 0 Then

                    'SE SIAMO IN MODIFICA/INSERIMENTO DI UNA CONCIMAZIONE, DEVO SOMMARE l'MG_OPERAZIONE
                    If Not isVerificaPosteriori Then
                        'AGGIUNGO l'MG DELL'OPERAZIONE
                        Mg_Distribuito += Mg_Operazione
                    End If

                    'If Mg_Massimo < Mg_Distribuito Then
                    If (Mg_Distribuito - Mg_Massimo) > 0.001 Then
                        Err_Code = enTipoErrCode_Verifica.Superato_M_Max
                        Err_Des = String.Format(Gias.ApportoMgSuAppSuperaMaxConsentitoAnno, app_nome, Format(Mg_Distribuito, "##,###,##0.000"), Mg_Massimo)

                        Inserisci_NonConformita(DatiInseriti,
                                                XmlDoc, XmlDatiNonConformi,
                                                Err_Code, Err_Des,
                                                Gias.ControllareMassimoApportoMg,
                                                0, 0, 0,
                                                "", "",
                                                distinta.Item("appezza"),
                                                distinta.Item("id_reg"),
                                                "", "", "",
                                                0, 0,
                                                0, 0,
                                                0, 0,
                                                Mg_Massimo, Mg_Distribuito,
                                                0, 0,
                                                0, 0)
                    End If
                End If


                '----------------
                '   RAME
                '----------------
                '(20/06/2017 fede) aggiunto controllo rame nella concimazione in caso di dpi e bio
                'controllo anche trattamenti rameici
                'AF: 05/23 -- IL CONTROLLO RAME SI FA A PRESCINDERE DAL REGOLAMENTO SELEZIONATO O MENO
                'If (Disciplinare_Cod > 0 Or Disciplinare_Cod = -2) And Cu_Operazione > 0 Then

                If Cu_Operazione > 0 Then
                    Cu_Distribuito += Cu_Distribuito_Trattamenti

                    Dim CU_Trattamenti_MultiAttivita As Decimal = 0
                    If paramVerificaDPIMultiAttivita IsNot Nothing Then
                        If paramVerificaDPIMultiAttivita.TotCUxHa_Trattamenti_MultiAttivita_xDistinta IsNot Nothing Then
                            For Each distintaxCU In paramVerificaDPIMultiAttivita.TotCUxHa_Trattamenti_MultiAttivita_xDistinta
                                '((PIVA, SA_COD, APPEZZA, ID_REG), CU_xDISTINTA)
                                Dim k1 = distintaxCU.Key

                                Dim t_piva As String = k1.Item1
                                Dim t_sa_cod As Integer = k1.Item2
                                Dim t_appezza As Integer = k1.Item3
                                Dim t_id_reg As Integer = k1.Item4

                                'Vado a ripescare il CU di tutti i trattamenti appena fatti sulla distinta che sto guardando 
                                'Escludo il CU del trattamento corrente perché viene già contato in CU_Operazione
                                If t_piva = distinta.Item("piva") And
                                        t_sa_cod = distinta.Item("sa_cod") And
                                        t_appezza = distinta.Item("appezza") And
                                        t_id_reg = distinta.Item("id_reg") Then

                                    CU_Trattamenti_MultiAttivita += distintaxCU.Value
                                End If
                            Next
                        End If
                        Cu_Distribuito += CU_Trattamenti_MultiAttivita
                    End If

                    Dim Cu_Massimo = 6
                    If Data_Riferimento >= #1/1/2019# Then
                        Cu_Massimo = 4
                    Else
                        Cu_Massimo = 6
                    End If

                    'SE SIAMO IN MODIFICA/INSERIMENTO DI UNA CONCIMAZIONE, DEVO SOMMARE IL CU_OPERAZIONE
                    If Not isVerificaPosteriori Then
                        'AGGIUNGO IL CU DELL'OPERAZIONE
                        Cu_Distribuito += Cu_Operazione
                    End If

                    'If Cu_Distribuito > Cu_Massimo Then
                    If (Cu_Distribuito - Cu_Massimo) > 0.001 Then
                        Err_Code = enTipoErrCode_Verifica.DoseEccessivaRame
                        Err_Des = String.Format(Gias.DoseRameDistribuitaSuAppSuperaMaxConsentitaAnno, app_nome, Math.Round(Cu_Distribuito, 3), Cu_Massimo)

                        Inserisci_NonConformita(DatiInseriti,
                                                XmlDoc, XmlDatiNonConformi,
                                                Err_Code, Err_Des,
                                                String.Format(Gias.ConsentitoUsareMaxRameAnno, Cu_Massimo),
                                                0, 0, 0,
                                                "", "",
                                                distinta.Item("appezza"),
                                                distinta.Item("id_reg"),
                                                "", "", "",
                                                0, 0, 0, 0, 0, 0, 0, 0,
                                                Cu_Massimo, Cu_Distribuito,
                                                0, 0)
                    End If
                End If
            Next


        Catch ex As Exception

            RisultatoFunzione = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

        Finally

            If FlagConnessioneLocale Then
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

    End Sub

    '================================================================================================
    '================================================================================================
    '================================================================================================
    'NON METTERE QUESTA ROUTINE QUI!!!!!!!
    'NEI DPI VANNO SOLO LE ROUTINE UTILIZZATE DAL WEB SERVICE!!!!!!
    '================================================================================================
    '================================================================================================
    '================================================================================================
    ''Lettura dei macroelementi associati ad un fertilizzante
    'Public Sub MacroElementi_da_Prodotto(ByVal PIVA As String, _
    '                                     ByVal Pro_Cod As Int32, _
    '                                     ByVal Mat_Cod As Int32, _
    '                                     ByRef N As Decimal, _
    '                                     ByRef P2O5 As Decimal, _
    '                                     ByRef K2O As Decimal, _
    '                                     ByRef MgO As Decimal, _
    '                                     ByRef objParametri As AgronicaCoreParametri)


    '    'Reset
    '    N = 0
    '    P2O5 = 0
    '    K2O = 0
    '    MgO = 0

    '    Dim DtProdotti As DataTable

    '    'ObjProdotti = CreateObject("Agro_Contab_AD.Prodotti_R")
    '    Dim ObjProdotti As New AgronicaCoreAnagrafeDAL.Prodotti_R
    '    'ObjMaterie_Prime = CreateObject("Agro_Contab_AD.Materie_Prime_R")
    '    Dim ObjMaterie_Prime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

    '    Select Case Mat_Cod

    '        Case 0

    '            '            '################################################################
    '            '            '###### ARCHIVI GENERALI  #######################################
    '            '            '################################################################





    '            DtProdotti = ObjProdotti.Leggi("Fertilizzanti", _
    '                                               "Fer_Cod", _
    '                                                Pro_Cod, _
    '                                                enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                "", _
    '                                                "", _
    '                                                objParametri)

    '        Case Else

    '            '################################################################
    '            '###### ARCHIVI AZIENDALI #######################################
    '            '################################################################


    '            DtProdotti = ObjMaterie_Prime.Leggi(PIVA, _
    '                                                0, _
    '                                                3, _
    '                                                Mat_Cod, _
    '                                                "", _
    '                                                0, _
    '                                                0, _
    '                                                0, _
    '                                                0, _
    '                                                0, _
    '                                                0, _
    '                                                0, _
    '                                                "", _
    '                                                0, _
    '                                                "", _
    '                                                True, _
    '                                                False, _
    '                                                "", _
    '                                                enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                "", _
    '                                                "", _
    '                                                objParametri)

    '    End Select

    '    If DtProdotti.Rows.Count <> 0 Then
    '        N = CDbl(DtProdotti.Rows(0).Item("N"))
    '        P2O5 = CDbl(DtProdotti.Rows(0).Item("P2O5"))
    '        K2O = CDbl(DtProdotti.Rows(0).Item("K2O"))
    '        MgO = CDbl(DtProdotti.Rows(0).Item("MgO"))

    '    End If


    '    'Distruggo gli oggetti
    '    DtProdotti = Nothing
    '    ObjProdotti = Nothing
    '    ObjMaterie_Prime = Nothing

    'End Sub

    '    'Rileva la quantità in kg ancora distribuibile sull'impianto affinché rispetti
    '    'il massimo apporto di macroelementi
    '    '============================================================================
    '    Public Function DPI_Rileva_Quantita_Distribuibile(ByVal PIVA As String, _
    '                                                      ByVal Sa_Cod As Int32, _
    '                                                      ByVal Appezza As Int32, _
    '                                                      ByVal Id_Reg As Int32, _
    '                                                      ByVal Pro_Cod As Int32, _
    '                                                      ByVal Mat_Cod As Int32, _
    '                                                      ByVal Progetto_Cod As Int32, _
    '                                                      ByVal Data_Riferimento As Date, _
    '                                                      ByRef objParametri As AgronicaCoreParametri _
    '                                                      ) As Decimal
    '        '============================================================================

    '        Dim NomeRoutine As String = "DpiBIZ.DPI_Verifica_R.DPI_Rileva_Quantita_Distribuibile()"
    '        Dim MessaggioErrore As String = ""
    '        Dim FlagConnessioneLocale As Boolean = False
    '        Dim xConnessione As OleDbConnection
    '         
    '        Dim xTransazione As OleDbTransaction
    '        Dim i As Int32
    '        Dim j As Int32
    '        Dim RisultatoFunzione As String = String.Empty

    '        Dim Lotto As String
    '        Dim Cul_Cod As Long
    '        Dim Sup_Imp As Decimal
    '        Dim Qta As Decimal

    '        Dim N_Distribuito As Decimal
    '        Dim P_Distribuito As Decimal
    '        Dim K_Distribuito As Decimal
    '        Dim Mg_Distribuito As Decimal

    '        Dim N_Massimo As Decimal
    '        Dim P_Massimo As Decimal
    '        Dim K_Massimo As Decimal
    '        Dim Mg_Massimo As Decimal

    '        Dim N_Prodotto As Decimal
    '        Dim P_Prodotto As Decimal
    '        Dim K_Prodotto As Decimal
    '        Dim Mg_Prodotto As Decimal

    '        Dim N_Residuo As Decimal
    '        Dim P_Residuo As Decimal
    '        Dim K_Residuo As Decimal
    '        Dim Mg_Residuo As Decimal

    '        Dim bCodiciSettati As Boolean
    '        '------------------------------

    '        Const MetodoNome = "DPI_Rileva_Quantita_Distribuibile:"

    '        '------------------------------
    '        Try

    '            '------------------------------
    '            'Verifico se è stata impostata una connessione
    '            If IsNothing(objParametri.objConnessione) Then
    '                'Flag
    '                FlagConnessioneLocale = True
    '                'Creo la connessione localmente
    '                xConnessione = New OleDbConnection(objParametri.StringaConnessione)
    '                xTransazione = Nothing
    '            Else
    '                'Utilizzo quella passata come parametro
    '                xConnessione = objParametri.objConnessione
    '                xConnectionState = objParametri.objConnessione.State
    '                xTransazione = objParametri.objTransazione
    '            End If

    '            '------------------------------

    '            'Lettura Parametri Base (Sup_Imp, Cu_Cod)
    '            'ObjImpianti = CreateObject("Agro_Anagrafe_AD.Reg_Impianti_Read")
    '            Dim ObjImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
    '            Dim DtImpianti As DataTable

    '            DtImpianti = ObjImpianti.Leggi(CStr(PIVA), _
    '                                               CLng(Sa_Cod), _
    '                                               Appezza, _
    '                                               Id_Reg, _
    '                                               enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                               "", _
    '                                               "", _
    '                                               objParametri)

    '            If DtImpianti.Rows.Count <> 0 Then

    '                Sup_Imp = DtImpianti.Rows(0).Item("Sup_Imp")
    '                Cul_Cod = DtImpianti.Rows(0).Item("Cul_Cod")

    '            End If

    '            'Lotto Interno
    '            Lotto = LottoImpianto(PIVA, Sa_Cod, Appezza, Id_Reg, Cul_Cod, Progetto_Cod, Data_Riferimento, objParametri)

    '            '###############################################################################
    '            '################# CONTROLLI PIANO CONCIMAZIONE ################################
    '            '###############################################################################

    '            '===========================================================================================
    '            'Lettura dei massimi apporti macroelementi
    '            '-------------------------------------------------------------------------------------------
    '            'Dim ObjCodici As Agro_Anagrafe_AD.Reg_Impianti_Codici_R
    '            Dim ObjCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
    '            Dim DtCodici As DataTable

    '            bCodiciSettati = False
    '            DtCodici = ObjCodici.Leggi(PIVA, _
    '                                           Sa_Cod, _
    '                                           Appezza, _
    '                                           Id_Reg, _
    '                                           "", _
    '                                           0, _
    '                                           "", _
    '                                           enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                           "", _
    '                                           "", _
    '                                           objParametri)
    '            N_Massimo = 9999 'Dummy
    '            P_Massimo = 9999 'Dummy
    '            K_Massimo = 9999 'Dummy
    '            Mg_Massimo = 9999 'Dummy

    '            If DtCodici.Rows.Count <> 0 Then

    '                Dim Dr() As DataRow

    '                'Azoto
    '                Dr = DtCodici.Select("id_cod = 1050")
    '                If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
    '                    If IsNumeric(Dr(0).Item("val_cod")) Then
    '                        N_Massimo = Dr(0).Item("val_cod")
    '                        bCodiciSettati = True
    '                    End If
    '                End If

    '                'Potassio
    '                Dr = DtCodici.Select("id_cod = 1051")
    '                If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
    '                    If IsNumeric(Dr(0).Item("val_cod")) Then
    '                        P_Massimo = Dr(0).Item("val_cod")
    '                        bCodiciSettati = True
    '                    End If
    '                End If

    '                'Fosforo
    '                Dr = DtCodici.Select("id_cod = 1052")
    '                If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
    '                    If IsNumeric(Dr(0).Item("val_cod")) Then
    '                        K_Massimo = Dr(0).Item("val_cod")
    '                        bCodiciSettati = True
    '                    End If
    '                End If

    '                'Magnesio
    '                Dr = DtCodici.Select("id_cod = 1053")
    '                If Not Dr Is Nothing AndAlso Dr.Length > 0 Then
    '                    If IsNumeric(Dr(0).Item("val_cod")) Then
    '                        Mg_Massimo = Dr(0).Item("val_cod")
    '                        bCodiciSettati = True
    '                    End If
    '                End If

    '            End If


    '            '===========================================================================================


    '            If bCodiciSettati Then


    '                '===============================================================================
    '                'CONTROLLO MASSIMO APPORTO MACROELEMENTI
    '                '-------------------------------------------------------------------------------

    '                Dim ObjTracciabilita As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
    '                Dim DtTracciabilita As DataTable
    '                'ObjTracciabilita = CreateObject("Agro_Contab_AD.Movimenti_Dettagli_R")

    '                'Lettura tracciabilità di fertilizzanti distribuiti sul singolo impianto
    '                DtTracciabilita = ObjTracciabilita.LeggiTracciabilità( _
    '                                                PIVA, _
    '                                                Sa_Cod, _
    '                                                0, 0, 0, 3, 0, 0, 0, _
    '                                                Appezza, _
    '                                                Id_Reg, _
    '                                                0, "", 0, 0, 0, 0, 0, "", _
    '                                                AGRODATAINIZIO, _
    '                                                AGRODATAFINE, _
    '                                                enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                "", "", objParametri)

    '                If DtTracciabilita.Rows.Count <> 0 Then

    '                    'Do While Not DtTracciabilita.EOF
    '                    Dim iTra As Integer
    '                    For iTra = 0 To DtTracciabilita.Rows.Count - 1

    '                        MacroElementi_da_Prodotto(PIVA, _
    '                                                  CLng(DtTracciabilita.Rows(iTra).Item("Pro_Cod")), _
    '                                                  CLng(DtTracciabilita.Rows(iTra).Item("Mat_Cod")), _
    '                                                  N_Prodotto, _
    '                                                  P_Prodotto, _
    '                                                  K_Prodotto, _
    '                                                  Mg_Prodotto, _
    '                                                  objParametri)

    '                        'Aggiornamenti Macroelementi Distribuiti
    '                        N_Distribuito = N_Distribuito + (CDbl(DtTracciabilita.Rows(iTra).Item("Qta")) * N_Prodotto / (100 * Sup_Imp))
    '                        P_Distribuito = P_Distribuito + (CDbl(DtTracciabilita.Rows(iTra).Item("Qta")) * P_Prodotto / (100 * Sup_Imp))
    '                        K_Distribuito = K_Distribuito + (CDbl(DtTracciabilita.Rows(iTra).Item("Qta")) * K_Prodotto / (100 * Sup_Imp))
    '                        Mg_Distribuito = Mg_Distribuito + (CDbl(DtTracciabilita.Rows(iTra).Item("Qta")) * Mg_Prodotto / (100 * Sup_Imp))

    '                    Next iTra

    '                End If


    '                'Controllo Residui Piano di Concimazione
    '                N_Residuo = N_Massimo - N_Distribuito
    '                P_Residuo = P_Massimo - P_Distribuito
    '                K_Residuo = K_Massimo - K_Distribuito
    '                Mg_Residuo = Mg_Massimo - Mg_Distribuito

    '                'Ricavo i quantitativi dei macroelementi del prodotto
    '                MacroElementi_da_Prodotto(PIVA, _
    '                                          Pro_Cod, _
    '                                          Mat_Cod, _
    '                                          N_Prodotto, _
    '                                          P_Prodotto, _
    '                                          K_Prodotto, _
    '                                          Mg_Prodotto, _
    '                                          objParametri)

    '                'Residuo sull'azoto
    '                Qta = (N_Residuo * 100) / (N_Prodotto + 0.01)

    '                If Qta > (P_Residuo * 100) / (P_Prodotto + 0.01) Then
    '                    'Residuo sul Potassio
    '                    Qta = (P_Residuo * 100) / (P_Prodotto + 0.01)
    '                End If

    '                If Qta > (K_Residuo * 100) / (K_Prodotto + 0.01) Then
    '                    'Residuo sul Fosforo
    '                    Qta = (K_Residuo * 100) / (K_Prodotto + 0.01)
    '                End If

    '                If Qta > (Mg_Residuo * 100) / (Mg_Prodotto + 0.01) Then
    '                    'Residuo sul Magnesio
    '                    Qta = (Mg_Residuo * 100) / (Mg_Prodotto + 0.01)
    '                End If

    '                DPI_Rileva_Quantita_Distribuibile = Format(Qta, "###,###.00#")

    '            Else
    '                DPI_Rileva_Quantita_Distribuibile = -2 'Codici non settati

    '            End If
    '            '------------------------------


    '        Catch ex As Exception

    '            RisultatoFunzione = ""
    '            MessaggioErrore = ex.Message
    '            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '            DPI_Rileva_Quantita_Distribuibile = -1

    '        Finally

    '            If FlagConnessioneLocale = True Then
    '                If Not IsNothing(xConnessione) Then
    '                    xConnessione.Close()
    '                    xConnessione.Dispose()
    '                End If
    '            Else
    '                If xConnectionState = ConnectionState.Closed Then
    '                    xConnessione.Close()
    '                End If
    '            End If

    '        End Try

    '    End Function
    '================================================================================================
    '================================================================================================
    '================================================================================================
    'NON METTERE QUESTA ROUTINE QUI!!!!!!!
    'NEI DPI VANNO SOLO LE ROUTINE UTILIZZATE DAL WEB SERVICE!!!!!!
    '================================================================================================
    '================================================================================================
    '================================================================================================

    '    '---------------------------------------------------------------------------------------------------------
    '    'Rileva la quantità in kg di un principio attivo distribuito sull'impianto contro una particolare avversita
    '    '=========================================================================================================
    '    Public Function DPI_PA_Distribuito(ByVal PIVA As String, _
    '                                       ByVal Sa_Cod As Int32, _
    '                                       ByVal Appezza As Int32, _
    '                                       ByVal Id_Reg As Int32, _
    '                                       ByVal Pa_Cod As Int32, _
    '                                       ByVal Av_Gru As Int32, _
    '                                       ByVal Av_Cod As Int32, _
    '                                       ByVal Id_Agenda_Escluso As Int32, _
    '                                        ByRef objParametri As AgronicaCoreParametri _
    '                                        ) As Decimal
    '        '============================================================================

    '        Dim NomeRoutine As String = "DpiBIZ.DPI_Verifica_R.DPI_PA_Distribuito()"
    '        Dim MessaggioErrore As String = ""
    '        Dim FlagConnessioneLocale As Boolean = False
    '        Dim xConnessione As OleDbConnection
    '         
    '        Dim xTransazione As OleDbTransaction
    '        Dim i As Int32
    '        Dim j As Int32
    '        Dim RisultatoFunzione As String = String.Empty

    '        Dim strPa_Cod As String

    '        '------------------------------

    '        Const MetodoNome = "DPI_PA_Distribuito:"

    '        '------------------------------

    '        Try

    '            '------------------------------
    '            'Verifico se e' stata impostata una connessione
    '            If IsNothing(objParametri.objConnessione) Then
    '                'Flag
    '                FlagConnessioneLocale = True
    '                'Creo la connessione localmente
    '                xConnessione = New OleDbConnection(objParametri.StringaConnessione)
    '                xTransazione = Nothing
    '            Else
    '                'Utilizzo quella passata come parametro
    '                xConnessione = objParametri.objConnessione
    '                xConnectionState = objParametri.objConnessione.State
    '                xTransazione = objParametri.objTransazione
    '            End If

    '            '------------------------------

    '            '###############################################################################
    '            '##################### CONTROLLI FORMULATI #####################################
    '            '###############################################################################

    '            strPa_Cod = "(" & Pa_Cod & ")"

    '            DPI_PA_Distribuito = 0
    '            '---------------------------------

    '            'ObjTracciabilita = CreateObject("Agro_Contab_AD.Movimenti_Dettagli_R")
    '            Dim ObjTracciabilita As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
    '            Dim DtTracciabilita As DataTable

    '            'Lettura dei dettagli degli interventi eseguiti precedentemente sull'impianto

    '            DtTracciabilita = ObjTracciabilita.LeggiLavorazioni_Da_Principi_Attivi(PIVA, _
    '                                                                    Sa_Cod, _
    '                                                                    strPa_Cod, _
    '                                                                    Av_Cod, _
    '                                                                    Av_Gru, _
    '                                                                    Appezza, _
    '                                                                    Id_Reg, _
    '                                                                    AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                                                                    "", _
    '                                                                    "", _
    '                                                                    objParametri)
    '            If DtTracciabilita.Rows.Count <> 0 Then

    '                'Do While Not RsTracciabilita.EOF
    '                Dim iTra As Integer
    '                For iTra = 0 To DtTracciabilita.Rows.Count - 1

    '                    If Id_Agenda_Escluso <> CLng(DtTracciabilita.Rows(0).Item("Id_Agenda")) Then
    '                        DPI_PA_Distribuito = DPI_PA_Distribuito + CDbl(DtTracciabilita.Rows(0).Item("Dest_Qta"))
    '                    End If

    '                Next iTra

    '            End If

    '            '----------------

    '        Catch ex As Exception

    '            RisultatoFunzione = ""
    '            MessaggioErrore = ex.Message
    '            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '            DPI_PA_Distribuito = -1

    '        Finally

    '            If FlagConnessioneLocale = True Then
    '                If Not IsNothing(xConnessione) Then
    '                    xConnessione.Close()
    '                    xConnessione.Dispose()
    '                End If
    '            Else
    '                If xConnectionState = ConnectionState.Closed Then
    '                    xConnessione.Close()
    '                End If
    '            End If

    '        End Try

    '    End Function


    '###############################################################################
    Public Sub VerificaEpoche(ByRef Err_Code As Int32,
                              ByRef Err_Des As String,
                              ByVal PIVA As String,
                              ByVal Sa_Cod As Int32,
                              ByVal Appezza As Int32,
                              ByVal Id_Reg As Int32,
                              ByVal Lotto As String,
                              ByVal Da_Epoca_Cod As Int32,
                              ByVal Da_Epoca_Des As String,
                              ByVal A_Epoca_Cod As Int32,
                              ByVal A_Epoca_Des As String,
                              ByVal Offset As Int32,
                              ByVal Data_Movimento As Date,
                              ByVal Pro_Des As String,
                              ByVal Validita_Inizio As Date,
                              ByVal Validita_Fine As Date,
                              ByRef objparametri As AgronicaCoreParametri)

        Dim Data_Valida As Date

        Dim ObjAgro_Contab_AD As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim DtAgro_Contab_AD As DataTable

        Err_Code = 0
        Err_Des = ""

        'Epoche Parametrizzate:
        '- 123: Raccolta
        '- 54:  Presemina

        Select Case A_Epoca_Cod

            Case 123 'Raccolta

                'Ricavo la data di raccolta valida entro l'arco temporale del progetto vegetale
                DtAgro_Contab_AD = ObjAgro_Contab_AD.Leggi_Raccolte(PIVA, Sa_Cod, 0, 0, 0, Appezza, Id_Reg, 0, Validita_Inizio, Validita_Fine, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objparametri)

                If DtAgro_Contab_AD.Rows.Count > 0 Then
                    Data_Valida = DateAdd("d", Offset, CDate(DtAgro_Contab_AD.Rows(0).Item("Data_Movimento")))

                    'Verifico se la data di movimento è entro periodo valido
                    If Data_Valida < Data_Movimento Then
                        Err_Code = 301
                        Err_Des = "L'intervento colturale effettuato sull'impianto " & Lotto & " è consentito solo entro " & IIf(Offset = 0, "la ", Math.Abs(Offset) & " giorni dalla ") & " data di " & A_Epoca_Des & " registrata il " & CDate(DtAgro_Contab_AD.Rows(0).Item("Data_Movimento")).ToShortDateString & ". "
                    End If


                End If

                ObjAgro_Contab_AD = Nothing
                DtAgro_Contab_AD = Nothing


            Case 54, 55, 67, 87 'Pre semina, pre trapianto, semina, trapianto

                Dim strFiltro As String = " Agenda.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " " &
                                             "  AND  Agenda.Validita_Inizio >= " & Agro_SQL_SaveDate(Validita_Inizio) & " " &
                                             " AND (Lav_Cod = 2 OR Lav_Cod = 71) "

                DtAgro_Contab_AD = ObjAgro_Contab_AD.Leggi(PIVA,
                                             Sa_Cod,
                                             0,
                                             0,
                                             0,
                                             Appezza,
                                             Id_Reg,
                                             0,
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             strFiltro,
                                             "",
                                             objparametri)

                If DtAgro_Contab_AD.Rows.Count > 0 Then

                    Dim iCont As Integer

                    For iCont = 0 To DtAgro_Contab_AD.Rows.Count - 1

                        Data_Valida = DateAdd("d", Offset, CDate(DtAgro_Contab_AD.Rows(iCont).Item("Validita_Inizio")))

                        If Data_Valida < Data_Movimento Then
                            Err_Code = 302
                            Err_Des = "L'utilizzo di '" & Pro_Des & "' è consentito solo su terreni privi di coltura. La data di " & IIf(DtAgro_Contab_AD.Rows(iCont)("Lav_Cod") = 2, "semina", "trapianto") & " risulta effettuata sull'impianto " & Lotto & " in data " & CDate(DtAgro_Contab_AD.Rows(iCont).Item("Validita_Inizio")).ToShortDateString & ". "
                        End If
                    Next

                End If

        End Select


    End Sub


    Private Sub Inserisci_NonConformita(ByRef DatiInseriti(,) As String,
                                        ByRef XmlDoc As XmlDocument,
                                        ByRef XmlDatiNonConformi As XmlElement,
                                        ByRef Err_Code As Int32,
                                        ByRef Err_Des As String,
                                        ByVal Consultazione As String,
                                        ByVal Pro_Cod As Int32,
                                        ByVal Mat_Cod As Int32,
                                        ByVal Udm_Cod As Int32,
                                        ByVal Dose As String,
                                        ByVal Dose_Consentita As String,
                                        ByVal Appezza As Int32,
                                        ByVal Id_Reg As Int32,
                                        ByVal strPa_Cod As String,
                                        ByVal Interventi As String,
                                        ByVal Interventi_Consentiti As String,
                                        ByVal N As Decimal,
                                        ByVal N_Distribuito As Decimal,
                                        ByVal P As Decimal,
                                        ByVal P_Distribuito As Decimal,
                                        ByVal K As Decimal,
                                        ByVal K_Distribuito As Decimal,
                                        ByVal Mg As Decimal,
                                        ByVal Mg_Distribuito As Decimal,
                                        ByVal Cu As Decimal,
                                        ByVal Cu_Distribuito As Decimal,
                                        ByVal Av_Gru As Int32,
                                        ByVal Av_Cod As Int32)


        Dim Indice As Int32
        Dim XmlDatoNonConforme As XmlElement
        Dim bPresente As Boolean

        'Controllo che l'errore non sia già stato inserito nella stringa xml
        bPresente = False

        For Indice = 0 To UBound(DatiInseriti, 2) - 1
            If CLng(DatiInseriti(0, Indice)) = Err_Code AndAlso
               CLng(DatiInseriti(1, Indice)) = Pro_Cod AndAlso
               CLng(DatiInseriti(2, Indice)) = Mat_Cod AndAlso
               CLng(DatiInseriti(3, Indice)) = Udm_Cod AndAlso
               CLng(DatiInseriti(4, Indice)) = Appezza AndAlso
               CLng(DatiInseriti(5, Indice)) = Id_Reg AndAlso
               CStr(DatiInseriti(6, Indice)) = strPa_Cod AndAlso
               CLng(DatiInseriti(7, Indice)) = Av_Gru AndAlso
               CLng(DatiInseriti(8, Indice)) = Av_Cod Then

                bPresente = True

                Exit For
            End If
        Next Indice

        If Not bPresente Then

            XmlDatoNonConforme = XmlDoc.CreateElement("DatoNonConforme")
            XmlDatoNonConforme.SetAttribute("err_code", Err_Code)
            XmlDatoNonConforme.SetAttribute("err_des", Err_Des)
            XmlDatoNonConforme.SetAttribute("consultazione", Consultazione)
            XmlDatoNonConforme.SetAttribute("pro_cod", Pro_Cod)
            XmlDatoNonConforme.SetAttribute("mat_cod", Mat_Cod)
            XmlDatoNonConforme.SetAttribute("udm_cod", Udm_Cod)
            XmlDatoNonConforme.SetAttribute("dose", Dose & " ")
            XmlDatoNonConforme.SetAttribute("dose_consentita", Dose_Consentita & " ")
            XmlDatoNonConforme.SetAttribute("appezza", Appezza)
            XmlDatoNonConforme.SetAttribute("id_reg", Id_Reg)
            XmlDatoNonConforme.SetAttribute("strpa_cod", strPa_Cod)
            XmlDatoNonConforme.SetAttribute("interventi", Interventi & " ")
            XmlDatoNonConforme.SetAttribute("interventi_consentiti", Interventi_Consentiti & " ")
            XmlDatoNonConforme.SetAttribute("n", N)
            XmlDatoNonConforme.SetAttribute("n_distribuito", N_Distribuito)
            XmlDatoNonConforme.SetAttribute("p", P)
            XmlDatoNonConforme.SetAttribute("p_distribuito", P_Distribuito)
            XmlDatoNonConforme.SetAttribute("k", K)
            XmlDatoNonConforme.SetAttribute("k_distribuito", K_Distribuito)
            XmlDatoNonConforme.SetAttribute("mg", Mg)
            XmlDatoNonConforme.SetAttribute("mg_distribuito", Mg_Distribuito)
            XmlDatoNonConforme.SetAttribute("cu", Cu)
            XmlDatoNonConforme.SetAttribute("cu_distribuito", Cu_Distribuito)
            XmlDatoNonConforme.SetAttribute("av_gru", Av_Gru)
            XmlDatoNonConforme.SetAttribute("av_cod", Av_Cod)


            XmlDatiNonConformi.AppendChild(XmlDatoNonConforme)

            XmlDatoNonConforme = Nothing

            'Inserimento in struttura 'DatiInseriti'
            ReDim Preserve DatiInseriti(UBound(DatiInseriti, 1), UBound(DatiInseriti, 2) + 1)

            DatiInseriti(0, UBound(DatiInseriti, 2) - 1) = Err_Code
            DatiInseriti(1, UBound(DatiInseriti, 2) - 1) = Pro_Cod
            DatiInseriti(2, UBound(DatiInseriti, 2) - 1) = Mat_Cod
            DatiInseriti(3, UBound(DatiInseriti, 2) - 1) = Udm_Cod
            DatiInseriti(4, UBound(DatiInseriti, 2) - 1) = Appezza
            DatiInseriti(5, UBound(DatiInseriti, 2) - 1) = Id_Reg
            DatiInseriti(6, UBound(DatiInseriti, 2) - 1) = strPa_Cod
            DatiInseriti(7, UBound(DatiInseriti, 2) - 1) = Av_Gru
            DatiInseriti(8, UBound(DatiInseriti, 2) - 1) = Av_Cod


        End If


        'Reset Errore
        Err_Code = 0
        Err_Des = ""

    End Sub



    Public Function DPI_Verifica_Carenza(ByRef Err_Code As Int32,
                                        ByVal PIVA As String,
                                        ByVal Sa_Cod As Int32,
                                        ByVal Appezza As Int32,
                                        ByVal Id_Reg As Int32,
                                        ByVal Veg_Cod As Int32,
                                        ByVal Grfi_Cod As Int32,
                                        ByVal Cop_Cod As Int32,
                                        ByVal Validita_Inizio As Date,
                                        ByVal Validita_Fine As Date,
                                        ByVal Data_Raccolta As Date,
                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreParametri,
                                         Optional ByVal DTCarenze As DataTable = Nothing,
                                         Optional ByRef DataCarenza_isRaccoltaNG As String = ""
                                         ) As String


        Dim Pro_Cod As Int32
        Dim Pro_Cod_Test As Int32
        Dim Data As Date
        Dim DataTest As Date
        Dim Pro_Des As String
        Dim Descrizione As String = ""
        Dim For_Veg_Cod As Integer

        Dim DtTraccia As DataTable

        Data = AGRODATAINIZIO
        Pro_Cod = 0
        Pro_Des = ".... "

        Dim ObjTraccia As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

        DtTraccia = ObjTraccia.LeggiTracciabilita(PIVA,
                                                      Sa_Cod,
                                                      0, 0, 0,
                                                      FORMULATI,
                                                      0, 0, 0,
                                                      Appezza,
                                                      Id_Reg,
                                                      0,
                                                      "", 0, 0, 0, 0, 0, "",
                                                      Validita_Inizio,
                                                      Data_Raccolta,
                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                      "",
                                                      "",
                                                      objParametri_Server)
        ObjTraccia = Nothing

        If DtTraccia.Rows.Count > 0 Then

            'Do While Not RsTraccia.EOF
            Dim iTra As Integer
            For iTra = 0 To DtTraccia.Rows.Count - 1

                ''Verifico che l'intervento sia valido per il controllo
                ''-- sia antecedente la data di raccolta non oltre 7 mesi
                'If Data_Raccolta > CDate(DtTraccia.Rows(iTra).Item("Data_Movimento")) And _
                '   Data_Raccolta < DateAdd("m", 7, CDate(DtTraccia.Rows(iTra).Item("Data_Movimento"))) Then

                'Verifico che l'intervento sia valido per il controllo
                '-- sia antecedente la data di raccolta e la fine della distinta
                If Data_Raccolta > CDate(DtTraccia.Rows(iTra).Item("Data_Movimento")) AndAlso
                   Data_Raccolta < Validita_Fine Then

                    Pro_Cod_Test = DtTraccia.Rows(iTra).Item("Pro_Cod")

                    For_Veg_Cod = 0
                    If Not IsDBNull(DtTraccia.Rows(iTra).Item("dett_extra_str")) AndAlso IsNumeric(DtTraccia.Rows(iTra).Item("dett_extra_str")) Then
                        For_Veg_Cod = DtTraccia.Rows(iTra).Item("dett_extra_str")
                    End If

                    If Pro_Cod_Test <> 0 Then

                        Dim Carenza As Integer = -1

                        If DTCarenze IsNot Nothing Then

                            'Dim DrCarenza() As DataRow = DTCarenze.Select("fr_cod=" & Pro_Cod_Test & " AND validita_inizio<='" & Data_Raccolta & "' AND validita_fine>='" & Data_Raccolta & "'")

                            Dim strFiltroProtetto = ""

                            Select Case Cop_Cod
                                Case 0, 3, 4, 5, 6, 1 'Nessuno
                                    strFiltroProtetto = " AND flag_protetto<> 1"
                                Case Else
                                    strFiltroProtetto = " AND flag_protetto<> 2"
                            End Select


                            Dim DrCarenza As DataRow() '= DTCarenze.Select("fr_cod=" & Pro_Cod_Test & " AND validita_inizio<='" & Data_Raccolta & "' AND validita_fine>='" & Data_Raccolta & "'" & strFiltroProtetto)

                            If For_Veg_Cod = 0 Then
                                DrCarenza = DTCarenze.Select("fr_cod=" & Pro_Cod_Test & " AND validita_inizio<='" & Data_Raccolta & "' AND validita_fine>='" & Data_Raccolta & "'" & strFiltroProtetto)
                            Else
                                DrCarenza = DTCarenze.Select("fr_cod=" & Pro_Cod_Test & " AND for_veg_cod=" & For_Veg_Cod & " AND validita_inizio<='" & Data_Raccolta & "' AND validita_fine>='" & Data_Raccolta & "'" & strFiltroProtetto)
                            End If


                            If DrCarenza IsNot Nothing AndAlso DrCarenza.Length > 0 Then
                                Carenza = DrCarenza(0).Item("tempocarenza")
                            End If
                        Else
                            Dim objWs As New AgronicaCoreWebService.AgroWs
                            Carenza = objWs.TempoCarenza_from_FrCod_VegCod(Pro_Cod_Test,
                                                                                   Veg_Cod,
                                                                                   For_Veg_Cod,
                                                                                   Grfi_Cod,
                                                                                   Cop_Cod,
                                                                                   Data_Raccolta,
                                                                                   objParametri_Server,
                                                                                    objParametri_Utenti)
                            objWs = Nothing
                        End If


                        If Carenza <> -1 AndAlso Carenza <> 0 Then

                            'ai giorni di carenza aggiungo 1gg per evitare controversie con i controllori

                            DataTest = DateAdd("d", Carenza + 1, CDate(DtTraccia.Rows(iTra).Item("Data_Movimento")))

                            If DataTest > Data Then

                                'Aggiornamento
                                Pro_Cod = Pro_Cod_Test
                                Data = DataTest
                                Descrizione = " (Carenza: " & Carenza & " gg) in data " & DtTraccia.Rows(iTra).Item("Data_Movimento")

                                'Ritorno la Data per la griglia degli impianti nelle Operazioni NG
                                DataCarenza_isRaccoltaNG = Data
                            End If

                        End If

                    End If

                End If

            Next iTra

        End If

        Select Case Data

            Case AGRODATAINIZIO

                Err_Code = 0
                Return "Nessun Formulato Distribuito"

            Case Else

                'Lettura del Prodotto
                Dim ObjFormulati As New AgronicaCoreMetaSchemaDAL.Formulati_R
                Dim DtFormulati As DataTable

                DtFormulati = ObjFormulati.Leggi(Pro_Cod,
                                                 AGRODATAINIZIO,
                                                 AGRODATAFINE,
                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                 "",
                                                "",
                                                objParametri_Server)
                ObjFormulati = Nothing

                If DtFormulati.Rows.Count > 0 Then
                    Pro_Des = LCase(DtFormulati.Rows(0).Item("Fr_Des"))
                End If

                If Data_Raccolta < Data Then
                    Err_Code = enTipoErrCode_Verifica.CarenzaNonRispettata
                Else
                    Err_Code = 0 'Tempi di carenza rispettati
                End If

                Return Data & " - Riferimento: '" & Pro_Des & "' " & Descrizione

        End Select

    End Function


    '############################################################################################################
    '############################################################################################################
    '######### INSERIMENTO WARNING DA EFFETTUARE SUCCESSIVAMENTE SU GIAS SERVER  ################################
    '############################################################################################################
    '############################################################################################################

    Private Sub Inserisci_Warning(ByRef XmlDoc As XmlDocument,
                                  ByRef XmlDatiWarnings As XmlElement,
                                  ByRef Warning_Code As Integer, ByRef Warning_Des As String,
                                  ByVal Appezza As Integer, ByVal Id_Reg As Integer, ByVal Lotto As String,
                                  ByVal Av_Gru As Integer, ByVal Av_Cod As Integer,
                                  ByVal Avversita_Des As String,
                                  ByVal Pro_Cod As Integer, ByVal Pro_Des As String,
                                  ByVal Pa_Cod As Integer, ByVal Pa_Des As String,
                                  ByVal Da_Ep_Cod As Integer, ByVal Da_Ep_Des As String,
                                  ByVal A_Ep_Cod As Integer, ByVal A_Ep_Des As String,
                                  ByVal Offset As Integer, ByVal Interventi_Consentiti As Integer,
                                  ByVal Numero_Interventi_Scheda As Integer, ByVal strPa_Cod As String,
                                  ByVal Lav_Cod As Integer, ByVal Udm_Cod As Integer,
                                  ByVal Soglia As Integer, ByVal Note As String,
                                  ByVal Validita_Inizio As String, ByVal Validita_Fine As String,
                                  ByVal Sup_Imp As Decimal, ByVal Titolo As Decimal)

        Dim XmlWarning As XmlElement

        XmlWarning = XmlDoc.CreateElement("Warning")
        XmlWarning.SetAttribute("warning_code", Warning_Code)
        XmlWarning.SetAttribute("warning_des", Warning_Des)
        XmlWarning.SetAttribute("appezza", Appezza)
        XmlWarning.SetAttribute("id_reg", Id_Reg)
        XmlWarning.SetAttribute("lotto", Lotto)
        XmlWarning.SetAttribute("sup_imp", Sup_Imp)
        XmlWarning.SetAttribute("pro_cod", Pro_Cod)
        XmlWarning.SetAttribute("pro_des", Pro_Des & " ")
        XmlWarning.SetAttribute("pa_cod", Pa_Cod)
        XmlWarning.SetAttribute("pa_des", Pa_Des & " ")
        XmlWarning.SetAttribute("titolo", Titolo)
        XmlWarning.SetAttribute("av_gru", Av_Gru)
        XmlWarning.SetAttribute("av_cod", Av_Cod)
        XmlWarning.SetAttribute("avversita_des", Avversita_Des & "")
        XmlWarning.SetAttribute("da_ep_cod", Da_Ep_Cod)
        XmlWarning.SetAttribute("da_ep_des", Da_Ep_Des & "")
        XmlWarning.SetAttribute("a_ep_cod", A_Ep_Cod)
        XmlWarning.SetAttribute("a_ep_des", A_Ep_Des & "")
        XmlWarning.SetAttribute("offset", Offset)
        XmlWarning.SetAttribute("interventi_consentiti", Interventi_Consentiti)
        XmlWarning.SetAttribute("interventi_scheda", Numero_Interventi_Scheda)
        XmlWarning.SetAttribute("strpa", strPa_Cod & "")
        XmlWarning.SetAttribute("lav_cod", Lav_Cod)
        XmlWarning.SetAttribute("udm_cod", Udm_Cod)
        XmlWarning.SetAttribute("soglia", Soglia)
        XmlWarning.SetAttribute("note", Note)
        XmlWarning.SetAttribute("validita_inizio", Validita_Inizio)
        XmlWarning.SetAttribute("validita_fine", Validita_Fine)

        XmlDatiWarnings.AppendChild(XmlWarning)

        XmlWarning = Nothing

    End Sub

    '============================================================================
    ''' <param name="TotCU_Fertilizzazione_MultiAttivita_xDistinta"> ((PIVA, SA_COD, APPEZZA, ID_REG), CU_xDISTINTA)
    ''' MI SALVO IL TOT DI RAME USATO NELLE CONCIMAZIONI X OGNI DISTINTA --> DA PASSARE AL VERIFICA TRATTAMENTI
    ''' </param>
    ''' <param name="dic_CUTrattamenti_xDistintaxLavCod"> ((PIVA, SA_COD, APPEZZA, ID_REG), LAV_COD), CU_xDISTINTAxLAV_COD)
    ''' MI SALVO IL TOT DI RAME USATO NELLE CONCIMAZIONI X OGNI DISTINTA X OGNI LAV_COD --> DA PASSARE AL VERIFICA TRATTAMENTI, COSI' DA POTER CONTARE I MULTI TRATTAMENTI
    ''' </param>
    Public Function XmlFINALE(ByVal DatiVerifica As String,
                              ByVal Id_Agenda_Escluso As Int32,
                              ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
                              ByRef objParametri_Server As AgronicaCoreParametri,
                              ByRef objParametri_Utenti As AgronicaCoreParametri,
                              Optional ByVal DatiAgenda As String = "",
                              Optional ByVal Biologico As Boolean = False,
                              Optional ByVal Dpi_Verificato As String = "",
                              Optional ByVal IDTestataTemp__tmp_FormulatiXPrincipiAttivi As Integer = -1,
                              Optional paramVerificaDPIMultiAttivita As AgronicaCoreModello.VerificaDPIMultiAttivita = Nothing
                              ) As rispostaStandard(Of Verifica_Disciplinare_Intervento)


        Dim r As New rispostaStandard(Of Verifica_Disciplinare_Intervento)
        r.RispostaStringa = New Verifica_Disciplinare_Intervento


        Dim InterventoConforme As Boolean = True

        Dim i As Integer
        Dim i_DatiRisultati As Long
        Dim i_DatiGenerali As Long
        Dim i_DatiNonConformi As Long
        Dim i_DatoNonConforme As Long
        Dim i_DatiWarnings As Long
        Dim i_DatoWarning As Long

        Dim XmlDoc As New XmlDocument
        Dim XmlDom As New XmlDocument

        Dim XML_Nodo As XmlElement

        Dim xDatiRisultati As XmlNodeList
        Dim xDatiRisultato As XmlElement
        Dim xDatiGenerali As XmlNodeList
        Dim xDatiGenerale As XmlElement
        Dim xDatiNonConformi As XmlNodeList
        Dim xDatoNonConforme As XmlElement
        Dim xNonConformi As XmlNodeList
        Dim xNonConforme As XmlElement
        Dim xDatiWarnings As XmlNodeList
        Dim xDatoWarning As XmlElement
        Dim xWarnings As XmlNodeList
        Dim xWarning As XmlElement

        Dim XmlDatiRisultatiFinali As XmlElement = Nothing
        Dim XmlDatiGeneraliFinali As XmlElement = Nothing
        Dim XmlDatiNonConformiFinali As XmlElement = Nothing
        Dim XmlDatoNonConformeFinale As XmlElement
        Dim XmlWarningFinali As XmlElement = Nothing
        Dim XmlNoteFinali As XmlElement = Nothing
        Dim xDatiDiserbo As XmlNodeList
        Dim xDatoDiserbo As XmlElement

        Dim PIVA As String
        Dim Sa_Cod As Integer
        Dim Veg_Cod As Integer
        Dim Disciplinare_Cod As Integer
        Dim Disciplinare_Des As String
        Dim Id_RcDpi As Integer
        Dim Id_RcDpi_Des As String
        Dim Data As Date
        Dim Des_Lib As String
        Dim TipoTestata As Integer
        Dim Lav_Cod As Integer
        Dim flagNuovoControlloRiduzioneDiserbo As Boolean = False

        Dim Err_Code As Integer
        Dim Err_Des As String

        Dim Warning_Code As Integer
        Dim Warning_Des As String

        Dim InserisciErrore As Boolean
        Dim InserisciWarning As Boolean

        Dim N_Warning As Integer

        Dim bConforme As Boolean

        Dim Appezza As Integer
        Dim Id_Reg As Integer
        Dim Lotto As String
        Dim Offset As Integer
        Dim Pa_Cod As Integer
        Dim Pa_Des As String
        Dim Titolo As Decimal
        Dim Peso As Decimal
        Dim Pro_Cod As Integer
        Dim Pro_Des As String
        Dim PaFrCod As Integer
        Dim Udm_Cod As Integer
        Dim Da_Ep_Cod As Integer
        Dim Da_Ep_Des As String
        Dim A_Ep_Cod As Integer
        Dim A_Ep_Des As String
        Dim strPA As String
        Dim strAvversita As String
        Dim strGruppiAvversita As String = ""
        Dim Max_Interventi As Integer
        Dim Min_Interventi As Integer
        Dim Max_Interventi_xProdotto As Integer
        Dim Max_Interventi_xProdotto_Udm As Integer
        Dim Max_Interventi_Udm_Des As String
        Dim Interventi_Scheda As Integer
        Dim Interventi_Scheda_Old As Integer
        Dim IntervalloTrattamenti_Min As Integer
        Dim IntervalloTrattamenti_Max As Integer
        Dim DoseDpi_MaxAnno As Decimal
        Dim DoseDpi_MaxAnno_Udm_Cod As Integer

        Dim Collegamento_Formulati_LU As Integer = 0
        Dim Elenco_Fr_Cod_LU As String = ""

        Dim DoseDpi_MaxAnno_Perc As Decimal
        Dim DoseDpi_MaxAnno_Peso As Decimal
        Dim DoseEtichetta_MaxAnno As Decimal
        Dim DoseEtichetta_MaxAnno_Udm_Cod As Integer
        Dim Consultazione As String = ""
        Dim Consultazione_Extra As String = ""
        Dim Epoca_Des As String
        Dim Superficie As String
        Dim Lav_Cod_Soglia As Integer
        Dim Udm_Cod_Soglia As Integer
        Dim Qta_Soglia As Decimal
        Dim Av_Cod As Integer
        Dim Av_Gru As Integer
        Dim Note As String
        Dim Soglie As String(,)
        Dim Qta_Rilievo As Decimal
        Dim Qta_Media_Rilievo As Decimal
        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date
        Dim Sup_Imp As Decimal
        Dim Raccoglitore_Cod As Integer

        Dim Max_Interventi_xProdotto_Globali As Integer
        Dim For_Veg_Av_Dos_Cod As Integer
        Dim Gruppo_Dosaggi As Integer

        Dim Sup_Coinvolta As Decimal = 0
        Dim strAppCoinvolti As String = ""

        Dim TipoOperazioneDB As Integer = 0
        Dim Destinazioni(9, 0) As String

        Dim HashProdottiOperazione As New Hashtable
        Dim HashPrincipiOperazione As New Hashtable

        Dim i_Piva As Integer = 0
        Dim i_Sa_Cod As Integer = 1
        Dim i_Appezza As Integer = 2
        Dim i_Id_Reg As Integer = 3
        Dim i_Sup_Imp As Integer = 4
        Dim i_Pro_Cod As Integer = 5
        Dim i_Qta As Integer = 6
        Dim i_Udm_Cod As Integer = 7
        Dim i_Dose As Integer = 8
        Dim i_Extra_Int As Integer = 9

        Dim HashErrori As New Hashtable


        'Variabili Controllo Dose Max Anno
        Dim Udm_Cod_Etichetta As Integer = 0
        Dim Udm_Des_Etichetta As String = ""
        Dim Udm_Cod_DPI As Integer = 0
        Dim Udm_Des_DPI As String = ""
        Dim Coefficiente_Etichetta As Decimal = 0
        Dim Coefficiente_DPI As Decimal = 0
        Dim Dose_Etichetta As Decimal = 0
        Dim Udm_Cod_Lavorazione As Integer = 0
        Dim Udm_Des_Lavorazione As String = ""
        Dim Coefficiente_Lavorazione As Decimal = 0
        Dim Dose_Lavorazione As Decimal = 0
        Dim Dose_Totale_Lavorazione As Decimal = 0
        Dim Udm_Des_Coefficiente_Etichetta As String = ""
        Dim Udm_Des_Coefficiente_DPI As String = ""
        Dim Udm_Des_Coefficiente_Lavorazione As String = ""


        '========================================================================================

        '#######################################################################################################
        '################### SPACCHETTAMENTO RISULTATO #########################################################
        '#######################################################################################################

        Dim impostazioni As New ImpostazioniUtentePerControlliBloccanti

        If VerificaSoloControlliImpostazioniUtente Then

            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            impostazioni = ObjUtenti.LeggiImpostazioniUtentePerControlliBloccanti(objParametri_Utenti)
        End If

        Try

            Dim IDTestataTemp As Integer = -1

            If DatiAgenda <> "" Then

                Dim XmlDocAgenda As New XmlDocument
                Dim xDatiAgende As XmlNodeList
                Dim xDatiAgenda As XmlElement
                Dim xAgende As XmlNodeList
                Dim xAgenda As XmlElement

                Dim xDatiMovimenti As XmlNodeList
                Dim xDatiMovimento As XmlElement
                Dim xMovimenti As XmlNodeList
                Dim xMovimento As XmlElement

                Dim xDatiMovimenti_Dettagli As XmlNodeList
                Dim xDatiMovimento_Dettaglio As XmlElement
                Dim xMovimenti_Dettagli As XmlNodeList
                Dim xMovimento_Dettaglio As XmlElement

                Dim xMov_Destinazioni As XmlNodeList
                Dim xMov_Destinazione As XmlElement

                XmlDocAgenda.LoadXml(DatiAgenda)

                xDatiAgende = XmlDocAgenda.GetElementsByTagName("DatiAgenda")

                Dim i_DatiAgenda As Integer = 0
                Dim i_Agenda As Integer = 0
                Dim i_DatiMovimento As Integer = 0
                Dim i_Movimento As Integer = 0
                Dim i_DatiMovimenti_Dettagli As Integer = 0
                Dim i_DatiMovimento_Dettaglio As Integer = 0
                Dim i_DatiMov_Destinazioni As Integer = 0


                '(10/08/2020 fede nuovi controlli tutti gli app assieme)
                Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                'IDTestataTemp = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("IDTestataTemp", objParametri_Server)
                IDTestataTemp = xAgrosequenze.NuovoId_Tabella("IDTestataTemp", 0, 20000000, objParametri_Server)
                'Dim OperazioneCorrente_FiltroImpianti As String

                Do While i_DatiAgenda < xDatiAgende.Count

                    xDatiAgenda = xDatiAgende.Item(i_DatiAgenda)
                    xAgende = xDatiAgenda.GetElementsByTagName("Agenda")

                    i_Agenda = 0



                    Do While i_Agenda < xAgende.Count

                        xAgenda = xAgende.Item(i_Agenda)

                        Raccoglitore_Cod = xAgenda.GetAttribute("raccoglitore_cod")

                        TipoOperazioneDB = CInt(xAgenda.GetAttribute("TipoOperazioneDB"))

                        xDatiMovimenti = xAgenda.GetElementsByTagName("DatiMovimenti")

                        i_DatiMovimento = 0

                        Do While i_DatiMovimento < xDatiMovimenti.Count

                            xDatiMovimento = xDatiMovimenti.Item(i_DatiMovimento)
                            xMovimenti = xDatiMovimento.GetElementsByTagName("Movimento")

                            i_Movimento = 0

                            Do While i_Movimento < xMovimenti.Count

                                xMovimento = xMovimenti.Item(i_Movimento)

                                Dim Data_Movimento As Date = xMovimento.GetAttribute("data_movimento")


                                'Verifico che il movimento sia la scheda di lavorazione
                                Select Case xMovimento.GetAttribute("cau_mov")

                                    Case CAU_TRATTAMENTO, CAU_RILIEVO_CAMPO, CAU_RILIEVO_RACCOLTA, CAU_LAVORAZIONE

                                        xDatiMovimenti_Dettagli = xMovimento.GetElementsByTagName("DatiMovimenti_Dettagli")

                                        i_DatiMovimenti_Dettagli = 0


                                        Do While i_DatiMovimenti_Dettagli < xDatiMovimenti_Dettagli.Count

                                            xDatiMovimento_Dettaglio = xDatiMovimenti_Dettagli.Item(i_DatiMovimenti_Dettagli)

                                            xMovimenti_Dettagli = xDatiMovimento_Dettaglio.GetElementsByTagName("Movimento_Dettaglio")

                                            i_DatiMovimento_Dettaglio = 0

                                            Do While i_DatiMovimento_Dettaglio < xMovimenti_Dettagli.Count

                                                xMovimento_Dettaglio = xMovimenti_Dettagli.Item(i_DatiMovimento_Dettaglio)

                                                Pro_Cod = CInt(xMovimento_Dettaglio.GetAttribute("pro_cod"))
                                                Udm_Cod = CInt(xMovimento_Dettaglio.GetAttribute("udm_cod"))

                                                If Not HashProdottiOperazione.ContainsKey(Pro_Cod) Then
                                                    HashProdottiOperazione.Add(Pro_Cod, "")
                                                End If

                                                If xMovimento_Dettaglio.HasAttribute("principiattivi") AndAlso xMovimento_Dettaglio.GetAttribute("principiattivi") <> "" Then
                                                    Dim Principi As String() = Split(xMovimento_Dettaglio.GetAttribute("principiattivi"), "|")
                                                    For fp = 0 To Principi.Length - 1
                                                        PaFrCod = Split(Principi(fp), "§")(0).Trim
                                                        If Not HashPrincipiOperazione.ContainsKey(Pro_Cod & "_" & PaFrCod) Then
                                                            HashPrincipiOperazione.Add(Pro_Cod & "_" & PaFrCod, "")
                                                        End If
                                                    Next
                                                End If

                                                xMov_Destinazioni = xMovimento_Dettaglio.GetElementsByTagName("Movimento_Destinazione")

                                                i_DatiMov_Destinazioni = 0
                                                Sup_Coinvolta = 0

                                                Do While i_DatiMov_Destinazioni < xMov_Destinazioni.Count

                                                    'Prelevo l'i-esimo Movimento Dettaglio
                                                    xMov_Destinazione = xMov_Destinazioni.Item(i_DatiMov_Destinazioni)

                                                    If CInt(xMov_Destinazione.GetAttribute("tipo_destinazione")) = 0 Then

                                                        ReDim Preserve Destinazioni(9, UBound(Destinazioni, 2) + 1)

                                                        Destinazioni(i_Piva, UBound(Destinazioni, 2) - 1) = xMov_Destinazione.GetAttribute("piva")
                                                        Destinazioni(i_Sa_Cod, UBound(Destinazioni, 2) - 1) = CInt(xMov_Destinazione.GetAttribute("sa_cod"))
                                                        Destinazioni(i_Appezza, UBound(Destinazioni, 2) - 1) = CInt(xMov_Destinazione.GetAttribute("appezza"))
                                                        Destinazioni(i_Id_Reg, UBound(Destinazioni, 2) - 1) = CInt(xMov_Destinazione.GetAttribute("id_destinazione"))
                                                        Destinazioni(i_Sup_Imp, UBound(Destinazioni, 2) - 1) = CDec(xMov_Destinazione.GetAttribute("qta2"))
                                                        Sup_Coinvolta += CDec(xMov_Destinazione.GetAttribute("qta2"))
                                                        Destinazioni(i_Pro_Cod, UBound(Destinazioni, 2) - 1) = Pro_Cod
                                                        Destinazioni(i_Udm_Cod, UBound(Destinazioni, 2) - 1) = Udm_Cod
                                                        Destinazioni(i_Qta, UBound(Destinazioni, 2) - 1) = CDec(xMov_Destinazione.GetAttribute("qta"))
                                                        Destinazioni(i_Dose, UBound(Destinazioni, 2) - 1) = CDec(xMovimento_Dettaglio.GetAttribute("qta"))
                                                        Destinazioni(i_Extra_Int, UBound(Destinazioni, 2) - 1) = CInt(xMovimento_Dettaglio.GetAttribute("extra_int"))

                                                        If i_DatiMovimento_Dettaglio = 0 Then

                                                            OperazioneCorrente_Scrivi___Tmp_Movimenti_Destinazioni_DateDistinta(IDTestataTemp,
                                                                                                                                xMov_Destinazione.GetAttribute("piva"),
                                                                                                                                xMov_Destinazione.GetAttribute("sa_cod"),
                                                                                                                                xMov_Destinazione.GetAttribute("appezza"),
                                                                                                                                xMov_Destinazione.GetAttribute("id_destinazione"),
                                                                                                                                Data_Movimento,
                                                                                                                                objParametri_Server)

                                                        End If

                                                    End If

                                                    i_DatiMov_Destinazioni += 1

                                                Loop

                                                i_DatiMovimento_Dettaglio += 1

                                            Loop

                                            i_DatiMovimenti_Dettagli += 1

                                        Loop

                                End Select

                                i_Movimento += 1

                            Loop

                            i_DatiMovimento += 1

                        Loop

                        i_Agenda += 1

                    Loop

                    i_DatiAgenda += 1

                Loop

            End If
            '------------------------------

            XmlDom.LoadXml(DatiVerifica)

            xDatiRisultati = XmlDom.GetElementsByTagName("DatiRisultati")

            i_DatiRisultati = 0

            '(06/09/2017 fede) utilizzo hashtable in caso i principi siano letti da ws x evitare rilettura ad ogni impianto
            Dim Hash_FormulatiPA As New Hashtable
            Dim Hash_FormulatiPAPesi As New Hashtable

            'Check di esistenza Informazioni
            Do While i_DatiRisultati < xDatiRisultati.Count

                xDatiRisultato = xDatiRisultati.Item(i_DatiRisultati)

                xDatiGenerali = xDatiRisultato.GetElementsByTagName("DatiGenerali")

                i_DatiGenerali = 0

                Do While i_DatiGenerali < xDatiGenerali.Count

                    xDatiGenerale = xDatiGenerali.Item(i_DatiGenerali)

                    'Impostazione Dati Generali
                    PIVA = xDatiGenerale.GetAttribute("piva")
                    Sa_Cod = CInt(xDatiGenerale.GetAttribute("sa_cod"))
                    Lav_Cod = CInt(xDatiGenerale.GetAttribute("lav_cod"))
                    Des_Lib = xDatiGenerale.GetAttribute("des_lib")
                    TipoTestata = CInt(xDatiGenerale.GetAttribute("tipotestata"))
                    Data = CDate(xDatiGenerale.GetAttribute("data_movimento"))
                    Disciplinare_Cod = CInt(xDatiGenerale.GetAttribute("disciplinare_cod"))
                    Disciplinare_Des = xDatiGenerale.GetAttribute("disciplinare_des")
                    Veg_Cod = CInt(xDatiGenerale.GetAttribute("veg_cod"))
                    Id_RcDpi = CInt(xDatiGenerale.GetAttribute("id_rcdpi"))
                    Id_RcDpi_Des = xDatiGenerale.GetAttribute("rcdpi_des")

                    If IsNumeric(xDatiGenerale.GetAttribute("superficie")) Then
                        Superficie = xDatiGenerale.GetAttribute("superficie")
                        If Superficie = 0 AndAlso Sup_Coinvolta <> 0 Then
                            Superficie = Sup_Coinvolta
                        End If
                    Else
                        Superficie = 0
                        If Sup_Coinvolta <> 0 Then
                            Superficie = Sup_Coinvolta
                        End If
                    End If

                    If xDatiGenerale.HasAttribute("appezzamenti") AndAlso xDatiGenerale.GetAttribute("appezzamenti") <> "" Then
                        strAppCoinvolti = xDatiGenerale.GetAttribute("appezzamenti")
                    End If

                    If xDatiGenerale.HasAttribute("flag_nuovo_controllo_riduzione_diserbo") AndAlso xDatiGenerale.GetAttribute("flag_nuovo_controllo_riduzione_diserbo") <> "" Then
                        flagNuovoControlloRiduzioneDiserbo = CBool(xDatiGenerale.GetAttribute("flag_nuovo_controllo_riduzione_diserbo"))
                    End If

                    Epoca_Des = Agro_SQL_SaveText(xDatiGenerale.GetAttribute("epoca_des"))

                    bConforme = True
                    ReDim Soglie(13, 0)

                    '=========================================================================
                    'Creazione Documento Risultato Finale
                    '-------------------------------------------------------------------------

                    XmlDatiRisultatiFinali = XmlDoc.CreateElement("DatiRisultati")

                    '----- < Dati Finali > ----
                    XmlDatiGeneraliFinali = XmlDoc.CreateElement("DatiGenerali")
                    XmlDatiNonConformiFinali = XmlDoc.CreateElement("DatiNonConformi")
                    XmlWarningFinali = XmlDoc.CreateElement("DatiWarnings")
                    XmlNoteFinali = XmlDoc.CreateElement("DatiNote")

                    XmlDatiGeneraliFinali.SetAttribute("piva", PIVA)
                    XmlDatiGeneraliFinali.SetAttribute("sa_cod", Sa_Cod)
                    XmlDatiGeneraliFinali.SetAttribute("data_movimento", CDate(Data).ToShortDateString)
                    XmlDatiGeneraliFinali.SetAttribute("lav_cod", Lav_Cod)
                    XmlDatiGeneraliFinali.SetAttribute("id_agenda", Id_Agenda_Escluso)
                    XmlDatiGeneraliFinali.SetAttribute("des_lib", Des_Lib)
                    XmlDatiGeneraliFinali.SetAttribute("id_rcdpi", Id_RcDpi)
                    XmlDatiGeneraliFinali.SetAttribute("rcdpi_des", Id_RcDpi_Des)
                    XmlDatiGeneraliFinali.SetAttribute("disciplinare_cod", Disciplinare_Cod)
                    XmlDatiGeneraliFinali.SetAttribute("disciplinare_des", Disciplinare_Des)
                    XmlDatiGeneraliFinali.SetAttribute("biologico", Biologico)
                    XmlDatiGeneraliFinali.SetAttribute("flag_nuovo_controllo_riduzione_diserbo", flagNuovoControlloRiduzioneDiserbo)

                    Select Case Disciplinare_Cod
                        Case -2
                            XmlDatiGeneraliFinali.SetAttribute("disciplinare_des", Descrizione_Regolamento_Bio)
                        Case > 0
                        Case Else
                            If Biologico Then
                                XmlDatiGeneraliFinali.SetAttribute("disciplinare_des", Descrizione_Regolamento_Bio)
                            Else
                                XmlDatiGeneraliFinali.SetAttribute("disciplinare_des", "Nessuno")
                            End If
                    End Select

                    XmlDatiGeneraliFinali.SetAttribute("tipotestata", TipoTestata)
                    XmlDatiGeneraliFinali.SetAttribute("veg_cod", Veg_Cod)
                    XmlDatiGeneraliFinali.SetAttribute("epoca_des", Epoca_Des)
                    XmlDatiGeneraliFinali.SetAttribute("superficie", Superficie)


                    If strAppCoinvolti = "" Then

                        '(13/10/2017 fede) aggiunto dettaglio app coinvolti
                        Dim objapp As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                        Dim dt_impianti As DataTable = objapp.Leggi_Dettagli_Impianti(PIVA, Id_Agenda_Escluso, "", "", objParametri_Server)
                        Dim Sup_Trattata As Decimal = 0
                        If dt_impianti IsNot Nothing Then
                            For dett = 0 To dt_impianti.Rows.Count - 1
                                '(15/01/2018 fede) aggiunta indicazione se l'appezzamento ha un vincolo più restrittivo di quello scelto per l'intervento
                                'se si è scelto il regolamento bio si possono trattare tutti gli impianti
                                'se si è scelto un dpi si evidenziano quelli bio
                                'se non si sono scelti disciplinari si evidenziano gli impianti con un dpi e quelli bio
                                Select Case Dpi_Verificato

                                    Case > 0 'dpi
                                        If dt_impianti.Rows(dett).Item("regolamento_cod") = 4 Then
                                            strAppCoinvolti &= dt_impianti.Rows(dett).Item("app_nome") & " (" & Gias.BIOVincoloPiuRestrittivo & ")" & ","
                                        Else
                                            strAppCoinvolti &= dt_impianti.Rows(dett).Item("app_nome") & ","
                                        End If

                                    Case "0"
                                        If Not Biologico Then
                                            If dt_impianti.Rows(dett).Item("regolamento_cod") = 4 Then
                                                strAppCoinvolti &= dt_impianti.Rows(dett).Item("app_nome") & " (" & Gias.BIOVincoloPiuRestrittivo & ")" & ","
                                            Else
                                                If dt_impianti.Rows(dett).Item("Disciplinare_Cod") <> 0 Then
                                                    strAppCoinvolti &= dt_impianti.Rows(dett).Item("app_nome") & " (" & Gias.DPIVincoloPiuRestrittivo & ")" & ","
                                                Else
                                                    strAppCoinvolti &= dt_impianti.Rows(dett).Item("app_nome") & ","
                                                End If
                                            End If
                                        Else
                                            strAppCoinvolti &= dt_impianti.Rows(dett).Item("app_nome") & ","
                                        End If

                                    Case Else
                                        strAppCoinvolti &= dt_impianti.Rows(dett).Item("app_nome") & ","

                                End Select

                                If Sup_Coinvolta = 0 Then
                                    Sup_Trattata += dt_impianti.Rows(dett).Item("qta2")
                                End If

                            Next

                        End If

                        If strAppCoinvolti <> "" Then
                            strAppCoinvolti = Left(strAppCoinvolti, strAppCoinvolti.Length - 1)
                        End If
                        If Sup_Coinvolta = 0 Then
                            Sup_Coinvolta = Sup_Trattata
                        End If

                        XmlDatiGeneraliFinali.SetAttribute("superficie", Sup_Coinvolta)

                    End If

                    XmlDatiGeneraliFinali.SetAttribute("appezzamenti", strAppCoinvolti)


                    '=============================================================================
                    'Ricerca Dati Non Conformi
                    '-----------------------------------------------------------------------------
                    xDatiNonConformi = xDatiGenerale.GetElementsByTagName("DatiNonConformi")

                    i_DatiNonConformi = 0

                    'Ne esiste solamente 1
                    Do While i_DatiNonConformi < xDatiNonConformi.Count

                        xDatoNonConforme = xDatiNonConformi.Item(i_DatiNonConformi)

                        xNonConformi = xDatoNonConforme.GetElementsByTagName("DatoNonConforme")

                        i_DatoNonConforme = 0

                        Do While i_DatoNonConforme < xNonConformi.Count


                            xNonConforme = xNonConformi.Item(i_DatoNonConforme)

                            InserisciErrore = True

                            Err_Code = CInt(xNonConforme.GetAttribute("err_code"))
                            Err_Des = CStr(xNonConforme.GetAttribute("err_des"))

                            Select Case Lav_Cod

                                Case LAVCOD_RACCOLTA

                                    Select Case impostazioni.UTENTE_COD_BLOCCA_RACCOLTA_CARENZA_NON_RISPETTATA
                                        Case "0"  'nessun blocco
                                            InserisciErrore = False
                                    End Select

                                Case LAVCOD_DISTRIBUZIONE_CONCIME,
                                     LAVCOD_SARCHIATURA_CONCIMAZIONE,
                                     LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                                     LAVCOD_CONCIMAZIONE_FOGLIARE,
                                     LAVCOD_FERTIRRIGAZIONE,
                                     LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                                    Select Case impostazioni.UTENTE_COD_DEFAULT_BLOCCO_SALVA_FERTILIZZAZIONI

                                        Case "0" 'nessun blocco
                                            InserisciErrore = False
                                        Case "1", "2" 'blocco, warning (verifica singole impostazioni)
                                            Select Case Err_Code
                                                Case enTipoErrCode_Verifica.ProdottoNonBiologico
                                                    If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_NON_BIO_REGOLAMENTO_BIO_FERTI <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.Superato_N_Max,
                                                     enTipoErrCode_Verifica.Superato_P_Max,
                                                     enTipoErrCode_Verifica.Superato_K_Max,
                                                     enTipoErrCode_Verifica.Superato_M_Max
                                                    If impostazioni.UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.Superato_N_Max_Intervento
                                                    If impostazioni.UTENTE_COD_BLOCCA_MASSIMALE_DISTRIBUZIONE_N_DPI_FERTI <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.DoseEccessivaRame
                                                    If impostazioni.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_FERTI <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.DoseEccessivaRame5Anni
                                                    If impostazioni.UTENTE_COD_BLOCCA_DOSERAME7ANNIMAX_FERTI <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                            End Select

                                    End Select

                                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, 'trattamenti
                                     LAVCOD_DISERBO,
                                     LAVCOD_DISSECCAMENTO,
                                     LAVCOD_GEODISINFESTAZIONE,
                                     LAVCOD_CONCIA_SEME,
                                     LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                                     LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                                     LAVCOD_TRATTAMENTO_POST_RACCOLTA


                                    '(25/07/2016 fede) introdotto controllo impostazione utente per ignorare i controlli
                                    Select Case impostazioni.UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME

                                        Case "0" ', "ND" 'nessun blocco
                                            InserisciErrore = False

                                        Case "1", "2" 'blocco, warning (verifica singole impostazioni)

                                            Select Case Err_Code

                                                Case enTipoErrCode_Verifica.AvversitaNonGiustificataDPI
                                                    If impostazioni.UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.ProdottoNonGiustificatoSuAvversita
                                                    If impostazioni.UTENTE_COD_BLOCCA_PRODOTTONOAVVERSITA_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.DoseNonDisponibile
                                                    If impostazioni.UTENTE_COD_BLOCCA_DOSENONDISPONIBILE_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.UnitaMisuraNonCompatibile
                                                    If impostazioni.UTENTE_COD_BLOCCA_DOSEUDMNONCONFORME_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.DoseEccessiva
                                                    If impostazioni.UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.DoseInsufficiente
                                                    If impostazioni.UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.AcquaNonCorretta
                                                    'acqua superiore
                                                    If InStr(Err_Des, "superiore") <> 0 Then
                                                        If impostazioni.UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA_SALVATAGGIO <> "1" Then
                                                            InserisciErrore = False
                                                        End If
                                                    End If
                                                    'acqua inferiore
                                                    If InStr(Err_Des, "inferiore") <> 0 Then
                                                        If impostazioni.UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA_SALVATAGGIO <> "1" Then
                                                            InserisciErrore = False
                                                        End If
                                                    End If

                                                Case enTipoErrCode_Verifica.DataInterventoMin
                                                    If impostazioni.UTENTE_COD_BLOCCA_DATADPIMIN_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If

                                                Case enTipoErrCode_Verifica.DataInterventoMax
                                                    If impostazioni.UTENTE_COD_BLOCCA_DATADPIMAX_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If

                                                Case enTipoErrCode_Verifica.ImpiantiNonCoerentiDPI,
                                                        enTipoErrCode_Verifica.ImpiantiRaggruppamentoColturaleNonOmogeneo,
                                                        enTipoErrCode_Verifica.ImpiantiCoperturaNonOmogeneo,
                                                        enTipoErrCode_Verifica.ImpiantiDPINonImpostato,
                                                        enTipoErrCode_Verifica.ImpiantiDpiNonOmogeneo
                                                    If impostazioni.UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If

                                                Case enTipoErrCode_Verifica.DoseDiserboEccessiva
                                                    If impostazioni.UTENTE_COD_BLOCCA_DOSEDPIDISERBOMAX_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If

                                        'Case enTipoErrCode_Verifica.Superato_N_Max
                                        '    If UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO <> "1" Then
                                        '        InserisciErrore = False
                                        '    End If

                                        'Case enTipoErrCode_Verifica.Superato_P_Max
                                        '    If UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO <> "1" Then
                                        '        InserisciErrore = False
                                        '    End If

                                        'Case enTipoErrCode_Verifica.Superato_K_Max
                                        '    If UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO <> "1" Then
                                        '        InserisciErrore = False
                                        '    End If

                                        'Case enTipoErrCode_Verifica.Superato_M_Max
                                        '    If UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO <> "1" Then
                                        '        InserisciErrore = False
                                        '    End If

                                                Case enTipoErrCode_Verifica.CarenzaNonRispettata
                                                    If impostazioni.UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If

                                                Case enTipoErrCode_Verifica.SuperatoVolumeMaxAcquaDpi
                                                    If impostazioni.UTENTE_COD_BLOCCA_ACQUAMASSIMA_DPI_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If

                                                Case enTipoErrCode_Verifica.Epoca_Etichetta
                                                    If impostazioni.UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If

                                                Case enTipoErrCode_Verifica.BufferZoneNonRispettata
                                                    If impostazioni.UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If

                                                Case enTipoErrCode_Verifica.DoseEccessivaEtichetta_Anno
                                                    If impostazioni.UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If

                                                Case enTipoErrCode_Verifica.DoseEccessivaEtichettaAvversita_Anno
                                                    If impostazioni.UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If

                                                Case enTipoErrCode_Verifica.ProdottoVincolatoFormulato
                                                    If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_RELAZIONE_FORMULATO_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If

                                                Case enTipoErrCode_Verifica.ProdottoNonUtilizzabileXDisciplinarexStatoImpianto
                                                    If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_ETA_IMPIANTO_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If

                                                Case enTipoErrCode_Verifica.MixPolveruentiENon
                                                    If impostazioni.UTENTE_COD_BLOCCA_MIX_POLVERULENTI_NONPOLVERULENTI_SALVATAGGIO <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.ProdottoNonUtilizzabileXData
                                                    If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_NON_UTILIZZABILE_X_DATA <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.ProdottoNonRegistratoSuColtura
                                                    If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_NON_REGISTRATO_SU_COLTURA <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.AvversitaNonTrattabileDPI
                                                    If impostazioni.UTENTE_COD_BLOCCA_AVVERSITA_NON_TRATTABILE_DPI <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.ProdottoNonGiustificatoSuAvversitaDPI
                                                    If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_NON_REGISTRATO_SU_AVVERSITA_DPI <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.ProdottoNonUtilizzabileXDisciplinare
                                                    If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_NON_UTILIZZABILE_DPI <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.InterventoNonConsentito
                                                    If impostazioni.UTENTE_COD_BLOCCA_INTERVENTO_NON_CONSENTITO <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                                Case enTipoErrCode_Verifica.ProdottoNonBiologico
                                                    If impostazioni.UTENTE_COD_BLOCCA_PRODOTTO_NON_BIO_REGOLAMENTO_BIO_FITO <> "1" Then
                                                        InserisciErrore = False
                                                    End If
                                            End Select


                                    End Select

                            End Select

                            If paramVerificaDPIMultiAttivita IsNot Nothing Then
                                If paramVerificaDPIMultiAttivita.dic_ProdottiRealixLavCod IsNot Nothing Then
                                    Dim pro_cod_errore As Integer = xNonConforme.GetAttribute("pro_cod")
                                    If paramVerificaDPIMultiAttivita.dic_ProdottiRealixLavCod.ContainsKey(Lav_Cod) Then
                                        If Not (paramVerificaDPIMultiAttivita.dic_ProdottiRealixLavCod.Item(Lav_Cod)).Contains(pro_cod_errore) Then
                                            InserisciErrore = False
                                        End If
                                    End If
                                End If
                            End If

                            If InserisciErrore Then
                                'Inserisco la non conformità nel risultato finale
                                XML_Nodo = XmlDoc.ImportNode(xNonConforme, True)
                                XmlDatiNonConformiFinali.AppendChild(XML_Nodo)
                                InterventoConforme = False
                            End If

                            i_DatoNonConforme = i_DatoNonConforme + 1

                        Loop

                        i_DatiNonConformi = i_DatiNonConformi + 1

                    Loop


                    '=============================================================================
                    'Ricerca Warnings
                    '-----------------------------------------------------------------------------
                    xDatiWarnings = xDatiGenerale.GetElementsByTagName("DatiWarnings")

                    i_DatiWarnings = 0

                    '(10/08/2020 fede nuovi controlli tutti gli app assieme)
                    Dim HashWarning As New Hashtable
                    Dim HashWarning_Intervallo_Trattamenti_xProdotto As New Hashtable
                    Dim HashWarning_Numero_Interventi_xProdotto As New Hashtable
                    Dim HashWarning_Numero_Interventi As New Hashtable
                    Dim HashWarning_Numero_Interventi_Min As New Hashtable
                    Dim HashWarning_Epoche As New Hashtable
                    Dim HashWarning_DoseDPI_MaxAnno As New Hashtable
                    Dim HashWarning_DoseRame_Anno_xBio As New Hashtable
                    Dim HashWarning_DoseRame_5Anni_xBio As New Hashtable
                    Dim HashWarning_Soglie As New Hashtable

                    Dim VerificaWarning As Boolean = False

                    'Ne esiste solamente 1
                    Do While i_DatiWarnings < xDatiWarnings.Count

                        xDatoWarning = xDatiWarnings.Item(i_DatiWarnings)

                        xWarnings = xDatoWarning.GetElementsByTagName("Warning")

                        i_DatoWarning = 0

                        'Loop sui Warning
                        Do While i_DatoWarning < xWarnings.Count

                            xWarning = xWarnings.Item(i_DatoWarning)


                            '###########################################################################################
                            '############ VERIFICA WARNING SUL DATABASE GIAS_SERVER ####################################
                            '###########################################################################################
                            N_Warning = CInt(xWarning.GetAttribute("n_warning"))
                            Warning_Code = CInt(xWarning.GetAttribute("warning_code"))

                            VerificaWarning = False

                            '(10/08/2020 fede nuovi controlli tutti gli app assieme)
                            ' in base al tipo di warning verifico se l'ho già controllato
                            Select Case Warning_Code

                                Case enTipoWarning_Verifica.Intervallo_Trattamenti_xProdotto
                                    Pro_Cod = CInt(xWarning.GetAttribute("pro_cod"))
                                    IntervalloTrattamenti_Min = 0
                                    If xWarning.HasAttribute("intervallotrattamenti_prodotto_min") Then
                                        IntervalloTrattamenti_Min = CInt(xWarning.GetAttribute("intervallotrattamenti_prodotto_min"))
                                    End If
                                    If Not HashWarning_Intervallo_Trattamenti_xProdotto.ContainsKey(Pro_Cod & "|" & IntervalloTrattamenti_Min) Then
                                        VerificaWarning = True
                                    End If

                                Case enTipoWarning_Verifica.Numero_Interventi_xProdotto
                                    Pro_Cod = CInt(xWarning.GetAttribute("pro_cod"))
                                    Max_Interventi_xProdotto = 0
                                    If xWarning.HasAttribute("interventi_consentiti_prodotto") Then
                                        Max_Interventi_xProdotto = CInt(xWarning.GetAttribute("interventi_consentiti_prodotto"))
                                    End If
                                    If xWarning.HasAttribute("interventi_consentiti_prodotto_globali") AndAlso IsNumeric(xWarning.GetAttribute("interventi_consentiti_prodotto_globali")) Then
                                        Max_Interventi_xProdotto_Globali = CInt(xWarning.GetAttribute("interventi_consentiti_prodotto_globali"))
                                    Else
                                        Max_Interventi_xProdotto_Globali = 0
                                    End If
                                    If Not HashWarning_Numero_Interventi_xProdotto.ContainsKey(Pro_Cod & "|" & Max_Interventi_xProdotto & "|" & Max_Interventi_xProdotto_Globali) Then
                                        VerificaWarning = True
                                    End If

                                Case enTipoWarning_Verifica.Numero_Interventi
                                    Max_Interventi = CInt(xWarning.GetAttribute("interventi_consentiti"))
                                    strPA = xWarning.GetAttribute("strpa")
                                    strAvversita = xWarning.GetAttribute("avversita_des")
                                    If xWarning.HasAttribute("gruppi_avversita_des") Then
                                        strGruppiAvversita = xWarning.GetAttribute("gruppi_avversita_des")
                                    Else
                                        strGruppiAvversita = ""
                                    End If

                                    'Impostazione Variabili Limitazione Uso
                                    If xWarning.HasAttribute("collegamento_formulati_lu") Then
                                        Collegamento_Formulati_LU = xWarning.GetAttribute("collegamento_formulati_lu")
                                    Else
                                        Collegamento_Formulati_LU = 0
                                    End If
                                    If xWarning.HasAttribute("elenco_fr_cod_lu") Then
                                        Elenco_Fr_Cod_LU = xWarning.GetAttribute("elenco_fr_cod_lu")
                                    Else
                                        Elenco_Fr_Cod_LU = ""
                                    End If

                                    If Not HashWarning_Numero_Interventi.ContainsKey(Max_Interventi & "|" & strPA & "|" & strAvversita & "|" & strGruppiAvversita) Then
                                        VerificaWarning = True
                                    End If

                                Case enTipoWarning_Verifica.Numero_Interventi_Min
                                    If xWarning.HasAttribute("interventi_consentiti_minimo") Then
                                        Min_Interventi = CInt(xWarning.GetAttribute("interventi_consentiti_minimo"))
                                    Else
                                        Min_Interventi = 0
                                    End If
                                    strPA = xWarning.GetAttribute("strpa")
                                    strAvversita = xWarning.GetAttribute("avversita_des")
                                    If xWarning.HasAttribute("gruppi_avversita_des") Then
                                        strGruppiAvversita = xWarning.GetAttribute("gruppi_avversita_des")
                                    Else
                                        strGruppiAvversita = ""
                                    End If
                                    If Not HashWarning_Numero_Interventi_Min.ContainsKey(Min_Interventi & "|" & strPA & "|" & strAvversita & "|" & strGruppiAvversita) Then
                                        VerificaWarning = True
                                    End If

                                Case enTipoWarning_Verifica.DoseRame_Anno_xBio
                                    strPA = xWarning.GetAttribute("strpa")
                                    If Not HashWarning_DoseRame_Anno_xBio.ContainsKey(strPA) Then
                                        VerificaWarning = True
                                    End If


                                Case enTipoWarning_Verifica.Dose_MaxAnno_GPAI

                                    If xWarning.HasAttribute("dosedpi_maxanno") AndAlso IsNumeric(xWarning.GetAttribute("dosedpi_maxanno")) Then
                                        DoseDpi_MaxAnno = CDbl(xWarning.GetAttribute("dosedpi_maxanno"))
                                    Else
                                        DoseDpi_MaxAnno = 0
                                    End If

                                    If xWarning.HasAttribute("udm_cod") AndAlso IsNumeric(xWarning.GetAttribute("udm_cod")) Then
                                        DoseDpi_MaxAnno_Udm_Cod = CInt(xWarning.GetAttribute("udm_cod"))
                                    Else
                                        DoseDpi_MaxAnno_Udm_Cod = 0
                                    End If

                                    If DoseDpi_MaxAnno <> 0 AndAlso DoseDpi_MaxAnno_Udm_Cod <> 0 Then
                                        VerificaWarning = True
                                    End If

                                Case enTipoWarning_Verifica.Dose_MaxAnno

                                    If xWarning.HasAttribute("doseetichetta_maxanno") AndAlso IsNumeric(xWarning.GetAttribute("doseetichetta_maxanno")) Then
                                        DoseEtichetta_MaxAnno = CDbl(xWarning.GetAttribute("doseetichetta_maxanno"))
                                    Else
                                        DoseEtichetta_MaxAnno = 0
                                    End If

                                    If xWarning.HasAttribute("interventi_consentiti_prodotto_udm") AndAlso IsNumeric(xWarning.GetAttribute("interventi_consentiti_prodotto_udm")) Then
                                        DoseEtichetta_MaxAnno_Udm_Cod = CInt(xWarning.GetAttribute("interventi_consentiti_prodotto_udm"))
                                    Else
                                        DoseEtichetta_MaxAnno_Udm_Cod = 0
                                    End If

                                    If DoseEtichetta_MaxAnno <> 0 AndAlso DoseEtichetta_MaxAnno_Udm_Cod <> 0 Then
                                        VerificaWarning = True
                                    End If


                                Case enTipoWarning_Verifica.Dose_MaxAnno_xAvversita_Infestante

                                    If xWarning.HasAttribute("doseetichetta_maxanno") AndAlso IsNumeric(xWarning.GetAttribute("doseetichetta_maxanno")) Then
                                        DoseEtichetta_MaxAnno = CDbl(xWarning.GetAttribute("doseetichetta_maxanno"))
                                    Else
                                        DoseEtichetta_MaxAnno = 0
                                    End If

                                    If xWarning.HasAttribute("interventi_consentiti_prodotto_udm") AndAlso IsNumeric(xWarning.GetAttribute("interventi_consentiti_prodotto_udm")) Then
                                        DoseEtichetta_MaxAnno_Udm_Cod = CInt(xWarning.GetAttribute("interventi_consentiti_prodotto_udm"))
                                    Else
                                        DoseEtichetta_MaxAnno_Udm_Cod = 0
                                    End If

                                    Av_Cod = xWarning.GetAttribute("av_cod")
                                    Av_Gru = xWarning.GetAttribute("av_gru")

                                    If DoseEtichetta_MaxAnno <> 0 AndAlso DoseEtichetta_MaxAnno_Udm_Cod <> 0 AndAlso (Av_Cod > 0 OrElse Av_Gru > 0) Then
                                        VerificaWarning = True
                                    End If


                                Case Else
                                    VerificaWarning = True

                            End Select


                            '(28/09/2018 fede) se la stessa anomalia è andata già in errore non proseguo la verifica
                            If Not HashErrori.ContainsKey(N_Warning & "|" & Warning_Code) AndAlso VerificaWarning Then


                                Warning_Des = xWarning.GetAttribute("warning_des")
                                If xWarning.HasAttribute("piva") Then
                                    PIVA = CStr(xWarning.GetAttribute("piva"))
                                End If
                                If xWarning.HasAttribute("sa_cod") Then
                                    Sa_Cod = CInt(xWarning.GetAttribute("sa_cod"))
                                End If
                                Appezza = CInt(xWarning.GetAttribute("appezza"))
                                Id_Reg = CInt(xWarning.GetAttribute("id_reg"))
                                Lotto = xWarning.GetAttribute("lotto")
                                Pro_Cod = CInt(xWarning.GetAttribute("pro_cod"))
                                Pro_Des = xWarning.GetAttribute("pro_des")
                                Pa_Cod = CInt(xWarning.GetAttribute("pa_cod"))
                                Pa_Des = xWarning.GetAttribute("pa_des")
                                Da_Ep_Cod = CInt(xWarning.GetAttribute("da_ep_cod"))
                                Da_Ep_Des = xWarning.GetAttribute("da_ep_des")
                                A_Ep_Cod = CInt(xWarning.GetAttribute("a_ep_cod"))
                                A_Ep_Des = xWarning.GetAttribute("a_ep_des")
                                Offset = CInt(xWarning.GetAttribute("offset"))
                                strPA = xWarning.GetAttribute("strpa")
                                strAvversita = xWarning.GetAttribute("avversita_des")
                                '(11/10/2018 fede) aggiunti gruppi per controlli dpi-limitazioni diserbo
                                If xWarning.HasAttribute("gruppi_avversita_des") Then
                                    strGruppiAvversita = xWarning.GetAttribute("gruppi_avversita_des")
                                End If
                                Max_Interventi = CInt(xWarning.GetAttribute("interventi_consentiti"))
                                Interventi_Scheda = CInt(xWarning.GetAttribute("interventi_scheda"))
                                If xWarning.HasAttribute("interventi_consentiti_prodotto") Then
                                    Max_Interventi_xProdotto = CInt(xWarning.GetAttribute("interventi_consentiti_prodotto"))
                                End If
                                If xWarning.HasAttribute("interventi_consentiti_prodotto_udm") Then
                                    Max_Interventi_xProdotto_Udm = CInt(xWarning.GetAttribute("interventi_consentiti_prodotto_udm"))
                                End If
                                If xWarning.HasAttribute("intervallotrattamenti_prodotto_min") Then
                                    IntervalloTrattamenti_Min = CInt(xWarning.GetAttribute("intervallotrattamenti_prodotto_min"))
                                End If
                                If xWarning.HasAttribute("intervallotrattamenti_prodotto_max") Then
                                    IntervalloTrattamenti_Max = CInt(xWarning.GetAttribute("intervallotrattamenti_prodotto_max"))
                                End If
                                Lav_Cod_Soglia = CInt(xWarning.GetAttribute("lav_cod"))
                                Udm_Cod_Soglia = CInt(xWarning.GetAttribute("udm_cod"))
                                Qta_Soglia = CDec(xWarning.GetAttribute("soglia"))
                                Av_Cod = CInt(xWarning.GetAttribute("av_cod"))
                                Note = xWarning.GetAttribute("note")
                                Validita_Inizio = If(IsDate(xWarning.GetAttribute("validita_inizio")), xWarning.GetAttribute("validita_inizio"), AGRODATAINIZIO)
                                Validita_Fine = If(IsDate(xWarning.GetAttribute("validita_fine")), xWarning.GetAttribute("validita_fine"), AGRODATAFINE)

                                If xWarning.HasAttribute("sup_imp") Then
                                    Sup_Imp = CDec(xWarning.GetAttribute("sup_imp"))
                                Else
                                    Sup_Imp = 0
                                End If
                                If xWarning.HasAttribute("titolo") Then
                                    Titolo = CDec(xWarning.GetAttribute("titolo"))
                                Else
                                    Titolo = 0
                                End If
                                If xWarning.HasAttribute("peso") Then
                                    Peso = CDec(xWarning.GetAttribute("peso"))
                                Else
                                    Peso = 0
                                End If

                                If xWarning.HasAttribute("dosedpi_maxanno") Then
                                    DoseDpi_MaxAnno = CDec(xWarning.GetAttribute("dosedpi_maxanno"))
                                Else
                                    DoseDpi_MaxAnno = 0
                                End If

                                If xWarning.HasAttribute("dosedpi_maxanno_perc") Then
                                    DoseDpi_MaxAnno_Perc = CDec(xWarning.GetAttribute("dosedpi_maxanno_perc"))
                                Else
                                    DoseDpi_MaxAnno_Perc = 0
                                End If
                                If xWarning.HasAttribute("dosedpi_maxanno_peso") Then
                                    DoseDpi_MaxAnno_Peso = CDec(xWarning.GetAttribute("dosedpi_maxanno_peso"))
                                Else
                                    DoseDpi_MaxAnno_Peso = 0
                                End If



                                If xWarning.HasAttribute("interventi_consentiti_minimo") Then
                                    Min_Interventi = CInt(xWarning.GetAttribute("interventi_consentiti_minimo"))
                                End If

                                If xWarning.HasAttribute("interventi_consentiti_prodotto_globali") AndAlso IsNumeric(xWarning.GetAttribute("interventi_consentiti_prodotto_globali")) Then
                                    Max_Interventi_xProdotto_Globali = CInt(xWarning.GetAttribute("interventi_consentiti_prodotto_globali"))
                                Else
                                    Max_Interventi_xProdotto_Globali = 0
                                End If

                                If xWarning.HasAttribute("for_veg_av_dos_cod") AndAlso IsNumeric(xWarning.GetAttribute("for_veg_av_dos_cod")) Then
                                    For_Veg_Av_Dos_Cod = CInt(xWarning.GetAttribute("for_veg_av_dos_cod"))
                                Else
                                    For_Veg_Av_Dos_Cod = 0
                                End If

                                If xWarning.HasAttribute("gruppo_dosaggi") AndAlso IsNumeric(xWarning.GetAttribute("gruppo_dosaggi")) Then
                                    Gruppo_Dosaggi = CInt(xWarning.GetAttribute("gruppo_dosaggi"))
                                Else
                                    Gruppo_Dosaggi = 0
                                End If


                                Err_Code = 0
                                Err_Des = ""

                                InserisciWarning = True


                                '(25/07/2016 fede) introdotto controllo impostazione utente per ignorare i controlli
                                Select Case impostazioni.UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME

                                    Case "0" 'nessun blocco
                                        InserisciWarning = False

                                    Case "1", "2", "ND" 'blocco, warning (verifica singole impostazioni)

                                        Select Case Warning_Code

                                            Case enTipoWarning_Verifica.Epoche

                                                If impostazioni.UTENTE_COD_BLOCCA_EPOCADPINONCONFORME_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else
                                                    VerificaEpoche(
                                                                Err_Code,
                                                                Err_Des,
                                                                PIVA,
                                                                Sa_Cod,
                                                                Appezza,
                                                                Id_Reg,
                                                                Lotto,
                                                                Da_Ep_Cod,
                                                                Da_Ep_Des,
                                                                A_Ep_Cod,
                                                                A_Ep_Des,
                                                                Offset,
                                                                Data,
                                                                Pro_Des,
                                                                Validita_Inizio,
                                                                Validita_Fine,
                                                                objParametri_Server)
                                                End If

                                            Case enTipoWarning_Verifica.Numero_Interventi  'Verifica Numero Interventi x principi attivi da dpi

                                                If impostazioni.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_DPI_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    Interventi_Scheda_Old = Interventi_Scheda

                                                    Dim FiltroAvCod As String = ""
                                                    Dim FiltroAvGru As String = ""
                                                    Select Case TipoTestata
                                                        Case enum_Disciplinare_Tipo_Testata.Difesa
                                                            FiltroAvCod = strAvversita
                                                            FiltroAvGru = ""
                                                        Case enum_Disciplinare_Tipo_Testata.Diserbo
                                                            FiltroAvCod = ""
                                                            FiltroAvGru = strGruppiAvversita
                                                    End Select

                                                    '(10/08/2020 fede nuovi controlli tutti gli app assieme)
                                                    'N.B. modificato calcolo sostanze x prodotti miscele
                                                    'd'ora in poi se un prodotto è miscela di 2 sostanze viene conteggiato solo 1 volta
                                                    Dim ObjContabDAL As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                                                    Dim dt_Movimenti As DataTable
                                                    Dim Str_FiltroAggiuntivo As New StringBuilder

                                                    Dim FiltroPa As String = ""
                                                    Dim strPaNew As String = strPA
                                                    strPaNew = Replace(strPaNew, "(", "").Trim
                                                    strPaNew = Replace(strPaNew, ")", "").Trim
                                                    Dim Pa As String() = Split(strPaNew, ",")
                                                    If Pa IsNot Nothing Then
                                                        For Each principio In Pa
                                                            Dim PrincipiDPI() = Split(principio.Trim, "+") ' necessario per caso miscele dpi
                                                            For pdpi = 0 To PrincipiDPI.Length - 1
                                                                FiltroPa += "(Movimenti_dettagli.PrincipiAttivi Like '" & PrincipiDPI(pdpi).Trim & "§%' OR Movimenti_dettagli.PrincipiAttivi like '%|" & PrincipiDPI(pdpi).Trim & "§%') OR "
                                                            Next
                                                        Next
                                                    End If
                                                    If FiltroPa <> "" Then
                                                        FiltroPa = "  (" & Left(FiltroPa, FiltroPa.Length - 3) & ")"
                                                        Str_FiltroAggiuntivo.Append(FiltroPa)
                                                    End If

                                                    '======================================================================================================
                                                    'Inserimento Filtro Limitazione Uso
                                                    '------------------------------------------------------------------------------------------------------
                                                    Select Case Collegamento_Formulati_LU

                                                        Case 1 'Includi

                                                            If Str_FiltroAggiuntivo.Length > 0 Then
                                                                Str_FiltroAggiuntivo.Append(" AND ")
                                                            End If
                                                            Str_FiltroAggiuntivo.Append(" Movimenti_Dettagli.Pro_Cod In " & Elenco_Fr_Cod_LU & " ")

                                                        Case 2 'Escludi

                                                            If Str_FiltroAggiuntivo.Length > 0 Then
                                                                Str_FiltroAggiuntivo.Append(" AND ")
                                                            End If
                                                            Str_FiltroAggiuntivo.Append(" Movimenti_Dettagli.Pro_Cod Not In " & Elenco_Fr_Cod_LU & " ")

                                                        Case Else 'Bypass

                                                    End Select
                                                    '======================================================================================================

                                                    'se sono in inserimento o modifica dell'operazione vado in join con gli il filtro impianti temporaneo (potrebbero essere cambiati in modifica)
                                                    'se sono nel controllore a posteriori vado in join con le destinazioni dell'operazione corrente
                                                    If IDTestataTemp > 0 Then
                                                        'se sono in modifica devo escludere l'id_agenda già salvato dal conteggio (lo aggiungo dopo)
                                                        If Id_Agenda_Escluso <> 0 Then
                                                            If Str_FiltroAggiuntivo.Length > 0 Then
                                                                Str_FiltroAggiuntivo.Append(" And Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " ")
                                                            Else
                                                                Str_FiltroAggiuntivo.Append(" Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " ")
                                                            End If
                                                        End If
                                                        dt_Movimenti = ObjContabDAL.NumeroProdottiImpianti_suAvversita(PIVA, 0, IDTestataTemp, FiltroAvCod, FiltroAvGru, Str_FiltroAggiuntivo.ToString, "", objParametri_Server)
                                                    Else
                                                        'se sono in verifica a posteriori conteggio nella query anche l'operazione corrente
                                                        dt_Movimenti = ObjContabDAL.NumeroProdottiImpianti_suAvversita(PIVA, Id_Agenda_Escluso, 0, FiltroAvCod, FiltroAvGru, Str_FiltroAggiuntivo.ToString, "", objParametri_Server)
                                                    End If

                                                    Dim NTot As Integer = 0

                                                    'se sono in inserimento o modifica dell'operazione
                                                    'conteggio le sostanze dell'operazione corrente
                                                    Dim HashProdTmp As New Hashtable

                                                    For Each key In HashPrincipiOperazione.Keys
                                                        For Each principio In Pa
                                                            Dim ProCodTmp As Integer = Split(key, "_")(0)
                                                            Dim PaCodTmp As Integer = Split(key, "_")(1)
                                                            Dim PrincipiDPI() = Split(principio.Trim, "+") ' necessario per caso miscele dpi
                                                            For pdpi = 0 To PrincipiDPI.Length - 1
                                                                If Not HashProdTmp.ContainsKey(ProCodTmp) AndAlso PaCodTmp = PrincipiDPI(pdpi).Trim Then

                                                                    '================================================================================
                                                                    'Controllo Limitazione Uso
                                                                    '--------------------------------------------------------------------------------
                                                                    Dim HashProdTmp_LU As New Hashtable 'Prodotti da Limitazioni Uso
                                                                    Dim bOk_LU As Boolean = True
                                                                    Dim arrayLU As String()
                                                                    Dim i_LU As Integer = 0

                                                                    If Trim(Elenco_Fr_Cod_LU) <> "" Then

                                                                        'Formattazione
                                                                        Elenco_Fr_Cod_LU = Replace(Elenco_Fr_Cod_LU, "(", ",")
                                                                        Elenco_Fr_Cod_LU = Replace(Elenco_Fr_Cod_LU, ")", ",")

                                                                        arrayLU = Split(Elenco_Fr_Cod_LU & ",", ",")
                                                                        bOk_LU = False

                                                                        Select Case Collegamento_Formulati_LU

                                                                            Case 1 'Includi

                                                                                bOk_LU = False
                                                                                For i_LU = 0 To UBound(arrayLU) - 1

                                                                                    If IsNumeric(arrayLU(i_LU)) Then

                                                                                        If CInt(arrayLU(i_LU)) = ProCodTmp Then
                                                                                            bOk_LU = True
                                                                                            Exit For
                                                                                        End If

                                                                                    End If

                                                                                Next

                                                                            Case 2 'Escludi

                                                                                bOk_LU = True
                                                                                For i_LU = 0 To UBound(arrayLU) - 1

                                                                                    If IsNumeric(arrayLU(i_LU)) Then

                                                                                        If CInt(arrayLU(i_LU)) = ProCodTmp Then
                                                                                            bOk_LU = False
                                                                                            Exit For
                                                                                        End If

                                                                                    End If

                                                                                Next

                                                                            Case Else 'Bypass

                                                                                bOk_LU = True

                                                                        End Select

                                                                    End If
                                                                    '================================================================================

                                                                    If bOk_LU Then
                                                                        HashProdTmp.Add(ProCodTmp, "")
                                                                    End If

                                                                End If

                                                            Next
                                                        Next
                                                    Next

                                                    Dim N_Prodotti_OperazioneCorrente As Integer = HashProdTmp.Count

                                                    Dim app_non_conformi As String = ""
                                                    Dim Hash_AppNonConformi As New Hashtable

                                                    If dt_Movimenti.Rows.Count > 0 Then

                                                        For Each drTmp As DataRow In dt_Movimenti.Rows
                                                            NTot = drTmp("N_Prodotti")
                                                            Interventi_Scheda = NTot + N_Prodotti_OperazioneCorrente

                                                            'Marco: Nota. La non conformità viene data solo se il prodotto utilizzato fa parte dei controlli. 
                                                            'Per esempio se utilizzo un prodotto consentito, ma altri interventi con altri prodotti sullo stesso impianti non lo sono --> bypass
                                                            If Interventi_Scheda > Max_Interventi AndAlso (HashProdTmp.Count > 0 OrElse HashPrincipiOperazione.Keys.Count = 0) Then
                                                                'Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "(" & Interventi_Scheda & ")")
                                                                Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "")
                                                                Err_Code = enTipoErrCode_Verifica.SuperatoNumMaxInterventiPA
                                                                Err_Des = Warning_Des & String.Format(Gias.ConsentitiMassimoNInterventiAnno, Max_Interventi)
                                                                Consultazione = String.Format(Gias.MassimoNINterventiAnno, Max_Interventi)
                                                                If Not Hash_AppNonConformi.ContainsKey(Interventi_Scheda) Then
                                                                    Hash_AppNonConformi.Add(Interventi_Scheda, drTmp("app_nome"))
                                                                Else
                                                                    Hash_AppNonConformi(Interventi_Scheda) = Hash_AppNonConformi(Interventi_Scheda) & ", " & drTmp("app_nome")
                                                                End If
                                                                'Exit For
                                                            End If
                                                        Next
                                                        If Hash_AppNonConformi.Count > 0 Then
                                                            For Each key In Hash_AppNonConformi.Keys
                                                                app_non_conformi &= String.Format(Gias.InterventiSuApp, key, Hash_AppNonConformi(key))
                                                            Next
                                                        End If
                                                        If app_non_conformi <> "" Then
                                                            Err_Des &= String.Format(Gias.RegistratiAppNonConformi, Left(app_non_conformi, app_non_conformi.Length - 1))
                                                        End If

                                                    Else

                                                        'è l'unico intervento
                                                        If N_Prodotti_OperazioneCorrente > Max_Interventi Then
                                                            Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "(" & N_Prodotti_OperazioneCorrente & ")")
                                                            Err_Code = enTipoErrCode_Verifica.SuperatoNumMaxInterventiPA
                                                            Err_Des = Warning_Des & String.Format(Gias.ConsentitiMassimoNInterventiAnno, Max_Interventi)
                                                            Consultazione = String.Format(Gias.MassimoNINterventiAnno, Max_Interventi)
                                                        End If

                                                    End If

                                                    HashWarning_Numero_Interventi.Add(Max_Interventi & "|" & strPA & "|" & strAvversita & "|" & strGruppiAvversita, "")

                                                    ''-----------------------------------------------------------

                                                    '''Salvo il numero di interventi parziale
                                                    ''Interventi_Scheda_Old = Interventi_Scheda

                                                    'Dim ObjContab_AD As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R
                                                    'Dim Hash_IdAgenda As Hashtable



                                                    'Hash_IdAgenda = ObjContab_AD.LeggiLavorazioni_Da_Principi_Attivi_e_Avversita(PIVA,
                                                    '                                                Sa_Cod,
                                                    '                                                strPA,
                                                    '                                                FiltroAvCod, FiltroAvGru,
                                                    '                                                Appezza,
                                                    '                                                Id_Reg,
                                                    '                                                Validita_Inizio,
                                                    '                                                Validita_Fine,
                                                    '                                                "",
                                                    '                                                "",
                                                    '                                                objParametri_Server,
                                                    '                                                objParametri_Utenti,
                                                    '                                                     Hash_FormulatiPA)

                                                    'If Hash_IdAgenda.Count <> 0 Then
                                                    '    Interventi_Scheda = Interventi_Scheda + Hash_IdAgenda.Count
                                                    'End If

                                                    ''Aggiorno il Warning_Des con il numero di interventi finale
                                                    'Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "(" & Interventi_Scheda & ")")

                                                    ''Verifica Risultato
                                                    'Select Case Interventi_Scheda

                                                    '    Case Is > Max_Interventi
                                                    '        'Numero di Interventi Eccedente
                                                    '        Err_Code = enTipoErrCode_Verifica.SuperatoNumMaxInterventiPA
                                                    '        Err_Des = Warning_Des & "Sono consentiti al massimo " & Max_Interventi & " interventi all'anno. "
                                                    '        Consultazione = "Al massimo " & Max_Interventi & " interventi all'anno. "

                                                    '    Case Is = Max_Interventi
                                                    '    'Tetto Raggiunto

                                                    '    Case Is <Max_Interventi
                                                    '        'Ok

                                                    'End Select

                                                End If

                                            '(12/03/2018) aggiunto nuovo controllo numero minimo interventi da dpi
                                            Case enTipoWarning_Verifica.Numero_Interventi_Min

                                                If impostazioni.UTENTE_COD_BLOCCA_NUMERO_MINIMO_TRATTAMENTI_DPI_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    Interventi_Scheda_Old = Interventi_Scheda

                                                    Dim FiltroAvCod As String = ""
                                                    Dim FiltroAvGru As String = ""
                                                    Select Case TipoTestata
                                                        Case enum_Disciplinare_Tipo_Testata.Difesa
                                                            FiltroAvCod = strAvversita
                                                            FiltroAvGru = ""
                                                        Case enum_Disciplinare_Tipo_Testata.Diserbo
                                                            FiltroAvCod = ""
                                                            FiltroAvGru = strGruppiAvversita
                                                    End Select

                                                    '(10/08/2020 fede nuovi controlli tutti gli app assieme)
                                                    Dim ObjContabDAL As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                                                    Dim dt_Movimenti As DataTable
                                                    Dim Str_FiltroAggiuntivo As New StringBuilder

                                                    Dim FiltroPa As String = ""
                                                    Dim strPaNew As String = strPA
                                                    strPaNew = Replace(strPaNew, "(", "").Trim
                                                    strPaNew = Replace(strPaNew, ")", "").Trim
                                                    Dim Pa As String() = Split(strPaNew, ",")
                                                    If Pa IsNot Nothing Then
                                                        For Each principio In Pa
                                                            Dim PrincipiDPI() = Split(principio.Trim, "+") ' necessario per caso miscele dpi
                                                            For pdpi = 0 To PrincipiDPI.Length - 1
                                                                FiltroPa += "(Movimenti_dettagli.PrincipiAttivi like '" & PrincipiDPI(pdpi).Trim & "§%' OR Movimenti_dettagli.PrincipiAttivi like '%|" & PrincipiDPI(pdpi).Trim & "§%') OR "
                                                            Next
                                                        Next
                                                    End If
                                                    If FiltroPa <> "" Then
                                                        FiltroPa = "  (" & Left(FiltroPa, FiltroPa.Length - 3) & ")"
                                                        Str_FiltroAggiuntivo.Append(FiltroPa)
                                                    End If


                                                    'se sono in inserimento o modifica dell'operazione vado in join con gli il filtro impianti temporaneo (potrebbero essere cambiati in modifica)
                                                    'se sono nel controllore a posteriori vado in join con le destinazioni dell'operazione corrente
                                                    If IDTestataTemp > 0 Then
                                                        'se sono in modifica devo escludere l'id_agenda già salvato dal conteggio (lo aggiungo dopo)
                                                        If Id_Agenda_Escluso <> 0 Then
                                                            If Str_FiltroAggiuntivo.Length > 0 Then
                                                                Str_FiltroAggiuntivo.Append(" AND Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " ")
                                                            Else
                                                                Str_FiltroAggiuntivo.Append(" Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " ")
                                                            End If
                                                        End If
                                                        dt_Movimenti = ObjContabDAL.NumeroProdottiImpianti_suAvversita(PIVA, 0, IDTestataTemp, FiltroAvCod, FiltroAvGru, Str_FiltroAggiuntivo.ToString, "", objParametri_Server)
                                                    Else
                                                        'se sono in verifica a posteriori conteggio nella query anche l'operazione corrente
                                                        dt_Movimenti = ObjContabDAL.NumeroProdottiImpianti_suAvversita(PIVA, Id_Agenda_Escluso, 0, FiltroAvCod, FiltroAvGru, Str_FiltroAggiuntivo.ToString, "", objParametri_Server)
                                                    End If

                                                    Dim NTot As Integer = 0

                                                    'se sono in inserimento o modifica dell'operazione
                                                    'conteggio le sostanze dell'operazione corrente
                                                    Dim HashProdTmp As New Hashtable
                                                    For Each key In HashPrincipiOperazione.Keys
                                                        For Each principio In Pa
                                                            Dim ProCodTmp As Integer = Split(key, "_")(0)
                                                            Dim PaCodTmp As Integer = Split(key, "_")(1)
                                                            Dim PrincipiDPI() = Split(principio.Trim, "+") ' necessario per caso miscele dpi
                                                            For pdpi = 0 To PrincipiDPI.Length - 1
                                                                If Not HashProdTmp.ContainsKey(ProCodTmp) AndAlso PaCodTmp = PrincipiDPI(pdpi).Trim Then
                                                                    HashProdTmp.Add(ProCodTmp, "")
                                                                End If
                                                            Next
                                                        Next
                                                    Next

                                                    Dim N_Prodotti_OperazioneCorrente As Integer = HashProdTmp.Count

                                                    Dim app_non_conformi As String = ""
                                                    Dim Hash_AppNonConformi As New Hashtable

                                                    If dt_Movimenti.Rows.Count > 0 Then

                                                        For Each drTmp As DataRow In dt_Movimenti.Rows
                                                            NTot = drTmp("N_Prodotti")
                                                            Interventi_Scheda = NTot + N_Prodotti_OperazioneCorrente
                                                            If Interventi_Scheda < Min_Interventi Then
                                                                'Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "(" & Interventi_Scheda & ")")
                                                                Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "")
                                                                Err_Code = enTipoErrCode_Verifica.NonRaggiuntoNumMinInterventiPA
                                                                Err_Des = Warning_Des & String.Format(Gias.NecessariAlmenoNInterventiAnno, Min_Interventi)
                                                                Consultazione = String.Format(Gias.AlmenoNInterventiAnno, Min_Interventi)
                                                                If Not Hash_AppNonConformi.ContainsKey(Interventi_Scheda) Then
                                                                    Hash_AppNonConformi.Add(Interventi_Scheda, drTmp("app_nome"))
                                                                Else
                                                                    Hash_AppNonConformi(Interventi_Scheda) = Hash_AppNonConformi(Interventi_Scheda) & ", " & drTmp("app_nome")
                                                                End If
                                                                'Exit For
                                                            End If
                                                        Next
                                                        If Hash_AppNonConformi.Count > 0 Then
                                                            For Each key In Hash_AppNonConformi.Keys
                                                                app_non_conformi &= String.Format(Gias.InterventiSuApp, key, Hash_AppNonConformi(key))
                                                            Next
                                                        End If
                                                        If app_non_conformi <> "" Then
                                                            Err_Des &= String.Format(Gias.RegistratiAppNonConformi, Left(app_non_conformi, app_non_conformi.Length - 1))
                                                        End If

                                                    Else

                                                        'è l'unico intervento
                                                        If N_Prodotti_OperazioneCorrente < Min_Interventi Then
                                                            Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "(" & N_Prodotti_OperazioneCorrente & ")")
                                                            Err_Code = enTipoErrCode_Verifica.NonRaggiuntoNumMinInterventiPA
                                                            Err_Des = Warning_Des & String.Format(Gias.NecessariAlmenoNInterventiAnno, Min_Interventi)
                                                            Consultazione = String.Format(Gias.AlmenoNInterventiAnno, Min_Interventi)
                                                        End If

                                                    End If

                                                    HashWarning_Numero_Interventi_Min.Add(Min_Interventi & "|" & strPA & "|" & strAvversita & "|" & strGruppiAvversita, "")


                                                    '-----------------------------------------------------------------------------

                                                    '    'Salvo il numero di interventi parziale
                                                    '    Interventi_Scheda_Old = Interventi_Scheda

                                                    '    Dim ObjContab_AD As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R
                                                    '    Dim Hash_IdAgenda As Hashtable

                                                    '    Dim FiltroAvCod As String = ""
                                                    '    Dim FiltroAvGru As String = ""
                                                    '    Select Case TipoTestata
                                                    '        Case enum_Disciplinare_Tipo_Testata.Difesa
                                                    '            FiltroAvCod = strAvversita
                                                    '            FiltroAvGru = ""
                                                    '        Case enum_Disciplinare_Tipo_Testata.Diserbo
                                                    '            FiltroAvCod = ""
                                                    '            FiltroAvGru = strGruppiAvversita
                                                    '    End Select

                                                    '    Hash_IdAgenda = ObjContab_AD.LeggiLavorazioni_Da_Principi_Attivi_e_Avversita(PIVA,
                                                    '                                                    Sa_Cod,
                                                    '                                                    strPA,
                                                    '                                                    FiltroAvCod, FiltroAvGru,
                                                    '                                                    Appezza,
                                                    '                                                    Id_Reg,
                                                    '                                                    Validita_Inizio,
                                                    '                                                    Validita_Fine,
                                                    '                                                    "",
                                                    '                                                    "",
                                                    '                                                    objParametri_Server,
                                                    '                                                    objParametri_Utenti,
                                                    '                                                         Hash_FormulatiPA)

                                                    '    If Hash_IdAgenda.Count <> 0 Then
                                                    '        Interventi_Scheda = Interventi_Scheda + Hash_IdAgenda.Count
                                                    '    End If

                                                    '    'Aggiorno il Warning_Des con il numero di interventi finale
                                                    '    Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "(" & Interventi_Scheda & ")")

                                                    '    'Verifica Risultato
                                                    '    Select Case Interventi_Scheda

                                                    '        Case Is < Min_Interventi
                                                    '            'Numero di Interventi non suff
                                                    '            Err_Code = enTipoErrCode_Verifica.NonRaggiuntoNumMinInterventiPA
                                                    '            Err_Des = Warning_Des & "Sono necessari almeno " & Min_Interventi & " interventi all'anno. "
                                                    '            Consultazione = "Almeno " & Min_Interventi & " interventi all'anno. "

                                                    '        Case Is = Min_Interventi
                                                    '        'Tetto Raggiunto

                                                    '        Case Is > Min_Interventi
                                                    '            'Ok

                                                    '    End Select

                                                End If

                                            Case enTipoWarning_Verifica.Numero_Interventi_xProdotto

                                                If impostazioni.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    '(10/08/2020 fede nuovi controlli tutti gli app assieme)
                                                    Interventi_Scheda_Old = Interventi_Scheda

                                                    Dim Data_Inizio As Date = Validita_Inizio
                                                    Dim Data_Fine As Date = Validita_Fine

                                                    Max_Interventi_Udm_Des = " " & Gias.AllAnno

                                                    Select Case Max_Interventi_xProdotto_Udm
                                                        Case enum_UnitaMisura.Anno
                                                            Max_Interventi_Udm_Des = " " & Gias.AllAnno
                                                        Case enum_UnitaMisura.CicloColturale
                                                            Max_Interventi_Udm_Des = " " & Gias.aCicloColturale
                                                        Case enum_UnitaMisura.StagioneColturale
                                                            Max_Interventi_Udm_Des = " " & Gias.aStagioneColturale
                                                    End Select

                                                    '(12/10/2018 fede) se ho un globale verifico 
                                                    'num volte utilizzata la dose corrente
                                                    'num volte utilizzato il prodotto

                                                    Dim ObjContabDAL As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                                                    Dim dt_Movimenti As DataTable = Nothing
                                                    Dim dt_Movimenti_SingolaDose As DataTable = Nothing

                                                    Dim Str_FiltroAggiuntivo As New StringBuilder

                                                    'esempio doseetichetta_value:
                                                    '1843813$500$600$23$g/hl$1000$1500$22$l/ha$74$9$4$2019$all'anno$$0$7$0$0$0$5$$1$0<br>1843814$0$7,5$88$kg/ha$0$0$0$$74$9$4$2019$all'anno$$0$7$0$0$0$5$$1$0

                                                    Dim filtroDosaggio As String = $"(Movimenti_dettagli.doseetichetta_value like '{Agro_SQL_SaveText(For_Veg_Av_Dos_Cod.ToString)}%'
                                                                                      OR  Movimenti_dettagli.doseetichetta_value like '%<br>{Agro_SQL_SaveText(For_Veg_Av_Dos_Cod.ToString)}%')"

                                                    'se sono in inserimento o modifica dell'operazione vado in join con gli il filtro impianti temporaneo (potrebbero essere cambiati in modifica)
                                                    'se sono nel controllore a posteriori vado in join con le destinazioni dell'operazione corrente
                                                    If IDTestataTemp > 0 Then

                                                        'se sono in modifica devo escludere l'id_agenda già salvato dal conteggio (lo aggiungo dopo)
                                                        If Id_Agenda_Escluso <> 0 Then
                                                            Str_FiltroAggiuntivo.Append(" Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " ")
                                                        End If

                                                        If Max_Interventi_xProdotto_Globali > 0 Then
                                                            dt_Movimenti = ObjContabDAL.NumeroTrattamentiImpianti_conFrCod(PIVA, 0, IDTestataTemp, Pro_Cod, Str_FiltroAggiuntivo.ToString, "", objParametri_Server)
                                                        End If

                                                        If Str_FiltroAggiuntivo.Length > 0 Then
                                                            Str_FiltroAggiuntivo.Append($" AND {filtroDosaggio}")
                                                        Else
                                                            Str_FiltroAggiuntivo.Append($" {filtroDosaggio}")
                                                        End If

                                                        dt_Movimenti_SingolaDose = ObjContabDAL.NumeroTrattamentiImpianti_conFrCod(PIVA, 0, IDTestataTemp, Pro_Cod, Str_FiltroAggiuntivo.ToString, "", objParametri_Server)

                                                    Else
                                                        'se sono in verifica a posteriori conteggio nella query anche l'operazione corrente

                                                        If Max_Interventi_xProdotto_Globali > 0 Then
                                                            dt_Movimenti = ObjContabDAL.NumeroTrattamentiImpianti_conFrCod(PIVA, Id_Agenda_Escluso, 0, Pro_Cod, Str_FiltroAggiuntivo.ToString, "", objParametri_Server)
                                                        End If

                                                        Str_FiltroAggiuntivo.Append($" {filtroDosaggio}")

                                                        dt_Movimenti_SingolaDose = ObjContabDAL.NumeroTrattamentiImpianti_conFrCod(PIVA, Id_Agenda_Escluso, 0, Pro_Cod, Str_FiltroAggiuntivo.ToString, "", objParametri_Server)
                                                    End If

                                                    Dim app_non_conformi As String = ""
                                                    Dim Hash_AppNonConformi As New Hashtable

                                                    If dt_Movimenti_SingolaDose IsNot Nothing AndAlso dt_Movimenti_SingolaDose.Rows.Count > 0 Then

                                                        If dt_Movimenti IsNot Nothing AndAlso dt_Movimenti.Rows.Count > 0 AndAlso Max_Interventi_xProdotto_Globali > 0 Then

                                                            Dim NTot As Integer = 0
                                                            Dim NSingolaDose As Integer = 0

                                                            For Each drTmp As DataRow In dt_Movimenti.Rows

                                                                NTot = drTmp("N_Operazioni")

                                                                Interventi_Scheda = NTot

                                                                'se sono in modifica o inserimento aggiungo l'operazione corrente al conteggio
                                                                If IDTestataTemp > 0 Then
                                                                    Interventi_Scheda = Interventi_Scheda + 1
                                                                End If

                                                                If Interventi_Scheda > Max_Interventi_xProdotto_Globali Then
                                                                    Err_Code = enTipoErrCode_Verifica.SuperatoNumMaxInterventiProdotto
                                                                    Warning_Des = Replace(Warning_Des, Max_Interventi_xProdotto, Max_Interventi_xProdotto_Globali)
                                                                    Err_Des = Warning_Des
                                                                    Consultazione = String.Format(Gias.MassimoNInterventiUDMDes, Max_Interventi_xProdotto_Globali, Max_Interventi_Udm_Des)
                                                                    Exit For
                                                                End If

                                                                Dim DrTmpDoseSingola As DataRow()
                                                                NSingolaDose = 0

                                                                If dt_Movimenti_SingolaDose IsNot Nothing AndAlso dt_Movimenti_SingolaDose.Rows.Count > 0 Then
                                                                    DrTmpDoseSingola = dt_Movimenti_SingolaDose.Select("piva='" & drTmp("piva").ToString & "' and sa_cod=" & drTmp("sa_cod").ToString & " and appezza=" & drTmp("appezza").ToString & " and id_reg=" & drTmp("id_reg").ToString)
                                                                    If DrTmpDoseSingola IsNot Nothing AndAlso DrTmpDoseSingola.Length > 0 Then

                                                                        NSingolaDose = DrTmpDoseSingola(0).Item("N_Operazioni")

                                                                        Dim Interventi_Scheda_SingolaDose As Integer = NSingolaDose

                                                                        'se sono in modifica o inserimento aggiungo l'operazione corrente al conteggio
                                                                        If IDTestataTemp > 0 Then
                                                                            Interventi_Scheda_SingolaDose = Interventi_Scheda_SingolaDose + 1
                                                                        End If

                                                                        If Interventi_Scheda_SingolaDose > Max_Interventi_xProdotto Then
                                                                            Err_Code = enTipoErrCode_Verifica.SuperatoNumMaxInterventiProdotto
                                                                            Warning_Des = Replace(Warning_Des, "da etichetta", "per questo dosaggio da etichetta")
                                                                            Err_Des = Warning_Des
                                                                            Consultazione = String.Format(Gias.MassimoNInterventiUDMDes, Max_Interventi_xProdotto, Max_Interventi_Udm_Des)
                                                                            Exit For
                                                                        End If

                                                                    End If
                                                                End If

                                                            Next

                                                        Else

                                                            Dim NTot As Integer = 0

                                                            For Each drTmp As DataRow In dt_Movimenti_SingolaDose.Rows
                                                                NTot = drTmp("N_Operazioni")
                                                                Interventi_Scheda = NTot
                                                                'se sono in modifica o inserimento aggiungo l'operazione corrente al conteggio
                                                                If IDTestataTemp > 0 Then
                                                                    Interventi_Scheda = Interventi_Scheda + 1
                                                                End If

                                                                If Interventi_Scheda > Max_Interventi_xProdotto Then
                                                                    'Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "(" & Interventi_Scheda & ")")
                                                                    Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "")
                                                                    Err_Code = enTipoErrCode_Verifica.SuperatoNumMaxInterventiProdotto
                                                                    Err_Des = Warning_Des
                                                                    Consultazione = String.Format(Gias.MassimoNInterventiUDMDes, Max_Interventi_xProdotto, Max_Interventi_Udm_Des)
                                                                    If Not Hash_AppNonConformi.ContainsKey(Interventi_Scheda) Then
                                                                        Hash_AppNonConformi.Add(Interventi_Scheda, drTmp("app_nome"))
                                                                    Else
                                                                        Hash_AppNonConformi(Interventi_Scheda) = Hash_AppNonConformi(Interventi_Scheda) & ", " & drTmp("app_nome")
                                                                    End If
                                                                    'Exit For
                                                                End If

                                                            Next

                                                        End If

                                                        If Hash_AppNonConformi.Count > 0 Then
                                                            For Each key In Hash_AppNonConformi.Keys
                                                                app_non_conformi &= String.Format(Gias.InterventiSuApp, key, Hash_AppNonConformi(key))
                                                            Next
                                                        End If
                                                        If app_non_conformi <> "" Then
                                                            Err_Des &= String.Format(Gias.RegistratiAppNonConformi, Left(app_non_conformi, app_non_conformi.Length - 1))
                                                        End If

                                                    End If

                                                    HashWarning_Numero_Interventi_xProdotto.Add(Pro_Cod & "|" & Max_Interventi_xProdotto & "|" & Max_Interventi_xProdotto_Globali, "")

                                                    '-----------------------------------------------------

                                                    '''Salvo il numero di interventi parziale
                                                    ''Interventi_Scheda_Old = Interventi_Scheda

                                                    ''Dim ObjContab_AD As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R
                                                    ''Dim Hash_IdAgenda As Hashtable
                                                    ''Dim Hash_IdAgenda_SingolaDose As Hashtable

                                                    ''Dim Data_Inizio As Date = Validita_Inizio
                                                    ''Dim Data_Fine As Date = Validita_Fine

                                                    ''Max_Interventi_Udm_Des = " all'anno. "

                                                    '''(26/04/2017 fede) modificato controllo 
                                                    '''verranno controllate le seguenti unità di misura (2019=anno, 2024=stagione colturale, 2025=ciclo colturale) 
                                                    '''e verranno controllate sempre in base alle date dell'esercizio
                                                    '''Select Case Max_Interventi_xProdotto_Udm
                                                    '''    Case enum_UnitaMisura.Anno
                                                    '''        Data_Inizio = "31/12/" & Data.Year - 1
                                                    '''        Data_Fine = "31/12/" & Data.Year
                                                    '''    Case enum_UnitaMisura.CicloColturale
                                                    '''        Max_Interventi_Udm_Des = " a ciclo colturale. "
                                                    '''End Select
                                                    ''Select Case Max_Interventi_xProdotto_Udm
                                                    ''    Case enum_UnitaMisura.Anno
                                                    ''        Max_Interventi_Udm_Des = " all'anno. "
                                                    ''    Case enum_UnitaMisura.CicloColturale
                                                    ''        Max_Interventi_Udm_Des = " a ciclo colturale. "
                                                    ''    Case enum_UnitaMisura.StagioneColturale
                                                    ''        Max_Interventi_Udm_Des = " a stagione colturale. "
                                                    ''End Select

                                                    ''Dim Filtro As String = ""
                                                    '''If Gruppo_Dosaggi <> 0 Then
                                                    '''    Filtro = " and DoseEtichetta_Value like '%" & For_Veg_Av_Dos_Cod & "%'"
                                                    '''End If

                                                    '''(12/10/2018 fede) se ho un globale verifico 
                                                    '''num volte utilizzata la dose corrente
                                                    '''num volte utilizzato il prodotto


                                                    ''Hash_IdAgenda = ObjContab_AD.LeggiTrattamenti_Da_Formulato(PIVA,
                                                    ''                                                Sa_Cod,
                                                    ''                                                Pro_Cod,
                                                    ''                                                Appezza,
                                                    ''                                                Id_Reg,
                                                    ''                                                Data_Inizio,
                                                    ''                                                Data_Fine,
                                                    ''                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    ''                                                Filtro,
                                                    ''                                                "",
                                                    ''                                                objParametri_Server,
                                                    ''                                                objParametri_Utenti)

                                                    ''If Max_Interventi_xProdotto_Globali > 0 Then

                                                    ''    Filtro = " AND (Movimenti_dettagli.doseetichetta_value like '" & Agro_SQL_SaveText(For_Veg_Av_Dos_Cod.ToString) & "$%' " &
                                                    ''    "      OR  Movimenti_dettagli.doseetichetta_value like '<br>" & Agro_SQL_SaveText(For_Veg_Av_Dos_Cod.ToString) & "$%') "

                                                    ''    'se sono in modifica escludo l'operazione corrente (potrei aver cambiato il dosaggio)
                                                    ''    If Id_Agenda_Escluso <> 0 Then
                                                    ''        Filtro += " AND Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " "
                                                    ''    End If

                                                    ''    Hash_IdAgenda_SingolaDose = ObjContab_AD.LeggiTrattamenti_Da_Formulato(PIVA,
                                                    ''                                        Sa_Cod,
                                                    ''                                        Pro_Cod,
                                                    ''                                        Appezza,
                                                    ''                                        Id_Reg,
                                                    ''                                        Data_Inizio,
                                                    ''                                        Data_Fine,
                                                    ''                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    ''                                        Filtro,
                                                    ''                                        "",
                                                    ''                                        objParametri_Server,
                                                    ''                                        objParametri_Utenti)


                                                    ''    'marco
                                                    ''    Dim Interventi_Scheda_SingolaDose As Integer = 1
                                                    ''    If Hash_IdAgenda_SingolaDose.Count <> 0 Then
                                                    ''        Interventi_Scheda_SingolaDose += Hash_IdAgenda_SingolaDose.Count
                                                    ''    End If
                                                    ''    'Dim Interventi_Scheda_SingolaDose As Integer = 0
                                                    ''    'If Hash_IdAgenda_SingolaDose.Count <> 0 Then
                                                    ''    '    Interventi_Scheda_SingolaDose = Interventi_Scheda + Hash_IdAgenda_SingolaDose.Count
                                                    ''    '    ''se sono in modifica aggiungo l'operazione corrente (potrei aver cambiato il dosaggio)
                                                    ''    '    'If Id_Agenda_Escluso <> 0 Then
                                                    ''    '    '    Interventi_Scheda_SingolaDose += 1
                                                    ''    '    'End If
                                                    ''    'End If

                                                    ''    If Hash_IdAgenda.Count <> 0 Then
                                                    ''        Interventi_Scheda = Interventi_Scheda + Hash_IdAgenda.Count
                                                    ''    End If

                                                    ''    'Verifica Risultato
                                                    ''    Select Case Interventi_Scheda_SingolaDose
                                                    ''        Case Is > Max_Interventi_xProdotto
                                                    ''            Err_Code = enTipoErrCode_Verifica.SuperatoNumMaxInterventiProdotto
                                                    ''            Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "(" & Interventi_Scheda_SingolaDose & ")")
                                                    ''            Warning_Des = Replace(Warning_Des, "da etichetta", "per questo dosaggio da etichetta")
                                                    ''            Err_Des = Warning_Des
                                                    ''            Consultazione = "Al massimo " & Max_Interventi_xProdotto & " interventi " & Max_Interventi_Udm_Des
                                                    ''    End Select

                                                    ''    'Verifica Risultato
                                                    ''    Select Case Interventi_Scheda
                                                    ''        Case Is > Max_Interventi_xProdotto_Globali
                                                    ''            'Numero di Interventi Eccedente
                                                    ''            Err_Code = enTipoErrCode_Verifica.SuperatoNumMaxInterventiProdotto
                                                    ''            Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "(" & Interventi_Scheda & ")")
                                                    ''            Err_Des = Warning_Des
                                                    ''            Consultazione = "Al massimo " & Max_Interventi_xProdotto_Globali & " interventi " & Max_Interventi_Udm_Des
                                                    ''    End Select



                                                    ''Else

                                                    ''    ' CASO VECCHIO

                                                    ''    If Hash_IdAgenda.Count <> 0 Then
                                                    ''        Interventi_Scheda = Interventi_Scheda + Hash_IdAgenda.Count
                                                    ''    End If

                                                    ''    'Aggiorno il Warning_Des con il numero di interventi finale
                                                    ''    Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "(" & Interventi_Scheda & ")")

                                                    ''    'Verifica Risultato
                                                    ''    Select Case Interventi_Scheda

                                                    ''        Case Is > Max_Interventi_xProdotto
                                                    ''            'Numero di Interventi Eccedente
                                                    ''            Err_Code = enTipoErrCode_Verifica.SuperatoNumMaxInterventiProdotto
                                                    ''            Err_Des = Warning_Des
                                                    ''            Consultazione = "Al massimo " & Max_Interventi_xProdotto & " interventi " & Max_Interventi_Udm_Des

                                                    ''        Case Is = Max_Interventi_xProdotto
                                                    ''    'Tetto Raggiunto

                                                    ''        Case Is < Max_Interventi_xProdotto
                                                    ''            'Ok

                                                    ''    End Select

                                                    ''End If

                                                End If

                                            Case enTipoWarning_Verifica.Intervallo_Trattamenti_xProdotto

                                                If impostazioni.UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    '(10/08/2020 fede nuovi controlli tutti gli app assieme)
                                                    Dim ObjContab_AD As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                                                    Dim dt_Movimenti As DataTable

                                                    Dim Str_FiltroAggiuntivo As New StringBuilder

                                                    'sia nel controllo in fase di modifica dell'operazione che nel controllo dell'operazione a posteriori
                                                    'devo escludere l'operazione corrente 
                                                    If Id_Agenda_Escluso <> 0 Then
                                                        Str_FiltroAggiuntivo.Append(" Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " ")
                                                    End If

                                                    'se sono in inserimento o modifica dell'operazione vado in join con gli il filtro impianti temporaneo (potrebbero essere cambiati in modifica)
                                                    'se sono nel controllore a posteriori vado in join con le destinazioni dell'operazione corrente
                                                    If IDTestataTemp > 0 Then
                                                        dt_Movimenti = ObjContab_AD.UltimoTrattamentoImpianti_conFrCod(PIVA, 0, IDTestataTemp, Pro_Cod, Data, Str_FiltroAggiuntivo.ToString, "", objParametri_Server)
                                                    Else
                                                        dt_Movimenti = ObjContab_AD.UltimoTrattamentoImpianti_conFrCod(PIVA, Id_Agenda_Escluso, 0, Pro_Cod, Data, Str_FiltroAggiuntivo.ToString, "", objParametri_Server)
                                                    End If

                                                    For Each drTmp As DataRow In dt_Movimenti.Rows
                                                        If IsDate(drTmp("data_movimento")) AndAlso drTmp("data_movimento").AddDays(IntervalloTrattamenti_Min) > Data Then
                                                            Err_Code = enTipoErrCode_Verifica.IntervalloInterventiNonRispettato
                                                            Err_Des = Warning_Des
                                                            Consultazione = String.Format(Gias.RispettareIntervalloGiorniFraInterventi, IntervalloTrattamenti_Min)
                                                            Exit For
                                                        End If
                                                    Next

                                                    HashWarning_Intervallo_Trattamenti_xProdotto.Add(Pro_Cod & "|" & IntervalloTrattamenti_Min, "")

                                                    '    Dim ObjContab_AD As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

                                                    '    Dim dt_Movimenti As DataTable
                                                    '    Dim Str_FiltroAggiuntivo As New StringBuilder
                                                    '    Str_FiltroAggiuntivo.Append("( ")
                                                    '    Str_FiltroAggiuntivo.Append(" Mov_Destinazioni.Piva = '" & PIVA & "' ")
                                                    '    Str_FiltroAggiuntivo.Append(" AND Mov_Destinazioni.Sa_Cod = " & Sa_Cod & " ")
                                                    '    Str_FiltroAggiuntivo.Append(" AND Mov_Destinazioni.Appezza = " & Appezza & " ")
                                                    '    Str_FiltroAggiuntivo.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Id_Reg & " ")
                                                    '    Str_FiltroAggiuntivo.Append(" AND  Imprese_Progetti.Validita_Inizio <= '" & Data & "' AND Imprese_Progetti.Validita_Fine  >= '" & Data & "' ")
                                                    '    Str_FiltroAggiuntivo.Append(" AND  data_movimento <= '" & Data & "'  ")

                                                    '    If Id_Agenda_Escluso <> 0 Then
                                                    '        Str_FiltroAggiuntivo.Append(" AND Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " ")
                                                    '    End If

                                                    '    Str_FiltroAggiuntivo.Append(" ) ")

                                                    '    dt_Movimenti = ObjContab_AD.UltimoTrattamentoImpianti(Pro_Cod, Data, Str_FiltroAggiuntivo.ToString, " data_movimento desc", objParametri_Server)
                                                    '    If Not dt_Movimenti Is Nothing AndAlso dt_Movimenti.Rows.Count > 0 Then
                                                    '        Dim UltimaData As Date = dt_Movimenti.Rows(0).Item("data_movimento")
                                                    '        If UltimaData.AddDays(IntervalloTrattamenti_Min) > Data Then
                                                    '            Err_Code = enTipoErrCode_Verifica.IntervalloInterventiNonRispettato
                                                    '            Err_Des = Warning_Des
                                                    '            Consultazione = "Rispettare l'intervallo di giorni (" & IntervalloTrattamenti_Min & ") tra un intervento e l'altro. "
                                                    '        End If
                                                    '    End If

                                                End If

                                            Case enTipoWarning_Verifica.DoseRame_Anno_xBio

                                                If impostazioni.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    '(02/11/2018 fede) modificato controllo rame per deroghe
                                                    'se ho DoseDpi_MaxAnno valorizzata
                                                    '   prendo quel valore come massimo 
                                                    '   es. pomodoro 2017 batteriosi limite 6 kg annullato (caso in cui non arriva più il warning)
                                                    '   es. pomodoro 2018 batteriosi totale concessi 9 kg
                                                    'altrimenti 
                                                    '   il massimo rimane i 6 kg

                                                    Dim Rame_Qta_Max_Kg As Decimal = 6

                                                    If Data >= #1/1/2019# Then
                                                        Rame_Qta_Max_Kg = 4
                                                    End If

                                                    Dim objRame As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

                                                    Dim DtDistinte As DataTable
                                                    Dim Str_FiltroAggiuntivo As New StringBuilder

                                                    If IDTestataTemp__tmp_FormulatiXPrincipiAttivi <= 0 Then
                                                        Dim strFrCod As String = ""
                                                        Dim DtPrincipi As DataTable
                                                        Dim obj_Op As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                                                        Dim DTFrCod As DataTable = obj_Op.Leggi_Prodotti_Utilizzati_Su_VegCod_IntervalloTemporale(PIVA, Sa_Cod, Veg_Cod, AGRODATAINIZIO, AGRODATAFINE, FORMULATI, "", "", objParametri_Server)
                                                        If DTFrCod IsNot Nothing AndAlso DTFrCod.Rows.Count > 0 Then
                                                            For f = 0 To DTFrCod.Rows.Count - 1
                                                                strFrCod &= DTFrCod.Rows(f).Item("pro_cod") & ","
                                                            Next
                                                            If strFrCod <> "" Then
                                                                Dim objWs As New AgronicaCoreWebService.AgroWs
                                                                DtPrincipi = objWs.ComposizioneFormulatiRecupera(Left(strFrCod, strFrCod.Length - 1), objParametri_Server, objParametri_Utenti)
                                                                If DtPrincipi IsNot Nothing Then
                                                                    Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                                                                    'IDTestataTemp__tmp_FormulatiXPrincipiAttivi = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("IDTestataTemp", objParametri_Server)
                                                                    'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                                                                    IDTestataTemp__tmp_FormulatiXPrincipiAttivi = xAgrosequenze.NuovoId_Tabella("IDTestataTemp", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
                                                                    Dim filtroRameici As String = PaRameici_str.Replace("(", "").Replace(")", "")
                                                                    For p = 0 To DtPrincipi.Rows.Count - 1
                                                                        PopolaTabellaFormulatiPA(filtroRameici, IDTestataTemp__tmp_FormulatiXPrincipiAttivi, objParametri_Server, DtPrincipi.Rows(p).Item("elenco_PrincipiAttivi"), DtPrincipi.Rows(p).Item("elenco_PrincipiAttiviPesi"), DtPrincipi.Rows(p).Item("fr_cod"))
                                                                    Next
                                                                End If
                                                            End If
                                                        End If
                                                    End If

                                                    ''se sono in inserimento o modifica dell'operazione vado in join con gli il filtro impianti temporaneo (potrebbero essere cambiati in modifica)
                                                    ''se sono nel controllore a posteriori vado in join con le destinazioni dell'operazione corrente
                                                    If IDTestataTemp > 0 Then
                                                        'se sono in modifica devo escludere l'id_agenda già salvato dal conteggio (lo aggiungo dopo)
                                                        If Id_Agenda_Escluso <> 0 Then
                                                            Str_FiltroAggiuntivo.Append(" Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " ")
                                                            If Raccoglitore_Cod <> 0 Then
                                                                Str_FiltroAggiuntivo.Append(" AND ISNULL(Agenda.Raccoglitore_Cod, 0) <> " & Agro_SQL_SaveNum(Raccoglitore_Cod) & " ")
                                                            End If
                                                        End If
                                                        DtDistinte = objRame.Leggi_Rame_Distribuito_suDistinte(PIVA, 0, IDTestataTemp, IDTestataTemp__tmp_FormulatiXPrincipiAttivi, Str_FiltroAggiuntivo.ToString, objParametri_Server)
                                                    Else
                                                        'se sono in verifica a posteriori conteggio nella query anche l'operazione corrente
                                                        DtDistinte = objRame.Leggi_Rame_Distribuito_suDistinte(PIVA, Id_Agenda_Escluso, 0, IDTestataTemp__tmp_FormulatiXPrincipiAttivi, "", objParametri_Server)
                                                    End If

                                                    If IDTestataTemp > 0 Then
                                                        If IDTestataTemp__tmp_FormulatiXPrincipiAttivi > 0 Then
                                                            PulisciTabella__tmp_FormulatiXPrincipiAttivi(IDTestataTemp__tmp_FormulatiXPrincipiAttivi, objParametri_Server)
                                                        End If
                                                    End If

                                                    Dim Cu_Distribuito_Fertilizzazioni As Decimal = 0
                                                    Dim Cu_Distribuito_Trattamenti As Decimal = 0
                                                    Dim Cu_Operazione As Decimal = 0
                                                    Dim Qta_Pa_Tot As Decimal = 0

                                                    If DoseDpi_MaxAnno > 0 Then
                                                        Rame_Qta_Max_Kg = DoseDpi_MaxAnno
                                                    End If

                                                    For Each distinta As DataRow In DtDistinte.Rows

                                                        Cu_Distribuito_Fertilizzazioni = 0
                                                        Cu_Distribuito_Trattamenti = 0
                                                        Cu_Operazione = 0

                                                        If Not IsDBNull(distinta.Item("Cu_Distribuito_Fertilizzazioni_Ha")) Then
                                                            Cu_Distribuito_Fertilizzazioni = distinta.Item("Cu_Distribuito_Fertilizzazioni_Ha")
                                                        End If
                                                        If Not IsDBNull(distinta.Item("Cu_Distribuito_Trattamenti_Ha")) Then
                                                            Cu_Distribuito_Trattamenti = distinta.Item("Cu_Distribuito_Trattamenti_Ha")
                                                        End If


                                                        Dim Sup As Decimal
                                                        ''se sono in inserimento/modifica devo aggiungere il rame dell'operazione corrente
                                                        If TipoOperazioneDB > 0 AndAlso Destinazioni IsNot Nothing Then

                                                            For i = 0 To UBound(Destinazioni, 2) - 1
                                                                If Destinazioni(i_Piva, i) = distinta.Item("piva") AndAlso
                                                                    Destinazioni(i_Sa_Cod, i) = distinta.Item("sa_cod") AndAlso
                                                                    Destinazioni(i_Appezza, i) = distinta.Item("appezza") AndAlso
                                                                    Destinazioni(i_Id_Reg, i) = distinta.Item("id_reg") AndAlso
                                                                    Destinazioni(i_Pro_Cod, i) = Pro_Cod Then

                                                                    If CDec(Destinazioni(i_Sup_Imp, i)) > 0 Then
                                                                        Sup = CDec(Destinazioni(i_Sup_Imp, i))
                                                                    Else
                                                                        Sup = Sup_Imp
                                                                    End If
                                                                    If Sup <> 0 Then
                                                                        If Peso = 0 Then
                                                                            Cu_Operazione += (CDec(Destinazioni(i_Qta, i)) / Sup) * Titolo / 100
                                                                        Else
                                                                            Cu_Operazione += (CDec(Destinazioni(i_Qta, i)) / Sup) * Peso / 1000
                                                                        End If
                                                                    End If
                                                                End If
                                                            Next
                                                        End If

                                                        Dim CU_Fertilizzazione_MultiAttivita As Decimal = 0
                                                        Dim CU_Trattamenti_MultiAttivita As Decimal = 0
                                                        If paramVerificaDPIMultiAttivita IsNot Nothing Then
                                                            If paramVerificaDPIMultiAttivita.dic_CUxHaTrattamenti_xDistintaxLavCod IsNot Nothing Then
                                                                For Each trattamento In paramVerificaDPIMultiAttivita.dic_CUxHaTrattamenti_xDistintaxLavCod
                                                                    '((PIVA, SA_COD, APPEZZA, ID_REG, PRO_COD), LAV_COD), CU_xDISTINTAxLAV_COD)
                                                                    Dim k1 = trattamento.Key.Item1
                                                                    Dim k2 = trattamento.Key.Item2

                                                                    Dim t_piva As String = k1.Item1
                                                                    Dim t_sa_cod As Integer = k1.Item2
                                                                    Dim t_appezza As Integer = k1.Item3
                                                                    Dim t_id_reg As Integer = k1.Item4
                                                                    Dim t_lav_cod As Integer = k2

                                                                    'Vado a ripescare il CU di tutti i trattamenti appena fatti sulla distinta che sto guardando 
                                                                    'Escludo il CU del trattamento corrente perché viene già contato in CU_Operazione
                                                                    If t_piva = distinta.Item("piva") And
                                                                        t_sa_cod = distinta.Item("sa_cod") And
                                                                        t_appezza = distinta.Item("appezza") And
                                                                        t_id_reg = distinta.Item("id_reg") And
                                                                        t_lav_cod <> Lav_Cod Then

                                                                        CU_Trattamenti_MultiAttivita += trattamento.Value
                                                                    End If
                                                                Next
                                                            End If

                                                            If paramVerificaDPIMultiAttivita.TotCUxHa_Fertilizzazione_MultiAttivita_xDistinta IsNot Nothing Then
                                                                For Each distintaxCU In paramVerificaDPIMultiAttivita.TotCUxHa_Fertilizzazione_MultiAttivita_xDistinta
                                                                    '((PIVA, SA_COD, APPEZZA, ID_REG), CU_xDISTINTA)
                                                                    Dim k1 = distintaxCU.Key

                                                                    Dim t_piva As String = k1.Item1
                                                                    Dim t_sa_cod As Integer = k1.Item2
                                                                    Dim t_appezza As Integer = k1.Item3
                                                                    Dim t_id_reg As Integer = k1.Item4

                                                                    'Vado a ripescare il CU di tutti i trattamenti appena fatti sulla distinta che sto guardando 
                                                                    'Escludo il CU del trattamento corrente perché viene già contato in CU_Operazione
                                                                    If t_piva = distinta.Item("piva") And
                                                                        t_sa_cod = distinta.Item("sa_cod") And
                                                                        t_appezza = distinta.Item("appezza") And
                                                                        t_id_reg = distinta.Item("id_reg") Then

                                                                        CU_Fertilizzazione_MultiAttivita += distintaxCU.Value
                                                                    End If
                                                                Next
                                                            End If
                                                        End If

                                                        Qta_Pa_Tot = Cu_Distribuito_Fertilizzazioni + Cu_Distribuito_Trattamenti + Cu_Operazione + CU_Trattamenti_MultiAttivita + CU_Fertilizzazione_MultiAttivita

                                                        If Qta_Pa_Tot > Rame_Qta_Max_Kg Then
                                                            Err_Code = enTipoErrCode_Verifica.DoseEccessivaRame
                                                            If DoseDpi_MaxAnno <> 6 AndAlso DoseDpi_MaxAnno <> 4 Then
                                                                Err_Des = String.Format(Gias.DoseRameDistribuitaSuperaMaxConsentitaAnnoDaDeroga, Math.Round(Qta_Pa_Tot, 3), Rame_Qta_Max_Kg)
                                                            Else
                                                                Err_Des = String.Format(Gias.DoseRameDistribuitaSuAppSuperaMaxConsentitaAnno, distinta.Item("app_nome"), Math.Round(Qta_Pa_Tot, 3), Rame_Qta_Max_Kg)
                                                            End If
                                                            Consultazione = String.Format(Gias.ConsentitoUsareMaxRameAnno, Rame_Qta_Max_Kg)
                                                            Exit For
                                                        End If

                                                    Next

                                                    HashWarning_DoseRame_Anno_xBio.Add(strPA, "")

                                                    '------------------------------
                                                    '------------------------------
                                                    '------------------------------


                                                    '    Dim Inizio As Date = Validita_Inizio
                                                    '    Dim Fine As Date = Validita_Fine

                                                    '    Dim dtLavorazioni As DataTable
                                                    '    Dim ObjContab As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R
                                                    '    dtLavorazioni = ObjContab.LeggiLavorazioni_Da_Principi_Attivi(PIVA,
                                                    '                                                    Sa_Cod,
                                                    '                                                    strPA,
                                                    '                                                    Appezza,
                                                    '                                                    Id_Reg,
                                                    '                                                    Inizio,
                                                    '                                                    Fine,
                                                    '                                                    "",
                                                    '                                                    "",
                                                    '                                                    objParametri_Server, objParametri_Utenti,
                                                    '                                                               Hash_FormulatiPA, Hash_FormulatiPAPesi)

                                                    '    'Dim Qta_Pa_Tot As Decimal = 0
                                                    '    Dim Qta_Pa_Ha As Decimal = 0
                                                    '    Dim Qta_Prodotto_Ha As Decimal = 0
                                                    '    Dim Sup As Decimal
                                                    '    Dim TitoloP As Decimal
                                                    '    Dim PesoP As Decimal

                                                    '    For i = 0 To dtLavorazioni.Rows.Count - 1
                                                    '        TitoloP = 0
                                                    '        PesoP = 0
                                                    '        If Not IsDBNull(dtLavorazioni.Rows(i).Item("titolo")) AndAlso CDec(dtLavorazioni.Rows(i).Item("titolo")) > 0 Then
                                                    '            TitoloP = CDec(dtLavorazioni.Rows(i).Item("titolo"))
                                                    '        End If
                                                    '        If Not IsDBNull(dtLavorazioni.Rows(i).Item("peso")) AndAlso CDec(dtLavorazioni.Rows(i).Item("peso")) > 0 Then
                                                    '            PesoP = CDec(dtLavorazioni.Rows(i).Item("peso"))
                                                    '        End If
                                                    '        If Not IsDBNull(dtLavorazioni.Rows(i).Item("sup_trattata")) AndAlso CDbl(dtLavorazioni.Rows(i).Item("sup_trattata")) > 0 Then
                                                    '            Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_trattata"))
                                                    '        Else
                                                    '            Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_imp"))
                                                    '        End If
                                                    '        If Sup <> 0 Then
                                                    '            Qta_Prodotto_Ha = CDbl(dtLavorazioni.Rows(i).Item("qta")) / Sup
                                                    '            If PesoP = 0 Then
                                                    '                Qta_Pa_Ha = Qta_Prodotto_Ha * TitoloP / 100
                                                    '            Else
                                                    '                Qta_Pa_Ha = Qta_Prodotto_Ha * PesoP / 1000
                                                    '            End If
                                                    '            Qta_Pa_Tot += Qta_Pa_Ha
                                                    '        End If
                                                    '    Next

                                                    '    '(19/06/2017 fede) aggiunto al conteggio il rame delle concimazioni
                                                    '    'Dim Cu_Distribuito As Decimal = 0
                                                    '    Dim objDett As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                                                    '    objDett.Leggi_Macroelementi_Distribuiti(0,
                                                    '                0,
                                                    '                0,
                                                    '                0,
                                                    '                Cu_Distribuito,
                                                    '                0, 0, 0, 0, 0,
                                                    '                PIVA,
                                                    '                Sa_Cod,
                                                    '                Appezza,
                                                    '                Id_Reg,
                                                    '                0,
                                                    '                Inizio,
                                                    '                Fine,
                                                    '                0,
                                                    '                objParametri_Server)

                                                    '    Qta_Pa_Tot += Cu_Distribuito

                                                    '    'se sono in inserimento devo aggiungere il rame dell'operazione corrente
                                                    '    If TipoOperazioneDB = 1 AndAlso Not Destinazioni Is Nothing Then
                                                    '        For i = 0 To UBound(Destinazioni, 2) - 1
                                                    '            If Destinazioni(i_Piva, i) = PIVA And
                                                    '            Destinazioni(i_Sa_Cod, i) = Sa_Cod And
                                                    '            Destinazioni(i_Appezza, i) = Appezza And
                                                    '            Destinazioni(i_Id_Reg, i) = Id_Reg And
                                                    '            Destinazioni(i_Pro_Cod, i) = Pro_Cod Then

                                                    '                If CDbl(Destinazioni(i_Sup_Imp, i)) > 0 Then
                                                    '                    Sup = CDbl(Destinazioni(i_Sup_Imp, i))
                                                    '                Else
                                                    '                    Sup = Sup_Imp
                                                    '                End If
                                                    '                If Sup <> 0 Then
                                                    '                    Qta_Prodotto_Ha = CDbl(Destinazioni(i_Qta, i)) / Sup
                                                    '                    If Peso = 0 Then
                                                    '                        Qta_Pa_Ha = Qta_Prodotto_Ha * Titolo / 100
                                                    '                    Else
                                                    '                        Qta_Pa_Ha = Qta_Prodotto_Ha * Peso / 1000
                                                    '                    End If
                                                    '                    Qta_Pa_Tot += Qta_Pa_Ha
                                                    '                End If

                                                    '            End If
                                                    '        Next
                                                    '    End If


                                                    '    If DoseDpi_MaxAnno > 0 Then
                                                    '        Rame_Qta_Max_Kg = DoseDpi_MaxAnno
                                                    '    End If

                                                    '    If Qta_Pa_Tot > Rame_Qta_Max_Kg Then
                                                    '        Err_Code = enTipoErrCode_Verifica.DoseEccessivaRame
                                                    '        If DoseDpi_MaxAnno <> 6 And DoseDpi_MaxAnno <> 4 Then
                                                    '            'If DoseDpi_MaxAnno <> 6 Then
                                                    '            Err_Des = "La dose di rame distribuita (" & Qta_Pa_Tot & " Kg/ha) supera la massima consentita (" & Rame_Qta_Max_Kg & " Kg/Ha all'anno da Deroga)"
                                                    '        Else
                                                    '            Err_Des = "La dose di rame distribuita (" & Qta_Pa_Tot & " Kg/ha) supera la massima consentita (" & Rame_Qta_Max_Kg & " Kg/Ha all'anno)"
                                                    '        End If
                                                    '        Consultazione = "E' consentito utilizzare non più di " & Rame_Qta_Max_Kg & " Kg/ha di rame all'anno. "
                                                    '    End If

                                                End If



                                            Case enTipoWarning_Verifica.DoseRame_5Anni_xBio

                                                If impostazioni.UTENTE_COD_BLOCCA_DOSERAME5ANNIMAX_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    Dim Inizio As Date = CDate("01/01/" & (Data.Year - 4).ToString)
                                                    Dim Fine As Date = CDate("31/12/" & Data.Year.ToString)

                                                    Dim Qta_Pa_Tot As Decimal = 0
                                                    Dim Qta_Pa_Ha As Decimal = 0
                                                    Dim Qta_Prodotto_Ha As Decimal = 0
                                                    Dim Sup As Decimal
                                                    Dim TitoloP As Decimal
                                                    Dim PesoP As Decimal

                                                    Dim dtLavorazioni As DataTable

                                                    Dim ObjContab As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R

                                                    dtLavorazioni = ObjContab.LeggiLavorazioni_Da_Principi_Attivi(PIVA,
                                                                                                    Sa_Cod,
                                                                                                    strPA,
                                                                                                    Appezza,
                                                                                                    Id_Reg,
                                                                                                    Inizio,
                                                                                                    Fine,
                                                                                                    "",
                                                                                                    "",
                                                                                                    objParametri_Server, objParametri_Utenti,
                                                                                                               Hash_FormulatiPA, Hash_FormulatiPAPesi)

                                                    For i = 0 To dtLavorazioni.Rows.Count - 1

                                                        TitoloP = 0
                                                        PesoP = 0
                                                        If Not IsDBNull(dtLavorazioni.Rows(i).Item("titolo")) AndAlso CDec(dtLavorazioni.Rows(i).Item("titolo")) > 0 Then
                                                            TitoloP = CDec(dtLavorazioni.Rows(i).Item("titolo"))
                                                        End If
                                                        If Not IsDBNull(dtLavorazioni.Rows(i).Item("peso")) AndAlso CDec(dtLavorazioni.Rows(i).Item("peso")) > 0 Then
                                                            PesoP = CDec(dtLavorazioni.Rows(i).Item("peso"))
                                                        End If

                                                        If Not IsDBNull(dtLavorazioni.Rows(i).Item("sup_trattata")) AndAlso CDbl(dtLavorazioni.Rows(i).Item("sup_trattata")) > 0 Then
                                                            Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_trattata"))
                                                        Else
                                                            Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_imp"))
                                                        End If
                                                        If Sup <> 0 Then
                                                            Qta_Prodotto_Ha = CDbl(dtLavorazioni.Rows(i).Item("qta")) / Sup
                                                            If PesoP = 0 Then
                                                                Qta_Pa_Ha = Qta_Prodotto_Ha * TitoloP / 100
                                                            Else
                                                                Qta_Pa_Ha = Qta_Prodotto_Ha * PesoP / 1000
                                                            End If
                                                            Qta_Pa_Tot += Qta_Pa_Ha
                                                        End If

                                                    Next

                                                    '(19/06/2017 fede) aggiunto al conteggio il rame delle concimazioni
                                                    Dim Cu_Distribuito As Decimal = 0
                                                    Dim objDett As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                                                    objDett.Leggi_Macroelementi_Distribuiti(0,
                                                                0,
                                                                0,
                                                                0,
                                                                Cu_Distribuito,
                                                                0, 0, 0, 0, 0,
                                                                PIVA,
                                                                Sa_Cod,
                                                                Appezza,
                                                                Id_Reg,
                                                                0,
                                                                Inizio,
                                                                Fine,
                                                                0,
                                                                objParametri_Server)

                                                    Qta_Pa_Tot += Cu_Distribuito

                                                    'se sono in inserimento devo aggiungere il rame dell'operazione corrente
                                                    If TipoOperazioneDB = 1 AndAlso Destinazioni IsNot Nothing Then
                                                        For i = 0 To UBound(Destinazioni, 2) - 1
                                                            If Destinazioni(i_Piva, i) = PIVA AndAlso
                                                               Destinazioni(i_Sa_Cod, i) = Sa_Cod AndAlso
                                                               Destinazioni(i_Appezza, i) = Appezza AndAlso
                                                               Destinazioni(i_Id_Reg, i) = Id_Reg AndAlso
                                                               Destinazioni(i_Pro_Cod, i) = Pro_Cod Then

                                                                If CDbl(Destinazioni(i_Sup_Imp, i)) > 0 Then
                                                                    Sup = CDbl(Destinazioni(i_Sup_Imp, i))
                                                                Else
                                                                    Sup = Sup_Imp
                                                                End If
                                                                If Sup <> 0 Then
                                                                    Qta_Prodotto_Ha = CDbl(Destinazioni(i_Qta, i)) / Sup
                                                                    If Peso = 0 Then
                                                                        Qta_Pa_Ha = Qta_Prodotto_Ha * Titolo / 100
                                                                    Else
                                                                        Qta_Pa_Ha = Qta_Prodotto_Ha * Peso / 1000
                                                                    End If
                                                                    Qta_Pa_Tot += Qta_Pa_Ha
                                                                End If

                                                            End If
                                                        Next
                                                    End If

                                                    If Qta_Pa_Tot / 5 > 6 Then
                                                        Err_Code = enTipoErrCode_Verifica.DoseEccessivaRame5Anni
                                                        Err_Des = String.Format(Gias.DoseRameDistribuitaMedia5AnniSuperaMaxConsentita, (Qta_Pa_Tot / 5))
                                                        'Err_Des = Warning_Des
                                                        Consultazione = Gias.ConsentitoUsareMax6KgRameMediaAnno
                                                    End If

                                                End If

                                            Case enTipoWarning_Verifica.DoseDPI_MaxAnno

                                                If impostazioni.UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else
                                                    Dim Filtro As String = ""
                                                    If TipoOperazioneDB <> 0 AndAlso Destinazioni IsNot Nothing Then
                                                        If Id_Agenda_Escluso <> 0 Then
                                                            Filtro += " Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " "
                                                        End If
                                                    End If

                                                    Dim dtLavorazioni As DataTable

                                                    Dim ObjContab As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R
                                                    dtLavorazioni = ObjContab.LeggiLavorazioni_Da_Principi_Attivi(PIVA,
                                                                                                        Sa_Cod,
                                                                                                        Pa_Cod,
                                                                                                        Appezza,
                                                                                                        Id_Reg,
                                                                                                        Validita_Inizio,
                                                                                                        Validita_Fine,
                                                                                                        Filtro,
                                                                                                        "",
                                                                                                        objParametri_Server, objParametri_Utenti,
                                                                                                                   Hash_FormulatiPA, Hash_FormulatiPAPesi)


                                                    Dim Qta_Pa_Tot As Decimal = 0
                                                    Dim Qta_Pa_Ha As Decimal = 0
                                                    Dim Qta_Prodotto_Ha As Decimal = 0
                                                    Dim Sup As Decimal
                                                    Dim TitoloP As Decimal
                                                    Dim PesoP As Decimal

                                                    For i = 0 To dtLavorazioni.Rows.Count - 1

                                                        TitoloP = 0
                                                        PesoP = 0
                                                        If Not IsDBNull(dtLavorazioni.Rows(i).Item("titolo")) AndAlso CDec(dtLavorazioni.Rows(i).Item("titolo")) > 0 Then
                                                            TitoloP = CDec(dtLavorazioni.Rows(i).Item("titolo"))
                                                        End If
                                                        If Not IsDBNull(dtLavorazioni.Rows(i).Item("peso")) AndAlso CDec(dtLavorazioni.Rows(i).Item("peso")) > 0 Then
                                                            PesoP = CDec(dtLavorazioni.Rows(i).Item("peso"))
                                                        End If

                                                        If Not IsDBNull(dtLavorazioni.Rows(i).Item("sup_trattata")) AndAlso CDbl(dtLavorazioni.Rows(i).Item("sup_trattata")) > 0 Then
                                                            Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_trattata"))
                                                        Else
                                                            Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_imp"))
                                                        End If

                                                        If Sup <> 0 Then
                                                            Qta_Prodotto_Ha = CDbl(dtLavorazioni.Rows(i).Item("qta")) / Sup

                                                            If DoseDpi_MaxAnno_Peso <> 0 AndAlso PesoP <> 0 Then
                                                                Qta_Pa_Ha = Qta_Prodotto_Ha * PesoP / 1000
                                                            Else
                                                                Qta_Pa_Ha = Qta_Prodotto_Ha * TitoloP / 100
                                                            End If

                                                            'If PesoP = 0 Then
                                                            '    Qta_Pa_Ha = Qta_Prodotto_Ha * TitoloP / 100
                                                            'Else
                                                            '    Qta_Pa_Ha = Qta_Prodotto_Ha * PesoP / 1000
                                                            'End If
                                                            Qta_Pa_Tot += Qta_Pa_Ha
                                                        End If

                                                    Next

                                                    'se sono in inserimento devo aggiungere il rame dell'operazione corrente
                                                    'If TipoOperazioneDB = 1 AndAlso Not Destinazioni Is Nothing Then
                                                    If TipoOperazioneDB <> 0 AndAlso Destinazioni IsNot Nothing Then
                                                        For i = 0 To UBound(Destinazioni, 2) - 1
                                                            If Destinazioni(i_Piva, i) = PIVA AndAlso
                                                               Destinazioni(i_Sa_Cod, i) = Sa_Cod AndAlso
                                                               Destinazioni(i_Appezza, i) = Appezza AndAlso
                                                               Destinazioni(i_Id_Reg, i) = Id_Reg AndAlso
                                                               Destinazioni(i_Pro_Cod, i) = Pro_Cod Then

                                                                If CDbl(Destinazioni(i_Sup_Imp, i)) > 0 Then
                                                                    Sup = CDbl(Destinazioni(i_Sup_Imp, i))
                                                                Else
                                                                    Sup = Sup_Imp
                                                                End If

                                                                If Sup <> 0 Then
                                                                    Qta_Prodotto_Ha = CDbl(Destinazioni(i_Qta, i)) / Sup

                                                                    If DoseDpi_MaxAnno_Peso <> 0 AndAlso Peso <> 0 Then
                                                                        Qta_Pa_Ha = Qta_Prodotto_Ha * Peso / 1000
                                                                    Else
                                                                        Qta_Pa_Ha = Qta_Prodotto_Ha * Titolo / 100
                                                                    End If

                                                                    'If Peso = 0 Then
                                                                    '    Qta_Pa_Ha = Qta_Prodotto_Ha * Titolo / 100
                                                                    'Else
                                                                    '    Qta_Pa_Ha = Qta_Prodotto_Ha * Peso / 1000
                                                                    'End If
                                                                    Qta_Pa_Tot += Qta_Pa_Ha
                                                                End If

                                                            End If
                                                        Next
                                                    End If

                                                    'arrotondo x i decimali del ws
                                                    Qta_Pa_Tot = CDec(Format(Qta_Pa_Tot, "###,###.0##"))

                                                    If Qta_Pa_Tot > DoseDpi_MaxAnno Then
                                                        Err_Code = enTipoErrCode_Verifica.DoseEccessivaSADpi_Anno
                                                        Err_Des = Warning_Des
                                                        Consultazione = String.Format(Gias.ConsentitoUsareMaxPaDesAnno, DoseDpi_MaxAnno, Pa_Des) & "</br>" &
                                                            String.Format(Gias.DistribuitiQtaAnno, Format(Qta_Pa_Tot, "##,###,##0.000"))
                                                        Err_Des = Consultazione
                                                    End If

                                                End If

                                                '(02/11/2017 fede) reintrodotte soglie
                                            Case enTipoWarning_Verifica.Soglie

                                                If Not ((impostazioni.UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME = "ND" AndAlso impostazioni.UTENTE_COD_LIVELLO_CHK_DPI = "ND") OrElse
                                                        (impostazioni.UTENTE_COD_LIVELLO_CHK_DPI = "2")) Then
                                                    InserisciWarning = False
                                                Else

                                                    'Controllo se esiste già l'impianto
                                                    Dim bEsiste As Boolean = False
                                                    For i = 0 To UBound(Soglie, 2) - 1
                                                        If Soglie(0, i) = PIVA AndAlso
                                                           Soglie(1, i) = Sa_Cod AndAlso
                                                           Soglie(2, i) = Appezza AndAlso
                                                           Soglie(3, i) = Id_Reg AndAlso
                                                           Soglie(8, i) = Pro_Cod Then

                                                            bEsiste = True
                                                            Exit For

                                                        End If
                                                    Next

                                                    If Not bEsiste Then

                                                        ReDim Preserve Soglie(13, UBound(Soglie, 2) + 1)

                                                        Soglie(0, UBound(Soglie, 2) - 1) = PIVA
                                                        Soglie(1, UBound(Soglie, 2) - 1) = Sa_Cod
                                                        Soglie(2, UBound(Soglie, 2) - 1) = Appezza
                                                        Soglie(3, UBound(Soglie, 2) - 1) = Id_Reg
                                                        Soglie(4, UBound(Soglie, 2) - 1) = Av_Cod
                                                        Soglie(5, UBound(Soglie, 2) - 1) = 0 'Soglia Non Soddisfatta
                                                        Soglie(6, UBound(Soglie, 2) - 1) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                        Soglie(7, UBound(Soglie, 2) - 1) = Warning_Des
                                                        Soglie(8, UBound(Soglie, 2) - 1) = Pro_Cod
                                                        Soglie(9, UBound(Soglie, 2) - 1) = Pro_Des
                                                        Soglie(10, UBound(Soglie, 2) - 1) = Note
                                                        Soglie(11, UBound(Soglie, 2) - 1) = 0 'Media Catture per Trappola
                                                        Soglie(12, UBound(Soglie, 2) - 1) = Lav_Cod_Soglia

                                                        i = UBound(Soglie, 2) - 1

                                                    End If

                                                    If Soglie(5, i) = 0 Then

                                                        Dim Data_Validita_Inizio As Date = Validita_Inizio
                                                        If Offset <> 0 Then
                                                            Data_Validita_Inizio = DateAdd("d", -Offset, Data)
                                                        End If

                                                        Select Case Lav_Cod_Soglia

                                                            Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE

                                                                Dim ObjRilievi As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
                                                                Dim dtRilievi As DataTable

                                                                'Ricerca di un rilievo fatto nel centro rispetta all'avversità e alla specie vegetale
                                                                dtRilievi = ObjRilievi.Leggi_RilieviTrappole(PIVA,
                                                                                                        Sa_Cod,
                                                                                                        0,
                                                                                                        0,
                                                                                                        0,
                                                                                                        Av_Cod,
                                                                                                        Veg_Cod,
                                                                                                        Data_Validita_Inizio,
                                                                                                        Data,
                                                                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                        "",
                                                                                                        "",
                                                                                                        objParametri_Server)

                                                                Qta_Rilievo = 0

                                                                If dtRilievi.Rows.Count > 0 Then

                                                                    Soglie(11, i) = dtRilievi.Rows(0).Item("Qta")

                                                                    Dim iRil As Integer
                                                                    For iRil = 0 To dtRilievi.Rows.Count - 1
                                                                        Qta_Rilievo = Qta_Rilievo + dtRilievi.Rows(iRil).Item("Qta_Ril")
                                                                    Next iRil

                                                                    Qta_Media_Rilievo = (Qta_Rilievo / dtRilievi.Rows.Count)

                                                                    If Not (Qta_Media_Rilievo >= Qta_Soglia) Then
                                                                        Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                        If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                            Soglie(7, i) &= Warning_Des
                                                                        End If
                                                                    Else
                                                                        Soglie(5, i) = 1 'Soglia Soddisfatta --> Impianto Ok
                                                                    End If
                                                                Else
                                                                    Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                    If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                        Soglie(7, i) &= Warning_Des
                                                                    End If
                                                                End If

                                                                '(27/02/2019 fede) aggiunto controllo se è stato registrato il rilievo avv in campo della soglia necessaria
                                                                If Soglie(5, i) = 0 Then

                                                                    dtRilievi = ObjRilievi.Leggi_RilieviAvversita(PIVA,
                                                                                                       Sa_Cod,
                                                                                                       Appezza,
                                                                                                       Id_Reg,
                                                                                                       Udm_Cod_Soglia,
                                                                                                       Av_Cod,
                                                                                                       Data_Validita_Inizio,
                                                                                                       Data,
                                                                                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                       "",
                                                                                                       "Mov_Destinazioni.Validita_Inizio Desc, Mov_Destinazioni.Qta Desc",
                                                                                                       objParametri_Server)

                                                                    If dtRilievi.Rows.Count > 0 Then

                                                                        'Gestione Presenza
                                                                        If CDbl(Qta_Soglia) = 0 Then
                                                                            Qta_Soglia = 0.5 'Presenza
                                                                        End If

                                                                        Soglie(11, i) = dtRilievi.Rows(0).Item("Qta")

                                                                        If Not (CDbl(dtRilievi.Rows(0).Item("Qta")) >= Qta_Soglia) Then
                                                                            Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                            If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                                Soglie(7, i) &= Warning_Des
                                                                            End If
                                                                        Else
                                                                            Soglie(5, i) = 1 'Soglia Soddisfatta --> Impianto Ok
                                                                        End If

                                                                    Else
                                                                        Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                        If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                            Soglie(7, i) &= Warning_Des
                                                                        End If
                                                                    End If

                                                                End If



                                                            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO

                                                                Dim ObjRilievi As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
                                                                Dim dtRilievi As DataTable

                                                                'Lettura Rilievi Precedenti
                                                                dtRilievi = ObjRilievi.Leggi_RilieviAvversita(PIVA,
                                                                                                        Sa_Cod,
                                                                                                        Appezza,
                                                                                                        Id_Reg,
                                                                                                        Udm_Cod_Soglia,
                                                                                                        Av_Cod,
                                                                                                        Data_Validita_Inizio,
                                                                                                        Data,
                                                                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                        "",
                                                                                                        "Mov_Destinazioni.Validita_Inizio Desc, Mov_Destinazioni.Qta Desc",
                                                                                                        objParametri_Server)

                                                                If dtRilievi.Rows.Count > 0 Then

                                                                    'Gestione Presenza
                                                                    If CDbl(Qta_Soglia) = 0 Then
                                                                        Qta_Soglia = 0.5 'Presenza
                                                                    End If

                                                                    Soglie(11, i) = dtRilievi.Rows(0).Item("Qta")

                                                                    If Not (CDbl(dtRilievi.Rows(0).Item("Qta")) >= Qta_Soglia) Then
                                                                        Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                        If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                            Soglie(7, i) &= Warning_Des
                                                                        End If
                                                                    Else
                                                                        Soglie(5, i) = 1 'Soglia Soddisfatta --> Impianto Ok
                                                                    End If

                                                                Else
                                                                    Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                    If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                        Soglie(7, i) &= Warning_Des
                                                                    End If
                                                                End If

                                                                '(27/02/2019 fede) aggiunto controllo se è stato registrato il rilievo avv in campo della soglia necessaria
                                                                If Soglie(5, i) = 0 Then

                                                                    'Ricerca di un rilievo fatto nel centro rispetta all'avversità e alla specie vegetale
                                                                    dtRilievi = ObjRilievi.Leggi_RilieviTrappole(PIVA,
                                                                                                            Sa_Cod,
                                                                                                            0,
                                                                                                            0,
                                                                                                            0,
                                                                                                            Av_Cod,
                                                                                                            Veg_Cod,
                                                                                                            Data_Validita_Inizio,
                                                                                                            Data,
                                                                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                            "",
                                                                                                            "",
                                                                                                            objParametri_Server)

                                                                    Qta_Rilievo = 0

                                                                    If dtRilievi.Rows.Count > 0 Then

                                                                        Soglie(11, i) = dtRilievi.Rows(0).Item("Qta")

                                                                        Dim iRil As Integer
                                                                        For iRil = 0 To dtRilievi.Rows.Count - 1
                                                                            Qta_Rilievo = Qta_Rilievo + dtRilievi.Rows(iRil).Item("Qta_Ril")
                                                                        Next iRil

                                                                        Qta_Media_Rilievo = (Qta_Rilievo / dtRilievi.Rows.Count)

                                                                        If Not (Qta_Media_Rilievo >= Qta_Soglia) Then
                                                                            Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                            If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                                Soglie(7, i) &= Warning_Des
                                                                            End If
                                                                        Else
                                                                            Soglie(5, i) = 1 'Soglia Soddisfatta --> Impianto Ok
                                                                        End If
                                                                    Else
                                                                        Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                        If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                            Soglie(7, i) &= Warning_Des
                                                                        End If
                                                                    End If

                                                                End If

                                                                '(13/03/2019 fede) aggiunto controllo registrata giustificazione richiesta
                                                            Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE

                                                                Dim ObjRilievi As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
                                                                Dim dtRilievi As DataTable

                                                                dtRilievi = ObjRilievi.Leggi_ConfusioneDisorientamentoSessuale(PIVA,
                                                                                                        Sa_Cod,
                                                                                                        Appezza,
                                                                                                        Id_Reg,
                                                                                                        Lav_Cod_Soglia,
                                                                                                        Av_Cod,
                                                                                                        Data_Validita_Inizio,
                                                                                                        Data,
                                                                                                        "",
                                                                                                        "Mov_Destinazioni.Validita_Inizio Desc, Mov_Destinazioni.Qta Desc",
                                                                                                        objParametri_Server)

                                                                If dtRilievi.Rows.Count > 0 Then

                                                                    ''Gestione Presenza
                                                                    'If CDbl(Qta_Soglia) = 0 Then
                                                                    '    Qta_Soglia = 0.5 'Presenza
                                                                    'End If

                                                                    'Soglie(11, i) = dtRilievi.Rows(0).Item("Qta")

                                                                    'If Not (CDbl(dtRilievi.Rows(0).Item("Qta")) >= Qta_Soglia) Then
                                                                    '    Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                    '    If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                    '        Soglie(7, i) &= Warning_Des
                                                                    '    End If
                                                                    'Else
                                                                    Soglie(5, i) = 1 'Soglia Soddisfatta --> Impianto Ok
                                                                    'End If

                                                                Else
                                                                    Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                    If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                        Soglie(7, i) &= Warning_Des
                                                                    End If
                                                                End If


                                                        End Select

                                                    End If

                                                End If


                                            Case enTipoWarning_Verifica.Dose_MaxAnno_GPAI

                                                If impostazioni.UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO = "ND" OrElse impostazioni.UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    Dim iPa_Cod As Integer = 0
                                                    Dim ArrayPA As String()
                                                    Dim bOk As Boolean = False

                                                    ArrayPA = Split(strPA & ",", ",")

                                                    'Scorro tutti i prodotti alla ricerca della compatibilità
                                                    For Each key In HashPrincipiOperazione.Keys

                                                        Dim PaCodTmp As Integer = Split(key, "_")(1)
                                                        For iPa_Cod = 0 To UBound(ArrayPA) - 1

                                                            If IsNumeric(ArrayPA(iPa_Cod)) Then

                                                                If CInt(ArrayPA(iPa_Cod)) = PaCodTmp Then
                                                                    bOk = True
                                                                End If

                                                            End If

                                                        Next

                                                    Next

                                                    If bOk Then

                                                        Dim FiltroPA As String = ""
                                                        Dim Str_FiltroAggiuntivo As New StringBuilder

                                                        Str_FiltroAggiuntivo.Append(" Movimenti_dettagli.Pro_Cod=" & Pro_Cod & "  ")

                                                        'se sono in modifica devo escludere l'id_agenda già salvato dal conteggio (lo aggiungo dopo)
                                                        If TipoOperazioneDB <> 0 AndAlso Destinazioni IsNot Nothing Then
                                                            If Id_Agenda_Escluso <> 0 Then
                                                                Str_FiltroAggiuntivo.Append(" And Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " ")
                                                            End If
                                                        End If

                                                        Dim ObjContab As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                                                        Dim dtLavorazioni As DataTable = ObjContab.Leggi_TrattamentiImpiantoEsteso(PIVA, Sa_Cod, Appezza, Id_Reg,
                                                                                                                                           "", "",
                                                                                                                                            Validita_Inizio, Validita_Fine,
                                                                                                                                            Str_FiltroAggiuntivo.ToString,
                                                                                                                                            "",
                                                                                                                                            objParametri_Server)



                                                        'Determinazione Parametri Dose DPI
                                                        Parametri_Dose(DoseDpi_MaxAnno_Udm_Cod, Udm_Cod_DPI, Coefficiente_DPI, Udm_Des_DPI, Udm_Des_Coefficiente_DPI)

                                                        If Coefficiente_DPI = -1 Then

                                                            'Errore: Udm DPI non Gestita --> Bypass
                                                            '
                                                        Else

                                                            Dose_Totale_Lavorazione = 0 'Reset
                                                            DoseDpi_MaxAnno = Coefficiente_DPI * DoseDpi_MaxAnno

                                                            'Calcolo Dose Totale su Dati DB
                                                            If dtLavorazioni IsNot Nothing AndAlso dtLavorazioni.Rows.Count > 0 Then

                                                                For Each drTmp As DataRow In dtLavorazioni.Rows

                                                                    'Determinazione Parametri Lavorazione
                                                                    Parametri_Dose(drTmp("Extra_Int"), Udm_Cod_Lavorazione, Coefficiente_Lavorazione, Udm_Des_Lavorazione, Udm_Des_Coefficiente_Lavorazione)

                                                                    'Calcolo Dose Lavorazione
                                                                    Dose_Lavorazione = Coefficiente_Lavorazione * drTmp("Qta_Dose")

                                                                    'Sommo la dose
                                                                    Dose_Totale_Lavorazione = Dose_Totale_Lavorazione + Dose_Lavorazione

                                                                Next

                                                            End If

                                                            'Controllo Operazione in Modifica
                                                            If TipoOperazioneDB <> 0 AndAlso Destinazioni IsNot Nothing Then

                                                                For i = 0 To UBound(Destinazioni, 2) - 1
                                                                    If Destinazioni(i_Piva, i) = PIVA AndAlso
                                                                       Destinazioni(i_Sa_Cod, i) = Sa_Cod AndAlso
                                                                       Destinazioni(i_Appezza, i) = Appezza AndAlso
                                                                       Destinazioni(i_Id_Reg, i) = Id_Reg AndAlso
                                                                       Destinazioni(i_Pro_Cod, i) = Pro_Cod Then

                                                                        'Determinazione Parametri Lavorazione
                                                                        Parametri_Dose(Destinazioni(i_Extra_Int, i), Udm_Cod_Lavorazione, Coefficiente_Lavorazione, Udm_Des_Lavorazione, Udm_Des_Coefficiente_Lavorazione)

                                                                        'Calcolo Dose Lavorazione
                                                                        Dose_Lavorazione = Coefficiente_Lavorazione * Destinazioni(i_Dose, i)

                                                                        'Sommo la dose
                                                                        Dose_Totale_Lavorazione = Dose_Totale_Lavorazione + Dose_Lavorazione

                                                                        Exit For

                                                                    End If

                                                                Next i

                                                            End If

                                                            'Controllo Superamento Dose Annuale DPI
                                                            If Dose_Totale_Lavorazione > DoseDpi_MaxAnno AndAlso Err_Code = 0 Then

                                                                Err_Code = enTipoErrCode_Verifica.DoseEccessivaSADpi_Anno
                                                                Consultazione = Warning_Des & " " &
                                                                                "Ne sono stati distribuiti " & Format(Dose_Totale_Lavorazione, "##,###,##0.000") & " " & Udm_Des_Coefficiente_Lavorazione & " " & Dose_Totale_Lavorazione & ". "

                                                                Err_Des = Consultazione

                                                            End If

                                                        End If

                                                    End If

                                                End If





                                            Case enTipoWarning_Verifica.Dose_MaxAnno

                                                If impostazioni.UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO = "ND" OrElse impostazioni.UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    Max_Interventi_Udm_Des = " " & Gias.AllAnno

                                                    Dim Str_FiltroAggiuntivo As New StringBuilder

                                                    Str_FiltroAggiuntivo.Append(" Movimenti_dettagli.Pro_Cod=" & Pro_Cod & "  ")

                                                    'se sono in modifica devo escludere l'id_agenda già salvato dal conteggio (lo aggiungo dopo)
                                                    If TipoOperazioneDB <> 0 AndAlso Destinazioni IsNot Nothing Then
                                                        If Id_Agenda_Escluso <> 0 Then
                                                            Str_FiltroAggiuntivo.Append(" And Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " ")
                                                        End If
                                                    End If

                                                    Dim ObjContab As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                                                    Dim dtLavorazioni As DataTable = ObjContab.Leggi_TrattamentiImpiantoEsteso(PIVA, Sa_Cod, Appezza, Id_Reg,
                                                                                                                                       "", "",
                                                                                                                                        Validita_Inizio, Validita_Fine,
                                                                                                                                        Str_FiltroAggiuntivo.ToString,
                                                                                                                                        "",
                                                                                                                                        objParametri_Server)



                                                    'Determinazione Parametri Dose Etichetta
                                                    Parametri_Dose(DoseEtichetta_MaxAnno_Udm_Cod, Udm_Cod_Etichetta, Coefficiente_Etichetta, Udm_Des_Etichetta, Udm_Des_Coefficiente_Etichetta)

                                                    If Coefficiente_Etichetta = -1 Then

                                                        'Errore: Udm Etichetta non Gestita --> Bypass
                                                        '
                                                    Else

                                                        Dose_Totale_Lavorazione = 0 'Reset
                                                        Dose_Etichetta = Coefficiente_Etichetta * DoseEtichetta_MaxAnno

                                                        'Calcolo Dose Totale su Dati DB
                                                        If dtLavorazioni IsNot Nothing AndAlso dtLavorazioni.Rows.Count > 0 Then

                                                            For Each drTmp As DataRow In dtLavorazioni.Rows

                                                                'Determinazione Parametri Lavorazione
                                                                Parametri_Dose(drTmp("Extra_Int"), Udm_Cod_Lavorazione, Coefficiente_Lavorazione, Udm_Des_Lavorazione, Udm_Des_Coefficiente_Lavorazione)

                                                                'Calcolo Dose Lavorazione
                                                                Dose_Lavorazione = Coefficiente_Lavorazione * drTmp("Qta_Dose")

                                                                If Udm_Cod_Etichetta <> Udm_Cod_Lavorazione Then

                                                                    'Errore: Udm Lavorazione Non Compatibile con Etichetta
                                                                    Err_Code = enTipoErrCode_Verifica.DoseEccessivaEtichetta_Anno
                                                                    'Err_Des = Warning_Des
                                                                    If drTmp("Id_Agenda") = Id_Agenda_Escluso Then
                                                                        Consultazione = String.Format(Gias.UDMNonConsentita, Udm_Des_Lavorazione)
                                                                    Else
                                                                        Consultazione = String.Format(Gias.UDMNonConsentitaPerOperazioneDel, Udm_Des_Coefficiente_Lavorazione, drTmp("Des_Lib"), Format(drTmp("Data_Movimento"), "dd/MM/yyyy"))
                                                                    End If

                                                                    Consultazione = Consultazione & String.Format(Gias.UDMConsentitaDaEtichettaUDMDes, Udm_Des_Coefficiente_Etichetta)
                                                                    Err_Des = Consultazione

                                                                Else

                                                                    'Sommo la dose
                                                                    Dose_Totale_Lavorazione = Dose_Totale_Lavorazione + Dose_Lavorazione

                                                                End If

                                                            Next

                                                        End If

                                                        'Controllo Operazione in Modifica
                                                        If TipoOperazioneDB <> 0 AndAlso Destinazioni IsNot Nothing Then

                                                            For i = 0 To UBound(Destinazioni, 2) - 1
                                                                If Destinazioni(i_Piva, i) = PIVA AndAlso
                                                                   Destinazioni(i_Sa_Cod, i) = Sa_Cod AndAlso
                                                                   Destinazioni(i_Appezza, i) = Appezza AndAlso
                                                                   Destinazioni(i_Id_Reg, i) = Id_Reg AndAlso
                                                                   Destinazioni(i_Pro_Cod, i) = Pro_Cod Then

                                                                    'Determinazione Parametri Lavorazione
                                                                    Parametri_Dose(Destinazioni(i_Extra_Int, i), Udm_Cod_Lavorazione, Coefficiente_Lavorazione, Udm_Des_Lavorazione, Udm_Des_Coefficiente_Lavorazione)

                                                                    'Calcolo Dose Lavorazione
                                                                    Dose_Lavorazione = Coefficiente_Lavorazione * Destinazioni(i_Dose, i)

                                                                    If Udm_Cod_Etichetta <> Udm_Cod_Lavorazione Then

                                                                        'Errore: Udm Lavorazione Non Compatibile con Etichetta
                                                                        Err_Code = enTipoErrCode_Verifica.DoseEccessivaEtichetta_Anno
                                                                        'Err_Des = Warning_Des
                                                                        Consultazione = String.Format(Gias.UDMNonConsentita, Udm_Des_Lavorazione)
                                                                        Consultazione = Consultazione & String.Format(Gias.UDMConsentitaDaEtichettaUDMDes, Udm_Des_Etichetta)
                                                                        Err_Des = Consultazione

                                                                    Else

                                                                        'Sommo la dose
                                                                        Dose_Totale_Lavorazione = Dose_Totale_Lavorazione + Dose_Lavorazione

                                                                    End If

                                                                    Exit For

                                                                End If

                                                            Next i

                                                        End If

                                                        'Controllo Superamento Dose Annuale Etichetta
                                                        If Dose_Totale_Lavorazione > Dose_Etichetta AndAlso Err_Code = 0 Then

                                                            Err_Code = enTipoErrCode_Verifica.DoseEccessivaEtichetta_Anno
                                                            'Err_Des = Warning_Des
                                                            Consultazione = "E' consentito utilizzare non più di " & DoseEtichetta_MaxAnno & " " & Udm_Des_Etichetta & " " & Max_Interventi_Udm_Des & " " &
                                                                            "Ne sono stati distribuiti " & Format(Dose_Totale_Lavorazione, "##,###,##0.000") & " " & Udm_Des_Coefficiente_Lavorazione & " " & Max_Interventi_Udm_Des & " "

                                                            Err_Des = Consultazione

                                                        End If

                                                    End If

                                                End If


                                            Case enTipoWarning_Verifica.Dose_MaxAnno_xAvversita_Infestante

                                                'DA TESTARE PER CASISTICA MANCANTE DA BANCHE DATI!!!

                                                If impostazioni.UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO = "ND" OrElse impostazioni.UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    Max_Interventi_Udm_Des = " all'anno"

                                                    Dim Str_FiltroAggiuntivo As New StringBuilder

                                                    Str_FiltroAggiuntivo.Append(" Movimenti_dettagli.Pro_Cod=" & Pro_Cod & "  ")

                                                    'se sono in modifica devo escludere l'id_agenda già salvato dal conteggio (lo aggiungo dopo)
                                                    If TipoOperazioneDB <> 0 AndAlso Destinazioni IsNot Nothing Then
                                                        If Id_Agenda_Escluso <> 0 Then
                                                            Str_FiltroAggiuntivo.Append(" And Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " ")
                                                        End If
                                                    End If

                                                    Dim FiltroAvCod As String = ""
                                                    Dim FiltroAvGru As String = ""

                                                    If Av_Cod <> 0 Then
                                                        FiltroAvCod = "(" & Av_Cod & ")"
                                                    End If
                                                    If Av_Gru <> 0 Then
                                                        FiltroAvGru = "(" & Av_Gru & ")"
                                                    End If


                                                    Dim ObjContab As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                                                    Dim dtLavorazioni As DataTable = ObjContab.Leggi_TrattamentiImpiantoEsteso(PIVA, Sa_Cod, Appezza, Id_Reg,
                                                                                                                                   FiltroAvCod, FiltroAvGru,
                                                                                                                                   Validita_Inizio, Validita_Fine,
                                                                                                                                   Str_FiltroAggiuntivo.ToString,
                                                                                                                                   "",
                                                                                                                                   objParametri_Server)






                                                    'Determinazione Parametri Dose Etichetta
                                                    Parametri_Dose(DoseEtichetta_MaxAnno_Udm_Cod, Udm_Cod_Etichetta, Coefficiente_Etichetta, Udm_Des_Etichetta, Udm_Des_Coefficiente_Etichetta)

                                                    If Coefficiente_Etichetta = -1 Then

                                                        'Errore: Udm Etichetta non Gestita --> Bypass
                                                        '
                                                    Else

                                                        Dose_Totale_Lavorazione = 0 'Reset
                                                        Dose_Etichetta = Coefficiente_Etichetta * DoseEtichetta_MaxAnno

                                                        'Calcolo Dose Totale su Dati DB
                                                        If dtLavorazioni IsNot Nothing AndAlso dtLavorazioni.Rows.Count > 0 Then

                                                            For Each drTmp As DataRow In dtLavorazioni.Rows

                                                                'Determinazione Parametri Lavorazione
                                                                Parametri_Dose(drTmp("Extra_Int"), Udm_Cod_Lavorazione, Coefficiente_Lavorazione, Udm_Des_Lavorazione, Udm_Des_Coefficiente_Lavorazione)

                                                                'Calcolo Dose Lavorazione
                                                                Dose_Lavorazione = Coefficiente_Lavorazione * drTmp("Qta_Dose")

                                                                If Udm_Cod_Etichetta <> Udm_Cod_Lavorazione Then

                                                                    'Errore: Udm Lavorazione Non Compatibile con Etichetta
                                                                    Err_Code = enTipoErrCode_Verifica.DoseEccessivaEtichettaAvversita_Anno
                                                                    'Err_Des = Warning_Des
                                                                    If drTmp("Id_Agenda") = Id_Agenda_Escluso Then
                                                                        Consultazione = String.Format(Gias.UDMNonConsentita, Udm_Des_Lavorazione)
                                                                    Else
                                                                        Consultazione = String.Format(Gias.UDMNonConsentitaPerOperazioneDel, Udm_Des_Coefficiente_Lavorazione, drTmp("Des_Lib"), Format(drTmp("Data_Movimento"), "dd/MM/yyyy"))
                                                                    End If

                                                                    Consultazione = Consultazione & String.Format(Gias.UDMConsentitaDaEtichettaUDMDes, Udm_Des_Etichetta)
                                                                    Err_Des = Consultazione

                                                                Else

                                                                    'Sommo la dose
                                                                    Dose_Totale_Lavorazione = Dose_Totale_Lavorazione + Dose_Lavorazione

                                                                End If

                                                            Next

                                                        End If

                                                        'Controllo Operazione in Modifica
                                                        If TipoOperazioneDB <> 0 AndAlso Destinazioni IsNot Nothing Then

                                                            For i = 0 To UBound(Destinazioni, 2) - 1
                                                                If Destinazioni(i_Piva, i) = PIVA AndAlso
                                                                   Destinazioni(i_Sa_Cod, i) = Sa_Cod AndAlso
                                                                   Destinazioni(i_Appezza, i) = Appezza AndAlso
                                                                   Destinazioni(i_Id_Reg, i) = Id_Reg AndAlso
                                                                   Destinazioni(i_Pro_Cod, i) = Pro_Cod Then

                                                                    'Determinazione Parametri Lavorazione
                                                                    Parametri_Dose(Destinazioni(i_Extra_Int, i), Udm_Cod_Lavorazione, Coefficiente_Lavorazione, Udm_Des_Lavorazione, Udm_Des_Coefficiente_Lavorazione)

                                                                    'Calcolo Dose Lavorazione
                                                                    Dose_Lavorazione = Coefficiente_Lavorazione * Destinazioni(i_Dose, i)

                                                                    If Udm_Cod_Etichetta <> Udm_Cod_Lavorazione Then

                                                                        'Errore: Udm Lavorazione Non Compatibile con Etichetta
                                                                        Err_Code = enTipoErrCode_Verifica.DoseEccessivaEtichettaAvversita_Anno
                                                                        'Err_Des = Warning_Des
                                                                        Consultazione = String.Format(Gias.UDMNonConsentita, Udm_Des_Lavorazione)
                                                                        Consultazione = Consultazione & String.Format(Gias.UDMConsentitaDaEtichettaUDMDes, Udm_Des_Coefficiente_Etichetta)
                                                                        Err_Des = Consultazione

                                                                    Else

                                                                        'Sommo la dose
                                                                        Dose_Totale_Lavorazione = Dose_Totale_Lavorazione + Dose_Lavorazione

                                                                    End If


                                                                    Exit For

                                                                End If

                                                            Next i

                                                        End If

                                                        'Controllo Superamento Dose Annuale Etichetta
                                                        If Dose_Totale_Lavorazione > Dose_Etichetta AndAlso Err_Code = 0 Then

                                                            Err_Code = enTipoErrCode_Verifica.DoseEccessivaEtichettaAvversita_Anno
                                                            'Err_Des = Warning_Des
                                                            Consultazione = "E' consentito utilizzare non più di " & DoseEtichetta_MaxAnno & " " & Udm_Des_Etichetta & " " & Max_Interventi_Udm_Des & " contro l'avversità " & strAvversita & ". " &
                                                                            "Ne sono stati distribuiti " & Format(Dose_Totale_Lavorazione, "##,###,##0.000") & " " & Udm_Des_Lavorazione & " " & Max_Interventi_Udm_Des & ". "
                                                            Err_Des = Consultazione

                                                        End If

                                                    End If

                                                End If

                                        End Select










                                        '..........


                                End Select







                                '============================================================================
                                Select Case Err_Code

                                    Case 0 'Verde (Warning Conforme)

                                        'Do Nothing

                                    Case Else 'Rosso (Warning Non Conforme)

                                        'Inserimento Dato Non Conforme in Stringa Risultato

                                        HashErrori.Add(N_Warning & "|" & Warning_Code, "")

                                        XmlDatoNonConformeFinale = XmlDoc.CreateElement("DatoNonConforme")
                                        XmlDatoNonConformeFinale.SetAttribute("err_code", Err_Code)
                                        XmlDatoNonConformeFinale.SetAttribute("err_des", Err_Des)
                                        XmlDatoNonConformeFinale.SetAttribute("consultazione", Consultazione)
                                        XmlDatoNonConformeFinale.SetAttribute("pro_cod", Pro_Cod)
                                        XmlDatoNonConformeFinale.SetAttribute("pro_des", Pro_Des)
                                        XmlDatoNonConformeFinale.SetAttribute("mat_cod", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("udm_cod", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("dose", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("dose_consentita", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("appezza", Appezza)
                                        XmlDatoNonConformeFinale.SetAttribute("id_reg", Id_Reg)
                                        XmlDatoNonConformeFinale.SetAttribute("strpa_cod", strPA)
                                        XmlDatoNonConformeFinale.SetAttribute("interventi", Interventi_Scheda & " ")
                                        XmlDatoNonConformeFinale.SetAttribute("interventi_consentiti", Max_Interventi & " ")
                                        XmlDatoNonConformeFinale.SetAttribute("n", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("n_distribuito", "")
                                        XmlDatoNonConformeFinale.SetAttribute("p", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("p_distribuito", "")
                                        XmlDatoNonConformeFinale.SetAttribute("k", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("k_distribuito", "")
                                        XmlDatoNonConformeFinale.SetAttribute("mg", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("mg_distribuito", "")
                                        XmlDatoNonConformeFinale.SetAttribute("av_gru", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("av_cod", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("avversita_des", strAvversita)

                                        XmlDatiNonConformiFinali.AppendChild(XmlDatoNonConformeFinale)

                                        XmlDatoNonConformeFinale = Nothing

                                        InterventoConforme = False

                                End Select
                                '============================================================================



                            End If


                            i_DatoWarning = i_DatoWarning + 1

                        Loop

                        i_DatiWarnings = i_DatiWarnings + 1

                    Loop


                    '=============================================================================
                    'Ricerca Soglie Non Raggiunte
                    '-----------------------------------------------------------------------------
                    For i = 0 To UBound(Soglie, 2) - 1

                        If Soglie(5, i) = 0 Then

                            XmlDatoNonConformeFinale = XmlDoc.CreateElement("DatoNonConforme")
                            XmlDatoNonConformeFinale.SetAttribute("err_code", Soglie(6, i))

                            'Completamento Stringa Errore
                            If Agro_SQL_SaveNum(Soglie(11, i)) = 0 Then
                                Soglie(7, i) = Soglie(7, i) & " " & Gias.NonRisultaGiustificazioneIntervento
                            Else
                                Select Case Soglie(12, i)
                                    Case 110 'Avversità nelle Trappole
                                        Soglie(7, i) = Soglie(7, i) & " " & String.Format(Gias.MediaCattureTrappolaPariA, Soglie(11, i))
                                    Case 113 'Avversità in Campo
                                        Soglie(7, i) = Soglie(7, i) & " " & String.Format(Gias.SogliaRilevataPariA, Soglie(11, i))
                                End Select
                            End If

                            XmlDatoNonConformeFinale.SetAttribute("err_des", Soglie(7, i))
                            '==================================================================================================
                            XmlDatoNonConformeFinale.SetAttribute("consultazione", Soglie(10, i))
                            XmlDatoNonConformeFinale.SetAttribute("pro_cod", Soglie(8, i))
                            XmlDatoNonConformeFinale.SetAttribute("pro_des", Soglie(9, i))
                            XmlDatoNonConformeFinale.SetAttribute("mat_cod", 0)
                            XmlDatoNonConformeFinale.SetAttribute("udm_cod", 0)
                            XmlDatoNonConformeFinale.SetAttribute("dose", 0)
                            XmlDatoNonConformeFinale.SetAttribute("dose_consentita", 0)
                            XmlDatoNonConformeFinale.SetAttribute("appezza", Soglie(2, i))
                            XmlDatoNonConformeFinale.SetAttribute("id_reg", Soglie(3, i))
                            XmlDatoNonConformeFinale.SetAttribute("strpa_cod", "")
                            XmlDatoNonConformeFinale.SetAttribute("interventi", "")
                            XmlDatoNonConformeFinale.SetAttribute("interventi_consentiti", "")
                            XmlDatoNonConformeFinale.SetAttribute("n", 0)
                            XmlDatoNonConformeFinale.SetAttribute("n_distribuito", "")
                            XmlDatoNonConformeFinale.SetAttribute("p", 0)
                            XmlDatoNonConformeFinale.SetAttribute("p_distribuito", "")
                            XmlDatoNonConformeFinale.SetAttribute("k", 0)
                            XmlDatoNonConformeFinale.SetAttribute("k_distribuito", "")
                            XmlDatoNonConformeFinale.SetAttribute("mg", 0)
                            XmlDatoNonConformeFinale.SetAttribute("mg_distribuito", "")
                            XmlDatoNonConformeFinale.SetAttribute("av_gru", 0)
                            XmlDatoNonConformeFinale.SetAttribute("av_cod", Soglie(4, i))
                            XmlDatoNonConformeFinale.SetAttribute("avversita_des", "")

                            XmlDatiNonConformiFinali.AppendChild(XmlDatoNonConformeFinale)

                            XmlDatoNonConformeFinale = Nothing

                            InterventoConforme = False

                        End If

                    Next i

                    '=============================================================================
                    'Ricerca Dati Diserbo
                    '-----------------------------------------------------------------------------
                    xDatiDiserbo = xDatiGenerale.GetElementsByTagName("DatiDiserbo")

                    'Ne esiste solamente 1
                    If xDatiDiserbo.Count > 0 Then

                        xDatoDiserbo = xDatiDiserbo.Item(0)

                        XML_Nodo = XmlDoc.ImportNode(xDatoDiserbo, True)
                        XmlDatiGeneraliFinali.AppendChild(XML_Nodo)
                        'XmlDatiGeneraliFinali.AppendChild(xDatoDiserbo)

                    End If

                    '=============================================================================

                    i_DatiGenerali = i_DatiGenerali + 1

                Loop

                i_DatiRisultati = i_DatiRisultati + 1

            Loop

            XmlDatiGeneraliFinali.AppendChild(XmlDatiNonConformiFinali)
            XmlDatiGeneraliFinali.AppendChild(XmlWarningFinali)
            XmlDatiGeneraliFinali.AppendChild(XmlNoteFinali)
            XmlDatiRisultatiFinali.AppendChild(XmlDatiGeneraliFinali)

            XmlDoc.AppendChild(XmlDatiRisultatiFinali)


            r.RispostaOK = True
            r.RispostaStringa.Risultato = XmlDoc.OuterXml
            r.RispostaStringa.Conforme = InterventoConforme

            '(10/08/2020 fede nuovi controlli tutti gli app assieme)
            If IDTestataTemp > 0 Then
                PulisciTabella__Tmp_Movimenti_Destinazioni_DateDistinta(IDTestataTemp, objParametri_Server)
            End If


            'Distruggo gli oggetti
            XmlDom = Nothing
            xDatiRisultati = Nothing
            xDatiRisultato = Nothing
            xDatiGenerali = Nothing
            xDatiGenerale = Nothing
            xDatiWarnings = Nothing
            xDatoWarning = Nothing

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = ex.Message


            Dim StrDummy As String

            ''Messaggio di errore
            StrDummy = ex.Message.ToString()
            If ex.InnerException IsNot Nothing Then
                StrDummy = StrDummy + " InnerException:" + ex.InnerException.Message.ToString()
            End If

            Scrivi_LOG(objParametri_Server, "Dpi_Verifica.XMLFinale()", StrDummy)

        End Try

        Return r

    End Function

    Private Shared Sub Parametri_Dose(ByVal Dose_Udm_Cod As Integer,
                                      ByRef Udm_Cod As Integer,
                                      ByRef Dose_Coefficiente As Decimal,
                                      ByRef Udm_Des As String,
                                      ByRef Udm_Des_Coefficiente As String)

        Select Case Dose_Udm_Cod

            Case 2 'Kg
                Udm_Cod = 2
                Dose_Coefficiente = 1
                Udm_Des = "Kg/Ha"
                Udm_Des_Coefficiente = "Kg/ha"

            Case 29, 318 'l/Ha
                Udm_Cod = 29
                Dose_Coefficiente = 1
                Udm_Des = "l/hl"
                Udm_Des_Coefficiente = "l/ha"

            Case 23 'g/hl
                Udm_Cod = 2
                Dose_Coefficiente = 1 / 1000
                Udm_Des = "g/hl"
                Udm_Des_Coefficiente = "Kg/ha"

            Case 22 'l/ha
                Udm_Cod = 29
                Dose_Coefficiente = 1
                Udm_Des = "l/ha"
                Udm_Des_Coefficiente = "l/ha"

            Case 175 'Kg/hl
                Udm_Cod = 2
                Dose_Coefficiente = 1
                Udm_Des = "Kg/hl"
                Udm_Des_Coefficiente = "Kg/ha"

            Case 20 'g/ha
                Udm_Cod = 2
                Dose_Coefficiente = 1 / 1000
                Udm_Des = "g/ha"
                Udm_Des_Coefficiente = "Kg/ha"

            Case 302 'l/mq
                Udm_Cod = 29
                Dose_Coefficiente = 10000
                Udm_Des = "l/mq"
                Udm_Des_Coefficiente = "l/ha"

            Case 173 'l/hl
                Udm_Cod = 29
                Dose_Coefficiente = 1
                Udm_Des = "l/hl"
                Udm_Des_Coefficiente = "l/ha"

            Case 88 'Kg/ha
                Udm_Cod = 2
                Dose_Coefficiente = 1
                Udm_Des = "Kg/ha"
                Udm_Des_Coefficiente = "Kg/ha"

            Case 163 'ml/ha
                Udm_Cod = 29
                Dose_Coefficiente = 1 / 1000
                Udm_Des = "ml/ha"
                Udm_Des_Coefficiente = "l/ha"

            Case 171 'g/mq
                Udm_Cod = 29
                Dose_Coefficiente = 10
                Udm_Des = "g/mq"
                Udm_Des_Coefficiente = "Kg/ha"

            Case 164, 101 'ml/hl
                Udm_Cod = 29
                Dose_Coefficiente = 1 / 1000
                Udm_Des = "ml/hl"
                Udm_Des_Coefficiente = "l/ha"

            Case 172 'ml/mq
                Udm_Cod = 29
                Dose_Coefficiente = 10
                Udm_Des = "ml/mq"
                Udm_Des_Coefficiente = "l/ha"

            Case 2016 'ml/l
                Udm_Cod = 29
                Dose_Coefficiente = 1 / 1000 * 100
                Udm_Des = "ml/l"
                Udm_Des_Coefficiente = "l/ha"

            Case 2003 'g/l
                Udm_Cod = 2
                Dose_Coefficiente = 1 / 1000 * 100
                Udm_Des = "g/l"
                Udm_Des_Coefficiente = "Kg/ha"

            Case Else
                'Mappatura non esistente
                Dose_Coefficiente = -1

        End Select

    End Sub

    Private Shared Sub OperazioneCorrente_Scrivi___Tmp_Movimenti_Destinazioni_DateDistinta(idTestataTemp As Integer, piva As String, sa_cod As Integer, appezza As Integer, id_reg As Integer, data As Date, ByRef objParametri As AgronicaCoreParametri)

        Dim stb As New StringBuilder

        stb.AppendLine(" insert into __Tmp_Movimenti_Destinazioni_DateDistinta ( ")
        stb.AppendLine(" 	idTestataTemp,  ")
        stb.AppendLine(" 	piva,  ")
        stb.AppendLine(" 	sa_cod,  ")
        stb.AppendLine(" 	appezza,  ")
        stb.AppendLine(" 	id_reg, ")
        stb.AppendLine(" 	progetto_cod, ")
        stb.AppendLine(" 	validita_inizio, ")
        stb.AppendLine(" 	validita_fine ")
        stb.AppendLine(" ) ")

        stb.AppendLine(" Select  ")
        stb.AppendLine(idTestataTemp & ",")
        stb.AppendLine(" piva, sa_cod, appezza, id_reg, progetto_cod, validita_inizio, validita_fine ")
        stb.AppendLine(" from imprese_progetti ")
        stb.AppendLine(" where piva ='" & Agro_SQL_SaveText(piva) & "'")
        stb.AppendLine(" and sa_cod =" & Agro_SQL_SaveNum(sa_cod))
        stb.AppendLine(" and appezza =" & Agro_SQL_SaveNum(appezza))
        stb.AppendLine(" and id_reg =" & Agro_SQL_SaveNum(id_reg))
        stb.AppendLine(" and validita_inizio <=" & Agro_SQL_SaveDate(data))
        stb.AppendLine(" and validita_fine >=" & Agro_SQL_SaveDate(data))


        Dim scriviImpianti As New AgronicaCoreDataProvider.DataProvider
        Dim rVal As Boolean = scriviImpianti.EseguiQuery_Scrittura(objParametri, stb.ToString, "PopolaTabella__Tmp_Movimenti_Destinazioni_DateDistinta")

    End Sub

    Private Shared Sub PulisciTabella__Tmp_Movimenti_Destinazioni_DateDistinta(idTestataTemp As Integer, ByRef objParametri As AgronicaCoreParametri)

        Dim stb As New StringBuilder
        stb.AppendLine(" delete from __Tmp_Movimenti_Destinazioni_DateDistinta where IDTestatatemp = " & idTestataTemp & " ")
        Dim scrivi As New AgronicaCoreDataProvider.DataProvider
        Dim rVal As Boolean = scrivi.EseguiQuery_Scrittura(objParametri, stb.ToString, "PulisciTabellaFormulatiPA")

    End Sub


    Private Shared Sub PopolaTabellaFormulatiPA(ByVal principiAttivi As String, ByVal IDTestataTemp As Integer, ByRef objParametri As AgronicaCoreParametri, ByVal sPrincipiAttivi As String, ByVal sPrincipiAttiviPesi As String, ByVal fr_cod As Integer)

        If sPrincipiAttivi = "" Then
            Exit Sub
        End If

        Dim vSplitAP As String() = sPrincipiAttivi.Split("|")
        Dim vSplitAPP As String() = sPrincipiAttiviPesi.Split("|")

        Dim vPrincipiAttivi As String() = principiAttivi.Split(",")

        Dim p As Integer = 0

        For Each sSplitAP In vSplitAP

            Dim vPaTitolo As String() = sSplitAP.Split("§")

            Dim vPaPeso As String() = vSplitAPP(p).Split("§")

            If principiAttivi = "" OrElse vPrincipiAttivi.Contains(vPaTitolo(0)) Then

                'inserisco il pa se ha valorizzato titolo o peso
                If CDec(vPaTitolo(2)) > 0 OrElse CDec(vPaPeso(2)) > 0 Then

                    Dim stb As New StringBuilder
                    stb.Length = 0
                    stb.AppendLine(" if not exists ( select  1 from  __tmp_FormulatiXPrincipiAttivi where IDTestataTemp = " & IDTestataTemp & " and  pa_cod = " & vPaTitolo(0) & " and  fr_cod = " & fr_cod & " )")
                    stb.AppendLine(" insert __tmp_FormulatiXPrincipiAttivi(IDTestataTemp, pa_cod, fr_cod, titolo, peso) ")
                    stb.AppendLine(" values (" & IDTestataTemp & ", " & vPaTitolo(0) & "," & fr_cod & "," & vPaTitolo(2).Replace(",", ".") & "," & vPaPeso(2).Replace(",", ".") & ")")

                    '--------------------------------------------------------------------------
                    Dim scrivi As New AgronicaCoreDataProvider.DataProvider
                    Dim rVal As Boolean = scrivi.EseguiQuery_Scrittura(objParametri, stb.ToString, "PopolaTabellaFormulatiPA")
                    '--------------------------------------------------------------------------

                End If

            End If

            p += 1

        Next

    End Sub

    Private Shared Sub PulisciTabella__tmp_FormulatiXPrincipiAttivi(idTestataTemp As Integer, ByRef objParametri As AgronicaCoreParametri)

        Dim stb As New StringBuilder
        stb.AppendLine(" delete from __tmp_FormulatiXPrincipiAttivi where IDTestatatemp = " & idTestataTemp & " ")

        Dim scrivi As New AgronicaCoreDataProvider.DataProvider
        Dim rVal As Boolean = scrivi.EseguiQuery_Scrittura(objParametri, stb.ToString, "PulisciTabellaFormulatiPA")

    End Sub
    Public Function XmlFINALE_OLD(ByVal DatiVerifica As String,
                                  ByVal Id_Agenda_Escluso As Int32,
                                  ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                  ByRef objParametri_Utenti As AgronicaCoreParametri,
                                  Optional ByVal DatiAgenda As String = "",
                                  Optional ByVal Biologico As Boolean = False,
                                  Optional ByVal Dpi_Verificato As String = ""
                                  ) As rispostaStandard(Of Verifica_Disciplinare_Intervento)


        Dim r As New rispostaStandard(Of Verifica_Disciplinare_Intervento)
        r.RispostaStringa = New Verifica_Disciplinare_Intervento

        Dim InterventoConforme As Boolean = True

        Dim i As Integer
        Dim i_DatiRisultati As Long
        Dim i_DatiGenerali As Long
        Dim i_DatiNonConformi As Long
        Dim i_DatoNonConforme As Long
        Dim i_DatiWarnings As Long
        Dim i_DatoWarning As Long

        Dim XmlDoc As New XmlDocument
        Dim XmlDom As New XmlDocument

        Dim XML_Nodo As XmlElement

        Dim xDatiRisultati As XmlNodeList
        Dim xDatiRisultato As XmlElement
        Dim xDatiGenerali As XmlNodeList
        Dim xDatiGenerale As XmlElement
        Dim xDatiNonConformi As XmlNodeList
        Dim xDatoNonConforme As XmlElement
        Dim xNonConformi As XmlNodeList
        Dim xNonConforme As XmlElement
        Dim xDatiWarnings As XmlNodeList
        Dim xDatoWarning As XmlElement
        Dim xWarnings As XmlNodeList
        Dim xWarning As XmlElement

        Dim XmlDatiRisultatiFinali As XmlElement = Nothing
        Dim XmlDatiGeneraliFinali As XmlElement = Nothing
        Dim XmlDatiNonConformiFinali As XmlElement = Nothing
        Dim XmlDatoNonConformeFinale As XmlElement
        Dim XmlWarningFinali As XmlElement = Nothing
        Dim XmlNoteFinali As XmlElement = Nothing
        Dim xDatiDiserbo As XmlNodeList
        Dim xDatoDiserbo As XmlElement

        Dim PIVA As String
        Dim Sa_Cod As Integer
        Dim Veg_Cod As Integer
        Dim Disciplinare_Cod As Integer
        Dim Disciplinare_Des As String
        Dim Id_RcDpi As Integer
        Dim Id_RcDpi_Des As String
        Dim Data As Date
        Dim Des_Lib As String
        Dim TipoTestata As Integer
        Dim Lav_Cod As Integer

        Dim Err_Code As Integer
        Dim Err_Des As String

        Dim Warning_Code As Integer
        Dim Warning_Des As String

        Dim InserisciErrore As Boolean
        Dim InserisciWarning As Boolean

        Dim N_Warning As Integer

        Dim bConforme As Boolean

        Dim Appezza As Integer
        Dim Id_Reg As Integer
        Dim Lotto As String
        Dim Offset As Integer
        Dim Pa_Cod As Integer
        Dim Pa_Des As String
        Dim Titolo As Decimal
        Dim Peso As Decimal
        Dim Pro_Cod As Integer
        Dim Pro_Des As String
        Dim Udm_Cod As Integer
        Dim Da_Ep_Cod As Integer
        Dim Da_Ep_Des As String
        Dim A_Ep_Cod As Integer
        Dim A_Ep_Des As String
        Dim strPA As String
        Dim strAvversita As String
        Dim strGruppiAvversita As String = ""
        Dim Max_Interventi As Integer
        Dim Min_Interventi As Integer
        Dim Max_Interventi_xProdotto As Integer
        Dim Max_Interventi_xProdotto_Udm As Integer
        Dim Max_Interventi_Udm_Des As String
        Dim Interventi_Scheda As Integer
        Dim Interventi_Scheda_Old As Integer
        Dim IntervalloTrattamenti_Min As Integer
        Dim IntervalloTrattamenti_Max As Integer
        Dim DoseDpi_MaxAnno As Decimal
        Dim DoseDpi_MaxAnno_Perc As Decimal
        Dim DoseDpi_MaxAnno_Peso As Decimal
        Dim Consultazione As String = ""
        Dim Epoca_Des As String
        Dim Superficie As String
        Dim Lav_Cod_Soglia As Integer
        Dim Udm_Cod_Soglia As Integer
        Dim Qta_Soglia As Decimal
        Dim Av_Cod As Integer
        Dim Note As String
        Dim Soglie As String(,)
        Dim Qta_Rilievo As Decimal
        Dim Qta_Media_Rilievo As Decimal
        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date
        Dim Sup_Imp As Decimal

        Dim Max_Interventi_xProdotto_Globali As Integer
        Dim For_Veg_Av_Dos_Cod As Integer
        Dim Gruppo_Dosaggi As Integer

        Dim Sup_Coinvolta As Decimal = 0
        Dim strAppCoinvolti As String = ""

        Dim TipoOperazioneDB As Integer = 0
        Dim Destinazioni(7, 0) As String

        Dim i_Piva As Integer = 0
        Dim i_Sa_Cod As Integer = 1
        Dim i_Appezza As Integer = 2
        Dim i_Id_Reg As Integer = 3
        Dim i_Sup_Imp As Integer = 4
        Dim i_Pro_Cod As Integer = 5
        Dim i_Qta As Integer = 6
        Dim i_Udm_Cod As Integer = 7

        Dim HashErrori As New Hashtable

        '========================================================================================

        '#######################################################################################################
        '################### SPACCHETTAMENTO RISULTATO #########################################################
        '#######################################################################################################



        Dim UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME As String = "ND"

        Dim UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_PRODOTTONOAVVERSITA_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_DOSENONDISPONIBILE_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_DOSEUDMNONCONFORME_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_DATADPIMIN_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_DATADPIMAX_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_IMPIANTINONCOERENTIDPI_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_DOSEDPIDISERBOMAX_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_DPI_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_NUMERO_MINIMO_TRATTAMENTI_DPI_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_DOSERAME5ANNIMAX_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_EPOCADPINONCONFORME_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_ACQUAMASSIMA_DPI_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_LIVELLO_CHK_DPI As String = "ND"

        Dim UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA_SALVATAGGIO As String = "ND"

        If VerificaSoloControlliImpostazioniUtente Then

            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim Dt_Impostazioni As DataTable

            Dt_Impostazioni = ObjUtenti.Leggi(0,
                                              1,
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "",
                                              "",
                                              objParametri_Utenti)

            If Dt_Impostazioni IsNot Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then
                For i = 0 To Dt_Impostazioni.Rows.Count - 1
                    Select Case CInt(Dt_Impostazioni.Rows(i).Item("Impostazione_Cod"))
                        Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME
                            UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO
                            UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTONOAVVERSITA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_PRODOTTONOAVVERSITA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSENONDISPONIBILE_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSENONDISPONIBILE_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEUDMNONCONFORME_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSEUDMNONCONFORME_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_IMPIANTINONCOERENTIDPI_SALVATAGGIO
                            UTENTE_COD_BLOCCA_IMPIANTINONCOERENTIDPI_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DATADPIMAX_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DATADPIMAX_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DATADPIMIN_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DATADPIMIN_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_EPOCADPINONCONFORME_SALVATAGGIO
                            UTENTE_COD_BLOCCA_EPOCADPINONCONFORME_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEDPIDISERBOMAX_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSEDPIDISERBOMAX_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_DPI_SALVATAGGIO
                            UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_DPI_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAME5ANNIMAX_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSERAME5ANNIMAX_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_LIVELLO_CHK_DPI
                            UTENTE_COD_LIVELLO_CHK_DPI = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMASSIMA_DPI_SALVATAGGIO
                            UTENTE_COD_BLOCCA_ACQUAMASSIMA_DPI_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                    End Select
                Next
            End If

        End If

        Try


            If DatiAgenda <> "" Then

                Dim XmlDocAgenda As New XmlDocument
                Dim xDatiAgende As XmlNodeList
                Dim xDatiAgenda As XmlElement
                Dim xAgende As XmlNodeList
                Dim xAgenda As XmlElement

                Dim xDatiMovimenti As XmlNodeList
                Dim xDatiMovimento As XmlElement
                Dim xMovimenti As XmlNodeList
                Dim xMovimento As XmlElement

                Dim xDatiMovimenti_Dettagli As XmlNodeList
                Dim xDatiMovimento_Dettaglio As XmlElement
                Dim xMovimenti_Dettagli As XmlNodeList
                Dim xMovimento_Dettaglio As XmlElement

                Dim xMov_Destinazioni As XmlNodeList
                Dim xMov_Destinazione As XmlElement

                XmlDocAgenda.LoadXml(DatiAgenda)

                xDatiAgende = XmlDocAgenda.GetElementsByTagName("DatiAgenda")

                Dim i_DatiAgenda As Integer = 0
                Dim i_Agenda As Integer = 0
                Dim i_DatiMovimento As Integer = 0
                Dim i_Movimento As Integer = 0
                Dim i_DatiMovimenti_Dettagli As Integer = 0
                Dim i_DatiMovimento_Dettaglio As Integer = 0
                Dim i_DatiMov_Destinazioni As Integer = 0


                Do While i_DatiAgenda < xDatiAgende.Count

                    xDatiAgenda = xDatiAgende.Item(i_DatiAgenda)
                    xAgende = xDatiAgenda.GetElementsByTagName("Agenda")

                    i_Agenda = 0

                    Do While i_Agenda < xAgende.Count

                        xAgenda = xAgende.Item(i_Agenda)

                        TipoOperazioneDB = CInt(xAgenda.GetAttribute("TipoOperazioneDB"))

                        xDatiMovimenti = xAgenda.GetElementsByTagName("DatiMovimenti")

                        i_DatiMovimento = 0

                        Do While i_DatiMovimento < xDatiMovimenti.Count

                            xDatiMovimento = xDatiMovimenti.Item(i_DatiMovimento)
                            xMovimenti = xDatiMovimento.GetElementsByTagName("Movimento")

                            i_Movimento = 0

                            Do While i_Movimento < xMovimenti.Count

                                xMovimento = xMovimenti.Item(i_Movimento)

                                'Verifico che il movimento sia la scheda di lavorazione
                                Select Case xMovimento.GetAttribute("cau_mov")

                                    Case CAU_TRATTAMENTO, CAU_RILIEVO_CAMPO, CAU_RILIEVO_RACCOLTA, CAU_LAVORAZIONE

                                        xDatiMovimenti_Dettagli = xMovimento.GetElementsByTagName("DatiMovimenti_Dettagli")

                                        i_DatiMovimenti_Dettagli = 0

                                        Do While i_DatiMovimenti_Dettagli < xDatiMovimenti_Dettagli.Count

                                            xDatiMovimento_Dettaglio = xDatiMovimenti_Dettagli.Item(i_DatiMovimenti_Dettagli)

                                            xMovimenti_Dettagli = xDatiMovimento_Dettaglio.GetElementsByTagName("Movimento_Dettaglio")

                                            i_DatiMovimento_Dettaglio = 0

                                            Do While i_DatiMovimento_Dettaglio < xMovimenti_Dettagli.Count

                                                xMovimento_Dettaglio = xMovimenti_Dettagli.Item(i_DatiMovimento_Dettaglio)

                                                Pro_Cod = CInt(xMovimento_Dettaglio.GetAttribute("pro_cod"))
                                                Udm_Cod = CInt(xMovimento_Dettaglio.GetAttribute("udm_cod"))

                                                xMov_Destinazioni = xMovimento_Dettaglio.GetElementsByTagName("Movimento_Destinazione")

                                                i_DatiMov_Destinazioni = 0
                                                Sup_Coinvolta = 0

                                                Do While i_DatiMov_Destinazioni < xMov_Destinazioni.Count

                                                    'Prelevo l'i-esimo Movimento Dettaglio
                                                    xMov_Destinazione = xMov_Destinazioni.Item(i_DatiMov_Destinazioni)

                                                    If CInt(xMov_Destinazione.GetAttribute("tipo_destinazione")) = 0 Then

                                                        ReDim Preserve Destinazioni(7, UBound(Destinazioni, 2) + 1)

                                                        Destinazioni(i_Piva, UBound(Destinazioni, 2) - 1) = xMov_Destinazione.GetAttribute("piva")
                                                        Destinazioni(i_Sa_Cod, UBound(Destinazioni, 2) - 1) = CInt(xMov_Destinazione.GetAttribute("sa_cod"))
                                                        Destinazioni(i_Appezza, UBound(Destinazioni, 2) - 1) = CInt(xMov_Destinazione.GetAttribute("appezza"))
                                                        Destinazioni(i_Id_Reg, UBound(Destinazioni, 2) - 1) = CInt(xMov_Destinazione.GetAttribute("id_destinazione"))
                                                        Destinazioni(i_Sup_Imp, UBound(Destinazioni, 2) - 1) = CDec(xMov_Destinazione.GetAttribute("qta2"))
                                                        Sup_Coinvolta += CDec(xMov_Destinazione.GetAttribute("qta2"))
                                                        Destinazioni(i_Pro_Cod, UBound(Destinazioni, 2) - 1) = Pro_Cod
                                                        Destinazioni(i_Udm_Cod, UBound(Destinazioni, 2) - 1) = Udm_Cod
                                                        Destinazioni(i_Qta, UBound(Destinazioni, 2) - 1) = CDec(xMov_Destinazione.GetAttribute("qta"))



                                                    End If

                                                    i_DatiMov_Destinazioni += 1

                                                Loop

                                                i_DatiMovimento_Dettaglio += 1

                                            Loop

                                            i_DatiMovimenti_Dettagli += 1

                                        Loop

                                End Select

                                i_Movimento += 1

                            Loop

                            i_DatiMovimento += 1

                        Loop

                        i_Agenda += 1

                    Loop

                    i_DatiAgenda += 1

                Loop

            End If
            '------------------------------

            XmlDom.LoadXml(DatiVerifica)

            xDatiRisultati = XmlDom.GetElementsByTagName("DatiRisultati")

            i_DatiRisultati = 0

            '(06/09/2017 fede) utilizzo hashtable in caso i principi siano letti da ws x evitare rilettura ad ogni impianto
            Dim Hash_FormulatiPA As New Hashtable
            Dim Hash_FormulatiPAPesi As New Hashtable

            'Check di esistenza Informazioni
            Do While i_DatiRisultati < xDatiRisultati.Count

                xDatiRisultato = xDatiRisultati.Item(i_DatiRisultati)

                xDatiGenerali = xDatiRisultato.GetElementsByTagName("DatiGenerali")

                i_DatiGenerali = 0

                Do While i_DatiGenerali < xDatiGenerali.Count

                    xDatiGenerale = xDatiGenerali.Item(i_DatiGenerali)

                    'Impostazione Dati Generali
                    PIVA = xDatiGenerale.GetAttribute("piva")
                    Sa_Cod = CInt(xDatiGenerale.GetAttribute("sa_cod"))
                    Lav_Cod = CInt(xDatiGenerale.GetAttribute("lav_cod"))
                    Des_Lib = xDatiGenerale.GetAttribute("des_lib")
                    TipoTestata = CInt(xDatiGenerale.GetAttribute("tipotestata"))
                    Data = CDate(xDatiGenerale.GetAttribute("data_movimento"))
                    Disciplinare_Cod = CInt(xDatiGenerale.GetAttribute("disciplinare_cod"))
                    Disciplinare_Des = xDatiGenerale.GetAttribute("disciplinare_des")
                    Veg_Cod = CInt(xDatiGenerale.GetAttribute("veg_cod"))
                    Id_RcDpi = CInt(xDatiGenerale.GetAttribute("id_rcdpi"))
                    Id_RcDpi_Des = xDatiGenerale.GetAttribute("rcdpi_des")

                    If IsNumeric(xDatiGenerale.GetAttribute("superficie")) Then
                        Superficie = xDatiGenerale.GetAttribute("superficie")
                        If Superficie = 0 AndAlso Sup_Coinvolta <> 0 Then
                            Superficie = Sup_Coinvolta
                        End If
                    Else
                        Superficie = 0
                        If Sup_Coinvolta <> 0 Then
                            Superficie = Sup_Coinvolta
                        End If
                    End If

                    If xDatiGenerale.HasAttribute("appezzamenti") AndAlso xDatiGenerale.GetAttribute("appezzamenti") <> "" Then
                        strAppCoinvolti = xDatiGenerale.GetAttribute("appezzamenti")
                    End If

                    Epoca_Des = Agro_SQL_SaveText(xDatiGenerale.GetAttribute("epoca_des"))

                    bConforme = True
                    ReDim Soglie(13, 0)

                    '=========================================================================
                    'Creazione Documento Risultato Finale
                    '-------------------------------------------------------------------------

                    XmlDatiRisultatiFinali = XmlDoc.CreateElement("DatiRisultati")

                    '----- < Dati Finali > ----
                    XmlDatiGeneraliFinali = XmlDoc.CreateElement("DatiGenerali")
                    XmlDatiNonConformiFinali = XmlDoc.CreateElement("DatiNonConformi")
                    XmlWarningFinali = XmlDoc.CreateElement("DatiWarnings")
                    XmlNoteFinali = XmlDoc.CreateElement("DatiNote")

                    XmlDatiGeneraliFinali.SetAttribute("piva", PIVA)
                    XmlDatiGeneraliFinali.SetAttribute("sa_cod", Sa_Cod)
                    XmlDatiGeneraliFinali.SetAttribute("data_movimento", CDate(Data).ToShortDateString)
                    XmlDatiGeneraliFinali.SetAttribute("lav_cod", Lav_Cod)
                    XmlDatiGeneraliFinali.SetAttribute("id_agenda", Id_Agenda_Escluso)
                    XmlDatiGeneraliFinali.SetAttribute("des_lib", Des_Lib)
                    XmlDatiGeneraliFinali.SetAttribute("id_rcdpi", Id_RcDpi)
                    XmlDatiGeneraliFinali.SetAttribute("rcdpi_des", Id_RcDpi_Des)
                    XmlDatiGeneraliFinali.SetAttribute("disciplinare_cod", Disciplinare_Cod)
                    XmlDatiGeneraliFinali.SetAttribute("disciplinare_des", Disciplinare_Des)
                    XmlDatiGeneraliFinali.SetAttribute("biologico", Biologico)

                    Select Case Disciplinare_Cod
                        Case -2
                            XmlDatiGeneraliFinali.SetAttribute("disciplinare_des", Descrizione_Regolamento_Bio)
                        Case > 0
                        Case Else
                            If Biologico Then
                                XmlDatiGeneraliFinali.SetAttribute("disciplinare_des", Descrizione_Regolamento_Bio)
                            Else
                                XmlDatiGeneraliFinali.SetAttribute("disciplinare_des", "Nessuno")
                            End If
                    End Select

                    XmlDatiGeneraliFinali.SetAttribute("tipotestata", TipoTestata)
                    XmlDatiGeneraliFinali.SetAttribute("veg_cod", Veg_Cod)
                    XmlDatiGeneraliFinali.SetAttribute("epoca_des", Epoca_Des)
                    XmlDatiGeneraliFinali.SetAttribute("superficie", Superficie)


                    If strAppCoinvolti = "" Then

                        '(13/10/2017 fede) aggiunto dettaglio app coinvolti
                        Dim objapp As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                        Dim dt_impianti As DataTable = objapp.Leggi_Dettagli_Impianti(PIVA, Id_Agenda_Escluso, "", "", objParametri_Server)
                        If dt_impianti IsNot Nothing Then
                            For dett = 0 To dt_impianti.Rows.Count - 1
                                '(15/01/2018 fede) aggiunta indicazione se l'appezzamento ha un vincolo più restrittivo di quello scelto per l'intervento
                                'se si è scelto il regolamento bio si possono trattare tutti gli impianti
                                'se si è scelto un dpi si evidenziano quelli bio
                                'se non si sono scelti disciplinari si evidenziano gli impianti con un dpi e quelli bio
                                Select Case Dpi_Verificato

                                    Case > 0 'dpi
                                        If dt_impianti.Rows(dett).Item("regolamento_cod") = 4 Then
                                            strAppCoinvolti &= dt_impianti.Rows(dett).Item("app_nome") & " (BIO-vincolo più restrittivo)" & ","
                                        Else
                                            strAppCoinvolti &= dt_impianti.Rows(dett).Item("app_nome") & ","
                                        End If

                                    Case "0"
                                        If Not Biologico Then
                                            If dt_impianti.Rows(dett).Item("regolamento_cod") = 4 Then
                                                strAppCoinvolti &= dt_impianti.Rows(dett).Item("app_nome") & " (BIO-vincolo più restrittivo)" & ","
                                            Else
                                                If dt_impianti.Rows(dett).Item("Disciplinare_Cod") <> 0 Then
                                                    strAppCoinvolti &= dt_impianti.Rows(dett).Item("app_nome") & " (DPI-vincolo più restrittivo)" & ","
                                                Else
                                                    strAppCoinvolti &= dt_impianti.Rows(dett).Item("app_nome") & ","
                                                End If
                                            End If
                                        Else
                                            strAppCoinvolti &= dt_impianti.Rows(dett).Item("app_nome") & ","
                                        End If

                                    Case Else
                                        strAppCoinvolti &= dt_impianti.Rows(dett).Item("app_nome") & ","

                                End Select
                            Next
                        End If
                        If strAppCoinvolti <> "" Then
                            strAppCoinvolti = Left(strAppCoinvolti, strAppCoinvolti.Length - 1)
                        End If

                    End If

                    XmlDatiGeneraliFinali.SetAttribute("appezzamenti", strAppCoinvolti)

                    '=============================================================================
                    'Ricerca Dati Non Conformi
                    '-----------------------------------------------------------------------------
                    xDatiNonConformi = xDatiGenerale.GetElementsByTagName("DatiNonConformi")

                    i_DatiNonConformi = 0

                    'Ne esiste solamente 1
                    Do While i_DatiNonConformi < xDatiNonConformi.Count

                        xDatoNonConforme = xDatiNonConformi.Item(i_DatiNonConformi)

                        xNonConformi = xDatoNonConforme.GetElementsByTagName("DatoNonConforme")

                        i_DatoNonConforme = 0

                        Do While i_DatoNonConforme < xNonConformi.Count


                            xNonConforme = xNonConformi.Item(i_DatoNonConforme)

                            InserisciErrore = True

                            Err_Code = CInt(xNonConforme.GetAttribute("err_code"))
                            Err_Des = CStr(xNonConforme.GetAttribute("err_des"))

                            '(25/07/2016 fede) introdotto controllo impostazione utente per ignorare i controlli
                            Select Case UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME

                                Case "0" ', "ND" 'nessun blocco
                                    InserisciErrore = False

                                Case "1", "2" 'blocco, warning (verifica singole impostazioni)

                                    Select Case Err_Code

                                        Case enTipoErrCode_Verifica.AvversitaNonGiustificataDPI
                                            If UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO <> "1" Then
                                                InserisciErrore = False
                                            End If
                                        Case enTipoErrCode_Verifica.ProdottoNonGiustificatoSuAvversita
                                            If UTENTE_COD_BLOCCA_PRODOTTONOAVVERSITA_SALVATAGGIO <> "1" Then
                                                InserisciErrore = False
                                            End If
                                        Case enTipoErrCode_Verifica.DoseNonDisponibile
                                            If UTENTE_COD_BLOCCA_DOSENONDISPONIBILE_SALVATAGGIO <> "1" Then
                                                InserisciErrore = False
                                            End If
                                        Case enTipoErrCode_Verifica.UnitaMisuraNonCompatibile
                                            If UTENTE_COD_BLOCCA_DOSEUDMNONCONFORME_SALVATAGGIO <> "1" Then
                                                InserisciErrore = False
                                            End If
                                        Case enTipoErrCode_Verifica.DoseEccessiva
                                            If UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA_SALVATAGGIO <> "1" Then
                                                InserisciErrore = False
                                            End If
                                        Case enTipoErrCode_Verifica.DoseInsufficiente
                                            If UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA_SALVATAGGIO <> "1" Then
                                                InserisciErrore = False
                                            End If
                                        Case enTipoErrCode_Verifica.AcquaNonCorretta
                                            'acqua superiore
                                            If InStr(Err_Des, "superiore") <> 0 Then
                                                If UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA_SALVATAGGIO <> "1" Then
                                                    InserisciErrore = False
                                                End If
                                            End If
                                            'acqua inferiore
                                            If InStr(Err_Des, "inferiore") <> 0 Then
                                                If UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA_SALVATAGGIO <> "1" Then
                                                    InserisciErrore = False
                                                End If
                                            End If

                                        Case enTipoErrCode_Verifica.DataInterventoMin
                                            If UTENTE_COD_BLOCCA_DATADPIMIN_SALVATAGGIO <> "1" Then
                                                InserisciErrore = False
                                            End If

                                        Case enTipoErrCode_Verifica.DataInterventoMax
                                            If UTENTE_COD_BLOCCA_DATADPIMAX_SALVATAGGIO <> "1" Then
                                                InserisciErrore = False
                                            End If

                                        Case enTipoErrCode_Verifica.ImpiantiNonCoerentiDPI,
                                            enTipoErrCode_Verifica.ImpiantiRaggruppamentoColturaleNonOmogeneo,
                                            enTipoErrCode_Verifica.ImpiantiCoperturaNonOmogeneo,
                                            enTipoErrCode_Verifica.ImpiantiDPINonImpostato,
                                            enTipoErrCode_Verifica.ImpiantiDpiNonOmogeneo
                                            If UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO <> "1" Then
                                                InserisciErrore = False
                                            End If

                                        Case enTipoErrCode_Verifica.DoseDiserboEccessiva
                                            If UTENTE_COD_BLOCCA_DOSEDPIDISERBOMAX_SALVATAGGIO <> "1" Then
                                                InserisciErrore = False
                                            End If

                                            'Case enTipoErrCode_Verifica.Superato_N_Max
                                            '    If UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO <> "1" Then
                                            '        InserisciErrore = False
                                            '    End If

                                            'Case enTipoErrCode_Verifica.Superato_P_Max
                                            '    If UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO <> "1" Then
                                            '        InserisciErrore = False
                                            '    End If

                                            'Case enTipoErrCode_Verifica.Superato_K_Max
                                            '    If UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO <> "1" Then
                                            '        InserisciErrore = False
                                            '    End If

                                            'Case enTipoErrCode_Verifica.Superato_M_Max
                                            '    If UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO <> "1" Then
                                            '        InserisciErrore = False
                                            '    End If

                                        Case enTipoErrCode_Verifica.CarenzaNonRispettata
                                            If UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA_SALVATAGGIO <> "1" Then
                                                InserisciErrore = False
                                            End If

                                        Case enTipoErrCode_Verifica.SuperatoVolumeMaxAcquaDpi
                                            If UTENTE_COD_BLOCCA_ACQUAMASSIMA_DPI_SALVATAGGIO <> "1" Then
                                                InserisciErrore = False
                                            End If

                                        Case enTipoErrCode_Verifica.Epoca_Etichetta
                                            If UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA_SALVATAGGIO <> "1" Then
                                                InserisciErrore = False
                                            End If

                                        Case enTipoErrCode_Verifica.BufferZoneNonRispettata
                                            If UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA_SALVATAGGIO <> "1" Then
                                                InserisciErrore = False
                                            End If

                                    End Select


                            End Select

                            If InserisciErrore Then
                                'Inserisco la non conformità nel risultato finale
                                XML_Nodo = XmlDoc.ImportNode(xNonConforme, True)
                                XmlDatiNonConformiFinali.AppendChild(XML_Nodo)
                                InterventoConforme = False
                            End If

                            i_DatoNonConforme += 1

                        Loop

                        i_DatiNonConformi = i_DatiNonConformi + 1

                    Loop


                    '=============================================================================
                    'Ricerca Warnings
                    '-----------------------------------------------------------------------------
                    xDatiWarnings = xDatiGenerale.GetElementsByTagName("DatiWarnings")

                    i_DatiWarnings = 0

                    'Ne esiste solamente 1
                    Do While i_DatiWarnings < xDatiWarnings.Count

                        xDatoWarning = xDatiWarnings.Item(i_DatiWarnings)

                        xWarnings = xDatoWarning.GetElementsByTagName("Warning")

                        i_DatoWarning = 0

                        'Loop sui Warning
                        Do While i_DatoWarning < xWarnings.Count

                            xWarning = xWarnings.Item(i_DatoWarning)


                            '###########################################################################################
                            '############ VERIFICA WARNING SUL DATABASE GIAS_SERVER ####################################
                            '###########################################################################################
                            N_Warning = CInt(xWarning.GetAttribute("n_warning"))
                            Warning_Code = CInt(xWarning.GetAttribute("warning_code"))

                            '(28/09/2018 fede) se la stessa anomalia è andata già in errore non proseguo la verifica
                            If Not HashErrori.ContainsKey(N_Warning & "|" & Warning_Code) Then



                                Warning_Des = xWarning.GetAttribute("warning_des")
                                If xWarning.HasAttribute("piva") Then
                                    PIVA = CStr(xWarning.GetAttribute("piva"))
                                End If
                                If xWarning.HasAttribute("sa_cod") Then
                                    Sa_Cod = CInt(xWarning.GetAttribute("sa_cod"))
                                End If
                                Appezza = CInt(xWarning.GetAttribute("appezza"))
                                Id_Reg = CInt(xWarning.GetAttribute("id_reg"))
                                Lotto = xWarning.GetAttribute("lotto")
                                Pro_Cod = CInt(xWarning.GetAttribute("pro_cod"))
                                Pro_Des = xWarning.GetAttribute("pro_des")
                                Pa_Cod = CInt(xWarning.GetAttribute("pa_cod"))
                                Pa_Des = xWarning.GetAttribute("pa_des")
                                Da_Ep_Cod = CInt(xWarning.GetAttribute("da_ep_cod"))
                                Da_Ep_Des = xWarning.GetAttribute("da_ep_des")
                                A_Ep_Cod = CInt(xWarning.GetAttribute("a_ep_cod"))
                                A_Ep_Des = xWarning.GetAttribute("a_ep_des")
                                Offset = CInt(xWarning.GetAttribute("offset"))
                                strPA = xWarning.GetAttribute("strpa")
                                strAvversita = xWarning.GetAttribute("avversita_des")
                                '(11/10/2018 fede) aggiunti gruppi per controlli dpi-limitazioni diserbo
                                If xWarning.HasAttribute("gruppi_avversita_des") Then
                                    strGruppiAvversita = xWarning.GetAttribute("gruppi_avversita_des")
                                End If
                                Max_Interventi = CInt(xWarning.GetAttribute("interventi_consentiti"))
                                Interventi_Scheda = CInt(xWarning.GetAttribute("interventi_scheda"))
                                If xWarning.HasAttribute("interventi_consentiti_prodotto") Then
                                    Max_Interventi_xProdotto = CInt(xWarning.GetAttribute("interventi_consentiti_prodotto"))
                                End If
                                If xWarning.HasAttribute("interventi_consentiti_prodotto_udm") Then
                                    Max_Interventi_xProdotto_Udm = CInt(xWarning.GetAttribute("interventi_consentiti_prodotto_udm"))
                                End If
                                If xWarning.HasAttribute("intervallotrattamenti_prodotto_min") Then
                                    IntervalloTrattamenti_Min = CInt(xWarning.GetAttribute("intervallotrattamenti_prodotto_min"))
                                End If
                                If xWarning.HasAttribute("intervallotrattamenti_prodotto_max") Then
                                    IntervalloTrattamenti_Max = CInt(xWarning.GetAttribute("intervallotrattamenti_prodotto_max"))
                                End If
                                Lav_Cod_Soglia = CInt(xWarning.GetAttribute("lav_cod"))
                                Udm_Cod_Soglia = CInt(xWarning.GetAttribute("udm_cod"))
                                Qta_Soglia = CDec(xWarning.GetAttribute("soglia"))
                                Av_Cod = CInt(xWarning.GetAttribute("av_cod"))
                                Note = xWarning.GetAttribute("note")
                                Validita_Inizio = If(IsDate(xWarning.GetAttribute("validita_inizio")), xWarning.GetAttribute("validita_inizio"), AGRODATAINIZIO)
                                Validita_Fine = If(IsDate(xWarning.GetAttribute("validita_fine")), xWarning.GetAttribute("validita_fine"), AGRODATAFINE)

                                If xWarning.HasAttribute("sup_imp") Then
                                    Sup_Imp = CDec(xWarning.GetAttribute("sup_imp"))
                                Else
                                    Sup_Imp = 0
                                End If
                                If xWarning.HasAttribute("titolo") Then
                                    Titolo = CDec(xWarning.GetAttribute("titolo"))
                                Else
                                    Titolo = 0
                                End If
                                If xWarning.HasAttribute("peso") Then
                                    Peso = CDec(xWarning.GetAttribute("peso"))
                                Else
                                    Peso = 0
                                End If

                                If xWarning.HasAttribute("dosedpi_maxanno") Then
                                    DoseDpi_MaxAnno = CDec(xWarning.GetAttribute("dosedpi_maxanno"))
                                Else
                                    DoseDpi_MaxAnno = 0
                                End If

                                If xWarning.HasAttribute("dosedpi_maxanno_perc") Then
                                    DoseDpi_MaxAnno_Perc = CDec(xWarning.GetAttribute("dosedpi_maxanno_perc"))
                                Else
                                    DoseDpi_MaxAnno_Perc = 0
                                End If
                                If xWarning.HasAttribute("dosedpi_maxanno_peso") Then
                                    DoseDpi_MaxAnno_Peso = CDec(xWarning.GetAttribute("dosedpi_maxanno_peso"))
                                Else
                                    DoseDpi_MaxAnno_Peso = 0
                                End If



                                If xWarning.HasAttribute("interventi_consentiti_minimo") Then
                                    Min_Interventi = CInt(xWarning.GetAttribute("interventi_consentiti_minimo"))
                                End If

                                If xWarning.HasAttribute("interventi_consentiti_prodotto_globali") AndAlso IsNumeric(xWarning.GetAttribute("interventi_consentiti_prodotto_globali")) Then
                                    Max_Interventi_xProdotto_Globali = CInt(xWarning.GetAttribute("interventi_consentiti_prodotto_globali"))
                                Else
                                    Max_Interventi_xProdotto_Globali = 0
                                End If

                                If xWarning.HasAttribute("for_veg_av_dos_cod") AndAlso IsNumeric(xWarning.GetAttribute("for_veg_av_dos_cod")) Then
                                    For_Veg_Av_Dos_Cod = CInt(xWarning.GetAttribute("for_veg_av_dos_cod"))
                                Else
                                    For_Veg_Av_Dos_Cod = 0
                                End If

                                If xWarning.HasAttribute("gruppo_dosaggi") AndAlso IsNumeric(xWarning.GetAttribute("gruppo_dosaggi")) Then
                                    Gruppo_Dosaggi = CInt(xWarning.GetAttribute("gruppo_dosaggi"))
                                Else
                                    Gruppo_Dosaggi = 0
                                End If


                                Err_Code = 0
                                Err_Des = ""

                                InserisciWarning = True


                                '(25/07/2016 fede) introdotto controllo impostazione utente per ignorare i controlli
                                Select Case UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME

                                    Case "0" ', "ND" 'nessun blocco
                                        InserisciWarning = False

                                    Case "1", "2", "ND" 'blocco, warning (verifica singole impostazioni)

                                        Select Case Warning_Code

                                            Case enTipoWarning_Verifica.Epoche

                                                'If UTENTE_COD_BLOCCA_EPOCADPINONCONFORME_SALVATAGGIO <> "1" Then
                                                If UTENTE_COD_BLOCCA_EPOCADPINONCONFORME_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else
                                                    VerificaEpoche(
                                                                Err_Code,
                                                                Err_Des,
                                                                PIVA,
                                                                Sa_Cod,
                                                                Appezza,
                                                                Id_Reg,
                                                                Lotto,
                                                                Da_Ep_Cod,
                                                                Da_Ep_Des,
                                                                A_Ep_Cod,
                                                                A_Ep_Des,
                                                                Offset,
                                                                Data,
                                                               Pro_Des,
                                                                Validita_Inizio,
                                                                Validita_Fine,
                                                                objParametri_Server)
                                                End If


                                            Case enTipoWarning_Verifica.Numero_Interventi  'Verifica Numero Interventi x principi attivi

                                                If UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_DPI_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    'Salvo il numero di interventi parziale
                                                    Interventi_Scheda_Old = Interventi_Scheda

                                                    Dim ObjContab_AD As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R
                                                    Dim Hash_IdAgenda As Hashtable

                                                    Dim FiltroAvCod As String = ""
                                                    Dim FiltroAvGru As String = ""
                                                    Select Case TipoTestata
                                                        Case enum_Disciplinare_Tipo_Testata.Difesa
                                                            FiltroAvCod = strAvversita
                                                            FiltroAvGru = ""
                                                        Case enum_Disciplinare_Tipo_Testata.Diserbo
                                                            FiltroAvCod = ""
                                                            FiltroAvGru = strGruppiAvversita
                                                    End Select

                                                    Hash_IdAgenda = ObjContab_AD.LeggiLavorazioni_Da_Principi_Attivi_e_Avversita(PIVA,
                                                                                                    Sa_Cod,
                                                                                                    strPA,
                                                                                                    FiltroAvCod, FiltroAvGru,
                                                                                                    Appezza,
                                                                                                    Id_Reg,
                                                                                                    Validita_Inizio,
                                                                                                    Validita_Fine,
                                                                                                    "",
                                                                                                    "",
                                                                                                    objParametri_Server,
                                                                                                    objParametri_Utenti,
                                                                                                         Hash_FormulatiPA)

                                                    If Hash_IdAgenda.Count <> 0 Then
                                                        Interventi_Scheda = Interventi_Scheda + Hash_IdAgenda.Count
                                                    End If

                                                    'Aggiorno il Warning_Des con il numero di interventi finale
                                                    Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "(" & Interventi_Scheda & ")")

                                                    'Verifica Risultato
                                                    Select Case Interventi_Scheda

                                                        Case Is > Max_Interventi
                                                            'Numero di Interventi Eccedente
                                                            Err_Code = enTipoErrCode_Verifica.SuperatoNumMaxInterventiPA
                                                            Err_Des = Warning_Des & "Sono consentiti al massimo " & Max_Interventi & " interventi all'anno. "
                                                            Consultazione = "Al massimo " & Max_Interventi & " interventi all'anno. "

                                                        Case Is = Max_Interventi
                                                        'Tetto Raggiunto

                                                        Case Is < Max_Interventi
                                                            'Ok

                                                    End Select

                                                End If

                                            '(12/03/2018) aggiunto nuovo controllo numero minimo interventi da dpi
                                            Case enTipoWarning_Verifica.Numero_Interventi_Min

                                                If UTENTE_COD_BLOCCA_NUMERO_MINIMO_TRATTAMENTI_DPI_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    'Salvo il numero di interventi parziale
                                                    Interventi_Scheda_Old = Interventi_Scheda

                                                    Dim ObjContab_AD As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R
                                                    Dim Hash_IdAgenda As Hashtable

                                                    Dim FiltroAvCod As String = ""
                                                    Dim FiltroAvGru As String = ""
                                                    Select Case TipoTestata
                                                        Case enum_Disciplinare_Tipo_Testata.Difesa
                                                            FiltroAvCod = strAvversita
                                                            FiltroAvGru = ""
                                                        Case enum_Disciplinare_Tipo_Testata.Diserbo
                                                            FiltroAvCod = ""
                                                            FiltroAvGru = strGruppiAvversita
                                                    End Select

                                                    Hash_IdAgenda = ObjContab_AD.LeggiLavorazioni_Da_Principi_Attivi_e_Avversita(PIVA,
                                                                                                    Sa_Cod,
                                                                                                    strPA,
                                                                                                    FiltroAvCod, FiltroAvGru,
                                                                                                    Appezza,
                                                                                                    Id_Reg,
                                                                                                    Validita_Inizio,
                                                                                                    Validita_Fine,
                                                                                                    "",
                                                                                                    "",
                                                                                                    objParametri_Server,
                                                                                                    objParametri_Utenti,
                                                                                                         Hash_FormulatiPA)

                                                    If Hash_IdAgenda.Count <> 0 Then
                                                        Interventi_Scheda = Interventi_Scheda + Hash_IdAgenda.Count
                                                    End If

                                                    'Aggiorno il Warning_Des con il numero di interventi finale
                                                    Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "(" & Interventi_Scheda & ")")

                                                    'Verifica Risultato
                                                    Select Case Interventi_Scheda

                                                        Case Is < Min_Interventi
                                                            'Numero di Interventi non suff
                                                            Err_Code = enTipoErrCode_Verifica.NonRaggiuntoNumMinInterventiPA
                                                            Err_Des = Warning_Des & "Sono necessari almeno " & Min_Interventi & " interventi all'anno. "
                                                            Consultazione = "Almeno " & Min_Interventi & " interventi all'anno. "

                                                        Case Is = Min_Interventi
                                                        'Tetto Raggiunto

                                                        Case Is > Min_Interventi
                                                            'Ok

                                                    End Select

                                                End If



                                            Case enTipoWarning_Verifica.Numero_Interventi_xProdotto  'Verifica Numero Interventi

                                                If UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    'Salvo il numero di interventi parziale
                                                    Interventi_Scheda_Old = Interventi_Scheda



                                                    Dim ObjContab_AD As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R
                                                    Dim Hash_IdAgenda As Hashtable
                                                    Dim Hash_IdAgenda_SingolaDose As Hashtable

                                                    Dim Data_Inizio As Date = Validita_Inizio
                                                    Dim Data_Fine As Date = Validita_Fine

                                                    Max_Interventi_Udm_Des = " all'anno. "

                                                    '(26/04/2017 fede) modificato controllo 
                                                    'verranno controllate le seguenti unità di misura (2019=anno, 2024=stagione colturale, 2025=ciclo colturale) 
                                                    'e verranno controllate sempre in base alle date dell'esercizio
                                                    'Select Case Max_Interventi_xProdotto_Udm
                                                    '    Case enum_UnitaMisura.Anno
                                                    '        Data_Inizio = "31/12/" & Data.Year - 1
                                                    '        Data_Fine = "31/12/" & Data.Year
                                                    '    Case enum_UnitaMisura.CicloColturale
                                                    '        Max_Interventi_Udm_Des = " a ciclo colturale. "
                                                    'End Select
                                                    Select Case Max_Interventi_xProdotto_Udm
                                                        Case enum_UnitaMisura.Anno
                                                            Max_Interventi_Udm_Des = " all'anno. "
                                                        Case enum_UnitaMisura.CicloColturale
                                                            Max_Interventi_Udm_Des = " a ciclo colturale. "
                                                        Case enum_UnitaMisura.StagioneColturale
                                                            Max_Interventi_Udm_Des = " a stagione colturale. "
                                                    End Select

                                                    Dim Filtro As String = ""
                                                    'If Gruppo_Dosaggi <> 0 Then
                                                    '    Filtro = " and DoseEtichetta_Value like '%" & For_Veg_Av_Dos_Cod & "%'"
                                                    'End If

                                                    '(12/10/2018 fede) se ho un globale verifico 
                                                    'num volte utilizzata la dose corrente
                                                    'num volte utilizzato il prodotto


                                                    Hash_IdAgenda = ObjContab_AD.LeggiTrattamenti_Da_Formulato(PIVA,
                                                                                                    Sa_Cod,
                                                                                                    Pro_Cod,
                                                                                                    Appezza,
                                                                                                    Id_Reg,
                                                                                                    Data_Inizio,
                                                                                                    Data_Fine,
                                                                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                    Filtro,
                                                                                                    "",
                                                                                                    objParametri_Server,
                                                                                                    objParametri_Utenti)

                                                    If Max_Interventi_xProdotto_Globali > 0 Then

                                                        Filtro = " AND (Movimenti_dettagli.doseetichetta_value like '" & Agro_SQL_SaveText(For_Veg_Av_Dos_Cod.ToString) & "$%' " &
                                                        "      OR  Movimenti_dettagli.doseetichetta_value like '<br>" & Agro_SQL_SaveText(For_Veg_Av_Dos_Cod.ToString) & "$%') "

                                                        'se sono in modifica escludo l'operazione corrente (potrei aver cambiato il dosaggio)
                                                        If Id_Agenda_Escluso <> 0 Then
                                                            Filtro += " AND Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " "
                                                        End If

                                                        Hash_IdAgenda_SingolaDose = ObjContab_AD.LeggiTrattamenti_Da_Formulato(PIVA,
                                                                                            Sa_Cod,
                                                                                            Pro_Cod,
                                                                                            Appezza,
                                                                                            Id_Reg,
                                                                                            Data_Inizio,
                                                                                            Data_Fine,
                                                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                            Filtro,
                                                                                            "",
                                                                                            objParametri_Server,
                                                                                            objParametri_Utenti)


                                                        'marco
                                                        Dim Interventi_Scheda_SingolaDose As Integer = 1
                                                        If Hash_IdAgenda_SingolaDose.Count <> 0 Then
                                                            Interventi_Scheda_SingolaDose += Hash_IdAgenda_SingolaDose.Count
                                                        End If
                                                        'Dim Interventi_Scheda_SingolaDose As Integer = 0
                                                        'If Hash_IdAgenda_SingolaDose.Count <> 0 Then
                                                        '    Interventi_Scheda_SingolaDose = Interventi_Scheda + Hash_IdAgenda_SingolaDose.Count
                                                        '    ''se sono in modifica aggiungo l'operazione corrente (potrei aver cambiato il dosaggio)
                                                        '    'If Id_Agenda_Escluso <> 0 Then
                                                        '    '    Interventi_Scheda_SingolaDose += 1
                                                        '    'End If
                                                        'End If

                                                        If Hash_IdAgenda.Count <> 0 Then
                                                            Interventi_Scheda = Interventi_Scheda + Hash_IdAgenda.Count
                                                        End If

                                                        'Verifica Risultato
                                                        Select Case Interventi_Scheda_SingolaDose
                                                            Case Is > Max_Interventi_xProdotto
                                                                Err_Code = enTipoErrCode_Verifica.SuperatoNumMaxInterventiProdotto
                                                                Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "(" & Interventi_Scheda_SingolaDose & ")")
                                                                Warning_Des = Replace(Warning_Des, "da etichetta", "per questo dosaggio da etichetta")
                                                                Err_Des = Warning_Des
                                                                Consultazione = "Al massimo " & Max_Interventi_xProdotto & " interventi " & Max_Interventi_Udm_Des
                                                        End Select

                                                        'Verifica Risultato
                                                        Select Case Interventi_Scheda
                                                            Case Is > Max_Interventi_xProdotto_Globali
                                                                'Numero di Interventi Eccedente
                                                                Err_Code = enTipoErrCode_Verifica.SuperatoNumMaxInterventiProdotto
                                                                Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "(" & Interventi_Scheda & ")")
                                                                Err_Des = Warning_Des
                                                                Consultazione = "Al massimo " & Max_Interventi_xProdotto_Globali & " interventi " & Max_Interventi_Udm_Des
                                                        End Select



                                                    Else

                                                        ' CASO VECCHIO

                                                        If Hash_IdAgenda.Count <> 0 Then
                                                            Interventi_Scheda = Interventi_Scheda + Hash_IdAgenda.Count
                                                        End If

                                                        'Aggiorno il Warning_Des con il numero di interventi finale
                                                        Warning_Des = Replace(Warning_Des, "(" & Interventi_Scheda_Old & ")", "(" & Interventi_Scheda & ")")

                                                        'Verifica Risultato
                                                        Select Case Interventi_Scheda

                                                            Case Is > Max_Interventi_xProdotto
                                                                'Numero di Interventi Eccedente
                                                                Err_Code = enTipoErrCode_Verifica.SuperatoNumMaxInterventiProdotto
                                                                Err_Des = Warning_Des
                                                                Consultazione = "Al massimo " & Max_Interventi_xProdotto & " interventi " & Max_Interventi_Udm_Des

                                                            Case Is = Max_Interventi_xProdotto
                                                        'Tetto Raggiunto

                                                            Case Is < Max_Interventi_xProdotto
                                                                'Ok

                                                        End Select

                                                    End If


                                                End If



                                            Case enTipoWarning_Verifica.Intervallo_Trattamenti_xProdotto

                                                'If UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA_SALVATAGGIO <> "1" Then
                                                If UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    Dim ObjContab_AD As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

                                                    Dim dt_Movimenti As DataTable
                                                    Dim Str_FiltroAggiuntivo As New StringBuilder
                                                    Str_FiltroAggiuntivo.Append("( ")
                                                    Str_FiltroAggiuntivo.Append(" Mov_Destinazioni.Piva = '" & PIVA & "' ")
                                                    Str_FiltroAggiuntivo.Append(" AND Mov_Destinazioni.Sa_Cod = " & Sa_Cod & " ")
                                                    Str_FiltroAggiuntivo.Append(" AND Mov_Destinazioni.Appezza = " & Appezza & " ")
                                                    Str_FiltroAggiuntivo.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Id_Reg & " ")
                                                    Str_FiltroAggiuntivo.Append(" AND  Imprese_Progetti.Validita_Inizio <= '" & Data & "' AND Imprese_Progetti.Validita_Fine  >= '" & Data & "' ")
                                                    Str_FiltroAggiuntivo.Append(" AND  data_movimento <= '" & Data & "'  ")

                                                    If Id_Agenda_Escluso <> 0 Then
                                                        Str_FiltroAggiuntivo.Append(" AND Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " ")
                                                    End If

                                                    Str_FiltroAggiuntivo.Append(" ) ")

                                                    dt_Movimenti = ObjContab_AD.UltimoTrattamentoImpianti(Pro_Cod, Data, Str_FiltroAggiuntivo.ToString, " data_movimento desc", objParametri_Server)
                                                    If dt_Movimenti IsNot Nothing AndAlso dt_Movimenti.Rows.Count > 0 Then
                                                        Dim ultimaData As Date = dt_Movimenti.Rows(0).Item("data_movimento")
                                                        If ultimaData.AddDays(IntervalloTrattamenti_Min) > Data Then
                                                            Err_Code = enTipoErrCode_Verifica.IntervalloInterventiNonRispettato
                                                            Err_Des = Warning_Des
                                                            Consultazione = "Rispettare l'intervallo di giorni (" & IntervalloTrattamenti_Min & ") tra un intervento e l'altro. "
                                                        End If
                                                    End If

                                                End If



                                            Case enTipoWarning_Verifica.DoseRame_Anno_xBio

                                                If UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    '(02/11/2018 fede) modificato controllo rame per deroghe
                                                    'se ho DoseDpi_MaxAnno valorizzata
                                                    '   prendo quel valore come massimo 
                                                    '   es. pomodoro 2017 batteriosi limite 6 kg annullato (caso in cui non arriva più il warning)
                                                    '   es. pomodoro 2018 batteriosi totale concessi 9 kg
                                                    'altrimenti 
                                                    '   il massimo rimane i 6 kg

                                                    Dim Rame_Qta_Max_Kg As Decimal = 6

                                                    If Data >= #1/1/2019# Then
                                                        Rame_Qta_Max_Kg = 4
                                                    End If

                                                    Dim Inizio As Date = Validita_Inizio
                                                    Dim Fine As Date = Validita_Fine

                                                    Dim dtLavorazioni As DataTable
                                                    Dim ObjContab As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R
                                                    dtLavorazioni = ObjContab.LeggiLavorazioni_Da_Principi_Attivi(PIVA,
                                                                                                        Sa_Cod,
                                                                                                        strPA,
                                                                                                        Appezza,
                                                                                                        Id_Reg,
                                                                                                        Inizio,
                                                                                                        Fine,
                                                                                                        "",
                                                                                                        "",
                                                                                                        objParametri_Server, objParametri_Utenti,
                                                                                                                   Hash_FormulatiPA, Hash_FormulatiPAPesi)

                                                    Dim Qta_Pa_Tot As Decimal = 0
                                                    Dim Qta_Pa_Ha As Decimal = 0
                                                    Dim Qta_Prodotto_Ha As Decimal = 0
                                                    Dim Sup As Decimal
                                                    Dim TitoloP As Decimal
                                                    Dim PesoP As Decimal

                                                    For i = 0 To dtLavorazioni.Rows.Count - 1
                                                        TitoloP = 0
                                                        PesoP = 0
                                                        If Not IsDBNull(dtLavorazioni.Rows(i).Item("titolo")) AndAlso CDec(dtLavorazioni.Rows(i).Item("titolo")) > 0 Then
                                                            TitoloP = CDec(dtLavorazioni.Rows(i).Item("titolo"))
                                                        End If
                                                        If Not IsDBNull(dtLavorazioni.Rows(i).Item("peso")) AndAlso CDec(dtLavorazioni.Rows(i).Item("peso")) > 0 Then
                                                            PesoP = CDec(dtLavorazioni.Rows(i).Item("peso"))
                                                        End If
                                                        If Not IsDBNull(dtLavorazioni.Rows(i).Item("sup_trattata")) AndAlso CDbl(dtLavorazioni.Rows(i).Item("sup_trattata")) > 0 Then
                                                            Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_trattata"))
                                                        Else
                                                            Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_imp"))
                                                        End If
                                                        If Sup <> 0 Then
                                                            Qta_Prodotto_Ha = CDbl(dtLavorazioni.Rows(i).Item("qta")) / Sup
                                                            If PesoP = 0 Then
                                                                Qta_Pa_Ha = Qta_Prodotto_Ha * TitoloP / 100
                                                            Else
                                                                Qta_Pa_Ha = Qta_Prodotto_Ha * PesoP / 1000
                                                            End If
                                                            Qta_Pa_Tot += Qta_Pa_Ha
                                                        End If
                                                    Next

                                                    '(19/06/2017 fede) aggiunto al conteggio il rame delle concimazioni
                                                    Dim Cu_Distribuito As Decimal = 0
                                                    Dim objDett As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                                                    objDett.Leggi_Macroelementi_Distribuiti(0,
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    Cu_Distribuito,
                                                                    0, 0, 0, 0, 0,
                                                                    PIVA,
                                                                    Sa_Cod,
                                                                    Appezza,
                                                                    Id_Reg,
                                                                    0,
                                                                    Inizio,
                                                                    Fine,
                                                                    0,
                                                                    objParametri_Server)

                                                    Qta_Pa_Tot += Cu_Distribuito

                                                    'se sono in inserimento devo aggiungere il rame dell'operazione corrente
                                                    If TipoOperazioneDB = 1 AndAlso Destinazioni IsNot Nothing Then
                                                        For i = 0 To UBound(Destinazioni, 2) - 1
                                                            If Destinazioni(i_Piva, i) = PIVA AndAlso
                                                               Destinazioni(i_Sa_Cod, i) = Sa_Cod AndAlso
                                                               Destinazioni(i_Appezza, i) = Appezza AndAlso
                                                               Destinazioni(i_Id_Reg, i) = Id_Reg AndAlso
                                                               Destinazioni(i_Pro_Cod, i) = Pro_Cod Then

                                                                If CDbl(Destinazioni(i_Sup_Imp, i)) > 0 Then
                                                                    Sup = CDbl(Destinazioni(i_Sup_Imp, i))
                                                                Else
                                                                    Sup = Sup_Imp
                                                                End If
                                                                If Sup <> 0 Then
                                                                    Qta_Prodotto_Ha = CDbl(Destinazioni(i_Qta, i)) / Sup
                                                                    If Peso = 0 Then
                                                                        Qta_Pa_Ha = Qta_Prodotto_Ha * Titolo / 100
                                                                    Else
                                                                        Qta_Pa_Ha = Qta_Prodotto_Ha * Peso / 1000
                                                                    End If
                                                                    Qta_Pa_Tot += Qta_Pa_Ha
                                                                End If

                                                            End If
                                                        Next
                                                    End If


                                                    If DoseDpi_MaxAnno > 0 Then
                                                        Rame_Qta_Max_Kg = DoseDpi_MaxAnno
                                                    End If

                                                    If Qta_Pa_Tot > Rame_Qta_Max_Kg Then
                                                        Err_Code = enTipoErrCode_Verifica.DoseEccessivaRame
                                                        If DoseDpi_MaxAnno <> 6 AndAlso DoseDpi_MaxAnno <> 4 Then
                                                            'If DoseDpi_MaxAnno <> 6 Then
                                                            Err_Des = "La dose di rame distribuita (" & Qta_Pa_Tot & " Kg/ha) supera la massima consentita (" & Rame_Qta_Max_Kg & " Kg/Ha all'anno da Deroga)"
                                                        Else
                                                            Err_Des = "La dose di rame distribuita (" & Qta_Pa_Tot & " Kg/ha) supera la massima consentita (" & Rame_Qta_Max_Kg & " Kg/Ha all'anno)"
                                                        End If
                                                        Consultazione = "E' consentito utilizzare non più di " & Rame_Qta_Max_Kg & " Kg/ha di rame all'anno. "
                                                    End If

                                                End If

                                            Case enTipoWarning_Verifica.DoseRame_5Anni_xBio

                                                'If UTENTE_COD_BLOCCA_DOSERAME5ANNIMAX_SALVATAGGIO <> "1" Then
                                                If UTENTE_COD_BLOCCA_DOSERAME5ANNIMAX_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    Dim Inizio As Date = CDate("01/01/" & (Data.Year - 4).ToString)
                                                    Dim Fine As Date = CDate("31/12/" & Data.Year.ToString)

                                                    Dim Qta_Pa_Tot As Decimal = 0
                                                    Dim Qta_Pa_Ha As Decimal = 0
                                                    Dim Qta_Prodotto_Ha As Decimal = 0
                                                    Dim Sup As Decimal
                                                    Dim TitoloP As Decimal
                                                    Dim PesoP As Decimal

                                                    Dim dtLavorazioni As DataTable

                                                    Dim ObjContab As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R

                                                    dtLavorazioni = ObjContab.LeggiLavorazioni_Da_Principi_Attivi(PIVA,
                                                                                                    Sa_Cod,
                                                                                                    strPA,
                                                                                                    Appezza,
                                                                                                    Id_Reg,
                                                                                                    Inizio,
                                                                                                    Fine,
                                                                                                    "",
                                                                                                    "",
                                                                                                    objParametri_Server, objParametri_Utenti,
                                                                                                               Hash_FormulatiPA, Hash_FormulatiPAPesi)

                                                    For i = 0 To dtLavorazioni.Rows.Count - 1

                                                        TitoloP = 0
                                                        PesoP = 0
                                                        If Not IsDBNull(dtLavorazioni.Rows(i).Item("titolo")) AndAlso CDec(dtLavorazioni.Rows(i).Item("titolo")) > 0 Then
                                                            TitoloP = CDec(dtLavorazioni.Rows(i).Item("titolo"))
                                                        End If
                                                        If Not IsDBNull(dtLavorazioni.Rows(i).Item("peso")) AndAlso CDec(dtLavorazioni.Rows(i).Item("peso")) > 0 Then
                                                            PesoP = CDec(dtLavorazioni.Rows(i).Item("peso"))
                                                        End If

                                                        If Not IsDBNull(dtLavorazioni.Rows(i).Item("sup_trattata")) AndAlso CDbl(dtLavorazioni.Rows(i).Item("sup_trattata")) > 0 Then
                                                            Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_trattata"))
                                                        Else
                                                            Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_imp"))
                                                        End If
                                                        If Sup <> 0 Then
                                                            Qta_Prodotto_Ha = CDbl(dtLavorazioni.Rows(i).Item("qta")) / Sup
                                                            If PesoP = 0 Then
                                                                Qta_Pa_Ha = Qta_Prodotto_Ha * TitoloP / 100
                                                            Else
                                                                Qta_Pa_Ha = Qta_Prodotto_Ha * PesoP / 1000
                                                            End If
                                                            Qta_Pa_Tot += Qta_Pa_Ha
                                                        End If

                                                    Next

                                                    '(19/06/2017 fede) aggiunto al conteggio il rame delle concimazioni
                                                    Dim Cu_Distribuito As Decimal = 0
                                                    Dim objDett As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                                                    objDett.Leggi_Macroelementi_Distribuiti(0,
                                                                0,
                                                                0,
                                                                0,
                                                                Cu_Distribuito,
                                                                0, 0, 0, 0, 0,
                                                                PIVA,
                                                                Sa_Cod,
                                                                Appezza,
                                                                Id_Reg,
                                                                0,
                                                                Inizio,
                                                                Fine,
                                                                0,
                                                                objParametri_Server)

                                                    Qta_Pa_Tot += Cu_Distribuito

                                                    'se sono in inserimento devo aggiungere il rame dell'operazione corrente
                                                    If TipoOperazioneDB = 1 AndAlso Destinazioni IsNot Nothing Then
                                                        For i = 0 To UBound(Destinazioni, 2) - 1
                                                            If Destinazioni(i_Piva, i) = PIVA AndAlso
                                                               Destinazioni(i_Sa_Cod, i) = Sa_Cod AndAlso
                                                               Destinazioni(i_Appezza, i) = Appezza AndAlso
                                                               Destinazioni(i_Id_Reg, i) = Id_Reg AndAlso
                                                               Destinazioni(i_Pro_Cod, i) = Pro_Cod Then

                                                                If CDbl(Destinazioni(i_Sup_Imp, i)) > 0 Then
                                                                    Sup = CDbl(Destinazioni(i_Sup_Imp, i))
                                                                Else
                                                                    Sup = Sup_Imp
                                                                End If
                                                                If Sup <> 0 Then
                                                                    Qta_Prodotto_Ha = CDbl(Destinazioni(i_Qta, i)) / Sup
                                                                    If Peso = 0 Then
                                                                        Qta_Pa_Ha = Qta_Prodotto_Ha * Titolo / 100
                                                                    Else
                                                                        Qta_Pa_Ha = Qta_Prodotto_Ha * Peso / 1000
                                                                    End If
                                                                    Qta_Pa_Tot += Qta_Pa_Ha
                                                                End If

                                                            End If
                                                        Next
                                                    End If

                                                    If Qta_Pa_Tot / 5 > 6 Then
                                                        Err_Code = enTipoErrCode_Verifica.DoseEccessivaRame5Anni
                                                        Err_Des = "La dose di rame distribuita in media in 5 anni (" & Qta_Pa_Tot / 5 & " Kg/ha) supera la massima media consentita in tale intervallo (6 Kg/Ha all'anno)"
                                                        'Err_Des = Warning_Des
                                                        Consultazione = "E' consentito utilizzare non più di 6 Kg/ha di rame all'anno di media in 5 anni. "
                                                    End If

                                                End If


                                            Case enTipoWarning_Verifica.DoseDPI_MaxAnno

                                                If UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO = "0" Then
                                                    InserisciWarning = False
                                                Else
                                                    Dim Filtro As String = ""
                                                    If TipoOperazioneDB <> 0 AndAlso Destinazioni IsNot Nothing Then
                                                        If Id_Agenda_Escluso <> 0 Then
                                                            Filtro &= " Mov_Destinazioni.id_agenda <> " & Agro_SQL_SaveNum(Id_Agenda_Escluso) & " "
                                                        End If
                                                    End If

                                                    Dim dtLavorazioni As DataTable

                                                    Dim ObjContab As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R
                                                    dtLavorazioni = ObjContab.LeggiLavorazioni_Da_Principi_Attivi(PIVA,
                                                                                                        Sa_Cod,
                                                                                                        Pa_Cod,
                                                                                                        Appezza,
                                                                                                        Id_Reg,
                                                                                                        Validita_Inizio,
                                                                                                        Validita_Fine,
                                                                                                        Filtro,
                                                                                                        "",
                                                                                                        objParametri_Server, objParametri_Utenti,
                                                                                                                   Hash_FormulatiPA, Hash_FormulatiPAPesi)


                                                    Dim Qta_Pa_Tot As Decimal = 0
                                                    Dim Qta_Pa_Ha As Decimal = 0
                                                    Dim Qta_Prodotto_Ha As Decimal = 0
                                                    Dim Sup As Decimal
                                                    Dim TitoloP As Decimal
                                                    Dim PesoP As Decimal

                                                    For i = 0 To dtLavorazioni.Rows.Count - 1

                                                        TitoloP = 0
                                                        PesoP = 0
                                                        If Not IsDBNull(dtLavorazioni.Rows(i).Item("titolo")) AndAlso CDec(dtLavorazioni.Rows(i).Item("titolo")) > 0 Then
                                                            TitoloP = CDec(dtLavorazioni.Rows(i).Item("titolo"))
                                                        End If
                                                        If Not IsDBNull(dtLavorazioni.Rows(i).Item("peso")) AndAlso CDec(dtLavorazioni.Rows(i).Item("peso")) > 0 Then
                                                            PesoP = CDec(dtLavorazioni.Rows(i).Item("peso"))
                                                        End If

                                                        If Not IsDBNull(dtLavorazioni.Rows(i).Item("sup_trattata")) AndAlso CDbl(dtLavorazioni.Rows(i).Item("sup_trattata")) > 0 Then
                                                            Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_trattata"))
                                                        Else
                                                            Sup = CDbl(dtLavorazioni.Rows(i).Item("sup_imp"))
                                                        End If

                                                        If Sup <> 0 Then
                                                            Qta_Prodotto_Ha = CDbl(dtLavorazioni.Rows(i).Item("qta")) / Sup

                                                            If DoseDpi_MaxAnno_Peso <> 0 AndAlso PesoP <> 0 Then
                                                                Qta_Pa_Ha = Qta_Prodotto_Ha * PesoP / 1000
                                                            Else
                                                                Qta_Pa_Ha = Qta_Prodotto_Ha * TitoloP / 100
                                                            End If

                                                            'If PesoP = 0 Then
                                                            '    Qta_Pa_Ha = Qta_Prodotto_Ha * TitoloP / 100
                                                            'Else
                                                            '    Qta_Pa_Ha = Qta_Prodotto_Ha * PesoP / 1000
                                                            'End If
                                                            Qta_Pa_Tot += Qta_Pa_Ha
                                                        End If

                                                    Next

                                                    'se sono in inserimento devo aggiungere il rame dell'operazione corrente
                                                    'If TipoOperazioneDB = 1 AndAlso Not Destinazioni Is Nothing Then
                                                    If TipoOperazioneDB <> 0 AndAlso Destinazioni IsNot Nothing Then
                                                        For i = 0 To UBound(Destinazioni, 2) - 1
                                                            If Destinazioni(i_Piva, i) = PIVA AndAlso
                                                               Destinazioni(i_Sa_Cod, i) = Sa_Cod AndAlso
                                                               Destinazioni(i_Appezza, i) = Appezza AndAlso
                                                               Destinazioni(i_Id_Reg, i) = Id_Reg AndAlso
                                                               Destinazioni(i_Pro_Cod, i) = Pro_Cod Then

                                                                If CDbl(Destinazioni(i_Sup_Imp, i)) > 0 Then
                                                                    Sup = CDbl(Destinazioni(i_Sup_Imp, i))
                                                                Else
                                                                    Sup = Sup_Imp
                                                                End If

                                                                If Sup <> 0 Then
                                                                    Qta_Prodotto_Ha = CDbl(Destinazioni(i_Qta, i)) / Sup

                                                                    If DoseDpi_MaxAnno_Peso <> 0 AndAlso Peso <> 0 Then
                                                                        Qta_Pa_Ha = Qta_Prodotto_Ha * Peso / 1000
                                                                    Else
                                                                        Qta_Pa_Ha = Qta_Prodotto_Ha * Titolo / 100
                                                                    End If

                                                                    'If Peso = 0 Then
                                                                    '    Qta_Pa_Ha = Qta_Prodotto_Ha * Titolo / 100
                                                                    'Else
                                                                    '    Qta_Pa_Ha = Qta_Prodotto_Ha * Peso / 1000
                                                                    'End If
                                                                    Qta_Pa_Tot += Qta_Pa_Ha
                                                                End If

                                                            End If
                                                        Next
                                                    End If

                                                    'arrotondo x i decimali del ws
                                                    Qta_Pa_Tot = CDec(Format(Qta_Pa_Tot, "###,###.0##"))

                                                    If Qta_Pa_Tot > DoseDpi_MaxAnno Then
                                                        Err_Code = enTipoErrCode_Verifica.DoseEccessivaSADpi_Anno
                                                        Err_Des = Warning_Des
                                                        Consultazione = "E' consentito utilizzare non più di " & DoseDpi_MaxAnno & " Kg/ha di " & Pa_Des & " all'anno. " &
                                                                        "Ne sono stati distribuiti " & Format(Qta_Pa_Tot, "##,###,##0.000") & " Kg/ha all'anno. "
                                                        Err_Des = Consultazione
                                                    End If

                                                End If

                                                '(02/11/2017 fede) reintrodotte soglie
                                            Case enTipoWarning_Verifica.Soglie 'Verifica SOGLIE

                                                If Not ((UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME = "ND" AndAlso UTENTE_COD_LIVELLO_CHK_DPI = "ND") OrElse
                                                (UTENTE_COD_LIVELLO_CHK_DPI = "2")) Then
                                                    'If UTENTE_COD_LIVELLO_CHK_DPI = "0" Then
                                                    InserisciWarning = False
                                                Else

                                                    'Controllo se esiste già l'impianto
                                                    Dim bEsiste As Boolean = False
                                                    For i = 0 To UBound(Soglie, 2) - 1
                                                        If Soglie(0, i) = PIVA AndAlso
                                                           Soglie(1, i) = Sa_Cod AndAlso
                                                           Soglie(2, i) = Appezza AndAlso
                                                           Soglie(3, i) = Id_Reg AndAlso
                                                           Soglie(8, i) = Pro_Cod Then

                                                            bEsiste = True
                                                            Exit For

                                                        End If
                                                    Next

                                                    If Not bEsiste Then

                                                        ReDim Preserve Soglie(13, UBound(Soglie, 2) + 1)

                                                        Soglie(0, UBound(Soglie, 2) - 1) = PIVA
                                                        Soglie(1, UBound(Soglie, 2) - 1) = Sa_Cod
                                                        Soglie(2, UBound(Soglie, 2) - 1) = Appezza
                                                        Soglie(3, UBound(Soglie, 2) - 1) = Id_Reg
                                                        Soglie(4, UBound(Soglie, 2) - 1) = Av_Cod
                                                        Soglie(5, UBound(Soglie, 2) - 1) = 0 'Soglia Non Soddisfatta
                                                        Soglie(6, UBound(Soglie, 2) - 1) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                        Soglie(7, UBound(Soglie, 2) - 1) = Warning_Des
                                                        Soglie(8, UBound(Soglie, 2) - 1) = Pro_Cod
                                                        Soglie(9, UBound(Soglie, 2) - 1) = Pro_Des
                                                        Soglie(10, UBound(Soglie, 2) - 1) = Note
                                                        Soglie(11, UBound(Soglie, 2) - 1) = 0 'Media Catture per Trappola
                                                        Soglie(12, UBound(Soglie, 2) - 1) = Lav_Cod_Soglia

                                                        i = UBound(Soglie, 2) - 1

                                                    End If

                                                    If Soglie(5, i) = 0 Then

                                                        Dim Data_Validita_Inizio As Date = Validita_Inizio
                                                        If Offset <> 0 Then
                                                            Data_Validita_Inizio = DateAdd("d", -Offset, Data)
                                                        End If

                                                        Select Case Lav_Cod_Soglia

                                                            Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE

                                                                Dim ObjRilievi As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
                                                                Dim dtRilievi As DataTable

                                                                'Ricerca di un rilievo fatto nel centro rispetta all'avversità e alla specie vegetale
                                                                dtRilievi = ObjRilievi.Leggi_RilieviTrappole(PIVA,
                                                                                                        Sa_Cod,
                                                                                                        0,
                                                                                                        0,
                                                                                                        0,
                                                                                                        Av_Cod,
                                                                                                        Veg_Cod,
                                                                                                        Data_Validita_Inizio,
                                                                                                        Data,
                                                                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                        "",
                                                                                                        "",
                                                                                                        objParametri_Server)

                                                                Qta_Rilievo = 0

                                                                If dtRilievi.Rows.Count > 0 Then

                                                                    Soglie(11, i) = dtRilievi.Rows(0).Item("Qta")

                                                                    Dim iRil As Integer
                                                                    For iRil = 0 To dtRilievi.Rows.Count - 1
                                                                        Qta_Rilievo = Qta_Rilievo + dtRilievi.Rows(iRil).Item("Qta_Ril")
                                                                    Next iRil

                                                                    Qta_Media_Rilievo = (Qta_Rilievo / dtRilievi.Rows.Count)

                                                                    If Not (Qta_Media_Rilievo >= Qta_Soglia) Then
                                                                        Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                        If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                            Soglie(7, i) &= Warning_Des
                                                                        End If
                                                                    Else
                                                                        Soglie(5, i) = 1 'Soglia Soddisfatta --> Impianto Ok
                                                                    End If
                                                                Else
                                                                    Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                    If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                        Soglie(7, i) &= Warning_Des
                                                                    End If
                                                                End If

                                                                '(27/02/2019 fede) aggiunto controllo se è stato registrato il rilievo avv in campo della soglia necessaria
                                                                If Soglie(5, i) = 0 Then

                                                                    dtRilievi = ObjRilievi.Leggi_RilieviAvversita(PIVA,
                                                                                                       Sa_Cod,
                                                                                                       Appezza,
                                                                                                       Id_Reg,
                                                                                                       Udm_Cod_Soglia,
                                                                                                       Av_Cod,
                                                                                                       Data_Validita_Inizio,
                                                                                                       Data,
                                                                                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                       "",
                                                                                                       "Mov_Destinazioni.Validita_Inizio Desc, Mov_Destinazioni.Qta Desc",
                                                                                                       objParametri_Server)

                                                                    If dtRilievi.Rows.Count > 0 Then

                                                                        'Gestione Presenza
                                                                        If CDbl(Qta_Soglia) = 0 Then
                                                                            Qta_Soglia = 0.5 'Presenza
                                                                        End If

                                                                        Soglie(11, i) = dtRilievi.Rows(0).Item("Qta")

                                                                        If Not (CDbl(dtRilievi.Rows(0).Item("Qta")) >= Qta_Soglia) Then
                                                                            Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                            If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                                Soglie(7, i) &= Warning_Des
                                                                            End If
                                                                        Else
                                                                            Soglie(5, i) = 1 'Soglia Soddisfatta --> Impianto Ok
                                                                        End If

                                                                    Else
                                                                        Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                        If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                            Soglie(7, i) &= Warning_Des
                                                                        End If
                                                                    End If

                                                                End If



                                                            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO

                                                                Dim ObjRilievi As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
                                                                Dim dtRilievi As DataTable

                                                                'Lettura Rilievi Precedenti
                                                                dtRilievi = ObjRilievi.Leggi_RilieviAvversita(PIVA,
                                                                                                        Sa_Cod,
                                                                                                        Appezza,
                                                                                                        Id_Reg,
                                                                                                        Udm_Cod_Soglia,
                                                                                                        Av_Cod,
                                                                                                        Data_Validita_Inizio,
                                                                                                        Data,
                                                                                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                       "",
                                                                                                        "Mov_Destinazioni.Validita_Inizio Desc, Mov_Destinazioni.Qta Desc",
                                                                                                        objParametri_Server)

                                                                If dtRilievi.Rows.Count > 0 Then

                                                                    'Gestione Presenza
                                                                    If CDbl(Qta_Soglia) = 0 Then
                                                                        Qta_Soglia = 0.5 'Presenza
                                                                    End If

                                                                    Soglie(11, i) = dtRilievi.Rows(0).Item("Qta")

                                                                    If Not (CDbl(dtRilievi.Rows(0).Item("Qta")) >= Qta_Soglia) Then
                                                                        Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                        If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                            Soglie(7, i) &= Warning_Des
                                                                        End If
                                                                    Else
                                                                        Soglie(5, i) = 1 'Soglia Soddisfatta --> Impianto Ok
                                                                    End If

                                                                Else
                                                                    Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                    If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                        Soglie(7, i) &= Warning_Des
                                                                    End If
                                                                End If

                                                                '(27/02/2019 fede) aggiunto controllo se è stato registrato il rilievo avv in campo della soglia necessaria
                                                                If Soglie(5, i) = 0 Then

                                                                    'Ricerca di un rilievo fatto nel centro rispetta all'avversità e alla specie vegetale
                                                                    dtRilievi = ObjRilievi.Leggi_RilieviTrappole(PIVA,
                                                                                                            Sa_Cod,
                                                                                                            0,
                                                                                                            0,
                                                                                                            0,
                                                                                                            Av_Cod,
                                                                                                            Veg_Cod,
                                                                                                            Data_Validita_Inizio,
                                                                                                            Data,
                                                                                                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                                            "",
                                                                                                            "",
                                                                                                            objParametri_Server)

                                                                    Qta_Rilievo = 0

                                                                    If dtRilievi.Rows.Count > 0 Then

                                                                        Soglie(11, i) = dtRilievi.Rows(0).Item("Qta")

                                                                        Dim iRil As Integer
                                                                        For iRil = 0 To dtRilievi.Rows.Count - 1
                                                                            Qta_Rilievo = Qta_Rilievo + dtRilievi.Rows(iRil).Item("Qta_Ril")
                                                                        Next iRil

                                                                        Qta_Media_Rilievo = (Qta_Rilievo / dtRilievi.Rows.Count)

                                                                        If Not (Qta_Media_Rilievo >= Qta_Soglia) Then
                                                                            Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                            If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                                Soglie(7, i) &= Warning_Des
                                                                            End If
                                                                        Else
                                                                            Soglie(5, i) = 1 'Soglia Soddisfatta --> Impianto Ok
                                                                        End If
                                                                    Else
                                                                        Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                        If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                            Soglie(7, i) &= Warning_Des
                                                                        End If
                                                                    End If

                                                                End If

                                                                '(13/03/2019 fede) aggiunto controllo registrata giustificazione richiesta
                                                            Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE

                                                                Dim ObjRilievi As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
                                                                Dim dtRilievi As DataTable

                                                                dtRilievi = ObjRilievi.Leggi_ConfusioneDisorientamentoSessuale(PIVA,
                                                                                                        Sa_Cod,
                                                                                                        Appezza,
                                                                                                        Id_Reg,
                                                                                                        Lav_Cod_Soglia,
                                                                                                        Av_Cod,
                                                                                                        Data_Validita_Inizio,
                                                                                                        Data,
                                                                                                        "",
                                                                                                        "Mov_Destinazioni.Validita_Inizio Desc, Mov_Destinazioni.Qta Desc",
                                                                                                        objParametri_Server)

                                                                If dtRilievi.Rows.Count > 0 Then

                                                                    ''Gestione Presenza
                                                                    'If CDbl(Qta_Soglia) = 0 Then
                                                                    '    Qta_Soglia = 0.5 'Presenza
                                                                    'End If

                                                                    'Soglie(11, i) = dtRilievi.Rows(0).Item("Qta")

                                                                    'If Not (CDbl(dtRilievi.Rows(0).Item("Qta")) >= Qta_Soglia) Then
                                                                    '    Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                    '    If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                    '        Soglie(7, i) &= Warning_Des
                                                                    '    End If
                                                                    'Else
                                                                    Soglie(5, i) = 1 'Soglia Soddisfatta --> Impianto Ok
                                                                    'End If

                                                                Else
                                                                    Soglie(6, i) = enTipoErrCode_Verifica.SogliaNonRegistrata
                                                                    If InStr(Soglie(7, i), Warning_Des) = 0 Then
                                                                        Soglie(7, i) &= Warning_Des
                                                                    End If
                                                                End If


                                                        End Select

                                                    End If

                                                End If

                                        End Select

                                End Select







                                '============================================================================
                                Select Case Err_Code

                                    Case 0 'Verde (Warning Conforme)

                                        'Do Nothing

                                    Case Else 'Rosso (Warning Non Conforme)

                                        'Inserimento Dato Non Conforme in Stringa Risultato

                                        HashErrori.Add(N_Warning & "|" & Warning_Code, "")

                                        XmlDatoNonConformeFinale = XmlDoc.CreateElement("DatoNonConforme")
                                        XmlDatoNonConformeFinale.SetAttribute("err_code", Err_Code)
                                        XmlDatoNonConformeFinale.SetAttribute("err_des", Err_Des)
                                        XmlDatoNonConformeFinale.SetAttribute("consultazione", Consultazione)
                                        XmlDatoNonConformeFinale.SetAttribute("pro_cod", Pro_Cod)
                                        XmlDatoNonConformeFinale.SetAttribute("pro_des", Pro_Des)
                                        XmlDatoNonConformeFinale.SetAttribute("mat_cod", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("udm_cod", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("dose", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("dose_consentita", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("appezza", Appezza)
                                        XmlDatoNonConformeFinale.SetAttribute("id_reg", Id_Reg)
                                        XmlDatoNonConformeFinale.SetAttribute("strpa_cod", strPA)
                                        XmlDatoNonConformeFinale.SetAttribute("interventi", Interventi_Scheda & " ")
                                        XmlDatoNonConformeFinale.SetAttribute("interventi_consentiti", Max_Interventi & " ")
                                        XmlDatoNonConformeFinale.SetAttribute("n", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("n_distribuito", "")
                                        XmlDatoNonConformeFinale.SetAttribute("p", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("p_distribuito", "")
                                        XmlDatoNonConformeFinale.SetAttribute("k", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("k_distribuito", "")
                                        XmlDatoNonConformeFinale.SetAttribute("mg", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("mg_distribuito", "")
                                        XmlDatoNonConformeFinale.SetAttribute("av_gru", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("av_cod", 0)
                                        XmlDatoNonConformeFinale.SetAttribute("avversita_des", strAvversita)

                                        XmlDatiNonConformiFinali.AppendChild(XmlDatoNonConformeFinale)

                                        XmlDatoNonConformeFinale = Nothing

                                        InterventoConforme = False

                                End Select
                                '============================================================================



                            End If


                            i_DatoWarning = i_DatoWarning + 1

                        Loop

                        i_DatiWarnings = i_DatiWarnings + 1

                    Loop


                    '=============================================================================
                    'Ricerca Soglie Non Raggiunte
                    '-----------------------------------------------------------------------------
                    For i = 0 To UBound(Soglie, 2) - 1

                        If Soglie(5, i) = 0 Then

                            XmlDatoNonConformeFinale = XmlDoc.CreateElement("DatoNonConforme")
                            XmlDatoNonConformeFinale.SetAttribute("err_code", Soglie(6, i))

                            'Completamento Stringa Errore
                            If Agro_SQL_SaveNum(Soglie(11, i)) = 0 Then
                                Soglie(7, i) = Soglie(7, i) & " Non risulta in archivio alcuna giustificazione di intervento. "
                            Else
                                Select Case Soglie(12, i)
                                    Case 110 'Avversità nelle Trappole
                                        Soglie(7, i) = Soglie(7, i) & " La media di catture per trappola rilevata è pari a " & Soglie(11, i) & ". "
                                    Case 113 'Avversità in Campo
                                        Soglie(7, i) = Soglie(7, i) & " La soglia rilevata è pari a " & Soglie(11, i) & ". "
                                End Select
                            End If

                            XmlDatoNonConformeFinale.SetAttribute("err_des", Soglie(7, i))
                            '==================================================================================================
                            XmlDatoNonConformeFinale.SetAttribute("consultazione", Soglie(10, i))
                            XmlDatoNonConformeFinale.SetAttribute("pro_cod", Soglie(8, i))
                            XmlDatoNonConformeFinale.SetAttribute("pro_des", Soglie(9, i))
                            XmlDatoNonConformeFinale.SetAttribute("mat_cod", 0)
                            XmlDatoNonConformeFinale.SetAttribute("udm_cod", 0)
                            XmlDatoNonConformeFinale.SetAttribute("dose", 0)
                            XmlDatoNonConformeFinale.SetAttribute("dose_consentita", 0)
                            XmlDatoNonConformeFinale.SetAttribute("appezza", Soglie(2, i))
                            XmlDatoNonConformeFinale.SetAttribute("id_reg", Soglie(3, i))
                            XmlDatoNonConformeFinale.SetAttribute("strpa_cod", "")
                            XmlDatoNonConformeFinale.SetAttribute("interventi", "")
                            XmlDatoNonConformeFinale.SetAttribute("interventi_consentiti", "")
                            XmlDatoNonConformeFinale.SetAttribute("n", 0)
                            XmlDatoNonConformeFinale.SetAttribute("n_distribuito", "")
                            XmlDatoNonConformeFinale.SetAttribute("p", 0)
                            XmlDatoNonConformeFinale.SetAttribute("p_distribuito", "")
                            XmlDatoNonConformeFinale.SetAttribute("k", 0)
                            XmlDatoNonConformeFinale.SetAttribute("k_distribuito", "")
                            XmlDatoNonConformeFinale.SetAttribute("mg", 0)
                            XmlDatoNonConformeFinale.SetAttribute("mg_distribuito", "")
                            XmlDatoNonConformeFinale.SetAttribute("av_gru", 0)
                            XmlDatoNonConformeFinale.SetAttribute("av_cod", Soglie(4, i))
                            XmlDatoNonConformeFinale.SetAttribute("avversita_des", "")

                            XmlDatiNonConformiFinali.AppendChild(XmlDatoNonConformeFinale)

                            XmlDatoNonConformeFinale = Nothing

                            InterventoConforme = False

                        End If

                    Next i

                    '=============================================================================
                    'Ricerca Dati Diserbo
                    '-----------------------------------------------------------------------------
                    xDatiDiserbo = xDatiGenerale.GetElementsByTagName("DatiDiserbo")

                    'Ne esiste solamente 1
                    If xDatiDiserbo.Count > 0 Then

                        xDatoDiserbo = xDatiDiserbo.Item(0)

                        XML_Nodo = XmlDoc.ImportNode(xDatoDiserbo, True)
                        XmlDatiGeneraliFinali.AppendChild(XML_Nodo)
                        'XmlDatiGeneraliFinali.AppendChild(xDatoDiserbo)

                    End If

                    '=============================================================================

                    i_DatiGenerali = i_DatiGenerali + 1

                Loop

                i_DatiRisultati = i_DatiRisultati + 1

            Loop

            XmlDatiGeneraliFinali.AppendChild(XmlDatiNonConformiFinali)
            XmlDatiGeneraliFinali.AppendChild(XmlWarningFinali)
            XmlDatiGeneraliFinali.AppendChild(XmlNoteFinali)
            XmlDatiRisultatiFinali.AppendChild(XmlDatiGeneraliFinali)

            XmlDoc.AppendChild(XmlDatiRisultatiFinali)


            r.RispostaOK = True
            r.RispostaStringa.Risultato = XmlDoc.OuterXml
            r.RispostaStringa.Conforme = InterventoConforme

            'Distruggo gli oggetti
            XmlDom = Nothing
            xDatiRisultati = Nothing
            xDatiRisultato = Nothing
            xDatiGenerali = Nothing
            xDatiGenerale = Nothing
            xDatiWarnings = Nothing
            xDatoWarning = Nothing

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = ex.Message

        End Try

        Return r

    End Function




    Public Sub Analizza_RisultatoVerifica(ByVal Xml_Risultato_Verifica As String,
                                          ByVal VerificaSoloControlliImpostazioniUtente As Boolean,
                                          ByVal DtCentri As DataTable,
                                          ByRef DT_InterventoVericato As DataTable,
                                          ByRef DT_RisultatiVerifiche As DataTable,
                                          ByRef Disciplinare_cod As Integer,
                                          ByRef Biologico As Boolean,
                                          ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim XmlDoc As New XmlDocument
        Dim XML_DatiRisultati As XmlElement
        Dim XML_DatiGenerali As XmlElement
        Dim XML_DatiNonConformi As XmlElement
        Dim XMLs_DatoNonConforme As XmlNodeList
        Dim XML_DatoNonConforme As XmlElement

        Dim Piva As String
        Dim sacod As Integer
        Dim Lav_Cod As Integer
        Dim Id_Agenda As Integer
        Dim DescrizioneOperazione As String
        Dim Data_Movimento As String
        Dim Superficie As Decimal = 0
        Dim Disciplinare_Des As String
        Dim Id_Rcdpi As Integer
        Dim Rcdpi_des As String
        Dim RisultatoVerifica As String
        Dim TipoIntervento As String = ""
        Dim Appezzamenti As String = ""

        Dim Dr_InterventoVericato As DataRow
        Dim Dr_RisultatiVerifiche As DataRow

        Dim Conforme As Boolean


        '------------------------------------------
        '----- Analizzo la stringa XML
        '------------------------------------------

        'Carico la stringa nel documento XML
        XmlDoc.LoadXml(Xml_Risultato_Verifica)

        '----- Elemento <DatiRisultati>
        XML_DatiRisultati = XmlDoc.SelectSingleNode("DatiRisultati")

        '----- Elemento <DatiGenerali>
        XML_DatiGenerali = XML_DatiRisultati.SelectSingleNode("DatiGenerali")

        '----- Attributi di <DatiGenerali>
        Piva = CStr(XML_DatiGenerali.GetAttribute("piva"))
        sacod = CInt(XML_DatiGenerali.GetAttribute("sa_cod"))

        Id_Rcdpi = CInt(XML_DatiGenerali.GetAttribute("id_rcdpi"))
        Rcdpi_des = CStr(XML_DatiGenerali.GetAttribute("rcdpi_des"))

        Biologico = CBool(XML_DatiGenerali.GetAttribute("biologico"))
        Disciplinare_cod = CInt(XML_DatiGenerali.GetAttribute("disciplinare_cod"))
        Disciplinare_Des = CStr(XML_DatiGenerali.GetAttribute("disciplinare_des"))

        If Rcdpi_des <> "" Then
            Disciplinare_Des &= " - " & Rcdpi_des
        End If

        Lav_Cod = CInt(XML_DatiGenerali.GetAttribute("lav_cod"))
        Id_Agenda = CInt(XML_DatiGenerali.GetAttribute("id_agenda"))

        DescrizioneOperazione = CStr(XML_DatiGenerali.GetAttribute("des_lib"))

        Data_Movimento = CStr(XML_DatiGenerali.GetAttribute("data_movimento"))

        If IsNumeric(XML_DatiGenerali.GetAttribute("superficie")) Then
            Superficie = CDec(XML_DatiGenerali.GetAttribute("superficie"))
        End If

        Appezzamenti = CStr(XML_DatiGenerali.GetAttribute("appezzamenti"))

        Select Case CInt(Lav_Cod)
            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                 LAVCOD_GEODISINFESTAZIONE,
                 LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                 LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA,
                 LAVCOD_REINNESCO_TRAPPOLE
                TipoIntervento = "Difesa"

            Case LAVCOD_TRATTAMENTO_FITOREGOLATORE
                TipoIntervento = "Fitoregolatore"

            Case LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO
                TipoIntervento = "Diserbo"

            Case LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                 LAVCOD_CONCIMAZIONE_FOGLIARE,
                 LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                 LAVCOD_DISTRIBUZIONE_CONCIME,
                 LAVCOD_FERTIRRIGAZIONE,
                 LAVCOD_SARCHIATURA_CONCIMAZIONE
                TipoIntervento = "Concimazione"

            Case LAVCOD_RACCOLTA
                TipoIntervento = "Raccolta"

            Case Else
                Exit Sub
        End Select

        '----- Elemento <DatiNonConformi>
        XML_DatiNonConformi = XML_DatiGenerali.SelectSingleNode("DatiNonConformi")

        If Not XML_DatiNonConformi.HasChildNodes Then
            RisultatoVerifica = "L'Intervento Colturale risulta essere conforme"
            Conforme = True
        Else
            RisultatoVerifica = "L'Intervento Colturale NON risulta essere conforme"
            Conforme = False
            XMLs_DatoNonConforme = XML_DatiNonConformi.GetElementsByTagName("DatoNonConforme")
        End If

        Dim UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME As String = "ND"

        Dim UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_PRODOTTONOAVVERSITA_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_DOSENONDISPONIBILE_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_DOSEUDMNONCONFORME_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_DATADPIMIN_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_DATADPIMAX_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_IMPIANTINONCOERENTIDPI_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_DOSEDPIDISERBOMAX_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_DPI_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_NUMERO_MINIMO_TRATTAMENTI_DPI_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_DOSERAME5ANNIMAX_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_EPOCADPINONCONFORME_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_LIVELLO_CHK_DPI As String = "ND"

        Dim UTENTE_COD_BLOCCA_ACQUAMASSIMA_DPI_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_PRODOTTO_RELAZIONE_FORMULATO_SALVATAGGIO As String = "ND"
        Dim UTENTE_COD_BLOCCA_PRODOTTO_ETA_IMPIANTO_SALVATAGGIO As String = "ND"

        Dim UTENTE_COD_BLOCCA_MIX_POLVERULENTI_NONPOLVERULENTI_SALVATAGGIO As String = "ND"

        If VerificaSoloControlliImpostazioniUtente Then

            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim Dt_Impostazioni As DataTable

            Dt_Impostazioni = ObjUtenti.Leggi(0,
                                              1,
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "",
                                              "",
                                              objParametri_Utenti)

            If Dt_Impostazioni IsNot Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then
                For i = 0 To Dt_Impostazioni.Rows.Count - 1
                    Select Case CInt(Dt_Impostazioni.Rows(i).Item("Impostazione_Cod"))
                        Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME
                            UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_ACQUAMINIMA_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO
                            UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_PRODOTTONOAVVERSITA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_PRODOTTONOAVVERSITA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSENONDISPONIBILE_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSENONDISPONIBILE_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEUDMNONCONFORME_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSEUDMNONCONFORME_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_IMPIANTINONCOERENTIDPI_SALVATAGGIO
                            UTENTE_COD_BLOCCA_IMPIANTINONCOERENTIDPI_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DATADPIMAX_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DATADPIMAX_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DATADPIMIN_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DATADPIMIN_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_EPOCADPINONCONFORME_SALVATAGGIO
                            UTENTE_COD_BLOCCA_EPOCADPINONCONFORME_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEDPIDISERBOMAX_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSEDPIDISERBOMAX_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_DPI_SALVATAGGIO
                            UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_DPI_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_NUMERO_MINIMO_TRATTAMENTI_DPI_SALVATAGGIO
                            UTENTE_COD_BLOCCA_NUMERO_MINIMO_TRATTAMENTI_DPI_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_DOSERAME5ANNIMAX_SALVATAGGIO
                            UTENTE_COD_BLOCCA_DOSERAME5ANNIMAX_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_LIVELLO_CHK_DPI
                            UTENTE_COD_LIVELLO_CHK_DPI = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_ACQUAMASSIMA_DPI_SALVATAGGIO
                            UTENTE_COD_BLOCCA_ACQUAMASSIMA_DPI_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA_SALVATAGGIO
                            UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                        Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_MIX_POLVERULENTI_NONPOLVERULENTI_SALVATAGGIO
                            UTENTE_COD_BLOCCA_MIX_POLVERULENTI_NONPOLVERULENTI_SALVATAGGIO = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                    End Select
                Next
            End If
        End If

        '---------------------------------------------------
        'DT DATI INTERVENTO VERIFICATO

        Dr_InterventoVericato = DT_InterventoVericato.NewRow

        Dr_InterventoVericato.Item("TipoIntervento") = TipoIntervento
        Dr_InterventoVericato.Item("des_lib") = DescrizioneOperazione
        Dr_InterventoVericato.Item("data_movimento") = CDate(Data_Movimento)
        Dr_InterventoVericato.Item("RisultatoVerifica") = RisultatoVerifica
        Dr_InterventoVericato.Item("Piva") = Piva
        Dr_InterventoVericato.Item("Sa_Cod") = sacod
        Dr_InterventoVericato.Item("Lav_Cod") = Lav_Cod
        Dr_InterventoVericato.Item("Id_Agenda") = Id_Agenda
        Dr_InterventoVericato.Item("Xml_Risultato_Verifica") = Xml_Risultato_Verifica
        Dr_InterventoVericato.Item("BooleanRisVer") = Conforme.ToString
        Dr_InterventoVericato.Item("Biologico") = Biologico.ToString

        Dr_InterventoVericato.Item("superficie") = Superficie.ToString
        Dr_InterventoVericato.Item("appezzamenti") = Appezzamenti

        Dr_InterventoVericato.Item("Disciplinare_cod") = Disciplinare_cod
        Dr_InterventoVericato.Item("Disciplinare_Des") = Disciplinare_Des

        Dr_InterventoVericato.Item("sa_nome") = ""

        If DtCentri IsNot Nothing Then
            Dim drCentro As DataRow() = DtCentri.Select("piva='" & Piva & "' and sa_cod=" & sacod)
            If drCentro IsNot Nothing AndAlso drCentro.Length > 0 Then
                Dr_InterventoVericato.Item("sa_nome") = drCentro(0).Item("sa_nome")
            End If
        End If


        DT_InterventoVericato.Rows.Add(Dr_InterventoVericato)

        '---------------------------------------------------
        'DT TUTTI CONTROLLI FATTI

        Select Case CInt(Lav_Cod)

            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                 LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                 LAVCOD_GEODISINFESTAZIONE,
                 LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO,
                 LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                 LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA,
                 LAVCOD_REINNESCO_TRAPPOLE

                '---------------------------------------------------
                ' ETICHETTA

                'DoseNonDisponibile
                Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.ProdottoNonUtilizzabileXData
                Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Utilizzabilità Prodotto alla data (Revoca/Sospensione/Fine Comm./Fine Scorte)"
                Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                Dr_RisultatiVerifiche.Item("pro_cod") = 0
                Dr_RisultatiVerifiche.Item("pro_des") = ""
                Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                'ProdottoNonRegistratoSuColtura
                Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.ProdottoNonRegistratoSuColtura
                Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Registrazione Prodotto su Coltura"
                Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                Dr_RisultatiVerifiche.Item("pro_cod") = 0
                Dr_RisultatiVerifiche.Item("pro_des") = ""
                Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                'Epoca
                If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_FINO_FIORITURA_ETICHETTA_SALVATAGGIO <> "0" Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.Epoca_Etichetta
                    Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Rispettata Epoca/Fioritura/Fase fenologica"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                'CARENZA
                If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA_SALVATAGGIO <> "0" Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.CarenzaNonRispettata
                    Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Rispetto Carenza alla Data Raccolta"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                'buffer zone
                If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_BUFFERZONE_ETICHETTA_SALVATAGGIO <> "0" Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.BufferZoneNonRispettata
                    Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Rispetto Buffer Zone (Fasce Rispetto)"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                'ProdottoNonGiustificatoSuAvversita
                If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_PRODOTTONOAVVERSITA_SALVATAGGIO <> "0" Then

                    'DoseNonDisponibile
                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.DoseNonDisponibile 'enTipoErrCode_Verifica.ProdottoNonGiustificatoSuAvversita
                    Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Giustificazione Prodotto su Avversita/Infestante"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                'DOSE MAX
                If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_DOSEMASSIMA_ETICHETTA_SALVATAGGIO <> "0" Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.DoseEccessiva
                    Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Dose Massima Prodotto"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                'DOSE MIN
                If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_DOSEMINIMA_ETICHETTA_SALVATAGGIO <> "0" Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.DoseInsufficiente
                    Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Dose Minima Prodotto"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                'Marco: DOSE MAX ANNO
                If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO <> "0" Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.DoseEccessivaEtichetta_Anno
                    Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Dose Massima Prodotto per Anno"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                'Marco: DOSE MAX ANNO X AVVERSITA'-INFESTANTI
                If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_DOSEMASSIMA_ANNO_ETICHETTA_SALVATAGGIO <> "0" Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.DoseEccessivaEtichettaAvversita_Anno
                    Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Dose Massima Prodotto per Anno su Avversita/Infestante"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                '(10/07/2019 fede) aggiungere
                If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO <> "0" Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.DoseEccessivaRame
                    Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Dose Rame Anno"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                'ACQUA
                If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_ACQUAMASSIMA_ETICHETTA_SALVATAGGIO <> "0" Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.AcquaNonCorretta
                    Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Quantità Acqua"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                'UDM CONFORME
                If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_DOSEUDMNONCONFORME_SALVATAGGIO <> "0" Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.UnitaMisuraNonCompatibile
                    Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Compatibilità Unita Misura"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                'Num Max Interventi Prodotto
                If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_ETICHETTA_SALVATAGGIO <> "0" Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.SuperatoNumMaxInterventiProdotto
                    Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Numero Massimo Interventi ammesso"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                'Intervallo tra Trattamenti
                If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_INTERVALLOTRATTAMENTI_ETICHETTA_SALVATAGGIO <> "0" Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.IntervalloInterventiNonRispettato
                    Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Intervallo tra Trattamenti"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                'ProdottoVincolatoFormulato
                If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_PRODOTTO_RELAZIONE_FORMULATO_SALVATAGGIO <> "0" Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.ProdottoVincolatoFormulato
                    Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Prodotto Vincolato da Miscela Con Altro Formulato"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                'mix polverulenti
                If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_MIX_POLVERULENTI_NONPOLVERULENTI_SALVATAGGIO <> "0" Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.MixPolveruentiENon
                    Dr_RisultatiVerifiche.Item("err_des") = "(ETI) Miscibilità prodotti polverulenti e non"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If


                '---------------------------------------------------
                ' DPI

                If Disciplinare_cod <> 0 Then

                    Select Case Lav_Cod

                        Case LAVCOD_TRATTAMENTO_FITOREGOLATORE

                            Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                            Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                            Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.ProdottoNonUtilizzabileXDisciplinare
                            Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Sostanza Attiva ammessa da Disciplinare"
                            Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                            Dr_RisultatiVerifiche.Item("pro_cod") = 0
                            Dr_RisultatiVerifiche.Item("pro_des") = ""
                            Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                            DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                        Case Else

                            '(23/07/2019 fede) aggiunto controllo SA escluse per la classe tossicologica
                            Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                            Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                            Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.ProdottoNonUtilizzabileXDisciplinare
                            Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Sostanza Attiva ammessa da Disciplinare"
                            Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                            Dr_RisultatiVerifiche.Item("pro_cod") = 0
                            Dr_RisultatiVerifiche.Item("pro_des") = ""
                            Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                            DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)


                            If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_AVVERSITANODPI_SALVATAGGIO <> "0" Then

                                Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                                Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                                Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.AvversitaNonGiustificataDPI
                                Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Avversita/Infestante giustificata da Disciplinare"
                                Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                                Dr_RisultatiVerifiche.Item("pro_cod") = 0
                                Dr_RisultatiVerifiche.Item("pro_des") = ""
                                Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                                DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                            End If

                            Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                            Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                            Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.AvversitaNonTrattabileDPI
                            Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Giustificazione Sostanza su Avversita/Infestante"
                            Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                            Dr_RisultatiVerifiche.Item("pro_cod") = 0
                            Dr_RisultatiVerifiche.Item("pro_des") = ""
                            Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                            DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                            Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                            Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                            Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.ProdottoNonGiustificatoSuAvversitaDPI
                            Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Giustificazione Prodotto su Avversita/Infestante"
                            Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                            Dr_RisultatiVerifiche.Item("pro_cod") = 0
                            Dr_RisultatiVerifiche.Item("pro_des") = ""
                            Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                            DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                            Select Case Lav_Cod

                                Case LAVCOD_GEODISINFESTAZIONE, LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                                     LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA, LAVCOD_REINNESCO_TRAPPOLE
                                    If (UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME = "ND" AndAlso UTENTE_COD_LIVELLO_CHK_DPI = "ND") OrElse
                                   (UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_LIVELLO_CHK_DPI = "2") Then
                                        'If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" And UTENTE_COD_LIVELLO_CHK_DPI <> "0" Then
                                        Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                                        Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                                        Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.SogliaNonRegistrata
                                        Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Registrazione adeguata della Giustificazione (Catture/Rilievi)"
                                        Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                                        Dr_RisultatiVerifiche.Item("pro_cod") = 0
                                        Dr_RisultatiVerifiche.Item("pro_des") = ""
                                        Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                                        DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)
                                    End If
                            End Select
                    End Select

                    'ImpiantiNonCoerentiDPI = 700
                    'ImpiantiRaggruppamentoColturaleNonOmogeneo = 701
                    'ImpiantiCoperturaNonOmogeneo = 702
                    'ImpiantiDPINonImpostato = 703 'non gestito
                    'ImpiantiDpiNonOmogeneo = 704
                    If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_IMPIANTINONCOERENTIDPI_SALVATAGGIO <> "0" Then

                        Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                        Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                        Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.ImpiantiNonCoerentiDPI
                        Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Utilizzo Prodotto non ammesso in agricoltura Bio/Integrata"
                        Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                        Dr_RisultatiVerifiche.Item("pro_cod") = 0
                        Dr_RisultatiVerifiche.Item("pro_des") = ""
                        Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                        DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                        Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                        Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                        Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.ImpiantiRaggruppamentoColturaleNonOmogeneo
                        Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Raggruppamento colturale non omogeneo"
                        Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                        Dr_RisultatiVerifiche.Item("pro_cod") = 0
                        Dr_RisultatiVerifiche.Item("pro_des") = ""
                        Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                        DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                    End If

                    If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_NUMERO_MASSIMO_TRATTAMENTI_DPI_SALVATAGGIO <> "0" Then

                        Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                        Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                        Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.SuperatoNumMaxInterventiPA
                        Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Numero Massimo utilizzo Sostanze/Miscele"
                        Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                        Dr_RisultatiVerifiche.Item("pro_cod") = 0
                        Dr_RisultatiVerifiche.Item("pro_des") = ""
                        Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                        DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                    End If

                    If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_NUMERO_MINIMO_TRATTAMENTI_DPI_SALVATAGGIO <> "0" Then

                        Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                        Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                        Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.NonRaggiuntoNumMinInterventiPA
                        Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Numero Minimo utilizzo Sostanze/Miscele"
                        Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                        Dr_RisultatiVerifiche.Item("pro_cod") = 0
                        Dr_RisultatiVerifiche.Item("pro_des") = ""
                        Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                        DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                    End If

                    'ProdottoVincolatoFormulato
                    If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_PRODOTTO_ETA_IMPIANTO_SALVATAGGIO <> "0" Then

                        Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                        Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                        Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.ProdottoNonUtilizzabileXDisciplinarexStatoImpianto
                        Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Sostanza Attiva ammessa in base allo Stato Impianto"
                        Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                        Dr_RisultatiVerifiche.Item("pro_cod") = 0
                        Dr_RisultatiVerifiche.Item("pro_des") = ""
                        Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                        DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                    End If


                    'Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    'Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    'Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.IncompatibilitaTraSostanze
                    'Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Incompatibilità tra Sostanze Attive (Interazioni)"
                    'Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    'Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    'Dr_RisultatiVerifiche.Item("pro_des") = ""
                    'Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    'DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)


                    Select Case CInt(Lav_Cod)

                        Case LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO

                            If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_DOSEDPIDISERBOMAX_SALVATAGGIO <> "0" Then

                                Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                                Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                                Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.DoseDiserboEccessiva
                                Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Dose Diserbo"
                                Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                                Dr_RisultatiVerifiche.Item("pro_cod") = 0
                                Dr_RisultatiVerifiche.Item("pro_des") = ""
                                Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                                DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                            End If

                            If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_DOSEDPIDISERBOANNOMAX_SALVATAGGIO <> "0" Then

                                Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                                Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                                Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.DoseEccessivaSADpi_Anno
                                Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Dose Diserbo Anno"
                                Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                                Dr_RisultatiVerifiche.Item("pro_cod") = 0
                                Dr_RisultatiVerifiche.Item("pro_des") = ""
                                Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                                DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                            End If

                    End Select

                    If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_EPOCADPINONCONFORME_SALVATAGGIO <> "0" Then

                        Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                        Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                        Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.Epoca_PreRaccolta
                        Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Rispetto Distanza Giorni Raccolta"
                        Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                        Dr_RisultatiVerifiche.Item("pro_cod") = 0
                        Dr_RisultatiVerifiche.Item("pro_des") = ""
                        Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                        DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                        Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                        Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                        Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.Epoca_PreSemina
                        Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Rispetto Distanza Giorni Semina/Trapianto"
                        Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                        Dr_RisultatiVerifiche.Item("pro_cod") = 0
                        Dr_RisultatiVerifiche.Item("pro_des") = ""
                        Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                        DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                    End If

                    If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_DATADPIMIN_SALVATAGGIO <> "0" Then

                        Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                        Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                        Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.DataInterventoMin
                        Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Data Intervento Minima Disciplinare"
                        Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                        Dr_RisultatiVerifiche.Item("pro_cod") = 0
                        Dr_RisultatiVerifiche.Item("pro_des") = ""
                        Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                        DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                    End If

                    If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_DATADPIMAX_SALVATAGGIO <> "0" Then

                        Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                        Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                        Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.DataInterventoMax
                        Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Data Intervento Massima Disciplinare"
                        Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                        Dr_RisultatiVerifiche.Item("pro_cod") = 0
                        Dr_RisultatiVerifiche.Item("pro_des") = ""
                        Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                        DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                    End If

                    If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO <> "0" Then

                        Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                        Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                        Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.DoseEccessivaRame
                        Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Dose Rame Anno"
                        Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                        Dr_RisultatiVerifiche.Item("pro_cod") = 0
                        Dr_RisultatiVerifiche.Item("pro_des") = ""
                        Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                        DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                    End If

                    '(16/01/2018) aggiunta acqua
                    If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_ACQUAMASSIMA_DPI_SALVATAGGIO <> "0" Then

                        Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                        Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                        Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.SuperatoVolumeMaxAcquaDpi
                        Dr_RisultatiVerifiche.Item("err_des") = "(DPI) Volume Irrorazione Massimo"
                        Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                        Dr_RisultatiVerifiche.Item("pro_cod") = 0
                        Dr_RisultatiVerifiche.Item("pro_des") = ""
                        Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                        DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                    End If


                End If

                '---------------------------------------------------
                ' BIO

                If Biologico Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.InterventoNonConsentito
                    Dr_RisultatiVerifiche.Item("err_des") = "(BIO) Tipologia Intervento Consentita"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.ProdottoNonBiologico
                    Dr_RisultatiVerifiche.Item("err_des") = "(BIO) Prodotto Biologico"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                    If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_DOSERAMEANNOMAX_SALVATAGGIO <> "0" Then

                        Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                        Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                        Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.DoseEccessivaRame
                        Dr_RisultatiVerifiche.Item("err_des") = "(BIO) Dose Rame Anno"
                        Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                        Dr_RisultatiVerifiche.Item("pro_cod") = 0
                        Dr_RisultatiVerifiche.Item("pro_des") = ""
                        Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                        DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                    End If

                    If UTENTE_COD_DEFAULT_BLOCCO_SALVA_NOCONFORME <> "0" AndAlso UTENTE_COD_BLOCCA_DOSERAME5ANNIMAX_SALVATAGGIO <> "0" Then

                        Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                        Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                        Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.DoseEccessivaRame5Anni
                        Dr_RisultatiVerifiche.Item("err_des") = "(BIO) Dose Media Rame 5 Anni"
                        Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                        Dr_RisultatiVerifiche.Item("pro_cod") = 0
                        Dr_RisultatiVerifiche.Item("pro_des") = ""
                        Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                        DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                    End If

                End If


            Case LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                 LAVCOD_CONCIMAZIONE_FOGLIARE,
                 LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                 LAVCOD_DISTRIBUZIONE_CONCIME,
                 LAVCOD_FERTIRRIGAZIONE,
                 LAVCOD_SARCHIATURA_CONCIMAZIONE

                If Biologico Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.ProdottoNonBiologico
                    Dr_RisultatiVerifiche.Item("err_des") = "Fertilizzante Biologico"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.Superato_N_Max
                Dr_RisultatiVerifiche.Item("err_des") = "N Massimo"
                Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                Dr_RisultatiVerifiche.Item("pro_cod") = 0
                Dr_RisultatiVerifiche.Item("pro_des") = ""
                Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                If Disciplinare_cod > 0 Then

                    Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                    Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                    Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.Superato_N_Max_Intervento
                    Dr_RisultatiVerifiche.Item("err_des") = "N Massimo per Intervento"
                    Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                    Dr_RisultatiVerifiche.Item("pro_cod") = 0
                    Dr_RisultatiVerifiche.Item("pro_des") = ""
                    Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                    DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                End If

                Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.Superato_P_Max
                Dr_RisultatiVerifiche.Item("err_des") = "P Massimo"
                Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                Dr_RisultatiVerifiche.Item("pro_cod") = 0
                Dr_RisultatiVerifiche.Item("pro_des") = ""
                Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.Superato_K_Max
                Dr_RisultatiVerifiche.Item("err_des") = "K Massimo"
                Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                Dr_RisultatiVerifiche.Item("pro_cod") = 0
                Dr_RisultatiVerifiche.Item("pro_des") = ""
                Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

                'AF: 05/23 -- IL CONTROLLO RAME SI FA A PRESCINDERE DAL REGOLAMENTO SELEZIONATO O MENO
                'If Disciplinare_cod > 0 Or Biologico = True Then
                Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.DoseEccessivaRame
                Dr_RisultatiVerifiche.Item("err_des") = "Cu Massimo"
                Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                Dr_RisultatiVerifiche.Item("pro_cod") = 0
                Dr_RisultatiVerifiche.Item("pro_des") = ""
                Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)
                'End If

                Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.Superato_M_Max
                Dr_RisultatiVerifiche.Item("err_des") = "Mg Massimo"
                Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                Dr_RisultatiVerifiche.Item("pro_cod") = 0
                Dr_RisultatiVerifiche.Item("pro_des") = ""
                Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

            Case LAVCOD_RACCOLTA

                Dr_RisultatiVerifiche = DT_RisultatiVerifiche.NewRow
                Dr_RisultatiVerifiche.Item("id_agenda") = Id_Agenda
                Dr_RisultatiVerifiche.Item("err_code") = enTipoErrCode_Verifica.CarenzaNonRispettata
                Dr_RisultatiVerifiche.Item("err_des") = "Rispetto Carenza Prodotti"
                Dr_RisultatiVerifiche.Item("Err_Nota") = ""
                Dr_RisultatiVerifiche.Item("pro_cod") = 0
                Dr_RisultatiVerifiche.Item("pro_des") = ""
                Dr_RisultatiVerifiche.Item("BooleanRisVer") = "true"
                DT_RisultatiVerifiche.Rows.Add(Dr_RisultatiVerifiche)

        End Select


        'ANALIZZO LE NON CONFORMITA

        XMLs_DatoNonConforme = XML_DatiNonConformi.GetElementsByTagName("DatoNonConforme")

        Dim Err_Code As Integer
        Dim Err_Nota As String

        Dim Pro_Cod As Integer
        Dim Pro_Des As String

        Dim DrErr As DataRow()

        For x = 0 To XMLs_DatoNonConforme.Count - 1

            XML_DatoNonConforme = XMLs_DatoNonConforme.Item(x)

            Err_Code = CInt(XML_DatoNonConforme.GetAttribute("err_code"))
            Err_Nota = CStr(XML_DatoNonConforme.GetAttribute("err_des"))

            Pro_Cod = CInt(XML_DatoNonConforme.GetAttribute("pro_cod"))
            Pro_Des = CStr(XML_DatoNonConforme.GetAttribute("pro_des"))

            DrErr = DT_RisultatiVerifiche.Select("id_agenda=" & Id_Agenda.ToString & " AND err_code=" & Err_Code.ToString)

            If DrErr IsNot Nothing AndAlso DrErr.Length > 0 Then

                DrErr(0).Item("BooleanRisVer") = "false"

                If InStr(DrErr(0).Item("Err_Nota"), Err_Nota) = 0 Then
                    If DrErr(0).Item("pro_cod") = "0" Then
                        DrErr(0).Item("pro_cod") = Pro_Cod
                        DrErr(0).Item("pro_des") = Pro_Des
                    Else
                        DrErr(0).Item("pro_cod") &= "," & Pro_Cod
                        DrErr(0).Item("pro_des") &= "," & Pro_Des
                    End If
                    If Pro_Des <> "" Then
                        DrErr(0).Item("Err_Nota") &= Pro_Des & ":" & Err_Nota
                    Else
                        DrErr(0).Item("Err_Nota") &= Err_Nota
                    End If
                End If

            End If

        Next

    End Sub

    Public Sub Crea_DT_InterventoVerificato(ByRef DT_InterventoVericato As DataTable)

        DT_InterventoVericato.Columns.Add(New DataColumn("data_movimento", GetType(Date)))
        DT_InterventoVericato.Columns.Add(New DataColumn("TipoIntervento", GetType(String)))
        DT_InterventoVericato.Columns.Add(New DataColumn("des_lib", GetType(String)))
        DT_InterventoVericato.Columns.Add(New DataColumn("Disciplinare_cod", GetType(Integer)))
        DT_InterventoVericato.Columns.Add(New DataColumn("Disciplinare_Des", GetType(String)))
        DT_InterventoVericato.Columns.Add(New DataColumn("RisultatoVerifica", GetType(String)))
        DT_InterventoVericato.Columns.Add(New DataColumn("Piva", GetType(String)))
        DT_InterventoVericato.Columns.Add(New DataColumn("Sa_Cod", GetType(Integer)))
        DT_InterventoVericato.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
        DT_InterventoVericato.Columns.Add(New DataColumn("Id_Agenda", GetType(Integer)))
        DT_InterventoVericato.Columns.Add(New DataColumn("Xml_Risultato_Verifica", GetType(String)))
        DT_InterventoVericato.Columns.Add(New DataColumn("BooleanRisVer", GetType(String)))
        DT_InterventoVericato.Columns.Add(New DataColumn("Biologico", GetType(String)))

        DT_InterventoVericato.Columns.Add(New DataColumn("Appezzamenti", GetType(String)))
        DT_InterventoVericato.Columns.Add(New DataColumn("Superficie", GetType(String)))

        DT_InterventoVericato.Columns.Add(New DataColumn("sa_nome", GetType(String)))

    End Sub

    Public Sub Crea_DT_RisultatiVerifiche(ByRef DT_RisultatiVerifiche As DataTable)

        DT_RisultatiVerifiche.Columns.Add(New DataColumn("id_agenda", GetType(Integer)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("err_code", GetType(Integer)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("err_des", GetType(String)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("err_nota", GetType(String)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("pro_cod", GetType(String)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("pro_des", GetType(String)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("BooleanRisVer", GetType(String)))

    End Sub

    Public Sub Crea_DT_ImpiantoVerificato(ByRef DT_ImpiantoVericato As DataTable)

        DT_ImpiantoVericato.Columns.Add(New DataColumn("chiave", GetType(String)))
        DT_ImpiantoVericato.Columns.Add(New DataColumn("piva", GetType(String)))
        DT_ImpiantoVericato.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
        DT_ImpiantoVericato.Columns.Add(New DataColumn("appezza", GetType(Integer)))
        DT_ImpiantoVericato.Columns.Add(New DataColumn("id_reg", GetType(Integer)))
        DT_ImpiantoVericato.Columns.Add(New DataColumn("progetto_cod", GetType(Integer)))

        DT_ImpiantoVericato.Columns.Add(New DataColumn("sa_nome", GetType(String)))
        DT_ImpiantoVericato.Columns.Add(New DataColumn("app_nome", GetType(String)))
        DT_ImpiantoVericato.Columns.Add(New DataColumn("superficie", GetType(String)))

        DT_ImpiantoVericato.Columns.Add(New DataColumn("Disciplinare_cod", GetType(Integer)))
        DT_ImpiantoVericato.Columns.Add(New DataColumn("Disciplinare_Des", GetType(String)))

        DT_ImpiantoVericato.Columns.Add(New DataColumn("RisultatoVerifica", GetType(String)))

        DT_ImpiantoVericato.Columns.Add(New DataColumn("BooleanRisVer", GetType(String)))
        DT_ImpiantoVericato.Columns.Add(New DataColumn("Biologico", GetType(String)))

    End Sub

    Public Sub Crea_DT_ImpiantoRisultatiVerificheIAF(ByRef DT_ImpiantoVericatoDettagliIAF As DataTable)

        DT_ImpiantoVericatoDettagliIAF.Columns.Add(New DataColumn("chiave", GetType(String)))
        DT_ImpiantoVericatoDettagliIAF.Columns.Add(New DataColumn("iaf_cod", GetType(Integer)))
        DT_ImpiantoVericatoDettagliIAF.Columns.Add(New DataColumn("iaf_des", GetType(String)))
        DT_ImpiantoVericatoDettagliIAF.Columns.Add(New DataColumn("BooleanRisVer", GetType(String)))

    End Sub

    Public Sub InserisciRiga_DT_ImpiantoVerificato(ByRef DT_ImpiantoVericato As DataTable,
                                                   ByVal Chiave As String,
                                                   ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer, ByVal Id_Reg As Integer, ByVal Progetto_Cod As Integer,
                                                   ByVal Sa_Nome As String, ByVal App_Nome As String, ByVal Superficie As String, ByVal Disciplinare_Cod As Integer, ByVal Disciplinare_Des As String,
                                                   ByVal RisultatoVerifica As String, ByVal BooleanRisVer As String, ByVal Biologico As String)

        Dim Dr As DataRow

        Dr = DT_ImpiantoVericato.NewRow
        Dr.Item("chiave") = Chiave
        Dr.Item("piva") = Piva
        Dr.Item("sa_cod") = Sa_Cod
        Dr.Item("appezza") = Appezza
        Dr.Item("id_reg") = Id_Reg
        Dr.Item("progetto_cod") = Progetto_Cod

        Dr.Item("sa_nome") = Sa_Nome
        Dr.Item("app_nome") = App_Nome
        Dr.Item("superficie") = Superficie

        Dr.Item("disciplinare_cod") = Disciplinare_Cod
        Dr.Item("disciplinare_des") = Disciplinare_Des

        Dr.Item("RisultatoVerifica") = RisultatoVerifica

        Dr.Item("BooleanRisVer") = BooleanRisVer
        Dr.Item("Biologico") = Biologico

        DT_ImpiantoVericato.Rows.Add(Dr)

    End Sub

    Public Sub InserisciRiga_DT_ImpiantoDettagliIAF(ByRef DT_ImpiantoVericatoDettagliIAF As DataTable,
                                               ByVal Chiave As String,
                                               ByVal iaf_cod As Integer, ByVal iaf_des As String,
                                               ByVal BooleanRisVer As String)

        Dim Dr As DataRow

        Dr = DT_ImpiantoVericatoDettagliIAF.NewRow
        Dr.Item("chiave") = Chiave
        Dr.Item("iaf_cod") = iaf_cod
        Dr.Item("iaf_des") = iaf_des
        Dr.Item("BooleanRisVer") = BooleanRisVer
        DT_ImpiantoVericatoDettagliIAF.Rows.Add(Dr)

    End Sub

    Public Sub Crea_DT_InterventoVerificatoMagazzino(ByRef DT_InterventoVericato As DataTable)

        DT_InterventoVericato.Columns.Add(New DataColumn("data_movimento", GetType(Date)))
        DT_InterventoVericato.Columns.Add(New DataColumn("des_lib", GetType(String)))
        DT_InterventoVericato.Columns.Add(New DataColumn("fabbricato_cod", GetType(String)))
        DT_InterventoVericato.Columns.Add(New DataColumn("fabbricato_des", GetType(String)))
        DT_InterventoVericato.Columns.Add(New DataColumn("piva", GetType(String)))
        DT_InterventoVericato.Columns.Add(New DataColumn("sa_nome", GetType(String)))
        DT_InterventoVericato.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
        DT_InterventoVericato.Columns.Add(New DataColumn("lav_cod", GetType(Integer)))
        DT_InterventoVericato.Columns.Add(New DataColumn("id_agenda", GetType(Integer)))
        DT_InterventoVericato.Columns.Add(New DataColumn("appezzamenti", GetType(String)))
        DT_InterventoVericato.Columns.Add(New DataColumn("superficie", GetType(String)))

        DT_InterventoVericato.Columns.Add(New DataColumn("BooleanRisVer", GetType(String)))
        DT_InterventoVericato.Columns.Add(New DataColumn("RisultatoVerifica", GetType(String)))

    End Sub

    Public Sub Crea_DT_RisultatiVerificheMagazzino(ByRef DT_RisultatiVerifiche As DataTable, Optional salvaMagazzino As Boolean = False)

        DT_RisultatiVerifiche.Columns.Add(New DataColumn("id_agenda", GetType(Integer)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("elem_cod", GetType(Integer)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("pro_cod", GetType(Integer)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("mat_cod", GetType(Integer)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("cod_progetto", GetType(Integer)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("cal_cod", GetType(Integer)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("lotto", GetType(String)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("prodotto", GetType(String)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("udm_sim", GetType(String)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("udm_cod", GetType(Integer)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("qta", GetType(Decimal)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("qta_presente", GetType(Decimal)))

        If salvaMagazzino Then
            DT_RisultatiVerifiche.Columns.Add(New DataColumn("fabbricato_cod", GetType(String)))
            DT_RisultatiVerifiche.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
            DT_RisultatiVerifiche.Columns.Add(New DataColumn("piva", GetType(String)))
        End If


        DT_RisultatiVerifiche.Columns.Add(New DataColumn("BooleanRisVer", GetType(String)))
        DT_RisultatiVerifiche.Columns.Add(New DataColumn("RisultatoVerifica", GetType(String)))

    End Sub

    Public Sub Crea_DT_DettagliGiacenzeMagazzino(ByRef dt As DataTable)
        dt.Clear()
        dt.Columns.Clear()

        ' Struttura basata sull'immagine fornita
        dt.Columns.Add("Id_Agenda", GetType(Integer))
        dt.Columns.Add("Id_Esito_Giacenza", GetType(Integer))
        dt.Columns.Add("Id_Testata", GetType(Integer))
        dt.Columns.Add("Piva_Magazzino", GetType(String))
        dt.Columns.Add("Sa_Cod_Magazzino", GetType(Integer))
        dt.Columns.Add("Fabbricato_Cod_Magazzino", GetType(Integer))
        dt.Columns.Add("Lotto", GetType(String))
        dt.Columns.Add("Conforme", GetType(Integer))
        dt.Columns.Add("Id_Prodotto", GetType(Integer))
        dt.Columns.Add("Categoria_Prodotto", GetType(Integer))
        dt.Columns.Add("Giacenza_Magazzino", GetType(Decimal))
        dt.Columns.Add("Unita_Misura", GetType(Integer))
        dt.Columns.Add("Dettagli", GetType(String))
        dt.Columns.Add("DataGiacenza", GetType(DateTime))
        dt.Columns.Add("TipoGiacenza", GetType(Integer))
    End Sub

    Public Sub InserisciRiga_DT_InterventoVerificatoMagazzino(ByRef DT_InterventoVericato As DataTable,
                                                   ByVal data_movimento As Date,
                                                   ByVal des_lib As String, ByVal Fabbricato_cod As String, ByVal Fabbricato_Des As String,
                                                              ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Sa_Nome As String,
                                                              ByVal lav_cod As Integer, ByVal id_agenda As Integer, ByVal Appezzamenti As String, ByVal Superficie As String,
                                                                    ByVal BooleanRisVer As String, ByVal RisultatoVerifica As String)



        Dim Dr As DataRow

        Dr = DT_InterventoVericato.NewRow

        Dr.Item("data_movimento") = data_movimento
        Dr.Item("des_lib") = des_lib
        Dr.Item("fabbricato_cod") = Fabbricato_cod
        Dr.Item("fabbricato_des") = Fabbricato_Des
        Dr.Item("RisultatoVerifica") = RisultatoVerifica
        Dr.Item("piva") = Piva
        Dr.Item("sa_cod") = Sa_Cod
        Dr.Item("sa_nome") = Sa_Nome
        Dr.Item("lav_cod") = lav_cod
        Dr.Item("id_agenda") = id_agenda
        Dr.Item("appezzamenti") = Appezzamenti
        Dr.Item("superficie") = Superficie
        Dr.Item("RisultatoVerifica") = RisultatoVerifica
        Dr.Item("BooleanRisVer") = BooleanRisVer

        DT_InterventoVericato.Rows.Add(Dr)

    End Sub

    Public Sub InserisciRiga_DT_RisultatiVerificheMagazzino(ByRef DT_RisultatiVerifiche As DataTable,
                                               ByVal id_agenda As Integer,
                                                   ByVal elem_cod As Integer, ByVal pro_cod As Integer, ByVal mat_cod As Integer, ByVal cod_progetto As Integer, ByVal cal_cod As Integer, ByVal lotto As String,
                                                       ByVal Prodotto As String,
                                                            ByVal udm_sim As String, ByVal udm_cod As Integer, ByVal qta As Decimal, ByVal qta_presente As Decimal,
                                                                    ByVal BooleanRisVer As String, ByVal RisultatoVerifica As String,
                                                                         Optional salvaMagazzino As Boolean = False, Optional piva As String = "", Optional sa_cod As Integer = 0, Optional fabbricato_cod As String = "")

        Dim Dr As DataRow

        Dr = DT_RisultatiVerifiche.NewRow
        Dr.Item("id_agenda") = id_agenda
        Dr.Item("elem_cod") = elem_cod
        Dr.Item("pro_cod") = pro_cod
        Dr.Item("mat_cod") = mat_cod
        Dr.Item("cod_progetto") = cod_progetto
        Dr.Item("cal_cod") = cal_cod
        Dr.Item("lotto") = lotto
        Dr.Item("prodotto") = Prodotto
        Dr.Item("udm_sim") = udm_sim
        Dr.Item("udm_cod") = udm_cod
        Dr.Item("qta") = qta
        Dr.Item("qta_presente") = qta_presente
        Dr.Item("BooleanRisVer") = BooleanRisVer
        Dr.Item("RisultatoVerifica") = RisultatoVerifica
        If salvaMagazzino Then
            Dr.Item("fabbricato_cod") = fabbricato_cod
            Dr.Item("sa_cod") = sa_cod
            Dr.Item("piva") = piva
        End If
        DT_RisultatiVerifiche.Rows.Add(Dr)

    End Sub

    Public Sub InserisciRiga_DT_DettagliGiacenzeMagazzino(ByRef dt As DataTable,
                                                          ByVal id_agenda As Integer,
                                                          idTestata As Integer,
                                                          pivaMagazzino As String,
                                                          saCodMagazzino As Integer,
                                                          fabbricatoCodMagazzino As Integer,
                                                          lotto As String,
                                                          conforme As Boolean,
                                                          idProdotto As Integer,
                                                          categoriaProdotto As Integer,
                                                          giacenzaMagazzino As Decimal,
                                                          unitaMisura As Integer,
                                                          dettagli As String,
                                                          dataGiacenza As DateTime,
                                                          tipoGiacenza As Integer)

        Dim newRow As DataRow = dt.NewRow()
        newRow("id_agenda") = -1 ' Verrà impostato dal databaseDettagli
        newRow("Id_Esito_Giacenza") = -1 ' Verrà impostato dal databaseDettagli
        newRow("Id_Testata") = idTestata
        newRow("Piva_Magazzino") = pivaMagazzino
        newRow("Sa_Cod_Magazzino") = saCodMagazzino
        newRow("Fabbricato_Cod_Magazzino") = fabbricatoCodMagazzino
        newRow("Lotto") = lotto
        newRow("Conforme") = conforme
        newRow("Id_Prodotto") = idProdotto
        newRow("Categoria_Prodotto") = categoriaProdotto
        newRow("Giacenza_Magazzino") = giacenzaMagazzino
        newRow("Unita_Misura") = unitaMisura
        newRow("Dettagli") = dettagli
        newRow("DataGiacenza") = dataGiacenza
        newRow("TipoGiacenza") = tipoGiacenza

        dt.Rows.Add(newRow)
    End Sub

    Private Function Leggi_FlagNuovoControlloRiduzioneDiserbo(ByVal objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim flagNuovoControlloRiduzioneDiserbo As Boolean = False
        Dim csr As New Configurazione_Siti_R
        Dim dt As DataTable = csr.Leggi(0, "Flag_Nuovo_Controllo_Riduzione_Diserbo", "", "", objParametri_Server)
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            flagNuovoControlloRiduzioneDiserbo = CBool(dt.Rows(0)("Valore"))
        End If
        Return flagNuovoControlloRiduzioneDiserbo

    End Function

End Class


Public Class Verifica_Disciplinare_Intervento

    Public Conforme As Boolean

    Public Risultato As String

    Public strNonConformita As String

End Class

Public Class Verifica_Disciplinare_Impianto

    Public Conforme As Boolean

    Public Risultato As String

    Public Num_Interventi As Integer
    Public Num_InterventiSoggetti As Integer
    Public Num_Interventi_Conformi As Integer
    Public Num_Interventi_Non_Conformi As Integer

End Class