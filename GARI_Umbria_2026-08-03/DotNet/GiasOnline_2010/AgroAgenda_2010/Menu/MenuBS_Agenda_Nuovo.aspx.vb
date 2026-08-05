Imports System.Web.Script.Serialization
Imports System.Web.Services
Imports System.Xml
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreXML.XML_Stampe
Imports Newtonsoft.Json.Linq

Public Class MenuBS_Agenda_Nuovo
    Inherits System.Web.UI.Page

    Protected WithEvents hf_LavCodRaccolta As Global.System.Web.UI.WebControls.HiddenField

    Public PageMode As String
    Public VerificaAnagraficaImpreseProfilate As Boolean = False
    Dim objParametri_Server As AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreParametri

    Public permessi As PermessiUtente
    Dim objParametriAgenda As ParametriAgenda

    Public Menu_Agenda_Visualizzazione_Dettagli_Operazioni As Boolean

    Public RicetteOrdiniDiLavoroAttivi As Boolean = False

    Public VisualizzaTabColturali As Boolean = True
    Public VisualizzaTabMagContab As Boolean = True
    Public VisualizzaTabAudit As Boolean = True
    Public VisualizzaTabMacchine As Boolean = True
    Public VisualizzaTabZoo As Boolean = True
    Public DefaultTab As Integer = 0
    Public Mode As String = ""

    Public StrLavCodRicettabili As String = ""
    Public ListLavCodRicettabili As String()
    Public ImportaSoloAziendaSelezionata As Boolean = True 'test
    Public SincroDatiApp As Boolean = False 'test
    Public CaricaDatiApp As Boolean = True 'test

    Public debug_isattached As Boolean

    Public Shared LAV_COD_COPIABILI As Integer() = {LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE, LAVCOD_DISTRIBUZIONE_INSETTI,
                                                    LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_CONCIA_SEME,
                                                    LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                                                    LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_DISTRIBUZIONE_CONCIME,
                                                    LAVCOD_FERTIRRIGAZIONE, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_ANDANAMENTO, LAVCOD_ARATURA,
                                                    LAVCOD_DEFOGLIAZIONE, LAVCOD_ASPORTAZIONE_ORGANI_INFETTI, LAVCOD_ASSOLCATURA,
                                                    LAVCOD_CARICO_MANUALE_FRUTTA, LAVCOD_CIMATURA, LAVCOD_DIRADAMENTO_MANUALE, LAVCOD_DISSODAMENTO,
                                                    LAVCOD_ERPICATURA, LAVCOD_ESTIRPATURA, LAVCOD_ESPIANTO, LAVCOD_FALCIACONDIZIONATURA, LAVCOD_FALCIATURA_ERBAI,
                                                    LAVCOD_FORMAZIONE_ARGINELLI, LAVCOD_FRANGIZOLLATURA, LAVCOD_FRESATURA, LAVCOD_IMBALLO_FIENO_ROTOLI,
                                                    LAVCOD_INTERRAMENTO_PAGLIE, LAVCOD_LAVORAZIONE_TRA_FILA, LAVCOD_LAVORAZIONE_SU_FILA,
                                                    LAVCOD_LEGATURA, LAVCOD_LIVELLAMENTO, LAVCOD_MANUTENZIONE_ARGINI, LAVCOD_MESSA_DIMORA_PIANTE,
                                                    LAVCOD_MIETITREBBIATURA, LAVCOD_MINIMUM_TILLAGE, LAVCOD_PACCIAMATURA, LAVCOD_POTATURA_SECCA,
                                                    LAVCOD_POTATURA_VERDE, LAVCOD_PRESSATURA, LAVCOD_RACCOLTA_LEGNA_POTATURA, LAVCOD_RACCOLTA_MANUALE,
                                                    LAVCOD_RACCOLTA_MECCANICA, LAVCOD_RANGHINATURA, LAVCOD_RINCALZATURA, LAVCOD_RIPPATURA, LAVCOD_RIPUNTATURA,
                                                    LAVCOD_RIVOLTAMENTO_FORAGGIO, LAVCOD_RULLATURA, LAVCOD_SARCHIATURA, LAVCOD_SCARIFICATURA, LAVCOD_SCASSO,
                                                    LAVCOD_TRINCIATURA, LAVCOD_VANGATURA, LAVCOD_ZAPPATURA, LAVCOD_GEBIATURA, LAVCOD_ROMPICROSTA,
                                                    LAVCOD_LAVORAZIONE_CONBINATA, LAVCOD_ERPICATURA_ROTANTE, LAVCOD_INTERVENTO_ANTIBRINA, LAVCOD_ALTRE_OPERAZIONI,
                                                    LAVCOD_STRIGLIATURA, LAVCOD_PIRODISERBO, LAVCOD_ABBATTIMENTOIMPIANTI}

    Public Shared LAV_COD_NON_EDITABILI As Integer() = {LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI,
                                                        LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI, LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI,
                                                        LAVCOD_CURA}

    Public ReadOnly Property PATH_GIASBASE As String
        Get
            Return Me.Master.PATH_GIASBASE
        End Get
    End Property



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Debugger.IsAttached Then
            debug_isattached = True
        End If

        permessi = New PermessiUtente()

        objParametriAgenda = New ParametriAgenda
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        Dim agendaStr = objParametriAgenda.ToString()

        agrometeo.Meteo_headerPlaceHeader = Meteo_headerPlaceHeader

        Master.Master_versione = VERSIONE_MASTER_DEFAULT

        'Master.Header_versione = VERSIONE_HEADER_DEFAULT

        Dim xLeggiCfgVerifica As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim dtLeggiCfgVerifica As DataTable = xLeggiCfgVerifica.Leggi(0, "VerificaAnagraficaImpreseProfilate", "", "", objParametri_Server)

        If dtLeggiCfgVerifica.Rows.Count > 0 AndAlso
            CStr(dtLeggiCfgVerifica.Rows(0)("valore")).ToLower = "true" AndAlso
            objParametriAgenda.Piva <> "" Then

            Dim redirectAuto As Boolean = DecidiSeImpostareRedirectAutomatico(objParametri_Server.UtenteUsername, objParametri_Utenti)

            If redirectAuto Then

                VerificaAnagraficaImpreseProfilate = True

                'se l'azienda non è profilata allora passo automaticamente alla sua profilazione.
                Dim letturaStatoWorkflow As New AgronicaCoreProfilazioneBIZ.Pratiche_R
                Dim statoAttuale As Integer = letturaStatoWorkflow.StatoAttualeDaPivaServizio(objParametriAgenda.Piva, "", enum_Servizi.Workflow_di_attivazione_aziende_GIAS, objParametri_Server)

                If statoAttuale <> enum_WWorflow_WAnagraficaStati.Attivazione_Aziende_Agrarie_in_GIAS_Impresa_correttamente_profilata Then
                    Dim idleParametroDue As String = ""
                    Dim idleTipo_risposta As String = ""
                    Dim urlRedirect As String = ""
                    RedirezionePaginaSincroProfilazioneAzienda(objParametriAgenda, idleParametroDue, idleTipo_risposta, urlRedirect)

                    Response.Redirect(urlRedirect)
                End If

            End If

        End If

        ' VAnni: 2/8/2018: attivazione delle schede per ricette 2018
        Dim dtRicetteOrdiniDiLavoroAttivi As DataTable = xLeggiCfgVerifica.Leggi(0, "RicetteOrdiniDiLavoro2018", "", "", objParametri_Server)

        If dtRicetteOrdiniDiLavoroAttivi.Rows.Count > 0 AndAlso dtRicetteOrdiniDiLavoroAttivi(0)("valore") <> "" Then
            RicetteOrdiniDiLavoroAttivi = CBool(dtRicetteOrdiniDiLavoroAttivi(0)("valore"))
        End If

        'Grilli: 5/11/2018 attivazione delle tab per le tipologie di operazioni
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT As DataTable = objUtentiImpostazioni.Leggi_Utente_Poi_SuperUser(
                                            enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_TIPI_GRUPPI_OPERAZIONI_VISIBILI_MENU_AGENDA, 1,
                                            enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

        If DT.Rows.Count > 0 AndAlso Not IsDBNull(DT.Rows(0).Item("Impostazione_Valore_1")) AndAlso DT.Rows(0).Item("Impostazione_Valore_1") <> "" Then
            Dim tipi() As String = DT.Rows(0).Item("Impostazione_Valore_1").Split("|")
            VisualizzaTabColturali = False
            VisualizzaTabMagContab = False
            VisualizzaTabAudit = False
            VisualizzaTabMacchine = False
            VisualizzaTabZoo = False
            For Each s As String In tipi
                Select Case s
                    Case "C"
                        VisualizzaTabColturali = True
                    Case "E", "E6", "E10"
                        VisualizzaTabMagContab = True
                    Case "Z"
                        VisualizzaTabZoo = True
                    Case "P"
                        VisualizzaTabMacchine = True
                    Case "V"
                        VisualizzaTabAudit = True
                End Select
            Next
        End If

        ' nuova importazione dati app
        Dim objAppHelper As New AgronicaCoreModello.AppHelper
        CaricaDatiApp = objAppHelper.Leggi_CaricaDati_APP(SincroDatiApp, objParametri_Server)

        ' se presente configurazione per GIASAPP importa i dati di tutte le aziende
        If objUtentiImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_NUOVO_INTERVENTO, objParametri_Utenti, 2) = "1" Then
            ImportaSoloAziendaSelezionata = False
        End If

        'Controllo se è stata richiesta la preselezione di una tab
        If IsNumeric(Request.QueryString("defaulttab")) Then
            DefaultTab = CInt(Request.QueryString("defaulttab"))
        End If
        If Not IsNothing(Request.QueryString("Mode")) Then
            Mode = Stringa_Decodifica(Request.QueryString("Mode").ToString, AgroKey_EncoderDecoder, Server)
        End If

        ricetta_cod.Value = "0"
        If Not IsNothing(Request.QueryString("r")) Then
            ricetta_cod.Value = Stringa_Decodifica(Request.QueryString("r").ToString, AgroKey_EncoderDecoder, Server)
        End If
        Master.Lbl_Titolo.Text = AgronicaAgenda_2010.MenùAgenda
        Master.flag_MostraBtnEsci = True

        ' Forzo il titolo pagina in base al tab di default
        If Master.flag_MenuBS_2017 Then
            Master.Lbl_Titolo.Text = ""
            If DefaultTab = 7 Then
                Master.SetTitoloPagina(21)
                Master.Header_versione = VERSIONE_HEADER_DEFAULT
            ElseIf DefaultTab = 5 Then
                Master.SetTitoloPagina(73)
            Else
                Master.Header_versione = VERSIONE_HEADER_DEFAULT
                Dim qr_gis As String = Request.QueryString("gis")
                If qr_gis Is Nothing OrElse qr_gis <> "true" Then
                    Master.SetTitoloPagina(7)
                    Master.Lbl_Titolo.Text = AgronicaAgenda_2010.MenùAgenda
                End If
            End If
        End If

        ' Setto la visibilità dei bottoni in Master
        Master.flag_pag_MenuAgenda = True

        StrLavCodRicettabili = CostantiPersonalizzate.STR_OP_RICETTABILI

        'Escludo le Ricette di Irrigazione se la chiave IrrigazioneBS è false
        'perchè le Irrigazioni vecchie non le gestivano le ricette
        ListLavCodRicettabili = StrLavCodRicettabili.Split(",")

        If Not IsNothing(ListLavCodRicettabili) AndAlso ListLavCodRicettabili.Length > 0 Then
            Dim cf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dtIrrigazione As DataTable = cf.Leggi(0, "IrrigazioneBS", " valore = 'true' ", "", HttpContext.Current.Session("ASG_objParametri_Server"))

            If IsNothing(dtIrrigazione) OrElse dtIrrigazione.Rows.Count = 0 Then
                ListLavCodRicettabili = ListLavCodRicettabili.Where(Function(value) value <> "1").ToArray()
            End If

        End If

        StrLavCodRicettabili = String.Join(",", ListLavCodRicettabili.ToArray())

        If Not IsPostBack Then

            'imposto la finestra temporale di default
            data_inizio.Value = If(objParametri_Server.FinestraTemporaleInizio = AGRODATAINIZIO, "", objParametri_Server.FinestraTemporaleInizio.ToShortDateString)
            data_fine.Value = If(objParametri_Server.FinestraTemporaleFine = AGRODATAFINE, "", objParametri_Server.FinestraTemporaleFine.ToShortDateString)

            'Se non è stata impostata nessuna finestra temporale allora prendo l'annata agraria
            If data_inizio.Value = "" AndAlso data_fine.Value = "" Then

                Dim d1, d2 As Date
                d1 = New Date(Date.Now.Year, 1, 1)
                d2 = New Date(Date.Now.Year, 12, 31)
                'Dim xAnnata As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                'xAnnata.AnnataAgraria(Now, d1, d2, objParametri_Utenti)

                data_inizio.Value = d1.ToShortDateString
                data_fine.Value = d2.ToShortDateString

            End If

            DistruggiSessionVecchie()
            ControlloPermessiUtente()


            hf_LavCodRaccolta.Value = CostantiPersonalizzate.LAVCOD_RACCOLTA
            hf_LinkPaginaOrigine.Value = getLink()

        End If

    End Sub


#Region "Utility"
    Public Property Property_hf_esistonoCostiCollegatiCDG() As String
        Get
            Return hf_esistonoCostiCollegatiCDG.Value
        End Get
        Set(value As String)
            hf_esistonoCostiCollegatiCDG.Value = value
        End Set
    End Property

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Private Sub DistruggiSessionVecchie()
        'Master
        Session.Remove("DT_Impianti")
        Session.Remove("dtScarico")
        Session.Remove("dtScarico_old")
        Session.Remove("DT_CentriCosto")
        Session.Remove("Dt_Prodotti")
        Session.Remove("Dt_Macchine")
        Session.Remove("Dt_Manodopera")
        Session.Remove("Dt_Terzisti")


        'Page Trattamenti
        Session.Remove("UtenteAbilitato_Lettura")
        Session.Remove("UtenteAbilitato_Modifica")
        Session.Remove("DtAvv")
        Session.Remove("DtAvvGru")
        Session.Remove("vs_dtDosi")
        Session.Remove("DoseMax")
        Session.Remove("Udm_Cod_Max")
        Session.Remove("DoseEtichetta")
        Session.Remove("Dpi_Cod")
        Session.Remove("modulo")
        Session.Remove("Disciplinare_Des")
        Session.Remove("Id_Rcdpi")

        'pagina Installazione trappole 
        Session.Remove("Tabella")

        Session.Remove("visualizzaRiferimentoAlfanumericoImpianto")
        Session.Remove("ordinaDataUltimoImpianto")

    End Sub

    Private Sub ControlloPermessiUtente()

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        'Gestione Nuovo Allegato
        hf_UtenteAbilitatoGestioneNuovoAllegato.Value = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                              Session("ASG_IdServizio"),
                                                                              enum_Security_Attivita.Documentale_Inser,
                                                                              enum_Security_Operazione.Modifica,
                                                                              Now, "",
                                                                              objParametri_Utenti)
        'Gestione Visualiza Allegato
        hf_UtenteAbilitatoGestioneVisualizaAllegato.Value = objPermessi.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                              Session("ASG_IdServizio"),
                                                                              enum_Security_Attivita.Documentale_Lista,
                                                                              enum_Security_Operazione.Lettura,
                                                                              Now, "",
                                                                              objParametri_Utenti)

        hf_UtenteAbilitatoCostiRicaviDaQdCeCdG.Value = permessi.getPermesso(enum_Security_Attivita.Inserimento_CostiRicavi_Da_QdC_CdG).Scrittura

        Dim leggi_CDG_R As New CDG_BIZ_R
        Dim tipoCdG = leggi_CDG_R.GetTipoCdG(objParametriAgenda.Piva, objParametri_Server, objParametri_Utenti)
        If tipoCdG <> enum_TipoCdG.NuovoTipo Then
            hf_UtenteAbilitatoFlagMostraBtnSalvaCDG.Value = False
        End If



    End Sub

    Private Shared Function SeDocumentoRicevutoLight(OpUtil As AgronicaCoreModello.Utility_Operazioni) As Boolean

        Dim DocumentoRicevutoLight As Boolean

        Dim objconfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim DocumentoRicevutoLight_str As String = objconfigSiti.Leggi_Valore(Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                                              "DocumentoRicevutoLight",
                                                                              "",
                                                                              "",
                                                                              objParametri_Server)

        If DocumentoRicevutoLight_str = "" Then
            DocumentoRicevutoLight = False
        Else
            DocumentoRicevutoLight = DocumentoRicevutoLight_str
        End If

        If DocumentoRicevutoLight AndAlso OpUtil.SeSetupDoc2021(objParametri_Server) Then
            DocumentoRicevutoLight = False
        End If

        Return DocumentoRicevutoLight

    End Function

    Public Function SincroMenuVerificaPermessiAccesso() As Boolean

        Dim rval As Boolean = False

        Dim DT_Permessi As DataTable
        Dim i As Integer
        Dim ID_Permesso As Integer
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read

        DT_Permessi = objUtenti.Leggi_DatiUtente_e_DatiSuperUser(CStr(Session("ASG_Utente_Username")),
                                                "",
                                                "",
                                                0,
                                                0,
                                                Date.Now,
                                                CType(Now.Hour, Short),
                                                True,
                                                5,
                                                "", "",
                                                objParametri_Utenti)

        If Not IsNothing(DT_Permessi) Then

            For i = 0 To DT_Permessi.Rows.Count - 1

                ID_Permesso = DT_Permessi.Rows(i).Item("Id_Attivita")

                Select Case ID_Permesso
                    Case enum_Security_Attivita.ManutenzioneArchivi_SincronizzazioneHarvard,
                        enum_Security_Attivita.ManutenzioneArchivi_ImportazioneHarvard,
                        enum_Security_Attivita.ManutenzioneArchivi_ImportDateRaccolta_Harvard,
                        enum_Security_Attivita.ManutenzioneArchivi_SincronizzazioneArcView,
                        enum_Security_Attivita.ManutenzioneArchivi_Import_DDTFatture_Seled,
                        enum_Security_Attivita.ManutenzioneArchivi_SincroRaccoltaCCCI,
                        enum_Security_Attivita.ManutenzioneArchivi_Import_Anagrafiche_Seled,
                        enum_Security_Attivita.ManutenzioneArchivi_Importa_AVEPA_Vino,
                        enum_Security_Attivita.ManutenzioneArchivi_Gias_2_Gias,
                        enum_Security_Attivita.ManutenzioneArchivi_ImportaMagazzino_XmlPubblico,
                        enum_Security_Attivita.ManutenzioneArchivi_ImportaAnagrafiche_XLS2GIAS,
                        enum_Security_Attivita.ManutenzioneArchivi_Importazione_Catasto_Uniforma,
                        enum_Security_Attivita.ManutenzioneArchivi_ImportazioneAnagraficheZespri,
                        enum_Security_Attivita.ManutenzioneArchivi_ImportazioneQDCZespri,
                        enum_Security_Attivita.ManutenzioneArchivi_EsportazioneAgendaSISCO,
                        enum_Security_Attivita.ManutenzioneArchivi_EsportazioneAGEA,
                        enum_Security_Attivita.ManutenzioneArchivi_AccessoMenu

                        rval = True

                End Select

            Next

        End If

        Return rval
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Preleva_Preferiti_Stampe() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) OrElse IsNothing(objParametri_Utenti) Then
            r.Sessione = False
            Return r
        End If


        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT As DataTable = objUtentiImpostazioni.Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, 1,
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

        If DT.Rows.Count > 0 AndAlso Not IsDBNull(DT.Rows(0).Item("Impostazione_Valore_1")) AndAlso DT.Rows(0).Item("Impostazione_Valore_1") <> "" Then

            Dim codstr() As String = DT.Rows(0).Item("Impostazione_Valore_1").Split("|")
            Dim listaStampe As New List(Of String)

            For Each stmp In codstr
                If IsNumeric(stmp) Then
                    Dim stampacod As String = "Stampa-" & stmp
                    Dim stampades As String = New AgronicaCoreMetaSchemaDAL.StampeReport().LeggiDescrizione(CInt(stmp), "", objParametri_Server)

                    listaStampe.Add(" "" " & stampacod & " "": "" " & stampades & " "" ")
                End If
            Next

            r.RispostaStringa = "{" & String.Join(",", listaStampe) & "}"

        Else
            r.RispostaStringa = "{}"
        End If

        r.RispostaOK = True
        Return r

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function Gestione_Operazione_ConRigheSelezionate(ByVal lav_cod As String, ByVal tipo_operazione As String, ByVal specie As String,
                                                                   ByVal righe_selezionate As String) As RispostaStandard

        Dim r As New RispostaStandard

        'Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim objParametriAgenda As New ParametriAgenda
        Dim TargetUrl = ""
        Dim ParametroDue = ""
        Dim tipo_risposta = ""

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim jss = New JavaScriptSerializer()
        Dim operazioni As List(Of SelezionaMenuAgenda_Nuovo) = jss.Deserialize(Of List(Of SelezionaMenuAgenda_Nuovo))(righe_selezionate)

        Dim Id_Agenda As String = ""
        Dim Blocco_Flag As String = ""
        Dim Sa_Cod As Integer
        Dim Veg_Cod As Integer = 0
        Dim Lav_Des As String = ""
        Dim Rag_Soc As String = ""

        Dim Origine As String = "../Menu/MenuBS_Agenda_Nuovo.aspx"

        Select Case tipo_operazione
            Case "5d" 'MODIFICA MULTIPLA OPERAZIONI

                Dim Lavorazioni_Selezionate As Integer = 0

                Dim Matrice_Agenda(,) As String
                Dim colonna As Integer = 0
                Dim Data_Inizio As Date

                For Each riga As SelezionaMenuAgenda_Nuovo In operazioni

                    ReDim Preserve Matrice_Agenda(6, colonna)

                    Id_Agenda = CInt(riga.Id_Agenda)
                    lav_cod = CInt(riga.Lav_Cod)
                    Veg_Cod = CInt(riga.Veg_Cod)
                    Data_Inizio = CDate(riga.Data)
                    Lav_Des = CStr(riga.Lav_Des)
                    Blocco_Flag = CStr(riga.Blocco_Flag)
                    Rag_Soc = CStr(riga.Rag_Soc)
                    Sa_Cod = CInt(riga.Sa_Cod)

                    If Blocco_Flag = "1" Then
                        r.RispostaOK = False
                        r.Errore = AgronicaAgenda_2010.ImpossibileModificareOperazioniBloccate
                        Return r
                    End If

                    'se non ho selezionato una lavorazione..
                    If (Id_Agenda = -1) OrElse (lav_cod = -1) OrElse (lav_cod = 1022) Then
                        r.RispostaOK = False
                        r.Errore = AgronicaAgenda_2010.ModificabiliSoloOperazioniColturali
                        Return r
                    End If

                    Matrice_Agenda(0, colonna) = Id_Agenda
                    Matrice_Agenda(1, colonna) = lav_cod
                    Matrice_Agenda(2, colonna) = Lav_Des
                    Matrice_Agenda(3, colonna) = Data_Inizio.ToShortDateString
                    Matrice_Agenda(4, colonna) = Rag_Soc
                    Matrice_Agenda(5, colonna) = Sa_Cod

                    colonna += 1

                    Lavorazioni_Selezionate += 1

                Next

                If Lavorazioni_Selezionate <= 0 Then
                    r.RispostaOK = False
                    r.Errore = AgronicaAgenda_2010.SelezionareAlmenoUnOperazione
                    Return r
                End If

                Dim StrNodiVariabili As String = ""

                If Matrice_Agenda IsNot Nothing Then

                    Dim objVS As New AgronicaCoreXML.XML_Stampe

                    For i As Integer = 0 To UBound(Matrice_Agenda, 2)

                        Dim vVarStampe(6) As ElementoStampe

                        vVarStampe(0).Nome = "piva"
                        vVarStampe(0).Valore = objParametriAgenda.Piva

                        vVarStampe(1).Nome = "rag_soc"
                        vVarStampe(1).Valore = Matrice_Agenda(4, i)

                        vVarStampe(2).Nome = "id_agenda"
                        vVarStampe(2).Valore = Matrice_Agenda(0, i)

                        vVarStampe(3).Nome = "lav_cod"
                        vVarStampe(3).Valore = Matrice_Agenda(1, i)

                        vVarStampe(4).Nome = "des_lib"
                        vVarStampe(4).Valore = Matrice_Agenda(2, i)

                        vVarStampe(5).Nome = "data_movimento"
                        vVarStampe(5).Valore = Matrice_Agenda(3, i)

                        vVarStampe(6).Nome = "sa_cod"
                        vVarStampe(6).Valore = Matrice_Agenda(5, i)

                        StrNodiVariabili &= objVS.XML_VariabiliStampe(vVarStampe)

                    Next

                End If

                Dim XmlDoc As New System.Xml.XmlDocument
                Dim Xml_FiltroStampa As System.Xml.XmlElement = XmlDoc.CreateElement("FiltroAgenda")
                XmlDoc.AppendChild(Xml_FiltroStampa)
                Xml_FiltroStampa.InnerXml = StrNodiVariabili
                Dim StrVariabiliStampe As String = XmlDoc.InnerXml

                If StrVariabiliStampe <> "" Then

                    Dim objWebW As New AgronicaCoreVarieDAL.Web_ComunicazionePagine_W
                    Dim Unid_Operazione As String = System.Guid.NewGuid.ToString
                    Dim res As Boolean = objWebW.Scrivi(Unid_Operazione, 0, enum_TipoOperazioneDB.Scrittura, 0, "", "", StrVariabiliStampe, "", objParametri_Server)

                    If res Then
                        TargetUrl = "../Operazioni/ModificaMultipla_Operazioni.aspx?unid_operazioni=" & Stringa_Codifica(Unid_Operazione, AgroKey_EncoderDecoder, objParametri_Server)
                    End If

                End If



            Case "6b" 'CREAZIONE RICETTA DA OPERAZIONI

                Dim Matrice_Agenda(,) As String
                Dim colonna = 0
                Dim Ricetta_Cod As Integer
                Dim Data_Inizio, Data_Fine As Date
                Dim Lavorazioni_Selezionate As Integer
                Dim VegCodOld As Integer

                For Each riga As SelezionaMenuAgenda_Nuovo In operazioni

                    ReDim Preserve Matrice_Agenda(5, colonna)

                    Id_Agenda = CInt(riga.Id_Agenda)
                    lav_cod = CInt(riga.Lav_Cod)
                    Veg_Cod = CInt(riga.Veg_Cod)
                    Ricetta_Cod = CInt(riga.Ricetta_Cod)
                    Data_Inizio = CDate(riga.Data)
                    Lav_Des = CStr(riga.Lav_Des)

                    Matrice_Agenda(0, colonna) = CStr(Id_Agenda)
                    Matrice_Agenda(1, colonna) = CStr(lav_cod)
                    Matrice_Agenda(2, colonna) = CStr(Veg_Cod)
                    Matrice_Agenda(3, colonna) = CStr(Ricetta_Cod)
                    Matrice_Agenda(4, colonna) = CStr(Data_Inizio.ToShortDateString)

                    colonna += 1

                    Lavorazioni_Selezionate += 1

                    'verifico che la specie sia una sola
                    If UBound(Matrice_Agenda, 2) = 0 Then
                        VegCodOld = Veg_Cod
                    Else
                        If VegCodOld <> Veg_Cod Then
                            r.RispostaOK = False
                            r.Errore = AgronicaAgenda_2010.SelezionareOperazioniRegistrateSingolaSpecieCreareRicetta
                            Return r
                        End If
                    End If

                    Select Case lav_cod

                        Case LAVCOD_DISERBO, LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                             LAVCOD_GEODISINFESTAZIONE, LAVCOD_CONCIA_SEME, LAVCOD_DISSECCAMENTO,
                             LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE, LAVCOD_INSTALLAZIONE_TRAPPOLE,
                             LAVCOD_CATTURE_MASSA, LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_FERTIRRIGAZIONE,
                             LAVCOD_TRATTAMENTO_ANTIBUTTERATURA, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                             LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_IRRIGAZIONE, LAVCOD_ARATURA, LAVCOD_DEFOGLIAZIONE,
                             LAVCOD_ANDANAMENTO, LAVCOD_ASPORTAZIONE_ORGANI_INFETTI,
                             LAVCOD_ASSOLCATURA, LAVCOD_CARICO_MANUALE_FRUTTA, LAVCOD_CIMATURA,
                             LAVCOD_DIRADAMENTO_MANUALE, LAVCOD_DISSODAMENTO, LAVCOD_ERPICATURA,
                             LAVCOD_ERPICATURA_ROTANTE, LAVCOD_ESPIANTO, LAVCOD_ESTIRPATURA,
                             LAVCOD_FALCIACONDIZIONATURA, LAVCOD_FALCIATURA_ERBAI, LAVCOD_FORMAZIONE_ARGINELLI,
                             LAVCOD_FRANGIZOLLATURA, LAVCOD_FRESATURA, LAVCOD_GEBIATURA,
                             LAVCOD_IMBALLO_FIENO_ROTOLI, LAVCOD_INTERRAMENTO_PAGLIE, LAVCOD_INTERVENTO_ANTIBRINA,
                             LAVCOD_LAVORAZIONE_CONBINATA, LAVCOD_LAVORAZIONE_TRA_FILA, LAVCOD_LAVORAZIONE_SU_FILA,
                             LAVCOD_LEGATURA, LAVCOD_LIVELLAMENTO, LAVCOD_MANUTENZIONE_ARGINI,
                             LAVCOD_MESSA_DIMORA_PIANTE, LAVCOD_MIETITREBBIATURA, LAVCOD_MINIMUM_TILLAGE,
                             LAVCOD_PACCIAMATURA, LAVCOD_POTATURA_SECCA, LAVCOD_POTATURA_VERDE,
                             LAVCOD_PRESSATURA, LAVCOD_RACCOLTA_LEGNA_POTATURA,
                             LAVCOD_RANGHINATURA, LAVCOD_RINCALZATURA, LAVCOD_RIPPATURA,
                             LAVCOD_RIPUNTATURA, LAVCOD_RIVOLTAMENTO_FORAGGIO, LAVCOD_ROMPICROSTA,
                             LAVCOD_RULLATURA, LAVCOD_SARCHIATURA, LAVCOD_SCARIFICATURA,
                             LAVCOD_SCASSO, LAVCOD_SOD_SEDDING, LAVCOD_TRINCIATURA,
                             LAVCOD_VANGATURA, LAVCOD_ZAPPATURA, LAVCOD_SEMINA,
                             LAVCOD_SOVESCIO, LAVCOD_TRAPIANTO

                        Case Else
                            r.RispostaOK = False
                            r.Errore = AgronicaAgenda_2010.NonÈPossibileCreareLaRicettaRelativaAllOp & Lav_Des & " (" & Data_Inizio.ToShortDateString & ")"
                            Return r
                    End Select


                Next

                If Lavorazioni_Selezionate <= 0 Then
                    r.RispostaOK = False
                    r.Errore = AgronicaAgenda_2010.SelezionareAlmenoUnOperazionePerCreareRicetta
                    Return r
                End If

                Dim XmlDoc As New System.Xml.XmlDocument
                Dim Xml_FiltroStampa As System.Xml.XmlElement

                Dim StrVariabiliStampe As String = ""
                Dim StrNodiVariabili As String = ""
                Dim StrNodo As String = ""


                If Matrice_Agenda IsNot Nothing Then

                    Dim objVS As New AgronicaCoreXML.XML_Stampe

                    For i = 0 To UBound(Matrice_Agenda, 2)

                        Dim vVarStampe(2) As ElementoStampe

                        vVarStampe(0).Nome = "piva"
                        vVarStampe(0).Valore = objParametriAgenda.Piva

                        vVarStampe(1).Nome = "sa_cod"
                        vVarStampe(1).Valore = objParametriAgenda.Sa_Cod

                        'per ora mando un solo id_reg ed un solo appezza..
                        vVarStampe(2).Nome = "id_agenda"
                        vVarStampe(2).Valore = Matrice_Agenda(0, i)

                        StrNodo = objVS.XML_VariabiliStampe(vVarStampe)

                        StrNodiVariabili &= StrNodo

                        If CDate(Matrice_Agenda(4, i)) < Data_Inizio Then
                            Data_Inizio = CDate(Matrice_Agenda(4, i))
                        End If

                        If CDate(Matrice_Agenda(4, i)) > Data_Fine Then
                            Data_Fine = CDate(Matrice_Agenda(4, i))
                        End If

                    Next

                End If


                Xml_FiltroStampa = XmlDoc.CreateElement("FiltroAgenda")

                Xml_FiltroStampa.SetAttribute("veg_cod", Veg_Cod)

                Xml_FiltroStampa.SetAttribute("data_inizio", Data_Inizio.ToShortDateString)
                Xml_FiltroStampa.SetAttribute("data_fine", Data_Fine.ToShortDateString)

                Xml_FiltroStampa.SetAttribute("username", CStr(HttpContext.Current.Session("ASG_Utente_Username")))
                Xml_FiltroStampa.SetAttribute("user_profilo", CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS")))

                XmlDoc.AppendChild(Xml_FiltroStampa)

                Xml_FiltroStampa.InnerXml = StrNodiVariabili

                StrVariabiliStampe = XmlDoc.InnerXml

                HttpContext.Current.Session("FiltroAgenda") = StrVariabiliStampe

                TargetUrl = "../Ricette/Ricette_Edit.aspx?r=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, objParametri_Server) &
                            "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, objParametri_Server) &
                            "&destinazione=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, objParametri_Server) &
                            "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, objParametri_Server) &
                            "&data_inizio=" & Stringa_Codifica(Data_Inizio.ToShortDateString, AgroKey_EncoderDecoder, objParametri_Server) &
                            "&data_fine=" & Stringa_Codifica(Data_Fine.ToShortDateString, AgroKey_EncoderDecoder, objParametri_Server) &
                            "&veg_cod=" & Stringa_Codifica(Veg_Cod, AgroKey_EncoderDecoder, objParametri_Server) &
                            "&tipo_ricetta=" & Stringa_Codifica(enum_TipoRicetta.Standard_Destinazioni, AgroKey_EncoderDecoder, objParametri_Server)


            Case "45" 'Blocca Operazioni Agenda

                Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W

                For Each riga As SelezionaMenuAgenda_Nuovo In operazioni
                    'se operazione zoo azzera il sa_cod
                    If CInt(riga.Lav_Cod) >= 3000 AndAlso CInt(riga.Lav_Cod) < 4000 Then
                        lav_cod = CInt(riga.Lav_Cod)
                        riga.Sa_Cod = 0
                    End If
                    Dim resp As Boolean = objAgenda.Agenda_Blocca(riga.Piva, riga.Sa_Cod, riga.Id_Agenda, objParametri_Server.UsernameOperazione, DateTime.Now, "", objParametri_Server)
                Next

                'ricarico la griglia kendo con le modifiche apportate
                '(se operazione zoo azzera griglia zoo, altrimenti griglia operazioni)
                If lav_cod >= 3000 AndAlso lav_cod < 4000 Then
                    ParametroDue = "<script language='javascript'>" & vbCrLf &
                               "       $('#divKendoZoo').html(''); //Pulisco la tabella" & vbCrLf &
                               "       CaricaGrigliaZoo();// Aggiorno la tabella" & vbCrLf &
                               " </script>"
                Else
                    ParametroDue = "<script language='javascript'>" & vbCrLf &
                                   "       $('#divKendoOperazioni').html(''); //Pulisco la tabella" & vbCrLf &
                                   "       CaricaGrigliaOperazioni();// Aggiorno la tabella" & vbCrLf &
                                   " </script>"
                End If

                tipo_risposta = "1"

            Case "46" 'Sblocca Operazioni Agenda

                Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W

                For Each riga As SelezionaMenuAgenda_Nuovo In operazioni
                    'se operazione zoo azzera il sa_cod
                    If CInt(riga.Lav_Cod) >= 3000 AndAlso CInt(riga.Lav_Cod) < 4000 Then
                        lav_cod = CInt(riga.Lav_Cod)
                        riga.Sa_Cod = 0
                    End If
                    objAgenda.Agenda_Sblocca(riga.Piva, riga.Sa_Cod, riga.Id_Agenda, objParametri_Server.UsernameOperazione, DateTime.Now, "", objParametri_Server)
                Next

                'ricarico la griglia kendo con le modifiche apportate
                '(se operazione zoo azzera griglia zoo, altrimenti griglia operazioni)
                If lav_cod >= 3000 AndAlso lav_cod < 4000 Then
                    ParametroDue = "<script language='javascript'>" & vbCrLf &
                               "       $('#divKendoZoo').html(''); //Pulisco la tabella" & vbCrLf &
                               "       CaricaGrigliaZoo();// Aggiorno la tabella" & vbCrLf &
                               " </script>"
                Else
                    ParametroDue = "<script language='javascript'>" & vbCrLf &
                                   "       $('#divKendoOperazioni').html(''); //Pulisco la tabella" & vbCrLf &
                                   "       CaricaGrigliaOperazioni();// Aggiorno la tabella" & vbCrLf &
                                   " </script>"
                End If

                tipo_risposta = "1"

        End Select

        r.RispostaOK = True
        r.RispostaStringa = TargetUrl
        r.ParametroDue_stringa = ParametroDue
        r.Tipo = tipo_risposta
        Return r

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function Gestione_Operazione(ByVal lav_cod As String, ByVal tipo_operazione As String, ByVal specie As String, ByVal impianti As String) As RispostaStandard


        Dim permessi = New PermessiUtente()
        Dim r As New RispostaStandard

        'Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        Lingua.Gias_InizializzaCultura_DaSession()

        'Dim jss = New JavaScriptSerializer()
        'Dim listaImpianti As List(Of Reg_Impianti) = jss.Deserialize(Of List(Of Reg_Impianti))(impianti)
        Dim listaImpianti As New List(Of Reg_Impianti)
        If Not String.IsNullOrWhiteSpace(impianti) Then
            Dim arrayChiaviImpianti As String() = impianti.Split("|")

            For Each strChiaviImpianto As String In arrayChiaviImpianti

                Dim chiaviImpianto As String() = strChiaviImpianto.Split("_")

                listaImpianti.Add(New Reg_Impianti() With {
                    .PIVA = chiaviImpianto(0),
                    .SA_COD = chiaviImpianto(1),
                    .APPEZZA = chiaviImpianto(2),
                    .ID_REG = chiaviImpianto(3)
                })
            Next

        End If

        Dim objParametriAgenda As New ParametriAgenda
        Dim TargetUrl
        Dim ParametroDue = ""
        Dim tipo_risposta = ""

        'Dim Id_Agenda As String = ""
        'Dim Blocco_Flag As String = ""
        'Dim Veg_Cod As Integer = 0
        'Dim Rag_Soc As String = ""

        Dim Origine As String = "../Menu/MenuBS_Agenda_Nuovo.aspx"

        Select Case tipo_operazione

            Case "5a" 'RAGGRUPPAMENTO


                'Case "5b" 'AGGIUNGI COSTI ACCESSORI

                '    Dim Matrice_Agenda(,) As String
                '    Dim colonna = 0
                '    Dim Data_Inizio, Data_Fine As Date
                '    Dim Lavorazioni_Selezionate As Integer


                '    For Each riga As SelezionaMenuAgenda_Nuovo In operazioni

                '        ReDim Preserve Matrice_Agenda(5, colonna)

                '        Id_Agenda = CInt(riga.Id_Agenda)
                '        lav_cod = CInt(riga.Lav_Cod)
                '        Veg_Cod = CInt(riga.Veg_Cod)
                '        Data_Inizio = CDate(riga.Data)
                '        Rag_Soc = CStr(riga.Rag_Soc)

                '        Blocco_Flag = CStr(riga.Blocco_Flag)

                '        If Blocco_Flag = "1" Then
                '            r.RispostaOK = False
                '            r.Errore = "Impossibile modificare operazioni bloccate!"
                '            Return r
                '        End If

                '        'se non ho selezionato una lavorazione..
                '        If (Id_Agenda = -1) Or (lav_cod = -1) Or (lav_cod = 1022) Or (lav_cod = 1022) Then
                '            r.RispostaOK = False
                '            r.Errore = "E' possibile selezionare solo operazioni colturali!"
                '            Return r
                '        End If

                '        Matrice_Agenda(0, colonna) = Id_Agenda
                '        Matrice_Agenda(1, colonna) = lav_cod
                '        Matrice_Agenda(2, colonna) = Veg_Cod
                '        Matrice_Agenda(3, colonna) = Data_Inizio.ToShortDateString
                '        Matrice_Agenda(4, colonna) = Rag_Soc

                '        colonna += 1

                '        Lavorazioni_Selezionate += 1

                '    Next

                '    If Lavorazioni_Selezionate <= 0 Then
                '        r.RispostaOK = False
                '        r.Errore = "Selezionare almeno un'operazione!"
                '        Return r
                '    End If

                '    Dim XmlDoc As New System.Xml.XmlDocument
                '    Dim Xml_FiltroStampa As System.Xml.XmlElement

                '    Dim StrVariabiliStampe As String = ""
                '    Dim StrNodiVariabili As String = ""
                '    Dim StrNodo As String = ""

                '    If Not Matrice_Agenda Is Nothing Then

                '        Dim objVS As New AgronicaCoreXML.XML_Stampe

                '        For i = 0 To UBound(Matrice_Agenda, 2)

                '            Dim vVarStampe(7) As ElementoStampe

                '            vVarStampe(0).Nome = "piva"
                '            vVarStampe(0).Valore = objParametriAgenda.Piva

                '            vVarStampe(1).Nome = "rag_soc"
                '            vVarStampe(1).Valore = Matrice_Agenda(4, i)

                '            vVarStampe(2).Nome = "sa_cod"
                '            vVarStampe(2).Valore = objParametriAgenda.Sa_Cod

                '            vVarStampe(3).Nome = "sa_nome"
                '            vVarStampe(3).Valore = ""

                '            vVarStampe(4).Nome = "id_agenda"
                '            vVarStampe(4).Valore = Matrice_Agenda(0, i)

                '            vVarStampe(5).Nome = "lav_cod"
                '            vVarStampe(5).Valore = Matrice_Agenda(1, i)

                '            vVarStampe(6).Nome = "des_lib"
                '            vVarStampe(6).Valore = Matrice_Agenda(2, i)

                '            vVarStampe(7).Nome = "data_movimento"
                '            vVarStampe(7).Valore = Matrice_Agenda(3, i)

                '            StrNodo = objVS.XML_VariabiliStampe(vVarStampe)

                '            StrNodiVariabili = StrNodiVariabili & StrNodo


                '        Next

                '    End If

                '    Xml_FiltroStampa = XmlDoc.CreateElement("FiltroAgenda")

                '    XmlDoc.AppendChild(Xml_FiltroStampa)

                '    Xml_FiltroStampa.InnerXml = StrNodiVariabili

                '    StrVariabiliStampe = XmlDoc.InnerXml

                '    'lo metto in objGiasOnline.Xml_Generico e
                '    'quando sonoi n gestione richieste lo rimetto in sessione
                '    'Session("FiltroAgenda") = StrVariabiliStampe

                '    Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                '    objGiasOnline.Cul_Cod = objParametriAgenda.Cul_Cod
                '    objGiasOnline.DataSelezionata = objParametriAgenda.Data
                '    objGiasOnline.Id_Agenda = 0
                '    objGiasOnline.Lavorazione = 0
                '    objGiasOnline.Operazione = enum_TipoOperazioneDB.Modifica
                '    objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.CostiAccessori_Eredita
                '    objGiasOnline.Piva = objParametriAgenda.Piva
                '    objGiasOnline.Sa_Cod = objParametriAgenda.Sa_Cod
                '    Dim specie_cod As Integer = 0
                '    If IsNumeric(objParametriAgenda.Veg_Cod.Split("/")(0)) AndAlso CInt(objParametriAgenda.Veg_Cod.Split("/")(0)) > 0 Then
                '        specie_cod = CInt(objParametriAgenda.Veg_Cod.Split("/")(0))
                '    End If
                '    objGiasOnline.Veg_Cod = specie_cod

                '    objGiasOnline.Xml_Generico = New StringBuilder(StrVariabiliStampe)

                '    If Not IsNothing(ConfigurationSettings.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationSettings.AppSettings("LinkAgronicaAgenda2010") <> "" Then
                '        objGiasOnline.LinkAgronicaAgenda2010 = ConfigurationSettings.AppSettings("LinkAgronicaAgenda2010")
                '    Else
                '        objGiasOnline.LinkAgronicaAgenda2010 = ""
                '    End If



                '    TargetUrl = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline( _
                '                              Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
                '                              objGiasOnline)


                '    '---------------------------------------

            Case "5c" 'FILTRA E AGGIUNGI COSTI ACCESSORI alle operazioni


                Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                objGiasOnline.Cul_Cod = objParametriAgenda.Cul_Cod
                objGiasOnline.DataSelezionata = objParametriAgenda.Data
                objGiasOnline.Id_Agenda = 0
                objGiasOnline.Lavorazione = 0
                objGiasOnline.Operazione = enum_TipoOperazioneDB.Modifica
                objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.FiltroImpresa_new4_E_CostiAccessori_Eredita
                objGiasOnline.Piva = objParametriAgenda.Piva
                objGiasOnline.Sa_Cod = objParametriAgenda.Sa_Cod
                Dim specie_cod As Integer = 0
                If IsNumeric(objParametriAgenda.Veg_Cod.Split("/")(0)) AndAlso CInt(objParametriAgenda.Veg_Cod.Split("/")(0)) > 0 Then
                    specie_cod = CInt(objParametriAgenda.Veg_Cod.Split("/")(0))
                End If
                objGiasOnline.Veg_Cod = specie_cod

                If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
                    objGiasOnline.LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
                Else
                    objGiasOnline.LinkAgronicaAgenda2010 = ""
                End If

                TargetUrl = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                             Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                             objGiasOnline)


            Case "6c" 'MENU RICETTE

                'menu ricette
                TargetUrl = "../Ricette/Ricette_Manager.aspx?origine=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, objParametri_Server)

                '    'Case "11"
                '    '    ''blocchi e impostazioni
                '    '    CaricaDatiListaBloccoControlli_ETICHETTA()
                '    '    Exit Function
                '    '    'blocchi e impostazioni
                '    '    tipo_risposta = "2"


            Case "6d" 'CREAZIONE RICETTA PER PLANNING

                TargetUrl = "../Ricette/Ricette_Edit.aspx?r=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, objParametri_Server) &
                            "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, objParametri_Server) &
                            "&destinazione=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, objParametri_Server) &
                            "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, objParametri_Server) &
                            "&tipo_ricetta=" & Stringa_Codifica(enum_TipoRicetta.Standard_Destinazioni_Planning, AgroKey_EncoderDecoder, objParametri_Server)


            Case "6e" 'NUOVA RICETTA 

                Dim tipoRicetta = lav_cod
                TargetUrl = "../Ricette/Ricette_Edit.aspx?r=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, objParametri_Server) &
                            "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, objParametri_Server) &
                            "&destinazione=" & Stringa_Codifica("../Menu/MenuBS_Agenda_Nuovo.aspx", AgroKey_EncoderDecoder, objParametri_Server) &
                            "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, objParametri_Server) &
                            "&tipo_ricetta=" & Stringa_Codifica(tipoRicetta, AgroKey_EncoderDecoder, objParametri_Server)

                'objParametriAgenda.Id_Agenda = 0
                'objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura

                ''objParametriAgenda.Data = Txt_DataInizio.Text
                'objParametriAgenda.Lav_Cod = lav_cod
                'objParametriAgenda.Veg_Cod = specie

                'objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta

                'objParametriAgenda.TipoRicetta = enum_TipoRicetta.Standard_Destinazioni ' Qs_Tipo_Ricetta

                ''If ComboPianificazione_Get_COD() <> 0 Then
                ''    objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Planning
                ''Else
                'objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
                ''End If

                ''objParametriAgenda.Programmazione_Cod = ComboPianificazione_Get_COD()

                'Dim OpUtil As New Utility_NS.Utility_Operazioni
                'TargetUrl = OpUtil.LinkPagina_from_LavCod(lav_cod, objParametriAgenda)

                ''Dim Unid_Ricetta As String = System.Guid.NewGuid.ToString
                'Dim Unid_Ricetta As String = ""

                ''Dim StringaXmlCreazione As String = XML_GeneraStringa_Ricetta()

                ''Dim objWebW As New AgronicaCoreVarieDAL.Web_ComunicazionePagine_W
                ''Dim res As Boolean = False
                ''res = objWebW.Scrivi(Unid_Ricetta, 0, enum_TipoOperazioneDB.Scrittura, 0, "", "", StringaXmlCreazione, "", objParametri_Server)
                ''If res = True Then
                'TargetUrl &= "?unid_ricetta=" & Sicurezza.Stringa_Codifica(Unid_Ricetta, CostantiPersonalizzate.AgroKey_EncoderDecoder) &
                '                            "&operazione_ricetta=" & Sicurezza.Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, CostantiPersonalizzate.AgroKey_EncoderDecoder) &
                '                            "&r=" & Sicurezza.Stringa_Codifica("0", CostantiPersonalizzate.AgroKey_EncoderDecoder)

                'End If



            Case "10" 'verifica conformita

                'Dim Lavorazioni_Selezionate As Integer
                'Dim Lista_Id_Agenda As New List(Of Integer)

                'For Each riga As SelezionaMenuAgenda_Nuovo In operazioni

                '    Id_Agenda = CInt(riga.Id_Agenda)
                '    lav_cod = CInt(riga.Lav_Cod)

                '    Select Case lav_cod
                '        Case 18, 158, 74, 155, 13, 106, 123, 124, 14, 26, 156
                '            Lista_Id_Agenda.Add(Id_Agenda)
                '    End Select

                '    Lavorazioni_Selezionate += 1

                'Next

                'If Lavorazioni_Selezionate <= 0 Then
                '    r.RispostaOK = False
                '    r.Errore = "Selezionare almeno un'operazione!"
                '    Return r
                'End If


                'Dim Elemento_Verifica_Disciplinare As New AgronicaCoreModello.Agenda.Util.Elemento_Verifica_Disciplinare(0, 0, "", Lista_Id_Agenda)
                'Elemento_Verifica_Disciplinare.Piva = objParametriAgenda.Piva
                'HttpContext.Current.Session("Elemento_Verifica_Disciplinare") = Elemento_Verifica_Disciplinare
                'TargetUrl = "../GestioneDisciplinari/Verifica_Disciplinare.aspx"
                objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS

                Dim Elemento_Verifica_Disciplinare As New AgronicaCoreModello.Agenda.Util.Elemento_Verifica_Disciplinare
                Elemento_Verifica_Disciplinare.Piva = objParametriAgenda.Piva
                HttpContext.Current.Session("Elemento_Verifica_Disciplinare") = Elemento_Verifica_Disciplinare
                TargetUrl = "../GestioneDisciplinari/Verifica_DisciplinareBS.aspx"

                'Dim Script As String
                'Script = "<script language='javascript'>" & _
                '            "           var win = window.open('" & TargetUrl & "' ," & _
                '            "           'Verifica Conformita');" & _
                '            "           win.focus(); " & _
                '         " </script> "

                'ParametroDue = Script
                'tipo_risposta = "1"


            Case "13" 'PROFITOSAN

                Dim Link As String = AgronicaCoreGestioneRichieste.profitosan.getLinkSimple(True, HttpContext.Current.Request, New AgronicaCoreGestioneRichieste.AgroWebConfig, objParametri_Server, objParametri_Utenti)

                Dim Script As String = "<script language='javascript'>" &
                            "           var win = window.open('" & Link & "' ," & "'Profitosan');" &
                            "           win.focus(); " &
                            " </script> "

                ParametroDue = Script
                tipo_risposta = "1"


            Case "15" 'GESTIONE MAGAZZINI

                objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

                TargetUrl = "../GestioneMagazzini/GestioneMagazziniBS.aspx"

            Case "16" 'GIASONLINE ANAGRAFICA

                Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuAnagrafeBS", "", "", objParametri_Server)

                If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
                    TargetUrl = "../MenuAnagrafica/Menubs_anagrafica.aspx"
                Else
                    objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                    objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS
                    TargetUrl = AgronicaCoreModello.Utility_Operazioni.Link_GiasOnline_STR(enum_PagineGiasOnline.AlberoImprese, objParametriAgenda)
                End If


            Case "21" 'IMPOSTAZIONE UTENTI

                Dim Utenti_Permessi_R As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                Dim DT2 As DataTable = Utenti_Permessi_R.Leggi(objParametri_Utenti.UtenteUsername, 5, enum_Security_Attivita.Gest_UtentiImpostazioni, enum_Security_Operazione.Lettura, 0, "", "", objParametri_Utenti)
                If DT2.Rows.Count = 0 Then
                    r.RispostaOK = False
                    r.Errore = "Non si dispone dei permessi per la modifica delle Impostazioni Utente"
                    Return r
                End If

                Dim strJS As String = RedirectGestione.ApriPopUp_SitoProfilazione_PassandoDirettamenteIParametri(
                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                   enum_PagineProfilazione_2010.Pagina_Utenti_Impostazioni,
                                   enum_PagineAgenda_2010.Menu,
                                   objParametriAgenda.Piva)

                ParametroDue = strJS
                tipo_risposta = "1"

            Case "22" 'PROFILAZIONE IMPRESA

                Dim Utenti_Permessi_R As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                Dim DT2 As DataTable = Utenti_Permessi_R.Leggi(objParametri_Utenti.UtenteUsername, 5, enum_Security_Attivita.ProfilazioneImpresa, enum_Security_Operazione.Lettura, 0, "", "", objParametri_Utenti)
                If DT2.Rows.Count = 0 Then
                    r.RispostaOK = False
                    r.Errore = "Non si dispone dei permessi per la modifica della Profilazione Impresa"
                    Return r
                End If

                Dim strJS As String = RedirectGestione.ApriPopUp_SitoProfilazione_PassandoDirettamenteIParametri(
                               Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                enum_PagineProfilazione_2010.Pagina_Home,
                               enum_PagineAgenda_2010.Menu,
                               objParametriAgenda.Piva)

                ParametroDue = strJS
                tipo_risposta = "1"

            Case "23" 'GESTIONE CONTATTI

                'menu contatti
                TargetUrl = "../GestioneContatti/Contatti_Manager.aspx?piva=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                    "&origine=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, HttpContext.Current.Session)


                'Case "Anagrafica-New"
                '    TargetUrl = "../MenuAnagrafica/Menubs_anagrafica.aspx"

            Case "24" 'PIANO CONCIMAZIONE

                Dim objConcimazione As New ParametriConcimazione_2017
                objConcimazione.Pagina_Richiesta = enum_PaginePianoConcimazione_2017.MenuBS
                objConcimazione.Piva = objParametriAgenda.Piva
                objConcimazione.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                objConcimazione.Pagina_SitoOrigine = enum_PagineAgenda_2010.Menu_BS

                TargetUrl = RedirectGestione.IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriConcimazione_2017(
                    Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objConcimazione)

            Case "25" 'GESTIONE UTENTI

                Dim Utenti_Permessi_R As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                Dim DT2 As DataTable = Utenti_Permessi_R.Leggi(objParametri_Utenti.UtenteUsername, 5, enum_Security_Attivita.Gest_UtentiPermessi, enum_Security_Operazione.Lettura, 0, "", "", objParametri_Utenti)
                If DT2.Rows.Count = 0 Then
                    r.RispostaOK = False
                    r.Errore = "Non si dispone dei permessi per la modifica degli Utenti"
                    Return r
                End If


                Dim strJS As String = RedirectGestione.ApriPopUp_SitoProfilazione_PassandoDirettamenteIParametri(
                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                   enum_PagineProfilazione_2010.Pagina_Utenti_Lista,
                                   enum_PagineAgenda_2010.Menu,
                                   objParametriAgenda.Piva)

                ParametroDue = strJS
                tipo_risposta = "1"

            Case "26" 'ANALISI DATI SCHEDE RILIEVI

                objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

                TargetUrl = "../DataAnalisiBI/AnalisiRilievi/AnalisiRilievi.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, objParametri_Server)

            Case "27" 'PLANNING

                objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

                Dim ParametriPlanning As New ParametriPlanning()
                ParametriPlanning.PaginaProvenienza = enum_PagineGiasOnline.MenuAgenda
                ParametriPlanning.PaginaRichiesta = enum_CodificaPagPlanning.PianificazioneVegetale
                ParametriPlanning.Cuaa = CUAA_from_PIVA(objParametri_Server, objParametriAgenda.Piva)
                ParametriPlanning.Piva = objParametriAgenda.Piva
                Dim strJS As String = RedirectGestione.ApriPopUp_Sito_AgronicaPlanning_PassandoDirettamente_ParametriPlanning(
                                     Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriPlanning)

                ParametroDue = strJS
                tipo_risposta = "1"

            Case "28" 'Tabelle LookUp

                objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

                TargetUrl = "../configOpzioni/Colturali/Rilievi/cfgRilievi.aspx"


            Case "29" 'Report Sostenibilità

                objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

                TargetUrl = "../GestioneDisciplinari/Verifica_Sostenibilita.aspx"

            Case "30" 'Torna al menu precedente

                objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS
                TargetUrl = Utility_Operazioni.Link_GiasOnline_STR(enum_PagineGiasOnline.Menu_principale, objParametriAgenda)

            Case "31" 'Scadenzario

                objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

                TargetUrl = "../Scadenzario/Scad_Lista.aspx"

            Case "32" 'Messaggistica ed SMS

                objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

                TargetUrl = "../PannelloDiControllo/smsSender.aspx"

            Case "43" 'Report Percorsi

                objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

                TargetUrl = "../DataAnalisiBI/ReportPercorsi/ReportPercorsi.aspx"

            Case "33" ' Cartografia Aziendale


                TargetUrl = RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                    Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                    enum_PagineGiasOnline_2010.Cartografia,
                                    enum_PagineAgenda_2010.Menu_BS,
                                    objParametriAgenda.Piva, "", "", 0, "")



            Case "34" ' analisi dei costi

                'Dim orig As String = Stringa_Codifica("../AnalisiCostiProduzione/MenuAnalisiCosti.aspx", AgroKey_EncoderDecoder, Server)
                'Dim dest As String = Stringa_Codifica("../AnalisiCostiProduzione/AnalisiCostiProduzione.aspx", AgroKey_EncoderDecoder, Server)
                'Dim Funzione As String = Stringa_Codifica(CStr(enum_TipoFiltrone.AnalisiCosti), AgroKey_EncoderDecoder, Server)

                'TargetUrl = "../Utility/FiltroImpresa_new4.aspx" & "?o=" & orig & "&d=" & dest & "&f=" & Funzione


                'Dim prm As New ParametriGiasOnline()
                'prm.funzioneoriginedestinazionefiltrone = enum_TipoFiltrone.AnalisiCosti
                'prm.PaginaRichiesta = enum_PagineGiasOnline.FiltroImpresa_new4_AnalisiCosti

                'TargetUrl = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                '                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010, prm)

                objParametriAgenda.Impianti.Clear()

                TargetUrl = "../Filtrone/Filtrone_nuovo.aspx?" &
                     "p_o=" & Stringa_Codifica(enum_PagineAgenda_2010.Menu_BS, AgroKey_EncoderDecoder, Nothing) &
                     "&s_o=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing) &
                     "&p_d=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_AnalisiCosti, AgroKey_EncoderDecoder, Nothing) &
                     "&s_d=" & Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing) &
                     "&t_f=" & Stringa_Codifica(enum_TipoFiltrone.AnalisiCosti, AgroKey_EncoderDecoder, Nothing) &
                     "&c_s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing) &
                     "&piva=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Nothing)

            Case "35" ' Pulsante rilievo punti smart

                Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim xGis2017 As String = objConfigSiti.Leggi_Valore(0, "Gis2017", "", "", objParametri_Server)

                If xGis2017.ToLower = "true" Then

                    HttpContext.Current.Session("_Piva") = objParametriAgenda.Piva
                    TargetUrl = "../Gis/Gis.aspx"

                    '***********************************************************************************************
                    'Gestione del filtrone permanente...

                    Dim leggiFiltroImpianti As New AgronicaCoreVarieDAL.__tmp_FiltroImpianti_R
                    Dim dtImpianti As DataTable = leggiFiltroImpianti.LeggiElencoDaUsername(objParametri_Server.UtenteUsername, objParametri_Server)

                    If (dtImpianti.Rows.Count > 0) Then

                        Dim IDTestataTemp As Integer = dtImpianti.Rows(0)("idTestataTemp")

                        For Each dr In dtImpianti.Rows

                            objParametriAgenda.Impianti.Add(New AgronicaCoreModello.ParametriAgenda_Temp.Impianto With
                            {
                            .Piva = dr("piva"),
                            .Sa_Cod = dr("sa_cod"),
                            .Appezza = dr("appezza"),
                            .ID_Reg = dr("id_reg")
                            })

                        Next

                        TargetUrl &= "?f=true&IDTestataTemp=" & IDTestataTemp

                    End If
                    '***********************************************************************************************

                Else
                    TargetUrl = RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                   enum_PagineGiasOnline_2010.giassmart_punti,
                                   enum_PagineAgenda_2010.Menu_BS,
                                   objParametriAgenda.Piva, "", "", 0, "")
                End If


            Case "36"

                Dim idleUrl As String = ""
                RedirezionePaginaSincroProfilazioneAzienda(objParametriAgenda, ParametroDue, tipo_risposta, idleUrl)

            Case "37" ' PUA ZOO


                Dim script As String = RedirectGestione.ApriPopUp_Sito_AgronicaPUA_PassandoDirettamente_Parametri(
                                 Enum_SiteRedirector.Sito_AgronicaAgenda_2010, enum_AuditPuaTipo.PUA,
                                 HttpContext.Current.Session("ASG_Utente_CodFiscale").ToString, objParametriAgenda.Piva, 0)

                ParametroDue = script
                tipo_risposta = "1"

            Case "53" ' PUA ZOO 2.0

                Dim objConcimazione As New ParametriConcimazione_2017
                objConcimazione.Pagina_Richiesta = enum_PaginePianoConcimazione_2017.MenuBS_PUA
                objConcimazione.Piva = objParametriAgenda.Piva
                objConcimazione.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                objConcimazione.Pagina_SitoOrigine = enum_PagineAgenda_2010.Menu_BS

                TargetUrl = RedirectGestione.IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriConcimazione_2017(
                    Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objConcimazione)


            Case "38" 'Menu Visite

                HttpContext.Current.Session("Sito_Origine") = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS
                TargetUrl = "../Visite/Visite_Lista.aspx"

            Case "39"

                Dim LinkAgronicaAgenda2010 As String = ""

                ''VAnni, 20/04/2018: la lettura avviene dal configurazione siti... l'accrocchio fatto per APOT per leggere il link di agende diverse
                ''   andrà gestito in maniera ortodossa attraverso configurazione di utenti ... 
                ''If Not IsNothing(ConfigurationSettings.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationSettings.AppSettings("LinkAgronicaAgenda2010") <> "" Then
                ''    LinkAgronicaAgenda2010 = ConfigurationSettings.AppSettings("LinkAgronicaAgenda2010")
                ''End If

                Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "LinkAgronicaAgenda2010", "", "", objParametri_Server)

                If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
                    LinkAgronicaAgenda2010 = DTConfigSiti.Rows(0).Item("Valore")
                End If

                Dim script As String = RedirectGestione.ApriPopUp_Sito_AgronicaAuditSicurezzaGlobalCoop_PassandoDirettamente_Parametri(
                                            Enum_SiteRedirector.Sito_GiasOnline, enum_AuditPuaTipo.Audit_Condizionalita,
                                            HttpContext.Current.Session("ASG_Utente_CodFiscale").ToString, objParametriAgenda.Piva, 0, LinkAgronicaAgenda2010)

                ParametroDue = script
                tipo_risposta = "1"

            Case "40" 'Manuale Utente GIAS

                objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                TargetUrl = "../Documentazione.aspx"

            Case "41" 'CdG - Gestione Completa

                TargetUrl = "../AnalisiCostiProduzione/GestioneCompletaCdG.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                    "&origine=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, HttpContext.Current.Session)

            Case "48" 'CdG - Inserimento costi

                TargetUrl = "../AnalisiCostiProduzione/GestioneCosti.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                    "&origine=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                    "&entrata_diretta=" & Stringa_Codifica(1, AgroKey_EncoderDecoder, Nothing) &
                    "&costi_ricavi=" & Stringa_Codifica("costi", AgroKey_EncoderDecoder, Nothing)

            Case "50" 'CdG - Inserimento ricavi

                TargetUrl = "../AnalisiCostiProduzione/GestioneCosti.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                    "&origine=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                    "&entrata_diretta=" & Stringa_Codifica(1, AgroKey_EncoderDecoder, Nothing) &
                     "&costi_ricavi=" & Stringa_Codifica("ricavi", AgroKey_EncoderDecoder, Nothing)


            Case "51" 'CdG - ScaricoTempi

                TargetUrl = "../AnalisiCostiProduzione/ScaricoTempi.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                    "&origine=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, HttpContext.Current.Session)

            Case "52" 'Menu importazione/esportazione

                Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoSincronizzatore_PassandoDirettamenteIParametri(
                            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                            enum_PagineAgronicaSincro.MenuPrincipale,
                            enum_PagineAgenda_2010.Menu,
                            objParametriAgenda.Piva,
                            AgronicaCoreDataProvider.Conversioni.IdCodCliente_fromCodiceGIAS(CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))))

                ParametroDue = strJS
                tipo_risposta = "1"

            Case "49" 'CdG - Griglia e Pivot su DW / Report

                TargetUrl = "../AnalisiCostiProduzione/AnalisiProgetti.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, HttpContext.Current.Session) &
                    "&origine=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, HttpContext.Current.Session)

            Case "42" 'Statistiche Utilizzo
                Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

                Dim UtentiAbilitatoStatisticheOLD = objPermessi.Controlla_Permessi_Utente(
                                            HttpContext.Current.Session("ASG_Utente_Username"),
                                             HttpContext.Current.Session("ASG_IdServizio"),
                                             enum_Security_Attivita.Statistiche_Sito,
                                             enum_Security_Operazione.Modifica,
                                             Date.Now,
                                             "",
                                             objParametri_Utenti)

                Dim UtentiAbilitatoStatisticheNEW = objPermessi.Controlla_Permessi_Utente(
                                            HttpContext.Current.Session("ASG_Utente_Username"),
                                             HttpContext.Current.Session("ASG_IdServizio"),
                                             enum_Security_Attivita.Statistometro_New,
                                             enum_Security_Operazione.Modifica,
                                             Date.Now,
                                             "",
                                             objParametri_Utenti)

                If UtentiAbilitatoStatisticheNEW Then
                    ParametroDue = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamenteIParametri(
                        Enum_SiteRedirector.Sito_AgronicaProfilazione,
                        enum_CodificaStampe.Statistometro,
                        CStr(HttpContext.Current.Session("ASG_Utente_Username")),
                        CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS")),
                        "", "", "", "", "", "", "", "")
                ElseIf UtentiAbilitatoStatisticheOLD Then
                    ParametroDue = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoProfilazione_PassandoDirettamenteIParametri(
                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                       enum_PagineProfilazione_2010.Pagina_Statistiche,
                                       enum_PagineAgenda_2010.Menu,
                                       objParametriAgenda.Piva)
                End If

                tipo_risposta = "1"

            Case "44" 'Gestione Pratiche e Servizi
                Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

                Dim UtentiAbilitatoWorkflowOLD = objPermessi.Controlla_Permessi_Utente(
                                            HttpContext.Current.Session("ASG_Utente_Username"),
                                             HttpContext.Current.Session("ASG_IdServizio"),
                                             enum_Security_Attivita.Gestione_Servizi,
                                             enum_Security_Operazione.Lettura,
                                             Date.Now,
                                             "",
                                             objParametri_Utenti)

                Dim UtentiAbilitatoWorkflowNEW = objPermessi.Controlla_Permessi_Utente(
                                            HttpContext.Current.Session("ASG_Utente_Username"),
                                             HttpContext.Current.Session("ASG_IdServizio"),
                                             enum_Security_Attivita.Gestione_Servizi_NEW,
                                             enum_Security_Operazione.Lettura,
                                             Date.Now,
                                             "",
                                             objParametri_Utenti)

                Dim strJS As String = ""

                If UtentiAbilitatoWorkflowNEW Then

                    strJS = RedirectGestione.PreparaScripPerPopupFull("../Servizi/Servizi_Lista_BS.aspx", "Workflow")

                ElseIf UtentiAbilitatoWorkflowOLD Then

                    strJS = RedirectGestione.ApriPopUp_SitoProfilazione_PassandoDirettamenteIParametri(
                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                   enum_PagineProfilazione_2010.Pagina_Servizi_Lista,
                   enum_PagineAgenda_2010.Menu,
                   objParametriAgenda.Piva)
                End If


                ParametroDue = strJS
                tipo_risposta = "1"

            Case "47" 'import CIO

                Dim strJS As String = RedirectGestione.ApriPopUp_SitoSincronizzatore_PassandoDirettamenteIParametri(
                            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                            enum_PagineAgronicaSincro.ImportazioneCIO,
                            enum_PagineAgenda_2010.Menu,
                            objParametriAgenda.Piva,
                            Conversioni.IdCodCliente_fromCodiceGIAS(CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))))

                ParametroDue = strJS
                tipo_risposta = "1"

            Case "7z", "7" 'menu stampe

                TargetUrl = "../Stampe/MenuStampe.aspx"

                Dim Script As String = "<script language='javascript'>" &
                                       "           var win = window.open('" & TargetUrl & "' ," & "'Menu Stampe');" &
                                       "           win.focus();" &
                                       " </script> "

                ParametroDue = Script
                tipo_risposta = "1"

            Case Else

                If tipo_operazione.Contains("Stampa-") Then
                    Dim s As RispostaStandard = gestisciStampa(CInt(tipo_operazione.Replace("Stampa-", "")), specie, tipo_operazione, listaImpianti)
                    Return s
                End If

        End Select

        r.RispostaOK = True
        r.RispostaStringa = TargetUrl
        r.ParametroDue_stringa = ParametroDue
        r.Tipo = tipo_risposta
        Return r

    End Function

    Private Shared Sub RedirezionePaginaSincroProfilazioneAzienda(ByRef objParametriAgenda As ParametriAgenda, ByRef ParametroDue As String, ByRef tipo_risposta As String, ByRef urlRedirect As String)
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

        Dim parametriSincro As New ParametriSincronizzatore_2010
        parametriSincro.Pagina_Richiesta = enum_PagineAgronicaSincro.ImportazionePC_Anteprima
        parametriSincro.Piva = objParametriAgenda.Piva
        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoSincronizzatore_PassandoDirettamenteIParametri(
                             Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                             parametriSincro.Pagina_Richiesta,
                             enum_PagineAgenda_2010.Menu_BS,
                             objParametriAgenda.Piva,
                             0)

        Dim xRedir As String() = strJS.Split("'")
        urlRedirect = xRedir(3)
        Dim redirMe As String = "<script> window.location = '" & urlRedirect & "' </script>"
        ParametroDue = redirMe
        tipo_risposta = "1"
    End Sub

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaDatiListaBloccoControlli() As String

        ' VAnni: 25/2/2020: Verifica Sessione..?
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim rval As String = ""
        'Dim cmb As New DropDownList

        Dim Filtro As String = ""
        Dim _Tipo_GruppoOperazioni As String = "'C'"

        Dim DTOperazioni As DataTable

        Dim Flag_FiltroOperazioniUtente As Boolean = False

        Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Dim filtroUtente As String = ""
        Dim dt_FiltroUtente As DataTable

        dt_FiltroUtente = objUtente.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI,
                                          1,
                                          enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                          "", "",
                                          HttpContext.Current.Session("ASG_objParametri_Utenti"))
        If dt_FiltroUtente.Rows.Count > 0 Then
            filtroUtente = " AND Operazioni.Lav_Cod in ("
            Dim j As Integer = 0
            For j = 0 To dt_FiltroUtente.Rows.Count - 1

                If j <> 0 Then
                    filtroUtente &= " ,"
                End If
                filtroUtente &= dt_FiltroUtente.Rows(j).Item("ID_0")
            Next
            filtroUtente &= " )  "
            Flag_FiltroOperazioniUtente = True
        End If

        Dim FiltroAggiuntivo As String = ""



        'FiltroAggiuntivo = STR_OP_NON_GESTITE + "AND ( GruppoOperazioni.Tipo IN ('C','Z','P','E')) "
        FiltroAggiuntivo = STR_OP_NON_GESTITE & " " & Filtro & " AND  GruppoOperazioni.Tipo IN (" & _Tipo_GruppoOperazioni & ")"

        FiltroAggiuntivo &= filtroUtente

        ' Dim Ordinamento As String = " GruppoOperazioni.Tipo, GruppoOperazioni.GRU_COD, Lav_Des "
        Dim Ordinamento As String = " Operazioni.Lav_Des "

        Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R

        DTOperazioni = objOperazioniLeggi.Leggi(0, 0, 0, "", 0, "", "", False, False, False, False,
                                                    enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                    FiltroAggiuntivo,
                                                    Ordinamento,
                                                    HttpContext.Current.Session("ASG_objParametri_Server"))





        Dim i As Integer
        rval = "<option value=''>" & AgronicaAgenda_2010.Seleziona.ToUpper() & "</option>"

        For i = 0 To DTOperazioni.Rows.Count - 1

            'cmb.Items.Add(New ListItem(DTOperazioni.Rows(i).Item("LAV_DES"), _
            '                                       DTOperazioni.Rows(i).Item("LAV_COD")))

            rval &= "<option value=""" & DTOperazioni.Rows(i).Item("LAV_COD") & """>" & DTOperazioni.Rows(i).Item("LAV_DES") & "</option>"

        Next

        Return rval

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function ValorizzaSelect_BloccoControlli() As String()

        ' VAnni: 25/2/2020: Verifica Sessione..?
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT As DataTable
        DT = objUtentiImpostazioni.Leggi_Utente_Poi_SuperUser(0, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "", "", HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim DR() As DataRow

        Dim lavstr As String()

        'operazioni preferite
        DR = DT.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_QDC_PREFERITE)
        If DR.Length > 0 Then
            If Not IsDBNull(DR(0).Item("Impostazione_Valore_1")) AndAlso DR(0).Item("Impostazione_Valore_1") <> "" Then
                Dim lavs As String = DR(0).Item("Impostazione_Valore_1")
                lavstr = lavs.Split("|")

            End If

        End If

        Return lavstr

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function ValorizzaCheckbox_BloccoControlli() As String()

        ' VAnni: 25/2/2020: Verifica Sessione..?
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT As DataTable
        DT = objUtentiImpostazioni.Leggi_Utente_Poi_SuperUser(0, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "", "", HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim DR() As DataRow

        Dim tipis As String()
        Dim lavstr(5) As String

        'TipoGruppiOperazioni
        DR = DT.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_TIPI_GRUPPI_OPERAZIONI_VISIBILI_MENU_AGENDA)
        If DR.Length > 0 Then
            For i = 0 To lavstr.Count - 1
                lavstr(i) = 0
            Next
            If Not IsDBNull(DR(0).Item("Impostazione_Valore_1")) AndAlso DR(0).Item("Impostazione_Valore_1") <> "" Then
                Dim tipi As String = DR(0).Item("Impostazione_Valore_1")
                tipis = tipi.Split("|")
                For i = 0 To tipis.Count - 1
                    Select Case (tipis(i))
                        Case "C"
                            'CheckBoxListGruppiOperazioni.Items(0).Selected = True
                            lavstr(0) = 1
                        Case "E"
                        Case "E6"
                            'CheckBoxListGruppiOperazioni.Items(1).Selected = True
                            lavstr(1) = 1
                        Case "E10"
                            'CheckBoxListGruppiOperazioni.Items(2).Selected = True
                            lavstr(2) = 1
                        Case "Z"
                            'CheckBoxListGruppiOperazioni.Items(3).Selected = True
                            lavstr(3) = 1
                        Case "P"
                            'CheckBoxListGruppiOperazioni.Items(4).Selected = True
                            lavstr(4) = 1
                        Case "V"
                    End Select

                Next
            End If
            'Else
            '    'se non sono ancora impostate metto visibili tutte agenda
            '    For i = 0 To CheckBoxListGruppiOperazioni.Items.Count - 1
            '        If CheckBoxListGruppiOperazioni.Items(i).Value = "C" Or _
            '            CheckBoxListGruppiOperazioni.Items(i).Value = "E6" Or _
            '            CheckBoxListGruppiOperazioni.Items(i).Value = "E10" Then
            '            CheckBoxListGruppiOperazioni.Items(i).Selected = True
            '        Else
            '            CheckBoxListGruppiOperazioni.Items(i).Selected = False
            '        End If
            '    Next

        End If

        Return lavstr

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function ValorizzaAltre_BloccoControlli() As String()


        ' VAnni: 25/2/2020: Verifica Sessione..?
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT As DataTable = objUtentiImpostazioni.Leggi_Utente_Poi_SuperUser(0, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "", "", HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim DR() As DataRow

        Dim lavstr(2) As String

        For i = 0 To lavstr.Count - 1
            lavstr(i) = 0
        Next

        'AltreImpostazioni
        DR = DT.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_AlberoAnagrafica_visualizzaRiferimentoAlfanumericoImpianto)
        If DR.Length > 0 Then
            lavstr(0) = 1
        End If
        DR = DT.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_AlberoAnagrafica_ordinaDataUltimoImpianto)
        If DR.Length > 0 Then
            lavstr(1) = 1
        End If

        Return lavstr

    End Function

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_PianoConcimazione_2017 Then

            If Not IsNothing(Request.QueryString("r")) Then

                Dim Qs_Ricetta_Cod As String = Stringa_Decodifica(Request.QueryString("r").ToString, AgroKey_EncoderDecoder, Server)
                Dim objConcimazione As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
                objConcimazione.Pagina_Richiesta = enum_PaginePianoConcimazione_2017.PUA_Dichiarazione_Effluenti
                objConcimazione.Piva = objParametriAgenda.Piva
                objConcimazione.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                objConcimazione.Pagina_SitoOrigine = enum_PagineAgenda_2010.Menu_BS
                objConcimazione.PianoConcimazione_Testata_Cod = Qs_Ricetta_Cod

                Dim TargetUrl As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriConcimazione_2017(
                        Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objConcimazione)

                Response.Redirect(TargetUrl)
            End If

        End If

        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 Then

            If Request.QueryString("PaginaOrigine") IsNot Nothing AndAlso IsNumeric(Request.QueryString("PaginaOrigine")) Then
                Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(True, Request.QueryString("PaginaOrigine"), objParametriAgenda))
            End If

            If Master.flag_MenuBS_2017 Then
                Response.Redirect("MenuBS_2017.aspx")
            End If

            If Debugger.IsAttached Then
                'si tratta di un logout.
                Session.Abandon()
                Session.Clear()
                Response.Redirect("../index.aspx")
            Else

                ' VAnni: 3/11/2017: se provengo da un token di autenticazione allora la pagina di logout potrebbe non essere la nostra.
                ' leggo pertanto la configurazione.

                Dim xRedir As String =
                    AgronicaCoreUtility.Http.CookieLeggi("LinkHomePageGlobale")

                Dim xRedirPivaSuperUser As String = "?" &
                    AgronicaCoreUtility.Http.CookieLeggi("LinkHomePageGlobalePivaSuperUser")

                If Not String.IsNullOrEmpty(xRedir) Then
                    Session.Abandon()
                    Session.Clear()
                    If (Not (xRedir.ToLower.Contains("agronica") OrElse xRedir.ToLower.Contains("gias"))) OrElse xRedir.ToLower.Contains("pivasuperuser") Then
                        xRedirPivaSuperUser = ""
                    End If

                    Response.Redirect(AgronicaCoreUtility.Http.UriUnescape(xRedir & xRedirPivaSuperUser))
                End If

                Session.Abandon()
                Session.Clear()
                Response.Redirect("../index.aspx?pivasuperuser=" & xRedirPivaSuperUser)
            End If


        End If

        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then
            Dim TargetRedirect As String = ""
            MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                                  Enum_SiteRedirector.GiasNG,
                                                                                  objParametriAgenda.PaginaSitoOrigine,
                                                                                  TargetRedirect,
                                                                                  objParametri_Server)
            Response.Redirect(TargetRedirect)
        End If

        Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine
        If paginaOnLineRitorno = 0 Then
            If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline_2010 Then
                paginaOnLineRitorno = enum_PagineGiasOnline_2010.Menu
            Else
                paginaOnLineRitorno = enum_PagineGiasOnline.MenuPrincipale
            End If
        End If

        Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(True, paginaOnLineRitorno, objParametriAgenda))

    End Sub

    Private Sub CambiaImpresa(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim TargetUrl As String
        Dim Origine As String
        Dim Destinazione As String

        'azzero il filtro scelto x le operazioni multi-aziendali
        Session("VariabiliFiltro") = Nothing

        'Costruisco il link
        Origine = Stringa_Codifica(
                        "../menu/MenuBS_Agenda_Nuovo.aspx",
                        AgroKey_EncoderDecoder, Server)

        Destinazione = Stringa_Codifica(
                        "../menu/MenuBS_Agenda_Nuovo.aspx",
                        AgroKey_EncoderDecoder, Server)


        TargetUrl = "../Filtrino/FiltrinoImprese.aspx" &
                    "?o=" & Origine &
                    "&d=" & Destinazione

        Response.Redirect(TargetUrl)

    End Sub

    Private Sub MenuBS_Agenda_Nuovo_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        ' VAnni: 2/10/2017: a seguito di AgroMasterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_Filtro"), ImageButton).Click, AddressOf Me.CambiaImpresa
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto

        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtnFiltro.Click, AddressOf Me.CambiaImpresa
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto

        hf_UtenteAbilitatoFlagMostraBtnSalvaCDG.Value = True

    End Sub

    Private Function DecidiSeImpostareRedirectAutomatico(user As String, objParametri_Utenti As AgronicaCoreParametri) As Boolean


        'se esite l'impostazione e vale 1 allora effettua redirect automatico. 

        Dim rval As Boolean = False

        Dim leggiImpostazioniUtente As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R
        rval = leggiImpostazioniUtente.UTENTE_Attiva_Configurazione_Pratica_FlagAttivo(user, objParametri_Utenti)
        Return rval

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function eliminaZoo(piva As String, id_agenda As Integer) As RispostaStandard


        Dim r As New RispostaStandard
        'Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        Lingua.Gias_InizializzaCultura_DaSession()

        'Dim objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        If piva = "" Then
            r.Errore = "Il parametro piva non è stato valorizzato."
            Return r
        End If

        If id_agenda <= 0 Then
            r.Errore = "Il parametro id_agenda non è stato valorizzato."
            Return r
        End If

        Try

            Dim obj_Zoo As New AgronicaCoreAnagrafeBIZ.Zoo
            r = obj_Zoo.Elimina_Operazione(piva, id_agenda, objParametri_Server)

        Catch ex As Exception

            r.Errore = "Errore durante la cancellazione della ricetta."

            'inserimento fallito
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiProdottiInterventiAPP(piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objRicetteR As New AgronicaCoreContabDAL.Ricette_R
        Dim dettagli = objRicetteR.LeggiRicetteDettagliAPP(piva, 0, 0, "", True, objParametri_Server)

        r.RispostaOK = True
        If dettagli.Count > 0 Then
            Dim descrizioni As String = ""
            Dim prodotti As New Dictionary(Of String, List(Of APP_Ricette_Dettagli))
            For Each dettaglio In dettagli
                Dim tipo = dettaglio.Elem_Cod
                Dim chiave = dettaglio.Elem_Cod & "|" & dettaglio.Descrizione
                Dim listaDettagli As List(Of APP_Ricette_Dettagli)
                If prodotti.ContainsKey(chiave) Then
                    listaDettagli = prodotti(chiave)
                Else
                    listaDettagli = New List(Of APP_Ricette_Dettagli)
                    descrizioni &= "<br>" & If(tipo = 191, "FIT", If(tipo = 3, "FER", tipo)) & ": " & dettaglio.Descrizione
                End If
                listaDettagli.Add(dettaglio)
                prodotti(chiave) = listaDettagli
            Next
            r.RispostaStringa = "Ci sono " & prodotti.Count & " nuovi prodotti da importare:" & descrizioni
        End If

        Return r

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function CodificaProdottiAPP(piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim Parametrisincronizzatore_2010 As New ParametriSincronizzatore_2010
        Parametrisincronizzatore_2010.Piva = piva
        Parametrisincronizzatore_2010.Pagina_Richiesta = enum_PagineAgronicaSincro.Codifica_ProdottiAPP
        Parametrisincronizzatore_2010.Id_Cod_Cliente = 0
        Parametrisincronizzatore_2010.ParametriQueryString = "&win=1"
        Parametrisincronizzatore_2010.Salva()

        r.RispostaOK = True
        r.RispostaStringa = RedirectGestione.IndirizzoCompleto_SitoSincronizzatore_PassandoDirettamente_ParametriSincro_2010(
                                        Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                        Parametrisincronizzatore_2010)
        Return r

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiImpianti(piva As String, Sa_Cod As Integer, Veg_Cod As String, Data_Inizio As Date, Data_Fine As Date) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim DT As DataTable
        If IsNumeric(Veg_Cod) Then
            DT = objReg_Impianti.Leggi_ImpiantiMenuAgenda(piva, Sa_Cod, 0, 0, Veg_Cod, -1, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)
        Else
            DT = objReg_Impianti.Leggi_ImpiantiMenuAgenda(piva, Sa_Cod, 0, 0, -1, Veg_Cod.Split("/")(1), AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)
        End If

        Dim returnArray As New JArray

        For Each row In DT.Rows
            Dim objRet As New JObject
            objRet("chiave") = CStr(row("piva") & "_" & row("sa_cod") & "_" & row("appezza") & "_" & row("id_reg"))

            Dim des As String = ""

            If (row("Campo_Des") <> "") Then
                des &= row("Campo_Des") & " - "
            End If

            If IsNumeric(Veg_Cod) AndAlso Veg_Cod = 0 Then
                des &= row("App_Nome") & " - " & row("Veg_Des") & " " & row("Cul_Des") & " - " & row("Sup_Imp")
            Else
                des &= row("App_Nome") & " - " & row("Cul_Des") & " - " & row("Sup_Imp")
            End If

            objRet("des") = des
            returnArray.Add(objRet)
        Next

        r.RispostaOK = True
        r.RispostaStringa = returnArray.ToString
        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function VaiAiCosti(linkPaginaOrigine As String, id_Agenda As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriAgenda As New ParametriAgenda

        'Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If


        Dim PaginaLink As String = "../AnalisiCostiProduzione/GestioneCosti.aspx"


        Try
            PaginaLink &= "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                          "&id_agenda=" & Stringa_Codifica(id_Agenda, AgroKey_EncoderDecoder) &
                          "&origine=" & Stringa_Codifica(linkPaginaOrigine, AgroKey_EncoderDecoder) &
                          "&entrata_diretta=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing)


            r.RispostaOK = True
            r.RispostaStringa = PaginaLink
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    Protected Sub btn_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim id As String = (TryCast(sender, Control)).ClientID
    End Sub

    Public Function getLink() As String
        Dim link As String = ""

        Try
            Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")
            Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

            If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                link = RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                               Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                               enum_PagineGiasOnline_2010.RegistazioneSmart,
                                               enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

            Else
                link = CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
            End If

        Catch ex As Exception
            link = CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
        End Try




        'ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel),
        '                               CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
        '                               String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID),
        '                               link.ToString,
        '                               True)

        'Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
        '                          CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))


        Return link

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function aggiungi_al_pua(ByVal data As String,
                                           ByVal id_agenda_checked As String, ByVal id_agenda As String,
                                           ByVal lav_cod_checked As String, ByVal lav_cod As String,
                                           ByVal piva As String, ByVal sa_cod As String,
                                           ByVal ricetta_cod As String) As RispostaStandard

        Dim r As New RispostaStandard


        'Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim id_agenda_array As String() = id_agenda_checked.Split(",")
        Dim lav_cod_array As String() = lav_cod_checked.Split(",")

        Dim id_agenda_copiabili_array As New List(Of String)
        Dim lav_cod_copiabili_array As New List(Of String)

        For i = 0 To id_agenda_array.Length - 1
            If id_agenda_array(i) = "-1" Then
                id_agenda_copiabili_array.Add(id_agenda_array(i))
                lav_cod_copiabili_array.Add(lav_cod_array(i))
                Continue For
            End If
            id_agenda_copiabili_array.Add(id_agenda_array(i))
            lav_cod_copiabili_array.Add(lav_cod_array(i))
        Next

        If id_agenda_copiabili_array.Count = 0 OrElse (id_agenda_copiabili_array.Count = 1 AndAlso id_agenda_copiabili_array(0) = "-1") Then
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.NonÈPossibileCopiareQuestOperazionePlurale
            Return r
        End If


        'salvataggio ricette
        Dim objRicettaOpW As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
        Dim aggiungi_operazioni As Boolean = objRicettaOpW.Aggiungi_RicettaOperazioni_Da_OperazioniAgenda(piva, sa_cod, ricetta_cod, enum_TipoRicetta.PianoDistribuzionePua, id_agenda_checked, objParametri_Server, objParametri_Utenti, "")

        Dim objConcimazione As New ParametriConcimazione_2017
        objConcimazione.Pagina_Richiesta = enum_PaginePianoConcimazione_2017.PUA_Dichiarazione_Effluenti
        objConcimazione.Piva = piva
        objConcimazione.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objConcimazione.Pagina_SitoOrigine = enum_PagineAgenda_2010.Menu_BS
        objConcimazione.PianoConcimazione_Testata_Cod = ricetta_cod

        Dim TargetUrl As String = RedirectGestione.IndirizzoCompleto_SitoPianoConcimazione_PassandoDirettamente_ParametriConcimazione_2017(
                        Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objConcimazione)

        r.RispostaOK = True
        r.RispostaStringa = TargetUrl


        Return r

    End Function

    Private Sub ModificaFinestreTemp(ByRef objParametri As AgronicaCoreParametri)
        'sistemata in data 21/03/2012: era andata persa una funzionalità dell'agenda vecchia
        '(usata da fruttagel e da cab bagnacavallo, ad esempio), ripristinata

        'nel caso in cuim la finestra temporale dell'utente si illimitata
        'allora vengono visualizzate le operazioni dell'anno
        'altrimenti vengono visualizzate le operazioni presenti nell'arco temporale
        'ad esempio si impostano l'annata agraria corrente e quindi quando visualizzano le
        'operazioni di una specie, vogliono vedere se sono state registrate tutte,
        'dalla semina in poi e di solito al semina è nell'anno precedente!!!!


        ' VAnni: 25/2/2020: Verifica Sessione..?
        Lingua.Gias_InizializzaCultura_DaSession()


        Dim Anno As String
        Anno = CDate(objParametriAgenda.Data).Year

        Dim Validita_Inizio, Validita_Fine As Date
        If objParametri_Server.FinestraTemporaleInizio = AGRODATAINIZIO Then
            Validita_Inizio = CDate("01/01/" & Anno)
        Else
            Validita_Inizio = objParametri_Server.FinestraTemporaleInizio
        End If

        If objParametri_Server.FinestraTemporaleFine = AGRODATAFINE Then
            Validita_Fine = CDate("31/12/" & Anno)
        Else
            Validita_Fine = objParametri_Server.FinestraTemporaleFine
        End If
        objParametri.ImpostaFinestre_con_SalvataggioTemporale(Validita_Inizio, Validita_Fine)
    End Sub

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function DdlCentri_Change(sa_cod As String)
        Dim r As New RispostaStandard

        Dim objParametriAgenda As New ParametriAgenda
        objParametriAgenda.Sa_Cod = sa_cod

        r.RispostaOK = True

        Return r
    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function DdlSpecie_Change(veg_cod As String)
        Dim r As New RispostaStandard

        Dim objParametriAgenda As New ParametriAgenda
        objParametriAgenda.Veg_Cod = veg_cod

        r.RispostaOK = True

        Return r
    End Function

#End Region

#Region "Permessi"
    Private Shared Function controlloPermessi(ID_Attivita As enum_Security_Attivita,
                                              ID_Operazione As enum_Security_Operazione) As Boolean

        Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R


        Select Case ID_Operazione
            Case enum_Security_Operazione.Cancellazione, enum_Security_Operazione.Scrittura, -1 'Duplicazione
                'Tratto scrittura e cancellazione con l'enum 2 = Modifica
                ID_Operazione = enum_Security_Operazione.Modifica
        End Select

        Dim permesso As Boolean = objPermessi.Controlla_Permessi_Utente(HttpContext.Current.Session("ASG_Utente_Username"),
                                                                        HttpContext.Current.Session("ASG_IdServizio"),
                                                                        ID_Attivita, ID_Operazione,
                                                                        Date.Now, "",
                                                                        objParametri_Utenti)

        Return permesso
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function check_permessoOperazione(ID_Attivita As enum_Security_Attivita,
                                                    ID_Operazione As enum_Security_Operazione
                                                       ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim permesso As Boolean = controlloPermessi(ID_Attivita, ID_Operazione)

        If Not permesso Then
            Dim errore As String = generaMessaggioMancanzaPermessi(ID_Attivita, ID_Operazione)
            r.RispostaOK = False
            r.Errore = errore
        Else

            r.RispostaOK = True

        End If

        Return r

    End Function

    Private Shared Function generaMessaggioMancanzaPermessi(ID_Attivita As enum_Security_Attivita,
                                                            ID_Operazione As enum_Security_Operazione) As String

        Dim messaggio = ""

        Select Case ID_Attivita
            Case enum_Security_Attivita.Agenda_AccessoMenu
                Select Case ID_Operazione
                    Case enum_Security_Operazione.Scrittura
                        messaggio = AgronicaAgenda_2010.MancanzaPermessiOperazioneSceltaSuGruppoDiOperazioni
                    Case enum_Security_Operazione.Modifica
                        messaggio = AgronicaAgenda_2010.MancanzaPermessiOperazioneSceltaSuGruppoDiOperazioni
                    Case enum_Security_Operazione.Cancellazione
                        messaggio = AgronicaAgenda_2010.MancanzaPermessiCancellazioneSuGruppoDiOperazioni
                    Case -1 'Duplicazione
                        messaggio = AgronicaAgenda_2010.MancanzaPermessiOperazioneSceltaSuGruppoDiOperazioni
                End Select

            Case enum_Security_Attivita.Gest_Ricette
                Select Case ID_Operazione
                    Case enum_Security_Operazione.Scrittura
                        messaggio = Gias.MancanzaPermessiCancellazioneRicette
                    Case enum_Security_Operazione.Modifica
                        messaggio = Gias.MancanzaPermessiModificaRicette
                    Case enum_Security_Operazione.Cancellazione
                        messaggio = Gias.MancanzaPermessiCancellazioneRicette
                    Case -1 'Duplicazione
                        messaggio = Gias.MancanzaPermessiCopiaRicette
                End Select


            Case enum_Security_Attivita.Brogliaccio
                Select Case ID_Operazione
                    Case enum_Security_Operazione.Scrittura
                        messaggio = Gias.MancanzaPermessiScritturaBrogliacci
                    Case enum_Security_Operazione.Modifica
                        messaggio = Gias.MancanzaPermessiModificaBrogliacci
                    Case enum_Security_Operazione.Cancellazione
                        messaggio = Gias.MancanzaPermessiCancellazioneBrogliacci
                    Case -1 'Duplicazione
                        messaggio = Gias.MancanzaPermessiCopiaBrogliacci
                End Select


            Case enum_Security_Attivita.Gest_Stalle
                Select Case ID_Operazione
                    Case enum_Security_Operazione.Scrittura
                        messaggio = Gias.MancanzaPermessiScritturaZoo
                    Case enum_Security_Operazione.Modifica
                        messaggio = Gias.MancanzaPermessiModificaZoo
                    Case enum_Security_Operazione.Cancellazione
                        messaggio = Gias.MancanzaPermessiCancellazioneZoo
                    Case -1 'Duplicazione
                        messaggio = Gias.MancanzaPermessiCopiaZoo
                End Select

            Case Else
                messaggio = AgronicaAgenda_2010.MancanzaPermessiOperazioneSceltaSuGruppoDiOperazioni
        End Select

        Return messaggio

    End Function
#End Region

#Region "Carica DDL"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaCentriAziendali(ByVal data_inizio As String, ByVal data_fine As String) As RispostaStandard

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Specie(ByVal data_inizio As String, ByVal data_fine As String) As String

        Dim cmb_finalita As New DropDownList
        Dim objParametriAgenda As New ParametriAgenda

        ' VAnni: 25/2/2020: Verifica Sessione..?
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim clc As New CaricaListControl
        clc.TutteSpecieColtivate_3_Data_Da_A(cmb_finalita,
                                                        True, AgronicaAgenda_2010.TutteLeSpecie, "-1",
                                                        objParametriAgenda.Piva,
                                                        objParametriAgenda.Sa_Cod,
                                                        data_inizio,
                                                        data_fine,
                                                        True,
                                                        "", "", HttpContext.Current.Session("ASG_objParametri_Server"), True)

        Dim rval As String = ""
        For Each itm As ListItem In cmb_finalita.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Varieta(ByVal specie As String) As String

        ' VAnni: 25/2/2020: Verifica Sessione..?
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim cmb_finalita As New DropDownList
        Dim objParametriAgenda As New ParametriAgenda

        objParametriAgenda.Veg_Cod = specie

        If CInt(specie.Split("/")(0)) > 0 Then

            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.CaricaCombo_TutteVarietaColtivate(cmb_finalita,
                                              True, Resources.AgronicaAgenda_2010.TutteLeVarietà, "-1",
                                              CStr(objParametriAgenda.Piva),
                                              CInt(objParametriAgenda.Sa_Cod),
                                              0, CInt(objParametriAgenda.Veg_Cod),
                                              "", "", HttpContext.Current.Session("ASG_objParametri_Server"))

            'If objParametriAgenda.Cul_Cod <> "0" Then

            '    'Imposto la selezione della varietà
            '    Me.Cmb_Varieta.SelectedIndex = _
            '         Me.Cmb_Varieta.Items.IndexOf(Me.Cmb_Varieta.Items.FindByValue( _
            '            objParametriAgenda.Cul_Cod))

            '    Cambio_Varieta()

            'End If

        End If

        Dim rval As String = ""
        For Each itm As ListItem In cmb_finalita.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next


        Return rval

    End Function

#End Region


#Region "Nuova Operazione"

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function NuovaOperazioneAgenda(ByVal lavcod As Integer, Veg_Cod As String) As String

        ' VAnni: 25/2/2020: Verifica Sessione..?
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametriAgenda As New ParametriAgenda

        'Dim permesso As Boolean = AgronicaCoreModello.Utility_Operazioni.PermessiOpContabiliEMagazzino(lavcod, "1", HttpContext.Current.Session("ASG_objParametri_Server"), HttpContext.Current.Session("ASG_objParametri_Utenti"), HttpContext.Current.Session)
        Dim permesso As Boolean = controlloPermessi(enum_Security_Attivita.Agenda_AccessoMenu, enum_TipoOperazioneDB.Modifica) 'Nella fuzione sopra covertiva 1 in 2
        If Not permesso Then
            ' Messaggi.AgroMsgBox("Non si hanno i permessi per questa operazione su questo gruppo di operazioni", Page, , Nothing)
            Return "error"
            Exit Function
        End If

        Dim TargetUrl As String = ""

        Select Case lavcod
        'AUDIT
            Case LAVCOD_PRATICA_ECOLOGICA, LAVCOD_FORMAZIONE

                Dim TipoAudit As enum_AuditTipi

                Select Case lavcod
                    Case LAVCOD_PRATICA_ECOLOGICA
                        TipoAudit = enum_AuditTipi.AuditTipi_PraticheEcologicheAPOT
                    Case LAVCOD_FORMAZIONE
                        TipoAudit = enum_AuditTipi.AuditTipi_Formazione
                End Select

                Dim LinkAgronicaAgenda2010 As String = ""

                If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
                    LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
                End If

                TargetUrl = RedirectGestione.IndirizzoCompleto_Sito_AgronicaAuditSicurezzaGlobalCoop_PassandoDirettamente_Parametri(
                                    Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                    TipoAudit,
                                    HttpContext.Current.Session("ASG_Utente_CodFiscale").ToString,
                                    objParametriAgenda.Piva,
                                    0,
                                    enum_TipoOperazioneDB.Scrittura,
                                    1,
                                    0,
                                    LinkAgronicaAgenda2010)

                Return TargetUrl

            Case LAVCOD_CARICO, LAVCOD_SCARICO
                objParametriAgenda.Sa_Cod = 0
        End Select

        Dim OpUtil As New AgronicaCoreModello.Utility_Operazioni ' Utility_NS.Utility_Operazioni
        'attenzione, occorre impostare tipo operazione altrimenti form prodotto e op contabile che usano querystrig potrebbero dare errore
        'infatti la prima volta che entro nel menu non è inizializzato ed è stringa vuota
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        objParametriAgenda.Lav_Cod = lavcod
        objParametriAgenda.Id_Agenda = 0
        'objParametriAgenda.Sa_Cod = 0
        objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna
        objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
        objParametriAgenda.Programmazione_Cod = 0
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010


        Dim strErrore As String = ""
        TargetUrl = OpUtil.LinkPagina_from_LavCod_NEW(lavcod, objParametriAgenda, ServizioCod:=enum_Servizi.Quaderno_Campagna_Caa, strErrore:=strErrore,
                                                      fromBootstrapToBootstrap:=True, redirectPortateDomandaIrrigua:=True)
        'Se dalla creazione link errore ritorna un errore, esco
        If strErrore <> "" Then
            Return "error"
            Exit Function
        End If

        'gestione provenienza da operazioni zootecniche o operazioni colturali
        Dim referer As String = HttpContext.Current.Request.Headers.Get("Referer")
        If Not IsNothing(referer) AndAlso referer.Contains("DefaultTab=5") AndAlso TargetUrl.Contains("/Zoo/") Then
            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS_TrackMode
        End If

        'impedisco di fare raccolta senza selezionare prima il centro
        'If objParametriAgenda.Lav_Cod = LAVCOD_RACCOLTA AndAlso objParametriAgenda.Sa_Cod = "0" Then  'AndAlso
        '    'TargetUrl <> "../Operazioni/Raccolta.aspx" Or  Then
        '    'Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.PerRaccoltaSelezionareCentro, Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
        '    objParametriAgenda.Lav_Cod = 0
        '    Exit Function
        'End If

        '26/02/2019: nuovo sviluppo, documento di carico in versione light

        Select Case objParametriAgenda.Lav_Cod
            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA

                Dim DocumentoRicevutoLight As Boolean = SeDocumentoRicevutoLight(OpUtil)

                If DocumentoRicevutoLight Then

                    Dim mode As String
                    If objParametriAgenda.Lav_Cod = LAVCOD_BOLLA_RICEVUTA Then
                        mode = "bolla"
                    Else
                        mode = "fattura"
                    End If

                    TargetUrl = ""

                    'ottengo url della form prodotto
                    TargetUrl = OpUtil.LinkPagina_from_LavCod_NEW(LAVCOD_CARICO, objParametriAgenda, DocumentoRicevutoLight:=DocumentoRicevutoLight,
                                                                  fromBootstrapToBootstrap:=True)

                    If objParametriAgenda.SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 Then

                        TargetUrl &= "&light=" & Stringa_Codifica("true", AgroKey_EncoderDecoder) &
                                     "&lcl=" & Stringa_Codifica(objParametriAgenda.Lav_Cod, AgroKey_EncoderDecoder) &
                                     "&mol=" & Stringa_Codifica(mode, AgroKey_EncoderDecoder)

                    End If

                End If

        End Select

        '26/02/2019: commentato, doppione
        'objParametriAgenda.Lav_Cod = lavcod
        'objParametriAgenda.Id_Agenda = 0
        'objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        'objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna
        'objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
        'objParametriAgenda.Programmazione_Cod = 0

        objParametriAgenda.Data = Now.ToShortDateString

        If objParametriAgenda.Lav_Cod = LAVCOD_CURA Then

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                        HttpContext.Current.Session("ASG_Utente_Username"),
                                        HttpContext.Current.Session("ASG_IdServizio"),
                                        enum_Security_Attivita.Agenda_Operazione_Di_Cura,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        HttpContext.Current.Session("ASG_objParametri_Utenti"))

            If Not UtenteAbilitato Then
                TargetUrl = ""
                'Messaggi.AgroMsgBox("Non si hanno i permessi per l'operazione di cura", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
                objParametriAgenda.Lav_Cod = 0
                Exit Function
            End If

        End If

        'If TargetUrl <> "" Then
        '    Response.Redirect(TargetUrl)
        'Else
        '    'Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.OperazioneInManutenzione, Page, , Nothing)
        '    objParametriAgenda.Lav_Cod = 0
        '    Exit Function
        'End If

        'tutte le specie
        objParametriAgenda.Veg_Cod = CStr(Veg_Cod)

        If TargetUrl <> "" Then
            Return TargetUrl
        Else
            Return "error"
        End If

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function NuovaOperazioneRicettaAgenda(ByVal lavcod As Integer) As String

        ' VAnni: 25/2/2020: Verifica Sessione..?
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametriAgenda As New ParametriAgenda
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        'Dim permesso As Boolean = AgronicaCoreModello.Utility_Operazioni.PermessiOpContabiliEMagazzino(lavcod, "1", objParametri_Server, objParametri_Utenti, HttpContext.Current.Session)
        Dim permesso As Boolean = controlloPermessi(enum_Security_Attivita.Gest_Ricette, enum_TipoOperazioneDB.Modifica) 'Nella funzione sopra covertiva 1 in 2
        If Not permesso Then
            ' Messaggi.AgroMsgBox("Non si hanno i permessi per questa operazione su questo gruppo di operazioni", Page, , Nothing)
            Return "error"
        End If

        'attenzione, occorre impostare tipo operazione altrimenti form prodotto e op contabile che usano querystrig potrebbero dare errore
        'infatti la prima volta che entro nel menu non è inizializzato ed è stringa vuota
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        objParametriAgenda.Data = Now.ToShortDateString
        objParametriAgenda.Lav_Cod = lavcod
        objParametriAgenda.Id_Agenda = 0
        objParametriAgenda.Sa_Cod = 0
        objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta
        objParametriAgenda.TipoRicetta = enum_TipoRicetta.Standard_Destinazioni
        objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
        objParametriAgenda.Programmazione_Cod = 0
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline

        Dim strErrore As String = ""
        Dim OpUtil As New Utility_Operazioni
        Dim TargetUrl As String = OpUtil.LinkPagina_from_LavCod_NEW(lavcod, objParametriAgenda,
                                                                    PaginaSitoAgendaOrigine:=enum_PagineAgenda_2010.Menu_BS,
                                                                    LeggiFlagConfigurazioneSiti:=True,
                                                                    strErrore:=strErrore,
                                                                    fromBootstrapToBootstrap:=True)
        'Se dalla creazione link errore ritorna un errore, esco
        If strErrore <> "" Then
            Return "error"
        End If

        'tutte le specie
        objParametriAgenda.Veg_Cod = "-1"

        If TargetUrl = "" Then
            Return "error"
        End If

        Return TargetUrl

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function NuovaOperazioneBrogliaccioAgenda(ByVal lavcod As Integer) As String

        ' VAnni: 25/2/2020: Verifica Sessione..?
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametriAgenda As New ParametriAgenda
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")


        'Dim permesso As Boolean = AgronicaCoreModello.Utility_Operazioni.PermessiOpContabiliEMagazzino(lavcod, "1", objParametri_Server, objParametri_Utenti, HttpContext.Current.Session)
        Dim permesso As Boolean = controlloPermessi(enum_Security_Attivita.Brogliaccio, enum_TipoOperazioneDB.Modifica) 'Nella funzione sopra covertiva 1 in 2
        If Not permesso Then
            ' Messaggi.AgroMsgBox("Non si hanno i permessi per questa operazione su questo gruppo di operazioni", Page, , Nothing)
            Return "error"
        End If

        'attenzione, occorre impostare tipo operazione altrimenti form prodotto e op contabile che usano querystrig potrebbero dare errore
        'infatti la prima volta che entro nel menu non è inizializzato ed è stringa vuota
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        objParametriAgenda.Data = Now.ToShortDateString
        objParametriAgenda.Lav_Cod = lavcod
        objParametriAgenda.Id_Agenda = 0
        objParametriAgenda.Sa_Cod = 0
        objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio
        objParametriAgenda.TipoRicetta = enum_TipoRicetta.Standard_Destinazioni
        objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
        objParametriAgenda.Programmazione_Cod = 0
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline

        Dim strErrore As String = ""
        Dim OpUtil As New Utility_Operazioni
        Dim TargetUrl As String = OpUtil.LinkPagina_from_LavCod_NEW(lavcod, objParametriAgenda,
                                                                    PaginaSitoAgendaOrigine:=enum_PagineAgenda_2010.Menu_BS,
                                                                    LeggiFlagConfigurazioneSiti:=True,
                                                                    strErrore:=strErrore,
                                                                    fromBootstrapToBootstrap:=True)
        'Se dalla creazione link errore ritorna un errore, esco
        If strErrore <> "" Then
            Return "error"
        End If

        'tutte le specie
        objParametriAgenda.Veg_Cod = "-1"

        If TargetUrl = "" Then
            Return "error"
        End If

        Return TargetUrl

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function getURLGeneraModello4(ByVal chiave As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametriAgenda = New ParametriAgenda
            objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS

            Dim objParametriSincro As New ParametriSincronizzatore_2010()

            objParametriSincro.Piva = objParametriAgenda.Piva
            objParametriSincro.Pagina_Richiesta = enum_PagineAgronicaSincro.SincronizzatoreBDN
            Dim objOggetto As New JObject
            Dim objArray = JArray.Parse(chiave)
            objOggetto("OperazioniScarico") = objArray
            objParametriSincro.Xml_Generico = chiave

            objParametriSincro.ParametriQueryString = "tipoSincro=3" '& "&chiave_arr=" & Stringa_Codifica(chiave, AgroKey_EncoderDecoder)

            Dim targetUrl = RedirectGestione.IndirizzoCompleto_SitoSincronizzatore_PassandoDirettamente_ParametriSincro_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objParametriSincro)
            r.RispostaOK = True
            r.RispostaStringa = targetUrl

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function getURLRegistraModello4(ByVal chiave As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametriAgenda = New ParametriAgenda
            objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS

            Dim objParametriSincro As New ParametriSincronizzatore_2010()

            objParametriSincro.Piva = objParametriAgenda.Piva
            objParametriSincro.Pagina_Richiesta = enum_PagineAgronicaSincro.SincronizzatoreBDN
            Dim objOggetto As New JObject
            Dim objArray = JArray.Parse(chiave)
            objOggetto("OperazioniScarico") = objArray
            objParametriSincro.Xml_Generico = chiave

            objParametriSincro.ParametriQueryString = "tipoSincro=6" '& "&chiave_arr=" & Stringa_Codifica(chiave, AgroKey_EncoderDecoder)

            Dim targetUrl = RedirectGestione.IndirizzoCompleto_SitoSincronizzatore_PassandoDirettamente_ParametriSincro_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objParametriSincro)
            r.RispostaOK = True
            r.RispostaStringa = targetUrl

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function

#End Region


#Region "Alberi"

    <WebMethod(EnableSession:=True)>
    Public Shared Function GetNodesAgenda(ByVal id As String, ByVal PathRoot As String) As String

        Lingua.Gias_InizializzaCultura_DaSession()

        Return AgronicaControlli_2010.AlberoAgenda.GetNodesAgenda(id, PathRoot)
    End Function
    ''' <summary>
    ''' Per il caricamento dell'albero Anagrafico
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="PathRoot"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod(EnableSession:=True)>
    Public Shared Function GetNodesAlberoAnagrafe(ByVal id As String,
                                                  ByVal PathRoot As String) As String


        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objAnagraficaDettaglio As New AgronicaControlli_2010.AlberoAnagraficaDettaglio
        'leggo la piva, il sa_cod, la varietà e la specie
        Dim objParametriAgenda As New ParametriAgenda
        'objParametriAgenda.Leggi()
        objAnagraficaDettaglio.Piva = objParametriAgenda.Piva
        objAnagraficaDettaglio.Sa_Cod = objParametriAgenda.Sa_Cod

        objAnagraficaDettaglio.Flag_CatastoAziendale = True

        objAnagraficaDettaglio.Flag_Anagrafica = True
        objAnagraficaDettaglio.Flag_Contatti = True
        objAnagraficaDettaglio.Flag_Fabbricati = True
        objAnagraficaDettaglio.Flag_ParcoMacchine = True
        objAnagraficaDettaglio.Flag_Carica_Primo_Giro = True

        'lasciare false o con le cab si impianta!
        objAnagraficaDettaglio.Flag_Esplodi_Tutto = False

        If Not IsNothing(HttpContext.Current.Session("visualizzaRiferimentoAlfanumericoImpianto")) Then
            objAnagraficaDettaglio.visualizzaRiferimentoAlfanumericoImpianto = HttpContext.Current.Session("visualizzaRiferimentoAlfanumericoImpianto")
        End If

        If Not IsNothing(HttpContext.Current.Session("ordinaDataUltimoImpianto")) Then
            objAnagraficaDettaglio.visualizzaRiferimentoAlfanumericoImpianto = HttpContext.Current.Session("ordinaDataUltimoImpianto")
        End If

        If objParametriAgenda.Veg_Cod.Split("/")(0) <> -1 Then
            objAnagraficaDettaglio.Veg_Cod = objParametriAgenda.Veg_Cod.Split("/")(0)
        End If


        Return objAnagraficaDettaglio.GetNodesAlberoAnagrafe(id, PathRoot)
    End Function

#End Region


#Region "Caricamento Tabelle"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaTutto(ByVal data_inizio As String, ByVal data_fine As String, ByVal filtro As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriAgenda As New ParametriAgenda

        If Not IsDate(data_inizio) Then
            data_inizio = AGRODATAINIZIO
        End If

        If Not IsDate(data_fine) Then
            data_fine = AGRODATAFINE
        End If


        ''' @Paolo: inserire il filtro in sessione --> HttpContext.Current.Session("Filtro")

        HttpContext.Current.Session("Filtro") = Nothing

        Dim filtroImpianti As String = ""
        'Dim sa_cods As String = "-1"
        'Dim Appezzas As String = "-1"
        'Dim Id_Regs As String = "-1"
        If Split(filtro, "§")(0) > "2" Then
            'ho selezionato un nodo dal centro in su

            If Split(filtro, "§")(2) <> "0" Then
                If filtroImpianti <> "" Then
                    filtroImpianti &= " and Reg_Impianti.sa_cod in ( " & Split(filtro, "§")(2) & " ) "
                Else
                    filtroImpianti &= " Reg_Impianti.sa_cod in ( " & Split(filtro, "§")(2) & " ) "
                End If
            End If

            If Split(filtro, "§")(4) <> "0" Then
                If filtroImpianti <> "" Then
                    filtroImpianti &= " and Reg_Impianti.Appezza in ( " & Split(filtro, "§")(4) & " ) "
                Else
                    filtroImpianti &= " Reg_Impianti.Appezza in ( " & Split(filtro, "§")(4) & " ) "
                End If
            End If

            If Split(filtro, "§")(5) <> "0" Then
                If filtroImpianti <> "" Then
                    filtroImpianti &= " and Reg_Impianti.Id_Reg in ( " & Split(filtro, "§")(5) & " ) "
                Else
                    filtroImpianti &= " Reg_Impianti.Id_Reg in ( " & Split(filtro, "§")(5) & " ) "
                End If
            End If


        Else



        End If

        'filtro = " Lav_Cod in ( " & Lav_Cods & " ) "
        If filtroImpianti = "" Then
            HttpContext.Current.Session("Filtro") = Nothing
        Else
            HttpContext.Current.Session("Filtro") = "|" & filtroImpianti
        End If

        Dim dt As DataTable = MenuBS_Lavorazioni.Carica_Lavorazioni(
            Piva:=objParametriAgenda.Piva,
            Sa_Cod:=0,
            DataDa:=CDate(data_inizio),
            DataA:=CDate(data_fine),
            Veg_Cod:=0,
            Cul_Cod:=0,
            Tipo:=CStr(objParametriAgenda.TipoOperazioneColturale),
            Gru_Cod:=CInt(objParametriAgenda.GruppoOperazioneColturale),
            Lav_Cod:=0,
            Flag_TerrenoNudo:=False,
            xFiltroAggiuntivo_colturali:="",
            xFiltroAggiuntivo_postRaccolta:="",
            xFiltroAggiuntivo_contabili:="",
            xFiltroAggiuntivo_contabili_Macchine:="",
            xFiltroAggiuntivo_contabili_Audit:="",
            xOrderBy:="",
            objparametri_Server:=HttpContext.Current.Session("ASG_objParametri_Server"),
            objparametri_Utenti:=HttpContext.Current.Session("ASG_objParametri_Utenti"),
           FF_TrackedData_Cod:=-1, FromOutToIn:=True
        )


        If Not dt.Columns.Contains("Centro_Aziendale") Then
            dt.Columns.Add("Centro_Aziendale", GetType(String))
            dt.Columns.Add("Specie", GetType(String))
            dt.Columns.Add("Appezzamenti_Coinvolti", GetType(String))
            dt.Columns.Add("Prodotti_Utilizzati", GetType(String))
            dt.Columns.Add("Avversita", GetType(String))

            dt.Columns.Add("chiave_composita", GetType(String))

            For i = 0 To dt.Rows.Count - 1

                Dim dettagli = dt.Rows(i).Item("Dettagli")

                Dim delim As String() = New String(0) {"<br>"}
                Dim dett() As String = dettagli.Split(delim, StringSplitOptions.None)

                For count = 0 To dett.Length - 1

                    If InStr(dett(count), ":") Then

                        Dim dett2 As String() = dett(count).Split(":")

                        If dett(count) <> "" Then

                            Dim final_s As String = dett2(1).Replace("</b> ", "")

                            Select Case dett2(0)
                                Case "<b>Centro Az."
                                    dt.Rows(i).Item("Centro_Aziendale") = final_s
                                Case "<b>Specie"
                                    dt.Rows(i).Item("Specie") = final_s
                                Case "<b>Appezzamenti Coinvolti"
                                    dt.Rows(i).Item("Appezzamenti_Coinvolti") = final_s
                                Case "<b>Prodotti Utilizzati"
                                    dt.Rows(i).Item("Prodotti_Utilizzati") = final_s
                                Case " <b> Avversità"
                                    final_s = final_s.Replace("-", "")
                                    dt.Rows(i).Item("Avversita") = final_s
                            End Select

                        End If

                    End If
                Next


                dt.Rows(i).Item("chiave_composita") = dt.Rows(i).Item("Data2") & "_" &
                                                            dt.Rows(i).Item("Id_Agenda") & "_" &
                                                            dt.Rows(i).Item("Lav_Cod") & "_" &
                                                            dt.Rows(i).Item("Piva") & "_" &
                                                            dt.Rows(i).Item("Sa_Cod") & "_" &
                                                            dt.Rows(i).Item("Blocco_Flag") & "_" &
                                                            dt.Rows(i).Item("Veg_Cod")


            Next

        End If


        ''creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c = New ColonneNome("chiave_composita", "Operazioni", "string")

        Dim listaBtn = New List(Of btnAzioni)
        listaBtn.Add(New btnAzioni("fa-info info_elem", "infomodifica_operazione_singola(this, 0);", "Info"))
        listaBtn.Add(New btnAzioni("fa-pencil-square-o edit_elem", "infomodifica_operazione_singola(this, 2);", "Modifica"))
        listaBtn.Add(New btnAzioni("fa-trash-o del_elem", "elimina_operazione(this);", "Elimina"))
        listaBtn.Add(New btnAzioni("fa-files-o copy_elem", "copia_operazione_singola(this);", "Copia"))

        Dim tool As New ToolStandard(listaBtn)
        tool.cssColonna = "colOperazioni"
        c._FormatoParticolare = tool.toString()
        c._Filtrabile = False
        c._ColonnaDiSelezione = True
        l.Add(c)

        Dim cn As New ColonneNome("Data2", "Data", "date")
        cn._Filtrabile = True
        l.Add(cn)

        cn = New ColonneNome("Specie", "Specie", "string")
        cn._Filtrabile = True
        l.Add(cn)

        cn = New ColonneNome("cul_Des", "Varietà", "string")
        cn._Filtrabile = True
        l.Add(cn)

        cn = New ColonneNome("Lav_Des", "Operazione", "string")
        cn._Filtrabile = True
        cn._css = "desc_attivita"
        l.Add(cn)

        cn = New ColonneNome("Prodotti_Utilizzati", "Prodotti_Utilizzati", "string")
        cn._Filtrabile = True
        l.Add(cn)

        cn = New ColonneNome("Avversita", "Avversita", "string")
        cn._Filtrabile = True
        l.Add(cn)

        'cn = New ColonneNome("Dettagli", "Dettagli", "string")
        'cn._css = "dett"
        'cn._Filtrabile = True
        'cn._hidden = True
        'l.Add(cn)

        'cn = New ColonneNome("Rag_Soc", "Rag_Soc", "string")
        'cn._Filtrabile = False
        'cn._css = "Rag_Soc"
        'l.Add(cn)

        'cn = New ColonneNome("Gru_Des", "Gru_Des", "string")
        'cn._Filtrabile = False
        'cn._hidden = True
        'cn._css = "Gru_Des"
        'l.Add(cn)

        cn = New ColonneNome("Centro_Aziendale", "Centro_Aziendale", "string")
        cn._Filtrabile = True
        l.Add(cn)

        cn = New ColonneNome("Appezzamenti_Coinvolti", "Appezzamenti_Coinvolti", "string")
        cn._Filtrabile = True
        l.Add(cn)

        cn = New ColonneNome("Data", "Data_stringa", "string")
        cn._Filtrabile = True
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("Piva", "Piva", "string")
        cn._Filtrabile = False
        cn._hidden = True
        cn._css = "Piva"
        l.Add(cn)

        cn = New ColonneNome("Sa_Cod", "Sa_Cod", "string")
        cn._Filtrabile = False
        cn._hidden = True
        cn._css = "Sa_Cod"
        l.Add(cn)

        cn = New ColonneNome("Lav_Cod", "Lav_Cod", "string")
        cn._Filtrabile = False
        cn._hidden = True
        cn._css = "Lav_Cod"
        l.Add(cn)

        cn = New ColonneNome("Tipo", "Tipo", "string")
        cn._Filtrabile = True
        cn._hidden = True
        l.Add(cn)

        cn = New ColonneNome("Id_Agenda", "Id_Agenda", "string")
        cn._Filtrabile = False
        cn._hidden = True
        cn._css = "Id_Agenda"
        l.Add(cn)

        cn = New ColonneNome("Blocco_Flag", "Blocco_Flag", "string")
        cn._Filtrabile = False
        cn._hidden = True
        cn._css = "Blocco_Flag"
        l.Add(cn)

        cn = New ColonneNome("Veg_Cod", "Veg_Cod", "string")
        cn._Filtrabile = False
        cn._hidden = True
        cn._css = "Veg_Cod"
        l.Add(cn)

        cn = New ColonneNome("Ricetta_Cod", "Ricetta_Cod", "string")
        cn._Filtrabile = False
        cn._hidden = True
        cn._css = "Ricetta_Cod"
        l.Add(cn)

        Dim js As New JSON_DataTable
        r.RispostaOK = True
        r.RispostaStringa = js.JSON_DataTable_Senza_Colonne_Gia_Inserite(dt, l, l, True, True)

        Return r


    End Function

    Public Shared Function gestisciStampa(ByVal Report As enum_CodificaStampe, ByVal specie As String, ByVal tipo_operazione As String, ByVal listaImpianti As List(Of Reg_Impianti)) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriAgenda As New ParametriAgenda
        Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        '07/01/2019: delibera di dismettere le stampe semplificate facendole puntare sempre alle multicentro
        Report = AgronicaCoreStampeDAL.Stampe_QDC.ReplaceTipoReport(Report)

        r.RispostaOK = True
        r.Tipo = ""
        r.ParametroDue_stringa = ""
        r.RispostaStringa = ""

        Dim OrigineM As String = "../Menu/MenuBS_Agenda_Nuovo.aspx"

        Select Case Report

            'stampe gestite a parte, selezionando la specie e/o gli impianti e chiamando direttamente la pagina
            'senza passare dal filtrone
            Case enum_CodificaStampe.SchedaCampagna_Multicentro,
                 enum_CodificaStampe.SchedaCampagna_Multicentro_ACA,
                enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
                enum_CodificaStampe.Eurep_Gap_Multicentro,
                enum_CodificaStampe.Eurep_Gap_Semplificata,
                enum_CodificaStampe.Registro_Fertilizzazioni,
                enum_CodificaStampe.RegistroTrattamenti_Semplificata,
                enum_CodificaStampe.RegistroTrattamenti_Veneto,
                enum_CodificaStampe.SchedaCampagna_ProvAut_Trento,
                enum_CodificaStampe.SchedaCampagna_Multi_Lombardia,
                enum_CodificaStampe.SchedaInterventiAgronomici,
                 enum_CodificaStampe.SchedaRegistrazione_Semplificata,
                 enum_CodificaStampe.SchedaCampagna_ConserveItalia,
                 enum_CodificaStampe.RegistroAziendaleUnico,
                 enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita

                Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe
                objAgronicaStampe.report = Report
                Dim StrNodiVariabili As String = ""
                Dim xmlDoc As New XmlDocument
                Dim objVS As New AgronicaCoreXML.XML_Stampe

                '07/01/2019: delibera di dismettere le stampe semplificate facendole puntare sempre alle multicentro
                ''se non c'è il centro selezionato e ho selezionato la semplificata forzo la multicentro
                'If objAgronicaStampe.report = enum_CodificaStampe.SchedaCampagna_2078_Semplificata Then
                '    If objParametriAgenda.Sa_Cod = "0" Then
                '        objAgronicaStampe.report = enum_CodificaStampe.SchedaCampagna_Multicentro
                '    End If
                'End If

                'If objAgronicaStampe.report = enum_CodificaStampe.Eurep_Gap_Semplificata Then
                '    If objParametriAgenda.Sa_Cod = "0" Then
                '        objAgronicaStampe.report = enum_CodificaStampe.Eurep_Gap_Multicentro
                '    End If
                'End If

                Dim xmlFiltroStampa As XmlElement = xmlDoc.CreateElement("FiltroStampa")
                xmlDoc.AppendChild(xmlFiltroStampa)

                If listaImpianti.Count > 0 Then

                    Dim handleImp As New Reg_Impianti_Read()

                    For Each impianto As Reg_Impianti In listaImpianti

                        Dim specieDaImpianto As String = specie

                        If Not IsNumeric(specie) OrElse specie = "-1" Then
                            specieDaImpianto = handleImp.VegCod_from_PivaSaCodAppezzaIdimp(impianto.PIVA,
                                                                                           impianto.SA_COD,
                                                                                           impianto.APPEZZA,
                                                                                           impianto.ID_REG,
                                                                                           "",
                                                                                           "",
                                                                                           objParametri_Server)
                        End If

                        Dim vVarStampe(4) As ElementoStampe
                        vVarStampe(0).Nome = "piva"
                        vVarStampe(0).Valore = impianto.PIVA
                        vVarStampe(1).Nome = "sa_cod"
                        vVarStampe(1).Valore = impianto.SA_COD
                        vVarStampe(2).Nome = "appezza"
                        vVarStampe(2).Valore = impianto.APPEZZA
                        vVarStampe(3).Nome = "id_reg"
                        vVarStampe(3).Valore = impianto.ID_REG
                        vVarStampe(4).Nome = "veg_cod"
                        vVarStampe(4).Valore = specieDaImpianto
                        StrNodiVariabili &= objVS.XML_VariabiliStampe(vVarStampe)
                    Next

                    xmlFiltroStampa.InnerXml = StrNodiVariabili

                    Dim xmlTxt As XmlElement = xmlDoc.CreateElement("FiltraImpianti")
                    xmlTxt.SetAttribute("UtilizzaImpiantiFiltrati", 1)
                    xmlFiltroStampa.AppendChild(xmlTxt)

                Else

                    'Verifico le stampe che devono avere la specie selezionata
                    'spostata la scelta della specie nel filtro pre-stampa
                    Dim strVegCod As String = ""
                    Dim filtroImp As String = ""
                    'Select Case Report
                    '    Case enum_CodificaStampe.SchedaCampagna_Multicentro,
                    '        enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
                    '        enum_CodificaStampe.Eurep_Gap_Multicentro,
                    '        enum_CodificaStampe.Eurep_Gap_Semplificata

                    '        If Not IsNothing(HttpContext.Current.Session("Filtro")) Then
                    '            'se ho il filtro dell'impianto allora non occorre selezionare la scpecie perchè l'impianto è 1
                    '        Else
                    '            If specie = "-1" Then
                    '                'Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.PerStampaRicettaSelezSpecie, Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
                    '                'Exit Function
                    '                r.RispostaOK = False
                    '                r.Errore = "Per stampare la Scheda scelta occorre selezionare la Specie Vegetale"
                    '            End If
                    '        End If
                    '    Case Else
                    'End Select
                    If Not IsNothing(HttpContext.Current.Session("Filtro")) Then
                        'se ho il filtro dell'impianto allora non occorre selezionare la scpecie perchè l'impianto è 1
                        Dim sessionfiltro As String = HttpContext.Current.Session("Filtro")
                        If sessionfiltro.Split("|").Count > 1 AndAlso sessionfiltro.Split("|")(1).Trim <> "" Then
                            filtroImp = " (" & sessionfiltro.Split("|")(1) & " ) "
                        End If
                    Else
                        If specie <> "-1" Then
                            'tare etc
                            If InStr(specie, "/") <> 0 Then
                                strVegCod = Split(specie, "/")(0)
                            Else
                                'specie
                                strVegCod = specie
                            End If
                        End If
                    End If

                    Dim StrNodo As String

                    Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                    Dim HashImp As New Hashtable
                    Dim leggiAncheBloccati As Boolean = True
                    Dim Dt As DataTable = objImpianti.Leggi_Impianti_xAgenda2(False,
                                                    objParametriAgenda.Piva,
                                                    objParametriAgenda.Sa_Cod,
                                                    strVegCod,
                                                    objParametriAgenda.Cul_Cod,
                                                    "",
                                                    0,
                                                    -1,
                                                    filtroImp,
                                                    " Cul_Des, App_Nome, Progetto ",
                                                    objParametri_Server, leggiAncheBloccati)

                    Dim vegcodstrtemp As String = ""
                    If Dt.Rows.Count = 0 Then
                        'Messaggi.AgroMsgBox("Nessun Impianto Selezionato Per La Stampa", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
                        'Exit Function
                        r.RispostaOK = False
                        r.Errore = "Nessun Impianto Selezionato Per La Stampa"

                    Else

                        For i = 0 To Dt.Rows.Count - 1
                            'controllo che ci sia una sola specie
                            If i = 0 Then
                                vegcodstrtemp = Dt.Rows(i).Item("veg_cod")
                            End If
                            If vegcodstrtemp <> Dt.Rows(i).Item("veg_cod") AndAlso (tipo_operazione = "7a" OrElse tipo_operazione = "7b") Then
                                'Messaggi.AgroMsgBox("Non c'è un'unica specie negli impianti selezionati Per La Stampa", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
                                'Exit Function
                                r.RispostaOK = False
                                r.Errore = "Non c'è un'unica specie negli impianti selezionati Per La Stampa"

                            End If
                            vegcodstrtemp = Dt.Rows(i).Item("veg_cod")
                            If Not HashImp.ContainsKey(Dt.Rows(i).Item("sa_cod") & "|" & Dt.Rows(i).Item("appezza") & "|" & Dt.Rows(i).Item("id_reg")) Then
                                HashImp.Add(Dt.Rows(i).Item("sa_cod") & "|" & Dt.Rows(i).Item("appezza") & "|" & Dt.Rows(i).Item("id_reg"), "")
                                Dim vVarStampe(4) As ElementoStampe
                                vVarStampe(0).Nome = "piva"
                                vVarStampe(0).Valore = objParametriAgenda.Piva
                                vVarStampe(1).Nome = "sa_cod"
                                vVarStampe(1).Valore = Dt.Rows(i).Item("sa_cod")
                                vVarStampe(2).Nome = "appezza"
                                vVarStampe(2).Valore = Dt.Rows(i).Item("appezza")
                                vVarStampe(3).Nome = "id_reg"
                                vVarStampe(3).Valore = Dt.Rows(i).Item("id_reg")
                                vVarStampe(4).Nome = "veg_cod"
                                vVarStampe(4).Valore = Dt.Rows(i).Item("veg_cod")
                                StrNodo = objVS.XML_VariabiliStampe(vVarStampe)
                                StrNodiVariabili &= StrNodo
                            End If
                        Next

                        xmlFiltroStampa.InnerXml = StrNodiVariabili

                    End If


                End If

                objAgronicaStampe.username = CStr(HttpContext.Current.Session("ASG_Utente_Username"))
                objAgronicaStampe.user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
                objAgronicaStampe.Xml_Generico.Length = 0
                objAgronicaStampe.Xml_Generico.Append(xmlDoc.InnerXml)

                Dim strJS As String = RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                 Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgronicaStampe)

                r.Tipo = "1"
                r.ParametroDue_stringa = strJS

            Case enum_CodificaStampe.Bolle, enum_CodificaStampe.Fatture, enum_CodificaStampe.Nota_Accredito

                'Dim Id_Agenda As String = ""
                'Dim Data As String = ""
                'Dim Lav_Cod As String = ""
                'Dim Blocco_Flag As String = ""
                'Dim Piva As String = ""
                'Dim Sa_Cod As Integer
                'Dim Data2 As Date
                'Dim Veg_Cod As Integer = 0

                Dim Dt_Operazioni As DataTable

                If HttpContext.Current.Session("DataGrid_Lavorazioni") IsNot Nothing Then
                    Dt_Operazioni = New DataTable
                    Dt_Operazioni = CType(HttpContext.Current.Session("DataGrid_Lavorazioni"), DataTable)
                End If

                'If OperazioniSelezionate(Dt_Operazioni, Piva, Sa_Cod, Id_Agenda, Data, Data2, Lav_Cod, Blocco_Flag, Veg_Cod) <> 1 Then
                '    'alert 1 sola
                '    'Messaggi.AgroMsgBox("Selezionare un documento contabile alla volta!", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
                '    Exit Function
                'Else
                'se non ho selezionato una FATTURA,bolla,nota emessa
                'Select Case Lav_Cod
                '    Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA
                '    Case Else
                '        'Messaggi.AgroMsgBox("E' possibile stampare solo le Documenti Emessi!", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
                '        Exit Function
                'End Select
                ''Stampa_Documento(Server, objParametri_Server, Session, Page, Lav_Cod, Piva, Id_Agenda, "", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
                'End If

            Case Else

                Dim piva As String = objParametriAgenda.Piva
                Dim Origine As String = Stringa_Codifica("../Stampe/MenuStampe.aspx", AgroKey_EncoderDecoder, objParametri_Server)
                Dim Destinazione As String = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder, objParametri_Server)
                Dim Funzione As String = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder, objParametri_Server)

                Select Case Report

                    '---------------------- 
                    'Scheda Campagna
                    '---------------------- 

                    Case enum_CodificaStampe.SchedaCampagna_Biologico
                        'AgroMsgBox("Report in fase di costruzione!", Page)
                        'Exit Function
                        r.RispostaOK = False
                        r.Errore = "Report in fase di costruzione"

                    Case enum_CodificaStampe.SchedaCampagna_Biologico_Semplificata
                        ' AgroMsgBox("Report in fase di costruzione!", Page)
                        'Exit Function
                        r.RispostaOK = False
                        r.Errore = "Report in fase di costruzione"


                        '========================================================================

                    Case enum_CodificaStampe.SchedaMateriePrime_Biologico,
                        enum_CodificaStampe.SchedaVendite_Biologico,
                        enum_CodificaStampe.SchedaPreparati_Biologico


                        Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                          AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                      Report, piva, HttpContext.Current.Session, objParametri_Server)

                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                         Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                         ParametriAgronicaStampe)

                        r.Tipo = "1"
                        r.ParametroDue_stringa = strJS
                        Return r

                        '========================================================================

                    Case enum_CodificaStampe.PAP_Vegetale

                        If piva IsNot Nothing Then
                            piva = piva.ToString
                        End If

                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_Sito_AgronicaBio_PassandoDirettamente_Parametri(
                                                    Enum_SiteRedirector.Sito_GiasOnline, enum_CodificaPagBio.PAP_Vegetale,
                                                    HttpContext.Current.Session("ASG_Utente_CodFiscale").ToString, piva)

                        r.Tipo = "1"
                        r.ParametroDue_stringa = strJS
                        Return r

                        '========================================================================

                    Case enum_CodificaStampe.Notifica_Biologico

                        Dim strJS As String = RedirectGestione.ApriPopUp_Sito_AgronicaBio_PassandoDirettamente_Parametri(
                                                    Enum_SiteRedirector.Sito_GiasOnline, enum_CodificaPagBio.Notifica,
                                                    HttpContext.Current.Session("ASG_Utente_CodFiscale").ToString, piva)


                        r.Tipo = "1"
                        r.ParametroDue_stringa = strJS

                        '========================================================================

                    Case enum_CodificaStampe.SchedaMagazzinoGiacenze,
                            enum_CodificaStampe.SchedaMagazzinoMovimenti,
                                 enum_CodificaStampe.SchedaMagazzinoFertilizzanti,
                                    enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari,
                                        enum_CodificaStampe.RiepilogoProdottiUtilizzati

                        Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                            AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                        Report, piva, HttpContext.Current.Session, objParametri_Server, "", 0, 0, 0, 0, 0)

                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                         Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

                        r.Tipo = "1"
                        r.ParametroDue_stringa = strJS

                        Return r

                        '========================================================================


                    Case enum_CodificaStampe.RiepilogoImpiegoSuperfici

                        Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                        objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.Filtro_RiepilogoUtilizzoSuperfici
                        objGiasOnline.Piva = objParametriAgenda.Piva
                        objGiasOnline.funzioneoriginedestinazionefiltrone = Funzione & "|" & Origine & "|" & Destinazione & "|" & Report

                        If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
                            objGiasOnline.LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
                        Else
                            objGiasOnline.LinkAgronicaAgenda2010 = ""
                        End If

                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                                  Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objGiasOnline)

                        r.Tipo = "1"
                        r.ParametroDue_stringa = strJS


                        '========================================================================

                    Case enum_CodificaStampe.Esporta_GiasToSap

                        'Dim XmlDoc As New System.Xml.XmlDocument

                        'Dim LinkPaginaStampa As String = "../GestioneStampe/ChiamaStampe.aspx"
                        'Dim LinkSitoStampe As String = ConfigurationSettings.AppSettings("LinkAgronicaStampe")

                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
                                                                                 enum_CodificaStampe.Esporta_GiasToSap,
                                                                                 CStr(HttpContext.Current.Session("ASG_Utente_Username")),
                                                                                 CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS")),
                                                                                 "", "", "", "", "", "", "", "")

                        r.Tipo = "1"
                        r.ParametroDue_stringa = strJS

                        '========================================================================


                    Case enum_CodificaStampe.Costo_Manodopera_XLS


                        Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                        objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.Filtro_Manodopera
                        objGiasOnline.Piva = objParametriAgenda.Piva
                        objGiasOnline.funzioneoriginedestinazionefiltrone = Funzione & "|" & Origine & "|" & Destinazione & "|" & Report

                        If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
                            objGiasOnline.LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
                        Else
                            objGiasOnline.LinkAgronicaAgenda2010 = ""
                        End If

                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                                  Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objGiasOnline)

                        r.Tipo = "1"
                        r.ParametroDue_stringa = strJS

                        '========================================================================

                    Case enum_CodificaStampe.Costo_ParcoMacchine_XLS


                        Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                        objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.Filtro_ParcoMacchine
                        objGiasOnline.Piva = objParametriAgenda.Piva
                        objGiasOnline.funzioneoriginedestinazionefiltrone = Funzione & "|" & Origine & "|" & Destinazione & "|" & Report


                        If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
                            objGiasOnline.LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
                        Else
                            objGiasOnline.LinkAgronicaAgenda2010 = ""
                        End If

                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                                  Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objGiasOnline)


                        r.Tipo = "1"
                        r.ParametroDue_stringa = strJS

                        '========================================================================



                    Case enum_CodificaStampe.Esportazione_OP_Inv

                        Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_OP_Inv), AgroKey_EncoderDecoder, objParametri_Server)

                    Case enum_CodificaStampe.Esportazione_OP_Gest

                        Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_OP_Gest), AgroKey_EncoderDecoder, objParametri_Server)

                    Case enum_CodificaStampe.Esportazione_OP_Gest_Coop

                        Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_OP_Gest_Coop), AgroKey_EncoderDecoder, objParametri_Server)

                    Case enum_CodificaStampe.Report_RiconversioneVarietale

                        'Dim LinkSitoStampe, LinkpaginaStampa As String
                        'Dim XmlDoc As New System.Xml.XmlDocument


                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
                                                                                 enum_CodificaStampe.Report_RiconversioneVarietale,
                                                                                 CStr(HttpContext.Current.Session("ASG_Utente_Username")),
                                                                                 CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS")),
                                                                                 "", "", "", "", "", "", "", "")

                        r.Tipo = "1"
                        r.ParametroDue_stringa = strJS

                        '========================================================================


                    Case enum_CodificaStampe.Report_ImpegnoProduzioneSoci

                        'Dim LinkSitoStampe, LinkPaginaStampa As String
                        'Dim XmlDoc As New System.Xml.XmlDocument

                        'LinkSitoStampe = ConfigurationSettings.AppSettings("LinkAgronicaStampe")


                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
                                                                                 enum_CodificaStampe.Report_ImpegnoProduzioneSoci,
                                                                                 CStr(HttpContext.Current.Session("ASG_Utente_Username")),
                                                                                 CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS")),
                                                                                 "", "", "", "", "", "", "", "")

                        r.Tipo = "1"
                        r.ParametroDue_stringa = strJS

                        '========================================================================

                    Case enum_CodificaStampe.Esportazione_CellulariContatti

                        Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportazione_CellulariTecnici), AgroKey_EncoderDecoder, objParametri_Server)

                    Case enum_CodificaStampe.Esportatore_Universale_Imprese


                        Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Imprese), AgroKey_EncoderDecoder, objParametri_Server)

                    Case enum_CodificaStampe.Esportatore_Universale_Centri

                        Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Centri), AgroKey_EncoderDecoder, objParametri_Server)

                    Case enum_CodificaStampe.Esportatore_Universale_Appezza

                        'AgroMsgBox("Funzione non ancora attivata!", Page)
                        'Exit Function
                        r.RispostaOK = False
                        r.Errore = "Funzione non ancora attivata"
                        '========================================================================


                    Case enum_CodificaStampe.Esportatore_Universale_Impianti

                        Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Impianti), AgroKey_EncoderDecoder, objParametri_Server)

                        '========================================================================

                    Case enum_CodificaStampe.Esportatore_Universale_Agenda

                        Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Agenda), AgroKey_EncoderDecoder, objParametri_Server)

                        '========================================================================

                    Case enum_CodificaStampe.Esportatore_Universale_Rintraccio

                        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                        Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                                    HttpContext.Current.Session("ASG_Utente_Username"), HttpContext.Current.Session("ASG_IdServizio"),
                                                                    enum_Security_Attivita.Stampe_Esportazione_Rintraccio, enum_Security_Operazione.Modifica,
                                                                    Date.Now, "", objParametri_Utenti)

                        If UtenteAbilitato Then

                            Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Universale_Rintraccio), AgroKey_EncoderDecoder, objParametri_Server)

                        Else
                            'AgroMsgBox("Permesso negato!", Page)
                            'Exit Function
                            r.RispostaOK = False
                            r.Errore = "Permesso negato"

                        End If

                        '========================================================================

                        '--------------------------------
                        'Schede Varie x le OP
                        '--------------------------------
                    Case enum_CodificaStampe.Atto_Notorio

                        'Dim LinkSitoStampe, LinkPaginaStampa As String

                        'LinkSitoStampe = ConfigurationSettings.AppSettings("LinkAgronicaStampe")

                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
                                                                                 enum_CodificaStampe.Atto_Notorio,
                                                                                 CStr(HttpContext.Current.Session("ASG_Utente_Username")),
                                                                                 CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS")),
                                                                                 "", "", "", "", "", "", "", "")

                        r.Tipo = "1"
                        r.ParametroDue_stringa = strJS

                        '========================================================================

                    Case enum_CodificaStampe.Programmazione_Vegetale

                        Dim ParametriPlanning As New AgronicaCoreGestioneRichieste.ParametriPlanning
                        ParametriPlanning.PaginaProvenienza = enum_PagineGiasOnline.MenuStampe
                        ParametriPlanning.PaginaRichiesta = enum_CodificaPagPlanning.PianificazioneVegetale
                        ParametriPlanning.Cuaa = CUAA_from_PIVA(objParametri_Server, piva)
                        ParametriPlanning.Piva = piva

                        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_Sito_AgronicaPlanning_PassandoDirettamente_ParametriPlanning(
                                             Enum_SiteRedirector.Sito_GiasOnline, ParametriPlanning)

                        r.Tipo = "1"
                        r.ParametroDue_stringa = strJS

                        '========================================================================

                    Case enum_CodificaStampe.Esportazione_AnagraficaProdotti



                        piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                        If piva = "" Then
                            'mando al filtrino
                            Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
                                                                "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, Report)

                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

                            r.Tipo = "1"
                            r.ParametroDue_stringa = strJS

                        Else
                            Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                            Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
                            Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                                        AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                        Report, piva, HttpContext.Current.Session, objParametri_Server, rag_soc)

                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                             Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)


                            r.Tipo = "1"
                            r.ParametroDue_stringa = strJS


                        End If

                        'Exit Function


                        '=======================================================================

                    Case enum_CodificaStampe.Esportazione_AnagraficaContatti

                        Funzione = Stringa_Codifica(CStr(enum_TipoFiltrone.Esportatore_Contatti), AgroKey_EncoderDecoder, objParametri_Server)

                        '=======================================================================

                    Case enum_CodificaStampe.SchedaTracciabilita_Animale

                        'imposto la versione ZOO dell'alberoimprese
                        HttpContext.Current.Session("VersioneAlbero") = enum_VersioneAlberoImprese.Albero_Stalle

                        'come pagina di ritorno non metto il menùstampe, ma l'alberoimprese
                        'perché così se l'utente vuole cambiare centro, può farlo facendo exit
                        'dalla apgian delle consistenze

                        If piva <> "" Then
                            'c'è una sola azienda o l'utente vede una sola azienda

                            Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                            objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.StalleConsistenze_Info
                            objGiasOnline.Piva = objParametriAgenda.Piva

                            If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
                                objGiasOnline.LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
                            Else
                                objGiasOnline.LinkAgronicaAgenda2010 = ""
                            End If

                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                                      Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objGiasOnline)


                            r.Tipo = "1"
                            r.ParametroDue_stringa = strJS

                        Else

                            Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
                                            "../Stampe/MenuStampe.aspx", ".../GestioneStalle/StalleConsistenze_Info.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, Report)

                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

                            r.Tipo = "1"
                            r.ParametroDue_stringa = strJS

                        End If

                        '========================================================================

                    Case enum_CodificaStampe.PacchettoIgiene_RegistroFornitori

                        piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                        If piva = "" Then
                            'mando al filtrino
                            Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
                                                                "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, enum_CodificaStampe.PacchettoIgiene_RegistroFornitori)

                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

                            r.Tipo = "1"
                            r.ParametroDue_stringa = strJS
                        Else

                            Dim vVarStampe(0) As ElementoStampe
                            vVarStampe(0).Nome = "piva"
                            vVarStampe(0).Valore = piva
                            Dim StrNodo As String = XML_VariabiliStampe(vVarStampe)
                            Dim StrNodiVariabili As String = StrNodo

                            Dim username As String = HttpContext.Current.Session("ASG_Utente_Username")
                            Dim user_profilo As String = HttpContext.Current.Session("ASG_ProgressivoGIAS")

                            Dim user_profilo_codfiscale As String = HttpContext.Current.Session("ASG_Utente_CodFiscale")
                            Dim Sql_Filtro As String = ""
                            Dim XML_Filtro As String = ""
                            Dim username_codfisc As String = ""

                            Dim utente_codfiscale As String = HttpContext.Current.Session("ASG_Utente_CodFiscale")
                            Dim superuser_username As String = HttpContext.Current.Session("ASG_SuperUser_Username")
                            Dim superuser_codfiscale As String = HttpContext.Current.Session("ASG_SuperUser_CodFiscale")

                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamenteIParametri(Enum_SiteRedirector.Sito_GiasOnline,
                                                                            Report,
                                                                            username,
                                                                            user_profilo,
                                                                            StrNodiVariabili,
                                                                            user_profilo_codfiscale,
                                                                            Sql_Filtro,
                                                                            XML_Filtro,
                                                                            username_codfisc,
                                                                            utente_codfiscale,
                                                                            superuser_username,
                                                                            superuser_codfiscale)

                            r.Tipo = "1"
                            r.ParametroDue_stringa = strJS

                        End If


                        'Exit Function

                        '========================================================================

                    Case enum_CodificaStampe.PacchettoIgiene_RegistroClienti

                        piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                        If piva = "" Then
                            'mando al filtrino
                            Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
                                                                "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, Report)

                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

                            r.Tipo = "1"
                            r.ParametroDue_stringa = strJS

                        Else
                            Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                            Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
                            Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                                        AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                        Report, piva, HttpContext.Current.Session, objParametri_Server, rag_soc)

                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                             Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

                            r.Tipo = "1"
                            r.ParametroDue_stringa = strJS

                        End If

                        ' Exit Function

                        '========================================================================

                    Case enum_CodificaStampe.PacchettoIgiene_SchedaUsoAlimentiOGM

                        'AgroMsgBox("Stampa in fase di manutenzione.", Page)
                        ' Exit Function
                        r.RispostaOK = False
                        r.Errore = "Stampa in fase di manutenzione"

                        '========================================================================


                    Case enum_CodificaStampe.PacchettoIgiene_RegistroAlimentazioneStalla

                        piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                        If piva = "" Then
                            'mando al filtrino
                            Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
                                                                "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, Report)

                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

                            r.Tipo = "1"
                            r.ParametroDue_stringa = strJS

                        Else
                            Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                            Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
                            Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                                        AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                        Report, piva, HttpContext.Current.Session, objParametri_Server, rag_soc)

                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                             Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

                            r.Tipo = "1"
                            r.ParametroDue_stringa = strJS

                        End If

                        ' Exit Function


                        '========================================================================

                    Case enum_CodificaStampe.PacchettoIgiene_RegistroRazionamento

                        piva = AgronicaCoreGestioneRichieste.RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                        If piva = "" Then
                            'mando al filtrino
                            Dim Indirizzofiltrino As String = AgronicaCoreGestioneRichieste.RedirectGestione.GetLinkFiltrinoAgenda(
                                                                "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, Report)

                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

                            r.Tipo = "1"
                            r.ParametroDue_stringa = strJS

                        Else
                            Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                            Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
                            Dim ParametriAgronicaStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe =
                                      AgronicaCoreGestioneRichieste.RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                      Report, piva, HttpContext.Current.Session, objParametri_Server, rag_soc)

                            Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                             Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                             ParametriAgronicaStampe)

                            r.Tipo = "1"
                            r.ParametroDue_stringa = strJS

                        End If

                        'Exit Function


                        '========================================================================

                    Case enum_CodificaStampe.PacchettoIgiene_RegistroAnalisiNonConformi

                        'AgroMsgBox("Stampa in fase di manutenzione.", Page)
                        ' Exit Function
                        r.RispostaOK = False
                        r.Errore = "Stampa in fase di manutenzione"

                        '========================================================================


                    Case enum_CodificaStampe.Registro_Fertilizzazioni

                        '========================================================================


                    Case enum_CodificaStampe.ADD_Filtro_Report_Accettazione_DaDiversi

                        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                        Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                                 HttpContext.Current.Session("ASG_Utente_Username"), HttpContext.Current.Session("ASG_IdServizio"),
                                                                 enum_Security_Attivita.Report_Accettazione_DaDiversi, enum_Security_Operazione.Lettura,
                                                                 Date.Now, "", objParametri_Utenti)

                        If UtenteAbilitato Then

                            piva = RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                            If piva = "" Then
                                'mando al filtrino
                                Dim Indirizzofiltrino As String = RedirectGestione.GetLinkFiltrinoAgenda(
                                                                    "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, Report)


                                Dim strJS As String = RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

                                r.Tipo = "1"
                                r.ParametroDue_stringa = strJS

                            Else
                                Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                                Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
                                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                                            RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                            Report, piva, HttpContext.Current.Session, objParametri_Server, rag_soc)

                                Dim strJS As String = RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

                                r.Tipo = "1"
                                r.ParametroDue_stringa = strJS

                            End If



                        Else
                            'AgroMsgBox("Non si dispone dei permessi di stampa di questo report.", Page)
                            'Exit Function
                            r.RispostaOK = False
                            r.Errore = "Non si dispone dei permessi di stampa di questo report"

                        End If

                        'Exit Function

                        '========================================================================

                    Case enum_CodificaStampe.Registri_Preparazioni


                        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                        Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                                 HttpContext.Current.Session("ASG_Utente_Username"), HttpContext.Current.Session("ASG_IdServizio"),
                                                                 enum_Security_Attivita.Registri_Cantina, enum_Security_Operazione.Lettura,
                                                                 Date.Now, "", objParametri_Utenti)

                        If UtenteAbilitato Then
                            piva = RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                            If piva = "" Then
                                'mando al filtrino
                                Dim Indirizzofiltrino As String = RedirectGestione.GetLinkFiltrinoAgenda(
                                                                    "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx", Enum_SiteRedirector.Sito_AgronicaStampe, Report)
                                Dim strJS As String = RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

                                r.Tipo = "1"
                                r.ParametroDue_stringa = strJS

                            Else
                                Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                                Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
                                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                                            RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                            Report, piva, HttpContext.Current.Session, objParametri_Server, rag_soc)

                                Dim strJS As String = RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                                 Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

                                r.Tipo = "1"
                                r.ParametroDue_stringa = strJS

                            End If

                        End If

                        'Exit Function

                        '========================================================================

                    Case enum_CodificaStampe.Report_Incongruenze_CatastoVSAgrea

                        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                        Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                                 HttpContext.Current.Session("ASG_Utente_Username"), HttpContext.Current.Session("ASG_IdServizio"),
                                                                 enum_Security_Attivita.Report_Incongruenze_CatastoVSAgrea, enum_Security_Operazione.Lettura,
                                                                 Date.Now, "", objParametri_Utenti)

                        If UtenteAbilitato Then

                            piva = RedirectGestione.ControllaNumeroImprese_per_SiteRedirector(objParametri_Server, objParametri_Utenti)
                            If piva = "" Then
                                'mando al filtrino
                                Dim Indirizzofiltrino As String = RedirectGestione.GetLinkFiltrinoAgenda(
                                                                                            "../Stampe/MenuStampe.aspx", "../GestioneStampe/ChiamaStampe.aspx",
                                                                                            Enum_SiteRedirector.Sito_AgronicaStampe, Report)

                                Dim strJS As String = RedirectGestione.PreparaScripPerPopup(Indirizzofiltrino, "Stampa")

                                r.Tipo = "1"
                                r.ParametroDue_stringa = strJS

                            Else
                                Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                                Dim rag_soc As String = objanag.RagSoc_from_Piva(piva, objParametri_Server)
                                Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                                                    RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                                    Report, piva, HttpContext.Current.Session, objParametri_Server, rag_soc)

                                Dim strJS As String = RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                                          Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)

                                r.Tipo = "1"
                                r.ParametroDue_stringa = strJS

                            End If


                        Else
                            'AgroMsgBox("Non si dispone dei permessi di stampa di questo report.", Page)
                            'Exit Function
                            r.RispostaOK = False
                            r.Errore = "Non si dispone dei permessi di stampa di questo report"
                        End If

                        'Exit Function

                        '========================================================================


                    Case enum_CodificaStampe.Esportazione_OP_Catasto

                        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                        Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                                HttpContext.Current.Session("ASG_Utente_Username"), HttpContext.Current.Session("ASG_IdServizio"),
                                                                enum_Security_Attivita.Stampe_Esportazione_OP_Catasto, enum_Security_Operazione.Modifica,
                                                                Date.Now, "", objParametri_Utenti)

                        If Not UtenteAbilitato Then
                            ' Messaggi.AgroMsgBox("Permesso negato!", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
                            'Exit Function
                            r.RispostaOK = False
                        End If

                        Dim objp As New ParametriFILTRONE_2010
                        objp.Sito_Origine = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, objParametri_Server)
                        objp.Pagina_Origine = Stringa_Codifica(OrigineM, AgroKey_EncoderDecoder, objParametri_Server)
                        objp.Sito_Destinazione = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaStampe_2010, AgroKey_EncoderDecoder, objParametri_Server)
                        objp.Pagina_Destinazione = Stringa_Codifica("", AgroKey_EncoderDecoder, objParametri_Server)
                        objp.TipoFiltrone = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder, objParametri_Server)
                        objp.CodificaStampe = Stringa_Codifica(enum_CodificaStampe.Esportazione_OP_Catasto, AgroKey_EncoderDecoder, objParametri_Server)

                        Dim link As String = RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriFILTRONE_2010(
                                                               Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objp)

                        'Response.Redirect(link)
                        r.RispostaStringa = link


                    Case enum_CodificaStampe.Esportazione_OP_Produttori

                        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                        Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                             HttpContext.Current.Session("ASG_Utente_Username"), HttpContext.Current.Session("ASG_IdServizio"),
                                                             enum_Security_Attivita.Stampe_Esportazione_OP_Produttori, enum_Security_Operazione.Modifica,
                                                             Date.Now, "", objParametri_Utenti)

                        If Not UtenteAbilitato Then
                            ' Messaggi.AgroMsgBox("Permesso negato!", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
                            'Exit Function
                            r.RispostaOK = False
                            r.Errore = "Permesso negato"
                        End If

                        Dim objp As New ParametriFILTRONE_2010
                        objp.Sito_Origine = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, objParametri_Server)
                        objp.Pagina_Origine = Stringa_Codifica(OrigineM, AgroKey_EncoderDecoder, objParametri_Server)
                        objp.Sito_Destinazione = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaStampe_2010, AgroKey_EncoderDecoder, objParametri_Server)
                        objp.Pagina_Destinazione = Stringa_Codifica("", AgroKey_EncoderDecoder, objParametri_Server)
                        objp.TipoFiltrone = Stringa_Codifica(CStr(enum_TipoFiltrone.Stampa), AgroKey_EncoderDecoder, objParametri_Server)
                        objp.CodificaStampe = Stringa_Codifica(enum_CodificaStampe.Esportazione_OP_Produttori, AgroKey_EncoderDecoder, objParametri_Server)

                        Dim link As String = RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriFILTRONE_2010(
                                                               Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objp)

                        'Response.Redirect(link)
                        r.RispostaStringa = link

                    Case enum_CodificaStampe.Quadro_P

                        Dim ParametriAgronicaStampe As ParametriAgronicaStampe =
                          RedirectGestione.Genera_ParametriAgronicaStampe_Da_Session_SuStileXmlParametri_for_SiteRedirector(
                                      Report, piva, HttpContext.Current.Session, objParametri_Server)

                        Dim strJS As String = RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                                                         Enum_SiteRedirector.Sito_AgronicaAgenda_2010, ParametriAgronicaStampe)
                        r.Tipo = "1"
                        r.ParametroDue_stringa = strJS
                        Return r

                    Case enum_CodificaStampe.SchedaTracciabilita, enum_CodificaStampe.SchedaColturale_Biologico

                        Dim objp As New ParametriFILTRONE_2010

                        objp.Sito_Origine = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing)
                        objp.Pagina_Origine = Stringa_Codifica("../Menu/MenuBs_Agenda_Nuovo.aspx", AgroKey_EncoderDecoder, Nothing)
                        objp.Sito_Destinazione = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaStampe_2010, AgroKey_EncoderDecoder, Nothing)
                        objp.Pagina_Destinazione = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder, Nothing)
                        objp.TipoFiltrone = Stringa_Codifica(enum_TipoFiltrone.Stampa, AgroKey_EncoderDecoder, Nothing)
                        objp.CodificaStampe = Stringa_Codifica(Report, AgroKey_EncoderDecoder, Nothing)

                        Dim TargetRedirect = "../Filtrone/Filtrone_nuovo.aspx" &
                                             "?p_o=" & objp.Pagina_Origine &
                                             "&s_o=" & objp.Sito_Origine &
                                             "&p_d=" & objp.Pagina_Destinazione &
                                             "&s_d=" & objp.Sito_Destinazione &
                                             "&t_f=" & objp.TipoFiltrone &
                                             "&c_s=" & objp.CodificaStampe &
                                             "&v_c=" & objp.Veg_Cod &
                                             "&c_c=" & objp.Cul_Cod &
                                             "&d_i=" & objp.Data_Inizio &
                                             "&d_f=" & objp.Data_Fine

                        r.RispostaStringa = TargetRedirect
                        Return r

                    Case enum_CodificaStampe.Bilancio_Fertilizzazioni, enum_CodificaStampe.Bilancio_Fertilizzazioni_Dettagliato

                        Dim objp As New ParametriFILTRONE_2010

                        objp.Sito_Origine = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, AgroKey_EncoderDecoder, Nothing)
                        objp.Pagina_Origine = Stringa_Codifica("../Menu/MenuBs_Agenda_Nuovo.aspx", AgroKey_EncoderDecoder, Nothing)
                        objp.Sito_Destinazione = Stringa_Codifica(Enum_SiteRedirector.Sito_AgronicaStampe_2010, AgroKey_EncoderDecoder, Nothing)
                        objp.Pagina_Destinazione = Stringa_Codifica("../GestioneStampe/ChiamaStampe.aspx", AgroKey_EncoderDecoder, Nothing)
                        objp.TipoFiltrone = Stringa_Codifica(enum_TipoFiltrone.Stampa, AgroKey_EncoderDecoder, Nothing)
                        objp.CodificaStampe = Stringa_Codifica(Report, AgroKey_EncoderDecoder, Nothing)

                        Dim TargetRedirect = "../Filtrone/Filtrone.aspx" &
                           "?p_o=" & objp.Pagina_Origine &
                           "&s_o=" & objp.Sito_Origine &
                           "&p_d=" & objp.Pagina_Destinazione &
                           "&s_d=" & objp.Sito_Destinazione &
                           "&t_f=" & objp.TipoFiltrone &
                           "&c_s=" & objp.CodificaStampe &
                           "&v_c=" & objp.Veg_Cod &
                           "&c_c=" & objp.Cul_Cod &
                           "&d_i=" & objp.Data_Inizio &
                           "&d_f=" & objp.Data_Fine

                        r.RispostaStringa = TargetRedirect
                        Return r

                    Case Else

                        Dim debug As Boolean = True

                        ' ''Case enum_CodificaStampe.SchedaCampagna_2078
                        ' ''Case enum_CodificaStampe.RegistroTrattamenti
                        ' ''Case enum_CodificaStampe.SchedaRegistrazione
                        ' ''Case enum_CodificaStampe.RegistroTrattamenti_Veneto
                        ' ''Case enum_CodificaStampe.SchedaCampagna_2078_Semplificata
                        ' ''Case enum_CodificaStampe.SchedaCampagna_Multicentro
                        ' ''Case enum_CodificaStampe.RegistroTrattamenti_Semplificata
                        ' ''Case enum_CodificaStampe.SchedaRegistrazione_Semplificata
                        ' ''Case enum_CodificaStampe.Eurep_Gap
                        ' ''Case enum_CodificaStampe.Eurep_Gap_Semplificata
                        ' ''Case enum_CodificaStampe.Eurep_Gap_Multicentro


                        ' ''Case enum_CodificaStampe.Quadro_P
                        ' ''Case enum_CodificaStampe.PianoRaccolta
                        ' ''Case enum_CodificaStampe.SchedaColturale_Biologico
                        ' ''Case enum_CodificaStampe.RiepilogoImpiegoSuperfici_Multiazienda
                        ' ''Case enum_CodificaStampe.SchedaTracciabilita
                        ' ''Case enum_CodificaStampe.ReportConserveItalia
                        ' ''Case enum_CodificaStampe.SchedaCampagna_ConserveItalia
                        ' ''Case enum_CodificaStampe.DatiAnelloFilieraIngresso
                        ' ''Case enum_CodificaStampe.DatiAnelloFilieraLegameLotti
                        ' ''Case enum_CodificaStampe.EstrattoreDatiGrafici
                        ' ''Case enum_CodificaStampe.PianoColturale
                        ' ''Case enum_CodificaStampe.ReportRisultatoFilrone
                        ' ''Case enum_CodificaStampe.SchedaCampagna_Pizzoli



                End Select

                '--------------------------------------------------



                Dim objGiasOnline2 As New ParametriGiasOnline
                objGiasOnline2.PaginaRichiesta = enum_PagineGiasOnline.FiltroImpresa_new4_menustampeagenda
                objGiasOnline2.Piva = objParametriAgenda.Piva

                objGiasOnline2.funzioneoriginedestinazionefiltrone = Funzione & "|" & Origine & "|" & Destinazione & "|" & Report

                If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
                    objGiasOnline2.LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
                Else
                    objGiasOnline2.LinkAgronicaAgenda2010 = ""
                End If

                Dim strJS2 As String = RedirectGestione.ApriPopUp_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                          Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objGiasOnline2)

                r.Tipo = "1"
                r.ParametroDue_stringa = strJS2

                '----------------------------------------------------------------------------------


        End Select

        Return r

    End Function

#End Region


#Region "Info Modifica"

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function infomodifica_operazione_singola(ByVal type As Integer,
                                                           ByVal data As String,
                                                           ByVal id_agenda As String,
                                                           ByVal lav_cod As String,
                                                           ByVal blocco_flag As String,
                                                           ByVal veg_cod As Integer,
                                                           ID_Attivita As enum_Security_Attivita,
                                                           raccoglitore_cod As Integer) As RispostaStandard
        Dim piva As String = ""

        If Utils.LEGACY_SWITCH_USECOREWS Then
            Dim r As New RispostaStandard
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Dim risposta = Utility_Operazioni.InfoModificaOperazioneSingola(type, data, id_agenda, lav_cod, blocco_flag, veg_cod,
                                                                            objParametri_Server, objParametri_Utenti,
                                                                            False, piva)
            Return risposta

        Else

            Dim r As New RispostaStandard
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim permessi = New PermessiUtente()
            Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
            Dim objParametriAgenda As New ParametriAgenda

            Dim TargetUrl As String = ""

            '---------------------------------------------
            ' CONTROLLI

            'verifico permesso op contabili e magazzino
            Dim permesso As Boolean = Utility_Operazioni.PermessiOpContabiliEMagazzino(lav_cod, type, objParametri_Server, objParametri_Utenti, HttpContext.Current.Session)
            If Not permesso Then
                r.RispostaOK = False
                r.Errore = AgronicaAgenda_2010.MancanzaPermessiOperazioneSceltaSuGruppoDiOperazioni
                Return r
            End If

            'Verifico Permessi per operazioni colturali
            Dim olav As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim gru_op As Integer = olav.Gru_Op_from_LavorazioneCod(CInt(lav_cod), objParametri_Server)
            If Not {6, 10, 20}.Contains(gru_op) Then
                'Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                'permesso = objPermessi.Controlla_Permessi_Utente(
                '           HttpContext.Current.Session("ASG_Utente_Username"),
                '           HttpContext.Current.Session("ASG_IdServizio"),
                '           enum_Security_Attivita.Agenda_AccessoMenu,
                '           type,
                '           Date.Now,
                '           "",
                '           objParametri_Utenti)

                'Controllo permessi
                permesso = controlloPermessi(enum_Security_Attivita.Agenda_AccessoMenu, type)
            End If
            If Not permesso Then
                r.RispostaOK = False
                r.Errore = AgronicaAgenda_2010.MancanzaPermessiOperazioneSceltaSuGruppoDiOperazioni
                Return r
            End If


            'AUDIT
            Select Case lav_cod
                Case LAVCOD_PRATICA_ECOLOGICA,
                    LAVCOD_FORMAZIONE

                    Dim Audit_Cod As Integer = 0
                    Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R
                    Dim DtAgenda As DataTable = objAgenda.Leggi("", 0, id_agenda, lav_cod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                    If DtAgenda IsNot Nothing AndAlso DtAgenda.Rows.Count > 0 Then
                        Audit_Cod = DtAgenda.Rows(0).Item("audit_cod")
                    End If
                    Dim TipoAudit As enum_AuditTipi

                    Select Case lav_cod
                        Case LAVCOD_PRATICA_ECOLOGICA
                            TipoAudit = enum_AuditTipi.AuditTipi_PraticheEcologicheAPOT
                        Case LAVCOD_FORMAZIONE
                            TipoAudit = enum_AuditTipi.AuditTipi_Formazione
                    End Select

                    Dim LinkAgronicaAgenda2010 As String = ""

                    If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
                        LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
                    End If

                    TargetUrl = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_Sito_AgronicaAuditSicurezzaGlobalCoop_PassandoDirettamente_Parametri(
                                     Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                     TipoAudit,
                                     HttpContext.Current.Session("ASG_Utente_CodFiscale").ToString,
                                     objParametriAgenda.Piva,
                                     0,
                                     type,
                                     1,
                                     Audit_Cod,
                                     LinkAgronicaAgenda2010)

                    r.RispostaOK = True
                    r.RispostaStringa = TargetUrl

                    Return r

            End Select


            If veg_cod > 0 Then

                Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                Dim Dt As DataTable = objSpecVeg.SpecieVegetali_GestioneFiltroUtente_Leggi(veg_cod, 0, "", "", "", "", objParametri_Utenti)
                If Dt.Rows.Count = 0 Then
                    r.RispostaOK = False
                    r.Errore = AgronicaAgenda_2010.NoEliminazioneNoPermessoSpecie
                    Return r
                End If

            End If


            If lav_cod = LAVCOD_CURA Then
                Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            HttpContext.Current.Session("ASG_Utente_Username"),
                                            HttpContext.Current.Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Agenda_Operazione_Di_Cura,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
                If Not UtenteAbilitato Then
                    r.RispostaOK = False
                    r.Errore = AgronicaAgenda_2010.NoPermessiOperazioneCura
                    Return r
                End If
            End If


            'impedisco di modificare una raccolta senza selezionare prima il centro
            If lav_cod = LAVCOD_RACCOLTA AndAlso objParametriAgenda.Sa_Cod = "0" Then
                Dim cf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim dt As DataTable = cf.Leggi(0, "RaccoltaNew", " valore = 'true' ", "", HttpContext.Current.Session("ASG_objParametri_Server"))
                If dt.Rows.Count = 1 Then
                Else
                    r.RispostaOK = False
                    r.Errore = AgronicaAgenda_2010.BisognaSelezionareUnCentroPerLaRaccolta
                    Return r
                End If
            End If

            '---------------------------------------------

            objParametriAgenda.Data = data
            objParametriAgenda.Id_Agenda = id_agenda
            objParametriAgenda.Raccoglitore_Cod = raccoglitore_cod
            objParametriAgenda.Sa_Cod = 0
            objParametriAgenda.Lav_Cod = lav_cod
            objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna
            objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
            objParametriAgenda.Programmazione_Cod = 0
            objParametriAgenda.Tipo_Operazione = type
            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS
            objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010




            Dim OpUtil As New AgronicaCoreModello.Utility_Operazioni
            TargetUrl = OpUtil.LinkPagina_from_LavCod_NEW(lav_cod, objParametriAgenda, strErrore:=r.Errore,
                                                          fromBootstrapToBootstrap:=True, redirectPortateDomandaIrrigua:=True)

            'Se dalla creazione link errore ritorna un errore, esco
            If r.Errore <> "" Then
                TargetUrl = ""
                r.RispostaOK = False
                Return r
            End If

            'gestione provenienza da operazioni zootecniche o operazioni colturali
            Dim referer As String = HttpContext.Current.Request.Headers.Get("Referer")
            If Not IsNothing(referer) AndAlso referer.Contains("DefaultTab=5") AndAlso TargetUrl.Contains("/Zoo/") Then
                objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS_TrackMode
            End If

            'Controllo blocchi se non sono in modifica
            If type <> 0 Then
                Try
                    Dim matrice_delete(,) As String
                    If ControllaOperazione(matrice_delete, objParametriAgenda, objParametri_Server, "", False, objParametri_Utenti:=objParametri_Utenti) Then
                    End If
                Catch ex As Exception
                    r.RispostaOK = False
                    r.Errore = ex.Message
                    Return r
                End Try
            End If

            r.RispostaOK = True
            r.RispostaStringa = TargetUrl

            Return r
        End If
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function infomodifica_operazione_ricetta(type As Integer, data_operazione As String, ricetta_cod As Integer, ricetta_operazione_cod As Integer, lav_cod As String, veg_cod As String, tipo_ricetta As Integer) As RispostaStandard



        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        'Controllo permessi
        Dim permesso As Boolean = controlloPermessi(enum_Security_Attivita.Gest_Ricette, type)
        If Not permesso Then
            r.RispostaOK = False
            r.Errore = Gias.MancanzaPermessiModificaRicette
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametriAgenda As New ParametriAgenda
        objParametriAgenda.Data = data_operazione
        objParametriAgenda.Id_Agenda = 0
        objParametriAgenda.Sa_Cod = 0
        objParametriAgenda.Lav_Cod = lav_cod
        objParametriAgenda.Veg_Cod = veg_cod
        objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta
        objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
        objParametriAgenda.Programmazione_Cod = 0
        objParametriAgenda.Tipo_Operazione = type
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS
        objParametriAgenda.TipoRicetta = tipo_ricetta

        Dim Parametri_Aggiuntivi As New JObject

        Parametri_Aggiuntivi.Item("Ricetta_Cod") = ricetta_cod
        Parametri_Aggiuntivi.Item("Ricetta_Operazione_Cod") = ricetta_operazione_cod

        Dim strErrore As String = ""
        Dim OpUtil As New Utility_Operazioni
        Dim TargetUrl As String = OpUtil.LinkPagina_from_LavCod_NEW(objParametriAgenda.Lav_Cod, objParametriAgenda,
                                                                    PaginaSitoAgendaOrigine:=enum_PagineAgenda_2010.Menu,
                                                                    LeggiFlagConfigurazioneSiti:=True,
                                                                    Parametri_Aggiuntivi_for_Redirect:=Parametri_Aggiuntivi,
                                                                    strErrore:=strErrore,
                                                                    fromBootstrapToBootstrap:=True)
        'Se dalla creazione link errore ritorna un errore, esco
        If strErrore <> "" Then
            r.RispostaOK = False
            r.Errore = strErrore
            Return r
        End If

        r.RispostaOK = True

        r.RispostaStringa = TargetUrl

        If objParametriAgenda.SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 Then
            r.RispostaStringa = TargetUrl & "?operazione_ricetta=" & Sicurezza.Stringa_Codifica(ricetta_operazione_cod, CostantiPersonalizzate.AgroKey_EncoderDecoder) &
                                "&r=" & Sicurezza.Stringa_Codifica(ricetta_cod, CostantiPersonalizzate.AgroKey_EncoderDecoder)
        End If

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function infomodifica_operazione_brogliaccio(type As Integer, data_operazione As String, ricetta_cod As Integer, ricetta_operazione_cod As Integer, lav_cod As String, veg_cod As String, tipo_ricetta As Integer) As RispostaStandard



        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        'Controllo permessi
        Dim permesso As Boolean = controlloPermessi(enum_Security_Attivita.Brogliaccio, type)
        If Not permesso Then
            r.RispostaOK = False
            r.Errore = Gias.MancanzaPermessiModificaBrogliacci
            Return r
        End If

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametriAgenda As New ParametriAgenda
        objParametriAgenda.Data = data_operazione
        objParametriAgenda.Id_Agenda = 0
        objParametriAgenda.Sa_Cod = 0
        objParametriAgenda.Lav_Cod = lav_cod
        objParametriAgenda.Veg_Cod = veg_cod
        objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio
        objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
        objParametriAgenda.Programmazione_Cod = 0
        objParametriAgenda.Tipo_Operazione = type
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS
        objParametriAgenda.TipoRicetta = tipo_ricetta

        Dim Parametri_Aggiuntivi As New JObject

        Parametri_Aggiuntivi.Item("Ricetta_Cod") = ricetta_cod
        Parametri_Aggiuntivi.Item("Ricetta_Operazione_Cod") = ricetta_operazione_cod

        Dim strErrore As String = ""
        Dim OpUtil As New AgronicaCoreModello.Utility_Operazioni
        Dim TargetUrl As String = OpUtil.LinkPagina_from_LavCod_NEW(objParametriAgenda.Lav_Cod, objParametriAgenda,
                                                                    PaginaSitoAgendaOrigine:=enum_PagineAgenda_2010.Menu,
                                                                    LeggiFlagConfigurazioneSiti:=True,
                                                                    Parametri_Aggiuntivi_for_Redirect:=Parametri_Aggiuntivi,
                                                                    strErrore:=strErrore,
                                                                    fromBootstrapToBootstrap:=True)
        If strErrore <> "" Then
            r.RispostaOK = False
            r.Errore = strErrore
            Return r
        End If

        r.RispostaOK = True

        If objParametriAgenda.SitoDestinazione = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 Then
            r.RispostaStringa = TargetUrl & "?operazione_ricetta=" & Sicurezza.Stringa_Codifica(ricetta_operazione_cod, CostantiPersonalizzate.AgroKey_EncoderDecoder) &
                                "&r=" & Sicurezza.Stringa_Codifica(ricetta_cod, CostantiPersonalizzate.AgroKey_EncoderDecoder)
        Else
            r.RispostaStringa = TargetUrl
        End If

        Return r

    End Function

#End Region


#Region "QdC"

    '#########################################################################################
    '<Script.Services.ScriptMethod()>
    '<WebMethod(EnableSession:=True)>
    'Public Shared Function elimina_operazione_singola(ByVal data As String, ByVal id_agenda As String, ByVal lav_cod As String, ByVal veg_cod As String) As RispostaStandard

    '    Dim r As New RispostaStandard
    '    Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
    '    Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

    '    Dim err As String = elimina_operazione_singola_VerificaPermessi(data, id_agenda, lav_cod, veg_cod, objParametri_Server, objParametri_Utenti)

    '    If err = "" Then
    '        r.RispostaOK = True
    '        r.RispostaStringa = Resources.AgronicaAgenda_2010.OperazioneCancellata
    '    Else
    '        r.RispostaOK = False
    '        r.Errore = Resources.AgronicaAgenda_2010.OperazioneNonCancellataWarning + vbCrLf + err
    '    End If

    '    Return r

    'End Function

    '#########################################################################################

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function elimina_operazione_singola_VerificaPermessi(ByVal piva As String,
                                                                       ByVal data As String,
                                                                       ByVal id_agenda As String,
                                                                       ByVal lav_cod As String,
                                                                       ByVal veg_cod As String,
                                                                       ByVal blocco_flag As String,
                                                                       ByVal proseguiInCasoDiAlert As Boolean,
                                                                       ByRef objParametri_Server As AgronicaCoreParametri,
                                                                       ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                       ByRef OUT_ErroreBloccante As Boolean) As String

        If Utils.LEGACY_SWITCH_USECOREWS Then
            ' VAnni: 25/2/2020: Verifica Sessione..?
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim risposta = Utility_Operazioni.elimina_operazione_singola_VerificaPermessi(piva, data, id_agenda, lav_cod, veg_cod,
                                                                                          blocco_flag, proseguiInCasoDiAlert,
                                                                                          objParametri_Server, objParametri_Utenti,
                                                                                          OUT_ErroreBloccante) & vbCrLf & vbCrLf
            Return risposta

        Else

            Try

                ' VAnni: 25/2/2020: Verifica Sessione..?
                Lingua.Gias_InizializzaCultura_DaSession()

                OUT_ErroreBloccante = True

                'Il controllo viene fatto lato client, ma non si sa mai...
                If blocco_flag = 1 Then
                    If Not {LAVCOD_VENDITA_ANIMALI, LAVCOD_MACELLAZIONE_ANIMALI, LAVCOD_TRASFERIMENTO_ANIMALI}.Contains(lav_cod) Then
                        Return AgronicaAgenda_2010.ImpossibileModificareOperazioneBlocc
                    End If
                End If

                'verifico permesso op contabili e magazzino
                'permessi op contabili e magazzino
                Dim permesso As Boolean = Utility_Operazioni.PermessiOpContabiliEMagazzino(lav_cod, enum_TipoOperazioneDB.Cancellazione, objParametri_Server, objParametri_Utenti, HttpContext.Current.Session)
                If Not permesso Then
                    Return AgronicaAgenda_2010.MancanzaPermessiCancellazioneSuGruppoDiOperazioni
                End If

                'Verifico Permessi per operazioni colturali
                Dim olav As New AgronicaCoreMetaSchemaDAL.Operazioni_R
                Dim gru_op As Integer = olav.Gru_Op_from_LavorazioneCod(CInt(lav_cod), objParametri_Server)
                If Not {6, 10, 20}.Contains(gru_op) Then
                    Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                    permesso = objPermessi.Controlla_Permessi_Utente(
                HttpContext.Current.Session("ASG_Utente_Username"),
                HttpContext.Current.Session("ASG_IdServizio"),
                enum_Security_Attivita.Agenda_AccessoMenu,
                enum_Security_Operazione.Modifica,
                Date.Now, "", objParametri_Utenti)
                End If
                If Not permesso Then
                    Return AgronicaAgenda_2010.MancanzaPermessiCancellazioneSuGruppoDiOperazioni
                End If

                'Creo l'oggetto ParametriAgenda
                Dim objParametriAgenda As New ParametriAgenda
                If Not String.IsNullOrEmpty(piva) Then
                    objParametriAgenda.Piva = piva
                End If
                objParametriAgenda.Data = data
                objParametriAgenda.Id_Agenda = id_agenda
                objParametriAgenda.Lav_Cod = lav_cod
                objParametriAgenda.Tipo_Operazione = CStr(CInt(enum_TipoOperazioneDB.Cancellazione))

                'pratica ecologica (eliminazione id_agenda)
                Select Case lav_cod

                    Case LAVCOD_PRATICA_ECOLOGICA, LAVCOD_FORMAZIONE
                        Dim res As Boolean = OperazioneAgendaAudit_Cancella(objParametriAgenda, objParametri_Server)
                        If Not res Then
                            Return AgronicaAgenda_2010.ImpossibileEliminareOperazione
                        Else
                            Return ""
                        End If
                End Select

                If veg_cod <> "" AndAlso veg_cod > 0 Then

                    Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                    Dim Dt As DataTable = objSpecVeg.SpecieVegetali_GestioneFiltroUtente_Leggi(veg_cod, 0,
                                                                                               "", "",
                                                                                               "", "",
                                                                                               objParametri_Utenti)
                    If Dt.Rows.Count = 0 Then
                        Return Resources.AgronicaAgenda_2010.NoModificaNoPermessoSpecie
                    End If

                End If

                Dim matrice_delete(,) As String
                Dim messaggio1 As String = ""
                Dim messaggio2 As String = ""

                If ControllaOperazione(matrice_delete, objParametriAgenda, objParametri_Server, messaggio1, proseguiInCasoDiAlert, messaggio2, objParametri_Utenti:=objParametri_Utenti) Then
                    Dim res As Boolean = Operazione_Agenda_Utility.Cancella_Operazione_E_Collegate(objParametriAgenda,
                objParametri_Server,
                messaggio1,
                                                                                                   True,
                objParametri_Utenti:=objParametri_Utenti)
                    If Not res Then
                        Return Resources.AgronicaAgenda_2010.OperazioneNonCancellataWarning
                    End If
                Else
                    OUT_ErroreBloccante = False
                    Return AgronicaAgenda_2010.Avviso & ": " & messaggio1 & " " & messaggio2
                End If

                Return ""

            Catch ex As Exception
                Return AgronicaAgenda_2010.ImpossibileEliminareOperazione & " " & ex.Message
            End Try
        End If

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function copia_operazione_singola(ByVal data As String, ByVal id_agenda_checked As String, ByVal id_agenda As String, ByVal lav_cod_checked As String, ByVal lav_cod As String, ByVal piva As String, ByVal sa_cod As String) As RispostaStandard



        Dim r As New RispostaStandard

        'Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        'Dim LAV_COD_NON_COPIABILI As Integer() = {LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING, LAVCOD_RACCOLTA,
        '                                            LAVCOD_FORMAZIONE, LAVCOD_GESTIONE_RIFIUTI, LAVCOD_PRATICA_ECOLOGICA,
        '                                            LAVCOD_IRRIGAZIONE, LAVCOD_RILIEVO_PIOGGE,
        '                                            LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA, LAVCOD_REINNESCO_TRAPPOLE,
        '                                            LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE, LAVCOD_RILIEVO_ERBE_INFESTANTI, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_DANNI_RACCOLTA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA}

        Dim id_agenda_array As String() = id_agenda_checked.Split(",")
        Dim lav_cod_array As String() = lav_cod_checked.Split(",")

        Dim id_agenda_copiabili_array As New List(Of String)
        Dim lav_cod_copiabili_array As New List(Of String)

        'Creo un nuovo elenco di operazioni d'agenda copiabili
        For i = 0 To id_agenda_array.Length - 1

            'Se è il -1, lo includo
            If id_agenda_array(i) = "-1" Then
                id_agenda_copiabili_array.Add(id_agenda_array(i))
                lav_cod_copiabili_array.Add(lav_cod_array(i))
                Continue For
            End If

            'Controllo che il lav_cod sia tra quelli copiabili
            If Not LAV_COD_COPIABILI.Contains(lav_cod_array(i)) Then
                Continue For
            End If

            'Controllo se l'utente ha permessi di scrittura sull'operazione in oggetto

            'permessi op contabili e magazzino
            Dim permesso As Boolean = Utility_Operazioni.PermessiOpContabiliEMagazzino(lav_cod, enum_TipoOperazioneDB.Copia,
                                                                                       objParametri_Server, objParametri_Utenti,
                                                                                       HttpContext.Current.Session)
            If Not permesso Then
                Continue For
            End If

            'Verifico Permessi per operazioni colturali
            Dim op_R As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            Dim gru_op As Integer = op_R.Gru_Op_from_LavorazioneCod(lav_cod, objParametri_Server)

            If Not {6, 10, 20}.Contains(gru_op) Then
                Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                permesso = objPermessi.Controlla_Permessi_Utente(
                           HttpContext.Current.Session("ASG_Utente_Username"),
                           HttpContext.Current.Session("ASG_IdServizio"),
                           enum_Security_Attivita.Agenda_AccessoMenu,
                           enum_Security_Operazione.Modifica,
                           Date.Now, "", objParametri_Utenti)
            End If

            If Not permesso Then
                Continue For
            End If

            'Aggiungo l'elemento tra i copiabili
            id_agenda_copiabili_array.Add(id_agenda_array(i))
            lav_cod_copiabili_array.Add(lav_cod_array(i))
        Next

        'Se non ci sono operazioni d'agenda copiabili, ritorno un errore
        If id_agenda_copiabili_array.Count = 0 OrElse (id_agenda_copiabili_array.Count = 1 AndAlso id_agenda_copiabili_array(0) = "-1") Then
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.NonÈPossibileCopiareQuestOperazionePlurale
            Return r
        End If

        'Se c'è almeno un operazione d'agenda copiabile, allora vado in duplicazione con i soli id copiabili
        id_agenda_checked = String.Join(",", id_agenda_copiabili_array)

        Dim objImpre As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim objCentriAziendali As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim objParametriAgenda As New ParametriAgenda

        objParametriAgenda.Data = data
        objParametriAgenda.Id_Agenda = id_agenda_copiabili_array(0) 'id_agenda
        objParametriAgenda.Lav_Cod = lav_cod_copiabili_array(0) 'lav_cod
        objParametriAgenda.Piva = piva
        objParametriAgenda.Sa_Cod = sa_cod
        objParametriAgenda.RagSoc = objImpre.RagSoc_from_Piva(piva, objParametri_Server)
        objParametriAgenda.SaNome = objCentriAziendali.SaNome_from_SaCod(piva, sa_cod, objParametri_Server)
        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS

        r.RispostaOK = True
        r.RispostaStringa = "../Operazioni/DuplicaOperazione.aspx?id=" & id_agenda_checked



        'Select Case lav_cod
        '    Case LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_RACCOLTA, LAVCOD_SOVESCIO, LAVCOD_FORMAZIONE, LAVCOD_GESTIONE_RIFIUTI, LAVCOD_PRATICA_ECOLOGICA

        '        'Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.NonÈPossibileCopiareQuestoTipoDiOperazione, Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanel_Menu"), UpdatePanel))
        '        'Exit Function
        '        r.RispostaOK = False
        '        r.Errore = "Non è possibile copiare questo tipo di Operazione"

        '    Case Else

        '        'permessi op contabili e magazzino
        '        Dim permesso As Boolean = Utility_NS.Utility_Operazioni.PermessiOpContabiliEMagazzino(lav_cod, enum_TipoOperazioneDB.Copia, objParametri_Server, objParametri_Utenti, HttpContext.Current.Session)
        '        If permesso = False Then
        '            r.RispostaOK = False
        '            r.Errore = "Non si hanno i permessi per questa operazione su questo gruppo di operazioni!"
        '            Return r
        '        End If

        '        'Verifico Permessi per operazioni colturali
        '        Dim olav As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        '        Dim gru_op As Integer = olav.Gru_Op_from_LavorazioneCod(CInt(lav_cod), objParametri_Server)
        '        If Not {6, 10, 20}.Contains(gru_op) Then
        '            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        '            permesso = objPermessi.Controlla_Permessi_Utente( _
        '                       HttpContext.Current.Session("ASG_Utente_Username"), _
        '                       HttpContext.Current.Session("ASG_IdServizio"), _
        '                       enum_Security_Attivita.Agenda_AccessoMenu, _
        '                       enum_Security_Operazione.Modifica, _
        '                       Date.Now, _
        '                       "", _
        '                       objParametri_Utenti)
        '        End If
        '        If permesso = False Then
        '            r.RispostaOK = False
        '            r.Errore = "Non si hanno i permessi per questa operazione su questo gruppo di operazioni!"
        '            Return r
        '        End If

        '        Dim objImpre As New AgronicaCoreAnagrafeDAL.Imprese_Read
        '        Dim objCentriAziendali As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        '        Dim objParametriAgenda As New ParametriAgenda

        '        Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
        '        objParametriAgenda.Data = data
        '        objParametriAgenda.Id_Agenda = id_agenda
        '        objParametriAgenda.Lav_Cod = lav_cod
        '        objParametriAgenda.Piva = piva
        '        objParametriAgenda.Sa_Cod = sa_cod
        '        objParametriAgenda.RagSoc = objImpre.RagSoc_from_Piva(piva, objParametri_Server)
        '        objParametriAgenda.SaNome = objCentriAziendali.SaNome_from_SaCod(piva, sa_cod, objParametri_Server)

        '        r.RispostaOK = True
        '        r.RispostaStringa = "../Operazioni/DuplicaOperazione.aspx?id=" & id_agenda_checked

        'End Select


        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function elimina_operazione(ByVal righe_selezionate As String) As RispostaStandard

        Dim r As New RispostaStandard

        'Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametriAgenda As New ParametriAgenda

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim messaggio As String = ""
        Dim res As Boolean
        Dim N_Eliminate As Integer = 0
        Dim N_NON_Eliminate As Integer = 0
        Dim strErrore As String = AgronicaAgenda_2010.OperazioniNONEliminate & ": " & vbCrLf

        Try

            Dim jss = New JavaScriptSerializer()
            Dim operazioni As List(Of SelezionaMenuAgenda_Nuovo) = jss.Deserialize(Of List(Of SelezionaMenuAgenda_Nuovo))(righe_selezionate)

            For Each riga As SelezionaMenuAgenda_Nuovo In operazioni

                messaggio = ""

                objParametriAgenda.Data = riga.Data
                objParametriAgenda.Id_Agenda = riga.Id_Agenda
                objParametriAgenda.Lav_Cod = riga.Lav_Cod
                objParametriAgenda.Tipo_Operazione = CStr(CInt(enum_TipoOperazioneDB.Cancellazione))

                res = Operazione_Agenda_Utility.Cancella_Operazione_E_Collegate(objParametriAgenda, objParametri_Server, messaggio, True)

                If res Then
                    N_Eliminate += 1
                Else
                    N_NON_Eliminate += 1
                    strErrore &= riga.Lav_Des & " (" & riga.Data & ") " & messaggio & vbCrLf
                End If

            Next
            r.RispostaOK = True
            r.RispostaStringa = AgronicaAgenda_2010.OperazioniEliminate & ": " & N_Eliminate & " - " & AgronicaAgenda_2010.OperazioniNonEliminateLower & ": " & N_NON_Eliminate & " " & strErrore
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ImpossibileEliminareLeOperazioniSelezionate
            Return r
        Finally

        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function elimina_operazione_multipla(ByVal strChiaviComposite As String, proseguiInCasoDiAlert As Boolean) As RispostaStandard


        If Utils.LEGACY_SWITCH_USECOREWS Then

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                Dim r As New RispostaStandard
                r.Sessione = False
                Return r
            End If

            ' VAnni: 25/2/2020: Verifica Sessione..? Ok
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim riposta As RispostaStandard = Utility_Operazioni.elimina_operazione_multipla(strChiaviComposite, proseguiInCasoDiAlert, objParametri_Server, objParametri_Utenti)

            Return riposta
        Else

            Dim r As New RispostaStandard

            'Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            ' VAnni: 25/2/2020: Verifica Sessione..? Ok
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
            Dim msgErrBloccante As String = ""
            Dim msgErrNonBloccante As String = ""

            strChiaviComposite = strChiaviComposite.Split("|")(1)
            Dim listaChiaviComposite As String() = strChiaviComposite.Split(",")

            If listaChiaviComposite.Length > 1 Then
                'verifico permesso multi-cancellazione
                Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                Dim UtenteAbilitato_MultiCancellazione As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                                    HttpContext.Current.Session("ASG_Utente_Username"),
                                                                    HttpContext.Current.Session("ASG_IdServizio"),
                                                                    enum_Security_Attivita.ManutenzioneArchivi_MultiCancellazioneInterventi,
                                                                    enum_Security_Operazione.Modifica,
                                                                    Date.Now, "", objParametri_Utenti)

                If Not UtenteAbilitato_MultiCancellazione Then
                    r.RispostaOK = False
                    r.Errore = Resources.AgronicaAgenda_2010.EliminareUnOperazioneAllaVolta
                    Return r
                End If
            End If

            '-----------------------------------------------------
            '----------- CONNESSIONE E TRANSAZIONE ---------------
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            '-----------------------------------------------------

            For Each chiaveComposita As String In listaChiaviComposite
                Dim chiave = chiaveComposita.Split("_")
                Dim data As String = chiave(0)
                Dim id_agenda As String = chiave(1)
                Dim lav_cod As String = chiave(2)
                Dim blocco_flag As String = chiave(5)
                Dim veg_cod As String = chiave(6)
                Dim piva As String = If(chiave.Length > 7, chiave(7), "")

                Dim OUT_ErroreBloccante As Boolean = True

                Dim msgErr = elimina_operazione_singola_VerificaPermessi(piva,
                                                                         data,
                                                                         id_agenda,
                                                                         lav_cod,
                                                                         veg_cod,
                                                                         blocco_flag,
                                                                         proseguiInCasoDiAlert,
                                                                         objParametri_Server,
                                                                         objParametri_Utenti,
                                                                         OUT_ErroreBloccante) & vbCrLf & vbCrLf

                If msgErr.Trim() <> "" Then
                    If OUT_ErroreBloccante Then
                        msgErrBloccante &= msgErr
                    Else
                        msgErrNonBloccante &= msgErr
                    End If
                End If

            Next

            If msgErrBloccante.Trim() = "" AndAlso msgErrNonBloccante.Trim() = "" Then
                'chiudi connessione e commit transazione
                ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
                ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

                r.RispostaOK = True
                r.RispostaStringa = Resources.AgronicaAgenda_2010.OperazioneCancellata
            Else
                'inserimento fallito
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

                r.RispostaOK = False

                'Utilizzo il campo RispostaConferma per indicare che ho bisogno di una conferma per andare avanti
                If msgErrBloccante.Trim() <> "" Then
                    r.Errore = Resources.AgronicaAgenda_2010.OperazioneNonCancellataWarning & "<br>" & "<br>" & msgErrBloccante.Trim().Replace(vbCrLf, "<br>")
                    r.RispostaConferma = False
                Else
                    r.Errore = msgErrNonBloccante.Trim().Replace(vbCrLf, "<br>")
                    r.RispostaConferma = True
                End If

            End If

            Return r
        End If

    End Function

#End Region


#Region "Ricette"

    '<Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    'Public Shared Function Carica_Note_Ricetta(ByVal data_inizio As String, ByVal data_fine As String) As RispostaStandard

    '    Dim r As New RispostaStandard
    '    Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
    '    If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
    '        r.Sessione = False
    '        Return r
    '    End If

    '    ' VAnni: 25/2/2020: Verifica Sessione..? Ok
    '    Lingua.Gias_InizializzaCultura_DaSession()

    '    ''Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

    '    'If ricetta_cod <= 0 OrElse Ricetta_Operazione_Cod <= 0 OrElse lav_cod <= 0 OrElse veg_cod_op < 0 Then
    '    '    r.Errore = AgronicaAgenda_2010.ValoreDiUnParametroPassatoNonCorretto
    '    '    Return r
    '    'End If

    '    'Dim TargetUrl As String = NuovaOperazioneAgenda(lav_cod)

    '    'If TargetUrl <> "" AndAlso ricetta_cod <> 0 AndAlso Ricetta_Operazione_Cod <> 0 Then
    '    '    TargetUrl &= "?r=" & Stringa_Codifica(ricetta_cod, AgroKey_EncoderDecoder) & "&operazione_ricetta=" & Stringa_Codifica(Ricetta_Operazione_Cod, AgroKey_EncoderDecoder)
    '    'End If

    '    'Dim objParametriAgenda As New ParametriAgenda
    '    'objParametriAgenda.Data = Ricetta_Operazione_Data
    '    'objParametriAgenda.Id_Agenda = 0
    '    'objParametriAgenda.Sa_Cod = 0
    '    'objParametriAgenda.Lav_Cod = lav_cod
    '    'objParametriAgenda.Veg_Cod = veg_cod_op
    '    'objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio
    '    'objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
    '    'objParametriAgenda.Programmazione_Cod = 0
    '    'objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
    '    'objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS
    '    'objParametriAgenda.TipoRicetta = enum_TipoRicetta.Standard_Destinazioni
    '    'objParametriAgenda.salva()

    '    'r.RispostaStringa = TargetUrl
    '    'r.RispostaOK = True


    '    'modifico la finestra temporale in modo da caricare solamente quelle che sono attive a oggi
    '    objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(data_inizio, data_fine)

    '    Dim strFiltro As String = " Note_Intervento.Nota_Cod > 0 "
    '    Dim GruppoDes As String = ""

    '    AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Consigli,
    '                                                    False,
    '                                                     "", "",
    '                                                     0,
    '                                                     enum_Note_Intervento_Utilizzo.Ricetta,
    '                                                     strFiltro, "",
    '                                                     objParametri_Server,
    '                                                     1,
    '                                                     GruppoDes)

    '    If GruppoDes <> "" Then
    '        lblGiustificazioni.Text = GruppoDes
    '    End If

    '    objParametri_Server.ResettaFinestra()



    '    '(07/09/2016 fede) aggiunti tab standard (NotaGruppo_Cod <0)
    '    Dim objNoteGruppi_R As New AgronicaCoreContabDAL.Note_Intervento_Gruppi_R

    '    Dim DT_NoteGruppi As DataTable =
    '        objNoteGruppi_R.Leggi(0,
    '                              AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
    '                                " NotaGruppo_Cod < 0",
    '                                "",
    '                                 objParametri_Server)

    '    If Not DT_NoteGruppi Is Nothing Then
    '        For n = 0 To DT_NoteGruppi.Rows.Count - 1
    '            Select Case DT_NoteGruppi.Rows(n).Item("NotaGruppo_Cod")

    '                Case enum_Note_Intervento_Gruppi.Meteo
    '                    If DT_NoteGruppi.Rows(n).Item("visibile") = 0 Then
    '                        tabMeteo.Visible = False
    '                        tab_meteo.Visible = False
    '                    Else
    '                        AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Meteo,
    '                                            False,
    '                                             "", "",
    '                                             enum_Note_Intervento_Gruppi.Meteo,
    '                                             enum_Note_Intervento_Utilizzo.Ricetta,
    '                                             "", "",
    '                                             objParametri_Server,
    '                                             1)
    '                    End If

    '                Case enum_Note_Intervento_Gruppi.Vento_Intensita
    '                    If DT_NoteGruppi.Rows(n).Item("visibile") = 0 Then
    '                        tabVentoIntensita.Visible = False
    '                        tab_ventoint.Visible = False
    '                    Else
    '                        AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_VentoIntensita,
    '                                            False,
    '                                             "", "",
    '                                             enum_Note_Intervento_Gruppi.Vento_Intensita,
    '                                             enum_Note_Intervento_Utilizzo.Ricetta,
    '                                             "", "",
    '                                             objParametri_Server,
    '                                             1)
    '                    End If

    '                Case enum_Note_Intervento_Gruppi.Vento_Direzione
    '                    If DT_NoteGruppi.Rows(n).Item("visibile") = 0 Then
    '                        tabVentoDirezione.Visible = False
    '                        tab_ventodir.Visible = False
    '                    Else
    '                        AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_VentoDirezione,
    '                                            False,
    '                                             "", "",
    '                                             enum_Note_Intervento_Gruppi.Vento_Direzione,
    '                                             enum_Note_Intervento_Utilizzo.Ricetta,
    '                                             "", "",
    '                                             objParametri_Server,
    '                                             1)
    '                    End If
    '                Case enum_Note_Intervento_Gruppi.Temperatura
    '                    If DT_NoteGruppi.Rows(n).Item("visibile") = 0 Then
    '                        tabTemperatura.Visible = False
    '                        tab_temperatura.Visible = False
    '                    Else
    '                        AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Temperatura,
    '                                            False,
    '                                             "", "",
    '                                             enum_Note_Intervento_Gruppi.Temperatura,
    '                                             enum_Note_Intervento_Utilizzo.Ricetta,
    '                                             "", "",
    '                                             objParametri_Server,
    '                                             1)
    '                    End If
    '                Case enum_Note_Intervento_Gruppi.Orario
    '                    If DT_NoteGruppi.Rows(n).Item("visibile") = 0 Then
    '                        tabOrario.Visible = False
    '                        tab_orario.Visible = False
    '                    Else
    '                        AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Orario,
    '                                            False,
    '                                             "", "",
    '                                             enum_Note_Intervento_Gruppi.Orario,
    '                                             enum_Note_Intervento_Utilizzo.Ricetta,
    '                                             "", "",
    '                                             objParametri_Server,
    '                                             1)
    '                    End If
    '                Case enum_Note_Intervento_Gruppi.Motivazione
    '                    If DT_NoteGruppi.Rows(n).Item("visibile") = 0 Then
    '                        tabMotivazioni.Visible = False
    '                        tab_motivazioni.Visible = False
    '                    Else
    '                        AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Motivazione,
    '                                            False,
    '                                             "", "",
    '                                             enum_Note_Intervento_Gruppi.Motivazione,
    '                                             enum_Note_Intervento_Utilizzo.Ricetta,
    '                                             "", "",
    '                                             objParametri_Server,
    '                                             1)
    '                    End If
    '            End Select
    '        Next
    '    End If

    '    '-----------------------------------------------------
    '    'se ho note selezionate le checkko!!!
    '    '-----------------------------------------------------
    '    Dim Nota_Cod As Integer
    '    Dim TrovataNota As Boolean = False

    '    For i = 0 To objParametriAgenda.Note.Count - 1

    '        TrovataNota = False

    '        Nota_Cod = objParametriAgenda.Note(i).Nota_Cod

    '        Select Case Nota_Cod

    '            Case Is > 0 'utente

    '                For j = 0 To CBL_Consigli.Items.Count - 1
    '                    If Nota_Cod = CBL_Consigli.Items(j).Value Then
    '                        CBL_Consigli.Items(j).Selected = True
    '                        TrovataNota = True
    '                        Continue For
    '                    End If
    '                Next

    '            Case Is < 0 'standard

    '                If CBL_Meteo.Visible = True And TrovataNota = False Then
    '                    For j = 0 To CBL_Meteo.Items.Count - 1
    '                        If Nota_Cod = CBL_Meteo.Items(j).Value Then
    '                            CBL_Meteo.Items(j).Selected = True
    '                            TrovataNota = True
    '                            Continue For
    '                        End If
    '                    Next
    '                End If
    '                If CBL_VentoIntensita.Visible = True And TrovataNota = False Then
    '                    For j = 0 To CBL_VentoIntensita.Items.Count - 1
    '                        If Nota_Cod = CBL_VentoIntensita.Items(j).Value Then
    '                            CBL_VentoIntensita.Items(j).Selected = True
    '                            TrovataNota = True
    '                            Continue For
    '                        End If
    '                    Next
    '                End If
    '                If CBL_VentoDirezione.Visible = True And TrovataNota = False Then
    '                    For j = 0 To CBL_VentoDirezione.Items.Count - 1
    '                        If Nota_Cod = CBL_VentoDirezione.Items(j).Value Then
    '                            CBL_VentoDirezione.Items(j).Selected = True
    '                            TrovataNota = True
    '                            Continue For
    '                        End If
    '                    Next
    '                End If
    '                If CBL_Temperatura.Visible = True And TrovataNota = False Then
    '                    For j = 0 To CBL_Temperatura.Items.Count - 1
    '                        If Nota_Cod = CBL_Temperatura.Items(j).Value Then
    '                            CBL_Temperatura.Items(j).Selected = True
    '                            TrovataNota = True
    '                            Continue For
    '                        End If
    '                    Next
    '                End If
    '                If CBL_Orario.Visible = True And TrovataNota = False Then
    '                    For j = 0 To CBL_Orario.Items.Count - 1
    '                        If Nota_Cod = CBL_Orario.Items(j).Value Then
    '                            CBL_Orario.Items(j).Selected = True
    '                            TrovataNota = True
    '                            Continue For
    '                        End If
    '                    Next
    '                End If
    '                If CBL_Motivazione.Visible = True And TrovataNota = False Then
    '                    For j = 0 To CBL_Motivazione.Items.Count - 1
    '                        If Nota_Cod = CBL_Motivazione.Items(j).Value Then
    '                            CBL_Motivazione.Items(j).Selected = True
    '                            TrovataNota = True
    '                            Continue For
    '                        End If
    '                    Next
    '                End If

    '        End Select

    '        'nota salvata in agenda ma resa ora non visibile
    '        'se il SUO gruppo è visibile la aggiungo al suo gruppo
    '        'se il SUO gruppo è NON visibile la aggiungo alla lista generica consigli
    '        'la chekko in ogni caso
    '        If TrovataNota = False Then
    '            Dim objNota As New AgronicaCoreContabDAL.Note_Intervento_Utilizzo_R
    '            Dim Dt_Nota As DataTable
    '            Dim NotaDes As String = ""
    '            Dim FiltroNota As String = " NI.Nota_Cod = " & Nota_Cod.ToString
    '            Dt_Nota = objNota.LeggiNote_X_Utilizzo_Visibile(0, 0, False, FiltroNota, objParametri_Server)
    '            If Not Dt_Nota Is Nothing AndAlso Dt_Nota.Rows.Count > 0 Then
    '                NotaDes = Dt_Nota.Rows(0).Item("Nota_Des")
    '                Select Case Dt_Nota.Rows(0).Item("VisibileGruppo")
    '                    Case 0
    '                        CBL_Consigli.Items.Add(New ListItem(NotaDes, Nota_Cod))
    '                        CBL_Consigli.Items(CBL_Consigli.Items.Count - 1).Selected = True
    '                    Case Else
    '                        Select Case Dt_Nota.Rows(0).Item("NotaGruppo_Cod")
    '                            Case enum_Note_Intervento_Gruppi.Meteo
    '                                CBL_Meteo.Items.Add(New ListItem(NotaDes, Nota_Cod))
    '                                CBL_Meteo.Items(CBL_Meteo.Items.Count - 1).Selected = True
    '                            Case enum_Note_Intervento_Gruppi.Vento_Intensita
    '                                CBL_VentoIntensita.Items.Add(New ListItem(NotaDes, Nota_Cod))
    '                                CBL_VentoIntensita.Items(CBL_VentoIntensita.Items.Count - 1).Selected = True
    '                            Case enum_Note_Intervento_Gruppi.Vento_Direzione
    '                                CBL_VentoDirezione.Items.Add(New ListItem(NotaDes, Nota_Cod))
    '                                CBL_VentoDirezione.Items(CBL_VentoDirezione.Items.Count - 1).Selected = True
    '                            Case enum_Note_Intervento_Gruppi.Temperatura
    '                                CBL_Temperatura.Items.Add(New ListItem(NotaDes, Nota_Cod))
    '                                CBL_Temperatura.Items(CBL_Temperatura.Items.Count - 1).Selected = True
    '                            Case enum_Note_Intervento_Gruppi.Orario
    '                                CBL_Orario.Items.Add(New ListItem(NotaDes, Nota_Cod))
    '                                CBL_Orario.Items(CBL_Orario.Items.Count - 1).Selected = True
    '                            Case enum_Note_Intervento_Gruppi.Motivazione
    '                                CBL_Motivazione.Items.Add(New ListItem(NotaDes, Nota_Cod))
    '                                CBL_Motivazione.Items(CBL_Motivazione.Items.Count - 1).Selected = True
    '                            Case Else
    '                                CBL_Consigli.Items.Add(New ListItem(NotaDes, Nota_Cod))
    '                                CBL_Consigli.Items(CBL_Consigli.Items.Count - 1).Selected = True
    '                        End Select
    '                End Select

    '            End If
    '        End If

    '    Next


    '    Return r

    'End Function

    Private Shared Function XML_GeneraStringa_Ricetta(piva As String, objParametri_Server As AgronicaCoreParametri) As String

        Dim XmlDoc As New System.Xml.XmlDocument

        'Grilli 10/05/2018 Su indicazione di Fabrizio propongo di default l'anno + un progressivo
        Dim dt_elenco As DataTable = New AgronicaCoreContabDAL.Ricette_R().Leggi(0, piva, 0, enum_TipoRicetta.Standard_Destinazioni, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "Validita_Inizio >= " & UtilityProvider.Agro_SQL_SaveDate(New Date(Date.Now.Year, 1, 1)) & " AND Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(New Date(Date.Now.Year, 12, 31)), "", objParametri_Server)
        Dim contatore As Integer = If(Not IsNothing(dt_elenco) AndAlso dt_elenco.Rows.Count > 0, dt_elenco.Rows.Count, 0)
        Dim numeroRicetta As String = Date.Now.Year & "_" & CStr(contatore + 1)

        Dim frm_BaseCode, frm_TopCode As Integer
        UtilityProvider.Calcola_BaseCode_TopCode(frm_BaseCode, frm_TopCode,
                                                 HttpContext.Current.Session("ASG_ProgressivoGIAS"))


        '------------------------------------------------
        '----- Genero la struttura XML
        '------------------------------------------------

        Dim objXml As New AgronicaCoreXML.XML_Contab

        '----- DatiRicetta
        Dim XMLDatiRicetta As System.Xml.XmlElement = XmlDoc.CreateElement("DatiRicetta")
        XMLDatiRicetta.InnerXml = objXml.XML_Ricetta(
                                        enum_TipoOperazioneDB.Scrittura,
                                        CStr(HttpContext.Current.Session("ASG_SuperUser_CodFiscale")),
                                        frm_BaseCode,
                                        frm_TopCode,
                                        0,
                                        numeroRicetta,
                                        numeroRicetta,
                                        -1,
                                        "",
                                        AGRODATAINIZIO,
                                        AGRODATAFINE,
                                        piva,
                                        0,
                                        enum_TipoRicetta.Standard_Destinazioni,
                                        numeroRicetta,
                                        0)

        Dim XMLRicetta As System.Xml.XmlElement = XMLDatiRicetta.SelectSingleNode("Ricetta")

        '----- DatiRicettaxCultivar
        Dim XMLDatiRicettaxCultivar As System.Xml.XmlElement = XmlDoc.CreateElement("DatiRicettaxCultivar")
        XMLRicetta.AppendChild(XMLDatiRicettaxCultivar)


        Dim str_Ricetta_Cultivar As String = objXml.XML_RicettaxCultivar(enum_TipoOperazioneDB.Scrittura,
                                                        CStr(HttpContext.Current.Session("ASG_SuperUser_CodFiscale")),
                                                        0, -1, 0, AGRODATAINIZIO, AGRODATAFINE)

        XMLDatiRicettaxCultivar.InnerXml &= str_Ricetta_Cultivar


        '----- DatiRicettaxNote
        Dim XMLDatiRicettaxNote As System.Xml.XmlElement = XmlDoc.CreateElement("DatiRicettaxNote")
        XMLRicetta.AppendChild(XMLDatiRicettaxNote)

        '----- DatiRicetta_Operazioni
        Dim XMLDatiRicetta_Operazioni As System.Xml.XmlElement = XmlDoc.CreateElement("DatiRicetta_Operazioni")
        XMLRicetta.AppendChild(XMLDatiRicetta_Operazioni)


        '----- Assemblo la struttura

        XmlDoc.AppendChild(XMLDatiRicetta)

        objXml = Nothing

        '----- Restituisco il risultato

        Return XmlDoc.OuterXml

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function crea_ricetta(ByVal data_inizio As String, ByVal data_fine As String,
                                           ByVal id_agenda_checked As String, ByVal id_agenda As String,
                                           ByVal piva As String, ByVal sa_cod As String, ByVal veg_cod As String,
                                           ByVal ricetta_des As String, ByVal ricetta_numero As String,
                                           ByVal nota_des As String) As RispostaStandard

        Dim r As New RispostaStandard


        'Dim objParametri_Server = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim progressivoGias As Integer = HttpContext.Current.Session("ASG_ProgressivoGIAS")

        Dim RicetteOpW As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
        r = RicetteOpW.CreaRicetta(data_inizio, data_fine,
                               id_agenda_checked, id_agenda,
                               piva, sa_cod, veg_cod,
                               ricetta_des, ricetta_numero,
                               nota_des, progressivoGias, objParametri_Server, objParametri_Utenti)



        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function ricetta_numero_default(ByVal piva As String, ByVal sa_cod As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim ricetta_numero As String = ""

        Dim dt_elenco As DataTable = New AgronicaCoreContabDAL.Ricette_R().Leggi(0, piva, sa_cod,
                                                                                 enum_TipoRicetta.Standard_Destinazioni, 0,
                                                                                 AGRODATAINIZIO, AGRODATAFINE,
                                                                                 enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                 "Validita_Inizio >= " & UtilityProvider.Agro_SQL_SaveDate(New Date(Date.Now.Year, 1, 1)) & " AND Validita_Inizio <= " & UtilityProvider.Agro_SQL_SaveDate(New Date(Date.Now.Year, 12, 31)),
                                                                                 "", objParametri_Server)
        If Not IsNothing(dt_elenco) AndAlso dt_elenco.Rows.Count > 0 Then
            ricetta_numero = Date.Now.Year & "_" & CStr(dt_elenco.Rows.Count + 1)
        Else
            ricetta_numero = Date.Now.Year & "_1"
        End If

        r.RispostaOK = True
        r.RispostaStringa = ricetta_numero

        Return r

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function Ricette_Copia(ricetta_cod As Integer, piva As String, data_inizio As String, data_fine As String, tipo_ricetta As String) As RispostaStandard

        Dim r As New RispostaStandard

        'Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim OUTPUT_Ricetta_Cod As Integer = 0

        Dim r_W As New AgronicaCoreContabBIZ.Ricette_W
        Dim esito As Boolean = r_W.Ricetta_Copia(ricetta_cod, OUTPUT_Ricetta_Cod, HttpContext.Current.Session("ASG_ProgressivoGIAS"), objParametri_Server)

        r.RispostaOK = esito

        Return r

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function Ricette_Stampa(ricetta_cod As Integer, ricetta_stampa_tipo As Integer) As RispostaStandard

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        Lingua.Gias_InizializzaCultura_DaSession()

        Dim r As New RispostaStandard
        'Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        'Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        Lingua.Gias_InizializzaCultura_DaSession()

        r.RispostaOK = True
        r.RispostaStringa = "../Ricette/Stampa/Ricetta_Stampa.aspx?" &
                            "ricetta_cod=" & Stringa_Codifica(ricetta_cod, AgroKey_EncoderDecoder, objParametri_Server) &
                            "&ricetta_stampa_tipo=" & Stringa_Codifica(ricetta_stampa_tipo, AgroKey_EncoderDecoder, objParametri_Server)

        Return r

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function Ricette_Cancella(ricetta_cod As Integer, ricetta_operazione_cod As Integer, in_uso As Integer, isBrogliaccio As Boolean) As RispostaStandard

        If Utils.LEGACY_SWITCH_USECOREWS Then
            Dim r As New RispostaStandard
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            Return Utility_Operazioni.Ricette_Cancella(ricetta_cod, ricetta_operazione_cod, in_uso)
        Else
            Dim r As New RispostaStandard
            'Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            ' VAnni: 25/2/2020: Verifica Sessione..? Ok
            Lingua.Gias_InizializzaCultura_DaSession()

            'Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            'Controllo permessi cancellazione...
            Dim ID_Attivita As enum_Security_Attivita = enum_Security_Attivita.Gest_Ricette
            If isBrogliaccio Then
                ID_Attivita = enum_Security_Attivita.Brogliaccio
            End If
            Dim permesso As Boolean = controlloPermessi(ID_Attivita, enum_TipoOperazioneDB.Scrittura)
            If Not permesso Then
                r.RispostaOK = True
                r.Errore = AgronicaAgenda_2010.OperazioneNonCancellataWarning & "<br>" & "<br>" & Gias.MancanzaPermessiCancellazioneTipoOperazione
                Return r
            End If


            If ricetta_cod <= 0 Then
                r.Errore = "Il parametro ricetta_cod non è stato valorizzato."
                Return r
            End If

            If ricetta_operazione_cod <= 0 Then
                r.Errore = "Il parametro ricetta_operazione_cod non è stato valorizzato."
                Return r
            End If

            If in_uso <> 0 Then
                r.Errore = AgronicaAgenda_2010.ImpossibileCancellareRicettaInUso
                Return r
            End If

            Dim a As New AgronicaCoreContabBIZ.Ricette_Operazioni_W
            Dim msgErr As String = a.Ricetta_Operazione_Cancella_ESeUnicaAncheLaRicettaPadre(ricetta_cod, ricetta_operazione_cod, objParametri_Server)

            If msgErr = "" Then
                r.RispostaOK = True
            Else
                r.Errore = msgErr
            End If

            Return r
        End If

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function Ricette_VerificaSeCostiCollegatiECancella(ricetta_cod As Integer, ricetta_operazione_cod As Integer, in_uso As Integer, APP_Ricetta_Operazione_ID As String) As RispostaStandard

        If Utils.LEGACY_SWITCH_USECOREWS Then
            Dim r As New RispostaStandard

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            r = Utility_Operazioni.Ricette_VerificaSeCostiCollegatiECancella(ricetta_cod, ricetta_operazione_cod, in_uso, APP_Ricetta_Operazione_ID)
            Return r
        Else

            Dim r As New RispostaStandard
            'Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            ' VAnni: 25/2/2020: Verifica Sessione..? Ok
            Lingua.Gias_InizializzaCultura_DaSession()

            If ricetta_cod <= 0 Then
                r.Errore = "Il parametro ricetta_cod non è stato valorizzato."
                Return r
            End If

            If ricetta_operazione_cod <= 0 Then
                r.Errore = "Il parametro ricetta_operazione_cod non è stato valorizzato."
                Return r
            End If

            If in_uso <> 0 Then
                r.Errore = AgronicaAgenda_2010.ImpossibileCancellareRicettaInUso
                Return r
            End If

            'Controllo permessi cancellazione...
            Dim permesso As Boolean = controlloPermessi(enum_Security_Attivita.Brogliaccio, enum_TipoOperazioneDB.Scrittura)
            If Not permesso Then
                r.RispostaOK = True
                r.Errore = AgronicaAgenda_2010.OperazioneNonCancellataWarning & "<br>" & "<br>" & Gias.MancanzaPermessiCancellazioneTipoOperazione
                Return r
            End If

            Dim CDG_APP As New AgronicaCoreContabBIZ.CDG_APP
            Dim dtMovimentiCDG = CDG_APP.LeggiRiferimentiCDGInterventiAPP(APP_Ricetta_Operazione_ID, objParametri_Server)

            If dtMovimentiCDG.Rows.Count > 0 Then  'Allora ci sono ore APP collegate
                r.RispostaOK = True
                r.RispostaStringa = AgronicaAgenda_2010.EsistonoCostiCollegatiImpossibileCancellare
            Else
                r = Ricette_Cancella(ricetta_cod, ricetta_operazione_cod, in_uso, isBrogliaccio:=True)
            End If

            Return r

        End If

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function Ricette_CancellaRicettaCancellaCosti(ricetta_cod As Integer, ricetta_operazione_cod As Integer, in_uso As Integer, APP_Ricetta_Operazione_ID As String) As RispostaStandard

        If Utils.LEGACY_SWITCH_USECOREWS Then
            Lingua.Gias_InizializzaCultura_DaSession()

            Return Utility_Operazioni.Ricette_Cancella(ricetta_cod, ricetta_operazione_cod, in_uso)
        Else
            Lingua.Gias_InizializzaCultura_DaSession()

            'La cancellazione dei costi in realtà prevede il fatto di ignorarli e lasciare degli zombie non collegati
            Return Ricette_Cancella(ricetta_cod, ricetta_operazione_cod, in_uso, isBrogliaccio:=True)
        End If

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function Ricette_CancellaRicettaConvertiCosti(ricetta_cod As Integer, ricetta_operazione_cod As Integer, in_uso As Integer, APP_Ricetta_Operazione_ID As String) As RispostaStandard

        If Utils.LEGACY_SWITCH_USECOREWS Then
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                Dim r As New RispostaStandard
                r.Sessione = False
                Return r
            End If

            ' VAnni: 25/2/2020: Verifica Sessione..? Ok
            Lingua.Gias_InizializzaCultura_DaSession()

            Return Utility_Operazioni.Ricette_CancellaRicettaConvertiCosti(ricetta_cod, ricetta_operazione_cod, in_uso, APP_Ricetta_Operazione_ID)
        Else
            'TODO: TRANSAZIONE

            Dim r As New RispostaStandard
            'Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            ' VAnni: 25/2/2020: Verifica Sessione..? Ok
            Lingua.Gias_InizializzaCultura_DaSession()

            If ricetta_cod <= 0 Then
                r.Errore = "Il parametro ricetta_cod non è stato valorizzato."
                Return r
            End If

            If ricetta_operazione_cod <= 0 Then
                r.Errore = "Il parametro ricetta_operazione_cod non è stato valorizzato."
                Return r
            End If

            If in_uso <> 0 Then
                r.Errore = AgronicaAgenda_2010.ImpossibileCancellareRicettaInUso
                Return r
            End If

            Try
                'apro una transazione
                ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

                Dim CDG_APP As New AgronicaCoreContabBIZ.CDG_APP
                Dim esito As Boolean = CDG_APP.ConvertiMovimentiCDGInterventiAPP(APP_Ricetta_Operazione_ID, objParametri_Server)

                If Not esito Then
                    Throw New Exception("Errore durante la conversione dei costi")
                End If

                r = Ricette_Cancella(ricetta_cod, ricetta_operazione_cod, in_uso, isBrogliaccio:=True)

                'Se è andato tutto bene
                ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server) 'Flag_Commit1_Rollback2

            Catch ex As Exception
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
                r.RispostaOK = False
                r.Errore = ex.Message
            End Try

            Return r

        End If

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function ApriRicettaFattoInPaginaQdC(ricetta_cod As Integer, Ricetta_Operazione_Cod As Integer, lav_cod As Integer, veg_cod_op As String, Ricetta_Operazione_Data As Date) As RispostaStandard



        Dim r As New RispostaStandard
        'Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        Lingua.Gias_InizializzaCultura_DaSession()

        'Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        If IsNothing(ricetta_cod) OrElse ricetta_cod <= 0 OrElse IsNothing(Ricetta_Operazione_Cod) OrElse Ricetta_Operazione_Cod <= 0 OrElse
            IsNothing(lav_cod) OrElse lav_cod <= 0 OrElse IsNothing(veg_cod_op) OrElse veg_cod_op < 0 Then
            r.Errore = AgronicaAgenda_2010.ValoreDiUnParametroPassatoNonCorretto
            Return r
        End If

        Dim TargetUrl As String = NuovaOperazioneAgenda(lav_cod, -1)
        If TargetUrl = "error" Then
            r.Errore = "TODO Non si hanno i permessi per visualizzare questo tipo di operazione"
            Return r
        End If

        If TargetUrl <> "" AndAlso ricetta_cod <> 0 AndAlso Ricetta_Operazione_Cod <> 0 Then
            TargetUrl &= "?r=" & Stringa_Codifica(ricetta_cod, AgroKey_EncoderDecoder) & "&operazione_ricetta=" & Stringa_Codifica(Ricetta_Operazione_Cod, AgroKey_EncoderDecoder)
        End If

        Dim objParametriAgenda As New ParametriAgenda
        objParametriAgenda.Veg_Cod = veg_cod_op
        objParametriAgenda.Data = Ricetta_Operazione_Data
        objParametriAgenda.salva()

        r.RispostaStringa = TargetUrl
        r.RispostaOK = True

        Return r

    End Function

    <Script.Services.ScriptMethod()> <WebMethod(EnableSession:=True)>
    Public Shared Function ApriRicettaDaFareInPaginaFatto(ricetta_cod As Integer, Ricetta_Operazione_Cod As Integer, lav_cod As Integer, veg_cod_op As Integer, Ricetta_Operazione_Data As Date) As RispostaStandard

        Dim r As New RispostaStandard
        'Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        ' VAnni: 25/2/2020: Verifica Sessione..? Ok
        Lingua.Gias_InizializzaCultura_DaSession()

        'Dim objParametri_Utenti = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim objParametriAgenda As New ParametriAgenda
        objParametriAgenda.Data = Ricetta_Operazione_Data
        objParametriAgenda.Id_Agenda = 0
        objParametriAgenda.Sa_Cod = 0
        objParametriAgenda.Lav_Cod = lav_cod
        objParametriAgenda.Veg_Cod = veg_cod_op
        objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio
        objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
        objParametriAgenda.Programmazione_Cod = 0
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Menu_BS
        objParametriAgenda.TipoRicetta = enum_TipoRicetta.Standard_Destinazioni
        objParametriAgenda.salva()

        If ricetta_cod <= 0 OrElse Ricetta_Operazione_Cod <= 0 OrElse lav_cod <= 0 OrElse veg_cod_op < 0 Then
            r.Errore = AgronicaAgenda_2010.ValoreDiUnParametroPassatoNonCorretto
            Return r
        End If

        Dim TargetUrl As String = NuovaOperazioneAgenda(lav_cod, -1)
        If TargetUrl = "error" Then
            r.Errore = "TODO Non si hanno i permessi per visualizzare questo tipo di operazione"
            Return r
        End If

        If TargetUrl <> "" AndAlso ricetta_cod <> 0 AndAlso Ricetta_Operazione_Cod <> 0 Then
            TargetUrl &= "?r=" & Stringa_Codifica(ricetta_cod, AgroKey_EncoderDecoder) & "&operazione_ricetta=" & Stringa_Codifica(Ricetta_Operazione_Cod, AgroKey_EncoderDecoder)
        End If

        r.RispostaStringa = TargetUrl
        r.RispostaOK = True

        Return r

    End Function
#End Region

End Class


Public Class SelezionaMenuAgenda_Nuovo
    Public Data As String
    Public Operazione As String
    Public Piva As String
    Public Sa_Cod As String
    Public Lav_Cod As String
    Public Lav_Des As String
    Public Id_Agenda As String
    Public Blocco_Flag As String
    'Public Info As String
    Public Veg_Cod As String
    Public Ricetta_Cod As String
    Public Rag_Soc As String
    Public Gru_Des As String
    Public Info As String
    Public Dettagli As String
End Class
