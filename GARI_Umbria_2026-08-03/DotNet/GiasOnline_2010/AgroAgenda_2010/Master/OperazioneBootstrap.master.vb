Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider.Sicurezza

Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports System.Web.Script.Serialization
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.IO
Imports AgronicaCoreDataProvider.UtilityProvider

Partial Class OperazioneBootstrap

    Inherits System.Web.UI.MasterPage

    Public RicetteOrdiniDiLavoroAttivi As Boolean = False
    Public RicetteOrdiniDiLavoroAttivi_CSS As String = "display: none"

    Public Const RitardoMillisecondiKeyUp As String = "800"
    Dim objParametriAgenda As ParametriAgenda
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Public ricetta_cod As String = ""
    Public ricetta_operazione_cod As String = ""
    Dim Ricetta_Tipo As String = ""
    Public StrLavCodRicettabili As String = ""
    Public ListLavCodRicettabili As String()

    Dim SupTrattata As Boolean = True

    Public ObbligoUsaDPI As String = Resources.AgronicaAgenda_2010.PerSelezionareQuestaCulturaSeiObbligatoAUt
    Public MsgSupImpiego As String = Resources.AgronicaAgenda_2010.LaSuperficieDImpiegoDeveEssereMinoreOUgual
    Public MsgSupImpiegata As String = Resources.AgronicaAgenda_2010.LaSuperficieImpiegataDeveEssereMinoreOUgua
    Public msgDaTaraturaUgelli As String = Resources.AgronicaAgenda_2010.DaTaraturaUgelli
    Public msgTestoImpiantiRicetta As String = Resources.AgronicaAgenda_2010.InserisciGliImpiantiDellaRicetta
    Public msgCostiAccessoriRicetta As String = Resources.AgronicaAgenda_2010.InserisciICostiAccessoriDellaRicetta
    Public msgCaricaRicetta As String = Resources.AgronicaAgenda_2010.CaricaLaRicetta
    Public msgAnnulla As String = Resources.AgronicaAgenda_2010.Annulla
    Public divCostiAccessori As String = Resources.AgronicaAgenda_2010.CostiAccessori
    Public divCostiAccessoriAvanzati As String = Resources.AgronicaAgenda_2010.CostiAccessoriAvanzati
    Public btnSelezionaColonneText As String = Resources.AgronicaAgenda_2010.SelezionaColonne
    Public ImpostaSelezionePreferitaText As String = Resources.AgronicaAgenda_2010.ImpostaSelezionePreferitaText
    Public NuovaRisorsaText As String = Resources.AgronicaAgenda_2010.NuovaRisorsaText
    Public RisorsaCancellaText As String = Resources.AgronicaAgenda_2010.RisorsaCancella


    Public cRisorsaDes As String = Resources.AgronicaAgenda_2010.Risorsa
    Public cModificaRisorsa As String = Resources.AgronicaAgenda_2010.RisorsaModifica
    Public cUnitaDiMisura As String = Resources.AgronicaAgenda_2010.RisorsaUDM


    Dim CONSIDERATERRENONUDO As Boolean
    Dim DETTAGLITERRENONUDO As Boolean
    Dim CONSIDERADISCIPLINARE As Boolean

    Public UpdatePanelSpecie_ClientID As String
    Public UpdateCostiAccessoriKendo_ClientID As String

    Public ComboOperazioniAttiva As String = "true"

    Public Property AttivaOrarioInDataMovimento As Boolean = False

    Public visualizza_Kpin_BlockName As Boolean
    Public visualizza_codici_imp_app_prj As Boolean

    Private _ComboOperazioni_AttivaPostBack As Boolean = True

    Public ReadOnly Property UpdatePanelDisciplinareMaster As UpdatePanel
        Get
            Return UpdatePanelDisciplinare
        End Get
    End Property

    Public Property ComboOperazioni_AttivaPostBack As Boolean
        Get
            Return _ComboOperazioni_AttivaPostBack
        End Get
        Set(value As Boolean)
            _ComboOperazioni_AttivaPostBack = value
        End Set
    End Property

    Public ALGORITMO_COSTI_ACCESSORI As enum_AlgoritmoCostiAccessori

    Public objParametriAgenda_TipoOperazione As Integer
    Public objParametriAgenda_TipoOperazioneAgenda As Integer
    Public objParametriAgenda_VisualizzaSoloBottoneSalvaEsci As String
    Public objParametriAgenda_TipoRicetta As Integer

    Public Permessi As PermessiUtente

    Public Property Property_ImgBtn_DoseConsigliata() As ImageButton
        Get
            Return ImgBtn_DoseConsigliata
        End Get
        Set(ByVal value As ImageButton)
            ImgBtn_DoseConsigliata = value
        End Set
    End Property

    Public Property flag_MostraBtnSalvaCDG As Boolean


    Private Sub CaricaObjParametri()

        'Leggo i parametri
        objParametriAgenda = New ParametriAgenda
        'objParametriAgenda.Leggi()

        objParametriAgenda_TipoOperazione = objParametriAgenda.Tipo_Operazione
        objParametriAgenda_TipoOperazioneAgenda = objParametriAgenda.TipoOperazioneAgenda
        objParametriAgenda_TipoRicetta = objParametriAgenda.TipoRicetta
        objParametriAgenda_VisualizzaSoloBottoneSalvaEsci = "false"
        If objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Trattamenti Then
            objParametriAgenda_VisualizzaSoloBottoneSalvaEsci = "true"
        End If

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
    End Sub



    Private Sub GridViewToPhone()
        If Not IsNothing(GridView_Impianti.HeaderRow) Then
            GridView_Impianti.HeaderRow.Cells(0).Attributes("data-class") = "expand"

            'Attribute to hide column in Phone.
            Dim i As Integer
            For i = 15 To GridView_Impianti.HeaderRow.Cells.Count - 1
                Select Case GridView_Impianti.HeaderRow.Cells(i).Text.ToLower
                    Case "app.", "varietà", "sup. [ha]", "sup.trattata [ha]"
                    Case Else
                        GridView_Impianti.HeaderRow.Cells(i).Attributes("data-hide") = "phone"
                End Select


            Next

            'Adds THEAD and TBODY to GridView.
            GridView_Impianti.HeaderRow.TableSection = TableRowSection.TableHeader
        End If

    End Sub


    Public Sub Operazioni_Dispose()
        Session.Remove("DT_Impianti")
        Session.Remove("dtScarico")
        Session.Remove("dtScarico_old")
        Session.Remove("DT_CentriCosto")
        Session.Remove("Dt_Prodotti")
        Session.Remove("Dt_Macchine")
        Session.Remove("Dt_Manodopera")
        Session.Remove("Dt_Terzisti")
    End Sub

    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        'per le operazioni con logica modificata e utilizzo di page_operazione e modello operazioni colturali 
        'la master deve essere modificata, noin voglio che interagista coon i parametri operazioni
        'e setti i valori, a questo ci pensa la page_operazione, che aventualmente interagisce con le property della master,
        'la master diventa solo un oggetto per rappresentazione, non deve contenere logica, uno strumentop passivo della page_operazione,
        'usata solo per raggruppare controlli e struttura html comune a molte pagine.
        'per questo nelel operazioni nuove i controlli devono essere pronti gia prima della sua load, quindi nella init della master,
        'per ora rimane una fortma ibrida, piu si sbuoterà la master il piu possibile


        Master.flag_pag_Operazione = True

        CaricaObjParametri()

        caricaDefaultUtente_Disciplinare = True
        caricaDefaultAziendale_Disciplinare = True

        flag_MostraBtnSalvaCDG = True
        'Leggo l'impostazione
        Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        'Leggo l'impostazione per il tipo di algoritmo
        If IsNothing(Session("Agenda_ALGORITMO_COSTI_ACCESSORI")) Then

            Dim Dt_Impost = objImpost.Leggi2(
                                        2, objParametri_Server.SuperUserUsername,
                                        enum_Impostazioni_Utenti.SUPERUSER_COD_ALGORITMO_COSTI_ACCESSORI,
                                        "", "", objParametri_Utenti)

            If Not IsNothing(Dt_Impost) AndAlso Dt_Impost.Rows.Count > 0 Then
                ALGORITMO_COSTI_ACCESSORI = Dt_Impost.Rows(0).Item("Impostazione_Valore_1")
            Else
                ALGORITMO_COSTI_ACCESSORI = enum_AlgoritmoCostiAccessori.CAB
            End If

            Session("Agenda_ALGORITMO_COSTI_ACCESSORI") = ALGORITMO_COSTI_ACCESSORI
        Else
            ALGORITMO_COSTI_ACCESSORI = Session("Agenda_ALGORITMO_COSTI_ACCESSORI")
        End If

        Session("Disciplinare_Attivo") = False

        If Not IsPostBack Then
            Session("UtilizzataRicetta") = False
            Session("PrimaVolta") = True
            Filtro_MaterialiAvanzati.Visible = False
            Filtro_Materiali.Visible = False
        End If

        Select Case objParametriAgenda.Lav_Cod
            Case LAVCOD_INSTALLAZIONE_TRAPPOLE,
                LAVCOD_CATTURE_MASSA,
                LAVCOD_REINNESCO_TRAPPOLE,
                LAVCOD_DISTRIBUZIONE_INSETTI,
                 LAVCOD_CONFUSIONE_SESSUALE,
                 LAVCOD_DISORIENTAMENTO_SESSUALE,
                 LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE,
                LAVCOD_FASI_FENOLOGICHE,
                LAVCOD_RILIEVO_INDICI_MATURITA,
                LAVCOD_CONCIA_SEME,
                LAVCOD_RILIEVO_AVVERSITA_CAMPO,
                LAVCOD_IRRIGAZIONE,
                LAVCOD_RILIEVO_ERBE_INFESTANTI,
                LAVCOD_RILIEVO_PIOGGE

                CONSIDERATERRENONUDO = False
                DETTAGLITERRENONUDO = False

            Case Else

                CONSIDERATERRENONUDO = True
                DETTAGLITERRENONUDO = True

        End Select

        Select Case objParametriAgenda.Lav_Cod
            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_DISERBO, LAVCOD_CONCIA_SEME, LAVCOD_DISSECCAMENTO, LAVCOD_GEODISINFESTAZIONE, LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                 LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                 LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                CONSIDERADISCIPLINARE = True
                'Div_Disciplinare.Visible = True
                UpdatePanelDisciplinare.Visible = True
            Case Else
                CONSIDERADISCIPLINARE = False
                'Div_Disciplinare.Attributes.Add("style")
                'Div_Disciplinare.Visible = False
                UpdatePanelDisciplinare.Visible = False
        End Select



        '--------------------------------------------------------------------------------------------------------
        'Per la semina, voglio le impostazioni utente lette quando carico l'operazione, le leggo nella init
        'devo controllare anche se conviene fare così per le altre operazioni,
        'per ora leggo tutto come per reinnesco, ripeto due volte operazione ma non ho errori, da ottimizzare
        'If objParametriAgenda.Lav_Cod = LAVCOD_SEMINA Or
        '    objParametriAgenda.Lav_Cod = LAVCOD_TRAPIANTO Then

        '    ''Lettura dei Default
        'da errore, devo caricare anche costi , da capire cose mi serve subito e gestire ed ottimizzare
        '    DefaultUtente()
        '    DefaultAziendali()

        'End If
        '--------------------------------------------------------------------------------------------------------


        Dim dtVCodici = objImpost.Leggi2(2, objParametri_Server.SuperUserUsername,
                         enum_Impostazioni_Utenti.SuperUser_Visualizza_Codici_Anagrafici,
                         "", "", objParametri_Utenti)

        If dtVCodici.Rows.Count > 0 AndAlso dtVCodici.Rows(0)("Impostazione_Valore_1") = "1" Then
            visualizza_codici_imp_app_prj = True
        End If

        Dim dtVKPIN = objImpost.Leggi2(2, objParametri_Server.SuperUserUsername,
                         enum_Impostazioni_Utenti.SuperUser_KPIN_BlockName,
                         "", "", objParametri_Utenti)

        If dtVKPIN.Rows.Count > 0 AndAlso dtVKPIN.Rows(0)("Impostazione_Valore_1") = "1" Then
            visualizza_Kpin_BlockName = True
        End If

        'DRUDI COMMENTO TUTTO PER CODICE DUPLICATO SU PAGE_LOAD
        'Select Case objParametriAgenda.Lav_Cod
        '    Case LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE, LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING

        '        ScriptGridImpianti()
        '        InizializzaData(objParametriAgenda.Piva)

        '        If Not IsPostBack Then



        '            CreaImpostazioniColonne()
        '            ImpostazioniColonne()

        '            AggiornaCentroAziendale()

        '            txt_DataOperazione.Text = objParametriAgenda.Data

        '            AggiornaSpecie()

        '            AggiornaOperazioni()

        '            CaricaGriglia_Impianti()

        '            ImpostaPannellibyOperazione()

        '            Carica_Note()

        '            CaricaCostiAccessori()

        '            DefaultUtente()

        '            'If ricetta_cod = "" Then
        '            DefaultAziendali(True, True, True, True)
        '            'End If

        '            If objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Scrittura Then
        '                'If ricetta_cod = "" Then
        '                DefaultAziendali(True, True, True, True)
        '                'End If
        '            Else
        '                AggiornaMagazzino()
        '            End If

        '            'Attivo disattivo btn Magazzino
        '            If ComboMagazzini.Valore_Combo <> "0" Then
        '                'ImgBtn_Carico_div.Visible = True
        '                'ImgBtn_Carico_div.CssClass = ImgBtn_Carico_div.CssClass.Replace("displaynone", "")
        '                ImgBtn_Carico_div.Attributes.Remove("style")
        '            Else
        '                'ImgBtn_Carico_div.Visible = False
        '                'ImgBtn_Carico_div.CssClass &= " displaynone"
        '                ImgBtn_Carico_div.Attributes.Add("style", "display:none")
        '            End If

        '            'se ho già selezionato Data - Lavorazione e Specie posso caricare le ricette
        '            CaricaElencoRicette()

        '        End If

        'End Select

        defaultSuperuser()

    End Sub

    Private Sub SettaUdmDatoCodice(ByVal lUdm As Integer)
        Dim udmSim As String

        udmSim = UDM_Helper.GetUdmSim(lUdm, objParametri_Server)
        impostazione_EtichetteUDM(udmSim)
    End Sub

    Private Sub SettaImpostazioneUtente_UDM()
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Dt_Impostazioni As DataTable

        Dt_Impostazioni = ObjUtenti.Leggi(0,
                                          1,
                                          AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "",
                                          "",
                                          objParametri_Utenti)

        Dim fatto As Boolean = False

        If Dt_Impostazioni IsNot Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then

            For i = 0 To Dt_Impostazioni.Rows.Count - 1

                Select Case CInt(Dt_Impostazioni.Rows(i).Item("Impostazione_Cod"))

                    Case enum_Impostazioni_Utenti.UTENTE_UDM_Area_COD
                        Dim lUdm As Integer = CInt(Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1"))

                        SettaUdmDatoCodice(lUdm)
                        fatto = True
                End Select

            Next
        End If

        If Not fatto Then
            SettaUdmDatoCodice(2123)
        End If

    End Sub


    Private Sub impostazione_EtichetteUDM(ByVal UDM_DaUtente As String)



        UDM_Helper.CambiaIntestazioneGridview(0, "Sup_Imp", UDM_DaUtente, GridView_Impianti, 4)
        UDM_Helper.CambiaIntestazioneGridview(0, "Sup_Imp", UDM_DaUtente, GridView_Impianti, 5)
        UDM_Helper.CambiaIntestazioneGridview(9, "", UDM_DaUtente, GridView_Impianti, 4)

        ' GridView_Impianti.DataBind()
        GridViewToPhone()
        'BoundFieldResource21.HeaderText = String.Format(BoundFieldResource21.HeaderText, UDM_DaUtente)


    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Master.bootstrapSelect_versione = "1.12.4"

        GisSmartBS.absolutePath =
            Path.GetDirectoryName(HttpContext.Current.Request.Url.AbsolutePath).Replace("Operazioni", "GIS")

        If Not IsPostBack Then
            CalcolaCosti.Checked = True
            dgrCentriCosto_selezionato_tipo(False)
        End If

        UpdatePanelSpecie_ClientID = UpdatePanelSpecie.ClientID
        UpdateCostiAccessoriKendo_ClientID = UpdateCostiAccessoriKendo.ClientID

        If Not _ComboOperazioni_AttivaPostBack Then
            ComboOperazioniAttiva = "false"
        End If


        Permessi = New PermessiUtente()

        Dim Str As New StringBuilder
        Str.AppendLine("$(document).ready(function () {")
        Str.AppendLine("    initScriptOp(); ")
        Str.AppendLine("  inizializzazionePulsanteSalvataggio(); ")
        Str.AppendLine("});")
        ScriptManager.RegisterClientScriptBlock(UpdatePanelPerScript, UpdatePanelPerScript.GetType(),
                                         String.Format("jQuery_{0}", UpdatePanelPerScript.ClientID), Str.ToString, True)


        'per le operazioni con logica modificata e utilizzo di page_operazione e modello operazioni colturali 
        'la master deve essere modificata, noin voglio che interagista coon i parametri operazioni
        'e setti i valori, a questo ci pensa la page_operazione, che aventualmente interagisce con le property della master,
        'la master diventa solo un oggetto per rappresentazione, non deve contenere logica, uno strumentop passivo della page_operazione,
        'usata solo per raggruppare controlli e struttura html comune a molte pagine.
        'per questo nelel operazioni nuove i controlli devono essere pronti gia prima della sua load, quindi nella init della master,
        'per ora rimane una fortma ibrida, piu si sbuoterà la master il piu possibile
        CaricaObjParametri()

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


        'DRUDI 2020-05-25
        'If objParametriAgenda.Lav_Cod <> LAVCOD_REINNESCO_TRAPPOLE And objParametriAgenda.Lav_Cod <> LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE Then
        If True Then

            'CaricaObjParametri()

            ''Aggiunta degli script
            ScriptGridImpianti()
            InizializzaData(objParametriAgenda.Piva)
            InizializzaVarie()


            Select Case objParametriAgenda.Lav_Cod
                Case LAVCOD_INSTALLAZIONE_TRAPPOLE,
                    LAVCOD_CONFUSIONE_SESSUALE,
                    LAVCOD_DISORIENTAMENTO_SESSUALE,
                    LAVCOD_CATTURE_MASSA,
                    LAVCOD_DISTRIBUZIONE_INSETTI

                    SupTrattata = False
            End Select


            If Not IsPostBack Then

                ' VAnni: 2/8/2018: attivazione delle schede per ricette 2018
                Dim xLeggiCfgVerifica As New AgronicaCoreVarieDAL.Configurazione_Siti_R

                Dim dtRicetteOrdiniDiLavoroAttivi As DataTable =
                xLeggiCfgVerifica.Leggi(0, "RicetteOrdiniDiLavoro2018", "", "", objParametri_Server)

                If dtRicetteOrdiniDiLavoroAttivi.Rows.Count > 0 AndAlso dtRicetteOrdiniDiLavoroAttivi(0)("valore") <> "" Then
                    RicetteOrdiniDiLavoroAttivi = CBool(dtRicetteOrdiniDiLavoroAttivi(0)("valore"))
                    If RicetteOrdiniDiLavoroAttivi Then
                        RicetteOrdiniDiLavoroAttivi_CSS = ""
                    End If

                End If


                CreaImpostazioniColonne()
                ImpostazioniColonne()

                'Centro Aziendale
                AggiornaCentroAziendale()



                'Data
                txt_DataOperazione.Text = objParametriAgenda.Data

                'Specie messo anche fuori
                AggiornaSpecie()

                'carico le operazioni
                AggiornaOperazioni()

                'DISCIPLINARI
                'AggiornaDisciplinari()

                CaricaGriglia_Impianti()

                ImpostaPannellibyOperazione()

                Carica_Note()

                CaricaCostiAccessori()

                ''Lettura dei Default
                DefaultUtente()

                'Magazzini
                'AggiornaMagazzino()

                If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura AndAlso
                   (ricetta_cod = "0" OrElse ricetta_cod = "") AndAlso
                   (ricetta_operazione_cod = "0" OrElse ricetta_operazione_cod = "") Then
                    'If ricetta_cod = "" Then
                    DefaultAziendali(True, True, True, True)
                    'Else
                    If objParametriAgenda.TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua Then
                        AggiornaMagazzino()
                    End If
                    'End If
                Else
                    AggiornaMagazzino()
                End If


                'Attivo disattivo btn Magazzino
                If ComboMagazzini.Valore_Combo <> "0" Then
                    'ImgBtn_Carico_div.Visible = True
                    'ImgBtn_Carico_div.CssClass = ImgBtn_Carico_div.CssClass.Replace("displaynone", "")
                    ImgBtn_Carico_div.Attributes.Remove("style")
                Else
                    'ImgBtn_Carico_div.Visible = False
                    'ImgBtn_Carico_div.CssClass &= " displaynone"
                    ImgBtn_Carico_div.Attributes.Add("style", "display:none")
                End If

                SelezionaDatiDaObjParametriAgenda(True, True)

                'se ho già selezionato Data - Lavorazione e Specie posso caricare le ricette
                CaricaElencoRicette()
                hdPageLoadFirst.Value = "true"
            Else
                hdPageLoadFirst.Value = "false"
            End If
        End If

        'per fare in modo che si veda sempre correttamente la tabella,
        'dato che aggiorno i valore via js e può succedere che il dato rimanga nel viewstate e presenti il dato sbagliato
        'cosa che succede quando aggiorno i valori via js, il dato visualizzato non è giusto ma in realtà il dato giusto
        'rimane nella sessione e viene usato per il salvataggio.
        AggiornaGridViewCostiAccessoriVisibili()

        'Controllo se il tipo di gestione CdG è di vecchio tipo
        Dim leggi_CDG_R As New CDG_BIZ_R
        Dim tipoCdG = leggi_CDG_R.GetTipoCdG(objParametriAgenda.Piva, objParametri_Server, objParametri_Utenti)
        If tipoCdG <> enum_TipoCdG.NuovoTipo Then
            flag_MostraBtnSalvaCDG = False
        End If

    End Sub


#Region "Gestione Colonne Impianti"
    Private Sub CreaImpostazioniColonne()
        'creo
        ListaColonneVisibili.Items.Clear()
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.RagioneSociale))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.CentroAziendale))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Campo))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.App))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.AppRifNum))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.AppBioCod))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Catasto))
        '(12/11/2018 fede) aggiunta fase fenologica corrente
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.FaseFenologicaCorrente))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DestinazioneDUso))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Varietà))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.GruppoVarietale))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Disciplinare))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Regolamento))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Capitolato))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Finalità))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DataInizioImpianto))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DataSemina))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DataFioritura))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DataFiorituraPrevista))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DataRaccoltaPrevista))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.LottoImpianto))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Copertura))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DataRaccolta))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DataSeminaPrevista))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.ClasseTessitura))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.NKgHa))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.PKgHa))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.KKgHa))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.MgKgHa))

        '  Vanni, 20/09/2013 11:51:06: colonne AGEA
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.SpecieAgea))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.VarietàAgea))

        ' Nicoletta 15/07/2014 per smart rilevamento Pozzi
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.TraFila))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.SuFila))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.NPianteImpianto))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.FormaAllevamento))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Portinnesto))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.AppRifNum))

        If visualizza_Kpin_BlockName Then
            ListaColonneVisibili.Items.Add(New ListItem("KPIN"))
            ListaColonneVisibili.Items.Add(New ListItem("Block Name"))
        End If

        If visualizza_codici_imp_app_prj Then
            ListaColonneVisibili.Items.Add(New ListItem("Codice Impianto"))
        End If


    End Sub

    Private Sub ImpostazioniColonne()
        'carico le impostazioni utente
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Dt As DataTable
        Dt = objUtenti.Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_ColonneVisibili_TabellaImpiantiAgenda,
                 1,
                 AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                 "",
                 "",
                 objParametri_Utenti)


        Dim i As Integer = 0
        Dim str As String = ""

        'se esiste l'impostazione utilizzo quella altrimenti metto dei default (x i nuovi utenti per esempio)
        If Dt.Rows.Count > 0 Then
            str = Dt.Rows(0).Item("Impostazione_Valore_1")
        End If
        If str = "" Then
            str = "Centro Aziendale|App.|Varietà"
        End If

        For i = 0 To str.Split("|").Length - 1
            Dim valore As String = str.Split("|")(i)
            Dim j = 0
            For j = 0 To ListaColonneVisibili.Items.Count - 1
                If ListaColonneVisibili.Items(j).Text = valore Then
                    ListaColonneVisibili.Items(j).Selected = True
                End If
            Next
        Next


        hdKendo_Impianti_Colonne.Value = str

        AggiornaVisibilitaColonneImpianti()

    End Sub

    Private Sub AggiornaVisibilitaColonneImpianti()
        Dim i As Integer = 0
        Dim j As Integer = 0

        Dim style As New TableItemStyle
        style.CssClass = "displaynone"

        For j = 0 To ListaColonneVisibili.Items.Count - 1

            Dim trovato = False
            Dim visibile = False

            For i = 0 To GridView_Impianti.Columns.Count - 1
                If GridView_Impianti.Columns(i).HeaderText = ListaColonneVisibili.Items(j).Text Then
                    visibile = ListaColonneVisibili.Items(j).Selected
                    trovato = True
                    Exit For
                End If
            Next

            If trovato = True Then
                GridView_Impianti.Columns(i).Visible = visibile
            End If
        Next


        'Disabilito la colonna della superficie trattata
        If SupTrattata = False Then
            For i = 0 To GridView_Impianti.Columns.Count - 1
                Dim trovato = False
                Dim visibile = False
                If GridView_Impianti.Columns(i).HeaderText = "Sup.Trattata [Ha]" Then
                    GridView_Impianti.Columns(i).ItemStyle.CssClass = "displaynone"
                    GridView_Impianti.Columns(i).HeaderStyle.CssClass = "displaynone"
                End If
            Next
        End If

    End Sub

    Protected Sub SalvaImpostazioniColonne_Click(ByVal sender As Object, ByVal e As EventArgs) Handles SalvaImpostazioniColonne.Click
        Dim str As String = ""
        Dim i As Integer = 0
        For i = 0 To ListaColonneVisibili.Items.Count - 1
            If ListaColonneVisibili.Items(i).Selected = True Then
                str = str & ListaColonneVisibili.Items(i).Text & "|"
            End If
        Next

        hdKendo_Impianti_Colonne.Value = str
        hdKendo_Impianti_Ricarica.Value = "colonne"

        Dim Flag_Connessione, Flag_Transazione As Boolean

        Try

            Utility.VerificaApriTransazione(objParametri_Utenti,
                                         Flag_Connessione,
                                         Flag_Transazione)




            Dim objImpostazioniUtente As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
            'prima cancello le vechie impostazioni
            objImpostazioniUtente.Cancella(enum_Impostazioni_Utenti.UTENTE_COD_ColonneVisibili_TabellaImpiantiAgenda,
                                            "", objParametri_Utenti)

            If str.Length > 0 Then
                str = str.Substring(0, str.Length - 1)
                'salvo
                objImpostazioniUtente.Scrivi(enum_Impostazioni_Utenti.UTENTE_COD_ColonneVisibili_TabellaImpiantiAgenda,
                                             str, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
            End If



            Utility.VerificaChiudiTransazione(objParametri_Utenti,
                                  Flag_Transazione)

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametri_Utenti,
                                               Flag_Transazione)
        Finally

            Utility.VerificaChiudiConnessione(objParametri_Utenti,
                                                   Flag_Connessione)

        End Try




        AggiornaVisibilitaColonneImpianti()

        If Not IsNothing(Session("DT_Impianti")) Then
            GridView_Impianti.DataSource = ViewState("DT_Impianti")
            'GridView_Impianti.DataBind()
            GridViewToPhone()
            'GridView_Impianti_EvidenziaRigheDPI()
        End If

    End Sub
#End Region

#Region "Generazione dei movimenti dei costi accessori"
    Private Sub SalvaCostiAccessori_Default()
        Dim MovimentiCosti As List(Of Movimento) = New List(Of Movimento)

        Dim i As Integer = 0
        Dim DT As DataTable = Session("dtScarico")

        'in questo caso le quantità sono a zero

        'devo fare un movimento solo per ogni causale (al max 4)
        For i = -1 To -5 Step -1
            'filtro solo i movimenti di tipo ...
            Dim DR() As DataRow
            If i = -5 Then
                DR = DT.Select("Centro_cod Not In (-1,-2,-3,-4)")
            Else
                DR = DT.Select("Centro_cod = " & i)
            End If
            If DR.Length > 0 Then
                'ho qualche movimento da inserire 
                Dim Cau_Mov As Integer = 0
                Select Case i
                    Case -1
                        Cau_Mov = CAU_IMPUTAZIONE_PARCOMACCHINE
                    Case -2
                        Cau_Mov = CAU_IMPUTAZIONE_MANODOPERA
                    Case -3
                        Cau_Mov = CAU_IMPUTAZIONE_TERZISTI
                    Case -4
                        Cau_Mov = CAU_IMPUTAZIONE_TECNICO_RESPONSABILE
                    Case Else
                        Cau_Mov = CAU_SCARICO
                End Select
                'creo un nuovo movimento 
                Dim movimento As New Movimento
                movimento.Cau_Mov = Cau_Mov
                movimento.Cod_Risum = 0
                movimento.Data = objParametriAgenda.Data
                movimento.Id_Agenda = objParametriAgenda.Id_Agenda
                movimento.Lav_Cod = 0
                movimento.Mezzo = 0
                movimento.Piva = objParametriAgenda.Piva
                movimento.Sa_Cod = 0

                movimento.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)
                'creo tanti dettagli quanti sono gli item di dr()
                Dim j As Integer = 0
                For j = 0 To DR.Length - 1
                    Dim movDet As New Movimento_Dettaglio
                    'movDet.Cau_Mov = Cau_Mov
                    movDet.Data = objParametriAgenda.Data

                    movDet.Qta = 0
                    movDet.Udm_Cod = DR(j).Item("Udm_Cod")
                    movDet.Mat_Cod = DR(j).Item("Mat_cod")
                    movDet.Elem_Cod = DR(j).Item("Elem_Cod")
                    movDet.Pro_Cod = DR(j).Item("Pro_Cod") 'mancava
                    movDet.Piva = objParametriAgenda.Piva

                    movDet.Contabilizzato = NONCONTABILE

                    If Cau_Mov = CAU_SCARICO Then
                        movDet.Movimenti_Destinazioni = New List(Of Movimento_Destinazione)
                        Dim movDest As New Movimento_Destinazione
                        movDest.Id_Agenda = objParametriAgenda.Id_Agenda
                        movDest.Piva = objParametriAgenda.Piva
                        movDest.Qta = 0
                        movDest.Id_Destinazione = DR(j).Item("Centro_Cod")
                        movDet.Movimenti_Destinazioni.Add(movDest)
                    End If

                    movimento.Movimenti_Dettagli.Add(movDet)
                Next
                MovimentiCosti.Add(movimento)
            End If
        Next

        objParametriAgenda.Movimenti = MovimentiCosti


    End Sub
#End Region

#Region "Default sia aziendali che dell'utente"
    Private Sub DefaultAziendali(caricaDisciplinare As Boolean, caricaNote As Boolean, caricaCosti As Boolean, caricaMagazzino As Boolean)

        If objParametriAgenda.Tipo_Operazione <> TipiEnumerativi.enum_TipoOperazioneDB.Scrittura Then
            Exit Sub
        End If

        ''''''''''''''''''''''''''''''''''''''''
        ''''''' CARICO IL DISCIPLINARE '''''''''
        ''''''''''''''''''''''''''''''''''''''''
        If caricaDisciplinare AndAlso caricaDefaultAziendale_Disciplinare = True AndAlso
            Not (objParametriAgenda.TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua AndAlso objParametriAgenda.Lav_Cod = LAVCOD_DISTRIBUZIONE_AMMENDANTI) AndAlso
            Not (objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna AndAlso ricetta_cod <> "0" AndAlso ricetta_cod <> "") Then
            Dim ic_r As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim val As String = ic_r.Leggi_Codice_from_Imprese_Codici(objParametriAgenda.Piva, enum_CodiciAnagrafe.Disciplinare_Aziendale_Default, objParametri_Server)
            If val <> "" Then
                impostaDisciplinareDaPreferenza(val)
            End If
        End If


        ''''''''''''''''''''''''''''''''''''''''
        ''''''''''' CARICO LE NOTE '''''''''''''
        ''''''''''''''''''''''''''''''''''''''''
        If caricaNote Then

            Dim NotaUtilizzo_Cod As enum_Note_Intervento_Utilizzo = enum_Note_Intervento_Utilizzo.QuadernoCampagna

            If Not objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna Then
                NotaUtilizzo_Cod = enum_Note_Intervento_Utilizzo.Ricetta
            End If

            Dim objProfilazioneR As New AgronicaCoreProfilazioneBIZ.Profilazione_R
            Dim DT_note As DataTable = objProfilazioneR.LeggiProfilazioneNote_In_Cascata(objParametriAgenda.Piva,
                                         NotaUtilizzo_Cod,
                                         objParametriAgenda.Lav_Cod,
                                         objParametriAgenda.Veg_Cod.Split("/")(0),
                                         objParametri_Server)

            'sce
            For x = 0 To CBL_Consigli.Items.Count - 1
                CBL_Consigli.Items(x).Selected = False
            Next
            If CBL_Meteo.Visible = True Then
                For x = 0 To CBL_Meteo.Items.Count - 1
                    CBL_Meteo.Items(x).Selected = False
                Next
            End If
            If CBL_VentoIntensita.Visible = True Then
                For x = 0 To CBL_VentoIntensita.Items.Count - 1
                    CBL_VentoIntensita.Items(x).Selected = False
                Next
            End If
            If CBL_VentoDirezione.Visible = True Then
                For x = 0 To CBL_VentoDirezione.Items.Count - 1
                    CBL_VentoDirezione.Items(x).Selected = False
                Next
            End If
            If CBL_Temperatura.Visible = True Then
                For x = 0 To CBL_Temperatura.Items.Count - 1
                    CBL_Temperatura.Items(x).Selected = False
                Next
            End If
            If CBL_Orario.Visible = True Then
                For x = 0 To CBL_Orario.Items.Count - 1
                    CBL_Orario.Items(x).Selected = False
                Next
            End If
            If CBL_Motivazione.Visible = True Then
                For x = 0 To CBL_Motivazione.Items.Count - 1
                    CBL_Motivazione.Items(x).Selected = False
                Next
            End If

            Dim j As Integer = 0
            For i As Integer = 0 To DT_note.Rows.Count - 1
                For j = 0 To CBL_Consigli.Items.Count - 1
                    If CBL_Consigli.Items(j).Value = DT_note.Rows(i).Item("nota_cod") Then
                        CBL_Consigli.Items(j).Selected = True
                        Exit For
                    End If
                Next
                If CBL_Meteo.Visible = True Then
                    For j = 0 To CBL_Meteo.Items.Count - 1
                        If CBL_Meteo.Items(j).Value = DT_note.Rows(i).Item("nota_cod") Then
                            CBL_Meteo.Items(j).Selected = True
                            Exit For
                        End If
                    Next
                End If
                If CBL_VentoIntensita.Visible = True Then
                    For j = 0 To CBL_VentoIntensita.Items.Count - 1
                        If CBL_VentoIntensita.Items(j).Value = DT_note.Rows(i).Item("nota_cod") Then
                            CBL_VentoIntensita.Items(j).Selected = True
                            Exit For
                        End If
                    Next
                End If
                If CBL_VentoDirezione.Visible = True Then
                    For j = 0 To CBL_VentoDirezione.Items.Count - 1
                        If CBL_VentoDirezione.Items(j).Value = DT_note.Rows(i).Item("nota_cod") Then
                            CBL_VentoDirezione.Items(j).Selected = True
                            Exit For
                        End If
                    Next
                End If
                If CBL_Temperatura.Visible = True Then
                    For j = 0 To CBL_Temperatura.Items.Count - 1
                        If CBL_Temperatura.Items(j).Value = DT_note.Rows(i).Item("nota_cod") Then
                            CBL_Temperatura.Items(j).Selected = True
                            Exit For
                        End If
                    Next
                End If
                If CBL_Orario.Visible = True Then
                    For j = 0 To CBL_Orario.Items.Count - 1
                        If CBL_Orario.Items(j).Value = DT_note.Rows(i).Item("nota_cod") Then
                            CBL_Orario.Items(j).Selected = True
                            Exit For
                        End If
                    Next
                End If
                If CBL_Motivazione.Visible = True Then
                    For j = 0 To CBL_Motivazione.Items.Count - 1
                        If CBL_Motivazione.Items(j).Value = DT_note.Rows(i).Item("nota_cod") Then
                            CBL_Motivazione.Items(j).Selected = True
                            Exit For
                        End If
                    Next
                End If
            Next

        End If


        ''''''''''''''''''''''''''''''''''''''''
        ''''' CARICO MACCHINE E CONTATTI '''''''
        ''''''''''''''''''''''''''''''''''''''''
        If caricaCosti Then
            'elimino le macchine salvate
            objParametriAgenda.Movimenti = New List(Of Movimento)
            Costruisci_DT_Scarico()

            Dim objProfilazioneR As New AgronicaCoreProfilazioneBIZ.Profilazione_R
            Dim DS_Macchine As DataSet = objProfilazioneR.LeggiProfilazioneMacchine_In_Cascata(objParametriAgenda.Piva,
                                                 objParametriAgenda.Lav_Cod, objParametriAgenda.Veg_Cod.Split("/")(0),
                                                 objParametri_Server)


            'Creo le liste
            Dim listaMacCod As New List(Of String)
            Dim listaMinMac As New List(Of Int32)
            Dim listaOreMac As New List(Of Int32)
            Dim listaCodContatto As New List(Of String)
            Dim listaOreContatto As New List(Of Int32)
            Dim listaMinContatto As New List(Of Int32)

            For Each dtDati As DataTable In DS_Macchine.Tables
                For Each drDati In dtDati.Rows
                    If (dtDati.TableName.Equals("macXlav")) Then
                        'cerco il check e lo seleziono
                        If Not listaMacCod.Contains(drDati("mac_cod")) Then
                            listaMacCod.Add(drDati("mac_cod"))
                            listaOreMac.Add(drDati("ore"))
                            listaMinMac.Add(drDati("minuti"))
                        End If
                    ElseIf (dtDati.TableName.Equals("contXlav")) Then
                        If Not listaCodContatto.Contains(drDati("cod_risum")) Then
                            listaCodContatto.Add(drDati("cod_risum"))
                            listaOreContatto.Add(drDati("ore"))
                            listaMinContatto.Add(drDati("minuti"))
                        End If
                    End If
                Next
            Next




            Dim dtScarico As DataTable = Session("dtScarico")

            Dim objPrezzofromprodotti As New AgronicaCoreContabDAL.Prodotti_Costi_R
            'PARCO MACCHINE
            For i = 0 To listaMacCod.Count - 1
                Dim Mezzo As Integer = 0
                Dim Prezzo As Decimal = 0
                Dim Centro As String = "Parco Macchine"
                Dim Tipo_Centro As String = "PM"
                Dim Centro_Cod As Integer = -1
                Dim Udm_Cod As Integer = 0

                objPrezzofromprodotti.Prezzo_from_Prodotto(objParametri_Server.PivaSuperUser,
                                 1,
                                 0,
                                 listaMacCod(i),
                                 objParametriAgenda.Data,
                                 Mezzo,
                                 Udm_Cod,
                                 Prezzo,
                                 "", objParametri_Server)


                Dim Udm_Des As String = ""


                If IsDBNull(Mezzo) Then
                    Udm_Des = "indefinito"
                    Udm_Cod = "-1"
                Else
                    Select Case Mezzo
                        Case TipiEnumerativi.enum_TipoMezzo.Ettaro
                            Udm_Des = "ha"
                            Udm_Cod = "1"
                        Case TipiEnumerativi.enum_TipoMezzo.Ora
                            Udm_Des = "ora"
                            Udm_Cod = "2"
                        Case TipiEnumerativi.enum_TipoMezzo.Indefinito
                            Udm_Des = "indefinito"
                            Udm_Cod = "-1"
                    End Select
                End If


                'Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
                'Dim DT = objContab.MacchinaDes(objParametriAgenda.Piva,
                '                       objParametriAgenda.Data,
                '                       listaMacCod(i),
                '                        "", "",
                '                       objParametri_Server)
                'objContab = Nothing

                '(26/028/2019 fede) sostituita query per filtrare solo le macchine visibili all'utente (macchine assegnate ad un centro)
                Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
                Dim DT = objContab.LeggiMacchine_xCostiAccessori(objParametriAgenda.Piva,
                                       objParametriAgenda.Data,
                                        " PM.Mac_Cod=" & listaMacCod(i), "",
                                       objParametri_Server)
                objContab = Nothing




                Dim Categoria_Des As String = ""
                Dim Risorsa_Des As String = ""
                Dim Costo_Unitario As String = ""
                Dim Costo As String = ""
                If DT.Rows.Count > 0 Then

                    Categoria_Des = DT.Rows(0).Item("Col_0")
                    'Risorsa_Des = String.Join(" - ", {Categoria_Des, DT.Rows(0).Item("Ditta_Des"), DT.Rows(0).Item("Modello"), DT.Rows(0).Item("Mac_Des")}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                    Risorsa_Des = String.Join(" - ", {DT.Rows(0).Item("Ditta_Des"), DT.Rows(0).Item("Modello"), DT.Rows(0).Item("Col_1")}.Where(Function(s) (Not IsDBNull(s) AndAlso Not String.IsNullOrEmpty(s))))
                    'If DT.Rows(0).Item("Mac_Des") <> "" Then
                    '    Risorsa_Des = DT.Rows(0).Item("Mac_Des")
                    'Else
                    '    Risorsa_Des = DT.Rows(0).Item("Ditta_Des") & " " & DT.Rows(0).Item("Modello")
                    'End If

                    Costo_Unitario = Format(Prezzo * 1, "0.00")
                    Costo = Format(Prezzo * 0, "0.00")



                    'verifico se ho l'acqua impostata 
                    If QtaAcqua.Value = "" OrElse QtaAcqua.Value = "0" Then
                        If Not IsDBNull(DT.Rows(0).Item("Taratura_Ugello")) Then
                            If IsNumeric(DT.Rows(0).Item("Taratura_Ugello")) Then
                                Dim Acqua As Decimal = DT.Rows(0).Item("Taratura_Ugello")
                                QtaAcqua.Value = Acqua
                            End If
                        End If
                    End If


                    InserisciRiga_dtScarico(Centro,
                                        0,
                                        Centro_Cod,
                                        Categoria_Des,
                                        Risorsa_Des,
                                        Udm_Des,
                                        0,
                                        Udm_Cod,
                                        MACCHINE,
                                        0,
                                        Tipo_Centro,
                                        0,
                                        0,
                                        listaMacCod(i),
                                        Costo_Unitario,
                                        Costo,
                                        0,
                                        "",
                                        0,
                                        "",
                                        listaOreMac(i),
                                        listaMinMac(i),
                                        0, 0, 0,
                                            dtScarico)
                End If
            Next


            'RAPPORTI CONTABILI
            For i = 0 To listaCodContatto.Count - 1

                Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

                Dim Str As String = " ( (Rapporti_Contabili.Cod_Rapporto in (-1,-4,-5,-6,-12)) or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Terzista=1 )  "
                Dim Dt_Manodopera As DataTable = objRapp_Contabili.RapportiContabilixCostiAccessoribyCod_RisUm(objParametriAgenda.Piva,
                                                                                     listaCodContatto(i),
                                                                                     False,
                                                                                     False,
                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                      Str, " Contatti.Sa_Cod desc, Contatti.Rag_Soc, cognome, nome ASC  ", objParametri_Server)

                If Dt_Manodopera.Rows.Count = 0 Then
                    'Exit For
                    Continue For
                End If

                Dim Cod_Rapporto As String = Dt_Manodopera.Rows(0).Item("Cod_Rapporto")
                Dim Centro_Cod As Integer = 0
                Dim Riga As Integer = 0
                Dim Elem_Cod As Integer = 0
                Dim Tipo_Centro As String = ""
                Dim Centro As String = ""

                Select Case Cod_Rapporto
                    Case -1, -4, -6
                        Centro = "Manodopera"
                        Tipo_Centro = "MD"
                        Centro_Cod = -2
                    Case -5
                        Centro_Cod = -2
                        Centro = "C/Terzisti"
                        Tipo_Centro = "CT"
                    Case -12
                        Centro_Cod = -2
                        Centro = "Tecnico Responsabile"
                        Tipo_Centro = "TR"
                    Case Else
                        Centro = "Manodopera"
                        Tipo_Centro = "MD"
                        Centro_Cod = -2
                End Select

                'If Dt_Manodopera.Rows(0).Item("Terzista") = 1 Then
                '    Centro_Cod = -3
                '    Centro = "C/Terzisti"
                '    Tipo_Centro = "CT"
                'End If

                'If Dt_Manodopera.Rows(0).Item("Dipendente") = 1 Then
                '    Centro = "Manodopera"
                '    Tipo_Centro = "MD"
                '    Centro_Cod = -2
                'End If


                Dim Categoria_Des As String = Dt_Manodopera.Rows(0).Item("Rapporto_Des")
                Dim Risorsa_Des As String = Dt_Manodopera.Rows(0).Item("Rag_Soc")
                Elem_Cod = 0
                Dim Udm_Des As String = "ora"
                Dim Udm_Cod As Integer = 2
                Dim Mat_Cod As Integer = Dt_Manodopera.Rows(0).Item("Cod_Risum")
                Dim Pro_Cod As Integer = 0

                'If IsDBNull(Dt_Manodopera.Rows(0).Item("Mezzo")) Then
                '    Udm_Des = "ha"
                '    Udm_Cod = "1"
                'Else
                '    If Dt_Manodopera.Rows(0).Item("Mezzo") = TipiEnumerativi.enum_TipoMezzo.Ettaro Then
                '        Udm_Des = "ha"
                '        Udm_Cod = "1"
                '    Else
                '        Udm_Des = "ora"
                '        Udm_Cod = "2"
                '    End If
                'End If


                If IsDBNull(Dt_Manodopera.Rows(0).Item("Mezzo")) Then
                    Udm_Des = "indefinito"
                    Udm_Cod = -1
                Else
                    Select Case Dt_Manodopera.Rows(0).Item("Mezzo")
                        Case TipiEnumerativi.enum_TipoMezzo.Ettaro
                            Udm_Des = "ha"
                            Udm_Cod = "1"
                        Case TipiEnumerativi.enum_TipoMezzo.Ora
                            Udm_Des = "ora"
                            Udm_Cod = "2"
                        Case TipiEnumerativi.enum_TipoMezzo.Indefinito
                            Udm_Des = "indefinito"
                            Udm_Cod = "-1"
                    End Select
                End If

                'Qta_Ril = Dt_Manodopera.Rows(0).Item("Col_4")

                Dim Costo_Unitario, Costo As String
                'Costo_Unitario = Format(Dt_Manodopera.Rows(0).Item("Prezzo_Unitario") * 1, "0.00")
                'Costo = Format(Dt_Manodopera.Rows(0).Item("Prezzo_Unitario") * 0, "0.00")
                If IsDBNull(Dt_Manodopera.Rows(0).Item("Prezzo_Unitario")) Then
                    Costo_Unitario = "0.00"
                    Costo = "0.00"
                Else
                    Costo_Unitario = Format(Dt_Manodopera.Rows(0).Item("Prezzo_Unitario") * 1, "0.00")
                    Costo = Format(Dt_Manodopera.Rows(0).Item("Prezzo_Unitario") * 0, "0.00")
                End If


                Dim Ditta_Cod As String = 0

                ''    Case -3 'TERZISTI

                ''Categoria_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")
                ''Risorsa_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_1")

                ''Elem_Cod = 0
                ''Pro_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_3")
                ''Mat_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_8")

                ''Udm_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_2")
                ''Udm_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_5")

                ''Qta_Ril = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_4")
                ''Costo_Unitario = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                ''Costo = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                ''Ditta_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_7")


                InserisciRiga_dtScarico(Centro,
                                    0,
                                    Centro_Cod,
                                    Categoria_Des,
                                    Risorsa_Des,
                                    Udm_Des,
                                    0,
                                    Udm_Cod,
                                    Elem_Cod,
                                    Riga,
                                    Tipo_Centro,
                                    Pro_Cod,
                                    Ditta_Cod,
                                    Mat_Cod,
                                    Costo_Unitario,
                                    Costo,
                                    0,
                                    "",
                                    0,
                                    "",
                                    listaOreContatto(i),
                                    listaMinContatto(i),
                                    0, 0,
                                      Cod_Rapporto,
                                        dtScarico)
            Next

            CaricaGriglia_CostiAccessori_xJSON(dtScarico)

            'aggiorno i costi
            Session("dtScarico") = dtScarico
            AggiornaGridViewCostiAccessoriVisibili()


            SalvaCostiAccessori_Default()

        End If

        If caricaMagazzino Then
            AggiornaMagazzino()
        End If

    End Sub

    Private Sub defaultSuperuser()
        Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI") = False
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Dt_Impostazioni_Super As DataTable = ObjUtenti.Leggi(0,
                                                    2,
                                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    "",
                                                    "",
                                                    objParametri_Utenti)

        For i = 0 To Dt_Impostazioni_Super.Rows.Count - 1
            Select Case CInt(Dt_Impostazioni_Super.Rows(i).Item("Impostazione_Cod"))
                Case enum_Impostazioni_Utenti.SUPERUSER_COD_PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI
                    Select Case Dt_Impostazioni_Super.Rows(i).Item("Impostazione_Valore_1")
                        Case "0"
                            Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI") = False
                        Case "1"
                            Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI") = True
                        Case Else
                            Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI") = False
                    End Select
            End Select
        Next
    End Sub

    Private Sub DefaultUtente()
        'leggo le eventuali IMPOSTAZIONI UTENTE
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Dt_Impostazioni As New DataTable
        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura OrElse
           objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
            Dt_Impostazioni = ObjUtenti.Leggi(0,
                                              1,
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "",
                                              "",
                                              objParametri_Utenti)

        End If

        'Dim impostatodisciplinarepredefinito As Boolean = False
        If Dt_Impostazioni IsNot Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then

            For i = 0 To Dt_Impostazioni.Rows.Count - 1

                'Impostazioni sia per scrittura che modifica
                Select Case CInt(Dt_Impostazioni.Rows(i).Item("Impostazione_Cod"))

                    Case enum_Impostazioni_Utenti.UTENTE_COD_CHILI_LITRI
                        Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                            Case "0"
                                Session("Chili_Litri") = False
                            Case Else
                                Session("Chili_Litri") = True
                        End Select

                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO
                        Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                            Case "0"
                                Session("UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO") = False
                            Case Else
                                Session("UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO") = True
                        End Select

                    Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_NOTE
                        Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                            Case "0"
                                Session("UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_NOTE") = False
                            Case Else
                                Session("UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_NOTE") = True
                        End Select
                    Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_MACCHINE
                        Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                            Case "0"
                                Session("UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_MACCHINE") = False
                            Case Else
                                Session("UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_MACCHINE") = True
                        End Select
                    Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_OPERATORE
                        Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                            Case "0"
                                Session("UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_OPERATORE") = False
                            Case Else
                                Session("UTENTE_COD_DEFAULT_BLOCCO_INSERIMENTO_OPERATORE") = True
                        End Select


                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE
                        Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                            Case "0"
                                Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE") = False
                            Case Else
                                Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE") = True
                        End Select

                    Case enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS
                        Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                            Case "0"
                                Session("UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS") = False
                            Case Else
                                Session("UTENTE_COD_BLOCCA_SE_SUPERA_LIMITIMAS") = True
                        End Select

                        'Utilizzo Magazzino  --> valore: 0=no 1=si
                    Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO
                        Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                            Case "0"
                                Session("UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO") = False
                            Case Else
                                Session("UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO") = True
                        End Select


                    Case Else

                        'Impostazioni solo per scrittura
                        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura AndAlso
                           (ricetta_cod = "0" OrElse ricetta_cod = "") AndAlso
                           (ricetta_operazione_cod = "0" OrElse ricetta_operazione_cod = "") Then

                            Select Case CInt(Dt_Impostazioni.Rows(i).Item("Impostazione_Cod"))

                                ''Utilizzo Magazzino  --> valore: 0=no 1=si
                                'Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO
                                '    Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                                '        Case "0"
                                '            Session("UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO") = False
                                '            'ComboMagazzini.Indice_Combo = 0
                                '        Case Else
                                '            Session("UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO") = True
                                '            'If ComboMagazzini.N_Magazzini > 1 Then
                                '            '    impostoMagazzinoAziendale()
                                '            '    objParametriAgenda.Fabbricato = ComboMagazzini.Valore_Combo
                                '            'End If
                                '    End Select

                                ''Impostazione Tutti i Centri
                                Case enum_Impostazioni_Utenti.UTENTE_COD_TUTTI_I_CENTRI
                                    If Not IsPostBack Then
                                        Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                                            Case "0"
                                                'non apporto modifiche 
                                            Case Else
                                                If objParametriAgenda.OperazioneMulticentro AndAlso objParametriAgenda.Sa_Cod = "0" Then
                                                    Me.ComboCentroAziendale.ddl_CentroAziendale.SelectedValue = 0
                                                    objParametriAgenda.Sa_Cod = 0
                                                    AggiornaSpecie()
                                                    CaricaGriglia_Impianti()
                                                    GridView_Impianti.Columns(2).Visible = True
                                                Else
                                                    'se non è permesso il multicentro non faccio nulla, altrimenti mi precarica gli impianti di tutti i centri
                                                    'E' una toppa, funziona con menu vecchio ma con menu nuovo parzialmente
                                                End If

                                        End Select
                                    End If


                                'DPI  --> valore: disciplinare predefinito es 34/1
                                Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_DPI_PREDEFINITO

                                    'If ricetta_cod = "" AndAlso
                                    'escludo il default per distr amm di PUA 
                                    'ribaltamento di ricette
                                    If caricaDefaultUtente_Disciplinare = True AndAlso
                                       Not (objParametriAgenda.TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua AndAlso objParametriAgenda.Lav_Cod = LAVCOD_DISTRIBUZIONE_AMMENDANTI) AndAlso
                                       Not (objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna AndAlso ricetta_cod <> "0" AndAlso ricetta_cod <> "") Then

                                        Dim preferenza As String = Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                                        impostaDisciplinareDaPreferenza(preferenza)

                                        Dim Str As New StringBuilder
                                        Str.AppendLine("$(document).ready(function () {")
                                        Str.AppendLine("    $('#" & BTN_ComboDisciplinari1.ClientID & "').click(); ")
                                        Str.AppendLine("});")
                                        'ScriptManager.RegisterClientScriptBlock(UpdatePanelPerScript, UpdatePanelPerScript.GetType(),
                                        '                                 String.Format("jQuery_{0}", txt_DataOperazione.ClientID), Str.ToString, True)

                                        ScriptManager.RegisterStartupScript(UpdatePanelPerScript, UpdatePanelPerScript.GetType(),
                                               String.Format("jQuery_{0}", UpdatePanelPerScript.ClientID), Str.ToString, True)


                                    End If

                            End Select

                            'Impostazioni solo per mopdifica
                        ElseIf objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Modifica Then




                        End If

                End Select



            Next
        End If

    End Sub

    Private Sub SelezionaDatiDaObjParametriAgenda(ByVal selezionaCentro As Boolean, ByVal selezionaImpianti As Boolean)
        If selezionaCentro AndAlso objParametriAgenda.Sa_Cod <> "" AndAlso objParametriAgenda.Sa_Cod <> "0" Then
            'objParametriAgenda.Sa_Cod = ComboCentroAziendale.Valore_Combo.Split("|")(1)
            'ComboCentroAziendale.ddl_CentroAziendale.Text = objParametriAgenda.Piva & "|" & objParametriAgenda.Sa_Cod
            'ComboCentroAziendale.Piva = objParametriAgenda.Piva
            'ComboCentroAziendale.Valore_Combo = objParametriAgenda.Piva & "|" & objParametriAgenda.Sa_Cod
        End If

        'If selezionaMagazzino AndAlso objParametriAgenda.Fabbricato <> "" AndAlso objParametriAgenda.Fabbricato <> "0" Then

        'End If

    End Sub

#End Region

    'evento associato ad un btn non visualizzato, viene richiamato il click lato js che scatena quindi il postback
    Protected Sub Wrap_Client_PostedBack(ByVal sender As Object, ByVal e As EventArgs)
        If (Not fooName_PostedBack.Value.Equals("")) Then
            'nel campo hidden ho il nome di funzione da richiamare per il postback
            Dim metodo As System.Reflection.MethodInfo = Me.GetType().GetMethod(fooName_PostedBack.Value)
            If (Not IsNothing(metodo)) Then
                Dim objParam() As Object = {Me, EventArgs.Empty}
                metodo.Invoke(Me, objParam)
            End If
            'svuoto
            fooName_PostedBack.Value = ""
        End If
    End Sub

#Region "EventiCombo"



    'CAMBIO SPECIE
    Public Sub ComboSpecie_IndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        objParametriAgenda.Veg_Cod = ComboSpecie.Valore_Combo '.Split("/")(0)
    End Sub

    'CAMBIO DPI
    Public Sub ComboDisciplinari1_IndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        objParametriAgenda.Disciplinare = ComboDisciplinari1.Valore_Combo
    End Sub

    'CAMBIO MAGAZZINO
    Public Sub ComboMagazzini_IndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        objParametriAgenda.Fabbricato = ComboMagazzini.Valore_Combo
    End Sub

    'CAMBIO OPERAZIONE
    Public Sub ComboOperazioni_IndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        objParametriAgenda.Lav_Cod = ComboOperazione.Valore_Combo
        objParametriAgenda.Lav_Des = ComboOperazione.Testo_Combo
    End Sub

    'CAMBIO CENTRO AZIENDALE
    Public Sub ComboCentroAziendale_IndexChanged(ByVal sender As Object, ByVal e As EventArgs)

        objParametriAgenda.Sa_Cod = ComboCentroAziendale.Valore_Combo.Split("|")(1)

        Dim app As String
        app = ComboSpecie.Valore_Combo.Split("/")(0)

        ComboSpecie.Piva = objParametriAgenda.Piva
        ComboSpecie.Sa_Cod = objParametriAgenda.Sa_Cod
        ComboSpecie.Data = objParametriAgenda.Data
        ComboSpecie.ConsideraTerrenoNudo = CONSIDERATERRENONUDO
        ComboSpecie.Valore_Combo = objParametriAgenda.Veg_Cod

        ComboSpecie.Valore_Combo = app

        'UpdatePanelSpecie.Update()
        UpdatePanelSpecie.Update()

    End Sub

#End Region

#Region "Bottoni Nascosti"
    Protected Sub BTN_ChangeData_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ChangeData.Click

        If IsDate(txt_DataOperazione.Text) Then

            objParametriAgenda.Data = txt_DataOperazione.Text
            txt_DataOperazione.Text = objParametriAgenda.Data.ToShortDateString
            Dim dataMin As Date = AGRODATAINIZIO
            Dim dataMax As Date = AGRODATAFINE
            Dim SportelloAperto As Boolean = True
            If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura OrElse objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
                Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R
                objPratiche.Data_Sportello_Da_Servizio(objParametriAgenda.Piva,
                                                       enum_Servizi.Quaderno_Campagna_Caa,
                                                       DateTime.Now,
                                                       SportelloAperto,
                                                       dataMin,
                                                       dataMax,
                                                       objParametri_Server,
                                                       objParametri_Utenti)

                objPratiche.Sportello_ChiamataSecondaria_SeNessunCambiamento(objParametriAgenda.Piva,
                                                                                        enum_Servizi.QuadernoCampagnaBio,
                                                                                        DateTime.Now,
                                                                                        True,
                                                                                        AGRODATAINIZIO,
                                                                                        AGRODATAFINE,
                                                                                        SportelloAperto,
                                                                                        dataMin,
                                                                                        dataMax,
                                                                                        objParametri_Server,
                                                                                        objParametri_Utenti)
                If objParametriAgenda.Data < dataMin Then
                    objParametriAgenda.Data = dataMin
                    txt_DataOperazione.Text = dataMin.ToShortDateString
                    Messaggi.AgroMsgBox("Non è possibile inserire un'operazione prima del " & dataMin.ToShortDateString & ". Sportello chiuso", Page, , UpdatePanelPerScript)
                End If

                If objParametriAgenda.Data > dataMax Then
                    objParametriAgenda.Data = dataMax
                    txt_DataOperazione.Text = dataMax.ToShortDateString
                    Messaggi.AgroMsgBox("Non è possibile inserire un'operazione dopo il " & dataMax.ToShortDateString & ". Sportello chiuso", Page, , UpdatePanelPerScript)
                End If
            End If


        End If

        'aggiorno specie 
        AggiornaSpecie()

        'aggiorno disciplinare        
        'CaricaComboDisciplinariExteso()

        'aggiorno Impianti
        CaricaGriglia_Impianti()

        'ricarico le ricette
        CaricaElencoRicette()

        Popola_Manodopera()

    End Sub

    Protected Sub BTN_ComboOperazione_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboOperazione.Click
        'Cambio la slave

        If ComboOperazione.Valore_Combo = LAVCOD_RACCOLTA Then
            If objParametriAgenda.Sa_Cod = "0" OrElse objParametriAgenda.Sa_Cod = "" Then
                ComboOperazione.ddl_Operazioni.SelectedValue = objParametriAgenda.Lav_Cod
                Messaggi.AgroMsgBox("Per la raccolta occorre selezionare prima un centro aziendale", Page, , UpdatePanelPerScript)
                Exit Sub
            End If
        End If

        objParametriAgenda.Lav_Cod = ComboOperazione.Valore_Combo
        objParametriAgenda.Lav_Des = ComboOperazione.Testo_Combo

        Dim fromBootstrapToBootstrap As Boolean = False
        If objParametriAgenda.SitoOrigine <> Enum_SiteRedirector.GiasNG Then
            fromBootstrapToBootstrap = True
        End If

        Dim strErrore As String = ""
        Dim OpUtil As New AgronicaCoreModello.Utility_Operazioni
        Dim PaginaLink As String = OpUtil.LinkPagina_from_LavCod_NEW(objParametriAgenda.Lav_Cod, objParametriAgenda, strErrore:=strErrore, fromBootstrapToBootstrap:=fromBootstrapToBootstrap, redirectPortateDomandaIrrigua:=True)

        If strErrore <> "" Then
            Messaggi.AgroMsgBox(strErrore, Page, , UpdatePanelPerScript)
            Exit Sub
        End If

        If PaginaLink = "" Then
            Messaggi.AgroMsgBox("Operazione in manutenzione", Page, , UpdatePanelPerScript)
            Exit Sub
        End If

        Session.Remove("dtScarico")
        'Dim c As IPostBackEventHandler = DirectCast(BTN_ComboSpecie, IPostBackEventHandler)
        'c.RaisePostBackEvent(String.Empty)

        objParametriAgenda.Disciplinare = "0"
        CaricaElencoRicette()

        '(27/03/2020 fede) modificato per il salva ed aggiungi a dettaglio ricetta
        If ricetta_cod <> "" AndAlso ricetta_cod <> "0" Then
            PaginaLink &= "?r=" & Stringa_Codifica(ricetta_cod, AgroKey_EncoderDecoder, Server)
        End If
        Response.Redirect(PaginaLink)

    End Sub

    Protected Sub BTN_ComboCentroAziendale_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboCentroAziendale.Click
        objParametriAgenda.Sa_Cod = ComboCentroAziendale.Valore_Combo
        AggiornaSpecie()
        CaricaGriglia_Impianti()
    End Sub

    Protected Sub BTN_ComboSpecie_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboSpecie.Click

        BTN_ComboSpecie_ClickExtracted()

    End Sub

    Private Sub BTN_ComboSpecie_ClickExtracted()
        objParametriAgenda.Veg_Cod = ComboSpecie.Valore_Combo '.Split("/")(0)
        objParametriAgenda.Cul_Cod = "0"

        ''Lettura dei Default
        DefaultUtente()

        'DefaultAziendali(True, True, False, True) ' Non ricarico i costi
        DefaultAziendali(True, True, True, True)

        objParametriAgenda.Cul_Cod = "0"

        CaricaComboDisciplinariExteso()

        CaricaGriglia_Impianti()

        'ricarico la griglia delle ricette
        CaricaElencoRicette()
    End Sub


    Protected Sub BTN_Magazzini_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_Magazzini.Click
        objParametriAgenda.Fabbricato = ComboMagazzini.Valore_Combo

        Dim Str As New StringBuilder
        Str.AppendLine("$(document).ready(function () {")

        If ComboMagazzini.Valore_Combo <> "0" Then
            'ImgBtn_Carico_div.Visible = True
            'Str.AppendLine("    $('#" & ImgBtn_Carico_div.ClientID & "').removeClass('displaynone'); ")
            Str.AppendLine("    $('#" & ImgBtn_Carico_div.ClientID & "').css('display',''); ")
        Else
            'ImgBtn_Carico_div.Visible = False
            'Str.AppendLine("    $('#" & ImgBtn_Carico_div.ClientID & "').addClass('displaynone'); ")
            Str.AppendLine("    $('#" & ImgBtn_Carico_div.ClientID & "').css('display','none'); ")
        End If

        Str.AppendLine("});")

        ScriptManager.RegisterClientScriptBlock(UpdatePanelPerScript, UpdatePanelPerScript.GetType(),
                                                         String.Format("jQuery_{0}", txt_DataOperazione.ClientID), Str.ToString, True)


    End Sub

    Private Sub ComboDpiValorizzaPerPUA()
        Dim Dpi_Cod As Integer = objParametriAgenda.Cul_Cod
        If objParametriAgenda.TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua Then
            objParametriAgenda.Cul_Cod = 0
            If Dpi_Cod > 0 Then
                For c = 1 To Property_ComboDisciplinari.ddl_Disciplinari.Items.Count - 1
                    If Dpi_Cod = Split(Property_ComboDisciplinari.ddl_Disciplinari.Items(c).Value, "/")(0) Then

                        Dim Disciplinare_Valore As String = Property_ComboDisciplinari.ddl_Disciplinari.Items(c).Value
                        objParametriAgenda.Disciplinare = Disciplinare_Valore
                        Property_ComboDisciplinari.ddl_Disciplinari.SelectedIndex = c

                        Dim Str As New StringBuilder
                        Str.AppendLine("$(document).ready(function () {")
                        Str.AppendLine("  setTimeout(function(){  $('#" & BTN_ComboDisciplinari1.ClientID & "').click(); }, 0); ")
                        Str.AppendLine("});")
                        '                        ScriptManager.RegisterClientScriptBlock(UpdatePanelPerScript, UpdatePanelPerScript.GetType(),
                        '                       String.Format("jQuery_{0}", UpdatePanelPerScript.ClientID), Str.ToString, True)
                        ScriptManager.RegisterStartupScript(UpdatePanelPerScript, UpdatePanelPerScript.GetType(),
                                    String.Format("jQuery_{0}", UpdatePanelPerScript.ClientID), Str.ToString, True)
                    End If
                Next
            End If
        End If
    End Sub


    Private Sub AggiornaSpecie()

        'Dim FiltroAggiuntivo As String = ""

        ComboSpecie.Piva = objParametriAgenda.Piva
        ComboSpecie.Sa_Cod = objParametriAgenda.Sa_Cod
        ComboSpecie.Data = objParametriAgenda.Data

        ComboSpecie.ConsideraTerrenoNudo = CONSIDERATERRENONUDO
        ComboSpecie.DettagliTerrenoNudo = DETTAGLITERRENONUDO

        Dim leggiAncheBloccati As Boolean = False
        If Not IsNothing(Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI")) Then
            If Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI") = True Then
                leggiAncheBloccati = True
            End If
        End If
        ComboSpecie.leggiAncheImpiantiBloccati = leggiAncheBloccati

        ComboSpecie.PrimaRiga_Flag = True
        ComboSpecie.PrimaRiga_Value = "-1"
        ComboSpecie.PrimaRiga_Text = ""

        ComboSpecie.Carica_Tutte_Specie_Esistenti = False
        ComboSpecie.Veg_Cod_ModificaLettura = 0
        ComboSpecie.Usa_Filtro_Utente = True
        Select Case objParametriAgenda.Tipo_Operazione
            Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura, TipiEnumerativi.enum_TipoOperazioneDB.Modifica
                'se sono in modifica o lettura imposto la Veg_Cod_ModificaLettura in modo che la specie sia comunque inserita nella combo anche se filtrata
                ComboSpecie.Veg_Cod_ModificaLettura = objParametriAgenda.Veg_Cod.Split("/")(0)
            Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                ComboSpecie.Veg_Cod_ModificaLettura = 0
        End Select

        Select Case objParametriAgenda.Lav_Cod
            Case LAVCOD_TRAPIANTO, LAVCOD_SEMINA, LAVCOD_SOVESCIO, LAVCOD_SOVESCIO
                'Grilli: 29/06/2018 Fabrizio ha detto di togliere la limitazione in seguito a segnalazione di Selvi che non può trapiantare
                'se sono in semina o trapianto carico tutte le specie )filtrando comunque tra quelle presenti nelle impostazioni utenti
                'LAVCOD_SOVESCIO:  per il sovescio non carico tutte le specie, potrei al massimo caricare solo
                'gli impianti di arboree, ma lascio come le altre operazioni per ora
                '    Select Case objParametriAgenda.Lav_Cod
                '        Case LAVCOD_SEMINA
                '            FiltroAggiuntivo = " ( (SpecieVegetali.Gru_Cod = '2') OR (SpecieVegetali.Gru_Cod = '3') ) "
                '        Case LAVCOD_TRAPIANTO
                '            FiltroAggiuntivo = " ( (SpecieVegetali.Gru_Cod = '1') OR (SpecieVegetali.Gru_Cod = '3') OR (SpecieVegetali.Veg_Cod = '6') OR (SpecieVegetali.Veg_Cod = '335')) "
                '    End Select
                ComboSpecie.CaricaxPDC = False
                'ComboSpecie.Carica_Tutte_Specie_Esistenti = True
                'Grilli: 11/04/2019 per la semina nuova mostro le sole specie che ho in anagrafica
                ComboSpecie.Carica_Tutte_Specie_Esistenti = False
        End Select

        'ComboSpecie.FiltroAggiuntivo = FiltroAggiuntivo

        ''Dim CampoCod As Integer = 0
        ''If ComboCampo.Testo_Combo <> "" Then
        ''    If Not IsNothing(Split(ComboCampo.Valore_Combo, "/")(2)) Then
        ''        CampoCod = CInt(Split(ComboCampo.Valore_Combo, "/")(2))
        ''    End If
        ''End If
        ''ComboSpecie.Campo_Cod = CampoCod

        'Dim Valori_Combo As List(Of String) = ComboCampo.Valori_Combo
        'ComboSpecie.Campi = Valori_Combo

        ComboSpecie.CaricaComboSpecie()

        'imposto il valore
        If objParametriAgenda.Veg_Cod <> "-1" Then
            ComboSpecie.Valore_Combo = objParametriAgenda.Veg_Cod
        Else
            'se ho una sola specie, quindi se ho due valori nella combo imposto la specie
            Try
                If ComboSpecie.PrimaRiga_Flag AndAlso objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Scrittura AndAlso ComboSpecie.ddl_Specie.Items.Count = 2 Then
                    'ComboSpecie.ddl_Specie.SelectedIndex = 1
                    ComboSpecie.Valore_Combo = ComboSpecie.ddl_Specie.Items(1).Value
                    'BTN_ComboCentroAziendale_Click(Me, Nothing)


                    Dim Str As New StringBuilder
                    Str.AppendLine("$(document).ready(function () {")
                    Str.AppendLine("    $('#" & BTN_ChangeData.ClientID & "').click(); ")
                    Str.AppendLine("    $('#" & BTN_ComboSpecie.ClientID & "').click(); ")
                    Str.AppendLine("    $('#" & BTN_ComboDisciplinari1.ClientID & "').click(); ")
                    Str.AppendLine("});")
                    ScriptManager.RegisterClientScriptBlock(UpdatePanelPerScript, UpdatePanelPerScript.GetType(),
                                                     String.Format("jQuery_{0}", txt_DataOperazione.ClientID), Str.ToString, True)
                End If
            Catch ex As Exception

            End Try

            objParametriAgenda.Veg_Cod = ComboSpecie.Valore_Combo '.Split("/")(0)


        End If

        CaricaComboDisciplinariExteso()

        ComboDpiValorizzaPerPUA()

        Select Case objParametriAgenda.Tipo_Operazione
            Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura, TipiEnumerativi.enum_TipoOperazioneDB.Modifica
                'se sono in modifica o lettura imposto la Veg_Cod_ModificaLettura in modo che la specie sia comunque inserita nella combo anche se filtrata
                'ComboSpecie.Veg_Cod_ModificaLettura = objParametriAgenda.Veg_Cod.Split("/")(0)
                'ComboDisciplinari1.Valore_Combo = objParametriAgenda.Disciplinare
                ComboDisciplinari1.ddl_Disciplinari.SelectedIndex =
                                      ComboDisciplinari1.ddl_Disciplinari.Items.IndexOf(ComboDisciplinari1.ddl_Disciplinari.Items.FindByValue(
                                          objParametriAgenda.Disciplinare))
            Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                'ComboSpecie.Veg_Cod_ModificaLettura = 0

                If ricetta_cod <> "" Then
                    ComboDisciplinari1.ddl_Disciplinari.SelectedIndex =
                                      ComboDisciplinari1.ddl_Disciplinari.Items.IndexOf(ComboDisciplinari1.ddl_Disciplinari.Items.FindByValue(
                                          objParametriAgenda.Disciplinare))
                End If

        End Select

    End Sub

    Public Sub CaricaComboDisciplinariExteso()

        Dim _Includi_Biologico As Boolean = True

        ComboDisciplinari1.WS_Disciplinari_AgroWS_Disciplinari = objParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari
        ComboDisciplinari1.Includi_Biologico = _Includi_Biologico
        ComboDisciplinari1.Veg_Cod = objParametriAgenda.Veg_Cod.Split("/")(0)

        If objParametriAgenda.Veg_Cod.Split("/")(0) <> 0 Then
            div_AlertTerrenoNudo.Visible = False
        Else
            Select Case objParametriAgenda.Lav_Cod
                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE,
                     LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO,
                     LAVCOD_TRATTAMENTO_FITOREGOLATORE
                    div_AlertTerrenoNudo.Visible = True
            End Select
        End If



        Select Case objParametriAgenda.Lav_Cod

            Case LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_VISITA
                ComboDisciplinari1.Tipo_Testata = enum_Disciplinare_Tipo_Testata.Difesa
                ComboDisciplinari1.Includi_NessunoNessuno = False

            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE,
                 LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                ComboDisciplinari1.Tipo_Testata = enum_Disciplinare_Tipo_Testata.Difesa
                ComboDisciplinari1.Includi_NessunoNessuno = True
                If Not objParametriAgenda.Veg_Cod.Split("/")(0) > 0 Then
                    ComboDisciplinari1.Includi_Nessuno = False
                End If

            Case LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO
                ComboDisciplinari1.Tipo_Testata = enum_Disciplinare_Tipo_Testata.Diserbo
                ComboDisciplinari1.Includi_NessunoNessuno = True
                If Not objParametriAgenda.Veg_Cod.Split("/")(0) > 0 Then
                    ComboDisciplinari1.Includi_Nessuno = False
                End If

            Case LAVCOD_TRATTAMENTO_FITOREGOLATORE
                ComboDisciplinari1.Tipo_Testata = enum_Disciplinare_Tipo_Testata.Fitoregolatore
                ComboDisciplinari1.Includi_NessunoNessuno = True
                If Not objParametriAgenda.Veg_Cod.Split("/")(0) > 0 Then
                    ComboDisciplinari1.Includi_Nessuno = False
                End If

            Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA
                ComboDisciplinari1.Tipo_Testata = enum_Disciplinare_Tipo_Testata.Fertilizzazione
                ComboDisciplinari1.Includi_Nessuno = True
                If objParametriAgenda.Lav_Cod = LAVCOD_DISTRIBUZIONE_AMMENDANTI Then
                    ComboDisciplinari1.Includi_Dir_Nitrati = True
                End If
        End Select


        'If Session("permessoDPIPrivati") = False Then
        '    ComboDisciplinari1.Flag_Privato_Pubblico = 1
        'Else
        Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Select Case CBool(objAgroWeb.Flag_DisciplinarePrivato)
            Case True
                ComboDisciplinari1.Flag_Privato_Pubblico = 0
            Case Else
                ComboDisciplinari1.Flag_Privato_Pubblico = 1
        End Select
        'ComboDisciplinari1.Flag_Privato_Pubblico = 0
        'End If

        ComboDisciplinari1.FinestraTemporaleInizio = objParametriAgenda.Data
        ComboDisciplinari1.FinestraTemporaleFine = objParametriAgenda.Data
        ComboDisciplinari1.CaricaComboDisciplinari()


        'se ho come impostazioni utente il dpi lo imposto
        'metto un try catch in modo che non sia bloccante se ci sono errori
        Try

            If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura AndAlso
                Not IsNothing(ViewState("ImpostoDPINellaCombo")) AndAlso
                ViewState("ImpostoDPINellaCombo") = True AndAlso
                ComboDisciplinari1.ddl_Disciplinari.Items.Count > 1 Then

                If Not IsNothing(Session("DpiPredefinitoUtente")) AndAlso Session("DpiPredefinitoUtente") <> "0" Then

                    'se cambio la specie il dpi predefinito cambia, cambia il codice della specie,
                    'quini non posso fare un assegnamento diretto ma devo ciclare
                    Select Case objParametriAgenda.Lav_Cod
                        Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA
                            Dim disciplinare As String = Session("DpiFertPredefinitoUtente")
                            ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 0
                            For kk = 0 To ComboDisciplinari1.ddl_Disciplinari.Items.Count - 1
                                Dim val As String() = ComboDisciplinari1.ddl_Disciplinari.Items(kk).Value.Split("/")
                                If val(0) = disciplinare.Split("/")(0) Then
                                    ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                End If
                            Next
                        Case Else
                            Dim disciplinare As String = Session("DpiPredefinitoUtente")
                            ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 0
                            Dim selected As Boolean = False
                            For kk = 0 To ComboDisciplinari1.ddl_Disciplinari.Items.Count - 1
                                Dim val As String() = ComboDisciplinari1.ddl_Disciplinari.Items(kk).Value.Split("/")
                                If val(0) = disciplinare.Split("/")(0) Then
                                    If val.Length = 1 Then
                                        ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                        selected = True
                                        'Exit For
                                    End If
                                    If val.Length = 5 AndAlso disciplinare.Split("/").Length = 2 Then
                                        If val(4) = disciplinare.Split("/")(1) Then
                                            ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                            selected = True
                                            'Exit For
                                        End If
                                    End If
                                    If val.Length = 5 AndAlso disciplinare.Split("/").Length = 5 Then
                                        If val(4) = disciplinare.Split("/")(4) Then
                                            ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                            selected = True
                                            'Exit For
                                        End If
                                    End If
                                End If
                            Next

                            Dim x As New AgronicaCoreDpiBIZ.CaricaListControl

                            If Not selected AndAlso disciplinare.Split("/").Length > 1 Then

                                Dim ente_cod As String = ""
                                Dim fp As String = ""

                                x.Trova_Ente_Disciplinare(disciplinare, False, "", "",
                                                                   Session, objParametri_Server, objParametri_Utenti,
                                                                   0, 0, 0, 0, True, True, False,
                                                                   New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = True, .Flag_DisciplinareAttivo = True},
                                                                   ente_cod,
                                                                   fp)

                                If ente_cod <> "" Then

                                    objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(objParametriAgenda.Data, objParametriAgenda.Data)

                                    Dim disciplinare_cod As String = ""

                                    x.Trova_Disciplinare_Ente(ente_cod, fp, False, "", "",
                                                                       Session, objParametri_Server, objParametri_Utenti,
                                                                       0, 0, 0, 0, True, True, False,
                                                       New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = True, .Flag_DisciplinareAttivo = True}, disciplinare_cod, fp)

                                    objParametri_Server.ResettaFinestra()

                                    disciplinare = disciplinare_cod & "/" & fp

                                    For kk = 0 To ComboDisciplinari1.ddl_Disciplinari.Items.Count - 1
                                        Dim val As String() = ComboDisciplinari1.ddl_Disciplinari.Items(kk).Value.Split("/")
                                        If val(0) = disciplinare.Split("/")(0) Then
                                            If val.Length = 1 Then
                                                ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                                'Exit For
                                            End If
                                            If val.Length = 5 AndAlso disciplinare.Split("/").Length = 2 Then
                                                If val(4) = disciplinare.Split("/")(1) Then
                                                    ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                                    'Exit For
                                                End If
                                            End If
                                        End If
                                    Next

                                End If

                            End If

                    End Select



                End If

                objParametriAgenda.Disciplinare = ComboDisciplinari1.Valore_Combo

            End If

        Catch ex As Exception
            'ignoro
        End Try




    End Sub

    Protected Sub BTN_ComboDisciplinari1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboDisciplinari1.Click
        Cambiato_Disciplinare()
    End Sub

    Private Sub Cambiato_Disciplinare()

        If ComboDisciplinari1.Valore_Combo <> "0" Then
            Session("Disciplinare_Attivo") = True
        Else
            Session("Disciplinare_Attivo") = False
        End If
        If objParametriAgenda.Disciplinare <> ComboDisciplinari1.Valore_Combo Then
            objParametriAgenda.Disciplinare = ComboDisciplinari1.Valore_Combo
            '(09/08/2018 fede) svuotati gli impianti al cambio disciplinare
            objParametriAgenda.Impianti.Clear()
            CaricaGriglia_Impianti()
        End If

    End Sub


    Public Sub AggiornaMagazzino()

        'ComboMagazzini.Sa_Cod = objParametriAgenda.Sa_Cod
        ComboMagazzini.Piva = objParametriAgenda.Piva
        ComboMagazzini.Flag_CodCentroFabbricato = True
        ComboMagazzini.TipoMagazzino = MAGAZZINO
        Select Case objParametriAgenda.Lav_Cod
            Case LAVCOD_TRAPIANTO, LAVCOD_SEMINA, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING
                ComboMagazzini.Flag_GestioneMagazziniImpresaPadre = True

                'Se ho l'impostazione per forzare una semina che non è quella semplice allora il magazzino deve essere obbligatorio
                Dim ObjUtentiI As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim Tipo_Semina_Val As String = ObjUtentiI.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_SEMINA_TIPO, objParametri_Utenti)

                If IsNumeric(Tipo_Semina_Val) AndAlso (Tipo_Semina_Val = enum_SEMINA_TIPO.Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Vincolo OrElse
                                                       Tipo_Semina_Val = enum_SEMINA_TIPO.Semina_e_Modifica_Appezzamenti_Vincolo) Then

                    ComboMagazzini.PrimaRiga_Flag = False
                End If
        End Select

        If Not IsNothing(Session("UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO")) AndAlso
            Session("UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO") = True AndAlso
            objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Scrittura Then
            ComboMagazzini.PrimaRiga_Flag = False
        End If

        '(22/11/2018 fede) aggiunto terzista
        ComboMagazzini.Pive_Terzisti = ""
        ComboMagazzini.Flag_GestioneMagazziniTerzisti = False

        Select Case objParametriAgenda.Lav_Cod

            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_DISERBO, LAVCOD_CONCIA_SEME, LAVCOD_DISSECCAMENTO, LAVCOD_GEODISINFESTAZIONE, LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                 LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                 LAVCOD_TRAPIANTO, LAVCOD_SEMINA, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING

                Dim Pive_Terzisti As String = ""
                Dim objcontatti As New AgronicaCoreAnagrafeDAL.Contatti_R
                Dim Dt_Terzisti As DataTable
                Dt_Terzisti = objcontatti.Leggi_Contatti_ImpreseGias_ByCod_Rapporto(False,
                                                                    objParametriAgenda.Piva,
                                                                    "",
                                                                    COD_TERZISTA,
                                                                    "", "",
                                                                    objParametri_Server)
                If Dt_Terzisti IsNot Nothing AndAlso Dt_Terzisti.Rows.Count > 0 Then
                    For t = 0 To Dt_Terzisti.Rows.Count - 1
                        Pive_Terzisti &= "'" & Dt_Terzisti.Rows(t).Item("cod_contatto") & "',"
                    Next
                    If Pive_Terzisti <> "" Then
                        ComboMagazzini.Pive_Terzisti = Left(Pive_Terzisti, Pive_Terzisti.Length - 1)
                        ComboMagazzini.Flag_GestioneMagazziniTerzisti = True
                    End If
                End If

        End Select

        ComboMagazzini.CaricaComboMagazzini()

        'default (spostato qui dalla funzione DefaultUtente)
        'escludo il default per distr amm di PUA 
        'ribaltamento di ricette
        If objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Scrittura AndAlso
           Session("PrimaVolta") = True AndAlso
           Not (objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna AndAlso
                ricetta_cod <> "0" AndAlso ricetta_cod <> "") Then
            Session("PrimaVolta") = False
            If Not IsNothing(Session("UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO")) AndAlso
                Session("UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO") = True Then
                If ComboMagazzini.PrimaRiga_Flag = True Then
                    If ComboMagazzini.N_Magazzini > 1 Then
                        ComboMagazzini.Indice_Combo = 1
                        'For i = 0 To ComboMagazzini.ddl_Magazzini.Items.Count - 1
                        '    Dim valorecombo As String = ComboMagazzini.ddl_Magazzini.Items(i).Value
                        '    If Split(valorecombo, "|").Count = 3 Then
                        '        If Split(valorecombo, "|")(2) = objParametri_Server.PivaSuperUser Then
                        '            ComboMagazzini.Indice_Combo = i
                        '            Exit For
                        '        End If
                        '        If Split(valorecombo, "|")(2) <> objParametriAgenda.Piva Then
                        '            ComboMagazzini.Indice_Combo = i
                        '        End If
                        '    End If
                        'Next
                    End If
                Else
                    ComboMagazzini.Indice_Combo = 0
                    'If ComboMagazzini.N_Magazzini > 0 Then
                    '    For i = 0 To ComboMagazzini.ddl_Magazzini.Items.Count - 1
                    '        Dim valorecombo As String = ComboMagazzini.ddl_Magazzini.Items(i).Value
                    '        If Split(valorecombo, "|").Count = 3 Then
                    '            If Split(valorecombo, "|")(2) = objParametri_Server.PivaSuperUser Then
                    '                ComboMagazzini.Indice_Combo = i
                    '                Exit For
                    '            End If
                    '            If Split(valorecombo, "|")(2) <> objParametriAgenda.Piva Then
                    '                ComboMagazzini.Indice_Combo = i
                    '            End If
                    '        End If
                    '    Next
                    'End If
                End If
            Else
                ComboMagazzini.Indice_Combo = 0
            End If
            If objParametriAgenda.Fabbricato = "" OrElse objParametriAgenda.Fabbricato = "0" Then
                objParametriAgenda.Fabbricato = ComboMagazzini.Valore_Combo
            End If
        End If

        'imposto il valore
        If objParametriAgenda.Fabbricato <> "0" AndAlso Split(objParametriAgenda.Fabbricato, "|").Count = 3 Then
            ComboMagazzini.Valore_Combo = objParametriAgenda.Fabbricato
            ComboMagazzini.ddl_Magazzini.SelectedIndex = ComboMagazzini.ddl_Magazzini.Items.IndexOf(ComboMagazzini.ddl_Magazzini.Items.FindByValue(objParametriAgenda.Fabbricato))
        Else
            ComboMagazzini.Valore_Combo = objParametriAgenda.Fabbricato & "|" & objParametriAgenda.Piva
            ComboMagazzini.ddl_Magazzini.SelectedIndex = ComboMagazzini.ddl_Magazzini.Items.IndexOf(ComboMagazzini.ddl_Magazzini.Items.FindByValue(objParametriAgenda.Fabbricato & "|" & objParametriAgenda.Piva))
        End If

    End Sub

    Private Sub AggiornaCentroAziendale()
        ComboCentroAziendale.Piva = objParametriAgenda.Piva
        ComboCentroAziendale.CaricaComboCentroAziendale(objParametriAgenda.OperazioneMulticentro)


        Select Case objParametriAgenda.Tipo_Operazione
            Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                If objParametriAgenda.Sa_Cod <> 0 Then
                    'se il valore sa_cod è diverso da 0 allora è stato preselezionato dal menu il centro aziendale, quindi utilizzo quel centro come preselezionato
                    ComboCentroAziendale.Valore_Combo = objParametriAgenda.Sa_Cod
                Else
                    'se il centro è 0 allora gestisco i casi in cui sono in operazioni che supportano o no il multicentro
                    If objParametriAgenda.OperazioneMulticentro = True Then

                    Else
                        'operazione non multicentro, non ho la prima riga vuota e se sono in scrittura
                        'avrei sa_cod=0, quindi una incongruenza tra il valore combo che ha un centro e l'objparametri che ha 0,
                        'imposto quindi il valore sa_cod uguale a quello del centro selezionato
                        objParametriAgenda.Sa_Cod = ComboCentroAziendale.ddl_CentroAziendale.SelectedValue
                    End If


                    'se ho un solo centro lo imposto
                    Try
                        If objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Scrittura AndAlso ComboCentroAziendale.ddl_CentroAziendale.Items.Count = 2 Then
                            ComboCentroAziendale.ddl_CentroAziendale.SelectedIndex = 1
                            objParametriAgenda.Sa_Cod = ComboCentroAziendale.ddl_CentroAziendale.SelectedValue
                        End If
                    Catch ex As Exception

                    End Try

                End If


            Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica
                ComboCentroAziendale.Valore_Combo = objParametriAgenda.Sa_Cod
            Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura
                ComboCentroAziendale.Valore_Combo = objParametriAgenda.Sa_Cod
        End Select


    End Sub


    Private Sub AggiornaOperazioni()

        Select Case objParametriAgenda.Tipo_Operazione
            Case enum_TipoOperazioneDB.Modifica, enum_TipoOperazioneDB.Lettura
                PannelloOperazioni.Style.Add("display", "none")
        End Select

        Dim filtro As String = ""
        If objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.RicettaBrogliaccio OrElse objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta Then
            If Not (objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Trattamenti) Then
                filtro = " AND Operazioni.lav_cod IN (" & StrLavCodRicettabili & ")"
            End If
        End If

        'Escludo le Operazioni non getsite sulle pagine aspx
        filtro += " AND " + CostantiPersonalizzate.STR_OP_NON_GESTITE_BS

        ComboOperazione.CaricaComboLavorazioni(True, filtro)

        'imposto il valore
        ComboOperazione.Valore_Combo = objParametriAgenda.Lav_Cod

        Select Case objParametriAgenda.Lav_Cod
            Case LAVCOD_FERTIRRIGAZIONE,
                LAVCOD_CONCIMAZIONE_FOGLIARE,
                LAVCOD_DISTRIBUZIONE_CONCIME,
                LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                LAVCOD_SARCHIATURA_CONCIMAZIONE,
                LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                objParametriAgenda.Elem_Cod = FERTILIZZANTI
        End Select
    End Sub

#End Region

#Region "Inizializzazioni"

    Private Sub InizializzaData(Piva As String)
        Dim Str As New StringBuilder
        Str.AppendLine("$(document).ready(function () {")


        Str.AppendLine(" console.log('datepicker impostato server side .. :)');")

        'Str.AppendLine("    $('#" & txt_DataOperazione.ClientID & "').datepicker({ ")
        Dim dataMin As Date = AGRODATAINIZIO
        Dim dataMax As Date = AGRODATAFINE

        Dim SportelloAperto As Boolean = True
        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura OrElse
           objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

            Dim Servizio_cod As Integer
            Select Case objParametriAgenda.Lav_Cod
                Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO
                    If objParametriAgenda.TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua Then
                        Servizio_cod = enum_Servizi.PUA
                    Else
                        Servizio_cod = enum_Servizi.Quaderno_Campagna_Caa
                    End If
                Case Else
                    Servizio_cod = enum_Servizi.Quaderno_Campagna_Caa
            End Select

            Dim objPraticheBIZ As New AgronicaCoreProfilazioneBIZ.Pratiche_R

            'Azienda in Verifica
            Dim AziendaInVerifica As Boolean = False
            Dim dataMinxVerifica = AGRODATAINIZIO
            Dim dataMaxxVerifica = AGRODATAFINE
            objPraticheBIZ.Limitazione_Data_Per_VerificaInCorso(Piva,
                                                       Servizio_cod,
                                                       objParametriAgenda.Data,
                                                       AziendaInVerifica,
                                                       dataMinxVerifica,
                                                       dataMaxxVerifica,
                                                       objParametri_Server,
                                                       objParametri_Utenti)

            'Controllo Sportello
            objPraticheBIZ.Data_Sportello_Da_Servizio(Piva,
                                                       Servizio_cod,
                                                       objParametriAgenda.Data,
                                                       SportelloAperto,
                                                       dataMin,
                                                       dataMax,
                                                       objParametri_Server,
                                                       objParametri_Utenti)

            If Servizio_cod = enum_Servizi.Quaderno_Campagna_Caa Then

                objPraticheBIZ.VerificaInCorso_ChiamataSecondaria_SeNessunCambiamento(objParametriAgenda.Piva,
                                                                                        enum_Servizi.QuadernoCampagnaBio,
                                                                                        objParametriAgenda.Data,
                                                                                        False,
                                                                                        AGRODATAINIZIO,
                                                                                        AGRODATAFINE,
                                                                                        AziendaInVerifica,
                                                                                        dataMinxVerifica,
                                                                                        dataMaxxVerifica,
                                                                                        objParametri_Server,
                                                                                        objParametri_Utenti)

                objPraticheBIZ.Sportello_ChiamataSecondaria_SeNessunCambiamento(objParametriAgenda.Piva,
                                                                                        enum_Servizi.QuadernoCampagnaBio,
                                                                                        objParametriAgenda.Data,
                                                                                        True,
                                                                                        AGRODATAINIZIO,
                                                                                        AGRODATAFINE,
                                                                                        SportelloAperto,
                                                                                        dataMin,
                                                                                        dataMax,
                                                                                        objParametri_Server,
                                                                                        objParametri_Utenti)
            End If

            If dataMinxVerifica > dataMin Then
                dataMin = dataMinxVerifica
            End If

            If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then

                If objParametriAgenda.Data < dataMin Then
                    objParametriAgenda.Data = dataMin
                    txt_DataOperazione.Text = dataMin.ToShortDateString
                End If

                If objParametriAgenda.Data > dataMax Then
                    objParametriAgenda.Data = dataMax
                    txt_DataOperazione.Text = dataMax.ToShortDateString
                End If

            End If

            If Not SportelloAperto OrElse (objParametriAgenda.Data < dataMin OrElse objParametriAgenda.Data > dataMax) AndAlso
                                          AziendaInVerifica = False Then

                UpdatePanelSalvataggio.Visible = False
                If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
                    Messaggi.AgroMsgBox(" Non è possibile inserire l'operazione. Sportello chiuso. ", Page, , UpdatePanelPerScript)
                End If

            ElseIf AziendaInVerifica Then

                UpdatePanelSalvataggio.Visible = False
                If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
                    Messaggi.AgroMsgBox(" Non è possibile inserire l'operazione. Azienda in verifica. ", Page, , UpdatePanelPerScript)
                End If

            Else

                UpdatePanelSalvataggio.Visible = True
            End If

        End If

        'txt_DataOperazione.Attributes.Add("min", dataMin.ToShortDateString)
        'txt_DataOperazione.Attributes.Add("max", dataMax.ToShortDateString)

        'txt_DataOperazione.Attributes.Remove("type")

        'txt_DataOperazione.Attributes.Add("type", "date")

        If AttivaOrarioInDataMovimento = False Then
            Str.AppendLine("    $('#" & txt_DataOperazione.ClientID & "').kendoDatePicker({ " & vbCrLf &
                           " min: new Date(" & CStr(dataMin.Year) & ", " & CStr(dataMin.Month - 1) & ", " & CStr(dataMin.Day) & ")," & vbCrLf &
                           " max: new Date(" & CStr(dataMax.Year) & ", " & CStr(dataMax.Month - 1) & ", " & CStr(dataMax.Day) & ")" & vbCrLf &
                           " }); ")
            'If dataMin <> AGRODATAINIZIO Or dataMax <> AGRODATAFINE Then
            '    Str.AppendLine(" $('#" & txt_DataOperazione.ClientID & "').kendoDateInput({ " & vbCrLf &
            '               " min: new Date(" & CStr(dataMin.Year) & ", " & CStr(dataMin.Month - 1) & ", " & CStr(dataMin.Day) & ")," & vbCrLf &
            '               " max: new Date(" & CStr(dataMax.Year) & ", " & CStr(dataMax.Month - 1) & ", " & CStr(dataMax.Day) & ")" & vbCrLf &
            '               " }); ")
            'End If

        Else
            Str.AppendLine("    $('#" & txt_DataOperazione.ClientID & "').kendoDateTimePicker({ " & vbCrLf &
                           " min: new Date(" & CStr(dataMin.Year) & ", " & CStr(dataMin.Month - 1) & ", " & CStr(dataMin.Day) & ", " & CStr(dataMin.Hour) & ", " & CStr(dataMin.Minute) & ", 59, 0)," & vbCrLf &
                           " max: new Date(" & CStr(dataMax.Year) & ", " & CStr(dataMax.Month - 1) & ", " & CStr(dataMax.Day) & ", " & CStr(dataMax.Hour) & ", " & CStr(dataMax.Minute) & ", 59, 0)" & vbCrLf &
                           " }); ")
            Str.AppendLine(" $('#" & txt_DataOperazione.ClientID & "').kendoDateInput(); ")
        End If

        'Str.AppendLine("    format: 'dd/mm/yyyy',")
        'Str.AppendLine("    disabled: false,")
        'Str.AppendLine("    changeMonth: true,")
        'Str.AppendLine("    changeYear: true,")
        'Str.AppendLine("    autoclose: true")
        'Str.AppendLine("});")

        ''Str.AppendLine("$.datepicker.regional['it'];")

        Str.AppendLine("$('#" & txt_DataOperazione.ClientID & "').change(function () {")
        Str.AppendLine("    $('#" & BTN_ChangeData.ClientID & "').click();")
        Str.AppendLine("    });")

        Str.AppendLine("});")
        ScriptManager.RegisterStartupScript(UpdatePanelData, UpdatePanelData.GetType(),
                                         String.Format("jQuery_{0}", txt_DataOperazione.ClientID), Str.ToString, True)


        ' Str = New StringBuilder
        'Str.AppendLine("$(document).ready(function () {")

        'Dim contenitore As String = "window"
        'Dim scarto As String = "-30"
        'Dim scartoW As String = "-60"

        ''Str.AppendLine("$('#dialog').dialog({")
        ''Str.AppendLine("    autoOpen: false,")
        ''Str.AppendLine("    height: 600,")
        ''Str.AppendLine("    width: 850,")
        ''Str.AppendLine("    modal: true")
        ''Str.AppendLine("});")

        ''Str.AppendLine("$('select').selectpicker('refresh');")

        'Str.AppendLine("});")
        'ScriptManager.RegisterStartupScript(UpdatePanelPerScript, UpdatePanelPerScript.GetType(),
        '                                 String.Format("jQuery_{0}", UpdatePanelPerScript.ClientID), Str.ToString, True)

    End Sub





    Private Sub InizializzaVarie()

        ''inizializzazione pulsanti

        Dim stb As New StringBuilder

        stb.AppendLine(" console.log(""inizializzazionePulsanteSalvataggio()""); inizializzazionePulsanteSalvataggio();  ")

        ScriptManager.RegisterStartupScript(updateToolBar, updateToolBar.GetType(),
                                         String.Format("jQuery_{0}", updateToolBar.ClientID), stb.ToString, True)


    End Sub

    Private Sub ImpostaPannellibyOperazione()

        'pannelloDisciplinari.Visible = False
        'pannelloFiltriRicerca.Visible = False

        Select Case objParametriAgenda.Lav_Cod

            Case LAVCOD_FERTIRRIGAZIONE,
                  LAVCOD_CONCIMAZIONE_FOGLIARE,
                  LAVCOD_DISTRIBUZIONE_CONCIME,
                  LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                  LAVCOD_SARCHIATURA_CONCIMAZIONE,
                  LAVCOD_TRATTAMENTO_ANTIBUTTERATURA



            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE,
                LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO

                'pannelloDisciplinari.Visible = True
                'pannelloFiltriRicerca.Visible = True

        End Select

    End Sub

#End Region

#Region "Impianti"

    Public Sub CaricaGriglia_Impianti()


        Select Case objParametriAgenda.Lav_Cod

            Case LAVCOD_TRATTAMENTO_POST_RACCOLTA

            Case Else

                'objParametriAgenda.Leggi()
                Dim Dt As New DataTable

                'icona per le info aggiuntive
                Dim DtKeys(14) As String
                CaricaGriglia_Impianti_getDT(Dt, DtKeys)

                CaricaGriglia_Impianti_xJSON(Dt)

                '----------------------
                '--- 'SINGOLO CENTRO
                '----------------------
                GridView_Impianti.Columns(1).Visible = False

                'If objParametriAgenda.Sa_Cod <> "0" Then
                '    GridView_Impianti.Columns(2).Visible = False
                'Else
                '    '----------------------
                '    '--- 'PIU CENTRI
                '    '----------------------
                '    GridView_Impianti.Columns(2).Visible = True
                'End If

                '----- Associo il DataTable con la DataGrid

                'prima di ricaricare il DT controllo che non sia uguale al precedente
                'Dim sostituisci As Boolean = True

                'If Not IsNothing(ViewState("DT_Impianti")) AndAlso Dt.Rows.Count = CType(ViewState("DT_Impianti"), DataTable).Rows.Count Then
                '    Dim DT_app1 As DataTable
                '    DT_app1 = CType(ViewState("DT_Impianti"), DataTable).Copy
                '    Dim dv1 As New DataView(Dt, _
                '                           "", _
                '                           " Piva Desc , Sa_Cod Desc, Appezza Desc, ID_Reg Desc", DataViewRowState.CurrentRows)
                '    Dim dv2 As New DataView(CType(ViewState("DT_Impianti"), DataTable), _
                '                           "", _
                '                           " Piva Desc , Sa_Cod Desc, Appezza Desc, ID_Reg Desc", DataViewRowState.CurrentRows)

                '    Dim dt1, dt2 As DataTable
                '    dt1 = dv1.ToTable
                '    dt2 = dv2.ToTable
                '    sostituisci = False
                '    For i = 0 To dt1.Rows.Count - 1
                '        If dt1.Rows(i).Item("Piva") <> dt2.Rows(i).Item("Piva") Or _
                '            dt1.Rows(i).Item("sa_cod") <> dt2.Rows(i).Item("sa_cod") Or _
                '            dt1.Rows(i).Item("Appezza") <> dt2.Rows(i).Item("Appezza") Or _
                '            dt1.Rows(i).Item("ID_Reg") <> dt2.Rows(i).Item("ID_Reg") Then
                '            sostituisci = True
                '            Exit For
                '        End If
                '    Next
                'End If

                GridView_Impianti.DataSource = Dt
                GridView_Impianti.DataKeyNames = DtKeys
                GridViewToPhone()
                ViewState("DT_Impianti") = Dt

                SettaImpostazioneUtente_UDM()

                Dim jss As New JavaScriptSerializer
                hdKendo_Impianti_Selezione.Value = jss.Serialize(objParametriAgenda.Impianti)


        End Select



    End Sub

    Private Sub CaricaGriglia_Impianti_getDT(ByRef Dt As DataTable, ByRef DtKeys() As String)

        Dim j As Integer = 0
        Dim ColturaProtetta As String = ""

        Dim SaCod As Integer = CInt(objParametriAgenda.Sa_Cod)

        '----- Recupero l'elenco degli impianti

        ''------------------------------------------------------------------------------
        Dim Array() As String = Split(objParametriAgenda.Disciplinare, "/")
        'Dim Dpi_Cod As Integer = Array(0)
        'Dim IdRcdpi As Integer = Array(1)
        Dim Grfi_Cod As Integer = 0
        Dim Flag_Protetto As Integer = 0
        Dim Flag_Disciplinare As Boolean
        Dim Flag_PubblicoPrivato As Integer = 0




        If Not IsNothing(Array) AndAlso Array.Length > 1 Then
            If Array.Length > 2 Then
                Grfi_Cod = Array(2)
            End If
            If Array.Length > 3 Then
                Flag_Protetto = Array(3)
            End If
            If Array.Length > 4 Then
                Flag_PubblicoPrivato = Array(4)
            End If
            Flag_Disciplinare = True
        Else
            Grfi_Cod = 0
            Flag_Protetto = 0
            Flag_PubblicoPrivato = 0
            Flag_Disciplinare = False
        End If

        'metto Flag_Protetto = -1 x non filtrarlo nel caricamento degli impianti
        Select Case objParametriAgenda.Lav_Cod
            Case LAVCOD_FERTIRRIGAZIONE,
                  LAVCOD_CONCIMAZIONE_FOGLIARE,
                  LAVCOD_DISTRIBUZIONE_CONCIME,
                  LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                  LAVCOD_SARCHIATURA_CONCIMAZIONE,
                  LAVCOD_TRATTAMENTO_ANTIBUTTERATURA
                Flag_Protetto = -1
        End Select

        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

        'filtro in scrittura se ho impostato il filtro dal menu agenda
        Dim filtro As String = "|"
        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
            If Not IsNothing(Session("Filtro")) AndAlso Session("Filtro") <> "" Then
                'No un filtro

                filtro = Session("Filtro")

            End If
        End If

        Dim id_cod As Integer = 0
        If Not IsNothing(ComboSpecie) AndAlso
            DETTAGLITERRENONUDO AndAlso
            ComboSpecie.Valore_Combo.Split("/").Length = 2 AndAlso
            ComboSpecie.Valore_Combo.Split("/")(0) = "0" Then
            'terreno nuudo con indicata la destinazione
            id_cod = CInt(ComboSpecie.Valore_Combo.Split("/")(1))
        End If

        Dim specie As String = objParametriAgenda.Veg_Cod.Split("/")(0)
        If Not IsNothing(ComboSpecie) AndAlso CONSIDERATERRENONUDO Then
            specie = ComboSpecie.Valore_Combo.Split("/")(0)
        Else
            If objParametriAgenda.Veg_Cod.Split("/")(0) = "0" Then
                specie = "-999"
            Else
                specie = objParametriAgenda.Veg_Cod.Split("/")(0)
            End If
        End If
        '----------------------------------
        Dim leggiAncheBloccati As Boolean = False
        If Not IsNothing(Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI")) Then
            If Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI") = True Then
                leggiAncheBloccati = True
            End If
        End If

        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura Then
            leggiAncheBloccati = True
        End If

        Dim CampoCod As Integer = 0

        Dim FiltroAggiuntivo As String = ""

        Dim filtroFinale As String = filtro.Split("|")(1)
        If FiltroAggiuntivo <> "" Then
            FiltroAggiuntivo = "(" & FiltroAggiuntivo & ")"
            If filtroFinale <> "" Then
                filtroFinale = filtroFinale & " AND " & FiltroAggiuntivo
            Else
                filtroFinale = FiltroAggiuntivo
            End If
        End If

        '(12/11/2018 fede) aggiunta indicazione fase fenologica corrente
        Dim objParametriUscitaFasiNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
        Dim objParametriUscitaFasiOld As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output

        Dim FF_Cod_Fioritura_Old As Integer = 0
        Dim FF_Cod_Fioritura_New As Integer = 0

        Dim strFFCod As String = ""

        If IsNumeric(specie) AndAlso CInt(specie) > 0 Then

            Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
            objParametriIngresso.Veg_Cod = CInt(specie)
            objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod

            Dim Leggi_impostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE, objParametri_Utenti, 2)
            If imp = "1" Then
                objParametriIngresso.Personalizzate = True
            End If

            Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS

            objParametriUscitaFasiNew = objFasi_WS.FasiFenologiche(objParametriIngresso)
            objParametriUscitaFasiOld = objFasi_WS.FasiFenologiche_OLD(objParametriIngresso)

            'fasi bbch x fioritura
            For f = 0 To objParametriUscitaFasiNew.ListaFasiFenologiche.Count - 1
                If objParametriUscitaFasiNew.ListaFasiFenologiche(f).Fioritura Then
                    FF_Cod_Fioritura_New = objParametriUscitaFasiNew.ListaFasiFenologiche(f).Cod_SS
                    FF_Cod_Fioritura_Old = objParametriUscitaFasiNew.ListaFasiFenologiche(f).FF_Cod
                    Exit For
                End If
            Next

        End If

        If FF_Cod_Fioritura_New <> 0 AndAlso FF_Cod_Fioritura_Old <> 0 Then
            strFFCod = " (" & FF_Cod_Fioritura_New & "," & FF_Cod_Fioritura_Old & ")"
        ElseIf FF_Cod_Fioritura_New <> 0 AndAlso FF_Cod_Fioritura_Old = 0 Then
            strFFCod = " (" & FF_Cod_Fioritura_New & ")"
        ElseIf FF_Cod_Fioritura_New = 0 AndAlso FF_Cod_Fioritura_Old <> 0 Then
            strFFCod = " (" & FF_Cod_Fioritura_Old & ")"
        Else
            strFFCod = " (-1)"
        End If

        'lettura impianti
        Dt = objImpianti.Leggi_Impianti_xAgenda3(Flag_Disciplinare,
                                        objParametriAgenda.Piva,
                                        SaCod,
                                        CampoCod,
                                        specie,
                                        objParametriAgenda.Cul_Cod,
                                        objParametriAgenda.Data,
                                        Grfi_Cod,
                                        Flag_Protetto,
                                        True,
                                        id_cod,
                                        filtroFinale, "  App_Nome, Cul_Des, Progetto ",
                                        objParametri_Server, leggiAncheBloccati,
                                        visualizza_Kpin_BlockName,
                                        visualizza_codici_imp_app_prj,
                                        strFFCod)
        '----------------------------------
        'lettura particelle impianti
        'Getsione precedente per la lettura della classe tessitura
        'Dim includiClasseTessitura As Boolean = TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua AndAlso LAVCOD_DISTRIBUZIONE_AMMENDANTI

        Dim includiClasseTessitura As Boolean = objParametriAgenda.Lav_Cod = LAVCOD_DISTRIBUZIONE_AMMENDANTI
        Dim DtParticelle As DataTable = objImpianti.Leggi_ParticelleImpianti_xAgenda2(Flag_Disciplinare,
                                                                                     objParametriAgenda.Piva,
                                                                                     SaCod,
                                                                                     specie,
                                                                                     objParametriAgenda.Cul_Cod,
                                                                                     objParametriAgenda.Data,
                                                                                     Grfi_Cod,
                                                                                     Flag_Protetto,
                                                                                     id_cod,
                                                                                     filtro.Split("|")(1), " App_Nome, Cul_Des, Progetto ",
                                                                                     objParametri_Server, leggiAncheBloccati, includiClasseTessitura:=includiClasseTessitura)

        '----------------------------------
        'lettura zone vulnerabili
        Dim objPV As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
        Dim DtPV As DataTable = objPV.Leggi(-17,
                                            "", "", "", 0, 0, "",
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "", objParametri_Server)

        '----------------------------------------
        'lettura fasce (la metto in un try catch in caso non esista la tabella nel DB PianoConcimazione_Pua e di conseguenza la vista)
        Dim DtPVF As DataTable
        Try
            Dim objPVF As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R
            DtPVF = objPVF.Leggi("", "", "", 0, 0, "", 0,
                                 " Fascia_Cod <>0 ",
                                 "", objParametri_Server)
        Catch ex As Exception

        End Try

        '----------------------------------------
        'lettura delle analisi
        Dim classTess_output As New AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_output
        'TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua AndAlso LAVCOD_DISTRIBUZIONE_AMMENDANTI
        If objParametriAgenda.Lav_Cod = LAVCOD_DISTRIBUZIONE_AMMENDANTI Then

            Dim pcws As New AgronicaCoreWebService.PianoConcimazione_WS
            Dim classTess_input As New AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_input
            classTess_input.Regolamento_Cod = objParametriAgenda.Disciplinare.Split("/")(0)
            classTess_output = pcws.ClassiTessitura(classTess_input)
        End If




        'Valorizzo le celle del vettore
        DtKeys(0) = "Piva"
        DtKeys(1) = "Sa_Cod"
        DtKeys(2) = "Appezza"
        DtKeys(3) = "Id_Reg"
        DtKeys(4) = "Progetto_Cod"
        DtKeys(5) = "Sup_Imp"
        DtKeys(6) = "Validita_Inizio_Distinta"
        DtKeys(7) = "Validita_Fine_Distinta"
        DtKeys(8) = "Cul_Des"
        DtKeys(9) = "Data_Raccolta"
        DtKeys(10) = "Data_Raccolta_Prevista"
        DtKeys(11) = "App_Nome"
        DtKeys(12) = "Data_Fioritura"
        DtKeys(13) = "Data_Fioritura_Prevista"
        DtKeys(14) = "Codici_Anagrafe_Des"

        'aggiungo la colonna catasto
        Dt.Columns.Add(New DataColumn("catasto", GetType(String)))
        Dt.Columns.Add(New DataColumn("kendoKey", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Imp_help", GetType(Double)))
        Dt.Columns.Add(New DataColumn("Descrizione_Unica", GetType(String)))

        '(31/07/2018 fede) aggiunte colonne x dati gis
        Dt.Columns.Add(New DataColumn("GisWkt", GetType(String)))
        Dt.Columns.Add(New DataColumn("GisWktGps", GetType(String)))
        Dt.Columns.Add(New DataColumn("GisWktSistemaRiferimento", GetType(String)))
        Dt.Columns.Add(New DataColumn("GisTipoEntita_cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("GisLayerCod", GetType(Integer)))

        '(12/11/2018 fede) aggiunta indicazione fase fenologica corrente
        Dt.Columns.Add(New DataColumn("Fase_Corrente", GetType(String)))

        Dt.Columns.Add(New DataColumn("Sup_Riduzione_BufferZone", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Perc_Riduzione_Deriva", GetType(Decimal)))

        '(12/06/2019 Marco G) aggiunta classi di Tessitura
        Dt.Columns.Add(New DataColumn("Id_ClasseTessitura", GetType(String)))
        Dt.Columns.Add(New DataColumn("Str_ClasseTessitura", GetType(String)))

        Dt.Columns.Add(New DataColumn("N_Max", GetType(String)))
        Dt.Columns.Add(New DataColumn("P_Max", GetType(String)))
        Dt.Columns.Add(New DataColumn("K_Max", GetType(String)))
        Dt.Columns.Add(New DataColumn("Mg_Max", GetType(String)))

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then

            For j = 0 To Dt.Rows.Count - 1

                Dim kk As String =
                    Dt.Rows(j).Item("Piva") & "-" &
                    Dt.Rows(j).Item("Sa_Cod").ToString & "-" &
                    Dt.Rows(j).Item("Appezza").ToString & "-" &
                    Dt.Rows(j).Item("id_reg").ToString


                Dt.Rows(j).Item("kendoKey") = kk
                Dt.Rows(j).Item("Sup_Imp_help") = 0



                '--------------------------
                '02/09/2014 esclusa dalla copertura cop_cod=1 (protezione grandine)
                Select Case Dt.Rows(j).Item("Cop_Cod")
                    Case 0, 1, 3, 4, 5, 6 'nessuna copertura
                        ColturaProtetta = "No"
                        Flag_Protetto = 0
                    Case Else
                        ColturaProtetta = "Si"
                        Flag_Protetto = 1
                End Select

                Dt.Rows(j).Item("id_rcdpi") = 0

                '--------------------------
                'Formatto le date
                If IsDate(Dt.Rows(j).Item("Validita_Inizio")) Then
                    If CDate(Dt.Rows(j).Item("Validita_Inizio")) = AGRODATAINIZIO OrElse CDate(Dt.Rows(j).Item("Validita_Inizio")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Validita_Inizio") = ""
                    End If
                End If
                If IsDate(Dt.Rows(j).Item("Validita_Fine")) Then
                    If CDate(Dt.Rows(j).Item("Validita_Fine")) = AGRODATAINIZIO OrElse CDate(Dt.Rows(j).Item("Validita_Fine")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Validita_Fine") = ""
                    End If
                End If
                If IsDate(Dt.Rows(j).Item("Data_Semina")) Then
                    If CDate(Dt.Rows(j).Item("Data_Semina")) = AGRODATAINIZIO OrElse CDate(Dt.Rows(j).Item("Data_Semina")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Data_Semina") = ""
                    End If
                End If
                If IsDate(Dt.Rows(j).Item("Data_Semina_Prevista")) Then
                    If CDate(Dt.Rows(j).Item("Data_Semina_Prevista")) = AGRODATAINIZIO OrElse CDate(Dt.Rows(j).Item("Data_Semina_Prevista")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Data_Semina_Prevista") = ""
                    End If
                End If
                If IsDate(Dt.Rows(j).Item("Data_Raccolta")) Then
                    If CDate(Dt.Rows(j).Item("Data_Raccolta")) = AGRODATAINIZIO OrElse CDate(Dt.Rows(j).Item("Data_Raccolta")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Data_Raccolta") = ""
                    End If
                End If
                If IsDate(Dt.Rows(j).Item("Data_Raccolta_Prevista")) Then
                    If CDate(Dt.Rows(j).Item("Data_Raccolta_Prevista")) = AGRODATAINIZIO OrElse CDate(Dt.Rows(j).Item("Data_Raccolta_Prevista")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Data_Raccolta_Prevista") = ""
                    End If
                End If
                If IsDate(Dt.Rows(j).Item("Data_Fioritura")) Then
                    If CDate(Dt.Rows(j).Item("Data_Fioritura")) = AGRODATAINIZIO OrElse CDate(Dt.Rows(j).Item("Data_Fioritura")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Data_Fioritura") = ""
                    End If
                End If
                If IsDate(Dt.Rows(j).Item("Data_Fioritura_Prevista")) Then
                    If CDate(Dt.Rows(j).Item("Data_Fioritura_Prevista")) = AGRODATAINIZIO OrElse CDate(Dt.Rows(j).Item("Data_Fioritura_Prevista")) = AGRODATAFINE Then
                        Dt.Rows(j).Item("Data_Fioritura_Prevista") = ""
                    End If
                End If
                '========= PermessoDPI fine

                '-----------------------------------------
                'aggiunta indicazione catasto (19/09/2012)
                Dim strCatasto As String = ""
                Dim strId_ClasseTessitura As String = ""
                Dim strStr_ClasseTessitura As String = ""
                Dim DrParticelle() As DataRow
                Dim DrPV() As DataRow
                Dim DrPVFA() As DataRow
                Dim DrPVFB() As DataRow
                Dim p As Integer

                Dim strParticella As String
                Dim strVulnerabile As String

                If DtParticelle IsNot Nothing AndAlso DtParticelle.Rows.Count > 0 Then
                    DrParticelle = DtParticelle.Select("piva='" & Dt.Rows(j).Item("piva").ToString & "' and sa_cod=" & Dt.Rows(j).Item("sa_cod").ToString & " and appezza=" & Dt.Rows(j).Item("appezza").ToString & " and id_reg=" & Dt.Rows(j).Item("id_reg").ToString)
                    If DrParticelle IsNot Nothing AndAlso DrParticelle.Length > 0 Then
                        For p = 0 To DrParticelle.Length - 1
                            strParticella = DrParticelle(p).Item("prov") & "_" & DrParticelle(p).Item("com") & "_" & DrParticelle(p).Item("sezione") & "_" & DrParticelle(p).Item("foglio") & "_" & DrParticelle(p).Item("numero") & "_" & DrParticelle(p).Item("subalterno")
                            strVulnerabile = ""
                            If DtPV IsNot Nothing AndAlso DtPV.Rows.Count > 0 Then
                                DrPV = DtPV.Select("prov='" & DrParticelle(p).Item("prov").ToString & "' AND Com='" & DrParticelle(p).Item("com").ToString & "' AND Sezione='" & DrParticelle(p).Item("sezione").ToString & "' AND Foglio=" & DrParticelle(p).Item("foglio").ToString & " AND Numero=" & DrParticelle(p).Item("numero").ToString & " AND subalterno='" & DrParticelle(p).Item("subalterno").ToString & "'")
                                If DrPV IsNot Nothing AndAlso DrPV.Length > 0 Then
                                    strVulnerabile = " <b>(V)</b>"
                                End If
                            End If
                            If DtPVF IsNot Nothing AndAlso DtPVF.Rows.Count > 0 Then
                                DrPVFA = DtPVF.Select("prov='" & DrParticelle(p).Item("prov").ToString & "' AND Com='" & DrParticelle(p).Item("com").ToString & "' AND Foglio=" & DrParticelle(p).Item("foglio").ToString & " AND Fascia_Cod=1")
                                If DrPVFA IsNot Nothing AndAlso DrPVFA.Length > 0 Then
                                    strVulnerabile = " <b>(V - Fascia A)</b>"
                                End If
                                DrPVFB = DtPVF.Select("prov='" & DrParticelle(p).Item("prov").ToString & "' AND Com='" & DrParticelle(p).Item("com").ToString & "' AND Foglio=" & DrParticelle(p).Item("foglio").ToString & " AND Fascia_Cod=2")
                                If DrPVFB IsNot Nothing AndAlso DrPVFB.Length > 0 Then
                                    strVulnerabile = " <b>(V - Fascia B)</b>"
                                End If
                            End If
                            strCatasto &= strParticella & strVulnerabile & "<br>"
                        Next

                        Dim arrId_ClassiTessitura As Integer() = (From drP As DataRow In DrParticelle Where CInt(drP.Item("Id_ClasseTessitura")) > 0 Select CInt(drP.Item("Id_ClasseTessitura"))).Distinct().ToArray()
                        Dim listaStrClassiTessitura As New List(Of String)
                        For Each el As Integer In arrId_ClassiTessitura
                            Dim strClassiTessitura As String = (From ct As AgronicaCorePianoConcimazioneBIZ.PUA_ClasseTessitura In classTess_output.ListaClassiTessitura Where el = ct.Tessitura_Cod Select CStr(ct.Tessitura_Des)).ToArray().FirstOrDefault()
                            If Not IsNothing(strClassiTessitura) Then
                                listaStrClassiTessitura.Add(strClassiTessitura.Trim())
                            End If
                        Next
                        strId_ClasseTessitura = String.Join(",", arrId_ClassiTessitura)
                        strStr_ClasseTessitura = String.Join(", ", listaStrClassiTessitura)
                    End If
                End If
                If strCatasto <> "" Then
                    strCatasto = Left(strCatasto, strCatasto.Length - 4)
                End If
                Dt.Rows(j).Item("Id_ClasseTessitura") = strId_ClasseTessitura
                Dt.Rows(j).Item("Str_ClasseTessitura") = strStr_ClasseTessitura

                Dt.Rows(j).Item("catasto") = strCatasto

                Dt.Rows(j).Item("N_Max") = Dt.Rows(j).Item("N_Massimo")
                Dt.Rows(j).Item("P_Max") = Dt.Rows(j).Item("P_Massimo")
                Dt.Rows(j).Item("K_Max") = Dt.Rows(j).Item("K_Massimo")
                Dt.Rows(j).Item("Mg_Max") = Dt.Rows(j).Item("Mg_Massimo")

                'NPK MarcoG 13/06/2019
                Dim strN As String = "<b>Massimo: </b><span class='qdc_n_masssimo'>" & CStr(Dt.Rows(j).Item("N_Max")) & "</span><br>" '&
                '"<b>Distribuito: </b><span class='qdc_n_distribuito'>" & CStr(Dt.Rows(j).Item("N_Distribuito")) & "</span><br>" &
                '"<b>Residuo: </b><span class='qdc_n_residuo'>" & CStr(Dt.Rows(j).Item("N_Residuo")) & "</span>"

                Dim strP As String = "<b>Massimo: </b><span class='qdc_p_masssimo'>" & CStr(Dt.Rows(j).Item("P_Max")) & "</span><br>" '&
                '"<b>Distribuito: </b><span class='qdc_p_distribuito'>" & CStr(Dt.Rows(j).Item("P_Distribuito")) & "</span><br>" &
                '"<b>Residuo: </b><span class='qdc_p_residuo'>" & CStr(Dt.Rows(j).Item("P_Residuo")) & "</span>"

                Dim strK As String = "<b>Massimo: </b><span class='qdc_k_masssimo'>" & CStr(Dt.Rows(j).Item("K_Max")) & "</span><br>" '&
                '"<b>Distribuito: </b><span class='qdc_k_distribuito'>" & CStr(Dt.Rows(j).Item("K_Distribuito")) & "</span><br>" &
                '"<b>Residuo: </b><span class='qdc_k_residuo'>" & CStr(Dt.Rows(j).Item("K_Residuo")) & "</span>"

                Dim strMg As String = "<b>Massimo: </b><span class='qdc_mg_masssimo'>" & CStr(Dt.Rows(j).Item("Mg_Max")) & "</span><br>" '&
                '"<b>Distribuito: </b><span class='qdc_mg_distribuito'>" & CStr(Dt.Rows(j).Item("Mg_Distribuito")) & "</span><br>" &
                '"<b>Residuo: </b><span class='qdc_mg_residuo'>" & CStr(Dt.Rows(j).Item("Mg_Residuo")) & "</span>"




                Dt.Rows(j).Item("Descrizione_Unica") = Dt.Rows(j).Item("App_Nome") &
                                                        If(String.IsNullOrEmpty(Dt.Rows(j).Item("RifNumerico")), "", " (" & Dt.Rows(j).Item("RifNumerico") & ")") &
                                                        " - <i>Varietà: " & Dt.Rows(j).Item("Cul_Des") & "</i> - Sup: " & Dt.Rows(j).Item("Sup_Imp") & " Ha"


                '----------------------------------------------------------------
                'DA OTTIMIZZARE se la si vuole ripristinare
                '----------------------------------------------------------------

                Dim objFert As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                Dim objDettRic As New AgronicaCoreContabDAL.Ricette_Dettagli_R
                Dim N_Distribuito As Decimal = 0
                Dim P_Distribuito As Decimal = 0
                Dim K_Distribuito As Decimal = 0
                Dim Mg_Distribuito As Decimal = 0
                Dim Cu_Distribuito As Decimal = 0
                Dim N_Distribuito_Ricetta As Decimal = 0
                Dim P_Distribuito_Ricetta As Decimal = 0
                Dim K_Distribuito_Ricetta As Decimal = 0
                Dim Mg_Distribuito_Ricetta As Decimal = 0
                Dim Cu_Distribuito_Ricetta As Decimal = 0
                Dim N_Residuo As Decimal = 0
                Dim P_Residuo As Decimal = 0
                Dim K_Residuo As Decimal = 0
                Dim Mg_Residuo As Decimal = 0

                Select Case objParametriAgenda.Lav_Cod

                    Case LAVCOD_FERTIRRIGAZIONE,
                          LAVCOD_CONCIMAZIONE_FOGLIARE,
                          LAVCOD_DISTRIBUZIONE_CONCIME,
                          LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                          LAVCOD_SARCHIATURA_CONCIMAZIONE,
                          LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                        'se almeno un valore massimo è stato impostato sull'impianto
                        If Dt.Rows(j).Item("N_Max") <> "" Or
                            Dt.Rows(j).Item("P_Max") <> "" Or
                            Dt.Rows(j).Item("K_Max") <> "" Or
                            Dt.Rows(j).Item("Mg_Max") <> "" Then

                            objFert.Leggi_Macroelementi_Distribuiti(N_Distribuito,
                                                                    P_Distribuito,
                                                                    K_Distribuito,
                                                                    Mg_Distribuito,
                                                                    Cu_Distribuito,
                                                                    0, 0, 0, 0, 0,
                                                                        CStr(Dt.Rows(j).Item("Piva")),
                                                                        CInt(Dt.Rows(j).Item("Sa_Cod")),
                                                                        CInt(Dt.Rows(j).Item("Appezza")),
                                                                        CInt(Dt.Rows(j).Item("Id_Reg")),
                                                                        CInt(Dt.Rows(j).Item("Progetto_Cod")),
                                                                        CDate(Dt.Rows(j).Item("Validita_Inizio_Distinta")),
                                                                        CDate(Dt.Rows(j).Item("Validita_Fine_Distinta")),
                                                                    objParametriAgenda.Id_Agenda,
                                                                    objParametri_Server)

                            If objParametriAgenda.TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua Then
                                objDettRic.Leggi_Macroelementi_Distribuiti(N_Distribuito_Ricetta,
                                                                        P_Distribuito_Ricetta,
                                                                        K_Distribuito_Ricetta,
                                                                        Mg_Distribuito_Ricetta,
                                                                        Cu_Distribuito_Ricetta,
                                                                        0, 0, 0, 0, 0,
                                                                        CStr(Dt.Rows(j).Item("Piva")),
                                                                        CInt(Dt.Rows(j).Item("Sa_Cod")),
                                                                        CInt(Dt.Rows(j).Item("Appezza")),
                                                                        CInt(Dt.Rows(j).Item("Id_Reg")),
                                                                        CInt(Dt.Rows(j).Item("Progetto_Cod")),
                                                                        CDate(Dt.Rows(j).Item("Validita_Inizio_Distinta")),
                                                                        CDate(Dt.Rows(j).Item("Validita_Fine_Distinta")),
                                                                        objParametriAgenda.Id_Agenda,
                                                                        objParametri_Server)
                            End If



                            If IsNumeric(Dt.Rows(j).Item("N_Max")) Then
                                Dt.Rows(j).Item("N_Distribuito") = Format(N_Distribuito, "0.###")
                                Dt.Rows(j).Item("N_Residuo") = Format(CDec(Dt.Rows(j).Item("N_Max")) - N_Distribuito, "0.###")
                                strN &= "<b>Già Distribuito: </b><span class='qdc_n_distribuito'>" & Format(N_Distribuito, "0.###") & "</span><br>"
                                If objParametriAgenda.TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua Then
                                    strN &= "<b>Preventivato: </b><span class='qdc_n_preventivato'>" & Format(N_Distribuito_Ricetta, "0.###") & "</span><br>"
                                End If
                                strN &= "<b>Residuo: </b><span class='qdc_n_residuo'>" & Format(CDec(Dt.Rows(j).Item("N_Max")) - N_Distribuito - N_Distribuito_Ricetta, "0.###") & "</span>"
                            End If
                            If IsNumeric(Dt.Rows(j).Item("P_Max")) Then
                                Dt.Rows(j).Item("P_Distribuito") = Format(P_Distribuito, "0.###")
                                Dt.Rows(j).Item("P_Residuo") = Format(CDec(Dt.Rows(j).Item("P_Max")) - P_Distribuito, "0.###")
                                strP &= "<b>Già Distribuito: </b><span class='qdc_p_distribuito'>" & Format(P_Distribuito, "0.###") & "</span><br>"
                                If objParametriAgenda.TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua Then
                                    strP &= "<b>Preventivato: </b><span class='qdc_p_preventivato'>" & Format(P_Distribuito_Ricetta, "0.###") & "</span><br>"
                                End If
                                strP &= "<b>Residuo: </b><span class='qdc_p_residuo'>" & Format(CDec(Dt.Rows(j).Item("P_Max")) - P_Distribuito - P_Distribuito_Ricetta, "0.###") & "</span>"
                            End If
                            If IsNumeric(Dt.Rows(j).Item("K_Max")) Then
                                Dt.Rows(j).Item("K_Distribuito") = Format(K_Distribuito, "0.###")
                                Dt.Rows(j).Item("K_Residuo") = Format(CDec(Dt.Rows(j).Item("K_Max")) - K_Distribuito, "0.###")
                                strK &= "<b>Già Distribuito: </b><span class='qdc_k_distribuito'>" & Format(K_Distribuito, "0.###") & "</span><br>"
                                If objParametriAgenda.TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua Then
                                    strK &= "<b>Preventivato: </b><span class='qdc_k_preventivato'>" & Format(K_Distribuito_Ricetta, "0.###") & "</span><br>"
                                End If
                                strK &= "<b>Residuo: </b><span class='qdc_k_residuo'>" & Format(CDec(Dt.Rows(j).Item("k_Max")) - K_Distribuito - K_Distribuito_Ricetta, "0.###") & "</span>"
                            End If
                            If IsNumeric(Dt.Rows(j).Item("Mg_Max")) Then
                                Dt.Rows(j).Item("Mg_Distribuito") = Format(Mg_Distribuito, "0.###")
                                Dt.Rows(j).Item("Mg_Residuo") = Format(CDec(Dt.Rows(j).Item("Mg_Max")) - Mg_Distribuito, "0.###")
                                strMg &= "<b>Già Distribuito: </b><span class='qdc_mg_distribuito'>" & Format(Mg_Distribuito, "0.###") & "</span><br>"
                                If objParametriAgenda.TipoRicetta = enum_TipoRicetta.PianoDistribuzionePua Then
                                    strMg &= "<b>Preventivato: </b><span class='qdc_mg_preventivato'>" & Format(Mg_Distribuito_Ricetta, "0.###") & "</span><br>"
                                End If
                                strMg &= "<b>Residuo: </b><span class='qdc_mg_residuo'>" & Format(CDec(Dt.Rows(j).Item("Mg_Max")) - Mg_Distribuito - Mg_Distribuito_Ricetta, "0.###") & "</span>"
                            End If
                        End If

                End Select

                Dt.Rows(j).Item("N_Massimo") = strN
                Dt.Rows(j).Item("P_Massimo") = strP
                Dt.Rows(j).Item("K_Massimo") = strK
                Dt.Rows(j).Item("Mg_Massimo") = strMg

                'ANALISI
                'If Not IsNothing(DtTutteLeAnalisi) AndAlso DtTutteLeAnalisi.Rows.Count > 0 Then
                'Dim strPiva As String = Dt.Rows(j).Item("Piva")
                'Dim strSaCod As String = Dt.Rows(j).Item("Sa_Cod")
                'Dim strCampi As String = " (Piva = '" & strPiva & "' AND Sa_Cod = " & strSaCod & " AND Campo_Cod = " & Dt.Rows(j).Item("campo_cod") & ")"
                'Dim strAppezzamenti As String = " (Piva = '" & strPiva & "' AND Sa_Cod = " & strSaCod & " AND Campo_Cod = " & Dt.Rows(j).Item("campo_cod") & " AND Appezza= " & Dt.Rows(j).Item("appezza") & ")"
                'Dim strRegImpianti As String = " (Piva = '" & strPiva & "' AND Sa_Cod = " & strSaCod & " AND Campo_Cod = " & Dt.Rows(j).Item("campo_cod") & " AND Appezza= " & Dt.Rows(j).Item("appezza") & " AND Id_Imp = " & Dt.Rows(j).Item("id_reg") & ")"
                'Dim strAnalisiCatasto As String = ""

                'If Not DtParticelle Is Nothing AndAlso DtParticelle.Rows.Count > 0 Then
                '    DrParticelle = DtParticelle.Select("piva='" & Dt.Rows(j).Item("piva").ToString & "' and sa_cod=" & Dt.Rows(j).Item("sa_cod").ToString & " and appezza=" & Dt.Rows(j).Item("appezza").ToString & " and id_reg=" & Dt.Rows(j).Item("id_reg").ToString)
                '    Dim listaParticelle As New List(Of String)
                '    For Each dr As DataRow In DrParticelle
                '        Dim str As String = " (Piva = '" & strPiva & "' AND prov = '" & dr("prov") & "' AND com = '" & dr("com") & "' AND sezione = '" & dr("sezione") & "' AND numero = " & dr("numero") & " AND foglio = " & dr("foglio") & " AND subalterno = '" & dr("subalterno") & "')"
                '        If Not listaParticelle.Contains(str) Then
                '            listaParticelle.Add(str)
                '        End If
                '    Next
                '    strAnalisiCatasto = String.Join(" OR ", listaParticelle)
                'End If


                'Dim drAnalisi() As DataRow = Nothing
                'If strAnalisiCatasto <> "" Then
                '    drAnalisi = DtTutteLeAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Particella & " and (" & strAnalisiCatasto & ")")
                'End If
                'If Not (Not drAnalisi Is Nothing AndAlso drAnalisi.Length > 0) Then
                '    If strRegImpianti <> "" Then
                '        drAnalisi = DtTutteLeAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Impianto & " and (" & strRegImpianti & ")")
                '    End If
                '    If Not (Not drAnalisi Is Nothing AndAlso drAnalisi.Length > 0) Then
                '        If strAppezzamenti <> "" Then
                '            drAnalisi = DtTutteLeAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Appezzamento & " and (" & strAppezzamenti & ")")
                '        End If
                '        If Not (Not drAnalisi Is Nothing AndAlso drAnalisi.Length > 0) Then
                '            If strCampi <> "" Then
                '                drAnalisi = DtTutteLeAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Campo & " and (" & strCampi & ")")
                '            End If
                '            If Not (Not drAnalisi Is Nothing AndAlso drAnalisi.Length > 0) Then
                '                drAnalisi = DtTutteLeAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Centro & " and Piva = '" & strPiva & "' AND Sa_Cod = " & strSaCod)
                '                If Not (Not drAnalisi Is Nothing AndAlso drAnalisi.Length > 0) Then
                '                    drAnalisi = DtTutteLeAnalisi.Select("analisi_entita_cod=" & enum_Entita_Analisi.Impresa & " and Piva = '" & strPiva & "'")
                '                End If
                '            End If
                '        End If
                '    End If
                'End If

                'Dim arrId_ClassiTessitura As Integer() = (From drA As DataRow In drAnalisi Where IsNumeric(drA.Item("Id_ClasseTessitura")) Select CInt(drA.Item("Id_ClasseTessitura"))).Distinct().ToArray()
                '    Dim listaStrClassiTessitura As New List(Of String)
                '    For Each el As Integer In arrId_ClassiTessitura
                '        Dim strClassiTessitura As String = (From ct As AgronicaCorePianoConcimazioneBIZ.PUA_ClasseTessitura In classTess_output.ListaClassiTessitura Where el = ct.Tessitura_Cod Select CStr(ct.Tessitura_Des)).ToArray()(0)
                '        listaStrClassiTessitura.Add(strClassiTessitura.Trim())
                '    Next

                '    Dt.Rows(j).Item("Id_ClasseTessitura") = String.Join(",", arrId_ClassiTessitura)
                '    Dt.Rows(j).Item("Str_ClasseTessitura") = String.Join(", ", listaStrClassiTessitura)
                'Else
                '    Dt.Rows(j).Item("Id_ClasseTessitura") = ""
                '    Dt.Rows(j).Item("Str_ClasseTessitura") = ""
                'End If



                Dim lpiva As String = Dt.Rows(j)("piva")
                Dim lSa_cod As Integer = Dt.Rows(j)("sa_cod")
                Dim lAppezza As Integer = Dt.Rows(j)("appezza")
                Dim lId_reg As Integer = Dt.Rows(j)("id_reg")

                Dim impianto1 As Impianto = (
                    From ii In objParametriAgenda.Impianti
                    Where ii.Piva = lpiva AndAlso
                          ii.Sa_Cod = lSa_cod AndAlso
                          ii.Appezza = lAppezza AndAlso
                          ii.ID_Reg = lId_reg).FirstOrDefault

                If impianto1 IsNot Nothing AndAlso impianto1.GisWkt <> "" Then
                    Dt.Rows(j).Item("GisWkt") = impianto1.GisWkt
                    Dt.Rows(j).Item("GisWktGps") = impianto1.GisWktGps
                    Dt.Rows(j).Item("GisWktSistemaRiferimento") = impianto1.GisWktSistemaRiferimento
                    Dt.Rows(j).Item("GisTipoEntita_cod") = impianto1.GisTipoEntita_cod
                    Dt.Rows(j).Item("GisLayerCod") = impianto1.GisLayerCod
                Else
                    Dt.Rows(j).Item("GisWkt") = ""
                    Dt.Rows(j).Item("GisWktGps") = ""
                    Dt.Rows(j).Item("GisWktSistemaRiferimento") = ""
                    Dt.Rows(j).Item("GisTipoEntita_cod") = 0
                    Dt.Rows(j).Item("GisLayerCod") = 0
                End If


                If impianto1 IsNot Nothing Then
                    Dt.Rows(j).Item("Sup_Riduzione_BufferZone") = impianto1.Sup_Riduzione_BufferZone
                    Dt.Rows(j).Item("Perc_Riduzione_Deriva") = impianto1.Perc_Riduzione_Deriva
                Else
                    Dt.Rows(j).Item("Sup_Riduzione_BufferZone") = 0
                    Dt.Rows(j).Item("Perc_Riduzione_Deriva") = 0
                End If


                Dim Fase_Cod_Corrente As Integer = 0
                Dim Fase_Des_Corrente As String = ""
                Dim Data_Fase_Corrente As String = ""

                '(12/11/2018 fede)
                If Not IsDBNull(Dt.Rows(j).Item("Fase_Cod_Corrente")) AndAlso
                    IsNumeric(Dt.Rows(j).Item("Fase_Cod_Corrente")) AndAlso
                    CInt(Dt.Rows(j).Item("Fase_Cod_Corrente")) > 0 Then

                    Fase_Cod_Corrente = CInt(Dt.Rows(j).Item("Fase_Cod_Corrente"))

                    Select Case Fase_Cod_Corrente

                        Case < 1000 'caso vecchio av_cod = ff_cod
                            Fase_Des_Corrente = (From aa In objParametriUscitaFasiOld.ListaFasiFenologiche
                                                 Where aa.FF_Cod = Fase_Cod_Corrente
                                                 Select aa.Descrizione
                                        ).FirstOrDefault

                        Case Else ' caso nuovo av_cod= cod_css
                            Fase_Des_Corrente = (From aa In objParametriUscitaFasiNew.ListaFasiFenologiche
                                                 Where aa.Cod_SS = Fase_Cod_Corrente
                                                 Select aa.Descrizione & " - BBCH " & aa.Stadio
                                        ).FirstOrDefault
                    End Select

                    If Not IsDBNull(Dt.Rows(j).Item("Data_Fase_Corrente")) AndAlso
                                IsDate(Dt.Rows(j).Item("Data_Fase_Corrente")) Then
                        Data_Fase_Corrente = CDate(Dt.Rows(j).Item("Data_Fase_Corrente")).ToShortDateString
                    End If

                End If

                If Data_Fase_Corrente <> "" Then
                    Fase_Des_Corrente &= " (" & Data_Fase_Corrente & ")"
                End If

                Dt.Rows(j).Item("Fase_Corrente") = Fase_Des_Corrente

                '(17/12/2018 fede)
                Dim strBuffer As String = ""

                If Not (Dt.Rows(j).Item("DistBZ_CorpiIdrici") = 0 And
                     Dt.Rows(j).Item("DistBZ_AreeResPub") = 0 And
                    Dt.Rows(j).Item("DistBZ_Allevamenti") = 0 And
                    Dt.Rows(j).Item("DistBZ_VegNatNonColt") = 0) Then

                    If Dt.Rows(j).Item("DistBZ_CorpiIdrici") <> 0 Then
                        strBuffer &= "corpi idrici per " & Dt.Rows(j).Item("DistBZ_CorpiIdrici") & " m" & ","
                    End If
                    If Dt.Rows(j).Item("DistBZ_AreeResPub") <> 0 Then
                        strBuffer &= "aree residenziali/pubbliche per " & Dt.Rows(j).Item("DistBZ_AreeResPub") & " m" & ","
                    End If
                    If Dt.Rows(j).Item("DistBZ_Allevamenti") <> 0 Then
                        strBuffer &= "allevamenti per " & Dt.Rows(j).Item("DistBZ_Allevamenti") & " m" & ","
                    End If
                    If Dt.Rows(j).Item("DistBZ_VegNatNonColt") <> 0 Then
                        strBuffer &= "vegetazione naturale/non coltivata per " & Dt.Rows(j).Item("DistBZ_VegNatNonColt") & " m" & ","
                    End If

                    If strBuffer <> "" Then
                        strBuffer = " - <b>contiguo a " & Left(strBuffer, strBuffer.Length - 1) & "</b>"
                    End If

                    If Dt.Rows(j).Item("SupBZ_Riduzione") <> 0 Then
                        strBuffer &= " - <b>Offset (capezzagna) di " & Dt.Rows(j).Item("SupBZ_Riduzione") & " m</b>"
                    End If


                End If

                If strBuffer <> "" Then
                    Dt.Rows(j).Item("app_nome") &= strBuffer
                End If



            Next

        End If

        '(31/07/2018 fede) aggiunte colonne x dati gis
        Dim objGis As New AgronicaControlliGIS.V_M
        Dim r As New RispostaStandard
        r = objGis.AgendaLetturaWKT_SuListaImpianti(objParametriAgenda.Lav_Cod, Dt, False, objParametri_Server)

    End Sub

    Public Function CaricaGriglia_CostiAccessori_xJSON(ByVal dt As DataTable) As String


        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        ' VAnni: 15/2/2017: todo: verificare tutte le chiavi commentate (es: tariffa_cod per costi SBTF..)

        c = New ColonneNome("kendoKey", "kendoKey", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Sa_Cod", "Sa_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Centro_Cod", "Centro_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Centro", "Centri di Costo", "string")
        c._Filtrabile = True
        c._Display = False
        l.Add(c)

        'c = New ColonneNome("Categoria_Des", "Categoria Risorsa", "string")
        'l.Add(c)

        c = New ColonneNome("Elem_Cod", "Elem_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Pro_Cod", "Pro_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Mat_Cod", "Mat_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Risorsa_Cod", "Risorsa_Cod", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Risorsa_Des", Resources.AgronicaAgenda_2010.Risorsa, "string")
        c._hidden = True
        l.Add(c)

        'decido le colonne in base al tipo di algoritmo
        Dim xDisplayBloccoUDM As Boolean = False
        If {enum_AlgoritmoCostiAccessori.CAB, enum_AlgoritmoCostiAccessori.SBTF}.Contains(ALGORITMO_COSTI_ACCESSORI) Then
            xDisplayBloccoUDM = True
        End If

        c = New ColonneNome("Udm_Cod", "Udm_Cod", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Udm_Des", Resources.AgronicaAgenda_2010.RisorsaUDM, "string")
        c._Filtrabile = False
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Qta_Ril", Resources.AgronicaAgenda_2010.RisorsaQuantità, "number")
        c._Filtrabile = False
        c._Display = xDisplayBloccoUDM
        l.Add(c)

        c = New ColonneNome("Riga", "Riga", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Tipo_Centro", "Tipo_Centro", "string")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Costo_Unitario", Resources.AgronicaAgenda_2010.RisorsaCostoUnitario & " (&euro;)", "number")
        c._hidden = Not xDisplayBloccoUDM
        c._formatNr = "n2"
        l.Add(c)

        c = New ColonneNome("Costo", Resources.AgronicaAgenda_2010.RisorsaCosto & " (&euro;)", "number")
        c._hidden = Not xDisplayBloccoUDM
        c._formatNr = "n2"
        l.Add(c)

        c = New ColonneNome("Ore", "Ore", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Minuti", "Minuti", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Ditta_Cod", "Ditta_Cod", "number")
        c._hidden = True
        l.Add(c)

        'c = New ColonneNome("Codice", "Codice", "string")
        'c._hidden = true
        'l.Add(c)

        'c = New ColonneNome("Lotto", "Lotto", "string")
        'c._hidden = true
        'l.Add(c)

        'c = New ColonneNome("Udm_Selezionata", "Udm_Selezionata", "number")
        'c._hidden = true
        'l.Add(c)

        c = New ColonneNome("Valore", "Valore", "string")
        c._hidden = True
        l.Add(c)

        'Attvità
        c = New ColonneNome("Id_attivita", "Id_attivita", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Attivita_Des", "Attivita", "string")
        c._hidden = True
        l.Add(c)

        'Turno
        If ALGORITMO_COSTI_ACCESSORI = enum_AlgoritmoCostiAccessori.CAB Then
            c = New ColonneNome("Turno_Cod", "Turno_cod", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("Turno_Des", "Turno", "string")
            c._hidden = True
            l.Add(c)
        End If

        'c = New ColonneNome("Qualifica_Cod", "Qualifica_Cod", "number")
        'c._hidden = true
        'l.Add(c)

        'c = New ColonneNome("Tariffa_Cod", "Tariffa_Cod", "number")
        'c._hidden = true
        'l.Add(c)

        c = New ColonneNome("Cod_Rapporto", "Cod_Rapporto", "number")
        c._hidden = True
        l.Add(c)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = True
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)

        hdKendo_CostiAccessori.Value = risp
        hdKendo_CostiAccessori_Ricarica.Value = "ricarica"

        Return risp

    End Function


    Public Function CaricaGriglia_Impianti_xJSON(Optional ByVal dtParam As DataTable = Nothing) As String

        Dim Dt As New DataTable

        'icona per le info aggiuntive
        Dim DtKeys As String() = Nothing

        If dtParam Is Nothing Then
            CaricaGriglia_Impianti_getDT(Dt, DtKeys)
        Else
            Dt = dtParam
        End If




        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("kendoKey", "kendoKey", "string"))
        l.Add(New ColonneNome("Rag_Soc", Resources.AgronicaAgenda_2010.RagioneSociale, "string"))
        l.Add(New ColonneNome("Sa_Nome", Resources.AgronicaAgenda_2010.CentroAziendale, "string"))
        l.Add(New ColonneNome("Campo_Des", Resources.AgronicaAgenda_2010.Campo, "string"))
        l.Add(New ColonneNome("App_Nome", Resources.AgronicaAgenda_2010.App, "string") With {._RemoveHtmlEncode = True})
        l.Add(New ColonneNome("CodBioApp", Resources.AgronicaAgenda_2010.AppBioCod, "string"))
        l.Add(New ColonneNome("Catasto", Resources.AgronicaAgenda_2010.Catasto, "string") With {._RemoveHtmlEncode = True})

        '(12/11/2018 fede) aggiunta fase fenologica corrente
        l.Add(New ColonneNome("Fase_Corrente", Resources.AgronicaAgenda_2010.FaseFenologicaCorrente, "string"))

        l.Add(New ColonneNome("Codici_Anagrafe_Des", Resources.AgronicaAgenda_2010.DestinazioneDUso, "string"))
        l.Add(New ColonneNome("Cul_Des", Resources.AgronicaAgenda_2010.Varietà, "string"))
        l.Add(New ColonneNome("Grva_Des", Resources.AgronicaAgenda_2010.GruppoVarietale, "string"))

        l.Add(New ColonneNome("Sup_Imp", "Sup.[Ha]", "number") With {._formatNr = "n4", ._css = "Sup_Imp"})

        'Select Case objParametriAgenda.Lav_Cod
        l.Add(New ColonneNome("Sup_Riduzione_BufferZone", "Sup. Riduzione [Ha] Buffer", "number") With {._Editabile = True, ._css = "Sup_Riduzione_Buffer", ._formatNr = "n4"})
        l.Add(New ColonneNome("Perc_Riduzione_Deriva", "Mitigazione Deriva [%]", "number") With {._Editabile = True, ._css = "Perc_Riduzione_Deriva", ._formatNr = "n4"})
        l.Add(New ColonneNome("SupBZ_Riduzione", "SupBZ_Riduzione", "number") With {._hidden = False})

        'la colonna viene costruita lato client
        'c._FormatoParticolare = "<input type=\""text\"" id=\""${dataItem[idModel]}\"" class=\""Sup_Coinvolta txtUI\"" style=\""width:  60px\"" value=\""0\"" > "
        l.Add(New ColonneNome("Sup_Imp_help", "Sup. Trattata[Ha]", "number") With {._Editabile = True, ._css = "Sup_Coinvolta", ._formatNr = "n5", ._hidden = True})

        l.Add(New ColonneNome("DistBZ_CorpiIdrici", "DistBZ_CorpiIdrici", "number") With {._formatNr = "n5", ._css = "DistBZ_CorpiIdrici", ._hidden = True})
        l.Add(New ColonneNome("DistBZ_AreeResPub", "DistBZ_AreeResPub", "number") With {._formatNr = "n5", ._css = "DistBZ_AreeResPub", ._hidden = True})
        l.Add(New ColonneNome("DistBZ_Allevamenti", "DistBZ_Allevamenti", "number") With {._formatNr = "n5", ._css = "DistBZ_Allevamenti", ._hidden = True})
        l.Add(New ColonneNome("DistBZ_VegNatNonColt", "DistBZ_VegNatNonColt", "number") With {._formatNr = "n5", ._css = "DistBZ_VegNatNonColt", ._hidden = True})
        l.Add(New ColonneNome("SupBZ_Riduzione", "Sup.Riduzione BZ[Ha]", "number") With {._formatNr = "n5", ._css = "SupBZ_Riduzione", ._hidden = True})


        l.Add(New ColonneNome("Disciplinare", Resources.AgronicaAgenda_2010.Disciplinare, "string"))
        l.Add(New ColonneNome("Reg_Des", Resources.AgronicaAgenda_2010.Regolamento, "string"))
        l.Add(New ColonneNome("Capitolato_Privato_Des", Resources.AgronicaAgenda_2010.Capitolato, "string"))
        l.Add(New ColonneNome("Grfi_Des", Resources.AgronicaAgenda_2010.Finalità, "string"))
        l.Add(New ColonneNome("Validita_Inizio", Resources.AgronicaAgenda_2010.DataInizioImpianto, "date"))
        l.Add(New ColonneNome("Validita_Fine", "Validita_Fine", "date") With {._hidden = True})
        l.Add(New ColonneNome("Data_Semina", Resources.AgronicaAgenda_2010.DataSemina, "date"))
        l.Add(New ColonneNome("Data_Fioritura", Resources.AgronicaAgenda_2010.DataFioritura, "date"))
        l.Add(New ColonneNome("Data_Fioritura_Prevista", Resources.AgronicaAgenda_2010.DataFiorituraPrevista, "date"))
        l.Add(New ColonneNome("Data_Raccolta_Prevista", Resources.AgronicaAgenda_2010.DataRaccoltaPrevista, "date"))
        l.Add(New ColonneNome("Progetto", Resources.AgronicaAgenda_2010.LottoImpianto, "string"))
        l.Add(New ColonneNome("Copertura", Resources.AgronicaAgenda_2010.Copertura, "string"))
        l.Add(New ColonneNome("Data_Raccolta", Resources.AgronicaAgenda_2010.DataRaccolta, "date"))
        l.Add(New ColonneNome("Data_Semina_Prevista", Resources.AgronicaAgenda_2010.DataSeminaPrevista, "date"))
        l.Add(New ColonneNome("Id_ClasseTessitura", "Id_ClasseTessitura", "String") With {._hidden = True})
        l.Add(New ColonneNome("Str_ClasseTessitura", Resources.AgronicaAgenda_2010.ClasseTessitura, "String"))
        l.Add(New ColonneNome("N_Massimo", Resources.AgronicaAgenda_2010.NKgHa, "String") With {._RemoveHtmlEncode = True})
        l.Add(New ColonneNome("P_Massimo", Resources.AgronicaAgenda_2010.PKgHa, "String") With {._RemoveHtmlEncode = True})
        l.Add(New ColonneNome("K_Massimo", Resources.AgronicaAgenda_2010.KKgHa, "String") With {._RemoveHtmlEncode = True})
        l.Add(New ColonneNome("Mg_Massimo", Resources.AgronicaAgenda_2010.MgKgHa, "String") With {._RemoveHtmlEncode = True})
        l.Add(New ColonneNome("SpecieAgea", Resources.AgronicaAgenda_2010.SpecieAgea, "String"))
        l.Add(New ColonneNome("CultivarAgea", Resources.AgronicaAgenda_2010.VarietàAgea, "String"))
        l.Add(New ColonneNome("Tra_Fila", Resources.AgronicaAgenda_2010.TraFila, "String"))
        l.Add(New ColonneNome("Su_Fila", Resources.AgronicaAgenda_2010.SuFila, "String"))
        l.Add(New ColonneNome("P_Impianto", Resources.AgronicaAgenda_2010.NPianteImpianto, "String"))
        l.Add(New ColonneNome("Foral_Des", Resources.AgronicaAgenda_2010.FormaAllevamento, "String"))
        l.Add(New ColonneNome("Port_Des", Resources.AgronicaAgenda_2010.Portinnesto, "String"))
        l.Add(New ColonneNome("Finanziamento", "Finanziamento", "String"))
        l.Add(New ColonneNome("Regolamento", "Regolamento_Cod", "String"))
        l.Add(New ColonneNome("Progetto_Cod", "Progetto_Cod", "number"))
        l.Add(New ColonneNome("Validita_Inizio_Distinta", "Validita_Inizio_Distinta", "Date"))
        l.Add(New ColonneNome("Validita_Fine_Distinta", "Validita_Fine_Distinta", "Date"))
        l.Add(New ColonneNome("Descrizione_Unica", "Descrizione_Unica", "String") With {._RemoveHtmlEncode = True})

        '(31/07/2018 fede) aggiunte colonne x dati gis
        l.Add(New ColonneNome("GisWkt", "GisWkt", "String") With {._hidden = False})
        l.Add(New ColonneNome("GisWktGps", "GisWktGps", "String") With {._hidden = False})
        l.Add(New ColonneNome("GisWktSistemaRiferimento", "GisWktSistemaRiferimento", "String") With {._hidden = False})
        l.Add(New ColonneNome("GisTipoEntita_cod", "GisTipoEntita_cod", "number") With {._hidden = False})
        l.Add(New ColonneNome("GisLayerCod", "GisLayerCod", "number") With {._hidden = False})

        l.Add(New ColonneNome("Cul_Cod", "Cul_Cod", "String") With {._hidden = True})

        l.Add(New ColonneNome("Veg_Cod", "Veg_Cod", "String") With {._hidden = True})
        l.Add(New ColonneNome("Campo_Cod", "Campo_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("Grfi_Cod", "Grfi_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("Cop_Cod", "Cop_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("Stato_Impianto", "Stato_Impianto", "number") With {._hidden = True})
        l.Add(New ColonneNome("Validita_Inizio_Appezzamento", "Validita_Inizio_Appezzamento", "Date") With {._hidden = True})
        l.Add(New ColonneNome("Validita_Fine_Appezzamento", "Validita_Fine_Appezzamento", "Date") With {._hidden = True})
        l.Add(New ColonneNome("RifNumerico", Resources.AgronicaAgenda_2010.AppRifNum, "string"))

        If visualizza_Kpin_BlockName Then
            l.Add(New ColonneNome("KPIN", "KPIN", "string"))
            l.Add(New ColonneNome("Block_Name", "Block Name", "string"))
        End If

        If visualizza_codici_imp_app_prj Then
            l.Add(New ColonneNome("Codice_Impianto", "Codice Impianto", "string"))

        End If

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(Dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaDiscesa)

        hdKendo_Impianti.Value = risp
        hdKendo_Impianti_Ricarica.Value = "ricarica"

        Return risp

    End Function

    Public Function GetImpiantiKendo() As List(Of Impianto)
        Dim jss = New JavaScriptSerializer()
        Return jss.Deserialize(Of List(Of Impianto))(hdKendo_Impianti_Selezione.Value)
    End Function

    Public Function GetImpianti() As List(Of Impianto)

        Return GetImpiantiKendo()

    End Function

    Public Function GetClassiTessituraImpiantiSelezionati() As List(Of Integer)

        Dim ClassiTessitura As New List(Of Integer)

        Dim ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto) = GetImpianti()
        For Each imp As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In ListaImpianti
            If Not IsNothing(imp.ListaClassiTessitura) Then
                For Each i As Integer In imp.ListaClassiTessitura
                    If Not ClassiTessitura.Contains(i) Then
                        ClassiTessitura.Add(i)
                    End If
                Next
            End If
        Next

        Return ClassiTessitura

    End Function

#End Region

#Region "Note"

    Private Sub Carica_Note()

        'modifico la finestra temporale in modo da caricare solamente quelle che sono attive a oggi
        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(objParametriAgenda.Data,
                                                                      objParametriAgenda.Data)

        Dim strFiltro As String = " Note_Intervento.Nota_Cod > 0 "
        Dim GruppoDes As String = ""

        Dim NotaUtilizzo_Cod As enum_Note_Intervento_Utilizzo = enum_Note_Intervento_Utilizzo.QuadernoCampagna

        If Not objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna Then
            NotaUtilizzo_Cod = enum_Note_Intervento_Utilizzo.Ricetta
        End If


        AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Consigli,
                                                        False,
                                                         "", "",
                                                         0,
                                                         NotaUtilizzo_Cod,
                                                         strFiltro, "",
                                                         objParametri_Server,
                                                         1,
                                                         GruppoDes)

        If GruppoDes <> "" Then
            lblGiustificazioni.Text = GruppoDes
        End If

        objParametri_Server.ResettaFinestra()



        '(07/09/2016 fede) aggiunti tab standard (NotaGruppo_Cod <0)
        Dim objNoteGruppi_R As New AgronicaCoreContabDAL.Note_Intervento_Gruppi_R

        Dim DT_NoteGruppi As DataTable =
            objNoteGruppi_R.Leggi(0,
                                  enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                    " NotaGruppo_Cod < 0",
                                    "",
                                     objParametri_Server)

        If DT_NoteGruppi IsNot Nothing Then
            For n = 0 To DT_NoteGruppi.Rows.Count - 1
                Select Case DT_NoteGruppi.Rows(n).Item("NotaGruppo_Cod")

                    Case enum_Note_Intervento_Gruppi.Meteo
                        If DT_NoteGruppi.Rows(n).Item("visibile") = 0 Then
                            tabMeteo.Visible = False
                            tab_meteo.Visible = False
                        Else
                            AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Meteo,
                                                False,
                                                 "", "",
                                                 enum_Note_Intervento_Gruppi.Meteo,
                                                 NotaUtilizzo_Cod,
                                                 "", "",
                                                 objParametri_Server,
                                                 1)
                        End If

                    Case enum_Note_Intervento_Gruppi.Vento_Intensita
                        If DT_NoteGruppi.Rows(n).Item("visibile") = 0 Then
                            tabVentoIntensita.Visible = False
                            tab_ventoint.Visible = False
                        Else
                            AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_VentoIntensita,
                                                False,
                                                 "", "",
                                                 enum_Note_Intervento_Gruppi.Vento_Intensita,
                                                 NotaUtilizzo_Cod,
                                                 "", "",
                                                 objParametri_Server,
                                                 1)
                        End If

                    Case enum_Note_Intervento_Gruppi.Vento_Direzione
                        If DT_NoteGruppi.Rows(n).Item("visibile") = 0 Then
                            tabVentoDirezione.Visible = False
                            tab_ventodir.Visible = False
                        Else
                            AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_VentoDirezione,
                                                False,
                                                 "", "",
                                                 enum_Note_Intervento_Gruppi.Vento_Direzione,
                                                 NotaUtilizzo_Cod,
                                                 "", "",
                                                 objParametri_Server,
                                                 1)
                        End If
                    Case enum_Note_Intervento_Gruppi.Temperatura
                        If DT_NoteGruppi.Rows(n).Item("visibile") = 0 Then
                            tabTemperatura.Visible = False
                            tab_temperatura.Visible = False
                        Else
                            AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Temperatura,
                                                False,
                                                 "", "",
                                                 enum_Note_Intervento_Gruppi.Temperatura,
                                                 NotaUtilizzo_Cod,
                                                 "", "",
                                                 objParametri_Server,
                                                 1)
                        End If
                    Case enum_Note_Intervento_Gruppi.Orario
                        If DT_NoteGruppi.Rows(n).Item("visibile") = 0 Then
                            tabOrario.Visible = False
                            tab_orario.Visible = False
                        Else
                            AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Orario,
                                                False,
                                                 "", "",
                                                 enum_Note_Intervento_Gruppi.Orario,
                                                 NotaUtilizzo_Cod,
                                                 "", "",
                                                 objParametri_Server,
                                                 1)
                        End If
                    Case enum_Note_Intervento_Gruppi.Motivazione
                        If DT_NoteGruppi.Rows(n).Item("visibile") = 0 Then
                            tabMotivazioni.Visible = False
                            tab_motivazioni.Visible = False
                        Else
                            AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Motivazione,
                                                False,
                                                 "", "",
                                                 enum_Note_Intervento_Gruppi.Motivazione,
                                                 NotaUtilizzo_Cod,
                                                 "", "",
                                                 objParametri_Server,
                                                 1)
                        End If
                End Select
            Next
        End If

        '-----------------------------------------------------
        'se ho note selezionate le checkko!!!
        '-----------------------------------------------------
        Dim Nota_Cod As Integer
        Dim TrovataNota As Boolean = False

        For i = 0 To objParametriAgenda.Note.Count - 1

            TrovataNota = False

            Nota_Cod = objParametriAgenda.Note(i).Nota_Cod

            Select Case Nota_Cod

                Case Is > 0 'utente

                    For j = 0 To CBL_Consigli.Items.Count - 1
                        If Nota_Cod = CBL_Consigli.Items(j).Value Then
                            CBL_Consigli.Items(j).Selected = True
                            TrovataNota = True
                            Continue For
                        End If
                    Next

                Case Is < 0 'standard

                    If CBL_Meteo.Visible = True AndAlso TrovataNota = False Then
                        For j = 0 To CBL_Meteo.Items.Count - 1
                            If Nota_Cod = CBL_Meteo.Items(j).Value Then
                                CBL_Meteo.Items(j).Selected = True
                                TrovataNota = True
                                Continue For
                            End If
                        Next
                    End If
                    If CBL_VentoIntensita.Visible = True AndAlso TrovataNota = False Then
                        For j = 0 To CBL_VentoIntensita.Items.Count - 1
                            If Nota_Cod = CBL_VentoIntensita.Items(j).Value Then
                                CBL_VentoIntensita.Items(j).Selected = True
                                TrovataNota = True
                                Continue For
                            End If
                        Next
                    End If
                    If CBL_VentoDirezione.Visible = True AndAlso TrovataNota = False Then
                        For j = 0 To CBL_VentoDirezione.Items.Count - 1
                            If Nota_Cod = CBL_VentoDirezione.Items(j).Value Then
                                CBL_VentoDirezione.Items(j).Selected = True
                                TrovataNota = True
                                Continue For
                            End If
                        Next
                    End If
                    If CBL_Temperatura.Visible = True AndAlso TrovataNota = False Then
                        For j = 0 To CBL_Temperatura.Items.Count - 1
                            If Nota_Cod = CBL_Temperatura.Items(j).Value Then
                                CBL_Temperatura.Items(j).Selected = True
                                TrovataNota = True
                                Continue For
                            End If
                        Next
                    End If
                    If CBL_Orario.Visible = True AndAlso TrovataNota = False Then
                        For j = 0 To CBL_Orario.Items.Count - 1
                            If Nota_Cod = CBL_Orario.Items(j).Value Then
                                CBL_Orario.Items(j).Selected = True
                                TrovataNota = True
                                Continue For
                            End If
                        Next
                    End If
                    If CBL_Motivazione.Visible = True AndAlso TrovataNota = False Then
                        For j = 0 To CBL_Motivazione.Items.Count - 1
                            If Nota_Cod = CBL_Motivazione.Items(j).Value Then
                                CBL_Motivazione.Items(j).Selected = True
                                TrovataNota = True
                                Continue For
                            End If
                        Next
                    End If

            End Select

            'nota salvata in agenda ma resa ora non visibile
            'se il SUO gruppo è visibile la aggiungo al suo gruppo
            'se il SUO gruppo è NON visibile la aggiungo alla lista generica consigli
            'la chekko in ogni caso
            If TrovataNota = False Then
                Dim objNota As New AgronicaCoreContabDAL.Note_Intervento_Utilizzo_R
                Dim Dt_Nota As DataTable
                Dim NotaDes As String = ""
                Dim FiltroNota As String = " NI.Nota_Cod = " & Nota_Cod.ToString
                Dt_Nota = objNota.LeggiNote_X_Utilizzo_Visibile(0, 0, False, FiltroNota, objParametri_Server)
                If Dt_Nota IsNot Nothing AndAlso Dt_Nota.Rows.Count > 0 Then
                    NotaDes = Dt_Nota.Rows(0).Item("Nota_Des")
                    Select Case Dt_Nota.Rows(0).Item("VisibileGruppo")
                        Case 0
                            CBL_Consigli.Items.Add(New ListItem(NotaDes, Nota_Cod))
                            CBL_Consigli.Items(CBL_Consigli.Items.Count - 1).Selected = True
                        Case Else
                            Select Case Dt_Nota.Rows(0).Item("NotaGruppo_Cod")
                                Case enum_Note_Intervento_Gruppi.Meteo
                                    CBL_Meteo.Items.Add(New ListItem(NotaDes, Nota_Cod))
                                    CBL_Meteo.Items(CBL_Meteo.Items.Count - 1).Selected = True
                                Case enum_Note_Intervento_Gruppi.Vento_Intensita
                                    CBL_VentoIntensita.Items.Add(New ListItem(NotaDes, Nota_Cod))
                                    CBL_VentoIntensita.Items(CBL_VentoIntensita.Items.Count - 1).Selected = True
                                Case enum_Note_Intervento_Gruppi.Vento_Direzione
                                    CBL_VentoDirezione.Items.Add(New ListItem(NotaDes, Nota_Cod))
                                    CBL_VentoDirezione.Items(CBL_VentoDirezione.Items.Count - 1).Selected = True
                                Case enum_Note_Intervento_Gruppi.Temperatura
                                    CBL_Temperatura.Items.Add(New ListItem(NotaDes, Nota_Cod))
                                    CBL_Temperatura.Items(CBL_Temperatura.Items.Count - 1).Selected = True
                                Case enum_Note_Intervento_Gruppi.Orario
                                    CBL_Orario.Items.Add(New ListItem(NotaDes, Nota_Cod))
                                    CBL_Orario.Items(CBL_Orario.Items.Count - 1).Selected = True
                                Case enum_Note_Intervento_Gruppi.Motivazione
                                    CBL_Motivazione.Items.Add(New ListItem(NotaDes, Nota_Cod))
                                    CBL_Motivazione.Items(CBL_Motivazione.Items.Count - 1).Selected = True
                                Case Else
                                    CBL_Consigli.Items.Add(New ListItem(NotaDes, Nota_Cod))
                                    CBL_Consigli.Items(CBL_Consigli.Items.Count - 1).Selected = True
                            End Select
                    End Select

                End If
            End If

        Next


    End Sub


    'Public Function GetConsigli() As List(Of Nota)

    '    Dim ListaNote As New List(Of Nota)
    '    Dim i As Integer

    '    For i = 0 To CBL_Consigli.Items.Count - 1

    '        If CBL_Consigli.Items(i).Selected = True Then

    '            Dim Nota As New Nota
    '            Nota.Nota_Cod = CBL_Consigli.Items(i).Value
    '            ListaNote.Add(Nota)

    '        End If

    '    Next

    '    Return ListaNote

    'End Function

    Public Function GetConsigli() As List(Of Nota)

        Dim ListaNote As New List(Of Nota)
        Dim i As Integer

        For i = 0 To CBL_Consigli.Items.Count - 1
            If CBL_Consigli.Items(i).Selected = True Then
                Dim Nota As New Nota
                Nota.Nota_Cod = CBL_Consigli.Items(i).Value
                ListaNote.Add(Nota)
            End If
        Next

        'aggiunte note standard (se visibili)
        If CBL_Meteo.Visible = True Then
            For i = 0 To CBL_Meteo.Items.Count - 1
                If CBL_Meteo.Items(i).Selected = True Then
                    Dim Nota As New Nota
                    Nota.Nota_Cod = CBL_Meteo.Items(i).Value
                    ListaNote.Add(Nota)
                End If
            Next
        End If
        If CBL_VentoIntensita.Visible = True Then
            For i = 0 To CBL_VentoIntensita.Items.Count - 1
                If CBL_VentoIntensita.Items(i).Selected = True Then
                    Dim Nota As New Nota
                    Nota.Nota_Cod = CBL_VentoIntensita.Items(i).Value
                    ListaNote.Add(Nota)
                End If
            Next
        End If
        If CBL_VentoDirezione.Visible = True Then
            For i = 0 To CBL_VentoDirezione.Items.Count - 1
                If CBL_VentoDirezione.Items(i).Selected = True Then
                    Dim Nota As New Nota
                    Nota.Nota_Cod = CBL_VentoDirezione.Items(i).Value
                    ListaNote.Add(Nota)
                End If
            Next
        End If
        If CBL_Temperatura.Visible = True Then
            For i = 0 To CBL_Temperatura.Items.Count - 1
                If CBL_Temperatura.Items(i).Selected = True Then
                    Dim Nota As New Nota
                    Nota.Nota_Cod = CBL_Temperatura.Items(i).Value
                    ListaNote.Add(Nota)
                End If
            Next
        End If
        If CBL_Orario.Visible = True Then
            For i = 0 To CBL_Orario.Items.Count - 1
                If CBL_Orario.Items(i).Selected = True Then
                    Dim Nota As New Nota
                    Nota.Nota_Cod = CBL_Orario.Items(i).Value
                    ListaNote.Add(Nota)
                End If
            Next
        End If
        If CBL_Motivazione.Visible = True Then
            For i = 0 To CBL_Motivazione.Items.Count - 1
                If CBL_Motivazione.Items(i).Selected = True Then
                    Dim Nota As New Nota
                    Nota.Nota_Cod = CBL_Motivazione.Items(i).Value
                    ListaNote.Add(Nota)
                End If
            Next
        End If

        Return ListaNote

    End Function
    Public Function GetNota() As String
        Return Txt_Note.Text
    End Function

    Public Sub SetNota(ByVal Nota As String)
        Txt_Note.Text = Nota
    End Sub



#End Region

#Region "Costi Accessori"


    ' Rende visibile il bottone dei costi accessori se c'è il permesso del rilievo attività

    ' nico, vale per i costi acc normali e avanzati
    Public Sub CaricaCostiAccessori()
        'il DT viene salvato nel Session
        Session("dtScarico") = Nothing

        CaricaObjParametri()
        Costruisci_DT_Scarico()
        Datatable_from_MovimentiCostiAccessori()    ' salva Session("dtScarico") 
        AggiornaGridViewCostiAccessoriVisibili()
    End Sub

    Private Sub AggiornaGridViewCostiAccessoriVisibili()

        ' VAnni: 15/2/2017: TODO: verificare il databind..
        Exit Sub

        If Not IsNothing(Session("dtScarico")) Then

            'Vettore di DataColumn
            Dim ScaricoKeys(3) As String
            ScaricoKeys(0) = "Udm_Selezionata"
            ScaricoKeys(1) = "ID_Attivita"
            ScaricoKeys(2) = "Turno_Cod"
            ScaricoKeys(3) = "Qta_Ril"

            GridViewCostiAccessoriVisibili.DataSource = Session("dtScarico")
            GridViewCostiAccessoriVisibili.DataKeyNames = ScaricoKeys
            GridViewCostiAccessoriVisibili.DataBind()

            ControllaDDLCostiAccessori(GridViewCostiAccessoriVisibili)
        End If
    End Sub

    Private Sub AggiornaDgrScarico()
        If Not IsNothing(Session("dtScarico")) Then

            'Vettore di DataColumn
            Dim ScaricoKeys(3) As String
            ScaricoKeys(0) = "Udm_Selezionata"
            ScaricoKeys(1) = "ID_Attivita"
            ScaricoKeys(2) = "Turno_Cod"
            ScaricoKeys(3) = "Qta_Ril"

            dgrScarico.DataSource = Session("dtScarico")
            dgrScarico.DataKeyNames = ScaricoKeys
            dgrScarico.DataBind()

            'dopo aver fatto il bind verifico come impostare la dropdown
            ControllaDDLCostiAccessori(dgrScarico)
        End If
    End Sub

    Private Sub ControllaDDLCostiAccessori(ByVal Griglia As GridView)
        Dim dt As DataTable = Session("dtScarico")
        If dt IsNot Nothing AndAlso dt.Rows.Count = Griglia.Rows.Count Then
            For i As Integer = 0 To Griglia.Rows.Count - 1
                CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Clear()
                If dt.Rows(i).Item("Centro_cod") < 0 Then
                    CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("ora", "2"))
                    CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("ha", "1"))
                    CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("indefinito", "-1"))
                Else
                    CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("kg", "2"))
                    CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("litri", "29"))
                End If

                'controllo se l'utente ha cambiato valore
                If Not IsDBNull(dt.Rows(i).Item("Udm_Selezionata")) AndAlso dt.Rows(i).Item("Udm_Selezionata") <> "" Then
                    CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).SelectedValue = Griglia.DataKeys(i).Item(0).ToString  'dt.Rows(i).Item("Udm_Selezionata") 
                Else
                    Dim desUdm As String
                    If dt.Rows(i).Item("Udm_Des").ToString.Contains(" ") Then
                        desUdm = Split(dt.Rows(i).Item("Udm_Des"), " ")(0)
                    Else
                        desUdm = dt.Rows(i).Item("Udm_Des")
                    End If

                    For j As Integer = 0 To CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Count - 1
                        If CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items(j).Text = desUdm Then
                            CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).SelectedIndex = j
                            Exit For
                        End If
                    Next
                End If

                'If CInt(dt.Rows(i).Item("Costo_Unitario")) <> 0 Then
                '    CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Enabled = False
                'End If

                'txt selezionata
                If Not IsDBNull(dt.Rows(i).Item("Qta_Ril")) AndAlso dt.Rows(i).Item("Qta_Ril") <> "0.0" Then
                    CType(Griglia.Rows(i).FindControl("Txt_Qta_Ril"), TextBox).Text = Griglia.DataKeys(i).Item(3).ToString  ' dt.Rows(i).Item("Qta_Ril")
                End If

                Dim objCmbAttivita As DropDownList = CType(Griglia.Rows(i).FindControl("Cmb_Attivita"), DropDownList)
                Dim objCmbTurni As DropDownList = CType(Griglia.Rows(i).FindControl("Cmb_Turni"), DropDownList)

                If Not IsNothing(objCmbAttivita) AndAlso Not IsNothing(objCmbTurni) Then

                    ' solo con manodopera, terzisti e tecnico responsabile
                    If dt.Rows(i).Item("Centro_cod") < -1 Then

                        AgronicaCoreUtility.CaricaListControl.Attivita(objCmbAttivita, True, "", "0", 0, "", "", objParametri_Server)
                        AgronicaCoreUtility.CaricaListControl.Turni(objCmbTurni, True, "", "0", 0, "", "", objParametri_Server)

                        'Dim StrSelect As New StringBuilder
                        'classe per autocomplete della combo
                        'StrSelect.AppendLine("$(document).ready(Function() { ")
                        'StrSelect.AppendLine("   $('#" & objCmbAttivita.ClientID & "').combobox();")
                        'StrSelect.AppendLine("   $('#" & objCmbAttivita.ClientID & "').combobox().parent().find('input.ui-autocomplete-input').css('width', '150px');")
                        'StrSelect.AppendLine("   $('#" & objCmbTurni.ClientID & "').combobox();")
                        ' StrSelect.AppendLine("   $('#" & objCmbTurni.ClientID & "').combobox().parent().find('input.ui-autocomplete-input').css('width', '150px');")
                        'StrSelect.AppendLine("});")

                        'ScriptManager.RegisterStartupScript(UpdatePanelPerScript, UpdatePanelPerScript.GetType(),
                        '                                       String.Format("jQuery_{0}", objCmbAttivita.ClientID), StrSelect.ToString, True)

                        If Not IsDBNull(dt.Rows(i).Item("Id_Attivita")) Then
                            objCmbAttivita.SelectedIndex = objCmbAttivita.Items.IndexOf(objCmbAttivita.Items.FindByValue(Griglia.DataKeys(i).Item(1).ToString))
                        End If

                        If Not IsDBNull(dt.Rows(i).Item("Turno_Cod")) Then
                            objCmbTurni.SelectedIndex = objCmbTurni.Items.IndexOf(objCmbTurni.Items.FindByValue(Griglia.DataKeys(i).Item(2).ToString))
                        End If

                    Else

                        objCmbAttivita.Visible = False
                        objCmbTurni.Visible = False

                    End If


                End If


            Next
        End If

    End Sub

    Private Sub BTN_ComboAttivita_Click(sender As Object, e As System.EventArgs) Handles BTN_ComboAttivita.Click
        ' per riapplicare la combo con l'autocomplete
        AggiornaCostiAccessoriAvanzati_Click(Me, Nothing)

    End Sub

    Private Sub BTN_ComboTurni_Click(sender As Object, e As System.EventArgs) Handles BTN_ComboTurni.Click
        ' per riapplicare la combo con l'autocomplete
        AggiornaCostiAccessoriAvanzati_Click(Me, Nothing)

    End Sub

    'click sul bottone dei costi accessori
    Protected Sub AggiornaCostiAccessori_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AggiornaCostiAccessori.Click


        ' CalcolaCosti.Checked = False

        'tabellaCostiAccessori.Visible = True

        Dim dummylist As DataTable
        If Not IsNothing(Session("dtScarico")) Then
            dummylist = Session("dtScarico")
            If dummylist.Rows.Count = dgrScarico.Rows.Count Then
                For i As Integer = 0 To dgrScarico.Rows.Count - 1
                    dummylist.Rows(i).Item("Udm_Selezionata") = CType(dgrScarico.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).SelectedValue
                    dummylist.Rows(i).Item("Valore") = CType(dgrScarico.Rows(i).FindControl("Txt_Qta_Ril"), TextBox).Text
                Next
            End If

            Session("dtScarico") = dummylist
        End If

        Crea_Griglia_CentriCosto()
        'Datatable_from_MovimentiCostiAccessori()
        AggiornaDgrScarico()
        Session("dtScarico_old") = Session("dtScarico")
    End Sub

    ''' <summary>
    ''' salvataggio dei costi accessori
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub SalvaCostiAccessori_Click(ByVal sender As Object, ByVal e As EventArgs) Handles SalvaCostiAccessori.Click

        SalvaCostiAccessori_SuAgendaMovimenti(False, dgrScarico)

    End Sub

    Public Sub SalvaCostiAccessori_SuAgendaMovimenti(ByVal LeggiDaGrigliaPopup_O_SoloDaTabellaInSessione As Boolean, ByVal Griglia As GridView)

        ' VAnni: 15/2/2017: lettura da griglia Kendo
        Dim jss As New JavaScriptSerializer
        Dim listaCostiAccessori As List(Of AgronicaCoreModello.CostiAccessori) = jss.Deserialize(Of List(Of AgronicaCoreModello.CostiAccessori))(hdKendo_CostiAccessori_Selezione.Value)

        If IsNothing(listaCostiAccessori) Then
            listaCostiAccessori = New List(Of AgronicaCoreModello.CostiAccessori)
        End If

        Dim DT As DataTable = Oggetti_DatatableUtility.CreateDataTable(Of AgronicaCoreModello.CostiAccessori)(listaCostiAccessori)

        Dim objCmbAttivita As DropDownList
        Dim objCmbTurni As DropDownList

        If LeggiDaGrigliaPopup_O_SoloDaTabellaInSessione Then
            For i = 0 To Griglia.Rows.Count - 1

                Dim app As String = CType(Griglia.Rows(i).FindControl("Txt_Qta_Ril"), TextBox).Text.Replace(".", ",")

                DT.Rows(i).Item("Qta_Ril") = If(IsNumeric(app), CDbl(app), 0)

                DT.Rows(i).Item("Udm_Cod") = CInt(CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).SelectedValue)

                DT.Rows(i).Item("Udm_Des") = CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).SelectedItem.Text

                objCmbAttivita = CType(Griglia.Rows(i).FindControl("Cmb_Attivita"), DropDownList)
                objCmbTurni = CType(Griglia.Rows(i).FindControl("Cmb_Turni"), DropDownList)

                If (Not IsNothing(objCmbAttivita) AndAlso Not IsNothing(objCmbTurni)) AndAlso
                   (objCmbAttivita.Visible AndAlso objCmbTurni.Visible) Then
                    DT.Rows(i).Item("Id_Attivita") = CType(Griglia.Rows(i).FindControl("Cmb_Attivita"), DropDownList).SelectedValue
                    DT.Rows(i).Item("Turno_Cod") = CType(Griglia.Rows(i).FindControl("Cmb_Turni"), DropDownList).SelectedValue
                Else
                    DT.Rows(i).Item("Id_Attivita") = 0
                    DT.Rows(i).Item("Turno_Cod") = 0
                End If

            Next

        End If


        Dim MovimentiCosti As List(Of Movimento) = New List(Of Movimento)

        Dim j As Integer = 0
        Dim Cau_Mov As Integer = 0
        Dim Sa_Cod As Integer = 0

        Dim DR() As DataRow

        '(15/02/2018) fede aggiunti terzisti
        'modificato salvataggio poichè ora terzisti e tecnici responsabili sono assieme alla manodopera a video ma mantengono movimenti separati
        '-----------------------------
        'MACCHINE
        DR = DT.Select("Centro_cod = " & -1)

        If DR IsNot Nothing AndAlso DR.Length > 0 Then

            Cau_Mov = CAU_IMPUTAZIONE_PARCOMACCHINE

            'verifico se ho l'acqua impostata 
            If QtaAcqua.Value <> "" Then
                'controllo se almeno una macchina ha la taratura ugelli
                Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
                For j = 0 To DR.Length - 1
                    Dim DT_Macc = objContab.MacchinaDes(objParametriAgenda.Piva,
                                                       objParametriAgenda.Data,
                                                       DR(j).Item("Mat_cod"),
                                                        "", "",
                                                       objParametri_Server)

                    If Not IsDBNull(DT_Macc.Rows(0).Item("Taratura_Ugello")) Then
                        Dim Acqua As Decimal = DT_Macc.Rows(0).Item("Taratura_Ugello")
                        QtaAcqua.Value = Acqua
                        Dim strJS As String = "$(document).ready(function () {AggiornaAcqua();}); "
                        ScriptManager.RegisterStartupScript(UpdatePanelPerScript, UpdatePanelPerScript.GetType(),
                                                 String.Format("jQuery_{0}", UpdatePanelPerScript.ClientID), strJS, True)

                        Exit For
                    End If
                Next
            End If

            'creo un nuovo movimento 
            Dim movimento As New Movimento
            movimento.Cau_Mov = Cau_Mov
            movimento.Cod_Risum = 0
            movimento.Data = objParametriAgenda.Data
            movimento.Id_Agenda = objParametriAgenda.Id_Agenda
            movimento.Lav_Cod = 0
            movimento.Mezzo = 0
            movimento.Piva = objParametriAgenda.Piva

            movimento.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

            For j = 0 To DR.Length - 1

                Dim movDet As New Movimento_Dettaglio
                'movDet.Cau_Mov = Cau_Mov
                movDet.Data = objParametriAgenda.Data
                movDet.Qta = CDbl(DR(j).Item("Qta_Ril"))
                movDet.Udm_Cod = DR(j).Item("Udm_Cod")
                movDet.Mat_Cod = DR(j).Item("Mat_cod")
                movDet.Elem_Cod = DR(j).Item("Elem_Cod")
                movDet.Pro_Cod = DR(j).Item("Pro_Cod") 'mancava
                movDet.Piva = objParametriAgenda.Piva
                movDet.Sa_Cod = Sa_Cod
                movDet.Extra_Int = DR(j).Item("Ore") * 60 + DR(j).Item("Minuti")
                movDet.Prezzo_Unitario = DR(j).Item("Costo_Unitario")
                movDet.Contabilizzato = NONCONTABILE

                movimento.Movimenti_Dettagli.Add(movDet)

            Next

            MovimentiCosti.Add(movimento)

        End If

        '-----------------------------
        'MANODOPERA
        'DR = DT.Select("Centro_cod = " & -2 & " AND (Cod_Rapporto=" & COD_LEGALE & " or Cod_Rapporto=" & COD_DIPENDENTE & ")")
        DR = DT.Select("Centro_cod = " & -2 & " AND (Cod_Rapporto <> " & COD_TERZISTA & " AND Cod_Rapporto <> " & COD_TECNICORESPONSABILE & ")")

        If DR IsNot Nothing AndAlso DR.Length > 0 Then

            Cau_Mov = CAU_IMPUTAZIONE_MANODOPERA

            'creo un nuovo movimento 
            Dim movimento As New Movimento
            movimento.Cau_Mov = Cau_Mov
            movimento.Cod_Risum = 0
            movimento.Data = objParametriAgenda.Data
            movimento.Id_Agenda = objParametriAgenda.Id_Agenda
            movimento.Lav_Cod = 0
            movimento.Mezzo = 0
            movimento.Piva = objParametriAgenda.Piva

            movimento.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

            For j = 0 To DR.Length - 1
                Dim movDet As New Movimento_Dettaglio
                movDet.Data = objParametriAgenda.Data
                movDet.Qta = CDbl(DR(j).Item("Qta_Ril"))
                movDet.Udm_Cod = DR(j).Item("Udm_Cod")
                movDet.Mat_Cod = DR(j).Item("Mat_cod")
                movDet.Elem_Cod = DR(j).Item("Elem_Cod")
                movDet.Pro_Cod = DR(j).Item("Pro_Cod") 'mancava
                movDet.Piva = objParametriAgenda.Piva
                movDet.Sa_Cod = Sa_Cod
                movDet.Extra_Int = DR(j).Item("Ore") * 60 + DR(j).Item("Minuti")
                movDet.Prezzo_Unitario = DR(j).Item("Costo_Unitario")

                movDet.Contabilizzato = NONCONTABILE

                movDet.ID_Attivita = DR(j).Item("Id_Attivita")
                movDet.Turno_Cod = DR(j).Item("Turno_Cod")

                If movDet.ID_Attivita <> 0 AndAlso movDet.Turno_Cod <> 0 Then

                    Dim objAttivita As New AgronicaCoreContabDAL.Attivita_R
                    movDet.Prezzo_Unitario = objAttivita.CostoOrario(movDet.ID_Attivita, movDet.Turno_Cod, movDet.Data, objParametri_Server)
                    objAttivita = Nothing

                    ' sono diversi solo se c'è uno sconto (usato per ora solo nelle fatture)
                    movDet.Prezzo_Unitario_Netto = movDet.Prezzo_Unitario

                End If

                movimento.Movimenti_Dettagli.Add(movDet)

            Next
            MovimentiCosti.Add(movimento)
        End If

        '-----------------------------
        'TERZISTI
        DR = DT.Select("Centro_cod = " & -2 & " AND Cod_Rapporto=" & COD_TERZISTA)

        If DR IsNot Nothing AndAlso DR.Length > 0 Then

            Cau_Mov = CAU_IMPUTAZIONE_TERZISTI

            'creo un nuovo movimento 
            Dim movimento As New Movimento
            movimento.Cau_Mov = Cau_Mov
            movimento.Cod_Risum = 0
            movimento.Data = objParametriAgenda.Data
            movimento.Id_Agenda = objParametriAgenda.Id_Agenda
            movimento.Lav_Cod = 0
            movimento.Mezzo = 0
            movimento.Piva = objParametriAgenda.Piva

            movimento.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)
            'creo tanti dettagli quanti sono gli item di dr()

            For j = 0 To DR.Length - 1
                Dim movDet As New Movimento_Dettaglio
                'movDet.Cau_Mov = Cau_Mov
                movDet.Data = objParametriAgenda.Data
                movDet.Qta = CDbl(DR(j).Item("Qta_Ril"))
                movDet.Udm_Cod = DR(j).Item("Udm_Cod")
                movDet.Mat_Cod = DR(j).Item("Mat_cod")
                movDet.Elem_Cod = DR(j).Item("Elem_Cod")
                movDet.Pro_Cod = DR(j).Item("Pro_Cod") 'mancava
                movDet.Piva = objParametriAgenda.Piva
                movDet.Sa_Cod = Sa_Cod
                movDet.Extra_Int = DR(j).Item("Ore") * 60 + DR(j).Item("Minuti")
                movDet.Prezzo_Unitario = DR(j).Item("Costo_Unitario")
                movDet.Contabilizzato = NONCONTABILE

                movimento.Movimenti_Dettagli.Add(movDet)

            Next
            MovimentiCosti.Add(movimento)
        End If

        '-----------------------------
        'TECNICO RESPONSABILE
        DR = DT.Select("Centro_cod = " & -2 & " AND Cod_Rapporto=" & COD_TECNICORESPONSABILE)

        If DR IsNot Nothing AndAlso DR.Length > 0 Then

            Cau_Mov = CAU_IMPUTAZIONE_TECNICO_RESPONSABILE

            'creo un nuovo movimento 
            Dim movimento As New Movimento
            movimento.Cau_Mov = Cau_Mov
            movimento.Cod_Risum = 0
            movimento.Data = objParametriAgenda.Data
            movimento.Id_Agenda = objParametriAgenda.Id_Agenda
            movimento.Lav_Cod = 0
            movimento.Mezzo = 0
            movimento.Piva = objParametriAgenda.Piva

            movimento.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)
            'creo tanti dettagli quanti sono gli item di dr()

            For j = 0 To DR.Length - 1
                Dim movDet As New Movimento_Dettaglio
                'movDet.Cau_Mov = Cau_Mov
                movDet.Data = objParametriAgenda.Data
                movDet.Qta = CDbl(DR(j).Item("Qta_Ril"))
                movDet.Udm_Cod = DR(j).Item("Udm_Cod")
                movDet.Mat_Cod = DR(j).Item("Mat_cod")
                movDet.Elem_Cod = DR(j).Item("Elem_Cod")
                movDet.Pro_Cod = DR(j).Item("Pro_Cod") 'mancava
                movDet.Piva = objParametriAgenda.Piva
                movDet.Sa_Cod = Sa_Cod
                movDet.Extra_Int = DR(j).Item("Ore") * 60 + DR(j).Item("Minuti")
                movDet.Prezzo_Unitario = DR(j).Item("Costo_Unitario")
                movDet.Contabilizzato = NONCONTABILE

                movimento.Movimenti_Dettagli.Add(movDet)

            Next
            MovimentiCosti.Add(movimento)
        End If

        '-----------------------------
        'SCARICO
        DR = DT.Select("Centro_cod not in (-1,-2,-3,-4)")

        If DR IsNot Nothing AndAlso DR.Length > 0 Then

            Cau_Mov = CAU_SCARICO

            'creo un nuovo movimento 
            Dim movimento As New Movimento
            movimento.Cau_Mov = Cau_Mov
            movimento.Cod_Risum = 0
            movimento.Data = objParametriAgenda.Data
            movimento.Id_Agenda = objParametriAgenda.Id_Agenda
            movimento.Lav_Cod = 0
            movimento.Mezzo = 0
            movimento.Piva = objParametriAgenda.Piva

            movimento.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)
            'creo tanti dettagli quanti sono gli item di dr()

            For j = 0 To DR.Length - 1
                Dim movDet As New Movimento_Dettaglio
                'movDet.Cau_Mov = Cau_Mov
                movDet.Data = objParametriAgenda.Data
                movDet.Qta = CDbl(DR(j).Item("Qta_Ril"))
                movDet.Udm_Cod = DR(j).Item("Udm_Cod")
                movDet.Mat_Cod = DR(j).Item("Mat_cod")
                movDet.Elem_Cod = DR(j).Item("Elem_Cod")
                movDet.Pro_Cod = DR(j).Item("Pro_Cod") 'mancava
                movDet.Piva = objParametriAgenda.Piva
                movDet.Sa_Cod = Sa_Cod
                movDet.Extra_Int = DR(j).Item("Ore") * 60 + DR(j).Item("Minuti")
                movDet.Prezzo_Unitario = DR(j).Item("Costo_Unitario")
                movDet.Contabilizzato = NONCONTABILE
                movDet.Sa_Cod = DR(j).Item("Sa_Cod")

                movDet.Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

                Dim movDest As New Movimento_Destinazione
                movDest.Id_Agenda = objParametriAgenda.Id_Agenda
                movDest.Sa_Cod = DR(j).Item("Sa_Cod")
                movDest.Piva = objParametriAgenda.Piva
                movDest.Qta = CDbl(DR(j).Item("Qta_Ril"))
                movDest.Tipo = enum_FabbricatiTipi.MagazzinoAziendale
                movDest.Id_Destinazione = DR(j).Item("Centro_Cod")
                movDet.Movimenti_Destinazioni.Add(movDest)
                movimento.Movimenti_Dettagli.Add(movDet)

            Next

            MovimentiCosti.Add(movimento)

        End If

        'devo fare un movimento solo per ogni causale (al max 4)
        'For i = -1 To -5 Step -1
        '    'filtro solo i movimenti di tipo ...
        '    Dim DR() As DataRow
        '    If i = -5 Then
        '        DR = DT.Select("Centro_cod not in (-1,-2,-3,-4)")
        '    Else
        '        DR = DT.Select("Centro_cod = " & i)
        '    End If

        '    If DR.Length > 0 Then

        '        'ho qualche movimento da inserire 
        '        Dim Cau_Mov As Integer = 0
        '        Dim Sa_Cod As Integer = 0   ' il sa_cod è <> 0 se ho uno scarico 

        '        Select Case i

        '            Case -1
        '                Cau_Mov = CAU_IMPUTAZIONE_PARCOMACCHINE
        '                'verifico se ho l'acqua impostata 
        '                If QtaAcqua.Value <> "" Then
        '                    'controllo se almeno una macchina ha la taratura ugelli
        '                    Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
        '                    For j = 0 To DR.Length - 1
        '                        Dim DT_Macc = objContab.MacchinaDes(objParametriAgenda.Piva,
        '                                               objParametriAgenda.Data,
        '                                               DR(j).Item("Mat_cod"),
        '                                                "", "",
        '                                               objParametri_Server)

        '                        If Not IsDBNull(DT_Macc.Rows(0).Item("Taratura_Ugello")) Then
        '                            Dim Acqua As Decimal
        '                            Acqua = DT_Macc.Rows(0).Item("Taratura_Ugello")
        '                            QtaAcqua.Value = Acqua
        '                            Dim strJS As String = "$(document).ready(function () {AggiornaAcqua();}); "
        '                            ScriptManager.RegisterStartupScript(UpdatePanelPerScript, UpdatePanelPerScript.GetType(),
        '                                         String.Format("jQuery_{0}", UpdatePanelPerScript.ClientID), strJS, True)

        '                            Exit For
        '                        End If
        '                    Next
        '                End If
        '            Case -2
        '                Cau_Mov = CAU_IMPUTAZIONE_MANODOPERA
        '            Case -3
        '                Cau_Mov = CAU_IMPUTAZIONE_TERZISTI
        '            Case -4
        '                Cau_Mov = CAU_IMPUTAZIONE_TECNICO_RESPONSABILE
        '            Case Else
        '                Cau_Mov = CAU_SCARICO
        '        End Select

        '        'creo un nuovo movimento 
        '        Dim movimento As New Movimento
        '        movimento.Cau_Mov = Cau_Mov
        '        movimento.Cod_Risum = 0
        '        movimento.Data = objParametriAgenda.Data
        '        movimento.Id_Agenda = objParametriAgenda.Id_Agenda
        '        movimento.Lav_Cod = 0
        '        movimento.Mezzo = 0
        '        movimento.Piva = objParametriAgenda.Piva

        '        movimento.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)
        '        'creo tanti dettagli quanti sono gli item di dr()

        '        For j = 0 To DR.Length - 1
        '            Dim movDet As New Movimento_Dettaglio
        '            'movDet.Cau_Mov = Cau_Mov
        '            movDet.Data = objParametriAgenda.Data

        '            Dim QTA As Decimal
        '            QTA = CDbl(DR(j).Item("Qta_Ril"))
        '            movDet.Qta = QTA

        '            movDet.Udm_Cod = DR(j).Item("Udm_Cod")
        '            movDet.Mat_Cod = DR(j).Item("Mat_cod")
        '            movDet.Elem_Cod = DR(j).Item("Elem_Cod")
        '            movDet.Pro_Cod = DR(j).Item("Pro_Cod") 'mancava
        '            movDet.Piva = objParametriAgenda.Piva
        '            movDet.Sa_Cod = Sa_Cod
        '            movDet.Extra_Int = DR(j).Item("Ore") * 60 + DR(j).Item("Minuti")
        '            movDet.Prezzo_Unitario = DR(j).Item("Costo_Unitario")

        '            movDet.Contabilizzato = NONCONTABILE

        '            If Cau_Mov = CAU_SCARICO Then
        '                ' movimento.Sa_Cod = DR(j).Item("Sa_Cod")
        '                movDet.Sa_Cod = DR(j).Item("Sa_Cod")

        '                movDet.Movimenti_Destinazioni = New List(Of Movimento_Destinazione)
        '                Dim movDest As New Movimento_Destinazione
        '                movDest.Id_Agenda = objParametriAgenda.Id_Agenda
        '                movDest.Sa_Cod = DR(j).Item("Sa_Cod")
        '                movDest.Piva = objParametriAgenda.Piva
        '                movDest.Qta = QTA
        '                movDest.Tipo = enum_FabbricatiTipi.MagazzinoAziendale
        '                movDest.Id_Destinazione = DR(j).Item("Centro_Cod")
        '                movDet.Movimenti_Destinazioni.Add(movDest)
        '            End If

        '            If Cau_Mov = CAU_IMPUTAZIONE_MANODOPERA Then

        '                movDet.ID_Attivita = DR(j).Item("Id_Attivita")
        '                movDet.Turno_Cod = DR(j).Item("Turno_Cod")

        '                If movDet.ID_Attivita <> 0 And movDet.Turno_Cod <> 0 Then

        '                    Dim objAttivita As New AgronicaCoreContabDAL.Attivita_R
        '                    movDet.Prezzo_Unitario = objAttivita.CostoOrario(movDet.ID_Attivita, movDet.Turno_Cod, movDet.Data, objParametri_Server)
        '                    objAttivita = Nothing

        '                    ' sono diversi solo se c'è uno sconto (usato per ora solo nelle fatture)
        '                    movDet.Prezzo_Unitario_Netto = movDet.Prezzo_Unitario
        '                    'movDet.Udm_Cod = 141        'Ore

        '                End If

        '            End If

        '            movimento.Movimenti_Dettagli.Add(movDet)

        '        Next
        '        MovimentiCosti.Add(movimento)
        '    End If
        'Next


        objParametriAgenda.Movimenti = MovimentiCosti

        Costruisci_DT_Scarico()     ' sia costi acc normali che avanzati
        Datatable_from_MovimentiCostiAccessori() ' sia costi acc normali che avanzati

        AggiornaGridViewCostiAccessoriVisibili()    ' sia costi acc normali che avanzati

        AggiornaMagazzino()

    End Sub

    ''' <summary>
    ''' Annullamento dei costi accessori
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub AnnullaCostiAccessori_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AnnullaCostiAccessori.Click
        Session("dtScarico") = Session("dtScarico_old")
        AggiornaGridViewCostiAccessoriVisibili()
    End Sub

    '##########################################################################################
    Private Sub Crea_Griglia_CentriCosto()

        'Aggiungo al datagrid dei centri di costo i magazzini
        Dim objDTableCentriCosto As DataTable
        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        objDTableCentriCosto = objFabbricati.LeggixCostiAccessori(objParametriAgenda.Piva, 0,
                                                                  objParametriAgenda.Data,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                   "", "", objParametri_Server)

        '  "1=2", "", objParametri_Server)

        objFabbricati = Nothing

        Dim Dr As DataRow

        'Aggiungo al datagrid la riga del parco macchine
        Dr = objDTableCentriCosto.NewRow
        Dr.Item(0) = Resources.AgronicaAgenda_2010.ParcoMacchine
        Dr.Item(1) = -1 'Fabbricato_Cod = -1 corrisponde al Parco Macchine
        Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
        objDTableCentriCosto.Rows.Add(Dr)


        'Aggiungo al datagrid la riga della manodopera
        Dr = objDTableCentriCosto.NewRow
        Dr.Item(0) = Resources.AgronicaAgenda_2010.Manodopera
        Dr.Item(1) = -2 'Fabbricato_Cod = -2 corrisponde alla Manodopera
        Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
        objDTableCentriCosto.Rows.Add(Dr)


        'Aggiungo al datagrid la riga del terzisti
        Dr = objDTableCentriCosto.NewRow
        Dr.Item(0) = Resources.AgronicaAgenda_2010.CTerzisti
        Dr.Item(1) = -3 'Fabbricato_Cod = -3 corrisponde ai C/Terzisti
        Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
        objDTableCentriCosto.Rows.Add(Dr)

        'Aggiungo al datagrid la tecnico responsabile
        Dr = objDTableCentriCosto.NewRow
        Dr.Item(0) = Resources.AgronicaAgenda_2010.TecnicoResponsabile
        Dr.Item(1) = -4 'Fabbricato_Cod = -3 corrisponde ai C/Terzisti
        Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
        objDTableCentriCosto.Rows.Add(Dr)

        'metto il DT all'interno del Session
        Session("DT_CentriCosto") = objDTableCentriCosto

        'Vettore di DataColumn
        Dim CdcKeys(1) As String

        'Valorizzo le celle del vettore
        CdcKeys(0) = "Sa_Cod"
        CdcKeys(1) = "Fabbricato_Cod"

        dgrCentriCosto.DataSource = objDTableCentriCosto
        dgrCentriCosto.DataKeyNames = CdcKeys
        dgrCentriCosto.DataBind()

        For i = 0 To dgrCentriCosto.Rows.Count - 1
            Dim tipo As String = objDTableCentriCosto.Rows(i).Item(0)
            Select Case tipo
                Case Resources.AgronicaAgenda_2010.ParcoMacchine
                    dgrCentriCosto.Rows(i).Cells(1).Controls(0).Visible = False
                Case Resources.AgronicaAgenda_2010.Manodopera

                Case Resources.AgronicaAgenda_2010.CTerzisti

                Case Resources.AgronicaAgenda_2010.TecnicoResponsabile

                Case Else
                    dgrCentriCosto.Rows(i).Cells(1).Controls(0).Visible = False
            End Select
        Next


    End Sub

    Private Sub dgrCentriCosto_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dgrCentriCosto.SelectedIndexChanged
        dgrCentriCosto_selezionato_tipo(True)
    End Sub


    Private Sub btnKendo_CostiAccessori_ComboHelper_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnKendo_CostiAccessori_ComboHelper.Click
        dgrCentriCosto_selezionato_tipo(False)
        hdKendo_CostiAccessori_ComboHelper_Gestione.Value = ""
    End Sub

    ''' <summary>
    ''' Riporta sul controllo nascosto un json con anagrafica costi, così come letto da funzioni precedenti
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="Centro_Cod"></param>
    Private Sub hdKendo_CostiAccessori_ComboHelper_popola(ByVal dt As DataTable, ByVal Centro_Cod As Integer)

        Dim lista As List(Of AgronicaCoreModello.Risorsa_keyValue) = (
            From ll In dt.AsEnumerable
            Select New AgronicaCoreModello.Risorsa_keyValue With {
                    .Risorsa_Cod =
                        Centro_Cod.ToString & "*" &
                        ll("Col_6") & "*" &
                        ll("Col_3") & "*" &
                        ll("Col_8"),
                    .Costo_Unitario = ll("Col_9"),
                    .Risorsa_Des = If(Centro_Cod = -1, String.Join(" - ", {ll("Col_0"), ll("Ditta_Des"), ll("Modello"), ll("Col_1")}.Where(Function(s) (Not IsDBNull(s) AndAlso Not String.IsNullOrEmpty(s)))), ll("Col_0") & " - " & ll("Col_1")),
                    .Udm_Cod = ll("Col_5"),
                    .Udm_Des = ll("Col_2"),
                    .Cod_Rapporto = ll("Col_11")
                }
            ).ToList
        '.Risorsa_Des = ll("Col_0") & " - " & ll("Col_1")
        '.Risorsa_Des = If(Centro_Cod <> -2, ll("Col_0") & " - " & ll("Col_1"), ll("Col_1"))


        Dim jss As New JavaScriptSerializer
        hdKendo_CostiAccessori_ComboHelper.Value = jss.Serialize(lista)



        If (Not String.IsNullOrEmpty(hdKendo_CostiAccessori_ComboHelper_Comando_Dati.Value)) Then

            Dim trova As String = hdKendo_CostiAccessori_ComboHelper_Comando_Dati.Value.Split(":")(1)
            hdKendo_CostiAccessori_ComboHelper_NuovoValore.Value = (
                From ll In lista
                Where ll.Risorsa_Cod = trova
                Select ll.Risorsa_Des
            ).FirstOrDefault()

            hdKendo_CostiAccessori_ComboHelper_Comando_Dati.Value = "Chiudi:" & trova

        End If


    End Sub

    Private Sub dgrCentriCosto_selezionato_tipo(ByVal BindData As Boolean)
        AggiornaCostiAccessori_Click(Me, Nothing)

        ' lo rendo visibile solo se ho scelto un magazzino (codice cdc>0)
        Filtro_Materiali.Visible = False


        ' svuoto la variabile nel viewstate
        ViewState("SaCodFabbricato") = ""

        Dim DT_CentriCosto As DataTable = Session("DT_CentriCosto")
        Dim Dt As DataTable

        Dim lFabbricatoCod As Integer
        If BindData Then
            lFabbricatoCod = CInt(DT_CentriCosto.Rows(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod"))
        Else
            If hdKendo_CostiAccessori_ComboHelper_Comando.Value = "" Then
                lFabbricatoCod = -1
            Else
                lFabbricatoCod = hdKendo_CostiAccessori_ComboHelper_Comando.Value
            End If

        End If

        Dim LeggiDaCache As Boolean = True
        If hdKendo_CostiAccessori_ComboHelper_Gestione.Value = "Ricarica" Then
            LeggiDaCache = False
        End If

        Select Case lFabbricatoCod

            Case Is > 0

                Filtro_Materiali.Visible = True

                'Dt = Session("Dt_Prodotti")
                Dt = Nothing
                Lbl_RisFiltro.Text = ""

                ViewState("SaCodFabbricato") = dgrCentriCosto.DataKeys(dgrCentriCosto.SelectedIndex).Item("Sa_Cod")

                'Session("Dt_Prodotti") = Dt

                If BindData Then
                    Me.dgrMateriali.DataSource = Dt
                    Me.dgrMateriali.DataBind()
                Else
                    hdKendo_CostiAccessori_ComboHelper_popola(Dt, lFabbricatoCod)
                End If
                ' commentato e aggiunto exit nicoletta 13/03/2014
                Exit Sub

                ''Cambio l'intestazione delle colonne nel datagrid
                'Me.dgrMateriali.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.CategoriaProdotto
                'Me.dgrMateriali.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Prodotto
                'Me.dgrMateriali.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.Giacenza

                'If Not IsNothing(Session("Dt_Prodotti")) Then
                '    Dt = Session("Dt_Prodotti")
                'Else
                '    Popola_ProdottiMagazzino(0, _
                '                             CInt(DT_CentriCosto.Rows(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod")), _
                '                             objParametriAgenda.Elem_Cod, _
                '                             False)
                '    Dt = Session("Dt_Prodotti")
                'End If

                'Me.dgrMateriali.DataSource = Dt
                'Me.dgrMateriali.DataBind()

            Case -1

                'Cambio l'intestazione delle colonne nel datagrid
                dgrMateriali.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.MacchinaAttrezzatura
                dgrMateriali.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Descrizione
                dgrMateriali.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

                If Not IsNothing(Session("Dt_Macchine")) AndAlso LeggiDaCache Then
                    Dt = Session("Dt_Macchine")
                Else
                    Popola_ParcoMacchine()
                    Dt = Session("Dt_Macchine")
                End If

                If BindData Then
                    Me.dgrMateriali.DataSource = Dt
                    Me.dgrMateriali.DataBind()
                Else
                    hdKendo_CostiAccessori_ComboHelper_popola(Dt, lFabbricatoCod)
                End If


            Case -2

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMateriali.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.RapportoContabile
                Me.dgrMateriali.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Manodopera
                Me.dgrMateriali.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

                If Not IsNothing(Session("Dt_Manodopera")) AndAlso LeggiDaCache Then
                    Dt = Session("Dt_Manodopera")
                Else
                    Popola_Manodopera()
                    Dt = Session("Dt_Manodopera")
                End If

                If BindData Then
                    Me.dgrMateriali.DataSource = Dt
                    Me.dgrMateriali.DataBind()
                Else
                    hdKendo_CostiAccessori_ComboHelper_popola(Dt, lFabbricatoCod)
                End If

            Case -3

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMateriali.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.RapportoContabile
                Me.dgrMateriali.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Terzista
                Me.dgrMateriali.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

                If Not IsNothing(Session("Dt_Terzisti")) AndAlso LeggiDaCache Then
                    Dt = Session("Dt_Terzisti")
                Else
                    Popola_Terzisti()
                    Dt = Session("Dt_Terzisti")
                End If

                If BindData Then
                    Me.dgrMateriali.DataSource = Dt
                    Me.dgrMateriali.DataBind()
                Else
                    hdKendo_CostiAccessori_ComboHelper_popola(Dt, lFabbricatoCod)
                End If



            Case -4

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMateriali.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.RapportoContabile
                Me.dgrMateriali.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.TecnicoResponsabile
                Me.dgrMateriali.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

                If Not IsNothing(Session("Dt_TecnicoResponsabile")) AndAlso LeggiDaCache Then
                    Dt = Session("Dt_TecnicoResponsabile")
                Else
                    Popola_TecnicoResponsabile()
                    Dt = Session("Dt_TecnicoResponsabile")
                End If

                If BindData Then

                    Me.dgrMateriali.DataSource = Dt
                    Me.dgrMateriali.DataBind()

                Else
                    hdKendo_CostiAccessori_ComboHelper_popola(Dt, lFabbricatoCod)
                End If

        End Select

        ControllaDDLCostiAccessori(dgrScarico)
    End Sub
    ''' <summary>
    ''' funzione per la creazione del DT da passare ai gridview dei costi accessori
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Costruisci_DT_Scarico()
        Dim dt As DataTable
        dt = New DataTable("dtScarico")
        dt.Columns.Add("kendoKey", System.Type.GetType("System.String"))
        dt.Columns.Add("Risorsa_Cod", System.Type.GetType("System.String"))
        dt.Columns.Add("Centro", System.Type.GetType("System.String"))
        dt.Columns.Add("Sa_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Centro_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Categoria_Des", System.Type.GetType("System.String"))
        dt.Columns.Add("Risorsa_Des", System.Type.GetType("System.String"))
        dt.Columns.Add("Udm_Des", System.Type.GetType("System.String"))
        dt.Columns.Add("Qta_Ril", System.Type.GetType("System.Decimal"))
        dt.Columns.Add("Udm_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Elem_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Riga", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Tipo_Centro", System.Type.GetType("System.String"))
        dt.Columns.Add("Pro_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Ditta_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Mat_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Costo_Unitario", System.Type.GetType("System.String"))
        dt.Columns.Add("Costo", System.Type.GetType("System.String"))
        dt.Columns.Add("Ore", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Minuti", System.Type.GetType("System.Int32"))


        'per gli input degli utenti
        dt.Columns.Add("Udm_Selezionata", System.Type.GetType("System.String"))
        dt.Columns.Add("Valore", System.Type.GetType("System.String"))
        dt.Columns.Add("ID_Attivita", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Turno_Cod", System.Type.GetType("System.Int32"))

        dt.Columns.Add("Qualifica_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Tariffa_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Cod_Rapporto", System.Type.GetType("System.Int32"))

        'Per griglia Kendo
        dt.Columns.Add("Attivita_Des", System.Type.GetType("System.String"))
        dt.Columns.Add("Turno_Des", System.Type.GetType("System.String"))

        Session("dtScarico") = dt
    End Sub


    Private Sub InserisciRiga_dtScarico(ByVal Centro As String,
                                        ByVal Sa_Cod As String,
                                        ByVal Centro_Cod As Int32,
                                        ByVal Categoria_Des As String,
                                        ByVal Risorsa_Des As String,
                                        ByVal Udm_Des As String,
                                        ByVal Qta_Ril As Decimal,
                                        ByVal Udm_Cod As Int32,
                                        ByVal Elem_Cod As Int32,
                                        ByVal Riga As Int32,
                                        ByVal Tipo_Centro As String,
                                        ByVal Pro_Cod As Int32,
                                        ByVal Ditta_Cod As Int32,
                                        ByVal Mat_Cod As Int32,
                                        ByVal Costo_Unitario As String,
                                        ByVal Costo As String,
                                        ByVal Id_Attivita As Int32,
                                        ByVal Attivita_Des As String,
                                        ByVal Turno_Cod As Int32,
                                        ByVal Turno_Des As String,
                                        ByVal Ore As Int32,
                                        ByVal Minuti As Int32,
                                        ByVal Qualifica_Cod As Int32,
                                        ByVal Tariffa_Cod As Int32,
                                        ByVal Cod_Rapporto As Int32,
                                        ByVal DT As DataTable)



        Dim Dr As DataRow
        Dim ElementoPresente As Boolean
        Dim Messaggio As String

        '----- Verifico che il formulato non sia gia' presente nel datatable

        'Inizializzo
        ElementoPresente = False

        'Recupero il datatable
        Dim IndiceRiga As Integer
        Dim RigaElemento As Integer = -1

        'Ciclo nelle righe del datatable
        For IndiceRiga = 0 To DT.Rows.Count - 1

            If DT.Rows(IndiceRiga).Item("Centro_Cod") = Centro_Cod And
               DT.Rows(IndiceRiga).Item("Elem_Cod") = Elem_Cod And
               DT.Rows(IndiceRiga).Item("Pro_Cod") = Pro_Cod And
               DT.Rows(IndiceRiga).Item("Mat_Cod") = Mat_Cod Then
                ElementoPresente = True
                RigaElemento = IndiceRiga
            End If

        Next

        ' se uso turni e attività permetto l'inserimento di più righe uguali
        If ElementoPresente AndAlso RigaElemento >= 0 Then
            If DT.Rows(RigaElemento).Item("Turno_Cod") <> 0 And DT.Rows(RigaElemento).Item("Id_Attivita") <> 0 Then
                ElementoPresente = False
            End If
        End If

        'Se esiste gia' allora esco
        If ElementoPresente = True Then
            Messaggio = "Non e' consentito inserire un elemento gia' presente"
            Messaggi.AgroMsgBox(Messaggio, Page, , UpdatePanelPerScript)
            Exit Sub
        End If

        '----- Inserisco il nuovo record

        'Creo una nuova riga
        Dr = DT.NewRow

        'Definisco i valori

        'Todo: _Tariffa_Cod andrà in chiave ....
        Dr.Item("kendoKey") =
            Udm_Cod.ToString & "*" &
            Id_Attivita.ToString & "*" &
            Turno_Cod.ToString & "*" &
            Centro_Cod & "*" &
            Elem_Cod.ToString & "*" &
            Pro_Cod.ToString & "*" &
            Mat_Cod.ToString


        Dr.Item("Risorsa_Cod") =
            Centro_Cod.ToString & "*" &
            Elem_Cod.ToString & "*" &
            Pro_Cod.ToString & "*" &
            Mat_Cod.ToString

        Risorsa_Des = Categoria_Des & " - " & Risorsa_Des

        Dr.Item("Centro") = Centro
        Dr.Item("Sa_Cod") = Sa_Cod
        Dr.Item("Centro_Cod") = Centro_Cod
        Dr.Item("Categoria_Des") = Categoria_Des
        Dr.Item("Risorsa_Des") = Risorsa_Des
        Dr.Item("Udm_Des") = Udm_Des
        Dr.Item("Qta_Ril") = Qta_Ril
        Dr.Item("Udm_Cod") = Udm_Cod
        Dr.Item("Elem_Cod") = Elem_Cod
        Dr.Item("Riga") = Riga
        Dr.Item("Tipo_Centro") = Tipo_Centro
        Dr.Item("Pro_Cod") = Pro_Cod
        Dr.Item("Ditta_Cod") = Ditta_Cod
        Dr.Item("Mat_Cod") = Mat_Cod
        Dr.Item("Costo_Unitario") = Costo_Unitario
        Dr.Item("Costo") = Costo
        Dr.Item("Id_Attivita") = Id_Attivita
        Dr.Item("Attivita_Des") = Attivita_Des
        Dr.Item("Turno_Cod") = Turno_Cod
        Dr.Item("Turno_Des") = Turno_Cod.ToString & "-" & Turno_Des
        Dr.Item("Ore") = Ore
        Dr.Item("Minuti") = Minuti

        Dr.Item("Qualifica_Cod") = Qualifica_Cod
        Dr.Item("Tariffa_Cod") = Tariffa_Cod
        Dr.Item("Cod_Rapporto") = Cod_Rapporto

        'Associo alla tabella la nuova riga creata
        DT.Rows.Add(Dr)


    End Sub

    Private Sub dgrScarico_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles dgrScarico.RowCommand

        AggiornaCostiAccessori_Click(Me, Nothing)

        Dim IndiceRigaGriglia As Integer = 0
        Dim Dr As DataRow

        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Select Case e.CommandName

            Case "EliminaCosto"

                If Session("dtScarico") IsNot Nothing Then

                    Dim dtScarico As DataTable = Session("dtScarico")

                    If dtScarico.Rows.Count >= IndiceRigaGriglia Then

                        'Trovo la riga da cancellare    (chiave = FrCod)
                        Dr = dtScarico.Rows(IndiceRigaGriglia)

                        'Elimino la riga
                        Dr.Delete()
                        'Salvo il DataTable dentro il Session
                        Session("dtScarico") = dtScarico

                    End If

                End If

        End Select


        AggiornaDgrScarico()
        AggiornaDgrScaricoAvanzati()
        AggiornaGridViewCostiAccessoriVisibili()

        'se sono nella griglia pupup dei costidrg scarico (come in questo caso) non è necessario aggiornare anche la lista dei costi
        'prersente nei movimenti dell'agenda, dato che viene ricreata quando si seleziona salva_costi_accessori,
        'mentre se si seleziona annulla allora viene ricaricata in sessione la tabella old quindi 
        'i movimenti non devono essere toccati
        'se invece sono nella griglia a fondo pagina gridviewcostiaccessorivisibili allora se seleziono cancella 
        'oltre che a modificare la tabella dei costi in sessione Session("dtScarico") che non verrà mai ripristinata
        'dalla versione precedente, dato che l'operazione non è annullabile o confermabile, devo 
        'agire anche sulla lista dei movimenti eliminando il movimento 
        'corrispondente alla riga selezionata. Se non lo faccio non ho più corrispondenza tra la tabella in sessione e 
        'la lista movimenti e dato che la prima corrisponde di solito a ciò che vedo, mentre la seconda 
        'corrisponde a ciò che viene salvato mi trovo a salvare cose diverse da quello che vedo
    End Sub

    Private Sub GridViewCostiAccessoriVisibili_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridViewCostiAccessoriVisibili.RowCommand

        AggiornaCostiAccessori_Click(Me, Nothing)

        Dim IndiceRigaGriglia As Integer = 0
        Dim Dr As DataRow

        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Select Case e.CommandName

            Case "EliminaCosto"

                If Session("dtScarico") IsNot Nothing Then

                    Dim dtScarico As DataTable = Session("dtScarico")

                    If dtScarico.Rows.Count >= IndiceRigaGriglia Then


                        'Trovo la riga da cancellare    (chiave = FrCod)
                        Dr = dtScarico.Rows(IndiceRigaGriglia)

                        'Elimino la riga
                        Dr.Delete()
                        'Salvo il DataTable dentro il Session
                        Session("dtScarico") = dtScarico

                    End If

                End If

        End Select


        AggiornaDgrScarico()
        AggiornaGridViewCostiAccessoriVisibili()
        AggiornaMagazzino()



        '----------------------------------------------------------------------
        '----------------------------------------------------------------------
        'Elimino dai movimenti
        SalvaCostiAccessori_SuAgendaMovimenti(False, dgrScarico)
        'se sono nella griglia pupup dei costidrg scarico non è necessario aggiornare anche la lista dei costi
        'prersente nei movimenti dell'agenda, dato che viene ricreata quando si seleziona salva_costi_accessori,
        'mentre se si seleziona annulla allora viene ricaricata in sessione la tabella old quindi 
        'i movimenti non devono essere toccati
        'se invece sono nella griglia a fondo pagina gridviewcostiaccessorivisibili  (come in questo caso) allora se seleziono cancella 
        'oltre che a modificare la tabella dei costi in sessione Session("dtScarico") che non verrà mai ripristinata
        'dalla versione precedente, dato che l'operazione non è annullabile o confermabile, devo 
        'agire anche sulla lista dei movimenti eliminando il movimento 
        'corrispondente alla riga selezionata. Se non lo faccio non ho più corrispondenza tra la tabella in sessione e 
        'la lista movimenti e dato che la prima corrisponde di solito a ciò che vedo, mentre la seconda 
        'corrisponde a ciò che viene salvato mi trovo a salvare cose diverse da quello che vedo
        '' '' '' ''Dim ElementiPresenti As Integer = 0
        '' '' '' ''Dim IdElementoDaEliminare As Integer = 0
        '' '' '' ''For i = 0 To objParametriAgenda.Movimenti.Count - 1

        '' '' '' ''    Dim Cau_Mov As String = objParametriAgenda.Movimenti(i).Cau_Mov

        '' '' '' ''    If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli) AndAlso _
        '' '' '' ''        objParametriAgenda.Movimenti(i).Movimenti_Dettagli.Count > 0 Then

        '' '' '' ''        For j = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli.Count - 1


        '' '' '' ''            Dim Elem_Cod As Integer = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod
        '' '' '' ''            Dim Pro_Cod As Integer = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod
        '' '' '' ''            Dim Mat_Cod As Integer = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod



        '' '' '' ''            If Cau_Mov = CAU_SCARICO Then
        '' '' '' ''                If objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count <> 0 Then
        '' '' '' ''                    Throw New Exception("per lo scarico ci deve essere un dettaglio destinazione")
        '' '' '' ''                End If
        '' '' '' ''                Dim Centro_Cod As Integer = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Id_Destinazione

        '' '' '' ''                If dtScarico.Rows(IndiceRigaGriglia).Item("Centro_Cod") = Centro_Cod And _
        '' '' '' ''                   dtScarico.Rows(IndiceRigaGriglia).Item("Elem_Cod") = Elem_Cod And _
        '' '' '' ''                   dtScarico.Rows(IndiceRigaGriglia).Item("Pro_Cod") = Pro_Cod And _
        '' '' '' ''                   dtScarico.Rows(IndiceRigaGriglia).Item("Mat_Cod") = Mat_Cod Then
        '' '' '' ''                    ElementiPresenti += 1
        '' '' '' ''                End If

        '' '' '' ''            Else

        '' '' '' ''                If dtScarico.Rows(IndiceRigaGriglia).Item("Centro_Cod") = -1 And _
        '' '' '' ''                   dtScarico.Rows(IndiceRigaGriglia).Item("Elem_Cod") = Elem_Cod And _
        '' '' '' ''                   dtScarico.Rows(IndiceRigaGriglia).Item("Pro_Cod") = Pro_Cod And _
        '' '' '' ''                  dtScarico.Rows(IndiceRigaGriglia).Item("Mat_Cod") = Mat_Cod Then
        '' '' '' ''                    ElementiPresenti += 1
        '' '' '' ''                End If

        '' '' '' ''            End If
        '' '' '' ''        Next
        '' '' '' ''    End If
        '' '' '' ''Next
        '' '' '' ''If ElementiPresenti = 0 Then
        '' '' '' ''    Throw New Exception("Non è stato trovato l'elemento da eliminare nella lista dei movimenti")
        '' '' '' ''End If
        '' '' '' ''If ElementiPresenti > 1 Then
        '' '' '' ''    Throw New Exception("Sono tati trovati " & ElementiPresenti & " elementi da eliminare nella lista dei movimenti")
        '' '' '' ''End If
        ' '' '' '' ''elimino elemento

        '----------------------------------------------------------------------
        '----------------------------------------------------------------------




    End Sub


    ''' <summary>
    ''' riempio il datatable dall oggetto
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Datatable_from_MovimentiCostiAccessori()

        Dim strUdmSimb As String = ""
        Dim strCategoria As String = ""

        Dim Piva As String = ""
        Dim Sa_Cod As Integer = 0

        Dim DT As DataTable

        Dim Centro As String = ""
        Dim Centro_Cod As Int32
        Dim Categoria_Des As String = ""
        Dim Risorsa_Des As String = ""
        Dim Udm_Des As String = ""
        Dim Qta_Ril As Decimal
        Dim Udm_Cod As Int32
        Dim Elem_Cod As Int32
        Dim Riga As Int32
        Dim Tipo_Centro As String = ""
        Dim Pro_Cod As Int32
        Dim Ditta_Cod As Int32
        Dim Mat_Cod As Int32
        Dim Costo_Unitario As String = Format(0, "0.00")
        Dim Costo As String = Format(0, "0.00")

        Dim Id_Attivita As Integer
        Dim Turno_Cod As Integer
        Dim Prezzo_Unitario As Decimal

        Dim Qualifica_Cod As Integer
        Dim Tariffa_Cod As Integer

        Dim Ore As Integer
        Dim Minuti As Integer

        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim dtScarico As DataTable = Session("dtScarico")

        If Not IsNothing(objParametriAgenda) AndAlso Not IsNothing(objParametriAgenda.Movimenti) AndAlso
            objParametriAgenda.Movimenti.Count > 0 Then

            For i = 0 To objParametriAgenda.Movimenti.Count - 1

                If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli) AndAlso
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli.Count > 0 Then

                    For j = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                        Piva = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Piva
                        Sa_Cod = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod

                        Elem_Cod = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod
                        Pro_Cod = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod
                        Mat_Cod = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod

                        Qta_Ril = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Qta
                        Ditta_Cod = 0
                        Udm_Cod = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod

                        Id_Attivita = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).ID_Attivita
                        Turno_Cod = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Turno_Cod
                        Prezzo_Unitario = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Prezzo_Unitario

                        ' per la profilazione
                        If objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int <> 0 Then
                            Ore = Math.Truncate(objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int / 60)
                            Minuti = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Int - (Ore * 60)
                        Else
                            Ore = 0
                            Minuti = 0
                        End If


                        Centro_Cod = 0

                        Riga = 0
                        Ditta_Cod = 0

                        Qualifica_Cod = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Qualifica_Cod
                        Tariffa_Cod = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Tariffa_Cod

                        Select Case Udm_Cod
                            Case TipiEnumerativi.enum_TipoMezzo.Ettaro
                                Udm_Des = "ha"
                                'Udm_Cod = "1"
                            Case TipiEnumerativi.enum_TipoMezzo.Ora
                                Udm_Des = "ora"
                                'Udm_Cod = "2"
                            Case TipiEnumerativi.enum_TipoMezzo.Indefinito
                                Udm_Des = "indefinito"
                                'Udm_Cod = "-1"
                        End Select

                        Dim Cod_Rapporto As Integer = 0

                        Select Case objParametriAgenda.Movimenti(i).Cau_Mov

                            Case CAU_SCARICO 'Scarico da un magazzino

                                If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) AndAlso
                                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count > 0 Then
                                    Centro_Cod = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Id_Destinazione
                                Else
                                    Centro_Cod = 0
                                End If

                                'Centro_Cod = objXml.XML_ReadInt(xmlMovimentoDettaglio.GetAttribute("id_destinazione"))
                                ' tag nico sostituito sa_cod
                                Dim objFabb As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                                Centro = objFabb.FabbricatoDes_from_FabbricatoCod(
                                                                                objParametriAgenda.Piva,
                                                                                Sa_Cod,
                                                                                Centro_Cod,
                                                                                objParametri_Server) 'Magazzino

                                Tipo_Centro = "M"

                                Dim objProdotto As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R

                                Risorsa_Des = objProdotto.Prodotto_Des(objParametriAgenda.Piva,
                                                                       Elem_Cod,
                                                                       Pro_Cod,
                                                                       Mat_Cod,
                                                                       False,
                                                                        strCategoria,
                                                                        objParametri_Server)

                                If Risorsa_Des IsNot DBNull.Value Then
                                    If CStr(Risorsa_Des) <> "" Then Categoria_Des = strCategoria
                                Else
                                    Categoria_Des = ""
                                End If

                                'serve??? o da errore???
                                Dim objConvert As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                                Udm_Des = objConvert.UdmDes_from_UdmCod(Udm_Cod, strUdmSimb, objParametri_Server)
                                objConvert = Nothing

                                'Dim objGiacenze As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                                'Dim DTGiacenze As DataTable

                                'DTGiacenze = objGiacenze.LeggiGiacenze(objParametriAgenda.Piva, _
                                '                                       objParametriAgenda.Sa_Cod, _
                                '                                       Elem_Cod, _
                                '                                       Pro_Cod, _
                                '                                       Mat_Cod, _
                                '                                       Udm_Cod, _
                                '                                       Centro_Cod, _
                                '                                       0, 0, 0, "", "", "", _
                                '                                       objParametri_Server)

                                'If DTGiacenze.Rows.Count > 0 Then
                                '    Costo_Unitario = Format(CDbl(DTGiacenze.Rows(0).Item("Prezzo_Unitario")), "0.00")
                                '    Costo = Format(CDbl(Costo_Unitario) * Qta_Ril, "0.00")
                                'End If

                                ' Nico: non leggo più dal record statico della giacenza, ma da prodotti costi

                                'cerco se c'è un prezzo con udm selezionata, mettendo udm come filtro per il mezzo
                                Dim objPrezzo As New AgronicaCoreContabDAL.Prodotti_Costi_R
                                Dim Mezzo As Integer = 0
                                Dim Prezzo As Decimal = 0

                                Prezzo = Prezzo_Unitario

                                If Prezzo = 0 Then
                                    Dim objAnCosti As New AgronicaCoreContabDAL.AnalisiCosti_R

                                    'il prezzo ricavato dalla media ponderata dei prezzi registrati nelle op. contabili
                                    Prezzo = objAnCosti.ValorizzazioneProdotto_New(1,
                                                                                objParametriAgenda.Piva,
                                                                                Elem_Cod,
                                                                                Pro_Cod,
                                                                                Mat_Cod,
                                                                                Udm_Cod,
                                                                                0,
                                                                                0,
                                                                                0,
                                                                                "",
                                                                                objParametriAgenda.Data.AddMonths(-6),
                                                                                objParametriAgenda.Data.AddMonths(6),
                                                                                objParametri_Server)
                                    objAnCosti = Nothing

                                    If Prezzo = 0 Then
                                        objPrezzo.Prezzo_from_Prodotto(objParametri_Server.PivaSuperUser,
                                                                  Elem_Cod,
                                                                  Pro_Cod,
                                                                  Mat_Cod,
                                                                  objParametriAgenda.Data,
                                                                  Mezzo,
                                                                  0,
                                                                  Prezzo,
                                                                  "Prodotti_Costi.Udm_Cod= " & Udm_Cod & " ", objParametri_Server)
                                    End If

                                End If

                                Costo_Unitario = Format(Prezzo, "0.00")
                                Costo = Format(Prezzo * Qta_Ril, "0.00")


                            Case CAU_IMPUTAZIONE_PARCOMACCHINE   'Parco Macchine

                                Dim Mezzo As Integer = 0
                                Dim Prezzo As Decimal = 0

                                Centro = "Parco Macchine"
                                Tipo_Centro = "PM"
                                Centro_Cod = -1



                                'cerco se c'è un prezzo con udm selezionata, mettendo udm come filtro per il mezzo
                                Dim objPrezzofromprodotti As New AgronicaCoreContabDAL.Prodotti_Costi_R
                                objPrezzofromprodotti.Prezzo_from_Prodotto(objParametri_Server.PivaSuperUser,
                                                     1,
                                                     Pro_Cod,
                                                     Mat_Cod,
                                                     objParametriAgenda.Data,
                                                     Mezzo,
                                                     0,
                                                     Prezzo,
                                                     "Prodotti_Costi.Mezzo= " & Udm_Cod & " ", objParametri_Server)


                                'se Udm_Cod diverso da -1 allora ho profilato il costo per quella udm
                                'ma dovrebbe essere uguale, comunque rimane 0
                                If Mezzo <> -1 Then

                                Else
                                    'il prezzo unitario non è impostato, metto a 0 e imposto udm e des con udm il mezzo è -1 (dovrebbero essere uguali)

                                    Prezzo = 0

                                End If


                                If Prezzo_Unitario <> 0 Then
                                    Prezzo = Prezzo_Unitario
                                End If

                                Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
                                'DT = objContab.MacchinaDes(objParametriAgenda.Piva, objParametriAgenda.Data, Mat_Cod, "", "", objParametri_Server)
                                DT = objContab.LeggiParcoMacchinexSuperUser(objParametriAgenda.Piva, Mat_Cod, 0, "", True,
                                                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                                                            "", "", objParametri_Server)

                                objContab = Nothing

                                If DT.Rows.Count > 0 Then

                                    Categoria_Des = DT.Rows(0).Item("Class_Desc")
                                    'Risorsa_Des = String.Join(" - ", {Categoria_Des, DT.Rows(0).Item("Ditta_Des"), DT.Rows(0).Item("Modello"), DT.Rows(0).Item("Mac_Des")}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                                    Risorsa_Des = String.Join(" - ", {DT.Rows(0).Item("Ditta_Des"), DT.Rows(0).Item("Modello"), DT.Rows(0).Item("Mac_Des")}.Where(Function(s) (Not IsDBNull(s) AndAlso Not String.IsNullOrEmpty(s))))
                                    'If DT.Rows(0).Item("Mac_Des") <> "" Then
                                    '    Risorsa_Des = DT.Rows(0).Item("Mac_Des")
                                    'Else
                                    '    Risorsa_Des = DT.Rows(0).Item("Ditta_Des") & " " & DT.Rows(0).Item("Modello")
                                    'End If
                                    Costo_Unitario = Format(Prezzo * 1, "0.00")   'Format(CDbl(objRS("Ammortamento").Value), "0.00")
                                    Costo = Format(Prezzo * Qta_Ril, "0.00")

                                End If


                                '(15/02/2018) fede 
                                'modificato poichè ora terzisti e tecnici responsabili sono assieme alla manodopera a video ma mantengono movimenti separati
                                'raggruppate in manodopera
                            Case CAU_IMPUTAZIONE_MANODOPERA, CAU_IMPUTAZIONE_TERZISTI, CAU_IMPUTAZIONE_TECNICO_RESPONSABILE

                                Dim Corrispettivo_Orario As Decimal = 0

                                Centro_Cod = -2
                                Centro = "Manodopera"
                                Tipo_Centro = "MD"

                                Dim Udm_Des_Contatto As String = ""
                                Dim objAnagrafe As New AgronicaCoreAnagrafeDAL.Contatti_R
                                objAnagrafe.Recupera_DatiContatto2(Mat_Cod, Categoria_Des, Risorsa_Des, Corrispettivo_Orario, Udm_Des_Contatto, 0, "", True, "", objParametri_Server, Cod_Rapporto)
                                objAnagrafe = Nothing

                                ' se uso i costi accessori avanzati, il costo è dato dal turno e dall'attività, non dal costo unitario della risorsa
                                If Id_Attivita <> 0 AndAlso Turno_Cod <> 0 Then
                                    Costo_Unitario = Format(Prezzo_Unitario, "0.00")
                                    Costo = Format(Prezzo_Unitario * Qta_Ril, "0.00")
                                Else
                                    'ho dovuto aggiungere il * 1 perché altrimenti settava a  "0.00"
                                    If Udm_Des = Udm_Des_Contatto Then
                                        Costo_Unitario = Format(Corrispettivo_Orario * 1, "0.00")
                                        Costo = Format(Corrispettivo_Orario * Qta_Ril, "0.00")
                                    Else
                                        Costo_Unitario = Format(Prezzo_Unitario * 1, "0.00")
                                        Costo = Format(Prezzo_Unitario * Qta_Ril, "0.00")
                                        'Costo_Unitario = Prezzo_Unitario
                                        'Costo = "0.00"
                                    End If
                                End If


                                'Case CAU_IMPUTAZIONE_TERZISTI

                                '    Dim Corrispettivo_Orario As Decimal = 0

                                '    Centro_Cod = -3
                                '    Centro = "C/Terzisti"
                                '    Tipo_Centro = "CT"

                                '    Dim Udm_Des_Contatto As String = ""
                                '    Dim objAnagrafe As New AgronicaCoreAnagrafeDAL.Contatti_R
                                '    objAnagrafe.Recupera_DatiContatto2(Mat_Cod, Categoria_Des, Risorsa_Des, Corrispettivo_Orario, Udm_Des_Contatto, 0, "", True, "", objParametri_Server, Cod_Rapporto)
                                '    'ho dovuto aggiungere il * 1 perchè altrimenti settava a  "0.00"
                                '    objAnagrafe = Nothing

                                '    If Udm_Des = Udm_Des_Contatto Then
                                '        Costo_Unitario = Format(Corrispettivo_Orario * 1, "0.00")
                                '        Costo = Format(Corrispettivo_Orario * Qta_Ril, "0.00")
                                '    Else
                                '        Costo_Unitario = "0.00"
                                '        Costo = "0.00"
                                '    End If

                                'Case CAU_IMPUTAZIONE_TECNICO_RESPONSABILE

                                '    Dim Corrispettivo_Orario As Decimal = 0

                                '    Centro_Cod = -4
                                '    Centro = "Tecnico Responsabile"
                                '    Tipo_Centro = "TR"

                                '    Dim Udm_Des_Contatto As String = ""
                                '    Dim objAnagrafe As New AgronicaCoreAnagrafeDAL.Contatti_R
                                '    objAnagrafe.Recupera_DatiContatto2(Mat_Cod, Categoria_Des, Risorsa_Des, Corrispettivo_Orario, Udm_Des_Contatto, 0, "", True, "", objParametri_Server, Cod_Rapporto)
                                '    'ho dovuto aggiungere il * 1 perchè altrimenti settava a  "0.00"
                                '    objAnagrafe = Nothing

                                '    If Udm_Des = Udm_Des_Contatto Then
                                '        Costo_Unitario = Format(Corrispettivo_Orario * 1, "0.00")
                                '        Costo = Format(Corrispettivo_Orario * Qta_Ril, "0.00")
                                '    Else
                                '        Costo_Unitario = "0.00"
                                '        Costo = "0.00"
                                '    End If

                        End Select

                        Dim Attivita_Des As String = ""

                        If Id_Attivita <> 0 Then
                            Dim Attivita As New AgronicaCoreContabDAL.Attivita_R
                            Dim dtAttivita As DataTable = Attivita.Leggi(Id_Attivita, "", "", objParametri_Server)

                            If dtAttivita.Rows.Count > 0 Then
                                Attivita_Des = dtAttivita(0)("Desc")
                            End If
                        End If

                        Dim Turno_Des As String = ""

                        If Turno_Cod <> 0 Then
                            Dim Turno As New AgronicaCoreContabDAL.Turni_R
                            Dim dtTurno As DataTable = Turno.Leggi(Turno_Cod, "", "", objParametri_Server)

                            If dtTurno.Rows.Count > 0 Then
                                Turno_Des = dtTurno(0)("Turno_Des")
                            End If
                        End If

                        InserisciRiga_dtScarico(Centro,
                                                Sa_Cod,
                                                Centro_Cod,
                                                Categoria_Des,
                                                Risorsa_Des,
                                                Udm_Des,
                                                Qta_Ril,
                                                Udm_Cod,
                                                Elem_Cod,
                                                Riga,
                                                Tipo_Centro,
                                                Pro_Cod,
                                                Ditta_Cod,
                                                Mat_Cod,
                                                Costo_Unitario,
                                                Costo,
                                                Id_Attivita,
                                                Attivita_Des,
                                                Turno_Cod,
                                                Turno_Des,
                                                Ore, Minuti,
                                                Qualifica_Cod,
                                                Tariffa_Cod,
                                                Cod_Rapporto,
                                                dtScarico)



                    Next

                End If

            Next

        End If

        '' VAnni: 14/2/2017: aggancio griglia kendo costi accessori..
        CaricaGriglia_CostiAccessori_xJSON(dtScarico)

        Session("dtScarico") = dtScarico
    End Sub


    '##########################################################################################

    ''' <summary>
    ''' inizializza datatable dei materiali
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function Costruisci_Dt_Materiali() As DataTable

        Dim Dt As New DataTable

        Dt.Columns.Add("Col_0", System.Type.GetType("System.String")) 'Categoria
        Dt.Columns.Add("Col_1", System.Type.GetType("System.String")) 'Descrizione
        Dt.Columns.Add("Col_2", System.Type.GetType("System.String")) 'Giacenza
        Dt.Columns.Add("Col_3", System.Type.GetType("System.Int32"))  'Pro_cod
        Dt.Columns.Add("Col_4", System.Type.GetType("System.Decimal")) 'Qta
        Dt.Columns.Add("Col_5", System.Type.GetType("System.Int32"))  'Udm_Cod
        Dt.Columns.Add("Col_6", System.Type.GetType("System.Int32"))  'Elem_Cod
        Dt.Columns.Add("Col_7", System.Type.GetType("System.Int32"))  'Ditta_Cod
        Dt.Columns.Add("Col_8", System.Type.GetType("System.Int32"))  'Mat_cod
        Dt.Columns.Add("Col_9", System.Type.GetType("System.String")) 'Prezzo_Unitario

        Dt.Columns.Add("Col_10", System.Type.GetType("System.Int32")) 'Qualifica_Cod
        Dt.Columns.Add("Col_11", System.Type.GetType("System.Int32")) 'Cod_Rapporto
        Dt.Columns.Add("Col_12", System.Type.GetType("System.String")) 'Lotto
        Dt.Columns.Add("Col_13", System.Type.GetType("System.String")) 'Codice Macchina

        Return Dt

    End Function

    Private Sub dgrMateriali_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles dgrMateriali.RowCommand


        AggiornaCostiAccessori_Click(Me, Nothing)



        Dim IndiceRigaGriglia As Integer = 0

        Dim Centro As String = ""
        Dim Centro_Cod As Int32
        Dim Categoria_Des As String = ""
        Dim Risorsa_Des As String = ""
        Dim Udm_Des As String = ""
        Dim Qta_Ril As Decimal
        Dim Udm_Cod As Int32
        Dim Elem_Cod As Int32
        Dim Riga As Int32
        Dim Tipo_Centro As String = ""
        Dim Pro_Cod As Int32
        Dim Ditta_Cod As Int32
        Dim Mat_Cod As Int32
        Dim Costo_Unitario As String = ""
        Dim Costo As String = ""
        Dim Sa_Cod As Integer

        Dim Cod_Rapporto As Integer = 0

        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Dim dtScarico As DataTable

        dtScarico = Session("dtScarico")

        Select Case e.CommandName

            Case "AggiungiCosto"

                If Session("DT_CentriCosto") IsNot Nothing Then

                    Dim DT_CentriCosto As DataTable = Session("DT_CentriCosto")

                    Select Case CInt(DT_CentriCosto.Rows(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod"))

                        Case Is > 0

                            If Not IsNothing(Session("Dt_Prodotti")) Then

                                Dim Dt_Prodotti As DataTable = Session("Dt_Prodotti")

                                If Dt_Prodotti.Rows.Count >= IndiceRigaGriglia Then

                                    Dim objFabb As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                                    Centro = objFabb.FabbricatoDes_from_FabbricatoCod(
                                                objParametriAgenda.Piva,
                                                dgrCentriCosto.DataKeys(dgrCentriCosto.SelectedIndex).Item("Sa_Cod"),
                                                dgrCentriCosto.DataKeys(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod"),
                                                objParametri_Server) 'Magazzino

                                    Tipo_Centro = "M"
                                    Centro_Cod = CInt(dgrCentriCosto.DataKeys(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod"))
                                    Sa_Cod = dgrCentriCosto.DataKeys(dgrCentriCosto.SelectedIndex).Item("Sa_Cod")

                                    Elem_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_6")
                                    Pro_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_8")

                                    Categoria_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Udm_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_7")
                                    Riga = 0 'IndiceRigaGriglia

                                End If

                            End If

                        Case -1

                            If Not IsNothing(Session("Dt_Macchine")) Then

                                Dim Dt_Macchine As DataTable = Session("Dt_Macchine")

                                If Dt_Macchine.Rows.Count >= IndiceRigaGriglia Then

                                    Centro = "Parco Macchine"
                                    Tipo_Centro = "PM"
                                    Centro_Cod = -1
                                    Sa_Cod = 0

                                    Categoria_Des = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Elem_Cod = MACCHINE
                                    Pro_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_8")

                                    Udm_Des = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_7")
                                    Riga = 0 'IndiceRigaGriglia

                                End If

                            End If

                        Case -2

                            If Not IsNothing(Session("Dt_Manodopera")) Then

                                Dim Dt_Manodopera As DataTable = Session("Dt_Manodopera")

                                If Dt_Manodopera.Rows.Count >= IndiceRigaGriglia Then

                                    Centro = "Manodopera"
                                    Tipo_Centro = "MD"
                                    Centro_Cod = -2
                                    Sa_Cod = 0

                                    Categoria_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Elem_Cod = 0
                                    Pro_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_8")

                                    Udm_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_7")

                                    Cod_Rapporto = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_11")
                                    Riga = 0 'IndiceRigaGriglia

                                End If

                            End If

                        Case -3 'TERZISTI


                            If Not IsNothing(Session("Dt_Terzisti")) Then

                                Dim Dt_Manodopera As DataTable = Session("Dt_Terzisti")

                                If Dt_Manodopera.Rows.Count >= IndiceRigaGriglia Then

                                    Centro_Cod = -3
                                    Centro = "C/Terzisti"
                                    Tipo_Centro = "CT"
                                    Sa_Cod = 0

                                    Categoria_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Elem_Cod = 0
                                    Pro_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_8")

                                    Udm_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_7")
                                    Riga = 0 'IndiceRigaGriglia

                                End If

                            End If



                        Case -4  'Tecnico Responsabile

                            If Not IsNothing(Session("Dt_TecnicoResponsabile")) Then

                                Dim Dt_Manodopera As DataTable = Session("Dt_TecnicoResponsabile")

                                If Dt_Manodopera.Rows.Count >= IndiceRigaGriglia Then

                                    Centro_Cod = -4
                                    Centro = "Tecnico Responsabile"
                                    Tipo_Centro = "TR"
                                    Sa_Cod = 0

                                    Categoria_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Elem_Cod = 0
                                    Pro_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_8")

                                    Udm_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_7")
                                    Riga = 0 'IndiceRigaGriglia

                                End If

                            End If

                    End Select

                    InserisciRiga_dtScarico(Centro,
                                            Sa_Cod,
                                            Centro_Cod,
                                            Categoria_Des,
                                            Risorsa_Des,
                                            Udm_Des,
                                            Qta_Ril,
                                            Udm_Cod,
                                            Elem_Cod,
                                            Riga,
                                            Tipo_Centro,
                                            Pro_Cod,
                                            Ditta_Cod,
                                            Mat_Cod,
                                            Costo_Unitario,
                                            Costo,
                                            0, "", 0, "",
                                            0, 0,
                                            0, 0,
                                             Cod_Rapporto,
                                            dtScarico)


                End If

                'InserisciCosto(CType(dgrMateriali.Rows(IndiceRigaGriglia).FindControl("Col_4"), TextBox), Nothing)

        End Select

        Session("dtScarico") = dtScarico
        AggiornaDgrScarico()
    End Sub

    Public Sub Popola_ProdottiMagazzino(ByVal Sa_Cod As Integer,
                                        ByVal Destinazione As Integer,
                                        ByVal Elem_Cod As Integer,
                                        ByVal Avanzati As Boolean)


        Dim NomeTabella As String = ""
        Dim NomeCodice As String = ""
        Dim NomeDescrizione As String = ""
        Dim CategoriaProdotto As String = ""

        Dim objDt As DataTable
        Dim DT As DataTable
        Dim DTGiacenze As DataTable
        Dim DTProdotti As DataTable
        Dim DR As DataRow

        Dim strUdm_Des As String = ""

        Dim txtFiltro As TextBox
        Dim LblRisultatoRicerca As Label

        If Avanzati Then
            txtFiltro = Txt_FiltroMaterialiAvanzati
            LblRisultatoRicerca = Lbl_RisFiltroAvanzati
        Else
            txtFiltro = Txt_FiltroMateriali
            LblRisultatoRicerca = Lbl_RisFiltro
        End If

        Try

            objDt = Costruisci_Dt_Materiali()

            Dim filtro As String = ""
            If Elem_Cod = 197 Then
                filtro = " elem_cod <> " & CStr(Elem_Cod) & " AND elem_cod <> 198"
            Else
                filtro = " elem_cod <> " & CStr(Elem_Cod) & " "
            End If

            Dim strFiltroDesc As String = " LIKE '%" & Agro_SQL_SaveText(txtFiltro.Text) & "%'"


            Dim objGiac As New AgronicaCoreContabDAL.Giacenze_R

            DTGiacenze = objGiac.SchedaGiacenzeMagazzino(objParametriAgenda.Data,
                                                       objParametriAgenda.Piva, Sa_Cod,
                                                       Destinazione, 0, 0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, True,
                                                       " AND CategorieMagazzino.Elem_Cod IN (2,200,205) ",
                                                       " AND Coad_Des " & strFiltroDesc,
                                                       " AND Car_Des " & strFiltroDesc,
                                                       " AND Fer_Des " & strFiltroDesc,
                                                       " AND Fr_Des " & strFiltroDesc,
                                                       " AND Av_Des_Vol " & strFiltroDesc,
                                                       " AND Ins_Des " & strFiltroDesc,
                                                       " AND Mat_Des " & strFiltroDesc,
                                                       " AND TRAP_DES " & strFiltroDesc,
                                                       " AND Mat_Des " & strFiltroDesc,
                                                       " AND Mat_Des " & strFiltroDesc,
                                                       "", objParametri_Server, objParametri_Utenti)



            Dim DrGiacenze As DataRow()
            ' DrGiacenze = DTGiacenze.Select("Descrizione_Prodotto LIKE '%" & Txt_FiltroMaterialiAvanzati.Text & "%'")

            DrGiacenze = DTGiacenze.Select("1=1")

            'Dim objGiacenze As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            'DTGiacenze = objGiacenze.LeggiGiacenze(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, 0, _
            '                                       0, 0, 0, Destinazione, 0, 0, 0, "", filtro, "", _
            '                                       objParametri_Server)
            'objGiacenze = Nothing

            If DrGiacenze.Length > 0 Then

                'Faccio un filtro per evitare di inserire l'elem_cod che ho già usato nella lavorazione
                Dim i As Integer = 0
                Dim j As Integer = 0

                For i = 0 To DrGiacenze.Length - 1

                    DR = objDt.NewRow

                    DR("Col_0") = DrGiacenze(i).Item("NomeComune")
                    DR("Col_1") = DrGiacenze(i).Item("Descrizione_Prodotto")  'DTProdotti.Rows(j).Item(NomeDescrizione)
                    'Dim objConvert As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                    'DR("Col_2") = objConvert.UdmDes_from_UdmCod(DTGiacenze.Rows(i).Item("Udm_Cod"), _
                    '                                            strUdm_Des, objParametri_Server) + " " + _
                    '                                        Format(DTGiacenze.Rows(i).Item("Qta"), "0.00")
                    'objConvert = Nothing
                    DR("Col_2") = DrGiacenze(i).Item("Udm_Des") & " " & Format(DrGiacenze(i).Item("Giacenza"), "0.00")
                    DR("Col_3") = DrGiacenze(i).Item("Pro_Cod")
                    DR("Col_4") = 0
                    DR("Col_5") = DrGiacenze(i).Item("Udm_Cod")
                    DR("Col_6") = DrGiacenze(i).Item("Elem_Cod")
                    DR("Col_7") = 0 'Ditta_Cod
                    DR("Col_8") = DrGiacenze(i).Item("Mat_Cod")
                    DR("Col_9") = 0         ' nicoletta   Format(CDbl(DTGiacenze.Rows(i).Item("Prezzo_Unitario")), "0.00")

                    DR("Col_11") = 0

                    Dim Mezzo As Integer = 0
                    Dim Udm As Integer = DrGiacenze(i).Item("Udm_Cod")
                    Dim Prezzo_Unitario As Decimal = 0

                    Dim objAnCosti As New AgronicaCoreContabDAL.AnalisiCosti_R

                    'il prezzo ricavato dalla media ponderata dei prezzi registrati nelle op. contabili
                    Prezzo_Unitario = objAnCosti.ValorizzazioneProdotto_New(1,
                                                                        objParametriAgenda.Piva,
                                                                        DrGiacenze(i).Item("Elem_Cod"),
                                                                        DrGiacenze(i).Item("Pro_Cod"),
                                                                        DrGiacenze(i).Item("Mat_Cod"),
                                                                        DrGiacenze(i).Item("Udm_Cod"),
                                                                        0,
                                                                        0,
                                                                        0,
                                                                        "",
                                                                        objParametriAgenda.Data.AddMonths(-6),
                                                                        objParametriAgenda.Data.AddMonths(6),
                                                                        objParametri_Server)
                    objAnCosti = Nothing

                    ' se non ci sono carichi o fatture leggo il prodotto da prodotti_costi
                    If Prezzo_Unitario = 0 Then
                        Dim objProdottiPrezzi As New AgronicaCoreContabDAL.Prodotti_Costi_R
                        ' il primo parametro che è la piva non viene usata
                        objProdottiPrezzi.Prezzo_from_Prodotto(objParametri_Server.PivaSuperUser,
                                             DrGiacenze(i).Item("Elem_Cod"),
                                             DrGiacenze(i).Item("Pro_Cod"),
                                             DrGiacenze(i).Item("Mat_Cod"),
                                             objParametriAgenda.Data,
                                             Mezzo,
                                             Udm,
                                             Prezzo_Unitario,
                                             "", objParametri_Server)

                        objProdottiPrezzi = Nothing
                    End If

                    DR("Col_9") = Prezzo_Unitario

                    ''Leggo i prodotti associate al profilo selezionato			
                    'Dim objCategorieMag As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
                    'DT = objCategorieMag.Leggi(DTGiacenze.Rows(i).Item("Elem_Cod"), 0, False, _
                    '                    "", _
                    '                    "", _
                    '                     objParametri_Server)
                    'objCategorieMag = Nothing

                    'If DT.Rows.Count > 0 Then
                    '    NomeTabella = DT.Rows(0).Item("Tabella")
                    '    NomeCodice = DT.Rows(0).Item("Tabella_Cod")
                    '    NomeDescrizione = DT.Rows(0).Item("Tabella_Des")
                    '    CategoriaProdotto = DT.Rows(0).Item("NomeComune")
                    'End If

                    'Select Case CLng(DTGiacenze.Rows(i).Item("pro_cod"))

                    '    Case Is <> 0

                    '        Dim objProdotti As New AgronicaCoreContabDAL.Prodotti_R
                    '        DTProdotti = objProdotti.Leggi(CStr(NomeTabella), _
                    '                                        CStr(NomeCodice), _
                    '                                        CInt(DTGiacenze.Rows(i).Item("Pro_Cod")), _
                    '                                         AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                    '                                         "", _
                    '                                         "", _
                    '                                         objParametri_Server)
                    '        objProdotti = Nothing

                    '        For j = 0 To DTProdotti.Rows.Count - 1
                    '            DR("Col_0") = CategoriaProdotto
                    '            DR("Col_1") = DTProdotti.Rows(j).Item(NomeDescrizione)
                    '            Dim objConvert As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                    '            DR("Col_2") = objConvert.UdmDes_from_UdmCod(DTGiacenze.Rows(i).Item("Udm_Cod"), _
                    '                                                        strUdm_Des, objParametri_Server) + " " + _
                    '                                                    Format(DTGiacenze.Rows(i).Item("Qta"), "0.00")
                    '            objConvert = Nothing
                    '            DR("Col_3") = DTGiacenze.Rows(i).Item("Pro_Cod")
                    '            DR("Col_4") = 0
                    '            DR("Col_5") = DTGiacenze.Rows(i).Item("Udm_Cod")
                    '            DR("Col_6") = DTGiacenze.Rows(i).Item("Elem_Cod")
                    '            DR("Col_7") = 0 'Ditta_Cod
                    '            DR("Col_8") = 0 'Mat_Cod
                    '            DR("Col_9") = Format(CDbl(DTGiacenze.Rows(i).Item("Prezzo_Unitario")), "0.00")
                    '        Next


                    '    Case 0

                    '        'Leggo le materie prime
                    '        Dim objProdottiAziendali As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                    '        DTProdotti = objProdottiAziendali.LeggiMateriePrimexSuperUser( _
                    '                             CStr(objParametri_Server.PivaSuperUser), _
                    '                             objParametriAgenda.Piva, _
                    '                             DTGiacenze.Rows(i).Item("Elem_Cod"), _
                    '                             DTGiacenze.Rows(i).Item("Mat_Cod"), _
                    '                             0, _
                    '                             "", _
                    '                             False, _
                    '                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, _
                    '                             "", "", _
                    '                             objParametri_Server)
                    '        objProdottiAziendali = Nothing

                    '        For j = 0 To DTProdotti.Rows.Count - 1
                    '            DR("Col_0") = CategoriaProdotto
                    '            DR("Col_1") = DTProdotti.Rows(j).Item("Mat_Des")
                    '            Dim objConvert As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                    '            DR("Col_2") = objConvert.UdmDes_from_UdmCod(DTGiacenze.Rows(i).Item("Udm_Cod"), _
                    '                                                        strUdm_Des, objParametri_Server) + " " + _
                    '                                                    Format(DTGiacenze.Rows(i).Item("Qta"), "0.00")
                    '            objConvert = Nothing
                    '            DR("Col_3") = 0 'Pro_Cod
                    '            DR("Col_4") = 0
                    '            DR("Col_5") = DTGiacenze.Rows(i).Item("Udm_Cod")
                    '            DR("Col_6") = DTGiacenze.Rows(i).Item("Elem_Cod")
                    '            DR("Col_7") = 0 'Ditta_Cod
                    '            DR("Col_8") = DTProdotti.Rows(j).Item("Mat_Cod")
                    '            DR("Col_9") = Format(CDbl(DTGiacenze.Rows(i).Item("Prezzo_Unitario")), "0.00")

                    '        Next

                    'End Select

                    objDt.Rows.Add(DR)

                Next

                If DrGiacenze.Length = 1 Then
                    LblRisultatoRicerca.Text = "Trovato n." & DrGiacenze.Length.ToString & " prodotto"
                Else
                    LblRisultatoRicerca.Text = "Trovati n." & DrGiacenze.Length.ToString & " prodotti"
                End If


            Else

                LblRisultatoRicerca.Text = "Trovati n.0 prodotti"

            End If

            Session("Dt_Prodotti") = objDt

        Catch ex As Exception
            Session("Dt_Prodotti") = Nothing
        End Try

    End Sub

    Private Sub Popola_ParcoMacchine()

        Dim i As Integer = 0
        Dim Mezzo As Integer = 0
        Dim Prezzo_Unitario As Decimal
        Dim Udm_Cod As Integer = 0
        Dim objDTableParcoMacchine As DataTable
        Dim Dr As DataRow

        Try

            objDTableParcoMacchine = Costruisci_Dt_Materiali()
            objDTableParcoMacchine.Columns.Add("Ditta_Des", Type.GetType("System.String"))
            objDTableParcoMacchine.Columns.Add("Modello", Type.GetType("System.String"))

            'Preparo la query per recuperare le macchine del centro aziendale
            'Sa_Cod=-1 : Macchine di un altro centro a disposizione di tutti

            Dim objPM As New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim Dt_Macchine As DataTable = objPM.LeggiMacchine_xCostiAccessori(objParametriAgenda.Piva,
                                                                           objParametriAgenda.Data,
                                                                           "", "", objParametri_Server,
                                                                           visualizzaMacchineImportate:=True)
            objPM = Nothing



            For i = 0 To Dt_Macchine.Rows.Count - 1

                Dr = objDTableParcoMacchine.NewRow()

                Dr.Item("Col_0") = Dt_Macchine.Rows(i).Item("Col_0")
                Dr.Item("Col_1") = Dt_Macchine.Rows(i).Item("Col_1")
                If Dt_Macchine.Rows(i).Item("Col_1") = "" Then
                    Dr.Item("Col_1") = "Targa: " & Dt_Macchine.Rows(i).Item("Targa")
                End If
                Dr.Item("Col_8") = Dt_Macchine.Rows(i).Item("Col_8")

                Dim objProdottiPrezzi As New AgronicaCoreContabDAL.Prodotti_Costi_R
                objProdottiPrezzi.Prezzo_from_Prodotto(Session("ASG_SuperUser_CodFiscale"),
                                     MACCHINE,
                                     0,
                                     Dr.Item("Col_8"),
                                     objParametriAgenda.Data,
                                     Mezzo,
                                     Udm_Cod,
                                     Prezzo_Unitario,
                                     "", objParametri_Server)

                objProdottiPrezzi = Nothing


                If IsDBNull(Mezzo) Then
                    Dr.Item("Col_2") = "indefinito"
                    Dr.Item("Col_5") = -1
                Else
                    Select Case Mezzo
                        Case TipiEnumerativi.enum_TipoMezzo.Ettaro
                            Dr.Item("Col_2") = "ha"
                            Dr.Item("Col_5") = "1"
                        Case TipiEnumerativi.enum_TipoMezzo.Ora
                            Dr.Item("Col_2") = "ora"
                            Dr.Item("Col_5") = "2"
                        Case TipiEnumerativi.enum_TipoMezzo.Indefinito
                            Dr.Item("Col_2") = "indefinito"
                            Dr.Item("Col_5") = "-1"
                    End Select
                End If




                Dr.Item("Col_9") = Prezzo_Unitario

                Dr.Item("Col_3") = 0
                Dr.Item("Col_4") = 0
                Dr.Item("Col_6") = MACCHINE
                Dr.Item("Col_7") = 0

                Dr.Item("Col_11") = 0

                Dr.Item("Ditta_Des") = Dt_Macchine.Rows(i).Item("Ditta_Des")
                Dr.Item("Modello") = Dt_Macchine.Rows(i).Item("Modello")

                objDTableParcoMacchine.Rows.Add(Dr)

            Next

            Dt_Macchine = Nothing

            'Me.dgrMateriali.DataSource = objDTableParcoMacchine
            'Me.dgrMateriali.DataBind()

            Session("Dt_Macchine") = objDTableParcoMacchine



        Catch ex As Exception

            Session("Dt_Macchine") = Nothing
            'MsgBox("Impossibile caricare i centri di costo. " + ex.Message, MsgBoxStyle.Exclamation, "GiasOnline")

        End Try


    End Sub

    '(12/02/2018 fede) aggiunti terzisti
    Private Sub Popola_Manodopera()
        'Popola la griglia della "Manodpera"


        Dim i As Integer = 0
        Dim objDTableManodopera As DataTable
        Dim Dr As DataRow

        Try

            objDTableManodopera = Costruisci_Dt_Materiali()

            'Preparo la query per recuperare tutti i dipendenti(manodopera)
            'Sa_Cod=-1 : manodopera di un altro centro a disposizione di tutti
            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R

            'Dim Str As String = " ( (Rapporti_Contabili.Cod_Rapporto in(-1,-4,-6))  or Rapporti_Contabili.Dipendente=1 ) "
            Dim Str As String = " (( Rapporti_Contabili.Cod_Rapporto in(-1,-4,-6, -5) ) or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Legale=1 or Rapporti_Contabili.Terzista=1) " & vbCrLf &
                                " AND Risorse_Umane.Validita_Inizio <= " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDate(objParametriAgenda.Data) & " AND Risorse_Umane.Validita_Fine >= " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDate(objParametriAgenda.Data) & " "

            Dim Dt_Manodopera As DataTable = objRapp_Contabili.RapportiContabilixCostiAccessori(objParametriAgenda.Piva,
                                                                                     False,
                                                                                     False,
                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                      Str, " Contatti.Sa_Cod desc, Contatti.Rag_Soc, cognome, nome ASC  ", objParametri_Server)

            objRapp_Contabili = Nothing

            For i = 0 To Dt_Manodopera.Rows.Count - 1

                Dr = objDTableManodopera.NewRow()

                Dr.Item("Col_0") = CStr(Dt_Manodopera.Rows(i).Item("Rapporto_Des"))

                'Se c'è aggiungo anche il progressivo
                If Not IsDBNull(Dt_Manodopera.Rows(i).Item("Settore_Des")) AndAlso CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")).Trim() <> "" Then
                    Dr.Item("Col_0") &= " (Progressivo: " & CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")).Trim() & ")"
                End If

                Dr.Item("Col_1") = Dt_Manodopera.Rows(i).Item("Col_1")
                Dr.Item("Col_8") = Dt_Manodopera.Rows(i).Item("Col_8")
                Dr.Item("Col_9") = Dt_Manodopera.Rows(i).Item("Col_9")
                Dr.Item("Col_5") = Dt_Manodopera.Rows(i).Item("Col_5")

                If Dr.Item("Col_5") = 2 Then
                    Dr.Item("Col_2") = "ora"
                Else
                    Dr.Item("Col_2") = "ha"
                End If

                Dr.Item("Col_3") = 0
                Dr.Item("Col_4") = 0
                Dr.Item("Col_6") = 0
                Dr.Item("Col_7") = 0

                Dr.Item("Col_10") = 0
                If Not IsDBNull(Dt_Manodopera.Rows(i).Item("Qualifica_Cod")) AndAlso IsNumeric(Dt_Manodopera.Rows(i).Item("Qualifica_Cod")) Then
                    Dr.Item("Col_10") = Dt_Manodopera.Rows(i).Item("Qualifica_Cod")
                End If

                Dr.Item("Col_11") = Dt_Manodopera.Rows(i).Item("Cod_Rapporto")

                objDTableManodopera.Rows.Add(Dr)

            Next

            Dt_Manodopera = Nothing

            'Me.dgrMateriali.DataSource = objDTableManodopera
            'Me.dgrMateriali.DataBind()
            'Format(Me.dgrMateriali.Columns.Item(9), "0.00")

            Session("Dt_Manodopera") = objDTableManodopera



        Catch ex As Exception

            Session("Dt_Manodopera") = Nothing
            'MsgBox("Impossibile caricare i centri di costo. " + ex.Message, MsgBoxStyle.Exclamation, "GiasOnline")

        End Try


    End Sub

    Private Sub Popola_Terzisti()


        Dim i As Integer = 0
        Dim objDTableManodopera As DataTable
        Dim Dr As DataRow

        Try

            objDTableManodopera = Costruisci_Dt_Materiali()

            'Preparo la query per recuperare tutti i dipendenti(manodopera)
            'Sa_Cod=-1 : manodopera di un altro centro a disposizione di tutti

            'Risorse_Umane.Settore_Des as Col_2

            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
            Dim Dt_Manodopera As DataTable
            Dim Str As String = ""
            Str = " ( (Rapporti_Contabili.Cod_Rapporto = -5)  or Rapporti_Contabili.Terzista=1 ) "
            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessori(objParametriAgenda.Piva,
                                                                                     False,
                                                                                     False,
                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                      Str, " Contatti.Sa_Cod desc, Contatti.Rag_Soc, cognome, nome ASC  ", objParametri_Server)
            objRapp_Contabili = Nothing



            For i = 0 To Dt_Manodopera.Rows.Count - 1

                Dr = objDTableManodopera.NewRow()

                Dr.Item("Col_0") = CStr(Dt_Manodopera.Rows(i).Item("Rapporto_Des")) &
                                  "  (Progressivo: " & CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")) & ")"

                Dr.Item("Col_1") = Dt_Manodopera.Rows(i).Item("Col_1")
                Dr.Item("Col_8") = Dt_Manodopera.Rows(i).Item("Col_8")
                Dr.Item("Col_9") = Dt_Manodopera.Rows(i).Item("Col_9")
                Dr.Item("Col_5") = Dt_Manodopera.Rows(i).Item("Col_5")

                If Dr.Item("Col_5") = 2 Then
                    Dr.Item("Col_2") = "ora"
                Else
                    Dr.Item("Col_2") = "ha"
                End If

                Dr.Item("Col_3") = 0
                Dr.Item("Col_4") = 0
                Dr.Item("Col_6") = 0
                Dr.Item("Col_7") = 0

                Dr.Item("Col_11") = 0

                objDTableManodopera.Rows.Add(Dr)

            Next

            Dt_Manodopera = Nothing

            'Me.dgrMateriali.DataSource = objDTableManodopera
            'Me.dgrMateriali.DataBind()

            Session("Dt_Terzisti") = objDTableManodopera


        Catch ex As Exception

            Session("Dt_Terzisti") = Nothing

            'MsgBox("Impossibile caricare i centri di costo. " + ex.Message, MsgBoxStyle.Exclamation, "GiasOnline")

        End Try


    End Sub

    Private Sub Popola_TecnicoResponsabile()


        Dim i As Integer = 0
        Dim objDTableManodopera As DataTable
        Dim Dr As DataRow

        Try

            objDTableManodopera = Costruisci_Dt_Materiali()

            'Preparo la query per recuperare tutti i dipendenti(manodopera)
            'Sa_Cod=-1 : manodopera di un altro centro a disposizione di tutti

            'Risorse_Umane.Settore_Des as Col_2

            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
            Dim Dt_Manodopera As DataTable
            Dim Str As String = ""
            Str = " (Rapporti_Contabili.Cod_Rapporto = -12) "
            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessori(objParametriAgenda.Piva,
                                                                                     False,
                                                                                     False,
                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                      Str, " Contatti.Sa_Cod desc, Contatti.Rag_Soc, cognome, nome ASC  ", objParametri_Server)
            objRapp_Contabili = Nothing



            For i = 0 To Dt_Manodopera.Rows.Count - 1

                Dr = objDTableManodopera.NewRow()

                Dr.Item("Col_0") = CStr(Dt_Manodopera.Rows(i).Item("Rapporto_Des"))

                'Se c'è aggiungo anche il progressivo
                If Not IsDBNull(Dt_Manodopera.Rows(i).Item("Settore_Des")) AndAlso CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")).Trim() <> "" Then
                    Dr.Item("Col_0") &= " (Progressivo: " & CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")).Trim() & ")"
                End If

                Dr.Item("Col_1") = Dt_Manodopera.Rows(i).Item("Col_1")
                Dr.Item("Col_8") = Dt_Manodopera.Rows(i).Item("Col_8")
                Dr.Item("Col_9") = Dt_Manodopera.Rows(i).Item("Col_9")
                Dr.Item("Col_5") = Dt_Manodopera.Rows(i).Item("Col_5")

                If Dr.Item("Col_5") = 2 Then
                    Dr.Item("Col_2") = "ora"
                Else
                    Dr.Item("Col_2") = "ha"
                End If

                Dr.Item("Col_3") = 0
                Dr.Item("Col_4") = 0
                Dr.Item("Col_6") = 0
                Dr.Item("Col_7") = 0

                Dr.Item("Col_11") = 0

                objDTableManodopera.Rows.Add(Dr)

            Next

            Dt_Manodopera = Nothing

            'Me.dgrMateriali.DataSource = objDTableManodopera
            'Me.dgrMateriali.DataBind()

            Session("Dt_TecnicoResponsabile") = objDTableManodopera


        Catch ex As Exception

            Session("Dt_TecnicoResponsabile") = Nothing

            'MsgBox("Impossibile caricare i centri di costo. " + ex.Message, MsgBoxStyle.Exclamation, "GiasOnline")

        End Try


    End Sub
#End Region

#Region "x Ordinamenti GridView"

    Private Sub ScriptGridImpianti()
        Dim script As New StringBuilder


        script.AppendLine("$(document).ready(function () { ")


        script.AppendLine(" setTimeout(function () { Abilita_Disabilita_Resto(); }, 100);  ")

        script.AppendLine("     AbilitaDisabilita_SupTrattata();")

        Select Case objParametriAgenda.Lav_Cod

            Case Is <> LAVCOD_TRATTAMENTO_POST_RACCOLTA

                If SupTrattata = True Then
                    script.AppendLine("     RicalcolaSuperficieCoinvolta(true); ")
                    script.AppendLine("     $('.SommaSuperficieTrattata').keyup(function () {")
                    script.AppendLine("         delay_KeyUp(function(){ ")
                    script.AppendLine("         SommaSuperficieTrattata_Keyup(); ")
                    script.AppendLine("         }, " & RitardoMillisecondiKeyUp & " );")
                    script.AppendLine("     });")
                End If

        End Select


        script.AppendLine("}); ")


        ScriptManager.RegisterStartupScript(UpdatePanelImpianti, UpdatePanelImpianti.GetType(),
                                         String.Format("jQuery_{0}", UpdatePanelImpianti.ClientID), script.ToString, True)




    End Sub


    Protected Sub GridView_Impianti_Sorting(ByVal sender As Object, ByVal e As GridViewSortEventArgs)
        If Not IsNothing(Session("SortExpression")) Then
            Dim sortDirection As String = Convert.ToString(Session("SortDirection"))
            Dim sortExpression As String = Convert.ToString(Session("SortExpression"))
            If (sortExpression = e.SortExpression) Then
                Session("SortDirection") = ChangeDirection(sortDirection)
            Else
                Session("SortExpression") = e.SortExpression
                Session("SortDirection") = "ASC"
            End If
        Else

            Session("SortExpression") = e.SortExpression
            Session("SortDirection") = "ASC"
        End If


        If Not IsNothing(ViewState("DT_Impianti")) Then
            Dim DT As DataTable = ViewState("DT_Impianti")

            Dim dv As New DataView(DT,
                                    "",
                                    "", DataViewRowState.CurrentRows)

            '            Dim dv As DataView = DT.DataSet.Tables(0).DefaultView
            Dim SortExpDirection As String = Convert.ToString(Session("SortExpression"))
            SortExpDirection += " " & Session("SortDirection")
            dv.Sort = SortExpDirection
            GridView_Impianti.DataSource = dv.ToTable()
            ViewState("DT_Impianti") = dv.ToTable()

            GridView_Impianti.DataBind()
            GridViewToPhone()
        End If

    End Sub


    Private Function ChangeDirection(ByVal oldDirection As String) As String
        Dim newDirection As String = ""
        Select Case oldDirection
            Case "ASC"
                newDirection = "DESC"
            Case Else
                newDirection = "ASC"
        End Select
        Return newDirection
    End Function

#End Region

    Protected Sub AggiornaGrigliaImpianti_Click(sender As Object, e As EventArgs) Handles AggiornaGrigliaImpianti.Click
        CType(Page.Master, OperazioneBootstrap).CaricaGriglia_Impianti()
    End Sub


#Region "Costi Accessori Avanzati"

    '##########################################################################################
    Private Sub Crea_Griglia_CentriCostoAvanzati()

        'Aggiungo al datagrid dei centri di costo i magazzini (nella query c'è già il filtro sul tipo di fabbricato)
        Dim objDTableCentriCosto As DataTable
        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        objDTableCentriCosto = objFabbricati.LeggixCostiAccessori(objParametriAgenda.Piva, 0,
                                                                  objParametriAgenda.Data,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                  "", "", objParametri_Server)

        ' "1=2", "", objParametri_Server)

        objFabbricati = Nothing

        Dim Dr As DataRow

        'Aggiungo al datagrid la riga del parco macchine
        Dr = objDTableCentriCosto.NewRow
        Dr.Item(0) = Resources.AgronicaAgenda_2010.ParcoMacchine
        Dr.Item(1) = -1 'Fabbricato_Cod = -1 corrisponde al Parco Macchine
        Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
        objDTableCentriCosto.Rows.Add(Dr)

        'Aggiungo al datagrid la riga della manodopera
        Dr = objDTableCentriCosto.NewRow
        Dr.Item(0) = Resources.AgronicaAgenda_2010.Manodopera
        Dr.Item(1) = -2 'Fabbricato_Cod = -2 corrisponde alla Manodopera
        Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
        objDTableCentriCosto.Rows.Add(Dr)


        'Aggiungo al datagrid la riga del terzisti
        Dr = objDTableCentriCosto.NewRow
        Dr.Item(0) = Resources.AgronicaAgenda_2010.CTerzisti
        Dr.Item(1) = -3 'Fabbricato_Cod = -3 corrisponde ai C/Terzisti
        Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
        objDTableCentriCosto.Rows.Add(Dr)

        'Aggiungo al datagrid la tecnico responsabile
        Dr = objDTableCentriCosto.NewRow
        Dr.Item(0) = Resources.AgronicaAgenda_2010.TecnicoResponsabile
        Dr.Item(1) = -4 'Fabbricato_Cod = -3 corrisponde ai C/Terzisti
        Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
        objDTableCentriCosto.Rows.Add(Dr)

        'metto il DT all'interno del Session
        Session("DT_CentriCosto") = objDTableCentriCosto

        'Vettore di DataColumn
        Dim CdcKeys(1) As String

        'Valorizzo le celle del vettore
        CdcKeys(0) = "Sa_Cod"
        CdcKeys(1) = "Fabbricato_Cod"

        dgrCentriCostoAvanzati.DataSource = objDTableCentriCosto
        dgrCentriCostoAvanzati.DataKeyNames = CdcKeys
        dgrCentriCostoAvanzati.DataBind()


    End Sub


    'click sul bottone dei costi accessori
    Protected Sub AggiornaCostiAccessoriAvanzati_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AggiornaCostiAccessoriAvanzati.Click

        ' CalcolaCosti.Checked = False

        'tabellaCostiAccessoriAvanzati.Visible = True

        Dim dummylist As DataTable
        If Not IsNothing(Session("dtScarico")) Then
            dummylist = Session("dtScarico")

            For i As Integer = 0 To dgrScaricoAvanzati.Rows.Count - 1
                dummylist.Rows(i).Item("Udm_Selezionata") = CType(dgrScaricoAvanzati.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).SelectedValue
                If IsNumeric(CType(dgrScaricoAvanzati.Rows(i).FindControl("Txt_Qta_Ril"), TextBox).Text) Then
                    dummylist.Rows(i).Item("Qta_Ril") = CType(dgrScaricoAvanzati.Rows(i).FindControl("Txt_Qta_Ril"), TextBox).Text
                Else
                    dummylist.Rows(i).Item("Qta_Ril") = 0
                End If

                If IsNumeric(CType(dgrScaricoAvanzati.Rows(i).FindControl("Cmb_Attivita"), DropDownList).SelectedValue) Then
                    dummylist.Rows(i).Item("Id_Attivita") = CType(dgrScaricoAvanzati.Rows(i).FindControl("Cmb_Attivita"), DropDownList).SelectedValue
                Else
                    dummylist.Rows(i).Item("Id_Attivita") = 0
                End If

                If IsNumeric(CType(dgrScaricoAvanzati.Rows(i).FindControl("Cmb_Turni"), DropDownList).SelectedValue) Then
                    dummylist.Rows(i).Item("Turno_Cod") = CType(dgrScaricoAvanzati.Rows(i).FindControl("Cmb_Turni"), DropDownList).SelectedValue
                Else
                    dummylist.Rows(i).Item("Turno_Cod") = 0
                End If

                If dummylist.Rows(i).Item("Id_Attivita") <> 0 And dummylist.Rows(i).Item("Turno_Cod") <> 0 And IsDate(txt_DataOperazione.Text) Then
                    Dim objAttivita As New AgronicaCoreContabDAL.Attivita_R

                    dummylist.Rows(i).Item("Costo_Unitario") = Format(objAttivita.CostoOrario(dummylist.Rows(i).Item("Id_Attivita"), dummylist.Rows(i).Item("Turno_Cod"), txt_DataOperazione.Text, objParametri_Server), "0.00")
                    dummylist.Rows(i).Item("Costo") = Format(dummylist.Rows(i).Item("Costo_Unitario") * dummylist.Rows(i).Item("Qta_Ril"), "0.00")
                    objAttivita = Nothing
                End If
            Next

            Session("dtScarico") = dummylist
        End If

        'Filtro_MaterialiAvanzati.Visible = False

        Crea_Griglia_CentriCostoAvanzati()

        AggiornaDgrScaricoAvanzati()
        Session("dtScarico_old") = Session("dtScarico")

        AggiornaMagazzino()

    End Sub

    Private Sub AggiornaDgrScaricoAvanzati()
        If Not IsNothing(Session("dtScarico")) Then

            'Vettore di DataColumn
            Dim ScaricoKeys(3) As String

            'Valorizzo le celle del vettore
            ScaricoKeys(0) = "Udm_Selezionata"
            ScaricoKeys(1) = "ID_Attivita"
            ScaricoKeys(2) = "Turno_Cod"
            ScaricoKeys(3) = "Qta_Ril"

            dgrScaricoAvanzati.DataSource = Session("dtScarico")
            dgrScaricoAvanzati.DataKeyNames = ScaricoKeys
            dgrScaricoAvanzati.DataBind()

            'dopo aver fatto il bind verifico come impostare la dropdown
            ControllaDDLCostiAccessori(dgrScaricoAvanzati)
        End If
    End Sub

    Private Sub SalvaCostiAccessoriAvanzati_Click(sender As Object, e As System.EventArgs) Handles SalvaCostiAccessoriAvanzati.Click


        SalvaCostiAccessori_SuAgendaMovimenti(True, dgrScaricoAvanzati)

    End Sub


    Private Sub AnnullaCostiAccessoriAvanzati_Click(sender As Object, e As System.EventArgs) Handles AnnullaCostiAccessoriAvanzati.Click

        Session("dtScarico") = Session("dtScarico_old")
        AggiornaGridViewCostiAccessoriVisibili()

    End Sub

    Private Sub BTN_FiltraMateriali_Click(sender As Object, e As System.EventArgs) Handles BTN_FiltraMateriali.Click

        Dim DT_CentriCosto As DataTable = Session("DT_CentriCosto")
        Dim Dt As DataTable

        'Cambio l'intestazione delle colonne nel datagrid
        Me.dgrMateriali.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.CategoriaProdotto
        Me.dgrMateriali.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Prodotto
        Me.dgrMateriali.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.Giacenza

        'If Not IsNothing(Session("Dt_Prodotti")) Then
        '    Dt.Clear()
        'End If

        'If Not IsNothing(Session("Dt_Prodotti")) Then
        '    Dt = Session("Dt_Prodotti")
        'Else
        Popola_ProdottiMagazzino(dgrCentriCosto.DataKeys(dgrCentriCosto.SelectedIndex).Item("Sa_Cod"),
                                 dgrCentriCosto.DataKeys(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod"),
                                 objParametriAgenda.Elem_Cod,
                                 False)

        Dt = Session("Dt_Prodotti")
        ' End If

        Me.dgrMateriali.DataSource = Dt
        Me.dgrMateriali.DataBind()

        ControllaDDLCostiAccessori(dgrScarico)


    End Sub


    Private Sub BTN_FiltraMaterialiAvanzati_Click(sender As Object, e As System.EventArgs) Handles BTN_FiltraMaterialiAvanzati.Click

        Dim DT_CentriCosto As DataTable = Session("DT_CentriCosto")
        Dim Dt As DataTable

        'Cambio l'intestazione delle colonne nel datagrid
        Me.dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.CategoriaProdotto
        Me.dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Prodotto
        Me.dgrMaterialiAvanzati.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.Giacenza

        'If Not IsNothing(Session("Dt_Prodotti")) Then
        '    Dt.Clear()
        'End If

        'If Not IsNothing(Session("Dt_Prodotti")) Then
        '    Dt = Session("Dt_Prodotti")
        'Else
        Popola_ProdottiMagazzino(dgrCentriCostoAvanzati.DataKeys(dgrCentriCostoAvanzati.SelectedIndex).Item("Sa_Cod"),
                                 dgrCentriCostoAvanzati.DataKeys(dgrCentriCostoAvanzati.SelectedIndex).Item("Fabbricato_Cod"),
                                 objParametriAgenda.Elem_Cod,
                                 True)

        Dt = Session("Dt_Prodotti")
        ' End If

        Me.dgrMaterialiAvanzati.DataSource = Dt
        Me.dgrMaterialiAvanzati.DataBind()

        ControllaDDLCostiAccessori(dgrScaricoAvanzati)


    End Sub

    Private Sub dgrCentriCosto_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles dgrCentriCosto.RowCommand

        Dim IndiceRigaGriglia As Integer = Convert.ToInt32(e.CommandArgument)

        Session("IndiceRigaGrigliaCliccato") = IndiceRigaGriglia

        Select Case e.CommandName



            Case "NuovoElemento"
                Dim tipo As String = CType(dgrCentriCosto.Rows(IndiceRigaGriglia).Cells(0).Controls(0), LinkButton).Text
                Select Case tipo
                    Case Resources.AgronicaAgenda_2010.ParcoMacchine
                        ApriMacchinaCosti(objParametriAgenda.Piva)
                    Case Resources.AgronicaAgenda_2010.Manodopera
                        ApriConttattiCosti(objParametriAgenda.Piva, COD_LEGALE)
                    Case Resources.AgronicaAgenda_2010.CTerzisti
                        ApriConttattiCosti(objParametriAgenda.Piva, COD_TERZISTA)
                    Case Resources.AgronicaAgenda_2010.TecnicoResponsabile
                        ApriConttattiCosti(objParametriAgenda.Piva, COD_TECNICORESPONSABILE)
                End Select


        End Select

    End Sub


    Private Sub dgrCentriCostoAvanzati_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles dgrCentriCostoAvanzati.SelectedIndexChanged

        AggiornaCostiAccessoriAvanzati_Click(Me, Nothing)

        Dim DT_CentriCosto As DataTable = Session("DT_CentriCosto")
        Dim Dt As DataTable

        ' lo rendo visibile solo se ho scelto un magazzino (codice cdc>0)
        Filtro_MaterialiAvanzati.Visible = False

        ' svuoto la variabile nel viewstate
        ViewState("SaCodFabbricato") = ""

        'Select Case CInt(DT_CentriCosto.Rows(dgrCentriCostoAvanzati.SelectedIndex).Item("Fabbricato_Cod"))

        Select Case CInt(dgrCentriCostoAvanzati.DataKeys(dgrCentriCostoAvanzati.SelectedIndex).Item("Fabbricato_Cod"))

            Case Is > 0

                Filtro_MaterialiAvanzati.Visible = True
                'Dt = Session("Dt_Prodotti")
                Dt = Nothing
                Lbl_RisFiltroAvanzati.Text = ""

                ViewState("SaCodFabbricato") = dgrCentriCostoAvanzati.DataKeys(dgrCentriCostoAvanzati.SelectedIndex).Item("Sa_Cod")

                'Session("Dt_Prodotti") = Dt

                Me.dgrMaterialiAvanzati.DataSource = Dt
                Me.dgrMaterialiAvanzati.DataBind()

                ' commentato e aggiunto exit nicoletta 13/03/2014
                Exit Sub

                ''Cambio l'intestazione delle colonne nel datagrid
                'Me.dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.CategoriaProdotto
                'Me.dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Prodotto
                'Me.dgrMaterialiAvanzati.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.Giacenza

                'If Not IsNothing(Session("Dt_Prodotti")) Then
                '    Dt = Session("Dt_Prodotti")
                'Else
                '    ' nico
                '    Popola_ProdottiMagazzino(CInt(DT_CentriCosto.Rows(dgrCentriCostoAvanzati.SelectedIndex).Item("Fabbricato_Cod")), _
                '                             objParametriAgenda.Elem_Cod)
                '    Dt = Session("Dt_Prodotti")
                'End If

                'Me.dgrMaterialiAvanzati.DataSource = Dt
                'Me.dgrMaterialiAvanzati.DataBind()

            Case -1

                'Cambio l'intestazione delle colonne nel datagrid
                dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.MacchinaAttrezzatura
                dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Descrizione
                dgrMaterialiAvanzati.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

                If Not IsNothing(Session("Dt_Macchine")) Then
                    Dt = Session("Dt_Macchine")
                Else
                    'nico
                    Popola_ParcoMacchine()
                    Dt = Session("Dt_Macchine")
                End If

                Me.dgrMaterialiAvanzati.DataSource = Dt
                Me.dgrMaterialiAvanzati.DataBind()

            Case -2

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.RapportoContabile
                Me.dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Manodopera
                Me.dgrMaterialiAvanzati.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

                If Not IsNothing(Session("Dt_Manodopera")) Then
                    Dt = Session("Dt_Manodopera")
                Else
                    ' nico
                    Popola_Manodopera()
                    Dt = Session("Dt_Manodopera")
                End If

                Me.dgrMaterialiAvanzati.DataSource = Dt
                Me.dgrMaterialiAvanzati.DataBind()

            Case -3

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.RapportoContabile
                Me.dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Terzista
                Me.dgrMaterialiAvanzati.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

                If Not IsNothing(Session("Dt_Terzisti")) Then
                    Dt = Session("Dt_Terzisti")
                Else
                    Popola_Terzisti()
                    Dt = Session("Dt_Terzisti")
                End If

                Me.dgrMaterialiAvanzati.DataSource = Dt
                Me.dgrMaterialiAvanzati.DataBind()


            Case -4

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.RapportoContabile
                Me.dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.TecnicoResponsabile
                Me.dgrMaterialiAvanzati.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

                If Not IsNothing(Session("Dt_TecnicoResponsabile")) Then
                    Dt = Session("Dt_TecnicoResponsabile")
                Else
                    ' nico
                    Popola_TecnicoResponsabile()
                    Dt = Session("Dt_TecnicoResponsabile")
                End If

                Me.dgrMaterialiAvanzati.DataSource = Dt
                Me.dgrMaterialiAvanzati.DataBind()

        End Select

        ' nico
        ControllaDDLCostiAccessori(dgrScaricoAvanzati)
    End Sub


    Private Sub dgrMaterialiAvanzati_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles dgrMaterialiAvanzati.RowCommand

        AggiornaCostiAccessoriAvanzati_Click(Me, Nothing)

        Dim IndiceRigaGriglia As Integer = 0

        Dim Centro As String = ""
        Dim Centro_Cod As Int32
        Dim Categoria_Des As String = ""
        Dim Risorsa_Des As String = ""
        Dim Udm_Des As String = ""
        Dim Qta_Ril As Decimal
        Dim Udm_Cod As Int32
        Dim Elem_Cod As Int32
        Dim Riga As Int32
        Dim Tipo_Centro As String = ""
        Dim Pro_Cod As Int32
        Dim Ditta_Cod As Int32
        Dim Mat_Cod As Int32
        Dim Costo_Unitario As String = ""
        Dim Costo As String = ""
        Dim Sa_Cod As Integer

        Dim Cod_Rapporto As Integer = 0

        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Dim dtScarico As DataTable

        dtScarico = Session("dtScarico")

        Select Case e.CommandName

            Case "AggiungiCosto"

                If Session("DT_CentriCosto") IsNot Nothing Then

                    Dim DT_CentriCosto As DataTable = Session("DT_CentriCosto")

                    Select Case CInt(DT_CentriCosto.Rows(dgrCentriCostoAvanzati.SelectedIndex).Item("Fabbricato_Cod"))

                        Case Is > 0

                            If Not IsNothing(Session("Dt_Prodotti")) Then

                                Dim Dt_Prodotti As DataTable = Session("Dt_Prodotti")

                                If Dt_Prodotti.Rows.Count >= IndiceRigaGriglia Then

                                    Dim objFabb As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                                    Centro = objFabb.FabbricatoDes_from_FabbricatoCod(
                                                objParametriAgenda.Piva,
                                                dgrCentriCostoAvanzati.DataKeys(dgrCentriCostoAvanzati.SelectedIndex).Item("Sa_Cod"),
                                                dgrCentriCostoAvanzati.DataKeys(dgrCentriCostoAvanzati.SelectedIndex).Item("Fabbricato_Cod"),
                                                objParametri_Server) 'Magazzino

                                    Tipo_Centro = "M"
                                    Centro_Cod = CInt(dgrCentriCostoAvanzati.DataKeys(dgrCentriCostoAvanzati.SelectedIndex).Item("Fabbricato_Cod"))
                                    Sa_Cod = dgrCentriCostoAvanzati.DataKeys(dgrCentriCostoAvanzati.SelectedIndex).Item("Sa_Cod")

                                    Elem_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_6")
                                    Pro_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_8")
                                    Categoria_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Udm_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_7")
                                    Riga = 0 'IndiceRigaGriglia

                                End If

                            End If

                        Case -1

                            If Not IsNothing(Session("Dt_Macchine")) Then

                                Dim Dt_Macchine As DataTable = Session("Dt_Macchine")

                                If Dt_Macchine.Rows.Count >= IndiceRigaGriglia Then

                                    Centro = "Parco Macchine"
                                    Tipo_Centro = "PM"
                                    Centro_Cod = -1
                                    Sa_Cod = 0

                                    Categoria_Des = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Elem_Cod = MACCHINE
                                    Pro_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_8")

                                    Udm_Des = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_7")
                                    Riga = 0 'IndiceRigaGriglia

                                End If

                            End If

                        Case -2

                            If Not IsNothing(Session("Dt_Manodopera")) Then

                                Dim Dt_Manodopera As DataTable = Session("Dt_Manodopera")

                                If Dt_Manodopera.Rows.Count >= IndiceRigaGriglia Then

                                    Centro = "Manodopera"
                                    Tipo_Centro = "MD"
                                    Centro_Cod = -2
                                    Sa_Cod = 0

                                    Categoria_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Elem_Cod = 0
                                    Pro_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_8")

                                    Udm_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_7")

                                    Cod_Rapporto = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_11")

                                    Riga = 0 'IndiceRigaGriglia

                                End If

                            End If

                        Case -3 'TERZISTI


                            If Not IsNothing(Session("Dt_Terzisti")) Then

                                Dim Dt_Manodopera As DataTable = Session("Dt_Terzisti")

                                If Dt_Manodopera.Rows.Count >= IndiceRigaGriglia Then

                                    Centro_Cod = -3
                                    Centro = "C/Terzisti"
                                    Tipo_Centro = "CT"
                                    Sa_Cod = 0

                                    Categoria_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Elem_Cod = 0
                                    Pro_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_8")

                                    Udm_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_7")
                                    Riga = 0 'IndiceRigaGriglia

                                End If

                            End If



                        Case -4  'Tecnico Responsabile

                            If Not IsNothing(Session("Dt_TecnicoResponsabile")) Then

                                Dim Dt_Manodopera As DataTable = Session("Dt_TecnicoResponsabile")

                                If Dt_Manodopera.Rows.Count >= IndiceRigaGriglia Then

                                    Centro_Cod = -4
                                    Centro = "Tecnico Responsabile"
                                    Tipo_Centro = "TR"
                                    Sa_Cod = 0

                                    Categoria_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Elem_Cod = 0
                                    Pro_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_8")

                                    Udm_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_7")
                                    Riga = 0 'IndiceRigaGriglia

                                End If

                            End If

                    End Select

                    InserisciRiga_dtScarico(Centro,
                                            Sa_Cod,
                                            Centro_Cod,
                                            Categoria_Des,
                                            Risorsa_Des,
                                            Udm_Des,
                                            Qta_Ril,
                                            Udm_Cod,
                                            Elem_Cod,
                                            Riga,
                                            Tipo_Centro,
                                            Pro_Cod,
                                            Ditta_Cod,
                                            Mat_Cod,
                                            Costo_Unitario,
                                            Costo,
                                            0, "", 0, "",
                                            0, 0,
                                            0, 0,
                                            Cod_Rapporto, dtScarico)


                End If

                'InserisciCosto(CType(dgrMateriali.Rows(IndiceRigaGriglia).FindControl("Col_4"), TextBox), Nothing)

        End Select

        Session("dtScarico") = dtScarico
        AggiornaDgrScaricoAvanzati()

    End Sub



#End Region



#Region "Ricette"

    Private Sub CaricaElencoRicette()

        Dim objRicette As New AgronicaCoreContabDAL.Ricette_Operazioni_R

        Dim Filtro As String = " (Ricette.Piva = '' OR Ricette.Piva='" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(objParametriAgenda.Piva) & "')"

        If objParametriAgenda.Lav_Cod = 0 Then
            lbl_Ricette.Text = "Selezionare la tipologia di Lavorazione per caricare le ricette"
            Exit Sub
        End If
        If objParametriAgenda.Veg_Cod.Split("/")(0) = -1 Then
            lbl_Ricette.Text = "Selezionare una specie per caricare le ricette"
            Exit Sub
        End If

        Dim Dt_Ricette As DataTable
        Dim Id_Cod As Integer = 0
        If objParametriAgenda.Veg_Cod.Split("/").Length > 1 Then
            Id_Cod = objParametriAgenda.Veg_Cod.Split("/")(1)
        End If
        'Dt_Ricette = objRicette.Leggi_LavCod_Cul(objParametriAgenda.Lav_Cod,
        '                objParametriAgenda.Veg_Cod.Split("/")(0),
        '                objParametriAgenda.Data,
        '                objParametriAgenda.Data,
        '                    "",
        '                    "",
        '                    HttpContext.Current.Session("ASG_objParametri_Server"))

        Dt_Ricette = objRicette.Leggi_conDettagli(objParametriAgenda.Lav_Cod,
                        objParametriAgenda.Veg_Cod.Split("/")(0),
                        Id_Cod,
                        objParametriAgenda.Data,
                        objParametriAgenda.Data,
                            " Ricette_dettagli.cau_mov IN ('2050','2300', '2100', '2200') ",
                            " Ricette.tipo_ricetta ASC, Ricette_Operazioni.Validita_inizio DESC ",
                            HttpContext.Current.Session("ASG_objParametri_Server"))

        'leggo le avversita
        Dim objMovTec As New AgronicaCoreContabDAL.Ricette_Dett_Tecnico_R
        Dim dtMovDetTec As DataTable = objMovTec.Leggi_x_avversita(objParametriAgenda.Piva, "", "", objParametri_Server)

        If Dt_Ricette.Rows.Count = 0 Then
            lbl_Ricette.Text = "Nessuna ricetta attiva su questa specie  una cultura per caricare le ricette"
            Exit Sub
        Else
            lbl_Ricette.Text = ""
        End If

        Dim i As Integer = 0

        rbl_RicetteAttive.Items.Clear()

        Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility
        Dim strOperazioni() As String = objSqlDis.SelectDistinct(Dt_Ricette, "Ricetta_Operazione_Cod")

        Dim Valore As String
        Dim Descrizione As String

        For i = 0 To strOperazioni.Length - 1

            Dim DrOperazione = Dt_Ricette.Select("Ricetta_Operazione_Cod=" & strOperazioni(i))
            Dim DtOperazione As New DataTable

            Dim strCentro As String = ""
            Dim strAppezzamenti As String = ""
            Dim AppezzamentoNome As String = ""
            Dim strCulDes As String = ""
            Dim strSpecieVarieta As String = ""

            Dim DtApp As New DataTable
            Dim DtProdotti As New DataTable
            Dim DtProdotti1 As New DataTable
            Dim strProdotti As String = ""
            Dim Prodotto As String = ""

            Valore = ""
            Descrizione = ""


            If DrOperazione IsNot Nothing AndAlso DrOperazione.Length > 0 Then

                DtOperazione = Dt_Ricette.Clone

                For j = 0 To DrOperazione.Length - 1
                    DtOperazione.ImportRow(DrOperazione(j))
                Next

                Valore = DrOperazione(0).Item("ricetta_cod") & "|" & DrOperazione(0).Item("Ricetta_Operazione_Cod") & "|" & DrOperazione(0).Item("Tipo_Ricetta")
                Select Case DrOperazione(0).Item("Tipo_Ricetta")
                    Case enum_TipoRicetta.Standard
                        Descrizione = "Linea tecnica (" & DrOperazione(0).Item("Ricetta_Des") & "): "
                    Case enum_TipoRicetta.Standard_Destinazioni
                        'Descrizione = "Ricetta aziendale: " & DrOperazione(0).Item("Ricetta_Operazione_Des") & " - " & CDate(DrOperazione(0).Item("validita_inizio")).ToShortDateString
                        Descrizione = "Ricetta aziendale (" & DrOperazione(0).Item("Ricetta_Des") & "): " & CDate(DrOperazione(0).Item("validita_inizio")).ToShortDateString
                    Case enum_TipoRicetta.PianoDistribuzioneConcimi
                        Descrizione = "Piano Distribuzione (" & DrOperazione(0).Item("Ricetta_Des") & "): " & CDate(DrOperazione(0).Item("validita_inizio")).ToShortDateString
                End Select

                'appezzamenti
                DtApp = objSqlDis.SelectDistinct("Appezzamenti", DtOperazione, "appezza", False)

                For j = 0 To DtApp.Rows.Count - 1
                    If j = 0 Then
                        Try
                            strCentro = DtApp.Rows(j).Item("Sa_Nome")
                        Catch ex As Exception
                            strCentro = ""
                        End Try
                    End If
                    If DtApp.Rows(j).Item("App_Nome") <> "" Then
                        AppezzamentoNome = Replace(DtApp.Rows(j).Item("App_Nome"), "'", "")
                        strAppezzamenti &= AppezzamentoNome & ", "
                    End If
                    'If DtApp.Rows(j).Item("cul_des") <> "" AndAlso InStr(strCulDes, DtApp.Rows(j).Item("cul_des")) = 0 Then
                    '    strCulDes &= Replace(DtApp.Rows(j).Item("cul_des"), "'", "") & ", "
                    'End If

                    'If DtApp.Rows(j).Item("cul_des") <> "" AndAlso InStr(strSpecieVarieta, DtApp.Rows(j).Item("cul_des")) = 0 Then
                    '    Dim specie As String = If(DtApp.Rows(j).Item("veg_cod") <> 0, DtApp.Rows(j).Item("veg_des") & " - ", "")
                    '    Dim varieta As String = Replace(DtApp.Rows(j).Item("cul_des"), "'", "")
                    '    strSpecieVarieta &= specie & varieta & ", "
                    'End If

                Next
                If strAppezzamenti <> "" Then
                    strAppezzamenti = " - App: " & Left(strAppezzamenti, strAppezzamenti.Length - 2)
                    If strCentro <> "" Then
                        strAppezzamenti = " - Centro: " & strCentro & strAppezzamenti
                    End If
                End If


                'prodotti
                Select Case CInt(DrOperazione(0).Item("Elem_Cod"))

                    Case FERTILIZZANTI  'FERTILIZZANTI

                        DtProdotti = objSqlDis.SelectDistinct("Fertilizzanti", DtOperazione, "pro_cod", False)
                        DtProdotti1 = objSqlDis.SelectDistinct("Fertilizzanti1", DtOperazione, "mat_cod", False)
                        For j = 0 To DtProdotti.Rows.Count - 1
                            If DtProdotti.Rows(j).Item("Pro_Cod") <> 0 Then
                                Prodotto = DtProdotti.Rows(j).Item("Fer_Des")
                                strProdotti &= Prodotto & ", "
                            End If
                        Next
                        For j = 0 To DtProdotti1.Rows.Count - 1
                            If DtProdotti1.Rows(j).Item("Mat_Cod") <> 0 Then
                                Prodotto = DtProdotti1.Rows(j).Item("Mat_Des")
                                strProdotti &= Prodotto & ", "
                            End If
                        Next
                        If strProdotti <> "" Then
                            strProdotti = " - " & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & " " & Left(strProdotti, strProdotti.Length - 2)
                        End If

                    Case FORMULATI    'FORMULATI

                        DtProdotti = objSqlDis.SelectDistinct("Formulati", DtOperazione, "pro_cod", False)
                        For j = 0 To DtProdotti.Rows.Count - 1
                            If DtProdotti.Rows(j).Item("Fr_Des") <> "" Then

                                Dim drMovDetTec2() As DataRow = dtMovDetTec.Select("Ricetta_Operazione_Cod=" & DtProdotti.Rows(j).Item("Ricetta_Operazione_Cod") & " AND Ricetta_Dettaglio_Cod=" & DtProdotti.Rows(j).Item("Ricetta_Dettaglio_Cod"))
                                Dim listaAvv2 As New List(Of String)

                                For Each drAvv2 As DataRow In drMovDetTec2
                                    If drAvv2.Item("Av_des_vol") <> "" Then
                                        listaAvv2.Add(drAvv2.Item("Av_des_vol"))
                                    End If
                                    If drAvv2.Item("Av_Gru_des") <> "" Then
                                        listaAvv2.Add(drAvv2.Item("Av_Gru_des"))
                                    End If
                                Next

                                Dim avv As String = String.Join(", ", listaAvv2)

                                Prodotto = DtProdotti.Rows(j).Item("Fr_Des")
                                If avv <> "" Then
                                    Prodotto &= " (" & avv & ")"
                                End If
                                strProdotti &= Prodotto & ", "
                            End If
                        Next
                        If strProdotti <> "" Then
                            strProdotti = " - " & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & " " & Left(strProdotti, strProdotti.Length - 2)
                        End If

                    Case TRAPPOLE

                        DtProdotti = objSqlDis.SelectDistinct("Trappole", DtOperazione, "pro_cod", False)
                        For j = 0 To DtProdotti.Rows.Count - 1
                            If DtProdotti.Rows(j).Item("Trap_Des") <> "" Then
                                Prodotto = DtProdotti.Rows(j).Item("Trap_Des")
                                strProdotti &= Prodotto & ", "
                            End If
                        Next
                        If strProdotti <> "" Then
                            strProdotti = " - " & Resources.AgronicaAgenda_2010.ProdottiUtilizzati & " " & Left(strProdotti, strProdotti.Length - 2)
                        End If

                    Case SEMENTI

                        DtProdotti = objSqlDis.SelectDistinct("Materie Prime", DtOperazione, "mat_cod", False)
                        For j = 0 To DtProdotti.Rows.Count - 1
                            If DtProdotti.Rows(j).Item("Mat_Des") <> "" Then
                                Prodotto = DtProdotti.Rows(j).Item("Mat_Des") '& " (Lotto: " & DtProdotti.Rows(j).Item("Cod_Articolo") & ")"
                                strProdotti &= Prodotto & ", "
                            End If
                        Next
                        If strProdotti <> "" Then
                            strProdotti = " - " & Resources.AgronicaAgenda_2010.MaterialeVivaistaUtilizzato & " " & Left(strProdotti, strProdotti.Length - 2)
                        End If

                End Select

            End If

            rbl_RicetteAttive.Items.Insert(0, New ListItem(Descrizione & strProdotti & strAppezzamenti, Valore))

        Next

        'For i = 0 To Dt_Ricette.Rows.Count - 1
        '    Descrizione = ""
        '    Select Case Dt_Ricette.Rows(i).Item("Tipo_Ricetta")
        '        Case enum_TipoRicetta.Standard
        '            Descrizione = "Linea tecnica: " & Dt_Ricette.Rows(i).Item("Ricetta_Operazione_Des")
        '        Case enum_TipoRicetta.Standard_Destinazioni
        '            Descrizione = "Ricetta aziendale: " & Dt_Ricette.Rows(i).Item("Ricetta_Operazione_Des") & " - " & CDate(Dt_Ricette.Rows(i).Item("validita_inizio")).ToShortDateString
        '        Case enum_TipoRicetta.PianoDistribuzioneConcimi
        '            Descrizione = "Piano Distribuzione: " & Dt_Ricette.Rows(i).Item("Ricetta_Operazione_Des") & " - " & CDate(Dt_Ricette.Rows(i).Item("validita_inizio")).ToShortDateString
        '    End Select
        '    rbl_RicetteAttive.Items.Add(New ListItem(Descrizione, Dt_Ricette.Rows(i).Item("ricetta_cod") & "|" & Dt_Ricette.Rows(i).Item("Ricetta_Operazione_Cod") & "|" & Dt_Ricette.Rows(i).Item("Tipo_Ricetta")))
        'Next

    End Sub


    Protected Sub Btn_Inserisci_Impianti_Ricetta_Click(sender As Object, e As EventArgs) Handles Btn_Inserisci_Impianti_Ricetta.Click
        Dim ricetta As String = rbl_RicetteAttive.SelectedValue
        If ricetta = "" Then
            Messaggi.AgroMsgBox("Selezionare una ricetta prima di procedere ", Page, , UpdatePanelPerScript)
            Exit Sub
        End If
        ricetta_cod = Split(ricetta, "|")(0)
        Session("ricetta_cod") = ricetta_cod
        ricetta_operazione_cod = Split(ricetta, "|")(1)
        Session("Ricetta_Operazione_Cod") = ricetta_operazione_cod
        Ricetta_Tipo = Split(ricetta, "|")(2)
        Session("Ricetta_Tipo") = Ricetta_Tipo

        Dim impiantiPreSelezionati As Integer = GetImpianti().Count

        Dim ListaImpiantiRicetta As New List(Of Impianto)
        Inserisci_Impianti_Della_Ricetta(ListaImpiantiRicetta)
        Dim impiantiPostSelezionati As Integer = GetImpianti().Count
        If ListaImpiantiRicetta.Count = 0 Then
            If impiantiPreSelezionati = 0 Then
                Messaggi.AgroMsgBox("La ricetta non indica nessuna destinazione e non è selezionato alcun impianto nella tabella.", Page, , UpdatePanelPerScript)
                chiudiDialog()
                Exit Sub
            Else
                Messaggi.AgroMsgBox("La ricetta non indica nessuna destinazione ma sono selezionati " & impiantiPreSelezionati & " impianti nella tabella.", Page, , UpdatePanelPerScript)
                'nascondiVisualizzaPulsantiCaricaConferma(False)
                Exit Sub
            End If

        Else
            Messaggi.AgroMsgBox("Sono stati aggiunti " & ListaImpiantiRicetta.Count & " impianti dalla ricetta.", Page, , UpdatePanelPerScript)
            'nascondiVisualizzaPulsantiCaricaConferma(False)
            Exit Sub
        End If


    End Sub


    Public Sub Inserisci_Impianti_Della_Ricetta(ByRef ListaImpiantiRicetta As List(Of Impianto))

        If objParametriAgenda.Impianti.Count = 0 Then

            If ListaImpiantiRicetta.Count = 0 Then
                Dim dt As DataTable = New AgronicaCoreContabDAL.Ricette_Destinazioni_R().Leggi(CInt(ricetta_cod), CInt(ricetta_operazione_cod), 0, 0, 0, "", 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, " Tipo_Destinazione=0", "", objParametri_Server)
                Dim imp As Impianto

                For i = 0 To dt.Rows.Count - 1

                    Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                    objAppezzamento.Piva = dt.Rows(i).Item("Piva")
                    objAppezzamento.Sa_Cod = dt.Rows(i).Item("Sa_Cod")
                    objAppezzamento.Appezza = dt.Rows(i).Item("Appezza")
                    objAppezzamento.ID_Reg = dt.Rows(i).Item("ID_Reg")
                    objAppezzamento.Qta2 = dt.Rows(i).Item("Qta2")

                    imp = New Impianto
                    imp.Piva = dt.Rows(i).Item("Piva")
                    imp.Sa_Cod = dt.Rows(i).Item("Sa_Cod")
                    imp.Appezza = dt.Rows(i).Item("Appezza")
                    imp.ID_Reg = dt.Rows(i).Item("ID_Reg")
                    ListaImpiantiRicetta.Add(imp)

                    objParametriAgenda.Impianti.Add(objAppezzamento)
                    objParametriAgenda.salva()
                Next
            End If




            For i = 0 To ListaImpiantiRicetta.Count - 1

                Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
                objAppezzamento.Piva = ListaImpiantiRicetta(i).Piva
                objAppezzamento.Sa_Cod = ListaImpiantiRicetta(i).Sa_Cod
                objAppezzamento.Appezza = ListaImpiantiRicetta(i).Appezza
                objAppezzamento.ID_Reg = ListaImpiantiRicetta(i).ID_Reg
                objAppezzamento.Qta2 = ListaImpiantiRicetta(i).Qta2

                objParametriAgenda.Impianti.Add(objAppezzamento)
                objParametriAgenda.salva()

                'For j = 0 To GridView_Impianti.Rows.Count - 1
                '    If ListaImpiantiRicetta(i).Piva = GridView_Impianti.DataKeys(j).Item("Piva") And
                '       ListaImpiantiRicetta(i).Sa_Cod = GridView_Impianti.DataKeys(j).Item("Sa_Cod") And
                '       ListaImpiantiRicetta(i).Appezza = GridView_Impianti.DataKeys(j).Item("Appezza") And
                '       ListaImpiantiRicetta(i).ID_Reg = GridView_Impianti.DataKeys(j).Item("Id_Reg") Then
                '        CType(GridView_Impianti.Rows(j).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True
                '        CType(GridView_Impianti.Rows(j).FindControl("Txt_Trattata"), TextBox).Text = GridView_Impianti.DataKeys(j).Item("Sup_Imp")
                '        Exit For
                '    End If
                'Next
            Next

        End If

        Dim jss As New JavaScriptSerializer
        hdKendo_Impianti_Selezione.Value = jss.Serialize(objParametriAgenda.Impianti)

        hdKendo_Impianti_Ricarica.Value = "ricarica"

        'Dim script As New StringBuilder
        'script.AppendLine("$(document).ready(function () { ")
        'script.AppendLine("     RicalcolaSuperficieCoinvolta(); ")
        'script.AppendLine("     RicalcolaSuperficieTotale(); ")
        'script.AppendLine("}); ")

        'ScriptManager.RegisterStartupScript(UpdatePanelPerScript, UpdatePanelPerScript.GetType(),
        '                                 String.Format("jQuery_{0}", UpdatePanelPerScript.ClientID), script.ToString, True)

    End Sub


    Protected Sub Btn_Inserisci_Costi_Ricetta_Click(sender As Object, e As EventArgs) Handles Btn_Inserisci_Costi_Ricetta.Click
        Dim ricetta As String = rbl_RicetteAttive.SelectedValue
        If ricetta = "" Then
            Messaggi.AgroMsgBox("Selezionare una ricetta prima di procedere ", Page, , UpdatePanelPerScript)
            Exit Sub
        End If
        ricetta_cod = Split(ricetta, "|")(0)
        Session("ricetta_cod") = ricetta_cod
        ricetta_operazione_cod = Split(ricetta, "|")(1)
        Session("Ricetta_Operazione_Cod") = ricetta_operazione_cod
        Ricetta_Tipo = Split(ricetta, "|")(2)
        Session("Ricetta_Tipo") = Ricetta_Tipo

        Dim impiantiPreSelezionati As Integer = GetImpianti().Count

        Dim dt_CostiRicetta As DataTable = Inserisci_Costi_Della_Ricetta()

        If IsNothing(dt_CostiRicetta) OrElse dt_CostiRicetta.Rows.Count = 0 Then
            Messaggi.AgroMsgBox("La ricetta non indica nessun costo accessorio.", Page, , UpdatePanelPerScript)
        End If

    End Sub


    Private Function Inserisci_Costi_Della_Ricetta() As DataTable

        Dim dt_RicetteDett As DataTable = New AgronicaCoreContabDAL.Ricette_Dettagli_R().Leggi(CInt(ricetta_cod),
                                                                                               CInt(ricetta_operazione_cod),
                                                                                               0,
                                                                                               "",
                                                                                               0,
                                                                                               0,
                                                                                               0,
                                                                                               0,
                                                                                               AGRODATAINIZIO,
                                                                                               AGRODATAFINE,
                                                                                               AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                               " (  Cau_Mov = " & CAU_IMPUTAZIONE_PARCOMACCHINE & " or Cau_Mov = " & CAU_IMPUTAZIONE_MANODOPERA & " or Cau_Mov = " & CAU_IMPUTAZIONE_TERZISTI & " or Cau_Mov = " & CAU_IMPUTAZIONE_UTILIZZO_PRODOTTI & " or Cau_Mov = " & CAU_IMPUTAZIONE_TECNICO_RESPONSABILE & "   )",
                                                                                               "",
                                                                                               objParametri_Server)


        If dt_RicetteDett.Rows.Count > 0 Then

        Else
            Return dt_RicetteDett
        End If

        Dim MovimentiCosti As List(Of AgronicaCoreModello.OperazioneAgenda_Temp.Movimento) = New List(Of AgronicaCoreModello.OperazioneAgenda_Temp.Movimento)

        'cau_mov costi
        For i = 0 To dt_RicetteDett.Rows.Count - 1
            Dim movimento As New AgronicaCoreModello.OperazioneAgenda_Temp.Movimento

            movimento.Piva = objParametriAgenda.Piva
            movimento.Sa_Cod = 0


            movimento.Cau_Mov = dt_RicetteDett.Rows(i).Item("Cau_Mov")
            movimento.Data = objParametriAgenda.Data

            Dim movdet As New AgronicaCoreModello.OperazioneAgenda_Temp.Movimento_Dettaglio

            movdet.Piva = objParametriAgenda.Piva
            movdet.Sa_Cod = 0


            'movdet.Miscela_Cod = dt_RicetteDettagli.Rows(i).Item("Miscela_Cod")
            movdet.Elem_Cod = dt_RicetteDett.Rows(i).Item("Elem_Cod")
            movdet.Pro_Cod = dt_RicetteDett.Rows(i).Item("Pro_Cod")
            movdet.Mat_Cod = dt_RicetteDett.Rows(i).Item("Mat_Cod")
            movdet.Udm_Cod = dt_RicetteDett.Rows(i).Item("Udm_Cod")

            If dt_RicetteDett.Rows(i).Item("Qta") <> 0 Then
                movdet.Qta = dt_RicetteDett.Rows(i).Item("Qta")
            Else
                Dim k As Integer = 0
                Dim sum As Decimal = 0
                Dim listaim As List(Of Impianto) = GetImpianti()
                For k = 0 To listaim.Count - 1
                    sum = sum + listaim(k).Qta2
                Next
                movdet.Qta = sum
            End If

            movdet.Extra_Int = dt_RicetteDett.Rows(i).Item("Extra_Int")
            'movdet.DataLock = dt_RicetteDettagli.Rows(i).Item("DataLock")
            'movdet.inviato = dt_RicetteDettagli.Rows(i).Item("inviato")
            'movdet.datainvio = dt_RicetteDettagli.Rows(i).Item("datainvio")
            'movdet.Validita_Inizio = dt_RicetteDettagli.Rows(i).Item("Validita_Inizio")
            'movdet.Validita_Fine = dt_RicetteDettagli.Rows(i).Item("Validita_Fine")
            'movdet.Prezzo_Unitario = dt_RicetteDettagli.Rows(i).Item("Prezzo_Unitario")
            movdet.Data = objParametriAgenda.Data

            movdet.Contabilizzato = NONCONTABILE

            movimento.Movimenti_Dettagli.Add(movdet)

            Select Case CInt(dt_RicetteDett.Rows(i).Item("Cau_Mov"))


                Case CAU_SCARICO


                Case CAU_IMPUTAZIONE_PARCOMACCHINE


                Case CAU_IMPUTAZIONE_MANODOPERA


                Case CAU_IMPUTAZIONE_TERZISTI


                Case CAU_IMPUTAZIONE_UTILIZZO_PRODOTTI


                Case CAU_IMPUTAZIONE_TECNICO_RESPONSABILE


            End Select

            MovimentiCosti.Add(movimento)
        Next

        objParametriAgenda.Movimenti = MovimentiCosti

        CaricaCostiAccessori()

        Return dt_RicetteDett

    End Function


    Protected Sub Btn_Conferma_Ricetta_Click(sender As Object, e As EventArgs) Handles Btn_Conferma_Ricetta.Click

        Dim ricetta As String = rbl_RicetteAttive.SelectedValue
        If ricetta = "" Then
            Messaggi.AgroMsgBox("Selezionare una ricetta prima di procedere ", Page, , UpdatePanelPerScript)
            Exit Sub
        End If
        ricetta_cod = Split(ricetta, "|")(0)
        Session("ricetta_cod") = ricetta_cod
        ricetta_operazione_cod = Split(ricetta, "|")(1)
        Session("Ricetta_Operazione_Cod") = ricetta_operazione_cod
        Ricetta_Tipo = Split(ricetta, "|")(2)
        Session("Ricetta_Tipo") = Ricetta_Tipo

        Select Case Ricetta_Tipo
            Case enum_TipoRicetta.Standard_Destinazioni, enum_TipoRicetta.PianoDistribuzioneConcimi
            Case Else
                Dim listaImpiantiSelezionati As List(Of Impianto) = GetImpianti()
                If listaImpiantiSelezionati.Count = 0 Then
                    Messaggi.AgroMsgBox("Non è selezionato nessun impianto dalla tabella", Page, , UpdatePanelPerScript)
                    chiudiDialog()
                    Exit Sub
                End If
        End Select

        Ripristina_Dati_nei_ControlliDaRicetta()

        chiudiDialog()
    End Sub


    Private Sub Ripristina_Dati_nei_ControlliDaRicetta()



    End Sub


    Private Sub chiudiDialog()
        Dim script As New StringBuilder
        script.AppendLine("$(document).ready(function () { ")
        script.AppendLine("         $('#dialogRicette').modal('hide');")
        script.AppendLine("     });")

        ScriptManager.RegisterStartupScript(Property_UpdatePanelPerScript,
                                UpdatePanelPerScript.GetType(),
                                "jQuery_{0}", script.ToString, True)
    End Sub


    Private Sub nascondiVisualizzaPulsantiCaricaConferma(ByRef carica As Boolean)
        Dim script As New StringBuilder
        If carica Then
            script.AppendLine("$(document).ready(function () { ")
            script.AppendLine("             $('#btn_carica_dialog').show();")
            script.AppendLine("             $('#btn_conferma_dialog').hide();")
            script.AppendLine("     });")
        Else
            script.AppendLine("$(document).ready(function () { ")
            script.AppendLine("             $('#btn_carica_dialog').hide();")
            script.AppendLine("             $('#btn_conferma_dialog').show();")
            script.AppendLine("     });")
        End If

        ScriptManager.RegisterStartupScript(Property_UpdatePanelPerScript,
                                UpdatePanelPerScript.GetType(),
                                "jQuery_{0}", script.ToString, True)
    End Sub







#End Region

#Region "Proprietà"

    Public Property Property_ricetta_cod() As String
        Get
            Return ricetta_cod
        End Get
        Set(value As String)
            ricetta_cod = value
        End Set
    End Property

    Public Property Property_Ricetta_Operazione_Cod() As String
        Get
            Return ricetta_operazione_cod
        End Get
        Set(value As String)
            ricetta_operazione_cod = value
        End Set
    End Property

    Public Property Property_Ricetta_Tipo() As Integer
        Get
            Return Ricetta_Tipo
        End Get
        Set(value As Integer)
            Ricetta_Tipo = value
        End Set
    End Property

    Public Property Property_Btn_Conferma_Ricetta() As Global.System.Web.UI.WebControls.Button
        Get
            Return Btn_Conferma_Ricetta
        End Get
        Set(value As Global.System.Web.UI.WebControls.Button)
            Btn_Conferma_Ricetta = value
        End Set
    End Property

    Public Property Property_BTN_ChangeData() As Global.System.Web.UI.WebControls.Button
        Get
            Return BTN_ChangeData
        End Get
        Set(value As Global.System.Web.UI.WebControls.Button)
            BTN_ChangeData = value
        End Set
    End Property

    Public Property Property_BTN_CentroAziendale() As Global.System.Web.UI.WebControls.Button
        Get
            Return BTN_ComboCentroAziendale
        End Get
        Set(value As Global.System.Web.UI.WebControls.Button)
            BTN_ComboCentroAziendale = value
        End Set
    End Property

    Public Property Property_BTN_ComboOperazione() As Global.System.Web.UI.WebControls.Button
        Get
            Return BTN_ComboOperazione
        End Get
        Set(value As Global.System.Web.UI.WebControls.Button)
            BTN_ComboOperazione = value
        End Set
    End Property

    Public Property Property_BTN_Magazzini() As Global.System.Web.UI.WebControls.Button
        Get
            Return BTN_Magazzini
        End Get
        Set(value As Global.System.Web.UI.WebControls.Button)
            BTN_Magazzini = value
        End Set
    End Property

    Public Property Property_BTN_ComboSpecie() As Global.System.Web.UI.WebControls.Button
        Get
            Return BTN_ComboSpecie
        End Get
        Set(value As Global.System.Web.UI.WebControls.Button)
            BTN_ComboSpecie = value
        End Set
    End Property

    Public Property Property_BTN_ComboDisciplinari() As Global.System.Web.UI.WebControls.Button
        Get
            Return BTN_ComboDisciplinari1
        End Get
        Set(value As Global.System.Web.UI.WebControls.Button)
            BTN_ComboDisciplinari1 = value
        End Set
    End Property

    Public Property Property_Box_Salva() As Global.System.Web.UI.WebControls.Panel
        Get
            Return Box_Salva
        End Get
        Set(value As Global.System.Web.UI.WebControls.Panel)
            Box_Salva = value
        End Set
    End Property

    Public Property Property_CBL_Consigli() As Global.System.Web.UI.WebControls.CheckBoxList
        Get
            Return CBL_Consigli
        End Get
        Set(value As Global.System.Web.UI.WebControls.CheckBoxList)
            CBL_Consigli = value
        End Set
    End Property

    Public Property Property_ComboSpecie() As ComboSpecie
        Get
            Return ComboSpecie
        End Get
        Set(value As ComboSpecie)
            ComboSpecie = value
        End Set
    End Property

    Public Property Property_txtUsernameCreazione() As TextBox
        Get
            Return txtUsernameCreazione
        End Get
        Set(value As TextBox)
            txtUsernameCreazione = value
        End Set
    End Property

    Public Property Property_divUsernameCreazione() As Panel
        Get
            Return divUsernameCreazione
        End Get
        Set(value As Panel)
            divUsernameCreazione = value
        End Set
    End Property

    Public Property Property_txtImpresa() As TextBox
        Get
            Return txtImpresa
        End Get
        Set(value As TextBox)
            txtImpresa = value
        End Set
    End Property

    Public Property Property_divImpresa() As Panel
        Get
            Return divImpresa
        End Get
        Set(value As Panel)
            divImpresa = value
        End Set
    End Property

    Public Property Property_divDisciplinare() As Panel
        Get
            Return Div_Disciplinare
        End Get
        Set(value As Panel)
            Div_Disciplinare = value
        End Set
    End Property

    Public Property Property_divOperazione() As Panel
        Get
            Return divOperazione
        End Get
        Set(value As Panel)
            divOperazione = value
        End Set
    End Property

    Public Property Property_divPosizione() As Panel
        Get
            Return divPosizione
        End Get
        Set(value As Panel)
            divPosizione = value
        End Set
    End Property

    Public Property Property_txtPosizione As String
        Get
            Return txtPosizione.Text
        End Get
        Set(value As String)
            txtPosizione.Text = value
        End Set
    End Property

    Public Property GisAttivo As Boolean
        Get
            Return GisSmartBS.Attivo
        End Get
        Set(value As Boolean)
            GisSmartBS.Attivo = value
        End Set
    End Property


    'Public Property Property_ImgBtn_Costi() As Global.System.Web.UI.WebControls.ImageButton
    '    Get
    '        Return ImgBtn_Costi
    '    End Get
    '    Set(value As Global.System.Web.UI.WebControls.ImageButton)
    '        ImgBtn_Costi = value
    '    End Set
    'End Property





    Public Property Property_ComboMagazzini() As ComboMagazzini
        Get
            Return ComboMagazzini
        End Get
        Set(value As ComboMagazzini)
            ComboMagazzini = value
        End Set
    End Property

    Public Property Property_ComboCentroAziendale() As ComboCentroAziendale
        Get
            Return ComboCentroAziendale
        End Get
        Set(value As ComboCentroAziendale)
            ComboCentroAziendale = value
        End Set
    End Property

    Public Property Property_ComboOperazione() As AgronicaControlli_2010.ComboOperazioni
        Get
            Return ComboOperazione
        End Get
        Set(value As AgronicaControlli_2010.ComboOperazioni)
            ComboOperazione = value
        End Set
    End Property

    Public Property Property_ComboDisciplinari() As AgronicaControlli_2010.ComboDisciplinari
        Get
            Return ComboDisciplinari1
        End Get
        Set(value As AgronicaControlli_2010.ComboDisciplinari)
            ComboDisciplinari1 = value
        End Set
    End Property

    Public Property Property_GridView_Impianti() As Global.System.Web.UI.WebControls.GridView
        Get
            Return GridView_Impianti
        End Get
        Set(value As Global.System.Web.UI.WebControls.GridView)
            GridView_Impianti = value
        End Set
    End Property

    Public Property Property_ImgBtn_Salva() As Global.System.Web.UI.WebControls.ImageButton
        Get
            Return ImgBtn_Salva
        End Get
        Set(value As Global.System.Web.UI.WebControls.ImageButton)
            ImgBtn_Salva = value
        End Set
    End Property


    Public Property Property_txt_Note() As Global.System.Web.UI.WebControls.TextBox
        Get
            Return Txt_Note
        End Get
        Set(value As Global.System.Web.UI.WebControls.TextBox)
            Txt_Note = value
        End Set
    End Property

    Public Property Property_txt_DataOperazione() As Global.System.Web.UI.WebControls.TextBox
        Get
            Return txt_DataOperazione
        End Get
        Set(value As Global.System.Web.UI.WebControls.TextBox)
            txt_DataOperazione = value
        End Set
    End Property


    Public Property Property_AggiornaGrigliaImpianti() As Global.System.Web.UI.WebControls.Button
        Get
            Return AggiornaGrigliaImpianti
        End Get
        Set(value As Global.System.Web.UI.WebControls.Button)
            AggiornaGrigliaImpianti = value
        End Set
    End Property

    Public Property Property_UpdatePanelPerScript() As Global.System.Web.UI.UpdatePanel
        Get
            Return UpdatePanelPerScript
        End Get
        Set(value As Global.System.Web.UI.UpdatePanel)
            UpdatePanelPerScript = value
        End Set
    End Property


    Public Property Property_Div_ProvenienzaRisorse() As Global.System.Web.UI.HtmlControls.HtmlGenericControl
        Get
            Return ProvenienzaRisorse
        End Get
        Set(value As Global.System.Web.UI.HtmlControls.HtmlGenericControl)
            ProvenienzaRisorse = value
        End Set
    End Property



    Public Property Property_Div_Specie() As Global.System.Web.UI.HtmlControls.HtmlGenericControl
        Get
            Return Div_Specie
        End Get
        Set(value As Global.System.Web.UI.HtmlControls.HtmlGenericControl)
            Div_Specie = value
        End Set
    End Property

    ''da Agenda.Master
    'Public Property Property_Lbl_Titolo() As Global.System.Web.UI.WebControls.Label
    '    Get
    '        Return CType(Me.Master, Agenda).Property_Lbl_Titolo
    '    End Get
    '    Set(value As Global.System.Web.UI.WebControls.Label)
    '        CType(Me.Master, Agenda).Property_Lbl_Titolo = value
    '    End Set
    'End Property

    ''da Agenda.Master
    'Public Property Property_ImgBtn_AnnullaTutto() As Global.System.Web.UI.WebControls.ImageButton
    '    Get
    '        Return CType(Me.Master, Agenda).Property_ImgBtn_AnnullaTutto
    '    End Get
    '    Set(value As Global.System.Web.UI.WebControls.ImageButton)
    '        CType(Me.Master, Agenda).Property_ImgBtn_AnnullaTutto = value
    '    End Set
    'End Property

    Public Property Property_NoteGiustPanel() As Global.System.Web.UI.WebControls.Panel
        Get
            Return Me.NoteGiustPanel
        End Get
        Set(value As Global.System.Web.UI.WebControls.Panel)
            Me.NoteGiustPanel = value
        End Set
    End Property


    Public Property Property_ImgBtn_DDT() As Global.System.Web.UI.WebControls.ImageButton
        Get
            Return ImgBtn_DDT
        End Get
        Set(value As Global.System.Web.UI.WebControls.ImageButton)
            ImgBtn_DDT = value
        End Set
    End Property

    Public Property Property_ImgBtn_DDT_Cancella() As Global.System.Web.UI.WebControls.ImageButton
        Get
            Return ImgBtn_DDT_Cancella
        End Get
        Set(value As Global.System.Web.UI.WebControls.ImageButton)
            ImgBtn_DDT_Cancella = value
        End Set
    End Property

    Public Property Property_ImgBtn_CheckDPI() As Global.System.Web.UI.WebControls.ImageButton
        Get
            Return ImgBtn_CheckDPI
        End Get
        Set(value As Global.System.Web.UI.WebControls.ImageButton)
            ImgBtn_CheckDPI = value
        End Set
    End Property

    Public Property Property_Lbl_ProvenienzaRisorse() As Global.System.Web.UI.WebControls.Label
        Get
            Return lblProvenienzaRisorse
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            lblProvenienzaRisorse = value
        End Set
    End Property

    Public Property Property_Div_ImgBtn_CheckDPI() As Global.System.Web.UI.HtmlControls.HtmlGenericControl
        Get
            Return ImgBtn_CheckDPI_div
        End Get
        Set(value As Global.System.Web.UI.HtmlControls.HtmlGenericControl)
            ImgBtn_CheckDPI_div = value
        End Set
    End Property

    Public Property Property_Div_ImgBtn_DoseConsigliata() As Global.System.Web.UI.HtmlControls.HtmlGenericControl
        Get
            Return ImgBtn_DoseConsigliata_div
        End Get
        Set(value As Global.System.Web.UI.HtmlControls.HtmlGenericControl)
            ImgBtn_DoseConsigliata_div = value
        End Set
    End Property

    Public Property Property_Div_ImgBtn_DoseConsigliataDettaglio() As Global.System.Web.UI.HtmlControls.HtmlGenericControl
        Get
            Return ImgBtn_DoseConsigliataDettaglio_div
        End Get
        Set(value As Global.System.Web.UI.HtmlControls.HtmlGenericControl)
            ImgBtn_DoseConsigliataDettaglio_div = value
        End Set
    End Property

    Public Property Property_Div_ImgBtn_Carico_Eff() As Global.System.Web.UI.HtmlControls.HtmlGenericControl
        Get
            Return ImgBtn_Carico_Eff_div
        End Get
        Set(value As Global.System.Web.UI.HtmlControls.HtmlGenericControl)
            ImgBtn_Carico_Eff_div = value
        End Set
    End Property

    Public Property Property_hf_esistonoCostiCollegatiCDG() As String
        Get
            Return hf_esistonoCostiCollegatiCDG.Value
        End Get
        Set(value As String)
            hf_esistonoCostiCollegatiCDG.Value = value
        End Set
    End Property

    Public Property caricaDefaultUtente_Disciplinare As Boolean
    Public Property caricaDefaultAziendale_Disciplinare As Boolean

    Public Property Ricetta_Operazione_Cod1 As String
        Get
            Return ricetta_operazione_cod
        End Get
        Set(value As String)
            ricetta_operazione_cod = value
        End Set
    End Property


#End Region






    Private Sub dgrScaricoAvanzati_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles dgrScaricoAvanzati.RowCommand

        AggiornaCostiAccessoriAvanzati_Click(Me, Nothing)

        Dim IndiceRigaGriglia As Integer = 0
        Dim Dr As DataRow

        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Select Case e.CommandName

            Case "EliminaCosto"

                If Session("dtScarico") IsNot Nothing Then

                    Dim dtScarico As DataTable = Session("dtScarico")

                    If dtScarico.Rows.Count >= IndiceRigaGriglia Then

                        'Trovo la riga da cancellare    (chiave = FrCod)
                        Dr = dtScarico.Rows(IndiceRigaGriglia)

                        'Elimino la riga
                        Dr.Delete()
                        'Salvo il DataTable dentro il Session
                        Session("dtScarico") = dtScarico

                    End If

                End If

        End Select


        AggiornaDgrScaricoAvanzati()
        'AggiornaDgrScarico()
        AggiornaGridViewCostiAccessoriVisibili()
        AggiornaMagazzino()

        'se sono nella griglia pupup dei costidrg scarico (come in questo caso) non è necessario aggiornare anche la lista dei costi
        'prersente nei movimenti dell'agenda, dato che viene ricreata quando si seleziona salva_costi_accessori,
        'mentre se si seleziona annulla allora viene ricaricata in sessione la tabella old quindi 
        'i movimenti non devono essere toccati
        'se invece sono nella griglia a fondo pagina gridviewcostiaccessorivisibili allora se seleziono cancella 
        'oltre che a modificare la tabella dei costi in sessione Session("dtScarico") che non verrà mai ripristinata
        'dalla versione precedente, dato che l'operazione non è annullabile o confermabile, devo 
        'agire anche sulla lista dei movimenti eliminando il movimento 
        'corrispondente alla riga selezionata. Se non lo faccio non ho più corrispondenza tra la tabella in sessione e 
        'la lista movimenti e dato che la prima corrisponde di solito a ciò che vedo, mentre la seconda 
        'corrisponde a ciò che viene salvato mi trovo a salvare cose diverse da quello che vedo
    End Sub


    Private Sub btnApriContattiCosti_Click(sender As Object, e As System.EventArgs) Handles btnApriContattiCosti.Click

        Dim tipo As String = hdKendo_CostiAccessori_ComboHelper_Comando.Value

        Select Case tipo
            Case -1
                ApriMacchinaCosti_BS(objParametriAgenda.Piva)

            Case -2
                ApriConttattiCosti(objParametriAgenda.Piva, COD_LEGALE)


        End Select

    End Sub

    Public Sub ApriConttattiCosti(ByVal piva As String, ByVal TipoRapp_cont As Integer)

        Dim StrWindowOpen As String

        Dim UtenteAbilitato_Modifica As Boolean = Permessi.getPermesso(enum_Security_Attivita.Anagrafica_Contatto).Scrittura


        If UtenteAbilitato_Modifica Then

            Dim xOperazione As String = enum_TipoOperazioneDB.Scrittura
            Dim codRisum As String = "0"
            Dim codContatto As String = "0"

            If hdKendo_CostiAccessori_ComboHelper_Comando_Dati.Value.Contains("Apri") Then

                xOperazione = enum_TipoOperazioneDB.Modifica
                codRisum = ApriRisorsa_EstraiCodice()

                Dim xLeggiContatto As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
                Dim dtXLeggiContatto As DataTable =
                    xLeggiContatto.Leggi(objParametriAgenda.Piva, "", codRisum, 0, 0, "", True, True, "", "", objParametri_Server)

                If dtXLeggiContatto.Rows.Count > 0 Then
                    codContatto = dtXLeggiContatto.Rows(0)("Cod_Contatto")
                    TipoRapp_cont = dtXLeggiContatto.Rows(0)("Cod_Rapporto")
                End If


            End If


            Dim QueryString As String
            QueryString = "?o=" &
                            Stringa_Codifica(xOperazione, AgroKey_EncoderDecoder, Server) &
                            "&tipo_rapporto=" &
                            Stringa_Codifica(TipoRapp_cont, AgroKey_EncoderDecoder, Server) &
                            "&lav_cod=" &
                            Stringa_Codifica(objParametriAgenda.Lav_Cod, AgroKey_EncoderDecoder, Server) &
                            "&piva=" &
                            Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                            "&orig=" &
                            Stringa_Codifica("", AgroKey_EncoderDecoder, Server) &
                            "&codcont=" &
                            Stringa_Codifica(codContatto, AgroKey_EncoderDecoder, Server) &
                            "&dialog=" &
                            Stringa_Codifica("true", AgroKey_EncoderDecoder, Server)

            'URL VECCHIO: "../GestioneContatti/Contatto.aspx"
            StrWindowOpen = UtilityProvider.JqueryModalDialogScript(
                     "../Anagrafica/New_Contatto_Edit.aspx", QueryString, UpdatePanelPerScript.ClientID,
                    550, 850, 0, 0,
                    , , , , , , NomeForm:="aspnetForm")

        Else

            StrWindowOpen = "alert('Non si dispone del permesso necessario per creare un nuovo contatto');"

        End If

        Dim tags As Boolean = True
        ScriptManager.RegisterStartupScript(UpdatePanelPerScript,
                                            UpdatePanelPerScript.GetType(),
                                            String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, tags)

    End Sub


    Public Sub ApriMacchinaCosti_BS(ByVal piva As String)

        Dim xOperazione As String = enum_TipoOperazioneDB.Scrittura


        Dim xMac_Cod As String = ""

        If hdKendo_CostiAccessori_ComboHelper_Comando_Dati.Value.Contains("Apri") Then


            xOperazione = enum_TipoOperazioneDB.Modifica

            xMac_Cod = ApriRisorsa_EstraiCodice()

        End If

        Dim QueryString As String
        QueryString = "?o=" &
                        Stringa_Codifica(xOperazione, AgroKey_EncoderDecoder, Server) &
                        "&m=" & Stringa_Codifica(xMac_Cod, AgroKey_EncoderDecoder, Server) &
                        "&modalBS=1"

        Dim UtenteAbilitato_Modifica As Boolean = Permessi.getPermesso(enum_Security_Attivita.Anagrafica_ParcoMacchine).Scrittura

        Dim StrWindowOpen As String
        If UtenteAbilitato_Modifica Then
            StrWindowOpen = UtilityProvider.JqueryModalDialogScript(
                 "../Anagrafica/macchina_Edit.aspx", QueryString, UpdatePanelPerScript.ClientID,
                550, 850, 0, 0,
                , , , , , , NomeForm:="aspnetForm")
        Else
            StrWindowOpen = "alert('Non si dispone del permesso richiesto per gestire il parco macchine.');"
        End If


        'StrWindowOpen = StrWindowOpen & "<script language='javascript'> " & _
        '         "  clickButtonContatti() " & _
        '         "</script>"
        'Dim StrWindowOpen As String = AgronicaCoreDataProvider.UtilityProvider.Page_NewWindow_RitornaJavascript(Nothing, _
        '        "../GestioneContatti/Contatto.aspx", QueryString, "", _
        '         500, 800, 0, 0)

        Dim tags As Boolean = True
        ScriptManager.RegisterStartupScript(UpdatePanelPerScript,
                                            UpdatePanelPerScript.GetType(),
                                            String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, tags)
    End Sub

    Private Function ApriRisorsa_EstraiCodice() As String
        Dim xMac_Cod As String
        Dim xChiave As String = ""
        xChiave = hdKendo_CostiAccessori_ComboHelper_Comando_Dati.Value.Split(":")(1)
        Dim vChiavi As String() = xChiave.Split("*")
        xMac_Cod = vChiavi(vChiavi.Length - 1)
        Return xMac_Cod
    End Function

    Public Sub ApriMacchinaCosti(ByVal piva As String)

        Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
        objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.ParcoMacchine_Edit
        objGiasOnline.Piva = objParametriAgenda.Piva
        objGiasOnline.Operazione = enum_TipoOperazioneDB.Scrittura

        Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                  Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                  objGiasOnline)


        Dim StrWindowOpen As String = AgronicaCoreDataProvider.UtilityProvider.JqueryModalDialogScript(
         strJS, "", UpdatePanelPerScript.ClientID,
        550, 850, 0, 0,
        , , , , , , NomeForm:="aspnetForm")

        'StrWindowOpen = StrWindowOpen & "<script language='javascript'> " & _
        '         "  clickButtonContatti() " & _
        '         "</script>"
        'Dim StrWindowOpen As String = AgronicaCoreDataProvider.UtilityProvider.Page_NewWindow_RitornaJavascript(Nothing, _
        '        "../GestioneContatti/Contatto.aspx", QueryString, "", _
        '         500, 800, 0, 0)

        Dim tags As Boolean = True
        ScriptManager.RegisterStartupScript(UpdatePanelPerScript,
                                            UpdatePanelPerScript.GetType(),
                                            String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, tags)

        'ScriptManager.RegisterStartupScript(UpdatePanelPerScript, _
        '                                    UpdatePanelPerScript.GetType(), _
        '                                    "jQuery_{0}", strJS, False)

    End Sub

    Protected Sub SalvaCostiDefault_Click(sender As Object, e As EventArgs) Handles SalvaCostiDefault.Click

        SalvaCostiAccessori_SuAgendaMovimenti(False, Nothing)

        Dim dtScarico As DataTable
        Dim i As Integer = 0

        If Session("dtScarico") IsNot Nothing Then

            dtScarico = Session("dtScarico")

            Dim val2save As String = ""
            Dim strMacCod As String = ""
            Dim strContCod As String = ""

            val2save += "lav_cod=" & objParametriAgenda.Lav_Cod

            For i = 0 To dtScarico.Rows.Count - 1
                Select Case dtScarico.Rows(i).Item("elem_cod")
                    Case MACCHINE
                        strMacCod &= dtScarico.Rows(i).Item("mat_cod") & ","
                    Case 0
                        strContCod &= dtScarico.Rows(i).Item("mat_cod") & ","
                End Select
            Next

            If strMacCod <> "" Then
                strMacCod = "|mac_cod={" & strMacCod.Substring(0, strMacCod.Length - 1) & "}"
                val2save += strMacCod
            End If

            If strContCod <> "" Then
                strContCod = "|cod_cont={" & strContCod.Substring(0, strContCod.Length - 1) & "}"
                val2save += strContCod
            End If

            Dim objProf_W As New AgronicaCoreProfilazioneBIZ.Profilazione_W
            If (objProf_W.Scrivi_Inserisce_O_Aggiorna(objParametriAgenda.Piva,
                                                      objParametriAgenda.Lav_Cod,
                                                      "macXlav",
                                                      objParametri_Server.PivaSuperUser,
                                                      "Macchine/Contatti per operazioni", val2save,
                                                      AGRODATAINIZIO, AGRODATAFINE,
                                                      objParametriAgenda.Lav_Cod,
                                                      0,
                                                      objParametri_Server)) Then

                hdKendo_CostiAccessori_Ricarica.Value = "profiloOk"
            Else
                hdKendo_CostiAccessori_Ricarica.Value = "profiloNo"

            End If

        End If


    End Sub


    Private Sub BottoneNascostoContatti_Click(sender As Object, e As System.EventArgs) Handles BottoneNascostoContatti.Click

        Popola_Manodopera()
        Popola_Terzisti()
        Popola_TecnicoResponsabile()

        If Not IsNothing(Session("IndiceRigaGrigliaCliccato")) AndAlso IsNumeric(Session("IndiceRigaGrigliaCliccato")) Then
            dgrCentriCosto.SelectedIndex = CInt(Session("IndiceRigaGrigliaCliccato"))
            dgrCentriCosto_selezionato_tipo(True)
            Session("IndiceRigaGrigliaCliccato") = Nothing
        Else
            If dgrCentriCosto.SelectedIndex >= 0 Then
                dgrCentriCosto_selezionato_tipo(True)
            End If
        End If



    End Sub

    Private Sub impostaDisciplinareDaPreferenza(preferenza As String)
        'impostatodisciplinarepredefinito = True
        If ComboDisciplinari1.ddl_Disciplinari.Visible = True Then

            Session("DpiPredefinitoUtente") = "0"
            Session("DpiFertPredefinitoUtente") = "0"

            Select Case preferenza
                Case "0"
                    ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 0
                    objParametriAgenda.Disciplinare = ComboDisciplinari1.Valore_Combo
                    ViewState("ImpostoDPINellaCombo") = False
                Case Else
                    'salvo in viewstate un valore per indicare che devo impostare il disciplinare,
                    'usato nella caricacombo quando viene ricaricata a seguito di modifiche

                    Dim x As New AgronicaCoreDpiBIZ.CaricaListControl

                    Dim disciplinare As String = preferenza

                    If disciplinare.Contains("e:") Then
                        'Caso nuovo salvo idEnte
                        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(objParametriAgenda.Data, objParametriAgenda.Data)

                        Dim disciplinare_cod As String = ""
                        Dim fp As String = ""

                        x.Trova_Disciplinare_Ente(disciplinare.Split("/")(0).Split(":")(1),
                                                              disciplinare.Split("/")(1).Split(":")(1),
                                                              False, "", "", Session, objParametri_Server, objParametri_Utenti,
                                                              0, 0, 0, 0, True, True, False,
                                                   New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = True, .Flag_DisciplinareAttivo = True}, disciplinare_cod, fp)

                        objParametri_Server.ResettaFinestra()

                        disciplinare = disciplinare_cod & "/" & fp

                    End If

                    ViewState("ImpostoDPINellaCombo") = True
                    Session("DpiPredefinitoUtente") = disciplinare

                    Select Case objParametriAgenda.Lav_Cod

                        Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                            If CInt(disciplinare.Split("/")(0)) > 0 Then

                                'leggo il cod_regolamento_pua associato
                                Dim PUA_Regolamento_Cod As Integer = 0
                                Dim agroWs As String
                                Dim Dati As String
                                Dim strErr As String = ""
                                Dim XmlDocumento As New System.Xml.XmlDocument
                                Dim XmlNodo As System.Xml.XmlNodeList
                                Dim XmlElemento As System.Xml.XmlElement
                                Dim ObjDownloadWs As WS_Disciplinari.AgroWS_Disciplinari
                                If IsNothing(ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari")) Then
                                    'creo l'agrowebconfig
                                    Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                                    agroWs = objAgroWeb.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
                                Else
                                    agroWs = ConfigurationManager.AppSettings("GiasOnline.WS_Disciplinari.AgroWS_Disciplinari").ToString()
                                End If

                                ObjDownloadWs = New WS_Disciplinari.AgroWS_Disciplinari
                                Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                                objWs.NewWS(ObjDownloadWs, agroWs, objParametri_Utenti)

                                If disciplinare.Contains("e:") Then

                                    objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(objParametriAgenda.Data, objParametriAgenda.Data)

                                    Dim disciplinare_cod As String = ""
                                    Dim fp As String = ""

                                    x.Trova_Disciplinare_Ente(disciplinare.Split("/")(0).Split(":")(1),
                                                              disciplinare.Split("/")(1).Split(":")(1),
                                                              False, "", "", Session, objParametri_Server, objParametri_Utenti,
                                                              0, 0, 0, 0, True, True, False,
                                                   New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = True, .Flag_DisciplinareAttivo = True}, disciplinare_cod, fp)

                                    objParametri_Server.ResettaFinestra()

                                    disciplinare = disciplinare_cod & "/" & fp

                                End If


                                'Richiamo il disciplinare pubblico
                                Dati = ObjDownloadWs.Leggi_Disciplinari2(CInt(disciplinare.Split("/")(0)),
                                        0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                        CStr(Session("ASG_Utente_Username_Crypt").ToString),
                                        CStr(Session("ASG_Utente_Password_Crypt").ToString),
                                        strErr)
                                ObjDownloadWs.Dispose()
                                If strErr = "" Then
                                    If Not IsNothing(Dati) AndAlso Dati.ToLower.IndexOf("errore") < 0 Then
                                        XmlDocumento.LoadXml(Dati)
                                        XmlNodo = XmlDocumento.GetElementsByTagName("Record")
                                        If XmlNodo IsNot Nothing Then
                                            For Each XmlElemento In XmlNodo
                                                PUA_Regolamento_Cod = CInt(XmlElemento.GetAttribute("pua_regolamento_cod"))
                                                Session("DpiFertPredefinitoUtente") = PUA_Regolamento_Cod
                                            Next
                                        End If
                                    End If
                                End If

                                If PUA_Regolamento_Cod <> 0 Then
                                    If ComboDisciplinari1.ddl_Disciplinari.Items.Count > 1 Then
                                        ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 0
                                        For kk = 0 To ComboDisciplinari1.ddl_Disciplinari.Items.Count - 1
                                            Dim val As String() = ComboDisciplinari1.ddl_Disciplinari.Items(kk).Value.Split("/")
                                            If val(0) = PUA_Regolamento_Cod Then
                                                ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                            End If
                                        Next
                                        objParametriAgenda.Disciplinare = ComboDisciplinari1.Valore_Combo
                                    End If
                                End If
                            Else

                                If ComboDisciplinari1.ddl_Disciplinari.Items.Count > 1 Then
                                    ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 0
                                    For kk = 0 To ComboDisciplinari1.ddl_Disciplinari.Items.Count - 1
                                        Dim val As String() = ComboDisciplinari1.ddl_Disciplinari.Items(kk).Value.Split("/")
                                        If val.Length = 1 Then
                                            ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                        End If
                                    Next
                                    objParametriAgenda.Disciplinare = ComboDisciplinari1.Valore_Combo
                                    Session("DpiFertPredefinitoUtente") = ComboDisciplinari1.Valore_Combo
                                End If

                            End If

                        Case Else

                            If ComboDisciplinari1.ddl_Disciplinari.Items.Count > 1 Then
                                ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 0
                                Dim selected As Boolean = False
                                For kk = 0 To ComboDisciplinari1.ddl_Disciplinari.Items.Count - 1
                                    Dim val As String() = ComboDisciplinari1.ddl_Disciplinari.Items(kk).Value.Split("/")
                                    If val(0) = disciplinare.Split("/")(0) Then
                                        If val.Length = 1 Then
                                            ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                            selected = True
                                            'Exit For
                                        End If
                                        If val.Length = 5 AndAlso disciplinare.Split("/").Length = 2 Then
                                            If val(4) = disciplinare.Split("/")(1) Then
                                                ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                                selected = True
                                                'Exit For
                                            End If
                                        End If
                                    End If
                                Next

                                If Not selected AndAlso disciplinare.Split("/").Length > 1 Then

                                    Dim ente_cod As String = ""
                                    Dim fp As String = ""

                                    x.Trova_Ente_Disciplinare(disciplinare, False, "", "",
                                                                   Session, objParametri_Server, objParametri_Utenti,
                                                                   0, 0, 0, 0, True, True, False,
                                                                   New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = True, .Flag_DisciplinareAttivo = True},
                                                                   ente_cod,
                                                                   fp)

                                    If ente_cod <> "" Then

                                        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(objParametriAgenda.Data, objParametriAgenda.Data)

                                        Dim disciplinare_cod As String = ""

                                        x.Trova_Disciplinare_Ente(ente_cod, fp, False, "", "",
                                                                       Session, objParametri_Server, objParametri_Utenti,
                                                                       0, 0, 0, 0, True, True, False,
                                                       New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = True, .Flag_DisciplinareAttivo = True}, disciplinare_cod, fp)

                                        objParametri_Server.ResettaFinestra()

                                        disciplinare = disciplinare_cod & "/" & fp

                                        For kk = 0 To ComboDisciplinari1.ddl_Disciplinari.Items.Count - 1
                                            Dim val As String() = ComboDisciplinari1.ddl_Disciplinari.Items(kk).Value.Split("/")
                                            If val(0) = disciplinare.Split("/")(0) Then
                                                If val.Length = 1 Then
                                                    ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                                    'Exit For
                                                End If
                                                If val.Length = 5 AndAlso disciplinare.Split("/").Length = 2 Then
                                                    If val(4) = disciplinare.Split("/")(1) Then
                                                        ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = kk
                                                        'Exit For
                                                    End If
                                                End If
                                            End If
                                        Next

                                    End If

                                End If


                                If objParametriAgenda.Disciplinare = "" Then
                                    objParametriAgenda.Disciplinare = ComboDisciplinari1.Valore_Combo
                                Else
                                    If ComboDisciplinari1.ddl_Disciplinari.Items.FindByValue(objParametriAgenda.Disciplinare) IsNot Nothing AndAlso objParametriAgenda.Disciplinare <> "0" Then
                                        ComboDisciplinari1.ddl_Disciplinari.Text = objParametriAgenda.Disciplinare
                                        ComboDisciplinari1.Valore_Combo = objParametriAgenda.Disciplinare
                                    End If
                                End If


                            End If

                    End Select

            End Select

        Else
            'ComboDisciplinari1.ddl_Disciplinari.SelectedIndex = 0
            objParametriAgenda.Disciplinare = "0"
            ViewState("ImpostoDPINellaCombo") = False

        End If
    End Sub

    Public Sub Btn_Aggiorna_Magazzino_Click(sender As Object, e As EventArgs) Handles Btn_Aggiorna_Magazzino.Click
        ComboMagazzini.Valore_Combo = objParametriAgenda.Fabbricato
        ComboMagazzini.ddl_Magazzini.SelectedIndex = ComboMagazzini.ddl_Magazzini.Items.IndexOf(ComboMagazzini.ddl_Magazzini.Items.FindByValue(objParametriAgenda.Fabbricato))
    End Sub


End Class