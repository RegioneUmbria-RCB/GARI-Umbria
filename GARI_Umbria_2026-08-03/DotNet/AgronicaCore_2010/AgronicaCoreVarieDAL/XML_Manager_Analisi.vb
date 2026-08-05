Imports System.Xml
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Module XML_Manager_Analisi


    '##########################################################################################
    Public Function XML_VariabiliSessione( _
                                        ByVal Progressivo_Gias As String, _
                                        ByVal Cn_Server As String, _
                                        ByVal Cn_Utenti As String, _
                                        ByVal Utente_Usr As String, _
                                        ByVal Utente_Pwd As String, _
                                        ByVal SuperUser_Usr As String, _
                                        ByVal SuperUser_Pwd As String, _
                                        ByVal SuperUser_Piva As String) _
                                        As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("VariabiliSessione")

        'Imposto gli attributi
        XmlTxt.SetAttribute(LCase("Progressivo_Gias"), CStr(Progressivo_Gias))
        XmlTxt.SetAttribute(LCase("Cn_Server"), CStr(Cn_Server))
        XmlTxt.SetAttribute(LCase("Cn_Utenti"), CStr(Cn_Utenti))
        XmlTxt.SetAttribute(LCase("Utente_Usr"), CStr(Utente_Usr))
        XmlTxt.SetAttribute(LCase("Utente_Pwd"), CStr(Utente_Pwd))
        XmlTxt.SetAttribute(LCase("SuperUser_Usr"), CStr(SuperUser_Usr))
        XmlTxt.SetAttribute(LCase("SuperUser_Pwd"), CStr(SuperUser_Pwd))
        XmlTxt.SetAttribute(LCase("SuperUser_Piva"), CStr(SuperUser_Piva))

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function


    '##########################################################################################
    Public Function XML_Analisi_Testata( _
                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                        ByVal Analisi_SuperUser As String, _
                                        Optional ByVal Analisi_Certificato_Cod As Integer = 0, _
                                        Optional ByVal Analisi_Testata_Cod As Integer = 0, _
                                        Optional ByVal Analisi_Testata_Des As String = "", _
                                        Optional ByVal Analisi_Testata_Data_Inizio As Date = #1/1/1900#, _
                                        Optional ByVal Analisi_Testata_Data_Fine As Date = #12/31/2100#, _
                                        Optional ByVal Analisi_Testata_Coord_X As Double = 0, _
                                        Optional ByVal Analisi_Testata_Coord_Y As Double = 0, _
                                        Optional ByVal Analisi_Testata_Riferimento_1 As String = "", _
                                        Optional ByVal Analisi_Testata_Riferimento_2 As String = "", _
                                        Optional ByVal Analisi_Testata_Riferimento_3 As String = "", _
                                        Optional ByVal Analisi_Testata_Riferimento_4 As String = "", _
                                        Optional ByVal Analisi_Testata_Riferimento_5 As String = "", _
                                        Optional ByVal Analisi_Testata_Note1 As String = "", _
                                        Optional ByVal Analisi_Testata_Note2 As String = "", _
                                        Optional ByVal Analisi_Testata_Note3 As String = "", _
                                        Optional ByVal Analisi_Testata_Note4 As String = "", _
                                        Optional ByVal Analisi_Testata_Tipo As Integer = 0, _
                                        Optional ByVal Analisi_Id_Agenda As Integer = 0, _
                                        Optional ByVal Validita_Inizio As Date = #1/1/1900#, _
                                        Optional ByVal Validita_Fine As Date = #12/31/2100#, _
                                        Optional ByVal BaseCode As Integer = 0, _
                                        Optional ByVal TopCode As Integer = 200000000,
                                        Optional ByVal Id_ClasseTessitura As Integer = 0) _
                                        As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Testata")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Analisi_SuperUser"), Analisi_SuperUser)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Cod"), CStr(Analisi_Certificato_Cod))
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Cod"), CStr(Analisi_Testata_Cod))
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Des"), Analisi_Testata_Des)
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Data_Inizio"), Format(Analisi_Testata_Data_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Data_Fine"), Format(Analisi_Testata_Data_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Coord_X"), CStr(Analisi_Testata_Coord_X))
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Coord_Y"), CStr(Analisi_Testata_Coord_Y))
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Riferimento_1"), Analisi_Testata_Riferimento_1)
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Riferimento_2"), Analisi_Testata_Riferimento_2)
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Riferimento_3"), Analisi_Testata_Riferimento_3)
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Riferimento_4"), Analisi_Testata_Riferimento_4)
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Riferimento_5"), Analisi_Testata_Riferimento_5)
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Note1"), Analisi_Testata_Note1)
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Note2"), Analisi_Testata_Note2)
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Note3"), Analisi_Testata_Note3)
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Note4"), Analisi_Testata_Note4)
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Tipo"), CStr(Analisi_Testata_Tipo))
        XmlTxt.SetAttribute(LCase("Analisi_Id_Agenda"), CStr(Analisi_Id_Agenda))
        XmlTxt.SetAttribute(LCase("Id_ClasseTessitura"), CStr(Id_ClasseTessitura))
        XmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        XmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function


    '##########################################################################################
    Public Function XML_Analisi_Certificato( _
                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                        ByVal Analisi_SuperUser As String, _
                                        Optional ByVal Analisi_Certificato_Cod As Integer = 0, _
                                        Optional ByVal Analisi_Certificato_Des As String = "", _
                                        Optional ByVal Analisi_Certificato_Data_Inizio As Date = #1/1/1900#, _
                                        Optional ByVal Analisi_Certificato_Data_Fine As Date = #12/31/2100#, _
                                        Optional ByVal Analisi_Certificato_Laboratorio As String = "", _
                                        Optional ByVal Analisi_Certificato_TipologiaCod As String = "", _
                                        Optional ByVal Analisi_Certificato_TipoCampione As String = "", _
                                        Optional ByVal Analisi_Certificato_Provenienza As String = "", _
                                        Optional ByVal Analisi_Certificato_Verbale As String = "", _
                                        Optional ByVal Analisi_Certificato_Richiedente As String = "", _
                                        Optional ByVal Analisi_Certificato_PrelevatoDa As String = "", _
                                        Optional ByVal Analisi_Certificato_Comune As String = "", _
                                        Optional ByVal Analisi_Certificato_Protocollo As String = "", _
                                        Optional ByVal Analisi_Certificato_NumRegistro As String = "", _
                                        Optional ByVal Analisi_Certificato_Sezione As String = "", _
                                        Optional ByVal Analisi_Certificato_Responsabile As String = "", _
                                        Optional ByVal Analisi_Certificato_Analista As String = "", _
                                        Optional ByVal Analisi_Certificato_Data_Firma As Date = #1/1/1900#, _
                                        Optional ByVal Analisi_Certificato_Validita_Inizio As Date = #1/1/1900#, _
                                        Optional ByVal Analisi_Certificato_Validita_Fine As Date = #12/31/2100#, _
                                        Optional ByVal BaseCode As Integer = 0, _
                                        Optional ByVal TopCode As Integer = 200000000) _
                                        As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Certificato")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Analisi_SuperUser"), Analisi_SuperUser)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Cod"), CStr(Analisi_Certificato_Cod))
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Des"), Analisi_Certificato_Des)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Data_Inizio"), Format(Analisi_Certificato_Data_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Data_Fine"), Format(Analisi_Certificato_Data_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Laboratorio"), Analisi_Certificato_Laboratorio)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_TipologiaCod"), Analisi_Certificato_TipologiaCod)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_TipoCampione"), Analisi_Certificato_TipoCampione)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Provenienza"), Analisi_Certificato_Provenienza)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Verbale"), Analisi_Certificato_Verbale)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Richiedente"), Analisi_Certificato_Richiedente)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_PrelevatoDa"), Analisi_Certificato_PrelevatoDa)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Comune"), Analisi_Certificato_Comune)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Protocollo"), Analisi_Certificato_Protocollo)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_NumRegistro"), Analisi_Certificato_NumRegistro)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Sezione"), Analisi_Certificato_Sezione)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Responsabile"), Analisi_Certificato_Responsabile)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Analista"), Analisi_Certificato_Analista)
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_DataFirma"), Format(Analisi_Certificato_Data_Firma, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Validita_Inizio"), Format(Analisi_Certificato_Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Analisi_Certificato_Validita_Fine"), Format(Analisi_Certificato_Validita_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        XmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function



    '##########################################################################################
    Public Function XML_Analisi_EntitaxTestata( _
                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                        ByVal Analisi_SuperUser As String, _
                                        Optional ByVal Analisi_Testata_Cod As Integer = 0, _
                                        Optional ByVal Analisi_Entita_Cod As Integer = 0, _
                                        Optional ByVal Piva As String = "", _
                                        Optional ByVal Sa_Cod As Integer = 0, _
                                        Optional ByVal Campo_Cod As Integer = 0, _
                                        Optional ByVal Appezza As Integer = 0, _
                                        Optional ByVal Id_Imp As Integer = 0, _
                                        Optional ByVal Fabbricato_Cod As Integer = 0, _
                                        Optional ByVal Prov As String = "", _
                                        Optional ByVal Com As String = "", _
                                        Optional ByVal Sezione As String = "0", _
                                        Optional ByVal Foglio As Integer = 0, _
                                        Optional ByVal Numero As Integer = 0, _
                                        Optional ByVal Subalterno As String = "0", _
                                        Optional ByVal ID_Oggetto_Grafico As String = "0", _
                                        Optional ByVal Validita_Inizio As Date = #1/1/1900#, _
                                        Optional ByVal Validita_Fine As Date = #12/31/2100#, _
                                        Optional ByVal Vas_Cod As Integer = 0) _
                                        As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Analisi_EntitaxTestata")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Analisi_SuperUser"), Analisi_SuperUser)
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Cod"), CStr(Analisi_Testata_Cod))
        XmlTxt.SetAttribute(LCase("Analisi_Entita_Cod"), CStr(Analisi_Entita_Cod))
        XmlTxt.SetAttribute(LCase("Piva"), Piva)
        XmlTxt.SetAttribute(LCase("Sa_Cod"), CStr(Sa_Cod))
        XmlTxt.SetAttribute(LCase("Campo_Cod"), CStr(Campo_Cod))
        XmlTxt.SetAttribute(LCase("Appezza"), CStr(Appezza))
        XmlTxt.SetAttribute(LCase("Id_Imp"), CStr(Id_Imp))
        XmlTxt.SetAttribute(LCase("Fabbricato_Cod"), CStr(Fabbricato_Cod))
        XmlTxt.SetAttribute(LCase("Prov"), Prov)
        XmlTxt.SetAttribute(LCase("Com"), Com)
        XmlTxt.SetAttribute(LCase("Sezione"), Sezione)
        XmlTxt.SetAttribute(LCase("Foglio"), CStr(Foglio))
        XmlTxt.SetAttribute(LCase("Numero"), CStr(Numero))
        XmlTxt.SetAttribute(LCase("Subalterno"), Subalterno)
        XmlTxt.SetAttribute(LCase("ID_Oggetto_Grafico"), ID_Oggetto_Grafico)
        XmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Vas_Cod"), CStr(Vas_Cod))

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function


    '##########################################################################################
    Public Function XML_Analisi_Campione( _
                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                        ByVal Analisi_SuperUser As String, _
                                        Optional ByVal Analisi_Campione_Cod As Integer = 0, _
                                        Optional ByVal Analisi_Campione_Des As String = "", _
                                        Optional ByVal Analisi_Campione_Coord_X As Double = 0, _
                                        Optional ByVal Analisi_Campione_Coord_Y As Double = 0, _
                                        Optional ByVal Analisi_Campione_Quantita As Double = 0, _
                                        Optional ByVal Analisi_Campione_UdM As Integer = 0, _
                                        Optional ByVal Analisi_Campione_Profondita As Double = 0, _
                                        Optional ByVal Analisi_Campione_Profondita_Min As Double = 0, _
                                        Optional ByVal Analisi_Campione_Profondita_Max As Double = 0, _
                                        Optional ByVal Analisi_Campione_Riferimento_1 As String = "", _
                                        Optional ByVal Analisi_Campione_Riferimento_2 As String = "", _
                                        Optional ByVal Analisi_Campione_Riferimento_3 As String = "", _
                                        Optional ByVal Analisi_Campione_Riferimento_4 As String = "", _
                                        Optional ByVal Analisi_Campione_Riferimento_5 As String = "", _
                                        Optional ByVal Analisi_Campione_Note As String = "", _
                                        Optional ByVal Analisi_Campione_Key_Piva As String = "", _
                                        Optional ByVal Analisi_Campione_Key_SaCod As Integer = 0, _
                                        Optional ByVal Analisi_Campione_Key_IDGrafica As String = "", _
                                        Optional ByVal Analisi_Campione_Prov As String = "", _
                                        Optional ByVal Analisi_Campione_Com As String = "", _
                                        Optional ByVal Analisi_Campione_Sezione As String = "0", _
                                        Optional ByVal Analisi_Campione_Foglio As Integer = 0, _
                                        Optional ByVal Analisi_Campione_Numero As Integer = 0, _
                                        Optional ByVal Analisi_Campione_Subalterno As String = "0", _
                                        Optional ByVal Analisi_Campione_Validita_Inizio As Date = #1/1/1900#, _
                                        Optional ByVal Analisi_Campione_Validita_Fine As Date = #12/31/2100#, _
                                        Optional ByVal BaseCode As Integer = 0, _
                                        Optional ByVal TopCode As Integer = 0) As String


        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Campione")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Analisi_SuperUser"), Analisi_SuperUser)
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Cod"), CStr(Analisi_Campione_Cod))
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Des"), Analisi_Campione_Des)
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Coord_X"), CStr(Analisi_Campione_Coord_X))
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Coord_Y"), CStr(Analisi_Campione_Coord_Y))
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Quantita"), CStr(Analisi_Campione_Quantita))
        XmlTxt.SetAttribute(LCase("Analisi_Campione_UdM"), CStr(Analisi_Campione_UdM))
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Profondita"), CStr(Analisi_Campione_Profondita))
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Profondita_Min"), CStr(Analisi_Campione_Profondita_Min))
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Profondita_Max"), CStr(Analisi_Campione_Profondita_Max))
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Riferimento_1"), Analisi_Campione_Riferimento_1)
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Riferimento_2"), Analisi_Campione_Riferimento_2)
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Riferimento_3"), Analisi_Campione_Riferimento_3)
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Riferimento_4"), Analisi_Campione_Riferimento_4)
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Riferimento_5"), Analisi_Campione_Riferimento_5)
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Note"), Analisi_Campione_Note)
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Key_Piva"), Analisi_Campione_Key_Piva)
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Key_SaCod"), Analisi_Campione_Key_SaCod)
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Key_IDGrafica"), CStr(Analisi_Campione_Key_IDGrafica))
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Prov"), Analisi_Campione_Prov)
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Com"), Analisi_Campione_Com)
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Sezione"), Analisi_Campione_Sezione)
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Foglio"), CStr(Analisi_Campione_Foglio))
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Numero"), CStr(Analisi_Campione_Numero))
        XmlTxt.SetAttribute(LCase("Analisi_Campione_Subalterno"), Analisi_Campione_Subalterno)
        XmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Analisi_Campione_Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Analisi_Campione_Validita_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("basecode"), BaseCode)
        XmlTxt.SetAttribute(LCase("topcode"), TopCode)

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function


    '##########################################################################################
    Public Function XML_Analisi_Dettaglio( _
                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                        ByVal Analisi_SuperUser As String, _
                                        ByVal Analisi_Testata_Cod As Integer, _
                                        Optional ByVal Analisi_Dettaglio_Cod As String = "0", _
                                        Optional ByVal Analisi_Parametro_Cod As Integer = 1, _
                                        Optional ByVal Analisi_Dettaglio_Valore_1 As Double = 0, _
                                        Optional ByVal Analisi_Dettaglio_MargineErrore_1 As Double = 0, _
                                        Optional ByVal Analisi_Dettaglio_Valore_2 As Double = 0, _
                                        Optional ByVal Analisi_Dettaglio_MargineErrore_2 As Double = 0, _
                                        Optional ByVal Analisi_Dettaglio_Validita_Inizio As Date = #1/1/1900#, _
                                        Optional ByVal Analisi_Dettaglio_Validita_Fine As Date = #12/31/2100#) _
                                        As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Dettaglio")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("Analisi_SuperUser"), Analisi_SuperUser)
        XmlTxt.SetAttribute(LCase("Analisi_Testata_Cod"), CStr(Analisi_Testata_Cod))
        XmlTxt.SetAttribute(LCase("Analisi_Dettaglio_Cod"), CStr(Analisi_Dettaglio_Cod))
        XmlTxt.SetAttribute(LCase("Analisi_Parametro_Cod"), CStr(Analisi_Parametro_Cod))
        XmlTxt.SetAttribute(LCase("Analisi_Dettaglio_Valore_1"), CStr(Analisi_Dettaglio_Valore_1))
        XmlTxt.SetAttribute(LCase("Analisi_Dettaglio_MargineErrore_1"), CStr(Analisi_Dettaglio_MargineErrore_1))
        XmlTxt.SetAttribute(LCase("Analisi_Dettaglio_Valore_2"), CStr(Analisi_Dettaglio_Valore_2))
        XmlTxt.SetAttribute(LCase("Analisi_Dettaglio_MargineErrore_2"), CStr(Analisi_Dettaglio_MargineErrore_2))
        XmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Analisi_Dettaglio_Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Analisi_Dettaglio_Validita_Fine, "dd/MM/yyyy"))

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function



    '###############################################################################################
    Public Sub XML_EstraiVariabiliStampe(ByVal strXml As String, ByRef htVariabiliStampe As System.Collections.Hashtable, ByRef strErr As String)

        Dim XmlDoc As System.Xml.XmlDocument
        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement
        Dim xmlAttributo As System.Xml.XmlAttribute
        Dim i, x As Integer

        Try

            'Carico la stringa xml in un nuovo documento
            XmlDoc = New System.Xml.XmlDocument
            XmlDoc.LoadXml(strXml)

            If XmlDoc.HasChildNodes Then

                XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")

                '----- Tag VariabiliStampe (multiplo)

                XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

                For x = 0 To XMLs_VariabiliStampe.Count - 1

                    XML_VariabiliStampe = XMLs_VariabiliStampe.Item(x)

                    i = 0
                    htVariabiliStampe = New System.Collections.Hashtable

                    'Recupero l'insieme dei nodi 
                    'XML_VariabiliStampe = XmlDoc.FirstChild

                    If XML_VariabiliStampe.HasAttributes Then

                        For Each xmlAttributo In XML_VariabiliStampe.Attributes()

                            If Not htVariabiliStampe.ContainsKey(xmlAttributo.Name) Then

                                htVariabiliStampe.Add(xmlAttributo.Name, xmlAttributo.Value)

                            End If

                        Next

                    End If


                Next


            End If

        Catch ex As Exception

            strErr = ex.Message
            htVariabiliStampe = Nothing

        End Try

    End Sub



End Module
