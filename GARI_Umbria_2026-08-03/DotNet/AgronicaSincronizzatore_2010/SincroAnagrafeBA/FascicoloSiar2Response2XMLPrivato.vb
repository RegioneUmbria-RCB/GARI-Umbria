Imports System.Xml
Imports AgronicaCoreXML
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreUtility
Imports System.Transactions
Imports AgronicaCoreAnagrafeDAL
Imports System.Globalization
Imports System.Data.Entity
Imports System.Data.Entity.Core

Public Class FascicoloSiar2Response2XMLPrivato

    Public Shared Function Convert2XMLPrivato(pivaSuperUser As String, source As FascicoloSiar2Response, destination As XDocument) As XmlDocument
        Dim XmlDoc As New XmlDocument
        Try
            If XmlDoc Is Nothing Then
                XmlDoc = New XmlDocument
            End If
            Dim xmlGenerator As New XML_Privato_Anagrafe2
            If source.messaggioRisposta IsNot Nothing Then
                'Posso importare i Dati
                If source.messaggioRisposta.cod = "000" Then

                    'Inserisco DATI IMPRESA
                    Dim Errori As String = ""
                    Dim BaseCode As Integer = 0
                    Dim TopCode As Integer = 0
                    Dim TipoOperazioneDB As Integer = AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                    Dim Piva As String = source.datiAnagrafici.partitaIva
                    Dim Rag_Soc As String = source.datiAnagrafici.ragioneSociale
                    Dim PivaPadre As String = pivaSuperUser
                    Dim TipoImpresaGerarchia As Integer = 0
                    Dim Delega As String = ""
                    Dim AT_Prevalente As String = ""
                    Dim Forma_Giuridica As String = source.datiAnagrafici.formaGiuridicaDescr
                    Dim Forma_Conduzione As String = ""
                    Dim Sup_Totale As Decimal = 0
                    Dim Note As String = ""
                    Dim Validita_Inizio As Date = AGRODATAINIZIO
                    Dim Validita_Fine As Date = strToDatetime(source.datiAnagrafici.dtCessazione)
                    Dim Data_Creazione As DateTime = Now.Date
                    Dim Data_Modifica As DateTime = strToDatetime(source.datiAnagrafici.dtVariazione)
                    Dim Username_Creazione As String = pivaSuperUser
                    Dim Username_Modifica As String = pivaSuperUser
                    Dim Validazione As String = ""
                    Dim Data_Validazione As String = ""
                    Dim UserName_Validazione As String = ""
                    Dim Blk_Flag As String = ""
                    Dim Blk_Inizio_Data As DateTime = Nothing
                    Dim Blk_Inizio_Username As String = ""
                    Dim Blk_Inizio_Note As String = ""
                    Dim Blk_Fine_Data As DateTime = Nothing
                    Dim Blk_Fine_Username As String = ""
                    Dim Blk_Fine_Note As String = ""
                    Dim codEsenzione As String = source.datiAnagrafici.codEsenzione
                    Dim codOp As String = source.datiAnagrafici.codOp
                    Dim documento As String = source.datiAnagrafici.documento
                    Dim dtDocumento As DateTime = strToDatetime(source.datiAnagrafici.dtDocumento)
                    Dim dtValidazione As DateTime = strToDatetime(source.datiAnagrafici.dtValidazione)
                    Dim esenzioneDescr As String = source.datiAnagrafici.esenzioneDescr
                    Dim flagAltreSedi As String = validaBooleano(source.datiAnagrafici.flagAltreSedi)
                    Dim flagValidato As String = validaBooleano(source.datiAnagrafici.flagValidato)
                    Dim idUtenteValidazione As String = source.datiAnagrafici.idUtenteValidazione
                    Dim opDescr As String = source.datiAnagrafici.opDescr
                    Dim fonte As String = ""
                    Dim dt_Fonte As DateTime = strToDatetime(Nothing)
                    Dim aziendaCessata As String = validaBooleano(source.statoAzienda.aziendaCessata)
                    Dim aziendaIscrittaCAA As String = validaBooleano(source.statoAzienda.aziendaIscrittaCaa)
                    Dim aziendaPresente As String = validaBooleano(source.statoAzienda.aziendaPresente)
                    Dim aziendaValidata As String = validaBooleano(source.statoAzienda.aziendaValidata)
                    Dim dataIscrizioneCAA As DateTime = strToDatetime(source.statoAzienda.dataIscrizioneCaa)
                    Dim dataValidazione As DateTime = strToDatetime(source.statoAzienda.dataValidazione)
                    Dim dataVariazioneAzienda As DateTime = strToDatetime(source.statoAzienda.dataVariazioneAzienda)
                    Dim maxDataVariazioneIbanAzienda As DateTime = strToDatetime(source.statoAzienda.maxDataVariazioneIbanAzienda)
                    Dim maxDataVariazionePersoneAzienda As DateTime = strToDatetime(source.statoAzienda.maxDataVariazionePersoneAzienda)
                    Dim maxDataVariazionePossessiAzienda As DateTime = strToDatetime(source.statoAzienda.maxDataVariazionePossessiAzienda)

                    Dim impresa = xmlGenerator.XML_2_Impresa_Impresa(Errori, XmlDoc, BaseCode, TopCode, TipoOperazioneDB, Piva, Rag_Soc, PivaPadre, TipoImpresaGerarchia, Delega, AT_Prevalente, Forma_Giuridica, Forma_Conduzione, Sup_Totale, Note, Validita_Inizio, Validita_Fine, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validazione, Data_Validazione, UserName_Validazione, Blk_Flag, Blk_Inizio_Data, Blk_Inizio_Username, Blk_Inizio_Note, Blk_Fine_Data, Blk_Fine_Username, Blk_Fine_Note, codEsenzione, codOp, documento)
                    If source.datiAnagrafici.cuaa IsNot Nothing AndAlso source.datiAnagrafici.cuaa <> "" Then
                        Dim datiAnagraficiCuaa = xmlGenerator.XML_Campo_Codice(Errori, AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, Piva, 0, 1010, source.datiAnagrafici.cuaa, 0, 0, XmlDoc, 0, AGRODATAINIZIO, AGRODATAFINE)
                        impresa.AppendChild(datiAnagraficiCuaa)
                    End If
                    If source.datiAnagrafici.provRea IsNot Nothing AndAlso source.datiAnagrafici.provRea <> "" AndAlso
                        source.datiAnagrafici.numRea IsNot Nothing AndAlso source.datiAnagrafici.numRea <> "" Then
                        impresa.AppendChild(xmlGenerator.XML_Campo_Codice(Errori, AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, Piva, 0, 1112, source.datiAnagrafici.provRea + " - " + source.datiAnagrafici.numRea, 0, 0, XmlDoc, 0, AGRODATAINIZIO, AGRODATAFINE))
                    End If

                    'CITTA ESTERA LEGALE
                    If source.datiAnagrafici.cittaEsteraLegale IsNot Nothing Then
                        xmlGenerator.XML_2_Indirizzo(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                     1, "", source.datiAnagrafici.cittaEsteraLegale.citta, XmlDoc, 0, "", "", "", source.datiAnagrafici.cittaEsteraLegale.codStato, source.datiAnagrafici.cittaEsteraLegale.statoDescr)
                    End If

                    'CONTATTI
                    If source.datiAnagrafici.contatti IsNot Nothing Then
                        impresa.AppendChild(xmlGenerator.XML_2_Rubrica(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                   0,
                                                   source.datiAnagrafici.contatti.email,
                                                   "Email", XmlDoc))

                        impresa.AppendChild(xmlGenerator.XML_2_Rubrica(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                   0,
                                                   source.datiAnagrafici.contatti.fax,
                                                   "Fax", XmlDoc))

                        impresa.AppendChild(xmlGenerator.XML_2_Rubrica(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                   0,
                                                   source.datiAnagrafici.contatti.pec,
                                                   "Email", XmlDoc))

                        impresa.AppendChild(xmlGenerator.XML_2_Rubrica(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                   0,
                                                   source.datiAnagrafici.contatti.telefono,
                                                   "Telefono", XmlDoc))

                    End If

                    'SEDE LEGALE
                    If source.datiAnagrafici.sedeLegale IsNot Nothing Then
                        'CITTA ESTERA SEDE LEGALE
                        If source.datiAnagrafici.sedeLegale.cittaEstera IsNot Nothing Then
                            xmlGenerator.XML_2_Indirizzo(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                     1, "", source.datiAnagrafici.sedeLegale.cittaEstera.citta, XmlDoc, 0, "", "", "", source.datiAnagrafici.sedeLegale.cittaEstera.codStato, source.datiAnagrafici.sedeLegale.cittaEstera.statoDescr)
                        End If
                        xmlGenerator.XML_2_Indirizzo(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                     1,
                                                     source.datiAnagrafici.sedeLegale.codProv,
                                                     source.datiAnagrafici.sedeLegale.codCom,
                                                     XmlDoc, 0, source.datiAnagrafici.sedeLegale.indirizzo,
                                                     source.datiAnagrafici.sedeLegale.localita,
                                                     source.datiAnagrafici.sedeLegale.cap, "ITALIA")
                    End If


                    'Inserisco ISCRIZIONECAA
                    If source.iscrizioneCAA IsNot Nothing Then
                        Dim iscrizioneCAA = xmlGenerator.XML_2_IscrizioneCAA(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                    Piva,
                                                    source.iscrizioneCAA.denominazione,
                                                    source.iscrizioneCAA.codFiscaleCAA,
                                                    source.iscrizioneCAA.idCAA,
                                                    0, 0,
                                                    DateTime.Now,
                                                    DateTime.Now,
                                                    DateTime.Now,
                                                    pivaSuperUser,
                                                    pivaSuperUser,
                                                    DateTime.Now,
                                                    DateTime.Now,
                                                    XmlDoc)

                        Dim contattoCaa = xmlGenerator.XML_2_Indirizzo(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                       1, source.iscrizioneCAA.codProvinciaCAA,
                                                                       source.iscrizioneCAA.codComuneCAA,
                                                                       XmlDoc, 0,
                                                                       source.iscrizioneCAA.indirizzoCAA, "",
                                                                       source.iscrizioneCAA.capCAA, "IT", "",
                                                                       AGRODATAINIZIO, AGRODATAFINE)
                        iscrizioneCAA.AppendChild(contattoCaa)
                        impresa.AppendChild(iscrizioneCAA)
                    End If

                    If source.produzioniQualita IsNot Nothing AndAlso
                        source.produzioniQualita.produzione IsNot Nothing Then
                        For Each Prod In source.produzioniQualita.produzione
                            impresa.AppendChild(xmlGenerator.XML_2_ProduzioniQualita(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                         Piva,
                                                         Prod.codMacroarea,
                                                         Prod.codProduzione,
                                                         Prod.descrMacroArea,
                                                         Prod.descrProduzione,
                                                         Nothing,
                                                         Nothing,
                                                         Nothing,
                                                         0,
                                                         Now.Date,
                                                         Now.Date,
                                                         Now.Date,
                                                         pivaSuperUser,
                                                         pivaSuperUser,
                                                         AGRODATAINIZIO,
                                                         AGRODATAFINE,
                                                         XmlDoc))
                        Next
                    End If

                    If source.diritti IsNot Nothing AndAlso
                        source.diritti.diritto IsNot Nothing Then
                        For Each dirdir In source.diritti.diritto
                            xmlGenerator.XML_2_DirittiReimpianti(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                 Piva,
                                                                 dirdir.codAutorizzazione,
                                                                 strToDatetime(dirdir.dataProtocollo),
                                                                 strToDatetime(dirdir.dataRilascio),
                                                                 strToDatetime(dirdir.dataTermine),
                                                                 strToDatetime(dirdir.dtIns),
                                                                 strToDatetime(dirdir.dtVar),
                                                                 strToInteger(dirdir.numDiritto),
                                                                 dirdir.numeroProtocollo,
                                                                 dirdir.praticaAutorizzata,
                                                                 dirdir.provRilascio,
                                                                 dirdir.provRilascioDescr,
                                                                 dirdir.provRilascioSigla,
                                                                 strToInteger(dirdir.supAutorizzata),
                                                                 strToInteger(dirdir.supImpiantata),
                                                                 strToInteger(dirdir.supResidua),
                                                                 dirdir.tipoDiritto,
                                                                 dirdir.tipoDirittoDescr,
                                                                 dirdir.tipoProcedimento,
                                                                 0,
                                                                 DateTime.Now,
                                                                 DateTime.Now,
                                                                 DateTime.Now,
                                                                 pivaSuperUser,
                                                                 pivaSuperUser,
                                                                 AGRODATAINIZIO,
                                                                 AGRODATAFINE,
                                                                 XmlDoc)





                        Next
                    End If

                    If source.personeRuoli IsNot Nothing AndAlso
                        source.personeRuoli.persone IsNot Nothing Then
                        For Each persona In source.personeRuoli.persone
                            Dim Cod_Contatto As String = persona.codiceFiscale
                            Dim Sa_Cod As Integer = Sa_Cod
                            Dim Id_CF As Integer = 0
                            Dim Rag_SocPersona As String = ""
                            Dim Convenevoli As String = ""
                            Dim Codice_Fiscale As String = persona.codiceFiscale
                            Dim Tipo_Indirizzo_Default As Integer = 0
                            Dim Nome As String = persona.nome
                            Dim Cognome As String = persona.cognome
                            Dim Data_Nascita As Date = persona.datiNascita.dataNascita
                            Dim Sesso As String = persona.sesso
                            Dim Cod_Contatto_Referente As String = ""
                            Dim Validita_InizioPersona As Date = AGRODATAINIZIO
                            Dim Validita_FinePersona As Date = AGRODATAFINE
                            Dim BaseCodePersona As Integer = 0
                            Dim TopCodePersona As Integer = 200000000
                            Dim Cod_Risum_Destinazione_Diversa As Integer = 0
                            Dim Tipo_Indirizzo_Default_Destinazione_Diversa As Integer = 0
                            Dim Tipo_Speditore As Integer = 0
                            Dim Tipo_Destinazione As Integer = 0
                            Dim Agente_Cod As Integer = 0
                            Dim Provvigione As Double = 0
                            Dim NotePersona As String = ""
                            Dim Id_Gestione_Note As Integer = 0
                            Dim Note2 As String = ""
                            Dim Note_Operazioni As String = ""
                            Dim Note2_Operazioni As String = ""
                            Dim Fido As Double = 0
                            Dim Limite_Posizioni As Integer = 0
                            Dim Limite_Giorni_Evasione As Double = 0
                            Dim Orari_Ritiro As String = ""
                            Dim Filtro_Rimborsi As String = ""
                            Dim Vettore_Cod As Integer = 0
                            Dim CapoArea_Cod As Integer = 0
                            Dim Provvigione_CapoArea As Double = 0
                            Dim DocumentoPersona As String = persona.documento
                            Dim dtDocumentoPersona As DateTime = strToDatetime(persona.dtDocumento)
                            Dim dtFonte As DateTime = Nothing
                            Dim dtVariazioneRuolo As DateTime = strToDatetime(persona.dtVariazione)
                            Dim flagReferente As String = persona.flagReferente
                            Dim fontePersona As String = persona.fonte
                            Dim fonteDescr As String = persona.fonteDescr
                            Dim AlboProfessionale As String = ""
                            Dim AlboProfessionaleDescr As String = ""
                            Dim numeroIscrizione As Nullable(Of Integer) = Nothing
                            Dim Qualifica As String = ""
                            Dim QualificaDescr As String = ""
                            Dim TitoloStudio As String = ""
                            Dim TitoloStudioDescr As String = ""
                            Dim contatto = xmlGenerator.XML_2_Contatto_Contatto(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                Piva, Cod_Contatto, XmlDoc, Sa_Cod, Id_CF, Rag_Soc, Convenevoli, Codice_Fiscale, Tipo_Indirizzo_Default, Nome, Cognome,
                                Data_Nascita, Sesso, Cod_Contatto_Referente, Validita_InizioPersona, Validita_FinePersona, BaseCodePersona,
                                TopCodePersona, Cod_Risum_Destinazione_Diversa, Tipo_Indirizzo_Default_Destinazione_Diversa, Tipo_Speditore,
                                Tipo_Destinazione, Agente_Cod, Provvigione, NotePersona, Id_Gestione_Note, Note2, Note_Operazioni, Note2_Operazioni,
                                Fido, Limite_Posizioni, Limite_Giorni_Evasione, Orari_Ritiro, Filtro_Rimborsi, Vettore_Cod, CapoArea_Cod,
                                Provvigione_CapoArea, DocumentoPersona, dtDocumentoPersona, dtFonte, flagReferente, fontePersona, fonteDescr,
                                AlboProfessionale, AlboProfessionaleDescr, numeroIscrizione, Qualifica, QualificaDescr, TitoloStudio, TitoloStudioDescr)

                            contatto.AppendChild(xmlGenerator.XML_2_RisorseUmane(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                            Piva, 0, 0, "", 0, AGRODATAINIZIO, AGRODATAFINE, "", "", 0, 0, 0, 0, 0, 0,
                                                            0, 0, Now.Date, Now.Date, Now.Date, pivaSuperUser, pivaSuperUser, "", Nothing, Nothing,
                                                            0, "", "", 0, 0, 0, 0, "", 0, 0, "", persona.ruolo.codRuolo, persona.ruolo.ruoloDescr,
                                                            dtVariazioneRuolo, persona.ruolo.fonte, persona.ruolo.fonteDescr, XmlDoc))

                            contatto.AppendChild(xmlGenerator.XML_2_Rubrica(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, 0, persona.contatti.email, "Email", XmlDoc))
                            contatto.AppendChild(xmlGenerator.XML_2_Rubrica(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, 0, persona.contatti.fax, "Fax", XmlDoc))
                            contatto.AppendChild(xmlGenerator.XML_2_Rubrica(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, 0, persona.contatti.pec, "Pec", XmlDoc))
                            contatto.AppendChild(xmlGenerator.XML_2_Rubrica(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, 0, persona.contatti.telefono, "Telefono", XmlDoc))


                        Next
                    End If

                    'CREO UNITA AZIENDALE FITTIZIA PER ALLEVAMENTI E PARTICELLE
                    Dim erroriFittizio As String = ""
                    Dim centroAziendaleFittizio = xmlGenerator.XML_2_CentroAziendale_CentroAziendale(erroriFittizio,
                                                                                                     XmlDoc,
                                                                                                     0,
                                                                                                     200000,
                                                                                                     AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                                                     Piva,
                                                                                                     0,
                                                                                                     "",
                                                                                                     0,
                                                                                                     0,
                                                                                                     0,
                                                                                                     0,
                                                                                                     0,
                                                                                                     0,
                                                                                                     "",
                                                                                                     "",
                                                                                                     "",
                                                                                                     0,
                                                                                                     0,
                                                                                                     0,
                                                                                                     0,
                                                                                                     0,
                                                                                                     0,
                                                                                                     0,
                                                                                                     0,
                                                                                                     0,
                                                                                                     0,
                                                                                                     AGRODATAINIZIO,
                                                                                                     AGRODATAFINE,
                                                                                                     "",
                                                                                                     "",
                                                                                                     "",
                                                                                                     "",
                                                                                                     pivaSuperUser,
                                                                                                     pivaSuperUser,
                                                                                                     "",
                                                                                                     "",
                                                                                                     "",
                                                                                                     "",
                                                                                                     "",
                                                                                                     "",
                                                                                                     "",
                                                                                                     "",
                                                                                                     "",
                                                                                                     "")


                    If source.allevamenti IsNot Nothing AndAlso
                        source.allevamenti.allevamento IsNot Nothing Then
                        For Each all In source.allevamenti.allevamento
                            Dim erroriFabbr As String = ""
                            Dim fabbricato = xmlGenerator.XML_2_Fabbricato_Fabbricato(erroriFabbr,
                                                                                      XmlDoc,
                                                                                      0,
                                                                                      2000000,
                                                                                      AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                                      Piva,
                                                                                      0,
                                                                                      0,
                                                                                      all.idAllevamento + " - " + all.allevamentoDescr,
                                                                                      0,
                                                                                      20)

                            xmlGenerator.XML_2_Fabbricato_Codice(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, Piva, 0, 0, 2028, 0, 20000000, XmlDoc)

                            fabbricato.AppendChild(xmlGenerator.XML_2_Indirizzo(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                                1, "", all.comCodice, XmlDoc, 0, all.indirizzo, all.localita, all.cap))
                            fabbricato.AppendChild(xmlGenerator.XML_2_Stalla(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                             Piva,
                                                                             0,
                                                                             0,
                                                                             all.codAllevamento,
                                                                             "",
                                                                             Nothing,
                                                                             Nothing,
                                                                             0,
                                                                             0,
                                                                             0,
                                                                             0,
                                                                             "",
                                                                             "",
                                                                             Nothing,
                                                                             Nothing,
                                                                             Nothing,
                                                                             all.idFiscale,
                                                                             "",
                                                                             "",
                                                                             "",
                                                                             XmlDoc,
                                                                             ,
                                                                             ,
                                                                             all.aziendaCodice,
                                                                             all.allevamentoDescr,
                                                                             all.codAllevamento,
                                                                             all.speCodice))



                        Next
                    End If

                    If source.conduzioniTerreni IsNot Nothing AndAlso
                        source.conduzioniTerreni.particelle IsNot Nothing Then
                        For Each particella In source.conduzioniTerreni.particelle
                            If particella.conduzione.contratti IsNot Nothing Then
                                For Each contratto In particella.conduzione.contratti
                                    'Ho almeno un contratto
                                    Dim particellaXml = xmlGenerator.XML_Particella_Particella(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                           Piva,
                                           0,
                                           particella.codProv,
                                           particella.codCom, particella.sezione, strToInteger(particella.foglio), strToInteger(particella.particella), particella.subalterno, 0, 0, 0, 0, 0, Now.Date, AGRODATAFINE,
                                           AGRODATAINIZIO, AGRODATAFINE, 0, "", 0, "", 0, 0, 0, , XmlDoc, particella.casiParticolari, particella.conduzione.fasciaAltimetrica,
                                           particella.conduzione.fasciaAltimetricaDescr, particella.fonte, particella.fonteDescr, particella.tipoDocumento, particella.tipoDocumentoDescr,
                                           particella.utilizzo, particella.idParticella, particella.conduzione.irriguo, particella.conduzione.rotazioneColturale,
                                           particella.conduzione.biologico, particella.conduzione.flagAnomaliaMacrouso, particella.conduzione.flagContenzioso,
                                           particella.conduzione.flagSupero, particella.conduzione.percEleggibile)

                                    particellaXml.AppendChild(xmlGenerator.XML_2_CentriAziendalixParticelle(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                                  0, Piva, 0, particella.codProv, particella.codCom, particella.sezione, strToInteger(particella.foglio), strToInteger(particella.particella),
                                                                                  particella.subalterno, 0, "", 0, strToDatetime(particella.dataInserimento), strToDatetime(particella.dataInizio), strToDatetime(particella.dataVariazione),
                                                                                  pivaSuperUser, pivaSuperUser, strToDatetime(particella.dataInizio), strToDatetime(particella.dataFine), 0, Nothing, "",
                                                                                  strToInteger(particella.conduzione.totSupPossesso), 0, 0, "", "", "", "", "", "", XmlDoc))

                                    particellaXml.AppendChild(xmlGenerator.XML_2_ParticelleCatastalixEleggiblitaParticelle(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                                                 particella.codProv, particella.codCom, particella.sezione, particella.foglio, particella.particella,
                                                                                                 particella.subalterno, 0, particella.conduzione.supEleggibile, 0, Now.Date, Now.Date, Now.Date,
                                                                                                 pivaSuperUser, pivaSuperUser, AGRODATAINIZIO, AGRODATAFINE, particella.conduzione.percEleggibile, XmlDoc))

                                    If particella.conduzione.macrousi IsNot Nothing Then
                                        For Each macrouso In particella.conduzione.macrousi
                                            particellaXml.AppendChild(xmlGenerator.XML_2_ParticelleCatastalixMacrousi(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                                                                      particella.codProv, particella.codCom, particella.sezione, particella.foglio,
                                                                                                                      particella.particella, particella.subalterno, macrouso.codMacrouso,
                                                                                                                      macrouso.supMacrouso, 0, Now.Date, Now.Date, Now.Date, pivaSuperUser,
                                                                                                                      pivaSuperUser, AGRODATAINIZIO, AGRODATAFINE, 0, Piva, "", Nothing,
                                                                                                                      macrouso.fonte, macrouso.fonteDescr, XmlDoc))
                                        Next
                                    End If

                                    If particella.conduzione.unitaVitate IsNot Nothing Then
                                        For Each unita In particella.conduzione.unitaVitate
                                            Dim unitaVitataProg = xmlGenerator.XML_2_Programmazione_Entita(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                                                               pivaSuperUser, 0, 0, "", Piva, 0, 0, 0, 0, 0, "", 0, 0, 0, 0, 0,
                                                                                                               strToDouble(unita.supVitataDich), 0, "", 0, 0, 0, 0, 0,
                                                                                                               Now.Date, Now.Date, pivaSuperUser, pivaSuperUser, AGRODATAINIZIO, AGRODATAFINE,
                                                                                                               0, 0, 0, 0, Nothing, Nothing, "", "", "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, "", "", unita.idUnitaVitata,
                                                                                                               Nothing, strToInteger(unita.altitudineSlm), strToInteger(unita.altriVitigniPresenti), strToInteger(unita.ancoraggiTestata), strToInteger(unita.annoRiferimento),
                                                                                                               unita.codFiliSostegno, unita.codPaliTessitura, unita.codPaliTestata, unita.codStatoColt, unita.codTipoVari,
                                                                                                               strToDatetime(unita.dataProtocollo), strToDatetime(unita.dataRilievo), unita.destProduttiva, unita.destProduttivaDescr,
                                                                                                               strToDouble(unita.distanzaPali), strToDatetime(unita.dtFine), strToDatetime(unita.dtFineGestione), strToDatetime(unita.dtInizio), strToDatetime(unita.dtInizioGestione),
                                                                                                               strToDatetime(unita.dtIns), strToDatetime(unita.dtVar), strToDouble(unita.fallanzePerc), validaBooleano(unita.flagAnomalia), validaBooleano(unita.flagAttuale), validaBooleano(unita.flagCessata),
                                                                                                               validaBooleano(unita.flagContributo), validaBooleano(unita.flagRegolarizz2009), validaBooleano(unita.flagRicalcoloGis), unita.giacituraTerreno, strToInteger(unita.idUnitaVitata),
                                                                                                               unita.idUtenteIns, unita.idUtenteVar, unita.numeroProtocollo, unita.progPoligono, strToInteger(unita.supVitataDich),
                                                                                                               strToInteger(unita.supVitataDichPRicalcolo), strToInteger(unita.superficieServizioMq), strToInteger(unita.terrazzamenti), unita.tipoColtura,
                                                                                                               unita.tipoProcedimento, unita.tipoUnar, unita.tipoVariazione, unita.unar, XmlDoc)


                                            If unita.altriVitigni IsNot Nothing Then
                                                For Each altroVit In unita.altriVitigni
                                                    unitaVitataProg.AppendChild(xmlGenerator.XML_2_AltraSpecie_Impianti(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                                            Piva, 0, 0, 0, 0, 0, altroVit.codVitigno, altroVit.descrVitigno, strToDatetime(altroVit.dtIns),
                                                                                            altroVit.idUnitaVitata, altroVit.perc, altroVit.progr, "1", 0, Now.Date, Now.Date, Now.Date,
                                                                                            pivaSuperUser, pivaSuperUser, AGRODATAINIZIO, AGRODATAFINE))

                                                Next
                                            End If

                                            If unita.idoneita IsNot Nothing Then
                                                For Each idon In unita.idoneita
                                                    unitaVitataProg.AppendChild(xmlGenerator.XML_2_Idoneita_Appezzamento(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, Piva, 0, 0, 0, 0, strToDatetime(idon.dataRev),
                                                                                             strToDatetime(idon.dataRic), idon.docigtDescr, strToDatetime(idon.dtInizio), strToDatetime(idon.dtIns), strToDatetime(idon.dtVar), idon.idDocigt, idon.idUnitaVitata,
                                                                                             idon.idUtenteIns, idon.idUtenteVar, strToInteger(idon.numIscrizione), idon.tipologiaDescr, 0, Now.Date, Now.Date, Now.Date, pivaSuperUser,
                                                                                             pivaSuperUser, AGRODATAINIZIO, AGRODATAFINE, XmlDoc))
                                                Next
                                            End If
                                            particellaXml.AppendChild(unitaVitataProg)
                                        Next
                                    End If

                                    If particella.conduzione.proprietari IsNot Nothing Then
                                        For Each prop In particella.conduzione.proprietari
                                            particellaXml.AppendChild(xmlGenerator.XML_2_ImpresexParticelle_Contatti(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, 0, 0, 0,
                                                                                                                     Piva, 0, particella.codProv, particella.codCom, particella.sezione, particella.foglio,
                                                                                                                     particella.particella, particella.subalterno, 0, 0, Nothing, Now.Date, Now.Date,
                                                                                                                     pivaSuperUser, pivaSuperUser, AGRODATAINIZIO, AGRODATAFINE, prop.cuaaProprietario, XmlDoc))
                                        Next
                                    End If

                                    If particella.conduzione.zone IsNot Nothing Then
                                        For Each zona In particella.conduzione.zone
                                            particellaXml.AppendChild(xmlGenerator.XML_2_ZonexParticelle(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, Piva, zona.codZona,
                                                                                                         particella.codProv, particella.codCom, particella.sezione, particella.foglio, particella.particella,
                                                                                                         particella.subalterno, "", 0, Now.Date, Now.Date, Now.Date, pivaSuperUser, pivaSuperUser, AGRODATAINIZIO,
                                                                                                         AGRODATAFINE, zona.fonte, zona.fonteDescr, zona.conforme, XmlDoc))


                                        Next
                                    End If
                                    centroAziendaleFittizio.AppendChild(particellaXml)
                                Next
                            Else
                                'Non ho contratti registrati
                                Dim particellaXml = xmlGenerator.XML_Particella_Particella(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                                           Piva,
                                                                                           0,
                                                                                           particella.codProv,
                                                                                           particella.codCom, particella.sezione, strToInteger(particella.foglio), strToInteger(particella.particella), particella.subalterno, 0, 0, 0, 0, 0, Now.Date, AGRODATAFINE,
                                                                                           AGRODATAINIZIO, AGRODATAFINE, 0, "", 0, "", 0, 0, 0, , XmlDoc, particella.casiParticolari, particella.conduzione.fasciaAltimetrica,
                                                                                           particella.conduzione.fasciaAltimetricaDescr, particella.fonte, particella.fonteDescr, particella.tipoDocumento, particella.tipoDocumentoDescr,
                                                                                           particella.utilizzo, particella.idParticella, particella.conduzione.irriguo, particella.conduzione.rotazioneColturale,
                                                                                           particella.conduzione.biologico, particella.conduzione.flagAnomaliaMacrouso, particella.conduzione.flagContenzioso,
                                                                                           particella.conduzione.flagSupero, particella.conduzione.percEleggibile)

                                particellaXml.AppendChild(xmlGenerator.XML_2_CentriAziendalixParticelle(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                              0, Piva, 0, particella.codProv, particella.codCom, particella.sezione, strToInteger(particella.foglio), strToInteger(particella.particella),
                                                                              particella.subalterno, 0, "", 0, strToDatetime(particella.dataInserimento), strToDatetime(particella.dataInizio), strToDatetime(particella.dataVariazione),
                                                                              pivaSuperUser, pivaSuperUser, strToDatetime(particella.dataInizio), strToDatetime(particella.dataFine), 0, Nothing, "",
                                                                              strToInteger(particella.conduzione.totSupPossesso), 0, 0, "", "", "", "", "", "", XmlDoc))

                                particellaXml.AppendChild(xmlGenerator.XML_2_ParticelleCatastalixEleggiblitaParticelle(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                                             particella.codProv, particella.codCom, particella.sezione, particella.foglio, particella.particella,
                                                                                             particella.subalterno, 0, particella.conduzione.supEleggibile, 0, Now.Date, Now.Date, Now.Date,
                                                                                             pivaSuperUser, pivaSuperUser, AGRODATAINIZIO, AGRODATAFINE, particella.conduzione.percEleggibile, XmlDoc))

                                If particella.conduzione.macrousi IsNot Nothing Then
                                    For Each macrouso In particella.conduzione.macrousi
                                        particellaXml.AppendChild(xmlGenerator.XML_2_ParticelleCatastalixMacrousi(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                                                                  particella.codProv, particella.codCom, particella.sezione, particella.foglio,
                                                                                                                  particella.particella, particella.subalterno, macrouso.codMacrouso,
                                                                                                                  macrouso.supMacrouso, 0, Now.Date, Now.Date, Now.Date, pivaSuperUser,
                                                                                                                  pivaSuperUser, AGRODATAINIZIO, AGRODATAFINE, 0, Piva, "", Nothing,
                                                                                                                  macrouso.fonte, macrouso.fonteDescr, XmlDoc))
                                    Next
                                End If

                                If particella.conduzione.unitaVitate IsNot Nothing Then
                                    For Each unita In particella.conduzione.unitaVitate
                                        Dim unitaVitataProg = xmlGenerator.XML_2_Programmazione_Entita(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                                                           pivaSuperUser, 0, 0, "", Piva, 0, 0, 0, 0, 0, "", 0, 0, 0, 0, 0,
                                                                                                           strToDouble(unita.supVitataDich), 0, "", 0, 0, 0, 0, 0,
                                                                                                           Now.Date, Now.Date, pivaSuperUser, pivaSuperUser, AGRODATAINIZIO, AGRODATAFINE,
                                                                                                           0, 0, 0, 0, Nothing, Nothing, "", "", "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, "", "", unita.idUnitaVitata,
                                                                                                           Nothing, strToInteger(unita.altitudineSlm), strToInteger(unita.altriVitigniPresenti), strToInteger(unita.ancoraggiTestata), strToInteger(unita.annoRiferimento),
                                                                                                           unita.codFiliSostegno, unita.codPaliTessitura, unita.codPaliTestata, unita.codStatoColt, unita.codTipoVari,
                                                                                                           strToDatetime(unita.dataProtocollo), strToDatetime(unita.dataRilievo), unita.destProduttiva, unita.destProduttivaDescr,
                                                                                                           strToDouble(unita.distanzaPali), strToDatetime(unita.dtFine), strToDatetime(unita.dtFineGestione), strToDatetime(unita.dtInizio), strToDatetime(unita.dtInizioGestione),
                                                                                                           strToDatetime(unita.dtIns), strToDatetime(unita.dtVar), strToDouble(unita.fallanzePerc), validaBooleano(unita.flagAnomalia), validaBooleano(unita.flagAttuale), validaBooleano(unita.flagCessata),
                                                                                                           validaBooleano(unita.flagContributo), validaBooleano(unita.flagRegolarizz2009), validaBooleano(unita.flagRicalcoloGis), unita.giacituraTerreno, strToInteger(unita.idUnitaVitata),
                                                                                                           unita.idUtenteIns, unita.idUtenteVar, unita.numeroProtocollo, unita.progPoligono, strToInteger(unita.supVitataDich),
                                                                                                           strToInteger(unita.supVitataDichPRicalcolo), strToInteger(unita.superficieServizioMq), strToInteger(unita.terrazzamenti), unita.tipoColtura,
                                                                                                           unita.tipoProcedimento, unita.tipoUnar, unita.tipoVariazione, unita.unar, XmlDoc)


                                        If unita.altriVitigni IsNot Nothing Then
                                            For Each altroVit In unita.altriVitigni
                                                unitaVitataProg.AppendChild(xmlGenerator.XML_2_AltraSpecie_Impianti(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura,
                                                                                        Piva, 0, 0, 0, 0, 0, altroVit.codVitigno, altroVit.descrVitigno, strToDatetime(altroVit.dtIns),
                                                                                        altroVit.idUnitaVitata, altroVit.perc, altroVit.progr, "1", 0, Now.Date, Now.Date, Now.Date,
                                                                                        pivaSuperUser, pivaSuperUser, AGRODATAINIZIO, AGRODATAFINE))

                                            Next
                                        End If

                                        If unita.idoneita IsNot Nothing Then
                                            For Each idon In unita.idoneita
                                                unitaVitataProg.AppendChild(xmlGenerator.XML_2_Idoneita_Appezzamento(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, Piva, 0, 0, 0, 0, strToDatetime(idon.dataRev),
                                                                                         strToDatetime(idon.dataRic), idon.docigtDescr, strToDatetime(idon.dtInizio), strToDatetime(idon.dtIns), strToDatetime(idon.dtVar), idon.idDocigt, idon.idUnitaVitata,
                                                                                         idon.idUtenteIns, idon.idUtenteVar, strToInteger(idon.numIscrizione), idon.tipologiaDescr, 0, Now.Date, Now.Date, Now.Date, pivaSuperUser,
                                                                                         pivaSuperUser, AGRODATAINIZIO, AGRODATAFINE, XmlDoc))
                                            Next
                                        End If
                                    Next
                                End If

                                If particella.conduzione.proprietari IsNot Nothing Then
                                    For Each prop In particella.conduzione.proprietari
                                        particellaXml.AppendChild(xmlGenerator.XML_2_ImpresexParticelle_Contatti(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, 0, 0, 0,
                                                                                                                 Piva, 0, particella.codProv, particella.codCom, particella.sezione, particella.foglio,
                                                                                                                 particella.particella, particella.subalterno, 0, 0, Nothing, Now.Date, Now.Date,
                                                                                                                 pivaSuperUser, pivaSuperUser, AGRODATAINIZIO, AGRODATAFINE, prop.cuaaProprietario, XmlDoc))
                                    Next
                                End If

                                If particella.conduzione.zone IsNot Nothing Then
                                    For Each zona In particella.conduzione.zone
                                        particellaXml.AppendChild(xmlGenerator.XML_2_ZonexParticelle(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, Piva, zona.codZona,
                                                                                                     particella.codProv, particella.codCom, particella.sezione, particella.foglio, particella.particella,
                                                                                                     particella.subalterno, "", 0, Now.Date, Now.Date, Now.Date, pivaSuperUser, pivaSuperUser, AGRODATAINIZIO,
                                                                                                     AGRODATAFINE, zona.fonte, zona.fonteDescr, zona.conforme, XmlDoc))


                                    Next
                                End If
                                centroAziendaleFittizio.AppendChild(particellaXml)
                            End If

                        Next

                    End If
                    impresa.AppendChild(centroAziendaleFittizio)
                    If source.unitaAziendali IsNot Nothing AndAlso
                        source.unitaAziendali.unita IsNot Nothing Then
                        For Each uni In source.unitaAziendali.unita
                            Dim logErrori As String = ""

                            Dim centroAziendale = xmlGenerator.XML_2_CentroAziendale_CentroAziendale(logErrori, XmlDoc, 0, 200000, AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, Piva, 0, "", 0, 0, 0, 0, 0, 0, "", "", "", 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", uni.dataInizio, uni.dataVariazione, pivaSuperUser, pivaSuperUser, "", "", "", uni.documento, uni.dataDocumento, uni.flagLegale, uni.flagPrincipale, uni.fonte, uni.fonteDescr, uni.dataFonte)

                            centroAziendale.AppendChild(xmlGenerator.XML_2_Rubrica(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, 0, uni.contatti.email, "Email", XmlDoc))
                            centroAziendale.AppendChild(xmlGenerator.XML_2_Rubrica(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, 0, uni.contatti.fax, "Fax", XmlDoc))
                            centroAziendale.AppendChild(xmlGenerator.XML_2_Rubrica(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, 0, uni.contatti.pec, "Pec", XmlDoc))
                            centroAziendale.AppendChild(xmlGenerator.XML_2_Rubrica(AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB.Scrittura, 0, uni.contatti.telefono, "Telefono", XmlDoc))

                            impresa.AppendChild(centroAziendale)

                        Next
                    End If



                    XmlDoc.AppendChild(impresa)
                End If

            End If

        Catch ex As Exception

        End Try

        Return XmlDoc
    End Function

    Public Shared Function Convert2EntityFrameworkModel(source As FascicoloSiar2Response,
                                                        objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                        ByVal ControlExist As Boolean,
                                                        Optional ByRef returnStr As String = "") As Boolean
        'Using scope As New TransactionScope()
        'Using scope As New TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(90))
        Dim startMillis As Long = DateTime.Now.Ticks
        Dim numParticella As Integer = 0
        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility

        Dim pivaSuperUser As String = objParametri_Utenti.PivaSuperUser
        Dim SuperUserUsername As String = objParametri_Utenti.SuperUserUsername

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim dal As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(EFConnString)
        'dal.ContextOptions.UseLegacyPreserveChangesBehavior = False
        dal.Database.Connection.Open()
        'disabilitaMergeOptions(dal)


        dal.Database.ExecuteSqlCommand("SET ARITHABORT ON;")

        Dim idGen = New AgronicaCoreDataProvider.Agro_Sequenze
        Dim validazione As Integer

        Dim piva As String
        If source.datiAnagrafici.partitaIva Is Nothing Or source.datiAnagrafici.partitaIva = "" Then
            piva = source.datiAnagrafici.cuaa.Substring(0, 11)
            validazione = 1
        Else
            piva = source.datiAnagrafici.partitaIva
            validazione = 0
        End If

        Dim dataValidazione As DateTime

        Dim impresaExist = (From im As Imprese In dal.Imprese Where im.PIVA = piva).Count
        Dim imp As AgronicaCoreEntityFramework_POCO.Imprese
        Dim datavalidazione_impresa As DateTime = AGRODATAINIZIO


        Dim dataValiazioneStr As String = source.statoAzienda.dataValidazione
        If source.statoAzienda.dataValidazione Is Nothing Then
            dataValiazioneStr = source.datiAnagrafici.dtValidazione
        End If
        dataValidazione = CDate(dataValiazioneStr)
        If impresaExist <> 0 Then
            imp = (From im As Imprese In dal.Imprese Where im.PIVA = piva).FirstOrDefault
            If ControlExist Then
                If imp.dataValidazione IsNot Nothing Then
                    datavalidazione_impresa = imp.dataValidazione
                End If
                If datavalidazione_impresa = dataValidazione Then
                    Return False
                End If
            End If
            If (From gi In dal.GerarchiaImprese Where gi.Figlio = piva).Count = 0 Then
                EFImprese.CreateGerarchiaImprese(dal, objParametri, pivaSuperUser, piva, 1, 2, pivaSuperUser, objParametri_Utenti)
            End If
            If (From ui In dal.UtentiXImprese Where ui.PIVA = piva).Count = 0 Then
                EFImprese.CreateUtentixImprese(dal, objParametri, pivaSuperUser, piva, pivaSuperUser)
            End If
        Else
            imp = EFImprese.CreaImpresaCompleta(dal, objParametri, piva, source.datiAnagrafici.ragioneSociale, "", AGRODATAINIZIO, SuperUserUsername, pivaSuperUser, objParametri_Utenti)
        End If

        If source.statoAzienda.dataCessazione IsNot Nothing AndAlso source.statoAzienda.dataCessazione <> "" Then
            imp.Validita_Fine = strToDatetime(source.statoAzienda.dataCessazione)
        Else
            imp.Validita_Fine = AGRODATAFINE
        End If
        imp.aziendaValidata = validaBooleano(source.statoAzienda.aziendaValidata)
        imp.aziendaCessata = validaBooleano(source.statoAzienda.aziendaCessata)
        imp.aziendaIscrittaCAA = validaBooleano(source.statoAzienda.aziendaIscrittaCaa)
        imp.aziendaPresente = validaBooleano(source.statoAzienda.aziendaPresente)
        imp.dataIscrizioneCAA = strToDatetime(source.statoAzienda.dataIscrizioneCaa)
        imp.Validazione = validazione
        imp.dataValidazione = dataValidazione
        imp.dataVariazioneAzienda = strToDatetime(source.statoAzienda.dataVariazioneAzienda)
        imp.maxDataVariazioneIbanAzienda = strToDatetime(source.statoAzienda.maxDataVariazioneIbanAzienda)
        imp.maxDataVariazionePersoneAzienda = strToDatetime(source.statoAzienda.maxDataVariazionePersoneAzienda)
        imp.maxDataVariazionePossessiAzienda = strToDatetime(source.statoAzienda.maxDataVariazionePossessiAzienda)
        imp.codEsenzione = source.datiAnagrafici.codEsenzione
        imp.codOp = source.datiAnagrafici.codOp
        imp.documento = source.datiAnagrafici.documento
        imp.dtDocumento = strToDatetime(source.datiAnagrafici.dtDocumento)
        If source.datiAnagrafici.dtCessazione IsNot Nothing AndAlso source.datiAnagrafici.dtCessazione <> "" Then
            imp.Validita_Fine = strToDatetime(source.datiAnagrafici.dtCessazione)
        Else
            imp.Validita_Fine = AGRODATAFINE
        End If
        imp.dtValidazione = strToDatetime(source.datiAnagrafici.dtValidazione)
        imp.Data_Modifica = strToDatetime(source.datiAnagrafici.dtVariazione)
        imp.esenzioneDescr = source.datiAnagrafici.esenzioneDescr
        imp.flagAltreSedi = validaBooleano(source.datiAnagrafici.flagAltreSedi)
        imp.flagValidato = validaBooleano(source.datiAnagrafici.flagValidato)
        imp.Forma_Giuridica = source.datiAnagrafici.formaGiuridica
        imp.idUtenteValidazione = source.datiAnagrafici.idUtenteValidazione
        imp.opDescr = source.datiAnagrafici.opDescr

        'imp.MarkAsModified()
        dal.Entry(imp).State = EntityState.Modified

        dal.SaveChanges()

        If source.datiAnagrafici.cuaa IsNot Nothing AndAlso source.datiAnagrafici.cuaa <> "" Then
            Dim impCod As Imprese_Codici
            If (From ic As Imprese_Codici In dal.Imprese_Codici Where ic.PIVA = piva And ic.id_cod = 1010).Count = 0 Then
                impCod = AgronicaCoreAnagrafeDAL.EFImprese.CreateImprese_CodiciEF(dal, objParametri, imp, 1010, source.datiAnagrafici.cuaa, SuperUserUsername)
            Else
                impCod = (From ic As Imprese_Codici In dal.Imprese_Codici Where ic.PIVA = piva And ic.id_cod = 1010).FirstOrDefault
                impCod.val_cod = source.datiAnagrafici.cuaa
                'impCod.MarkAsModified()
                dal.Entry(impCod).State = EntityState.Modified
            End If
            dal.SaveChanges()
        End If

        If source.datiAnagrafici.provRea IsNot Nothing AndAlso source.datiAnagrafici.provRea <> "" AndAlso
                        source.datiAnagrafici.numRea IsNot Nothing AndAlso source.datiAnagrafici.numRea <> "" Then
            Dim impCod As Imprese_Codici
            If (From ic As Imprese_Codici In dal.Imprese_Codici Where ic.PIVA = piva And ic.id_cod = 1112).Count = 0 Then
                impCod = AgronicaCoreAnagrafeDAL.EFImprese.CreateImprese_CodiciEF(dal, objParametri, imp, 1112, source.datiAnagrafici.provRea + " - " + source.datiAnagrafici.numRea, SuperUserUsername)
            Else
                impCod = (From ic As Imprese_Codici In dal.Imprese_Codici Where ic.PIVA = piva And ic.id_cod = 1112).FirstOrDefault
                impCod.val_cod = source.datiAnagrafici.provRea + " - " + source.datiAnagrafici.numRea
                'impCod.MarkAsModified()
                dal.Entry(impCod).State = EntityState.Modified
            End If
            dal.SaveChanges()
        End If



        If source.datiAnagrafici.cittaEsteraLegale IsNot Nothing Then
            If source.datiAnagrafici.cittaEsteraLegale.statoDescr IsNot Nothing AndAlso
                source.datiAnagrafici.cittaEsteraLegale.citta IsNot Nothing AndAlso
                source.datiAnagrafici.cittaEsteraLegale.statoDescr <> "" AndAlso
                source.datiAnagrafici.cittaEsteraLegale.citta <> "" Then
                Dim indirizzo1 As Indirizzi
                Dim impxIndi As ImpresexIndirizzi
                If (From indi As ImpresexIndirizzi In dal.ImpresexIndirizzi Join ind As Indirizzi In dal.Indirizzi On indi.cod_indirizzo Equals ind.cod_indirizzo Where indi.PIVA = piva And indi.Tipo_Indirizzo = 1).Count = 0 Then
                    indirizzo1 = AgronicaCoreAnagrafeDAL.EFImprese.CreateIndirizzoImpresa(dal, objParametri, imp, 1, SuperUserUsername)
                Else
                    indirizzo1 = (From indi As ImpresexIndirizzi In dal.ImpresexIndirizzi Join ind As Indirizzi In dal.Indirizzi On indi.cod_indirizzo Equals ind.cod_indirizzo Where indi.PIVA = piva And indi.Tipo_Indirizzo = 1).FirstOrDefault.ind
                End If
                indirizzo1.stato = source.datiAnagrafici.cittaEsteraLegale.statoDescr
                indirizzo1.com_des = source.datiAnagrafici.cittaEsteraLegale.citta
                dal.Entry(indirizzo1).State = EntityState.Modified 'indirizzo1.MarkAsModified()
                dal.SaveChanges()
                'dal.AddObject("ImpresexIndirizzi", impxind)
            End If
        End If

        dal.SaveChanges()

        'SEDE LEGALE
        If source.datiAnagrafici.sedeLegale IsNot Nothing Then
            'CITTA ESTERA SEDE LEGALE
            If source.datiAnagrafici.sedeLegale.cittaEstera IsNot Nothing Then
                If source.datiAnagrafici.sedeLegale.cittaEstera.statoDescr IsNot Nothing AndAlso
                source.datiAnagrafici.sedeLegale.cittaEstera.citta IsNot Nothing AndAlso
                source.datiAnagrafici.sedeLegale.cittaEstera.statoDescr <> "" AndAlso
                source.datiAnagrafici.sedeLegale.cittaEstera.citta <> "" Then
                    'Dim cod_indirizzo1 = newID("Indirizzi", objParametri)
                    Dim indirizzo1 As Indirizzi
                    Dim impxIndi As ImpresexIndirizzi
                    If (From indi As ImpresexIndirizzi In dal.ImpresexIndirizzi Join ind As Indirizzi In dal.Indirizzi On indi.cod_indirizzo Equals ind.cod_indirizzo Where indi.PIVA = piva And indi.Tipo_Indirizzo = 1).Count = 0 Then
                        indirizzo1 = AgronicaCoreAnagrafeDAL.EFImprese.CreateIndirizzoImpresa(dal, objParametri, imp, 1, SuperUserUsername)
                    Else
                        indirizzo1 = (From indi As ImpresexIndirizzi In dal.ImpresexIndirizzi Join ind As Indirizzi In dal.Indirizzi On indi.cod_indirizzo Equals ind.cod_indirizzo Where indi.PIVA = piva And indi.Tipo_Indirizzo = 1).FirstOrDefault.ind
                    End If
                    indirizzo1.stato = source.datiAnagrafici.sedeLegale.cittaEstera.statoDescr
                    indirizzo1.com_des = source.datiAnagrafici.sedeLegale.cittaEstera.citta
                    dal.Entry(indirizzo1).State = EntityState.Modified 'indirizzo1.MarkAsModified()
                    dal.SaveChanges()
                End If
            End If
            Dim indirizzo As Indirizzi
            If (From indi As ImpresexIndirizzi In dal.ImpresexIndirizzi Where indi.PIVA = piva And indi.Tipo_Indirizzo = 1).Count = 0 Then
                indirizzo = AgronicaCoreAnagrafeDAL.EFImprese.CreateIndirizzoImpresa(dal, objParametri, imp, 1, SuperUserUsername)
            Else
                indirizzo = (From indi As ImpresexIndirizzi In dal.ImpresexIndirizzi
                             Join inin In dal.Indirizzi On indi.cod_indirizzo Equals inin.cod_indirizzo
                             Where indi.PIVA = piva And indi.Tipo_Indirizzo = 1
                             Select inin).FirstOrDefault
            End If
            indirizzo.pro_cod_istat = source.datiAnagrafici.sedeLegale.codProv
            indirizzo.com_cod_istat = source.datiAnagrafici.sedeLegale.codCom
            If source.datiAnagrafici.sedeLegale.codProv <> "" Then
                indirizzo.pro_cod = (From istat As ISTAT In dal.ISTAT Where istat.PROV = source.datiAnagrafici.sedeLegale.codProv).FirstOrDefault.COMUNI_PROV
            End If
            indirizzo.ind_des = source.datiAnagrafici.sedeLegale.indirizzo
            indirizzo.com_des = source.datiAnagrafici.sedeLegale.localita
            indirizzo.CAP = source.datiAnagrafici.sedeLegale.cap
            If source.datiAnagrafici.sedeLegale.cittaEstera.statoDescr <> "" Then
                indirizzo.stato = source.datiAnagrafici.sedeLegale.cittaEstera.statoDescr
            Else
                indirizzo.stato = "IT"
            End If
            indirizzo.Username_Creazione = SuperUserUsername
            indirizzo.Username_Modifica = SuperUserUsername
            'If source.datiAnagrafici.sedeLegale.localita Is Nothing Then
            '    If source.datiAnagrafici.sedeLegale.codCom <> "" Then
            '        indirizzo.com_des = (From istat As ISTAT_Comuni In dal.ISTAT_Comuni Where istat.Com_Cod_Istat = source.datiAnagrafici.sedeLegale.codCom).FirstOrDefault.Descrizione
            '    End If
            'Else
            '    If source.datiAnagrafici.sedeLegale.localita = "" Then
            '        indirizzo.com_des = (From istat As ISTAT_Comuni In dal.ISTAT_Comuni Where istat.Com_Cod_Istat = source.datiAnagrafici.sedeLegale.codCom).FirstOrDefault.Descrizione
            '    End If
            'End If

            dal.Entry(indirizzo).State = EntityState.Modified 'indirizzo.MarkAsModified()
        End If

        dal.SaveChanges()

        Dim numCaa = (From c As CAA In dal.CAA Where c.idCAA = CInt(source.iscrizioneCAA.idCAA)).Count
        Dim ca As CAA
        If numCaa = 0 Then
            ca = New CAA '.CreateCAA(source.iscrizioneCAA.codFiscaleCAA, source.iscrizioneCAA.idCAA)
            ca.codFiscaleCAA = source.iscrizioneCAA.codFiscaleCAA
            ca.idCAA = source.iscrizioneCAA.idCAA
            ca.Denominazione = source.iscrizioneCAA.denominazione
            Dim codIndirizzo = idGen.NuovoId_Tabella_EF(dal, "Indirizzi", 0, 20000000, objParametri)
            'Dim indirizzoCAA As New AgronicaCoreEntityFramework_POCO.Indirizzi
            Dim indirizzoCAA = dal.Set(Of Indirizzi)().Create() ' dal.CreateObject(Of Indirizzi)()
            indirizzoCAA.CAP = source.iscrizioneCAA.capCAA
            indirizzoCAA.com_cod_istat = source.iscrizioneCAA.codComuneCAA
            indirizzoCAA.com_des = source.iscrizioneCAA.comuneCAADescr
            indirizzoCAA.Data_Creazione = DateTime.Now
            indirizzoCAA.Data_Modifica = DateTime.Now
            indirizzoCAA.ind_des = source.iscrizioneCAA.indirizzoCAA
            indirizzoCAA.pro_cod = source.iscrizioneCAA.provinciaCAASigla
            indirizzoCAA.pro_cod_istat = source.iscrizioneCAA.codProvinciaCAA
            indirizzoCAA.cod_indirizzo = codIndirizzo
            indirizzoCAA.stato = "IT"
            indirizzoCAA.note = ""
            indirizzoCAA.inviato = 0
            indirizzoCAA.Validazione = 0
            indirizzoCAA.Data_Validazione = Now.Date
            indirizzoCAA.UserName_Validazione = ""
            indirizzoCAA.Codice_Lingua = ""
            indirizzoCAA.Codice_Alternativo = ""
            indirizzoCAA.Username_Creazione = SuperUserUsername
            indirizzoCAA.Username_Modifica = SuperUserUsername
            indirizzoCAA.Data_Creazione = DateTime.Now
            indirizzoCAA.Data_Modifica = DateTime.Now
            indirizzoCAA.Validita_Inizio = AGRODATAINIZIO
            indirizzoCAA.Validita_Fine = AGRODATAFINE
            ca.Validita_Inizio = strToDatetime(source.iscrizioneCAA.dataInizio)
            ca.Data_Modifica = strToDatetime(source.iscrizioneCAA.dataVariazione)
            ca.Data_Creazione = DateTime.Now
            ca.Indirizzi = indirizzoCAA
        Else
            ca = (From c As CAA In dal.CAA Where c.idCAA = source.iscrizioneCAA.idCAA).FirstOrDefault
        End If

        If (From iscr As IscrizioneCAA In dal.IscrizioneCAA Where iscr.PIVA = piva And iscr.CAA.idCAA = ca.idCAA).Count = 0 Then
            Dim iscrizione = New IscrizioneCAA '.CreateIscrizioneCAA(source.iscrizioneCAA.idCAA, piva, source.iscrizioneCAA.idAzienda)
            iscrizione.idCAA = source.iscrizioneCAA.idCAA
            iscrizione.codFiscaleCAA = ca.codFiscaleCAA
            iscrizione.idCAA = ca.idCAA
            iscrizione.dataInizio = strToDatetime(source.iscrizioneCAA.dataInizio)
            iscrizione.dataRichiesta = strToDatetime(source.iscrizioneCAA.dataRichiesta)
            iscrizione.dataVariazione = strToDatetime(source.iscrizioneCAA.dataVariazione)
            iscrizione.dtFonte = strToDatetime(source.iscrizioneCAA.dtFonte)
            iscrizione.fonte = source.iscrizioneCAA.fonte
            iscrizione.utenteVariazione = source.iscrizioneCAA.utenteVariazione
            iscrizione.Username_Creazione = SuperUserUsername
            iscrizione.Username_Modifica = SuperUserUsername
            iscrizione.PIVA = piva
            imp.IscrizioneCAA.Add(iscrizione)
            dal.SaveChanges()
            'dal.AddObject("IscrizioneCAA", iscrizione)
        Else
            Dim IscrizioneCAA = (From iscr As IscrizioneCAA In dal.IscrizioneCAA Where iscr.PIVA = piva And iscr.CAA.idCAA = ca.idCAA).FirstOrDefault
            IscrizioneCAA.dataInizio = strToDatetime(source.iscrizioneCAA.dataInizio)
            IscrizioneCAA.dataRichiesta = strToDatetime(source.iscrizioneCAA.dataRichiesta)
            IscrizioneCAA.dataVariazione = strToDatetime(source.iscrizioneCAA.dataVariazione)
            IscrizioneCAA.dtFonte = strToDatetime(source.iscrizioneCAA.dtFonte)
            IscrizioneCAA.fonte = source.iscrizioneCAA.fonte
            IscrizioneCAA.utenteVariazione = source.iscrizioneCAA.utenteVariazione
            IscrizioneCAA.Username_Creazione = SuperUserUsername
            IscrizioneCAA.Username_Modifica = SuperUserUsername
            dal.Entry(IscrizioneCAA).State = EntityState.Modified 'IscrizioneCAA.MarkAsModified()
        End If

        dal.SaveChanges()
        If source.produzioniQualita IsNot Nothing AndAlso
            source.produzioniQualita.produzione IsNot Nothing Then
            For Each Prod In source.produzioniQualita.produzione
                Dim prodQual As ProduzioniQualita '.CreateProduzioniQualita(piva, Prod.codMacroarea, Prod.codProduzione)
                If (From prq As ProduzioniQualita In dal.ProduzioniQualita Where prq.PIVA = piva And prq.CodMacroarea = Prod.codMacroarea And prq.CodProduzione = Prod.codProduzione).Count = 0 Then
                    prodQual = New ProduzioniQualita
                    prodQual.PIVA = piva
                    prodQual.CodMacroarea = Prod.codMacroarea
                    prodQual.CodProduzione = Prod.codProduzione
                    prodQual.Data_Creazione = Now.Date
                    prodQual.Data_Modifica = Now.Date
                    prodQual.descrMacroarea = Prod.descrMacroArea
                    prodQual.descrProduzione = Prod.descrProduzione
                    prodQual.Username_Creazione = SuperUserUsername
                    prodQual.Username_Modifica = SuperUserUsername
                    prodQual.Validita_Inizio = AGRODATAINIZIO
                    prodQual.Validita_Fine = AGRODATAFINE
                    imp.ProduzioniQualita.Add(prodQual)
                Else
                    prodQual = (From prq As ProduzioniQualita In dal.ProduzioniQualita Where prq.PIVA = piva And prq.CodMacroarea = Prod.codMacroarea And prq.CodProduzione = Prod.codProduzione).FirstOrDefault
                    prodQual.Data_Creazione = Now.Date
                    prodQual.Data_Modifica = Now.Date
                    prodQual.descrMacroarea = Prod.descrMacroArea
                    prodQual.descrProduzione = Prod.descrProduzione
                    prodQual.Username_Creazione = SuperUserUsername
                    prodQual.Username_Modifica = SuperUserUsername
                    prodQual.Validita_Inizio = AGRODATAINIZIO
                    prodQual.Validita_Fine = AGRODATAFINE
                    dal.Entry(prodQual).State = EntityState.Modified 'prodQual.MarkAsModified()
                End If
            Next
        End If
        dal.SaveChanges()

        If source.diritti IsNot Nothing AndAlso
            source.diritti.diritto IsNot Nothing Then
            For Each dirdir In source.diritti.diritto
                Dim dir As DirittiReimpianti '.CreateDirittiReimpianti(piva, dirdir.codAutorizzazione)
                If (From d As DirittiReimpianti In dal.DirittiReimpianti Where d.PIVA = piva And d.numDiritto = dirdir.numDiritto).Count = 0 Then
                    dir = New DirittiReimpianti
                    Dim idDiritto = idGen.NuovoId_Tabella_EF(dal, "DirittiReimpianti", 0, 20000000, objParametri)
                    dir.ID = idDiritto
                    dir.PIVA = piva
                    dir.codAutorizzazione = dirdir.codAutorizzazione
                    dir.Data_Creazione = Now.Date
                    dir.Data_Modifica = Now.Date
                    dir.dataProtocollo = strToDatetime(dirdir.dataProtocollo)
                    dir.dataRilascio = strToDatetime(dirdir.dataRilascio)
                    dir.dataTermine = strToDatetime(dirdir.dataTermine)
                    dir.dtIns = strToDatetime(dirdir.dtIns)
                    dir.dtVar = strToDatetime(dirdir.dtVar)
                    dir.numDiritto = dirdir.numDiritto
                    dir.numeroProtocollo = dirdir.numeroProtocollo
                    dir.praticaAutorizzata = validaBooleano(dirdir.praticaAutorizzata)
                    dir.provRilascio = dirdir.provRilascio
                    dir.provRilascioDescr = dirdir.provRilascioDescr
                    dir.provRilascioSigla = dirdir.provRilascioSigla
                    dir.supAutorizzata = strToInteger(dirdir.supAutorizzata)
                    dir.supImpiantata = strToInteger(dirdir.supImpiantata)
                    dir.supResidua = strToInteger(dirdir.supResidua)
                    dir.tipoDiritto = dirdir.tipoDiritto
                    dir.tipoDirittoDescr = dirdir.tipoDirittoDescr
                    dir.tipoProcedimento = dirdir.tipoProcedimento
                    dir.Username_Creazione = SuperUserUsername
                    dir.Username_Modifica = SuperUserUsername
                    dir.Validita_Inizio = AGRODATAINIZIO
                    dir.Validita_Fine = AGRODATAFINE
                    imp.DirittiReimpianti.Add(dir)
                Else
                    dir = (From d As DirittiReimpianti In dal.DirittiReimpianti Where d.PIVA = piva And d.numDiritto = dirdir.numDiritto).FirstOrDefault
                    dir.codAutorizzazione = dirdir.codAutorizzazione
                    dir.Data_Creazione = Now.Date
                    dir.Data_Modifica = Now.Date
                    dir.dataProtocollo = strToDatetime(dirdir.dataProtocollo)
                    dir.dataRilascio = strToDatetime(dirdir.dataRilascio)
                    dir.dataTermine = strToDatetime(dirdir.dataTermine)
                    dir.dtIns = strToDatetime(dirdir.dtIns)
                    dir.dtVar = strToDatetime(dirdir.dtVar)
                    dir.numDiritto = dirdir.numDiritto
                    dir.numeroProtocollo = dirdir.numeroProtocollo
                    dir.praticaAutorizzata = validaBooleano(dirdir.praticaAutorizzata)
                    dir.provRilascio = dirdir.provRilascio
                    dir.provRilascioDescr = dirdir.provRilascioDescr
                    dir.provRilascioSigla = dirdir.provRilascioSigla
                    dir.supAutorizzata = strToInteger(dirdir.supAutorizzata)
                    dir.supImpiantata = strToInteger(dirdir.supImpiantata)
                    dir.supResidua = strToInteger(dirdir.supResidua)
                    dir.tipoDiritto = dirdir.tipoDiritto
                    dir.tipoDirittoDescr = dirdir.tipoDirittoDescr
                    dir.tipoProcedimento = dirdir.tipoProcedimento
                    dir.Username_Creazione = SuperUserUsername
                    dir.Username_Modifica = SuperUserUsername
                    dir.Validita_Inizio = AGRODATAINIZIO
                    dir.Validita_Fine = AGRODATAFINE
                    'dir.MarkAsModified()
                    dal.Entry(dir).State = EntityState.Modified
                End If
            Next
        End If
        dal.SaveChanges()

        Dim sa_cod0 = 0
        If source.personeRuoli IsNot Nothing AndAlso
            source.personeRuoli.persone IsNot Nothing Then
            For Each persona In source.personeRuoli.persone
                Dim codContatto = persona.codiceFiscale
                Dim contatto As AgronicaCoreEntityFramework_POCO.Contatti
                Dim contattoDB = (From c As Contatti In dal.Contatti Where c.Cod_Contatto = codContatto).FirstOrDefault
                If (From c As Contatti In dal.Contatti Where c.Cod_Contatto = codContatto And c.Piva = piva).Count = 0 Then
                    contatto = AgronicaCoreAnagrafeDAL.EFImprese.CreaContattoImpresa(dal, objParametri, imp, codContatto, sa_cod0, 0, SuperUserUsername)
                Else
                    contatto = (From c As Contatti In dal.Contatti Where c.Cod_Contatto = codContatto And c.Piva = piva).FirstOrDefault
                End If
                contatto.Rag_Soc = ""
                contatto.Tipo_Indirizzo_Default = 0
                contatto.Nome = persona.nome
                contatto.Cognome = persona.cognome
                If persona.datiNascita.dataNascita IsNot Nothing AndAlso persona.datiNascita.dataNascita <> "" Then
                    contatto.Data_Nascita = strToDatetime(persona.datiNascita.dataNascita)
                End If
                contatto.Sesso = persona.sesso
                contatto.Cod_Contatto_Referente = ""


                If (From rc In dal.ContattiXRubrica
                    Join r In dal.Rubrica On rc.Cod_Rubrica Equals r.cod_rubrica
                    Where rc.Cod_Contatto = codContatto And r.descr = "Email").Count = 0 Then
                    Dim mail = AgronicaCoreAnagrafeDAL.EFContatti.CreateRubricaContatto(dal, objParametri, piva, sa_cod0, contatto, strToStrObb(persona.contatti.email), "Email", SuperUserUsername)
                Else
                    Dim m As Rubrica = (From rc In dal.ContattiXRubrica
                                        Join r In dal.Rubrica On rc.Cod_Rubrica Equals r.cod_rubrica
                                        Where rc.Cod_Contatto = codContatto And r.descr = "Email"
                                        Select r).FirstOrDefault
                    m.numero = strToStrObb(persona.contatti.email)
                    dal.Entry(m).State = EntityState.Modified 'm.MarkAsModified()

                End If

                If (From rc In dal.ContattiXRubrica
                    Join r In dal.Rubrica On rc.Cod_Rubrica Equals r.cod_rubrica
                    Where rc.Cod_Contatto = codContatto And r.descr = "Fax").Count = 0 Then
                    Dim Fax = AgronicaCoreAnagrafeDAL.EFContatti.CreateRubricaContatto(dal, objParametri, piva, sa_cod0, contatto, strToStrObb(persona.contatti.fax), "Fax", SuperUserUsername)
                Else
                    Dim m As Rubrica = (From rc In dal.ContattiXRubrica
                                        Join r In dal.Rubrica On rc.Cod_Rubrica Equals r.cod_rubrica
                                        Where rc.Cod_Contatto = codContatto And r.descr = "Fax"
                                        Select r).FirstOrDefault
                    m.numero = strToStrObb(persona.contatti.fax)
                    dal.Entry(m).State = EntityState.Modified 'm.MarkAsModified()

                End If

                If (From rc In dal.ContattiXRubrica
                    Join r In dal.Rubrica On rc.Cod_Rubrica Equals r.cod_rubrica
                    Where rc.Cod_Contatto = codContatto And r.descr = "Pec").Count = 0 Then
                    Dim pec = AgronicaCoreAnagrafeDAL.EFContatti.CreateRubricaContatto(dal, objParametri, piva, sa_cod0, contatto, strToStrObb(persona.contatti.pec), "Pec", pivaSuperUser)
                Else
                    Dim m As Rubrica = (From rc In dal.ContattiXRubrica
                                        Join r In dal.Rubrica On rc.Cod_Rubrica Equals r.cod_rubrica
                                        Where rc.Cod_Contatto = codContatto And r.descr = "Pec"
                                        Select r).FirstOrDefault
                    m.numero = strToStrObb(persona.contatti.pec)
                    dal.Entry(m).State = EntityState.Modified 'm.MarkAsModified()

                End If

                If (From rc In dal.ContattiXRubrica
                    Join r In dal.Rubrica On rc.Cod_Rubrica Equals r.cod_rubrica
                    Where rc.Cod_Contatto = codContatto And r.descr = "Telefono").Count = 0 Then
                    Dim Telefono = AgronicaCoreAnagrafeDAL.EFContatti.CreateRubricaContatto(dal, objParametri, piva, sa_cod0, contatto, strToStrObb(persona.contatti.telefono), "Telefono", SuperUserUsername)
                Else
                    Dim m As Rubrica = (From rc In dal.ContattiXRubrica
                                        Join r In dal.Rubrica On rc.Cod_Rubrica Equals r.cod_rubrica
                                        Where rc.Cod_Contatto = codContatto And r.descr = "Telefono"
                                        Select r).FirstOrDefault
                    m.numero = strToStrObb(persona.contatti.telefono)
                    dal.Entry(m).State = EntityState.Modified 'm.MarkAsModified()

                End If

                Dim indirizzo As Indirizzi
                If (From ii In dal.Indirizzi
                    Join ci In dal.ContattiXIndirizzi On ii.cod_indirizzo Equals ci.Cod_Indirizzo
                    Where ci.Cod_Contatto = codContatto And ci.Tipo_Indirizzo = 3).Count = 0 Then
                    indirizzo = AgronicaCoreAnagrafeDAL.EFContatti.CreateIndirizzoContatto(dal, objParametri, piva, sa_cod0, contatto, 3, SuperUserUsername)
                Else
                    indirizzo = (From ii In dal.Indirizzi
                                 Join ci In dal.ContattiXIndirizzi On ii.cod_indirizzo Equals ci.Cod_Indirizzo
                                 Where ci.Cod_Contatto = codContatto And ci.Tipo_Indirizzo = 3
                                 Select ii).FirstOrDefault
                End If
                indirizzo.CAP = persona.residenza.cap
                indirizzo.com_cod_istat = persona.residenza.codCom
                indirizzo.pro_cod_istat = persona.residenza.codProv
                indirizzo.ind_des = persona.residenza.indirizzo
                indirizzo.frz_des = persona.residenza.localita
                indirizzo.stato = "IT"

                If persona.residenza.codCom IsNot Nothing AndAlso persona.residenza.codCom <> "" Then
                    If (From istat As ISTAT_Comuni In dal.ISTAT_Comuni Where istat.Com_Cod_Istat = persona.residenza.codCom).Count <> 0 Then
                        indirizzo.com_des = (From istat As ISTAT_Comuni In dal.ISTAT_Comuni Where istat.Com_Cod_Istat = persona.residenza.codCom).FirstOrDefault.Descrizione
                    End If
                End If
                If persona.residenza.codProv IsNot Nothing AndAlso persona.residenza.codProv <> "" Then
                    If (From istat As ISTAT In dal.ISTAT Where istat.PROV = persona.residenza.codProv).Count <> 0 Then
                        indirizzo.pro_cod = (From istat As ISTAT In dal.ISTAT Where istat.PROV = persona.residenza.codProv).FirstOrDefault.COMUNI_PROV
                    End If
                End If

                If persona.residenza.cittaEstera IsNot Nothing Then
                    If persona.residenza.cittaEstera.citta IsNot Nothing AndAlso
                        persona.residenza.cittaEstera.codStato IsNot Nothing Then
                        If persona.residenza.cittaEstera.citta <> "" Then
                            indirizzo.com_des = persona.residenza.cittaEstera.citta
                        End If
                        If persona.residenza.cittaEstera.codStato <> "" Then
                            indirizzo.stato = persona.residenza.cittaEstera.codStato
                        End If
                    End If
                End If
                dal.Entry(indirizzo).State = EntityState.Modified 'indirizzo.MarkAsModified()


                Dim luogoNascita As Indirizzi
                If (From ii In dal.Indirizzi
                    Join ci In dal.ContattiXIndirizzi On ii.cod_indirizzo Equals ci.Cod_Indirizzo
                    Where ci.Cod_Contatto = codContatto And ci.Tipo_Indirizzo = 5).Count = 0 Then
                    luogoNascita = AgronicaCoreAnagrafeDAL.EFContatti.CreateIndirizzoContatto(dal, objParametri, piva, sa_cod0, contatto, 5, SuperUserUsername)
                Else
                    luogoNascita = (From ii In dal.Indirizzi
                                    Join ci In dal.ContattiXIndirizzi On ii.cod_indirizzo Equals ci.Cod_Indirizzo
                                    Where ci.Cod_Contatto = codContatto And ci.Tipo_Indirizzo = 5
                                    Select ii).FirstOrDefault
                End If

                luogoNascita.com_cod_istat = persona.datiNascita.codCom
                luogoNascita.pro_cod_istat = persona.datiNascita.codProv
                If persona.datiNascita.codCom <> "" Then
                    If (From istat As ISTAT_Comuni In dal.ISTAT_Comuni Where istat.Com_Cod_Istat = persona.datiNascita.codCom).Count = 0 Then
                        luogoNascita.com_des = persona.datiNascita.comDescr
                    Else
                        luogoNascita.com_des = (From istat As ISTAT_Comuni In dal.ISTAT_Comuni Where istat.Com_Cod_Istat = persona.datiNascita.codCom).FirstOrDefault.Descrizione
                    End If
                End If
                If persona.datiNascita.codProv <> "" Then
                    luogoNascita.pro_cod = (From istat As ISTAT In dal.ISTAT Where istat.PROV = persona.datiNascita.codProv).FirstOrDefault.COMUNI_PROV
                End If

                If persona.datiNascita.cittaEsteraNascita IsNot Nothing Then
                    If persona.datiNascita.cittaEsteraNascita.citta IsNot Nothing AndAlso persona.datiNascita.cittaEsteraNascita.citta <> "" Then
                        luogoNascita.com_des = persona.datiNascita.cittaEsteraNascita.citta
                    End If
                    If persona.datiNascita.cittaEsteraNascita.codStato IsNot Nothing AndAlso persona.datiNascita.cittaEsteraNascita.codStato <> "" Then
                        luogoNascita.stato = persona.datiNascita.cittaEsteraNascita.codStato
                    End If
                End If

                'dal.AddObject("luogoNascita", luogoNascita)
                dal.Entry(luogoNascita).State = EntityState.Modified 'luogoNascita.MarkAsModified()


                Dim codRapporto As Integer = getCodRapportoFromRa_Cod(dal, persona.ruolo.codRuolo)
                Dim codRuolo As String = persona.ruolo.codRuolo
                Dim RisUmDB = (From r As Risorse_Umane In dal.Risorse_Umane Where r.Piva = piva And r.Sa_Cod = sa_cod0 And r.Cod_Contatto = codContatto And r.Ra_Cod = codRuolo).FirstOrDefault
                If RisUmDB IsNot Nothing Then
                    RisUmDB.Cod_Rapporto = codRapporto
                    RisUmDB.Validita_Inizio = strToDatetime(persona.ruolo.dtInizioRapporto)
                    RisUmDB.Validita_Fine = IIf(strToDatetime(persona.ruolo.dtFineRapporto) Is Nothing, AGRODATAFINE, strToDatetime(persona.ruolo.dtFineRapporto))
                    RisUmDB.Settore_Des = persona.ruolo.ruoloDescr
                    RisUmDB.Data_Modifica = strToDatetime(persona.ruolo.dtVariazioneRuolo)
                    RisUmDB.Occasionale = 0
                    RisUmDB.Ra_Cod = persona.ruolo.codRuolo
                    dal.Entry(RisUmDB).State = EntityState.Modified 'RisUmDB.MarkAsModified()
                    'dal.ObjectStateManager.ChangeObjectState(RisUmDB, EntityState.Modified)
                    'dal.Risorse_Umane.Attach(RisUmDB)
                Else
                    Dim risUm = AgronicaCoreAnagrafeDAL.EFContatti.CreateRisorse_Umane(dal, objParametri, contatto, SuperUserUsername)
                    risUm.Cod_Rapporto = codRapporto
                    risUm.Validita_Inizio = strToDatetime(persona.ruolo.dtInizioRapporto)
                    risUm.Validita_Fine = IIf(strToDatetime(persona.ruolo.dtFineRapporto) Is Nothing, AGRODATAFINE, strToDatetime(persona.ruolo.dtFineRapporto))
                    risUm.Settore_Des = persona.ruolo.ruoloDescr
                    risUm.Data_Modifica = strToDatetime(persona.ruolo.dtVariazioneRuolo)
                    risUm.Occasionale = 0
                    risUm.Ra_Cod = persona.ruolo.codRuolo
                    dal.Entry(risUm).State = EntityState.Modified 'risUm.MarkAsModified()
                End If


                If persona.flagReferente IsNot Nothing AndAlso persona.flagReferente <> "" Then
                    contatto.flagReferente = validaBooleano(persona.flagReferente)
                    If persona.flagReferente = "S" Then
                        Dim risUmReferente As Risorse_Umane
                        If (From r As Risorse_Umane In dal.Risorse_Umane Where r.Piva = piva And r.Sa_Cod = sa_cod0 And r.Cod_Contatto = codContatto And r.Cod_Rapporto = -14).Count = 0 Then
                            risUmReferente = AgronicaCoreAnagrafeDAL.EFContatti.CreateRisorse_Umane(dal, objParametri, contatto, SuperUserUsername)
                            risUmReferente.Cod_Rapporto = -14
                            dal.Entry(risUmReferente).State = EntityState.Modified 'risUmReferente.MarkAsModified()
                            dal.SaveChanges()
                        End If
                    End If
                End If

                'contatto.MarkAsModified()
                dal.Entry(contatto).State = EntityState.Modified
                'dal.AddObject("risUm", risUm)
            Next
        End If
        dal.SaveChanges()


        If source.referenteAziendale IsNot Nothing Then
            If source.referenteAziendale.persona IsNot Nothing Then
                Dim contattoReferente As Contatti
                Dim referente = source.referenteAziendale.persona
                If (From con In dal.Contatti Where con.Cod_Contatto = referente.codiceFiscale).Count = 0 Then
                    contattoReferente = EFContatti.CreateContattiEF(dal, objParametri, piva, 0, referente.codiceFiscale, 0, SuperUserUsername)
                Else
                    contattoReferente = (From con In dal.Contatti Where con.Cod_Contatto = referente.codiceFiscale).FirstOrDefault
                End If
                contattoReferente.Cognome = referente.cognome
                contattoReferente.Documento = referente.documento
                If referente.dtDocumento IsNot Nothing AndAlso referente.dtDocumento <> "" Then
                    contattoReferente.dtDocumento = referente.dtDocumento
                End If
                If referente.dtFonte IsNot Nothing AndAlso referente.dtFonte <> "" Then
                    contattoReferente.dtFonte = referente.dtFonte
                End If
                If referente.dtVariazione IsNot Nothing AndAlso referente.dtVariazione <> "" Then
                    contattoReferente.Data_Modifica = referente.dtVariazione
                End If
                contattoReferente.fonte = referente.fonte
                contattoReferente.fonteDescr = referente.fonteDescr
                contattoReferente.Nome = referente.nome
                contattoReferente.Sesso = referente.sesso

                Dim mail As Rubrica
                If (From cr In dal.ContattiXRubrica
                    Join r In dal.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica
                    Where r.descr = "Email" And cr.Cod_Contatto = referente.codiceFiscale).Count = 0 Then
                    mail = EFContatti.CreateRubricaContatto(dal, objParametri, piva, 0, contattoReferente, referente.contatti.email, "Email", SuperUserUsername)
                Else
                    mail = (From cr In dal.ContattiXRubrica
                            Join r In dal.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica
                            Where r.descr = "Email" And cr.Cod_Contatto = referente.codiceFiscale
                            Select r).FirstOrDefault
                    mail.numero = referente.contatti.email
                End If
                dal.Entry(mail).State = EntityState.Modified 'mail.MarkAsModified()


                Dim Fax As Rubrica
                If (From cr In dal.ContattiXRubrica
                    Join r In dal.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica
                    Where r.descr = "Fax" And cr.Cod_Contatto = referente.codiceFiscale).Count = 0 Then
                    Fax = EFContatti.CreateRubricaContatto(dal, objParametri, piva, 0, contattoReferente, referente.contatti.fax, "Fax", SuperUserUsername)
                Else
                    Fax = (From cr In dal.ContattiXRubrica
                           Join r In dal.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica
                           Where r.descr = "Fax" And cr.Cod_Contatto = referente.codiceFiscale
                           Select r).FirstOrDefault
                    Fax.numero = referente.contatti.fax
                End If
                dal.Entry(Fax).State = EntityState.Modified 'Fax.MarkAsModified()


                Dim Pec As Rubrica
                If (From cr In dal.ContattiXRubrica
                    Join r In dal.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica
                    Where r.descr = "Pec" And cr.Cod_Contatto = referente.codiceFiscale).Count = 0 Then
                    Pec = EFContatti.CreateRubricaContatto(dal, objParametri, piva, 0, contattoReferente, referente.contatti.pec, "Pec", SuperUserUsername)
                Else
                    Pec = (From cr In dal.ContattiXRubrica
                           Join r In dal.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica
                           Where r.descr = "Pec" And cr.Cod_Contatto = referente.codiceFiscale
                           Select r).FirstOrDefault
                    Pec.numero = referente.contatti.pec
                End If
                dal.Entry(Pec).State = EntityState.Modified 'Pec.MarkAsModified()


                Dim Telefono As Rubrica
                If (From cr In dal.ContattiXRubrica
                    Join r In dal.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica
                    Where r.descr = "Telefono" And cr.Cod_Contatto = referente.codiceFiscale).Count = 0 Then
                    Telefono = EFContatti.CreateRubricaContatto(dal, objParametri, piva, 0, contattoReferente, referente.contatti.telefono, "Telefono", SuperUserUsername)
                Else
                    Telefono = (From cr In dal.ContattiXRubrica
                                Join r In dal.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica
                                Where r.descr = "Telefono" And cr.Cod_Contatto = referente.codiceFiscale
                                Select r).FirstOrDefault
                    mail.numero = referente.contatti.telefono
                End If
                dal.Entry(Telefono).State = EntityState.Modified 'Telefono.MarkAsModified()


                Dim indirizzo As Indirizzi
                If (From ii In dal.Indirizzi
                    Join ci In dal.ContattiXIndirizzi On ii.cod_indirizzo Equals ci.Cod_Indirizzo
                    Where ci.Cod_Contatto = referente.codiceFiscale And ci.Tipo_Indirizzo = 3).Count = 0 Then
                    indirizzo = AgronicaCoreAnagrafeDAL.EFContatti.CreateIndirizzoContatto(dal, objParametri, piva, 0, contattoReferente, 3, SuperUserUsername)
                Else
                    indirizzo = (From ii In dal.Indirizzi
                                 Join ci In dal.ContattiXIndirizzi On ii.cod_indirizzo Equals ci.Cod_Indirizzo
                                 Where ci.Cod_Contatto = referente.codiceFiscale And ci.Tipo_Indirizzo = 3
                                 Select ii).FirstOrDefault
                End If
                indirizzo.CAP = referente.residenza.cap
                indirizzo.com_cod_istat = referente.residenza.codCom
                indirizzo.pro_cod_istat = referente.residenza.codProv
                indirizzo.ind_des = referente.residenza.indirizzo
                indirizzo.frz_des = referente.residenza.localita
                indirizzo.stato = "IT"
                indirizzo.com_des = (From istat As ISTAT_Comuni In dal.ISTAT_Comuni Where istat.Com_Cod_Istat = referente.residenza.codCom).FirstOrDefault.Descrizione
                indirizzo.pro_cod = (From istat As ISTAT In dal.ISTAT Where istat.PROV = referente.residenza.codProv).FirstOrDefault.COMUNI_PROV
                If referente.residenza.cittaEstera IsNot Nothing Then
                    If referente.residenza.cittaEstera.citta IsNot Nothing AndAlso
                        referente.residenza.cittaEstera.codStato IsNot Nothing Then
                        If referente.residenza.cittaEstera.citta <> "" Then
                            indirizzo.com_des = referente.residenza.cittaEstera.citta
                        End If
                        If referente.residenza.cittaEstera.codStato <> "" Then
                            indirizzo.stato = referente.residenza.cittaEstera.codStato
                        End If
                    End If
                End If
                dal.Entry(indirizzo).State = EntityState.Modified 'indirizzo.MarkAsModified()


                Dim luogoNascita As Indirizzi
                If (From ii In dal.Indirizzi
                    Join ci In dal.ContattiXIndirizzi On ii.cod_indirizzo Equals ci.Cod_Indirizzo
                    Where ci.Cod_Contatto = referente.codiceFiscale And ci.Tipo_Indirizzo = 5).Count = 0 Then
                    luogoNascita = AgronicaCoreAnagrafeDAL.EFContatti.CreateIndirizzoContatto(dal, objParametri, piva, 0, contattoReferente, 5, SuperUserUsername)
                Else
                    luogoNascita = (From ii In dal.Indirizzi
                                    Join ci In dal.ContattiXIndirizzi On ii.cod_indirizzo Equals ci.Cod_Indirizzo
                                    Where ci.Cod_Contatto = referente.codiceFiscale And ci.Tipo_Indirizzo = 5
                                    Select ii).FirstOrDefault
                End If

                luogoNascita.com_cod_istat = referente.datiNascita.codCom
                luogoNascita.pro_cod_istat = referente.datiNascita.codProv
                If referente.datiNascita.codCom <> "" Then
                    luogoNascita.com_des = (From istat As ISTAT_Comuni In dal.ISTAT_Comuni Where istat.Com_Cod_Istat = referente.datiNascita.codCom).FirstOrDefault.Descrizione
                End If
                If referente.datiNascita.codProv <> "" Then
                    luogoNascita.pro_cod = (From istat As ISTAT In dal.ISTAT Where istat.PROV = referente.datiNascita.codProv).FirstOrDefault.COMUNI_PROV
                End If

                If referente.datiNascita.cittaEsteraNascita IsNot Nothing Then
                    If referente.datiNascita.cittaEsteraNascita.citta IsNot Nothing AndAlso referente.datiNascita.cittaEsteraNascita.citta <> "" Then
                        luogoNascita.com_des = referente.datiNascita.cittaEsteraNascita.citta
                    End If
                    If referente.datiNascita.cittaEsteraNascita.codStato IsNot Nothing AndAlso referente.datiNascita.cittaEsteraNascita.codStato <> "" Then
                        luogoNascita.stato = referente.datiNascita.cittaEsteraNascita.codStato
                    End If
                End If

                'dal.AddObject("luogoNascita", luogoNascita)
                dal.Entry(luogoNascita).State = EntityState.Modified 'luogoNascita.MarkAsModified()


                Dim codRapporto As Integer = getCodRapportoFromRa_Cod(dal, referente.ruolo.codRuolo)
                Dim RisUmDB = (From r As Risorse_Umane In dal.Risorse_Umane Where r.Piva = piva And r.Cod_Rapporto = codRapporto And r.Cod_Contatto = referente.codiceFiscale And r.Ra_Cod = referente.ruolo.codRuolo).FirstOrDefault
                If RisUmDB IsNot Nothing Then
                    RisUmDB.Cod_Rapporto = codRapporto
                    RisUmDB.Validita_Inizio = strToDatetime(referente.ruolo.dtInizioRapporto)
                    RisUmDB.Validita_Fine = IIf(strToDatetime(referente.ruolo.dtFineRapporto) Is Nothing, AGRODATAFINE, strToDatetime(referente.ruolo.dtFineRapporto))
                    RisUmDB.Settore_Des = referente.ruolo.ruoloDescr
                    RisUmDB.Data_Modifica = strToDatetime(referente.ruolo.dtVariazioneRuolo)
                    RisUmDB.Occasionale = 0
                    RisUmDB.Ra_Cod = referente.ruolo.codRuolo
                    dal.Entry(RisUmDB).State = EntityState.Modified 'RisUmDB.MarkAsModified()
                    'dal.ObjectStateManager.ChangeObjectState(RisUmDB, EntityState.Modified)
                    'dal.Risorse_Umane.Attach(RisUmDB)
                Else
                    Dim risUm = AgronicaCoreAnagrafeDAL.EFContatti.CreateRisorse_Umane(dal, objParametri, contattoReferente, SuperUserUsername)
                    risUm.Cod_Rapporto = codRapporto
                    risUm.Validita_Inizio = strToDatetime(referente.ruolo.dtInizioRapporto)
                    risUm.Validita_Fine = IIf(strToDatetime(referente.ruolo.dtFineRapporto) Is Nothing, AGRODATAFINE, strToDatetime(referente.ruolo.dtFineRapporto))
                    risUm.Settore_Des = referente.ruolo.ruoloDescr
                    risUm.Data_Modifica = strToDatetime(referente.ruolo.dtVariazioneRuolo)
                    risUm.Occasionale = 0
                    risUm.Ra_Cod = referente.ruolo.codRuolo
                    dal.Entry(risUm).State = EntityState.Modified 'risUm.MarkAsModified()
                End If

                If (From rs In dal.Risorse_Umane Where rs.Cod_Contatto = referente.codiceFiscale And rs.Cod_Rapporto = -14).Count = 0 Then
                    Dim risUm = EFContatti.CreateRisorse_Umane(dal, objParametri, contattoReferente, SuperUserUsername)
                    risUm.Cod_Rapporto = -14
                    dal.Entry(risUm).State = EntityState.Modified 'risUm.MarkAsModified()
                End If

            End If
        End If
        dal.SaveChanges()


        If source.responsabileFito IsNot Nothing Then
            Dim responsabile = source.responsabileFito
            Dim contattoResponsabile As Contatti
            If (From c In dal.Contatti Where c.Piva = piva And c.Cod_Contatto = responsabile.codFiscale).Count = 0 Then
                contattoResponsabile = EFContatti.CreateContattiEF(dal, objParametri, piva, sa_cod0, responsabile.codFiscale, 0, SuperUserUsername)
            Else
                contattoResponsabile = (From c In dal.Contatti Where c.Piva = piva And c.Cod_Contatto = responsabile.codFiscale).FirstOrDefault
            End If

            contattoResponsabile.AlboProfessionale = responsabile.alboProfessionale
            contattoResponsabile.AlboProfessionaleDescr = responsabile.alboProfessionaleDescr
            contattoResponsabile.Cognome = responsabile.cognome
            contattoResponsabile.Nome = responsabile.nome
            contattoResponsabile.numeroIscrizione = responsabile.numeroIscizione
            contattoResponsabile.Qualifica = responsabile.qualifica
            contattoResponsabile.QualificaDescr = responsabile.qualificaDescr
            contattoResponsabile.Rag_Soc = responsabile.ragioneSociale
            contattoResponsabile.Sesso = responsabile.sesso
            contattoResponsabile.TitoloStudio = responsabile.titoloStudio
            contattoResponsabile.TitoloStudioDescr = responsabile.titoloStudioDescr
            If responsabile.dataNascita IsNot Nothing AndAlso responsabile.dataNascita <> "" Then
                contattoResponsabile.Data_Nascita = strToDatetime(responsabile.dataNascita)
            End If
            'contattoResponsabile.MarkAsModified()
            dal.Entry(contattoResponsabile).State = EntityState.Modified

            Dim risUm As Risorse_Umane
            If (From rs In dal.Risorse_Umane
                Join c In dal.Contatti On rs.Cod_Contatto Equals c.Cod_Contatto
                Where rs.Cod_Rapporto = -12).Count = 0 Then
                risUm = EFContatti.CreateRisorse_Umane(dal, objParametri, contattoResponsabile, SuperUserUsername)
                If responsabile.dataInizioRapporto IsNot Nothing AndAlso responsabile.dataInizioRapporto <> "" Then
                    risUm.Validita_Inizio = responsabile.dataInizioRapporto
                End If
                risUm.Cod_Rapporto = -12
            Else
                risUm = (From rs In dal.Risorse_Umane
                         Join c In dal.Contatti On rs.Cod_Contatto Equals c.Cod_Contatto
                         Where rs.Cod_Rapporto = -12
                         Select rs).FirstOrDefault
                If responsabile.dataInizioRapporto IsNot Nothing AndAlso responsabile.dataInizioRapporto <> "" Then
                    risUm.Validita_Inizio = responsabile.dataInizioRapporto
                End If
            End If

            If responsabile.comuneDomicilio IsNot Nothing AndAlso responsabile.comuneDomicilio <> "" Then
                Dim indirizzo As Indirizzi
                If (From ii In dal.Indirizzi
                    Join ci In dal.ContattiXIndirizzi On ii.cod_indirizzo Equals ci.Cod_Indirizzo
                    Where ci.Cod_Contatto = responsabile.codFiscale AndAlso ci.Tipo_Indirizzo = 3).Count = 0 Then
                    indirizzo = EFContatti.CreateIndirizzoContatto(dal, objParametri, piva, sa_cod0, contattoResponsabile, 3, SuperUserUsername)
                Else
                    indirizzo = (From ii In dal.Indirizzi
                                 Join ci In dal.ContattiXIndirizzi On ii.cod_indirizzo Equals ci.Cod_Indirizzo
                                 Where ci.Cod_Contatto = responsabile.codFiscale AndAlso ci.Tipo_Indirizzo = 3
                                 Select ii).FirstOrDefault
                End If
                If responsabile.cittaEsteraDomicilio IsNot Nothing AndAlso responsabile.cittaEsteraDomicilio <> "" Then
                    indirizzo.com_des = responsabile.cittaEsteraDomicilio
                    indirizzo.stato = responsabile.statoEsteroDomicilio
                Else
                    indirizzo.com_des = responsabile.comuneDomicilioDescr
                    indirizzo.com_cod_istat = responsabile.comuneDomicilio
                    indirizzo.pro_cod = responsabile.provinciaDomicilioSigla
                    indirizzo.pro_cod_istat = responsabile.provinciaDomicilio
                    indirizzo.ind_des = responsabile.indirizzoDomicilio
                    indirizzo.CAP = responsabile.capDomicilio
                    indirizzo.stato = "IT"
                End If
                dal.Entry(indirizzo).State = EntityState.Modified 'indirizzo.MarkAsModified()
            End If

            If responsabile.comuneNascita IsNot Nothing AndAlso responsabile.comuneNascita <> "" Then
                Dim indirizzo As Indirizzi
                If (From ii In dal.Indirizzi
                    Join ci In dal.ContattiXIndirizzi On ii.cod_indirizzo Equals ci.Cod_Indirizzo
                    Where ci.Cod_Contatto = responsabile.codFiscale AndAlso ci.Tipo_Indirizzo = 5).Count = 0 Then
                    indirizzo = EFContatti.CreateIndirizzoContatto(dal, objParametri, piva, sa_cod0, contattoResponsabile, 3, SuperUserUsername)
                Else
                    indirizzo = (From ii In dal.Indirizzi
                                 Join ci In dal.ContattiXIndirizzi On ii.cod_indirizzo Equals ci.Cod_Indirizzo
                                 Where ci.Cod_Contatto = responsabile.codFiscale AndAlso ci.Tipo_Indirizzo = 5
                                 Select ii).FirstOrDefault
                End If
                If responsabile.cittaEsteraNascita IsNot Nothing AndAlso responsabile.cittaEsteraNascita <> "" Then
                    indirizzo.com_des = responsabile.cittaEsteraNascita
                    indirizzo.stato = responsabile.cittaEsteraNascita
                Else
                    indirizzo.com_des = responsabile.comuneNascitaDescr
                    indirizzo.com_cod_istat = responsabile.comuneNascita
                    indirizzo.pro_cod = responsabile.provinciaNascitaSigla
                    indirizzo.pro_cod_istat = responsabile.provinciaNascita
                    indirizzo.stato = "IT"
                End If
                dal.Entry(indirizzo).State = EntityState.Modified 'indirizzo.MarkAsModified()
            End If

            Dim mail As Rubrica
            If (From cr In dal.ContattiXRubrica
                Join r In dal.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica
                Where r.descr = "Email" And cr.Cod_Contatto = responsabile.codFiscale).Count = 0 Then
                mail = EFContatti.CreateRubricaContatto(dal, objParametri, piva, 0, contattoResponsabile, responsabile.emailDomicilio, "Email", SuperUserUsername)
            Else
                mail = (From cr In dal.ContattiXRubrica
                        Join r In dal.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica
                        Where r.descr = "Email" And cr.Cod_Contatto = responsabile.codFiscale
                        Select r).FirstOrDefault
                mail.numero = responsabile.emailDomicilio
            End If
            dal.Entry(mail).State = EntityState.Modified 'mail.MarkAsModified()

            Dim Fax As Rubrica
            If (From cr In dal.ContattiXRubrica
                Join r In dal.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica
                Where r.descr = "Fax" And cr.Cod_Contatto = responsabile.codFiscale).Count = 0 Then
                Fax = EFContatti.CreateRubricaContatto(dal, objParametri, piva, 0, contattoResponsabile, responsabile.faxDomicilio, "Fax", SuperUserUsername)
            Else
                Fax = (From cr In dal.ContattiXRubrica
                       Join r In dal.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica
                       Where r.descr = "Fax" And cr.Cod_Contatto = responsabile.codFiscale
                       Select r).FirstOrDefault
                Fax.numero = responsabile.faxDomicilio
            End If
            dal.Entry(Fax).State = EntityState.Modified 'Fax.MarkAsModified()

            Dim Telefono As Rubrica
            If (From cr In dal.ContattiXRubrica
                Join r In dal.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica
                Where r.descr = "Telefono" And cr.Cod_Contatto = responsabile.codFiscale).Count = 0 Then
                Telefono = EFContatti.CreateRubricaContatto(dal, objParametri, piva, 0, contattoResponsabile, responsabile.telefonoDomicilio, "Telefono", SuperUserUsername)
            Else
                Telefono = (From cr In dal.ContattiXRubrica
                            Join r In dal.Rubrica On cr.Cod_Rubrica Equals r.cod_rubrica
                            Where r.descr = "Telefono" And cr.Cod_Contatto = responsabile.codFiscale
                            Select r).FirstOrDefault
                mail.numero = responsabile.telefonoDomicilio
            End If
            dal.Entry(Telefono).State = EntityState.Modified 'Telefono.MarkAsModified()

        End If
        dal.SaveChanges()
        '''''''''''''''''''''''''''
        '''''''''''''''''''''''''''
        '''''''''''''''''''''''''''
        Dim centroF As Centri_Aziendali
        If (From ceaz As Centri_Aziendali In dal.Centri_Aziendali Where ceaz.PIVA = piva And ceaz.sa_nome.Contains("CentroAziendaleFittizio0")).Count = 0 Then
            centroF = EFCentri_Aziendali.CreateCentri_AziendaliEF(dal, objParametri, imp, "CentroAziendaleFittizio0", SuperUserUsername, objParametri_Utenti)
            Dim indirizzoCentro = EFCentri_Aziendali.CreateIndirizzoCentro(dal, objParametri, centroF, 0, SuperUserUsername)
        Else
            centroF = (From ceaz As Centri_Aziendali In dal.Centri_Aziendali Where ceaz.PIVA = piva And ceaz.sa_nome = "CentroAziendaleFittizio0").FirstOrDefault
        End If
        Dim sa_cod = centroF.sa_cod
        dal.SaveChanges()
        '''''''''''''''''''''''''''
        '''''''''''''''''''''''''''
        '''''''''''''''''''''''''''

        If source.contiCorrenti IsNot Nothing AndAlso source.contiCorrenti.contoCorrente IsNot Nothing Then
            For Each conto In source.contiCorrenti.contoCorrente
                Dim istituto As Ist_Credito = (From iss As Ist_Credito In dal.Ist_Credito Where iss.Istituto_Des = conto.descrizione + " " + conto.nomeFiliale).FirstOrDefault
                If istituto Is Nothing Then
                    istituto = AgronicaCoreAnagrafeDAL.EFIst_Credito.CreateIstCredito(dal, objParametri, piva, 0, conto.descrizione + " " + conto.nomeFiliale, SuperUserUsername)
                    Dim indirizzo = AgronicaCoreAnagrafeDAL.EFIst_Credito.CreateIndirizzoIstCredito(dal, objParametri, istituto, SuperUserUsername)
                    istituto.Cod_Indirizzo = indirizzo.cod_indirizzo
                    dal.Entry(istituto).State = EntityState.Modified 'istituto.MarkAsModified()
                    indirizzo.CAP = conto.cap
                    indirizzo.stato = conto.codPaese
                    indirizzo.ind_des = conto.indirizzo
                    indirizzo.com_des = conto.descrizioneComune
                    indirizzo.pro_cod = conto.siglaProvincia
                    indirizzo.frz_des = conto.localita
                    dal.Entry(indirizzo).State = EntityState.Modified 'indirizzo.MarkAsModified()
                End If
                Dim liq As Liquidita
                If (From l In dal.Liquidita Where l.Piva = piva And l.Numero = conto.contoCorrente).Count = 0 Then
                    liq = AgronicaCoreAnagrafeDAL.EFLiquidita.CreateLiquidita(dal, objParametri, piva, 0, piva, SuperUserUsername)
                Else
                    liq = (From l In dal.Liquidita Where l.Piva = piva And l.Numero = conto.contoCorrente).FirstOrDefault
                End If
                liq.Anomalo = validaBooleano(conto.anomalo)
                liq.Cin = conto.cinBban
                liq.Nazione = conto.codPaese
                liq.Cifre_Controllo = conto.cinIban
                liq.Abi = conto.codAbi
                liq.codAnomalia = conto.codAnomalia
                liq.Cab = conto.codCab
                liq.Numero = conto.contoCorrente
                liq.Cod_Istituto = istituto.Cod_Istituto
                liq.dataAnomalia = strToDatetime(conto.dataAnomalia)
                liq.dataFine = strToDatetime(conto.dataFine)
                liq.DataInizioSportello = strToDatetime(conto.dataInizioSportello)
                liq.dataInserimento = strToDatetime(conto.dataInserimento)
                liq.DataVariazione = strToDatetime(conto.dataVariazione)
                liq.descrAnomalia = conto.descrAnomalia
                liq.progr = strToInteger(conto.progr)
                liq.flagTesoriere = validaBooleano(conto.flagTesoriere)
                liq.Preferito = validaBooleano(conto.preferito)
                dal.Entry(liq).State = EntityState.Modified 'liq.MarkAsModified()
            Next
        End If
        dal.SaveChanges()

        'CONTATTI
        If source.datiAnagrafici.contatti IsNot Nothing Then
            Dim mail As Rubrica
            If (From cr In dal.CentrixRubrica
                Join r In dal.Rubrica On cr.cod_rubrica Equals r.cod_rubrica
                Where r.descr = "Email" And cr.PIVA = piva And cr.sa_cod = sa_cod).Count = 0 Then
                mail = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateRubricaCentro(dal, objParametri, centroF, strToStrObb(source.datiAnagrafici.contatti.email), "Email", SuperUserUsername)
            Else
                mail = (From cr In dal.CentrixRubrica
                        Join r In dal.Rubrica On cr.cod_rubrica Equals r.cod_rubrica
                        Where r.descr = "Email" And cr.PIVA = piva And cr.sa_cod = sa_cod
                        Select r).FirstOrDefault
                mail.numero = strToStrObb(source.datiAnagrafici.contatti.email)
            End If

            Dim Fax As Rubrica
            If (From cr In dal.CentrixRubrica
                Join r In dal.Rubrica On cr.cod_rubrica Equals r.cod_rubrica
                Where r.descr = "Fax" And cr.PIVA = piva And cr.sa_cod = sa_cod).Count = 0 Then
                Fax = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateRubricaCentro(dal, objParametri, centroF, strToStrObb(source.datiAnagrafici.contatti.fax), "Fax", SuperUserUsername)
            Else
                Fax = (From cr In dal.CentrixRubrica
                       Join r In dal.Rubrica On cr.cod_rubrica Equals r.cod_rubrica
                       Where r.descr = "Fax" And cr.PIVA = piva And cr.sa_cod = sa_cod
                       Select r).FirstOrDefault
                Fax.numero = strToStrObb(source.datiAnagrafici.contatti.fax)
            End If



            Dim Pec As Rubrica
            If (From cr In dal.CentrixRubrica
                Join r In dal.Rubrica On cr.cod_rubrica Equals r.cod_rubrica
                Where r.descr = "Pec" And cr.PIVA = piva And cr.sa_cod = sa_cod).Count = 0 Then
                Pec = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateRubricaCentro(dal, objParametri, centroF, strToStrObb(source.datiAnagrafici.contatti.pec), "Pec", SuperUserUsername)
            Else
                Pec = (From cr In dal.CentrixRubrica
                       Join r In dal.Rubrica On cr.cod_rubrica Equals r.cod_rubrica
                       Where r.descr = "Pec" And cr.PIVA = piva And cr.sa_cod = sa_cod
                       Select r).FirstOrDefault
                Pec.numero = strToStrObb(source.datiAnagrafici.contatti.pec)
            End If

            Dim Telefono As Rubrica
            If (From cr In dal.CentrixRubrica
                Join r In dal.Rubrica On cr.cod_rubrica Equals r.cod_rubrica
                Where r.descr = "Telefono" And cr.PIVA = piva And cr.sa_cod = sa_cod).Count = 0 Then
                Telefono = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateRubricaCentro(dal, objParametri, centroF, strToStrObb(source.datiAnagrafici.contatti.telefono), "Telefono", SuperUserUsername)
            Else
                Telefono = (From cr In dal.CentrixRubrica
                            Join r In dal.Rubrica On cr.cod_rubrica Equals r.cod_rubrica
                            Where r.descr = "Telefono" And cr.PIVA = piva And cr.sa_cod = sa_cod
                            Select r).FirstOrDefault
                Telefono.numero = strToStrObb(source.datiAnagrafici.contatti.telefono)
            End If

        End If

        dal.SaveChanges()

        If source.allevamenti IsNot Nothing AndAlso
                        source.allevamenti.allevamento IsNot Nothing Then
            For Each all In source.allevamenti.allevamento
                Dim fabbricato As Fabbricati
                Dim fabbricato_des = CStr(all.codAllevamento) + " - " + CStr(all.allevamentoDescr) + " (" + CStr(all.idAllevamento) + ")"
                If (From f In dal.Fabbricati Where f.PIVA = piva And f.Fabbricato_Des = fabbricato_des).Count = 0 Then
                    Dim fabbricato_Cod As Integer = idGen.NuovoId_SeqMagazzino(piva, sa_cod, 80740352, 80871423, objParametri)
                    fabbricato = AgronicaCoreAnagrafeDAL.EFFabbricati.CreateFabbricato(dal, objParametri, objParametri_Utenti, centroF, SuperUserUsername)
                Else
                    fabbricato = (From f In dal.Fabbricati Where f.PIVA = piva And f.Fabbricato_Des = fabbricato_des).FirstOrDefault
                End If
                fabbricato.Fabbricato_Des = fabbricato_des
                fabbricato.Tipo_Fabbricato_Cod = 70
                fabbricato.Data_Richiesta_Autorizzazione = AGRODATAINIZIO
                fabbricato.Tipologia_Utilizzo = 0
                fabbricato.Regolamento_Cod = 1


                Dim indFabb As Indirizzi
                If (From f In dal.Fabbricati
                    Join indF In dal.Indirizzi On f.Indirizzo_Cod Equals indF.cod_indirizzo
                    Where f.PIVA = piva And f.Fabbricato_Des = all.idAllevamento).Count = 0 Then
                    indFabb = AgronicaCoreAnagrafeDAL.EFIndirizzi.CreateIndirizziEF(dal, objParametri, SuperUserUsername)
                    fabbricato.Indirizzo_Cod = indFabb.cod_indirizzo
                Else
                    indFabb = (From f In dal.Fabbricati
                               Join indF In dal.Indirizzi On f.Indirizzo_Cod Equals indF.cod_indirizzo
                               Where f.PIVA = piva And f.Fabbricato_Des = all.idAllevamento
                               Select indF).FirstOrDefault

                End If
                indFabb.ind_des = all.indirizzo
                indFabb.CAP = all.cap
                indFabb.com_cod_istat = all.comCodice
                indFabb.com_des = all.comDescr
                indFabb.frz_des = all.localita
                'indFabb.MarkAsModified()
                dal.Entry(indFabb).State = EntityState.Modified
                'fabbricato.MarkAsModified()
                dal.Entry(fabbricato).State = EntityState.Modified


                Dim sta As Stalla
                If (From s In dal.Stalla Where s.PIVA = piva And s.sa_cod = sa_cod And s.STA_NUM = fabbricato.Fabbricato_Cod).Count = 0 Then
                    sta = AgronicaCoreAnagrafeDAL.EFStalla.CreateStalla(dal, objParametri, fabbricato, SuperUserUsername)
                Else
                    sta = (From s In dal.Stalla Where s.PIVA = piva And s.sa_cod = sa_cod And s.STA_NUM = fabbricato.Fabbricato_Cod).FirstOrDefault
                End If
                sta.speCodice = all.speCodice
                dal.Entry(sta).State = EntityState.Modified 'sta.MarkAsModified()

                If all.consistenze IsNot Nothing AndAlso
                    all.consistenze.consistenza IsNot Nothing Then
                    For Each cons In all.consistenze.consistenza
                        Dim cod_progetto As Integer = 0
                        'dal.Zoo_Animali.AddObject(zooAnimali)
                        'dal.SaveChanges()
                        Dim mov_Descr = cons.codZootecnica + " - " + cons.descrZootecnica
                        Dim dataOperazione As Date = New Date(cons.annoRif, 1, 1)
                        If (From m In dal.Movimenti Where m.PIVA = piva And m.Mov_Desc = mov_Descr And m.Data_Movimento = dataOperazione And m.Sa_Cod = sa_cod).Count = 0 Then
                            Dim agendaOp As Agenda
                            agendaOp = EFAgenda.CreateAgenda(dal, objParametri, piva, sa_cod, -2, mov_Descr, SuperUserUsername)
                            Dim mov = EFAgenda.CreateMovimenti(dal, agendaOp, objParametri, 0, "ANIMALI", mov_Descr, SuperUserUsername)
                            Dim movDett = EFMovimenti.CreateMovimentiDettagli(dal, objParametri, mov, 300, 0, 0, "", 38, strToIntegerNotNothing(cons.numCapi), SuperUserUsername)
                            mov.Data_Movimento = dataOperazione
                            movDett.Cod_Progetto = cod_progetto
                            movDett.Pendente = 5
                            dal.Entry(movDett).State = EntityState.Modified 'movDett.MarkAsModified()
                            dal.SaveChanges()
                            Dim dest = EFMovimenti_Dettagli.CreateMov_Destinazioni(dal, objParametri, movDett, 0, sta.STA_NUM, 15, strToIntegerNotNothing(cons.numCapi), SuperUserUsername)
                            'Dim gen_cod As Integer
                            'Dim spe_cod As Integer
                            'Dim ipro_cod As Integer
                            'Dim cat_cod As Integer
                            'Dim raz_cod As Integer
                            'SpecieDaCodificaCliente(dal, cons.codZootecnica, gen_cod, spe_cod, ipro_cod, cat_cod, raz_cod)
                            'Dim ZooAnimali = EFZoo_Animali.CreateZoo_Animali(dal, objParametri, fabbricato, dest, "", gen_cod, spe_cod, ipro_cod, cat_cod, raz_cod, SuperUserUsername)
                            'ZooAnimali.GEN_COD = gen_cod
                            'ZooAnimali.SPE_COD = spe_cod
                            'ZooAnimali.IPRO_COD = ipro_cod
                            'ZooAnimali.CAT_COD = cat_cod
                            'ZooAnimali.RAZ_COD = raz_cod
                            'ZooAnimali.codZootecnica = cons.codZootecnica
                            'ZooAnimali.descrZootecnica = cons.descrZootecnica
                            'ZooAnimali.Nome = cons.codZootecnica
                            'ZooAnimali.MarkAsModified()
                        Else
                            Dim agenda1 = (From a In dal.Agenda
                                           Join m In dal.Movimenti On a.Id_Agenda Equals m.Id_Agenda
                                           Where m.PIVA = piva And m.Mov_Desc = mov_Descr And m.Data_Movimento = dataOperazione And m.Sa_Cod = sa_cod
                                           Select a).FirstOrDefault
                            Dim mov1 = (From m In dal.Movimenti Where m.Id_Agenda = agenda1.Id_Agenda).FirstOrDefault
                            Dim movDett1 = (From md In dal.Movimenti_dettagli Where md.Id_Agenda = agenda1.Id_Agenda And md.Id_Mov = mov1.Id_Mov).FirstOrDefault
                            mov1.Data_Movimento = dataOperazione
                            movDett1.Cod_Progetto = cod_progetto
                            movDett1.Pendente = 5
                            dal.Entry(movDett1).State = EntityState.Modified 'movDett1.MarkAsModified()
                            Dim dest1 = (From md In dal.Mov_Destinazioni Where md.Id_Agenda = agenda1.Id_Agenda And md.Id_Mov = mov1.Id_Mov And md.Id_Mov_Det = movDett1.Id_Mov_Det).FirstOrDefault
                            'If (From zoo In dal.Zoo_Animali Where zoo.PIVA = piva And zoo.sa_cod = sa_cod And zoo.Cod_Progetto = dest1.Id_Destinazione And zoo.codZootecnica = cons.codZootecnica).Count = 0 Then
                            '    Dim gen_cod As Integer
                            '    Dim spe_cod As Integer
                            '    Dim ipro_cod As Integer
                            '    Dim cat_cod As Integer
                            '    Dim raz_cod As Integer
                            '    SpecieDaCodificaCliente(dal, cons.codZootecnica, gen_cod, spe_cod, ipro_cod, cat_cod, raz_cod)
                            '    Dim ZooAnimali = EFZoo_Animali.CreateZoo_Animali(dal, objParametri, fabbricato, dest1, "", gen_cod, spe_cod, ipro_cod, cat_cod, raz_cod, SuperUserUsername)
                            '    ZooAnimali.GEN_COD = gen_cod
                            '    ZooAnimali.SPE_COD = spe_cod
                            '    ZooAnimali.IPRO_COD = ipro_cod
                            '    ZooAnimali.CAT_COD = cat_cod
                            '    ZooAnimali.RAZ_COD = raz_cod
                            '    ZooAnimali.codZootecnica = cons.codZootecnica
                            '    ZooAnimali.descrZootecnica = cons.descrZootecnica
                            '    ZooAnimali.Nome = cons.codZootecnica
                            '    ZooAnimali.MarkAsModified()
                            '    dal.SaveChanges()
                            'Else
                            '    Dim zooAnimali = (From zoo In dal.Zoo_Animali Where zoo.PIVA = piva And zoo.sa_cod = sa_cod And zoo.Cod_Progetto = dest1.Id_Destinazione And zoo.codZootecnica = cons.codZootecnica).FirstOrDefault
                            '    Dim gen_cod As Integer
                            '    Dim spe_cod As Integer
                            '    Dim ipro_cod As Integer
                            '    Dim cat_cod As Integer
                            '    Dim raz_cod As Integer
                            '    SpecieDaCodificaCliente(dal, cons.codZootecnica, gen_cod, spe_cod, ipro_cod, cat_cod, raz_cod)
                            '    zooAnimali.GEN_COD = gen_cod
                            '    zooAnimali.SPE_COD = spe_cod
                            '    zooAnimali.IPRO_COD = ipro_cod
                            '    zooAnimali.CAT_COD = cat_cod
                            '    zooAnimali.RAZ_COD = raz_cod
                            '    zooAnimali.codZootecnica = cons.codZootecnica
                            '    zooAnimali.descrZootecnica = cons.descrZootecnica
                            '    zooAnimali.Nome = cons.codZootecnica
                            '    zooAnimali.MarkAsModified()
                            '    dal.SaveChanges()
                            'End If
                        End If
                    Next
                End If
            Next
        End If
        dal.SaveChanges()


        If source.conduzioniTerreni IsNot Nothing AndAlso
            source.conduzioniTerreni.particelle IsNot Nothing Then
            For Each particella In source.conduzioniTerreni.particelle
                Dim sezione As String = "0"
                If particella.sezione IsNot Nothing AndAlso particella.sezione <> "" Then
                    sezione = particella.sezione
                End If
                Dim subalterno As String = "0"
                If particella.subalterno IsNot Nothing AndAlso particella.subalterno <> "" And particella.subalterno <> "000" Then
                    subalterno = particella.subalterno
                End If
                Dim part As ParticelleCatastali
                Dim particellaProv = particella.codProv
                Dim particellaCom = particella.codCom
                Dim particellaFoglio = particella.foglio
                Dim particellaNumero = particella.particella
                If (From ip In dal.ParticelleCatastali
                    Where ip.PROV = particellaProv And ip.COM = particellaCom And ip.SEZIONE = sezione And ip.FOGLIO = particellaFoglio And ip.NUMERO = particellaNumero And ip.SUBALTERNO = subalterno).Count = 0 Then
                    part = AgronicaCoreAnagrafeDAL.EFParticelle.CreateParticelleCatastaliEF(dal, objParametri, particella.codProv, particella.codCom, sezione, particella.foglio, particella.particella, subalterno, SuperUserUsername)
                Else
                    part = (From ip In dal.ParticelleCatastali
                            Where ip.PROV = particellaProv And ip.COM = particellaCom And ip.SEZIONE = sezione And ip.FOGLIO = particellaFoglio And ip.NUMERO = particellaNumero And ip.SUBALTERNO = subalterno).FirstOrDefault
                End If
                'DATI PARTICELLA
                part.casiParticolari = particella.casiParticolari
                part.CLASSE = strToCodiceClasse(particella.codiceClasse)
                part.QUALITA_COD = strToInteger(particella.codiceQualita)
                'particella.codReg
                If particella.dataFine IsNot Nothing AndAlso particella.dataFine <> "" Then
                    part.Validita_Fine = strToDatetime(particella.dataFine)
                Else
                    part.Validita_Fine = AGRODATAFINE
                End If
                If strToDatetime(particella.dataInizio) IsNot Nothing Then
                    part.Validita_Inizio = strToDatetime(particella.dataInizio)
                Else
                    part.Validita_Inizio = AGRODATAINIZIO
                End If
                part.datainvio = strToDatetime(particella.dataInserimento)
                part.Data_Modifica = strToDatetime(particella.dataVariazione)
                part.fasciaAltimetrica = particella.fasciaAltimetrica
                part.fasciaAltimetricaDescr = part.fasciaAltimetricaDescr
                part.Fonte = particella.fonte
                part.FonteDescr = particella.fonteDescr
                'particella.regDescr
                If particella.supCatastale IsNot Nothing AndAlso particella.supCatastale <> "" Then
                    Dim ettariAreCentiare = strToDouble(particella.supCatastale) / 1000
                    Dim ettari, are, centiare
                    AgronicaCoreDataProvider.Conversioni.EttariAreCentiare_from_Ettari(ettariAreCentiare, ettari, are, centiare)
                    part.ETTARI = CDbl(ettari)
                    part.ARE = CDbl(are)
                    part.CENTIARE = CDbl(centiare)
                Else
                    part.ETTARI = 0
                    part.ARE = 0
                    part.CENTIARE = 0
                End If
                part.tipoDocumento = particella.tipoDocumento
                part.tipoDocumentoDescr = particella.tipoDocumentoDescr
                part.utilizzo = particella.utilizzo

                dal.Entry(part).State = EntityState.Modified 'part.MarkAsModified()

                Dim conduzione = particella.conduzione
                If conduzione IsNot Nothing Then
                    'DATI CONDUZIONE
                    part.fasciaAltimetrica = conduzione.fasciaAltimetrica
                    part.fasciaAltimetricaDescr = conduzione.fasciaAltimetricaDescr
                    dal.Entry(part).State = EntityState.Modified 'part.MarkAsModified()

                    Dim eleggibilitaCod = idGen.NuovoId_Tabella_EF(dal, "ParticelleCatastalixEleggibilitaParticelle", 0, 200000000, objParametri)
                    Dim pcxep As ParticelleCatastalixEleggibilitaParticelle
                    If (From pe In dal.ParticelleCatastalixEleggibilitaParticelle
                        Where pe.PROV = particella.codProv And pe.COM = particella.codCom And pe.SEZIONE = sezione And pe.FOGLIO = particella.foglio And pe.NUMERO = particella.particella And pe.SUBALTERNO = subalterno).Count = 0 Then
                        pcxep = AgronicaCoreAnagrafeDAL.EFParticelle.ParticelleCatastalixEleggibilitaParticelle(dal, objParametri, part, 0, SuperUserUsername)
                    Else
                        pcxep = (From pe In dal.ParticelleCatastalixEleggibilitaParticelle
                                 Where pe.PROV = particella.codProv And pe.COM = particella.codCom And pe.SEZIONE = sezione And pe.FOGLIO = particella.foglio And pe.NUMERO = particella.particella And pe.SUBALTERNO = subalterno).FirstOrDefault
                    End If
                    If conduzione.supEleggibile IsNot Nothing AndAlso conduzione.supEleggibile <> "" Then
                        pcxep.Superficie = strToDouble(conduzione.supEleggibile) / 1000
                    Else
                        pcxep.Superficie = 0
                    End If
                    pcxep.percEleggibile = CStr(conduzione.percEleggibile)
                    dal.Entry(pcxep).State = EntityState.Modified 'pcxep.MarkAsModified()
                    dal.SaveChanges()

                    If conduzione.contratti IsNot Nothing Then
                        Dim j = 0
                        For Each contratto In conduzione.contratti
                            'ID_Contratto in ImpreseXParticelle
                            'Inserire ID_Contratto
                            'contratto.formaPossesso
                            'contratto.formaPossessoDescr
                            'contratto.supPossesso
                            'contratto.titoloConduzione
                            Dim contr As Imprese_Contratti '.CreateImprese_Contratti(piva, contrattoCod, "", pivaSuperUser, pivaSuperUser, 0, 0, 0, 0, AGRODATAINIZIO)
                            Dim imxp As ImpreseXParticelle
                            Dim dataInizio As DateTime = strToDatetime(contratto.dtInizio)
                            If (From ixc In dal.ImpreseXParticelle
                                Join ic In dal.Imprese_Contratti On ic.Contratto_Cod Equals ixc.Contratto_Cod
                                Where ixc.PIVA = piva _
                                And ixc.PROV = particella.codProv _
                                And ixc.COM = particella.codCom _
                                And ixc.SEZIONE = sezione _
                                And ixc.FOGLIO = particella.foglio _
                                And ixc.NUMERO = particella.particella _
                                And ixc.SUBALTERNO = subalterno _
                                And ic.prog = contratto.progr).Count = 0 Then
                                Dim contrattoCod = idGen.NuovoId_Tabella_EF(dal, "Contratti", 0, 20000000, objParametri)
                                contr = New Imprese_Contratti
                                imxp = EFCentri_Aziendali.CreateCentriAziendaliXParticelle(dal, objParametri, centroF, part, SuperUserUsername)
                                imxp.Contratto_Cod = contrattoCod
                                contr.Piva = piva
                                contr.Contratto_Cod = contrattoCod
                                dal.Imprese_Contratti.Add(contr)
                            Else
                                contr = (From ixc In dal.ImpreseXParticelle
                                         Join ic In dal.Imprese_Contratti On ic.Contratto_Cod Equals ixc.Contratto_Cod
                                         Where ixc.PIVA = piva _
                                         And ixc.PROV = particella.codProv _
                                         And ixc.COM = particella.codCom _
                                         And ixc.SEZIONE = sezione _
                                         And ixc.FOGLIO = particella.foglio _
                                         And ixc.NUMERO = particella.particella _
                                         And ixc.SUBALTERNO = subalterno _
                                         And ic.prog = contratto.progr
                                         Select ic).FirstOrDefault
                                imxp = (From ixc In dal.ImpreseXParticelle
                                        Join ic In dal.Imprese_Contratti On ic.Contratto_Cod Equals ixc.Contratto_Cod
                                        Where ixc.PIVA = piva _
                                        And ixc.PROV = particella.codProv _
                                        And ixc.COM = particella.codCom _
                                        And ixc.SEZIONE = sezione _
                                        And ixc.FOGLIO = particella.foglio _
                                        And ixc.NUMERO = particella.particella _
                                        And ixc.SUBALTERNO = subalterno _
                                        And ic.prog = contratto.progr
                                        Select ixc).FirstOrDefault
                            End If
                            contr.Descrizione_1 = contratto.formaPossesso
                            contr.Descrizione_2 = contratto.formaPossessoDescr
                            contr.Riferimento = ""
                            contr.Username_Creazione = SuperUserUsername
                            contr.Username_Modifica = SuperUserUsername
                            contr.Cod_Risum = 0
                            contr.Stato = 0
                            contr.ChkStato_Automatico = 0
                            contr.Cau_Pagamento = 0
                            contr.Data_Stipulazione = strToDatetime(contratto.dtInizio)
                            contr.Validita_Fine = strToDatetime(contratto.dtFine)
                            contr.DataVariazione = strToDatetime(contratto.dataVariazione)
                            contr.prog = contratto.progr
                            imxp.biologico = validaBooleano(conduzione.biologico)
                            imxp.flagAnomaliaMacrouso = validaBooleano(conduzione.flagAnomaliaMacrouso)
                            imxp.flagContenzioso = validaBooleano(conduzione.flagContenzioso)
                            imxp.flagSupero = validaBooleano(conduzione.flagSupero)
                            imxp.Irrigabilita = conduzione.irriguo
                            If conduzione.totSupPossesso IsNot Nothing AndAlso conduzione.totSupPossesso <> "" Then
                                imxp.Sup_Condotta = strToDouble(conduzione.totSupPossesso) / 1000
                            Else
                                imxp.Sup_Condotta = 0
                            End If
                            imxp.RotazioneColturale = conduzione.rotazioneColturale
                            'imxp.MarkAsModified()
                            dal.Entry(imxp).State = EntityState.Modified
                            'contr.MarkAsModified()
                            dal.Entry(contr).State = EntityState.Modified
                            'dal.AddObject("contr", contr)
                        Next
                    Else
                        Dim impxPart As ImpreseXParticelle
                        If (From ip In dal.ImpreseXParticelle
                            Where ip.PIVA = piva And ip.PROV = particella.codProv And ip.COM = particella.codCom And ip.SEZIONE = sezione And ip.FOGLIO = particella.foglio And ip.NUMERO = particella.particella And ip.SUBALTERNO = subalterno And ip.Contratto_Cod = 0).Count = 0 Then
                            impxPart = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateCentriAziendaliXParticelle(dal, objParametri, centroF, part, SuperUserUsername)
                        Else
                            impxPart = (From ip In dal.ImpreseXParticelle
                                        Where ip.PIVA = piva And ip.PROV = particella.codProv And ip.COM = particella.codCom And ip.SEZIONE = sezione And ip.FOGLIO = particella.foglio And ip.NUMERO = particella.particella And ip.SUBALTERNO = subalterno And ip.Contratto_Cod = 0).FirstOrDefault
                        End If
                        impxPart.biologico = validaBooleano(conduzione.biologico)
                        impxPart.flagAnomaliaMacrouso = validaBooleano(conduzione.flagAnomaliaMacrouso)
                        impxPart.flagContenzioso = validaBooleano(conduzione.flagContenzioso)
                        impxPart.flagSupero = validaBooleano(conduzione.flagSupero)
                        impxPart.Irrigabilita = conduzione.irriguo
                        If conduzione.totSupPossesso IsNot Nothing AndAlso conduzione.totSupPossesso <> "" Then
                            impxPart.Sup_Condotta = strToDouble(conduzione.totSupPossesso) / 1000
                        Else
                            impxPart.Sup_Condotta = 0
                        End If
                        impxPart.RotazioneColturale = conduzione.rotazioneColturale
                        'impxPart.MarkAsModified()
                        dal.Entry(impxPart).State = EntityState.Modified
                    End If
                    dal.SaveChanges()

                    If conduzione.macrousi IsNot Nothing Then
                        For Each macrouso In conduzione.macrousi
                            Dim macro As ParticelleCatastalixMacrousi
                            If (From m In dal.ParticelleCatastalixMacrousi Where m.PIVA = piva And m.PROV = particella.codProv And m.COM = particella.codCom And m.SEZIONE = sezione And m.FOGLIO = particella.foglio And m.NUMERO = particella.particella And m.SUBALTERNO = subalterno And m.Macrouso_Cod = macrouso.codMacrouso).Count = 0 Then
                                macro = AgronicaCoreAnagrafeDAL.EFParticelle.CreaParticelleCatastalixMacrousi(dal, objParametri, part, macrouso.codMacrouso, SuperUserUsername)
                            Else
                                macro = (From m In dal.ParticelleCatastalixMacrousi Where m.PIVA = piva And m.PROV = particella.codProv And m.COM = particella.codCom And m.SEZIONE = sezione And m.FOGLIO = particella.foglio And m.NUMERO = particella.particella And m.SUBALTERNO = subalterno And m.Macrouso_Cod = macrouso.codMacrouso).FirstOrDefault
                            End If
                            macro.Fonte = macrouso.fonte
                            macro.PIVA = piva
                            macro.FonteDescr = macrouso.fonteDescr
                            macro.Superficie = strToDouble(macrouso.supMacrouso)
                            'dal.AddObject("macro", macro)
                            dal.Entry(macro).State = EntityState.Modified 'macro.MarkAsModified()
                        Next
                    End If
                    dal.SaveChanges()

                    If conduzione.proprietari IsNot Nothing Then
                        For Each proprietario In conduzione.proprietari
                            Dim id_CF As Integer
                            Dim cuaa As String = proprietario.cuaaProprietario
                            If cuaa.Length = 11 Then
                                id_CF = 0
                            Else
                                id_CF = 1
                            End If
                            Dim exists = False
                            Dim cc = (From c As Contatti In dal.Contatti Where c.Cod_Contatto = cuaa).FirstOrDefault
                            Dim contatto As Contatti
                            If cc Is Nothing Then
                                contatto = EFContatti.CreateContattiEF(dal, objParametri, piva, sa_cod, cuaa, id_CF, SuperUserUsername)
                                contatto.Rag_Soc = ""
                                contatto.Tipo_Indirizzo_Default = 0
                                contatto.Nome = ""
                                contatto.Cognome = ""
                                contatto.Data_Nascita = AGRODATAINIZIO
                                contatto.Sesso = ""
                                contatto.Cod_Contatto_Referente = ""
                                'contatto.MarkAsModified()
                                dal.Entry(contatto).State = EntityState.Modified
                            Else
                                contatto = cc
                                exists = True
                            End If
                            Dim risUm As Risorse_Umane
                            If exists Then
                                Dim RisUmNum = (From r As Risorse_Umane In dal.Risorse_Umane Where r.Piva = piva And r.Cod_Contatto = contatto.Cod_Contatto And r.Cod_Rapporto = -9).Count
                                If RisUmNum = 0 Then
                                    risUm = AgronicaCoreAnagrafeDAL.EFContatti.CreateRisorse_Umane(dal, objParametri, contatto, SuperUserUsername)
                                    risUm.Cod_Rapporto = -9
                                    dal.Entry(risUm).State = EntityState.Modified 'risUm.MarkAsModified()
                                Else
                                    risUm = (From r As Risorse_Umane In dal.Risorse_Umane Where r.Piva = piva And r.Cod_Contatto = contatto.Cod_Contatto And r.Cod_Rapporto = -9).FirstOrDefault
                                End If
                            Else
                                risUm = AgronicaCoreAnagrafeDAL.EFContatti.CreateRisorse_Umane(dal, objParametri, contatto, SuperUserUsername)
                                risUm.Cod_Rapporto = -9
                                dal.Entry(risUm).State = EntityState.Modified 'risUm.MarkAsModified()
                            End If

                            Dim impxpar_cont As ImpresexParticelle_Contatti
                            If (From ipc In dal.ImpresexParticelle_Contatti Where ipc.PROV = particella.codProv And ipc.COM = particella.codCom And ipc.SEZIONE = sezione And ipc.FOGLIO = particella.foglio And ipc.NUMERO = particella.particella And ipc.SUBALTERNO = subalterno And ipc.Piva = piva And ipc.Sa_Cod = sa_cod).Count = 0 Then
                                impxpar_cont = AgronicaCoreAnagrafeDAL.EFParticelle.CreateImpresexParticelle_Contatti(dal, objParametri, centroF, part, risUm, SuperUserUsername)
                            Else
                                impxpar_cont = (From ipc In dal.ImpresexParticelle_Contatti Where ipc.PROV = particella.codProv And ipc.COM = particella.codCom And ipc.SEZIONE = sezione And ipc.FOGLIO = particella.foglio And ipc.NUMERO = particella.particella And ipc.SUBALTERNO = particella.subalterno And ipc.Piva = piva And ipc.Sa_Cod = sa_cod).FirstOrDefault
                            End If
                            dal.SaveChanges()
                            If impxpar_cont IsNot Nothing AndAlso impxpar_cont.cuaaProprietario IsNot Nothing AndAlso contatto IsNot Nothing AndAlso contatto.Cod_Contatto IsNot Nothing Then
                                impxpar_cont.cuaaProprietario = contatto.Cod_Contatto
                                'impxpar_cont.MarkAsModified()
                                dal.Entry(impxpar_cont).State = EntityState.Modified
                            End If
                        Next
                    End If

                    dal.SaveChanges()
                    If conduzione.unitaVitate IsNot Nothing Then
                        If conduzione.unitaVitate.Length > 0 Then
                            Dim progTest As Programmazione_Testata '.CreateProgrammazione_Testata(pivaSuperUser, programmazioneTestataCod, piva)
                            Dim Programmazione_des_long As String = ""
                            If source.statoAzienda.dataValidazione IsNot Nothing Then
                                Programmazione_des_long = "Schedario Viticolo SIAR " + CStr(source.statoAzienda.dataValidazione)
                            Else
                                Programmazione_des_long = "Schedario Viticolo SIAR " + CStr(AGRODATAINIZIO)
                            End If
                            If (From pt In dal.Programmazione_Testata Where pt.Piva = piva And pt.Programmazione_Des_Long = Programmazione_des_long).Count = 0 Then
                                progTest = New Programmazione_Testata
                                progTest = EFProgrammazione_Testata.CreateProgrammazione_TestataEF(dal, pivaSuperUser, "", Programmazione_des_long, piva, objParametri, SuperUserUsername)
                            Else
                                progTest = (From pt In dal.Programmazione_Testata Where pt.Piva = piva And pt.Piva_SuperUser = pivaSuperUser).FirstOrDefault
                            End If
                            progTest.Piva_SuperUser = pivaSuperUser
                            progTest.Programmazione_Des = Programmazione_des_long
                            progTest.Programmazione_Des_Long = Programmazione_des_long

                            For Each unita In conduzione.unitaVitate
                                'DATI UNITA VITATA
                                Dim ent As Programmazione_Entita
                                If (From e In dal.Programmazione_Entita Where e.Programmazione_Testata.Programmazione_Cod = progTest.Programmazione_Cod And e.idUnitaVitata = unita.idUnitaVitata).Count = 0 Then
                                    ent = EFProgrammazione_Entita.CreateProgrammazione_EntitaEF(dal, pivaSuperUser, progTest.Programmazione_Cod, CStr(CInt(unita.idUnitaVitata)), piva, 0, 0, 0, 0, objParametri, SuperUserUsername)
                                Else
                                    ent = (From e In dal.Programmazione_Entita Where e.Programmazione_Testata.Programmazione_Cod = progTest.Programmazione_Cod And e.idUnitaVitata = unita.idUnitaVitata).FirstOrDefault
                                End If
                                ent.idUnitaVitata = unita.idUnitaVitata
                                'ent.altitudineSlm = unita.altitudineSlm
                                ent.AltriVitigniPresenti = unita.altriVitigniPresenti
                                ent.ancoraggiTestata = strToInteger(unita.ancoraggiTestata)
                                ent.annoRiferimento = unita.annoRiferimento
                                'ent.codFiliSostegno = unita.codFiliSostegno
                                ent.codPaliTessitura = unita.codPaliTessitura
                                ent.codPaliTestata = unita.codPaliTestata
                                ent.codStatoColt = unita.codStatoColt
                                ent.codTipoVari = unita.codTipoVari
                                ent.Cul_Cod = unita.codVitigno
                                ent.Validita_Fine = strToDatetime(unita.dataCessazione)
                                ent.DataProtocollo = strToDatetime(unita.dataProtocollo)
                                ent.DataRilievo = strToDatetime(unita.dataRilievo)
                                'ent.densita = unita.densita
                                ent.destProduttiva = unita.destProduttiva
                                ent.destProduttivaDescr = unita.destProduttivaDescr
                                ent.distanzaPali = unita.distanzaPali
                                ent.dtFine = strToDatetime(unita.dtFine)
                                ent.dtFineGestione = strToDatetime(unita.dtFineGestione)
                                ent.dtInizio = strToDatetime(unita.dtInizio)
                                ent.dtInizioGestione = strToDatetime(unita.dtInizioGestione)
                                ent.dtIns = strToDatetime(unita.dtIns)
                                ent.dtVar = strToDatetime(unita.dtVar)
                                ent.fallanzePerc = strToDouble(unita.fallanzePerc)
                                ent.flagAnomalia = validaBooleano(unita.flagAnomalia)
                                ent.flagAttuale = validaBooleano(unita.flagAttuale)
                                ent.flagCessata = validaBooleano(unita.flagCessata)
                                ent.flagContributo = validaBooleano(unita.flagContributo)
                                ent.flagRegolarizz2009 = validaBooleano(unita.flagRegolarizz2009)
                                ent.flagRicalcoloGis = validaBooleano(unita.flagRicalcoloGis)
                                ent.Foral_Cod = unita.formaAllevamento
                                ent.GiacituraTerreno = unita.giacituraTerreno
                                If unita.giornoImpianto IsNot Nothing AndAlso unita.giornoImpianto <> "" Then
                                    Dim data As Date
                                    Try
                                        data = New Date(CInt(unita.annoImpianto), CInt(unita.meseImpianto), CInt(unita.giornoImpianto))
                                        If data < AGRODATAINIZIO Then
                                            data = AGRODATAINIZIO
                                        End If
                                        If data > AGRODATAFINE Then
                                            data = AGRODATAFINE
                                        End If
                                    Catch ex As Exception
                                        data = AGRODATAINIZIO
                                    End Try
                                    ent.Validita_Inizio = data
                                End If
                                ent.idUtenteIns = unita.idUtenteIns
                                ent.idUtenteVar = unita.idUtenteVar
                                ent.Imp_Cod = unita.irrigazione
                                ent.Num_Piante = unita.numCeppi
                                ent.numeroProtocollo = unita.numeroProtocollo
                                ent.Unita_Vitata = unita.numUnitaVitata
                                ent.progPoligono = unita.numUnitaVitata
                                ent.SU_Fila = strToDouble(unita.sestoSuFila)
                                ent.TRA_Fila = strToDouble(unita.sestoTraFila)
                                ent.SuperficieServizioMq = strToInteger(unita.superficieServizioMq)
                                ent.supVitataDich = strToInteger(unita.supVitataDich)
                                'ent.supVitataDichPRicalcolo = unita.supVitataDichPRicalcolo
                                ent.Terrazzamenti = strToInteger(unita.terrazzamenti)
                                ent.TipoColtura = unita.tipoColtura
                                ent.tipoProcedimento = unita.tipoProcedimento
                                ent.tipoUnar = unita.tipoUnar
                                ent.tipoVariazione = unita.tipoVariazione
                                'ent.tipoVigneto = unita.tipoVigneto
                                ent.unar = unita.unar
                                'ent.vitignoDescr = unita.vitignoDescr
                                dal.Entry(ent).State = EntityState.Modified 'ent.MarkAsModified()
                                dal.SaveChanges()
                                'dal.Refresh(Objects.RefreshMode.ClientWins, ent)

                                Dim programmazione_part As Programmazione_Particelle
                                If (From pp As Programmazione_Particelle In dal.Programmazione_Particelle Where pp.Programmazione_Entita_Cod = ent.Programmazione_Entita_Cod _
                                    And pp.Prov = part.PROV _
                                    And pp.Com = part.COM _
                                    And pp.Sezione = part.SEZIONE _
                                    And pp.Foglio = part.FOGLIO _
                                    And pp.Numero = part.NUMERO _
                                    And pp.Subalterno = part.SUBALTERNO).Count = 0 Then
                                    programmazione_part = EFProgrammazione_Particelle.CreateProgrammazione_ParticelleEF(dal, pivaSuperUser, ent.Programmazione_Entita_Cod, part.PROV, part.COM, part.SEZIONE, part.FOGLIO, part.NUMERO, part.SUBALTERNO, objParametri, SuperUserUsername)
                                Else
                                    programmazione_part = (From pp As Programmazione_Particelle In dal.Programmazione_Particelle Where pp.Programmazione_Entita_Cod = ent.Programmazione_Entita_Cod _
                                    And pp.Prov = part.PROV _
                                    And pp.Com = part.COM _
                                    And pp.Sezione = part.SEZIONE _
                                    And pp.Foglio = part.FOGLIO _
                                    And pp.Numero = part.NUMERO _
                                    And pp.Subalterno = part.SUBALTERNO).FirstOrDefault

                                End If
                                programmazione_part.Superficie = 0
                                programmazione_part.inviato = 0
                                programmazione_part.Data_Modifica = DateTime.Now
                                programmazione_part.Username_Creazione = SuperUserUsername
                                programmazione_part.Username_Modifica = SuperUserUsername
                                programmazione_part.Validita_Fine = AGRODATAFINE
                                programmazione_part.Validita_Inizio = AGRODATAINIZIO
                                dal.Entry(programmazione_part).State = EntityState.Modified 'programmazione_part.MarkAsModified()
                                dal.SaveChanges()
                                'dal.Refresh(Objects.RefreshMode.ClientWins, programmazione_part)

                                If unita.altriVitigni IsNot Nothing Then
                                    For Each altroV In unita.altriVitigni
                                        Dim altraSpecieImpianti As AltraSpecie_Impianti '.CreateAltraSpecie_Impianti(piva, 0, 0, 0, entitaCod, 0)

                                        Dim prog As Integer = strToIntegerNotNothing(altroV.progr)
                                        If (From asi In dal.AltraSpecie_Impianti Where asi.PIVA = piva And asi.SA_COD = sa_cod And asi.APPEZZA = 0 And asi.ID_REG = 0 And asi.idUnitaVitata = unita.idUnitaVitata And asi.Programmazione_Cod = ent.Programmazione_Entita_Cod And asi.codVitigno = altroV.codVitigno And asi.progr = prog).Count = 0 Then
                                            Dim idAltraSpece = idGen.NuovoId_Tabella_EF(dal, "AltraSpecie_Impianti", 0, 200000000, objParametri)
                                            altraSpecieImpianti = New AltraSpecie_Impianti
                                            altraSpecieImpianti.PIVA = piva
                                            altraSpecieImpianti.SA_COD = sa_cod
                                            altraSpecieImpianti.APPEZZA = 0
                                            altraSpecieImpianti.ID_REG = 0
                                            altraSpecieImpianti.Programmazione_Cod = ent.Programmazione_Entita_Cod
                                            altraSpecieImpianti.ID = idAltraSpece
                                            dal.AltraSpecie_Impianti.Add(altraSpecieImpianti)
                                            dal.Entry(altraSpecieImpianti).State = EntityState.Added
                                            'altraSpecieImpianti.MarkAsAdded()
                                            dal.SaveChanges()
                                            'dal.Attach(altraSpecieImpianti)
                                        Else
                                            altraSpecieImpianti = (From asi In dal.AltraSpecie_Impianti Where asi.PIVA = piva And asi.SA_COD = sa_cod And asi.APPEZZA = 0 And asi.ID_REG = 0 And asi.idUnitaVitata = unita.idUnitaVitata And asi.Programmazione_Cod = ent.Programmazione_Entita_Cod And asi.codVitigno = altroV.codVitigno And asi.progr = prog).FirstOrDefault
                                        End If

                                        altraSpecieImpianti.codVitigno = altroV.codVitigno
                                        altraSpecieImpianti.descrVitigno = altroV.descrVitigno
                                        altraSpecieImpianti.Data_Creazione = strToDatetime(altroV.dtIns)
                                        altraSpecieImpianti.perc = strToDouble(altroV.perc)
                                        altraSpecieImpianti.progr = strToInteger(altroV.progr)


                                        'altraSpecieImpianti.MarkAsModified()
                                        dal.Entry(altraSpecieImpianti).State = EntityState.Modified
                                        dal.SaveChanges()
                                        'dal.Refresh(Objects.RefreshMode.ClientWins, altraSpecieImpianti)
                                        'dal.AddObject("altraSpeciaImpianti", altraSpeciaImpianti)
                                    Next
                                End If
                                If unita.idoneita IsNot Nothing Then
                                    For Each idoneita In unita.idoneita
                                        Dim codTipologia As String = strToStrObb(idoneita.codTipologia)
                                        Dim idUnitaVitata As String = strToStrObb(idoneita.idUnitaVitata)
                                        Dim idDocIgt As String = strToStrObb(idoneita.idDocigt)
                                        Dim ipi As Imprese_Progetti_Idoneita

                                        If (From ip In dal.Imprese_Progetti_Idoneita Where ip.Piva = piva And ip.idDocIgt = idDocIgt And ip.codTipologia = codTipologia And ip.idUnitaVitata = idUnitaVitata).Count = 0 Then
                                            ipi = New Imprese_Progetti_Idoneita
                                            ipi.Data_Creazione = DateTime.Now
                                            ipi.codTipologia = codTipologia
                                            ipi.idDocIgt = idDocIgt
                                            ipi.Piva = piva
                                            ipi.idUnitaVitata = idUnitaVitata
                                            ipi.Data_Creazione = DateTime.Now
                                            ipi.Piva_SuperUser = pivaSuperUser
                                            ipi.Sa_Cod = 0
                                            ipi.APPEZZA = 0
                                            ipi.ID_REG = 0
                                            ipi.Progetto_Cod = 0
                                            ipi.Programmazione_Entita_Cod = ent.Programmazione_Entita_Cod
                                            dal.Imprese_Progetti_Idoneita.Add(ipi)
                                            'ipi.MarkAsAdded()
                                            dal.SaveChanges()
                                            'dal.Attach(ipi)
                                            'dal.AcceptAllChanges()
                                            'dal.SaveChanges()
                                        Else
                                            ipi = (From ip In dal.Imprese_Progetti_Idoneita Where ip.Piva = piva And ip.idDocIgt = idDocIgt And ip.codTipologia = codTipologia And ip.idUnitaVitata = idUnitaVitata).FirstOrDefault
                                        End If
                                        ipi.dataRev = strToDatetime(idoneita.dataRev)
                                        ipi.dataRic = strToDatetime(idoneita.dataRic)
                                        ipi.DocIgtDescr = idoneita.docigtDescr
                                        ipi.idDocIgt = idoneita.idDocigt
                                        ipi.dtInizio = strToDatetime(idoneita.dtInizio)
                                        ipi.dtIns = strToDatetime(idoneita.dtIns)
                                        ipi.dtVar = strToDatetime(idoneita.dtVar)
                                        ipi.idUtenteIns = strToStrObb(idoneita.idUtenteIns)
                                        ipi.idUtenteVar = strToStrObb(idoneita.idUtenteVar)
                                        ipi.numIscrizione = strToStrObb(idoneita.numIscrizione)
                                        ipi.TipologiaDescr = strToStrObb(idoneita.tipologiaDescr)
                                        ipi.Username_Creazione = SuperUserUsername
                                        ipi.Username_Modifica = SuperUserUsername
                                        ipi.Validita_Fine = AGRODATAFINE
                                        ipi.Validita_Inizio = AGRODATAINIZIO
                                        ipi.Data_Modifica = DateTime.Now
                                        ipi.inviato = 0
                                        ipi.datainvio = DateTime.Now
                                        dal.SaveChanges()
                                        'dal.Refresh(Objects.RefreshMode.ClientWins, ipi)

                                        'dal.AcceptAllChanges()
                                    Next
                                End If
                                dal.Entry(progTest).State = EntityState.Modified 'progTest.MarkAsModified()
                                dal.SaveChanges()

                                'dal.Refresh(Objects.RefreshMode.ClientWins, progTest)
                                'dal.AcceptAllChanges()
                                'dal.SaveChanges()
                                returnStr = "idUnitaVitata: " + unita.idUnitaVitata
                                'dal.SaveChanges()
                            Next
                        End If
                    End If

                    If conduzione.zone IsNot Nothing Then
                        For Each zona In conduzione.zone
                            Dim zona_cod As Integer = gestioneZonaCod(zona.codZona, zona.zonaDescr, pivaSuperUser, dal)
                            Dim zonaxPar As ZonexParticelle '.CreateZonexParticelle(piva, zonaCod, particella.codProv, particella.codCom, particella.sezione, particella.foglio, particella.particella, particella.subalterno, DateTime.Now, DateTime.Now, pivaSuperUser, pivaSuperUser, AGRODATAINIZIO, AGRODATAFINE)
                            If (From z In dal.ZonexParticelle Where z.PROV = particella.codProv And z.COM = particella.codCom And z.SEZIONE = sezione And z.FOGLIO = particella.foglio And z.NUMERO = particella.particella And z.SUBALTERNO = subalterno And z.Zona_Cod = zona_cod).Count = 0 Then
                                zonaxPar = New ZonexParticelle
                                zonaxPar.Zona_Cod = zona_cod
                                zonaxPar.Piva_SuperUser = pivaSuperUser
                                zonaxPar.PROV = particella.codProv
                                zonaxPar.COM = particella.codCom
                                zonaxPar.SEZIONE = sezione
                                zonaxPar.FOGLIO = particella.foglio
                                zonaxPar.NUMERO = particella.particella
                                zonaxPar.SUBALTERNO = subalterno
                                zonaxPar.Data_Creazione = DateTime.Now
                                zonaxPar.Data_Modifica = DateTime.Now
                                zonaxPar.Username_Creazione = SuperUserUsername
                                zonaxPar.Username_Modifica = SuperUserUsername
                                zonaxPar.Validita_Inizio = AGRODATAINIZIO
                                zonaxPar.Validita_Fine = AGRODATAFINE
                                zonaxPar.Conforme = validaBooleano(zona.conforme)
                                zonaxPar.Fonte = zona.fonte
                                zonaxPar.FonteDescr = zona.fonteDescr
                                dal.ZonexParticelle.Add(zonaxPar)
                            Else
                                zonaxPar = (From z In dal.ZonexParticelle Where z.PROV = particella.codProv And z.COM = particella.codCom And z.SEZIONE = sezione And z.FOGLIO = particella.foglio And z.NUMERO = particella.particella And z.SUBALTERNO = subalterno And z.Zona_Cod = zona_cod).FirstOrDefault
                                zonaxPar.Zona_Cod = zona_cod
                                zonaxPar.Conforme = validaBooleano(zona.conforme)
                                zonaxPar.Fonte = zona.fonte
                                zonaxPar.FonteDescr = zona.fonteDescr
                            End If
                            dal.Entry(zonaxPar).State = EntityState.Modified 'zonaxPar.MarkAsModified()
                        Next
                    End If
                End If
                numParticella += 1
                dal.SaveChanges()
            Next
        End If

        Dim i = 1
        If source.unitaAziendali IsNot Nothing AndAlso
                    source.unitaAziendali.unita IsNot Nothing Then
            For Each uni In source.unitaAziendali.unita
                Dim centroF1 As Centri_Aziendali
                If (From c In dal.Centri_Aziendali Where c.PIVA = piva And c.sa_nome = "Centro Aziendale " + uni.progr).Count = 0 Then
                    centroF1 = EFCentri_Aziendali.CreateCentri_AziendaliEF(dal, objParametri, imp, "Centro Aziendale " + uni.progr, SuperUserUsername, objParametri_Utenti)
                Else
                    centroF1 = (From c In dal.Centri_Aziendali Where c.PIVA = piva And c.sa_nome = "Centro Aziendale " + uni.progr).FirstOrDefault
                End If
                If uni.dataDocumento IsNot Nothing AndAlso uni.dataDocumento <> "" Then
                    centroF1.dataDocumento = strToDatetime(uni.dataDocumento)
                End If
                If uni.dataFine IsNot Nothing AndAlso uni.dataFine <> "" Then
                    centroF1.Validita_Fine = strToDatetime(uni.dataFine)
                End If
                If uni.dataInizio IsNot Nothing AndAlso uni.dataInizio <> "" Then
                    centroF1.Validita_Inizio = strToDatetime(uni.dataInizio)
                End If
                If uni.dataFonte IsNot Nothing AndAlso uni.dataFonte <> "" Then
                    centroF1.dataFonte = strToDatetime(uni.dataFonte)
                End If
                centroF1.Documento = uni.documento
                If uni.flagLegale IsNot Nothing AndAlso uni.flagLegale <> "" Then
                    centroF1.flagLegale = validaBooleano(uni.flagLegale)
                End If
                If uni.flagPrincipale IsNot Nothing AndAlso uni.flagPrincipale <> "" Then
                    centroF1.flagPrincipale = validaBooleano(uni.flagPrincipale)
                End If
                If uni.fonte IsNot Nothing AndAlso uni.fonte <> "" Then
                    centroF1.fonte = validaBooleano(uni.fonte)
                End If
                If uni.fonteDescr IsNot Nothing AndAlso uni.fonteDescr <> "" Then
                    centroF1.fonteDescr = validaBooleano(uni.fonteDescr)
                End If

                If uni.provRea IsNot Nothing AndAlso uni.provRea <> "" AndAlso uni.numRea IsNot Nothing AndAlso uni.numRea <> "" Then
                    Dim centroCodice As Centri_Aziendali_Codici
                    If (From c In dal.Centri_Aziendali_Codici Where c.PIVA = piva And c.sa_cod = centroF1.sa_cod And c.id_cod = 1112).Count = 0 Then
                        centroCodice = EFCentri_Aziendali.CreateCentri_AziendaliCodici(dal, objParametri, centroF1, 1112, uni.provRea + " - " + uni.numRea, SuperUserUsername)
                    Else
                        centroCodice = (From c In dal.Centri_Aziendali_Codici Where c.PIVA = piva And c.sa_cod = centroF1.sa_cod And c.id_cod = 1112).FirstOrDefault
                        centroCodice.val_cod = uni.provRea + " - " + uni.numRea
                    End If
                End If

                If uni.sedeEstera IsNot Nothing Then
                    If uni.sedeEstera.citta IsNot Nothing Then
                        Dim indEstero As Indirizzi
                        If (From cai In dal.CentrixIndirizzi
                            Join iii In dal.Indirizzi On cai.cod_indirizzo Equals iii.cod_indirizzo
                            Where cai.PIVA = piva And cai.sa_cod = centroF1.sa_cod).Count = 0 Then
                            indEstero = EFCentri_Aziendali.CreateIndirizzoCentro(dal, objParametri, centroF1, 1, SuperUserUsername)
                        Else
                            indEstero = (From cai In dal.CentrixIndirizzi
                                         Join iii In dal.Indirizzi On cai.cod_indirizzo Equals iii.cod_indirizzo
                                         Where cai.PIVA = piva And cai.sa_cod = centroF1.sa_cod
                                         Select iii).FirstOrDefault
                        End If
                        indEstero.com_des = uni.sedeEstera.citta
                        indEstero.stato = uni.sedeEstera.codStato
                        dal.Entry(indEstero).State = EntityState.Modified 'indEstero.MarkAsModified()
                    End If
                End If
                If uni.sede IsNot Nothing Then
                    Dim ind As Indirizzi
                    If (From cai In dal.CentrixIndirizzi
                        Join iii In dal.Indirizzi On cai.cod_indirizzo Equals iii.cod_indirizzo
                        Where cai.PIVA = piva And cai.sa_cod = centroF1.sa_cod).Count = 0 Then
                        ind = EFCentri_Aziendali.CreateIndirizzoCentro(dal, objParametri, centroF1, 1, SuperUserUsername)
                    Else
                        ind = (From cai In dal.CentrixIndirizzi
                               Join iii In dal.Indirizzi On cai.cod_indirizzo Equals iii.cod_indirizzo
                               Where cai.PIVA = piva And cai.sa_cod = centroF1.sa_cod
                               Select iii).FirstOrDefault
                    End If
                    ind.CAP = uni.sede.cap
                    ind.com_cod_istat = uni.sede.codCom
                    ind.pro_cod_istat = uni.sede.codProv
                    ind.com_des = uni.sede.comDescr
                    ind.pro_cod = uni.sede.provSigla
                    ind.ind_des = uni.sede.indirizzo
                    ind.frz_des = uni.sede.localita
                    dal.Entry(ind).State = EntityState.Modified 'ind.MarkAsModified()
                End If
                i += 1
                dal.SaveChanges()
            Next
        End If

        dal.SaveChanges()

        imp = (From im As Imprese In dal.Imprese Where im.PIVA = piva).FirstOrDefault
        imp.dataValidazione = strToDate(source.statoAzienda.dataValidazione)
        'imp.MarkAsModified()
        dal.Entry(imp).State = EntityState.Modified
        dal.SaveChanges()

        'dal.AcceptAllChanges()
        dal.Database.Connection.Close()
        Dim endTime = DateTime.Now.Ticks
        Dim totalSecond As Double = (endTime - startMillis) / 1000
        'returnStr = "Execution Time: " + CStr(totalSecond)
        Return True
        'scope.Complete()
        'End Using
    End Function

    Public Shared Function ImportaUnitaVitate(source As FascicoloSiar2Response,
                                              objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByVal ControlExist As Boolean,
                                              Optional ByVal returnStr As String = "") As Boolean

        Dim pivaSuperUser As String = objParametri_Utenti.PivaSuperUser
        Dim SuperUserUsername As String = objParametri_Utenti.SuperUserUsername

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility

        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Dim dal As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(EFConnString)
        'dal.ContextOptions.UseLegacyPreserveChangesBehavior = False
        dal.Database.Connection.Open()
        'disabilitaMergeOptions(dal)
        Dim idGen = New AgronicaCoreDataProvider.Agro_Sequenze
        Dim validazione As Integer
        'Using scope As New TransactionScope
        Try
            Dim piva As String
            If source.datiAnagrafici.partitaIva Is Nothing Or source.datiAnagrafici.partitaIva = "" Then
                piva = source.datiAnagrafici.cuaa.Substring(0, 11)
                validazione = 1
            Else
                piva = source.datiAnagrafici.partitaIva
                validazione = 0
            End If
            Dim impresaExist = (From im As Imprese In dal.Imprese Where im.PIVA = piva).Count
            Dim imp As AgronicaCoreEntityFramework_POCO.Imprese
            If impresaExist <> 0 Then
                imp = (From im As Imprese In dal.Imprese Where im.PIVA = piva).FirstOrDefault
                If ControlExist Then
                    Dim dataValidazione As DateTime
                    Dim datavalidazione_impresa As DateTime = imp.dataValidazione
                    Dim dataValiazioneStr As String = source.statoAzienda.dataValidazione
                    If source.statoAzienda.dataValidazione Is Nothing Then
                        dataValiazioneStr = source.datiAnagrafici.dtValidazione
                    End If
                    dataValidazione = CDate(dataValiazioneStr)
                    If datavalidazione_impresa = dataValidazione Then
                        Return False
                    End If
                End If
                If (From gi In dal.GerarchiaImprese Where gi.Figlio = piva).Count = 0 Then
                    EFImprese.CreateGerarchiaImprese(dal, objParametri, pivaSuperUser, piva, 1, 2, pivaSuperUser, objParametri_Utenti)
                End If
                If (From ui In dal.UtentiXImprese Where ui.PIVA = piva).Count = 0 Then
                    EFImprese.CreateUtentixImprese(dal, objParametri, pivaSuperUser, piva, pivaSuperUser)
                End If
            Else
                imp = EFImprese.CreaImpresaCompleta(dal, objParametri, piva, source.datiAnagrafici.ragioneSociale, "", AGRODATAINIZIO, SuperUserUsername, pivaSuperUser, objParametri_Utenti)
            End If
            'imp.MarkAsModified()
            dal.Entry(imp).State = EntityState.Modified

            Dim centroF As Centri_Aziendali
            If (From ceaz As Centri_Aziendali In dal.Centri_Aziendali Where ceaz.PIVA = piva And ceaz.sa_nome.Contains("CentroAziendaleFittizio0")).Count = 0 Then
                centroF = EFCentri_Aziendali.CreateCentri_AziendaliEF(dal, objParametri, imp, "CentroAziendaleFittizio0", SuperUserUsername, objParametri_Utenti)
                Dim indirizzoCentro = EFCentri_Aziendali.CreateIndirizzoCentro(dal, objParametri, centroF, 0, SuperUserUsername)
            Else
                centroF = (From ceaz As Centri_Aziendali In dal.Centri_Aziendali Where ceaz.PIVA = piva And ceaz.sa_nome = "CentroAziendaleFittizio0").FirstOrDefault
            End If
            Dim sa_cod = centroF.sa_cod
            'dal.SaveChanges()
            'dal.AcceptAllChanges()
            dal.SaveChanges()

            If source.conduzioniTerreni IsNot Nothing AndAlso
                source.conduzioniTerreni.particelle IsNot Nothing Then
                For Each particella In source.conduzioniTerreni.particelle
                    Dim sezione As String = "0"
                    If particella.sezione IsNot Nothing AndAlso particella.sezione <> "" Then
                        sezione = particella.sezione
                    End If
                    Dim subalterno As String = "0"
                    If particella.subalterno IsNot Nothing AndAlso particella.subalterno <> "" And particella.subalterno <> "000" Then
                        subalterno = particella.sezione
                    End If
                    Dim part As ParticelleCatastali
                    Dim particellaProv = particella.codProv
                    Dim particellaCom = particella.codCom
                    Dim particellaFoglio = particella.foglio
                    Dim particellaNumero = particella.particella
                    If (From ip In dal.ParticelleCatastali
                        Where ip.PROV = particellaProv And ip.COM = particellaCom And ip.SEZIONE = sezione And ip.FOGLIO = particellaFoglio And ip.NUMERO = particellaNumero And ip.SUBALTERNO = subalterno).Count = 0 Then
                        part = AgronicaCoreAnagrafeDAL.EFParticelle.CreateParticelleCatastaliEF(dal, objParametri, particella.codProv, particella.codCom, sezione, particella.foglio, particella.particella, subalterno, SuperUserUsername)
                    Else
                        part = (From ip In dal.ParticelleCatastali
                                Where ip.PROV = particellaProv And ip.COM = particellaCom And ip.SEZIONE = sezione And ip.FOGLIO = particellaFoglio And ip.NUMERO = particellaNumero And ip.SUBALTERNO = subalterno).FirstOrDefault
                    End If

                    Dim conduzione = particella.conduzione
                    If conduzione IsNot Nothing Then
                        If conduzione.unitaVitate IsNot Nothing Then
                            If conduzione.unitaVitate.Length > 0 Then
                                Dim progTest As Programmazione_Testata '.CreateProgrammazione_Testata(pivaSuperUser, programmazioneTestataCod, piva)
                                Dim Programmazione_des_long As String = ""
                                If source.statoAzienda.dataValidazione IsNot Nothing Then
                                    Programmazione_des_long = "Schedario Viticolo SIAR " + CStr(source.statoAzienda.dataValidazione)
                                Else
                                    Programmazione_des_long = "Schedario Viticolo SIAR " + CStr(AGRODATAINIZIO)
                                End If
                                If (From pt In dal.Programmazione_Testata Where pt.Piva = piva And pt.Programmazione_Des_Long = Programmazione_des_long).Count = 0 Then
                                    progTest = New Programmazione_Testata
                                    progTest = EFProgrammazione_Testata.CreateProgrammazione_TestataEF(dal, pivaSuperUser, "", Programmazione_des_long, piva, objParametri, SuperUserUsername)
                                Else
                                    progTest = (From pt In dal.Programmazione_Testata Where pt.Piva = piva And pt.Piva_SuperUser = pivaSuperUser).FirstOrDefault
                                End If
                                progTest.Piva_SuperUser = pivaSuperUser

                                For Each unita In conduzione.unitaVitate
                                    'DATI UNITA VITATA
                                    Dim ent As Programmazione_Entita
                                    If (From e In dal.Programmazione_Entita Where e.Programmazione_Testata.Programmazione_Cod = progTest.Programmazione_Cod And e.idUnitaVitata = unita.idUnitaVitata).Count = 0 Then
                                        ent = EFProgrammazione_Entita.CreateProgrammazione_EntitaEF(dal, pivaSuperUser, progTest.Programmazione_Cod, CStr(CInt(unita.idUnitaVitata)), piva, 0, 0, 0, 0, objParametri, SuperUserUsername)
                                    Else
                                        ent = (From e In dal.Programmazione_Entita Where e.Programmazione_Testata.Programmazione_Cod = progTest.Programmazione_Cod And e.idUnitaVitata = unita.idUnitaVitata).FirstOrDefault
                                    End If
                                    ent.idUnitaVitata = unita.idUnitaVitata
                                    'ent.altitudineSlm = unita.altitudineSlm
                                    ent.AltriVitigniPresenti = unita.altriVitigniPresenti
                                    ent.ancoraggiTestata = unita.ancoraggiTestata
                                    ent.annoRiferimento = unita.annoRiferimento
                                    'ent.codFiliSostegno = unita.codFiliSostegno
                                    ent.codPaliTessitura = unita.codPaliTessitura
                                    ent.codPaliTestata = unita.codPaliTestata
                                    ent.codStatoColt = unita.codStatoColt
                                    ent.codTipoVari = unita.codTipoVari
                                    ent.Cul_Cod = unita.codVitigno
                                    ent.Validita_Fine = unita.dataCessazione
                                    ent.DataProtocollo = strToDatetime(unita.dataProtocollo)
                                    ent.DataRilievo = strToDatetime(unita.dataRilievo)
                                    'ent.densita = unita.densita
                                    ent.destProduttiva = unita.destProduttiva
                                    ent.destProduttivaDescr = unita.destProduttivaDescr
                                    ent.distanzaPali = unita.distanzaPali
                                    ent.dtFine = strToDatetime(unita.dtFine)
                                    ent.dtFineGestione = strToDatetime(unita.dtFineGestione)
                                    ent.dtInizio = strToDatetime(unita.dtInizio)
                                    ent.dtInizioGestione = strToDatetime(unita.dtInizioGestione)
                                    ent.dtIns = strToDatetime(unita.dtIns)
                                    ent.dtVar = strToDatetime(unita.dtVar)
                                    ent.fallanzePerc = strToDouble(unita.fallanzePerc)
                                    ent.flagAnomalia = validaBooleano(unita.flagAnomalia)
                                    ent.flagAttuale = validaBooleano(unita.flagAttuale)
                                    ent.flagCessata = validaBooleano(unita.flagCessata)
                                    ent.flagContributo = validaBooleano(unita.flagContributo)
                                    ent.flagRegolarizz2009 = validaBooleano(unita.flagRegolarizz2009)
                                    ent.flagRicalcoloGis = validaBooleano(unita.flagRicalcoloGis)
                                    ent.Foral_Cod = unita.formaAllevamento
                                    ent.GiacituraTerreno = unita.giacituraTerreno
                                    If unita.giornoImpianto IsNot Nothing AndAlso unita.giornoImpianto <> "" Then
                                        Dim data As New Date(CInt(unita.annoImpianto), CInt(unita.meseImpianto), CInt(unita.giornoImpianto))
                                        ent.Validita_Inizio = data
                                    End If
                                    ent.idUtenteIns = unita.idUtenteIns
                                    ent.idUtenteVar = unita.idUtenteVar
                                    ent.Imp_Cod = unita.irrigazione
                                    ent.Num_Piante = unita.numCeppi
                                    ent.numeroProtocollo = unita.numeroProtocollo
                                    ent.Unita_Vitata = unita.numUnitaVitata
                                    ent.progPoligono = unita.numUnitaVitata
                                    ent.SU_Fila = strToDouble(unita.sestoSuFila)
                                    ent.TRA_Fila = strToDouble(unita.sestoTraFila)
                                    ent.SuperficieServizioMq = strToInteger(unita.superficieServizioMq)
                                    ent.supVitataDich = strToInteger(unita.supVitataDich)
                                    'ent.supVitataDichPRicalcolo = unita.supVitataDichPRicalcolo
                                    ent.Terrazzamenti = strToInteger(unita.terrazzamenti)
                                    ent.TipoColtura = unita.tipoColtura
                                    ent.tipoProcedimento = unita.tipoProcedimento
                                    ent.tipoUnar = unita.tipoUnar
                                    ent.tipoVariazione = unita.tipoVariazione
                                    'ent.tipoVigneto = unita.tipoVigneto
                                    ent.unar = unita.unar
                                    'ent.vitignoDescr = unita.vitignoDescr
                                    dal.Entry(ent).State = EntityState.Modified 'ent.MarkAsModified()
                                    dal.SaveChanges()
                                    'dal.Refresh(Objects.RefreshMode.ClientWins, ent)

                                    Dim programmazione_part As Programmazione_Particelle
                                    If (From pp As Programmazione_Particelle In dal.Programmazione_Particelle Where pp.Programmazione_Entita_Cod = ent.Programmazione_Entita_Cod _
                                        And pp.Prov = part.PROV _
                                        And pp.Com = part.COM _
                                        And pp.Sezione = part.SEZIONE _
                                        And pp.Foglio = part.FOGLIO _
                                        And pp.Numero = part.NUMERO _
                                        And pp.Subalterno = part.SUBALTERNO).Count = 0 Then
                                        programmazione_part = EFProgrammazione_Particelle.CreateProgrammazione_ParticelleEF(dal, pivaSuperUser, ent.Programmazione_Entita_Cod, part.PROV, part.COM, part.SEZIONE, part.FOGLIO, part.NUMERO, part.SUBALTERNO, objParametri, SuperUserUsername)
                                    Else
                                        programmazione_part = (From pp As Programmazione_Particelle In dal.Programmazione_Particelle Where pp.Programmazione_Entita_Cod = ent.Programmazione_Entita_Cod _
                                        And pp.Prov = part.PROV _
                                        And pp.Com = part.COM _
                                        And pp.Sezione = part.SEZIONE _
                                        And pp.Foglio = part.FOGLIO _
                                        And pp.Numero = part.NUMERO _
                                        And pp.Subalterno = part.SUBALTERNO).FirstOrDefault

                                    End If
                                    programmazione_part.Superficie = 0
                                    programmazione_part.inviato = 0
                                    programmazione_part.Data_Modifica = DateTime.Now
                                    programmazione_part.Username_Creazione = SuperUserUsername
                                    programmazione_part.Username_Modifica = SuperUserUsername
                                    programmazione_part.Validita_Fine = AGRODATAFINE
                                    programmazione_part.Validita_Inizio = AGRODATAINIZIO
                                    dal.Entry(programmazione_part).State = EntityState.Modified 'programmazione_part.MarkAsModified()
                                    dal.SaveChanges()
                                    'dal.Refresh(Objects.RefreshMode.ClientWins, programmazione_part)

                                    If unita.altriVitigni IsNot Nothing Then
                                        For Each altroV In unita.altriVitigni
                                            Dim altraSpecieImpianti As AltraSpecie_Impianti '.CreateAltraSpecie_Impianti(piva, 0, 0, 0, entitaCod, 0)

                                            Dim prog As Integer = strToIntegerNotNothing(altroV.progr)
                                            If (From asi In dal.AltraSpecie_Impianti Where asi.PIVA = piva And asi.SA_COD = sa_cod And asi.APPEZZA = 0 And asi.ID_REG = 0 And asi.idUnitaVitata = unita.idUnitaVitata And asi.Programmazione_Cod = ent.Programmazione_Entita_Cod And asi.codVitigno = altroV.codVitigno And asi.progr = prog).Count = 0 Then
                                                Dim idAltraSpece = idGen.NuovoId_Tabella_EF(dal, "AltraSpecie_Impianti", 0, 200000000, objParametri)
                                                altraSpecieImpianti = New AltraSpecie_Impianti
                                                altraSpecieImpianti.PIVA = piva
                                                altraSpecieImpianti.SA_COD = sa_cod
                                                altraSpecieImpianti.APPEZZA = 0
                                                altraSpecieImpianti.ID_REG = 0
                                                altraSpecieImpianti.Programmazione_Cod = ent.Programmazione_Entita_Cod
                                                altraSpecieImpianti.ID = idAltraSpece
                                                dal.AltraSpecie_Impianti.Add(altraSpecieImpianti)
                                                dal.Entry(altraSpecieImpianti).State = EntityState.Added
                                                'altraSpecieImpianti.MarkAsAdded()
                                                dal.SaveChanges()
                                                'dal.Attach(altraSpecieImpianti)
                                            Else
                                                altraSpecieImpianti = (From asi In dal.AltraSpecie_Impianti Where asi.PIVA = piva And asi.SA_COD = sa_cod And asi.APPEZZA = 0 And asi.ID_REG = 0 And asi.idUnitaVitata = unita.idUnitaVitata And asi.Programmazione_Cod = ent.Programmazione_Entita_Cod And asi.codVitigno = altroV.codVitigno And asi.progr = prog).FirstOrDefault
                                            End If

                                            altraSpecieImpianti.codVitigno = altroV.codVitigno
                                            altraSpecieImpianti.descrVitigno = altroV.descrVitigno
                                            altraSpecieImpianti.Data_Creazione = strToDatetime(altroV.dtIns)
                                            altraSpecieImpianti.perc = strToDouble(altroV.perc)
                                            altraSpecieImpianti.progr = strToInteger(altroV.progr)


                                            'altraSpecieImpianti.MarkAsModified()
                                            dal.Entry(altraSpecieImpianti).State = EntityState.Modified
                                            dal.SaveChanges()
                                            'dal.Refresh(Objects.RefreshMode.ClientWins, altraSpecieImpianti)
                                            'dal.AddObject("altraSpeciaImpianti", altraSpeciaImpianti)
                                        Next
                                    End If
                                    If unita.idoneita IsNot Nothing Then
                                        For Each idoneita In unita.idoneita
                                            Dim codTipologia As String = strToStrObb(idoneita.codTipologia)
                                            Dim idUnitaVitata As String = strToStrObb(idoneita.idUnitaVitata)
                                            Dim idDocIgt As String = strToStrObb(idoneita.idDocigt)
                                            Dim ipi As Imprese_Progetti_Idoneita

                                            If (From ip In dal.Imprese_Progetti_Idoneita Where ip.Piva = piva And ip.idDocIgt = idDocIgt And ip.codTipologia = codTipologia And ip.idUnitaVitata = idUnitaVitata).Count = 0 Then
                                                ipi = New Imprese_Progetti_Idoneita
                                                ipi.Data_Creazione = DateTime.Now
                                                ipi.codTipologia = codTipologia
                                                ipi.idDocIgt = idDocIgt
                                                ipi.Piva = piva
                                                ipi.idUnitaVitata = idUnitaVitata
                                                ipi.Data_Creazione = DateTime.Now
                                                ipi.Piva_SuperUser = pivaSuperUser
                                                ipi.Sa_Cod = 0
                                                ipi.APPEZZA = 0
                                                ipi.ID_REG = 0
                                                ipi.Progetto_Cod = 0
                                                ipi.Programmazione_Entita_Cod = ent.Programmazione_Entita_Cod
                                                dal.Imprese_Progetti_Idoneita.Add(ipi)
                                                'ipi.MarkAsAdded()
                                                dal.SaveChanges()
                                                'dal.Attach(ipi)
                                                'dal.AcceptAllChanges()
                                                'dal.SaveChanges()
                                            Else
                                                ipi = (From ip In dal.Imprese_Progetti_Idoneita Where ip.Piva = piva And ip.idDocIgt = idDocIgt And ip.codTipologia = codTipologia And ip.idUnitaVitata = idUnitaVitata).FirstOrDefault
                                            End If
                                            ipi.dataRev = strToDatetime(idoneita.dataRev)
                                            ipi.dataRic = strToDatetime(idoneita.dataRic)
                                            ipi.DocIgtDescr = idoneita.docigtDescr
                                            ipi.idDocIgt = idoneita.idDocigt
                                            ipi.dtInizio = strToDatetime(idoneita.dtInizio)
                                            ipi.dtIns = strToDatetime(idoneita.dtIns)
                                            ipi.dtVar = strToDatetime(idoneita.dtVar)
                                            ipi.idUtenteIns = strToStrObb(idoneita.idUtenteIns)
                                            ipi.idUtenteVar = strToStrObb(idoneita.idUtenteVar)
                                            ipi.numIscrizione = strToStrObb(idoneita.numIscrizione)
                                            ipi.TipologiaDescr = strToStrObb(idoneita.tipologiaDescr)
                                            ipi.Username_Creazione = SuperUserUsername
                                            ipi.Username_Modifica = SuperUserUsername
                                            ipi.Validita_Fine = AGRODATAFINE
                                            ipi.Validita_Inizio = AGRODATAINIZIO
                                            ipi.Data_Modifica = DateTime.Now
                                            ipi.inviato = 0
                                            ipi.datainvio = DateTime.Now
                                            dal.SaveChanges()
                                            'dal.Refresh(Objects.RefreshMode.ClientWins, ipi)

                                            'dal.AcceptAllChanges()
                                        Next
                                    End If
                                    dal.Entry(progTest).State = EntityState.Modified 'progTest.MarkAsModified()
                                    dal.SaveChanges()

                                    'dal.Refresh(Objects.RefreshMode.ClientWins, progTest)
                                    'dal.AcceptAllChanges()
                                    'dal.SaveChanges()
                                    returnStr = "idUnitaVitata: " + unita.idUnitaVitata
                                    'dal.SaveChanges()
                                Next
                            End If
                        End If
                    End If
                Next
            End If

            'dal.AcceptAllChanges()
            dal.SaveChanges()
            'scope.Complete()
            'scope.Dispose()
        Catch ex As OptimisticConcurrencyException
            Throw ex ' EF6 rilancio l'eccezione in attesa di capire come gestirlo
            'For Each elem In dal.MetadataWorkspace.GetItemCollection(Metadata.Edm.DataSpace.SSpace)
            '    dal.Refresh(Objects.RefreshMode.ClientWins, elem)
            'Next
            'dal.SaveChanges()
        End Try
        'End Using
    End Function

    'Public Shared Sub disabilitaMergeOptions(ByRef dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities)
    '    dal.Programmazione_Testata.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Programmazione_Entita.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Programmazione_Particelle.MergeOption = Objects.MergeOption.NoTracking
    '    dal.ParticelleCatastali.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Imprese_Progetti_Idoneita.MergeOption = Objects.MergeOption.NoTracking
    '    dal.AltraSpecie_Impianti.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Imprese.MergeOption = Objects.MergeOption.NoTracking
    '    dal.GerarchiaImprese.MergeOption = Objects.MergeOption.NoTracking
    '    dal.UtentiXImprese.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Imprese_Codici.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Indirizzi.MergeOption = Objects.MergeOption.NoTracking
    '    dal.ImpresexIndirizzi.MergeOption = Objects.MergeOption.NoTracking
    '    dal.ISTAT.MergeOption = Objects.MergeOption.NoTracking
    '    dal.CAA.MergeOption = Objects.MergeOption.NoTracking
    '    dal.IscrizioneCAA.MergeOption = Objects.MergeOption.NoTracking
    '    dal.ProduzioniQualita.MergeOption = Objects.MergeOption.NoTracking
    '    dal.DirittiReimpianti.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Contatti.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Rubrica.MergeOption = Objects.MergeOption.NoTracking
    '    dal.ContattiXRubrica.MergeOption = Objects.MergeOption.NoTracking
    '    dal.ContattiXIndirizzi.MergeOption = Objects.MergeOption.NoTracking
    '    dal.ISTAT_Comuni.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Risorse_Umane.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Centri_Aziendali.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Ist_Credito.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Liquidita.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Fabbricati.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Stalla.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Agenda.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Movimenti.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Movimenti_dettagli.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Mov_Destinazioni.MergeOption = Objects.MergeOption.NoTracking
    '    dal.ParticelleCatastali.MergeOption = Objects.MergeOption.NoTracking
    '    dal.ParticelleCatastalixEleggibilitaParticelle.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Imprese_Contratti.MergeOption = Objects.MergeOption.NoTracking
    '    dal.ImpreseXParticelle.MergeOption = Objects.MergeOption.NoTracking
    '    dal.ParticelleCatastalixMacrousi.MergeOption = Objects.MergeOption.NoTracking
    '    dal.ImpresexParticelle_Contatti.MergeOption = Objects.MergeOption.NoTracking
    '    dal.ZonexParticelle.MergeOption = Objects.MergeOption.NoTracking
    '    dal.Centri_Aziendali_Codici.MergeOption = Objects.MergeOption.NoTracking
    '    dal.CentrixIndirizzi.MergeOption = Objects.MergeOption.NoTracking
    'End Sub

    'Public Shared Sub aggiornaEntita(dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities, entity As AgronicaCoreEntityFramework_POCO.IObjectWithChangeTracker)
    '    Dim state = entity.ChangeTracker.State
    '    Select Case state
    '        Case ObjectState.Added
    '        Case ObjectState.Deleted
    '            entity.ChangeTracker.State = ObjectState.Modified
    '        Case ObjectState.Modified
    '            entity.ChangeTracker.State = ObjectState.Modified
    '        Case ObjectState.Unchanged
    '            'entity.ChangeTracker.State = ObjectState.Modified
    '    End Select
    'End Sub

    'Public Shared Function ConvertiEntityFramework1(pivaSuperUser As String, source As FascicoloSiar2Response, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Impresa
    '    Dim numParticella As Integer = 0
    '    Try
    '        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility

    '        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

    '        Dim dal As New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(EFConnString)
    '        dal.Connection.Open()
    '        Dim idGen = New AgronicaCoreDataProvider.Agro_Sequenze
    '        Dim validazione As Integer
    '        Dim piva As String
    '        If source.datiAnagrafici.partitaIva Is Nothing Or source.datiAnagrafici.partitaIva = "" Then
    '            piva = source.datiAnagrafici.cuaa.Substring(0, 11)
    '            validazione = 1
    '        Else
    '            piva = source.datiAnagrafici.partitaIva
    '            validazione = 0
    '        End If

    '        Dim impresaExist = (From im As Imprese In dal.Imprese Where im.PIVA = piva).Count
    '        Dim imp As AgronicaCoreEntityFramework_POCO.Imprese
    '        If impresaExist <> 0 Then
    '            imp = (From im As Imprese In dal.Imprese Where im.PIVA = piva).FirstOrDefault
    '        Else
    '            imp = AgronicaCoreAnagrafeDAL.EFImprese.CreaImpresaCompleta(dal, objParametri, piva, source.datiAnagrafici.ragioneSociale, pivaSuperUser, pivaSuperUser)
    '        End If

    '        If source.statoAzienda.dataCessazione IsNot Nothing AndAlso source.statoAzienda.dataCessazione <> "" Then
    '            imp.Validita_Fine = strToDatetime(source.statoAzienda.dataCessazione)
    '        Else
    '            imp.Validita_Fine = AGRODATAFINE
    '        End If
    '        imp.aziendaValidata = validaBooleano(source.statoAzienda.aziendaValidata)
    '        imp.aziendaCessata = validaBooleano(source.statoAzienda.aziendaCessata)
    '        imp.aziendaIscrittaCAA = validaBooleano(source.statoAzienda.aziendaIscrittaCaa)
    '        imp.Validazione = validazione
    '        imp.aziendaPresente = validaBooleano(source.statoAzienda.aziendaPresente)
    '        imp.dataIscrizioneCAA = strToDatetime(source.statoAzienda.dataIscrizioneCaa)
    '        imp.dataValidazione = strToDatetime(source.statoAzienda.dataValidazione)
    '        imp.dataVariazioneAzienda = strToDatetime(source.statoAzienda.dataVariazioneAzienda)
    '        imp.maxDataVariazioneIbanAzienda = strToDatetime(source.statoAzienda.maxDataVariazioneIbanAzienda)
    '        imp.maxDataVariazionePersoneAzienda = strToDatetime(source.statoAzienda.maxDataVariazionePersoneAzienda)
    '        imp.maxDataVariazionePossessiAzienda = strToDatetime(source.statoAzienda.maxDataVariazionePossessiAzienda)
    '        imp.codEsenzione = source.datiAnagrafici.codEsenzione
    '        imp.codOp = source.datiAnagrafici.codOp
    '        imp.documento = source.datiAnagrafici.documento
    '        imp.dtDocumento = strToDatetime(source.datiAnagrafici.dtDocumento)
    '        If source.datiAnagrafici.dtCessazione IsNot Nothing AndAlso source.datiAnagrafici.dtCessazione <> "" Then
    '            imp.Validita_Fine = strToDatetime(source.datiAnagrafici.dtCessazione)
    '        Else
    '            imp.Validita_Fine = AGRODATAFINE
    '        End If
    '        imp.dtValidazione = strToDatetime(source.datiAnagrafici.dtValidazione)
    '        imp.Data_Modifica = strToDatetime(source.datiAnagrafici.dtVariazione)
    '        imp.esenzioneDescr = source.datiAnagrafici.esenzioneDescr
    '        imp.flagAltreSedi = validaBooleano(source.datiAnagrafici.flagAltreSedi)
    '        imp.flagValidato = validaBooleano(source.datiAnagrafici.flagValidato)
    '        imp.Forma_Giuridica = source.datiAnagrafici.formaGiuridica
    '        imp.idUtenteValidazione = source.datiAnagrafici.idUtenteValidazione
    '        imp.opDescr = source.datiAnagrafici.opDescr

    '        imp.MarkAsModified()

    '        dal.SaveChanges()

    '        If source.datiAnagrafici.cuaa IsNot Nothing AndAlso source.datiAnagrafici.cuaa <> "" Then
    '            Dim impCod As Imprese_Codici
    '            If (From ic As Imprese_Codici In dal.Imprese_Codici Where ic.PIVA = piva And ic.id_cod = 1010).Count = 0 Then
    '                impCod = AgronicaCoreAnagrafeDAL.EFImprese.CreateImprese_CodiciEF(dal, objParametri, imp, 1010, source.datiAnagrafici.cuaa, pivaSuperUser)
    '            Else
    '                impCod = (From ic As Imprese_Codici In dal.Imprese_Codici Where ic.PIVA = piva And ic.id_cod = 1010).FirstOrDefault
    '                impCod.val_cod = source.datiAnagrafici.cuaa
    '                impCod.MarkAsModified()
    '            End If
    '        End If

    '        If source.datiAnagrafici.provRea IsNot Nothing AndAlso source.datiAnagrafici.provRea <> "" AndAlso _
    '                        source.datiAnagrafici.numRea IsNot Nothing AndAlso source.datiAnagrafici.numRea <> "" Then
    '            Dim impCod As Imprese_Codici
    '            If (From ic As Imprese_Codici In dal.Imprese_Codici Where ic.PIVA = piva And ic.id_cod = 1112).Count = 0 Then
    '                impCod = AgronicaCoreAnagrafeDAL.EFImprese.CreateImprese_CodiciEF(dal, objParametri, imp, 1112, source.datiAnagrafici.provRea + " - " + source.datiAnagrafici.numRea, pivaSuperUser)
    '            Else
    '                impCod = (From ic As Imprese_Codici In dal.Imprese_Codici Where ic.PIVA = piva And ic.id_cod = 1112).FirstOrDefault
    '                impCod.val_cod = source.datiAnagrafici.provRea + " - " + source.datiAnagrafici.numRea
    '                impCod.MarkAsModified()
    '            End If
    '        End If



    '        If source.datiAnagrafici.cittaEsteraLegale IsNot Nothing Then
    '            If source.datiAnagrafici.cittaEsteraLegale.statoDescr IsNot Nothing AndAlso _
    '                source.datiAnagrafici.cittaEsteraLegale.citta IsNot Nothing AndAlso _
    '                source.datiAnagrafici.cittaEsteraLegale.statoDescr <> "" AndAlso _
    '                source.datiAnagrafici.cittaEsteraLegale.citta <> "" Then
    '                Dim indirizzo1 = AgronicaCoreAnagrafeDAL.EFImprese.CreateIndirizzoImpresa(dal, objParametri, imp, 1, pivaSuperUser)
    '                indirizzo1.stato = source.datiAnagrafici.cittaEsteraLegale.statoDescr
    '                indirizzo1.com_des = source.datiAnagrafici.cittaEsteraLegale.citta
    '                indirizzo1.MarkAsModified()
    '                dal.SaveChanges()
    '                'dal.AddObject("ImpresexIndirizzi", impxind)
    '            End If
    '        End If

    '        dal.SaveChanges()

    '        Dim numCaa = (From c As CAA In dal.CAAs Where c.idCAA = source.iscrizioneCAA.idCAA).Count
    '        Dim ca As CAA
    '        If numCaa = 0 Then
    '            ca = New CAA '.CreateCAA(source.iscrizioneCAA.codFiscaleCAA, source.iscrizioneCAA.idCAA)
    '            ca.codFiscaleCAA = source.iscrizioneCAA.codFiscaleCAA
    '            ca.idCAA = source.iscrizioneCAA.idCAA
    '            ca.Denominazione = source.iscrizioneCAA.denominazione
    '            Dim codIndirizzo = idGen.NuovoId_Tabella_EF(dal, "Indirizzi", 0, 20000000, objParametri)
    '            'Dim indirizzoCAA As New AgronicaCoreEntityFramework_POCO.Indirizzi
    '            Dim indirizzoCAA = dal.CreateObject(Of Indirizzi)()
    '            indirizzoCAA.CAP = source.iscrizioneCAA.capCAA
    '            indirizzoCAA.com_cod_istat = source.iscrizioneCAA.codComuneCAA
    '            indirizzoCAA.com_des = source.iscrizioneCAA.comuneCAADescr
    '            indirizzoCAA.Data_Creazione = DateTime.Now
    '            indirizzoCAA.Data_Modifica = DateTime.Now
    '            indirizzoCAA.ind_des = source.iscrizioneCAA.indirizzoCAA
    '            indirizzoCAA.pro_cod = source.iscrizioneCAA.provinciaCAASigla
    '            indirizzoCAA.pro_cod_istat = source.iscrizioneCAA.codProvinciaCAA
    '            indirizzoCAA.cod_indirizzo = codIndirizzo
    '            indirizzoCAA.stato = "IT"
    '            indirizzoCAA.note = ""
    '            indirizzoCAA.inviato = 0
    '            indirizzoCAA.Validazione = 0
    '            indirizzoCAA.Data_Validazione = Now.Date
    '            indirizzoCAA.UserName_Validazione = ""
    '            indirizzoCAA.Codice_Lingua = ""
    '            indirizzoCAA.Codice_Alternativo = ""
    '            indirizzoCAA.Username_Creazione = pivaSuperUser
    '            indirizzoCAA.Username_Modifica = pivaSuperUser
    '            indirizzoCAA.Data_Creazione = DateTime.Now
    '            indirizzoCAA.Data_Modifica = DateTime.Now
    '            indirizzoCAA.Validita_Inizio = AGRODATAINIZIO
    '            indirizzoCAA.Validita_Fine = AGRODATAFINE
    '            ca.Validita_Inizio = strToDatetime(source.iscrizioneCAA.dataInizio)
    '            ca.Data_Modifica = strToDatetime(source.iscrizioneCAA.dataVariazione)
    '            ca.Data_Creazione = DateTime.Now
    '            ca.Indirizzi = indirizzoCAA
    '        Else
    '            ca = (From c As CAA In dal.CAAs Where c.idCAA = source.iscrizioneCAA.idCAA).FirstOrDefault
    '        End If

    '        'dal.IscrizioneCAAs.DeleteObject(From iscr As IscrizioneCAA In dal.IscrizioneCAAs Where iscr.PIVA = piva)
    '        For Each obj In (From iscr As IscrizioneCAA In dal.IscrizioneCAAs Where iscr.PIVA = piva)
    '            dal.IscrizioneCAAs.DeleteObject(obj)
    '        Next

    '        dal.SaveChanges()

    '        If (From iscr As IscrizioneCAA In dal.IscrizioneCAAs Where iscr.PIVA = piva And iscr.CAA.idCAA = ca.idCAA).Count = 0 Then
    '            Dim iscrizione = New IscrizioneCAA '.CreateIscrizioneCAA(source.iscrizioneCAA.idCAA, piva, source.iscrizioneCAA.idAzienda)
    '            iscrizione.idCAA = source.iscrizioneCAA.idCAA
    '            iscrizione.CAA = ca
    '            iscrizione.dataInizio = strToDatetime(source.iscrizioneCAA.dataInizio)
    '            iscrizione.dataRichiesta = strToDatetime(source.iscrizioneCAA.dataRichiesta)
    '            iscrizione.dataVariazione = strToDatetime(source.iscrizioneCAA.dataVariazione)
    '            iscrizione.dtFonte = strToDatetime(source.iscrizioneCAA.dtFonte)
    '            iscrizione.fonte = source.iscrizioneCAA.fonte
    '            iscrizione.utenteVariazione = source.iscrizioneCAA.utenteVariazione
    '            iscrizione.Username_Creazione = pivaSuperUser
    '            iscrizione.Username_Modifica = pivaSuperUser
    '            iscrizione.CAA = ca
    '            imp.IscrizioneCAAs.Add(iscrizione)
    '            'dal.AddObject("IscrizioneCAA", iscrizione)
    '            dal.SaveChanges()
    '        Else
    '            Dim IscrizioneCAA = (From iscr As IscrizioneCAA In dal.IscrizioneCAAs Where iscr.PIVA = piva And iscr.CAA.idCAA = ca.idCAA).FirstOrDefault
    '            IscrizioneCAA.dataInizio = strToDatetime(source.iscrizioneCAA.dataInizio)
    '            IscrizioneCAA.dataRichiesta = strToDatetime(source.iscrizioneCAA.dataRichiesta)
    '            IscrizioneCAA.dataVariazione = strToDatetime(source.iscrizioneCAA.dataVariazione)
    '            IscrizioneCAA.dtFonte = strToDatetime(source.iscrizioneCAA.dtFonte)
    '            IscrizioneCAA.fonte = source.iscrizioneCAA.fonte
    '            IscrizioneCAA.utenteVariazione = source.iscrizioneCAA.utenteVariazione
    '            IscrizioneCAA.Username_Creazione = pivaSuperUser
    '            IscrizioneCAA.Username_Modifica = pivaSuperUser
    '            IscrizioneCAA.MarkAsModified()
    '            dal.SaveChanges()
    '        End If

    '        '''''''''''''''''''''''''''
    '        '''''''''''''''''''''''''''
    '        '''''''''''''''''''''''''''
    '        Dim centroF As Centri_Aziendali
    '        If (From ceaz As Centri_Aziendali In dal.Centri_Aziendali Where ceaz.PIVA = piva And ceaz.sa_nome.Contains("CentroAziendaleFittizio0")).Count = 0 Then
    '            centroF = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateCentri_AziendaliEF(dal, objParametri, imp, "CentroAziendaleFittizio0", pivaSuperUser)
    '            Dim indirizzoCentro = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateIndirizzoCentro(dal, objParametri, centroF, 0, pivaSuperUser)
    '        Else
    '            centroF = (From ceaz As Centri_Aziendali In dal.Centri_Aziendali Where ceaz.PIVA = piva And ceaz.sa_nome = "CentroAziendaleFittizio0").FirstOrDefault
    '        End If
    '        Dim sa_cod = centroF.sa_cod
    '        dal.SaveChanges()
    '        '''''''''''''''''''''''''''
    '        '''''''''''''''''''''''''''
    '        '''''''''''''''''''''''''''
    '        For Each obj In (From d In dal.Mov_Destinazioni Where d.Piva = piva)
    '            dal.Mov_Destinazioni.DeleteObject(obj)
    '        Next
    '        For Each obj In (From md In dal.Movimenti_dettagli Where md.PIVA = piva)
    '            dal.Movimenti_dettagli.DeleteObject(obj)
    '        Next
    '        For Each obj In (From m In dal.Movimentis Where m.PIVA = piva)
    '            dal.Movimentis.DeleteObject(obj)
    '        Next
    '        For Each obj In (From a In dal.Agenda Where a.PIVA = piva)
    '            dal.Agenda.DeleteObject(obj)
    '        Next

    '        dal.SaveChanges()

    '        If source.allevamenti IsNot Nothing AndAlso _
    '                        source.allevamenti.allevamento IsNot Nothing Then
    '            For Each all In source.allevamenti.allevamento
    '                Dim fabbricato As Fabbricati
    '                Dim fabbricato_des = CStr(all.codAllevamento) + " - " + CStr(all.allevamentoDescr) + " (" + CStr(all.idAllevamento) + ")"
    '                If (From f In dal.Fabbricatis Where f.PIVA = piva And f.Fabbricato_Des = fabbricato_des).Count = 0 Then
    '                    Dim fabbricato_Cod As Integer = idGen.NuovoId_SeqMagazzino(piva, sa_cod, 80740352, 80871423, objParametri)
    '                    fabbricato = AgronicaCoreAnagrafeDAL.EFFabbricati.CreateFabbricato(dal, objParametri, centroF, pivaSuperUser)
    '                Else
    '                    fabbricato = (From f In dal.Fabbricatis Where f.PIVA = piva And f.Fabbricato_Des = fabbricato_des).FirstOrDefault
    '                End If
    '                fabbricato.Fabbricato_Des = fabbricato_des
    '                fabbricato.Tipo_Fabbricato_Cod = 70
    '                fabbricato.Data_Richiesta_Autorizzazione = AGRODATAINIZIO
    '                fabbricato.Tipologia_Utilizzo = 0
    '                fabbricato.Regolamento_Cod = 1
    '                fabbricato.MarkAsModified()
    '                dal.SaveChanges()


    '                Dim sta As Stalla
    '                If (From s In dal.Stallas Where s.PIVA = piva And s.sa_cod = sa_cod And s.STA_NUM = fabbricato.Fabbricato_Cod).Count = 0 Then
    '                    sta = AgronicaCoreAnagrafeDAL.EFStalla.CreateStalla(dal, objParametri, fabbricato, pivaSuperUser)
    '                Else
    '                    sta = (From s In dal.Stallas Where s.PIVA = piva And s.sa_cod = sa_cod And s.STA_NUM = fabbricato.Fabbricato_Cod).FirstOrDefault
    '                End If
    '                sta.speCodice = all.speCodice
    '                sta.MarkAsModified()
    '                dal.SaveChanges()

    '                If all.consistenze IsNot Nothing AndAlso _
    '                    all.consistenze.consistenza IsNot Nothing Then
    '                    For Each cons In all.consistenze.consistenza
    '                        'Dim zooAnimali As New Zoo_Animali
    '                        Dim cod_progetto As Integer = 0
    '                        'zooAnimali.PIVA = piva
    '                        'zooAnimali.sa_cod = sa_cod
    '                        'zooAnimali.Cod_Progetto = cod_progetto
    '                        'zooAnimali.codZootecnica = cons.codZootecnica
    '                        'zooAnimali.descrZootecnica = cons.descrZootecnica
    '                        'dal.Zoo_Animali.AddObject(zooAnimali)
    '                        'dal.SaveChanges()
    '                        Dim mov_Descr = cons.codZootecnica + " - " + cons.descrZootecnica
    '                        Dim dataOperazione As Date = New Date(cons.annoRif, 1, 1)
    '                        If (From m In dal.Movimentis Where m.PIVA = piva And m.Mov_Desc = mov_Descr And m.Data_Movimento = dataOperazione And m.Sa_Cod = sa_cod).Count = 0 Then
    '                            Dim agendaOp As Agendum
    '                            agendaOp = EFAgenda.CreateAgenda(dal, objParametri, piva, sa_cod, 3001, mov_Descr, pivaSuperUser)
    '                            Dim mov = EFAgenda.CreateMovimenti(dal, agendaOp, objParametri, 0, 7300, mov_Descr, pivaSuperUser)
    '                            Dim movDett = EFMovimenti.CreateMovimentiDettagli(dal, objParametri, mov, 300, 0, 0, "", 38, strToIntegerNotNothing(cons.numCapi), pivaSuperUser)
    '                            mov.Data_Movimento = dataOperazione
    '                            movDett.Cod_Progetto = cod_progetto
    '                            movDett.Pendente = 5
    '                            movDett.MarkAsModified()
    '                            dal.SaveChanges()
    '                            Dim dest = EFMovimenti_Dettagli.CreateMov_Destinazioni(dal, objParametri, movDett, 0, sta.STA_NUM, 15, strToIntegerNotNothing(cons.numCapi), pivaSuperUser)
    '                        Else
    '                            Dim agenda1 = (From a In dal.Agenda
    '                                            Join m In dal.Movimentis On a.Id_Agenda Equals m.Id_Agenda
    '                                           Where m.PIVA = piva And m.Mov_Desc = mov_Descr And m.Data_Movimento = dataOperazione And m.Sa_Cod = sa_cod
    '                                           Select a).FirstOrDefault
    '                            Dim mov1 = (From m In dal.Movimentis Where m.Id_Agenda = agenda1.Id_Agenda).FirstOrDefault
    '                            Dim movDett1 = (From md In dal.Movimenti_dettagli Where md.Id_Agenda = agenda1.Id_Agenda And md.Id_Mov = mov1.Id_Mov).FirstOrDefault
    '                            mov1.Data_Movimento = dataOperazione
    '                            movDett1.Cod_Progetto = cod_progetto
    '                            movDett1.Pendente = 5
    '                            movDett1.MarkAsModified()
    '                            dal.SaveChanges()
    '                            Dim dest1 = (From md In dal.Mov_Destinazioni Where md.Id_Agenda = agenda1.Id_Agenda And md.Id_Mov = mov1.Id_Mov And md.Id_Mov_Det = movDett1.Id_Mov_Det)
    '                        End If
    '                    Next
    '                End If
    '            Next
    '        End If

    '        For Each obj In (From ip In dal.ImpreseXParticelles Where ip.PIVA = piva)
    '            dal.ImpreseXParticelles.DeleteObject(obj)
    '        Next

    '        For Each obj In (From ic In dal.Imprese_Contratti Where ic.Piva = piva)
    '            dal.Imprese_Contratti.DeleteObject(obj)
    '        Next

    '        If source.conduzioniTerreni IsNot Nothing AndAlso _
    '            source.conduzioniTerreni.particelle IsNot Nothing Then
    '            For Each particella In source.conduzioniTerreni.particelle
    '                Dim sezione As String = "0"
    '                If particella.sezione IsNot Nothing AndAlso particella.sezione <> "" Then
    '                    sezione = particella.sezione
    '                End If
    '                Dim subalterno As String = "0"
    '                If particella.subalterno IsNot Nothing AndAlso particella.subalterno <> "" And particella.subalterno <> "000" Then
    '                    subalterno = particella.sezione
    '                End If
    '                Dim part As ParticelleCatastali
    '                Dim particellaProv = particella.codProv
    '                Dim particellaCom = particella.codCom
    '                Dim particellaFoglio = particella.foglio
    '                Dim particellaNumero = particella.particella
    '                If (From ip In dal.ParticelleCatastalis
    '                    Where ip.PROV = particellaProv And ip.COM = particellaCom And ip.SEZIONE = sezione And ip.FOGLIO = particellaFoglio And ip.NUMERO = particellaNumero And ip.SUBALTERNO = subalterno).Count = 0 Then
    '                    part = AgronicaCoreAnagrafeDAL.EFParticelle.CreateParticelleCatastaliEF(dal, objParametri, particella.codProv, particella.codCom, sezione, particella.foglio, particella.particella, subalterno, pivaSuperUser)
    '                Else
    '                    part = (From ip In dal.ParticelleCatastalis
    '                    Where ip.PROV = particellaProv And ip.COM = particellaCom And ip.SEZIONE = sezione And ip.FOGLIO = particellaFoglio And ip.NUMERO = particellaNumero And ip.SUBALTERNO = subalterno).FirstOrDefault
    '                End If

    '                Dim impxPart As ImpreseXParticelle
    '                If (From ip In dal.ImpreseXParticelles
    '                Where ip.PIVA = piva And ip.PROV = particella.codProv And ip.COM = particella.codCom And ip.SEZIONE = sezione And ip.FOGLIO = particella.foglio And ip.NUMERO = particella.particella And ip.SUBALTERNO = subalterno).Count = 0 Then
    '                    impxPart = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateCentriAziendaliXParticelle(dal, objParametri, centroF, part, pivaSuperUser)
    '                Else
    '                    impxPart = (From ip In dal.ImpreseXParticelles
    '                                Where ip.PIVA = piva And ip.PROV = particella.codProv And ip.COM = particella.codCom And ip.SEZIONE = sezione And ip.FOGLIO = particella.foglio And ip.NUMERO = particella.particella And ip.SUBALTERNO = subalterno).FirstOrDefault
    '                End If

    '                'DATI PARTICELLA
    '                part.casiParticolari = particella.casiParticolari
    '                part.CLASSE = strToCodiceClasse(particella.codiceClasse)
    '                part.QUALITA_COD = strToInteger(particella.codiceQualita)
    '                'particella.codReg
    '                If particella.dataFine IsNot Nothing AndAlso particella.dataFine <> "" Then
    '                    part.Validita_Fine = strToDatetime(particella.dataFine)
    '                Else
    '                    part.Validita_Fine = AGRODATAFINE
    '                End If
    '                part.Validita_Inizio = strToDatetime(particella.dataInizio)
    '                part.datainvio = strToDatetime(particella.dataInserimento)
    '                part.Data_Modifica = strToDatetime(particella.dataVariazione)
    '                part.fasciaAltimetrica = particella.fasciaAltimetrica
    '                part.fasciaAltimetricaDescr = part.fasciaAltimetricaDescr
    '                part.Fonte = particella.fonte
    '                part.FonteDescr = particella.fonteDescr
    '                'particella.regDescr
    '                If particella.supCatastale IsNot Nothing AndAlso particella.supCatastale <> "" Then
    '                    Dim ettariAreCentiare = strToDouble(particella.supCatastale) / 1000
    '                    Dim ettari, are, centiare
    '                    AgronicaCoreDataProvider.Conversioni.EttariAreCentiare_from_Ettari(ettariAreCentiare, ettari, are, centiare)
    '                    part.ETTARI = CDbl(ettari)
    '                    part.ARE = CDbl(are)
    '                    part.CENTIARE = CDbl(centiare)
    '                Else
    '                    part.ETTARI = 0
    '                    part.ARE = 0
    '                    part.CENTIARE = 0
    '                End If
    '                part.tipoDocumento = particella.tipoDocumento
    '                part.tipoDocumentoDescr = particella.tipoDocumentoDescr
    '                part.utilizzo = particella.utilizzo

    '                part.MarkAsModified()
    '                dal.SaveChanges()

    '                Dim conduzione = particella.conduzione
    '                If conduzione IsNot Nothing Then
    '                    'DATI CONDUZIONE
    '                    part.fasciaAltimetrica = conduzione.fasciaAltimetrica
    '                    part.fasciaAltimetricaDescr = conduzione.fasciaAltimetricaDescr
    '                    part.MarkAsModified()
    '                    impxPart.MarkAsModified()
    '                    dal.SaveChanges()

    '                    If conduzione.contratti IsNot Nothing Then
    '                        Dim j = 0
    '                        For Each contratto In conduzione.contratti
    '                            'ID_Contratto in ImpreseXParticelle
    '                            If j = 0 Then
    '                                'Inserire ID_Contratto
    '                                'contratto.formaPossesso
    '                                'contratto.formaPossessoDescr
    '                                'contratto.supPossesso
    '                                'contratto.titoloConduzione
    '                                Dim contr As Imprese_Contratti '.CreateImprese_Contratti(piva, contrattoCod, "", pivaSuperUser, pivaSuperUser, 0, 0, 0, 0, AGRODATAINIZIO)
    '                                Dim dataInizio As DateTime = strToDatetime(contratto.dtInizio)
    '                                If (From ixc In dal.ImpreseXParticelles
    '                                    Join ic In dal.Imprese_Contratti On ic.Contratto_Cod Equals ixc.Contratto_Cod _
    '                                                                                Where ixc.PIVA = piva _
    '                                                                                And ixc.PROV = particella.codProv _
    '                                                                                And ixc.COM = particella.codCom _
    '                                                                                And ixc.SEZIONE = sezione _
    '                                                                                And ixc.FOGLIO = particella.foglio _
    '                                                                                And ixc.NUMERO = particella.particella _
    '                                                                                And ixc.SUBALTERNO = subalterno _
    '                                                                                And ic.Data_Stipulazione = dataInizio).Count = 0 Then
    '                                    Dim contrattoCod = idGen.NuovoId_Tabella_EF(dal, "Contratti", 0, 20000000, objParametri)
    '                                    contr = New Imprese_Contratti
    '                                    impxPart.Contratto_Cod = contrattoCod
    '                                    contr.Piva = piva
    '                                    contr.Contratto_Cod = contrattoCod
    '                                    dal.Imprese_Contratti.AddObject(contr)
    '                                Else
    '                                    contr = (From ixc In dal.ImpreseXParticelles
    '                                    Join ic In dal.Imprese_Contratti On ic.Contratto_Cod Equals ixc.Contratto_Cod _
    '                                                                                Where ixc.PIVA = piva _
    '                                                                                And ixc.PROV = particella.codProv _
    '                                                                                And ixc.COM = particella.codCom _
    '                                                                                And ixc.SEZIONE = sezione _
    '                                                                                And ixc.FOGLIO = particella.foglio _
    '                                                                                And ixc.NUMERO = particella.particella _
    '                                                                                And ixc.SUBALTERNO = subalterno _
    '                                                                                And ic.Data_Stipulazione = dataInizio _
    '                                                                                Select ic).FirstOrDefault
    '                                End If
    '                                contr.Piva = piva
    '                                contr.Riferimento = ""
    '                                contr.Username_Creazione = pivaSuperUser
    '                                contr.Username_Modifica = pivaSuperUser
    '                                contr.Cod_Risum = 0
    '                                contr.Descrizione_1 = contratto.formaPossesso
    '                                contr.Descrizione_2 = contratto.formaPossessoDescr
    '                                contr.Riferimento = ""
    '                                contr.Username_Creazione = pivaSuperUser
    '                                contr.Username_Modifica = pivaSuperUser
    '                                contr.Cod_Risum = 0
    '                                contr.Stato = 0
    '                                contr.ChkStato_Automatico = 0
    '                                contr.Cau_Pagamento = 0
    '                                contr.Data_Stipulazione = strToDatetime(contratto.dtInizio)
    '                                contr.Validita_Fine = strToDatetime(contratto.dtFine)
    '                                contr.DataVariazione = strToDatetime(contratto.dataVariazione)
    '                                contr.prog = contratto.progr
    '                                contr.Stato = 0
    '                                contr.ChkStato_Automatico = 0
    '                                contr.Cau_Pagamento = 0
    '                                contr.Data_Stipulazione = strToDatetime(contratto.dtInizio)

    '                                contr.Validita_Fine = strToDatetime(contratto.dtFine)
    '                                contr.DataVariazione = strToDatetime(contratto.dataVariazione)
    '                                contr.prog = contratto.progr
    '                                dal.SaveChanges()
    '                                contr.MarkAsModified()
    '                                'dal.AddObject("contr", contr)
    '                                If contratto.supPossesso IsNot Nothing AndAlso contratto.supPossesso <> "" Then
    '                                    impxPart.Sup_Condotta = (contratto.supPossesso / 1000)
    '                                Else
    '                                    impxPart.Sup_Condotta = 0
    '                                End If
    '                                impxPart.biologico = validaBooleano(conduzione.biologico)
    '                                impxPart.flagAnomaliaMacrouso = validaBooleano(conduzione.flagAnomaliaMacrouso)
    '                                impxPart.flagContenzioso = validaBooleano(conduzione.flagContenzioso)
    '                                impxPart.flagSupero = validaBooleano(conduzione.flagSupero)
    '                                impxPart.Irrigabilita = conduzione.irriguo
    '                                impxPart.RotazioneColturale = conduzione.rotazioneColturale
    '                                impxPart.MarkAsModified()
    '                                dal.SaveChanges()
    '                            Else
    '                                Dim impxPart_Contratto = AgronicaCoreAnagrafeDAL.EFCentri_Aziendali.CreateCentriAziendaliXParticelle(dal, objParametri, centroF, part, pivaSuperUser)

    '                                Dim contrattoCod = idGen.NuovoId_Tabella_EF(dal, "Contratti", 0, 2000000, objParametri)
    '                                impxPart_Contratto.Contratto_Cod = contrattoCod
    '                                Dim contr = New Imprese_Contratti '.CreateImprese_Contratti(piva, contrattoCod, "", pivaSuperUser, pivaSuperUser, 0, 0, 0, 0, AGRODATAINIZIO)
    '                                contr.Piva = piva
    '                                contr.Contratto_Cod = contrattoCod
    '                                contr.Riferimento = ""
    '                                contr.Username_Creazione = pivaSuperUser
    '                                contr.Username_Modifica = pivaSuperUser
    '                                contr.Cod_Risum = 0
    '                                contr.Descrizione_1 = contratto.formaPossesso
    '                                contr.Descrizione_2 = contratto.formaPossessoDescr
    '                                contr.Riferimento = ""
    '                                contr.Username_Creazione = pivaSuperUser
    '                                contr.Username_Modifica = pivaSuperUser
    '                                contr.Cod_Risum = 0
    '                                contr.Stato = 0
    '                                contr.ChkStato_Automatico = 0
    '                                contr.Cau_Pagamento = 0
    '                                contr.Data_Stipulazione = strToDatetime(contratto.dtInizio)
    '                                contr.Validita_Fine = strToDatetime(contratto.dtFine)
    '                                contr.DataVariazione = strToDatetime(contratto.dataVariazione)
    '                                contr.prog = contratto.progr
    '                                contr.Stato = 0
    '                                contr.ChkStato_Automatico = 0
    '                                contr.Cau_Pagamento = 0
    '                                contr.Data_Stipulazione = strToDatetime(contratto.dtInizio)

    '                                contr.Validita_Fine = strToDatetime(contratto.dtFine)
    '                                contr.DataVariazione = strToDatetime(contratto.dataVariazione)
    '                                contr.prog = contratto.progr

    '                                If contratto.supPossesso IsNot Nothing AndAlso contratto.supPossesso <> "" Then
    '                                    impxPart_Contratto.Sup_Condotta = (contratto.supPossesso / 1000)
    '                                Else
    '                                    impxPart_Contratto.Sup_Condotta = 0
    '                                End If

    '                                impxPart_Contratto.biologico = validaBooleano(conduzione.biologico)
    '                                impxPart_Contratto.flagAnomaliaMacrouso = validaBooleano(conduzione.flagAnomaliaMacrouso)
    '                                impxPart_Contratto.flagContenzioso = validaBooleano(conduzione.flagContenzioso)
    '                                impxPart_Contratto.flagSupero = validaBooleano(conduzione.flagSupero)
    '                                impxPart_Contratto.Irrigabilita = conduzione.irriguo
    '                                impxPart_Contratto.RotazioneColturale = conduzione.rotazioneColturale
    '                                impxPart_Contratto.MarkAsModified()
    '                                dal.Imprese_Contratti.AddObject(contr)
    '                                'dal.AddObject("contr", contr)

    '                                dal.SaveChanges()
    '                            End If
    '                            j += 1
    '                        Next

    '                    End If

    '                End If
    '                numParticella += 1
    '                dal.SaveChanges()
    '            Next
    '        End If
    '        dal.AcceptAllChanges()
    '        dal.Connection.Close()
    '    Catch ex As Exception
    '        Dim a = numParticella
    '    End Try
    'End Function

    Public Shared Function validaBooleano(ByVal value As String) As String
        Select Case value
            Case "S"
                Return "1"
            Case "True"
                Return "1"
            Case "N"
                Return "0"
            Case "False"
                Return "0"
            Case "0"
                Return "0"
            Case "1"
                Return "1"
            Case "s"
                Return "1"
            Case "true"
                Return "1"
            Case "n"
                Return "0"
            Case "false"
                Return "0"
            Case Else
                Return value
        End Select
    End Function

    Public Shared Function strToStrObb(ByVal value As String) As String
        Try
            If value IsNot Nothing Then
                Return value
            Else
                Return ""
            End If
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Shared Function strToDatetime(val As String) As DateTime?
        Try
            If val IsNot Nothing Then
                If val <> "" Then
                    Dim data As DateTime = DateTime.Parse(val)
                    If data < AGRODATAINIZIO Then
                        data = AGRODATAINIZIO
                        Return data
                    End If
                    If data > AGRODATAFINE Then
                        data = AGRODATAFINE
                        Return data
                    End If
                    Return data
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Shared Function strToDate(val As String) As Date?
        Try
            If val IsNot Nothing Then
                If val <> "" Then
                    Dim data As DateTime = DateTime.Parse(val)
                    If data < AGRODATAINIZIO Then
                        data = AGRODATAINIZIO
                        Return data
                    End If
                    If data > AGRODATAFINE Then
                        data = AGRODATAFINE
                        Return data
                    End If
                    Return data
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Shared Function strToInteger(val As String) As Integer?
        Try
            If val IsNot Nothing Then
                If val <> "" Then
                    Return CInt(val)
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Shared Function strToIntegerNotNothing(val As String) As Integer
        Try
            If val IsNot Nothing Then
                If val <> "" Then
                    Return CInt(val)
                Else
                    Return 0
                End If
            Else
                Return 0
            End If
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Shared Function strToDouble(val As String) As Double?
        Try
            If val IsNot Nothing Then
                If val <> "" Then
                    Return CDbl(val)
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Shared Function getCodRapportoFromRa_Cod(dal As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities, value As String) As Integer
        Dim CacRappCont = (From r As CAC_Codifica_RapportiContabili In dal.CAC_Codifica_RapportiContabili Where r.Ra_Cod = value).FirstOrDefault
        If CacRappCont IsNot Nothing Then
            Return CacRappCont.Cod_Rapporto
        End If
        Return -1
    End Function

    Private Shared Function strToCodiceClasse(value As String) As String
        Try
            If value IsNot Nothing AndAlso value <> "" Then
                Dim inVal = CInt(value)
                If inVal < 10 Then
                    Return inVal.ToString
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Shared Sub modificaImpresa(dal As Gias_DeveloperServer_Entities, pivaSuperUser As String, source As FascicoloSiar2Response, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

    End Sub

    Private Shared Function gestioneZonaCod(codZona As String, zonaDescr As String, pivaSuperUser As String, dal As Gias_DeveloperServer_Entities) As Integer
        If (From codificaZona In dal.CAC_Codifica_Zone Where codificaZona.Piva_SuperUser = pivaSuperUser And codificaZona.Zona_Cod_Cliente = codZona).Count = 0 Then
            Dim id As Integer
            If (From zone In dal.Zone Where zone.Zona_Cod > 0).Count = 0 Then
                id = 1
            Else
                id = (From zone In dal.Zone Where zone.Zona_Cod > 0 Order By zone.Zona_Cod Descending).FirstOrDefault.Zona_Cod + 1
            End If
            Dim zona As New Zone
            zona.Piva_SuperUser = pivaSuperUser
            zona.Zona_Cod = id
            zona.Descrizione = zonaDescr
            zona.Tessitura_cod = 0
            zona.Altimetria = "p"
            zona.SO = 0
            zona.inviato = 0
            zona.datainvio = AGRODATAINIZIO
            zona.Data_Creazione = DateTime.Now
            zona.Data_Modifica = DateTime.Now
            zona.Username_Creazione = pivaSuperUser
            zona.Username_Modifica = pivaSuperUser
            zona.Validita_Inizio = AGRODATAINIZIO
            zona.Validita_Fine = AGRODATAFINE
            zona.TipoZona = "n"
            dal.Zone.Add(zona)

            Dim codificaZone As New AgronicaCoreEntityFramework_POCO.CAC_Codifica_Zone
            codificaZone.Piva_SuperUser = pivaSuperUser
            codificaZone.Zona_Cod_Cliente = codZona
            codificaZone.Zona_Cod_Gias = id
            codificaZone.Descrizione = zonaDescr
            codificaZone.Data_Modifica = DateTime.Now
            dal.CAC_Codifica_Zone.Add(codificaZone)
            dal.SaveChanges()
            Return id
        Else
            Dim zona As AgronicaCoreEntityFramework_POCO.CAC_Codifica_Zone = (From codificaZona In dal.CAC_Codifica_Zone Where codificaZona.Piva_SuperUser = pivaSuperUser And codificaZona.Zona_Cod_Cliente = codZona).FirstOrDefault
            Return zona.Zona_Cod_Gias
        End If
    End Function

    Private Shared Sub SpecieDaCodificaCliente(dal As Gias_DeveloperServer_Entities, ByRef codificaCliente As String, ByRef gen_cod As Integer, ByRef spe_cod As Integer, ByRef ipro_cod As Integer, ByRef cat_cod As Integer, ByRef raz_cod As Integer)
        Dim codCliente As String = codificaCliente
        If (From cac In dal.CAC_Codifica_Animali Where cac.Cod_Cliente = codCliente).Count = 0 Then
            gen_cod = 0
            spe_cod = 0
            ipro_cod = 0
            cat_cod = 0
            raz_cod = 0
        Else
            Dim res = (From cac In dal.CAC_Codifica_Animali Where cac.Cod_Cliente = codCliente).FirstOrDefault
            gen_cod = res.Gen_Cod_Gias
            spe_cod = res.Spe_Cod_Gias
            ipro_cod = res.Ipro_Cod_Gias
            cat_cod = res.Cat_Cod_Gias
            raz_cod = res.Raz_Cod_Gias
        End If

    End Sub

End Class
