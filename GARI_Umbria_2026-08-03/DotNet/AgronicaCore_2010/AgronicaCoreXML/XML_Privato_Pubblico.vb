Imports System.Xml
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Imports AgronicaCoreXML.XML_Manager

Public Class Agronica_XML

    'HT_Piva_objCodici
    'questo hash table contiene una coppia per ogni piva
    'key = piva
    'value = objdeletecodici

    Private BaseCode As Integer
    Private TopCode As Integer

    'MATRICE BIDIMENSIONALE PER IL SALVATAGGIO DELLE PARTICELLE
    Dim MatriceParticelle(,) As String

    Dim Cliente As String
    Dim pippo As String

    'CODICI ANAGRAFE
    Dim Id_Codice_ChiaveCliente As Integer
    'Dim Id_Codice_CUAA As Integer = 1010
    'Dim Id_Codice_Socio As Integer = 1033
    'Dim Id_Codice_TitoloPossesso As Integer = 1016
    ' Dim Id_Codice_TipoAttivita As Integer = 1000
    ' Dim Id_Codice_Operatore As Integer = 1003
    Dim Id_Codice_MetodoProduzione As Integer = 1018
    Dim Id_Codice_Contratto As Integer = 1073
    'Dim Id_Codice_TraFila As Integer = 1063
    ' Dim Id_Codice_SuFila As Integer = 1061
    ' Dim Id_Codice_Interbina As Integer = 1065
    'Dim Id_Codice_PianoSemina As Integer = 1071
    Dim Id_Codice_NumAppBio As Integer = 1085
    'Dim Id_Codice_Libro_Soci As Integer = 1086
    'Dim Id_Codice_Data_Iscrizione_Libro_Soci As Integer = 1087
    'Dim Id_Codice_Cooperativa As Integer = 1074
    ' Dim Id_Codice_Tecnico As Integer = 1088
    '  Dim Id_Codice_Cliente As Integer = 1089
    'Dim Id_Codice_Fornitore As Integer = 1090
    'Dim Id_Codice_Fornitore_2 As Integer = 1091
    'Dim Id_Codice_Fornitore_3 As Integer = 1092
    'Dim Id_Capitolato_Privato As Integer = 1093
    'Dim Id_Codice_Ausl As Integer = 4

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    'QUESTA FUNZIONE PUBBLICA RICEVE IN INGRESSO L'XML PUBBLICO, LO TRASFORMA IN XML PRIVATO CHE 
    'VIENE DATO IN PASTO a AgronicaCoreAnagrafeBIZ.Importa_GIAS.vb PER LA SCRITTURA DEI DATI
    '-----
    'HT_Piva_objCodici=hashtable che contiene per ogni piva l'oggetto di cancellazione dei codici
    '-----
    Public Function XMLPrivato_from_XMLPubblico(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                                ByRef HT_Piva_objCodici As Hashtable, _
                                                ByVal Str_XML_Pubblico As String, _
                                                ByVal Codice_Chiave_Cliente As Integer, _
                                                ByRef strErr As String _
                                                ) As String

        Dim NomeRoutine As String = "WS_Importa_GIAS.XML_Privato_Pubblico.XMLPrivato_from_XMLPubblico()"

        Dim xmlAnagHlp As New AgronicaCoreXML.XML_Anagrafe
        Dim xmlContabHlp As New AgronicaCoreXML.XML_Contab

        'ByVal Piva_SuperUser As String, _
        'Optional ByVal Legale_Rappresentante As Integer = 1, _
        'Optional ByVal Connessione As String = "", _


        '--------------
        'AgronicaCore
        'Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        'Dim objCACCultivar As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_R
        'Dim objCACVegcod As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod_R
        Dim objCACAnimali As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Animali_R
        Dim objCentriInd As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
        Dim objImpreseInd As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R


        '------------------------------------------
        '----- Dichiarazione delle Variabili
        '------------------------------------------
        Dim XmlDocPrivato As New System.Xml.XmlDocument
        Dim XmlDocPubblico As New System.Xml.XmlDocument

        Dim XmlDocImpresa As System.Xml.XmlDocument
        Dim XmlDocPrivato_2 As System.Xml.XmlDocument
        Dim XmlDocPrivato_3 As System.Xml.XmlDocument
        Dim XmlDocPrivato_4 As System.Xml.XmlDocument
        Dim XmlDocPrivato_5 As System.Xml.XmlDocument
        Dim XmlDocPrivato_6 As System.Xml.XmlDocument
        Dim XmlDocPrivato_7 As System.Xml.XmlDocument
        Dim XmlDocPrivato_8 As System.Xml.XmlDocument
        Dim XmlDocPrivato_8_b As System.Xml.XmlDocument
        Dim XmlDocPrivato_9 As System.Xml.XmlDocument


        'NODI XML
        Dim XML_UtentePrivato As System.Xml.XmlElement
        Dim XML_UtentePubblico As System.Xml.XmlElement

        Dim XML_DatiImprese As System.Xml.XmlElement
        Dim XML_ImpresaPrivato As System.Xml.XmlElement
        Dim XMLs_ImpresaPubblico As System.Xml.XmlNodeList
        Dim XML_ImpresaPubblico As System.Xml.XmlElement
        Dim XML_GerarchiaImprese As XmlElement

        Dim XML_DatiContatti As System.Xml.XmlElement
        Dim XML_DatoContatto As System.Xml.XmlElement

        Dim XMLs_FascicoloPubblico As System.Xml.XmlNodeList
        Dim XML_FascicoloPubblico As System.Xml.XmlElement

        Dim XML_DatiCentriAziendali As System.Xml.XmlElement
        Dim XMLs_CentroAziendalePubblico As System.Xml.XmlNodeList
        Dim XML_CentroAziendalePubblico As System.Xml.XmlElement
        Dim XML_CentroAziendalePubblicoClonato As System.Xml.XmlElement
        Dim XML_CentroAziendalePrivato As System.Xml.XmlElement

        Dim XMLs_ContattoPubblico As System.Xml.XmlNodeList
        Dim XML_ContattoPubblico As System.Xml.XmlElement

        Dim XMLs_RuoloPubblico As System.Xml.XmlNodeList
        Dim XML_RuoloPubblico As System.Xml.XmlElement

        Dim XML_DatiParticelle As System.Xml.XmlElement
        Dim XMLs_ParticellaPubblico As System.Xml.XmlNodeList
        Dim XML_ParticellaPubblico As System.Xml.XmlElement
        Dim XML_ParticellaPrivato As System.Xml.XmlElement

        Dim XMLs_ParticellaPossessiPubblico As System.Xml.XmlNodeList
        Dim XML_ParticellaPossessoPubblico As System.Xml.XmlElement
        Dim XMLs_ParticellaPossessoPrivato As System.Xml.XmlElement

        Dim XMLs_ZonaxParticellaPubblico As System.Xml.XmlNodeList
        Dim XML_ZonaxParticellaPubblico As System.Xml.XmlElement
        Dim XML_ZonaxParticellaPrivato As System.Xml.XmlElement


        Dim XMLs_ClassamentoxParticellaPubblico As System.Xml.XmlNodeList
        Dim XML_ClassamentoxParticellaPubblico As System.Xml.XmlElement
        Dim XML_ClassamentoxParticellaPrivato As System.Xml.XmlElement

        Dim XMLs_EleggibilitaxParticellaPubblico As System.Xml.XmlNodeList
        Dim XML_EleggibilitaxParticellaPubblico As System.Xml.XmlElement
        Dim XML_EleggibilitaxParticellaPrivato As System.Xml.XmlElement

        Dim XMLs_ParticellaxMacrousiPubblico As System.Xml.XmlNodeList
        Dim XML_ParticellaxMacrousiPubblico As System.Xml.XmlElement
        Dim XML_ParticellaxMacrousiPrivato As System.Xml.XmlElement

        Dim XMLs_ParticellaxMacrousixUtilizziPubblico As System.Xml.XmlNodeList
        Dim XML_ParticellaxMacrousixUtilizziPubblico As System.Xml.XmlElement
        Dim XML_ParticellaxMacrousixUtilizziPrivato As System.Xml.XmlElement

        Dim XML_DatiFabbricati As System.Xml.XmlElement
        Dim XMLs_FabbricatoPubblico As System.Xml.XmlNodeList
        Dim XML_FabbricatoPubblico As System.Xml.XmlElement
        Dim XML_FabbricatoPrivato As System.Xml.XmlElement

        Dim XMLs_StallaPubblico As System.Xml.XmlNodeList
        Dim XML_StallaPubblico As System.Xml.XmlElement
        Dim XML_StallaPrivato As System.Xml.XmlElement

        Dim XML_DatiCampi As System.Xml.XmlElement
        Dim XMLs_CampoPubblico As System.Xml.XmlNodeList
        Dim XML_CampoPubblico As System.Xml.XmlElement
        Dim XML_CampoPrivato As System.Xml.XmlElement

        Dim XML_DatiCampixParticelle As System.Xml.XmlElement
        Dim XMLs_CampoxParticellaPubblico As System.Xml.XmlNodeList
        Dim XML_CampoxParticellaPubblico As System.Xml.XmlElement
        Dim XML_CampoxParticellaPrivato As System.Xml.XmlElement

        Dim XML_DatiAppezzamenti As System.Xml.XmlElement
        Dim XMLs_AppezzamentoPubblico As System.Xml.XmlNodeList
        Dim XML_AppezzamentoPubblico As System.Xml.XmlElement
        Dim XML_AppezzamentoPrivato As System.Xml.XmlElement

        Dim XML_DatiAppezzamentixParticelle As System.Xml.XmlElement
        Dim XMLs_AppezzamentoxParticellaPubblico As System.Xml.XmlNodeList
        Dim XML_AppezzamentoxParticellaPubblico As System.Xml.XmlElement
        Dim XML_AppezzamentoxParticellaPrivato As System.Xml.XmlElement

        Dim XML_DatiRegImpianti As System.Xml.XmlElement

        Dim XML_DatiProgetto As System.Xml.XmlElement

        Dim XML_DatiAgenda As System.Xml.XmlElement
        Dim XMLs_AgendaPubblico As System.Xml.XmlNodeList
        Dim XML_AgendaPubblico As System.Xml.XmlElement
        Dim XML_AgendaPrivato As System.Xml.XmlElement
        Dim XML_DatiMovimenti As System.Xml.XmlElement
        Dim XML_Movimento As System.Xml.XmlElement
        Dim XML_DatiMovimentiDettagli As System.Xml.XmlElement
        Dim XML_MovimentoDettaglio As System.Xml.XmlElement
        Dim XML_MovimentoDestinazione As System.Xml.XmlElement

        Dim XMLs_CapiAnimaliPubblico As System.Xml.XmlNodeList
        Dim XML_CapoAnimalePubblico As System.Xml.XmlElement
        Dim XML_DatiZoo_Animale_Anagrafe As System.Xml.XmlElement
        Dim XML_Zoo_Animale_Anagrafe As System.Xml.XmlElement

        Dim XML_Nodo As System.Xml.XmlElement
        Dim XML_Temp As System.Xml.XmlElement

        'STRINGHE XML
        Dim Str_Indirizzo As String
        Dim Str_Codice As String
        Dim Str_Contatto As String
        Dim Str_RisorsaUmana As String
        Dim Str_Rubrica As String
        Dim Str_CentroAziendale As String
        Dim Str_Particella As String
        Dim Str_Fabbricato As String
        Dim Str_Campo As String
        Dim Str_CampixParticelle As String
        Dim Str_CodiceCampo As String
        Dim Str_Appezzamento As String
        Dim Str_AppezzamentoxParticelle As String
        Dim Str_CodiceAppezzamento As String
        'Dim Str_CodiceImpianto As String
        Dim Str_Impianto As String
        'Dim str_Progetto As String
        Dim Str_Zona As String
        Dim Str_Classamento As String
        Dim Str_Eleggibilita As String
        Dim Str_Possesso As String
        Dim Str_Macrousi As String
        Dim Str_Macrouso As String
        Dim strUtilizzi As String
        Dim strGerarchiaImprese As String = ""

        Dim StrTemp As String


        'Dim AppezzaIdReg As String
        'Dim ArrayAppezzaIdReg() As String
        'Dim Appezza As String
        'Dim Id_Reg As String

        Dim Cod_Indirizzo As Integer
        Dim Cod_Rubrica As Integer

        'Dim Grfi_Cod As Integer
        'Dim Cul_Cod As Integer
        'Dim Veg_Cod As Integer
        Dim Cod_Specie As String = ""
        Dim Cod_Varieta As String = ""


        Dim Cod_Finalita As String = ""

        Dim Gen_Cod As Integer
        Dim Spe_Cod As Integer
        Dim Ipro_Cod As Integer
        Dim Cat_Cod As Integer
        Dim Raz_Cod As Integer
        Dim Cod_Animale_Cliente As String

        'CONTATORI
        Dim i_1 As Integer
        Dim i_2 As Integer
        Dim i_3 As Integer
        Dim i_4 As Integer
        Dim i_5 As Integer
        Dim i_6 As Integer
        Dim i_7 As Integer
        Dim i_7C As Integer
        Dim i_8 As Integer
        'Dim i_9 As Integer
        Dim i_t As Integer
        Dim i_7A As Integer
        Dim i_8A As Integer
        Dim i_9A As Integer

        Dim i_10 As Integer
        Dim i_10A As Integer

        Dim j As Integer
        Dim i_Fascicolo As Integer

        Dim nCampi As Integer
        Dim nAppezzamentiLiberi As Integer
        Dim nAppezzamenti As Integer


        'VARIABILI PER LA PARTICELLA CATASTALE
        Dim Indice As Integer
        Dim Prov As String
        Dim Com As String
        Dim Sezione As String
        Dim Foglio As String
        Dim Numero As String
        Dim Subalterno As String
        Dim Sup_Particella As String
        Dim Ettari_Particella As Integer
        Dim Are_Particella As Integer
        Dim Centiare_Particella As Integer
        Dim Sup_AppxPart As String
        Dim Ettari_AppxPart As Double
        Dim Are_AppxPart As Double
        Dim Centiare_AppxPart As Double

        Dim MessaggioErrore As String = ""

        Id_Codice_ChiaveCliente = Codice_Chiave_Cliente

        'oggetto che contiene i flag sui codici anagrafici da cancellare
        Dim obj_DeleteCodici As ObjDeleteCodici
        Dim objXML As New AgronicaCoreXML.XML_Anagrafe

        Dim Piva As String
        Dim Piva_Padre As String
        Dim codice_socio As String

        Try


            '------------------------------------------
            '----- Analizzo la stringa XML Privata
            '------------------------------------------
            'Carico la stringa nel documento XML pubblico
            XmlDocPubblico.LoadXml(Str_XML_Pubblico)

            '------------------------------------------
            '----- Tag Utente
            '------------------------------------------
            'leggo
            XML_UtentePubblico = XmlDocPubblico.SelectSingleNode("Utente")

            If Not XML_UtentePubblico Is Nothing Then

                '------------------------------------------
                '----- Calcolo BaseCode e TopCode
                '------------------------------------------
                AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, CInt(XML_UtentePubblico.GetAttribute("codice")))

                XML_DatiImprese = XmlDocPrivato.CreateElement("DatiImprese")

                XmlDocPrivato.AppendChild(XML_DatiImprese)

                'Leggo l'insieme dei nodi impresa
                XMLs_ImpresaPubblico = XML_UtentePubblico.GetElementsByTagName("Impresa")

                For i_1 = 0 To XMLs_ImpresaPubblico.Count - 1

                    'nuova impresa
                    'creo il nuovo oggetto dei codici
                    obj_DeleteCodici = New ObjDeleteCodici

                    'ciclo sui nodi impresa
                    XML_ImpresaPubblico = XMLs_ImpresaPubblico(i_1)

                    XmlDocImpresa = New System.Xml.XmlDocument

                    '££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                    '££££££££££££££££££                   IMPRESA                ££££££££££££££££££££££££
                    '££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

                    XML_ImpresaPrivato = XmlDocImpresa.CreateElement("Impresa")

                    '------------------------------------------
                    'verifica correttezza dei dati
                    '------------------------------------------
                    'Tipo Operazione
                    Select Case CStr(XML_ImpresaPubblico.GetAttribute("tipo_operazione"))
                        Case "1", "2", "0"
                        Case Else
                            MessaggioErrore += "- E' necessario indicate il codice dell'Operazione richiesta (0=lettura; 1=Inserimento; 2=modifica)!" & vbCrLf
                    End Select

                    'Ragione Sociale
                    If (CStr(XML_ImpresaPubblico.GetAttribute("ragione_sociale")) = "#" Or CStr(XML_ImpresaPubblico.GetAttribute("ragione_sociale")) = "") And XML_ImpresaPubblico.GetAttribute("tipo_operazione") <> "0" Then
                        MessaggioErrore += "- E' necessario inserire la Ragione Sociale dell'Impresa!" & vbCrLf
                    End If

                    'Piva
                    If CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")) = "#" Or CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")) = "" Then
                        ''se non ho la partita iva metto il CUAA tagliato a 11 cifre
                        Dim newPiva As String
                        newPiva = CStr(XML_ImpresaPubblico.GetAttribute("codice_cuaa")).Substring(0, 11)
                        XML_ImpresaPubblico.SetAttribute("partita_iva", newPiva)
                        'MessaggioErrore += "- E' necessario inserire la Partita Iva dell'Impresa!" & vbCrLf
                    End If

                    'Modifica del 10/05/2012: introdotta per zani (imprese straniere)
                    'If CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")).Length <> 11 Then
                    '    MessaggioErrore += "- La Partita Iva dell'Impresa non è corretta!" & vbCrLf
                    'End If
                    If CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")).Length > 25 Then
                        MessaggioErrore += "- La Partita Iva " & CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")) & " dell'Impresa non può superare i 25 caratteri!" & vbCrLf
                    End If

                    'Piva Padre
                    If (CStr(XML_ImpresaPubblico.GetAttribute("partita_iva_padre")) = "#" Or CStr(XML_ImpresaPubblico.GetAttribute("partita_iva_padre")) = "") And XML_ImpresaPubblico.GetAttribute("tipo_operazione") <> "0" Then

                        If CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")) = objParametri_Server.PivaSuperUser Then

                            'il superuser nonha padre, ha stringa vuota in gerarchia imprese

                            'ok

                            'If CStr(XML_ImpresaPubblico.GetAttribute("tipo_operazione")) <> "0" Then
                            '    MessaggioErrore += "- Si sta importando il superuser: " & CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")) & " il tipo operazione deve essere lettura!" & vbCrLf
                            'End If


                        Else

                            MessaggioErrore += "- E' necessario inserire la Partita Iva del Padre dell'Impresa!" & vbCrLf

                        End If


                    End If
                    If CStr(XML_ImpresaPubblico.GetAttribute("partita_iva_padre")).Length <> 11 And XML_ImpresaPubblico.GetAttribute("tipo_operazione") <> "0" Then
                        If CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")) = objParametri_Server.PivaSuperUser AndAlso
                                        (CStr(XML_ImpresaPubblico.GetAttribute("partita_iva_padre")) = "#" Or
                                        CStr(XML_ImpresaPubblico.GetAttribute("partita_iva_padre")) = "") Then
                            'ok

                        Else
                            MessaggioErrore += "- La Partita Iva del Padre dell'Impresa non è corretta!" & vbCrLf
                        End If

                    End If


                    If CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")) = objParametri_Server.PivaSuperUser Then
                        If CStr(XML_ImpresaPubblico.GetAttribute("partita_iva_padre")) <> "#" And CStr(XML_ImpresaPubblico.GetAttribute("partita_iva_padre")) <> "" Then
                            MessaggioErrore += "- con il superuser la Partita Iva del Padre dell'Impresa deve essere vuota!" & vbCrLf
                        End If
                    End If

                    'Indirizzo
                    'If CStr(XML_ImpresaPubblico.GetAttribute("i_cap")) = "#" Or CStr(XML_ImpresaPubblico.GetAttribute("i_cap")) = "" Then
                    '    MessaggioErrore += "- E' necessario inserire il CAP nell'Indirizzo dell'Impresa!" & vbCrLf
                    'End If
                    'i campi comune e provincia della tabella Indirizzi non sono più necessari
                    'si leggono dalla tabella istat
                    'If CStr(XML_ImpresaPubblico.GetAttribute("i_comune")) = "#" Or CStr(XML_ImpresaPubblico.GetAttribute("i_comune")) = "" Then
                    '    MessaggioErrore += "- E' necessario inserire il Comune nell'Indirizzo dell'Impresa!" & vbCrLf
                    'End If
                    'If CStr(XML_ImpresaPubblico.GetAttribute("i_provincia")) = "#" Or CStr(XML_ImpresaPubblico.GetAttribute("i_provincia")) = "" Then
                    '    MessaggioErrore += "- E' necessario inserire la Provincia nell'Indirizzo dell'Impresa!" & vbCrLf
                    'End If

                    If (CStr(XML_ImpresaPubblico.GetAttribute("i_codice_istat_comune")) = "#" Or CStr(XML_ImpresaPubblico.GetAttribute("i_codice_istat_comune")) = "") And XML_ImpresaPubblico.GetAttribute("tipo_operazione") <> "0" Then
                        MessaggioErrore += "- E' necessario inserire il codice Istat del Comune nell'Indirizzo dell'Impresa!" & vbCrLf
                    End If
                    If (CStr(XML_ImpresaPubblico.GetAttribute("i_codice_istat_provincia")) = "#" Or CStr(XML_ImpresaPubblico.GetAttribute("i_codice_istat_provincia")) = "") And XML_ImpresaPubblico.GetAttribute("tipo_operazione") <> "0" Then
                        MessaggioErrore += "- E' necessario inserire il codice Istat della Provincia nell'Indirizzo dell'Impresa!" & vbCrLf
                    End If

                    If MessaggioErrore <> "" Then
                        Throw New Exception(MessaggioErrore)
                    End If

                    'era già gestito dopo
                    'If XML_ImpresaPubblico.HasAttribute("i_cod_indirizzo") = True Then
                    '    Cod_Indirizzo_Impresa = XML_ImpresaPubblico.GetAttribute("i_cod_indirizzo")
                    'Else
                    '    Cod_Indirizzo_Impresa = 0
                    'End If

                    Piva = CStr(XML_ImpresaPubblico.GetAttribute("partita_iva"))
                    Piva_Padre = CStr(XML_ImpresaPubblico.GetAttribute("partita_iva_padre"))
                    codice_socio = Agro_If(XML_ImpresaPubblico.GetAttribute("codice_socio"), CStr(0))

                    '------------------------------------------
                    '----- Inserimento record nella hashtable
                    '------------------------------------------
                    If Not HT_Piva_objCodici.Contains(Piva) Then
                        HT_Piva_objCodici.Add(Piva, obj_DeleteCodici)
                    End If

                    '------------------------------------------
                    '----- Tag Impresa
                    '------------------------------------------
                    'scrivo
                    XML_ImpresaPrivato = xmlAnagHlp.XML_Impresa(CStr(XML_ImpresaPubblico.GetAttribute("tipo_operazione")),
                                                    CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                    Left(CStr(XML_ImpresaPubblico.GetAttribute("ragione_sociale")), 120),
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("sup_totale"), CDbl(0)),
                                                     Piva_Padre,
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("tipo_impresa_gerarchia"), CInt(1)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)

                    '------------------------------------------
                    '----- Tag GerarchiaImprese
                    '------------------------------------------
                    'non esisteva, inserito in data 28/03/2013:
                    'caso successo da Coldiretti: un'azienda di Baldon era sotto Bologna
                    'e non riusciva ad essere aggiornata sotto a Ferrara
                    Select Case CInt(XML_ImpresaPubblico.GetAttribute("tipo_operazione"))
                        'se sono in modifica impresa
                        Case enum_TipoOperazioneDB.Modifica

                            Dim objGera As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
                            Dim HT_Padri As Hashtable
                            HT_Padri = objGera.Ricava_HT_PivePadre(Piva, _
                                                                    "", _
                                                                    objParametri_Server)
                            objGera = Nothing

                            If Not IsNothing(HT_Padri) AndAlso HT_Padri.Count > 0 Then
                                If HT_Padri.ContainsKey(Piva_Padre) = True Then
                                    'ok, il padre c'è già in gerarchia
                                Else
                                    'il padre è nuovo, lo inserisco
                                    'se ci sono altri padri non li cancello (non è detto che debbano sparire)
                                    XML_GerarchiaImprese = objXML.XML_GerarchiaImprese(enum_TipoOperazioneDB.Scrittura, _
                                                                                        Piva_Padre, _
                                                                                        Nothing, _
                                                                                        AGRODATAINIZIO, _
                                                                                        AGRODATAFINE)

                                    'XML_Nodo = XML_ImpresaPrivato.OwnerDocument.ImportNode(XML_GerarchiaImprese, True)
                                    'XML_ImpresaPrivato.AppendChild(XML_Nodo)
                                    strGerarchiaImprese = XML_GerarchiaImprese.OuterXml
                                End If
                            Else
                                'errore, non dovrebbe succedere
                                'inserisco il padre
                                XML_GerarchiaImprese = objXML.XML_GerarchiaImprese(enum_TipoOperazioneDB.Scrittura, _
                                                                                    Piva_Padre, _
                                                                                    Nothing, _
                                                                                    AGRODATAINIZIO, _
                                                                                    AGRODATAFINE)
                                'XML_Nodo = XML_ImpresaPrivato.OwnerDocument.ImportNode(XML_GerarchiaImprese, True)
                                'XML_ImpresaPrivato.AppendChild(XML_Nodo)
                                strGerarchiaImprese = XML_GerarchiaImprese.OuterXml
                            End If
                            HT_Padri = Nothing

                    End Select


                    '------------------------------------------
                    '----- Tag Indirizzo Impresa
                    '------------------------------------------

                    Cod_Indirizzo = 0

                    Select Case CInt(XML_ImpresaPubblico.GetAttribute("tipo_operazione"))
                        Case 2
                            'Cod_Indirizzo = Cod_Indirizzo_Impresa(CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")), Connessione)
                            Cod_Indirizzo = objImpreseInd.CodIndirizzo_from_Piva(CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")), objParametri_Server)
                        Case Else
                            Cod_Indirizzo = 0
                    End Select

                    Str_Indirizzo = xmlAnagHlp.XML_Indirizzo(CStr(XML_ImpresaPubblico.GetAttribute("tipo_operazione")),
                                                    CInt(1),
                                                    CInt(Cod_Indirizzo),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("i_indirizzo"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("i_frazione"), ""),
                                                    CStr(XML_ImpresaPubblico.GetAttribute("i_cap")),
                                                    CStr(XML_ImpresaPubblico.GetAttribute("i_comune")),
                                                    CStr(XML_ImpresaPubblico.GetAttribute("i_provincia")),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("i_stato"), "IT"),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("i_note_indirizzo"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("i_codice_istat_provincia"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("i_codice_istat_comune"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)

                    '------------------------------------------
                    '----- Tag Codici Impresa
                    '------------------------------------------

                    'inizializzo la stringa dei codici
                    Str_Codice = ""

                    Dim TipoOp_Codice As enum_TipoOperazioneDB

                    TipoOp_Codice = CStr(XML_ImpresaPubblico.GetAttribute("tipo_operazione"))

                    If TipoOp_Codice = enum_TipoOperazioneDB.Modifica Then
                        TipoOp_Codice = enum_TipoOperazioneDB.Scrittura
                    End If

                    '------------------------------------------
                    '----- Tag Codice CUAA Impresa
                    '------------------------------------------

                    Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                            enum_CodiciAnagrafe.CodiceCUAA,
                                            Agro_If(XML_ImpresaPubblico.GetAttribute("codice_cuaa"), CStr(XML_ImpresaPubblico.GetAttribute("partita_iva"))),
                                            Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                            Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                            BaseCode,
                                            TopCode)


                    '------------------------------------------
                    '----- Tag Codice Socio Impresa
                    '------------------------------------------

                    Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                            enum_CodiciAnagrafe.Codice_Socio,
                                            codice_socio,
                                            Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                            Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                            BaseCode,
                                            TopCode)


                    '------------------------------------------
                    '----- Tag Titolo Possesso Impresa
                    '------------------------------------------

                    Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                            enum_CodiciAnagrafe.TitoloPossesso,
                                            Agro_If(XML_ImpresaPubblico.GetAttribute("titolo_possesso"), CStr(1)),
                                            Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                            Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                            BaseCode,
                                            TopCode)


                    '------------------------------------------
                    '----- Tag Tecnico Referente 
                    '------------------------------------------

                    If XML_ImpresaPubblico.HasAttribute("cf_tecnico_referente") = True Then
                        If CStr(XML_ImpresaPubblico.GetAttribute("cf_tecnico_referente")) <> "#" Then
                            Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                    enum_CodiciAnagrafe.Tecnico,
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("cf_tecnico_referente"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)
                        End If
                    End If

                    '------------------------------------------
                    '----- Tag Codice Libro Soci 
                    '------------------------------------------

                    If XML_ImpresaPubblico.HasAttribute("codice_libro_soci") = True Then
                        If CStr(XML_ImpresaPubblico.GetAttribute("codice_libro_soci")) <> "#" Then
                            Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                    enum_CodiciAnagrafe.Codice_Libro_Soci,
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("codice_libro_soci"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)
                        End If
                    End If

                    '------------------------------------------
                    '----- Tag Data Iscrizione Libro Soci 
                    '------------------------------------------

                    If XML_ImpresaPubblico.HasAttribute("data_iscrizione_libro_soci") = True Then
                        If CStr(XML_ImpresaPubblico.GetAttribute("data_iscrizione_libro_soci")) <> "#" Then
                            Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                    enum_CodiciAnagrafe.Data_Iscrizione_Libro_Soci,
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("data_iscrizione_libro_soci"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)
                        End If
                    End If


                    '------------------------------------------
                    '----- Tag Codice Cliente 
                    '------------------------------------------

                    If XML_ImpresaPubblico.HasAttribute("codice_cliente") = True Then
                        If CStr(XML_ImpresaPubblico.GetAttribute("codice_cliente")) <> "#" Then
                            Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                    enum_CodiciAnagrafe.Codice_Cliente,
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("codice_cliente"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)
                        End If
                    End If

                    '------------------------------------------
                    '----- Tag Codice Fornitore 
                    '------------------------------------------

                    If XML_ImpresaPubblico.HasAttribute("codice_fornitore") = True Then
                        If CStr(XML_ImpresaPubblico.GetAttribute("codice_fornitore")) <> "#" Then
                            Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                    enum_CodiciAnagrafe.Codice_Fornitore,
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("codice_fornitore"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)
                        End If
                    End If

                    '------------------------------------------
                    '----- Tag Codice Fornitore 2
                    '------------------------------------------

                    If XML_ImpresaPubblico.HasAttribute("codice_fornitore_2") = True Then
                        If CStr(XML_ImpresaPubblico.GetAttribute("codice_fornitore_2")) <> "#" Then
                            Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                    enum_CodiciAnagrafe.Codice_Fornitore_2,
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("codice_fornitore_2"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)
                        End If
                    End If

                    '------------------------------------------
                    '----- Tag Codice Fornitore 3
                    '------------------------------------------

                    If XML_ImpresaPubblico.HasAttribute("codice_fornitore_3") = True Then
                        If CStr(XML_ImpresaPubblico.GetAttribute("codice_fornitore_3")) <> "#" Then
                            Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                    enum_CodiciAnagrafe.Codice_Fornitore_3,
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("codice_fornitore_3"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)
                        End If
                    End If

                    '------------------------------------------
                    '----- Tag CodiceConferente
                    '------------------------------------------

                    If XML_ImpresaPubblico.HasAttribute("CodiceConferente") = True Then
                        If CStr(XML_ImpresaPubblico.GetAttribute("CodiceConferente")) <> "#" Then
                            Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                    enum_CodiciAnagrafe.CodiceConferente,
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("CodiceConferente"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)
                        End If
                    End If
                    '------------------------------------------
                    '----- Tag Codice_Zona
                    '------------------------------------------

                    If XML_ImpresaPubblico.HasAttribute("Codice_Zona") = True Then
                        If CStr(XML_ImpresaPubblico.GetAttribute("Codice_Zona")) <> "#" Then
                            Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                    enum_CodiciAnagrafe.Codice_Zona,
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("Codice_Zona"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)
                        End If
                    End If

                    '------------------------------------------
                    '----- Tag Codice AUSL
                    '------------------------------------------

                    If XML_ImpresaPubblico.HasAttribute("codice_ausl") = True Then
                        If CStr(XML_ImpresaPubblico.GetAttribute("codice_ausl")) <> "#" Then
                            Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                    enum_CodiciAnagrafe.Codice_Ausl,
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("codice_ausl"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)
                        End If
                    End If

                    '------------------------------------------
                    '----- Tag Codice PAT
                    '------------------------------------------

                    If XML_ImpresaPubblico.HasAttribute("codice_pat") = True Then
                        If CStr(XML_ImpresaPubblico.GetAttribute("codice_pat")) <> "#" Then
                            Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                    enum_CodiciAnagrafe.ImpresaPAT,
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("codice_pat"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)
                        End If
                    End If
                    '------------------------------------------
                    '----- Tag Chiave Cliente
                    '------------------------------------------

                    If CStr(XML_ImpresaPubblico.GetAttribute("i_chiave_cliente")) <> "#" Then
                        Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                Id_Codice_ChiaveCliente,
                                                Agro_If(XML_ImpresaPubblico.GetAttribute("i_chiave_cliente"), ""),
                                                Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                BaseCode,
                                                TopCode)
                    End If

                    'se ci sono dei codici legati all'impresa
                    'e se sono in modifica
                    If Str_Codice <> "" And CInt(XML_ImpresaPubblico.GetAttribute("tipo_operazione")) = enum_TipoOperazioneDB.Modifica Then

                        Codici_toDelete_Impresa(HT_Piva_objCodici,
                                                  CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")))

                    End If

                    ''se sono in modifica imposto i codici gestiti sopra con tipo operazione scrittura (prima vengono cancellati)
                    ''gli altri codici andranno gestiti
                    'If CInt(XML_ImpresaPubblico.GetAttribute("tipo_operazione")) = enum_TipoOperazioneDB.Modifica Then
                    '    Dim xmlAnagHlp.XML_Codice_list As XmlNodeList
                    '    Dim xmlAnagHlp.XML_Codice As XmlElement
                    '    Dim xml_datiCodici As XmlElement
                    '    Dim i As Integer
                    '    xml_datiCodici = XML_RegImpiantoPrivato.SelectSingleNode("DatiCodici")
                    '    xmlAnagHlp.XML_Codice_list = xml_datiCodici.SelectNodes("CodiceImpianto")
                    '    For i = 0 To xmlAnagHlp.XML_Codice_list.Count - 1
                    '        xmlAnagHlp.XML_Codice = xmlAnagHlp.XML_Codice_list.Item(i)
                    '        Select Case CInt(xmlAnagHlp.XML_Codice.GetAttribute("id_cod"))
                    '            'al momento gestisco solo il sesto d'impianto!!!!!!!
                    '        Case enum_CodiciAnagrafe.Impianto_SuFila_Maschio, enum_CodiciAnagrafe.Impianto_TraFila_Maschio
                    '                xmlAnagHlp.XML_Codice.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Scrittura))
                    '            Case Else
                    '                'lascio il mondo come sta
                    '        End Select
                    '    Next
                    'End If

                    XML_ImpresaPrivato.InnerXml = Str_Codice + Str_Indirizzo & strGerarchiaImprese


                    '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                    '££££££££££££££££££££££                 CONTATTI                     £££££££££££££££££££££££££££
                    '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££


                    '------------------------------------------
                    '----- Verificare se l'azienda è già contatto

                    Dim objContattoImpresa As New AgronicaCoreAnagrafeDAL.Contatti_R
                    Dim flag_EsisteContattoImpresa As Boolean

                    flag_EsisteContattoImpresa = objContattoImpresa.Esiste_Contatto(objParametri_Server.PivaSuperUser,
                                                                    CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                    "", "",
                                                                    objParametri_Server)



                    '------------------------------------------
                    '----- Tag Dati Contatti
                    '------------------------------------------

                    'creo una nuova istanza del documento di appoggio
                    XmlDocPrivato_2 = New System.Xml.XmlDocument

                    'CREO IL TAG DEI DATI_CONTATTI IN UN SECONDO DOCUMENTO CHE DOPO IMPORTERO' IN QUELLO PRINCIPALE
                    XML_DatiContatti = XmlDocPrivato_2.CreateElement("DatiContatti")


                    'sa_cod = 0 --> Risorsa visibile solo dall'impresa 
                    'sa_cod = -1 --> Risorsa visibile da tutti

                    Dim TipoOperazione_ContattoImpresa As enum_TipoOperazioneDB
                    Dim TipoOperazione_RisorsaUmanaImpresa As enum_TipoOperazioneDB

                    If CInt(XML_ImpresaPubblico.GetAttribute("tipo_operazione")) = enum_TipoOperazioneDB.Scrittura Then

                        If flag_EsisteContattoImpresa = False Then
                            'il contatto non esiste

                            '-> tutto in scrittura
                            TipoOperazione_ContattoImpresa = enum_TipoOperazioneDB.Scrittura
                            TipoOperazione_RisorsaUmanaImpresa = enum_TipoOperazioneDB.Scrittura


                        Else
                            '---------------------------------------------------
                            '----- Esiste già il contatto, non devo crearlo ----
                            '(probabilmente già creato sui piani di campionamento)
                            '---------------------------------------------------
                            'vado in modifica del contatto (così ad esempio aggiorna la ragione sociale)
                            'la risorsa umana non la tocco
                            TipoOperazione_ContattoImpresa = enum_TipoOperazioneDB.Modifica
                            TipoOperazione_RisorsaUmanaImpresa = enum_TipoOperazioneDB.Lettura

                        End If 'esistenza contatto


                        'sa_cod = 0 --> contatto visibile solo dall'impresa 
                        'sa_cod = -1 --> contatto visibile da tutti
                        Str_Contatto = xmlAnagHlp.XML_Contatto(TipoOperazione_ContattoImpresa,
                                                                CStr(objParametri_Server.PivaSuperUser),
                                                                CInt(0),
                                                                CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                Left(CStr(XML_ImpresaPubblico.GetAttribute("ragione_sociale")), 100),
                                                                "",
                                                                "",
                                                                "",
                                                                Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                                CInt(1),
                                                                Agro_If(XML_ImpresaPubblico.GetAttribute("codice_fiscale"), CStr(XML_ImpresaPubblico.GetAttribute("partita_iva"))),
                                                                "",
                                                                "Spett.le",
                                                                Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                                Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                                BaseCode,
                                                                TopCode)


                        XmlDocPrivato_8 = New System.Xml.XmlDocument

                        XmlDocPrivato_8.LoadXml(Str_Contatto)

                        'recupero il nodo Contatto
                        XML_DatoContatto = XmlDocPrivato_8.SelectSingleNode("Contatto")

                        '------------------------------------------
                        '----- Tag RapCon (Risorsa Umana)
                        '------------------------------------------
                        Str_RisorsaUmana = xmlAnagHlp.XML_RapportoContabileXRisorseUmane(TipoOperazione_RisorsaUmanaImpresa,
                                                                            CStr(objParametri_Server.PivaSuperUser),
                                                                            CInt(0),
                                                                            0,
                                                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                            COD_FORNITORE,
                                                                            ,
                                                                            codice_socio,
                                                                            , , , , , , ,
                                                                            Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                                            Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)))



                    ElseIf CInt(XML_ImpresaPubblico.GetAttribute("tipo_operazione")) = enum_TipoOperazioneDB.Modifica Then
                        'MODIFICA

                        Dim Cod_Risum_Impresa As Integer = 0

                        If flag_EsisteContattoImpresa = False Then
                            'il contatto-azienda non esiste x niente
                            '(azienda vecchia, inserita su Gias quando ancora non veniva salvata anche come contatto)
                            '-> tutto in scrittura
                            TipoOperazione_ContattoImpresa = enum_TipoOperazioneDB.Scrittura
                            TipoOperazione_RisorsaUmanaImpresa = enum_TipoOperazioneDB.Scrittura
                        Else
                            'il contatto-azienda  esiste
                            '-> verifico se è presente come fornitore
                            Dim objRisUm As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R

                            Try
                                'verifico se l'azienda è contatto Gias con rapporto-contabile fornitore
                                Cod_Risum_Impresa = objRisUm.CodRisUm_by_PivaCodContattoCodRapporto(CStr(objParametri_Server.PivaSuperUser),
                                                                                                    CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                                                    COD_FORNITORE,
                                                                                                    objParametri_Server)

                            Catch ex As Exception
                                Cod_Risum_Impresa = 0
                            End Try

                            If Cod_Risum_Impresa = 0 Then
                                'azienda è presente ma ha un altro rapporto contabile associato
                                'vado in modifica del contatto (così ad esempio aggiorna la ragione sociale)
                                TipoOperazione_ContattoImpresa = enum_TipoOperazioneDB.Modifica
                                'non faccio niente sulla risorsa umana, metto operazione lettura
                                TipoOperazione_RisorsaUmanaImpresa = enum_TipoOperazioneDB.Lettura
                            Else
                                'il cod_risum esiste, è fornitore, posso andare in modifica
                                TipoOperazione_ContattoImpresa = enum_TipoOperazioneDB.Modifica
                                TipoOperazione_RisorsaUmanaImpresa = enum_TipoOperazioneDB.Modifica
                            End If

                        End If 'flag_EsisteContattoImpresa


                        '------------------------------------------
                        '----- Tag Contatto IMPRESA
                        '------------------------------------------

                        'sa_cod = 0 --> contatto visibile solo dall'impresa 
                        'sa_cod = -1 --> contatto visibile da tutti
                        Str_Contatto = xmlAnagHlp.XML_Contatto(CStr(TipoOperazione_ContattoImpresa),
                                                                CStr(objParametri_Server.PivaSuperUser),
                                                                CInt(0),
                                                                CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                Left(CStr(XML_ImpresaPubblico.GetAttribute("ragione_sociale")), 100),
                                                                "",
                                                                "",
                                                                "",
                                                                Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                                CInt(1),
                                                                Agro_If(XML_ImpresaPubblico.GetAttribute("codice_fiscale"), CStr(XML_ImpresaPubblico.GetAttribute("partita_iva"))),
                                                                "",
                                                                "Spett.le",
                                                                Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                                Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                                BaseCode,
                                                                TopCode)


                        XmlDocPrivato_8 = New System.Xml.XmlDocument

                        XmlDocPrivato_8.LoadXml(Str_Contatto)

                        'recupero il nodo Contatto
                        XML_DatoContatto = XmlDocPrivato_8.SelectSingleNode("Contatto")

                        '------------------------------------------
                        '----- Tag RapCon (Risorsa Umana)
                        '------------------------------------------

                        Str_RisorsaUmana = xmlAnagHlp.XML_RapportoContabileXRisorseUmane(TipoOperazione_RisorsaUmanaImpresa,
                                                                            CStr(objParametri_Server.PivaSuperUser),
                                                                            0,
                                                                            Cod_Risum_Impresa,
                                                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                            COD_FORNITORE,
                                                                            ,
                                                                            codice_socio,
                                                                            , , , , , , ,
                                                                            Agro_If(XML_ImpresaPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                                            Agro_If(XML_ImpresaPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)))


                    End If 'tipo operazione impresa

                    If Str_Contatto <> "" Then

                        XML_DatoContatto.InnerXml = Str_RisorsaUmana

                        XML_Nodo = XML_DatiContatti.OwnerDocument.ImportNode(XML_DatoContatto, True)

                        XML_DatiContatti.AppendChild(XML_Nodo)

                    End If

                    'controllo se è da inserire il tag del contatto
                    'se legale_rappresentante o lr_codicefiscale sono # non inserisco il contatto
                    If CStr(XML_ImpresaPubblico.GetAttribute("lr_codice_fiscale")) <> "#" Then

                        Dim tipo_operazione_RL As enum_TipoOperazioneDB = CInt(XML_ImpresaPubblico.GetAttribute("tipo_operazione"))

                        'se l'operazione dell'impresa è modifica potrebbe cmq non esistere il legale rappresentente
                        If CInt(XML_ImpresaPubblico.GetAttribute("tipo_operazione")) = 2 Then
                            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
                            If objContatti.Esiste_Contatto(Piva, CStr(XML_ImpresaPubblico.GetAttribute("lr_codice_fiscale")), AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server) = True Then
                                tipo_operazione_RL = enum_TipoOperazioneDB.Modifica
                            Else
                                tipo_operazione_RL = enum_TipoOperazioneDB.Scrittura
                            End If
                        End If
                        '------------------------------------------
                        '----- Tag Contatto
                        '------------------------------------------

                        'sa_cod = 0 --> contatto visibile solo dall'impresa 
                        'sa_cod = -1 --> contatto visibile da tutti

                        Dim Cognome As String = ""
                        Dim Nome As String = ""


                        If XML_ImpresaPubblico.HasAttribute("lr_cognome") = True Then
                            If XML_ImpresaPubblico.GetAttribute("lr_cognome") = "#" Then
                                Cognome = ""
                            Else
                                Cognome = XML_ImpresaPubblico.GetAttribute("lr_cognome")
                            End If
                        Else
                            Cognome = ""
                        End If

                        If XML_ImpresaPubblico.HasAttribute("lr_nome") = True Then
                            If XML_ImpresaPubblico.GetAttribute("lr_nome") = "#" Then
                                Nome = ""
                            Else
                                Nome = XML_ImpresaPubblico.GetAttribute("lr_nome")
                            End If
                        Else
                            Nome = ""
                        End If


                        IIf(XML_ImpresaPubblico.HasAttribute("lr_cognome") = True, CStr(XML_ImpresaPubblico.GetAttribute("lr_cognome")), "")

                        Str_Contatto = xmlAnagHlp.XML_Contatto(CStr(tipo_operazione_RL),
                                                                CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                CInt(0),
                                                                CStr(XML_ImpresaPubblico.GetAttribute("lr_codice_fiscale")),
                                                                Agro_If(CStr(XML_ImpresaPubblico.GetAttribute("legale_rappresentante")), ""),
                                                                CStr(Cognome),
                                                                CStr(Nome),
                                                                CStr(XML_ImpresaPubblico.GetAttribute("lr_sesso")),
                                                                CDate(XML_ImpresaPubblico.GetAttribute("lr_nascita_data")),
                                                                CInt(0),
                                                                CStr(XML_ImpresaPubblico.GetAttribute("lr_codice_fiscale")),
                                                                "",
                                                                "Egregio",
                                                                Agro_If(XML_ImpresaPubblico.GetAttribute("lr_validita_inizio"), CDate(#1/1/1900#)),
                                                                Agro_If(XML_ImpresaPubblico.GetAttribute("lr_validita_fine"), CDate(#12/31/2100#)),
                                                                BaseCode,
                                                                TopCode)


                        XmlDocPrivato_7 = New System.Xml.XmlDocument

                        XmlDocPrivato_7.LoadXml(Str_Contatto)

                        'recupero il nodo Contatto
                        XML_DatoContatto = XmlDocPrivato_7.SelectSingleNode("Contatto")

                        '------------------------------------------
                        '----- Tag RapCon (Risorsa Umana)
                        '------------------------------------------

                        Dim CodRisumLR As Integer = 0
                        ' se sono in modifica recuper il codice della risorsa umana
                        If tipo_operazione_RL = enum_TipoOperazioneDB.Modifica Then

                            Dim objRisUm As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
                            CodRisumLR = objRisUm.CodRisUm_by_PivaCodContattoCodRapporto(CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                                         CStr(XML_ImpresaPubblico.GetAttribute("lr_codice_fiscale")),
                                                                                         COD_LEGALE,
                                                                                         objParametri_Server)
                        End If

                        'sa_cod = 0 --> Risorsa visibile solo dall'impresa 
                        'sa_cod = -1 --> Risorsa visibile da tutti
                        Str_RisorsaUmana = xmlAnagHlp.XML_RapportoContabileXRisorseUmane(CStr(tipo_operazione_RL),
                                                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                            CInt(0),
                                                                            CodRisumLR,
                                                                            CStr(XML_ImpresaPubblico.GetAttribute("lr_codice_fiscale")),
                                                                            COD_LEGALE,
                                                                            , , , , , , , , ,
                                                                            Agro_If(XML_ImpresaPubblico.GetAttribute("lr_validita_inizio"), CDate(#1/1/1900#)),
                                                                            Agro_If(XML_ImpresaPubblico.GetAttribute("lr_validita_fine"), CDate(#12/31/2100#)))
                        '------------------------------------------
                        '----- Tag Indirizzi Legale Rappresentante
                        '------------------------------------------

                        'inizializzo la stringa
                        Str_Indirizzo = ""

                        'CREO TUTTI E 4 GLI INDIRIZZI ANCHE SE NON HO I DATI
                        'COME NELL'ON-LINE

                        Dim Cod_Indirizzo_Residenza As Integer = 0
                        Dim Cod_Indirizzo_Nascita As Integer = 0
                        Dim Cod_Indirizzo_Domicilio As Integer = 0
                        Dim Cod_Indirizzo_Residenza_Estiva As Integer = 0

                        Select Case tipo_operazione_RL

                            Case 2
                                Dim objContattiIndirizzi As New AgronicaCoreAnagrafeDAL.ContattiXIndirizzi_R
                                Dim DtIndirizzi As DataTable
                                Dim DrIndirizzoResidenza() As DataRow
                                Dim DrIndirizzoNascita() As DataRow
                                Dim DrIndirizzoDomicilio() As DataRow
                                Dim DrIndirizzoResidenzaEstiva() As DataRow
                                DtIndirizzi = objContattiIndirizzi.Leggi(CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                         CStr(XML_ImpresaPubblico.GetAttribute("lr_codice_fiscale")),
                                                                         0, 0,
                                                                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                         "", "", objParametri_Server)

                                If Not DtIndirizzi Is Nothing AndAlso DtIndirizzi.Rows.Count > 0 Then
                                    DrIndirizzoResidenza = DtIndirizzi.Select("Tipo_Indirizzo=3")
                                    If Not DrIndirizzoResidenza Is Nothing AndAlso DrIndirizzoResidenza.Length > 0 Then
                                        Cod_Indirizzo_Residenza = DrIndirizzoResidenza(0).Item("Cod_Indirizzo")
                                    End If
                                    DrIndirizzoDomicilio = DtIndirizzi.Select("Tipo_Indirizzo=2")
                                    If Not DrIndirizzoDomicilio Is Nothing AndAlso DrIndirizzoDomicilio.Length > 0 Then
                                        Cod_Indirizzo_Domicilio = DrIndirizzoDomicilio(0).Item("Cod_Indirizzo")
                                    End If
                                    DrIndirizzoNascita = DtIndirizzi.Select("Tipo_Indirizzo=5")
                                    If Not DrIndirizzoNascita Is Nothing AndAlso DrIndirizzoNascita.Length > 0 Then
                                        Cod_Indirizzo_Nascita = DrIndirizzoNascita(0).Item("Cod_Indirizzo")
                                    End If
                                    DrIndirizzoResidenzaEstiva = DtIndirizzi.Select("Tipo_Indirizzo=4")
                                    If Not DrIndirizzoResidenzaEstiva Is Nothing AndAlso DrIndirizzoResidenzaEstiva.Length > 0 Then
                                        Cod_Indirizzo_Residenza_Estiva = DrIndirizzoResidenzaEstiva(0).Item("Cod_Indirizzo")
                                    End If
                                End If
                            Case Else
                                Cod_Indirizzo = 0
                        End Select



                        'residenza del legale rappresentante
                        Str_Indirizzo += xmlAnagHlp.XML_Indirizzo(CStr(tipo_operazione_RL),
                                                    CInt(3),
                                                    CInt(Cod_Indirizzo_Residenza),
                                                    CStr(XML_ImpresaPubblico.GetAttribute("lr_indirizzo")),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_frazione"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_cap"), ""),
                                                    CStr(XML_ImpresaPubblico.GetAttribute("lr_comune")),
                                                    CStr(XML_ImpresaPubblico.GetAttribute("lr_provincia")),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_stato"), "IT"),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_note_indirizzo"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_codice_istat_provincia"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_codice_istat_comune"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)

                        'luogo di nascita del legale rappresentante
                        Str_Indirizzo += xmlAnagHlp.XML_Indirizzo(CStr(tipo_operazione_RL),
                                                    CInt(5),
                                                    CInt(Cod_Indirizzo_Nascita),
                                                    "",
                                                    "",
                                                    "",
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_nascita_comune"), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_nascita_provincia"), ""),
                                                    "",
                                                    "",
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_nascita_istat_provincia"), "000"),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_nascita_istat_comune"), "000"),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)

                        'domicilio del legale rappresentante
                        Str_Indirizzo += xmlAnagHlp.XML_Indirizzo(CStr(tipo_operazione_RL),
                                                    CInt(2),
                                                    CInt(Cod_Indirizzo_Domicilio),
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "000",
                                                    "000",
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)


                        'residenza estiva del legale rappresentante
                        Str_Indirizzo += xmlAnagHlp.XML_Indirizzo(CStr(tipo_operazione_RL),
                                                    CInt(4),
                                                    CInt(Cod_Indirizzo_Residenza_Estiva),
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "000",
                                                    "000",
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)

                        '------------------------------------------
                        '----- Tag Rubrica Legale Rappresentante
                        '------------------------------------------

                        'inizializzo la stringa
                        Str_Rubrica = ""

                        For i_t = 1 To 5

                            If CStr(XML_ImpresaPubblico.GetAttribute("lr_rubrica_" & i_t)) <> "#" Then

                                Str_Rubrica += xmlAnagHlp.XML_Rubrica(CStr(tipo_operazione_RL),
                                                    CInt(0),
                                                    Agro_If(Split(XML_ImpresaPubblico.GetAttribute("lr_rubrica_" & i_t), ":")(1), ""),
                                                    Agro_If(Split(XML_ImpresaPubblico.GetAttribute("lr_rubrica_" & i_t), ":")(0), ""),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ImpresaPubblico.GetAttribute("lr_validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)
                            End If

                        Next

                        XML_DatoContatto.InnerXml = Str_RisorsaUmana & Str_Indirizzo & Str_Rubrica

                        XML_Nodo = XML_DatiContatti.OwnerDocument.ImportNode(XML_DatoContatto, True)

                        XML_DatiContatti.AppendChild(XML_Nodo)


                    End If      'CONTROLLO SE BISOGNA INSERIRE IL CONTATTO DEL RAPPRESENTANTE LEGALE



                    '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                    '££££££££££££££££££££££           ALTRI CONTATTI                ££££££££££££££££££££££££££££££££
                    '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

                    'Leggo l'insieme dei Contatti
                    XMLs_ContattoPubblico = XML_ImpresaPubblico.GetElementsByTagName("Contatto")

                    Dim i_c As Integer      ' contatore per i contatti
                    For i_c = 0 To XMLs_ContattoPubblico.Count - 1

                        XML_ContattoPubblico = XMLs_ContattoPubblico(i_c)

                        'sa_cod = 0 --> contatto visibile solo dall'impresa 
                        'sa_cod = -1 --> contatto visibile da tutti

                        Dim Cognome As String = ""
                        Dim Nome As String = ""

                        If XML_ContattoPubblico.HasAttribute("cognome") = True Then
                            If XML_ContattoPubblico.GetAttribute("cognome") = "#" Then
                                Cognome = ""
                            Else
                                Cognome = XML_ContattoPubblico.GetAttribute("cognome")
                            End If
                        Else
                            Cognome = ""
                        End If

                        If XML_ContattoPubblico.HasAttribute("nome") = True Then
                            If XML_ContattoPubblico.GetAttribute("nome") = "#" Then
                                Nome = ""
                            Else
                                Nome = XML_ContattoPubblico.GetAttribute("nome")
                            End If
                        Else
                            Nome = ""
                        End If


                        Str_Contatto = xmlAnagHlp.XML_Contatto(CStr(XML_ContattoPubblico.GetAttribute("tipo_operazione")),
                                                    CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                    CInt(0),
                                                    CStr(XML_ContattoPubblico.GetAttribute("codice_fiscale")),
                                                    "",
                                                    CStr(Cognome),
                                                    CStr(Nome),
                                                    CStr(XML_ContattoPubblico.GetAttribute("sesso")),
                                                    CDate(XML_ContattoPubblico.GetAttribute("nascita_data")),
                                                    CInt(0),
                                                    CStr(XML_ContattoPubblico.GetAttribute("codice_fiscale")),
                                                    "",
                                                    "Egregio",
                                                    Agro_If(XML_ContattoPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ContattoPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)


                        '------------------------------------------
                        '----- Tag RapCon (Risorsa Umana)
                        '------------------------------------------

                        XmlDocPrivato_8_b = New System.Xml.XmlDocument

                        XmlDocPrivato_8_b.LoadXml(Str_Contatto)

                        'recupero il nodo Contatto
                        XML_DatoContatto = XmlDocPrivato_8_b.SelectSingleNode("Contatto")

                        Str_RisorsaUmana = ""
                        Str_Indirizzo = ""
                        Str_Rubrica = ""

                        'Leggo l'insieme dei Contatti
                        XMLs_RuoloPubblico = XML_ContattoPubblico.GetElementsByTagName("Ruolo")


                        Dim i_r As Integer      ' contatore per i contatti
                        For i_r = 0 To XMLs_RuoloPubblico.Count - 1

                            XML_RuoloPubblico = XMLs_RuoloPubblico(i_r)

                            'sa_cod = 0 --> Risorsa visibile solo dall'impresa 
                            'sa_cod = -1 --> Risorsa visibile da tutti
                            Str_RisorsaUmana = xmlAnagHlp.XML_RapportoContabileXRisorseUmane(CStr(XML_RuoloPubblico.GetAttribute("tipo_operazione")),
                                                                                  CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                                  CInt(0),
                                                                                  0,
                                                                                  CStr(XML_ContattoPubblico.GetAttribute("codice_fiscale")),
                                                                                  CInt(XML_RuoloPubblico.GetAttribute("codice_ruolo")),
                                                                                  , , , , , , , , ,
                                                                                  Agro_If(XML_ContattoPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                                                  Agro_If(XML_ContattoPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)))



                        Next

                        Str_Rubrica = String.Empty
                        ' rubrica
                        If CStr(XML_ContattoPubblico.GetAttribute("email")) <> "" Then
                            Str_Rubrica += xmlAnagHlp.XML_Rubrica(1,
                                                       CInt(0),
                                                       XML_ContattoPubblico.GetAttribute("email"),
                                                       "Email",
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                       BaseCode,
                                                       TopCode)
                        End If

                        ' rubrica
                        If CStr(XML_ContattoPubblico.GetAttribute("telefono")) <> "" Then
                            Str_Rubrica += xmlAnagHlp.XML_Rubrica(1,
                                                       CInt(0),
                                                       XML_ContattoPubblico.GetAttribute("telefono"),
                                                       "Telefono",
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                       BaseCode,
                                                       TopCode)
                        End If


                        ' rubrica
                        If CStr(XML_ContattoPubblico.GetAttribute("fax")) <> "" Then
                            Str_Rubrica += xmlAnagHlp.XML_Rubrica(1,
                                                       CInt(0),
                                                       XML_ContattoPubblico.GetAttribute("fax"),
                                                       "Fax",
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                       BaseCode,
                                                       TopCode)
                        End If



                        'residenza del contatto
                        Str_Indirizzo += xmlAnagHlp.XML_Indirizzo(CStr(XML_ContattoPubblico.GetAttribute("tipo_operazione")),
                                                       CInt(3),
                                                       CInt(0),
                                                       CStr(XML_ContattoPubblico.GetAttribute("residenza_indirizzo")),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("residenza_frazione"), ""),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("residenza_cap"), ""),
                                                       CStr(XML_ContattoPubblico.GetAttribute("residenza_comune")),
                                                       CStr(XML_ContattoPubblico.GetAttribute("residenza_provincia")),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("residenza_stato"), "IT"),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("residenza_note_indirizzo"), ""),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("residenza_codice_istat_provincia"), ""),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("residenza_codice_istat_comune"), ""),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                       BaseCode,
                                                       TopCode)

                        'luogo di nascita del contatto
                        Str_Indirizzo += xmlAnagHlp.XML_Indirizzo(CStr(XML_ContattoPubblico.GetAttribute("tipo_operazione")),
                                                       CInt(5),
                                                       CInt(0),
                                                       "",
                                                       "",
                                                       "",
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("nascita_comune"), ""),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("nascita_provincia"), ""),
                                                       "",
                                                       "",
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("nascita_istat_provincia"), "000"),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("nascita_istat_comune"), "000"),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                       BaseCode,
                                                       TopCode)

                        'domicilio del contatto
                        Str_Indirizzo += xmlAnagHlp.XML_Indirizzo(CStr(XML_ContattoPubblico.GetAttribute("tipo_operazione")),
                                                    CInt(2),
                                                    CInt(0),
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "",
                                                    "000",
                                                    "000",
                                                    Agro_If(XML_ContattoPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_ContattoPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)


                        'residenza estiva del contatto
                        Str_Indirizzo += xmlAnagHlp.XML_Indirizzo(CStr(XML_ContattoPubblico.GetAttribute("tipo_operazione")),
                                                       CInt(4),
                                                       CInt(0),
                                                       "",
                                                       "",
                                                       "",
                                                       "",
                                                       "",
                                                       "",
                                                       "",
                                                       "000",
                                                       "000",
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                       Agro_If(XML_ContattoPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                       BaseCode,
                                                       TopCode)


                        XML_DatoContatto.InnerXml = Str_RisorsaUmana & Str_Indirizzo & Str_Rubrica

                        XML_Nodo = XML_DatiContatti.OwnerDocument.ImportNode(XML_DatoContatto, True)

                        XML_DatiContatti.AppendChild(XML_Nodo)

                    Next

                    ' nodo Dati_Contatti agganciato al nodo impresa
                    XML_Nodo = XML_ImpresaPrivato.OwnerDocument.ImportNode(XML_DatiContatti, True)

                    XML_ImpresaPrivato.AppendChild(XML_Nodo)

                    XmlDocPrivato_8_b = Nothing
                    XmlDocPrivato_2 = Nothing

                    'pippo = XML_ImpresaPrivato.OuterXml

                    '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                    '££££££££££££££££££££££                 FASCICOLO        £££££££££££££££££££££££££££££££££££££££
                    '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

                    XMLs_FascicoloPubblico = XML_ImpresaPubblico.GetElementsByTagName("Fascicolo")

                    For i_Fascicolo = 0 To XMLs_FascicoloPubblico.Count - 1

                        XML_FascicoloPubblico = XMLs_FascicoloPubblico(i_Fascicolo)

                        'importo il tag DATI-CENTRI AZIENDALI nel documento dell'impresa
                        XML_Nodo = XML_ImpresaPrivato.OwnerDocument.ImportNode(XML_FascicoloPubblico, True)

                        'appendo il tag DATI-CENTRI AZIENDALI al centro aziendale
                        XML_ImpresaPrivato.AppendChild(XML_Nodo)

                    Next

                    Dim pippo As String = XML_ImpresaPrivato.OuterXml


                    '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                    '££££££££££££££££££££££                 CENTRO AZIENDALE        ££££££££££££££££££££££££££££££££
                    '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

                    XML_DatiCentriAziendali = XML_ImpresaPrivato.OwnerDocument.CreateElement("DatiCentriAziendali")

                    'Leggo l'insieme dei nodi Centro Aziendale
                    XMLs_CentroAziendalePubblico = XML_ImpresaPubblico.GetElementsByTagName("CentroAziendale")

                    For i_2 = 0 To XMLs_CentroAziendalePubblico.Count - 1

                        'ciclo sui nodi centro aziendale
                        XML_CentroAziendalePubblico = XMLs_CentroAziendalePubblico(i_2)

                        '------------------------------------------
                        '----- Tag Rubrica Legale Rappresentante
                        '------------------------------------------

                        XML_CentroAziendalePrivato = xmlAnagHlp.XML_CentroAziendale(CStr(XML_CentroAziendalePubblico.GetAttribute("tipo_operazione")),
                                                                        CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                        CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                                        Agro_If(XML_CentroAziendalePubblico.GetAttribute("nome_centro"), "Centro n." & Right("00" & i_2 + 1, 2)),
                                                                        0, 0, 0, 0, 0, 0, "", "", "", 101, 0,
                                                                        Agro_If(XML_CentroAziendalePubblico.GetAttribute("sup_bosco"), 0),
                                                                        0, 0,
                                                                        Agro_If(XML_CentroAziendalePubblico.GetAttribute("sup_prati"), 0),
                                                                        Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                                        Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                                        BaseCode,
                                                                        TopCode,
                                                                        Agro_If(XML_CentroAziendalePubblico.GetAttribute("titolo_possesso"), CInt(1)),
                                                                        0, 0, 0)

                        '------------------------------------------
                        '----- Tag Codici Centro Aziendale
                        '------------------------------------------

                        'inizializzo la stringa dei codici
                        Str_Codice = ""

                        TipoOp_Codice = CStr(XML_CentroAziendalePubblico.GetAttribute("tipo_operazione"))

                        If TipoOp_Codice = enum_TipoOperazioneDB.Modifica Then
                            TipoOp_Codice = enum_TipoOperazioneDB.Scrittura
                        End If

                        '------------------------------------------
                        '----- Tag Titolo Possesso centro
                        '------------------------------------------

                        Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                enum_CodiciAnagrafe.TitoloPossesso,
                                                Agro_If(XML_CentroAziendalePubblico.GetAttribute("titolo_possesso"), CStr(1)),
                                                Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                BaseCode,
                                                TopCode)

                        '------------------------------------------
                        '----- Tag Codice Operatore Centro Aziendale
                        '------------------------------------------

                        Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                enum_CodiciAnagrafe.CodiceCentro_Attuale,
                                                Agro_If(XML_CentroAziendalePubblico.GetAttribute("codice_operatore"), CStr(0)),
                                                Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                BaseCode,
                                                TopCode)

                        '------------------------------------------
                        '----- Tag Codice Tipo Attivita Centro Aziendale
                        '------------------------------------------

                        Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                enum_CodiciAnagrafe.TipoAttivita,
                                                Agro_If(XML_CentroAziendalePubblico.GetAttribute("tipo_attivita"), CStr("PV")),
                                                Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                BaseCode,
                                                TopCode)

                        '------------------------------------------
                        '----- Tag Chiave Cliente
                        '------------------------------------------

                        If CStr(XML_CentroAziendalePubblico.GetAttribute("c_chiave_cliente")) <> "#" Then
                            Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                    Id_Codice_ChiaveCliente,
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("c_chiave_cliente"), ""),
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)
                        End If

                        '------------------------------------------
                        '----- Zespri, codice kpin
                        '------------------------------------------

                        If CInt(XML_CentroAziendalePubblico.GetAttribute("tipo_operazione")) = enum_TipoOperazioneDB.Scrittura AndAlso
                            XML_CentroAziendalePubblico.HasAttribute("zespri_kpin") AndAlso
                            CStr(XML_CentroAziendalePubblico.GetAttribute("zespri_kpin")) <> "#" Then
                            Str_Codice += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                    enum_CodiciAnagrafe.Zespri_Codice_kPIN,
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("zespri_kpin"), ""),
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)
                        End If

                        Try

                            'se ci sono dei codici legati al centro
                            ' se sono in modifica
                            'e se il codice del centro è valorizzato
                            If Str_Codice <> "" And
                                CInt(XML_CentroAziendalePubblico.GetAttribute("tipo_operazione")) = enum_TipoOperazioneDB.Modifica And
                                    CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")) <> 0 Then

                                Dim XML_Codice_list As XmlNodeList
                                Dim XML_Codice As XmlElement
                                Dim i As Integer
                                XML_Codice_list = XML_CentroAziendalePubblico.SelectNodes("CodiceCentro")
                                For i = 0 To XML_Codice_list.Count - 1
                                    XML_Codice = XML_Codice_list.Item(i)
                                    XML_Codice.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Scrittura))
                                Next

                                Codici_toDelete_Centro(HT_Piva_objCodici,
                                                       CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                       CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")))

                            End If

                        Catch ex As Exception
                            Dim debug As Boolean = True
                        End Try


                        '------------------------------------------
                        '----- Tag Indirizzo Centro Aziendale
                        '------------------------------------------

                        'inizializzo la stringa
                        Str_Indirizzo = ""

                        Cod_Indirizzo = 0

                        Select Case CInt(XML_CentroAziendalePubblico.GetAttribute("tipo_operazione"))

                            Case 2
                                'Cod_Indirizzo = Cod_Indirizzo_Centro(CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")), _
                                '                                    CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")), _
                                '                                    Connessione)

                                If CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")) <> 0 Then
                                    Cod_Indirizzo = objCentriInd.CodIndirizzo_from_PivaSaCod(CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                                                            objParametri_Server)
                                Else
                                    'qui non deve mai capitare!!!!!!
                                    'se finisce qui significa che c'è un errore
                                    'nell'importatore o nel sincronizzatore
                                    'che non valorizza il codice del centro in modifica
                                    Cod_Indirizzo = 0
                                End If

                            Case Else
                                Cod_Indirizzo = 0
                        End Select

                        Str_Indirizzo += xmlAnagHlp.XML_Indirizzo(CStr(XML_CentroAziendalePubblico.GetAttribute("tipo_operazione")),
                                                    CInt(1),
                                                    CInt(Cod_Indirizzo),
                                                    CStr(XML_CentroAziendalePubblico.GetAttribute("c_indirizzo")),
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("c_frazione"), ""),
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("c_cap"), ""),
                                                    CStr(XML_CentroAziendalePubblico.GetAttribute("c_comune")),
                                                    CStr(XML_CentroAziendalePubblico.GetAttribute("c_provincia")),
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("c_stato"), "IT"),
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("c_note_indirizzo"), ""),
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("c_codice_istat_provincia"), ""),
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("c_codice_istat_comune"), ""),
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)

                        '------------------------------------------
                        '----- Tag Rubrica Centro Aziendale
                        '------------------------------------------

                        'inizializzo la stringa
                        Str_Rubrica = ""

                        For i_t = 1 To 9

                            If CStr(XML_CentroAziendalePubblico.GetAttribute("c_rubrica_" & i_t)) <> "#" Then

                                Str_Rubrica += xmlAnagHlp.XML_Rubrica(CStr(XML_CentroAziendalePubblico.GetAttribute("tipo_operazione")),
                                                    CInt(0),
                                                    CStr(Split(XML_CentroAziendalePubblico.GetAttribute("c_rubrica_" & i_t), ":")(1)),
                                                    CStr(Split(XML_CentroAziendalePubblico.GetAttribute("c_rubrica_" & i_t), ":")(0)),
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                    Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                    BaseCode,
                                                    TopCode)
                            End If

                        Next

                        XML_CentroAziendalePrivato.InnerXml = Str_Codice + Str_Indirizzo + Str_Rubrica

                        'pippo = XML_CentroAziendalePrivato.OuterXml

                        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                        '££££££££££££££££££££££                 PARTICELLE CATASTALI                ££££££££££££££££££££
                        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

                        '------------------------------------------
                        '----- Tag Dati Praticelle Catastali
                        '------------------------------------------

                        XML_DatiParticelle = XML_CentroAziendalePrivato.OwnerDocument.CreateElement("DatiParticelle")

                        XML_CentroAziendalePrivato.AppendChild(XML_DatiParticelle)

                        pippo = XML_CentroAziendalePrivato.OuterXml

                        'Leggo l'insieme dei nodi particella
                        XMLs_ParticellaPubblico = XML_CentroAziendalePubblico.GetElementsByTagName("Particella")

                        'inizializzo la stringa
                        Str_Particella = ""

                        'ridimensiono la matrice delle particelle catastali
                        ReDim MatriceParticelle(XMLs_ParticellaPubblico.Count - 1, 6)

                        For i_3 = 0 To XMLs_ParticellaPubblico.Count - 1

                            'ciclo sui nodi centro aziendale
                            XML_ParticellaPubblico = XMLs_ParticellaPubblico(i_3)

                            'riempio la matrice
                            MatriceParticelle(i_3, 0) = CStr(XML_ParticellaPubblico.GetAttribute("codice_particella"))
                            MatriceParticelle(i_3, 1) = CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_provincia"))
                            MatriceParticelle(i_3, 2) = CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_comune"))
                            MatriceParticelle(i_3, 3) = LCase(CStr(XML_ParticellaPubblico.GetAttribute("sezione")))
                            MatriceParticelle(i_3, 4) = CStr(XML_ParticellaPubblico.GetAttribute("foglio"))
                            MatriceParticelle(i_3, 5) = CStr(XML_ParticellaPubblico.GetAttribute("numero"))
                            MatriceParticelle(i_3, 6) = LCase(CStr(XML_ParticellaPubblico.GetAttribute("subalterno")))

                            '------------------------------------------
                            '----- Tag Particella
                            '------------------------------------------

                            XML_ParticellaPrivato = xmlAnagHlp.XML_Particella(CStr(XML_ParticellaPubblico.GetAttribute("tipo_operazione")),
                                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                            CInt(0),
                                                            CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_provincia")),
                                                            CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_comune")),
                                                            LCase(CStr(XML_ParticellaPubblico.GetAttribute("sezione"))),
                                                            CInt(XML_ParticellaPubblico.GetAttribute("foglio")),
                                                            CInt(XML_ParticellaPubblico.GetAttribute("numero")),
                                                            LCase(CStr(XML_ParticellaPubblico.GetAttribute("subalterno"))),
                                                            Agro_If(XML_ParticellaPubblico.GetAttribute("partita_catastale"), CStr("")),
                                                            CDbl(XML_ParticellaPubblico.GetAttribute("ettari")),
                                                            CInt(XML_ParticellaPubblico.GetAttribute("are")),
                                                            CInt(XML_ParticellaPubblico.GetAttribute("centiare")),
                                                            Agro_If(XML_ParticellaPubblico.GetAttribute("titolo_possesso"), CInt(1)),
                                                            Agro_If(XML_ParticellaPubblico.GetAttribute("qualita_catasto_codice"), CInt(0)),
                                                            Agro_If(XML_ParticellaPubblico.GetAttribute("classe"), CStr("")),
                                                            Agro_If(XML_ParticellaPubblico.GetAttribute("reddito_dominicale"), CDbl(0)),
                                                            Agro_If(XML_ParticellaPubblico.GetAttribute("reddito_agrario"), CDbl(0)),
                                                            Agro_If(XML_ParticellaPubblico.GetAttribute("superficie_condotta"), CDbl(0)),
                                                            #1/1/1900#,
                                                            #12/31/2100#,
                                                            Agro_If(XML_ParticellaPubblico.GetAttribute("validita_inizio_possesso"), CDate(#1/1/1900#)),
                                                            Agro_If(XML_ParticellaPubblico.GetAttribute("validita_fine_possesso"), CDate(#12/31/2100#)),
                                                            BaseCode,
                                                            TopCode)

                            pippo = XML_ParticellaPrivato.OuterXml

                            Str_Possesso = ""

                            XMLs_ParticellaPossessiPubblico = XML_ParticellaPubblico.GetElementsByTagName("Possesso")

                            If Not XMLs_ParticellaPossessiPubblico Is Nothing Then

                                For i_4 = 0 To XMLs_ParticellaPossessiPubblico.Count - 1

                                    XML_ParticellaPossessoPubblico = XMLs_ParticellaPossessiPubblico(i_4)

                                    Str_Possesso += xmlAnagHlp.XML_ParticellaPossesso(CStr(XML_ParticellaPossessoPubblico.GetAttribute("tipo_operazione")),
                                                                           CStr(XML_ParticellaPossessoPubblico.GetAttribute("partita_catastale")),
                                                                           CInt(XML_ParticellaPossessoPubblico.GetAttribute("titolo_possesso")),
                                                                            CDbl(XML_ParticellaPossessoPubblico.GetAttribute("superficie_condotta")),
                                                                            CDate(XML_ParticellaPossessoPubblico.GetAttribute("validita_inizio_possesso")),
                                                                            CDate(XML_ParticellaPossessoPubblico.GetAttribute("validita_fine_possesso")),
                                                                            BaseCode,
                                                                            TopCode)

                                Next

                            End If


                            Str_Zona = ""

                            XMLs_ZonaxParticellaPubblico = XML_ParticellaPubblico.GetElementsByTagName("Zona")

                            If Not XMLs_ZonaxParticellaPubblico Is Nothing Then

                                For i_4 = 0 To XMLs_ZonaxParticellaPubblico.Count - 1

                                    XML_ZonaxParticellaPubblico = XMLs_ZonaxParticellaPubblico(i_4)

                                    Str_Zona += xmlAnagHlp.XML_ZonaxParticella(CStr(XML_ZonaxParticellaPubblico.GetAttribute("tipo_operazione")),
                                                                          CInt(XML_ZonaxParticellaPubblico.GetAttribute("codice_zona_gias")),
                                                                          CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_provincia")),
                                                                          CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_comune")),
                                                                          LCase(CStr(XML_ParticellaPubblico.GetAttribute("sezione"))),
                                                                            CInt(XML_ParticellaPubblico.GetAttribute("foglio")),
                                                                            CInt(XML_ParticellaPubblico.GetAttribute("numero")),
                                                                            LCase(CStr(XML_ParticellaPubblico.GetAttribute("subalterno"))),
                                                                            CDbl(XML_ZonaxParticellaPubblico.GetAttribute("area")),
                                                                            #1/1/1900#,
                                                                            #12/31/2100#,
                                                                            BaseCode,
                                                                            TopCode)

                                Next

                            End If

                            Str_Macrousi = ""

                            XMLs_ParticellaxMacrousiPubblico = XML_ParticellaPubblico.GetElementsByTagName("Macrouso")

                            Dim Validita_Inizio_Macrouso As Date = AGRODATAINIZIO
                            Dim Validita_Fine_Macrouso As Date = AGRODATAFINE
                            Dim Validita_Inizio_Utilizzo As Date = AGRODATAINIZIO
                            Dim Validita_Fine_Utilizzo As Date = AGRODATAFINE
                            Dim Id_Utilizzo As String

                            If Not XMLs_ParticellaxMacrousiPubblico Is Nothing Then

                                For i_4 = 0 To XMLs_ParticellaxMacrousiPubblico.Count - 1

                                    strUtilizzi = ""

                                    XML_ParticellaxMacrousiPubblico = XMLs_ParticellaxMacrousiPubblico(i_4)

                                    Validita_Inizio_Macrouso = AGRODATAINIZIO
                                    Validita_Fine_Macrouso = AGRODATAFINE

                                    If XML_ParticellaxMacrousiPubblico.HasAttribute("validita_inizio") = True Then
                                        Validita_Inizio_Macrouso = CDate(XML_ParticellaxMacrousiPubblico.GetAttribute("validita_inizio"))
                                    End If
                                    If XML_ParticellaxMacrousiPubblico.HasAttribute("validita_fine") = True Then
                                        Validita_Fine_Macrouso = CDate(XML_ParticellaxMacrousiPubblico.GetAttribute("validita_fine"))
                                    End If

                                    Str_Macrouso = xmlAnagHlp.XML_ParticellaxMacrouso(CStr(XML_ParticellaxMacrousiPubblico.GetAttribute("tipo_operazione")),
                                                                           CStr(XML_ParticellaxMacrousiPubblico.GetAttribute("piva")),
                                                                           CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_provincia")),
                                                                           CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_comune")),
                                                                           LCase(CStr(XML_ParticellaPubblico.GetAttribute("sezione"))),
                                                                           CInt(XML_ParticellaPubblico.GetAttribute("foglio")),
                                                                           CInt(XML_ParticellaPubblico.GetAttribute("numero")),
                                                                           LCase(CStr(XML_ParticellaPubblico.GetAttribute("subalterno"))),
                                                                           CStr(XML_ParticellaxMacrousiPubblico.GetAttribute("codice_macrouso_gias")),
                                                                           CDbl(XML_ParticellaxMacrousiPubblico.GetAttribute("superficie")),
                                                                           Validita_Inizio_Macrouso,
                                                                           Validita_Fine_Macrouso,
                                                                           BaseCode,
                                                                           TopCode)

                                    XMLs_ParticellaxMacrousixUtilizziPubblico = XML_ParticellaxMacrousiPubblico.GetElementsByTagName("Utilizzo")

                                    Dim XmlDocPrivato_Macrouso = New System.Xml.XmlDocument

                                    XmlDocPrivato_Macrouso.LoadXml(Str_Macrouso)

                                    XML_ParticellaxMacrousiPrivato = XmlDocPrivato_Macrouso.SelectSingleNode("Macrouso")

                                    strUtilizzi = ""

                                    For i_5 = 0 To XMLs_ParticellaxMacrousixUtilizziPubblico.Count - 1

                                        XML_ParticellaxMacrousixUtilizziPubblico = XMLs_ParticellaxMacrousixUtilizziPubblico(i_5)

                                        Validita_Inizio_Utilizzo = AGRODATAINIZIO
                                        Validita_Fine_Utilizzo = AGRODATAFINE

                                        If XML_ParticellaxMacrousixUtilizziPubblico.HasAttribute("validita_inizio") = True Then
                                            Validita_Inizio_Utilizzo = CDate(XML_ParticellaxMacrousixUtilizziPubblico.GetAttribute("validita_inizio"))
                                        End If
                                        If XML_ParticellaxMacrousixUtilizziPubblico.HasAttribute("validita_fine") = True Then
                                            Validita_Fine_Utilizzo = CDate(XML_ParticellaxMacrousixUtilizziPubblico.GetAttribute("validita_fine"))
                                        End If
                                        If XML_ParticellaxMacrousixUtilizziPubblico.HasAttribute("id_utilizzo") = True Then
                                            Id_Utilizzo = XML_ParticellaxMacrousixUtilizziPubblico.GetAttribute("id_utilizzo")
                                        End If

                                        If Id_Utilizzo <> "#" And Id_Utilizzo <> "" Then

                                            strUtilizzi += xmlAnagHlp.XML_ParticellaxMacrousoxUtilizzo(CStr(XML_ParticellaxMacrousixUtilizziPubblico.GetAttribute("tipo_operazione")),
                                                                                            Id_Utilizzo,
                                                                                            CStr(XML_ParticellaxMacrousiPubblico.GetAttribute("piva")),
                                                                                            CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_provincia")),
                                                                                            CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_comune")),
                                                                                            LCase(CStr(XML_ParticellaPubblico.GetAttribute("sezione"))),
                                                                                            CInt(XML_ParticellaPubblico.GetAttribute("foglio")),
                                                                                            CInt(XML_ParticellaPubblico.GetAttribute("numero")),
                                                                                            LCase(CStr(XML_ParticellaPubblico.GetAttribute("subalterno"))),
                                                                                            CStr(XML_ParticellaxMacrousiPubblico.GetAttribute("codice_macrouso_gias")),
                                                                                            CStr(XML_ParticellaxMacrousixUtilizziPubblico.GetAttribute("codice_specie_agea")),
                                                                                            CStr(XML_ParticellaxMacrousixUtilizziPubblico.GetAttribute("codice_varieta_agea")),
                                                                                            CDbl(XML_ParticellaxMacrousixUtilizziPubblico.GetAttribute("superficie")),
                                                                                            Validita_Inizio_Utilizzo,
                                                                                            Validita_Fine_Utilizzo,
                                                                                            BaseCode,
                                                                                            TopCode)
                                        Else

                                            strUtilizzi += xmlAnagHlp.XML_ParticellaxMacrousoxUtilizzo(CStr(XML_ParticellaxMacrousixUtilizziPubblico.GetAttribute("tipo_operazione")),
                                                                                            CStr(XML_ParticellaxMacrousiPubblico.GetAttribute("piva")),
                                                                                            CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_provincia")),
                                                                                            CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_comune")),
                                                                                            LCase(CStr(XML_ParticellaPubblico.GetAttribute("sezione"))),
                                                                                            CInt(XML_ParticellaPubblico.GetAttribute("foglio")),
                                                                                            CInt(XML_ParticellaPubblico.GetAttribute("numero")),
                                                                                            LCase(CStr(XML_ParticellaPubblico.GetAttribute("subalterno"))),
                                                                                            CStr(XML_ParticellaxMacrousiPubblico.GetAttribute("codice_macrouso_gias")),
                                                                                            CStr(XML_ParticellaxMacrousixUtilizziPubblico.GetAttribute("codice_specie_agea")),
                                                                                            CStr(XML_ParticellaxMacrousixUtilizziPubblico.GetAttribute("codice_varieta_agea")),
                                                                                            CDbl(XML_ParticellaxMacrousixUtilizziPubblico.GetAttribute("superficie")),
                                                                                            Validita_Inizio_Utilizzo,
                                                                                            Validita_Fine_Utilizzo,
                                                                                            BaseCode,
                                                                                            TopCode)
                                        End If



                                        Id_Utilizzo = ""

                                    Next

                                    XML_ParticellaxMacrousiPrivato.InnerXml = strUtilizzi

                                    XML_Nodo = XML_ParticellaPrivato.OwnerDocument.ImportNode(XML_ParticellaxMacrousiPrivato, True)

                                    XML_ParticellaPrivato.AppendChild(XML_Nodo)

                                Next

                            End If

                            Str_Classamento = ""

                            Dim Porzione As String = ""
                            Dim Classe As String = ""
                            Dim Sup_Classe As Double = 0
                            Dim Reddito_Dominicale As Double = 0
                            Dim Reddito_Agrario As Double = 0
                            Dim Deduzione As String = ""

                            XMLs_ClassamentoxParticellaPubblico = XML_ParticellaPubblico.GetElementsByTagName("Classamento")

                            If Not XMLs_ClassamentoxParticellaPubblico Is Nothing Then

                                For i_4 = 0 To XMLs_ClassamentoxParticellaPubblico.Count - 1

                                    XML_ClassamentoxParticellaPubblico = XMLs_ClassamentoxParticellaPubblico(i_4)

                                    If XML_ClassamentoxParticellaPubblico.HasAttribute("porzione") = True Then
                                        Porzione = XML_ClassamentoxParticellaPubblico.GetAttribute("porzione")
                                    End If
                                    If XML_ClassamentoxParticellaPubblico.HasAttribute("classe") = True Then
                                        Classe = XML_ClassamentoxParticellaPubblico.GetAttribute("classe")
                                    End If
                                    If XML_ClassamentoxParticellaPubblico.HasAttribute("sup_classe") = True AndAlso IsNumeric(XML_ClassamentoxParticellaPubblico.HasAttribute("sup_classe")) Then
                                        Sup_Classe = CDbl(XML_ClassamentoxParticellaPubblico.GetAttribute("sup_classe"))
                                    End If
                                    If XML_ClassamentoxParticellaPubblico.HasAttribute("reddito_dominicale") = True AndAlso IsNumeric(XML_ClassamentoxParticellaPubblico.HasAttribute("reddito_dominicale")) Then
                                        Reddito_Dominicale = CDbl(XML_ClassamentoxParticellaPubblico.GetAttribute("reddito_dominicale"))
                                    End If
                                    If XML_ClassamentoxParticellaPubblico.HasAttribute("reddito_agrario") = True AndAlso IsNumeric(XML_ClassamentoxParticellaPubblico.HasAttribute("reddito_agrario")) Then
                                        Reddito_Agrario = CDbl(XML_ClassamentoxParticellaPubblico.GetAttribute("reddito_agrario"))
                                    End If
                                    If XML_ClassamentoxParticellaPubblico.HasAttribute("deduzione") = True Then
                                        Deduzione = XML_ClassamentoxParticellaPubblico.GetAttribute("deduzione")
                                    End If

                                    Str_Classamento += xmlAnagHlp.XML_ParticellaxClassamento(CStr(XML_ClassamentoxParticellaPubblico.GetAttribute("tipo_operazione")),
                                                                                            CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_provincia")),
                                                                                            CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_comune")),
                                                                                            LCase(CStr(XML_ParticellaPubblico.GetAttribute("sezione"))),
                                                                                            CInt(XML_ParticellaPubblico.GetAttribute("foglio")),
                                                                                            CInt(XML_ParticellaPubblico.GetAttribute("numero")),
                                                                                            LCase(CStr(XML_ParticellaPubblico.GetAttribute("subalterno"))),
                                                                                            CInt(XML_ClassamentoxParticellaPubblico.GetAttribute("qualita_cod")),
                                                                                            Porzione,
                                                                                            Classe,
                                                                                            Sup_Classe,
                                                                                            Reddito_Dominicale,
                                                                                            Reddito_Agrario,
                                                                                            Deduzione,
                                                                                                #1/1/1900#,
                                                                                                #12/31/2100#,
                                                                                                BaseCode,
                                                                                                TopCode)

                                Next

                            End If


                            Str_Eleggibilita = ""

                            Dim Sup_Eleggibilita As Double = 0

                            XMLs_EleggibilitaxParticellaPubblico = XML_ParticellaPubblico.GetElementsByTagName("Eleggibilita")

                            If Not XMLs_EleggibilitaxParticellaPubblico Is Nothing Then

                                For i_4 = 0 To XMLs_EleggibilitaxParticellaPubblico.Count - 1

                                    XML_EleggibilitaxParticellaPubblico = XMLs_EleggibilitaxParticellaPubblico(i_4)

                                    If XML_EleggibilitaxParticellaPubblico.HasAttribute("superficie") = True AndAlso IsNumeric(XML_EleggibilitaxParticellaPubblico.HasAttribute("superficie")) Then
                                        Sup_Eleggibilita = CDbl(XML_EleggibilitaxParticellaPubblico.GetAttribute("superficie"))
                                    End If

                                    Str_Eleggibilita += xmlAnagHlp.XML_ParticellaxEleggibilita(CStr(XML_EleggibilitaxParticellaPubblico.GetAttribute("tipo_operazione")),
                                                                                            CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_provincia")),
                                                                                            CStr(XML_ParticellaPubblico.GetAttribute("p_codice_istat_comune")),
                                                                                            LCase(CStr(XML_ParticellaPubblico.GetAttribute("sezione"))),
                                                                                            CInt(XML_ParticellaPubblico.GetAttribute("foglio")),
                                                                                            CInt(XML_ParticellaPubblico.GetAttribute("numero")),
                                                                                            LCase(CStr(XML_ParticellaPubblico.GetAttribute("subalterno"))),
                                                                                            CInt(XML_EleggibilitaxParticellaPubblico.GetAttribute("codice_eleggibilita")),
                                                                                            Sup_Eleggibilita,
                                                                                                #1/1/1900#,
                                                                                                #12/31/2100#,
                                                                                                BaseCode,
                                                                                                TopCode)

                                Next

                            End If

                            XML_ParticellaPrivato.InnerXml &= Str_Zona & Str_Possesso & Str_Classamento & Str_Eleggibilita

                            pippo = XML_ParticellaPrivato.OuterXml

                            'importo il nodo CentroAziendale nel documento del tag DATI-CENTRIAZIENDALI
                            XML_Nodo = XML_DatiParticelle.OwnerDocument.ImportNode(XML_ParticellaPrivato, True)

                            'appendo il centro aziendale al tag DatiCentriAziendali
                            XML_DatiParticelle.AppendChild(XML_Nodo)

                            pippo = XML_DatiParticelle.OuterXml
                            pippo = XML_CentroAziendalePrivato.OuterXml


                        Next    'CICLO SULLE PARTICELLE CATASTALI   

                        pippo = XML_DatiParticelle.OuterXml

                        pippo = XML_CentroAziendalePrivato.OuterXml

                        If Str_Particella <> "" Then
                            'XML_DatiParticelle.InnerXml = Str_Particella

                        End If



                        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                        '££££££££££££££££££££££                 FABBRICATO              ££££££££££££££££££££££££££££££££
                        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££


                        '------------------------------------------
                        '----- Tag Dati Praticelle Catastali
                        '------------------------------------------

                        'Leggo l'insieme dei nodi fabbricato
                        XMLs_FabbricatoPubblico = XML_CentroAziendalePubblico.GetElementsByTagName("Fabbricato")

                        XmlDocPrivato_2 = New System.Xml.XmlDocument

                        'creo il tag raccoglitore
                        XML_DatiFabbricati = XmlDocPrivato_2.CreateElement("DatiFabbricati")

                        'inizializzo la stringa
                        Str_Fabbricato = ""

                        For i_4 = 0 To XMLs_FabbricatoPubblico.Count - 1

                            'ciclo sui nodi fabbricato
                            XML_FabbricatoPubblico = XMLs_FabbricatoPubblico(i_4)

                            '------------------------------------------
                            '----- cerco i dati della particella del
                            '----- fabbricato nella matrice
                            '------------------------------------------

                            If CStr(XML_FabbricatoPubblico.GetAttribute("codice_particella")) <> "#" Then

                                Indice = Me.TrovaIndiceMatrice(CStr(XML_FabbricatoPubblico.GetAttribute("codice_particella")))

                                If Indice <> -1 Then

                                    Prov = Me.MatriceParticelle(Indice, 1)
                                    Com = Me.MatriceParticelle(Indice, 2)
                                    Sezione = Me.MatriceParticelle(Indice, 3)
                                    Foglio = Me.MatriceParticelle(Indice, 4)
                                    Numero = Me.MatriceParticelle(Indice, 5)
                                    Subalterno = Me.MatriceParticelle(Indice, 6)
                                Else
                                    Com = ""
                                    Prov = ""
                                    Sezione = ""
                                    Foglio = 0
                                    Numero = 0
                                    Subalterno = 0

                                End If

                            End If


                            Str_Fabbricato = xmlAnagHlp.XML_Fabbricato(CStr(XML_FabbricatoPubblico.GetAttribute("tipo_operazione")),
                                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                            CInt(XML_FabbricatoPubblico.GetAttribute("codice_fabbricato")),
                                                            Agro_If(XML_FabbricatoPubblico.GetAttribute("fabbricato_denominazione"), "Fabbricato n." & Right("00" & i_4 + 1, 2)),
                                                            CInt(0),
                                                            CInt(XML_FabbricatoPubblico.GetAttribute("tipo_fabbricato_codice")),
                                                            "000",
                                                            "000",
                                                            "0",
                                                            "0",
                                                            "0",
                                                            "0",
                                                            Agro_If(XML_FabbricatoPubblico.GetAttribute("volume_convenzionale"), CDbl(0)),
                                                            Agro_If(XML_FabbricatoPubblico.GetAttribute("volume_conversione"), CDbl(0)),
                                                            Agro_If(XML_FabbricatoPubblico.GetAttribute("volume_biologico"), CDbl(0)),
                                                            Agro_If(XML_FabbricatoPubblico.GetAttribute("codice_regolamento"), CInt(1)),
                                                            Agro_If(XML_FabbricatoPubblico.GetAttribute("titolo_possesso"), CInt(1)),
                                                            CDate(#1/1/1900#),
                                                            CDate(#12/31/2100#),
                                                            CInt(0),
                                                            CInt(0),
                                                            CInt(0),
                                                            CInt(0),
                                                            CInt(0),
                                                            CInt(0),
                                                            CInt(0),
                                                            CInt(0),
                                                            CInt(0),
                                                            CInt(0),
                                                            CInt(0),
                                                            CInt(0),
                                                            Agro_If(XML_FabbricatoPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                            Agro_If(XML_FabbricatoPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                            BaseCode,
                                                            TopCode)

                            'creo ogni ciclo una nuova istanza del documento3
                            'perchè ogni volta gli scrivo cose nuove
                            XmlDocPrivato_3 = New System.Xml.XmlDocument

                            XmlDocPrivato_3.LoadXml(Str_Fabbricato)

                            XML_FabbricatoPrivato = XmlDocPrivato_3.SelectSingleNode("Fabbricato")

                            XML_FabbricatoPrivato.InnerXml = xmlAnagHlp.XML_Indirizzo(CStr(XML_FabbricatoPubblico.GetAttribute("tipo_operazione")),
                                                                        CInt(1),
                                                                        CInt(0),
                                                                        Agro_If(XML_FabbricatoPubblico.GetAttribute("f_indirizzo"), ""),
                                                                        Agro_If(XML_FabbricatoPubblico.GetAttribute("f_frazione"), ""),
                                                                        Agro_If(XML_FabbricatoPubblico.GetAttribute("f_cap"), ""),
                                                                        Agro_If(XML_FabbricatoPubblico.GetAttribute("f_comune"), ""),
                                                                        Agro_If(XML_FabbricatoPubblico.GetAttribute("f_provincia"), ""),
                                                                        Agro_If(XML_FabbricatoPubblico.GetAttribute("f_stato"), "IT"),
                                                                        Agro_If(XML_FabbricatoPubblico.GetAttribute("f_note_indirizzo"), ""),
                                                                        Agro_If(XML_FabbricatoPubblico.GetAttribute("f_codice_istat_provincia"), ""),
                                                                        Agro_If(XML_FabbricatoPubblico.GetAttribute("f_codice_istat_comune"), ""),
                                                                        Agro_If(XML_FabbricatoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                        Agro_If(XML_FabbricatoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                        BaseCode,
                                                                        TopCode)



                            '------------------------------------------
                            '----- Tag Chiave Cliente
                            '------------------------------------------

                            TipoOp_Codice = CStr(XML_FabbricatoPubblico.GetAttribute("tipo_operazione"))

                            If TipoOp_Codice = enum_TipoOperazioneDB.Modifica Then
                                TipoOp_Codice = enum_TipoOperazioneDB.Scrittura
                            End If

                            If CStr(XML_FabbricatoPubblico.GetAttribute("f_chiave_cliente")) <> "#" Then
                                XML_FabbricatoPrivato.InnerXml += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                                            Id_Codice_ChiaveCliente,
                                                                            Agro_If(XML_FabbricatoPubblico.GetAttribute("f_chiave_cliente"), ""),
                                                                            Agro_If(XML_FabbricatoPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                                            Agro_If(XML_FabbricatoPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                                            BaseCode,
                                                                            TopCode,
                                                                            "Fabbricato")
                            End If


                            '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                            '££££££££££££££££££££££                 STALLA              ££££££££££££££££££££££££££££££££££££
                            '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

                            XMLs_StallaPubblico = XML_FabbricatoPubblico.GetElementsByTagName("Stalla")

                            If Not XMLs_StallaPubblico Is Nothing AndAlso XMLs_StallaPubblico.Count > 0 Then

                                XML_StallaPubblico = XMLs_StallaPubblico(0)

                                'CASO CODICI CLIENTE
                                If CStr(XML_StallaPubblico.GetAttribute("codice_genere_animale_gias")) = "#" And
                                   CStr(XML_StallaPubblico.GetAttribute("codice_specie_animale_gias")) = "#" And
                                   CStr(XML_StallaPubblico.GetAttribute("codice_indirizzo_produttivo_animale_gias")) = "#" Then

                                    Cod_Animale_Cliente = CStr(XML_StallaPubblico.GetAttribute("codice_specie_animale_cliente"))

                                    'Converti il codice animale del Cliente a GIAS
                                    'ConvertiAnimale(Cod_Animale_Cliente, _
                                    '                Gen_Cod, _
                                    '                Spe_Cod, _
                                    '                Ipro_Cod, _
                                    '                Nothing, _
                                    '                Nothing, _
                                    '                Connessione)

                                    objCACAnimali.GenSpeIproCatRazCodGias_from_CodCliente(Cod_Animale_Cliente,
                                                                                            Gen_Cod,
                                                                                            Spe_Cod,
                                                                                            Ipro_Cod,
                                                                                            Nothing,
                                                                                            Nothing,
                                                                                            objParametri_Server)

                                Else

                                    'CASO CODICI GIAS
                                    If CStr(XML_StallaPubblico.GetAttribute("codice_genere_animale_gias")) <> "#" And
                                        CStr(XML_StallaPubblico.GetAttribute("codice_specie_animale_gias")) <> "#" And
                                        CStr(XML_StallaPubblico.GetAttribute("codice_indirizzo_produttivo_animale_gias")) <> "#" Then

                                        Gen_Cod = XML_StallaPubblico.GetAttribute("codice_genere_animale_gias")
                                        Spe_Cod = XML_StallaPubblico.GetAttribute("codice_specie_animale_gias")
                                        Ipro_Cod = XML_StallaPubblico.GetAttribute("codice_indirizzo_produttivo_animale_gias")

                                    End If

                                End If


                                XML_FabbricatoPrivato.InnerXml += xmlAnagHlp.XML_Stalla(CStr(XML_StallaPubblico.GetAttribute("tipo_operazione")),
                                                                             CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                             CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                                             CInt(XML_FabbricatoPubblico.GetAttribute("codice_fabbricato")),
                                                                             Agro_If(XML_FabbricatoPubblico.GetAttribute("fabbricato_denominazione"), "Stalla n." & Right("00" & i_4 + 1, 2)),
                                                                             "0",
                                                                             Agro_If(XML_StallaPubblico.GetAttribute("data_costruzione"), #1/1/1900#),
                                                                             Agro_If(XML_StallaPubblico.GetAttribute("data_chiusura"), #12/31/2100#),
                                                                             "1.2.0",
                                                                             Gen_Cod,
                                                                             Spe_Cod,
                                                                             Ipro_Cod,
                                                                             "0",
                                                                             "0",
                                                                             Agro_If(XML_StallaPubblico.GetAttribute("latitudine"), 0),
                                                                             Agro_If(XML_StallaPubblico.GetAttribute("longitudine"), 0),
                                                                             #1/1/1900#,
                                                                             #12/31/2100#,
                                                                             BaseCode,
                                                                             TopCode)



                                '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                                '££££££££££££££££££££££                 ANIMALI              £££££££££££££££££££££££££££££££££££
                                '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

                                'StrTemp = XML_DatiImprese.OuterXml

                                XMLs_CapiAnimaliPubblico = XML_StallaPubblico.GetElementsByTagName("Capo_Animale")

                                If Not XMLs_CapiAnimaliPubblico Is Nothing AndAlso XMLs_CapiAnimaliPubblico.Count > 0 Then

                                    XML_DatiAgenda = XML_CentroAziendalePrivato.OwnerDocument.CreateElement("DatiAgenda")

                                    For i_5 = 0 To XMLs_CapiAnimaliPubblico.Count - 1

                                        XML_CapoAnimalePubblico = XMLs_CapiAnimaliPubblico(i_5)

                                        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                                        '££££££££££££££££££££                 AUMENTO CONSISTENZA              £££££££££££££££££££££££££
                                        '££££££££££££££££££££                 ANAGRAFE CAPO                    £££££££££££££££££££££££££
                                        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

                                        XML_AgendaPrivato = xmlContabHlp.XML_Agenda_Agenda(
                                            CStr(XML_CapoAnimalePubblico.GetAttribute("tipo_operazione")),
                                                                              CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                              CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                                              0,
                                                                              3001,
                                                                              "Aumento Consistenze Zootecniche",
                                                                              , , , ,
                                                                              Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                                                              Agro_If(XML_CentroAziendalePubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                                                              BaseCode,
                                                                              TopCode,
                                                                              XML_CentroAziendalePrivato.OwnerDocument)

                                        XML_DatiAgenda.AppendChild(XML_AgendaPrivato)


                                        XML_DatiMovimenti = XML_CentroAziendalePrivato.OwnerDocument.CreateElement("DatiMovimenti")

                                        XML_AgendaPrivato.AppendChild(XML_DatiMovimenti)

                                        '######################################################################################################
                                        '----- Movimento

                                        'Imposto il tag Movimento creato nella routine XML_Agenda_Movimento come figlio del tag XML_DatiMovimenti
                                        XML_Movimento = xmlContabHlp.XML_Agenda_Movimento(
                                                                        CStr(XML_CapoAnimalePubblico.GetAttribute("tipo_operazione")),
                                                                        CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                        CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                                        0, 0,
                                                                        "7300",
                                                                        "",
                                                                        ,
                                                                        ,
                                                                        , ,
                                                                        , ,
                                                                        , , , , , , , , , , , , , , , , , , , , , ,
                                                                        , , ,
                                                                        BaseCode, TopCode,
                                                                        XML_CentroAziendalePrivato.OwnerDocument)

                                        XML_DatiMovimenti.AppendChild(XML_Movimento)


                                        '######################################################################################################
                                        '----- DatiMovimenti_Dettagli

                                        'Creo un tag volante di nome dati movimenti
                                        XML_DatiMovimentiDettagli = XML_CentroAziendalePrivato.OwnerDocument.CreateElement("DatiMovimenti_Dettagli")

                                        'Imposto il tag volante come figlio di Movimento
                                        XML_Movimento.AppendChild(XML_DatiMovimentiDettagli)

                                        '######################################################################################################
                                        '----- Movimento_Dettaglio

                                        XML_MovimentoDettaglio = xmlContabHlp.XML_Agenda_MovimentoDettaglio(CStr(XML_CapoAnimalePubblico.GetAttribute("tipo_operazione")),
                                                                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                                                            0, 0, 0, ,
                                                                                            300,
                                                                                            , , , , , ,
                                                                                            38, ,
                                                                                            CDbl(XML_CapoAnimalePubblico.GetAttribute("numero")),
                                                                                            , , , , , , , ,
                                                                                            , , , , , , , , , , , ,
                                                                                            BaseCode,
                                                                                            TopCode,
                                                                                            XML_CentroAziendalePrivato.OwnerDocument,
                                                                                            , ,
                                                                                            "7300")

                                        XML_DatiMovimentiDettagli.AppendChild(XML_MovimentoDettaglio)


                                        '######################################################################################################
                                        '----- Movimento_Destinazione

                                        '--------------------------------
                                        'IMPORTANTE!!!!!!!!!!!!!!!!!!!!!!
                                        'In Id_Destinazione metto il codice del fabbricato (f_chiave_cliente)
                                        'in cui devo caricare gli animali........
                                        'il COMPONENTE utilizzerà questa chiave x capire (leggendo in fabbricati_codici)
                                        'in quale fabbricato (il fabbricato_cod) vanno caricati
                                        '--------------------------------

                                        XML_MovimentoDestinazione = xmlContabHlp.XML_Agenda_MovimentoDestinazione(CStr(XML_CapoAnimalePubblico.GetAttribute("tipo_operazione")),
                                                                                                    CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                                                    CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                                                                    0, 0, 0, 0,
                                                                                                    CInt(XML_FabbricatoPubblico.GetAttribute("f_chiave_cliente")),
                                                                                                    15,
                                                                                                    CDbl(XML_CapoAnimalePubblico.GetAttribute("numero")),
                                                                                                    , , , , ,
                                                                                                    BaseCode,
                                                                                                    TopCode,
                                                                                                    XML_CentroAziendalePrivato.OwnerDocument)

                                        XML_MovimentoDettaglio.AppendChild(XML_MovimentoDestinazione)

                                        '######################################################################################################
                                        '----- DatiZoo_Animali_Anagrafe

                                        XML_DatiZoo_Animale_Anagrafe = XML_CentroAziendalePrivato.OwnerDocument.CreateElement("DatiZoo_Animali_Anagrafe")

                                        XML_MovimentoDettaglio.AppendChild(XML_DatiZoo_Animale_Anagrafe)

                                        '######################################################################################################
                                        '----- Zoo_Animale_Anagrafe

                                        XML_Zoo_Animale_Anagrafe = xmlAnagHlp.XML_Zoo_Animale(CStr(XML_CapoAnimalePubblico.GetAttribute("tipo_operazione")),
                                                                                    CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                                    CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                                                    CInt(XML_CapoAnimalePubblico.GetAttribute("codice_capo")),
                                                                                    Agro_If(XML_CapoAnimalePubblico.GetAttribute("matricola"), ""),
                                                                                    Gen_Cod,
                                                                                    Spe_Cod,
                                                                                    Agro_If(XML_CapoAnimalePubblico.GetAttribute("codice_indirizzo_produttivo_animale_gias"), 0),
                                                                                    Agro_If(XML_CapoAnimalePubblico.GetAttribute("codice_razza_animale_gias"), 0),
                                                                                    Agro_If(XML_CapoAnimalePubblico.GetAttribute("codice_categoria_animale_gias"), 0),
                                                                                    Agro_If(XML_CapoAnimalePubblico.GetAttribute("nome"), ""),
                                                                                    Agro_If(XML_CapoAnimalePubblico.GetAttribute("collare"), ""),
                                                                                    "",
                                                                                    "",
                                                                                    Agro_If(XML_CapoAnimalePubblico.GetAttribute("data_nascita"), #1/1/1900#),
                                                                                    Agro_If(XML_CapoAnimalePubblico.GetAttribute("data_nascita"), #1/1/1900#),
                                                                                    #12/31/2100#,
                                                                                    "000",
                                                                                    "000",
                                                                                    "",
                                                                                    "",
                                                                                    Agro_If(XML_CapoAnimalePubblico.GetAttribute("sesso"), ""),
                                                                                    Agro_If(XML_CapoAnimalePubblico.GetAttribute("matricola_padre"), ""),
                                                                                    Agro_If(XML_CapoAnimalePubblico.GetAttribute("matricola_madre"), ""),
                                                                                    "",
                                                                                    "",
                                                                                    0,
                                                                                    Agro_If(XML_CapoAnimalePubblico.GetAttribute("peso"), 0),
                                                                                    #1/1/1900#,
                                                                                    Agro_If(XML_CapoAnimalePubblico.GetAttribute("metodo_produzione"), 1),
                                                                                    0,
                                                                                    #1/1/1900#,
                                                                                    #1/1/1900#,
                                                                                    BaseCode,
                                                                                    TopCode,
                                                                                    XML_CentroAziendalePrivato.OwnerDocument)

                                        XML_DatiZoo_Animale_Anagrafe.AppendChild(XML_Zoo_Animale_Anagrafe)

                                        '----------------

                                    Next

                                    StrTemp = XML_DatiAgenda.OuterXml

                                    XML_CentroAziendalePrivato.AppendChild(XML_DatiAgenda)

                                End If


                            End If

                            'importo IL FABBRICATO nel documento di DATI FABBRICATI
                            XML_Nodo = XML_DatiFabbricati.OwnerDocument.ImportNode(XML_FabbricatoPrivato, True)

                            'APPENDO IL FABBRICATO A DATI FABBRICATI
                            XML_DatiFabbricati.AppendChild(XML_Nodo)

                            XmlDocPrivato_3 = Nothing

                        Next    'CICLO SUI FABBRICATI   


                        If Str_Fabbricato <> "" Then

                            'importo DATI-FABBRICATI nel documento del centro aziendale
                            XML_Nodo = XML_CentroAziendalePrivato.OwnerDocument.ImportNode(XML_DatiFabbricati, True)

                            'appendo DatiFabbricati al centro aziendale
                            XML_CentroAziendalePrivato.AppendChild(XML_Nodo)

                        End If


                        XmlDocPrivato_2 = Nothing

                        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                        '££££££££££££££££££££££                 CAMPO                   ££££££££££££££££££££££££££££££££
                        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

                        XmlDocPrivato_2 = New System.Xml.XmlDocument

                        'creo il tag raccoglitore
                        XML_DatiCampi = XmlDocPrivato_2.CreateElement("DatiCampi")

                        'Leggo l'insieme dei nodi campo
                        XMLs_CampoPubblico = XML_CentroAziendalePubblico.GetElementsByTagName("Campo")

                        'Leggo l'insieme dei nodi appezzamento (sfusi)
                        XMLs_AppezzamentoPubblico = XML_CentroAziendalePubblico.GetElementsByTagName("Appezzamento")

                        Str_Campo = ""

                        nCampi = XMLs_CampoPubblico.Count

                        nAppezzamentiLiberi = XMLs_AppezzamentoPubblico.Count

                        If nCampi <> 0 Then

                            'creo i campi..
                            For i_5 = 0 To nCampi - 1

                                'ciclo sui nodi campo
                                'prendo sempre il primo perchè mano mano che li analizzo li cancello
                                XML_CampoPubblico = XMLs_CampoPubblico(i_5)

                                'controllo se il nodo campo è vuoto...
                                'se non lo è creo il campo, altrimenti procedo con l'appezzamento...
                                If Not XML_CampoPubblico.GetAttributeNode("campo_denominazione") Is Nothing Then

                                    Str_Campo = xmlAnagHlp.XML_Campo(CStr(XML_CampoPubblico.GetAttribute("tipo_operazione")),
                                                        CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                        CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                        CInt(XML_CampoPubblico.GetAttribute("codice_campo")),
                                                        Agro_If(XML_CampoPubblico.GetAttribute("campo_denominazione"), "Campo n." & Right("000" & i_5 + 1, 3)),
                                                        CInt(XML_CampoPubblico.GetAttribute("campo_tipo")),
                                                        CInt(Agro_If(XML_CampoPubblico.GetAttribute("codice_gruppo_vegetale_gias"), 0)),
                                                        CInt(Agro_If(XML_CampoPubblico.GetAttribute("codice_specie_gias"), 0)),
                                                        #1/1/1900#,
                                                        #12/31/2100#,
                                                        0, 0, 0, 0, 0,
                                                        Agro_If(XML_CampoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                        Agro_If(XML_CampoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                        BaseCode,
                                                        TopCode)

                                    'If CStr(XML_CampoPubblico.GetAttribute("cam_chiave_cliente")) <> "#" Then
                                    '    Str_CodiceCampo = xmlAnagHlp.XML_Codice(CStr(XML_CampoPubblico.GetAttribute("tipo_operazione")), _
                                    '                                Id_Codice_ChiaveCliente, _
                                    '                                Agro_If(XML_CampoPubblico.GetAttribute("cam_chiave_cliente"), ""), _
                                    '                                Agro_If(XML_CampoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
                                    '                                Agro_If(XML_CampoPubblico.GetAttribute("validita_fine"), #12/31/2100#), _
                                    '                                BaseCode, _
                                    '                                TopCode, "Campo")
                                    'End If

                                    'creo ogni ciclo una nuova istanza del documento3
                                    'perchè ogni volta gli scrivo cose nuove
                                    XmlDocPrivato_3 = New System.Xml.XmlDocument

                                    XmlDocPrivato_3.LoadXml(Str_Campo)

                                    XML_CampoPrivato = XmlDocPrivato_3.SelectSingleNode("Campo")

                                    'Leggo l'insieme dei nodi campi x particella
                                    XMLs_CampoxParticellaPubblico = XML_CampoPubblico.GetElementsByTagName("Campo_Particella")

                                    Str_CampixParticelle = ""

                                    For i_6 = 0 To XMLs_CampoxParticellaPubblico.Count - 1

                                        'ciclo sui nodi centro aziendale
                                        XML_CampoxParticellaPubblico = XMLs_CampoxParticellaPubblico(i_6)

                                        Prov = XML_CampoxParticellaPubblico.GetAttribute("istat_provincia")
                                        Com = XML_CampoxParticellaPubblico.GetAttribute("istat_comune")
                                        Sezione = LCase(CStr(XML_CampoxParticellaPubblico.GetAttribute("sezione")))
                                        Foglio = CStr(XML_CampoxParticellaPubblico.GetAttribute("foglio"))
                                        Numero = CStr(XML_CampoxParticellaPubblico.GetAttribute("numero"))
                                        Subalterno = LCase(CStr(XML_CampoxParticellaPubblico.GetAttribute("subalterno")))

                                        Str_CampixParticelle += xmlAnagHlp.XML_CampoParticella(CStr(XML_CampoxParticellaPubblico.GetAttribute("tipo_operazione")),
                                                                                    CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                                    CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                                                    CInt(XML_CampoPubblico.GetAttribute("codice_campo")),
                                                                                    Prov,
                                                                                    Com,
                                                                                    Sezione,
                                                                                    Foglio,
                                                                                    Numero,
                                                                                    Subalterno,
                                                                                    CDbl(XML_CampoxParticellaPubblico.GetAttribute("superficie")),
                                                                                    0, 0, 0, 0, 0, 0, 0, 0, 0,
                                                                                    Agro_If(XML_CampoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                                    Agro_If(XML_CampoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                                    BaseCode,
                                                                                    TopCode)



                                    Next        'CICLO SUI TAG CAMPOxPARTICELLA

                                    If Str_CampixParticelle <> "" Then

                                        XML_DatiCampixParticelle = XmlDocPrivato_3.CreateElement("DatiCampixParticelle")

                                        XML_CampoPrivato.AppendChild(XML_DatiCampixParticelle)

                                        XML_DatiCampixParticelle.InnerXml = Str_CampixParticelle

                                    End If

                                    'XML_CampoPrivato.InnerXml = XML_CampoPrivato.InnerXml & Str_CodiceCampo

                                    '----------------------------------------------------------------------------
                                    'Leggo l'insieme dei nodi appezzamento
                                    XMLs_AppezzamentoPubblico = XML_CampoPubblico.GetElementsByTagName("AppezzamentoCampo")

                                    'nuova istanza del documento
                                    XmlDocPrivato_4 = New System.Xml.XmlDocument

                                    'creo il tag raccoglitore
                                    XML_DatiAppezzamenti = XmlDocPrivato_4.CreateElement("DatiAppezzamenti")

                                    Str_Appezzamento = ""

                                    nAppezzamenti = XMLs_AppezzamentoPubblico.Count

                                    For i_7 = 0 To XMLs_AppezzamentoPubblico.Count - 1

                                        XML_AppezzamentoPubblico = XMLs_AppezzamentoPubblico(i_7)

                                        'Dim pippo1 As String = XML_AppezzamentoPubblico.OuterXml

                                        Dim app_nome As String
                                        app_nome = Agro_If(XML_AppezzamentoPubblico.GetAttribute("appezzamento_denominazione"), "Appezzamento n." & Right("000" & i_7 + 1, 3))

                                        Dim codice_alfanumerico As String = "#"
                                        If XML_AppezzamentoPubblico.HasAttribute("codice_alfanumerico") Then
                                            If Not IsNothing(XML_AppezzamentoPubblico.GetAttribute("codice_alfanumerico")) Then
                                                If CStr(XML_AppezzamentoPubblico.GetAttribute("codice_alfanumerico")) <> "#" Then
                                                    codice_alfanumerico = CStr(XML_AppezzamentoPubblico.GetAttribute("codice_alfanumerico"))
                                                End If
                                            End If
                                        End If

                                        Dim ubicazione As String = ""
                                        If XML_AppezzamentoPubblico.HasAttribute("ubicazione") Then
                                            If Not IsNothing(XML_AppezzamentoPubblico.GetAttribute("ubicazione")) Then
                                                If CStr(XML_AppezzamentoPubblico.GetAttribute("ubicazione")) <> "#" Then
                                                    ubicazione = CStr(XML_AppezzamentoPubblico.GetAttribute("ubicazione"))
                                                    ubicazione = ubicazione.Trim.ToLower
                                                    If ubicazione = "collina" Or ubicazione = "pianura" Or
                                                        ubicazione = "mezza costa" Or ubicazione = "montagna" Then
                                                        'ok
                                                    Else
                                                        ubicazione = ""
                                                    End If
                                                End If
                                            End If
                                        End If

                                        Dim Blk_Flag As Integer = 0
                                        If XML_AppezzamentoPubblico.HasAttribute("bloccato") Then
                                            If Not IsNothing(XML_AppezzamentoPubblico.GetAttribute("bloccato")) Then
                                                If IsNumeric(XML_AppezzamentoPubblico.GetAttribute("bloccato")) AndAlso CInt(XML_AppezzamentoPubblico.GetAttribute("bloccato")) = 1 Then
                                                    Blk_Flag = -1
                                                End If
                                            End If
                                        End If


                                        Str_Appezzamento = xmlAnagHlp.XML_Appezzamento(CStr(XML_AppezzamentoPubblico.GetAttribute("tipo_operazione")),
                                                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                                                            CDbl(XML_AppezzamentoPubblico.GetAttribute("sup_appezzamento")),
                                                                            Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                            "", 0, 0, 0, "",
                                                                            Agro_If(XML_AppezzamentoPubblico.GetAttribute("altitudine"), CDbl(0)),
                                                                            ubicazione, 0, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, "", "", 0, "", "",
                                                                             app_nome,
                                                                            0, 0, "",
                                                                            CInt(XML_CampoPubblico.GetAttribute("codice_campo")),
                                                                            0,
                                                                            Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                            Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                            Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                            Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                            BaseCode,
                                                                            TopCode,
                                                                            codice_alfanumerico,
                                                                            Blk_Flag)


                                        'creo ogni ciclo una nuova istanza del documento5
                                        'perchè ogni volta gli scrivo cose nuove
                                        XmlDocPrivato_5 = New System.Xml.XmlDocument

                                        XmlDocPrivato_5.LoadXml(Str_Appezzamento)

                                        XML_AppezzamentoPrivato = XmlDocPrivato_5.SelectSingleNode("Appezzamento")

                                        Str_AppezzamentoxParticelle = ""

                                        'Leggo l'insieme dei nodi campi x particella
                                        XMLs_AppezzamentoxParticellaPubblico = XML_AppezzamentoPubblico.GetElementsByTagName("Appezzamento_Particella")

                                        Str_AppezzamentoxParticelle = ""

                                        For i_8 = 0 To XMLs_AppezzamentoxParticellaPubblico.Count - 1

                                            'ciclo sui nodi centro aziendale
                                            XML_AppezzamentoxParticellaPubblico = XMLs_AppezzamentoxParticellaPubblico(i_8)

                                            Prov = XML_AppezzamentoxParticellaPubblico.GetAttribute("istat_provincia")
                                            Com = XML_AppezzamentoxParticellaPubblico.GetAttribute("istat_comune")
                                            Sezione = LCase(CStr(XML_AppezzamentoxParticellaPubblico.GetAttribute("sezione")))
                                            Foglio = CStr(XML_AppezzamentoxParticellaPubblico.GetAttribute("foglio"))
                                            Numero = CStr(XML_AppezzamentoxParticellaPubblico.GetAttribute("numero"))
                                            Subalterno = LCase(CStr(XML_AppezzamentoxParticellaPubblico.GetAttribute("subalterno")))

                                            Sup_AppxPart = XML_AppezzamentoxParticellaPubblico.GetAttribute("superficie")

                                            AgronicaCoreDataProvider.Conversioni.EttariAreCentiare_from_Ettari(CDbl(Sup_AppxPart), Ettari_AppxPart, Are_AppxPart, Centiare_AppxPart)

                                            Str_AppezzamentoxParticelle += xmlAnagHlp.XML_AppezzamentoParticella(CStr(XML_AppezzamentoxParticellaPubblico.GetAttribute("tipo_operazione")),
                                                                                        CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                                        CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                                                        CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                                                                        Prov,
                                                                                        Com,
                                                                                        Sezione,
                                                                                        Foglio,
                                                                                        Numero,
                                                                                        Subalterno,
                                                                                        CDbl(Sup_AppxPart),
                                                                                        CDbl(Ettari_AppxPart),
                                                                                        CInt(Are_AppxPart),
                                                                                        CInt(Centiare_AppxPart),
                                                                                        0, 0, 0, 0, 0, 0,
                                                                                        Agro_If(XML_AppezzamentoxParticellaPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                                        Agro_If(XML_AppezzamentoxParticellaPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                                        BaseCode,
                                                                                        TopCode)



                                        Next   'CICLO SUI TAG APPEZZAMENTOxPARTICELLA 

                                        Str_CodiceAppezzamento = xmlAnagHlp.XML_Codice(CStr(XML_AppezzamentoPubblico.GetAttribute("tipo_operazione")),
                                                                            Id_Codice_MetodoProduzione,
                                                                            Agro_If(XML_AppezzamentoPubblico.GetAttribute("metodo_produzione"), 1),
                                                                            Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                            Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                            BaseCode,
                                                                            TopCode, "Appezzamento")


                                        Str_CodiceAppezzamento += xmlAnagHlp.XML_Codice(CStr(XML_AppezzamentoPubblico.GetAttribute("tipo_operazione")),
                                                                            enum_CodiciAnagrafe.TitoloPossesso,
                                                                            Agro_If(XML_AppezzamentoPubblico.GetAttribute("titolo_possesso"), 1),
                                                                            Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                            Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                            BaseCode,
                                                                            TopCode, "Appezzamento")

                                        If CStr(XML_AppezzamentoPubblico.GetAttribute("codice_contratto")) <> "#" Then
                                            Str_CodiceAppezzamento += xmlAnagHlp.XML_Codice(CStr(XML_AppezzamentoPubblico.GetAttribute("tipo_operazione")),
                                                                                                    Id_Codice_Contratto,
                                                                                                    Agro_If(XML_AppezzamentoPubblico.GetAttribute("codice_contratto"), ""),
                                                                                                    Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                                                    Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                                                    BaseCode,
                                                                                                    TopCode, "Appezzamento")
                                        End If

                                        If CStr(XML_AppezzamentoPubblico.GetAttribute("numero_appezzamento_bio")) <> "#" Then
                                            Str_CodiceAppezzamento += xmlAnagHlp.XML_Codice(CStr(XML_AppezzamentoPubblico.GetAttribute("tipo_operazione")),
                                                                                Id_Codice_NumAppBio,
                                                                                Agro_If(XML_AppezzamentoPubblico.GetAttribute("numero_appezzamento_bio"), ""),
                                                                                Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                                Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                                BaseCode,
                                                                                TopCode, "Appezzamento")
                                        End If

                                        If CStr(XML_AppezzamentoPubblico.GetAttribute("app_chiave_cliente")) <> "#" Then
                                            Str_CodiceAppezzamento += xmlAnagHlp.XML_Codice(CStr(XML_AppezzamentoPubblico.GetAttribute("tipo_operazione")),
                                                                                Id_Codice_ChiaveCliente,
                                                                                Agro_If(XML_AppezzamentoPubblico.GetAttribute("app_chiave_cliente"), ""),
                                                                                Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                                Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                                BaseCode,
                                                                                TopCode, "Appezzamento")
                                        End If




                                        XML_AppezzamentoPrivato.InnerXml = Str_AppezzamentoxParticelle & Str_CodiceAppezzamento


                                        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                                        '££££££££££££££££££££££                 REG_IMPIANTO            ££££££££££££££££££££££££££££££££
                                        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

                                        'modalità dal campo

                                        XML_DatiRegImpianti = Gestione_XML_Impianto(HT_Piva_objCodici,
                                                                                        XML_ImpresaPubblico,
                                                                                        XML_CentroAziendalePubblico,
                                                                                        XML_AppezzamentoPubblico,
                                                                                        app_nome,
                                                                                        Str_Appezzamento,
                                                                                        objParametri_Server)


                                        '----------  DATI REG-IMPIANTI 

                                        'importo il tag DATI-REG_IMPIANTI nel documento del APPEZZAMENTO
                                        XML_Nodo = XML_AppezzamentoPrivato.OwnerDocument.ImportNode(XML_DatiRegImpianti, True)

                                        'appendo il tag DATI-REG_IMPIANTI ALL'APPEZZAMENTO
                                        XML_AppezzamentoPrivato.AppendChild(XML_Nodo)


                                        '----------  APPEZZAMENTO

                                        'importo l'APPEZZAMENTO IN DATI APPEZZAMENTI
                                        XML_Nodo = XML_DatiAppezzamenti.OwnerDocument.ImportNode(XML_AppezzamentoPrivato, True)

                                        'appendo l'APPEZZAMENTO A DATI-APPEZZAMENTO
                                        XML_DatiAppezzamenti.AppendChild(XML_Nodo)

                                        StrTemp = XML_DatiAppezzamenti.OuterXml


                                    Next        'CICLO SUI TAG APPEZZAMENTO

                                    'DATI APPEZZAMENTI

                                    'importo DATI-APPEZZAMENTI nel documento del CAMPO
                                    XML_Nodo = XML_CampoPrivato.OwnerDocument.ImportNode(XML_DatiAppezzamenti, True)

                                    'appendo DATI APPEZZAMENTI AL CAMPO
                                    XML_CampoPrivato.AppendChild(XML_Nodo)


                                    StrTemp = XML_CampoPrivato.OuterXml

                                    'importo IL CAMPO nel documento di DATI-CAMPI
                                    XML_Nodo = XML_DatiCampi.OwnerDocument.ImportNode(XML_CampoPrivato, True)

                                    'appendo IL CAMPO A DATI-CAMPO
                                    XML_DatiCampi.AppendChild(XML_Nodo)

                                    'pippo = XML_DatiCampi.OuterXml


                                Else

                                    'se il nodo campo è vuoto ne genero uno vuoto anche x l'xml privato
                                    XmlDocPrivato_3 = New System.Xml.XmlDocument
                                    XML_CampoPrivato = XmlDocPrivato_3.CreateElement("Campo")

                                End If


                            Next 'CICLO SUI CAMPI


                            'importo il tag DATI-CAMPI nel documento del CENTRO AZIENDALE
                            XML_Nodo = XML_CentroAziendalePrivato.OwnerDocument.ImportNode(XML_DatiCampi, True)

                            'appendo il tag DATI-CAMPI al  CENTRO AZIENDALE
                            XML_CentroAziendalePrivato.AppendChild(XML_Nodo)


                        End If 'esistono campi

                        XmlDocPrivato_3 = Nothing

                        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                        '££££££££££££££££££££££                 APPEZZAMENTO            ££££££££££££££££££££££££££££££££
                        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

                        'un giorno li gestiremo
                        'Dim blk_flag As Boolean
                        'Dim blk_inizio_data As Date = AGRODATAINIZIO
                        'Dim blk_inizio_username As String = ""
                        'Dim blk_inizio_note As String = ""
                        'Dim blk_fine_data As Date = AGRODATAFINE
                        'Dim blk_fine_username As String = ""
                        'Dim blk_fine_note As String = ""

                        'nuova istanza del documento
                        XmlDocPrivato_3 = New System.Xml.XmlDocument

                        'creo il tag raccoglitore
                        XML_DatiAppezzamenti = XmlDocPrivato_3.CreateElement("DatiAppezzamenti")

                        XMLs_AppezzamentoPubblico = XML_CentroAziendalePubblico.GetElementsByTagName("Appezzamento")

                        Str_Appezzamento = ""

                        nAppezzamenti = XMLs_AppezzamentoPubblico.Count

                        If nAppezzamenti > 0 Then

                            For i_7 = 0 To XMLs_AppezzamentoPubblico.Count - 1

                                XML_AppezzamentoPubblico = XMLs_AppezzamentoPubblico(i_7)

                                Dim app_nome As String
                                app_nome = Agro_If(XML_AppezzamentoPubblico.GetAttribute("appezzamento_denominazione"), "Appezzamento n." & Right("000" & i_7 + 1, 3))

                                Dim codice_alfanumerico As String = "#"
                                If XML_AppezzamentoPubblico.HasAttribute("codice_alfanumerico") Then
                                    If Not IsNothing(XML_AppezzamentoPubblico.GetAttribute("codice_alfanumerico")) Then
                                        If CStr(XML_AppezzamentoPubblico.GetAttribute("codice_alfanumerico")) <> "#" Then
                                            codice_alfanumerico = CStr(XML_AppezzamentoPubblico.GetAttribute("codice_alfanumerico"))
                                        End If
                                    End If
                                End If

                                Dim Blk_Flag As Integer = 0
                                If XML_AppezzamentoPubblico.HasAttribute("bloccato") Then
                                    If Not IsNothing(XML_AppezzamentoPubblico.GetAttribute("bloccato")) Then
                                        If IsNumeric(XML_AppezzamentoPubblico.GetAttribute("bloccato")) AndAlso CInt(XML_AppezzamentoPubblico.GetAttribute("bloccato")) = 1 Then
                                            Blk_Flag = -1
                                        End If
                                    End If
                                End If

                                Str_Appezzamento = xmlAnagHlp.XML_Appezzamento(CStr(XML_AppezzamentoPubblico.GetAttribute("tipo_operazione")),
                                                                    CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                    CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                                    CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                                                    CDbl(XML_AppezzamentoPubblico.GetAttribute("sup_appezzamento")),
                                                                    Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                    "", 0, 0, 0, "",
                                                                    Agro_If(XML_AppezzamentoPubblico.GetAttribute("altitudine"), CDbl(0)),
                                                                    "", 0, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, "", "", 0, "", "",
                                                                    app_nome,
                                                                    0, 0, "", 0, 0,
                                                                    Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                    Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                    Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                    Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                    BaseCode,
                                                                    TopCode,
                                                                    codice_alfanumerico,
                                                                    Blk_Flag)

                                ' IIf(CStr(XML_AppezzamentoPubblico.GetAttribute("altitudine")) <> "#", CDbl(XML_AppezzamentoPubblico.GetAttribute("altitudine")), CDbl(0)), _

                                'creo ogni ciclo una nuova istanza del documento3
                                'perchè ogni volta gli scrivo cose nuove
                                XmlDocPrivato_4 = New System.Xml.XmlDocument

                                XmlDocPrivato_4.LoadXml(Str_Appezzamento)

                                XML_AppezzamentoPrivato = XmlDocPrivato_4.SelectSingleNode("Appezzamento")

                                Str_AppezzamentoxParticelle = ""

                                'Leggo l'insieme dei nodi campi x particella
                                XMLs_AppezzamentoxParticellaPubblico = XML_AppezzamentoPubblico.GetElementsByTagName("Appezzamento_Particella")

                                Str_AppezzamentoxParticelle = ""

                                For i_8 = 0 To XMLs_AppezzamentoxParticellaPubblico.Count - 1

                                    'ciclo sui nodi centro aziendale
                                    XML_AppezzamentoxParticellaPubblico = XMLs_AppezzamentoxParticellaPubblico(i_8)

                                    Prov = XML_AppezzamentoxParticellaPubblico.GetAttribute("istat_provincia")
                                    Com = XML_AppezzamentoxParticellaPubblico.GetAttribute("istat_comune")
                                    Sezione = LCase(CStr(XML_AppezzamentoxParticellaPubblico.GetAttribute("sezione")))
                                    Foglio = CStr(XML_AppezzamentoxParticellaPubblico.GetAttribute("foglio"))
                                    Numero = CStr(XML_AppezzamentoxParticellaPubblico.GetAttribute("numero"))
                                    Subalterno = LCase(CStr(XML_AppezzamentoxParticellaPubblico.GetAttribute("subalterno")))

                                    Sup_AppxPart = XML_AppezzamentoxParticellaPubblico.GetAttribute("superficie")

                                    AgronicaCoreDataProvider.Conversioni.EttariAreCentiare_from_Ettari(CDbl(Sup_AppxPart), Ettari_AppxPart, Are_AppxPart, Centiare_AppxPart)

                                    Str_AppezzamentoxParticelle += xmlAnagHlp.XML_AppezzamentoParticella(CStr(XML_AppezzamentoxParticellaPubblico.GetAttribute("tipo_operazione")),
                                                                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                                                                            Prov,
                                                                                            Com,
                                                                                            Sezione,
                                                                                            Foglio,
                                                                                            Numero,
                                                                                            Subalterno,
                                                                                            CDbl(Sup_AppxPart),
                                                                                            CDbl(Ettari_AppxPart),
                                                                                            CInt(Are_AppxPart),
                                                                                            CInt(Centiare_AppxPart),
                                                                                            0, 0, 0, 0, 0, 0,
                                                                                            Agro_If(XML_AppezzamentoxParticellaPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                                            Agro_If(XML_AppezzamentoxParticellaPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                                            BaseCode,
                                                                                            TopCode)



                                Next   'CICLO SUI TAG APPEZZAMENTOxPARTICELLA 

                                Str_CodiceAppezzamento = xmlAnagHlp.XML_Codice(CStr(XML_AppezzamentoPubblico.GetAttribute("tipo_operazione")),
                                                                    Id_Codice_MetodoProduzione,
                                                                    Agro_If(XML_AppezzamentoPubblico.GetAttribute("metodo_produzione"), 1),
                                                                    Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                    Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                    BaseCode,
                                                                    TopCode, "Appezzamento")


                                Str_CodiceAppezzamento += xmlAnagHlp.XML_Codice(CStr(XML_AppezzamentoPubblico.GetAttribute("tipo_operazione")),
                                                                    enum_CodiciAnagrafe.TitoloPossesso,
                                                                    Agro_If(XML_AppezzamentoPubblico.GetAttribute("titolo_possesso"), 1),
                                                                    Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                    Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                    BaseCode,
                                                                    TopCode, "Appezzamento")

                                If CStr(XML_AppezzamentoPubblico.GetAttribute("codice_contratto")) <> "#" Then
                                    Str_CodiceAppezzamento += xmlAnagHlp.XML_Codice(CStr(XML_AppezzamentoPubblico.GetAttribute("tipo_operazione")),
                                                                                            Id_Codice_Contratto,
                                                                                            Agro_If(XML_AppezzamentoPubblico.GetAttribute("codice_contratto"), ""),
                                                                                            Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                                            Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                                            BaseCode,
                                                                                            TopCode, "Appezzamento")
                                End If

                                If CStr(XML_AppezzamentoPubblico.GetAttribute("numero_appezzamento_bio")) <> "#" Then
                                    Str_CodiceAppezzamento += xmlAnagHlp.XML_Codice(CStr(XML_AppezzamentoPubblico.GetAttribute("tipo_operazione")),
                                                                        Id_Codice_NumAppBio,
                                                                        Agro_If(XML_AppezzamentoPubblico.GetAttribute("numero_appezzamento_bio"), ""),
                                                                        Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                        Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                        BaseCode,
                                                                        TopCode, "Appezzamento")
                                End If

                                If CStr(XML_AppezzamentoPubblico.GetAttribute("app_chiave_cliente")) <> "#" Then
                                    Str_CodiceAppezzamento += xmlAnagHlp.XML_Codice(CStr(XML_AppezzamentoPubblico.GetAttribute("tipo_operazione")),
                                                                        Id_Codice_ChiaveCliente,
                                                                        Agro_If(XML_AppezzamentoPubblico.GetAttribute("app_chiave_cliente"), ""),
                                                                        Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                                        Agro_If(XML_AppezzamentoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                                        BaseCode,
                                                                        TopCode, "Appezzamento")
                                End If

                                XML_AppezzamentoPrivato.InnerXml = Str_AppezzamentoxParticelle & Str_CodiceAppezzamento


                                ''£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                                ''££££££££££££££££££££££                 REG_IMPIANTO            ££££££££££££££££££££££££££££££££
                                ''£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

                                'modalità appezzamento senza campo

                                XML_DatiRegImpianti = Gestione_XML_Impianto(HT_Piva_objCodici,
                                                                                 XML_ImpresaPubblico,
                                                                                XML_CentroAziendalePubblico,
                                                                                XML_AppezzamentoPubblico,
                                                                                app_nome,
                                                                                Str_Appezzamento,
                                                                                objParametri_Server)

                                'XmlDocPrivato_4 = New System.Xml.XmlDocument

                                ''creo il tag raccoglitore
                                'XML_DatiRegImpianti = XmlDocPrivato_4.CreateElement("DatiReg_Impianti")

                                ''Leggo l'insieme dei nodi particella
                                'XMLs_RegImpiantoPubblico = XML_AppezzamentoPubblico.GetElementsByTagName("Impianto")

                                'Str_Appezzamento = ""

                                'For i_9 = 0 To XMLs_RegImpiantoPubblico.Count - 1

                                '    XML_RegImpiantoPubblico = XMLs_RegImpiantoPubblico(i_9)

                                '    Grfi_Cod = 0

                                '    'CASO CODICI CLIENTE
                                '    If CStr(XML_RegImpiantoPubblico.GetAttribute("codice_specie_gias")) = "#" And _
                                '    CStr(XML_RegImpiantoPubblico.GetAttribute("codice_varieta_gias")) = "#" Then

                                '        If CStr(XML_RegImpiantoPubblico.GetAttribute("codice_varieta_cliente")) <> "#" Then

                                '            'Converti il codice varietà del Cliente a GIAS
                                '            'ConvertiVarieta(XML_RegImpiantoPubblico.GetAttribute("codice_specie_cliente"), _
                                '            '                XML_RegImpiantoPubblico.GetAttribute("codice_varieta_cliente"), _
                                '            '                Cul_Cod, _
                                '            '                Veg_Cod, _
                                '            '                Connessione)

                                '            objCACCultivar.VegCulCodGias_from_CulCodCliente( _
                                '                                                XML_RegImpiantoPubblico.GetAttribute("codice_varieta_cliente"), _
                                '                                                Cul_Cod, _
                                '                                                Veg_Cod, _
                                '                                                objParametri_Server)

                                '            If Veg_Cod <> 0 Then

                                '                If Cul_Cod = 0 Then
                                '                    Cul_Cod = objCultivar.VarietaAltre(Veg_Cod, objParametri_Server)
                                '                End If

                                '            Else

                                '                If CStr(XML_RegImpiantoPubblico.GetAttribute("codice_specie_cliente")) <> "#" Then

                                '                    'Veg_Cod = ConvertiSpecie(XML_RegImpiantoPubblico.GetAttribute("codice_specie_cliente"), _
                                '                    '                        Connessione)

                                '                    Veg_Cod = objCACVegcod.VegCodGias_from_VegCodCliente(XML_RegImpiantoPubblico.GetAttribute("codice_specie_cliente"), _
                                '                                                                            objParametri_Server)

                                '                    If Veg_Cod <> 0 Then
                                '                        Cul_Cod = objCultivar.VarietaAltre(Veg_Cod, objParametri_Server)
                                '                    Else
                                '                        Cul_Cod = 0
                                '                    End If

                                '                Else
                                '                    Cul_Cod = 0
                                '                End If

                                '            End If

                                '        ElseIf CStr(XML_RegImpiantoPubblico.GetAttribute("codice_specie_cliente")) <> "#" Then

                                '            'Veg_Cod = ConvertiSpecie(XML_RegImpiantoPubblico.GetAttribute("codice_specie_cliente"), _
                                '            '                        Connessione)

                                '            Veg_Cod = objCACVegcod.VegCodGias_from_VegCodCliente(XML_RegImpiantoPubblico.GetAttribute("codice_specie_cliente"), _
                                '                                                                         objParametri_Server)

                                '            If Veg_Cod <> 0 Then
                                '                Cul_Cod = objCultivar.VarietaAltre(Veg_Cod, objParametri_Server)
                                '            Else
                                '                Cul_Cod = 0
                                '            End If

                                '        End If

                                '    Else

                                '        'CASO CODICI GIAS
                                '        If CStr(XML_RegImpiantoPubblico.GetAttribute("codice_specie_gias")) <> "#" And _
                                '        CStr(XML_RegImpiantoPubblico.GetAttribute("codice_varieta_gias")) <> "#" Then

                                '            Cul_Cod = XML_RegImpiantoPubblico.GetAttribute("codice_varieta_gias")
                                '            Veg_Cod = XML_RegImpiantoPubblico.GetAttribute("codice_specie_gias")

                                '        ElseIf CStr(XML_RegImpiantoPubblico.GetAttribute("codice_specie_gias")) <> "#" Then

                                '            Veg_Cod = XML_RegImpiantoPubblico.GetAttribute("codice_specie_gias")
                                '            Cul_Cod = objCultivar.VarietaAltre(Veg_Cod, objParametri_Server)
                                '        Else
                                '            Cul_Cod = XML_RegImpiantoPubblico.GetAttribute("codice_varieta_gias")
                                '            'Veg_Cod = VegCod_from_CulCod(Veg_Cod, Connessione)
                                '            Veg_Cod = objCultivar.VegCod_from_CulCod(Cul_Cod, objParametri_Server)
                                '        End If

                                '        If Veg_Cod <> 0 Then
                                '            If CStr(XML_RegImpiantoPubblico.GetAttribute("codice_finalita_gias")) <> "#" Then
                                '                Grfi_Cod = XML_RegImpiantoPubblico.GetAttribute("codice_finalita_gias")
                                '            End If
                                '        End If


                                '    End If

                                '    AppezzaIdReg = "0"
                                '    Appezza = "0"
                                '    Id_Reg = "0"

                                '    AppezzaIdReg = XML_RegImpiantoPubblico.GetAttribute("codice_impianto")

                                '    If AppezzaIdReg <> "0" Then
                                '        ArrayAppezzaIdReg = Split(AppezzaIdReg, "/")
                                '        Id_Reg = CInt(ArrayAppezzaIdReg(1))
                                '    Else
                                '        Id_Reg = "0"
                                '    End If


                                '    XML_RegImpiantoPrivato = XML_Impianto(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")), _
                                '                                        CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")), _
                                '                                        CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")), _
                                '                                        CInt(0), _
                                '                                        CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")), _
                                '                                        CInt(Id_Reg), _
                                '                                        CInt(0), _
                                '                                        CInt(0), _
                                '                                        CInt(0), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
                                '                                        Cul_Cod, _
                                '                                        "", _
                                '                                         0, _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("resa_effettiva"), CDbl(0)), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("scarto"), CDbl(0)), _
                                '                                        0, "0", "", "", "", _
                                '                                        0, _
                                '                                        0, _
                                '                                         0, _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("semina_trapianto"), ""), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("codice_portinnesto"), CInt(0)), _
                                '                                        0, 0, 0, 0, 0, 0, "", 0, 0, 0, 0, 0, "", "", _
                                '                                        CInt(Grfi_Cod), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("codice_impianto_irrigazione"), ""), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("codice_regolamento"), CInt(1)), _
                                '                                        CInt(0), _
                                '                                        -1, _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("codice_copertura"), CInt(-1)), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("codice_forma_allevamento"), CInt(-1)), _
                                '                                        -1, 0, _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("sup_imp"), CDbl(0)), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("codice_tipologia_varieta_gias"), CDbl(0)), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#), _
                                '                                        BaseCode, _
                                '                                        TopCode)

                                '    'CODICI IMPIANTO
                                '    Str_CodiceImpianto = ""

                                '    If CStr(XML_RegImpiantoPubblico.GetAttribute("titolo_possesso")) <> "#" Then
                                '        Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")), _
                                '                                        Id_Codice_TitoloPossesso, _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("titolo_possesso"), 1), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#), _
                                '                                        BaseCode, _
                                '                                        TopCode, "Impianto")
                                '    End If

                                '    If CStr(XML_RegImpiantoPubblico.GetAttribute("distanza_tra_fila")) <> "#" Then
                                '        Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")), _
                                '                                        Id_Codice_TraFila, _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("distanza_tra_fila"), 0), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#), _
                                '                                        BaseCode, _
                                '                                        TopCode, "Impianto")
                                '    End If

                                '    If CStr(XML_RegImpiantoPubblico.GetAttribute("distanza_su_fila")) <> "#" Then
                                '        Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")), _
                                '                                        Id_Codice_SuFila, _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("distanza_su_fila"), 0), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#), _
                                '                                        BaseCode, _
                                '                                        TopCode, "Impianto")
                                '    End If
                                '    If CStr(XML_RegImpiantoPubblico.GetAttribute("interbina")) <> "#" Then
                                '        Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")), _
                                '                                    Id_Codice_Interbina, _
                                '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("interbina"), 0), _
                                '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
                                '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#), _
                                '                                    BaseCode, _
                                '                                    TopCode, "Impianto")
                                '    End If

                                '    If CStr(XML_RegImpiantoPubblico.GetAttribute("piano_semina")) <> "#" Then
                                '        Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")), _
                                '                                        Id_Codice_PianoSemina, _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("piano_semina"), ""), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#), _
                                '                                        BaseCode, _
                                '                                        TopCode, "Impianto")
                                '    End If

                                '    'no, è un codice della distinta!
                                '    'If CStr(XML_RegImpiantoPubblico.GetAttribute("cooperativa_referente")) <> "#" Then
                                '    '    Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")), _
                                '    '                                    Id_Codice_Cooperativa, _
                                '    '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("cooperativa_referente"), ""), _
                                '    '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
                                '    '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#), _
                                '    '                                    BaseCode, _
                                '    '                                    TopCode, "Impianto")

                                '    'End If

                                '    'no, è un codice della distinta!
                                '    'If CStr(XML_RegImpiantoPubblico.GetAttribute("capitolato_privato")) <> "#" Then
                                '    '    Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")), _
                                '    '                                    Id_Capitolato_Privato, _
                                '    '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("capitolato_privato"), ""), _
                                '    '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
                                '    '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#), _
                                '    '                                    BaseCode, _
                                '    '                                    TopCode, "Impianto")

                                '    'End If

                                '    If CStr(XML_RegImpiantoPubblico.GetAttribute("codice_raggruppamento_varieta_gias")) <> "#" Then
                                '        Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")), _
                                '                                        CInt(XML_RegImpiantoPubblico.GetAttribute("codice_raggruppamento_varieta_gias")), _
                                '                                        CStr(""), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#), _
                                '                                        BaseCode, _
                                '                                        TopCode, "Impianto")

                                '    End If

                                '    If CStr(XML_RegImpiantoPubblico.GetAttribute("imp_chiave_cliente")) <> "#" Then
                                '        Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")), _
                                '                                        Id_Codice_ChiaveCliente, _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("imp_chiave_cliente"), ""), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
                                '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#), _
                                '                                        BaseCode, _
                                '                                        TopCode, "Impianto")

                                '    End If

                                '    If XML_RegImpiantoPubblico.HasAttribute("codice_destinazioneuso_gias") = True Then
                                '        If CStr(XML_RegImpiantoPubblico.GetAttribute("codice_destinazioneuso_gias")) <> "#" Then
                                '            Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")), _
                                '                                    CInt(XML_RegImpiantoPubblico.GetAttribute("codice_destinazioneuso_gias")), _
                                '                                    "", _
                                '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)), _
                                '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)), _
                                '                                    BaseCode, _
                                '                                    TopCode, "Impianto")
                                '        End If
                                '    End If

                                '    'Modifica del 24/02/2011: sia il COM+ che il core si aspettano il nodo DatiCodici
                                '    'quindi occorre crearlo come raccoglitore dei codici,
                                '    'altrimenti sul db non viene scritto nulla

                                '    'creo il tag raccoglitore
                                '    XML_DatiCodiciImpianto = XmlDocPrivato_4.CreateElement("DatiCodici")

                                '    XML_DatiCodiciImpianto.InnerXml = Str_CodiceImpianto

                                '    'XML_RegImpiantoPrivato.InnerXml = Str_CodiceImpianto '& str_Progetto
                                '    XML_RegImpiantoPrivato.InnerXml = XML_DatiCodiciImpianto.OuterXml

                                '    XML_DatiProgetto = Gestione_XML_Progetto(HT_Piva_objCodici, _
                                '                                                XML_RegImpiantoPubblico, _
                                '                                                 XML_ImpresaPubblico, _
                                '                                                 XML_CentroAziendalePubblico, _
                                '                                                 XML_AppezzamentoPubblico, _
                                '                                                 Id_Reg, _
                                '                                                 app_nome)

                                '    ''£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                                '    ''££££££££££££££££££££££                 PROGETTO            ££££££££££££££££££££££££££££££££
                                '    ''£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

                                '    'XmlDocPrivato_9 = New System.Xml.XmlDocument

                                '    ''creo il tag raccoglitore
                                '    'XML_DatiProgetto = XmlDocPrivato_9.CreateElement("DatiProgetto")

                                '    ''se nn esistono nodi progetto ne creo cmq uno associato all'impianto
                                '    'XMLs_ProgettoPubblico = XML_RegImpiantoPubblico.GetElementsByTagName("Progetto")


                                '    'If XMLs_ProgettoPubblico.Count = 0 Then

                                '    '    'non sono più gestite nella tabella impianti
                                '    '    'ma sulla distinta
                                '    '    Dim Piante_HA As Double = 0
                                '    '    Dim Resa_Prevista As Double = 0
                                '    '    Dim Data_Semina_Prevista As Date = AGRODATAINIZIO
                                '    '    Dim Data_Raccolta_Prevista As Date = AGRODATAFINE

                                '    '    Piante_HA = Agro_If(XML_RegImpiantoPubblico.GetAttribute("piante_per_ha"), CDbl(0))
                                '    '    Resa_Prevista = Agro_If(XML_RegImpiantoPubblico.GetAttribute("resa_prevista"), CDbl(0))

                                '    '    If XML_RegImpiantoPubblico.HasAttribute("data_semina_prevista") = True Then
                                '    '        Data_Semina_Prevista = XML_RegImpiantoPubblico.GetAttribute("data_semina_prevista")
                                '    '    End If

                                '    '    If XML_RegImpiantoPubblico.HasAttribute("data_raccolta_prevista") = True Then
                                '    '        Data_Raccolta_Prevista = XML_RegImpiantoPubblico.GetAttribute("data_raccolta_prevista")
                                '    '    End If

                                '    '    Dim data_inizio_impianto, data_fine_impianto As Date
                                '    '    data_inizio_impianto = Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#)
                                '    '    data_fine_impianto = Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#)
                                '    '    Dim lotto_progetto As String = ""

                                '    '    If data_inizio_impianto <> #1/1/1900# Then

                                '    '        If data_fine_impianto <> #12/31/2100# Then

                                '    '            If CDate(data_inizio_impianto).Year <> CDate(data_fine_impianto).Year Then
                                '    '                lotto_progetto = app_nome + " - " + _
                                '    '                                 " Lotto " + CStr(data_inizio_impianto.Year) + "/" + CStr(data_fine_impianto.Year)
                                '    '            Else
                                '    '                lotto_progetto = app_nome + " - " + " Lotto " + CStr(data_inizio_impianto.Year)
                                '    '            End If
                                '    '        Else
                                '    '            lotto_progetto = app_nome + " - " + " Lotto " + CStr(data_inizio_impianto.Year)
                                '    '        End If
                                '    '    Else
                                '    '        lotto_progetto = app_nome
                                '    '    End If

                                '    '    XML_ProgettoPrivato = XML_Progetto(CInt(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")), _
                                '    '                                    CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")), _
                                '    '                                    CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")), _
                                '    '                                    0, _
                                '    '                                    lotto_progetto, _
                                '    '                                    lotto_progetto, _
                                '    '                                        9100, _
                                '    '                                        , _
                                '    '                                        , _
                                '    '                                        Data_Semina_Prevista, _
                                '    '                                        Data_Raccolta_Prevista, _
                                '    '                                        "", _
                                '    '                                        CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")), _
                                '    '                                        CInt(Id_Reg), _
                                '    '                                        , , _
                                '    '                                        , _
                                '    '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("codice_regolamento"), CInt(1)), _
                                '    '                                        , _
                                '    '                                        , _
                                '    '                                         , _
                                '    '                                         Resa_Prevista, _
                                '    '                                        data_inizio_impianto, _
                                '    '                                        data_fine_impianto, _
                                '    '                                        BaseCode, _
                                '    '                                        TopCode, _
                                '    '                                        Piante_HA)


                                '    '    'importo l'appezzamento nel documento del campo
                                '    '    XML_Nodo = XML_DatiProgetto.OwnerDocument.ImportNode(XML_ProgettoPrivato, True)

                                '    '    'appendo l'appezzamento as campo
                                '    '    XML_DatiProgetto.AppendChild(XML_Nodo)

                                '    'Else

                                '    '    'str_Progetto = ""

                                '    '    Dim Piante_HA As Double
                                '    '    Dim Resa_Prevista As Double
                                '    '    Dim Data_Semina_Prevista As Date
                                '    '    Dim Data_Raccolta_Prevista As Date

                                '    '    For i_10 = 0 To XMLs_ProgettoPubblico.Count - 1

                                '    '        XML_ProgettoPubblico = XMLs_ProgettoPubblico(i_10)

                                '    '        Piante_HA = 0
                                '    '        Resa_Prevista = 0
                                '    '        Data_Semina_Prevista = AGRODATAINIZIO
                                '    '        Data_Raccolta_Prevista = AGRODATAFINE

                                '    '        If XML_ProgettoPubblico.HasAttribute("piante_per_ha") Then
                                '    '            Piante_HA = XML_ProgettoPubblico.GetAttribute("piante_per_ha")
                                '    '        End If

                                '    '        If XML_ProgettoPubblico.HasAttribute("resa_prevista") Then
                                '    '            Resa_Prevista = XML_ProgettoPubblico.GetAttribute("resa_prevista")
                                '    '        End If

                                '    '        If XML_ProgettoPubblico.HasAttribute("data_semina_prevista") = True Then
                                '    '            Data_Semina_Prevista = XML_ProgettoPubblico.GetAttribute("data_semina_prevista")
                                '    '        End If

                                '    '        If XML_ProgettoPubblico.HasAttribute("data_raccolta_prevista") = True Then
                                '    '            Data_Raccolta_Prevista = XML_ProgettoPubblico.GetAttribute("data_raccolta_prevista")
                                '    '        End If

                                '    '        XML_ProgettoPrivato = XML_Progetto(CInt(XML_ProgettoPubblico.GetAttribute("tipo_operazione")), _
                                '    '                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")), _
                                '    '                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")), _
                                '    '                                            CInt(XML_ProgettoPubblico.GetAttribute("codice_progetto")), _
                                '    '                                            CStr(XML_ProgettoPubblico.GetAttribute("descrizione_progetto")), _
                                '    '                                            CStr(XML_ProgettoPubblico.GetAttribute("descrizione_progetto")), _
                                '    '                                            9100, _
                                '    '                                            , _
                                '    '                                             , _
                                '    '                                             Data_Semina_Prevista, _
                                '    '                                            Data_Raccolta_Prevista, _
                                '    '                                            "", _
                                '    '                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")), _
                                '    '                                            CInt(Id_Reg), _
                                '    '                                            , , _
                                '    '                                            , _
                                '    '                                            Agro_If(XML_ProgettoPubblico.GetAttribute("codice_regolamento"), CInt(1)), _
                                '    '                                            Agro_If(XML_ProgettoPubblico.GetAttribute("codice_disciplinare"), CInt(1)), _
                                '    '                                            , _
                                '    '                                            , _
                                '    '                                             Resa_Prevista, _
                                '    '                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
                                '    '                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_fine"), #12/31/2100#), _
                                '    '                                            BaseCode, _
                                '    '                                            TopCode, _
                                '    '                                            Piante_HA)

                                '    '        'importo l'appezzamento nel documento del campo
                                '    '        XML_Nodo = XML_DatiProgetto.OwnerDocument.ImportNode(XML_ProgettoPrivato, True)

                                '    '        'appendo l'appezzamento as campo
                                '    '        XML_DatiProgetto.AppendChild(XML_Nodo)

                                '    '    Next


                                '    'End If


                                '    '----------  DATI PROGETTO 

                                '    If Not IsNothing(XML_DatiProgetto) Then

                                '        'importo il tag DATI-APPEZZAMENTI nel documento del campo
                                '        XML_Nodo = XML_RegImpiantoPrivato.OwnerDocument.ImportNode(XML_DatiProgetto, True)

                                '        'appendo il tag DATI-APPEZZAMENTI al campo
                                '        XML_RegImpiantoPrivato.AppendChild(XML_Nodo)

                                '    End If


                                '    '-----------  IMPIANTO

                                '    'importo l'appezzamento nel documento del campo
                                '    XML_Nodo = XML_DatiRegImpianti.OwnerDocument.ImportNode(XML_RegImpiantoPrivato, True)

                                '    'appendo l'appezzamento as campo
                                '    XML_DatiRegImpianti.AppendChild(XML_Nodo)


                                'Next    'CICLO SUI TAG IMPIANTO

                                ''----------  DATI REGIMPIANTI 

                                'importo il tag DATI-APPEZZAMENTI nel documento del campo
                                XML_Nodo = XML_AppezzamentoPrivato.OwnerDocument.ImportNode(XML_DatiRegImpianti, True)

                                'appendo il tag DATI-APPEZZAMENTI al campo
                                XML_AppezzamentoPrivato.AppendChild(XML_Nodo)

                                '----------  APPEZZAMENTO

                                'importo l'appezzamento nel documento del campo
                                XML_Nodo = XML_DatiAppezzamenti.OwnerDocument.ImportNode(XML_AppezzamentoPrivato, True)

                                'appendo l'appezzamento as campo
                                XML_DatiAppezzamenti.AppendChild(XML_Nodo)


                                ' StrTemp = XML_CampoPrivato.OuterXml

                            Next        'CICLO SUI TAG APPEZZAMENTO

                            'pippo = XML_DatiAppezzamenti.OuterXml

                            XML_Nodo = XML_CentroAziendalePrivato.OwnerDocument.ImportNode(XML_DatiAppezzamenti, True)

                            XML_CentroAziendalePrivato.AppendChild(XML_Nodo)

                        End If

                        '-----------------  DATI APPEZZAMENTI 


                        ''£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
                        ''££££££££££££££££££££££                 AGENDA            ££££££££££££££££££££££££££££££££££££££
                        ''£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££


                        pippo = XML_CentroAziendalePrivato.OuterXml

                        'èèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèèè

                        '------------- CENTRO AZIENDALE

                        'importo il nodo CentroAziendale nel documento del tag DATI-CENTRIAZIENDALI
                        XML_Nodo = XML_DatiCentriAziendali.OwnerDocument.ImportNode(XML_CentroAziendalePrivato, True)

                        'appendo il centro aziendale al tag DatiCentriAziendali
                        XML_DatiCentriAziendali.AppendChild(XML_Nodo)


                    Next        'CICLO SUI TAG CENTRO AZIENDALE

                    'importo il tag DATI-CENTRI AZIENDALI nel documento dell'impresa
                    XML_Nodo = XML_ImpresaPrivato.OwnerDocument.ImportNode(XML_DatiCentriAziendali, True)

                    'appendo il tag DATI-CENTRI AZIENDALI al centro aziendale
                    XML_ImpresaPrivato.AppendChild(XML_Nodo)


                    '------------- IMPRESA

                    'importo il nodo Impresa nel documento del tag DATI-IMPRESE
                    XML_Nodo = XML_DatiImprese.OwnerDocument.ImportNode(XML_ImpresaPrivato, True)

                    'appendo l'impresa al tag DatiImprese
                    XML_DatiImprese.AppendChild(XML_Nodo)

                    pippo = XML_DatiImprese.OuterXml


                Next            'CICLO SUI TAG IMPRESA

                '------------- IMPRESA

                StrTemp = XML_DatiImprese.OuterXml

                Return XML_DatiImprese.OwnerDocument.OuterXml

            Else

                Throw New Exception("E' necessario inserire un nodo <Utente>!")

            End If


        Catch ex As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            'Messaggio di errore
            strErr = NomeRoutine + ": si è verificato il seguente errore: " + ex.Message.ToString()

        End Try


    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Private Function TrovaIndiceMatrice(ByVal Cod As String) As Integer

        Dim m, n As Integer

        For m = 0 To UBound(Me.MatriceParticelle, 1)

            If MatriceParticelle(m, 0) = Cod Then
                Return m
            End If

        Next
        Return -1

    End Function


    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    'Funzione che setta il flag per l'impresa per la cancellazione dei codici
    Private Sub Codici_toDelete_Impresa(ByRef HT_Piva_objCodici As Hashtable,
                                            ByVal Piva As String)

        Dim obj_DelCod As ObjDeleteCodici
        obj_DelCod = HT_Piva_objCodici(Piva)
        obj_DelCod.Impresa = True

        'imposto l'oggetto nell'hash piva-oggetto
        HT_Piva_objCodici(Piva) = obj_DelCod

    End Sub

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    'Funzione che setta il flag per l'impianto per la cancellazione dei codici
    'e memorizza le chiavi degli impianti
    Private Sub Codici_toDelete_Centro(ByRef HT_Piva_objCodici As Hashtable,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer)

        'nell'oggetto corrispondente all'impresa setto il flag a true per il centro 
        'così so che per questa impresa dovrò gestire la cancellazione dei codici del centro
        Dim obj_DelCod As ObjDeleteCodici
        obj_DelCod = HT_Piva_objCodici(Piva)
        obj_DelCod.Centro = True

        'memorizzo la chiave dell'impianto in oggetto
        Dim HT_Keys As Hashtable
        Dim Key As String
        'recupero l'hash delle chiavi degli impianti
        HT_Keys = obj_DelCod.HT_Keys_Centro

        'nuova chiave
        Key = Piva + "|" +
                    CStr(Sa_Cod)

        'se non c'è l'aggiungo in elenco
        If Not HT_Keys.Contains(Key) Then
            HT_Keys.Add(Key, "")
        End If

        'imposto l'hash aggiornato nell'oggetto
        obj_DelCod.HT_Keys_Centro = HT_Keys

        'imposto l'oggetto nell'hash piva-oggetto
        HT_Piva_objCodici(Piva) = obj_DelCod

    End Sub

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    'crea il filtro aggiuntivo con le chiavi degli impianti da passare alla query di cancellazione
    Public Function Codici_toDelete_Centro_MakeFiltroAggiuntivo(ByVal objDelCod As ObjDeleteCodici) As String

        Dim FiltroAgg As String = ""
        Dim HT_Keys As Hashtable
        Dim Key As String
        Dim Lista_Chiavi As String = ""

        'recupero l'hash delle chiavi delle distinte
        HT_Keys = objDelCod.HT_Keys_Centro

        For Each Key In HT_Keys.Keys

            Lista_Chiavi += "'" + Key + "',"

        Next

        'tolgo l'ultima virgola
        Lista_Chiavi = Left(Lista_Chiavi, Lista_Chiavi.Length - 1)

        FiltroAgg = "   (               (Centri_Aziendali_Codici.Piva  " & vbCrLf
        FiltroAgg += "                     + '|' + CONVERT(varchar(10), Centri_Aziendali_Codici.Sa_Cod)  " & vbCrLf
        FiltroAgg += "                   )  " & vbCrLf
        FiltroAgg += "                     IN (" & Lista_Chiavi & ")  " & vbCrLf
        FiltroAgg += "  )"

        Return FiltroAgg

    End Function


    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    'Funzione che setta il flag per la distinta per la cancellazione dei codici
    'e memorizza le chiavi delle distinte
    Private Sub Codici_toDelete_Distinta(ByRef HT_Piva_objCodici As Hashtable,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Appezza As Integer,
                                            ByVal Id_Reg As Integer,
                                            ByVal Cod_Progetto As Integer)

        'nell'oggetto corrispondente all'impresa setto il flag a true per la distinta 
        'così so che per questa impresa dovrò gestire la cancellazione dei codici della distinta
        Dim obj_DelCod As ObjDeleteCodici
        obj_DelCod = HT_Piva_objCodici(Piva)
        obj_DelCod.Distinta = True

        'memorizzo la chiave della distinta in oggetto
        Dim HT_Keys_Distinta As Hashtable
        Dim KeyDistinta As String
        'recupero l'hash delle chiavi delle distinte
        HT_Keys_Distinta = obj_DelCod.HT_Keys_Distinta

        'nuova chiave
        KeyDistinta = Piva + "|" +
                    CStr(Sa_Cod) + "|" +
                    CStr(Appezza) + "|" +
                     CStr(Id_Reg) + "|" +
                     CStr(Cod_Progetto)

        'se non c'è l'aggiungo in elenco
        If Not HT_Keys_Distinta.Contains(KeyDistinta) Then
            HT_Keys_Distinta.Add(KeyDistinta, "")
        End If

        'imposto l'hash aggiornato nell'oggetto
        obj_DelCod.HT_Keys_Distinta = HT_Keys_Distinta

        'imposto l'oggetto nell'hash piva-oggetto
        HT_Piva_objCodici(Piva) = obj_DelCod

    End Sub

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    'crea il filtro aggiuntivo con le chiavi delle distinte da passare a AgronicaCoreAnagrafeDAL.Impresa_Progetti_W.Cancella_Solo_Imprese_Progetti()
    Public Function Codici_toDelete_Distinta_MakeFiltroAggiuntivo(ByVal objDelCod As ObjDeleteCodici) As String

        Dim FiltroAgg As String = ""
        Dim HT_Keys_Distinta As Hashtable
        Dim KeyDistinta As String
        Dim Lista_Chiavi As String = ""
        'Dim Piva, Sa_Cod, Appezza, Id_Reg, Cod_Progetto As String

        'recupero l'hash delle chiavi delle distinte
        HT_Keys_Distinta = objDelCod.HT_Keys_Distinta

        For Each KeyDistinta In HT_Keys_Distinta.Keys

            'Piva = KeyDistinta.Split("|")(0)
            'Sa_Cod = KeyDistinta.Split("|")(1)
            'Appezza = KeyDistinta.Split("|")(2)
            'Id_Reg = KeyDistinta.Split("|")(3)
            'Cod_Progetto = KeyDistinta.Split("|")(4)

            Lista_Chiavi += "'" + KeyDistinta + "',"

        Next

        'tolgo l'ultima virgola
        Lista_Chiavi = Left(Lista_Chiavi, Lista_Chiavi.Length - 1)

        FiltroAgg = "   (               (Reg_Impianti_Codici.Piva  " & vbCrLf
        FiltroAgg += "                     + '|' + CONVERT(varchar(10), Reg_Impianti_Codici.Sa_Cod)  " & vbCrLf
        FiltroAgg += "                     + '|' + CONVERT(varchar(10), Reg_Impianti_Codici.Appezza)  " & vbCrLf
        FiltroAgg += "                     + '|' + CONVERT(varchar(10), Reg_Impianti_Codici.Id_Reg)  " & vbCrLf
        FiltroAgg += "                     + '|' + CONVERT(varchar(10), Reg_Impianti_Codici.Progetto_Cod))  " & vbCrLf
        FiltroAgg += "                     IN (" & Lista_Chiavi & ")  " & vbCrLf
        FiltroAgg += "  )"

        Return FiltroAgg

    End Function

    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    'Funzione che setta il flag per l'impianto per la cancellazione dei codici
    'e memorizza le chiavi degli impianti
    Private Sub Codici_toDelete_Impianto(ByRef HT_Piva_objCodici As Hashtable,
                                            ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Appezza As Integer,
                                            ByVal Id_Reg As Integer)

        'nell'oggetto corrispondente all'impresa setto il flag a true per l'impianto 
        'così so che per questa impresa dovrò gestire la cancellazione dei codici dell'impianto
        Dim obj_DelCod As ObjDeleteCodici
        obj_DelCod = HT_Piva_objCodici(Piva)
        obj_DelCod.Impianto = True

        'memorizzo la chiave dell'impianto in oggetto
        Dim HT_Keys_Impianto As Hashtable
        Dim KeyImpianto As String
        'recupero l'hash delle chiavi degli impianti
        HT_Keys_Impianto = obj_DelCod.HT_Keys_Impianto

        'nuova chiave
        KeyImpianto = Piva + "|" +
                    CStr(Sa_Cod) + "|" +
                    CStr(Appezza) + "|" +
                     CStr(Id_Reg)

        'se non c'è l'aggiungo in elenco
        If Not HT_Keys_Impianto.Contains(KeyImpianto) Then
            HT_Keys_Impianto.Add(KeyImpianto, "")
        End If

        'imposto l'hash aggiornato nell'oggetto
        obj_DelCod.HT_Keys_Impianto = HT_Keys_Impianto

        'imposto l'oggetto nell'hash piva-oggetto
        HT_Piva_objCodici(Piva) = obj_DelCod

    End Sub


    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    'crea il filtro aggiuntivo con le chiavi degli impianti da passare alla query di cancellazione
    Public Function Codici_toDelete_Impianto_MakeFiltroAggiuntivo(ByVal objDelCod As ObjDeleteCodici) As String

        Dim FiltroAgg As String = ""
        Dim HT_Keys_Impianto As Hashtable
        Dim KeyImpianto As String
        Dim Lista_Chiavi As String = ""
        'Dim Piva, Sa_Cod, Appezza, Id_Reg As String

        'recupero l'hash delle chiavi delle distinte
        HT_Keys_Impianto = objDelCod.HT_Keys_Impianto

        For Each KeyImpianto In HT_Keys_Impianto.Keys

            'Piva = KeyDistinta.Split("|")(0)
            'Sa_Cod = KeyDistinta.Split("|")(1)
            'Appezza = KeyDistinta.Split("|")(2)
            'Id_Reg = KeyDistinta.Split("|")(3)

            Lista_Chiavi += "'" + KeyImpianto + "',"

        Next

        'tolgo l'ultima virgola
        Lista_Chiavi = Left(Lista_Chiavi, Lista_Chiavi.Length - 1)

        FiltroAgg = "   (               (Reg_Impianti_Codici.Piva  " & vbCrLf
        FiltroAgg += "                     + '|' + CONVERT(varchar(10), Reg_Impianti_Codici.Sa_Cod)  " & vbCrLf
        FiltroAgg += "                     + '|' + CONVERT(varchar(10), Reg_Impianti_Codici.Appezza)  " & vbCrLf
        FiltroAgg += "                     + '|' + CONVERT(varchar(10), Reg_Impianti_Codici.Id_Reg) )  " & vbCrLf
        FiltroAgg += "                     IN (" & Lista_Chiavi & ")  " & vbCrLf
        FiltroAgg += "  )"

        Return FiltroAgg

    End Function



    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Private Function Gestione_XML_Impianto(ByRef HT_Piva_objCodici As Hashtable,
                                            ByVal XML_ImpresaPubblico As XmlElement,
                                            ByVal XML_CentroAziendalePubblico As XmlElement,
                                            ByVal XML_AppezzamentoPubblico As XmlElement,
                                            ByVal app_nome As String,
                                            ByRef Str_Appezzamento As String,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As XmlElement

        Dim xmlAnagHlp As New XML_Anagrafe


        Dim XmlDocPrivato_Impianto As XmlDocument

        Dim XML_DatiRegImpianti As System.Xml.XmlElement
        Dim XMLs_RegImpiantoPubblico As System.Xml.XmlNodeList
        Dim XML_RegImpiantoPubblico As System.Xml.XmlElement
        Dim XML_RegImpiantoPrivato As System.Xml.XmlElement
        Dim XML_DatiCodiciImpianto As XmlElement

        Dim XML_DatiProgetto As XmlElement

        Dim XML_Nodo As System.Xml.XmlElement

        Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Dim objCACCultivar As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_R
        Dim objCACVegcod As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Veg_Cod_R

        Dim i_9 As Integer

        Dim AppezzaIdReg As String
        Dim ArrayAppezzaIdReg() As String
        Dim Appezza As String
        Dim Id_Reg As String

        Dim Grfi_Cod As Integer
        Dim Cul_Cod As Integer
        Dim Veg_Cod As Integer

        Dim Str_CodiceImpianto As String

        Dim Codice_Fiscale_Tecnico As String


        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
        '££££££££££££££££££££££                 REG_IMPIANTO            ££££££££££££££££££££££££££££££££
        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

        XmlDocPrivato_Impianto = New System.Xml.XmlDocument

        'creo il tag raccoglitore
        XML_DatiRegImpianti = XmlDocPrivato_Impianto.CreateElement("DatiReg_Impianti")

        'Leggo l'insieme dei nodi particella
        XMLs_RegImpiantoPubblico = XML_AppezzamentoPubblico.GetElementsByTagName("Impianto")

        Str_Appezzamento = ""

        For i_9 = 0 To XMLs_RegImpiantoPubblico.Count - 1

            XML_RegImpiantoPubblico = XMLs_RegImpiantoPubblico(i_9)

            Grfi_Cod = 0

            'CASO CODICI CLIENTE
            If CStr(XML_RegImpiantoPubblico.GetAttribute("codice_specie_gias")) = "#" And
            CStr(XML_RegImpiantoPubblico.GetAttribute("codice_varieta_gias")) = "#" Then

                If CStr(XML_RegImpiantoPubblico.GetAttribute("codice_varieta_cliente")) <> "#" Then

                    'Converti il codice varietà del Cliente a GIAS
                    'ConvertiVarieta(XML_RegImpiantoPubblico.GetAttribute("codice_specie_cliente"), _
                    '                XML_RegImpiantoPubblico.GetAttribute("codice_varieta_cliente"), _
                    '                Cul_Cod, _
                    '                Veg_Cod, _
                    '                Connessione)

                    objCACCultivar.VegCulCodGias_from_CulCodCliente(
                                                        XML_RegImpiantoPubblico.GetAttribute("codice_varieta_cliente"),
                                                        Cul_Cod,
                                                        Veg_Cod,
                                                        objParametri_Server)

                    If Veg_Cod <> 0 Then

                        If Cul_Cod = 0 Then
                            Cul_Cod = objCultivar.VarietaAltre(Veg_Cod, objParametri_Server)
                        End If

                    Else

                        If CStr(XML_RegImpiantoPubblico.GetAttribute("codice_specie_cliente")) <> "#" Then

                            'Veg_Cod = ConvertiSpecie(XML_RegImpiantoPubblico.GetAttribute("codice_specie_cliente"), _
                            '                        Connessione)

                            Veg_Cod = objCACVegcod.VegCodGias_from_VegCodCliente(XML_RegImpiantoPubblico.GetAttribute("codice_specie_cliente"),
                                                                                    objParametri_Server)

                            If Veg_Cod <> 0 Then
                                Cul_Cod = objCultivar.VarietaAltre(Veg_Cod, objParametri_Server)
                            Else
                                Cul_Cod = 0
                            End If

                        Else
                            Cul_Cod = 0
                        End If

                    End If

                ElseIf CStr(XML_RegImpiantoPubblico.GetAttribute("codice_specie_cliente")) <> "#" Then

                    'Veg_Cod = ConvertiSpecie(XML_RegImpiantoPubblico.GetAttribute("codice_specie_cliente"), _
                    '                        Connessione)

                    Veg_Cod = objCACVegcod.VegCodGias_from_VegCodCliente(XML_RegImpiantoPubblico.GetAttribute("codice_specie_cliente"),
                                                                                 objParametri_Server)

                    If Veg_Cod <> 0 Then
                        Cul_Cod = objCultivar.VarietaAltre(Veg_Cod, objParametri_Server)
                    Else
                        Cul_Cod = 0
                    End If

                End If

            Else

                'CASO CODICI GIAS
                If CStr(XML_RegImpiantoPubblico.GetAttribute("codice_specie_gias")) <> "#" And
                CStr(XML_RegImpiantoPubblico.GetAttribute("codice_varieta_gias")) <> "#" Then

                    Cul_Cod = XML_RegImpiantoPubblico.GetAttribute("codice_varieta_gias")
                    Veg_Cod = XML_RegImpiantoPubblico.GetAttribute("codice_specie_gias")

                ElseIf CStr(XML_RegImpiantoPubblico.GetAttribute("codice_specie_gias")) <> "#" Then

                    Veg_Cod = XML_RegImpiantoPubblico.GetAttribute("codice_specie_gias")
                    Cul_Cod = objCultivar.VarietaAltre(Veg_Cod, objParametri_Server)
                Else
                    Cul_Cod = XML_RegImpiantoPubblico.GetAttribute("codice_varieta_gias")
                    'Veg_Cod = VegCod_from_CulCod(Veg_Cod, Connessione)
                    Veg_Cod = objCultivar.VegCod_from_CulCod(Cul_Cod, objParametri_Server)
                End If

                If Veg_Cod <> 0 Then
                    If CStr(XML_RegImpiantoPubblico.GetAttribute("codice_finalita_gias")) <> "#" Then
                        Grfi_Cod = XML_RegImpiantoPubblico.GetAttribute("codice_finalita_gias")
                    End If
                End If


            End If

            AppezzaIdReg = "0"
            Appezza = "0"
            Id_Reg = "0"

            AppezzaIdReg = XML_RegImpiantoPubblico.GetAttribute("codice_impianto")

            If AppezzaIdReg <> "0" Then
                ArrayAppezzaIdReg = Split(AppezzaIdReg, "/")
                Id_Reg = CInt(ArrayAppezzaIdReg(1))
            Else
                Id_Reg = "0"
            End If


            Dim unita_vitata As String = "#"
            If XML_RegImpiantoPubblico.HasAttribute("unita_vitata") Then
                If Not IsNothing(XML_RegImpiantoPubblico.GetAttribute("unita_vitata")) Then
                    If CStr(XML_RegImpiantoPubblico.GetAttribute("unita_vitata")) <> "#" Then
                        If IsNumeric(XML_RegImpiantoPubblico.GetAttribute("unita_vitata")) Then
                            unita_vitata = CStr(CInt(XML_RegImpiantoPubblico.GetAttribute("unita_vitata")))
                        End If
                    End If
                End If
            End If

            If XML_RegImpiantoPubblico.HasAttribute("Codice_Fiscale_Tecnico".ToLower) = True Then

                If Not IsNothing(XML_RegImpiantoPubblico.GetAttribute("Codice_Fiscale_Tecnico".ToLower)) AndAlso
                     CStr(XML_RegImpiantoPubblico.GetAttribute("Codice_Fiscale_Tecnico".ToLower)) <> "#" AndAlso
                        CStr(XML_RegImpiantoPubblico.GetAttribute("Codice_Fiscale_Tecnico".ToLower)) <> "" Then

                    Codice_Fiscale_Tecnico = CStr(XML_RegImpiantoPubblico.GetAttribute("Codice_Fiscale_Tecnico".ToLower))

                Else
                    Codice_Fiscale_Tecnico = objParametri_Server.PivaSuperUser
                End If
            Else
                Codice_Fiscale_Tecnico = objParametri_Server.PivaSuperUser
            End If


            XML_RegImpiantoPrivato = xmlAnagHlp.XML_Impianto(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")),
                                                CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                CInt(0),
                                                CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                                CInt(Id_Reg),
                                                CInt(0),
                                                CInt(0),
                                                CInt(0),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                Cul_Cod,
                                                "",
                                                 0,
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("resa_effettiva"), CDbl(0)),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("scarto"), CDbl(0)),
                                                0, "0", "", "", "",
                                                0,
                                                0,
                                                 0,
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("semina_trapianto"), ""),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("codice_portinnesto"), CInt(0)),
                                                0, 0, 0, 0, 0, 0, "", 0, 0, 0, 0, 0,
                                                Codice_Fiscale_Tecnico,
                                                "",
                                                CInt(Grfi_Cod),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("codice_impianto_irrigazione"), ""),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("codice_regolamento"), CInt(1)),
                                                CInt(0),
                                                -1,
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("codice_copertura"), CInt(-1)),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("codice_forma_allevamento"), CInt(-1)),
                                                -1, 0,
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("sup_imp"), CDbl(0)),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("codice_tipologia_varieta_gias"), CDbl(0)),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                BaseCode,
                                                TopCode,
                                                unita_vitata)

            'CODICI IMPIANTO
            Str_CodiceImpianto = ""

            Dim TipoOp_Codice As enum_TipoOperazioneDB

            TipoOp_Codice = CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione"))

            If TipoOp_Codice = enum_TipoOperazioneDB.Modifica Then
                TipoOp_Codice = enum_TipoOperazioneDB.Scrittura
            End If


            If CStr(XML_RegImpiantoPubblico.GetAttribute("titolo_possesso")) <> "#" Then
                Str_CodiceImpianto += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                enum_CodiciAnagrafe.TitoloPossesso,
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("titolo_possesso"), 1),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                BaseCode,
                                                TopCode, "Impianto")
            End If

            If CStr(XML_RegImpiantoPubblico.GetAttribute("distanza_tra_fila")) <> "#" Then
                Str_CodiceImpianto += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                enum_CodiciAnagrafe.Impianto_TraFila_Maschio,
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("distanza_tra_fila"), 0),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                BaseCode,
                                                TopCode, "Impianto")
            End If

            If CStr(XML_RegImpiantoPubblico.GetAttribute("distanza_su_fila")) <> "#" Then
                Str_CodiceImpianto += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                enum_CodiciAnagrafe.Impianto_SuFila_Maschio,
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("distanza_su_fila"), 0),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                BaseCode,
                                                TopCode, "Impianto")
            End If
            If CStr(XML_RegImpiantoPubblico.GetAttribute("interbina")) <> "#" Then
                Str_CodiceImpianto += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                enum_CodiciAnagrafe.Impianto_Interbina,
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("interbina"), 0),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                BaseCode,
                                                TopCode, "Impianto")
            End If

            If CStr(XML_RegImpiantoPubblico.GetAttribute("piano_semina")) <> "#" Then
                Str_CodiceImpianto += xmlAnagHlp.XML_Codice(TipoOp_Codice,
                                                enum_CodiciAnagrafe.Impianto_PianoSemina,
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("piano_semina"), ""),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                BaseCode,
                                                TopCode, "Impianto")
            End If

            'no, è un codice della distinta!
            'If CStr(XML_RegImpiantoPubblico.GetAttribute("cooperativa_referente")) <> "#" Then
            '    Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")), _
            '                                    Id_Codice_Cooperativa, _
            '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("cooperativa_referente"), ""), _
            '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
            '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#), _
            '                                    BaseCode, _
            '                                    TopCode, "Impianto")

            'End If

            'no, è un codice della distinta!
            'If CStr(XML_RegImpiantoPubblico.GetAttribute("capitolato_privato")) <> "#" Then
            '    Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")), _
            '                                    Id_Capitolato_Privato, _
            '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("capitolato_privato"), ""), _
            '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
            '                                    Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#), _
            '                                    BaseCode, _
            '                                    TopCode, "Impianto")

            'End If

            If CStr(XML_RegImpiantoPubblico.GetAttribute("codice_raggruppamento_varieta_gias")) <> "#" Then
                Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")),
                                                CInt(XML_RegImpiantoPubblico.GetAttribute("codice_raggruppamento_varieta_gias")),
                                                CStr(""),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                BaseCode,
                                                TopCode, "Impianto")

            End If

            If CStr(XML_RegImpiantoPubblico.GetAttribute("imp_chiave_cliente")) <> "#" Then
                Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")),
                                                Id_Codice_ChiaveCliente,
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("imp_chiave_cliente"), ""),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#),
                                                BaseCode,
                                                TopCode, "Impianto")

            End If

            If XML_RegImpiantoPubblico.HasAttribute("codice_destinazioneuso_gias") = True Then
                If CStr(XML_RegImpiantoPubblico.GetAttribute("codice_destinazioneuso_gias")) <> "#" Then
                    Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")),
                                            CInt(XML_RegImpiantoPubblico.GetAttribute("codice_destinazioneuso_gias")),
                                            "",
                                            Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
                                            Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
                                            BaseCode,
                                            TopCode, "Impianto")
                End If
            End If

            ' VAnni: 10/9/2018: Spostato salvataggio dei codici kpin, blockname su distinta
            'If CInt(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")) = enum_TipoOperazioneDB.Scrittura AndAlso
            '    XML_RegImpiantoPubblico.HasAttribute("zespri_blockname") = True Then

            '    If CStr(XML_RegImpiantoPubblico.GetAttribute("zespri_blockname")) <> "#" Then
            '        Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")),
            '                                enum_CodiciAnagrafe.Zespri_Block_Name,
            '                                XML_RegImpiantoPubblico.GetAttribute("zespri_blockname"),
            '                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
            '                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
            '                                BaseCode,
            '                                TopCode, "Impianto")
            '    End If

            '    If CStr(XML_RegImpiantoPubblico.GetAttribute("zespri_kpin")) <> "#" Then
            '        Str_CodiceImpianto += xmlAnagHlp.XML_Codice(CStr(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")),
            '                                enum_CodiciAnagrafe.Zespri_Codice_kPIN,
            '                                XML_RegImpiantoPubblico.GetAttribute("zespri_kpin"),
            '                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), CDate(#1/1/1900#)),
            '                                Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), CDate(#12/31/2100#)),
            '                                BaseCode,
            '                                TopCode, "Impianto")
            '    End If

            'End If

            'Modifica del 24/02/2011: sia il COM+ che il core si aspettano il nodo DatiCodici
            'quindi occorre crearlo come raccoglitore dei codici,
            'altrimenti sul db non viene scritto nulla

            'creo il tag raccoglitore
            XML_DatiCodiciImpianto = XmlDocPrivato_Impianto.CreateElement("DatiCodici")

            XML_DatiCodiciImpianto.InnerXml = Str_CodiceImpianto

            'XML_RegImpiantoPrivato.InnerXml = Str_CodiceImpianto '& str_Progetto
            XML_RegImpiantoPrivato.InnerXml = XML_DatiCodiciImpianto.OuterXml


            'se ci sono dei codici legati all'impianto
            'e se sono in modifica
            If Str_CodiceImpianto <> "" And CInt(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")) = enum_TipoOperazioneDB.Modifica Then

                Codici_toDelete_Impianto(HT_Piva_objCodici,
                                          CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                            CInt(Id_Reg))

            End If

            'se sono in modifica imposto i codici del sesto d'impianto con tipo operazione scrittura (prima vengono cancellati)
            'gli altri codici andranno gestiti
            If CInt(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")) = enum_TipoOperazioneDB.Modifica Then
                Dim XML_Codice_list As XmlNodeList
                Dim XML_Codice As XmlElement
                Dim xml_datiCodici As XmlElement
                Dim i As Integer
                xml_datiCodici = XML_RegImpiantoPrivato.SelectSingleNode("DatiCodici")
                XML_Codice_list = xml_datiCodici.SelectNodes("CodiceImpianto")
                For i = 0 To XML_Codice_list.Count - 1
                    XML_Codice = XML_Codice_list.Item(i)
                    Select Case CInt(XML_Codice.GetAttribute("id_cod"))
                        'al momento non gestisco
                        'Id_Codice_ChiaveCliente 
                        'e CInt(XML_RegImpiantoPubblico.GetAttribute("codice_raggruppamento_varieta_gias"))
                        'e CInt(XML_RegImpiantoPubblico.GetAttribute("codice_destinazioneuso_gias"))
                        Case enum_CodiciAnagrafe.TitoloPossesso,
                            enum_CodiciAnagrafe.Impianto_TraFila_Maschio,
                            enum_CodiciAnagrafe.Impianto_SuFila_Maschio,
                            enum_CodiciAnagrafe.Impianto_Interbina,
                            enum_CodiciAnagrafe.Impianto_PianoSemina,
                            enum_CodiciAnagrafe.Zespri_Block_Name
                            XML_Codice.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Scrittura))
                        Case Else
                            'lascio il mondo come sta
                    End Select
                Next
            End If


            ''£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
            ''££££££££££££££££££££££                 PROGETTO            ££££££££££££££££££££££££££££££££
            ''£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

            XML_DatiProgetto = Gestione_XML_Progetto(HT_Piva_objCodici,
                                                    XML_RegImpiantoPubblico,
                                                    XML_ImpresaPubblico,
                                                    XML_CentroAziendalePubblico,
                                                    XML_AppezzamentoPubblico,
                                                    Id_Reg,
                                                    app_nome)


            'XmlDocPrivato_9 = New System.Xml.XmlDocument

            ''creo il tag raccoglitore
            'XML_DatiProgetto = XmlDocPrivato_9.CreateElement("DatiProgetto")

            ''se nn esistono nodi progetto ne creo cmq uno associato all'impianto
            'XMLs_ProgettoPubblico = XML_RegImpiantoPubblico.GetElementsByTagName("Progetto")


            'If XMLs_ProgettoPubblico.Count = 0 Then

            '    'non sono più gestite nella tabella impianti
            '    'ma sulla distinta
            '    Dim Piante_HA As Double = 0
            '    Dim Resa_Prevista As Double = 0
            '    Dim Data_Semina_Prevista As Date = AGRODATAINIZIO
            '    Dim Data_Raccolta_Prevista As Date = AGRODATAFINE

            '    Piante_HA = Agro_If(XML_RegImpiantoPubblico.GetAttribute("piante_per_ha"), CDbl(0))
            '    Resa_Prevista = Agro_If(XML_RegImpiantoPubblico.GetAttribute("resa_prevista"), CDbl(0))

            '    If XML_RegImpiantoPubblico.HasAttribute("data_semina_prevista") = True Then
            '        Data_Semina_Prevista = XML_RegImpiantoPubblico.GetAttribute("data_semina_prevista")
            '    End If

            '    If XML_RegImpiantoPubblico.HasAttribute("data_raccolta_prevista") = True Then
            '        Data_Raccolta_Prevista = XML_RegImpiantoPubblico.GetAttribute("data_raccolta_prevista")
            '    End If

            '    Dim data_inizio_impianto, data_fine_impianto As Date
            '    data_inizio_impianto = Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#)
            '    data_fine_impianto = Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#)
            '    Dim lotto_progetto As String = ""

            '    If data_inizio_impianto <> #1/1/1900# Then

            '        If data_fine_impianto <> #12/31/2100# Then

            '            If CDate(data_inizio_impianto).Year <> CDate(data_fine_impianto).Year Then
            '                lotto_progetto = app_nome + " - " + _
            '                                 " Lotto " + CStr(data_inizio_impianto.Year) + "/" + CStr(data_fine_impianto.Year)
            '            Else
            '                lotto_progetto = app_nome + " - " + " Lotto " + CStr(data_inizio_impianto.Year)
            '            End If
            '        Else
            '            lotto_progetto = app_nome + " - " + " Lotto " + CStr(data_inizio_impianto.Year)
            '        End If
            '    Else
            '        lotto_progetto = app_nome
            '    End If

            '    XML_ProgettoPrivato = XML_Progetto(CInt(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")), _
            '                                    CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")), _
            '                                    CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")), _
            '                                    0, _
            '                                    lotto_progetto, _
            '                                    lotto_progetto, _
            '                                        9100, _
            '                                        , _
            '                                        , _
            '                                        Data_Semina_Prevista, _
            '                                        Data_Raccolta_Prevista, _
            '                                        "", _
            '                                        CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")), _
            '                                        CInt(Id_Reg), _
            '                                        , , _
            '                                        , _
            '                                        Agro_If(XML_RegImpiantoPubblico.GetAttribute("codice_regolamento"), CInt(1)), _
            '                                        , _
            '                                        , _
            '                                         , _
            '                                         Resa_Prevista, _
            '                                        data_inizio_impianto, _
            '                                        data_fine_impianto, _
            '                                        BaseCode, _
            '                                        TopCode, _
            '                                        Piante_HA)


            '    'importo l'appezzamento nel documento del campo
            '    XML_Nodo = XML_DatiProgetto.OwnerDocument.ImportNode(XML_ProgettoPrivato, True)

            '    'appendo l'appezzamento as campo
            '    XML_DatiProgetto.AppendChild(XML_Nodo)

            'Else

            '    'str_Progetto = ""

            '    Dim Piante_HA As Double
            '    Dim Resa_Prevista As Double
            '    Dim Data_Semina_Prevista As Date
            '    Dim Data_Raccolta_Prevista As Date

            '    For i_10 = 0 To XMLs_ProgettoPubblico.Count - 1

            '        XML_ProgettoPubblico = XMLs_ProgettoPubblico(i_10)

            '        Piante_HA = 0
            '        Resa_Prevista = 0
            '        Data_Semina_Prevista = AGRODATAINIZIO
            '        Data_Raccolta_Prevista = AGRODATAFINE

            '        If XML_ProgettoPubblico.HasAttribute("piante_per_ha") Then
            '            Piante_HA = XML_ProgettoPubblico.GetAttribute("piante_per_ha")
            '        End If

            '        If XML_ProgettoPubblico.HasAttribute("resa_prevista") Then
            '            Resa_Prevista = XML_ProgettoPubblico.GetAttribute("resa_prevista")
            '        End If

            '        If XML_ProgettoPubblico.HasAttribute("data_semina_prevista") = True Then
            '            Data_Semina_Prevista = XML_ProgettoPubblico.GetAttribute("data_semina_prevista")
            '        End If

            '        If XML_ProgettoPubblico.HasAttribute("data_raccolta_prevista") = True Then
            '            Data_Raccolta_Prevista = XML_ProgettoPubblico.GetAttribute("data_raccolta_prevista")
            '        End If

            '        XML_ProgettoPrivato = XML_Progetto(CInt(XML_ProgettoPubblico.GetAttribute("tipo_operazione")), _
            '                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")), _
            '                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")), _
            '                                            CInt(XML_ProgettoPubblico.GetAttribute("codice_progetto")), _
            '                                            CStr(XML_ProgettoPubblico.GetAttribute("descrizione_progetto")), _
            '                                            CStr(XML_ProgettoPubblico.GetAttribute("descrizione_progetto")), _
            '                                            9100, _
            '                                            , _
            '                                             , _
            '                                             Data_Semina_Prevista, _
            '                                            Data_Raccolta_Prevista, _
            '                                            "", _
            '                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")), _
            '                                            CInt(Id_Reg), _
            '                                            , , _
            '                                            , _
            '                                            Agro_If(XML_ProgettoPubblico.GetAttribute("codice_regolamento"), CInt(1)), _
            '                                            Agro_If(XML_ProgettoPubblico.GetAttribute("codice_disciplinare"), CInt(1)), _
            '                                            , _
            '                                            , _
            '                                             Resa_Prevista, _
            '                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_inizio"), #1/1/1900#), _
            '                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_fine"), #12/31/2100#), _
            '                                            BaseCode, _
            '                                            TopCode, _
            '                                            Piante_HA)

            '        'importo l'appezzamento nel documento del campo
            '        XML_Nodo = XML_DatiProgetto.OwnerDocument.ImportNode(XML_ProgettoPrivato, True)

            '        'appendo l'appezzamento as campo
            '        XML_DatiProgetto.AppendChild(XML_Nodo)

            '    Next


            'End If


            '----------  DATI PROGETTO 

            If Not IsNothing(XML_DatiProgetto) Then

                'importo il tag DATI-APPEZZAMENTI nel documento del campo
                XML_Nodo = XML_RegImpiantoPrivato.OwnerDocument.ImportNode(XML_DatiProgetto, True)

                'appendo il tag DATI-APPEZZAMENTI al campo
                XML_RegImpiantoPrivato.AppendChild(XML_Nodo)

            End If


            '-----------  IMPIANTO

            'importo l'appezzamento nel documento del campo
            XML_Nodo = XML_DatiRegImpianti.OwnerDocument.ImportNode(XML_RegImpiantoPrivato, True)

            'appendo l'appezzamento as campo
            XML_DatiRegImpianti.AppendChild(XML_Nodo)


        Next    'CICLO SUI TAG IMPIANTO

        '----------  DATI REGIMPIANTI 

        Return XML_DatiRegImpianti


    End Function


    '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
    Private Function Gestione_XML_Progetto(ByRef HT_Piva_objCodici As Hashtable,
                                        ByVal XML_RegImpiantoPubblico As XmlElement,
                                        ByVal XML_ImpresaPubblico As XmlElement,
                                        ByVal XML_CentroAziendalePubblico As XmlElement,
                                        ByVal XML_AppezzamentoPubblico As XmlElement,
                                        ByVal Id_Reg As Integer,
                                        ByVal app_nome As String) As XmlElement




        Dim XML_DatiProgetto As System.Xml.XmlElement
        Dim XMLs_ProgettoPubblico As System.Xml.XmlNodeList
        Dim XML_ProgettoPubblico As System.Xml.XmlElement
        Dim XML_ProgettoPrivato As System.Xml.XmlElement
        Dim XML_Nodo As System.Xml.XmlElement
        Dim XmlDocPrivato_9 As XmlDocument

        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££
        '££££££££££££££££££££££                 PROGETTO            ££££££££££££££££££££££££££££££££
        '£££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££££

        XmlDocPrivato_9 = New System.Xml.XmlDocument

        'creo il tag raccoglitore
        XML_DatiProgetto = XmlDocPrivato_9.CreateElement("DatiProgetto")

        'se nn esistono nodi progetto ne creo cmq uno associato all'impianto
        XMLs_ProgettoPubblico = XML_RegImpiantoPubblico.GetElementsByTagName("Progetto")

        'pippo = XML_RegImpiantoPubblico.OuterXml


        '============================================================
        '========       PROGETTO NON PRESENTE     ===================
        '============================================================
        If XMLs_ProgettoPubblico.Count = 0 Then

            '< Impianto		tipo_operazione="1" *
            'codice_impianto="0" * // nel caso <>0 è composto da “codice_appezzamento/codice_impianto”
            'codice_specie_gias="52"		// opzionale si ricava dalla var.
            'codice_finalita_gias="2"	     	// opzionale default=0 	vedi nota
            'codice_varieta_gias ="678"*	// 
            'codice_tipologia_varieta_gias="2"	// opzionale default=0 	vedi nota
            'codice_destinazioneuso_gias="2"	// opzionale default=0 	
            'codice_dettaglio_specie_personalizzato="2"	// opzionale default=0 vedi nota
            'codice_specie_cliente =""		// si inseriscono se non presenti
            'codice_finalita_cliente =""		// quelli gias…
            'codice_varieta_cliente =""		//
            'titolo_possesso="1" 			// opzionale default=1 	vedi nota
            'metodo_produzione="1" 		// opzionale default=1 	vedi nota
            'resa_prevista="2"        		// opzionale default=0
            'resa_effettiva=""			// opzionale default=0
            'scarto="0" 				// opzionale default=0
            'codice_portinnesto="31" 		// opzionale			vedi nota
            'codice_impianto_irrigazione="31"  // opzionale default=0        vedi nota
            'codice_regolamento="7" 		// opzionale default=1 	vedi nota
            'codice_forma_allevamento ="9" 	// opzionale  			vedi nota
            'codice_copertura ="4" 		// opzionale  			vedi nota
            'semina_trapianto="-1" 		//opzionale default=-1	vedi nota
            'distanza_tra_fila ="3"		// opzionale default=0
            'distanza_su_fila ="2"		// opzionale default=0
            'interbina ="0"			// opzionale default=0
            'piante_per_ha="1666"		// opzionale default=0
            'piano_semina=""			// opzionale default=””
            'seme_qta=""				// opzionale default=0
            'seme_udm=""			// opzionale default=0
            'seme_lotto=""			// opzionale default=0
            'cooperativa_referente=""		// P.iva opz. default=””
            'capitolato_privato=""		// opzionale default=””
            'validita_inizio="07/10/2000"         // opzionale   default = 01/01/1900
            'validita_fine="31/12/2100" 	// opzionale   default = 31/12/2100
            'data_semina_prevista = “01/11/2011” // opzionale   default = 01/01/1900
            'data_raccolta_prevista = “31/12/2100” // opzionale   default = 31/12/2100
            'sup_imp="20" *
            'imp_chiave_cliente="001">	// opzionale 
            '</Impianto>

            '==================================================================================================
            '========  Non c'è il PROGETTO nell'xml, si usano i dati dell'impianto     ===================
            '==================================================================================================
            If CInt(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")) = enum_TipoOperazioneDB.Scrittura Then

                'non sono più gestite nella tabella impianti
                'ma sulla distinta
                Dim Piante_HA As Double = 0
                Dim Resa_Prevista As Double = 0
                Dim Data_Semina_Prevista As Date = AGRODATAINIZIO
                Dim Data_Raccolta_Prevista As Date = AGRODATAFINE

                Piante_HA = Agro_If(XML_RegImpiantoPubblico.GetAttribute("piante_per_ha"), CDbl(0))
                Resa_Prevista = Agro_If(XML_RegImpiantoPubblico.GetAttribute("resa_prevista"), CDbl(0))

                If XML_RegImpiantoPubblico.HasAttribute("data_semina_prevista") = True Then
                    Data_Semina_Prevista = XML_RegImpiantoPubblico.GetAttribute("data_semina_prevista")
                End If

                If XML_RegImpiantoPubblico.HasAttribute("data_raccolta_prevista") = True Then
                    Data_Raccolta_Prevista = XML_RegImpiantoPubblico.GetAttribute("data_raccolta_prevista")
                End If

                Dim data_inizio_impianto, data_fine_impianto As Date
                data_inizio_impianto = Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_inizio"), #1/1/1900#)
                data_fine_impianto = Agro_If(XML_RegImpiantoPubblico.GetAttribute("validita_fine"), #12/31/2100#)
                Dim lotto_progetto As String = ""

                If data_inizio_impianto <> #1/1/1900# Then

                    If data_fine_impianto <> #12/31/2100# Then

                        If CDate(data_inizio_impianto).Year <> CDate(data_fine_impianto).Year Then
                            lotto_progetto = app_nome + " - " +
                                             " Lotto " + CStr(data_inizio_impianto.Year) + "/" + CStr(data_fine_impianto.Year)
                        Else
                            lotto_progetto = app_nome + " - " + " Lotto " + CStr(data_inizio_impianto.Year)
                        End If
                    Else
                        lotto_progetto = app_nome + " - " + " Lotto " + CStr(data_inizio_impianto.Year)
                    End If
                Else
                    lotto_progetto = app_nome
                End If


                '#######################################################
                '##################    PROGETTO  #######################
                '#######################################################

                Dim DT_Codici_Progetto As DataTable
                Dim obj_XMLUtil As New AgronicaCoreXML.XML_Utility
                Dim Log_Errori As String = ""

                DT_Codici_Progetto = obj_XMLUtil.CaricaGriglia_CodiciProgetto_for_XML()

                If XML_RegImpiantoPubblico.HasAttribute("cooperativa_referente") = True Then

                    Dim Org_Referente As String
                    Org_Referente = Agro_If(XML_RegImpiantoPubblico.GetAttribute("cooperativa_referente"), "")

                    If Org_Referente <> "" Then

                        obj_XMLUtil.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                            DT_Codici_Progetto,
                                            CInt(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")),
                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                            CInt(Id_Reg),
                                            0,
                                            enum_CodiciAnagrafe.Organismo_Referente,
                                             Org_Referente,
                                             data_inizio_impianto,
                                            data_fine_impianto)

                    End If

                End If 'org referente

                If XML_RegImpiantoPubblico.HasAttribute("capitolato_privato") = True Then

                    Dim Capitolato_Privato As String
                    Capitolato_Privato = Agro_If(XML_RegImpiantoPubblico.GetAttribute("capitolato_privato"), "")

                    If Capitolato_Privato <> "" Then

                        obj_XMLUtil.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                            DT_Codici_Progetto,
                                            CInt(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")),
                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                            CInt(Id_Reg),
                                            0,
                                            enum_CodiciAnagrafe.Capitolato_Privato,
                                             Capitolato_Privato,
                                             data_inizio_impianto,
                                            data_fine_impianto)

                    End If

                End If 'capitolato privato

                ' VAnni: 10/9/2018: Spostato salvataggio dei codici kpin, blockname su distinta
                If CInt(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")) = enum_TipoOperazioneDB.Scrittura AndAlso
                XML_RegImpiantoPubblico.HasAttribute("zespri_blockname") = True Then

                    If CStr(XML_RegImpiantoPubblico.GetAttribute("zespri_blockname")) <> "#" Then

                        obj_XMLUtil.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                            DT_Codici_Progetto,
                                             enum_TipoOperazioneDB.Scrittura,
                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                            CInt(Id_Reg),
                                            CInt(XML_ProgettoPubblico.GetAttribute("codice_progetto")),
                                            enum_CodiciAnagrafe.Zespri_Block_Name,
                                             XML_RegImpiantoPubblico.GetAttribute("zespri_blockname"),
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_fine"), #12/31/2100#))

                    End If


                    If CStr(XML_RegImpiantoPubblico.GetAttribute("zespri_kpin")) <> "#" Then

                        obj_XMLUtil.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                            DT_Codici_Progetto,
                                             enum_TipoOperazioneDB.Scrittura,
                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                            CInt(Id_Reg),
                                            CInt(XML_ProgettoPubblico.GetAttribute("codice_progetto")),
                                            enum_CodiciAnagrafe.Zespri_Codice_kPIN,
                                             XML_RegImpiantoPubblico.GetAttribute("zespri_kpin"),
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_fine"), #12/31/2100#))


                    End If

                End If


                Dim Stato_Impianto As String
                If XML_RegImpiantoPubblico.HasAttribute("stato_impianto") = True Then
                    Stato_Impianto = Agro_If(XML_RegImpiantoPubblico.GetAttribute("stato_impianto"), enum_Stato_Impianto.Impianto_Produzione)
                Else
                    Stato_Impianto = enum_Stato_Impianto.Impianto_Produzione
                End If

                Dim Cod_Regolamento As String

                If XML_RegImpiantoPubblico.HasAttribute("codice_regolamento") = True Then
                    Cod_Regolamento = Agro_If(XML_RegImpiantoPubblico.GetAttribute("codice_regolamento"), CInt(1))
                Else
                    Cod_Regolamento = "1"
                End If


                Dim obj_XML As New AgronicaCoreXML.XML_Anagrafe
                Dim XmlDocProg As XmlDocument

                XML_ProgettoPrivato = obj_XML.XML_ProgettoImpianto(Log_Errori,
                                                    XmlDocProg,
                                                    BaseCode,
                                                    TopCode,
                                                    DT_Codici_Progetto,
                                                    CInt(XML_RegImpiantoPubblico.GetAttribute("tipo_operazione")),
                                                    CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                    CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                     CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                                    CInt(Id_Reg),
                                                    0,
                                                    lotto_progetto,
                                                    lotto_progetto,
                                                    CAU_PROGETTO_PRODUZIONE,
                                                    data_inizio_impianto,
                                                    data_fine_impianto,
                                                    0,
                                                    0,
                                                    0,
                                                    Resa_Prevista,
                                                     "",
                                                    0,
                                                    0,
                                                     0,
                                                    Stato_Impianto,
                                                    Cod_Regolamento,
                                                    0,
                                                    0,
                                                    0,
                                                    Data_Semina_Prevista,
                                                    Data_Raccolta_Prevista,
                                                    Piante_HA)

                XML_Nodo = XML_DatiProgetto.OwnerDocument.ImportNode(XML_ProgettoPrivato, True)

                XML_DatiProgetto.AppendChild(XML_Nodo)

            End If

        Else

            '============================================================
            '========       PROGETTO PRESENTE     ===================
            '============================================================

            '< Progetto	tipo_operazione="1" *
            'codice_progetto="0" *               // se <> 0 è progetto_cod
            'descrizione_progetto = "Progetto 2007"
            'codice_regolamento="7" 		// opzionale default=1 	vedi nota
            'codice_disciplinare="1" 		// opzionale default=1 	vedi nota
            'cooperativa_referente=""		// P.iva opz. default=””
            'capitolato_privato=""		// opzionale default=”” 
            'stato_impianto="102" 		// opz. default=102 nota        
            'resa_prevista="2"        	// opzionale default=0(kg/ha)
            'piante_per_ha = “0”			// opzionale default=0
            'data_semina_prevista = “01/11/2011” // opzionale   default = 01/01/1900
            'data_raccolta_prevista = “31/12/2100” // opzionale   default = 31/12/2100
            'validita_inizio="07/10/2000"        // opzionale   default = 01/01/1900
            'validita_fine="31/12/2100" 	// opzionale   default = 31/12/2100
            '</ Progetto>


            Dim obj_XML As New AgronicaCoreXML.XML_Anagrafe
            Dim XmlDocProg As XmlDocument
            Dim DT_Codici_Progetto As DataTable
            Dim obj_XMLUtil As New AgronicaCoreXML.XML_Utility
            Dim Log_Errori As String = ""

            'str_Progetto = ""
            Dim i_10 As Integer
            Dim Piante_HA As Double
            Dim Resa_Prevista As Double
            Dim Data_Semina_Prevista As Date
            Dim Data_Fioritura_Prevista As Date
            Dim Data_Raccolta_Prevista As Date
            Dim Disciplinare_PubblicoPrivato As Integer
            Dim Regolamento_Concimazioni_Cod As Integer


            '==================================================================================================
            '========  PROGETTO presente nell'xml     ===================
            '==================================================================================================

            For i_10 = 0 To XMLs_ProgettoPubblico.Count - 1

                XML_ProgettoPubblico = XMLs_ProgettoPubblico(i_10)

                DT_Codici_Progetto = Nothing
                DT_Codici_Progetto = obj_XMLUtil.CaricaGriglia_CodiciProgetto_for_XML()

                If XML_ProgettoPubblico.HasAttribute("cooperativa_referente") = True Then

                    Dim Org_Referente As String
                    Org_Referente = Agro_If(XML_ProgettoPubblico.GetAttribute("cooperativa_referente"), "")

                    If Org_Referente <> "" Then

                        obj_XMLUtil.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                            DT_Codici_Progetto,
                                            enum_TipoOperazioneDB.Scrittura,
                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                            CInt(Id_Reg),
                                            CInt(XML_ProgettoPubblico.GetAttribute("codice_progetto")),
                                            enum_CodiciAnagrafe.Organismo_Referente,
                                             Org_Referente,
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_fine"), #12/31/2100#))

                    End If

                End If 'org referente

                If XML_ProgettoPubblico.HasAttribute("capitolato_privato") = True Then

                    Dim Capitolato_Privato As String
                    Capitolato_Privato = Agro_If(XML_ProgettoPubblico.GetAttribute("capitolato_privato"), "")

                    If Capitolato_Privato <> "" Then

                        obj_XMLUtil.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                            DT_Codici_Progetto,
                                             enum_TipoOperazioneDB.Scrittura,
                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                            CInt(Id_Reg),
                                            CInt(XML_ProgettoPubblico.GetAttribute("codice_progetto")),
                                            enum_CodiciAnagrafe.Capitolato_Privato,
                                             Capitolato_Privato,
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_fine"), #12/31/2100#))

                    End If

                End If 'capitolato privato

                If XML_ProgettoPubblico.HasAttribute("magazzino_conferimento") = True Then

                    Dim Mag_conferimento As String
                    Mag_conferimento = Agro_If(XML_ProgettoPubblico.GetAttribute("magazzino_conferimento"), "")

                    If Mag_conferimento <> "" Then

                        obj_XMLUtil.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                            DT_Codici_Progetto,
                                             enum_TipoOperazioneDB.Scrittura,
                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                            CInt(Id_Reg),
                                            CInt(XML_ProgettoPubblico.GetAttribute("codice_progetto")),
                                            enum_CodiciAnagrafe.Magazzino_Conferimento,
                                             Mag_conferimento,
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_fine"), #12/31/2100#))

                    End If

                End If 'Mag_conferimento

                If XML_ProgettoPubblico.HasAttribute("limite_n") = True Then

                    Dim limite_n As String = Agro_If(XML_ProgettoPubblico.GetAttribute("limite_n"), "")

                    If limite_n <> "" Then

                        obj_XMLUtil.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                            DT_Codici_Progetto,
                                             enum_TipoOperazioneDB.Scrittura,
                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                            CInt(Id_Reg),
                                            CInt(XML_ProgettoPubblico.GetAttribute("codice_progetto")),
                                            enum_CodiciAnagrafe.Impianto_LimiteN,
                                             limite_n,
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_fine"), #12/31/2100#))

                    End If

                End If 'limite_n

                If XML_ProgettoPubblico.HasAttribute("limite_p") = True Then

                    Dim limite_p As String = Agro_If(XML_ProgettoPubblico.GetAttribute("limite_p"), "")

                    If limite_p <> "" Then

                        obj_XMLUtil.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                            DT_Codici_Progetto,
                                             enum_TipoOperazioneDB.Scrittura,
                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                            CInt(Id_Reg),
                                            CInt(XML_ProgettoPubblico.GetAttribute("codice_progetto")),
                                            enum_CodiciAnagrafe.Impianto_LimiteP,
                                             limite_p,
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_fine"), #12/31/2100#))

                    End If

                End If 'limite_p

                If XML_ProgettoPubblico.HasAttribute("limite_k") = True Then

                    Dim limite_k As String = Agro_If(XML_ProgettoPubblico.GetAttribute("limite_k"), "")

                    If limite_k <> "" Then

                        obj_XMLUtil.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                            DT_Codici_Progetto,
                                             enum_TipoOperazioneDB.Scrittura,
                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                            CInt(Id_Reg),
                                            CInt(XML_ProgettoPubblico.GetAttribute("codice_progetto")),
                                            enum_CodiciAnagrafe.Impianto_LimiteK,
                                             limite_k,
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_fine"), #12/31/2100#))

                    End If

                End If 'limite_k

                If XML_ProgettoPubblico.HasAttribute("limite_mg") = True Then

                    Dim limite_mg As String = Agro_If(XML_ProgettoPubblico.GetAttribute("limite_mg"), "")

                    If limite_mg <> "" Then

                        obj_XMLUtil.Inserisci_Riga_Dt_CodiciProgetto_for_XML(
                                            DT_Codici_Progetto,
                                             enum_TipoOperazioneDB.Scrittura,
                                            CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                            CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                            CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                            CInt(Id_Reg),
                                            CInt(XML_ProgettoPubblico.GetAttribute("codice_progetto")),
                                            enum_CodiciAnagrafe.Impianto_LimiteMg,
                                             limite_mg,
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_inizio"), #1/1/1900#),
                                            Agro_If(XML_ProgettoPubblico.GetAttribute("validita_fine"), #12/31/2100#))

                    End If

                End If 'limite_mg

                Dim Stato_Impianto As String
                If XML_ProgettoPubblico.HasAttribute("stato_impianto") = True Then
                    Stato_Impianto = Agro_If(XML_ProgettoPubblico.GetAttribute("stato_impianto"), enum_Stato_Impianto.Impianto_Produzione)
                Else
                    Stato_Impianto = enum_Stato_Impianto.Impianto_Produzione
                End If

                Piante_HA = 0
                Resa_Prevista = 0
                Data_Semina_Prevista = AGRODATAINIZIO
                Data_Fioritura_Prevista = AGRODATAINIZIO
                Data_Raccolta_Prevista = AGRODATAFINE
                Disciplinare_PubblicoPrivato = 0
                Regolamento_Concimazioni_Cod = 0

                If XML_ProgettoPubblico.HasAttribute("piante_per_ha") = True Then
                    Piante_HA = XML_ProgettoPubblico.GetAttribute("piante_per_ha")
                End If

                If XML_ProgettoPubblico.HasAttribute("resa_prevista") = True Then
                    Resa_Prevista = XML_ProgettoPubblico.GetAttribute("resa_prevista")
                End If

                If XML_ProgettoPubblico.HasAttribute("data_semina_prevista") = True Then
                    Data_Semina_Prevista = XML_ProgettoPubblico.GetAttribute("data_semina_prevista")
                End If

                If XML_ProgettoPubblico.HasAttribute("data_fioritura_prevista") = True Then
                    Data_Fioritura_Prevista = XML_ProgettoPubblico.GetAttribute("data_fioritura_prevista")
                End If

                If XML_ProgettoPubblico.HasAttribute("data_raccolta_prevista") = True Then
                    Data_Raccolta_Prevista = XML_ProgettoPubblico.GetAttribute("data_raccolta_prevista")
                End If

                If XML_ProgettoPubblico.HasAttribute("disciplinare_pubblicoprivato") = True Then
                    Disciplinare_PubblicoPrivato = XML_ProgettoPubblico.GetAttribute("disciplinare_pubblicoprivato")
                End If

                If XML_ProgettoPubblico.HasAttribute("regolamento_concimazioni_cod") = True Then
                    Regolamento_Concimazioni_Cod = XML_ProgettoPubblico.GetAttribute("regolamento_concimazioni_cod")
                End If

                '#######################################################
                '##################   PROGETTO    ######################
                '#######################################################

                Dim Cod_Regolamento As String
                Dim Cod_Disciplinare As String
                Dim Validita_Inizio, Validita_Fine As String

                If XML_ProgettoPubblico.HasAttribute("codice_regolamento") = True Then
                    Cod_Regolamento = Agro_If(XML_ProgettoPubblico.GetAttribute("codice_regolamento"), CInt(1))
                Else
                    Cod_Regolamento = "1"
                End If

                If XML_ProgettoPubblico.HasAttribute("codice_disciplinare") = True Then
                    Cod_Disciplinare = Agro_If(XML_ProgettoPubblico.GetAttribute("codice_disciplinare"), CInt(1))
                Else
                    Cod_Disciplinare = "1"
                End If

                If XML_ProgettoPubblico.HasAttribute("validita_inizio") = True Then
                    Validita_Inizio = Agro_If(XML_ProgettoPubblico.GetAttribute("validita_inizio"), AGRODATAINIZIO)
                Else
                    Validita_Inizio = AGRODATAINIZIO
                End If

                If XML_ProgettoPubblico.HasAttribute("validita_fine") = True Then
                    Validita_Fine = Agro_If(XML_ProgettoPubblico.GetAttribute("validita_fine"), AGRODATAFINE)
                Else
                    Validita_Fine = AGRODATAFINE
                End If

                XML_ProgettoPrivato = obj_XML.XML_ProgettoImpianto(Log_Errori,
                                                                    XmlDocProg,
                                                                    BaseCode,
                                                                    TopCode,
                                                                    DT_Codici_Progetto,
                                                                    CInt(XML_ProgettoPubblico.GetAttribute("tipo_operazione")),
                                                                    CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                                    CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                                     CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                                                    CInt(Id_Reg),
                                                                   CInt(XML_ProgettoPubblico.GetAttribute("codice_progetto")),
                                                                    CStr(XML_ProgettoPubblico.GetAttribute("descrizione_progetto")),
                                                                    CStr(XML_ProgettoPubblico.GetAttribute("descrizione_progetto")),
                                                                    CAU_PROGETTO_PRODUZIONE,
                                                                    Validita_Inizio,
                                                                   Validita_Fine,
                                                                     0,
                                                                    0,
                                                                    0,
                                                                    Resa_Prevista,
                                                                    "",
                                                                    0,
                                                                    0,
                                                                    0,
                                                                    Stato_Impianto,
                                                                    Cod_Regolamento,
                                                                    Cod_Disciplinare,
                                                                    Disciplinare_PubblicoPrivato,
                                                                    Regolamento_Concimazioni_Cod,
                                                                    Data_Semina_Prevista,
                                                                    Data_Raccolta_Prevista,
                                                                    Piante_HA,
                                                                    Data_Fioritura_Prevista)

                'se ci sono dei codici legati alla distinta
                'e se sono in modifica
                If DT_Codici_Progetto.Rows.Count > 0 And
                    CInt(XML_ProgettoPubblico.GetAttribute("tipo_operazione")) = enum_TipoOperazioneDB.Modifica Then

                    Codici_toDelete_Distinta(HT_Piva_objCodici,
                                              CStr(XML_ImpresaPubblico.GetAttribute("partita_iva")),
                                                CInt(XML_CentroAziendalePubblico.GetAttribute("codice_centro")),
                                                CInt(XML_AppezzamentoPubblico.GetAttribute("codice_appezzamento")),
                                                CInt(Id_Reg),
                                                CInt(XML_ProgettoPubblico.GetAttribute("codice_progetto")))

                End If

                ''impostato direttamente sopra
                ''se sono in modifica imposto cmq i codici con tipo operazione scrittura (prima vengono cancellati)
                'If CInt(XML_ProgettoPubblico.GetAttribute("tipo_operazione")) = enum_TipoOperazioneDB.Modifica Then
                '    Dim xmlAnagHlp.XML_Codice_list As XmlNodeList
                '    Dim xml_distinta_cod As XmlElement
                '    Dim i As Integer
                '    xmlAnagHlp.XML_Codice_list = XML_ProgettoPrivato.SelectNodes("CodiceImpianto")
                '    For i = 0 To xmlAnagHlp.XML_Codice_list.Count - 1
                '        xml_distinta_cod = xmlAnagHlp.XML_Codice_list.Item(i)
                '        xml_distinta_cod.SetAttribute("TipoOperazioneDB", CStr(enum_TipoOperazioneDB.Scrittura))
                '    Next
                'End If

                'PROGETTO

                'importo IL PROGETTO IN DATI-PROGETTO
                XML_Nodo = XML_DatiProgetto.OwnerDocument.ImportNode(XML_ProgettoPrivato, True)

                'appendo PROGETTO A DATI-PROGETTO 
                XML_DatiProgetto.AppendChild(XML_Nodo)


            Next 'CICLO SUI PROGETTI


        End If


        Return XML_DatiProgetto


    End Function



End Class
