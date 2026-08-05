Imports System.IO
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreXML.XML_Stampe

Public Class ImportConferimentiAgribologna

    Private _ConfigurazioneServizio As Configurazione_Servizio
    Private objParametri_SuperServer As AgronicaCoreParametri
    Private objParametri_Server As AgronicaCoreParametri
    Private objParametri_Utenti As AgronicaCoreParametri

    Private _hostSmtp As String = ""
    Private _mittente As String = ""
    Private _destinatari As String = ""

    ''Oggetti utilizzati
    Private Utenti_Read As AgronicaCoreUtentiDAL.Utenti_Read

    ''Da ricavare da Configurazione_Servizio
    Private SuperUserUsername As String
    Private SuperUserPassword As String
    Private SuperUserPiva As String
    Private Codice_Chiave_Cliente As Integer
    Private Piva_Padre As String
    Private LogFileName As String
    Private LogDirectory As String
    Private LogDescrizioneUtente As String
    Private DirectoryFileImportazioni As String
    Private DirectoryFileEsportazioni As String
    Private Parametri_Extra As String


    Public Sub New(ByVal _Configurazione_Servizio As Configurazione_Servizio,
            ByVal _ObjParametri_SuperServer As AgronicaCoreParametri,
            ByVal _ObjParametri_Server As AgronicaCoreParametri,
            ByVal _ObjParametri_Utenti As AgronicaCoreParametri)

        Parametri_Extra = _Configurazione_Servizio.Parametri_Extra
        'Configurazione_Servizio = _Configurazione_Servizio leggo solo i parametri che mi servono 
        objParametri_SuperServer = _ObjParametri_SuperServer
        objParametri_Server = _ObjParametri_Server
        objParametri_Utenti = _ObjParametri_Utenti

        _mittente = _Configurazione_Servizio.Mittente
        _destinatari = _Configurazione_Servizio.Destinatari

    End Sub


    Public Function Avvia_Import_Conferimenti_Agribologna(Optional ByVal bEmail As Boolean = True) As RispostaStandard

        Dim r As New RispostaStandard
        Dim erroreMail As String = ""
        Dim objMail As New Mail

        Try

            r = Importazione_Tracciabilita_Agribologna_Sync(objParametri_Server, objParametri_Utenti)


        Catch ex As Exception
            r.RispostaStringa &= "Errore durante l'importazione: " & ex.Message
            r.Errore &= "Errore durante l'importazione: " & ex.Message
            r.RispostaOK = False
        End Try

        Try
            If bEmail Then

                erroreMail = objMail.invia(objParametri_Server, _mittente, _destinatari, "", "",
                                           "Importazione Conferimenti " & r.RispostaStringa,
                                           r.Errore, False, Nothing)

            End If

            'Concateno il messaggio eventualmente dato a video
            r.RispostaStringa = r.Errore & " ------ " & r.RispostaStringa

        Catch ex As Exception
            r.RispostaStringa = r.Errore & " ------ " & r.RispostaStringa & " ------ " & "Non è stato possibile inviare l'email. " & ex.Message
            r.RispostaOK = True
        End Try


        Return r

    End Function


    Public Function Avvia_Export_Trapianti_Agribologna(ByVal Validita_Inizio_Pluriennali As String,
                                                       ByVal Validita_Fine_Pluriennali As String,
                                                       ByVal Validita_Inizio_Annuali As String,
                                                       ByVal Validita_Fine_Annuali As String) As RispostaStandard

        Dim r As New RispostaStandard

        r = Esporta_Trapianti_Agribologna_Sync(objParametri_Server, objParametri_Utenti, Validita_Inizio_Pluriennali, Validita_Fine_Pluriennali, Validita_Inizio_Annuali, Validita_Fine_Annuali)

        Return r

    End Function

    Public Function Avvia_Export_Lotti_Agribologna() As RispostaStandard

        Dim r As New RispostaStandard

        r = Esporta_Lotti_Agribologna_Sync(objParametri_Server, objParametri_Utenti)

        Return r

    End Function


    Public Function Avvia_Import_QDC_Agribologna(piva As String, ByVal bGSB As Boolean) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            r = Importa_QDC_Agribologna_Sync(objParametri_SuperServer, objParametri_Server, objParametri_Utenti, piva, bGSB)


        Catch ex As Exception
            r.RispostaStringa &= "Errore durante l'importazione: " & ex.Message
            r.Errore &= "Errore durante l'importazione: " & ex.Message
            r.RispostaOK = False
        End Try

        Return r

    End Function



    Public Shared Function Importazione_Tracciabilita_Agribologna_Sync(ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard

        Dim Err_Code As Integer = 0
        Dim Err_Des As String = ""
        Dim Err_Des_File As String = ""
        Dim Err_Des_MP As String = ""

        Dim Id_Agenda_Duplicato As Integer = 0
        Dim Id_Agenda_Check As Integer = 0
        Dim Id_Agenda_Output As Integer = 0
        Dim Id_Mov_Carico_Output As Integer = 0
        Dim Sa_Cod_Check As Integer = 0
        Dim Id_Mov_Check As Integer = 0
        Dim Id_Mov_Det_Check As Integer = 0
        Dim Mat_Cod_Trapianto As Integer = 0
        Dim Grado_Check As Integer = 0
        Dim DataOk As Boolean = False
        Dim bTrapianto As Boolean = False
        Dim Grado As Integer = 0
        Dim Mat_Cod_Trapianto_Check As Integer = 0
        Dim Des_Lib_Trapianto As String = ""
        Dim VEG_COD As Integer = 0
        Dim GRVA_COD_VEG As Integer = 0
        Dim CUL_COD As Integer = 0


        Dim Sa_Cod As Integer = 0
        Dim Appezza As Integer = 0
        Dim Id_Reg As Integer = 0

        Dim Sa_Cod_Carico As Integer = 0


        Dim ObjImport_R As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim ObjMaterie_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim ObjImpianti_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim ObjSpecie_R As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Dim ObjCultivar_R As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim ObjContatti As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
        Dim ObjIndirizzi As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
        Dim ObjCentriAziendali As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim ObjDoc As New AgronicaCoreContabDAL.Movimenti_R
        Dim ObjImportXml_R As New AgronicaCoreXMLUniversale.XML_Import_Log_R
        Dim ObjImportXml_W As New AgronicaCoreXMLUniversale.XML_Import_Log_W
        Dim objFile As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim objCore_XML_Anagrafe As New AgronicaCoreXML.XML_Anagrafe
        Dim objCore_MP_W As New AgronicaCoreAnagrafeBIZ.Materie_Prime_W


        Dim DT_Semine As New DataTable
        Dim DT_Traccia As New DataTable
        Dim DT_Prodotto As New DataTable
        Dim DT_Destinazioni As New DataTable
        Dim DT_Impianto As New DataTable
        Dim DT_Specie As New DataTable
        Dim DT_Cultivar As New DataTable
        Dim DT_Contatti As New DataTable
        Dim DT_Indirizzi As New DataTable
        Dim DT_CentriAziendali As New DataTable
        Dim DT_Doc As New DataTable
        Dim DT_ImportXml As New DataTable

        Dim r As New RispostaStandard

        Dim XmlDocBolle As New System.Xml.XmlDocument
        Dim XmlDom As New System.Xml.XmlDocument
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Dati As String
        Dim xDatiBolle As System.Xml.XmlNodeList
        Dim xDatiBolla As System.Xml.XmlElement
        Dim XmlBolle As System.Xml.XmlNodeList
        Dim XmlBolla As System.Xml.XmlElement

        Dim Str_XML_MP As String = ""
        Dim Flag_Insert As Boolean = False

        Dim NumeroBolleImportate As Integer = 0
        Dim NumeroBollePresenti As Integer = 0
        Dim NumeroBolleGiaImportate As Integer = 0
        Dim NumeroBolleSovrascritte As Integer = 0
        Dim NumeroBolleErrate As Integer = 0

        Dim XmlPM101241 As System.Xml.XmlNodeList
        Dim XmlPM101242 As System.Xml.XmlNodeList
        Dim XmlPM101243 As System.Xml.XmlNodeList
        Dim XmlPM101246 As System.Xml.XmlNodeList
        Dim XmlPM101247 As System.Xml.XmlNodeList
        Dim XmlPM101248 As System.Xml.XmlNodeList
        Dim XmlPM101249 As System.Xml.XmlNodeList
        Dim XmlPM101250 As System.Xml.XmlNodeList
        Dim XmlPM101600 As System.Xml.XmlNodeList
        Dim XmlDatiMateriePrime As System.Xml.XmlElement

        Dim BollaImportabile As Boolean = False
        Dim FiltroAggiuntvo As String = ""

        'Variabili Dati
        Dim LottoIngresso As String = ""
        Dim PivaSocio As String = ""
        Dim Rag_Soc As String = ""
        Dim strDataTrapianto As String
        Dim DataTrapianto As Date = CDate("01/01/1900")
        Dim strDataRaccolta As String
        Dim DataRaccolta As Date
        Dim strDataBolla As String
        Dim DataBolla As Date
        Dim strNumeroBolla As String
        Dim NumeroBolla As Double
        Dim CodiceArticolo As String = ""
        Dim Elem_Cod As Integer = TRASFORMATI_VEGETALI
        Dim Mat_Cod As Integer
        Dim Mat_Des As String
        Dim Qta As Double
        Dim Udm_Des As String = ""
        Dim Udm_Cod As Long
        Dim Veg_Des As String
        Dim Sup_Imp As Decimal
        Dim Tipo_Xml As String = "ImportConferimento"

        Dim i_DatiBolle As Integer = 0
        Dim i_Bolle As Integer = 0
        Dim i_Bolle2 As Integer = 0

        Dim Fase As String = ""

        Dim Cod_RisUm As Integer = 0
        Dim Cod_Indirizzo As Integer = 0
        Dim Outer_Xml As String = ""
        Dim bContatto As Boolean = False
        Dim Xml_Import_Key As String = ""
        Dim FileImportazione As String = ""
        Dim Peso_Totale As Decimal = 0


        Dim objContabHelper As New ContabilitaHelper_Testata(objParametri_Server, objParametri_Utenti)

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim Bolle(0 To 25, 0 To 0) As String

        'Raccolta Informazioni
        Dim COL_PIVASOCIO As Integer = 0
        Dim COL_SA_COD As Integer = 1
        Dim COL_APPEZZA As Integer = 2
        Dim COL_ID_REG As Integer = 3
        Dim COL_DATABOLLA As Integer = 4
        Dim COL_DATARACCOLTA As Integer = 5
        Dim COL_MAT_COD As Integer = 6
        Dim COL_LOTTOINGRESSO As Integer = 7
        Dim COL_UDM_COD As Integer = 8
        Dim COL_UDM_DES As Integer = 9
        Dim COL_QTA As Integer = 10
        Dim COL_CODICE_ARTICOLO As Integer = 11
        Dim COL_SUP_IMP As Integer = 12
        Dim COL_VALIDITA As Integer = 13
        Dim COL_VALIDITA_DES As Integer = 14
        Dim COL_COD_INDIRIZZO As Integer = 15
        Dim COL_VEG_COD As Integer = 16
        Dim COL_VEG_DES As Integer = 17
        Dim COL_COD_RISUM As Integer = 18
        Dim COL_NUMEROBOLLA As Integer = 19
        Dim COL_MAT_DES As Integer = 20
        Dim COL_RAG_SOC As Integer = 21
        Dim COL_XML As Integer = 22

        Try

            'Recupero FIle Importazione
            FileImportazione = objFile.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_FILE_IMPORTAZIONE_DATI, objParametri_Utenti, 2)


            If String.IsNullOrEmpty(FileImportazione) Then

                Err_Code = 100
                Err_Des_File = Err_Des_File & "Il File di importazione non è impostato." & Chr(13)

            Else

                '##################################################################################################################################
                '########################################## DECODIFICA ############################################################################
                '##################################################################################################################################


                'Lettura file Xml
                XmlDocBolle.Load(Trim(FileImportazione))
                Dati = XmlDocBolle.OuterXml

                'Controllo Correttezza Xml
                '=====================================================================================
                If Trim(Dati) = "" Then

                    Err_Code = 101
                    Err_Des_File = Err_Des_File & "Il File di importazione non è valido." & Chr(13)

                Else

                    Fase = "Decodifica"

                    'Impostazione Sa_Cod Carico
                    DT_CentriAziendali = ObjCentriAziendali.Leggi(Piva_SuperUser, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "Sa_Cod Asc", objParametri_Server)

                    Sa_Cod_Carico = DT_CentriAziendali(0)("Sa_Cod")

                    ' Magazzino Carico
                    Dim MagazzinoCarico As New Contabilita_Magazzino

                    If Not LeggiContabilitaMagazzino(Piva_SuperUser, Sa_Cod_Carico, MagazzinoCarico, objParametri_Server) Then

                        Err_Code = 11
                        Err_Des = Err_Des & "Non è possibile identificare il magazzino di carico." & Chr(13)


                    Else


                        XmlDom.LoadXml(Dati)

                        xDatiBolle = XmlDom.GetElementsByTagName("dataroot")

                        i_DatiBolle = 0


                        'Check di esistenza Informazioni
                        Do While i_DatiBolle < xDatiBolle.Count

                            xDatiBolla = xDatiBolle.Item(i_DatiBolle)

                            XmlBolle = xDatiBolla.GetElementsByTagName("P100147M100060")

                            i_Bolle = 0

                            ReDim Bolle(0 To 25, 0 To XmlBolle.Count)

                            NumeroBollePresenti = XmlBolle.Count

                            'Check di esistenza Informazioni
                            Do While i_Bolle < XmlBolle.Count

                                'Reset Variabili
                                'LottoIngresso = ""
                                Err_Code = 0
                                Err_Des = ""
                                LottoIngresso = ""

                                XmlBolla = XmlBolle.Item(i_Bolle)

                                'Lotto Ingresso
                                XmlPM101241 = XmlBolla.GetElementsByTagName("PM101241")

                                If XmlPM101241.Count = 1 Then

                                    LottoIngresso = Trim(XmlPM101241.Item(0).ChildNodes.Item(0).OuterXml)

                                    'Scomposizione LottoIngresso
                                    PivaSocio = Left(LottoIngresso, 11)
                                    strDataTrapianto = Mid(LottoIngresso, 12, 8)
                                    If UtilityProvider.Agro_SQL_SaveNum(strDataTrapianto) = 0 Then
                                        Err_Code = 345
                                        Err_Des = Err_Des & "La data di trapianto non è stata impostata." & Chr(13)
                                    Else
                                        DataTrapianto = FormConvertToData(strDataTrapianto)
                                        'Controllo Coerenza Data
                                        If DataTrapianto = AGRODATAINIZIO Then
                                            Err_Code = 347
                                            Err_Des = Err_Des & "La data di trapianto non è corretta." & Chr(13)
                                        End If

                                    End If


                                    '##################################################################################
                                    '#############     RICERCA FORNITORE IN ANAGRAFICA CONTATTI  ######################
                                    '##################################################################################

                                    bContatto = False

                                    DT_Contatti = ObjContatti.Leggi(Piva_SuperUser, PivaSocio, 0, enum_Rapporti_Contabili_Standard.Conferente, 0, "", True, True, "", "", objParametri_Server)

                                    If DT_Contatti.Rows.Count > 0 Then

                                        'Ok                            
                                        Cod_RisUm = DT_Contatti(0)("Cod_RisUm")

                                        '=================================================================================================================
                                        'Lettura dell'indirizzo del contatto (sede operativa)
                                        DT_Indirizzi = ObjIndirizzi.Leggi(PivaSocio, 0, 0, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                                        If DT_Indirizzi.Rows.Count = 0 Then

                                            'Indirizzo Contatto Inesistente
                                            Err_Code = 8
                                            Err_Des = Err_Des & "Il Contatto non ha un indirizzo valido." & Chr(13)

                                        Else
                                            'Ok
                                            Cod_Indirizzo = DT_Indirizzi(0)("Cod_Indirizzo")
                                            bContatto = True

                                        End If
                                        '=================================================================================================================

                                    Else

                                        'Contatto Inesistente
                                        Err_Code = 9
                                        Err_Des = Err_Des & "Il Contatto non esiste in archivio." & Chr(13)


                                    End If


                                Else
                                    Err_Code = 1
                                    Err_Des = Err_Des & "Il 'Lotto di Ingresso' non può essere analizzato." & Chr(13)
                                End If


                                'Data Raccolta
                                XmlPM101600 = XmlBolla.GetElementsByTagName("PM101600")
                                If XmlPM101600.Count = 1 Then
                                    strDataRaccolta = XmlPM101600.Item(0).ChildNodes.Item(0).OuterXml
                                    DataRaccolta = CDate(strDataRaccolta)
                                    'DataRaccolta = Format(strDataRaccolta, "dd/mm/yyyy")

                                Else
                                    Err_Code = 346
                                    Err_Des = Err_Des & "La data di raccolta non è stata impostata." & Chr(13)
                                End If


                                'Ragione Sociale Socio
                                XmlPM101246 = XmlBolla.GetElementsByTagName("PM101246")
                                If XmlPM101246.Count = 1 Then
                                    Rag_Soc = XmlPM101246.Item(0).ChildNodes.Item(0).OuterXml
                                Else
                                    Err_Code = 15
                                    Err_Des = Err_Des & "La ragione sociale del socio conferente non può essere analizzata." & Chr(13)
                                End If

                                'Data Bolla
                                XmlPM101242 = XmlBolla.GetElementsByTagName("PM101242")
                                If XmlPM101242.Count = 1 Then
                                    strDataBolla = XmlPM101242.Item(0).ChildNodes.Item(0).OuterXml
                                    DataBolla = CDate(strDataBolla)
                                Else
                                    Err_Code = 2
                                    Err_Des = Err_Des & "La data della 'Bolla di Ingresso' non è valida." & Chr(13)
                                End If


                                'Numero Bolla
                                XmlPM101243 = XmlBolla.GetElementsByTagName("PM101243")
                                If XmlPM101243.Count = 1 Then

                                    If XmlPM101243.Item(0).ChildNodes.Count = 1 Then

                                        'A volte inseriscono le bolle con numero alfanumerico!!
                                        'Esempio Z89898 --> lo trasformo in 89898
                                        strNumeroBolla = Replace(XmlPM101243.Item(0).ChildNodes.Item(0).OuterXml, "Z", "")

                                        If IsNumeric(strNumeroBolla) Then
                                            NumeroBolla = strNumeroBolla

                                        Else
                                            Err_Code = 3
                                            Err_Des = Err_Des & "Il numero della 'Bolla di Ingresso' non è numerico. Gli archivi GiasLan non gestiscono bolle con numero alfanumerico. " & Chr(13)
                                        End If

                                    Else
                                        Err_Code = 3
                                        Err_Des = Err_Des & "Il numero della 'Bolla di Ingresso' non è stato impostato." & Chr(13)

                                        'ERRORE IRREVERSIBILE

                                    End If


                                Else
                                    Err_Code = 3
                                    Err_Des = Err_Des & "Il numero della 'Bolla di Ingresso' non è valida." & Chr(13)
                                End If


                                'Descrizione Articolo
                                XmlPM101248 = XmlBolla.GetElementsByTagName("PM101248")
                                If XmlPM101248.Count = 1 Then
                                    Mat_Des = XmlPM101248.Item(0).ChildNodes.Item(0).OuterXml
                                Else
                                    Mat_Des = "Indefinito"
                                End If


                                'Codice Articolo
                                XmlPM101247 = XmlBolla.GetElementsByTagName("PM101247")


                                'Reset
                                Mat_Cod = 0
                                VEG_COD = 0
                                GRVA_COD_VEG = 0
                                CUL_COD = 0
                                Veg_Des = ""


                                If XmlPM101247.Count = 1 Then

                                    'Prodotto Presente in Archivio
                                    CodiceArticolo = Trim(XmlPM101247.Item(0).ChildNodes.Item(0).OuterXml)

                                    'Lettura del Prodotto dal Codice Articolo
                                    DT_Prodotto = ObjMaterie_R.Leggi2("", Elem_Cod, 0, CodiceArticolo, 0, "", True, enumSelezioneVariabile.Selezione_TabellaCompleta, "Elem_Cod in (201, 210)", "", objParametri_Server)

                                    If DT_Prodotto.Rows.Count > 0 Then

                                        Mat_Cod = DT_Prodotto(0)("Mat_Cod")
                                        VEG_COD = UtilityProvider.Agro_SQL_SaveNum(DT_Prodotto(0)("Veg_Cod"))
                                        GRVA_COD_VEG = UtilityProvider.Agro_SQL_SaveNum(DT_Prodotto(0)("Grva_Cod_Veg"))
                                        CUL_COD = UtilityProvider.Agro_SQL_SaveNum(DT_Prodotto(0)("Cul_Cod"))

                                    Else


                                        '##################################################################################
                                        '#########     INSERIMENTO PRODOTTO IN ANAGRAFICA MATERIE   ######################
                                        '##################################################################################

                                        VEG_COD = UtilityProvider.Agro_SQL_SaveNum(Mid(LottoIngresso, 20, 3))

                                        If Len(LottoIngresso) >= 26 Then
                                            GRVA_COD_VEG = UtilityProvider.Agro_SQL_SaveNum(Mid(LottoIngresso, 23, 3))
                                        Else
                                            GRVA_COD_VEG = 0
                                        End If

                                        If Len(LottoIngresso) >= 31 Then
                                            CUL_COD = UtilityProvider.Agro_SQL_SaveNum(Mid(LottoIngresso, 26, 6))
                                        Else
                                            CUL_COD = 0
                                        End If

                                        XmlDoc = Nothing

                                        XmlDatiMateriePrime = objCore_XML_Anagrafe.XML_MateriePrime(
                                                                Err_Des_MP,
                                                                XmlDoc,
                                                                0,
                                                                2000000000,
                                                                enum_TipoOperazioneDB.Scrittura,
                                                                enum_TipoOperazioneDB.Scrittura,
                                                                enum_TipoOperazioneDB.Scrittura,
                                                                enum_TipoOperazioneDB.Scrittura,
                                                                enum_TipoOperazioneDB.Scrittura,
                                                                enum_TipoOperazioneDB.Scrittura,
                                                                objParametri_Server.PivaSuperUser,
                                                                objParametri_Server.PivaSuperUser,
                                                                TRASFORMATI_VEGETALI,
                                                                0,
                                                                CodiceArticolo,
                                                                Mat_Des,
                                                                Nothing,
                                                                Nothing,
                                                                Nothing,
                                                                Nothing,
                                                                Nothing,
                                                                -1,
                                                                0,
                                                                VEG_COD,
                                                                CUL_COD,
                                                                ,
                                                                GRVA_COD_VEG, , , , , , , , , , , ,
                                                                1,
                                                                ,
                                                                0,
                                                                , , , , , , , , , , , , ,
                                                                "",
                                                                , , , , , , )


                                        If Err_Des_MP <> "" Then

                                            'Prodotto Inesistente
                                            Err_Code = 7
                                            Err_Des = Err_Des & "Il Prodotto " & Mat_Des & " non puà essere creato." & Chr(13)

                                        Else

                                            Str_XML_MP = XmlDoc.OuterXml

                                            Flag_Insert = objCore_MP_W.Materia_Prima_Scrivi(Str_XML_MP,
                                                                                        Mat_Cod,
                                                                                        AGRODATAINIZIO,
                                                                                        AGRODATAFINE,
                                                                                        objParametri_Server)

                                            If Not Flag_Insert Then

                                                'Prodotto Inesistente
                                                Err_Code = 7
                                                Err_Des = Err_Des & "Il Prodotto " & Mat_Des & " non puà essere creato." & Chr(13)

                                            End If

                                        End If

                                    End If

                                    'Lettura Veg_Des
                                    DT_Specie = ObjSpecie_R.Leggi(CLng(VEG_COD),
                                                              0,
                                                              "",
                                                              "",
                                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                              "",
                                                              "",
                                                             objParametri_Server)

                                    If DT_Specie.Rows.Count > 0 Then

                                        Veg_Des = DT_Specie(0)("Veg_Des")

                                    End If

                                Else

                                    Err_Code = 4
                                    Err_Des = Err_Des & "Il 'Codice Articolo' non può essere analizzato." & Chr(13)
                                End If

                                'Quantità
                                XmlPM101249 = XmlBolla.GetElementsByTagName("PM101249")
                                If XmlPM101249.Count = 1 Then
                                    Qta = XmlPM101249.Item(0).ChildNodes.Item(0).OuterXml
                                Else
                                    Err_Code = 5
                                    Err_Des = Err_Des & "La quantità della bolla di ingresso non può essere analizzata." & Chr(13)
                                End If

                                'Unità Misura
                                XmlPM101250 = XmlBolla.GetElementsByTagName("PM101250")
                                If XmlPM101250.Count = 1 Then
                                    Udm_Des = XmlPM101250.Item(0).ChildNodes.Item(0).OuterXml

                                    'Decodifica Udm_Des
                                    Select Case UCase(Udm_Des)

                                        Case "KG" : Udm_Cod = 2

                                        Case Else
                                            Err_Code = 6
                                            Err_Des = Err_Des & "L'unità di misura della bolla di ingresso non può essere analizzata." & Chr(13)

                                    End Select

                                Else
                                    Err_Code = 6
                                    Err_Des = Err_Des & "L'unità di misura della bolla di ingresso non può essere analizzata." & Chr(13)
                                End If


                                If Err_Code = 0 Then

                                    'Lettura del Trapianto
                                    Sa_Cod_Check = 0
                                    Id_Agenda_Check = 0
                                    Id_Mov_Check = 0
                                    Id_Mov_Det_Check = 0I

                                    DT_Semine = ObjImport_R.Leggi_Import_Agribologna(PivaSocio, CAU_LAVORAZIONE, DataTrapianto, DataTrapianto, enumSelezioneVariabile.Selezione_JoinCompleta, "Lav_Cod In (2, 71)", "", objParametri_Server)

                                    If DT_Semine.Rows.Count > 0 Then

                                        Mat_Cod_Trapianto = 0
                                        Grado_Check = 0
                                        DataOk = False
                                        bTrapianto = False

                                        'Cerco il Trapianto
                                        For Each dr_semine As DataRow In DT_Semine.Rows


                                            If CDate(dr_semine("Data_Movimento")) = DataTrapianto Then

                                                DataOk = True


                                                DT_Traccia = ObjImport_R.Leggi_Semine_Trapianti_Import_Agribologna(PivaSocio,
                                                                                                   CLng(dr_semine("Sa_Cod")),
                                                                                                   CLng(dr_semine("Id_Agenda")),
                                                                                                   10,
                                                                                                   CAU_LAVORAZIONE,
                                                                                                   enumSelezioneVariabile.Selezione_JoinCompleta,
                                                                                                   "Lav_Cod In (2, 71)",
                                                                                                   "",
                                                                                                   objParametri_Server)



                                                If DT_Traccia.Rows.Count > 0 Then


                                                    For Each dr_traccia As DataRow In DT_Traccia.Rows

                                                        '==================================================================================================
                                                        'Determinazione Trapianto
                                                        '--------------------------------------------------------------------------------------------------

                                                        Grado = 0

                                                        'Caso in cui si è trapiantato un pro_cod e non esistono in archivio mat_cod del veg_cod impostato!
                                                        If UtilityProvider.Agro_SQL_SaveNum(dr_traccia("Pro_Cod")) <> 0 And dr_traccia("Mat_Cod") = 0 Then

                                                            Grado = 1

                                                        Else

                                                            Mat_Cod_Trapianto = dr_traccia("Mat_Cod")

                                                            'Lettura del prodotto trapianto
                                                            DT_Prodotto = ObjMaterie_R.Leggi2(Piva_SuperUser, 10, Mat_Cod_Trapianto, "", 0, "", True, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                                                            If DT_Prodotto.Rows.Count > 0 Then

                                                                'Verifica Specie Vegetale
                                                                If VEG_COD = UtilityProvider.Agro_SQL_SaveNum(DT_Prodotto(0)("Veg_Cod")) Then
                                                                    'Specie Ok
                                                                    Grado = 2
                                                                    'Verifica Gruppo Varietale
                                                                    If GRVA_COD_VEG = UtilityProvider.Agro_SQL_SaveNum(DT_Prodotto(0)("Grva_Cod_Veg")) Then
                                                                        'Gruppo Varietale Ok
                                                                        Grado = 3
                                                                        'Verifica Cultivar
                                                                        If CUL_COD = UtilityProvider.Agro_SQL_SaveNum(DT_Prodotto(0)("Cul_Cod")) Then
                                                                            'Ok
                                                                            Grado = 4

                                                                        End If

                                                                    End If

                                                                End If

                                                            End If

                                                        End If

                                                        'Aggiornamento
                                                        If Grado_Check < Grado OrElse Not bTrapianto Then

                                                            Mat_Cod_Trapianto_Check = Mat_Cod_Trapianto
                                                            Grado_Check = Grado
                                                            Sa_Cod_Check = dr_traccia("Sa_Cod")
                                                            Id_Agenda_Check = dr_traccia("Id_Agenda")
                                                            Id_Mov_Check = dr_traccia("Id_Mov")
                                                            Id_Mov_Det_Check = dr_traccia("Id_Mov_Det")
                                                            Des_Lib_Trapianto = dr_traccia("Des_Lib")

                                                            'Leggo i dati dell'impianto
                                                            DT_Destinazioni = ObjImport_R.Leggi_Destinazioni_Import_Agribologna(PivaSocio,
                                                                                                                Sa_Cod_Check,
                                                                                                                Id_Agenda_Check,
                                                                                                                Id_Mov_Check,
                                                                                                                CDate(DataTrapianto),
                                                                                                                CDate(DataTrapianto),
                                                                                                                enumSelezioneVariabile.Selezione_JoinCompleta,
                                                                                                                "",
                                                                                                                "",
                                                                                                                objParametri_Server)


                                                            If DT_Destinazioni.Rows.Count > 0 Then

                                                                For Each dr_destinazioni As DataRow In DT_Destinazioni.Rows

                                                                    If Not bTrapianto Then


                                                                        'Lettura dell'Impianto
                                                                        Sa_Cod = dr_destinazioni("Sa_Cod")
                                                                        Appezza = dr_destinazioni("Appezza")
                                                                        Id_Reg = dr_destinazioni("Id_Destinazione")

                                                                        'Lettura della specie vegetale dell'impianto
                                                                        DT_Impianto = ObjImpianti_R.Leggi(CStr(PivaSocio),
                                                                                          Sa_Cod,
                                                                                          Appezza,
                                                                                          Id_Reg,
                                                                                           enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                           "",
                                                                                           "",
                                                                                           objParametri_Server)


                                                                        If DT_Impianto.Rows.Count <> 0 Then

                                                                            Sup_Imp = DT_Impianto(0)("Sup_Imp")


                                                                            'Lettura del Veg_Cod
                                                                            DT_Cultivar = ObjCultivar_R.Leggi(DT_Impianto(0)("Cul_Cod"),
                                                                                              CLng(VEG_COD),
                                                                                              "",
                                                                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                              "",
                                                                                              "",
                                                                                             objParametri_Server)

                                                                            If DT_Cultivar.Rows.Count <> 0 Then

                                                                                'Dati Corrispondenti
                                                                                bTrapianto = True

                                                                            End If

                                                                        End If


                                                                    End If


                                                                Next


                                                            End If

                                                        End If

                                                    Next

                                                End If

                                            End If

                                        Next

                                    End If


                                    If Not bTrapianto OrElse Not DataOk Then

                                        Err_Code = 10

                                        Select Case DataOk

                                            Case True : Err_Des = Err_Des & "La data di trapianto " & DataTrapianto & " non corrisponde alla specie " & Veg_Des & "."

                                            Case False : Err_Des = Err_Des & "Non Esistono su Gias operazioni di trapianto in data " & DataTrapianto & "."

                                        End Select

                                    End If



                                    'If bTrapianto And DataOk And bContatto Then


                                    'End If

                                End If

                                'Impostazione Chiave
                                Bolle(COL_COD_RISUM, i_Bolle) = Cod_RisUm
                                Bolle(COL_NUMEROBOLLA, i_Bolle) = NumeroBolla
                                Bolle(COL_VALIDITA, i_Bolle) = Err_Code
                                Bolle(COL_VALIDITA_DES, i_Bolle) = Err_Des


                                Select Case Err_Code

                                    Case 0

                                        'Aggiunta al riferimento

                                        Bolle(COL_PIVASOCIO, i_Bolle) = PivaSocio
                                        Bolle(COL_SA_COD, i_Bolle) = Sa_Cod
                                        Bolle(COL_APPEZZA, i_Bolle) = Appezza
                                        Bolle(COL_ID_REG, i_Bolle) = Id_Reg
                                        Bolle(COL_SUP_IMP, i_Bolle) = Sup_Imp
                                        Bolle(COL_DATABOLLA, i_Bolle) = DataRaccolta
                                        Bolle(COL_DATARACCOLTA, i_Bolle) = DataRaccolta
                                        Bolle(COL_MAT_COD, i_Bolle) = Mat_Cod
                                        Bolle(COL_MAT_DES, i_Bolle) = Mat_Des
                                        Bolle(COL_LOTTOINGRESSO, i_Bolle) = LottoIngresso
                                        Bolle(COL_UDM_COD, i_Bolle) = Udm_Cod
                                        Bolle(COL_UDM_DES, i_Bolle) = Udm_Des
                                        Bolle(COL_QTA, i_Bolle) = Qta
                                        Bolle(COL_CODICE_ARTICOLO, i_Bolle) = CodiceArticolo
                                        Bolle(COL_COD_INDIRIZZO, i_Bolle) = Cod_Indirizzo
                                        Bolle(COL_VEG_COD, i_Bolle) = VEG_COD
                                        Bolle(COL_VEG_DES, i_Bolle) = Veg_Des
                                        Bolle(COL_RAG_SOC, i_Bolle) = Rag_Soc
                                        Bolle(COL_XML, i_Bolle) = XmlBolla.OuterXml


                                    Case Else

                                        Err_Des_File = Err_Des_File & " Lotto Ingresso: " & LottoIngresso & " Socio " & Rag_Soc & " Errore No. " & Err_Code & " " & Err_Des & Chr(13)
                                        NumeroBolleErrate += 1


                                End Select


                                i_Bolle += 1

                            Loop

                            i_DatiBolle += 1

                        Loop

                    End If


                    '##################################################################################
                    '#####################     CONTROLLO VALIDITA BOLLA   #############################
                    '##################################################################################                    

                    For i_Bolle = 0 To UBound(Bolle, 2) - 1

                        Cod_RisUm = Bolle(COL_COD_RISUM, i_Bolle)
                        NumeroBolla = Bolle(COL_NUMEROBOLLA, i_Bolle)

                        BollaImportabile = True

                        For i_Bolle2 = 0 To UBound(Bolle, 2) - 1

                            If Cod_RisUm = Bolle(COL_COD_RISUM, i_Bolle2) AndAlso
                               NumeroBolla = Bolle(COL_NUMEROBOLLA, i_Bolle2) Then

                                If CInt(Bolle(COL_VALIDITA, i_Bolle)) <> 0 Then

                                    'Almeno una riga non valida --> tutto il documento è non valido
                                    BollaImportabile = False
                                    Exit For

                                End If

                            End If

                        Next

                        If Not BollaImportabile Then

                            'Marco tutte le righe della bolla come non processabili
                            For i_Bolle2 = 0 To UBound(Bolle, 2) - 1

                                If Cod_RisUm = Bolle(COL_COD_RISUM, i_Bolle2) AndAlso
                                   NumeroBolla = Bolle(COL_NUMEROBOLLA, i_Bolle2) Then

                                    Bolle(COL_VALIDITA, i_Bolle2) = -1 'Non valida

                                End If

                            Next

                        End If

                    Next


                    '##################################################################################
                    '##########################    SCORRIMENTO RIGHE      #############################
                    '##################################################################################

                    For i_Bolle = 0 To UBound(Bolle, 2) - 1

                        If IsNumeric(Bolle(COL_VALIDITA, i_Bolle)) Then

                            If Bolle(COL_VALIDITA, i_Bolle) = 0 Then

                                Cod_RisUm = Bolle(COL_COD_RISUM, i_Bolle)
                                NumeroBolla = Bolle(COL_NUMEROBOLLA, i_Bolle)
                                DataRaccolta = Bolle(COL_DATARACCOLTA, i_Bolle)
                                Rag_Soc = Bolle(COL_RAG_SOC, i_Bolle)
                                Cod_RisUm = Bolle(COL_COD_RISUM, i_Bolle)
                                Cod_Indirizzo = Bolle(COL_COD_INDIRIZZO, i_Bolle)
                                Outer_Xml = Bolle(COL_XML, i_Bolle)

                                PivaSocio = Bolle(COL_PIVASOCIO, i_Bolle)
                                Sa_Cod = Bolle(COL_SA_COD, i_Bolle)
                                Appezza = Bolle(COL_APPEZZA, i_Bolle)
                                Id_Reg = Bolle(COL_ID_REG, i_Bolle)
                                Sup_Imp = Bolle(COL_SUP_IMP, i_Bolle)
                                Mat_Cod = Bolle(COL_MAT_COD, i_Bolle)
                                Mat_Des = Bolle(COL_MAT_DES, i_Bolle)
                                LottoIngresso = Bolle(COL_LOTTOINGRESSO, i_Bolle)

                                Udm_Cod = Bolle(COL_UDM_COD, i_Bolle)
                                Udm_Des = Bolle(COL_UDM_DES, i_Bolle)
                                Qta = Bolle(COL_QTA, i_Bolle)
                                CodiceArticolo = Bolle(COL_CODICE_ARTICOLO, i_Bolle)
                                Cod_Indirizzo = Bolle(COL_COD_INDIRIZZO, i_Bolle)

                                VEG_COD = Bolle(COL_VEG_COD, i_Bolle)
                                Veg_Des = Bolle(COL_VEG_DES, i_Bolle)
                                Rag_Soc = Bolle(COL_RAG_SOC, i_Bolle)

                                '========================================================================================================================
                                'Impostazione XML_Import_Key
                                '------------------------------------------------------------------------------------------------------------------------
                                Outer_Xml = LottoIngresso & "-" & CInt(Bolle(COL_QTA, i_Bolle)) & " "
                                Xml_Import_Key = Outer_Xml
                                Peso_Totale = Qta

                                For i_Bolle2 = i_Bolle + 1 To UBound(Bolle, 2) - 1

                                    If Cod_RisUm = Bolle(COL_COD_RISUM, i_Bolle2) AndAlso
                                   NumeroBolla = Bolle(COL_NUMEROBOLLA, i_Bolle2) Then

                                        Xml_Import_Key = Xml_Import_Key & Bolle(COL_LOTTOINGRESSO, i_Bolle2) & "-" & CInt(Bolle(COL_QTA, i_Bolle2)) & " "
                                        Peso_Totale = Peso_Totale + Bolle(COL_QTA, i_Bolle2)

                                    End If

                                Next
                                '========================================================================================================================


                                'Reset Errori
                                Err_Code = 0
                                Err_Des = ""

                                '========================================================================================================================
                                'Controllo Bolla già importata
                                '------------------------------------------------------------------------------------------------------------------------
                                BollaImportabile = True

                                Fase = "Verifica Importazione Precedente"

                                FiltroAggiuntvo = "Lav_Cod In ( " & LAVCOD_ACCETTAZIONE_DIVERSI & ", " & LAVCOD_CONFERIMENTO & ") And " &
                                " Cau_Mov in ('" & CAU_REGISTRAZIONE_SECONDARIA & "', '" & CAU_REGISTRAZIONI & "') And Doc_Numero = " & NumeroBolla &
                                " And Cod_Risum In (Select Cod_Risum From Risorse_Umane Where Cod_Contatto = '" & PivaSocio & "')" &
                                " And Agenda.Id_Agenda In (Select Id_Agenda From Movimenti_Dettagli Where Elem_Cod In (201,210))"

                                DT_Doc = ObjDoc.Leggi(Piva_SuperUser, 0, 0, 0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, FiltroAggiuntvo, "", objParametri_Server)
                                'DT_Doc = ObjDoc.Leggi(Piva_SuperUser, 0, 0, 0, Cod_RisUm, "", enumSelezioneVariabile.Selezione_TabellaCompleta, FiltroAggiuntvo, "", objParametri_Server)
                                'DT_Doc = ObjDoc.Leggi(Piva_SuperUser, 0, 0, 0, Cod_RisUm, CAU_REGISTRAZIONE_SECONDARIA, enumSelezioneVariabile.Selezione_TabellaCompleta, FiltroAggiuntvo, "", objParametri_Server)

                                If DT_Doc.Rows.Count > 0 Then

                                    'Controllo Xml Duplicato
                                    Id_Agenda_Duplicato = DT_Doc(0)("Id_Agenda")

                                    FiltroAggiuntvo = "Id_Agenda = " & Id_Agenda_Duplicato

                                    DT_ImportXml = ObjImportXml_R.Leggi("", "", 0, LAVCOD_ACCETTAZIONE_DIVERSI, Tipo_Xml, FiltroAggiuntvo, "", objParametri_Server)

                                    If DT_ImportXml.Rows.Count > 0 Then

                                        'Controllo Xml
                                        If UCase(Trim(Xml_Import_Key)) <> UCase(DT_ImportXml(0)("messaggio")) Then

                                            'Cancellazione Xml Import
                                            BollaImportabile = ObjImportXml_W.Cancella(objParametri_Server, Piva_SuperUser, "", Id_Agenda_Duplicato, LAVCOD_ACCETTAZIONE_DIVERSI, Tipo_Xml, "")

                                            NumeroBolleSovrascritte += 1

                                        Else

                                            'Bolla già importata con medesimo xml
                                            BollaImportabile = False
                                            NumeroBolleGiaImportate += 1

                                        End If

                                    Else

                                        'Record Inesistente --> Bolla giù importata (magari dal lan)
                                        BollaImportabile = False
                                        NumeroBolleGiaImportate += 1


                                    End If

                                    If BollaImportabile Then

                                        'Cancellazione DDT 
                                        Dim rDDT As New RispostaStandard


                                        '##################################################################################
                                        '##########################    CORREGGERE QUI   ###################################
                                        '##################################################################################

                                        Fase = "Cancellazione"
                                        rDDT = VerificaEliminaModificaDocumento(Piva_SuperUser, Id_Agenda_Duplicato, 0, 0, 0, enum_TipoOperazioneDB.Cancellazione, True, 2, False, objParametri_Server)

                                        'rDDT = VerificaEliminaModificaDocumento(Piva_SuperUser, Id_Agenda_Duplicato, 0, 0, 0, enum_TipoOperazioneDB.Cancellazione, True, 2, False)

                                        BollaImportabile = True

                                        If Not rDDT.RispostaOK Then

                                            Err_Code = 102
                                            Err_Des = Err_Des & "Non è possibile cancellare il DDT duplicato numero " & NumeroBolla & "." & Chr(13)

                                            BollaImportabile = False

                                        End If

                                    End If


                                End If


                                If BollaImportabile Then

                                    '##################################################################################
                                    '##########################    CREAZIONE TESTATA      #############################
                                    '##################################################################################

                                    Fase = "Creazione Testata"

                                    '========================================================================================================
                                    'Testata
                                    '--------------------------------------------------------------------------------------------------------
                                    Dim objContabTestata As New Contabilita_Testata With {
                                .Piva = Piva_SuperUser,
                                .SaCod = 0,
                                .IdAgenda = 0,
                                .IdMov = 0,
                                .StatoExport2 = 0,
                                .LavCod = LAVCOD_ACCETTAZIONE_DIVERSI,
                                .DescrizioneAgenda = "",
                                .TipoAccettazione = 2,
                                .ModuloGias = enum_Omni_Modulo_Generazione.FreshFood,
                                .DocNumeroSin = "",
                                .DocNumero = NumeroBolla,
                                .DocNumeroDes = "",
                                .DocNumeroVisualizzato = "",
                                .DocNumeroLock = True,
                                .DocNumeroLunghezza = 0,
                                .DocNumeroCarattereFormattazione = "",
                                .DataMovimento = DataRaccolta,
                                .DataRegistrazione = DataRaccolta,
                                .ProgrRegistrazione = 1,
                                .ProgrProtocollo = 0,
                                .SezionaleCod = 0,
                                .RagSoc = Rag_Soc,
                                .CodRisUm = Cod_RisUm,
                                .CodIndirizzoRisUm = Cod_Indirizzo,
                                .CodDestinazione = 0,
                                .CodIndirizzoDestinazione = 0,
                                .CodRisUmAltro = 0,
                                .CodRisUmAggiuntivo = 0,
                                .CodIndirizzoAggiuntivo = 0,
                                .SecondaCooperativa = 0,
                                .NoteIntestazione = "",
                                .Mezzo = 0,
                                .CodVettore = 0,
                                .CodIndirizzoVettore = 0,
                                .MacCodTrasporto = 0,
                                .Targa = "INDEFINITA",
                                .NumImmatricolazioneRimorchio = "INDEFINITO",
                                .DescrizioneMezzo = "",
                                .Aspetto = "VISIBILE",
                                .CausaleTrasportoCod = 8,
                                .CausaleTrasporto = "CONTO CONFERIMENTO",
                                .TipoPeso = 0,
                                .Colli = 0,
                                .OraSpedizione = DataRaccolta,
                                .NaturaBeni = "",
                                .ImballiVuoti = 0,
                                .TaraImballi = 0,
                                .PesoTaraTrasporto = 0,
                                .Peso = Peso_Totale,
                                .TaraVeicolo = 0,
                                .PesoNettoProd = Qta,
                                .TotaleDocumento = 0,
                                .DocumentoAccettazione = New Contabilita_Accettazione With {
                                    .IdMov = 0,
                                    .DocNumeroSin = "",
                                    .DocNumero = NumeroBolla,
                                    .DocNumeroDes = "",
                                    .DataMovimento = DataRaccolta
                                }
                            }



                                    '##################################################################################
                                    '###################    CREAZIONE DETTAGLIO DEL DDT    ############################
                                    '##################################################################################


                                    '================================================================================================
                                    'Impianto
                                    '------------------------------------------------------------------------------------------------
                                    Dim impianti As New List(Of Contabilita_Impianti_Raccolta)

                                    Dim objImpianto As New Contabilita_Impianti_Raccolta With {
                                        .Piva = PivaSocio,
                                        .Sa_Cod = Sa_Cod,
                                        .Appezza = Appezza,
                                        .Id_Reg = Id_Reg,
                                        .Veg_Des = Veg_Des,
                                        .Qta = Qta,
                                        .Sup_Imp = Sup_Imp
                                    }


                                    impianti.Add(objImpianto)

                                    '========================================================================================================

                                    '========================================================================================================
                                    'Campionatura
                                    '--------------------------------------------------------------------------------------------------------
                                    Dim Campionatura As New Dictionary(Of String, String)
                                    Dim MateriePrimeCampionature = LeggiMateriePrimeCampionatureAgribologna(Campionatura, Cod_RisUm)
                                    '========================================================================================================

                                    '========================================================================================================
                                    'Dettaglio
                                    '--------------------------------------------------------------------------------------------------------
                                    Dim objContabDettaglio As New Contabilita_Riga With {
                                .Piva = Piva_SuperUser,
                                .SaCod = Sa_Cod_Carico,
                                .IdAgenda = Id_Agenda_Output,
                                .IdMov = Id_Mov_Carico_Output,
                                .IdMovDet = 0,
                                .ModuloGias = enum_Omni_Modulo_Generazione.FreshFood,
                                .ElemCod = TRASFORMATI_VEGETALI,
                                .ProCod = 0,
                                .MatCod = Mat_Cod,
                                .MatDes = Mat_Des,
                                .Note = "",
                                .DestinazioneCarico = MagazzinoCarico,
                                .DestinazioneScarico = MagazzinoCarico,
                                .CalCod = 0,
                                .CodIva = -1,
                                .Pendente = 4,
                                .jolly_int = 0,
                                .ScontoModalita = 0,
                                .UdM = enum_UnitaMisura.KG,
                                .PrezzoRiferitoA = enum_PrezzoLivello.Kg_Litri,
                                .ValoreRiferimentoPrezzo = 0,
                                .NumConfezioni = 0,
                                .NumContenitori = 0,
                                .NumImballi = 0,
                                .Tara = 0,
                                .Lotto = LottoIngresso,
                                .KgLordi = Qta,
                                .KgNetti = Qta,
                                .Degrado = 0,
                                .Prezzo = 0,
                                .PrezzoEffettivoKgL = 0,
                                .PrezzoNetto = 0,
                                .ImportoUnitario = 0,
                                .ImponibileTotale = 0,
                                .ImponibileTotaleNetto = 0,
                                .PivaConferente = PivaSocio,
                                .Conferimento_Speciale = New Contabilita_Conferimento_Speciale With {
                                    .FaseCodContratto = 0,
                                    .TagliandoPesa = "",
                                    .CodVarieta = CUL_COD,
                                    .DescAppezzamenti = "",
                                    .PremioComplessivo = 0
                                },
                                .MateriePrimeCampionature = MateriePrimeCampionature,
                                .RigheImpianti = impianti,
                                .ChkImpiantiIndefiniti = False
                            }



                                    '========================================================================================================
                                    'Salvataggio
                                    '--------------------------------------------------------------------------------------------------------
                                    Dim objOutput = objContabHelper.SalvaTestataPiuRiga(objContabTestata, CAU_CARICO, 0, 0, True, objContabDettaglio)

                                    If objOutput.MsgError <> "" Then

                                        Err_Code = 104
                                        Err_Des = "Non è possibile inserire il DDT numero " & NumeroBolla & " (" & objOutput.MsgError & ")" & Chr(13)
                                        BollaImportabile = False

                                    Else

                                        'Inserimento Xml 
                                        Id_Agenda_Output = objOutput.Id_Agenda
                                        Id_Mov_Carico_Output = objOutput.Id_Mov_Carico

                                        objContabTestata.IdAgenda = Id_Agenda_Output
                                        objContabTestata.DataOraUltimaLettura = Now


                                        'Salvataggio LOG
                                        ObjImportXml_W.Scrivi(objParametri_Server, Piva_SuperUser, FileImportazione,
                                                            Id_Agenda_Output, LAVCOD_ACCETTAZIONE_DIVERSI, Tipo_Xml,
                                                            enum_WFlow_Import_XML_Universale.File_Importazione_OK, Outer_Xml, DataRaccolta)



                                        '##################################################################################
                                        '###################    CREAZIONE ALTRI DETTAGLI DEL DDT    #######################
                                        '##################################################################################


                                        For i_Bolle2 = i_Bolle + 1 To UBound(Bolle, 2) - 1

                                            If Cod_RisUm = Bolle(COL_COD_RISUM, i_Bolle2) AndAlso
                                             NumeroBolla = Bolle(COL_NUMEROBOLLA, i_Bolle2) Then

                                                Fase = "Creazione Dettaglio"

                                                PivaSocio = Bolle(COL_PIVASOCIO, i_Bolle2)
                                                Sa_Cod = Bolle(COL_SA_COD, i_Bolle2)
                                                Appezza = Bolle(COL_APPEZZA, i_Bolle2)
                                                Id_Reg = Bolle(COL_ID_REG, i_Bolle2)
                                                Sup_Imp = Bolle(COL_SUP_IMP, i_Bolle2)
                                                Mat_Cod = Bolle(COL_MAT_COD, i_Bolle2)
                                                Mat_Des = Bolle(COL_MAT_DES, i_Bolle2)
                                                LottoIngresso = Bolle(COL_LOTTOINGRESSO, i_Bolle2)

                                                Udm_Cod = Bolle(COL_UDM_COD, i_Bolle2)
                                                Udm_Des = Bolle(COL_UDM_DES, i_Bolle2)
                                                Qta = Bolle(COL_QTA, i_Bolle2)
                                                CodiceArticolo = Bolle(COL_CODICE_ARTICOLO, i_Bolle2)
                                                Cod_Indirizzo = Bolle(COL_COD_INDIRIZZO, i_Bolle2)

                                                VEG_COD = Bolle(COL_VEG_COD, i_Bolle2)
                                                Veg_Des = Bolle(COL_VEG_DES, i_Bolle2)
                                                Rag_Soc = Bolle(COL_RAG_SOC, i_Bolle2)

                                                '================================================================================================
                                                'Impianto
                                                '------------------------------------------------------------------------------------------------
                                                Dim impianti2 As New List(Of Contabilita_Impianti_Raccolta)

                                                Dim objImpianto2 As New Contabilita_Impianti_Raccolta With {
                                                    .Piva = PivaSocio,
                                                    .Sa_Cod = Sa_Cod,
                                                    .Appezza = Appezza,
                                                    .Id_Reg = Id_Reg,
                                                    .Veg_Des = Veg_Des,
                                                    .Qta = Qta,
                                                    .Sup_Imp = Sup_Imp
                                                }


                                                impianti2.Add(objImpianto2)

                                                '========================================================================================================

                                                '========================================================================================================
                                                'Campionatura
                                                '--------------------------------------------------------------------------------------------------------
                                                Dim Campionatura2 As New Dictionary(Of String, String)
                                                Dim MateriePrimeCampionature2 = LeggiMateriePrimeCampionatureAgribologna(Campionatura2, Cod_RisUm)
                                                '========================================================================================================

                                                '========================================================================================================
                                                'Dettaglio
                                                '--------------------------------------------------------------------------------------------------------
                                                Dim objContabDettaglio2 As New Contabilita_Riga With {
                                            .Piva = Piva_SuperUser,
                                            .SaCod = Sa_Cod_Carico,
                                            .IdAgenda = Id_Agenda_Output,
                                            .IdMov = Id_Mov_Carico_Output,
                                            .IdMovDet = 0,
                                            .ModuloGias = enum_Omni_Modulo_Generazione.FreshFood,
                                            .ElemCod = TRASFORMATI_VEGETALI,
                                            .ProCod = 0,
                                            .MatCod = Mat_Cod,
                                            .MatDes = Mat_Des,
                                            .Note = "",
                                            .DestinazioneCarico = MagazzinoCarico,
                                            .DestinazioneScarico = MagazzinoCarico,
                                            .CalCod = 0,
                                            .CodIva = -1,
                                            .Pendente = 4,
                                            .jolly_int = 0,
                                            .ScontoModalita = 0,
                                            .UdM = enum_UnitaMisura.KG,
                                            .PrezzoRiferitoA = enum_PrezzoLivello.Kg_Litri,
                                            .ValoreRiferimentoPrezzo = 0,
                                            .NumConfezioni = 0,
                                            .NumContenitori = 0,
                                            .NumImballi = 0,
                                            .Tara = 0,
                                            .Lotto = LottoIngresso,
                                            .KgLordi = Qta,
                                            .KgNetti = Qta,
                                            .Degrado = 0,
                                            .Prezzo = 0,
                                            .PrezzoEffettivoKgL = 0,
                                            .PrezzoNetto = 0,
                                            .ImportoUnitario = 0,
                                            .ImponibileTotale = 0,
                                            .ImponibileTotaleNetto = 0,
                                            .PivaConferente = PivaSocio,
                                            .Conferimento_Speciale = New Contabilita_Conferimento_Speciale With {
                                                .FaseCodContratto = 0,
                                                .TagliandoPesa = "",
                                                .CodVarieta = CUL_COD,
                                                .DescAppezzamenti = "",
                                                .PremioComplessivo = 0
                                            },
                                            .MateriePrimeCampionature = MateriePrimeCampionature2,
                                            .RigheImpianti = impianti2,
                                            .ChkImpiantiIndefiniti = False
                                        }


                                                '========================================================================================================
                                                'Salvataggio
                                                '--------------------------------------------------------------------------------------------------------
                                                objContabTestata.DataOraUltimaLettura = Now

                                                Dim objOutputDettaglio2 = objContabHelper.SalvaTestataPiuRiga(objContabTestata, CAU_CARICO, Id_Mov_Carico_Output, 0, True, objContabDettaglio2)

                                                If objOutputDettaglio2.MsgError <> "" Then

                                                    Err_Code = 104
                                                    Err_Des = Err_Des & "Non è possibile inserire il DDT numero " & NumeroBolla & " (" & objOutput.MsgError & ")" & Chr(13)
                                                    BollaImportabile = False

                                                Else

                                                    'Aggiornamento Import Key
                                                    Outer_Xml = Outer_Xml & LottoIngresso & "-" & CInt(Qta) & " "

                                                    'Aggiornamento LOG
                                                    ObjImportXml_W.ModificaMessaggio(objParametri_Server, Piva_SuperUser, Id_Agenda_Output, Trim(Outer_Xml))

                                                    'Marco la Riga come Processata
                                                    Bolle(COL_VALIDITA, i_Bolle2) = 1


                                                End If

                                            End If

                                        Next



                                    End If


                                End If


                                Select Case BollaImportabile

                                    Case True

                                        NumeroBolleImportate += 1

                                    Case False

                                        'Marco tutte le righe della bolla come non processabili
                                        For i_Bolle2 = i_Bolle To UBound(Bolle, 2) - 1

                                            If Cod_RisUm = Bolle(COL_COD_RISUM, i_Bolle2) AndAlso
                                                    NumeroBolla = Bolle(COL_NUMEROBOLLA, i_Bolle2) Then

                                                Bolle(COL_VALIDITA, i_Bolle2) = -1 'Non valida

                                            End If

                                        Next

                                        If Err_Code <> 0 Then

                                            Err_Des_File = Err_Des_File & " Lotto Ingresso: " & LottoIngresso & " Errore No. " & Err_Code & " " & Err_Des & Chr(13)
                                            NumeroBolleErrate += 1


                                        End If


                                End Select



                            End If

                        End If

                    Next


                    r.RispostaStringa = "File Importazione " & FileImportazione & ": Righe Presenti: " & NumeroBollePresenti & ", Bolle Importate: " & NumeroBolleImportate & " (" & NumeroBolleSovrascritte & " Sovrascritte), Righe Errate: " & NumeroBolleErrate & ", Bolle Già Presenti: " & NumeroBolleGiaImportate

                End If

            End If

            Select Case Trim(Err_Des_File)

                Case ""

                    r.Errore = "Processo Terminato Correttamente."
                    r.RispostaOK = True

                    'If NumeroBolleImportate <> 0 Then
                    '    r.RispostaOK = True
                    'Else
                    '    r.Errore = "NumeroBolleImportate = 0"
                    '    r.RispostaOK = False
                    'End If
                Case Else

                    r.Errore = Err_Des_File
                    r.RispostaOK = True

            End Select

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore critico durante l'elaborazione del file importazione " & FileImportazione & " al " & i_Bolle & "/" & NumeroBollePresenti & " della Fase " & Fase & " (" & ex.Message & ")"
        End Try

        Return r

    End Function


    Public Shared Function Esporta_Trapianti_Agribologna_Sync(ByVal objParametri_Server As AgronicaCoreParametri,
                                                              ByVal objParametri_Utenti As AgronicaCoreParametri,
                                                              ByVal Validita_Inizio_Pluriennali As String,
                                                              ByVal Validita_Fine_Pluriennali As String,
                                                              ByVal Validita_Inizio_Annuali As String,
                                                              ByVal Validita_Fine_Annuali As String
                                                              ) As RispostaStandard

        Dim Err_Code As Integer = 0
        Dim Err_Des As String = ""
        Dim Err_Des_File As String = ""

        Dim VEG_COD As Integer = 0
        Dim GRVA_COD_VEG As Integer = 0
        Dim CUL_COD As Integer = 0
        Dim Elem_Cod As Integer = TRASFORMATI_VEGETALI

        Dim ObjImport_R As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim ObjMaterie_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim ObjImpianti_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim ObjSpecie_R As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Dim ObjCultivar_R As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim objFile As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim DT_Traccia As New DataTable
        Dim DT_Prodotto As New DataTable
        Dim DT_Impianto As New DataTable
        Dim DT_Specie As New DataTable
        Dim DT_Cultivar As New DataTable
        Dim DT_Trapianti As New DataTable

        Dim r As New RispostaStandard

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XmlDati As System.Xml.XmlElement
        Dim XmlDato As System.Xml.XmlElement


        Dim Count As Integer = 0
        Dim Count_Totale As Integer = 0

        'Variabili Dati

        Dim Mat_Cod As Integer

        Dim FileEsportazione As String = ""

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Try

            'Recupero File Esportazione
            FileEsportazione = objFile.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_FILE_ESPORTAZIONE_DATI, objParametri_Utenti, 2)

            If String.IsNullOrEmpty(FileEsportazione) Then

                Err_Code = 100
                Err_Des_File = Err_Des_File & "Il File di esportazione non è impostato." & Chr(13)

            Else

                '##################################################################################################################################
                '########################################## DECODIFICA ############################################################################
                '##################################################################################################################################

                Dim bImpianto_Valido As Boolean
                Dim Validita_Inizio As Date
                Dim Validita_Fine As Date


                Dim strFiltro As String

                Dim bFirst As Boolean

                Dim ggDurata As Double


                'XmlDoc = CreateObject("Msxml.DOMDocument")
                XmlDati = XmlDoc.CreateElement("Dati")


                'Filtro la semina/trapianto
                strFiltro = "(Agenda.Lav_Cod = 2 Or Agenda.Lav_Cod = 71) "

                '========================================================================================================
                'Filtro Date
                '--------------------------------------------------------------------------------------------------------
                If Not IsDate(Validita_Inizio_Pluriennali) Then
                    Validita_Inizio_Pluriennali = AGRODATAINIZIO
                End If

                If Not IsDate(Validita_Fine_Pluriennali) Then
                    Validita_Fine_Pluriennali = AGRODATAFINE
                End If

                If Not IsDate(Validita_Inizio_Annuali) Then
                    Validita_Inizio_Annuali = AGRODATAINIZIO
                End If

                If Not IsDate(Validita_Fine_Annuali) Then
                    Validita_Fine_Annuali = AGRODATAFINE
                End If


                If Validita_Inizio_Annuali < Validita_Inizio_Pluriennali Then
                    Validita_Inizio = Validita_Inizio_Annuali
                Else
                    Validita_Inizio = Validita_Inizio_Pluriennali
                End If

                If Validita_Fine_Annuali > Validita_Fine_Pluriennali Then
                    Validita_Fine = Validita_Fine_Annuali
                Else
                    Validita_Fine = Validita_Fine_Pluriennali
                End If

                DT_Trapianti = ObjImport_R.Leggi_Import_Agribologna("", CAU_LAVORAZIONE, Validita_Inizio, Validita_Fine, enumSelezioneVariabile.Selezione_JoinCompleta, strFiltro, "", objParametri_Server)

                DT_Specie = ObjSpecie_R.Leggi(0,
                                              0,
                                              "",
                                              "",
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "",
                                              "",
                                              objParametri_Server)



                If DT_Trapianti.Rows.Count > 0 Then

                    Count_Totale = DT_Trapianti.Rows.Count

                    For Each dr_trapianti As DataRow In DT_Trapianti.Rows

                        Count += 1

                        DT_Traccia = ObjImport_R.Leggi_Semine_Trapianti_Import_Agribologna(dr_trapianti("Piva"),
                                                                                           CLng(dr_trapianti("Sa_Cod")),
                                                                                           CLng(dr_trapianti("Id_Agenda")),
                                                                                           10,
                                                                                           CAU_LAVORAZIONE,
                                                                                           enumSelezioneVariabile.Selezione_JoinCompleta,
                                                                                           "Lav_Cod In (2, 71)",
                                                                                           "",
                                                                                           objParametri_Server)




                        If DT_Traccia.Rows.Count <> 0 Then

                            'Lettura della specie vegetale dell'impianto
                            DT_Impianto = ObjImpianti_R.Leggi(DT_Traccia(0)("Piva"),
                                                              DT_Traccia(0)("Sa_Cod"),
                                                              DT_Traccia(0)("Appezza"),
                                                              DT_Traccia(0)("Id_Destinazione"),
                                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                              "",
                                                              "",
                                                              objParametri_Server)




                            If DT_Impianto.Rows.Count > 0 Then


                                '==============================================================================================
                                'Controllo Validità Impianto
                                '----------------------------------------------------------------------------------------------
                                'Controllo Durata Impianto per Verificare se è Pluriennale
                                ggDurata = DateDiff("d", CDate(DT_Impianto(0)("Validita_Inizio")), CDate(DT_Impianto(0)("Validita_Fine")))

                                Select Case ggDurata

                                    Case Is > 366 'Impianto Pluriennale

                                        Validita_Inizio = Validita_Inizio_Pluriennali
                                        Validita_Fine = Validita_Fine_Pluriennali

                                    Case Else

                                        Validita_Inizio = Validita_Inizio_Annuali
                                        Validita_Fine = Validita_Fine_Annuali

                                End Select

                                'Controllo Validita
                                If CDate(dr_trapianti("Data_Movimento")) >= Validita_Inizio AndAlso
                                   CDate(dr_trapianti("Data_Movimento")) <= Validita_Fine Then

                                    bImpianto_Valido = True

                                Else

                                    bImpianto_Valido = False 'Impianto Temporalmente Non Valido

                                End If

                                If bImpianto_Valido Then

                                    'Lettura del Veg_Cod
                                    DT_Cultivar = ObjCultivar_R.Leggi(DT_Impianto(0)("Cul_Cod"),
                                                                      0,
                                                                      "",
                                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "",
                                                                      "",
                                                                      objParametri_Server)

                                    If DT_Cultivar.Rows.Count > 0 Then

                                        VEG_COD = UtilityProvider.Agro_SQL_SaveNum(DT_Cultivar(0)("Veg_Cod"))
                                        GRVA_COD_VEG = 0

                                        DT_Specie.Select("Veg_Cod = " & VEG_COD)

                                        If DT_Traccia(0)("Pro_Cod") <> 0 Then

                                            'Utilizzato una tipologia generica

                                            'Il Cul_Cod risulta essere quello dell'impianto
                                            CUL_COD = UtilityProvider.Agro_SQL_SaveNum(DT_Cultivar(0)("Cul_Cod"))

                                        Else

                                            Mat_Cod = DT_Traccia(0)("Mat_Cod")

                                            'Lettura del Prodotto dal Codice Articolo
                                            DT_Prodotto = ObjMaterie_R.Leggi2("", Elem_Cod, Mat_Cod, "", 0, "", True, enumSelezioneVariabile.Selezione_TabellaCompleta, "Elem_Cod in (201, 210)", "", objParametri_Server)

                                            If DT_Prodotto.Rows.Count > 0 Then


                                                VEG_COD = UtilityProvider.Agro_SQL_SaveNum(DT_Prodotto(0)("Veg_Cod"))
                                                GRVA_COD_VEG = UtilityProvider.Agro_SQL_SaveNum(DT_Prodotto(0)("Grva_Cod_Veg"))
                                                CUL_COD = UtilityProvider.Agro_SQL_SaveNum(DT_Prodotto(0)("Cul_Cod"))

                                                'Controllo che la specie sia la lattuga (Veg_Cod = 35)
                                                If VEG_COD = 35 AndAlso DT_Traccia.Rows.Count > 1 Then

                                                    CUL_COD = 1490  'altre

                                                End If

                                            End If

                                        End If

                                        bFirst = True

                                        For Each dr_traccia As DataRow In DT_Traccia.Rows

                                            If ((CUL_COD = 1490 AndAlso bFirst) OrElse CUL_COD <> 1490) Then

                                                bFirst = False

                                                XmlDato = XmlDoc.CreateElement("Dato")

                                                XmlDato.SetAttribute("FLAG_SEMINA_TRAPIANTO", If(dr_traccia("Lav_Cod") = 2, "S", "T"))
                                                XmlDato.SetAttribute("PARTITA_IVA_SOCIO", dr_traccia("Piva"))
                                                XmlDato.SetAttribute("SPECIE", VEG_COD)
                                                XmlDato.SetAttribute("TIPOLOGIA", GRVA_COD_VEG)
                                                XmlDato.SetAttribute("VARIETA", CUL_COD)
                                                XmlDato.SetAttribute("DATA", Format(dr_trapianti("Data_Movimento"), "dd/MM/yyyy"))

                                                XmlDati.AppendChild(XmlDato)

                                                XmlDato = Nothing


                                            Else
                                                bFirst = False
                                                Exit For


                                            End If

                                        Next


                                    End If

                                End If

                            End If

                        End If

                    Next

                End If


                XmlDoc.AppendChild(XmlDati)

                'Salvataggio
                File.WriteAllText(FileEsportazione, XmlDoc.OuterXml)


            End If


            Select Case Err_Code

                Case 0
                    r.RispostaOK = True
                    r.Errore = ""
                    r.RispostaStringa = "File " & FileEsportazione & " generato con successo."

                Case Else

                    r.RispostaOK = False
                    r.Errore = If(Err_Des = "", Err_Des_File, Err_Des)
                    r.RispostaStringa = If(Err_Des = "", Err_Des_File, Err_Des)

            End Select

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "File " & FileEsportazione & " non processabile. Errore al " & Count & "/" & Count_Totale & " (" & ex.Message & ")"
        End Try

        Return r

    End Function



    Public Shared Function Esporta_Lotti_Agribologna_Sync(ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard

        Dim Err_Code As Integer = 0
        Dim Err_Des As String = ""
        Dim Err_Des_File As String = ""

        Dim FileEsportazione As String = ""

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XmlDati As System.Xml.XmlElement

        Dim Elem_Cod As Integer = TRASFORMATI_VEGETALI


        Dim ObjAppezza_R As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim ObjImport_R As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim ObjRaccolta_R As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
        Dim ObjDestinazioni_R As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim objFile As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read


        Dim DT_Conferimenti As New DataTable
        Dim DT_Appezza As New DataTable
        Dim DT_Raccolta As New DataTable
        Dim DT_Destinazione As New DataTable

        Dim r As New RispostaStandard

        Dim XmlPM101703 As System.Xml.XmlElement
        Dim XmlPM101704 As System.Xml.XmlElement
        Dim XmlPM101705 As System.Xml.XmlElement
        Dim XmlPM101703Text As System.Xml.XmlText
        Dim XmlPM101704Text As System.Xml.XmlText
        Dim XmlPM101705Text As System.Xml.XmlText
        Dim XmlP100147M100091 As System.Xml.XmlElement


        'Variabili Dati

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Try

            'Recupero File Esportazione
            FileEsportazione = objFile.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_FILE_ESPORTAZIONE_DATI_2, objParametri_Utenti, 2)

            FileEsportazione = "C:\giaslan\temp_prova_export.xml"

            If String.IsNullOrEmpty(FileEsportazione) Then

                Err_Code = 100
                Err_Des = Err_Des_File & "Il File di esportazione non è impostato." & Chr(13)

            Else


                XmlDati = XmlDoc.CreateElement("dataroot")

                DT_Conferimenti = ObjImport_R.Leggi_Tracciabilita_Agribologna(Elem_Cod,
                                                                          CAU_CARICO,
                                                                          enumSelezioneVariabile.Selezione_JoinCompleta,
                                                                          "Lav_Cod = 1054",
                                                                          "",
                                                                          objParametri_Server)


                If DT_Conferimenti.Rows.Count > 0 Then

                    For Each dr_conferimenti As DataRow In DT_Conferimenti.Rows

                        DT_Raccolta = ObjRaccolta_R.Leggi_Specifica(Piva_SuperUser, 0, dr_conferimenti("id_agenda"), 0, dr_conferimenti("id_mov_det"), 0, "", "", 0, 0, 0, 0, 125, "", "", "", objParametri_Server)

                        If DT_Raccolta.Rows.Count > 0 Then

                            'Lettura dei Dati Raccolta
                            If DT_Raccolta(0).Item("Id_Agenda_Rif") <> 0 Then

                                DT_Destinazione = ObjDestinazioni_R.Leggi(DT_Raccolta(0).Item("Piva_Rif"), 0, DT_Raccolta(0).Item("Id_Agenda_Rif"), 0, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "Tipo_Destinazione = 0", "", objParametri_Server)

                                If DT_Destinazione.Rows.Count > 0 Then

                                    '=================================================================
                                    'Lettura del nome dell'appezzamento
                                    '-----------------------------------------------------------------
                                    DT_Appezza = ObjAppezza_R.Leggi(DT_Destinazione(0)("Piva"),
                                                           DT_Destinazione(0)("Sa_Cod"),
                                                           UtilityProvider.Agro_SQL_SaveNum(DT_Destinazione(0)("Appezza")),
                                                           enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                           "",
                                                           "",
                                                           objParametri_Server)

                                    If DT_Appezza.Rows.Count > 0 Then


                                        XmlP100147M100091 = XmlDoc.CreateElement("P100147M100091")
                                        XmlPM101703 = XmlDoc.CreateElement("PM101703")
                                        XmlPM101704 = XmlDoc.CreateElement("PM101704")
                                        XmlPM101705 = XmlDoc.CreateElement("PM101705")

                                        XmlPM101703Text = XmlDoc.CreateTextNode(Trim(dr_conferimenti("Lotto")))
                                        XmlPM101704Text = XmlDoc.CreateTextNode(DT_Appezza(0)("Appezza"))
                                        XmlPM101705Text = XmlDoc.CreateTextNode(DT_Appezza(0)("App_Nome"))

                                        XmlPM101703.AppendChild(XmlPM101703Text)
                                        XmlPM101704.AppendChild(XmlPM101704Text)
                                        XmlPM101705.AppendChild(XmlPM101705Text)

                                        XmlP100147M100091.AppendChild(XmlPM101703)
                                        XmlP100147M100091.AppendChild(XmlPM101704)
                                        XmlP100147M100091.AppendChild(XmlPM101705)

                                        XmlDati.AppendChild(XmlP100147M100091)

                                        XmlPM101703 = Nothing
                                        XmlPM101704 = Nothing
                                        XmlPM101705 = Nothing
                                        XmlP100147M100091 = Nothing

                                    End If

                                End If


                            End If

                        End If

                    Next

                End If


                XmlDoc.AppendChild(XmlDati)

                'Salvataggio
                File.WriteAllText(FileEsportazione, XmlDoc.OuterXml)

            End If


            Select Case Err_Code
                Case 0
                    r.RispostaOK = True
                    r.Errore = ""
                    r.RispostaStringa = "File " & FileEsportazione & " generato con successo."
                Case Else
                    r.RispostaOK = False
                    r.Errore = Err_Des
                    r.RispostaStringa = "Importazione terminata con errori."
            End Select

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "File Esportazione Lotti non processabile."
        End Try

        Return r

    End Function


    Public Shared Function LeggiMateriePrimeCampionatureAgribologna(ByRef certificato As Dictionary(Of String, String), ByVal cod_risum As Integer) As List(Of Materia_Prima_Campionatura)

        Return New List(Of Materia_Prima_Campionatura) From {
            New Materia_Prima_Campionatura With {.Progressivo = 0, .Tipo = "ofornitore", .Tipo_Cod = cod_risum, .Val_Cod = "", .ChkTara_Campionatura = 0, .Tara_Campionatura = 0}
        }

    End Function


    Public Shared Function Importa_QDC_Agribologna_Sync(ByVal objParametri_Super_Server As AgronicaCoreParametri,
                                                        ByVal objParametri_Server As AgronicaCoreParametri,
                                                        ByVal objParametri_Utenti As AgronicaCoreParametri,
                                                        ByVal Piva As String,
                                                        Optional ByVal bGSB As Boolean = False) As RispostaStandard


        Dim r As New RispostaStandard

        'Parametri
        Dim Validita_Inizio As String
        Dim Validita_Fine As String


        Dim DtImprese As New DataTable
        Dim DtImpianti As New DataTable
        Dim DtSpecie As New DataTable
        Dim DTUtenti As New DataTable

        Dim ObjImprese_R As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim ObjSpecie_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim ObjImpianti_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim ObjUtenti_R As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
        Dim objMail As New Mail
        Dim ScriviLog As New LogProvider

        Validita_Inizio = Year(Date.Now) & "-01-01"
        Validita_Fine = Year(Date.Now) & "-12-31"


        Dim strXmlVariabilistampe As String = ""
        Dim StrNodiVariabili As String = ""
        Dim StrNodo As String = ""
        Dim objVS As New AgronicaCoreXML.XML_Stampe
        Dim Sa_Cod As Integer = 0
        Dim Veg_Cod As Integer = 0
        Dim Cul_Cod As Integer = 0
        Dim Rag_Soc As String = ""
        Dim Veg_Des As String = ""
        Dim HashImp As New Hashtable
        Dim strDataFiltro As String = String.Empty
        Dim ProgressivoGIAS As Integer = 0
        Dim xFiltroAggiuntivo As String = ""

        Dim myRes As Net.WebResponse = Nothing
        Dim myReq As Net.HttpWebRequest
        Dim Count As Integer = 0
        Dim Err_Count As Integer = 0
        Dim Numero_Report As Integer = 1000000 'Dummy
        Dim Id_Tipologia As Integer = 0
        Dim Id_Indice_Veg_Cod As Integer = 0
        Dim Id_Indice_Anno As Integer = 0
        Dim Id_StampeReport As enum_CodificaStampe = enum_CodificaStampe.Eurep_Gap_Multicentro
        Dim bOk As Boolean = False
        Dim erroreMail As String = ""
        Dim strerrore As String = ""

        Dim nomeRoutine As String = "Importa_QDC_Agribologna_Sync"


        Try

            'Piva = "04236970408"
            'xFiltroAggiuntivo = " SpecieVegetali.Veg_Cod = 66

            'Lettura Codice
            DTUtenti = ObjUtenti_R.Leggi("username = '" & objParametri_Utenti.SuperUserUsername & "'", "", objParametri_Utenti)
            ProgressivoGIAS = DTUtenti(0)("ProgressivoGIAS")

            'Lettura Imprese
            DtImprese = ObjImprese_R.Leggi(Piva, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            If DtImprese.Rows.Count > 0 Then

                'Costruzione Xml Variabili Stampe

                strDataFiltro = String.Format("  Imprese_Progetti.Validita_Inizio <= {0} ", "Convert(datetime, '" & Validita_Fine & "', 120)")
                strDataFiltro = String.Format("{0}AND Imprese_Progetti.Validita_Fine >= {1} ", strDataFiltro, "Convert(datetime, '" & Validita_Inizio & "', 120)")


                'Lettura Configurazione Servizi
                Dim objConfigurazioneServizi As New AgronicaCoreVarieDAL.Configurazione_Servizi_R
                Dim configurazioneServizio = objConfigurazioneServizi.LeggiSingolo(
                        objParametri_Server.PivaSuperUser, enum_Id_Servizio.GiasOnline,
                        enum_Tipi_Servizi_Background.ImportQDC_Documentale,
                        0, "", objParametri_Super_Server)

                'numero report max
                If configurazioneServizio Is Nothing OrElse String.IsNullOrEmpty(configurazioneServizio.Parametri_Extra) Then
                    Throw New Exception(String.Format("Configurazione servizio non disponibile o percorso non impostato"))
                End If

                Dim vPara As String() = configurazioneServizio.Parametri_Extra.Split("|")
                Dim item As String = ""

                'Numero Report
                item = Array.Find(vPara, Function(x) x.StartsWith("numero_report" & "="))
                If item IsNot Nothing Then
                    If CInt(item.Split("=")(1)) > 0 Then
                        Numero_Report = item.Split("=")(1)
                    End If
                End If

                'Id_Tipologia
                item = Array.Find(vPara, Function(x) x.StartsWith("id_tipologia" & "="))
                If item IsNot Nothing Then
                    Id_Tipologia = item.Split("=")(1)
                End If

                'Id_Indice_Veg_Cod
                item = Array.Find(vPara, Function(x) x.StartsWith("id_indice_veg_cod" & "="))
                If item IsNot Nothing Then
                    Id_Indice_Veg_Cod = item.Split("=")(1)
                End If

                'Id_StampeReport
                item = Array.Find(vPara, Function(x) x.StartsWith("Id_StampeReport" & "="))
                If item IsNot Nothing Then
                    Dim sVal As String = item.Split("=")(1)
                    Dim tmpInt As Integer
                    If Integer.TryParse(sVal, tmpInt) Then
                        If [Enum].IsDefined(GetType(enum_CodificaStampe), tmpInt) Then
                            Id_StampeReport = CType(tmpInt, enum_CodificaStampe)
                        End If
                    End If
                Else
                    Id_StampeReport = enum_CodificaStampe.Eurep_Gap_Multicentro
                End If

                'Id_Indice_Anno
                item = Array.Find(vPara, Function(x) x.StartsWith("id_indice_anno" & "="))
                If item IsNot Nothing Then
                    Id_Indice_Anno = item.Split("=")(1)
                End If

                If Id_Tipologia = 0 OrElse Id_Indice_Anno = 0 OrElse Id_Indice_Veg_Cod = 0 Then
                    Throw New Exception(String.Format("Parametri extra non impostati"))
                End If

                For Each dr_Imprese As DataRow In DtImprese.Rows

                    'Lettura delle specie vegetali valide per l'impresa
                    DtSpecie = ObjSpecie_R.Distinct_SpecieColtivate(dr_Imprese("Piva"), xFiltroAggiuntivo, "", objParametri_Server)

                    If DtSpecie.Rows.Count > 0 Then

                        For Each dr_Specie As DataRow In DtSpecie.Rows

                            If Numero_Report <= Count Then
                                GoTo Fine
                            End If

                            Rag_Soc = dr_Imprese("Rag_Soc")
                            Veg_Des = dr_Specie("Veg_Des")

                            Sa_Cod = 0
                            Veg_Cod = dr_Specie("Veg_Cod")
                            Cul_Cod = 0

                            DtImpianti = ObjImpianti_R.Leggi_Impianti_xAgenda2(False,
                                                                            dr_Imprese("Piva"),
                                                                            0,
                                                                            Veg_Cod,
                                                                            Cul_Cod,
                                                                            "",
                                                                            0,
                                                                            -1,
                                                                            strDataFiltro,
                                                                            " Cul_Des, App_Nome, Progetto ",
                                                                            objParametri_Server, True)

                            If DtImpianti.Rows.Count > 0 Then

                                bOk = True

                                If Numero_Report < 1000000 Then

                                    'Lettura della chiave
                                    Dim objElenco As New AgronicaCoreScadenziario.Alert_Elenco_R
                                    Dim dt As DataTable = objElenco.Leggi_x_QDC(dr_Imprese("Piva"), Id_Tipologia, Id_Indice_Veg_Cod, Veg_Cod, Id_Indice_Anno, Year(Validita_Inizio), objParametri_Server)

                                    If dt.Rows.Count > 0 Then

                                        If CInt(Format(Now, "yyyyMMdd")) = CInt(Format(dt(0)("Data_Upload"), "yyyyMMdd")) Then
                                            bOk = False 'Documento generato lo stesso giorno --> bypass
                                        End If

                                    End If

                                End If



                                If bOk Then

                                    HashImp.Clear()
                                    StrNodiVariabili = ""

                                    For i = 0 To DtImpianti.Rows.Count - 1
                                        If Not HashImp.ContainsKey(DtImpianti.Rows(i).Item("sa_cod") & "|" & DtImpianti.Rows(i).Item("appezza") & "|" & DtImpianti.Rows(i).Item("id_reg")) Then
                                            HashImp.Add(DtImpianti.Rows(i).Item("sa_cod") & "|" & DtImpianti.Rows(i).Item("appezza") & "|" & DtImpianti.Rows(i).Item("id_reg"), "")
                                            Dim vVarStampe(5) As ElementoStampe
                                            vVarStampe(0).Nome = "piva"
                                            vVarStampe(0).Valore = DtImpianti.Rows(i).Item("piva")
                                            vVarStampe(1).Nome = "sa_cod"
                                            vVarStampe(1).Valore = DtImpianti.Rows(i).Item("sa_cod")
                                            vVarStampe(2).Nome = "appezza"
                                            vVarStampe(2).Valore = DtImpianti.Rows(i).Item("appezza")
                                            vVarStampe(3).Nome = "id_reg"
                                            vVarStampe(3).Valore = DtImpianti.Rows(i).Item("id_reg")
                                            vVarStampe(4).Nome = "veg_cod"
                                            vVarStampe(4).Valore = DtImpianti.Rows(i).Item("veg_cod")
                                            vVarStampe(5).Nome = "StampaDiretta" 'Inserimento Flag StampaDiretta
                                            vVarStampe(5).Valore = 1
                                            StrNodo = objVS.XML_VariabiliStampe(vVarStampe)
                                            StrNodiVariabili &= StrNodo
                                        End If
                                    Next


                                    'Costruzione Root
                                    Dim XmlDoc As New System.Xml.XmlDocument
                                    Dim xmlVariabiliStampe As XmlElement = XmlDoc.CreateElement("ParametriAgronicaStampe_2010")

                                    xmlVariabiliStampe.InnerXml &= StrNodiVariabili

                                    strXmlVariabilistampe = xmlVariabiliStampe.OuterXml

                                    'Impostazione URL e Web Parametri
                                    Dim objStampeUtility As New AgronicaCoreStampeDAL.Utility
                                    Dim url As String = objStampeUtility.GetUrlStampaSchedaCampagna(dr_Imprese("Piva"),
                                                                                                    Id_StampeReport,
                                                                                                    strXmlVariabilistampe,
                                                                                                    ProgressivoGIAS,
                                                                                                    objParametri_Super_Server,
                                                                                                    objParametri_Server,
                                                                                                    objParametri_Utenti,
                                                                                                    Enum_SiteRedirector.AgroGSB)



                                    'Chiamata Gestione Richieste

                                    Try

                                        myReq = Net.WebRequest.Create(url)
                                        myReq.AllowAutoRedirect = True
                                        myReq.KeepAlive = True
                                        myReq.CookieContainer = New Net.CookieContainer()
                                        myReq.Timeout = 300000

                                        myRes = myReq.GetResponse()

                                        Count += 1



                                    Catch ex As Exception

                                        Select Case bGSB

                                            Case True

                                                'Invio Mail di Errore
                                                Err_Count += 1

                                                If Err_Count < 5 Then

                                                    If configurazioneServizio.Destinatari <> "" Then

                                                        strerrore = "Stampa non processabile. Ragione Sociale " & Rag_Soc & " Specie " & Veg_Des & " (" & ex.Message & " " & ")"

                                                        If Not IsNothing(ex.InnerException) Then

                                                            strerrore = strerrore & " " & ex.InnerException.ToString

                                                        End If

                                                        ScriviLog.Scrivi_LOG(objParametri_Server, nomeRoutine, strerrore)

                                                        erroreMail = objMail.invia(objParametri_Server, configurazioneServizio.Mittente, configurazioneServizio.Destinatari, "", "",
                                                               "Errore Import QDC AGB Documentale " & " " & Now.ToShortDateString & " " & Now.ToShortTimeString,
                                                               strerrore, False, Nothing)

                                                    End If


                                                End If

                                            Case False

                                                'Lancio eccezione
                                                Throw New Exception(String.Format("Stampa non processabile."))


                                        End Select



                                    End Try

                                End If

                            End If

                        Next


                    End If


                Next


            End If

Fine:

            r.RispostaOK = True
            r.RispostaStringa = "Aggiornati correttamente " & Count & " documenti."



        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Ragione Sociale " & Rag_Soc & " Specie " & Veg_Des & " (" & ex.Message & ")"
        End Try

        Return r

    End Function












    Public Shared Function LeggiContabilitaMagazzino(ByVal Piva As String, ByVal Sa_Cod As Integer, ByRef Magazzino As Contabilita_Magazzino, ByRef objParametriServer As AgronicaCoreParametri) As Boolean

        Dim objSequenze As New Agro_Sequenze
        Dim objVascheR As New AgronicaCoreAnagrafeDAL.Cantina_Vasche_R
        Dim objVascheW As New AgronicaCoreAnagrafeDAL.Cantina_Vasche_W
        Dim esito As Boolean

        Dim dtMagazzino = objVascheR.Leggi(Piva, Sa_Cod, 0, "", "", objParametriServer)
        If dtMagazzino.Rows.Count > 0 Then
            Magazzino.SaCod = CInt(dtMagazzino.Rows(0).Item("Sa_Cod"))
            Magazzino.TipoDestinazione = CInt(dtMagazzino.Rows(0).Item("Tipo_Destinazione"))
            Magazzino.IdDestinazione = CInt(dtMagazzino.Rows(0).Item("Vas_Cod"))
            esito = True
        Else
            Dim Vas_Cod = objSequenze.NuovoId_SeqMagazzino(Piva, Sa_Cod, 0, 20000000, objParametriServer)
            esito = objVascheW.ScriviCella(Piva, Sa_Cod, Vas_Cod, 1, "Magazzino " & Sa_Cod, 0, "", objParametriServer)
            'esito = objVascheW.Scrivi(Piva, Sa_Cod, Vas_Cod, 1, "Cella 1", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, 0, 0, 0, 0, AGRODATAINIZIO, "R", 0, 0, 1, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, objParametriServer)
            If esito Then
                Magazzino.SaCod = Sa_Cod
                Magazzino.IdDestinazione = Vas_Cod
                Magazzino.TipoDestinazione = CELLA_FRIGORIFERA
            End If
        End If

        Return esito

    End Function





    Public Shared Function FormConvertToData(ByVal strData As String) As Date

        Dim Giorno As String = Right(strData, 2)
        Dim Mese As String = Mid(strData, 5, 2)
        Dim Anno As String = Left(strData, 4)

        Dim ris As String = Giorno & "/" & Mese & "/" & Anno

        If IsDate(ris) Then
            Return CDate(ris)
        Else
            'Eccezione
            Return AGRODATAINIZIO
        End If

    End Function


    Public Shared Function VerificaEliminaModificaDocumento(ByVal piva As String,
                                                            ByVal Id_Agenda As Integer,
                                                            ByVal Id_Mov As Integer,
                                                            ByVal Id_Mov_Det As Integer,
                                                            ByVal Lav_Cod As Integer,
                                                            ByVal Tipo_Operazione As Integer,
                                                            ByVal IgnoraAvvisoWarning As Boolean,
                                                            ByVal ModuloGiasLicenziato As Integer,
                                                            ByVal flagAggiornaConteggi As Boolean,
                                                            ByVal objParametri_Server As AgronicaCoreParametri
                                                            ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim messaggio As String = ""
        Dim Modalita_Protetta As Integer = 0

        Try

            r = UtilityHelper.CancellaDocumento(objParametri_Server,
                                                piva, Id_Agenda, Id_Mov, Id_Mov_Det,
                                                Lav_Cod, Tipo_Operazione, IgnoraAvvisoWarning, False,
                                                ModuloGiasLicenziato, flagAggiornaConteggi,
                                                messaggio, Modalita_Protetta, True)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
            r.RispostaStringa = ""
            r.RispostaConferma = False
        End Try

        Return r

    End Function













End Class
