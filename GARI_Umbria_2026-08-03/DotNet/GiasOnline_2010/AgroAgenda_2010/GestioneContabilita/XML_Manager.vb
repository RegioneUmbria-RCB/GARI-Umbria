Imports System.Xml
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi


Module XML_Manager

    '##########################################################################################
    Public Sub XML_Indirizzo(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                             ByRef StringaXML As String,
                             ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                             ByRef Tipo_Indirizzo As Integer,
                             ByRef Cod_Indirizzo As Integer,
                             ByRef Ind_Des As String,
                             ByRef Frz_Des As String,
                             ByRef CAP As String,
                             ByRef Com_Des As String,
                             ByRef Pro_Cod As String,
                             ByRef Stato As String,
                             ByRef Note As String,
                             ByRef Pro_Cod_Istat As String,
                             ByRef Com_Cod_Istat As String,
                             ByRef Validita_Inizio As Date,
                             ByRef Validita_Fine As Date,
                             ByRef BaseCode As Integer,
                             ByRef TopCode As Integer)

        Dim xmlDoc As New XmlDocument
        Dim xmlIndirizzo As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlIndirizzo = xmlDoc.CreateElement("Indirizzo")

                'Imposto gli attributi
                xmlIndirizzo.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlIndirizzo.SetAttribute("tipo_indirizzo", CStr(Tipo_Indirizzo))
                xmlIndirizzo.SetAttribute("cod_indirizzo", CStr(Cod_Indirizzo))
                xmlIndirizzo.SetAttribute("ind_des", Ind_Des)
                xmlIndirizzo.SetAttribute("frz_des", Frz_Des)
                xmlIndirizzo.SetAttribute("cap", CAP)
                xmlIndirizzo.SetAttribute("com_des", Com_Des)
                xmlIndirizzo.SetAttribute("pro_cod", Pro_Cod)
                xmlIndirizzo.SetAttribute("stato", Stato)
                xmlIndirizzo.SetAttribute("note", Note)
                xmlIndirizzo.SetAttribute("pro_cod_istat", Pro_Cod_Istat)
                xmlIndirizzo.SetAttribute("com_cod_istat", Com_Cod_Istat)
                xmlIndirizzo.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlIndirizzo.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlIndirizzo.SetAttribute("basecode", CStr(BaseCode))
                xmlIndirizzo.SetAttribute("topcode", CStr(TopCode))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlIndirizzo)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlIndirizzo = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlIndirizzo = xmlDoc.SelectSingleNode("//Indirizzo")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlIndirizzo.GetAttribute("TipoOperazioneDB"))
                Tipo_Indirizzo = CInt(xmlIndirizzo.GetAttribute("tipo_indirizzo"))
                Cod_Indirizzo = CInt(xmlIndirizzo.GetAttribute("cod_indirizzo"))
                Ind_Des = CStr(xmlIndirizzo.GetAttribute("ind_des"))
                Frz_Des = CStr(xmlIndirizzo.GetAttribute("frz_des"))
                CAP = CStr(xmlIndirizzo.GetAttribute("cap"))
                Com_Des = CStr(xmlIndirizzo.GetAttribute("com_des"))
                Pro_Cod = CStr(xmlIndirizzo.GetAttribute("pro_cod"))
                Stato = CStr(xmlIndirizzo.GetAttribute("stato"))
                Note = CStr(xmlIndirizzo.GetAttribute("note"))
                Pro_Cod_Istat = CStr(xmlIndirizzo.GetAttribute("pro_cod_istat"))
                Com_Cod_Istat = CStr(xmlIndirizzo.GetAttribute("com_cod_istat"))
                Validita_Inizio = CDate(xmlIndirizzo.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlIndirizzo.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlIndirizzo = Nothing
                xmlDoc = Nothing

        End Select

    End Sub




    'da utilizzare la nuova versione sotto
    '##########################################################################################
    'Public Sub XML_Codice(ByVal OperazioneRichiesta As enum_CodificaDecodifica, _
    '                      ByRef StringaXML As String, _
    '                      ByRef TipoOperazioneDB As enum_TipoOperazioneDB, _
    '                      ByRef Id_Cod As Integer, _
    '                      ByRef Val_Cod As String, _
    '                      ByRef Validita_Inizio As Date, _
    '                      ByRef Validita_Fine As Date, _
    '                      ByRef BaseCode As Integer, _
    '                      ByRef TopCode As Integer, _
    '                      Optional ByVal ElementoInteressato As String = "")

    '    'Nota: 
    '    'Per ElementoInteressato si intende una stringa che vale "Appezzamento" oppure "Impianto"
    '    'a seconda che il codice appartenga all'uno o all'altro elemento

    '    Dim XmlDoc As New System.Xml.XmlDocument
    '    Dim XmlCodice As System.Xml.XmlElement

    '    'Verifico quale operazione deve essere effettuata
    '    Select Case OperazioneRichiesta

    '        Case enum_CodificaDecodifica.Codifica

    '            '----- Genero la stringa XML a partire dai valori dei parametri

    '            'Creo il nodo 
    '            XmlCodice = XmlDoc.CreateElement("Codice" & ElementoInteressato)

    '            'Imposto gli attributi
    '            XmlCodice.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
    '            XmlCodice.SetAttribute("id_cod", CStr(Id_Cod))
    '            XmlCodice.SetAttribute("val_cod", Val_Cod)
    '            XmlCodice.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
    '            XmlCodice.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
    '            XmlCodice.SetAttribute("basecode", CStr(BaseCode))
    '            XmlCodice.SetAttribute("topcode", CStr(TopCode))

    '            'Imposto XmlIndirizzo come figlio del documento principale
    '            XmlDoc.AppendChild(XmlCodice)

    '            'Restituisco in uscita la stringa creata
    '            StringaXML = XmlDoc.InnerXml

    '            'Distruggo gli oggetti
    '            XmlCodice = Nothing
    '            XmlDoc = Nothing



    '        Case enum_CodificaDecodifica.Decodifica

    '            '----- Recupero i valori dei parametri a partire dalla stringa XML

    '            'Carico la stringa XML nel documento
    '            XmlDoc.LoadXml(StringaXML)

    '            'Prelevo il nodo
    '            XmlCodice = XmlDoc.SelectSingleNode("//Codice")

    '            'Prelevo gli attributi
    '            TipoOperazioneDB = CInt(XmlCodice.GetAttribute("TipoOperazioneDB"))
    '            Id_Cod = CInt(XmlCodice.GetAttribute("id_cod"))
    '            Val_Cod = CStr(XmlCodice.GetAttribute("val_cod"))
    '            Validita_Inizio = CDate(XmlCodice.GetAttribute("validita_inizio"))
    '            Validita_Fine = CDate(XmlCodice.GetAttribute("validita_fine"))
    '            BaseCode = 0
    '            TopCode = 0

    '            'Distruggo gli oggetti
    '            XmlCodice = Nothing
    '            XmlDoc = Nothing

    '    End Select

    'End Sub


    '##########################################################################################
    Public Sub XML_Codice(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                          ByRef StringaXML As String,
                          ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                          ByRef Id_Cod As Integer,
                          ByRef Val_Cod As String,
                          ByRef Validita_Inizio As Date,
                          ByRef Validita_Fine As Date,
                          ByRef BaseCode As Integer,
                          ByRef TopCode As Integer,
                          Optional ByVal ElementoInteressato As String = "")

        'Nota: 
        'Per ElementoInteressato si intende una stringa che vale "Appezzamento" oppure "Impianto"
        'a seconda che il codice appartenga all'uno o all'altro elemento

        Dim xmlDoc As New XmlDocument
        Dim xmlCodice As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlCodice = xmlDoc.CreateElement("Codice" & ElementoInteressato)

                'Imposto gli attributi
                xmlCodice.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlCodice.SetAttribute("id_cod", CStr(Id_Cod))
                xmlCodice.SetAttribute("val_cod", Val_Cod)
                xmlCodice.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlCodice.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlCodice.SetAttribute("basecode", CStr(BaseCode))
                xmlCodice.SetAttribute("topcode", CStr(TopCode))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlCodice)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlCodice = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlCodice = xmlDoc.SelectSingleNode("//Codice")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlCodice.GetAttribute("TipoOperazioneDB"))
                Id_Cod = CInt(xmlCodice.GetAttribute("id_cod"))
                Val_Cod = CStr(xmlCodice.GetAttribute("val_cod"))
                Validita_Inizio = CDate(xmlCodice.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlCodice.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlCodice = Nothing
                xmlDoc = Nothing

        End Select

    End Sub


    '##########################################################################################
    Public Sub XML_ParticellaxProgetto(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                                       ByRef StringaXML As String,
                                       ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                                       ByRef Prov As String,
                                       ByRef Com As String,
                                       ByRef Sezione As String,
                                       ByRef Foglio As Integer,
                                       ByRef Numero As Integer,
                                       ByRef Subalterno As String,
                                       ByRef Id_Cod As Integer,
                                       ByRef Val_Cod As String,
                                       ByRef Validita_Inizio As Date,
                                       ByRef Validita_Fine As Date,
                                       ByRef BaseCode As Integer,
                                       ByRef TopCode As Integer)

        Dim xmlDoc As New XmlDocument
        Dim xmlParticella As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlParticella = xmlDoc.CreateElement("ParticellaxProgetto")

                'Imposto gli attributi
                xmlParticella.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlParticella.SetAttribute("prov", CStr(Prov))
                xmlParticella.SetAttribute("com", CStr(Com))
                xmlParticella.SetAttribute("sezione", CStr(Sezione))
                xmlParticella.SetAttribute("foglio", CStr(Foglio))
                xmlParticella.SetAttribute("numero", CStr(Numero))
                xmlParticella.SetAttribute("subalterno", CStr(Subalterno))
                xmlParticella.SetAttribute("id_cod", CStr(Id_Cod))
                xmlParticella.SetAttribute("val_cod", CStr(Val_Cod))
                xmlParticella.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlParticella.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlParticella.SetAttribute("basecode", CStr(BaseCode))
                xmlParticella.SetAttribute("topcode", CStr(TopCode))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlParticella)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlParticella = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlParticella = xmlDoc.SelectSingleNode("//ParticellaxProgetto")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlParticella.GetAttribute("TipoOperazioneDB"))

                Prov = CStr(xmlParticella.GetAttribute("prov"))
                Com = CStr(xmlParticella.GetAttribute("com"))
                Sezione = CStr(xmlParticella.GetAttribute("sezione"))
                Foglio = CInt(xmlParticella.GetAttribute("foglio"))
                Numero = CInt(xmlParticella.GetAttribute("numero"))
                Subalterno = CStr(xmlParticella.GetAttribute("subalterno"))
                Validita_Inizio = CDate(xmlParticella.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlParticella.GetAttribute("validita_fine"))
                Id_Cod = CInt(xmlParticella.GetAttribute("id_cod"))
                Val_Cod = CStr(xmlParticella.GetAttribute("val_cod"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlParticella = Nothing
                xmlDoc = Nothing

        End Select

    End Sub



    '##########################################################################################
    Public Sub XML_Impresa(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                           ByRef StringaXML As String,
                           ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                           ByRef Piva As String,
                           ByRef Rag_Soc As String,
                           ByRef Delega As String,
                           ByRef AT_Prevalente As String,
                           ByRef Forma_Giuridica As String,
                           ByRef Forma_Conduzione As String,
                           ByRef Sup_Totale As Decimal,
                           ByRef PivaPadre As String,
                           ByRef TipoImpresaGerarchia As Integer,
                           ByRef Validita_Inizio As Date,
                           ByRef Validita_Fine As Date,
                           ByRef BaseCode As Integer,
                           ByRef TopCode As Integer)

        Dim xmlDoc As New XmlDocument
        Dim xmlImpresa As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlImpresa = xmlDoc.CreateElement("Impresa")

                'Imposto gli attributi
                xmlImpresa.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlImpresa.SetAttribute("piva", Piva)
                xmlImpresa.SetAttribute("rag_soc", Rag_Soc)
                xmlImpresa.SetAttribute("delega", Delega)
                xmlImpresa.SetAttribute("at_prevalente", AT_Prevalente)
                xmlImpresa.SetAttribute("forma_giuridica", Forma_Giuridica)
                xmlImpresa.SetAttribute("forma_conduzione", Forma_Conduzione)
                xmlImpresa.SetAttribute("sup_totale", CStr(Sup_Totale))

                xmlImpresa.SetAttribute("padre", CStr(PivaPadre))
                xmlImpresa.SetAttribute("tipoimpresagerarchia", CStr(TipoImpresaGerarchia))

                xmlImpresa.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlImpresa.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlImpresa.SetAttribute("basecode", CStr(BaseCode))
                xmlImpresa.SetAttribute("topcode", CStr(TopCode))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlImpresa)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlImpresa = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlImpresa = xmlDoc.SelectSingleNode("//Impresa")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlImpresa.GetAttribute("TipoOperazioneDB"))
                Piva = CStr(xmlImpresa.GetAttribute("piva"))
                Rag_Soc = CStr(xmlImpresa.GetAttribute("rag_soc"))
                Delega = CStr(xmlImpresa.GetAttribute("delega"))
                AT_Prevalente = CStr(xmlImpresa.GetAttribute("at_prevalente"))
                Forma_Giuridica = CStr(xmlImpresa.GetAttribute("forma_giuridica"))
                Forma_Conduzione = CStr(xmlImpresa.GetAttribute("forma_conduzione"))
                Sup_Totale = CDbl(xmlImpresa.GetAttribute("sup_totale"))
                Validita_Inizio = CDate(xmlImpresa.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlImpresa.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlImpresa = Nothing
                xmlDoc = Nothing

        End Select

    End Sub


    '##########################################################################################
    Public Sub XML_Rubrica(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                           ByRef StringaXML As String,
                           ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                           ByRef Cod_Rubrica As Long,
                           ByRef Numero As String,
                           ByRef Descrizione As String,
                           ByRef Validita_Inizio As Date,
                           ByRef Validita_Fine As Date,
                           ByRef BaseCode As Integer,
                           ByRef TopCode As Integer)

        Dim xmlDoc As New XmlDocument
        Dim xmlRubrica As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlRubrica = xmlDoc.CreateElement("Rubrica")

                'Imposto gli attributi
                xmlRubrica.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlRubrica.SetAttribute("cod_rubrica", CStr(Cod_Rubrica))
                xmlRubrica.SetAttribute("numero", Numero)
                xmlRubrica.SetAttribute("descr", Descrizione)
                xmlRubrica.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlRubrica.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlRubrica.SetAttribute("basecode", CStr(BaseCode))
                xmlRubrica.SetAttribute("topcode", CStr(TopCode))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlRubrica)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlRubrica = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlRubrica = xmlDoc.SelectSingleNode("//Rubrica")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlRubrica.GetAttribute("TipoOperazioneDB"))
                Cod_Rubrica = CStr(xmlRubrica.GetAttribute("cod_rubrica"))
                Numero = CStr(xmlRubrica.GetAttribute("numero"))
                Descrizione = CStr(xmlRubrica.GetAttribute("descr"))
                Validita_Inizio = CDate(xmlRubrica.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlRubrica.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlRubrica = Nothing
                xmlDoc = Nothing

        End Select

    End Sub


    '##########################################################################################
    Public Sub XML_CentroAziendale(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                                   ByRef StringaXML As String,
                                   ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                                   ByRef Piva As String,
                                   ByRef Sa_Cod As Integer,
                                   ByRef Sa_Nome As String,
                                   ByRef X As Decimal,
                                   ByRef Y As Decimal,
                                   ByRef ZSLM As Decimal,
                                   ByRef Longitudine As Decimal,
                                   ByRef Latitudine As Decimal,
                                   ByRef Area As Decimal,
                                   ByRef CA_Sipi As String,
                                   ByRef AT_Prevalente As String,
                                   ByRef Forma_Possesso As String,
                                   ByRef Cod_TipoCentro As Integer,
                                   ByRef Sup_Totale As Decimal,
                                   ByRef Sup_Bosco As Decimal,
                                   ByRef Sup_Tare As Decimal,
                                   ByRef Sup_SAU As Decimal,
                                   ByRef Sup_Prati As Decimal,
                                   ByRef Validita_Inizio As Date,
                                   ByRef Validita_Fine As Date,
                                   ByRef BaseCode As Integer,
                                   ByRef TopCode As Integer,
                                   ByRef TitoloPossesso As Integer,
                                   ByRef Sup_SAU_Convenzionale As Decimal,
                                   ByRef Sup_SAU_Conversione As Decimal,
                                   ByRef Sup_SAU_Biologico As Decimal)

        Dim xmlDoc As New XmlDocument
        Dim xmlCentro As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlCentro = xmlDoc.CreateElement("CentroAziendale")

                'Imposto gli attributi
                xmlCentro.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlCentro.SetAttribute("piva", Piva)
                xmlCentro.SetAttribute("sa_cod", CStr(Sa_Cod))
                xmlCentro.SetAttribute("sa_nome", Sa_Nome)

                xmlCentro.SetAttribute("x", CStr(X))
                xmlCentro.SetAttribute("y", CStr(Y))
                xmlCentro.SetAttribute("zslm", CStr(ZSLM))
                xmlCentro.SetAttribute("long", CStr(Longitudine))
                xmlCentro.SetAttribute("lat", CStr(Latitudine))
                xmlCentro.SetAttribute("area", CStr(Area))
                xmlCentro.SetAttribute("ca_sipi", CA_Sipi)
                xmlCentro.SetAttribute("at_prevalente", AT_Prevalente)
                xmlCentro.SetAttribute("forma_possesso", Forma_Possesso)

                xmlCentro.SetAttribute("tipo", CStr(Cod_TipoCentro))

                xmlCentro.SetAttribute("sup_totale", CStr(Sup_Totale))
                xmlCentro.SetAttribute("sup_bosco", CStr(Sup_Bosco))
                xmlCentro.SetAttribute("sup_tare", CStr(Sup_Tare))
                xmlCentro.SetAttribute("sup_sau", CStr(Sup_SAU))
                xmlCentro.SetAttribute("sup_prati", CStr(Sup_Prati))

                xmlCentro.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlCentro.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlCentro.SetAttribute("basecode", CStr(BaseCode))
                xmlCentro.SetAttribute("topcode", CStr(TopCode))
                xmlCentro.SetAttribute("titolopossesso", TitoloPossesso.ToString)
                xmlCentro.SetAttribute("sup_sau_convenzionale", Sup_SAU_Convenzionale.ToString)
                xmlCentro.SetAttribute("sup_sau_conversione", Sup_SAU_Conversione.ToString)
                xmlCentro.SetAttribute("sup_sau_biologico", Sup_SAU_Biologico.ToString)


                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlCentro)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlCentro = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlCentro = xmlDoc.SelectSingleNode("//CentroAziendale")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlCentro.GetAttribute("TipoOperazioneDB"))
                Piva = CStr(xmlCentro.GetAttribute("piva"))
                Sa_Cod = CInt(xmlCentro.GetAttribute("sa_cod"))
                Sa_Nome = CStr(xmlCentro.GetAttribute("sa_nome"))

                X = CDbl(xmlCentro.GetAttribute("x"))
                Y = CDbl(xmlCentro.GetAttribute("y"))
                ZSLM = CDbl(xmlCentro.GetAttribute("zslm"))
                Longitudine = CDbl(xmlCentro.GetAttribute("long"))
                Latitudine = CDbl(xmlCentro.GetAttribute("lat"))
                Area = CDbl(xmlCentro.GetAttribute("area"))
                CA_Sipi = CStr(xmlCentro.GetAttribute("ca_sipi"))
                AT_Prevalente = CStr(xmlCentro.GetAttribute("at_prevalente"))
                Forma_Possesso = CStr(xmlCentro.GetAttribute("forma_possesso"))

                Sup_Totale = CDbl(xmlCentro.GetAttribute("sup_totale"))
                Sup_Bosco = CDbl(xmlCentro.GetAttribute("sup_bosco"))
                Sup_Tare = CDbl(xmlCentro.GetAttribute("sup_tare"))
                Sup_SAU = CDbl(xmlCentro.GetAttribute("sup_sau"))
                Sup_Prati = CDbl(xmlCentro.GetAttribute("sup_prati"))

                Validita_Inizio = CDate(xmlCentro.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlCentro.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0
                TitoloPossesso = CInt(xmlCentro.GetAttribute("titolopossesso"))
                Sup_SAU_Convenzionale = CDbl(xmlCentro.GetAttribute("sup_sau_convenzionale"))
                Sup_SAU_Conversione = CDbl(xmlCentro.GetAttribute("sup_sau_conversione"))
                Sup_SAU_Biologico = CDbl(xmlCentro.GetAttribute("sup_sau_biologico"))

                'Distruggo gli oggetti
                xmlCentro = Nothing
                xmlDoc = Nothing

        End Select

    End Sub


    '##########################################################################################
    Public Sub XML_Fabbricato(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                              ByRef StringaXML As String,
                              ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                              ByRef Piva As String,
                              ByRef Sa_Cod As Integer,
                              ByRef Fabbricato_Cod As Integer,
                              ByRef Fabbricato_Des As String,
                              ByRef Indirizzo_Cod As Integer,
                              ByRef Tipo_Fabbricato_Cod As Integer,
                              ByRef Prov As String,
                              ByRef Com As String,
                              ByRef Sezione As String,
                              ByRef Foglio As Integer,
                              ByRef Numero As Integer,
                              ByRef Subalterno As String,
                              ByRef MC_Convenzionale As Decimal,
                              ByRef MC_Conversione As Decimal,
                              ByRef MC_Biologico As Decimal,
                              ByRef Regolamento_Cod As Integer,
                              ByRef TitoloPossesso As Integer,
                              ByRef Idoneo_Costruzione As Integer,
                              ByRef Idoneo_SeparazAmbienti As Integer,
                              ByRef Idoneo_SeparazProdotti As Integer,
                              ByRef Idoneo_CondIgieniche As Integer,
                              ByRef Idoneo_AutorizSanitaria As Integer,
                              ByRef Idoneo_HACCP As Integer,
                              ByRef Idoneo_Planimetria As Integer,
                              ByRef Idoneo_LayOut As Integer,
                              ByRef Idoneo_DiagrammiFlusso As Integer,
                              ByRef Idoneo_CDX_M004 As Integer,
                              ByRef Idoneo_SupMinCoperte As Integer,
                              ByRef Idoneo_SupMinScoperte As Integer,
                              ByRef Conversione_Inizio As Date,
                              ByRef Conversione_Fine As Date,
                              ByRef UserName_Creazione As String,
                              ByRef Validita_Inizio As Date,
                              ByRef Validita_Fine As Date,
                              ByRef BaseCode As Integer,
                              ByRef TopCode As Integer,
                              ByRef ProprietarioCapi As String,
                              ByRef flagMagazzinoFarmaci As Integer,
                              ByRef CodiceBDN As String)

        Dim xmlDoc As New XmlDocument
        Dim xmlFabbricato As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlFabbricato = xmlDoc.CreateElement("Fabbricato")

                'Imposto gli attributi
                xmlFabbricato.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlFabbricato.SetAttribute("piva", Piva)
                xmlFabbricato.SetAttribute("sa_cod", CStr(Sa_Cod))
                xmlFabbricato.SetAttribute("fabbricato_cod", CStr(Fabbricato_Cod))
                xmlFabbricato.SetAttribute("fabbricato_des", CStr(Fabbricato_Des))

                xmlFabbricato.SetAttribute("indirizzo_cod", CStr(Indirizzo_Cod))
                xmlFabbricato.SetAttribute("tipo_fabbricato_cod", CStr(Tipo_Fabbricato_Cod))
                xmlFabbricato.SetAttribute("prov", CStr(Prov))
                xmlFabbricato.SetAttribute("com", CStr(Com))
                xmlFabbricato.SetAttribute("sezione", CStr(Sezione))
                xmlFabbricato.SetAttribute("foglio", CStr(Foglio))
                xmlFabbricato.SetAttribute("numero", CStr(Numero))
                xmlFabbricato.SetAttribute("subalterno", Subalterno)

                xmlFabbricato.SetAttribute("mc_convenzionale", CStr(MC_Convenzionale))
                xmlFabbricato.SetAttribute("mc_conversione", CStr(MC_Conversione))
                xmlFabbricato.SetAttribute("mc_biologico", CStr(MC_Biologico))
                xmlFabbricato.SetAttribute("titolopossesso", CStr(TitoloPossesso))
                xmlFabbricato.SetAttribute("regolamento_cod", CStr(Regolamento_Cod))
                xmlFabbricato.SetAttribute("idoneo_costruzione", CStr(Idoneo_Costruzione))
                xmlFabbricato.SetAttribute("idoneo_separazambienti", CStr(Idoneo_SeparazAmbienti))
                xmlFabbricato.SetAttribute("idoneo_separazprodotti", CStr(Idoneo_SeparazProdotti))
                xmlFabbricato.SetAttribute("idoneo_condigieniche", CStr(Idoneo_CondIgieniche))
                xmlFabbricato.SetAttribute("idoneo_autorizsanitaria", CStr(Idoneo_AutorizSanitaria))
                xmlFabbricato.SetAttribute("idoneo_haccp", CStr(Idoneo_HACCP))
                xmlFabbricato.SetAttribute("idoneo_planimetria", CStr(Idoneo_Planimetria))
                xmlFabbricato.SetAttribute("idoneo_layout", CStr(Idoneo_LayOut))
                xmlFabbricato.SetAttribute("idoneo_diagrammiflusso", CStr(Idoneo_DiagrammiFlusso))
                xmlFabbricato.SetAttribute("idoneo_cdx_m004", CStr(Idoneo_CDX_M004))
                xmlFabbricato.SetAttribute(LCase("Idoneo_SupMinCoperte"), CStr(Idoneo_SupMinCoperte))
                xmlFabbricato.SetAttribute(LCase("Idoneo_SupMinScoperte"), CStr(Idoneo_SupMinScoperte))
                xmlFabbricato.SetAttribute("conversione_inizio", Format(Conversione_Inizio, "dd/MM/yyyy"))
                xmlFabbricato.SetAttribute("conversione_fine", Format(Conversione_Fine, "dd/MM/yyyy"))
                xmlFabbricato.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlFabbricato.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlFabbricato.SetAttribute("basecode", CStr(BaseCode))
                xmlFabbricato.SetAttribute("topcode", CStr(TopCode))

                xmlFabbricato.SetAttribute("proprietario_capi", ProprietarioCapi)
                xmlFabbricato.SetAttribute("chkmagazzinofarmaci", flagMagazzinoFarmaci)
                xmlFabbricato.SetAttribute("codice_bdn", CodiceBDN)

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlFabbricato)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlFabbricato = Nothing
                xmlDoc = Nothing

            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlFabbricato = xmlDoc.SelectSingleNode("//Fabbricato")

                '...................





                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlFabbricato)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlFabbricato = Nothing
                xmlDoc = Nothing

        End Select

    End Sub

    '##########################################################################################
    Public Sub XML_Stalla(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                          ByRef StringaXML As String,
                          ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                          ByRef Piva As String,
                          ByRef Sa_Cod As Integer,
                          ByRef Sta_Num As Integer,
                          ByRef Sta_Des As String,
                          ByRef Ausl_Cod As Integer,
                          ByRef DAT_COSTR As Date,
                          ByRef DAT_CHIU As Date,
                          ByRef Cod_Fabb As String,
                          ByRef Gen_Cod As Integer,
                          ByRef Spe_Cod As Integer,
                          ByRef IPro_Cod As Integer,
                          ByRef x As String,
                          ByRef y As String,
                          ByRef BDN_Codice_Azienda As String,
                          ByRef BDN_Allev_IdFiscale As String,
                          ByRef Validita_Inizio As Date,
                          ByRef Validita_Fine As Date,
                          ByRef BaseCode As Integer,
                          ByRef TopCode As Integer)

        Dim xmlDoc As New XmlDocument
        Dim xmlStalla As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlStalla = xmlDoc.CreateElement("Stalla")

                'Imposto gli attributi
                xmlStalla.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlStalla.SetAttribute("piva", CStr(Piva))
                xmlStalla.SetAttribute("sa_cod", CStr(Sa_Cod))
                xmlStalla.SetAttribute("sta_num", CStr(Sta_Num))
                xmlStalla.SetAttribute("sta_des", Sta_Des)
                xmlStalla.SetAttribute("ausl_cod", CStr(Ausl_Cod))
                xmlStalla.SetAttribute("dat_costr", CStr(DAT_COSTR))
                xmlStalla.SetAttribute("dat_chiu", CStr(DAT_CHIU))
                xmlStalla.SetAttribute("cod_fabb", CStr(Cod_Fabb))
                xmlStalla.SetAttribute("gen_cod", CStr(Gen_Cod))
                xmlStalla.SetAttribute("spe_cod", CStr(Spe_Cod))
                xmlStalla.SetAttribute("ipro_cod", CStr(IPro_Cod))
                xmlStalla.SetAttribute("x", CStr(x))
                xmlStalla.SetAttribute("y", CStr(y))
                xmlStalla.SetAttribute("BDN_codice_azienda", CStr(BDN_Codice_Azienda))
                xmlStalla.SetAttribute("BDN_allev_idfiscale", CStr(BDN_Allev_IdFiscale))
                xmlStalla.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlStalla.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlStalla.SetAttribute("basecode", CStr(BaseCode))
                xmlStalla.SetAttribute("topcode", CStr(TopCode))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlStalla)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlStalla = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlStalla = xmlDoc.SelectSingleNode("//Stalla")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlStalla.GetAttribute("TipoOperazioneDB"))
                Sta_Num = CInt(xmlStalla.GetAttribute("sta_num"))
                Sta_Des = CStr(xmlStalla.GetAttribute("sta_des"))
                Validita_Inizio = CDate(xmlStalla.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlStalla.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlStalla = Nothing
                xmlDoc = Nothing

        End Select

    End Sub

    '##########################################################################################
    Public Sub XML_Stalla_Caratteristica(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                                         ByRef StringaXML As String,
                                         ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                                         ByRef Att_Cod As Integer,
                                         ByRef Valore As String,
                                         ByRef Validita_Inizio As Date,
                                         ByRef Validita_Fine As Date,
                                         ByRef BaseCode As Integer,
                                         ByRef TopCode As Integer)

        Dim xmlDoc As New XmlDocument
        Dim xmlStallaCaratteristica As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlStallaCaratteristica = xmlDoc.CreateElement("Stalla_Caratteristica")

                'Imposto gli attributi
                xmlStallaCaratteristica.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlStallaCaratteristica.SetAttribute("att_cod", CStr(Att_Cod))
                xmlStallaCaratteristica.SetAttribute("valore", CStr(Valore))
                xmlStallaCaratteristica.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlStallaCaratteristica.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlStallaCaratteristica.SetAttribute("basecode", CStr(BaseCode))
                xmlStallaCaratteristica.SetAttribute("topcode", CStr(TopCode))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlStallaCaratteristica)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlStallaCaratteristica = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlStallaCaratteristica = xmlDoc.SelectSingleNode("//Stalla_Caratteristica")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlStallaCaratteristica.GetAttribute("TipoOperazioneDB"))
                Att_Cod = CInt(xmlStallaCaratteristica.GetAttribute("att_cod"))
                Valore = CStr(xmlStallaCaratteristica.GetAttribute("valore"))
                Validita_Inizio = CDate(xmlStallaCaratteristica.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlStallaCaratteristica.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlStallaCaratteristica = Nothing
                xmlDoc = Nothing

        End Select

    End Sub

    '##########################################################################################
    Public Sub XML_Stalla_Configurazione_BDN(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                                             ByRef StringaXML As String,
                                             ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                                             ByRef id As Integer,
                                             ByRef CF_Detentore As String,
                                             ByRef CF_Proprietario As String,
                                             ByRef RagSoc_Detentore As String,
                                             ByRef RagSoc_Proprietario As String,
                                             ByRef Validita_Inizio As Date,
                                             ByRef Validita_Fine As Date,
                                             ByRef BaseCode As Integer,
                                             ByRef TopCode As Integer)

        Dim xmlDoc As New XmlDocument
        Dim xmlStallaCaratteristica As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlStallaCaratteristica = xmlDoc.CreateElement("Stalla_Configurazione")

                'Imposto gli attributi
                xmlStallaCaratteristica.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlStallaCaratteristica.SetAttribute("id", CStr(id))
                xmlStallaCaratteristica.SetAttribute("cf_detentore", CStr(CF_Detentore))
                xmlStallaCaratteristica.SetAttribute("cf_proprietario", CStr(CF_Proprietario))
                xmlStallaCaratteristica.SetAttribute("ragsoc_detentore", CStr(RagSoc_Detentore))
                xmlStallaCaratteristica.SetAttribute("ragsoc_proprietario", CStr(RagSoc_Proprietario))
                xmlStallaCaratteristica.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlStallaCaratteristica.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlStallaCaratteristica.SetAttribute("basecode", CStr(BaseCode))
                xmlStallaCaratteristica.SetAttribute("topcode", CStr(TopCode))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlStallaCaratteristica)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlStallaCaratteristica = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlStallaCaratteristica = xmlDoc.SelectSingleNode("//Stalla_Caratteristica")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlStallaCaratteristica.GetAttribute("TipoOperazioneDB"))
                CF_Detentore = CStr(xmlStallaCaratteristica.GetAttribute("cf_detentore"))
                CF_Proprietario = CStr(xmlStallaCaratteristica.GetAttribute("cf_proprietario"))
                RagSoc_Detentore = CStr(xmlStallaCaratteristica.GetAttribute("ragsoc_detentore"))
                RagSoc_Proprietario = CStr(xmlStallaCaratteristica.GetAttribute("ragsoc_proprietario"))
                Validita_Inizio = CDate(xmlStallaCaratteristica.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlStallaCaratteristica.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlStallaCaratteristica = Nothing
                xmlDoc = Nothing

        End Select

    End Sub

    '##########################################################################################
    Public Function XML_Zoo_Animale(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                    Optional ByVal Piva As String = "00000000000",
                                    Optional ByVal Sa_Cod As Integer = 0,
                                    Optional ByVal Cod_Progetto As Integer = 0,
                                    Optional ByVal Matricola As String = "",
                                    Optional ByVal Gen_Cod As Integer = 0,
                                    Optional ByVal Spe_Cod As Integer = 0,
                                    Optional ByVal IPro_Cod As Integer = 0,
                                    Optional ByVal Raz_Cod As Integer = 0,
                                    Optional ByVal Cat_Cod As Integer = 0,
                                    Optional ByVal Spe_Des As String = "",
                                    Optional ByVal Ipro_Des As String = "",
                                    Optional ByVal Raz_Des As String = "",
                                    Optional ByVal Nome As String = "",
                                    Optional ByVal Collare As String = "",
                                    Optional ByVal Nome_Aia As String = "",
                                    Optional ByVal Matricola_Aia As String = "",
                                    Optional ByVal Dat_Nascita As Date = #1/1/1900#,
                                    Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                    Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                    Optional ByVal Stato_Nascita As String = "000",
                                    Optional ByVal Provincia_Nascita As String = "000",
                                    Optional ByVal AUA_Azi_Nascita As String = "",
                                    Optional ByVal AUSL_Azi_Nascita As String = "",
                                    Optional ByVal Sesso As String = "M",
                                    Optional ByVal Mat_Padre As String = "",
                                    Optional ByVal Mat_Madre As String = "",
                                    Optional ByVal CF_Proprietario As String = "",
                                    Optional ByVal CF_Detentore As String = "",
                                    Optional ByVal Presente As Decimal = 0,
                                    Optional ByVal Peso As Decimal = 0,
                                    Optional ByVal Data_Pesa As Date = #1/1/1900#,
                                    Optional ByVal Metodo_Produzione As Integer = 1,
                                    Optional ByVal Regolamento_Cod As Integer = 0,
                                    Optional ByVal Conversione_Inizio As Date = #1/1/1900#,
                                    Optional ByVal Conversione_Fine As Date = #1/1/1900#,
                                    Optional ByVal BaseCode As Integer = 0,
                                    Optional ByVal TopCode As Integer = 200000000
                                    ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Zoo_Animale_Anagrafe")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("piva"), Piva)
        xmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("cod_progetto"), CStr(Cod_Progetto))
        xmlTxt.SetAttribute(LCase("matricola"), CStr(Matricola))
        xmlTxt.SetAttribute(LCase("gen_cod"), CStr(Gen_Cod))
        xmlTxt.SetAttribute(LCase("spe_cod"), CStr(Spe_Cod))
        xmlTxt.SetAttribute(LCase("ipro_cod"), CStr(IPro_Cod))
        xmlTxt.SetAttribute(LCase("raz_cod"), CStr(Raz_Cod))
        xmlTxt.SetAttribute(LCase("cat_cod"), CStr(Cat_Cod))
        xmlTxt.SetAttribute(LCase("spe_des"), CStr(Spe_Des))
        xmlTxt.SetAttribute(LCase("ipro_des"), CStr(Ipro_Des))
        xmlTxt.SetAttribute(LCase("raz_des"), CStr(Raz_Des))
        xmlTxt.SetAttribute(LCase("nome"), CStr(Nome))
        xmlTxt.SetAttribute(LCase("collare"), CStr(Collare))
        xmlTxt.SetAttribute(LCase("nome_aia"), CStr(Nome_Aia))
        xmlTxt.SetAttribute(LCase("matricola_aia"), CStr(Matricola_Aia))
        xmlTxt.SetAttribute(LCase("dat_nascita"), Format(Dat_Nascita, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("stato_nascita"), CStr(Stato_Nascita))
        xmlTxt.SetAttribute(LCase("prov_nascita"), CStr(Provincia_Nascita))
        xmlTxt.SetAttribute(LCase("aua_azi_nascita"), CStr(AUA_Azi_Nascita))
        xmlTxt.SetAttribute(LCase("ausl_azi_nascita"), CStr(AUSL_Azi_Nascita))
        xmlTxt.SetAttribute(LCase("sesso"), CStr(Sesso))
        xmlTxt.SetAttribute(LCase("mat_padre"), CStr(Mat_Padre))
        xmlTxt.SetAttribute(LCase("mat_madre"), CStr(Mat_Madre))
        xmlTxt.SetAttribute(LCase("cf_proprietario"), CStr(CF_Proprietario))
        xmlTxt.SetAttribute(LCase("cf_detentore"), CStr(CF_Detentore))
        xmlTxt.SetAttribute(LCase("presente"), CStr(Presente))
        xmlTxt.SetAttribute(LCase("peso"), CStr(Peso))
        xmlTxt.SetAttribute(LCase("data_pesa"), CStr(Data_Pesa))
        xmlTxt.SetAttribute(LCase("metodo_produzione"), CStr(Metodo_Produzione))
        xmlTxt.SetAttribute(LCase("regolamento_cod"), CStr(Regolamento_Cod))
        xmlTxt.SetAttribute(LCase("conversione_inizio"), CStr(Conversione_Inizio))
        xmlTxt.SetAttribute(LCase("conversione_fine"), CStr(Conversione_Fine))
        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function


    '##########################################################################################
    Public Sub XML_Campo(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                         ByRef StringaXML As String,
                         ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                         ByRef Piva As String,
                         ByRef Sa_Cod As Integer,
                         ByRef Campo_Cod As Long,
                         ByRef Campo_Des As String,
                         ByRef Conversione_Inizio As Date,
                         ByRef Conversione_Fine As Date,
                         ByRef SAU_Totale As Decimal,
                         ByRef SAU_Biologico As Decimal,
                         ByRef SAU_Conversione As Decimal,
                         ByRef SAU_Convenzionale As Decimal,
                         ByRef ConfiniRischio As String,
                         ByRef Validita_Inizio As Date,
                         ByRef Validita_Fine As Date,
                         ByRef BaseCode As Integer,
                         ByRef TopCode As Integer)

        Dim xmlDoc As New XmlDocument
        Dim xmlCampo As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlCampo = xmlDoc.CreateElement("Campo")

                'Imposto gli attributi
                xmlCampo.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlCampo.SetAttribute("piva", CStr(Piva))
                xmlCampo.SetAttribute("sa_cod", CStr(Sa_Cod))
                xmlCampo.SetAttribute("campo_cod", CStr(Campo_Cod))
                xmlCampo.SetAttribute("campo_des", Campo_Des)
                xmlCampo.SetAttribute("conversione_inizio", CStr(Conversione_Inizio))
                xmlCampo.SetAttribute("conversione_fine", CStr(Conversione_Fine))
                xmlCampo.SetAttribute("sau_totale", CStr(SAU_Totale))
                xmlCampo.SetAttribute("sau_biologico", CStr(SAU_Biologico))
                xmlCampo.SetAttribute("sau_conversione", CStr(SAU_Conversione))
                xmlCampo.SetAttribute("sau_convenzionale", CStr(SAU_Convenzionale))
                xmlCampo.SetAttribute("confinirischio", CStr(ConfiniRischio))
                xmlCampo.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlCampo.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlCampo.SetAttribute("basecode", CStr(BaseCode))
                xmlCampo.SetAttribute("topcode", CStr(TopCode))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlCampo)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlCampo = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlCampo = xmlDoc.SelectSingleNode("//Campo")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlCampo.GetAttribute("TipoOperazioneDB"))
                Campo_Cod = CInt(xmlCampo.GetAttribute("campo_cod"))
                Campo_Des = CStr(xmlCampo.GetAttribute("campo_des"))
                Validita_Inizio = CDate(xmlCampo.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlCampo.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlCampo = Nothing
                xmlDoc = Nothing

        End Select

    End Sub


    '##########################################################################################
    Public Sub XML_Campo_2(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                           ByRef StringaXML As String,
                           ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                           ByRef Piva As String,
                           ByRef Sa_Cod As Integer,
                           ByRef Campo_Cod As Long,
                           ByRef Campo_Des As String,
                           ByRef Conversione_Inizio As Date,
                           ByRef Conversione_Fine As Date,
                           ByRef SAU_Totale As Decimal,
                           ByRef SAU_Biologico As Decimal,
                           ByRef SAU_Conversione As Decimal,
                           ByRef SAU_Convenzionale As Decimal,
                           ByRef ConfiniRischio As String,
                           ByRef Gru_Cod As Integer,
                           ByRef Veg_Cod As Integer,
                           ByRef Validita_Inizio As Date,
                           ByRef Validita_Fine As Date,
                           ByRef BaseCode As Integer,
                           ByRef TopCode As Integer)

        Dim xmlDoc As New XmlDocument
        Dim xmlCampo As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlCampo = xmlDoc.CreateElement("Campo")

                'Imposto gli attributi
                xmlCampo.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlCampo.SetAttribute("piva", CStr(Piva))
                xmlCampo.SetAttribute("sa_cod", CStr(Sa_Cod))
                xmlCampo.SetAttribute("campo_cod", CStr(Campo_Cod))
                xmlCampo.SetAttribute("campo_des", Campo_Des)
                xmlCampo.SetAttribute("conversione_inizio", CStr(Conversione_Inizio))
                xmlCampo.SetAttribute("conversione_fine", CStr(Conversione_Fine))
                xmlCampo.SetAttribute("sau_totale", CStr(SAU_Totale))
                xmlCampo.SetAttribute("sau_biologico", CStr(SAU_Biologico))
                xmlCampo.SetAttribute("sau_conversione", CStr(SAU_Conversione))
                xmlCampo.SetAttribute("sau_convenzionale", CStr(SAU_Convenzionale))
                xmlCampo.SetAttribute("confinirischio", CStr(ConfiniRischio))
                xmlCampo.SetAttribute("gru_cod", CStr(Gru_Cod))
                xmlCampo.SetAttribute("veg_cod", CStr(Veg_Cod))
                xmlCampo.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlCampo.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlCampo.SetAttribute("basecode", CStr(BaseCode))
                xmlCampo.SetAttribute("topcode", CStr(TopCode))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlCampo)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlCampo = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlCampo = xmlDoc.SelectSingleNode("//Campo")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlCampo.GetAttribute("TipoOperazioneDB"))
                Campo_Cod = CInt(xmlCampo.GetAttribute("campo_cod"))
                Campo_Des = CStr(xmlCampo.GetAttribute("campo_des"))
                Gru_Cod = CInt(xmlCampo.GetAttribute("gru_cod"))
                Veg_Cod = CInt(xmlCampo.GetAttribute("veg_cod"))
                Validita_Inizio = CDate(xmlCampo.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlCampo.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlCampo = Nothing
                xmlDoc = Nothing

        End Select

    End Sub


    '##########################################################################################
    ' A differenza di XML_Campo_2 gestisce "Campo_tipo"
    '##########################################################################################
    'Public Sub XML_Campo_3(ByVal OperazioneRichiesta As enum_CodificaDecodifica, _
    '                        ByRef StringaXML As String, _
    '                        ByRef TipoOperazioneDB As enum_TipoOperazioneDB, _
    '                        ByRef Piva As String, _
    '                        ByRef Sa_Cod As Integer, _
    '                        ByRef Campo_Cod As Long, _
    '                        ByRef Campo_tipo As Integer, _
    '                        ByRef Campo_Des As String, _
    '                        ByRef Conversione_Inizio As Date, _
    '                        ByRef Conversione_Fine As Date, _
    '                        ByRef SAU_Totale As Decimal, _
    '                        ByRef SAU_Biologico As Decimal, _
    '                        ByRef SAU_Conversione As Decimal, _
    '                        ByRef SAU_Convenzionale As Decimal, _
    '                        ByRef ConfiniRischio As String, _
    '                        ByRef Gru_Cod As Integer, _
    '                        ByRef Veg_Cod As Integer, _
    '                        ByRef Validita_Inizio As Date, _
    '                        ByRef Validita_Fine As Date, _
    '                        ByRef BaseCode As Integer, _
    '                        ByRef TopCode As Integer)

    '    Dim XmlDoc As New System.Xml.XmlDocument
    '    Dim XmlCampo As System.Xml.XmlElement
    '    Dim CampoAttribute As System.Xml.XmlAttribute

    '    'Verifico quale operazione deve essere effettuata
    '    Select Case OperazioneRichiesta

    '        Case enum_CodificaDecodifica.Codifica

    '            '----- Genero la stringa XML a partire dai valori dei parametri

    '            'Creo il nodo 
    '            XmlCampo = XmlDoc.CreateElement("Campo")

    '            'Imposto gli attributi
    '            XmlCampo.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
    '            XmlCampo.SetAttribute("piva", CStr(Piva))
    '            XmlCampo.SetAttribute("sa_cod", CStr(Sa_Cod))
    '            XmlCampo.SetAttribute("campo_cod", CStr(Campo_Cod))
    '            XmlCampo.SetAttribute("campo_tipo", CStr(Campo_tipo))
    '            XmlCampo.SetAttribute("campo_des", Campo_Des)
    '            XmlCampo.SetAttribute("conversione_inizio", CStr(Conversione_Inizio))
    '            XmlCampo.SetAttribute("conversione_fine", CStr(Conversione_Fine))
    '            XmlCampo.SetAttribute("sau_totale", CStr(SAU_Totale))
    '            XmlCampo.SetAttribute("sau_biologico", CStr(SAU_Biologico))
    '            XmlCampo.SetAttribute("sau_conversione", CStr(SAU_Conversione))
    '            XmlCampo.SetAttribute("sau_convenzionale", CStr(SAU_Convenzionale))
    '            XmlCampo.SetAttribute("confinirischio", CStr(ConfiniRischio))
    '            XmlCampo.SetAttribute("gru_cod", CStr(Gru_Cod))
    '            XmlCampo.SetAttribute("veg_cod", CStr(Veg_Cod))
    '            XmlCampo.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
    '            XmlCampo.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
    '            XmlCampo.SetAttribute("basecode", CStr(BaseCode))
    '            XmlCampo.SetAttribute("topcode", CStr(TopCode))



    '            'Imposto XmlIndirizzo come figlio del documento principale
    '            XmlDoc.AppendChild(XmlCampo)

    '            'Restituisco in uscita la stringa creata
    '            StringaXML = XmlDoc.InnerXml

    '            'Distruggo gli oggetti
    '            XmlCampo = Nothing
    '            XmlDoc = Nothing



    '        Case enum_CodificaDecodifica.Decodifica

    '            '----- Recupero i valori dei parametri a partire dalla stringa XML

    '            'Carico la stringa XML nel documento
    '            XmlDoc.LoadXml(StringaXML)

    '            'Prelevo il nodo
    '            XmlCampo = XmlDoc.SelectSingleNode("//Campo")

    '            'Prelevo gli attributi
    '            TipoOperazioneDB = CInt(XmlCampo.GetAttribute("TipoOperazioneDB"))
    '            Campo_Cod = CInt(XmlCampo.GetAttribute("campo_cod"))
    '            Campo_Des = CStr(XmlCampo.GetAttribute("campo_des"))
    '            Campo_tipo = CStr(XmlCampo.GetAttribute("campo_tipo"))
    '            Gru_Cod = CInt(XmlCampo.GetAttribute("gru_cod"))
    '            Veg_Cod = CInt(XmlCampo.GetAttribute("veg_cod"))
    '            Validita_Inizio = CDate(XmlCampo.GetAttribute("validita_inizio"))
    '            Validita_Fine = CDate(XmlCampo.GetAttribute("validita_fine"))
    '            BaseCode = 0
    '            TopCode = 0


    '            'Distruggo gli oggetti
    '            XmlCampo = Nothing
    '            XmlDoc = Nothing

    '    End Select

    'End Sub


    '##########################################################################################
    Public Sub XML_Appezzamento(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                                ByRef StringaXML As String,
                                ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                                ByRef Piva As String,
                                ByRef Sa_Cod As Integer,
                                ByRef Appezza As Integer,
                                ByRef Sup_App As Decimal,
                                ByRef Data_App As String,
                                ByRef Ep_Camp As String,
                                ByRef X As Decimal,
                                ByRef Y As Decimal,
                                ByRef Zslm As Decimal,
                                ByRef Esposiz As String,
                                ByRef Pende As Decimal,
                                ByRef Ubicazione As String,
                                ByRef Num_Del As Integer,
                                ByRef Clas As String,
                                ByRef Sabbia As Decimal,
                                ByRef Limo As Decimal,
                                ByRef Argilla As Decimal,
                                ByRef pH As Decimal,
                                ByRef CalTot As Decimal,
                                ByRef CalAtt As Decimal,
                                ByRef SostOrg As Decimal,
                                ByRef K2OAss As Decimal,
                                ByRef P2O5Ass As Decimal,
                                ByRef Mg As Decimal,
                                ByRef Ntot As Decimal,
                                ByRef Um_S As Decimal,
                                ByRef Cl_Dren As String,
                                ByRef Falda As Integer,
                                ByRef CsC As Decimal,
                                ByRef K2OAss_Data As String,
                                ByRef MatOrg As Decimal,
                                ByRef MatOrg_Data As String,
                                ByRef NOtot_Data As String,
                                ByRef NOtot As Decimal,
                                ByRef P2O5Ass_Data As String,
                                ByRef Suolo_CodAttri As String,
                                ByRef App_Nome As String,
                                ByRef Campo_Spia As Integer,
                                ByRef Campo_Spia_Area As Decimal,
                                ByRef Cs_SIPI As String,
                                ByRef Campo_Cod As Integer,
                                ByRef Prossimo As Integer,
                                ByRef Data_Inizio As String,
                                ByRef Data_Fine As String,
                                ByRef Validita_Inizio As Date,
                                ByRef Validita_Fine As Date,
                                ByRef BaseCode As Integer,
                                ByRef TopCode As Integer,
                                Optional ByRef BZ_CorpiIdrici As Double = 0,
                                Optional ByRef BZ_AreeResPub As Double = 0,
                                Optional ByRef BZ_Allevamenti As Double = 0,
                                Optional ByRef BZ_VegNatNonColt As Double = 0,
                                Optional ByRef BZ_SupRiduzione As Double = 0)

        Dim xmlDoc As New XmlDocument
        Dim xmlAppezzamento As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlAppezzamento = xmlDoc.CreateElement("Appezzamento")

                'Imposto gli attributi
                xmlAppezzamento.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

                xmlAppezzamento.SetAttribute("piva", CStr(Piva))
                xmlAppezzamento.SetAttribute("sa_cod", CStr(Sa_Cod))
                xmlAppezzamento.SetAttribute("appezza", CStr(Appezza))

                xmlAppezzamento.SetAttribute("sup_app", CStr(Sup_App))
                xmlAppezzamento.SetAttribute("data_app", CStr(Data_App))
                xmlAppezzamento.SetAttribute("ep_camp", CStr(Ep_Camp))
                xmlAppezzamento.SetAttribute("x", CStr(X))
                xmlAppezzamento.SetAttribute("y", CStr(Y))
                xmlAppezzamento.SetAttribute("zslm", CStr(Zslm))
                xmlAppezzamento.SetAttribute("esposiz", CStr(Esposiz))
                xmlAppezzamento.SetAttribute("pende", CStr(Pende))
                xmlAppezzamento.SetAttribute("ubicazione", CStr(Ubicazione))
                xmlAppezzamento.SetAttribute("num_del", CStr(Num_Del))
                xmlAppezzamento.SetAttribute("clas", CStr(Clas))

                xmlAppezzamento.SetAttribute("sabbia", CStr(Sabbia))
                xmlAppezzamento.SetAttribute("limo", CStr(Limo))
                xmlAppezzamento.SetAttribute("argilla", CStr(Argilla))
                xmlAppezzamento.SetAttribute("ph", CStr(pH))

                xmlAppezzamento.SetAttribute("caltot", CStr(CalTot))
                xmlAppezzamento.SetAttribute("calatt", CStr(CalAtt))
                xmlAppezzamento.SetAttribute("sostorg", CStr(SostOrg))
                xmlAppezzamento.SetAttribute("k2oass", CStr(K2OAss))
                xmlAppezzamento.SetAttribute("p2o5ass", CStr(P2O5Ass))
                xmlAppezzamento.SetAttribute("mg", CStr(Mg))
                xmlAppezzamento.SetAttribute("ntot", CStr(Ntot))
                xmlAppezzamento.SetAttribute("um_s", CStr(Um_S))
                xmlAppezzamento.SetAttribute("cl_dren", CStr(Cl_Dren))
                xmlAppezzamento.SetAttribute("falda", CStr(Falda))
                xmlAppezzamento.SetAttribute("csc", CStr(CsC))
                xmlAppezzamento.SetAttribute("k2oass_data", CStr(K2OAss_Data))
                xmlAppezzamento.SetAttribute("matorg", CStr(MatOrg))
                xmlAppezzamento.SetAttribute("matorg_data", CStr(MatOrg_Data))
                xmlAppezzamento.SetAttribute("notot_data", CStr(NOtot_Data))
                xmlAppezzamento.SetAttribute("notot", CStr(NOtot))
                xmlAppezzamento.SetAttribute("p2o5ass_data", CStr(P2O5Ass_Data))
                xmlAppezzamento.SetAttribute("suolo_codattri", CStr(Suolo_CodAttri))

                xmlAppezzamento.SetAttribute("app_nome", CStr(App_Nome))
                xmlAppezzamento.SetAttribute("campo_spia", CStr(Campo_Spia))
                xmlAppezzamento.SetAttribute("campo_spia_area", CStr(Campo_Spia_Area))
                xmlAppezzamento.SetAttribute("cs_sipi", CStr(Cs_SIPI))
                xmlAppezzamento.SetAttribute("campo_cod", CStr(Campo_Cod))
                xmlAppezzamento.SetAttribute("prossimo", CStr(Prossimo))
                xmlAppezzamento.SetAttribute("data_inizio", CStr(Data_Inizio))
                xmlAppezzamento.SetAttribute("data_fine", CStr(Data_Fine))

                xmlAppezzamento.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlAppezzamento.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlAppezzamento.SetAttribute("basecode", CStr(BaseCode))
                xmlAppezzamento.SetAttribute("topcode", CStr(TopCode))

                xmlAppezzamento.SetAttribute(LCase("BZ_CorpiIdrici"), CStr(BZ_CorpiIdrici))
                xmlAppezzamento.SetAttribute(LCase("BZ_AreeResPub"), CStr(BZ_AreeResPub))
                xmlAppezzamento.SetAttribute(LCase("BZ_Allevamenti"), CStr(BZ_Allevamenti))
                xmlAppezzamento.SetAttribute(LCase("BZ_VegNatNonColt"), CStr(BZ_VegNatNonColt))
                xmlAppezzamento.SetAttribute(LCase("BZ_SupRiduzione"), CStr(BZ_SupRiduzione))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlAppezzamento)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlAppezzamento = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlAppezzamento = xmlDoc.SelectSingleNode("//Appezzamento")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlAppezzamento.GetAttribute("TipoOperazioneDB"))

                Piva = CStr(xmlAppezzamento.GetAttribute("piva"))
                Sa_Cod = CInt(xmlAppezzamento.GetAttribute("sa_cod"))
                Appezza = CInt(xmlAppezzamento.GetAttribute("appezza"))

                Sup_App = CDbl(xmlAppezzamento.GetAttribute("sup_app"))
                Data_App = CStr(xmlAppezzamento.GetAttribute("data_app"))
                Ep_Camp = CStr(xmlAppezzamento.GetAttribute("ep_camp"))
                X = CDbl(xmlAppezzamento.GetAttribute("x"))
                Y = CDbl(xmlAppezzamento.GetAttribute("y"))
                Zslm = CDbl(xmlAppezzamento.GetAttribute("zslm"))

                Esposiz = CStr(xmlAppezzamento.GetAttribute("esposiz"))
                Pende = CDbl(xmlAppezzamento.GetAttribute("pende"))
                Ubicazione = CStr(xmlAppezzamento.GetAttribute("ubicazione"))
                Num_Del = CInt(xmlAppezzamento.GetAttribute("num_del"))
                Clas = CStr(xmlAppezzamento.GetAttribute("clas"))

                Sabbia = CDbl(xmlAppezzamento.GetAttribute("sabbia"))
                Limo = CDbl(xmlAppezzamento.GetAttribute("limo"))
                Argilla = CDbl(xmlAppezzamento.GetAttribute("argilla"))
                pH = CDbl(xmlAppezzamento.GetAttribute("ph"))

                CalTot = CDbl(xmlAppezzamento.GetAttribute("caltot"))
                CalAtt = CDbl(xmlAppezzamento.GetAttribute("calatt"))
                SostOrg = CDbl(xmlAppezzamento.GetAttribute("sostorg"))
                K2OAss = CDbl(xmlAppezzamento.GetAttribute("k2oass"))
                P2O5Ass = CDbl(xmlAppezzamento.GetAttribute("p2o5ass"))
                Mg = CDbl(xmlAppezzamento.GetAttribute("mg"))
                Ntot = CDbl(xmlAppezzamento.GetAttribute("ntot"))
                Um_S = CDbl(xmlAppezzamento.GetAttribute("um_s"))
                Cl_Dren = CStr(xmlAppezzamento.GetAttribute("cl_dren"))
                Falda = CInt(xmlAppezzamento.GetAttribute("falda"))
                CsC = CDbl(xmlAppezzamento.GetAttribute("csc"))
                K2OAss_Data = CStr(xmlAppezzamento.GetAttribute("k2oass_data"))
                MatOrg = CDbl(xmlAppezzamento.GetAttribute("matorg"))
                MatOrg_Data = CStr(xmlAppezzamento.GetAttribute("matorg_data"))
                NOtot_Data = CStr(xmlAppezzamento.GetAttribute("notot_data"))
                NOtot = CDbl(xmlAppezzamento.GetAttribute("notot"))
                P2O5Ass_Data = CStr(xmlAppezzamento.GetAttribute("p2o5ass_data"))
                Suolo_CodAttri = CStr(xmlAppezzamento.GetAttribute("suolo_codattri"))

                App_Nome = CStr(xmlAppezzamento.GetAttribute("app_nome"))
                Campo_Spia = CInt(xmlAppezzamento.GetAttribute("campo_spia"))
                Campo_Spia_Area = CDbl(xmlAppezzamento.GetAttribute("campo_spia_area"))
                Cs_SIPI = CStr(xmlAppezzamento.GetAttribute("cs_sipi"))
                Campo_Cod = CInt(xmlAppezzamento.GetAttribute("campo_cod"))
                Prossimo = CInt(xmlAppezzamento.GetAttribute("prossimo"))
                Data_Inizio = CStr(xmlAppezzamento.GetAttribute("data_inizio"))
                Data_Fine = CStr(xmlAppezzamento.GetAttribute("data_fine"))

                Validita_Inizio = CDate(xmlAppezzamento.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlAppezzamento.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                BZ_CorpiIdrici = CDbl(xmlAppezzamento.GetAttribute(LCase("BZ_CorpiIdrici")))
                BZ_AreeResPub = CDbl(xmlAppezzamento.GetAttribute(LCase("BZ_AreeResPub")))
                BZ_Allevamenti = CDbl(xmlAppezzamento.GetAttribute(LCase("BZ_Allevamenti")))
                BZ_VegNatNonColt = CDbl(xmlAppezzamento.GetAttribute(LCase("BZ_VegNatNonColt")))
                BZ_SupRiduzione = CDbl(xmlAppezzamento.GetAttribute(LCase("BZ_SupRiduzione")))

                'Distruggo gli oggetti
                xmlAppezzamento = Nothing
                xmlDoc = Nothing

        End Select

    End Sub


    '##########################################################################################
    Public Sub XML_Particella(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                              ByRef StringaXML As String,
                              ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                              ByRef Piva As String,
                              ByRef Sa_Cod As Integer,
                              ByRef Part_cod As Integer,
                              ByRef Prov As String,
                              ByRef Com As String,
                              ByRef Sezione As String,
                              ByRef Foglio As Integer,
                              ByRef Numero As Integer,
                              ByRef Subalterno As String,
                              ByRef Partita_Catastale As String,
                              ByRef Ettari As Decimal,
                              ByRef Are As Integer,
                              ByRef Centiare As Integer,
                              ByRef strTitoloPossesso As String,
                              ByRef Qualita_Cod As Integer,
                              ByRef Classe As String,
                              ByRef Reddito_Dominicale As Decimal,
                              ByRef Reddito_Agrario As Decimal,
                              ByRef Validita_Inizio As Date,
                              ByRef Validita_Fine As Date,
                              ByRef Validita_Inizio_Centro As Date,
                              ByRef Validita_Fine_Centro As Date,
                              ByRef BaseCode As Integer,
                              ByRef TopCode As Integer)

        Dim xmlDoc As New XmlDocument
        Dim xmlParticella As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlParticella = xmlDoc.CreateElement("Particella")

                'Imposto gli attributi
                xmlParticella.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                xmlParticella.SetAttribute("piva", CStr(Piva))
                xmlParticella.SetAttribute("sa_cod", CStr(Sa_Cod))
                xmlParticella.SetAttribute("part_cod", CStr(Part_cod))
                xmlParticella.SetAttribute("prov", CStr(Prov))
                xmlParticella.SetAttribute("com", CStr(Com))
                xmlParticella.SetAttribute("sezione", CStr(Sezione))
                xmlParticella.SetAttribute("foglio", CStr(Foglio))
                xmlParticella.SetAttribute("numero", CStr(Numero))
                xmlParticella.SetAttribute("subalterno", CStr(Subalterno))
                xmlParticella.SetAttribute("partita_catastale", CStr(Partita_Catastale))
                xmlParticella.SetAttribute("ettari", CStr(Ettari))
                xmlParticella.SetAttribute("are", CStr(Are))
                xmlParticella.SetAttribute("centiare", CStr(Centiare))
                xmlParticella.SetAttribute("titolopossesso", strTitoloPossesso)
                xmlParticella.SetAttribute("qualita_cod", CStr(Qualita_Cod))
                xmlParticella.SetAttribute("classe", CStr(Classe))
                xmlParticella.SetAttribute("reddito_dominicale", CStr(Reddito_Dominicale))
                xmlParticella.SetAttribute("reddito_agrario", CStr(Reddito_Agrario))
                xmlParticella.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlParticella.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlParticella.SetAttribute("validita_inizio_centro", Format(Validita_Inizio_Centro, "dd/MM/yyyy"))
                xmlParticella.SetAttribute("validita_fine_centro", Format(Validita_Fine_Centro, "dd/MM/yyyy"))
                xmlParticella.SetAttribute("basecode", CStr(BaseCode))
                xmlParticella.SetAttribute("topcode", CStr(TopCode))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlParticella)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlParticella = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlParticella = xmlDoc.SelectSingleNode("//Particella")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlParticella.GetAttribute("TipoOperazioneDB"))

                Piva = CStr(xmlParticella.GetAttribute("piva"))
                Sa_Cod = CInt(xmlParticella.GetAttribute("sa_cod"))
                Part_cod = CInt(xmlParticella.GetAttribute("part_cod"))
                Prov = CStr(xmlParticella.GetAttribute("prov"))
                Com = CStr(xmlParticella.GetAttribute("com"))
                Sezione = CStr(xmlParticella.GetAttribute("sezione"))
                Foglio = CInt(xmlParticella.GetAttribute("foglio"))
                Numero = CInt(xmlParticella.GetAttribute("numero"))
                Subalterno = CStr(xmlParticella.GetAttribute("subalterno"))
                Partita_Catastale = CStr(xmlParticella.GetAttribute("partita_catastale"))
                Ettari = CDbl(xmlParticella.GetAttribute("ettari"))
                Are = CInt(xmlParticella.GetAttribute("are"))
                Centiare = CInt(xmlParticella.GetAttribute("centiare"))
                Qualita_Cod = CInt(xmlParticella.GetAttribute("qualita_cod"))
                Classe = CStr(xmlParticella.GetAttribute("classe"))
                Reddito_Dominicale = CDbl(xmlParticella.GetAttribute("reddito_dominicale"))
                Reddito_Agrario = CDbl(xmlParticella.GetAttribute("reddito_agrario"))
                Validita_Inizio = CDate(xmlParticella.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlParticella.GetAttribute("validita_fine"))
                Validita_Inizio_Centro = CDate(xmlParticella.GetAttribute("validita_inizio_centro"))
                Validita_Fine_Centro = CDate(xmlParticella.GetAttribute("validita_fine_centro"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlParticella = Nothing
                xmlDoc = Nothing

        End Select

    End Sub


    '##########################################################################################
    Public Sub XML_Impianto(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                            ByRef StringaXML As String,
                            ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                            ByRef Piva As String,
                            ByRef Sa_Cod As Integer,
                            ByRef Campo_Cod As Integer,
                            ByRef Appezza As Integer,
                            ByRef Id_Imp As Integer,
                            ByRef Sup_Imp As Decimal,
                            ByRef Cod_Resp As Integer,
                            ByRef Cod_Ente As Integer,
                            ByRef Campo_Spia As Integer,
                            ByRef Data As Date,
                            ByRef Cul_Cod As Integer,
                            ByRef Grva_Cod_Veg As Integer,
                            ByRef Data_Raccolta As String,
                            ByRef ResaPrevista As Decimal,
                            ByRef ResaEffettiva As Decimal,
                            ByRef Scarto As Integer,
                            ByRef Ind_Mat_Cod As Integer,
                            ByRef Ind_Mat_Ril As String,
                            ByRef Sta_Ter As String,
                            ByRef Cop_DI As String,
                            ByRef Cop_DF As String,
                            ByRef Tra_Fila As Decimal,
                            ByRef Su_Fila As Decimal,
                            ByRef P_HA As Decimal,
                            ByRef Setup_Cod As String,
                            ByRef Port_Cod As Integer,
                            ByRef Stru_Prot As Integer,
                            ByRef Pro_Pag As Integer,
                            ByRef Seme_Q As Integer,
                            ByRef Seme_T As Integer,
                            ByRef Seme_P As Integer,
                            ByRef Seme_D As Integer,
                            ByRef Stato_Residui As String,
                            ByRef Denitrificazione As Integer,
                            ByRef Volatilizzazione As Integer,
                            ByRef ProfonditaLav As Integer,
                            ByRef Cover As Integer,
                            ByRef Monitorato As Integer,
                            ByRef Codice_Ficale_Tecnico As String,
                            ByRef Data_Conversione As String,
                            ByRef Grfi_Cod As Integer,
                            ByRef Imp_Cod As Integer,
                            ByRef Regolamento As Integer,
                            ByRef Finanziamento As Integer,
                            ByRef Su_Cod As Integer,
                            ByRef Cop_Cod As Integer,
                            ByRef Foral_Cod As Integer,
                            ByRef Tecn_Cod As Integer,
                            ByRef ProvenienzaSeme As Integer,
                            ByRef Validita_Inizio As Date,
                            ByRef Validita_Fine As Date,
                            ByRef BaseCode As Integer,
                            ByRef TopCode As Integer,
                            Optional ByRef Id_Consociazione As Integer = 0,
                            Optional ByRef Unita_Vitata As String = "",
                            Optional ByRef destinazioneuso_old As String = "0",
                            Optional ByRef destinazioneuso As String = "")

        Dim xmlDoc As New XmlDocument
        Dim xmlImpianto As XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                xmlImpianto = xmlDoc.CreateElement("Reg_Impianto")

                'Imposto gli attributi
                xmlImpianto.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

                xmlImpianto.SetAttribute("piva", CStr(Piva))
                xmlImpianto.SetAttribute("sa_cod", CStr(Sa_Cod))
                xmlImpianto.SetAttribute("id_campo", CStr(Campo_Cod))
                xmlImpianto.SetAttribute("appezza", CStr(Appezza))

                xmlImpianto.SetAttribute("sup_imp", CStr(Sup_Imp))
                xmlImpianto.SetAttribute("id_consociazione", CStr(Id_Consociazione))
                xmlImpianto.SetAttribute("unita_vitata", CStr(Unita_Vitata))

                xmlImpianto.SetAttribute("id_reg", CStr(Id_Imp))
                xmlImpianto.SetAttribute("cod_resp", CStr(Cod_Resp))
                xmlImpianto.SetAttribute("cod_ente", CStr(Cod_Ente))
                xmlImpianto.SetAttribute("campo_spia", CStr(Campo_Spia))
                xmlImpianto.SetAttribute("data", CStr(Data))
                xmlImpianto.SetAttribute("cul_cod", CStr(Cul_Cod))
                xmlImpianto.SetAttribute("grva_cod_veg", CStr(Grva_Cod_Veg))
                xmlImpianto.SetAttribute("data_raccolta", CStr(Data_Raccolta))
                xmlImpianto.SetAttribute("resa_prevista", CStr(ResaPrevista))
                xmlImpianto.SetAttribute("resa_effettiva", CStr(ResaEffettiva))
                xmlImpianto.SetAttribute("scarto", CStr(Scarto))
                xmlImpianto.SetAttribute("ind_mat_cod", CStr(Ind_Mat_Cod))
                xmlImpianto.SetAttribute("ind_mat_ril", CStr(Ind_Mat_Ril))
                xmlImpianto.SetAttribute("sta_ter", CStr(Sta_Ter))
                xmlImpianto.SetAttribute("cop_di", CStr(Cop_DI))
                xmlImpianto.SetAttribute("cop_df", CStr(Cop_DF))
                xmlImpianto.SetAttribute("tra_fila", CStr(Tra_Fila))
                xmlImpianto.SetAttribute("su_fila", CStr(Su_Fila))
                xmlImpianto.SetAttribute("p_ha", CStr(P_HA))
                xmlImpianto.SetAttribute("setup_cod", CStr(Setup_Cod))
                xmlImpianto.SetAttribute("port_cod", CStr(Port_Cod))
                xmlImpianto.SetAttribute("stru_prot", CStr(Stru_Prot))
                xmlImpianto.SetAttribute("pro_pag", CStr(Pro_Pag))
                xmlImpianto.SetAttribute("seme_q", CStr(Seme_Q))
                xmlImpianto.SetAttribute("seme_t", CStr(Seme_T))
                xmlImpianto.SetAttribute("seme_p", CStr(Seme_P))
                xmlImpianto.SetAttribute("seme_d", CStr(Seme_D))

                xmlImpianto.SetAttribute("stato_residui", CStr(Stato_Residui))
                xmlImpianto.SetAttribute("denitrificazione", CStr(Denitrificazione))
                xmlImpianto.SetAttribute("volatilizzazione", CStr(Volatilizzazione))
                xmlImpianto.SetAttribute("profonditalav", CStr(ProfonditaLav))
                xmlImpianto.SetAttribute("cover", CStr(Cover))
                xmlImpianto.SetAttribute("monitorato", CStr(Monitorato))
                xmlImpianto.SetAttribute("codice_fiscale_tecnico", CStr(Codice_Ficale_Tecnico))
                xmlImpianto.SetAttribute("data_conversione", CStr(Data_Conversione))
                xmlImpianto.SetAttribute("grfi_cod", CStr(Grfi_Cod))
                xmlImpianto.SetAttribute("imp_cod", CStr(Imp_Cod))
                xmlImpianto.SetAttribute("regolamento", CStr(Regolamento))
                xmlImpianto.SetAttribute("finanziamento", CStr(Finanziamento))
                xmlImpianto.SetAttribute("su_cod", CStr(Su_Cod))
                xmlImpianto.SetAttribute("cop_cod", CStr(Cop_Cod))
                xmlImpianto.SetAttribute("foral_cod", CStr(Foral_Cod))
                xmlImpianto.SetAttribute("tecn_cod", CStr(Tecn_Cod))
                xmlImpianto.SetAttribute("provenienzaseme", CStr(ProvenienzaSeme))

                xmlImpianto.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                xmlImpianto.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                xmlImpianto.SetAttribute("basecode", CStr(BaseCode))
                xmlImpianto.SetAttribute("topcode", CStr(TopCode))

                xmlImpianto.SetAttribute("destinazioneuso_old", CStr(destinazioneuso_old))
                xmlImpianto.SetAttribute("destinazioneuso", CStr(destinazioneuso))

                'Imposto XmlIndirizzo come figlio del documento principale
                xmlDoc.AppendChild(xmlImpianto)

                'Restituisco in uscita la stringa creata
                StringaXML = xmlDoc.InnerXml

                'Distruggo gli oggetti
                xmlImpianto = Nothing
                xmlDoc = Nothing



            Case enum_CodificaDecodifica.Decodifica

                '----- Recupero i valori dei parametri a partire dalla stringa XML

                'Carico la stringa XML nel documento
                xmlDoc.LoadXml(StringaXML)

                'Prelevo il nodo
                xmlImpianto = xmlDoc.SelectSingleNode("//Reg_Impianto")

                'Prelevo gli attributi
                TipoOperazioneDB = CInt(xmlImpianto.GetAttribute("TipoOperazioneDB"))

                Piva = CStr(xmlImpianto.GetAttribute("piva"))
                Sa_Cod = CInt(xmlImpianto.GetAttribute("sa_cod"))
                Campo_Cod = CInt(xmlImpianto.GetAttribute("id_campo"))
                Appezza = CInt(xmlImpianto.GetAttribute("appezza"))



                Validita_Inizio = CDate(xmlImpianto.GetAttribute("validita_inizio"))
                Validita_Fine = CDate(xmlImpianto.GetAttribute("validita_fine"))
                BaseCode = 0
                TopCode = 0

                'Distruggo gli oggetti
                xmlImpianto = Nothing
                xmlDoc = Nothing

        End Select

    End Sub





    '##########################################################################################
    '##########################################################################################
    '##########################################################################################
    '##########################################################################################

    '##########################################################################################
    '##########################################################################################
    '##########################################################################################
    '##########################################################################################





    '##########################################################################################
    'OVERLOADED
    'NUOVA: XML_2_Agenda_Agenda
    Public Function XML_Agenda_Agenda(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                      Optional ByVal Piva As String = "00000000000",
                                      Optional ByVal Sa_Cod As Integer = 0,
                                      Optional ByVal ID_Agenda As Integer = 0,
                                      Optional ByVal Des_Lib As String = "",
                                      Optional ByVal Lav_Cod As Integer = 0,
                                      Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                      Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                      Optional ByVal BaseCode As Integer = 0,
                                      Optional ByVal TopCode As Integer = 200000000
                                      ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Agenda")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("piva"), Piva)
        xmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        xmlTxt.SetAttribute(LCase("des_lib"), Des_Lib)
        xmlTxt.SetAttribute(LCase("lav_cod"), CStr(Lav_Cod))
        xmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function


    '##########################################################################################
    'OVERLOADED
    'NUOVA: XML_2_Agenda_Movimento
    Public Function XML_Agenda_Movimento(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                         Optional ByVal Piva As String = "00000000000",
                                         Optional ByVal Sa_Cod As Integer = 0,
                                         Optional ByVal ID_Agenda As Integer = 0,
                                         Optional ByVal ID_Mov As Integer = 0,
                                         Optional ByVal Cod_RisUm As Integer = 0,
                                         Optional ByVal Cau_Mov As String = "",
                                         Optional ByVal Mov_Desc As String = "",
                                         Optional ByVal Data_Movimento As Date = #1/1/1900#,
                                         Optional ByVal Scadenza As Date = #12/31/2100#,
                                         Optional ByVal Doc_Numero As Integer = 0,
                                         Optional ByVal Num_Protocollo As Decimal = 0.0,
                                         Optional ByVal Mezzo As Integer = 0,
                                         Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                         Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                         Optional ByVal BaseCode As Integer = 0,
                                         Optional ByVal TopCode As Integer = 200000000,
                                         Optional ByVal Ora As String = "00.00",
                                         Optional ByVal Extra_Int As Integer = 0
                                         ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Movimento")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("piva"), Piva)
        xmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        xmlTxt.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        xmlTxt.SetAttribute(LCase("Cod_RisUm"), CStr(Cod_RisUm))
        xmlTxt.SetAttribute(LCase("Cau_Mov"), Cau_Mov)
        xmlTxt.SetAttribute(LCase("Mov_Desc"), Mov_Desc)
        xmlTxt.SetAttribute(LCase("Data_Movimento"), Format(Data_Movimento, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Scadenza"), Format(Scadenza, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Doc_Numero"), CStr(Doc_Numero))
        xmlTxt.SetAttribute(LCase("Num_Protocollo"), CStr(Num_Protocollo))
        xmlTxt.SetAttribute(LCase("Mezzo"), CStr(Mezzo))
        xmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))
        xmlTxt.SetAttribute(LCase("ora"), CStr(Ora))
        xmlTxt.SetAttribute(LCase("extra_int"), CStr(Extra_Int))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function


    '##########################################################################################
    Public Function XML_Agenda_MovimentoDettaglioTecnico(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                         Optional ByVal Piva As String = "00000000000",
                                                         Optional ByVal Sa_Cod As Integer = 0,
                                                         Optional ByVal ID_Agenda As Integer = 0,
                                                         Optional ByVal ID_Mov As Integer = 0,
                                                         Optional ByVal ID_Mov_Det As Integer = 0,
                                                         Optional ByVal ID_Reg_Dettaglio As Integer = 0,
                                                         Optional ByVal Qta_Ril As Decimal = 0,
                                                         Optional ByVal Data_Ril As String = "0",
                                                         Optional ByVal Ditta_Cod As Integer = 0,
                                                         Optional ByVal Dett_Cod As Integer = 0,
                                                         Optional ByVal ID_Insetto As Integer = 0,
                                                         Optional ByVal FF_Classe As Integer = 0,
                                                         Optional ByVal Dose As Decimal = 0,
                                                         Optional ByVal Mg As Decimal = 0,
                                                         Optional ByVal N As Decimal = 0,
                                                         Optional ByVal K As Decimal = 0,
                                                         Optional ByVal P As Decimal = 0,
                                                         Optional ByVal Parziale As Integer = 0,
                                                         Optional ByVal Nitrati As Integer = 0,
                                                         Optional ByVal Freatimetro As Decimal = 0,
                                                         Optional ByVal Piezo1 As Decimal = 0,
                                                         Optional ByVal Piezo2 As Decimal = 0,
                                                         Optional ByVal Piezo3 As Decimal = 0,
                                                         Optional ByVal Piezo4 As Decimal = 0,
                                                         Optional ByVal Sigla_AV As String = "0",
                                                         Optional ByVal Trap_Num As Integer = 0,
                                                         Optional ByVal Inn1_Data As String = "0",
                                                         Optional ByVal Inn2_Data As String = "0",
                                                         Optional ByVal Inn3_Data As String = "0",
                                                         Optional ByVal Inn4_Data As String = "0",
                                                         Optional ByVal Av_Cod As Integer = 0,
                                                         Optional ByVal Av_Gru As Integer = 0,
                                                         Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                                         Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                                         Optional ByVal BaseCode As Integer = 0,
                                                         Optional ByVal TopCode As Integer = 200000000,
                                                         Optional ByVal Lotto As String = ""
                                                         ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Movimento_Dettaglio_Tecnico")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("piva"), Piva)
        xmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        xmlTxt.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        xmlTxt.SetAttribute(LCase("ID_Mov_Det"), CStr(ID_Mov_Det))
        xmlTxt.SetAttribute(LCase("ID_Reg_Dettaglio"), CStr(ID_Reg_Dettaglio))
        xmlTxt.SetAttribute(LCase("Qta_Ril"), CStr(Qta_Ril))
        xmlTxt.SetAttribute(LCase("Data_Ril"), Data_Ril)
        xmlTxt.SetAttribute(LCase("Ditta_Cod"), CStr(Ditta_Cod))
        xmlTxt.SetAttribute(LCase("Dett_Cod"), CStr(Dett_Cod))
        xmlTxt.SetAttribute(LCase("ID_Insetto"), CStr(ID_Insetto))
        xmlTxt.SetAttribute(LCase("FF_Classe"), CStr(FF_Classe))
        xmlTxt.SetAttribute(LCase("Dose"), CStr(Dose))
        xmlTxt.SetAttribute(LCase("Mg"), CStr(Mg))
        xmlTxt.SetAttribute(LCase("N"), CStr(N))
        xmlTxt.SetAttribute(LCase("K"), CStr(K))
        xmlTxt.SetAttribute(LCase("P"), CStr(P))
        xmlTxt.SetAttribute(LCase("Parziale"), CStr(Parziale))
        xmlTxt.SetAttribute(LCase("Nitrati"), CStr(Nitrati))
        xmlTxt.SetAttribute(LCase("Freatimetro"), CStr(Freatimetro))
        xmlTxt.SetAttribute(LCase("Piezo1"), CStr(Piezo1))
        xmlTxt.SetAttribute(LCase("Piezo2"), CStr(Piezo2))
        xmlTxt.SetAttribute(LCase("Piezo3"), CStr(Piezo3))
        xmlTxt.SetAttribute(LCase("Piezo4"), CStr(Piezo4))
        xmlTxt.SetAttribute(LCase("Sigla_AV"), Sigla_AV)
        xmlTxt.SetAttribute(LCase("Trap_Num"), CStr(Trap_Num))
        xmlTxt.SetAttribute(LCase("Inn1_Data"), Inn1_Data)
        xmlTxt.SetAttribute(LCase("Inn2_Data"), Inn2_Data)
        xmlTxt.SetAttribute(LCase("Inn3_Data"), Inn3_Data)
        xmlTxt.SetAttribute(LCase("Inn4_Data"), Inn4_Data)
        xmlTxt.SetAttribute(LCase("Av_Cod"), CStr(Av_Cod))
        xmlTxt.SetAttribute(LCase("Av_Gru"), CStr(Av_Gru))
        xmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))
        xmlTxt.SetAttribute(LCase("lotto"), CStr(Lotto))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function

    '##########################################################################################
    Public Function XML_Agenda_MovimentoDettaglioTecnico_2(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                           Optional ByVal Piva As String = "00000000000",
                                                           Optional ByVal Sa_Cod As Integer = 0,
                                                           Optional ByVal ID_Agenda As Integer = 0,
                                                           Optional ByVal ID_Mov As Integer = 0,
                                                           Optional ByVal ID_Mov_Det As Integer = 0,
                                                           Optional ByVal ID_Reg_Dettaglio As Integer = 0,
                                                           Optional ByVal Qta_Ril As Decimal = 0,
                                                           Optional ByVal Data_Ril As String = "0",
                                                           Optional ByVal Ditta_Cod As Integer = 0,
                                                           Optional ByVal Dett_Cod As Integer = 0,
                                                           Optional ByVal ID_Insetto As Integer = 0,
                                                           Optional ByVal FF_Classe As Integer = 0,
                                                           Optional ByVal Dose As Decimal = 0,
                                                           Optional ByVal Mg As Decimal = 0,
                                                           Optional ByVal N As Decimal = 0,
                                                           Optional ByVal K As Decimal = 0,
                                                           Optional ByVal P As Decimal = 0,
                                                           Optional ByVal Parziale As Integer = 0,
                                                           Optional ByVal Nitrati As Integer = 0,
                                                           Optional ByVal Freatimetro As Decimal = 0,
                                                           Optional ByVal Piezo1 As Decimal = 0,
                                                           Optional ByVal Piezo2 As Decimal = 0,
                                                           Optional ByVal Piezo3 As Decimal = 0,
                                                           Optional ByVal Piezo4 As Decimal = 0,
                                                           Optional ByVal Sigla_AV As String = "0",
                                                           Optional ByVal Trap_Num As Integer = 0,
                                                           Optional ByVal Inn1_Data As String = "0",
                                                           Optional ByVal Inn2_Data As String = "0",
                                                           Optional ByVal Inn3_Data As String = "0",
                                                           Optional ByVal Inn4_Data As String = "0",
                                                           Optional ByVal Av_Cod As Integer = 0,
                                                           Optional ByVal Av_Gru As Integer = 0,
                                                           Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                                           Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                                           Optional ByVal BaseCode As Integer = 0,
                                                           Optional ByVal TopCode As Integer = 200000000) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Movimento_Dettaglio_Tecnico_2")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("piva"), Piva)
        xmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        xmlTxt.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        xmlTxt.SetAttribute(LCase("ID_Mov_Det"), CStr(ID_Mov_Det))
        xmlTxt.SetAttribute(LCase("ID_Reg_Dettaglio"), CStr(ID_Reg_Dettaglio))
        xmlTxt.SetAttribute(LCase("Qta_Ril"), CStr(Qta_Ril))
        xmlTxt.SetAttribute(LCase("Data_Ril"), Data_Ril)
        xmlTxt.SetAttribute(LCase("Ditta_Cod"), CStr(Ditta_Cod))
        xmlTxt.SetAttribute(LCase("Dett_Cod"), CStr(Dett_Cod))
        xmlTxt.SetAttribute(LCase("ID_Insetto"), CStr(ID_Insetto))
        xmlTxt.SetAttribute(LCase("FF_Classe"), CStr(FF_Classe))
        xmlTxt.SetAttribute(LCase("Dose"), CStr(Dose))
        xmlTxt.SetAttribute(LCase("Mg"), CStr(Mg))
        xmlTxt.SetAttribute(LCase("N"), CStr(N))
        xmlTxt.SetAttribute(LCase("K"), CStr(K))
        xmlTxt.SetAttribute(LCase("P"), CStr(P))
        xmlTxt.SetAttribute(LCase("Parziale"), CStr(Parziale))
        xmlTxt.SetAttribute(LCase("Nitrati"), CStr(Nitrati))
        xmlTxt.SetAttribute(LCase("Freatimetro"), CStr(Freatimetro))
        xmlTxt.SetAttribute(LCase("Piezo1"), CStr(Piezo1))
        xmlTxt.SetAttribute(LCase("Piezo2"), CStr(Piezo2))
        xmlTxt.SetAttribute(LCase("Piezo3"), CStr(Piezo3))
        xmlTxt.SetAttribute(LCase("Piezo4"), CStr(Piezo4))
        xmlTxt.SetAttribute(LCase("Sigla_AV"), Sigla_AV)
        xmlTxt.SetAttribute(LCase("Trap_Num"), CStr(Trap_Num))
        xmlTxt.SetAttribute(LCase("Inn1_Data"), Inn1_Data)
        xmlTxt.SetAttribute(LCase("Inn2_Data"), Inn2_Data)
        xmlTxt.SetAttribute(LCase("Inn3_Data"), Inn3_Data)
        xmlTxt.SetAttribute(LCase("Inn4_Data"), Inn4_Data)
        xmlTxt.SetAttribute(LCase("Av_Cod"), CStr(Av_Cod))
        xmlTxt.SetAttribute(LCase("Av_Gru"), CStr(Av_Gru))
        xmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function

    '##########################################################################################
    'OVERLOADED
    'NUOVA: XML_2_Agenda_MovimentoDestinazione
    Public Function XML_Agenda_MovimentoDestinazione(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
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
                                                     Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                                     Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                                     Optional ByVal BaseCode As Integer = 0,
                                                     Optional ByVal TopCode As Integer = 200000000
                                                     ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Movimento_Destinazione")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("piva"), Piva)
        xmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        xmlTxt.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        xmlTxt.SetAttribute(LCase("ID_Mov_Det"), CStr(ID_Mov_Det))
        xmlTxt.SetAttribute(LCase("Appezza"), CStr(Appezza))
        xmlTxt.SetAttribute(LCase("ID_Destinazione"), CStr(ID_Destinazione))
        xmlTxt.SetAttribute(LCase("Tipo_Destinazione"), CStr(Tipo_Destinazione))
        xmlTxt.SetAttribute(LCase("Qta"), CStr(Qta))
        xmlTxt.SetAttribute(LCase("Qta2"), CStr(Qta2))
        xmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function

    '##########################################################################################
    Public Function XML_Agenda_RaccoltoCampionatura(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                    Optional ByVal Progressivo As Integer = 0,
                                                    Optional ByVal Tipo As String = "calibro",
                                                    Optional ByVal Tipo_Cod As Integer = 0,
                                                    Optional ByVal Udm_Cod As Integer = 0,
                                                    Optional ByVal Val_Cod As String = "0",
                                                    Optional ByVal Descrizione As String = "",
                                                    Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                                    Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                                    Optional ByVal BaseCode As Integer = 0,
                                                    Optional ByVal TopCode As Integer = 200000000
                                                    ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Raccolto_Campionatura")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("progressivo"), Progressivo)
        xmlTxt.SetAttribute(LCase("tipo"), CStr(Tipo))
        xmlTxt.SetAttribute(LCase("tipo_cod"), CStr(Tipo_Cod))
        xmlTxt.SetAttribute(LCase("udm_cod"), CStr(Udm_Cod))
        xmlTxt.SetAttribute(LCase("val_cod"), CStr(Val_Cod))
        xmlTxt.SetAttribute(LCase("descrizione"), CStr(Descrizione))
        xmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function

    '##########################################################################################
    Public Function XML_Agenda_ParcoMacchine(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                             Optional ByVal Piva As String = "00000000000",
                                             Optional ByVal Sa_Cod As Integer = 0,
                                             Optional ByVal Mac_Cod As Integer = 0,
                                             Optional ByVal Class_Code As String = "",
                                             Optional ByVal Mac_Des As String = "",
                                             Optional ByVal Costo_Acquisto As Decimal = 0,
                                             Optional ByVal Targa As String = "",
                                             Optional ByVal Telaio As String = "",
                                             Optional ByVal Ditta_Cod As Integer = 0,
                                             Optional ByVal Modello As String = "",
                                             Optional ByVal Potenza As String = "",
                                             Optional ByVal Ammortamento As Decimal = 0,
                                             Optional ByVal Ammortizzato As Decimal = 0,
                                             Optional ByVal Data_Immatricolazione As Date = #1/1/1900#,
                                             Optional ByVal Ultima_Manutenzione As Date = #1/1/1900#,
                                             Optional ByVal Data_Revisione As Date = #1/1/1900#,
                                             Optional ByVal Stato_Utilizzo As String = "",
                                             Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                             Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                             Optional ByVal BaseCode As Integer = 0,
                                             Optional ByVal TopCode As Integer = 200000000,
                                             Optional ByVal Note As String = "",
                                             Optional ByVal Tipo As Integer = 0,
                                             Optional ByVal N_Immatricolazione As String = "",
                                             Optional ByVal N_Immatricolazione_Rimorchio As String = "",
                                             Optional ByVal N_Autorizzazione_Trasporto As String = "",
                                             Optional ByVal Data_Rilascio_Autorizzazione As Date = #1/1/1900#,
                                             Optional ByVal Peso As Decimal = 0
                                             ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement
        Dim XML_DatiParcoMacchine As XmlElement

        '######################################################################################################
        '----- Dati parco macchine

        'Creo un tag volante di nome dati movimenti
        XML_DatiParcoMacchine = xmlDoc.CreateElement("DatiParcoMacchine")

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 

        xmlTxt = xmlDoc.CreateElement("ParcoMacchina")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("piva"), Piva)
        xmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("Mac_Cod"), CStr(Mac_Cod))
        xmlTxt.SetAttribute(LCase("Class_Code"), CStr(Class_Code))
        xmlTxt.SetAttribute(LCase("Mac_Des"), CStr(Mac_Des))
        xmlTxt.SetAttribute(LCase("Costo_Acquisto"), CStr(Costo_Acquisto))
        xmlTxt.SetAttribute(LCase("Targa"), CStr(Targa))
        xmlTxt.SetAttribute(LCase("Telaio"), CStr(Telaio))
        xmlTxt.SetAttribute(LCase("Ditta_Cod"), CStr(Ditta_Cod))
        xmlTxt.SetAttribute(LCase("Modello"), CStr(Modello))
        xmlTxt.SetAttribute(LCase("Potenza"), CStr(Potenza))
        xmlTxt.SetAttribute(LCase("Ammortamento"), CStr(Ammortamento))
        xmlTxt.SetAttribute(LCase("Ammortizzato"), CStr(Ammortizzato))
        xmlTxt.SetAttribute(LCase("Data_Immatricolazione"), Format(Data_Immatricolazione, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Ultima_Manutenzione"), Format(Ultima_Manutenzione, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Ultima_Revisione"), Format(Data_Revisione, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Stato_Utilizzo"), CStr(Stato_Utilizzo))
        xmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))
        xmlTxt.SetAttribute(LCase("Note"), CStr(Note))
        xmlTxt.SetAttribute(LCase("Tipo"), CStr(Tipo))
        xmlTxt.SetAttribute(LCase("N_Immatricolazione"), CStr(N_Immatricolazione))
        xmlTxt.SetAttribute(LCase("N_Immatricolazione_Rimorchio"), CStr(N_Immatricolazione_Rimorchio))
        xmlTxt.SetAttribute(LCase("N_Autorizzazione_Trasporto"), CStr(N_Autorizzazione_Trasporto))
        xmlTxt.SetAttribute(LCase("Data_Rilascio_Autorizzazione"), Format(Data_Rilascio_Autorizzazione, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Peso"), CStr(Peso))

        'Rendo il tag appena creato figlio del nodo Parco Macchine
        XML_DatiParcoMacchine.AppendChild(xmlTxt)

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(XML_DatiParcoMacchine)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function


    '##########################################################################################
    'OVERLOADED
    'NUOVA: XML_2_Agenda_MovimentoDettaglio

    'Questa routine risale ai tempi antichi...
    'visto che è usata da tutte le operazioni di agenda, vegetali e zootecniche, e per il parco macchine
    'non voglio sapere di eventuali danni e quindi
    'ho creato XML_Agenda_MovimentoDettaglio_New che crea l'xml per la nuova struttura
    'della tabella Movimenti_Dettagli (usata x magazzino e contabilità) 
    'comment by Maga
    Public Function XML_Agenda_MovimentoDettaglio(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                  Optional ByVal Piva As String = "00000000000",
                                                  Optional ByVal Sa_Cod As Integer = 0,
                                                  Optional ByVal ID_Agenda As Integer = 0,
                                                  Optional ByVal ID_Mov As Integer = 0,
                                                  Optional ByVal ID_Mov_Det As Integer = 0,
                                                  Optional ByVal Elem_Cod As Integer = 0,
                                                  Optional ByVal Pro_Cod As Integer = 0,
                                                  Optional ByVal Mat_Cod As Integer = 0,
                                                  Optional ByVal Mov_Det_Des As String = "",
                                                  Optional ByVal Udm_Cod As Integer = 0,
                                                  Optional ByVal Extra_Int As Integer = 0,
                                                  Optional ByVal Qta As Decimal = 0,
                                                  Optional ByVal Cod_IVA As Integer = 0,
                                                  Optional ByVal Sconto As Decimal = 0,
                                                  Optional ByVal Prezzo_Unitario As Decimal = 0,
                                                  Optional ByVal Cod_Conto As Integer = 0,
                                                  Optional ByVal Cod_Progetto As Integer = 0,
                                                  Optional ByVal Fase_Cod As Integer = 0,
                                                  Optional ByVal Contabilizzato As Integer = 1,
                                                  Optional ByVal Pendente As Integer = enum_Pendenza.MovESENTE,
                                                  Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                                  Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                                  Optional ByVal BaseCode As Integer = 0,
                                                  Optional ByVal TopCode As Integer = 200000000,
                                                  Optional ByVal ID_Destinazione As Integer = 0,
                                                  Optional ByVal Cau_Mov As String = "",
                                                  Optional ByVal Old_Qta As Decimal = 0,
                                                  Optional ByVal Old_Prezzo_Unitario As Decimal = 0,
                                                  Optional ByVal Cal_Cod As Integer = 0,
                                                  Optional ByVal Lotto As String = "",
                                                  Optional ByVal Anno As Integer = 1900,
                                                  Optional ByVal Udm_Cod_Extra As Integer = 0,
                                                  Optional ByVal Qta_Extra As Integer = 0
                                                  ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Movimento_Dettaglio")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("piva"), Piva)
        xmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        xmlTxt.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        xmlTxt.SetAttribute(LCase("ID_Mov_Det"), CStr(ID_Mov_Det))
        xmlTxt.SetAttribute(LCase("Elem_Cod"), CStr(Elem_Cod))
        xmlTxt.SetAttribute(LCase("Pro_Cod"), CStr(Pro_Cod))
        xmlTxt.SetAttribute(LCase("Mat_Cod"), CStr(Mat_Cod))
        xmlTxt.SetAttribute(LCase("Mov_Det_Des"), Mov_Det_Des)
        xmlTxt.SetAttribute(LCase("Udm_Cod"), CStr(Udm_Cod))
        xmlTxt.SetAttribute(LCase("Extra_Int"), CStr(Extra_Int))
        xmlTxt.SetAttribute(LCase("Qta"), CStr(Qta))
        xmlTxt.SetAttribute(LCase("Cod_IVA"), CStr(Cod_IVA))
        xmlTxt.SetAttribute(LCase("Sconto"), CStr(Sconto))
        xmlTxt.SetAttribute(LCase("Prezzo_Unitario"), CStr(Prezzo_Unitario))
        xmlTxt.SetAttribute(LCase("Cod_Conto"), CStr(Cod_Conto))
        xmlTxt.SetAttribute(LCase("Cod_Progetto"), CStr(Cod_Progetto))
        xmlTxt.SetAttribute(LCase("Fase_Cod"), CStr(Fase_Cod))
        xmlTxt.SetAttribute(LCase("Contabilizzato"), CStr(Contabilizzato))
        xmlTxt.SetAttribute(LCase("Pendente"), CStr(Pendente))
        xmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))
        xmlTxt.SetAttribute(LCase("ID_Destinazione"), CStr(ID_Destinazione))
        xmlTxt.SetAttribute(LCase("Cau_Mov"), Cau_Mov)
        xmlTxt.SetAttribute(LCase("Old_Qta"), CStr(Old_Qta))
        xmlTxt.SetAttribute(LCase("Old_Prezzo_Unitario"), CStr(Old_Prezzo_Unitario))
        xmlTxt.SetAttribute(LCase("Cal_Cod"), CStr(Cal_Cod))
        xmlTxt.SetAttribute(LCase("Lotto"), CStr(Lotto))
        xmlTxt.SetAttribute(LCase("Anno"), CInt(Anno))
        xmlTxt.SetAttribute(LCase("udm_cod_extra"), CInt(Udm_Cod_Extra))
        xmlTxt.SetAttribute(LCase("qta_extra"), CInt(Qta_Extra))



        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function



    '##########################################################################################
    'OVERLOADED
    'NUOVA: XML_2_Agenda_MovimentoDettaglio

    'Nuova versione per creazione xml per la nuova struttura
    'della tabella Movimenti_Dettagli (usata x contabilità) 
    'la FormProdotto (che gestisce il magazzino) ce l'ha all'interno del codice (NOME: XML_GeneraMovimentiDettagli) perché deve gestire vari casi
    'comment by Maga
    Public Function XML_Agenda_MovimentoDettaglio_New(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                      Optional ByVal Piva As String = "00000000000",
                                                      Optional ByVal Sa_Cod As Integer = 0,
                                                      Optional ByVal ID_Agenda As Integer = 0,
                                                      Optional ByVal ID_Mov As Integer = 0,
                                                      Optional ByVal ID_Mov_Det As Integer = 0,
                                                      Optional ByVal ID_Destinazione As Integer = 0,
                                                      Optional ByVal Mov_Det_Des As String = "",
                                                      Optional ByVal Cau_Mov As String = "",
                                                      Optional ByVal Elem_Cod As Integer = 0,
                                                      Optional ByVal Pro_Cod As Integer = 0,
                                                      Optional ByVal Mat_Cod As Integer = 0,
                                                      Optional ByVal Cod_Progetto As String = "",
                                                      Optional ByVal Cal_Cod As Integer = 0,
                                                      Optional ByVal Lotto As String = "",
                                                      Optional ByVal Udm_Cod As Integer = 0,
                                                      Optional ByVal Udm_Cod_Extra As Integer = 0,
                                                      Optional ByVal Qta_Extra As Decimal = 0.0,
                                                      Optional ByVal Qta As Decimal = 0.0,
                                                      Optional ByVal Contabilizzato As Integer = CONTABILE,
                                                      Optional ByVal Pendente As Integer = enum_Pendenza.MovESENTE,
                                                      Optional ByVal Jolly_Int As Integer = MagazzinoMovimentato,
                                                      Optional ByVal Extra_Int As Integer = 0,
                                                      Optional ByVal Extra_Str As String = "",
                                                      Optional ByVal Extra_Date As Date = #1/1/1900#,
                                                      Optional ByVal Cod_IVA As Integer = 0,
                                                      Optional ByVal IVA As Decimal = 0.0,
                                                      Optional ByVal Sconto As Decimal = 0,
                                                      Optional ByVal Tipo_Sconto As Integer = 0,
                                                      Optional ByVal Prezzo_Unitario As Decimal = 0.0,
                                                      Optional ByVal Prezzo_Unitario_Netto As Decimal = 0.0,
                                                      Optional ByVal Imponibile As Decimal = 0.0,
                                                      Optional ByVal Imponibile_Netto As Decimal = 0.0,
                                                      Optional ByVal Cod_Conto As Integer = 0,
                                                      Optional ByVal Ric_Cod As Integer = 0,
                                                      Optional ByVal Anno As Integer = 1900,
                                                      Optional ByVal Fase_Cod As Integer = 0,
                                                      Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                                      Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                                      Optional ByVal BaseCode As Integer = 0,
                                                      Optional ByVal TopCode As Integer = 200000000
                                                      ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Movimento_Dettaglio")

        'Imposto gli attributi

        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        xmlTxt.SetAttribute(LCase("piva"), Piva)
        xmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        xmlTxt.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        xmlTxt.SetAttribute(LCase("ID_Mov_Det"), CStr(ID_Mov_Det))
        xmlTxt.SetAttribute(LCase("ID_Destinazione"), CStr(ID_Destinazione))

        xmlTxt.SetAttribute(LCase("Mov_Det_Des"), Mov_Det_Des)
        xmlTxt.SetAttribute(LCase("Cau_Mov"), Cau_Mov)

        xmlTxt.SetAttribute(LCase("Elem_Cod"), CStr(Elem_Cod))
        xmlTxt.SetAttribute(LCase("Pro_Cod"), CStr(Pro_Cod))
        xmlTxt.SetAttribute(LCase("Mat_Cod"), CStr(Mat_Cod))
        xmlTxt.SetAttribute(LCase("Cod_Progetto"), CStr(Cod_Progetto))
        xmlTxt.SetAttribute(LCase("Cal_Cod"), CStr(Cal_Cod))
        xmlTxt.SetAttribute(LCase("Lotto"), CStr(Lotto))
        xmlTxt.SetAttribute(LCase("Udm_Cod"), CStr(Udm_Cod))

        xmlTxt.SetAttribute(LCase("udm_cod_extra"), CInt(Udm_Cod_Extra))
        xmlTxt.SetAttribute(LCase("qta_extra"), CInt(Qta_Extra))

        xmlTxt.SetAttribute(LCase("Qta"), CStr(Qta))

        xmlTxt.SetAttribute(LCase("Contabilizzato"), CStr(Contabilizzato))
        xmlTxt.SetAttribute(LCase("Pendente"), CStr(Pendente))
        xmlTxt.SetAttribute(LCase("jolly_Int"), CStr(Jolly_Int))

        xmlTxt.SetAttribute(LCase("Extra_Int"), CStr(Extra_Int))
        xmlTxt.SetAttribute(LCase("Extra_Str"), CStr(Extra_Str))
        xmlTxt.SetAttribute(LCase("Extra_Date"), CStr(Extra_Date))

        xmlTxt.SetAttribute(LCase("Cod_IVA"), CStr(Cod_IVA))
        xmlTxt.SetAttribute(LCase("Iva"), CStr(IVA))
        xmlTxt.SetAttribute(LCase("Sconto"), CStr(Sconto))
        xmlTxt.SetAttribute(LCase("Tipo_Sconto"), CStr(Tipo_Sconto))
        xmlTxt.SetAttribute(LCase("Prezzo_Unitario"), CStr(Prezzo_Unitario))
        xmlTxt.SetAttribute(LCase("Prezzo_Unitario_Netto"), CStr(Prezzo_Unitario_Netto))
        xmlTxt.SetAttribute(LCase("Imponibile"), CStr(Imponibile))
        xmlTxt.SetAttribute(LCase("Imponibile_Netto"), CStr(Imponibile_Netto))

        xmlTxt.SetAttribute(LCase("Cod_Conto"), CStr(Cod_Conto))
        xmlTxt.SetAttribute(LCase("Ric_Cod"), CStr(Ric_Cod))
        xmlTxt.SetAttribute(LCase("Anno"), CInt(Anno))

        xmlTxt.SetAttribute(LCase("Fase_Cod"), CStr(Fase_Cod))

        xmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))

        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function


    '###############################################################################################
    Public Function Estrai_XML_CostiAccessori(ByVal strXml As String, ByVal Elem_Cod As Integer) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlDocCA As New XmlDocument

        Dim XML_Movimento As XmlElement
        Dim XMLs_Movimento As XmlNodeList
        Dim XML_DatiMovimentiDettagli As XmlElement
        Dim XML_MovimentoDettaglio As XmlElement
        Dim XMLs_MovimentoDettaglio As XmlNodeList
        Dim xmlDatiMovimentiCA As XmlElement
        Dim xmlMovimentoCA As XmlElement
        Dim xmlErr As XmlElement
        Dim xmlMovimentoDettaglioCA As XmlElement
        Dim xmlDatiMovimentiDettagliCA As XmlElement

        Dim appendiMovimento As Boolean

        Dim i As Integer

        Try

            appendiMovimento = False

            strXml = strXml.Replace("TipoOperazioneDB=" & Chr(34) & "0" & Chr(34), "TipoOperazioneDB=" & Chr(34) & "1" & Chr(34))

            'Carico la stringa xml in un nuovo documento
            xmlDoc = New XmlDocument
            xmlDoc.LoadXml(strXml)

            'Creo un nuovo documento per i costi accessori
            xmlDocCA = New XmlDocument
            xmlDatiMovimentiCA = xmlDocCA.CreateElement("DatiMovimenti")


            'Recupero l'insieme dei nodi Movimento
            XMLs_Movimento = xmlDoc.GetElementsByTagName("Movimento")

            For Each XML_Movimento In XMLs_Movimento

                xmlMovimentoCA = xmlDocCA.ImportNode(XML_Movimento, True)
                For i = 0 To xmlMovimentoCA.ChildNodes.Count - 1

                    xmlMovimentoCA.RemoveChild(xmlMovimentoCA.FirstChild)

                Next
                xmlDatiMovimentiDettagliCA = xmlDocCA.CreateElement("DatiMovimenti_Dettagli")

                If XML_Movimento.GetAttribute("cau_mov") = CStr(enum_Agenda_Causali.SCARICO) Then

                    '----- Tag DatiMovimenti_Dettagli

                    If XML_Movimento.HasChildNodes Then

                        XML_DatiMovimentiDettagli = XML_Movimento.SelectSingleNode("DatiMovimenti_Dettagli")

                        '----- Tag Movimento_Dettaglio  (multiplo)

                        'Recupero la collezione dei nodi
                        XMLs_MovimentoDettaglio = XML_DatiMovimentiDettagli.GetElementsByTagName("Movimento_Dettaglio")


                        'Ciclo su tutti i nodi
                        For i = 0 To XMLs_MovimentoDettaglio.Count - 1

                            'Prendo l'i-esimo nodo della collezione
                            XML_MovimentoDettaglio = XMLs_MovimentoDettaglio.Item(i)

                            Select Case Elem_Cod

                                Case TRAPPOLE

                                    If XML_MovimentoDettaglio.GetAttribute("elem_cod") <> Elem_Cod AndAlso
                                       XML_MovimentoDettaglio.GetAttribute("elem_cod") <> INNESCHI Then

                                        'Ho un movimento di scarico da magazzino relativo ai costi accessori
                                        appendiMovimento = True

                                        'Attacco il nodo MovimentoDettaglio al nodo Movimento
                                        xmlMovimentoDettaglioCA = xmlDocCA.ImportNode(XML_MovimentoDettaglio, True)
                                        xmlDatiMovimentiDettagliCA.AppendChild(xmlMovimentoDettaglioCA)

                                    End If

                                Case 301 'CONSISTENZE



                                Case Else

                                    If XML_MovimentoDettaglio.GetAttribute("elem_cod") <> Elem_Cod Then

                                        'Ho un movimento di scarico da magazzino relativo ai costi accessori
                                        appendiMovimento = True

                                        'Attacco il nodo MovimentoDettaglio al nodo Movimento
                                        xmlMovimentoDettaglioCA = xmlDocCA.ImportNode(XML_MovimentoDettaglio, True)
                                        xmlDatiMovimentiDettagliCA.AppendChild(xmlMovimentoDettaglioCA)

                                    End If

                            End Select

                        Next

                        If appendiMovimento Then

                            xmlMovimentoCA.AppendChild(xmlDatiMovimentiDettagliCA)
                            xmlDatiMovimentiCA.AppendChild(xmlMovimentoCA)

                        End If

                        appendiMovimento = False

                    End If

                    'Se ho un movimento di imputazione costi: manodopera, parco macchine, contoterzisti lo aggancio al nodo dei costi accessori
                ElseIf XML_Movimento.GetAttribute("cau_mov") = CStr(enum_Agenda_Causali.IMPUTAZIONE_MANODOPERA) OrElse
                       XML_Movimento.GetAttribute("cau_mov") = CStr(enum_Agenda_Causali.IMPUTAZIONE_PARCOMACCHINE) OrElse
                       XML_Movimento.GetAttribute("cau_mov") = CStr(enum_Agenda_Causali.IMPUTAZIONE_TERZISTI) Then

                    'Attacco il nodo Movimento ai costi Accessori
                    xmlMovimentoCA = xmlDocCA.ImportNode(XML_Movimento, True)
                    xmlDatiMovimentiCA.AppendChild(xmlMovimentoCA)

                End If

            Next  'ciclo dei movimenti

            xmlDocCA.AppendChild(xmlDatiMovimentiCA)

        Catch ex As Exception
            xmlDocCA = New XmlDocument
            xmlErr = xmlDocCA.CreateElement("NodoErrore")
            xmlErr.SetAttribute("Message", ex.Message)
            xmlDocCA.AppendChild(xmlErr)
        End Try

        If xmlDocCA.OuterXml = "<DatiMovimenti />" Then
            xmlDocCA = New XmlDocument
            xmlErr = xmlDocCA.CreateElement("NodoErrore")
            xmlErr.SetAttribute("Message", "nessun costo accessorio")
            xmlDocCA.AppendChild(xmlErr)
        End If

        Return xmlDocCA.OuterXml

    End Function


    '##########################################################################################
    Public Function XML_RapportoContabile(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                          Optional ByVal Piva As String = "00000000000",
                                          Optional ByVal Sa_Cod As Integer = 0,
                                          Optional ByVal Cod_Rapporto As Integer = 0,
                                          Optional ByVal Rapporto_Des As String = "",
                                          Optional ByVal Cliente As Integer = 0,
                                          Optional ByVal Fornitore As Integer = 0,
                                          Optional ByVal Dipendente As Integer = 0,
                                          Optional ByVal Terzista As Integer = 0,
                                          Optional ByVal Legale As Integer = 0,
                                          Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                          Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                          Optional ByVal BaseCode As Integer = 0,
                                          Optional ByVal TopCode As Integer = 200000000
                                          ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("RapCon")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("piva"), CStr(Piva))
        xmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("cod_rapporto"), CStr(Cod_Rapporto))
        xmlTxt.SetAttribute(LCase("rapporto_des"), CStr(Rapporto_Des))
        xmlTxt.SetAttribute(LCase("cliente"), CStr(Cliente))
        xmlTxt.SetAttribute(LCase("fornitore"), CStr(Fornitore))
        xmlTxt.SetAttribute(LCase("dipendente"), CStr(Dipendente))
        xmlTxt.SetAttribute(LCase("terzista"), CStr(Terzista))
        xmlTxt.SetAttribute(LCase("legale"), CStr(Legale))
        xmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function


    '##########################################################################################
    Public Function XML_Contatto(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                 Optional ByVal Piva As String = "00000000000",
                                 Optional ByVal Sa_Cod As Integer = 0,
                                 Optional ByVal Cod_Contatto As String = "",
                                 Optional ByVal Rag_Soc As String = "",
                                 Optional ByVal Id_CF As Integer = 0,
                                 Optional ByVal Codice_Fiscale As String = "",
                                 Optional ByVal Convenevoli As String = "",
                                 Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                 Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                 Optional ByVal BaseCode As Integer = 0,
                                 Optional ByVal TopCode As Integer = 200000000
                                 ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Contatto")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("piva"), CStr(Piva))
        xmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("cod_contatto"), CStr(Cod_Contatto))
        xmlTxt.SetAttribute(LCase("rag_soc"), CStr(Rag_Soc))
        xmlTxt.SetAttribute(LCase("id_cf"), CStr(Id_CF))
        xmlTxt.SetAttribute(LCase("codice_fiscale"), CStr(Codice_Fiscale))
        xmlTxt.SetAttribute(LCase("convenevoli"), CStr(Convenevoli))
        xmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function


    '##########################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Nuova versione con in più nome, cognome, data di nascita e sesso.
    ''' </summary>
    ''' <param name="TipoOperazioneDB"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Cod_Contatto"></param>
    ''' <param name="Cognome"></param>
    ''' <param name="Nome"></param>
    ''' <param name="DataNascita"></param>
    ''' <param name="Sesso"></param>
    ''' <param name="Rag_Soc"></param>
    ''' <param name="Id_CF"></param>
    ''' <param name="Codice_Fiscale"></param>
    ''' <param name="Convenevoli"></param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="BaseCode"></param>
    ''' <param name="TopCode"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	21/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function XML_Contatto2(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                  Optional ByVal Piva As String = "00000000000",
                                  Optional ByVal Sa_Cod As Integer = 0,
                                  Optional ByVal Cod_Contatto As String = "",
                                  Optional ByVal Cognome As String = "",
                                  Optional ByVal Nome As String = "",
                                  Optional ByVal DataNascita As String = "",
                                  Optional ByVal Sesso As String = "",
                                  Optional ByVal Rag_Soc As String = "",
                                  Optional ByVal Id_CF As Integer = 0,
                                  Optional ByVal Codice_Fiscale As String = "",
                                  Optional ByVal Convenevoli As String = "",
                                  Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                  Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                  Optional ByVal BaseCode As Integer = 0,
                                  Optional ByVal TopCode As Integer = 200000000
                                  ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Contatto")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("piva"), CStr(Piva))
        xmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("cod_contatto"), CStr(Cod_Contatto))
        xmlTxt.SetAttribute(LCase("cognome"), CStr(Cognome))
        xmlTxt.SetAttribute(LCase("nome"), CStr(Nome))
        xmlTxt.SetAttribute(LCase("data_nascita"), Format(DataNascita, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("sesso"), CStr(Sesso))
        xmlTxt.SetAttribute(LCase("rag_soc"), CStr(Rag_Soc))
        xmlTxt.SetAttribute(LCase("id_cf"), CStr(Id_CF))
        xmlTxt.SetAttribute(LCase("codice_fiscale"), CStr(Codice_Fiscale))
        xmlTxt.SetAttribute(LCase("convenevoli"), CStr(Convenevoli))
        xmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function



    '##########################################################################################
    Public Function XML_RapportoContabileXRisorseUmane(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                       Optional ByVal Piva As String = "00000000000",
                                                       Optional ByVal Sa_Cod As Integer = 0,
                                                       Optional ByVal Cod_Risum As Integer = - 1,
                                                       Optional ByVal Cod_Contatto As String = "",
                                                       Optional ByVal Cod_Rapporto As Integer = 0,
                                                       Optional ByVal Attivita_Des As String = "",
                                                       Optional ByVal Settore_Des As String = "",
                                                       Optional ByVal Occasionale As Integer = 0,
                                                       Optional ByVal Corrispettivo_Mensile As Decimal = 0,
                                                       Optional ByVal Corrispettivo_Orario As Decimal = 0,
                                                       Optional ByVal Ore_Settimanali As Decimal = 0,
                                                       Optional ByVal Giorni_Ferie As Integer = 0,
                                                       Optional ByVal Ferie_Godute As Integer = 0,
                                                       Optional ByVal Giorni_Malattia As Integer = 0,
                                                       Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                                       Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                                       Optional ByVal Patentino As String = "",
                                                       Optional ByVal Data_Rilascio_Patentino As Date = #1/1/1900#,
                                                       Optional ByVal Data_Scadenza_Patentino As Date = #12/31/2100#
                                                       ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("RapCon")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("piva"), CStr(Piva))
        xmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("cod_risum"), CStr(Cod_Risum))
        xmlTxt.SetAttribute(LCase("cod_contatto"), CStr(Cod_Contatto))
        xmlTxt.SetAttribute(LCase("cod_rapporto"), CStr(Cod_Rapporto))
        xmlTxt.SetAttribute(LCase("attivita_des"), CStr(Attivita_Des))
        xmlTxt.SetAttribute(LCase("settore_des"), CStr(Settore_Des))
        xmlTxt.SetAttribute(LCase("occasionale"), CStr(Occasionale))
        xmlTxt.SetAttribute(LCase("corrispettivo_mensile"), CStr(Corrispettivo_Mensile))
        xmlTxt.SetAttribute(LCase("corrispettivo_orario"), CStr(Corrispettivo_Orario))
        xmlTxt.SetAttribute(LCase("ore_settimanali"), CStr(Ore_Settimanali))
        xmlTxt.SetAttribute(LCase("giorni_ferie"), CStr(Giorni_Ferie))
        xmlTxt.SetAttribute(LCase("ferie_godute"), CStr(Ferie_Godute))
        xmlTxt.SetAttribute(LCase("giorni_malattia"), CStr(Giorni_Malattia))
        xmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("patentino"), CStr(Patentino))
        xmlTxt.SetAttribute(LCase("data_rilascio_patentino"), Format(Data_Rilascio_Patentino, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("data_scadenza_patentino"), Format(Data_Scadenza_Patentino, "dd/MM/yyyy"))


        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function

    '##########################################################################################
    Public Function XML_RisorsaUmana(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                     Optional ByVal Piva As String = "00000000000",
                                     Optional ByVal Sa_Cod As Integer = 0,
                                     Optional ByVal Cod_Risum As Integer = - 1,
                                     Optional ByVal Cod_Contatto As String = "",
                                     Optional ByVal Cod_Rapporto As Integer = 0,
                                     Optional ByVal Attivita_Des As String = "",
                                     Optional ByVal Settore_Des As String = "",
                                     Optional ByVal Occasionale As Integer = 0,
                                     Optional ByVal Corrispettivo_Mensile As Decimal = 0,
                                     Optional ByVal Corrispettivo_Orario As Decimal = 0,
                                     Optional ByVal Ore_Settimanali As Decimal = 0,
                                     Optional ByVal Giorni_Ferie As Integer = 0,
                                     Optional ByVal Ferie_Godute As Integer = 0,
                                     Optional ByVal Giorni_Malattia As Integer = 0,
                                     Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                     Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                     Optional ByVal Patentino As String = "",
                                     Optional ByVal Data_Rilascio_Patentino As Date = #1/1/1900#,
                                     Optional ByVal Data_Scadenza_Patentino As Date = #12/31/2100#
                                     ) As XmlElement

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("RapCon")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("piva"), CStr(Piva))
        xmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("cod_risum"), CStr(Cod_Risum))
        xmlTxt.SetAttribute(LCase("cod_contatto"), CStr(Cod_Contatto))
        xmlTxt.SetAttribute(LCase("cod_rapporto"), CStr(Cod_Rapporto))
        xmlTxt.SetAttribute(LCase("attivita_des"), CStr(Attivita_Des))
        xmlTxt.SetAttribute(LCase("settore_des"), CStr(Settore_Des))
        xmlTxt.SetAttribute(LCase("occasionale"), CStr(Occasionale))
        xmlTxt.SetAttribute(LCase("corrispettivo_mensile"), CStr(Corrispettivo_Mensile))
        xmlTxt.SetAttribute(LCase("corrispettivo_orario"), CStr(Corrispettivo_Orario))
        xmlTxt.SetAttribute(LCase("ore_settimanali"), CStr(Ore_Settimanali))
        xmlTxt.SetAttribute(LCase("giorni_ferie"), CStr(Giorni_Ferie))
        xmlTxt.SetAttribute(LCase("ferie_godute"), CStr(Ferie_Godute))
        xmlTxt.SetAttribute(LCase("giorni_malattia"), CStr(Giorni_Malattia))
        xmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("patentino"), CStr(Patentino))
        xmlTxt.SetAttribute(LCase("data_rilascio_patentino"), Format(Data_Rilascio_Patentino, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("data_scadenza_patentino"), Format(Data_Scadenza_Patentino, "dd/MM/yyyy"))


        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlTxt

    End Function

    '##########################################################################################
    'OVERLOADED
    'NUOVA: XML_2_Agenda_Movimento
    Public Function XML_Agenda_Movimento_Contabile(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                   Optional ByVal Piva As String = "00000000000",
                                                   Optional ByVal Sa_Cod As Integer = 0,
                                                   Optional ByVal ID_Agenda As Integer = 0,
                                                   Optional ByVal ID_Mov As Integer = 0,
                                                   Optional ByVal Cau_Mov As String = "",
                                                   Optional ByVal Mov_Desc As String = "",
                                                   Optional ByVal Data_Movimento As Date = #1/1/1900#,
                                                   Optional ByVal Scadenza As Date = #12/31/2100#,
                                                   Optional ByVal Doc_Numero As Integer = 0,
                                                   Optional ByVal Num_Protocollo As Decimal = 0.0,
                                                   Optional ByVal Mezzo As Integer = 0,
                                                   Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                                   Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                                   Optional ByVal Aspetto As String = "",
                                                   Optional ByVal Causale_Trasporto As String = "",
                                                   Optional ByVal Ora As String = "00.00",
                                                   Optional ByVal Peso As Decimal = 0.0,
                                                   Optional ByVal Colli As Integer = 0,
                                                   Optional ByVal Cod_RisUm As Integer = 0,
                                                   Optional ByVal Cod_IndirizzoRisUm As Integer = 0,
                                                   Optional ByVal Cod_Destinazione As Integer = 0,
                                                   Optional ByVal Cod_IndirizzoDestinazione As Integer = 0,
                                                   Optional ByVal Cod_Vettore As Integer = 0,
                                                   Optional ByVal Cod_IndirizzoVettore As Integer = 0,
                                                   Optional ByVal BaseCode As Integer = 0,
                                                   Optional ByVal TopCode As Integer = 200000000
                                                   ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Movimento")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("piva"), Piva)
        xmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("id_agenda"), CStr(ID_Agenda))
        xmlTxt.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        xmlTxt.SetAttribute(LCase("Cod_RisUm"), CStr(Cod_RisUm))
        xmlTxt.SetAttribute(LCase("Cau_Mov"), Cau_Mov)
        xmlTxt.SetAttribute(LCase("Mov_Desc"), Mov_Desc)
        xmlTxt.SetAttribute(LCase("Data_Movimento"), Format(Data_Movimento, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Scadenza"), Format(Scadenza, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Doc_Numero"), CStr(Doc_Numero))
        xmlTxt.SetAttribute(LCase("Num_Protocollo"), CStr(Num_Protocollo))
        xmlTxt.SetAttribute(LCase("Mezzo"), CStr(Mezzo))
        xmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))
        xmlTxt.SetAttribute(LCase("ora"), CStr(Ora))
        xmlTxt.SetAttribute(LCase("Aspetto"), CStr(Aspetto))
        xmlTxt.SetAttribute(LCase("Causale_Trasporto"), CStr(Causale_Trasporto))
        xmlTxt.SetAttribute(LCase("peso"), CStr(Peso))
        xmlTxt.SetAttribute(LCase("colli"), CStr(Colli))
        xmlTxt.SetAttribute(LCase("Cod_IndirizzoRisUm"), CStr(Cod_IndirizzoRisUm))
        xmlTxt.SetAttribute(LCase("Cod_Destinazione"), CStr(Cod_Destinazione))
        xmlTxt.SetAttribute(LCase("Cod_IndirizzoDestinazione"), CStr(Cod_IndirizzoDestinazione))
        xmlTxt.SetAttribute(LCase("Cod_Vettore"), CStr(Cod_Vettore))
        xmlTxt.SetAttribute(LCase("Cod_IndirizzoVettore"), CStr(Cod_IndirizzoVettore))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function


    '##########################################################################################
    Public Function XML_Agenda_MovimentoRiferimento(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                    Optional ByVal Piva As String = "00000000000",
                                                    Optional ByVal Sa_Cod As Integer = 0,
                                                    Optional ByVal ID_Agenda As Integer = 0,
                                                    Optional ByVal ID_Mov As Integer = 0,
                                                    Optional ByVal ID_Mov_Det As Integer = 0,
                                                    Optional ByVal ID_Agenda_Rif As Integer = 0,
                                                    Optional ByVal ID_Mov_Rif As Integer = 0,
                                                    Optional ByVal ID_Mov_Det_Rif As Integer = 0,
                                                    Optional ByVal Cau_Mov_Rif As String = "") As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Movimento_Riferimento2")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("Piva"), CStr(Piva))
        xmlTxt.SetAttribute(LCase("Sa_Cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("Id_Agenda"), CStr(ID_Agenda))
        xmlTxt.SetAttribute(LCase("ID_Mov"), CStr(ID_Mov))
        xmlTxt.SetAttribute(LCase("ID_Mov_Det"), CStr(ID_Mov_Det))
        xmlTxt.SetAttribute(LCase("Id_Agenda_Rif"), CStr(ID_Agenda_Rif))
        xmlTxt.SetAttribute(LCase("ID_Mov_Rif"), CStr(ID_Mov_Rif))
        xmlTxt.SetAttribute(LCase("ID_Mov_Det_Rif"), CStr(ID_Mov_Det_Rif))
        xmlTxt.SetAttribute(LCase("Cau_Mov_Rif"), CStr(Cau_Mov_Rif))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function


    '##########################################################################################
    Public Function XML_Prodotto_Costo(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                       Optional ByVal Piva As String = "00000000000",
                                       Optional ByVal Riferimento As String = "",
                                       Optional ByVal Elem_Cod As Integer = 0,
                                       Optional ByVal Pro_Cod As Integer = 0,
                                       Optional ByVal Mat_Cod As Integer = 0,
                                       Optional ByVal Udm_Cod As Integer = 0,
                                       Optional ByVal Mezzo As Integer = 0,
                                       Optional ByVal Prezzo_Unitario As Decimal = 0.0,
                                       Optional ByVal Veg_Cod As Integer = 0,
                                       Optional ByVal Cul_Cod As Integer = 0,
                                       Optional ByRef Validita_Inizio As Date = #1/1/1900#,
                                       Optional ByRef Validita_Fine As Date = #12/31/2100#
                                       ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Prodotto_Costo")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("Piva"), CStr(Piva))
        xmlTxt.SetAttribute(LCase("Riferimento"), CStr(Riferimento))
        xmlTxt.SetAttribute(LCase("Elem_Cod"), CStr(Elem_Cod))
        xmlTxt.SetAttribute(LCase("Pro_Cod"), CStr(Pro_Cod))
        xmlTxt.SetAttribute(LCase("Mat_Cod"), CStr(Mat_Cod))
        xmlTxt.SetAttribute(LCase("Udm_Cod"), CStr(Udm_Cod))
        xmlTxt.SetAttribute(LCase("Mezzo"), CStr(Mezzo))
        xmlTxt.SetAttribute(LCase("Prezzo_Unitario"), CStr(Prezzo_Unitario))
        xmlTxt.SetAttribute(LCase("Veg_Cod"), CStr(Veg_Cod))
        xmlTxt.SetAttribute(LCase("Cul_Cod"), CStr(Cul_Cod))
        xmlTxt.SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
        xmlTxt.SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function


    '##########################################################################################
    Public Function XML_Analisi_Certificato(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                            ByVal Analisi_SuperUser As String,
                                            Optional ByVal Analisi_Certificato_Cod As Integer = 0,
                                            Optional ByVal Analisi_Certificato_Des As String = "",
                                            Optional ByVal Analisi_Certificato_Data_Inizio As Date = #1/1/1900#,
                                            Optional ByVal Analisi_Certificato_Data_Fine As Date = #12/31/2100#,
                                            Optional ByVal Analisi_Certificato_Laboratorio As String = "",
                                            Optional ByVal Analisi_Certificato_TipoCampione As String = "",
                                            Optional ByVal Analisi_Certificato_Provenienza As String = "",
                                            Optional ByVal Analisi_Certificato_Verbale As String = "",
                                            Optional ByVal Analisi_Certificato_Richiedente As String = "",
                                            Optional ByVal Analisi_Certificato_PrelevatoDa As String = "",
                                            Optional ByVal Analisi_Certificato_Comune As String = "",
                                            Optional ByVal Analisi_Certificato_Protocollo As String = "",
                                            Optional ByVal Analisi_Certificato_NumRegistro As String = "",
                                            Optional ByVal Analisi_Certificato_Sezione As String = "",
                                            Optional ByVal Analisi_Certificato_Responsabile As String = "",
                                            Optional ByVal Analisi_Certificato_Analista As String = "",
                                            Optional ByVal Analisi_Certificato_Data_Firma As Date = #1/1/1900#,
                                            Optional ByVal Analisi_Certificato_Validita_Inizio As Date = #1/1/1900#,
                                            Optional ByVal Analisi_Certificato_Validita_Fine As Date = #12/31/2100#,
                                            Optional ByVal BaseCode As Integer = 0,
                                            Optional ByVal TopCode As Integer = 200000000
                                            ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Certificato")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("Analisi_SuperUser"), Analisi_SuperUser)
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Cod"), CStr(Analisi_Certificato_Cod))
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Des"), Analisi_Certificato_Des)
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Data_Inizio"), Format(Analisi_Certificato_Data_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Data_Fine"), Format(Analisi_Certificato_Data_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Laboratorio"), Analisi_Certificato_Laboratorio)
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_TipoCampione"), Analisi_Certificato_TipoCampione)
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Provenienza"), Analisi_Certificato_Provenienza)
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Verbale"), Analisi_Certificato_Verbale)
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Richiedente"), Analisi_Certificato_Richiedente)
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_PrelevatoDa"), Analisi_Certificato_PrelevatoDa)
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Comune"), Analisi_Certificato_Comune)
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Protocollo"), Analisi_Certificato_Protocollo)
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_NumRegistro"), Analisi_Certificato_NumRegistro)
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Sezione"), Analisi_Certificato_Sezione)
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Responsabile"), Analisi_Certificato_Responsabile)
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Analista"), Analisi_Certificato_Analista)
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_DataFirma"), Format(Analisi_Certificato_Data_Firma, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Validita_Inizio"), Format(Analisi_Certificato_Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Validita_Fine"), Format(Analisi_Certificato_Validita_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function

    '##########################################################################################
    Public Function XML_Analisi_EntitaxTestata(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                               ByVal Analisi_SuperUser As String,
                                               Optional ByVal Analisi_Testata_Cod As Integer = 0,
                                               Optional ByVal Analisi_Entita_Cod As Integer = 0,
                                               Optional ByVal Piva As String = "",
                                               Optional ByVal Sa_Cod As Integer = 0,
                                               Optional ByVal Campo_Cod As Integer = 0,
                                               Optional ByVal Appezza As Integer = 0,
                                               Optional ByVal Id_Imp As Integer = 0,
                                               Optional ByVal Fabbricato_Cod As Integer = 0,
                                               Optional ByVal Prov As String = "",
                                               Optional ByVal Com As String = "",
                                               Optional ByVal Sezione As String = "0",
                                               Optional ByVal Foglio As Integer = 0,
                                               Optional ByVal Numero As Integer = 0,
                                               Optional ByVal Subalterno As String = "0",
                                               Optional ByVal ID_Oggetto_Grafico As String = "0",
                                               Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                               Optional ByVal Validita_Fine As Date = #12/31/2100#
                                               ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Analisi_EntitaxTestata")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("Analisi_SuperUser"), Analisi_SuperUser)
        xmlTxt.SetAttribute(LCase("Analisi_Testata_Cod"), CStr(Analisi_Testata_Cod))
        xmlTxt.SetAttribute(LCase("Analisi_Entita_Cod"), CStr(Analisi_Entita_Cod))
        xmlTxt.SetAttribute(LCase("Piva"), Piva)
        xmlTxt.SetAttribute(LCase("Sa_Cod"), CStr(Sa_Cod))
        xmlTxt.SetAttribute(LCase("Campo_Cod"), CStr(Campo_Cod))
        xmlTxt.SetAttribute(LCase("Appezza"), CStr(Appezza))
        xmlTxt.SetAttribute(LCase("Id_Imp"), CStr(Id_Imp))
        xmlTxt.SetAttribute(LCase("Fabbricato_Cod"), CStr(Fabbricato_Cod))
        xmlTxt.SetAttribute(LCase("Prov"), Prov)
        xmlTxt.SetAttribute(LCase("Com"), Com)
        xmlTxt.SetAttribute(LCase("Sezione"), Sezione)
        xmlTxt.SetAttribute(LCase("Foglio"), CStr(Foglio))
        xmlTxt.SetAttribute(LCase("Numero"), CStr(Numero))
        xmlTxt.SetAttribute(LCase("Subalterno"), Subalterno)
        xmlTxt.SetAttribute(LCase("ID_Oggetto_Grafico"), ID_Oggetto_Grafico)
        xmlTxt.SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function


    '##########################################################################################
    Public Function XML_Analisi_Testata(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                        ByVal Analisi_SuperUser As String,
                                        Optional ByVal Analisi_Certificato_Cod As Integer = 0,
                                        Optional ByVal Analisi_Testata_Cod As Integer = 0,
                                        Optional ByVal Analisi_Testata_Des As String = "",
                                        Optional ByVal Analisi_Testata_Data_Inizio As Date = #1/1/1900#,
                                        Optional ByVal Analisi_Testata_Data_Fine As Date = #12/31/2100#,
                                        Optional ByVal Analisi_Testata_Coord_X As Decimal = 0,
                                        Optional ByVal Analisi_Testata_Coord_Y As Decimal = 0,
                                        Optional ByVal Analisi_Testata_Riferimento_1 As String = "",
                                        Optional ByVal Analisi_Testata_Riferimento_2 As String = "",
                                        Optional ByVal Analisi_Testata_Riferimento_3 As String = "",
                                        Optional ByVal Analisi_Testata_Riferimento_4 As String = "",
                                        Optional ByVal Analisi_Testata_Riferimento_5 As String = "",
                                        Optional ByVal BaseCode As Integer = 0,
                                        Optional ByVal TopCode As Integer = 200000000
                                        ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Testata")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("Analisi_SuperUser"), Analisi_SuperUser)
        xmlTxt.SetAttribute(LCase("Analisi_Certificato_Cod"), CStr(Analisi_Certificato_Cod))
        xmlTxt.SetAttribute(LCase("Analisi_Testata_Cod"), CStr(Analisi_Testata_Cod))
        xmlTxt.SetAttribute(LCase("Analisi_Testata_Des"), Analisi_Testata_Des)
        xmlTxt.SetAttribute(LCase("Analisi_Testata_Data_Inizio"), Format(Analisi_Testata_Data_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Analisi_Testata_Data_Fine"), Format(Analisi_Testata_Data_Fine, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Analisi_Testata_Coord_X"), CStr(Analisi_Testata_Coord_X))
        xmlTxt.SetAttribute(LCase("Analisi_Testata_Coord_Y"), CStr(Analisi_Testata_Coord_Y))
        xmlTxt.SetAttribute(LCase("Analisi_Testata_Riferimento_1"), Analisi_Testata_Riferimento_1)
        xmlTxt.SetAttribute(LCase("Analisi_Testata_Riferimento_2"), Analisi_Testata_Riferimento_2)
        xmlTxt.SetAttribute(LCase("Analisi_Testata_Riferimento_3"), Analisi_Testata_Riferimento_3)
        xmlTxt.SetAttribute(LCase("Analisi_Testata_Riferimento_4"), Analisi_Testata_Riferimento_4)
        xmlTxt.SetAttribute(LCase("Analisi_Testata_Riferimento_5"), Analisi_Testata_Riferimento_5)
        xmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        xmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function

    '##########################################################################################
    Public Function XML_Analisi_Dettaglio( _
                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
                                        ByVal Analisi_SuperUser As String, _
                                        ByVal Analisi_Testata_Cod As Integer, _
                                        Optional ByVal Analisi_Dettaglio_Cod As String = "0", _
                                        Optional ByVal Analisi_Parametro_Cod As Integer = 1, _
                                        Optional ByVal Analisi_Dettaglio_Valore_1 As Decimal = 0, _
                                        Optional ByVal Analisi_Dettaglio_MargineErrore_1 As Decimal = 0, _
                                        Optional ByVal Analisi_Dettaglio_Valore_2 As Decimal = 0, _
                                        Optional ByVal Analisi_Dettaglio_MargineErrore_2 As Decimal = 0, _
                                        Optional ByVal Analisi_Dettaglio_Validita_Inizio As Date = #1/1/1900#, _
                                        Optional ByVal Analisi_Dettaglio_Validita_Fine As Date = #12/31/2100#) _
                                        As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Dettaglio")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("Analisi_SuperUser"), Analisi_SuperUser)
        xmlTxt.SetAttribute(LCase("Analisi_Testata_Cod"), CStr(Analisi_Testata_Cod))
        xmlTxt.SetAttribute(LCase("Analisi_Dettaglio_Cod"), CStr(Analisi_Dettaglio_Cod))
        xmlTxt.SetAttribute(LCase("Analisi_Parametro_Cod"), CStr(Analisi_Parametro_Cod))
        xmlTxt.SetAttribute(LCase("Analisi_Dettaglio_Valore_1"), CStr(Analisi_Dettaglio_Valore_1))
        xmlTxt.SetAttribute(LCase("Analisi_Dettaglio_MargineErrore_1"), CStr(Analisi_Dettaglio_MargineErrore_1))
        xmlTxt.SetAttribute(LCase("Analisi_Dettaglio_Valore_2"), CStr(Analisi_Dettaglio_Valore_2))
        xmlTxt.SetAttribute(LCase("Analisi_Dettaglio_MargineErrore_2"), CStr(Analisi_Dettaglio_MargineErrore_2))
        xmlTxt.SetAttribute(LCase("Analisi_Dettaglio_Data_Inizio"), Format(Analisi_Dettaglio_Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Analisi_Dettaglio_Data_Fine"), Format(Analisi_Dettaglio_Validita_Fine, "dd/MM/yyyy"))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function

    '##########################################################################################
    Public Function XML_Analisi_Campione(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                         ByVal Analisi_SuperUser As String,
                                         Optional ByVal Analisi_Campione_Cod As Integer = 0,
                                         Optional ByVal Analisi_Campione_Des As String = "",
                                         Optional ByVal Analisi_Campione_Coord_X As Decimal = 0,
                                         Optional ByVal Analisi_Campione_Coord_Y As Decimal = 0,
                                         Optional ByVal Analisi_Campione_Quantita As Decimal = 0,
                                         Optional ByVal Analisi_Campione_UdM As Integer = 0,
                                         Optional ByVal Analisi_Campione_Profondita As Decimal = 0,
                                         Optional ByVal Analisi_Campione_Profondita_Min As Decimal = 0,
                                         Optional ByVal Analisi_Campione_Profondita_Max As Decimal = 0,
                                         Optional ByVal Analisi_Campione_Riferimento_1 As String = "",
                                         Optional ByVal Analisi_Campione_Riferimento_2 As String = "",
                                         Optional ByVal Analisi_Campione_Riferimento_3 As String = "",
                                         Optional ByVal Analisi_Campione_Riferimento_4 As String = "",
                                         Optional ByVal Analisi_Campione_Riferimento_5 As String = "",
                                         Optional ByVal Analisi_Campione_Note As String = "",
                                         Optional ByVal Analisi_Campione_Key_Piva As String = "",
                                         Optional ByVal Analisi_Campione_Key_SaCod As Integer = 0,
                                         Optional ByVal Analisi_Campione_Key_IDGrafica As String = "",
                                         Optional ByVal Analisi_Campione_Prov As String = "",
                                         Optional ByVal Analisi_Campione_Com As String = "",
                                         Optional ByVal Analisi_Campione_Sezione As String = "0",
                                         Optional ByVal Analisi_Campione_Foglio As Integer = 0,
                                         Optional ByVal Analisi_Campione_Numero As Integer = 0,
                                         Optional ByVal Analisi_Campione_Subalterno As String = "0",
                                         Optional ByVal Analisi_Campione_Validita_Inizio As Date = #1/1/1900#,
                                         Optional ByVal Analisi_Campione_Validita_Fine As Date = #12/31/2100#
                                         ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlTxt As XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        xmlTxt = xmlDoc.CreateElement("Campione")

        'Imposto gli attributi
        xmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        xmlTxt.SetAttribute(LCase("Analisi_SuperUser"), Analisi_SuperUser)
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Cod"), CStr(Analisi_Campione_Cod))
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Des"), Analisi_Campione_Des)
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Coord_X"), CStr(Analisi_Campione_Coord_X))
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Coord_Y"), CStr(Analisi_Campione_Coord_Y))
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Quantita"), CStr(Analisi_Campione_Quantita))
        xmlTxt.SetAttribute(LCase("Analisi_Campione_UdM"), CStr(Analisi_Campione_UdM))
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Profondita"), CStr(Analisi_Campione_Profondita))
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Profondita_Min"), CStr(Analisi_Campione_Profondita_Min))
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Profondita_Max"), CStr(Analisi_Campione_Profondita_Max))
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Riferimento_1"), Analisi_Campione_Riferimento_1)
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Riferimento_2"), Analisi_Campione_Riferimento_2)
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Riferimento_3"), Analisi_Campione_Riferimento_3)
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Riferimento_4"), Analisi_Campione_Riferimento_4)
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Riferimento_5"), Analisi_Campione_Riferimento_5)
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Note"), Analisi_Campione_Note)
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Key_Piva"), Analisi_Campione_Key_Piva)
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Key_SaCod"), Analisi_Campione_Key_SaCod)
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Key_IDGrafica"), CStr(Analisi_Campione_Key_IDGrafica))
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Prov"), Analisi_Campione_Prov)
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Com"), Analisi_Campione_Com)
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Sezione"), Analisi_Campione_Sezione)
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Foglio"), CStr(Analisi_Campione_Foglio))
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Numero"), CStr(Analisi_Campione_Numero))
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Subalterno"), Analisi_Campione_Subalterno)
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Validita_Inizio"), Format(Analisi_Campione_Validita_Inizio, "dd/MM/yyyy"))
        xmlTxt.SetAttribute(LCase("Analisi_Campione_Validita_Fine"), Format(Analisi_Campione_Validita_Fine, "dd/MM/yyyy"))

        'Imposto XmlTxt come figlio del documento principale
        xmlDoc.AppendChild(xmlTxt)

        'Restituisco in uscita la stringa creata
        Return xmlDoc.InnerXml

    End Function


    '###################################################
    Public Function XML_Conto(ByVal OperazioneConto As enum_TipoOperazioneDB,
                              ByVal OperazioneRicXCod As enum_TipoOperazioneDB,
                              ByVal Piva As String,
                              ByVal Cod_Conto As Integer,
                              ByVal Conto_Descr As String,
                              ByVal Cod_Contatto As String,
                              ByVal Flag_UE As Integer,
                              ByVal Ric_Cod As Integer,
                              ByVal Anno As Integer,
                              ByVal Id_Riclassificazione As String,
                              ByVal Dare_Avere As String,
                              ByVal Imputabile As Integer,
                              ByVal Saldo As Decimal,
                              Optional ByVal BaseCode As Integer = 0,
                              Optional ByVal TopCode As Integer = 200000000,
                              Optional ByVal Old_Id_Riclassificazione As String = ""
                              ) As String

        Dim xmlDoc As New XmlDocument
        Dim xmlRicxCod As XmlElement
        Dim xmlConto As XmlElement

        xmlConto = xmlDoc.CreateElement("Conto")
        xmlRicxCod = xmlDoc.CreateElement("RicXCod")

        With xmlConto
            .SetAttribute("TipoOperazioneDB", CStr(OperazioneConto))
            .SetAttribute("piva", Piva)
            .SetAttribute("cod_conto", CStr(Cod_Conto))
            .SetAttribute("conto_descr", Conto_Descr)
            .SetAttribute("cod_contatto", Cod_Contatto)
            .SetAttribute("extra_str", "")
            .SetAttribute("extra_int", CStr(0))
            .SetAttribute("extra_date", Format(CDate(Now), "dd/MM/yyyy"))
            .SetAttribute("flag_ue", CStr(Flag_UE))
            .SetAttribute("basecode", CStr(BaseCode))
            .SetAttribute("topcode", CStr(TopCode))

        End With


        With xmlRicxCod
            .SetAttribute("TipoOperazioneDB", OperazioneRicXCod)
            .SetAttribute("piva", Piva)
            .SetAttribute("cod_conto", Cod_Conto)
            .SetAttribute("ric_cod", CStr(Ric_Cod))
            .SetAttribute("anno", CStr(Anno))
            .SetAttribute("id_riclassificazione", Id_Riclassificazione)
            .SetAttribute("dare_avere", Dare_Avere)
            .SetAttribute("saldo", CStr(Saldo))
            .SetAttribute("imputabile", CStr(Imputabile))
            .SetAttribute("validita_inizio", Format(CDate("01/01/" & CStr(Anno)), "dd/MM/yyyy"))
            .SetAttribute("validita_fine", Format(CDate("31/12/" & CStr(Anno)), "dd/MM/yyyy"))

            'In caso di Riclassificazione del conto occorre Riclassificare i suoi figli
            If Old_Id_Riclassificazione <> "" AndAlso
               Trim(Old_Id_Riclassificazione) <> Trim(Id_Riclassificazione) Then
                .SetAttribute("old_id_riclassificazione", Old_Id_Riclassificazione)
            End If


        End With

        xmlConto.AppendChild(xmlRicxCod)

        Return xmlConto.OuterXml

    End Function

End Module
