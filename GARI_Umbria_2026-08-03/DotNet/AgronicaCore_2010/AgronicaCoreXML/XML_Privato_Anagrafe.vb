Imports System.Xml
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class XML_Anagrafe


#Region "Riportate da WS_Importa GIAS per migloria infrastruttura"

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_Impresa(ByRef TipoOperazioneDB As String,
                                ByRef Piva As String,
                                ByRef Rag_Soc As String,
                                ByRef Delega As String,
                                ByRef AT_Prevalente As String,
                                ByRef Forma_Giuridica As String,
                                ByRef Forma_Conduzione As String,
                                ByRef Sup_Totale As Double,
                                ByRef PivaPadre As String,
                                ByRef TipoImpresaGerarchia As Integer,
                                ByRef Validita_Inizio As Date,
                                ByRef Validita_Fine As Date,
                                ByRef BaseCode As Integer,
                                ByRef TopCode As Integer) As System.Xml.XmlElement

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlImpresa As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlImpresa = XmlDoc.CreateElement("Impresa")

        'Imposto gli attributi
        XmlImpresa.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlImpresa.SetAttribute("piva", Piva)
        XmlImpresa.SetAttribute("rag_soc", Rag_Soc)
        XmlImpresa.SetAttribute("delega", Delega)
        XmlImpresa.SetAttribute("at_prevalente", AT_Prevalente)
        XmlImpresa.SetAttribute("forma_giuridica", Forma_Giuridica)
        XmlImpresa.SetAttribute("forma_conduzione", Forma_Conduzione)
        XmlImpresa.SetAttribute("sup_totale", CStr(Sup_Totale))

        XmlImpresa.SetAttribute("padre", CStr(PivaPadre))
        XmlImpresa.SetAttribute("tipoimpresagerarchia", CStr(TipoImpresaGerarchia))

        XmlImpresa.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlImpresa.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlImpresa.SetAttribute("basecode", CStr(BaseCode))
        XmlImpresa.SetAttribute("topcode", CStr(TopCode))

        'ImpostXmlDoco XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlImpresa)

        'Restituisco in uscita la stringa creata   
        Return XmlImpresa


    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_CentroAziendale(ByRef TipoOperazioneDB As String,
                                    ByRef Piva As String,
                                    ByRef Sa_Cod As Integer,
                                    ByRef Sa_Nome As String,
                                    ByRef X As Double,
                                    ByRef Y As Double,
                                    ByRef ZSLM As Double,
                                    ByRef Longitudine As Double,
                                    ByRef Latitudine As Double,
                                    ByRef Area As Double,
                                    ByRef CA_Sipi As String,
                                    ByRef AT_Prevalente As String,
                                    ByRef Forma_Possesso As String,
                                    ByRef Cod_TipoCentro As Integer,
                                    ByRef Sup_Totale As Double,
                                    ByRef Sup_Bosco As Double,
                                    ByRef Sup_Tare As Double,
                                    ByRef Sup_SAU As Double,
                                    ByRef Sup_Prati As Double,
                                    ByRef Validita_Inizio As Date,
                                    ByRef Validita_Fine As Date,
                                    ByRef BaseCode As Integer,
                                    ByRef TopCode As Integer,
                                    ByRef TitoloPossesso As Integer,
                                    ByRef Sup_SAU_Convenzionale As Double,
                                    ByRef Sup_SAU_Conversione As Double,
                                    ByRef Sup_SAU_Biologico As Double) As System.Xml.XmlElement


        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlCentro As System.Xml.XmlElement


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlCentro = XmlDoc.CreateElement("CentroAziendale")

        'Imposto gli attributi
        XmlCentro.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlCentro.SetAttribute("piva", Piva)
        XmlCentro.SetAttribute("sa_cod", CStr(Sa_Cod))
        XmlCentro.SetAttribute("sa_nome", Sa_Nome)

        XmlCentro.SetAttribute("x", CStr(X))
        XmlCentro.SetAttribute("y", CStr(Y))
        XmlCentro.SetAttribute("zslm", CStr(ZSLM))
        XmlCentro.SetAttribute("long", CStr(Longitudine))
        XmlCentro.SetAttribute("lat", CStr(Latitudine))
        XmlCentro.SetAttribute("area", CStr(Area))
        XmlCentro.SetAttribute("ca_sipi", CA_Sipi)
        XmlCentro.SetAttribute("at_prevalente", AT_Prevalente)
        XmlCentro.SetAttribute("forma_possesso", Forma_Possesso)

        XmlCentro.SetAttribute("tipo", CStr(Cod_TipoCentro))

        XmlCentro.SetAttribute("sup_totale", CStr(Sup_Totale))
        XmlCentro.SetAttribute("sup_bosco", CStr(Sup_Bosco))
        XmlCentro.SetAttribute("sup_tare", CStr(Sup_Tare))
        XmlCentro.SetAttribute("sup_sau", CStr(Sup_SAU))
        XmlCentro.SetAttribute("sup_prati", CStr(Sup_Prati))

        XmlCentro.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlCentro.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlCentro.SetAttribute("basecode", CStr(BaseCode))
        XmlCentro.SetAttribute("topcode", CStr(TopCode))
        XmlCentro.SetAttribute("titolopossesso", TitoloPossesso.ToString)
        XmlCentro.SetAttribute("sup_sau_convenzionale", Sup_SAU_Convenzionale.ToString)
        XmlCentro.SetAttribute("sup_sau_conversione", Sup_SAU_Conversione.ToString)
        XmlCentro.SetAttribute("sup_sau_biologico", Sup_SAU_Biologico.ToString)


        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlCentro)

        'Restituisco in uscita la stringa creata
        'Return XmlDoc.InnerXml
        Return XmlCentro

        'Distruggo gli oggetti
        XmlCentro = Nothing
        XmlDoc = Nothing

    End Function



    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_Indirizzo(ByRef TipoOperazioneDB As String,
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
                                  ByRef TopCode As Integer) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlIndirizzo As System.Xml.XmlElement


        'Creo il nodo 
        XmlIndirizzo = XmlDoc.CreateElement("Indirizzo")

        'Imposto gli attributi
        XmlIndirizzo.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlIndirizzo.SetAttribute("tipo_indirizzo", CStr(Tipo_Indirizzo))
        XmlIndirizzo.SetAttribute("cod_indirizzo", CStr(Cod_Indirizzo))
        XmlIndirizzo.SetAttribute("ind_des", Left(Ind_Des, 42))
        XmlIndirizzo.SetAttribute("frz_des", Left(Frz_Des, 30))
        XmlIndirizzo.SetAttribute("cap", CAP)
        XmlIndirizzo.SetAttribute("com_des", Left(Com_Des, 40))
        XmlIndirizzo.SetAttribute("pro_cod", Left(Pro_Cod, 2))
        XmlIndirizzo.SetAttribute("stato", Stato)
        XmlIndirizzo.SetAttribute("note", Note)
        XmlIndirizzo.SetAttribute("pro_cod_istat", Pro_Cod_Istat)
        XmlIndirizzo.SetAttribute("com_cod_istat", Com_Cod_Istat)
        XmlIndirizzo.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlIndirizzo.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlIndirizzo.SetAttribute("basecode", CStr(BaseCode))
        XmlIndirizzo.SetAttribute("topcode", CStr(TopCode))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlIndirizzo)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlIndirizzo = Nothing
        XmlDoc = Nothing

    End Function



    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_Codice(ByRef TipoOperazioneDB As String,
                                ByRef Id_Cod As Integer,
                                ByRef Val_Cod As String,
                                ByRef Validita_Inizio As Date,
                                ByRef Validita_Fine As Date,
                                ByRef BaseCode As Integer,
                                ByRef TopCode As Integer,
                                Optional ByVal ElementoInteressato As String = "") As String

        'Nota: 
        'Per ElementoInteressato si intende una stringa che vale "Appezzamento" oppure "Impianto"
        'a seconda che il codice appartenga all'uno o all'altro elemento

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlCodice As System.Xml.XmlElement


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlCodice = XmlDoc.CreateElement("Codice" & ElementoInteressato)

        'Imposto gli attributi
        XmlCodice.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlCodice.SetAttribute("id_cod", CStr(Id_Cod))
        XmlCodice.SetAttribute("val_cod", Val_Cod)
        XmlCodice.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlCodice.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlCodice.SetAttribute("basecode", CStr(BaseCode))
        XmlCodice.SetAttribute("topcode", CStr(TopCode))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlCodice)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlCodice = Nothing
        XmlDoc = Nothing


    End Function





    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_Contatto(
                                ByVal TipoOperazioneDB As String,
                                Optional ByVal Piva As String = "00000000000",
                                Optional ByVal Sa_Cod As Integer = 0,
                                Optional ByVal Cod_Contatto As String = "",
                                Optional ByVal Rag_Soc As String = "",
                                Optional ByVal Cognome As String = "",
                                Optional ByVal Nome As String = "",
                                Optional ByVal Sesso As String = "",
                                Optional ByVal Data_Nascita As Date = #1/1/1900#,
                                Optional ByVal Id_CF As Integer = 0,
                                Optional ByVal Codice_Fiscale As String = "",
                                Optional ByVal Cod_Contatto_Referente As String = "",
                                Optional ByVal Convenevoli As String = "",
                                Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                Optional ByVal BaseCode As Integer = 0,
                                Optional ByVal TopCode As Integer = 200000000) _
                                As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Contatto")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("piva"), CStr(Piva))
        XmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        XmlTxt.SetAttribute(LCase("cod_contatto"), CStr(Cod_Contatto))
        XmlTxt.SetAttribute(LCase("rag_soc"), CStr(Rag_Soc))
        XmlTxt.SetAttribute(LCase("cognome"), CStr(Cognome))
        XmlTxt.SetAttribute(LCase("nome"), CStr(Nome))
        XmlTxt.SetAttribute(LCase("sesso"), CStr(Sesso))
        XmlTxt.SetAttribute(LCase("data_nascita"), Format(Data_Nascita, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("id_cf"), CStr(Id_CF))
        XmlTxt.SetAttribute(LCase("codice_fiscale"), CStr(Codice_Fiscale))
        XmlTxt.SetAttribute(LCase("cod_contatto_referente"), CStr(Cod_Contatto_Referente))
        XmlTxt.SetAttribute(LCase("convenevoli"), CStr(Convenevoli))
        XmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
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


    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_RapportoContabileXRisorseUmane(
                                        ByVal TipoOperazioneDB As String,
                                        Optional ByVal Piva As String = "00000000000",
                                        Optional ByVal Sa_Cod As Integer = 0,
                                        Optional ByVal Cod_Risum As Integer = -1,
                                        Optional ByVal Cod_Contatto As String = "",
                                        Optional ByVal Cod_Rapporto As Integer = 0,
                                        Optional ByVal Attivita_Des As String = "",
                                        Optional ByVal Settore_Des As String = "",
                                        Optional ByVal Occasionale As Integer = 0,
                                        Optional ByVal Corrispettivo_Mensile As Double = 0,
                                        Optional ByVal Corrispettivo_Orario As Double = 0,
                                        Optional ByVal Ore_Settimanali As Double = 0,
                                        Optional ByVal Giorni_Ferie As Integer = 0,
                                        Optional ByVal Ferie_Godute As Integer = 0,
                                        Optional ByVal Giorni_Malattia As Integer = 0,
                                        Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                        Optional ByVal Validita_Fine As Date = #12/31/2100#) _
                                        As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlTxt As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("RapCon")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute("piva", CStr(Piva))
        XmlTxt.SetAttribute("sa_cod", CStr(Sa_Cod))
        XmlTxt.SetAttribute("cod_risum", CStr(Cod_Risum))
        XmlTxt.SetAttribute("cod_contatto", CStr(Cod_Contatto))
        XmlTxt.SetAttribute("cod_rapporto", CStr(Cod_Rapporto))
        XmlTxt.SetAttribute("attivita_des", CStr(Attivita_Des))
        XmlTxt.SetAttribute("settore_des", CStr(Settore_Des))
        XmlTxt.SetAttribute("occasionale", CStr(Occasionale))
        XmlTxt.SetAttribute("corrispettivo_mensile", CStr(Corrispettivo_Mensile))
        XmlTxt.SetAttribute("corrispettivo_orario", CStr(Corrispettivo_Orario))
        XmlTxt.SetAttribute("ore_settimanali", CStr(Ore_Settimanali))
        XmlTxt.SetAttribute("giorni_ferie", CStr(Giorni_Ferie))
        XmlTxt.SetAttribute("ferie_godute", CStr(Ferie_Godute))
        XmlTxt.SetAttribute("giorni_malattia", CStr(Giorni_Malattia))
        XmlTxt.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))


        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XmlTxt)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlTxt = Nothing
        XmlDoc = Nothing

    End Function



    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_Rubrica(ByRef TipoOperazioneDB As String,
                                ByRef Cod_Rubrica As Long,
                                ByRef Numero As String,
                                ByRef Descrizione As String,
                                ByRef Validita_Inizio As Date,
                                ByRef Validita_Fine As Date,
                                ByRef BaseCode As Integer,
                                ByRef TopCode As Integer) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlRubrica As System.Xml.XmlElement


        'Creo il nodo 
        XmlRubrica = XmlDoc.CreateElement("Rubrica")

        'Imposto gli attributi
        XmlRubrica.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlRubrica.SetAttribute("cod_rubrica", CStr(Cod_Rubrica))
        XmlRubrica.SetAttribute("numero", Numero)
        XmlRubrica.SetAttribute("descr", Descrizione)
        XmlRubrica.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlRubrica.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlRubrica.SetAttribute("basecode", CStr(BaseCode))
        XmlRubrica.SetAttribute("topcode", CStr(TopCode))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlRubrica)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlRubrica = Nothing
        XmlDoc = Nothing

    End Function



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
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        NodoXml.SetAttribute(LCase("padre"), CStr(padre))
        NodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(NodoXml)

        'Restituisco in uscita la stringa creata
        StringaXML = XmlDoc.InnerXml

        'DRUDI dava errore perché restituiva stringa e non XmlElement
        'Return StringaXML
        Return XmlDoc.DocumentElement

        'Distruggo gli oggetti
        NodoXml = Nothing
        XmlDoc = Nothing

        'Restituisco in uscita 

        




    End Function



    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_Particella(ByRef TipoOperazioneDB As String,
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
                              ByRef Ettari As Double,
                              ByRef Are As Integer,
                              ByRef Centiare As Integer,
                              ByRef TitoloPossesso As Integer,
                              ByRef Qualita_Cod As Integer,
                              ByRef Classe As String,
                              ByRef Reddito_Dominicale As Double,
                              ByRef Reddito_Agrario As Double,
                              ByRef Sup_Condotta As Double,
                              ByRef Validita_Inizio As Date,
                              ByRef Validita_Fine As Date,
                              ByRef Validita_Inizio_Centro As Date,
                              ByRef Validita_Fine_Centro As Date,
                              ByRef BaseCode As Integer,
                              ByRef TopCode As Integer) As System.Xml.XmlElement

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlParticella As System.Xml.XmlElement

        'Creo il nodo 
        XmlParticella = XmlDoc.CreateElement("Particella")

        'Imposto gli attributi
        XmlParticella.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlParticella.SetAttribute("piva", CStr(Piva))
        XmlParticella.SetAttribute("sa_cod", CStr(Sa_Cod))
        XmlParticella.SetAttribute("part_cod", CStr(Part_cod))
        XmlParticella.SetAttribute("prov", CStr(Prov))
        XmlParticella.SetAttribute("com", CStr(Com))
        XmlParticella.SetAttribute("sezione", CStr(Sezione))
        XmlParticella.SetAttribute("foglio", CStr(Foglio))
        XmlParticella.SetAttribute("numero", CStr(Numero))
        XmlParticella.SetAttribute("subalterno", CStr(Subalterno))
        XmlParticella.SetAttribute("partita_catastale", CStr(Partita_Catastale))
        XmlParticella.SetAttribute("ettari", CStr(Ettari))
        XmlParticella.SetAttribute("are", CStr(Are))
        XmlParticella.SetAttribute("centiare", CStr(Centiare))
        XmlParticella.SetAttribute("titolopossesso", CStr(TitoloPossesso))
        XmlParticella.SetAttribute("qualita_cod", CStr(Qualita_Cod))
        XmlParticella.SetAttribute("classe", CStr(Classe))
        XmlParticella.SetAttribute("reddito_dominicale", CStr(Reddito_Dominicale))
        XmlParticella.SetAttribute("reddito_agrario", CStr(Reddito_Agrario))
        XmlParticella.SetAttribute("sup_condotta", CStr(Sup_Condotta))
        XmlParticella.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlParticella.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlParticella.SetAttribute("validita_inizio_centro", Format(Validita_Inizio_Centro, "dd/MM/yyyy"))
        XmlParticella.SetAttribute("validita_fine_centro", Format(Validita_Fine_Centro, "dd/MM/yyyy"))
        XmlParticella.SetAttribute("basecode", CStr(BaseCode))
        XmlParticella.SetAttribute("topcode", CStr(TopCode))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlParticella)

        'Restituisco in uscita la stringa creata
        Return XmlParticella

        'Distruggo gli oggetti
        XmlParticella = Nothing
        XmlDoc = Nothing



    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_ParticellaPossesso(ByRef TipoOperazioneDB As String,
                              ByRef Partita_Catastale As String,
                              ByRef TitoloPossesso As Integer,
                              ByRef Sup_Condotta As Double,
                              ByRef Validita_Inizio As Date,
                              ByRef Validita_Fine As Date,
                              ByRef BaseCode As Integer,
                              ByRef TopCode As Integer) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlPossesso As System.Xml.XmlElement

        'Creo il nodo 
        XmlPossesso = XmlDoc.CreateElement("Possesso")

        'Imposto gli attributi
        XmlPossesso.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlPossesso.SetAttribute("partita_catastale", CStr(Partita_Catastale))
        XmlPossesso.SetAttribute("titolopossesso", CStr(TitoloPossesso))
        XmlPossesso.SetAttribute("sup_condotta", CStr(Sup_Condotta))
        XmlPossesso.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlPossesso.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlPossesso.SetAttribute("basecode", CStr(BaseCode))
        XmlPossesso.SetAttribute("topcode", CStr(TopCode))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlPossesso)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlPossesso = Nothing
        XmlDoc = Nothing



    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_ZonaxParticella(ByRef TipoOperazioneDB As String,
                                        ByRef Zona_Cod As Integer,
                                        ByRef Prov As String,
                                        ByRef Com As String,
                                        ByRef Sezione As String,
                                        ByRef Foglio As Integer,
                                        ByRef Numero As Integer,
                                        ByRef Subalterno As String,
                                        ByRef Area As Double,
                                        ByRef Validita_Inizio As Date,
                                        ByRef Validita_Fine As Date,
                                        ByRef BaseCode As Integer,
                                        ByRef TopCode As Integer) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlZona As System.Xml.XmlElement

        'Creo il nodo 
        XmlZona = XmlDoc.CreateElement("Zona")

        'Imposto gli attributi
        XmlZona.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlZona.SetAttribute("zona_cod", CStr(Zona_Cod))
        XmlZona.SetAttribute("prov", CStr(Prov))
        XmlZona.SetAttribute("com", CStr(Com))
        XmlZona.SetAttribute("sezione", CStr(Sezione))
        XmlZona.SetAttribute("foglio", CStr(Foglio))
        XmlZona.SetAttribute("numero", CStr(Numero))
        XmlZona.SetAttribute("subalterno", CStr(Subalterno))
        XmlZona.SetAttribute("area", CStr(Area))
        XmlZona.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlZona.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlZona.SetAttribute("basecode", CStr(BaseCode))
        XmlZona.SetAttribute("topcode", CStr(TopCode))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlZona)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlZona = Nothing
        XmlDoc = Nothing



    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_ParticellaxMacrouso(ByRef TipoOperazioneDB As String,
                                            ByRef Piva As String,
                                            ByRef Prov As String,
                                            ByRef Com As String,
                                            ByRef Sezione As String,
                                            ByRef Foglio As Integer,
                                            ByRef Numero As Integer,
                                            ByRef Subalterno As String,
                                            ByRef Macrouso_Cod As String,
                                            ByRef Superficie As Double,
                                            ByRef Validita_Inizio As Date,
                                            ByRef Validita_Fine As Date,
                                            ByRef BaseCode As Integer,
                                            ByRef TopCode As Integer) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlMacrouso As System.Xml.XmlElement

        'Creo il nodo 
        XmlMacrouso = XmlDoc.CreateElement("Macrouso")

        'Imposto gli attributi
        XmlMacrouso.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlMacrouso.SetAttribute("piva", CStr(Piva))
        XmlMacrouso.SetAttribute("prov", CStr(Prov))
        XmlMacrouso.SetAttribute("com", CStr(Com))
        XmlMacrouso.SetAttribute("sezione", CStr(Sezione))
        XmlMacrouso.SetAttribute("foglio", CStr(Foglio))
        XmlMacrouso.SetAttribute("numero", CStr(Numero))
        XmlMacrouso.SetAttribute("subalterno", CStr(Subalterno))
        XmlMacrouso.SetAttribute("macrouso_cod", CStr(Macrouso_Cod))
        XmlMacrouso.SetAttribute("superficie", CStr(Superficie))
        XmlMacrouso.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlMacrouso.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlMacrouso.SetAttribute("basecode", CStr(BaseCode))
        XmlMacrouso.SetAttribute("topcode", CStr(TopCode))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlMacrouso)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlMacrouso = Nothing
        XmlDoc = Nothing



    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_ParticellaxMacrousoxUtilizzo(ByRef TipoOperazioneDB As String,
                                            ByRef Piva As String,
                                            ByRef Prov As String,
                                            ByRef Com As String,
                                            ByRef Sezione As String,
                                            ByRef Foglio As Integer,
                                            ByRef Numero As Integer,
                                            ByRef Subalterno As String,
                                            ByRef Macrouso_Cod As String,
                                            ByRef Veg_Cod_Agea As String,
                                            ByRef Cul_Cod_Agea As String,
                                            ByRef Superficie As Double,
                                            ByRef Validita_Inizio As Date,
                                            ByRef Validita_Fine As Date,
                                            ByRef BaseCode As Integer,
                                            ByRef TopCode As Integer) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlMacrouso As System.Xml.XmlElement

        'Creo il nodo 
        XmlMacrouso = XmlDoc.CreateElement("Utilizzo")

        'Imposto gli attributi
        XmlMacrouso.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlMacrouso.SetAttribute("piva", CStr(Piva))
        XmlMacrouso.SetAttribute("prov", CStr(Prov))
        XmlMacrouso.SetAttribute("com", CStr(Com))
        XmlMacrouso.SetAttribute("sezione", CStr(Sezione))
        XmlMacrouso.SetAttribute("foglio", CStr(Foglio))
        XmlMacrouso.SetAttribute("numero", CStr(Numero))
        XmlMacrouso.SetAttribute("subalterno", CStr(Subalterno))
        XmlMacrouso.SetAttribute("macrouso_cod", CStr(Macrouso_Cod))
        XmlMacrouso.SetAttribute("veg_cod_agea", CStr(Veg_Cod_Agea))
        XmlMacrouso.SetAttribute("cul_cod_agea", CStr(Cul_Cod_Agea))
        XmlMacrouso.SetAttribute("superficie", CStr(Superficie))
        XmlMacrouso.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlMacrouso.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlMacrouso.SetAttribute("basecode", CStr(BaseCode))
        XmlMacrouso.SetAttribute("topcode", CStr(TopCode))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlMacrouso)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlMacrouso = Nothing
        XmlDoc = Nothing

    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_ParticellaxMacrousoxUtilizzo(ByRef TipoOperazioneDB As String,
                                            ByRef IdUtilizzo As String,
                                            ByRef Piva As String,
                                            ByRef Prov As String,
                                            ByRef Com As String,
                                            ByRef Sezione As String,
                                            ByRef Foglio As Integer,
                                            ByRef Numero As Integer,
                                            ByRef Subalterno As String,
                                            ByRef Macrouso_Cod As String,
                                            ByRef Veg_Cod_Agea As String,
                                            ByRef Cul_Cod_Agea As String,
                                            ByRef Superficie As Double,
                                            ByRef Validita_Inizio As Date,
                                            ByRef Validita_Fine As Date,
                                            ByRef BaseCode As Integer,
                                            ByRef TopCode As Integer) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlMacrouso As System.Xml.XmlElement

        'Creo il nodo 
        XmlMacrouso = XmlDoc.CreateElement("Utilizzo")

        'Imposto gli attributi
        XmlMacrouso.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlMacrouso.SetAttribute("id", CStr(IdUtilizzo))
        XmlMacrouso.SetAttribute("piva", CStr(Piva))
        XmlMacrouso.SetAttribute("prov", CStr(Prov))
        XmlMacrouso.SetAttribute("com", CStr(Com))
        XmlMacrouso.SetAttribute("sezione", CStr(Sezione))
        XmlMacrouso.SetAttribute("foglio", CStr(Foglio))
        XmlMacrouso.SetAttribute("numero", CStr(Numero))
        XmlMacrouso.SetAttribute("subalterno", CStr(Subalterno))
        XmlMacrouso.SetAttribute("macrouso_cod", CStr(Macrouso_Cod))
        XmlMacrouso.SetAttribute("veg_cod_agea", CStr(Veg_Cod_Agea))
        XmlMacrouso.SetAttribute("cul_cod_agea", CStr(Cul_Cod_Agea))
        XmlMacrouso.SetAttribute("superficie", CStr(Superficie))
        XmlMacrouso.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlMacrouso.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlMacrouso.SetAttribute("basecode", CStr(BaseCode))
        XmlMacrouso.SetAttribute("topcode", CStr(TopCode))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlMacrouso)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlMacrouso = Nothing
        XmlDoc = Nothing

    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_ParticellaxClassamento(ByRef TipoOperazioneDB As String,
                                            ByRef Prov As String,
                                            ByRef Com As String,
                                            ByRef Sezione As String,
                                            ByRef Foglio As Integer,
                                            ByRef Numero As Integer,
                                            ByRef Subalterno As String,
                                            ByRef Qualita_cod As Integer,
                                            ByVal Porzione As String,
                                            ByVal CLASSE As String,
                                            ByVal Sup_Classe As Double,
                                            ByVal REDDITO_DOMINICALE As Double,
                                            ByVal REDDITO_AGRARIO As Double,
                                            ByVal Deduzione As String,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            ByRef BaseCode As Integer,
                                            ByRef TopCode As Integer) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlClassamento As System.Xml.XmlElement

        'Creo il nodo 
        XmlClassamento = XmlDoc.CreateElement("Classamento")

        'Imposto gli attributi
        XmlClassamento.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlClassamento.SetAttribute("prov", CStr(Prov))
        XmlClassamento.SetAttribute("com", CStr(Com))
        XmlClassamento.SetAttribute("sezione", CStr(Sezione))
        XmlClassamento.SetAttribute("foglio", CStr(Foglio))
        XmlClassamento.SetAttribute("numero", CStr(Numero))
        XmlClassamento.SetAttribute("subalterno", CStr(Subalterno))
        XmlClassamento.SetAttribute("qualita_cod", CStr(Qualita_cod))
        XmlClassamento.SetAttribute("porzione", CStr(Porzione))
        XmlClassamento.SetAttribute("classe", CStr(CLASSE))
        XmlClassamento.SetAttribute("sup_classe", CStr(Sup_Classe))
        XmlClassamento.SetAttribute("reddito_dominicale", CStr(REDDITO_DOMINICALE))
        XmlClassamento.SetAttribute("reddito_agrario", CStr(REDDITO_AGRARIO))
        XmlClassamento.SetAttribute("deduzione", CStr(Deduzione))
        XmlClassamento.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlClassamento.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlClassamento.SetAttribute("basecode", CStr(BaseCode))
        XmlClassamento.SetAttribute("topcode", CStr(TopCode))


        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlClassamento)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlClassamento = Nothing
        XmlDoc = Nothing



    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_ParticellaxEleggibilita(ByRef TipoOperazioneDB As String,
                                            ByRef Prov As String,
                                            ByRef Com As String,
                                            ByRef Sezione As String,
                                            ByRef Foglio As Integer,
                                            ByRef Numero As Integer,
                                            ByRef Subalterno As String,
                                            ByRef Eleggibilita_cod As Integer,
                                            ByVal Superficie As Double,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            ByRef BaseCode As Integer,
                                            ByRef TopCode As Integer) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlClassamento As System.Xml.XmlElement

        'Creo il nodo 
        XmlClassamento = XmlDoc.CreateElement("Eleggibilita")

        'Imposto gli attributi
        XmlClassamento.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlClassamento.SetAttribute("prov", CStr(Prov))
        XmlClassamento.SetAttribute("com", CStr(Com))
        XmlClassamento.SetAttribute("sezione", CStr(Sezione))
        XmlClassamento.SetAttribute("foglio", CStr(Foglio))
        XmlClassamento.SetAttribute("numero", CStr(Numero))
        XmlClassamento.SetAttribute("subalterno", CStr(Subalterno))
        XmlClassamento.SetAttribute("eleggibilita_cod", CStr(Eleggibilita_cod))
        XmlClassamento.SetAttribute("superficie", CStr(Superficie))
        XmlClassamento.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlClassamento.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlClassamento.SetAttribute("basecode", CStr(BaseCode))
        XmlClassamento.SetAttribute("topcode", CStr(TopCode))


        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlClassamento)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlClassamento = Nothing
        XmlDoc = Nothing



    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_Fabbricato(ByRef TipoOperazioneDB As String,
                                   ByRef Piva As String,
                                   ByRef Sa_Cod As Integer,
                                   ByRef Fabbricato_Cod As Integer,
                                   ByRef Fabbricato_Des As String,
                                   ByRef Indirizzo_Cod As Integer,
                                   ByRef Tipo_Fabbricato_Cod As Integer,
                                   ByRef Com As String,
                                   ByRef Prov As String,
                                   ByRef Sezione As String,
                                   ByRef Foglio As Integer,
                                   ByRef Numero As Integer,
                                   ByRef Subalterno As String,
                                   ByRef MC_Convenzionale As Double,
                                   ByRef MC_Conversione As Double,
                                   ByRef MC_Biologico As Double,
                                   ByRef Regolamento_Cod As Integer,
                                   ByRef TitoloPossesso As Integer,
                                   ByRef Conversione_Inizio As Date,
                                   ByRef Conversione_Fine As Date,
                                   ByRef Idoneo_Costruzione As Integer,
                                   ByRef Idoneo_SeparazAmbienti As Integer,
                                   ByRef Idoneo_SeparazProdotti As Integer,
                                   ByRef Idoneo_CondIgieniche As Integer,
                                   ByRef Idoneo_AutorizSanitaria As Integer,
                                   ByRef Idoneo_HACCP As Integer,
                                   ByRef Idoneo_Planimetria As Integer,
                                   ByRef Idoneo_Layout As Integer,
                                   ByRef Idoneo_DiagrammiFlusso As Integer,
                                   ByRef Idoneo_CDX_M004 As Integer,
                                   ByRef Idoneo_SupMinCoperte As Integer,
                                   ByRef Idoneo_SupMinScoperte As Integer,
                                   ByRef Validita_Inizio As Date,
                                   ByRef Validita_Fine As Date,
                                   ByRef BaseCode As Integer,
                                   ByRef TopCode As Integer) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlFabbricato As System.Xml.XmlElement

        'Creo il nodo 
        XmlFabbricato = XmlDoc.CreateElement("Fabbricato")

        'Imposto gli attributi
        XmlFabbricato.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlFabbricato.SetAttribute("piva", CStr(Piva))
        XmlFabbricato.SetAttribute("sa_cod", CStr(Sa_Cod))
        XmlFabbricato.SetAttribute("fabbricato_cod", CStr(Fabbricato_Cod))
        XmlFabbricato.SetAttribute("fabbricato_des", CStr(Fabbricato_Des))
        XmlFabbricato.SetAttribute("indirizzo_cod", CStr(Indirizzo_Cod))
        XmlFabbricato.SetAttribute("tipo_fabbricato_cod", CStr(Tipo_Fabbricato_Cod))
        XmlFabbricato.SetAttribute("prov", CStr(Prov))
        XmlFabbricato.SetAttribute("com", CStr(Com))
        XmlFabbricato.SetAttribute("sezione", CStr(Sezione))
        XmlFabbricato.SetAttribute("foglio", CStr(Foglio))
        XmlFabbricato.SetAttribute("numero", CStr(Numero))
        XmlFabbricato.SetAttribute("subalterno", CStr(Subalterno))
        XmlFabbricato.SetAttribute("mc_convenzionale", CStr(MC_Convenzionale))
        XmlFabbricato.SetAttribute("mc_conversione", CStr(MC_Conversione))
        XmlFabbricato.SetAttribute("mc_biologico", CStr(MC_Biologico))
        XmlFabbricato.SetAttribute("regolamento_cod", CStr(Regolamento_Cod))
        XmlFabbricato.SetAttribute("titolopossesso", CStr(TitoloPossesso))
        XmlFabbricato.SetAttribute("conversione_inizio", CStr(Conversione_Inizio))
        XmlFabbricato.SetAttribute("conversione_fine", CStr(Conversione_Inizio))
        XmlFabbricato.SetAttribute("idoneo_costruzione", CStr(Idoneo_Costruzione))
        XmlFabbricato.SetAttribute("idoneo_separazambienti", CStr(Idoneo_SeparazAmbienti))
        XmlFabbricato.SetAttribute("idoneo_separazprodotti", CStr(Idoneo_SeparazProdotti))
        XmlFabbricato.SetAttribute("idoneo_condigieniche", CStr(Idoneo_CondIgieniche))
        XmlFabbricato.SetAttribute("idoneo_autorizsanitaria", CStr(Idoneo_AutorizSanitaria))
        XmlFabbricato.SetAttribute("idoneo_haccp", CStr(Idoneo_HACCP))
        XmlFabbricato.SetAttribute("idoneo_planimetria", CStr(Idoneo_Planimetria))
        XmlFabbricato.SetAttribute("idoneo_layout", CStr(Idoneo_Layout))
        XmlFabbricato.SetAttribute("idoneo_diagrammiflusso", CStr(Idoneo_DiagrammiFlusso))
        XmlFabbricato.SetAttribute("idoneo_cdx_m004", CStr(Idoneo_CDX_M004))
        XmlFabbricato.SetAttribute("idoneo_supmincoperte", CStr(Idoneo_SupMinCoperte))
        XmlFabbricato.SetAttribute("idoneo_supminscoperte", CStr(Idoneo_SupMinScoperte))
        XmlFabbricato.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlFabbricato.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlFabbricato.SetAttribute("basecode", CStr(BaseCode))
        XmlFabbricato.SetAttribute("topcode", CStr(TopCode))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlFabbricato)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlFabbricato = Nothing
        XmlDoc = Nothing



    End Function


    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_Stalla(ByRef TipoOperazioneDB As String,
                                ByRef Piva As String,
                                ByRef Sa_Cod As Integer,
                                ByRef Sta_Num As Integer,
                                ByRef Sta_Des As String,
                                ByRef AUSL_COD As String,
                                ByRef DAT_COSTR As Date,
                                ByRef DAT_CHIU As Date,
                                ByRef Cod_Fabb As String,
                                ByRef Gen_Cod As Integer,
                                ByRef Spe_Cod As Integer,
                                ByRef IPro_Cod As Integer,
                                ByRef x As String,
                                ByRef y As String,
                                ByRef Latitudine As Double,
                                ByRef Longitudine As Double,
                                ByRef Validita_Inizio As Date,
                                ByRef Validita_Fine As Date,
                                ByRef BaseCode As Integer,
                                ByRef TopCode As Integer) As String


        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlStalla As System.Xml.XmlElement

        'Creo il nodo 
        XmlStalla = XmlDoc.CreateElement("Stalla")

        'Imposto gli attributi
        XmlStalla.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlStalla.SetAttribute("piva", CStr(Piva))
        XmlStalla.SetAttribute("sa_cod", CStr(Sa_Cod))
        XmlStalla.SetAttribute("sta_num", CStr(Sta_Num))
        XmlStalla.SetAttribute("sta_des", Sta_Des)
        XmlStalla.SetAttribute("ausl_cod", AUSL_COD)
        XmlStalla.SetAttribute("dat_costr", CStr(DAT_COSTR))
        XmlStalla.SetAttribute("dat_chiu", CStr(DAT_CHIU))
        XmlStalla.SetAttribute("cod_fabb", CStr(Cod_Fabb))
        XmlStalla.SetAttribute("gen_cod", CStr(Gen_Cod))
        XmlStalla.SetAttribute("spe_cod", CStr(Spe_Cod))
        XmlStalla.SetAttribute("ipro_cod", CStr(IPro_Cod))
        XmlStalla.SetAttribute("x", CStr(x))
        XmlStalla.SetAttribute("y", CStr(y))
        XmlStalla.SetAttribute("latitudine", CStr(Latitudine))
        XmlStalla.SetAttribute("longitudine", CStr(Longitudine))
        XmlStalla.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlStalla.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlStalla.SetAttribute("basecode", CStr(BaseCode))
        XmlStalla.SetAttribute("topcode", CStr(TopCode))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlStalla)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlStalla = Nothing
        XmlDoc = Nothing


    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_Zoo_Animale(ByRef TipoOperazioneDB As String,
                                    ByRef Piva As String,
                                    ByRef Sa_Cod As Integer,
                                    ByVal Cod_Progetto As Integer,
                                    ByVal Matricola As String,
                                    ByVal Gen_Cod As Integer,
                                    ByVal Spe_Cod As Integer,
                                    ByVal IPro_Cod As Integer,
                                    ByVal Raz_Cod As Integer,
                                    ByVal Cat_Cod As Integer,
                                    ByVal Nome As String,
                                    ByVal Collare As String,
                                    ByVal Nome_Aia As String,
                                    ByVal Matricola_Aia As String,
                                    ByVal Dat_Nascita As Date,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                    ByVal Stato_Nascita As String,
                                    ByVal Provincia_Nascita As String,
                                    ByVal AUA_Azi_Nascita As String,
                                    ByVal AUSL_Azi_Nascita As String,
                                    ByVal Sesso As String,
                                    ByVal Mat_Padre As String,
                                    ByVal Mat_Madre As String,
                                    ByVal CF_Proprietario As String,
                                    ByVal CF_Detentore As String,
                                    ByVal Presente As Double,
                                    ByVal Peso As Double,
                                    ByVal Data_Pesa As Date,
                                    ByVal Metodo_Produzione As Integer,
                                    ByVal Regolamento_Cod As Integer,
                                    ByVal Conversione_Inizio As Date,
                                    ByVal Conversione_Fine As Date,
                                    ByRef BaseCode As Integer,
                                    ByRef TopCode As Integer,
                                    Optional ByRef XmlDoc As XmlDocument = Nothing) As XmlElement


        Dim XmlTxt As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        XmlTxt = XmlDoc.CreateElement("Zoo_Animale_Anagrafe")

        'Imposto gli attributi
        XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlTxt.SetAttribute(LCase("piva"), Piva)
        XmlTxt.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        XmlTxt.SetAttribute(LCase("cod_progetto"), CStr(Cod_Progetto))
        XmlTxt.SetAttribute(LCase("matricola"), CStr(Matricola))
        XmlTxt.SetAttribute(LCase("gen_cod"), CStr(Gen_Cod))
        XmlTxt.SetAttribute(LCase("spe_cod"), CStr(Spe_Cod))
        XmlTxt.SetAttribute(LCase("ipro_cod"), CStr(IPro_Cod))
        XmlTxt.SetAttribute(LCase("raz_cod"), CStr(Raz_Cod))
        XmlTxt.SetAttribute(LCase("cat_cod"), CStr(Cat_Cod))
        XmlTxt.SetAttribute(LCase("nome"), CStr(Nome))
        XmlTxt.SetAttribute(LCase("collare"), CStr(Collare))
        XmlTxt.SetAttribute(LCase("nome_aia"), CStr(Nome_Aia))
        XmlTxt.SetAttribute(LCase("matricola_aia"), CStr(Matricola_Aia))
        XmlTxt.SetAttribute(LCase("dat_nascita"), Format(Dat_Nascita, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        XmlTxt.SetAttribute(LCase("stato_nascita"), CStr(Stato_Nascita))
        XmlTxt.SetAttribute(LCase("prov_nascita"), CStr(Provincia_Nascita))
        XmlTxt.SetAttribute(LCase("aua_azi_nascita"), CStr(AUA_Azi_Nascita))
        XmlTxt.SetAttribute(LCase("ausl_azi_nascita"), CStr(AUSL_Azi_Nascita))
        XmlTxt.SetAttribute(LCase("sesso"), CStr(Sesso))
        XmlTxt.SetAttribute(LCase("mat_padre"), CStr(Mat_Padre))
        XmlTxt.SetAttribute(LCase("mat_madre"), CStr(Mat_Madre))
        XmlTxt.SetAttribute(LCase("cf_proprietario"), CStr(CF_Proprietario))
        XmlTxt.SetAttribute(LCase("cf_detentore"), CStr(CF_Detentore))
        XmlTxt.SetAttribute(LCase("presente"), CStr(Presente))
        XmlTxt.SetAttribute(LCase("peso"), CStr(Peso))
        XmlTxt.SetAttribute(LCase("data_pesa"), CStr(Data_Pesa))
        XmlTxt.SetAttribute(LCase("metodo_produzione"), CStr(Metodo_Produzione))
        XmlTxt.SetAttribute(LCase("regolamento_cod"), CStr(Regolamento_Cod))
        XmlTxt.SetAttribute(LCase("conversione_inizio"), CStr(Conversione_Inizio))
        XmlTxt.SetAttribute(LCase("conversione_fine"), CStr(Conversione_Fine))
        XmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
        XmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Restituisco in uscita la stringa creata
        Return XmlTxt


    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_Campo(ByRef TipoOperazioneDB As String,
                                ByRef Piva As String,
                                ByRef Sa_Cod As Integer,
                                ByRef Campo_Cod As Long,
                                ByRef Campo_Des As String,
                                ByRef Campo_tipo As Integer,
                                ByRef Gru_Cod As Integer,
                                ByRef Veg_Cod As Integer,
                                ByRef Conversione_Inizio As Date,
                                ByRef Conversione_Fine As Date,
                                ByRef SAU_Totale As Double,
                                ByRef SAU_Biologico As Double,
                                ByRef SAU_Conversione As Double,
                                ByRef SAU_Convenzionale As Double,
                                ByRef ConfiniRischio As String,
                                ByRef Validita_Inizio As Date,
                                ByRef Validita_Fine As Date,
                                ByRef BaseCode As Integer,
                                ByRef TopCode As Integer) As String


        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlCampo As System.Xml.XmlElement


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlCampo = XmlDoc.CreateElement("Campo")

        'Imposto gli attributi
        XmlCampo.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlCampo.SetAttribute("piva", CStr(Piva))
        XmlCampo.SetAttribute("sa_cod", CStr(Sa_Cod))
        XmlCampo.SetAttribute("campo_cod", CStr(Campo_Cod))
        XmlCampo.SetAttribute("campo_des", Campo_Des)
        XmlCampo.SetAttribute("campo_tipo", CStr(Campo_tipo))
        XmlCampo.SetAttribute("gru_cod", CStr(Gru_Cod))
        XmlCampo.SetAttribute("veg_cod", CStr(Veg_Cod))
        XmlCampo.SetAttribute("conversione_inizio", CStr(Conversione_Inizio))
        XmlCampo.SetAttribute("conversione_fine", CStr(Conversione_Fine))
        XmlCampo.SetAttribute("sau_totale", CStr(SAU_Totale))
        XmlCampo.SetAttribute("sau_biologico", CStr(SAU_Biologico))
        XmlCampo.SetAttribute("sau_conversione", CStr(SAU_Conversione))
        XmlCampo.SetAttribute("sau_convenzionale", CStr(SAU_Convenzionale))
        XmlCampo.SetAttribute("confinirischio", CStr(ConfiniRischio))
        XmlCampo.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlCampo.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlCampo.SetAttribute("basecode", CStr(BaseCode))
        XmlCampo.SetAttribute("topcode", CStr(TopCode))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlCampo)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlCampo = Nothing
        XmlDoc = Nothing

    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_CampoParticella(ByRef TipoOperazioneDB As String,
                                         ByRef Piva As String,
                                         ByRef Sa_Cod As Integer,
                                         ByRef Campo_Cod As Long,
                                         ByRef Prov As String,
                                         ByRef Com As String,
                                         ByRef Sezione As String,
                                         ByRef Foglio As Integer,
                                         ByRef Numero As Integer,
                                         ByRef Subalterno As String,
                                         ByRef Area As Double,
                                         ByRef SAU_Convenz_Ettari As Double,
                                         ByRef SAU_Convenz_Are As Integer,
                                         ByRef SAU_Convenz_Centiare As Integer,
                                         ByRef SAU_Convers_Ettari As Double,
                                         ByRef SAU_Convers_Are As Integer,
                                         ByRef SAU_Convers_Centiare As Integer,
                                         ByRef SAU_Bio_Ettari As Double,
                                         ByRef SAU_Bio_Are As Integer,
                                         ByRef SAU_Bio_Centiare As Integer,
                                         ByRef Validita_Inizio As Date,
                                         ByRef Validita_Fine As Date,
                                         ByRef BaseCode As Integer,
                                         ByRef TopCode As Integer) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlCampo As System.Xml.XmlElement


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlCampo = XmlDoc.CreateElement("CampoxParticella")

        'Imposto gli attributi
        XmlCampo.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        XmlCampo.SetAttribute("piva", CStr(Piva))
        XmlCampo.SetAttribute("sa_cod", CStr(Sa_Cod))
        XmlCampo.SetAttribute("campo_cod", CStr(Campo_Cod))
        XmlCampo.SetAttribute("prov", CStr(Prov))
        XmlCampo.SetAttribute("com", CStr(Com))
        XmlCampo.SetAttribute("sezione", CStr(Sezione))
        XmlCampo.SetAttribute("foglio", CStr(Foglio))
        XmlCampo.SetAttribute("numero", CStr(Numero))
        XmlCampo.SetAttribute("subalterno", CStr(Subalterno))
        XmlCampo.SetAttribute("area", CStr(Area))
        XmlCampo.SetAttribute("sau_convenz_ettari", CStr(SAU_Convenz_Ettari))
        XmlCampo.SetAttribute("sau_convenz_are", CStr(SAU_Convenz_Are))
        XmlCampo.SetAttribute("sau_convenz_centiare", CStr(SAU_Convenz_Centiare))
        XmlCampo.SetAttribute("sau_convers_ettari", CStr(SAU_Convers_Ettari))
        XmlCampo.SetAttribute("sau_convers_are", CStr(SAU_Convers_Are))
        XmlCampo.SetAttribute("sau_convers_centiare", CStr(SAU_Convers_Centiare))
        XmlCampo.SetAttribute("sau_bio_ettari", CStr(SAU_Bio_Ettari))
        XmlCampo.SetAttribute("sau_bio_are", CStr(SAU_Bio_Are))
        XmlCampo.SetAttribute("sau_bio_centiare", CStr(SAU_Bio_Centiare))
        XmlCampo.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlCampo.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlCampo.SetAttribute("basecode", CStr(BaseCode))
        XmlCampo.SetAttribute("topcode", CStr(TopCode))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlCampo)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlCampo = Nothing
        XmlDoc = Nothing


    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_Appezzamento(ByRef TipoOperazioneDB As String,
                                ByRef Piva As String,
                                ByRef Sa_Cod As Integer,
                                ByRef Appezza As Integer,
                                ByRef Sup_App As Double,
                                ByRef Data_App As String,
                                ByRef Ep_Camp As String,
                                ByRef X As Double,
                                ByRef Y As Double,
                                ByRef Zslm As Double,
                                ByRef Esposiz As String,
                                ByRef Pende As Double,
                                ByRef Ubicazione As String,
                                ByRef Num_Del As Integer,
                                ByRef Clas As String,
                                ByRef Sabbia As Double,
                                ByRef Limo As Double,
                                ByRef Argilla As Double,
                                ByRef pH As Double,
                                ByRef CalTot As Double,
                                ByRef CalAtt As Double,
                                ByRef SostOrg As Double,
                                ByRef K2OAss As Double,
                                ByRef P2O5Ass As Double,
                                ByRef Mg As Double,
                                ByRef Ntot As Double,
                                ByRef Um_S As Double,
                                ByRef Cl_Dren As String,
                                ByRef Falda As Integer,
                                ByRef CsC As Double,
                                ByRef K2OAss_Data As String,
                                ByRef MatOrg As Double,
                                ByRef MatOrg_Data As String,
                                ByRef NOtot_Data As String,
                                ByRef NOtot As Double,
                                ByRef P2O5Ass_Data As String,
                                ByRef Suolo_CodAttri As String,
                                ByRef App_Nome As String,
                                ByRef Campo_Spia As Integer,
                                ByRef Campo_Spia_Area As Double,
                                ByRef Cs_SIPI As String,
                                ByRef Campo_Cod As Integer,
                                ByRef Prossimo As Integer,
                                ByRef Data_Inizio As String,
                                ByRef Data_Fine As String,
                                ByRef Validita_Inizio As Date,
                                ByRef Validita_Fine As Date,
                                ByRef BaseCode As Integer,
                                ByRef TopCode As Integer,
                                    Optional ByVal codice_alfanumerico As String = "#",
                                    Optional ByVal Blk_Flag As Integer = 0) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlAppezzamento As System.Xml.XmlElement



        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlAppezzamento = XmlDoc.CreateElement("Appezzamento")

        'Imposto gli attributi
        XmlAppezzamento.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        XmlAppezzamento.SetAttribute("piva", CStr(Piva))
        XmlAppezzamento.SetAttribute("sa_cod", CStr(Sa_Cod))
        XmlAppezzamento.SetAttribute("appezza", CStr(Appezza))

        XmlAppezzamento.SetAttribute("sup_app", CStr(Sup_App))
        XmlAppezzamento.SetAttribute("data_app", CStr(Data_App))
        XmlAppezzamento.SetAttribute("ep_camp", CStr(Ep_Camp))
        XmlAppezzamento.SetAttribute("x", CStr(X))
        XmlAppezzamento.SetAttribute("y", CStr(Y))
        XmlAppezzamento.SetAttribute("zslm", CStr(Zslm))
        XmlAppezzamento.SetAttribute("esposiz", CStr(Esposiz))
        XmlAppezzamento.SetAttribute("pende", CStr(Pende))
        XmlAppezzamento.SetAttribute("ubicazione", CStr(Ubicazione))
        XmlAppezzamento.SetAttribute("num_del", CStr(Num_Del))
        XmlAppezzamento.SetAttribute("clas", CStr(Clas))

        XmlAppezzamento.SetAttribute("sabbia", CStr(Sabbia))
        XmlAppezzamento.SetAttribute("limo", CStr(Limo))
        XmlAppezzamento.SetAttribute("argilla", CStr(Argilla))
        XmlAppezzamento.SetAttribute("ph", CStr(pH))

        XmlAppezzamento.SetAttribute("caltot", CStr(CalTot))
        XmlAppezzamento.SetAttribute("calatt", CStr(CalAtt))
        XmlAppezzamento.SetAttribute("sostorg", CStr(SostOrg))
        XmlAppezzamento.SetAttribute("k2oass", CStr(K2OAss))
        XmlAppezzamento.SetAttribute("p2o5ass", CStr(P2O5Ass))
        XmlAppezzamento.SetAttribute("mg", CStr(Mg))
        XmlAppezzamento.SetAttribute("ntot", CStr(Ntot))
        XmlAppezzamento.SetAttribute("um_s", CStr(Um_S))
        XmlAppezzamento.SetAttribute("cl_dren", CStr(Cl_Dren))
        XmlAppezzamento.SetAttribute("falda", CStr(Falda))
        XmlAppezzamento.SetAttribute("csc", CStr(CsC))
        XmlAppezzamento.SetAttribute("k2oass_data", CStr(K2OAss_Data))
        XmlAppezzamento.SetAttribute("matorg", CStr(MatOrg))
        XmlAppezzamento.SetAttribute("matorg_data", CStr(MatOrg_Data))
        XmlAppezzamento.SetAttribute("notot_data", CStr(NOtot_Data))
        XmlAppezzamento.SetAttribute("notot", CStr(NOtot))
        XmlAppezzamento.SetAttribute("p2o5ass_data", CStr(P2O5Ass_Data))
        XmlAppezzamento.SetAttribute("suolo_codattri", CStr(Suolo_CodAttri))

        XmlAppezzamento.SetAttribute("app_nome", CStr(App_Nome))
        XmlAppezzamento.SetAttribute("campo_spia", CStr(Campo_Spia))
        XmlAppezzamento.SetAttribute("campo_spia_area", CStr(Campo_Spia_Area))
        XmlAppezzamento.SetAttribute("cs_sipi", CStr(Cs_SIPI))
        XmlAppezzamento.SetAttribute("campo_cod", CStr(Campo_Cod))
        XmlAppezzamento.SetAttribute("prossimo", CStr(Prossimo))
        XmlAppezzamento.SetAttribute("data_inizio", CStr(Data_Inizio))
        XmlAppezzamento.SetAttribute("data_fine", CStr(Data_Fine))

        XmlAppezzamento.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlAppezzamento.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlAppezzamento.SetAttribute("basecode", CStr(BaseCode))
        XmlAppezzamento.SetAttribute("topcode", CStr(TopCode))

        XmlAppezzamento.SetAttribute("codice_alfanumerico", CStr(codice_alfanumerico))

        XmlAppezzamento.SetAttribute("blk_flag", CStr(Blk_Flag))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlAppezzamento)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XmlAppezzamento = Nothing
        XmlDoc = Nothing

    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function XML_AppezzamentoParticella(ByRef TipoOperazioneDB As String,
                                                ByRef Piva As String,
                                                ByRef Sa_Cod As Integer,
                                                ByRef Appezza As Long,
                                                ByRef Prov As String,
                                                ByRef Com As String,
                                                ByRef Sezione As String,
                                                ByRef Foglio As Integer,
                                                ByRef Numero As Integer,
                                                ByRef Subalterno As String,
                                                ByRef Area As Double,
                                                ByRef SAU_Convenz_Ettari As Double,
                                                ByRef SAU_Convenz_Are As Integer,
                                                ByRef SAU_Convenz_Centiare As Integer,
                                                ByRef SAU_Convers_Ettari As Double,
                                                ByRef SAU_Convers_Are As Integer,
                                                ByRef SAU_Convers_Centiare As Integer,
                                                ByRef SAU_Bio_Ettari As Double,
                                                ByRef SAU_Bio_Are As Integer,
                                                ByRef SAU_Bio_Centiare As Integer,
                                                ByRef Validita_Inizio As Date,
                                                ByRef Validita_Fine As Date,
                                                ByRef BaseCode As Integer,
                                                ByRef TopCode As Integer) As String

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim Xml_AppezzamentoPart As System.Xml.XmlElement


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        Xml_AppezzamentoPart = XmlDoc.CreateElement("Particella")

        'Imposto gli attributi
        Xml_AppezzamentoPart.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        Xml_AppezzamentoPart.SetAttribute("piva", CStr(Piva))
        Xml_AppezzamentoPart.SetAttribute("sa_cod", CStr(Sa_Cod))
        Xml_AppezzamentoPart.SetAttribute("appezza", CStr(Appezza))
        Xml_AppezzamentoPart.SetAttribute("prov", CStr(Prov))
        Xml_AppezzamentoPart.SetAttribute("com", CStr(Com))
        Xml_AppezzamentoPart.SetAttribute("sezione", CStr(Sezione))
        Xml_AppezzamentoPart.SetAttribute("foglio", CStr(Foglio))
        Xml_AppezzamentoPart.SetAttribute("numero", CStr(Numero))
        Xml_AppezzamentoPart.SetAttribute("subalterno", CStr(Subalterno))
        Xml_AppezzamentoPart.SetAttribute("area", CStr(Area))
        Xml_AppezzamentoPart.SetAttribute("sau_convenz_ettari", CStr(SAU_Convenz_Ettari))
        Xml_AppezzamentoPart.SetAttribute("sau_convenz_are", CStr(SAU_Convenz_Are))
        Xml_AppezzamentoPart.SetAttribute("sau_convenz_centiare", CStr(SAU_Convenz_Centiare))
        Xml_AppezzamentoPart.SetAttribute("sau_convers_ettari", CStr(SAU_Convers_Ettari))
        Xml_AppezzamentoPart.SetAttribute("sau_convers_are", CStr(SAU_Convers_Are))
        Xml_AppezzamentoPart.SetAttribute("sau_convers_centiare", CStr(SAU_Convers_Centiare))
        Xml_AppezzamentoPart.SetAttribute("sau_bio_ettari", CStr(SAU_Bio_Ettari))
        Xml_AppezzamentoPart.SetAttribute("sau_bio_are", CStr(SAU_Bio_Are))
        Xml_AppezzamentoPart.SetAttribute("sau_bio_centiare", CStr(SAU_Bio_Centiare))
        Xml_AppezzamentoPart.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        Xml_AppezzamentoPart.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        Xml_AppezzamentoPart.SetAttribute("basecode", CStr(BaseCode))
        Xml_AppezzamentoPart.SetAttribute("topcode", CStr(TopCode))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(Xml_AppezzamentoPart)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        Xml_AppezzamentoPart = Nothing
        XmlDoc = Nothing


    End Function


    '##########################################################################################
    Public Function XML_Impianto(ByRef TipoOperazioneDB As String,
                            ByRef Piva As String,
                            ByRef Sa_Cod As Integer,
                            ByRef Campo_Cod As Integer,
                            ByRef Appezza As Integer,
                            ByRef Id_Imp As Integer,
                            ByRef Cod_Resp As Integer,
                            ByRef Cod_Ente As Integer,
                            ByRef Campo_Spia As Integer,
                            ByRef Data As Date,
                            ByRef Cul_Cod As Integer,
                            ByRef Data_Raccolta As String,
                            ByRef ResaPrevista As Double,
                            ByRef ResaEffettiva As Double,
                            ByRef Scarto As Integer,
                            ByRef Ind_Mat_Cod As Integer,
                            ByRef Ind_Mat_Ril As String,
                            ByRef Sta_Ter As String,
                            ByRef Cop_DI As String,
                            ByRef Cop_DF As String,
                            ByRef Tra_Fila As Double,
                            ByRef Su_Fila As Double,
                            ByRef P_HA As Double,
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
                            ByRef Imp_Cod As String,
                            ByRef Regolamento As Integer,
                            ByRef Finanziamento As Integer,
                            ByRef Su_Cod As Integer,
                            ByRef Cop_Cod As Integer,
                            ByRef Foral_Cod As Integer,
                            ByRef Tecn_Cod As Integer,
                            ByRef ProvenienzaSeme As Integer,
                            ByRef Sup_Imp As Double,
                            ByRef Grva_Cod As Integer,
                            ByRef Validita_Inizio As Date,
                            ByRef Validita_Fine As Date,
                            ByRef BaseCode As Integer,
                            ByRef TopCode As Integer,
                            Optional ByVal unita_vitata As String = "#") As System.Xml.XmlElement

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlImpianto As System.Xml.XmlElement

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        XmlImpianto = XmlDoc.CreateElement("Reg_Impianto")

        'Imposto gli attributi
        XmlImpianto.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        XmlImpianto.SetAttribute("piva", CStr(Piva))
        XmlImpianto.SetAttribute("sa_cod", CStr(Sa_Cod))
        XmlImpianto.SetAttribute("id_campo", CStr(Campo_Cod))
        XmlImpianto.SetAttribute("appezza", CStr(Appezza))

        XmlImpianto.SetAttribute("id_reg", CStr(Id_Imp))
        XmlImpianto.SetAttribute("cod_resp", CStr(Cod_Resp))
        XmlImpianto.SetAttribute("cod_ente", CStr(Cod_Ente))
        XmlImpianto.SetAttribute("campo_spia", CStr(Campo_Spia))
        XmlImpianto.SetAttribute("data", CStr(Data))
        XmlImpianto.SetAttribute("cul_cod", CStr(Cul_Cod))
        XmlImpianto.SetAttribute("data_raccolta", CStr(Data_Raccolta))
        XmlImpianto.SetAttribute("resa_prevista", CStr(ResaPrevista))
        XmlImpianto.SetAttribute("resa_effettiva", CStr(ResaEffettiva))
        XmlImpianto.SetAttribute("scarto", CStr(Scarto))
        XmlImpianto.SetAttribute("ind_mat_cod", CStr(Ind_Mat_Cod))
        XmlImpianto.SetAttribute("ind_mat_ril", CStr(Ind_Mat_Ril))
        XmlImpianto.SetAttribute("sta_ter", CStr(Sta_Ter))
        XmlImpianto.SetAttribute("cop_di", CStr(Cop_DI))
        XmlImpianto.SetAttribute("cop_df", CStr(Cop_DF))
        XmlImpianto.SetAttribute("tra_fila", CStr(Tra_Fila))
        XmlImpianto.SetAttribute("su_fila", CStr(Su_Fila))
        XmlImpianto.SetAttribute("p_ha", CStr(P_HA))
        XmlImpianto.SetAttribute("setup_cod", CStr(Setup_Cod))
        XmlImpianto.SetAttribute("port_cod", CStr(Port_Cod))
        XmlImpianto.SetAttribute("stru_prot", CStr(Stru_Prot))
        XmlImpianto.SetAttribute("pro_pag", CStr(Pro_Pag))
        XmlImpianto.SetAttribute("seme_q", CStr(Seme_Q))
        XmlImpianto.SetAttribute("seme_t", CStr(Seme_T))
        XmlImpianto.SetAttribute("seme_p", CStr(Seme_P))
        XmlImpianto.SetAttribute("seme_d", CStr(Seme_D))

        XmlImpianto.SetAttribute("stato_residui", CStr(Stato_Residui))
        XmlImpianto.SetAttribute("denitrificazione", CStr(Denitrificazione))
        XmlImpianto.SetAttribute("volatilizzazione", CStr(Volatilizzazione))
        XmlImpianto.SetAttribute("profonditalav", CStr(ProfonditaLav))
        XmlImpianto.SetAttribute("cover", CStr(Cover))
        XmlImpianto.SetAttribute("monitorato", CStr(Monitorato))
        XmlImpianto.SetAttribute("codice_fiscale_tecnico", CStr(Codice_Ficale_Tecnico))
        XmlImpianto.SetAttribute("data_conversione", CStr(Data_Conversione))
        XmlImpianto.SetAttribute("grfi_cod", CStr(Grfi_Cod))
        XmlImpianto.SetAttribute("imp_cod", CStr(Imp_Cod))
        XmlImpianto.SetAttribute("regolamento", CStr(Regolamento))
        XmlImpianto.SetAttribute("finanziamento", CStr(Finanziamento))
        XmlImpianto.SetAttribute("su_cod", CStr(Su_Cod))
        XmlImpianto.SetAttribute("cop_cod", CStr(Cop_Cod))
        XmlImpianto.SetAttribute("foral_cod", CStr(Foral_Cod))
        XmlImpianto.SetAttribute("tecn_cod", CStr(Tecn_Cod))
        XmlImpianto.SetAttribute("provenienzaseme", CStr(ProvenienzaSeme))
        XmlImpianto.SetAttribute("sup_imp", CStr(Sup_Imp))
        XmlImpianto.SetAttribute("grva_cod_veg", CStr(Grva_Cod))

        XmlImpianto.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        XmlImpianto.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        XmlImpianto.SetAttribute("basecode", CStr(BaseCode))
        XmlImpianto.SetAttribute("topcode", CStr(TopCode))

        XmlImpianto.SetAttribute("unita_vitata", CStr(unita_vitata))

        'Imposto XmlIndirizzo come figlio del documento principale
        XmlDoc.AppendChild(XmlImpianto)

        Return XmlImpianto

        'Distruggo gli oggetti
        XmlImpianto = Nothing
        XmlDoc = Nothing


    End Function


#End Region

    '##########################################################################################
    'Viene chiamata quando si deve creare l'xml di una sola impresa
    'crea il nodo DatiImprese
    'e poi chiama la funzione XML_2_Impresa che crea tutto il blocco dell'impresa (impresa, indirizzi, codici, gerarchia, ecc)
    Public Function XML_2_Imprese(ByRef Log_Errori As String,
                                    ByRef XmlDoc As XmlDocument,
                                    ByVal BaseCode As Integer,
                                    ByVal TopCode As Integer,
                                    ByVal TipoOperazioneDB_Impresa As enum_TipoOperazioneDB,
                                    ByVal TipoOperazioneDB_Contatto As enum_TipoOperazioneDB,
                                    ByVal Piva As String,
                                    ByVal Rag_Soc As String,
                                    ByVal PivaPadre As String,
                                    ByVal PivaContatto As String,
                                    ByVal TipoImpresaGerarchia As Integer,
                                    ByVal Tipo_Indirizzo As Integer,
                                    ByVal Pro_Cod_Istat As String,
                                    ByVal Com_Cod_Istat As String,
                                    ByVal Cod_Indirizzo As Integer,
                                    ByVal Ind_Des As String,
                                    ByVal Frz_Des As String,
                                    ByVal CAP As String,
                                    ByVal Stato As String,
                                    ByVal Note As String,
                                    ByVal Dt_Codici As DataTable,
                                    ByVal DT_RisUm As DataTable,
                                    ByVal Dt_ContattiCodici As DataTable,
                                    Optional ByVal Delega As String = "",
                                    Optional ByVal AT_Prevalente As String = "",
                                    Optional ByVal Forma_Giuridica As String = "",
                                    Optional ByVal Forma_Conduzione As String = "",
                                    Optional ByVal Sup_Totale As Decimal = 0,
                                    Optional ByVal Note_Impresa As String = "",
                                    Optional ByVal Sa_Cod As Integer = 0,
                                    Optional ByVal Id_CF As Integer = 0,
                                    Optional ByVal Convenevoli As String = "",
                                    Optional ByVal Codice_Fiscale As String = "",
                                    Optional ByVal Tipo_Indirizzo_Default As Integer = 0,
                                    Optional ByVal Cod_Contatto_Referente As String = "",
                                    Optional ByVal Validita_Inizio_Impresa As Date = AGRODATAINIZIO,
                                    Optional ByVal Validita_Fine_Impresa As Date = AGRODATAFINE
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

            XmlImpresa = XML_2_Impresa(Log_Errori,
                                        XmlDoc,
                                        BaseCode,
                                        TopCode,
                                        TipoOperazioneDB_Impresa,
                                        TipoOperazioneDB_Contatto,
                                        Piva,
                                        Rag_Soc,
                                        PivaPadre,
                                        PivaContatto,
                                        TipoImpresaGerarchia,
                                        Tipo_Indirizzo,
                                        Pro_Cod_Istat,
                                        Com_Cod_Istat,
                                        Cod_Indirizzo,
                                        Ind_Des,
                                        Frz_Des,
                                        CAP,
                                        Stato,
                                        Note,
                                        Dt_Codici,
                                        DT_RisUm,
                                        Dt_ContattiCodici,
                                        Delega,
                                        AT_Prevalente,
                                        Forma_Giuridica,
                                        Forma_Conduzione,
                                        Sup_Totale,
                                        Note_Impresa,
                                        Sa_Cod,
                                        Id_CF,
                                        Convenevoli,
                                        Codice_Fiscale,
                                        Tipo_Indirizzo_Default,
                                        Cod_Contatto_Referente,
                                        Validita_Inizio_Impresa,
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
    Public Function XML_2_Impresa(ByRef Log_Errori As String,
                                    ByRef XmlDoc As XmlDocument,
                                    ByVal BaseCode As Integer,
                                    ByVal TopCode As Integer,
                                    ByVal TipoOperazioneDB_Impresa As enum_TipoOperazioneDB,
                                    ByVal TipoOperazioneDB_Contatto As enum_TipoOperazioneDB,
                                    ByVal Piva As String,
                                    ByVal Rag_Soc As String,
                                    ByVal PivaPadre As String,
                                    ByVal PivaContatto As String,
                                    ByVal TipoImpresaGerarchia As Integer,
                                    ByVal Tipo_Indirizzo As Integer,
                                    ByVal Pro_Cod_Istat As String,
                                    ByVal Com_Cod_Istat As String,
                                    ByVal Cod_Indirizzo As Integer,
                                    ByVal Ind_Des As String,
                                    ByVal Frz_Des As String,
                                    ByVal CAP As String,
                                    ByVal Stato As String,
                                    ByVal Note As String,
                                    ByVal Dt_Codici As DataTable,
                                    ByVal DT_RisUm As DataTable,
                                    ByVal Dt_ContattiCodici As DataTable,
                                    Optional ByVal Delega As String = "",
                                    Optional ByVal AT_Prevalente As String = "",
                                    Optional ByVal Forma_Giuridica As String = "",
                                    Optional ByVal Forma_Conduzione As String = "",
                                    Optional ByVal Sup_Totale As Decimal = 0,
                                    Optional ByVal Note_Impresa As String = "",
                                    Optional ByVal Sa_Cod As Integer = 0,
                                    Optional ByVal Id_CF As Integer = 0,
                                    Optional ByVal Convenevoli As String = "",
                                    Optional ByVal Codice_Fiscale As String = "",
                                    Optional ByVal Tipo_Indirizzo_Default As Integer = 0,
                                    Optional ByVal Cod_Contatto_Referente As String = "",
                                    Optional ByVal Validita_Inizio_Impresa As Date = AGRODATAINIZIO,
                                    Optional ByVal Validita_Fine_Impresa As Date = AGRODATAFINE
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

            XmlImpresa = XML_2_Impresa_Impresa(Log_Errori,
                                                    XmlDoc,
                                                    BaseCode,
                                                    TopCode,
                                                    TipoOperazioneDB_Impresa,
                                                    Piva,
                                                    Rag_Soc,
                                                    PivaPadre,
                                                    TipoImpresaGerarchia,
                                                    Delega,
                                                    AT_Prevalente,
                                                    Forma_Giuridica,
                                                    Forma_Conduzione,
                                                    Sup_Totale,
                                                    Note_Impresa,
                                                    Validita_Inizio_Impresa,
                                                    Validita_Fine_Impresa)



            '#######################################################
            '##################   INDIRIZZO    #####################
            '#######################################################

            XmlIndirizzo = XML_2_Indirizzo(TipoOperazioneDB_Impresa,
                                            Tipo_Indirizzo,
                                            Pro_Cod_Istat,
                                            Com_Cod_Istat,
                                            XmlDoc,
                                            Cod_Indirizzo,
                                            Ind_Des,
                                            Frz_Des,
                                            CAP,
                                            Stato,
                                            Note,
                                            Validita_Inizio_Impresa,
                                            Validita_Fine_Impresa,
                                            BaseCode,
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

                    XmlImpresaCodice = XML_2_Codice(TipoOperazioneDB_Codice,
                                                    Id_Cod,
                                                    BaseCode,
                                                    TopCode,
                                                    XmlDoc,
                                                    Val_Cod,
                                                    Validita_Inizio_Codice,
                                                    Validita_Fine_Codice)

                    XmlImpresa.AppendChild(XmlImpresaCodice)

                Next

            End If


            '#######################################################
            '################   DATI CONTATTI    ###################
            '#######################################################

            XmlDatiContatti = XML_2_Contatti(Log_Errori,
                                            XmlDoc,
                                            BaseCode,
                                            TopCode,
                                            True,
                                            TipoOperazioneDB_Contatto,
                                            PivaContatto,
                                            Piva,
                                            Nothing,
                                            DT_RisUm,
                                            Sa_Cod,
                                            Id_CF,
                                            Rag_Soc,
                                            Convenevoli,
                                            Codice_Fiscale,
                                            Tipo_Indirizzo_Default,
                                            ,
                                            ,
                                            ,
                                            ,
                                            Cod_Contatto_Referente,
                                            Validita_Inizio_Impresa,
                                            Validita_Fine_Impresa,
                                            Nothing,
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
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        NodoXml.SetAttribute("piva", Piva)
        NodoXml.SetAttribute("rag_soc", Rag_Soc)
        NodoXml.SetAttribute("padre", CStr(PivaPadre))
        NodoXml.SetAttribute("tipoimpresagerarchia", CStr(TipoImpresaGerarchia))
        NodoXml.SetAttribute("delega", Delega)
        NodoXml.SetAttribute("at_prevalente", AT_Prevalente)
        NodoXml.SetAttribute("forma_giuridica", Forma_Giuridica)
        NodoXml.SetAttribute("forma_conduzione", Forma_Conduzione)
        NodoXml.SetAttribute("sup_totale", CStr(Sup_Totale))
        NodoXml.SetAttribute("note", Note)
        NodoXml.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        NodoXml.SetAttribute("basecode", CStr(BaseCode))
        NodoXml.SetAttribute("topcode", CStr(TopCode))
        NodoXml.SetAttribute("Data_Creazione", CStr(Data_Creazione))
        NodoXml.SetAttribute("Data_Modifica", CStr(Data_Modifica))
        NodoXml.SetAttribute("Username_Creazione", CStr(Username_Creazione))
        NodoXml.SetAttribute("Username_Modifica", CStr(Username_Modifica))
        NodoXml.SetAttribute("Validazione", CStr(Validazione))
        NodoXml.SetAttribute("Data_Validazione", CStr(Data_Validazione))
        NodoXml.SetAttribute("UserName_Validazione", CStr(UserName_Validazione))
        NodoXml.SetAttribute("Blk_Flag", CStr(Blk_Flag))
        NodoXml.SetAttribute("Blk_Inizio_Data", CStr(Blk_Inizio_Data))
        NodoXml.SetAttribute("Blk_Inizio_Username", CStr(Blk_Inizio_Username))
        NodoXml.SetAttribute("Blk_Inizio_Note", CStr(Blk_Inizio_Note))
        NodoXml.SetAttribute("Blk_Fine_Data", CStr(Blk_Fine_Data))
        NodoXml.SetAttribute("Blk_Fine_Username", CStr(Blk_Fine_Username))
        NodoXml.SetAttribute("Blk_Fine_Note", CStr(Blk_Fine_Note))
        NodoXml.SetAttribute("codEsenzione", CStr(codEsenzione))
        NodoXml.SetAttribute("codOp", CStr(codOp))
        NodoXml.SetAttribute("documento", CStr(documento))
        NodoXml.SetAttribute("dtDocumento", CStr(dtDocumento))
        NodoXml.SetAttribute("dtValidazione", CStr(dtValidazione))
        NodoXml.SetAttribute("esenzioneDescr", CStr(esenzioneDescr))
        NodoXml.SetAttribute("flagAltreSedi", CStr(flagAltreSedi))
        NodoXml.SetAttribute("flagValidato", CStr(flagValidato))
        NodoXml.SetAttribute("idUtenteValidazione", CStr(idUtenteValidazione))
        NodoXml.SetAttribute("opDescr", CStr(opDescr))
        NodoXml.SetAttribute("fonte", CStr(fonte))
        NodoXml.SetAttribute("dt_Fonte", CStr(dt_Fonte))
        NodoXml.SetAttribute("aziendaCessata", CStr(aziendaCessata))
        NodoXml.SetAttribute("aziendaIscrittaCAA", CStr(aziendaIscrittaCAA))
        NodoXml.SetAttribute("aziendaPresente", CStr(aziendaPresente))
        NodoXml.SetAttribute("aziendaValidata", CStr(aziendaValidata))
        NodoXml.SetAttribute("dataIscrizioneCAA", CStr(dataIscrizioneCAA))
        NodoXml.SetAttribute("dataValidazione", CStr(dataValidazione))
        NodoXml.SetAttribute("dataVariazioneAzienda", CStr(dataVariazioneAzienda))
        NodoXml.SetAttribute("maxDataVariazioneIbanAzienda", CStr(maxDataVariazioneIbanAzienda))
        NodoXml.SetAttribute("maxDataVariazionePersoneAzienda", CStr(maxDataVariazionePersoneAzienda))
        NodoXml.SetAttribute("maxDataVariazionePossessiAzienda", CStr(maxDataVariazionePossessiAzienda))
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
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        NodoXml.SetAttribute(LCase("id_cod"), CStr(Id_Cod))
        NodoXml.SetAttribute(LCase("val_cod"), CStr(Val_Cod))
        NodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
        NodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))

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
    Public Function XML_2_CentroAziendale_CentroAziendale(ByRef Log_Errori As String,
                                                            ByRef XmlDoc As XmlDocument,
                                                            ByVal BaseCode As Integer,
                                                            ByVal TopCode As Integer,
                                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                            ByVal Piva As String,
                                                            ByVal Sa_Cod As Integer,
                                                            ByVal Sa_Nome As String,
                                                            Optional ByVal X As Decimal = 0,
                                                            Optional ByVal Y As Decimal = 0,
                                                            Optional ByVal ZSLM As Decimal = 0,
                                                            Optional ByVal Longitudine As Decimal = 0,
                                                            Optional ByVal Latitudine As Decimal = 0,
                                                            Optional ByVal Area As Decimal = 0,
                                                            Optional ByVal CA_Sipi As String = "",
                                                            Optional ByVal AT_Prevalente As String = "",
                                                            Optional ByVal Forma_Possesso As String = "",
                                                            Optional ByVal TitoloPossesso As Integer = 0,
                                                            Optional ByVal Cod_TipoCentro As Integer = 0,
                                                            Optional ByVal Sup_Totale As Decimal = 0,
                                                            Optional ByVal Sup_Bosco As Decimal = 0,
                                                            Optional ByVal Sup_Tare As Decimal = 0,
                                                            Optional ByVal Sup_SAU As Decimal = 0,
                                                            Optional ByVal Sup_Prati As Decimal = 0,
                                                            Optional ByVal Sup_SAU_Convenzionale As Decimal = 0,
                                                            Optional ByVal Sup_SAU_Conversione As Decimal = 0,
                                                            Optional ByVal Sup_SAU_Biologico As Decimal = 0,
                                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                                            Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                                            Optional ByVal inviato As String = "",
                                                            Optional ByVal dataInvio As String = "",
                                                            Optional ByVal Data_Creazione As String = "",
                                                            Optional ByVal Data_Modifica As String = "",
                                                            Optional ByVal Username_Creazione As String = "",
                                                            Optional ByVal Username_Modifica As String = "",
                                                            Optional ByVal Validazione As String = "",
                                                            Optional ByVal Data_Validazione As String = "",
                                                            Optional ByVal UserName_Validazione As String = "",
                                                            Optional ByVal Documento As String = "",
                                                            Optional ByVal dataDocumento As String = "",
                                                            Optional ByVal flagLegale As String = "",
                                                            Optional ByVal flagPrincipale As String = "",
                                                            Optional ByVal fonte As String = "",
                                                            Optional ByVal fonteDescr As String = "",
                                                            Optional ByVal dataFonte As String = ""
                                                            ) As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("CentroAziendale")

        'Imposto gli attributi
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        NodoXml.SetAttribute("piva", Piva)
        NodoXml.SetAttribute("sa_cod", CStr(Sa_Cod))
        NodoXml.SetAttribute("sa_nome", Sa_Nome)

        NodoXml.SetAttribute("x", CStr(X))
        NodoXml.SetAttribute("y", CStr(Y))
        NodoXml.SetAttribute("zslm", CStr(ZSLM))
        NodoXml.SetAttribute("long", CStr(Longitudine))
        NodoXml.SetAttribute("lat", CStr(Latitudine))
        NodoXml.SetAttribute("area", CStr(Area))
        NodoXml.SetAttribute("ca_sipi", CA_Sipi)
        NodoXml.SetAttribute("at_prevalente", AT_Prevalente)
        NodoXml.SetAttribute("forma_possesso", Forma_Possesso)
        NodoXml.SetAttribute("titolopossesso", CStr(TitoloPossesso))
        NodoXml.SetAttribute("tipo", CStr(Cod_TipoCentro))
        NodoXml.SetAttribute("sup_totale", CStr(Sup_Totale))
        NodoXml.SetAttribute("sup_bosco", CStr(Sup_Bosco))
        NodoXml.SetAttribute("sup_tare", CStr(Sup_Tare))
        NodoXml.SetAttribute("sup_sau", CStr(Sup_SAU))
        NodoXml.SetAttribute("sup_prati", CStr(Sup_Prati))
        NodoXml.SetAttribute("sup_sau_convenzionale", CStr(Sup_SAU_Convenzionale))
        NodoXml.SetAttribute("sup_sau_conversione", CStr(Sup_SAU_Conversione))
        NodoXml.SetAttribute("sup_sau_biologico", CStr(Sup_SAU_Biologico))
        NodoXml.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        NodoXml.SetAttribute("basecode", CStr(BaseCode))
        NodoXml.SetAttribute("topcode", CStr(TopCode))
        NodoXml.SetAttribute("inviato", CStr(inviato))
        NodoXml.SetAttribute("dataInvio", CStr(dataInvio))
        NodoXml.SetAttribute("Data_Creazione", CStr(Data_Creazione))
        NodoXml.SetAttribute("Data_Modifica", CStr(Data_Modifica))
        NodoXml.SetAttribute("Username_Creazione", CStr(Username_Creazione))
        NodoXml.SetAttribute("Username_Modifica", CStr(Username_Modifica))
        NodoXml.SetAttribute("Validazione", CStr(Validazione))
        NodoXml.SetAttribute("Data_Validazione", CStr(Data_Validazione))
        NodoXml.SetAttribute("UserName_Validazione", CStr(UserName_Validazione))
        NodoXml.SetAttribute("Documento", CStr(Documento))
        NodoXml.SetAttribute("dataDocumento", CStr(dataDocumento))
        NodoXml.SetAttribute("flagLegale", CStr(flagLegale))
        NodoXml.SetAttribute("flagPrincipale", CStr(flagPrincipale))
        NodoXml.SetAttribute("fonte", CStr(fonte))
        NodoXml.SetAttribute("fonteDescr", CStr(fonteDescr))
        NodoXml.SetAttribute("dataFonte", CStr(dataFonte))

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
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        NodoXml.SetAttribute(LCase("piva"), Piva)
        NodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        NodoXml.SetAttribute(LCase("fabbricato_cod"), CStr(Fabbricato_Cod))
        NodoXml.SetAttribute(LCase("id_cod"), CStr(Id_Cod))
        NodoXml.SetAttribute(LCase("val_cod"), CStr(Val_Cod))
        NodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
        NodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))

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
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        NodoXml.SetAttribute("piva", Piva)
        NodoXml.SetAttribute("sa_cod", CStr(Sa_Cod))
        NodoXml.SetAttribute("fabbricato_cod", CStr(Fabbricato_Cod))
        NodoXml.SetAttribute("fabbricato_des", Fabbricato_Des)

        NodoXml.SetAttribute("indirizzo_cod", CStr(Indirizzo_Cod))
        NodoXml.SetAttribute("tipo_fabbricato_cod", CStr(Tipo_Fabbricato_Cod))
        NodoXml.SetAttribute("prov", CStr(Prov))
        NodoXml.SetAttribute("com", CStr(Com))
        NodoXml.SetAttribute("sezione", CStr(Sezione))
        NodoXml.SetAttribute("foglio", CStr(Foglio))
        NodoXml.SetAttribute("numero", CStr(Numero))
        NodoXml.SetAttribute("subalterno", Subalterno)

        NodoXml.SetAttribute("mc_convenzionale", CStr(MC_Convenzionale))
        NodoXml.SetAttribute("mc_conversione", CStr(MC_Conversione))
        NodoXml.SetAttribute("mc_biologico", CStr(MC_Biologico))
        NodoXml.SetAttribute("regolamento_cod", CStr(Regolamento_Cod))
        NodoXml.SetAttribute("titolopossesso", CStr(TitoloPossesso))
        NodoXml.SetAttribute("conversione_inizio", Format(Conversione_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute("conversione_fine", Format(Conversione_Fine, "dd/MM/yyyy"))

        NodoXml.SetAttribute("idoneo_costruzione", CStr(Idoneo_Costruzione))
        NodoXml.SetAttribute("idoneo_separazambienti", CStr(Idoneo_SeparazAmbienti))
        NodoXml.SetAttribute("idoneo_separazprodotti", CStr(Idoneo_SeparazProdotti))
        NodoXml.SetAttribute("idoneo_condigieniche", CStr(Idoneo_CondIgieniche))
        NodoXml.SetAttribute("idoneo_autorizsanitaria", CStr(Idoneo_AutorizSanitaria))
        NodoXml.SetAttribute("idoneo_haccp", CStr(Idoneo_HACCP))
        NodoXml.SetAttribute("idoneo_planimetria", CStr(Idoneo_Planimetria))
        NodoXml.SetAttribute("idoneo_layout", CStr(Idoneo_LayOut))
        NodoXml.SetAttribute("idoneo_diagrammiflusso", CStr(Idoneo_DiagrammiFlusso))
        NodoXml.SetAttribute("idoneo_cdx_m004", CStr(Idoneo_CDX_M004))
        NodoXml.SetAttribute(LCase("Idoneo_SupMinCoperte"), CStr(Idoneo_SupMinCoperte))
        NodoXml.SetAttribute(LCase("Idoneo_SupMinScoperte"), CStr(Idoneo_SupMinScoperte))

        NodoXml.SetAttribute("mq_convenzionale", CStr(MQ_Convenzionale))
        NodoXml.SetAttribute("mq_conversione", CStr(MQ_Conversione))
        NodoXml.SetAttribute("mq_biologico", CStr(MQ_Biologico))
        NodoXml.SetAttribute("mq_convenzionale_scoperto", CStr(MQ_Convenzionale_Scoperto))
        NodoXml.SetAttribute("mq_conversione_scoperto", CStr(MQ_Conversione_Scoperto))
        NodoXml.SetAttribute("mq_biologico_scoperto", CStr(MQ_Biologico_Scoperto))
        NodoXml.SetAttribute("n_piani", CStr(N_Piani))
        NodoXml.SetAttribute("sup_piano", CStr(Sup_Piano))

        NodoXml.SetAttribute("num_autorizzazione", CStr(Num_Autorizzazione))
        NodoXml.SetAttribute("data_richiesta_autorizzazione", Format(Data_Richiesta_Autorizzazione, "dd/MM/yyyy"))
        NodoXml.SetAttribute("tipologia_utilizzo", CStr(Tipologia_Utilizzo))

        NodoXml.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        NodoXml.SetAttribute("basecode", CStr(BaseCode))
        NodoXml.SetAttribute("topcode", CStr(TopCode))

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function



    '##########################################################################################
    'Viene chiamata quando si deve creare l'xml di un solo contatto
    'crea il nodo DatiContatti
    'e poi chiama la funzione XML_2_Contatto che crea tutto il blocco del contatto (contatto, indirizzi, risorse umane, ecc)
    Public Function XML_2_Contatti(ByRef Log_Errori As String,
                                        ByRef XmlDoc As XmlDocument,
                                        ByVal BaseCode As Integer,
                                        ByVal TopCode As Integer,
                                        ByVal Flag_Contatto_is_ImpresaGIAS As Boolean,
                                        ByVal TipoOperazioneDB_Contatto As enum_TipoOperazioneDB,
                                        ByVal Piva As String,
                                        ByVal Cod_Contatto As String,
                                        ByVal DT_Indirizzi As DataTable,
                                        ByVal DT_RisUm As DataTable,
                                        Optional ByVal Sa_Cod As Integer = 0,
                                        Optional ByVal Id_CF As Integer = 0,
                                        Optional ByVal Rag_Soc As String = "",
                                        Optional ByVal Convenevoli As String = "",
                                        Optional ByVal Codice_Fiscale As String = "",
                                        Optional ByVal Tipo_Indirizzo_Default As Integer = 0,
                                        Optional ByVal Nome As String = "",
                                        Optional ByVal Cognome As String = "",
                                        Optional ByVal Data_Nascita As Date = AGRODATAINIZIO,
                                        Optional ByVal Sesso As String = "",
                                        Optional ByVal Cod_Contatto_Referente As String = "",
                                        Optional ByVal Validita_Inizio_Contatto As Date = AGRODATAINIZIO,
                                        Optional ByVal Validita_Fine_Contatto As Date = AGRODATAFINE,
                                        Optional ByVal Dt_Rubrica As DataTable = Nothing,
                                        Optional ByVal Dt_Codici As DataTable = Nothing,
                                        Optional ByVal Cod_Risum_Destinazione_Diversa As Integer = 0,
                                        Optional ByVal Tipo_Indirizzo_Default_Destinazione_Diversa As Integer = 0,
                                        Optional ByVal Tipo_Speditore As Integer = 0,
                                        Optional ByVal Tipo_Destinazione As Integer = 0,
                                        Optional ByVal Agente_Cod As Integer = 0,
                                        Optional ByVal Provvigione As Integer = 0,
                                        Optional ByVal Note As String = "",
                                        Optional ByVal Id_Gestione_Note As Integer = 0,
                                        Optional ByVal Note2 As String = "",
                                        Optional ByVal Note_Operazioni As String = "",
                                        Optional ByVal Note2_Operazioni As String = "",
                                        Optional ByVal Fido As Integer = 0,
                                        Optional ByVal Limite_Posizioni As Integer = 0,
                                        Optional ByVal Limite_Giorni_Evasione As Integer = 0,
                                        Optional ByVal Orari_Ritiro As String = "",
                                        Optional ByVal Filtro_Rimborsi As String = "",
                                        Optional ByVal Vettore_Cod As Integer = 0,
                                        Optional ByVal CapoArea_Cod As Integer = 0,
                                        Optional ByVal Provvigione_CapoArea As Integer = 0,
                                        Optional ByVal Documento As String = "",
                                        Optional ByVal dtDocumento As DateTime = Nothing,
                                        Optional ByVal dtFonte As DateTime = Nothing,
                                        Optional ByVal flagReferente As String = "",
                                        Optional ByVal fonte As String = "",
                                        Optional ByVal fonteDescr As String = "",
                                        Optional ByVal nrBadge As String = "",
                                        Optional ByVal Memo As String = Nothing,
                                        Optional ByVal DT_Liquidita As DataTable = Nothing,
                                        Optional ByVal Sconto_Testo As String = Nothing,
                                        Optional ByVal Modalita_Fatturazione As Integer? = Nothing,
                                        Optional ByVal Cod_Iva_Contatto As Integer? = Nothing,
                                        Optional ByVal DT_Conti As DataTable = Nothing,
                                        Optional ByVal ChkFittizio As Boolean? = Nothing,
                                        Optional ByVal Cod_Conto_Economico_Default As Integer? = Nothing,
                                        Optional ByVal Cod_Conto_Patrimoniale_Default As Integer? = Nothing,
                                        Optional ByVal Nome_Breve As String = Nothing,
                                        Optional ByVal EUDR As Boolean? = Nothing
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

            If Not Flag_Contatto_is_ImpresaGIAS Then
                XmlDoc.AppendChild(XmlDatiContatti)
            End If


            '#######################################################
            '###################   CONTATTO    #####################
            '#######################################################

            XmlContatto = XML_2_Contatto(Log_Errori,
                                            XmlDoc,
                                            BaseCode,
                                            TopCode,
                                            Flag_Contatto_is_ImpresaGIAS,
                                            TipoOperazioneDB_Contatto,
                                            Piva,
                                            Cod_Contatto,
                                            DT_Indirizzi,
                                            DT_RisUm,
                                            Sa_Cod,
                                            Id_CF,
                                            Rag_Soc,
                                            Convenevoli,
                                            Codice_Fiscale,
                                            Tipo_Indirizzo_Default,
                                            Nome,
                                            Cognome,
                                            Data_Nascita,
                                            Sesso,
                                            Cod_Contatto_Referente,
                                            Validita_Inizio_Contatto,
                                            Validita_Fine_Contatto,
                                            Dt_Rubrica,
                                            Dt_Codici,
                                            Cod_Risum_Destinazione_Diversa,
                                            Tipo_Indirizzo_Default_Destinazione_Diversa,
                                            Tipo_Speditore,
                                             Tipo_Destinazione,
                                              Agente_Cod,
                                             Provvigione,
                                             Note,
                                             Id_Gestione_Note,
                                            Note2,
                                             Note_Operazioni,
                                             Note2_Operazioni,
                                             Fido,
                                            Limite_Posizioni,
                                             Limite_Giorni_Evasione,
                                             Orari_Ritiro,
                                             Filtro_Rimborsi,
                                             Vettore_Cod,
                                             CapoArea_Cod,
                                             Provvigione_CapoArea,
                                             nrBadge:=nrBadge,
                                             MEMO:=Memo,
                                             DT_Liquidita:=DT_Liquidita,
                                             Sconto_Testo:=Sconto_Testo,
                                             Cod_Iva_Contatto:=Cod_Iva_Contatto,
                                             Cod_Conto_Economico_Default:=Cod_Conto_Economico_Default,
                                             Cod_Conto_Patrimoniale_Default:=Cod_Conto_Patrimoniale_Default,
                                             Modalita_Fatturazione:=Modalita_Fatturazione,
                                             DT_Conti:=DT_Conti,
                                             ChkFittizio:=ChkFittizio,
                                             Nome_Breve:=Nome_Breve,
                                             EUDR:=EUDR
                                            )

            XmlDatiContatti.AppendChild(XmlContatto)



            '                   Modalita_Fatturazione:=Modalita_Fatturazione,


        Catch ex As Exception

            Log_Errori += ex.Message

        End Try


        Return XmlDatiContatti


    End Function





    '##########################################################################################
    'crea tutto il blocco del contatto (contatto, indirizzi, risorse umane, ecc)
    'se si deve inserire un contatto solo, conviene chiamare XML_2_Contatti che crea il nodo raccoglitore, prima di chiamare questa funzione
    'altrimenti, se si devono inserire più contatti, questa funzione può essere chiamata tante volte quanti sono i contatti da inserire
    Public Function XML_2_Contatto(ByRef Log_Errori As String,
                                     ByRef XmlDoc As XmlDocument,
                                     ByVal BaseCode As Integer,
                                     ByVal TopCode As Integer,
                                     ByVal Flag_Contatto_is_ImpresaGIAS As Boolean,
                                     ByVal TipoOperazioneDB_Contatto As enum_TipoOperazioneDB,
                                     ByVal Piva As String,
                                     ByVal Cod_Contatto As String,
                                     ByVal DT_Indirizzi As DataTable,
                                     ByVal DT_RisUm As DataTable,
                                     Optional ByVal Sa_Cod As Integer = 0,
                                     Optional ByVal Id_CF As Integer = 0,
                                     Optional ByVal Rag_Soc As String = "",
                                     Optional ByVal Convenevoli As String = "",
                                     Optional ByVal Codice_Fiscale As String = "",
                                     Optional ByVal Tipo_Indirizzo_Default As Integer = 0,
                                     Optional ByVal Nome As String = "",
                                     Optional ByVal Cognome As String = "",
                                     Optional ByVal Data_Nascita As Date = AGRODATAINIZIO,
                                     Optional ByVal Sesso As String = "",
                                     Optional ByVal Cod_Contatto_Referente As String = "",
                                     Optional ByVal Validita_Inizio_Contatto As Date = AGRODATAINIZIO,
                                     Optional ByVal Validita_Fine_Contatto As Date = AGRODATAFINE,
                                     Optional ByVal Dt_Rubrica As DataTable = Nothing,
                                     Optional ByVal Dt_Codici As DataTable = Nothing,
                                     Optional ByVal Cod_Risum_Destinazione_Diversa As Integer = 0,
                                     Optional ByVal Tipo_Indirizzo_Default_Destinazione_Diversa As Integer = 0,
                                     Optional ByVal Tipo_Speditore As Integer = 0,
                                     Optional ByVal Tipo_Destinazione As Integer = 0,
                                     Optional ByVal Agente_Cod As Integer = 0,
                                     Optional ByVal Provvigione As Integer = 0,
                                     Optional ByVal Note As String = "",
                                     Optional ByVal Id_Gestione_Note As Integer = 0,
                                     Optional ByVal Note2 As String = "",
                                     Optional ByVal Note_Operazioni As String = "",
                                     Optional ByVal Note2_Operazioni As String = "",
                                     Optional ByVal Fido As Integer = 0,
                                     Optional ByVal Limite_Posizioni As Integer = 0,
                                     Optional ByVal Limite_Giorni_Evasione As Integer = 0,
                                     Optional ByVal Orari_Ritiro As String = "",
                                     Optional ByVal Filtro_Rimborsi As String = "",
                                     Optional ByVal Vettore_Cod As Integer = 0,
                                     Optional ByVal CapoArea_Cod As Integer = 0,
                                     Optional ByVal Provvigione_CapoArea As Integer = 0,
                                     Optional ByVal Documento As String = "",
                                     Optional ByVal dtDocumento As DateTime = Nothing,
                                     Optional ByVal dtFonte As DateTime = Nothing,
                                     Optional ByVal flagReferente As String = "",
                                     Optional ByVal fonte As String = "",
                                     Optional ByVal fonteDescr As String = "",
                                     Optional ByVal nrBadge As String = "",
                                     Optional ByVal MEMO As String = "",
                                     Optional ByVal AlboProfessionale As String = "",
                                     Optional ByVal AlboProfessionaleDescr As String = "",
                                     Optional ByVal numeroIscrizione As Nullable(Of Integer) = Nothing,
                                     Optional ByVal Qualifica As String = "",
                                     Optional ByVal QualificaDescr As String = "",
                                     Optional ByVal TitoloStudio As String = "",
                                     Optional ByVal TitoloStudioDescr As String = "",
                                     Optional ByVal Modalita_Fatturazione As Integer? = Nothing,
                                     Optional ByVal DT_Liquidita As DataTable = Nothing,
                                     Optional ByVal Sconto_Testo As String = Nothing,
                                     Optional ByVal Cod_Iva_Contatto As Integer? = Nothing,
                                     Optional ByVal DT_Conti As DataTable = Nothing,
                                     Optional ByVal ChkFittizio As Boolean? = Nothing,
                                     Optional ByVal Cod_Conto_Economico_Default As Integer? = Nothing,
                                     Optional ByVal Cod_Conto_Patrimoniale_Default As Integer? = Nothing,
                                     Optional ByVal Nome_Breve As String = Nothing,
                                     Optional ByVal EUDR As Boolean? = Nothing
                                     ) As XmlElement


        'Dim XmlDatiContatti As System.Xml.XmlElement
        Dim XmlContatto As System.Xml.XmlElement
        Dim XmlContattoCodice As System.Xml.XmlElement
        Dim XmlIndirizzo As System.Xml.XmlElement
        Dim XmlRubrica As System.Xml.XmlElement
        Dim XmlDatiProdottiCosti As System.Xml.XmlElement
        Dim XmlContattoCosto As XmlElement
        'Dim XmlDatiLiquidita As System.Xml.XmlElement
        Dim XmlLiquidita As XmlElement
        Dim XmlConto As XmlElement
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

            XmlContatto = XML_2_Contatto_Contatto(TipoOperazioneDB_Contatto,
                                                    Piva,
                                                    Cod_Contatto,
                                                    XmlDoc,
                                                    Sa_Cod,
                                                    Id_CF,
                                                    Rag_Soc,
                                                    Convenevoli,
                                                    Codice_Fiscale,
                                                    Tipo_Indirizzo_Default,
                                                    Nome,
                                                    Cognome,
                                                    Data_Nascita,
                                                    Sesso,
                                                    Cod_Contatto_Referente,
                                                    Validita_Inizio_Contatto,
                                                    Validita_Fine_Contatto,
                                                    BaseCode,
                                                    TopCode,
                                                    Cod_Risum_Destinazione_Diversa,
                                                    Tipo_Indirizzo_Default_Destinazione_Diversa,
                                                    Tipo_Speditore,
                                                    Tipo_Destinazione,
                                                    Agente_Cod,
                                                    Provvigione,
                                                    Note,
                                                    Id_Gestione_Note,
                                                    Note2,
                                                    Note_Operazioni,
                                                    Note2_Operazioni,
                                                    Fido,
                                                    Limite_Posizioni,
                                                    Limite_Giorni_Evasione,
                                                    Orari_Ritiro,
                                                    Filtro_Rimborsi,
                                                    Vettore_Cod,
                                                    CapoArea_Cod,
                                                    Provvigione_CapoArea,
                                                    Documento,
                                                    dtDocumento,
                                                    dtFonte,
                                                    flagReferente,
                                                    fonte,
                                                    fonteDescr,
                                                    AlboProfessionale,
                                                    AlboProfessionaleDescr,
                                                    numeroIscrizione,
                                                    Qualifica,
                                                    QualificaDescr,
                                                    TitoloStudio,
                                                    TitoloStudioDescr,
                                                    nrBadge,
                                                    MEMO,
                                                    Modalita_Fatturazione,
                                                    Sconto_Testo:=Sconto_Testo,
                                                    Cod_Iva_Contatto:=Cod_Iva_Contatto,
                                                    Cod_Conto_Economico_Default:=Cod_Conto_Economico_Default,
                                                    Cod_Conto_Patrimoniale_Default:=Cod_Conto_Patrimoniale_Default,
                                                    ChkFittizio:=ChkFittizio,
                                                    Nome_Breve:=Nome_Breve,
                                                    EUDR:=EUDR
                                                    )

            '#######################################################
            '##################   INDIRIZZO    #####################
            '#######################################################

            'ATTENZIONE! Gli indirizzi vanno inseriti solo per i contatti 
            'che non sono imprese GIAS (poiché questi hanno i record in ImpreseXIndirizzi)
            If Not Flag_Contatto_is_ImpresaGIAS Then

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
                Dim SiglaProvincia As String
                Dim CodiceLingua As String = Nothing
                Dim TipoOperazioneDB_Indirizzo As enum_TipoOperazioneDB
                Dim Citta_Des As String


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
                        Pro_Cod_Istat = DT_Indirizzi.Rows(i).Item("Pro_Cod_Istat").ToString()
                        Com_Cod_Istat = DT_Indirizzi.Rows(i).Item("Com_Cod_Istat").ToString()
                        Cod_Indirizzo = DT_Indirizzi.Rows(i).Item("Cod_Indirizzo")
                        Ind_Des = DT_Indirizzi.Rows(i).Item("Ind_Des")
                        Frz_Des = DT_Indirizzi.Rows(i).Item("Frz_Des")
                        CAP = DT_Indirizzi.Rows(i).Item("CAP")
                        Stato = DT_Indirizzi.Rows(i).Item("Stato")
                        Note_Indirizzo = DT_Indirizzi.Rows(i).Item("Note")
                        Validita_Inizio_Indirizzo = DT_Indirizzi.Rows(i).Item("Validita_Inizio")
                        Validita_Fine_Indirizzo = DT_Indirizzi.Rows(i).Item("Validita_Fine")
                        SiglaProvincia = DT_Indirizzi.Rows(i).Item("Sigla_Prov")
                        If EsisteColonna(DT_Indirizzi, "Codice_Lingua") Then
                            CodiceLingua = DT_Indirizzi.Rows(i).Item("Codice_Lingua")
                        End If
                        Citta_Des = DT_Indirizzi.Rows(i).Item("Citta_Des").ToString()


                        XmlIndirizzo = XML_2_Indirizzo(TipoOperazioneDB_Indirizzo,
                                                        Tipo_Indirizzo,
                                                        Pro_Cod_Istat,
                                                        Com_Cod_Istat,
                                                        XmlDoc,
                                                        Cod_Indirizzo,
                                                        Ind_Des,
                                                        Frz_Des,
                                                        CAP,
                                                        Stato,
                                                        Note_Indirizzo,
                                                        Validita_Inizio_Indirizzo,
                                                        Validita_Fine_Indirizzo,
                                                        BaseCode,
                                                        TopCode, SiglaProvincia, CodiceLingua, Citta_Des)

                        XmlContatto.AppendChild(XmlIndirizzo)

                    Next

                    If Numero_Indirizzi_Fittizi > 0 Then

                        'va gestito il corretto utilizzo del tipo indirizzo
                        'e del tipo operazione
                        If TipoOperazioneDB_Indirizzo = enum_TipoOperazioneDB.Scrittura Then
                            Tipo_Indirizzo = 0
                            Pro_Cod_Istat = "000"
                            Com_Cod_Istat = "000"
                            Cod_Indirizzo = 0
                            Ind_Des = ""
                            Frz_Des = ""
                            CAP = ""
                            Stato = "IT"
                            Note_Indirizzo = ""
                            Validita_Inizio_Indirizzo = AGRODATAINIZIO
                            Validita_Fine_Indirizzo = AGRODATAFINE
                            Citta_Des = ""

                            For i = 0 To Numero_Indirizzi_Fittizi - 1

                                'inserisco degli indirizzi fittizi

                                XmlIndirizzo = XML_2_Indirizzo(TipoOperazioneDB_Indirizzo,
                                                         Tipo_Indirizzo,
                                                         Pro_Cod_Istat,
                                                         Com_Cod_Istat,
                                                         XmlDoc,
                                                         Cod_Indirizzo,
                                                         Ind_Des,
                                                         Frz_Des,
                                                         CAP,
                                                         Stato,
                                                         Note_Indirizzo,
                                                         Validita_Inizio_Indirizzo,
                                                         Validita_Fine_Indirizzo,
                                                         BaseCode,
                                                         TopCode, , ,
                                                         Citta_Des)

                                XmlContatto.AppendChild(XmlIndirizzo)

                            Next

                            Log_Errori &= "Sono stati inseriti " & CStr(Numero_Indirizzi_Fittizi) & " indirizzi vuoti."

                        End If

                    End If


                Else

                    For i = 1 To 4

                        'inserisco degli indirizzi fittizi

                        XmlIndirizzo = XML_2_Indirizzo(TipoOperazioneDB_Indirizzo,
                                                   Tipo_Indirizzo,
                                                   Pro_Cod_Istat,
                                                   Com_Cod_Istat,
                                                   XmlDoc,
                                                   Cod_Indirizzo,
                                                   Ind_Des,
                                                   Frz_Des,
                                                   CAP,
                                                   Stato,
                                                   Note_Indirizzo,
                                                   Validita_Inizio_Indirizzo,
                                                   Validita_Fine_Indirizzo,
                                                   BaseCode,
                                                   TopCode,,,
                                                   Citta_Des)

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

                    XmlRubrica = XML_2_Rubrica(TipoOperazioneDB_Rubrica,
                                               Cod_Rubrica,
                                                Numero,
                                                Descrizione,
                                                XmlDoc,
                                                Validita_Inizio_Rubrica,
                                                Validita_Fine_Rubrica,
                                                BaseCode,
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


                    XmlContattoCodice = XML_2_Contatto_Codice(TipoOperazioneDB_Codice,
                                                                Piva,
                                                                Cod_Contatto,
                                                                Id_Cod,
                                                                BaseCode,
                                                                TopCode,
                                                                XmlDoc,
                                                                Sa_Cod,
                                                                Val_Cod,
                                                                Validita_Inizio_Codice,
                                                                Validita_Fine_Codice)

                    XmlContatto.AppendChild(XmlContattoCodice)

                Next

            End If



            '#######################################################
            '##################   LIQUIDITA    #####################
            '#######################################################
            If Not IsNothing(DT_Liquidita) AndAlso DT_Liquidita.Rows.Count <> 0 Then

                Dim Validita_Inizio_Liquidita As Date
                Dim Validita_Fine_Liquidita As Date
                Dim TipoOperazioneDB_Liquidita As enum_TipoOperazioneDB


                For i = 0 To DT_Liquidita.Rows.Count - 1
                    TipoOperazioneDB_Liquidita = DT_Liquidita.Rows(i).Item("TipoOperazioneDB")
                    Validita_Inizio_Liquidita = DT_Liquidita.Rows(i).Item("Validita_Inizio")
                    Validita_Fine_Liquidita = DT_Liquidita.Rows(i).Item("Validita_Fine")

                    XmlLiquidita = XML_2_Liquidita(
                                BaseCode, TopCode,
                                TipoOperazioneDB_Liquidita,
                                DT_Liquidita.Rows(i).Item("Piva"),
                                DT_Liquidita.Rows(i).Item("Sa_Cod"),
                                DT_Liquidita.Rows(i).Item("Cod_Liquidita"),
                                DT_Liquidita.Rows(i).Item("Riferimento"),
                                DT_Liquidita.Rows(i).Item("Numero"),
                                DT_Liquidita.Rows(i).Item("Abi"),
                                DT_Liquidita.Rows(i).Item("Cab"),
                                DT_Liquidita.Rows(i).Item("Interbancario"),
                                0, 0,
                                DT_Liquidita.Rows(i).Item("Cau_Risorsa"),
                                DT_Liquidita.Rows(i).Item("Cod_Istituto"),
                                "0",
                                0,
                                DT_Liquidita.Rows(i).Item("Note"), 0, AGRODATAINIZIO,
                                "", "",
                                Validita_Inizio_Liquidita,
                                Validita_Fine_Liquidita,
                                DT_Liquidita.Rows(i).Item("Cin"),
                                DT_Liquidita.Rows(i).Item("Cifre_Controllo"),
                                DT_Liquidita.Rows(i).Item("Nazione"),
                                 DT_Liquidita.Rows(i).Item("Bic"),
                                  DT_Liquidita.Rows(i).Item("Cod_Contatto"),
                                  0, AGRODATAINIZIO,
                                0,
                                DT_Liquidita.Rows(i).Item("ChkDefault"),
                                DT_Liquidita.Rows(i).Item("ChkAbilitazione"),
                                "", "", "", "", "", "", "", "", "", "", "", "",
                                XmlDoc)

                    XmlContatto.AppendChild(XmlLiquidita)
                Next


            End If

            ' TODO
            'XmlDatiLiquidita = XmlDoc.CreateElement("DatiLiquidita")

            'XmlContatto.AppendChild(XmlDatiLiquidita)


            '#######################################################
            '###################    CONTI    #######################
            '#######################################################

            If Not IsNothing(DT_Conti) AndAlso DT_Conti.Rows.Count <> 0 Then

                Dim Validita_Inizio_Conto As Date
                Dim Validita_Fine_Conto As Date
                Dim TipoOperazioneDB_Conto As enum_TipoOperazioneDB

                For i = 0 To DT_Conti.Rows.Count - 1
                    TipoOperazioneDB_Conto = DT_Conti.Rows(i).Item("TipoOperazioneDB")
                    Validita_Inizio_Conto = DT_Conti.Rows(i).Item("Validita_Inizio")
                    Validita_Fine_Conto = DT_Conti.Rows(i).Item("Validita_Fine")

                    XmlConto = XML_2_Conto(
                           BaseCode, TopCode,
                           TipoOperazioneDB_Conto,
                           DT_Conti.Rows(i).Item("Piva"),
                           DT_Conti.Rows(i).Item("Cod_Conto"),
                           DT_Conti.Rows(i).Item("Cod_Contatto"),
                           Validita_Inizio_Conto, Validita_Fine_Conto,
                            XmlDoc)

                    XmlContatto.AppendChild(XmlConto)
                Next

            End If

            'XmlDatiConti = XmlDoc.CreateElement("DatiConti")

            'XmlContatto.AppendChild(XmlDatiConti)


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
                Dim Qualifica_Cod As Integer = 0
                Dim Mansione_Cod As Integer = 0
                Dim Classificazione_Cod As Integer = 0
                Dim Info_Famiglia As String = ""
                Dim Cod_Iva_Contatto_rs As Integer = -1
                Dim Cod_Conto_Econ As Integer = 0
                Dim Cod_Conto_Pat As Integer = 0

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
                    Qualifica_Cod = DT_RisUm.Rows(i).Item("Qualifica_Cod")
                    Mansione_Cod = DT_RisUm.Rows(i).Item("Mansione_Cod")
                    Classificazione_Cod = DT_RisUm.Rows(i).Item("Classificazione_Cod")
                    Info_Famiglia = DT_RisUm.Rows(i).Item("Info_Famiglia")
                    Cod_Iva_Contatto_rs = DT_RisUm.Rows(i).Item("Cod_Iva_Contatto")
                    Cod_Conto_Econ = DT_RisUm.Rows(i).Item("Cod_Conto_Econ")
                    Cod_Conto_Pat = DT_RisUm.Rows(i).Item("Cod_Conto_Pat")

                    XmlRisorseUmane = XML_2_Contatto_RisorsaUmana(TipoOperazioneDB_RisUm,
                                                                    Piva,
                                                                    Cod_Contatto,
                                                                    BaseCode,
                                                                    TopCode,
                                                                    XmlDoc,
                                                                    Sa_Cod,
                                                                    Cod_Risum,
                                                                    Cod_Rapporto,
                                                                    Settore_Des,
                                                                    Attivita_Des,
                                                                    Occasionale,
                                                                    Ore_Settimanali,
                                                                    Giorni_Ferie,
                                                                    Ferie_Godute,
                                                                    Giorni_Malattia,
                                                                    Patentino,
                                                                    Data_Rilascio_Patentino,
                                                                    Data_Scadenza_Patentino,
                                                                    Cod_RisUm_Origine,
                                                                    Piva_SuperUser_Origine,
                                                                    Validita_Inizio_RisUm,
                                                                    Validita_Fine_RisUm,
                                                                    Ente_di_rilascio,
                                                                    ChkSpesometro,
                                                                    Saldo_Iniziale_Crediti,
                                                                    Saldo_Iniziale_Debiti,
                                                                    Qualifica_Cod,
                                                                    Mansione_Cod,
                                                                    Classificazione_Cod,
                                                                    Info_Famiglia,
                                                                    Cod_Iva_Contatto:=Cod_Iva_Contatto_rs,
                                                                    Cod_Conto_Econ:=Cod_Conto_Econ,
                                                                    Cod_Conto_Pat:=Cod_Conto_Pat
                                                                    )

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

                            If InserisciPrezzo Then
                                XmlContattoCosto = XML_ProdottiCosti(TipoOperazioneDB_PC,
                                                                        Id,
                                                                        Piva,
                                                                        Riferimento,
                                                                        Elem_Cod,
                                                                        Pro_Cod,
                                                                        Mat_Cod,
                                                                        Udm_Cod,
                                                                        Mezzo,
                                                                        Prezzo_Unitario,
                                                                        Veg_Cod,
                                                                        Cul_Cod,
                                                                        XmlDoc,
                                                                        Validita_Inizio_PC,
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
    Public Function XML_2_Contatto_Contatto(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                            ByVal Piva As String,
                                            ByVal Cod_Contatto As String,
                                            Optional ByRef XmlDoc As XmlDocument = Nothing,
                                            Optional ByVal Sa_Cod As Integer = 0,
                                            Optional ByVal Id_CF As Integer = 0,
                                            Optional ByVal Rag_Soc As String = "",
                                            Optional ByVal Convenevoli As String = "",
                                            Optional ByVal Codice_Fiscale As String = "",
                                            Optional ByVal Tipo_Indirizzo_Default As Integer = 0,
                                            Optional ByVal Nome As String = "",
                                            Optional ByVal Cognome As String = "",
                                            Optional ByVal Data_Nascita As Date = AGRODATAINIZIO,
                                            Optional ByVal Sesso As String = "",
                                            Optional ByVal Cod_Contatto_Referente As String = "",
                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                            Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                            Optional ByVal BaseCode As Integer = 0,
                                            Optional ByVal TopCode As Integer = 200000000,
                                            Optional ByVal Cod_Risum_Destinazione_Diversa As Integer = 0,
                                            Optional ByVal Tipo_Indirizzo_Default_Destinazione_Diversa As Integer = 0,
                                             Optional ByVal Tipo_Speditore As Integer = 0,
                                             Optional ByVal Tipo_Destinazione As Integer = 0,
                                             Optional ByVal Agente_Cod As Integer = 0,
                                             Optional ByVal Provvigione As Double = 0,
                                             Optional ByVal Note As String = "",
                                             Optional ByVal Id_Gestione_Note As Integer = 0,
                                             Optional ByVal Note2 As String = "",
                                             Optional ByVal Note_Operazioni As String = "",
                                             Optional ByVal Note2_Operazioni As String = "",
                                             Optional ByVal Fido As Double = 0,
                                             Optional ByVal Limite_Posizioni As Integer = 0,
                                             Optional ByVal Limite_Giorni_Evasione As Double = 0,
                                             Optional ByVal Orari_Ritiro As String = "",
                                             Optional ByVal Filtro_Rimborsi As String = "",
                                             Optional ByVal Vettore_Cod As Integer = 0,
                                             Optional ByVal CapoArea_Cod As Integer = 0,
                                             Optional ByVal Provvigione_CapoArea As Double = 0,
                                             Optional ByVal Documento As String = "",
                                            Optional ByVal dtDocumento As DateTime = Nothing,
                                            Optional ByVal dtFonte As DateTime = Nothing,
                                            Optional ByVal flagReferente As String = "",
                                            Optional ByVal fonte As String = "",
                                            Optional ByVal fonteDescr As String = "",
                                            Optional ByVal AlboProfessionale As String = "",
                                            Optional ByVal AlboProfessionaleDescr As String = "",
                                            Optional ByVal numeroIscrizione As Nullable(Of Integer) = Nothing,
                                            Optional ByVal Qualifica As String = "",
                                            Optional ByVal QualificaDescr As String = "",
                                            Optional ByVal TitoloStudio As String = "",
                                            Optional ByVal TitoloStudioDescr As String = "",
                                            Optional ByVal nrBadge As String = "",
                                            Optional ByVal MEMO As String = "",
                                            Optional ByVal Modalita_Fatturazione As Integer? = Nothing,
                                            Optional ByVal Sconto_Testo As String = Nothing,
                                            Optional ByVal Cod_Iva_Contatto As Integer? = Nothing,
                                            Optional ByVal ChkFittizio As Boolean? = Nothing,
                                            Optional ByVal Cod_Conto_Economico_Default As Integer? = Nothing,
                                            Optional ByVal Cod_Conto_Patrimoniale_Default As Integer? = Nothing,
                                            Optional ByVal Nome_Breve As String = Nothing,
                                            Optional ByVal EUDR As Boolean? = Nothing
                                            ) As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Contatto")

        'Imposto gli attributi
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        NodoXml.SetAttribute(LCase("piva"), CStr(Piva))
        NodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        NodoXml.SetAttribute(LCase("cod_contatto"), CStr(Cod_Contatto))
        NodoXml.SetAttribute(LCase("id_cf"), CStr(Id_CF))
        NodoXml.SetAttribute(LCase("rag_soc"), CStr(Rag_Soc))
        NodoXml.SetAttribute(LCase("convenevoli"), CStr(Convenevoli))
        NodoXml.SetAttribute(LCase("codice_fiscale"), CStr(Codice_Fiscale))
        NodoXml.SetAttribute(LCase("Tipo_Indirizzo_Default"), CStr(Tipo_Indirizzo_Default))

        NodoXml.SetAttribute(LCase("Cod_Risum_Destinazione_Diversa"), CStr(Cod_Risum_Destinazione_Diversa))
        NodoXml.SetAttribute(LCase("Tipo_Indirizzo_Default_Destinazione_Diversa"), CStr(Tipo_Indirizzo_Default_Destinazione_Diversa))

        'nuovi attributi aggiunti il 02/11/2009
        NodoXml.SetAttribute(LCase("nome"), Nome)
        NodoXml.SetAttribute(LCase("cognome"), Cognome)
        NodoXml.SetAttribute(LCase("data_nascita"), CStr(Data_Nascita))
        NodoXml.SetAttribute(LCase("sesso"), Sesso)
        NodoXml.SetAttribute(LCase("cod_contatto_referente"), Cod_Contatto_Referente)

        'nuovi attributi aggiunti il 09/12/2015
        NodoXml.SetAttribute(LCase("Tipo_Speditore"), Tipo_Speditore)
        NodoXml.SetAttribute(LCase("Tipo_Destinazione"), Tipo_Destinazione)
        NodoXml.SetAttribute(LCase("Agente_Cod"), Agente_Cod)
        NodoXml.SetAttribute(LCase("Provvigione"), Provvigione)
        NodoXml.SetAttribute(LCase("Note"), Note)
        NodoXml.SetAttribute(LCase("Id_Gestione_Note"), Id_Gestione_Note)
        NodoXml.SetAttribute(LCase("Note2"), Note2)
        NodoXml.SetAttribute(LCase("Note_Operazioni"), Note_Operazioni)
        NodoXml.SetAttribute(LCase("Note2_Operazioni"), Note2_Operazioni)
        NodoXml.SetAttribute(LCase("Fido"), Fido)
        NodoXml.SetAttribute(LCase("Limite_Posizioni"), Limite_Posizioni)
        NodoXml.SetAttribute(LCase("Limite_Giorni_Evasione"), Limite_Giorni_Evasione)
        NodoXml.SetAttribute(LCase("Orari_Ritiro"), Orari_Ritiro)
        NodoXml.SetAttribute(LCase("Filtro_Rimborsi"), Filtro_Rimborsi)
        NodoXml.SetAttribute(LCase("Vettore_Cod"), Vettore_Cod)
        NodoXml.SetAttribute(LCase("CapoArea_Cod"), CapoArea_Cod)
        NodoXml.SetAttribute(LCase("Provvigione_CapoArea"), Provvigione_CapoArea)
        NodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
        NodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))

        NodoXml.SetAttribute(LCase("Documento"), CStr(Documento))
        NodoXml.SetAttribute(LCase("dtDocumento"), CStr(dtDocumento))
        NodoXml.SetAttribute(LCase("dtFonte"), CStr(dtFonte))
        NodoXml.SetAttribute(LCase("flagReferente"), CStr(flagReferente))
        NodoXml.SetAttribute(LCase("fonte"), CStr(fonte))
        NodoXml.SetAttribute(LCase("fonteDescr"), CStr(fonteDescr))

        'questi non sono gestiti, roba di drudi???
        'Optional ByVal AlboProfessionale As String = "",
        'Optional ByVal AlboProfessionaleDescr As String = "",
        'Optional ByVal numeroIscrizione As Nullable(Of Integer) = Nothing,
        'Optional ByVal Qualifica As String = "",
        'Optional ByVal QualificaDescr As String = "",
        'Optional ByVal TitoloStudio As String = "",
        'Optional ByVal TitoloStudioDescr As String = "",

        NodoXml.SetAttribute(LCase("nrBadge"), CStr(nrBadge))
        NodoXml.SetAttribute(LCase("MEMO"), CStr(MEMO))

        If Modalita_Fatturazione IsNot Nothing Then
            NodoXml.SetAttribute(LCase("Modalita_Fatturazione"), CStr(Modalita_Fatturazione))
        End If

        If Sconto_Testo IsNot Nothing Then
            NodoXml.SetAttribute(LCase("Sconto_Testo"), CStr(Sconto_Testo))
        End If

        If Cod_Iva_Contatto IsNot Nothing Then
            NodoXml.SetAttribute(LCase("Cod_Iva_Contatto"), CStr(Cod_Iva_Contatto))
        End If

        If Cod_Conto_Economico_Default IsNot Nothing Then
            NodoXml.SetAttribute(LCase("Cod_Conto_Economico_Default"), CStr(Cod_Conto_Economico_Default))
        End If

        If Cod_Conto_Patrimoniale_Default IsNot Nothing Then
            NodoXml.SetAttribute(LCase("Cod_Conto_Patrimoniale_Default"), CStr(Cod_Conto_Patrimoniale_Default))
        End If

        If ChkFittizio IsNot Nothing Then
            NodoXml.SetAttribute(LCase("ChkFittizio"), CStr(ChkFittizio))
        End If

        If Nome_Breve IsNot Nothing Then
            NodoXml.SetAttribute(LCase("Nome_Breve"), CStr(Nome_Breve))
        End If

        If EUDR IsNot Nothing Then
            NodoXml.SetAttribute(LCase("EUDR"), CStr(EUDR))
        End If

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function


    '##########################################################################################
    Public Function XML_2_Contatto_Codice(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                            ByVal Piva As String,
                                            ByVal Cod_Contatto As String,
                                            ByVal Id_Cod As Integer,
                                            ByVal BaseCode As Integer,
                                            ByVal TopCode As Integer,
                                            Optional ByRef XmlDoc As XmlDocument = Nothing,
                                            Optional ByVal Sa_Cod As Integer = 0,
                                            Optional ByVal Val_Cod As String = "",
                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
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
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        NodoXml.SetAttribute(LCase("piva"), CStr(Piva))
        NodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        NodoXml.SetAttribute(LCase("cod_contatto"), CStr(Cod_Contatto))
        NodoXml.SetAttribute(LCase("id_cod"), CStr(Id_Cod))
        NodoXml.SetAttribute(LCase("val_cod"), CStr(Val_Cod))
        NodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
        NodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function


    '##########################################################################################
    Public Function XML_ProdottiCosti(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                        ByVal Id As Integer,
                                        ByVal Piva As String,
                                        ByVal Riferimento As String,
                                        ByVal Elem_Cod As Integer,
                                        ByVal Pro_Cod As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Udm_Cod As Integer,
                                        ByVal Mezzo As Integer,
                                        ByVal Prezzo_Unitario As Decimal,
                                        ByVal Veg_Cod As Integer,
                                        ByVal Cul_Cod As Integer,
                                        Optional ByRef XmlDoc As XmlDocument = Nothing,
                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
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
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        NodoXml.SetAttribute(LCase("id"), CStr(Id))
        NodoXml.SetAttribute(LCase("piva"), CStr(Piva))
        NodoXml.SetAttribute(LCase("riferimento"), CStr(Riferimento))
        NodoXml.SetAttribute(LCase("elem_cod"), CStr(Elem_Cod))
        NodoXml.SetAttribute(LCase("pro_cod"), CStr(Pro_Cod))
        NodoXml.SetAttribute(LCase("mat_cod"), CStr(Mat_Cod))
        NodoXml.SetAttribute(LCase("udm_cod"), CStr(Udm_Cod))
        NodoXml.SetAttribute(LCase("mezzo"), CStr(Mezzo))
        NodoXml.SetAttribute(LCase("prezzo_unitario"), CStr(Prezzo_Unitario))
        NodoXml.SetAttribute(LCase("veg_cod"), CStr(Veg_Cod))
        NodoXml.SetAttribute(LCase("cul_cod"), CStr(Cul_Cod))
        NodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))


        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function



    '##########################################################################################
    'i corrispettivi sono salvati nella tabella prodotti_costi
    Public Function XML_2_Contatto_RisorsaUmana(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                ByVal Piva As String,
                                                ByVal Cod_Contatto As String,
                                                ByVal BaseCode As Integer,
                                                ByVal TopCode As Integer,
                                                Optional ByRef XmlDoc As XmlDocument = Nothing,
                                                Optional ByVal Sa_Cod As Integer = 0,
                                                Optional ByVal Cod_Risum As Integer = 0,
                                                Optional ByVal Cod_Rapporto As Integer = 0,
                                                Optional ByVal Settore_Des As String = "",
                                                Optional ByVal Attivita_Des As String = "",
                                                Optional ByVal Occasionale As Integer = 0,
                                                Optional ByVal Ore_Settimanali As Decimal = 0,
                                                Optional ByVal Giorni_Ferie As Integer = 0,
                                                Optional ByVal Ferie_Godute As Integer = 0,
                                                Optional ByVal Giorni_Malattia As Integer = 0,
                                                Optional ByVal Patentino As String = "",
                                                Optional ByVal Data_Rilascio_Patentino As Date = AGRODATAINIZIO,
                                                Optional ByVal Data_Scadenza_Patentino As Date = AGRODATAFINE,
                                                Optional ByVal Cod_RisUm_Origine As Integer = 0,
                                                Optional ByVal Piva_SuperUser_Origine As String = "",
                                                Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                                Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                                Optional ByVal Ente_di_rilascio As String = "",
                                                Optional ByVal ChkSpesometro As Integer = 0,
                                                Optional ByVal Saldo_Iniziale_Crediti As Decimal = 0,
                                                Optional ByVal Saldo_Iniziale_Debiti As Decimal = 0,
                                                Optional ByVal Qualifica_Cod As Integer = 0,
                                                Optional ByVal Mansione_Cod As Integer = 0,
                                                Optional ByVal Classificazione_Cod As Integer = 0,
                                                Optional ByVal Info_Famiglia As String = "",
                                                Optional ByVal Cod_Iva_Contatto As Integer = -1,
                                                Optional ByVal Cod_Conto_Econ As Integer = 0,
                                                Optional ByVal Cod_Conto_Pat As Integer = 0) _
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
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        NodoXml.SetAttribute(LCase("piva"), CStr(Piva))
        NodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        NodoXml.SetAttribute(LCase("cod_contatto"), CStr(Cod_Contatto))
        NodoXml.SetAttribute(LCase("cod_risum"), CStr(Cod_Risum))
        NodoXml.SetAttribute(LCase("cod_rapporto"), CStr(Cod_Rapporto))
        NodoXml.SetAttribute(LCase("settore_des"), CStr(Settore_Des))
        NodoXml.SetAttribute(LCase("attivita_des"), CStr(Attivita_Des))
        NodoXml.SetAttribute(LCase("occasionale"), CStr(Occasionale))
        NodoXml.SetAttribute(LCase("corrispettivo_mensile"), CStr(0))
        NodoXml.SetAttribute(LCase("corrispettivo_orario"), CStr(0))
        NodoXml.SetAttribute(LCase("ore_settimanali"), CStr(Ore_Settimanali))
        NodoXml.SetAttribute(LCase("giorni_ferie"), CStr(Giorni_Ferie))
        NodoXml.SetAttribute(LCase("ferie_godute"), CStr(Ferie_Godute))
        NodoXml.SetAttribute(LCase("giorni_malattia"), CStr(Giorni_Malattia))
        NodoXml.SetAttribute(LCase("patentino"), CStr(Patentino))
        NodoXml.SetAttribute(LCase("data_rilascio_patentino"), Format(Data_Rilascio_Patentino, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("data_scadenza_patentino"), Format(Data_Scadenza_Patentino, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("Ente_di_rilascio"), CStr(Ente_di_rilascio))
        NodoXml.SetAttribute(LCase("ChkSpesometro"), CStr(ChkSpesometro))
        NodoXml.SetAttribute(LCase("Saldo_Iniziale_Crediti"), CStr(Saldo_Iniziale_Crediti))
        NodoXml.SetAttribute(LCase("Saldo_Iniziale_Debiti"), CStr(Saldo_Iniziale_Debiti))
        NodoXml.SetAttribute(LCase("cod_risum_origine"), CStr(Cod_RisUm_Origine))
        NodoXml.SetAttribute(LCase("piva_superuser_origine"), CStr(Piva_SuperUser_Origine))
        NodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
        NodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))
        NodoXml.SetAttribute(LCase("Qualifica_Cod"), CStr(Qualifica_Cod))
        NodoXml.SetAttribute(LCase("Mansione_Cod"), CStr(Mansione_Cod))
        NodoXml.SetAttribute(LCase("Classificazione_Cod"), CStr(Classificazione_Cod))
        NodoXml.SetAttribute(LCase("Info_Famiglia"), CStr(Info_Famiglia))
        NodoXml.SetAttribute(LCase("Cod_Iva_Contatto"), CStr(Cod_Iva_Contatto))
        NodoXml.SetAttribute(LCase("Cod_Conto_Econ"), CStr(Cod_Conto_Econ))
        NodoXml.SetAttribute(LCase("Cod_Conto_Pat"), CStr(Cod_Conto_Pat))

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function



    '##########################################################################################
    'il cap, il comune e la provincia sono ridondanti
    'vengono letti nelle tabelle del metaschema (istat e lista_province)
    Public Function XML_2_Indirizzo(ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                    ByVal Tipo_Indirizzo As Integer,
                                    ByVal Pro_Cod_Istat As String,
                                    ByVal Com_Cod_Istat As String,
                                    Optional ByRef XmlDoc As XmlDocument = Nothing,
                                    Optional ByVal Cod_Indirizzo As Integer = 0,
                                    Optional ByVal Ind_Des As String = "",
                                    Optional ByVal Frz_Des As String = "",
                                    Optional ByVal CAP As String = "",
                                    Optional ByVal Stato As String = "IT",
                                    Optional ByVal Note As String = "",
                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                    Optional ByVal BaseCode As Integer = 0,
                                    Optional ByVal TopCode As Integer = 200000000,
                                    Optional ByVal SiglaProvincia As String = "",
                                    Optional ByVal Codice_Lingua As String = Nothing,
                                    Optional ByVal Citta_Des As String = "") As XmlElement


        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Indirizzo")

        'Imposto gli attributi
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        NodoXml.SetAttribute("tipo_indirizzo", CStr(Tipo_Indirizzo))
        NodoXml.SetAttribute("cod_indirizzo", CStr(Cod_Indirizzo))
        NodoXml.SetAttribute("ind_des", Ind_Des)
        NodoXml.SetAttribute("frz_des", Frz_Des)
        'NodoXml.SetAttribute("com_des", "")
        NodoXml.SetAttribute("pro_cod", SiglaProvincia)
        NodoXml.SetAttribute("stato", Stato)
        NodoXml.SetAttribute("note", Note)
        NodoXml.SetAttribute("cap", CAP)
        NodoXml.SetAttribute("pro_cod_istat", Pro_Cod_Istat)
        NodoXml.SetAttribute("com_cod_istat", Com_Cod_Istat)
        NodoXml.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        NodoXml.SetAttribute("basecode", CStr(BaseCode))
        NodoXml.SetAttribute("topcode", CStr(TopCode))
        If Not IsNothing(Codice_Lingua) Then
            NodoXml.SetAttribute("codice_lingua", CStr(Codice_Lingua))
        End If
        NodoXml.SetAttribute("Citta_Des", Citta_Des)
        If (Stato <> "IT" AndAlso Citta_Des <> "") Then
            NodoXml.SetAttribute("frz_des", Citta_Des)
        End If


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
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        NodoXml.SetAttribute("cod_rubrica", CStr(Cod_Rubrica))
        NodoXml.SetAttribute("numero", Numero)
        NodoXml.SetAttribute("descr", Descrizione)
        NodoXml.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        NodoXml.SetAttribute("basecode", CStr(BaseCode))
        NodoXml.SetAttribute("topcode", CStr(TopCode))


        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function

    '##########################################################################################
    Public Function XML_2_Stalla(
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Int32,
                                ByVal STA_NUM As Int32,
                                ByVal Sta_Des As String,
                                ByVal Ausl_Cod As String,
                                ByVal Dat_Costr As Date,
                                ByVal Dat_Chiu As Date,
                                ByVal Cod_Fabb As String,
                                ByVal Gen_Cod As Int32,
                                ByVal Spe_Cod As Int32,
                                ByVal Ipro_Cod As Int32,
                                ByVal X As String,
                                ByVal Y As String,
                                ByVal Dat_Ult_Agg As Date,
                                ByVal Latitudine As Int32,
                                ByVal Longitudine As Int32,
                                ByVal CUAA_Proprietario As String,
                                ByVal Denominazione_Proprietario As String,
                                ByVal CUAA_Detentore As String,
                                ByVal Denominazione_Detentore As String,
                                Optional ByRef XmlDoc As XmlDocument = Nothing,
                                Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                Optional ByVal AziendaCodice As String = "",
                                Optional ByVal allevamentoDescr As String = "",
                                Optional ByVal codAllevamento As String = "",
                                Optional ByVal speCodice As String = "",
                                Optional ByVal ID_Utente As String = "",
                                Optional ByVal DT_Variazione As DateTime = Nothing,
                                Optional ByVal flag_Libri_Gen As String = "",
                                Optional ByVal Autorizzazione_Latte As String = "",
                                Optional ByVal Data_ultimo_Censimento As DateTime = Nothing,
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

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("Piva"), CStr(Piva))
            .SetAttribute(LCase("Sa_Cod"), CStr(Sa_Cod))
            .SetAttribute(LCase("STA_NUM"), CStr(STA_NUM))
            .SetAttribute(LCase("Sta_Des"), CStr(Sta_Des))
            .SetAttribute(LCase("Ausl_Cod"), CStr(Ausl_Cod))
            .SetAttribute(LCase("Dat_Costr"), Format(Dat_Costr, "dd/MM/yyyy"))
            .SetAttribute(LCase("Dat_Chiu"), Format(Dat_Chiu, "dd/MM/yyyy"))
            .SetAttribute(LCase("Cod_Fabb"), CStr(Cod_Fabb))
            .SetAttribute(LCase("Gen_Cod"), CStr(Gen_Cod))
            .SetAttribute(LCase("Spe_Cod"), CStr(Spe_Cod))
            .SetAttribute(LCase("Ipro_Cod"), CStr(Ipro_Cod))
            .SetAttribute(LCase("X"), CStr(X))
            .SetAttribute(LCase("Y"), CStr(Y))
            .SetAttribute(LCase("Dat_Ult_Agg"), Format(Dat_Ult_Agg, "dd/MM/yyyy"))
            .SetAttribute(LCase("Latitudine"), CStr(Latitudine))
            .SetAttribute(LCase("Longitudine"), CStr(Longitudine))
            .SetAttribute(LCase("CUAA_Proprietario"), CStr(CUAA_Proprietario))
            .SetAttribute(LCase("Denominazione_Proprietario"), CStr(Denominazione_Proprietario))
            .SetAttribute(LCase("CUAA_Detentore"), CStr(CUAA_Detentore))
            .SetAttribute(LCase("Denominazione_Detentore"), CStr(Denominazione_Detentore))
            .SetAttribute(LCase("Validita_Inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
            .SetAttribute(LCase("Validita_Fine"), Format(Validita_Fine, "dd/MM/yyyy"))
            .SetAttribute(LCase("AziendaCodice"), CStr(AziendaCodice))
            .SetAttribute(LCase("AllevamentoDescr"), CStr(allevamentoDescr))
            .SetAttribute(LCase("codAllevamento"), CStr(codAllevamento))
            .SetAttribute(LCase("speCodice"), CStr(speCodice))
            .SetAttribute(LCase("ID_Utente"), CStr(ID_Utente))
            .SetAttribute(LCase("DT_Variazione"), String.Format("{0:d/M/yyyy HH:mm:ss}", DT_Variazione))
            .SetAttribute(LCase("Flag_Libri_Gen"), CStr(flag_Libri_Gen))
            .SetAttribute(LCase("Autorizzazione_Latte"), CStr(Autorizzazione_Latte))
            .SetAttribute(LCase("Data_Ultimo_Censimento"), String.Format("{0:d/M/yyyy HH:mm:ss}", Data_ultimo_Censimento))
            .SetAttribute(LCase("Capi_Totali"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(capi_totali))

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
    Public Function XML_MateriePrime(ByRef Log_Errori As String,
                                        ByRef XmlDoc As XmlDocument,
                                        ByVal BaseCode As Integer,
                                        ByVal TopCode As Integer,
                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                        ByVal TipoOperazioneDB_Report As enum_TipoOperazioneDB,
                                        ByVal TipoOperazioneDB_LottoConf As enum_TipoOperazioneDB,
                                        ByVal TipoOperazioneDB_ProdCosti As enum_TipoOperazioneDB,
                                        ByVal TipoOperazioneDB_Dettagli As enum_TipoOperazioneDB,
                                        ByVal TipoOperazioneDB_ParamQual As enum_TipoOperazioneDB,
                                        ByVal Piva_SuperUser As String,
                                        ByVal Piva As String,
                                        ByVal Elem_Cod As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Cod_Articolo As String,
                                        ByVal Mat_Des As String,
                                        Optional ByVal DT_MP_Report As DataTable = Nothing,
                                        Optional ByVal DT_MP_LottoConf As DataTable = Nothing,
                                        Optional ByVal DT_MP_ProdCosti As DataTable = Nothing,
                                        Optional ByVal DT_MP_Dettagli As DataTable = Nothing,
                                        Optional ByVal DT_MP_ParamQual As DataTable = Nothing,
                                        Optional ByVal Sa_Cod As Integer = 0,
                                        Optional ByVal Sem_Cod As Integer = 0,
                                        Optional ByVal Veg_Cod As Integer = 0,
                                        Optional ByVal Cul_Cod As Integer = 0,
                                        Optional ByVal Cal_Cod As Integer = 0,
                                        Optional ByVal Grva_Cod_Veg As Integer = 0,
                                        Optional ByVal Grfi_Cod As Integer = 0,
                                        Optional ByVal Trap_Dur As Integer = 0,
                                        Optional ByVal Uso As Integer = 0,
                                        Optional ByVal ClToss_Cod As String = "",
                                        Optional ByVal NewClToss_Cod As String = "",
                                        Optional ByVal N As Decimal = 0,
                                        Optional ByVal P2O5 As Decimal = 0,
                                        Optional ByVal K2O As Decimal = 0,
                                        Optional ByVal MgO As Decimal = 0,
                                        Optional ByVal Ditta_Cod As Integer = 0,
                                        Optional ByVal Prezzo_Unitario As Decimal = 0,
                                        Optional ByVal Regolamento As Integer = 0,
                                        Optional ByVal Flag_Convenzionale As Integer = 0,
                                        Optional ByVal Flag_Biologico As Integer = 0,
                                        Optional ByVal Flag_NonAgricolo As Integer = 0,
                                        Optional ByVal Flag_AusiliareFabbricazione As Integer = 0,
                                        Optional ByVal Gen_Cod As Integer = 0,
                                        Optional ByVal Spe_Cod As Integer = 0,
                                        Optional ByVal Raz_Cod As Integer = 0,
                                        Optional ByVal Ipro_Cod As Integer = 0,
                                        Optional ByVal Cat_Cod As Integer = 0,
                                        Optional ByVal ChkImballaggio As Integer = 0,
                                        Optional ByVal ChkListino As Integer = 0,
                                        Optional ByVal Taglio As Integer = 0,
                                        Optional ByVal Flag_Extra As Integer = 0,
                                        Optional ByVal Udm_Cod_Extra As Integer = 0,
                                        Optional ByVal Qta_Extra As Decimal = 0,
                                        Optional ByVal Note As String = "",
                                        Optional ByVal Mat_Cod_Origine As Integer = 0,
                                        Optional ByVal Piva_SuperUser_Origine As String = "",
                                        Optional ByVal Extra_Str As String = "",
                                        Optional ByVal Extra_Int As Integer = 0,
                                        Optional ByVal Extra_Date As Date = AGRODATAFINE,
                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                        Optional ByVal Codice_Esterno As String = "",
                                        Optional ByVal Flag_Importato As Int32 = 0
                                     ) As XmlElement


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

            XmlMateriaPrima = XML_MateriaPrima(Log_Errori,
                                                    XmlDoc,
                                                    BaseCode,
                                                    TopCode,
                                                    TipoOperazioneDB,
                                                    TipoOperazioneDB_Report,
                                                    TipoOperazioneDB_LottoConf,
                                                    TipoOperazioneDB_ProdCosti,
                                                    TipoOperazioneDB_Dettagli,
                                                    TipoOperazioneDB_ParamQual,
                                                    Piva_SuperUser,
                                                    Piva,
                                                    Elem_Cod,
                                                    Mat_Cod,
                                                    Cod_Articolo,
                                                    Mat_Des,
                                                    DT_MP_Report,
                                                    DT_MP_LottoConf,
                                                    DT_MP_ProdCosti,
                                                    DT_MP_Dettagli,
                                                    DT_MP_ParamQual,
                                                    Sa_Cod,
                                                    Sem_Cod,
                                                    Veg_Cod,
                                                    Cul_Cod,
                                                    Cal_Cod,
                                                    Grva_Cod_Veg,
                                                    Grfi_Cod,
                                                    Trap_Dur,
                                                    Uso,
                                                    ClToss_Cod,
                                                    NewClToss_Cod,
                                                    N,
                                                    P2O5,
                                                    K2O,
                                                    MgO,
                                                    Ditta_Cod,
                                                    Prezzo_Unitario,
                                                    Regolamento,
                                                    Flag_Convenzionale,
                                                    Flag_Biologico,
                                                    Flag_NonAgricolo,
                                                    Flag_AusiliareFabbricazione,
                                                    Gen_Cod,
                                                    Spe_Cod,
                                                    Raz_Cod,
                                                    Ipro_Cod,
                                                    Cat_Cod,
                                                    ChkImballaggio,
                                                    ChkListino,
                                                    Taglio,
                                                    Flag_Extra,
                                                    Udm_Cod_Extra,
                                                    Qta_Extra,
                                                    Note,
                                                    Mat_Cod_Origine,
                                                    Piva_SuperUser_Origine,
                                                    Extra_Str,
                                                    Extra_Int,
                                                    Extra_Date,
                                                    Validita_Inizio,
                                                    Validita_Fine,
                                                    Codice_Esterno,
                                                    Flag_Importato)

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
    Public Function XML_MateriaPrima(ByRef Log_Errori As String,
                                        ByRef XmlDoc As XmlDocument,
                                        ByVal BaseCode As Integer,
                                        ByVal TopCode As Integer,
                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                        ByVal TipoOperazioneDB_Report As enum_TipoOperazioneDB,
                                        ByVal TipoOperazioneDB_LottoConf As enum_TipoOperazioneDB,
                                        ByVal TipoOperazioneDB_ProdCosti As enum_TipoOperazioneDB,
                                        ByVal TipoOperazioneDB_Dettagli As enum_TipoOperazioneDB,
                                        ByVal TipoOperazioneDB_ParamQual As enum_TipoOperazioneDB,
                                        ByVal Piva_SuperUser As String,
                                        ByVal Piva As String,
                                        ByVal Elem_Cod As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Cod_Articolo As String,
                                        ByVal Mat_Des As String,
                                        Optional ByVal DT_MP_Report As DataTable = Nothing,
                                        Optional ByVal DT_MP_LottoConf As DataTable = Nothing,
                                        Optional ByVal DT_MP_ProdCosti As DataTable = Nothing,
                                        Optional ByVal DT_MP_Dettagli As DataTable = Nothing,
                                        Optional ByVal DT_MP_ParamQual As DataTable = Nothing,
                                        Optional ByVal Sa_Cod As Integer = 0,
                                        Optional ByVal Sem_Cod As Integer = 0,
                                        Optional ByVal Veg_Cod As Integer = 0,
                                        Optional ByVal Cul_Cod As Integer = 0,
                                        Optional ByVal Cal_Cod As Integer = 0,
                                        Optional ByVal Grva_Cod_Veg As Integer = 0,
                                        Optional ByVal Grfi_Cod As Integer = 0,
                                        Optional ByVal Trap_Dur As Integer = 0,
                                        Optional ByVal Uso As Integer = 0,
                                        Optional ByVal ClToss_Cod As String = "",
                                        Optional ByVal NewClToss_Cod As String = "",
                                        Optional ByVal N As Decimal = 0,
                                        Optional ByVal P2O5 As Decimal = 0,
                                        Optional ByVal K2O As Decimal = 0,
                                        Optional ByVal MgO As Decimal = 0,
                                        Optional ByVal Ditta_Cod As Integer = 0,
                                        Optional ByVal Prezzo_Unitario As Decimal = 0,
                                        Optional ByVal Regolamento As Integer = 0,
                                        Optional ByVal Flag_Convenzionale As Integer = 0,
                                        Optional ByVal Flag_Biologico As Integer = 0,
                                        Optional ByVal Flag_NonAgricolo As Integer = 0,
                                        Optional ByVal Flag_AusiliareFabbricazione As Integer = 0,
                                        Optional ByVal Gen_Cod As Integer = 0,
                                        Optional ByVal Spe_Cod As Integer = 0,
                                        Optional ByVal Raz_Cod As Integer = 0,
                                        Optional ByVal Ipro_Cod As Integer = 0,
                                        Optional ByVal Cat_Cod As Integer = 0,
                                        Optional ByVal ChkImballaggio As Integer = 0,
                                        Optional ByVal ChkListino As Integer = 0,
                                        Optional ByVal Taglio As Integer = 0,
                                        Optional ByVal Flag_Extra As Integer = 0,
                                        Optional ByVal Udm_Cod_Extra As Integer = 0,
                                        Optional ByVal Qta_Extra As Decimal = 0,
                                        Optional ByVal Note As String = "",
                                        Optional ByVal Mat_Cod_Origine As Integer = 0,
                                        Optional ByVal Piva_SuperUser_Origine As String = "",
                                        Optional ByVal Extra_Str As String = "",
                                        Optional ByVal Extra_Int As Integer = 0,
                                        Optional ByVal Extra_Date As Date = AGRODATAINIZIO,
                                        Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                        Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                     Optional ByVal Codice_Esterno As String = "",
                                        Optional ByVal Flag_Importato As Int32 = 0
                                     ) As XmlElement


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

            XmlMateriaPrima = XML_MateriaPrima_MateriaPrima(Log_Errori,
                                                    XmlDoc,
                                                    BaseCode,
                                                    TopCode,
                                                    TipoOperazioneDB,
                                                    Piva,
                                                    Elem_Cod,
                                                    Mat_Cod,
                                                    Cod_Articolo,
                                                    Mat_Des,
                                                    Sa_Cod,
                                                    Sem_Cod,
                                                    Veg_Cod,
                                                    Cul_Cod,
                                                    Cal_Cod,
                                                    Grva_Cod_Veg,
                                                    Grfi_Cod,
                                                    Trap_Dur,
                                                    Uso,
                                                    ClToss_Cod,
                                                    NewClToss_Cod,
                                                    N,
                                                    P2O5,
                                                    K2O,
                                                    MgO,
                                                    Ditta_Cod,
                                                    Prezzo_Unitario,
                                                    Regolamento,
                                                    Flag_Convenzionale,
                                                    Flag_Biologico,
                                                    Flag_NonAgricolo,
                                                    Flag_AusiliareFabbricazione,
                                                    Gen_Cod,
                                                    Spe_Cod,
                                                    Raz_Cod,
                                                    Ipro_Cod,
                                                    Cat_Cod,
                                                    ChkImballaggio,
                                                    ChkListino,
                                                    Taglio,
                                                    Flag_Extra,
                                                    Udm_Cod_Extra,
                                                    Qta_Extra,
                                                    Note,
                                                    Mat_Cod_Origine,
                                                    Piva_SuperUser_Origine,
                                                    Extra_Str,
                                                    Extra_Int,
                                                    Extra_Date,
                                                    Validita_Inizio,
                                                    Validita_Fine,
                                                    Codice_Esterno,
                                                    Flag_Importato)


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

                    XmlParametroQualitativo = XML_MateriaPrima_ParametroQualitativo(Log_Errori,
                                                                            XmlDoc,
                                                                            BaseCode,
                                                                            TopCode,
                                                                            TipoOperazioneDB_ParamQual,
                                                                            Piva,
                                                                            Mat_Cod,
                                                                            Sa_Cod,
                                                                            Tipo,
                                                                            Tipo_Cod,
                                                                            Udm_Cod,
                                                                            Valore_Des,
                                                                            Valore_Min,
                                                                            Valore_Max,
                                                                            ChkRegistri,
                                                                            ChkCalibri,
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
    Public Function XML_MateriaPrima_MateriaPrima(ByRef Log_Errori As String,
                                                    ByRef XmlDoc As XmlDocument,
                                                    ByVal BaseCode As Integer,
                                                    ByVal TopCode As Integer,
                                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                    ByVal Piva As String,
                                                    ByVal Elem_Cod As Integer,
                                                    ByVal Mat_Cod As Integer,
                                                    ByVal Cod_Articolo As String,
                                                    ByVal Mat_Des As String,
                                                    Optional ByVal Sa_Cod As Integer = 0,
                                                    Optional ByVal Sem_Cod As Integer = 0,
                                                    Optional ByVal Veg_Cod As Integer = 0,
                                                    Optional ByVal Cul_Cod As Integer = 0,
                                                    Optional ByVal Cal_Cod As Integer = 0,
                                                    Optional ByVal Grva_Cod_Veg As Integer = 0,
                                                    Optional ByVal Grfi_Cod As Integer = 0,
                                                    Optional ByVal Trap_Dur As Integer = 0,
                                                    Optional ByVal Uso As Integer = 0,
                                                    Optional ByVal ClToss_Cod As String = "",
                                                    Optional ByVal NewClToss_Cod As String = "",
                                                    Optional ByVal N As Decimal = 0,
                                                    Optional ByVal P2O5 As Decimal = 0,
                                                    Optional ByVal K2O As Decimal = 0,
                                                    Optional ByVal MgO As Decimal = 0,
                                                    Optional ByVal Ditta_Cod As Integer = 0,
                                                    Optional ByVal Prezzo_Unitario As Decimal = 0,
                                                    Optional ByVal Regolamento As Integer = 0,
                                                    Optional ByVal Flag_Convenzionale As Integer = 0,
                                                    Optional ByVal Flag_Biologico As Integer = 0,
                                                    Optional ByVal Flag_NonAgricolo As Integer = 0,
                                                    Optional ByVal Flag_AusiliareFabbricazione As Integer = 0,
                                                    Optional ByVal Gen_Cod As Integer = 0,
                                                    Optional ByVal Spe_Cod As Integer = 0,
                                                    Optional ByVal Raz_Cod As Integer = 0,
                                                    Optional ByVal Ipro_Cod As Integer = 0,
                                                    Optional ByVal Cat_Cod As Integer = 0,
                                                    Optional ByVal ChkImballaggio As Integer = 0,
                                                    Optional ByVal ChkListino As Integer = 0,
                                                    Optional ByVal Taglio As Integer = 0,
                                                    Optional ByVal Flag_Extra As Integer = 0,
                                                    Optional ByVal Udm_Cod_Extra As Integer = 0,
                                                    Optional ByVal Qta_Extra As Decimal = 0,
                                                    Optional ByVal Note As String = "",
                                                    Optional ByVal Mat_Cod_Origine As Integer = 0,
                                                    Optional ByVal Piva_SuperUser_Origine As String = "",
                                                    Optional ByVal Extra_Str As String = "",
                                                    Optional ByVal Extra_Int As Integer = 0,
                                                    Optional ByVal Extra_Date As Date = AGRODATAINIZIO,
                                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                                                    Optional ByVal Validita_Fine As Date = AGRODATAFINE,
                                                    Optional ByVal Codice_Esterno As String = "",
                                                    Optional ByVal Flag_Importato As Int32 = 0
                                                    ) As XmlElement

        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Materia_Prima")

        'Imposto gli attributi
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        NodoXml.SetAttribute(LCase("piva"), Piva)
        NodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        NodoXml.SetAttribute(LCase("elem_cod"), CStr(Elem_Cod))
        NodoXml.SetAttribute(LCase("mat_cod"), CStr(Mat_Cod))
        NodoXml.SetAttribute(LCase("cod_articolo"), Cod_Articolo)
        NodoXml.SetAttribute(LCase("mat_des"), Mat_Des)
        NodoXml.SetAttribute(LCase("Note"), Note)
        NodoXml.SetAttribute(LCase("regolamento"), CStr(Regolamento))

        NodoXml.SetAttribute(LCase("sem_cod"), CStr(Sem_Cod))
        NodoXml.SetAttribute(LCase("veg_cod"), CStr(Veg_Cod))
        NodoXml.SetAttribute(LCase("cul_cod"), CStr(Cul_Cod))
        NodoXml.SetAttribute(LCase("cal_cod"), CStr(Cal_Cod))
        NodoXml.SetAttribute(LCase("grva_cod_veg"), CStr(Grva_Cod_Veg))
        NodoXml.SetAttribute(LCase("grfi_cod"), CStr(Grfi_Cod))

        NodoXml.SetAttribute(LCase("gen_cod"), CStr(Gen_Cod))
        NodoXml.SetAttribute(LCase("spe_cod"), CStr(Spe_Cod))
        NodoXml.SetAttribute(LCase("raz_cod"), CStr(Raz_Cod))
        NodoXml.SetAttribute(LCase("ipro_cod"), CStr(Ipro_Cod))
        NodoXml.SetAttribute(LCase("cat_cod"), CStr(Cat_Cod))

        NodoXml.SetAttribute(LCase("trap_dur"), CStr(Trap_Dur))
        NodoXml.SetAttribute(LCase("uso"), CStr(Uso))
        NodoXml.SetAttribute(LCase("cltoss_cod"), CStr(ClToss_Cod))
        NodoXml.SetAttribute(LCase("newcltoss_cod"), CStr(NewClToss_Cod))
        NodoXml.SetAttribute(LCase("n"), CStr(N))
        NodoXml.SetAttribute(LCase("p2o5"), CStr(P2O5))
        NodoXml.SetAttribute(LCase("k2o"), CStr(K2O))
        NodoXml.SetAttribute(LCase("mgo"), CStr(MgO))
        NodoXml.SetAttribute(LCase("ditta_cod"), CStr(Ditta_Cod))

        NodoXml.SetAttribute(LCase("prezzo_unitario"), CStr(Prezzo_Unitario))
        NodoXml.SetAttribute(LCase("flag_convenzionale"), CStr(Flag_Convenzionale))
        NodoXml.SetAttribute(LCase("flag_biologico"), CStr(Flag_Biologico))
        NodoXml.SetAttribute(LCase("flag_nonagricolo"), CStr(Flag_NonAgricolo))
        NodoXml.SetAttribute(LCase("flag_ausiliarefabbricazione"), CStr(Flag_AusiliareFabbricazione))

        NodoXml.SetAttribute(LCase("ChkImballaggio"), CStr(ChkImballaggio))
        NodoXml.SetAttribute(LCase("ChkListino"), CStr(ChkListino))
        NodoXml.SetAttribute(LCase("Taglio"), CStr(Taglio))
        NodoXml.SetAttribute(LCase("Flag_Extra"), CStr(Flag_Extra))
        NodoXml.SetAttribute(LCase("Udm_Cod_Extra"), CStr(Udm_Cod_Extra))
        NodoXml.SetAttribute(LCase("Qta_Extra"), CStr(Qta_Extra))
        NodoXml.SetAttribute(LCase("Mat_Cod_Origine"), CStr(Mat_Cod_Origine))
        NodoXml.SetAttribute(LCase("Piva_SuperUser_Origine"), Piva_SuperUser_Origine)

        NodoXml.SetAttribute(LCase("Codice_Esterno"), Codice_Esterno)
        NodoXml.SetAttribute(LCase("Flag_Importato"), CStr(Flag_Importato))

        NodoXml.SetAttribute(LCase("extra_str"), Extra_Str)
        NodoXml.SetAttribute(LCase("extra_int"), CStr(Extra_Int))
        NodoXml.SetAttribute(LCase("extra_date"), CStr(Extra_Date))
        NodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
        NodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))


        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function



    '##########################################################################################
    Public Function XML_MateriaPrima_Dettagli(ByRef Log_Errori As String,
                                                    ByRef XmlDoc As XmlDocument,
                                                    ByVal BaseCode As Integer,
                                                    ByVal TopCode As Integer,
                                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                    ByVal Piva_SuperUser As String,
                                                    ByVal Piva As String,
                                                    ByVal Mat_Cod As Integer,
                                                    Optional ByVal Extra_SmallInt1 As Integer = 0,
                                                    Optional ByVal Extra_SmallInt2 As Integer = 0,
                                                    Optional ByVal Extra_SmallInt3 As Integer = 0,
                                                    Optional ByVal Extra_SmallInt4 As Integer = 0,
                                                    Optional ByVal Extra_SmallInt5 As Integer = 0,
                                                    Optional ByVal Extra_SmallInt6 As Integer = 0,
                                                    Optional ByVal Extra_Int1 As Integer = 0,
                                                    Optional ByVal Extra_Int2 As Integer = 0,
                                                    Optional ByVal Extra_Int3 As Integer = 0,
                                                    Optional ByVal Extra_Int4 As Integer = 0,
                                                    Optional ByVal Extra_Int5 As Integer = 0,
                                                    Optional ByVal Extra_Int6 As Integer = 0,
                                                    Optional ByVal Extra_Dbl1 As Decimal = 0,
                                                    Optional ByVal Extra_Dbl2 As Decimal = 0,
                                                    Optional ByVal Extra_Dbl3 As Decimal = 0,
                                                    Optional ByVal Extra_Dbl4 As Decimal = 0,
                                                    Optional ByVal Extra_Dbl5 As Decimal = 0,
                                                    Optional ByVal Extra_Dbl6 As Decimal = 0,
                                                    Optional ByVal Extra_Str1 As String = "",
                                                    Optional ByVal Extra_Str2 As String = "",
                                                    Optional ByVal Extra_Str3 As String = "",
                                                    Optional ByVal Extra_Str4 As String = "",
                                                    Optional ByVal Extra_Str5 As String = "",
                                                    Optional ByVal Extra_Str6 As String = "",
                                                    Optional ByVal Extra_Date1 As Date = AGRODATAINIZIO,
                                                    Optional ByVal Extra_Date2 As Date = AGRODATAINIZIO,
                                                    Optional ByVal Extra_Date3 As Date = AGRODATAINIZIO,
                                                    Optional ByVal Extra_Date4 As Date = AGRODATAINIZIO,
                                                    Optional ByVal Extra_Date5 As Date = AGRODATAINIZIO,
                                                    Optional ByVal Extra_Date6 As Date = AGRODATAINIZIO,
                                                    Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
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
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        NodoXml.SetAttribute(LCase("piva_superuser"), Piva_SuperUser)
        NodoXml.SetAttribute(LCase("piva"), Piva)
        NodoXml.SetAttribute(LCase("mat_cod"), CStr(Mat_Cod))

        NodoXml.SetAttribute("extra_smallint1", Extra_SmallInt1)
        NodoXml.SetAttribute("extra_smallint2", Extra_SmallInt2)
        NodoXml.SetAttribute("extra_smallint3", Extra_SmallInt3)
        NodoXml.SetAttribute("extra_smallint4", Extra_SmallInt4)
        NodoXml.SetAttribute("extra_smallint5", Extra_SmallInt5)
        NodoXml.SetAttribute("extra_smallint6", Extra_SmallInt6)

        NodoXml.SetAttribute("extra_int1", Extra_Int1)
        NodoXml.SetAttribute("extra_int2", Extra_Int2)
        NodoXml.SetAttribute("extra_int3", Extra_Int3)
        NodoXml.SetAttribute("extra_int4", Extra_Int4)
        NodoXml.SetAttribute("extra_int5", Extra_Int5)
        NodoXml.SetAttribute("extra_int6", Extra_Int6)

        NodoXml.SetAttribute("extra_dbl1", Extra_Dbl1)
        NodoXml.SetAttribute("extra_dbl2", Extra_Dbl2)
        NodoXml.SetAttribute("extra_dbl3", Extra_Dbl3)
        NodoXml.SetAttribute("extra_dbl4", Extra_Dbl4)
        NodoXml.SetAttribute("extra_dbl5", Extra_Dbl5)
        NodoXml.SetAttribute("extra_dbl6", Extra_Dbl6)

        NodoXml.SetAttribute("extra_str1", Extra_Str1)
        NodoXml.SetAttribute("extra_str2", Extra_Str2)
        NodoXml.SetAttribute("extra_str3", Extra_Str3)
        NodoXml.SetAttribute("extra_str4", Extra_Str4)
        NodoXml.SetAttribute("extra_str5", Extra_Str5)
        NodoXml.SetAttribute("extra_str6", Extra_Str6)

        NodoXml.SetAttribute("extra_date1", Extra_Date1)
        NodoXml.SetAttribute("extra_date2", Extra_Date2)
        NodoXml.SetAttribute("extra_date3", Extra_Date3)
        NodoXml.SetAttribute("extra_date4", Extra_Date4)
        NodoXml.SetAttribute("extra_date5", Extra_Date5)
        NodoXml.SetAttribute("extra_date6", Extra_Date6)

        NodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
        NodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))


        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing


    End Function




    '##########################################################################################
    Public Function XML_MateriaPrima_ParametroQualitativo(ByRef Log_Errori As String,
                                                            ByRef XmlDoc As XmlDocument,
                                                            ByVal BaseCode As Integer,
                                                            ByVal TopCode As Integer,
                                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                            ByVal Piva As String,
                                                            ByVal Mat_Cod As Integer,
                                                            Optional ByVal Sa_Cod As Integer = 0,
                                                            Optional ByVal Tipo As String = "",
                                                            Optional ByVal Tipo_Cod As Integer = 0,
                                                            Optional ByVal Udm_Cod As Integer = 0,
                                                            Optional ByVal Valore_Des As String = "",
                                                            Optional ByVal Valore_Min As Decimal = 0,
                                                            Optional ByVal Valore_Max As Decimal = 0,
                                                            Optional ByVal ChkRegistri As Integer = 0,
                                                            Optional ByVal ChkCalibri As Integer = 0,
                                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
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
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

        NodoXml.SetAttribute(LCase("piva"), Piva)
        NodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
        NodoXml.SetAttribute(LCase("mat_cod"), CStr(Mat_Cod))
        NodoXml.SetAttribute(LCase("tipo"), Tipo)
        NodoXml.SetAttribute(LCase("tipo_cod"), CStr(Tipo_Cod))
        NodoXml.SetAttribute(LCase("udm_cod"), CStr(Udm_Cod))

        NodoXml.SetAttribute(LCase("valore_des"), Valore_Des)
        NodoXml.SetAttribute(LCase("valore_min"), CStr(Valore_Min))
        NodoXml.SetAttribute(LCase("valore_max"), CStr(Valore_Max))
        NodoXml.SetAttribute(LCase("chkregistri"), CStr(ChkRegistri))
        NodoXml.SetAttribute(LCase("ChkCalibri"), CStr(ChkCalibri))

        NodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
        NodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
        NodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))


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
    Public Function XML_Appezzamenti(ByRef Log_Errori As String,
                                        ByRef XmlDoc As XmlDocument,
                                        ByVal BaseCode As Integer,
                                        ByVal TopCode As Integer,
                                        ByVal Flag_CreaImpianto As Boolean,
                                        ByVal DT_Codici_Appezzamento As DataTable,
                                        ByVal DT_Codici_Impianto As DataTable,
                                        ByVal DT_Codici_Progetto As DataTable,
                                        ByVal TipoOperazioneDB_Appezzamento As enum_TipoOperazioneDB,
                                        ByVal TipoOperazioneDB_Impianto As enum_TipoOperazioneDB,
                                        ByVal TipoOperazioneDB_Progetto As enum_TipoOperazioneDB,
                                        ByVal Piva_SuperUser As String,
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Appezza As Integer,
                                        ByVal Sup_App As Decimal,
                                        ByVal App_Nome As String,
                                        ByVal Campo_Cod As Integer,
                                        ByVal Validita_Inizio_Appezza As Date,
                                        ByVal Validita_Fine_Appezza As Date,
                                        ByVal Id_Reg As Integer,
                                        ByVal Id_Consociazione As Integer,
                                        ByVal Sup_Imp As Decimal,
                                        ByVal Cul_Cod As Integer,
                                        ByVal Validita_Inizio_Impianto As Date,
                                        ByVal Validita_Fine_Impianto As Date,
                                        ByVal Data As Date,
                                        ByVal Progetto_Cod As Integer,
                                        ByVal Progetto_Nome As String,
                                        ByVal Progetto_Des As String,
                                        ByVal Cau_Progetto As Integer,
                                        ByVal Validita_Inizio_Progetto As Date,
                                        ByVal Validita_Fine_Progetto As Date,
                                        Optional ByVal Data_App As String = "0",
                                        Optional ByVal Data_Inizio As String = "0",
                                        Optional ByVal Data_Fine As String = "0",
                                        Optional ByVal Ep_Camp As String = "0",
                                        Optional ByVal X As Decimal = 0,
                                        Optional ByVal Y As Decimal = 0,
                                        Optional ByVal Zslm As Decimal = 0,
                                        Optional ByVal Esposiz As String = "...",
                                        Optional ByVal Pende As Decimal = 0,
                                        Optional ByVal Ubicazione As String = "...",
                                        Optional ByVal Num_Del As Integer = 0,
                                        Optional ByVal Clas As String = "",
                                        Optional ByVal Sabbia As Decimal = 0,
                                        Optional ByVal Limo As Decimal = 0,
                                        Optional ByVal Argilla As Decimal = 0,
                                        Optional ByVal pH As Decimal = 0,
                                        Optional ByVal CalTot As Decimal = 0,
                                        Optional ByVal CalAtt As Decimal = 0,
                                        Optional ByVal SostOrg As Decimal = 0,
                                        Optional ByVal K2OAss As Decimal = 0,
                                        Optional ByVal P2O5Ass As Decimal = 0,
                                        Optional ByVal Mg As Decimal = 0,
                                        Optional ByVal Ntot As Decimal = 0,
                                        Optional ByVal Um_S As Decimal = 0,
                                        Optional ByVal Cl_Dren As String = "0",
                                        Optional ByVal Falda As Integer = 0,
                                        Optional ByVal CsC As Decimal = 0,
                                        Optional ByVal K2OAss_Data As String = "0",
                                        Optional ByVal MatOrg As Decimal = 0,
                                        Optional ByVal MatOrg_Data As String = "0",
                                        Optional ByVal NOtot_Data As String = "0",
                                        Optional ByVal NOtot As Decimal = 0,
                                        Optional ByVal P2O5Ass_Data As String = "0",
                                        Optional ByVal Suolo_CodAttri As String = "",
                                        Optional ByVal Campo_Spia As Integer = 0,
                                        Optional ByVal Campo_Spia_Area As Decimal = 0,
                                        Optional ByVal Cs_SIPI As String = "",
                                        Optional ByVal Prossimo As Integer = 0,
                                        Optional ByVal Blk_Flag As Integer = 0,
                                        Optional ByVal Blk_Inizio_Data As Date = AGRODATAINIZIO,
                                        Optional ByVal Blk_Inizio_Username As String = "",
                                        Optional ByVal Blk_Inizio_Note As String = "",
                                        Optional ByVal Blk_Fine_Data As Date = AGRODATAFINE,
                                        Optional ByVal Blk_Fine_Username As String = "",
                                        Optional ByVal Blk_Fine_Note As String = "",
                                        Optional ByVal Grva_Cod_Veg As Integer = 0,
                                        Optional ByVal Grfi_Cod As Integer = 0,
                                        Optional ByVal Cod_Resp As Integer = 0,
                                        Optional ByVal Cod_Ente As Integer = 0,
                                        Optional ByVal Campo_Spia_Impianto As Integer = 0,
                                        Optional ByVal Data_Raccolta As String = "0",
                                        Optional ByVal Produzione As Integer = 0,
                                        Optional ByVal ResaPrevista As Decimal = 0,
                                        Optional ByVal ResaEffettiva As Decimal = 0,
                                        Optional ByVal Scarto As Integer = 0,
                                        Optional ByVal Ind_Mat_Cod As Integer = 0,
                                        Optional ByVal Ind_Mat_Ril As String = "0",
                                        Optional ByVal Sta_Ter As String = "",
                                        Optional ByVal Cop_DI As String = "0",
                                        Optional ByVal Cop_DF As String = "0",
                                        Optional ByVal Tra_Fila As Decimal = 0,
                                        Optional ByVal Su_Fila As Decimal = 0,
                                        Optional ByVal P_HA As Decimal = 0,
                                        Optional ByVal Foral_Cod As Integer = -1,
                                        Optional ByVal Setup_Cod As String = "-1",
                                        Optional ByVal Port_Cod As Integer = -1,
                                        Optional ByVal Imp_Cod As Integer = -1,
                                        Optional ByVal Stru_Prot As Integer = 0,
                                        Optional ByVal Pro_Pag As Integer = 0,
                                        Optional ByVal Seme_Q As Integer = 0,
                                        Optional ByVal Seme_T As Integer = 0,
                                        Optional ByVal Seme_P As Integer = 0,
                                        Optional ByVal Seme_D As Integer = 0,
                                        Optional ByVal Stato_Residui As String = "",
                                        Optional ByVal Tecn_Cod As Integer = -1,
                                        Optional ByVal Denitrificazione As Integer = 0,
                                        Optional ByVal Volatilizzazione As Integer = 0,
                                        Optional ByVal ProfonditaLav As Integer = 0,
                                        Optional ByVal Id_Campo As Integer = 0,
                                        Optional ByVal Su_Cod As Integer = -1,
                                        Optional ByVal Cop_Cod As Integer = 0,
                                        Optional ByVal Cover As Integer = 0,
                                        Optional ByVal Monitorato As Integer = 0,
                                        Optional ByVal Codice_Ficale_Tecnico As String = "",
                                        Optional ByVal Regolamento As Integer = enum_Cod_Regolamento.Regolamento_Nessuno,
                                        Optional ByVal Finanziamento As Integer = 0,
                                        Optional ByVal Data_Conversione As String = "0",
                                        Optional ByVal ProvenienzaSeme As Integer = 0,
                                        Optional ByVal Cod_Contratto As Integer = 0,
                                        Optional ByVal Cod_Conto As Integer = 0,
                                        Optional ByVal Ricavi_Previsti As Decimal = 0,
                                        Optional ByVal Produzione_Prevista As Decimal = 0,
                                        Optional ByVal Giudizio As String = "",
                                        Optional ByVal Veg_Cod As Integer = 0,
                                        Optional ByVal Grfi_Cod_Progetto As Integer = 0,
                                        Optional ByVal CSProgetto_Cod As Integer = 0,
                                        Optional ByVal Stato_Impianto As enum_Stato_Impianto = enum_Stato_Impianto.Impianto_Produzione,
                                        Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno,
                                        Optional ByVal Disciplinare_Cod As Integer = 0,
                                        Optional ByVal Disciplinare_PrivatoPubblico As Integer = 0,
                                        Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0,
                                        Optional ByVal Data_Inizio_Prevista As Date = AGRODATAINIZIO,
                                        Optional ByVal Data_Fine_Prevista As Date = AGRODATAFINE,
                                        Optional ByVal via_stringa As String = "",
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

            XmlAppezzamento = XML_Appezzamento(Log_Errori,
                                                XmlDoc,
                                                BaseCode,
                                                TopCode,
                                                Flag_CreaImpianto,
                                                DT_Codici_Appezzamento,
                                                DT_Codici_Impianto,
                                                DT_Codici_Progetto,
                                                TipoOperazioneDB_Appezzamento,
                                                TipoOperazioneDB_Impianto,
                                                TipoOperazioneDB_Progetto,
                                                Piva_SuperUser,
                                                Piva,
                                                Sa_Cod,
                                                Appezza,
                                                Sup_App,
                                                App_Nome,
                                                Campo_Cod,
                                                Validita_Inizio_Appezza,
                                                Validita_Fine_Appezza,
                                                Id_Reg,
                                           Id_Consociazione,
                                           Sup_Imp,
                                           Cul_Cod,
                                           Validita_Inizio_Impianto,
                                           Validita_Fine_Impianto,
                                           Data,
                                           Progetto_Cod,
                                           Progetto_Nome,
                                           Progetto_Des,
                                           Cau_Progetto,
                                           Validita_Inizio_Progetto,
                                           Validita_Fine_Progetto,
                                                Data_App,
                                                Data_Inizio,
                                                Data_Fine,
                                                Ep_Camp,
                                                X,
                                                Y,
                                                Zslm,
                                                Esposiz,
                                                Pende,
                                                Ubicazione,
                                                Num_Del,
                                                Clas,
                                                Sabbia,
                                                Limo,
                                                Argilla,
                                                pH,
                                                CalTot,
                                                CalAtt,
                                                SostOrg,
                                                K2OAss,
                                                P2O5Ass,
                                                Mg,
                                                Ntot,
                                                Um_S,
                                                Cl_Dren,
                                                Falda,
                                                CsC,
                                                K2OAss_Data,
                                                MatOrg,
                                                MatOrg_Data,
                                                NOtot_Data,
                                                NOtot,
                                                P2O5Ass_Data,
                                                Suolo_CodAttri,
                                                Campo_Spia,
                                                Campo_Spia_Area,
                                                Cs_SIPI,
                                                Prossimo,
                                                Blk_Flag,
                                                Blk_Inizio_Data,
                                                Blk_Inizio_Username,
                                                Blk_Inizio_Note,
                                                Blk_Fine_Data,
                                                Blk_Fine_Username,
                                                Blk_Fine_Note,
                                                Grva_Cod_Veg,
                                           Grfi_Cod,
                                           Cod_Resp,
                                           Cod_Ente,
                                           Campo_Spia,
                                           Data_Raccolta,
                                           Produzione,
                                           ResaPrevista,
                                           ResaEffettiva,
                                           Scarto,
                                           Ind_Mat_Cod,
                                           Ind_Mat_Ril,
                                           Sta_Ter,
                                           Cop_DI,
                                           Cop_DF,
                                           Tra_Fila,
                                           Su_Fila,
                                           P_HA,
                                           Foral_Cod,
                                           Setup_Cod,
                                           Port_Cod,
                                           Imp_Cod,
                                           Stru_Prot,
                                           Pro_Pag,
                                           Seme_Q,
                                           Seme_T,
                                           Seme_P,
                                           Seme_D,
                                           Stato_Residui,
                                           Tecn_Cod,
                                           Denitrificazione,
                                           Volatilizzazione,
                                           ProfonditaLav,
                                           Id_Campo,
                                           Su_Cod,
                                           Cop_Cod,
                                           Cover,
                                           Monitorato,
                                           Codice_Ficale_Tecnico,
                                           Regolamento,
                                           Finanziamento,
                                           Data_Conversione,
                                           ProvenienzaSeme,
                                           Cod_Contratto,
                                           Cod_Conto,
                                           Ricavi_Previsti,
                                           Produzione_Prevista,
                                           Giudizio,
                                           Veg_Cod,
                                           Grfi_Cod_Progetto,
                                           CSProgetto_Cod,
                                           Stato_Impianto,
                                           Regolamento_Cod,
                                           Disciplinare_Cod,
                                           Disciplinare_PrivatoPubblico,
                                           Regolamento_Concimazioni_Cod,
                                           Data_Inizio_Prevista,
                                           Data_Fine_Prevista,
                                           via_stringa,
                                           Data_Fioritura_Prevista)


            XmlDatiAppezzamenti.AppendChild(XmlAppezzamento)



        Catch ex As Exception
            Log_Errori &= "XML_Appezzamenti. Errore durante la creazione dell'XML dell'Appezzamento: " & ex.Message
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
    Public Function XML_Appezzamento(ByRef Log_Errori As String,
                                        ByRef XmlDoc As XmlDocument,
                                        ByVal BaseCode As Integer,
                                        ByVal TopCode As Integer,
                                        ByVal Flag_CreaImpianto As Boolean,
                                        ByVal DT_Codici_Appezzamento As DataTable,
                                        ByVal DT_Codici_Impianto As DataTable,
                                        ByVal DT_Codici_Progetto As DataTable,
                                        ByVal TipoOperazioneDB_Appezzamento As enum_TipoOperazioneDB,
                                        ByVal TipoOperazioneDB_Impianto As enum_TipoOperazioneDB,
                                        ByVal TipoOperazioneDB_Progetto As enum_TipoOperazioneDB,
                                        ByVal Piva_SuperUser As String,
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Appezza As Integer,
                                        ByVal Sup_App As Decimal,
                                        ByVal App_Nome As String,
                                        ByVal Campo_Cod As Integer,
                                        ByVal Validita_Inizio_Appezza As Date,
                                        ByVal Validita_Fine_Appezza As Date,
                                        ByVal Id_Reg As Integer,
                                        ByVal Id_Consociazione As Integer,
                                        ByVal Sup_Imp As Decimal,
                                        ByVal Cul_Cod As Integer,
                                        ByVal Validita_Inizio_Impianto As Date,
                                        ByVal Validita_Fine_Impianto As Date,
                                        ByVal Data As Date,
                                        ByVal Progetto_Cod As Integer,
                                        ByVal Progetto_Nome As String,
                                        ByVal Progetto_Des As String,
                                        ByVal Cau_Progetto As Integer,
                                        ByVal Validita_Inizio_Progetto As Date,
                                        ByVal Validita_Fine_Progetto As Date,
                                        Optional ByVal Data_App As String = "0",
                                        Optional ByVal Data_Inizio As String = "0",
                                        Optional ByVal Data_Fine As String = "0",
                                        Optional ByVal Ep_Camp As String = "0",
                                        Optional ByVal X As Decimal = 0,
                                        Optional ByVal Y As Decimal = 0,
                                        Optional ByVal Zslm As Decimal = 0,
                                        Optional ByVal Esposiz As String = "...",
                                        Optional ByVal Pende As Decimal = 0,
                                        Optional ByVal Ubicazione As String = "...",
                                        Optional ByVal Num_Del As Integer = 0,
                                        Optional ByVal Clas As String = "",
                                        Optional ByVal Sabbia As Decimal = 0,
                                        Optional ByVal Limo As Decimal = 0,
                                        Optional ByVal Argilla As Decimal = 0,
                                        Optional ByVal pH As Decimal = 0,
                                        Optional ByVal CalTot As Decimal = 0,
                                        Optional ByVal CalAtt As Decimal = 0,
                                        Optional ByVal SostOrg As Decimal = 0,
                                        Optional ByVal K2OAss As Decimal = 0,
                                        Optional ByVal P2O5Ass As Decimal = 0,
                                        Optional ByVal Mg As Decimal = 0,
                                        Optional ByVal Ntot As Decimal = 0,
                                        Optional ByVal Um_S As Decimal = 0,
                                        Optional ByVal Cl_Dren As String = "0",
                                        Optional ByVal Falda As Integer = 0,
                                        Optional ByVal CsC As Decimal = 0,
                                        Optional ByVal K2OAss_Data As String = "0",
                                        Optional ByVal MatOrg As Decimal = 0,
                                        Optional ByVal MatOrg_Data As String = "0",
                                        Optional ByVal NOtot_Data As String = "0",
                                        Optional ByVal NOtot As Decimal = 0,
                                        Optional ByVal P2O5Ass_Data As String = "0",
                                        Optional ByVal Suolo_CodAttri As String = "",
                                        Optional ByVal Campo_Spia As Integer = 0,
                                        Optional ByVal Campo_Spia_Area As Decimal = 0,
                                        Optional ByVal Cs_SIPI As String = "",
                                        Optional ByVal Prossimo As Integer = 0,
                                        Optional ByVal Blk_Flag As Integer = 0,
                                        Optional ByVal Blk_Inizio_Data As Date = AGRODATAINIZIO,
                                        Optional ByVal Blk_Inizio_Username As String = "",
                                        Optional ByVal Blk_Inizio_Note As String = "",
                                        Optional ByVal Blk_Fine_Data As Date = AGRODATAFINE,
                                        Optional ByVal Blk_Fine_Username As String = "",
                                        Optional ByVal Blk_Fine_Note As String = "",
                                        Optional ByVal Grva_Cod_Veg As Integer = 0,
                                        Optional ByVal Grfi_Cod As Integer = 0,
                                        Optional ByVal Cod_Resp As Integer = 0,
                                        Optional ByVal Cod_Ente As Integer = 0,
                                        Optional ByVal Campo_Spia_Impianto As Integer = 0,
                                        Optional ByVal Data_Raccolta As String = "0",
                                        Optional ByVal Produzione As Integer = 0,
                                        Optional ByVal ResaPrevista As Decimal = 0,
                                        Optional ByVal ResaEffettiva As Decimal = 0,
                                        Optional ByVal Scarto As Integer = 0,
                                        Optional ByVal Ind_Mat_Cod As Integer = 0,
                                        Optional ByVal Ind_Mat_Ril As String = "0",
                                        Optional ByVal Sta_Ter As String = "",
                                        Optional ByVal Cop_DI As String = "0",
                                        Optional ByVal Cop_DF As String = "0",
                                        Optional ByVal Tra_Fila As Decimal = 0,
                                        Optional ByVal Su_Fila As Decimal = 0,
                                        Optional ByVal P_HA As Decimal = 0,
                                        Optional ByVal Foral_Cod As Integer = -1,
                                        Optional ByVal Setup_Cod As String = "-1",
                                        Optional ByVal Port_Cod As Integer = -1,
                                        Optional ByVal Imp_Cod As Integer = -1,
                                        Optional ByVal Stru_Prot As Integer = 0,
                                        Optional ByVal Pro_Pag As Integer = 0,
                                        Optional ByVal Seme_Q As Integer = 0,
                                        Optional ByVal Seme_T As Integer = 0,
                                        Optional ByVal Seme_P As Integer = 0,
                                        Optional ByVal Seme_D As Integer = 0,
                                        Optional ByVal Stato_Residui As String = "",
                                        Optional ByVal Tecn_Cod As Integer = -1,
                                        Optional ByVal Denitrificazione As Integer = 0,
                                        Optional ByVal Volatilizzazione As Integer = 0,
                                        Optional ByVal ProfonditaLav As Integer = 0,
                                        Optional ByVal Id_Campo As Integer = 0,
                                        Optional ByVal Su_Cod As Integer = -1,
                                        Optional ByVal Cop_Cod As Integer = 0,
                                        Optional ByVal Cover As Integer = 0,
                                        Optional ByVal Monitorato As Integer = 0,
                                        Optional ByVal Codice_Ficale_Tecnico As String = "",
                                        Optional ByVal Regolamento As Integer = enum_Cod_Regolamento.Regolamento_Nessuno,
                                        Optional ByVal Finanziamento As Integer = 0,
                                        Optional ByVal Data_Conversione As String = "0",
                                        Optional ByVal ProvenienzaSeme As Integer = 0,
                                        Optional ByVal Cod_Contratto As Integer = 0,
                                        Optional ByVal Cod_Conto As Integer = 0,
                                        Optional ByVal Ricavi_Previsti As Decimal = 0,
                                        Optional ByVal Produzione_Prevista As Decimal = 0,
                                        Optional ByVal Giudizio As String = "",
                                        Optional ByVal Veg_Cod As Integer = 0,
                                        Optional ByVal Grfi_Cod_Progetto As Integer = 0,
                                        Optional ByVal CSProgetto_Cod As Integer = 0,
                                        Optional ByVal Stato_Impianto As enum_Stato_Impianto = enum_Stato_Impianto.Impianto_Produzione,
                                        Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno,
                                        Optional ByVal Disciplinare_Cod As Integer = 0,
                                        Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0,
                                        Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0,
                                        Optional ByVal Data_Inizio_Prevista As Date = AGRODATAINIZIO,
                                        Optional ByVal Data_Fine_Prevista As Date = AGRODATAFINE,
                                        Optional ByVal via_stringa As String = "",
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

            XmlAppezzamento = XML_Appezzamento_Appezzamento(Log_Errori,
                                                            XmlDoc,
                                                            BaseCode,
                                                            TopCode,
                                                            TipoOperazioneDB_Appezzamento,
                                                            Piva_SuperUser,
                                                            Piva,
                                                            Sa_Cod,
                                                            Appezza,
                                                            Sup_App,
                                                            App_Nome,
                                                            Campo_Cod,
                                                            Validita_Inizio_Appezza,
                                                            Validita_Fine_Appezza,
                                                            Data_App,
                                                            Data_Inizio,
                                                            Data_Fine,
                                                            Ep_Camp,
                                                            X,
                                                            Y,
                                                            Zslm,
                                                            Esposiz,
                                                            Pende,
                                                            Ubicazione,
                                                            Num_Del,
                                                            Clas,
                                                            Sabbia,
                                                            Limo,
                                                            Argilla,
                                                            pH,
                                                            CalTot,
                                                            CalAtt,
                                                            SostOrg,
                                                            K2OAss,
                                                            P2O5Ass,
                                                            Mg,
                                                            Ntot,
                                                            Um_S,
                                                            Cl_Dren,
                                                            Falda,
                                                            CsC,
                                                            K2OAss_Data,
                                                            MatOrg,
                                                            MatOrg_Data,
                                                            NOtot_Data,
                                                            NOtot,
                                                            P2O5Ass_Data,
                                                            Suolo_CodAttri,
                                                            Campo_Spia,
                                                            Campo_Spia_Area,
                                                            Cs_SIPI,
                                                            Prossimo,
                                                            Blk_Flag,
                                                            Blk_Inizio_Data,
                                                            Blk_Inizio_Username,
                                                            Blk_Inizio_Note,
                                                            Blk_Fine_Data,
                                                            Blk_Fine_Username,
                                                            Blk_Fine_Note,
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

                    XmlAppezzamentoCodice = XML_Appezzamento_Codice(Log_Errori,
                                                                TipoOperazioneDB_Codice,
                                                                Piva,
                                                                Id_Cod,
                                                                BaseCode,
                                                                TopCode,
                                                                XmlDoc,
                                                                Sa_Cod,
                                                                Appezza,
                                                                Val_Cod,
                                                                Validita_Inizio_Codice,
                                                                Validita_Fine_Codice)

                    XmlAppezzamento.AppendChild(XmlAppezzamentoCodice)

                Next

            End If



            'se oltre all'appezzamento si vuole creare anche l'impianto
            If Flag_CreaImpianto Then

                '#######################################################
                '###############   DATI REG IMPIANTI    ################
                '#######################################################

                XmlDatiRegImpianto = XML_Impianti(Log_Errori,
                                               XmlDoc,
                                               True,
                                               BaseCode,
                                               TopCode,
                                               DT_Codici_Impianto,
                                                DT_Codici_Progetto,
                                               TipoOperazioneDB_Impianto,
                                               TipoOperazioneDB_Progetto,
                                               Piva_SuperUser,
                                               Piva,
                                               Sa_Cod,
                                               Appezza,
                                               Id_Reg,
                                               Id_Consociazione,
                                               Sup_Imp,
                                               Cul_Cod,
                                               Grfi_Cod,
                                               Validita_Inizio_Impianto,
                                               Validita_Fine_Impianto,
                                               Data,
                                               Progetto_Cod,
                                               Progetto_Nome,
                                               Progetto_Des,
                                               Cau_Progetto,
                                               Validita_Inizio_Progetto,
                                               Validita_Fine_Progetto,
                                               Grva_Cod_Veg,
                                               Cod_Resp,
                                               Cod_Ente,
                                               Campo_Spia,
                                               Data_Raccolta,
                                               Produzione,
                                               ResaPrevista,
                                               ResaEffettiva,
                                               Scarto,
                                               Ind_Mat_Cod,
                                               Ind_Mat_Ril,
                                               Sta_Ter,
                                               Cop_DI,
                                               Cop_DF,
                                               Tra_Fila,
                                               Su_Fila,
                                               P_HA,
                                               Foral_Cod,
                                               Setup_Cod,
                                               Port_Cod,
                                               Imp_Cod,
                                               Stru_Prot,
                                               Pro_Pag,
                                               Seme_Q,
                                               Seme_T,
                                               Seme_P,
                                               Seme_D,
                                               Stato_Residui,
                                               Tecn_Cod,
                                               Denitrificazione,
                                               Volatilizzazione,
                                               ProfonditaLav,
                                               Id_Campo,
                                               Su_Cod,
                                               Cop_Cod,
                                               Cover,
                                               Monitorato,
                                               Codice_Ficale_Tecnico,
                                               Regolamento,
                                               Finanziamento,
                                               Data_Conversione,
                                               ProvenienzaSeme,
                                               Cod_Contratto,
                                               Cod_Conto,
                                               Ricavi_Previsti,
                                               Produzione_Prevista,
                                               Giudizio,
                                               Veg_Cod,
                                               Grfi_Cod_Progetto,
                                               CSProgetto_Cod,
                                               Stato_Impianto,
                                               Regolamento_Cod,
                                               Disciplinare_Cod,
                                               Disciplinare_PubblicoPrivato,
                                               Regolamento_Concimazioni_Cod,
                                               Data_Inizio_Prevista,
                                               Data_Fine_Prevista,
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
            Log_Errori &= "XML_Appezzamento. Errore durante la creazione dell'XML dell'Appezzamento: " & ex.Message
        End Try


        Return XmlAppezzamento


    End Function


    '##########################################################################################
    Public Function XML_Appezzamento_Appezzamento(ByRef Log_Errori As String,
                                                    ByRef XmlDoc As XmlDocument,
                                                    ByVal BaseCode As Integer,
                                                    ByVal TopCode As Integer,
                                                    ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                    ByVal Piva_SuperUser As String,
                                                    ByVal Piva As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Appezza As Integer,
                                                    ByVal Sup_App As Decimal,
                                                    ByVal App_Nome As String,
                                                    ByVal Campo_Cod As Integer,
                                                    ByVal Validita_Inizio As Date,
                                                    ByVal Validita_Fine As Date,
                                                    Optional ByVal Data_App As String = "0",
                                                    Optional ByVal Data_Inizio As String = "0",
                                                    Optional ByVal Data_Fine As String = "0",
                                                    Optional ByVal Ep_Camp As String = "0",
                                                    Optional ByVal X As Decimal = 0,
                                                    Optional ByVal Y As Decimal = 0,
                                                    Optional ByVal Zslm As Decimal = 0,
                                                    Optional ByVal Esposiz As String = "...",
                                                    Optional ByVal Pende As Decimal = 0,
                                                    Optional ByVal Ubicazione As String = "...",
                                                    Optional ByVal Num_Del As Integer = 0,
                                                    Optional ByVal Clas As String = "",
                                                    Optional ByVal Sabbia As Decimal = 0,
                                                    Optional ByVal Limo As Decimal = 0,
                                                    Optional ByVal Argilla As Decimal = 0,
                                                    Optional ByVal pH As Decimal = 0,
                                                    Optional ByVal CalTot As Decimal = 0,
                                                    Optional ByVal CalAtt As Decimal = 0,
                                                    Optional ByVal SostOrg As Decimal = 0,
                                                    Optional ByVal K2OAss As Decimal = 0,
                                                    Optional ByVal P2O5Ass As Decimal = 0,
                                                    Optional ByVal Mg As Decimal = 0,
                                                    Optional ByVal Ntot As Decimal = 0,
                                                    Optional ByVal Um_S As Decimal = 0,
                                                    Optional ByVal Cl_Dren As String = "0",
                                                    Optional ByVal Falda As Integer = 0,
                                                    Optional ByVal CsC As Decimal = 0,
                                                    Optional ByVal K2OAss_Data As String = "0",
                                                    Optional ByVal MatOrg As Decimal = 0,
                                                    Optional ByVal MatOrg_Data As String = "0",
                                                    Optional ByVal NOtot_Data As String = "0",
                                                    Optional ByVal NOtot As Decimal = 0,
                                                    Optional ByVal P2O5Ass_Data As String = "0",
                                                    Optional ByVal Suolo_CodAttri As String = "",
                                                    Optional ByVal Campo_Spia As Integer = 0,
                                                    Optional ByVal Campo_Spia_Area As Decimal = 0,
                                                    Optional ByVal Cs_SIPI As String = "",
                                                    Optional ByVal Prossimo As Integer = 0,
                                                    Optional ByVal Blk_Flag As Integer = 0,
                                                    Optional ByVal Blk_Inizio_Data As Date = AGRODATAINIZIO,
                                                    Optional ByVal Blk_Inizio_Username As String = "",
                                                    Optional ByVal Blk_Inizio_Note As String = "",
                                                    Optional ByVal Blk_Fine_Data As Date = AGRODATAFINE,
                                                    Optional ByVal Blk_Fine_Username As String = "",
                                                    Optional ByVal Blk_Fine_Note As String = "",
                                                     Optional ByVal via_stringa As String = "",
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
            XmlAppezzamento.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

            'su Appezzamento_Scrivi non viene usato questo attributo,
            'ma viene usato direttamente il campo Piva_SuperUser
            XmlAppezzamento.SetAttribute("user", CStr(Piva_SuperUser))

            XmlAppezzamento.SetAttribute("piva", CStr(Piva))
            XmlAppezzamento.SetAttribute("sa_cod", CStr(Sa_Cod))
            XmlAppezzamento.SetAttribute("appezza", CStr(Appezza))
            XmlAppezzamento.SetAttribute("sup_app", CStr(Sup_App))
            XmlAppezzamento.SetAttribute("data_app", CStr(Data_App)) '= validita_inizio
            XmlAppezzamento.SetAttribute("app_nome", CStr(App_Nome))
            XmlAppezzamento.SetAttribute("campo_cod", CStr(Campo_Cod))

            XmlAppezzamento.SetAttribute("ep_camp", CStr(Ep_Camp))
            XmlAppezzamento.SetAttribute("x", CStr(X))
            XmlAppezzamento.SetAttribute("y", CStr(Y))
            XmlAppezzamento.SetAttribute("zslm", CStr(Zslm))
            XmlAppezzamento.SetAttribute("esposiz", CStr(Esposiz))
            XmlAppezzamento.SetAttribute("pende", CStr(Pende))
            XmlAppezzamento.SetAttribute("ubicazione", CStr(Ubicazione))
            XmlAppezzamento.SetAttribute("num_del", CStr(Num_Del))
            XmlAppezzamento.SetAttribute("clas", CStr(Clas))
            XmlAppezzamento.SetAttribute("sabbia", CStr(Sabbia))
            XmlAppezzamento.SetAttribute("limo", CStr(Limo))
            XmlAppezzamento.SetAttribute("argilla", CStr(Argilla))
            XmlAppezzamento.SetAttribute("ph", CStr(pH))
            XmlAppezzamento.SetAttribute("caltot", CStr(CalTot))
            XmlAppezzamento.SetAttribute("calatt", CStr(CalAtt))
            XmlAppezzamento.SetAttribute("sostorg", CStr(SostOrg))
            XmlAppezzamento.SetAttribute("k2oass", CStr(K2OAss))
            XmlAppezzamento.SetAttribute("p2o5ass", CStr(P2O5Ass))
            XmlAppezzamento.SetAttribute("mg", CStr(Mg))
            XmlAppezzamento.SetAttribute("ntot", CStr(Ntot))
            XmlAppezzamento.SetAttribute("um_s", CStr(Um_S))
            XmlAppezzamento.SetAttribute("cl_dren", CStr(Cl_Dren))
            XmlAppezzamento.SetAttribute("falda", CStr(Falda))
            XmlAppezzamento.SetAttribute("csc", CStr(CsC))
            XmlAppezzamento.SetAttribute("k2oass_data", CStr(K2OAss_Data))
            XmlAppezzamento.SetAttribute("matorg", CStr(MatOrg))
            XmlAppezzamento.SetAttribute("matorg_data", CStr(MatOrg_Data))
            XmlAppezzamento.SetAttribute("notot_data", CStr(NOtot_Data))
            XmlAppezzamento.SetAttribute("notot", CStr(NOtot))
            XmlAppezzamento.SetAttribute("p2o5ass_data", CStr(P2O5Ass_Data))
            XmlAppezzamento.SetAttribute("suolo_codattri", CStr(Suolo_CodAttri))
            XmlAppezzamento.SetAttribute("campo_spia", CStr(Campo_Spia))
            XmlAppezzamento.SetAttribute("campo_spia_area", CStr(Campo_Spia_Area))
            XmlAppezzamento.SetAttribute("cs_sipi", CStr(Cs_SIPI))
            XmlAppezzamento.SetAttribute("prossimo", CStr(Prossimo))
            XmlAppezzamento.SetAttribute("data_inizio", CStr(Data_Inizio)) ' = Validita_Inizio
            XmlAppezzamento.SetAttribute("data_fine", CStr(Data_Fine)) ' = Validita_Fine
            XmlAppezzamento.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
            XmlAppezzamento.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))

            XmlAppezzamento.SetAttribute("via_stringa", via_stringa)


            'non sono gestiti attualmente da Appezzamento_Scrivi
            XmlAppezzamento.SetAttribute(LCase("Blk_Flag"), CStr(Blk_Flag))
            XmlAppezzamento.SetAttribute(LCase("Blk_Inizio_Data"), CStr(Blk_Inizio_Data))
            XmlAppezzamento.SetAttribute(LCase("Blk_Inizio_Username"), CStr(Blk_Inizio_Username))
            XmlAppezzamento.SetAttribute(LCase("Blk_Inizio_Note"), CStr(Blk_Inizio_Note))
            XmlAppezzamento.SetAttribute(LCase("Blk_Fine_Data"), CStr(Blk_Fine_Data))
            XmlAppezzamento.SetAttribute(LCase("Blk_Fine_Username"), CStr(Blk_Fine_Username))
            XmlAppezzamento.SetAttribute(LCase("Blk_Fine_Note"), CStr(Blk_Fine_Note))

            XmlAppezzamento.SetAttribute("basecode", CStr(BaseCode))
            XmlAppezzamento.SetAttribute("topcode", CStr(TopCode))


            XmlAppezzamento.SetAttribute("AltriVitigniPresenti", AgronicaCoreDataProvider.UtilityProvider.ValoreToString(AltriVitigniPresenti))

            'Distruggo gli oggetti
            Return XmlAppezzamento

            XmlAppezzamento = Nothing



        Catch ex As Exception
            Log_Errori &= "XML_Appezzamento_Appezzamento. Errore durante la creazione dell'XML dell'Appezzamento: " & ex.Message
        End Try


    End Function


    '##########################################################################################
    Public Function XML_Appezzamento_Codice(ByRef Log_Errori As String,
                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                            ByVal Piva As String,
                                            ByVal Id_Cod As Integer,
                                            ByVal BaseCode As Integer,
                                            ByVal TopCode As Integer,
                                            Optional ByRef XmlDoc As XmlDocument = Nothing,
                                            Optional ByVal Sa_Cod As Integer = 0,
                                            Optional ByVal Appezza As Integer = 0,
                                            Optional ByVal Val_Cod As String = "",
                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
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
            NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            NodoXml.SetAttribute(LCase("piva"), CStr(Piva))
            NodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
            NodoXml.SetAttribute(LCase("appezza"), CStr(Appezza))
            NodoXml.SetAttribute(LCase("id_cod"), CStr(Id_Cod))
            NodoXml.SetAttribute(LCase("val_cod"), CStr(Val_Cod))
            NodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
            NodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
            NodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
            NodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))

            'Restituisco in uscita 
            Return NodoXml

            'Distruggo gli oggetti
            NodoXml = Nothing

        Catch ex As Exception
            Log_Errori &= "XML_Appezzamento_Codice. Errore durante la creazione dell'XML del codice dell'Appezzamento: " & ex.Message
        End Try


    End Function


    '##########################################################################################
    Public Function XML_Campo_Codice(ByRef Log_Errori As String,
                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal id_cod As Integer,
                                            ByVal val_cod As String,
                                            ByVal BaseCode As Integer,
                                            ByVal TopCode As Integer,
                                            Optional ByRef XmlDoc As XmlDocument = Nothing,
                                            Optional ByVal Campo_Cod As Integer = 0,
                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
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
            NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            NodoXml.SetAttribute(LCase("piva"), CStr(Piva))
            NodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
            NodoXml.SetAttribute(LCase("id_cod"), CStr(id_cod))
            NodoXml.SetAttribute(LCase("val_cod"), CStr(val_cod))
            NodoXml.SetAttribute(LCase("Campo_Cod"), CStr(Campo_Cod))
            NodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
            NodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
            NodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
            NodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))

            'Restituisco in uscita 
            Return NodoXml

            'Distruggo gli oggetti
            NodoXml = Nothing

        Catch ex As Exception
            Log_Errori &= "XML_Campo_Codice. Errore durante la creazione dell'XML del codice del Campo: " & ex.Message
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
    Public Function XML_Impianti(ByRef Log_Errori As String,
                                    ByRef XmlDoc As XmlDocument,
                                    ByVal Flag_AggancioAppezzamento As Boolean,
                                    ByVal BaseCode As Integer,
                                    ByVal TopCode As Integer,
                                    ByVal DT_Codici_Impianto As DataTable,
                                    ByVal DT_Codici_Progetto As DataTable,
                                    ByVal TipoOperazioneDB_Impianto As enum_TipoOperazioneDB,
                                    ByVal TipoOperazioneDB_Progetto As enum_TipoOperazioneDB,
                                    ByVal Piva_SuperUser As String,
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Appezza As Integer,
                                    ByVal Id_Reg As Integer,
                                    ByVal Id_Consociazione As Integer,
                                    ByVal Sup_Imp As Decimal,
                                    ByVal Cul_Cod As Integer,
                                    ByVal Grfi_Cod As Integer,
                                    ByVal Validita_Inizio_Impianto As Date,
                                    ByVal Validita_Fine_Impianto As Date,
                                    ByVal Data As Date,
                                        ByVal Progetto_Cod As Integer,
                                        ByVal Progetto_Nome As String,
                                        ByVal Progetto_Des As String,
                                        ByVal Cau_Progetto As Integer,
                                        ByVal Validita_Inizio_Progetto As Date,
                                        ByVal Validita_Fine_Progetto As Date,
                                            Optional ByVal Grva_Cod_Veg As Integer = 0,
                                            Optional ByVal Cod_Resp As Integer = 0,
                                            Optional ByVal Cod_Ente As Integer = 0,
                                            Optional ByVal Campo_Spia As Integer = 0,
                                            Optional ByVal Data_Raccolta As String = "0",
                                            Optional ByVal Produzione As Integer = 0,
                                            Optional ByVal ResaPrevista As Decimal = 0,
                                            Optional ByVal ResaEffettiva As Decimal = 0,
                                            Optional ByVal Scarto As Integer = 0,
                                            Optional ByVal Ind_Mat_Cod As Integer = 0,
                                            Optional ByVal Ind_Mat_Ril As String = "0",
                                            Optional ByVal Sta_Ter As String = "",
                                            Optional ByVal Cop_DI As String = "0",
                                            Optional ByVal Cop_DF As String = "0",
                                            Optional ByVal Tra_Fila As Decimal = 0,
                                            Optional ByVal Su_Fila As Decimal = 0,
                                            Optional ByVal P_HA As Decimal = 0,
                                            Optional ByVal Foral_Cod As Integer = -1,
                                            Optional ByVal Setup_Cod As String = "-1",
                                            Optional ByVal Port_Cod As Integer = -1,
                                            Optional ByVal Imp_Cod As Integer = -1,
                                            Optional ByVal Stru_Prot As Integer = 0,
                                            Optional ByVal Pro_Pag As Integer = 0,
                                            Optional ByVal Seme_Q As Integer = 0,
                                            Optional ByVal Seme_T As Integer = 0,
                                            Optional ByVal Seme_P As Integer = 0,
                                            Optional ByVal Seme_D As Integer = 0,
                                            Optional ByVal Stato_Residui As String = "",
                                            Optional ByVal Tecn_Cod As Integer = -1,
                                            Optional ByVal Denitrificazione As Integer = 0,
                                            Optional ByVal Volatilizzazione As Integer = 0,
                                            Optional ByVal ProfonditaLav As Integer = 0,
                                            Optional ByVal Id_Campo As Integer = 0,
                                            Optional ByVal Su_Cod As Integer = -1,
                                            Optional ByVal Cop_Cod As Integer = 0,
                                            Optional ByVal Cover As Integer = 0,
                                            Optional ByVal Monitorato As Integer = 0,
                                            Optional ByVal Codice_Ficale_Tecnico As String = "",
                                            Optional ByVal Regolamento As Integer = enum_Cod_Regolamento.Regolamento_Nessuno,
                                            Optional ByVal Finanziamento As Integer = 0,
                                            Optional ByVal Data_Conversione As String = "0",
                                            Optional ByVal ProvenienzaSeme As Integer = 0,
                                    Optional ByVal Cod_Contratto As Integer = 0,
                                    Optional ByVal Cod_Conto As Integer = 0,
                                    Optional ByVal Ricavi_Previsti As Decimal = 0,
                                    Optional ByVal Produzione_Prevista As Decimal = 0,
                                    Optional ByVal Giudizio As String = "",
                                    Optional ByVal Veg_Cod As Integer = 0,
                                    Optional ByVal Grfi_Cod_Progetto As Integer = 0,
                                    Optional ByVal CSProgetto_Cod As Integer = 0,
                                    Optional ByVal Stato_Impianto As enum_Stato_Impianto = enum_Stato_Impianto.Impianto_Produzione,
                                    Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno,
                                    Optional ByVal Disciplinare_Cod As Integer = 0,
                                    Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0,
                                    Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0,
                                    Optional ByVal Data_Inizio_Prevista As Date = AGRODATAINIZIO,
                                    Optional ByVal Data_Fine_Prevista As Date = AGRODATAFINE,
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

            If Not Flag_AggancioAppezzamento Then
                XmlDoc.AppendChild(XmlDatiRegImpianti)
            End If


            '#######################################################
            '################# REG  IMPIANTO    ####################
            '#######################################################

            XmlRegImpianto = XML_Impianto(Log_Errori,
                                            XmlDoc,
                                            BaseCode,
                                            TopCode,
                                            DT_Codici_Impianto,
                                            DT_Codici_Progetto,
                                            TipoOperazioneDB_Impianto,
                                            TipoOperazioneDB_Progetto,
                                            Piva_SuperUser,
                                            Piva,
                                            Sa_Cod,
                                            Appezza,
                                            Id_Reg,
                                            Id_Consociazione,
                                            Sup_Imp,
                                            Cul_Cod,
                                            Grfi_Cod,
                                            Validita_Inizio_Impianto,
                                            Validita_Fine_Impianto,
                                            Data,
                                            Progetto_Cod,
                                            Progetto_Nome,
                                            Progetto_Des,
                                            Cau_Progetto,
                                            Validita_Inizio_Progetto,
                                            Validita_Fine_Progetto,
                                            Grva_Cod_Veg,
                                            Cod_Resp,
                                            Cod_Ente,
                                            Campo_Spia,
                                            Data_Raccolta,
                                            Produzione,
                                            ResaPrevista,
                                            ResaEffettiva,
                                            Scarto,
                                            Ind_Mat_Cod,
                                            Ind_Mat_Ril,
                                            Sta_Ter,
                                            Cop_DI,
                                            Cop_DF,
                                            Tra_Fila,
                                            Su_Fila,
                                            P_HA,
                                            Foral_Cod,
                                            Setup_Cod,
                                            Port_Cod,
                                            Imp_Cod,
                                            Stru_Prot,
                                            Pro_Pag,
                                            Seme_Q,
                                            Seme_T,
                                            Seme_P,
                                            Seme_D,
                                            Stato_Residui,
                                            Tecn_Cod,
                                            Denitrificazione,
                                            Volatilizzazione,
                                            ProfonditaLav,
                                            Id_Campo,
                                            Su_Cod,
                                            Cop_Cod,
                                            Cover,
                                            Monitorato,
                                            Codice_Ficale_Tecnico,
                                            Regolamento,
                                            Finanziamento,
                                            Data_Conversione,
                                            ProvenienzaSeme,
                                            Cod_Contratto,
                                            Cod_Conto,
                                            Ricavi_Previsti,
                                            Produzione_Prevista,
                                            Giudizio,
                                            Veg_Cod,
                                            Grfi_Cod_Progetto,
                                            CSProgetto_Cod,
                                            Stato_Impianto,
                                            Regolamento_Cod,
                                            Disciplinare_Cod,
                                            Disciplinare_PubblicoPrivato,
                                            Regolamento_Concimazioni_Cod,
                                            Data_Inizio_Prevista,
                                            Data_Fine_Prevista,
                                            Data_Fioritura_Prevista)


            XmlDatiRegImpianti.AppendChild(XmlRegImpianto)



        Catch ex As Exception
            Log_Errori &= "XML_Impianti. Errore durante la creazione dell'XML dell'Impianto: " & ex.Message
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
    Public Function XML_Impianto(ByRef Log_Errori As String,
                                    ByRef XmlDoc As XmlDocument,
                                    ByVal BaseCode As Integer,
                                    ByVal TopCode As Integer,
                                    ByVal DT_Codici_Impianto As DataTable,
                                    ByVal DT_Codici_Progetto As DataTable,
                                    ByVal TipoOperazioneDB_Impianto As enum_TipoOperazioneDB,
                                    ByVal TipoOperazioneDB_Progetto As enum_TipoOperazioneDB,
                                    ByVal Piva_SuperUser As String,
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Appezza As Integer,
                                    ByVal Id_Reg As Integer,
                                    ByVal Id_Consociazione As Integer,
                                    ByVal Sup_Imp As Decimal,
                                    ByVal Cul_Cod As Integer,
                                    ByVal Grfi_Cod As Integer,
                                    ByVal Validita_Inizio_Impianto As Date,
                                    ByVal Validita_Fine_Impianto As Date,
                                    ByVal Data As Date,
                                    ByVal Progetto_Cod As Integer,
                                    ByVal Progetto_Nome As String,
                                    ByVal Progetto_Des As String,
                                    ByVal Cau_Progetto As Integer,
                                    ByVal Validita_Inizio_Progetto As Date,
                                    ByVal Validita_Fine_Progetto As Date,
                                            Optional ByVal Grva_Cod_Veg As Integer = 0,
                                            Optional ByVal Cod_Resp As Integer = 0,
                                            Optional ByVal Cod_Ente As Integer = 0,
                                            Optional ByVal Campo_Spia As Integer = 0,
                                            Optional ByVal Data_Raccolta As String = "0",
                                            Optional ByVal Produzione As Integer = 0,
                                            Optional ByVal ResaPrevista As Decimal = 0,
                                            Optional ByVal ResaEffettiva As Decimal = 0,
                                            Optional ByVal Scarto As Integer = 0,
                                            Optional ByVal Ind_Mat_Cod As Integer = 0,
                                            Optional ByVal Ind_Mat_Ril As String = "0",
                                            Optional ByVal Sta_Ter As String = "",
                                            Optional ByVal Cop_DI As String = "0",
                                            Optional ByVal Cop_DF As String = "0",
                                            Optional ByVal Tra_Fila As Decimal = 0,
                                            Optional ByVal Su_Fila As Decimal = 0,
                                            Optional ByVal P_HA As Decimal = 0,
                                            Optional ByVal Foral_Cod As Integer = -1,
                                            Optional ByVal Setup_Cod As String = "-1",
                                            Optional ByVal Port_Cod As Integer = -1,
                                            Optional ByVal Imp_Cod As Integer = -1,
                                            Optional ByVal Stru_Prot As Integer = 0,
                                            Optional ByVal Pro_Pag As Integer = 0,
                                            Optional ByVal Seme_Q As Integer = 0,
                                            Optional ByVal Seme_T As Integer = 0,
                                            Optional ByVal Seme_P As Integer = 0,
                                            Optional ByVal Seme_D As Integer = 0,
                                            Optional ByVal Stato_Residui As String = "",
                                            Optional ByVal Tecn_Cod As Integer = -1,
                                            Optional ByVal Denitrificazione As Integer = 0,
                                            Optional ByVal Volatilizzazione As Integer = 0,
                                            Optional ByVal ProfonditaLav As Integer = 0,
                                            Optional ByVal Id_Campo As Integer = 0,
                                            Optional ByVal Su_Cod As Integer = -1,
                                            Optional ByVal Cop_Cod As Integer = 0,
                                            Optional ByVal Cover As Integer = 0,
                                            Optional ByVal Monitorato As Integer = 0,
                                            Optional ByVal Codice_Ficale_Tecnico As String = "",
                                            Optional ByVal Regolamento As Integer = enum_Cod_Regolamento.Regolamento_Nessuno,
                                            Optional ByVal Finanziamento As Integer = 0,
                                            Optional ByVal Data_Conversione As String = "0",
                                            Optional ByVal ProvenienzaSeme As Integer = 0,
                                    Optional ByVal Cod_Contratto As Integer = 0,
                                    Optional ByVal Cod_Conto As Integer = 0,
                                    Optional ByVal Ricavi_Previsti As Decimal = 0,
                                    Optional ByVal Produzione_Prevista As Decimal = 0,
                                    Optional ByVal Giudizio As String = "",
                                    Optional ByVal Veg_Cod As Integer = 0,
                                    Optional ByVal Grfi_Cod_Progetto As Integer = 0,
                                    Optional ByVal CSProgetto_Cod As Integer = 0,
                                    Optional ByVal Stato_Impianto As enum_Stato_Impianto = enum_Stato_Impianto.Impianto_Produzione,
                                    Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno,
                                    Optional ByVal Disciplinare_Cod As Integer = 0,
                                    Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0,
                                    Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0,
                                    Optional ByVal Data_Inizio_Prevista As Date = AGRODATAINIZIO,
                                    Optional ByVal Data_Fine_Prevista As Date = AGRODATAFINE,
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

            XmlRegImpianto = XML_Impianto_Impianto(Log_Errori,
                                                    XmlDoc,
                                                    BaseCode,
                                                    TopCode,
                                                    TipoOperazioneDB_Impianto,
                                                    Piva_SuperUser,
                                                    Piva,
                                                    Sa_Cod,
                                                    Appezza,
                                                    Id_Reg,
                                                    Id_Consociazione,
                                                    Sup_Imp,
                                                    Cul_Cod,
                                                    Grfi_Cod,
                                                    Validita_Inizio_Impianto,
                                                    Validita_Fine_Impianto,
                                                    Data,
                                                    Grva_Cod_Veg,
                                                    Cod_Resp,
                                                    Cod_Ente,
                                                    Campo_Spia,
                                                    Data_Raccolta,
                                                    Produzione,
                                                    ResaPrevista,
                                                    ResaEffettiva,
                                                    Scarto,
                                                    Ind_Mat_Cod,
                                                    Ind_Mat_Ril,
                                                    Sta_Ter,
                                                    Cop_DI,
                                                    Cop_DF,
                                                    Tra_Fila,
                                                    Su_Fila,
                                                    P_HA,
                                                    Foral_Cod,
                                                    Setup_Cod,
                                                    Port_Cod,
                                                    Imp_Cod,
                                                    Stru_Prot,
                                                    Pro_Pag,
                                                    Seme_Q,
                                                    Seme_T,
                                                    Seme_P,
                                                    Seme_D,
                                                    Stato_Residui,
                                                    Tecn_Cod,
                                                    Denitrificazione,
                                                    Volatilizzazione,
                                                    ProfonditaLav,
                                                    Id_Campo,
                                                    Su_Cod,
                                                    Cop_Cod,
                                                    Cover,
                                                    Monitorato,
                                                    Codice_Ficale_Tecnico,
                                                    Regolamento,
                                                    Finanziamento,
                                                    Data_Conversione,
                                                    ProvenienzaSeme,
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

                    XmlImpiantoCodice = XML_Impianto_Codice(Log_Errori,
                                                            TipoOperazioneDB_Codice,
                                                            Piva,
                                                            Id_Cod,
                                                            BaseCode,
                                                            TopCode,
                                                            XmlDoc,
                                                            Sa_Cod,
                                                            Appezza,
                                                            Id_Reg,
                                                            Val_Cod,
                                                            Validita_Inizio_Codice,
                                                            Validita_Fine_Codice)

                    XmlRegImpianto.SelectSingleNode("child::DatiCodici").AppendChild(XmlImpiantoCodice)

                Next

            End If



            '#######################################################
            '################   DATI PROGETTI    ###################
            '#######################################################

            XmlDatiProgetto = XML_ProgettiImpianto(Log_Errori,
                                                XmlDoc,
                                                True,
                                                BaseCode,
                                                TopCode,
                                                DT_Codici_Progetto,
                                                TipoOperazioneDB_Progetto,
                                                Piva,
                                                Sa_Cod,
                                                Appezza,
                                                Id_Reg,
                                                Progetto_Cod,
                                                Progetto_Nome,
                                                Progetto_Des,
                                                Cau_Progetto,
                                                Validita_Inizio_Progetto,
                                                Validita_Fine_Progetto,
                                                Cod_Contratto,
                                                Cod_Conto,
                                                Ricavi_Previsti,
                                                Produzione_Prevista,
                                                Giudizio,
                                                Veg_Cod,
                                                Grfi_Cod_Progetto,
                                                CSProgetto_Cod,
                                                Stato_Impianto,
                                                Regolamento_Cod,
                                                Disciplinare_Cod,
                                                Disciplinare_PubblicoPrivato,
                                                Regolamento_Concimazioni_Cod,
                                                Data_Inizio_Prevista,
                                                Data_Fine_Prevista,
                                                P_HA,
                                                Data_Fioritura_Prevista)


            XmlRegImpianto.AppendChild(XmlDatiProgetto)



        Catch ex As Exception
            Log_Errori &= "XML_Impianto. Errore durante la creazione dell'XML dell'Impianto: " & ex.Message
        End Try


        Return XmlRegImpianto


    End Function


    '##########################################################################################
    Public Function XML_Impianto_Impianto(ByRef Log_Errori As String,
                                            ByRef XmlDoc As XmlDocument,
                                            ByVal BaseCode As Integer,
                                            ByVal TopCode As Integer,
                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                            ByVal Piva_SuperUser As String,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Appezza As Integer,
                                            ByVal Id_Reg As Integer,
                                            ByVal Id_Consociazione As Integer,
                                            ByVal Sup_Imp As Decimal,
                                            ByVal Cul_Cod As Integer,
                                            ByVal Grfi_Cod As Integer,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            ByVal Data As Date,
                                            Optional ByVal Grva_Cod_Veg As Integer = 0,
                                            Optional ByVal Cod_Resp As Integer = 0,
                                            Optional ByVal Cod_Ente As Integer = 0,
                                            Optional ByVal Campo_Spia As Integer = 0,
                                            Optional ByVal Data_Raccolta As String = "0",
                                            Optional ByVal Produzione As Integer = 0,
                                            Optional ByVal ResaPrevista As Decimal = 0,
                                            Optional ByVal ResaEffettiva As Decimal = 0,
                                            Optional ByVal Scarto As Integer = 0,
                                            Optional ByVal Ind_Mat_Cod As Integer = 0,
                                            Optional ByVal Ind_Mat_Ril As String = "0",
                                            Optional ByVal Sta_Ter As String = "",
                                            Optional ByVal Cop_DI As String = "0",
                                            Optional ByVal Cop_DF As String = "0",
                                            Optional ByVal Tra_Fila As Decimal = 0,
                                            Optional ByVal Su_Fila As Decimal = 0,
                                            Optional ByVal P_HA As Decimal = 0,
                                            Optional ByVal Foral_Cod As Integer = -1,
                                            Optional ByVal Setup_Cod As String = "-1",
                                            Optional ByVal Port_Cod As Integer = -1,
                                            Optional ByVal Imp_Cod As Integer = -1,
                                            Optional ByVal Stru_Prot As Integer = 0,
                                            Optional ByVal Pro_Pag As Integer = 0,
                                            Optional ByVal Seme_Q As Integer = 0,
                                            Optional ByVal Seme_T As Integer = 0,
                                            Optional ByVal Seme_P As Integer = 0,
                                            Optional ByVal Seme_D As Integer = 0,
                                            Optional ByVal Stato_Residui As String = "",
                                            Optional ByVal Tecn_Cod As Integer = -1,
                                            Optional ByVal Denitrificazione As Integer = 0,
                                            Optional ByVal Volatilizzazione As Integer = 0,
                                            Optional ByVal ProfonditaLav As Integer = 0,
                                            Optional ByVal Id_Campo As Integer = 0,
                                            Optional ByVal Su_Cod As Integer = -1,
                                            Optional ByVal Cop_Cod As Integer = 0,
                                            Optional ByVal Cover As Integer = 0,
                                            Optional ByVal Monitorato As Integer = 0,
                                            Optional ByVal Codice_Ficale_Tecnico As String = "",
                                            Optional ByVal Regolamento As Integer = enum_Cod_Regolamento.Regolamento_Nessuno,
                                            Optional ByVal Finanziamento As Integer = 0,
                                            Optional ByVal Data_Conversione As String = "0",
                                            Optional ByVal ProvenienzaSeme As Integer = 0,
                                            Optional ByVal Data_Fioritura_Prevista As Date = AGRODATAINIZIO,
                                            Optional ByVal Unita_Vitata As Integer = 0,
                                            Optional ByVal Sovrainnesto_Cod As Integer = 0,
                                            Optional ByVal ancoraggiTestata As Nullable(Of Int32) = 0,
                                            Optional ByVal annoRiferimento As Nullable(Of Int32) = 0,
                                            Optional ByVal codFiliStostegno As String = "",
                                            Optional ByVal codPaliTessitura As String = "",
                                            Optional ByVal codPaliTestata As String = "",
                                            Optional ByVal codStatoColt As String = "",
                                            Optional ByVal codTipoVari As String = "",
                                            Optional ByVal DataProtocollo As Date = Nothing,
                                            Optional ByVal DataRilievo As Date = Nothing,
                                            Optional ByVal destProduttiva As String = "",
                                            Optional ByVal destProduttivaDescr As String = "",
                                            Optional ByVal distanzaPali As Double = Nothing,
                                            Optional ByVal dtFine As Date = Nothing,
                                            Optional ByVal dtFineGestione As Date = Nothing,
                                            Optional ByVal dtInizio As Date = Nothing,
                                            Optional ByVal dtInizioGestione As Date = Nothing,
                                            Optional ByVal dtIns As Date = Nothing,
                                            Optional ByVal dtVar As Date = Nothing,
                                            Optional ByVal fallanzePerc As Double = Nothing,
                                            Optional ByVal flagAnomalia As String = "",
                                            Optional ByVal flagAttuale As String = "",
                                            Optional ByVal flagCessata As String = "",
                                            Optional ByVal flagContributo As String = "",
                                            Optional ByVal flagRegolarizz2009 As String = "",
                                            Optional ByVal flagRicalcoloGis As String = "",
                                            Optional ByVal GiacituraTerreno As String = "",
                                            Optional ByVal idUnitaVitata As Nullable(Of Int32) = 0,
                                            Optional ByVal idUtenteIns As String = "",
                                            Optional ByVal idUtenteVar As String = "",
                                            Optional ByVal numeroProtocollo As String = "",
                                            Optional ByVal progPoligono As String = "",
                                            Optional ByVal supVitataDich As Nullable(Of Int32) = 0,
                                            Optional ByVal supVitataDichPRCalcolo As Nullable(Of Int32) = 0,
                                            Optional ByVal SuperficieServizioMq As Nullable(Of Int32) = 0,
                                            Optional ByVal Terrazzamenti As Nullable(Of Int32) = 0,
                                            Optional ByVal TipoColtura As String = "",
                                            Optional ByVal tipoProcedimento As String = "",
                                            Optional ByVal tipoUnar As String = "",
                                            Optional ByVal tipoVariazione As String = "",
                                            Optional ByVal unar As String = "",
                                            Optional ByVal Data_Inizio_Portinnesto As Date = AGRODATAINIZIO,
                                            Optional ByVal Data_Inizio_Impianto As Date = AGRODATAINIZIO
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
            XmlImpianto.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))

            'su Reg_Impianto_Scrivi non viene usato questo attributo,
            'ma viene usato direttamente il campo Piva_SuperUser
            XmlImpianto.SetAttribute("user", CStr(Piva_SuperUser))

            XmlImpianto.SetAttribute("piva", CStr(Piva))
            XmlImpianto.SetAttribute("sa_cod", CStr(Sa_Cod))
            XmlImpianto.SetAttribute("appezza", CStr(Appezza))
            XmlImpianto.SetAttribute("id_reg", CStr(Id_Reg))
            XmlImpianto.SetAttribute("id_consociazione", CStr(Id_Consociazione))
            XmlImpianto.SetAttribute("sup_imp", CStr(Sup_Imp))
            XmlImpianto.SetAttribute("cul_cod", CStr(Cul_Cod))

            XmlImpianto.SetAttribute("grva_cod_veg", CStr(Grva_Cod_Veg))
            XmlImpianto.SetAttribute("grfi_cod", CStr(Grfi_Cod))
            XmlImpianto.SetAttribute("cod_resp", CStr(Cod_Resp))
            XmlImpianto.SetAttribute("cod_ente", CStr(Cod_Ente))
            XmlImpianto.SetAttribute("campo_spia", CStr(Campo_Spia))
            XmlImpianto.SetAttribute("data", CStr(Data)) 'impostare = validita_inizio
            XmlImpianto.SetAttribute("data_raccolta", CStr(Data_Raccolta))

            'questo campo non viene gestito da Reg_Impianto_Scrivi
            'ma è nel db!
            XmlImpianto.SetAttribute("produzione", CStr(Produzione))

            XmlImpianto.SetAttribute("resa_prevista", CStr(ResaPrevista))
            XmlImpianto.SetAttribute("resa_effettiva", CStr(ResaEffettiva))
            XmlImpianto.SetAttribute("scarto", CStr(Scarto))
            XmlImpianto.SetAttribute("ind_mat_cod", CStr(Ind_Mat_Cod))
            XmlImpianto.SetAttribute("ind_mat_ril", CStr(Ind_Mat_Ril))
            XmlImpianto.SetAttribute("sta_ter", CStr(Sta_Ter))
            XmlImpianto.SetAttribute("cop_di", CStr(Cop_DI))
            XmlImpianto.SetAttribute("cop_df", CStr(Cop_DF))
            XmlImpianto.SetAttribute("tra_fila", CStr(Tra_Fila))
            XmlImpianto.SetAttribute("su_fila", CStr(Su_Fila))
            XmlImpianto.SetAttribute("p_ha", CStr(P_HA))
            XmlImpianto.SetAttribute("foral_cod", CStr(Foral_Cod))
            XmlImpianto.SetAttribute("setup_cod", CStr(Setup_Cod))
            XmlImpianto.SetAttribute("port_cod", CStr(Port_Cod))
            XmlImpianto.SetAttribute("imp_cod", CStr(Imp_Cod))
            XmlImpianto.SetAttribute("stru_prot", CStr(Stru_Prot))
            XmlImpianto.SetAttribute("pro_pag", CStr(Pro_Pag))
            XmlImpianto.SetAttribute("seme_q", CStr(Seme_Q))
            XmlImpianto.SetAttribute("seme_t", CStr(Seme_T))
            XmlImpianto.SetAttribute("seme_p", CStr(Seme_P))
            XmlImpianto.SetAttribute("seme_d", CStr(Seme_D))
            XmlImpianto.SetAttribute("stato_residui", CStr(Stato_Residui))
            XmlImpianto.SetAttribute("tecn_cod", CStr(Tecn_Cod))
            XmlImpianto.SetAttribute("denitrificazione", CStr(Denitrificazione))
            XmlImpianto.SetAttribute("volatilizzazione", CStr(Volatilizzazione))
            XmlImpianto.SetAttribute("profonditalav", CStr(ProfonditaLav))
            XmlImpianto.SetAttribute("id_campo", CStr(Id_Campo))
            XmlImpianto.SetAttribute("su_cod", CStr(Su_Cod))
            XmlImpianto.SetAttribute("cop_cod", CStr(Cop_Cod))
            XmlImpianto.SetAttribute("cover", CStr(Cover))
            XmlImpianto.SetAttribute("monitorato", CStr(Monitorato))
            XmlImpianto.SetAttribute("codice_fiscale_tecnico", CStr(Codice_Ficale_Tecnico))
            XmlImpianto.SetAttribute("regolamento", CStr(Regolamento))
            XmlImpianto.SetAttribute("finanziamento", CStr(Finanziamento))
            XmlImpianto.SetAttribute("data_conversione", CStr(Data_Conversione))
            XmlImpianto.SetAttribute("provenienzaseme", CStr(ProvenienzaSeme))
            XmlImpianto.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
            XmlImpianto.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))

            XmlImpianto.SetAttribute("data_fioritura_prevista", CStr(Data_Fioritura_Prevista))

            XmlImpianto.SetAttribute("basecode", CStr(BaseCode))
            XmlImpianto.SetAttribute("topcode", CStr(TopCode))



            XmlImpianto.SetAttribute("Unita_Vitata", CStr(Unita_Vitata))
            XmlImpianto.SetAttribute("Sovrainnesto_Cod", CStr(Sovrainnesto_Cod))
            XmlImpianto.SetAttribute("ancoraggiTestata", CStr(ancoraggiTestata))
            XmlImpianto.SetAttribute("annoRiferimento", CStr(annoRiferimento))
            XmlImpianto.SetAttribute("codFiliStostegno", CStr(codFiliStostegno))
            XmlImpianto.SetAttribute("codPaliTessitura", CStr(codPaliTessitura))
            XmlImpianto.SetAttribute("codPaliTestata", CStr(codPaliTestata))
            XmlImpianto.SetAttribute("codStatoColt", CStr(codStatoColt))
            XmlImpianto.SetAttribute("codTipoVari", CStr(codTipoVari))
            XmlImpianto.SetAttribute("DataProtocollo", CStr(DataProtocollo))
            XmlImpianto.SetAttribute("DataRilievo", CStr(DataRilievo))
            XmlImpianto.SetAttribute("destProduttiva", CStr(destProduttiva))
            XmlImpianto.SetAttribute("destProduttivaDescr", CStr(destProduttivaDescr))
            XmlImpianto.SetAttribute("distanzaPali", CStr(distanzaPali))
            XmlImpianto.SetAttribute("dtFine", CStr(dtFine))
            XmlImpianto.SetAttribute("dtFineGestione", CStr(dtFineGestione))
            XmlImpianto.SetAttribute("dtInizio", CStr(dtInizio))
            XmlImpianto.SetAttribute("dtInizioGestione", CStr(dtInizioGestione))
            XmlImpianto.SetAttribute("dtIns", CStr(dtIns))
            XmlImpianto.SetAttribute("dtVar", CStr(dtVar))
            XmlImpianto.SetAttribute("fallanzePerc", CStr(fallanzePerc))
            XmlImpianto.SetAttribute("flagAnomalia", CStr(flagAnomalia))
            XmlImpianto.SetAttribute("flagAttuale", CStr(flagAttuale))
            XmlImpianto.SetAttribute("flagCessata", CStr(flagCessata))
            XmlImpianto.SetAttribute("flagContributo", CStr(flagContributo))
            XmlImpianto.SetAttribute("flagRegolarizz2009", CStr(flagRegolarizz2009))
            XmlImpianto.SetAttribute("flagRicalcoloGis", CStr(flagRicalcoloGis))
            XmlImpianto.SetAttribute("GiacituraTerreno", CStr(GiacituraTerreno))
            XmlImpianto.SetAttribute("idUnitaVitata", CStr(idUnitaVitata))
            XmlImpianto.SetAttribute("idUtenteIns", CStr(idUtenteIns))
            XmlImpianto.SetAttribute("idUtenteVar", CStr(idUtenteVar))
            XmlImpianto.SetAttribute("numeroProtocollo", CStr(numeroProtocollo))
            XmlImpianto.SetAttribute("progPoligono", CStr(progPoligono))
            XmlImpianto.SetAttribute("supVitataDich", CStr(supVitataDich))
            XmlImpianto.SetAttribute("supVitataDichPRCalcolo", CStr(supVitataDichPRCalcolo))
            XmlImpianto.SetAttribute("SuperficieServizioMq", CStr(SuperficieServizioMq))
            XmlImpianto.SetAttribute("Terrazzamenti", CStr(Terrazzamenti))
            XmlImpianto.SetAttribute("TipoColtura", CStr(TipoColtura))
            XmlImpianto.SetAttribute("tipoProcedimento", CStr(tipoProcedimento))
            XmlImpianto.SetAttribute("tipoUnar", CStr(tipoUnar))
            XmlImpianto.SetAttribute("tipoVariazione", CStr(tipoVariazione))
            XmlImpianto.SetAttribute("unar", CStr(unar))

            XmlImpianto.SetAttribute("Data_Inizio_Portinnesto", Data_Inizio_Portinnesto.ToShortDateString)

            XmlImpianto.SetAttribute("Data_Inizio_Impianto", Format(Data_Inizio_Impianto, "dd/MM/yyyy"))

            Return XmlImpianto

            'Distruggo gli oggetti
            XmlImpianto = Nothing


        Catch ex As Exception
            Log_Errori &= "XML_Impianto_Impianto. Errore durante la creazione dell'XML dell'Impianto: " & ex.Message
        End Try


    End Function


    '##########################################################################################
    Public Function XML_Impianto_Codice(ByRef Log_Errori As String,
                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                            ByVal Piva As String,
                                            ByVal Id_Cod As Integer,
                                            ByVal BaseCode As Integer,
                                            ByVal TopCode As Integer,
                                            Optional ByRef XmlDoc As XmlDocument = Nothing,
                                            Optional ByVal Sa_Cod As Integer = 0,
                                            Optional ByVal Appezza As Integer = 0,
                                            Optional ByVal Id_Reg As Integer = 0,
                                            Optional ByVal Val_Cod As String = "",
                                            Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
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
            NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            NodoXml.SetAttribute(LCase("piva"), CStr(Piva))
            NodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
            NodoXml.SetAttribute(LCase("appezza"), CStr(Appezza))
            NodoXml.SetAttribute(LCase("id_reg"), CStr(Id_Reg))
            NodoXml.SetAttribute(LCase("id_cod"), CStr(Id_Cod))
            NodoXml.SetAttribute(LCase("val_cod"), CStr(Val_Cod))
            NodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
            NodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
            NodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
            NodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))

            'Restituisco in uscita 
            Return NodoXml

            'Distruggo gli oggetti
            NodoXml = Nothing

        Catch ex As Exception
            Log_Errori &= "XML_Impianto_Codice. Errore durante la creazione dell'XML del codice dell'Impianto: " & ex.Message
        End Try


    End Function


    '##########################################################################################
    'Raccoglitore del progetto (distinta)
    '23/03/2012
    'aggiunto
    '    Optional ByVal Disciplinare_PrivatoPubblico As Integer = 0, _
    'Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0, _
    Public Function XML_ProgettiImpianto(ByRef Log_Errori As String,
                                        ByRef XmlDoc As XmlDocument,
                                        ByVal Flag_AggancioImpianto As Boolean,
                                        ByVal BaseCode As Integer,
                                        ByVal TopCode As Integer,
                                        ByVal DT_Codici_Progetto As DataTable,
                                        ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Appezza As Integer,
                                        ByVal Id_Reg As Integer,
                                        ByVal Progetto_Cod As Integer,
                                        ByVal Progetto_Nome As String,
                                        ByVal Progetto_Des As String,
                                        ByVal Cau_Progetto As Integer,
                                        ByVal Validita_Inizio As Date,
                                        ByVal Validita_Fine As Date,
                                        Optional ByVal Cod_Contratto As Integer = 0,
                                        Optional ByVal Cod_Conto As Integer = 0,
                                        Optional ByVal Ricavi_Previsti As Decimal = 0,
                                        Optional ByVal Produzione_Prevista As Decimal = 0,
                                        Optional ByVal Giudizio As String = "",
                                        Optional ByVal Veg_Cod As Integer = 0,
                                        Optional ByVal Grfi_Cod As Integer = 0,
                                        Optional ByVal CSProgetto_Cod As Integer = 0,
                                        Optional ByVal Stato_Impianto As enum_Stato_Impianto = enum_Stato_Impianto.Impianto_Produzione,
                                        Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno,
                                        Optional ByVal Disciplinare_Cod As Integer = 0,
                                        Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0,
                                        Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0,
                                        Optional ByVal Data_Inizio_Prevista As Date = AGRODATAINIZIO,
                                        Optional ByVal Data_Fine_Prevista As Date = AGRODATAFINE,
                                        Optional ByVal P_HA As Decimal = 0,
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

            If Not Flag_AggancioImpianto Then
                XmlDoc.AppendChild(XmlDatiProgetto)
            End If


            '#######################################################
            '##################    PROGETTO  #######################
            '#######################################################

            XmlProgetto = XML_ProgettoImpianto(Log_Errori,
                                                XmlDoc,
                                                BaseCode,
                                                TopCode,
                                                DT_Codici_Progetto,
                                                TipoOperazioneDB,
                                                Piva,
                                                Sa_Cod,
                                                Appezza,
                                                Id_Reg,
                                                Progetto_Cod,
                                                Progetto_Nome,
                                                Progetto_Des,
                                                Cau_Progetto,
                                                Validita_Inizio,
                                                Validita_Fine,
                                                Cod_Contratto,
                                                Cod_Conto,
                                                Ricavi_Previsti,
                                                Produzione_Prevista,
                                                Giudizio,
                                                Veg_Cod,
                                                Grfi_Cod,
                                                CSProgetto_Cod,
                                                Stato_Impianto,
                                                Regolamento_Cod,
                                                Disciplinare_Cod,
                                                Disciplinare_PubblicoPrivato,
                                                Regolamento_Concimazioni_Cod,
                                                Data_Inizio_Prevista,
                                                Data_Fine_Prevista,
                                                P_HA,
                                                Data_Fioritura_Prevista)


            XmlDatiProgetto.AppendChild(XmlProgetto)



        Catch ex As Exception
            Log_Errori &= "XML_ProgettiImpianto. Errore durante la creazione dell'XML della Distinta dell'Impianto: " & ex.Message
        End Try


        Return XmlDatiProgetto


    End Function


    '##########################################################################################
    'crea tutto il blocco del progetto (distinta)
    '23/03/2012
    'aggiunto
    '    Optional ByVal Disciplinare_PrivatoPubblico As Integer = 0, _
    'Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0, _
    Public Function XML_ProgettoImpianto(ByRef Log_Errori As String,
                                            ByRef XmlDoc As XmlDocument,
                                            ByVal BaseCode As Integer,
                                            ByVal TopCode As Integer,
                                            ByVal DT_Codici_Progetto As DataTable,
                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Appezza As Integer,
                                            ByVal Id_Reg As Integer,
                                            ByVal Progetto_Cod As Integer,
                                            ByVal Progetto_Nome As String,
                                            ByVal Progetto_Des As String,
                                            ByVal Cau_Progetto As Integer,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            Optional ByVal Cod_Contratto As Integer = 0,
                                            Optional ByVal Cod_Conto As Integer = 0,
                                            Optional ByVal Ricavi_Previsti As Decimal = 0,
                                            Optional ByVal Produzione_Prevista As Decimal = 0,
                                            Optional ByVal Giudizio As String = "",
                                            Optional ByVal Veg_Cod As Integer = 0,
                                            Optional ByVal Grfi_Cod As Integer = 0,
                                            Optional ByVal CSProgetto_Cod As Integer = 0,
                                            Optional ByVal Stato_Impianto As enum_Stato_Impianto = enum_Stato_Impianto.Impianto_Produzione,
                                            Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno,
                                            Optional ByVal Disciplinare_Cod As Integer = 0,
                                            Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0,
                                            Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0,
                                            Optional ByVal Data_Inizio_Prevista As Date = AGRODATAINIZIO,
                                            Optional ByVal Data_Fine_Prevista As Date = AGRODATAFINE,
                                            Optional ByVal P_HA As Decimal = 0,
                                            Optional ByVal Data_Fioritura_Prevista As Date = AGRODATAINIZIO,
                                            Optional ByVal P_HA_Maschi As Decimal = 0) As XmlElement


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

            XmlProgetto = XML_ProgettoImpianto_ProgettoImpianto(Log_Errori,
                                                                XmlDoc,
                                                                BaseCode,
                                                                TopCode,
                                                                TipoOperazioneDB,
                                                                Piva,
                                                                Sa_Cod,
                                                                Appezza,
                                                                Id_Reg,
                                                                Progetto_Cod,
                                                                Progetto_Nome,
                                                                Progetto_Des,
                                                                Cau_Progetto,
                                                                Validita_Inizio,
                                                                Validita_Fine,
                                                                Cod_Contratto,
                                                                Cod_Conto,
                                                                Ricavi_Previsti,
                                                                Produzione_Prevista,
                                                                Giudizio,
                                                                Veg_Cod,
                                                                Grfi_Cod,
                                                                CSProgetto_Cod,
                                                                Stato_Impianto,
                                                                Regolamento_Cod,
                                                                Disciplinare_Cod,
                                                                Disciplinare_PubblicoPrivato,
                                                                Regolamento_Concimazioni_Cod,
                                                                Data_Inizio_Prevista,
                                                                Data_Fine_Prevista,
                                                                P_HA,
                                                                Data_Fioritura_Prevista,
                                                                P_HA_Maschi)


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

                    XmlProgettoCodice = XML_ProgettoImpianto_Codice(Log_Errori,
                                                            TipoOperazioneDB_Codice,
                                                            Piva,
                                                            Id_Cod,
                                                            BaseCode,
                                                            TopCode,
                                                            XmlDoc,
                                                            Sa_Cod,
                                                            Appezza,
                                                            Id_Reg,
                                                            Progetto_Cod,
                                                            Val_Cod,
                                                            Validita_Inizio_Codice,
                                                            Validita_Fine_Codice)

                    XmlProgetto.AppendChild(XmlProgettoCodice)

                Next

            End If


        Catch ex As Exception
            Log_Errori &= "XML_ProgettoImpianto. Errore durante la creazione dell'XML della Distinta dell'Impianto: " & ex.Message
        End Try


        Return XmlProgetto


    End Function



    '#################################################################################################
    '23/03/2012
    'aggiunto
    '    Optional ByVal Disciplinare_PrivatoPubblico As Integer = 0, _
    'Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0, _
    Public Function XML_ProgettoImpianto_ProgettoImpianto(ByRef Log_Errori As String,
                                                            ByRef XmlDoc As XmlDocument,
                                                            ByVal BaseCode As Integer,
                                                            ByVal TopCode As Integer,
                                                            ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                            ByVal Piva As String,
                                                            ByVal Sa_Cod As Integer,
                                                            ByVal Appezza As Integer,
                                                            ByVal Id_Reg As Integer,
                                                            ByVal Progetto_Cod As Integer,
                                                            ByVal Progetto_Nome As String,
                                                            ByVal Progetto_Des As String,
                                                            ByVal Cau_Progetto As Integer,
                                                            ByVal Validita_Inizio As Date,
                                                            ByVal Validita_Fine As Date,
                                                            Optional ByVal Cod_Contratto As Integer = 0,
                                                            Optional ByVal Cod_Conto As Integer = 0,
                                                            Optional ByVal Ricavi_Previsti As Decimal = 0,
                                                            Optional ByVal Produzione_Prevista As Decimal = 0,
                                                            Optional ByVal Giudizio As String = "",
                                                            Optional ByVal Veg_Cod As Integer = 0,
                                                            Optional ByVal Grfi_Cod As Integer = 0,
                                                            Optional ByVal CSProgetto_Cod As Integer = 0,
                                                            Optional ByVal Stato_Impianto As Integer = 0,
                                                            Optional ByVal Regolamento_Cod As Integer = enum_Cod_Regolamento.Regolamento_Nessuno,
                                                            Optional ByVal Disciplinare_Cod As Integer = 0,
                                                            Optional ByVal Disciplinare_PubblicoPrivato As Integer = 0,
                                                            Optional ByVal Regolamento_Concimazioni_Cod As Integer = 0,
                                                            Optional ByVal Data_Inizio_Prevista As Date = AGRODATAINIZIO,
                                                            Optional ByVal Data_Fine_Prevista As Date = AGRODATAFINE,
                                                            Optional ByVal P_HA As Decimal = 0,
                                                            Optional ByVal Data_Fioritura_Prevista As Date = AGRODATAINIZIO,
                                                            Optional ByVal P_HA_Maschi As Decimal = 0) As XmlElement

        Dim XML_Progetto As System.Xml.XmlElement


        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If


            '----- Genero la stringa XML a partire dai valori dei parmetri

            XML_Progetto = XmlDoc.CreateElement("Progetto")

            XML_Progetto.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            XML_Progetto.SetAttribute("piva", CStr(Piva))
            XML_Progetto.SetAttribute("sa_cod", CStr(Sa_Cod))
            XML_Progetto.SetAttribute("appezza", CInt(Appezza))
            XML_Progetto.SetAttribute("id_reg", CInt(Id_Reg))
            XML_Progetto.SetAttribute("progetto_cod", CStr(Progetto_Cod))
            XML_Progetto.SetAttribute("progetto_nome", CStr(Progetto_Nome))
            XML_Progetto.SetAttribute("progetto_des", CStr(Progetto_Des))
            XML_Progetto.SetAttribute("cau_progetto", CInt(Cau_Progetto))

            XML_Progetto.SetAttribute("cod_contratto", CInt(Cod_Contratto))
            XML_Progetto.SetAttribute("cod_conto", CInt(Cod_Conto))
            XML_Progetto.SetAttribute("ricavi_previsti", CDbl(Ricavi_Previsti))
            XML_Progetto.SetAttribute("produzione_prevista", CDbl(Produzione_Prevista))
            XML_Progetto.SetAttribute("giudizio", CStr(Giudizio))
            XML_Progetto.SetAttribute("veg_cod", CInt(Veg_Cod))
            XML_Progetto.SetAttribute("grfi_cod", CInt(Grfi_Cod))
            XML_Progetto.SetAttribute("csprogetto_cod", CInt(CSProgetto_Cod))
            XML_Progetto.SetAttribute("stato_impianto", CInt(Stato_Impianto))
            XML_Progetto.SetAttribute("regolamento_cod", CInt(Regolamento_Cod))
            XML_Progetto.SetAttribute("disciplinare_cod", CInt(Disciplinare_Cod))

            XML_Progetto.SetAttribute("disciplinare_pubblicoprivato", CInt(Disciplinare_PubblicoPrivato))
            XML_Progetto.SetAttribute("regolamento_concimazioni_cod", CInt(Regolamento_Concimazioni_Cod))

            XML_Progetto.SetAttribute("p_ha", P_HA)
            XML_Progetto.SetAttribute("P_HA_Maschi", P_HA_Maschi)

            XML_Progetto.SetAttribute("data_inizio_prevista", CDate(Data_Inizio_Prevista))
            XML_Progetto.SetAttribute("data_fioritura_prevista", CDate(Data_Fioritura_Prevista))
            XML_Progetto.SetAttribute("data_fine_prevista", CDate(Data_Fine_Prevista))

            XML_Progetto.SetAttribute("validita_inizio", CDate(Validita_Inizio))
            XML_Progetto.SetAttribute("validita_fine", CDate(Validita_Fine))

            XML_Progetto.SetAttribute("basecode", CInt(BaseCode))
            XML_Progetto.SetAttribute("topcode", CInt(TopCode))

            Return XML_Progetto

            'Distruggo gli oggetti
            XML_Progetto = Nothing


        Catch ex As Exception
            Log_Errori &= "XML_ProgettoImpianto_ProgettoImpianto. Errore durante la creazione dell'XML della Distinta dell'Impianto: " & ex.Message
        End Try



    End Function


    '##########################################################################################
    Public Function XML_ProgettoImpianto_Codice(ByRef Log_Errori As String,
                                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                ByVal Piva As String,
                                                ByVal Id_Cod As Integer,
                                                ByVal BaseCode As Integer,
                                                ByVal TopCode As Integer,
                                                Optional ByRef XmlDoc As XmlDocument = Nothing,
                                                Optional ByVal Sa_Cod As Integer = 0,
                                                Optional ByVal Appezza As Integer = 0,
                                                Optional ByVal Id_Reg As Integer = 0,
                                                Optional ByVal Progetto_Cod As Integer = 0,
                                                Optional ByVal Val_Cod As String = "",
                                                Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
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
            NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            NodoXml.SetAttribute(LCase("piva"), CStr(Piva))
            NodoXml.SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
            NodoXml.SetAttribute(LCase("appezza"), CStr(Appezza))
            NodoXml.SetAttribute(LCase("id_reg"), CStr(Id_Reg))
            NodoXml.SetAttribute(LCase("Progetto_Cod"), CStr(Progetto_Cod))
            NodoXml.SetAttribute(LCase("id_cod"), CStr(Id_Cod))
            NodoXml.SetAttribute(LCase("val_cod"), CStr(Val_Cod))
            NodoXml.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
            NodoXml.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
            NodoXml.SetAttribute(LCase("basecode"), CStr(BaseCode))
            NodoXml.SetAttribute(LCase("topcode"), CStr(TopCode))

            'Restituisco in uscita 
            Return NodoXml

            'Distruggo gli oggetti
            NodoXml = Nothing

        Catch ex As Exception
            Log_Errori &= "XML_Impianto_Codice. Errore durante la creazione dell'XML del codice dell'Impianto: " & ex.Message
        End Try


    End Function


    '##########################################################################################
    'Viene chiamata quando si deve creare l'xml di un solo centro
    'crea il nodo DatiCentriAziendali
    'e poi chiama la funzione XML_Particella che crea tutto il blocco della aprticella (particella, zona, macrouso ecc)
    '---------------------
    'NOTA BENE!!!
    'zone e macrousi sonod a gestire!!!!!
    Public Function XML_Particelle(ByRef Log_Errori As String,
                                            ByRef XmlDoc As XmlDocument,
                                            ByVal BaseCode As Integer,
                                            ByVal TopCode As Integer,
                                            ByVal TipoOperazioneDB As String,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                             ByVal Prov As String,
                                            ByVal Com As String,
                                            ByVal Sezione As String,
                                            ByVal Foglio As Integer,
                                            ByVal Numero As Integer,
                                            ByVal Subalterno As String,
                                            ByVal Ettari As Decimal,
                                            ByVal Are As Integer,
                                            ByVal Centiare As Integer,
                                            ByVal TitoloPossesso As Integer,
                                            ByVal Sup_Condotta As Decimal,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            ByVal Validita_Inizio_Centro As Date,
                                            ByVal Validita_Fine_Centro As Date,
                                            ByVal DT_ParticelleZone As DataTable,
                                            ByVal DT_ParticelleMacrousi As DataTable,
                                            Optional ByVal Part_cod As Integer = 0,
                                            Optional ByVal Partita_Catastale As String = "",
                                            Optional ByVal Qualita_Cod As Integer = 0,
                                            Optional ByVal Classe As String = "",
                                            Optional ByVal Reddito_Dominicale As Decimal = 0,
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

            XmlParticella = XML_Particella(Log_Errori,
                                            XmlDoc,
                                            BaseCode,
                                            TopCode,
                                            TipoOperazioneDB,
                                            Piva,
                                            Sa_Cod,
                                            Prov,
                                            Com,
                                            Sezione,
                                            Foglio,
                                            Numero,
                                            Subalterno,
                                            Ettari,
                                            Are,
                                            Centiare,
                                            TitoloPossesso,
                                            Sup_Condotta,
                                            Validita_Inizio,
                                            Validita_Fine,
                                            Validita_Inizio_Centro,
                                            Validita_Fine_Centro,
                                            DT_ParticelleZone,
                                            DT_ParticelleMacrousi,
                                            Part_cod,
                                            Partita_Catastale,
                                            Qualita_Cod,
                                            Classe,
                                            Reddito_Dominicale,
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
    Public Function XML_Particella(ByRef Log_Errori As String,
                                            ByRef XmlDoc As XmlDocument,
                                            ByVal BaseCode As Integer,
                                            ByVal TopCode As Integer,
                                            ByVal TipoOperazioneDB As String,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                             ByVal Prov As String,
                                            ByVal Com As String,
                                            ByVal Sezione As String,
                                            ByVal Foglio As Integer,
                                            ByVal Numero As Integer,
                                            ByVal Subalterno As String,
                                            ByVal Ettari As Decimal,
                                            ByVal Are As Integer,
                                            ByVal Centiare As Integer,
                                            ByVal TitoloPossesso As Integer,
                                            ByVal Sup_Condotta As Decimal,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            ByVal Validita_Inizio_Centro As Date,
                                            ByVal Validita_Fine_Centro As Date,
                                            ByVal DT_ParticelleZone As DataTable,
                                            ByVal DT_ParticelleMacrousi As DataTable,
                                            Optional ByVal Part_cod As Integer = 0,
                                            Optional ByVal Partita_Catastale As String = "",
                                            Optional ByVal Qualita_Cod As Integer = 0,
                                            Optional ByVal Classe As String = "",
                                            Optional ByVal Reddito_Dominicale As Decimal = 0,
                                            Optional ByVal Reddito_Agrario As Decimal = 0) As XmlElement


        Dim XmlParticella As System.Xml.XmlElement
        'Dim i As Integer

        Try

            'CREAZIONE DOCUMENTO
            If IsNothing(XmlDoc) Then
                XmlDoc = New XmlDocument
            End If



            '#######################################################
            '###############   PARTICELLA    #################
            '#######################################################

            XmlParticella = XML_Particella_Particella(TipoOperazioneDB,
                                                                Piva,
                                                                Sa_Cod,
                                                                Prov,
                                                                Com,
                                                                Sezione,
                                                                Foglio,
                                                                Numero,
                                                                Subalterno,
                                                                Ettari,
                                                                Are,
                                                                Centiare,
                                                                TitoloPossesso,
                                                                Sup_Condotta,
                                                                Validita_Inizio,
                                                                Validita_Fine,
                                                                Validita_Inizio_Centro,
                                                                Validita_Fine_Centro,
                                                                Part_cod,
                                                                Partita_Catastale,
                                                                Qualita_Cod,
                                                                Classe,
                                                                Reddito_Dominicale,
                                                                Reddito_Agrario,
                                                                BaseCode,
                                                                TopCode,
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
    Public Function XML_Particella_Particella(ByVal TipoOperazioneDB As String,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                             ByVal Prov As String,
                                            ByVal Com As String,
                                            ByVal Sezione As String,
                                            ByVal Foglio As Integer,
                                            ByVal Numero As Integer,
                                            ByVal Subalterno As String,
                                            ByVal Ettari As Decimal,
                                            ByVal Are As Integer,
                                            ByVal Centiare As Integer,
                                            ByVal TitoloPossesso As Integer,
                                            ByVal Sup_Condotta As Decimal,
                                            ByVal Validita_Inizio As Date,
                                            ByVal Validita_Fine As Date,
                                            ByVal Validita_Inizio_Centro As Date,
                                            ByVal Validita_Fine_Centro As Date,
                                            Optional ByVal Part_cod As Integer = 0,
                                            Optional ByVal Partita_Catastale As String = "",
                                            Optional ByVal Qualita_Cod As Integer = 0,
                                            Optional ByVal Classe As String = "",
                                            Optional ByVal Reddito_Dominicale As Decimal = 0,
                                            Optional ByVal Reddito_Agrario As Decimal = 0,
                                            Optional ByVal BaseCode As Integer = 0,
                                            Optional ByVal TopCode As Integer = 200000000,
                                            Optional ByRef XmlDoc As XmlDocument = Nothing,
                                            Optional ByVal casiParticolari As String = "",
                                            Optional ByVal fasciaAltimetrica As String = "",
                                            Optional ByVal fasciaAltimetricaDescr As String = "",
                                            Optional ByVal Fonte As String = "",
                                            Optional ByVal FonteDescr As String = "",
                                            Optional ByVal tipoDocumento As String = "",
                                            Optional ByVal tipoDocumentoDescr As String = "",
                                            Optional ByVal utilizzo As String = "",
                                            Optional ByVal IDParticellaOrig As String = "",
                                            Optional ByVal Irrigabilita As String = "",
                                            Optional ByVal RotazioneColturale As String = "",
                                            Optional ByVal biologico As String = "",
                                            Optional ByVal flagAnomaliaMacrouso As String = "",
                                            Optional ByVal flagContenzioso As String = "",
                                            Optional ByVal flagSupero As String = "",
                                            Optional ByVal percEleggibile As String = ""
                                            ) As XmlElement

        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Particella")


        'Imposto gli attributi
        NodoXml.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
        NodoXml.SetAttribute("piva", CStr(Piva))
        NodoXml.SetAttribute("sa_cod", CStr(Sa_Cod))
        NodoXml.SetAttribute("part_cod", CStr(Part_cod))
        NodoXml.SetAttribute("prov", CStr(Prov))
        NodoXml.SetAttribute("com", CStr(Com))
        NodoXml.SetAttribute("sezione", CStr(Sezione))
        NodoXml.SetAttribute("foglio", CStr(Foglio))
        NodoXml.SetAttribute("numero", CStr(Numero))
        NodoXml.SetAttribute("subalterno", CStr(Subalterno))
        NodoXml.SetAttribute("partita_catastale", CStr(Partita_Catastale))
        NodoXml.SetAttribute("ettari", CStr(Ettari))
        NodoXml.SetAttribute("are", CStr(Are))
        NodoXml.SetAttribute("centiare", CStr(Centiare))
        NodoXml.SetAttribute("titolopossesso", CStr(TitoloPossesso))
        NodoXml.SetAttribute("qualita_cod", CStr(Qualita_Cod))
        NodoXml.SetAttribute("classe", CStr(Classe))
        NodoXml.SetAttribute("reddito_dominicale", CStr(Reddito_Dominicale))
        NodoXml.SetAttribute("reddito_agrario", CStr(Reddito_Agrario))
        NodoXml.SetAttribute("sup_condotta", CStr(Sup_Condotta))
        NodoXml.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
        NodoXml.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
        NodoXml.SetAttribute("validita_inizio_centro", Format(Validita_Inizio_Centro, "dd/MM/yyyy"))
        NodoXml.SetAttribute("validita_fine_centro", Format(Validita_Fine_Centro, "dd/MM/yyyy"))
        NodoXml.SetAttribute("basecode", CStr(BaseCode))
        NodoXml.SetAttribute("topcode", CStr(TopCode))
        NodoXml.SetAttribute("casiParticolari", CStr(casiParticolari))
        NodoXml.SetAttribute("fasciaAltimetrica", CStr(fasciaAltimetrica))
        NodoXml.SetAttribute("fasciaAltimetricaDescr", CStr(fasciaAltimetricaDescr))
        NodoXml.SetAttribute("Fonte", CStr(Fonte))
        NodoXml.SetAttribute("FonteDescr", CStr(FonteDescr))
        NodoXml.SetAttribute("tipoDocumento", CStr(tipoDocumento))
        NodoXml.SetAttribute("tipoDocumentoDescr", CStr(tipoDocumentoDescr))
        NodoXml.SetAttribute("utilizzo", CStr(utilizzo))
        NodoXml.SetAttribute("IDParticellaOrig", CStr(IDParticellaOrig))
        NodoXml.SetAttribute("Irrigabilita", CStr(Irrigabilita))
        NodoXml.SetAttribute("RotazioneColturale", CStr(RotazioneColturale))
        NodoXml.SetAttribute("biologico", CStr(biologico))
        NodoXml.SetAttribute("flagAnomaliaMacrouso", CStr(flagAnomaliaMacrouso))
        NodoXml.SetAttribute("flagContenzioso", CStr(flagContenzioso))
        NodoXml.SetAttribute("flagSupero", CStr(flagSupero))
        NodoXml.SetAttribute("percEleggibile", CStr(percEleggibile))
        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing



    End Function


    Public Sub XML_Campo_3(ByVal OperazioneRichiesta As enum_CodificaDecodifica,
                            ByRef StringaXML As String,
                            ByRef TipoOperazioneDB As enum_TipoOperazioneDB,
                            ByRef Piva As String,
                            ByRef Sa_Cod As Integer,
                            ByRef Campo_Cod As Long,
                            ByRef Campo_tipo As Integer,
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

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlCampo As System.Xml.XmlElement

        'Verifico quale operazione deve essere effettuata
        Select Case OperazioneRichiesta

            Case enum_CodificaDecodifica.Codifica

                '----- Genero la stringa XML a partire dai valori dei parametri

                'Creo il nodo 
                XmlCampo = XmlDoc.CreateElement("Campo")

                'Imposto gli attributi
                XmlCampo.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
                XmlCampo.SetAttribute("piva", CStr(Piva))
                XmlCampo.SetAttribute("sa_cod", CStr(Sa_Cod))
                XmlCampo.SetAttribute("campo_cod", CStr(Campo_Cod))
                XmlCampo.SetAttribute("campo_tipo", CStr(Campo_tipo))
                XmlCampo.SetAttribute("campo_des", Campo_Des)
                XmlCampo.SetAttribute("conversione_inizio", CStr(Conversione_Inizio))
                XmlCampo.SetAttribute("conversione_fine", CStr(Conversione_Fine))
                XmlCampo.SetAttribute("sau_totale", CStr(SAU_Totale))
                XmlCampo.SetAttribute("sau_biologico", CStr(SAU_Biologico))
                XmlCampo.SetAttribute("sau_conversione", CStr(SAU_Conversione))
                XmlCampo.SetAttribute("sau_convenzionale", CStr(SAU_Convenzionale))
                XmlCampo.SetAttribute("confinirischio", CStr(ConfiniRischio))
                XmlCampo.SetAttribute("gru_cod", CStr(Gru_Cod))
                XmlCampo.SetAttribute("veg_cod", CStr(Veg_Cod))
                XmlCampo.SetAttribute("validita_inizio", Format(Validita_Inizio, "dd/MM/yyyy"))
                XmlCampo.SetAttribute("validita_fine", Format(Validita_Fine, "dd/MM/yyyy"))
                XmlCampo.SetAttribute("basecode", CStr(BaseCode))
                XmlCampo.SetAttribute("topcode", CStr(TopCode))


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

    Public Function XML_2_Zoo_Animali(
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                ByVal PIVA As String,
                                ByVal sa_cod As Int32,
                                ByVal Cod_Progetto As Int32,
                                ByVal Matricola As String,
                                ByVal GEN_COD As Int32,
                                ByVal SPE_COD As Int32,
                                ByVal IPRO_COD As Int32,
                                ByVal RAZ_COD As Int32,
                                ByVal Nome As String,
                                ByVal Collare As String,
                                ByVal NOME_AIA As String,
                                ByVal MATRICOLA_AIA As String,
                                ByVal DAT_NASCITA As String,
                                ByVal PROV_NASCITA As String,
                                ByVal STATO_NASCITA As String,
                                ByVal AUA_AZI_NASCITA As String,
                                ByVal AUSL_AZI_NASCITA As String,
                                ByVal Sesso As String,
                                ByVal MAT_PADRE As String,
                                ByVal MAT_MADRE As String,
                                ByVal CF_PROPRIETARIO As String,
                                ByVal CF_DETENTORE As String,
                                ByVal PRESENTE As Int32,
                                ByVal inviato As Int32,
                                ByVal datainvio As DateTime,
                                ByVal Data_Creazione As DateTime,
                                ByVal Data_Modifica As DateTime,
                                ByVal Username_Creazione As String,
                                ByVal Username_Modifica As String,
                                ByVal Validita_Inizio As DateTime,
                                ByVal Validita_Fine As DateTime,
                                ByVal CAT_COD As Int32,
                                ByVal PESO As Double,
                                ByVal DATA_PESA As DateTime,
                                ByVal Metodo_Produzione As Int32,
                                ByVal Regolamento_Cod As Int32,
                                ByVal Conversione_Inizio As DateTime,
                                ByVal Conversione_Fine As DateTime,
                                ByVal Chk_Batteria As Int32,
                                ByVal codZootecnica As String,
                                ByVal descrZootecnica As String,
                                ByVal fonte As String,
                                ByVal ID_Utente As String,
                                ByVal DT_Variazione As DateTime,
                                Optional ByRef XmlDoc As XmlDocument = Nothing
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("ZooAnimali")


        With NodoXml

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("Piva"), CStr(PIVA))
            .SetAttribute(LCase("sa_cod"), CStr(sa_cod))
            .SetAttribute(LCase("Cod_Progetto"), CStr(Cod_Progetto))
            .SetAttribute(LCase("Matricola"), CStr(Matricola))
            .SetAttribute(LCase("GEN_COD"), CStr(GEN_COD))
            .SetAttribute(LCase("SPE_COD"), CStr(SPE_COD))
            .SetAttribute(LCase("IPRO_COD"), CStr(IPRO_COD))
            .SetAttribute(LCase("RAZ_COD"), CStr(RAZ_COD))
            .SetAttribute(LCase("Nome"), CStr(Nome))
            .SetAttribute(LCase("Collare"), CStr(Collare))
            .SetAttribute(LCase("NOME_AIA"), CStr(NOME_AIA))
            .SetAttribute(LCase("MATRICOLA_AIA"), CStr(MATRICOLA_AIA))
            .SetAttribute(LCase("DAT_NASCITA"), CStr(DAT_NASCITA))
            .SetAttribute(LCase("PROV_NASCITA"), CStr(PROV_NASCITA))
            .SetAttribute(LCase("STATO_NASCITA"), CStr(STATO_NASCITA))
            .SetAttribute(LCase("AUA_AZI_NASCITA"), CStr(AUA_AZI_NASCITA))
            .SetAttribute(LCase("AUSL_AZI_NASCITA"), CStr(AUSL_AZI_NASCITA))
            .SetAttribute(LCase("Sesso"), CStr(Sesso))
            .SetAttribute(LCase("MAT_PADRE"), CStr(MAT_PADRE))
            .SetAttribute(LCase("MAT_MADRE"), CStr(MAT_MADRE))
            .SetAttribute(LCase("CF_PROPRIETARIO"), CStr(CF_PROPRIETARIO))
            .SetAttribute(LCase("CF_DETENTORE"), CStr(CF_DETENTORE))
            .SetAttribute(LCase("PRESENTE"), CStr(PRESENTE))
            .SetAttribute(LCase("inviato"), CStr(inviato))
            .SetAttribute(LCase("datainvio"), CStr(datainvio))
            .SetAttribute(LCase("Data_Creazione"), CStr(Data_Creazione))
            .SetAttribute(LCase("Data_Modifica"), CStr(Data_Modifica))
            .SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), CStr(Username_Modifica))
            .SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))
            .SetAttribute(LCase("CAT_COD"), CStr(CAT_COD))
            .SetAttribute(LCase("PESO"), CStr(PESO))
            .SetAttribute(LCase("DATA_PESA"), CStr(DATA_PESA))
            .SetAttribute(LCase("Metodo_Produzione"), CStr(Metodo_Produzione))
            .SetAttribute(LCase("Regolamento_Cod"), CStr(Regolamento_Cod))
            .SetAttribute(LCase("Conversione_Inizio"), CStr(Conversione_Inizio))
            .SetAttribute(LCase("Conversione_Fine"), CStr(Conversione_Fine))
            .SetAttribute(LCase("Chk_Batteria"), CStr(Chk_Batteria))
            .SetAttribute(LCase("codZootecnica"), CStr(codZootecnica))
            .SetAttribute(LCase("descrZootecnica"), CStr(descrZootecnica))
            .SetAttribute(LCase("fonte"), CStr(fonte))
            .SetAttribute(LCase("ID_Utente"), CStr(ID_Utente))
            .SetAttribute(LCase("DT_Variazione"), CStr(DT_Variazione))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_Imprese_Contratti(
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                ByVal Piva As String,
                                ByVal Contratto_Cod As String,
                                ByVal Riferimento As String,
                                ByVal Superficie_Prevista As String,
                                ByVal Resa_Prevista As String,
                                ByVal Contratto_Nome As String,
                                ByVal Contratto_Numero As String,
                                ByVal Contratto_Des As String,
                                ByVal Cau_Contratto As String,
                                ByVal Cod_Conto As String,
                                ByVal Data_Inizio_Prevista As String,
                                ByVal Data_Fine_Prevista As String,
                                ByVal Descrizione_1 As String,
                                ByVal Descrizione_2 As String,
                                ByVal Giudizio As String,
                                ByVal Validita_Inizio As String,
                                ByVal Validita_Fine As String,
                                ByVal Inviato As String,
                                ByVal DataInvio As String,
                                ByVal Data_Creazione As String,
                                ByVal Data_Modifica As String,
                                ByVal Username_Creazione As String,
                                ByVal Username_Modifica As String,
                                ByVal Ricavi_Previsti As String,
                                ByVal Cod_Risum As String,
                                ByVal Stato As String,
                                ByVal ChkStato_Automatico As String,
                                ByVal Cau_Pagamento As String,
                                ByVal Data_Stipulazione As String,
                                ByVal Data_Variazione As DateTime,
                                ByVal prog As Int32,
                                ByVal id_doc As Int32,
                                ByVal id_contratto As Int32,
                                Optional ByRef XmlDoc As XmlDocument = Nothing
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("ImpreseContratti")


        With NodoXml

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("Piva"), CStr(Piva))
            .SetAttribute(LCase("Contratto_Cod"), CStr(Contratto_Cod))
            .SetAttribute(LCase("Riferimento"), CStr(Riferimento))
            .SetAttribute(LCase("Superficie_Prevista"), CStr(Superficie_Prevista))
            .SetAttribute(LCase("Resa_Prevista"), CStr(Resa_Prevista))
            .SetAttribute(LCase("Contratto_Nome"), CStr(Contratto_Nome))
            .SetAttribute(LCase("Contratto_Numero"), CStr(Contratto_Numero))
            .SetAttribute(LCase("Contratto_Des"), CStr(Contratto_Des))
            .SetAttribute(LCase("Cau_Contratto"), CStr(Cau_Contratto))
            .SetAttribute(LCase("Cod_Conto"), CStr(Cod_Conto))
            .SetAttribute(LCase("Data_Inizio_Prevista"), CStr(Data_Inizio_Prevista))
            .SetAttribute(LCase("Data_Fine_Prevista"), CStr(Data_Fine_Prevista))
            .SetAttribute(LCase("Descrizione_1"), CStr(Descrizione_1))
            .SetAttribute(LCase("Descrizione_2"), CStr(Descrizione_2))
            .SetAttribute(LCase("Giudizio"), CStr(Giudizio))
            .SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))
            .SetAttribute(LCase("Inviato"), CStr(Inviato))
            .SetAttribute(LCase("DataInvio"), CStr(DataInvio))
            .SetAttribute(LCase("Data_Creazione"), CStr(Data_Creazione))
            .SetAttribute(LCase("Data_Modifica"), CStr(Data_Modifica))
            .SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), CStr(Username_Modifica))
            .SetAttribute(LCase("Ricavi_Previsti"), CStr(Ricavi_Previsti))
            .SetAttribute(LCase("Cod_Risum"), CStr(Cod_Risum))
            .SetAttribute(LCase("Stato"), CStr(Stato))
            .SetAttribute(LCase("ChkStato_Automatico"), CStr(ChkStato_Automatico))
            .SetAttribute(LCase("Cau_Pagamento"), CStr(Cau_Pagamento))
            .SetAttribute(LCase("Data_Stipulazione"), CStr(Data_Stipulazione))
            .SetAttribute(LCase("prog"), CStr(prog))
            .SetAttribute(LCase("id_doc"), CStr(id_doc))
            .SetAttribute(LCase("id_contratto"), CStr(id_contratto))
            '.SetAttribute(LCase("dt_Inizio_Rinnovo"), CStr(dt_Inizio_Rinnovo))
            '.SetAttribute(LCase("dt_Fine_Rinnovo"), CStr(dt_Fine_Rinnovo))
            '.SetAttribute(LCase("prog_Rinnovato"), CStr(prog_Rinnovato))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_DirittiReimpianti(
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                ByVal PIVA As String,
                                ByVal codAutorizzazione As String,
                                ByVal dataProtocollo As Date,
                                ByVal dataRilascio As Date,
                                ByVal dataTermine As Date,
                                ByVal dtIns As Date,
                                ByVal dtVar As Date,
                                ByVal numDiritto As Integer,
                                ByVal numeroProtocollo As String,
                                ByVal praticaAutorizzata As String,
                                ByVal provRilascio As String,
                                ByVal provRilascioDescr As String,
                                ByVal provRilascioSigla As String,
                                ByVal supAutorizzata As Integer,
                                ByVal supImpiantata As Integer,
                                ByVal supResidua As Integer,
                                ByVal tipoDiritto As String,
                                ByVal tipoDirittoDescr As String,
                                ByVal tipoProcedimento As String,
                                ByVal inviato As Int32,
                                ByVal datainvio As DateTime,
                                ByVal Data_Creazione As DateTime,
                                ByVal Data_Modifica As DateTime,
                                ByVal Username_Creazione As String,
                                ByVal Username_Modifica As String,
                                ByVal Validita_Inizio As DateTime,
                                ByVal Validita_Fine As DateTime,
                                Optional ByRef XmlDoc As XmlDocument = Nothing
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("DirittiReimpianti")


        With NodoXml

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("PIVA"), CStr(PIVA))
            .SetAttribute(LCase("codAutorizzazione"), CStr(codAutorizzazione))
            .SetAttribute(LCase("dataProtocollo"), CStr(dataProtocollo))
            .SetAttribute(LCase("dataRilascio"), CStr(dataRilascio))
            .SetAttribute(LCase("dataTermine"), CStr(dataTermine))
            .SetAttribute(LCase("dtIns"), CStr(dtIns))
            .SetAttribute(LCase("dtVar"), CStr(dtVar))
            .SetAttribute(LCase("numDiritto"), CStr(numDiritto))
            .SetAttribute(LCase("numeroProtocollo"), CStr(numeroProtocollo))
            .SetAttribute(LCase("praticaAutorizzata"), CStr(praticaAutorizzata))
            .SetAttribute(LCase("provRilascio"), CStr(provRilascio))
            .SetAttribute(LCase("provRilascioDescr"), CStr(provRilascioDescr))
            .SetAttribute(LCase("provRilascioSigla"), CStr(provRilascioSigla))
            .SetAttribute(LCase("supAutorizzata"), CStr(supAutorizzata))
            .SetAttribute(LCase("supImpiantata"), CStr(supImpiantata))
            .SetAttribute(LCase("supResidua"), CStr(supResidua))
            .SetAttribute(LCase("tipoDiritto"), CStr(tipoDiritto))
            .SetAttribute(LCase("tipoDirittoDescr"), CStr(tipoDirittoDescr))
            .SetAttribute(LCase("tipoProcedimento"), CStr(tipoProcedimento))
            .SetAttribute(LCase("inviato"), CStr(inviato))
            .SetAttribute(LCase("datainvio"), CStr(datainvio))
            .SetAttribute(LCase("Data_Creazione"), CStr(Data_Creazione))
            .SetAttribute(LCase("Data_Modifica"), CStr(Data_Modifica))
            .SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), CStr(Username_Modifica))
            .SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_Conto(ByVal BaseCode As Integer,
                                ByVal TopCode As Integer,
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                ByVal Piva As String,
                                ByVal Cod_Conto As Integer,
                                ByVal Cod_Contatto As String,
                                ByVal Validita_Inizio As DateTime,
                                ByVal Validita_Fine As DateTime,
                                Optional ByRef XmlDoc As XmlDocument = Nothing
                                ) As XmlElement

        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If

        NodoXml = XmlDoc.CreateElement("DatiConti")

        With NodoXml
            .SetAttribute("basecode", CStr(BaseCode))
            .SetAttribute("topcode", CStr(TopCode))
            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute("piva", CStr(Piva))
            .SetAttribute("Cod_Conto", CStr(Cod_Conto))
            .SetAttribute("Cod_Contatto", Cod_Contatto)
            .SetAttribute("Validita_Inizio", CStr(Validita_Inizio))
            .SetAttribute("Validita_Fine", CStr(Validita_Fine))
        End With

        Return NodoXml

        NodoXml = Nothing

    End Function
    Public Function XML_2_Liquidita(
                                ByVal BaseCode As Integer,
                                ByVal TopCode As Integer,
                                ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Cod_Liquidita As Integer,
                                ByVal Riferimento As String,
                                ByVal Numero As String,
                                ByVal Abi As String,
                                ByVal Cab As String,
                                ByVal Interbancario As String,
                                ByVal Saldo_Attuale As Double,
                                ByVal Saldo_Iniziale As Double,
                                ByVal Cau_Risorsa As String,
                                ByVal Cod_Istituto As Integer,
                                ByVal Avviso As String,
                                ByVal Importo_Avviso As Double,
                                ByVal Note As String,
                                ByVal inviato As Int32,
                                ByVal datainvio As DateTime,
                                ByVal Username_Creazione As String,
                                ByVal Username_Modifica As String,
                                ByVal Validita_Inizio As DateTime,
                                ByVal Validita_Fine As DateTime,
                                ByVal Cin As String,
                                ByVal Cifre_Controllo As String,
                                ByVal Nazione As String,
                                ByVal Bic As String,
                                ByVal Cod_Contatto As String,
                                ByVal Rilevamento As Double,
                                ByVal Data_Rilevamento As DateTime,
                                ByVal Offset As Double,
                                ByVal ChkDefault As Int32,
                                ByVal ChkAbilitazione As Int32,
                                ByVal anomalo As String,
                                ByVal codAnomalia As String,
                                ByVal dataAnomalia As String,
                                ByVal dataFine As String,
                                ByVal dataFineSportello As String,
                                ByVal dataInizioSportello As String,
                                ByVal dataInserimento As String,
                                ByVal DataVariazione As String,
                                ByVal descrAnomalia As String,
                                ByVal flagTesoriere As String,
                                ByVal Preferito As String,
                                ByVal progr As String,
                                Optional ByRef XmlDoc As XmlDocument = Nothing
                                    ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("DatiLiquidita")

        With NodoXml

            .SetAttribute("basecode", CStr(BaseCode))
            .SetAttribute("topcode", CStr(TopCode))
            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("piva"), CStr(Piva))
            .SetAttribute(LCase("sa_cod"), CStr(Sa_Cod))
            .SetAttribute(LCase("Cod_Liquidita"), CStr(Cod_Liquidita))
            .SetAttribute(LCase("riferimento"), CStr(Riferimento))
            .SetAttribute(LCase("numero"), CStr(Numero))
            .SetAttribute(LCase("abi"), CStr(Abi))
            .SetAttribute(LCase("cab"), CStr(Cab))
            .SetAttribute(LCase("interbancario"), CStr(Interbancario))
            .SetAttribute(LCase("saldo_attuale"), CStr(Saldo_Attuale))
            .SetAttribute(LCase("saldo_iniziale"), CStr(Saldo_Iniziale))
            .SetAttribute(LCase("cau_risorsa"), CStr(Cau_Risorsa))
            .SetAttribute(LCase("cod_istituto"), CStr(Cod_Istituto))
            .SetAttribute(LCase("avviso"), CStr(Avviso))
            .SetAttribute(LCase("importo_avviso"), CStr(Importo_Avviso))
            .SetAttribute(LCase("note"), CStr(Note))
            .SetAttribute(LCase("Inviato"), CStr(inviato))
            .SetAttribute(LCase("DataInvio"), CStr(datainvio))
            .SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), CStr(Username_Modifica))
            .SetAttribute(LCase("validita_inizio"), CStr(Validita_Inizio))
            .SetAttribute(LCase("validita_fine"), CStr(Validita_Fine))
            .SetAttribute(LCase("cin"), CStr(Cin))
            .SetAttribute(LCase("cifre_controllo"), CStr(Cifre_Controllo))
            .SetAttribute(LCase("nazione"), CStr(Nazione))
            .SetAttribute(LCase("bic"), CStr(Bic))
            .SetAttribute(LCase("cod_contatto"), CStr(Cod_Contatto))
            .SetAttribute(LCase("Rilevamento"), CStr(Rilevamento))
            .SetAttribute(LCase("Data_Rilevamento"), CStr(Data_Rilevamento))
            .SetAttribute(LCase("Offset"), CStr(Offset))
            .SetAttribute(LCase("ChkDefault"), CStr(ChkDefault))
            .SetAttribute(LCase("ChkAbilitazione"), CStr(ChkAbilitazione))
            .SetAttribute(LCase("anomalo"), CStr(anomalo))
            .SetAttribute(LCase("codAnomalia"), CStr(codAnomalia))
            .SetAttribute(LCase("dataAnomalia"), CStr(dataAnomalia))
            .SetAttribute(LCase("dataFine"), CStr(dataFine))
            .SetAttribute(LCase("dataFineSportello"), CStr(dataFineSportello))
            .SetAttribute(LCase("dataInizioSportello"), CStr(dataInizioSportello))
            .SetAttribute(LCase("dataInserimento"), CStr(dataInserimento))
            .SetAttribute(LCase("DataVariazione"), CStr(DataVariazione))
            .SetAttribute(LCase("descrAnomalia"), CStr(descrAnomalia))
            .SetAttribute(LCase("flagTesoriere"), CStr(flagTesoriere))
            .SetAttribute(LCase("Preferito"), CStr(Preferito))
            .SetAttribute(LCase("progr"), CStr(progr))

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

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("Piva"), CStr(Piva))
            .SetAttribute(LCase("Sa_Cod"), CStr(Sa_Cod))
            .SetAttribute(LCase("Cod_Istituto"), CStr(Cod_Istituto))
            .SetAttribute(LCase("Istituto_Des"), CStr(Istituto_Des))
            .SetAttribute(LCase("Filiale"), CStr(Filiale))
            .SetAttribute(LCase("Per_Risorsa"), CStr(Per_Risorsa))
            .SetAttribute(LCase("Inviato"), CStr(Inviato))
            .SetAttribute(LCase("DataInvio"), CStr(DataInvio))
            .SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), CStr(Username_Modifica))
            .SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))
            .SetAttribute(LCase("cod_Contatto"), CStr(cod_Contatto))

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

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("Piva"), CStr(Piva))
            .SetAttribute(LCase("Denominazione"), CStr(Denominazione))
            .SetAttribute(LCase("codFiscaleCAA"), CStr(codFiscaleCAA))
            .SetAttribute(LCase("idCAA"), CStr(idCAA))
            .SetAttribute(LCase("Cod_Indirizzo"), CStr(Cod_Indirizzo))
            .SetAttribute(LCase("inviato"), CStr(inviato))
            .SetAttribute(LCase("datainvio"), CStr(datainvio))
            .SetAttribute(LCase("Data_Creazione"), CStr(Data_Creazione))
            .SetAttribute(LCase("Data_Modifica"), CStr(Data_Modifica))
            .SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), CStr(Username_Modifica))
            .SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))

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

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("Piva"), CStr(Piva))
            .SetAttribute(LCase("Sa_Cod"), CStr(Sa_Cod))
            .SetAttribute(LCase("Cod_RisUm"), CStr(Cod_RisUm))
            .SetAttribute(LCase("Cod_Contatto"), CStr(Cod_Contatto))
            .SetAttribute(LCase("Cod_Rapporto"), CStr(Cod_Rapporto))
            .SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))
            .SetAttribute(LCase("Settore_Des"), CStr(Settore_Des))
            .SetAttribute(LCase("Attivita_Des"), CStr(Attivita_Des))
            .SetAttribute(LCase("Corrispettivo_Mensile"), CStr(Corrispettivo_Mensile))
            .SetAttribute(LCase("Corrispettivo_Orario"), CStr(Corrispettivo_Orario))
            .SetAttribute(LCase("Occasionale"), CStr(Occasionale))
            .SetAttribute(LCase("Ore_Settimanali"), CStr(Ore_Settimanali))
            .SetAttribute(LCase("Giorni_Ferie"), CStr(Giorni_Ferie))
            .SetAttribute(LCase("Ferie_Godute"), CStr(Ferie_Godute))
            .SetAttribute(LCase("Giorni_Malattia"), CStr(Giorni_Malattia))
            .SetAttribute(LCase("Inviato"), CStr(Inviato))
            .SetAttribute(LCase("DataInvio"), CStr(DataInvio))
            .SetAttribute(LCase("Data_Creazione"), CStr(Data_Creazione))
            .SetAttribute(LCase("Data_Modifica"), CStr(Data_Modifica))
            .SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), CStr(Username_Modifica))
            .SetAttribute(LCase("Patentino"), CStr(Patentino))
            .SetAttribute(LCase("Data_Rilascio_Patentinvo"), CStr(Data_Rilascio_Patentinvo))
            .SetAttribute(LCase("Data_Scadenza_Patentino"), CStr(Data_Scadenza_Patentino))
            .SetAttribute(LCase("Cod_RisUm_Origine"), CStr(Cod_RisUm_Origine))
            .SetAttribute(LCase("Piva_SuperUser_Origine"), CStr(Piva_SuperUser_Origine))
            .SetAttribute(LCase("Ente_di_rilascio"), CStr(Ente_di_rilascio))
            .SetAttribute(LCase("Saldo_Iniziale_Crediti"), CStr(Saldo_Iniziale_Crediti))
            .SetAttribute(LCase("Saldo_Iniziale_Debiti"), CStr(Saldo_Iniziale_Debiti))
            .SetAttribute(LCase("ChkSpesometro"), CStr(ChkSpesometro))
            .SetAttribute(LCase("ChkBlocco"), CStr(ChkBlocco))
            .SetAttribute(LCase("Blocco_Des"), CStr(Blocco_Des))
            .SetAttribute(LCase("Qualifica_Cod"), CStr(Qualifica_Cod))
            .SetAttribute(LCase("Mansione_Cod"), CStr(Mansione_Cod))
            .SetAttribute(LCase("Info_Famiglia"), CStr(Info_Famiglia))
            .SetAttribute(LCase("codRuolo"), CStr(Piva))
            .SetAttribute(LCase("ruoloDescr"), CStr(Piva))
            .SetAttribute(LCase("dtVariazioneRuolo"), CStr(Piva))
            .SetAttribute(LCase("fonte"), CStr(Piva))
            .SetAttribute(LCase("fonteDescr"), CStr(Piva))

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

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("Piva"), CStr(Piva))
            .SetAttribute(LCase("CodMacroarea"), CStr(CodMacroarea))
            .SetAttribute(LCase("CodProduzione"), CStr(CodProduzione))
            .SetAttribute(LCase("DescrMacroarea"), CStr(DescrMacroarea))
            .SetAttribute(LCase("DescrProduzione"), CStr(DescrProduzione))
            .SetAttribute(LCase("Fonte"), CStr(Fonte))
            .SetAttribute(LCase("ID_Utente"), CStr(ID_Utente))
            .SetAttribute(LCase("DT_Variazione"), CStr(DT_Variazione))
            .SetAttribute(LCase("Inviato"), CStr(Inviato))
            .SetAttribute(LCase("DataInvio"), CStr(DataInvio))
            .SetAttribute(LCase("Data_Creazione"), CStr(Data_Creazione))
            .SetAttribute(LCase("Data_Modifica"), CStr(Data_Modifica))
            .SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), CStr(Username_Modifica))
            .SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))

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

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("ID"), CStr(ID))
            .SetAttribute(LCase("PIVA"), CStr(PIVA))
            .SetAttribute(LCase("sa_cod"), CStr(sa_cod))
            .SetAttribute(LCase("PROV"), CStr(PROV))
            .SetAttribute(LCase("COM"), CStr(COM))
            .SetAttribute(LCase("SEZIONE"), CStr(SEZIONE))
            .SetAttribute(LCase("FOGLIO"), CStr(FOGLIO))
            .SetAttribute(LCase("NUMERO"), CStr(NUMERO))
            .SetAttribute(LCase("SUBALTERNO"), CStr(SUBALTERNO))
            .SetAttribute(LCase("TitoloPossesso"), CStr(TitoloPossesso))
            .SetAttribute(LCase("PARTITA_CATASTALE"), CStr(PARTITA_CATASTALE))
            .SetAttribute(LCase("inviato"), CStr(inviato))
            .SetAttribute(LCase("datainvio"), CStr(datainvio))
            .SetAttribute(LCase("Data_Creazione"), CStr(Data_Creazione))
            .SetAttribute(LCase("Data_Modifica"), CStr(Data_Modifica))
            .SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), CStr(Username_Modifica))
            .SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))
            .SetAttribute(LCase("Validazione"), CStr(Validazione))
            .SetAttribute(LCase("Data_Validazione"), CStr(Data_Validazione))
            .SetAttribute(LCase("UserName_Validazione"), CStr(UserName_Validazione))
            .SetAttribute(LCase("Sup_Condotta"), CStr(Sup_Condotta))
            .SetAttribute(LCase("Sup_Spandibile"), CStr(Sup_Spandibile))
            .SetAttribute(LCase("Sup_Divieto"), CStr(Sup_Divieto))
            .SetAttribute(LCase("Irrigabilita"), CStr(Irrigabilita))
            .SetAttribute(LCase("RotazioneColturale"), CStr(RotazioneColturale))
            .SetAttribute(LCase("biologico"), CStr(biologico))
            .SetAttribute(LCase("flagAnomaliaMacrouso"), CStr(flagAnomaliaMacrouso))
            .SetAttribute(LCase("flagContenzioso"), CStr(flagContenzioso))
            .SetAttribute(LCase("flagSupero"), CStr(flagSupero))

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

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("PROV"), CStr(PROV))
            .SetAttribute(LCase("COM"), CStr(COM))
            .SetAttribute(LCase("SEZIONE"), CStr(SEZIONE))
            .SetAttribute(LCase("FOGLIO"), CStr(FOGLIO))
            .SetAttribute(LCase("NUMERO"), CStr(NUMERO))
            .SetAttribute(LCase("SUBALTERNO"), CStr(SUBALTERNO))
            .SetAttribute(LCase("Eleggibilita_Cod"), CStr(Eleggibilita_Cod))
            .SetAttribute(LCase("Superficie"), CStr(Superficie))
            .SetAttribute(LCase("inviato"), CStr(inviato))
            .SetAttribute(LCase("datainvio"), CStr(datainvio))
            .SetAttribute(LCase("Data_Creazione"), CStr(Data_Creazione))
            .SetAttribute(LCase("Data_Modifica"), CStr(Data_Modifica))
            .SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), CStr(Username_Modifica))
            .SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))

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

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("Piva"), CStr(Piva))
            .SetAttribute(LCase("Contratto_Cod"), CStr(Contratto_Cod))
            .SetAttribute(LCase("Fase_Cod"), CStr(Fase_Cod))
            .SetAttribute(LCase("Fase_Des"), CStr(Fase_Des))
            .SetAttribute(LCase("Elem_Cod"), CStr(Elem_Cod))
            .SetAttribute(LCase("Pro_Cod"), CStr(Pro_Cod))
            .SetAttribute(LCase("Mat_Cod"), CStr(Mat_Cod))
            .SetAttribute(LCase("Udm_Cod"), CStr(Udm_Cod))
            .SetAttribute(LCase("Progetto_Cod"), CStr(Progetto_Cod))
            .SetAttribute(LCase("Cal_Cod"), CStr(Cal_Cod))
            .SetAttribute(LCase("Lotto"), CStr(Lotto))
            .SetAttribute(LCase("Qta"), CStr(Qta))
            .SetAttribute(LCase("Data_Inizio_Prevista"), CStr(Data_Inizio_Prevista))
            .SetAttribute(LCase("Data_Fine_Prevista"), CStr(Data_Fine_Prevista))
            .SetAttribute(LCase("Importo"), CStr(Importo))
            .SetAttribute(LCase("Giudizio"), CStr(Giudizio))
            .SetAttribute(LCase("inviato"), CStr(inviato))
            .SetAttribute(LCase("datainvio"), CStr(datainvio))
            .SetAttribute(LCase("Data_Creazione"), CStr(Data_Creazione))
            .SetAttribute(LCase("Data_Modifica"), CStr(Data_Modifica))
            .SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), CStr(Username_Modifica))
            .SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))
            .SetAttribute(LCase("Listino_Cod"), CStr(Listino_Cod))
            .SetAttribute(LCase("Valore1"), CStr(Valore1))
            .SetAttribute(LCase("Valore2"), CStr(Valore2))
            .SetAttribute(LCase("Valore3"), CStr(Valore3))
            .SetAttribute(LCase("Valore4"), CStr(Valore4))
            .SetAttribute(LCase("Valore5"), CStr(Valore5))
            .SetAttribute(LCase("Valore6"), CStr(Valore6))
            .SetAttribute(LCase("Valore7"), CStr(Valore7))
            .SetAttribute(LCase("Valore8"), CStr(Valore8))
            .SetAttribute(LCase("Valore9"), CStr(Valore9))

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

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("PROV"), CStr(PROV))
            .SetAttribute(LCase("COM"), CStr(COM))
            .SetAttribute(LCase("SEZIONE"), CStr(SEZIONE))
            .SetAttribute(LCase("FOGLIO"), CStr(FOGLIO))
            .SetAttribute(LCase("NUMERO"), CStr(NUMERO))
            .SetAttribute(LCase("SUBALTERNO"), CStr(SUBALTERNO))
            .SetAttribute(LCase("Macrouso_Cod"), CStr(Macrouso_Cod))
            .SetAttribute(LCase("Superficie"), CStr(Superficie))
            .SetAttribute(LCase("inviato"), CStr(inviato))
            .SetAttribute(LCase("datainvio"), CStr(datainvio))
            .SetAttribute(LCase("Data_Creazione"), CStr(Data_Creazione))
            .SetAttribute(LCase("Data_Modifica"), CStr(Data_Modifica))
            .SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), CStr(Username_Modifica))
            .SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))
            .SetAttribute(LCase("ID"), CStr(ID))
            .SetAttribute(LCase("PIVA"), CStr(PIVA))
            .SetAttribute(LCase("Numero_Fascicolo"), CStr(Numero_Fascicolo))
            .SetAttribute(LCase("Data_Validazione_Fascicolo"), CStr(Data_Validazione_Fascicolo))
            .SetAttribute(LCase("Fonte"), CStr(Fonte))
            .SetAttribute(LCase("FonteDescr"), CStr(FonteDescr))

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

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("ID"), CStr(ID))
            .SetAttribute(LCase("Tipo_Contatto"), CStr(Tipo_Contatto))
            .SetAttribute(LCase("Cod_RisUm"), CStr(Cod_RisUm))
            .SetAttribute(LCase("Piva"), CStr(Piva))
            .SetAttribute(LCase("Sa_Cod"), CStr(Sa_Cod))
            .SetAttribute(LCase("PROV"), CStr(PROV))
            .SetAttribute(LCase("COM"), CStr(COM))
            .SetAttribute(LCase("SEZIONE"), CStr(SEZIONE))
            .SetAttribute(LCase("FOGLIO"), CStr(FOGLIO))
            .SetAttribute(LCase("NUMERO"), CStr(NUMERO))
            .SetAttribute(LCase("SUBALTERNO"), CStr(SUBALTERNO))
            .SetAttribute(LCase("Quota"), CStr(Quota))
            .SetAttribute(LCase("inviato"), CStr(inviato))
            .SetAttribute(LCase("datainvio"), CStr(datainvio))
            .SetAttribute(LCase("Data_Creazione"), CStr(Data_Creazione))
            .SetAttribute(LCase("Data_Modifica"), CStr(Data_Modifica))
            .SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), CStr(Username_Modifica))
            .SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))
            .SetAttribute(LCase("cuaaProprietario"), CStr(cuaaProprietario))
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

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("Piva_SuperUser"), CStr(Piva_SuperUser))
            .SetAttribute(LCase("Zona_Cod"), CStr(Zona_Cod))
            .SetAttribute(LCase("PROV"), CStr(PROV))
            .SetAttribute(LCase("COM"), CStr(COM))
            .SetAttribute(LCase("SEZIONE"), CStr(SEZIONE))
            .SetAttribute(LCase("FOGLIO"), CStr(FOGLIO))
            .SetAttribute(LCase("NUMERO"), CStr(NUMERO))
            .SetAttribute(LCase("SUBALTERNO"), CStr(SUBALTERNO))
            .SetAttribute(LCase("Area"), CStr(Area))
            .SetAttribute(LCase("inviato"), CStr(inviato))
            .SetAttribute(LCase("datainvio"), CStr(datainvio))
            .SetAttribute(LCase("Data_Creazione"), CStr(Data_Creazione))
            .SetAttribute(LCase("Data_Modifica"), CStr(Data_Modifica))
            .SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), CStr(Username_Modifica))
            .SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))
            .SetAttribute(LCase("Fonte"), CStr(Fonte))
            .SetAttribute(LCase("FonteDescr"), CStr(FonteDescr))
            .SetAttribute(LCase("Conforme"), CStr(Conforme))

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

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("Programmazione_Entita_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Programmazione_Entita_Cod))
            .SetAttribute(LCase("Programmazione_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Programmazione_Cod))
            .SetAttribute(LCase("Entita_Des"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Entita_Des))
            .SetAttribute(LCase("Piva"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Piva))
            .SetAttribute(LCase("Sa_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Sa_Cod))
            .SetAttribute(LCase("Campo_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Campo_Cod))
            .SetAttribute(LCase("Appezza"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Appezza))
            .SetAttribute(LCase("Id_Reg"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Id_Reg))
            .SetAttribute(LCase("Progetto_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Progetto_Cod))
            .SetAttribute(LCase("Progetto_Des"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Progetto_Des))
            .SetAttribute(LCase("Id_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Id_Cod))
            .SetAttribute(LCase("Veg_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Veg_Cod))
            .SetAttribute(LCase("Cul_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Cul_Cod))
            .SetAttribute(LCase("Grfi_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Grfi_Cod))
            .SetAttribute(LCase("Cop_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Cop_Cod))
            .SetAttribute(LCase("Superficie"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Superficie))
            .SetAttribute(LCase("Resa"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Resa))
            .SetAttribute(LCase("TipoZona"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(TipoZona))
            .SetAttribute(LCase("Veg_Cod_Prec"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Veg_Cod_Prec))
            .SetAttribute(LCase("Id_Mat_O"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Id_Mat_O))
            .SetAttribute(LCase("Id_Fre"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Id_Fre))
            .SetAttribute(LCase("N_distribuito"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(N_distribuito))
            .SetAttribute(LCase("inviato"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(inviato))
            .SetAttribute(LCase("Data_Creazione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Data_Creazione))
            .SetAttribute(LCase("Data_Modifica"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Data_Modifica))
            .SetAttribute(LCase("Username_Creazione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Username_Modifica))
            .SetAttribute(LCase("Validita_Inizio"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Validita_Fine))
            .SetAttribute(LCase("Num_Piante"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Num_Piante))
            .SetAttribute(LCase("Stato_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Stato_Cod))
            .SetAttribute(LCase("Ciclo"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Ciclo))
            .SetAttribute(LCase("Grva_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Grva_Cod))
            .SetAttribute(LCase("Data_Semina"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Data_Semina))
            .SetAttribute(LCase("Data_Raccolta"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Data_Raccolta))
            .SetAttribute(LCase("Note"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Note))
            .SetAttribute(LCase("Veg_Cod_Cliente"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Veg_Cod_Cliente))
            .SetAttribute(LCase("Cul_Cod_Cliente"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Cul_Cod_Cliente))
            .SetAttribute(LCase("N_fabbisogno"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(N_fabbisogno))
            .SetAttribute(LCase("Foral_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Foral_Cod))
            .SetAttribute(LCase("Port_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Port_Cod))
            .SetAttribute(LCase("Imp_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Imp_Cod))
            .SetAttribute(LCase("Regolamento_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Regolamento_Cod))
            .SetAttribute(LCase("Disciplinare_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Disciplinare_Cod))
            .SetAttribute(LCase("TRA_Fila"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(TRA_Fila))
            .SetAttribute(LCase("SU_Fila"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(SU_Fila))
            .SetAttribute(LCase("MetodoProduzione_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(MetodoProduzione_Cod))
            .SetAttribute(LCase("FlagIrrigabilita"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(FlagIrrigabilita))
            .SetAttribute(LCase("FlagSecondoRaccolto"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(FlagSecondoRaccolto))
            .SetAttribute(LCase("Codice_Fiscale_Tecnico"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Codice_Fiscale_Tecnico))
            .SetAttribute(LCase("Superficie_Futura"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Superficie_Futura))
            .SetAttribute(LCase("Operazione_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Operazione_Cod))
            .SetAttribute(LCase("Macrouso_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Macrouso_Cod))
            .SetAttribute(LCase("Via_Stringa"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Via_Stringa))
            .SetAttribute(LCase("Unita_Vitata"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Unita_Vitata))
            .SetAttribute(LCase("Validita_Inizio_Impianto"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Validita_Inizio_Impianto))
            .SetAttribute(LCase("Zslm"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Zslm))
            .SetAttribute(LCase("AltriVitigniPresenti"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(AltriVitigniPresenti))
            .SetAttribute(LCase("ancoraggiTestata"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(ancoraggiTestata))
            .SetAttribute(LCase("annoRiferimento"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(annoRiferimento))
            .SetAttribute(LCase("codFiliStostegno"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(codFiliStostegno))
            .SetAttribute(LCase("codPaliTessitura"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(codPaliTessitura))
            .SetAttribute(LCase("codPaliTestata"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(codPaliTestata))
            .SetAttribute(LCase("codStatoColt"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(codStatoColt))
            .SetAttribute(LCase("codTipoVari"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(codTipoVari))
            .SetAttribute(LCase("DataProtocollo"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(DataProtocollo))
            .SetAttribute(LCase("DataRilievo"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(DataRilievo))
            .SetAttribute(LCase("destProduttiva"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(destProduttiva))
            .SetAttribute(LCase("destProduttivaDescr"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(destProduttivaDescr))
            .SetAttribute(LCase("distanzaPali"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(distanzaPali))
            .SetAttribute(LCase("dtFine"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtFine))
            .SetAttribute(LCase("dtFineGestione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtFineGestione))
            .SetAttribute(LCase("dtInizio"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtInizio))
            .SetAttribute(LCase("dtInizioGestione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtInizioGestione))
            .SetAttribute(LCase("dtIns"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtIns))
            .SetAttribute(LCase("dtVar"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtVar))
            .SetAttribute(LCase("fallanzePerc"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(fallanzePerc))
            .SetAttribute(LCase("flagAnomalia"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(flagAnomalia))
            .SetAttribute(LCase("flagAttuale"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(flagAttuale))
            .SetAttribute(LCase("flagCessata"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(flagCessata))
            .SetAttribute(LCase("flagContributo"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(flagContributo))
            .SetAttribute(LCase("flagRegolarizz2009"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(flagRegolarizz2009))
            .SetAttribute(LCase("flagRicalcoloGis"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(flagRicalcoloGis))
            .SetAttribute(LCase("GiacituraTerreno"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(GiacituraTerreno))
            .SetAttribute(LCase("idUnitaVitata"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(idUnitaVitata))
            .SetAttribute(LCase("idUtenteIns"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(idUtenteIns))
            .SetAttribute(LCase("idUtenteVar"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(idUtenteVar))
            .SetAttribute(LCase("numeroProtocollo"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(numeroProtocollo))
            .SetAttribute(LCase("progPoligono"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(progPoligono))
            .SetAttribute(LCase("supVitataDich"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(supVitataDich))
            .SetAttribute(LCase("supVitataDichPRCalcolo"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(supVitataDichPRCalcolo))
            .SetAttribute(LCase("SuperficieServizioMq"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(SuperficieServizioMq))
            .SetAttribute(LCase("Terrazzamenti"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Terrazzamenti))
            .SetAttribute(LCase("TipoColtura"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(TipoColtura))
            .SetAttribute(LCase("tipoProcedimento"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(tipoProcedimento))
            .SetAttribute(LCase("tipoUnar"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(tipoUnar))
            .SetAttribute(LCase("tipoVariazione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(tipoVariazione))
            .SetAttribute(LCase("unar"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(unar))

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

            .SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
            .SetAttribute(LCase("PIVA"), CStr(PIVA))
            .SetAttribute(LCase("SA_COD"), CStr(SA_COD))
            .SetAttribute(LCase("APPEZZA"), CStr(APPEZZA))
            .SetAttribute(LCase("ID_REG"), CStr(ID_REG))
            .SetAttribute(LCase("Programmazione_Cod"), CStr(Programmazione_Cod))
            .SetAttribute(LCase("ID"), CStr(ID))
            .SetAttribute(LCase("codVitigno"), CStr(codVitigno))
            .SetAttribute(LCase("descrVitigno"), CStr(descrVitigno))
            .SetAttribute(LCase("dtIns"), CStr(dtIns))
            .SetAttribute(LCase("idUnitaVitata"), CStr(idUnitaVitata))
            .SetAttribute(LCase("perc"), CStr(perc))
            .SetAttribute(LCase("progr"), CStr(progr))
            .SetAttribute(LCase("Reale"), CStr(Reale))
            .SetAttribute(LCase("inviato"), CStr(inviato))
            .SetAttribute(LCase("datainvio"), CStr(datainvio))
            .SetAttribute(LCase("Data_Creazione"), CStr(Data_Creazione))
            .SetAttribute(LCase("Data_Modifica"), CStr(Data_Modifica))
            .SetAttribute(LCase("Username_Creazione"), CStr(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), CStr(Username_Modifica))
            .SetAttribute(LCase("Validita_Inizio"), CStr(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), CStr(Validita_Fine))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    Public Function XML_2_Idoneita_Appezzamento(
                               ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                               ByVal PIVA As String,
                                ByVal Progetto_Cod As Int32,
                                ByVal Programmazione_Cod As Int32,
                                ByVal IDIdoneita As Int32,
                                ByVal codTipologia As Int32,
                                ByVal dataRev As Date,
                                ByVal dataRic As Date,
                                ByVal docIgtDescr As String,
                                ByVal dtInizio As Date,
                                ByVal dtIns As Date,
                                ByVal dtVar As Date,
                                ByVal idDocIgt As String,
                                ByVal idUnitaVitata As String,
                                ByVal idUtenteIns As String,
                                ByVal idUtenteVar As String,
                                ByVal numIscrizione As Int32?,
                                ByVal tipologiaDescr As String,
                                ByVal inviato As Int32,
                                ByVal datainvio As DateTime,
                                ByVal Data_Creazione As DateTime,
                                ByVal Data_Modifica As DateTime,
                                ByVal Username_Creazione As String,
                                ByVal Username_Modifica As String,
                                ByVal Validita_Inizio As DateTime,
                                ByVal Validita_Fine As DateTime,
                               Optional ByRef XmlDoc As XmlDocument = Nothing
                                   ) As XmlElement
        Dim NodoXml As System.Xml.XmlElement

        If XmlDoc Is Nothing Then
            XmlDoc = New XmlDocument
        End If


        '----- Genero la stringa XML a partire dai valori dei parametri

        'Creo il nodo 
        NodoXml = XmlDoc.CreateElement("Idoneita_Appezzamento")


        With NodoXml

            .SetAttribute("TipoOperazioneDB", AgronicaCoreDataProvider.UtilityProvider.ValoreToString(TipoOperazioneDB))
            .SetAttribute(LCase("PIVA"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(PIVA))
            .SetAttribute(LCase("Progetto_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Progetto_Cod))
            .SetAttribute(LCase("Programmazione_Cod"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Programmazione_Cod))
            .SetAttribute(LCase("IDIdoneita"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(IDIdoneita))
            .SetAttribute(LCase("codTipologia"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(codTipologia))
            .SetAttribute(LCase("dataRev"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dataRev))
            .SetAttribute(LCase("dataRic"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dataRic))
            .SetAttribute(LCase("docIgtDescr"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(docIgtDescr))
            .SetAttribute(LCase("dtInizio"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtInizio))
            .SetAttribute(LCase("dtIns"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtIns))
            .SetAttribute(LCase("dtVar"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(dtVar))
            .SetAttribute(LCase("idDocIgt"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(idDocIgt))
            .SetAttribute(LCase("idUnitaVitata"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(idUnitaVitata))
            .SetAttribute(LCase("idUtenteIns"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(idUtenteIns))
            .SetAttribute(LCase("idUtenteVar"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(idUtenteVar))
            .SetAttribute(LCase("numIscrizione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(numIscrizione))
            .SetAttribute(LCase("tipologiaDescr"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(tipologiaDescr))
            .SetAttribute(LCase("inviato"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(inviato))
            .SetAttribute(LCase("datainvio"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(datainvio))
            .SetAttribute(LCase("Data_Creazione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Data_Creazione))
            .SetAttribute(LCase("Data_Modifica"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Data_Modifica))
            .SetAttribute(LCase("Username_Creazione"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Username_Creazione))
            .SetAttribute(LCase("Username_Modifica"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Username_Modifica))
            .SetAttribute(LCase("Validita_Inizio"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Validita_Inizio))
            .SetAttribute(LCase("Validita_Fine"), AgronicaCoreDataProvider.UtilityProvider.ValoreToString(Validita_Fine))

        End With

        'Restituisco in uscita 
        Return NodoXml

        'Distruggo gli oggetti
        NodoXml = Nothing
    End Function

    '#################################################################################################
    Public Function CreaProgettoPerImpianto(ByVal Operazione As Integer,
                                       ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Progetto_Cod As Integer,
                                       Optional ByVal Progetto_Nome As String = "lotto nuovo",
                                       Optional ByVal Progetto_Des As String = "",
                                       Optional ByVal Cau_Progetto As Integer = 9100,
                                       Optional ByVal Cod_Conto As Integer = 0,
                                       Optional ByVal Cod_Contratto As Integer = 0,
                                       Optional ByVal Data_Inizio_Prevista As Date = #1/1/1900#,
                                       Optional ByVal Data_Fine_Prevista As Date = #12/31/2100#,
                                       Optional ByVal Giudizio As String = "",
                                       Optional ByVal Appezza As Integer = 0,
                                       Optional ByVal Id_Reg As Integer = 0,
                                       Optional ByVal Ricavi_Previsti As Double = 0,
                                       Optional ByVal Produzione_Prevista As Double = 0,
                                       Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                       Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                       Optional ByVal Stato_Impianto As Integer = 0,
                                       Optional ByVal Regolamento_Cod As Integer = 1,
                                       Optional ByVal Disciplinare_Cod As Integer = 0,
                                       Optional ByVal P_Ha As Double = 0,
                                       Optional ByVal BaseCode As Integer = 0,
                                       Optional ByVal TopCode As Integer = 200000000) As String


        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_DatiProgetto As System.Xml.XmlElement
        Dim XML_Progetto As System.Xml.XmlElement

        Dim strProgettoFase As String

        XML_DatiProgetto = XmlDoc.CreateElement("DatiProgetto")
        XML_Progetto = XmlDoc.CreateElement("Progetto")

        XML_Progetto.SetAttribute("TipoOperazioneDB", CStr(Operazione))
        XML_Progetto.SetAttribute("piva", CStr(Piva))
        XML_Progetto.SetAttribute("sa_cod", CStr(Sa_Cod))
        XML_Progetto.SetAttribute("progetto_cod", CStr(Progetto_Cod))
        XML_Progetto.SetAttribute("progetto_nome", CStr(Progetto_Nome))
        XML_Progetto.SetAttribute("progetto_des", CStr(Progetto_Des))
        XML_Progetto.SetAttribute("cau_progetto", CInt(Cau_Progetto))
        XML_Progetto.SetAttribute("cod_conto", CInt(Cod_Conto))
        XML_Progetto.SetAttribute("cod_contratto", CInt(Cod_Contratto))
        XML_Progetto.SetAttribute("data_inizio_prevista", CDate(Data_Inizio_Prevista))
        XML_Progetto.SetAttribute("data_fine_prevista", CDate(Data_Fine_Prevista))
        XML_Progetto.SetAttribute("giudizio", CStr(Giudizio))
        XML_Progetto.SetAttribute("appezza", CInt(Appezza))
        XML_Progetto.SetAttribute("id_reg", CInt(Id_Reg))
        XML_Progetto.SetAttribute("ricavi_previsti", CDbl(Ricavi_Previsti))
        XML_Progetto.SetAttribute("produzione_prevista", CDbl(Produzione_Prevista))
        XML_Progetto.SetAttribute("validita_inizio", CDate(Validita_Inizio))
        XML_Progetto.SetAttribute("validita_fine", CDate(Validita_Fine))
        XML_Progetto.SetAttribute("stato_impianto", CInt(Stato_Impianto))
        XML_Progetto.SetAttribute("regolamento_cod", CInt(Regolamento_Cod))
        XML_Progetto.SetAttribute("disciplinare_cod", CInt(Disciplinare_Cod))
        XML_Progetto.SetAttribute("p_ha", CDbl(P_Ha))
        XML_Progetto.SetAttribute("basecode", CInt(BaseCode))
        XML_Progetto.SetAttribute("topcode", CInt(TopCode))

        XML_DatiProgetto.AppendChild(XML_Progetto)


        Dim FaseDes As String() = {"Aratura",
                                    "Concimazione Fogliare",
                                    "Concimazione in Pieno Campo",
                                    "Fertirrigazione",
                                    "Frangizollatura",
                                    "Irrigazione",
                                    "Semina",
                                    "Trapianto in Pieno Campo",
                                    "Trapianto in Serra",
                                    "Zappatura",
                                    "Altre Lavorazioni",
                                    "Diserbo",
                                    "Trattamento Antiparassitario",
                                    "Altri Trattamenti",
                                    "Installazione e Reinnesco Trappole, Rilievi in Campo",
                                    "Rilievi Produzione, Indici di Maturita, Danni"}

        Dim LavCod As Integer() = {8, 123, 14, 26, 31, 1, 2, 71, 151, 77, -4, 18, 74, -3, -1, -2}
        Dim Gru_Op As Integer() = {4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 3, 3, 3, 1, 2}

        If Progetto_Cod <> 0 Then
            Operazione = 1
        End If

        Dim i As Integer
        For i = 0 To UBound(LavCod)

            strProgettoFase &= GeneraBloccoProgettoFase(Operazione,
                                                        Piva,
                                                        Progetto_Cod,
                                                        0,
                                                        FaseDes(i),
                                                        Gru_Op(i),
                                                        LavCod(i), , , , , , , , )

        Next

        XML_Progetto.InnerXml = strProgettoFase

        '----- Assemblo la struttura

        XmlDoc.AppendChild(XML_DatiProgetto)

        Return XmlDoc.OuterXml


    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Public Function GeneraBloccoProgettoFase(ByVal Operazione As Integer,
                                              ByVal Piva As String,
                                              ByVal Progetto_Cod As Integer,
                                              ByVal Fase_Cod As Integer,
                                              ByVal Fase_Des As String,
                                              ByVal Gru_Op As Integer,
                                              ByVal Lav_Cod As Integer,
                                              Optional ByVal Data_Inizio_Prevista As Date = #1/1/1900#,
                                              Optional ByVal Data_Fine_Prevista As Date = #12/31/2100#,
                                              Optional ByVal Giudizio As String = "",
                                              Optional ByVal Budget As Double = 0,
                                              Optional ByVal Validita_Inizio As Date = #1/1/1900#,
                                              Optional ByVal Validita_Fine As Date = #12/31/2100#,
                                              Optional ByVal BaseCode As Integer = 0,
                                              Optional ByVal TopCode As Integer = 200000000) As String

        Dim XmlDoc As New System.Xml.XmlDocument

        Dim XML_ProgettoFase As System.Xml.XmlElement
        XML_ProgettoFase = XmlDoc.CreateElement("Progetto_Fase")

        XML_ProgettoFase.SetAttribute("TipoOperazioneDB", CStr(Operazione))
        XML_ProgettoFase.SetAttribute("piva", CStr(Piva))
        XML_ProgettoFase.SetAttribute("progetto_cod", CStr(Progetto_Cod))
        XML_ProgettoFase.SetAttribute("fase_cod", CInt(Fase_Cod))
        XML_ProgettoFase.SetAttribute("fase_des", CStr(Fase_Des))
        XML_ProgettoFase.SetAttribute("data_inizio_prevista", CDate(Data_Inizio_Prevista))
        XML_ProgettoFase.SetAttribute("data_fine_prevista", CDate(Data_Fine_Prevista))
        XML_ProgettoFase.SetAttribute("giudizio", CStr(Giudizio))
        XML_ProgettoFase.SetAttribute("budget", CDbl(Budget))
        XML_ProgettoFase.SetAttribute("lav_cod", CInt(Lav_Cod))
        XML_ProgettoFase.SetAttribute("gru_op", CInt(Gru_Op))
        XML_ProgettoFase.SetAttribute("validita_inizio", CDate(Validita_Inizio))
        XML_ProgettoFase.SetAttribute("validita_fine", CDate(Validita_Fine))
        XML_ProgettoFase.SetAttribute("basecode", CInt(BaseCode))
        XML_ProgettoFase.SetAttribute("topcode", CInt(TopCode))

        'Imposto XmlTxt come figlio del documento principale
        XmlDoc.AppendChild(XML_ProgettoFase)

        'Restituisco in uscita la stringa creata
        Return XmlDoc.InnerXml

        'Distruggo gli oggetti
        XML_ProgettoFase = Nothing
        XmlDoc = Nothing


    End Function

End Class
