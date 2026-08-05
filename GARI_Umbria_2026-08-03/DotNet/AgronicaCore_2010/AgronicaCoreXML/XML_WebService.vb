Imports System.Xml
Imports System.Text
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class XML_WS_Importa_Gias

    '##########################################################
    'genera la stringa xml per le credenziali
    'ovvero Str_Parametri1 per Importa_DataRaccoltaPrevista
    'e Credenziali per Importa_DocumentoPubblico_SuperServer
    Public Function Genera_Stringa_Credenziali(ByVal Flag_CriptaStringa As Boolean, _
                                                ByRef XmlDoc As XmlDocument, _
                                                ByVal Utente_Username As String, _
                                                ByVal Utente_Password As String, _
                                                ByVal PivaSuperUser As String, _
                                                ByVal Super_Server As String, _
                                                ByVal Super_Server_Provider As String, _
                                                ByVal Super_Server_Server As String, _
                                                ByVal Super_Server_DB As String, _
                                                ByVal Super_Server_UserId As String, _
                                                ByVal Super_Server_Password As String, _
                                                ByVal Super_Server_FiltroAgg As String, _
                                                ByVal StrConnessioneServer As String, _
                                                ByVal StrConnessioneUtenti As String _
                                                    ) As String

        'Str_Parametri1:
        '   <CREDENZIALI>
        '           <USER></USER>
        '           <PWD></PWD>
        '           <Super_Server></Super_Server>
        '           <Super_Server_Provider></Super_Server_Provider>
        '           <Super_Server_Server></Super_Server_Server>
        '           <Super_Server_DB></Super_Server_DB>
        '           <Super_Server_UserId></Super_Server_UserId>
        '           <Super_Server_Password></Super_Server_Password>
        '           <Super_Server_FiltroAgg></Super_Server_FiltroAgg>
        '           <StrConnessioneServer></Super_Server_Password>
        '           <StrConnessioneUtenti></Super_Server_Password>
        '   </CREDENZIALI>

        Dim Str_XML_Credenziali As String
        Dim Xmlcredenziali As XmlElement
        Dim XmlEl As XmlElement

        If IsNothing(XmlDoc) Then
            XmlDoc = New XmlDocument
        End If

        Xmlcredenziali = XmlDoc.CreateElement("credenziali")
        XmlDoc.AppendChild(Xmlcredenziali)

        XmlEl = XmlDoc.CreateElement("user")
        XmlEl.InnerText = Utente_Username
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("pwd")
        XmlEl.InnerText = Utente_Password
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("PivaSuperUser".ToLower)
        XmlEl.InnerText = PivaSuperUser
        Xmlcredenziali.AppendChild(XmlEl)

        'CASO 1
        XmlEl = XmlDoc.CreateElement("Super_Server".ToLower)
        XmlEl.InnerText = Super_Server
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("Super_Server_Provider".ToLower)
        XmlEl.InnerText = Super_Server_Provider
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("Super_Server_Server".ToLower)
        XmlEl.InnerText = Super_Server_Server
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("Super_Server_DB".ToLower)
        XmlEl.InnerText = Super_Server_DB
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("Super_Server_UserId".ToLower)
        XmlEl.InnerText = Super_Server_UserId
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("Super_Server_Password".ToLower)
        XmlEl.InnerText = Super_Server_Password
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("Super_Server_FiltroAgg".ToLower)
        XmlEl.InnerText = Super_Server_FiltroAgg
        Xmlcredenziali.AppendChild(XmlEl)

        'CASO 2
        XmlEl = XmlDoc.CreateElement("StrConnessioneServer".ToLower)
        XmlEl.InnerText = StrConnessioneServer
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("StrConnessioneUtenti".ToLower)
        XmlEl.InnerText = StrConnessioneUtenti
        Xmlcredenziali.AppendChild(XmlEl)

        Str_XML_Credenziali = XmlDoc.OuterXml

        If Flag_CriptaStringa = True Then
            Dim objSecurity As New AgronicaCoreDataProvider.Sicurezza
            Str_XML_Credenziali = objSecurity.Stringa_Codifica_WebService(Str_XML_Credenziali)
        End If

        Return Str_XML_Credenziali

    End Function


    '##########################################################
    'genera la stringa xml dei parametri da inviare alla funzione Importa_DataRaccoltaPrevista
    'ovvero Str_Parametri2
    Public Function Genera_Stringa_Parametri_ImportaDataRaccoltaPrevista( _
                                                ByVal Flag_CriptaStringa As Boolean, _
                                               ByRef XmlDoc As XmlDocument, _
                                               ByVal Piva As String, _
                                               ByVal Sa_Cod As String, _
                                               ByVal Appezza As String, _
                                               ByVal Id_Reg As String, _
                                               ByVal Cod_Progetto As String, _
                                               ByVal Data_Raccolta As String) As String

        'Str_Parametri2:
        '   <parametri>
        '           <Piva></Piva>
        '           <Sa_Cod></Sa_Cod>
        '           <Appezza></Appezza>
        '           <Id_Reg></Id_Reg>
        '           <Cod_Progetto></Cod_Progetto>
        '           <Data_Raccolta></Data_Raccolta>
        '   </parametri>

        Dim Str_XML_Parametri As String
        Dim Xmlparametri As XmlElement
        Dim Xmlpiva As XmlElement
        Dim Xmlsa_cod As XmlElement
        Dim Xmlappezza As XmlElement
        Dim Xmlid_reg As XmlElement
        Dim Xmlcod_progetto As XmlElement
        Dim Xmldata_raccolta As XmlElement

        If IsNothing(XmlDoc) Then
            XmlDoc = New XmlDocument
        End If

        Xmlparametri = XmlDoc.CreateElement("parametri")
        XmlDoc.AppendChild(Xmlparametri)

        Xmlpiva = XmlDoc.CreateElement("piva")
        Xmlpiva.InnerText = Piva
        Xmlparametri.AppendChild(Xmlpiva)

        Xmlsa_cod = XmlDoc.CreateElement("sa_cod")
        Xmlsa_cod.InnerText = Sa_Cod
        Xmlparametri.AppendChild(Xmlsa_cod)

        Xmlappezza = XmlDoc.CreateElement("appezza")
        Xmlappezza.InnerText = Appezza
        Xmlparametri.AppendChild(Xmlappezza)

        Xmlid_reg = XmlDoc.CreateElement("id_reg")
        Xmlid_reg.InnerText = Id_Reg
        Xmlparametri.AppendChild(Xmlid_reg)

        Xmlcod_progetto = XmlDoc.CreateElement("cod_progetto")
        Xmlcod_progetto.InnerText = Cod_Progetto
        Xmlparametri.AppendChild(Xmlcod_progetto)

        Xmldata_raccolta = XmlDoc.CreateElement("data_raccolta")
        Xmldata_raccolta.InnerText = Data_Raccolta
        Xmlparametri.AppendChild(Xmldata_raccolta)

        Str_XML_Parametri = XmlDoc.OuterXml

        If Flag_CriptaStringa = True Then
            Dim objSecurity As New AgronicaCoreDataProvider.Sicurezza
            Str_XML_Parametri = objSecurity.Stringa_Codifica_WebService(Str_XML_Parametri)
        End If

        Return Str_XML_Parametri

    End Function

    '##########################################################
    'decodifica l'xml di risposta del webservice
    'eventualmente la decripta
    Public Sub Decodifica_StrXMLRisposta_Generale(ByVal Flag_StringaCriptata As Boolean, _
                                                 ByVal Str_XML_Risposta As String, _
                                                 ByRef Msg_Errore As String, _
                                                 ByRef Msg_Risposta As String)

        'tutto in minuscolo!!!!!!!!!!!!!!!!!!!!!
        '<RISULTATO>
        '   <ERRORE></ERRORE>
        '   <RISPOSTA>risposta</RISPOSTA>
        '</RISULTATO>

        'questo xml viene creato da WS_Importa_Magazzino_2010.GestioneRisposta.vb: Genera_Stringa_RispostaGenerale

        Dim objSecurity As New AgronicaCoreDataProvider.Sicurezza

        If Flag_StringaCriptata = True Then
            Str_XML_Risposta = objSecurity.Stringa_Decodifica_WebService(Str_XML_Risposta)
        End If

        Dim XmlDoc As New XmlDocument
        Dim XML_Risultato As XmlElement
        Dim XML_Errore As XmlElement
        Dim XML_Risposta As XmlElement

        XmlDoc.LoadXml(Str_XML_Risposta)

        XML_Risultato = XmlDoc.SelectSingleNode("risultato")
        XML_Errore = XML_Risultato.SelectSingleNode("errore")
        Msg_Errore = XML_Errore.InnerText
        XML_Risposta = XML_Risultato.SelectSingleNode("risposta")
        Msg_Risposta = XML_Risposta.InnerText


    End Sub


End Class


Public Class XML_WS_importa_Magazzino_2010

    '##########################################################
    'genera la stringa xml per le credenziali
    'ovvero Str_Parametri1 
    Public Function Genera_Stringa_Credenziali_ImportaMagazzino(ByVal Flag_CriptaStringa As Boolean, _
                                                                 ByRef XmlDoc As XmlDocument, _
                                                                 ByVal Utente_Username As String, _
                                                                 ByVal Utente_Password As String, _
                                                                 ByVal PivaSuperUser As String, _
                                                                ByVal Super_Server As String, _
                                                                ByVal Super_Server_Provider As String, _
                                                                ByVal Super_Server_Server As String, _
                                                                ByVal Super_Server_DB As String, _
                                                                ByVal Super_Server_UserId As String, _
                                                                ByVal Super_Server_Password As String, _
                                                                ByVal StrConnessioneServer As String, _
                                                                ByVal StrConnessioneUtenti As String _
                                                                  ) As String

        '1)
        'i dati del superserver vengono passati quando l'applicazione è schedulata
        '(e quei dati sono settati su app.config)
        '2)
        'le stringhe di connessione vengono passate quando l'import è chiamata da una pagina del dincronizzatore
        'quindi le stringhe di connessione sono prese dall'objparametri del sito

        'Str_Parametri1:
        '   <CREDENZIALI>
        '           <USER></USER>
        '           <PWD></PWD>
        '           <Super_Server></Super_Server>
        '           <Super_Server_Provider></Super_Server_Provider>
        '           <Super_Server_Server></Super_Server_Server>
        '           <Super_Server_DB></Super_Server_DB>
        '           <Super_Server_UserId></Super_Server_UserId>
        '           <Super_Server_Password></Super_Server_Password>
        '           <StrConnessioneServer></Super_Server_Password>
        '           <StrConnessioneUtenti></Super_Server_Password>
        '   </CREDENZIALI>

        Dim Str_XML_Credenziali As String
        Dim Xmlcredenziali As XmlElement
        Dim XmlEl As XmlElement
        'Dim Xmlpwd As XmlElement
        'Dim Xmlcn_server As XmlElement
        'Dim Xmlcn_utenti As XmlElement

        If IsNothing(XmlDoc) Then
            XmlDoc = New XmlDocument
        End If

        Xmlcredenziali = XmlDoc.CreateElement("credenziali")
        XmlDoc.AppendChild(Xmlcredenziali)

        XmlEl = XmlDoc.CreateElement("user")
        XmlEl.InnerText = Utente_Username
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("pwd")
        XmlEl.InnerText = Utente_Password
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("PivaSuperUser".ToLower)
        XmlEl.InnerText = PivaSuperUser
        Xmlcredenziali.AppendChild(XmlEl)

        'CASO 1
        XmlEl = XmlDoc.CreateElement("Super_Server".ToLower)
        XmlEl.InnerText = Super_Server
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("Super_Server_Provider".ToLower)
        XmlEl.InnerText = Super_Server_Provider
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("Super_Server_Server".ToLower)
        XmlEl.InnerText = Super_Server_Server
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("Super_Server_DB".ToLower)
        XmlEl.InnerText = Super_Server_DB
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("Super_Server_UserId".ToLower)
        XmlEl.InnerText = Super_Server_UserId
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("Super_Server_Password".ToLower)
        XmlEl.InnerText = Super_Server_Password
        Xmlcredenziali.AppendChild(XmlEl)

        'CASO 2
        XmlEl = XmlDoc.CreateElement("StrConnessioneServer".ToLower)
        XmlEl.InnerText = StrConnessioneServer
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("StrConnessioneUtenti".ToLower)
        XmlEl.InnerText = StrConnessioneUtenti
        Xmlcredenziali.AppendChild(XmlEl)

        Str_XML_Credenziali = XmlDoc.OuterXml

        If Flag_CriptaStringa = True Then
            Dim objSecurity As New AgronicaCoreDataProvider.Sicurezza
            Str_XML_Credenziali = objSecurity.Stringa_Codifica_WebService(Str_XML_Credenziali)
        End If

        Return Str_XML_Credenziali

    End Function

    '##########################################################
    'genera la stringa xml per le credenziali
    'ovvero Str_Parametri1 
    Public Function Genera_Stringa_Credenziali_ImportaMagazzino_OLD(ByVal Flag_CriptaStringa As Boolean, _
                                                                    ByRef XmlDoc As XmlDocument, _
                                                                    ByVal Utente_Username As String, _
                                                                    ByVal Utente_Password As String, _
                                                                    ByVal Cn_Server As String, _
                                                                    ByVal Cn_Utenti As String _
                                                                     ) As String

        'ByVal Cn_Server As String, _
        ' ByVal Cn_Utenti As String
        '           <CN_SERVER></CN_SERVER>
        '           <CN_UTENTI></CN_UTENTI>

        'Str_Parametri1:
        '   <CREDENZIALI>
        '           <USER></USER>
        '           <PWD></PWD>
        '           <Super_Server></Super_Server>
        '           <Super_Server_Provider></Super_Server_Provider>
        '           <Super_Server_Server></Super_Server_Server>
        '           <Super_Server_DB></Super_Server_DB>
        '           <Super_Server_UserId></Super_Server_UserId>
        '           <Super_Server_Password></Super_Server_Password>
        '   </CREDENZIALI>

        Dim Str_XML_Credenziali As String
        Dim Xmlcredenziali As XmlElement
        Dim XmlEl As XmlElement

        If IsNothing(XmlDoc) Then
            XmlDoc = New XmlDocument
        End If

        Xmlcredenziali = XmlDoc.CreateElement("credenziali")
        XmlDoc.AppendChild(Xmlcredenziali)

        XmlEl = XmlDoc.CreateElement("user")
        XmlEl.InnerText = Utente_Username
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("pwd")
        XmlEl.InnerText = Utente_Password
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("cn_server")
        XmlEl.InnerText = Cn_Server
        Xmlcredenziali.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("cn_utenti")
        XmlEl.InnerText = Cn_Utenti
        Xmlcredenziali.AppendChild(XmlEl)

        Str_XML_Credenziali = XmlDoc.OuterXml

        If Flag_CriptaStringa = True Then
            Dim objSecurity As New AgronicaCoreDataProvider.Sicurezza
            Str_XML_Credenziali = objSecurity.Stringa_Codifica_WebService(Str_XML_Credenziali)
        End If

        Return Str_XML_Credenziali

    End Function


    '##########################################################
    'genera la stringa xml per le credenziali
    'ovvero Str_Parametri1 
    Public Function Genera_Stringa_Credenziali_LeggiMagazzino(ByVal Flag_CriptaStringa As Boolean, _
                                                               ByRef XmlDoc As XmlDocument, _
                                                               ByVal Utente_Username As String, _
                                                               ByVal Utente_Password As String, _
                                                               ByVal Id_Attivita As enum_Security_Attivita, _
                                                               ByVal Id_Operazione As enum_TipoOperazioneDB) As String

        'Str_Parametri1:
        '(tutto in minuscolo!)
        '   <CREDENZIALI>
        '           <USER></USER>
        '           <PWD></PWD>
        '           <ID_ATTIVITA></ID_ATTIVITA>
        '           <ID_OPERAZIONE></ID_OPERAZIONE>
        '   </CREDENZIALI>

        Dim Str_XML_Credenziali As String
        Dim Xmlcredenziali As XmlElement
        Dim Xml As XmlElement

        If IsNothing(XmlDoc) Then
            XmlDoc = New XmlDocument
        End If

        Xmlcredenziali = XmlDoc.CreateElement("credenziali")
        XmlDoc.AppendChild(Xmlcredenziali)

        Xml = XmlDoc.CreateElement("user")
        Xml.InnerText = Utente_Username
        Xmlcredenziali.AppendChild(Xml)

        Xml = XmlDoc.CreateElement("pwd")
        Xml.InnerText = Utente_Password
        Xmlcredenziali.AppendChild(Xml)

        Xml = XmlDoc.CreateElement("id_attivita")
        Xml.InnerText = Id_Attivita
        Xmlcredenziali.AppendChild(Xml)

        Xml = XmlDoc.CreateElement("id_operazione")
        Xml.InnerText = Id_Operazione
        Xmlcredenziali.AppendChild(Xml)

        Str_XML_Credenziali = XmlDoc.OuterXml

        If Flag_CriptaStringa = True Then
            Dim objSecurity As New AgronicaCoreDataProvider.Sicurezza
            Str_XML_Credenziali = objSecurity.Stringa_Codifica_WebService(Str_XML_Credenziali)
        End If

        Return Str_XML_Credenziali

    End Function


    '##########################################################
    'genera la stringa xml dei parametri da inviare alla funzione RecuperaChiaviDocumentiAcquisto
    'ovvero Str_Parametri2
    Public Function Genera_Stringa_Parametri_RecuperaChiaviDocumentiAcquisto( _
                                            ByVal Flag_CriptaStringa As Boolean, _
                                               ByRef XmlDoc As XmlDocument, _
                                               ByVal Piva As String, _
                                                ByVal piva_fornitore As String, _
                                                ByVal tipo_documento As Integer, _
                                               ByVal Data_Inizio As Date, _
                                                ByVal Data_Fine As Date, _
                                                ByVal Flag_Prodotti_BdGias As Boolean _
                                                ) As String

        'Str_Parametri2:
        '   <PARAMETRI>
        '           <PIVA></PIVA>
        '           <LAV_COD></LAV_COD>
        '           <DATA_INIZIO></DATA_INIZIO>
        '           <DATA_FINE></DATA_FINE>
        '           <FLAG_PRODOTTI_BDGIAS></FLAG_PRODOTTI_BDGIAS>
        '   </PARAMETRI>

        Dim Str_XML_Parametri As String
        Dim Xmlparametri As XmlElement
        Dim Xmlel As XmlElement

        If IsNothing(XmlDoc) Then
            XmlDoc = New XmlDocument
        End If

        Xmlparametri = XmlDoc.CreateElement("parametri")
        XmlDoc.AppendChild(Xmlparametri)

        Xmlel = XmlDoc.CreateElement("piva")
        Xmlel.InnerText = Piva
        Xmlparametri.AppendChild(Xmlel)

        'Xmlel = XmlDoc.CreateElement("lav_cod")
        'Xmlel.InnerText = Lav_Cod
        'Xmlparametri.AppendChild(Xmlel)

        Xmlel = XmlDoc.CreateElement("piva_fornitore")
        Xmlel.InnerText = piva_fornitore
        Xmlparametri.AppendChild(Xmlel)

        Xmlel = XmlDoc.CreateElement("tipo_documento")
        Xmlel.InnerText = tipo_documento
        Xmlparametri.AppendChild(Xmlel)

        Xmlel = XmlDoc.CreateElement("Data_Inizio".ToLower)
        Xmlel.InnerText = Data_Inizio
        Xmlparametri.AppendChild(Xmlel)

        Xmlel = XmlDoc.CreateElement("Data_Fine".ToLower)
        Xmlel.InnerText = Data_Fine
        Xmlparametri.AppendChild(Xmlel)

        Xmlel = XmlDoc.CreateElement("Flag_Prodotti_BdGias".ToLower)
        Xmlel.InnerText = Flag_Prodotti_BdGias
        Xmlparametri.AppendChild(Xmlel)

        'Xmlel = XmlDoc.CreateElement("Flag_FattureDifferite".ToLower)
        'Xmlel.InnerText = Flag_FattureDifferite
        'Xmlparametri.AppendChild(Xmlel)

        Str_XML_Parametri = XmlDoc.OuterXml

        If Flag_CriptaStringa = True Then
            Dim objSecurity As New AgronicaCoreDataProvider.Sicurezza
            Str_XML_Parametri = objSecurity.Stringa_Codifica_WebService(Str_XML_Parametri)
        End If

        Return Str_XML_Parametri

    End Function



    '##########################################################
    'genera la stringa xml dei parametri da inviare alla funzione VerificaEsistenzaDocumento
    'ovvero Str_Parametri2
    Public Function Genera_Stringa_Parametri_VerificaEsistenzaDocumento( _
                                            ByVal Flag_CriptaStringa As Boolean, _
                                               ByRef XmlDoc As XmlDocument, _
                                               ByVal Piva_Impresa As String, _
                                               ByVal Piva_Fornitore As String, _
                                                ByVal Codice_Fornitore As String, _
                                               ByVal Lav_Cod As String, _
                                               ByVal Anno As String, _
                                               ByVal Data As String, _
                                               ByVal Doc_Numero_Sin As String, _
                                               ByVal Doc_Numero As String, _
                                               ByVal Doc_Numero_Des As String) As String

        'Str_Parametri2:
        '(tutto minuscolo)
        '   <PARAMETRI>
        '           <Piva_impresa></Piva_impresa>
        '           <Piva_fornitore></Piva_fornitore>
        '           <Codice_Fornitore></Codice_Fornitore>
        '           <LAV_COD></LAV_COD>
        '           <ANNO></ANNO>
        '           <DATA></DATA>
        '           <Doc_Numero_Sin></Doc_Numero_Sin>
        '           <Doc_Numero></Doc_Numero>
        '           <Doc_Numero_Des></Doc_Numero_Des>
        '   </PARAMETRI>

        Dim Str_XML_Parametri As String
        Dim Xmlparametri As XmlElement
        Dim XmlEl As XmlElement

        If IsNothing(XmlDoc) Then
            XmlDoc = New XmlDocument
        End If

        Xmlparametri = XmlDoc.CreateElement("parametri")
        XmlDoc.AppendChild(Xmlparametri)

        XmlEl = XmlDoc.CreateElement("piva_impresa")
        XmlEl.InnerText = Piva_Impresa
        Xmlparametri.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("piva_fornitore")
        XmlEl.InnerText = Piva_Fornitore
        Xmlparametri.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("codice_fornitore")
        XmlEl.InnerText = Codice_Fornitore
        Xmlparametri.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("lav_cod")
        XmlEl.InnerText = Lav_Cod
        Xmlparametri.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("anno")
        XmlEl.InnerText = Anno
        Xmlparametri.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("data")
        XmlEl.InnerText = Data
        Xmlparametri.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("doc_numero_sin")
        XmlEl.InnerText = Doc_Numero_Sin
        Xmlparametri.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("doc_numero")
        XmlEl.InnerText = Doc_Numero
        Xmlparametri.AppendChild(XmlEl)

        XmlEl = XmlDoc.CreateElement("doc_numero_des")
        XmlEl.InnerText = Doc_Numero_Des
        Xmlparametri.AppendChild(XmlEl)

        Str_XML_Parametri = XmlDoc.OuterXml

        If Flag_CriptaStringa = True Then
            Dim objSecurity As New AgronicaCoreDataProvider.Sicurezza
            Str_XML_Parametri = objSecurity.Stringa_Codifica_WebService(Str_XML_Parametri)
        End If

        Return Str_XML_Parametri

    End Function


    ''##########################################################
    ''genera la stringa xml dei parametri da inviare alla funzione VerificaEsistenzaDocumento
    ''ovvero Str_Parametri2
    'Public Function Genera_Stringa_Parametri_TestCollegamento( _
    '                                                    ByVal Flag_CriptaStringa As Boolean, _
    '                                                   ByRef XmlDoc As XmlDocument, _
    '                                                   ByVal Addendo1 As Integer, _
    '                                                   ByVal Addendo2 As Integer) As String

    '    'Str_Parametri2:
    '    '   <PARAMETRI>
    '    '           <Addendo1></Addendo1>
    '    '           <Addendo2></Addendo2>
    '    '   </PARAMETRI>

    '    Dim Str_XML_Parametri As String
    '    Dim Xmlparametri As XmlElement
    '    Dim XmlAddendo1 As XmlElement
    '    Dim XmlAddendo2 As XmlElement

    '    If IsNothing(XmlDoc) Then
    '        XmlDoc = New XmlDocument
    '    End If

    '    Xmlparametri = XmlDoc.CreateElement("parametri")
    '    XmlDoc.AppendChild(Xmlparametri)

    '    XmlAddendo1 = XmlDoc.CreateElement("Addendo1")
    '    XmlAddendo1.InnerText = Addendo1
    '    Xmlparametri.AppendChild(XmlAddendo1)

    '    XmlAddendo2 = XmlDoc.CreateElement("Addendo2")
    '    XmlAddendo2.InnerText = Addendo2
    '    Xmlparametri.AppendChild(XmlAddendo2)

    '    Str_XML_Parametri = XmlDoc.OuterXml

    '    If Flag_CriptaStringa = True Then
    '        Dim objSecurity As New AgronicaCoreDataProvider.Sicurezza
    '        Str_XML_Parametri = objSecurity.Stringa_Codifica_WebService(Str_XML_Parametri)
    '    End If

    '    Return Str_XML_Parametri

    'End Function


    '##########################################################
    'genera la prima parte della stringa xml dei parametri da inviare alla funzione VerificaCodificaProdotti
    'ovvero Str_Parametri2
    Public Function Genera_Stringa_Parametri_VerificaCodificaProdotti_1Radice(ByRef XmlDoc As XmlDocument) As String

        'tutto in minuscolo!!!!
        'Str_Parametri2:
        '   <PARAMETRI>
        '   </PARAMETRI>

        Dim Str_XML_Parametri As String
        Dim Xmlparametri As XmlElement

        If IsNothing(XmlDoc) Then
            XmlDoc = New XmlDocument
        End If

        Xmlparametri = XmlDoc.CreateElement("parametri")
        XmlDoc.AppendChild(Xmlparametri)

        Str_XML_Parametri = XmlDoc.OuterXml

        Return Str_XML_Parametri

    End Function

    '##########################################################
    'genera la 2 parte della stringa xml dei parametri da inviare alla funzione VerificaCodificaProdotti
    'ovvero Str_Parametri2
    Public Function Genera_Stringa_Parametri_VerificaCodificaProdotti_2Impresa(ByRef XmlDoc As XmlDocument, _
                                                                                ByVal Piva_Impresa As String) As String

        'tutto in minuscolo!!!!
        'Str_Parametri2:
        '   <PARAMETRI>
        '       <impresa piva="">
        '       </impresa>
        '   </PARAMETRI>

        Dim Str_XML_Parametri As String
        Dim XmlParametri As XmlElement
        Dim XmlImpresa As XmlElement

        XmlParametri = XmlDoc.SelectSingleNode("parametri")

        XmlImpresa = XmlDoc.CreateElement("impresa")
        XmlImpresa.SetAttribute("piva", Piva_Impresa)
        XmlParametri.AppendChild(XmlImpresa)

        Str_XML_Parametri = XmlDoc.OuterXml

        Return Str_XML_Parametri

    End Function

    '##########################################################
    'genera la terza parte della stringa xml dei parametri da inviare alla funzione VerificaCodificaProdotti
    'ovvero Str_Parametri2
    Public Function Genera_Stringa_Parametri_VerificaCodificaProdotti_3Prodotto(ByRef XmlDoc As XmlDocument, _
                                                                                   ByVal Piva_Impresa As String, _
                                                                                   ByVal Piva_Codifica As String, _
                                                                                    ByVal Elem_Cod As Integer, _
                                                                                    ByVal Cod_Prodotto As String, _
                                                                                    ByVal Desc_Prodotto As String, _
                                                                                    ByVal cod_articolo As String, _
                                                                                    ByVal Note As String) As String

        'tutto in minuscolo!!!!
        'Str_Parametri2:
        '   <PARAMETRI>
        '       <impresa piva="">
        '           <prodotto>
        '               <piva_codifica></piva_codifica>
        '               <elem_cod></elem_cod>
        '               <cod_prodotto></cod_prodotto>
        '               <desc_prodotto></desc_prodotto>
        '               <cod_articolo></cod_articolo> 'serve per le sementi
        '               <note></note> 
        '           </prodotto>
        '       </impresa>
        '   </PARAMETRI>

        Dim Str_XML_Parametri As String
        Dim XML_prodotto, XML_piva_codifica, XML_elem_cod, XML_cod_prodotto, XML_desc_prodotto, XML_cod_articolo, XML_note As XmlElement
        Dim XmlImpresa As XmlElement

        XmlImpresa = XmlDoc.SelectSingleNode("//impresa[@piva='" + Piva_Impresa + "']")

        XML_prodotto = XmlDoc.CreateElement("prodotto")
        XmlImpresa.AppendChild(XML_prodotto)

        XML_piva_codifica = XmlDoc.CreateElement("piva_codifica")
        XML_piva_codifica.InnerText = Piva_Codifica
        XML_prodotto.AppendChild(XML_piva_codifica)

        XML_elem_cod = XmlDoc.CreateElement("elem_cod")
        XML_elem_cod.InnerText = Elem_Cod
        XML_prodotto.AppendChild(XML_elem_cod)

        XML_cod_prodotto = XmlDoc.CreateElement("cod_prodotto")
        XML_cod_prodotto.InnerText = Cod_Prodotto
        XML_prodotto.AppendChild(XML_cod_prodotto)

        XML_desc_prodotto = XmlDoc.CreateElement("desc_prodotto")
        XML_desc_prodotto.InnerText = Desc_Prodotto
        XML_prodotto.AppendChild(XML_desc_prodotto)

        XML_cod_articolo = XmlDoc.CreateElement("cod_articolo")
        XML_cod_articolo.InnerText = cod_articolo
        XML_prodotto.AppendChild(XML_cod_articolo)

        XML_note = XmlDoc.CreateElement("note")
        XML_note.InnerText = Note
        XML_prodotto.AppendChild(XML_note)

        Str_XML_Parametri = XmlDoc.OuterXml

        Return Str_XML_Parametri

    End Function


    '##########################################################
    'decodifica l'xml di risposta del webservice
    'eventualmente la decripta
    Public Sub Decodifica_StrXMLRisposta_Generale(ByVal Flag_StringaCriptata As Boolean, _
                                                 ByVal Str_XML_Risposta As String, _
                                                 ByRef Msg_Errore As String, _
                                                 ByRef Msg_Risposta As String)

        'tutto in minuscolo!!!!!!!!!!!!!!!!!!!!!
        '<RISULTATO>
        '   <ERRORE></ERRORE>
        '   <RISPOSTA>risposta</RISPOSTA>
        '</RISULTATO>

        'questo xml viene creato da WS_Importa_Magazzino_2010.GestioneRisposta.vb: Genera_Stringa_RispostaGenerale

        Dim objSecurity As New AgronicaCoreDataProvider.Sicurezza

        If Flag_StringaCriptata = True Then
            Str_XML_Risposta = objSecurity.Stringa_Decodifica_WebService(Str_XML_Risposta)
        End If

        Dim XmlDoc As New XmlDocument
        Dim XML_Risultato As XmlElement
        Dim XML_Errore As XmlElement
        Dim XML_Risposta As XmlElement

        XmlDoc.LoadXml(Str_XML_Risposta)

        XML_Risultato = XmlDoc.SelectSingleNode("risultato")
        XML_Errore = XML_Risultato.SelectSingleNode("errore")
        Msg_Errore = XML_Errore.InnerText
        XML_Risposta = XML_Risultato.SelectSingleNode("risposta")
        Msg_Risposta = XML_Risposta.InnerText


    End Sub

    '##########################################################
    'decodifica l'xml di risposta (generato da Genera_Stringa_RispostaImportDDT)
    'del webservice WS_Importa_Magazzino_2010
    'non è da decriptare (l'ha già fatto al funzione generale)
    Public Sub Decodifica_StrXMLRisposta_ImportDDT(ByVal Str_XML_Risposta As String, _
                                                     ByRef Risp_log_import As StringBuilder, _
                                                     ByRef Risp_log_errori As StringBuilder, _
                                                     ByRef Risp_log_banchedati As StringBuilder)

        'tutto in minuscolo!!!!!!!!!!!!!!!!!!!!!
        '<root>
        '   <log_import>testo</log_import>
        '   <log_errori>testo</log_errori>
        '   <log_banchedati>testo</log_banchedati>
        '</root>

        'questo xml viene creato da WS_Importa_Magazzino_2010.GestioneRisposta.vb: Genera_Stringa_RispostaImportDDT

        Dim objSecurity As New AgronicaCoreDataProvider.Sicurezza

        'If Flag_StringaCriptata = True Then
        '    Str_XML_Risposta = objSecurity.Stringa_Decodifica_WebService(Str_XML_Risposta)
        'End If

        Dim XmlDoc As New XmlDocument
        Dim XML_Risultato As XmlElement
        Dim XML_log_import As XmlElement
        Dim XML_log_errori As XmlElement
        Dim XML_log_banchedati As XmlElement

        XmlDoc.LoadXml(Str_XML_Risposta)

        XML_Risultato = XmlDoc.SelectSingleNode("root")

        Risp_log_import.Length = 0
        Risp_log_errori.Length = 0
        Risp_log_banchedati.Length = 0

        XML_log_import = XML_Risultato.SelectSingleNode("log_import")
        Risp_log_import.Append(XML_log_import.InnerText)

        XML_log_errori = XML_Risultato.SelectSingleNode("log_errori")
        Risp_log_errori.Append(XML_log_errori.InnerText)

        XML_log_banchedati = XML_Risultato.SelectSingleNode("log_banchedati")
        Risp_log_banchedati.Append(XML_log_banchedati.InnerText)


    End Sub

    '##########################################################
    'decodifica l'xml di risposta (generato da Genera_Stringa_RispostaImportDocAcquisto) 
    'del webservice WS_Importa_Magazzino_2010
    'non è da decriptare (l'ha già fatto al funzione generale)
    Public Sub Decodifica_StrXMLRisposta_ImportDocAcquisto(ByVal Str_XML_Risposta As String, _
                                                            ByRef Risp_log_import As StringBuilder, _
                                                            ByRef Risp_log_errori As StringBuilder, _
                                                            ByRef Risp_log_banchedati As StringBuilder, _
                                                            ByRef Piva As String, _
                                                            ByRef str_chiave_doc As String, _
                                                            ByRef Flag_NonVerraImportato As Boolean)


        'tutto in minuscolo!!!!!!!!!!!!!!!!!!!!!
        '<root>
        '   <log_import>testo</log_import>
        '   <log_errori>testo</log_errori>
        '   <log_banchedati>testo</log_banchedati>
        '   <piva>testo</piva>
        '   <str_chiave_doc>testo</str_chiave_doc>
        '   <flag_no_import>true/false</flag_no_import>---> viene utilizzato solo per l'analisi dell'xml, non in fase di importazione
        '</root>

        'questo xml viene creato da WS_Importa_Magazzino_2010.GestioneRisposta.vb: Genera_Stringa_RispostaImportDDT

        Dim objSecurity As New AgronicaCoreDataProvider.Sicurezza

        'If Flag_StringaCriptata = True Then
        '    Str_XML_Risposta = objSecurity.Stringa_Decodifica_WebService(Str_XML_Risposta)
        'End If

        Dim XmlDoc As New XmlDocument
        Dim XML_Risultato As XmlElement
        Dim XML_log_import As XmlElement
        Dim XML_log_errori As XmlElement
        Dim XML_log_banchedati As XmlElement
        Dim XML_str_chiave_doc As XmlElement
        Dim XML_piva As XmlElement
        Dim XML_Flag_NonVerraImportato As XmlElement

        XmlDoc.LoadXml(Str_XML_Risposta)

        XML_Risultato = XmlDoc.SelectSingleNode("root")

        Risp_log_import.Length = 0
        Risp_log_errori.Length = 0
        Risp_log_banchedati.Length = 0

        XML_log_import = XML_Risultato.SelectSingleNode("log_import")
        Risp_log_import.Append(XML_log_import.InnerText)

        XML_log_errori = XML_Risultato.SelectSingleNode("log_errori")
        Risp_log_errori.Append(XML_log_errori.InnerText)

        XML_log_banchedati = XML_Risultato.SelectSingleNode("log_banchedati")
        Risp_log_banchedati.Append(XML_log_banchedati.InnerText)

        XML_piva = XML_Risultato.SelectSingleNode("piva")
        Piva = XML_piva.InnerText

        XML_str_chiave_doc = XML_Risultato.SelectSingleNode("str_chiave_doc")
        str_chiave_doc = XML_str_chiave_doc.InnerText

        XML_Flag_NonVerraImportato = XML_Risultato.SelectSingleNode("flag_no_import")
        Flag_NonVerraImportato = XML_Flag_NonVerraImportato.InnerText

    End Sub


    '##########################################################
    'decodifica l'xml di risposta (generato da Genera_Stringa_RispostaRecuperaChiaviDocumentiAcquisto) 
    'del webservice WS_Importa_Magazzino_2010
    'non è da decriptare (l'ha già fatto la funzione generale)
    Public Sub Decodifica_StrXMLRisposta_RecuperaChiaviDocumentiAcquisto(ByVal Str_XML_Risposta As String, _
                                                            ByRef Risp_log_import As StringBuilder, _
                                                            ByRef Risp_log_errori As StringBuilder)


        'tutto in minuscolo!!!!!!!!!!!!!!!!!!!!!
        '<root>
        '   <log_import>testo</log_import>
        '   <log_errori>testo</log_errori>
        '</root>

        'questo xml viene creato da WS_Importa_Magazzino_2010.GestioneRisposta.vb: Genera_Stringa_RispostaImportDDT

        Dim objSecurity As New AgronicaCoreDataProvider.Sicurezza

        'If Flag_StringaCriptata = True Then
        '    Str_XML_Risposta = objSecurity.Stringa_Decodifica_WebService(Str_XML_Risposta)
        'End If

        Dim XmlDoc As New XmlDocument
        Dim XML_Risultato As XmlElement
        Dim XML_log_import As XmlElement
        Dim XML_log_errori As XmlElement

        XmlDoc.LoadXml(Str_XML_Risposta)

        XML_Risultato = XmlDoc.SelectSingleNode("root")

        Risp_log_import.Length = 0
        Risp_log_errori.Length = 0

        XML_log_import = XML_Risultato.SelectSingleNode("log_import")
        Risp_log_import.Append(XML_log_import.InnerText)

        XML_log_errori = XML_Risultato.SelectSingleNode("log_errori")
        Risp_log_errori.Append(XML_log_errori.InnerText)

    End Sub


End Class
