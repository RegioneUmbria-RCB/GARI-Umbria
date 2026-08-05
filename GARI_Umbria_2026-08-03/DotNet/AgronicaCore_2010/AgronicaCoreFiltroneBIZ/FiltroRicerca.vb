Imports System.Text
Imports System.Web
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello
Imports AgronicaCoreXML.XML_Stampe
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDTOStd.InData.FiltroRicerca
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreVarieDAL

Public Class FiltroRicerca
    Public Function usaFiltroRicercaNG(objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim utentiImpostazioniDal As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim rowsModalitaFiltroRicerca = utentiImpostazioniDal.LeggiImpostazioneScalare(enum_Impostazioni_Utenti.SUPERUSER_Mod_Filtro_Ricerca, objParametri_Utenti.UtenteUsername, objParametri_Utenti)
        Dim modalitaFiltroRicerca = rowsModalitaFiltroRicerca _
                    .Where(Function(item) Not IsDBNull(item("Impostazione_Valore_1"))) _
                    .Select(Function(item) item("Impostazione_Valore_1").ToString()) _
                    .FirstOrDefault()

        'Il filtro di ricerca NG è il default (case else)
        Select Case modalitaFiltroRicerca
            Case "1"
                Return False
            Case Else
                Return True
        End Select
    End Function

    Public Function Link_Pagina_FiltroRicercaNG(Piva As String,
                                                ParametriFiltroRicercaNG As ParametriFiltroRicercaNG) As String

        Try

            Dim objAgenda As New Parametri_ObjParametriAgenda_NG

            objAgenda.Pagina_Richiesta = enum_PagineGiasNG.Pagina_Filtro_Ricerca
            objAgenda.Pagina_Provenienza = ParametriFiltroRicercaNG.PaginaProvenienza
            objAgenda.Sito_Provenienza = ParametriFiltroRicercaNG.SitoOrigine
            objAgenda.Piva = ParametriFiltroRicercaNG.Piva
            objAgenda.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
            objAgenda.GenericObj_string = JsonConvert.SerializeObject(ParametriFiltroRicercaNG)
            objAgenda.QueryStringFiltrino = "?seFrame=1"

            objAgenda.Salva()

            Return RedirectGestione.IndirizzoCompleto_SitoGiasNG_PassandoDirettamente_ParametriAgenda_NG(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgenda)

        Catch ex As Exception

            Throw New Exception("Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True))

        End Try

    End Function

#Region "METODI PORTED DA filtrone_nuovo.aspx"
    Public Function Gestisci_Redirect_Stampe(tipoMostra As Enum_TipoMostra_FiltroRicerca,
                                             codificaStampe As enum_CodificaStampe,
                                             chiavi As List(Of String),
                                             datiStampe As DatiStampe,
                                             objParametri_Server As AgronicaCoreParametri) As String

        Dim objAgronicaStampe As New ParametriAgronicaStampe

        Dim StrSelezionati As String = ""
        Dim StrVariabiliStampe As String = ""

        Dim strFiltro As String = ""
        Dim xmlFiltro As String = ""

        Dim XmlDoc As New XmlDocument
        Dim Xml_FiltroStampa As XmlElement

        objAgronicaStampe.report = codificaStampe

        Select Case codificaStampe
            Case enum_CodificaStampe.SchedaCampagna_2078,
                 enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
                 enum_CodificaStampe.RegistroTrattamenti,
                 enum_CodificaStampe.RegistroTrattamenti_Semplificata,
                 enum_CodificaStampe.SchedaRegistrazione,
                 enum_CodificaStampe.SchedaRegistrazione_Semplificata,
                 enum_CodificaStampe.SchedaCampagna_Biologico,
                 enum_CodificaStampe.SchedaCampagna_Biologico_Semplificata,
                 enum_CodificaStampe.Eurep_Gap,
                 enum_CodificaStampe.Eurep_Gap_Semplificata,
                 enum_CodificaStampe.Eurep_Gap_Multicentro,
                 enum_CodificaStampe.SchedaCampagna_ProvAut_Trento,
                 enum_CodificaStampe.SchedaColturale_Biologico,
                 enum_CodificaStampe.SchedaTracciabilita,
                 enum_CodificaStampe.SchedaCampagna_ConserveItalia,
                 enum_CodificaStampe.DatiAnelloFilieraIngresso,
                 enum_CodificaStampe.DatiAnelloFilieraLegameLotti,
                 enum_CodificaStampe.SchedaCampagna_Pizzoli,
                 enum_CodificaStampe.Registro_Fertilizzazioni,
                 enum_CodificaStampe.RegistroTrattamenti_Veneto,
                 enum_CodificaStampe.SchedaCampagna_Multicentro,
                 enum_CodificaStampe.SchedaCampagna_Multicentro_ACA,
                 enum_CodificaStampe.Scheda_Rilievi,
                 enum_CodificaStampe.PianoColturale,
                 enum_CodificaStampe.PianoColturaleCatasto,
                 enum_CodificaStampe.PianoColturaleCatastoGrid,
                 enum_CodificaStampe.SchedaCampagna_Multi_Lombardia,
                 enum_CodificaStampe.SchedaInterventiAgronomici,
                 enum_CodificaStampe.RegistroAziendaleUnico,
                 enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then
                    StrSelezionati = getStringaXml_PIVA_SA_COD_APPEZZA_ID_REG_VEG_COD_from_richiesta(chiavi)
                    Xml_FiltroStampa = XmlDoc.CreateElement("FiltroStampa")
                    XmlDoc.AppendChild(Xml_FiltroStampa)
                    '  Galassi, 16/09/2016 10:03:32: Per nuove stampe con selezione specie nel filtro (Fede) bisogna
                    '  specificare se usare tutti gli impianti della specie o UtilizzaImpiantiFiltrati ;)
                    Xml_FiltroStampa.InnerXml = StrSelezionati
                    '  Galassi, 19/09/2016 11:16:35: Riproviamo con un altro tag invece che l'attributo commentato sopra
                    ' perchè il filtrone del 2003 non ha "FiltroStampa" come TAG
                    Dim Xml_FitraImpianti As XmlElement
                    Xml_FitraImpianti = XmlDoc.CreateElement("FiltraImpianti")
                    Xml_FitraImpianti.SetAttribute("UtilizzaImpiantiFiltrati", 1)
                    Xml_FiltroStampa.AppendChild(Xml_FitraImpianti)
                    StrVariabiliStampe = XmlDoc.InnerXml
                End If

            Case enum_CodificaStampe.Registro_Fertilizzazioni_Massivo,
                 enum_CodificaStampe.Registro_Trattamenti_Massivo
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then
                    StrSelezionati = getStringaXml_PIVA_from_richiesta(chiavi)
                    Xml_FiltroStampa = XmlDoc.CreateElement("FiltroStampa")
                    XmlDoc.AppendChild(Xml_FiltroStampa)
                    Xml_FiltroStampa.InnerXml = StrSelezionati
                    StrVariabiliStampe = XmlDoc.InnerXml
                End If

            Case enum_CodificaStampe.EstrattoreDatiGrafici
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then
                    StrSelezionati = getStringaXml_PIVA_SA_COD_from_richiesta(chiavi)
                    Xml_FiltroStampa = XmlDoc.CreateElement("FiltroStampa")
                    XmlDoc.AppendChild(Xml_FiltroStampa)
                    Xml_FiltroStampa.InnerXml = StrSelezionati
                    StrVariabiliStampe = XmlDoc.InnerXml
                End If

            Case enum_CodificaStampe.Esportatore_Universale_Impianti,
                 enum_CodificaStampe.SchedaCatastoeUtilizzi,
                 enum_CodificaStampe.EsportazionePomodoroIndustriaOINordItalia
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then
                    StrSelezionati = getStringaXml_PIVA_SA_COD_APPEZZA_ID_REG_from_richiesta_Esporta_Impianti(chiavi)
                    Xml_FiltroStampa = XmlDoc.CreateElement("FiltroStampa")
                    XmlDoc.AppendChild(Xml_FiltroStampa)
                    Xml_FiltroStampa.InnerXml = StrSelezionati
                    StrVariabiliStampe = XmlDoc.InnerXml
                End If

            Case enum_CodificaStampe.Esportatore_Universale_Centri
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then
                    StrSelezionati = getStringaXml_PIVA_SA_COD_from_richiesta(chiavi)
                    Xml_FiltroStampa = XmlDoc.CreateElement("FiltroStampa")
                    XmlDoc.AppendChild(Xml_FiltroStampa)
                    Xml_FiltroStampa.InnerXml = StrSelezionati
                    StrVariabiliStampe = XmlDoc.InnerXml
                End If

            Case enum_CodificaStampe.Esportatore_Universale_Imprese
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then
                    StrSelezionati = getStringaXml_PIVA_SA_COD_APPEZZA_ID_REG_from_richiesta_Esporta_Imprese(chiavi)
                    Xml_FiltroStampa = XmlDoc.CreateElement("FiltroStampa")
                    XmlDoc.AppendChild(Xml_FiltroStampa)
                    Xml_FiltroStampa.InnerXml = StrSelezionati
                    StrVariabiliStampe = XmlDoc.InnerXml
                End If

            Case enum_CodificaStampe.Esportatore_Universale_Agenda
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then
                    '29/01/2020: sistemato, passava la chiave impianti anzichè agenda!
                    '(chissà da quanti millenni non funzionava)
                    'StrSelezionati = getStringaXml_PIVA_SA_COD_APPEZZA_ID_REG_from_richiesta_Esporta_Agenda(chiavi)
                    StrSelezionati = getStringaXml_PIVA_SA_COD_IDAGENDA_from_richiesta_Esporta_Agenda(chiavi)
                    Xml_FiltroStampa = XmlDoc.CreateElement("FiltroStampa")
                    XmlDoc.AppendChild(Xml_FiltroStampa)
                    Xml_FiltroStampa.InnerXml = StrSelezionati
                    StrVariabiliStampe = XmlDoc.InnerXml
                End If

            Case enum_CodificaStampe.Quadro_P,
                 enum_CodificaStampe.Esportazione_AnagraficaContatti,
                 enum_CodificaStampe.Esportazione_CellulariContatti
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then
                    'controllo se ho selezionato Aziende o Centri
                    If chiavi(0).Split("_").Length = 1 Then
                        'piva 
                        If enum_CodificaStampe.Esportazione_AnagraficaContatti = codificaStampe Then
                            StrSelezionati = getStringaXml_PIVA_from_richiesta_P(chiavi)
                        Else
                            StrSelezionati = getStringaXml_PIVA_from_richiesta(chiavi)
                        End If
                    Else
                        'piva 'sa cod
                        StrSelezionati = getStringaXml_PIVA_SA_COD_from_richiesta(chiavi)
                    End If

                    'XmlDoc.InnerXml = StrSelezionati
                    If enum_CodificaStampe.Quadro_P = codificaStampe Then
                        XmlDoc.InnerXml = StrSelezionati
                        StrVariabiliStampe = XmlDoc.InnerXml
                    Else
                        Xml_FiltroStampa = XmlDoc.CreateElement("FiltroStampa")
                        XmlDoc.AppendChild(Xml_FiltroStampa)
                        Xml_FiltroStampa.InnerXml = StrSelezionati

                        StrVariabiliStampe = XmlDoc.InnerXml
                    End If
                End If

                StrSelezionati = ""

            Case enum_CodificaStampe.Esportazione_OP_Produttori
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then

                    Dim objVS As New AgronicaCoreXML.XML_Stampe

                    StrSelezionati = getStringaXml_PIVA_from_richiesta(chiavi)

                    Xml_FiltroStampa = XmlDoc.CreateElement("FiltroStampa")
                    XmlDoc.AppendChild(Xml_FiltroStampa)
                    Xml_FiltroStampa.InnerXml = StrSelezionati
                    StrVariabiliStampe = XmlDoc.InnerXml
                End If

            Case enum_CodificaStampe.Esportazione_OP_Catasto
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then
                    StrSelezionati = getStringaXml_PIVA_SA_COD_APPEZZA_ID_REG_from_richiesta(chiavi)
                    Xml_FiltroStampa = XmlDoc.CreateElement("FiltroStampa")
                    XmlDoc.AppendChild(Xml_FiltroStampa)
                    Xml_FiltroStampa.InnerXml = StrSelezionati
                    StrVariabiliStampe = XmlDoc.InnerXml
                End If

                StrSelezionati = ""

            Case enum_CodificaStampe.RiepilogoImpiegoSuperfici_Multiazienda
                'TODO
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then

                    XmlDoc = getStringaXml_PIVA_SA_COD_APPEZZA_ID_REG_VEG_COD_from_richiesta_xdoc(XmlDoc, chiavi)

                    Dim XML_VariabiliStampe As XmlElement
                    XML_VariabiliStampe = XmlDoc.SelectSingleNode("VariabiliStampe")

                    XML_VariabiliStampe.SetAttribute("username", CStr(HttpContext.Current.Session("ASG_Utente_Username")))
                    XML_VariabiliStampe.SetAttribute("codice_report", codificaStampe)
                    XML_VariabiliStampe.SetAttribute("user_profilo", CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS")))

                    StrVariabiliStampe = XmlDoc.InnerXml

                End If

            Case enum_CodificaStampe.Bilancio_Fertilizzazioni

                Select Case tipoMostra
                    Case Enum_TipoMostra_FiltroRicerca.Impianti
                        XmlDoc = getStringaXml_PIVA_SA_COD_APPEZZA_ID_REG_VEG_COD_TOT_from_richiesta_xdoc(XmlDoc, chiavi, objParametri_Server)
                End Select

                Dim XML_VariabiliStampe As XmlElement
                XML_VariabiliStampe = XmlDoc.SelectSingleNode("Pippo")

                StrVariabiliStampe = XML_VariabiliStampe.OuterXml

            Case enum_CodificaStampe.Bilancio_Fertilizzazioni_Dettagliato
                Select Case tipoMostra
                    Case Enum_TipoMostra_FiltroRicerca.Impianti
                        XmlDoc = getStringaXml_PIVA_SA_COD_APPEZZA_ID_REG_VEG_COD_TOT_from_richiesta_xdoc(XmlDoc, chiavi, objParametri_Server)
                    Case Enum_TipoMostra_FiltroRicerca.Movimenti
                        XmlDoc = getStringaXml_PIVA_SA_COD_IDAGENDA_from_richiesta_Esporta_Agenda_xdoc(XmlDoc, chiavi)
                End Select

                Dim XML_VariabiliStampe As XmlElement
                XML_VariabiliStampe = XmlDoc.SelectSingleNode("Pippo")

                StrVariabiliStampe = XML_VariabiliStampe.OuterXml

            Case enum_CodificaStampe.ReportConserveItalia
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then
                    StrSelezionati = getStringaXml_PIVA_SA_COD_APPEZZA_ID_REG_from_richiesta(chiavi)
                    Xml_FiltroStampa = XmlDoc.CreateElement("FiltroStampa")
                    XmlDoc.AppendChild(Xml_FiltroStampa)
                    Xml_FiltroStampa.InnerXml = StrSelezionati
                    StrVariabiliStampe = XmlDoc.InnerXml
                End If

            Case enum_CodificaStampe.Atto_Notorio, ' Schede OP che accettano Impianti
                 enum_CodificaStampe.Atto_Notorio * -1, ' Atto notorio con riepilogo catasto
                 enum_CodificaStampe.Allegato_CatastoeValorizzazioni,
                 enum_CodificaStampe.SchedaAziendale 'Accetta anche solo piva
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then
                    Dim piva = chiavi(0).Split("_")(0)
                    Dim stampaVuota As Boolean = False
                    Dim fronteRetro As Boolean = False
                    Dim anno = AGRODATAINIZIO.Year.ToString
                    Dim objAnagDal = New AgronicaCoreAnagrafeDAL.Imprese_Read
                    Dim rag_soc = objAnagDal.RagSoc_from_Piva(piva, objParametri_Server)
                    Dim tipoSelezione = "azienda"

                    If Not IsNothing(datiStampe) Then
                        anno = datiStampe.anno
                    End If

                    'If Not IsNothing(datiStampe) Then
                    '    stampaVuota = datiStampe.switchGenerale1
                    'End If

                    If Not IsNothing(datiStampe) Then
                        fronteRetro = datiStampe.switchGenerale1
                    End If

                    If tipoMostra = Enum_TipoMostra_FiltroRicerca.Aziende Then
                        tipoSelezione = "azienda"
                        strFiltro = chiavi.
                                        Aggregate(New StringBuilder(" AND ("),
                                                  Function(stb, c) stb.AppendFormat(" ( Imprese.PIVA = '{0}' )  OR", c),
                                                  Function(stb) stb.Remove(stb.Length - 3, 3).Append(" )").ToString)
                    Else
                        tipoSelezione = "impianto"
                        strFiltro = chiavi.
                                        Select(Of List(Of String))(Function(c) c.Split("_").ToList).
                                        Aggregate(New StringBuilder(" AND ("),
                                                  Function(stb, c) stb.AppendFormat(" ( Reg_Impianti.PIVA = '{0}' AND  Reg_Impianti.SA_COD = {1} AND Reg_Impianti.APPEZZA = {2} AND Reg_Impianti.ID_REG = {3})  OR", c(0), c(1), c(2), c(3)),
                                                  Function(stb) stb.Remove(stb.Length - 3, 3).Append(" )").ToString)
                    End If

                    Dim xml_dati_schedeOP = XmlDoc.CreateElement("DatiSchedeOP")

                    XmlDoc.AppendChild(xml_dati_schedeOP)
                    xml_dati_schedeOP.InnerXml = ottieniSchedeDatiOP(piva, rag_soc, anno, codificaStampe, tipoSelezione, chiavi, objParametri_Server, stampaVuota:=stampaVuota, fronteRetro:=fronteRetro)
                    StrVariabiliStampe = XmlDoc.InnerXml

                End If

            Case enum_CodificaStampe.Impegnativa_Orticole_Gest_Annuale, ' Schede OP che accettano Impianti ma vogliono anche la descrizione dei vari impianti
                 enum_CodificaStampe.Impegnativa_Orticole_Gest_Breve,
                 enum_CodificaStampe.Impegnativa_Fagiolino_Mercato_Fresco,
                 enum_CodificaStampe.Impegnativa_Orticole_Industria,
                 enum_CodificaStampe.Impegnativa_Pomodoro_Industria,
                 enum_CodificaStampe.Dichiarazione_di_Responsabilita, ' Accetta anche solo piva, gestita con l'if
                 enum_CodificaStampe.Fitoregolatori_Kiwi ' Come la stampa Dichiarazione_di_Responsabilita sopra
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then
                    Dim tipoSelezione = "azienda"
                    Dim piva = chiavi(0).Split("_")(0)
                    Dim fronteRetro As Boolean = False
                    Dim anno = AGRODATAINIZIO.Year.ToString
                    Dim objAnagDal = New AgronicaCoreAnagrafeDAL.Imprese_Read
                    Dim rag_soc = objAnagDal.RagSoc_from_Piva(piva, objParametri_Server)
                    Dim sa_cod = ""

                    If Not IsNothing(datiStampe) Then
                        anno = datiStampe.anno
                    End If

                    If Not IsNothing(datiStampe) Then
                        fronteRetro = datiStampe.switchGenerale1
                    End If

                    If tipoMostra = Enum_TipoMostra_FiltroRicerca.Aziende Then
                        tipoSelezione = "azienda"
                        strFiltro = chiavi.
                                            Aggregate(New StringBuilder(" AND ("),
                                                      Function(stb, c) stb.AppendFormat(" ( Imprese.PIVA = '{0}' )  OR", c),
                                                      Function(stb) stb.Remove(stb.Length - 3, 3).Append(" )").ToString)
                    Else
                        tipoSelezione = "impianto"
                        sa_cod = chiavi(0).Split("_")(1)
                        strFiltro = chiavi.Select(Of List(Of String))(Function(c) c.Split("_").ToList).Aggregate(New StringBuilder("  AND ("), Function(stb, c) stb.AppendFormat(" ( Reg_Impianti.PIVA = '{0}' AND  Reg_Impianti.SA_COD = {1} AND Reg_Impianti.APPEZZA = {2} AND Reg_Impianti.ID_REG = {3})  OR", c(0), c(1), c(2), c(3)), Function(stb) stb.Remove(stb.Length - 3, 3).Append(" )").ToString)
                    End If

                    Dim xml_dati_schedeOP = XmlDoc.CreateElement("DatiSchedeOP")

                    XmlDoc.AppendChild(xml_dati_schedeOP)
                    xml_dati_schedeOP.InnerXml = ottieniSchedeDatiOP(piva, rag_soc, anno, codificaStampe, tipoSelezione, chiavi, objParametri_Server, sa_cod:=sa_cod, filtroSql:=strFiltro, fronteRetro:=fronteRetro)
                    StrVariabiliStampe = XmlDoc.InnerXml
                End If

            Case enum_CodificaStampe.Adesione_Etico_Ambientale, 'Schede op che vogliono solo la piva
                 enum_CodificaStampe.Tenuta_Scheda_Campagna,
                 enum_CodificaStampe.Codice_Condotta,
                 enum_CodificaStampe.Adesione_DPI,
                 enum_CodificaStampe.Impegnativa_Eurep,
                 enum_CodificaStampe.Impegnativa_QC,
                 enum_CodificaStampe.Impegnativa_Confusione_Sessuale,
                 enum_CodificaStampe.Mandato_Trasmissione_Telematica_Dati,
                 enum_CodificaStampe.Adesione_ModuloGrasp, 'new ones from here
                 enum_CodificaStampe.Adesione_ProtocolloGlobalGAP,
                 enum_CodificaStampe.Adesione_NurtureModule,
                 enum_CodificaStampe.Adesione_Conad,
                 enum_CodificaStampe.Adesione_Despar,
                 enum_CodificaStampe.Adesione_StandardLeaf,
                 enum_CodificaStampe.Accordo_Responsabilita_di_Filiera,
                 enum_CodificaStampe.ImpegnoProduzioneSociDivisoxCentri, 'Accetta sia solo piva che impianti
                 enum_CodificaStampe.QuestionarioValutazioneAzienda_Aggiornamento
                'Senza impianto 
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then
                    Dim piva = chiavi(0).Split("_")(0)
                    Dim anno = AGRODATAINIZIO.Year.ToString
                    Dim fronteRetro As Boolean = False
                    Dim objAnagDal = New AgronicaCoreAnagrafeDAL.Imprese_Read
                    Dim rag_soc = objAnagDal.RagSoc_from_Piva(piva, objParametri_Server)
                    Dim tipoSelezione = "azienda"

                    Dim xml_dati_schedeOP = XmlDoc.CreateElement("DatiSchedeOP")

                    If Not IsNothing(datiStampe) Then
                        anno = datiStampe.anno
                    End If

                    If Not IsNothing(datiStampe) Then
                        fronteRetro = datiStampe.switchGenerale1
                    End If


                    If tipoMostra <> Enum_TipoMostra_FiltroRicerca.Aziende AndAlso
                        (codificaStampe <> enum_CodificaStampe.Adesione_Conad AndAlso codificaStampe <> enum_CodificaStampe.Adesione_Despar) Then 'Gestisco caso in cui seleziono impianti

                        tipoSelezione = "impianto"
                        strFiltro = chiavi.
                                        Select(Of List(Of String))(Function(c) c.Split("_").ToList).
                                        Aggregate(New StringBuilder(" AND ("),
                                                  Function(stb, c) stb.AppendFormat(" ( Reg_Impianti.PIVA = '{0}' AND  Reg_Impianti.SA_COD = {1} AND Reg_Impianti.APPEZZA = {2} AND Reg_Impianti.ID_REG = {3})  OR", c(0), c(1), c(2), c(3)),
                                                  Function(stb) stb.Remove(stb.Length - 3, 3).Append(" )").ToString)
                    ElseIf chiavi.Count > 0 AndAlso
                        (tipoMostra = Enum_TipoMostra_FiltroRicerca.Aziende OrElse
                        codificaStampe = enum_CodificaStampe.Adesione_Conad OrElse
                        codificaStampe = enum_CodificaStampe.Adesione_Despar) Then ' Caso in cui seleziono piu aziende, compreso il caso per conad e despar in cui posso selezionare anche per esercizi ma alla stampa interessa solo le piva

                        tipoSelezione = "azienda"
                        strFiltro = chiavi.
                                        Select(Function(s) s.Split("_")).
                                        Aggregate(New StringBuilder(" AND ("),
                                                      Function(stb, c) stb.AppendFormat(" ( Imprese.PIVA = '{0}' )  OR", c(0)),
                                                      Function(stb) stb.Remove(stb.Length - 3, 3).Append(" )").ToString)
                    End If

                    XmlDoc.AppendChild(xml_dati_schedeOP)
                    xml_dati_schedeOP.InnerXml = ottieniSchedeDatiOP(piva, rag_soc, anno, codificaStampe, tipoSelezione, chiavi, objParametri_Server, filtroSql:=strFiltro, fronteRetro:=fronteRetro)
                    StrVariabiliStampe = XmlDoc.InnerXml
                End If

            Case enum_CodificaStampe.ImpegnativaColtivazioneConferimento
                Dim tipoSelezione = "azienda"
                Dim stampaVuota As Boolean = False
                Dim fronteRetro As Boolean = False

                If Not IsNothing(datiStampe) Then
                    stampaVuota = datiStampe.switchGenerale1
                End If

                If Not IsNothing(datiStampe) Then
                    fronteRetro = datiStampe.switchGenerale2
                End If

                Dim piva = chiavi(0).Split("_")(0)
                Dim anno = AGRODATAINIZIO.Year.ToString
                Dim objAnagDal = New AgronicaCoreAnagrafeDAL.Imprese_Read
                Dim rag_soc = objAnagDal.RagSoc_from_Piva(piva, objParametri_Server)

                If Not stampaVuota AndAlso chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then
                    tipoSelezione = "impianto"
                    strFiltro = chiavi.Select(Of List(Of String))(Function(c) c.Split("_").ToList).Aggregate(New StringBuilder(" AND ("), Function(stb, c) stb.AppendFormat(" ( Reg_Impianti.PIVA = '{0}' AND  Reg_Impianti.SA_COD = {1} AND Reg_Impianti.APPEZZA = {2} AND Reg_Impianti.ID_REG = {3})  OR", c(0), c(1), c(2), c(3)), Function(stb) stb.Remove(stb.Length - 3, 3).Append(" )").ToString)
                    If Not IsNothing(datiStampe) Then
                        anno = datiStampe.anno
                    End If
                ElseIf chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then

                    tipoSelezione = "azienda"
                    strFiltro = chiavi.
                                    Select(Function(s) s.Split("_")).
                                    Aggregate(New StringBuilder(" AND ("),
                                                  Function(stb, c) stb.AppendFormat(" ( Imprese.PIVA = '{0}' )  OR", c(0)),
                                                  Function(stb) stb.Remove(stb.Length - 3, 3).Append(" )").ToString)

                End If

                Dim xml_dati_schedeOP = XmlDoc.CreateElement("DatiSchedeOP")

                XmlDoc.AppendChild(xml_dati_schedeOP)

                xml_dati_schedeOP.InnerXml = ottieniSchedeDatiOP(piva, rag_soc, anno, codificaStampe, tipoSelezione, chiavi, objParametri_Server, stampaVuota:=stampaVuota, fronteRetro:=fronteRetro)
                StrVariabiliStampe = XmlDoc.InnerXml

            Case enum_CodificaStampe.ObiettivoDiProduzioneAsipo
                If chiavi IsNot Nothing AndAlso chiavi.Count > 0 Then
                    Dim piva = chiavi(0).Split("_")(0)
                    Dim objAnagDal = New AgronicaCoreAnagrafeDAL.Imprese_Read

                    Dim xmL_dati = XmlDoc.CreateElement("Dati")
                    XmlDoc.AppendChild(xmL_dati)
                    Dim objVS As New AgronicaCoreXML.XML_Stampe
                    Dim vVarStampe(2) As ElementoStampe

                    vVarStampe(0).Nome = "piva"
                    vVarStampe(0).Valore = piva
                    vVarStampe(1).Nome = "dataInizio"
                    vVarStampe(1).Valore = datiStampe.dataInizio
                    vVarStampe(2).Nome = "dataFine"
                    vVarStampe(2).Valore = datiStampe.dataFine

                    xmL_dati.InnerXml = objVS.XML_VariabiliStampe(vVarStampe)
                    StrVariabiliStampe = XmlDoc.InnerXml

                End If

        End Select


        Dim link = RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamenteIParametri(
                                    Enum_SiteRedirector.Sito_AgronicaStampe_2010,
                                    codificaStampe,
                                    CStr(HttpContext.Current.Session("ASG_Utente_Username")),
                                    CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS")),
                                    StrVariabiliStampe,
                                    "",
                                    strFiltro,
                                    xmlFiltro,
                                    "",
                                    "",
                                    "",
                                    "")

        Return link

    End Function

    Private Shared Function getSessionDataOrDefault(Of T)(chiave As String, defaultValue As T) As T
        Dim value As T = defaultValue
        If Not IsNothing(HttpContext.Current.Session(chiave)) Then
            value = HttpContext.Current.Session(chiave)
        End If

        Return value

    End Function

    Private Shared Function GetIDTestataForJoin(chiavi As List(Of String), tipoSelezione As String, objParametri_Server As AgronicaCoreParametri) As Integer

        Dim impianti = chiavi.
            Select(Function(s) s.Split("_").ToList).
            Select(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)(Function(list)
                                                                             Return New AgronicaCoreModello.ParametriAgenda_Temp.Impianto With {
                                        .Piva = list(0),
                                        .Sa_Cod = If(tipoSelezione = "azienda", 0, list(1)),
                                        .Appezza = If(tipoSelezione = "azienda", 0, list(2)),
                                        .ID_Reg = If(tipoSelezione = "azienda", 0, list(3))
                                    }
                                                                         End Function).ToList

        Dim xAgrosequenze As New Agro_Sequenze
        Dim IDTestataTemp = xAgrosequenze.NuovoId_Tabella("__tmp_FiltroImpianti", 0, CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)

        Dim OperazioneCorrente_FiltroImpianti As String = ""

        OperazioneCorrente_EstraiFiltrone(IDTestataTemp, impianti, OperazioneCorrente_FiltroImpianti)
        PopolaTabellaFiltroImpianti(OperazioneCorrente_FiltroImpianti, objParametri_Server)

        Return IDTestataTemp

    End Function

    Public Shared Sub OperazioneCorrente_EstraiFiltrone(ByVal IDTestataTemp As Integer,
                                                        ByVal Impianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto),
                                                        ByRef OperazioneCorrente_FiltroImpianti As String,
                                                            Optional ByVal objParametri As AgronicaCoreParametri = Nothing
                                                        )

        Dim listImp As New List(Of String)

        OperazioneCorrente_EstraiDAL_FiltroImpianti(IDTestataTemp, Impianti, listImp, objParametri)

        OperazioneCorrente_FiltroImpianti = String.Join("|", listImp.ToArray)

    End Sub

    Public Shared Sub PopolaTabellaFiltroImpianti(operazioneCorrente_FiltroImpianti As String, ByVal objParametri As AgronicaCoreParametri)

        Dim vQry As String() = operazioneCorrente_FiltroImpianti.Split("|")

        Dim scriviImpianti As New DataProvider

        For Each stmt In vQry
            Dim rVal As Boolean = scriviImpianti.EseguiQuery_Scrittura(objParametri, stmt, "PopolaTabellaFiltroImpianti")
        Next

    End Sub

    Public Shared Sub OperazioneCorrente_EstraiDAL_FiltroImpianti(ByVal IDTestataTemp As Integer, ByVal dest As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto), ByRef listImp As List(Of String), Optional ByVal objParametri As AgronicaCoreParametri = Nothing)
        Dim hs As New Hashtable

        Dim listImp1 As New List(Of String)

        Dim bUsername As Boolean = objParametri IsNot Nothing

        Dim contatoreDati As Integer = 1

        For Each iDest In dest

            Dim chiaveHash As String = "('" & iDest.Piva & "'," & iDest.Sa_Cod & "," & iDest.Appezza & "," & iDest.ID_Reg & "," & IDTestataTemp

            If objParametri IsNot Nothing Then
                chiaveHash &= ", '" & objParametri.UtenteUsername & "', '" & objParametri.UtenteUsername & "'"
            End If

            chiaveHash &= ")"

            If Not hs.ContainsKey(chiaveHash) Then

                hs.Add(chiaveHash, iDest)

                listImp1.Add(chiaveHash)


                If contatoreDati Mod 1000 = 0 Then
                    popolaConImpianti(listImp, listImp1, bUsername)
                    listImp1.Clear()
                End If

            Else
                'xdebug.
                Dim idle As Integer = 0
            End If

            contatoreDati += 1
        Next

        If dest.Count Mod 1000 <> 0 Then
            popolaConImpianti(listImp, listImp1, bUsername)
        End If
    End Sub

    Private Shared Sub popolaConImpianti(listImp As List(Of String), listImp1 As List(Of String), ByVal bUsername As Boolean)
        Dim sInsertStmt2 As New StringBuilder

        Dim line As String = " INSERT INTO __tmp_FiltroImpianti (piva, sa_cod, appezza, id_reg, IDTestataTemp"

        If bUsername Then
            line &= ", Username_Creazione, Username_Modifica"
        End If

        line &= ") VALUES "

        sInsertStmt2.AppendLine(line)

        sInsertStmt2.Append(String.Join(",", listImp1))

        listImp.Add(sInsertStmt2.ToString)
    End Sub

#Region "XML STAMPA"
    Public Shared Function getStringaXml_PIVA_SA_COD_APPEZZA_ID_REG_from_richiesta_Esporta_Impianti(ByVal Chiavi As List(Of String))
        Dim StrSelezionati As String = ""
        Dim StrNodo As String = ""
        Dim objVS As New AgronicaCoreXML.XML_Stampe

        For i = 0 To Chiavi.Count - 1

            Dim vVarStampe(3) As ElementoStampe
            vVarStampe(0).Nome = "p"
            vVarStampe(0).Valore = Chiavi(i).Split("_")(0)
            vVarStampe(1).Nome = "s"
            vVarStampe(1).Valore = Chiavi(i).Split("_")(1)
            vVarStampe(2).Nome = "a"
            vVarStampe(2).Valore = Chiavi(i).Split("_")(2)
            vVarStampe(3).Nome = "r"
            vVarStampe(3).Valore = Chiavi(i).Split("_")(3)

            StrNodo = objVS.XML_VariabiliStampe(vVarStampe)

            StrSelezionati &= StrNodo
        Next

        Return StrSelezionati

    End Function

    Public Shared Function getStringaXml_PIVA_SA_COD_APPEZZA_ID_REG_VEG_COD_TOT_from_richiesta_xdoc(ByVal XmlDoc As XmlDocument, Chiavi As List(Of String), objParametri_Server As AgronicaCoreParametri)
        Dim StrSelezionati As String = ""
        Dim StrNodo As String = ""
        Dim objVS As New AgronicaCoreXML.XML_Stampe

        Dim Xml_FiltroStampa = XmlDoc.CreateElement("Pippo")
        XmlDoc.AppendChild(Xml_FiltroStampa)

        Dim dt_dett As DataTable
        Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        For i = 0 To Chiavi.Count - 1
            Dim vVarStampe(21) As ElementoStampe

            vVarStampe(0).Nome = "piva"
            vVarStampe(0).Valore = Chiavi(i).Split("_")(0)
            vVarStampe(1).Nome = "sa_cod"
            vVarStampe(1).Valore = Chiavi(i).Split("_")(1)
            vVarStampe(2).Nome = "appezza"
            vVarStampe(2).Valore = Chiavi(i).Split("_")(2)
            vVarStampe(3).Nome = "id_reg"
            vVarStampe(3).Valore = Chiavi(i).Split("_")(3)
            vVarStampe(4).Nome = "veg_cod"
            vVarStampe(4).Valore = Chiavi(i).Split("_")(4)

            dt_dett = objImp.Leggi_Dati_Impianti_piu_distinta(Chiavi(i).Split("_")(0), Chiavi(i).Split("_")(1), Chiavi(i).Split("_")(2),
                                                              Chiavi(i).Split("_")(3),
                                                              Chiavi(i).Split("_")(5), objParametri_Server)

            vVarStampe(5).Nome = "rag_soc"
            vVarStampe(5).Valore = dt_dett.Rows(0).Item("rag_soc")

            vVarStampe(6).Nome = "sa_nome"
            vVarStampe(6).Valore = dt_dett.Rows(0).Item("sa_nome")

            vVarStampe(7).Nome = "app_nome"
            vVarStampe(7).Valore = dt_dett.Rows(0).Item("app_nome")

            vVarStampe(8).Nome = "sup_imp"
            vVarStampe(8).Valore = dt_dett.Rows(0).Item("sup_imp")

            vVarStampe(9).Nome = "progetto_cod"
            vVarStampe(9).Valore = Chiavi(i).Split("_")(5)

            vVarStampe(10).Nome = "veg_des"
            vVarStampe(10).Valore = ""
            If Not IsDBNull(dt_dett.Rows(0).Item("veg_des")) AndAlso dt_dett.Rows(0).Item("veg_des").ToString <> "" Then
                vVarStampe(10).Valore = dt_dett.Rows(0).Item("veg_des")
            ElseIf Not IsDBNull(dt_dett.Rows(0).Item("DestinazioneUso")) AndAlso dt_dett.Rows(0).Item("DestinazioneUso").ToString <> "" Then
                vVarStampe(10).Valore = dt_dett.Rows(0).Item("DestinazioneUso")
            End If

            vVarStampe(11).Nome = "cul_des"
            vVarStampe(11).Valore = ""
            If Not IsDBNull(dt_dett.Rows(0).Item("cul_des")) AndAlso dt_dett.Rows(0).Item("cul_des").ToString <> "" Then
                vVarStampe(11).Valore = dt_dett.Rows(0).Item("cul_des")
            End If

            vVarStampe(12).Nome = "progetto_validita_inizio"
            vVarStampe(12).Valore = dt_dett.Rows(0).Item("validita_inizio")

            vVarStampe(13).Nome = "progetto_validita_fine"
            vVarStampe(13).Valore = dt_dett.Rows(0).Item("validita_fine")

            vVarStampe(14).Nome = "impianto_validita_inizio"
            vVarStampe(14).Valore = dt_dett.Rows(0).Item("validita_inizio_impianto")
            vVarStampe(15).Nome = "impianto_validita_fine"
            vVarStampe(15).Valore = dt_dett.Rows(0).Item("validita_fine_impianto")

            vVarStampe(16).Nome = "grfi_cod"
            vVarStampe(16).Valore = 0
            If Not IsDBNull(dt_dett.Rows(0).Item("grfi_cod")) AndAlso IsNumeric(dt_dett.Rows(0).Item("grfi_cod")) Then
                vVarStampe(16).Valore = dt_dett.Rows(0).Item("grfi_cod")
            End If
            vVarStampe(17).Nome = "grfi_des"
            vVarStampe(17).Valore = dt_dett.Rows(0).Item("grfi_des")

            vVarStampe(18).Nome = "regolamento_cod"
            vVarStampe(18).Valore = 1
            If Not IsDBNull(dt_dett.Rows(0).Item("regolamento_cod")) AndAlso IsNumeric(dt_dett.Rows(0).Item("regolamento_cod")) Then
                vVarStampe(18).Valore = dt_dett.Rows(0).Item("regolamento_cod")
            End If
            vVarStampe(19).Nome = "reg_des"
            vVarStampe(19).Valore = dt_dett.Rows(0).Item("reg_des")

            vVarStampe(20).Nome = "campo_cod"
            vVarStampe(20).Valore = dt_dett.Rows(0).Item("campo_cod")
            vVarStampe(21).Nome = "campo_des"
            vVarStampe(21).Valore = dt_dett.Rows(0).Item("campo_des")

            objVS.CreaInserisci_SottoNodo_XML_VariabiliStampe(XmlDoc, vVarStampe)

        Next

        Return XmlDoc

    End Function

    Public Shared Function getStringaXml_PIVA_SA_COD_IDAGENDA_from_richiesta_Esporta_Agenda_xdoc(ByVal XmlDoc As XmlDocument, Chiavi As List(Of String))
        Dim StrSelezionati As String = ""
        Dim StrNodo As String = ""
        Dim objVS As New AgronicaCoreXML.XML_Stampe

        Dim Xml_FiltroStampa = XmlDoc.CreateElement("Pippo")
        XmlDoc.AppendChild(Xml_FiltroStampa)

        For i = 0 To Chiavi.Count - 1
            Dim vVarStampe(2) As ElementoStampe
            vVarStampe(0).Nome = "p"
            vVarStampe(0).Valore = Chiavi(i).Split("_")(0)
            vVarStampe(1).Nome = "s"
            vVarStampe(1).Valore = Chiavi(i).Split("_")(1)
            vVarStampe(2).Nome = "i"
            vVarStampe(2).Valore = Chiavi(i).Split("_")(5)

            objVS.CreaInserisci_SottoNodo_XML_VariabiliStampe(XmlDoc, vVarStampe)
        Next

        Return XmlDoc

    End Function

    Private Shared Function ottieniSchedeDatiOP(Piva As String,
                                                rag_soc As String,
                                                anno As String,
                                                stampa As enum_CodificaStampe,
                                                tipoSelezione As String,
                                                chiavi As List(Of String),
                                                objParametri_Server As AgronicaCoreParametri,
                                                    Optional sa_cod As String = "",
                                                    Optional filtroSql As String = "",
                                                    Optional stampaVuota As Boolean = False,
                                                    Optional fronteRetro As Boolean = False)

        Dim StrSelezionati As String = ""
        Dim StrNodo As String = ""
        Dim objVS As New AgronicaCoreXML.XML_Stampe
        Dim vVarStampe(7) As ElementoStampe

        vVarStampe(0).Nome = "piva"
        vVarStampe(0).Valore = Piva
        vVarStampe(1).Nome = "rag_soc"
        vVarStampe(1).Valore = rag_soc
        vVarStampe(2).Nome = "anno"
        vVarStampe(2).Valore = anno
        vVarStampe(3).Nome = "DescrImpiantiSchedaOP"
        vVarStampe(3).Valore = ""

        If filtroSql.Length > 0 AndAlso tipoSelezione <> "azienda" Then
            Dim objRegImp = New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim id = GetIDTestataForJoin(chiavi, tipoSelezione, objParametri_Server)
            Dim dt = objRegImp.LeggiDescrImpianti(id, anno, objParametri_Server).ToExpandoObject
            Dim objFiltroImp As New __tmp_FiltroImpianti_W
            objFiltroImp.CancellaRecordDaIDTestataTemp(id, objParametri_Server)
            If dt.Any() Then
                Dim objCac As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
                dt.ForEach(Sub(x)
                               Dim s = x("Veg_Des") & " "
                               s &= x("Grfi_Des") & " "

                               Select Case stampa
                                   Case enum_CodificaStampe.Impegnativa_Fagiolino_Mercato_Fresco,
                                    enum_CodificaStampe.Impegnativa_Orticole_Industria
                                       s &= x("Grva_Des") & " "

                               End Select

                               If x("Dettaglio_Specie_Cod") <> "" Then
                                   s &= objCac.InfoAgg_Des_from_InfoAgg_Cod(x("Dettaglio_Specie_Cod"), 2, 0, objParametri_Server) & " "
                               End If

                               If x("Capitolato_Cod") <> "" Then
                                   s &= objCac.InfoAgg_Des_from_InfoAgg_Cod(x("Capitolato_Cod"), 1, 0, objParametri_Server) & " "
                               End If
                               If Not vVarStampe(3).Valore.Contains(s) Then
                                   vVarStampe(3).Valore &= s & ","
                               End If

                           End Sub)
            End If
            If vVarStampe(3).Valore.Length > 0 Then
                vVarStampe(3).Valore = Left(vVarStampe(3).Valore, vVarStampe(3).Valore.Length - 2)
            End If
        End If

        vVarStampe(4).Nome = "Sa_CodSchedaOP"
        vVarStampe(4).Valore = sa_cod

        vVarStampe(5).Nome = "stampaVuota"
        vVarStampe(5).Valore = stampaVuota.ToString

        vVarStampe(6).Nome = "tipoSelezione"
        vVarStampe(6).Valore = tipoSelezione

        vVarStampe(7).Nome = "fronteRetro"
        vVarStampe(7).Valore = fronteRetro

        Return objVS.XML_VariabiliStampe(vVarStampe)

    End Function

    Public Shared Function getStringaXml_PIVA_SA_COD_APPEZZA_ID_REG_VEG_COD_from_richiesta(Chiavi As List(Of String))
        Dim StrSelezionati As String = ""
        Dim StrNodo As String = ""
        Dim objVS As New AgronicaCoreXML.XML_Stampe


        For i = 0 To Chiavi.Count - 1
            Dim vVarStampe(5) As ElementoStampe

            Dim n_campi_chiave As String = Chiavi(i).Split("_").Count

            For j = 0 To n_campi_chiave - 1

                Select Case j
                    Case 0

                        vVarStampe(0).Nome = "piva"
                        vVarStampe(0).Valore = Chiavi(i).Split("_")(j)

                    Case 1
                        vVarStampe(1).Nome = "sa_cod"
                        vVarStampe(1).Valore = Chiavi(i).Split("_")(j)

                    Case 2
                        vVarStampe(2).Nome = "appezza"
                        vVarStampe(2).Valore = Chiavi(i).Split("_")(j)

                    Case 3
                        vVarStampe(3).Nome = "id_reg"
                        vVarStampe(3).Valore = Chiavi(i).Split("_")(j)

                        'se ho selezionato un terreno nudo passo al sito delle stampe il veg_cod=0

                    Case 4
                        vVarStampe(4).Nome = "veg_cod"
                        vVarStampe(4).Valore = Chiavi(i).Split("_")(j)

                    Case 5
                        vVarStampe(5).Nome = "id_agenda"
                        vVarStampe(5).Valore = Chiavi(i).Split("_")(j)
                End Select
            Next

            StrNodo = objVS.XML_VariabiliStampe(vVarStampe)

            StrSelezionati &= StrNodo

        Next

        Return StrSelezionati

    End Function

    Public Shared Function getStringaXml_PIVA_from_richiesta_P(Chiavi As List(Of String))
        Dim StrSelezionati As String = ""
        Dim StrNodo As String = ""
        Dim objVS As New AgronicaCoreXML.XML_Stampe

        For i = 0 To Chiavi.Count - 1
            Dim vVarStampe(0) As ElementoStampe
            vVarStampe(0).Nome = "p"
            vVarStampe(0).Valore = Chiavi(i).Split("_")(0)

            StrNodo = objVS.XML_VariabiliStampe(vVarStampe)

            StrSelezionati &= StrNodo
        Next

        Return StrSelezionati

    End Function

    Public Shared Function getStringaXml_PIVA_SA_COD_from_richiesta(Chiavi As List(Of String))
        Dim StrSelezionati As String = ""
        Dim StrNodo As String = ""
        Dim objVS As New AgronicaCoreXML.XML_Stampe
        Dim list_sa_cod As New List(Of String)

        For i = 0 To Chiavi.Count - 1

            Dim vVarStampe(1) As ElementoStampe
            vVarStampe(0).Nome = "piva"
            vVarStampe(0).Valore = Chiavi(i).Split("_")(0)
            vVarStampe(1).Nome = "sa_cod"
            vVarStampe(1).Valore = Chiavi(i).Split("_")(1)

            If Not list_sa_cod.Contains(Chiavi(i).Split("_")(0) & "_" & Chiavi(i).Split("_")(1)) Then
                StrNodo = objVS.XML_VariabiliStampe(vVarStampe)
                StrSelezionati &= StrNodo
                list_sa_cod.Add(Chiavi(i).Split("_")(0) & "_" & Chiavi(i).Split("_")(1))
            End If

        Next

        Return StrSelezionati

    End Function

    Public Shared Function getStringaXml_PIVA_SA_COD_APPEZZA_ID_REG_VEG_COD_from_richiesta_xdoc(ByVal XmlDoc As XmlDocument, Chiavi As List(Of String))
        Dim StrSelezionati As String = ""
        Dim StrNodo As String = ""
        Dim objVS As New AgronicaCoreXML.XML_Stampe

        Dim Xml_FiltroStampa = XmlDoc.CreateElement("VariabiliStampe")
        XmlDoc.AppendChild(Xml_FiltroStampa)


        For i = 0 To Chiavi.Count - 1
            Dim vVarStampe(4) As ElementoStampe

            vVarStampe(0).Nome = "piva"
            vVarStampe(0).Valore = Chiavi(i).Split("_")(0)
            vVarStampe(1).Nome = "sa_cod"
            vVarStampe(1).Valore = Chiavi(i).Split("_")(1)
            vVarStampe(2).Nome = "appezza"
            vVarStampe(2).Valore = Chiavi(i).Split("_")(2)
            vVarStampe(3).Nome = "id_reg"
            vVarStampe(3).Valore = Chiavi(i).Split("_")(3)

            'se ho selezionato un terreno nudo passo al sito delle stampe il veg_cod=0
            vVarStampe(4).Nome = "veg_cod"
            vVarStampe(4).Valore = Chiavi(i).Split("_")(4)

            objVS.CreaInserisci_SottoNodo_XML_VarStampa(XmlDoc, vVarStampe)

        Next

        Return XmlDoc

    End Function

    Public Shared Function getStringaXml_PIVA_from_richiesta(Chiavi As List(Of String))
        Dim StrSelezionati As String = ""
        Dim StrNodo As String = ""
        Dim objVS As New AgronicaCoreXML.XML_Stampe

        For i = 0 To Chiavi.Count - 1
            Dim vVarStampe(0) As ElementoStampe
            vVarStampe(0).Nome = "piva"
            vVarStampe(0).Valore = Chiavi(i).Split("_")(0)

            StrNodo = objVS.XML_VariabiliStampe(vVarStampe)
            StrSelezionati &= StrNodo
        Next

        Return StrSelezionati

    End Function

    Public Shared Function getStringaXml_PIVA_SA_COD_APPEZZA_ID_REG_from_richiesta_Esporta_Imprese(Chiavi As List(Of String))
        Dim StrSelezionati As String = ""
        Dim StrNodo As String = ""
        Dim objVS As New AgronicaCoreXML.XML_Stampe

        For i = 0 To Chiavi.Count - 1
            Dim vVarStampe(0) As ElementoStampe
            vVarStampe(0).Nome = "p"
            vVarStampe(0).Valore = Chiavi(i).Split("_")(0)

            StrNodo = objVS.XML_VariabiliStampe(vVarStampe)

            StrSelezionati &= StrNodo
        Next

        Return StrSelezionati

    End Function

    Public Shared Function getStringaXml_PIVA_SA_COD_APPEZZA_ID_REG_from_richiesta(Chiavi As List(Of String))
        Dim StrSelezionati As String = ""
        Dim StrNodo As String = ""
        Dim objVS As New AgronicaCoreXML.XML_Stampe

        For i = 0 To Chiavi.Count - 1
            Dim vVarStampe(3) As ElementoStampe
            vVarStampe(0).Nome = "piva"
            vVarStampe(0).Valore = Chiavi(i).Split("_")(0)
            vVarStampe(1).Nome = "sa_cod"
            vVarStampe(1).Valore = Chiavi(i).Split("_")(1)
            vVarStampe(2).Nome = "appezza"
            vVarStampe(2).Valore = Chiavi(i).Split("_")(2)
            vVarStampe(3).Nome = "id_reg"
            vVarStampe(3).Valore = Chiavi(i).Split("_")(3)

            StrNodo = objVS.XML_VariabiliStampe(vVarStampe)

            StrSelezionati &= StrNodo
        Next
        Return StrSelezionati
    End Function

    Public Shared Function getStringaXml_PIVA_SA_COD_IDAGENDA_from_richiesta_Esporta_Agenda(Chiavi As List(Of String))
        Dim StrSelezionati As String = ""
        Dim StrNodo As String = ""
        Dim objVS As New AgronicaCoreXML.XML_Stampe

        For i = 0 To Chiavi.Count - 1
            Dim vVarStampe(2) As ElementoStampe
            vVarStampe(0).Nome = "p"
            vVarStampe(0).Valore = Chiavi(i).Split("_")(0)
            vVarStampe(1).Nome = "s"
            vVarStampe(1).Valore = Chiavi(i).Split("_")(1)
            vVarStampe(2).Nome = "i"
            vVarStampe(2).Valore = Chiavi(i).Split("_")(5)

            StrNodo = objVS.XML_VariabiliStampe(vVarStampe)

            StrSelezionati &= StrNodo
        Next

        Return StrSelezionati
    End Function
#End Region

#End Region

#Region "METODI Filtro Ricerca NG"
    Public Function GetTipoComportamentoFiltroRicercaPerStampa(CodificaStampe As enum_CodificaStampe) As Enum_TipoComportamento_FiltroRicerca
        Select Case CodificaStampe
            Case enum_CodificaStampe.RiepilogoImpiegoSuperfici_Multiazienda,
                 enum_CodificaStampe.PianoColturale,
                 enum_CodificaStampe.PianoColturaleCatasto,
                 enum_CodificaStampe.PianoColturaleCatastoGrid,
                 enum_CodificaStampe.ReportConserveItalia,
                 enum_CodificaStampe.Esportatore_Universale_Rintraccio,
                 enum_CodificaStampe.Esportatore_Universale_Agenda,
                 enum_CodificaStampe.Esportazione_AnagraficaContatti,
                 enum_CodificaStampe.Esportatore_Universale_Centri,
                 enum_CodificaStampe.Esportatore_Universale_Impianti,
                 enum_CodificaStampe.Esportatore_Universale_Imprese,
                 enum_CodificaStampe.Esportazione_CellulariContatti,
                 enum_CodificaStampe.Bilancio_Fertilizzazioni,
                 enum_CodificaStampe.Bilancio_Fertilizzazioni_Dettagliato,
                 enum_CodificaStampe.Esportazione_OP_Inv,
                 enum_CodificaStampe.Esportazione_OP_Gest,
                 enum_CodificaStampe.Esportazione_OP_Gest_Coop,
                 enum_CodificaStampe.Esportazione_OP_Produttori,
                 enum_CodificaStampe.Esportazione_OP_Catasto
                Return Enum_TipoComportamento_FiltroRicerca.EsportaExcel
            Case Else
                Return Enum_TipoComportamento_FiltroRicerca.EsportaPdf
        End Select

    End Function

    Public Function GetTipoMostraFiltroRicercaPerStampa(CodificaStampe As enum_CodificaStampe, datiStampe As DatiStampe) As List(Of Enum_TipoMostra_FiltroRicerca)
        Dim TipoMostraGestitixStampa As New List(Of Enum_TipoMostra_FiltroRicerca)

        Select Case CodificaStampe
#Region "CASI MISTI"
            Case enum_CodificaStampe.ImpegnativaColtivazioneConferimento
                If datiStampe.switchGenerale1 = True Then
                    TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Aziende)
                Else
                    TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Impianti)
                    TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Esercizi)
                End If
#End Region
#Region "AZIENDA"
            Case enum_CodificaStampe.Esportazione_AnagraficaContatti,
                 enum_CodificaStampe.Esportatore_Universale_Imprese,
                 enum_CodificaStampe.Accordo_Responsabilita_di_Filiera,
                 enum_CodificaStampe.Dichiarazione_di_Responsabilita,
                 enum_CodificaStampe.Fitoregolatori_Kiwi,
                 enum_CodificaStampe.QuestionarioValutazioneAzienda_Aggiornamento,
                 enum_CodificaStampe.Adesione_Etico_Ambientale,
                 enum_CodificaStampe.Tenuta_Scheda_Campagna,
                 enum_CodificaStampe.Adesione_DPI,
                 enum_CodificaStampe.Impegnativa_Eurep,
                 enum_CodificaStampe.Impegnativa_QC,
                 enum_CodificaStampe.Impegnativa_Confusione_Sessuale,
                 enum_CodificaStampe.Codice_Condotta,
                 enum_CodificaStampe.Mandato_Trasmissione_Telematica_Dati
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Aziende)
#End Region
#Region "CENTRO AZIENDALE"
            Case enum_CodificaStampe.EstrattoreDatiGrafici
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.CentriAziendali)
#End Region
#Region "IMPIANTO"
            Case enum_CodificaStampe.ReportConserveItalia,
                 enum_CodificaStampe.Bilancio_Fertilizzazioni
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Impianti)
#End Region
#Region "MOVIMENTO"
            Case enum_CodificaStampe.Esportatore_Universale_Agenda
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Movimenti)
#End Region

#Region "AZIENDA - CENTRO AZIENDALE"
            Case enum_CodificaStampe.Quadro_P
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Aziende)
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.CentriAziendali)
#End Region
#Region "AZIENDA - IMPIANTO - ESERCIZIO"
            Case enum_CodificaStampe.SchedaAziendale,
                 enum_CodificaStampe.ImpegnoProduzioneSociDivisoxCentri,
                 enum_CodificaStampe.ObiettivoDiProduzioneAsipo
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Aziende)
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Impianti)
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Esercizi)
#End Region
#Region "AZIENDA - ESERCIZIO"
            Case enum_CodificaStampe.Adesione_Despar,
                 enum_CodificaStampe.Adesione_Conad
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Aziende)
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Esercizi)
#End Region

#Region "CENTRO AZIENDALE - IMPIANTO - ESERCIZIO"
            Case enum_CodificaStampe.Esportatore_Universale_Centri
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.CentriAziendali)
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Impianti)
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Esercizi)

#End Region

#Region "IMPIANTO - ESERCIZIO"
            Case enum_CodificaStampe.SchedaColturale_Biologico,
                 enum_CodificaStampe.Esportatore_Universale_Impianti,
                 enum_CodificaStampe.Atto_Notorio,
                 -enum_CodificaStampe.Atto_Notorio,
                 enum_CodificaStampe.Esportazione_OP_Catasto,
                 enum_CodificaStampe.Adesione_ModuloGrasp,
                 enum_CodificaStampe.Adesione_NurtureModule,
                 enum_CodificaStampe.Adesione_ProtocolloGlobalGAP,
                 enum_CodificaStampe.Adesione_StandardLeaf,
                 enum_CodificaStampe.ImpegnativaColtivazioneConferimento,
                 enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
                 enum_CodificaStampe.RegistroTrattamenti_Semplificata,
                 enum_CodificaStampe.SchedaRegistrazione_Semplificata,
                 enum_CodificaStampe.Eurep_Gap_Semplificata,
                 enum_CodificaStampe.RegistroTrattamenti_Veneto,
                 enum_CodificaStampe.SchedaCampagna_Multicentro,
                 enum_CodificaStampe.SchedaCampagna_Multicentro_ACA,
                 enum_CodificaStampe.RiepilogoImpiegoSuperfici_Multiazienda,
                 enum_CodificaStampe.Eurep_Gap_Multicentro,
                 enum_CodificaStampe.SchedaCampagna_ProvAut_Trento,
                 enum_CodificaStampe.SchedaCampagna_Multi_Lombardia,
                 enum_CodificaStampe.SchedaInterventiAgronomici,
                 enum_CodificaStampe.RegistroAziendaleUnico,
                 enum_CodificaStampe.Registro_Fertilizzazioni,
                 enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita,
                 enum_CodificaStampe.SchedaTracciabilita,
                 enum_CodificaStampe.PianoColturaleCatastoGrid,
                 enum_CodificaStampe.SchedaCatastoeUtilizzi,
                 enum_CodificaStampe.PianoColturaleCatasto,
                 enum_CodificaStampe.PianoColturale,
                 enum_CodificaStampe.Impegnativa_Orticole_Gest_Annuale,
                 enum_CodificaStampe.Impegnativa_Orticole_Gest_Breve,
                 enum_CodificaStampe.Impegnativa_Orticole_Industria,
                 enum_CodificaStampe.Impegnativa_Pomodoro_Industria,
                 enum_CodificaStampe.Impegnativa_Fagiolino_Mercato_Fresco,
                 enum_CodificaStampe.Allegato_CatastoeValorizzazioni

                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Impianti)
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Esercizi)

#End Region
#Region "IMPIANTO - MOVIMENTO"
            Case enum_CodificaStampe.Bilancio_Fertilizzazioni_Dettagliato
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Movimenti)
                TipoMostraGestitixStampa.Add(Enum_TipoMostra_FiltroRicerca.Impianti)
#End Region
        End Select

        Return TipoMostraGestitixStampa

    End Function

    Public Function GetBlocchiSelezionePerStampa(CodificaStampe As enum_CodificaStampe) As BlocchiSelezionexStampa
        Dim BlocchiSelezionexStampa As New BlocchiSelezionexStampa

        BlocchiSelezionexStampa.SingolaAzienda = False
        BlocchiSelezionexStampa.SingoloCentro = False
        BlocchiSelezionexStampa.SingolaSpecie = False

        Select Case CodificaStampe
#Region "SINGOLA AZIENDA"
            Case enum_CodificaStampe.RegistroTrattamenti,
                 enum_CodificaStampe.RegistroTrattamenti_Semplificata,
                 enum_CodificaStampe.SchedaCampagna_Multi_Lombardia,
                 enum_CodificaStampe.Bilancio_Fertilizzazioni,
                 enum_CodificaStampe.Bilancio_Fertilizzazioni_Dettagliato,
                 enum_CodificaStampe.Atto_Notorio, -enum_CodificaStampe.Atto_Notorio,
                 enum_CodificaStampe.Adesione_Etico_Ambientale,
                 enum_CodificaStampe.Tenuta_Scheda_Campagna,
                 enum_CodificaStampe.Codice_Condotta,
                 enum_CodificaStampe.Adesione_DPI,
                 enum_CodificaStampe.Impegnativa_Eurep,
                 enum_CodificaStampe.Impegnativa_QC,
                 enum_CodificaStampe.Impegnativa_Confusione_Sessuale,
                 enum_CodificaStampe.Allegato_CatastoeValorizzazioni,
                 enum_CodificaStampe.Mandato_Trasmissione_Telematica_Dati,
                 enum_CodificaStampe.Impegnativa_Orticole_Gest_Annuale,
                 enum_CodificaStampe.Impegnativa_Orticole_Gest_Breve,
                 enum_CodificaStampe.Impegnativa_Fagiolino_Mercato_Fresco,
                 enum_CodificaStampe.Impegnativa_Orticole_Industria,
                 enum_CodificaStampe.Impegnativa_Pomodoro_Industria,
                 enum_CodificaStampe.ObiettivoDiProduzioneAsipo,
                 enum_CodificaStampe.SchedaCatastoeUtilizzi,
                 enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita,
                 enum_CodificaStampe.Registro_Fertilizzazioni,
                 enum_CodificaStampe.RegistroAziendaleUnico,
                 enum_CodificaStampe.SchedaInterventiAgronomici,
                 enum_CodificaStampe.SchedaCampagna_Multi_Lombardia,
                 enum_CodificaStampe.SchedaCampagna_ProvAut_Trento,
                 enum_CodificaStampe.RegistroTrattamenti_Veneto,
                 enum_CodificaStampe.SchedaColturale_Biologico

                BlocchiSelezionexStampa.SingolaAzienda = True
#End Region
#Region "SINGOLA AZIENDA - SPECIE"
            Case enum_CodificaStampe.SchedaCampagna_Multicentro,
                 enum_CodificaStampe.Eurep_Gap_Multicentro

                BlocchiSelezionexStampa.SingolaAzienda = True
                BlocchiSelezionexStampa.SingolaSpecie = True
#End Region
#Region "SINGOLA AZIENDA - CENTRO"
            Case enum_CodificaStampe.Quadro_P,
                 enum_CodificaStampe.SchedaTracciabilita,
                 enum_CodificaStampe.EstrattoreDatiGrafici

                BlocchiSelezionexStampa.SingolaAzienda = True
                BlocchiSelezionexStampa.SingoloCentro = True
#End Region
#Region "SINGOLA AZIENDA - CENTRO - SPECIE"
            Case enum_CodificaStampe.SchedaCampagna_2078,
                 enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
                 enum_CodificaStampe.SchedaRegistrazione_Semplificata,
                 enum_CodificaStampe.Eurep_Gap,
                 enum_CodificaStampe.Eurep_Gap_Semplificata
                BlocchiSelezionexStampa.SingolaAzienda = True
                BlocchiSelezionexStampa.SingoloCentro = True
                BlocchiSelezionexStampa.SingolaSpecie = True
#End Region
        End Select

        Return BlocchiSelezionexStampa

    End Function

    Public Function Imposta_FiltroEntitaAttivaAllaData(data As Date, entita As Enum_Entita_FiltroRicerca) As FiltriTemporali
        Dim FiltroTemporale As New List(Of FiltroTemporale)

        FiltroTemporale.Add(
        New FiltroTemporale With {
                    .Entita = entita,
                    .ColonnaData = Enum_ColonnaData_FiltroRicerca.ValiditaInizio,
                    .TipoConfronto = Enum_TipoConfronto_FiltroRicerca.MinoreUguale,
                    .ModalitaFiltroData = Enum_ModalitaFiltroData_FiltroRicerca.Manuale,
                    .Date = New IntervalloTemporale(data, AGRODATAFINE)
                    }
         )

        FiltroTemporale.Add(
        New FiltroTemporale With {
                    .Entita = entita,
                    .ColonnaData = Enum_ColonnaData_FiltroRicerca.ValiditaFine,
                    .TipoConfronto = Enum_TipoConfronto_FiltroRicerca.MaggioreUguale,
                    .ModalitaFiltroData = Enum_ModalitaFiltroData_FiltroRicerca.Manuale,
                    .Date = New IntervalloTemporale(data, AGRODATAFINE)
                    }
         )

        Return Imposta_FiltriTemporali(FiltroTemporale)

    End Function
    Public Function Imposta_FiltriTemporali(Optional FiltriData As List(Of FiltroTemporale) = Nothing,
                                            Optional OperatoreLogicoFiltriTemporali As Enum_FiltroOperatoreLogico_FiltroRicerca = Enum_FiltroOperatoreLogico_FiltroRicerca.AND_TutteLeCondizioniTrue
                                            ) As FiltriTemporali

        Return New FiltriTemporali With {
            .FiltriData = If(IsNothing(FiltriData), New List(Of FiltroTemporale), FiltriData),
            .OperatoreLogicoFiltriTemporali = OperatoreLogicoFiltriTemporali
        }

    End Function

    Public Function Imposta_FiltriMovimenti(Optional FiltroOperazioni As Enum_FiltroOperazioniSelezionate_FiltroRicerca = Enum_FiltroOperazioniSelezionate_FiltroRicerca.ConOperazioni,
                                            Optional GruppoOperazioni As List(Of Integer) = Nothing,
                                            Optional Operazioni As List(Of Integer) = Nothing,
                                            Optional DataMovimento As IntervalloTemporale = Nothing) As FiltriMovimenti

        Return New FiltriMovimenti With {
            .FiltroOperazioni = CInt(FiltroOperazioni),
            .GruppoOperazioni = If(IsNothing(GruppoOperazioni), New List(Of Integer), GruppoOperazioni),
            .Operazioni = If(IsNothing(Operazioni), New List(Of Integer), Operazioni),
            .DataMovimento = DataMovimento
        }

    End Function

    Public Function Imposta_FiltriPianoColturale(Optional FiltroDestinazioneUso As Enum_FiltroDestinazioneUso_FiltroRicerca = Enum_FiltroDestinazioneUso_FiltroRicerca.Tutto,
                                                 Optional UtilizzoTerreno As List(Of Integer) = Nothing,
                                                 Optional DestinazioniUso As List(Of Integer) = Nothing,
                                                 Optional GruppoVegetale As List(Of Integer) = Nothing,
                                                 Optional Specie As List(Of Integer) = Nothing,
                                                 Optional TipologiaVarietale As List(Of Integer) = Nothing,
                                                 Optional Varieta As List(Of Integer) = Nothing,
                                                 Optional Lotto As String = "",
                                                 Optional Progetto As String = "") As FiltriPianoColturale

        Return New FiltriPianoColturale With {
            .FiltroDestinazioneUso = FiltroDestinazioneUso,
            .UtilizzoTerreno = If(IsNothing(UtilizzoTerreno), New List(Of Integer), UtilizzoTerreno),
            .DestinazioniUso = If(IsNothing(DestinazioniUso), New List(Of Integer), DestinazioniUso),
            .Specie = If(IsNothing(Specie), New List(Of Integer), Specie),
            .TipologiaVarietale = If(IsNothing(TipologiaVarietale), New List(Of Integer), TipologiaVarietale),
            .GruppoVegetale = If(IsNothing(GruppoVegetale), New List(Of Integer), GruppoVegetale),
            .Varieta = If(IsNothing(Varieta), New List(Of Integer), Varieta),
            .Lotto = Lotto,
            .Progetto = Progetto
        }
    End Function

#End Region


End Class

Public Class RespFiltroRicercaNG
    Public Property TipoMostra As Integer
    Public Property RispostaStringa As String
    Public Property RedirectFiltroRicercaNG As Boolean
End Class