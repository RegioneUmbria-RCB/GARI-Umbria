Imports System.Xml
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class XML_Privato_Anagrafe2

    Private Enum enum_prova
        pinco = 0
    End Enum


    '##########################################################################################
    Public Function XML_GerarchiaImprese(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                        ByVal padre As String, _
                                        ByRef StringaXML As String, _
                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE)


        Dim XmlDoc As New System.Xml.XmlDocument
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("GerarchiaImprese")

        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("padre"), CStr(padre))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(NodoXml)

        'Restituisco in uscita la stringa creata
        StringaXML = XmlDoc.InnerXml

        'Distruggo gli oggetti
        NodoXml = Nothing
        XmlDoc = Nothing

        'Restituisco in uscita 
        Return StringaXML




    End Function


    '##########################################################################################
    'Viene chiamata quando si deve creare l'xml di una sola impresa
    'crea il nodo DatiImprese
    'e poi chiama la funzione XML_2_Impresa che crea tutto il blocco dell'impresa (impresa, indirizzi, codici, gerarchia, ecc)
    Public Function XML_2_Imprese(ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByVal BaseCode As Integer, _
                                    ByVal TopCode As Integer, _
                                    ByVal TipoOperazioneDB_Impresa As enum_TipoOperazioneDB, _
                                    ByVal TipoOperazioneDB_Contatto As enum_TipoOperazioneDB, _
                                    ByVal Piva As String, _
                                    ByVal Rag_Soc As String, _
                                    ByVal PivaPadre As String, _
                                    ByVal PivaContatto As String, _
                                    ByVal TipoImpresaGerarchia As Integer, _
                                    ByVal Tipo_Indirizzo As Integer, _
                                    ByVal Pro_Cod_Istat As String, _
                                    ByVal Com_Cod_Istat As String, _
                                    ByVal Cod_Indirizzo As Integer, _
                                    ByVal Ind_Des As String, _
                                    ByVal Frz_Des As String, _
                                    ByVal CAP As String, _
                                    ByVal Stato As String, _
                                    ByVal Note As String, _
                                    ByVal Dt_Codici As DataTable, _
                                    ByVal DT_RisUm As DataTable, _
                                    ByVal Dt_ContattiCodici As DataTable, _
                                    Optional ByVal Delega As String = "", _
                                    Optional ByVal AT_Prevalente As String = "", _
                                    Optional ByVal Forma_Giuridica As String = "", _
                                    Optional ByVal Forma_Conduzione As String = "", _
                                    Optional ByVal Sup_Totale As Decimal = 0, _
                                    Optional ByVal Note_Impresa As String = "", _
                                    Optional ByVal Sa_Cod As Integer = 0, _
                                    Optional ByVal Id_CF As Integer = 0, _
                                    Optional ByVal Convenevoli As String = "", _
                                    Optional ByVal Codice_Fiscale As String = "", _
                                    Optional ByVal Tipo_Indirizzo_Default As Integer = 0, _
                                    Optional ByVal Cod_Contatto_Referente As String = "", _
                                    Optional ByVal Validita_Inizio_Impresa As Date = AGRODATAINIZIO, _
                                    Optional ByVal Validita_Fine_Impresa As Date = AGRODATAFINE _
                                    ) As XmlElement


        Dim XmlDatiImprese As System.Xml.XmlElement
        Dim XmlImpresa As System.Xml.XmlElement


        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If



            '#######################################################
            '#################   DATI IMPRESE    ###################
            '#######################################################

            XmlDatiImprese = XmlDoc.CreateElement("DatiImprese")

            XmlDoc.AppendChild(XmlDatiImprese)


            '#######################################################
            '####################   IMPRESA    #####################
            '#######################################################

            XmlImpresa = XML_2_Impresa(Log_Errori, _
                                        XmlDoc, _
                                        BaseCode, _
                                        TopCode, _
                                        TipoOperazioneDB_Impresa, _
                                        TipoOperazioneDB_Contatto, _
                                        Piva, _
                                        Rag_Soc, _
                                        PivaPadre, _
                                        PivaContatto, _
                                        TipoImpresaGerarchia, _
                                        Tipo_Indirizzo, _
                                        Pro_Cod_Istat, _
                                        Com_Cod_Istat, _
                                        Cod_Indirizzo, _
                                        Ind_Des, _
                                        Frz_Des, _
                                        CAP, _
                                        Stato, _
                                        Note, _
                                        Dt_Codici, _
                                        DT_RisUm, _
                                        Dt_ContattiCodici, _
                                        Delega, _
                                        AT_Prevalente, _
                                        Forma_Giuridica, _
                                        Forma_Conduzione, _
                                        Sup_Totale, _
                                        Note_Impresa, _
                                        Sa_Cod, _
                                        Id_CF, _
                                        Convenevoli, _
                                        Codice_Fiscale, _
                                        Tipo_Indirizzo_Default, _
                                        Cod_Contatto_Referente, _
                                        Validita_Inizio_Impresa, _
                                        Validita_Fine_Impresa)

            XmlDatiImprese.AppendChild(XmlImpresa)



        Catch ex As Exception

            Log_Errori += ex.Message

        End Try


        Return XmlDatiImprese


    End Function


    '##########################################################################################
    'crea tutto il blocco dell'impresa (impresa, indirizzi, codici, gerarchia, ecc)
    'se si deve inserire un'impresa sola, conviene chiamare XML_2_Imprese che crea il nodo raccoglitore, prima di chiamare questa funzione
    'altrimenti, se si devono inserire più imprese, questa funzione può essere chiamata tante volte quanti sono le imprese da inserire
    Public Function XML_2_Impresa(ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByVal BaseCode As Integer, _
                                    ByVal TopCode As Integer, _
                                    ByVal TipoOperazioneDB_Impresa As enum_TipoOperazioneDB, _
                                    ByVal TipoOperazioneDB_Contatto As enum_TipoOperazioneDB, _
                                    ByVal Piva As String, _
                                    ByVal Rag_Soc As String, _
                                    ByVal PivaPadre As String, _
                                    ByVal PivaContatto As String, _
                                    ByVal TipoImpresaGerarchia As Integer, _
                                    ByVal Tipo_Indirizzo As Integer, _
                                    ByVal Pro_Cod_Istat As String, _
                                    ByVal Com_Cod_Istat As String, _
                                    ByVal Cod_Indirizzo As Integer, _
                                    ByVal Ind_Des As String, _
                                    ByVal Frz_Des As String, _
                                    ByVal CAP As String, _
                                    ByVal Stato As String, _
                                    ByVal Note As String, _
                                    ByVal Dt_Codici As DataTable, _
                                    ByVal DT_RisUm As DataTable, _
                                    ByVal Dt_ContattiCodici As DataTable, _
                                    Optional ByVal Delega As String = "", _
                                    Optional ByVal AT_Prevalente As String = "", _
                                    Optional ByVal Forma_Giuridica As String = "", _
                                    Optional ByVal Forma_Conduzione As String = "", _
                                    Optional ByVal Sup_Totale As Decimal = 0, _
                                    Optional ByVal Note_Impresa As String = "", _
                                    Optional ByVal Sa_Cod As Integer = 0, _
                                    Optional ByVal Id_CF As Integer = 0, _
                                    Optional ByVal Convenevoli As String = "", _
                                    Optional ByVal Codice_Fiscale As String = "", _
                                    Optional ByVal Tipo_Indirizzo_Default As Integer = 0, _
                                    Optional ByVal Cod_Contatto_Referente As String = "", _
                                    Optional ByVal Validita_Inizio_Impresa As Date = AGRODATAINIZIO, _
                                    Optional ByVal Validita_Fine_Impresa As Date = AGRODATAFINE _
                                    ) As XmlElement


        Dim XmlImpresa As System.Xml.XmlElement
        Dim XmlImpresaCodice As System.Xml.XmlElement
        Dim XmlIndirizzo As System.Xml.XmlElement
        Dim XmlDatiContatti As System.Xml.XmlElement


        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If



            '#######################################################
            '###################   IMPRESA    ######################
            '#######################################################

            XmlImpresa = XML_2_Impresa_Impresa(Log_Errori, _
                                                    XmlDoc, _
                                                    BaseCode, _
                                                    TopCode, _
                                                    TipoOperazioneDB_Impresa, _
                                                    Piva, _
                                                    Rag_Soc, _
                                                    PivaPadre, _
                                                    TipoImpresaGerarchia, _
                                                    Delega, _
                                                    AT_Prevalente, _
                                                    Forma_Giuridica, _
                                                    Forma_Conduzione, _
                                                    Sup_Totale, _
                                                    Note_Impresa, _
                                                    Validita_Inizio_Impresa, _
                                                    Validita_Fine_Impresa)



            '#######################################################
            '##################   INDIRIZZO    #####################
            '#######################################################

            XmlIndirizzo = XML_2_Indirizzo(TipoOperazioneDB_Impresa, _
                                            Tipo_Indirizzo, _
                                            Pro_Cod_Istat, _
                                            Com_Cod_Istat, _
                                            XmlDoc, _
                                            Cod_Indirizzo, _
                                            Ind_Des, _
                                            Frz_Des, _
                                            CAP, _
                                            Stato, _
                                            Note, _
                                            Validita_Inizio_Impresa, _
                                            Validita_Fine_Impresa, _
                                            BaseCode, _
                                            TopCode)

            XmlImpresa.AppendChild(XmlIndirizzo)


            '#######################################################
            '###############   IMPRESA CODICE    ###################
            '#######################################################

            'possono essere tanti nodo codice

            Dim i As Integer

            If Not IsNothing(Dt_Codici) AndAlso Dt_Codici.Rows.Count <> 0 Then

                Dim Id_Cod As String
                Dim Val_Cod As String
                Dim Validita_Inizio_Codice As Date
                Dim Validita_Fine_Codice As Date
                Dim TipoOperazioneDB_Codice As enum_TipoOperazioneDB

                For i = 0 To Dt_Codici.Rows.Count - 1

                    TipoOperazioneDB_Codice = Dt_Codici.Rows(i).Item("TipoOperazioneDB")

                    Id_Cod = Dt_Codici.Rows(i).Item("Id_Cod")
                    Val_Cod = Dt_Codici.Rows(i).Item("Val_Cod")
                    Validita_Inizio_Codice = Dt_Codici.Rows(i).Item("Validita_Inizio")
                    Validita_Fine_Codice = Dt_Codici.Rows(i).Item("Validita_Fine")

                    XmlImpresaCodice = XML_2_Codice(TipoOperazioneDB_Codice, _
                                                    Id_Cod, _
                                                    BaseCode, _
                                                    TopCode, _
                                                    XmlDoc, _
                                                    Val_Cod, _
                                                    Validita_Inizio_Codice, _
                                                    Validita_Fine_Codice)

                    XmlImpresa.AppendChild(XmlImpresaCodice)

                Next

            End If


            '#######################################################
            '################   DATI CONTATTI    ###################
            '#######################################################

            XmlDatiContatti = XML_2_Contatti(Log_Errori, _
                                            XmlDoc, _
                                            BaseCode, _
                                            TopCode, _
                                            True, _
                                            TipoOperazioneDB_Contatto, _
                                            PivaContatto, _
                                            Piva, _
                                            Nothing, _
                                            DT_RisUm, _
                                            Sa_Cod, _
                                            Id_CF, _
                                            Rag_Soc, _
                                            Convenevoli, _
                                            Codice_Fiscale, _
                                            Tipo_Indirizzo_Default, _
                                            , _
                                            , _
                                            , _
                                            , _
                                            Cod_Contatto_Referente, _
                                            Validita_Inizio_Impresa, _
                                            Validita_Fine_Impresa, _
                                            Nothing, _
                                            Dt_ContattiCodici)

            XmlImpresa.AppendChild(XmlDatiContatti)


        Catch ex As Exception

            Log_Errori += ex.Message

        End Try


        Return XmlImpresa


    End Function



    '##########################################################################################
    Public Function XML_2_Impresa_Impresa(ByRef Log_Errori As String, _
                                            ByRef XmlDoc As XmlDocument, _
                                            ByVal BaseCode As Integer, _
                                            ByVal TopCode As Integer, _
                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                            ByVal Piva As String, _
                                            ByVal Rag_Soc As String, _
                                            ByVal PivaPadre As String, _
                                            ByVal TipoImpresaGerarchia As Integer, _
                                            Optional ByVal Delega As String = "", _
                                            Optional ByVal AT_Prevalente As String = "", _
                                            Optional ByVal Forma_Giuridica As String = "", _
                                            Optional ByVal Forma_Conduzione As String = "", _
                                            Optional ByVal Sup_Totale As Decimal = 0, _
                                            Optional ByVal Note As String = "", _
                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                            Optional ByVal Validita_Fine As Date = AGRODATAFINE, _
                                            Optional ByVal Data_Creazione As DateTime = Nothing, _
                                            Optional ByVal Data_Modifica As DateTime = Nothing, _
                                            Optional ByVal Username_Creazione As String = "", _
                                            Optional ByVal Username_Modifica As String = "", _
                                            Optional ByVal Validazione As String = "", _
                                            Optional ByVal Data_Validazione As String = "", _
                                            Optional ByVal UserName_Validazione As String = "", _
                                            Optional ByVal Blk_Flag As String = "", _
                                            Optional ByVal Blk_Inizio_Data As DateTime = Nothing, _
                                            Optional ByVal Blk_Inizio_Username As String = "", _
                                            Optional ByVal Blk_Inizio_Note As String = "", _
                                            Optional ByVal Blk_Fine_Data As DateTime = Nothing, _
                                            Optional ByVal Blk_Fine_Username As String = "", _
                                            Optional ByVal Blk_Fine_Note As String = "", _
                                            Optional ByVal codEsenzione As String = "", _
                                            Optional ByVal codOp As String = "", _
                                            Optional ByVal documento As String = "", _
                                            Optional ByVal dtDocumento As DateTime = Nothing, _
                                            Optional ByVal dtValidazione As DateTime = Nothing, _
                                            Optional ByVal esenzioneDescr As String = "", _
                                            Optional ByVal flagAltreSedi As String = "", _
                                            Optional ByVal flagValidato As String = "", _
                                            Optional ByVal idUtenteValidazione As String = "", _
                                            Optional ByVal opDescr As String = "", _
                                            Optional ByVal fonte As String = "", _
                                            Optional ByVal dt_Fonte As DateTime = Nothing, _
                                            Optional ByVal aziendaCessata As String = "", _
                                            Optional ByVal aziendaIscrittaCAA As String = "", _
                                            Optional ByVal aziendaPresente As String = "", _
                                            Optional ByVal aziendaValidata As String = "", _
                                            Optional ByVal dataIscrizioneCAA As DateTime = Nothing, _
                                            Optional ByVal dataValidazione As DateTime = Nothing, _
                                            Optional ByVal dataVariazioneAzienda As DateTime = Nothing, _
                                            Optional ByVal maxDataVariazioneIbanAzienda As DateTime = Nothing, _
                                            Optional ByVal maxDataVariazionePersoneAzienda As DateTime = Nothing, _
                                            Optional ByVal maxDataVariazionePossessiAzienda As DateTime = Nothing _
                                            ) As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Impresa")

        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
        aggiungiFiglio(NodoXml, XmlDoc, "piva", Piva)
        aggiungiFiglio(NodoXml, XmlDoc, "rag_soc", Rag_Soc)
        aggiungiFiglio(NodoXml, XmlDoc, "padre", CStr(PivaPadre))
        aggiungiFiglio(NodoXml, XmlDoc, "tipoimpresagerarchia", CStr(TipoImpresaGerarchia))
        aggiungiFiglio(NodoXml, XmlDoc, "delega", Delega)
        aggiungiFiglio(NodoXml, XmlDoc, "at_prevalente", AT_Prevalente)
        aggiungiFiglio(NodoXml, XmlDoc, "forma_giuridica", Forma_Giuridica)
        aggiungiFiglio(NodoXml, XmlDoc, "forma_conduzione", Forma_Conduzione)
        aggiungiFiglio(NodoXml, XmlDoc, "sup_totale", CStr(Sup_Totale))
        aggiungiFiglio(NodoXml, XmlDoc, "note", Note)
        aggiungiFiglio(NodoXml, XmlDoc, "validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "basecode", CStr(BaseCode))
        aggiungiFiglio(NodoXml, XmlDoc, "topcode", CStr(TopCode))
        aggiungiFiglio(NodoXml, XmlDoc, "Data_Creazione", CStr(Data_Creazione))
        aggiungiFiglio(NodoXml, XmlDoc, "Data_Modifica", CStr(Data_Modifica))
        aggiungiFiglio(NodoXml, XmlDoc, "Username_Creazione", CStr(Username_Creazione))
        aggiungiFiglio(NodoXml, XmlDoc, "Username_Modifica", CStr(Username_Modifica))
        aggiungiFiglio(NodoXml, XmlDoc, "Validazione", CStr(Validazione))
        aggiungiFiglio(NodoXml, XmlDoc, "Data_Validazione", CStr(Data_Validazione))
        aggiungiFiglio(NodoXml, XmlDoc, "UserName_Validazione", CStr(UserName_Validazione))
        aggiungiFiglio(NodoXml, XmlDoc, "Blk_Flag", CStr(Blk_Flag))
        aggiungiFiglio(NodoXml, XmlDoc, "Blk_Inizio_Data", CStr(Blk_Inizio_Data))
        aggiungiFiglio(NodoXml, XmlDoc, "Blk_Inizio_Username", CStr(Blk_Inizio_Username))
        aggiungiFiglio(NodoXml, XmlDoc, "Blk_Inizio_Note", CStr(Blk_Inizio_Note))
        aggiungiFiglio(NodoXml, XmlDoc, "Blk_Fine_Data", CStr(Blk_Fine_Data))
        aggiungiFiglio(NodoXml, XmlDoc, "Blk_Fine_Username", CStr(Blk_Fine_Username))
        aggiungiFiglio(NodoXml, XmlDoc, "Blk_Fine_Note", CStr(Blk_Fine_Note))
        aggiungiFiglio(NodoXml, XmlDoc, "codEsenzione", CStr(codEsenzione))
        aggiungiFiglio(NodoXml, XmlDoc, "codOp", CStr(codOp))
        aggiungiFiglio(NodoXml, XmlDoc, "documento", CStr(documento))
        aggiungiFiglio(NodoXml, XmlDoc, "dtDocumento", CStr(dtDocumento))
        aggiungiFiglio(NodoXml, XmlDoc, "dtValidazione", CStr(dtValidazione))
        aggiungiFiglio(NodoXml, XmlDoc, "esenzioneDescr", CStr(esenzioneDescr))
        aggiungiFiglio(NodoXml, XmlDoc, "flagAltreSedi", CStr(flagAltreSedi))
        aggiungiFiglio(NodoXml, XmlDoc, "flagValidato", CStr(flagValidato))
        aggiungiFiglio(NodoXml, XmlDoc, "idUtenteValidazione", CStr(idUtenteValidazione))
        aggiungiFiglio(NodoXml, XmlDoc, "opDescr", CStr(opDescr))
        aggiungiFiglio(NodoXml, XmlDoc, "fonte", CStr(fonte))
        aggiungiFiglio(NodoXml, XmlDoc, "dt_Fonte", CStr(dt_Fonte))
        aggiungiFiglio(NodoXml, XmlDoc, "aziendaCessata", CStr(aziendaCessata))
        aggiungiFiglio(NodoXml, XmlDoc, "aziendaIscrittaCAA", CStr(aziendaIscrittaCAA))
        aggiungiFiglio(NodoXml, XmlDoc, "aziendaPresente", CStr(aziendaPresente))
        aggiungiFiglio(NodoXml, XmlDoc, "aziendaValidata", CStr(aziendaValidata))
        aggiungiFiglio(NodoXml, XmlDoc, "dataIscrizioneCAA", CStr(dataIscrizioneCAA))
        aggiungiFiglio(NodoXml, XmlDoc, "dataValidazione", CStr(dataValidazione))
        aggiungiFiglio(NodoXml, XmlDoc, "dataVariazioneAzienda", CStr(dataVariazioneAzienda))
        aggiungiFiglio(NodoXml, XmlDoc, "maxDataVariazioneIbanAzienda", CStr(maxDataVariazioneIbanAzienda))
        aggiungiFiglio(NodoXml, XmlDoc, "maxDataVariazionePersoneAzienda", CStr(maxDataVariazionePersoneAzienda))
        aggiungiFiglio(NodoXml, XmlDoc, "maxDataVariazionePossessiAzienda", CStr(maxDataVariazionePossessiAzienda))
        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function


    '##########################################################################################
    Public Function XML_2_Codice(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                    ByVal Id_Cod As Integer, _
                                    ByVal BaseCode As Integer, _
                                    ByVal TopCode As Integer, _
                                    Optional ByRef XmlDoc As XmlDocument = Nothing, _
                                    Optional ByVal Val_Cod As String = "", _
                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE) _
                                    As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Codice")

        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("id_cod"), CStr(Id_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("val_cod"), CStr(Val_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("basecode"), CStr(BaseCode))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("topcode"), CStr(TopCode))

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function


    '##########################################################################################
    'Viene chiamata quando si deve creare l'xml di un solo centro
    'crea il nodo DatiCentriAziendali
    'e poi chiama la funzione XML_2_CentroAziendale che crea tutto il blocco del centro (centro, indirizzo, codici, rubrica, ecc)
    Public Function XML_2_CentriAziendali(ByRef Log_Errori As String, _
                                            ByRef XmlDoc As XmlDocument, _
                                            ByVal BaseCode As Integer, _
                                            ByVal TopCode As Integer, _
                                            ByVal TipoOperazioneDB_CentroAziendale As enum_TipoOperazioneDB, _
                                            ByVal Piva As String, _
                                            ByVal Sa_Cod As Integer, _
                                            ByVal Sa_Nome As String, _
                                            ByVal Tipo_Indirizzo As Integer, _
                                            ByVal Pro_Cod_Istat As String, _
                                            ByVal Com_Cod_Istat As String, _
                                            ByVal Cod_Indirizzo As Integer, _
                                            ByVal Ind_Des As String, _
                                            ByVal Frz_Des As String, _
                                            ByVal CAP As String, _
                                            ByVal Stato As String, _
                                            ByVal Note As String, _
                                            ByVal Dt_Codici As DataTable, _
                                            ByVal DT_Rubrica As DataTable, _
                                            Optional ByVal X As Decimal = 0, _
                                            Optional ByVal Y As Decimal = 0, _
                                            Optional ByVal ZSLM As Decimal = 0, _
                                            Optional ByVal Longitudine As Decimal = 0, _
                                            Optional ByVal Latitudine As Decimal = 0, _
                                            Optional ByVal Area As Decimal = 0, _
                                            Optional ByVal CA_Sipi As String = "", _
                                            Optional ByVal AT_Prevalente As String = "", _
                                            Optional ByVal Forma_Possesso As String = "", _
                                            Optional ByVal TitoloPossesso As Integer = 0, _
                                            Optional ByVal Cod_TipoCentro As Integer = enum_TipoCentro.Sede_Legale, _
                                            Optional ByVal Sup_Totale As Decimal = 0, _
                                            Optional ByVal Sup_Bosco As Decimal = 0, _
                                            Optional ByVal Sup_Tare As Decimal = 0, _
                                            Optional ByVal Sup_SAU As Decimal = 0, _
                                            Optional ByVal Sup_Prati As Decimal = 0, _
                                            Optional ByVal Sup_SAU_Convenzionale As Decimal = 0, _
                                            Optional ByVal Sup_SAU_Conversione As Decimal = 0, _
                                            Optional ByVal Sup_SAU_Biologico As Decimal = 0, _
                                            Optional ByVal Validita_Inizio_Centro As Date = AGRODATAINIZIO, _
                                            Optional ByVal Validita_Fine_Centro As Date = AGRODATAFINE) As XmlElement


        Dim XmlDatiCentriAziendali As System.Xml.XmlElement
        Dim XmlCentroAziendale As System.Xml.XmlElement


        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If



            '#######################################################
            '###########   DATI CENTRI AZIENDALI    ################
            '#######################################################

            XmlDatiCentriAziendali = XmlDoc.CreateElement("DatiCentriAziendali")

            XmlDoc.AppendChild(XmlDatiCentriAziendali)


            '#######################################################
            '################   CENTRO AZIENDALE    ################
            '#######################################################

            XmlCentroAziendale = XML_2_CentroAziendale(Log_Errori, _
                                                        XmlDoc, _
                                                        BaseCode, _
                                                        TopCode, _
                                                        TipoOperazioneDB_CentroAziendale, _
                                                        Piva, _
                                                        Sa_Cod, _
                                                        Sa_Nome, _
                                                        Tipo_Indirizzo, _
                                                        Pro_Cod_Istat, _
                                                        Com_Cod_Istat, _
                                                        Cod_Indirizzo, _
                                                        Ind_Des, _
                                                        Frz_Des, _
                                                        CAP, _
                                                        Stato, _
                                                        Note, _
                                                        Dt_Codici, _
                                                        DT_Rubrica, _
                                                        X, _
                                                        Y, _
                                                        ZSLM, _
                                                        Longitudine, _
                                                        Latitudine, _
                                                        Area, _
                                                        CA_Sipi, _
                                                        AT_Prevalente, _
                                                        Forma_Possesso, _
                                                        TitoloPossesso, _
                                                        Cod_TipoCentro, _
                                                        Sup_Totale, _
                                                        Sup_Bosco, _
                                                        Sup_Tare, _
                                                        Sup_SAU, _
                                                        Sup_Prati, _
                                                        Sup_SAU_Convenzionale, _
                                                        Sup_SAU_Conversione, _
                                                        Sup_SAU_Biologico, _
                                                        Validita_Inizio_Centro, _
                                                        Validita_Fine_Centro)


            XmlDatiCentriAziendali.AppendChild(XmlCentroAziendale)



        Catch ex As Exception

            Log_Errori += ex.Message

        End Try


        Return XmlDatiCentriAziendali


    End Function


    '##########################################################################################
    'crea tutto il blocco del centro aziendale (centro, indirizzo, codici, rubrica, ecc)
    'se si deve inserire un centro solo, conviene chiamare XML_2_CentriAziendali che crea il nodo raccoglitore, prima di chiamare questa funzione
    'altrimenti, se si devono inserire più centri, questa funzione può essere chiamata tante volte quanti sono i centri da inserire
    Public Function XML_2_CentroAziendale(ByRef Log_Errori As String, _
                                            ByRef XmlDoc As XmlDocument, _
                                            ByVal BaseCode As Integer, _
                                            ByVal TopCode As Integer, _
                                            ByVal TipoOperazioneDB_CentroAziendale As enum_TipoOperazioneDB, _
                                            ByVal Piva As String, _
                                            ByVal Sa_Cod As Integer, _
                                            ByVal Sa_Nome As String, _
                                            ByVal Tipo_Indirizzo As Integer, _
                                            ByVal Pro_Cod_Istat As String, _
                                            ByVal Com_Cod_Istat As String, _
                                            ByVal Cod_Indirizzo As Integer, _
                                            ByVal Ind_Des As String, _
                                            ByVal Frz_Des As String, _
                                            ByVal CAP As String, _
                                            ByVal Stato As String, _
                                            ByVal Note As String, _
                                            ByVal Dt_Codici As DataTable, _
                                            ByVal DT_Rubrica As DataTable, _
                                            Optional ByVal X As Decimal = 0, _
                                            Optional ByVal Y As Decimal = 0, _
                                            Optional ByVal ZSLM As Decimal = 0, _
                                            Optional ByVal Longitudine As Decimal = 0, _
                                            Optional ByVal Latitudine As Decimal = 0, _
                                            Optional ByVal Area As Decimal = 0, _
                                            Optional ByVal CA_Sipi As String = "", _
                                            Optional ByVal AT_Prevalente As String = "", _
                                            Optional ByVal Forma_Possesso As String = "", _
                                            Optional ByVal TitoloPossesso As Integer = 0, _
                                            Optional ByVal Cod_TipoCentro As Integer = 0, _
                                            Optional ByVal Sup_Totale As Decimal = 0, _
                                            Optional ByVal Sup_Bosco As Decimal = 0, _
                                            Optional ByVal Sup_Tare As Decimal = 0, _
                                            Optional ByVal Sup_SAU As Decimal = 0, _
                                            Optional ByVal Sup_Prati As Decimal = 0, _
                                            Optional ByVal Sup_SAU_Convenzionale As Decimal = 0, _
                                            Optional ByVal Sup_SAU_Conversione As Decimal = 0, _
                                            Optional ByVal Sup_SAU_Biologico As Decimal = 0, _
                                            Optional ByVal Validita_Inizio_Centro As Date = AGRODATAINIZIO, _
                                            Optional ByVal Validita_Fine_Centro As Date = AGRODATAFINE) As XmlElement


        Dim XmlCentroAziendale As System.Xml.XmlElement
        Dim XmlCentroCodice As System.Xml.XmlElement
        Dim XmlIndirizzo As System.Xml.XmlElement
        Dim XmlRubrica As System.Xml.XmlElement
        Dim i As Integer

        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If



            '#######################################################
            '###############   CENTRO AZIENDALE    #################
            '#######################################################

            XmlCentroAziendale = XML_2_CentroAziendale_CentroAziendale(Log_Errori, _
                                                                        XmlDoc, _
                                                                        BaseCode, _
                                                                        TopCode, _
                                                                        TipoOperazioneDB_CentroAziendale, _
                                                                        Piva, _
                                                                        Sa_Cod, _
                                                                        Sa_Nome, _
                                                                        X, _
                                                                        Y, _
                                                                        ZSLM, _
                                                                        Longitudine, _
                                                                        Latitudine, _
                                                                        Area, _
                                                                        CA_Sipi, _
                                                                        AT_Prevalente, _
                                                                        Forma_Possesso, _
                                                                        TitoloPossesso, _
                                                                        Cod_TipoCentro, _
                                                                        Sup_Totale, _
                                                                        Sup_Bosco, _
                                                                        Sup_Tare, _
                                                                        Sup_SAU, _
                                                                        Sup_Prati, _
                                                                        Sup_SAU_Convenzionale, _
                                                                        Sup_SAU_Conversione, _
                                                                        Sup_SAU_Biologico, _
                                                                        Validita_Inizio_Centro, _
                                                                        Validita_Fine_Centro)



            '#######################################################
            '##################   INDIRIZZO    #####################
            '#######################################################

            XmlIndirizzo = XML_2_Indirizzo(TipoOperazioneDB_CentroAziendale, _
                                            Tipo_Indirizzo, _
                                            Pro_Cod_Istat, _
                                            Com_Cod_Istat, _
                                            XmlDoc, _
                                            Cod_Indirizzo, _
                                            Ind_Des, _
                                            Frz_Des, _
                                            CAP, _
                                            Stato, _
                                            Note, _
                                            Validita_Inizio_Centro, _
                                            Validita_Fine_Centro, _
                                            BaseCode, _
                                            TopCode)

            XmlCentroAziendale.AppendChild(XmlIndirizzo)


            '#######################################################
            '###################   RUBRICA    ######################
            '#######################################################

            'ci possono essere tanti nodi rubrica

            If Not IsNothing(DT_Rubrica) AndAlso DT_Rubrica.Rows.Count <> 0 Then

                Dim TipoOperazioneDB_Rubrica As Integer
                Dim Cod_Rubrica As Integer
                Dim Numero As String
                Dim Descrizione As String
                Dim Validita_Inizio_Rubrica As Date
                Dim Validita_Fine_Rubrica As Date

                For i = 0 To DT_Rubrica.Rows.Count - 1

                    TipoOperazioneDB_Rubrica = DT_Rubrica.Rows(i).Item("TipoOperazioneDB")

                    Cod_Rubrica = DT_Rubrica.Rows(i).Item("Cod_Rubrica")
                    Numero = DT_Rubrica.Rows(i).Item("Numero")
                    Descrizione = DT_Rubrica.Rows(i).Item("Descrizione")

                    Validita_Inizio_Rubrica = DT_Rubrica.Rows(i).Item("Validita_Inizio")
                    Validita_Fine_Rubrica = DT_Rubrica.Rows(i).Item("Validita_Fine")

                    XmlRubrica = XML_2_Rubrica(TipoOperazioneDB_Rubrica, _
                                               Cod_Rubrica, _
                                                Numero, _
                                                Descrizione, _
                                                XmlDoc, _
                                                Validita_Inizio_Rubrica, _
                                                Validita_Fine_Rubrica, _
                                                BaseCode, _
                                                TopCode)

                    XmlCentroAziendale.AppendChild(XmlRubrica)

                Next

            End If


            '#######################################################
            '################   CENTRO CODICE    ###################
            '#######################################################

            'possono essere tanti nodo codice

            If Not IsNothing(Dt_Codici) AndAlso Dt_Codici.Rows.Count <> 0 Then

                Dim Id_Cod As String
                Dim Val_Cod As String
                Dim Validita_Inizio_Codice As Date
                Dim Validita_Fine_Codice As Date
                Dim TipoOperazioneDB_Codice As enum_TipoOperazioneDB

                For i = 0 To Dt_Codici.Rows.Count - 1

                    TipoOperazioneDB_Codice = Dt_Codici.Rows(i).Item("TipoOperazioneDB")

                    Id_Cod = Dt_Codici.Rows(i).Item("Id_Cod")
                    Val_Cod = Dt_Codici.Rows(i).Item("Val_Cod")
                    Validita_Inizio_Codice = Dt_Codici.Rows(i).Item("Validita_Inizio")
                    Validita_Fine_Codice = Dt_Codici.Rows(i).Item("Validita_Fine")

                    XmlCentroCodice = XML_2_Codice(TipoOperazioneDB_Codice, _
                                                    Id_Cod, _
                                                    BaseCode, _
                                                    TopCode, _
                                                    XmlDoc, _
                                                    Val_Cod, _
                                                    Validita_Inizio_Codice, _
                                                    Validita_Fine_Codice)

                    XmlCentroAziendale.AppendChild(XmlCentroCodice)

                Next

            End If


        Catch ex As Exception

            Log_Errori += ex.Message

        End Try


        Return XmlCentroAziendale


    End Function



    '##########################################################################################
    Public Function XML_2_CentroAziendale_CentroAziendale(ByRef Log_Errori As String, _
                                                            ByRef XmlDoc As XmlDocument, _
                                                            ByVal BaseCode As Integer, _
                                                            ByVal TopCode As Integer, _
                                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                            ByVal Piva As String, _
                                                            ByVal Sa_Cod As Integer, _
                                                            ByVal Sa_Nome As String, _
                                                            Optional ByVal X As Decimal = 0, _
                                                            Optional ByVal Y As Decimal = 0, _
                                                            Optional ByVal ZSLM As Decimal = 0, _
                                                            Optional ByVal Longitudine As Decimal = 0, _
                                                            Optional ByVal Latitudine As Decimal = 0, _
                                                            Optional ByVal Area As Decimal = 0, _
                                                            Optional ByVal CA_Sipi As String = "", _
                                                            Optional ByVal AT_Prevalente As String = "", _
                                                            Optional ByVal Forma_Possesso As String = "", _
                                                            Optional ByVal TitoloPossesso As Integer = 0, _
                                                            Optional ByVal Cod_TipoCentro As Integer = 0, _
                                                            Optional ByVal Sup_Totale As Decimal = 0, _
                                                            Optional ByVal Sup_Bosco As Decimal = 0, _
                                                            Optional ByVal Sup_Tare As Decimal = 0, _
                                                            Optional ByVal Sup_SAU As Decimal = 0, _
                                                            Optional ByVal Sup_Prati As Decimal = 0, _
                                                            Optional ByVal Sup_SAU_Convenzionale As Decimal = 0, _
                                                            Optional ByVal Sup_SAU_Conversione As Decimal = 0, _
                                                            Optional ByVal Sup_SAU_Biologico As Decimal = 0, _
                                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                            Optional ByVal Validita_Fine As Date = AGRODATAFINE, _
                                                            Optional ByVal inviato As String = "", _
                                                            Optional ByVal dataInvio As String = "", _
                                                            Optional ByVal Data_Creazione As String = "", _
                                                            Optional ByVal Data_Modifica As String = "", _
                                                            Optional ByVal Username_Creazione As String = "", _
                                                            Optional ByVal Username_Modifica As String = "", _
                                                            Optional ByVal Validazione As String = "", _
                                                            Optional ByVal Data_Validazione As String = "", _
                                                            Optional ByVal UserName_Validazione As String = "", _
                                                            Optional ByVal Documento As String = "", _
                                                            Optional ByVal dataDocumento As String = "", _
                                                            Optional ByVal flagLegale As String = "", _
                                                            Optional ByVal flagPrincipale As String = "", _
                                                            Optional ByVal fonte As String = "", _
                                                            Optional ByVal fonteDescr As String = "", _
                                                            Optional ByVal dataFonte As String = "" _
                                                            ) As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("CentroAziendale")

        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
        aggiungiFiglio(NodoXml, XmlDoc, "piva", Piva)
        aggiungiFiglio(NodoXml, XmlDoc, "sa_cod", CStr(Sa_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, "sa_nome", Sa_Nome)

        aggiungiFiglio(NodoXml, XmlDoc, "x", CStr(X))
        aggiungiFiglio(NodoXml, XmlDoc, "y", CStr(Y))
        aggiungiFiglio(NodoXml, XmlDoc, "zslm", CStr(ZSLM))
        aggiungiFiglio(NodoXml, XmlDoc, "long", CStr(Longitudine))
        aggiungiFiglio(NodoXml, XmlDoc, "lat", CStr(Latitudine))
        aggiungiFiglio(NodoXml, XmlDoc, "area", CStr(Area))
        aggiungiFiglio(NodoXml, XmlDoc, "ca_sipi", CA_Sipi)
        aggiungiFiglio(NodoXml, XmlDoc, "at_prevalente", AT_Prevalente)
        aggiungiFiglio(NodoXml, XmlDoc, "forma_possesso", Forma_Possesso)
        aggiungiFiglio(NodoXml, XmlDoc, "titolopossesso", CStr(TitoloPossesso))
        aggiungiFiglio(NodoXml, XmlDoc, "tipo", CStr(Cod_TipoCentro))
        aggiungiFiglio(NodoXml, XmlDoc, "sup_totale", CStr(Sup_Totale))
        aggiungiFiglio(NodoXml, XmlDoc, "sup_bosco", CStr(Sup_Bosco))
        aggiungiFiglio(NodoXml, XmlDoc, "sup_tare", CStr(Sup_Tare))
        aggiungiFiglio(NodoXml, XmlDoc, "sup_sau", CStr(Sup_SAU))
        aggiungiFiglio(NodoXml, XmlDoc, "sup_prati", CStr(Sup_Prati))
        aggiungiFiglio(NodoXml, XmlDoc, "sup_sau_convenzionale", CStr(Sup_SAU_Convenzionale))
        aggiungiFiglio(NodoXml, XmlDoc, "sup_sau_conversione", CStr(Sup_SAU_Conversione))
        aggiungiFiglio(NodoXml, XmlDoc, "sup_sau_biologico", CStr(Sup_SAU_Biologico))
        aggiungiFiglio(NodoXml, XmlDoc, "validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "basecode", CStr(BaseCode))
        aggiungiFiglio(NodoXml, XmlDoc, "topcode", CStr(TopCode))
        aggiungiFiglio(NodoXml, XmlDoc, "inviato", CStr(inviato))
        aggiungiFiglio(NodoXml, XmlDoc, "dataInvio", CStr(dataInvio))
        aggiungiFiglio(NodoXml, XmlDoc, "Data_Creazione", CStr(Data_Creazione))
        aggiungiFiglio(NodoXml, XmlDoc, "Data_Modifica", CStr(Data_Modifica))
        aggiungiFiglio(NodoXml, XmlDoc, "Username_Creazione", CStr(Username_Creazione))
        aggiungiFiglio(NodoXml, XmlDoc, "Username_Modifica", CStr(Username_Modifica))
        aggiungiFiglio(NodoXml, XmlDoc, "Validazione", CStr(Validazione))
        aggiungiFiglio(NodoXml, XmlDoc, "Data_Validazione", CStr(Data_Validazione))
        aggiungiFiglio(NodoXml, XmlDoc, "UserName_Validazione", CStr(UserName_Validazione))
        aggiungiFiglio(NodoXml, XmlDoc, "Documento", CStr(Documento))
        aggiungiFiglio(NodoXml, XmlDoc, "dataDocumento", CStr(dataDocumento))
        aggiungiFiglio(NodoXml, XmlDoc, "flagLegale", CStr(flagLegale))
        aggiungiFiglio(NodoXml, XmlDoc, "flagPrincipale", CStr(flagPrincipale))
        aggiungiFiglio(NodoXml, XmlDoc, "fonte", CStr(fonte))
        aggiungiFiglio(NodoXml, XmlDoc, "fonteDescr", CStr(fonteDescr))
        aggiungiFiglio(NodoXml, XmlDoc, "dataFonte", CStr(dataFonte))

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function



    '##########################################################################################
    'Viene chiamata quando si deve creare l'xml di un solo fabbricato
    'crea il nodo DatiFabbricati
    'e poi chiama la funzione XML_2_Fabbricato che crea tutto il blocco del fabbricato (fabbricato, indirizzo, codici, ecc)
    Public Function XML_2_Fabbricati(ByRef Log_Errori As String, _
                                        ByRef XmlDoc As XmlDocument, _
                                        ByVal BaseCode As Integer, _
                                        ByVal TopCode As Integer, _
                                        ByVal TipoOperazioneDB_Fabbricato As enum_TipoOperazioneDB, _
                                        ByVal Piva As String, _
                                        ByVal Sa_Cod As Integer, _
                                        ByVal Fabbricato_Cod As Integer, _
                                        ByVal Fabbricato_Des As String, _
                                        ByVal Tipo_Fabbricato_Cod As Integer, _
                                        ByVal Tipo_Indirizzo As Integer, _
                                        ByVal Pro_Cod_Istat As String, _
                                        ByVal Com_Cod_Istat As String, _
                                        ByVal Cod_Indirizzo As Integer, _
                                        ByVal Ind_Des As String, _
                                        ByVal Frz_Des As String, _
                                        ByVal CAP As String, _
                                        ByVal Stato As String, _
                                        ByVal Note As String, _
                                        ByVal DT_Codici As DataTable, _
                                        ByVal DT_Stalla As DataTable, _
                                        ByVal DT_StallaCaratteristiche As DataTable, _
                                        ByVal DT_FabbricatoImpostazioni As DataTable, _
                                        ByVal DT_Vasca As DataTable, _
                                        Optional ByVal Prov As String = "000", _
                                        Optional ByVal Com As String = "000", _
                                        Optional ByVal Sezione As String = "", _
                                        Optional ByVal Foglio As Integer = 0, _
                                        Optional ByVal Numero As Integer = 0, _
                                        Optional ByVal Subalterno As String = "", _
                                        Optional ByVal MC_Convenzionale As Decimal = 0, _
                                        Optional ByVal MC_Conversione As Decimal = 0, _
                                        Optional ByVal MC_Biologico As Decimal = 0, _
                                        Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno, _
                                        Optional ByVal TitoloPossesso As Integer = 0, _
                                        Optional ByVal Conversione_Inizio As Date = AGRODATAINIZIO, _
                                        Optional ByVal Conversione_Fine As Date = AGRODATAINIZIO, _
                                        Optional ByVal Idoneo_Costruzione As Integer = 0, _
                                        Optional ByVal Idoneo_SeparazAmbienti As Integer = 0, _
                                        Optional ByVal Idoneo_SeparazProdotti As Integer = 0, _
                                        Optional ByVal Idoneo_CondIgieniche As Integer = 0, _
                                        Optional ByVal Idoneo_AutorizSanitaria As Integer = 0, _
                                        Optional ByVal Idoneo_HACCP As Integer = 0, _
                                        Optional ByVal Idoneo_Planimetria As Integer = 0, _
                                        Optional ByVal Idoneo_LayOut As Integer = 0, _
                                        Optional ByVal Idoneo_DiagrammiFlusso As Integer = 0, _
                                        Optional ByVal Idoneo_CDX_M004 As Integer = 0, _
                                        Optional ByVal Idoneo_SupMinCoperte As Integer = 0, _
                                        Optional ByVal Idoneo_SupMinScoperte As Integer = 0, _
                                        Optional ByVal Validita_Inizio_Fabbricato As Date = AGRODATAINIZIO, _
                                        Optional ByVal Validita_Fine_Fabbricato As Date = AGRODATAFINE, _
                                        Optional ByVal MQ_Convenzionale As Decimal = 0, _
                                        Optional ByVal MQ_Conversione As Decimal = 0, _
                                        Optional ByVal MQ_Biologico As Decimal = 0, _
                                        Optional ByVal MQ_Convenzionale_Scoperto As Decimal = 0, _
                                        Optional ByVal MQ_Conversione_Scoperto As Decimal = 0, _
                                        Optional ByVal MQ_Biologico_Scoperto As Decimal = 0, _
                                        Optional ByVal N_Piani As Integer = 0, _
                                        Optional ByVal Sup_Piano As Decimal = 0, _
                                        Optional ByVal Num_Autorizzazione As String = "", _
                                        Optional ByVal Data_Richiesta_Autorizzazione As Date = AGRODATAINIZIO, _
                                        Optional ByVal Tipologia_Utilizzo As Integer = 0 _
                                        ) As XmlElement


        Dim XmlDatiFabbricati As System.Xml.XmlElement
        Dim XmlFabbricato As System.Xml.XmlElement


        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If



            '#######################################################
            '###############   DATI FABBRICATI    ##################
            '#######################################################

            XmlDatiFabbricati = XmlDoc.CreateElement("DatiFabbricati")

            XmlDoc.AppendChild(XmlDatiFabbricati)


            '#######################################################
            '###################   FABBRICATO    ###################
            '#######################################################

            XmlFabbricato = XML_2_Fabbricato(Log_Errori, _
                                                XmlDoc, _
                                                BaseCode, _
                                                TopCode, _
                                                TipoOperazioneDB_Fabbricato, _
                                                Piva, _
                                                Sa_Cod, _
                                                Fabbricato_Cod, _
                                                Fabbricato_Des, _
                                                Tipo_Fabbricato_Cod, _
                                                Tipo_Indirizzo, _
                                                Pro_Cod_Istat, _
                                                Com_Cod_Istat, _
                                                Cod_Indirizzo, _
                                                Ind_Des, _
                                                Frz_Des, _
                                                CAP, _
                                                Stato, _
                                                Note, _
                                                DT_Codici, _
                                                DT_Stalla, _
                                                DT_StallaCaratteristiche, _
                                                DT_FabbricatoImpostazioni, _
                                                DT_Vasca, _
                                                Prov, _
                                                Com, _
                                                Sezione, _
                                                Foglio, _
                                                Numero, _
                                                Subalterno, _
                                                MC_Convenzionale, _
                                                MC_Conversione, _
                                                MC_Biologico, _
                                                Regolamento_Cod, _
                                                TitoloPossesso, _
                                                Conversione_Inizio, _
                                                Conversione_Fine, _
                                                Idoneo_Costruzione, _
                                                Idoneo_SeparazAmbienti, _
                                                Idoneo_SeparazProdotti, _
                                                Idoneo_CondIgieniche, _
                                                Idoneo_AutorizSanitaria, _
                                                Idoneo_HACCP, _
                                                Idoneo_Planimetria, _
                                                Idoneo_LayOut, _
                                                Idoneo_DiagrammiFlusso, _
                                                Idoneo_CDX_M004, _
                                                Idoneo_SupMinCoperte, _
                                                Idoneo_SupMinScoperte, _
                                                Validita_Inizio_Fabbricato, _
                                                Validita_Fine_Fabbricato, _
                                                MQ_Convenzionale, _
                                                MQ_Conversione, _
                                                MQ_Biologico, _
                                                MQ_Convenzionale_Scoperto, _
                                                MQ_Conversione_Scoperto, _
                                                MQ_Biologico_Scoperto, _
                                                N_Piani, _
                                                Sup_Piano, _
                                                Num_Autorizzazione, _
                                                Data_Richiesta_Autorizzazione, _
                                                Tipologia_Utilizzo)


            XmlDatiFabbricati.AppendChild(XmlFabbricato)



        Catch ex As Exception

            Log_Errori += ex.Message

        End Try


        Return XmlDatiFabbricati


    End Function


    '##########################################################################################
    'crea tutto il blocco del fabbricato (fabbricato, indirizzo, codici, ecc)
    'se si deve inserire un fabbricato solo, conviene chiamare XML_2_Fabbricati che crea il nodo raccoglitore, prima di chiamare questa funzione
    'altrimenti, se si devono inserire più fabbricati, questa funzione può essere chiamata tante volte quanti sono i fabbricati da inserire
    Public Function XML_2_Fabbricato(ByRef Log_Errori As String, _
                                        ByRef XmlDoc As XmlDocument, _
                                        ByVal BaseCode As Integer, _
                                        ByVal TopCode As Integer, _
                                        ByVal TipoOperazioneDB_Fabbricato As enum_TipoOperazioneDB, _
                                        ByVal Piva As String, _
                                        ByVal Sa_Cod As Integer, _
                                        ByVal Fabbricato_Cod As Integer, _
                                        ByVal Fabbricato_Des As String, _
                                        ByVal Tipo_Fabbricato_Cod As Integer, _
                                        ByVal Tipo_Indirizzo As Integer, _
                                        ByVal Pro_Cod_Istat As String, _
                                        ByVal Com_Cod_Istat As String, _
                                        ByVal Cod_Indirizzo As Integer, _
                                        ByVal Ind_Des As String, _
                                        ByVal Frz_Des As String, _
                                        ByVal CAP As String, _
                                        ByVal Stato As String, _
                                        ByVal Note As String, _
                                        ByVal DT_Codici As DataTable, _
                                        ByVal DT_Stalla As DataTable, _
                                        ByVal DT_StallaCaratteristiche As DataTable, _
                                        ByVal DT_FabbricatoImpostazioni As DataTable, _
                                        ByVal DT_Vasca As DataTable, _
                                        Optional ByVal Prov As String = "000", _
                                        Optional ByVal Com As String = "000", _
                                        Optional ByVal Sezione As String = "", _
                                        Optional ByVal Foglio As Integer = 0, _
                                        Optional ByVal Numero As Integer = 0, _
                                        Optional ByVal Subalterno As String = "", _
                                        Optional ByVal MC_Convenzionale As Decimal = 0, _
                                        Optional ByVal MC_Conversione As Decimal = 0, _
                                        Optional ByVal MC_Biologico As Decimal = 0, _
                                        Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno, _
                                        Optional ByVal TitoloPossesso As Integer = 0, _
                                        Optional ByVal Conversione_Inizio As Date = AGRODATAINIZIO, _
                                        Optional ByVal Conversione_Fine As Date = AGRODATAINIZIO, _
                                        Optional ByVal Idoneo_Costruzione As Integer = 0, _
                                        Optional ByVal Idoneo_SeparazAmbienti As Integer = 0, _
                                        Optional ByVal Idoneo_SeparazProdotti As Integer = 0, _
                                        Optional ByVal Idoneo_CondIgieniche As Integer = 0, _
                                        Optional ByVal Idoneo_AutorizSanitaria As Integer = 0, _
                                        Optional ByVal Idoneo_HACCP As Integer = 0, _
                                        Optional ByVal Idoneo_Planimetria As Integer = 0, _
                                        Optional ByVal Idoneo_LayOut As Integer = 0, _
                                        Optional ByVal Idoneo_DiagrammiFlusso As Integer = 0, _
                                        Optional ByVal Idoneo_CDX_M004 As Integer = 0, _
                                        Optional ByVal Idoneo_SupMinCoperte As Integer = 0, _
                                        Optional ByVal Idoneo_SupMinScoperte As Integer = 0, _
                                        Optional ByVal Validita_Inizio_Fabbricato As Date = AGRODATAINIZIO, _
                                        Optional ByVal Validita_Fine_Fabbricato As Date = AGRODATAFINE, _
                                        Optional ByVal MQ_Convenzionale As Decimal = 0, _
                                        Optional ByVal MQ_Conversione As Decimal = 0, _
                                        Optional ByVal MQ_Biologico As Decimal = 0, _
                                        Optional ByVal MQ_Convenzionale_Scoperto As Decimal = 0, _
                                        Optional ByVal MQ_Conversione_Scoperto As Decimal = 0, _
                                        Optional ByVal MQ_Biologico_Scoperto As Decimal = 0, _
                                        Optional ByVal N_Piani As Integer = 0, _
                                        Optional ByVal Sup_Piano As Decimal = 0, _
                                        Optional ByVal Num_Autorizzazione As String = "", _
                                        Optional ByVal Data_Richiesta_Autorizzazione As Date = AGRODATAINIZIO, _
                                        Optional ByVal Tipologia_Utilizzo As Integer = 0 _
                                            ) As XmlElement


        Dim XmlFabbricato As System.Xml.XmlElement
        Dim XmlFabbricatoCodice As System.Xml.XmlElement
        Dim XmlIndirizzo As System.Xml.XmlElement
        Dim XmlStalla As System.Xml.XmlElement
        Dim i As Integer

        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If


            '#######################################################
            '#################   FABBRICATO    #####################
            '#######################################################

            XmlFabbricato = XML_2_Fabbricato_Fabbricato( _
                                                Log_Errori, _
                                                XmlDoc, _
                                                BaseCode, _
                                                TopCode, _
                                                TipoOperazioneDB_Fabbricato, _
                                                Piva, _
                                                Sa_Cod, _
                                                Fabbricato_Cod, _
                                                Fabbricato_Des, _
                                                Cod_Indirizzo, _
                                                Tipo_Fabbricato_Cod, _
                                                Prov, _
                                                Com, _
                                                Sezione, _
                                                Foglio, _
                                                Numero, _
                                                Subalterno, _
                                                MC_Convenzionale, _
                                                MC_Conversione, _
                                                MC_Biologico, _
                                                Regolamento_Cod, _
                                                TitoloPossesso, _
                                                Conversione_Inizio, _
                                                Conversione_Fine, _
                                                Idoneo_Costruzione, _
                                                Idoneo_SeparazAmbienti, _
                                                Idoneo_SeparazProdotti, _
                                                Idoneo_CondIgieniche, _
                                                Idoneo_AutorizSanitaria, _
                                                Idoneo_HACCP, _
                                                Idoneo_Planimetria, _
                                                Idoneo_LayOut, _
                                                Idoneo_DiagrammiFlusso, _
                                                Idoneo_CDX_M004, _
                                                Idoneo_SupMinCoperte, _
                                                Idoneo_SupMinScoperte, _
                                                Validita_Inizio_Fabbricato, _
                                                Validita_Fine_Fabbricato, _
                                                MQ_Convenzionale, _
                                                MQ_Conversione, _
                                                MQ_Biologico, _
                                                MQ_Convenzionale_Scoperto, _
                                                MQ_Conversione_Scoperto, _
                                                MQ_Biologico_Scoperto, _
                                                N_Piani, _
                                                Sup_Piano, _
                                                Num_Autorizzazione, _
                                                Data_Richiesta_Autorizzazione, _
                                                Tipologia_Utilizzo)




            '#######################################################
            '##################   INDIRIZZO    #####################
            '#######################################################

            XmlIndirizzo = XML_2_Indirizzo(TipoOperazioneDB_Fabbricato, _
                                            Tipo_Indirizzo, _
                                            Pro_Cod_Istat, _
                                            Com_Cod_Istat, _
                                            XmlDoc, _
                                            Cod_Indirizzo, _
                                            Ind_Des, _
                                            Frz_Des, _
                                            CAP, _
                                            Stato, _
                                            Note, _
                                            Validita_Inizio_Fabbricato, _
                                            Validita_Fine_Fabbricato, _
                                            BaseCode, _
                                            TopCode)

            XmlFabbricato.AppendChild(XmlIndirizzo)


            '#######################################################
            '##############   FABBRICATO CODICE    #################
            '#######################################################

            'possono essere tanti nodo codice

            If Not IsNothing(DT_Codici) AndAlso DT_Codici.Rows.Count <> 0 Then

                Dim Id_Cod As String
                Dim Val_Cod As String
                Dim Validita_Inizio_Codice As Date
                Dim Validita_Fine_Codice As Date
                Dim TipoOperazioneDB_Codice As enum_TipoOperazioneDB

                For i = 0 To DT_Codici.Rows.Count - 1

                    TipoOperazioneDB_Codice = DT_Codici.Rows(i).Item("TipoOperazioneDB")

                    Id_Cod = DT_Codici.Rows(i).Item("Id_Cod")
                    Val_Cod = DT_Codici.Rows(i).Item("Val_Cod")
                    Validita_Inizio_Codice = DT_Codici.Rows(i).Item("Validita_Inizio")
                    Validita_Fine_Codice = DT_Codici.Rows(i).Item("Validita_Fine")

                    XmlFabbricatoCodice = XML_2_Fabbricato_Codice(TipoOperazioneDB_Codice, _
                                                                    Piva, _
                                                                    Sa_Cod, _
                                                                    Fabbricato_Cod, _
                                                                    Id_Cod, _
                                                                    BaseCode, _
                                                                    TopCode, _
                                                                    XmlDoc, _
                                                                    Val_Cod, _
                                                                    Validita_Inizio_Codice, _
                                                                    Validita_Fine_Codice)

                    XmlFabbricato.AppendChild(XmlFabbricatoCodice)

                Next

            End If



            '#######################################################
            '#############   FABBRICATO  IMPOSTAZIONI  #############
            '#######################################################

            '
            '
            '

            '#######################################################
            '####################   STALLA    ######################
            '#######################################################

            If Not IsNothing(DT_Stalla) AndAlso DT_Stalla.Rows.Count <> 0 Then

                For i = 0 To DT_Stalla.Rows.Count - 1

                    With DT_Stalla.Rows(i)

                        XmlStalla = XML_2_Stalla( _
                                                .Item("TipoOperazioneDB"), _
                                                .Item("Piva"), _
                                                .Item("Sa_Cod"), _
                                                .Item("STA_NUM"), _
                                                .Item("Sta_Des"), _
                                                .Item("Ausl_Cod"), _
                                                .Item("Dat_Costr"), _
                                                .Item("Dat_Chiu"), _
                                                .Item("Cod_Fabb"), _
                                                .Item("Gen_Cod"), _
                                                .Item("Spe_Cod"), _
                                                .Item("Ipro_Cod"), _
                                                .Item("X"), _
                                                .Item("Y"), _
                                                .Item("Dat_Ult_Agg"), _
                                                .Item("Latitudine"), _
                                                .Item("Longitudine"), _
                                                .Item("CUAA_Proprietario"), _
                                                .Item("Denominazione_Proprietario"), _
                                                .Item("CUAA_Detentore"), _
                                                .Item("Denominazione_Detentore"), _
                                                XmlDoc, _
                                                .Item("Validita_Inizio"), _
                                                .Item("Validita_Fine"))

                    End With

                    XmlFabbricato.AppendChild(XmlStalla)

                Next

            End If


            '#######################################################
            '#############   CARATTERISTICHE  STALLA    ############
            '#######################################################

            '
            '
            '

            '#######################################################
            '#################   VASCA ENOLOGICA    ################
            '#######################################################

            '
            '
            '



        Catch ex As Exception

            Log_Errori += ex.Message

        End Try


        Return XmlFabbricato


    End Function


    '##########################################################################################
    Public Function XML_2_Fabbricato_Codice(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                            ByVal Piva As String, _
                                            ByVal Sa_Cod As Integer, _
                                            ByVal Fabbricato_Cod As Integer, _
                                            ByVal Id_Cod As Integer, _
                                            ByVal BaseCode As Integer, _
                                            ByVal TopCode As Integer, _
                                            Optional ByRef XmlDoc As XmlDocument = Nothing, _
                                            Optional ByVal Val_Cod As String = "", _
                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                            Optional ByVal Validita_Fine As Date = AGRODATAFINE) _
                                            As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("CodiceFabbricato")

        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("piva"), Piva)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("sa_cod"), CStr(Sa_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("fabbricato_cod"), CStr(Fabbricato_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("id_cod"), CStr(Id_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("val_cod"), CStr(Val_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("basecode"), CStr(BaseCode))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("topcode"), CStr(TopCode))

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function

    '##########################################################################################
    Public Function XML_2_Fabbricato_Fabbricato(ByRef Log_Errori As String, _
                                                    ByRef XmlDoc As XmlDocument, _
                                                    ByVal BaseCode As Integer, _
                                                    ByVal TopCode As Integer, _
                                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                    ByVal Piva As String, _
                                                    ByVal Sa_Cod As Integer, _
                                                    ByVal Fabbricato_Cod As Integer, _
                                                    ByVal Fabbricato_Des As String, _
                                                    ByVal Indirizzo_Cod As Integer, _
                                                    ByVal Tipo_Fabbricato_Cod As Integer, _
                                                    Optional ByVal Prov As String = "000", _
                                                    Optional ByVal Com As String = "000", _
                                                    Optional ByVal Sezione As String = "", _
                                                    Optional ByVal Foglio As Integer = 0, _
                                                    Optional ByVal Numero As Integer = 0, _
                                                    Optional ByVal Subalterno As String = "", _
                                                    Optional ByVal MC_Convenzionale As Decimal = 0, _
                                                    Optional ByVal MC_Conversione As Decimal = 0, _
                                                    Optional ByVal MC_Biologico As Decimal = 0, _
                                                    Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno, _
                                                    Optional ByVal TitoloPossesso As Integer = 0, _
                                                    Optional ByVal Conversione_Inizio As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Conversione_Fine As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Idoneo_Costruzione As Integer = 0, _
                                                    Optional ByVal Idoneo_SeparazAmbienti As Integer = 0, _
                                                    Optional ByVal Idoneo_SeparazProdotti As Integer = 0, _
                                                    Optional ByVal Idoneo_CondIgieniche As Integer = 0, _
                                                    Optional ByVal Idoneo_AutorizSanitaria As Integer = 0, _
                                                    Optional ByVal Idoneo_HACCP As Integer = 0, _
                                                    Optional ByVal Idoneo_Planimetria As Integer = 0, _
                                                    Optional ByVal Idoneo_LayOut As Integer = 0, _
                                                    Optional ByVal Idoneo_DiagrammiFlusso As Integer = 0, _
                                                    Optional ByVal Idoneo_CDX_M004 As Integer = 0, _
                                                    Optional ByVal Idoneo_SupMinCoperte As Integer = 0, _
                                                    Optional ByVal Idoneo_SupMinScoperte As Integer = 0, _
                                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE, _
                                                    Optional ByVal MQ_Convenzionale As Decimal = 0, _
                                                    Optional ByVal MQ_Conversione As Decimal = 0, _
                                                    Optional ByVal MQ_Biologico As Decimal = 0, _
                                                    Optional ByVal MQ_Convenzionale_Scoperto As Decimal = 0, _
                                                    Optional ByVal MQ_Conversione_Scoperto As Decimal = 0, _
                                                    Optional ByVal MQ_Biologico_Scoperto As Decimal = 0, _
                                                    Optional ByVal N_Piani As Integer = 0, _
                                                    Optional ByVal Sup_Piano As Decimal = 0, _
                                                    Optional ByVal Num_Autorizzazione As String = "", _
                                                    Optional ByVal Data_Richiesta_Autorizzazione As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Tipologia_Utilizzo As Integer = 0 _
                                                    ) As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Fabbricato")

        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
        aggiungiFiglio(NodoXml, XmlDoc, "piva", Piva)
        aggiungiFiglio(NodoXml, XmlDoc, "sa_cod", CStr(Sa_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, "fabbricato_cod", CStr(Fabbricato_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, "fabbricato_des", Fabbricato_Des)

        aggiungiFiglio(NodoXml, XmlDoc, "indirizzo_cod", CStr(Indirizzo_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, "tipo_fabbricato_cod", CStr(Tipo_Fabbricato_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, "prov", CStr(Prov))
        aggiungiFiglio(NodoXml, XmlDoc, "com", CStr(Com))
        aggiungiFiglio(NodoXml, XmlDoc, "sezione", CStr(Sezione))
        aggiungiFiglio(NodoXml, XmlDoc, "foglio", CStr(Foglio))
        aggiungiFiglio(NodoXml, XmlDoc, "numero", CStr(Numero))
        aggiungiFiglio(NodoXml, XmlDoc, "subalterno", Subalterno)

        aggiungiFiglio(NodoXml, XmlDoc, "mc_convenzionale", CStr(MC_Convenzionale))
        aggiungiFiglio(NodoXml, XmlDoc, "mc_conversione", CStr(MC_Conversione))
        aggiungiFiglio(NodoXml, XmlDoc, "mc_biologico", CStr(MC_Biologico))
        aggiungiFiglio(NodoXml, XmlDoc, "regolamento_cod", CStr(Regolamento_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, "titolopossesso", CStr(TitoloPossesso))
        aggiungiFiglio(NodoXml, XmlDoc, "conversione_inizio", Format(Conversione_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "conversione_fine", Format(Conversione_Fine, "dd/MM/yyyy"))

        aggiungiFiglio(NodoXml, XmlDoc, "idoneo_costruzione", CStr(Idoneo_Costruzione))
        aggiungiFiglio(NodoXml, XmlDoc, "idoneo_separazambienti", CStr(Idoneo_SeparazAmbienti))
        aggiungiFiglio(NodoXml, XmlDoc, "idoneo_separazprodotti", CStr(Idoneo_SeparazProdotti))
        aggiungiFiglio(NodoXml, XmlDoc, "idoneo_condigieniche", CStr(Idoneo_CondIgieniche))
        aggiungiFiglio(NodoXml, XmlDoc, "idoneo_autorizsanitaria", CStr(Idoneo_AutorizSanitaria))
        aggiungiFiglio(NodoXml, XmlDoc, "idoneo_haccp", CStr(Idoneo_HACCP))
        aggiungiFiglio(NodoXml, XmlDoc, "idoneo_planimetria", CStr(Idoneo_Planimetria))
        aggiungiFiglio(NodoXml, XmlDoc, "idoneo_layout", CStr(Idoneo_LayOut))
        aggiungiFiglio(NodoXml, XmlDoc, "idoneo_diagrammiflusso", CStr(Idoneo_DiagrammiFlusso))
        aggiungiFiglio(NodoXml, XmlDoc, "idoneo_cdx_m004", CStr(Idoneo_CDX_M004))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Idoneo_SupMinCoperte"), CStr(Idoneo_SupMinCoperte))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Idoneo_SupMinScoperte"), CStr(Idoneo_SupMinScoperte))

        aggiungiFiglio(NodoXml, XmlDoc, "mq_convenzionale", CStr(MQ_Convenzionale))
        aggiungiFiglio(NodoXml, XmlDoc, "mq_conversione", CStr(MQ_Conversione))
        aggiungiFiglio(NodoXml, XmlDoc, "mq_biologico", CStr(MQ_Biologico))
        aggiungiFiglio(NodoXml, XmlDoc, "mq_convenzionale_scoperto", CStr(MQ_Convenzionale_Scoperto))
        aggiungiFiglio(NodoXml, XmlDoc, "mq_conversione_scoperto", CStr(MQ_Conversione_Scoperto))
        aggiungiFiglio(NodoXml, XmlDoc, "mq_biologico_scoperto", CStr(MQ_Biologico_Scoperto))
        aggiungiFiglio(NodoXml, XmlDoc, "n_piani", CStr(N_Piani))
        aggiungiFiglio(NodoXml, XmlDoc, "sup_piano", CStr(Sup_Piano))

        aggiungiFiglio(NodoXml, XmlDoc, "num_autorizzazione", CStr(Num_Autorizzazione))
        aggiungiFiglio(NodoXml, XmlDoc, "data_richiesta_autorizzazione", Format(Data_Richiesta_Autorizzazione, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "tipologia_utilizzo", CStr(Tipologia_Utilizzo))

        aggiungiFiglio(NodoXml, XmlDoc, "validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "basecode", CStr(BaseCode))
        aggiungiFiglio(NodoXml, XmlDoc, "topcode", CStr(TopCode))

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function



    '##########################################################################################
    'Viene chiamata quando si deve creare l'xml di un solo contatto
    'crea il nodo DatiContatti
    'e poi chiama la funzione XML_2_Contatto che crea tutto il blocco del contatto (contatto, indirizzi, risorse umane, ecc)
    Public Function XML_2_Contatti(ByRef Log_Errori As String, _
                                        ByRef XmlDoc As XmlDocument, _
                                        ByVal BaseCode As Integer, _
                                        ByVal TopCode As Integer, _
                                        ByVal Flag_Contatto_is_ImpresaGIAS As Boolean, _
                                        ByVal TipoOperazioneDB_Contatto As enum_TipoOperazioneDB, _
                                        ByVal Piva As String, _
                                        ByVal Cod_Contatto As String, _
                                        ByVal DT_Indirizzi As DataTable, _
                                        ByVal DT_RisUm As DataTable, _
                                        Optional ByVal Sa_Cod As Integer = 0, _
                                        Optional ByVal Id_CF As Integer = 0, _
                                        Optional ByVal Rag_Soc As String = "", _
                                        Optional ByVal Convenevoli As String = "", _
                                        Optional ByVal Codice_Fiscale As String = "", _
                                        Optional ByVal Tipo_Indirizzo_Default As Integer = 0, _
                                        Optional ByVal Nome As String = "", _
                                        Optional ByVal Cognome As String = "", _
                                        Optional ByVal Data_Nascita As Date = AGRODATAINIZIO, _
                                        Optional ByVal Sesso As String = "", _
                                        Optional ByVal Cod_Contatto_Referente As String = "", _
                                        Optional ByVal Validita_Inizio_Contatto As Date = AGRODATAINIZIO, _
                                        Optional ByVal Validita_Fine_Contatto As Date = AGRODATAFINE, _
                                        Optional ByVal Dt_Rubrica As DataTable = Nothing, _
                                        Optional ByVal Dt_Codici As DataTable = Nothing, _
                                       Optional ByVal Cod_Risum_Destinazione_Diversa As Integer = 0, _
                                         Optional ByVal Tipo_Indirizzo_Default_Destinazione_Diversa As Integer = 0, _
                                         Optional ByVal Tipo_Speditore As Integer = 0, _
                                         Optional ByVal Tipo_Destinazione As Integer = 0, _
                                         Optional ByVal Agente_Cod As Integer = 0, _
                                         Optional ByVal Provvigione As Integer = 0, _
                                         Optional ByVal Note As String = "", _
                                         Optional ByVal Id_Gestione_Note As Integer = 0, _
                                         Optional ByVal Note2 As String = "", _
                                         Optional ByVal Note_Operazioni As String = "", _
                                         Optional ByVal Note2_Operazioni As String = "", _
                                         Optional ByVal Fido As Integer = 0, _
                                         Optional ByVal Limite_Posizioni As Integer = 0, _
                                         Optional ByVal Limite_Giorni_Evasione As Integer = 0, _
                                         Optional ByVal Orari_Ritiro As String = "", _
                                         Optional ByVal Filtro_Rimborsi As String = "", _
                                         Optional ByVal Vettore_Cod As Integer = 0, _
                                         Optional ByVal CapoArea_Cod As Integer = 0, _
                                         Optional ByVal Provvigione_CapoArea As Integer = 0, _
                                         Optional ByVal Documento As String = "", _
                                         Optional ByVal dtDocumento As DateTime = Nothing, _
                                         Optional ByVal dtFonte As DateTime = Nothing, _
                                         Optional ByVal flagReferente As String = "", _
                                         Optional ByVal fonte As String = "", _
                                         Optional ByVal fonteDescr As String = "" _
                                        ) As XmlElement


        Dim XmlDatiContatti As System.Xml.XmlElement
        Dim XmlContatto As System.Xml.XmlElement


        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If



            '#######################################################
            '################   DATI CONTATTI    ###################
            '#######################################################

            XmlDatiContatti = XmlDoc.CreateElement("DatiContatti")

            If Flag_Contatto_is_ImpresaGIAS = False Then
                XmlDoc.AppendChild(XmlDatiContatti)
            End If


            '#######################################################
            '###################   CONTATTO    #####################
            '#######################################################

            XmlContatto = XML_2_Contatto(Log_Errori, _
                                            XmlDoc, _
                                            BaseCode, _
                                            TopCode, _
                                            Flag_Contatto_is_ImpresaGIAS, _
                                            TipoOperazioneDB_Contatto, _
                                            Piva, _
                                            Cod_Contatto, _
                                            DT_Indirizzi, _
                                            DT_RisUm, _
                                            Sa_Cod, _
                                            Id_CF, _
                                            Rag_Soc, _
                                            Convenevoli, _
                                            Codice_Fiscale, _
                                            Tipo_Indirizzo_Default, _
                                            Nome, _
                                            Cognome, _
                                            Data_Nascita, _
                                            Sesso, _
                                            Cod_Contatto_Referente, _
                                            Validita_Inizio_Contatto, _
                                            Validita_Fine_Contatto, _
                                            Dt_Rubrica, _
                                            Dt_Codici, _
                                            Cod_Risum_Destinazione_Diversa, _
                                            Tipo_Indirizzo_Default_Destinazione_Diversa, _
                                            Tipo_Speditore, _
                                             Tipo_Destinazione, _
                                              Agente_Cod, _
                                             Provvigione, _
                                             Note, _
                                             Id_Gestione_Note, _
                                            Note2, _
                                             Note_Operazioni, _
                                             Note2_Operazioni, _
                                             Fido, _
                                            Limite_Posizioni, _
                                             Limite_Giorni_Evasione, _
                                             Orari_Ritiro, _
                                             Filtro_Rimborsi, _
                                             Vettore_Cod, _
                                             CapoArea_Cod, _
                                             Provvigione_CapoArea)

            XmlDatiContatti.AppendChild(XmlContatto)



        Catch ex As Exception

            Log_Errori += ex.Message

        End Try


        Return XmlDatiContatti


    End Function





    '##########################################################################################
    'crea tutto il blocco del contatto (contatto, indirizzi, risorse umane, ecc)
    'se si deve inserire un contatto solo, conviene chiamare XML_2_Contatti che crea il nodo raccoglitore, prima di chiamare questa funzione
    'altrimenti, se si devono inserire più contatti, questa funzione può essere chiamata tante volte quanti sono i contatti da inserire
    Public Function XML_2_Contatto(ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByVal BaseCode As Integer, _
                                    ByVal TopCode As Integer, _
                                    ByVal Flag_Contatto_is_ImpresaGIAS As Boolean, _
                                    ByVal TipoOperazioneDB_Contatto As enum_TipoOperazioneDB, _
                                    ByVal Piva As String, _
                                    ByVal Cod_Contatto As String, _
                                    ByVal DT_Indirizzi As DataTable, _
                                    ByVal DT_RisUm As DataTable, _
                                    Optional ByVal Sa_Cod As Integer = 0, _
                                    Optional ByVal Id_CF As Integer = 0, _
                                    Optional ByVal Rag_Soc As String = "", _
                                    Optional ByVal Convenevoli As String = "", _
                                    Optional ByVal Codice_Fiscale As String = "", _
                                    Optional ByVal Tipo_Indirizzo_Default As Integer = 0, _
                                    Optional ByVal Nome As String = "", _
                                    Optional ByVal Cognome As String = "", _
                                    Optional ByVal Data_Nascita As Date = AGRODATAINIZIO, _
                                    Optional ByVal Sesso As String = "", _
                                    Optional ByVal Cod_Contatto_Referente As String = "", _
                                    Optional ByVal Validita_Inizio_Contatto As Date = AGRODATAINIZIO, _
                                    Optional ByVal Validita_Fine_Contatto As Date = AGRODATAFINE, _
                                    Optional ByVal Dt_Rubrica As DataTable = Nothing, _
                                    Optional ByVal Dt_Codici As DataTable = Nothing, _
                                    Optional ByVal Cod_Risum_Destinazione_Diversa As Integer = 0, _
                                     Optional ByVal Tipo_Indirizzo_Default_Destinazione_Diversa As Integer = 0, _
                                     Optional ByVal Tipo_Speditore As Integer = 0, _
                                     Optional ByVal Tipo_Destinazione As Integer = 0, _
                                     Optional ByVal Agente_Cod As Integer = 0, _
                                     Optional ByVal Provvigione As Integer = 0, _
                                     Optional ByVal Note As String = "", _
                                     Optional ByVal Id_Gestione_Note As Integer = 0, _
                                     Optional ByVal Note2 As String = "", _
                                     Optional ByVal Note_Operazioni As String = "", _
                                     Optional ByVal Note2_Operazioni As String = "", _
                                     Optional ByVal Fido As Integer = 0, _
                                     Optional ByVal Limite_Posizioni As Integer = 0, _
                                     Optional ByVal Limite_Giorni_Evasione As Integer = 0, _
                                     Optional ByVal Orari_Ritiro As String = "", _
                                     Optional ByVal Filtro_Rimborsi As String = "", _
                                     Optional ByVal Vettore_Cod As Integer = 0, _
                                     Optional ByVal CapoArea_Cod As Integer = 0, _
                                     Optional ByVal Provvigione_CapoArea As Integer = 0, _
                                     Optional ByVal Documento As String = "", _
                                     Optional ByVal dtDocumento As DateTime = Nothing, _
                                     Optional ByVal dtFonte As DateTime = Nothing, _
                                     Optional ByVal flagReferente As String = "", _
                                     Optional ByVal fonte As String = "", _
                                     Optional ByVal fonteDescr As String = "" _
                                    ) As XmlElement


        'Dim XmlDatiContatti As System.Xml.XmlElement
        Dim XmlContatto As System.Xml.XmlElement
        Dim XmlContattoCodice As System.Xml.XmlElement
        Dim XmlIndirizzo As System.Xml.XmlElement
        Dim XmlRubrica As System.Xml.XmlElement
        Dim XmlDatiProdottiCosti As System.Xml.XmlElement
        Dim XmlContattoCosto As XmlElement
        Dim XmlDatiLiquidita As System.Xml.XmlElement
        Dim XmlDatiConti As System.Xml.XmlElement
        Dim XmlDatiParcoMacchine As System.Xml.XmlElement
        Dim XmlRisorseUmane As System.Xml.XmlElement

        Dim i As Integer

        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If


            '#######################################################
            '###################   CONTATTO    #####################
            '#######################################################

            XmlContatto = XML_2_Contatto_Contatto(TipoOperazioneDB_Contatto, _
                                                    Piva, _
                                                    Cod_Contatto, _
                                                    XmlDoc, _
                                                    Sa_Cod, _
                                                    Id_CF, _
                                                    Rag_Soc, _
                                                    Convenevoli, _
                                                    Codice_Fiscale, _
                                                    Tipo_Indirizzo_Default, _
                                                    Nome, _
                                                    Cognome, _
                                                    Data_Nascita, _
                                                    Sesso, _
                                                    Cod_Contatto_Referente, _
                                                    Validita_Inizio_Contatto, _
                                                    Validita_Fine_Contatto, _
                                                    BaseCode, _
                                                    TopCode, _
                                                    Cod_Risum_Destinazione_Diversa, _
                                                    Tipo_Indirizzo_Default_Destinazione_Diversa, _
                                                    Tipo_Speditore, _
                                                     Tipo_Destinazione, _
                                                      Agente_Cod, _
                                                     Provvigione, _
                                                     Note, _
                                                     Id_Gestione_Note, _
                                                    Note2, _
                                                     Note_Operazioni, _
                                                     Note2_Operazioni, _
                                                     Fido, _
                                                    Limite_Posizioni, _
                                                     Limite_Giorni_Evasione, _
                                                     Orari_Ritiro, _
                                                     Filtro_Rimborsi, _
                                                     Vettore_Cod, _
                                                     CapoArea_Cod, _
                                                     Provvigione_CapoArea)


            '#######################################################
            '##################   INDIRIZZO    #####################
            '#######################################################

            'ATTENZIONE! Gli indirizzi vanno inseriti solo per i contatti 
            'che non sono imprese GIAS (poichè questi hanno i record in ImpreseXIndirizzi)
            If Flag_Contatto_is_ImpresaGIAS = False Then

                'il contatto deve avere 4 indirizzi obbligatori
                '4 tipi di indirizzo per le persone fisiche
                '4 tipi di indirizzo per le persone giuridiche

                Dim Tipo_Indirizzo As Integer 'va specificato in base al tipo di persona
                Dim Pro_Cod_Istat As String
                Dim Com_Cod_Istat As String
                Dim Cod_Indirizzo As Integer
                Dim Ind_Des As String
                Dim Frz_Des As String
                Dim CAP As String
                Dim Stato As String
                Dim Note_Indirizzo As String
                Dim Validita_Inizio_Indirizzo As Date
                Dim Validita_Fine_Indirizzo As Date
                Dim TipoOperazioneDB_Indirizzo As enum_TipoOperazioneDB

                Dim Numero_Indirizzi_Fittizi As Integer


                If Not IsNothing(DT_Indirizzi) AndAlso DT_Indirizzi.Rows.Count <> 0 Then

                    'Numero_Indirizzi_Fittizi deve essere 0
                    'se ho inviato più di 4 indirizzi, i successivi li ignoro
                    If DT_Indirizzi.Rows.Count > 4 Then
                        Numero_Indirizzi_Fittizi = 0
                    Else
                        Numero_Indirizzi_Fittizi = 4 - DT_Indirizzi.Rows.Count
                    End If


                    For i = 0 To DT_Indirizzi.Rows.Count - 1

                        TipoOperazioneDB_Indirizzo = DT_Indirizzi.Rows(i).Item("TipoOperazioneDB")

                        Tipo_Indirizzo = DT_Indirizzi.Rows(i).Item("Tipo_Indirizzo")
                        Pro_Cod_Istat = DT_Indirizzi.Rows(i).Item("Pro_Cod_Istat")
                        Com_Cod_Istat = DT_Indirizzi.Rows(i).Item("Com_Cod_Istat")
                        Cod_Indirizzo = DT_Indirizzi.Rows(i).Item("Cod_Indirizzo")
                        Ind_Des = DT_Indirizzi.Rows(i).Item("Ind_Des")
                        Frz_Des = DT_Indirizzi.Rows(i).Item("Frz_Des")
                        CAP = DT_Indirizzi.Rows(i).Item("CAP")
                        Stato = DT_Indirizzi.Rows(i).Item("Stato")
                        Note_Indirizzo = DT_Indirizzi.Rows(i).Item("Note")
                        Validita_Inizio_Indirizzo = DT_Indirizzi.Rows(i).Item("Validita_Inizio")
                        Validita_Fine_Indirizzo = DT_Indirizzi.Rows(i).Item("Validita_Fine")

                        XmlIndirizzo = XML_2_Indirizzo(TipoOperazioneDB_Indirizzo, _
                                                        Tipo_Indirizzo, _
                                                        Pro_Cod_Istat, _
                                                        Com_Cod_Istat, _
                                                        XmlDoc, _
                                                        Cod_Indirizzo, _
                                                        Ind_Des, _
                                                        Frz_Des, _
                                                        CAP, _
                                                        Stato, _
                                                        Note_Indirizzo, _
                                                        Validita_Inizio_Indirizzo, _
                                                        Validita_Fine_Indirizzo, _
                                                        BaseCode, _
                                                        TopCode)

                        XmlContatto.AppendChild(XmlIndirizzo)

                    Next

                    If Numero_Indirizzi_Fittizi > 0 Then

                        'va gestito il corretto utilizzo del tipo indirizzo
                        'e del tipo operazione
                        TipoOperazioneDB_Indirizzo = enum_TipoOperazioneDB.Scrittura
                        Tipo_Indirizzo = 0
                        Pro_Cod_Istat = "000"
                        Com_Cod_Istat = "000"
                        Cod_Indirizzo = 0
                        Ind_Des = ""
                        Frz_Des = ""
                        CAP = ""
                        Stato = "ITALIA"
                        Note_Indirizzo = ""
                        Validita_Inizio_Indirizzo = AGRODATAINIZIO
                        Validita_Fine_Indirizzo = AGRODATAFINE


                        For i = 0 To Numero_Indirizzi_Fittizi - 1

                            'inserisco degli indirizzi fittizi

                            XmlIndirizzo = XML_2_Indirizzo(TipoOperazioneDB_Indirizzo, _
                                                     Tipo_Indirizzo, _
                                                     Pro_Cod_Istat, _
                                                     Com_Cod_Istat, _
                                                     XmlDoc, _
                                                     Cod_Indirizzo, _
                                                     Ind_Des, _
                                                     Frz_Des, _
                                                     CAP, _
                                                     Stato, _
                                                     Note_Indirizzo, _
                                                     Validita_Inizio_Indirizzo, _
                                                     Validita_Fine_Indirizzo, _
                                                     BaseCode, _
                                                     TopCode)

                            XmlContatto.AppendChild(XmlIndirizzo)

                        Next

                        Log_Errori += "Sono stati inseriti " + CStr(Numero_Indirizzi_Fittizi) + " indirizzi vuoti."

                    End If


                Else

                    For i = 1 To 4

                        'inserisco degli indirizzi fittizi

                        XmlIndirizzo = XML_2_Indirizzo(TipoOperazioneDB_Indirizzo, _
                                                   Tipo_Indirizzo, _
                                                   Pro_Cod_Istat, _
                                                   Com_Cod_Istat, _
                                                   XmlDoc, _
                                                   Cod_Indirizzo, _
                                                   Ind_Des, _
                                                   Frz_Des, _
                                                   CAP, _
                                                   Stato, _
                                                   Note_Indirizzo, _
                                                   Validita_Inizio_Indirizzo, _
                                                   Validita_Fine_Indirizzo, _
                                                   BaseCode, _
                                                   TopCode)

                        XmlContatto.AppendChild(XmlIndirizzo)

                    Next

                    Log_Errori += "Indirizzi non inviati. Sono stati inseriti 4 indirizzi vuoti."

                End If

            End If


            '#######################################################
            '###################   RUBRICA    ######################
            '#######################################################

            'ci possono essere tanti nodi rubrica

            If Not IsNothing(Dt_Rubrica) AndAlso Dt_Rubrica.Rows.Count <> 0 Then

                Dim Cod_Rubrica As Integer
                Dim Numero As String
                Dim Descrizione As String
                Dim Validita_Inizio_Rubrica As Date
                Dim Validita_Fine_Rubrica As Date
                Dim TipoOperazioneDB_Rubrica As enum_TipoOperazioneDB

                For i = 0 To Dt_Rubrica.Rows.Count - 1

                    TipoOperazioneDB_Rubrica = Dt_Rubrica.Rows(i).Item("TipoOperazioneDB")

                    Cod_Rubrica = Dt_Rubrica.Rows(i).Item("Cod_Rubrica")
                    Numero = Dt_Rubrica.Rows(i).Item("Numero")
                    Descrizione = Dt_Rubrica.Rows(i).Item("Descrizione")

                    Validita_Inizio_Rubrica = Dt_Rubrica.Rows(i).Item("Validita_Inizio")
                    Validita_Fine_Rubrica = Dt_Rubrica.Rows(i).Item("Validita_Fine")

                    XmlRubrica = XML_2_Rubrica(TipoOperazioneDB_Rubrica, _
                                               Cod_Rubrica, _
                                                Numero, _
                                                Descrizione, _
                                                XmlDoc, _
                                                Validita_Inizio_Rubrica, _
                                                Validita_Fine_Rubrica, _
                                                BaseCode, _
                                                TopCode)

                    XmlContatto.AppendChild(XmlRubrica)

                Next

            End If


            '#######################################################
            '##############   CONTATTO CODICE    ###################
            '#######################################################

            'possono essere tanti nodo contatto codice

            If Not IsNothing(Dt_Codici) AndAlso Dt_Codici.Rows.Count <> 0 Then

                Dim Id_Cod As String
                Dim Val_Cod As String
                Dim Validita_Inizio_Codice As Date
                Dim Validita_Fine_Codice As Date
                Dim TipoOperazioneDB_Codice As enum_TipoOperazioneDB

                For i = 0 To Dt_Codici.Rows.Count - 1

                    TipoOperazioneDB_Codice = Dt_Codici.Rows(i).Item("TipoOperazioneDB")
                    Id_Cod = Dt_Codici.Rows(i).Item("Id_Cod")
                    Val_Cod = Dt_Codici.Rows(i).Item("Val_Cod")
                    Validita_Inizio_Codice = Dt_Codici.Rows(i).Item("Validita_Inizio")
                    Validita_Fine_Codice = Dt_Codici.Rows(i).Item("Validita_Fine")


                    XmlContattoCodice = XML_2_Contatto_Codice(TipoOperazioneDB_Codice, _
                                                                Piva, _
                                                                Cod_Contatto, _
                                                                Id_Cod, _
                                                                BaseCode, _
                                                                TopCode, _
                                                                XmlDoc, _
                                                                Sa_Cod, _
                                                                Val_Cod, _
                                                                Validita_Inizio_Codice, _
                                                                Validita_Fine_Codice)

                    XmlContatto.AppendChild(XmlContattoCodice)

                Next

            End If



            '#######################################################
            '##################   LIQUIDITA    #####################
            '#######################################################

            XmlDatiLiquidita = XmlDoc.CreateElement("DatiLiquidita")

            XmlContatto.AppendChild(XmlDatiLiquidita)


            '#######################################################
            '###################    CONTI    #######################
            '#######################################################

            XmlDatiConti = XmlDoc.CreateElement("DatiConti")

            XmlContatto.AppendChild(XmlDatiConti)


            '#######################################################
            '###############    PARCO MACCHINE    ##################
            '#######################################################

            XmlDatiParcoMacchine = XmlDoc.CreateElement("DatiParcoMacchine")

            XmlContatto.AppendChild(XmlDatiParcoMacchine)


            '#######################################################
            '############### RISORSE UMANE    ######################
            '#######################################################

            'possono essere tanti nodo rapcon

            If Not IsNothing(DT_RisUm) AndAlso DT_RisUm.Rows.Count <> 0 Then

                Dim Cod_Risum As Integer
                Dim Cod_Rapporto As Integer
                Dim Settore_Des As String
                Dim Attivita_Des As String
                Dim Occasionale As Integer
                Dim Ore_Settimanali As Decimal
                Dim Giorni_Ferie As Integer
                Dim Ferie_Godute As Integer
                Dim Giorni_Malattia As Integer
                Dim Patentino As String
                Dim Data_Rilascio_Patentino As Date
                Dim Data_Scadenza_Patentino As Date
                Dim Ente_di_rilascio As String
                Dim ChkSpesometro As Integer
                Dim Saldo_Iniziale_Crediti As Decimal = 0
                Dim Saldo_Iniziale_Debiti As Decimal = 0
                Dim Cod_RisUm_Origine As Integer
                Dim Piva_SuperUser_Origine As String
                Dim Validita_Inizio_RisUm As Date
                Dim Validita_Fine_RisUm As Date
                Dim TipoOperazioneDB_RisUm As enum_TipoOperazioneDB
                Dim Dt_Prodotticosti As DataTable

                For i = 0 To DT_RisUm.Rows.Count - 1

                    TipoOperazioneDB_RisUm = DT_RisUm.Rows(i).Item("TipoOperazioneDB")

                    Cod_Risum = DT_RisUm.Rows(i).Item("Cod_Risum")
                    Cod_Rapporto = DT_RisUm.Rows(i).Item("Cod_Rapporto")
                    Settore_Des = DT_RisUm.Rows(i).Item("Settore_Des")
                    Attivita_Des = DT_RisUm.Rows(i).Item("Attivita_Des")
                    Occasionale = DT_RisUm.Rows(i).Item("Occasionale")
                    Ore_Settimanali = DT_RisUm.Rows(i).Item("Ore_Settimanali")
                    Giorni_Ferie = DT_RisUm.Rows(i).Item("Giorni_Ferie")
                    Ferie_Godute = DT_RisUm.Rows(i).Item("Ferie_Godute")
                    Giorni_Malattia = DT_RisUm.Rows(i).Item("Giorni_Malattia")
                    Patentino = DT_RisUm.Rows(i).Item("Patentino")
                    Data_Rilascio_Patentino = DT_RisUm.Rows(i).Item("Data_Rilascio_Patentino")
                    Data_Scadenza_Patentino = DT_RisUm.Rows(i).Item("Data_Scadenza_Patentino")
                    Ente_di_rilascio = DT_RisUm.Rows(i).Item("Ente_di_rilascio")
                    ChkSpesometro = DT_RisUm.Rows(i).Item("ChkSpesometro")
                    Saldo_Iniziale_Crediti = DT_RisUm.Rows(i).Item("Saldo_Iniziale_Crediti")
                    Saldo_Iniziale_Debiti = DT_RisUm.Rows(i).Item("Saldo_Iniziale_Debiti")

                    Cod_RisUm_Origine = DT_RisUm.Rows(i).Item("Cod_RisUm_Origine")
                    Piva_SuperUser_Origine = DT_RisUm.Rows(i).Item("Piva_SuperUser_Origine")
                    Validita_Inizio_RisUm = DT_RisUm.Rows(i).Item("Validita_Inizio")
                    Validita_Fine_RisUm = DT_RisUm.Rows(i).Item("Validita_Fine")
                    If IsDBNull(DT_RisUm.Rows(i).Item("DT_ProdottiCosti")) Then
                        Dt_Prodotticosti = Nothing
                    Else
                        Dt_Prodotticosti = DT_RisUm.Rows(i).Item("DT_ProdottiCosti")
                    End If


                    XmlRisorseUmane = XML_2_Contatto_RisorsaUmana(TipoOperazioneDB_RisUm, _
                                                                    Piva, _
                                                                    Cod_Contatto, _
                                                                    BaseCode, _
                                                                    TopCode, _
                                                                    XmlDoc, _
                                                                    Sa_Cod, _
                                                                    Cod_Risum, _
                                                                    Cod_Rapporto, _
                                                                    Settore_Des, _
                                                                    Attivita_Des, _
                                                                    Occasionale, _
                                                                    Ore_Settimanali, _
                                                                    Giorni_Ferie, _
                                                                    Ferie_Godute, _
                                                                    Giorni_Malattia, _
                                                                    Patentino, _
                                                                    Data_Rilascio_Patentino, _
                                                                    Data_Scadenza_Patentino, _
                                                                    Cod_RisUm_Origine, _
                                                                    Piva_SuperUser_Origine, _
                                                                    Validita_Inizio_RisUm, _
                                                                    Validita_Fine_RisUm, _
                                                                    Ente_di_rilascio, _
                                                                    ChkSpesometro, _
                                                                    Saldo_Iniziale_Crediti, _
                                                                    Saldo_Iniziale_Debiti)

                    XmlContatto.AppendChild(XmlRisorseUmane)



                    '#######################################################
                    '################   PRODOTTI COSTI    ##################
                    '#######################################################

                    XmlDatiProdottiCosti = XmlDoc.CreateElement("DatiProdotti_Costi")

                    If Not IsNothing(Dt_Prodotticosti) AndAlso Dt_Prodotticosti.Rows.Count <> 0 Then

                        Dim Validita_Inizio_PC As Date
                        Dim Validita_Fine_PC As Date
                        Dim TipoOperazioneDB_PC As enum_TipoOperazioneDB
                        Dim Id, Elem_Cod, Pro_Cod, Mat_Cod, Udm_Cod, Mezzo, Veg_Cod, Cul_Cod As Integer
                        Dim Prezzo_Unitario As Decimal
                        Dim Riferimento As String
                        Dim j As Integer

                        Dim InserisciPrezzo As Boolean = False

                        For j = 0 To Dt_Prodotticosti.Rows.Count - 1

                            InserisciPrezzo = False

                            TipoOperazioneDB_PC = Dt_Prodotticosti.Rows(j).Item("TipoOperazioneDB")
                            Id = Dt_Prodotticosti.Rows(j).Item("Id")
                            Elem_Cod = Dt_Prodotticosti.Rows(j).Item("Elem_Cod")
                            Pro_Cod = Dt_Prodotticosti.Rows(j).Item("Pro_Cod")
                            Mat_Cod = Dt_Prodotticosti.Rows(j).Item("Mat_Cod")
                            Udm_Cod = Dt_Prodotticosti.Rows(j).Item("Udm_Cod")
                            Mezzo = Dt_Prodotticosti.Rows(j).Item("Mezzo")
                            Veg_Cod = Dt_Prodotticosti.Rows(j).Item("Veg_Cod")
                            Cul_Cod = Dt_Prodotticosti.Rows(j).Item("Cul_Cod")
                            Prezzo_Unitario = Dt_Prodotticosti.Rows(j).Item("Prezzo_Unitario")
                            Riferimento = Dt_Prodotticosti.Rows(j).Item("Riferimento")
                            Piva = Dt_Prodotticosti.Rows(j).Item("piva")
                            Validita_Inizio_PC = Dt_Prodotticosti.Rows(j).Item("Validita_Inizio")
                            Validita_Fine_PC = Dt_Prodotticosti.Rows(j).Item("Validita_Fine")

                            'devo inserire i prezzi solo della risorsa umana corrente
                            If Mat_Cod = Cod_Risum Then
                                InserisciPrezzo = True
                            End If

                            If InserisciPrezzo = True Then
                                XmlContattoCosto = XML_ProdottiCosti(TipoOperazioneDB_PC, _
                                                                        Id, _
                                                                        Piva, _
                                                                        Riferimento, _
                                                                        Elem_Cod, _
                                                                        Pro_Cod, _
                                                                        Mat_Cod, _
                                                                        Udm_Cod, _
                                                                        Mezzo, _
                                                                        Prezzo_Unitario, _
                                                                        Veg_Cod, _
                                                                        Cul_Cod, _
                                                                        XmlDoc, _
                                                                        Validita_Inizio_PC, _
                                                                        Validita_Fine_PC)

                                XmlDatiProdottiCosti.AppendChild(XmlContattoCosto)
                            End If

                        Next

                    End If


                    XmlRisorseUmane.AppendChild(XmlDatiProdottiCosti)

                Next

            Else
                Log_Errori += "RisorseUmane non inviate."
            End If




        Catch ex As Exception

            Log_Errori += ex.Message

            'aggiunto in data 10/10/2014:
            'se succedeva un errore a livello di indirizzo,
            'veniva creato un xml monco, senza risorsa umana,
            'non va bene!
            'deve essere gestito l'errore, non mi bisogna importare un contatto sgaffo che poi non si vede
            XmlContatto = Nothing

        End Try


        Return XmlContatto


    End Function



    '##########################################################################################
    Public Function XML_2_Contatto_Contatto(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                            ByVal Piva As String, _
                                            ByVal Cod_Contatto As String, _
                                            Optional ByRef XmlDoc As XmlDocument = Nothing, _
                                            Optional ByVal Sa_Cod As Integer = 0, _
                                            Optional ByVal Id_CF As Integer = 0, _
                                            Optional ByVal Rag_Soc As String = "", _
                                            Optional ByVal Convenevoli As String = "", _
                                            Optional ByVal Codice_Fiscale As String = "", _
                                            Optional ByVal Tipo_Indirizzo_Default As Integer = 0, _
                                            Optional ByVal Nome As String = "", _
                                            Optional ByVal Cognome As String = "", _
                                            Optional ByVal Data_Nascita As Date = AGRODATAINIZIO, _
                                            Optional ByVal Sesso As String = "", _
                                            Optional ByVal Cod_Contatto_Referente As String = "", _
                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                            Optional ByVal Validita_Fine As Date = AGRODATAFINE, _
                                            Optional ByVal BaseCode As Integer = 0, _
                                            Optional ByVal TopCode As Integer = 200000000, _
                                            Optional ByVal Cod_Risum_Destinazione_Diversa As Integer = 0, _
                                            Optional ByVal Tipo_Indirizzo_Default_Destinazione_Diversa As Integer = 0, _
                                             Optional ByVal Tipo_Speditore As Integer = 0, _
                                             Optional ByVal Tipo_Destinazione As Integer = 0, _
                                             Optional ByVal Agente_Cod As Integer = 0, _
                                             Optional ByVal Provvigione As Double = 0, _
                                             Optional ByVal Note As String = "", _
                                             Optional ByVal Id_Gestione_Note As Integer = 0, _
                                             Optional ByVal Note2 As String = "", _
                                             Optional ByVal Note_Operazioni As String = "", _
                                             Optional ByVal Note2_Operazioni As String = "", _
                                             Optional ByVal Fido As Double = 0, _
                                             Optional ByVal Limite_Posizioni As Integer = 0, _
                                             Optional ByVal Limite_Giorni_Evasione As Double = 0, _
                                             Optional ByVal Orari_Ritiro As String = "", _
                                             Optional ByVal Filtro_Rimborsi As String = "", _
                                             Optional ByVal Vettore_Cod As Integer = 0, _
                                             Optional ByVal CapoArea_Cod As Integer = 0, _
                                             Optional ByVal Provvigione_CapoArea As Double = 0, _
                                             Optional ByVal Documento As String = "", _
                                            Optional ByVal dtDocumento As DateTime = Nothing, _
                                            Optional ByVal dtFonte As DateTime = Nothing, _
                                            Optional ByVal flagReferente As String = "", _
                                            Optional ByVal fonte As String = "", _
                                            Optional ByVal fonteDescr As String = "", _
                                            Optional ByVal AlboProfessionale As String = "", _
                                            Optional ByVal AlboProfessionaleDescr As String = "", _
                                            Optional ByVal numeroIscrizione As Nullable(Of Integer) = Nothing, _
                                            Optional ByVal Qualifica As String = "", _
                                            Optional ByVal QualificaDescr As String = "", _
                                            Optional ByVal TitoloStudio As String = "", _
                                            Optional ByVal TitoloStudioDescr As String = "" _
                                            ) As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Contatto")

        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("piva"), CStr(Piva))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("sa_cod"), CStr(Sa_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("cod_contatto"), CStr(Cod_Contatto))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("id_cf"), CStr(Id_CF))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("rag_soc"), CStr(Rag_Soc))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("convenevoli"), CStr(Convenevoli))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("codice_fiscale"), CStr(Codice_Fiscale))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Tipo_Indirizzo_Default"), CStr(Tipo_Indirizzo_Default))

        aggiungiFiglio(NodoXml, XmlDoc, LCase("Cod_Risum_Destinazione_Diversa"), CStr(Cod_Risum_Destinazione_Diversa))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Tipo_Indirizzo_Default_Destinazione_Diversa"), CStr(Tipo_Indirizzo_Default_Destinazione_Diversa))

        'nuovi attributi aggiunti il 02/11/2009
        aggiungiFiglio(NodoXml, XmlDoc, LCase("nome"), Nome)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("cognome"), Cognome)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("data_nascita"), CStr(Data_Nascita))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("sesso"), Sesso)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("cod_contatto_referente"), Cod_Contatto_Referente)

        'nuovi attributi aggiunti il 09/12/2015
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Tipo_Speditore"), Tipo_Speditore)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Tipo_Destinazione"), Tipo_Destinazione)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Agente_Cod"), Agente_Cod)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Provvigione"), Provvigione)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Note"), Note)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Id_Gestione_Note"), Id_Gestione_Note)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Note2"), Note2)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Note_Operazioni"), Note_Operazioni)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Note2_Operazioni"), Note2_Operazioni)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Fido"), Fido)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Limite_Posizioni"), Limite_Posizioni)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Limite_Giorni_Evasione"), Limite_Giorni_Evasione)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Orari_Ritiro"), Orari_Ritiro)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Filtro_Rimborsi"), Filtro_Rimborsi)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Vettore_Cod"), Vettore_Cod)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("CapoArea_Cod"), CapoArea_Cod)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Provvigione_CapoArea"), Provvigione_CapoArea)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("basecode"), CStr(BaseCode))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("topcode"), CStr(TopCode))


        aggiungiFiglio(NodoXml, XmlDoc, LCase("Documento"), CStr(Documento))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("dtDocumento"), CStr(dtDocumento))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("dtFonte"), CStr(dtFonte))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("flagReferente"), CStr(flagReferente))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("fonte"), CStr(fonte))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("fonteDescr"), CStr(fonteDescr))
        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function


    '##########################################################################################
    Public Function XML_2_Contatto_Codice(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                            ByVal Piva As String, _
                                            ByVal Cod_Contatto As String, _
                                            ByVal Id_Cod As Integer, _
                                            ByVal BaseCode As Integer, _
                                            ByVal TopCode As Integer, _
                                            Optional ByRef XmlDoc As XmlDocument = Nothing, _
                                            Optional ByVal Sa_Cod As Integer = 0, _
                                            Optional ByVal Val_Cod As String = "", _
                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                            Optional ByVal Validita_Fine As Date = AGRODATAFINE) _
                                            As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Contatto_Codice")

        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("piva"), CStr(Piva))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("sa_cod"), CStr(Sa_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("cod_contatto"), CStr(Cod_Contatto))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("id_cod"), CStr(Id_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("val_cod"), CStr(Val_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("basecode"), CStr(BaseCode))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("topcode"), CStr(TopCode))

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function


    '##########################################################################################
    Public Function XML_ProdottiCosti(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                        ByVal Id As Integer, _
                                        ByVal Piva As String, _
                                        ByVal Riferimento As String, _
                                        ByVal Elem_Cod As Integer, _
                                        ByVal Pro_Cod As Integer, _
                                        ByVal Mat_Cod As Integer, _
                                        ByVal Udm_Cod As Integer, _
                                        ByVal Mezzo As Integer, _
                                        ByVal Prezzo_Unitario As Decimal, _
                                        ByVal Veg_Cod As Integer, _
                                        ByVal Cul_Cod As Integer, _
                                        Optional ByRef XmlDoc As XmlDocument = Nothing, _
                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE) _
                                        As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Prodotto_Costo")

        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("id"), CStr(Id))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("piva"), CStr(Piva))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("riferimento"), CStr(Riferimento))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("elem_cod"), CStr(Elem_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("pro_cod"), CStr(Pro_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("mat_cod"), CStr(Mat_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("udm_cod"), CStr(Udm_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("mezzo"), CStr(Mezzo))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("prezzo_unitario"), CStr(Prezzo_Unitario))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("veg_cod"), CStr(Veg_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("cul_cod"), CStr(Cul_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))


        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function



    '##########################################################################################
    'i corrispettivi sono salvati nella tabella prodotti_costi
    Public Function XML_2_Contatto_RisorsaUmana(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                ByVal Piva As String, _
                                                ByVal Cod_Contatto As String, _
                                                ByVal BaseCode As Integer, _
                                                ByVal TopCode As Integer, _
                                                Optional ByRef XmlDoc As XmlDocument = Nothing, _
                                                Optional ByVal Sa_Cod As Integer = 0, _
                                                Optional ByVal Cod_Risum As Integer = 0, _
                                                Optional ByVal Cod_Rapporto As Integer = 0, _
                                                Optional ByVal Settore_Des As String = "", _
                                                Optional ByVal Attivita_Des As String = "", _
                                                Optional ByVal Occasionale As Integer = 0, _
                                                Optional ByVal Ore_Settimanali As Decimal = 0, _
                                                Optional ByVal Giorni_Ferie As Integer = 0, _
                                                Optional ByVal Ferie_Godute As Integer = 0, _
                                                Optional ByVal Giorni_Malattia As Integer = 0, _
                                                Optional ByVal Patentino As String = "", _
                                                Optional ByVal Data_Rilascio_Patentino As Date = AGRODATAINIZIO, _
                                                Optional ByVal Data_Scadenza_Patentino As Date = AGRODATAFINE, _
                                                Optional ByVal Cod_RisUm_Origine As Integer = 0, _
                                                Optional ByVal Piva_SuperUser_Origine As String = "", _
                                                Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                Optional ByVal Validita_Fine As Date = AGRODATAFINE, _
                                                Optional ByVal Ente_di_rilascio As String = "", _
                                                Optional ByVal ChkSpesometro As Integer = 0, _
                                                Optional ByVal Saldo_Iniziale_Crediti As Decimal = 0, _
                                                Optional ByVal Saldo_Iniziale_Debiti As Decimal = 0) _
                                                As System.Xml.XmlElement


        'Optional ByVal Corrispettivo_Mensile As Decimal = 0, _
        'Optional ByVal Corrispettivo_Orario As Decimal = 0, _


        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("RapCon")

        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("piva"), CStr(Piva))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("sa_cod"), CStr(Sa_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("cod_contatto"), CStr(Cod_Contatto))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("cod_risum"), CStr(Cod_Risum))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("cod_rapporto"), CStr(Cod_Rapporto))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("settore_des"), CStr(Settore_Des))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("attivita_des"), CStr(Attivita_Des))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("occasionale"), CStr(Occasionale))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("corrispettivo_mensile"), CStr(0))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("corrispettivo_orario"), CStr(0))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("ore_settimanali"), CStr(Ore_Settimanali))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("giorni_ferie"), CStr(Giorni_Ferie))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("ferie_godute"), CStr(Ferie_Godute))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("giorni_malattia"), CStr(Giorni_Malattia))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("patentino"), CStr(Patentino))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("data_rilascio_patentino"), Format(Data_Rilascio_Patentino, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("data_scadenza_patentino"), Format(Data_Scadenza_Patentino, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Ente_di_rilascio"), CStr(Ente_di_rilascio))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("ChkSpesometro"), CStr(ChkSpesometro))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Saldo_Iniziale_Crediti"), CStr(Saldo_Iniziale_Crediti))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Saldo_Iniziale_Debiti"), CStr(Saldo_Iniziale_Debiti))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("cod_risum_origine"), CStr(Cod_RisUm_Origine))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("piva_superuser_origine"), CStr(Piva_SuperUser_Origine))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("basecode"), CStr(BaseCode))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("topcode"), CStr(TopCode))


        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function



    '##########################################################################################
    'il cap, il comune e la provincia sono ridondanti
    'vengono letti nelle tabelle del metaschema (istat e lista_province)
    Public Function XML_2_Indirizzo(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                    ByVal Tipo_Indirizzo As Integer, _
                                    ByVal Pro_Cod_Istat As String, _
                                    ByVal Com_Cod_Istat As String, _
                                    Optional ByRef XmlDoc As XmlDocument = Nothing, _
                                    Optional ByVal Cod_Indirizzo As Integer = 0, _
                                    Optional ByVal Ind_Des As String = "", _
                                    Optional ByVal Frz_Des As String = "", _
                                    Optional ByVal CAP As String = "", _
                                    Optional ByVal Stato As String = "ITALIA", _
                                    Optional ByVal Note As String = "", _
                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE, _
                                    Optional ByVal BaseCode As Integer = 0, _
                                    Optional ByVal TopCode As Integer = 200000000) As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Indirizzo")

        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
        aggiungiFiglio(NodoXml, XmlDoc, "tipo_indirizzo", CStr(Tipo_Indirizzo))
        aggiungiFiglio(NodoXml, XmlDoc, "cod_indirizzo", CStr(Cod_Indirizzo))
        aggiungiFiglio(NodoXml, XmlDoc, "ind_des", Ind_Des)
        aggiungiFiglio(NodoXml, XmlDoc, "frz_des", Frz_Des)
        aggiungiFiglio(NodoXml, XmlDoc, "com_des", "")
        aggiungiFiglio(NodoXml, XmlDoc, "pro_cod", "")
        aggiungiFiglio(NodoXml, XmlDoc, "stato", Stato)
        aggiungiFiglio(NodoXml, XmlDoc, "note", Note)
        aggiungiFiglio(NodoXml, XmlDoc, "cap", CAP)
        aggiungiFiglio(NodoXml, XmlDoc, "pro_cod_istat", Pro_Cod_Istat)
        aggiungiFiglio(NodoXml, XmlDoc, "com_cod_istat", Com_Cod_Istat)
        aggiungiFiglio(NodoXml, XmlDoc, "validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "basecode", CStr(BaseCode))
        aggiungiFiglio(NodoXml, XmlDoc, "topcode", CStr(TopCode))


        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function



    '##########################################################################################
    Public Function XML_2_Rubrica(ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                    ByVal Cod_Rubrica As Long, _
                                    ByVal Numero As String, _
                                    ByVal Descrizione As String, _
                                    Optional ByRef XmlDoc As XmlDocument = Nothing, _
                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE, _
                                    Optional ByVal BaseCode As Integer = 0, _
                                    Optional ByVal TopCode As Integer = 200000000) As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Rubrica")

        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
        aggiungiFiglio(NodoXml, XmlDoc, "cod_rubrica", CStr(Cod_Rubrica))
        aggiungiFiglio(NodoXml, XmlDoc, "numero", Numero)
        aggiungiFiglio(NodoXml, XmlDoc, "descr", Descrizione)
        aggiungiFiglio(NodoXml, XmlDoc, "validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "basecode", CStr(BaseCode))
        aggiungiFiglio(NodoXml, XmlDoc, "topcode", CStr(TopCode))


        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function




    '##########################################################################################
    Public Function XML_2_Stalla( _
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal STA_NUM As Int32, _
                                ByVal Sta_Des As String, _
                                ByVal Ausl_Cod As String, _
                                ByVal Dat_Costr As Date, _
                                ByVal Dat_Chiu As Date, _
                                ByVal Cod_Fabb As String, _
                                ByVal Gen_Cod As Int32, _
                                ByVal Spe_Cod As Int32, _
                                ByVal Ipro_Cod As Int32, _
                                ByVal X As String, _
                                ByVal Y As String, _
                                ByVal Dat_Ult_Agg As Date, _
                                ByVal Latitudine As Int32, _
                                ByVal Longitudine As Int32, _
                                ByVal CUAA_Proprietario As String, _
                                ByVal Denominazione_Proprietario As String, _
                                ByVal CUAA_Detentore As String, _
                                ByVal Denominazione_Detentore As String, _
                                Optional ByRef XmlDoc As XmlDocument = Nothing, _
                                Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                Optional ByVal Validita_Fine As Date = AGRODATAFINE, _
                                Optional ByVal AziendaCodice As String = "", _
                                Optional ByVal allevamentoDescr As String = "", _
                                Optional ByVal codAllevamento As String = "", _
                                Optional ByVal speCodice As String = "", _
                                Optional ByVal ID_Utente As String = "", _
                                Optional ByVal DT_Variazione As DateTime = Nothing, _
                                Optional ByVal flag_Libri_Gen As String = "", _
                                Optional ByVal Autorizzazione_Latte As String = "", _
                                Optional ByVal Data_ultimo_Censimento As DateTime = Nothing, _
                                Optional ByVal capi_totali As Nullable(Of Integer) = Nothing
                                    ) As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Stalla")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Piva"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Sa_Cod"), CStr(Sa_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("STA_NUM"), CStr(STA_NUM))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Sta_Des"), CStr(Sta_Des))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Ausl_Cod"), CStr(Ausl_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Dat_Costr"), Format(Dat_Costr, "dd/MM/yyyy"))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Dat_Chiu"), Format(Dat_Chiu, "dd/MM/yyyy"))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cod_Fabb"), CStr(Cod_Fabb))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Gen_Cod"), CStr(Gen_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Spe_Cod"), CStr(Spe_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Ipro_Cod"), CStr(Ipro_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("X"), CStr(X))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Y"), CStr(Y))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Dat_Ult_Agg"), Format(Dat_Ult_Agg, "dd/MM/yyyy"))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Latitudine"), CStr(Latitudine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Longitudine"), CStr(Longitudine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("CUAA_Proprietario"), CStr(CUAA_Proprietario))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Denominazione_Proprietario"), CStr(Denominazione_Proprietario))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("CUAA_Detentore"), CStr(CUAA_Detentore))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Denominazione_Detentore"), CStr(Denominazione_Detentore))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("AziendaCodice"), CStr(AziendaCodice))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("AllevamentoDescr"), CStr(allevamentoDescr))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("codAllevamento"), CStr(codAllevamento))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("speCodice"), CStr(speCodice))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("ID_Utente"), CStr(ID_Utente))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("DT_Variazione"), String.Format("{0:d/M/yyyy HH:mm:ss}", DT_Variazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Flag_Libri_Gen"), CStr(flag_Libri_Gen))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Autorizzazione_Latte"), CStr(Autorizzazione_Latte))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Ultimo_Censimento"), String.Format("{0:d/M/yyyy HH:mm:ss}", Data_ultimo_Censimento))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Capi_Totali"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(capi_totali))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function


    '##########################################################################################
    'Viene chiamata quando si deve creare l'xml di una sola materia prima
    'crea il nodo DatiMaterie_Prime
    'e poi chiama la funzione XML_2_MateriaPrima che crea tutto il blocco della materia prima
    Public Function XML_MateriePrime(ByRef Log_Errori As String, _
                                        ByRef XmlDoc As XmlDocument, _
                                        ByVal BaseCode As Integer, _
                                        ByVal TopCode As Integer, _
                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                        ByVal TipoOperazioneDB_Report As enum_TipoOperazioneDB, _
                                        ByVal TipoOperazioneDB_LottoConf As enum_TipoOperazioneDB, _
                                        ByVal TipoOperazioneDB_ProdCosti As enum_TipoOperazioneDB, _
                                        ByVal TipoOperazioneDB_Dettagli As enum_TipoOperazioneDB, _
                                        ByVal TipoOperazioneDB_ParamQual As enum_TipoOperazioneDB, _
                                        ByVal Piva_SuperUser As String, _
                                        ByVal Piva As String, _
                                        ByVal Elem_Cod As Integer, _
                                        ByVal Mat_Cod As Integer, _
                                        ByVal Cod_Articolo As String, _
                                        ByVal Mat_Des As String, _
                                        Optional ByVal DT_MP_Report As DataTable = Nothing, _
                                        Optional ByVal DT_MP_LottoConf As DataTable = Nothing, _
                                        Optional ByVal DT_MP_ProdCosti As DataTable = Nothing, _
                                        Optional ByVal DT_MP_Dettagli As DataTable = Nothing, _
                                        Optional ByVal DT_MP_ParamQual As DataTable = Nothing, _
                                        Optional ByVal Sa_Cod As Integer = 0, _
                                        Optional ByVal Sem_Cod As Integer = 0, _
                                        Optional ByVal Veg_Cod As Integer = 0, _
                                        Optional ByVal Cul_Cod As Integer = 0, _
                                        Optional ByVal Cal_Cod As Integer = 0, _
                                        Optional ByVal Grva_Cod_Veg As Integer = 0, _
                                        Optional ByVal Grfi_Cod As Integer = 0, _
                                        Optional ByVal Trap_Dur As Integer = 0, _
                                        Optional ByVal Uso As Integer = 0, _
                                        Optional ByVal ClToss_Cod As String = "", _
                                        Optional ByVal NewClToss_Cod As String = "", _
                                        Optional ByVal N As Decimal = 0, _
                                        Optional ByVal P2O5 As Decimal = 0, _
                                        Optional ByVal K2O As Decimal = 0, _
                                        Optional ByVal MgO As Decimal = 0, _
                                        Optional ByVal Ditta_Cod As Integer = 0, _
                                        Optional ByVal Prezzo_Unitario As Decimal = 0, _
                                        Optional ByVal Regolamento As Integer = 0, _
                                        Optional ByVal Flag_Convenzionale As Integer = 0, _
                                        Optional ByVal Flag_Biologico As Integer = 0, _
                                        Optional ByVal Flag_NonAgricolo As Integer = 0, _
                                        Optional ByVal Flag_AusiliareFabbricazione As Integer = 0, _
                                        Optional ByVal Gen_Cod As Integer = 0, _
                                        Optional ByVal Spe_Cod As Integer = 0, _
                                        Optional ByVal Raz_Cod As Integer = 0, _
                                        Optional ByVal Ipro_Cod As Integer = 0, _
                                        Optional ByVal Cat_Cod As Integer = 0, _
                                        Optional ByVal ChkImballaggio As Integer = 0, _
                                        Optional ByVal ChkListino As Integer = 0, _
                                        Optional ByVal Taglio As Integer = 0, _
                                        Optional ByVal Flag_Extra As Integer = 0, _
                                        Optional ByVal Udm_Cod_Extra As Integer = 0, _
                                        Optional ByVal Qta_Extra As Decimal = 0, _
                                        Optional ByVal Note As String = "", _
                                        Optional ByVal Mat_Cod_Origine As Integer = 0, _
                                        Optional ByVal Piva_SuperUser_Origine As String = "", _
                                        Optional ByVal Extra_Str As String = "", _
                                        Optional ByVal Extra_Int As Integer = 0, _
                                        Optional ByVal Extra_Date As Date = AGRODATAFINE, _
                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE) As XmlElement


        Dim XmlDatiMateriePrime As System.Xml.XmlElement
        Dim XmlMateriaPrima As System.Xml.XmlElement


        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If



            '#######################################################
            '############   DATI MATERIE PRIME    ##################
            '#######################################################

            XmlDatiMateriePrime = XmlDoc.CreateElement("DatiMaterie_Prime")

            XmlDoc.AppendChild(XmlDatiMateriePrime)


            '#######################################################
            '################   MATERIA PRIMA    ###################
            '#######################################################

            XmlMateriaPrima = XML_MateriaPrima(Log_Errori, _
                                                    XmlDoc, _
                                                    BaseCode, _
                                                    TopCode, _
                                                    TipoOperazioneDB, _
                                                    TipoOperazioneDB_Report, _
                                                    TipoOperazioneDB_LottoConf, _
                                                    TipoOperazioneDB_ProdCosti, _
                                                    TipoOperazioneDB_Dettagli, _
                                                    TipoOperazioneDB_ParamQual, _
                                                    Piva_SuperUser, _
                                                    Piva, _
                                                    Elem_Cod, _
                                                    Mat_Cod, _
                                                    Cod_Articolo, _
                                                    Mat_Des, _
                                                    DT_MP_Report, _
                                                    DT_MP_LottoConf, _
                                                    DT_MP_ProdCosti, _
                                                    DT_MP_Dettagli, _
                                                    DT_MP_ParamQual, _
                                                    Sa_Cod, _
                                                    Sem_Cod, _
                                                    Veg_Cod, _
                                                    Cul_Cod, _
                                                    Cal_Cod, _
                                                    Grva_Cod_Veg, _
                                                    Grfi_Cod, _
                                                    Trap_Dur, _
                                                    Uso, _
                                                    ClToss_Cod, _
                                                    NewClToss_Cod, _
                                                    N, _
                                                    P2O5, _
                                                    K2O, _
                                                    MgO, _
                                                    Ditta_Cod, _
                                                    Prezzo_Unitario, _
                                                    Regolamento, _
                                                    Flag_Convenzionale, _
                                                    Flag_Biologico, _
                                                    Flag_NonAgricolo, _
                                                    Flag_AusiliareFabbricazione, _
                                                    Gen_Cod, _
                                                    Spe_Cod, _
                                                    Raz_Cod, _
                                                    Ipro_Cod, _
                                                    Cat_Cod, _
                                                    ChkImballaggio, _
                                                    ChkListino, _
                                                    Taglio, _
                                                    Flag_Extra, _
                                                    Udm_Cod_Extra, _
                                                    Qta_Extra, _
                                                    Note, _
                                                    Mat_Cod_Origine, _
                                                    Piva_SuperUser_Origine, _
                                                    Extra_Str, _
                                                    Extra_Int, _
                                                    Extra_Date, _
                                                    Validita_Inizio, _
                                                    Validita_Fine)

            XmlDatiMateriePrime.AppendChild(XmlMateriaPrima)



        Catch ex As Exception

            Log_Errori += ex.Message

        End Try


        Return XmlDatiMateriePrime


    End Function




    '##########################################################################################
    'crea tutto il blocco della materia prima ()
    'se si deve inserire una materia prima sola, conviene chiamare XML_2_MateriePrime che crea il nodo raccoglitore, prima di chiamare questa funzione
    'altrimenti, se si devono inserire più materie prime, questa funzione può essere chiamata tante volte quanti sono le materie prime da inserire
    Public Function XML_MateriaPrima(ByRef Log_Errori As String, _
                                        ByRef XmlDoc As XmlDocument, _
                                        ByVal BaseCode As Integer, _
                                        ByVal TopCode As Integer, _
                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                        ByVal TipoOperazioneDB_Report As enum_TipoOperazioneDB, _
                                        ByVal TipoOperazioneDB_LottoConf As enum_TipoOperazioneDB, _
                                        ByVal TipoOperazioneDB_ProdCosti As enum_TipoOperazioneDB, _
                                        ByVal TipoOperazioneDB_Dettagli As enum_TipoOperazioneDB, _
                                        ByVal TipoOperazioneDB_ParamQual As enum_TipoOperazioneDB, _
                                        ByVal Piva_SuperUser As String, _
                                        ByVal Piva As String, _
                                        ByVal Elem_Cod As Integer, _
                                        ByVal Mat_Cod As Integer, _
                                        ByVal Cod_Articolo As String, _
                                        ByVal Mat_Des As String, _
                                        Optional ByVal DT_MP_Report As DataTable = Nothing, _
                                        Optional ByVal DT_MP_LottoConf As DataTable = Nothing, _
                                        Optional ByVal DT_MP_ProdCosti As DataTable = Nothing, _
                                        Optional ByVal DT_MP_Dettagli As DataTable = Nothing, _
                                        Optional ByVal DT_MP_ParamQual As DataTable = Nothing, _
                                        Optional ByVal Sa_Cod As Integer = 0, _
                                        Optional ByVal Sem_Cod As Integer = 0, _
                                        Optional ByVal Veg_Cod As Integer = 0, _
                                        Optional ByVal Cul_Cod As Integer = 0, _
                                        Optional ByVal Cal_Cod As Integer = 0, _
                                        Optional ByVal Grva_Cod_Veg As Integer = 0, _
                                        Optional ByVal Grfi_Cod As Integer = 0, _
                                        Optional ByVal Trap_Dur As Integer = 0, _
                                        Optional ByVal Uso As Integer = 0, _
                                        Optional ByVal ClToss_Cod As String = "", _
                                        Optional ByVal NewClToss_Cod As String = "", _
                                        Optional ByVal N As Decimal = 0, _
                                        Optional ByVal P2O5 As Decimal = 0, _
                                        Optional ByVal K2O As Decimal = 0, _
                                        Optional ByVal MgO As Decimal = 0, _
                                        Optional ByVal Ditta_Cod As Integer = 0, _
                                        Optional ByVal Prezzo_Unitario As Decimal = 0, _
                                        Optional ByVal Regolamento As Integer = 0, _
                                        Optional ByVal Flag_Convenzionale As Integer = 0, _
                                        Optional ByVal Flag_Biologico As Integer = 0, _
                                        Optional ByVal Flag_NonAgricolo As Integer = 0, _
                                        Optional ByVal Flag_AusiliareFabbricazione As Integer = 0, _
                                        Optional ByVal Gen_Cod As Integer = 0, _
                                        Optional ByVal Spe_Cod As Integer = 0, _
                                        Optional ByVal Raz_Cod As Integer = 0, _
                                        Optional ByVal Ipro_Cod As Integer = 0, _
                                        Optional ByVal Cat_Cod As Integer = 0, _
                                        Optional ByVal ChkImballaggio As Integer = 0, _
                                        Optional ByVal ChkListino As Integer = 0, _
                                        Optional ByVal Taglio As Integer = 0, _
                                        Optional ByVal Flag_Extra As Integer = 0, _
                                        Optional ByVal Udm_Cod_Extra As Integer = 0, _
                                        Optional ByVal Qta_Extra As Decimal = 0, _
                                        Optional ByVal Note As String = "", _
                                        Optional ByVal Mat_Cod_Origine As Integer = 0, _
                                        Optional ByVal Piva_SuperUser_Origine As String = "", _
                                        Optional ByVal Extra_Str As String = "", _
                                        Optional ByVal Extra_Int As Integer = 0, _
                                        Optional ByVal Extra_Date As Date = AGRODATAINIZIO, _
                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE) As XmlElement


        Dim XmlMateriaPrima As System.Xml.XmlElement
        Dim XmlDatiMateriePrimexReport As System.Xml.XmlElement
        Dim XmlDatiMateriePrimexLC As System.Xml.XmlElement
        Dim XmlDatiMateriePrimeDettagli As System.Xml.XmlElement
        Dim XmlDatiParametriQualitativi As System.Xml.XmlElement
        Dim XmlParametroQualitativo As System.Xml.XmlElement


        Dim i As Integer

        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If



            '#######################################################
            '################   MATERIA PRIMA    ###################
            '#######################################################

            XmlMateriaPrima = XML_MateriaPrima_MateriaPrima(Log_Errori, _
                                                    XmlDoc, _
                                                    BaseCode, _
                                                    TopCode, _
                                                    TipoOperazioneDB, _
                                                    Piva, _
                                                    Elem_Cod, _
                                                    Mat_Cod, _
                                                    Cod_Articolo, _
                                                    Mat_Des, _
                                                    Sa_Cod, _
                                                    Sem_Cod, _
                                                    Veg_Cod, _
                                                    Cul_Cod, _
                                                    Cal_Cod, _
                                                    Grva_Cod_Veg, _
                                                    Grfi_Cod, _
                                                    Trap_Dur, _
                                                    Uso, _
                                                    ClToss_Cod, _
                                                    NewClToss_Cod, _
                                                    N, _
                                                    P2O5, _
                                                    K2O, _
                                                    MgO, _
                                                    Ditta_Cod, _
                                                    Prezzo_Unitario, _
                                                    Regolamento, _
                                                    Flag_Convenzionale, _
                                                    Flag_Biologico, _
                                                    Flag_NonAgricolo, _
                                                    Flag_AusiliareFabbricazione, _
                                                    Gen_Cod, _
                                                    Spe_Cod, _
                                                    Raz_Cod, _
                                                    Ipro_Cod, _
                                                    Cat_Cod, _
                                                    ChkImballaggio, _
                                                    ChkListino, _
                                                    Taglio, _
                                                    Flag_Extra, _
                                                    Udm_Cod_Extra, _
                                                    Qta_Extra, _
                                                    Note, _
                                                    Mat_Cod_Origine, _
                                                    Piva_SuperUser_Origine, _
                                                    Extra_Str, _
                                                    Extra_Int, _
                                                    Extra_Date, _
                                                    Validita_Inizio, _
                                                    Validita_Fine)


            '#############################################
            '##########  MATERIE PRIME X REPORT   ########
            '#############################################

            XmlDatiMateriePrimexReport = XmlDoc.CreateElement("DatiMaterie_PrimexReport")

            XmlMateriaPrima.AppendChild(XmlDatiMateriePrimexReport)


            '#########################################################
            '##########  MATERIA PRIMA X LOTTO CONFIGURAZIONE ########
            '#########################################################

            XmlDatiMateriePrimexLC = XmlDoc.CreateElement("DatiMaterie_PrimexLC")

            XmlMateriaPrima.AppendChild(XmlDatiMateriePrimexLC)


            '#########################################################
            '###################  MATERIA PRIMA DETTAGLI #############
            '#########################################################

            XmlDatiMateriePrimeDettagli = XmlDoc.CreateElement("DatiMaterie_Prime_Dettagli")

            XmlMateriaPrima.AppendChild(XmlDatiMateriePrimeDettagli)


            'If Not IsNothing(DT_MP_Dettagli) AndAlso DT_MP_Dettagli.Rows.Count <> 0 Then
            '    'scorro il dt e inserisco l'xml

            'Else
            '    'creo un nodo fittizio
            '    XmlMateriaPrimaDettagli = XML_MateriaPrima_Dettagli(Log_Errori, _
            '                                                            XmlDoc, _
            '                                                            BaseCode, _
            '                                                            TopCode, _
            '                                                            TipoOperazioneDB_Dettagli, _
            '                                                            Piva_SuperUser, _
            '                                                            Piva, _
            '                                                            Mat_Cod, _
            '                                                            , , , , , , _
            '                                                            , , , , , , _
            '                                                            , , , , , , _
            '                                                            , , , , , , _
            '                                                            , , , , , , _
            '                                                            , )

            '    XmlDatiMateriePrimeDettagli.AppendChild(XmlMateriaPrimaDettagli)
            'End If


            '#######################################################
            '################   PRODOTTI COSTI    ##################
            '#######################################################

            'XmlDatiProdottiCosti = XmlDoc.CreateElement("DatiProdotti_Costi")

            'XmlMateriaPrima.AppendChild(XmlDatiProdottiCosti)


            '############################################
            '##########  PARAMETRI QUALITATIVI   ########
            '############################################

            XmlDatiParametriQualitativi = XmlDoc.CreateElement("DatiParametri_Qualitativi")

            XmlMateriaPrima.AppendChild(XmlDatiParametriQualitativi)

            If Not IsNothing(DT_MP_ParamQual) AndAlso DT_MP_ParamQual.Rows.Count <> 0 Then
                'scorro il dt e inserisco l'xml

                Dim Tipo As String
                Dim Tipo_Cod As Integer
                Dim Udm_Cod As Integer
                Dim Valore_Des As String
                Dim Valore_Min As Decimal
                Dim Valore_Max As Decimal
                Dim ChkRegistri As Integer
                Dim ChkCalibri As Integer
                Dim Validita_Inizio_ParamQual As Date
                Dim Validita_Fine_ParamQual As Date

                For i = 0 To DT_MP_ParamQual.Rows.Count - 1

                    Tipo = DT_MP_ParamQual.Rows(i).Item("Tipo")
                    Tipo_Cod = DT_MP_ParamQual.Rows(i).Item("Tipo_Cod")
                    Udm_Cod = DT_MP_ParamQual.Rows(i).Item("Udm_Cod")
                    Valore_Des = DT_MP_ParamQual.Rows(i).Item("Valore_Des")
                    Valore_Min = DT_MP_ParamQual.Rows(i).Item("Valore_Min")
                    Valore_Max = DT_MP_ParamQual.Rows(i).Item("Valore_Max")
                    ChkRegistri = DT_MP_ParamQual.Rows(i).Item("ChkRegistri")
                    ChkCalibri = DT_MP_ParamQual.Rows(i).Item("ChkCalibri")
                    Validita_Inizio_ParamQual = DT_MP_ParamQual.Rows(i).Item("Validita_Inizio")
                    Validita_Fine_ParamQual = DT_MP_ParamQual.Rows(i).Item("Validita_Fine")

                    XmlParametroQualitativo = XML_MateriaPrima_ParametroQualitativo(Log_Errori, _
                                                                            XmlDoc, _
                                                                            BaseCode, _
                                                                            TopCode, _
                                                                            TipoOperazioneDB_ParamQual, _
                                                                            Piva, _
                                                                            Mat_Cod, _
                                                                            Sa_Cod, _
                                                                            Tipo, _
                                                                            Tipo_Cod, _
                                                                            Udm_Cod, _
                                                                            Valore_Des, _
                                                                            Valore_Min, _
                                                                            Valore_Max, _
                                                                            ChkRegistri, _
                                                                            ChkCalibri, _
                                                                             , )

                    XmlDatiParametriQualitativi.AppendChild(XmlParametroQualitativo)

                Next


            End If


            '//////////////////////////////////////////////////


        Catch ex As Exception

            Log_Errori += ex.Message

        End Try


        Return XmlMateriaPrima


    End Function


    '##########################################################################################
    Public Function XML_MateriaPrima_MateriaPrima(ByRef Log_Errori As String, _
                                                    ByRef XmlDoc As XmlDocument, _
                                                    ByVal BaseCode As Integer, _
                                                    ByVal TopCode As Integer, _
                                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                    ByVal Piva As String, _
                                                    ByVal Elem_Cod As Integer, _
                                                    ByVal Mat_Cod As Integer, _
                                                    ByVal Cod_Articolo As String, _
                                                    ByVal Mat_Des As String, _
                                                    Optional ByVal Sa_Cod As Integer = 0, _
                                                    Optional ByVal Sem_Cod As Integer = 0, _
                                                    Optional ByVal Veg_Cod As Integer = 0, _
                                                    Optional ByVal Cul_Cod As Integer = 0, _
                                                    Optional ByVal Cal_Cod As Integer = 0, _
                                                    Optional ByVal Grva_Cod_Veg As Integer = 0, _
                                                    Optional ByVal Grfi_Cod As Integer = 0, _
                                                    Optional ByVal Trap_Dur As Integer = 0, _
                                                    Optional ByVal Uso As Integer = 0, _
                                                    Optional ByVal ClToss_Cod As String = "", _
                                                    Optional ByVal NewClToss_Cod As String = "", _
                                                    Optional ByVal N As Decimal = 0, _
                                                    Optional ByVal P2O5 As Decimal = 0, _
                                                    Optional ByVal K2O As Decimal = 0, _
                                                    Optional ByVal MgO As Decimal = 0, _
                                                    Optional ByVal Ditta_Cod As Integer = 0, _
                                                    Optional ByVal Prezzo_Unitario As Decimal = 0, _
                                                    Optional ByVal Regolamento As Integer = 0, _
                                                    Optional ByVal Flag_Convenzionale As Integer = 0, _
                                                    Optional ByVal Flag_Biologico As Integer = 0, _
                                                    Optional ByVal Flag_NonAgricolo As Integer = 0, _
                                                    Optional ByVal Flag_AusiliareFabbricazione As Integer = 0, _
                                                    Optional ByVal Gen_Cod As Integer = 0, _
                                                    Optional ByVal Spe_Cod As Integer = 0, _
                                                    Optional ByVal Raz_Cod As Integer = 0, _
                                                    Optional ByVal Ipro_Cod As Integer = 0, _
                                                    Optional ByVal Cat_Cod As Integer = 0, _
                                                    Optional ByVal ChkImballaggio As Integer = 0, _
                                                    Optional ByVal ChkListino As Integer = 0, _
                                                    Optional ByVal Taglio As Integer = 0, _
                                                    Optional ByVal Flag_Extra As Integer = 0, _
                                                    Optional ByVal Udm_Cod_Extra As Integer = 0, _
                                                    Optional ByVal Qta_Extra As Decimal = 0, _
                                                    Optional ByVal Note As String = "", _
                                                    Optional ByVal Mat_Cod_Origine As Integer = 0, _
                                                    Optional ByVal Piva_SuperUser_Origine As String = "", _
                                                    Optional ByVal Extra_Str As String = "", _
                                                    Optional ByVal Extra_Int As Integer = 0, _
                                                    Optional ByVal Extra_Date As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE) _
                                                    As XmlElement

        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Materia_Prima")

        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))

        aggiungiFiglio(NodoXml, XmlDoc, LCase("piva"), Piva)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("sa_cod"), CStr(Sa_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("elem_cod"), CStr(Elem_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("mat_cod"), CStr(Mat_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("cod_articolo"), Cod_Articolo)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("mat_des"), Mat_Des)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Note"), Note)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("regolamento"), CStr(Regolamento))

        aggiungiFiglio(NodoXml, XmlDoc, LCase("sem_cod"), CStr(Sem_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("veg_cod"), CStr(Veg_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("cul_cod"), CStr(Cul_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("cal_cod"), CStr(Cal_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("grva_cod_veg"), CStr(Grva_Cod_Veg))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("grfi_cod"), CStr(Grfi_Cod))

        aggiungiFiglio(NodoXml, XmlDoc, LCase("gen_cod"), CStr(Gen_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("spe_cod"), CStr(Spe_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("raz_cod"), CStr(Raz_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("ipro_cod"), CStr(Ipro_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("cat_cod"), CStr(Cat_Cod))

        aggiungiFiglio(NodoXml, XmlDoc, LCase("trap_dur"), CStr(Trap_Dur))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("uso"), CStr(Uso))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("cltoss_cod"), CStr(ClToss_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("newcltoss_cod"), CStr(NewClToss_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("n"), CStr(N))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("p2o5"), CStr(P2O5))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("k2o"), CStr(K2O))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("mgo"), CStr(MgO))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("ditta_cod"), CStr(Ditta_Cod))

        aggiungiFiglio(NodoXml, XmlDoc, LCase("prezzo_unitario"), CStr(Prezzo_Unitario))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("flag_convenzionale"), CStr(Flag_Convenzionale))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("flag_biologico"), CStr(Flag_Biologico))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("flag_nonagricolo"), CStr(Flag_NonAgricolo))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("flag_ausiliarefabbricazione"), CStr(Flag_AusiliareFabbricazione))

        aggiungiFiglio(NodoXml, XmlDoc, LCase("ChkImballaggio"), CStr(ChkImballaggio))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("ChkListino"), CStr(ChkListino))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Taglio"), CStr(Taglio))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Flag_Extra"), CStr(Flag_Extra))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Udm_Cod_Extra"), CStr(Udm_Cod_Extra))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Qta_Extra"), CStr(Qta_Extra))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Mat_Cod_Origine"), CStr(Mat_Cod_Origine))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("Piva_SuperUser_Origine"), Piva_SuperUser_Origine)

        aggiungiFiglio(NodoXml, XmlDoc, LCase("extra_str"), Extra_Str)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("extra_int"), CStr(Extra_Int))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("extra_date"), CStr(Extra_Date))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("basecode"), CStr(BaseCode))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("topcode"), CStr(TopCode))


        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function



    '##########################################################################################
    Public Function XML_MateriaPrima_Dettagli(ByRef Log_Errori As String, _
                                                    ByRef XmlDoc As XmlDocument, _
                                                    ByVal BaseCode As Integer, _
                                                    ByVal TopCode As Integer, _
                                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                    ByVal Piva_SuperUser As String, _
                                                    ByVal Piva As String, _
                                                    ByVal Mat_Cod As Integer, _
                                                    Optional ByVal Extra_SmallInt1 As Integer = 0, _
                                                    Optional ByVal Extra_SmallInt2 As Integer = 0, _
                                                    Optional ByVal Extra_SmallInt3 As Integer = 0, _
                                                    Optional ByVal Extra_SmallInt4 As Integer = 0, _
                                                    Optional ByVal Extra_SmallInt5 As Integer = 0, _
                                                    Optional ByVal Extra_SmallInt6 As Integer = 0, _
                                                    Optional ByVal Extra_Int1 As Integer = 0, _
                                                    Optional ByVal Extra_Int2 As Integer = 0, _
                                                    Optional ByVal Extra_Int3 As Integer = 0, _
                                                    Optional ByVal Extra_Int4 As Integer = 0, _
                                                    Optional ByVal Extra_Int5 As Integer = 0, _
                                                    Optional ByVal Extra_Int6 As Integer = 0, _
                                                    Optional ByVal Extra_Dbl1 As Decimal = 0, _
                                                    Optional ByVal Extra_Dbl2 As Decimal = 0, _
                                                    Optional ByVal Extra_Dbl3 As Decimal = 0, _
                                                    Optional ByVal Extra_Dbl4 As Decimal = 0, _
                                                    Optional ByVal Extra_Dbl5 As Decimal = 0, _
                                                    Optional ByVal Extra_Dbl6 As Decimal = 0, _
                                                    Optional ByVal Extra_Str1 As String = "", _
                                                    Optional ByVal Extra_Str2 As String = "", _
                                                    Optional ByVal Extra_Str3 As String = "", _
                                                    Optional ByVal Extra_Str4 As String = "", _
                                                    Optional ByVal Extra_Str5 As String = "", _
                                                    Optional ByVal Extra_Str6 As String = "", _
                                                    Optional ByVal Extra_Date1 As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Extra_Date2 As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Extra_Date3 As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Extra_Date4 As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Extra_Date5 As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Extra_Date6 As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE) _
                                                    As XmlElement

        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Particella")

        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))

        aggiungiFiglio(NodoXml, XmlDoc, LCase("piva_superuser"), Piva_SuperUser)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("piva"), Piva)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("mat_cod"), CStr(Mat_Cod))

        aggiungiFiglio(NodoXml, XmlDoc, "extra_smallint1", Extra_SmallInt1)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_smallint2", Extra_SmallInt2)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_smallint3", Extra_SmallInt3)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_smallint4", Extra_SmallInt4)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_smallint5", Extra_SmallInt5)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_smallint6", Extra_SmallInt6)

        aggiungiFiglio(NodoXml, XmlDoc, "extra_int1", Extra_Int1)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_int2", Extra_Int2)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_int3", Extra_Int3)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_int4", Extra_Int4)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_int5", Extra_Int5)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_int6", Extra_Int6)

        aggiungiFiglio(NodoXml, XmlDoc, "extra_dbl1", Extra_Dbl1)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_dbl2", Extra_Dbl2)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_dbl3", Extra_Dbl3)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_dbl4", Extra_Dbl4)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_dbl5", Extra_Dbl5)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_dbl6", Extra_Dbl6)

        aggiungiFiglio(NodoXml, XmlDoc, "extra_str1", Extra_Str1)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_str2", Extra_Str2)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_str3", Extra_Str3)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_str4", Extra_Str4)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_str5", Extra_Str5)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_str6", Extra_Str6)

        aggiungiFiglio(NodoXml, XmlDoc, "extra_date1", Extra_Date1)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_date2", Extra_Date2)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_date3", Extra_Date3)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_date4", Extra_Date4)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_date5", Extra_Date5)
        aggiungiFiglio(NodoXml, XmlDoc, "extra_date6", Extra_Date6)

        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("basecode"), CStr(BaseCode))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("topcode"), CStr(TopCode))


        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function




    '##########################################################################################
    Public Function XML_MateriaPrima_ParametroQualitativo(ByRef Log_Errori As String, _
                                                            ByRef XmlDoc As XmlDocument, _
                                                            ByVal BaseCode As Integer, _
                                                            ByVal TopCode As Integer, _
                                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                            ByVal Piva As String, _
                                                            ByVal Mat_Cod As Integer, _
                                                            Optional ByVal Sa_Cod As Integer = 0, _
                                                            Optional ByVal Tipo As String = "", _
                                                            Optional ByVal Tipo_Cod As Integer = 0, _
                                                            Optional ByVal Udm_Cod As Integer = 0, _
                                                            Optional ByVal Valore_Des As String = "", _
                                                            Optional ByVal Valore_Min As Decimal = 0, _
                                                            Optional ByVal Valore_Max As Decimal = 0, _
                                                            Optional ByVal ChkRegistri As Integer = 0, _
                                                            Optional ByVal ChkCalibri As Integer = 0, _
                                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                            Optional ByVal Validita_Fine As Date = AGRODATAFINE) _
                                                            As XmlElement

        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Parametro_Qualitativo")


        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))

        aggiungiFiglio(NodoXml, XmlDoc, LCase("piva"), Piva)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("sa_cod"), CStr(Sa_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("mat_cod"), CStr(Mat_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("tipo"), Tipo)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("tipo_cod"), CStr(Tipo_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("udm_cod"), CStr(Udm_Cod))

        aggiungiFiglio(NodoXml, XmlDoc, LCase("valore_des"), Valore_Des)
        aggiungiFiglio(NodoXml, XmlDoc, LCase("valore_min"), CStr(Valore_Min))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("valore_max"), CStr(Valore_Max))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("chkregistri"), CStr(ChkRegistri))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("ChkCalibri"), CStr(ChkCalibri))

        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("basecode"), CStr(BaseCode))
        aggiungiFiglio(NodoXml, XmlDoc, LCase("topcode"), CStr(TopCode))


        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function


    '##########################################################################################
    'Viene chiamata quando si deve creare l'xml di un solo appezzamento
    'crea il nodo DatiAppezzamenti
    'e poi chiama la funzione XML_Appezzamento che crea tutto il blocco dell'appezzamento 
    '
    'da Id_Reg a Data sono dati dell'impianto
    'da Grva_Cod_Veg a ProvenienzaSeme sono dati dell'impianto
    'da Progetto_Cod a Validita_Fine_Progetto sono dati del progetto
    'da Cod_Contratto a Disciplinare_Cod sono dati del progetto
    Public Function XML_Appezzamenti(ByRef Log_Errori As String, _
                                        ByRef XmlDoc As XmlDocument, _
                                        ByVal BaseCode As Integer, _
                                        ByVal TopCode As Integer, _
                                        ByVal Flag_CreaImpianto As Boolean, _
                                        ByVal DT_Codici_Appezzamento As DataTable, _
                                        ByVal DT_Codici_Impianto As DataTable, _
                                        ByVal DT_Codici_Progetto As DataTable, _
                                        ByVal TipoOperazioneDB_Appezzamento As enum_TipoOperazioneDB, _
                                        ByVal TipoOperazioneDB_Impianto As enum_TipoOperazioneDB, _
                                        ByVal TipoOperazioneDB_Progetto As enum_TipoOperazioneDB, _
                                        ByVal Piva_SuperUser As String, _
                                        ByVal Piva As String, _
                                        ByVal Sa_Cod As Integer, _
                                        ByVal Appezza As Integer, _
                                        ByVal Sup_App As Decimal, _
                                        ByVal App_Nome As String, _
                                        ByVal Campo_Cod As Integer, _
                                        ByVal Validita_Inizio_Appezza As Date, _
                                        ByVal Validita_Fine_Appezza As Date, _
                                        ByVal Id_Reg As Integer, _
                                        ByVal Id_Consociazione As Integer, _
                                        ByVal Sup_Imp As Decimal, _
                                        ByVal Cul_Cod As Integer, _
                                        ByVal Validita_Inizio_Impianto As Date, _
                                        ByVal Validita_Fine_Impianto As Date, _
                                        ByVal Data As Date, _
                                        ByVal Progetto_Cod As Integer, _
                                        ByVal Progetto_Nome As String, _
                                        ByVal Progetto_Des As String, _
                                        ByVal Cau_Progetto As Integer, _
                                        ByVal Validita_Inizio_Progetto As Date, _
                                        ByVal Validita_Fine_Progetto As Date, _
                                        Optional ByVal Data_App As String = "0", _
                                        Optional ByVal Data_Inizio As String = "0", _
                                        Optional ByVal Data_Fine As String = "0", _
                                        Optional ByVal Ep_Camp As String = "0", _
                                        Optional ByVal X As Decimal = 0, _
                                        Optional ByVal Y As Decimal = 0, _
                                        Optional ByVal Zslm As Decimal = 0, _
                                        Optional ByVal Esposiz As String = "...", _
                                        Optional ByVal Pende As Decimal = 0, _
                                        Optional ByVal Ubicazione As String = "...", _
                                        Optional ByVal Num_Del As Integer = 0, _
                                        Optional ByVal Clas As String = "", _
                                        Optional ByVal Sabbia As Decimal = 0, _
                                        Optional ByVal Limo As Decimal = 0, _
                                        Optional ByVal Argilla As Decimal = 0, _
                                        Optional ByVal pH As Decimal = 0, _
                                        Optional ByVal CalTot As Decimal = 0, _
                                        Optional ByVal CalAtt As Decimal = 0, _
                                        Optional ByVal SostOrg As Decimal = 0, _
                                        Optional ByVal K2OAss As Decimal = 0, _
                                        Optional ByVal P2O5Ass As Decimal = 0, _
                                        Optional ByVal Mg As Decimal = 0, _
                                        Optional ByVal Ntot As Decimal = 0, _
                                        Optional ByVal Um_S As Decimal = 0, _
                                        Optional ByVal Cl_Dren As String = "0", _
                                        Optional ByVal Falda As Integer = 0, _
                                        Optional ByVal CsC As Decimal = 0, _
                                        Optional ByVal K2OAss_Data As String = "0", _
                                        Optional ByVal MatOrg As Decimal = 0, _
                                        Optional ByVal MatOrg_Data As String = "0", _
                                        Optional ByVal NOtot_Data As String = "0", _
                                        Optional ByVal NOtot As Decimal = 0, _
                                        Optional ByVal P2O5Ass_Data As String = "0", _
                                        Optional ByVal Suolo_CodAttri As String = "", _
                                        Optional ByVal Campo_Spia As Integer = 0, _
                                        Optional ByVal Campo_Spia_Area As Decimal = 0, _
                                        Optional ByVal Cs_SIPI As String = "", _
                                        Optional ByVal Prossimo As Integer = 0, _
                                        Optional ByVal Blk_Flag As Integer = 0, _
                                        Optional ByVal Blk_Inizio_Data As Date = AGRODATAINIZIO, _
                                        Optional ByVal Blk_Inizio_Username As String = "", _
                                        Optional ByVal Blk_Inizio_Note As String = "", _
                                        Optional ByVal Blk_Fine_Data As Date = AGRODATAFINE, _
                                        Optional ByVal Blk_Fine_Username As String = "", _
                                        Optional ByVal Blk_Fine_Note As String = "", _
                                        Optional ByVal Grva_Cod_Veg As Integer = 0, _
                                        Optional ByVal Grfi_Cod As Integer = 0, _
                                        Optional ByVal Cod_Resp As Integer = 0, _
                                        Optional ByVal Cod_Ente As Integer = 0, _
                                        Optional ByVal Campo_Spia_Impianto As Integer = 0, _
                                        Optional ByVal Data_Raccolta As String = "0", _
                                        Optional ByVal Produzione As Integer = 0, _
                                        Optional ByVal ResaPrevista As Decimal = 0, _
                                        Optional ByVal ResaEffettiva As Decimal = 0, _
                                        Optional ByVal Scarto As Integer = 0, _
                                        Optional ByVal Ind_Mat_Cod As Integer = 0, _
                                        Optional ByVal Ind_Mat_Ril As String = "0", _
                                        Optional ByVal Sta_Ter As String = "", _
                                        Optional ByVal Cop_DI As String = "0", _
                                        Optional ByVal Cop_DF As String = "0", _
                                        Optional ByVal Tra_Fila As Decimal = 0, _
                                        Optional ByVal Su_Fila As Decimal = 0, _
                                        Optional ByVal P_HA As Decimal = 0, _
                                        Optional ByVal Foral_Cod As Integer = -1, _
                                        Optional ByVal Setup_Cod As String = "-1", _
                                        Optional ByVal Port_Cod As Integer = -1, _
                                        Optional ByVal Imp_Cod As Integer = -1, _
                                        Optional ByVal Stru_Prot As Integer = 0, _
                                        Optional ByVal Pro_Pag As Integer = 0, _
                                        Optional ByVal Seme_Q As Integer = 0, _
                                        Optional ByVal Seme_T As Integer = 0, _
                                        Optional ByVal Seme_P As Integer = 0, _
                                        Optional ByVal Seme_D As Integer = 0, _
                                        Optional ByVal Stato_Residui As String = "", _
                                        Optional ByVal Tecn_Cod As Integer = -1, _
                                        Optional ByVal Denitrificazione As Integer = 0, _
                                        Optional ByVal Volatilizzazione As Integer = 0, _
                                        Optional ByVal ProfonditaLav As Integer = 0, _
                                        Optional ByVal Id_Campo As Integer = 0, _
                                        Optional ByVal Su_Cod As Integer = -1, _
                                        Optional ByVal Cop_Cod As Integer = 0, _
                                        Optional ByVal Cover As Integer = 0, _
                                        Optional ByVal Monitorato As Integer = 0, _
                                        Optional ByVal Codice_Ficale_Tecnico As String = "", _
                                        Optional ByVal Regolamento As Integer = enum_Cod_Regolamento.Regolamento_Nessuno, _
                                        Optional ByVal Finanziamento As Integer = 0, _
                                        Optional ByVal Data_Conversione As String = "0", _
                                        Optional ByVal ProvenienzaSeme As Integer = 0, _
                                        Optional ByVal Cod_Contratto As Integer = 0, _
                                        Optional ByVal Cod_Conto As Integer = 0, _
                                        Optional ByVal Ricavi_Previsti As Decimal = 0, _
                                        Optional ByVal Produzione_Prevista As Decimal = 0, _
                                        Optional ByVal Giudizio As String = "", _
                                        Optional ByVal Veg_Cod As Integer = 0, _
                                        Optional ByVal Grfi_Cod_Progetto As Integer = 0, _
                                        Optional ByVal CSProgetto_Cod As Integer = 0, _
                                        Optional ByVal Stato_Impianto As enum_Stato_Impianto = enum_Stato_Impianto.Impianto_Produzione, _
                                        Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno, _
                                        Optional ByVal Disciplinare_Cod As Integer = 0, _
                                        Optional ByVal Disciplinare_PrivatoPubblico As Integer = 0, _
                                        Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0, _
                                        Optional ByVal Data_Inizio_Prevista As Date = AGRODATAINIZIO, _
                                        Optional ByVal Data_Fine_Prevista As Date = AGRODATAFINE, _
                                        Optional ByVal via_stringa As String = "", _
                                        Optional ByVal Data_Fioritura_Prevista As Date = AGRODATAINIZIO) As XmlElement


        Dim XmlDatiAppezzamenti As System.Xml.XmlElement
        Dim XmlAppezzamento As System.Xml.XmlElement


        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If


            '#######################################################
            '###############   DATI APPEZZAMENTI    ################
            '#######################################################

            XmlDatiAppezzamenti = XmlDoc.CreateElement("DatiAppezzamenti")

            XmlDoc.AppendChild(XmlDatiAppezzamenti)


            '#######################################################
            '################   APPEZZAMENTO    ####################
            '#######################################################

            XmlAppezzamento = XML_Appezzamento(Log_Errori, _
                                                XmlDoc, _
                                                BaseCode, _
                                                TopCode, _
                                                Flag_CreaImpianto, _
                                                DT_Codici_Appezzamento, _
                                                DT_Codici_Impianto, _
                                                DT_Codici_Progetto, _
                                                TipoOperazioneDB_Appezzamento, _
                                                TipoOperazioneDB_Impianto, _
                                                TipoOperazioneDB_Progetto, _
                                                Piva_SuperUser, _
                                                Piva, _
                                                Sa_Cod, _
                                                Appezza, _
                                                Sup_App, _
                                                App_Nome, _
                                                Campo_Cod, _
                                                Validita_Inizio_Appezza, _
                                                Validita_Fine_Appezza, _
                                                Id_Reg, _
                                           Id_Consociazione, _
                                           Sup_Imp, _
                                           Cul_Cod, _
                                           Validita_Inizio_Impianto, _
                                           Validita_Fine_Impianto, _
                                           Data, _
                                           Progetto_Cod, _
                                           Progetto_Nome, _
                                           Progetto_Des, _
                                           Cau_Progetto, _
                                           Validita_Inizio_Progetto, _
                                           Validita_Fine_Progetto, _
                                                Data_App, _
                                                Data_Inizio, _
                                                Data_Fine, _
                                                Ep_Camp, _
                                                X, _
                                                Y, _
                                                Zslm, _
                                                Esposiz, _
                                                Pende, _
                                                Ubicazione, _
                                                Num_Del, _
                                                Clas, _
                                                Sabbia, _
                                                Limo, _
                                                Argilla, _
                                                pH, _
                                                CalTot, _
                                                CalAtt, _
                                                SostOrg, _
                                                K2OAss, _
                                                P2O5Ass, _
                                                Mg, _
                                                Ntot, _
                                                Um_S, _
                                                Cl_Dren, _
                                                Falda, _
                                                CsC, _
                                                K2OAss_Data, _
                                                MatOrg, _
                                                MatOrg_Data, _
                                                NOtot_Data, _
                                                NOtot, _
                                                P2O5Ass_Data, _
                                                Suolo_CodAttri, _
                                                Campo_Spia, _
                                                Campo_Spia_Area, _
                                                Cs_SIPI, _
                                                Prossimo, _
                                                Blk_Flag, _
                                                Blk_Inizio_Data, _
                                                Blk_Inizio_Username, _
                                                Blk_Inizio_Note, _
                                                Blk_Fine_Data, _
                                                Blk_Fine_Username, _
                                                Blk_Fine_Note, _
                                                Grva_Cod_Veg, _
                                           Grfi_Cod, _
                                           Cod_Resp, _
                                           Cod_Ente, _
                                           Campo_Spia, _
                                           Data_Raccolta, _
                                           Produzione, _
                                           ResaPrevista, _
                                           ResaEffettiva, _
                                           Scarto, _
                                           Ind_Mat_Cod, _
                                           Ind_Mat_Ril, _
                                           Sta_Ter, _
                                           Cop_DI, _
                                           Cop_DF, _
                                           Tra_Fila, _
                                           Su_Fila, _
                                           P_HA, _
                                           Foral_Cod, _
                                           Setup_Cod, _
                                           Port_Cod, _
                                           Imp_Cod, _
                                           Stru_Prot, _
                                           Pro_Pag, _
                                           Seme_Q, _
                                           Seme_T, _
                                           Seme_P, _
                                           Seme_D, _
                                           Stato_Residui, _
                                           Tecn_Cod, _
                                           Denitrificazione, _
                                           Volatilizzazione, _
                                           ProfonditaLav, _
                                           Id_Campo, _
                                           Su_Cod, _
                                           Cop_Cod, _
                                           Cover, _
                                           Monitorato, _
                                           Codice_Ficale_Tecnico, _
                                           Regolamento, _
                                           Finanziamento, _
                                           Data_Conversione, _
                                           ProvenienzaSeme, _
                                           Cod_Contratto, _
                                           Cod_Conto, _
                                           Ricavi_Previsti, _
                                           Produzione_Prevista, _
                                           Giudizio, _
                                           Veg_Cod, _
                                           Grfi_Cod_Progetto, _
                                           CSProgetto_Cod, _
                                           Stato_Impianto, _
                                           Regolamento_Cod, _
                                           Disciplinare_Cod, _
                                           Disciplinare_PrivatoPubblico, _
                                           Regolamento_Concimazioni_Cod, _
                                           Data_Inizio_Prevista, _
                                           Data_Fine_Prevista, _
                                           via_stringa, _
                                           Data_Fioritura_Prevista)


            XmlDatiAppezzamenti.AppendChild(XmlAppezzamento)



        Catch ex As Exception
            Log_Errori += "XML_Appezzamenti. Errore durante la creazione dell'XML dell'Appezzamento: " + ex.Message
        End Try


        Return XmlDatiAppezzamenti


    End Function



    '##########################################################################################
    'crea tutto il blocco dell'Appezzamento ()
    'se si deve inserire un Appezzamento solo, conviene chiamare XML_Appezzamenti che crea il nodo raccoglitore, prima di chiamare questa funzione
    'altrimenti, se si devono inserire più Appezzamenti, questa funzione può essere chiamata tante volte quanti sono gli Appezzamenti da inserire
    '
    'da Id_Reg a Data sono dati dell'impianto
    'da Grva_Cod_Veg a ProvenienzaSeme sono dati dell'impianto
    'da Progetto_Cod a Validita_Fine_Progetto sono dati del progetto
    'da Cod_Contratto a Disciplinare_Cod sono dati del progetto
    Public Function XML_Appezzamento(ByRef Log_Errori As String, _
                                        ByRef XmlDoc As XmlDocument, _
                                        ByVal BaseCode As Integer, _
                                        ByVal TopCode As Integer, _
                                        ByVal Flag_CreaImpianto As Boolean, _
                                        ByVal DT_Codici_Appezzamento As DataTable, _
                                        ByVal DT_Codici_Impianto As DataTable, _
                                        ByVal DT_Codici_Progetto As DataTable, _
                                        ByVal TipoOperazioneDB_Appezzamento As enum_TipoOperazioneDB, _
                                        ByVal TipoOperazioneDB_Impianto As enum_TipoOperazioneDB, _
                                        ByVal TipoOperazioneDB_Progetto As enum_TipoOperazioneDB, _
                                        ByVal Piva_SuperUser As String, _
                                        ByVal Piva As String, _
                                        ByVal Sa_Cod As Integer, _
                                        ByVal Appezza As Integer, _
                                        ByVal Sup_App As Decimal, _
                                        ByVal App_Nome As String, _
                                        ByVal Campo_Cod As Integer, _
                                        ByVal Validita_Inizio_Appezza As Date, _
                                        ByVal Validita_Fine_Appezza As Date, _
                                        ByVal Id_Reg As Integer, _
                                        ByVal Id_Consociazione As Integer, _
                                        ByVal Sup_Imp As Decimal, _
                                        ByVal Cul_Cod As Integer, _
                                        ByVal Validita_Inizio_Impianto As Date, _
                                        ByVal Validita_Fine_Impianto As Date, _
                                        ByVal Data As Date, _
                                        ByVal Progetto_Cod As Integer, _
                                        ByVal Progetto_Nome As String, _
                                        ByVal Progetto_Des As String, _
                                        ByVal Cau_Progetto As Integer, _
                                        ByVal Validita_Inizio_Progetto As Date, _
                                        ByVal Validita_Fine_Progetto As Date, _
                                        Optional ByVal Data_App As String = "0", _
                                        Optional ByVal Data_Inizio As String = "0", _
                                        Optional ByVal Data_Fine As String = "0", _
                                        Optional ByVal Ep_Camp As String = "0", _
                                        Optional ByVal X As Decimal = 0, _
                                        Optional ByVal Y As Decimal = 0, _
                                        Optional ByVal Zslm As Decimal = 0, _
                                        Optional ByVal Esposiz As String = "...", _
                                        Optional ByVal Pende As Decimal = 0, _
                                        Optional ByVal Ubicazione As String = "...", _
                                        Optional ByVal Num_Del As Integer = 0, _
                                        Optional ByVal Clas As String = "", _
                                        Optional ByVal Sabbia As Decimal = 0, _
                                        Optional ByVal Limo As Decimal = 0, _
                                        Optional ByVal Argilla As Decimal = 0, _
                                        Optional ByVal pH As Decimal = 0, _
                                        Optional ByVal CalTot As Decimal = 0, _
                                        Optional ByVal CalAtt As Decimal = 0, _
                                        Optional ByVal SostOrg As Decimal = 0, _
                                        Optional ByVal K2OAss As Decimal = 0, _
                                        Optional ByVal P2O5Ass As Decimal = 0, _
                                        Optional ByVal Mg As Decimal = 0, _
                                        Optional ByVal Ntot As Decimal = 0, _
                                        Optional ByVal Um_S As Decimal = 0, _
                                        Optional ByVal Cl_Dren As String = "0", _
                                        Optional ByVal Falda As Integer = 0, _
                                        Optional ByVal CsC As Decimal = 0, _
                                        Optional ByVal K2OAss_Data As String = "0", _
                                        Optional ByVal MatOrg As Decimal = 0, _
                                        Optional ByVal MatOrg_Data As String = "0", _
                                        Optional ByVal NOtot_Data As String = "0", _
                                        Optional ByVal NOtot As Decimal = 0, _
                                        Optional ByVal P2O5Ass_Data As String = "0", _
                                        Optional ByVal Suolo_CodAttri As String = "", _
                                        Optional ByVal Campo_Spia As Integer = 0, _
                                        Optional ByVal Campo_Spia_Area As Decimal = 0, _
                                        Optional ByVal Cs_SIPI As String = "", _
                                        Optional ByVal Prossimo As Integer = 0, _
                                        Optional ByVal Blk_Flag As Integer = 0, _
                                        Optional ByVal Blk_Inizio_Data As Date = AGRODATAINIZIO, _
                                        Optional ByVal Blk_Inizio_Username As String = "", _
                                        Optional ByVal Blk_Inizio_Note As String = "", _
                                        Optional ByVal Blk_Fine_Data As Date = AGRODATAFINE, _
                                        Optional ByVal Blk_Fine_Username As String = "", _
                                        Optional ByVal Blk_Fine_Note As String = "", _
                                        Optional ByVal Grva_Cod_Veg As Integer = 0, _
                                        Optional ByVal Grfi_Cod As Integer = 0, _
                                        Optional ByVal Cod_Resp As Integer = 0, _
                                        Optional ByVal Cod_Ente As Integer = 0, _
                                        Optional ByVal Campo_Spia_Impianto As Integer = 0, _
                                        Optional ByVal Data_Raccolta As String = "0", _
                                        Optional ByVal Produzione As Integer = 0, _
                                        Optional ByVal ResaPrevista As Decimal = 0, _
                                        Optional ByVal ResaEffettiva As Decimal = 0, _
                                        Optional ByVal Scarto As Integer = 0, _
                                        Optional ByVal Ind_Mat_Cod As Integer = 0, _
                                        Optional ByVal Ind_Mat_Ril As String = "0", _
                                        Optional ByVal Sta_Ter As String = "", _
                                        Optional ByVal Cop_DI As String = "0", _
                                        Optional ByVal Cop_DF As String = "0", _
                                        Optional ByVal Tra_Fila As Decimal = 0, _
                                        Optional ByVal Su_Fila As Decimal = 0, _
                                        Optional ByVal P_HA As Decimal = 0, _
                                        Optional ByVal Foral_Cod As Integer = -1, _
                                        Optional ByVal Setup_Cod As String = "-1", _
                                        Optional ByVal Port_Cod As Integer = -1, _
                                        Optional ByVal Imp_Cod As Integer = -1, _
                                        Optional ByVal Stru_Prot As Integer = 0, _
                                        Optional ByVal Pro_Pag As Integer = 0, _
                                        Optional ByVal Seme_Q As Integer = 0, _
                                        Optional ByVal Seme_T As Integer = 0, _
                                        Optional ByVal Seme_P As Integer = 0, _
                                        Optional ByVal Seme_D As Integer = 0, _
                                        Optional ByVal Stato_Residui As String = "", _
                                        Optional ByVal Tecn_Cod As Integer = -1, _
                                        Optional ByVal Denitrificazione As Integer = 0, _
                                        Optional ByVal Volatilizzazione As Integer = 0, _
                                        Optional ByVal ProfonditaLav As Integer = 0, _
                                        Optional ByVal Id_Campo As Integer = 0, _
                                        Optional ByVal Su_Cod As Integer = -1, _
                                        Optional ByVal Cop_Cod As Integer = 0, _
                                        Optional ByVal Cover As Integer = 0, _
                                        Optional ByVal Monitorato As Integer = 0, _
                                        Optional ByVal Codice_Ficale_Tecnico As String = "", _
                                        Optional ByVal Regolamento As Integer = enum_Cod_Regolamento.Regolamento_Nessuno, _
                                        Optional ByVal Finanziamento As Integer = 0, _
                                        Optional ByVal Data_Conversione As String = "0", _
                                        Optional ByVal ProvenienzaSeme As Integer = 0, _
                                        Optional ByVal Cod_Contratto As Integer = 0, _
                                        Optional ByVal Cod_Conto As Integer = 0, _
                                        Optional ByVal Ricavi_Previsti As Decimal = 0, _
                                        Optional ByVal Produzione_Prevista As Decimal = 0, _
                                        Optional ByVal Giudizio As String = "", _
                                        Optional ByVal Veg_Cod As Integer = 0, _
                                        Optional ByVal Grfi_Cod_Progetto As Integer = 0, _
                                        Optional ByVal CSProgetto_Cod As Integer = 0, _
                                        Optional ByVal Stato_Impianto As enum_Stato_Impianto = enum_Stato_Impianto.Impianto_Produzione, _
                                        Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno, _
                                        Optional ByVal Disciplinare_Cod As Integer = 0, _
                                        Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0, _
                                        Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0, _
                                        Optional ByVal Data_Inizio_Prevista As Date = AGRODATAINIZIO, _
                                        Optional ByVal Data_Fine_Prevista As Date = AGRODATAFINE, _
                                        Optional ByVal via_stringa As String = "", _
                                        Optional ByVal Data_Fioritura_Prevista As Date = AGRODATAINIZIO) As XmlElement

        Dim XmlAppezzamento As System.Xml.XmlElement
        Dim XmlAppezzamentoCodice As System.Xml.XmlElement
        Dim XmlDatiRegImpianto As System.Xml.XmlElement
        Dim i As Integer

        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If



            '#######################################################
            '################   APPEZZAMENTO    ####################
            '#######################################################

            XmlAppezzamento = XML_Appezzamento_Appezzamento(Log_Errori, _
                                                            XmlDoc, _
                                                            BaseCode, _
                                                            TopCode, _
                                                            TipoOperazioneDB_Appezzamento, _
                                                            Piva_SuperUser, _
                                                            Piva, _
                                                            Sa_Cod, _
                                                            Appezza, _
                                                            Sup_App, _
                                                            App_Nome, _
                                                            Campo_Cod, _
                                                            Validita_Inizio_Appezza, _
                                                            Validita_Fine_Appezza, _
                                                            Data_App, _
                                                            Data_Inizio, _
                                                            Data_Fine, _
                                                            Ep_Camp, _
                                                            X, _
                                                            Y, _
                                                            Zslm, _
                                                            Esposiz, _
                                                            Pende, _
                                                            Ubicazione, _
                                                            Num_Del, _
                                                            Clas, _
                                                            Sabbia, _
                                                            Limo, _
                                                            Argilla, _
                                                            pH, _
                                                            CalTot, _
                                                            CalAtt, _
                                                            SostOrg, _
                                                            K2OAss, _
                                                            P2O5Ass, _
                                                            Mg, _
                                                            Ntot, _
                                                            Um_S, _
                                                            Cl_Dren, _
                                                            Falda, _
                                                            CsC, _
                                                            K2OAss_Data, _
                                                            MatOrg, _
                                                            MatOrg_Data, _
                                                            NOtot_Data, _
                                                            NOtot, _
                                                            P2O5Ass_Data, _
                                                            Suolo_CodAttri, _
                                                            Campo_Spia, _
                                                            Campo_Spia_Area, _
                                                            Cs_SIPI, _
                                                            Prossimo, _
                                                            Blk_Flag, _
                                                            Blk_Inizio_Data, _
                                                            Blk_Inizio_Username, _
                                                            Blk_Inizio_Note, _
                                                            Blk_Fine_Data, _
                                                            Blk_Fine_Username, _
                                                            Blk_Fine_Note, _
                                                            via_stringa)


            '#######################################################
            '#############   CodiceAppezzamento    #################
            '#######################################################

            'possono essere tanti nodo codice

            If Not IsNothing(DT_Codici_Appezzamento) AndAlso DT_Codici_Appezzamento.Rows.Count <> 0 Then

                Dim Id_Cod As String
                Dim Val_Cod As String
                Dim Validita_Inizio_Codice As Date
                Dim Validita_Fine_Codice As Date
                Dim TipoOperazioneDB_Codice As enum_TipoOperazioneDB

                For i = 0 To DT_Codici_Appezzamento.Rows.Count - 1

                    TipoOperazioneDB_Codice = DT_Codici_Appezzamento.Rows(i).Item("TipoOperazioneDB")

                    Id_Cod = DT_Codici_Appezzamento.Rows(i).Item("Id_Cod")
                    Val_Cod = DT_Codici_Appezzamento.Rows(i).Item("Val_Cod")
                    Validita_Inizio_Codice = DT_Codici_Appezzamento.Rows(i).Item("Validita_Inizio")
                    Validita_Fine_Codice = DT_Codici_Appezzamento.Rows(i).Item("Validita_Fine")

                    XmlAppezzamentoCodice = XML_Appezzamento_Codice(Log_Errori, _
                                                                TipoOperazioneDB_Codice, _
                                                                Piva, _
                                                                Id_Cod, _
                                                                BaseCode, _
                                                                TopCode, _
                                                                XmlDoc, _
                                                                Sa_Cod, _
                                                                Appezza, _
                                                                Val_Cod, _
                                                                Validita_Inizio_Codice, _
                                                                Validita_Fine_Codice)

                    XmlAppezzamento.AppendChild(XmlAppezzamentoCodice)

                Next

            End If



            'se oltre all'appezzamento si vuole creare anche l'impianto
            If Flag_CreaImpianto = True Then

                '#######################################################
                '###############   DATI REG IMPIANTI    ################
                '#######################################################

                XmlDatiRegImpianto = XML_Impianti(Log_Errori, _
                                               XmlDoc, _
                                               True, _
                                               BaseCode, _
                                               TopCode, _
                                               DT_Codici_Impianto, _
                                                DT_Codici_Progetto, _
                                               TipoOperazioneDB_Impianto, _
                                               TipoOperazioneDB_Progetto, _
                                               Piva_SuperUser, _
                                               Piva, _
                                               Sa_Cod, _
                                               Appezza, _
                                               Id_Reg, _
                                               Id_Consociazione, _
                                               Sup_Imp, _
                                               Cul_Cod, _
                                               Grfi_Cod, _
                                               Validita_Inizio_Impianto, _
                                               Validita_Fine_Impianto, _
                                               Data, _
                                               Progetto_Cod, _
                                               Progetto_Nome, _
                                               Progetto_Des, _
                                               Cau_Progetto, _
                                               Validita_Inizio_Progetto, _
                                               Validita_Fine_Progetto, _
                                               Grva_Cod_Veg, _
                                               Cod_Resp, _
                                               Cod_Ente, _
                                               Campo_Spia, _
                                               Data_Raccolta, _
                                               Produzione, _
                                               ResaPrevista, _
                                               ResaEffettiva, _
                                               Scarto, _
                                               Ind_Mat_Cod, _
                                               Ind_Mat_Ril, _
                                               Sta_Ter, _
                                               Cop_DI, _
                                               Cop_DF, _
                                               Tra_Fila, _
                                               Su_Fila, _
                                               P_HA, _
                                               Foral_Cod, _
                                               Setup_Cod, _
                                               Port_Cod, _
                                               Imp_Cod, _
                                               Stru_Prot, _
                                               Pro_Pag, _
                                               Seme_Q, _
                                               Seme_T, _
                                               Seme_P, _
                                               Seme_D, _
                                               Stato_Residui, _
                                               Tecn_Cod, _
                                               Denitrificazione, _
                                               Volatilizzazione, _
                                               ProfonditaLav, _
                                               Id_Campo, _
                                               Su_Cod, _
                                               Cop_Cod, _
                                               Cover, _
                                               Monitorato, _
                                               Codice_Ficale_Tecnico, _
                                               Regolamento, _
                                               Finanziamento, _
                                               Data_Conversione, _
                                               ProvenienzaSeme, _
                                               Cod_Contratto, _
                                               Cod_Conto, _
                                               Ricavi_Previsti, _
                                               Produzione_Prevista, _
                                               Giudizio, _
                                               Veg_Cod, _
                                               Grfi_Cod_Progetto, _
                                               CSProgetto_Cod, _
                                               Stato_Impianto, _
                                               Regolamento_Cod, _
                                               Disciplinare_Cod, _
                                               Disciplinare_PubblicoPrivato, _
                                               Regolamento_Concimazioni_Cod, _
                                               Data_Inizio_Prevista, _
                                               Data_Fine_Prevista, _
                                               Data_Fioritura_Prevista)


                XmlAppezzamento.AppendChild(XmlDatiRegImpianto)


            End If



            '#######################################################
            '#############   DatiAppezzamenti_Storico    ###########
            '#######################################################



            '#######################################################
            '################   Particella    ######################
            '#######################################################




        Catch ex As Exception
            Log_Errori += "XML_Appezzamento. Errore durante la creazione dell'XML dell'Appezzamento: " + ex.Message
        End Try


        Return XmlAppezzamento


    End Function


    '##########################################################################################
    Public Function XML_Appezzamento_Appezzamento(ByRef Log_Errori As String, _
                                                    ByRef XmlDoc As XmlDocument, _
                                                    ByVal BaseCode As Integer, _
                                                    ByVal TopCode As Integer, _
                                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                    ByVal Piva_SuperUser As String, _
                                                    ByVal Piva As String, _
                                                    ByVal Sa_Cod As Integer, _
                                                    ByVal Appezza As Integer, _
                                                    ByVal Sup_App As Decimal, _
                                                    ByVal App_Nome As String, _
                                                    ByVal Campo_Cod As Integer, _
                                                    ByVal Validita_Inizio As Date, _
                                                    ByVal Validita_Fine As Date, _
                                                    Optional ByVal Data_App As String = "0", _
                                                    Optional ByVal Data_Inizio As String = "0", _
                                                    Optional ByVal Data_Fine As String = "0", _
                                                    Optional ByVal Ep_Camp As String = "0", _
                                                    Optional ByVal X As Decimal = 0, _
                                                    Optional ByVal Y As Decimal = 0, _
                                                    Optional ByVal Zslm As Decimal = 0, _
                                                    Optional ByVal Esposiz As String = "...", _
                                                    Optional ByVal Pende As Decimal = 0, _
                                                    Optional ByVal Ubicazione As String = "...", _
                                                    Optional ByVal Num_Del As Integer = 0, _
                                                    Optional ByVal Clas As String = "", _
                                                    Optional ByVal Sabbia As Decimal = 0, _
                                                    Optional ByVal Limo As Decimal = 0, _
                                                    Optional ByVal Argilla As Decimal = 0, _
                                                    Optional ByVal pH As Decimal = 0, _
                                                    Optional ByVal CalTot As Decimal = 0, _
                                                    Optional ByVal CalAtt As Decimal = 0, _
                                                    Optional ByVal SostOrg As Decimal = 0, _
                                                    Optional ByVal K2OAss As Decimal = 0, _
                                                    Optional ByVal P2O5Ass As Decimal = 0, _
                                                    Optional ByVal Mg As Decimal = 0, _
                                                    Optional ByVal Ntot As Decimal = 0, _
                                                    Optional ByVal Um_S As Decimal = 0, _
                                                    Optional ByVal Cl_Dren As String = "0", _
                                                    Optional ByVal Falda As Integer = 0, _
                                                    Optional ByVal CsC As Decimal = 0, _
                                                    Optional ByVal K2OAss_Data As String = "0", _
                                                    Optional ByVal MatOrg As Decimal = 0, _
                                                    Optional ByVal MatOrg_Data As String = "0", _
                                                    Optional ByVal NOtot_Data As String = "0", _
                                                    Optional ByVal NOtot As Decimal = 0, _
                                                    Optional ByVal P2O5Ass_Data As String = "0", _
                                                    Optional ByVal Suolo_CodAttri As String = "", _
                                                    Optional ByVal Campo_Spia As Integer = 0, _
                                                    Optional ByVal Campo_Spia_Area As Decimal = 0, _
                                                    Optional ByVal Cs_SIPI As String = "", _
                                                    Optional ByVal Prossimo As Integer = 0, _
                                                    Optional ByVal Blk_Flag As Integer = 0, _
                                                    Optional ByVal Blk_Inizio_Data As Date = AGRODATAINIZIO, _
                                                    Optional ByVal Blk_Inizio_Username As String = "", _
                                                    Optional ByVal Blk_Inizio_Note As String = "", _
                                                    Optional ByVal Blk_Fine_Data As Date = AGRODATAFINE, _
                                                    Optional ByVal Blk_Fine_Username As String = "", _
                                                    Optional ByVal Blk_Fine_Note As String = "", _
                                                     Optional ByVal via_stringa As String = "", _
                                                     Optional ByVal AltriVitigniPresenti As Nullable(Of Integer) = Nothing) As XmlElement

        Try

            Dim XmlAppezzamento As XmlElement

            If XmlDoc Is Nothing Then
                XmlDoc = New XmlDocument
            End If


            '----- Genero la stringa XML a partire dai valori dei parametri

            'Creo il nodo 
            XmlAppezzamento = XmlDoc.CreateElement("Appezzamento")

            'Imposto gli attributi
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))

            'su Appezzamento_Scrivi non viene usato questo attributo,
            'ma viene usato direttamente il campo Piva_SuperUser
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "user", CStr(Piva_SuperUser))

            aggiungiFiglio(XmlAppezzamento, XmlDoc, "piva", CStr(Piva))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "sa_cod", CStr(Sa_Cod))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "appezza", CStr(Appezza))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "sup_app", CStr(Sup_App))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "data_app", CStr(Data_App)) '= validita_inizio
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "app_nome", CStr(App_Nome))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "campo_cod", CStr(Campo_Cod))

            aggiungiFiglio(XmlAppezzamento, XmlDoc, "ep_camp", CStr(Ep_Camp))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "x", CStr(X))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "y", CStr(Y))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "zslm", CStr(Zslm))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "esposiz", CStr(Esposiz))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "pende", CStr(Pende))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "ubicazione", CStr(Ubicazione))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "num_del", CStr(Num_Del))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "clas", CStr(Clas))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "sabbia", CStr(Sabbia))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "limo", CStr(Limo))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "argilla", CStr(Argilla))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "ph", CStr(pH))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "caltot", CStr(CalTot))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "calatt", CStr(CalAtt))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "sostorg", CStr(SostOrg))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "k2oass", CStr(K2OAss))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "p2o5ass", CStr(P2O5Ass))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "mg", CStr(Mg))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "ntot", CStr(Ntot))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "um_s", CStr(Um_S))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "cl_dren", CStr(Cl_Dren))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "falda", CStr(Falda))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "csc", CStr(CsC))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "k2oass_data", CStr(K2OAss_Data))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "matorg", CStr(MatOrg))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "matorg_data", CStr(MatOrg_Data))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "notot_data", CStr(NOtot_Data))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "notot", CStr(NOtot))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "p2o5ass_data", CStr(P2O5Ass_Data))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "suolo_codattri", CStr(Suolo_CodAttri))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "campo_spia", CStr(Campo_Spia))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "campo_spia_area", CStr(Campo_Spia_Area))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "cs_sipi", CStr(Cs_SIPI))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "prossimo", CStr(Prossimo))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "data_inizio", CStr(Data_Inizio)) ' = Validita_Inizio
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "data_fine", CStr(Data_Fine)) ' = Validita_Fine
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))

            aggiungiFiglio(XmlAppezzamento, XmlDoc, "via_stringa", via_stringa)


            'non sono gestiti attualmente da Appezzamento_Scrivi
            aggiungiFiglio(XmlAppezzamento, XmlDoc, LCase("Blk_Flag"), CStr(Blk_Flag))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, LCase("Blk_Inizio_Data"), CStr(Blk_Inizio_Data))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, LCase("Blk_Inizio_Username"), CStr(Blk_Inizio_Username))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, LCase("Blk_Inizio_Note"), CStr(Blk_Inizio_Note))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, LCase("Blk_Fine_Data"), CStr(Blk_Fine_Data))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, LCase("Blk_Fine_Username"), CStr(Blk_Fine_Username))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, LCase("Blk_Fine_Note"), CStr(Blk_Fine_Note))

            aggiungiFiglio(XmlAppezzamento, XmlDoc, "basecode", CStr(BaseCode))
            aggiungiFiglio(XmlAppezzamento, XmlDoc, "topcode", CStr(TopCode))


            aggiungiFiglio(XmlAppezzamento, XmlDoc, "AltriVitigniPresenti", AgronicaCoreDataProvider.UtilityProvider.ValoreToString(AltriVitigniPresenti))

            'Distruggo gli oggetti
            Return XmlAppezzamento

            XmlAppezzamento = Nothing



        Catch ex As Exception
            Log_Errori += "XML_Appezzamento_Appezzamento. Errore durante la creazione dell'XML dell'Appezzamento: " + ex.Message
        End Try


    End Function


    '##########################################################################################
    Public Function XML_Appezzamento_Codice(ByRef Log_Errori As String, _
                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                            ByVal Piva As String, _
                                            ByVal Id_Cod As Integer, _
                                            ByVal BaseCode As Integer, _
                                            ByVal TopCode As Integer, _
                                            Optional ByRef XmlDoc As XmlDocument = Nothing, _
                                            Optional ByVal Sa_Cod As Integer = 0, _
                                            Optional ByVal Appezza As Integer = 0, _
                                            Optional ByVal Val_Cod As String = "", _
                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                            Optional ByVal Validita_Fine As Date = AGRODATAFINE) _
                                            As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        Try

            If XmlDoc Is Nothing Then
                XmlDoc = New XmlDocument
            End If

            '----- Genero la stringa XML a partire dai valori dei parametri

            'Creo il nodo 
            NodoXml = XmlDoc.CreateElement("CodiceAppezzamento")

            'Imposto gli attributi
            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("piva"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("sa_cod"), CStr(Sa_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("appezza"), CStr(Appezza))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("id_cod"), CStr(Id_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("val_cod"), CStr(Val_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("basecode"), CStr(BaseCode))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("topcode"), CStr(TopCode))

            'Restituisco in uscita 
            Return NodoXml

            'Distruggo gli oggetti
            NodoXml = Nothing

        Catch ex As Exception
            Log_Errori += "XML_Appezzamento_Codice. Errore durante la creazione dell'XML del codice dell'Appezzamento: " + ex.Message
        End Try


    End Function


    '##########################################################################################
    Public Function XML_Campo_Codice(ByRef Log_Errori As String, _
                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                            ByVal Piva As String, _
                                            ByVal Sa_Cod As Integer, _
                                            ByVal id_cod As Integer, _
                                            ByVal val_cod As String, _
                                            ByVal BaseCode As Integer, _
                                            ByVal TopCode As Integer, _
                                            Optional ByRef XmlDoc As XmlDocument = Nothing, _
                                            Optional ByVal Campo_Cod As Integer = 0, _
                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                            Optional ByVal Validita_Fine As Date = AGRODATAFINE) _
                                            As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        Try

            If XmlDoc Is Nothing Then
                XmlDoc = New XmlDocument
            End If

            '----- Genero la stringa XML a partire dai valori dei parametri

            'Creo il nodo 
            NodoXml = XmlDoc.CreateElement("Codice")

            'Imposto gli attributi
            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("piva"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("sa_cod"), CStr(Sa_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("id_cod"), CStr(id_cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("val_cod"), CStr(val_cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Campo_Cod"), CStr(Campo_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("basecode"), CStr(BaseCode))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("topcode"), CStr(TopCode))

            'Restituisco in uscita 
            Return NodoXml

            'Distruggo gli oggetti
            NodoXml = Nothing

        Catch ex As Exception
            Log_Errori += "XML_Campo_Codice. Errore durante la creazione dell'XML del codice del Campo: " + ex.Message
        End Try


    End Function



    '##########################################################################################
    'Raccoglitore dell'impianto
    'da Progetto_Cod a Validita_Fine_Progetto sono dati del progetto
    'da Cod_Contratto a Disciplinare_Cod sono dati del progetto
    '
    '23/03/2012
    'aggiunto
    '    Optional ByVal Disciplinare_PrivatoPubblico As Integer = 0, _
    'Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0, _
    Public Function XML_Impianti(ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByVal Flag_AggancioAppezzamento As Boolean, _
                                    ByVal BaseCode As Integer, _
                                    ByVal TopCode As Integer, _
                                    ByVal DT_Codici_Impianto As DataTable, _
                                    ByVal DT_Codici_Progetto As DataTable, _
                                    ByVal TipoOperazioneDB_Impianto As enum_TipoOperazioneDB, _
                                    ByVal TipoOperazioneDB_Progetto As enum_TipoOperazioneDB, _
                                    ByVal Piva_SuperUser As String, _
                                    ByVal Piva As String, _
                                    ByVal Sa_Cod As Integer, _
                                    ByVal Appezza As Integer, _
                                    ByVal Id_Reg As Integer, _
                                    ByVal Id_Consociazione As Integer, _
                                    ByVal Sup_Imp As Decimal, _
                                    ByVal Cul_Cod As Integer, _
                                    ByVal Grfi_Cod As Integer, _
                                    ByVal Validita_Inizio_Impianto As Date, _
                                    ByVal Validita_Fine_Impianto As Date, _
                                    ByVal Data As Date, _
                                        ByVal Progetto_Cod As Integer, _
                                        ByVal Progetto_Nome As String, _
                                        ByVal Progetto_Des As String, _
                                        ByVal Cau_Progetto As Integer, _
                                        ByVal Validita_Inizio_Progetto As Date, _
                                        ByVal Validita_Fine_Progetto As Date, _
                                            Optional ByVal Grva_Cod_Veg As Integer = 0, _
                                            Optional ByVal Cod_Resp As Integer = 0, _
                                            Optional ByVal Cod_Ente As Integer = 0, _
                                            Optional ByVal Campo_Spia As Integer = 0, _
                                            Optional ByVal Data_Raccolta As String = "0", _
                                            Optional ByVal Produzione As Integer = 0, _
                                            Optional ByVal ResaPrevista As Decimal = 0, _
                                            Optional ByVal ResaEffettiva As Decimal = 0, _
                                            Optional ByVal Scarto As Integer = 0, _
                                            Optional ByVal Ind_Mat_Cod As Integer = 0, _
                                            Optional ByVal Ind_Mat_Ril As String = "0", _
                                            Optional ByVal Sta_Ter As String = "", _
                                            Optional ByVal Cop_DI As String = "0", _
                                            Optional ByVal Cop_DF As String = "0", _
                                            Optional ByVal Tra_Fila As Decimal = 0, _
                                            Optional ByVal Su_Fila As Decimal = 0, _
                                            Optional ByVal P_HA As Decimal = 0, _
                                            Optional ByVal Foral_Cod As Integer = -1, _
                                            Optional ByVal Setup_Cod As String = "-1", _
                                            Optional ByVal Port_Cod As Integer = -1, _
                                            Optional ByVal Imp_Cod As Integer = -1, _
                                            Optional ByVal Stru_Prot As Integer = 0, _
                                            Optional ByVal Pro_Pag As Integer = 0, _
                                            Optional ByVal Seme_Q As Integer = 0, _
                                            Optional ByVal Seme_T As Integer = 0, _
                                            Optional ByVal Seme_P As Integer = 0, _
                                            Optional ByVal Seme_D As Integer = 0, _
                                            Optional ByVal Stato_Residui As String = "", _
                                            Optional ByVal Tecn_Cod As Integer = -1, _
                                            Optional ByVal Denitrificazione As Integer = 0, _
                                            Optional ByVal Volatilizzazione As Integer = 0, _
                                            Optional ByVal ProfonditaLav As Integer = 0, _
                                            Optional ByVal Id_Campo As Integer = 0, _
                                            Optional ByVal Su_Cod As Integer = -1, _
                                            Optional ByVal Cop_Cod As Integer = 0, _
                                            Optional ByVal Cover As Integer = 0, _
                                            Optional ByVal Monitorato As Integer = 0, _
                                            Optional ByVal Codice_Ficale_Tecnico As String = "", _
                                            Optional ByVal Regolamento As Integer = enum_Cod_Regolamento.Regolamento_Nessuno, _
                                            Optional ByVal Finanziamento As Integer = 0, _
                                            Optional ByVal Data_Conversione As String = "0", _
                                            Optional ByVal ProvenienzaSeme As Integer = 0, _
                                    Optional ByVal Cod_Contratto As Integer = 0, _
                                    Optional ByVal Cod_Conto As Integer = 0, _
                                    Optional ByVal Ricavi_Previsti As Decimal = 0, _
                                    Optional ByVal Produzione_Prevista As Decimal = 0, _
                                    Optional ByVal Giudizio As String = "", _
                                    Optional ByVal Veg_Cod As Integer = 0, _
                                    Optional ByVal Grfi_Cod_Progetto As Integer = 0, _
                                    Optional ByVal CSProgetto_Cod As Integer = 0, _
                                    Optional ByVal Stato_Impianto As enum_Stato_Impianto = enum_Stato_Impianto.Impianto_Produzione, _
                                    Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno, _
                                    Optional ByVal Disciplinare_Cod As Integer = 0, _
                                    Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0, _
                                    Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0, _
                                    Optional ByVal Data_Inizio_Prevista As Date = AGRODATAINIZIO, _
                                    Optional ByVal Data_Fine_Prevista As Date = AGRODATAFINE, _
                                    Optional ByVal Data_Fioritura_Prevista As Date = AGRODATAINIZIO) As XmlElement


        Dim XmlDatiRegImpianti As System.Xml.XmlElement
        Dim XmlRegImpianto As System.Xml.XmlElement


        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If



            '#######################################################
            '###############   DATI REG IMPIANTI    ################
            '#######################################################

            XmlDatiRegImpianti = XmlDoc.CreateElement("DatiReg_Impianti")

            If Flag_AggancioAppezzamento = False Then
                XmlDoc.AppendChild(XmlDatiRegImpianti)
            End If


            '#######################################################
            '################# REG  IMPIANTO    ####################
            '#######################################################

            XmlRegImpianto = XML_Impianto(Log_Errori, _
                                            XmlDoc, _
                                            BaseCode, _
                                            TopCode, _
                                            DT_Codici_Impianto, _
                                            DT_Codici_Progetto, _
                                            TipoOperazioneDB_Impianto, _
                                            TipoOperazioneDB_Progetto, _
                                            Piva_SuperUser, _
                                            Piva, _
                                            Sa_Cod, _
                                            Appezza, _
                                            Id_Reg, _
                                            Id_Consociazione, _
                                            Sup_Imp, _
                                            Cul_Cod, _
                                            Grfi_Cod, _
                                            Validita_Inizio_Impianto, _
                                            Validita_Fine_Impianto, _
                                            Data, _
                                            Progetto_Cod, _
                                            Progetto_Nome, _
                                            Progetto_Des, _
                                            Cau_Progetto, _
                                            Validita_Inizio_Progetto, _
                                            Validita_Fine_Progetto, _
                                            Grva_Cod_Veg, _
                                            Cod_Resp, _
                                            Cod_Ente, _
                                            Campo_Spia, _
                                            Data_Raccolta, _
                                            Produzione, _
                                            ResaPrevista, _
                                            ResaEffettiva, _
                                            Scarto, _
                                            Ind_Mat_Cod, _
                                            Ind_Mat_Ril, _
                                            Sta_Ter, _
                                            Cop_DI, _
                                            Cop_DF, _
                                            Tra_Fila, _
                                            Su_Fila, _
                                            P_HA, _
                                            Foral_Cod, _
                                            Setup_Cod, _
                                            Port_Cod, _
                                            Imp_Cod, _
                                            Stru_Prot, _
                                            Pro_Pag, _
                                            Seme_Q, _
                                            Seme_T, _
                                            Seme_P, _
                                            Seme_D, _
                                            Stato_Residui, _
                                            Tecn_Cod, _
                                            Denitrificazione, _
                                            Volatilizzazione, _
                                            ProfonditaLav, _
                                            Id_Campo, _
                                            Su_Cod, _
                                            Cop_Cod, _
                                            Cover, _
                                            Monitorato, _
                                            Codice_Ficale_Tecnico, _
                                            Regolamento, _
                                            Finanziamento, _
                                            Data_Conversione, _
                                            ProvenienzaSeme, _
                                            Cod_Contratto, _
                                            Cod_Conto, _
                                            Ricavi_Previsti, _
                                            Produzione_Prevista, _
                                            Giudizio, _
                                            Veg_Cod, _
                                            Grfi_Cod_Progetto, _
                                            CSProgetto_Cod, _
                                            Stato_Impianto, _
                                            Regolamento_Cod, _
                                            Disciplinare_Cod, _
                                            Disciplinare_PubblicoPrivato, _
                                            Regolamento_Concimazioni_Cod, _
                                            Data_Inizio_Prevista, _
                                            Data_Fine_Prevista, _
                                            Data_Fioritura_Prevista)


            XmlDatiRegImpianti.AppendChild(XmlRegImpianto)



        Catch ex As Exception
            Log_Errori += "XML_Impianti. Errore durante la creazione dell'XML dell'Impianto: " + ex.Message
        End Try


        Return XmlDatiRegImpianti


    End Function


    '##########################################################################################
    'crea tutto il blocco dell'impianto ()
    ''
    '23/03/2012
    'aggiunto
    '    Optional ByVal Disciplinare_PrivatoPubblico As Integer = 0, _
    'Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0, _
    '
    '
    'da Progetto_Cod a Validita_Fine_Progetto sono dati del progetto
    'da Cod_Contratto a Disciplinare_Cod sono dati del progetto
    Public Function XML_Impianto(ByRef Log_Errori As String, _
                                    ByRef XmlDoc As XmlDocument, _
                                    ByVal BaseCode As Integer, _
                                    ByVal TopCode As Integer, _
                                    ByVal DT_Codici_Impianto As DataTable, _
                                    ByVal DT_Codici_Progetto As DataTable, _
                                    ByVal TipoOperazioneDB_Impianto As enum_TipoOperazioneDB, _
                                    ByVal TipoOperazioneDB_Progetto As enum_TipoOperazioneDB, _
                                    ByVal Piva_SuperUser As String, _
                                    ByVal Piva As String, _
                                    ByVal Sa_Cod As Integer, _
                                    ByVal Appezza As Integer, _
                                    ByVal Id_Reg As Integer, _
                                    ByVal Id_Consociazione As Integer, _
                                    ByVal Sup_Imp As Decimal, _
                                    ByVal Cul_Cod As Integer, _
                                    ByVal Grfi_Cod As Integer, _
                                    ByVal Validita_Inizio_Impianto As Date, _
                                    ByVal Validita_Fine_Impianto As Date, _
                                    ByVal Data As Date, _
                                    ByVal Progetto_Cod As Integer, _
                                    ByVal Progetto_Nome As String, _
                                    ByVal Progetto_Des As String, _
                                    ByVal Cau_Progetto As Integer, _
                                    ByVal Validita_Inizio_Progetto As Date, _
                                    ByVal Validita_Fine_Progetto As Date, _
                                            Optional ByVal Grva_Cod_Veg As Integer = 0, _
                                            Optional ByVal Cod_Resp As Integer = 0, _
                                            Optional ByVal Cod_Ente As Integer = 0, _
                                            Optional ByVal Campo_Spia As Integer = 0, _
                                            Optional ByVal Data_Raccolta As String = "0", _
                                            Optional ByVal Produzione As Integer = 0, _
                                            Optional ByVal ResaPrevista As Decimal = 0, _
                                            Optional ByVal ResaEffettiva As Decimal = 0, _
                                            Optional ByVal Scarto As Integer = 0, _
                                            Optional ByVal Ind_Mat_Cod As Integer = 0, _
                                            Optional ByVal Ind_Mat_Ril As String = "0", _
                                            Optional ByVal Sta_Ter As String = "", _
                                            Optional ByVal Cop_DI As String = "0", _
                                            Optional ByVal Cop_DF As String = "0", _
                                            Optional ByVal Tra_Fila As Decimal = 0, _
                                            Optional ByVal Su_Fila As Decimal = 0, _
                                            Optional ByVal P_HA As Decimal = 0, _
                                            Optional ByVal Foral_Cod As Integer = -1, _
                                            Optional ByVal Setup_Cod As String = "-1", _
                                            Optional ByVal Port_Cod As Integer = -1, _
                                            Optional ByVal Imp_Cod As Integer = -1, _
                                            Optional ByVal Stru_Prot As Integer = 0, _
                                            Optional ByVal Pro_Pag As Integer = 0, _
                                            Optional ByVal Seme_Q As Integer = 0, _
                                            Optional ByVal Seme_T As Integer = 0, _
                                            Optional ByVal Seme_P As Integer = 0, _
                                            Optional ByVal Seme_D As Integer = 0, _
                                            Optional ByVal Stato_Residui As String = "", _
                                            Optional ByVal Tecn_Cod As Integer = -1, _
                                            Optional ByVal Denitrificazione As Integer = 0, _
                                            Optional ByVal Volatilizzazione As Integer = 0, _
                                            Optional ByVal ProfonditaLav As Integer = 0, _
                                            Optional ByVal Id_Campo As Integer = 0, _
                                            Optional ByVal Su_Cod As Integer = -1, _
                                            Optional ByVal Cop_Cod As Integer = 0, _
                                            Optional ByVal Cover As Integer = 0, _
                                            Optional ByVal Monitorato As Integer = 0, _
                                            Optional ByVal Codice_Ficale_Tecnico As String = "", _
                                            Optional ByVal Regolamento As Integer = enum_Cod_Regolamento.Regolamento_Nessuno, _
                                            Optional ByVal Finanziamento As Integer = 0, _
                                            Optional ByVal Data_Conversione As String = "0", _
                                            Optional ByVal ProvenienzaSeme As Integer = 0, _
                                    Optional ByVal Cod_Contratto As Integer = 0, _
                                    Optional ByVal Cod_Conto As Integer = 0, _
                                    Optional ByVal Ricavi_Previsti As Decimal = 0, _
                                    Optional ByVal Produzione_Prevista As Decimal = 0, _
                                    Optional ByVal Giudizio As String = "", _
                                    Optional ByVal Veg_Cod As Integer = 0, _
                                    Optional ByVal Grfi_Cod_Progetto As Integer = 0, _
                                    Optional ByVal CSProgetto_Cod As Integer = 0, _
                                    Optional ByVal Stato_Impianto As enum_Stato_Impianto = enum_Stato_Impianto.Impianto_Produzione, _
                                    Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno, _
                                    Optional ByVal Disciplinare_Cod As Integer = 0, _
                                    Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0, _
                                    Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0, _
                                    Optional ByVal Data_Inizio_Prevista As Date = AGRODATAINIZIO, _
                                    Optional ByVal Data_Fine_Prevista As Date = AGRODATAFINE, _
                                    Optional ByVal Data_Fioritura_Prevista As Date = AGRODATAINIZIO) As XmlElement

        Dim XmlRegImpianto As System.Xml.XmlElement
        Dim XmlImpiantoCodice As System.Xml.XmlElement
        Dim XmlDatiProgetto As System.Xml.XmlElement
        Dim i As Integer

        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If


            '#######################################################
            '################   REG IMPIANTO    ####################
            '#######################################################

            XmlRegImpianto = XML_Impianto_Impianto(Log_Errori, _
                                                    XmlDoc, _
                                                    BaseCode, _
                                                    TopCode, _
                                                    TipoOperazioneDB_Impianto, _
                                                    Piva_SuperUser, _
                                                    Piva, _
                                                    Sa_Cod, _
                                                    Appezza, _
                                                    Id_Reg, _
                                                    Id_Consociazione, _
                                                    Sup_Imp, _
                                                    Cul_Cod, _
                                                    Grfi_Cod, _
                                                    Validita_Inizio_Impianto, _
                                                    Validita_Fine_Impianto, _
                                                    Data, _
                                                    Grva_Cod_Veg, _
                                                    Cod_Resp, _
                                                    Cod_Ente, _
                                                    Campo_Spia, _
                                                    Data_Raccolta, _
                                                    Produzione, _
                                                    ResaPrevista, _
                                                    ResaEffettiva, _
                                                    Scarto, _
                                                    Ind_Mat_Cod, _
                                                    Ind_Mat_Ril, _
                                                    Sta_Ter, _
                                                    Cop_DI, _
                                                    Cop_DF, _
                                                    Tra_Fila, _
                                                    Su_Fila, _
                                                    P_HA, _
                                                    Foral_Cod, _
                                                    Setup_Cod, _
                                                    Port_Cod, _
                                                    Imp_Cod, _
                                                    Stru_Prot, _
                                                    Pro_Pag, _
                                                    Seme_Q, _
                                                    Seme_T, _
                                                    Seme_P, _
                                                    Seme_D, _
                                                    Stato_Residui, _
                                                    Tecn_Cod, _
                                                    Denitrificazione, _
                                                    Volatilizzazione, _
                                                    ProfonditaLav, _
                                                    Id_Campo, _
                                                    Su_Cod, _
                                                    Cop_Cod, _
                                                    Cover, _
                                                    Monitorato, _
                                                    Codice_Ficale_Tecnico, _
                                                    Regolamento, _
                                                    Finanziamento, _
                                                    Data_Conversione, _
                                                    ProvenienzaSeme, _
                                                    Data_Fioritura_Prevista)


            '#######################################################
            '################   CodiceImpianto    ##################
            '#######################################################

            'possono essere tanti nodo codice

            If Not IsNothing(DT_Codici_Impianto) AndAlso DT_Codici_Impianto.Rows.Count <> 0 Then
                Dim XmlDatiCodici As System.Xml.XmlElement = XmlDoc.CreateElement("DatiCodici")
                XmlRegImpianto.AppendChild(XmlDatiCodici)
                Dim Id_Cod As String
                Dim Val_Cod As String
                Dim Validita_Inizio_Codice As Date
                Dim Validita_Fine_Codice As Date
                Dim TipoOperazioneDB_Codice As enum_TipoOperazioneDB

                For i = 0 To DT_Codici_Impianto.Rows.Count - 1

                    TipoOperazioneDB_Codice = DT_Codici_Impianto.Rows(i).Item("TipoOperazioneDB")

                    Id_Cod = DT_Codici_Impianto.Rows(i).Item("Id_Cod")
                    Val_Cod = DT_Codici_Impianto.Rows(i).Item("Val_Cod")
                    Validita_Inizio_Codice = DT_Codici_Impianto.Rows(i).Item("Validita_Inizio")
                    Validita_Fine_Codice = DT_Codici_Impianto.Rows(i).Item("Validita_Fine")

                    XmlImpiantoCodice = XML_Impianto_Codice(Log_Errori, _
                                                            TipoOperazioneDB_Codice, _
                                                            Piva, _
                                                            Id_Cod, _
                                                            BaseCode, _
                                                            TopCode, _
                                                            XmlDoc, _
                                                            Sa_Cod, _
                                                            Appezza, _
                                                            Id_Reg, _
                                                            Val_Cod, _
                                                            Validita_Inizio_Codice, _
                                                            Validita_Fine_Codice)

                    XmlRegImpianto.SelectSingleNode("child::DatiCodici").AppendChild(XmlImpiantoCodice)

                Next

            End If



            '#######################################################
            '################   DATI PROGETTI    ###################
            '#######################################################

            XmlDatiProgetto = XML_ProgettiImpianto(Log_Errori, _
                                                XmlDoc, _
                                                True, _
                                                BaseCode, _
                                                TopCode, _
                                                DT_Codici_Progetto, _
                                                TipoOperazioneDB_Progetto, _
                                                Piva, _
                                                Sa_Cod, _
                                                Appezza, _
                                                Id_Reg, _
                                                Progetto_Cod, _
                                                Progetto_Nome, _
                                                Progetto_Des, _
                                                Cau_Progetto, _
                                                Validita_Inizio_Progetto, _
                                                Validita_Fine_Progetto, _
                                                Cod_Contratto, _
                                                Cod_Conto, _
                                                Ricavi_Previsti, _
                                                Produzione_Prevista, _
                                                Giudizio, _
                                                Veg_Cod, _
                                                Grfi_Cod_Progetto, _
                                                CSProgetto_Cod, _
                                                Stato_Impianto, _
                                                Regolamento_Cod, _
                                                Disciplinare_Cod, _
                                                Disciplinare_PubblicoPrivato, _
                                                Regolamento_Concimazioni_Cod, _
                                                Data_Inizio_Prevista, _
                                                Data_Fine_Prevista, _
                                                P_HA, _
                                                Data_Fioritura_Prevista)


            XmlRegImpianto.AppendChild(XmlDatiProgetto)



        Catch ex As Exception
            Log_Errori += "XML_Impianto. Errore durante la creazione dell'XML dell'Impianto: " + ex.Message
        End Try


        Return XmlRegImpianto


    End Function


    '##########################################################################################
    Public Function XML_Impianto_Impianto(ByRef Log_Errori As String, _
                                            ByRef XmlDoc As XmlDocument, _
                                            ByVal BaseCode As Integer, _
                                            ByVal TopCode As Integer, _
                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                            ByVal Piva_SuperUser As String, _
                                            ByVal Piva As String, _
                                            ByVal Sa_Cod As Integer, _
                                            ByVal Appezza As Integer, _
                                            ByVal Id_Reg As Integer, _
                                            ByVal Id_Consociazione As Integer, _
                                            ByVal Sup_Imp As Decimal, _
                                            ByVal Cul_Cod As Integer, _
                                            ByVal Grfi_Cod As Integer, _
                                            ByVal Validita_Inizio As Date, _
                                            ByVal Validita_Fine As Date, _
                                            ByVal Data As Date, _
                                            Optional ByVal Grva_Cod_Veg As Integer = 0, _
                                            Optional ByVal Cod_Resp As Integer = 0, _
                                            Optional ByVal Cod_Ente As Integer = 0, _
                                            Optional ByVal Campo_Spia As Integer = 0, _
                                            Optional ByVal Data_Raccolta As String = "0", _
                                            Optional ByVal Produzione As Integer = 0, _
                                            Optional ByVal ResaPrevista As Decimal = 0, _
                                            Optional ByVal ResaEffettiva As Decimal = 0, _
                                            Optional ByVal Scarto As Integer = 0, _
                                            Optional ByVal Ind_Mat_Cod As Integer = 0, _
                                            Optional ByVal Ind_Mat_Ril As String = "0", _
                                            Optional ByVal Sta_Ter As String = "", _
                                            Optional ByVal Cop_DI As String = "0", _
                                            Optional ByVal Cop_DF As String = "0", _
                                            Optional ByVal Tra_Fila As Decimal = 0, _
                                            Optional ByVal Su_Fila As Decimal = 0, _
                                            Optional ByVal P_HA As Decimal = 0, _
                                            Optional ByVal Foral_Cod As Integer = -1, _
                                            Optional ByVal Setup_Cod As String = "-1", _
                                            Optional ByVal Port_Cod As Integer = -1, _
                                            Optional ByVal Imp_Cod As Integer = -1, _
                                            Optional ByVal Stru_Prot As Integer = 0, _
                                            Optional ByVal Pro_Pag As Integer = 0, _
                                            Optional ByVal Seme_Q As Integer = 0, _
                                            Optional ByVal Seme_T As Integer = 0, _
                                            Optional ByVal Seme_P As Integer = 0, _
                                            Optional ByVal Seme_D As Integer = 0, _
                                            Optional ByVal Stato_Residui As String = "", _
                                            Optional ByVal Tecn_Cod As Integer = -1, _
                                            Optional ByVal Denitrificazione As Integer = 0, _
                                            Optional ByVal Volatilizzazione As Integer = 0, _
                                            Optional ByVal ProfonditaLav As Integer = 0, _
                                            Optional ByVal Id_Campo As Integer = 0, _
                                            Optional ByVal Su_Cod As Integer = -1, _
                                            Optional ByVal Cop_Cod As Integer = 0, _
                                            Optional ByVal Cover As Integer = 0, _
                                            Optional ByVal Monitorato As Integer = 0, _
                                            Optional ByVal Codice_Ficale_Tecnico As String = "", _
                                            Optional ByVal Regolamento As Integer = enum_Cod_Regolamento.Regolamento_Nessuno, _
                                            Optional ByVal Finanziamento As Integer = 0, _
                                            Optional ByVal Data_Conversione As String = "0", _
                                            Optional ByVal ProvenienzaSeme As Integer = 0, _
                                            Optional ByVal Data_Fioritura_Prevista As Date = AGRODATAINIZIO, _
                                            Optional ByVal Unita_Vitata As Integer = 0, _
                                            Optional ByVal Sovrainnesto_Cod As Integer = 0, _
                                            Optional ByVal ancoraggiTestata As Nullable(Of Int32) = 0, _
                                            Optional ByVal annoRiferimento As Nullable(Of Int32) = 0, _
                                            Optional ByVal codFiliStostegno As String = "", _
                                            Optional ByVal codPaliTessitura As String = "", _
                                            Optional ByVal codPaliTestata As String = "", _
                                            Optional ByVal codStatoColt As String = "", _
                                            Optional ByVal codTipoVari As String = "", _
                                            Optional ByVal DataProtocollo As Date = Nothing, _
                                            Optional ByVal DataRilievo As Date = Nothing, _
                                            Optional ByVal destProduttiva As String = "", _
                                            Optional ByVal destProduttivaDescr As String = "", _
                                            Optional ByVal distanzaPali As Double = Nothing, _
                                            Optional ByVal dtFine As Date = Nothing, _
                                            Optional ByVal dtFineGestione As Date = Nothing, _
                                            Optional ByVal dtInizio As Date = Nothing, _
                                            Optional ByVal dtInizioGestione As Date = Nothing, _
                                            Optional ByVal dtIns As Date = Nothing, _
                                            Optional ByVal dtVar As Date = Nothing, _
                                            Optional ByVal fallanzePerc As Double = Nothing, _
                                            Optional ByVal flagAnomalia As String = "", _
                                            Optional ByVal flagAttuale As String = "", _
                                            Optional ByVal flagCessata As String = "", _
                                            Optional ByVal flagContributo As String = "", _
                                            Optional ByVal flagRegolarizz2009 As String = "", _
                                            Optional ByVal flagRicalcoloGis As String = "", _
                                            Optional ByVal GiacituraTerreno As String = "", _
                                            Optional ByVal idUnitaVitata As Nullable(Of Int32) = 0, _
                                            Optional ByVal idUtenteIns As String = "", _
                                            Optional ByVal idUtenteVar As String = "", _
                                            Optional ByVal numeroProtocollo As String = "", _
                                            Optional ByVal progPoligono As String = "", _
                                            Optional ByVal supVitataDich As Nullable(Of Int32) = 0, _
                                            Optional ByVal supVitataDichPRCalcolo As Nullable(Of Int32) = 0, _
                                            Optional ByVal SuperficieServizioMq As Nullable(Of Int32) = 0, _
                                            Optional ByVal Terrazzamenti As Nullable(Of Int32) = 0, _
                                            Optional ByVal TipoColtura As String = "", _
                                            Optional ByVal tipoProcedimento As String = "", _
                                            Optional ByVal tipoUnar As String = "", _
                                            Optional ByVal tipoVariazione As String = "", _
                                            Optional ByVal unar As String = "" _
                                            ) As XmlElement


        Try

            Dim XmlImpianto As System.Xml.XmlElement

            If XmlDoc Is Nothing Then
                XmlDoc = New XmlDocument
            End If


            '----- Genero la stringa XML a partire dai valori dei parametri

            'Creo il nodo 
            XmlImpianto = XmlDoc.CreateElement("Reg_Impianto")

            'Imposto gli attributi
            aggiungiFiglio(XmlImpianto, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))

            'su Reg_Impianto_Scrivi non viene usato questo attributo,
            'ma viene usato direttamente il campo Piva_SuperUser
            aggiungiFiglio(XmlImpianto, XmlDoc, "user", CStr(Piva_SuperUser))

            aggiungiFiglio(XmlImpianto, XmlDoc, "piva", CStr(Piva))
            aggiungiFiglio(XmlImpianto, XmlDoc, "sa_cod", CStr(Sa_Cod))
            aggiungiFiglio(XmlImpianto, XmlDoc, "appezza", CStr(Appezza))
            aggiungiFiglio(XmlImpianto, XmlDoc, "id_reg", CStr(Id_Reg))
            aggiungiFiglio(XmlImpianto, XmlDoc, "id_consociazione", CStr(Id_Consociazione))
            aggiungiFiglio(XmlImpianto, XmlDoc, "sup_imp", CStr(Sup_Imp))
            aggiungiFiglio(XmlImpianto, XmlDoc, "cul_cod", CStr(Cul_Cod))

            aggiungiFiglio(XmlImpianto, XmlDoc, "grva_cod_veg", CStr(Grva_Cod_Veg))
            aggiungiFiglio(XmlImpianto, XmlDoc, "grfi_cod", CStr(Grfi_Cod))
            aggiungiFiglio(XmlImpianto, XmlDoc, "cod_resp", CStr(Cod_Resp))
            aggiungiFiglio(XmlImpianto, XmlDoc, "cod_ente", CStr(Cod_Ente))
            aggiungiFiglio(XmlImpianto, XmlDoc, "campo_spia", CStr(Campo_Spia))
            aggiungiFiglio(XmlImpianto, XmlDoc, "data", CStr(Data)) 'impostare = validita_inizio
            aggiungiFiglio(XmlImpianto, XmlDoc, "data_raccolta", CStr(Data_Raccolta))

            'questo campo non viene gestito da Reg_Impianto_Scrivi
            'ma è nel db!
            aggiungiFiglio(XmlImpianto, XmlDoc, "produzione", CStr(Produzione))

            aggiungiFiglio(XmlImpianto, XmlDoc, "resa_prevista", CStr(ResaPrevista))
            aggiungiFiglio(XmlImpianto, XmlDoc, "resa_effettiva", CStr(ResaEffettiva))
            aggiungiFiglio(XmlImpianto, XmlDoc, "scarto", CStr(Scarto))
            aggiungiFiglio(XmlImpianto, XmlDoc, "ind_mat_cod", CStr(Ind_Mat_Cod))
            aggiungiFiglio(XmlImpianto, XmlDoc, "ind_mat_ril", CStr(Ind_Mat_Ril))
            aggiungiFiglio(XmlImpianto, XmlDoc, "sta_ter", CStr(Sta_Ter))
            aggiungiFiglio(XmlImpianto, XmlDoc, "cop_di", CStr(Cop_DI))
            aggiungiFiglio(XmlImpianto, XmlDoc, "cop_df", CStr(Cop_DF))
            aggiungiFiglio(XmlImpianto, XmlDoc, "tra_fila", CStr(Tra_Fila))
            aggiungiFiglio(XmlImpianto, XmlDoc, "su_fila", CStr(Su_Fila))
            aggiungiFiglio(XmlImpianto, XmlDoc, "p_ha", CStr(P_HA))
            aggiungiFiglio(XmlImpianto, XmlDoc, "foral_cod", CStr(Foral_Cod))
            aggiungiFiglio(XmlImpianto, XmlDoc, "setup_cod", CStr(Setup_Cod))
            aggiungiFiglio(XmlImpianto, XmlDoc, "port_cod", CStr(Port_Cod))
            aggiungiFiglio(XmlImpianto, XmlDoc, "imp_cod", CStr(Imp_Cod))
            aggiungiFiglio(XmlImpianto, XmlDoc, "stru_prot", CStr(Stru_Prot))
            aggiungiFiglio(XmlImpianto, XmlDoc, "pro_pag", CStr(Pro_Pag))
            aggiungiFiglio(XmlImpianto, XmlDoc, "seme_q", CStr(Seme_Q))
            aggiungiFiglio(XmlImpianto, XmlDoc, "seme_t", CStr(Seme_T))
            aggiungiFiglio(XmlImpianto, XmlDoc, "seme_p", CStr(Seme_P))
            aggiungiFiglio(XmlImpianto, XmlDoc, "seme_d", CStr(Seme_D))
            aggiungiFiglio(XmlImpianto, XmlDoc, "stato_residui", CStr(Stato_Residui))
            aggiungiFiglio(XmlImpianto, XmlDoc, "tecn_cod", CStr(Tecn_Cod))
            aggiungiFiglio(XmlImpianto, XmlDoc, "denitrificazione", CStr(Denitrificazione))
            aggiungiFiglio(XmlImpianto, XmlDoc, "volatilizzazione", CStr(Volatilizzazione))
            aggiungiFiglio(XmlImpianto, XmlDoc, "profonditalav", CStr(ProfonditaLav))
            aggiungiFiglio(XmlImpianto, XmlDoc, "id_campo", CStr(Id_Campo))
            aggiungiFiglio(XmlImpianto, XmlDoc, "su_cod", CStr(Su_Cod))
            aggiungiFiglio(XmlImpianto, XmlDoc, "cop_cod", CStr(Cop_Cod))
            aggiungiFiglio(XmlImpianto, XmlDoc, "cover", CStr(Cover))
            aggiungiFiglio(XmlImpianto, XmlDoc, "monitorato", CStr(Monitorato))
            aggiungiFiglio(XmlImpianto, XmlDoc, "codice_fiscale_tecnico", CStr(Codice_Ficale_Tecnico))
            aggiungiFiglio(XmlImpianto, XmlDoc, "regolamento", CStr(Regolamento))
            aggiungiFiglio(XmlImpianto, XmlDoc, "finanziamento", CStr(Finanziamento))
            aggiungiFiglio(XmlImpianto, XmlDoc, "data_conversione", CStr(Data_Conversione))
            aggiungiFiglio(XmlImpianto, XmlDoc, "provenienzaseme", CStr(ProvenienzaSeme))
            aggiungiFiglio(XmlImpianto, XmlDoc, "validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
            aggiungiFiglio(XmlImpianto, XmlDoc, "validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))

            aggiungiFiglio(XmlImpianto, XmlDoc, "data_fioritura_prevista", CStr(Data_Fioritura_Prevista))

            aggiungiFiglio(XmlImpianto, XmlDoc, "basecode", CStr(BaseCode))
            aggiungiFiglio(XmlImpianto, XmlDoc, "topcode", CStr(TopCode))



            aggiungiFiglio(XmlImpianto, XmlDoc, "Unita_Vitata", CStr(Unita_Vitata))
            aggiungiFiglio(XmlImpianto, XmlDoc, "Sovrainnesto_Cod", CStr(Sovrainnesto_Cod))
            aggiungiFiglio(XmlImpianto, XmlDoc, "ancoraggiTestata", CStr(ancoraggiTestata))
            aggiungiFiglio(XmlImpianto, XmlDoc, "annoRiferimento", CStr(annoRiferimento))
            aggiungiFiglio(XmlImpianto, XmlDoc, "codFiliStostegno", CStr(codFiliStostegno))
            aggiungiFiglio(XmlImpianto, XmlDoc, "codPaliTessitura", CStr(codPaliTessitura))
            aggiungiFiglio(XmlImpianto, XmlDoc, "codPaliTestata", CStr(codPaliTestata))
            aggiungiFiglio(XmlImpianto, XmlDoc, "codStatoColt", CStr(codStatoColt))
            aggiungiFiglio(XmlImpianto, XmlDoc, "codTipoVari", CStr(codTipoVari))
            aggiungiFiglio(XmlImpianto, XmlDoc, "DataProtocollo", CStr(DataProtocollo))
            aggiungiFiglio(XmlImpianto, XmlDoc, "DataRilievo", CStr(DataRilievo))
            aggiungiFiglio(XmlImpianto, XmlDoc, "destProduttiva", CStr(destProduttiva))
            aggiungiFiglio(XmlImpianto, XmlDoc, "destProduttivaDescr", CStr(destProduttivaDescr))
            aggiungiFiglio(XmlImpianto, XmlDoc, "distanzaPali", CStr(distanzaPali))
            aggiungiFiglio(XmlImpianto, XmlDoc, "dtFine", CStr(dtFine))
            aggiungiFiglio(XmlImpianto, XmlDoc, "dtFineGestione", CStr(dtFineGestione))
            aggiungiFiglio(XmlImpianto, XmlDoc, "dtInizio", CStr(dtInizio))
            aggiungiFiglio(XmlImpianto, XmlDoc, "dtInizioGestione", CStr(dtInizioGestione))
            aggiungiFiglio(XmlImpianto, XmlDoc, "dtIns", CStr(dtIns))
            aggiungiFiglio(XmlImpianto, XmlDoc, "dtVar", CStr(dtVar))
            aggiungiFiglio(XmlImpianto, XmlDoc, "fallanzePerc", CStr(fallanzePerc))
            aggiungiFiglio(XmlImpianto, XmlDoc, "flagAnomalia", CStr(flagAnomalia))
            aggiungiFiglio(XmlImpianto, XmlDoc, "flagAttuale", CStr(flagAttuale))
            aggiungiFiglio(XmlImpianto, XmlDoc, "flagCessata", CStr(flagCessata))
            aggiungiFiglio(XmlImpianto, XmlDoc, "flagContributo", CStr(flagContributo))
            aggiungiFiglio(XmlImpianto, XmlDoc, "flagRegolarizz2009", CStr(flagRegolarizz2009))
            aggiungiFiglio(XmlImpianto, XmlDoc, "flagRicalcoloGis", CStr(flagRicalcoloGis))
            aggiungiFiglio(XmlImpianto, XmlDoc, "GiacituraTerreno", CStr(GiacituraTerreno))
            aggiungiFiglio(XmlImpianto, XmlDoc, "idUnitaVitata", CStr(idUnitaVitata))
            aggiungiFiglio(XmlImpianto, XmlDoc, "idUtenteIns", CStr(idUtenteIns))
            aggiungiFiglio(XmlImpianto, XmlDoc, "idUtenteVar", CStr(idUtenteVar))
            aggiungiFiglio(XmlImpianto, XmlDoc, "numeroProtocollo", CStr(numeroProtocollo))
            aggiungiFiglio(XmlImpianto, XmlDoc, "progPoligono", CStr(progPoligono))
            aggiungiFiglio(XmlImpianto, XmlDoc, "supVitataDich", CStr(supVitataDich))
            aggiungiFiglio(XmlImpianto, XmlDoc, "supVitataDichPRCalcolo", CStr(supVitataDichPRCalcolo))
            aggiungiFiglio(XmlImpianto, XmlDoc, "SuperficieServizioMq", CStr(SuperficieServizioMq))
            aggiungiFiglio(XmlImpianto, XmlDoc, "Terrazzamenti", CStr(Terrazzamenti))
            aggiungiFiglio(XmlImpianto, XmlDoc, "TipoColtura", CStr(TipoColtura))
            aggiungiFiglio(XmlImpianto, XmlDoc, "tipoProcedimento", CStr(tipoProcedimento))
            aggiungiFiglio(XmlImpianto, XmlDoc, "tipoUnar", CStr(tipoUnar))
            aggiungiFiglio(XmlImpianto, XmlDoc, "tipoVariazione", CStr(tipoVariazione))
            aggiungiFiglio(XmlImpianto, XmlDoc, "unar", CStr(unar))

            Return XmlImpianto

            'Distruggo gli oggetti
            XmlImpianto = Nothing


        Catch ex As Exception
            Log_Errori += "XML_Impianto_Impianto. Errore durante la creazione dell'XML dell'Impianto: " + ex.Message
        End Try


    End Function


    '##########################################################################################
    Public Function XML_Impianto_Codice(ByRef Log_Errori As String, _
                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                            ByVal Piva As String, _
                                            ByVal Id_Cod As Integer, _
                                            ByVal BaseCode As Integer, _
                                            ByVal TopCode As Integer, _
                                            Optional ByRef XmlDoc As XmlDocument = Nothing, _
                                            Optional ByVal Sa_Cod As Integer = 0, _
                                            Optional ByVal Appezza As Integer = 0, _
                                            Optional ByVal Id_Reg As Integer = 0, _
                                            Optional ByVal Val_Cod As String = "", _
                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                            Optional ByVal Validita_Fine As Date = AGRODATAFINE) _
                                            As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        Try

            If XmlDoc Is Nothing Then
                XmlDoc = New XmlDocument
            End If

            '----- Genero la stringa XML a partire dai valori dei parametri

            'Creo il nodo 
            NodoXml = XmlDoc.CreateElement("CodiceImpianto")

            'Imposto gli attributi
            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("piva"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("sa_cod"), CStr(Sa_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("appezza"), CStr(Appezza))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("id_reg"), CStr(Id_Reg))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("id_cod"), CStr(Id_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("val_cod"), CStr(Val_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("basecode"), CStr(BaseCode))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("topcode"), CStr(TopCode))

            'Restituisco in uscita 
            Return NodoXml

            'Distruggo gli oggetti
            NodoXml = Nothing

        Catch ex As Exception
            Log_Errori += "XML_Impianto_Codice. Errore durante la creazione dell'XML del codice dell'Impianto: " + ex.Message
        End Try


    End Function


    '##########################################################################################
    'Raccoglitore del progetto (distinta)
    '23/03/2012
    'aggiunto
    '    Optional ByVal Disciplinare_PrivatoPubblico As Integer = 0, _
    'Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0, _
    Public Function XML_ProgettiImpianto(ByRef Log_Errori As String, _
                                        ByRef XmlDoc As XmlDocument, _
                                        ByVal Flag_AggancioImpianto As Boolean, _
                                        ByVal BaseCode As Integer, _
                                        ByVal TopCode As Integer, _
                                        ByVal DT_Codici_Progetto As DataTable, _
                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                        ByVal Piva As String, _
                                        ByVal Sa_Cod As Integer, _
                                        ByVal Appezza As Integer, _
                                        ByVal Id_Reg As Integer, _
                                        ByVal Progetto_Cod As Integer, _
                                        ByVal Progetto_Nome As String, _
                                        ByVal Progetto_Des As String, _
                                        ByVal Cau_Progetto As Integer, _
                                        ByVal Validita_Inizio As Date, _
                                        ByVal Validita_Fine As Date, _
                                        Optional ByVal Cod_Contratto As Integer = 0, _
                                        Optional ByVal Cod_Conto As Integer = 0, _
                                        Optional ByVal Ricavi_Previsti As Decimal = 0, _
                                        Optional ByVal Produzione_Prevista As Decimal = 0, _
                                        Optional ByVal Giudizio As String = "", _
                                        Optional ByVal Veg_Cod As Integer = 0, _
                                        Optional ByVal Grfi_Cod As Integer = 0, _
                                        Optional ByVal CSProgetto_Cod As Integer = 0, _
                                        Optional ByVal Stato_Impianto As enum_Stato_Impianto = enum_Stato_Impianto.Impianto_Produzione, _
                                        Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno, _
                                        Optional ByVal Disciplinare_Cod As Integer = 0, _
                                        Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0, _
                                        Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0, _
                                        Optional ByVal Data_Inizio_Prevista As Date = AGRODATAINIZIO, _
                                        Optional ByVal Data_Fine_Prevista As Date = AGRODATAFINE, _
                                        Optional ByVal P_HA As Decimal = 0, _
                                       Optional ByVal Data_Fioritura_Prevista As Date = AGRODATAINIZIO) As XmlElement


        Dim XmlDatiProgetto As System.Xml.XmlElement
        Dim XmlProgetto As System.Xml.XmlElement


        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If



            '#######################################################
            '##################   DATI PROGETTO    #################
            '#######################################################

            XmlDatiProgetto = XmlDoc.CreateElement("DatiProgetto")

            If Flag_AggancioImpianto = False Then
                XmlDoc.AppendChild(XmlDatiProgetto)
            End If


            '#######################################################
            '##################    PROGETTO  #######################
            '#######################################################

            XmlProgetto = XML_ProgettoImpianto(Log_Errori, _
                                                XmlDoc, _
                                                BaseCode, _
                                                TopCode, _
                                                DT_Codici_Progetto, _
                                                TipoOperazioneDB, _
                                                Piva, _
                                                Sa_Cod, _
                                                Appezza, _
                                                Id_Reg, _
                                                Progetto_Cod, _
                                                Progetto_Nome, _
                                                Progetto_Des, _
                                                Cau_Progetto, _
                                                Validita_Inizio, _
                                                Validita_Fine, _
                                                Cod_Contratto, _
                                                Cod_Conto, _
                                                Ricavi_Previsti, _
                                                Produzione_Prevista, _
                                                Giudizio, _
                                                Veg_Cod, _
                                                Grfi_Cod, _
                                                CSProgetto_Cod, _
                                                Stato_Impianto, _
                                                Regolamento_Cod, _
                                                Disciplinare_Cod, _
                                                Disciplinare_PubblicoPrivato, _
                                                Regolamento_Concimazioni_Cod, _
                                                Data_Inizio_Prevista, _
                                                Data_Fine_Prevista, _
                                                P_HA, _
                                                Data_Fioritura_Prevista)


            XmlDatiProgetto.AppendChild(XmlProgetto)



        Catch ex As Exception
            Log_Errori += "XML_ProgettiImpianto. Errore durante la creazione dell'XML della Distinta dell'Impianto: " + ex.Message
        End Try


        Return XmlDatiProgetto


    End Function


    '##########################################################################################
    'crea tutto il blocco del progetto (distinta)
    '23/03/2012
    'aggiunto
    '    Optional ByVal Disciplinare_PrivatoPubblico As Integer = 0, _
    'Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0, _
    Public Function XML_ProgettoImpianto(ByRef Log_Errori As String, _
                                            ByRef XmlDoc As XmlDocument, _
                                            ByVal BaseCode As Integer, _
                                            ByVal TopCode As Integer, _
                                            ByVal DT_Codici_Progetto As DataTable, _
                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                            ByVal Piva As String, _
                                            ByVal Sa_Cod As Integer, _
                                            ByVal Appezza As Integer, _
                                            ByVal Id_Reg As Integer, _
                                            ByVal Progetto_Cod As Integer, _
                                            ByVal Progetto_Nome As String, _
                                            ByVal Progetto_Des As String, _
                                            ByVal Cau_Progetto As Integer, _
                                            ByVal Validita_Inizio As Date, _
                                            ByVal Validita_Fine As Date, _
                                            Optional ByVal Cod_Contratto As Integer = 0, _
                                            Optional ByVal Cod_Conto As Integer = 0, _
                                            Optional ByVal Ricavi_Previsti As Decimal = 0, _
                                            Optional ByVal Produzione_Prevista As Decimal = 0, _
                                            Optional ByVal Giudizio As String = "", _
                                            Optional ByVal Veg_Cod As Integer = 0, _
                                            Optional ByVal Grfi_Cod As Integer = 0, _
                                            Optional ByVal CSProgetto_Cod As Integer = 0, _
                                            Optional ByVal Stato_Impianto As enum_Stato_Impianto = enum_Stato_Impianto.Impianto_Produzione, _
                                            Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno, _
                                            Optional ByVal Disciplinare_Cod As Integer = 0, _
                                            Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0, _
                                            Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0, _
                                            Optional ByVal Data_Inizio_Prevista As Date = AGRODATAINIZIO, _
                                            Optional ByVal Data_Fine_Prevista As Date = AGRODATAFINE, _
                                            Optional ByVal P_HA As Decimal = 0, _
                                            Optional ByVal Data_Fioritura_Prevista As Date = AGRODATAINIZIO) As XmlElement


        Dim XmlProgetto As System.Xml.XmlElement
        Dim XmlProgettoCodice As System.Xml.XmlElement
        Dim i As Integer

        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If



            '#######################################################
            '##################   PROGETTO    ######################
            '#######################################################

            XmlProgetto = XML_ProgettoImpianto_ProgettoImpianto(Log_Errori, _
                                                                XmlDoc, _
                                                                BaseCode, _
                                                                TopCode, _
                                                                TipoOperazioneDB, _
                                                                Piva, _
                                                                Sa_Cod, _
                                                                Appezza, _
                                                                Id_Reg, _
                                                                Progetto_Cod, _
                                                                Progetto_Nome, _
                                                                Progetto_Des, _
                                                                Cau_Progetto, _
                                                                Validita_Inizio, _
                                                                Validita_Fine, _
                                                                Cod_Contratto, _
                                                                Cod_Conto, _
                                                                Ricavi_Previsti, _
                                                                Produzione_Prevista, _
                                                                Giudizio, _
                                                                Veg_Cod, _
                                                                Grfi_Cod, _
                                                                CSProgetto_Cod, _
                                                                Stato_Impianto, _
                                                                Regolamento_Cod, _
                                                                Disciplinare_Cod, _
                                                                Disciplinare_PubblicoPrivato, _
                                                                Regolamento_Concimazioni_Cod, _
                                                                Data_Inizio_Prevista, _
                                                                Data_Fine_Prevista, _
                                                                P_HA, _
                                                                Data_Fioritura_Prevista)


            '#######################################################
            '################   CodiceImpianto    ##################
            '#######################################################

            'possono essere tanti nodo codice

            If Not IsNothing(DT_Codici_Progetto) AndAlso DT_Codici_Progetto.Rows.Count <> 0 Then

                Dim Id_Cod As String
                Dim Val_Cod As String
                Dim Validita_Inizio_Codice As Date
                Dim Validita_Fine_Codice As Date
                Dim TipoOperazioneDB_Codice As enum_TipoOperazioneDB

                For i = 0 To DT_Codici_Progetto.Rows.Count - 1

                    TipoOperazioneDB_Codice = DT_Codici_Progetto.Rows(i).Item("TipoOperazioneDB")

                    Id_Cod = DT_Codici_Progetto.Rows(i).Item("Id_Cod")
                    Val_Cod = DT_Codici_Progetto.Rows(i).Item("Val_Cod")
                    Validita_Inizio_Codice = DT_Codici_Progetto.Rows(i).Item("Validita_Inizio")
                    Validita_Fine_Codice = DT_Codici_Progetto.Rows(i).Item("Validita_Fine")

                    XmlProgettoCodice = XML_ProgettoImpianto_Codice(Log_Errori, _
                                                            TipoOperazioneDB_Codice, _
                                                            Piva, _
                                                            Id_Cod, _
                                                            BaseCode, _
                                                            TopCode, _
                                                            XmlDoc, _
                                                            Sa_Cod, _
                                                            Appezza, _
                                                            Id_Reg, _
                                                            Progetto_Cod, _
                                                            Val_Cod, _
                                                            Validita_Inizio_Codice, _
                                                            Validita_Fine_Codice)

                    XmlProgetto.AppendChild(XmlProgettoCodice)

                Next

            End If


        Catch ex As Exception
            Log_Errori += "XML_ProgettoImpianto. Errore durante la creazione dell'XML della Distinta dell'Impianto: " + ex.Message
        End Try


        Return XmlProgetto


    End Function



    '#################################################################################################
    '23/03/2012
    'aggiunto
    '    Optional ByVal Disciplinare_PrivatoPubblico As Integer = 0, _
    'Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0, _
    Public Function XML_ProgettoImpianto_ProgettoImpianto(ByRef Log_Errori As String, _
                                                            ByRef XmlDoc As XmlDocument, _
                                                            ByVal BaseCode As Integer, _
                                                            ByVal TopCode As Integer, _
                                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                            ByVal Piva As String, _
                                                            ByVal Sa_Cod As Integer, _
                                                            ByVal Appezza As Integer, _
                                                            ByVal Id_Reg As Integer, _
                                                            ByVal Progetto_Cod As Integer, _
                                                            ByVal Progetto_Nome As String, _
                                                            ByVal Progetto_Des As String, _
                                                            ByVal Cau_Progetto As Integer, _
                                                            ByVal Validita_Inizio As Date, _
                                                            ByVal Validita_Fine As Date, _
                                                            Optional ByVal Cod_Contratto As Integer = 0, _
                                                            Optional ByVal Cod_Conto As Integer = 0, _
                                                            Optional ByVal Ricavi_Previsti As Decimal = 0, _
                                                            Optional ByVal Produzione_Prevista As Decimal = 0, _
                                                            Optional ByVal Giudizio As String = "", _
                                                            Optional ByVal Veg_Cod As Integer = 0, _
                                                            Optional ByVal Grfi_Cod As Integer = 0, _
                                                            Optional ByVal CSProgetto_Cod As Integer = 0, _
                                                            Optional ByVal Stato_Impianto As Integer = 0, _
                                                            Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno, _
                                                            Optional ByVal Disciplinare_Cod As Integer = 0, _
                                                            Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0, _
                                                            Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0, _
                                                            Optional ByVal Data_Inizio_Prevista As Date = AGRODATAINIZIO, _
                                                            Optional ByVal Data_Fine_Prevista As Date = AGRODATAFINE, _
                                                            Optional ByVal P_HA As Decimal = 0, _
                                                            Optional ByVal Data_Fioritura_Prevista As Date = AGRODATAINIZIO) As XmlElement

        Dim XML_Progetto As System.Xml.XmlElement


        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If


            '----- Genero la stringa XML a partire dai valori dei parmetri

            XML_Progetto = XmlDoc.CreateElement("Progetto")

            aggiungiFiglio(XML_Progetto, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(XML_Progetto, XmlDoc, "piva", CStr(Piva))
            aggiungiFiglio(XML_Progetto, XmlDoc, "sa_cod", CStr(Sa_Cod))
            aggiungiFiglio(XML_Progetto, XmlDoc, "appezza", CInt(Appezza))
            aggiungiFiglio(XML_Progetto, XmlDoc, "id_reg", CInt(Id_Reg))
            aggiungiFiglio(XML_Progetto, XmlDoc, "progetto_cod", CStr(Progetto_Cod))
            aggiungiFiglio(XML_Progetto, XmlDoc, "progetto_nome", CStr(Progetto_Nome))
            aggiungiFiglio(XML_Progetto, XmlDoc, "progetto_des", CStr(Progetto_Des))
            aggiungiFiglio(XML_Progetto, XmlDoc, "cau_progetto", CInt(Cau_Progetto))

            aggiungiFiglio(XML_Progetto, XmlDoc, "cod_contratto", CInt(Cod_Contratto))
            aggiungiFiglio(XML_Progetto, XmlDoc, "cod_conto", CInt(Cod_Conto))
            aggiungiFiglio(XML_Progetto, XmlDoc, "ricavi_previsti", CDbl(Ricavi_Previsti))
            aggiungiFiglio(XML_Progetto, XmlDoc, "produzione_prevista", CDbl(Produzione_Prevista))
            aggiungiFiglio(XML_Progetto, XmlDoc, "giudizio", CStr(Giudizio))
            aggiungiFiglio(XML_Progetto, XmlDoc, "veg_cod", CInt(Veg_Cod))
            aggiungiFiglio(XML_Progetto, XmlDoc, "grfi_cod", CInt(Grfi_Cod))
            aggiungiFiglio(XML_Progetto, XmlDoc, "csprogetto_cod", CInt(CSProgetto_Cod))
            aggiungiFiglio(XML_Progetto, XmlDoc, "stato_impianto", CInt(Stato_Impianto))
            aggiungiFiglio(XML_Progetto, XmlDoc, "regolamento_cod", CInt(Regolamento_Cod))
            aggiungiFiglio(XML_Progetto, XmlDoc, "disciplinare_cod", CInt(Disciplinare_Cod))

            aggiungiFiglio(XML_Progetto, XmlDoc, "disciplinare_pubblicoprivato", CInt(Disciplinare_PubblicoPrivato))
            aggiungiFiglio(XML_Progetto, XmlDoc, "regolamento_concimazioni_cod", CInt(Regolamento_Concimazioni_Cod))

            aggiungiFiglio(XML_Progetto, XmlDoc, "p_ha", P_HA)

            aggiungiFiglio(XML_Progetto, XmlDoc, "data_inizio_prevista", CDate(Data_Inizio_Prevista))
            aggiungiFiglio(XML_Progetto, XmlDoc, "data_fioritura_prevista", CDate(Data_Fioritura_Prevista))
            aggiungiFiglio(XML_Progetto, XmlDoc, "data_fine_prevista", CDate(Data_Fine_Prevista))

            aggiungiFiglio(XML_Progetto, XmlDoc, "validita_inizio", CDate(Validita_Inizio))
            aggiungiFiglio(XML_Progetto, XmlDoc, "validita_fine", CDate(Validita_Fine))

            aggiungiFiglio(XML_Progetto, XmlDoc, "basecode", CInt(BaseCode))
            aggiungiFiglio(XML_Progetto, XmlDoc, "topcode", CInt(TopCode))

            Return XML_Progetto

            'Distruggo gli oggetti
            XML_Progetto = Nothing


        Catch ex As Exception
            Log_Errori += "XML_ProgettoImpianto_ProgettoImpianto. Errore durante la creazione dell'XML della Distinta dell'Impianto: " + ex.Message
        End Try



    End Function


    '##########################################################################################
    Public Function XML_ProgettoImpianto_Codice(ByRef Log_Errori As String, _
                                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                                ByVal Piva As String, _
                                                ByVal Id_Cod As Integer, _
                                                ByVal BaseCode As Integer, _
                                                ByVal TopCode As Integer, _
                                                Optional ByRef XmlDoc As XmlDocument = Nothing, _
                                                Optional ByVal Sa_Cod As Integer = 0, _
                                                Optional ByVal Appezza As Integer = 0, _
                                                Optional ByVal Id_Reg As Integer = 0, _
                                                Optional ByVal Progetto_Cod As Integer = 0, _
                                                Optional ByVal Val_Cod As String = "", _
                                                Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO, _
                                                Optional ByVal Validita_Fine As Date = AGRODATAFINE) _
                                                As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        Try

            If XmlDoc Is Nothing Then
                XmlDoc = New XmlDocument
            End If

            '----- Genero la stringa XML a partire dai valori dei parametri

            'Creo il nodo 
            NodoXml = XmlDoc.CreateElement("CodiceImpianto")

            'Imposto gli attributi
            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("piva"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("sa_cod"), CStr(Sa_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("appezza"), CStr(Appezza))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("id_reg"), CStr(Id_Reg))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Progetto_Cod"), CStr(Progetto_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("id_cod"), CStr(Id_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("val_cod"), CStr(Val_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("basecode"), CStr(BaseCode))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("topcode"), CStr(TopCode))

            'Restituisco in uscita 
            Return NodoXml

            'Distruggo gli oggetti
            NodoXml = Nothing

        Catch ex As Exception
            Log_Errori += "XML_Impianto_Codice. Errore durante la creazione dell'XML del codice dell'Impianto: " + ex.Message
        End Try


    End Function


    '##########################################################################################
    'Viene chiamata quando si deve creare l'xml di un solo centro
    'crea il nodo DatiCentriAziendali
    'e poi chiama la funzione XML_Particella che crea tutto il blocco della aprticella (particella, zona, macrouso ecc)
    '---------------------
    'NOTA BENE!!!
    'zone e macrousi sonod a gestire!!!!!
    Public Function XML_Particelle(ByRef Log_Errori As String, _
                                            ByRef XmlDoc As XmlDocument, _
                                            ByVal BaseCode As Integer, _
                                            ByVal TopCode As Integer, _
                                            ByVal TipoOperazioneDB As String, _
                                            ByVal Piva As String, _
                                            ByVal Sa_Cod As Integer, _
                                             ByVal Prov As String, _
                                            ByVal Com As String, _
                                            ByVal Sezione As String, _
                                            ByVal Foglio As Integer, _
                                            ByVal Numero As Integer, _
                                            ByVal Subalterno As String, _
                                            ByVal Ettari As Decimal, _
                                            ByVal Are As Integer, _
                                            ByVal Centiare As Integer, _
                                            ByVal TitoloPossesso As Integer, _
                                            ByVal Sup_Condotta As Decimal, _
                                            ByVal Validita_Inizio As Date, _
                                            ByVal Validita_Fine As Date, _
                                            ByVal Validita_Inizio_Centro As Date, _
                                            ByVal Validita_Fine_Centro As Date, _
                                            ByVal DT_ParticelleZone As DataTable, _
                                            ByVal DT_ParticelleMacrousi As DataTable, _
                                            Optional ByVal Part_cod As Integer = 0, _
                                            Optional ByVal Partita_Catastale As String = "", _
                                            Optional ByVal Qualita_Cod As Integer = 0, _
                                            Optional ByVal Classe As String = "", _
                                            Optional ByVal Reddito_Dominicale As Decimal = 0, _
                                            Optional ByVal Reddito_Agrario As Decimal = 0) As XmlElement


        Dim XmlDatiParticelle As System.Xml.XmlElement
        Dim XmlParticella As System.Xml.XmlElement


        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If



            '#######################################################
            '#########   DATI PARTICELLE CATASTALI    ################
            '#######################################################

            XmlDatiParticelle = XmlDoc.CreateElement("DatiParticelle")

            XmlDoc.AppendChild(XmlDatiParticelle)


            '#######################################################
            '################   CENTRO AZIENDALE    ################
            '#######################################################

            XmlParticella = XML_Particella(Log_Errori, _
                                            XmlDoc, _
                                            BaseCode, _
                                            TopCode, _
                                            TipoOperazioneDB, _
                                            Piva, _
                                            Sa_Cod, _
                                            Prov, _
                                            Com, _
                                            Sezione, _
                                            Foglio, _
                                            Numero, _
                                            Subalterno, _
                                            Ettari, _
                                            Are, _
                                            Centiare, _
                                            TitoloPossesso, _
                                            Sup_Condotta, _
                                            Validita_Inizio, _
                                            Validita_Fine, _
                                            Validita_Inizio_Centro, _
                                            Validita_Fine_Centro, _
                                            DT_ParticelleZone, _
                                            DT_ParticelleMacrousi, _
                                            Part_cod, _
                                            Partita_Catastale, _
                                            Qualita_Cod, _
                                            Classe, _
                                            Reddito_Dominicale, _
                                            Reddito_Agrario)


            XmlDatiParticelle.AppendChild(XmlParticella)



        Catch ex As Exception

            Log_Errori += ex.Message

        End Try


        Return XmlDatiParticelle


    End Function

    '##########################################################################################
    'crea tutto il blocco della particella (particella, zona, macrouso, ecc)
    'se si deve inserire una particella sola, conviene chiamare XML_Particelle che crea il nodo raccoglitore, prima di chiamare questa funzione
    'altrimenti, se si devono inserire più particelle, questa funzione può essere chiamata tante volte quanti sono le particelle da inserire
    Public Function XML_Particella(ByRef Log_Errori As String, _
                                            ByRef XmlDoc As XmlDocument, _
                                            ByVal BaseCode As Integer, _
                                            ByVal TopCode As Integer, _
                                            ByVal TipoOperazioneDB As String, _
                                            ByVal Piva As String, _
                                            ByVal Sa_Cod As Integer, _
                                             ByVal Prov As String, _
                                            ByVal Com As String, _
                                            ByVal Sezione As String, _
                                            ByVal Foglio As Integer, _
                                            ByVal Numero As Integer, _
                                            ByVal Subalterno As String, _
                                            ByVal Ettari As Decimal, _
                                            ByVal Are As Integer, _
                                            ByVal Centiare As Integer, _
                                            ByVal TitoloPossesso As Integer, _
                                            ByVal Sup_Condotta As Decimal, _
                                            ByVal Validita_Inizio As Date, _
                                            ByVal Validita_Fine As Date, _
                                            ByVal Validita_Inizio_Centro As Date, _
                                            ByVal Validita_Fine_Centro As Date, _
                                            ByVal DT_ParticelleZone As DataTable, _
                                            ByVal DT_ParticelleMacrousi As DataTable, _
                                            Optional ByVal Part_cod As Integer = 0, _
                                            Optional ByVal Partita_Catastale As String = "", _
                                            Optional ByVal Qualita_Cod As Integer = 0, _
                                            Optional ByVal Classe As String = "", _
                                            Optional ByVal Reddito_Dominicale As Decimal = 0, _
                                            Optional ByVal Reddito_Agrario As Decimal = 0) As XmlElement


        Dim XmlParticella As System.Xml.XmlElement
        Dim i As Integer

        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If



            '#######################################################
            '###############   PARTICELLA    #################
            '#######################################################

            XmlParticella = XML_Particella_Particella(TipoOperazioneDB, _
                                                                Piva, _
                                                                Sa_Cod, _
                                                                Prov, _
                                                                Com, _
                                                                Sezione, _
                                                                Foglio, _
                                                                Numero, _
                                                                Subalterno, _
                                                                Ettari, _
                                                                Are, _
                                                                Centiare, _
                                                                TitoloPossesso, _
                                                                Sup_Condotta, _
                                                                Validita_Inizio, _
                                                                Validita_Fine, _
                                                                Validita_Inizio_Centro, _
                                                                Validita_Fine_Centro, _
                                                                Part_cod, _
                                                                Partita_Catastale, _
                                                                Qualita_Cod, _
                                                                Classe, _
                                                                Reddito_Dominicale, _
                                                                Reddito_Agrario, _
                                                                BaseCode, _
                                                                TopCode, _
                                                                XmlDoc)




            '#######################################################
            '###################   ZONE    ######################
            '#######################################################

            'ci possono essere tanti nodi ZONA

            'If Not IsNothing(DT_Rubrica) AndAlso DT_Rubrica.Rows.Count <> 0 Then

            '    Dim TipoOperazioneDB_Rubrica As Integer
            '    Dim Cod_Rubrica As Integer
            '    Dim Numero As String
            '    Dim Descrizione As String
            '    Dim Validita_Inizio_Rubrica As Date
            '    Dim Validita_Fine_Rubrica As Date

            '    For i = 0 To DT_Rubrica.Rows.Count - 1

            '        TipoOperazioneDB_Rubrica = DT_Rubrica.Rows(i).Item("TipoOperazioneDB")

            '        Cod_Rubrica = DT_Rubrica.Rows(i).Item("Cod_Rubrica")
            '        Numero = DT_Rubrica.Rows(i).Item("Numero")
            '        Descrizione = DT_Rubrica.Rows(i).Item("Descrizione")

            '        Validita_Inizio_Rubrica = DT_Rubrica.Rows(i).Item("Validita_Inizio")
            '        Validita_Fine_Rubrica = DT_Rubrica.Rows(i).Item("Validita_Fine")

            '        XmlRubrica = XML_2_Rubrica(TipoOperazioneDB_Rubrica, _
            '                                   Cod_Rubrica, _
            '                                    Numero, _
            '                                    Descrizione, _
            '                                    XmlDoc, _
            '                                    Validita_Inizio_Rubrica, _
            '                                    Validita_Fine_Rubrica, _
            '                                    BaseCode, _
            '                                    TopCode)

            '        XmlCentroAziendale.AppendChild(XmlRubrica)

            '    Next

            'End If


            '#######################################################
            '################   MACROUSI   ###################
            '#######################################################

            'possono essere tanti nodo MACROUSO

            'If Not IsNothing(Dt_Codici) AndAlso Dt_Codici.Rows.Count <> 0 Then

            '    Dim Id_Cod As String
            '    Dim Val_Cod As String
            '    Dim Validita_Inizio_Codice As Date
            '    Dim Validita_Fine_Codice As Date
            '    Dim TipoOperazioneDB_Codice As enum_TipoOperazioneDB

            '    For i = 0 To Dt_Codici.Rows.Count - 1

            '        TipoOperazioneDB_Codice = Dt_Codici.Rows(i).Item("TipoOperazioneDB")

            '        Id_Cod = Dt_Codici.Rows(i).Item("Id_Cod")
            '        Val_Cod = Dt_Codici.Rows(i).Item("Val_Cod")
            '        Validita_Inizio_Codice = Dt_Codici.Rows(i).Item("Validita_Inizio")
            '        Validita_Fine_Codice = Dt_Codici.Rows(i).Item("Validita_Fine")

            '        XmlCentroCodice = XML_2_Codice(TipoOperazioneDB_Codice, _
            '                                        Id_Cod, _
            '                                        BaseCode, _
            '                                        TopCode, _
            '                                        XmlDoc, _
            '                                        Val_Cod, _
            '                                        Validita_Inizio_Codice, _
            '                                        Validita_Fine_Codice)

            '        XmlCentroAziendale.AppendChild(XmlCentroCodice)

            '    Next

            'End If



        Catch ex As Exception

            Log_Errori += ex.Message

        End Try


        Return XmlParticella


    End Function

    '############################################################################
    Public Function XML_Particella_Particella(ByVal TipoOperazioneDB As String, _
                                            ByVal Piva As String, _
                                            ByVal Sa_Cod As Integer, _
                                             ByVal Prov As String, _
                                            ByVal Com As String, _
                                            ByVal Sezione As String, _
                                            ByVal Foglio As Integer, _
                                            ByVal Numero As Integer, _
                                            ByVal Subalterno As String, _
                                            ByVal Ettari As Decimal, _
                                            ByVal Are As Integer, _
                                            ByVal Centiare As Integer, _
                                            ByVal TitoloPossesso As Integer, _
                                            ByVal Sup_Condotta As Decimal, _
                                            ByVal Validita_Inizio As Date, _
                                            ByVal Validita_Fine As Date, _
                                            ByVal Validita_Inizio_Centro As Date, _
                                            ByVal Validita_Fine_Centro As Date, _
                                            Optional ByVal Part_cod As Integer = 0, _
                                            Optional ByVal Partita_Catastale As String = "", _
                                            Optional ByVal Qualita_Cod As Integer = 0, _
                                            Optional ByVal Classe As String = "", _
                                            Optional ByVal Reddito_Dominicale As Decimal = 0, _
                                            Optional ByVal Reddito_Agrario As Decimal = 0, _
                                            Optional ByVal BaseCode As Integer = 0, _
                                            Optional ByVal TopCode As Integer = 200000000, _
                                            Optional ByRef XmlDoc As XmlDocument = Nothing, _
                                            Optional ByVal casiParticolari As String = "", _
                                            Optional ByVal fasciaAltimetrica As String = "", _
                                            Optional ByVal fasciaAltimetricaDescr As String = "", _
                                            Optional ByVal Fonte As String = "", _
                                            Optional ByVal FonteDescr As String = "", _
                                            Optional ByVal tipoDocumento As String = "", _
                                            Optional ByVal tipoDocumentoDescr As String = "", _
                                            Optional ByVal utilizzo As String = "", _
                                            Optional ByVal IDParticellaOrig As String = "", _
                                            Optional ByVal Irrigabilita As String = "", _
                                            Optional ByVal RotazioneColturale As String = "", _
                                            Optional ByVal biologico As String = "", _
                                            Optional ByVal flagAnomaliaMacrouso As String = "", _
                                            Optional ByVal flagContenzioso As String = "", _
                                            Optional ByVal flagSupero As String = "", _
                                            Optional ByVal percEleggibile As String = "" _
                                            ) As XmlElement

        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Particella")


        'Imposto gli attributi
        aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
        aggiungiFiglio(NodoXml, XmlDoc, "piva", CStr(Piva))
        aggiungiFiglio(NodoXml, XmlDoc, "sa_cod", CStr(Sa_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, "part_cod", CStr(Part_cod))
        aggiungiFiglio(NodoXml, XmlDoc, "prov", CStr(Prov))
        aggiungiFiglio(NodoXml, XmlDoc, "com", CStr(Com))
        aggiungiFiglio(NodoXml, XmlDoc, "sezione", CStr(Sezione))
        aggiungiFiglio(NodoXml, XmlDoc, "foglio", CStr(Foglio))
        aggiungiFiglio(NodoXml, XmlDoc, "numero", CStr(Numero))
        aggiungiFiglio(NodoXml, XmlDoc, "subalterno", CStr(Subalterno))
        aggiungiFiglio(NodoXml, XmlDoc, "partita_catastale", CStr(Partita_Catastale))
        aggiungiFiglio(NodoXml, XmlDoc, "ettari", CStr(Ettari))
        aggiungiFiglio(NodoXml, XmlDoc, "are", CStr(Are))
        aggiungiFiglio(NodoXml, XmlDoc, "centiare", CStr(Centiare))
        aggiungiFiglio(NodoXml, XmlDoc, "titolopossesso", CStr(TitoloPossesso))
        aggiungiFiglio(NodoXml, XmlDoc, "qualita_cod", CStr(Qualita_Cod))
        aggiungiFiglio(NodoXml, XmlDoc, "classe", CStr(Classe))
        aggiungiFiglio(NodoXml, XmlDoc, "reddito_dominicale", CStr(Reddito_Dominicale))
        aggiungiFiglio(NodoXml, XmlDoc, "reddito_agrario", CStr(Reddito_Agrario))
        aggiungiFiglio(NodoXml, XmlDoc, "sup_condotta", CStr(Sup_Condotta))
        aggiungiFiglio(NodoXml, XmlDoc, "validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "validita_inizio_centro", Format(Validita_Inizio_Centro, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "validita_fine_centro", Format(Validita_Fine_Centro, "dd/MM/yyyy"))
        aggiungiFiglio(NodoXml, XmlDoc, "basecode", CStr(BaseCode))
        aggiungiFiglio(NodoXml, XmlDoc, "topcode", CStr(TopCode))
        aggiungiFiglio(NodoXml, XmlDoc, "casiParticolari", CStr(casiParticolari))
        aggiungiFiglio(NodoXml, XmlDoc, "fasciaAltimetrica", CStr(fasciaAltimetrica))
        aggiungiFiglio(NodoXml, XmlDoc, "fasciaAltimetricaDescr", CStr(fasciaAltimetricaDescr))
        aggiungiFiglio(NodoXml, XmlDoc, "Fonte", CStr(Fonte))
        aggiungiFiglio(NodoXml, XmlDoc, "FonteDescr", CStr(FonteDescr))
        aggiungiFiglio(NodoXml, XmlDoc, "tipoDocumento", CStr(tipoDocumento))
        aggiungiFiglio(NodoXml, XmlDoc, "tipoDocumentoDescr", CStr(tipoDocumentoDescr))
        aggiungiFiglio(NodoXml, XmlDoc, "utilizzo", CStr(utilizzo))
        aggiungiFiglio(NodoXml, XmlDoc, "IDParticellaOrig", CStr(IDParticellaOrig))
        aggiungiFiglio(NodoXml, XmlDoc, "Irrigabilita", CStr(Irrigabilita))
        aggiungiFiglio(NodoXml, XmlDoc, "RotazioneColturale", CStr(RotazioneColturale))
        aggiungiFiglio(NodoXml, XmlDoc, "biologico", CStr(biologico))
        aggiungiFiglio(NodoXml, XmlDoc, "flagAnomaliaMacrouso", CStr(flagAnomaliaMacrouso))
        aggiungiFiglio(NodoXml, XmlDoc, "flagContenzioso", CStr(flagContenzioso))
        aggiungiFiglio(NodoXml, XmlDoc, "flagSupero", CStr(flagSupero))
        aggiungiFiglio(NodoXml, XmlDoc, "percEleggibile", CStr(percEleggibile))
        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing



    End Function


    Public Sub XML_Campo_3(ByVal OperazioneRichiesta As enum_CodificaDecodifica, _
                            ByRef StringaXML As String, _
                            ByRef TipoOperazioneDB As enum_TipoOperazioneDB, _
                            ByRef Piva As String, _
                            ByRef Sa_Cod As Integer, _
                            ByRef Campo_Cod As Long, _
                            ByRef Campo_tipo As Integer, _
                            ByRef Campo_Des As String, _
                            ByRef Conversione_Inizio As Date, _
                            ByRef Conversione_Fine As Date, _
                            ByRef SAU_Totale As Decimal, _
                            ByRef SAU_Biologico As Decimal, _
                            ByRef SAU_Conversione As Decimal, _
                            ByRef SAU_Convenzionale As Decimal, _
                            ByRef ConfiniRischio As String, _
                            ByRef Gru_Cod As Integer, _
                            ByRef Veg_Cod As Integer, _
                            ByRef Validita_Inizio As Date, _
                            ByRef Validita_Fine As Date, _
                            ByRef BaseCode As Integer, _
                            ByRef TopCode As Integer)

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlCampo As System.Xml.XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                XmlCampo = XmlDoc.CreateElement("Campo")

                'Imposto gli attributi
                aggiungiFiglio(XmlCampo, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
                aggiungiFiglio(XmlCampo, XmlDoc, "piva", CStr(Piva))
                aggiungiFiglio(XmlCampo, XmlDoc, "sa_cod", CStr(Sa_Cod))
                aggiungiFiglio(XmlCampo, XmlDoc, "campo_cod", CStr(Campo_Cod))
                aggiungiFiglio(XmlCampo, XmlDoc, "campo_tipo", CStr(Campo_tipo))
                aggiungiFiglio(XmlCampo, XmlDoc, "campo_des", Campo_Des)
                aggiungiFiglio(XmlCampo, XmlDoc, "conversione_inizio", CStr(Conversione_Inizio))
                aggiungiFiglio(XmlCampo, XmlDoc, "conversione_fine", CStr(Conversione_Fine))
                aggiungiFiglio(XmlCampo, XmlDoc, "sau_totale", CStr(SAU_Totale))
                aggiungiFiglio(XmlCampo, XmlDoc, "sau_biologico", CStr(SAU_Biologico))
                aggiungiFiglio(XmlCampo, XmlDoc, "sau_conversione", CStr(SAU_Conversione))
                aggiungiFiglio(XmlCampo, XmlDoc, "sau_convenzionale", CStr(SAU_Convenzionale))
                aggiungiFiglio(XmlCampo, XmlDoc, "confinirischio", CStr(ConfiniRischio))
                aggiungiFiglio(XmlCampo, XmlDoc, "gru_cod", CStr(Gru_Cod))
                aggiungiFiglio(XmlCampo, XmlDoc, "veg_cod", CStr(Veg_Cod))
                aggiungiFiglio(XmlCampo, XmlDoc, "validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                aggiungiFiglio(XmlCampo, XmlDoc, "validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                aggiungiFiglio(XmlCampo, XmlDoc, "basecode", CStr(BaseCode))
                aggiungiFiglio(XmlCampo, XmlDoc, "topcode", CStr(TopCode))


                'Imposto XmlIndirizzo come figlio del documento principale
                XmlDoc.AppendChild(XmlCampo)

                'Restituisco in uscita la stringa creata
                StringaXML = XmlDoc.InnerXml

                'Distruggo gli oggetti
                XmlCampo = Nothing
                XmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                XmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                XmlCampo = XmlDoc.SelectSingleNode("//Campo")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(XmlCampo.GetAttribute("TipoOperazioneDB"))
                Campo_Cod = CInt(XmlCampo.GetAttribute("campo_cod"))
                Campo_Des = CStr(XmlCampo.GetAttribute("campo_des"))
                Campo_tipo = CStr(XmlCampo.GetAttribute("campo_tipo"))
                Gru_Cod = CInt(XmlCampo.GetAttribute("gru_cod"))
                Veg_Cod = CInt(XmlCampo.GetAttribute("veg_cod"))
                Validita_Inizio = CDate(XmlCampo.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(XmlCampo.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0


                'Distruggo gli oggetti
                XmlCampo = Nothing
                XmlDoc = Nothing

        End Select

    End Sub

    Public Function XML_2_Zoo_Animali( _
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                ByVal PIVA As String, _
                                ByVal sa_cod As Int32, _
                                ByVal Cod_Progetto As Int32, _
                                ByVal Matricola As String, _
                                ByVal GEN_COD As Int32, _
                                ByVal SPE_COD As Int32, _
                                ByVal IPRO_COD As Int32, _
                                ByVal RAZ_COD As Int32, _
                                ByVal Nome As String, _
                                ByVal Collare As String, _
                                ByVal NOME_AIA As String, _
                                ByVal MATRICOLA_AIA As String, _
                                ByVal DAT_NASCITA As String, _
                                ByVal PROV_NASCITA As String, _
                                ByVal STATO_NASCITA As String, _
                                ByVal AUA_AZI_NASCITA As String, _
                                ByVal AUSL_AZI_NASCITA As String, _
                                ByVal Sesso As String, _
                                ByVal MAT_PADRE As String, _
                                ByVal MAT_MADRE As String, _
                                ByVal CF_PROPRIETARIO As String, _
                                ByVal CF_DETENTORE As String, _
                                ByVal PRESENTE As Int32, _
                                ByVal inviato As Int32, _
                                ByVal datainvio As DateTime, _
                                ByVal Data_Creazione As DateTime, _
                                ByVal Data_Modifica As DateTime, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                                ByVal CAT_COD As Int32, _
                                ByVal PESO As Double, _
                                ByVal DATA_PESA As DateTime, _
                                ByVal Metodo_Produzione As Int32, _
                                ByVal Regolamento_Cod As Int32, _
                                ByVal Conversione_Inizio As DateTime, _
                                ByVal Conversione_Fine As DateTime, _
                                ByVal Chk_Batteria As Int32, _
                                ByVal codZootecnica As String, _
                                ByVal descrZootecnica As String, _
                                ByVal fonte As String, _
                                ByVal ID_Utente As String, _
                                ByVal DT_Variazione As DateTime, _
                                Optional ByRef XmlDoc As XmlDocument = Nothing _
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("ZooAnimali")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Piva"), CStr(PIVA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("sa_cod"), CStr(sa_cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cod_Progetto"), CStr(Cod_Progetto))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Matricola"), CStr(Matricola))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("GEN_COD"), CStr(GEN_COD))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("SPE_COD"), CStr(SPE_COD))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("IPRO_COD"), CStr(IPRO_COD))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("RAZ_COD"), CStr(RAZ_COD))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Nome"), CStr(Nome))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Collare"), CStr(Collare))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("NOME_AIA"), CStr(NOME_AIA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("MATRICOLA_AIA"), CStr(MATRICOLA_AIA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("DAT_NASCITA"), CStr(DAT_NASCITA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("PROV_NASCITA"), CStr(PROV_NASCITA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("STATO_NASCITA"), CStr(STATO_NASCITA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("AUA_AZI_NASCITA"), CStr(AUA_AZI_NASCITA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("AUSL_AZI_NASCITA"), CStr(AUSL_AZI_NASCITA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Sesso"), CStr(Sesso))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("MAT_PADRE"), CStr(MAT_PADRE))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("MAT_MADRE"), CStr(MAT_MADRE))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("CF_PROPRIETARIO"), CStr(CF_PROPRIETARIO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("CF_DETENTORE"), CStr(CF_DETENTORE))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("PRESENTE"), CStr(PRESENTE))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("inviato"), CStr(inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("datainvio"), CStr(datainvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Creazione"), CStr(Data_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Modifica"), CStr(Data_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), CStr(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), CStr(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), CStr(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), CStr(Validita_Fine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("CAT_COD"), CStr(CAT_COD))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("PESO"), CStr(PESO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("DATA_PESA"), CStr(DATA_PESA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Metodo_Produzione"), CStr(Metodo_Produzione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Regolamento_Cod"), CStr(Regolamento_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Conversione_Inizio"), CStr(Conversione_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Conversione_Fine"), CStr(Conversione_Fine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Chk_Batteria"), CStr(Chk_Batteria))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("codZootecnica"), CStr(codZootecnica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("descrZootecnica"), CStr(descrZootecnica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("fonte"), CStr(fonte))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("ID_Utente"), CStr(ID_Utente))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("DT_Variazione"), CStr(DT_Variazione))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_Imprese_Contratti( _
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                ByVal Piva As String, _
                                ByVal Contratto_Cod As String, _
                                ByVal Riferimento As String, _
                                ByVal Superficie_Prevista As String, _
                                ByVal Resa_Prevista As String, _
                                ByVal Contratto_Nome As String, _
                                ByVal Contratto_Numero As String, _
                                ByVal Contratto_Des As String, _
                                ByVal Cau_Contratto As String, _
                                ByVal Cod_Conto As String, _
                                ByVal Data_Inizio_Prevista As String, _
                                ByVal Data_Fine_Prevista As String, _
                                ByVal Descrizione_1 As String, _
                                ByVal Descrizione_2 As String, _
                                ByVal Giudizio As String, _
                                ByVal Validita_Inizio As String, _
                                ByVal Validita_Fine As String, _
                                ByVal Inviato As String, _
                                ByVal DataInvio As String, _
                                ByVal Data_Creazione As String, _
                                ByVal Data_Modifica As String, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Ricavi_Previsti As String, _
                                ByVal Cod_Risum As String, _
                                ByVal Stato As String, _
                                ByVal ChkStato_Automatico As String, _
                                ByVal Cau_Pagamento As String, _
                                ByVal Data_Stipulazione As String, _
                                ByVal Data_Variazione As DateTime, _
                                ByVal prog As Int32, _
                                ByVal id_doc As Int32, _
                                ByVal id_contratto As Int32, _
                                Optional ByRef XmlDoc As XmlDocument = Nothing _
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("ImpreseContratti")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Piva"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Contratto_Cod"), CStr(Contratto_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Riferimento"), CStr(Riferimento))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Superficie_Prevista"), CStr(Superficie_Prevista))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Resa_Prevista"), CStr(Resa_Prevista))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Contratto_Nome"), CStr(Contratto_Nome))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Contratto_Numero"), CStr(Contratto_Numero))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Contratto_Des"), CStr(Contratto_Des))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cau_Contratto"), CStr(Cau_Contratto))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cod_Conto"), CStr(Cod_Conto))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Inizio_Prevista"), CStr(Data_Inizio_Prevista))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Fine_Prevista"), CStr(Data_Fine_Prevista))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Descrizione_1"), CStr(Descrizione_1))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Descrizione_2"), CStr(Descrizione_2))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Giudizio"), CStr(Giudizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), CStr(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), CStr(Validita_Fine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Inviato"), CStr(Inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("DataInvio"), CStr(DataInvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Creazione"), CStr(Data_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Modifica"), CStr(Data_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), CStr(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), CStr(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Ricavi_Previsti"), CStr(Ricavi_Previsti))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cod_Risum"), CStr(Cod_Risum))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Stato"), CStr(Stato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("ChkStato_Automatico"), CStr(ChkStato_Automatico))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cau_Pagamento"), CStr(Cau_Pagamento))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Stipulazione"), CStr(Data_Stipulazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("prog"), CStr(prog))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("id_doc"), CStr(id_doc))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("id_contratto"), CStr(id_contratto))
            'aggiungiFiglio(NodoXml, XmlDoc, LCase("dt_Inizio_Rinnovo"), CStr(dt_Inizio_Rinnovo))
            'aggiungiFiglio(NodoXml, XmlDoc, LCase("dt_Fine_Rinnovo"), CStr(dt_Fine_Rinnovo))
            'aggiungiFiglio(NodoXml, XmlDoc, LCase("prog_Rinnovato"), CStr(prog_Rinnovato))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_DirittiReimpianti( _
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                ByVal PIVA As String, _
                                ByVal codAutorizzazione As String, _
                                ByVal dataProtocollo As Date, _
                                ByVal dataRilascio As Date, _
                                ByVal dataTermine As Date, _
                                ByVal dtIns As Date, _
                                ByVal dtVar As Date, _
                                ByVal numDiritto As Integer, _
                                ByVal numeroProtocollo As String, _
                                ByVal praticaAutorizzata As String, _
                                ByVal provRilascio As String, _
                                ByVal provRilascioDescr As String, _
                                ByVal provRilascioSigla As String, _
                                ByVal supAutorizzata As Integer, _
                                ByVal supImpiantata As Integer, _
                                ByVal supResidua As Integer, _
                                ByVal tipoDiritto As String, _
                                ByVal tipoDirittoDescr As String, _
                                ByVal tipoProcedimento As String, _
                                ByVal inviato As Int32, _
                                ByVal datainvio As DateTime, _
                                ByVal Data_Creazione As DateTime, _
                                ByVal Data_Modifica As DateTime, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                                Optional ByRef XmlDoc As XmlDocument = Nothing _
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("DirittiReimpianti")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("PIVA"), CStr(PIVA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("codAutorizzazione"), CStr(codAutorizzazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dataProtocollo"), CStr(dataProtocollo))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dataRilascio"), CStr(dataRilascio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dataTermine"), CStr(dataTermine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dtIns"), CStr(dtIns))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dtVar"), CStr(dtVar))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("numDiritto"), CStr(numDiritto))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("numeroProtocollo"), CStr(numeroProtocollo))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("praticaAutorizzata"), CStr(praticaAutorizzata))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("provRilascio"), CStr(provRilascio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("provRilascioDescr"), CStr(provRilascioDescr))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("provRilascioSigla"), CStr(provRilascioSigla))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("supAutorizzata"), CStr(supAutorizzata))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("supImpiantata"), CStr(supImpiantata))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("supResidua"), CStr(supResidua))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("tipoDiritto"), CStr(tipoDiritto))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("tipoDirittoDescr"), CStr(tipoDirittoDescr))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("tipoProcedimento"), CStr(tipoProcedimento))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("inviato"), CStr(inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("datainvio"), CStr(datainvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Creazione"), CStr(Data_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Modifica"), CStr(Data_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), CStr(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), CStr(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), CStr(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), CStr(Validita_Fine))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_Liquidita( _
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Integer, _
                                ByVal Cod_Liquidita As Integer, _
                                ByVal Riferimento As String, _
                                ByVal Numero As String, _
                                ByVal Abi As String, _
                                ByVal Cab As String, _
                                ByVal Interbancario As String, _
                                ByVal Saldo_Attuale As Double, _
                                ByVal Saldo_Iniziale As Double, _
                                ByVal Cau_Risorsa As String, _
                                ByVal Cod_Istituto As Integer, _
                                ByVal Avviso As String, _
                                ByVal Importo_Avviso As Double, _
                                ByVal Note As String, _
                                ByVal inviato As Int32, _
                                ByVal datainvio As DateTime, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                                ByVal Cin As String, _
                                ByVal Cifre_Controllo As String, _
                                ByVal Nazione As String, _
                                ByVal Bic As String, _
                                ByVal Cod_Contatto As String, _
                                ByVal Rilevamento As Double, _
                                ByVal Data_Rilevamento As DateTime, _
                                ByVal Offset As Double, _
                                ByVal ChkDefault As Int32, _
                                ByVal ChkAbilitazione As Int32, _
                                ByVal anomalo As String, _
                                ByVal codAnomalia As String, _
                                ByVal dataAnomalia As String, _
                                ByVal dataFine As String, _
                                ByVal dataFineSportello As String, _
                                ByVal dataInizioSportello As String, _
                                ByVal dataInserimento As String, _
                                ByVal DataVariazione As String, _
                                ByVal descrAnomalia As String, _
                                ByVal flagTesoriere As String, _
                                ByVal Preferito As String, _
                                ByVal progr As String, _
                                Optional ByRef XmlDoc As XmlDocument = Nothing _
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Liquidita")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Piva"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Sa_Cod"), CStr(Sa_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cod_Liquidita"), CStr(Cod_Liquidita))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Riferimento"), CStr(Riferimento))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Numero"), CStr(Numero))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Abi"), CStr(Abi))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cab"), CStr(Cab))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Interbancario"), CStr(Interbancario))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Saldo_Attuale"), CStr(Saldo_Attuale))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Saldo_Iniziale"), CStr(Saldo_Iniziale))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cau_Risorsa"), CStr(Cau_Risorsa))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cod_Istituto"), CStr(Cod_Istituto))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Avviso"), CStr(Avviso))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Importo_Avviso"), CStr(Importo_Avviso))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Note"), CStr(Note))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Inviato"), CStr(inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("DataInvio"), CStr(datainvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), CStr(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), CStr(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), CStr(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), CStr(Validita_Fine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cin"), CStr(Cin))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cifre_Controllo"), CStr(Cifre_Controllo))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Nazione"), CStr(Nazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Bic"), CStr(Bic))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cod_Contatto"), CStr(Cod_Contatto))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Rilevamento"), CStr(Rilevamento))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Rilevamento"), CStr(Data_Rilevamento))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Offset"), CStr(Offset))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("ChkDefault"), CStr(ChkDefault))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("ChkAbilitazione"), CStr(ChkAbilitazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("anomalo"), CStr(anomalo))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("codAnomalia"), CStr(codAnomalia))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dataAnomalia"), CStr(dataAnomalia))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dataFine"), CStr(dataFine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dataFineSportello"), CStr(dataFineSportello))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dataInizioSportello"), CStr(dataInizioSportello))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dataInserimento"), CStr(dataInserimento))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("DataVariazione"), CStr(DataVariazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("descrAnomalia"), CStr(descrAnomalia))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("flagTesoriere"), CStr(flagTesoriere))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Preferito"), CStr(Preferito))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("progr"), CStr(progr))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_Ist_Credito( _
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As String, _
                                ByVal Cod_Istituto As String, _
                                ByVal Istituto_Des As String, _
                                ByVal Filiale As String, _
                                ByVal Per_Risorsa As String, _
                                ByVal Inviato As String, _
                                ByVal DataInvio As String, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Validita_Inizio As String, _
                                ByVal Validita_Fine As String, _
                                ByVal cod_Contatto As String, _
                                Optional ByRef XmlDoc As XmlDocument = Nothing _
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Ist_Credito")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Piva"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Sa_Cod"), CStr(Sa_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cod_Istituto"), CStr(Cod_Istituto))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Istituto_Des"), CStr(Istituto_Des))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Filiale"), CStr(Filiale))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Per_Risorsa"), CStr(Per_Risorsa))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Inviato"), CStr(Inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("DataInvio"), CStr(DataInvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), CStr(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), CStr(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), CStr(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), CStr(Validita_Fine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("cod_Contatto"), CStr(cod_Contatto))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_IscrizioneCAA( _
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                ByVal Piva As String, _
                                ByVal Denominazione As String, _
                                ByVal codFiscaleCAA As String, _
                                ByVal idCAA As String, _
                                ByVal Cod_Indirizzo As String, _
                                ByVal inviato As String, _
                                ByVal datainvio As DateTime, _
                                ByVal Data_Creazione As DateTime, _
                                ByVal Data_Modifica As DateTime, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                                Optional ByRef XmlDoc As XmlDocument = Nothing _
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("IscrizioneCAA")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Piva"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Denominazione"), CStr(Denominazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("codFiscaleCAA"), CStr(codFiscaleCAA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("idCAA"), CStr(idCAA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cod_Indirizzo"), CStr(Cod_Indirizzo))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("inviato"), CStr(inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("datainvio"), CStr(datainvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Creazione"), CStr(Data_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Modifica"), CStr(Data_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), CStr(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), CStr(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), CStr(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), CStr(Validita_Fine))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_RisorseUmane( _
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal Cod_RisUm As Int32, _
                                ByVal Cod_Contatto As String, _
                                ByVal Cod_Rapporto As Int32, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                                ByVal Settore_Des As String, _
                                ByVal Attivita_Des As String, _
                                ByVal Corrispettivo_Mensile As Double, _
                                ByVal Corrispettivo_Orario As Double, _
                                ByVal Occasionale As Int32, _
                                ByVal Ore_Settimanali As Double, _
                                ByVal Giorni_Ferie As Int32, _
                                ByVal Ferie_Godute As Int32, _
                                ByVal Giorni_Malattia As Int32, _
                                ByVal Inviato As Int32, _
                                ByVal DataInvio As DateTime, _
                                ByVal Data_Creazione As DateTime, _
                                ByVal Data_Modifica As DateTime, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Patentino As String, _
                                ByVal Data_Rilascio_Patentinvo As DateTime, _
                                ByVal Data_Scadenza_Patentino As DateTime, _
                                ByVal Cod_RisUm_Origine As Int32, _
                                ByVal Piva_SuperUser_Origine As String, _
                                ByVal Ente_di_rilascio As String, _
                                ByVal Saldo_Iniziale_Crediti As Double, _
                                ByVal Saldo_Iniziale_Debiti As Double, _
                                ByVal ChkSpesometro As Int32, _
                                ByVal ChkBlocco As Int32, _
                                ByVal Blocco_Des As String, _
                                ByVal Qualifica_Cod As Int32, _
                                ByVal Mansione_Cod As Int32, _
                                ByVal Info_Famiglia As String, _
                                ByVal codRuolo As String, _
                                ByVal ruoloDescr As String, _
                                ByVal dtVariazioneRuolo As Date, _
                                ByVal fonte As String, _
                                ByVal fonteDescr As String, _
                                Optional ByRef XmlDoc As XmlDocument = Nothing _
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Risorse_Umane")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Piva"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Sa_Cod"), CStr(Sa_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cod_RisUm"), CStr(Cod_RisUm))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cod_Contatto"), CStr(Cod_Contatto))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cod_Rapporto"), CStr(Cod_Rapporto))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), CStr(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), CStr(Validita_Fine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Settore_Des"), CStr(Settore_Des))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Attivita_Des"), CStr(Attivita_Des))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Corrispettivo_Mensile"), CStr(Corrispettivo_Mensile))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Corrispettivo_Orario"), CStr(Corrispettivo_Orario))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Occasionale"), CStr(Occasionale))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Ore_Settimanali"), CStr(Ore_Settimanali))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Giorni_Ferie"), CStr(Giorni_Ferie))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Ferie_Godute"), CStr(Ferie_Godute))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Giorni_Malattia"), CStr(Giorni_Malattia))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Inviato"), CStr(Inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("DataInvio"), CStr(DataInvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Creazione"), CStr(Data_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Modifica"), CStr(Data_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), CStr(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), CStr(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Patentino"), CStr(Patentino))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Rilascio_Patentinvo"), CStr(Data_Rilascio_Patentinvo))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Scadenza_Patentino"), CStr(Data_Scadenza_Patentino))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cod_RisUm_Origine"), CStr(Cod_RisUm_Origine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Piva_SuperUser_Origine"), CStr(Piva_SuperUser_Origine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Ente_di_rilascio"), CStr(Ente_di_rilascio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Saldo_Iniziale_Crediti"), CStr(Saldo_Iniziale_Crediti))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Saldo_Iniziale_Debiti"), CStr(Saldo_Iniziale_Debiti))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("ChkSpesometro"), CStr(ChkSpesometro))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("ChkBlocco"), CStr(ChkBlocco))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Blocco_Des"), CStr(Blocco_Des))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Qualifica_Cod"), CStr(Qualifica_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Mansione_Cod"), CStr(Mansione_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Info_Famiglia"), CStr(Info_Famiglia))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("codRuolo"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("ruoloDescr"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dtVariazioneRuolo"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("fonte"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("fonteDescr"), CStr(Piva))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_ProduzioniQualita(
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                ByVal Piva As String, _
                                ByVal CodMacroarea As String, _
                                ByVal CodProduzione As String, _
                                ByVal DescrMacroarea As String, _
                                ByVal DescrProduzione As String, _
                                ByVal Fonte As DateTime, _
                                ByVal ID_Utente As DateTime, _
                                ByVal DT_Variazione As String, _
                                ByVal Inviato As Int32, _
                                ByVal DataInvio As DateTime, _
                                ByVal Data_Creazione As DateTime, _
                                ByVal Data_Modifica As DateTime, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                                Optional ByRef XmlDoc As XmlDocument = Nothing _
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("ProduzioniQualita")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Piva"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("CodMacroarea"), CStr(CodMacroarea))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("CodProduzione"), CStr(CodProduzione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("DescrMacroarea"), CStr(DescrMacroarea))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("DescrProduzione"), CStr(DescrProduzione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Fonte"), CStr(Fonte))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("ID_Utente"), CStr(ID_Utente))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("DT_Variazione"), CStr(DT_Variazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Inviato"), CStr(Inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("DataInvio"), CStr(DataInvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Creazione"), CStr(Data_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Modifica"), CStr(Data_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), CStr(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), CStr(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), CStr(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), CStr(Validita_Fine))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_CentriAziendalixParticelle(
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                ByVal ID As Int32, _
                                ByVal PIVA As String, _
                                ByVal sa_cod As Int32, _
                                ByVal PROV As String, _
                                ByVal COM As String, _
                                ByVal SEZIONE As String, _
                                ByVal FOGLIO As Int32, _
                                ByVal NUMERO As Int32, _
                                ByVal SUBALTERNO As String, _
                                ByVal TitoloPossesso As Int32, _
                                ByVal PARTITA_CATASTALE As String, _
                                ByVal inviato As Int32, _
                                ByVal datainvio As DateTime, _
                                ByVal Data_Creazione As DateTime, _
                                ByVal Data_Modifica As DateTime, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                                ByVal Validazione As Int32, _
                                ByVal Data_Validazione As DateTime, _
                                ByVal UserName_Validazione As String, _
                                ByVal Sup_Condotta As Double, _
                                ByVal Sup_Spandibile As Double, _
                                ByVal Sup_Divieto As Double, _
                                ByVal Irrigabilita As String, _
                                ByVal RotazioneColturale As String, _
                                ByVal biologico As String, _
                                ByVal flagAnomaliaMacrouso As String, _
                                ByVal flagContenzioso As String, _
                                ByVal flagSupero As String, _
                                Optional ByRef XmlDoc As XmlDocument = Nothing _
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("ImpresexParticelle")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("ID"), CStr(ID))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("PIVA"), CStr(PIVA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("sa_cod"), CStr(sa_cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("PROV"), CStr(PROV))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("COM"), CStr(COM))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("SEZIONE"), CStr(SEZIONE))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("FOGLIO"), CStr(FOGLIO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("NUMERO"), CStr(NUMERO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("SUBALTERNO"), CStr(SUBALTERNO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("TitoloPossesso"), CStr(TitoloPossesso))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("PARTITA_CATASTALE"), CStr(PARTITA_CATASTALE))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("inviato"), CStr(inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("datainvio"), CStr(datainvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Creazione"), CStr(Data_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Modifica"), CStr(Data_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), CStr(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), CStr(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), CStr(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), CStr(Validita_Fine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validazione"), CStr(Validazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Validazione"), CStr(Data_Validazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("UserName_Validazione"), CStr(UserName_Validazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Sup_Condotta"), CStr(Sup_Condotta))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Sup_Spandibile"), CStr(Sup_Spandibile))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Sup_Divieto"), CStr(Sup_Divieto))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Irrigabilita"), CStr(Irrigabilita))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("RotazioneColturale"), CStr(RotazioneColturale))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("biologico"), CStr(biologico))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("flagAnomaliaMacrouso"), CStr(flagAnomaliaMacrouso))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("flagContenzioso"), CStr(flagContenzioso))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("flagSupero"), CStr(flagSupero))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_ParticelleCatastalixEleggiblitaParticelle(
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                ByVal PROV As String, _
                                ByVal COM As String, _
                                ByVal SEZIONE As String, _
                                ByVal FOGLIO As Int32, _
                                ByVal NUMERO As Int32, _
                                ByVal SUBALTERNO As String, _
                                ByVal Eleggibilita_Cod As Int32, _
                                ByVal Superficie As Double, _
                                ByVal inviato As Int32, _
                                ByVal datainvio As DateTime, _
                                ByVal Data_Creazione As DateTime, _
                                ByVal Data_Modifica As DateTime, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                                ByVal percEleggibile As String, _
                                Optional ByRef XmlDoc As XmlDocument = Nothing _
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("ParticelleCatastalixEleggiblitaParticelle")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("PROV"), CStr(PROV))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("COM"), CStr(COM))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("SEZIONE"), CStr(SEZIONE))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("FOGLIO"), CStr(FOGLIO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("NUMERO"), CStr(NUMERO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("SUBALTERNO"), CStr(SUBALTERNO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Eleggibilita_Cod"), CStr(Eleggibilita_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Superficie"), CStr(Superficie))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("inviato"), CStr(inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("datainvio"), CStr(datainvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Creazione"), CStr(Data_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Modifica"), CStr(Data_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), CStr(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), CStr(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), CStr(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), CStr(Validita_Fine))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_Imprese_Contratto_Fasi(
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                ByVal Piva As String, _
                                ByVal Contratto_Cod As Int32, _
                                ByVal Fase_Cod As Int32, _
                                ByVal Fase_Des As String, _
                                ByVal Elem_Cod As Int32, _
                                ByVal Pro_Cod As Int32, _
                                ByVal Mat_Cod As Int32, _
                                ByVal Udm_Cod As Int32, _
                                ByVal Progetto_Cod As Int32, _
                                ByVal Cal_Cod As Int32, _
                                ByVal Lotto As String, _
                                ByVal Qta As Double, _
                                ByVal Data_Inizio_Prevista As DateTime, _
                                ByVal Data_Fine_Prevista As DateTime, _
                                ByVal Importo As Double, _
                                ByVal Giudizio As String, _
                                ByVal inviato As Int32, _
                                ByVal datainvio As DateTime, _
                                ByVal Data_Creazione As DateTime, _
                                ByVal Data_Modifica As DateTime, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                                ByVal Listino_Cod As Int32, _
                                ByVal Valore1 As Double, _
                                ByVal Valore2 As Double, _
                                ByVal Valore3 As Double, _
                                ByVal Valore4 As Double, _
                                ByVal Valore5 As Double, _
                                ByVal Valore6 As Double, _
                                ByVal Valore7 As Double, _
                                ByVal Valore8 As Double, _
                                ByVal Valore9 As Double, _
                                ByVal dt_Inizio_Rinnovo As DateTime, _
                                ByVal dt_Fine_Rinnovo As DateTime, _
                                ByVal prog_Rinnovato As Int32, _
                                Optional ByRef XmlDoc As XmlDocument = Nothing _
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Imprese_Contratto_Fasi")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Piva"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Contratto_Cod"), CStr(Contratto_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Fase_Cod"), CStr(Fase_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Fase_Des"), CStr(Fase_Des))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Elem_Cod"), CStr(Elem_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Pro_Cod"), CStr(Pro_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Mat_Cod"), CStr(Mat_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Udm_Cod"), CStr(Udm_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Progetto_Cod"), CStr(Progetto_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cal_Cod"), CStr(Cal_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Lotto"), CStr(Lotto))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Qta"), CStr(Qta))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Inizio_Prevista"), CStr(Data_Inizio_Prevista))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Fine_Prevista"), CStr(Data_Fine_Prevista))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Importo"), CStr(Importo))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Giudizio"), CStr(Giudizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("inviato"), CStr(inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("datainvio"), CStr(datainvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Creazione"), CStr(Data_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Modifica"), CStr(Data_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), CStr(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), CStr(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), CStr(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), CStr(Validita_Fine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Listino_Cod"), CStr(Listino_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Valore1"), CStr(Valore1))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Valore2"), CStr(Valore2))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Valore3"), CStr(Valore3))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Valore4"), CStr(Valore4))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Valore5"), CStr(Valore5))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Valore6"), CStr(Valore6))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Valore7"), CStr(Valore7))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Valore8"), CStr(Valore8))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Valore9"), CStr(Valore9))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_ParticelleCatastalixMacrousi(
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                ByVal PROV As String, _
                                ByVal COM As String, _
                                ByVal SEZIONE As String, _
                                ByVal FOGLIO As Int32, _
                                ByVal NUMERO As Int32, _
                                ByVal SUBALTERNO As String, _
                                ByVal Macrouso_Cod As String, _
                                ByVal Superficie As Double, _
                                ByVal inviato As Int32, _
                                ByVal datainvio As DateTime, _
                                ByVal Data_Creazione As DateTime, _
                                ByVal Data_Modifica As DateTime, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                                ByVal ID As Int32, _
                                ByVal PIVA As String, _
                                ByVal Numero_Fascicolo As String, _
                                ByVal Data_Validazione_Fascicolo As DateTime, _
                                ByVal Fonte As String, _
                                ByVal FonteDescr As String, _
                                Optional ByRef XmlDoc As XmlDocument = Nothing _
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("ParticelleCatastalixMacrousi")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("PROV"), CStr(PROV))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("COM"), CStr(COM))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("SEZIONE"), CStr(SEZIONE))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("FOGLIO"), CStr(FOGLIO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("NUMERO"), CStr(NUMERO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("SUBALTERNO"), CStr(SUBALTERNO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Macrouso_Cod"), CStr(Macrouso_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Superficie"), CStr(Superficie))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("inviato"), CStr(inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("datainvio"), CStr(datainvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Creazione"), CStr(Data_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Modifica"), CStr(Data_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), CStr(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), CStr(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), CStr(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), CStr(Validita_Fine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("ID"), CStr(ID))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("PIVA"), CStr(PIVA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Numero_Fascicolo"), CStr(Numero_Fascicolo))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Validazione_Fascicolo"), CStr(Data_Validazione_Fascicolo))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Fonte"), CStr(Fonte))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("FonteDescr"), CStr(FonteDescr))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_ImpresexParticelle_Contatti(
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                ByVal ID As Int32, _
                                ByVal Tipo_Contatto As Int32, _
                                ByVal Cod_RisUm As Int32, _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal PROV As String, _
                                ByVal COM As String, _
                                ByVal SEZIONE As String, _
                                ByVal FOGLIO As Int32, _
                                ByVal NUMERO As Int32, _
                                ByVal SUBALTERNO As String, _
                                ByVal Quota As Double, _
                                ByVal inviato As Int32, _
                                ByVal datainvio As DateTime, _
                                ByVal Data_Creazione As DateTime, _
                                ByVal Data_Modifica As DateTime, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                                ByVal cuaaProprietario As String, _
                                Optional ByRef XmlDoc As XmlDocument = Nothing _
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("ImpresexParticelle_Contatti")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("ID"), CStr(ID))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Tipo_Contatto"), CStr(Tipo_Contatto))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cod_RisUm"), CStr(Cod_RisUm))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Piva"), CStr(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Sa_Cod"), CStr(Sa_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("PROV"), CStr(PROV))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("COM"), CStr(COM))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("SEZIONE"), CStr(SEZIONE))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("FOGLIO"), CStr(FOGLIO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("NUMERO"), CStr(NUMERO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("SUBALTERNO"), CStr(SUBALTERNO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Quota"), CStr(Quota))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("inviato"), CStr(inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("datainvio"), CStr(datainvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Creazione"), CStr(Data_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Modifica"), CStr(Data_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), CStr(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), CStr(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), CStr(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), CStr(Validita_Fine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("cuaaProprietario"), CStr(cuaaProprietario))
        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_ZonexParticelle(
                               ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                               ByVal Piva_SuperUser As String, _
                                ByVal Zona_Cod As String, _
                                ByVal PROV As String, _
                                ByVal COM As String, _
                                ByVal SEZIONE As String, _
                                ByVal FOGLIO As String, _
                                ByVal NUMERO As String, _
                                ByVal SUBALTERNO As String, _
                                ByVal Area As String, _
                                ByVal inviato As Int32, _
                                ByVal datainvio As DateTime, _
                                ByVal Data_Creazione As DateTime, _
                                ByVal Data_Modifica As DateTime, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                                ByVal Fonte As String, _
                                ByVal FonteDescr As String, _
                                ByVal Conforme As String, _
                               Optional ByRef XmlDoc As XmlDocument = Nothing _
                                   ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("ZonexParticelle")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Piva_SuperUser"), CStr(Piva_SuperUser))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Zona_Cod"), CStr(Zona_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("PROV"), CStr(PROV))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("COM"), CStr(COM))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("SEZIONE"), CStr(SEZIONE))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("FOGLIO"), CStr(FOGLIO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("NUMERO"), CStr(NUMERO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("SUBALTERNO"), CStr(SUBALTERNO))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Area"), CStr(Area))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("inviato"), CStr(inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("datainvio"), CStr(datainvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Creazione"), CStr(Data_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Modifica"), CStr(Data_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), CStr(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), CStr(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), CStr(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), CStr(Validita_Fine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Fonte"), CStr(Fonte))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("FonteDescr"), CStr(FonteDescr))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Conforme"), CStr(Conforme))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_Programmazione_Entita(
                               ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                               ByVal Piva_SuperUser As String, _
                                ByVal Programmazione_Entita_Cod As Int32, _
                                ByVal Programmazione_Cod As Int32, _
                                ByVal Entita_Des As String, _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal Campo_Cod As Int32, _
                                ByVal Appezza As Int32, _
                                ByVal Id_Reg As Int32, _
                                ByVal Progetto_Cod As Int32, _
                                ByVal Progetto_Des As String, _
                                ByVal Id_Cod As Int32, _
                                ByVal Veg_Cod As Int32, _
                                ByVal Cul_Cod As Int32, _
                                ByVal Grfi_Cod As Int32, _
                                ByVal Cop_Cod As Int32, _
                                ByVal Superficie As Double, _
                                ByVal Resa As Double, _
                                ByVal TipoZona As String, _
                                ByVal Veg_Cod_Prec As Int32, _
                                ByVal Id_Mat_O As Int32, _
                                ByVal Id_Fre As Int32, _
                                ByVal N_distribuito As Double, _
                                ByVal inviato As Int32, _
                                ByVal Data_Creazione As DateTime, _
                                ByVal Data_Modifica As DateTime, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                                ByVal Num_Piante As Int32?, _
                                ByVal Stato_Cod As Int32?, _
                                ByVal Ciclo As Int32?, _
                                ByVal Grva_Cod As Int32?, _
                                ByVal Data_Semina As DateTime, _
                                ByVal Data_Raccolta As DateTime, _
                                ByVal Note As String, _
                                ByVal Veg_Cod_Cliente As String, _
                                ByVal Cul_Cod_Cliente As String, _
                                ByVal N_fabbisogno As Double?, _
                                ByVal Foral_Cod As Int32?, _
                                ByVal Port_Cod As Int32?, _
                                ByVal Imp_Cod As Int32?, _
                                ByVal Regolamento_Cod As Int32?, _
                                ByVal Disciplinare_Cod As Int32?, _
                                ByVal TRA_Fila As Double, _
                                ByVal SU_Fila As Double, _
                                ByVal MetodoProduzione_Cod As Int32?, _
                                ByVal FlagIrrigabilita As Int32?, _
                                ByVal FlagSecondoRaccolto As Int32?, _
                                ByVal Codice_Fiscale_Tecnico As String, _
                                ByVal Superficie_Futura As Double?, _
                                ByVal Operazione_Cod As Int32?, _
                                ByVal Macrouso_Cod As String, _
                                ByVal Via_Stringa As String, _
                                ByVal Unita_Vitata As Int32?, _
                                ByVal Validita_Inizio_Impianto As DateTime, _
                                ByVal Zslm As Int32?, _
                                ByVal AltriVitigniPresenti As Int32?, _
                                ByVal ancoraggiTestata As Int32?, _
                                ByVal annoRiferimento As Int32?, _
                                ByVal codFiliStostegno As String, _
                                ByVal codPaliTessitura As String, _
                                ByVal codPaliTestata As String, _
                                ByVal codStatoColt As String, _
                                ByVal codTipoVari As String, _
                                ByVal DataProtocollo As DateTime, _
                                ByVal DataRilievo As DateTime, _
                                ByVal destProduttiva As String, _
                                ByVal destProduttivaDescr As String, _
                                ByVal distanzaPali As Double, _
                                ByVal dtFine As DateTime?, _
                                ByVal dtFineGestione As DateTime?, _
                                ByVal dtInizio As DateTime?, _
                                ByVal dtInizioGestione As DateTime?, _
                                ByVal dtIns As DateTime?, _
                                ByVal dtVar As DateTime?, _
                                ByVal fallanzePerc As Double?, _
                                ByVal flagAnomalia As String, _
                                ByVal flagAttuale As String, _
                                ByVal flagCessata As String, _
                                ByVal flagContributo As String, _
                                ByVal flagRegolarizz2009 As String, _
                                ByVal flagRicalcoloGis As String, _
                                ByVal GiacituraTerreno As String, _
                                ByVal idUnitaVitata As Int32?, _
                                ByVal idUtenteIns As String, _
                                ByVal idUtenteVar As String, _
                                ByVal numeroProtocollo As String, _
                                ByVal progPoligono As String, _
                                ByVal supVitataDich As Int32?, _
                                ByVal supVitataDichPRCalcolo As Int32?, _
                                ByVal SuperficieServizioMq As Int32?, _
                                ByVal Terrazzamenti As Int32?, _
                                ByVal TipoColtura As String, _
                                ByVal tipoProcedimento As String, _
                                ByVal tipoUnar As String, _
                                ByVal tipoVariazione As String, _
                                ByVal unar As String, _
                               Optional ByRef XmlDoc As XmlDocument = Nothing _
                                   ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Programmazione_Entita")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Programmazione_Entita_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Programmazione_Entita_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Programmazione_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Programmazione_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Entita_Des"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Entita_Des))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Piva"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Piva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Sa_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Sa_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Campo_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Campo_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Appezza"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Appezza))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Id_Reg"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Id_Reg))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Progetto_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Progetto_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Progetto_Des"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Progetto_Des))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Id_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Id_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Veg_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Veg_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cul_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Cul_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Grfi_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Grfi_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cop_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Cop_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Superficie"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Superficie))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Resa"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Resa))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("TipoZona"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(TipoZona))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Veg_Cod_Prec"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Veg_Cod_Prec))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Id_Mat_O"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Id_Mat_O))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Id_Fre"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Id_Fre))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("N_distribuito"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(N_distribuito))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("inviato"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Creazione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Data_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Modifica"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Data_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Validita_Fine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Num_Piante"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Num_Piante))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Stato_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Stato_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Ciclo"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Ciclo))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Grva_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Grva_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Semina"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Data_Semina))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Raccolta"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Data_Raccolta))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Note"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Note))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Veg_Cod_Cliente"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Veg_Cod_Cliente))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Cul_Cod_Cliente"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Cul_Cod_Cliente))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("N_fabbisogno"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(N_fabbisogno))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Foral_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Foral_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Port_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Port_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Imp_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Imp_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Regolamento_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Regolamento_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Disciplinare_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Disciplinare_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("TRA_Fila"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(TRA_Fila))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("SU_Fila"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(SU_Fila))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("MetodoProduzione_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(MetodoProduzione_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("FlagIrrigabilita"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(FlagIrrigabilita))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("FlagSecondoRaccolto"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(FlagSecondoRaccolto))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Codice_Fiscale_Tecnico"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Codice_Fiscale_Tecnico))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Superficie_Futura"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Superficie_Futura))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Operazione_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Operazione_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Macrouso_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Macrouso_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Via_Stringa"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Via_Stringa))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Unita_Vitata"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Unita_Vitata))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio_Impianto"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Validita_Inizio_Impianto))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Zslm"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Zslm))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("AltriVitigniPresenti"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(AltriVitigniPresenti))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("ancoraggiTestata"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(ancoraggiTestata))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("annoRiferimento"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(annoRiferimento))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("codFiliStostegno"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(codFiliStostegno))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("codPaliTessitura"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(codPaliTessitura))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("codPaliTestata"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(codPaliTestata))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("codStatoColt"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(codStatoColt))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("codTipoVari"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(codTipoVari))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("DataProtocollo"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(DataProtocollo))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("DataRilievo"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(DataRilievo))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("destProduttiva"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(destProduttiva))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("destProduttivaDescr"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(destProduttivaDescr))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("distanzaPali"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(distanzaPali))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dtFine"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtFine))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dtFineGestione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtFineGestione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dtInizio"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtInizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dtInizioGestione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtInizioGestione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dtIns"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtIns))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dtVar"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtVar))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("fallanzePerc"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(fallanzePerc))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("flagAnomalia"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(flagAnomalia))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("flagAttuale"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(flagAttuale))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("flagCessata"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(flagCessata))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("flagContributo"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(flagContributo))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("flagRegolarizz2009"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(flagRegolarizz2009))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("flagRicalcoloGis"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(flagRicalcoloGis))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("GiacituraTerreno"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(GiacituraTerreno))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("idUnitaVitata"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(idUnitaVitata))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("idUtenteIns"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(idUtenteIns))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("idUtenteVar"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(idUtenteVar))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("numeroProtocollo"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(numeroProtocollo))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("progPoligono"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(progPoligono))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("supVitataDich"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(supVitataDich))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("supVitataDichPRCalcolo"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(supVitataDichPRCalcolo))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("SuperficieServizioMq"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(SuperficieServizioMq))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Terrazzamenti"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Terrazzamenti))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("TipoColtura"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(TipoColtura))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("tipoProcedimento"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(tipoProcedimento))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("tipoUnar"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(tipoUnar))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("tipoVariazione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(tipoVariazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("unar"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(unar))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_AltraSpecie_Impianti(
                               ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                               ByVal PIVA As String, _
                                ByVal SA_COD As Int32, _
                                ByVal APPEZZA As Int32, _
                                ByVal ID_REG As Int32, _
                                ByVal Programmazione_Cod As Int32, _
                                ByVal ID As Int32, _
                                ByVal codVitigno As String, _
                                ByVal descrVitigno As String, _
                                ByVal dtIns As DateTime, _
                                ByVal idUnitaVitata As String, _
                                ByVal perc As Double, _
                                ByVal progr As Int32, _
                                ByVal Reale As String, _
                                ByVal inviato As Int32, _
                                ByVal datainvio As DateTime, _
                                ByVal Data_Creazione As DateTime, _
                                ByVal Data_Modifica As DateTime, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                               Optional ByRef XmlDoc As XmlDocument = Nothing _
                                   ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("AltraSpecie_Impianti")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", CStr(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("PIVA"), CStr(PIVA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("SA_COD"), CStr(SA_COD))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("APPEZZA"), CStr(APPEZZA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("ID_REG"), CStr(ID_REG))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Programmazione_Cod"), CStr(Programmazione_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("ID"), CStr(ID))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("codVitigno"), CStr(codVitigno))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("descrVitigno"), CStr(descrVitigno))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dtIns"), CStr(dtIns))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("idUnitaVitata"), CStr(idUnitaVitata))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("perc"), CStr(perc))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("progr"), CStr(progr))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Reale"), CStr(Reale))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("inviato"), CStr(inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("datainvio"), CStr(datainvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Creazione"), CStr(Data_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Modifica"), CStr(Data_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), CStr(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), CStr(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), CStr(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), CStr(Validita_Fine))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_Idoneita_Appezzamento(
                               ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                               ByVal PIVA As String, _
                                ByVal Progetto_Cod As Int32, _
                                ByVal Programmazione_Cod As Int32, _
                                ByVal IDIdoneita As Int32, _
                                ByVal codTipologia As Int32, _
                                ByVal dataRev As Date, _
                                ByVal dataRic As Date, _
                                ByVal docIgtDescr As String, _
                                ByVal dtInizio As Date, _
                                ByVal dtIns As Date, _
                                ByVal dtVar As Date, _
                                ByVal idDocIgt As String, _
                                ByVal idUnitaVitata As String, _
                                ByVal idUtenteIns As String, _
                                ByVal idUtenteVar As String, _
                                ByVal numIscrizione As Int32?, _
                                ByVal tipologiaDescr As String, _
                                ByVal inviato As Int32, _
                                ByVal datainvio As DateTime, _
                                ByVal Data_Creazione As DateTime, _
                                ByVal Data_Modifica As DateTime, _
                                ByVal Username_Creazione As String, _
                                ByVal Username_Modifica As String, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                               Optional ByRef XmlDoc As XmlDocument = Nothing _
                                   ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Idoneita_Appezzamento")


        With NodoXml

            aggiungiFiglio(NodoXml, XmlDoc, "TipoOperazioneDB", AgronicaCoreDataProvider.UtilityProvider.ValoreToString(TipoOperazioneDB))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("PIVA"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(PIVA))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Progetto_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Progetto_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Programmazione_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Programmazione_Cod))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("IDIdoneita"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(IDIdoneita))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("codTipologia"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(codTipologia))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dataRev"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dataRev))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dataRic"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dataRic))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("docIgtDescr"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(docIgtDescr))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dtInizio"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtInizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dtIns"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtIns))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("dtVar"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtVar))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("idDocIgt"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(idDocIgt))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("idUnitaVitata"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(idUnitaVitata))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("idUtenteIns"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(idUtenteIns))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("idUtenteVar"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(idUtenteVar))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("numIscrizione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(numIscrizione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("tipologiaDescr"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(tipologiaDescr))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("inviato"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(inviato))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("datainvio"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(datainvio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Creazione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Data_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Data_Modifica"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Data_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Creazione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Username_Creazione))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Username_Modifica"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Username_Modifica))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Inizio"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Validita_Inizio))
            aggiungiFiglio(NodoXml, XmlDoc, LCase("Validita_Fine"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Validita_Fine))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Sub aggiungiFiglio(ByRef NodoXml As System.Xml.XmlElement, ByRef XmlDoc As XmlDocument, tagName As String, value As String)
        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        Dim NodoXml1 = XmlDoc.CreateElement(tagName)
        NodoXml1.InnerText = value
        NodoXml.AppendChild(NodoXml1)
    End Sub

End Class
