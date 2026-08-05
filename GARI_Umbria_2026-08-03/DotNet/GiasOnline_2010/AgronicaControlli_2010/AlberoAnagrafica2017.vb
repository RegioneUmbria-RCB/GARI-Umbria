Imports System.Data
Imports System.Text
Imports System.Web.Script.Serialization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL

Public Class AlberoAnagrafica2017
    Inherits AgroControlliCommons
    Implements iAgronicaControlliCommons

    Private _alberoAnagrafica2017_headerPlaceHeader As PlaceHolder

    Public Property AlberoAnagrafica2017_headerPlaceHeader As PlaceHolder
        Get
            Return _alberoAnagrafica2017_headerPlaceHeader
        End Get
        Set(value As PlaceHolder)
            _alberoAnagrafica2017_headerPlaceHeader = value
        End Set
    End Property

    Protected Overrides Sub Inizializza()

        MyBase.inizializza()

    End Sub

    Private Sub AlberoAnagrafica2017_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        Inizializza()


        Dim PuntoInterrogativo As String = "?"

        If GiasVersioneCorrente.StartsWith("?") Then
            PuntoInterrogativo = ""
        End If

        AppendCssToHeader("", PuntoInterrogativo, PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AlberoAnagrafica2017/AlberoAnagrafica2017.css", _alberoAnagrafica2017_headerPlaceHeader)

    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        Inizializza()

        Dim IDDiv As String = "treeAlberoAnagrafica2017" & Me.ClientID

        Dim str As String = ""

        'classi generiche
        str = str & "<script type='text/javascript' src='" & PATH_GIASBASE & "agronica/Scripts/AgronicaControlli_2010/AlberoAnagrafica2017/AlberoAnagrafica2017.js'></script>"

        writer.Write(Utilita_Compressione.RemoveWhitespaceFromHtml(str))
        MyBase.Render(writer)

    End Sub

    Private Function LetturaDatiCfgDaCfgAlbero2017(
        Codice_Fiscale_Tecnico As String,
        ByVal cfg As ConfigurazioneAlbero,
        ByVal ObjParametri_server As AgronicaCoreParametri
    ) As AgronicaCoreAnagrafeDAL.AlberoAnagraficaFasterModel.AlberoAnagraficaFasterModelCfg


        ' Decido se applicare la visibilità (esistono record in tabella Utenti_Visibilita_Appoggio? E se non sono in contesto sementi, dove scattano filtro per codice_fiscale_tecnico)
        Dim UtentiVisibilitaAppoggioApplica As Boolean = False
        If String.IsNullOrEmpty(cfg.DatiSportelloSementieri) Then
            UtentiVisibilitaAppoggioApplica = LeggiGetQuery_DecidiFiltro_Utenti_Visibilita_Appoggio(ObjParametri_server)
        End If

        Dim rval As New AgronicaCoreAnagrafeDAL.AlberoAnagraficaFasterModel.AlberoAnagraficaFasterModelCfg

        rval.Piva = cfg.Piva
        rval.Sa_Cod = cfg.Sa_Cod
        rval.Elenco_Icone_SpecieVegetali = cfg.Elenco_Icone_SpecieVegetali
        rval.ApplicaFiltroUtentiVisibilitaAppoggio = UtentiVisibilitaAppoggioApplica
        rval.Flag_CatastoAziendale = cfg.Flag_CatastoAziendale
        rval.Flag_CatastoAppezzamento = cfg.Flag_CatastoAppezzamento
        rval.Flag_Planning = cfg.Flag_Planning
        rval.Flag_Ricette = cfg.Flag_Ricette
        rval.Flag_Fabbricati = cfg.Flag_Fabbricati
        rval.Flag_Anagrafica = cfg.Flag_Anagrafica
        rval.Flag_Analisi = cfg.Flag_Analisi
        rval.PivaPadre = cfg.PivaPadre
        rval.DatiSportelloSementieri = cfg.DatiSportelloSementieri
        rval.Flag_Appezzamenti_Filtra_Tecnico = cfg.Flag_Appezzamenti_Filtra_Tecnico

        rval.Codice_Fiscale_Tecnico = Codice_Fiscale_Tecnico

        Return rval

    End Function

    Private Shared Function LeggiGetQuery_DecidiFiltro_Utenti_Visibilita_Appoggio(ByVal ObjParametri_server As AgronicaCoreParametri) As Boolean

        Dim UtentiVisiblitaAppoggioLeggi As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim dt As DataTable =
            UtentiVisiblitaAppoggioLeggi.Leggi(2, "", "", ObjParametri_server)

        Return (dt.Rows.Count > 0)
    End Function

    Public Function GetNodesAlberoAnagrafeGetJsonData(
        ByVal cfg As ConfigurazioneAlbero,
        ByVal id As String, ByVal PathRoot As String,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri
    ) As RispostaStandard

        Dim results As New List(Of AjaxTreeNodeJsonObject)

        'Valorizzo il Codice_Fiscale_Tecnico prendendolo dalla tabella Gruppi_Utente
        Dim Codice_Fiscale_Tecnico As String = ""
        Dim xUtenteCorrente As String = CType(objParametri_Server, AgronicaCoreDataProvider.AgronicaCoreParametri).UtenteUsername
        Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
        Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(xUtenteCorrente, objParametri_Utenti)

        '' VAnni: 30/10/2017: viene ora passatto sulla configurazione
        'Dim data_inizio, data_fine As Date
        'data_inizio = CType(HttpContext.Current.Session("ASG_objParametri_Server"), AgronicaCoreDataProvider.AgronicaCoreParametri).FinestraTemporaleInizio
        'data_fine = CType(HttpContext.Current.Session("ASG_objParametri_Server"), AgronicaCoreDataProvider.AgronicaCoreParametri).FinestraTemporaleFine

        Dim r As New RispostaStandard
        r.RispostaOK = True

        Dim xLeggiVersioneSql As New DataProvider
        Dim majorSql As Integer = xLeggiVersioneSql.VersioneSqlServer_Major(objParametri_Server)

        'SOSTITUISCO SOLO SE LE DATE SONO PIU' STRINGENTI DI QUELLE DELLA VISIBILITA' UTENTE (finestraTemporale)
        If objParametri_Server.FinestraTemporaleInizio <> AGRODATAINIZIO AndAlso
                cfg.dataInizio.ToLocalTime() > objParametri_Server.FinestraTemporaleInizio Then
            objParametri_Server.FinestraTemporaleInizio = cfg.dataInizio.ToLocalTime()
            objParametri_Utenti.FinestraTemporaleInizio = cfg.dataInizio.ToLocalTime()
        Else
            cfg.dataInizio = objParametri_Server.FinestraTemporaleInizio
        End If

        If objParametri_Server.FinestraTemporaleFine <> AGRODATAFINE AndAlso
                cfg.dataFine.ToLocalTime() < objParametri_Server.FinestraTemporaleFine Then
            objParametri_Server.FinestraTemporaleFine = cfg.dataFine.ToLocalTime()
            objParametri_Utenti.FinestraTemporaleFine = cfg.dataFine.ToLocalTime()
        Else
            cfg.dataFine = objParametri_Server.FinestraTemporaleFine
        End If

        If cfg.LetturaViaSQLJson And majorSql >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            'lettura via Query con clausola for json path, se richiesta da cfg e supportata dal motore di database

            Dim numeroAliasImpostati As Integer = 0

            Dim letturaCfg As AgronicaCoreAnagrafeDAL.AlberoAnagraficaFasterModel.AlberoAnagraficaFasterModelCfg =
                LetturaDatiCfgDaCfgAlbero2017(Codice_Fiscale_Tecnico, cfg, objParametri_Server)

            Dim AgrLeggiAnag As New AgronicaCoreAnagrafeDAL.AlberoAnagraficaFaster_R
            r.RispostaStringa = AgrLeggiAnag.LeggiViaJsonSQL(
                letturaCfg,
                NumeroDiAliasImpostati:=numeroAliasImpostati,
                objParametri_Server:=objParametri_Server, objParametri_Utenti:=objParametri_Utenti)

            r.RispostaStringa = r.RispostaStringa.Replace("\/", "/")

            'sostituisco tutti gli alias utilizzati nel json
            For i As Integer = 0 To numeroAliasImpostati
                r.RispostaStringa = r.RispostaStringa.Replace("],""items" & AgrLeggiAnag.SuffissoAlias & i & """:[", ",")
            Next
            For i As Integer = 0 To numeroAliasImpostati
                r.RispostaStringa = r.RispostaStringa.Replace(AgrLeggiAnag.SuffissoAlias & i, "")
            Next
        Else
            'lettura standard


            If String.IsNullOrEmpty(id) Or id = "0" Then

                'controllo se devo precaricare il nodo delle analisi
                If cfg.Flag_Analisi = True Then
                    PrecaricaNodoAnalisi(cfg.Piva, objParametri_Server)
                    PrecaricaNodoCampioni(cfg.Piva, objParametri_Server)
                End If

                CaricaPrimi3Livelli(cfg, objParametri_Server, objParametri_Utenti, results, id, PathRoot, cfg.dataInizio, cfg.dataFine)
                'CaricaFinoALivelloX(results, 2, id, PathRoot, cfg.dataInizio, cfg.dataFine)

            Else

                'decodifico la chiave che mi arriva
                Dim Chiave As Integer
                Albero.ChiaveAlbero_Decodifica_TipoNodo_x_json(id, Chiave)

                Select Case Chiave

                    Case enum_TipoNodo.Centro
                        CaricaInfoDentroACentro(cfg, results, objParametri_Server, objParametri_Utenti, id, PathRoot, cfg.dataInizio, cfg.dataFine)

                    Case enum_TipoNodo.CatastoAziendale
                        'carico le 'richieste le particelle
                        CaricaParticelle(cfg, results, objParametri_Server, id, PathRoot, cfg.dataInizio, cfg.dataFine)

                    Case enum_TipoNodo.PlanningTestata
                        'CaricaPlanningDettaglio(results, id, PathRoot)
                        'Case enum_TipoNodo.PlanningEntita
                        CaricaPlanningDettaglioEntita(cfg, results, objParametri_Server, id, PathRoot, cfg.dataInizio, cfg.dataFine)

                    Case enum_TipoNodo.x_ListaFabbricatiAziendali
                        'carico la lista dei fabbricati
                        CaricaListaFabbricati(cfg, results, objParametri_Server, id, PathRoot, cfg.dataInizio, cfg.dataFine)

                    Case enum_TipoNodo.Campo
                        If cfg.Flag_Analisi = True Then
                            CaricaAnalisi(cfg, results, objParametri_Server, id, PathRoot, cfg.dataInizio, cfg.dataFine)
                        End If
                        Dim xCampoCod As Integer
                        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Campocod_x_json(id, "", 0, xCampoCod)
                        CaricaAppezzamenti(cfg, results, objParametri_Server, id, PathRoot, xCampoCod, Codice_Fiscale_Tecnico, cfg.dataInizio, cfg.dataFine)

                    Case enum_TipoNodo.Serra
                        If cfg.Flag_Analisi = True Then
                            CaricaAnalisi(cfg, results, objParametri_Server, id, PathRoot, cfg.dataInizio, cfg.dataFine)
                        End If
                        Dim xCampoCod As Integer
                        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Campocod_x_json(id, "", 0, xCampoCod)
                        CaricaAppezzamenti(cfg, results, objParametri_Server, id, PathRoot, xCampoCod, Codice_Fiscale_Tecnico, cfg.dataInizio, cfg.dataFine)

                    Case enum_TipoNodo.Appezzamento
                        If cfg.Flag_Analisi = True Then
                            CaricaAnalisi(cfg, results, objParametri_Server, id, PathRoot, cfg.dataInizio, cfg.dataFine)
                        End If
                        CaricaImpianti(cfg, results, objParametri_Server, id, PathRoot, cfg.dataInizio, cfg.dataFine)

                    'Case enum_TipoNodo.ImpiantoArborea, _
                    '     enum_TipoNodo.ImpiantoErbacea, _
                    '     enum_TipoNodo.ImpiantoOrticola, _
                    '     enum_TipoNodo.ImpiantoNudo
                    '    CaricaPianoConcimazioni(results, id, PathRoot, cfg.dataInizio, cfg.dataFine)

                    Case enum_TipoNodo.Impianto_Generico, enum_TipoNodo.ImpiantoArborea, enum_TipoNodo.ImpiantoErbacea, enum_TipoNodo.ImpiantoNudo, enum_TipoNodo.ImpiantoOrticola
                        CaricaRicette(cfg, results, objParametri_Server, id, PathRoot, cfg.dataInizio, cfg.dataFine)

                    Case enum_TipoNodo.ricette_Testata
                        CaricaRicetteOperazioni(cfg, results, objParametri_Server, id, PathRoot, cfg.dataInizio, cfg.dataFine, enum_TipoRicetta.Non_Filtrare)

                    Case enum_TipoNodo.Agenda
                        CaricaAgendaDettagli(cfg, results, objParametri_Server, objParametri_Utenti, id, PathRoot, cfg.dataInizio, cfg.dataFine)

                    'Case esercizio
                    Case enum_TipoNodo.DistintaDiProduzione
                        CaricaPianoConcimazioni(cfg, results, objParametri_Server, id, PathRoot, cfg.dataInizio, cfg.dataFine)

                End Select
            End If

            Dim kendoHierarch As New KendoHierarchicalDataSource
            AgronicaControlli_2010.AjaxTreeNodeJsonObjectConverter.AjaxTreeNodeJsonObject_KendoHierarchical(results, kendoHierarch)

            Dim ser As New JavaScriptSerializer()
            ser.MaxJsonLength = 50000000

            r.RispostaStringa = "[" & ser.Serialize(kendoHierarch) & "]"

        End If
        'tipo lettura dati

        Return r

    End Function

    ''' <summary>
    ''' funzione per precaricare il nodo delle analisi
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PrecaricaNodoAnalisi(piva As String, objParametri_Server As AgronicaCoreParametri)
        Dim DT As DataTable
        Dim objAnalisi As New AgronicaCoreAnagrafeDAL.Analisi_EntitaxTestata_R
        ' carico solo le concimazioni del terreno e dei residui

        DT = objAnalisi.LeggixPrecaricaAlbero(piva, "(Analisi_Testata_Tipo=1 OR Analisi_Testata_Tipo=7)", "", objParametri_Server)

        HttpContext.Current.Session("DT_Analisi") = DT
    End Sub

    ''' <summary>
    ''' funzione per precaricare il nodo dei campioni
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PrecaricaNodoCampioni(piva As String, objParametri_Server As AgronicaCoreParametri)
        Dim DT As DataTable
        Dim objcampioni As New AgronicaCoreAnagrafeDAL.Analisi_CampionexDet_R
        DT = objcampioni.LeggixPrecaricaAlbero(piva, "", "", objParametri_Server)
        HttpContext.Current.Session("DT_Campioni") = DT
    End Sub

    ''' <summary>
    ''' Carica il nodo Utente, Impresa, Contatti, Parco Macchine e la lista dei Centri Aziendali
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaPrimi3Livelli(
        ByVal cfg As ConfigurazioneAlbero,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
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

        DTUtente = objUtente.Leggi(objParametri_Server.UtenteUsername,
                                   5,
                                   enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                   "", "", objParametri_Utenti)
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

        Dim prefixUtente = "{" & My.Resources.AgronicaControlli_2010.Utente & "}"
        If cfg.Flag_CheckBox Then
            If Not cfg.CheckBoxes.Flag_CheckBoxUtente Then
                Radice = New AjaxTreeNodeJsonObject(xChiave, prefixUtente & Testo, "jstree-no-checkboxes PIPPO", "", "#", Livello_Impresa)
            Else
                Radice = New AjaxTreeNodeJsonObject(xChiave, prefixUtente & Testo, "", "", "#", Livello_Impresa)
            End If
        Else
            Radice = New AjaxTreeNodeJsonObject(xChiave, prefixUtente & Testo, "", "", "#", Livello_Impresa)
        End If

        Radice.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Utente, "")
        Radice.state = "open"
        Radice.type = "utente"
        '======================================================
        '===== Verifica l'uscita dalla routine
        '======================================================
        'Se la partita IVA e' nulla allora esco
        If cfg.Piva = "" Then
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
        DTImprese = objImprese.Leggi(CStr(cfg.Piva),
                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     "", "", objParametri_Server)
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
                                    PivaPadre:=cfg.PivaPadre
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

        Dim prefixImpresa = "{" & My.Resources.AgronicaControlli_2010.Impresa & "}"
        If cfg.Flag_CheckBox Then

            If Not cfg.CheckBoxes.Flag_CheckBoxImpresa Then
                Livello_Impresa.Add(New AjaxTreeNodeJsonObject(xChiave, prefixImpresa & xRag_Soc, "jstree-no-checkboxes", "", "#", Livello2))
            Else
                Livello_Impresa.Add(New AjaxTreeNodeJsonObject(xChiave, prefixImpresa & xRag_Soc, "", "", "#", Livello2))
            End If

        Else
            Livello_Impresa.Add(New AjaxTreeNodeJsonObject(xChiave, prefixImpresa & xRag_Soc, "", "", "#", Livello2))
        End If

        Livello_Impresa(Livello_Impresa.Count - 1).icon = PathRoot + Albero.RitornaPathImg(TipoImpresa)
        Livello_Impresa(Livello_Impresa.Count - 1).state = "open"
        Livello_Impresa(Livello_Impresa.Count - 1).type = "impresa"

        '################################## 
        '#####  ANALISI x impresa #########
        '################################## 

        If cfg.Flag_Analisi = True Then
            CaricaAnalisi(cfg, Livello2, objParametri_Server, xChiave, PathRoot, data_Inizio, data_Fine)
        End If

        '################################## 
        '#####  PLANNING Testata  #########
        '################################## 

        If cfg.Flag_Planning = True Then
            CaricaPlanningTestata(cfg, Livello2, objParametri_Server, xChiave, PathRoot, data_Inizio, data_Fine)
        End If

        '################################## 
        '#####  CONTATTI  #################
        '################################## 

        If (cfg.Flag_Contatti = True) Then
            Dim xChiaveContatti As String = ""
            Call Albero.ChiaveAlbero_Codifica_x_json(xChiaveContatti,
                                        enum_TipoNodo.x_Contatti,
                                        xPiva,
                                        , , , , , , , , , , , , )
            
            Dim prefixContatti = My.Resources.AgronicaControlli_2010.Contatti
            If cfg.Flag_CheckBox Then
                If Not cfg.CheckBoxes.Flag_CheckBoxContatti Then

                    Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveContatti, prefixContatti, "jstree-no-checkboxes", "", "#", False))
                Else

                    Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveContatti, prefixContatti, "", "", "#", False))
                End If

            Else

                Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveContatti, prefixContatti, "", "", "#", False))

            End If

            Livello2(Livello2.Count - 1).icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.x_Contatti)
            Livello2(Livello2.Count - 1).type = "contatti"
        End If

        '##################################
        '#####  PARCO MACCHINE  ###########
        '##################################

        If (cfg.Flag_ParcoMacchine = True) Then
            Dim xChiaveParcoMacchine As String = ""
            Call Albero.ChiaveAlbero_Codifica_x_json(xChiaveParcoMacchine,
                                        enum_TipoNodo.x_ParcoMacchine,
                                        xPiva,
                                        , , , , , , , , , , , , )

            Dim prefixParcoMacchine = My.Resources.AgronicaControlli_2010.ParcoMacchine
            If cfg.Flag_CheckBox Then

                If Not cfg.CheckBoxes.Flag_CheckBoxParcoMacchine Then

                    Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveParcoMacchine, prefixParcoMacchine, "jstree-no-checkboxes", "", "#", False))
                Else

                    Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveParcoMacchine, prefixParcoMacchine, "", "", "#", False))
                End If

            Else

                Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveParcoMacchine, prefixParcoMacchine, "", "", "#", False))

            End If

            Livello2(Livello2.Count - 1).icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.x_ParcoMacchine)
            Livello2(Livello2.Count - 1).type = "macchine"
        End If

        '####################
        '#####  CENTRI  #####
        '####################

        Dim DT_Centri As New DataTable
        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim str_aggiuntivo_x_specie As String
        If Not IsNothing(cfg.DatiSportelloSementieri) AndAlso (cfg.DatiSportelloSementieri <> "-1") Then
            Dim v = cfg.DatiSportelloSementieri.split("|")(4)
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

        DT_Centri = objCentri.Leggi(
            CStr(cfg.Piva),
            cfg.Sa_Cod,
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            str_aggiuntivo_x_specie, "", objParametri_Server)

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
                                        PivaPadre:=cfg.PivaPadre
                                        )

            'inserisco il nodo ed eventualmente i sui figli
            Dim child As Object

            'Grilli 02-05-2018 Commentato perché anche se non voglio esplodere tutto però voglio caricare i dati
            'If cfg.Flag_Esplodi_Tutto = True Then
            child = New List(Of AjaxTreeNodeJsonObject)
            CaricaInfoDentroACentro(cfg, child, objParametri_Server, objParametri_Utenti, xChiaveCentro, PathRoot, data_Inizio, data_Fine)
            'Else
            '    child = New Boolean
            '    child = True
            'End If

            If cfg.Flag_CheckBox Then
                If Not cfg.CheckBoxes.Flag_CheckBoxCentro Then
                    Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveCentro, DescrizioneNodo, "jstree-no-checkboxes", "", "#", child))
                Else
                    Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveCentro, DescrizioneNodo, "", "", "#", child))
                End If
            Else
                Livello2.Add(New AjaxTreeNodeJsonObject(xChiaveCentro, DescrizioneNodo, "", "", "#", child))
            End If

            If cfg.Flag_Esplodi_Tutto = True Then
                Livello2(Livello2.Count - 1).state = "open"
            End If
            Livello2(Livello2.Count - 1).icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Centro)
            Livello2(Livello2.Count - 1).type = "centri"
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
    Public Sub CaricaInfoDentroACentro(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objparametri_Server As AgronicaCoreParametri,
        ByVal objparametri_Utenti As AgronicaCoreParametri,
        ByVal Id As String,
        ByVal PathRoot As String,
        ByVal data_Inizio As Date,
        ByVal data_Fine As Date)

        If cfg.Flag_Analisi = True Then
            CaricaAnalisi(cfg, results, objparametri_Server, Id, PathRoot, data_Inizio, data_Fine)
        End If
        'per caricare il catasto
        If cfg.Flag_CatastoAziendale = True Then
            CaricaCatasto(cfg, results, objparametri_Server, Id, PathRoot, data_Inizio, data_Fine)
        End If

        If cfg.Flag_Planning = True Then
            CaricaPlanningTestata(cfg, results, objparametri_Server, Id, PathRoot, data_Inizio, data_Fine)
        End If

        '################################## 
        '#####  Operazioni Agenda #########
        '################################## 

        If cfg.Flag_Agenda = True Then
            CaricaOperazioniAgenda(cfg, results, objparametri_Server, objparametri_Utenti, Id, PathRoot, data_Inizio, data_Fine)
        End If

        'portafoglio prodotti
        If cfg.Flag_PortafoglioProdotti = True Then
            CaricaPortafoglioProdotti(cfg, results, Id, PathRoot, data_Inizio, data_Fine)
        End If
        'fabbricati
        If cfg.Flag_Fabbricati = True Then
            CaricaNodoFabbricati(cfg, results, objparametri_Server, Id, PathRoot, data_Inizio, data_Fine)
        End If

        If cfg.Flag_Anagrafica = True Then

            Dim listaAnag As New List(Of AjaxTreeNodeJsonObject)
            'Carico i Campi
            CaricaNodiCampi(cfg, listaAnag, objparametri_Server, objparametri_Utenti, Id, PathRoot, data_Inizio, data_Fine)

            'appezzamenti sfusi
            Dim xSementiero As String = ""
            If Not cfg.DatiSportelloSementieri <> "" AndAlso
                cfg.DatiSportelloSementieri <> "-1" Then
                xSementiero = cfg.PivaPadre
            End If

            'Valorizzo il Codice_Fiscale_Tecnico prendendolo dalla tabella Gruppi_Utente
            Dim Codice_Fiscale_Tecnico As String = ""
            Dim xUtenteCorrente As String = objparametri_Server.UtenteUsername
            Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
            Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(xUtenteCorrente, objparametri_Utenti)

            CaricaAppezzamenti(cfg, listaAnag, objparametri_Server, Id, PathRoot, 0, Codice_Fiscale_Tecnico, data_Inizio, data_Fine, xSementiero)

            If listaAnag.Count > 0 Then

                Dim opAnagrafica As AjaxTreeNodeJsonObject

                'i18n__
                Dim DescrizioneNodo As String = My.Resources.AgronicaControlli_2010.Anagrafica
                If cfg.Flag_CheckBox Then
                    If Not cfg.CheckBoxes.Flag_CheckBoxCentro Then
                        opAnagrafica = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "jstree-no-checkboxes", "", "#", listaAnag)
                    Else
                        opAnagrafica = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "", "", "#", listaAnag)
                    End If
                Else
                    opAnagrafica = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "", "", "#", listaAnag)
                End If
                If cfg.Flag_Esplodi_Tutto = True Then
                    opAnagrafica.state = "open"
                End If

                opAnagrafica.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Anagrafica_Generica, "")
                opAnagrafica.type = "anagrafica"
                opAnagrafica.attr.id = enum_TipoNodo.Anagrafica_Generica
                results.Add(opAnagrafica)

            End If

        End If

    End Sub

    ''' <summary>
    ''' Per il caricamento del catasto aziendale 
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaCatasto(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
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

        'i18n__
        Dim prefixCatastoAziendale = My.Resources.AgronicaControlli_2010.CatastoAziendale
        If cfg.Flag_CheckBox Then
            If Not cfg.CheckBoxes.Flag_CheckBoxCatasto Then
                Catasto = New AjaxTreeNodeJsonObject(xChiaveCatasto,
                                                  prefixCatastoAziendale, "jstree-no-checkboxes", "", "#", Livello_Particelle)
            Else
                Catasto = New AjaxTreeNodeJsonObject(xChiaveCatasto,
                                                  prefixCatastoAziendale, "", "", "#", Livello_Particelle)
            End If
        Else
            Catasto = New AjaxTreeNodeJsonObject(xChiaveCatasto,
                                                  prefixCatastoAziendale, "", "", "#", Livello_Particelle)
        End If

        Catasto.icon = PathRoot & Albero.RitornaPathImg(enum_TipoNodo.CatastoAziendale, "")
        Catasto.type = "catasto"

        Dim child As New List(Of AjaxTreeNodeJsonObject)
        CaricaParticelle(cfg, child, objParametri_Server, Id, PathRoot, data_Inizio, data_Fine)
        Catasto.children.AddRange(child)

        results.Add(Catasto)
    End Sub

    ''' <summary>
    ''' Per il caricamento delle Particelle
    ''' </summary>
    Public Sub CaricaParticelle(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal Id As String,
        ByVal PathRoot As String,
        ByVal data_Inizio As Date,
        ByVal data_Fine As Date)

        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Dim xAppezza As Integer

        ' VAnni: 16/7/2020: decodifica anche appezza..
        'Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_x_json(Id, xPiva, xSa_Cod)
        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Appezza_x_json(Id, xPiva, xSa_Cod, xAppezza)

        '########################
        '#####  PARTICELLE  #####
        '########################
        Dim DT_Particelle As DataTable
        Dim objParticelle As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R
        DT_Particelle = objParticelle.LeggixAlberoAnagrafica(
            xPiva,
            xSa_Cod,
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            "", "",
            objParametri_Server,
            xAppezza)

        Dim xValidita_Fine As String = ""
        Dim ValidazioneNodo As String = ""
        Dim stbDescrizioneNodo As New StringBuilder
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
            stbDescrizioneNodo.Length = 0

            stbDescrizioneNodo.Append(AgroPrefix_Particella)
            stbDescrizioneNodo.Append("{")


            If cfg.Contesto = ConfigurazioneAlbero.enum_Contesto.GIS Then

                stbDescrizioneNodo.Append(DT_Particelle.Rows(i).Item("ISTAT_PROVINCIA"))
                stbDescrizioneNodo.Append(" : ")
                stbDescrizioneNodo.Append(DT_Particelle.Rows(i).Item("ISTAT_COD_BELFIORE"))
                stbDescrizioneNodo.Append(" : ")
                stbDescrizioneNodo.Append(DT_Particelle.Rows(i).Item("ISTAT_COMUNE"))

            Else

                stbDescrizioneNodo.Append(DT_Particelle.Rows(i).Item("Prov"))
                stbDescrizioneNodo.Append(" : ")
                stbDescrizioneNodo.Append(DT_Particelle.Rows(i).Item("Com"))

            End If

            stbDescrizioneNodo.Append(" : ")
            If DT_Particelle.Rows(i).Item("Sezione").ToString = "0" Then
                stbDescrizioneNodo.Append("__")
            Else
                stbDescrizioneNodo.Append(Right("__" & DT_Particelle.Rows(i).Item("Sezione").ToString, 2))
            End If

            stbDescrizioneNodo.Append(" : ")
            stbDescrizioneNodo.Append(Right("______" & DT_Particelle.Rows(i).Item("Foglio").ToString, 6))
            stbDescrizioneNodo.Append(" : ")
            stbDescrizioneNodo.Append(Right("______" & DT_Particelle.Rows(i).Item("Numero").ToString, 6))

            stbDescrizioneNodo.Append(" : ")
            If DT_Particelle.Rows(i).Item("Subalterno").ToString = "0" Then
                stbDescrizioneNodo.Append("__")
            Else
                stbDescrizioneNodo.Append(Right("__" & DT_Particelle.Rows(i).Item("Subalterno").ToString, 2))
            End If

            stbDescrizioneNodo.Append("}")
            stbDescrizioneNodo.Append(" ..... ")
            stbDescrizioneNodo.Append(CStr(Int(DT_Particelle.Rows(i).Item("Ettari"))))
            stbDescrizioneNodo.Append(",")
            stbDescrizioneNodo.Append(Right("00" & DT_Particelle.Rows(i).Item("Are").ToString, 2))
            stbDescrizioneNodo.Append(Right("00" & DT_Particelle.Rows(i).Item("Centiare").ToString, 2))
            stbDescrizioneNodo.Append(" [ha]")

            stbDescrizioneNodo.Append(" ..... ")
            stbDescrizioneNodo.Append(TitoloPossessoDes_from_TitoloPossessoCod(
                                    DT_Particelle.Rows(i).Item("TitoloPossesso")))

            'Verifico lo stato di VALIDAZIONE
            If ValidazioneNodo = "-1" Then
                stbDescrizioneNodo.Append(" ..... (§§§ da confermare §§§)")
            End If

            DescrizioneNodo = stbDescrizioneNodo.ToString

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
            If cfg.Flag_Analisi = True Then
                Dim Livello_Analisi As New List(Of AjaxTreeNodeJsonObject)

                CaricaAnalisi(cfg, Livello_Analisi, objParametri_Server, xChiave, PathRoot, data_Inizio, data_Fine)
                If Livello_Analisi.Count > 0 Then
                    If cfg.Flag_CheckBox Then
                        If Not cfg.CheckBoxes.Flag_CheckBoxParticella Then
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

                    If cfg.Flag_CheckBox Then
                        If Not cfg.CheckBoxes.Flag_CheckBoxParticella Then
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

                If cfg.Flag_CheckBox Then
                    If Not cfg.CheckBoxes.Flag_CheckBoxParticella Then
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
            Particella.type = "particella"
            results.Add(Particella)

        Next

    End Sub

    '################################################################################
    Public Shared Function TitoloPossessoDes_from_TitoloPossessoCod(ByVal TitoloPossessoCod As Integer) As String

        Dim Des As String

        'i18n__
        Select Case TitoloPossessoCod

            Case enum_TitoloPossesso.Altro
                Des = "Altro"
            Case enum_TitoloPossesso.Proprieta
                Des = "Proprietà"
            Case enum_TitoloPossesso.Comodato
                Des = "Comodato d'uso"
            Case enum_TitoloPossesso.AffittoContratto
                Des = "Affitto con contratto"
            Case enum_TitoloPossesso.AffittoSenzaContratto
                Des = "Affitto senza contratto"
            Case enum_TitoloPossesso.InContoTerzi
                Des = "In conto terzi"
            Case enum_TitoloPossesso.InConvenzione
                Des = "In convenzione"
            Case enum_TitoloPossesso.InCompartecipazione
                Des = "In compartecipazione"
            Case Else
                Des = "Altro (non definito)"
        End Select

        'Restituisco il risultato
        Return Des

    End Function

#Region "Planning"

    ''' <summary>
    ''' Per il caricamento dei Planning
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaPlanningTestata(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
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

        '' VAnni: 30/10/2017: verificare bene questa lettura
        Dim xSementiero As String = ""
        If cfg.FlagModalitaSementieri Then
            xSementiero = cfg.PivaPadre
        End If

        '########################
        '#####  PLANNING  #####
        '########################
        Dim DT_Planning As DataTable
        Dim objPlanning As New AgronicaCoreAnagrafeDAL.Programmazione_Testata_R

        DT_Planning = objPlanning.Leggi_X_Albero(0, xSementiero, xPiva,
                                          xSa_Cod, data_Inizio, data_Fine, "", "",
                                          objParametri_Server)

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
            DescrizioneNodo &= "{" & My.Resources.AgronicaControlli_2010.Plan & "} : "
            DescrizioneNodo &= xDescrizione

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
                                    Programmazione_Cod:=DT_Planning.Rows(i).Item("programmazione_cod"))

            Dim planning As AjaxTreeNodeJsonObject

            'inserisco il nodo ed eventualmente i sui figli
            Dim child As Object
            If cfg.Flag_Esplodi_Tutto = True Then
                child = New List(Of AjaxTreeNodeJsonObject)
                'CaricaPlanningDettaglio(child, xChiave, PathRoot)
                CaricaPlanningDettaglioEntita(cfg, child, objParametri_Server, xChiave, PathRoot, data_Inizio, data_Fine)
            Else
                child = New Boolean
                child = True
            End If

            If cfg.Flag_CheckBox Then
                If Not cfg.CheckBoxes.Flag_CheckBoxCentro Then
                    planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "jstree-no-checkboxes", "", "#", child)
                Else
                    planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
                End If
            Else
                planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
            End If

            If cfg.Flag_Esplodi_Tutto = True Then
                planning.state = "open"
            End If
            planning.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.PlanningTestata, "")
            planning.type = "planning"
            listaPlan.Add(planning)

        Next

        If listaPlan.Count > 0 Then

            Dim planTot As New AjaxTreeNodeJsonObject
            DescrizioneNodo = "Planning"
            If cfg.Flag_CheckBox Then
                If Not cfg.CheckBoxes.Flag_CheckBoxCentro Then
                    planTot = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "jstree-no-checkboxes", "", "#", listaPlan)
                Else
                    planTot = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "", "", "#", listaPlan)
                End If
            Else
                planTot = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "", "", "#", listaPlan)
            End If
            If cfg.Flag_Esplodi_Tutto = True Then
                planTot.state = "open"
            End If

            planTot.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.PlanningTestata, "")
            planTot.type = "planningTot"
            planTot.attr.id = enum_TipoNodo.PlanningTotale
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
    Public Sub CaricaPlanningDettaglio(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal Id As String,
        ByVal PathRoot As String,
        ByVal data_Inizio As Date,
        ByVal data_Fine As Date
    )

        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Dim Programmazione_Cod As Integer
        Dim Programmazione_Entita_Cod As Integer
        Albero.ChiaveAlbero_Decodifica_PlanningEntita_x_json(Id, xPiva, xSa_Cod, Programmazione_Cod, Programmazione_Entita_Cod)

        '########################
        '### PLANNING ENTITA' ###
        '########################

        Dim DT_Planning As DataTable
        Dim objPlanning As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

        DT_Planning = objPlanning.Leggi(
            Programmazione_Cod, Programmazione_Entita_Cod,
            "", xPiva, xSa_Cod, 0, 0, 0, 0, data_Inizio, data_Fine, "", "", "",
            objParametri_Server)

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
            DescrizioneNodo &= AgroPrefix_DettaglioPlanning
            DescrizioneNodo &= xDescrizione

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
            If cfg.Flag_Esplodi_Tutto = True Then
                child = New List(Of AjaxTreeNodeJsonObject)
                CaricaPlanningDettaglioEntita(cfg, child, objParametri_Server, xChiave, PathRoot, data_Inizio, data_Fine)
            Else
                child = New Boolean
                child = True
            End If

            If cfg.Flag_CheckBox Then
                If Not cfg.CheckBoxes.Flag_CheckBoxCentro Then
                    planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "jstree-no-checkboxes", "", "#", child)
                Else
                    planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
                End If
            Else
                planning = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
            End If

            If cfg.Flag_Esplodi_Tutto = True Then
                planning.state = "open"
            End If
            planning.icon = PathRoot & Albero.RitornaPathImg(enum_TipoNodo.PlanningEntita, "")
            'nodo aggiunto

            results.Add(planning)
        Next

    End Sub

    ''' <summary>
    ''' Per il caricamento dei Planning
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaPlanningDettaglioEntita(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
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
        '### PLANNING ENTITA' ###
        '########################

        Dim DT_Planning As DataTable
        Dim objPlanning As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R

        DT_Planning = objPlanning.Leggi(Programmazione_Cod, Programmazione_Entita_Cod,
                                          "", xPiva, xSa_Cod, 0, 0, 0, 0, data_Inizio, data_Fine, "", "", "",
                                          objParametri_Server)

        Dim xDescrizione As String = ""

        Dim stbDescrizioneNodo As New StringBuilder
        Dim DescrizioneNodo As String

        Dim i As Integer
        For i = 0 To DT_Planning.Rows.Count - 1

            stbDescrizioneNodo.Length = 0

            Dim xSupPlan As String = IIf(IsDBNull(DT_Planning.Rows(i).Item("Superficie")), "", DT_Planning.Rows(i).Item("Superficie"))
            Dim xApp_Nome As String = IIf(IsDBNull(DT_Planning.Rows(i).Item("Entita_Des")), "", DT_Planning.Rows(i).Item("Entita_Des"))
            Dim xCul_Cod As Integer = IIf(IsDBNull(DT_Planning.Rows(i).Item("Cul_Cod")), 0, DT_Planning.Rows(i).Item("Cul_Cod"))
            Dim xGru_Cod As Integer
            Dim xVeg_Cod As Integer = IIf(IsDBNull(DT_Planning.Rows(i).Item("Veg_Cod")), 0, DT_Planning.Rows(i).Item("Veg_Cod"))
            Dim xValidita_Inizio As String = CDate(DT_Planning.Rows(i).Item("Validita_Inizio")).ToShortDateString()
            Dim xValidita_Fine As String = CDate(DT_Planning.Rows(i).Item("Validita_Fine")).ToShortDateString()
            Dim xDestinazioneUso_Des As String = IIf(IsDBNull(DT_Planning.Rows(i).Item("DestinazioneUso_Des")), "", DT_Planning.Rows(i).Item("DestinazioneUso_Des"))

            '' VAnni: 30/10/2017: ottimizzata questa roba, inoltre gru_cod era inutilizzato.
            'Dim objGruCod As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

            'Dim dtgru As DataTable
            'dtgru = objGruCod.Leggi_con_Cul_Des(xVeg_Cod, xCul_Cod, "", "", objParametri_Server)
            'xGru_Cod = dtgru.Rows(0).Item("Gru_Cod")

            'Dim Veg_Des, Cul_Des As String
            'Veg_Des = dtgru.Rows(0).Item("Veg_Des")
            'Cul_Des = dtgru.Rows(0).Item("Cul_DEs")

            Dim TipoNodo As Integer

            '---------------------------------------
            '----- Valuto i dati ottenuti ...
            '---------------------------------------

            'Inizio a costruire la descrizione
            DescrizioneNodo = "{" & My.Resources.AgronicaControlli_2010.Impianto & "}"

            'Se il Cul_Cod e' nullo ho terreno nudo ...
            If xCul_Cod = 0 Then
                '===== TERRENO NUDO =====

                TipoNodo = enum_TipoNodo.ImpiantoNudo

                'Descrizione del nodo
                stbDescrizioneNodo.Append(xValidita_Inizio)
                stbDescrizioneNodo.Append(" - ")
                stbDescrizioneNodo.Append(xApp_Nome)
                stbDescrizioneNodo.Append(" - ")
                stbDescrizioneNodo.Append(My.Resources.AgronicaControlli_2010.TerrenoNudo)
                stbDescrizioneNodo.Append(" - ")
                stbDescrizioneNodo.Append(xDestinazioneUso_Des)
                stbDescrizioneNodo.Append(": { ")
                stbDescrizioneNodo.Append(xSupPlan)
                stbDescrizioneNodo.Append(" Ha }")

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

                stbDescrizioneNodo.Length = 0

                'Descrizione del nodo
                stbDescrizioneNodo.Append(xValidita_Inizio)
                stbDescrizioneNodo.Append(" - ")
                stbDescrizioneNodo.Append(xApp_Nome)
                stbDescrizioneNodo.Append(" - ")
                stbDescrizioneNodo.Append(DT_Planning.Rows(i)("Veg_Des"))
                stbDescrizioneNodo.Append(" - ")
                stbDescrizioneNodo.Append(DT_Planning.Rows(i)("Cul_Des"))
                stbDescrizioneNodo.Append(": { ")
                stbDescrizioneNodo.Append(xSupPlan)
                stbDescrizioneNodo.Append(" Ha }")
            End If

            DescrizioneNodo = stbDescrizioneNodo.ToString

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
                                    0, 0, 0, , , , , , , , , , , , , , , , , ,
                                    DT_Planning.Rows(i).Item("programmazione_cod"), DT_Planning.Rows(i).Item("Programmazione_Entita_Cod"))

            Dim planning As AjaxTreeNodeJsonObject

            If cfg.Flag_CheckBox Then
                If Not cfg.CheckBoxes.Flag_CheckBoxAppezzamento Then
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
                                xVeg_Cod, ElencoIcone:=cfg.Elenco_Icone_SpecieVegetali
                                 )
            planning.icon = PathRoot + Albero.RitornaPathImg(TipoNodo, IconaPreferita)
            planning.type = "planningEntita"
            results.Add(planning)
        Next

    End Sub

#End Region

    ''' <summary>
    ''' Carica il nodo Portafoglio Prodotti
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaPortafoglioProdotti(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
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

        'i18n__
        Dim prefixProdottiAziendali = My.Resources.AgronicaControlli_2010.ProdottiAziendali
        If cfg.Flag_CheckBox Then

            If Not cfg.CheckBoxes.Flag_CheckBoxProdotti Then

                Portafoglio = New AjaxTreeNodeJsonObject(xChiave,
                                                  prefixProdottiAziendali, "jstree-no-checkboxes", "", "#", False)
            Else

                Portafoglio = New AjaxTreeNodeJsonObject(xChiave,
                                                  prefixProdottiAziendali, "", "", "#", False)
            End If

        Else

            Portafoglio = New AjaxTreeNodeJsonObject(xChiave,
                                                  prefixProdottiAziendali, "", "", "#", False)
        End If

        Portafoglio.icon = PathRoot & Albero.RitornaPathImg(enum_TipoNodo.p_PortafoglioProdotti, "")

        results.Add(Portafoglio)
    End Sub

    ''' <summary>
    ''' Carica la lista dei fabbricati
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaNodoFabbricati(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
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
        
        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim DtFabbricati = objFabbricati.Leggi_2(xPiva,
                                       xSa_Cod,
                                       0,
                                       0,
                                       "",
                                       "",
                                       objParametri_Server)

        Dim fabbricatiList = New List(Of AjaxTreeNodeJsonObject)

        For Each row as DataRow In DtFabbricati.Rows 
            Dim fabbricato_Cod = row.Item("Fabbricato_Cod")
            Dim fabbricato_Des = row.Item("Fabbricato_Des")
            
            Dim key = ""
            
            Call Albero.ChiaveAlbero_Codifica_x_json(
                key,
                enum_TipoNodo.Fabbricato_Generico,
                Piva := xPiva,
                Sa_Cod := xSa_Cod, 
                Fabbricato_Cod := fabbricato_Cod)
            Dim node = New AjaxTreeNodeJsonObject(key, fabbricato_Des, "", "", "#", False)
            node.icon = Nothing
            node.type = "fabbricati"
            fabbricatiList.Add(node)
        Next

        'i18n__
        Dim prefixFabbricatiAziendali = My.Resources.AgronicaControlli_2010.FabbricatiAziendali
        Fabbricati = New AjaxTreeNodeJsonObject(xChiaveListaFabbricati,
                                              prefixFabbricatiAziendali, "", "", "#", fabbricatiList)

        Fabbricati.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.x_ListaFabbricatiAziendali, "")
        Fabbricati.type = "fabbricati"

        results.Add(Fabbricati)
    End Sub

    ''' <summary>
    ''' Carica la lista dei sottonodi dei fabbricati ( tutti gli elementi all'interno del fabbricato)
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaListaFabbricati(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal Id As String,
        ByVal PathRoot As String,
        ByVal data_Inizio As Date,
        ByVal data_Fine As Date)

        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_x_json(Id, xPiva, xSa_Cod)

        '############################
        '##### Lista Fabbricati #####
        '############################

        Dim DT_Fabbricati As New DataTable
        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        DT_Fabbricati = objFabbricati.Leggi(xPiva, xSa_Cod, 0, enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                            "", "", objParametri_Server)

        Dim i As Integer
        Dim TipoNodoFabbricato As Integer
        Dim xFabbricato_Cod, xValidita_Fine, ValidazioneNodo, xTipoFabbricato_Cod, xTipoFabbricato_Des

        Dim DescrizioneNodo As String
        Dim stbDescrizioneNodo As New StringBuilder

        For i = 0 To DT_Fabbricati.Rows.Count - 1

            stbDescrizioneNodo.Length = 0

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

            stbDescrizioneNodo.Append("")
            stbDescrizioneNodo.Append(DT_Fabbricati.Rows(i).Item("Tipo_Fabbricato_Des"))
            stbDescrizioneNodo.Append(" (")
            stbDescrizioneNodo.Append(DT_Fabbricati.Rows(i).Item("Fabbricato_Des"))
            stbDescrizioneNodo.Append(") ")

            'Verifico lo stato di VALIDAZIONE
            If ValidazioneNodo = "-1" Then
                stbDescrizioneNodo.Append(" ..... (§§§ da confermare §§§)")
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

            DescrizioneNodo = stbDescrizioneNodo.ToString

            Dim Fabbricati As AjaxTreeNodeJsonObject

            If cfg.Flag_CheckBox Then

                If Not cfg.CheckBoxes.Flag_CheckBoxMagazzino Then
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

                    If cfg.Flag_Analisi = True Then
                        Dim Livello_Analisi As New List(Of AjaxTreeNodeJsonObject)
                        CaricaAnalisi(cfg, Livello_Analisi, objParametri_Server, App, PathRoot, data_Inizio, data_Fine)
                        If Livello_Analisi.Count > 0 Then
                            Fabbricati.children = Livello_Analisi
                        Else
                            Fabbricati.children = New List(Of AjaxTreeNodeJsonObject)
                        End If
                    Else
                        Fabbricati.children = New List(Of AjaxTreeNodeJsonObject)
                    End If

                    'i18n__
                    Dim prefixGiacenzeDiMagazzino = My.Resources.AgronicaControlli_2010.GiacenzeDiMagazzino
                    If cfg.Flag_CheckBox Then

                        If Not cfg.CheckBoxes.Flag_CheckBoxGiacenze Then

                            Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                                          prefixGiacenzeDiMagazzino, "jstree-no-checkboxes", "", "#", False))
                        Else

                            Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                                          prefixGiacenzeDiMagazzino, "", "", "#", False))
                        End If

                    Else

                        Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                                          prefixGiacenzeDiMagazzino, "", "", "#", False))
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

                    'i18n__
                    Dim prefixMovimentiDiMagazzino = My.Resources.AgronicaControlli_2010.MovimentiDiMagazzino
                    If cfg.Flag_CheckBox Then

                        If Not cfg.CheckBoxes.Flag_CheckBoxMovimenti Then

                            Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                      prefixMovimentiDiMagazzino, "jstree-no-checkboxes", "", "#", False))
                        Else

                            Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                      prefixMovimentiDiMagazzino, "", "", "#", False))
                        End If

                    Else

                        Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                      prefixMovimentiDiMagazzino, "", "", "#", False))
                    End If

                    Fabbricati.children(Fabbricati.children.Count - 1).icon = PathRoot & Albero.RitornaPathImg(enum_TipoNodo.x_MovimentiMagazzino, "")

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
                    'i18n__
                    If cfg.Flag_Analisi = True Then
                        Dim Livello_Analisi As New List(Of AjaxTreeNodeJsonObject)
                        CaricaAnalisi(cfg, Livello_Analisi, objParametri_Server, App, PathRoot, data_Inizio, data_Fine)
                        If Livello_Analisi.Count > 0 Then
                            Fabbricati.children = Livello_Analisi
                        Else
                            Fabbricati.children = New List(Of AjaxTreeNodeJsonObject)
                        End If
                    Else
                        Fabbricati.children = New List(Of AjaxTreeNodeJsonObject)
                    End If
                    Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                                             My.Resources.AgronicaControlli_2010.PreparazioniAlimentari, "", "", "#", False))

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

                    If cfg.Flag_Analisi = True Then
                        Dim Livello_Analisi As New List(Of AjaxTreeNodeJsonObject)
                        CaricaAnalisi(cfg, Livello_Analisi, objParametri_Server, App, PathRoot, data_Inizio, data_Fine)
                        If Livello_Analisi.Count > 0 Then
                            Fabbricati.children = Livello_Analisi
                        Else
                            Fabbricati.children = New List(Of AjaxTreeNodeJsonObject)
                        End If
                        Fabbricati.children = New List(Of AjaxTreeNodeJsonObject)
                    End If

                    Fabbricati.children.Add(New AjaxTreeNodeJsonObject(xChiave,
                                                                       My.Resources.AgronicaControlli_2010.ConsistenzeAnimali, "", "", "#", False))

                    Fabbricati.children(Fabbricati.children.Count - 1).icon = PathRoot & Albero.RitornaPathImg(enum_TipoNodo.x_ConsistenzeAnimali, "")
                    '//////////////////////////////////
                    '///// fine CONSISTENZE  //////////
                    '//////////////////////////////////

                    '-----

            End Select

            results.Add(Fabbricati)

        Next

    End Sub



#Region "ESERCIZI"
    ''' <summary>
    ''' Per il caricamento degli esercizi
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaEsercizio(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
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
                                           "", "", objParametri_Server)

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

                DescrizioneNodo = "{" & My.Resources.AgronicaControlli_2010.EsercizioAnnualita & "} " & DT_Esercizio.Rows(i).Item("Progetto_Des") & " - " & CDate(DT_Esercizio.Rows(i).Item("Validita_Inizio")).ToShortDateString

                'aggiungo effettibvamente il nodo dell'esercizio
                Dim Esercizio As AjaxTreeNodeJsonObject

                If cfg.Flag_PianoConcimazione = True Then

                    Dim Livello_PianoConcimazioni As New List(Of AjaxTreeNodeJsonObject)

                    CaricaPianoConcimazioni(cfg, Livello_PianoConcimazioni, objParametri_Server, xChiaveAlbero, PathRoot, data_Inizio, data_Fine)
                    If Livello_PianoConcimazioni.Count > 0 Then

                        If cfg.Flag_CheckBox Then

                            If Not cfg.CheckBoxes.Flag_CheckBoxImpianto Then
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

                        If cfg.Flag_CheckBox Then

                            If Not cfg.CheckBoxes.Flag_CheckBoxImpianto Then

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
                    If cfg.Flag_CheckBox Then

                        Esercizio = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                                          DescrizioneNodo, "", "", "#", False)

                    Else
                        Esercizio = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                                          DescrizioneNodo, "jstree-no-checkboxes", "", "#", False)
                    End If
                End If

                Esercizio.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.DistintaDiProduzione, "")
                If cfg.Flag_Esplodi_Tutto = True Then
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

    ''' <summary>
    ''' Per il caricamento delle Particelle
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaAnalisi(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal Id As String,
        ByVal PathRoot As String,
        ByVal data_Inizio As Date,
        ByVal data_Fine As Date)

        'Costruisco la stringa di filtro

        Dim strFiltro As String = ""
        Dim stbFiltro As New StringBuilder
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

        Albero.ChiaveAlbero_Decodifica_x_json(
            Id,
            xPiva,
            xSa_Cod,
            xCampo_Cod,
            xAppezza,
            xId_Reg,
            Nothing,
            xProv,
            xCom,
            xSezione,
            xFoglio,
            xNumero,
            xSubalterno,
            xCodFisc,
            xFabbricato_Cod,
            Nothing,
            Nothing,
            Nothing,
            Nothing,
            Nothing,
            Nothing,
            Nothing,
            xVas_cod,
            Nothing)

        If xEntita_Cod <> 0 Then
            stbFiltro.Append(" Analisi_Entita_Cod = " & Agro_SQL_SaveNum(xEntita_Cod) & " AND  ")
        End If
        If xPiva <> "" Then
            stbFiltro.Append(" Piva = '" & Agro_SQL_SaveText(xPiva) & "' AND ")
        End If
        If xSa_Cod <> 0 Then
            stbFiltro.Append(" Sa_Cod = " & Agro_SQL_SaveNum(xSa_Cod) & " AND ")
        End If
        If xCampo_Cod <> 0 Then
            stbFiltro.Append(" Campo_Cod = " & Agro_SQL_SaveNum(xCampo_Cod) & " AND ")
        End If
        If xAppezza <> 0 Then
            stbFiltro.Append(" Appezza = " & Agro_SQL_SaveNum(xAppezza) & " AND ")
        End If
        If xId_Reg <> 0 Then
            stbFiltro.Append(" Id_Imp = " & Agro_SQL_SaveNum(xId_Reg) & " AND ")
        End If
        If xFabbricato_Cod <> 0 Then
            stbFiltro.Append(" Fabbricato_Cod = " & Agro_SQL_SaveNum(xFabbricato_Cod) & " AND ")
        End If
        If xProv <> "" And xProv <> "0" Then
            stbFiltro.Append(" Prov = '" & Agro_SQL_SaveText(xProv) & "' AND ")
        End If
        If xCom <> "" And xCom <> "0" Then
            stbFiltro.Append(" Com = '" & Agro_SQL_SaveText(xCom) & "' AND ")
        End If
        If xSezione <> "" And xSezione <> "0" Then
            stbFiltro.Append(" Sezione = '" & Agro_SQL_SaveText(xSezione) & "' AND ")
        End If
        If xFoglio <> 0 Then
            stbFiltro.Append(" Foglio = " & Agro_SQL_SaveNum(xFoglio) & " AND ")
        End If
        If xNumero <> 0 Then
            stbFiltro.Append(" Numero = " & Agro_SQL_SaveNum(xNumero) & " AND ")
        End If
        If xSubalterno <> "" And xSubalterno <> "0" Then
            stbFiltro.Append(" Subalterno = '" & Agro_SQL_SaveText(xSubalterno) & "' AND ")
        End If
        If xID_oggetto_Grafico <> "0" And xID_oggetto_Grafico <> "" Then
            stbFiltro.Append(" ID_Oggetto_Grafico = '" & Agro_SQL_SaveText(xID_oggetto_Grafico) & "' AND ")
        End If
        If xVas_cod <> 0 Then
            stbFiltro.Append(" Vas_Cod = " & Agro_SQL_SaveNum(xVas_cod) & " AND ")
        End If

        strFiltro = stbFiltro.ToString

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

                Dim alberoCatasto = New With {
                    .prov = GetAnalisiItemValueCatasto(DR_Analisi(i), "Prov"),
                    .com = GetAnalisiItemValueCatasto(DR_Analisi(i), "Com"),
                    .sezione = GetAnalisiItemValueCatasto(DR_Analisi(i), "Sezione"),
                    .foglio = CInt(DR_Analisi(i).Item("Foglio")),
                    .numero = CInt(DR_Analisi(i).Item("Numero")),
                    .subalterno = GetAnalisiItemValueCatasto(DR_Analisi(i), "Subalterno")
                }

                Call Albero.ChiaveAlbero_Codifica_x_json(
                    xChiaveAlbero,
                    enum_TipoNodo.Analisi_Testata,
                    Piva:=xPiva,
                    Sa_Cod:=DR_Analisi(i).Item("sa_cod"),
                    Campo_Cod:=DR_Analisi(i).Item("Campo_cod"),
                    Appezza:=DR_Analisi(i).Item("Appezza"),
                    Id_Imp:=DR_Analisi(i).Item("ID_Imp"),
                    p_Provincia_Cod:=alberoCatasto.prov,
                    p_Comune_Cod:=alberoCatasto.com,
                    p_Sezione:=alberoCatasto.sezione,
                    p_Foglio:=alberoCatasto.foglio,
                    p_Numero:=alberoCatasto.numero,
                    p_Subalterno:=alberoCatasto.subalterno,
                    Fabbricato_Cod:=DR_Analisi(i).Item("Fabbricato_cod"),
                    Analisi_Testata_Cod:=DR_Analisi(i).Item("Analisi_Testata_Cod"))

                DescrizioneNodo = "{" & My.Resources.AgronicaControlli_2010.Analisi & "} " & DR_Analisi(i).Item("Analisi_Testata_Des") & " - " & CDate(DR_Analisi(i).Item("Analisi_Testata_Data_Inizio")).ToShortDateString

                '----- Cerco gli eventuali CAMPIONI di analisi

                'Filtro

                Dim strFiltro1 As String = ""
                strFiltro1 &= " Piva = '" & Agro_SQL_SaveText(Trim(cfg.Piva)) & "' AND "
                If DR_Analisi(i).Item("Analisi_Testata_Cod") <> -99999 Then
                    strFiltro1 &= " Analisi_Testata_Cod = " & Agro_SQL_SaveNum(DR_Analisi(i).Item("Analisi_Testata_Cod")) & " AND "
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

                    Call Albero.ChiaveAlbero_Codifica_x_json(
                        Chiave,
                        enum_TipoNodo.Analisi_Campione,
                        Piva:=cfg.Piva,
                        Sa_Cod:=DR_Analisi(i).Item("sa_cod"),
                        Campo_Cod:=DR_Analisi(i).Item("Campo_cod"),
                        Appezza:=DR_Analisi(i).Item("Appezza"),
                        Id_Imp:=DR_Analisi(i).Item("ID_Imp"),
                        p_Provincia_Cod:=alberoCatasto.prov,
                        p_Comune_Cod:=alberoCatasto.com,
                        p_Sezione:=alberoCatasto.sezione,
                        p_Foglio:=alberoCatasto.foglio,
                        p_Numero:=alberoCatasto.numero,
                        p_Subalterno:=alberoCatasto.subalterno,
                        Fabbricato_Cod:=DR_Analisi(i).Item("Fabbricato_cod"),
                        Analisi_Testata_Cod:=CInt(DR_CampioniXDettagli(j).Item("Analisi_Testata_Cod")),
                        Analisi_Dettaglio_Cod:=CInt(DR_CampioniXDettagli(j).Item("Analisi_Dettaglio_Cod")),
                        Analisi_Campione_Cod:=CInt(DR_CampioniXDettagli(j).Item("Analisi_Campione_Cod")))

                    'Aggiungo il nodo campione

                    If cfg.Flag_CheckBox Then

                        If Not cfg.CheckBoxes.Flag_CheckBoxCampioni Then

                            Dim campioneNode = New AjaxTreeNodeJsonObject(
                                Chiave,
                                "{" & My.Resources.AgronicaControlli_2010.Campione & "} " & DR_CampioniXDettagli(j).Item("Analisi_Campione_Des"),
                                "jstree-no-checkboxes",
                                "",
                                "#",
                                False)

                            campioneNode.type = "campione"
                            Campioni.Add(campioneNode)

                        Else

                            Dim campioneNode = New AjaxTreeNodeJsonObject(
                                Chiave,
                                "{" & My.Resources.AgronicaControlli_2010.Campione & "} " & DR_CampioniXDettagli(j).Item("Analisi_Campione_Des"),
                                "",
                                "",
                                "#",
                                False)

                            campioneNode.type = "campione"
                            Campioni.Add(campioneNode)

                        End If

                    Else

                        Dim campioneNode = New AjaxTreeNodeJsonObject(
                            Chiave,
                            "{" & My.Resources.AgronicaControlli_2010.Campione & "} " & DR_CampioniXDettagli(j).Item("Analisi_Campione_Des"),
                            "",
                            "",
                            "#",
                            False)

                        campioneNode.type = "campione"
                        Campioni.Add(campioneNode)

                    End If

                    Campioni(Campioni.Count - 1).icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Analisi_Campione, "")

                Next

                'Aggiungo effettibvamente il nodo dell'analisi

                Dim Analisi As AjaxTreeNodeJsonObject

                If Campioni.Count > 0 Then

                    If cfg.Flag_CheckBox Then

                        If Not cfg.CheckBoxes.Flag_CheckBoxAnalisi Then

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

                    If cfg.Flag_CheckBox Then

                        If Not cfg.CheckBoxes.Flag_CheckBoxAnalisi Then

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

                Analisi.icon = PathRoot & Albero.RitornaPathImg(enum_TipoNodo.Analisi_Testata, "")
                Analisi.type = "analisi"
                results.Add(Analisi)

            Next

        End If

    End Sub

    Private Function GetAnalisiItemValueCatasto(ByVal drAnalisi As DataRow, ByVal itemName As String) As String

        Dim itemValue = drAnalisi.Item(itemName)

        If itemValue = "" Then
            itemValue = "0"
        End If

        Return itemValue

    End Function

    Public Function CaricaAlberoCentriDropdown(piva As String, objPServer As AgronicaCoreParametri, Optional insertDefault As Boolean = False) As DataTable
        Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        Dim dtAnagrafica As DataTable = objCentri.Leggi_x_anagrafica(piva, 0, "", "", objPServer)

        If insertDefault Then
            Dim row As DataRow = dtAnagrafica.NewRow()
            row("sa_cod") = "0"
            row("sa_nome") = My.Resources.AgronicaControlli_2010.VisualizzazioneDiTuttiICentriAziendali
            dtAnagrafica.Rows.InsertAt(row, 0)
        End If

        Return dtAnagrafica
    End Function

    Private Sub FiltroSelectsuDt(ByRef DT_Analisi As DataTable,
                                             ByRef DR_Analisi As DataRow(),
                                             ByVal strFiltro As String)

        If Not DT_Analisi Is Nothing Then

            DR_Analisi = DT_Analisi.Select(strFiltro)
        End If

    End Sub


    ''' <summary>
    ''' Carica La lista dei nodi dei Campi
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaNodiCampi(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri,
        ByVal Id As String,
        ByVal PathRoot As String,
        ByVal data_Inizio As Date,
        ByVal data_Fine As Date)

        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_x_json(Id, xPiva, xSa_Cod)

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
        If cfg.DatiSportelloSementieri <> "" Then
            Dim v = cfg.DatiSportelloSementieri.Split("|")(4)
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
                                objParametri_Server)

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
                    DescrizioneNodo += "{" & My.Resources.AgronicaControlli_2010.Campo & "} : "

                Case Else   'SERRA
                    TipoNodo = enum_TipoNodo.Serra
                    DescrizioneNodo += "{" & My.Resources.AgronicaControlli_2010.Serra & "} : "

            End Select

            DescrizioneNodo &= xCampo_Des

            'Verifico lo stato di VALIDAZIONE
            If ValidazioneNodo = "-1" Then
                DescrizioneNodo &= " ..... (§§§ da confermare §§§)"
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
            If cfg.Flag_Esplodi_Tutto = True Then
                child = New List(Of AjaxTreeNodeJsonObject)
                If cfg.Flag_Analisi = True Then
                    CaricaAnalisi(cfg, child, objParametri_Server, xChiave, PathRoot, data_Inizio, data_Fine)
                End If
                CaricaAppezzamenti(cfg, child, objParametri_Server, xChiave, PathRoot, xCampo_Cod, Codice_Fiscale_Tecnico, data_Inizio, data_Fine)
            Else
                child = New Boolean
                child = True
            End If

            If cfg.Flag_CheckBox Then
                If Not cfg.CheckBoxes.Flag_CheckBoxCentro Then
                    Campo = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "jstree-no-checkboxes", "", "#", child)
                Else
                    Campo = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
                End If
            Else
                Campo = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
            End If

            If cfg.Flag_Esplodi_Tutto = True Then
                Campo.state = "open"
            End If
            Campo.icon = PathRoot + Albero.RitornaPathImg(TipoNodo, "")
            Campo.type = "campo"
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
    Public Sub CaricaAppezzamenti(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
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

        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer

        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_x_json(Id, xPiva, xSa_Cod)

        'filtro per la data operazione.anno, pèer non avere troppi impianti
        Dim filtrodata As String = ""
        Try
            If cfg.ParametriAgendaData <> AGRODATAINIZIO Then


                Dim anno As Integer = cfg.ParametriAgendaData.Year
                filtrodata = " Appezzamento.Validita_Fine > " & Agro_SQL_SaveDate("01/01/" & anno) & " AND   Appezzamento.Validita_Inizio < " & Agro_SQL_SaveDate("31/12/" & anno) & "  "


            End If

        Catch ex As Exception
            filtrodata = ""
        End Try

        '#########################
        '#####  APPEZZAMENTI #####
        '#########################

        Dim DT_Appezzamenti As DataTable
        Dim objAppezzamenti As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        If Not String.IsNullOrEmpty(xSementiero) Then
            xSementiero = " AND T1.Codice_Fiscale_Tecnico = '" & Agro_SQL_SaveText(xSementiero) & "' "
        End If

        If cfg.FlagModalitaSementieri Then
            xSementiero = " AND T1.Codice_Fiscale_Tecnico = '" & Agro_SQL_SaveText(Codice_Fiscale_Tecnico) & "' "
        End If

        Dim lOrderByDataUltimoImpianto As String = ""
        If cfg.ordinaDataUltimoImpianto Then
            lOrderByDataUltimoImpianto = " InizioImpianto desc "
        End If

        If xCampocod = 0 Then

            Dim filtro As String = "  Appezzamento.Campo_Cod = " & xCampocod & " "
            If filtrodata <> "" Then
                filtro &= " AND " & filtrodata
            End If

            If xSementiero <> "" And Not cfg.DatiSportelloSementieri Is Nothing Then

                Dim v = cfg.DatiSportelloSementieri.Split("|")(4)
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

                If (cfg.Veg_Cod <> 0) Then
                    filtrodata = "SpecieVegetali.Veg_Cod =" + CStr(cfg.Veg_Cod) + IIf(filtrodata = "", "", " AND " + filtrodata)
                    DT_Appezzamenti = objAppezzamenti.Recupera_Appezzamenti_Colture_del_Campo(xPiva, xSa_Cod, xCampocod, AGRODATAINIZIO, AGRODATAFINE, True, enumSelezioneVariabile.Selezione_JoinDescrizioni, filtrodata, lOrderByDataUltimoImpianto, objParametri_Server, cfg.FiltroImpiantiIdTestataTemp)
                Else
                    DT_Appezzamenti = objAppezzamenti.LeggiconFiltroSementieri(xPiva, xSa_Cod, xCampocod, xSementiero,
                                                enumSelezioneVariabile.Selezione_JoinCompleta,
                                                filtro, lOrderByDataUltimoImpianto, objParametri_Server, cfg.FiltroImpiantiIdTestataTemp)
                End If

            Else

                If cfg.Flag_Appezzamenti_Filtra_Tecnico Then

                    If (cfg.Veg_Cod <> 0) Then
                        filtrodata = "SpecieVegetali.Veg_Cod =" + CStr(cfg.Veg_Cod) + IIf(filtrodata = "", "", " AND " + filtrodata)
                        DT_Appezzamenti = objAppezzamenti.Recupera_Appezzamenti_Colture_del_Campo(xPiva, xSa_Cod, xCampocod, AGRODATAINIZIO, AGRODATAFINE, True, enumSelezioneVariabile.Selezione_JoinDescrizioni, filtrodata, lOrderByDataUltimoImpianto, objParametri_Server, cfg.FiltroImpiantiIdTestataTemp)
                    Else
                        DT_Appezzamenti = objAppezzamenti.LeggiconCampo_ConFiltroCFT(xPiva, xSa_Cod, xCampocod, Codice_Fiscale_Tecnico,
                                                    enumSelezioneVariabile.Selezione_JoinCompleta,
                                                    filtro, lOrderByDataUltimoImpianto, objParametri_Server, cfg.FiltroImpiantiIdTestataTemp)
                    End If

                Else
                    If (cfg.Veg_Cod <> 0) Then
                        filtrodata = "SpecieVegetali.Veg_Cod =" + CStr(cfg.Veg_Cod) + IIf(filtrodata = "", "", " AND " + filtrodata)
                        DT_Appezzamenti = objAppezzamenti.LeggiconCampo_eVegCod(xPiva, xSa_Cod, xCampocod, cfg.Veg_Cod,
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    filtro, lOrderByDataUltimoImpianto, objParametri_Server, cfg.FiltroImpiantiIdTestataTemp)
                    Else
                        DT_Appezzamenti = objAppezzamenti.LeggiconCampo(xPiva, xSa_Cod, xCampocod,
                                                    enumSelezioneVariabile.Selezione_JoinCompleta,
                                                    filtro, lOrderByDataUltimoImpianto, objParametri_Server, cfg.FiltroImpiantiIdTestataTemp)
                    End If

                End If

            End If
        Else

            If xSementiero <> "" And Not cfg.DatiSportelloSementieri Is Nothing Then
                Dim v = cfg.DatiSportelloSementieri.Split("|")(4)
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
                                                filtrodata, lOrderByDataUltimoImpianto, objParametri_Server, cfg.FiltroImpiantiIdTestataTemp)
            Else

                If cfg.Flag_Appezzamenti_Filtra_Tecnico Then
                    If (cfg.Veg_Cod <> 0) Then
                        filtrodata = "SpecieVegetali.Veg_Cod =" & CStr(cfg.Veg_Cod) & IIf(filtrodata = "", "", " AND " + filtrodata)
                        DT_Appezzamenti = objAppezzamenti.Recupera_Appezzamenti_Colture_del_Campo(xPiva, xSa_Cod, xCampocod, AGRODATAINIZIO, AGRODATAFINE, True, enumSelezioneVariabile.Selezione_JoinDescrizioni, filtrodata, lOrderByDataUltimoImpianto, objParametri_Server, cfg.FiltroImpiantiIdTestataTemp)
                    Else
                        DT_Appezzamenti = objAppezzamenti.LeggiconCampo_ConFiltroCFT(xPiva, xSa_Cod, xCampocod, Codice_Fiscale_Tecnico,
                                                        enumSelezioneVariabile.Selezione_JoinCompleta,
                                                        filtrodata, lOrderByDataUltimoImpianto, objParametri_Server, cfg.FiltroImpiantiIdTestataTemp)
                    End If
                Else
                    If (cfg.Veg_Cod <> 0) Then
                        filtrodata = "SpecieVegetali.Veg_Cod =" & CStr(cfg.Veg_Cod) + IIf(filtrodata = "", "", " AND " + filtrodata)
                        DT_Appezzamenti = objAppezzamenti.LeggiconCampo_eVegCod(xPiva, xSa_Cod, xCampocod, cfg.Veg_Cod,
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    filtrodata, lOrderByDataUltimoImpianto, objParametri_Server, cfg.FiltroImpiantiIdTestataTemp)
                    Else

                        DT_Appezzamenti = objAppezzamenti.LeggiconCampo(xPiva, xSa_Cod, xCampocod,
                                                    enumSelezioneVariabile.Selezione_JoinCompleta,
                                                    filtrodata, lOrderByDataUltimoImpianto, objParametri_Server, cfg.FiltroImpiantiIdTestataTemp)
                    End If
                End If

            End If

        End If

        Dim i As Integer

        For i = 0 To DT_Appezzamenti.Rows.Count - 1

            'Preparo dati nodo appezzamento

            Dim datiNodoAppezzamento = Utility_AlberoAnagrafica.PreparaDatiNodoAppezzamento(
                DT_Appezzamenti.Rows(i),
                cfg.visualizzaRiferimentoAlfanumericoImpianto)

            'Genero il nodo

            'Definisco la chiave per questo elemento catasto

            Dim xChiave As String = ""

            Call Albero.ChiaveAlbero_Codifica_x_json(
                xChiave,
                enum_TipoNodo.Appezzamento,
                xPiva,
                xSa_Cod,
                xCampocod,
                datiNodoAppezzamento.xAppezza)

            Dim Appezzamento As AjaxTreeNodeJsonObject

            'Inserisco il nodo ed eventualmente i sui figli

            Dim child As Object

            If cfg.Flag_Esplodi_Tutto = True Then

                child = New List(Of AjaxTreeNodeJsonObject)

                If cfg.Flag_Analisi = True Then

                    CaricaAnalisi(cfg, child, objParametri_Server, xChiave, PathRoot, data_Inizio, data_Fine)

                End If

                If cfg.Flag_CatastoAppezzamento Then

                    CaricaParticelle(cfg, child, objParametri_Server, xChiave, PathRoot, data_Inizio, data_Fine)

                End If

                CaricaImpianti(cfg, child, objParametri_Server, xChiave, PathRoot, data_Inizio, data_Fine, xSementiero, Codice_Fiscale_Tecnico)

            Else

                child = New Boolean
                child = True

            End If

            If cfg.Flag_CheckBox Then

                If Not cfg.CheckBoxes.Flag_CheckBoxCentro Then
                    Appezzamento = New AjaxTreeNodeJsonObject(
                        xChiave,
                        datiNodoAppezzamento.DescrizioneNodo,
                        "jstree-no-checkboxes",
                        datiNodoAppezzamento.style,
                        "#",
                        child,
                        datiNodoAppezzamento.xValidita_Inizio,
                        datiNodoAppezzamento.xValidita_Fine)
                Else
                    Appezzamento = New AjaxTreeNodeJsonObject(
                        xChiave,
                        datiNodoAppezzamento.DescrizioneNodo,
                        "",
                        datiNodoAppezzamento.style,
                        "#",
                        child,
                        datiNodoAppezzamento.xValidita_Inizio,
                        datiNodoAppezzamento.xValidita_Fine)
                End If

            Else
                Appezzamento = New AjaxTreeNodeJsonObject(
                    xChiave,
                    datiNodoAppezzamento.DescrizioneNodo,
                    "",
                    datiNodoAppezzamento.style,
                    "#",
                    child,
                    datiNodoAppezzamento.xValidita_Inizio,
                    datiNodoAppezzamento.xValidita_Fine)
            End If

            If cfg.Flag_Esplodi_Tutto = True Then
                Appezzamento.state = "open"
            End If

            Appezzamento.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Appezzamento, "")
            Appezzamento.type = "appezzamento"

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
    Public Sub CaricaAppezzamento(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef risultato As AjaxTreeNodeJsonObject,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal PathRoot As String,
        ByVal piva As String,
        ByVal sa_cod As Integer,
        ByVal appezza As Integer,
        Optional ByVal Id_Agenda As Integer = 0,
        Optional ByVal xTipoNodoChiave As Integer = 0
    )

        '#########################
        '#####  APPEZZAMENTI #####
        '#########################

        Dim DT_Appezzamenti As DataTable
        Dim objAppezzamenti As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        'leggo le info dell'appezzamento
        Dim filtroAgg As String = ""

        If (cfg.Veg_Cod <> 0) Then
            filtroAgg = "SpecieVegetali.Veg_Cod =" & CStr(cfg.Veg_Cod) + IIf(filtroAgg = "", "", " AND " + filtroAgg)
            DT_Appezzamenti = objAppezzamenti.LeggiAppezzamentiDaVegCod(
                piva, cfg.Veg_Cod, enumSelezioneVariabile.Selezione_JoinDescrizioni, filtroAgg, "", objParametri_Server)
        Else
            DT_Appezzamenti = objAppezzamenti.Leggi(piva, sa_cod, appezza, enumSelezioneVariabile.Selezione_TabellaCompleta, filtroAgg, "", objParametri_Server)
        End If
        Dim campocod As Integer = 0
        campocod = DT_Appezzamenti.Rows(0).Item("campo_cod")

        Dim datiNodoAppezzamento = Utility_AlberoAnagrafica.PreparaDatiNodoAppezzamento(
            DT_Appezzamenti.Rows(0),
            cfg.visualizzaRiferimentoAlfanumericoImpianto)

        '---------------------------------------
        '----- Genero il Nodo
        '---------------------------------------
        Dim xTipoNodo As Integer
        If xTipoNodoChiave = 0 Then
            xTipoNodo = enum_TipoNodo.Appezzamento
        Else
            xTipoNodo = xTipoNodoChiave
        End If

        'Definisco la chiave per questo elemento catasto

        Dim xChiave As String = ""
        Call Albero.ChiaveAlbero_Codifica_x_json(
            Chiave:=xChiave,
            TipoNodo:=xTipoNodo,
            Piva:=piva,
            Sa_Cod:=sa_cod,
            Campo_Cod:=campocod,
            Appezza:=appezza,
            id_agenda:=Id_Agenda)

        'Inserisco il nodo ed eventualmente i sui figli

        Dim child As Object
        If cfg.Flag_Esplodi_Tutto = True Then
            child = New List(Of AjaxTreeNodeJsonObject)
            CaricaImpianti(cfg,
                           child,
                           objParametri_Server,
                           xChiave,
                           PathRoot,
                           datiNodoAppezzamento.xValidita_Inizio,
                           datiNodoAppezzamento.xValidita_Fine,
                           "",
                           Id_Agenda,
                           xTipoNodoChiave)
        Else
            child = New Boolean
            child = True
        End If

        If cfg.Flag_CheckBox Then
            If Not cfg.CheckBoxes.Flag_CheckBoxCentro Then
                risultato = New AjaxTreeNodeJsonObject(
                    xChiave,
                    datiNodoAppezzamento.DescrizioneNodo,
                    "jstree-no-checkboxes",
                    "",
                    "#",
                    child)
            Else
                risultato = New AjaxTreeNodeJsonObject(
                    xChiave,
                    datiNodoAppezzamento.DescrizioneNodo,
                    "",
                    "",
                    "#",
                    child)
            End If
        Else
            risultato = New AjaxTreeNodeJsonObject(
                xChiave,
                datiNodoAppezzamento.DescrizioneNodo,
                "",
                "",
                "#",
                child)
        End If

        If cfg.Flag_Esplodi_Tutto = True Then
            risultato.state = "open"
        End If
        risultato.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Appezzamento, "")

    End Sub

    ''' <summary>
    ''' Carica Gli Impianti
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaImpianti(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal Id As String,
        ByVal PathRoot As String,
        ByVal data_Inizio As Date,
        ByVal data_Fine As Date,
        Optional ByVal xSementiero As String = "",
        Optional ByVal Codice_Fiscale_Tecnico As String = "",
        Optional ByVal xId_agenda As Integer = 0,
        Optional ByVal xTipoNodoChiaveParametro As Integer = 0
    )

        'Mi arriva la chiave del centro

        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Dim xAppezza As Integer
        Dim xCampocod As Integer

        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Campocod_x_json(Id, xPiva, xSa_Cod, xCampocod)
        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_Appezza_x_json(Id, xPiva, xSa_Cod, xAppezza)

        If Not String.IsNullOrEmpty(xSementiero) Then
            xSementiero = " Reg_Impianti.Codice_Fiscale_Tecnico = '" & xSementiero & "' "
        End If

        If cfg.FlagModalitaSementieri Then
            xSementiero = " Reg_Impianti.Codice_Fiscale_Tecnico = '" & Codice_Fiscale_Tecnico & "' "
        End If

        '#####################
        '#####  Impianti #####
        '#####################

        Dim DT_Impianti As DataTable
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim filtroAgg As String
        If (cfg.Veg_Cod <> 0) Then
            filtroAgg = "SpecieVegetali.Veg_Cod =" + CStr(cfg.Veg_Cod) & IIf(xSementiero = "", "", " AND " & xSementiero)
        Else
            filtroAgg = xSementiero
        End If

        DT_Impianti = objImpianti.Leggi_xAlbero(
            xPiva,
            xSa_Cod,
            xAppezza,
            0,
            filtroAgg,
            "",
            objParametri_Server,
            cfg.FiltroImpiantiIdTestataTemp)

        Dim i As Integer

        For i = 0 To DT_Impianti.Rows.Count - 1

            Dim datiNodoImpianto = Utility_AlberoAnagrafica.PreparaDatiNodoImpianto(
                DT_Impianti.Rows(i),
                xTipoNodoChiaveParametro)

            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------

            'Icona

            Dim IconaPreferita As String = ""
            Albero.Selezionatore_Icone(
                IconaPreferita,
                datiNodoImpianto.TipoNodo,
                datiNodoImpianto.xVeg_Cod,
                datiNodoImpianto.xDestinazioneUso_Cod,
                ElencoIcone:=cfg.Elenco_Icone_SpecieVegetali)

            'Definisco la chiave per questo elemento

            Dim xChiave As String = ""

            Call Albero.ChiaveAlbero_Codifica_x_json(
                                    Chiave:=xChiave,
                                    TipoNodo:=datiNodoImpianto.xTipoNodoChiave,
                                    Piva:=xPiva,
                                    Sa_Cod:=xSa_Cod,
                                    Campo_Cod:=xCampocod,
                                    Appezza:=xAppezza,
                                    Id_Imp:=datiNodoImpianto.xID_Imp,
                                    id_agenda:=xId_agenda)

            'Inserisco il nodo ed eventualmente i sui figli

            Dim child As Object

            If cfg.Flag_Esplodi_Tutto = True And cfg.Flag_Ricette Then
                child = New List(Of AjaxTreeNodeJsonObject)
                CaricaRicette(cfg, child, objParametri_Server, xChiave, PathRoot, data_Inizio, data_Fine)
            Else
                child = New Boolean
                child = cfg.Flag_Ricette
            End If

            Dim Impianto As New AjaxTreeNodeJsonObject

            If cfg.Flag_Analisi = True Then

                Dim Livello_Analisi As New List(Of AjaxTreeNodeJsonObject)

                CaricaAnalisi(cfg, Livello_Analisi, objParametri_Server, xChiave, PathRoot, data_Inizio, data_Fine)

                If Livello_Analisi.Count > 0 Then
                    If cfg.Flag_CheckBox Then
                        If Not cfg.CheckBoxes.Flag_CheckBoxImpianto Then
                            Impianto = GetTreeNodeImpiantoNoChkBoxList(xChiave, datiNodoImpianto, Livello_Analisi)
                        Else
                            Impianto = GetTreeNodeImpiantoList(xChiave, datiNodoImpianto, Livello_Analisi)
                        End If
                    Else
                        Impianto = GetTreeNodeImpiantoList(xChiave, datiNodoImpianto, Livello_Analisi)
                    End If
                Else
                    If cfg.Flag_CheckBox Then
                        If Not cfg.CheckBoxes.Flag_CheckBoxImpianto Then
                            Impianto = GetTreeNodeImpiantoNoChkBoxObj(xChiave, datiNodoImpianto, False)
                        Else
                            Impianto = GetTreeNodeImpiantoObj(xChiave, datiNodoImpianto, child)
                        End If
                    Else
                        Impianto = GetTreeNodeImpiantoObj(xChiave, datiNodoImpianto, child)
                    End If
                End If

            ElseIf cfg.Flag_Esercizio = True Then

                Dim Livello_Esercizio As New List(Of AjaxTreeNodeJsonObject)

                'Inserire parte dell'esercizio

                CaricaEsercizio(cfg, Livello_Esercizio, objParametri_Server, xChiave, PathRoot, data_Inizio, data_Fine)

                If Livello_Esercizio.Count > 0 Then
                    If cfg.Flag_CheckBox Then
                        If Not cfg.CheckBoxes.Flag_CheckBoxImpianto Then
                            Impianto = GetTreeNodeImpiantoNoChkBoxList(xChiave, datiNodoImpianto, Livello_Esercizio)
                        Else
                            Impianto = GetTreeNodeImpiantoList(xChiave, datiNodoImpianto, Livello_Esercizio)
                        End If
                    Else
                        Impianto = GetTreeNodeImpiantoList(xChiave, datiNodoImpianto, Livello_Esercizio)
                    End If
                Else
                    If cfg.Flag_CheckBox Then

                        If Not cfg.CheckBoxes.Flag_CheckBoxImpianto Then
                            Impianto = GetTreeNodeImpiantoNoChkBoxObj(xChiave, datiNodoImpianto, False)
                        Else
                            Impianto = GetTreeNodeImpiantoObj(xChiave, datiNodoImpianto, child)
                        End If
                    Else
                        Impianto = GetTreeNodeImpiantoObj(xChiave, datiNodoImpianto, child)
                    End If
                End If
            Else
                Impianto = GetTreeNodeImpiantoObj(xChiave, datiNodoImpianto, child)
            End If

            Impianto.icon = PathRoot + Albero.RitornaPathImg(datiNodoImpianto.TipoNodo, IconaPreferita)
            Impianto.type = "impianto"

            If cfg.Flag_Esplodi_Tutto = True Then
                Impianto.state = "open"
            End If
            Impianto.Id_Cod = 0
            Impianto.Veg_Cod = 0
            If Not IsDBNull(DT_Impianti.Rows(0)("DestinazioneUsoCodice")) AndAlso IsNumeric(DT_Impianti.Rows(0)("DestinazioneUsoCodice")) Then
                Impianto.Id_Cod = DT_Impianti.Rows(0)("DestinazioneUsoCodice")
            End If

            If Not IsDBNull(DT_Impianti.Rows(0)("Veg_Cod")) AndAlso IsNumeric(DT_Impianti.Rows(0)("Veg_Cod")) Then
                Impianto.Veg_Cod = DT_Impianti.Rows(0)("Veg_Cod")
            End If

            results.Add(Impianto)

        Next

    End Sub

    Private Function GetTreeNodeImpiantoObj(
        xChiave As String,
        datiNodoImpianto As DatiNodoImpianto,
        child As Object
        ) As AjaxTreeNodeJsonObject

        Return New AjaxTreeNodeJsonObject(xChiave,
                                          datiNodoImpianto.DescrizioneNodo,
                                          "",
                                          datiNodoImpianto.style,
                                          "#",
                                          child,
                                          datiNodoImpianto.xValidita_Inizio,
                                          datiNodoImpianto.xValidita_Fine)

    End Function

    Private Function GetTreeNodeImpiantoNoChkBoxObj(
        xChiave As String,
        datiNodoImpianto As DatiNodoImpianto,
        oggettoGenerico As Object
        ) As AjaxTreeNodeJsonObject

        Return New AjaxTreeNodeJsonObject(xChiave,
                                          datiNodoImpianto.DescrizioneNodo,
                                          "jstree-no-checkboxes",
                                          datiNodoImpianto.style,
                                          "#",
                                          oggettoGenerico,
                                          datiNodoImpianto.xValidita_Inizio,
                                          datiNodoImpianto.xValidita_Fine)

    End Function

    Private Function GetTreeNodeImpiantoList(
        xChiave As String,
        datiNodoImpianto As DatiNodoImpianto,
        listaNodi As List(Of AjaxTreeNodeJsonObject)
        ) As AjaxTreeNodeJsonObject

        Return New AjaxTreeNodeJsonObject(xChiave,
                                          datiNodoImpianto.DescrizioneNodo,
                                          "",
                                          datiNodoImpianto.style,
                                          "#",
                                          listaNodi,
                                          datiNodoImpianto.xValidita_Inizio,
                                          datiNodoImpianto.xValidita_Fine)

    End Function

    Private Function GetTreeNodeImpiantoNoChkBoxList(
        xChiave As String,
        datiNodoImpianto As DatiNodoImpianto,
        listaNodi As List(Of AjaxTreeNodeJsonObject)
        ) As AjaxTreeNodeJsonObject

        Return New AjaxTreeNodeJsonObject(xChiave,
                                          datiNodoImpianto.DescrizioneNodo,
                                          "jstree-no-checkboxes",
                                          datiNodoImpianto.style,
                                          "#",
                                          listaNodi,
                                          datiNodoImpianto.xValidita_Inizio,
                                          datiNodoImpianto.xValidita_Fine)

    End Function

    ''' <summary>
    ''' Carica le ricette
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaRicette(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
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
            objParametri_Server
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
            If cfg.Flag_Esplodi_Tutto = True And cfg.Flag_Ricette Then
                child = New List(Of AjaxTreeNodeJsonObject)
                CaricaRicetteOperazioni(cfg, child, objParametri_Server, xChiave, PathRoot, data_Inizio, data_Fine, r("Tipo_Ricetta"))
            Else
                child = New Boolean
                child = cfg.Flag_Ricette

            End If

            If cfg.Flag_CheckBox Then

                If Not cfg.CheckBoxes.Flag_CheckBoxRicette Then

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
            NodoRicetta.icon = PathRoot & Albero.RitornaPathImg(enum_TipoNodo.ricette_Testata, )

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
    Public Sub CaricaRicetteOperazioni(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal Id As String,
        ByVal PathRoot As String,
        ByVal data_Inizio As Date,
        ByVal data_Fine As Date,
        ByVal TipoRicetta As enum_TipoRicetta
    )

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
            objParametri_Server
        )

        For Each r In DT_Ricette.Rows
            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------

            Dim DescrizioneNodo As String = r("ricetta_Operazione_des")

            Select Case TipoRicetta
                Case enum_TipoRicetta.Standard_Destinazioni

                    DescrizioneNodo &= " - " & CDate(r("validita_inizio")).ToShortDateString & " - "

                    Dim wAnag As String = r("W_Anagrafica_Stati_Cod").ToString
                    If wAnag = "" Then
                        wAnag = "0"
                    End If
                    If wAnag = "300" Then
                        DescrizioneNodo &= " (" & My.Resources.AgronicaControlli_2010.OrdiniDiLavoroRicette & ") "
                    Else
                        DescrizioneNodo &= " (" & My.Resources.AgronicaControlli_2010.Brogliaccio & ") "
                    End If

                Case enum_TipoRicetta.PianoDistribuzioneConcimi
                    DescrizioneNodo &= " (" & My.Resources.AgronicaControlli_2010.PianoDistribuzione & ") "

                Case enum_TipoRicetta.PUA
                    DescrizioneNodo &= " (" & My.Resources.AgronicaControlli_2010.PuaZootecnico & ") "

            End Select

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
            If cfg.Flag_Analisi = True Then

                Dim Livello_Analisi As New List(Of AjaxTreeNodeJsonObject)

                CaricaAnalisi(cfg, Livello_Analisi, objParametri_Server, xChiave, PathRoot, data_Inizio, data_Fine)
                If Livello_Analisi.Count > 0 Then

                    If cfg.Flag_CheckBox Then

                        If Not cfg.CheckBoxes.Flag_CheckBoxImpianto Then
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

                    If cfg.Flag_CheckBox Then

                        If Not cfg.CheckBoxes.Flag_CheckBoxRicette Then

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
            RicettaOperazione.icon = PathRoot & Albero.RitornaPathImg(enum_TipoNodo.ricette_Testata, )

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
    Public Sub CaricaOperazioniAgenda(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri,
        ByVal Id As String,
        ByVal PathRoot As String,
        ByVal data_Inizio As Date,
        ByVal data_Fine As Date)

        Dim resultsNodiFigli As New List(Of AjaxTreeNodeJsonObject)
        'genero il primo nodo
        Dim agendaTot As AjaxTreeNodeJsonObject
        Dim DescrizioneNodo As String = ""

        'mi arriva la chiave del centro 
        Dim xPiva As String = ""
        Dim xSa_Cod As Integer
        Dim xPivaPadre As String = ""

        Albero.ChiaveAlbero_Decodifica_PartitaIVA_SaCod_x_json(Id, xPiva, xSa_Cod)

        '####################
        '#####  Agenda  #####
        '####################

        Dim DT_Agenda As DataTable

        Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R
        DT_Agenda = objOperazioni.Leggi_x_Albero(
            cfg.Piva,
            cfg.Sa_Cod, data_Inizio, data_Fine,
            "('C','V')",
            objParametri_Server)

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
            DescrizioneNodo &= "{" & My.Resources.AgronicaControlli_2010.Op & "} : "
            DescrizioneNodo &= xDescrizione

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

            If cfg.Flag_CheckBox Then
                If Not cfg.CheckBoxes.Flag_CheckBoxCentro Then
                    opAgenda = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "jstree-no-checkboxes", "", "#", child)
                Else
                    opAgenda = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
                End If
            Else
                opAgenda = New AjaxTreeNodeJsonObject(xChiave, DescrizioneNodo, "", "", "#", child)
            End If

            opAgenda.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.Agenda, "")

            Dim NodoDettagli As New List(Of AjaxTreeNodeJsonObject)
            CaricaAgendaDettagli(cfg, NodoDettagli, objParametri_Server, objParametri_Utenti, xChiave, PathRoot, cfg.dataInizio, cfg.dataFine)
            opAgenda.children = NodoDettagli
            opAgenda.type = "agenda"
            resultsNodiFigli.Add(opAgenda)

        Next

        If resultsNodiFigli.Count > 0 Then
            DescrizioneNodo = My.Resources.AgronicaControlli_2010.AgendaOperazioni
            If cfg.Flag_CheckBox Then
                If Not cfg.CheckBoxes.Flag_CheckBoxCentro Then
                    agendaTot = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "jstree-no-checkboxes", "", "#", resultsNodiFigli)
                Else
                    agendaTot = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "", "", "#", resultsNodiFigli)
                End If
            Else
                agendaTot = New AjaxTreeNodeJsonObject("", DescrizioneNodo, "", "", "#", resultsNodiFigli)
            End If
            If cfg.Flag_Esplodi_Tutto = True Then
                agendaTot.state = "open"
            End If
            agendaTot.icon = PathRoot & Albero.RitornaPathImg(enum_TipoNodo.Agenda, "")
            agendaTot.type = "agendaTot"
            results.Add(agendaTot)
        End If
    End Sub

    ''' <summary>
    ''' Carica le ricette
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaAgendaDettagli(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri,
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

        'Valorizzo il Codice_Fiscale_Tecnico prendendolo dalla tabella Gruppi_Utente
        Dim Codice_Fiscale_Tecnico As String = ""
        Dim xUtenteCorrente As String = objParametri_Server.UtenteUsername
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
             objParametri_Server,
             cfg.FiltroImpiantiIdTestataTemp
        )

        For Each r In DT_movimenti.Rows
            '---------------------------------------
            '----- Genero il Nodo
            '---------------------------------------

            Dim resultsapp As New AjaxTreeNodeJsonObject
            CaricaAppezzamento(cfg, resultsapp, objParametri_Server, PathRoot, r.Item("Piva"), r.Item("sa_cod"), r.Item("appezza"), xId_agenda, enum_TipoNodo.AgendaDestinazioni)

            Dim i As Integer
            results.Add(resultsapp)

        Next

    End Sub

#Region "PIANOCONCIMAZIONI"
    ''' <summary>
    ''' Per il caricamento delle Particelle
    ''' </summary>
    ''' <param name="results"></param>
    ''' <param name="Id"></param>
    ''' <param name="PathRoot"></param>
    ''' <remarks></remarks>
    Public Sub CaricaPianoConcimazioni(
        ByVal cfg As ConfigurazioneAlbero,
        ByRef results As List(Of AjaxTreeNodeJsonObject),
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal Id As String,
        ByVal PathRoot As String,
        ByVal data_Inizio As Date,
        ByVal data_Fine As Date
    )

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

        Albero.ChiaveAlbero_Decodifica_x_json(
            Id, xPiva, xSa_Cod, xCampo_Cod, xAppezza, xId_Reg, Nothing, Nothing, Nothing, Nothing, Nothing,
            xNumero, Nothing, xCodFisc, xFabbricato_Cod,
            Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, xProgetto_Cod, Nothing)

        Dim objPianoConcimazione As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R
        ' carico solo i Piani Concimazione del terreno e dei residui

        Dim DT_PianoConcimazioni As DataTable
        DT_PianoConcimazioni = objPianoConcimazione.Leggi(
            0, xEntita_Cod, xPiva, xSa_Cod, xCampo_Cod, xAppezza, xId_Reg, xFabbricato_Cod,
            xProv, xCom, xSezione, xFoglio, xNumero, xSubalterno, xProgetto_Cod, xID_oggetto_Grafico, enumSelezioneVariabile.Selezione_JoinDescrizioni,
            "", "", objParametri_Server)

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

                DescrizioneNodo = "{" & My.Resources.AgronicaControlli_2010.PianoConcimazioni & "} " & DT_PianoConcimazioni.Rows(i).Item("PC_Testata_Des")

                'aggiungo effettibvamente il nodo dell'analisi
                Dim PianoConcimazioni As AjaxTreeNodeJsonObject

                PianoConcimazioni = New AjaxTreeNodeJsonObject(xChiaveAlbero,
                                               DescrizioneNodo, "", "", "#", False)

                PianoConcimazioni.icon = PathRoot + Albero.RitornaPathImg(enum_TipoNodo.PianoConcimazione_Testata, "")

                If cfg.Flag_Esplodi_Tutto = True Then
                    PianoConcimazioni.state = "open"
                End If
                results.Add(PianoConcimazioni)

            Next

        End If

    End Sub

#End Region


End Class
