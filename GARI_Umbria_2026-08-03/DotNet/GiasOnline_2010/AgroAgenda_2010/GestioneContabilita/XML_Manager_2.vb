Imports System.Xml
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Module XML_Manager_2


    '##########################################################################################
    Public Function XML_2_Agenda_Agenda(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                        Optional ByVal Piva As String = "00000000000",
                                        Optional ByVal Sa_Cod As Integer = 0,
                                        Optional ByVal ID_Agenda As Integer = 0,
                                        Optional ByVal Lav_Cod As Integer = 0,
                                        Optional ByVal Des_Lib As String = "",
                                        Optional ByVal Tipo_Accettazione As Integer = 0,
                                        Optional ByVal Linea_Cod As Integer = 0,
                                        Optional ByVal Preparazione_Cod As Integer = 0,
                                        Optional ByVal Id_Trasformazione As Integer = 0,
                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                        Optional ByVal BaseCode As Integer = 0,
                                        Optional ByVal TopCode As Integer = 200000000,
                                        Optional ByRef XmlDoc As XmlDocument = Nothing,
                                        Optional ByVal Blocco_Flag As Integer = 0,
                                        Optional ByVal Blocco_Data As Date = AGRODATAINIZIO,
                                        Optional ByVal Blocco_Username As String = ""
                                        ) As XmlElement

        Dim nodoXml As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        nodoXml = XmlDoc.CreateElement("Agenda")

        'Imposto gli attributi
        nodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        nodoXml.SetAttribute(LCase("piva"), Piva)
        nodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        nodoXml.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        nodoXml.SetAttribute(LCase("lav_cod"), CStr(Lav_Cod))
        nodoXml.SetAttribute(LCase("des_lib"), Des_Lib)
        nodoXml.SetAttribute(LCase("tipo_accettazione"), CStr(Tipo_Accettazione))
        nodoXml.SetAttribute(LCase("linea_cod"), CStr(Linea_Cod))
        nodoXml.SetAttribute(LCase("preparazione_cod"), CStr(Preparazione_Cod))
        nodoXml.SetAttribute(LCase("id_trasformazione"), CStr(Id_Trasformazione))
        nodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        nodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        nodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
        nodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))
        nodoXml.SetAttribute(LCase("Blocco_Flag"), CStr(Blocco_Flag))
        nodoXml.SetAttribute(LCase("Blocco_Data"), CStr(Blocco_Data))
        nodoXml.SetAttribute(LCase("Blocco_Username"), Blocco_Username)

        'Restituisco in uscita 
        Return nodoXml

    End Function

    '##########################################################################################
    Public Function XML_2_Agenda_Movimento(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                           Optional ByVal Piva As String = "00000000000",
                                           Optional ByVal Sa_Cod As Integer = 0,
                                           Optional ByVal ID_Agenda As Integer = 0,
                                           Optional ByVal ID_Mov As Integer = 0,
                                           Optional ByVal Cau_Mov As String = "",
                                           Optional ByVal Mov_Desc As String = "",
                                           Optional ByVal Data_Movimento As Date = AGRODATAINIZIO,
                                           Optional ByVal Ora As String = "00.00",
                                           Optional ByVal Data_Registrazione As Date = AGRODATAINIZIO,
                                           Optional ByVal Scadenza As Date = AGRODATAFINE,
                                           Optional ByVal Scadenza_Extra As Date = AGRODATAINIZIO,
                                           Optional ByVal Doc_Numero_Sin As String = "",
                                           Optional ByVal Doc_Numero As Decimal = 0,
                                           Optional ByVal Doc_Numero_Des As String = "",
                                           Optional ByVal Num_Protocollo As Decimal = 0.0,
                                           Optional ByVal Colli As Integer = 0,
                                           Optional ByVal Peso As Decimal = 0.0,
                                           Optional ByVal Aspetto As String = "",
                                           Optional ByVal Causale_Trasporto As String = "",
                                           Optional ByVal Tipo_Sconto As Integer = 0,
                                           Optional ByVal Cod_RisUm As Integer = 0,
                                           Optional ByVal Cod_IndirizzoRisUm As Integer = 0,
                                           Optional ByVal Cod_Destinazione As Integer = 0,
                                           Optional ByVal Cod_IndirizzoDestinazione As Integer = 0,
                                           Optional ByVal Mezzo As Integer = 0,
                                           Optional ByVal Cod_Vettore As Integer = 0,
                                           Optional ByVal Cod_IndirizzoVettore As Integer = 0,
                                           Optional ByVal Natura_Beni As String = "",
                                           Optional ByVal Modalita As Integer = 0,
                                           Optional ByVal Tara_Veicolo As Decimal = 0,
                                           Optional ByVal Tara_Imballi As Decimal = 0,
                                           Optional ByVal Tipo_Peso As Integer = 0,
                                           Optional ByVal Username_Note As String = "",
                                           Optional ByVal Extra_Str As String = "",
                                           Optional ByVal Extra_Int As Integer = 0,
                                           Optional ByVal Extra_Date As Date = AGRODATAINIZIO,
                                           Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                           Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                           Optional ByVal BaseCode As Integer = 0,
                                           Optional ByVal TopCode As Integer = 200000000,
                                           Optional ByRef XmlDoc As XmlDocument = Nothing
                                           ) As XmlElement

        Dim nodoXml As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        nodoXml = XmlDoc.CreateElement("Movimento")

        'Imposto gli attributi
        nodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        nodoXml.SetAttribute(LCase("piva"), Piva)
        nodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        nodoXml.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        nodoXml.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        nodoXml.SetAttribute(LCase("Cau_Mov"), Cau_Mov)
        nodoXml.SetAttribute(LCase("Mov_Desc"), Mov_Desc)
        nodoXml.SetAttribute(LCase("Data_Movimento"), Format(Data_Movimento, "dd/MM/yyyy"))
        nodoXml.SetAttribute(LCase("ora"), Ora)
        nodoXml.SetAttribute(LCase("Data_Registrazione"), Format(Data_Registrazione, "dd/MM/yyyy"))
        nodoXml.SetAttribute(LCase("Scadenza"), Format(Scadenza, "dd/MM/yyyy"))
        nodoXml.SetAttribute(LCase("Scadenza_Extra"), Format(Scadenza_Extra, "dd/MM/yyyy"))
        nodoXml.SetAttribute(LCase("Doc_Numero_Sin"), Doc_Numero_Sin)
        nodoXml.SetAttribute(LCase("Doc_Numero"), CStr(Doc_Numero))
        nodoXml.SetAttribute(LCase("Doc_Numero_Des"), Doc_Numero_Des)
        nodoXml.SetAttribute(LCase("Num_Protocollo"), CStr(Num_Protocollo))
        nodoXml.SetAttribute(LCase("colli"), CStr(Colli))
        nodoXml.SetAttribute(LCase("peso"), CStr(Peso))
        nodoXml.SetAttribute(LCase("Aspetto"), Aspetto)
        nodoXml.SetAttribute(LCase("Causale_Trasporto"), Causale_Trasporto)
        nodoXml.SetAttribute(LCase("Tipo_Sconto"), CStr(Tipo_Sconto))
        nodoXml.SetAttribute(LCase("Cod_RisUm"), CStr(Cod_RisUm))
        nodoXml.SetAttribute(LCase("Cod_IndirizzoRisUm"), CStr(Cod_IndirizzoRisUm))
        nodoXml.SetAttribute(LCase("Cod_Destinazione"), CStr(Cod_Destinazione))
        nodoXml.SetAttribute(LCase("Cod_IndirizzoDestinazione"), CStr(Cod_IndirizzoDestinazione))
        nodoXml.SetAttribute(LCase("Mezzo"), CStr(Mezzo))
        nodoXml.SetAttribute(LCase("Cod_Vettore"), CStr(Cod_Vettore))
        nodoXml.SetAttribute(LCase("Cod_IndirizzoVettore"), CStr(Cod_IndirizzoVettore))
        nodoXml.SetAttribute(LCase("Natura_Beni"), Natura_Beni)
        nodoXml.SetAttribute(LCase("Modalita"), CStr(Modalita))
        nodoXml.SetAttribute(LCase("Tara_Veicolo"), CStr(Tara_Veicolo))
        nodoXml.SetAttribute(LCase("Tara_Imballi"), CStr(Tara_Imballi))
        nodoXml.SetAttribute(LCase("Tipo_Peso"), CStr(Tipo_Peso))
        nodoXml.SetAttribute(LCase("Username_Note"), Username_Note)
        nodoXml.SetAttribute(LCase("Extra_Str"), Extra_Str)
        nodoXml.SetAttribute(LCase("Extra_Int"), CStr(Extra_Int))
        nodoXml.SetAttribute(LCase("Extra_Date"), CStr(Extra_Date))
        nodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        nodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        nodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
        nodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Restituisco in uscita 
        Return nodoXml

    End Function


    '##########################################################################################
    Public Function XML_2_Agenda_MovimentoDestinazione(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                       Optional ByVal Piva As String = "00000000000",
                                                       Optional ByVal Sa_Cod As Integer = 0,
                                                       Optional ByVal ID_Agenda As Integer = 0,
                                                       Optional ByVal ID_Mov As Integer = 0,
                                                       Optional ByVal ID_Mov_Det As Integer = 0,
                                                       Optional ByVal Appezza As Integer = 0,
                                                       Optional ByVal ID_Destinazione As Integer = 0,
                                                       Optional ByVal Tipo_Destinazione As Integer = 0,
                                                       Optional ByVal Qta As Decimal = 0,
                                                       Optional ByVal Qta2 As Decimal = 0,
                                                       Optional ByVal Tipo_Scorta As Integer = 0,
                                                       Optional ByVal Scorta_Min As Decimal = 0,
                                                       Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                                       Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                                       Optional ByVal BaseCode As Integer = 0,
                                                       Optional ByVal TopCode As Integer = 200000000,
                                                       Optional ByRef XmlDoc As XmlDocument = Nothing
                                                       ) As XmlElement

        Dim nodoXml As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        nodoXml = XmlDoc.CreateElement("Movimento_Destinazione")

        'Imposto gli attributi
        nodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        nodoXml.SetAttribute(LCase("piva"), Piva)
        nodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        nodoXml.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        nodoXml.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        nodoXml.SetAttribute(LCase("ID_Mov_Det"), CStr(ID_Mov_Det))
        nodoXml.SetAttribute(LCase("Appezza"), CStr(Appezza))
        nodoXml.SetAttribute(LCase("ID_Destinazione"), CStr(ID_Destinazione))
        nodoXml.SetAttribute(LCase("Tipo_Destinazione"), CStr(Tipo_Destinazione))
        nodoXml.SetAttribute(LCase("Qta"), CStr(Qta))
        nodoXml.SetAttribute(LCase("Qta2"), CStr(Qta2))
        nodoXml.SetAttribute(LCase("Tipo_Scorta"), CStr(Tipo_Scorta))
        nodoXml.SetAttribute(LCase("Scorta_Min"), CStr(Scorta_Min))
        nodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        nodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        nodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
        nodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Restituisco in uscita 
        Return nodoXml

    End Function


    '##########################################################################################
    'Nuova versione per creazione xml per la nuova struttura
    'della tabella Movimenti_Dettagli  
    'la FormProdotto (che gestisce il magazzino) ce l'ha all'interno del codice 
    '(NOME: XML_Genera_MovimentoDettaglio) perché deve gestire vari casi
    Public Function XML_2_Agenda_MovimentoDettaglio(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                    Optional ByVal Piva As String = "00000000000",
                                                    Optional ByVal Sa_Cod As Integer = 0,
                                                    Optional ByVal ID_Agenda As Integer = 0,
                                                    Optional ByVal ID_Mov As Integer = 0,
                                                    Optional ByVal ID_Mov_Det As Integer = 0,
                                                    Optional ByVal Mov_Det_Des As String = "",
                                                    Optional ByVal Elem_Cod As Integer = 0,
                                                    Optional ByVal Pro_Cod As Integer = 0,
                                                    Optional ByVal Mat_Cod As Integer = 0,
                                                    Optional ByVal Cod_Progetto As String = "",
                                                    Optional ByVal Fase_Cod As Integer = 0,
                                                    Optional ByVal Lotto As String = "",
                                                    Optional ByVal Cal_Cod As Integer = 0,
                                                    Optional ByVal Udm_Cod As Integer = 0,
                                                    Optional ByVal Udm_Cod_Extra As Integer = 0,
                                                    Optional ByVal Qta As Decimal = 0.0,
                                                    Optional ByVal Qta_Extra As Decimal = 0.0,
                                                    Optional ByVal Prezzo_Unitario As Decimal = 0.0,
                                                    Optional ByVal Prezzo_Unitario_Netto As Decimal = 0.0,
                                                    Optional ByVal Imponibile As Decimal = 0.0,
                                                    Optional ByVal Imponibile_Netto As Decimal = 0.0,
                                                    Optional ByVal Cod_IVA As Integer = 0,
                                                    Optional ByVal IVA As Decimal = 0.0,
                                                    Optional ByVal Sconto As Decimal = 0,
                                                    Optional ByVal Prezzo_Effettivo As Decimal = 0,
                                                    Optional ByVal Anno As Integer = 1900,
                                                    Optional ByVal Ric_Cod As Integer = 0,
                                                    Optional ByVal Cod_Conto As Integer = 0,
                                                    Optional ByVal Jolly_Int As Integer = MagazzinoMovimentato,
                                                    Optional ByVal Contabilizzato As Integer = NONCONTABILE,
                                                    Optional ByVal Pendente As Integer = enum_Pendenza.MovPendente,
                                                    Optional ByVal Extra_Str As String = "",
                                                    Optional ByVal Extra_Int As Integer = 0,
                                                    Optional ByVal Extra_Date As Date = AGRODATAINIZIO,
                                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                                    Optional ByVal BaseCode As Integer = 0,
                                                    Optional ByVal TopCode As Integer = 200000000,
                                                    Optional ByRef XmlDoc As XmlDocument = Nothing,
                                                    Optional ByVal Id_Destinazione As Integer = 0,
                                                    Optional ByVal Lav_Cod_Allegato As Integer = 0,
                                                    Optional ByVal Cau_Mov As String = ""
                                                    ) As XmlElement

        Dim nodoXml As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        nodoXml = XmlDoc.CreateElement("Movimento_Dettaglio")

        'Imposto gli attributi

        nodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        nodoXml.SetAttribute(LCase("piva"), Piva)
        nodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        nodoXml.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        nodoXml.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        nodoXml.SetAttribute(LCase("ID_Mov_Det"), CStr(ID_Mov_Det))
        nodoXml.SetAttribute(LCase("Mov_Det_Des"), Mov_Det_Des)
        nodoXml.SetAttribute(LCase("Elem_Cod"), CStr(Elem_Cod))
        nodoXml.SetAttribute(LCase("Pro_Cod"), CStr(Pro_Cod))
        nodoXml.SetAttribute(LCase("Mat_Cod"), CStr(Mat_Cod))
        nodoXml.SetAttribute(LCase("Cod_Progetto"), CStr(Cod_Progetto))
        nodoXml.SetAttribute(LCase("Fase_Cod"), CStr(Fase_Cod))
        nodoXml.SetAttribute(LCase("Lotto"), Lotto)
        nodoXml.SetAttribute(LCase("Cal_Cod"), CStr(Cal_Cod))
        nodoXml.SetAttribute(LCase("Udm_Cod"), CStr(Udm_Cod))
        nodoXml.SetAttribute(LCase("udm_cod_extra"), CStr(Udm_Cod_Extra))
        nodoXml.SetAttribute(LCase("Qta"), CStr(Qta))
        nodoXml.SetAttribute(LCase("qta_extra"), CStr(Qta_Extra))
        nodoXml.SetAttribute(LCase("Prezzo_Unitario"), CStr(Prezzo_Unitario))
        nodoXml.SetAttribute(LCase("Prezzo_Unitario_Netto"), CStr(Prezzo_Unitario_Netto))
        nodoXml.SetAttribute(LCase("Imponibile"), CStr(Imponibile))
        nodoXml.SetAttribute(LCase("Imponibile_Netto"), CStr(Imponibile_Netto))
        nodoXml.SetAttribute(LCase("Cod_IVA"), CStr(Cod_IVA))
        nodoXml.SetAttribute(LCase("Iva"), CStr(IVA))
        nodoXml.SetAttribute(LCase("Sconto"), CStr(Sconto))
        nodoXml.SetAttribute(LCase("Prezzo_Effettivo"), CStr(Prezzo_Effettivo))
        nodoXml.SetAttribute(LCase("Anno"), CStr(Anno))
        nodoXml.SetAttribute(LCase("Ric_Cod"), CStr(Ric_Cod))
        nodoXml.SetAttribute(LCase("Cod_Conto"), CStr(Cod_Conto))
        nodoXml.SetAttribute(LCase("jolly_Int"), CStr(Jolly_Int))
        nodoXml.SetAttribute(LCase("Contabilizzato"), CStr(Contabilizzato))
        nodoXml.SetAttribute(LCase("Pendente"), CStr(Pendente))
        nodoXml.SetAttribute(LCase("Extra_Str"), Extra_Str)
        nodoXml.SetAttribute(LCase("Extra_Int"), CStr(Extra_Int))
        nodoXml.SetAttribute(LCase("Extra_Date"), CStr(Extra_Date))
        nodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        nodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        nodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
        nodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))

        '==============================================================================================================
        'Inserimento nella stringa Xml dei campi non presenti in Db ma utili (necessari) per la gestione delle giacenze
        '--------------------------------------------------------------------------------------------------------------
        nodoXml.SetAttribute("id_destinazione", CStr(Id_Destinazione))
        nodoXml.SetAttribute("lav_cod", CStr(Lav_Cod_Allegato)) 'Nota: Utile in FormFattura
        nodoXml.SetAttribute("cau_mov", Cau_Mov) 'IMPORTANTE!!!!!


        'Restituisco in uscita 
        Return nodoXml

    End Function

    '##########################################################################################
    Public Function XML_2_Agenda_Mov_Dettagli_Tecnici_Extra(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                            Optional ByVal Piva As String = "00000000000",
                                                            Optional ByVal Sa_Cod As Integer = 0,
                                                            Optional ByVal ID_Agenda As Integer = 0,
                                                            Optional ByVal ID_Mov As Integer = 0,
                                                            Optional ByVal ID_Mov_Det As Integer = 0,
                                                            Optional ByVal Id_Reg_Dettaglio As Integer = 0,
                                                            Optional ByVal Regione As String = "",
                                                            Optional ByVal ASL As String = "",
                                                            Optional ByVal Serie As String = "",
                                                            Optional ByVal Numero As String = "",
                                                            Optional ByVal Mac_Cod As Integer = 0,
                                                            Optional ByVal Cod_RisUm As Integer = 0,
                                                            Optional ByVal Trasportatore As String = "",
                                                            Optional ByVal Mezzo_Trasporto As String = "",
                                                            Optional ByVal Targa As String = "",
                                                            Optional ByVal N_Immatricolazione As String = "",
                                                            Optional ByVal N_Immatricolazione_Rimorchio As String = "",
                                                            Optional ByVal N_Autorizzazione_Trasporto As String = "",
                                                            Optional ByVal Data_Rilascio_Autorizzazione As Date =
                                                               AGRODATAINIZIO,
                                                            Optional ByVal Peso As Decimal = 0,
                                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                                            Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                                            Optional ByVal BaseCode As Integer = 0,
                                                            Optional ByVal TopCode As Integer = 200000000,
                                                            Optional ByRef XmlDoc As XmlDocument = Nothing
                                                            ) As XmlElement

        Dim nodoXml As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        nodoXml = XmlDoc.CreateElement("Movimento_Dettaglio_Tecnico_Extra")

        'Imposto gli attributi

        nodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        nodoXml.SetAttribute(LCase("piva"), Piva)
        nodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        nodoXml.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        nodoXml.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        nodoXml.SetAttribute(LCase("ID_Mov_Det"), CStr(ID_Mov_Det))
        nodoXml.SetAttribute(LCase("id_reg_dettaglio"), CStr(Id_Reg_Dettaglio))
        nodoXml.SetAttribute(LCase("regione"), CStr(Regione))
        nodoXml.SetAttribute(LCase("asl"), CStr(ASL))
        nodoXml.SetAttribute(LCase("serie"), CStr(Serie))
        nodoXml.SetAttribute(LCase("numero"), CStr(Numero))
        nodoXml.SetAttribute(LCase("mac_cod"), CStr(Mac_Cod))
        nodoXml.SetAttribute(LCase("cod_risum"), CStr(Cod_RisUm))
        nodoXml.SetAttribute(LCase("trasportatore"), CStr(Trasportatore))
        nodoXml.SetAttribute(LCase("mezzo_trasporto"), CStr(Mezzo_Trasporto))
        nodoXml.SetAttribute(LCase("targa"), CStr(Targa))
        nodoXml.SetAttribute(LCase("n_immatricolazione"), CStr(N_Immatricolazione))
        nodoXml.SetAttribute(LCase("n_immatricolazione_rimorchio"), CStr(N_Immatricolazione_Rimorchio))
        nodoXml.SetAttribute(LCase("n_autorizzazione_trasporto"), CStr(N_Autorizzazione_Trasporto))
        nodoXml.SetAttribute(LCase("data_rilascio_autorizzazione"), CStr(Data_Rilascio_Autorizzazione))
        nodoXml.SetAttribute(LCase("peso"), CStr(Peso))
        nodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        nodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        nodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
        nodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Restituisco in uscita 
        Return nodoXml

    End Function


    '##########################################################################################
    Public Function XML_2_Agenda_Movimento_Riferimento2(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                        Optional ByVal Piva As String = "",
                                                        Optional ByVal Sa_Cod As Integer = 0,
                                                        Optional ByVal ID_Agenda As Integer = 0,
                                                        Optional ByVal ID_Mov As Integer = 0,
                                                        Optional ByVal ID_Mov_Det As Integer = 0,
                                                        Optional ByVal Lav_Cod As Integer = 0,
                                                        Optional ByVal Cau_Mov As String = "",
                                                        Optional ByVal Piva_Rif As String = "",
                                                        Optional ByVal Sa_Cod_Rif As Integer = 0,
                                                        Optional ByVal ID_Agenda_Rif As Integer = 0,
                                                        Optional ByVal ID_Mov_Rif As Integer = 0,
                                                        Optional ByVal ID_Mov_Det_Rif As Integer = 0,
                                                        Optional ByVal Lav_Cod_Rif As Integer = 0,
                                                        Optional ByVal Cau_Mov_Rif As String = "",
                                                        Optional ByVal Qta As Decimal = 0,
                                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                                        Optional ByRef XmlDoc As XmlDocument = Nothing
                                                        ) As XmlElement

        Dim nodoXml As XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        nodoXml = XmlDoc.CreateElement("Movimento_Riferimento2")

        'Imposto gli attributi

        nodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        nodoXml.SetAttribute(LCase("piva"), Piva)
        nodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        nodoXml.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        nodoXml.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        nodoXml.SetAttribute(LCase("ID_Mov_Det"), CStr(ID_Mov_Det))
        nodoXml.SetAttribute(LCase("lav_cod"), CStr(Lav_Cod))
        nodoXml.SetAttribute(LCase("cau_mov"), Cau_Mov)
        nodoXml.SetAttribute(LCase("piva_rif"), Piva_Rif)
        nodoXml.SetAttribute(LCase("sa_cod_rif"), CStr(Sa_Cod_Rif))
        nodoXml.SetAttribute(LCase("id_agenda_rif"), CStr(ID_Agenda_Rif))
        nodoXml.SetAttribute(LCase("ID_Mov_rif"), CStr(ID_Mov_Rif))
        nodoXml.SetAttribute(LCase("ID_Mov_Det_rif"), CStr(ID_Mov_Det_Rif))
        nodoXml.SetAttribute(LCase("lav_cod_rif"), CStr(Lav_Cod_Rif))
        nodoXml.SetAttribute(LCase("cau_mov_rif"), Cau_Mov_Rif)
        nodoXml.SetAttribute(LCase("qta"), CStr(Qta))
        nodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        nodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))

        'Restituisco in uscita 
        Return nodoXml

    End Function



    '##########################################################################################
    Public Function XML_2_Contabilita_AltriCostiRicavi(ByRef XmlDoc As XmlDocument,
                                                       ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                       ByVal Piva As String,
                                                       ByVal Sa_Cod As Integer,
                                                       ByVal Lav_Cod As Integer,
                                                       ByVal Des_Lib As String,
                                                       ByVal Cau_Mov As String,
                                                       ByVal Mov_Desc As String,
                                                       ByVal Data_Movimento As Date,
                                                       ByVal Scadenza As Date,
                                                       ByVal Extra_Str As String,
                                                       ByVal Num_Protocollo As Decimal,
                                                       ByVal XmlDatiPagamenti As XmlElement,
                                                       ByVal XmlDatiMovDettagli As XmlElement,
                                                       ByVal BaseCode As Integer,
                                                       ByVal TopCode As Integer
                                                       ) As String

        Dim xmlDatiAgenda As XmlElement
        Dim xmlAgenda As XmlElement
        Dim xmlDatiMovimenti As XmlElement
        Dim xmlMovimento As XmlElement
        'Dim XmlDatiMovDettagli As XmlElement
        'Dim XmlMovDettaglio As XmlElement

        '#######################################################
        '##################   DATI AGENDA    ###################
        '#######################################################

        xmlDatiAgenda = XmlDoc.CreateElement("DatiAgenda")

        XmlDoc.AppendChild(xmlDatiAgenda)


        '#######################################################
        '#####################   AGENDA    #####################
        '#######################################################

        xmlAgenda = XML_2_Agenda_Agenda(enum_TipoOperazioneDB.Scrittura,
                                        Piva,
                                        Sa_Cod,
                                        0,
                                        Lav_Cod,
                                        Des_Lib,
                                        , , , ,
                                        Data_Movimento, ,
                                        BaseCode,
                                        TopCode,
                                        XmlDoc)

        xmlDatiAgenda.AppendChild(xmlAgenda)


        '#######################################################
        '################   DATI MOVIMENTI    ##################
        '#######################################################

        xmlDatiMovimenti = XmlDoc.CreateElement("DatiMovimenti")

        xmlAgenda.AppendChild(xmlDatiMovimenti)


        '#######################################################
        '##################   MOVIMENTO    #####################
        '#######################################################

        xmlMovimento = XML_2_Agenda_Movimento(enum_TipoOperazioneDB.Scrittura,
                                              Piva,
                                              Sa_Cod,
                                              0, 0,
                                              Cau_Mov,
                                              Mov_Desc,
                                              Data_Movimento, , ,
                                              Scadenza, , 
                                              , , ,
                                              Num_Protocollo,
                                              , , , , , , , , , , , , , , , , , ,
                                              Extra_Str, , ,
                                              Data_Movimento, ,
                                              BaseCode,
                                              TopCode,
                                              XmlDoc)

        xmlDatiMovimenti.AppendChild(xmlMovimento)


        '#######################################################
        '##################   PAGAMENTI   ######################
        '#######################################################


        ''XmlMovimento.AppendChild(XML_Pagamenti)

        'XmlMovimento.InnerText = Str_XML_Pagamenti


        '#######################################################
        '###############   MOVIMENTI DETTAGLI    ###############
        '#######################################################

        ''XmlDatiMovDettagli = XmlDoc.CreateElement("DatiMovimenti_Dettagli")

        xmlMovimento.AppendChild(XmlDatiMovDettagli)

        ''XmlDatiMovDettagli.InnerText = Str_XML_Mov_Dettagli

        Return XmlDoc.OuterXml

    End Function


    '##########################################################################################
    Public Function XML_2_Contabilita_AltriCostiRicavi_Dettagli(ByRef XmlDoc As XmlDocument,
                                                                ByVal DT As DataTable,
                                                                ByVal Piva As String,
                                                                ByVal Sa_Cod As Integer,
                                                                ByVal Lav_Cod As Integer,
                                                                ByVal Cau_Mov As String,
                                                                ByVal Data_Movimento As Date,
                                                                ByVal BaseCode As Integer,
                                                                ByVal TopCode As Integer
                                                                ) As XmlElement

        Dim i As Integer
        Dim xmlDatiMovDettagli As XmlElement
        Dim xmlMovDettaglio As XmlElement
        Dim movDetDes As String
        Dim prezzoUnitario As Decimal
        Dim prezzoUnitarioNetto As Decimal
        Dim imponibile As Decimal
        Dim imponibileNetto As Decimal
        Dim codIva As Integer
        Dim iva As Decimal
        Dim qta As Decimal
        Dim anno As Integer
        Dim ricCod As Integer
        Dim codConto As Integer


        xmlDatiMovDettagli = XmlDoc.CreateElement("DatiMovimenti_Dettagli")


        If Not IsNothing(DT) Then

            For i = 0 To DT.Rows.Count - 1

                movDetDes = DT.Rows(i).Item("Descrizione")

                prezzoUnitario = DT.Rows(i).Item("Imponibile")
                prezzoUnitarioNetto = DT.Rows(i).Item("Imponibile")

                Select Case Lav_Cod

                    Case LAVCOD_ALTRI_COSTI

                        imponibile = -1 * CDbl(DT.Rows(i).Item("Imponibile"))
                        imponibileNetto = -1 * CDbl(DT.Rows(i).Item("Imponibile"))
                        iva = DT.Rows(i).Item("Iva")

                    Case LAVCOD_ALTRI_RICAVI
                        imponibile = DT.Rows(i).Item("Imponibile")
                        imponibileNetto = DT.Rows(i).Item("Imponibile")
                        iva = -1 * CDbl(DT.Rows(i).Item("Iva"))

                End Select

                codIva = DT.Rows(i).Item("cod_iva")
                anno = DT.Rows(i).Item("Anno")
                ricCod = DT.Rows(i).Item("Ric_Cod")
                codConto = DT.Rows(i).Item("Cod_Conto")

                qta = 1

                xmlMovDettaglio = XML_2_Agenda_MovimentoDettaglio(enum_TipoOperazioneDB.Scrittura,
                                                                  Piva,
                                                                  Sa_Cod,
                                                                  0, 0, 0,
                                                                  movDetDes,
                                                                  0,
                                                                  0,
                                                                  0,
                                                                  0,
                                                                  0,
                                                                  "",
                                                                  0,
                                                                  0,
                                                                  0,
                                                                  qta,
                                                                  0,
                                                                  prezzoUnitario,
                                                                  prezzoUnitarioNetto,
                                                                  imponibile,
                                                                  imponibileNetto,
                                                                  codIva,
                                                                  iva,
                                                                  0,
                                                                  0,
                                                                  anno,
                                                                  ricCod,
                                                                  codConto,
                                                                  MagazzinoMovimentato,
                                                                  CONTABILE,
                                                                  enum_Pendenza.MovESENTE,
                                                                  "",
                                                                  0,
                                                                  AGRODATAINIZIO,
                                                                  Data_Movimento,
                                                                  AGRODATAFINE,
                                                                  BaseCode,
                                                                  TopCode,
                                                                  XmlDoc,
                                                                  0,
                                                                  0,
                                                                  "")

                xmlDatiMovDettagli.AppendChild(xmlMovDettaglio)

            Next

        End If

        Return xmlDatiMovDettagli

    End Function


    '##########################################################################################
    Public Function XML_2_Contabilita_Fattura(ByRef XmlDoc As XmlDocument,
                                              ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                              ByVal Piva As String,
                                              ByVal Sa_Cod_Contab As Integer,
                                              ByVal Lav_Cod As Integer,
                                              ByVal Des_Lib As String,
                                              ByVal Cau_Mov_Magazzino As String,
                                              ByVal Mov_Desc_Contabile As String,
                                              ByVal Mov_Desc_Magazzino As String,
                                              ByVal Extra_Str As String,
                                              ByVal Num_Protocollo As Decimal,
                                              ByVal Doc_Numero_Sin As String,
                                              ByVal Doc_Numero As Decimal,
                                              ByVal Doc_Numero_Des As String,
                                              ByVal Tipo_Sconto As Integer,
                                              ByVal Extra_Int As Integer,
                                              ByVal Peso As Integer,
                                              ByVal Cod_RisUm As Integer,
                                              ByVal Cod_IndirizzoRisUm As Integer,
                                              ByVal Cod_Destinazione As Integer,
                                              ByVal Cod_IndirizzoDestinazione As Integer,
                                              ByVal Mezzo As Integer,
                                              ByVal Cod_Vettore As Integer,
                                              ByVal Cod_IndirizzoVettore As Integer,
                                              ByVal Data_Movimento As Date,
                                              ByVal Data_Registrazione As Date,
                                              ByVal Scadenza As Date,
                                              ByVal Extra_Date As Date,
                                              ByVal XmlDatiPagamenti As XmlElement,
                                              ByVal XmlDatiMovDettagli As XmlElement,
                                              ByVal BaseCode As Integer,
                                              ByVal TopCode As Integer,
                                              ByVal Scadenza_Extra As Date,
                                              ByVal Colli As Integer,
                                              ByVal Aspetto As String,
                                              ByVal Causale_Trasporto As String,
                                              ByVal Natura_Beni As String,
                                              ByVal Modalita As Integer,
                                              ByVal Tara_Veicolo As Decimal,
                                              ByVal Tara_Imballi As Decimal,
                                              ByVal Tipo_Peso As Integer,
                                              ByVal Username_Note As String
                                              ) As String


        Dim xmlDatiAgenda As XmlElement
        Dim xmlAgenda As XmlElement
        Dim xmlDatiMovimenti As XmlElement
        Dim xmlMovimento As XmlElement
        'Dim XmlDatiMovDettagli As XmlElement
        'Dim XmlMovDettaglio As XmlElement

        '#######################################################
        '##################   DATI AGENDA    ###################
        '#######################################################

        xmlDatiAgenda = XmlDoc.CreateElement("DatiAgenda")

        XmlDoc.AppendChild(xmlDatiAgenda)


        '#######################################################
        '#####################   AGENDA    #####################
        '#######################################################

        xmlAgenda = XML_2_Agenda_Agenda(enum_TipoOperazioneDB.Scrittura,
                                        Piva,
                                        Sa_Cod_Contab,
                                        0,
                                        Lav_Cod,
                                        Des_Lib,
                                        , , , ,
                                        Data_Movimento,
                                        AGRODATAFINE,
                                        BaseCode,
                                        TopCode,
                                        XmlDoc)

        xmlDatiAgenda.AppendChild(xmlAgenda)


        '#######################################################
        '################   DATI MOVIMENTI    ##################
        '#######################################################

        xmlDatiMovimenti = XmlDoc.CreateElement("DatiMovimenti")

        xmlAgenda.AppendChild(xmlDatiMovimenti)


        '#######################################################
        '##############   MOVIMENTO CONTABILE   ################
        '#######################################################

        xmlMovimento = XML_2_Agenda_Movimento(enum_TipoOperazioneDB.Scrittura,
                                              Piva,
                                              Sa_Cod_Contab,
                                              0,
                                              0,
                                              CAU_REGISTRAZIONI,
                                              Mov_Desc_Contabile,
                                              Data_Movimento,
                                              "12.00",
                                              Data_Registrazione,
                                              Scadenza,
                                              Scadenza_Extra,
                                              Doc_Numero_Sin, Doc_Numero, Doc_Numero_Des,
                                              Num_Protocollo,
                                              Colli,
                                              Peso,
                                              Aspetto,
                                              Causale_Trasporto,
                                              Tipo_Sconto,
                                              Cod_RisUm,
                                              Cod_IndirizzoRisUm,
                                              Cod_Destinazione,
                                              Cod_IndirizzoDestinazione,
                                              Mezzo,
                                              Cod_Vettore,
                                              Cod_IndirizzoVettore,
                                              Natura_Beni,
                                              Modalita,
                                              Tara_Veicolo,
                                              Tara_Imballi,
                                              Tipo_Peso,
                                              Username_Note,
                                              Extra_Str,
                                              Extra_Int,
                                              Extra_Date,
                                              Data_Movimento,
                                              AGRODATAFINE,
                                              BaseCode,
                                              TopCode,
                                              XmlDoc)

        xmlDatiMovimenti.AppendChild(xmlMovimento)


        '#######################################################
        '##################   PAGAMENTI   ######################
        '#######################################################

        If Not IsNothing(XmlDatiPagamenti) Then

            xmlMovimento.AppendChild(XmlDatiPagamenti)

            'XmlMovimento.InnerText = Str_XML_Pagamenti

        End If


        '#######################################################
        '##############   MOVIMENTO MAGAZZINO   ################
        '#######################################################

        xmlMovimento = XML_2_Agenda_Movimento(enum_TipoOperazioneDB.Scrittura,
                                              Piva,
                                              Sa_Cod_Contab,
                                              0, 0,
                                              Cau_Mov_Magazzino,
                                              Mov_Desc_Magazzino,
                                              Data_Movimento,
                                              "12.00",
                                              Data_Registrazione,
                                              AGRODATAFINE,
                                              ,
                                              , , ,
                                              ,
                                              , , , ,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              0,
                                              , , , , , ,  _
                                              ,
                                              ,
                                              ,
                                              Data_Movimento,
                                              AGRODATAFINE,
                                              BaseCode,
                                              TopCode,
                                              XmlDoc)

        xmlDatiMovimenti.AppendChild(xmlMovimento)


        '#######################################################
        '###############   MOVIMENTI DETTAGLI    ###############
        '#######################################################

        xmlMovimento.AppendChild(XmlDatiMovDettagli)

        Return XmlDoc.OuterXml

    End Function


    '##########################################################################################
    Public Function XML_2_Contabilita_Fattura_Dettagli(ByRef XmlDoc As XmlDocument,
                                                       ByVal DT As DataTable,
                                                       ByVal Piva As String,
                                                       ByVal Sa_Cod_Contab As Integer,
                                                       ByVal Lav_Cod As Integer,
                                                       ByVal Cau_Mov As String,
                                                       ByVal Data_Movimento As Date,
                                                       ByVal BaseCode As Integer,
                                                       ByVal TopCode As Integer
                                                       ) As XmlElement

        Dim i As Integer
        Dim xmlDatiMovDettagli As XmlElement

        Dim strXmlMovimentiDettagli As String = ""
        Dim strTempMovimentiDettagli As String = ""

        xmlDatiMovDettagli = XmlDoc.CreateElement("DatiMovimenti_Dettagli")

        If Not IsNothing(DT) Then

            For i = 0 To DT.Rows.Count - 1

                Select Case Lav_Cod

                    Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA
                        'RICEVIMENTO
                        'carico
                        'destinazione
                        strTempMovimentiDettagli = CStr(DT.Rows(i).Item("Xml_Destinazione"))

                    Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA
                        'EMISSIONE
                        'scarico
                        'provenienza
                        strTempMovimentiDettagli = CStr(DT.Rows(i).Item("Xml_Provenienza"))

                End Select

                strXmlMovimentiDettagli &= strTempMovimentiDettagli

            Next

            xmlDatiMovDettagli.InnerXml = strXmlMovimentiDettagli

        End If

        Return xmlDatiMovDettagli

    End Function

End Module
