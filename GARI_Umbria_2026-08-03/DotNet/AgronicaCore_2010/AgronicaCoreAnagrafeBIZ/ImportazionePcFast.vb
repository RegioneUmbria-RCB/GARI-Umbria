Imports System.Linq
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class ImportazionePcFast_R

    Public Function AziendaVerificaEsiste(ByVal piva As String, ByRef objParametri_Server As AgronicaCoreParametri) As RispostaStandard


        Dim rval As New RispostaStandard



        Try

            Dim xLeggiContatti As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            Dim dtAzienda As DataTable =
                xLeggiContatti.Leggi(piva, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)



            If (dtAzienda.Rows.Count > 0) Then
                rval.RispostaOK = True
                rval.RispostaStringa = "Anagrafica azienda compilata"
            Else
                rval.RispostaOK = False
                rval.RispostaStringa = ""
            End If


        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            Dim messaggioErrore As String = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            rval.RispostaOK = False
            rval.Errore = messaggioErrore

        End Try

        Return rval

    End Function


    Public Function ContattiVerificaEsistePatentino(ByVal piva As String, ByRef objParametri_Server As AgronicaCoreParametri) As RispostaStandard


        Dim rval As New RispostaStandard


        Try

            Dim xLeggiContatti As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
            Dim xLeggiPatentino As New AgronicaCoreAnagrafeDAL.Contatti_R

            Dim dtContatti As DataTable = xLeggiContatti.Leggi(piva, "", 0, 0, 0, "", True, False, "", "", objParametri_Server)

            Dim nContatti As Integer = 0

            Dim listContatti As New List(Of String)

            For Each row In dtContatti.Rows
                Dim patentino As String = ""
                Dim data_rilascio As Date
                Dim data_scadenza As Date
                Dim ente As String = ""


                xLeggiPatentino.Ottieni_Dati_Patentino(row("piva"), row("cod_Contatto"), patentino, data_rilascio, data_scadenza, ente, DateTime.Now, objParametri_Server)

                If patentino <> "" AndAlso Not listContatti.Contains(row("Cod_Contatto")) Then
                    nContatti += 1
                    listContatti.Add(CStr(row("cod_Contatto")))
                End If

            Next

            rval.RispostaOK = (nContatti > 0)
            rval.RispostaStringa = nContatti & " contatti trovati con indicato il dato del patentino."

        Catch ex As Exception


            'uso questa funzione per ottenere il Messaggio..:
            Dim messaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            rval.RispostaOK = False
            rval.Errore = messaggioErrore

        End Try

        Return rval

    End Function



    Public Function PianoColturaleVerificaEsiste(ByVal piva As String, ByRef objParametri_Server As AgronicaCoreParametri) As RispostaStandard


        Dim rval As New RispostaStandard


        Try

            Dim xLeggiContatti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

            Dim objPratiche As New AgronicaCoreProfilazioneDAL.Pratiche_R
            If objPratiche.Leggi_conStatoAttuale(0, "", piva, "", 0, 0, 0, 1017, 1002, Now, Now, "", "", objParametri_Server, 0, 0).Rows.Count = 0 Then

                Dim dtImpianti As DataTable = xLeggiContatti.Leggi(piva, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                Dim sommaSuperficie As Double = dtImpianti.AsEnumerable().Sum(Function(x) x.Field(Of Double)("sup_imp"))

                rval.RispostaOK = (dtImpianti.Rows.Count > 0)
                'rval.RispostaStringa = dtImpianti.Rows.Count & " impianti confermati (superficie totale: " & sommaSuperficie.ToString & " Ha)."
                rval.RispostaStringa = "Ci sono impianti confermati."

            Else

                Dim Finetra_Temporale_Inizio_BK = objParametri_Server.FinestraTemporaleInizio
                Dim Finetra_Temporale_Fine_BK = objParametri_Server.FinestraTemporaleFine

                objParametri_Server.FinestraTemporaleInizio = Date.Now
                objParametri_Server.FinestraTemporaleFine = Date.Now

                Dim dtImpianti As DataTable = xLeggiContatti.Leggi(piva, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                Dim sommaSuperficie As Double = dtImpianti.AsEnumerable().Sum(Function(x) x.Field(Of Double)("sup_imp"))

                objParametri_Server.FinestraTemporaleInizio = Finetra_Temporale_Inizio_BK
                objParametri_Server.FinestraTemporaleFine = Finetra_Temporale_Fine_BK

                rval.RispostaOK = (dtImpianti.Rows.Count > 0)
                'rval.RispostaStringa = dtImpianti.Rows.Count & " impianti confermati (superficie totale: " & sommaSuperficie.ToString & " Ha)."
                If dtImpianti.Rows.Count > 0 Then
                    rval.RispostaStringa = "Ci sono impianti colturali grafici"
                Else
                    rval.RispostaStringa = ""
                End If

            End If



        Catch ex As Exception
            Dim messaggioErrore As String = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            rval.RispostaOK = False
            rval.Errore = messaggioErrore

        End Try

        Return rval

    End Function

    Public Function MacchineVerificaEsiste(ByVal piva As String, ByRef objParametri_Server As AgronicaCoreParametri) As RispostaStandard


        Dim rval As New RispostaStandard


        Try

            Dim xLeggiContatti As New AgronicaCoreContabDAL.Parco_Macchine_R

            Dim dtMacchine As DataTable = xLeggiContatti.Leggi(
                piva, 0, False, "", "", "", "", "", 0, "", False, 0, "", True, AGRODATAINIZIO, AGRODATAFINE, "  substring(Macchine.CLASS_Code, 1, 5) = '06.02' and parco_macchine.Ultima_Manutenzione <> " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDate(AGRODATAINIZIO) & " ", "", objParametri_Server)

            rval.RispostaOK = (dtMacchine.Rows.Count > 0)
            rval.RispostaStringa = dtMacchine.Rows.Count & " macchine trovate di tipo ""Macchine per la difesa chimica (06.02)"" con indicata una data di ultima taratura/manutenzione."

        Catch ex As Exception
            Dim messaggioErrore As String = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            rval.RispostaOK = False
            rval.Errore = messaggioErrore

        End Try

        Return rval

    End Function


    Public Function PreferenzeImpostazioniVerifica(ByVal piva As String, ByRef objParametri_Server As AgronicaCoreParametri) As RispostaStandard


        Dim rval As New RispostaStandard


        Try

            Dim r As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read

            Dim Val As String = r.Leggi_Codice_from_Imprese_Codici(piva, enum_CodiciAnagrafe.Disciplinare_Aziendale_Default, objParametri_Server)

            rval.RispostaOK = (Not String.IsNullOrEmpty(Val))
            Dim non As String = ""
            If Not rval.RispostaOK Then
                non = "non"
            End If
            rval.RispostaStringa = " Preferenze ed impostazioni predefinite " & non & " compilate."

        Catch ex As Exception
            Dim messaggioErrore As String = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            rval.RispostaOK = False
            rval.Errore = messaggioErrore

        End Try

        Return rval

    End Function
End Class

Public Class ImportazionePcFast

    Public Function Aggiorna_PcFast_Aziende(ByVal righeModificate As String,
                                            ByVal progressivoGias As Integer,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri
                                            ) As RispostaStandard

        Dim rval As New RispostaStandard

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try


            Dim letturaAzienda As New AgronicaCoreAnagrafeDAL.Imprese_Read

            Dim righeModificateArray As JArray = JArray.Parse(righeModificate)


            Dim piva As String = righeModificateArray(0)("piva")

            Dim Esisteimpresa As Boolean = letturaAzienda.Esiste_Impresa(piva, 0, AGRODATAINIZIO, "", "", "", objParametri_Server)
            Dim TipoOperazione As enum_TipoOperazioneDB



            Dim objXML As New AgronicaCoreXML.AnagrafeXML

            Dim Piva_Padre As String = objParametri_Server.PivaSuperUser
            If righeModificateArray(0)("Piva_Padre") IsNot Nothing AndAlso righeModificateArray(0)("Piva_Padre") <> "" Then
                Piva_Padre = righeModificateArray(0)("Piva_Padre")
            End If


            Dim Cuaa As String = righeModificateArray(0)("cuaa")
            Dim RagSoc As String = righeModificateArray(0)("Rag_Soc")
            Dim Indirizzo As String = righeModificateArray(0)("Indirizzo")
            Dim Cod_Indirizzo As Integer = -1
            Dim Cap As String = righeModificateArray(0)("Cap")
            Dim Comune As String = righeModificateArray(0)("Comune_Des")

            Dim Istat_Comune As String = righeModificateArray(0)("Istat_Com")
            Dim Istat_Provincia As String = righeModificateArray(0)("Istat_Prov")
            Dim Frazione As String = righeModificateArray(0)("Frazione")
            Dim stato As String = righeModificateArray(0)("Indirizzo_Stato")
            Dim CreaCentro As Boolean = False

            Dim objProv As New AgronicaCoreMetaSchemaDAL.Lista_Province_R
            Dim Sigla_Provincia As String = ""
            If objProv.Leggi("", "",
                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                      " PROV = '" & Istat_Provincia & "' ",
                                      "",
                                      objParametri_Server).Rows.Count > 0 Then

                Sigla_Provincia = objProv.Leggi("", "",
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                " PROV = '" & Istat_Provincia & "' ",
                                                "",
                                                objParametri_Server).Rows(0)("SIGLA")
            End If


            If stato <> "" AndAlso stato <> "IT" AndAlso stato <> "ITALIA" Then
                Cap = "00000"
                Istat_Comune = "000"
                Istat_Provincia = "000"
                Comune = ""
                Sigla_Provincia = ""
            End If

            'Spostata gestione..
            'Dim Legale_Rappresentante_Cognome As String = righeModificateArray(0)("Legale_Rappresentante_Cognome")
            'Dim Legale_Rappresentante_Nome As String = righeModificateArray(0)("Legale_Rappresentante_Nome")
            'Dim Legale_Rappresentante_CF As String = righeModificateArray(0)("Legale_Rappresentante_CF")
            'Dim Legale_Rappresentante_Sesso As String = righeModificateArray(0)("Legale_Rappresentante_Sesso")
            'Dim Legale_Rappresentante_Data_Nascita As Date = Date.Parse(righeModificateArray(0)("Legale_Rappresentante_Data_Nascita"))

            If Not Esisteimpresa Then
                TipoOperazione = enum_TipoOperazioneDB.Scrittura

                'devo anche eseguire avanzamento di stato, quindi gestisco da qui la transazione, altrimenti no.
                '------------------------------
                'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

                CreaCentro = True

            Else
                TipoOperazione = enum_TipoOperazioneDB.Modifica

                Dim xLeggiCodIndir As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
                Dim xLeggiGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R

                Dim dtLeggiCodIndir As DataTable =
                    xLeggiCodIndir.Leggi2(piva, True, 0, 1, "", "", objParametri_Server)

                If dtLeggiCodIndir.Rows.Count > 0 Then
                    Cod_Indirizzo = dtLeggiCodIndir.Rows(0)("Cod_Indirizzo")
                End If

                'Piva_Padre = xLeggiGerarchia.LeggiPadre(piva, objParametri_Server, "")


            End If

            Dim xmlDoc As New Xml.XmlDocument

            Dim XmlImpresa As System.Xml.XmlElement
            Dim XmlCentro As System.Xml.XmlElement
            Dim XmlFabbricato As System.Xml.XmlElement

            Dim logErrori As String = ""

            Dim baseCode As Integer
            Dim topCode As Integer

            UtilityProvider.Calcola_BaseCode_TopCode(baseCode, topCode, progressivoGias)

            Dim XmlUtente As System.Xml.XmlElement

            XmlUtente = objXML.Xml_Pubblico_Utente(
                xmlDoc,
                objParametri_Server.UtenteUsername,
                "#",
                progressivoGias
            )

            XmlImpresa = objXML.Xml_Pubblico_Impresa(
                TipoOperazione,
                piva,
                RagSoc,
                Cuaa,
                Cuaa,
                "#",
                "#",
                Piva_Padre,
                "1",
                "1",
                "#", "#",
                Indirizzo,
                Frazione,
                Cap,
                Comune,
                Sigla_Provincia,
                stato, "#",
                Istat_Comune,
                Istat_Provincia,
                "#",
                "#",
                "#",
                "#",
                "#",
                "#", "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#", "#", "#", "#", "#", "#",
                "#",
                xmlDoc)

            If CreaCentro Then

                Dim SaCod As Integer = 0
                Dim SaNome As String = "01"

                XmlCentro = objXML.Xml_Pubblico_CentroAziendale(enum_TipoOperazioneDB.Scrittura,
                                                            SaCod,
                                                            SaNome,
                                                            1,
                                                            "#", "#", "#", "#", "#", "#",
                                                            Indirizzo,
                                                            "#",
                                                            Cap,
                                                            Comune,
                                                            Sigla_Provincia,
                                                            "#", "#",
                                                            Istat_Comune,
                                                            Istat_Provincia,
                                                            "#", "#", "#", "#", "#", "#", "#", "#", "#",
                                                             "#",
                                                            xmlDoc)

                'Creo il fabbricato
                XmlFabbricato = objXML.Xml_Pubblico_Fabbricato(enum_TipoOperazioneDB.Scrittura,
                                                              "0",
                                                              "Magazzino n.01",
                                                              20,
                                                              "#", "#", "#",
                                                              "#",
                                                              "#",
                                                              "#",
                                                              AGRODATAINIZIO,
                                                              AGRODATAFINE,
                                                              Indirizzo,
                                                              "#",
                                                              Cap,
                                                              Comune,
                                                              Sigla_Provincia,
                                                              "#", "#",
                                                              Istat_Comune,
                                                              Istat_Provincia,
                                                               "#",
                                                              xmlDoc)

                XmlCentro.AppendChild(XmlFabbricato)


                XmlImpresa.AppendChild(XmlCentro)


            End If


            XmlUtente.AppendChild(XmlImpresa)

            xmlDoc.AppendChild(XmlUtente)

            Dim objAgronica_XML As New AgronicaCoreXML.Agronica_XML

            Dim strErrPP As String = ""
            Dim HT_Piva_objCodici As New Hashtable

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim codiceChiaveCliente As Integer

            codiceChiaveCliente = objConfSiti.Leggi_Valore(16, "Sincro_Codice_Chiave_Cliente", "", "", objParametri_Server)

            Dim Str_XML_Privato As String = objAgronica_XML.XMLPrivato_from_XMLPubblico(
                objParametri_Server,
                HT_Piva_objCodici,
                xmlDoc.OuterXml,
                codiceChiaveCliente,
                strErrPP)

            If strErrPP <> "" Then
                Throw New Exception("Errore in fase di generazione xml privato:" & strErrPP)
            End If

            Dim scriviImpresa As New Importa_GIAS

            If Esisteimpresa Then
                Str_XML_Privato = Str_XML_Privato.Replace("TipoOperazioneDB=""1""", "TipoOperazioneDB=""2""")
            End If

            scriviImpresa.Scrivi_Anagrafe(Str_XML_Privato, piva, 0, Id_Servizio_GiasOnline, 0, objParametri_Server, objParametri_Utenti)


            Dim objGerarchiaImprese_R As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            Dim objGerarchiaImprese_W As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_W
            Dim DTPadri = objGerarchiaImprese_R.LeggiPadriGerarchia("", piva, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)
            If DTPadri.Rows.Count > 1 Then
                For Each Row_Padre In DTPadri.Rows
                    If Row_Padre("Padre") <> Piva_Padre Then
                        objGerarchiaImprese_W.Cancella(Row_Padre("Padre"), piva, "", objParametri_Server, objParametri_Utenti)
                    End If
                Next
            End If

            rval.RispostaOK = True
            rval.RispostaStringa = "Salvataggio eseguito con successo."

            'commit solo se ho aperto la transazione
            If Not Esisteimpresa Then

                Dim xAvanzamentoStato As New AgronicaCoreProfilazioneBIZ.Pratiche_W

                Dim xRispAvanza As RispostaStandard =
                    xAvanzamentoStato.impostaPratica(
                        enum_WWorflow.Attivazione_Aziende_Agrarie_in_GIAS,
                        piva, Cuaa,
                        objParametri_Server.UtenteUsername,
                        enum_Servizi.Workflow_di_attivazione_aziende_GIAS,
                        enum_WWorflow_WAnagraficaStati.Attivazione_Aziende_Agrarie_in_GIAS_Anagrafica_Impresa_Memorizzata,
                        objParametri_Server,
                        objParametri_Utenti, 0, "", 0, False, "", AGRODATAINIZIO, AGRODATAFINE, 0, 0)

                If Not xRispAvanza.RispostaOK Then
                    Throw New Exception("Avanzamento di stato non riuscito:" & xRispAvanza.Errore)
                End If

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            End If

        Catch ex As Exception

            'rollback della transazione
            If objParametri_Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            'uso questa funzione per ottenere il Messaggio..:
            Dim messaggioErrore As String = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            rval.RispostaOK = False
            rval.Errore = messaggioErrore
        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return rval

    End Function

    Public Function Aggiorna_PcFast_PianoColtuale(
                            ByVal piva As String,
                            ByVal cuaa As String,
                            ByVal Programmazione_Cod As Integer,
                            ByVal DataInizio As Date, ByVal DataFine As Date,
                            ByVal numeroValidazione As String,
                            ByVal dataValidazione As DateTime,
                            ByVal pianificazioneFonte As String,
                            ByVal progressivoGias As Integer,
                            ByVal allegatoDocumentocod As Integer,
                            ByVal bDividiCentri As Boolean,
                            ByVal Ribalta As Boolean,
                            ByRef objParametri_Server As AgronicaCoreParametri,
                            ByRef objParametri_Utenti As AgronicaCoreParametri,
                            ByRef pratica_cod As String,
                            Optional ByVal str_Appezzamenti As String = Nothing,
                            Optional ByRef Sito_Cod As Enum_SiteRedirector = Enum_SiteRedirector.Sito_AgronicaSincronizzatore,
                            Optional ByRef Pagina_Cod As Integer = enum_PagineAgronicaSincro.ImportazionePC_Anteprima,
                            Optional ByRef Nome_Div As String = "kendo_Appezzamenti",
                            Optional ByVal importatoAutomaticamente As Boolean = False
                        ) As RispostaStandard

        Dim rval As New RispostaStandard

        'Dim strErr As String = ""

        Try
            Dim NomeRoutine = "Aggiorna_PcFast_PianoColtuale"

            Dim scritturaPianoColturale As New AgronicaCoreAnagrafeBIZ.Programmazione_W
            Dim PrimoPlanning As Boolean = True
            Dim eliminaAppezzamenti As Boolean = False
            Dim TipoOperazioneDB As Integer = enum_TipoOperazioneDB.Scrittura
            Dim Programmazione_Des As String = "Piano Colturale"
            Dim Tipo_Pianificazione = enum_TipoPianificazione.Pianificazione_Annuale

            Dim KendoCache_R As New AgronicaCoreVarieDAL.KendoCache_R
            Dim strKendo As String = ""
            Dim objLog As New AgronicaCoreDataProvider.LogProvider


            Dim FlagTransazioneLocale As Boolean = False
            Dim FlagConnessioneLocale As Boolean = False

            Try

                '------------------------------
                'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server, System.Data.IsolationLevel.ReadUncommitted)

                '--------------------------------------------
                ' LEGGO KENDO CACHE
                If str_Appezzamenti Is Nothing Then
                    strKendo = KendoCache_R.Leggi(piva, objParametri_Server.UtenteUsername, Sito_Cod,
                                                   Pagina_Cod,
                                                   Nome_Div,
                                                   "", "", numeroValidazione, objParametri_Server)
                Else
                    strKendo = str_Appezzamenti
                End If



                '--------------------------------------------
                ' SALVO PLANNING DA KENDO CACHE
                'scrittura del piano colturale (viene sempre scritto un nuovo piano colturale).
                If DataInizio = AGRODATAINIZIO AndAlso DataFine = AGRODATAFINE AndALso Programmazione_Cod = 0 Then

                    Dim DataInizioUtente As Date = AGRODATAINIZIO
                    Dim DataFineUtente As Date = AGRODATAFINE
                    Dim Anno As Integer

                    If dataValidazione <> AGRODATAINIZIO Then
                        Programmazione_Des &= " " & dataValidazione.Year.ToString
                        If numeroValidazione <> "" Then
                            Programmazione_Des &= " (" & numeroValidazione & ")"
                        End If
                        Anno = dataValidazione.Year
                        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                        objImpost.AnnataAgraria(dataValidazione, DataInizioUtente, DataFineUtente, objParametri_Utenti)
                    Else
                        Programmazione_Des &= " " & Today.Year.ToString
                        Anno = Today.Year
                        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                        objImpost.AnnataAgraria(Today, DataInizioUtente, DataFineUtente, objParametri_Utenti)
                    End If
                    DataInizio = DataInizioUtente
                    DataFine = DataFineUtente
                Else
                    If numeroValidazione = "" Then
                        Programmazione_Des = "PC " & CStr(Date.Now.Year)
                        If Programmazione_Cod <> 0 Then
                            Dim objProgrammazione As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
                            Dim DT = objProgrammazione.Leggi("", Programmazione_Cod, piva, "", 1, AGRODATAINIZIO, AGRODATAFINE, enum_TipoPianificazione.Pianificazione_Annuale, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                                Programmazione_Des = DT.Rows(0).Item("Programmazione_Des")
                            End If
                        End If
                    Else
                        If numeroValidazione.Substring(0, 1) = "p" Then 'numeroValidazione se parte per p allora entro
                            Programmazione_Des = "PC " & CStr(Date.Now.Year)
                            If Programmazione_Cod <> 0 Then
                                Dim objProgrammazione As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R
                                Dim DT = objProgrammazione.Leggi("", Programmazione_Cod, piva, "", 1, AGRODATAINIZIO, AGRODATAFINE, enum_TipoRicetta.Non_Filtrare, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                                If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                                    Programmazione_Des = DT.Rows(0).Item("Programmazione_Des")
                                    Tipo_Pianificazione = DT.Rows(0).Item("Tipo_Pianificazione")
                                End If
                            End If
                            numeroValidazione = ""
                            pianificazioneFonte = "0"
                        Else
                            If dataValidazione <> AGRODATAINIZIO Then
                                Programmazione_Des = "Scheda N." & numeroValidazione & " " & dataValidazione.ToString("dd/MM/yyyy")
                            Else
                                Programmazione_Des = "Scheda N." & numeroValidazione & " " & Date.Now.ToString("dd/MM/yyyy")
                            End If

                        End If
                    End If
                End If

                'se il planning già esiste lo elimino
                If Programmazione_Cod <> 0 Then
                    PrimoPlanning = False
                    TipoOperazioneDB = enum_TipoOperazioneDB.Modifica
                    eliminaAppezzamenti = True
                End If

                objLog.Scrivi_LOG(objParametri_Server,
                                  NomeRoutine,
                                  "Inizio Scrittura Planning")

                Dim rvalScriviPlanning As RispostaStandard =
                    scritturaPianoColturale.Pianificazione_Scrivi_da_JSON(
                        Tipo_Operazione:=TipoOperazioneDB,
                        Programmazione_Cod_OUT:=Programmazione_Cod,
                        Programmazione_Des:=Programmazione_Des,
                        Programmazione_Des_Long:="",
                        Piva:=piva,
                        Note:="",
                        Utente:=objParametri_Server.UsernameOperazione,
                        Validita_Inizio:=DataInizio,
                        Validita_Fine:=DataFine,
                        Tipo_Pianificazione:=Tipo_Pianificazione,
                        Tipo_Fonte:=pianificazioneFonte,
                        strAppezzamenti:=strKendo,
                        NumeroDiValidazione:=numeroValidazione,
                        DataDiValidazione:=dataValidazione,
                        objParametri:=objParametri_Server,
                        Stato_SQNPI:=0,
                        SalvaAllegato:=True,
                        objParametri_Utenti:=objParametri_Utenti,
                        Pratica_Cod:=pratica_cod,
                        importatoAutomaticamente:=importatoAutomaticamente
                )

                If Not rvalScriviPlanning.RispostaOK Then
                    Throw New Exception("Errore in fase di validazione Piano del Piano Colturale:" & rvalScriviPlanning.Errore)
                End If

                'sono in inserimento salvo legame planning - fascicolo
                'If PrimoPlanning = True Then
                If allegatoDocumentocod <> 0 Then
                    Dim fascicoloAllegatiLettura As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
                    Dim fascicoloAllegatiScrittura As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_W
                    Dim dtAllegati As DataTable = fascicoloAllegatiLettura.Leggi(allegatoDocumentocod, piva, 0, 0, 0, 0, 0, "", "", "", 0, 0, "", "", "", 0, Programmazione_Cod, 0, "", "", objParametri_Server)
                    If dtAllegati.Rows.Count = 0 Then
                        fascicoloAllegatiScrittura.Scrivi(allegatoDocumentocod, 0, piva, 0, 0, 0, 0, 0, "", "", "", 0, 0, "", "", "", "",
                                                          Programmazione_Cod, 0, DataInizio, DataFine,
                                                          objParametri_Server)
                    End If
                End If
                'End If

                '--------------------------------------------
                ' ELIMINO KENDO CACHE

                If Programmazione_Cod <> 0 AndAlso numeroValidazione = "" Then
                    numeroValidazione = "p" & Programmazione_Cod
                End If

                Dim KendoCache_W As New AgronicaCoreVarieDAL.KendoCache_W
                strKendo = KendoCache_W.Cancella(piva, Enum_SiteRedirector.Sito_AgronicaSincronizzatore,
                                                               enum_PagineAgronicaSincro.ImportazionePC_Anteprima,
                                                               "kendo_Appezzamenti", numeroValidazione,
                                                               objParametri_Server)

                objLog.Scrivi_LOG(objParametri_Server,
                                  NomeRoutine,
                                  "Fine Scrittura Planning")

                'commit della transazione
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            Catch ex As Exception

                'rollback della transazione
                If objParametri_Server.objTransazione IsNot Nothing Then
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                End If

                Throw New Exception(ex.Message)

            Finally

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

            End Try

            FlagTransazioneLocale = False
            FlagConnessioneLocale = False

            Try

                '------------------------------
                'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
                AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server, IsolationLevel.ReadUncommitted)

                If bDividiCentri AndAlso PrimoPlanning Then
                    Dim rvalDividiCentri As RispostaStandard = DividiCentriCatasto(piva, Programmazione_Cod, progressivoGias, objParametri_Server, objParametri_Utenti)
                    If Not rvalDividiCentri.RispostaOK Then
                        Throw New Exception("Errore durante la divisione dei Centri :" & rvalDividiCentri.Errore)
                    End If
                End If

                'If eliminaAppezzamenti Then

                'End If

                If Programmazione_Cod <> 0 Then
                    '--------------------------------------------
                    ' LEGGO PLANNING
                    Dim letturaPianoColturale As New AgronicaCoreAnagrafeBIZ.Programmazione_R
                    Dim p As RispostaStandard = letturaPianoColturale.LeggiAppezzamenti_xRibaltamento_JSON(piva, Programmazione_Cod,
                                                                                       DataInizio, DataFine, False,
                                                                                       "", objParametri_Server, True)

                    Dim dati As String = p.RispostaStringa
                    Dim d As JObject = JObject.Parse(dati)
                    Dim datifinali As JArray = d("kendo_rows")
                    Dim d1 As String = JsonConvert.SerializeObject(datifinali, Formatting.None)

                    Dim datiChiusi As String = ""

                    'CREO PRATICA PER ENTITA DI PLANNING
                    Dim obj_Dati As RibaltaAppezza()
                    obj_Dati = JsonConvert.DeserializeObject(Of RibaltaAppezza())(d1)
                    scritturaPianoColturale.ControlloScrittura_EntitaPratiche(obj_Dati, objParametri_Server, objParametri_Utenti)

                    If Ribalta Then
                        '--------------------------------------------
                        ' RIBALTO PLANNING SU REALE

                        Dim Nome_Campo As String = numeroValidazione
                        Dim objCampo As New AgronicaCoreAnagrafeDAL.Campi_R

                        Dim DT_Campi = objCampo.Leggi(piva, 0, 0,
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       " Campo_Des like '%" & Nome_Campo & "%' ", "", objParametri_Server)

                        Nome_Campo = Programmazione_Des & " " & (DT_Campi.Rows.Count + 1)

                        Dim Blocca_Appezza = True
                        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

                        If objConfSiti.Leggi_Valore(16, "Sincro_ImportAnagrafica_BloccaAppezzamenti", "", "", objParametri_Server).ToLower = "true" Then
                            Blocca_Appezza = True
                        Else
                            Blocca_Appezza = False
                        End If

                        Dim Chk_Crea_Campo As Boolean = True
                        If numeroValidazione <> "" AndAlso numeroValidazione <> "0" Then
                            Chk_Crea_Campo = True
                        End If

                        If Tipo_Pianificazione = enum_TipoPianificazione.Pianificazione_RichiestaPiante Then
                            Chk_Crea_Campo = False
                        End If

                        objLog.Scrivi_LOG(objParametri_Server,
                                      NomeRoutine,
                                      "Inizio Ribaltamento")


                        Dim rvalRibalta As RispostaStandard =
                                scritturaPianoColturale.Ribalta(
                                    dati:=d1,
                                datiChiusi:=datiChiusi,
                                Cmb_Regolamento_Value:=0,
                                Chk_Chiudi_Arboree:=True,
                                Chk_Crea_Campo:=Chk_Crea_Campo,
                                Campo_Des:=Nome_Campo,
                                ProgressivoGIAS:=progressivoGias,
                                paramValidita_Fine:=DataFine,
                                objParametri_Server:=objParametri_Server,
                                objParametri_Utenti:=objParametri_Utenti,
                                Blocca_Appezza:=Blocca_Appezza
                                )

                        objLog.Scrivi_LOG(objParametri_Server,
                                      NomeRoutine,
                                      "Fine Ribaltamento")

                        If Not rvalRibalta.RispostaOK Then
                            Throw New Exception("Errore durante il ribaltamento:" & rvalRibalta.Errore)
                        Else
                            If PrimoPlanning Then
                                'avanzamento del workflow per l'impresa
                                Dim gestioneWorkflow As New AgronicaCoreProfilazioneBIZ.Pratiche_W
                                gestioneWorkflow.impostaPratica(enum_WWorflow.Attivazione_Aziende_Agrarie_in_GIAS,
                                                                piva, cuaa, objParametri_Server.UtenteUsername,
                                                                enum_Servizi.Workflow_di_attivazione_aziende_GIAS,
                                                                enum_WWorflow_WAnagraficaStati.Attivazione_Aziende_Agrarie_in_GIAS_Piano_Colturale_Confermato,
                                                                objParametri_Server,
                                                                objParametri_Utenti, 0, "", 0, False, "",
                                                                AGRODATAINIZIO, AGRODATAFINE, 0, 0)
                            End If
                        End If
                    End If

                End If


                'commit della transazione
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            Catch ex As Exception

                'rollback della transazione
                If objParametri_Server.objTransazione IsNot Nothing Then
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                End If

                Throw New Exception(ex.Message)

            Finally

                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

            End Try

            rval.RispostaOK = True
            rval.RispostaStringa = Programmazione_Cod.ToString ' "Operazione di creazione Piano Colturale Completata con successo"

        Catch ex As Exception

            rval.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            rval.Errore = MessaggioErrore

        End Try

        Return rval

    End Function

    Private Function DividiCentriCatasto(ByVal piva As String,
                                         ByVal Programmazione_Cod As Integer,
                                         ByVal progressivoGias As Integer,
                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreParametri) As RispostaStandard

        'verifico se le particelle del planning sono su diversi comuni
        'in tal caso creo i nuovi centri
        'sposto il catasto
        'sposto le entità del planning
        Dim r As New RispostaStandard

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim objPP As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
            Dim DT_Comuni As New DataTable
            DT_Comuni = objPP.LeggiProvComParticelle_Da_Programmazione_Distinct(Programmazione_Cod, 0, "", "", objParametri_Server)

            If DT_Comuni.Rows.Count > 1 Then

                'leggo il sa_cod originale
                Dim Sa_Cod_Old As Integer = 0
                Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                Dim Dt_Centri As DataTable
                Dt_Centri = objCentri.Leggi(piva, 0,
                                            enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                            "", "",
                                            objParametri_Server)

                If Not IsNothing(Dt_Centri) AndAlso Dt_Centri.Rows.Count > 0 Then
                    Sa_Cod_Old = Dt_Centri.Rows(0).Item("sa_cod")
                End If

                For i = 0 To DT_Comuni.Rows.Count - 1

                    Dim rval As New RispostaStandard
                    Dim Sa_Cod_New As Integer

                    'creo il centro
                    rval = Crea_Centro(piva,
                                DT_Comuni.Rows(i).Item("prov"),
                                DT_Comuni.Rows(i).Item("com"),
                                DT_Comuni.Rows(i).Item("localita"),
                                DT_Comuni.Rows(i).Item("comuni_prov"),
                                DT_Comuni.Rows(i).Item("cap"),
                                progressivoGias,
                                objParametri_Server,
                                objParametri_Utenti)

                    If rval.RispostaOK Then

                        'leggo nuovo sa_cod
                        Dim objUltimo As New AgronicaCoreDataProvider.Agro_Sequenze
                        Sa_Cod_New = objUltimo.UltimoSaCod(piva, objParametri_Server)

                        Try
                            Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_W
                            objUtentiVisibilita.Scrivi(enum_TipoEntita.Centro, piva, Sa_Cod_New, 0, 0, objParametri_Server)

                        Catch ex As Exception

                        End Try

                        'sposto le particelle sul nuovo centro


                        Dim objP As New AgronicaCoreAnagrafeBIZ.Particella_W
                        Dim SPostataParticella As Boolean = False
                        SPostataParticella = objP.Modifica_Centro_Particella(piva,
                                                        Sa_Cod_Old, Sa_Cod_New,
                                                        DT_Comuni.Rows(i).Item("prov"), DT_Comuni.Rows(i).Item("com"),
                                                        "", 0, 0, "",
                                                        objParametri_Server)
                    Else
                        Throw New Exception("Errore durante la divisione dei Centri :" & rval.Errore)
                    End If

                Next

            End If

            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            If objParametri_Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return r

    End Function

    Private Function Crea_Centro(
            ByVal Piva As String,
            ByVal Prov As String,
            ByVal Com As String,
            ByVal Com_Des As String,
            ByVal Prov_Sigla As String,
            ByVal Cap As String,
            ByVal progressivoGias As Integer,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Utenti As AgronicaCoreParametri
        ) As RispostaStandard

        Dim rval As New RispostaStandard


        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim letturaAzienda As New AgronicaCoreAnagrafeDAL.Imprese_Read

            Dim Esisteimpresa As Boolean = letturaAzienda.Esiste_Impresa(Piva, 0, AGRODATAINIZIO, "", "", "", objParametri_Server)
            Dim TipoOperazione As enum_TipoOperazioneDB

            Dim objXML As New AgronicaCoreXML.AnagrafeXML

            Dim Piva_Padre As String = objParametri_Server.PivaSuperUser

            Dim Cuaa As String = Piva ' righeModificateArray(0)("cuaa")
            Dim RagSoc As String = "pippo" 'righeModificateArray(0)("Rag_Soc")
            Dim Indirizzo As String = "." 'righeModificateArray(0)("Indirizzo")
            Dim Cod_Indirizzo As Integer = -1
            Dim stato As String = ""

            TipoOperazione = enum_TipoOperazioneDB.Lettura


            Dim xLeggiCodIndir As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
            Dim xLeggiGerarchia As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R

            Dim dtLeggiCodIndir As DataTable =
                    xLeggiCodIndir.Leggi2(Piva, True, 0, 1, "", "", objParametri_Server)

            If dtLeggiCodIndir.Rows.Count > 0 Then
                Cod_Indirizzo = dtLeggiCodIndir.Rows(0)("Cod_Indirizzo")
            End If

            Piva_Padre = xLeggiGerarchia.LeggiPadre(Piva, objParametri_Server, "")

            Dim xmlDoc As New Xml.XmlDocument

            Dim XmlImpresa As System.Xml.XmlElement
            Dim XmlCentro As System.Xml.XmlElement

            Dim logErrori As String = ""

            Dim baseCode As Integer
            Dim topCode As Integer

            UtilityProvider.Calcola_BaseCode_TopCode(baseCode, topCode, progressivoGias)

            Dim XmlUtente As System.Xml.XmlElement

            XmlUtente = objXML.Xml_Pubblico_Utente(
                xmlDoc,
                objParametri_Server.UtenteUsername,
                "#",
                progressivoGias
            )

            XmlImpresa = objXML.Xml_Pubblico_Impresa(
                TipoOperazione,
                Piva,
                RagSoc,
                "#",
                Cuaa,
                "#",
                "#",
                Piva_Padre,
                "1",
                "1",
                "#", "#",
                Indirizzo,
                "#",
                Cap,
                Com_Des,
                Prov_Sigla,
                "#", "#",
                Com,
                Prov,
                "",
                "#",
                "#",
                "#",
                "#",
                "#", "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#",
                "#", "#", "#", "#", "#", "#",
                "#",
                xmlDoc)


            Dim SaCod As Integer = 0
            Dim SaNome As String = Com_Des 'Prov_Sigla & " - " & Com_Des '"01"

            XmlCentro = objXML.Xml_Pubblico_CentroAziendale(enum_TipoOperazioneDB.Scrittura,
                                                            SaCod,
                                                            SaNome,
                                                            1,
                                                            "#", "#", "#", "#", "#", "#",
                                                            Indirizzo,
                                                            "#",
                                                            Cap,
                                                            Com_Des,
                                                            Prov_Sigla,
                                                            "#", "#",
                                                            Com,
                                                            Prov,
                                                            "#", "#", "#", "#", "#", "#", "#", "#", "#",
                                                             "#",
                                                            xmlDoc)



            XmlImpresa.AppendChild(XmlCentro)

            XmlUtente.AppendChild(XmlImpresa)

            xmlDoc.AppendChild(XmlUtente)

            Dim objAgronica_XML As New AgronicaCoreXML.Agronica_XML

            Dim strErrPP As String = ""
            Dim HT_Piva_objCodici As New Hashtable

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim codiceChiaveCliente As Integer

            codiceChiaveCliente = objConfSiti.Leggi_Valore(16, "Sincro_Codice_Chiave_Cliente", "", "", objParametri_Server)

            Dim Str_XML_Privato As String = objAgronica_XML.XMLPrivato_from_XMLPubblico(
                objParametri_Server,
                HT_Piva_objCodici,
                xmlDoc.OuterXml,
                codiceChiaveCliente,
                strErrPP)

            If strErrPP <> "" Then
                Throw New Exception("Errore in fase di generazione xml privato:" & strErrPP)
            End If

            Dim scriviImpresa As New Importa_GIAS

            'If Esisteimpresa Then
            '    Str_XML_Privato = Str_XML_Privato.Replace("TipoOperazioneDB=""1""", "TipoOperazioneDB=""2""")
            'End If

            scriviImpresa.Scrivi_Anagrafe(Str_XML_Privato, Piva, 0, Id_Servizio_GiasOnline, 0, objParametri_Server, objParametri_Utenti)

            rval.RispostaOK = True
            rval.RispostaStringa = "Salvataggio eseguito con successo."

        Catch ex As Exception

            If objParametri_Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            'uso questa funzione per ottenere il Messaggio..:
            Dim messaggioErrore As String = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            rval.RispostaOK = False
            rval.Errore = messaggioErrore

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return rval


    End Function

    Public Function SpostaParticelle_Impianti_Entita(ByVal Piva As String,
                                                      ByVal sa_codFrom As Integer,
                                                      ByVal sa_codTo As Integer,
                                                      ByVal listaParticelle As String(),
                                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                                      ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                      progressivoGIAS As Integer) As RispostaStandard

        'verifico se le particelle del planning sono su diversi comuni
        'in tal caso creo i nuovi centri
        'sposto il catasto
        'sposto le entità del planning
        Dim r As New RispostaStandard

        Dim FlagTransazioneLocale As Boolean = True
        Dim FlagConnessioneLocale As Boolean = True

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim objMessage As New JArray()

            Dim validita_inizio_finestra As Date = objParametri_Server.FinestraTemporaleInizio
            Dim validita_fine_finestra As Date = objParametri_Server.FinestraTemporaleFine

            objParametri_Server.FinestraTemporaleInizio = AGRODATAINIZIO
            objParametri_Server.FinestraTemporaleFine = AGRODATAFINE

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)

            Dim tutte_selezionate As Boolean = True
            Dim NonSelezionate_List As New List(Of String)
            For Each part In listaParticelle
                Dim objM As New JObject()
                objM.Add(New JProperty("Particella", part))
                Dim objPartDip As New JArray()
                objM.Add(New JProperty("ParticelleDipendenti", objPartDip))
                Dim partSorelle = ElencoParticelleInteressate(part, Piva, sa_codFrom, sa_codTo, objParametri_Server, objParametri_Utenti)
                For Each partSorella In partSorelle
                    If Not listaParticelle.Contains(partSorella) Then
                        tutte_selezionate = False
                        NonSelezionate_List.Add(partSorella)
                        objPartDip.Add(partSorella)
                    End If
                Next
                If objPartDip.Count > 0 Then
                    objMessage.Add(objM)
                End If
            Next
            Dim objRet As New JObject()
            If tutte_selezionate Then
                For Each part In listaParticelle
                    SpostaParticella(part, Piva, sa_codFrom, sa_codTo, objParametri_Server, objParametri_Utenti, progressivoGIAS)
                Next
                objRet.Add(New JProperty("ReturnCode", "0"))
                objRet.Add(New JProperty("Message", "Spostamento effettuato correttamente."))
            Else
                objRet.Add(New JProperty("ReturnCode", "1"))
                objRet.Add(New JProperty("Message", objMessage))
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            objParametri_Server.FinestraTemporaleInizio = validita_inizio_finestra
            objParametri_Server.FinestraTemporaleFine = validita_fine_finestra

            r.RispostaStringa = objRet.ToString
            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            If objParametri_Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return r

    End Function

    Public Function SpostaAppezzamenti_Impianti_Entita(ByVal Piva As String,
                                                      ByVal sa_codFrom As Integer,
                                                      ByVal sa_codTo As Integer,
                                                      ByVal listaAppezzamenti As String(),
                                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                                      ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                      progressivoGIAS As Integer) As RispostaStandard

        'sposto il catasto
        'sposto le entità del planning
        Dim r As New RispostaStandard

        Dim FlagTransazioneLocale As Boolean = True
        Dim FlagConnessioneLocale As Boolean = True

        Try

            'Se la connessione è chiusa la apro e se la transazione è chiusa la inizio
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim objMessage As New JArray()
            Dim validita_inizio_finestra As Date = objParametri_Server.FinestraTemporaleInizio
            Dim validita_fine_finestra As Date = objParametri_Server.FinestraTemporaleFine

            objParametri_Server.FinestraTemporaleInizio = AGRODATAINIZIO
            objParametri_Server.FinestraTemporaleFine = AGRODATAFINE

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)


            Dim tutte_selezionate As Boolean = True
            Dim NonSelezionate_List As New List(Of String)
            For Each app In listaAppezzamenti
                Dim appDes = DescrizioneAppezzamentoDaChiave(app, objParametri_Server)

                Dim objM As New JObject()
                objM.Add(New JProperty("Appezzamento", app))
                objM.Add(New JProperty("Appezzamento_Des", appDes))
                Dim objAppDip As New JArray()
                objM.Add(New JProperty("AppezzamentiDipendenti", objAppDip))
                Dim AppFratelli = ElencoAppezzamentiInteressati(app, Piva, sa_codFrom, sa_codTo, objParametri_Server, objParametri_Utenti)
                For Each appFratello In AppFratelli
                    If Not listaAppezzamenti.Contains(appFratello) Then
                        tutte_selezionate = False
                        NonSelezionate_List.Add(appFratello)
                        Dim appFratelloDes = DescrizioneAppezzamentoDaChiave(appFratello, objParametri_Server)
                        Dim objAppFratello As New JObject()
                        objAppFratello.Add(New JProperty("chiave", appFratello))
                        objAppFratello.Add(New JProperty("des", appFratelloDes))
                        objAppDip.Add(objAppFratello)
                    End If
                Next
                If objAppDip.Count > 0 Then
                    objMessage.Add(objM)
                End If
            Next
            Dim objRet As New JObject()
            If tutte_selezionate Then
                Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                For Each part In listaAppezzamenti
                    Dim Appezza As Integer = part.Split("_")(2)
                    Dim dt = objImpianti.Leggi_Operazioni_Impianti(Piva, sa_codFrom, Appezza, 0, "", "", objParametri_Server)
                    If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                        Throw New Exception("Appezzamento Movimentato (" & Piva & "_" & sa_codFrom & "_" & Appezza & ")")
                    End If
                    SpostaAppezzamento(part, Piva, sa_codFrom, sa_codTo, objParametri_Server, objParametri_Utenti, progressivoGIAS)
                Next
                objRet.Add(New JProperty("ReturnCode", "0"))
                objRet.Add(New JProperty("Message", "Spostamento effettuato correttamente."))
            Else
                objRet.Add(New JProperty("ReturnCode", "1"))
                objRet.Add(New JProperty("Message", objMessage))
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(validita_inizio_finestra, validita_fine_finestra)

            r.RispostaStringa = objRet.ToString
            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            If objParametri_Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return r

    End Function

    Private Function DescrizioneAppezzamentoDaChiave(app As String, ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim objReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        Dim appArr = app.Split("_")

        Dim piva = appArr(0)
        Dim sa_cod = appArr(1)
        Dim appezza = appArr(2)

        Dim dt As DataTable = objAppezza.Leggi(piva, sa_cod, appezza, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        dt.Columns.Add(New DataColumn("utilizzo"))
        For Each rowAppezza In dt.Rows

            Dim dt_Impianto = objReg_Impianti.Leggi_x_anagrafica(rowAppezza("piva"), rowAppezza("sa_cod"), rowAppezza("appezza"), 0, 0, "", "", objParametri_Server)
            If dt_Impianto.Rows.Count > 0 Then
                Dim rowImpianto = dt_Impianto.Rows(0)
                If rowImpianto("cul_cod") = 0 Then

                    Dim val = objReg_Impianti.Codice_Anagrafe_from_PivaSaCodAppezzaIdImp(rowImpianto("PIVA"), rowImpianto("SA_COD"), rowImpianto("APPEZZA"), rowImpianto("ID_REG"), objParametri_Server)
                    If val = "" Then
                        val = Str_TerrenoNudo
                    End If
                    rowAppezza("utilizzo") = val
                Else

                    rowAppezza("utilizzo") = rowImpianto("veg_des") & " - " & rowImpianto("cul_des")

                End If
            End If
        Next

        If dt.Rows.Count > 0 Then
            Return CStr(dt.Rows(0)("APP_NOME")) & " " & CStr(dt.Rows(0)("Utilizzo")) & " " & CStr(dt.Rows(0)("SUP_APP"))
        End If

        Return ""

    End Function

    Private Sub SpostaParticella(ByVal part As String,
                                 ByVal Piva As String,
                                 ByVal sa_codFrom As Integer,
                                 ByVal sa_codTo As Integer,
                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreParametri,
                                 progressivoGIAS As Integer)

        Dim partArr = part.Split("_")

        Dim prov As String = partArr(0).ToString
        Dim com As String = partArr(1).ToString
        Dim sezione As String = partArr(2).ToString
        Dim foglio As Integer = CInt(partArr(3).ToString)
        Dim numero As Integer = CInt(partArr(4).ToString)
        Dim subalterno As String = partArr(5).ToString

        Dim objImpresexParticelle_R As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_R
        Dim objImpresexParticelle_W As New AgronicaCoreAnagrafeDAL.ImpresexParticelle2_W

        Dim dtImpresexParticelle = objImpresexParticelle_R.Leggi(0, Piva, sa_codFrom, 0, prov, com, sezione, foglio, numero, subalterno, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        objImpresexParticelle_W.Modifica_Centro(Piva, sa_codFrom, sa_codTo, prov, com, sezione, foglio, numero, subalterno, objParametri_Server)


        Dim objProgrammazioneEntita_R As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
        Dim objProgrammazioneEntita_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W

        Dim objGis_Entita_R As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim objGis_Entita_W As New AgronicaCoreGisDAL.GIS_Entita_W

        Dim dtEntita = objProgrammazioneEntita_R.Leggi_Entita_Data_Particella(Nothing, Piva, prov, com, sezione, foglio, numero, subalterno, "", objParametri_Server)
        For Each entita In dtEntita.Rows
            objProgrammazioneEntita_W.Modifica(entita("Programmazione_Cod"),
                                               entita("Programmazione_Entita_Cod"),
                                               StrDefault_per_MODIFICA,
                                               Piva,
                                               sa_codTo,
                                               0,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               StrDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               DoubleDefault_per_MODIFICA,
                                               DoubleDefault_per_MODIFICA,
                                               StrDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               DoubleDefault_per_MODIFICA,
                                               DoubleDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               DataDefault_per_MODIFICA,
                                               DataDefault_per_MODIFICA,
                                               StrDefault_per_MODIFICA,
                                               StrDefault_per_MODIFICA,
                                               StrDefault_per_MODIFICA,
                                               StrDefault_per_MODIFICA,
                                               IntDefault_per_MODIFICA,
                                               DataDefault_per_MODIFICA,
                                               DataDefault_per_MODIFICA,
                                               DataDefault_per_MODIFICA,
                                               "",
                                               objParametri_Server)

            'GIS
            Dim dtEntitaGIS = objGis_Entita_R.LeggiDB(objParametri_Server.PivaSuperUser, 0, 0, Piva, sa_codFrom, 0, 0, 0, "", "", "", 0, 0, "", 0, 0, entita("programmazione_Cod"), entita("Programmazione_Entita_Cod"), "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            For Each entitaGIS In dtEntitaGIS.Rows
                objGis_Entita_W.Modifica(objParametri_Server.PivaSuperUser,
                                         entitaGIS("Entita_Cod"),
                                         Nothing, Nothing,
                                         sa_codTo, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing,
                                         Nothing, Nothing, Nothing, Nothing, objParametri_Server)

            Next

        Next


        Dim agroSeq As New AgronicaCoreDataProvider.Agro_Sequenze

        Dim BaseCode As Long
        Dim TopCode As Long
        agroSeq.Calcola_BaseCode(progressivoGIAS, TopCode, BaseCode, objParametri_Server)

        Dim objAppezzaxParticelle_R As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
        Dim dtAppezza = objAppezzaxParticelle_R.LeggiAppezzamenti_Da_Particella(prov, com, sezione, foglio, numero, subalterno, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        For Each appezzamento In dtAppezza.Rows
            Dim appCod As String = appezzamento("PIVA") & "_" & appezzamento("Sa_Cod") & "_" & appezzamento("appezza")
            SpostaAppezzamentoInterno(appCod, Piva, sa_codFrom, sa_codTo, objParametri_Server, objParametri_Utenti, progressivoGIAS)
        Next

    End Sub

    Private Sub SpostaAppezzamento(ByVal app As String,
                                   ByVal Piva As String,
                                   ByVal sa_codFrom As Integer,
                                   ByVal sa_codTo As Integer,
                                   ByRef objParametri_Server As AgronicaCoreParametri,
                                   ByRef objParametri_Utenti As AgronicaCoreParametri,
                                   progressivoGIAS As Integer)

        Dim appArr = app.Split("_")

        Dim appezza As String = appArr(2).ToString

        Dim objAppezzaxParticelle_R As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R

        Dim objAppezza_R As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        Dim agroSeq As New AgronicaCoreDataProvider.Agro_Sequenze

        Dim BaseCode As Long
        Dim TopCode As Long
        agroSeq.Calcola_BaseCode(progressivoGIAS, TopCode, BaseCode, objParametri_Server)

        Dim DTApp = objAppezza_R.Leggi(Piva, sa_codFrom, appezza, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        If DTApp.Rows.Count > 0 Then

            Dim DTAppxPart = objAppezzaxParticelle_R.LeggiParticelle_Da_Appezzamento(Piva, sa_codFrom, appezza, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            If DTAppxPart.Rows.Count > 0 Then

                'Appezzamento con gestione catastale, sposto direttamente la particella
                Dim part = DTAppxPart.Rows(0)("PROV") & "_" & DTAppxPart.Rows(0)("COM") & "_" & DTAppxPart.Rows(0)("Sezione") & "_" & DTAppxPart.Rows(0)("Foglio") & "_" & DTAppxPart.Rows(0)("Numero") & "_" & DTAppxPart.Rows(0)("Subalterno")
                SpostaParticella(part, Piva, sa_codFrom, sa_codTo, objParametri_Server, objParametri_Utenti, progressivoGIAS)

            Else
                'Appezzamento senza gestione catastale
                SpostaAppezzamentoInterno(Piva & "_" & sa_codFrom & "_" & CStr(DTApp.Rows(0)("appezza")), Piva, sa_codFrom, sa_codTo, objParametri_Server, objParametri_Utenti, progressivoGIAS)

            End If

        End If

    End Sub

    Private Function ElencoParticelleInteressate(ByVal part As String,
                                                 ByVal piva As String,
                                                 ByVal sa_codFrom As Integer,
                                                 ByVal sa_codTo As Integer,
                                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                                 ByRef objParametri_Utenti As AgronicaCoreParametri) As String()

        Dim objProgrammazioneParticelle As New AgronicaCoreAnagrafeDAL.Programmazione_Particelle_R
        Dim objProgrammazioneEntita As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

        Dim partInteressate As New List(Of String)
        Dim partArr = part.Split("_")

        Dim prov As String = partArr(0).ToString
        Dim com As String = partArr(1).ToString
        Dim sezione As String = partArr(2).ToString
        Dim foglio As Integer = CInt(partArr(3).ToString)
        Dim numero As Integer = CInt(partArr(4).ToString)
        Dim subalterno As String = partArr(5).ToString

        Dim dtEntita = objProgrammazioneEntita.Leggi_Entita_Data_Particella(0, piva, prov, com, sezione, foglio, numero, subalterno, "", objParametri_Server)
        Dim dtParticelleP As DataTable
        Dim chiave As String
        For Each entita In dtEntita.Rows
            dtParticelleP = objProgrammazioneParticelle.LeggiParticelle_Da_Programmazione(piva, 0, 0, entita("Programmazione_Entita_Cod"), enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            For Each particella In dtParticelleP.Rows
                chiave = CStr(particella("PROV")) & "_" &
                         CStr(particella("COM")) & "_" &
                         CStr(particella("SEZIONE")) & "_" &
                         CStr(particella("FOGLIO")) & "_" &
                         CStr(particella("NUMERO")) & "_" &
                         CStr(particella("SUBALTERNO"))

                If Not partInteressate.Contains(chiave) Then
                    partInteressate.Add(chiave)
                End If
            Next
        Next

        Dim objAppezzaxParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R


        Dim dtAppezza = objAppezzaxParticelle.LeggiAppezzamenti_Da_Particella(prov, com, sezione, foglio, numero, subalterno, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        Dim dtParticelleA As DataTable
        For Each appezzamento In dtAppezza.Rows
            dtParticelleA = objAppezzaxParticelle.LeggiParticelle_Da_Appezzamento(piva, appezzamento("sa_cod"), appezzamento("appezza"), AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            For Each particella In dtParticelleA.Rows
                chiave = CStr(particella("PROV")) & "_" &
                         CStr(particella("COM")) & "_" &
                         CStr(particella("SEZIONE")) & "_" &
                         CStr(particella("FOGLIO")) & "_" &
                         CStr(particella("NUMERO")) & "_" &
                         CStr(particella("SUBALTERNO"))

                If Not partInteressate.Contains(chiave) Then
                    partInteressate.Add(chiave)
                End If
            Next
        Next

        Return partInteressate.ToArray

    End Function

    Private Function ElencoAppezzamentiInteressati(ByVal app As String,
                                                 ByVal piva As String,
                                                 ByVal sa_codFrom As Integer,
                                                 ByVal sa_codTo As Integer,
                                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                                 ByRef objParametri_Utenti As AgronicaCoreParametri) As String()

        Dim objAppezzaxParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
        Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        Dim appInteressati As New List(Of String)
        Dim partApp = app.Split("_")

        Dim sa_cod As String = partApp(1).ToString
        Dim appezza As String = partApp(2).ToString

        Dim dtParticelle = objAppezzaxParticelle.LeggiParticelle_Da_Appezzamento(piva, sa_cod, appezza, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        Dim dtEntitaP As DataTable
        Dim chiave As String
        For Each particella In dtParticelle.Rows
            dtEntitaP = objAppezzaxParticelle.LeggiAppezzamenti_Da_Particella(particella("PROV"), particella("COM"), particella("Sezione"), particella("Foglio"), particella("Numero"), particella("Subalterno"), enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            For Each entita In dtEntitaP.Rows
                chiave = CStr(entita("Piva")) & "_" &
                         CStr(entita("sa_cod")) & "_" &
                         CStr(entita("appezza"))

                If Not appInteressati.Contains(chiave) Then
                    appInteressati.Add(chiave)
                    AppCoinvolti(appInteressati, entita("Piva"), entita("sa_cod"), entita("appezza"), objParametri_Server)
                End If
            Next
        Next

        Return appInteressati.ToArray

    End Function

    Public Sub AppCoinvolti(ByRef listaApp As List(Of String), piva As String, sa_cod As Integer, appezza As Integer, objParametri_server As AgronicaCoreParametri)
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim objAppezzaParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R

        Dim dtParticelle = objAppezzaParticelle.LeggiParticelle_Da_Appezzamento(piva, sa_cod, appezza, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)
        For Each partRow In dtParticelle.Rows
            'Dim strPart = ""
            'strPart = partRow("Prov") & "_" & partRow("Com") & "_" & partRow("Sezione") & "_" & partRow("Foglio") & "_" & partRow("Numero") & "_" & partRow("Subalterno")
            'If Not listaParticelleCoinvolte.Contains(strPart) Then
            '    listaParticelleCoinvolte.Add(strPart)
            'End If
            Dim dtAppezza = objAppezzaParticelle.LeggiAppezzamenti_Da_Particella(partRow("Prov"),
                                                                 partRow("Com"),
                                                                 partRow("Sezione"),
                                                                 partRow("Foglio"),
                                                                 partRow("Numero"),
                                                                 partRow("Subalterno"),
                                                                 enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                 "", "",
                                                                 objParametri_server)

            For Each rowAppezza In dtAppezza.Rows
                Dim strAppezza = rowAppezza("Piva") & "_" & rowAppezza("sa_cod") & "_" & rowAppezza("Appezza")
                If Not listaApp.Contains(strAppezza) Then
                    listaApp.Add(strAppezza)
                    AppCoinvolti(listaApp, rowAppezza("Piva"), rowAppezza("sa_cod"), rowAppezza("Appezza"), objParametri_server)
                End If
            Next

        Next

    End Sub

    Private Sub SpostaAppezzamentoInterno(ByVal app As String,
                                          ByVal Piva As String,
                                          ByVal sa_codFrom As Integer,
                                          ByVal sa_codTo As Integer,
                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreParametri,
                                          progressivoGIAS As Integer)
 
        Dim appArr = app.Split("_")

        Dim appezza As String = appArr(2).ToString

        Dim objProgrammazioneEntita_R As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
        Dim objProgrammazioneEntita_W As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_W

        Dim objGis_Entita_R As New AgronicaCoreGisDAL.GIS_Entita_R
        Dim objGis_Entita_W As New AgronicaCoreGisDAL.GIS_Entita_W

        Dim objAppezzaxParticelle_R As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
        Dim objAppezzaxParticelle_W As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W

        Dim objAppezza_W As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
        Dim objAppezza_R As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        Dim objAppezza_Codici_W As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_W

        Dim objAppezzaxParticellexMacrousi_W As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousi_W
        Dim objAppezzaxParticellexMacrousi_R As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousi_R

        Dim objAppezzaxParticellexMacrousixUtilizzo_W As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_W
        Dim objAppezzaxParticellexMacrousixUtilizzo_R As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_R

        Dim objreg_Impianti_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim objreg_Impianti_W As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write

        Dim objreg_Impianti_Codici_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dim objreg_Impianti_Codici_W As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W

        Dim objreg_Impianti_Programmazioni_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_R
        Dim objreg_Impianti_Programmazioni_W As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Programmazioni_W

        Dim objImprese_Progetti_R As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        Dim objImprese_Progetti_W As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W

        Dim objProgettixParticelle_R As New AgronicaCoreAnagrafeDAL.ProgettixParticelle_R
        Dim objProgettixParticelle_W As New AgronicaCoreAnagrafeDAL.ProgettixParticelle_W

        Dim objUtentixAppezzamenti_R As New AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_R
        Dim objUtentixAppezzamenti_W As New AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W

        Dim agroSeq As New AgronicaCoreDataProvider.Agro_Sequenze

        Dim BaseCode As Long
        Dim TopCode As Long
        agroSeq.Calcola_BaseCode(progressivoGIAS, TopCode, BaseCode, objParametri_Server)

        Dim dtAppezza = objAppezza_R.Leggi(Piva, sa_codFrom, appezza, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        For Each appezzamento In dtAppezza.Rows
            'APPEZZAMENTO

            Dim appezza_NEW = agroSeq.NuovoId_Appezzamento(Piva, sa_codTo, BaseCode, TopCode, objParametri_Server)
            Dim dtUtentxapp = objUtentixAppezzamenti_R.Leggi(Piva, sa_codFrom, 0, appezzamento("appezza"), False, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim dtAppezzaxParticelle = objAppezzaxParticelle_R.LeggiParticelle_Da_Appezzamento(Piva, sa_codFrom, appezza, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            For Each appezzaxPart In dtAppezzaxParticelle.Rows
                objAppezzaxParticelle_W.ModificaChiave2(Piva,
                                                                    sa_codTo,
                                                                    appezza_NEW,
                                                                    appezzaxPart("PROV"),
                                                                    appezzaxPart("COM"),
                                                                    appezzaxPart("SEZIONE"),
                                                                    appezzaxPart("FOGLIO"),
                                                                    appezzaxPart("NUMERO"),
                                                                    appezzaxPart("SUBALTERNO"),
                                                                    Piva,
                                                                    sa_codFrom,
                                                                    appezzaxPart("APPEZZA"),
                                                                    appezzaxPart("PROV"),
                                                                    appezzaxPart("COM"),
                                                                    appezzaxPart("SEZIONE"),
                                                                    appezzaxPart("FOGLIO"),
                                                                    appezzaxPart("NUMERO"),
                                                                    appezzaxPart("SUBALTERNO"), "", objParametri_Server)

                'objAppezza_Codici_W.Modifica_Parametrizzata(Piva, sa_codFrom, appezzamento("APPEZZA"), "Appezza", appezza_NEW, "", objParametri_Server)
                'objAppezza_Codici_W.Modifica_Parametrizzata(Piva, sa_codFrom, appezza_NEW, "Sa_Cod", sa_codTo, "", objParametri_Server)
                objAppezza_Codici_W.Modifica_Chiave(Piva, sa_codTo, appezza_NEW, Piva, sa_codFrom, appezzaxPart("appezza"), "", objParametri_Server)

                Dim dtAppezzaxParticellexMacrousi = objAppezzaxParticellexMacrousi_R.Leggi(Piva, sa_codFrom,
                                                                                           appezzaxPart("APPEZZA"),
                                                                                           appezzaxPart("PROV"),
                                                                                            appezzaxPart("COM"),
                                                                                            appezzaxPart("SEZIONE"),
                                                                                            appezzaxPart("FOGLIO"),
                                                                                            appezzaxPart("NUMERO"),
                                                                                            appezzaxPart("SUBALTERNO"), "", "", "", objParametri_Server)

                For Each appezzaxmacrousi In dtAppezzaxParticellexMacrousi.Rows
                    objAppezzaxParticellexMacrousi_W.Modifica_Parametrizzata(appezzaxmacrousi("ID"), "appezza", appezza_NEW, "", objParametri_Server)
                    objAppezzaxParticellexMacrousi_W.Modifica_Parametrizzata(appezzaxmacrousi("ID"), "SA_COD", sa_codTo, "", objParametri_Server)
                Next

                Dim dtAppezzaxParticellexMacrousixUtilizzi = objAppezzaxParticellexMacrousixUtilizzo_R.Leggi(Piva,
                                                                                                             sa_codFrom,
                                                                                                             appezzaxPart("APPEZZA"),
                                                                                                             appezzaxPart("PROV"),
                                                                                                             appezzaxPart("COM"),
                                                                                                             appezzaxPart("SEZIONE"),
                                                                                                             appezzaxPart("FOGLIO"),
                                                                                                             appezzaxPart("NUMERO"),
                                                                                                             appezzaxPart("SUBALTERNO"), "", "", "", "", "", objParametri_Server)

                For Each appezzaxmacrousixutilizzi In dtAppezzaxParticellexMacrousixUtilizzi.Rows
                    objAppezzaxParticellexMacrousixUtilizzo_W.Modifica_Parametrizzata(appezzaxmacrousixutilizzi("ID"), "appezza", appezza_NEW, "", objParametri_Server)
                    objAppezzaxParticellexMacrousixUtilizzo_W.Modifica_Parametrizzata(appezzaxmacrousixutilizzi("ID"), "SA_COD", sa_codTo, "", objParametri_Server)
                Next
            Next




            'objAppezza_W.Modifica_Parametrizzata(Piva, sa_codFrom, appezzamento("APPEZZA"), "Campo_Cod", 0, "", objParametri_Server)
            'objAppezza_W.Modifica_Parametrizzata(Piva, sa_codFrom, appezzamento("APPEZZA"), "Appezza", appezza_NEW, "", objParametri_Server)
            'objAppezza_W.Modifica_Parametrizzata(Piva, sa_codFrom, appezza_NEW, "Sa_Cod", sa_codTo, "", objParametri_Server)
            objAppezza_W.Modifica_Chiave(Piva, sa_codTo, appezza_NEW, Piva, sa_codFrom, appezzamento("appezza"), "", objParametri_Server)
            objAppezza_W.Modifica_Parametrizzata(Piva, sa_codTo, appezza_NEW, "Campo_Cod", 0, "", objParametri_Server)


            If dtUtentxapp.Rows.Count > 0 Then
                For Each utentixappezza In dtUtentxapp.Rows
                    objUtentixAppezzamenti_W.Cancella(objParametri_Server.PivaSuperUser, utentixappezza("Piva"), utentixappezza("Sa_Cod"), utentixappezza("Appezza"), "", objParametri_Server)
                    objUtentixAppezzamenti_W.Scrivi(objParametri_Server.PivaSuperUser, utentixappezza("Piva"), sa_codTo, appezza_NEW, utentixappezza("Validita_Inizio"), utentixappezza("Validita_Fine"), objParametri_Server)
                Next
            End If


            'GIS
            'Dim dtEntitaGIS = objGis_Entita_R.Leggi(objParametri_Server.PivaSuperUser, 0, 1, Piva, sa_codFrom, appezzamento("appezza"), 0, 0, "", "", "-1", -1, -1, "-1", 0, 0, 0, 0, "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Dim dtEntitaGIS = objGis_Entita_R.LeggiDB(objParametri_Server.PivaSuperUser, 0, 1, Piva, sa_codFrom, appezzamento("appezza"), 0, 0, "", "", "", 0, 0, "", 0, 0, 0, 0, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            For Each entitaGIS In dtEntitaGIS.Rows
                objGis_Entita_W.Modifica(objParametri_Server.PivaSuperUser,
                                         entitaGIS("Entita_Cod"),
                                         Nothing, Nothing,
                                         sa_codTo, appezza_NEW, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing,
                                         Nothing, Nothing, Nothing, Nothing, objParametri_Server)

            Next

            'IMPIANTI
            Dim dtImpianti = objreg_Impianti_R.Leggi(Piva, sa_codFrom, appezzamento("APPEZZA"), 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            For Each impianto In dtImpianti.Rows

                Dim Id_Reg_New = agroSeq.NuovoId_Reg_Impianti(Piva, sa_codTo, appezza_NEW, BaseCode, TopCode, objParametri_Server)

                'objreg_Impianti_W.Modifica_Parametrizzata(Piva, sa_codFrom, appezzamento("appezza"), impianto("Id_Reg"), "ID_CAMPO", 0, "", objParametri_Server)
                'objreg_Impianti_W.Modifica_Parametrizzata(Piva, sa_codFrom, appezzamento("appezza"), impianto("Id_Reg"), "sa_cod", sa_codTo, "", objParametri_Server)
                objreg_Impianti_W.Modifica_Chiave(Piva, sa_codTo, appezza_NEW, Id_Reg_New, Piva, sa_codFrom, appezzamento("appezza"), impianto("Id_Reg"), "", objParametri_Server)
                objreg_Impianti_W.Modifica_Parametrizzata(Piva, sa_codTo, appezza_NEW, Id_Reg_New, "ID_Campo", 0, "", objParametri_Server)

                'objreg_Impianti_Codici_W.Modifica_Parametrizzata(Piva, sa_codFrom, appezzamento("appezza"), impianto("id_Reg"), "sa_cod", sa_codTo, "", objParametri_Server)
                objreg_Impianti_Codici_W.Modifica_Chiave(Piva, sa_codTo, appezza_NEW, Id_Reg_New, Piva, sa_codFrom, appezzamento("appezza"), impianto("Id_Reg"), "", objParametri_Server)

                'objreg_Impianti_Programmazioni_W.Modifica_Parametrizzata(Piva, sa_codFrom, appezzamento("appezza"), impianto("id_reg"), 0, 0, 0, "sa_cod", sa_codTo, "", objParametri_Server)
                objreg_Impianti_Programmazioni_W.Modifica_Chiave(Piva, sa_codTo, appezza_NEW, Id_Reg_New, Piva, sa_codFrom, appezzamento("appezza"), impianto("Id_Reg"), "", objParametri_Server)

                'GIS
                'Dim dtEntitaGISImp = objGis_Entita_R.Leggi(objParametri_Server.PivaSuperUser, 0, 0, Piva, sa_codFrom, appezzamento("appezza"), 0, impianto("Id_Reg"), "", "", "-1", -1, -1, "-1", 0, 0, 0, 0, "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                Dim dtEntitaGISImp = objGis_Entita_R.LeggiDB(objParametri_Server.PivaSuperUser, 0, 0, Piva, sa_codFrom, appezzamento("appezza"), 0, impianto("Id_Reg"), "", "", "", 0, 0, "", 0, 0, 0, 0, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                For Each entitaGIS In dtEntitaGISImp.Rows
                    objGis_Entita_W.Modifica(objParametri_Server.PivaSuperUser,
                                         entitaGIS("Entita_Cod"),
                                         Nothing, Nothing,
                                         sa_codTo, appezza_NEW, Nothing, Id_Reg_New, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing,
                                         Nothing, Nothing, Nothing, Nothing, objParametri_Server)

                Next

                'DISTINTE
                Dim dtProgetti = objImprese_Progetti_R.Leggi(Piva, 0, "", 0, sa_codFrom, appezzamento("appezza"), impianto("id_reg"), 0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                For Each progetto In dtProgetti.Rows
                    objImprese_Progetti_W.Modifica_Chiave(Piva, sa_codTo, appezza_NEW, Id_Reg_New, progetto("progetto_cod"), Piva, sa_codFrom, appezzamento("appezza"), impianto("id_reg"), progetto("progetto_cod"), "", objParametri_Server)
                    Dim dtProgettixParticelle = objProgettixParticelle_R.Leggi(Piva, sa_codFrom, appezzamento("appezza"), impianto("id_reg"), progetto("progetto_Cod"), "", "", "", 0, 0, "", 0, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                    For Each progettoxParticella In dtProgettixParticelle.Rows
                        objProgettixParticelle_W.ModificaChiave2(Piva,
                                                                 sa_codTo,
                                                                 appezza_NEW,
                                                                 Id_Reg_New,
                                                                 progetto("Progetto_Cod"),
                                                                 progettoxParticella("PROV"),
                                                                 progettoxParticella("COM"),
                                                                 progettoxParticella("SEZIONE"),
                                                                 progettoxParticella("FOGLIO"),
                                                                 progettoxParticella("NUMERO"),
                                                                 progettoxParticella("SUBALTERNO"),
                                                                 Piva,
                                                                 sa_codFrom,
                                                                 appezzamento("appezza"),
                                                                 impianto("ID_REG"),
                                                                 progetto("Progetto_Cod"),
                                                                 progettoxParticella("PROV"),
                                                                 progettoxParticella("COM"),
                                                                 progettoxParticella("SEZIONE"),
                                                                 progettoxParticella("FOGLIO"),
                                                                 progettoxParticella("NUMERO"),
                                                                 progettoxParticella("SUBALTERNO"),
                                                                 "",
                                                                 objParametri_Server)
                    Next
                Next

            Next

        Next

    End Sub

End Class
