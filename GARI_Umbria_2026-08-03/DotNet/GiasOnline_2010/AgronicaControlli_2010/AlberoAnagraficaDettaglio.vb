Imports System.Web.Services
Imports System.Text
Imports System.Data
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services

Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Albero
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtility
Imports System.ComponentModel
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreVarieDAL

Imports AgronicaCoreModelloInSviluppo
Imports AgronicaCoreModelsSTD.Gis
Imports System.Text.RegularExpressions

''' <summary>
''' Per utilizzare questo controllo è necessario includere all'iterno della pagina i seguenti script 
''' jquery.cookie.js
''' jquery.hotkeys.js
''' jquery.jstree.js
''' 
''' Se si caricano le analisi, ricordarsi di eliminare al dispose della pagina le due variabili di sessione
''' HttpContext.Current.Session("DT_Analisi") = Nothing
''' HttpContext.Current.Session("DT_Campioni") = Nothing
''' </summary>
''' <remarks></remarks>

<DefaultProperty("Text"), ToolboxData("<{0}:AlberoAnagraficaDettaglio runat=server></{0}:AlberoAnagraficaDettaglio>")> Public Class AlberoAnagraficaDettaglio
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class AlberoAnagraficaDettaglio
    '    Inherits ScriptControl

    Public Hidden As HiddenField

    Private _livello_eplosione As Integer = 0

    Private _Flag_Carica_Primo_Giro As Boolean = True

    Private _Flag_Esplodi_Tutto As Boolean = False
    Private _Flag_CheckBox As Boolean = False

    Private _Flag_Planning As Boolean = False

    Private _Flag_Anagrafica As Boolean = False

    Private _Flag_Contatti As Boolean = False
    Private _Flag_Analisi As Boolean = False
    Private _Flag_PianoConcimazione As Boolean = False
    Private _Flag_Esercizio As Boolean = False
    Private _Flag_ParcoMacchine As Boolean = False
    Private _Flag_CatastoAziendale As Boolean = False
    Private _Flag_Fabbricati As Boolean = False
    Private _Flag_PortafoglioProdotti As Boolean = False
    Private _Flag_Singola_Selezione As Boolean = True

    Private _Flag_Appezzamenti_Filtra_Tecnico As Boolean = True

    Private _Flag_Agenda As Boolean = False

    Private _CheckBoxes As New CheckBoxFlags
    Private _Piva As String
    Private _Sa_Cod As String
    Private _Veg_Cod As Integer = 0
    Private _Cul_Cod As Integer

    Private _Flag_JS As Boolean = False

    Private _DT_Analisi As DataTable
    Private _DT_Campioni As DataTable
    Private _ordinaDataUltimoImpianto As Boolean = False
    Private _visualizzaRiferimentoAlfanumericoImpianto As Boolean = False

    Private _TipoOperazioneColturale As String


    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        Hidden = New HiddenField
        Hidden.ID = "HiddenSelezioneAlberoAnagrafica" & Me.ClientID

        'Hidden.ClientIDMode = UI.ClientIDMode.Static


        Me.Controls.Add(Hidden)

        MyBase.OnInit(e)
    End Sub



#Region "Property"




    Public Property Flag_Anagrafica() As Boolean
        Get
            Return _Flag_Anagrafica
        End Get
        Set(value As Boolean)
            _Flag_Anagrafica = value
        End Set
    End Property


    Public Property Flag_Carica_Primo_Giro() As Boolean
        Get
            Return _Flag_Carica_Primo_Giro
        End Get
        Set(value As Boolean)
            _Flag_Carica_Primo_Giro = value
        End Set
    End Property

    Public Property visualizzaRiferimentoAlfanumericoImpianto() As Boolean
        Get
            Return _visualizzaRiferimentoAlfanumericoImpianto
        End Get
        Set(value As Boolean)
            _visualizzaRiferimentoAlfanumericoImpianto = value
        End Set
    End Property

    Public Property ordinaDataUltimoImpianto() As Boolean
        Get
            Return _ordinaDataUltimoImpianto
        End Get
        Set(value As Boolean)
            _ordinaDataUltimoImpianto = value
        End Set
    End Property


    Public Property Flag_Agenda() As Boolean
        Get
            Return _Flag_Agenda
        End Get
        Set(value As Boolean)
            _Flag_Agenda = value
        End Set
    End Property

    Private _Flag_Ricette As Boolean = False
    Public Property Flag_Ricette() As Boolean
        Get
            Return _Flag_Ricette
        End Get
        Set(value As Boolean)
            _Flag_Ricette = value
        End Set
    End Property


    Public Property Flag_Esplodi_Tutto() As Boolean
        Get
            Return _Flag_Esplodi_Tutto
        End Get
        Set(ByVal value As Boolean)
            _Flag_Esplodi_Tutto = value
        End Set
    End Property

    Public Property Flag_Planning() As Boolean
        Get
            Return _Flag_Planning
        End Get
        Set(ByVal value As Boolean)
            _Flag_Planning = value
        End Set
    End Property

    Public Property Flag_Singola_Selezione() As Boolean
        Get
            Return _Flag_Singola_Selezione
        End Get
        Set(ByVal value As Boolean)
            _Flag_Singola_Selezione = value
        End Set
    End Property

    Public Property Flag_CheckBox() As Boolean
        Get
            Return _Flag_CheckBox
        End Get
        Set(ByVal value As Boolean)
            _Flag_CheckBox = value
        End Set
    End Property
    Public Property Flag_Contatti() As Boolean
        Get
            Return _Flag_Contatti
        End Get
        Set(ByVal value As Boolean)
            _Flag_Contatti = value
        End Set
    End Property

    Public Property Flag_Analisi() As Boolean
        Get
            Return _Flag_Analisi
        End Get
        Set(ByVal value As Boolean)
            _Flag_Analisi = value
        End Set
    End Property

    Public Property Flag_PianoConcimazione() As Boolean
        Get
            Return _Flag_PianoConcimazione
        End Get
        Set(ByVal value As Boolean)
            _Flag_PianoConcimazione = value
        End Set
    End Property

    Public Property Flag_Esercizio() As Boolean
        Get
            Return _Flag_Esercizio
        End Get
        Set(ByVal value As Boolean)
            _Flag_Esercizio = value
        End Set
    End Property

    Public Property Flag_Appezzamenti_Filtra_Tecnico() As Boolean
        Get
            Return _Flag_Appezzamenti_Filtra_Tecnico
        End Get
        Set(ByVal value As Boolean)
            _Flag_Appezzamenti_Filtra_Tecnico = value
        End Set
    End Property

    Public Property Cul_Cod() As Integer
        Get
            Return _Cul_Cod
        End Get
        Set(ByVal value As Integer)
            _Cul_Cod = value
        End Set
    End Property
    Public Property Veg_Cod() As Integer
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal value As Integer)
            _Veg_Cod = value
        End Set
    End Property

    Public Property Flag_PortafoglioProdotti() As Boolean
        Get
            Return _Flag_PortafoglioProdotti
        End Get
        Set(ByVal value As Boolean)
            _Flag_PortafoglioProdotti = value
        End Set
    End Property

    Private _PivaPadre As String = ""


    Public Property PivaPadre() As String
        Get
            Return _PivaPadre
        End Get
        Set(value As String)
            _PivaPadre = value
        End Set
    End Property

    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property


    Public Property Sa_Cod() As String
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As String)
            If (value = "") Then
                _Sa_Cod = 0
            Else
                _Sa_Cod = value
            End If
        End Set
    End Property


    Public Property Flag_ParcoMacchine() As Boolean
        Get
            Return _Flag_ParcoMacchine
        End Get
        Set(ByVal value As Boolean)
            _Flag_ParcoMacchine = value
        End Set
    End Property

    Public Property Flag_CatastoAziendale() As Boolean
        Get
            Return _Flag_CatastoAziendale
        End Get
        Set(ByVal value As Boolean)
            _Flag_CatastoAziendale = value
        End Set
    End Property

    Public Property Valore_Albero() As String
        Get
            Return Hidden.Value
        End Get
        Set(ByVal value As String)
            Hidden.Value = value
        End Set
    End Property


    Public Property Flag_Fabbricati() As Boolean
        Get
            Return _Flag_Fabbricati
        End Get
        Set(ByVal value As Boolean)
            _Flag_Fabbricati = value
        End Set
    End Property

    Public Property CheckBoxes() As CheckBoxFlags
        Get
            Return _CheckBoxes
        End Get
        Set(ByVal value As CheckBoxFlags)
            _CheckBoxes = value
        End Set
    End Property

    Private _FlagModalitaSementieri As Boolean = False
    Public Property FlagModalitaSementieri() As Boolean
        Get
            Return _FlagModalitaSementieri
        End Get
        Set(value As Boolean)
            _FlagModalitaSementieri = value
        End Set
    End Property


    Public Property TipoOperazioneColturale As String
        Get
            Return _TipoOperazioneColturale
        End Get
        Set(ByVal value As String)
            _TipoOperazioneColturale = value
        End Set
    End Property
    Private _GruppoOperazioneColturale As String
    Public Property GruppoOperazioneColturale As String
        Get
            Return _GruppoOperazioneColturale
        End Get
        Set(ByVal value As String)
            _GruppoOperazioneColturale = value
        End Set
    End Property

    '  Galassi, 27/06/2016 10:47:49: Proprietà che gestisce il livello di esplosione dell'albero
    Public Property LivelloEsplosione As Integer
        Get
            Return _livello_eplosione
        End Get
        Set(ByVal value As Integer)
            _livello_eplosione = value
        End Set
    End Property

#End Region

#Region "CARICAMENTO DEI NODI"
    ''' <summary>
    ''' Funzione che viene invocata quando si richiede il caricamento del sottonodo
    ''' NB ricordarsi di far "l'overload" all'interno della pagina chiamante come nell'esempio seguente
    ''' WebMethod(EnableSession:=True)> _
    ''' Public Shared Function GetNodes(ByVal id As String) As String
    '''    Return AgronicaControlli_2010.ServerControl1.GetNodesAgenda(id)
    ''' End Function
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod(EnableSession:=True)>
    Public Function GetNodesAlberoAnagrafe(ByVal id As String,
                                                  ByVal PathRoot As String) As String
        Dim results As New List(Of AjaxTreeNodeJsonObject)
        Dim ser As New JavaScriptSerializer()



        Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        'Valorizzo il Codice_Fiscale_Tecnico prendendolo dalla tabella Gruppi_Utente
        Dim Codice_Fiscale_Tecnico As String = "CF TEC"

        Dim LeggiCFGDatiIniziali As New Configurazione_Siti_R
        Dim GIS_EscludiFiltroCodiceFiscaleTecnico As String =
            LeggiCFGDatiIniziali.Leggi_Valore(0, "GIS_EscludiFiltroCodiceFiscaleTecnico", "", "", objParametri_Server)

        If GIS_EscludiFiltroCodiceFiscaleTecnico = "false" Or GIS_EscludiFiltroCodiceFiscaleTecnico = "" Then
            Dim xUtenteCorrente As String = CType(objParametri_Server, AgronicaCoreDataProvider.AgronicaCoreParametri).UtenteUsername
            Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
            Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(xUtenteCorrente, objParametri_Utenti)
        End If


        Dim data_inizio, data_fine As Date
        data_inizio = CType(HttpContext.Current.Session("ASG_objParametri_Server"), AgronicaCoreDataProvider.AgronicaCoreParametri).FinestraTemporaleInizio
        data_fine = CType(HttpContext.Current.Session("ASG_objParametri_Server"), AgronicaCoreDataProvider.AgronicaCoreParametri).FinestraTemporaleFine


        If String.IsNullOrEmpty(id) Or id = "0" Then

            'controllo se devo precaricare il nodo delle analisi
            If Flag_Analisi = True Then
                PrecaricaNodoAnalisi()
                PrecaricaNodoCampioni()
            End If

            CaricaPrimi3Livelli(results, id, PathRoot, data_inizio, data_fine)
            'CaricaFinoALivelloX(results, 2, id, PathRoot, data_inizio, data_fine)

        Else

            'decodifico la chiave che mi arriva
            Dim Chiave As Integer
            Albero.ChiaveAlbero_Decodifica_TipoNodo_x_json(id, Chiave)

            Select Case Chiave

                Case enum_TipoNodo.Centro
                    CaricaInfoDentroACentro(results, id, PathRoot, data_inizio, data_fine)


                Case enum_TipoNodo.CatastoAziendale
                    'carico le 'richieste le particelle
                    CaricaParticelle(results, id, PathRoot, data_inizio, data_fine)

                Case enum_TipoNodo.PlanningTestata
                    'CaricaPlanningDettaglio(results, id, PathRoot)
                    'Case enum_TipoNodo.PlanningEntita
                    CaricaPlanningDettaglioEntita(results, id, PathRoot, data_inizio, data_fine)


                Case enum_TipoNodo.x_ListaFabbricatiAziendali
                    'carico la lista dei fabbricati
                    CaricaListaFabbricati(results, id, PathRoot, data_inizio, data_fine)


                Case enum_TipoNodo.Campo
                    If Flag_Analisi = True Then
                        CaricaAnalisi(results, id, PathRoot, data_inizio, data_fine)
                    End If
                    Dim xCampoCod As Integer
                    Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Campocod_x_json(id, "", 0, xCampoCod)
                    CaricaAppezzamenti(results, id, PathRoot, xCampoCod, Codice_Fiscale_Tecnico, data_inizio, data_fine)


                Case enum_TipoNodo.Serra
                    If Flag_Analisi = True Then
                        CaricaAnalisi(results, id, PathRoot, data_inizio, data_fine)
                    End If
                    Dim xCampoCod As Integer
                    Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Campocod_x_json(id, "", 0, xCampoCod)
                    CaricaAppezzamenti(results, id, PathRoot, xCampoCod, Codice_Fiscale_Tecnico, data_inizio, data_fine)


                Case enum_TipoNodo.Appezzamento
                    If Flag_Analisi = True Then
                        CaricaAnalisi(results, id, PathRoot, data_inizio, data_fine)
                    End If
                    CaricaImpianti(results, id, PathRoot, data_inizio, data_fine)

                    'Case enum_TipoNodo.ImpiantoArborea, _
                    '     enum_TipoNodo.ImpiantoErbacea, _
                    '     enum_TipoNodo.ImpiantoOrticola, _
                    '     enum_TipoNodo.ImpiantoNudo
                    '    CaricaPianoConcimazioni(results, id, PathRoot, data_inizio, data_fine)


                Case enum_TipoNodo.Impianto_Generico, enum_TipoNodo.ImpiantoArborea, enum_TipoNodo.ImpiantoErbacea, enum_TipoNodo.ImpiantoNudo, enum_TipoNodo.ImpiantoOrticola
                    CaricaRicette(results, id, PathRoot, data_inizio, data_fine)


                Case enum_TipoNodo.ricette_Testata
                    CaricaRicetteOperazioni(results, id, PathRoot, data_inizio, data_fine)

                Case enum_TipoNodo.Agenda
                    CaricaAgendaDettagli(results, id, PathRoot, data_inizio, data_fine)

                    'Case esercizio
                Case enum_TipoNodo.DistintaDiProduzione
                    CaricaPianoConcimazioni(results, id, PathRoot, data_inizio, data_fine)



            End Select
        End If
        ser.MaxJsonLength = 50000000
        Return ser.Serialize(results)

    End Function


    ''' <summary>
    ''' funzione per caricare l'albero fino al livello desiderato. Funzionde presa da CaricaPrimi3Livelli e modificata per estenderla a tutti
    ''' i livelli.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CaricaFinoALivelloX(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                  ByVal Livello As Integer,
                                                  ByVal Id As String,
                                                  ByVal PathRoot As String,
                                                  ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)

        Dim xChiave As String = ""
        Dim Testo As String = ""

        Dim Livello_Impresa As New List(Of AjaxTreeNodeJsonObject)

        If Livello >= 1 Then
            '************************
            '***** NODO UTENTE ******
            '************************
            Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
            Dim DTUtente As DataTable

            DTUtente = objUtente.Leggi(HttpContext.Current.Session("ASG_Utente_Username"),
                                       5,
                                       enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                       "", "", HttpContext.Current.Session("ASG_objParametri_Utenti"))
            Dim TipoUtente As Integer

            'Prelevo la Ragione Sociale oppure Nome e Cognome
            If DTUtente.Rows.Count > 0 Then
                'Verifico il tipo di utente ... Azienda/Persona
                TipoUtente = DTUtente.Rows(0).Item("Flag_Azienda_Persona")
                If TipoUtente = 1 Then
                    Testo = DTUtente.Rows(0).Item("Rag_Soc")
                Else
                    Testo = DTUtente.Rows(0).Item("Cognome") & " " &
                            DTUtente.Rows(0).Item("Nome")
                End If
            Else
                Testo = HttpContext.Current.Session("ASG_Utente_Username")
            End If
            'Genero la chiave
            Call Albero.ChiaveAlbero_Codifica_x_json(xChiave,
                                        enum_TipoNodo.Utente,
                                        , , , , , , , , , , , , , )

            Dim Radice As AjaxTreeNodeJsonObject

            If Flag_CheckBox Then
                If Not CheckBoxes.Flag_CheckBoxUtente Then
                    Radice = New AjaxTreeNodeJsonObject(xChiave, AgroPrefix_Utente & Testo, "jstree-no-checkboxes PIPPO", "", "#", Livello_Impresa)
                Else
                    Radice = New AjaxTreeNodeJsonObject(xChiave, AgroPrefix_Utente & Testo, "", "", "#", Livello_Impresa)
                End If
            Else
                Radice = New AjaxTreeNodeJsonObject(xChiave, AgroPrefix_Utente & Testo, "", "", "#", Livello_Impresa)
            End If

            Radice.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Utente, "")
            Radice.state = "open"

            '======================================================
            '===== Verifica l'uscita dalla routine
            '======================================================
            'Se la partita IVA e' nulla allora esco
            If _Piva = "" Then
                Return
            End If

            If Livello >= 2 Then

                '#####################
                '#####  IMPRESA  #####
                '#####################

                'Creo gli oggetti COM
                Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Dim DTImprese As DataTable
                Dim ValiditaFineImpresa As Date
                Dim TipoImpresaGerarchia As Integer

                Dim xPiva As String = ""
                Dim xSa_Cod As String = ""
                Dim xRag_Soc As String = ""
                Dim xSa_Nome As String = ""
                Dim xValidita_Fine As String = ""
                Dim ValidazioneNodo As String = ""
                Dim CertificatiBloccati As String = ""

                'Leggo le informazioni sull'impresa selezionata			
                DTImprese = objImprese.Leggi(CStr(_Piva),
                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             "", "", HttpContext.Current.Session("ASG_objParametri_Server"))
                objImprese = Nothing

                If DTImprese.Rows.Count > 0 Then
                    xPiva = DTImprese.Rows(0).Item("Piva")
                    xRag_Soc = DTImprese.Rows(0).Item("Rag_Soc")
                    ValidazioneNodo = DTImprese.Rows(0).Item("Validazione") & ""
                    CertificatiBloccati = DTImprese.Rows(0).Item("Blk_Flag") & ""
                    ValiditaFineImpresa = CDate(DTImprese.Rows(0).Item("validita_fine"))
                    TipoImpresaGerarchia = DTImprese.Rows(0).Item("TipoImpresaGerarchia")
                Else
                    ValiditaFineImpresa = #12/31/2100#
                    xPiva = "?????"
                    xRag_Soc = "?????"
                    ValidazioneNodo = "-1"
                    TipoImpresaGerarchia = 1
                End If

                '----- Creo il nodo IMPRESA sul albero
                If CertificatiBloccati = "-1" Then
                    xRag_Soc = "--SOSPESA-- " & xRag_Soc
                End If

                'Genero una chiave
                Call Albero.ChiaveAlbero_Codifica_x_json(xChiave,
                                            enum_TipoNodo.Impresa,
                                            Piva:=xPiva,
                                            PivaPadre:=_PivaPadre
                                        )

                Dim TipoImpresa As enum_TipoNodo

                Select Case TipoImpresaGerarchia
                    Case 1
                        TipoImpresa = enum_TipoNodo.Impresa
                    Case 2
                        TipoImpresa = enum_TipoNodo.x_Cooperativa
                    Case 3
                        TipoImpresa = enum_TipoNodo.x_Consorzio
                    Case 4
                        TipoImpresa = enum_TipoNodo.x_OP
                End Select


                Dim Livello2 As New List(Of AjaxTreeNodeJsonObject)
                'Livello_Impresa.Add(New AjaxTreeNodeJsonObject(xChiave, AgroPrefix_Impresa & xRag_Soc, "#", Livello2))

                If Flag_CheckBox Then

                    If Not CheckBoxes.Flag_CheckBoxImpresa Then
                        Livello_Impresa.Add(New AjaxTreeNodeJsonObject(xChiave, AgroPrefix_Impresa & xRag_Soc, "jstree-no-checkboxes", "", "#", Livello2))
                    Else
                        Livello_Impresa.Add(New AjaxTreeNodeJsonObject(xChiave, AgroPrefix_Impresa & xRag_Soc, "", "", "#", Livello2))
                    End If

                Else
                    Livello_Impresa.Add(New AjaxTreeNodeJsonObject(xChiave, AgroPrefix_Impresa & xRag_Soc, "", "", "#", Livello2))
                End If


                Livello_Impresa(Livello_Impresa.Count - 1).icon = PathRoot + Albero.RitornaPathImg(TipoImpresa)
                Livello_Impresa(Livello_Impresa.Count - 1).state = "open"


                '----------------------
                'FINE - IMPRESA
                '----------------------
                If Livello >= 3 Then

                    '################################## 
                    '#####  ANALISI x impresa #########
                    '################################## 
                    If Flag_Analisi = True Then
                        CaricaAnalisi(Livello2, xChiave, PathRoot, data_Inizio, data_Fine)
                    End If
                    '----------------------
                    'FINE - ANALISI x impresa
                    '----------------------




                    '################################## 
                    '#####  PLANNING Testata  #########
                    '################################## 
                    If _Flag_Planning = True Then
                        CaricaPlanningTestata(Livello2, xChiave, PathRoot, data_Inizio, data_Fine)
                    End If



                    '################################## 
                    '#####  CONTATTI  #################
                    '################################## 
                    If (_Flag_Contatti = True) Then
                        Dim xChiaveContatti As String = ""
                        Call Albero.ChiaveAlbero_Codifica_x_json(xChiaveContatti,
                                                    enum_TipoNodo.x_Contatti,
                                                    xPiva,
                                                    , , , , , , , , , , , , )
                        If Flag_CheckBox Then
                            If Not CheckBoxes.Flag_CheckBoxContatti Then

                                Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveContatti, "Contatti", "jstree-no-checkboxes", "", "#", False))
                            Else

                                Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveContatti, "Contatti", "", "", "#", False))
                            End If

                        Else

                            Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveContatti, "Contatti", "", "", "#", False))

                        End If

                        Livello2(Livello2.Count - 1).icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.x_Contatti)
                    End If
                    '----------------------
                    'FINE - CONTATTI
                    '----------------------





                    '##################################
                    '#####  PARCO MACCHINE  ###########
                    '##################################
                    If (_Flag_ParcoMacchine = True) Then
                        Dim xChiaveParcoMacchine As String = ""
                        Call Albero.ChiaveAlbero_Codifica_x_json(xChiaveParcoMacchine,
                                                    enum_TipoNodo.x_ParcoMacchine,
                                                    xPiva,
                                                    , , , , , , , , , , , , )

                        If Flag_CheckBox Then

                            If Not CheckBoxes.Flag_CheckBoxParcoMacchine Then

                                Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveParcoMacchine, "Parco Macchine", "jstree-no-checkboxes", "", "#", False))
                            Else

                                Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveParcoMacchine, "Parco Macchine", "", "", "#", False))
                            End If

                        Else

                            Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveParcoMacchine, "Parco Macchine", "", "", "#", False))

                        End If


                        Livello2(Livello2.Count - 1).icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.x_ParcoMacchine)
                    End If
                    '---------------------- 
                    'FINE - PARCO MACCHINE 
                    '---------------------- 





                    '####################
                    '#####  CENTRI  #####
                    '####################
                    Dim DT_Centri As New DataTable
                    Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    Dim str_aggiuntivo_x_specie As String
                    If Not IsNothing(HttpContext.Current.Session("Sementi")) AndAlso (HttpContext.Current.Session("Sementi") <> "-1") Then
                        Dim v = HttpContext.Current.Session("Sementi").split("|")(4)
                        str_aggiuntivo_x_specie = "  ( exists(select * from reg_impianti " &
                                " where cul_cod in( select cul_cod from cultivar  " &
                                " where(reg_impianti.piva = Centri_Aziendali.PIVA And reg_impianti.SA_COD = Centri_Aziendali.SA_COD  ) " &
                                " and veg_cod in (SELECT     distinct   Mappatura_Specie.Veg_Cod FROM            Sementieri_Sportello_ConfigurazioneXmappatura_specie INNER JOIN Mappatura_Specie ON Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Specie = Mappatura_Specie.ID_Specie AND  Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_SottoSpecie = Mappatura_Specie.ID_SottoSpecie AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Gruppo = Mappatura_Specie.ID_Gruppo AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Genotipo = Mappatura_Specie.ID_Genotipo " &
                                " WHERE Sementieri_Sportello_ConfigurazioneXmappatura_specie.Sementieri_Sportello_Configurazione_cod = " & v &
                                " ))) " &
                                " OR " &
                                "   exists(select * from programmazione_entita " &
                                " where cul_cod in( select cul_cod from cultivar  " &
                                " where(programmazione_entita.piva = Centri_Aziendali.PIVA And programmazione_entita.SA_COD = Centri_Aziendali.SA_COD  ) " &
                                " and veg_cod in (SELECT     distinct   Mappatura_Specie.Veg_Cod FROM            Sementieri_Sportello_ConfigurazioneXmappatura_specie INNER JOIN Mappatura_Specie ON Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Specie = Mappatura_Specie.ID_Specie AND  Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_SottoSpecie = Mappatura_Specie.ID_SottoSpecie AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Gruppo = Mappatura_Specie.ID_Gruppo AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Genotipo = Mappatura_Specie.ID_Genotipo " &
                                " WHERE Sementieri_Sportello_ConfigurazioneXmappatura_specie.Sementieri_Sportello_Configurazione_cod = " & v &
                                " ))) " &
                                " ) "
                    End If

                    DT_Centri = objCentri.Leggi(CStr(_Piva),
                                                _Sa_Cod,
                                                 enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                  str_aggiuntivo_x_specie, "", HttpContext.Current.Session("ASG_objParametri_Server"))

                    Dim i As Integer
                    For i = 0 To DT_Centri.Rows.Count - 1
                        xPiva = DT_Centri(i)("Piva")
                        xSa_Cod = DT_Centri(i)("Sa_Cod")
                        xSa_Nome = DT_Centri(i)("Sa_Nome")
                        xValidita_Fine = DT_Centri(i)("Validita_Fine")
                        ValidazioneNodo = DT_Centri(i)("Validazione")

                        Dim DescrizioneNodo As String = xSa_Nome
                        If ValidazioneNodo = "-1" Then
                            DescrizioneNodo = DescrizioneNodo & " ..... (§§§ da confermare §§§)"
                        End If


                        Dim xChiaveCentro As String = ""
                        Call Albero.ChiaveAlbero_Codifica_x_json(xChiaveCentro,
                                                    enum_TipoNodo.Centro,
                                                    Piva:=xPiva,
                                                    Sa_Cod:=xSa_Cod,
                                                    PivaPadre:=_PivaPadre
                                                    )



                        'inserisco il nodo ed eventualmente i sui figli
                        Dim child As Object
                        If Flag_Esplodi_Tutto = True Then
                            child = New List(Of AjaxTreeNodeJsonObject)
                            CaricaInfoDentroACentro(child, xChiaveCentro, PathRoot, data_Inizio, data_Fine)
                        Else
                            child = New Boolean
                            child = True
                        End If

                        If Flag_CheckBox Then
                            If Not CheckBoxes.Flag_CheckBoxCentro Then
                                Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveCentro, DescrizioneNodo, "jstree-no-checkboxes", "", "#", child))
                            Else
                                Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveCentro, DescrizioneNodo, "", "", "#", child))
                            End If
                        Else
                            Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveCentro, DescrizioneNodo, "", "", "#", child))
                        End If

                        If Flag_Esplodi_Tutto = True Then
                            Livello2(Livello2.Count - 1).state = "open"
                        End If
                        Livello2(Livello2.Count - 1).icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Centro)
                        'nodo aggiunto

                    Next
                End If


            End If

            results.Add(Radice)
        End If
    End Sub


    ''' <summary>
    ''' funzione per precaricare il nodo delle analisi
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PrecaricaNodoAnalisi()
        Dim DT As DataTable
        Dim objAnalisi As New AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_R
        ' carico solo le concimazioni del terreno e dei residui

        DT = objAnalisi.LeggixPrecaricaAlbero(Me.Piva, "(Analisi_Testata_Tipo=1 OR Analisi_Testata_Tipo=7)", "", HttpContext.Current.Session("ASG_objParametri_Server"))

        HttpContext.Current.Session("DT_Analisi") = DT
    End Sub

    '''' <summary>
    '''' funzione per precaricare il nodo dei piani concimazione
    '''' </summary>
    '''' <remarks></remarks>
    'Private Sub PrecaricaNodoPianoConcimazione()
    '    Dim DT As DataTable
    '    Dim objPianoConcimazione As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R
    '    ' carico solo i Piani Concimazione del terreno e dei residui
    '    DT = objPianoConcimazione.Leggi(, , Piva, , , , impia, )
    '    'DT = objPianoConcimazione.LeggixPrecaricaAlbero(Me.Piva, "(PianoConcimazione_Testata_Tipo=1 OR PianoConcimazione_Testata_Tipo=7)", "", HttpContext.Current.Session("ASG_objParametri_Server"))
    '    HttpContext.Current.Session("DT_PianoConcimazione") = DT
    'End Sub


    ''' <summary>
    ''' funzione per precaricare il nodo dei campioni
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PrecaricaNodoCampioni()
        Dim DT As DataTable
        Dim objcampioni As New AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_R
        DT = objcampioni.LeggixPrecaricaAlbero(Me.Piva, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))
        HttpContext.Current.Session("DT_Campioni") = DT
    End Sub



    ''' <summary>
    ''' Carica il nodo Utente, Impresa, Contatti, Parco Macchine e la lista dei Centri Aziendali
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaPrimi3Livelli(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                  ByVal Id As String,
                                                  ByVal PathRoot As String,
                                                  ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)

        Dim xChiave As String = ""
        Dim Testo As String = ""

        Dim Livello_Impresa As New List(Of AjaxTreeNodeJsonObject)

        '************************
        '***** NODO UTENTE ******
        '************************
        Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim DTUtente As DataTable

        DTUtente = objUtente.Leggi(HttpContext.Current.Session("ASG_Utente_Username"),
                                   5,
                                   enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                   "", "", HttpContext.Current.Session("ASG_objParametri_Utenti"))
        Dim TipoUtente As Integer

        'Prelevo la Ragione Sociale oppure Nome e Cognome
        If DTUtente.Rows.Count > 0 Then
            'Verifico il tipo di utente ... Azienda/Persona
            TipoUtente = DTUtente.Rows(0).Item("Flag_Azienda_Persona")
            If TipoUtente = 1 Then
                Testo = DTUtente.Rows(0).Item("Rag_Soc")
            Else
                Testo = DTUtente.Rows(0).Item("Cognome") & " " &
                        DTUtente.Rows(0).Item("Nome")
            End If
        Else
            Testo = HttpContext.Current.Session("ASG_Utente_Username")
        End If
        'Genero la chiave
        Call Albero.ChiaveAlbero_Codifica_x_json(xChiave,
                                    enum_TipoNodo.Utente,
                                    , , , , , , , , , , , , , )

        Dim Radice As AjaxTreeNodeJsonObject

        If Flag_CheckBox Then
            If Not CheckBoxes.Flag_CheckBoxUtente Then
                Radice = New AjaxTreeNodeJsonObject(xChiave, AgroPrefix_Utente & Testo, "jstree-no-checkboxes PIPPO", "", "#", Livello_Impresa)
            Else
                Radice = New AjaxTreeNodeJsonObject(xChiave, AgroPrefix_Utente & Testo, "", "", "#", Livello_Impresa)
            End If
        Else
            Radice = New AjaxTreeNodeJsonObject(xChiave, AgroPrefix_Utente & Testo, "", "", "#", Livello_Impresa)
        End If

        Radice.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Utente, "")
        Radice.state = "open"

        '======================================================
        '===== Verifica l'uscita dalla routine
        '======================================================
        'Se la partita IVA e' nulla allora esco
        If _Piva = "" Then
            Return
        End If



        '#####################
        '#####  IMPRESA  #####
        '#####################

        'Creo gli oggetti COM
        Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim DTImprese As DataTable
        Dim ValiditaFineImpresa As Date
        Dim TipoImpresaGerarchia As Integer

        Dim xPiva As String = ""
        Dim xSa_Cod As String = ""
        Dim xRag_Soc As String = ""
        Dim xSa_Nome As String = ""
        Dim xValidita_Fine As String = ""
        Dim ValidazioneNodo As String = ""
        Dim CertificatiBloccati As String = ""

        'Leggo le informazioni sull'impresa selezionata			
        DTImprese = objImprese.Leggi(CStr(_Piva),
                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "", "", HttpContext.Current.Session("ASG_objParametri_Server"))
        objImprese = Nothing

        If DTImprese.Rows.Count > 0 Then
            xPiva = DTImprese.Rows(0).Item("Piva")
            xRag_Soc = DTImprese.Rows(0).Item("Rag_Soc")
            ValidazioneNodo = DTImprese.Rows(0).Item("Validazione") & ""
            CertificatiBloccati = DTImprese.Rows(0).Item("Blk_Flag") & ""
            ValiditaFineImpresa = CDate(DTImprese.Rows(0).Item("validita_fine"))
            TipoImpresaGerarchia = DTImprese.Rows(0).Item("TipoImpresaGerarchia")
        Else
            ValiditaFineImpresa = #12/31/2100#
            xPiva = "?????"
            xRag_Soc = "?????"
            ValidazioneNodo = "-1"
            TipoImpresaGerarchia = 1
        End If

        '----- Creo il nodo IMPRESA sul albero
        If CertificatiBloccati = "-1" Then
            xRag_Soc = "--SOSPESA-- " & xRag_Soc
        End If

        'Genero una chiave
        Call Albero.ChiaveAlbero_Codifica_x_json(xChiave,
                                    enum_TipoNodo.Impresa,
                                    Piva:=xPiva,
                                    PivaPadre:=_PivaPadre
                                )

        Dim TipoImpresa As enum_TipoNodo

        Select Case TipoImpresaGerarchia
            Case 1
                TipoImpresa = enum_TipoNodo.Impresa
            Case 2
                TipoImpresa = enum_TipoNodo.x_Cooperativa
            Case 3
                TipoImpresa = enum_TipoNodo.x_Consorzio
            Case 4
                TipoImpresa = enum_TipoNodo.x_OP
        End Select


        Dim Livello2 As New List(Of AjaxTreeNodeJsonObject)
        'Livello_Impresa.Add(New AjaxTreeNodeJsonObject(xChiave, AgroPrefix_Impresa & xRag_Soc, "#", Livello2))

        If Flag_CheckBox Then

            If Not CheckBoxes.Flag_CheckBoxImpresa Then
                Livello_Impresa.Add(New AjaxTreeNodeJsonObject(xChiave, AgroPrefix_Impresa & xRag_Soc, "jstree-no-checkboxes", "", "#", Livello2))
            Else
                Livello_Impresa.Add(New AjaxTreeNodeJsonObject(xChiave, AgroPrefix_Impresa & xRag_Soc, "", "", "#", Livello2))
            End If

        Else
            Livello_Impresa.Add(New AjaxTreeNodeJsonObject(xChiave, AgroPrefix_Impresa & xRag_Soc, "", "", "#", Livello2))
        End If


        Livello_Impresa(Livello_Impresa.Count - 1).icon = PathRoot + Albero.RitornaPathImg(TipoImpresa)
        Livello_Impresa(Livello_Impresa.Count - 1).state = "open"


        '----------------------
        'FINE - IMPRESA
        '----------------------

        '################################## 
        '#####  ANALISI x impresa #########
        '################################## 
        If Flag_Analisi = True Then
            CaricaAnalisi(Livello2, xChiave, PathRoot, data_Inizio, data_Fine)
        End If
        '----------------------
        'FINE - ANALISI x impresa
        '----------------------




        '################################## 
        '#####  PLANNING Testata  #########
        '################################## 
        If _Flag_Planning = True Then
            CaricaPlanningTestata(Livello2, xChiave, PathRoot, data_Inizio, data_Fine)
        End If



        '################################## 
        '#####  CONTATTI  #################
        '################################## 
        If (_Flag_Contatti = True) Then
            Dim xChiaveContatti As String = ""
            Call Albero.ChiaveAlbero_Codifica_x_json(xChiaveContatti,
                                        enum_TipoNodo.x_Contatti,
                                        xPiva,
                                        , , , , , , , , , , , , )
            If Flag_CheckBox Then
                If Not CheckBoxes.Flag_CheckBoxContatti Then

                    Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveContatti, "Contatti", "jstree-no-checkboxes", "", "#", False))
                Else

                    Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveContatti, "Contatti", "", "", "#", False))
                End If

            Else

                Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveContatti, "Contatti", "", "", "#", False))

            End If

            Livello2(Livello2.Count - 1).icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.x_Contatti)
        End If
        '----------------------
        'FINE - CONTATTI
        '----------------------





        '##################################
        '#####  PARCO MACCHINE  ###########
        '##################################
        If (_Flag_ParcoMacchine = True) Then
            Dim xChiaveParcoMacchine As String = ""
            Call Albero.ChiaveAlbero_Codifica_x_json(xChiaveParcoMacchine,
                                        enum_TipoNodo.x_ParcoMacchine,
                                        xPiva,
                                        , , , , , , , , , , , , )

            If Flag_CheckBox Then

                If Not CheckBoxes.Flag_CheckBoxParcoMacchine Then

                    Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveParcoMacchine, "Parco Macchine", "jstree-no-checkboxes", "", "#", False))
                Else

                    Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveParcoMacchine, "Parco Macchine", "", "", "#", False))
                End If

            Else

                Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveParcoMacchine, "Parco Macchine", "", "", "#", False))

            End If


            Livello2(Livello2.Count - 1).icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.x_ParcoMacchine)
        End If
        '---------------------- 
        'FINE - PARCO MACCHINE 
        '---------------------- 





        '####################
        '#####  CENTRI  #####
        '####################
        Dim DT_Centri As New DataTable
        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim str_aggiuntivo_x_specie As String
        If Not IsNothing(HttpContext.Current.Session("Sementi")) AndAlso (HttpContext.Current.Session("Sementi") <> "-1") Then
            Dim v = HttpContext.Current.Session("Sementi").split("|")(4)
            str_aggiuntivo_x_specie = "  ( exists(select * from reg_impianti " &
                    " where cul_cod in( select cul_cod from cultivar  " &
                    " where(reg_impianti.piva = Centri_Aziendali.PIVA And reg_impianti.SA_COD = Centri_Aziendali.SA_COD  ) " &
                    " and veg_cod in (SELECT     distinct   Mappatura_Specie.Veg_Cod FROM            Sementieri_Sportello_ConfigurazioneXmappatura_specie INNER JOIN Mappatura_Specie ON Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Specie = Mappatura_Specie.ID_Specie AND  Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_SottoSpecie = Mappatura_Specie.ID_SottoSpecie AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Gruppo = Mappatura_Specie.ID_Gruppo AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Genotipo = Mappatura_Specie.ID_Genotipo " &
                    " WHERE Sementieri_Sportello_ConfigurazioneXmappatura_specie.Sementieri_Sportello_Configurazione_cod = " & v &
                    " ))) " &
                    " OR " &
                    "   exists(select * from programmazione_entita " &
                    " where cul_cod in( select cul_cod from cultivar  " &
                    " where(programmazione_entita.piva = Centri_Aziendali.PIVA And programmazione_entita.SA_COD = Centri_Aziendali.SA_COD  ) " &
                    " and veg_cod in (SELECT     distinct   Mappatura_Specie.Veg_Cod FROM            Sementieri_Sportello_ConfigurazioneXmappatura_specie INNER JOIN Mappatura_Specie ON Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Specie = Mappatura_Specie.ID_Specie AND  Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_SottoSpecie = Mappatura_Specie.ID_SottoSpecie AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Gruppo = Mappatura_Specie.ID_Gruppo AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Genotipo = Mappatura_Specie.ID_Genotipo " &
                    " WHERE Sementieri_Sportello_ConfigurazioneXmappatura_specie.Sementieri_Sportello_Configurazione_cod = " & v &
                    " ))) " &
                    " ) "
        End If

        DT_Centri = objCentri.Leggi(CStr(_Piva),
                                    _Sa_Cod,
                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                      str_aggiuntivo_x_specie, "", HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim i As Integer
        For i = 0 To DT_Centri.Rows.Count - 1
            xPiva = DT_Centri(i)("Piva")
            xSa_Cod = DT_Centri(i)("Sa_Cod")
            xSa_Nome = DT_Centri(i)("Sa_Nome")
            xValidita_Fine = DT_Centri(i)("Validita_Fine")
            ValidazioneNodo = DT_Centri(i)("Validazione")

            Dim DescrizioneNodo As String = xSa_Nome
            If ValidazioneNodo = "-1" Then
                DescrizioneNodo = DescrizioneNodo & " ..... (§§§ da confermare §§§)"
            End If


            Dim xChiaveCentro As String = ""
            Call Albero.ChiaveAlbero_Codifica_x_json(xChiaveCentro,
                                        enum_TipoNodo.Centro,
                                        Piva:=xPiva,
                                        Sa_Cod:=xSa_Cod,
                                        PivaPadre:=_PivaPadre
                                        )



            'inserisco il nodo ed eventualmente i sui figli
            Dim child As Object
            If Flag_Esplodi_Tutto = True Then
                child = New List(Of AjaxTreeNodeJsonObject)
                CaricaInfoDentroACentro(child, xChiaveCentro, PathRoot, data_Inizio, data_Fine)
            Else
                child = New Boolean
                child = True
            End If

            If Flag_CheckBox Then
                If Not CheckBoxes.Flag_CheckBoxCentro Then
                    Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveCentro, DescrizioneNodo, "jstree-no-checkboxes", "", "#", child))
                Else
                    Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveCentro, DescrizioneNodo, "", "", "#", child))
                End If
            Else
                Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveCentro, DescrizioneNodo, "", "", "#", child))
            End If

            If Flag_Esplodi_Tutto = True Then
                Livello2(Livello2.Count - 1).state = "open"
            End If
            Livello2(Livello2.Count - 1).icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Centro)
            'nodo aggiunto

        Next



        results.Add(Radice)
    End Sub





    ''' <summary>
    ''' Carica un livello dentro a centro aziendale (Nodo Catasto, Fabbricati, Campi e appezzamenti)
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaInfoDentroACentro(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                  ByVal Id As String,
                                                  ByVal PathRoot As String,
                                                  ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)

        If Flag_Analisi = True Then
            CaricaAnalisi(results, Id, PathRoot, data_Inizio, data_Fine)
        End If
        'per caricare il catasto
        If Flag_CatastoAziendale = True Then
            CaricaCatasto(results, Id, PathRoot, data_Inizio, data_Fine)
        End If

        If Flag_Planning = True Then
            CaricaPlanningTestata(results, Id, PathRoot, data_Inizio, data_Fine)
        End If

        '################################## 
        '#####  Operazioni Agenda #########
        '################################## 
        If Flag_Agenda = True Then
            CaricaOperazioniAgenda(results, Id, PathRoot, data_Inizio, data_Fine)
        End If
        '----------------------
        'FINE - Operazioni Agenda
        '----------------------

        'portafoglio prodotti
        If Flag_PortafoglioProdotti = True Then
            CaricaPortafoglioProdotti(results, Id, PathRoot, data_Inizio, data_Fine)
        End If
        'fabbricati
        If Flag_Fabbricati = True Then
            CaricaNodoFabbricati(results, Id, PathRoot, data_Inizio, data_Fine)
        End If



        If Flag_Anagrafica = True Then


            Dim listaAnag As New List(Of AjaxTreeNodeJsonObject)
            'Carico i Campi
            CaricaNodiCampi(listaAnag, Id, PathRoot, data_Inizio, data_Fine)

            'appezzamenti sfusi
            Dim xSementiero As String = ""
            If Not HttpContext.Current.Session("sementi") Is Nothing AndAlso
                HttpContext.Current.Session("sementi") <> "-1" Then
                xSementiero = HttpContext.Current.Session("Codice_Fiscale_Tecnico")
            End If


            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

            Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

            'Valorizzo il Codice_Fiscale_Tecnico prendendolo dalla tabella Gruppi_Utente
            Dim Codice_Fiscale_Tecnico As String = ""
            Dim xUtenteCorrente As String = CType(objParametri_Server, AgronicaCoreDataProvider.AgronicaCoreParametri).UtenteUsername
            Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
            Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(xUtenteCorrente, objParametri_Utenti)

            CaricaAppezzamenti(listaAnag, Id, PathRoot, 0, Codice_Fiscale_Tecnico, data_Inizio, data_Fine, xSementiero)

            If listaAnag.Count > 0 Then

                Dim opAnagrafica As AjaxTreeNodeJsonObject

                Dim DescrizioneNodo As String = "Anagrafica"
                If Flag_CheckBox Then
                    If Not CheckBoxes.Flag_CheckBoxCentro Then
                        opAnagrafica = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "jstree-no-checkboxes", "", "#", listaAnag)
                    Else
                        opAnagrafica = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "", "", "#", listaAnag)
                    End If
                Else
                    opAnagrafica = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "", "", "#", listaAnag)
                End If
                If Flag_Esplodi_Tutto = True Then
                    opAnagrafica.state = "open"
                End If

                opAnagrafica.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Anagrafica_Generica, "")
                results.Add(opAnagrafica)
            End If

        End If


    End Sub

#Region "PIANOCONCIMAZIONI"
    ''' <summary>
    ''' Per il caricamento delle Particelle
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaPianoConcimazioni(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)



        'costruisco la stringa di filtro
        Dim strFiltro As String = ""
        Dim xPiva As String = ""
        Dim xProv As String = ""
        Dim xCom As String = ""
        Dim xSezione As String = ""
        Dim xSubalterno As String = ""
        Dim xID_oggetto_Grafico As String = ""
        Dim xCodFisc As String = ""
        Dim xEntita_Cod, xSa_Cod, xCampo_Cod, xAppezza, xId_Reg, xFabbricato_Cod, xFoglio, xNumero, xProgetto_Cod As Integer

        Albero.ChiaveAlbero_Decodifica_TipoNodo_x_json(Id, xEntita_Cod)


        Select Case xEntita_Cod
            Case enum_TipoNodo.Appezzamento
                xEntita_Cod = enum_EntitaAlberoImprese.Appezzamento

            Case enum_TipoNodo.Campo
                xEntita_Cod = enum_EntitaAlberoImprese.Campo

            Case enum_TipoNodo.CatastoAziendale
                xEntita_Cod = enum_EntitaAlberoImprese.Catasto

            Case enum_TipoNodo.Centro
                xEntita_Cod = enum_EntitaAlberoImprese.Centro

            Case enum_TipoNodo.Fabbricato_Generico
                xEntita_Cod = enum_EntitaAlberoImprese.Fabbricato

            Case enum_TipoNodo.Impianto_Generico
                xEntita_Cod = enum_EntitaAlberoImprese.Impianto
            Case enum_TipoNodo.ImpiantoArborea
                xEntita_Cod = enum_EntitaAlberoImprese.Impianto
            Case enum_TipoNodo.ImpiantoErbacea
                xEntita_Cod = enum_EntitaAlberoImprese.Impianto
            Case enum_TipoNodo.ImpiantoNudo
                xEntita_Cod = enum_EntitaAlberoImprese.Impianto
            Case enum_TipoNodo.ImpiantoOrticola
                xEntita_Cod = enum_EntitaAlberoImprese.Impianto

            Case enum_TipoNodo.Impresa
                xEntita_Cod = enum_EntitaAlberoImprese.Impresa
            Case enum_TipoNodo.x_Consorzio
                xEntita_Cod = enum_EntitaAlberoImprese.Impresa
            Case enum_TipoNodo.x_Cooperativa
                xEntita_Cod = enum_EntitaAlberoImprese.Impresa
            Case enum_TipoNodo.x_OP
                xEntita_Cod = enum_EntitaAlberoImprese.Impresa


            Case enum_TipoNodo.Particella
                xEntita_Cod = enum_EntitaAlberoImprese.Particella

            Case enum_TipoNodo.DistintaDiProduzione
                xEntita_Cod = enum_EntitaAlberoImprese.Distinta


            Case enum_TipoNodo.x_MovimentiMagazzino,
                     enum_TipoNodo.f_Abitazione,
                     enum_TipoNodo.f_CellaFrigorifera,
                     enum_TipoNodo.f_Magazzino,
                     enum_TipoNodo.f_Silos,
                     enum_TipoNodo.f_ImpiantoLavorazione,
                     enum_TipoNodo.f_Stalla
                xEntita_Cod = enum_EntitaAlberoImprese.Fabbricato


        End Select






        Albero.ChiaveAlbero_Decodifica_x_json(Id, xPiva, xSa_Cod, xCampo_Cod, xAppezza, xId_Reg, Nothing, Nothing, Nothing, Nothing, Nothing,
                                       xNumero, Nothing, xCodFisc, xFabbricato_Cod, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, xProgetto_Cod, Nothing)


        Dim objPianoConcimazione As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R
        ' carico solo i Piani Concimazione del terreno e dei residui

        Dim DT_PianoConcimazioni As DataTable
        DT_PianoConcimazioni = objPianoConcimazione.Leggi(0, xEntita_Cod, xPiva, xSa_Cod, xCampo_Cod, xAppezza, xId_Reg, xFabbricato_Cod, xProv, xCom, xSezione, xFoglio, xNumero, xSubalterno, xProgetto_Cod, xID_oggetto_Grafico, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim DescrizioneNodo As String
        Dim i As Integer

        If Not DT_PianoConcimazioni Is Nothing Then

            For i = 0 To (DT_PianoConcimazioni.Rows.Count - 1)
                Dim xChiaveAlbero As String = ""

                Call Albero.ChiaveAlbero_Codifica_x_json(
                                    xChiaveAlbero,
                                    enum_TipoNodo.PianoConcimazione_Testata,
                                    xPiva,
                                    DT_PianoConcimazioni.Rows(i).Item("sa_cod"),
                                    DT_PianoConcimazioni.Rows(i).Item("Campo_cod"),
                                    DT_PianoConcimazioni.Rows(i).Item("Appezza"),
                                    DT_PianoConcimazioni.Rows(i).Item("ID_Imp"),
                                    , DT_PianoConcimazioni.Rows(i).Item("Prov"),
                                    DT_PianoConcimazioni.Rows(i).Item("Com"),
                                    DT_PianoConcimazioni.Rows(i).Item("Sezione"),
                                    DT_PianoConcimazioni.Rows(i).Item("Foglio"),
                                    DT_PianoConcimazioni.Rows(i).Item("Numero"),
                                    DT_PianoConcimazioni.Rows(i).Item("Subalterno"),
                                    , DT_PianoConcimazioni.Rows(i).Item("Fabbricato_cod"),
                                    , , ,
                                    , , ,
                                    DT_PianoConcimazioni.Rows(i).Item("PC_Testata_Cod"),
                                    DT_PianoConcimazioni.Rows(i).Item("Progetto_Cod"))

                DescrizioneNodo = "{PianoConcimazioni} " & DT_PianoConcimazioni.Rows(i).Item("PC_Testata_Des")



                'aggiungo effettibvamente il nodo dell'analisi
                Dim PianoConcimazioni As AjaxTreeNodeJsonObject


                PianoConcimazioni = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                               DescrizioneNodo, "", "", "#", False)

                PianoConcimazioni.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.PianoConcimazione_Testata, "")

                If Flag_Esplodi_Tutto = True Then
                    PianoConcimazioni.state = "open"
                End If
                results.Add(PianoConcimazioni)

            Next

        End If


    End Sub

    'Private Sub FiltroSelectsuDt(ByRef DT_Analisi As DataTable, _
    '                                         ByRef DR_Analisi As DataRow(), _
    '                                         ByVal strFiltro As String)

    '    If Not DT_Analisi Is Nothing Then

    '        DR_Analisi = DT_Analisi.Select(strFiltro)
    '    End If

    'End Sub


#End Region

#Region "ESERCIZI"
    ''' <summary>
    ''' Per il caricamento degli esercizi
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaEsercizio(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)



        'costruisco la stringa di filtro
        Dim strFiltro As String = ""
        Dim xPiva As String = ""
        Dim xProv As String = ""
        Dim xCom As String = ""
        Dim xSezione As String = ""
        Dim xSubalterno As String = ""
        Dim xID_oggetto_Grafico As String = ""
        Dim xCodFisc As String = ""
        Dim xSa_Cod, xCampo_Cod, xAppezza, xId_Reg, xFabbricato_Cod, xFoglio, xNumero, xProgetto_Cod As Integer




        Albero.ChiaveAlbero_Decodifica_x_json(Id, xPiva, xSa_Cod, xCampo_Cod, xAppezza, xId_Reg, Nothing, xProv, xCom, xSezione, xFoglio,
                                       xNumero, xSubalterno, xCodFisc, xFabbricato_Cod, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, xProgetto_Cod, Nothing)


        Dim objEsercizio As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        ' carico solo i Piani Concimazione del terreno e dei residui

        Dim DT_Esercizio As DataTable
        'DT_Esercizio = HttpContext.Current.Session("DT_Esercizio")
        DT_Esercizio = objEsercizio.Leggi(xPiva, xProgetto_Cod, "", 0, xSa_Cod, xAppezza, xId_Reg, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                           "", "", HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim DescrizioneNodo As String
        Dim i As Integer

        If Not DT_Esercizio Is Nothing Then

            For i = 0 To (DT_Esercizio.Rows.Count - 1)
                Dim xChiaveAlbero As String = ""

                Call Albero.ChiaveAlbero_Codifica_x_json(
                                    xChiaveAlbero,
                                    enum_TipoNodo.DistintaDiProduzione,
                                    xPiva,
                                    DT_Esercizio.Rows(i).Item("sa_cod"),
                                    xCampo_Cod,
                                    DT_Esercizio.Rows(i).Item("Appezza"),
                                    DT_Esercizio.Rows(i).Item("Id_Reg"),
                                    , xProv,
                                    xCom,
                                    xSezione,
                                    xFoglio,
                                    xNumero,
                                    xSubalterno,
                                    xCodFisc, xFabbricato_Cod,
                                    ,
                                    , , ,
                                    , , ,
                                    DT_Esercizio.Rows(i).Item("Progetto_Cod"))

                DescrizioneNodo = "{Esercizio/Annualita} " & DT_Esercizio.Rows(i).Item("Progetto_Des") & " - " & CDate(DT_Esercizio.Rows(i).Item("Validita_Inizio")).ToShortDateString


                'aggiungo effettibvamente il nodo dell'esercizio
                Dim Esercizio As AjaxTreeNodeJsonObject

                If Flag_PianoConcimazione = True Then

                    Dim Livello_PianoConcimazioni As New List(Of AjaxTreeNodeJsonObject)

                    CaricaPianoConcimazioni(Livello_PianoConcimazioni, xChiaveAlbero, PathRoot, data_Inizio, data_Fine)
                    If Livello_PianoConcimazioni.Count > 0 Then

                        If Flag_CheckBox Then

                            If Not CheckBoxes.Flag_CheckBoxImpianto Then
                                Esercizio = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                                              DescrizioneNodo, "jstree-no-checkboxes", "", "#", Livello_PianoConcimazioni)
                            Else
                                Esercizio = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                                              DescrizioneNodo, "", "", "#", Livello_PianoConcimazioni)
                            End If

                        Else
                            Esercizio = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                                              DescrizioneNodo, "", "", "#", Livello_PianoConcimazioni)
                        End If

                    Else

                        If Flag_CheckBox Then

                            If Not CheckBoxes.Flag_CheckBoxImpianto Then

                                Esercizio = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                                              DescrizioneNodo, "jstree-no-checkboxes", "", "#", False)
                            Else

                                Esercizio = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                                              DescrizioneNodo, "", "", "#", False)
                            End If

                        Else
                            Esercizio = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                                              DescrizioneNodo, "", "", "#", False)
                        End If

                    End If

                Else

                    'Esercizio = New AjaxTreeNodeJsonObject(xChiaveAlbero, _
                    '                               DescrizioneNodo, "", "", "#", False)


                    'TEST
                    If Flag_CheckBox Then

                        Esercizio = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                                          DescrizioneNodo, "", "", "#", False)

                    Else
                        Esercizio = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                                          DescrizioneNodo, "jstree-no-checkboxes", "", "#", False)
                    End If
                End If

                Esercizio.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.DistintaDiProduzione, "")
                If Flag_Esplodi_Tutto = True Then
                    Esercizio.state = "open"
                End If
                results.Add(Esercizio)

            Next

        End If


    End Sub

    'Private Sub FiltroSelectsuDt(ByRef DT_Analisi As DataTable, _
    '                                         ByRef DR_Analisi As DataRow(), _
    '                                         ByVal strFiltro As String)

    '    If Not DT_Analisi Is Nothing Then

    '        DR_Analisi = DT_Analisi.Select(strFiltro)
    '    End If

    'End Sub


#End Region

#Region "ANALISI"
    ''' <summary>
    ''' Per il caricamento delle Particelle
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaAnalisi(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)



        'costruisco la stringa di filtro
        Dim strFiltro As String = ""
        Dim xPiva As String = ""
        Dim xProv As String = ""
        Dim xCom As String = ""
        Dim xSezione As String = ""
        Dim xSubalterno As String = ""
        Dim xID_oggetto_Grafico As String = ""
        Dim xCodFisc As String = ""
        Dim xEntita_Cod, xSa_Cod, xCampo_Cod, xAppezza, xId_Reg, xFabbricato_Cod, xFoglio, xNumero, xVas_cod As Integer

        Albero.ChiaveAlbero_Decodifica_TipoNodo_x_json(Id, xEntita_Cod)

        Select Case xEntita_Cod
            Case enum_TipoNodo.Appezzamento
                xEntita_Cod = enum_EntitaAlberoImprese.Appezzamento

            Case enum_TipoNodo.Campo
                xEntita_Cod = enum_EntitaAlberoImprese.Campo

            Case enum_TipoNodo.CatastoAziendale
                xEntita_Cod = enum_EntitaAlberoImprese.Catasto

            Case enum_TipoNodo.Centro
                xEntita_Cod = enum_EntitaAlberoImprese.Centro

            Case enum_TipoNodo.Fabbricato_Generico
                xEntita_Cod = enum_EntitaAlberoImprese.Fabbricato

            Case enum_TipoNodo.Impianto_Generico
                xEntita_Cod = enum_EntitaAlberoImprese.Impianto
            Case enum_TipoNodo.ImpiantoArborea
                xEntita_Cod = enum_EntitaAlberoImprese.Impianto
            Case enum_TipoNodo.ImpiantoErbacea
                xEntita_Cod = enum_EntitaAlberoImprese.Impianto
            Case enum_TipoNodo.ImpiantoNudo
                xEntita_Cod = enum_EntitaAlberoImprese.Impianto
            Case enum_TipoNodo.ImpiantoOrticola
                xEntita_Cod = enum_EntitaAlberoImprese.Impianto

            Case enum_TipoNodo.Impresa
                xEntita_Cod = enum_EntitaAlberoImprese.Impresa
            Case enum_TipoNodo.x_Consorzio
                xEntita_Cod = enum_EntitaAlberoImprese.Impresa
            Case enum_TipoNodo.x_Cooperativa
                xEntita_Cod = enum_EntitaAlberoImprese.Impresa
            Case enum_TipoNodo.x_OP
                xEntita_Cod = enum_EntitaAlberoImprese.Impresa


            Case enum_TipoNodo.Particella
                xEntita_Cod = enum_EntitaAlberoImprese.Particella


            Case enum_TipoNodo.x_MovimentiMagazzino,
                     enum_TipoNodo.f_Abitazione,
                     enum_TipoNodo.f_CellaFrigorifera,
                     enum_TipoNodo.f_Magazzino,
                     enum_TipoNodo.f_Silos,
                     enum_TipoNodo.f_ImpiantoLavorazione,
                     enum_TipoNodo.f_Stalla
                xEntita_Cod = enum_EntitaAlberoImprese.Fabbricato


        End Select






        Albero.ChiaveAlbero_Decodifica_x_json(Id, xPiva, xSa_Cod, xCampo_Cod, xAppezza, xId_Reg, Nothing, xProv, xCom, xSezione, xFoglio,
                                       xNumero, xSubalterno, xCodFisc, xFabbricato_Cod, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, xVas_cod, Nothing)

        If xEntita_Cod <> 0 Then
            strFiltro += " Analisi_Entita_Cod = " & Agro_SQL_SaveNum(xEntita_Cod) & " AND  "
        End If
        If xPiva <> "" Then
            strFiltro += " Piva = '" & Agro_SQL_SaveText(xPiva) & "' AND "
        End If
        If xSa_Cod <> 0 Then
            strFiltro += " Sa_Cod = " & Agro_SQL_SaveNum(xSa_Cod) & " AND "
        End If
        If xCampo_Cod <> 0 Then
            strFiltro += " Campo_Cod = " & Agro_SQL_SaveNum(xCampo_Cod) & " AND "
        End If
        If xAppezza <> 0 Then
            strFiltro += " Appezza = " & Agro_SQL_SaveNum(xAppezza) & " AND "
        End If
        If xId_Reg <> 0 Then
            strFiltro += " Id_Imp = " & Agro_SQL_SaveNum(xId_Reg) & " AND "
        End If
        If xFabbricato_Cod <> 0 Then
            strFiltro += " Fabbricato_Cod = " & Agro_SQL_SaveNum(xFabbricato_Cod) & " AND "
        End If
        If xProv <> "" And xProv <> "0" Then
            strFiltro += " Prov = '" & Agro_SQL_SaveText(xProv) & "' AND "
        End If
        If xCom <> "" And xCom <> "0" Then
            strFiltro += " Com = '" & Agro_SQL_SaveText(xCom) & "' AND "
        End If
        If xSezione <> "" And xSezione <> "0" Then
            strFiltro += " Sezione = '" & Agro_SQL_SaveText(xSezione) & "' AND "
        End If
        If xFoglio <> 0 Then
            strFiltro += " Foglio = " & Agro_SQL_SaveNum(xFoglio) & " AND "
        End If
        If xNumero <> 0 Then
            strFiltro += " Numero = " & Agro_SQL_SaveNum(xNumero) & " AND "
        End If
        If xSubalterno <> "" And xSubalterno <> "0" Then
            strFiltro += " Subalterno = '" & Agro_SQL_SaveText(xSubalterno) & "' AND "
        End If
        If xID_oggetto_Grafico <> "0" And xID_oggetto_Grafico <> "" Then
            strFiltro += " ID_Oggetto_Grafico = '" & Agro_SQL_SaveText(xID_oggetto_Grafico) & "' AND "
        End If
        If xVas_cod <> 0 Then
            strFiltro += " Vas_Cod = " & Agro_SQL_SaveNum(xVas_cod) & " AND "
        End If
        If strFiltro.Length > 0 Then
            strFiltro = strFiltro.Substring(0, strFiltro.Length - 4)
        End If
        Dim DT_Analisi As DataTable
        DT_Analisi = HttpContext.Current.Session("DT_Analisi")

        Dim DR_Analisi As DataRow()
        FiltroSelectsuDt(DT_Analisi, DR_Analisi, strFiltro)

        Dim DescrizioneNodo As String
        Dim i As Integer

        If Not DR_Analisi Is Nothing Then

            For i = 0 To DR_Analisi.Length - 1
                Dim xChiaveAlbero As String = ""

                Call Albero.ChiaveAlbero_Codifica_x_json(
                                    xChiaveAlbero,
                                    enum_TipoNodo.Analisi_Testata,
                                    xPiva,
                                    DR_Analisi(i).Item("sa_cod"),
                                    DR_Analisi(i).Item("Campo_cod"),
                                    DR_Analisi(i).Item("Appezza"),
                                    DR_Analisi(i).Item("ID_Imp"),
                                    , DR_Analisi(i).Item("Prov"),
                                    DR_Analisi(i).Item("Com"),
                                    DR_Analisi(i).Item("Sezione"),
                                    DR_Analisi(i).Item("Foglio"),
                                    DR_Analisi(i).Item("Numero"),
                                    DR_Analisi(i).Item("Subalterno"),
                                    , DR_Analisi(i).Item("Fabbricato_cod"),
                                    , , ,
                                    DR_Analisi(i).Item("Analisi_Testata_Cod"),
                                    , , , )

                DescrizioneNodo = "{Analisi} " & DR_Analisi(i).Item("Analisi_Testata_Des") & " - " & CDate(DR_Analisi(i).Item("Analisi_Testata_Data_Inizio")).ToShortDateString


                ''----- Cerco gli eventuali CAMPIONI di analisi
                'filtro
                Dim strFiltro1 As String = ""
                strFiltro1 += " Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' AND "
                If DR_Analisi(i).Item("Analisi_Testata_Cod") <> -99999 Then
                    strFiltro1 += " Analisi_Testata_Cod = " & Agro_SQL_SaveNum(DR_Analisi(i).Item("Analisi_Testata_Cod")) & " AND "
                End If
                If strFiltro1.Length > 0 Then
                    strFiltro1 = strFiltro1.Substring(0, strFiltro1.Length - 4)
                End If

                Dim DT_Campioni As DataTable = HttpContext.Current.Session("DT_Campioni")

                Dim DR_CampioniXDettagli As DataRow()
                FiltroSelectsuDt(DT_Campioni, DR_CampioniXDettagli, strFiltro1)

                Dim j As Integer
                '----- LOOP CAMPIONI
                Dim Campioni As New List(Of AjaxTreeNodeJsonObject)
                For j = 0 To DR_CampioniXDettagli.Length - 1
                    Dim Chiave As String = ""
                    'Creo la chiave per il nodo CAMPIONE
                    Call Albero.ChiaveAlbero_Codifica_x_json(Chiave,
                                            enum_TipoNodo.Analisi_Campione,
                                              Piva, DR_Analisi(i).Item("sa_cod"),
                                              DR_Analisi(i).Item("Campo_cod"),
                                    DR_Analisi(i).Item("Appezza"),
                                    DR_Analisi(i).Item("ID_Imp"),
                                    , DR_Analisi(i).Item("Prov"),
                                    DR_Analisi(i).Item("Com"),
                                    DR_Analisi(i).Item("Sezione"),
                                    DR_Analisi(i).Item("Foglio"),
                                    DR_Analisi(i).Item("Numero"),
                                    DR_Analisi(i).Item("Subalterno"),
                                    , DR_Analisi(i).Item("Fabbricato_cod"),
                                              , , , CInt(DR_CampioniXDettagli(j).Item("Analisi_Testata_Cod")),
                                              CInt(DR_CampioniXDettagli(j).Item("Analisi_Dettaglio_Cod")),
                                              CInt(DR_CampioniXDettagli(j).Item("Analisi_Campione_Cod")))



                    'Aggiungo il nodo campione
                    If Flag_CheckBox Then
                        If Not CheckBoxes.Flag_CheckBoxCampioni Then
                            Campioni.Add(New AjaxTreeNodeJsonObject(Chiave,
                                               "{Campione} " & DR_CampioniXDettagli(j).Item("Analisi_Campione_Des"), "jstree-no-checkboxes", "", "#", False))
                        Else
                            Campioni.Add(New AjaxTreeNodeJsonObject(Chiave,
                                               "{Campione} " & DR_CampioniXDettagli(j).Item("Analisi_Campione_Des"), "", "", "#", False))
                        End If
                    Else
                        Campioni.Add(New AjaxTreeNodeJsonObject(Chiave,
                                               "{Campione} " & DR_CampioniXDettagli(j).Item("Analisi_Campione_Des"), "", "", "#", False))
                    End If
                    Campioni(Campioni.Count - 1).icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Analisi_Campione, "")
                Next



                'aggiungo effettibvamente il nodo dell'analisi
                Dim Analisi As AjaxTreeNodeJsonObject
                If Campioni.Count > 0 Then

                    If Flag_CheckBox Then

                        If Not CheckBoxes.Flag_CheckBoxAnalisi Then

                            Analisi = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                               DescrizioneNodo, "jstree-no-checkboxes", "", "#", Campioni)
                        Else

                            Analisi = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                               DescrizioneNodo, "", "", "#", Campioni)
                        End If

                    Else

                        Analisi = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                               DescrizioneNodo, "", "", "#", Campioni)
                    End If

                Else

                    If Flag_CheckBox Then

                        If Not CheckBoxes.Flag_CheckBoxAnalisi Then

                            Analisi = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                               DescrizioneNodo, "jstree-no-checkboxes", "", "#", False)
                        Else

                            Analisi = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                               DescrizioneNodo, "", "", "#", False)
                        End If

                    Else

                        Analisi = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                               DescrizioneNodo, "", "", "#", False)
                    End If

                End If



                Analisi.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Analisi_Testata, "")

                results.Add(Analisi)

            Next

        End If









        '////////////////////////////
        '///// fine PARTICELLE  /////
        '////////////////////////////



    End Sub

    Private Sub FiltroSelectsuDt(ByRef DT_Analisi As DataTable,
                                             ByRef DR_Analisi As DataRow(),
                                             ByVal strFiltro As String)

        If Not DT_Analisi Is Nothing Then

            DR_Analisi = DT_Analisi.Select(strFiltro)
        End If

    End Sub


#End Region










    ''' <summary>
    ''' Per il caricamento del catasto aziendale 
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaCatasto(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)

        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_x_json(Id, xPiva, xSa_Cod)
        '#####################
        '#####  CATASTO  #####
        '#####################
        Dim xChiaveCatasto As String = ""
        'Definisco la chiave per questo elemento catasto
        Call Albero.ChiaveAlbero_Codifica_x_json(
                                xChiaveCatasto,
                                enum_TipoNodo.CatastoAziendale,
                                xPiva,
                                xSa_Cod,
                                , , , , , , , , , , , )

        Dim Livello_Particelle As New List(Of AjaxTreeNodeJsonObject)
        Dim Catasto As AjaxTreeNodeJsonObject

        If Flag_CheckBox Then
            If Not CheckBoxes.Flag_CheckBoxCatasto Then
                Catasto = New AjaxTreeNodeJsonObject(xChiaveCatasto,
                                                  "Catasto Aziendale", "jstree-no-checkboxes", "", "#", Livello_Particelle)
            Else
                Catasto = New AjaxTreeNodeJsonObject(xChiaveCatasto,
                                                  "Catasto Aziendale", "", "", "#", Livello_Particelle)
            End If
        Else
            Catasto = New AjaxTreeNodeJsonObject(xChiaveCatasto,
                                                  "Catasto Aziendale", "", "", "#", Livello_Particelle)
        End If

        Catasto.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.CatastoAziendale, "")

        results.Add(Catasto)
    End Sub


    ''' <summary>
    ''' Per il caricamento delle Particelle
    ''' </summary>
    Public Sub CaricaParticelle(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)

        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_x_json(Id, xPiva, xSa_Cod)

        '########################
        '#####  PARTICELLE  #####
        '########################
        Dim DT_Particelle As DataTable
        Dim objParticelle As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
        DT_Particelle = objParticelle.LeggixAlberoAnagrafica(xPiva,
                                                             xSa_Cod,
                                                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                             "", "",
                                                              HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim xValidita_Fine As String = ""
        Dim ValidazioneNodo As String = ""
        Dim DescrizioneNodo As String = ""
        Dim xChiave As String = ""

        Dim i As Integer
        For i = 0 To DT_Particelle.Rows.Count - 1

            '---------------------------------------
            '--- Recupero le informazioni ...
            '---------------------------------------

            xValidita_Fine = DT_Particelle.Rows(i).Item("Validita_Fine")
            ValidazioneNodo = DT_Particelle.Rows(i).Item("Validazione")

            '---------------------------------------
            '----- Valuto i dati ottenuti ...
            '---------------------------------------
            DescrizioneNodo = ""

            DescrizioneNodo += AgroPrefix_Particella
            DescrizioneNodo += "{"
            DescrizioneNodo += DT_Particelle.Rows(i).Item("Prov")
            DescrizioneNodo += " : "
            DescrizioneNodo += DT_Particelle.Rows(i).Item("Com")

            DescrizioneNodo += " : "
            If DT_Particelle.Rows(i).Item("Sezione").ToString = "0" Then
                DescrizioneNodo += "__"
            Else
                DescrizioneNodo += Right("__" & DT_Particelle.Rows(i).Item("Sezione").ToString, 2)
            End If

            DescrizioneNodo += " : "
            DescrizioneNodo += Right("______" & DT_Particelle.Rows(i).Item("Foglio").ToString, 6)
            DescrizioneNodo += " : "
            DescrizioneNodo += Right("______" & DT_Particelle.Rows(i).Item("Numero").ToString, 6)

            DescrizioneNodo += " : "
            If DT_Particelle.Rows(i).Item("Subalterno").ToString = "0" Then
                DescrizioneNodo += "__"
            Else
                DescrizioneNodo += Right("__" & DT_Particelle.Rows(i).Item("Subalterno").ToString, 2)
            End If

            DescrizioneNodo += "}"
            DescrizioneNodo += " ..... "
            DescrizioneNodo += CStr(Int(DT_Particelle.Rows(i).Item("Ettari")))
            DescrizioneNodo += ","
            DescrizioneNodo += Right("00" & DT_Particelle.Rows(i).Item("Are").ToString, 2)
            DescrizioneNodo += Right("00" & DT_Particelle.Rows(i).Item("Centiare").ToString, 2)
            DescrizioneNodo += " [ha]"

            DescrizioneNodo += " ..... "
            DescrizioneNodo += TitoloPossessoDes_from_TitoloPossessoCod(
                                    DT_Particelle.Rows(i).Item("TitoloPossesso"))


            'Verifico lo stato di VALIDAZIONE
            If ValidazioneNodo = "-1" Then
                DescrizioneNodo = DescrizioneNodo & " ..... (§§§ da confermare §§§)"
            End If

            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------

            'Definisco la chiave
            Call Albero.ChiaveAlbero_Codifica_x_json(
                                xChiave,
                                enum_TipoNodo.Particella,
                                xPiva,
                                xSa_Cod,
                                , , ,
                                DT_Particelle.Rows(i).Item("Part_cod"),
                                DT_Particelle.Rows(i).Item("Prov"),
                                DT_Particelle.Rows(i).Item("Com"),
                                DT_Particelle.Rows(i).Item("Sezione"),
                                DT_Particelle.Rows(i).Item("Foglio"),
                                DT_Particelle.Rows(i).Item("Numero"),
                                DT_Particelle.Rows(i).Item("Subalterno"),
                                , )

            Dim Particella As AjaxTreeNodeJsonObject
            If Flag_Analisi = True Then
                Dim Livello_Analisi As New List(Of AjaxTreeNodeJsonObject)

                CaricaAnalisi(Livello_Analisi, xChiave, PathRoot, data_Inizio, data_Fine)
                If Livello_Analisi.Count > 0 Then
                    If Flag_CheckBox Then
                        If Not CheckBoxes.Flag_CheckBoxParticella Then
                            Particella = New AjaxTreeNodeJsonObject(xChiave,
                                                        DescrizioneNodo, "jstree-no-checkboxes", "", "#", Livello_Analisi)
                        Else
                            Particella = New AjaxTreeNodeJsonObject(xChiave,
                                                        DescrizioneNodo, "", "", "#", Livello_Analisi)
                        End If
                    Else
                        Particella = New AjaxTreeNodeJsonObject(xChiave,
                                                    DescrizioneNodo, "", "", "#", Livello_Analisi)
                    End If

                Else

                    If Flag_CheckBox Then
                        If Not CheckBoxes.Flag_CheckBoxParticella Then
                            Particella = New AjaxTreeNodeJsonObject(xChiave,
                                                        DescrizioneNodo, "jstree-no-checkboxes", "", "#", False)
                        Else
                            Particella = New AjaxTreeNodeJsonObject(xChiave,
                                                        DescrizioneNodo, "", "", "#", False)
                        End If
                    Else
                        Particella = New AjaxTreeNodeJsonObject(xChiave,
                                                    DescrizioneNodo, "", "", "#", False)
                    End If
                End If

            Else

                If Flag_CheckBox Then
                    If Not CheckBoxes.Flag_CheckBoxParticella Then
                        Particella = New AjaxTreeNodeJsonObject(xChiave,
                                                    DescrizioneNodo, "jstree-no-checkboxes", "", "#", False)
                    Else
                        Particella = New AjaxTreeNodeJsonObject(xChiave,
                                                    DescrizioneNodo, "", "", "#", False)
                    End If
                Else
                    Particella = New AjaxTreeNodeJsonObject(xChiave,
                                                DescrizioneNodo, "", "", "#", False)
                End If
            End If


            Particella.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Particella, "")

            results.Add(Particella)



        Next
        '////////////////////////////
        '///// fine PARTICELLE  /////
        '////////////////////////////



    End Sub



    ''' <summary>
    ''' Per il caricamento dei Planning
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaPlanningTestata(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)
        Dim listaPlan As New List(Of AjaxTreeNodeJsonObject)
        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Dim xPivaPadre As String = ""
        Dim Programmazione_Cod As Integer
        Albero.ChiaveAlbero_Decodifica_PlanningTestata_x_json(Id, xPivaPadre, xPiva, xSa_Cod, Programmazione_Cod)

        Dim xSementiero As String = ""
        If Not HttpContext.Current.Session("sementi") Is Nothing AndAlso
            HttpContext.Current.Session("sementi") <> "-1" Then
            xSementiero = HttpContext.Current.Session("Codice_Fiscale_Tecnico")
        End If

        '########################
        '#####  PLANNING  #####
        '########################
        Dim DT_Planning As DataTable
        Dim objPlanning As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R

        DT_Planning = objPlanning.Leggi_X_Albero(0, xSementiero, xPiva,
                                          xSa_Cod, data_Inizio, data_Fine, "", "",
                                          HttpContext.Current.Session("ASG_objParametri_Server"))



        Dim xValidita_Inizio As String = ""
        Dim xValidita_Fine As String = ""
        Dim xDescrizione As String = ""



        Dim DescrizioneNodo As String
        Dim i As Integer
        For i = 0 To DT_Planning.Rows.Count - 1


            '---------------------------------------
            '--- Recupero le informazioni ...
            '---------------------------------------
            xValidita_Inizio = CDate(DT_Planning.Rows(i).Item("Validita_Inizio")).ToShortDateString()
            xValidita_Fine = CDate(DT_Planning.Rows(i).Item("Validita_Fine")).ToShortDateString()

            xDescrizione = DT_Planning.Rows(i).Item("Programmazione_Des")

            '---------------------------------------
            '----- Valuto i dati ottenuti ...
            '---------------------------------------

            'Inizio a costruire la descrizione
            DescrizioneNodo = ""
            DescrizioneNodo += AgroPrefix_Planning
            DescrizioneNodo += xDescrizione

            'Verifico se devo aggiungere dettagli ...

            'If (DettagliNodoAppezzamento = True) Then
            'DescrizioneNodo += " (" & " v.Inizio: " & xValidita_Inizio & " - v.Fine: " & xValidita_Fine & ")"
            'End If


            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------

            Dim xChiave As String = ""
            'Definisco la chiave per questo elemento catasto
            Call Albero.ChiaveAlbero_Codifica_x_json(
                                    xChiave,
                                    enum_TipoNodo.PlanningTestata,
                                    Piva:=xPiva,
                                    Sa_Cod:=xSa_Cod,
                                    Programmazione_Cod:=DT_Planning.Rows(i).Item("programmazione_cod")
                            )

            Dim planning As AjaxTreeNodeJsonObject



            'inserisco il nodo ed eventualmente i sui figli
            Dim child As Object
            If Flag_Esplodi_Tutto = True Then
                child = New List(Of AjaxTreeNodeJsonObject)
                'CaricaPlanningDettaglio(child, xChiave, PathRoot)
                CaricaPlanningDettaglioEntita(child, xChiave, PathRoot, data_Inizio, data_Fine)
            Else
                child = New Boolean
                child = True
            End If

            If Flag_CheckBox Then
                If Not CheckBoxes.Flag_CheckBoxCentro Then
                    planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "jstree-no-checkboxes", "", "#", child)
                Else
                    planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
                End If
            Else
                planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
            End If

            If Flag_Esplodi_Tutto = True Then
                planning.state = "open"
            End If
            planning.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.PlanningTestata, "")
            'nodo aggiunto



            listaPlan.Add(planning)

        Next
        If listaPlan.Count > 0 Then
            Dim planTot As New AjaxTreeNodeJsonObject
            DescrizioneNodo = "Planning"
            If Flag_CheckBox Then
                If Not CheckBoxes.Flag_CheckBoxCentro Then
                    planTot = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "jstree-no-checkboxes", "", "#", listaPlan)
                Else
                    planTot = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "", "", "#", listaPlan)
                End If
            Else
                planTot = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "", "", "#", listaPlan)
            End If
            If Flag_Esplodi_Tutto = True Then
                planTot.state = "open"
            End If

            planTot.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.PlanningTestata, "")
            results.Add(planTot)
        End If


    End Sub


    ''' <summary>
    ''' Per il caricamento dei Planning
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaPlanningDettaglio(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)

        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Dim Programmazione_Cod As Integer
        Dim Programmazione_Entita_Cod As Integer
        Albero.ChiaveAlbero_Decodifica_PlanningEntita_x_json(Id, xPiva, xSa_Cod, Programmazione_Cod, Programmazione_Entita_Cod)

        '########################
        '#####  PLANNING ENTITA  #####
        '########################
        Dim DT_Planning As DataTable
        Dim objPlanning As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

        DT_Planning = objPlanning.Leggi(Programmazione_Cod, Programmazione_Entita_Cod,
                                          "", xPiva, xSa_Cod, 0, 0, 0, 0, data_Inizio, data_Fine, "", "", "",
                                          HttpContext.Current.Session("ASG_objParametri_Server"))



        'Dim xValidita_Inizio As String = ""
        'Dim xValidita_Fine As String = ""
        Dim xDescrizione As String = ""



        Dim DescrizioneNodo As String
        Dim i As Integer
        For i = 0 To DT_Planning.Rows.Count - 1


            '---------------------------------------
            '--- Recupero le informazioni ...
            '---------------------------------------
            'xValidita_Inizio = CDate(DT_Planning.Rows(i).Item("Validita_Inizio")).ToShortDateString()
            'xValidita_Fine = CDate(DT_Planning.Rows(i).Item("Validita_Fine")).ToShortDateString()

            xDescrizione = DT_Planning.Rows(i).Item("Programmazione_Des")

            '---------------------------------------
            '----- Valuto i dati ottenuti ...
            '---------------------------------------

            'Inizio a costruire la descrizione
            DescrizioneNodo = ""
            DescrizioneNodo += AgroPrefix_DettaglioPlanning
            DescrizioneNodo += xDescrizione

            'DescrizioneNodo += " (" & " v.Inizio: " & xValidita_Inizio & " - v.Fine: " & xValidita_Fine & ")"


            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------

            Dim xChiave As String = ""
            'Definisco la chiave per questo elemento catasto
            Call Albero.ChiaveAlbero_Codifica_x_json(
                                    xChiave,
                                    enum_TipoNodo.PlanningEntita,
                                    xPiva,
                                    xSa_Cod,
                                    , , , , , , , , , , , , , , , , , , , , DT_Planning.Rows(i).Item("programmazione_cod"), DT_Planning.Rows(i).Item("Programmazione_Entita_Cod"))


            Dim planning As AjaxTreeNodeJsonObject

            'If Flag_CheckBox Then

            '    If Not CheckBoxes.Flag_CheckboxAppezzamento Then

            '        planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "jstree-no-checkboxes", "", "#", True)
            '    Else

            '        planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", True)
            '    End If
            'Else
            '    planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", True)
            'End If

            'planning.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.PlanningEntita, "")





            'inserisco il nodo ed eventualmente i sui figli
            Dim child As Object
            If Flag_Esplodi_Tutto = True Then
                child = New List(Of AjaxTreeNodeJsonObject)
                CaricaPlanningDettaglioEntita(child, xChiave, PathRoot, data_Inizio, data_Fine)
            Else
                child = New Boolean
                child = True
            End If

            If Flag_CheckBox Then
                If Not CheckBoxes.Flag_CheckBoxCentro Then
                    planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "jstree-no-checkboxes", "", "#", child)
                Else
                    planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
                End If
            Else
                planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
            End If

            If Flag_Esplodi_Tutto = True Then
                planning.state = "open"
            End If
            planning.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.PlanningEntita, "")
            'nodo aggiunto






            results.Add(planning)
        Next
        '////////////////////////////
        '///// fine PARTICELLE  /////
        '////////////////////////////
    End Sub


    ''' <summary>
    ''' Per il caricamento dei Planning
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaPlanningDettaglioEntita(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)

        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Dim Programmazione_Cod As Integer
        Dim Programmazione_Entita_Cod As Integer
        Albero.ChiaveAlbero_Decodifica_PlanningEntita_x_json(Id, xPiva, xSa_Cod, Programmazione_Cod, Programmazione_Entita_Cod)

        '########################
        '#####  PLANNING ENTITA  #####
        '########################
        Dim DT_Planning As DataTable
        Dim objPlanning As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

        DT_Planning = objPlanning.Leggi(Programmazione_Cod, Programmazione_Entita_Cod,
                                          "", xPiva, xSa_Cod, 0, 0, 0, 0, data_Inizio, data_Fine, "", "", "",
                                          HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim xDescrizione As String = ""


        Dim DescrizioneNodo As String
        Dim i As Integer
        For i = 0 To DT_Planning.Rows.Count - 1




            Dim xSupPlan As String = IIf(IsDBNull(DT_Planning.Rows(i).Item("Superficie")), "", DT_Planning.Rows(i).Item("Superficie"))
            Dim xApp_Nome As String = IIf(IsDBNull(DT_Planning.Rows(i).Item("Entita_Des")), "", DT_Planning.Rows(i).Item("Entita_Des"))
            Dim xCul_Cod As Integer = IIf(IsDBNull(DT_Planning.Rows(i).Item("Cul_Cod")), 0, DT_Planning.Rows(i).Item("Cul_Cod"))
            Dim xGru_Cod As Integer
            Dim xVeg_Cod As Integer = IIf(IsDBNull(DT_Planning.Rows(i).Item("Veg_Cod")), 0, DT_Planning.Rows(i).Item("Veg_Cod"))
            Dim xValidita_Inizio As String = CDate(DT_Planning.Rows(i).Item("Validita_Inizio")).ToShortDateString()
            Dim xValidita_Fine As String = CDate(DT_Planning.Rows(i).Item("Validita_Fine")).ToShortDateString()

            Dim objGruCod As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            Dim dtgru As DataTable
            dtgru = objGruCod.Leggi_con_Cul_Des(xVeg_Cod, xCul_Cod, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))
            xGru_Cod = dtgru.Rows(0).Item("Gru_Cod")
            Dim Veg_Des, Cul_Des As String
            Veg_Des = dtgru.Rows(0).Item("Veg_Des")
            Cul_Des = dtgru.Rows(0).Item("Cul_DEs")

            Dim TipoNodo As Integer

            '---------------------------------------
            '----- Valuto i dati ottenuti ...
            '---------------------------------------

            'Inizio a costruire la descrizione
            DescrizioneNodo = AgroPrefix_Impianto

            'Se il Cul_Cod e' nullo ho terreno nudo ...
            If xCul_Cod = 0 Then
                '===== TERRENO NUDO =====

                TipoNodo = enum_TipoNodo.ImpiantoNudo
                DescrizioneNodo += "Terreno Nudo"

            Else

                '===== COLTURA ==========
                'Verifico il Gruppo Vegetale per scegliere l'icona
                Select Case xGru_Cod
                    Case 1
                        TipoNodo = enum_TipoNodo.ImpiantoArborea
                    Case 2
                        TipoNodo = enum_TipoNodo.ImpiantoErbacea
                    Case 3
                        TipoNodo = enum_TipoNodo.ImpiantoOrticola
                End Select

                'Descrizione del nodo
                DescrizioneNodo += xValidita_Inizio
                DescrizioneNodo += " - "
                DescrizioneNodo += xApp_Nome
                DescrizioneNodo += " - "
                DescrizioneNodo += Veg_Des
                DescrizioneNodo += " - "
                DescrizioneNodo += Cul_Des
                DescrizioneNodo += ": { "
                DescrizioneNodo += xSupPlan
                DescrizioneNodo += " Ha }"
            End If



            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------
            Dim xChiave As String = ""
            'Definisco la chiave per questo elemento catasto
            Call Albero.ChiaveAlbero_Codifica_x_json(
                                    xChiave,
                                    enum_TipoNodo.PlanningEntitaImpianto,
                                    xPiva,
                                    xSa_Cod,
                                    0, 0, 0, , , , , , , , , , , , , , , , , , DT_Planning.Rows(i).Item("programmazione_cod"), DT_Planning.Rows(i).Item("Programmazione_Entita_Cod"))


            Dim planning As AjaxTreeNodeJsonObject

            If Flag_CheckBox Then
                If Not CheckBoxes.Flag_CheckBoxAppezzamento Then
                    planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "jstree-no-checkboxes", "", "#", False)
                Else
                    planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", False)
                End If
            Else
                planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", False)
            End If

            Dim IconaPreferita As String = ""
            Albero.Selezionatore_Icone(IconaPreferita,
                                TipoNodo,
                                xVeg_Cod
                                 )
            planning.icon = PathRoot + Albero.RitornaPathImg(TipoNodo, IconaPreferita)

            results.Add(planning)
        Next
        '////////////////////////////
        '///// fine PARTICELLE  /////
        '////////////////////////////
    End Sub




    ''' <summary>
    ''' Carica il nodo Portafoglio Prodotti
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaPortafoglioProdotti(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)

        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_x_json(Id, xPiva, xSa_Cod)
        '##################################
        '#####  Portafoglio Prodotti  #####
        '##################################
        Dim xChiave As String = ""
        'Definisco la chiave per questo elemento catasto
        Call Albero.ChiaveAlbero_Codifica_x_json(
                                xChiave,
                                enum_TipoNodo.p_PortafoglioProdotti,
                                xPiva,
                                xSa_Cod,
                                , , , , , , , , , , , )

        Dim Livello_Particelle As New List(Of AjaxTreeNodeJsonObject)
        Dim Portafoglio As AjaxTreeNodeJsonObject

        If Flag_CheckBox Then

            If Not CheckBoxes.Flag_CheckBoxProdotti Then

                Portafoglio = New AjaxTreeNodeJsonObject(xChiave,
                                                  "Prodotti Aziendali", "jstree-no-checkboxes", "", "#", False)
            Else

                Portafoglio = New AjaxTreeNodeJsonObject(xChiave,
                                                  "Prodotti Aziendali", "", "", "#", False)
            End If

        Else

            Portafoglio = New AjaxTreeNodeJsonObject(xChiave,
                                                  "Prodotti Aziendali", "", "", "#", False)
        End If

        Portafoglio.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.p_PortafoglioProdotti, "")

        results.Add(Portafoglio)
    End Sub


    ''' <summary>
    ''' Carica La lista dei nodi dei Campi
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaNodiCampi(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)

        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_x_json(Id, xPiva, xSa_Cod)



        Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        'Valorizzo il Codice_Fiscale_Tecnico prendendolo dalla tabella Gruppi_Utente
        Dim Codice_Fiscale_Tecnico As String = ""
        Dim xUtenteCorrente As String = CType(objParametri_Server, AgronicaCoreDataProvider.AgronicaCoreParametri).UtenteUsername
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
        Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(xUtenteCorrente, objParametri_Utenti)




        '###################
        '#####  CAMPI  #####
        '###################
        Dim DT_Campi As DataTable
        Dim objCampi As New AgronicaCoreAnagrafeDAL.Campi_R

        Dim str_aggiuntivo_x_specie As String
        If Not IsNothing(HttpContext.Current.Session("Sementi")) AndAlso (HttpContext.Current.Session("Sementi") <> "-1") Then
            Dim v = HttpContext.Current.Session("Sementi").split("|")(4)
            str_aggiuntivo_x_specie = " ( exists(select * from reg_impianti " &
                    " where cul_cod in( select cul_cod from cultivar  " &
                    " where(reg_impianti.piva = Campi.PIVA And reg_impianti.SA_COD = Campi.SA_COD  ) " &
                    " and veg_cod in (SELECT     distinct   Mappatura_Specie.Veg_Cod FROM            Sementieri_Sportello_ConfigurazioneXmappatura_specie INNER JOIN Mappatura_Specie ON Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Specie = Mappatura_Specie.ID_Specie AND  Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_SottoSpecie = Mappatura_Specie.ID_SottoSpecie AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Gruppo = Mappatura_Specie.ID_Gruppo AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Genotipo = Mappatura_Specie.ID_Genotipo " &
                    " WHERE Sementieri_Sportello_ConfigurazioneXmappatura_specie.Sementieri_Sportello_Configurazione_cod = " & v &
                    " ))) " &
                    " OR " &
                    "   exists(select * from programmazione_entita " &
                    " where cul_cod in( select cul_cod from cultivar  " &
                    " where(programmazione_entita.piva = Campi.PIVA And programmazione_entita.SA_COD = Campi.SA_COD  ) " &
                    " and veg_cod in (SELECT     distinct   Mappatura_Specie.Veg_Cod FROM            Sementieri_Sportello_ConfigurazioneXmappatura_specie INNER JOIN Mappatura_Specie ON Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Specie = Mappatura_Specie.ID_Specie AND  Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_SottoSpecie = Mappatura_Specie.ID_SottoSpecie AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Gruppo = Mappatura_Specie.ID_Gruppo AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Genotipo = Mappatura_Specie.ID_Genotipo " &
                    " WHERE Sementieri_Sportello_ConfigurazioneXmappatura_specie.Sementieri_Sportello_Configurazione_cod = " & v &
                    " ))) " &
                    " ) "

        End If

        DT_Campi = objCampi.Leggi(xPiva, xSa_Cod, 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                str_aggiuntivo_x_specie, "",
                                HttpContext.Current.Session("ASG_objParametri_Server"))


        Dim i_Campi As Integer

        Dim xCampo_Cod, TipoNodo As Integer
        Dim xCampo_Des, ValidazioneNodo, xValidita_Fine, DescrizioneNodo As String
        For i_Campi = 0 To DT_Campi.Rows.Count - 1

            '---------------------------------------
            '--- Recupero le informazioni ...
            '---------------------------------------

            'xPiva = DR_Campi(i_Campi)("Piva")
            'xSa_Cod = DR_Campi(i_Campi)("Sa_Cod")
            xCampo_Cod = DT_Campi.Rows(i_Campi).Item("Campo_Cod")

            xCampo_Des = DT_Campi.Rows(i_Campi).Item("Campo_Des")
            ValidazioneNodo = DT_Campi.Rows(i_Campi).Item("Validazione")
            xValidita_Fine = DT_Campi.Rows(i_Campi).Item("Validita_Fine")

            '---------------------------------------
            '----- Valuto i dati ottenuti ...
            '---------------------------------------

            'Inizio a costruire la descrizione
            DescrizioneNodo = ""

            'Verifico se ho un campo oppure una serra ...
            Select Case DT_Campi.Rows(i_Campi).Item("Campo_Tipo") = 1
                Case 0      'CAMPO
                    TipoNodo = enum_TipoNodo.Campo
                    DescrizioneNodo += AgroPrefix_Campo

                Case Else   'SERRA
                    TipoNodo = enum_TipoNodo.Serra
                    DescrizioneNodo += AgroPrefix_Serra

            End Select

            DescrizioneNodo += xCampo_Des

            'Verifico lo stato di VALIDAZIONE
            If ValidazioneNodo = "-1" Then
                DescrizioneNodo = DescrizioneNodo & " ..... (§§§ da confermare §§§)"
            End If


            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------
            Dim xChiave As String = ""
            'Definisco la chiave per questo elemento catasto
            Call Albero.ChiaveAlbero_Codifica_x_json(
                                    xChiave,
                                    TipoNodo,
                                    xPiva,
                                    xSa_Cod,
                                    xCampo_Cod, , , , , , , , , , , )


            Dim Campo As AjaxTreeNodeJsonObject






            'inserisco il nodo ed eventualmente i sui figli
            Dim child As Object
            If Flag_Esplodi_Tutto = True Then
                child = New List(Of AjaxTreeNodeJsonObject)
                If Flag_Analisi = True Then
                    CaricaAnalisi(child, xChiave, PathRoot, data_Inizio, data_Fine)
                End If
                CaricaAppezzamenti(child, xChiave, PathRoot, xCampo_Cod, Codice_Fiscale_Tecnico, data_Inizio, data_Fine)
            Else
                child = New Boolean
                child = True
            End If

            If Flag_CheckBox Then
                If Not CheckBoxes.Flag_CheckBoxCentro Then
                    Campo = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "jstree-no-checkboxes", "", "#", child)
                Else
                    Campo = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
                End If
            Else
                Campo = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
            End If

            If Flag_Esplodi_Tutto = True Then
                Campo.state = "open"
            End If
            Campo.icon = PathRoot + Albero.RitornaPathImg(TipoNodo, "")

            results.Add(Campo)
        Next i_Campi

        '///////////////////////////
        '///// fine CAMPI  /////////
        '///////////////////////////

    End Sub




    ''' <summary>
    ''' Carico gli appezzamenti
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <param name="xCampocod"></param>
    ''' <remarks></remarks>
    Public Sub CaricaAppezzamenti(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal xCampocod As Integer,
                                                 ByVal Codice_Fiscale_Tecnico As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date,
                                                 Optional ByVal xSementiero As String = "")


        'Dim xCodice_Fiscale_Tecnico As String = ""
        'If Not IsNothing(HttpContext.Current.Session("Codice_Fiscale_Tecnico")) Then
        '    xCodice_Fiscale_Tecnico = HttpContext.Current.Session("Codice_Fiscale_Tecnico").ToString
        'Else
        '    xCodice_Fiscale_Tecnico = ""
        'End If

        '(11/06/2021) rimossa verifica su codice_fiscale_tecnico
        'ora il filtro non viene applicato
        Dim locFlag_Appezzamenti_Filtra_Tecnico As Boolean = False

        'Dim locFlag_Appezzamenti_Filtra_Tecnico As Boolean = True
        'If Codice_Fiscale_Tecnico = "CF TEC" Then
        '    locFlag_Appezzamenti_Filtra_Tecnico = False
        'End If


        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer

        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_x_json(Id, xPiva, xSa_Cod)


        'filtro per la data operazione.anno, pèer non avere troppi impianti
        Dim filtrodata As String = ""
        Try
            If Not IsNothing(HttpContext.Current.Session("ParametriAgenda")) AndAlso
                IsDate(HttpContext.Current.Session("ParametriAgenda")) Then

                Dim data As Date = HttpContext.Current.Session("ParametriAgenda").data
                Dim anno As Integer = data.Year
                filtrodata = " Appezzamento.Validita_Fine > " & Agro_SQL_SaveDate("01/01/" & anno) & " AND   Appezzamento.Validita_Inizio < " & Agro_SQL_SaveDate("31/12/" & anno) & "  "


            End If


        Catch ex As Exception
            filtrodata = ""
        End Try

        '###################################
        '#####  APPEZZAMENTI #####
        '###################################

        Dim DT_Appezzamenti As DataTable
        Dim objAppezzamenti As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        visualizzaRiferimentoAlfanumericoImpianto = HttpContext.Current.Session("visualizzaRiferimentoAlfanumericoImpianto")
        ordinaDataUltimoImpianto = HttpContext.Current.Session("ordinaDataUltimoImpianto")


        If Not String.IsNullOrEmpty(xSementiero) Then
            xSementiero = " AND T1.Codice_Fiscale_Tecnico = '" & Agro_SQL_SaveText(xSementiero) & "' "
        End If

        If FlagModalitaSementieri Then
            xSementiero = " AND T1.Codice_Fiscale_Tecnico = '" & Agro_SQL_SaveText(PivaPadre) & "' "
        End If

        Dim lOrderByDataUltimoImpianto As String = ""
        If ordinaDataUltimoImpianto Then
            lOrderByDataUltimoImpianto = " InizioImpianto desc "
        End If

        If xCampocod = 0 Then

            Dim filtro As String = "  Appezzamento.Campo_Cod = " & xCampocod & " "
            If filtrodata <> "" Then
                filtro &= " AND " & filtrodata
            End If


            If xSementiero <> "" Then
                Dim v = HttpContext.Current.Session("Sementi").split("|")(4)
                Dim str_aggiuntivo_x_specie = " (  exists(select * from reg_impianti " &
                        " where cul_cod in( select cul_cod from cultivar  " &
                        " where(reg_impianti.piva = Appezzamento.PIVA And reg_impianti.SA_COD = Appezzamento.SA_COD And reg_impianti.APPEZZA = Appezzamento.APPEZZA) " &
                        " and veg_cod in (SELECT     distinct   Mappatura_Specie.Veg_Cod FROM            Sementieri_Sportello_ConfigurazioneXmappatura_specie INNER JOIN Mappatura_Specie ON Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Specie = Mappatura_Specie.ID_Specie AND  Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_SottoSpecie = Mappatura_Specie.ID_SottoSpecie AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Gruppo = Mappatura_Specie.ID_Gruppo AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Genotipo = Mappatura_Specie.ID_Genotipo " &
                        " WHERE Sementieri_Sportello_ConfigurazioneXmappatura_specie.Sementieri_Sportello_Configurazione_cod = " & v &
                        " ))) " &
                        " OR " &
                        "   exists(select * from programmazione_entita " &
                        " where cul_cod in( select cul_cod from cultivar  " &
                        " where(programmazione_entita.piva = Appezzamento.PIVA And programmazione_entita.SA_COD = Appezzamento.SA_COD  And programmazione_entita.APPEZZA = Appezzamento.APPEZZA)   " &
                        " and veg_cod in (SELECT     distinct   Mappatura_Specie.Veg_Cod FROM            Sementieri_Sportello_ConfigurazioneXmappatura_specie INNER JOIN Mappatura_Specie ON Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Specie = Mappatura_Specie.ID_Specie AND  Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_SottoSpecie = Mappatura_Specie.ID_SottoSpecie AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Gruppo = Mappatura_Specie.ID_Gruppo AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Genotipo = Mappatura_Specie.ID_Genotipo " &
                        " WHERE Sementieri_Sportello_ConfigurazioneXmappatura_specie.Sementieri_Sportello_Configurazione_cod = " & v &
                        " ))) " &
                        " ) "
                If filtro = "" Then
                    filtro = str_aggiuntivo_x_specie
                Else
                    filtro = filtro & " AND " & str_aggiuntivo_x_specie
                End If

                If (Veg_Cod <> 0) Then
                    filtrodata = "SpecieVegetali.Veg_Cod =" + CStr(Veg_Cod) + IIf(filtrodata = "", "", " AND " + filtrodata)
                    DT_Appezzamenti = objAppezzamenti.Recupera_Appezzamenti_Colture_del_Campo(xPiva, xSa_Cod, xCampocod, AGRODATAINIZIO, AGRODATAFINE, True, enumSelezioneVariabile.Selezione_JoinDescrizioni, filtrodata, lOrderByDataUltimoImpianto, HttpContext.Current.Session("ASG_objParametri_Server"))
                Else
                    DT_Appezzamenti = objAppezzamenti.LeggiconFiltroSementieri(xPiva, xSa_Cod, xCampocod, xSementiero,
                                                enumSelezioneVariabile.Selezione_JoinCompleta,
                                                filtro, lOrderByDataUltimoImpianto, HttpContext.Current.Session("ASG_objParametri_Server"))
                End If
            Else


                If locFlag_Appezzamenti_Filtra_Tecnico Then

                    If (Veg_Cod <> 0) Then
                        filtrodata = "SpecieVegetali.Veg_Cod =" + CStr(Veg_Cod) + IIf(filtrodata = "", "", " AND " + filtrodata)
                        DT_Appezzamenti = objAppezzamenti.Recupera_Appezzamenti_Colture_del_Campo(xPiva, xSa_Cod, xCampocod, AGRODATAINIZIO, AGRODATAFINE, True, enumSelezioneVariabile.Selezione_JoinDescrizioni, filtrodata, lOrderByDataUltimoImpianto, HttpContext.Current.Session("ASG_objParametri_Server"))
                    Else
                        DT_Appezzamenti = objAppezzamenti.LeggiconCampo_ConFiltroCFT(xPiva, xSa_Cod, xCampocod, Codice_Fiscale_Tecnico,
                                                    enumSelezioneVariabile.Selezione_JoinCompleta,
                                                    filtro, lOrderByDataUltimoImpianto, HttpContext.Current.Session("ASG_objParametri_Server"))
                    End If

                Else
                    If (Veg_Cod <> 0) Then
                        filtrodata = "SpecieVegetali.Veg_Cod =" + CStr(Veg_Cod) + IIf(filtrodata = "", "", " AND " + filtrodata)
                        DT_Appezzamenti = objAppezzamenti.LeggiconCampo_eVegCod(xPiva, xSa_Cod, xCampocod, Veg_Cod,
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    filtro, lOrderByDataUltimoImpianto, HttpContext.Current.Session("ASG_objParametri_Server"))
                    Else
                        DT_Appezzamenti = objAppezzamenti.LeggiconCampo(xPiva, xSa_Cod, xCampocod,
                                                    enumSelezioneVariabile.Selezione_JoinCompleta,
                                                    filtro, lOrderByDataUltimoImpianto, HttpContext.Current.Session("ASG_objParametri_Server"))
                    End If

                End If

            End If
        Else

            If xSementiero <> "" Then
                Dim v = HttpContext.Current.Session("Sementi").split("|")(4)
                Dim str_aggiuntivo_x_specie = "   exists(select * from reg_impianti " &
                        " where cul_cod in( select cul_cod from cultivar  " &
                        " where(reg_impianti.piva = Appezzamento.PIVA And reg_impianti.SA_COD = Appezzamento.SA_COD And reg_impianti.APPEZZA = Appezzamento.APPEZZA) " &
                        " and veg_cod in (SELECT     distinct   Mappatura_Specie.Veg_Cod FROM            Sementieri_Sportello_ConfigurazioneXmappatura_specie INNER JOIN Mappatura_Specie ON Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Specie = Mappatura_Specie.ID_Specie AND  Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_SottoSpecie = Mappatura_Specie.ID_SottoSpecie AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Gruppo = Mappatura_Specie.ID_Gruppo AND Sementieri_Sportello_ConfigurazioneXmappatura_specie.ID_Genotipo = Mappatura_Specie.ID_Genotipo " &
                        " WHERE Sementieri_Sportello_ConfigurazioneXmappatura_specie.Sementieri_Sportello_Configurazione_cod = " & v &
                        " ))) "
                If filtrodata = "" Then
                    filtrodata = str_aggiuntivo_x_specie
                Else
                    filtrodata = filtrodata & " AND " & str_aggiuntivo_x_specie
                End If
                DT_Appezzamenti = objAppezzamenti.LeggiconFiltroSementieri(xPiva, xSa_Cod, xCampocod, xSementiero,
                                            enumSelezioneVariabile.Selezione_JoinCompleta,
                                            filtrodata, lOrderByDataUltimoImpianto, HttpContext.Current.Session("ASG_objParametri_Server"))
            Else

                If locFlag_Appezzamenti_Filtra_Tecnico Then

                    If (Veg_Cod <> 0) Then
                        filtrodata = "SpecieVegetali.Veg_Cod =" + CStr(Veg_Cod) + IIf(filtrodata = "", "", " AND " + filtrodata)
                        DT_Appezzamenti = objAppezzamenti.Recupera_Appezzamenti_Colture_del_Campo(xPiva, xSa_Cod, xCampocod, AGRODATAINIZIO, AGRODATAFINE, True, enumSelezioneVariabile.Selezione_JoinDescrizioni, filtrodata, lOrderByDataUltimoImpianto, HttpContext.Current.Session("ASG_objParametri_Server"))
                    Else
                        DT_Appezzamenti = objAppezzamenti.LeggiconCampo_ConFiltroCFT(xPiva, xSa_Cod, xCampocod, Codice_Fiscale_Tecnico,
                                                    enumSelezioneVariabile.Selezione_JoinCompleta,
                                                    filtrodata, lOrderByDataUltimoImpianto, HttpContext.Current.Session("ASG_objParametri_Server"))
                    End If
                Else
                    If (Veg_Cod <> 0) Then
                        filtrodata = "SpecieVegetali.Veg_Cod =" + CStr(Veg_Cod) + IIf(filtrodata = "", "", " AND " + filtrodata)
                        DT_Appezzamenti = objAppezzamenti.LeggiconCampo_eVegCod(xPiva, xSa_Cod, xCampocod, Veg_Cod,
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    filtrodata, lOrderByDataUltimoImpianto, HttpContext.Current.Session("ASG_objParametri_Server"))
                    Else

                        DT_Appezzamenti = objAppezzamenti.LeggiconCampo(xPiva, xSa_Cod, xCampocod,
                                                    enumSelezioneVariabile.Selezione_JoinCompleta,
                                                    filtrodata, lOrderByDataUltimoImpianto, HttpContext.Current.Session("ASG_objParametri_Server"))
                    End If
                End If



            End If


        End If

        Dim i As Integer
        'Per ciascun elemento
        Dim xAppezza, xAppNome, ValidazioneNodo, xValidita_Fine, DescrizioneNodo As String
        For i = 0 To DT_Appezzamenti.Rows.Count - 1

            '---------------------------------------
            '--- Recupero le informazioni ...
            '---------------------------------------
            xAppezza = DT_Appezzamenti.Rows(i).Item("Appezza")
            xAppNome = DT_Appezzamenti.Rows(i).Item("App_Nome")

            If visualizzaRiferimentoAlfanumericoImpianto Then
                Dim RiferimentoAlfanumerico As String = DT_Appezzamenti.Rows(i).Item("RiferimentoAlfanumerico")
                If RiferimentoAlfanumerico <> "" Then
                    xAppNome = RiferimentoAlfanumerico & " - " & xAppNome
                End If
            End If


            ValidazioneNodo = DT_Appezzamenti.Rows(i).Item("Validazione")
            xValidita_Fine = DT_Appezzamenti.Rows(i).Item("Validita_Fine")

            '---------------------------------------
            '----- Valuto i dati ottenuti ...
            '---------------------------------------

            'Inizio a costruire la descrizione
            DescrizioneNodo = ""
            DescrizioneNodo += AgroPrefix_Appezzamento
            DescrizioneNodo += xAppNome

            'Verifico se devo aggiungere dettagli ...

            'If (DettagliNodoAppezzamento = True) Then
            DescrizioneNodo += " : {" & String.Format(DT_Appezzamenti.Rows(i).Item("Sup_App"), "0.0000") & " ha}"
            'End If

            'Verifico lo stato di VALIDAZIONE
            If ValidazioneNodo = "-1" Then
                DescrizioneNodo = DescrizioneNodo & " ..... (§§§ da confermare §§§)"
            End If


            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------

            Dim xChiave As String = ""
            'Definisco la chiave per questo elemento catasto
            Call Albero.ChiaveAlbero_Codifica_x_json(
                                    xChiave,
                                    enum_TipoNodo.Appezzamento,
                                    xPiva,
                                    xSa_Cod,
                                    xCampocod, xAppezza)

            Dim Appezzamento As AjaxTreeNodeJsonObject








            'inserisco il nodo ed eventualmente i sui figli
            Dim child As Object
            If Flag_Esplodi_Tutto = True Then
                child = New List(Of AjaxTreeNodeJsonObject)
                If Flag_Analisi = True Then
                    CaricaAnalisi(child, xChiave, PathRoot, data_Inizio, data_Fine)
                End If
                CaricaImpianti(child, xChiave, PathRoot, data_Inizio, data_Fine, xSementiero)
            Else
                child = New Boolean
                child = True
            End If

            If Flag_CheckBox Then
                If Not CheckBoxes.Flag_CheckBoxCentro Then
                    Appezzamento = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "jstree-no-checkboxes", "", "#", child)
                Else
                    Appezzamento = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
                End If
            Else
                Appezzamento = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
            End If

            If Flag_Esplodi_Tutto = True Then
                Appezzamento.state = "open"
            End If
            Appezzamento.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Appezzamento, "")











            results.Add(Appezzamento)






        Next

    End Sub








    ''' <summary>
    ''' Carico gli appezzamenti
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <param name="xCampocod"></param>
    ''' <remarks></remarks>
    Public Sub CaricaAppezzamento(ByRef risultato As AjaxTreeNodeJsonObject,
                                  ByVal PathRoot As String,
                                                 ByVal piva As String,
                                                 ByVal sa_cod As Integer,
                                                 ByVal appezza As Integer,
                                                 Optional ByVal Id_Agenda As Integer = 0,
                                                 Optional ByVal xTipoNodoChiave As Integer = 0)

        '###################################
        '#####  APPEZZAMENTI #####
        '###################################

        Dim DT_Appezzamenti As DataTable
        Dim objAppezzamenti As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        visualizzaRiferimentoAlfanumericoImpianto = HttpContext.Current.Session("visualizzaRiferimentoAlfanumericoImpianto")
        ordinaDataUltimoImpianto = HttpContext.Current.Session("ordinaDataUltimoImpianto")


        'leggo le info dell'appezzamento
        Dim filtroAgg As String = ""

        If (Veg_Cod <> 0) Then
            filtroAgg = "SpecieVegetali.Veg_Cod =" + CStr(Veg_Cod) + IIf(filtroAgg = "", "", " AND " + filtroAgg)
            DT_Appezzamenti = objAppezzamenti.LeggiAppezzamentiDaVegCod(piva, Veg_Cod, enumSelezioneVariabile.Selezione_JoinDescrizioni, filtroAgg, "", HttpContext.Current.Session("ASG_objParametri_Server"))
        Else
            DT_Appezzamenti = objAppezzamenti.Leggi(piva, sa_cod, appezza, enumSelezioneVariabile.Selezione_TabellaCompleta, filtroAgg, "", HttpContext.Current.Session("ASG_objParametri_Server"))
        End If
        Dim campocod As Integer = 0
        campocod = DT_Appezzamenti.Rows(0).Item("campo_cod")
        Dim xAppNome, ValidazioneNodo, xValidita_Inizio, xValidita_Fine, DescrizioneNodo As String

        '---------------------------------------
        '--- Recupero le informazioni ...
        '---------------------------------------
        xAppNome = DT_Appezzamenti.Rows(0).Item("App_Nome")

        If visualizzaRiferimentoAlfanumericoImpianto Then
            Dim RiferimentoAlfanumerico As String = DT_Appezzamenti.Rows(0).Item("RiferimentoAlfanumerico")
            If RiferimentoAlfanumerico <> "" Then
                xAppNome = RiferimentoAlfanumerico & " - " & xAppNome
            End If
        End If


        ValidazioneNodo = DT_Appezzamenti.Rows(0).Item("Validazione")
        xValidita_Fine = DT_Appezzamenti.Rows(0).Item("Validita_Fine")
        xValidita_Inizio = DT_Appezzamenti.Rows(0).Item("Validita_inizio")


        '---------------------------------------
        '----- Valuto i dati ottenuti ...
        '---------------------------------------

        'Inizio a costruire la descrizione
        DescrizioneNodo = ""
        DescrizioneNodo += AgroPrefix_Appezzamento
        DescrizioneNodo += xAppNome
        DescrizioneNodo += " : {" & String.Format(DT_Appezzamenti.Rows(0).Item("Sup_App"), "0.0000") & " ha}"


        'Verifico lo stato di VALIDAZIONE
        If ValidazioneNodo = "-1" Then
            DescrizioneNodo = DescrizioneNodo & " ..... (§§§ da confermare §§§)"
        End If


        '---------------------------------------
        '----- Genero il Nodo
        '---------------------------------------
        Dim xTipoNodo As Integer
        If xTipoNodoChiave = 0 Then
            xTipoNodo = enum_TipoNodo.Appezzamento
        Else
            xTipoNodo = xTipoNodoChiave
        End If



        Dim xChiave As String = ""
        'Definisco la chiave per questo elemento catasto
        Call Albero.ChiaveAlbero_Codifica_x_json(
                                Chiave:=xChiave,
                                TipoNodo:=xTipoNodo,
                                Piva:=piva,
                                Sa_Cod:=sa_cod,
                                Campo_Cod:=campocod,
                                Appezza:=appezza,
                                id_agenda:=Id_Agenda)



        'inserisco il nodo ed eventualmente i sui figli
        Dim child As Object
        If Flag_Esplodi_Tutto = True Then
            child = New List(Of AjaxTreeNodeJsonObject)
            CaricaImpianti(child, xChiave, PathRoot, xValidita_Inizio, xValidita_Fine, "", Id_Agenda, xTipoNodoChiave)
        Else
            child = New Boolean
            child = True
        End If

        If Flag_CheckBox Then
            If Not CheckBoxes.Flag_CheckBoxCentro Then
                risultato = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "jstree-no-checkboxes", "", "#", child)
            Else
                risultato = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
            End If
        Else
            risultato = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
        End If

        If Flag_Esplodi_Tutto = True Then
            risultato.state = "open"
        End If
        risultato.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Appezzamento, "")



    End Sub





    ''' <summary>
    ''' Carica le ricette
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaRicette(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)


        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Dim xAppezza As Integer
        Dim xReg_impianto As Integer


        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Appezza_Impianto_x_json(Id, xPiva, xSa_Cod, xAppezza, xReg_impianto)

        Dim DT_Ricette As DataTable
        Dim leggiRicette As New AgronicaCoreContabDAL.Ricette_R
        DT_Ricette = leggiRicette.LeggiXDestinazione(
            0,
            xPiva,
            xSa_Cod,
            xAppezza,
            xReg_impianto,
            0,
            0,
            data_Inizio, data_Fine,
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            "",
            "",
            HttpContext.Current.Session("ASG_objParametri_Server")
        )





        For Each r In DT_Ricette.Rows
            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------

            Dim DescrizioneNodo As String = r("ricetta_des")

            Dim xChiave As String = ""
            'Definisco la chiave per questo elemento catasto
            Call Albero.ChiaveAlbero_Codifica_x_json(
                                    xChiave,
                                    enum_TipoNodo.ricette_Testata,
                                    xPiva,
                                    xSa_Cod,
                                    0,
                                    xAppezza,
                                    xReg_impianto,
                                    Ricetta_Cod:=r("ricetta_cod")
                )

            Dim NodoRicetta As New AjaxTreeNodeJsonObject




            Dim child As Object
            If Flag_Esplodi_Tutto = True And Flag_Ricette Then
                child = New List(Of AjaxTreeNodeJsonObject)
                CaricaRicetteOperazioni(child, xChiave, PathRoot, data_Inizio, data_Fine)
            Else
                child = New Boolean
                child = _Flag_Ricette

            End If

            If Flag_CheckBox Then

                If Not CheckBoxes.Flag_CheckBoxRicette Then

                    NodoRicetta = New AjaxTreeNodeJsonObject(xChiave,
                                                  DescrizioneNodo, "jstree-no-checkboxes", "", "#", child)
                Else

                    NodoRicetta = New AjaxTreeNodeJsonObject(xChiave,
                                                  DescrizioneNodo, "", "", "#", child)
                End If

            Else
                NodoRicetta = New AjaxTreeNodeJsonObject(xChiave,
                                                  DescrizioneNodo, "", "", "#", child)
            End If


            Dim IconaPreferita As String = ""
            NodoRicetta.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.ricette_Testata, )

            results.Add(NodoRicetta)
        Next


    End Sub


    ''' <summary>
    ''' Carica le ricette
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaAgendaDettagli(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)


        'mi arriva la chiave del centro 
        Dim xId_agenda As Integer
        Dim xPiva As String
        Dim xSa_cod As Integer
        Dim xAppezza As Integer
        Dim xReg_impianto As Integer

        Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        'Valorizzo il Codice_Fiscale_Tecnico prendendolo dalla tabella Gruppi_Utente
        Dim Codice_Fiscale_Tecnico As String = ""
        Dim xUtenteCorrente As String = CType(objParametri_Server, AgronicaCoreDataProvider.AgronicaCoreParametri).UtenteUsername
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

        Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(xUtenteCorrente, objParametri_Utenti)


        Albero.ChiaveAlbero_Decodifica_Agenda__x_json(Id, xPiva, xSa_cod, xAppezza, xReg_impianto, xId_agenda)

        Dim DT_movimenti As DataTable
        Dim leggiDestinazioni As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        DT_movimenti = leggiDestinazioni.Leggi(
            xPiva,
            xSa_cod,
            xId_agenda,
            0,
            0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "",
             HttpContext.Current.Session("ASG_objParametri_Server")
        )

        For Each r In DT_movimenti.Rows
            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------

            Dim resultsapp As New AjaxTreeNodeJsonObject
            CaricaAppezzamento(resultsapp, PathRoot, r.Item("Piva"), r.Item("sa_cod"), r.Item("appezza"), xId_agenda, enum_TipoNodo.AgendaDestinazioni)



            Dim i As Integer
            results.Add(resultsapp)

        Next


    End Sub




    ''' <summary>
    ''' Carica le ricette
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaRicetteOperazioni(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)


        'mi arriva la chiave del centro 
        Dim xRicetta_cod As Integer
        Dim xPiva As String
        Dim xSa_cod As Integer
        Dim xAppezza As Integer
        Dim xReg_impianto As Integer


        Albero.ChiaveAlbero_Decodifica_Ricetta_cod_x_json(Id, xPiva, xSa_cod, xAppezza, xReg_impianto, xRicetta_cod)

        Dim DT_Ricette As DataTable
        Dim leggiRicette As New AgronicaCoreContabDAL.Ricette_Operazioni_R
        DT_Ricette = leggiRicette.Leggi(
            xRicetta_cod,
            0,
            0,
            0,
            data_Inizio, data_Fine,
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            "",
            "",
            HttpContext.Current.Session("ASG_objParametri_Server")
        )





        For Each r In DT_Ricette.Rows
            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------

            Dim DescrizioneNodo As String = r("ricetta_Operazione_des")

            Dim xChiave As String = ""
            'Definisco la chiave per questo elemento catasto
            Call Albero.ChiaveAlbero_Codifica_x_json(
                                    xChiave,
                                    enum_TipoNodo.ricette_Testata,
                                    xPiva,
                                    xSa_cod,
                                    0,
                                    xAppezza,
                                    xReg_impianto,
                                    Ricetta_Cod:=xRicetta_cod,
                                    ricetta_Operazione_cod:=r("ricetta_Operazione_cod")
                            )

            Dim RicettaOperazione As New AjaxTreeNodeJsonObject
            If Flag_Analisi = True Then

                Dim Livello_Analisi As New List(Of AjaxTreeNodeJsonObject)

                CaricaAnalisi(Livello_Analisi, xChiave, PathRoot, data_Inizio, data_Fine)
                If Livello_Analisi.Count > 0 Then

                    If Flag_CheckBox Then

                        If Not CheckBoxes.Flag_CheckBoxImpianto Then
                            RicettaOperazione = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "jstree-no-checkboxes", "", "#", Livello_Analisi)
                        Else
                            RicettaOperazione = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", Livello_Analisi)
                        End If
                    Else
                        RicettaOperazione = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", Livello_Analisi)
                    End If

                Else

                    If Flag_CheckBox Then

                        If Not CheckBoxes.Flag_CheckBoxRicette Then

                            RicettaOperazione = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "jstree-no-checkboxes", "", "#", False)
                        Else

                            RicettaOperazione = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", False)
                        End If

                    Else
                        RicettaOperazione = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", False)
                    End If

                End If

            Else

                RicettaOperazione = New AjaxTreeNodeJsonObject(xChiave,
                                                      DescrizioneNodo, "", "", "#", False)
            End If
            Dim IconaPreferita As String = ""
            RicettaOperazione.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.ricette_Testata, )

            results.Add(RicettaOperazione)
        Next


    End Sub

    ''' <summary>
    ''' Carica le operazioni di agenda
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaOperazioniAgenda(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)

        Dim resultsNodiFigli As New List(Of AjaxTreeNodeJsonObject)
        'genero il primo nodo
        Dim agendaTot
        Dim DescrizioneNodo As String = ""


        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Dim xPivaPadre As String = ""

        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_x_json(Id, xPiva, xSa_Cod)



        '########################
        '#####  Agenda  #####
        '########################
        Dim DT_Agenda As DataTable

        Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
        DT_Agenda = objOperazioni.Leggi_x_Albero(Piva,
                                                   Sa_Cod, data_Inizio, data_Fine,
                                                    "C",
                                                    HttpContext.Current.Session("ASG_objParametri_Server"))



        Dim Data_Movimento As String = ""
        Dim xValidita_Fine As String = ""
        Dim xDescrizione As String = ""



        Dim i As Integer
        For i = 0 To DT_Agenda.Rows.Count - 1


            '---------------------------------------
            '--- Recupero le informazioni ...
            '---------------------------------------
            Data_Movimento = CDate(DT_Agenda.Rows(i).Item("Data_Movimento")).ToShortDateString()

            xDescrizione = Data_Movimento & " - " & DT_Agenda.Rows(i).Item("Des_Lib")

            '---------------------------------------
            '----- Valuto i dati ottenuti ...
            '---------------------------------------

            'Inizio a costruire la descrizione
            DescrizioneNodo = ""
            DescrizioneNodo += AgroPrefix_OperazioniAgenda
            DescrizioneNodo += xDescrizione


            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------

            Dim xChiave As String = ""
            'Definisco la chiave per questo elemento catasto
            Call Albero.ChiaveAlbero_Codifica_x_json(
                                    xChiave,
                                    enum_TipoNodo.Agenda,
                                    Piva:=xPiva,
                                    Sa_Cod:=xSa_Cod,
                                    id_agenda:=DT_Agenda.Rows(i).Item("Id_Agenda")
                            )

            Dim opAgenda As AjaxTreeNodeJsonObject

            Dim child As Boolean
            child = True


            If Flag_CheckBox Then
                If Not CheckBoxes.Flag_CheckBoxCentro Then
                    opAgenda = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "jstree-no-checkboxes", "", "#", child)
                Else
                    opAgenda = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
                End If
            Else
                opAgenda = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
            End If

            opAgenda.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Agenda, "")

            resultsNodiFigli.Add(opAgenda)

        Next

        If resultsNodiFigli.Count > 0 Then
            DescrizioneNodo = "Agenda Operazioni "
            If Flag_CheckBox Then
                If Not CheckBoxes.Flag_CheckBoxCentro Then
                    agendaTot = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "jstree-no-checkboxes", "", "#", resultsNodiFigli)
                Else
                    agendaTot = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "", "", "#", resultsNodiFigli)
                End If
            Else
                agendaTot = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "", "", "#", resultsNodiFigli)
            End If
            If Flag_Esplodi_Tutto = True Then
                agendaTot.state = "open"
            End If

            agendaTot.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Agenda, "")
            results.Add(agendaTot)
        End If
    End Sub

    ''' <summary>
    ''' Carica Gli Impianti
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaImpianti(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date,
                                                 Optional ByVal xSementiero As String = "",
                                                 Optional ByVal xId_agenda As Integer = 0,
                                                 Optional ByVal xTipoNodoChiave As Integer = 0)

        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Dim xAppezza As Integer
        Dim xCampocod As Integer


        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Campocod_x_json(Id, xPiva, xSa_Cod, xCampocod)
        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Appezza_x_json(Id, xPiva, xSa_Cod, xAppezza)


        If Not String.IsNullOrEmpty(xSementiero) Then
            xSementiero = " Reg_Impianti.Codice_Fiscale_Tecnico = '" & xSementiero & "' "
        End If

        If FlagModalitaSementieri Then
            xSementiero = " Reg_Impianti.Codice_Fiscale_Tecnico = '" & PivaPadre & "' "
        End If


        '###################################
        '#####  Impianti #####
        '###################################

        Dim DT_Impianti As DataTable
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim filtroAgg As String
        If (Veg_Cod <> 0) Then
            filtroAgg = "SpecieVegetali.Veg_Cod =" + CStr(Veg_Cod) + IIf(xSementiero = "", "", " AND " + xSementiero)
        Else
            filtroAgg = xSementiero
        End If
        DT_Impianti = objImpianti.Leggi_xAlbero(xPiva, xSa_Cod, xAppezza, 0,
                                                           filtroAgg, "", HttpContext.Current.Session("ASG_objParametri_Server"))


        Dim i, xID_Imp, TipoNodo As Integer
        'Per ciascun elemento
        Dim xValidita_Inizio, xValidita_Fine As Date
        Dim xCul_Cod, xGru_Cod, xVeg_Cod, xDestinazioneUso_Cod, ValidazioneNodo, DescrizioneNodo As String
        For i = 0 To DT_Impianti.Rows.Count - 1
            '---------------------------------------
            '--- Recupero le informazioni ...
            '---------------------------------------

            xID_Imp = DT_Impianti.Rows(i).Item("Id_Reg")

            xCul_Cod = IIf(IsDBNull(DT_Impianti.Rows(i).Item("Cul_Cod")), 0, DT_Impianti.Rows(i).Item("Cul_Cod"))
            xGru_Cod = IIf(IsDBNull(DT_Impianti.Rows(i).Item("Gru_Cod")), 0, DT_Impianti.Rows(i).Item("Gru_Cod"))
            xVeg_Cod = IIf(IsDBNull(DT_Impianti.Rows(i).Item("Veg_Cod")), 0, DT_Impianti.Rows(i).Item("Veg_Cod"))
            xDestinazioneUso_Cod = IIf(IsDBNull(DT_Impianti.Rows(i).Item("DestinazioneUsoCodice")), 0, DT_Impianti.Rows(i).Item("DestinazioneUsoCodice"))

            xValidita_Inizio = CDate(DT_Impianti.Rows(i).Item("Validita_Inizio"))
            xValidita_Fine = CDate(DT_Impianti.Rows(i).Item("Validita_Fine"))
            ValidazioneNodo = DT_Impianti.Rows(i).Item("Validazione")

            '---------------------------------------
            '----- Valuto i dati ottenuti ...
            '---------------------------------------

            'Inizio a costruire la descrizione
            DescrizioneNodo = AgroPrefix_Impianto

            'Se il Cul_Cod e' nullo ho terreno nudo ...
            If xCul_Cod = 0 Then

                '===== TERRENO NUDO =====
                If xTipoNodoChiave = 0 Then
                    xTipoNodoChiave = enum_TipoNodo.ImpiantoNudo
                End If
                TipoNodo = enum_TipoNodo.ImpiantoNudo

                'Controllo se è laghetto, boschetto o altro..
                If Not IsDBNull(DT_Impianti.Rows(i).Item("DestinazioneUso")) Then
                    DescrizioneNodo += xValidita_Inizio.ToShortDateString
                    DescrizioneNodo += " - "
                    DescrizioneNodo += DT_Impianti.Rows(i).Item("DestinazioneUso")
                Else
                    DescrizioneNodo += xValidita_Inizio.ToShortDateString
                    DescrizioneNodo += " - "
                    DescrizioneNodo += "Terreno Nudo"
                End If

            Else

                '===== COLTURA ==========

                'Verifico il Gruppo Vegetale per scegliere l'icona
                Select Case xGru_Cod
                    Case 1
                        TipoNodo = enum_TipoNodo.ImpiantoArborea
                    Case 2
                        TipoNodo = enum_TipoNodo.ImpiantoErbacea
                    Case 3
                        TipoNodo = enum_TipoNodo.ImpiantoOrticola
                End Select

                If xTipoNodoChiave = 0 Then
                    Select Case xGru_Cod
                        Case 1
                            xTipoNodoChiave = enum_TipoNodo.ImpiantoArborea
                        Case 2
                            xTipoNodoChiave = enum_TipoNodo.ImpiantoErbacea
                        Case 3
                            xTipoNodoChiave = enum_TipoNodo.ImpiantoOrticola
                    End Select
                End If

                'Descrizione del nodo
                DescrizioneNodo += xValidita_Inizio.ToShortDateString
                DescrizioneNodo += " - "
                DescrizioneNodo += DT_Impianti.Rows(i).Item("Veg_Des")
                DescrizioneNodo += " - "
                DescrizioneNodo += DT_Impianti.Rows(i).Item("Cul_Des")

            End If

            'Informazioni sulla Superficie SUP_IMP
            'If (InfoAggiuntiveImpianto = True) Then
            DescrizioneNodo += " : {" & String.Format(CDbl(DT_Impianti.Rows(i).Item("Sup_Imp")), "0.0000") & " ha}"
            'End If

            'Verifico lo stato di VALIDAZIONE
            If ValidazioneNodo = "-1" Then
                DescrizioneNodo = DescrizioneNodo & " ..... (§§§ da confermare §§§)"
            End If

            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------

            'Icona
            Dim IconaPreferita As String = ""
            Albero.Selezionatore_Icone(IconaPreferita,
                                TipoNodo,
                                xVeg_Cod,
                                 xDestinazioneUso_Cod)




            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------

            Dim xChiave As String = ""
            'Definisco la chiave per questo elemento catasto
            Call Albero.ChiaveAlbero_Codifica_x_json(
                                    Chiave:=xChiave,
                                    TipoNodo:=xTipoNodoChiave,
                                    Piva:=xPiva,
                                    Sa_Cod:=xSa_Cod,
                                    Campo_Cod:=xCampocod,
                                    Appezza:=xAppezza,
                                    Id_Imp:=xID_Imp,
                                    id_agenda:=xId_agenda)



            'inserisco il nodo ed eventualmente i sui figli

            Dim child As Object


            If Flag_Esplodi_Tutto = True And Flag_Ricette Then
                child = New List(Of AjaxTreeNodeJsonObject)
                CaricaRicette(child, xChiave, PathRoot, data_Inizio, data_Fine)
            Else
                child = New Boolean
                child = _Flag_Ricette

            End If


            Dim Impianto As New AjaxTreeNodeJsonObject
            If Flag_Analisi = True Then

                Dim Livello_Analisi As New List(Of AjaxTreeNodeJsonObject)

                CaricaAnalisi(Livello_Analisi, xChiave, PathRoot, data_Inizio, data_Fine)
                If Livello_Analisi.Count > 0 Then

                    If Flag_CheckBox Then

                        If Not CheckBoxes.Flag_CheckBoxImpianto Then
                            Impianto = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "jstree-no-checkboxes", "", "#", Livello_Analisi)
                        Else
                            Impianto = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", Livello_Analisi)
                        End If

                    Else
                        Impianto = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", Livello_Analisi)
                    End If

                Else

                    If Flag_CheckBox Then

                        If Not CheckBoxes.Flag_CheckBoxImpianto Then

                            Impianto = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "jstree-no-checkboxes", "", "#", ChildControlsCreated)
                        Else

                            Impianto = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", child)
                        End If

                    Else
                        Impianto = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", child)
                    End If

                End If

            ElseIf Flag_Esercizio = True Then

                Dim Livello_Esercizio As New List(Of AjaxTreeNodeJsonObject)

                'INSERIRE PARTE DELL'ESERCIZIO

                CaricaEsercizio(Livello_Esercizio, xChiave, PathRoot, data_Inizio, data_Fine)
                If Livello_Esercizio.Count > 0 Then

                    If Flag_CheckBox Then

                        If Not CheckBoxes.Flag_CheckBoxImpianto Then
                            Impianto = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "jstree-no-checkboxes", "", "#", Livello_Esercizio)
                        Else
                            Impianto = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", Livello_Esercizio)
                        End If

                    Else
                        Impianto = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", Livello_Esercizio)
                    End If

                Else

                    If Flag_CheckBox Then

                        If Not CheckBoxes.Flag_CheckBoxImpianto Then

                            Impianto = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "jstree-no-checkboxes", "", "#", ChildControlsCreated)
                        Else

                            Impianto = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", child)
                        End If

                    Else
                        Impianto = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", child)
                    End If

                End If
            Else

                Impianto = New AjaxTreeNodeJsonObject(xChiave,
                                                      DescrizioneNodo, "", "", "#", child)
            End If



            Impianto.icon = PathRoot + Albero.RitornaPathImg(TipoNodo, IconaPreferita)




            If Flag_Esplodi_Tutto = True Then
                Impianto.state = "open"
            End If

            results.Add(Impianto)


        Next




    End Sub



    ''' <summary>
    ''' Carica Gli Impianti
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaImpiantiPlanning(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)

        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Dim xAppezza As Integer
        Dim xCampocod As Integer

        Dim xProgrammazione_Entita_Cod As Integer



        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Campocod_x_json(Id, xPiva, xSa_Cod, xCampocod)
        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Appezza_x_json(Id, xPiva, xSa_Cod, xAppezza)


        '###################################
        '#####  Impianti #####
        '###################################

        Dim DT_Impianti As DataTable
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        DT_Impianti = objImpianti.Leggi_xAlbero(xPiva, xSa_Cod, xAppezza, 0,
                                                            "", "", HttpContext.Current.Session("ASG_objParametri_Server"))


        Dim i, xID_Imp, TipoNodo As Integer
        'Per ciascun elemento
        Dim xValidita_Inizio, xValidita_Fine As Date
        Dim xCul_Cod, xGru_Cod, xVeg_Cod, xDestinazioneUso_Cod, ValidazioneNodo, DescrizioneNodo As String
        For i = 0 To DT_Impianti.Rows.Count - 1
            '---------------------------------------
            '--- Recupero le informazioni ...
            '---------------------------------------

            xID_Imp = DT_Impianti.Rows(i).Item("Id_Reg")

            xCul_Cod = IIf(IsDBNull(DT_Impianti.Rows(i).Item("Cul_Cod")), 0, DT_Impianti.Rows(i).Item("Cul_Cod"))
            xGru_Cod = IIf(IsDBNull(DT_Impianti.Rows(i).Item("Gru_Cod")), 0, DT_Impianti.Rows(i).Item("Gru_Cod"))
            xVeg_Cod = IIf(IsDBNull(DT_Impianti.Rows(i).Item("Veg_Cod")), 0, DT_Impianti.Rows(i).Item("Veg_Cod"))
            xDestinazioneUso_Cod = IIf(IsDBNull(DT_Impianti.Rows(i).Item("DestinazioneUsoCodice")), 0, DT_Impianti.Rows(i).Item("DestinazioneUsoCodice"))

            xValidita_Inizio = CDate(DT_Impianti.Rows(i).Item("Validita_Inizio"))
            xValidita_Fine = CDate(DT_Impianti.Rows(i).Item("Validita_Fine"))
            ValidazioneNodo = DT_Impianti.Rows(i).Item("Validazione")

            '---------------------------------------
            '----- Valuto i dati ottenuti ...
            '---------------------------------------

            'Inizio a costruire la descrizione
            DescrizioneNodo = AgroPrefix_Impianto

            'Se il Cul_Cod e' nullo ho terreno nudo ...
            If xCul_Cod = 0 Then

                '===== TERRENO NUDO =====

                TipoNodo = enum_TipoNodo.ImpiantoNudo

                'Controllo se è laghetto, boschetto o altro..
                If Not IsDBNull(DT_Impianti.Rows(i).Item("DestinazioneUso")) Then
                    DescrizioneNodo += xValidita_Inizio.ToShortDateString
                    DescrizioneNodo += " - "
                    DescrizioneNodo += DT_Impianti.Rows(i).Item("DestinazioneUso")
                Else
                    DescrizioneNodo += xValidita_Inizio.ToShortDateString
                    DescrizioneNodo += " - "
                    DescrizioneNodo += "Terreno Nudo"
                End If

            Else

                '===== COLTURA ==========

                'Verifico il Gruppo Vegetale per scegliere l'icona
                Select Case xGru_Cod
                    Case 1
                        TipoNodo = enum_TipoNodo.ImpiantoArborea
                    Case 2
                        TipoNodo = enum_TipoNodo.ImpiantoErbacea
                    Case 3
                        TipoNodo = enum_TipoNodo.ImpiantoOrticola
                End Select

                'Descrizione del nodo
                DescrizioneNodo += xValidita_Inizio.ToShortDateString
                DescrizioneNodo += " - "
                DescrizioneNodo += DT_Impianti.Rows(i).Item("Veg_Des")
                DescrizioneNodo += " - "
                DescrizioneNodo += DT_Impianti.Rows(i).Item("Cul_Des")

            End If

            'Informazioni sulla Superficie SUP_IMP
            'If (InfoAggiuntiveImpianto = True) Then
            DescrizioneNodo += " : {" & String.Format(CDbl(DT_Impianti.Rows(i).Item("Sup_Imp")), "0.0000") & " ha}"
            'End If

            'Verifico lo stato di VALIDAZIONE
            If ValidazioneNodo = "-1" Then
                DescrizioneNodo = DescrizioneNodo & " ..... (§§§ da confermare §§§)"
            End If

            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------

            'Icona
            Dim IconaPreferita As String = ""
            Albero.Selezionatore_Icone(IconaPreferita,
                                TipoNodo,
                                xVeg_Cod,
                                 xDestinazioneUso_Cod)




            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------

            Dim xChiave As String = ""
            'Definisco la chiave per questo elemento catasto
            Call Albero.ChiaveAlbero_Codifica_x_json(
                                    xChiave,
                                    TipoNodo,
                                    xPiva,
                                    xSa_Cod,
                                    xCampocod, xAppezza, xID_Imp, , , , , , , , , )

            Dim Appezzamento As New AjaxTreeNodeJsonObject
            If Flag_Analisi = True Then

                Dim Livello_Analisi As New List(Of AjaxTreeNodeJsonObject)

                CaricaAnalisi(Livello_Analisi, xChiave, PathRoot, data_Inizio, data_Fine)
                If Livello_Analisi.Count > 0 Then

                    If Flag_CheckBox Then

                        If Not CheckBoxes.Flag_CheckBoxImpianto Then
                            Appezzamento = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "jstree-no-checkboxes", "", "#", Livello_Analisi)
                        Else
                            Appezzamento = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", Livello_Analisi)
                        End If

                    Else
                        Appezzamento = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", Livello_Analisi)
                    End If

                Else

                    If Flag_CheckBox Then

                        If Not CheckBoxes.Flag_CheckBoxImpianto Then

                            Appezzamento = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "jstree-no-checkboxes", "", "#", False)
                        Else

                            Appezzamento = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", False)
                        End If

                    Else
                        Appezzamento = New AjaxTreeNodeJsonObject(xChiave,
                                                          DescrizioneNodo, "", "", "#", False)
                    End If

                End If

            Else

                Appezzamento = New AjaxTreeNodeJsonObject(xChiave,
                                                      DescrizioneNodo, "", "", "#", False)
            End If

            Appezzamento.icon = PathRoot + Albero.RitornaPathImg(TipoNodo, IconaPreferita)

            results.Add(Appezzamento)


        Next

    End Sub




    ''' <summary>
    ''' Carica la lista dei fabbricati
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaNodoFabbricati(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                                 ByVal Id As String,
                                                 ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)

        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_x_json(Id, xPiva, xSa_Cod)

        '##################################
        '#####  Fabbricati            #####
        '##################################
        Dim xChiaveListaFabbricati As String = ""
        'Definisco la chiave per questo elemento catasto
        Call Albero.ChiaveAlbero_Codifica_x_json(
                                xChiaveListaFabbricati,
                                enum_TipoNodo.x_ListaFabbricatiAziendali,
                                xPiva,
                                xSa_Cod,
                                , , , , , , , , , , , )


        Dim Fabbricati As AjaxTreeNodeJsonObject

        If Flag_CheckBox Then

            If Not CheckBoxes.Flag_CheckBoxFabbricati Then

                Fabbricati = New AjaxTreeNodeJsonObject(xChiaveListaFabbricati,
                                                  "Fabbricati Aziendali", "jstree-no-checkboxes", "", "#", True)
            Else

                Fabbricati = New AjaxTreeNodeJsonObject(xChiaveListaFabbricati,
                                                  "Fabbricati Aziendali", "", "", "#", True)
            End If

        Else

            Fabbricati = New AjaxTreeNodeJsonObject(xChiaveListaFabbricati,
                                                  "Fabbricati Aziendali", "", "", "#", True)
        End If

        Fabbricati.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.x_ListaFabbricatiAziendali, "")

        results.Add(Fabbricati)
    End Sub


    ''' <summary>
    ''' Carica la lista dei sottonodi dei fabbricati ( tutti gli elementi all'interno del fabbricato)
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaListaFabbricati(ByRef results As List(Of AjaxTreeNodeJsonObject),
                                               ByVal Id As String,
                                               ByVal PathRoot As String,
                                                 ByVal data_Inizio As Date,
                                                  ByVal data_Fine As Date)

        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_x_json(Id, xPiva, xSa_Cod)

        '##################################
        '##### Lista  Fabbricati      #####
        '##################################

        Dim DT_Fabbricati As New DataTable
        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        DT_Fabbricati = objFabbricati.Leggi(xPiva, xSa_Cod, 0, enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                            "", "", HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim i As Integer
        Dim TipoNodoFabbricato As Integer
        Dim xFabbricato_Cod, xValidita_Fine, ValidazioneNodo, xTipoFabbricato_Cod, xTipoFabbricato_Des, DescrizioneNodo As String
        For i = 0 To DT_Fabbricati.Rows.Count - 1


            '---------------------------------------
            '--- Recupero le informazioni ...
            '---------------------------------------

            xFabbricato_Cod = DT_Fabbricati.Rows(i).Item("Fabbricato_Cod")

            xValidita_Fine = DT_Fabbricati.Rows(i).Item("Validita_Fine")
            ValidazioneNodo = DT_Fabbricati.Rows(i).Item("Validazione")
            xTipoFabbricato_Cod = DT_Fabbricati.Rows(i).Item("Tipo_Fabbricato_Cod")
            xTipoFabbricato_Des = DT_Fabbricati.Rows(i).Item("Tipo_Fabbricato_Des")

            'Verifico il tipo di nodo
            TipoNodoFabbricato = Albero.TipoNodoxFabbricato(xTipoFabbricato_Cod)


            '---------------------------------------
            '----- Valuto i dati ottenuti ...
            '---------------------------------------

            DescrizioneNodo = ""
            DescrizioneNodo += DT_Fabbricati.Rows(i).Item("Tipo_Fabbricato_Des")
            DescrizioneNodo += " ("
            DescrizioneNodo += DT_Fabbricati.Rows(i).Item("Fabbricato_Des")
            DescrizioneNodo += ") "


            'Verifico lo stato di VALIDAZIONE
            If ValidazioneNodo = "-1" Then
                DescrizioneNodo = DescrizioneNodo & " ..... (§§§ da confermare §§§)"
            End If


            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------
            Dim xChiave As String = ""
            'Definisco la chiave per questo elemento catasto
            Call Albero.ChiaveAlbero_Codifica_x_json(
                                    xChiave,
                                   TipoNodoFabbricato,
                                    xPiva,
                                    xSa_Cod,
                                    , , , , , , , , , , , xFabbricato_Cod)



            Dim Fabbricati As AjaxTreeNodeJsonObject

            If Flag_CheckBox Then

                If Not CheckBoxes.Flag_CheckBoxMagazzino Then
                    Fabbricati = New AjaxTreeNodeJsonObject(xChiave,
                                                       DescrizioneNodo, "jstree-no-checkboxes", "", "#", False)
                Else
                    Fabbricati = New AjaxTreeNodeJsonObject(xChiave,
                                                       DescrizioneNodo, "", "", "#", False)
                End If

            Else
                Fabbricati = New AjaxTreeNodeJsonObject(xChiave,
                                                       DescrizioneNodo, "", "", "#", False)
            End If


            Fabbricati.icon = PathRoot + Albero.RitornaPathImg(TipoNodoFabbricato, "")



            'In funzione del tipo di nodo visualizzo il figlio appropriato
            Select Case TipoNodoFabbricato

                Case enum_TipoNodo.f_Abitazione

                    '
                    '
                    '
                    '-----

                Case enum_TipoNodo.f_CellaFrigorifera,
                     enum_TipoNodo.f_Magazzino,
                     enum_TipoNodo.f_Silos

                    '#########################################
                    '#####  MOVIMENTI MAGAZZINO  #############
                    '#########################################
                    Dim App As String = xChiave

                    '----- Inserisco un nodo GIACENZE MAGAZZINO

                    'Definisco la chiave per questo elemento catasto
                    Call Albero.ChiaveAlbero_Codifica_x_json(
                                            xChiave,
                                           enum_TipoNodo.x_GiacenzeMagazzino,
                                            xPiva,
                                            xSa_Cod,
                                            , , , , , , , , , , , xFabbricato_Cod)


                    If Flag_Analisi = True Then
                        Dim Livello_Analisi As New List(Of AjaxTreeNodeJsonObject)
                        CaricaAnalisi(Livello_Analisi, App, PathRoot, data_Inizio, data_Fine)
                        If Livello_Analisi.Count > 0 Then
                            Fabbricati.children = Livello_Analisi
                        Else
                            Fabbricati.children = New List(Of AjaxTreeNodeJsonObject)
                        End If
                    Else
                        Fabbricati.children = New List(Of AjaxTreeNodeJsonObject)
                    End If

                    If Flag_CheckBox Then

                        If Not CheckBoxes.Flag_CheckBoxGiacenze Then

                            Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                                          "Giacenze di Magazzino", "jstree-no-checkboxes", "", "#", False))
                        Else

                            Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                                          "Giacenze di Magazzino", "", "", "#", False))
                        End If

                    Else

                        Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                                          "Giacenze di Magazzino", "", "", "#", False))
                    End If



                    Fabbricati.children(Fabbricati.children.Count - 1).icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.x_GiacenzeMagazzino, "")




                    '----- Inserisco un nodo MOVIMENTI MAGAZZINO
                    Call Albero.ChiaveAlbero_Codifica_x_json(
                                           xChiave,
                                          enum_TipoNodo.x_MovimentiMagazzino,
                                           xPiva,
                                           xSa_Cod,
                                           , , , , , , , , , , , xFabbricato_Cod)

                    'Fabbricati.children = New List(Of AjaxTreeNodeJsonObject)


                    If Flag_CheckBox Then

                        If Not CheckBoxes.Flag_CheckBoxMovimenti Then

                            Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                      "Movimenti di Magazzino", "jstree-no-checkboxes", "", "#", False))
                        Else

                            Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                      "Movimenti di Magazzino", "", "", "#", False))
                        End If

                    Else

                        Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                      "Movimenti di Magazzino", "", "", "#", False))
                    End If


                    Fabbricati.children(Fabbricati.children.Count - 1).icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.x_MovimentiMagazzino, "")


                    '////////////////////////////////////
                    '///// fine MOVIMENTI MAGAZZINO /////
                    '////////////////////////////////////


                    '-----


                Case enum_TipoNodo.f_ImpiantoLavorazione

                    '##################################
                    '#####  PREPARAZIONI  #############
                    '##################################
                    Dim App As String = xChiave
                    Call Albero.ChiaveAlbero_Codifica_x_json(
                                          xChiave,
                                         enum_TipoNodo.x_PreparazioniAlimentari,
                                          xPiva,
                                          xSa_Cod,
                                          , , , , , , , , , , , xFabbricato_Cod)

                    If Flag_Analisi = True Then
                        Dim Livello_Analisi As New List(Of AjaxTreeNodeJsonObject)
                        CaricaAnalisi(Livello_Analisi, App, PathRoot, data_Inizio, data_Fine)
                        If Livello_Analisi.Count > 0 Then
                            Fabbricati.children = Livello_Analisi
                        Else
                            Fabbricati.children = New List(Of AjaxTreeNodeJsonObject)
                        End If
                    Else
                        Fabbricati.children = New List(Of AjaxTreeNodeJsonObject)
                    End If
                    Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                                             "Preparazioni Alimentari", "", "", "#", False))


                    Fabbricati.children(Fabbricati.children.Count - 1).icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.x_PreparazioniAlimentari, "")


                Case enum_TipoNodo.f_Stalla

                    '##################################
                    '#####  CONSISTENZE  ##############
                    '##################################
                    Dim App As String = xChiave
                    Call Albero.ChiaveAlbero_Codifica_x_json(
                                         xChiave,
                                        enum_TipoNodo.x_ConsistenzeAnimali,
                                         xPiva,
                                         xSa_Cod,
                                         , , , , , , , , , , , xFabbricato_Cod)


                    If Flag_Analisi = True Then
                        Dim Livello_Analisi As New List(Of AjaxTreeNodeJsonObject)
                        CaricaAnalisi(Livello_Analisi, App, PathRoot, data_Inizio, data_Fine)
                        If Livello_Analisi.Count > 0 Then
                            Fabbricati.children = Livello_Analisi
                        Else
                            Fabbricati.children = New List(Of AjaxTreeNodeJsonObject)
                        End If
                        Fabbricati.children = New List(Of AjaxTreeNodeJsonObject)
                    End If

                    Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                                                  "Consistenze Animali", "", "", "#", False))

                    Fabbricati.children(Fabbricati.children.Count - 1).icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.x_ConsistenzeAnimali, "")
                    '//////////////////////////////////
                    '///// fine CONSISTENZE  //////////
                    '//////////////////////////////////

                    '-----

            End Select


            results.Add(Fabbricati)


        Next



    End Sub



#End Region













    ''' <summary>
    ''' Per Renderizzare il controllo
    ''' </summary>
    ''' <param name="writer"></param>
    ''' <remarks></remarks>
    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        Flag_CheckBox = IIf(IsNothing(HttpContext.Current.Session("Flag_CheckBox")), False, HttpContext.Current.Session("Flag_CheckBox"))


        Dim IDDiv As String = "treeAlberoAnagrafica" & Me.ClientID

        Dim provahtml As New StringBuilder
        provahtml.Append("<div id='" + IDDiv + "'></div>")
        writer.Write(GetJS(IDDiv) + provahtml.ToString)
        MyBase.Render(writer)

    End Sub



    ''' <summary>
    ''' aggiunge lo script per identificare quale nodo è stato selezionato
    ''' il valore dell'id selezionato viene scritto nella variabile hidden "Hidden.ID"
    ''' </summary>
    ''' <param name="ID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetScriptSelect(ByVal ID As String)

        Dim StrSelect As New StringBuilder

        If (Flag_CheckBox = False) Then

            StrSelect.AppendLine("  $(document).on('click','#" + ID + " ul li a', function(){ ")
            StrSelect.AppendLine("  if ( $('.jstree-checkbox').length == 0 ){")
            StrSelect.AppendLine("          var app= $(this).parent('li').attr('id'); ")
            StrSelect.AppendLine("          $('#" & Hidden.ClientID & "').val(app); ")
            StrSelect.AppendLine("          $('#" & Hidden.ClientID & "').addClass('chiaveAlberoAnagrafica'); ")
            StrSelect.AppendLine("      } ")
            StrSelect.AppendLine("  }); ")

        End If

        Return StrSelect.ToString

    End Function


    Public Function GetJS() As String
        _Flag_JS = True
        Dim IDDiv As String = "treeAlberoAnagrafica" & Me.ClientID
        Return GetJS(IDDiv)
    End Function


    ''' <summary>
    ''' Funzione per Farsi Restituire la stringa contenente il codice JS da inserire all'interno della pagina
    ''' !!! Da estendere eventualmente con la funzione del precaricamento tramite cookies !!!
    ''' </summary>
    ''' <param name="IDDiv">ID del DIV su cui applicare il plugin</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetJS(ByVal IDDiv As String) As String
        Dim js As New StringBuilder

        Dim IdNodiAperti As String = "''"

        js.AppendLine("<script type='text/javascript'>")

        '#################
        'serializza un oggetto con dentro l'id e il Path contenente la radice (es. localhost/analisi_2010/ ) e poi aggiungere ad esempio AB_Immagini/....
        '#################
        'js.AppendLine("function OnGetNodes(n){		")
        'js.AppendLine("     var port = location.port; ")
        'js.AppendLine("     if (port != '' ) { port = ':'+ port;} ")
        'js.AppendLine("     var obj = { ")
        'js.AppendLine("         id: n.attr ? n.attr('id') : '0'")
        'js.AppendLine("         , PathRoot : 'http://' + location.hostname + port + '/' +  location.pathname.split('/')[1]  ")
        'js.AppendLine("			}")

        'js.AppendLine("			return Sys.Serialization.JavaScriptSerializer.serialize(obj); ")
        'js.AppendLine(" }")

        Dim path As String
        Dim objAgroWebConfig As New AgroWebConfig
        path = objAgroWebConfig.LinkAgronicaAgenda2010.Replace("/GestioneRichieste.aspx", "")

        ' Giulia: 20/2/2018:se su db il valore comincia con la porta, qui la devo togliere
        Dim pathSpl = path.Split("/")
        If RegularExpressions.Regex.IsMatch(pathSpl(0), ":[0-9]*$", RegexOptions.None, TimeSpan.FromSeconds(3)) Then
            Dim grandezzaPorta = pathSpl(0).Length
            path = path.Remove(0, grandezzaPorta + 1)
        End If

        js.AppendLine("function OnGetNodesAnagraficaD(n){		")
        js.AppendLine("     var port = location.port; ")
        js.AppendLine("     if (port != '' ) { port = ':'+ port;} ")

        js.AppendLine("     var obj = { ")
        js.AppendLine("         id: n.attr ? n.attr('id') : '0'")
        js.AppendLine("         , PathRoot : 'http://' + location.hostname + port + '/" & path & "'")
        js.AppendLine("			}")
        js.AppendLine("			return Sys.Serialization.JavaScriptSerializer.serialize(obj); ")
        js.AppendLine("     }")



        '#################
        'deserializzo l'oggetto che mi arriva da VB
        'all'interno dell'oggetto VB cerco il .classCSS e lo sostituisco con class
        '#################
        js.AppendLine("function OnNodesRetrievedSuccess(data,textstatus,xhr){ ")
        js.AppendLine("     var pippo = data.d; ")
        js.AppendLine("     var pippo2 = pippo.replace(/classCss/g,'class'); ")

        js.AppendLine("     $(document).trigger('NodoCaricato'); ")

        js.AppendLine("     return Sys.Serialization.JavaScriptSerializer.deserialize(pippo2); ")

        js.AppendLine("} ")


        '#################
        'se ci sono degli errori si chiama un messagbox di errore
        '#################
        js.AppendLine(" function OnNodesRetrievedError(xhr,textstatus,errorThrown){")
        js.AppendLine("     alert('ERRORE!!!' +errorThrown); ")
        js.AppendLine(" } ")


        '#################
        'Inserisce all'interno di un hidden field gli id dei nodi checkati.
        '#################
        If Flag_CheckBox Then

            js.AppendLine("function CalcolaIDCheck(_element) {")

            js.AppendLine("    var Valore = $('#" & Hidden.ClientID & "').val();")

            js.AppendLine("    if(!Valore) {Valore='';}")

            js.AppendLine("     var objChk = _element.parent().parent();")

            js.AppendLine("     ID = objChk.attr('id');")
            'js.AppendLine("         $.logThis('ID_DA_MODIFICARE: ' + ID);")
            js.AppendLine("    if (objChk.hasClass('jstree-unchecked') )")
            js.AppendLine("    {")
            'js.AppendLine("    $.logThis('unChecked');")
            js.AppendLine("         Valore = Valore.replace(ID,'');")
            js.AppendLine("         Valore = Valore.replace('||','|');")
            js.AppendLine("         if (Valore.charAt(0)=='|')")
            js.AppendLine("              Valore=Valore.substring(1, Valore.length);")
            js.AppendLine("         if (Valore.charAt(Valore.length - 1) == '|')")
            js.AppendLine("             Valore=Valore.substring(0, Valore.length - 1);")
            js.AppendLine("         $('#" & Hidden.ClientID & "').val(Valore);")
            'js.AppendLine("         $.logThis(Valore);")
            js.AppendLine("     return;")
            js.AppendLine("    }")


            js.AppendLine("    if (objChk.hasClass('jstree-checked') )")
            js.AppendLine("    {")
            js.AppendLine("         if (Valore.length > 0)")
            js.AppendLine("         {")
            js.AppendLine("             Valore= Valore +'|' ;")
            js.AppendLine("         }")
            js.AppendLine("         Valore = Valore  + ID;")
            'js.AppendLine("         $.logThis(Valore);")
            js.AppendLine("         $('#" & Hidden.ClientID & "').val(Valore);")
            'js.AppendLine("         $.logThis($('#" & Hidden.ClientID & "').val());")
            js.AppendLine("         return;")
            js.AppendLine("    }")

            js.AppendLine("} ")

        End If


        '#################
        'Applicazione del Plugin JSTREE
        '#################


        js.AppendLine("$(document).ready(function () { ")

        If Flag_Carica_Primo_Giro = True Then
            js.AppendLine("Inizializza(); ")
        End If

        js.AppendLine(" });")


        js.AppendLine("function Inizializza() { ")


        js.AppendLine("    $('#" + IDDiv + "').jstree({ ")
        If (Flag_CheckBox = False) Then
            js.AppendLine("        core : { ")
            js.AppendLine("             'initially_open' : [ " & IdNodiAperti & "] ")
            js.AppendLine("        }, ")
        End If
        'DA COMMENTARE SE L'ALBERO HA DEI CHECKBOX
        If Flag_CheckBox = False Then
            js.AppendLine("        ui : { ")
            js.AppendLine("             'initially_select' :[ '' ] ")
            'imposto il limite per la selezione multipla
            If _Flag_Singola_Selezione = True Then
                js.AppendLine("             ,  ")
                js.AppendLine("              'select_limit' : 1 ")
                js.AppendLine("        }, ")
            Else
                'selezione multipla
                js.AppendLine("        }, ")
            End If
        End If


        If Flag_CheckBox = False Then
            js.AppendLine("        plugins: ['themes', 'json_data',  'ui',  'hotkeys'],")
        Else
            js.AppendLine("        plugins: ['themes', 'json_data', 'ui',  'hotkeys', 'twostatecheckbox'],")
        End If

        js.AppendLine("        json_data: { ")
        js.AppendLine("		            ajax: {")
        js.AppendLine("                     url:    GetNameofPage() +'/GetNodesAlberoAnagrafe',")
        js.AppendLine("                     async: true,")
        js.AppendLine("                 contentType:  'application/json; charset=utf-8',")
        js.AppendLine("                 dataType:  'json',")
        js.AppendLine("                 type:   'POST',")
        js.AppendLine("                 data: function (n) { return OnGetNodesAnagraficaD(n); },")
        js.AppendLine("                 success: function (data, textstatus, xhr) {")
        js.AppendLine("                     return OnNodesRetrievedSuccess(data, textstatus, xhr)")
        js.AppendLine("		            },")
        js.AppendLine("                 error: function (xhr, textstatus, errorThrown) {")
        js.AppendLine("                     OnNodesRetrievedError(xhr, textstatus, errorThrown)")
        js.AppendLine("                 }")
        js.AppendLine("             }")
        js.AppendLine("         },")


        js.AppendLine("        themes : { ")
        js.AppendLine("        'theme' : 'apple' ")
        js.AppendLine("        } ")


        js.AppendLine("     });")

        js.AppendLine("}")




        js.AppendLine("$(document).ready(function () { ")
        'gestione del dbClick
        'js.AppendLine("     $(document).on('click', '#" + IDDiv + " ul li a', function () { ")
        ''js.AppendLine("         alert('1'); ")
        ' ''                       'controllo se il nodo è aperto o chiuso
        'js.AppendLine("         if($(this).parent().attr('class')=='jstree-open') { ")
        'js.AppendLine("         $('#" + IDDiv + "').jstree('close_node', this); } else {")
        'js.AppendLine("         $('#" + IDDiv + "').jstree('open_node', this); } ")
        'js.AppendLine("     });")


        If Flag_CheckBox = False Then
            js.AppendLine(GetScriptSelect(IDDiv))
        End If
        js.AppendLine(" });")





        js.AppendLine()
        js.AppendLine("function GetNameofPage() {")
        js.AppendLine("     var pa = new String(window.location.pathname); ")
        js.AppendLine("     var p2 = pa.split('/');")
        js.AppendLine("    return p2[p2.length - 1]; ")
        js.AppendLine("}")


        If Flag_CheckBox Then
            js.AppendLine("$(document).ready(function () { ")
            js.AppendLine(" var repeatFlag = true;")
            js.AppendLine(" repeatFlag = $(document).on('click', '.jstree-checkbox', function () { ")
            'js.AppendLine(" repeatFlag = $('.jstree-checkbox').live('click', function (repeatFlag) {")
            js.AppendLine("         if (repeatFlag) {")
            js.AppendLine("             CalcolaIDCheck($(this));")
            'js.AppendLine("             repeatFlag=false;")
            js.AppendLine("         }")
            js.AppendLine("     return repeatFlag;")
            js.AppendLine("     });")
            js.AppendLine(" });")
        End If


        js.AppendLine("</script>")
        Return js.ToString
    End Function
End Class


'Public Class CheckBoxFlags

'    Private _Flag_CheckBoxUtente As Boolean = False
'    Private _Flag_CheckBoxImpresa As Boolean = False
'    Private _Flag_CheckBoxContatti As Boolean = False
'    Private _Flag_CheckBoxParcoMacchine As Boolean = False
'    Private _Flag_CheckBoxCentro As Boolean = False
'    Private _Flag_CheckBoxCatasto As Boolean = False
'    Private _Flag_CheckBoxParticella As Boolean = False
'    Private _Flag_CheckBoxProdotti As Boolean = False
'    Private _Flag_CheckBoxFabbricati As Boolean = False
'    Private _Flag_CheckBoxMagazzino As Boolean = False
'    Private _Flag_CheckBoxGiacenze As Boolean = False
'    Private _Flag_CheckBoxMovimenti As Boolean = False
'    Private _Flag_CheckBoxAppezzamento As Boolean = False
'    Private _Flag_CheckBoxImpianto As Boolean = False
'    Private _Flag_CheckBoxCampo As Boolean = False
'    Private _Flag_CheckBoxSerra As Boolean = False
'    Private _Flag_CheckBoxAnalisi As Boolean = False
'    Private _Flag_CheckBoxCampioni As Boolean = False
'    Private _Flag_CheckBoxRicette As Boolean = False

'    Public Property Flag_CheckBoxRicette() As Boolean
'        Get
'            Return _Flag_CheckBoxRicette
'        End Get
'        Set(value As Boolean)
'            _Flag_CheckBoxRicette = value
'        End Set
'    End Property


'    Public Property Flag_CheckBoxUtente() As Boolean
'        Get
'            Return _Flag_CheckBoxUtente
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxUtente = value
'        End Set
'    End Property
'    Public Property Flag_CheckboxImpresa() As Boolean
'        Get
'            Return _Flag_CheckBoxImpresa
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxImpresa = value
'        End Set
'    End Property

'    Public Property Flag_CheckBoxContatti() As Boolean
'        Get
'            Return _Flag_CheckBoxContatti
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxContatti = value
'        End Set
'    End Property

'    Public Property Flag_CheckBoxParcoMacchine() As Boolean
'        Get
'            Return _Flag_CheckBoxParcoMacchine
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxParcoMacchine = value
'        End Set
'    End Property

'    Public Property Flag_CheckboxCentro() As Boolean
'        Get
'            Return _Flag_CheckBoxCentro
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxCentro = value
'        End Set
'    End Property

'    Public Property Flag_CheckboxCatasto() As Boolean
'        Get
'            Return _Flag_CheckBoxCatasto
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxCatasto = value
'        End Set
'    End Property

'    Public Property Flag_CheckboxParticella() As Boolean
'        Get
'            Return _Flag_CheckBoxParticella
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxParticella = value
'        End Set
'    End Property

'    Public Property Flag_CheckboxProdotti() As Boolean
'        Get
'            Return _Flag_CheckBoxProdotti
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxProdotti = value
'        End Set
'    End Property

'    Public Property Flag_CheckboxFabbricati() As Boolean
'        Get
'            Return _Flag_CheckBoxFabbricati
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxFabbricati = value
'        End Set
'    End Property

'    Public Property Flag_CheckboxMagazzino() As Boolean
'        Get
'            Return _Flag_CheckBoxMagazzino
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxMagazzino = value
'        End Set
'    End Property

'    Public Property Flag_CheckboxGiacenze() As Boolean
'        Get
'            Return _Flag_CheckBoxGiacenze
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxGiacenze = value
'        End Set
'    End Property

'    Public Property Flag_CheckboxMovimenti() As Boolean
'        Get
'            Return _Flag_CheckBoxMovimenti
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxMovimenti = value
'        End Set
'    End Property

'    Public Property Flag_CheckboxAppezzamento() As Boolean
'        Get
'            Return _Flag_CheckBoxAppezzamento
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxAppezzamento = value
'        End Set
'    End Property

'    Public Property Flag_CheckboxImpianto() As Boolean
'        Get
'            Return _Flag_CheckBoxImpianto
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxImpianto = value
'        End Set
'    End Property


'    Public Property Flag_CheckboxCampo() As Boolean
'        Get
'            Return _Flag_CheckBoxCampo
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxCampo = value
'        End Set
'    End Property

'    Public Property Flag_CheckboxSerra() As Boolean
'        Get
'            Return _Flag_CheckBoxSerra
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxSerra = value
'        End Set
'    End Property

'    Public Property Flag_CheckboxAnalisi() As Boolean
'        Get
'            Return _Flag_CheckBoxSerra
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxSerra = value
'        End Set
'    End Property

'    Public Property Flag_CheckboxCampioni() As Boolean
'        Get
'            Return _Flag_CheckBoxSerra
'        End Get
'        Set(ByVal value As Boolean)
'            _Flag_CheckBoxSerra = value
'        End Set
'    End Property

'    ' ''' <summary>
'    ' ''' Restituisce True se almeno un livello della gerarchia è di tipo checkbox.
'    ' ''' </summary>
'    ' ''' <value></value>
'    ' ''' <returns></returns>
'    ' ''' <remarks></remarks>
'    'Public ReadOnly Property HasCheckBox() As Boolean
'    '    Get
'    '        Return _Flag_CheckBoxImpresa Or _
'    '               _Flag_CheckBoxCentro Or _
'    '               _Flag_CheckBoxParticella Or _
'    '               _Flag_CheckBoxMagazzino Or _
'    '               _Flag_CheckBoxAppezzamento Or _
'    '               _Flag_CheckBoxImpianto Or _
'    '               _Flag_CheckBoxCampo Or _
'    '               _Flag_CheckBoxSerra Or _
'    '               _Flag_CheckBoxUtente
'    '    End Get
'    'End Property


'End Class
