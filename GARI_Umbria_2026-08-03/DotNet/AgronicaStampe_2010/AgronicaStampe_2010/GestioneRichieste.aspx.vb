Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports Agronica.Helpers.GiasBase

Public Class GestioneRichieste
    Inherits System.Web.UI.Page

    Const PaginaLinkStampaFiltroSchedeMagazzino As String = "GestioneStampe/Magazzino/Filtro_SchedeMagazzino_new.aspx"

    Const PaginaLinkStampaSchedaColturaleBiologico = "GestioneStampe/SchedaCampagna/Selezione_SchedaCampagna.aspx"
    Const PaginaLinkFiltroSchedeBiologico = "GestioneStampe/Biologico/Filtro_SchedeBiologico.aspx"
    Const PaginaLinkFiltroReportBiologico = "GestioneStampe/Biologico/Filtro_ReportBiologico.aspx"

    Const PaginaFiltroElaboratiContabili = "GestioneStampe/Contabilita/Filtro_ElaboratiContabili.aspx"
    Const PaginaLinkLiquidazioneIVA = "GestioneStampe/Contabilita/LiquidazioneIVA/LiquidazioneIVA_Anteprima.aspx"
    Const PaginaLinkLiquidazioneIVA_NEW = "GestioneStampe/Contabilita/LiquidazioneIVA/LiquidazioneIVA_Anteprima_3.aspx"

    Const PaginaLinkStampeDDTBollaConferimento = "GestioneStampe/Contabilita/Fattura/DDT_BolleConf.aspx"
    Const PaginaLinkStampeFatturaNotaAccredito = "GestioneStampe/Contabilita/Fattura/Fattura_NotaAccredito.aspx"
    Const PaginaLinkFiltroFatturaDDT = "GestioneStampe/Contabilita/Filtro_Fattura.aspx"
    Const PaginaLinkStampeRicevutaPDFa5 = "GestioneStampe/Contabilita/Fattura/RicevutaFiscaleA5.aspx"
    Const PaginaLinkStampeRicevutaPDFa4 = "GestioneStampe/Contabilita/Fattura/RicevutaFiscaleA4.aspx"
    Const PaginaLinkStampeRicevutaPDFa4aCapoAuto = "GestioneStampe/Contabilita/Fattura/RicevutaFiscaleA4ACapoAuto.aspx"
    Const PaginaLinkStampeRicevutaWord = "GestioneStampe/Contabilita/RicevutaFiscale/RicevutaWord.aspx"
    Const PaginaLinkFiltroReportAccettazioneDaDiversi = "GestioneStampe/AccettazioneDaDiversi/Filtro_Report_AccettazioneDaDiversi.aspx"
    Const PaginaLinkFiltroReportFreshFood = "GestioneStampe/Conferimenti/Filtro_Stampe_Conf.aspx"
    Const PaginaLinkFiltroReportCespiti = "GestioneStampe/Cespiti/Filtro_Stampe_Cespiti.aspx"
    Const PaginaLinkStampeBollaFreshFood = "GestioneStampe/FreshAndFood/Bolla/Bolla_FF.aspx"
    Const PaginaLinkStampeConferimentoUva = "GestioneStampe/Contabilita/Fattura/DDT_BolleConf.aspx"
    Const PaginaLinkStampeCertificatoPomodoro = "GestioneStampe/Conferimenti/CertificatoPomodoro/CertificatoPomodoro.aspx"

    Const PaginaLinkEsportazione_AnagraficaContatti = "GestioneEsportazioni/Esportazione_AnagraficaContatti/Filtro_Contatti.aspx"
    Const PaginaLinkStampeDoco = "GestioneStampe/RegistriPreparazioni/Filtro_Registri.aspx"
    Const PaginaLinkStampeDAA = "GestioneStampe/RegistriPreparazioni/DAA/DAA_Avanti.aspx"

    'Const PaginaLinkStampeCantina = "GestioneStampe/RegistriPreparazioni/Filtro_Registri.aspx"
    Const PaginaLinkStampeCantina = "GestioneStampe/RegistriPreparazioni/Filtro_StampeCantine.aspx"
    Const PaginaLinkBrogliaccioMovimenti = "GestioneStampe/Cantine/BrogliaccioMovimentiTabella/BrogliaccioMovimentiTabella.aspx"

    Const PaginaLinkEsportatore_Universale = "GestioneEsportazioni/Esportatore_Universale/Esportatore_Universale_2.aspx"

    Const PaginaLinkStampaEstrattoreGrafica = "GestioneStampe/EstrattoreGrafica/Filtro_EstrattoreGrafica.aspx"

    Const PaginaLinkFreshFoodEtichette = "GestioneStampe/FreshAndFood/Etichette/AnteprimaEtichette.aspx"

    Const PaginaLinkLiquidazioneSoci = "GestioneStampe/Contabilita/LiquidazioneSoci/LiquidazioneSoci.aspx"

    Const PaginaLinkSchedeVarieOP = "GestioneStampe/Schede_OP/Filtro_Schede_OP.aspx"
    Const PaginaLinkEsportazioneOP_Filtro = "GestioneStampe/Schede_OP/Esportazioni_OP/Esportazioni_OP_Filtro.aspx"
    Const PaginaLinkEsportazioneOP_Catasto = "GestioneStampe/Schede_OP/Esportazioni_OP/Esportazione_OP_Catasto.aspx"
    Const PaginaLinkEsportazioneOP_Produttori = "GestioneStampe/Schede_OP/Esportazioni_OP/Esportazione_OP_Produttori.aspx"

    Const PaginaLinkBilancio_Fertilizzazioni = "GestioneStampe/BilancioFertilizzazioni/BilancioFertilizzazioni_XLS.aspx"
    Const PaginaLinkBilancio_Fertilizzazioni_Dettagliato = "GestioneStampe/BilancioFertilizzazioni/BilancioFertilizzazioni_Dettagliato_XLS.aspx"

    Const PaginaLinkStampeBollaCampionaturaFreshFood = "GestioneStampe/FreshAndFood/Liquidazione/BollaCampionatura.aspx"
    Const PaginaLinkStampeFatturaLiquidazioneSociFreshFood = "GestioneStampe/FreshAndFood/Liquidazione/FatturaLiquidazioneSoci.aspx"
    Const PaginaLinkStampePagatiSuConferitoFreshFood = "GestioneStampe/FreshAndFood/Liquidazione/PagatiSuConferito.aspx"
    Const PaginaLinkStampePagatiSuCampionatoFreshFood = "GestioneStampe/FreshAndFood/Liquidazione/PagatiSuCampionato.aspx"
    Const PaginaLinkStampeRiepilogoLiquidazioneSociFreshFood = "GestioneStampe/FreshAndFood/Liquidazione/RiepilogoLiquidazioneSoci.aspx"

    Const PaginaLinkStampaExcel_MonitoraggioCE = "GestioneStampe/MonitoraggioCorpiEstranei/MonitoraggioCorpiEstranei_XLS.aspx"
    Const PaginaLinkStampaExcel_MonitoraggioCEAggregata = "GestioneStampe/MonitoraggioCorpiEstranei/MonitoraggioCorpiEstraneiAggregata_XLS.aspx"

    Const PaginaLinkStampaRisultatoAnalisiConformita = "GestioneStampe/SchedaCampagna/RisultatoAnalisiConformita.aspx"
    Const PaginaLinkStampeAnalisiProgetti = "GestioneStampe/ControlloGestione/AnalisiProgetti.aspx"

    Const PaginaLinkStampeFiltroSchedaTracciabilitaVegetale = "GestioneStampe/SchedaTracciabilita/Filtro_SchedaTracciabilita.aspx"

    Const PaginaLinkStampaLibroConferimenti = "GestioneStampe/Conferimenti/LibroConferimenti/LibroConferimenti.aspx"
    Const PaginaLinkStampaLibroConferimentiXLS = "GestioneStampe/Conferimenti/LibroConferimenti/LibroConferimenti_XLS.aspx"

    Const PaginaLinkStampaDAAGaranzieCircolazione = "GestioneStampe/RegistriPreparazioni/DAA/DAA_PartiteSospensioneAccisa.aspx"
    Const PaginaLinkFiltroReportDAA = "GestioneStampe/RegistriPreparazioni/DAA/FiltroStampeDAA.aspx"

    Const PaginaLinkStampaMVV = "GestioneStampe/Contabilita/MVV/StampaMVV.aspx"

    Const PaginaLinkStampaPianoColturale = "GestioneStampe/CatastoPianoColturale/PianoColturaleExcel/PianoColturale_XLS.aspx"
    Const PaginaLinkStampaPianoColturaleCatasto = "GestioneStampe/CatastoPianoColturale/PianoColturaleExcel/PianoColturaleCatasto_XLS.aspx"
    Const PaginaLinkStampaPianoColturaleCatastoGrid = "GestioneStampe/CatastoPianoColturale/PianoColturaleGrid/PianoColturaleCatasto_grid.aspx"
    Const PaginaLinkEsportazionePomodoroIndustriaOINordItalia = "GestioneStampe/CatastoPianoColturale/OIPomodoroIndustriaNordItalia/CatastoOIPomodoroIndustriaNordItalia_XLS.aspx"

    Const PaginaLinkStampaStatistica12Mesi = "GestioneStampe/Contabilita/Statistiche/StampaStat12Mesi.aspx"

    Const PaginaLinkEsportazioneGiasToSap = "GestioneStampe/SchedePersonalizzate/Esporta_GiasToSap.aspx"

    Const PaginaLinkStampaPassaportoVivaistico = "GestioneStampe/Magazzino/Vivaismo/StampaPassaporto.aspx"

    Const PaginaLinkUMARichiestaCarbPrevisioneLav = "GestioneStampe/CarburantiUMA/RichiestaCarbPrevisioneLav.aspx"
    Const PaginaLinkUMAVerbaleIstruttoriaRichCarb = "GestioneStampe/CarburantiUMA/VerbaleIstruttoriaRichCarb.aspx"
    Const PaginaLinkUMARendicontazioneCarb = "GestioneStampe/CarburantiUMA/RendicontazioneCarb.aspx"
    Const PaginaLinkUMAIstruttoriaRendCarb = "GestioneStampe/CarburantiUMA/IstruttoriaRendCarb.aspx"

    Const PaginaLinkRiepilogoSuperficiMonoAzienda = "GestioneStampe/RiepilogoSuperfici/MonoAzienda/Filtro_RiepilogoUtilizzoSuperfici.aspx"
    Const PaginaLinkRiepilogoSuperficiMultiazienda = "GestioneStampe/RiepilogoSuperfici/MultiAzienda/Filtro_RiepilogoSuperficiMultiazienda.aspx"

    Const PaginaLinkEstrazioneCatastoAffitti = "GestioneStampe/CatastoAffitti/EstrazioneCatastoAffitti/EstrazioneCatastoAffitti.aspx"

    Const PaginaLinkImpegnoProduzioneSoci = "GestioneStampe/ImpegnoProduzioneSoci/Filtro_ImpegnoProduzioneSoci.aspx"

    Const PaginaLinkXLSConserveItalia = "GestioneStampe/SchedePersonalizzate/ReportConserveItalia_XLS.aspx"

    Const PaginaLinkOrdiniVivaio = "GestioneStampe/MaterialeVivaistico/OrdiniVivaio.aspx"
    Const PaginaLinkAttoNotorio = "GestioneStampe/Schede_OP/AttoNotorio/AttoNotorio.aspx"
    Const PaginaLinkAttoNotorioConCatasto = "GestioneStampe/Schede_OP/AttoNotorio_2tipo/SchedaAutoCertificazione.aspx"
    Const PaginaLinkSchedaOPTipo1 = "GestioneStampe/Schede_OP/Scheda_OP_Tipo_1/Scheda_OP_Tipo_1.aspx"
    Const PaginaLinkSchedaOPTipo2 = "GestioneStampe/Schede_OP/Scheda_OP_Tipo_2/Scheda_OP_Tipo_2.aspx"
    Const PaginaLinkMandatoTrasmissione = "GestioneStampe/Schede_OP/MandatoTrasmissioneDati/MandatoTrasmissioneDati.aspx"
    Const PaginaLinkSchedaAziendale = "GestioneStampe/Schede_OP/SchedaAziendale/SchedaAziendale.aspx"
    Const PaginaLinkImpegnoProduzioneSociDivisoxCentri = "GestioneStampe/Schede_OP/Autocertificazione/ImpegnoProduzioneSocixCentri.aspx"
    Const PaginaLinkExcelAutomatico_XLS = "GestioneEsportazioni/ExcelAutomatico/ExcelAutomatico_XLS.aspx"

    Const PaginaLinkObiettivoDiProduzione = "GestioneStampe/MaterialeVivaistico/ObiettivoDiProduzione.aspx"
    
    Const PaginaLinkImpegnativaColtivazioneConferimento = "GestioneStampe/Schede_OP/ImpegnativaColtivazioneConferimento/ImpegnativaColtivazioneConferimento.aspx"
    Const PaginaLinkQuestionarioValutazioneAzienda = "GestioneStampe/Schede_OP/QuestionarioValutazioneAzienda_Aggiornamento/QuestionarioValutazioneAzienda_Aggiornamento.aspx"
    Const PaginaStampaAbilitazioni = "GestioneStampe/PianoCampionamento/Abilitazioni/ReportAbilitazioni.aspx"

    Const PaginaLinkStatistometro = "Statistiche/Statistiche_Accesso.aspx"

    Const PaginaStampaAnalisiFitofarmaci = "GestioneStampe/PianoCampionamento/StampaRapidaFitofarmaci/Stampa_Rapida_Fitofarmaci.aspx"
    Const PaginaStampaEtichetta = "GestioneStampe/PianoCampionamento/Etichetta/StampaEtichetta.aspx"
    Const PaginaStampaRapportodiProva = "GestioneStampe/PianoCampionamento/RapportoProva/StampaRapportoDiProva.aspx"
    Const PaginaLinkChecklistGlobalGap_RappVerificaIspettiva = "GestioneStampe/RapportoVerificaIspettiva/GlobalGap_RappVerificaIspettiva.aspx"
    Const PaginaLinkChecklistGlobalGap_RapportoNC = "GestioneStampe/RapportoNC/GlobalGap_RapportoNC.aspx"
    Const PaginaLinkChecklistGlobalGap_StampaCrystal = "GestioneStampe/GlobalGap/GlobalGap_StampaCrystal.aspx"

    Const PaginaLinkFiltroZootecnia = "GestioneStampe/Zootecnia/FiltroZootecnia.aspx"

    '########################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        Response.Expires = 0

        Dim Qs_Report As enum_CodificaStampe = enum_CodificaStampe.Nessuna
        Dim Unid As String = ""
        Dim Cn_Server As String = ""
        Dim strXmlVariabilistampe As String
        Dim id_agenda As Integer = 0 'per Liquidazione soci

        Try

            'MS Elimina i report temporanei più vecchi di un giorno creati per il passaggio a VisualizzatoreReport 
            'Vanno eliminati qui perché il visualizzatore può eseguire varie PostBack e il report deve 
            'rimanere disponibile fino alla fine delle attività sulla stampa eseguita.
            CrystalHelper.eliminaReportTemporanei()

            If Request.QueryString("unid") IsNot Nothing Then

                '------------------- ATTENZIONE!!!!!!!!!!!!!!!!! ---------------------------
                'CHIAMATA DA GIASONLINE 2003, AGRONICA STAMPE 2003
                ' E NUOVA CHIAMATA DA GIASLAN
                '----------------------------------------------------------------------------

                Unid = Stringa_Decodifica(Request.QueryString("unid").ToString, AgroKey_EncoderDecoder, Server)

                Cn_Server = Stringa_Decodifica(Request.QueryString("cn").ToString, AgroKey_EncoderDecoder, Server)

                If Unid = "" Then
                    UtilityProvider.AgroMsgBox("Unid non passato tramite querystring!", Page)
                    Exit Sub
                End If

                If Cn_Server = "" Then
                    UtilityProvider.AgroMsgBox("Cn_Server non passato tramite querystring!", Page)
                    Exit Sub
                End If

            ElseIf Request.Form("TxtParametri") IsNot Nothing Then
                '----------------------------------------------------
                '   VECCHIA VERSIONE DI CHIAMATA 
                '----------------------------------------------------

                Dim strParametri As String = ""
                Dim StrParametriCodificata As String = ""

                StrParametriCodificata = Request.Form("TxtParametri").ToString()
                strParametri = Stringa_Decodifica_Nuova(StrParametriCodificata, AgroKey_EncoderDecoder, Server)

                If strParametri <> "" Then

                    Dim XmlDoc As New XmlDocument
                    Dim XML_Parametri As XmlElement

                    XmlDoc.LoadXml(strParametri)

                    If XmlDoc.HasChildNodes Then

                        XML_Parametri = XmlDoc.SelectSingleNode("Parametri")

                        If XML_Parametri IsNot Nothing Then

                            If XML_Parametri.HasAttribute("unid") Then
                                Unid = XML_Parametri.GetAttribute("unid")
                            Else
                                Unid = ""
                            End If
                            If XML_Parametri.HasAttribute("cn") Then
                                Cn_Server = XML_Parametri.GetAttribute("cn")
                            Else
                                Cn_Server = ""
                            End If

                            If Unid = "" Then
                                UtilityProvider.AgroMsgBox("Unid non passato tramite TxtParametri!", Page)
                                Exit Sub
                            End If

                            If Cn_Server = "" Then
                                UtilityProvider.AgroMsgBox("Cn_Server non passato tramite TxtParametri!", Page)
                                Exit Sub
                            End If

                        End If

                    End If

                End If

            ElseIf Request.Form("TxtVariabiliStampe") IsNot Nothing Then
                '----------------------------------------------------
                '   CHIAMATA DAL LAN OLD
                '----------------------------------------------------


                Me.TxtRisultato.Text = Request.Form("TxtVariabiliStampe").ToString()

                If Me.TxtRisultato.Text <> "" Then


                    If Left(Me.TxtRisultato.Text, 4) = "MAGA" Then
                        '---------------------------------------------
                        'CASO PARTICOLARE:
                        'Se il primo carattere della stringa è MAGA
                        'allora è il caso dell'Esportatore Universale o dell'Esportazione Op_Investimenti
                        'o dell'Esportazione Op_Gest
                        '---------------------------------------------

                        'NON DEVO DECODIFICARE!
                        strXmlVariabilistampe = Me.TxtRisultato.Text

                        'modifica del 30/05/2011: spostata prima della valorizzazione della variabile di sessione
                        'elimino il carattere MAGA poiché devo caricare la stringa nel documento xml
                        strXmlVariabilistampe = Mid(strXmlVariabilistampe, 5)

                        'Salvo nella variabile di sessione la stringa xml ricevuta dal sito GiasOnLine
                        'in modo che tutte la altre pagine possano ricavare i parametri
                        Session("strXmlVariabilistampe") = strXmlVariabilistampe



                    Else
                        '---------------------------------------------
                        'CASO NORMALE:
                        '---------------------------------------------

                        'tutti gli altri casi

                        strXmlVariabilistampe = Stringa_Decodifica_Nuova(TxtRisultato.Text, AgroKey_EncoderDecoder, Server)

                        'Salvo nella variabile di sessione la stringa xml ricevuta dal sito GiasOnLine
                        'in modo che tutte la altre pagine possano ricavare i parametri
                        Session("strXmlVariabilistampe") = strXmlVariabilistampe

                    End If


                Else

                    Response.Redirect("Messaggi/AccessoNegato.htm")

                End If


            End If

            'devo inserire anche nella querystring 
            'la stringa connessione al SuperServer, 
            'altrimenti il sito chiamato (che non ha nel WebConfig le chiavi per il SuperServer)
            'non è in grado di leggere l'xml di passaggio parametri
            'che è salvato sul db cliente.
            'quindi tramite la stringa SuperServer e l'id cn_server può
            'recuperare la stringa connessione per il db cliente e leggere xml
            'Senza SuperServer andava a leggere sempre da connessione.ini che ora non deve 
            'esser più utilizzato
            Dim objGestioneRichieste As GestioneRichiesteClasse
            If Request.QueryString("StrConSup") IsNot Nothing AndAlso Request.QueryString("StrConSup") <> "" Then
                Dim StringaConnessioneSuperserver As String = ""
                StringaConnessioneSuperserver = Stringa_Decodifica(Request.QueryString("StrConSup").ToString,
                                                                   AgroKey_EncoderDecoder, Server)

                objGestioneRichieste = New GestioneRichiesteClasse(Unid,
                                                                   Cn_Server,
                                                                   StringaConnessioneSuperserver,
                                                                   Server.MapPath("AB_Immagini/IconeVegetali").ToString)

            ElseIf Unid <> "" Then

                'modalità senza SuperServer
                objGestioneRichieste = New GestioneRichiesteClasse(Unid, Cn_Server, "",
                                                                   Server.MapPath("AB_Immagini/IconeVegetali").ToString)

                Dim inizializza As New AgronicaCoreGestioneRichieste.Inizializzatore
                Dim StringaConnessioneServer As String = Session("ASG_StringaConnessione_Server")
                Dim StringaConnessioneUtenti As String = Session("ASG_StringaConnessione_Utenti")
                inizializza.Crea_ObjParametri_Server_E_ObjParametri_Utenti_E_Salva_In_Sessione(Session, StringaConnessioneServer, StringaConnessioneUtenti)

            Else

                '--------------------------------------------------------------------------
                '----- CHIAMATA DAL LAN    ------------------------------------------------
                '--------------------------------------------------------------------------

                '--------------------------------------------------------------------------
                '----- Definizione delle variabili di SESSIONE ----------------------------
                '--------------------------------------------------------------------------
                Session("Pagina_Messaggio") = ""
                Session("PartitaIVA") = ""
                Session("AlberoImprese_SaCod") = ""
                'Session("permessoDPI") = CBool(ConfigurationSettings.AppSettings("Flag_DisciplinareAttivo"))
                'Session("permessoDPIPrivati") = CBool(ConfigurationSettings.AppSettings("Flag_DisciplinarePrivato"))
                Session("ASG_Utente_Username") = ""
                Session("ASG_Utente_Password") = ""
                Session("ASG_Utente_Username_Crypt") = ""
                Session("ASG_Utente_Password_Crypt") = ""
                Session("ASG_Utente_CodFiscale") = ""
                Session("ASG_SuperUser_Username") = ""
                Session("ASG_SuperUser_Password") = ""
                Session("ASG_SuperUser_Username_Crypt") = ""
                Session("ASG_SuperUser_Password_Crypt") = ""
                Session("ASG_SuperUser_CodFiscale") = ""
                Session("ASG_ProgressivoGIAS") = ""
                Session("ASG_PathFileINI") = ""
                Session("ASG_IdServizio") = ""
                Session("ASG_Connessione_Server") = ""
                Session("ASG_Connessione_Tabelle") = ""
                Session("ASG_Connessione_Utenti") = ""
                Session("ASG_Connessione_DPI") = ""
                Session("ASG_Connessione_LOG") = ""
                Session("ASG_FinestraTemporale_Inizio") = ""
                Session("ASG_FinestraTemporale_Fine") = ""
                '--------------------------------------------------------------------------
                '----- Inizializzazione delle variabili globali in funzione del sito ------
                '--------------------------------------------------------------------------
                Dim iniz As New AgronicaCoreGestioneRichieste.Inizializzatore
                iniz.Inizializza_Sito_Specifico_Versione_Standard(Session)

            End If

            Dim Qs_strFiltro_Sql As String
            Dim Qs_strFiltro_Xml As String



            If Request.Form("TxtVariabiliStampe") IsNot Nothing Then
                '------------------- ATTENZIONE!!!!!!!!!!!!!!!!! ---------------------------
                'QUI PASSA LA CHIAMATA DAL GIASLAN!!!!!
                'NON MODIFICARE IL CODICE
                '----------------------------------------------------------------------------

                TxtRisultato.Text = Request.Form("TxtVariabiliStampe").ToString()

                'tutti gli altri casi
                strXmlVariabilistampe = Stringa_Decodifica_Nuova(TxtRisultato.Text, AgroKey_EncoderDecoder, Server)

                'Salvo nella variabile di sessione la stringa xml ricevuta dal sito GiasOnLine
                'in modo che tutte la altre pagine possano ricavare i parametri
                Session("strXmlVariabilistampe") = strXmlVariabilistampe


                Dim ParametriAgronicaStampe As New ParametriAgronicaStampe("<Parametri>" & strXmlVariabilistampe & "</Parametri>")
                'ParametriAgronicaStampe.Leggi(strXmlVariabilistampe)

                id_agenda = ParametriAgronicaStampe.Id_Agenda


                'controllare se uguali
                Dim stringa As String
                stringa = ParametriAgronicaStampe.GeneraXML()

                Qs_Report = ParametriAgronicaStampe.report
                Session("ASG_Utente_Username") = ParametriAgronicaStampe.username
                Session("ASG_ProgressivoGIAS") = ParametriAgronicaStampe.user_profilo
                '---------------------------------------------------------------

                'inserisco nel valore querystring il valore sql_filtro che non viene passato nella query nella versione nuova
                If Not IsNothing(ParametriAgronicaStampe.Sql_Filtro) Then
                    Qs_strFiltro_Sql = ParametriAgronicaStampe.Sql_Filtro
                End If
                Session("Sql_Filtro") = Qs_strFiltro_Sql

                If Not IsNothing(ParametriAgronicaStampe.Xml_Filtro) Then
                    Qs_strFiltro_Xml = ParametriAgronicaStampe.Xml_Filtro
                End If
                Session("Xml_Filtro") = Qs_strFiltro_Xml

                'dato che i parametri Sql_Filtro e XML_Filtro possono avere dei caratteri particolari non gestiti da xml
                'la classe ParametriAgronicaStampe gestisce nella get e nella set questio casi sostituendoli
                'se ricavo la stringa xml perÃ², avrÃ  i caratteri sostituiti dato che 
                'questa funzione Ã¨ la stessa usata da scrivixml XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriAgronicaStampe.GeneraXML()
                'quindi devo cercarli e sostituirli
                ' NEL LAN NON CI SONO???????????? NICO
                'stringa = stringa.Replace("SQL_SAFE_CARATTERE_PERCENTUALE", "%")
                'stringa = stringa.Replace("SQL_SAFE_CARATTERE_APIVCE", "'")
                Session("strXmlVariabilistampe") = stringa

                strXmlVariabilistampe = ParametriAgronicaStampe.Xml_Generico.ToString()   'Session("strXmlVariabilistampe")


                '##############################################################################################
                '####################    CREAZIONE OBJPARAMETRI E VALORIZZAZIONE NELLA SESSIONE  ##############
                '##############################################################################################



                Dim Utente_Password As String
                Dim Utente_CodFiscale As String
                Dim Utente_NomeRagSoc As String
                Dim SuperUser_Username As String
                Dim SuperUser_Password As String
                Dim SuperUser_CodFiscale As String
                Dim SuperUser_NomeRagSoc As String
                Dim ProgressivoGIAS As Integer



                Try

                    'Dim uL As New AgronicaCoreUtentiDAL.Utenti_Profili_Write
                    RecuperaDati_Utente_SuperUser(ParametriAgronicaStampe.username,
                                                  Utente_Password,
                                                  Utente_CodFiscale,
                                                  Utente_NomeRagSoc,
                                                  SuperUser_Username,
                                                  SuperUser_Password,
                                                  SuperUser_CodFiscale,
                                                  SuperUser_NomeRagSoc,
                                                  ProgressivoGIAS,
                                                  CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri))

                Catch ex As Exception
                    Exit Sub
                End Try

                'testare
                Dim iniz As New AgronicaCoreGestioneRichieste.Inizializzatore
                iniz.Inizializza_Sito_Specifico_Versione_Standard(Session)
                'testare

                Dim objP_Server As AgronicaCoreParametri = CType(Session("ASG_objParametri_Server"), AgronicaCoreParametri)
                objP_Server.PivaSuperUser = SuperUser_CodFiscale
                objP_Server.SuperUserUsername = SuperUser_Username
                objP_Server.UtenteCodFiscale = Utente_CodFiscale
                objP_Server.UsernameOperazione = Utente_CodFiscale
                objP_Server.UtenteUsername = ParametriAgronicaStampe.username
                objP_Server.LogDescrizioneUtente = ParametriAgronicaStampe.username
                Session("ASG_objParametri_Server") = objP_Server


                Dim objP_Utenti As AgronicaCoreParametri = CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri)
                objP_Utenti.PivaSuperUser = SuperUser_CodFiscale
                objP_Utenti.SuperUserUsername = SuperUser_Username
                objP_Utenti.UtenteCodFiscale = Utente_CodFiscale
                objP_Utenti.UsernameOperazione = Utente_CodFiscale
                objP_Utenti.LogDescrizioneUtente = ParametriAgronicaStampe.username
                objP_Utenti.UtenteUsername = ParametriAgronicaStampe.username
                Session("ASG_objParametri_Utenti") = objP_Utenti

            ElseIf Not IsNothing(Session("ParametriAgronicaStampe_2010")) Then

                '------------------- ATTENZIONE!!!!!!!!!!!!!!!!! ---------------------------
                'CHIAMATA DA GIASONLINE 2003, AGRONICA STAMPE 2003
                '----------------------------------------------------------------------------

                Dim ParametriAgronicaStampe_2010 As New ParametriAgronicaStampe_2010
                ParametriAgronicaStampe_2010.Leggi()
                id_agenda = ParametriAgronicaStampe_2010.id_agenda
                'controllare se uguali
                Dim stringa As String
                stringa = ParametriAgronicaStampe_2010.GeneraXML()


                'inserisco nel valore querystring il valore sql_filtro che non viene passato nella query nella versione nuova
                Qs_strFiltro_Sql = ParametriAgronicaStampe_2010.Sql_Filtro
                Session("Sql_Filtro") = Qs_strFiltro_Sql

                Qs_strFiltro_Xml = ParametriAgronicaStampe_2010.Xml_Filtro
                Session("Xml_Filtro") = Qs_strFiltro_Xml

                Qs_Report = ParametriAgronicaStampe_2010.report

                'dato che i parametri Sql_Filtro e XML_Filtro possono avere dei caratteri particolari non gestiti da xml
                'la classe ParametriAgronicaStampe_2010 gestisce nella get e nella set questi casi sostituendoli
                'se ricavo la stringa xml però, avrà i caratteri sostituiti dato che 
                'questa funzione è la stessa usata da scrivixml XML_Parametri.InnerXml = XML_Parametri.InnerXml & ParametriAgronicaStampe_2010.GeneraXML()
                'quindi devo cercarli e sostituirli
                stringa = stringa.Replace("SQL_SAFE_CARATTERE_PERCENTUALE", "%")
                stringa = stringa.Replace("SQL_SAFE_CARATTERE_APIVCE", "'")
                Session("strXmlVariabilistampe") = stringa

                '16/08/2017 - stampando dal LAN funziona solo usando la funzione xml_generico, AgriBologna necessita della sessione
                '       è necessario capire come fare
                strXmlVariabilistampe = ParametriAgronicaStampe_2010.Xml_Generico.ToString()   'Session("strXmlVariabilistampe")
                'strXmlVariabilistampe = Session("strXmlVariabilistampe")

            End If


            '==========================================
            '=========== LINGUE ==============
            '==========================================
            'controllo se è impostata una lingua 
            Dim ling As New Lingua
            If Request.QueryString("ln") IsNot Nothing Then
                Dim linguaCodiceISO As String() = Request.QueryString("ln").Split("|")
                ling.CodiceISO = linguaCodiceISO(0)
                ling.Lingua_cod = linguaCodiceISO(1)
                CType(Session("ASG_objParametri_Server"), AgronicaCoreParametri).Lingua_Cod = ling.Lingua_cod
                CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri).Lingua_Cod = ling.Lingua_cod

                ImpostaCultura(ling)
            Else
                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
                ling = objUtenti.Leggi_Lingua(CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri).UtenteUsername, "", "", CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri))
                CType(Session("ASG_objParametri_Server"), AgronicaCoreParametri).Lingua_Cod = ling.Lingua_cod
                CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri).Lingua_Cod = ling.Lingua_cod
                ImpostaCultura(ling)
            End If

            '-----------------------------------------------------------
            '---------- CONTROLLO DATI OBJPARAMETRI --------------
            '-----------------------------------------------------------
            Dim objP As New AgronicaCoreDataProvider.AgronicaCoreParametri
            objP.VerificaProprietaObjParametri(Session("ASG_objParametri_Server"), 0)
            objP.VerificaProprietaObjParametri(Session("ASG_objParametri_Utenti"), 1)

        Catch ex As Exception
            UtilityProvider.AgroMsgBox("GestioneRichieste.aspx: " & ex.Message, Page)
            Exit Sub
        End Try

        '=======================================================
        '== leggo le variabilistampe e le carico nell'hashtable =========
        '=======================================================
        Dim htVariabiliStampe As System.Collections.Hashtable
        Dim strerr As String = ""
        Dim objXmlStampe As New AgronicaCoreXML.XML_Stampe
        '10/03/2017: non veniva passato strerr
        'objXmlStampe.XML_EstraiVariabiliStampe(strXmlVariabilistampe, htVariabiliStampe, "")
        objXmlStampe.XML_EstraiVariabiliStampe2(strXmlVariabilistampe, htVariabiliStampe, strerr)
        If strerr <> "" Then
            'si è verificato un errore, sarebbe meglio interrompere e mostrarlo,
            'così si capisce cosa è successo
            'non lo faccio per evitare che in alcuni casi sia voluto
            'salvo allora nel log
            '(succede ad esempio nella stampa scheda di campagna chiamata dal giaslan)
            '17/03/2020: tentativo di scrivere nella cartella di default dei log dell'objparametri
            Dim objcoreDP As New AgronicaCoreDataProvider.LogProvider
            Dim dirLog As String = ""
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing
            Try
                objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
                dirLog = objParametri_Server.LogDirectory
            Catch ex As Exception

            End Try

            Dim CustomLOGParams As New AgronicaCoreDataProvider.CustomLOGParams With {
                .LogDescrizioneUtente = "",
                .LogDirectory = dirLog,
                .LogFileName = "GestioneRichieste_Stampe.txt"
            }
            objcoreDP.Scrivi_LOG(objParametri_Server, "XML_EstraiVariabiliStampe2", strerr, CustomLOGParams:=CustomLOGParams)
        End If

        '=======================================================
        '========= GESTIONE STAMPE PERSONALIZZATE ==============
        '=======================================================
        Dim URL_StampaPersonalizzata As String = ""
        Dim str_ApriFiltroFattura As String = ""
        Dim str_ApriFiltroDDT As String = ""
        Dim str_ApriFiltroZootecnia As String = ""
        Dim objUtentiImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        '2016/06/14
        'Se nelle opzioni è stato indicato di Aprire la stampa di pre-filtro della fattura o ddt (stampa personalizzata),
        'apro la pagina di filtro che è la stessa per fatture o ddt, però
        'nei 2 casi possono essere visualizzate opzioni diverse a seconda delle personalizzazioni utente. 
        'In futuro le personalizzazioni verranno definite in apposita tabella 

        Try

            Select Case Qs_Report

                Case enum_CodificaStampe.Fatture,
                        enum_CodificaStampe.Nota_Accredito,
                        enum_CodificaStampe.Ordine,
                        enum_CodificaStampe.Ordine_Acquisto,
                        enum_CodificaStampe.Preventivo_Vendita

                    str_ApriFiltroFattura = objUtentiImp.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SuperUser_ApriFiltroFattura,
                                                                                         CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri), 2)

                    If str_ApriFiltroFattura = "1" Then
                        URL_StampaPersonalizzata = PaginaLinkFiltroFatturaDDT
                    End If

                    '------------------------------

                Case enum_CodificaStampe.DDT_Contabilizzato_Emesso,
                    enum_CodificaStampe.Bolle

                    str_ApriFiltroDDT = objUtentiImp.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SuperUser_ApriFiltroDDT,
                                                                                   CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri), 2)


                    If str_ApriFiltroDDT = "1" Then
                        URL_StampaPersonalizzata = PaginaLinkFiltroFatturaDDT
                    End If

                Case enum_CodificaStampe.Stampa_Zoo_Sintesi_Partite,
                    enum_CodificaStampe.Stampa_Zoo_Dettaglio_Partita

                    'str_ApriFiltroZootecnia = objUtentiImp.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SuperUser_ApriFiltroDDT,
                    '                                                               CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri), 2)
                    'str_ApriFiltroZootecnia = "1"

                    If True Then
                        URL_StampaPersonalizzata = PaginaLinkFiltroZootecnia
                    End If

            End Select

        Catch ex As Exception
            Dim objcoreDP As New AgronicaCoreDataProvider.LogProvider
            Dim dirLog As String = ""
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing
            Try
                objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
                dirLog = objParametri_Server.LogDirectory
            Catch ex2 As Exception
            End Try


            Dim CustomLOGParams As New AgronicaCoreDataProvider.CustomLOGParams With {
                .LogDescrizioneUtente = "",
                .LogDirectory = dirLog,
                .LogFileName = "GestioneRichieste_Stampe.txt"
            }
            objcoreDP.Scrivi_LOG(objParametri_Server, "impostazioni per apertura pagina pre stampa", ex.Message, CustomLOGParams:=CustomLOGParams)
        End Try

        '==========================================
        '=========== REDIRECT ==============
        '==========================================

        GiasBaseHelper.WarmUp_GestioneRichieste()

        If Qs_Report <> enum_CodificaStampe.Nessuna Then
            Gestione_Redirect(htVariabiliStampe, Qs_Report, id_agenda, URL_StampaPersonalizzata)
        End If

    End Sub

    '################################################################################
    Private Sub Gestione_Redirect(ByVal htVariabiliStampe As System.Collections.Hashtable,
                                  ByVal Report As enum_CodificaStampe,
                                  ByVal id_agenda As Integer,
                                  ByVal URL_StampaPersonalizzata As String)

        Dim TargetURL, QueryString, URL_Stampa As String

        'spostato nel load
        '' leggo le variabilistampe e le carico nell'hashtable
        'Dim htVariabiliStampe As System.Collections.Hashtable

        'Dim objXmlStampe As New AgronicaCoreXML.XML_Stampe
        'objXmlStampe.XML_EstraiVariabiliStampe(strXmlVariabilistampe, htVariabiliStampe, "")

        ' per portare dietro l'azienda selezionata in agenda
        If htVariabiliStampe IsNot Nothing AndAlso Not IsNothing(htVariabiliStampe("piva")) Then
            'Reimposto l'oggetto in sessione per fare in modo che il costruttore della classe ParametriAgenda lo re-inizializzi
            'perché la pagina GestioneRichieste è l'entry-point del sito
            If Not IsNothing(System.Web.HttpContext.Current) AndAlso Not IsNothing(System.Web.HttpContext.Current.Session) Then
                System.Web.HttpContext.Current.Session("ParametriAgenda") = Nothing
            End If

            Dim objParametriAgenda As New ParametriAgenda
            objParametriAgenda.Piva = CStr(htVariabiliStampe("piva"))
            objParametriAgenda.salva()
        End If

        '/////////////////////////////////////////////////
        '//////////////// REPORT /////////////////////////
        '/////////////////////////////////////////////////

        'TODO: chiedere a Margherita perché non è stato riportato questo in session...?
        HttpContext.Current.Session("ReportSelezionato") = Report

        Select Case Report

            Case enum_CodificaStampe.Esporta_GiasToSap

                If URL_StampaPersonalizzata <> "" Then
                    URL_Stampa = URL_StampaPersonalizzata
                Else
                    URL_Stampa = PaginaLinkEsportazioneGiasToSap
                End If

                TargetURL = URL_Stampa

                '############################################################################

            Case enum_CodificaStampe.Fatture,
                enum_CodificaStampe.Nota_Accredito,
                enum_CodificaStampe.Ordine,
                enum_CodificaStampe.Ordine_Acquisto,
                enum_CodificaStampe.Preventivo_Vendita


                If URL_StampaPersonalizzata <> "" Then
                    URL_Stampa = URL_StampaPersonalizzata
                Else
                    URL_Stampa = PaginaLinkStampeFatturaNotaAccredito
                End If


                TargetURL = URL_Stampa &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&i=" & Stringa_Codifica(CStr(htVariabiliStampe("id_agenda")), AgroKey_EncoderDecoder, Server) &
                            "&l=" & Stringa_Codifica(CStr(htVariabiliStampe("printcode")), AgroKey_EncoderDecoder, Server) &
                            "&r=" & Stringa_Codifica(CStr(htVariabiliStampe("printtoprinter")), AgroKey_EncoderDecoder, Server) &
                            "&a=" & Stringa_Codifica(CStr(htVariabiliStampe("printname")), AgroKey_EncoderDecoder, Server) &
                            "&rep=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

                If htVariabiliStampe("preview") IsNot Nothing AndAlso htVariabiliStampe("preview") <> "" Then
                    TargetURL &= "&prvw=" & Stringa_Codifica(CStr(htVariabiliStampe("preview")), AgroKey_EncoderDecoder, Server)
                End If


                '############################################################################

            Case enum_CodificaStampe.DDT_Contabilizzato_Emesso,
                enum_CodificaStampe.Bolle


                'Verifica Tipologia clienti Cantine/Fresh&Food/ecc
                Dim objOmni As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                Dim Modulo_Cantine As Boolean = False
                Dim Modulo_FreshFood As Boolean = False
                Dim Modulo_Tabacco As Boolean = False
                Dim Modulo_Zoo As Boolean = False

                objOmni.Recupera_Modulo_Cliente(CStr(htVariabiliStampe("piva")),
                                                Modulo_Cantine,
                                                Modulo_FreshFood,
                                                Modulo_Tabacco,
                                                Modulo_Zoo,
                                                Session("ASG_objParametri_Server"))

                '  Giulia, 10/01/2017 11.53.40: se F&F anche se fosse ddt con dettagli economici, passo cmq dalla stampa del DDT e non della fattura
                If Modulo_FreshFood Then

                    If URL_StampaPersonalizzata <> "" Then
                        URL_Stampa = URL_StampaPersonalizzata
                    Else
                        URL_Stampa = PaginaLinkStampeDDTBollaConferimento
                    End If

                    TargetURL = URL_Stampa &
                                "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                                "&i=" & Stringa_Codifica(CStr(htVariabiliStampe("id_agenda")), AgroKey_EncoderDecoder, Server) &
                                "&l=" & Stringa_Codifica(CStr(htVariabiliStampe("printcode")), AgroKey_EncoderDecoder, Server) &
                                "&r=" & Stringa_Codifica(CStr(htVariabiliStampe("printtoprinter")), AgroKey_EncoderDecoder, Server) &
                                "&a=" & Stringa_Codifica(CStr(htVariabiliStampe("printname")), AgroKey_EncoderDecoder, Server) &
                                "&rep=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                                "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)
                Else

                    Dim objMov As New AgronicaCoreContabDAL.Movimenti_R
                    Dim checkPrezzo As Integer = 0
                    Dim Lav_Cod As Integer = htVariabiliStampe("printcode")

                    checkPrezzo = objMov.ChkLayOutPrezzo_from_id_Agenda(CStr(htVariabiliStampe("piva")),
                                                                        CStr(htVariabiliStampe("id_agenda")),
                                                                        CStr(htVariabiliStampe("printcode")),
                                                                        "",
                                                                        Session("ASG_objParametri_Server"))
                    objMov = Nothing

                    If checkPrezzo = 1 AndAlso Lav_Cod <> LAVCOD_CONFERIMENTO AndAlso Lav_Cod <> LAVCOD_CONFERIMENTO_DIVERSI Then

                        If URL_StampaPersonalizzata <> "" Then
                            URL_Stampa = URL_StampaPersonalizzata
                        Else
                            URL_Stampa = PaginaLinkStampeFatturaNotaAccredito
                        End If

                        TargetURL = URL_Stampa &
                                    "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                                    "&i=" & Stringa_Codifica(CStr(htVariabiliStampe("id_agenda")), AgroKey_EncoderDecoder, Server) &
                                    "&l=" & Stringa_Codifica(CStr(htVariabiliStampe("printcode")), AgroKey_EncoderDecoder, Server) &
                                    "&r=" & Stringa_Codifica(CStr(htVariabiliStampe("printtoprinter")), AgroKey_EncoderDecoder, Server) &
                                    "&a=" & Stringa_Codifica(CStr(htVariabiliStampe("printname")), AgroKey_EncoderDecoder, Server) &
                                    "&rep=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                                    "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

                        If htVariabiliStampe("preview") IsNot Nothing AndAlso htVariabiliStampe("preview") <> "" Then
                            TargetURL &= "&prvw=" & Stringa_Codifica(CStr(htVariabiliStampe("preview")), AgroKey_EncoderDecoder, Server)
                        End If

                    Else

                        If URL_StampaPersonalizzata <> "" Then
                            URL_Stampa = URL_StampaPersonalizzata
                        Else
                            URL_Stampa = PaginaLinkStampeDDTBollaConferimento
                        End If

                        TargetURL = URL_Stampa &
                                    "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                                    "&i=" & Stringa_Codifica(CStr(htVariabiliStampe("id_agenda")), AgroKey_EncoderDecoder, Server) &
                                    "&l=" & Stringa_Codifica(CStr(htVariabiliStampe("printcode")), AgroKey_EncoderDecoder, Server) &
                                    "&r=" & Stringa_Codifica(CStr(htVariabiliStampe("printtoprinter")), AgroKey_EncoderDecoder, Server) &
                                    "&a=" & Stringa_Codifica(CStr(htVariabiliStampe("printname")), AgroKey_EncoderDecoder, Server) &
                                    "&rep=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                                    "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

                    End If

                End If


                '############################################################################

            Case enum_CodificaStampe.Bolle_Conferimento_Soci,
                enum_CodificaStampe.Bolle_Conferimento_Diversi ',
                'enum_CodificaStampe.Buono_Accettazione_Diversi_DDTRicevuto

                TargetURL = PaginaLinkStampeDDTBollaConferimento &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&i=" & Stringa_Codifica(CStr(htVariabiliStampe("id_agenda")), AgroKey_EncoderDecoder, Server) &
                            "&l=" & Stringa_Codifica(CStr(htVariabiliStampe("printcode")), AgroKey_EncoderDecoder, Server) &
                            "&r=" & Stringa_Codifica(CStr(htVariabiliStampe("printtoprinter")), AgroKey_EncoderDecoder, Server) &
                            "&a=" & Stringa_Codifica(CStr(htVariabiliStampe("printname")), AgroKey_EncoderDecoder, Server) &
                            "&rep=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

                '############################################################################

                'Case enum_CodificaStampe.Buono_Accettazione_Diversi
                '      l'enum stampe della stampa della bolla di accettazione di fruttagel è Case enum_CodificaStampe.Buono_Accettazione_Diversi
                '      ma rimarrà solo nelle stampe 2003
                '       la stampa nuova arriverà con l'enum stampa della bolla standard f&f

                'Const PaginaLinkStampeBollaAccettazione = "GestioneStampe/Contabilita/Fattura/BollaAccettazione.aspx"
                '    TargetURL = PaginaLinkStampeBollaAccettazione &
                '                "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                '                "&i=" & Stringa_Codifica(CStr(htVariabiliStampe("id_agenda")), AgroKey_EncoderDecoder, Server) &
                '                "&l=" & Stringa_Codifica(CStr(htVariabiliStampe("printcode")), AgroKey_EncoderDecoder, Server) &
                '                "&r=" & Stringa_Codifica(CStr(htVariabiliStampe("printtoprinter")), AgroKey_EncoderDecoder, Server) &
                '                "&a=" & Stringa_Codifica(CStr(htVariabiliStampe("printname")), AgroKey_EncoderDecoder, Server) &
                '                "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

                                '############################################################################

            Case enum_CodificaStampe.Conf_Certificato_Pomodoro

                TargetURL = PaginaLinkStampeCertificatoPomodoro &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&i=" & Stringa_Codifica(CStr(htVariabiliStampe("id_agenda")), AgroKey_EncoderDecoder, Server) &
                            "&l=" & Stringa_Codifica(CStr(htVariabiliStampe("printcode")), AgroKey_EncoderDecoder, Server) &
                            "&r=" & Stringa_Codifica(CStr(htVariabiliStampe("printtoprinter")), AgroKey_EncoderDecoder, Server) &
                            "&a=" & Stringa_Codifica(CStr(htVariabiliStampe("printname")), AgroKey_EncoderDecoder, Server) &
                            "&rep=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)


                '############################################################################

            Case enum_CodificaStampe.FreshFood_BollaAccettazione,
                enum_CodificaStampe.FreshFood_AutoDDT_Accettazione,
                enum_CodificaStampe.FreshFood_DistintaCarico_Accettazione

                'l'enum stampe della stampa della bolla di accettazione di fruttagel è Case enum_CodificaStampe.Buono_Accettazione_Diversi
                'ma rimarrà solo nelle stampe 2003, la stampa nuova arriverà con l'enum stampa della bolla standard f&f
                'tramite la tabella configurazione_Stampe si verrà cmq rediretti su "GestioneStampe/Contabilita/Fattura/BollaAccettazione.aspx"
                '(che è la pagina dedicata alla loro bolla)

                Dim _objParametriServer As AgronicaCoreParametri = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))

                Dim objConfStampe As New AgronicaCoreStampeDAL.Configurazione_Stampe_R
                URL_StampaPersonalizzata = objConfStampe.URLaspx_from_enum_CodificaStampe(CStr(htVariabiliStampe("piva")), Report, _objParametriServer)

                If URL_StampaPersonalizzata <> "" Then
                    URL_Stampa = URL_StampaPersonalizzata
                Else
                    URL_Stampa = PaginaLinkStampeBollaFreshFood
                End If

                TargetURL = URL_Stampa &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&i=" & Stringa_Codifica(CStr(htVariabiliStampe("id_agenda")), AgroKey_EncoderDecoder, Server) &
                            "&l=" & Stringa_Codifica(CStr(htVariabiliStampe("printcode")), AgroKey_EncoderDecoder, Server) &
                            "&r=" & Stringa_Codifica(CStr(htVariabiliStampe("printtoprinter")), AgroKey_EncoderDecoder, Server) &
                            "&a=" & Stringa_Codifica(CStr(htVariabiliStampe("printname")), AgroKey_EncoderDecoder, Server) &
                            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

                '############################################################################

            Case enum_CodificaStampe.FreshFood_BollaCampionatura

                TargetURL = PaginaLinkStampeBollaCampionaturaFreshFood &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&idm=" & Stringa_Codifica(CStr(htVariabiliStampe("idmovdet")), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.FreshFood_FatturaLiquidazioneSoci,
                enum_CodificaStampe.FreshFood_AutofatturaLiquidazioneSoci

                TargetURL = PaginaLinkStampeFatturaLiquidazioneSociFreshFood &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&rep=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                            "&liq=" & Stringa_Codifica(CStr(htVariabiliStampe("idliq")), AgroKey_EncoderDecoder, Server) &
                            "&f=" & Stringa_Codifica(CStr(htVariabiliStampe("risum")), AgroKey_EncoderDecoder, Server) &
                            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.FreshFood_RiepilogoLiquidazioneSoci

                TargetURL = PaginaLinkStampeRiepilogoLiquidazioneSociFreshFood &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&liq=" & Stringa_Codifica(CStr(htVariabiliStampe("idliq")), AgroKey_EncoderDecoder, Server) &
                            "&f=" & Stringa_Codifica(CStr(htVariabiliStampe("risum")), AgroKey_EncoderDecoder, Server) &
                            "&g=" & Stringa_Codifica(CStr(htVariabiliStampe("grfatt")), AgroKey_EncoderDecoder, Server) &
                            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.RiepiloghiAccise, enum_CodificaStampe.DAA_GaranzieCircolanti, enum_CodificaStampe.DAA_PariteSospensione

                TargetURL = PaginaLinkFiltroReportDAA &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&di=" & Stringa_Codifica(CStr(htVariabiliStampe("data_inizio")), AgroKey_EncoderDecoder, Server) &
                            "&da=" & Stringa_Codifica(CStr(htVariabiliStampe("data_fine")), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.MVV
                TargetURL = PaginaLinkStampaMVV &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&i=" & Stringa_Codifica(CStr(htVariabiliStampe("id_agenda")), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.Report_Vendita_PDF
                TargetURL = PaginaLinkStampaStatistica12Mesi &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("p")), AgroKey_EncoderDecoder, Server) &
                            "&numSin=" & Stringa_Codifica(CStr(htVariabiliStampe("numSin")), AgroKey_EncoderDecoder, Server) &
                            "&num=" & Stringa_Codifica(CStr(htVariabiliStampe("num")), AgroKey_EncoderDecoder, Server) &
                            "&numoDes=" & Stringa_Codifica(CStr(htVariabiliStampe("numoDes")), AgroKey_EncoderDecoder, Server) &
                            "&nrRiga=" & Stringa_Codifica(CStr(htVariabiliStampe("nrRiga")), AgroKey_EncoderDecoder, Server) &
                            "&dataDal=" & Stringa_Codifica(CStr(htVariabiliStampe("dataDal")), AgroKey_EncoderDecoder, Server) &
                            "&dataAl=" & Stringa_Codifica(CStr(htVariabiliStampe("dataAl")), AgroKey_EncoderDecoder, Server) &
                            "&cli=" & Stringa_Codifica(CStr(htVariabiliStampe("cli")), AgroKey_EncoderDecoder, Server) &
                            "&age=" & Stringa_Codifica(CStr(htVariabiliStampe("age")), AgroKey_EncoderDecoder, Server) &
                            "&causali=" & Stringa_Codifica(CStr(htVariabiliStampe("causali")), AgroKey_EncoderDecoder, Server) &
                            "&spe=" & Stringa_Codifica(CStr(htVariabiliStampe("spe")), AgroKey_EncoderDecoder, Server) &
                            "&var=" & Stringa_Codifica(CStr(htVariabiliStampe("var")), AgroKey_EncoderDecoder, Server) &
                            "&prod=" & Stringa_Codifica(CStr(htVariabiliStampe("prod")), AgroKey_EncoderDecoder, Server) &
                            "&categ=" & Stringa_Codifica(CStr(htVariabiliStampe("categ")), AgroKey_EncoderDecoder, Server) &
                            "&categcommerciali=" & Stringa_Codifica(CStr(htVariabiliStampe("categcommerciali")), AgroKey_EncoderDecoder, Server) &
                            "&cau_trasp=" & Stringa_Codifica(CStr(htVariabiliStampe("cau_trasp")), AgroKey_EncoderDecoder, Server) &
                            "&tipo_report=" & Stringa_Codifica(CStr(htVariabiliStampe("tipo_report")), AgroKey_EncoderDecoder, Server) &
                            "&titolo=" & Stringa_Codifica(CStr(htVariabiliStampe("titolo")), AgroKey_EncoderDecoder, Server) &
                            "&liv=" & Stringa_Codifica(CStr(htVariabiliStampe("liv")), AgroKey_EncoderDecoder, Server) &
                            "&da_mese=" & Stringa_Codifica(CStr(htVariabiliStampe("da_mese")), AgroKey_EncoderDecoder, Server) &
                            "&da_anno=" & Stringa_Codifica(CStr(htVariabiliStampe("da_anno")), AgroKey_EncoderDecoder, Server) &
                            "&a_mese=" & Stringa_Codifica(CStr(htVariabiliStampe("a_mese")), AgroKey_EncoderDecoder, Server) &
                            "&a_anno=" & Stringa_Codifica(CStr(htVariabiliStampe("a_anno")), AgroKey_EncoderDecoder, Server) &
                            "&confr_anno=" & Stringa_Codifica(CStr(htVariabiliStampe("confr_anno")), AgroKey_EncoderDecoder, Server) &
                            "&tipo_val=" & Stringa_Codifica(CStr(htVariabiliStampe("tipo_val")), AgroKey_EncoderDecoder, Server) &
                            "&scost_tot=" & Stringa_Codifica(CStr(htVariabiliStampe("scost_tot")), AgroKey_EncoderDecoder, Server) &
                            "&saltopag_1liv=" & Stringa_Codifica(CStr(htVariabiliStampe("saltopag_1liv")), AgroKey_EncoderDecoder, Server) &
                            "&mese_prev_scost=" & Stringa_Codifica(CStr(htVariabiliStampe("mese_prev_scost")), AgroKey_EncoderDecoder, Server) &
                            "&analisi=" & Stringa_Codifica(CStr(htVariabiliStampe("analisi")), AgroKey_EncoderDecoder, Server) &
                            "&cb_ordin_x_valore=" & Stringa_Codifica(CStr(htVariabiliStampe("cb_ordin_x_valore")), AgroKey_EncoderDecoder, Server) &
                            "&rappCont=" & Stringa_Codifica(CStr(htVariabiliStampe("rapporticontabili")), AgroKey_EncoderDecoder, Server) &
                            "&tipo_val_2=" & Stringa_Codifica(CStr(htVariabiliStampe("tipo_val_2")), AgroKey_EncoderDecoder, Server) &
                            "&decimali_qta=" & Stringa_Codifica(CStr(htVariabiliStampe("decimali_qta")), AgroKey_EncoderDecoder, Server) &
                            "&includi_corr=" & Stringa_Codifica(CStr(htVariabiliStampe("includi_corr")), AgroKey_EncoderDecoder, Server)

                Context.Session("FiltroStatistiche_Nazioni_Fatturazione") = Stringa_Codifica(CStr(htVariabiliStampe("nazionifatturazione")), AgroKey_EncoderDecoder, Server)


            Case enum_CodificaStampe.FreshFood_PagatiSuCampionato

                TargetURL = PaginaLinkStampePagatiSuCampionatoFreshFood &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&liq=" & Stringa_Codifica(CStr(htVariabiliStampe("idliq")), AgroKey_EncoderDecoder, Server) &
                            "&f=" & Stringa_Codifica(CStr(htVariabiliStampe("risum")), AgroKey_EncoderDecoder, Server) &
                            "&g=" & Stringa_Codifica(CStr(htVariabiliStampe("grfatt")), AgroKey_EncoderDecoder, Server) &
                            "&s=" & Stringa_Codifica(CStr(htVariabiliStampe("specie")), AgroKey_EncoderDecoder, Server) &
                            "&v=" & Stringa_Codifica(CStr(htVariabiliStampe("var")), AgroKey_EncoderDecoder, Server) &
                            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.FreshFood_PagatiSuConferito

                TargetURL = PaginaLinkStampePagatiSuConferitoFreshFood &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&liq=" & Stringa_Codifica(CStr(htVariabiliStampe("idliq")), AgroKey_EncoderDecoder, Server) &
                            "&f=" & Stringa_Codifica(CStr(htVariabiliStampe("risum")), AgroKey_EncoderDecoder, Server) &
                            "&g=" & Stringa_Codifica(CStr(htVariabiliStampe("grfatt")), AgroKey_EncoderDecoder, Server) &
                            "&s=" & Stringa_Codifica(CStr(htVariabiliStampe("specie")), AgroKey_EncoderDecoder, Server) &
                            "&v=" & Stringa_Codifica(CStr(htVariabiliStampe("var")), AgroKey_EncoderDecoder, Server) &
                            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.Analisi_Progetti

                TargetURL = PaginaLinkStampeAnalisiProgetti &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("p")), AgroKey_EncoderDecoder, Server) &
                            "&b_c=" & Stringa_Codifica(CStr(htVariabiliStampe("b_c")), AgroKey_EncoderDecoder, Server) &
                            "&c_r=" & Stringa_Codifica(CStr(htVariabiliStampe("c_r")), AgroKey_EncoderDecoder, Server) &
                            "&l_r=" & Stringa_Codifica(CStr(htVariabiliStampe("l_r")), AgroKey_EncoderDecoder, Server) &
                            "&dettMacchine=" & Stringa_Codifica(CStr(htVariabiliStampe("dettmacchine")), AgroKey_EncoderDecoder, Server) &
                            "&dettPersone=" & Stringa_Codifica(CStr(htVariabiliStampe("dettpersone")), AgroKey_EncoderDecoder, Server) &
                            "&dettProdotti=" & Stringa_Codifica(CStr(htVariabiliStampe("dettprodotti")), AgroKey_EncoderDecoder, Server) &
                            "&dettNote=" & Stringa_Codifica(CStr(htVariabiliStampe("dettnote")), AgroKey_EncoderDecoder, Server) &
                            "&dettVarieta=" & Stringa_Codifica(CStr(htVariabiliStampe("dettvarieta")), AgroKey_EncoderDecoder, Server) &
                            "&sopprimiDettaglio=" & Stringa_Codifica(CStr(htVariabiliStampe("sopprimidettaglio")), AgroKey_EncoderDecoder, Server) &
                            "&dtDal=" & Stringa_Codifica(CStr(htVariabiliStampe("dtdal")), AgroKey_EncoderDecoder, Server) &
                            "&dtAl=" & Stringa_Codifica(CStr(htVariabiliStampe("dtal")), AgroKey_EncoderDecoder, Server) &
                            "&dtRifProgetto=" & Stringa_Codifica(CStr(htVariabiliStampe("dtrifprogetto")), AgroKey_EncoderDecoder, Server) &
                            "&filtro_codice_appezzamento=" & Stringa_Codifica(CStr(htVariabiliStampe("filtro_codice_appezzamento")), AgroKey_EncoderDecoder, Server) &
                            "&filtro_codice_impianto=" & Stringa_Codifica(CStr(htVariabiliStampe("filtro_codice_impianto")), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.ConferimentoUva_DDTRicevuto,
                enum_CodificaStampe.ConferimentoUva_DistintaCarico,
                enum_CodificaStampe.ConferimentoUva_AutoDDT

                If URL_StampaPersonalizzata <> "" Then
                    URL_Stampa = URL_StampaPersonalizzata
                Else
                    URL_Stampa = PaginaLinkStampeConferimentoUva
                End If

                TargetURL = URL_Stampa &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&i=" & Stringa_Codifica(CStr(htVariabiliStampe("id_agenda")), AgroKey_EncoderDecoder, Server) &
                            "&l=" & Stringa_Codifica(CStr(htVariabiliStampe("printcode")), AgroKey_EncoderDecoder, Server) &
                            "&r=" & Stringa_Codifica(CStr(htVariabiliStampe("printtoprinter")), AgroKey_EncoderDecoder, Server) &
                            "&a=" & Stringa_Codifica(CStr(htVariabiliStampe("printname")), AgroKey_EncoderDecoder, Server) &
                            "&rep=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

                '############################################################################

                '----------------------------------------------------------------------------------------
                '§§§ RICEVUTA FISCALE
                '----------------------------------------------------------------------------------------
            Case enum_CodificaStampe.RicevuteFiscali

                Dim tipo_report_ricevuta As enum_TipoStampaRicevutaFiscale
                Dim mode_preview As enum_StampaConSenzaPreview
                Dim PaginaLinkStampeRicevuta, stampante As String

                If Not IsNothing(CStr(htVariabiliStampe("tipo_report"))) AndAlso
                   CStr(htVariabiliStampe("tipo_report")) <> 0 Then
                    tipo_report_ricevuta = CStr(htVariabiliStampe("tipo_report"))
                Else
                    tipo_report_ricevuta = enum_TipoStampaRicevutaFiscale.PdfA4Singola
                End If

                If Not IsNothing(CStr(htVariabiliStampe("mode_preview"))) AndAlso
                   CStr(htVariabiliStampe("mode_preview")) <> "" Then
                    mode_preview = CStr(htVariabiliStampe("mode_preview"))
                Else
                    mode_preview = enum_StampaConSenzaPreview.ConPreview
                End If

                If Not IsNothing(CStr(htVariabiliStampe("stampante"))) AndAlso
                   CStr(htVariabiliStampe("stampante")) <> "" Then
                    stampante = CStr(htVariabiliStampe("stampante"))
                Else
                    stampante = ""
                End If

                Select Case tipo_report_ricevuta
                    Case enum_TipoStampaRicevutaFiscale.PdfA4Singola
                        PaginaLinkStampeRicevuta = PaginaLinkStampeRicevutaPDFa4
                    Case enum_TipoStampaRicevutaFiscale.PdfA5
                        PaginaLinkStampeRicevuta = PaginaLinkStampeRicevutaPDFa5
                    Case enum_TipoStampaRicevutaFiscale.WordA5Personalizzabile
                        PaginaLinkStampeRicevuta = PaginaLinkStampeRicevutaWord
                    Case enum_TipoStampaRicevutaFiscale.PdfA4ACapoAutomatico
                        PaginaLinkStampeRicevuta = PaginaLinkStampeRicevutaPDFa4aCapoAuto
                    Case Else
                        PaginaLinkStampeRicevuta = PaginaLinkStampeRicevutaPDFa4aCapoAuto
                End Select

                '''§§§ RICEVUTA FISCALE
                'PaginaLinkStampeRicevuta = PaginaLinkStampeRicevutaPDFa5
                'mode_preview = enum_StampaConSenzaPreview.SenzaPreview
                'stampante = "HP LaserJet P2015 Series (192.168.1.240)"

                TargetURL = PaginaLinkStampeRicevuta &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&i=" & Stringa_Codifica(CStr(htVariabiliStampe("id_agenda")), AgroKey_EncoderDecoder, Server) &
                            "&l=" & Stringa_Codifica(CStr(htVariabiliStampe("printcode")), AgroKey_EncoderDecoder, Server) &
                            "&ptp=" & Stringa_Codifica(mode_preview, AgroKey_EncoderDecoder, Server) &
                            "&pn=" & Stringa_Codifica(stampante, AgroKey_EncoderDecoder, Server) &
                            "&rep=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

                '############################################################################

                '----------------------------------------------------------------------------------------
                'DOCO
                '----------------------------------------------------------------------------------------
            Case enum_CodificaStampe.DOCO

                'richiamo la pagina di gestione delle fatture
                TargetURL = PaginaLinkStampeDoco &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&i=" & Stringa_Codifica(CStr(htVariabiliStampe("id_agenda")), AgroKey_EncoderDecoder, Server) &
                            "&l=" & Stringa_Codifica(CStr(htVariabiliStampe("printcode")), AgroKey_EncoderDecoder, Server) &
                            "&r=" & Stringa_Codifica(CStr(htVariabiliStampe("printtoprinter")), AgroKey_EncoderDecoder, Server) &
                            "&a=" & Stringa_Codifica(CStr(htVariabiliStampe("printname")), AgroKey_EncoderDecoder, Server) &
                            "&rp=" & Stringa_Codifica(CStr(enum_CodificaStampe.DOCO), AgroKey_EncoderDecoder, Server)

                '############################################################################

                '----------------------------------------------------------------------------------------
                'DAA
                '---------------------------------------------------------------------------------------
            Case enum_CodificaStampe.DAA

                'richiamo la pagina di gestione delle fatture
                TargetURL = PaginaLinkStampeDAA &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&i=" & Stringa_Codifica(CStr(htVariabiliStampe("id_agenda")), AgroKey_EncoderDecoder, Server) &
                            "&l=" & Stringa_Codifica(CStr(htVariabiliStampe("printcode")), AgroKey_EncoderDecoder, Server) &
                            "&r=" & Stringa_Codifica(CStr(htVariabiliStampe("printtoprinter")), AgroKey_EncoderDecoder, Server) &
                            "&a=" & Stringa_Codifica(CStr(htVariabiliStampe("printname")), AgroKey_EncoderDecoder, Server)

                '############################################################################

                '----------------------------------------------------------------------------------------
                'Registri di cantina
                '---------------------------------------------------------------------------------------
            Case enum_CodificaStampe.Registri_Preparazioni

                TargetURL = PaginaLinkStampeCantina &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&s=" & Stringa_Codifica(CStr(htVariabiliStampe("sa_cod")), AgroKey_EncoderDecoder, Server) &
                            "&i=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                            "&l=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                            "&r=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                            "&a=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                            "&rp=" & Stringa_Codifica(-1, AgroKey_EncoderDecoder, Server) &
                            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

                'TargetURL = PaginaLinkBrogliaccioMovimenti &
                '            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                '            "&s=" & Stringa_Codifica(CStr(htVariabiliStampe("sa_cod")), AgroKey_EncoderDecoder, Server) &
                '            "&i=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                '            "&l=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                '            "&r=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                '            "&a=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                '            "&rp=" & Stringa_Codifica(-1, AgroKey_EncoderDecoder, Server) &
                '            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)
                '============================================================

            Case enum_CodificaStampe.EtichetteVascheEnologiche

                'TargetURL = PaginaLinkEtichetteVascheEnologiche
                'CStr(htVariabiliStampe("piva"))
                'CStr(htVariabiliStampe("sa_cod"))

                TargetURL = PaginaLinkStampeCantina &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                            "&i=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                            "&l=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                            "&r=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                            "&a=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                            "&rp=" & Stringa_Codifica(enum_CodificaStampe.EtichetteVascheEnologiche, AgroKey_EncoderDecoder, Server)


                '############################################################################

                '--------------------------------------------------------------
                'BILANCI - INSOLUTI - REGISTRO FATTURE - REGISTRO CORRISPETTIVI
                '--------------------------------------------------------------
            Case enum_CodificaStampe.PianoDeiConti,
                enum_CodificaStampe.Bilancio_Civilistico,
                enum_CodificaStampe.Mastrino,
                enum_CodificaStampe.GiornaleContabile,
                enum_CodificaStampe.Bilanci_DiVerifica_Confronto,
                enum_CodificaStampe.Lista_InsolutiClienti,
                enum_CodificaStampe.Lista_InsolutiFornitori,
                enum_CodificaStampe.RiBa_Report_Presentazione,
                enum_CodificaStampe.EstrattoConto_Contatti,
                enum_CodificaStampe.Registro_FattureAcquisto,
                enum_CodificaStampe.Registro_FattureVendita,
                enum_CodificaStampe.Registro_Corrispettivi

                Dim Chk_CE As Integer = 0
                Dim Chk_SP As Integer = 0
                Dim Ric_Cod_Eco As Integer = 0
                Dim Ric_Cod_Pat As Integer = 0
                Dim Cod_Conto_Eco As Integer = 0
                Dim Cod_Conto_Pat As Integer = 0
                Dim Cod_RisUm As Integer = 0
                'è importante perché se no cerca Cod_Liquidita = 0 che è la cassa
                Dim Cod_Liquidita As Integer = CODLIQUIDITA_NOFILTRO
                Dim Data_Inizio, Data_fine As String

                If Not IsNothing(htVariabiliStampe("tipo_pianoconti")) Then
                    Select Case CStr(htVariabiliStampe("tipo_pianoconti")).ToUpper
                        Case "SP"
                            Chk_SP = 1
                        Case "CE"
                            Chk_CE = 1
                    End Select
                End If

                If Not IsNothing(htVariabiliStampe("ric_cod")) AndAlso IsNumeric(htVariabiliStampe("ric_cod")) Then
                    If Chk_SP = 1 Then
                        Ric_Cod_Pat = htVariabiliStampe("ric_cod")
                    Else
                        Ric_Cod_Eco = htVariabiliStampe("ric_cod")
                    End If
                End If

                If Not IsNothing(htVariabiliStampe("cod_conto")) AndAlso IsNumeric(htVariabiliStampe("cod_conto")) Then
                    If Chk_SP = 1 Then
                        Cod_Conto_Pat = htVariabiliStampe("cod_conto")
                    Else
                        Cod_Conto_Eco = htVariabiliStampe("cod_conto")
                    End If
                End If

                If Not IsNothing(htVariabiliStampe("tipo_cod")) AndAlso IsNumeric(htVariabiliStampe("tipo_cod")) Then
                    Select Case Cod_Conto_Pat
                        Case enum_Conti_Patrimoniali.DepositiBancariPostali,
                            enum_Conti_Patrimoniali.DenaroValoriInCassa
                            Cod_Liquidita = htVariabiliStampe("tipo_cod")
                        Case enum_Conti_Patrimoniali.CreditiVersoClienti,
                            enum_Conti_Patrimoniali.DebitiVersoFornitori
                            Cod_RisUm = htVariabiliStampe("tipo_cod")
                    End Select
                End If

                If Chk_SP = 1 OrElse Chk_CE = 1 Then
                    Data_Inizio = "01/01/" & CStr(CInt(htVariabiliStampe("anno")))
                    Data_fine = "31/12/" & CStr(CInt(htVariabiliStampe("anno")))
                Else
                    Data_Inizio = CStr(htVariabiliStampe("data_inizio"))
                    Data_fine = CStr(htVariabiliStampe("data_fine"))
                End If

                TargetURL = PaginaFiltroElaboratiContabili &
                            "?r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                            "&p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&rs=" & Stringa_Codifica(CStr(htVariabiliStampe("rag_soc")), AgroKey_EncoderDecoder, Server) &
                            "&a=" & Stringa_Codifica(CInt(htVariabiliStampe("anno")), AgroKey_EncoderDecoder, Server) &
                            "&di=" & Stringa_Codifica(Data_Inizio, AgroKey_EncoderDecoder, Server) &
                            "&df=" & Stringa_Codifica(Data_fine, AgroKey_EncoderDecoder, Server) &
                            "&chk_ce=" & Stringa_Codifica(Chk_CE, AgroKey_EncoderDecoder, Server) &
                            "&chk_sp=" & Stringa_Codifica(Chk_SP, AgroKey_EncoderDecoder, Server) &
                            "&rce=" & Stringa_Codifica(Ric_Cod_Eco, AgroKey_EncoderDecoder, Server) &
                            "&cce=" & Stringa_Codifica(Cod_Conto_Eco, AgroKey_EncoderDecoder, Server) &
                            "&rcp=" & Stringa_Codifica(Ric_Cod_Pat, AgroKey_EncoderDecoder, Server) &
                            "&ccp=" & Stringa_Codifica(Cod_Conto_Pat, AgroKey_EncoderDecoder, Server) &
                            "&cru=" & Stringa_Codifica(Cod_RisUm, AgroKey_EncoderDecoder, Server) &
                            "&liq=" & Stringa_Codifica(Cod_Liquidita, AgroKey_EncoderDecoder, Server) &
                            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

                '############################################################################

                'Case enum_CodificaStampe.Bilancio_DiVerifica,
                '         enum_CodificaStampe.Bilancio_Civilistico,
                '         enum_CodificaStampe.Mastrino,
                '         enum_CodificaStampe.Bilanci_DiVerifica_Confronto


                '    TargetURL = "GestioneStampe/Contabilita/Bilancio/Bilancio.aspx" &
                '                "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                '                "&r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                '                "&a=" & Stringa_Codifica(CStr(htVariabiliStampe("anno")), AgroKey_EncoderDecoder, Server) &
                '                "&di=" & Stringa_Codifica(CStr(htVariabiliStampe("Data_Inizio".ToLower)), AgroKey_EncoderDecoder, Server) &
                '                "&df=" & Stringa_Codifica(CStr(htVariabiliStampe("Data_Fine".ToLower)), AgroKey_EncoderDecoder, Server) &
                '                "&fc=" & Stringa_Codifica(CStr(htVariabiliStampe("Filtro_Conti".ToLower)), AgroKey_EncoderDecoder, Server) &
                '                "&chk_ce=" & Stringa_Codifica(CStr(htVariabiliStampe("Chk_CE".ToLower)), AgroKey_EncoderDecoder, Server) &
                '                "&chk_sp=" & Stringa_Codifica(CStr(htVariabiliStampe("Chk_SP".ToLower)), AgroKey_EncoderDecoder, Server) &
                '                "&ue=" & Stringa_Codifica(CStr(htVariabiliStampe("Conti_UE_Tutti".ToLower)), AgroKey_EncoderDecoder, Server) &
                '                "&mov=" & Stringa_Codifica(CStr(htVariabiliStampe("Conti_0Movimentati_1Tutti".ToLower)), AgroKey_EncoderDecoder, Server) &
                '                "&saldo=" & Stringa_Codifica(CStr(htVariabiliStampe("Conti_0ImponibileNoZero_1Tutti".ToLower)), AgroKey_EncoderDecoder, Server)

                '############################################################################

                '------------------------------------------------------
                'LIQUIDAZIONE IVA
                '------------------------------------------------------
            Case enum_CodificaStampe.LiquidazionePeriodica_IVA

                Dim NomePaginaAspx As String

                '/************* NUOVI ARROTONDAMENTI ***************************/
                Dim Flag_NuovaVersioneRound As Boolean
                Flag_NuovaVersioneRound = UsaNuoviArrotondamenti(Session("ASG_objParametri_Server"))

                If Flag_NuovaVersioneRound Then
                    NomePaginaAspx = PaginaLinkLiquidazioneIVA_NEW
                Else
                    NomePaginaAspx = PaginaLinkLiquidazioneIVA
                End If

                TargetURL = NomePaginaAspx &
                            "?r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                            "&p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&rs=" & Stringa_Codifica(CStr(htVariabiliStampe("rag_soc")), AgroKey_EncoderDecoder, Server) &
                            "&a=" & Stringa_Codifica(CInt(htVariabiliStampe("anno")), AgroKey_EncoderDecoder, Server) &
                            "&di=" & Stringa_Codifica(CStr(htVariabiliStampe("data_inizio")), AgroKey_EncoderDecoder, Server) &
                            "&df=" & Stringa_Codifica(CStr(htVariabiliStampe("data_fine")), AgroKey_EncoderDecoder, Server) &
                            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

                '############################################################################

                '------------------------------------------------------
                'MAGAZZINO
                '------------------------------------------------------
            Case enum_CodificaStampe.SchedaMagazzinoGiacenze,
                enum_CodificaStampe.SchedaMagazzinoMovimenti,
                enum_CodificaStampe.SchedaMagazzinoFertilizzanti,
                enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari,
                enum_CodificaStampe.RiepilogoProdottiUtilizzati

                Dim data_Stampa As String
                Dim Data_Inizio, Data_Fine As String

                If Not IsNothing(CStr(htVariabiliStampe("data_stampa"))) Then
                    data_Stampa = CStr(htVariabiliStampe("data_stampa"))
                Else
                    data_Stampa = Date.Today.ToShortDateString
                End If

                Select Case Date.Today.Month
                    Case 11, 12
                        'se sono nei mesi di novembre o dicembre..........
                        'l'annata agraria va dal 1/11 di quest'anno al 31/10 del prossimo
                        If Not IsNothing(CStr(htVariabiliStampe("data_inizio"))) Then
                            Data_Inizio = CStr(htVariabiliStampe("data_inizio"))
                        Else
                            Data_Inizio = "01/11/" & Now.Year
                        End If
                        If Not IsNothing(CStr(htVariabiliStampe("data_fine"))) Then
                            Data_Fine = CStr(htVariabiliStampe("data_fine"))
                        Else
                            Data_Fine = "31/10/" & Now.Year + 1
                        End If

                    Case Else
                        'l'annata agraria va dal 1/11 dell'anno scorso al 31/10 di quest'anno
                        If Not IsNothing(CStr(htVariabiliStampe("data_inizio"))) Then
                            Data_Inizio = CStr(htVariabiliStampe("data_inizio"))
                        Else
                            Data_Inizio = "01/11/" & Now.Year - 1
                        End If
                        If Not IsNothing(CStr(htVariabiliStampe("data_fine"))) Then
                            Data_Fine = CStr(htVariabiliStampe("data_fine"))
                        Else
                            Data_Fine = "31/10/" & Now.Year
                        End If

                End Select

                TargetURL = PaginaLinkStampaFiltroSchedeMagazzino &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&s=" & Stringa_Codifica(CStr(htVariabiliStampe("sa_cod")), AgroKey_EncoderDecoder, Server) &
                            "&f=" & Stringa_Codifica(CStr(htVariabiliStampe("fabbricato_cod")), AgroKey_EncoderDecoder, Server) &
                            "&e=" & Stringa_Codifica(CStr(htVariabiliStampe("elem_cod")), AgroKey_EncoderDecoder, Server) &
                            "&pro=" & Stringa_Codifica(CStr(htVariabiliStampe("pro_cod")), AgroKey_EncoderDecoder, Server) &
                            "&mat=" & Stringa_Codifica(CStr(htVariabiliStampe("mat_cod")), AgroKey_EncoderDecoder, Server) &
                            "&ds=" & Stringa_Codifica(data_Stampa, AgroKey_EncoderDecoder, Server) &
                            "&di=" & Stringa_Codifica(Data_Inizio, AgroKey_EncoderDecoder, Server) &
                            "&df=" & Stringa_Codifica(Data_Fine, AgroKey_EncoderDecoder, Server) &
                            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

                '############################################################################

                '------------------------------------------------------
                'biologico
                '------------------------------------------------------
            Case enum_CodificaStampe.SchedaPreparati_Biologico

                'il GiasLan non manda data_inizio e data_fine
                Dim Data_Inizio, Data_Fine As String

                If Not IsNothing(CStr(htVariabiliStampe("data_inizio"))) Then
                    Data_Inizio = CStr(htVariabiliStampe("data_inizio"))
                Else
                    Data_Inizio = AGRODATAINIZIO
                End If
                If Not IsNothing(CStr(htVariabiliStampe("data_fine"))) Then
                    Data_Fine = CStr(htVariabiliStampe("data_fine"))
                Else
                    Data_Fine = AGRODATAFINE
                End If

                TargetURL = PaginaLinkFiltroSchedeBiologico &
                            "?r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                            "&p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&s=" & Stringa_Codifica(CStr(htVariabiliStampe("sa_cod")), AgroKey_EncoderDecoder, Server) &
                            "&di=" & Stringa_Codifica(CStr(Data_Inizio), AgroKey_EncoderDecoder, Server) &
                            "&df=" & Stringa_Codifica(CStr(Data_Fine), AgroKey_EncoderDecoder, Server) &
                            "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.Filtro_StampeBiologico,
                 enum_CodificaStampe.SchedaMateriePrime_Biologico,
                 enum_CodificaStampe.SchedaVendite_Biologico

                TargetURL = PaginaLinkFiltroReportBiologico &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server)


                'Dim bio As New AgronicaCoreBiologicoBIZ.ReportBIO
                'TargetURL = bio.ComponiUrlStampaDaVariabiliStampe(htVariabiliStampe, Server)

                '############################################################################

                '------------------------------------------------------
                'EXPORT RIBA CBI
                '------------------------------------------------------
            Case enum_CodificaStampe.RiBa_Export_CBI

                'Tipo_Report
                '0:              'RIBA
                '1:              'ANTICIPO FATTURA

                TargetURL = "GestioneStampe/Contabilita/RegistroRiBa/Registro_RiBa.aspx"

                QueryString = "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                              "&r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                              "&cr=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                              "&cru=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                              "&tsc=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                              "&sc=" & Stringa_Codifica(Date.Now.Date, AgroKey_EncoderDecoder, Server) &
                              "&cid=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                              "&cld=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                              "&cia=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                              "&cla=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                              "&flapag=" & Stringa_Codifica(True, AgroKey_EncoderDecoder, Server) &
                              "&dp=" & Stringa_Codifica(AGRODATAFINE, AgroKey_EncoderDecoder, Server) &
                              "&tnd=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                              "&nd=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                              "&ad=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                              "&tipo=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                              "&random=" & Stringa_Codifica(UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)
                If Not IsNothing(Request.QueryString("sidebar")) Then
                    QueryString = AgronicaCoreUtility.Varie.aggiungiAQueryString(QueryString, "sidebar", "off")
                End If
                Response.Redirect(TargetURL & QueryString)

                '----------------------------------------------------------
                '--------- ESPORTAZIONE AGENTI PROVVIGIONI -----------
                '----------------------------------------------------------

            Case enum_CodificaStampe.AgentiProvvigioni_XLS

                TargetURL = "GestioneStampe/AgentiProvvigioni/AgentiProvvigioni_XLS.aspx"

                'variabili x excel agenti provvigioni
                Dim Data_Inizio, Data_Fine, str_filtroagg As String
                Dim cod_risum_agente, cod_risum_cliente, mat_cod, lav_cod As Integer
                Dim chk_incassato, chk_nonincassato, chk_parzialmenteincassato As Integer
                Data_Inizio = htVariabiliStampe("data_inizio")
                Data_Fine = htVariabiliStampe("data_fine")
                cod_risum_agente = htVariabiliStampe("cod_risum_agente")
                cod_risum_cliente = htVariabiliStampe("cod_risum_cliente")
                mat_cod = htVariabiliStampe("mat_cod")
                lav_cod = htVariabiliStampe("lav_cod")
                chk_incassato = htVariabiliStampe("chk_incassato")
                chk_nonincassato = htVariabiliStampe("chk_nonincassato")
                chk_parzialmenteincassato = htVariabiliStampe("chk_parzialmenteincassato")
                str_filtroagg = htVariabiliStampe("str_filtroagg")

                TargetURL &= "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                             "&di=" & Stringa_Codifica(Data_Inizio, AgroKey_EncoderDecoder, Server) &
                             "&df=" & Stringa_Codifica(Data_Fine, AgroKey_EncoderDecoder, Server) &
                             "&cra=" & Stringa_Codifica(cod_risum_agente, AgroKey_EncoderDecoder, Server) &
                             "&crc=" & Stringa_Codifica(cod_risum_cliente, AgroKey_EncoderDecoder, Server) &
                             "&mc=" & Stringa_Codifica(mat_cod, AgroKey_EncoderDecoder, Server) &
                             "&lc=" & Stringa_Codifica(lav_cod, AgroKey_EncoderDecoder, Server) &
                             "&chki=" & Stringa_Codifica(chk_incassato, AgroKey_EncoderDecoder, Server) &
                             "&chkni=" & Stringa_Codifica(chk_nonincassato, AgroKey_EncoderDecoder, Server) &
                             "&chkpi=" & Stringa_Codifica(chk_parzialmenteincassato, AgroKey_EncoderDecoder, Server) &
                             "&strfa=" & Stringa_Codifica(str_filtroagg, AgroKey_EncoderDecoder, Server)


                '============================================================


                '----------------------------------------------------------
                '--------- ESPORTAZIONE ANAGRAFICA DEI CONTATTI -----------
                '----------------------------------------------------------
            Case enum_CodificaStampe.Esportazione_AnagraficaContatti

                TargetURL = PaginaLinkEsportazione_AnagraficaContatti &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server)

                '============================================================


                   '----------------------------------------------------------
                '--------- ESPORTAZIONE LISTINI -----------
                '----------------------------------------------------------

            Case enum_CodificaStampe.Listini_XLS

                'variabili x export listini
                Dim Listino_Classe_Cod, Listino_Cod, Elem_Cod, Pro_Cod As Integer
                Dim Tipo_Classe, Listino_Classe_Padre_Cod, ChkApplicabilita As Integer
                Dim Tipo_Iva, Tipo_Provvigione, Conto_Terzi, Cod_Conto_Default, Cal_Cod As Integer
                Dim Chk_Vettore, Cod_Rapporto, Cod_RisUm As Integer
                Dim Mat_Cod, Lav_Cod, Mezzo, Udm_Cod, Cod_Iva, Cod_Conto_Det_Default As Integer
                Dim str_filtroagg As String

                Listino_Classe_Cod = htVariabiliStampe("Listino_Classe_Cod".ToLower)
                Listino_Cod = htVariabiliStampe("Listino_Cod".ToLower)
                Elem_Cod = htVariabiliStampe("Elem_Cod".ToLower)
                Pro_Cod = htVariabiliStampe("Pro_Cod".ToLower)
                Mat_Cod = htVariabiliStampe("Mat_Cod".ToLower)
                Tipo_Classe = htVariabiliStampe("Tipo_Classe".ToLower)
                Listino_Classe_Padre_Cod = htVariabiliStampe("Listino_Classe_Padre_Cod".ToLower)
                ChkApplicabilita = htVariabiliStampe("ChkApplicabilita".ToLower)
                Tipo_Iva = htVariabiliStampe("Tipo_Iva".ToLower)
                Tipo_Provvigione = htVariabiliStampe("Tipo_Provvigione".ToLower)
                Conto_Terzi = htVariabiliStampe("Conto_Terzi".ToLower)
                Cod_Conto_Default = htVariabiliStampe("Cod_Conto_Default".ToLower)
                Cal_Cod = htVariabiliStampe("Cal_Cod".ToLower)
                Lav_Cod = htVariabiliStampe("Lav_Cod".ToLower)
                Chk_Vettore = htVariabiliStampe("Chk_Vettore".ToLower)
                Cod_Rapporto = htVariabiliStampe("Cod_Rapporto".ToLower)
                Cod_RisUm = htVariabiliStampe("Cod_RisUm".ToLower)
                Mezzo = htVariabiliStampe("Mezzo".ToLower)
                Udm_Cod = htVariabiliStampe("Udm_Cod".ToLower)
                Cod_Iva = htVariabiliStampe("Cod_Iva".ToLower)
                Cod_Conto_Det_Default = htVariabiliStampe("Cod_Conto_Det_Default".ToLower)
                str_filtroagg = htVariabiliStampe("str_filtroagg")

                TargetURL = "GestioneStampe/Contabilita/Listini/Listini_XLS.aspx" &
                            "?p=" &
                            Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&lcc=" &
                            Stringa_Codifica(Listino_Classe_Cod, AgroKey_EncoderDecoder, Server) &
                            "&lc=" &
                            Stringa_Codifica(Listino_Cod, AgroKey_EncoderDecoder, Server) &
                            "&elem=" &
                            Stringa_Codifica(Elem_Cod, AgroKey_EncoderDecoder, Server) &
                            "&pro=" &
                            Stringa_Codifica(Pro_Cod, AgroKey_EncoderDecoder, Server) &
                            "&mat=" &
                            Stringa_Codifica(Mat_Cod, AgroKey_EncoderDecoder, Server) &
                            "&tc=" &
                            Stringa_Codifica(Tipo_Classe, AgroKey_EncoderDecoder, Server) &
                            "&lcpc=" &
                            Stringa_Codifica(Listino_Classe_Padre_Cod, AgroKey_EncoderDecoder, Server) &
                            "&chka=" &
                            Stringa_Codifica(ChkApplicabilita, AgroKey_EncoderDecoder, Server) &
                            "&ti=" &
                            Stringa_Codifica(Tipo_Iva, AgroKey_EncoderDecoder, Server) &
                            "&tp=" &
                            Stringa_Codifica(Tipo_Provvigione, AgroKey_EncoderDecoder, Server) &
                            "&ct=" &
                            Stringa_Codifica(Conto_Terzi, AgroKey_EncoderDecoder, Server) &
                            "&ccd=" &
                            Stringa_Codifica(Cod_Conto_Default, AgroKey_EncoderDecoder, Server) &
                            "&cal=" &
                            Stringa_Codifica(Cal_Cod, AgroKey_EncoderDecoder, Server) &
                            "&lav=" &
                            Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder, Server) &
                            "&chkv=" &
                            Stringa_Codifica(Chk_Vettore, AgroKey_EncoderDecoder, Server) &
                            "&cr=" &
                            Stringa_Codifica(Cod_Rapporto, AgroKey_EncoderDecoder, Server) &
                            "&risum=" &
                            Stringa_Codifica(Cod_RisUm, AgroKey_EncoderDecoder, Server) &
                            "&m=" &
                            Stringa_Codifica(Mezzo, AgroKey_EncoderDecoder, Server) &
                            "&udm=" &
                            Stringa_Codifica(Udm_Cod, AgroKey_EncoderDecoder, Server) &
                            "&ci=" &
                            Stringa_Codifica(Cod_Iva, AgroKey_EncoderDecoder, Server) &
                            "&ccdd=" &
                            Stringa_Codifica(Cod_Conto_Det_Default, AgroKey_EncoderDecoder, Server) &
                            "&strfa=" &
                            Stringa_Codifica(str_filtroagg, AgroKey_EncoderDecoder, Server)

                '############################################################################

                '------------------------------------------------------
                'CAMPAGNA
                '------------------------------------------------------
            Case enum_CodificaStampe.SchedaCampagna_2078,
                enum_CodificaStampe.RegistroTrattamenti,
                enum_CodificaStampe.SchedaRegistrazione,
                enum_CodificaStampe.SchedaCampagna_Biologico,
                enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
                enum_CodificaStampe.RegistroTrattamenti_Semplificata,
                enum_CodificaStampe.SchedaRegistrazione_Semplificata,
                enum_CodificaStampe.SchedaCampagna_Biologico_Semplificata,
                enum_CodificaStampe.Eurep_Gap,
                enum_CodificaStampe.Eurep_Gap_Semplificata,
                enum_CodificaStampe.SchedaCampagna_ConserveItalia,
                enum_CodificaStampe.Registro_Fertilizzazioni,
                enum_CodificaStampe.RegistroTrattamenti_Veneto,
                enum_CodificaStampe.SchedaCampagna_Multicentro,
                enum_CodificaStampe.SchedaCampagna_Multicentro_ACA,
                enum_CodificaStampe.Eurep_Gap_Multicentro,
                enum_CodificaStampe.Registro_Fertilizzazioni_Massivo,
                enum_CodificaStampe.Registro_Trattamenti_Massivo,
                enum_CodificaStampe.SchedaCampagna_ProvAut_Trento,
                enum_CodificaStampe.SchedaCampagna_Multi_Lombardia,
                enum_CodificaStampe.SchedaInterventiAgronomici,
                enum_CodificaStampe.RegistroAziendaleUnico,
                enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita

                Dim StampaDiretta As Boolean = False

                If Not IsNothing(htVariabiliStampe) Then

                    If IsNumeric(htVariabiliStampe("stampadiretta")) Then
                        StampaDiretta = CBool(htVariabiliStampe("stampadiretta"))
                    End If

                End If

                Select Case StampaDiretta

                    Case True

                        Dim DataInizio As String = "01/01/" & Year(Date.Now)
                        Dim DataFine As String = "31/12/" & Year(Date.Now)
                        Dim DataStampa As String = ""
                        Dim Arrotondamento As Integer = 3
                        Dim StampaDefinitiva As Integer = 1 '0 = Prova, 1= Definitiva
                        Dim flag_organismoIntest As Boolean = False
                        Dim flag_reg_condizionalita As Boolean = False
                        Dim flag_reg_psr As Boolean = False
                        Dim flag_reg_misura10 As Boolean = False
                        Dim tipo_ordinamento As Integer = 0
                        Dim flag_StampaAnnoImpiantoPluriennali = True


                        TargetURL = "GestioneStampe/SchedaCampagna/SchedaCampagna.aspx" &
                            "?dI=" &
                            Stringa_Codifica(DataInizio, AgroKey_EncoderDecoder, Server) &
                            "&dF=" &
                            Stringa_Codifica(DataFine, AgroKey_EncoderDecoder, Server) &
                            "&dG=" &
                            Stringa_Codifica(DataStampa, AgroKey_EncoderDecoder, Server) &
                            "&arr=" &
                            Stringa_Codifica(Arrotondamento, AgroKey_EncoderDecoder, Server) &
                            "&stDef=" &
                            Stringa_Codifica(StampaDefinitiva, AgroKey_EncoderDecoder, Server) &
                            "&for=" &
                            Stringa_Codifica(flag_organismoIntest, AgroKey_EncoderDecoder, Server) &
                            "&frc=" &
                            Stringa_Codifica(flag_reg_condizionalita, AgroKey_EncoderDecoder, Server) &
                            "&frpsr=" &
                            Stringa_Codifica(flag_reg_psr, AgroKey_EncoderDecoder, Server) &
                            "&frmis=" &
                            Stringa_Codifica(flag_reg_misura10, AgroKey_EncoderDecoder, Server) &
                            "&ord=" &
                            Stringa_Codifica(tipo_ordinamento, AgroKey_EncoderDecoder, Server) &
                            "&chkannoimp=" &
                            Stringa_Codifica(flag_StampaAnnoImpiantoPluriennali, AgroKey_EncoderDecoder, Server) &
                            "&stampadiretta=" &
                            Stringa_Codifica(StampaDiretta, AgroKey_EncoderDecoder, Server)


                    Case False
                        Dim _objParametriServer As AgronicaCoreParametri = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
                        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                        Dim valAgendaNG = objConfSiti.Leggi_Valore(0, "Filtro_SchedaCampagna_BS", "", "", _objParametriServer)

                        If valAgendaNG.ToLower = "true" Then
                            TargetURL = "GestioneStampe/SchedaCampagna/Bootstrap/Selezione_SchedaCampagna_BS.aspx"
                        Else
                            TargetURL = "GestioneStampe/SchedaCampagna/Selezione_SchedaCampagna.aspx"
                        End If
                End Select



            Case enum_CodificaStampe.RisultatoAnalisiConformita

                TargetURL = PaginaLinkStampaRisultatoAnalisiConformita &
                            "?piva=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&veg_cod=" & Stringa_Codifica(CStr(htVariabiliStampe("veg_cod")), AgroKey_EncoderDecoder, Server) &
                            "&data_da=" & Stringa_Codifica(CStr(htVariabiliStampe("data_da")), AgroKey_EncoderDecoder, Server) &
                            "&data_a=" & Stringa_Codifica(CStr(htVariabiliStampe("data_a")), AgroKey_EncoderDecoder, Server)

                If htVariabiliStampe.ContainsKey("sa_cod") Then
                    TargetURL &= "&sa_cod=" & Stringa_Codifica(CStr(htVariabiliStampe("sa_cod")), AgroKey_EncoderDecoder, Server)
                Else
                    TargetURL &= "&sa_cod=" & Stringa_Codifica("0", AgroKey_EncoderDecoder, Server)
                End If

                '############################################################################

                '------------------------------------------------------
                'COLTURALE BIO
                '------------------------------------------------------
            Case enum_CodificaStampe.SchedaColturale_Biologico
                Dim _objParametriServer As AgronicaCoreParametri = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
                Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim valAgendaNG = objConfSiti.Leggi_Valore(0, "Filtro_SchedaCampagna_ColturaleBiologico_BS", "", "", _objParametriServer)

                If valAgendaNG.ToLower = "true" Then
                    TargetURL = "GestioneStampe/SchedaCampagna/Bootstrap/Selezione_SchedaCampagna_BS.aspx?sidebar=off"
                Else
                    TargetURL = PaginaLinkStampaSchedaColturaleBiologico
                End If

                '############################################################################

                '------------------------------------------------------
                'QUADRO P
                '------------------------------------------------------
            Case enum_CodificaStampe.Quadro_P

                TargetURL = "GestioneStampe/Quadro_P/Selezione_Quadro_P.aspx" &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&s=" & Stringa_Codifica(CStr(htVariabiliStampe("sa_cod")), AgroKey_EncoderDecoder, Server)

                '############################################################################


                '------------------------------------------------------
                'CATASTO E UTILIZZI
                '------------------------------------------------------
            Case enum_CodificaStampe.SchedaCatastoeUtilizzi

                TargetURL = "GestioneStampe/CatastoPianoColturale/SchedaCatastoUtilizzi/SchedaCatastoUtilizzi_Filtro.aspx" &
                                        "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("p")), AgroKey_EncoderDecoder, Server) '&
                '                         "&s=" & Stringa_Codifica(CStr(htVariabiliStampe("sa_cod")), AgroKey_EncoderDecoder, Server)

                '############################################################################

                'CAMPAGNA
                '------------------------------------------------------
            Case enum_CodificaStampe.Scheda_Rilievi

                TargetURL = "GestioneStampe/SchedaCampagna/SchedaRilievi.aspx"

                'Estrattore Grafica
            Case enum_CodificaStampe.EstrattoreDatiGrafici

                TargetURL = PaginaLinkStampaEstrattoreGrafica


            Case enum_CodificaStampe.FF_Etichette

                TargetURL = PaginaLinkFreshFoodEtichette &
                            "?i=" & Stringa_Codifica(CStr(htVariabiliStampe("id_agenda")), AgroKey_EncoderDecoder, Server) &
                            "&piva=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.Liquidazione_Soci

                TargetURL = PaginaLinkLiquidazioneSoci &
                            "?i=" & Stringa_Codifica(id_agenda, AgroKey_EncoderDecoder, Server)

            '    '------------------------------------
            '    '------ Schede OP Varie -----
            '    '------------------------------------
            Case enum_CodificaStampe.Impegnative_capitolati

                TargetURL = PaginaLinkSchedeVarieOP


                '------------------------------------
                '------ TRACCIABILITA' -----
                '------------------------------------
            Case enum_CodificaStampe.SchedaTracciabilita

                TargetURL = PaginaLinkStampeFiltroSchedaTracciabilitaVegetale

                '------------------------------------
                '------ Esportazione OP Produttori, catasto -----
                '------------------------------------
            Case enum_CodificaStampe.Esportazione_OP_Produttori,
                enum_CodificaStampe.Esportazione_OP_Catasto

                TargetURL = PaginaLinkEsportazioneOP_Filtro &
                            "?r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server)


                '-------------------------------------------------
                'Accettazione da Diversi - Filtro Report
                '-------------------------------------------------
            Case enum_CodificaStampe.ADD_Filtro_Report_Accettazione_DaDiversi

                Dim PrintName As String

                If IsNothing(htVariabiliStampe("printname")) Then
                    PrintName = ""
                Else
                    PrintName = CStr(htVariabiliStampe("printname"))
                End If

                If IsNothing(htVariabiliStampe("id_agenda")) Then
                    id_agenda = "0"
                Else
                    id_agenda = CStr(htVariabiliStampe("id_agenda"))
                End If

                TargetURL = PaginaLinkFiltroReportAccettazioneDaDiversi &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&rs=" & Stringa_Codifica(UtilityProvider.QS_SaveText(CStr(htVariabiliStampe("rag_soc"))), AgroKey_EncoderDecoder, Server) &
                            "&pn=" & Stringa_Codifica(UtilityProvider.QS_SaveText(PrintName), AgroKey_EncoderDecoder, Server) &
                            "&m=" & Stringa_Codifica(CStr(0), AgroKey_EncoderDecoder, Server) &
                            "&i=" & Stringa_Codifica(id_agenda, AgroKey_EncoderDecoder, Server)

                '========================================================================

                '-------------------------------------------------
                'Fresh&Food - Filtro Report
                '-------------------------------------------------
            Case enum_CodificaStampe.Conf_FiltroStampe

                TargetURL = PaginaLinkFiltroReportFreshFood &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server)

                '========================================================================

            Case enum_CodificaStampe.Cespiti_FiltroStampe

                TargetURL = PaginaLinkFiltroReportCespiti &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server)

                '============================================================

            Case enum_CodificaStampe.Bilancio_Fertilizzazioni

                TargetURL = PaginaLinkBilancio_Fertilizzazioni

            Case enum_CodificaStampe.Bilancio_Fertilizzazioni_Dettagliato

                TargetURL = PaginaLinkBilancio_Fertilizzazioni_Dettagliato


            Case enum_CodificaStampe.Esportatore_Universale_Impianti

                Dim StringaCodifica As String

                StringaCodifica = Stringa_Codifica("impianti", AgroKey_EncoderDecoder, Server)

                TargetURL = PaginaLinkEsportatore_Universale & "?t=" & StringaCodifica

            Case enum_CodificaStampe.Esportatore_Universale_Imprese

                Dim StringaCodifica As String

                StringaCodifica = Stringa_Codifica("imprese", AgroKey_EncoderDecoder, Server)

                TargetURL = PaginaLinkEsportatore_Universale & "?t=" & StringaCodifica

            Case enum_CodificaStampe.Esportatore_Universale_Centri

                Dim StringaCodifica As String

                StringaCodifica = Stringa_Codifica("centri", AgroKey_EncoderDecoder, Server)

                TargetURL = PaginaLinkEsportatore_Universale & "?t=" & StringaCodifica

            Case enum_CodificaStampe.Esportatore_Universale_Agenda

                Dim StringaCodifica As String

                StringaCodifica = Stringa_Codifica("agenda", AgroKey_EncoderDecoder, Server)

                TargetURL = PaginaLinkEsportatore_Universale & "?t=" & StringaCodifica

            Case enum_CodificaStampe.Esportatore_Universale_Rintraccio

                Dim StringaCodifica As String

                StringaCodifica = Stringa_Codifica("rintraccio", AgroKey_EncoderDecoder, Server)

                TargetURL = PaginaLinkEsportatore_Universale & "?t=" & StringaCodifica


            Case enum_CodificaStampe.LibroConferimenti

                TargetURL = PaginaLinkStampaLibroConferimenti

                '============================================================

            Case enum_CodificaStampe.LibroConferimenti_XLS

                TargetURL = PaginaLinkStampaLibroConferimentiXLS

                '============================================================

            Case enum_CodificaStampe.PianoColturale

                TargetURL = PaginaLinkStampaPianoColturale

                '============================================================

            Case enum_CodificaStampe.PianoColturaleCatasto

                TargetURL = PaginaLinkStampaPianoColturaleCatasto

            Case enum_CodificaStampe.PianoColturaleCatastoGrid
                TargetURL = AgronicaCoreUtility.Varie.aggiungiAQueryString(PaginaLinkStampaPianoColturaleCatastoGrid, "sidebar", "off")


                '============================================================
            Case enum_CodificaStampe.EsportazionePomodoroIndustriaOINordItalia

                TargetURL = PaginaLinkEsportazionePomodoroIndustriaOINordItalia

            Case enum_CodificaStampe.PassaportoMaterialeVivaistico

                'richiamo la pagina di gestione delle fatture
                TargetURL = PaginaLinkStampaPassaportoVivaistico &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&OperazioneCod=" & Stringa_Codifica(CStr(htVariabiliStampe("id_agenda")), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.UMA_RichiestaCarbPrevisioneLav

                TargetURL = PaginaLinkUMARichiestaCarbPrevisioneLav &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&crt=" & Stringa_Codifica(CStr(htVariabiliStampe("richiesta_cod")), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.UMA_VerbaleIstruttoriaRichCarb

                TargetURL = PaginaLinkUMAVerbaleIstruttoriaRichCarb &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&crt=" & Stringa_Codifica(CInt(htVariabiliStampe("richiesta_cod")), AgroKey_EncoderDecoder, Server) &
                            "&md=" & Stringa_Codifica(CBool(htVariabiliStampe("modifica_dati")), AgroKey_EncoderDecoder, Server) &
                            "&sm=" & Stringa_Codifica(CBool(htVariabiliStampe("segnalazioni_macchine")), AgroKey_EncoderDecoder, Server) &
                            "&e=" & Stringa_Codifica(CInt(htVariabiliStampe("esito")), AgroKey_EncoderDecoder, Server) &
                            "&ne=" & Stringa_Codifica(CStr(htVariabiliStampe("note_esito")), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.UMA_RendicontazioneCarb

                TargetURL = PaginaLinkUMARendicontazioneCarb &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&crt=" & Stringa_Codifica(CStr(htVariabiliStampe("richiesta_cod")), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.UMA_IstruttoriaRendCarb

                TargetURL = PaginaLinkUMAIstruttoriaRendCarb &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                            "&crt=" & Stringa_Codifica(CInt(htVariabiliStampe("richiesta_cod")), AgroKey_EncoderDecoder, Server) &
                            "&md=" & Stringa_Codifica(CBool(htVariabiliStampe("modifica_dati")), AgroKey_EncoderDecoder, Server) &
                            "&sm=" & Stringa_Codifica(CBool(htVariabiliStampe("segnalazioni_macchine")), AgroKey_EncoderDecoder, Server) &
                            "&e=" & Stringa_Codifica(CInt(htVariabiliStampe("esito")), AgroKey_EncoderDecoder, Server) &
                            "&ne=" & Stringa_Codifica(CStr(htVariabiliStampe("note_esito")), AgroKey_EncoderDecoder, Server)

                 '============================================================

                '-------------------------------------------------------
                'Report Riepilogo Utilizzo Superfici
                '-------------------------------------------------------
            Case enum_CodificaStampe.RiepilogoImpiegoSuperfici

                TargetURL = PaginaLinkRiepilogoSuperficiMonoAzienda &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server)

               '============================================================

                '-------------------------------------------------------
                'Stampa Excel Riepilogo Superfici Multiazienda
                '-------------------------------------------------------
            Case enum_CodificaStampe.RiepilogoImpiegoSuperfici_Multiazienda

                Dim xmlVarSuperficiMulti As New XmlDocument()
                xmlVarSuperficiMulti.LoadXml(Session("strXmlVariabilistampe"))

                'Ricavo dall'XML la lista di nodi VarStampa che contengono le variabili necessarie per la stampa selezionata
                Dim XML_VarStampaNodeList As System.Xml.XmlNodeList
                XML_VarStampaNodeList = xmlVarSuperficiMulti.SelectNodes("ParametriAgronicaStampe_2010/VariabiliStampe/VarStampa")
                Dim iMax As Integer = XML_VarStampaNodeList.Count - 1
                Dim ListaPiva(iMax) As String
                Dim ListSa_cod(iMax) As Integer
                Dim ListAppezza(iMax) As Integer
                Dim ListId_reg(iMax) As Integer

                'Inserisco ciascun gruppo dei valori dell'iva, sacod, appezza e idreg nella 
                'lista corrispondente che salvo in sessione
                Dim i As Integer = 0
                Dim nodeXml As System.Xml.XmlElement
                For i = 0 To iMax

                    nodeXml = (CType(XML_VarStampaNodeList.Item(i), System.Xml.XmlElement))

                    ListaPiva(i) = nodeXml.GetAttribute("piva")
                    ListSa_cod(i) = CInt(nodeXml.GetAttribute("sa_cod"))
                    ListAppezza(i) = CInt(nodeXml.GetAttribute("appezza"))
                    ListId_reg(i) = CInt(nodeXml.GetAttribute("id_reg"))

                Next

                Session("ListPiva") = ListaPiva
                Session("ListSa_cod") = ListSa_cod
                Session("ListAppezza") = ListAppezza
                Session("ListId_reg") = ListId_reg

                TargetURL = PaginaLinkRiepilogoSuperficiMultiazienda &
                    "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server)

                '============================================================

            Case enum_CodificaStampe.EstrazioneCatastoAffitti

                TargetURL = PaginaLinkEstrazioneCatastoAffitti &
                            "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server)

                '============================================================

            Case enum_CodificaStampe.Report_ImpegnoProduzioneSoci

                TargetURL = PaginaLinkImpegnoProduzioneSoci
                If htVariabiliStampe IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(htVariabiliStampe("piva")) Then
                    TargetURL &= "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server)
                End If

                '============================================================

            Case enum_CodificaStampe.ReportConserveItalia

                TargetURL = PaginaLinkXLSConserveItalia
                If htVariabiliStampe IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(htVariabiliStampe("piva")) Then
                    TargetURL &= "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server)
                End If

                '============================================================
            Case enum_CodificaStampe.Report_OrdiniVivaio

                TargetURL = PaginaLinkOrdiniVivaio
                If htVariabiliStampe IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(htVariabiliStampe("piva")) Then
                    TargetURL &= "?p=" & Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server)
                End If

                '============================================================
            Case enum_CodificaStampe.Atto_Notorio
                Session("strParametri") = Session("Sql_Filtro")
                TargetURL = PaginaLinkAttoNotorio
                TargetURL &= "?p=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                    "&r=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("rag_soc")), AgroKey_EncoderDecoder, Server) &
                    "&a=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("anno")), AgroKey_EncoderDecoder, Server) &
                    "&tipo=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("tiposelezione")), AgroKey_EncoderDecoder, Server)


            Case enum_CodificaStampe.Atto_Notorio * -1

                Session("strParametri") = Session("Sql_Filtro")
                TargetURL = PaginaLinkAttoNotorioConCatasto
                TargetURL &= "?p=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                    "&r=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("rag_soc")), AgroKey_EncoderDecoder, Server) &
                    "&a=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("anno")), AgroKey_EncoderDecoder, Server) &
                    "&tipo=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("tiposelezione")), AgroKey_EncoderDecoder, Server)



            Case enum_CodificaStampe.Produzioni_XLS

                TargetURL = PaginaLinkExcelAutomatico_XLS

                'VERIFICO QUANTI REPORT BISOGNA CHIAMARE

                Dim XmlDoc As New XmlDocument
                Dim XMLs_Report As XmlNodeList

                XmlDoc.LoadXml(Session("strXmlVariabilistampe").ToString)

                XMLs_Report = XmlDoc.GetElementsByTagName("report")


                Dim i As Integer
                Dim XML_Report As XmlElement
                Dim str_XML_Report As String

                For i = 0 To XMLs_Report.Count - 1

                    XML_Report = XMLs_Report.Item(i)

                    str_XML_Report = XML_Report.OuterXml
                    Session("strXmlVariabilistampe") = str_XML_Report

                    'Page_NewWindow(Server, Session, Page,
                    '                    PaginaLinkExcelAutomatico_XLS,
                    '                    "",
                    '                    "Produzioni_Excel",
                    '                    , , , , , , , )

                Next


            Case enum_CodificaStampe.Allegato_CatastoeValorizzazioni

                Session("strParametri") = Session("Sql_Filtro")
                TargetURL = PaginaLinkSchedaOPTipo1
                TargetURL &= "?p=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                    "&r=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("rag_soc")), AgroKey_EncoderDecoder, Server) &
                    "&a=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("anno")), AgroKey_EncoderDecoder, Server) &
                    "&t=" &
                    Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                    "&tipo=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("tiposelezione")), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.Adesione_Etico_Ambientale,
                 enum_CodificaStampe.Tenuta_Scheda_Campagna,
                 enum_CodificaStampe.Codice_Condotta,
                 enum_CodificaStampe.Adesione_DPI,
                 enum_CodificaStampe.Impegnativa_Eurep,
                 enum_CodificaStampe.Impegnativa_QC,
                 enum_CodificaStampe.Impegnativa_Confusione_Sessuale,
                 enum_CodificaStampe.Adesione_ModuloGrasp, 'new ones from here
                 enum_CodificaStampe.Adesione_ProtocolloGlobalGAP,
                 enum_CodificaStampe.Adesione_NurtureModule,
                 enum_CodificaStampe.Adesione_Conad,
                 enum_CodificaStampe.Adesione_Despar,
                 enum_CodificaStampe.Adesione_StandardLeaf,
                 enum_CodificaStampe.Accordo_Responsabilita_di_Filiera

                TargetURL = PaginaLinkSchedaOPTipo1
                TargetURL &= "?p=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                    "&r=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("rag_soc")), AgroKey_EncoderDecoder, Server) &
                    "&a=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("anno")), AgroKey_EncoderDecoder, Server) &
                    "&t=" &
                    Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                    "&tipo=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("tiposelezione")), AgroKey_EncoderDecoder, Server) &
                    "&fronteretro=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("fronteretro")), AgroKey_EncoderDecoder, Server)
                Session("strParametri") = Session("Sql_Filtro")

            Case enum_CodificaStampe.Mandato_Trasmissione_Telematica_Dati

                TargetURL = PaginaLinkMandatoTrasmissione
                TargetURL &= "?p=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                    "&r=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("rag_soc")), AgroKey_EncoderDecoder, Server) &
                    "&a=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("anno")), AgroKey_EncoderDecoder, Server) &
                    "&tipo=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("tiposelezione")), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.SchedaAziendale
                Session("strParametri") = Session("Sql_Filtro")
                TargetURL = PaginaLinkSchedaAziendale
                TargetURL &= "?p=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                    "&r=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("rag_soc")), AgroKey_EncoderDecoder, Server) &
                    "&a=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("anno")), AgroKey_EncoderDecoder, Server) &
                    "&tipo=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("tiposelezione")), AgroKey_EncoderDecoder, Server) &
                    "&fronteretro=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("fronteretro")), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.ImpegnoProduzioneSociDivisoxCentri

                TargetURL = PaginaLinkImpegnoProduzioneSociDivisoxCentri
                TargetURL &= "?p=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                    "&r=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("rag_soc")), AgroKey_EncoderDecoder, Server) &
                    "&a=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("anno")), AgroKey_EncoderDecoder, Server) &
                    "&tipo=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("tiposelezione")), AgroKey_EncoderDecoder, Server) &
                    "&fronteretro=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("fronteretro")), AgroKey_EncoderDecoder, Server)

                Session("strParametri") = Session("Sql_Filtro")

            Case enum_CodificaStampe.Impegnativa_Orticole_Gest_Annuale, ' Schede OP che accettano Impianti ma vogliono anche la descrizione dei vari impianti
                enum_CodificaStampe.Impegnativa_Orticole_Gest_Breve,
                enum_CodificaStampe.Impegnativa_Fagiolino_Mercato_Fresco,
                enum_CodificaStampe.Impegnativa_Orticole_Industria,
                enum_CodificaStampe.Impegnativa_Pomodoro_Industria,
                enum_CodificaStampe.Dichiarazione_di_Responsabilita,
                enum_CodificaStampe.Fitoregolatori_Kiwi

                Session("DescrImpianti") = htVariabiliStampe("descrimpiantischedaop")
                Session("strParametri") = Session("Sql_Filtro")
                TargetURL = PaginaLinkSchedaOPTipo2
                TargetURL &= "?p=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                    "&r=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("rag_soc")), AgroKey_EncoderDecoder, Server) &
                    "&a=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("anno")), AgroKey_EncoderDecoder, Server) &
                    "&t=" &
                    Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                    "&s=" &
                    Stringa_Codifica(htVariabiliStampe("sa_codschedaop"), AgroKey_EncoderDecoder, Server) &
                    "&tipo=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("tiposelezione")), AgroKey_EncoderDecoder, Server) &
                    "&fronteretro=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("fronteretro")), AgroKey_EncoderDecoder, Server)


            Case enum_CodificaStampe.ImpegnativaColtivazioneConferimento
                TargetURL = PaginaLinkImpegnativaColtivazioneConferimento
                TargetURL &= "?p=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                    "&r=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("rag_soc")), AgroKey_EncoderDecoder, Server) &
                    "&a=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("anno")), AgroKey_EncoderDecoder, Server) &
                    "&s=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("stampavuota")), AgroKey_EncoderDecoder, Server) &
                    "&tipo=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("tiposelezione")), AgroKey_EncoderDecoder, Server) &
                    "&fronteretro=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("fronteretro")), AgroKey_EncoderDecoder, Server)
                Session("strParametri") = Session("Sql_Filtro")

            Case enum_CodificaStampe.QuestionarioValutazioneAzienda_Aggiornamento
                Session("strParametri") = Session("Sql_Filtro")
                TargetURL = PaginaLinkQuestionarioValutazioneAzienda
                TargetURL &= "?p=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                    "&r=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("rag_soc")), AgroKey_EncoderDecoder, Server) &
                    "&a=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("anno")), AgroKey_EncoderDecoder, Server) &
                    "&tipo=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("tiposelezione")), AgroKey_EncoderDecoder, Server) &
                    "&fronteretro=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("fronteretro")), AgroKey_EncoderDecoder, Server)
            Case enum_CodificaStampe.ObiettivoDiProduzioneAsipo

                TargetURL = PaginaLinkObiettivoDiProduzione
                TargetURL &= "?p=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                    "&di=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("datainizio")), AgroKey_EncoderDecoder, Server) &
                    "&df=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("datafine")), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.Statistometro
                TargetURL = AgronicaCoreUtility.Varie.aggiungiAQueryString(PaginaLinkStatistometro, "sidebar", "off")

            Case enum_CodificaStampe.Stampa_Analisi_Fitofarmaci
                Session("strParametri") = Session("Sql_Filtro")
                TargetURL = PaginaStampaAnalisiFitofarmaci

            Case enum_CodificaStampe.Stampa_Etichetta
                Session("strParametri") = Session("Sql_Filtro")
                TargetURL = PaginaStampaEtichetta

            Case enum_CodificaStampe.Stampa_Rapporto_di_Prova
                Session("strParametri") = Session("Sql_Filtro")
                TargetURL = PaginaStampaRapportodiProva

            Case enum_CodificaStampe.Stampa_Abilitazioni
                Session("strParametri") = Session("Sql_Filtro")
                TargetURL = PaginaStampaAbilitazioni
                TargetURL &= "?p=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                    "&lp=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("stringa_lista_pive")), AgroKey_EncoderDecoder, Server) &
                    "&v=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("veg_cod")), AgroKey_EncoderDecoder, Server) &
                    "&g=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("grva_cod")), AgroKey_EncoderDecoder, Server) &
                    "&cc=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("cul_cod")), AgroKey_EncoderDecoder, Server) &
                    "&a=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("anno")), AgroKey_EncoderDecoder, Server) &
                    "&vi=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("validita_inizio")), AgroKey_EncoderDecoder, Server) &
                    "&vf=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("validita_fine")), AgroKey_EncoderDecoder, Server) &
                    "&tr=" &
                    Stringa_Codifica(CStr(htVariabiliStampe("tipologia_report")), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.Stampa_Checklist_Global_GAP
                Session("strParametri") = Session("Sql_Filtro")

                Select Case CStr(htVariabiliStampe("stampa"))
                    Case "0"
                        TargetURL = PaginaLinkChecklistGlobalGap_StampaCrystal
                        TargetURL &= "?p=" &
                        Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                        "&r=" &
                        Stringa_Codifica(CStr(htVariabiliStampe("regolamento_cod")), AgroKey_EncoderDecoder, Server) &
                        "&c=" &
                        Stringa_Codifica(CStr(htVariabiliStampe("audit_cod")), AgroKey_EncoderDecoder, Server) &
                        "&d=" &
                        Stringa_Codifica(CStr(htVariabiliStampe("audit_data")), AgroKey_EncoderDecoder, Server) &
                        "&e=" &
                        Stringa_Codifica(CStr(htVariabiliStampe("audit_disposizioni")), AgroKey_EncoderDecoder, Server)

                    Case "1"
                        TargetURL = PaginaLinkChecklistGlobalGap_RappVerificaIspettiva
                        TargetURL &= "?p=" &
                        Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                        "&r=" &
                        Stringa_Codifica(CStr(htVariabiliStampe("regolamento_cod")), AgroKey_EncoderDecoder, Server) &
                        "&c=" &
                        Stringa_Codifica(CStr(htVariabiliStampe("audit_cod")), AgroKey_EncoderDecoder, Server) &
                        "&d=" &
                        Stringa_Codifica(CStr(htVariabiliStampe("audit_data")), AgroKey_EncoderDecoder, Server)
                    Case "2"
                        TargetURL = PaginaLinkChecklistGlobalGap_RapportoNC
                        TargetURL &= "?p=" &
                        Stringa_Codifica(CStr(htVariabiliStampe("piva")), AgroKey_EncoderDecoder, Server) &
                        "&r=" &
                        Stringa_Codifica(CStr(htVariabiliStampe("regolamento_cod")), AgroKey_EncoderDecoder, Server) &
                        "&c=" &
                        Stringa_Codifica(CStr(htVariabiliStampe("audit_cod")), AgroKey_EncoderDecoder, Server) &
                        "&d=" &
                        Stringa_Codifica(CStr(htVariabiliStampe("audit_data")), AgroKey_EncoderDecoder, Server)
                End Select

            Case enum_CodificaStampe.ExportExcel_MonitoraggioCE,
                 enum_CodificaStampe.ExportExcel_MonitoraggioCE_Aggregata

                If Report = enum_CodificaStampe.ExportExcel_MonitoraggioCE_Aggregata Then
                    TargetURL = PaginaLinkStampaExcel_MonitoraggioCEAggregata
                Else
                    TargetURL = PaginaLinkStampaExcel_MonitoraggioCE
                End If

                TargetURL &= "?piva_produttore=" & Stringa_Codifica(CStr(htVariabiliStampe("piva_produttore")), AgroKey_EncoderDecoder, Server) &
                            "&veg_cod=" & Stringa_Codifica(CStr(htVariabiliStampe("veg_cod")), AgroKey_EncoderDecoder, Server) &
                            "&mat_cod=" & Stringa_Codifica(CStr(htVariabiliStampe("mat_cod")), AgroKey_EncoderDecoder, Server) &
                            "&flag_appezza=" & Stringa_Codifica(CStr(htVariabiliStampe("flag_appezza")), AgroKey_EncoderDecoder, Server) &
                            "&sa_cod=" & Stringa_Codifica(CStr(htVariabiliStampe("sa_cod")), AgroKey_EncoderDecoder, Server) &
                            "&appezza=" & Stringa_Codifica(CStr(htVariabiliStampe("appezza")), AgroKey_EncoderDecoder, Server) &
                            "&id_reg=" & Stringa_Codifica(CStr(htVariabiliStampe("id_reg")), AgroKey_EncoderDecoder, Server) &
                            "&tipologia=" & Stringa_Codifica(CStr(htVariabiliStampe("tipologia")), AgroKey_EncoderDecoder, Server) &
                            "&pericolosita=" & Stringa_Codifica(CStr(htVariabiliStampe("pericolosita")), AgroKey_EncoderDecoder, Server) &
                            "&dal=" & Stringa_Codifica(CStr(htVariabiliStampe("dal")), AgroKey_EncoderDecoder, Server) &
                            "&al=" & Stringa_Codifica(CStr(htVariabiliStampe("al")), AgroKey_EncoderDecoder, Server) &
                            "&regolamento=" & Stringa_Codifica(CStr(htVariabiliStampe("regolamento")), AgroKey_EncoderDecoder, Server) &
                            "&sa_cod_fabbr=" & Stringa_Codifica(CStr(htVariabiliStampe("sa_cod_fabbr")), AgroKey_EncoderDecoder, Server) &
                            "&fabbr_cod=" & Stringa_Codifica(CStr(htVariabiliStampe("fabbr_cod")), AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.Stampa_Zoo_Dettaglio_Partita,
                 enum_CodificaStampe.Stampa_Zoo_Sintesi_Partite

                If URL_StampaPersonalizzata <> "" Then
                    TargetURL = URL_StampaPersonalizzata
                Else
                    TargetURL = PaginaLinkFiltroZootecnia
                End If


            Case Else

                'report non gestito
                Me.TxtRisultato.Text = "Report non gestito oppure configurazione_siti non valorizzato correttamente."

                '############################################################################

        End Select


        If TargetURL <> String.Empty Then
            If Not IsNothing(Request.QueryString("sidebar")) Then
                TargetURL = AgronicaCoreUtility.Varie.aggiungiAQueryString(TargetURL, "sidebar", "off")
            End If
            Response.Redirect(TargetURL)
        End If


    End Sub



    '################################################################################
    Public Sub RecuperaDati_Utente_SuperUser(ByVal x_UserName_Utente As String,
                                             ByRef Utente_Password As String,
                                             ByRef Utente_CodFiscale As String,
                                             ByRef Utente_NomeRagSoc As String,
                                             ByRef SuperUser_Username As String,
                                             ByRef SuperUser_Password As String,
                                             ByRef SuperUser_CodFiscale As String,
                                             ByRef SuperUser_NomeRagSoc As String,
                                             ByRef ProgressivoGIAS As Integer,
                                             ByRef objparametri_Server As AgronicaCoreParametri)

        Dim dt As DataTable

        Dim leggi As New AgronicaCoreUtentiDAL.Utenti_Read
        dt = leggi.Leggi_DatiUtente_e_DatiSuperUser(x_UserName_Utente,
                                                    "",
                                                    "",
                                                    0,
                                                    0,
                                                    Date.Now,
                                                    CType(Now.Hour, Short),
                                                    False,
                                                    0,
                                                    "",
                                                    "",
                                                    objparametri_Server)


        If Not IsNothing(dt) Then

            If dt.Rows.Count <> 0 Then


                '-----------------------------------------------------------------------
                '------------ Memorizzo i dati dell' UTENTE ----------------------------
                '-----------------------------------------------------------------------

                Utente_Password = dt.Rows(0).Item("Password")

                Utente_CodFiscale = dt.Rows(0).Item("CodFisc")

                If dt.Rows(0).Item("Utente_Flag_Azienda_Persona") = 1 Then
                    Utente_NomeRagSoc = CStr(dt.Rows(0).Item("Rag_Soc"))
                Else
                    Utente_NomeRagSoc = CStr(dt.Rows(0).Item("Cognome")) & " " & CStr(dt.Rows(0).Item("Nome"))
                End If


                '---------------------------------------------------------
                '----- Memorizzo i dati del del SUPERUSER ----
                '---------------------------------------------------------

                SuperUser_Username = dt.Rows(0).Item("Utente_Profilo")
                SuperUser_Password = dt.Rows(0).Item("Password_SuperUser")
                SuperUser_CodFiscale = dt.Rows(0).Item("CodFisc_SuperUser")

                SuperUser_NomeRagSoc = CStr(dt.Rows(0).Item("RagSoc_SuperUser"))

                '-----------------------------------------------------------------------
                '------------ Memorizzo il Codice Progressivo GIAS ---------------------
                '-----------------------------------------------------------------------

                ProgressivoGIAS = dt.Rows(0).Item("ProgressivoGIAS")

            End If

        End If

    End Sub


    Friend Sub ImpostaCultura(ByRef lingua As Lingua)
        If lingua IsNot Nothing AndAlso lingua.CodiceISO IsNot Nothing Then
            System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(lingua.CodiceISO)
            '' questa istruzione da errore
            'System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("en")
            '_LinguaCorrente = lingua
            Session("LinguaCorrente") = lingua
        End If
    End Sub


End Class

