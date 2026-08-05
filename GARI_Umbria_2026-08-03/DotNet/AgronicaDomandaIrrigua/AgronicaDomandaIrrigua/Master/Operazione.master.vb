Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider.Sicurezza

Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreContabBIZ

Partial Class Operazione

    Inherits System.Web.UI.MasterPage
    Dim objParametriAgenda As ParametriAgenda
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim ricetta_cod As String = ""
    Dim Ricetta_Operazione_Cod As String = ""
    Dim Ricetta_Tipo As String = ""
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
    Dim CONSIDERATERRENONUDO As Boolean
    Dim DETTAGLITERRENONUDO As Boolean

    Dim ALGORITMO_COSTI_ACCESSORI As Integer
    Dim ComboModificata As String = ""

    Public objParametriAgenda_TargetOperazione As String

    Public Property flag_MostraBtnSalvaCDG As Boolean

    Private Sub CaricaObjParametri()

        'Leggo i parametri
        objParametriAgenda = New ParametriAgenda
        'objParametriAgenda.Leggi()

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
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
        Session.Remove("Dt_TuttiProdotti")
    End Sub


    Public Sub SelezionaAspettoPagina()

        'TODO: Vanni, 22/09/2016 16:34:35: impostare la classe e poi decommentare qui.
        Exit Sub

        If objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta Then


            Dim script As New StringBuilder


            script.AppendLine("$(document).ready(function () { ")

            script.AppendLine("$('.ui-widget-header').addClass('RicetteTile');")

            script.AppendLine("});")


            ScriptManager.RegisterClientScriptBlock(UpdatePanelImpianti, UpdatePanelImpianti.GetType(),
                                         String.Format("jQuery_{0}", UpdatePanelImpianti.ClientID), script.ToString, True)
        End If


    End Sub

    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        'per le operazioni con logica modificata e utilizzo di page_operazione e modello operazioni colturali 
        'la master deve essere modificata, noin voglio che interagista coon i parametri operazioni
        'e setti i valori, a questo ci pensa la page_operazione, che aventualmente interagisce con le property della master,
        'la master diventa solo un oggetto per rappresentazione, non deve contenere logica, uno strumentop passivo della page_operazione,
        'usata solo per raggruppare controlli e struttura html comune a molte pagine.
        'per questo nelel operazioni nuove i controlli devono essere pronti gia prima della sua load, quindi nella init della master,
        'per ora rimane una fortma ibrida, piu si sbuoterà la master il piu possibile
        CaricaObjParametri()
        Session("Disciplinare_Attivo") = False

        If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
            objParametriAgenda_TargetOperazione = "Enum_TargetOperazione.Reale"
        Else
            objParametriAgenda_TargetOperazione = "Enum_TargetOperazione.Planning"
        End If

        flag_MostraBtnSalvaCDG = False

        'Leggo l'impostazione per il tipo di algoritmo
        If IsNothing(Session("Agenda_ALGORITMO_COSTI_ACCESSORI")) Then
            'Leggo l'impostazione
            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
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

        SelezionaAspettoPagina()


        If Not IsPostBack Then
            Session("UtilizzataRicetta") = False
            Session("PrimaVolta") = True


            'Filtro_MaterialiAvanzati.Visible = False
            'Filtro_Materiali.Visible = False
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
                LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                LAVCOD_IRRIGAZIONE,
                LAVCOD_RILIEVO_ERBE_INFESTANTI,
                LAVCOD_RILIEVO_PIOGGE

                CONSIDERATERRENONUDO = False
                DETTAGLITERRENONUDO = False

            Case Else

                CONSIDERATERRENONUDO = True
                DETTAGLITERRENONUDO = True

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

        Select Case objParametriAgenda.Lav_Cod
            Case LAVCOD_RILIEVO_PIOGGE,
                 LAVCOD_TRATTAMENTO_POST_RACCOLTA,
                 LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE,
                 LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                 LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO,
                 LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO

                DivCampo.Visible = False
                DivVarieta.Visible = False

        End Select


        If objParametriAgenda.Lav_Cod = LAVCOD_REINNESCO_TRAPPOLE Or
             objParametriAgenda.Lav_Cod = LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE Or
             objParametriAgenda.Lav_Cod = LAVCOD_SEMINA Or
             objParametriAgenda.Lav_Cod = LAVCOD_TRAPIANTO Or
             objParametriAgenda.Lav_Cod = LAVCOD_SOVESCIO Or
             objParametriAgenda.Lav_Cod = LAVCOD_SOD_SEDDING Then

            'CaricaObjParametri()

            ''Aggiunta degli script
            If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
                ScriptGridImpianti()
            Else
                ScriptGridPlanning()
            End If

            InizializzaData()
            InizializzaVarie()
            ScriptCostiAccessoriEtAl()

            Select Case objParametriAgenda.Lav_Cod
                Case LAVCOD_INSTALLAZIONE_TRAPPOLE,
                     LAVCOD_CONFUSIONE_SESSUALE,
                     LAVCOD_DISORIENTAMENTO_SESSUALE,
                     LAVCOD_CATTURE_MASSA,
                     LAVCOD_DISTRIBUZIONE_INSETTI

                    SupTrattata = False
            End Select


            If Not IsPostBack Then


                CreaImpostazioniColonne()
                ImpostazioniColonne()


                'Centro Aziendale
                AggiornaCentroAziendale()


                'Data
                txt_DataOperazione.Text = objParametriAgenda.Data

                'Specie
                AggiornaSpecie()

                'carico le operazioni
                AggiornaOperazioni()

                If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
                    CaricaGriglia_Impianti()
                Else
                    CaricaGriglia_Planning()
                End If

                ImpostaPannellibyOperazione()

                Carica_Note()

                CaricaCostiAccessori()

                ''Lettura dei Default
                DefaultUtente()

                DefaultAziendali()

                'Magazzini
                'AggiornaMagazzino()

                If objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Scrittura Then
                    DefaultAziendali()
                Else
                    AggiornaMagazzino()
                End If

                'Attivo disattivo btn Magazzino
                If ComboMagazzini.Valore_Combo <> "0" Then
                    ImgBtn_Carico.Visible = True
                Else
                    ImgBtn_Carico.Visible = False
                End If

                'se ho già selezionato Data - Lavorazione e Specie posso caricare le ricette
                CaricaElencoRicette()

            End If
        End If


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

        If Not Dt_Impostazioni Is Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then

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

        GridView_Impianti.DataBind()

        'BoundFieldResource21.HeaderText = String.Format(BoundFieldResource21.HeaderText, UDM_DaUtente)


    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If Not IsPostBack Then
            CalcolaCosti.Checked = True
            'AttivaCostiAccessoriAvanzati()
        Else

        End If




        'per le operazioni con logica modificata e utilizzo di page_operazione e modello operazioni colturali 
        'la master deve essere modificata, noin voglio che interagista coon i parametri operazioni
        'e setti i valori, a questo ci pensa la page_operazione, che aventualmente interagisce con le property della master,
        'la master diventa solo un oggetto per rappresentazione, non deve contenere logica, uno strumentop passivo della page_operazione,
        'usata solo per raggruppare controlli e struttura html comune a molte pagine.
        'per questo nelel operazioni nuove i controlli devono essere pronti gia prima della sua load, quindi nella init della master,
        'per ora rimane una fortma ibrida, piu si sbuoterà la master il piu possibile
        CaricaObjParametri()

        If objParametriAgenda.Lav_Cod <> LAVCOD_REINNESCO_TRAPPOLE And
            objParametriAgenda.Lav_Cod <> LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE Then

            'CaricaObjParametri()

            ''Aggiunta degli script
            If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
                ScriptGridImpianti()
            Else
                ScriptGridPlanning()
            End If

            InizializzaData()
            InizializzaVarie()
            ScriptCostiAccessoriEtAl()

            Select Case objParametriAgenda.Lav_Cod
                Case LAVCOD_INSTALLAZIONE_TRAPPOLE,
                    LAVCOD_CONFUSIONE_SESSUALE,
                    LAVCOD_DISORIENTAMENTO_SESSUALE,
                    LAVCOD_CATTURE_MASSA

                    SupTrattata = False
            End Select


            If Not IsPostBack Then
                CreaImpostazioniColonne()
                ImpostazioniColonne()

                'Centro Aziendale
                AggiornaCentroAziendale()


                Dim dataMin As Date = AGRODATAINIZIO
                Dim dataMax As Date = AGRODATAFINE
                Dim SportelloAperto As Boolean = True
                If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Or objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
                    Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R
                    objPratiche.Data_Sportello_Da_Servizio(objParametriAgenda.Piva, enum_Servizi.Quaderno_Campagna_Caa, DateTime.Now, SportelloAperto, dataMin, dataMax, objParametri_Server, objParametri_Utenti)

                    objPratiche.Sportello_ChiamataSecondaria_SeNessunCambiamento(objParametriAgenda.Piva,
                                                                                        enum_Servizi.QuadernoCampagnaBio,
                                                                                        DateAndTime.Now,
                                                                                        True,
                                                                                        AGRODATAINIZIO,
                                                                                        AGRODATAFINE,
                                                                                        SportelloAperto,
                                                                                        dataMin,
                                                                                        dataMax,
                                                                                        objParametri_Server,
                                                                                        objParametri_Utenti)
                    If Not SportelloAperto Then
                        Box_Salva.Visible = False
                        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
                            Messaggi.AgroMsgBox(" Non è possibile inserire nessuna operazione, Sportello chiuso. ", Page, , UpdatePanelOperazione)
                        End If
                    End If

                    If objParametriAgenda.Data < dataMin Then
                        objParametriAgenda.Data = dataMin
                        txt_DataOperazione.Text = dataMin.ToShortDateString
                    End If

                    If objParametriAgenda.Data > dataMax Then
                        objParametriAgenda.Data = dataMax
                        txt_DataOperazione.Text = dataMax.ToShortDateString
                    End If
                End If

                'Data
                txt_DataOperazione.Text = objParametriAgenda.Data

                'Specie messo anche fuori
                AggiornaSpecie()

                'carico le operazioni
                AggiornaOperazioni()

                'DISCIPLINARI
                'AggiornaDisciplinari()

                'Filtro ricerca
                'AggiornaFiltroRicerca()

                If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
                    CaricaGriglia_Impianti()
                Else
                    CaricaGriglia_Planning()
                End If



                'SettaImpostazioneUtente_UDM()

                ImpostaPannellibyOperazione()

                Carica_Note()

                CaricaCostiAccessori()

                ''Lettura dei Default
                DefaultUtente()

                'Magazzini
                'AggiornaMagazzino()

                If objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Scrittura Then
                    DefaultAziendali()
                Else
                    AggiornaMagazzino()
                End If


                'Attivo disattivo btn Magazzino
                If ComboMagazzini.Valore_Combo <> "0" Then
                    ImgBtn_Carico.Visible = True
                Else
                    ImgBtn_Carico.Visible = False
                End If

                'se ho già selezionato Data - Lavorazione e Specie posso caricare le ricette
                CaricaElencoRicette()

            Else

            End If
        End If

        'Grilli: aggiungo il salva e vai ai costi
        If Not IsPostBack Then
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim utenteAbilitatoCdG As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                                enum_Security_Attivita.Inserimento_CostiRicavi_Da_QdC_CdG,
                                                enum_Security_Operazione.Modifica,
                                                Date.Now, "", objParametri_Utenti)

            Dim leggi_CDG_R As New CDG_BIZ_R
            Dim tipoCdG = leggi_CDG_R.GetTipoCdG(objParametriAgenda.Piva, objParametri_Server, objParametri_Utenti)

            If tipoCdG = enum_TipoCdG.NuovoTipo AndAlso utenteAbilitatoCdG = True AndAlso flag_MostraBtnSalvaCDG = True Then
                RBL_Salva.Items.Add(New ListItem("Salva e Vai ai Costi", 3))
                If (hf_esistonoCostiCollegatiCDG.Value.ToLower = "true") Then
                    RBL_Salva.Items(RBL_Salva.Items.Count - 1).Attributes.Add("style", "background-color: orange;")
                    'CType(RBL_Salva.Items(RBL_Salva.Items.Count - 1), System.Web.UI.WebControls.WebControl).BackColor = Drawing.Color.Orange
                End If
            End If
        End If



        'per fare in modo che si veda sempre correttamente la tabella,
        'dato che aggiorno i valore via js e può succedere che il dato rimanga nel viewstate e presenti il dato sbagliato
        'cosa che succede quando aggiorno i valori via js, il dato visualizzato non è giusto ma in realtà il dato giusto
        'rimane nella sessione e viene usato per il salvataggio.
        AggiornaGridViewCostiAccessoriVisibili()

        impostaPluginCombo()

    End Sub


#Region "Gestione Colonne Impianti"
    Private Sub CreaImpostazioniColonne()
        'creo
        ListaColonneVisibili.Items.Clear()
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.RagioneSociale))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.CentroAziendale))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Campo))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.App))
        ListaColonneVisibili.Items.Add(New ListItem("Cod. Biologico App."))
        ListaColonneVisibili.Items.Add(New ListItem("Rif. Numerico App."))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Catasto))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DestinazioneDUso))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Varietà))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Disciplinare))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Regolamento))
        ListaColonneVisibili.Items.Add(New ListItem("Capitolato"))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Finalità))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DataInizioImpianto))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DataSemina))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DataFioritura))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DataFiorituraPrevista))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DataRaccoltaPrevista))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.LottoImpianto))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.Copertura))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DateRaccoltePrecedenti))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DataRaccolta))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DataSeminaPrevista))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.NMassimoKgHa))
        'ListaColonneVisibili.Items.Add(New ListItem("N Distribuito [Kg/Ha]"))
        'ListaColonneVisibili.Items.Add(New ListItem("N Residuo [Kg/Ha]"))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.PMassimoKgHa))
        'ListaColonneVisibili.Items.Add(New ListItem("P Distribuito [Kg/Ha]"))
        'ListaColonneVisibili.Items.Add(New ListItem("P Residuo [Kg/Ha]"))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.KMassimoKgHa))
        'ListaColonneVisibili.Items.Add(New ListItem("K Distribuito [Kg/Ha]"))
        'ListaColonneVisibili.Items.Add(New ListItem("K Residuo [Kg/Ha]"))
        ListaColonneVisibili.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.MgMassimoKgHa))
        'ListaColonneVisibili.Items.Add(New ListItem("Mg Distribuito [Kg/Ha]"))
        'ListaColonneVisibili.Items.Add(New ListItem("Mg Residuo [Kg/Ha]"))

        '  Vanni, 20/09/2013 11:51:06: colonne AGEA
        ListaColonneVisibili.Items.Add(New ListItem("Specie Agea"))
        ListaColonneVisibili.Items.Add(New ListItem("Cultivar Agea"))

        ' Nicoletta 15/07/2014 per smart rilevamento Pozzi
        ListaColonneVisibili.Items.Add(New ListItem("Tra Fila"))
        ListaColonneVisibili.Items.Add(New ListItem("Su Fila"))
        ListaColonneVisibili.Items.Add(New ListItem("n Piante/Impianto"))
        ListaColonneVisibili.Items.Add(New ListItem("Forma Allevamento"))
        ListaColonneVisibili.Items.Add(New ListItem("Portinnesto"))

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
                str = str + ListaColonneVisibili.Items(i).Text & "|"
            End If
        Next


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

        If Not IsNothing(ViewState("DT_Impianti")) Then
            GridView_Impianti.DataSource = ViewState("DT_Impianti")
            GridView_Impianti.DataBind()
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
        For i = -1 To -6 Step -1
            'filtro solo i movimenti di tipo ...
            Dim DR() As DataRow
            If i = -6 Then
                DR = DT.Select("Centro_cod not in (-1,-2,-3,-4,-5)")
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
                    Case -5
                        Cau_Mov = CAU_SCARICO
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

                    If Cau_Mov = CAU_SCARICO AndAlso Not New ArrayList({-5, -6, -7, -8, -9}).Contains(i) Then
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
    Private Sub DefaultAziendali()
        If objParametriAgenda.Tipo_Operazione <> TipiEnumerativi.enum_TipoOperazioneDB.Scrittura Then
            Exit Sub
        End If


        ''''''''''''''''''''''''''''''''''''''''
        ''''''''''' CARICO LE NOTE '''''''''''''
        ''''''''''''''''''''''''''''''''''''''''
        Dim objProfilazioneR As New AgronicaCoreProfilazioneBIZ.Profilazione_R
        Dim DT_note As DataTable

        DT_note = objProfilazioneR.LeggiProfilazioneNote_In_Cascata(objParametriAgenda.Piva,
                                         "-2",
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

        Dim i As Integer = 0
        Dim j As Integer = 0
        For i = 0 To DT_note.Rows.Count - 1
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



        ''''''''''''''''''''''''''''''''''''''''
        ''''' CARICO MACCHINE E CONTATTI '''''''
        ''''''''''''''''''''''''''''''''''''''''

        'elimino le macchine salvate
        objParametriAgenda.Movimenti = New List(Of Movimento)
        Costruisci_DT_Scarico()

        Dim DS_Macchine As DataSet
        DS_Macchine = objProfilazioneR.LeggiProfilazioneMacchine_In_Cascata(objParametriAgenda.Piva,
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




        Dim dtScarico As DataTable
        dtScarico = Session("dtScarico")

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
                'Grilli: c'era scritto "indefinito, ma valerio ha voluto che mettessi "ora" per SBTF per coerenza col LAN
                Udm_Des = "ora"
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
                        'Grilli: c'era scritto "indefinito, ma valerio ha voluto che mettessi "ora" per SBTF per coerenza col LAN
                        Udm_Des = "ora"
                        Udm_Cod = "-1"
                End Select
            End If


            Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
            'Dim DT = objContab.MacchinaDes(objParametriAgenda.Piva, _
            '                           objParametriAgenda.Data, _
            '                           listaMacCod(i), _
            '                            "", "", _
            '                           objParametri_Server)

            Dim DT = objContab.Leggi_daMacCod("",
                                       listaMacCod(i),
                                        "", "",
                                       objParametri_Server)

            objContab = Nothing
            Dim Categoria_Des As String = ""
            Dim Risorsa_Des As String = ""
            Dim Costo_Unitario As String = ""
            Dim Costo As String = ""
            Dim Codice As String = ""
            If DT.Rows.Count > 0 Then

                Categoria_Des = DT.Rows(0).Item("Class_Desc")
                If DT.Rows(0).Item("Mac_Des") <> "" Then
                    Risorsa_Des = DT.Rows(0).Item("Mac_Des")
                Else
                    Risorsa_Des = DT.Rows(0).Item("Ditta_Des") & " " & DT.Rows(0).Item("Modello")
                End If

                Costo_Unitario = Format(Prezzo * 1, "0.00")
                Costo = Format(Prezzo * 0, "0.00")
                Codice = DT.Rows(0).Item("Codice")


                'verifico se ho l'acqua impostata 
                If QtaAcqua.Value = "" Or QtaAcqua.Value = "0" Then
                    If Not IsDBNull(DT.Rows(0).Item("Taratura_Ugello")) Then
                        If IsNumeric(DT.Rows(0).Item("Taratura_Ugello")) Then
                            Dim Acqua As Decimal
                            Acqua = DT.Rows(0).Item("Taratura_Ugello")
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
                                        0, 0,
                                        listaOreMac(i),
                                        listaMinMac(i),
                                        0, 0,
                                        0,
                                        dtScarico,
                                        Codice)
            End If
        Next









        'RAPPORTI CONTABILI
        For i = 0 To listaCodContatto.Count - 1

            Dim Dt_Manodopera As DataTable
            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R


            Dim Str As String
            Str = " ( (Rapporti_Contabili.Cod_Rapporto in (-1,-4,-5,-6,-12))  or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Terzista=1 )  "
            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessoribyCod_RisUm(objParametriAgenda.Piva,
                                                                                     listaCodContatto(i),
                                                                                     False,
                                                                                     False,
                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                      Str, " Contatti.Sa_Cod desc, Contatti.Rag_Soc, cognome, nome ASC  ", objParametri_Server)




            If Dt_Manodopera.Rows.Count = 0 Then
                Exit For
            End If

            Dim Cod_Rapporto As String = ""
            Dim Centro_Cod As Integer = 0
            Dim Riga As Integer = 0
            Dim Elem_Cod As Integer = 0
            Dim Tipo_Centro As String = ""
            Dim Centro As String = ""
            Dim Qualifica_Cod As Integer = 0
            Cod_Rapporto = Dt_Manodopera.Rows(0).Item("Cod_Rapporto")

            Select Case Cod_Rapporto
                Case -1, -4, -6
                    Centro = "Manodopera"
                    Tipo_Centro = "MD"
                    Centro_Cod = -2
                Case -5
                    Centro_Cod = -3
                    Centro = "C/Terzisti"
                    Tipo_Centro = "CT"
                Case -12
                    Centro_Cod = -4
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

            'Se è un dipendente allora imposto anche la qualifica in automatico
            If Cod_Rapporto = -4 Then
                Qualifica_Cod = Dt_Manodopera.Rows(0).Item("Qualifica_Cod")
            End If

            Dim Categoria_Des, Risorsa_Des As String

            Categoria_Des = Dt_Manodopera.Rows(0).Item("Rapporto_Des")
            Risorsa_Des = Dt_Manodopera.Rows(0).Item("Rag_Soc")
            If Not IsDBNull(Dt_Manodopera.Rows(0).Item("Qualifica_Des")) AndAlso CStr(Dt_Manodopera.Rows(0).Item("Qualifica_Des")).Trim() <> "" Then
                Risorsa_Des &= " (Qualifica: " + CStr(Dt_Manodopera.Rows(0).Item("Qualifica_Des")).Trim() + ")"
            End If
            Elem_Cod = 0
            Dim Udm_Des As String = "ora"
            Dim Udm_Cod As Integer = 2
            Dim Mat_Cod As Integer = 0
            Dim Pro_Cod As Integer = 0
            Pro_Cod = 0
            Mat_Cod = Dt_Manodopera.Rows(0).Item("Cod_Risum")

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
            Select Case ALGORITMO_COSTI_ACCESSORI
                Case enum_AlgoritmoCostiAccessori.CAB
                    If IsDBNull(Dt_Manodopera.Rows(0).Item("Prezzo_Unitario")) Then
                        Costo_Unitario = "0.00"
                        Costo = "0.00"
                    Else
                        Costo_Unitario = Format(Dt_Manodopera.Rows(0).Item("Prezzo_Unitario") * 1, "0.00")
                        Costo = Format(Dt_Manodopera.Rows(0).Item("Prezzo_Unitario") * 0, "0.00")
                    End If

                Case enum_AlgoritmoCostiAccessori.SBTF
                    Costo_Unitario = "0.00"
                    Costo = "0.00"
            End Select



            Dim Ditta_Cod As String
            Ditta_Cod = 0


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
                                    0, 0,
                                    listaOreContatto(i),
                                    listaMinContatto(i),
                                    Qualifica_Cod, 0,
                                    Cod_Rapporto,
                                    dtScarico)
        Next


        'aggiorno i costi
        Session("dtScarico") = dtScarico
        AggiornaGridViewCostiAccessoriVisibili()

        SalvaCostiAccessori_Default()

        AggiornaMagazzino()

    End Sub

    Private Sub defaultSuperuser()

        Session("PERMETTI_OPERAZIONI_SU_IMPIANTI_BLOCCATI") = False
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Dt_Impostazioni_Super As New DataTable
        Dt_Impostazioni_Super = ObjUtenti.Leggi(0,
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
        If objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Scrittura Or
           objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Modifica Then
            Dt_Impostazioni = ObjUtenti.Leggi(0,
                                              1,
                                              AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "",
                                              "",
                                              objParametri_Utenti)

        End If

        If Not Dt_Impostazioni Is Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then

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
                        If objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Scrittura Then

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
                                                If objParametriAgenda.OperazioneMulticentro Then
                                                    Me.ComboCentroAziendale.ddl_CentroAziendale.SelectedValue = 0
                                                    objParametriAgenda.Sa_Cod = 0
                                                    AggiornaSpecie()

                                                    If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
                                                        CaricaGriglia_Impianti()
                                                    Else
                                                        CaricaGriglia_Planning()
                                                    End If

                                                    GridView_Impianti.Columns(2).Visible = True
                                                Else
                                                    'se non è permesso il multicentro non faccio nulla, altrimenti mi precarica gli impianti di tutti i centri
                                                    'E' una toppa, funziona con menu vecchio ma con menu nuovo parzialmente
                                                End If

                                        End Select
                                    End If

                            End Select

                            'Impostazioni solo per mopdifica
                        ElseIf objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Modifica Then




                        End If

                End Select



            Next
        End If

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

        UpdatePanelSpecie.Update()


    End Sub

#End Region

#Region "Bottoni Nascosti"
    Protected Sub BTN_ChangeData_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ChangeData.Click
        If IsDate(txt_DataOperazione.Text) Then


            Dim dataMin As Date = AGRODATAINIZIO
            Dim dataMax As Date = AGRODATAFINE
            Dim SportelloAperto As Boolean = True
            If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
                Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R
                objPratiche.Data_Sportello_Da_Servizio(objParametriAgenda.Piva, enum_Servizi.Quaderno_Campagna_Caa, DateTime.Now, SportelloAperto, dataMin, dataMax, objParametri_Server, objParametri_Utenti)

                objPratiche.Sportello_ChiamataSecondaria_SeNessunCambiamento(objParametriAgenda.Piva,
                                                                                        enum_Servizi.QuadernoCampagnaBio,
                                                                                        DateAndTime.Now,
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
                End If

                If objParametriAgenda.Data > dataMax Then
                    objParametriAgenda.Data = dataMax
                    txt_DataOperazione.Text = dataMax.ToShortDateString
                End If

                Dim modificateDate As Boolean = False

                If CDate(txt_DataOperazione.Text) < dataMin Then
                    txt_DataOperazione.Text = dataMin.ToShortDateString
                    Messaggi.AgroMsgBox("Non è possibile inserire un'operazione prima del " & dataMin.ToShortDateString, Page, , UpdatePanelData)
                    modificateDate = True
                End If

                If CDate(txt_DataOperazione.Text) > dataMax Then
                    txt_DataOperazione.Text = dataMax.ToShortDateString
                    Messaggi.AgroMsgBox("Non è possibile inserire un'operazione successiva a " & dataMax.ToShortDateString, Page, , UpdatePanelData)
                    modificateDate = True
                End If
            End If

            objParametriAgenda.Data = txt_DataOperazione.Text

            'If modificateDate = True Then
            '    Exit Sub
            'End If

        End If
            AggiornaCampo()
        'aggiorno specie 
        AggiornaSpecie()
        'aggiorno disciplinare
        'AggiornaDisciplinari()
        'aggiorno Impianti

        If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
            CaricaGriglia_Impianti()
        Else
            CaricaGriglia_Planning()
        End If


        'ricarico le ricette
        CaricaElencoRicette()

    End Sub

    Protected Sub BTN_ComboOperazione_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboOperazione.Click
        'Cambio la slave

        Dim from_Angular = False
        Dim fromBootstrapToBootstrap As Boolean = False

        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.GiasNG Then
            from_Angular = True
        Else
            fromBootstrapToBootstrap = True
        End If

        If Not from_Angular Then
            If ComboOperazione.Valore_Combo = LAVCOD_RACCOLTA Then
                If objParametriAgenda.Sa_Cod = "0" Or objParametriAgenda.Sa_Cod = "" Then
                    ComboOperazione.ddl_Operazioni.SelectedValue = objParametriAgenda.Lav_Cod
                    Messaggi.AgroMsgBox("Per la raccolta occorre selezionare prima un centro aziendale", Page, , UpdatePanelOperazione)
                    Exit Sub
                End If
            End If
        End If

        objParametriAgenda.Lav_Cod = ComboOperazione.Valore_Combo
        objParametriAgenda.Lav_Des = ComboOperazione.Testo_Combo
        Dim PaginaLink As String = ""
        Dim OpUtil As New AgronicaCoreModello.Utility_Operazioni
        PaginaLink = OpUtil.Gestisci_Redirect_Cambio_Operazione(objParametriAgenda, objParametri_Server,
                                                                fromBootstrapToBootstrap:=fromBootstrapToBootstrap)

        If PaginaLink = "" Then
            Messaggi.AgroMsgBox("Operazione in manutenzione", Page, , UpdatePanelOperazione)
            Exit Sub
        End If

        If Not from_Angular Then
            objParametriAgenda.Disciplinare = "0"
            CaricaElencoRicette()
        End If

        Response.Redirect(PaginaLink)

    End Sub

    Protected Sub BTN_ComboCentroAziendale_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboCentroAziendale.Click
        objParametriAgenda.Sa_Cod = ComboCentroAziendale.Valore_Combo
        AggiornaCampo()
        AggiornaSpecie()
        If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
            CaricaGriglia_Impianti()
        Else
            CaricaGriglia_Planning()
        End If

    End Sub

    Protected Sub BTN_ComboCampo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboCampo.Click

        AggiornaSpecie()
        If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
            CaricaGriglia_Impianti()
        Else
            CaricaGriglia_Planning()
        End If


        'ricarico la griglia delle ricette
        'CaricaElencoRicette()

    End Sub

    Protected Sub BTN_ComboSpecie_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboSpecie.Click

        BTN_ComboSpecie_ClickExtracted()

    End Sub

    Private Sub BTN_ComboSpecie_ClickExtracted()
        objParametriAgenda.Veg_Cod = ComboSpecie.Valore_Combo '.Split("/")(0)
        objParametriAgenda.Cul_Cod = "0"

        ''Lettura dei Default
        DefaultUtente()

        DefaultAziendali()

        ComboVarieta.ddl_Varieta.Items.Clear()
        objParametriAgenda.Cul_Cod = "0"

        If IsNumeric(objParametriAgenda.Veg_Cod) AndAlso objParametriAgenda.Veg_Cod <> "0" AndAlso objParametriAgenda.Veg_Cod <> "-1" Then

            Dim CampoCod As Integer = 0

            'If ComboCampo.Testo_Combo <> "" Then
            '    If Not IsNothing(Split(ComboCampo.Valore_Combo, "/")(2)) Then
            '        CampoCod = CInt(Split(ComboCampo.Valore_Combo, "/")(2))
            '    End If
            'End If
            Dim Valori_Combo As List(Of String) = ComboCampo.Valori_Combo

            ComboVarieta.Campi = Valori_Combo

            ComboVarieta.Piva = objParametriAgenda.Piva
            ComboVarieta.Sa_Cod = objParametriAgenda.Sa_Cod
            'ComboVarieta.Campo_Cod = CampoCod
            ComboVarieta.Veg_Cod = objParametriAgenda.Veg_Cod
            ComboVarieta.SoloAziendali = True
            ComboVarieta.PrimaRiga_Flag = False
            ComboVarieta.PrimaRiga_Text = "Tutte le varietà"
            ComboVarieta.PrimaRiga_Value = "0"

            ComboVarieta.CaricaComboVarieta()

        End If

        'AggiornaDisciplinari()
        If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
            CaricaGriglia_Impianti()
        Else
            CaricaGriglia_Planning()
        End If


        'ricarico la griglia delle ricette
        CaricaElencoRicette()
    End Sub

    Protected Sub BTN_ComboVarieta_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_ComboVarieta.Click

        objParametriAgenda.Cul_Cod = "0"
        Dim strCulCod As String = ""
        Dim Valori_Combo As List(Of String) = ComboVarieta.Valori_Combo
        If Not Valori_Combo Is Nothing Then
            For i = 0 To Valori_Combo.Count - 1
                strCulCod &= Valori_Combo.Item(i) & ","
            Next
        End If
        If strCulCod <> "" Then
            strCulCod = Left(strCulCod, strCulCod.Length - 1)
            objParametriAgenda.Cul_Cod = strCulCod
        End If

        If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
            CaricaGriglia_Impianti()
        Else
            CaricaGriglia_Planning()
        End If


        'ricarico la griglia delle ricette
        'CaricaElencoRicette()

    End Sub

    Protected Sub BTN_Magazzini_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BTN_Magazzini.Click
        objParametriAgenda.Fabbricato = ComboMagazzini.Valore_Combo

        If ComboMagazzini.Valore_Combo <> "0" Then
            ImgBtn_Carico.Visible = True
        Else
            ImgBtn_Carico.Visible = False
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
            Case LAVCOD_TRAPIANTO, LAVCOD_SEMINA, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING
                'Grilli: 29/06/2018 Fabrizio ha detto di togliere la limitazione in seguito a segnalazione di Selvi che non può trapiantare
                ''se sono in semina o trapianto carico tutte le specie )filtrando comunque tra quelle presenti nelle impostazioni utenti
                ''LAVCOD_SOVESCIO:  per il sovescio non carico tutte le specie, potrei al massimo caricare solo
                ''gli impianti di arboree, ma lascio come le altre operazioni per ora
                '    Select Case objParametriAgenda.Lav_Cod
                '        Case LAVCOD_SEMINA
                '            FiltroAggiuntivo = " ( (SpecieVegetali.Gru_Cod = '2') OR (SpecieVegetali.Gru_Cod = '3') )"
                '        Case LAVCOD_TRAPIANTO
                '            FiltroAggiuntivo = " ( (SpecieVegetali.Gru_Cod = '1') OR (SpecieVegetali.Gru_Cod = '3') OR (SpecieVegetali.Veg_Cod = '6') OR (SpecieVegetali.Veg_Cod = '335')) "
                '    End Select
                ComboSpecie.CaricaxPDC = False
                ComboSpecie.Carica_Tutte_Specie_Esistenti = True
                'se sono in semina e trapianto e ho un impianto nella lista impianti allora
                'arrivo dall'albero anagrafe e quindi 
                'anche se sono in scrittura 
        End Select

        'ComboSpecie.FiltroAggiuntivo = FiltroAggiuntivo

        'Dim CampoCod As Integer = 0
        'If ComboCampo.Testo_Combo <> "" Then
        '    If Not IsNothing(Split(ComboCampo.Valore_Combo, "/")(2)) Then
        '        CampoCod = CInt(Split(ComboCampo.Valore_Combo, "/")(2))
        '    End If
        'End If
        'ComboSpecie.Campo_Cod = CampoCod

        Dim Valori_Combo As List(Of String) = ComboCampo.Valori_Combo
        ComboSpecie.Campi = Valori_Combo




        ComboSpecie.CaricaComboSpecie()

        If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Planning Then
            Dim xLeggiSpPlan As New AgronicaCoreAnagrafeDAL.Programmazione_Entita_R
            Dim dtSp As DataTable = xLeggiSpPlan.SpecieVegetali(objParametriAgenda.Programmazione_Cod, "", "", objParametri_Server)
            Dim listaSpecie As List(Of String) = (From ssPlan In dtSp.AsEnumerable
                                                  Select CStr(ssPlan("Veg_cod"))).ToList

            Dim newItems As New List(Of ListItem)
            For Each lItem As ListItem In ComboSpecie.ddl_Specie.Items
                If listaSpecie.Contains(lItem.Value) Then
                    newItems.Add(lItem)
                End If
            Next

            ComboSpecie.ddl_Specie.Items.Clear()

            For Each newItem As ListItem In newItems
                ComboSpecie.ddl_Specie.Items.Add(newItem)
            Next

        End If

        'imposto il valore
        If objParametriAgenda.Veg_Cod <> "-1" Then
            If objParametriAgenda.Veg_Cod = "0/0" Then
                objParametriAgenda.Veg_Cod = "0"
            End If
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
                    Str.AppendLine("});")
                    ScriptManager.RegisterClientScriptBlock(UpdatePanelCentroAziendale, UpdatePanelCentroAziendale.GetType(),
                                                     String.Format("jQuery_{0}", txt_DataOperazione.ClientID), Str.ToString, True)
                End If
            Catch ex As Exception

            End Try

            objParametriAgenda.Veg_Cod = ComboSpecie.Valore_Combo '.Split("/")(0)

        End If

        ComboVarieta.ddl_Varieta.Items.Clear()

        If IsNumeric(objParametriAgenda.Veg_Cod) AndAlso objParametriAgenda.Veg_Cod <> "0" AndAlso objParametriAgenda.Veg_Cod <> "-1" Then
            ComboVarieta.Piva = objParametriAgenda.Piva
            ComboVarieta.Sa_Cod = objParametriAgenda.Sa_Cod
            ' ComboVarieta.Campo_Cod = CampoCod
            ComboVarieta.Campi = Valori_Combo
            ComboVarieta.Veg_Cod = objParametriAgenda.Veg_Cod
            ComboVarieta.SoloAziendali = True
            ComboVarieta.PrimaRiga_Flag = False
            ComboVarieta.PrimaRiga_Text = "Tutte le varietà"
            ComboVarieta.PrimaRiga_Value = "0"
            ComboVarieta.CaricaComboVarieta()
        End If


    End Sub

    Private Sub AggiornaMagazzino()

        ComboMagazzini.Piva = objParametriAgenda.Piva
        ComboMagazzini.Flag_CodCentroFabbricato = True
        ComboMagazzini.TipoMagazzino = MAGAZZINO
        Select Case objParametriAgenda.Lav_Cod
            Case LAVCOD_TRAPIANTO, LAVCOD_SEMINA, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING
                ComboMagazzini.Flag_GestioneMagazziniImpresaPadre = True
        End Select

        If Not IsNothing(Session("UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO")) AndAlso
            Session("UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO") = True AndAlso
            objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Scrittura Then
            ComboMagazzini.PrimaRiga_Flag = False
        End If

        ComboMagazzini.Pive_Terzisti = ""
        ComboMagazzini.Flag_GestioneMagazziniTerzisti = False

        Select Case objParametriAgenda.Lav_Cod
            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                    LAVCOD_CONCIA_SEME,
                    LAVCOD_DISSECCAMENTO,
                    LAVCOD_GEODISINFESTAZIONE,
                    LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                    LAVCOD_DISERBO
                Dim Pive_Terzisti As String = ""
                If Not IsNothing(Session("dtScarico")) AndAlso CType(Session("dtScarico"), DataTable).Rows.Count > 0 Then
                    Dim dtScarico As DataTable = Session("dtScarico")
                    Dim DrTerzisti() As DataRow = dtScarico.Select("Centro_Cod=-3")
                    If Not DrTerzisti Is Nothing AndAlso DrTerzisti.Length > 0 Then
                        For t = 0 To DrTerzisti.Length - 1
                            Dim objC As New AgronicaCoreAnagrafeDAL.Contatti_R
                            Dim PivaTerzista As String = ""
                            If objC.VerificaEsistenza_CodContatto_as_PivaGIAS(DrTerzisti(t).Item("Mat_Cod"), PivaTerzista, objParametri_Server) = True Then
                                Pive_Terzisti &= "'" & PivaTerzista & "',"
                            End If
                        Next
                    End If
                End If
                If Pive_Terzisti <> "" Then
                    ComboMagazzini.Pive_Terzisti = Left(Pive_Terzisti, Pive_Terzisti.Length - 1)
                    ComboMagazzini.Flag_GestioneMagazziniTerzisti = True
                End If
        End Select

        ComboMagazzini.CaricaComboMagazzini()

        'default (spostato qui dalla funzione DefaultUtente)
        If objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Scrittura And Session("PrimaVolta") = True Then
            Session("PrimaVolta") = False
            If Not IsNothing(Session("UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO")) AndAlso
                Session("UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO") = True Then
                If ComboMagazzini.PrimaRiga_Flag = True Then
                    If ComboMagazzini.N_Magazzini > 1 Then
                        ComboMagazzini.Indice_Combo = 1
                        For i = 0 To ComboMagazzini.ddl_Magazzini.Items.Count - 1
                            Dim valorecombo As String = ComboMagazzini.ddl_Magazzini.Items(i).Value
                            If Split(valorecombo, "|").Count = 3 Then
                                If Split(valorecombo, "|")(2) = objParametri_Server.PivaSuperUser Then
                                    ComboMagazzini.Indice_Combo = i
                                    Exit For
                                End If
                                If Split(valorecombo, "|")(2) <> objParametriAgenda.Piva Then
                                    ComboMagazzini.Indice_Combo = i
                                End If
                            End If
                        Next
                    End If
                Else
                    ComboMagazzini.Indice_Combo = 0
                    If ComboMagazzini.N_Magazzini > 0 Then
                        For i = 0 To ComboMagazzini.ddl_Magazzini.Items.Count - 1
                            Dim valorecombo As String = ComboMagazzini.ddl_Magazzini.Items(i).Value
                            If Split(valorecombo, "|").Count = 3 Then
                                If Split(valorecombo, "|")(2) = objParametri_Server.PivaSuperUser Then
                                    ComboMagazzini.Indice_Combo = i
                                    Exit For
                                End If
                                If Split(valorecombo, "|")(2) <> objParametriAgenda.Piva Then
                                    ComboMagazzini.Indice_Combo = i
                                End If
                            End If
                        Next
                    End If
                End If
            Else
                ComboMagazzini.Indice_Combo = 0
            End If
            objParametriAgenda.Fabbricato = ComboMagazzini.Valore_Combo
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

        AggiornaCampo()


    End Sub

    Private Sub AggiornaCampo()

        ComboCampo.Piva = objParametriAgenda.Piva
        ComboCampo.Sa_Cod = objParametriAgenda.Sa_Cod
        ComboCampo.CaricaComboCampo()

    End Sub

    Private Sub AggiornaOperazioni()

        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
            PannelloOperazioni.Visible = False
        End If

        'Escludo le Operazioni non gestite sulle pagine aspx
        Dim Filtro As String = "AND" + CostantiPersonalizzate.STR_OP_NON_GESTITE_BS

        ComboOperazione.CaricaComboLavorazioni(True, Filtro)
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

    Private Sub InizializzaData()
        Dim Str As New StringBuilder
        Str.AppendLine("$(document).ready(function () {")


        Str.AppendLine("    $('#" & txt_DataOperazione.ClientID & "').datepicker({ ")
        Str.AppendLine("    dateFormat:  'dd/mm/yy',")
        Str.AppendLine("    disabled: false,")
        Str.AppendLine("    changeMonth: true,")
        Str.AppendLine("    changeYear: true")

        Str.AppendLine("});")

        Str.AppendLine("$.datepicker.regional['it'];")

        Str.AppendLine("$('#" & txt_DataOperazione.ClientID & "').change(function () {")
        Str.AppendLine("    $('#" & BTN_ChangeData.ClientID & "').click();")
        Str.AppendLine("    });")

        Str.AppendLine("});")
        ScriptManager.RegisterStartupScript(UpdatePanelData, UpdatePanelData.GetType(),
                                         String.Format("jQuery_{0}", txt_DataOperazione.ClientID), Str.ToString, True)


        Str = New StringBuilder
        Str.AppendLine("$(document).ready(function () {")

        Dim contenitore As String = "window"
        Dim scarto As String = "-30"
        Dim scartoW As String = "-60"

        Str.AppendLine("$('#dialog').dialog({")
        Str.AppendLine("    autoOpen: false,")
        Str.AppendLine("    height: 600,")
        Str.AppendLine("    width: 850,")
        'Str.AppendLine("    maxHeight: $(" & contenitore & ").height()" & scarto & ",")
        'Str.AppendLine("    maxWidth: $(" & contenitore & ").width()" & scartoW & ",")
        Str.AppendLine("    modal: true")
        Str.AppendLine("});")

        Str.AppendLine("});")
        ScriptManager.RegisterStartupScript(UpdateCostiAccessoriAvanzati, UpdateCostiAccessoriAvanzati.GetType(),
                                         String.Format("jQuery_{0}", UpdateCostiAccessoriAvanzati.ClientID), Str.ToString, True)

    End Sub





    Private Sub InizializzaVarie()

        'script Specie
        ScriptManager.RegisterStartupScript(UpdatePanelSpecie, UpdatePanelSpecie.GetType(),
                                         String.Format("jQuery_{0}", ComboSpecie.ClientID), ComboSpecie.GetJS(), True)

        'script varieta
        ScriptManager.RegisterStartupScript(UpdatePanelVarieta, UpdatePanelVarieta.GetType(),
                                         String.Format("jQuery_{0}", ComboVarieta.ClientID), ComboVarieta.GetJS2(), True)

        'script Centri
        ScriptManager.RegisterStartupScript(UpdatePanelCentroAziendale, UpdatePanelCentroAziendale.GetType(),
                                         String.Format("jQuery_{0}", ComboCentroAziendale.ClientID), ComboCentroAziendale.GetJS(), True)

        'script Campi
        ScriptManager.RegisterStartupScript(UpdatePanelCampo, UpdatePanelCampo.GetType(),
                                         String.Format("jQuery_{0}", ComboCampo.ClientID), ComboCampo.GetJS2(), True)

        'script Magazzini
        ScriptManager.RegisterStartupScript(UpdatePanelMagazzino, UpdatePanelMagazzino.GetType(),
                                         String.Format("jQuery_{0}", ComboMagazzini.ClientID), ComboMagazzini.GetJS(), True)

        'script operazione
        ScriptManager.RegisterStartupScript(UpdatePanelOperazione, UpdatePanelOperazione.GetType(),
                                         String.Format("jQuery_{0}", ComboOperazione.ClientID), ComboOperazione.GetJS(), True)

        'Dim i As Integer
        'Dim objCmbAttivita As DropDownList
        'Dim objCmbTurni As DropDownList

        'Dim dt As DataTable = Session("dtScarico")

        'For i = 0 To dgrScaricoAvanzati.Rows.Count - 1

        '    objCmbAttivita = CType(dgrScaricoAvanzati.Rows(i).FindControl("Cmb_Attivita"), DropDownList)
        '    objCmbTurni = CType(dgrScaricoAvanzati.Rows(i).FindControl("Cmb_Turni"), DropDownList)

        '    If Not IsNothing(objCmbAttivita) And Not IsNothing(objCmbTurni) Then

        '        AgronicaCoreUtility.CaricaListControl.Attivita(objCmbAttivita, True, "", "0", 0, "", "", objParametri_Server)
        '        AgronicaCoreUtility.CaricaListControl.Turni(objCmbTurni, True, "", "0", 0, "", "", objParametri_Server)

        '        Dim StrSelect As New StringBuilder
        '        'classe per autocomplete della combo
        '        StrSelect.AppendLine("$(document).ready(function () { ")
        '        StrSelect.AppendLine("   $('#" & objCmbAttivita.ClientID & "').combobox();")
        '        'StrSelect.AppendLine("   $('#" & objCmbAttivita.ClientID & "').combobox().parent().find('input.ui-autocomplete-input').css('width', '150px');")
        '        StrSelect.AppendLine("   $('#" & objCmbTurni.ClientID & "').combobox();")
        '        ' StrSelect.AppendLine("   $('#" & objCmbTurni.ClientID & "').combobox().parent().find('input.ui-autocomplete-input').css('width', '150px');")
        '        StrSelect.AppendLine("});")

        '        ScriptManager.RegisterStartupScript(UpdateCostiAccessoriAvanzati, UpdateCostiAccessoriAvanzati.GetType(),
        '                                         String.Format("jQuery_{0}", objCmbAttivita.ClientID), StrSelect.ToString, True)


        '        If Not IsDBNull(dt.Rows(i).Item("Id_Attivita")) Then
        '            objCmbAttivita.SelectedIndex = objCmbAttivita.Items.IndexOf(objCmbAttivita.Items.FindByValue(dt.Rows(i).Item("Id_Attivita")))
        '        End If

        '        If Not IsDBNull(dt.Rows(i).Item("Turno_Cod")) Then
        '            objCmbTurni.SelectedIndex = objCmbTurni.Items.IndexOf(objCmbAttivita.Items.FindByValue(dt.Rows(i).Item("Turno_Cod")))
        '        End If

        '    End If

        'Next


        ''script Disciplinari
        'ScriptManager.RegisterStartupScript(UpdatePanelDisciplinare, UpdatePanelDisciplinare.GetType(),
        '                                 String.Format("jQuery_{0}", ComboDisciplinari.ClientID), ComboDisciplinari.GetJS(), True)

        ''script Filtri Ricerca
        'ScriptManager.RegisterStartupScript(UpdatePanelFiltriAggiuntivi, UpdatePanelFiltriAggiuntivi.GetType(),
        '                                 String.Format("jQuery_{0}", ComboFiltriAggiuntiviAgenda.ClientID), ComboFiltriAggiuntiviAgenda.GetJS(), True)

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


#Region "Planning"


    Private TipoOperazioneAgenda As enum_Tipo_Operazione_Agenda

    Dim Qs_Unid_Ricetta As String
    Dim Qs_Unid_Ricetta_Operazione As String

    Private Sub ScriptGridPlanning()

        Dim script As New StringBuilder


        script.AppendLine("$(document).ready(function () { ")



        script.AppendLine(" setTimeout(function () { BloccaSbloccaTotale(); }, 100);  ")


        script.AppendLine("     AbilitaDisabilita_SupTrattataPlanning();")

        script.AppendLine("     $('#chkSelezionaTuttiPlanning').click(function (){ ")
        script.AppendLine("         SelezionaDeselezionaTuttiPlanning();")
        script.AppendLine("     });")

        script.AppendLine("     $('.ChkSelezionaPlanning').click(function (){ ")
        script.AppendLine("         ChkSelezionaPlanning_Click($(this).find('input'),false);")
        script.AppendLine("     });")


        Select Case objParametriAgenda.Lav_Cod

            Case Is <> LAVCOD_TRATTAMENTO_POST_RACCOLTA

                If SupTrattata = True Then
                    script.AppendLine("     RicalcolaSuperficieCoinvoltaPlanning(); ")

                    script.AppendLine("     $('.Sup_Coinvolta').keyup(function (){")
                    script.AppendLine("         Sup_Coinvolta_Keyup($(this));")
                    script.AppendLine("     });")

                    script.AppendLine("     $('.SommaSuperficieTrattata').keyup(function () {")
                    script.AppendLine("         SommaSuperficieTrattata_Keyup(); ")
                    script.AppendLine("     });")

                    script.AppendLine("     $('.AcquaHa').keyup(function () {")
                    script.AppendLine("         AcquaHA_Keyup();")
                    script.AppendLine("     });")

                    script.AppendLine("     $('.AcquaTot').keyup( function () {")
                    script.AppendLine("         AcquaTot_Keyup();")
                    script.AppendLine("     });")
                End If

        End Select

        'TODO: Vanni, 19/09/2016 18:30:28: Verificare ...
        script.AppendLine("     $('#dialogImpostazioniColonne').dialog({ ")
        script.AppendLine("             autoOpen: false,")
        script.AppendLine("             modal: true,")
        script.AppendLine("             buttons: {")
        script.AppendLine("                 'Aggiungi': function () {")
        script.AppendLine("                 $(this).dialog('close');")
        script.AppendLine("                 SalvaImpostazioniColonne();")
        script.AppendLine("             },")
        script.AppendLine("             'Annulla': function () {")
        script.AppendLine("                 $(this).dialog('close');")
        script.AppendLine("                 return false;")
        script.AppendLine("             }")
        script.AppendLine("         }")
        script.AppendLine("     });")

        script.AppendLine("     $('#dialogImpostazioniColonnePlanning').dialog({ ")
        script.AppendLine("             autoOpen: false,")
        script.AppendLine("             modal: true,")
        script.AppendLine("             buttons: {")
        script.AppendLine("                 'Aggiungi': function () {")
        script.AppendLine("                 $(this).dialog('close');")
        script.AppendLine("                 SalvaImpostazioniColonnePlanning();")
        script.AppendLine("             },")
        script.AppendLine("             'Annulla': function () {")
        script.AppendLine("                 $(this).dialog('close');")
        script.AppendLine("                 return false;")
        script.AppendLine("             }")
        script.AppendLine("         }")
        script.AppendLine("     });")


        script.AppendLine("     $('#btn_Impostazioni_Colonne_Planning').click(function () {")
        'script.AppendLine("         alert('');")
        'script.AppendLine("         if ($('.rigaImpianti').length == 0) {")
        script.AppendLine("             $('#dialogImpostazioniColonnePlanning').dialog('open');")
        script.AppendLine("             $('#dialogImpostazioniColonnePlanning').parent().appendTo($('form:first')); ")
        'script.AppendLine("         }")
        script.AppendLine("     });")



        script.AppendLine("}); ")


        ScriptManager.RegisterStartupScript(UpdatePanelImpianti, UpdatePanelImpianti.GetType(),
                                         String.Format("jQuery_{0}", UpdatePanelImpianti.ClientID), script.ToString, True)

    End Sub

    Public Sub CaricaGriglia_Planning()

        'se ho selezionato un'operazione che gestisce un gridview di impianti colturali in maniera specializzata e nella pagina specifica allora nascondo il gridview degli impianti.
        If Not Dato_Lav_cod_è_VisibileGrigliaImpiantiPlanning() Then
            GridView_Impianti.Visible = False
            GridView_Planning.Visible = False
            Exit Sub
        End If

        If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
            Exit Sub
        End If

        If ComboSpecie.Valore_Combo = "" Then
            Exit Sub
        End If

        GridView_Impianti.Visible = False
        GridView_Planning.Visible = True

        'ScriptGridPlanning()

        'TipoOperazioneAgenda = objParametriAgenda.TipoOperazioneAgenda

        'If Not IsNothing(Request.QueryString("unid_ricetta_operazione")) Then
        '    Qs_Unid_Ricetta_Operazione = Stringa_Decodifica(Request.QueryString("unid_ricetta_operazione").ToString, _
        '                                        AgroKey_EncoderDecoder, _
        '                                        Server)
        'Else
        '    Qs_Unid_Ricetta_Operazione = ""
        'End If



        'If TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.Ricetta Then

        '    Dim StringaXmlOperazione As String
        '    Dim objWebW As New AgronicaCoreVarieDAL.Web_ComunicazionePagine_W
        '    Dim objWebC As New AgronicaCoreVarieDAL.Web_ComunicazionePagine_R
        '    Dim DtWebC As DataTable
        '    DtWebC = objWebC.Leggi(Qs_Unid_Ricetta_Operazione, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        '    objWebW.Cancella(Qs_Unid_Ricetta_Operazione, 0, "", objParametri_Server)
        '    If DtWebC.Rows.Count > 0 Then
        '        StringaXmlOperazione = DtWebC.Rows(0).Item("Stringa_Parametri_Base")
        '        If StringaXmlOperazione <> "" Then
        '            'Ripristina_Dati_nei_Controlli_xRicetta(StringaXmlOperazione)
        '        End If
        '    End If

        'End If


        Dim DT_Campi As New DataTable
        Dim DT_Appezzamenti As New DataTable
        Dim DT_Particelle As New DataTable
        Dim DT_Intersezioni As New DataTable
        Dim DT_Appezzamenti_Eliminati As New DataTable
        Dim ElencoAppezzamenti As String

        Dim intDummy As Integer

        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date
        Dim ErrMSG As String = ""
        Dim Sa_Cod As Integer = 0
        Dim Tipo_Pianificazione As Integer = 0
        Dim TuttiCentri As Boolean = True

        Dim Qs_ProgrammazioneCod As Integer
        Qs_ProgrammazioneCod = objParametriAgenda.Programmazione_Cod


        Dim objP As New AgronicaCoreAnagrafeBIZ.Programmazione_R
        objP.DT_Appezzamenti_Crea(DT_Appezzamenti)
        objP.DT_Intersezioni_Crea(DT_Intersezioni)
        objP.DT_Appezzamenti_Eliminati_Crea(DT_Appezzamenti_Eliminati)

        Dim xFiltroAggiuntivo_Programmazione_entita As String =
            "  Programmazione_entita.veg_cod = " & ComboSpecie.Valore_Combo &
            "  AND " & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveDate(txt_DataOperazione.Text) & " BETWEEN Programmazione_entita.Validita_inizio and Programmazione_Entita.Validita_fine "

        If ComboVarieta.Valori_Combo.Count > 0 Then
            xFiltroAggiuntivo_Programmazione_entita &= " AND Programmazione_Entita.Cul_cod in ( " &
                String.Join(",", ComboVarieta.Valori_Combo.ToArray) &
            ")"
        End If

        If ComboCampo.Valori_Combo.Count > 0 Then
            xFiltroAggiuntivo_Programmazione_entita &=
                " AND   cast(Programmazione_Entita.Piva as varchar(100)) + '/' +  " &
                "       cast(Programmazione_Entita.Sa_cod as varchar(100)) + '/' + " &
                "       cast(Programmazione_Entita.Campo_Cod as varchar(100)) " &
                "in (" &
                "'" & String.Join(",", ComboCampo.Valori_Combo.ToArray).Replace(",", "','") & "'" &
            ")"
        End If

        intDummy = objP.Pianificazione_Leggi(Qs_ProgrammazioneCod,
                                      "",
                                      "",
                                      objParametriAgenda.Piva,
                                      Sa_Cod,
                                      "",
                                      Validita_Inizio,
                                      Validita_Fine,
                                      Tipo_Pianificazione,
                                      DT_Appezzamenti,
                                      DT_Intersezioni,
                                      DT_Appezzamenti_Eliminati,
                                      TuttiCentri,
                                      "",
                                      AGRODATAINIZIO,
                                      "",
                                      objParametri_Server,
                                      xFiltroAggiuntivo_Programmazione_entita)
        Griglia_Planning_Aggiorna(DT_Appezzamenti, False)


        '-----------------------------------------------------
        'se ho Planning selezionati li chekko!!!
        '-----------------------------------------------------
        For i = 0 To objParametriAgenda.Impianti.Count - 1

            For j = 0 To GridView_Planning.Rows.Count - 1
                If objParametriAgenda.Impianti(i).Programmazione_Entita_cod = DT_Appezzamenti.Rows(j).Item("Programmazione_Entita_Cod") Then

                    CType(GridView_Planning.Rows(j).FindControl("ChkSelezionaPlanning"), CheckBox).Checked = True
                    CType(GridView_Planning.Rows(j).FindControl("Txt_Trattata_Planning"), TextBox).Text = objParametriAgenda.Impianti(i).Qta2
                End If
            Next

        Next

    End Sub



    Private Sub Griglia_Planning_Aggiorna(ByRef DT_Appezzamenti As DataTable,
                                              Optional ByVal bMantieniSelezione As Boolean = False)

        Dim NomiChiavi(5) As String

        'Chiave della griglia
        NomiChiavi(0) = "Piva"
        NomiChiavi(1) = "Sa_Cod"
        NomiChiavi(2) = "Appezza"
        NomiChiavi(3) = "Id_Reg"
        NomiChiavi(4) = "Programmazione_Entita_Cod"
        NomiChiavi(5) = "Progetto_Cod"

        Dim i As Integer
        Dim vetSel(2, 0) As Integer
        Dim nSel As Integer = 0

        If bMantieniSelezione Then
            ' nel vettore vetSel salvo gli indici della griglia che sono selezionati
            For i = 0 To GridView_Planning.Rows.Count - 1
                If CType(GridView_Planning.Rows(i).FindControl("ChkSeleziona"), HtmlInputCheckBox).Checked = True Or
                   GridView_Planning.Rows(i).BackColor = Drawing.Color.Gold Then

                    ReDim Preserve vetSel(2, nSel)
                    vetSel(0, nSel) = i     ' indice della griglia

                    vetSel(1, nSel) = IIf(CType(GridView_Planning.Rows(i).FindControl("ChkSeleziona"), HtmlInputCheckBox).Checked = True, 1, 0)  ' selezionato con check
                    ' a volte la prima colonna non è colorata (a seconda del tipo di operazione), quindi controllo sempre la seconda
                    vetSel(2, nSel) = IIf(GridView_Planning.Rows(i).BackColor = Drawing.Color.Gold, 1, 0)   ' selezionato con colore
                    nSel += 1
                End If
            Next

        End If

        'Griglia_Appezzamenti_Catasto(DT_Appezzamenti)

        GridView_Planning.DataSource = DT_Appezzamenti
        GridView_Planning.DataKeyNames = NomiChiavi
        GridView_Planning.DataBind()

        If bMantieniSelezione Then
            ' durante il databinding perdo la selezione delle righe.
            ' in vetSel ci sono gli indici delle righe selezionate. Quindi ora le reimposto
            If Not IsNothing(vetSel) Then
                For i = 0 To vetSel.GetLength(1) - 1

                    Dim indice As Integer = vetSel(0, i)
                    If vetSel(1, i) = 1 Then
                        CType(GridView_Planning.Rows(indice).FindControl("ChkSeleziona"), HtmlInputCheckBox).Checked = True
                    End If

                    If vetSel(2, i) = 1 Then
                        Griglia_Appezzamenti_Seleziona(indice, enum_TipoPianificazione.Pianificazione_Annuale, False)
                    End If

                Next
            End If
        End If

        vetSel = Nothing

        For i = 0 To GridView_Planning.Rows.Count - 1
            Select Case CInt(GridView_Planning.Rows(i).Cells(5).Text)
                Case enum_TipoOperazioneProgrammazioneEntita.Confermato,
                     enum_TipoOperazioneProgrammazioneEntita.Nuovo_Appezzamento,
                      enum_TipoOperazioneProgrammazioneEntita.Modifica_Semplice
                Case enum_TipoOperazioneProgrammazioneEntita.Nuova_Distinta
                    GridView_Planning.Rows(i).BackColor = Drawing.Color.LightYellow
                Case enum_TipoOperazioneProgrammazioneEntita.Nuovo_Impianto
                    GridView_Planning.Rows(i).BackColor = Drawing.Color.Yellow
                Case enum_TipoOperazioneProgrammazioneEntita.Chiudi_Appezzamento
                    GridView_Planning.Rows(i).BackColor = Drawing.Color.Red
                Case enum_TipoOperazioneProgrammazioneEntita.Modifica_Superficie
                    GridView_Planning.Rows(i).BackColor = Drawing.Color.PaleGreen
                Case enum_TipoOperazioneProgrammazioneEntita.Unione
                    GridView_Planning.Rows(i).BackColor = Drawing.Color.LimeGreen
                Case enum_TipoOperazioneProgrammazioneEntita.Frazionamento
                    GridView_Planning.Rows(i).BackColor = Drawing.Color.LightSeaGreen
            End Select
        Next

    End Sub

    '##########################################################################################################################################
    Private Sub Griglia_Appezzamenti_Seleziona(
                                ByVal IndiceRiga As Integer,
                                ByVal TipoPianificazione As Integer,
                                Optional ByVal ImpostaCheck As Boolean = False)


        Dim Inizio As Integer

        'If Operazione = enum_TipoOperazioneDB.Lettura Then
        '    Inizio = 0
        'Else
        '    Inizio = 1
        'End If

        Select Case TipoPianificazione

            Case enum_TipoPianificazione.Pianificazione_Annuale,
                 enum_TipoPianificazione.Pianificazione_DaNotificaBio
                GridView_Planning.Rows(IndiceRiga).BackColor = Drawing.Color.Gold

            Case enum_TipoPianificazione.Pianificazione_Quindicinale

        End Select

        If ImpostaCheck Then
            CType(GridView_Planning.Rows(IndiceRiga).FindControl("ChkSeleziona"), HtmlInputCheckBox).Checked = True
        End If


    End Sub



#End Region

#Region "Impianti"

    Private Function Dato_Lav_cod_è_VisibileGrigliaImpiantiPlanning() As Boolean
        If {
                LAVCOD_RILIEVO_PIOGGE,
                 LAVCOD_TRATTAMENTO_POST_RACCOLTA,
                 LAVCOD_REINNESCO_TRAPPOLE,
                 LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE,
                 LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING,
                 LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO,
                 LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO
            }.Contains(objParametriAgenda.Lav_Cod) Then


            Return False

        Else
            Return True

        End If
    End Function

    Public Sub CaricaGriglia_Impianti()


        'se ho selezionato un'operazione che gestisce un gridview di impianti colturali in maniera specializzata e nella pagina specifica allora nascondo il gridview degli impianti.
        If Not Dato_Lav_cod_è_VisibileGrigliaImpiantiPlanning() Then
            GridView_Impianti.Visible = False
            GridView_Planning.Visible = False
            Exit Sub
        End If


        If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Planning Then
            Exit Sub
        End If


        GridView_Impianti.Visible = True
        GridView_Planning.Visible = False


        Select Case objParametriAgenda.Lav_Cod

            Case LAVCOD_TRATTAMENTO_POST_RACCOLTA

            Case Else

                'objParametriAgenda.Leggi()
                Dim Dt As New DataTable

                'icona per le info aggiuntive
                Dim Icona_INFO As String = "<img src='../AB_Immagini/Icone16/cI.ico' border='0'>"
                Dim Testo As String = ""
                Dim i As Integer = 0
                Dim j As Integer = 0

                Dim Dati As String
                Dim strErr As String = ""
                Dim ColturaProtetta As String = ""

                Dim SaCod As Integer = CInt(objParametriAgenda.Sa_Cod)

                '----- Recupero l'elenco degli impianti

                ''------------------------------------------------------------------------------
                Dim Array() As String
                Array = Split(objParametriAgenda.Disciplinare, "/")
                'Dim Dpi_Cod As Integer = Array(0)
                'Dim IdRcdpi As Integer = Array(1)
                Dim Grfi_Cod As Integer
                Dim Flag_Protetto As Integer
                Dim Flag_Disciplinare As Boolean
                Dim Flag_PubblicoPrivato As Integer = 0

                If Not IsNothing(Array) And Array.Length > 1 Then
                    Grfi_Cod = Array(2)
                    Flag_Protetto = Array(3)
                    Flag_PubblicoPrivato = Array(4)
                    Flag_Disciplinare = True
                Else
                    Grfi_Cod = 0
                    Flag_Protetto = 0
                    Flag_PubblicoPrivato = 0
                    Flag_Disciplinare = False
                End If


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

                Dim CampoCod As Integer = 0

                'If ComboCampo.Valore_Combo <> "xxxxxxxxxxx/0/0" Then
                '    If Not IsNothing(Split(ComboCampo.Valore_Combo, "/")(2)) Then
                '        CampoCod = CInt(Split(ComboCampo.Valore_Combo, "/")(2))
                '        SaCod = CInt(Split(ComboCampo.Valore_Combo, "/")(1))
                '    End If
                'End If
                Dim _campi As List(Of String) = ComboCampo.Valori_Combo
                Dim FiltroAggiuntivo As String = ""
                If Not IsNothing(_campi) Then
                    For i = 0 To _campi.Count - 1
                        Dim sa As Integer = _campi(i).Split("/")(1)
                        Dim ca As Integer = _campi(i).Split("/")(2)

                        Dim FiltroTemp As String = " ( Appezzamento.Sa_Cod = " & sa & " AND Appezzamento.Campo_Cod = " & ca & " ) "
                        If FiltroAggiuntivo = "" Then
                            FiltroAggiuntivo = FiltroTemp
                        Else
                            FiltroAggiuntivo = FiltroAggiuntivo & " OR " & FiltroTemp
                        End If

                    Next
                End If

                Dim filtroFinale As String = filtro.Split("|")(1)
                If FiltroAggiuntivo <> "" Then
                    FiltroAggiuntivo = "(" & FiltroAggiuntivo & ")"
                    If filtroFinale <> "" Then
                        filtroFinale = filtroFinale & " AND " & FiltroAggiuntivo
                    Else
                        filtroFinale = FiltroAggiuntivo
                    End If
                End If

                'lettura impianti
                'Dt = objImpianti.Leggi_Impianti_xAgenda(Flag_Disciplinare, _
                '                                objParametriAgenda.Piva, _
                '                                SaCod, _
                '                                CampoCod, _
                '                                specie, _
                '                                objParametriAgenda.Cul_Cod, _
                '                                objParametriAgenda.Data, _
                '                                Grfi_Cod, _
                '                                Flag_Protetto, _
                '                                id_cod, _
                '                                filtroFinale, " Cul_Des, App_Nome, Progetto ", _
                '                                objParametri_Server, leggiAncheBloccati)
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
                                                filtroFinale, " Cul_Des, App_Nome, Progetto ",
                                                objParametri_Server, leggiAncheBloccati)
                '----------------------------------
                'lettura particelle impianti
                Dim DtParticelle As DataTable
                DtParticelle = objImpianti.Leggi_ParticelleImpianti_xAgenda2(Flag_Disciplinare,
                                             objParametriAgenda.Piva,
                                             SaCod,
                                             specie,
                                             objParametriAgenda.Cul_Cod,
                                             objParametriAgenda.Data,
                                             Grfi_Cod,
                                             Flag_Protetto,
                                             id_cod,
                                             filtro.Split("|")(1), " Cul_Des, App_Nome, Progetto ",
                                             objParametri_Server, leggiAncheBloccati)

                '----------------------------------
                'lettura zone vulnerabili
                Dim DtPV As DataTable
                Dim objPV As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
                DtPV = objPV.Leggi(-17,
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

                '---------------------------------------
                'Vettore di DataColumn
                Dim DtKeys(16) As String

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
                DtKeys(15) = "Campo_Des"
                DtKeys(16) = "Sa_Nome"


                'aggiungo la colonna catasto
                Dt.Columns.Add(New DataColumn("catasto", GetType(String)))
                Dt.Columns.Add(New DataColumn("Raccolte_Precedenti", GetType(String)))

                Dim objRaccolte As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                Dim DTRAccolte As DataTable


                '(25/07/2016 fede) aggiunta verifica impostazione utente se utilizzare sup_app
                Dim ObjUtentiI As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
                Dim Dt_Impostazioni As New DataTable
                Dim Utilizza_SupApp As Boolean = False
                If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Or
                   objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
                    Dt_Impostazioni = ObjUtentiI.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_UTILIZZA_SUP_APP_AGENDA,
                                                      objParametriAgenda.Lav_Cod,
                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                      "",
                                                      "",
                                                      objParametri_Utenti)
                    If Not Dt_Impostazioni Is Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then
                        Utilizza_SupApp = True
                    End If

                End If


                If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

                    For j = 0 To Dt.Rows.Count - 1

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

                        If Utilizza_SupApp = True Then
                            Dt.Rows(j).Item("sup_imp") = Dt.Rows(j).Item("sup_app")
                        End If

                        '--------------------------
                        'Formatto le date
                        If IsDate(Dt.Rows(j).Item("Validita_Inizio")) Then
                            If CDate(Dt.Rows(j).Item("Validita_Inizio")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Validita_Inizio")) = AGRODATAFINE Then
                                Dt.Rows(j).Item("Validita_Inizio") = ""
                            End If
                        End If
                        If IsDate(Dt.Rows(j).Item("Validita_Fine")) Then
                            If CDate(Dt.Rows(j).Item("Validita_Fine")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Validita_Fine")) = AGRODATAFINE Then
                                Dt.Rows(j).Item("Validita_Fine") = ""
                            End If
                        End If
                        If IsDate(Dt.Rows(j).Item("Data_Semina")) Then
                            If CDate(Dt.Rows(j).Item("Data_Semina")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Data_Semina")) = AGRODATAFINE Then
                                Dt.Rows(j).Item("Data_Semina") = ""
                            End If
                        End If
                        If IsDate(Dt.Rows(j).Item("Data_Semina_Prevista")) Then
                            If CDate(Dt.Rows(j).Item("Data_Semina_Prevista")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Data_Semina_Prevista")) = AGRODATAFINE Then
                                Dt.Rows(j).Item("Data_Semina_Prevista") = ""
                            End If
                        End If
                        If IsDate(Dt.Rows(j).Item("Data_Raccolta")) Then
                            If CDate(Dt.Rows(j).Item("Data_Raccolta")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Data_Raccolta")) = AGRODATAFINE Then
                                Dt.Rows(j).Item("Data_Raccolta") = ""
                            End If
                        End If

                        DTRAccolte = objRaccolte.Leggi_Raccolte(objParametriAgenda.Piva,
                                          SaCod,
                                          0, 0, 0,
                                          Dt.Rows(j).Item("appezza"),
                                          Dt.Rows(j).Item("id_reg"),
                                          0,
                                          CDate(Dt.Rows(j).Item("Validita_Inizio_Distinta")),
                                          objParametriAgenda.Data,
                                          AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                          "", "", objParametri_Server)
                        Dim strRaccolte As String = ""
                        Dim r As Integer
                        If Not DTRAccolte Is Nothing AndAlso DTRAccolte.Rows.Count > 0 Then
                            For r = 0 To DTRAccolte.Rows.Count - 1
                                strRaccolte &= DTRAccolte.Rows(r).Item("Data_Movimento").ToShortDateString & "<br>"
                            Next
                            If strRaccolte <> "" Then
                                strRaccolte = Left(strRaccolte, strRaccolte.Length - 4)
                            End If
                        End If

                        Dt.Rows(j).Item("Raccolte_Precedenti") = strRaccolte

                        If IsDate(Dt.Rows(j).Item("Data_Raccolta_Prevista")) Then
                            If CDate(Dt.Rows(j).Item("Data_Raccolta_Prevista")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Data_Raccolta_Prevista")) = AGRODATAFINE Then
                                Dt.Rows(j).Item("Data_Raccolta_Prevista") = ""
                            End If
                        End If
                        If IsDate(Dt.Rows(j).Item("Data_Fioritura")) Then
                            If CDate(Dt.Rows(j).Item("Data_Fioritura")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Data_Fioritura")) = AGRODATAFINE Then
                                Dt.Rows(j).Item("Data_Fioritura") = ""
                            End If
                        End If
                        If IsDate(Dt.Rows(j).Item("Data_Fioritura_Prevista")) Then
                            If CDate(Dt.Rows(j).Item("Data_Fioritura_Prevista")) = AGRODATAINIZIO Or CDate(Dt.Rows(j).Item("Data_Fioritura_Prevista")) = AGRODATAFINE Then
                                Dt.Rows(j).Item("Data_Fioritura_Prevista") = ""
                            End If
                        End If
                        '========= PermessoDPI fine

                        '-----------------------------------------
                        'aggiunta indicazione catasto (19/09/2012)
                        Dim strCatasto As String = ""
                        Dim DrParticelle() As DataRow
                        Dim DrPV() As DataRow
                        Dim DrPVFA() As DataRow
                        Dim DrPVFB() As DataRow
                        Dim p As Integer

                        Dim strParticella As String
                        Dim strVulnerabile As String

                        If Not DtParticelle Is Nothing AndAlso DtParticelle.Rows.Count > 0 Then
                            DrParticelle = DtParticelle.Select("piva='" & Dt.Rows(j).Item("piva").ToString & "' and sa_cod=" & Dt.Rows(j).Item("sa_cod").ToString & " and appezza=" & Dt.Rows(j).Item("appezza").ToString & " and id_reg=" & Dt.Rows(j).Item("id_reg").ToString)
                            If Not DrParticelle Is Nothing AndAlso DrParticelle.Length > 0 Then
                                For p = 0 To DrParticelle.Length - 1
                                    strParticella = DrParticelle(p).Item("prov") & "_" & DrParticelle(p).Item("com") & "_" & DrParticelle(p).Item("sezione") & "_" & DrParticelle(p).Item("foglio") & "_" & DrParticelle(p).Item("numero") & "_" & DrParticelle(p).Item("subalterno")
                                    strVulnerabile = ""
                                    If Not DtPV Is Nothing AndAlso DtPV.Rows.Count > 0 Then
                                        DrPV = DtPV.Select("prov='" & DrParticelle(p).Item("prov").ToString & "' AND Com='" & DrParticelle(p).Item("com").ToString & "' AND Sezione='" & DrParticelle(p).Item("sezione").ToString & "' AND Foglio=" & DrParticelle(p).Item("foglio").ToString & " AND Numero=" & DrParticelle(p).Item("numero").ToString & " AND subalterno='" & DrParticelle(p).Item("subalterno").ToString & "'")
                                        If Not DrPV Is Nothing AndAlso DrPV.Length > 0 Then
                                            strVulnerabile = " <b>(V)</b>"
                                        End If
                                    End If
                                    If Not DtPVF Is Nothing AndAlso DtPVF.Rows.Count > 0 Then
                                        DrPVFA = DtPVF.Select("prov='" & DrParticelle(p).Item("prov").ToString & "' AND Com='" & DrParticelle(p).Item("com").ToString & "' AND Foglio=" & DrParticelle(p).Item("foglio").ToString & " AND Fascia_Cod=1")
                                        If Not DrPVFA Is Nothing AndAlso DrPVFA.Length > 0 Then
                                            strVulnerabile = " <b>(V - Fascia A)</b>"
                                        End If
                                        DrPVFB = DtPVF.Select("prov='" & DrParticelle(p).Item("prov").ToString & "' AND Com='" & DrParticelle(p).Item("com").ToString & "' AND Foglio=" & DrParticelle(p).Item("foglio").ToString & " AND Fascia_Cod=2")
                                        If Not DrPVFB Is Nothing AndAlso DrPVFB.Length > 0 Then
                                            strVulnerabile = " <b>(V - Fascia B)</b>"
                                        End If
                                    End If
                                    strCatasto &= strParticella & strVulnerabile & "<br>"
                                Next
                            End If
                        End If
                        If strCatasto <> "" Then
                            strCatasto = Left(strCatasto, strCatasto.Length - 4)
                        End If

                        Dt.Rows(j).Item("catasto") = strCatasto




                        '----------------------------------------------------------------
                        'DA OTTIMIZZARE se la si vuole ripristinare
                        '----------------------------------------------------------------

                        'Dim objFert As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                        'Dim DtFert As DataTable
                        'Dim N_Distribuito As Decimal = 0
                        'Dim P_Distribuito As Decimal = 0
                        'Dim K_Distribuito As Decimal = 0
                        'Dim Mg_Distribuito As Decimal = 0
                        'Dim N_Residuo As Decimal = 0
                        'Dim P_Residuo As Decimal = 0
                        'Dim K_Residuo As Decimal = 0
                        'Dim Mg_Residuo As Decimal = 0

                        'Select Case objParametriAgenda.Lav_Cod

                        '    Case LAVCOD_FERTIRRIGAZIONE,
                        '          LAVCOD_CONCIMAZIONE_FOGLIARE, _
                        '          LAVCOD_DISTRIBUZIONE_CONCIME, _
                        '          LAVCOD_DISTRIBUZIONE_AMMENDANTI, _
                        '          LAVCOD_SARCHIATURA_CONCIMAZIONE, _
                        '          LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

                        '        'se almeno un valore massimo è stato impostato sull'impianto
                        '        If Dt.Rows(j).Item("N_Massimo") <> "" Or _
                        '            Dt.Rows(j).Item("P_Massimo") <> "" Or _
                        '            Dt.Rows(j).Item("K_Massimo") <> "" Or _
                        '            Dt.Rows(j).Item("Mg_Massimo") <> "" Then

                        '            DtFert = objFert.Leggi_Macroelementi_Distribuiti(N_Distribuito, _
                        '                                    P_Distribuito, _
                        '                                    K_Distribuito, _
                        '                                    Mg_Distribuito, _
                        '                                    CStr(Dt.Rows(j).Item("Piva")), _
                        '                                    CInt(Dt.Rows(j).Item("Sa_Cod")), _
                        '                                    CInt(Dt.Rows(j).Item("Appezza")), _
                        '                                    CInt(Dt.Rows(j).Item("Id_Reg")), _
                        '                                    CInt(Dt.Rows(j).Item("Progetto_Cod")), _
                        '                                    CDbl(Dt.Rows(j).Item("Sup_Imp")), _
                        '                                    CDate(Dt.Rows(j).Item("Validita_Inizio_Distinta")), _
                        '                                    CDate(Dt.Rows(j).Item("Validita_Fine_Distinta")), _
                        '                                    CInt(objParametriAgenda.Id_Agenda), _
                        '                                    objParametri_Server)

                        '            If IsNumeric(Dt.Rows(j).Item("N_Massimo")) Then
                        '                Dt.Rows(j).Item("N_Distribuito") = Format(N_Distribuito, "0.###")
                        '                Dt.Rows(j).Item("N_Residuo") = Format(CDbl(Dt.Rows(j).Item("N_Massimo")) - N_Distribuito, "0.###")
                        '            End If
                        '            If IsNumeric(Dt.Rows(j).Item("P_Massimo")) Then
                        '                Dt.Rows(j).Item("P_Distribuito") = Format(P_Distribuito, "0.###")
                        '                Dt.Rows(j).Item("P_Residuo") = Format(CDbl(Dt.Rows(j).Item("P_Massimo")) - P_Distribuito, "0.###")
                        '            End If
                        '            If IsNumeric(Dt.Rows(j).Item("K_Massimo")) Then
                        '                Dt.Rows(j).Item("K_Distribuito") = Format(K_Distribuito, "0.###")
                        '                Dt.Rows(j).Item("K_Residuo") = Format(CDbl(Dt.Rows(j).Item("K_Massimo")) - K_Distribuito, "0.###")
                        '            End If
                        '            If IsNumeric(Dt.Rows(j).Item("Mg_Massimo")) Then
                        '                Dt.Rows(j).Item("Mg_Distribuito") = Format(Mg_Distribuito, "0.###")
                        '                Dt.Rows(j).Item("Mg_Residuo") = Format(CDbl(Dt.Rows(j).Item("Mg_Massimo")) - Mg_Distribuito, "0.###")
                        '            End If

                        '        End If

                        'End Select



                    Next

                End If


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
                'If sostituisci = True Then
                GridView_Impianti.DataSource = Dt
                GridView_Impianti.DataKeyNames = DtKeys
                GridView_Impianti.DataBind()
                ViewState("DT_Impianti") = Dt
                'End If

                SettaImpostazioneUtente_UDM()

                '-----------------------------------------------------
                'se ho impianti selezionati li chekko!!!
                '-----------------------------------------------------
                For i = 0 To objParametriAgenda.Impianti.Count - 1

                    For j = 0 To Dt.Rows.Count - 1
                        If objParametriAgenda.Impianti(i).Piva = Dt.Rows(j).Item("piva") And
                           objParametriAgenda.Impianti(i).Sa_Cod = Dt.Rows(j).Item("sa_cod") And
                           objParametriAgenda.Impianti(i).Appezza = Dt.Rows(j).Item("appezza") And
                           objParametriAgenda.Impianti(i).ID_Reg = Dt.Rows(j).Item("id_reg") Then

                            CType(GridView_Impianti.Rows(j).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True
                            CType(GridView_Impianti.Rows(j).FindControl("Txt_Trattata"), TextBox).Text = objParametriAgenda.Impianti(i).Qta2
                        End If
                    Next

                Next

        End Select

    End Sub

    Public Function GetImpianti() As List(Of Impianto)

        Dim ListaImpianti As New List(Of Impianto)
        Dim ListaImpiantiObjparametriagenda As New List(Of Impianto)
        Dim i As Integer
        Dim inizializza As Boolean = False
        If objParametriAgenda.Impianti.Count = 0 Then
            inizializza = True
            ListaImpiantiObjparametriagenda = New List(Of Impianto)
        End If

        Dim Imp As Impianto

        If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then

            For i = 0 To GridView_Impianti.Rows.Count - 1

                If (CType(GridView_Impianti.Rows(i).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True) Then

                    Imp = New Impianto
                    Imp.Reale_Or_Planning = enum_Tipo_Operazione_Agenda_Target.Reale

                    Imp.Piva = GridView_Impianti.DataKeys(i).Item(0).ToString
                    Imp.Sa_Cod = CInt(GridView_Impianti.DataKeys(i).Item(1))
                    Imp.Appezza = CInt(GridView_Impianti.DataKeys(i).Item(2))
                    Imp.ID_Reg = CInt(GridView_Impianti.DataKeys(i).Item(3))
                    Imp.Progetto_Cod = CDbl(GridView_Impianti.DataKeys(i).Item(4))
                    Imp.Sup_Imp = CDbl(GridView_Impianti.DataKeys(i).Item(5))
                    Imp.Validita_Inizio_Distinta = CDate(GridView_Impianti.DataKeys(i).Item(6))
                    Imp.Validita_Fine_Distinta = CDate(GridView_Impianti.DataKeys(i).Item(7))
                    Imp.App_Nome = GridView_Impianti.DataKeys(i).Item("App_Nome")
                    Imp.Qta2 = CType(GridView_Impianti.Rows(i).FindControl("Txt_Trattata"), TextBox).Text
                    Imp.Cul_Des = GridView_Impianti.DataKeys(i).Item("Cul_Des")
                    If GridView_Impianti.DataKeys(i).Item("Data_Raccolta") <> "" Then
                        Imp.Data_Raccolta = GridView_Impianti.DataKeys(i).Item("Data_Raccolta")
                    End If
                    If GridView_Impianti.DataKeys(i).Item("Data_Raccolta_Prevista") <> "" Then
                        Imp.Data_Raccolta_Prevista = GridView_Impianti.DataKeys(i).Item("Data_Raccolta_Prevista")
                    End If

                    If GridView_Impianti.DataKeys(i).Item("Data_Fioritura") <> "" Then
                        Imp.Data_Fioritura = GridView_Impianti.DataKeys(i).Item("Data_Fioritura")
                    End If
                    If GridView_Impianti.DataKeys(i).Item("Data_Fioritura_Prevista") <> "" Then
                        Imp.Data_Fioritura_Prevista = GridView_Impianti.DataKeys(i).Item("Data_Fioritura_Prevista")
                    End If

                    Imp.Codici_Anagrafe_Des = GridView_Impianti.DataKeys(i).Item("Codici_Anagrafe_Des")

                    Imp.Sa_Nome = GridView_Impianti.DataKeys(i).Item("Sa_Nome")
                    Imp.Campo_Des = GridView_Impianti.DataKeys(i).Item("Campo_Des")

                    ListaImpianti.Add(Imp)
                    If inizializza = True Then
                        ListaImpiantiObjparametriagenda.Add(Imp)
                    End If

                End If

            Next
        Else


            For i = 0 To GridView_Planning.Rows.Count - 1

                If (CType(GridView_Planning.Rows(i).FindControl("ChkSelezionaPlanning"), CheckBox).Checked = True) Then

                    Imp = New Impianto
                    Imp.Reale_Or_Planning = enum_Tipo_Operazione_Agenda_Target.Planning


                    Imp.Programmazione_Entita_cod = GridView_Planning.DataKeys(i).Item("Programmazione_Entita_Cod").ToString

                    Imp.Piva = GridView_Planning.DataKeys(i).Item("Piva").ToString
                    Imp.Sa_Cod = CInt(GridView_Planning.DataKeys(i).Item("Sa_Cod"))
                    Imp.Campo_Cod = CInt(GridView_Planning.DataKeys(i).Item("Campo_Co"))
                    Imp.Appezza = CInt(GridView_Planning.DataKeys(i).Item("Appezza"))
                    Imp.ID_Reg = CInt(GridView_Planning.DataKeys(i).Item("ID_Reg"))
                    Imp.Progetto_Cod = CDbl(GridView_Planning.DataKeys(i).Item("Progetto_Cod"))
                    Imp.Sup_Imp = CDbl(GridView_Planning.DataKeys(i).Item("Sup_App"))

                    Imp.Validita_Inizio_Distinta = CDate(GridView_Planning.DataKeys(i).Item("Validita_Inizio"))
                    Imp.Validita_Fine_Distinta = CDate(GridView_Planning.DataKeys(i).Item("Validita_Fine"))


                    Imp.App_Nome = GridView_Planning.DataKeys(i).Item("App_Nome")
                    Imp.Qta2 = CType(GridView_Planning.Rows(i).FindControl("Txt_Trattata_Planning"), TextBox).Text
                    Imp.Cul_Des = GridView_Planning.DataKeys(i).Item("Cul_Des")
                    If GridView_Planning.DataKeys(i).Item("Data_Raccolta") <> "" Then
                        Imp.Data_Raccolta = GridView_Planning.DataKeys(i).Item("Data_Raccolta")
                    End If

                    'TODO: Capire...
                    'If GridView_Planning.DataKeys(i).Item("Data_Raccolta_Prevista") <> "" Then
                    '    Imp.Data_Raccolta_Prevista = GridView_Planning.DataKeys(i).Item("Data_Raccolta_Prevista")
                    'End If

                    'If GridView_Planning.DataKeys(i).Item("Data_Fioritura") <> "" Then
                    '    Imp.Data_Fioritura = GridView_Planning.DataKeys(i).Item("Data_Fioritura")
                    'End If
                    'If GridView_Planning.DataKeys(i).Item("Data_Fioritura_Prevista") <> "" Then
                    '    Imp.Data_Fioritura_Prevista = GridView_Planning.DataKeys(i).Item("Data_Fioritura_Prevista")
                    'End If

                    'Imp.Codici_Anagrafe_Des = GridView_Impianti.DataKeys(i).Item("Codici_Anagrafe_Des")

                    Imp.Sa_Nome = GridView_Planning.DataKeys(i).Item("Sa_Nome")
                    Imp.Campo_Des = GridView_Planning.DataKeys(i).Item("Campo_Des")

                    ListaImpianti.Add(Imp)
                    If inizializza = True Then
                        ListaImpiantiObjparametriagenda.Add(Imp)
                    End If

                End If
            Next
        End If



        If inizializza = True Then
            objParametriAgenda.Impianti = ListaImpiantiObjparametriagenda
        End If

        Return ListaImpianti

    End Function

#End Region

#Region "Note"

    Private Sub Carica_Note()

        'modifico la finestra temporale in modo da caricare solamente quelle che sono attive a oggi
        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(objParametriAgenda.Data,
                                                                      objParametriAgenda.Data)

        Dim strFiltro As String = " Note_Intervento.Nota_Cod > 0 "
        Dim GruppoDes As String = ""

        AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Consigli,
                                                        False,
                                                         "", "",
                                                         0,
                                                         -2,
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
                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                    " NotaGruppo_Cod < 0",
                                    "",
                                     objParametri_Server)

        If Not DT_NoteGruppi Is Nothing Then
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
                                                 -2,
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
                                                 -2,
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
                                                 -2,
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
                                                 -2,
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
                                                 -2,
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
                                                 -2,
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

                    If CBL_Meteo.Visible = True And TrovataNota = False Then
                        For j = 0 To CBL_Meteo.Items.Count - 1
                            If Nota_Cod = CBL_Meteo.Items(j).Value Then
                                CBL_Meteo.Items(j).Selected = True
                                TrovataNota = True
                                Continue For
                            End If
                        Next
                    End If
                    If CBL_VentoIntensita.Visible = True And TrovataNota = False Then
                        For j = 0 To CBL_VentoIntensita.Items.Count - 1
                            If Nota_Cod = CBL_VentoIntensita.Items(j).Value Then
                                CBL_VentoIntensita.Items(j).Selected = True
                                TrovataNota = True
                                Continue For
                            End If
                        Next
                    End If
                    If CBL_VentoDirezione.Visible = True And TrovataNota = False Then
                        For j = 0 To CBL_VentoDirezione.Items.Count - 1
                            If Nota_Cod = CBL_VentoDirezione.Items(j).Value Then
                                CBL_VentoDirezione.Items(j).Selected = True
                                TrovataNota = True
                                Continue For
                            End If
                        Next
                    End If
                    If CBL_Temperatura.Visible = True And TrovataNota = False Then
                        For j = 0 To CBL_Temperatura.Items.Count - 1
                            If Nota_Cod = CBL_Temperatura.Items(j).Value Then
                                CBL_Temperatura.Items(j).Selected = True
                                TrovataNota = True
                                Continue For
                            End If
                        Next
                    End If
                    If CBL_Orario.Visible = True And TrovataNota = False Then
                        For j = 0 To CBL_Orario.Items.Count - 1
                            If Nota_Cod = CBL_Orario.Items(j).Value Then
                                CBL_Orario.Items(j).Selected = True
                                TrovataNota = True
                                Continue For
                            End If
                        Next
                    End If
                    If CBL_Motivazione.Visible = True And TrovataNota = False Then
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
                If Not Dt_Nota Is Nothing AndAlso Dt_Nota.Rows.Count > 0 Then
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


    '' Rende visibile il bottone dei costi accessori se c'è il permesso del rilievo attività
    'Public Sub AttivaCostiAccessoriAvanzati()

    '    Dim UtenteAbilitato As Boolean = False

    '    Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
    '    UtenteAbilitato = objPermessi.Controlla_Permessi_Utente( _
    '                                Session("ASG_Utente_Username"), _
    '                                Session("ASG_IdServizio"), _
    '                                enum_Security_Attivita.Rilievo_Attivita, _
    '                                enum_Security_Operazione.Modifica, _
    '                                Date.Now, _
    '                                "", _
    '                                objParametri_Utenti)

    '    If Not UtenteAbilitato Then
    '        DivAccessoriAvanzati.Visible = False
    '    End If


    'End Sub


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

        If Not IsNothing(Session("dtScarico")) Then

            'Vettore di DataColumn
            Dim ScaricoKeys(5) As String
            ScaricoKeys(0) = "Udm_Selezionata"
            ScaricoKeys(1) = "ID_Attivita"
            ScaricoKeys(2) = "Turno_Cod"
            ScaricoKeys(3) = "Qta_Ril"

            ScaricoKeys(4) = "Qualifica_Cod"
            ScaricoKeys(5) = "Tariffa_Cod"

            GridViewCostiAccessoriVisibili.DataSource = Session("dtScarico")
            GridViewCostiAccessoriVisibili.DataKeyNames = ScaricoKeys
            GridViewCostiAccessoriVisibili.DataBind()

            ControllaDDLCostiAccessori(GridViewCostiAccessoriVisibili)

        End If

    End Sub

    'Private Sub AggiornaDgrScarico()

    '    If Not IsNothing(Session("dtScarico")) Then

    '        'Vettore di DataColumn
    '        Dim ScaricoKeys(3) As String
    '        ScaricoKeys(0) = "Udm_Selezionata"
    '        ScaricoKeys(1) = "ID_Attivita"
    '        ScaricoKeys(2) = "Turno_Cod"
    '        ScaricoKeys(3) = "Qta_Ril"

    '        dgrScarico.DataSource = Session("dtScarico")
    '        dgrScarico.DataKeyNames = ScaricoKeys
    '        dgrScarico.DataBind()

    '        'dopo aver fatto il bind verifico come impostare la dropdown
    '        ControllaDDLCostiAccessori(dgrScarico)
    '        ControllaDDLCostiAccessori(dgrScaricoAvanzati)

    '    End If

    'End Sub

    Private Sub ControllaDDLCostiAccessori(ByVal Griglia As GridView)
        Dim i As Integer = 0
        Dim dt As DataTable = Session("dtScarico")
        For i = 0 To Griglia.Rows.Count - 1
            CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Clear()

            If dt.Rows(i).Item("Centro_cod") = -7 Then
                CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("numero", enum_UnitaMisura.Numero))

            ElseIf dt.Rows(i).Item("Centro_cod") < 0 AndAlso Not New ArrayList({-5, -6, -8, -9}).Contains(dt.Rows(i).Item("Centro_cod")) Then
                CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("ora", "2"))
                CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("ha", "1"))
                CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("indefinito", "-1"))
            Else

                AgronicaCoreUtility.CaricaListControl.Udm_Optimize(
                                          CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList),
                                         0,
                                         objParametriAgenda.Piva,
                                         dt.Rows(i).Item("sa_cod"),
                                         dt.Rows(i).Item("Centro_cod"),
                                         CAU_SCARICO,
                                         dt.Rows(i).Item("elem_cod"),
                                         True,
                                         0,
                                         0,
                                         False, "", "",
                                         "", "", "", objParametri_Server, objParametri_Utenti)
                'Select Case dt.Rows(i).Item("elem_cod")
                '    Case 2 'carburanti
                '        'CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("litri", "29"))
                '    Case 200
                '    Case 205
                'End Select
                'CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("kg", "2"))
                'CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Add(New ListItem("litri", "29"))
            End If

            'controllo se l'utente ha cambiato valore
            If Not IsDBNull(dt.Rows(i).Item("Udm_Selezionata")) AndAlso dt.Rows(i).Item("Udm_Selezionata") <> "" Then
                CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).SelectedValue = Griglia.DataKeys(i).Item(0).ToString  'dt.Rows(i).Item("Udm_Selezionata") 
            Else
                Dim j As Integer = 0
                Dim desUdm As String
                If dt.Rows(i).Item("Udm_Des").ToString.Contains(" ") Then
                    desUdm = Split(dt.Rows(i).Item("Udm_Des"), " ")(0)
                Else
                    desUdm = dt.Rows(i).Item("Udm_Des")
                End If

                For j = 0 To CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).Items.Count - 1
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

            Select Case ALGORITMO_COSTI_ACCESSORI
                Case enum_AlgoritmoCostiAccessori.CAB

                    Dim objCmbAttivita As DropDownList = CType(Griglia.Rows(i).FindControl("Cmb_Attivita"), DropDownList)
                    Dim objCmbTurni As DropDownList = CType(Griglia.Rows(i).FindControl("Cmb_Turni"), DropDownList)

                    If Not IsNothing(objCmbAttivita) And Not IsNothing(objCmbTurni) Then

                        ' solo con manodopera, terzisti e tecnico responsabile
                        If dt.Rows(i).Item("Centro_cod") < -1 Then

                            AgronicaCoreUtility.CaricaListControl.Attivita(objCmbAttivita, True, "", "0", 0, "", "", objParametri_Server)
                            AgronicaCoreUtility.CaricaListControl.Turni(objCmbTurni, True, "", "0", 0, "", "", objParametri_Server)

                            Dim StrSelect As New StringBuilder
                            'classe per autocomplete della combo
                            StrSelect.AppendLine("$(document).ready(function () { ")
                            StrSelect.AppendLine("   $('#" & objCmbAttivita.ClientID & "').combobox();")
                            StrSelect.AppendLine("   $('#" & objCmbTurni.ClientID & "').combobox();")
                            StrSelect.AppendLine("});")

                            ScriptManager.RegisterStartupScript(UpdateCostiAccessoriAvanzati, UpdateCostiAccessoriAvanzati.GetType(),
                                                             String.Format("jQuery_{0}", objCmbAttivita.ClientID), StrSelect.ToString, True)

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

                Case enum_AlgoritmoCostiAccessori.SBTF

                    Dim objCmbAttivita As DropDownList = CType(Griglia.Rows(i).FindControl("Cmb_Attivita"), DropDownList)
                    Dim objCmbQualifica As DropDownList = CType(Griglia.Rows(i).FindControl("Cmb_Qualifica"), DropDownList)
                    Dim objCmbTariffa As DropDownList = CType(Griglia.Rows(i).FindControl("Cmb_Tariffa"), DropDownList)

                    If Not IsNothing(objCmbAttivita) AndAlso Not IsNothing(objCmbQualifica) AndAlso Not IsNothing(objCmbTariffa) Then

                        ' solo con manodopera, terzisti e tecnico responsabile
                        If dt.Rows(i).Item("Centro_cod") < -1 AndAlso Not New ArrayList({-5, -6, -7, -8, -9}).Contains(dt.Rows(i).Item("Centro_cod")) Then

                            'Carico la combo delle attività in base al lav_cod
                            CaricaListControl.AttivitaXLavCod(objCmbAttivita, True, "", "0", objParametriAgenda.Lav_Cod, "", "", objParametri_Server)
                            'Carico la combo con tutte le qualifiche
                            CaricaListControl.Qualifica(objCmbQualifica, True, "", "0", 0, "", "", objParametri_Server)

                            'If IsNumeric(dt.Rows(i).Item("Cod_Rapporto")) AndAlso (dt.Rows(i).Item("Cod_Rapporto") = CostantiPersonalizzate.COD_DIPENDENTE OrElse dt.Rows(i).Item("Cod_Rapporto") = CostantiPersonalizzate.COD_TERZISTA) Then 'Dipendente
                            If IsNumeric(dt.Rows(i).Item("Cod_Rapporto")) AndAlso dt.Rows(i).Item("Cod_Rapporto") = CostantiPersonalizzate.COD_DIPENDENTE Then 'Dipendente

                                'Imposto la qualifica in base a quella del contatto
                                If IsNumeric(dt.Rows(i).Item("Qualifica_Cod")) AndAlso dt.Rows(i).Item("Qualifica_Cod") > 0 Then

                                    'Carico la combo delle tariffe in base alla qualifica
                                    CaricaListControl.TariffeXQualifica(objCmbTariffa, True, "", "0", dt.Rows(i).Item("Qualifica_Cod"), txt_DataOperazione.Text, "", "", objParametri_Server)
                                    'Imposto la qualifica dai dati passati
                                    objCmbQualifica.SelectedIndex = objCmbQualifica.Items.IndexOf(objCmbQualifica.Items.FindByValue(Griglia.DataKeys(i).Item(4).ToString))

                                    'Imposto la Tariffa (quella impostata o la prima se ce n'é una sola)
                                    If IsNumeric(dt.Rows(i).Item("Tariffa_Cod")) AndAlso dt.Rows(i).Item("Tariffa_Cod") <> 0 _
                                                AndAlso Not IsNothing(objCmbTariffa.Items.FindByValue(Griglia.DataKeys(i).Item(5).ToString)) Then

                                        objCmbTariffa.SelectedIndex = objCmbTariffa.Items.IndexOf(objCmbTariffa.Items.FindByValue(Griglia.DataKeys(i).Item(5).ToString))
                                    ElseIf objCmbTariffa.Items.Count = 2 Then
                                        objCmbTariffa.SelectedIndex = 1
                                    End If

                                    'aggiorno i costi unitario e totale in base alla qualifica
                                    If Not IsNothing(objCmbQualifica.SelectedItem) AndAlso Not IsNothing(objCmbTariffa.SelectedItem) _
                                        AndAlso objCmbQualifica.SelectedValue <> 0 AndAlso objCmbTariffa.SelectedValue <> 0 Then

                                        '  Marco Grilli, 08/09/2016 12:28:47: Leggo il costo orario data tariffa e qualifica
                                        Dim objQxT_R As New AgronicaCoreContabDAL.QualificheXTariffe_R
                                        Dim dtQxT As DataTable = objQxT_R.Leggi(objParametriAgenda.Piva, 1, objCmbQualifica.SelectedValue, objCmbTariffa.SelectedValue, "", "", objParametri_Server)

                                        '  Marco Grilli, 08/09/2016 12:29:05: se ho trovato una tariffa per la specifica qualifica, la imposto
                                        If dtQxT.Rows.Count = 1 Then
                                            dt.Rows(i).Item("Costo_Unitario") = Format(dtQxT.Rows(0).Item("Valore"), "0.00")
                                            dt.Rows(i).Item("Costo") = Format(dt.Rows(i).Item("Costo_Unitario") * dt.Rows(i).Item("Qta_Ril"), "0.00")
                                            dgrScaricoAvanzati.Rows(i).Cells(11).Text = dt.Rows(i).Item("Costo_Unitario")
                                            dgrScaricoAvanzati.Rows(i).Cells(12).Text = dt.Rows(i).Item("Costo")
                                        Else
                                            'ERRORE
                                        End If
                                    End If

                                End If

                                'Imposto la l'Attività (quella impostata o la prima se è una sola)
                                If IsNumeric(dt.Rows(i).Item("Id_Attivita")) AndAlso dt.Rows(i).Item("Id_Attivita") <> 0 _
                                            AndAlso Not IsNothing(objCmbAttivita.Items.FindByValue(Griglia.DataKeys(i).Item(1).ToString)) Then

                                    objCmbAttivita.SelectedIndex = objCmbAttivita.Items.IndexOf(objCmbAttivita.Items.FindByValue(Griglia.DataKeys(i).Item(1).ToString))
                                ElseIf IsNumeric(Property_AttivitaDefaultCostiAccessori) Then
                                    objCmbAttivita.SelectedIndex = objCmbAttivita.Items.IndexOf(objCmbAttivita.Items.FindByValue(Property_AttivitaDefaultCostiAccessori.ToString))
                                ElseIf objCmbAttivita.Items.Count = 2 Then
                                    objCmbAttivita.SelectedIndex = 1
                                End If


                            ElseIf IsNumeric(dt.Rows(i).Item("Cod_Rapporto")) AndAlso dt.Rows(i).Item("Cod_Rapporto") = CostantiPersonalizzate.COD_AVVENTIZIO Then 'Avventizio

                                'SE NON HO SELEZIONATO NESSUNA ATTIVITA'
                                If IsNothing(dt.Rows(i).Item("Id_Attivita")) OrElse dt.Rows(i).Item("Id_Attivita") = 0 Then

                                    Dim attivitaDaValutare As Integer = 0
                                    Dim qualificaDaValutare As Integer = 0

                                    'SE ESISTONO ATTIVITA' (IN GENERALE)
                                    If objCmbAttivita.Items.Count > 1 Then

                                        If IsNumeric(Property_AttivitaDefaultCostiAccessori) Then
                                            'Se Presente imposto l'attività di default
                                            attivitaDaValutare = Property_AttivitaDefaultCostiAccessori
                                        ElseIf objCmbAttivita.Items.Count = 2 Then
                                            'Estraggo la prima attività
                                            attivitaDaValutare = objCmbAttivita.Items(1).Value
                                        End If

                                        If attivitaDaValutare <> 0 Then
                                            'Seleziono l'attivita nella combo
                                            objCmbAttivita.SelectedIndex = objCmbAttivita.Items.IndexOf(objCmbAttivita.Items.FindByValue(attivitaDaValutare))

                                            'estraggo la qualifica minima per l'attività/lavorazione
                                            Dim objAttivita_R As New AgronicaCoreContabDAL.Attivita_R
                                            Dim dtAttivita As DataTable = objAttivita_R.Leggi(attivitaDaValutare, "", "", objParametri_Server)
                                            qualificaDaValutare = dtAttivita.Rows(0).Item("Qualifica_Cod_Min") 'Prendo la qualifica minima per quell'attività
                                        End If

                                        If qualificaDaValutare <> 0 Then
                                            'Seleziono la qualifica nella combo
                                            objCmbQualifica.SelectedIndex = objCmbQualifica.Items.IndexOf(objCmbQualifica.Items.FindByValue(qualificaDaValutare))

                                            'Carico la combo con le tariffe per la specifica qualifica
                                            CaricaListControl.TariffeXQualifica(objCmbTariffa, True, "", "0", qualificaDaValutare, txt_DataOperazione.Text, "", "", objParametri_Server)
                                        End If

                                        'Imposto la tariffa
                                        Dim tariffaDaValutare As Integer = 0
                                        If IsNumeric(dt.Rows(i).Item("Tariffa_cod")) AndAlso dt.Rows(i).Item("Tariffa_cod") <> 0 Then
                                            tariffaDaValutare = dt.Rows(i).Item("Tariffa_cod")
                                        ElseIf objCmbTariffa.Items.Count = 2 Then
                                            tariffaDaValutare = objCmbTariffa.Items(1).Value
                                        End If

                                        If tariffaDaValutare <> 0 Then
                                            objCmbTariffa.SelectedIndex = objCmbTariffa.Items.IndexOf(objCmbTariffa.Items.FindByValue(tariffaDaValutare))
                                        End If

                                        'aggiorno i costi unitario e totale in base alla qualifica e tariffa
                                        If qualificaDaValutare <> 0 AndAlso tariffaDaValutare <> 0 Then
                                            '  Marco Grilli, 08/09/2016 12:28:47: Leggo il costo orario data tariffa e qualifica
                                            Dim objQxT_R As New AgronicaCoreContabDAL.QualificheXTariffe_R
                                            Dim dtQxT As DataTable = objQxT_R.Leggi(objParametriAgenda.Piva, 1, qualificaDaValutare, tariffaDaValutare, "", "", objParametri_Server)

                                            '  Marco Grilli, 08/09/2016 12:29:05: se ho trovato una tariffa per la specifica qualifica, la imposto
                                            If dtQxT.Rows.Count = 1 Then
                                                dt.Rows(i).Item("Costo_Unitario") = Format(dtQxT.Rows(0).Item("Valore"), "0.00")
                                                dt.Rows(i).Item("Costo") = Format(dt.Rows(i).Item("Costo_Unitario") * dt.Rows(i).Item("Qta_Ril"), "0.00")
                                                dgrScaricoAvanzati.Rows(i).Cells(11).Text = dt.Rows(i).Item("Costo_Unitario")
                                                dgrScaricoAvanzati.Rows(i).Cells(12).Text = dt.Rows(i).Item("Costo")
                                            Else
                                                'ERRORE
                                            End If

                                        End If

                                    End If

                                ElseIf IsNumeric(dt.Rows(i).Item("Id_Attivita")) AndAlso dt.Rows(i).Item("Id_Attivita") > 0 Then

                                    'Seleziono l'attivita nella combo
                                    objCmbAttivita.SelectedIndex = objCmbAttivita.Items.IndexOf(objCmbAttivita.Items.FindByValue(dt.Rows(i).Item("Id_Attivita")))

                                    'Salvo l'attività nel viewstate (sovrascrivo sempre e quindi mi ritrovo l'ultima)
                                    Property_AttivitaDefaultCostiAccessori = dt.Rows(i).Item("Id_Attivita")

                                    Dim qualificaDaValutare As Integer = 0

                                    'calcolo la qualifica solo se è cambiata l'attività
                                    If ComboModificata.StartsWith("ATT") AndAlso i = CInt(ComboModificata.Replace("ATT|", "")) Then

                                        'estraggo la qualifica minima per l'attività/lavorazione
                                        Dim objAttivita_R As New AgronicaCoreContabDAL.Attivita_R
                                        Dim dtAttivita As DataTable = objAttivita_R.Leggi(dt.Rows(i).Item("Id_Attivita"), "", "", objParametri_Server)
                                        qualificaDaValutare = dtAttivita.Rows(0).Item("Qualifica_Cod_Min") 'Prendo la qualifica minima per quell'attività

                                    Else

                                        If IsNumeric(dt.Rows(i).Item("Qualifica_Cod")) AndAlso dt.Rows(i).Item("Qualifica_Cod") <> 0 Then
                                            qualificaDaValutare = dt.Rows(i).Item("Qualifica_Cod")
                                        ElseIf objCmbQualifica.Items.Count = 2 Then
                                            qualificaDaValutare = objCmbQualifica.Items(1).Value
                                        End If

                                    End If

                                    'Seleziono la qualifica nella combo
                                    objCmbQualifica.SelectedIndex = objCmbQualifica.Items.IndexOf(objCmbQualifica.Items.FindByValue(qualificaDaValutare))

                                    'Carico la combo con le tariffe per la specifica qualifica
                                    CaricaListControl.TariffeXQualifica(objCmbTariffa, True, "", "0", qualificaDaValutare, txt_DataOperazione.Text, "", "", objParametri_Server)

                                    'Imposto la tariffa
                                    Dim tariffaDaValutare As Integer = 0
                                    If IsNumeric(dt.Rows(i).Item("Tariffa_cod")) AndAlso dt.Rows(i).Item("Tariffa_cod") <> 0 Then
                                        tariffaDaValutare = dt.Rows(i).Item("Tariffa_cod")
                                    ElseIf objCmbTariffa.Items.Count = 2 Then
                                        tariffaDaValutare = objCmbTariffa.Items(1).Value
                                    End If
                                    objCmbTariffa.SelectedIndex = objCmbTariffa.Items.IndexOf(objCmbTariffa.Items.FindByValue(tariffaDaValutare))

                                    'aggiorno i costi unitario e totale in base alla qualifica e tariffa
                                    If qualificaDaValutare <> 0 AndAlso tariffaDaValutare <> 0 Then
                                        '  Marco Grilli, 08/09/2016 12:28:47: Leggo il costo orario data tariffa e qualifica
                                        Dim objQxT_R As New AgronicaCoreContabDAL.QualificheXTariffe_R
                                        Dim dtQxT As DataTable = objQxT_R.Leggi(objParametriAgenda.Piva, 1, qualificaDaValutare, tariffaDaValutare, "", "", objParametri_Server)

                                        '  Marco Grilli, 08/09/2016 12:29:05: se ho trovato una tariffa per la specifica qualifica, la imposto
                                        If dtQxT.Rows.Count = 1 Then
                                            dt.Rows(i).Item("Costo_Unitario") = Format(dtQxT.Rows(0).Item("Valore"), "0.00")
                                            dt.Rows(i).Item("Costo") = Format(dt.Rows(i).Item("Costo_Unitario") * dt.Rows(i).Item("Qta_Ril"), "0.00")
                                            dgrScaricoAvanzati.Rows(i).Cells(11).Text = dt.Rows(i).Item("Costo_Unitario")
                                            dgrScaricoAvanzati.Rows(i).Cells(12).Text = dt.Rows(i).Item("Costo")
                                        Else
                                            'ERRORE
                                        End If

                                    End If

                                End If

                            End If

                            Dim StrSelect As New StringBuilder
                            'classe per autocomplete della combo
                            StrSelect.AppendLine("$(document).ready(function () { ")
                            StrSelect.AppendLine("   $('#" & objCmbAttivita.ClientID & "').combobox();")
                            StrSelect.AppendLine("   $('#" & objCmbQualifica.ClientID & "').combobox();")
                            StrSelect.AppendLine("   $('#" & objCmbTariffa.ClientID & "').combobox();")
                            StrSelect.AppendLine("});")

                            ScriptManager.RegisterStartupScript(UpdateCostiAccessoriAvanzati, UpdateCostiAccessoriAvanzati.GetType(),
                                                             String.Format("jQuery_{0}", objCmbQualifica.ClientID), StrSelect.ToString, True)

                            'If IsNumeric(dt.Rows(i).Item("Id_Attivita")) AndAlso dt.Rows(i).Item("Id_Attivita") <> 0 Then
                            '    objCmbAttivita.SelectedIndex = objCmbAttivita.Items.IndexOf(objCmbAttivita.Items.FindByValue(Griglia.DataKeys(i).Item(1).ToString))
                            'ElseIf objCmbAttivita.Items.Count > 1 Then
                            '    objCmbAttivita.SelectedIndex = 1
                            'End If

                            'If IsNumeric(dt.Rows(i).Item("Tariffa_Cod")) AndAlso dt.Rows(i).Item("Tariffa_Cod") <> 0 Then
                            '    objCmbTariffa.SelectedIndex = objCmbTariffa.Items.IndexOf(objCmbTariffa.Items.FindByValue(Griglia.DataKeys(i).Item(5).ToString))
                            'ElseIf objCmbTariffa.Items.Count > 1 Then
                            '    objCmbTariffa.SelectedIndex = 1
                            'End If

                            ''aggiorno i costi unitario e totale in base alla qualifica
                            'If Not IsNothing(objCmbQualifica.SelectedItem) AndAlso Not IsNothing(objCmbTariffa.SelectedItem) _
                            '    AndAlso objCmbQualifica.SelectedValue <> 0 AndAlso objCmbTariffa.SelectedValue <> 0 Then

                            '    '  Marco Grilli, 08/09/2016 12:28:47: Leggo il costo orario data tariffa e qualifica
                            '    Dim objQxT_R As New AgronicaCoreContabDAL.QualificheXTariffe_R
                            '    Dim dtQxT As DataTable = objQxT_R.Leggi(objParametriAgenda.Piva, 1, objCmbQualifica.SelectedValue, objCmbTariffa.SelectedValue, "", "", objParametri_Server)

                            '    '  Marco Grilli, 08/09/2016 12:29:05: se ho trovato una tariffa per la specifica qualifica, la imposto
                            '    If dtQxT.Rows.Count = 1 Then
                            '        dt.Rows(i).Item("Costo_Unitario") = Format(dtQxT.Rows(0).Item("Valore"), "0.00")
                            '        dt.Rows(i).Item("Costo") = Format(dt.Rows(i).Item("Costo_Unitario") * dt.Rows(i).Item("Qta_Ril"), "0.00")
                            '    Else
                            '        'ERRORE
                            '    End If

                            'End If

                        Else

                            objCmbAttivita.Visible = False
                            objCmbQualifica.Visible = False
                            objCmbTariffa.Visible = False

                        End If


                    End If


            End Select

        Next


    End Sub

    Private Sub BTN_ComboAttivita_Click(sender As Object, e As System.EventArgs) Handles BTN_ComboAttivita.Click

        ComboModificata = "ATT|" & ComboAttivita_IndiceRigaCambiato.Value - 1

        caricaSuSessionDTControlliDgrScaricoAvanzati()
        ' per riapplicare la combo con l'autocomplete
        AggiornaCostiAccessoriAvanzati_Click(Me, Nothing)

        ComboModificata = ""
        ComboAttivita_IndiceRigaCambiato.Value = ""

    End Sub

    Private Sub BTN_ComboTurni_Click(sender As Object, e As System.EventArgs) Handles BTN_ComboTurni.Click
        caricaSuSessionDTControlliDgrScaricoAvanzati()
        ' per riapplicare la combo con l'autocomplete
        AggiornaCostiAccessoriAvanzati_Click(Me, Nothing)

    End Sub

    Private Sub BTN_ComboQualifica_Click(sender As Object, e As System.EventArgs) Handles BTN_ComboQualifica.Click
        caricaSuSessionDTControlliDgrScaricoAvanzati()
        ' per riapplicare la combo con l'autocomplete
        AggiornaCostiAccessoriAvanzati_Click(Me, Nothing)

    End Sub

    Private Sub BTN_ComboTariffa_Click(sender As Object, e As System.EventArgs) Handles BTN_ComboTariffa.Click
        caricaSuSessionDTControlliDgrScaricoAvanzati()
        ' per riapplicare la combo con l'autocomplete
        AggiornaCostiAccessoriAvanzati_Click(Me, Nothing)

    End Sub

    Private Sub BTN_CmbCentriDiCostoAvanzati_Click(sender As Object, e As System.EventArgs) Handles BTN_CmbCentriDiCostoAvanzati.Click
        ' per riapplicare la combo con l'autocomplete
        CmbCentriDiCostoAvanzati_SelectedIndexChanged(Me, e)

    End Sub

    'click sul bottone dei costi accessori
    'Protected Sub AggiornaCostiAccessori_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AggiornaCostiAccessori.Click

    '    tabellaCostiAccessori.Visible = True

    '    Dim dummylist As DataTable
    '    If Not IsNothing(Session("dtScarico")) Then
    '        dummylist = Session("dtScarico")

    '        For i As Integer = 0 To dgrScarico.Rows.Count - 1
    '            dummylist.Rows(i).Item("Udm_Selezionata") = CType(dgrScarico.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).SelectedValue
    '            dummylist.Rows(i).Item("Valore") = CType(dgrScarico.Rows(i).FindControl("Txt_Qta_Ril"), TextBox).Text
    '        Next

    '        Session("dtScarico") = dummylist
    '    End If

    '    Crea_Griglia_CentriCosto()

    '    AggiornaDgrScarico()
    '    Session("dtScarico_old") = Session("dtScarico")

    'End Sub

    ''' <summary>
    ''' salvataggio dei costi accessori
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    'Protected Sub SalvaCostiAccessori_Click(ByVal sender As Object, ByVal e As EventArgs) Handles SalvaCostiAccessori.Click

    '    SalvaCostiAccessori_SuAgendaMovimenti(True, dgrScarico)

    'End Sub

    Protected Sub SalvaCostiAccessori_SuAgendaMovimenti(ByVal LeggiDaGrigliaPopup_O_SoloDaTabellaInSessione As Boolean,
                                                        ByVal Griglia As GridView)

        Dim DT As DataTable = Session("dtScarico")
        'faccio un ciclo prima in modo da identificare i valori che sono stati selezionati

        Dim objCmbAttivita As DropDownList
        Dim objCmbTurni As DropDownList

        Dim objCmbQualifiche As DropDownList
        Dim objCmbTariffe As DropDownList

        If LeggiDaGrigliaPopup_O_SoloDaTabellaInSessione Then
            For i = 0 To Griglia.Rows.Count - 1

                Dim app As String
                app = CType(Griglia.Rows(i).FindControl("Txt_Qta_Ril"), TextBox).Text.Replace(".", ",")
                If IsNumeric(app) = True Then
                    DT.Rows(i).Item("Qta_Ril") = CDbl(app)
                Else
                    DT.Rows(i).Item("Qta_Ril") = 0
                End If

                DT.Rows(i).Item("Udm_Cod") = CInt(CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).SelectedValue)
                DT.Rows(i).Item("Udm_Des") = CType(Griglia.Rows(i).FindControl("Cmb_UdmCosti"), DropDownList).SelectedItem.Text

                Select Case ALGORITMO_COSTI_ACCESSORI
                    Case enum_AlgoritmoCostiAccessori.CAB
                        objCmbTurni = CType(Griglia.Rows(i).FindControl("Cmb_Turni"), DropDownList)
                        objCmbAttivita = CType(Griglia.Rows(i).FindControl("Cmb_Attivita"), DropDownList)

                        If (Not IsNothing(objCmbAttivita) And Not IsNothing(objCmbTurni)) _
                            AndAlso Not IsNothing(objCmbAttivita.SelectedItem) AndAlso Not IsNothing(objCmbTurni.SelectedItem) Then
                            DT.Rows(i).Item("Id_Attivita") = objCmbAttivita.SelectedValue
                            DT.Rows(i).Item("Turno_Cod") = objCmbTurni.SelectedValue
                        Else
                            DT.Rows(i).Item("Id_Attivita") = 0
                            DT.Rows(i).Item("Turno_Cod") = 0
                        End If

                    Case enum_AlgoritmoCostiAccessori.SBTF
                        objCmbAttivita = CType(Griglia.Rows(i).FindControl("Cmb_Attivita"), DropDownList)
                        objCmbQualifiche = CType(Griglia.Rows(i).FindControl("Cmb_Qualifica"), DropDownList)
                        objCmbTariffe = CType(Griglia.Rows(i).FindControl("Cmb_Tariffa"), DropDownList)

                        If (Not IsNothing(objCmbAttivita) AndAlso Not IsNothing(objCmbQualifiche) AndAlso Not IsNothing(objCmbTariffe)) _
                            AndAlso Not IsNothing(objCmbAttivita.SelectedItem) AndAlso Not IsNothing(objCmbQualifiche.SelectedItem) AndAlso Not IsNothing(objCmbTariffe.SelectedItem) Then
                            DT.Rows(i).Item("Id_Attivita") = objCmbAttivita.SelectedValue
                            DT.Rows(i).Item("Qualifica_Cod") = objCmbQualifiche.SelectedValue
                            DT.Rows(i).Item("Tariffa_Cod") = objCmbTariffe.SelectedValue
                        Else
                            DT.Rows(i).Item("Id_Attivita") = 0
                            DT.Rows(i).Item("Qualifica_Cod") = 0
                            DT.Rows(i).Item("Tariffa_Cod") = 0
                        End If
                End Select

            Next

        End If


        Dim MovimentiCosti As List(Of Movimento) = New List(Of Movimento)

        Dim j As Integer = 0
        'devo fare un movimento solo per ogni causale (al max 4)
        For i = -1 To -10 Step -1
            'filtro solo i movimenti di tipo ...
            Dim DR() As DataRow
            If i = -10 Then
                DR = DT.Select("Centro_cod not in (-1,-2,-3,-4,-5,-6,-7,-8,-9)")
            Else
                DR = DT.Select("Centro_cod = " & i)
            End If
            If DR.Length > 0 Then
                'ho qualche movimento da inserire 
                Dim Cau_Mov As Integer = 0
                Dim Sa_Cod As Integer = 0   ' il sa_cod è <> 0 se ho uno scarico 
                Select Case i
                    Case -1
                        Cau_Mov = CAU_IMPUTAZIONE_PARCOMACCHINE
                        'verifico se ho l'acqua impostata 
                        If QtaAcqua.Value <> "" Then
                            'controllo se almeno una macchina ha la taratura ugelli
                            Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
                            For j = 0 To DR.Length - 1
                                'Dim DT_Macc = objContab.MacchinaDes(objParametriAgenda.Piva, _
                                '                       objParametriAgenda.Data, _
                                '                       DR(j).Item("Mat_cod"), _
                                '                        "", "", _
                                '                       objParametri_Server)

                                Dim DT_Macc = objContab.Leggi_daMacCod("",
                                                       DR(j).Item("Mat_cod"),
                                                        "", "",
                                                       objParametri_Server)


                                If Not IsDBNull(DT_Macc.Rows(0).Item("Taratura_Ugello")) Then
                                    Dim Acqua As Decimal
                                    Acqua = DT_Macc.Rows(0).Item("Taratura_Ugello")
                                    QtaAcqua.Value = Acqua
                                    Dim strJS As String = "$(document).ready(function () {AggiornaAcqua();}); "
                                    ScriptManager.RegisterStartupScript(UpdatePanelCostiAccessoriVisibili, UpdatePanelCostiAccessoriVisibili.GetType(),
                                                 String.Format("jQuery_{0}", UpdatePanelCostiAccessoriVisibili.ClientID), strJS, True)

                                    Exit For
                                End If
                            Next
                        End If
                    Case -2
                        Cau_Mov = CAU_IMPUTAZIONE_MANODOPERA
                    Case -3
                        Cau_Mov = CAU_IMPUTAZIONE_TERZISTI
                    Case -4
                        Cau_Mov = CAU_IMPUTAZIONE_TECNICO_RESPONSABILE
                    Case -5, -6, -7, -8, -9
                        Cau_Mov = CAU_SCARICO
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

                movimento.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)
                'creo tanti dettagli quanti sono gli item di dr()


                For j = 0 To DR.Length - 1
                    Dim movDet As New Movimento_Dettaglio
                    'movDet.Cau_Mov = Cau_Mov
                    movDet.Data = objParametriAgenda.Data

                    Dim QTA As Decimal
                    QTA = CDbl(DR(j).Item("Qta_Ril"))
                    movDet.Qta = QTA

                    movDet.Udm_Cod = DR(j).Item("Udm_Cod")
                    movDet.Mat_Cod = DR(j).Item("Mat_cod")
                    movDet.Elem_Cod = DR(j).Item("Elem_Cod")
                    movDet.Pro_Cod = DR(j).Item("Pro_Cod") 'mancava
                    movDet.Piva = objParametriAgenda.Piva
                    movDet.Sa_Cod = Sa_Cod
                    movDet.Extra_Int = DR(j).Item("Ore") * 60 + DR(j).Item("Minuti")
                    movDet.Prezzo_Unitario = DR(j).Item("Costo_Unitario")

                    movDet.Lotto = DR(j).Item("Lotto")

                    movDet.Contabilizzato = NONCONTABILE

                    If Cau_Mov = CAU_SCARICO AndAlso Not New ArrayList({-5, -6, -7, -8, -9}).Contains(i) Then
                        ' movimento.Sa_Cod = DR(j).Item("Sa_Cod")
                        movDet.Sa_Cod = DR(j).Item("Sa_Cod")

                        movDet.Movimenti_Destinazioni = New List(Of Movimento_Destinazione)
                        Dim movDest As New Movimento_Destinazione
                        movDest.Id_Agenda = objParametriAgenda.Id_Agenda
                        movDest.Sa_Cod = DR(j).Item("Sa_Cod")
                        movDest.Piva = objParametriAgenda.Piva
                        movDest.Qta = QTA
                        movDest.Tipo = enum_FabbricatiTipi.MagazzinoAziendale
                        movDest.Id_Destinazione = DR(j).Item("Centro_Cod")
                        movDet.Movimenti_Destinazioni.Add(movDest)
                    End If

                    If Cau_Mov = CAU_IMPUTAZIONE_MANODOPERA Then

                        Select Case ALGORITMO_COSTI_ACCESSORI
                            Case enum_AlgoritmoCostiAccessori.CAB
                                movDet.ID_Attivita = DR(j).Item("Id_Attivita")
                                movDet.Turno_Cod = DR(j).Item("Turno_Cod")

                                If movDet.ID_Attivita <> 0 And movDet.Turno_Cod <> 0 Then

                                    Dim objAttivita As New AgronicaCoreContabDAL.Attivita_R
                                    movDet.Prezzo_Unitario = objAttivita.CostoOrario(movDet.ID_Attivita, movDet.Turno_Cod, movDet.Data, objParametri_Server)
                                    objAttivita = Nothing

                                    ' sono diversi solo se c'è uno sconto (usato per ora solo nelle fatture)
                                    movDet.Prezzo_Unitario_Netto = movDet.Prezzo_Unitario
                                    'movDet.Udm_Cod = 141        'Ore

                                End If
                            Case enum_AlgoritmoCostiAccessori.SBTF
                                movDet.ID_Attivita = DR(j).Item("Id_Attivita")
                                movDet.Qualifica_Cod = DR(j).Item("Qualifica_Cod")
                                movDet.Tariffa_Cod = DR(j).Item("Tariffa_Cod")

                                If movDet.ID_Attivita <> 0 AndAlso movDet.Qualifica_Cod <> 0 AndAlso movDet.Tariffa_Cod <> 0 Then

                                    '  Marco Grilli, 08/09/2016 12:28:47: Leggo il costo orario data tariffa e qualifica
                                    Dim objQxT_R As New AgronicaCoreContabDAL.QualificheXTariffe_R
                                    Dim dtQxT As DataTable = objQxT_R.Leggi(objParametriAgenda.Piva, 1, movDet.Qualifica_Cod, movDet.Tariffa_Cod, "", "", objParametri_Server)
                                    movDet.Prezzo_Unitario = dtQxT.Rows(0).Item("Valore")

                                    ' sono diversi solo se c'è uno sconto (usato per ora solo nelle fatture)
                                    movDet.Prezzo_Unitario_Netto = movDet.Prezzo_Unitario
                                    'movDet.Udm_Cod = 141        'Ore

                                End If

                        End Select




                    End If

                    movimento.Movimenti_Dettagli.Add(movDet)

                Next
                MovimentiCosti.Add(movimento)
            End If
        Next


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
    'Protected Sub AnnullaCostiAccessori_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AnnullaCostiAccessori.Click
    '    Session("dtScarico") = Session("dtScarico_old")
    '    AggiornaGridViewCostiAccessoriVisibili()
    'End Sub

    '##########################################################################################
    'Private Sub Crea_Griglia_CentriCosto()

    '    'Aggiungo al datagrid dei centri di costo i magazzini
    '    Dim objDTableCentriCosto As DataTable
    '    Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
    '    objDTableCentriCosto = objFabbricati.LeggixCostiAccessori(objParametriAgenda.Piva, 0, _
    '                                                              objParametriAgenda.Data, _
    '                                                              AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
    '                                                               "", "", objParametri_Server)

    '    '  "1=2", "", objParametri_Server)

    '    objFabbricati = Nothing

    '    Dim Dr As DataRow

    '    'Aggiungo al datagrid la riga del parco macchine
    '    Dr = objDTableCentriCosto.NewRow
    '    Dr.Item(0) = Resources.AgronicaAgenda_2010.ParcoMacchine
    '    Dr.Item(1) = -1 'Fabbricato_Cod = -1 corrisponde al Parco Macchine
    '    Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
    '    objDTableCentriCosto.Rows.Add(Dr)


    '    'Aggiungo al datagrid la riga della manodopera
    '    Dr = objDTableCentriCosto.NewRow
    '    Dr.Item(0) = Resources.AgronicaAgenda_2010.Manodopera
    '    Dr.Item(1) = -2 'Fabbricato_Cod = -2 corrisponde alla Manodopera
    '    Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
    '    objDTableCentriCosto.Rows.Add(Dr)


    '    'Aggiungo al datagrid la riga del terzisti
    '    Dr = objDTableCentriCosto.NewRow
    '    Dr.Item(0) = Resources.AgronicaAgenda_2010.CTerzisti
    '    Dr.Item(1) = -3 'Fabbricato_Cod = -3 corrisponde ai C/Terzisti
    '    Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
    '    objDTableCentriCosto.Rows.Add(Dr)

    '    'Aggiungo al datagrid la tecnico responsabile
    '    Dr = objDTableCentriCosto.NewRow
    '    Dr.Item(0) = Resources.AgronicaAgenda_2010.TecnicoResponsabile
    '    Dr.Item(1) = -4 'Fabbricato_Cod = -3 corrisponde ai C/Terzisti
    '    Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
    '    objDTableCentriCosto.Rows.Add(Dr)

    '    'metto il DT all'interno del Session
    '    Session("DT_CentriCosto") = objDTableCentriCosto

    '    'Vettore di DataColumn
    '    Dim CdcKeys(1) As String

    '    'Valorizzo le celle del vettore
    '    CdcKeys(0) = "Sa_Cod"
    '    CdcKeys(1) = "Fabbricato_Cod"

    '    dgrCentriCosto.DataSource = objDTableCentriCosto
    '    dgrCentriCosto.DataKeyNames = CdcKeys
    '    dgrCentriCosto.DataBind()

    '    For i = 0 To dgrCentriCosto.Rows.Count - 1
    '        Dim tipo As String = objDTableCentriCosto.Rows(i).Item(0)
    '        Select Case tipo
    '            Case Resources.AgronicaAgenda_2010.ParcoMacchine
    '                dgrCentriCosto.Rows(i).Cells(1).Controls(0).Visible = False
    '            Case Resources.AgronicaAgenda_2010.Manodopera

    '            Case Resources.AgronicaAgenda_2010.CTerzisti

    '            Case Resources.AgronicaAgenda_2010.TecnicoResponsabile

    '            Case Else
    '                dgrCentriCosto.Rows(i).Cells(1).Controls(0).Visible = False
    '        End Select
    '    Next


    'End Sub

    'Private Sub dgrCentriCosto_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dgrCentriCosto.SelectedIndexChanged
    '    dgrCentriCosto_selezionato_tipo()
    'End Sub

    'Private Sub dgrCentriCosto_selezionato_tipo()
    '    AggiornaCostiAccessori_Click(Me, Nothing)

    '    ' lo rendo visibile solo se ho scelto un magazzino (codice cdc>0)
    '    Filtro_Materiali.Visible = False


    '    ' svuoto la variabile nel viewstate
    '    ViewState("SaCodFabbricato") = ""

    '    Dim DT_CentriCosto As DataTable = Session("DT_CentriCosto")
    '    Dim Dt As DataTable

    '    Select Case CInt(DT_CentriCosto.Rows(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod"))

    '        Case Is > 0

    '            Filtro_Materiali.Visible = True

    '            'Dt = Session("Dt_Prodotti")
    '            Dt = Nothing
    '            Lbl_RisFiltro.Text = ""

    '            ViewState("SaCodFabbricato") = dgrCentriCosto.DataKeys(dgrCentriCosto.SelectedIndex).Item("Sa_Cod")

    '            'Session("Dt_Prodotti") = Dt

    '            Me.dgrMateriali.DataSource = Dt
    '            Me.dgrMateriali.DataBind()

    '            ' commentato e aggiunto exit nicoletta 13/03/2014
    '            Exit Sub

    '            ''Cambio l'intestazione delle colonne nel datagrid
    '            'Me.dgrMateriali.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.CategoriaProdotto
    '            'Me.dgrMateriali.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Prodotto
    '            'Me.dgrMateriali.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.Giacenza

    '            'If Not IsNothing(Session("Dt_Prodotti")) Then
    '            '    Dt = Session("Dt_Prodotti")
    '            'Else
    '            '    Popola_ProdottiMagazzino(0, _
    '            '                             CInt(DT_CentriCosto.Rows(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod")), _
    '            '                             objParametriAgenda.Elem_Cod, _
    '            '                             False)
    '            '    Dt = Session("Dt_Prodotti")
    '            'End If

    '            'Me.dgrMateriali.DataSource = Dt
    '            'Me.dgrMateriali.DataBind()

    '        Case -1

    '            'Cambio l'intestazione delle colonne nel datagrid
    '            dgrMateriali.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.MacchinaAttrezzatura
    '            dgrMateriali.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Descrizione
    '            dgrMateriali.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

    '            If Not IsNothing(Session("Dt_Macchine")) Then
    '                Dt = Session("Dt_Macchine")
    '            Else
    '                Popola_ParcoMacchine()
    '                Dt = Session("Dt_Macchine")
    '            End If

    '            Me.dgrMateriali.DataSource = Dt
    '            Me.dgrMateriali.DataBind()

    '        Case -2

    '            'Cambio l'intestazione delle colonne nel datagrid
    '            Me.dgrMateriali.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.RapportoContabile
    '            Me.dgrMateriali.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Manodopera
    '            Me.dgrMateriali.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

    '            If Not IsNothing(Session("Dt_Manodopera")) Then
    '                Dt = Session("Dt_Manodopera")
    '            Else
    '                Popola_Manodopera()
    '                Dt = Session("Dt_Manodopera")
    '            End If

    '            Me.dgrMateriali.DataSource = Dt
    '            Me.dgrMateriali.DataBind()

    '        Case -3

    '            'Cambio l'intestazione delle colonne nel datagrid
    '            Me.dgrMateriali.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.RapportoContabile
    '            Me.dgrMateriali.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Terzista
    '            Me.dgrMateriali.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

    '            If Not IsNothing(Session("Dt_Terzisti")) Then
    '                Dt = Session("Dt_Terzisti")
    '            Else
    '                Popola_Terzisti()
    '                Dt = Session("Dt_Terzisti")
    '            End If

    '            Me.dgrMateriali.DataSource = Dt
    '            Me.dgrMateriali.DataBind()


    '        Case -4

    '            'Cambio l'intestazione delle colonne nel datagrid
    '            Me.dgrMateriali.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.RapportoContabile
    '            Me.dgrMateriali.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.TecnicoResponsabile
    '            Me.dgrMateriali.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

    '            If Not IsNothing(Session("Dt_TecnicoResponsabile")) Then
    '                Dt = Session("Dt_TecnicoResponsabile")
    '            Else
    '                Popola_TecnicoResponsabile()
    '                Dt = Session("Dt_TecnicoResponsabile")
    '            End If

    '            Me.dgrMateriali.DataSource = Dt
    '            Me.dgrMateriali.DataBind()

    '    End Select

    '    ControllaDDLCostiAccessori(dgrScarico)
    'End Sub
    '''' <summary>
    ''' funzione per la creazione del DT da passare ai gridview dei costi accessori
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Costruisci_DT_Scarico()
        Dim dt As DataTable
        dt = New DataTable("dtScarico")
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
        dt.Columns.Add("Codice", System.Type.GetType("System.String"))
        dt.Columns.Add("Lotto", System.Type.GetType("System.String"))


        'per gli input degli utenti
        dt.Columns.Add("Udm_Selezionata", System.Type.GetType("System.String"))
        dt.Columns.Add("Valore", System.Type.GetType("System.String"))
        dt.Columns.Add("ID_Attivita", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Turno_Cod", System.Type.GetType("System.Int32"))

        dt.Columns.Add("Qualifica_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Tariffa_Cod", System.Type.GetType("System.Int32"))
        dt.Columns.Add("Cod_Rapporto", System.Type.GetType("System.Int32"))

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
                                        ByVal Turno_Cod As Int32,
                                        ByVal Ore As Int32,
                                        ByVal Minuti As Int32,
                                        ByVal Qualifica_Cod As Int32,
                                        ByVal Tariffa_Cod As Int32,
                                        ByVal Cod_Rapporto As Int32,
                                        ByVal DT As DataTable,
                                        Optional ByVal Codice As String = "",
                                        Optional ByVal Lotto As String = "")



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

        Select Case ALGORITMO_COSTI_ACCESSORI

            Case enum_AlgoritmoCostiAccessori.CAB

                ' se uso turni e attivita permetto l'inserimento di più righe uguali
                If ElementoPresente And RigaElemento >= 0 Then
                    If DT.Rows(RigaElemento).Item("Turno_Cod") <> 0 And DT.Rows(RigaElemento).Item("Id_Attivita") <> 0 Then
                        ElementoPresente = False
                    End If
                End If

                '2016-10-03 Su direttiva di Valerio per SBTF evitiamo per ora questi controlli
            Case enum_AlgoritmoCostiAccessori.SBTF
                ElementoPresente = False

        End Select

        'Se esiste gia' allora esco
        If ElementoPresente = True Then
            Messaggio = "Non e' consentito inserire un elemento gia' presente"
            Messaggi.AgroMsgBox(Messaggio, Page, , UpdateCostiAccessoriAvanzati)
            Exit Sub
        End If

        '----- Inserisco il nuovo record

        'Creo una nuova riga
        Dr = DT.NewRow

        'Definisco i valori

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
        Dr.Item("Turno_Cod") = Turno_Cod
        Dr.Item("Ore") = Ore
        Dr.Item("Minuti") = Minuti
        Dr.Item("Qualifica_Cod") = Qualifica_Cod
        Dr.Item("Tariffa_Cod") = Tariffa_Cod
        Dr.Item("Codice") = Codice
        Dr.Item("Cod_Rapporto") = Cod_Rapporto
        Dr.Item("Lotto") = Lotto

        'Associo alla tabella la nuova riga creata
        DT.Rows.Add(Dr)


    End Sub

    'Private Sub dgrScarico_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles dgrScarico.RowCommand

    '    AggiornaCostiAccessori_Click(Me, Nothing)

    '    Dim IndiceRigaGriglia As Integer = 0
    '    Dim Dr As DataRow

    '    IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

    '    Select Case e.CommandName

    '        Case "EliminaCosto"

    '            If Not Session("dtScarico") Is Nothing Then

    '                Dim dtScarico As DataTable = Session("dtScarico")

    '                If dtScarico.Rows.Count >= IndiceRigaGriglia Then

    '                    'Trovo la riga da cancellare    (chiave = FrCod)
    '                    Dr = dtScarico.Rows(IndiceRigaGriglia)

    '                    'Elimino la riga
    '                    Dr.Delete()
    '                    'Salvo il DataTable dentro il Session
    '                    Session("dtScarico") = dtScarico

    '                End If

    '            End If

    '    End Select


    '    AggiornaDgrScarico()
    '    AggiornaDgrScaricoAvanzati()
    '    AggiornaGridViewCostiAccessoriVisibili()

    '    'se sono nella griglia pupup dei costidrg scarico (come in questo caso) non è necessario aggiornare anche la lista dei costi
    '    'prersente nei movimenti dell'agenda, dato che viene ricreata quando si seleziona salva_costi_accessori,
    '    'mentre se si seleziona annulla allora viene ricaricata in sessione la tabella old quindi 
    '    'i movimenti non devono essere toccati
    '    'se invece sono nella griglia a fondo pagina gridviewcostiaccessorivisibili allora se seleziono cancella 
    '    'oltre che a modificare la tabella dei costi in sessione Session("dtScarico") che non verrà mai ripristinata
    '    'dalla versione precedente, dato che l'operazione non è annullabile o confermabile, devo 
    '    'agire anche sulla lista dei movimenti eliminando il movimento 
    '    'corrispondente alla riga selezionata. Se non lo faccio non ho più corrispondenza tra la tabella in sessione e 
    '    'la lista movimenti e dato che la prima corrisponde di solito a ciò che vedo, mentre la seconda 
    '    'corrisponde a ciò che viene salvato mi trovo a salvare cose diverse da quello che vedo
    'End Sub

    Private Sub GridViewCostiAccessoriVisibili_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridViewCostiAccessoriVisibili.RowCommand

        'AggiornaCostiAccessori_Click(Me, Nothing)
        caricaSuSessionDTControlliDgrScaricoAvanzati()
        AggiornaCostiAccessoriAvanzati_Click(Me, Nothing)

        Dim IndiceRigaGriglia As Integer = 0
        Dim Dr As DataRow

        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Select Case e.CommandName

            Case "EliminaCosto"

                If Not Session("dtScarico") Is Nothing Then

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


        'AggiornaDgrScarico()
        AggiornaDgrScaricoAvanzati()
        AggiornaGridViewCostiAccessoriVisibili()
        AggiornaMagazzino()



        '----------------------------------------------------------------------
        '----------------------------------------------------------------------
        'Elimino dai movimenti
        'SalvaCostiAccessori_SuAgendaMovimenti(False, dgrScarico)
        SalvaCostiAccessori_SuAgendaMovimenti(False, dgrScaricoAvanzati)
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
        Dim Costo_Unitario As String = ""
        Dim Costo As String = ""

        Dim Codice As String = ""

        Dim Id_Attivita As Integer
        Dim Turno_Cod As Integer
        Dim Prezzo_Unitario As Decimal

        Dim Ore As Integer
        Dim Minuti As Integer

        Dim Qualifica_Cod As Integer
        Dim Tariffa_Cod As Integer
        Dim Lotto As String = ""

        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim dtScarico As DataTable
        dtScarico = Session("dtScarico")

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
                        Codice = ""

                        '  Marco Grilli, 29/08/2016 10:15:02: Per portarmi dietro anche qualifica e tariffa
                        Qualifica_Cod = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Qualifica_Cod
                        Tariffa_Cod = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Tariffa_Cod

                        Lotto = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Lotto

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

                                Tipo_Centro = "M"

                                If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) AndAlso
                                                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count > 0 Then
                                    Centro_Cod = objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Id_Destinazione
                                Else

                                    Centro_Cod = 0

                                    Select Case ALGORITMO_COSTI_ACCESSORI
                                        Case enum_AlgoritmoCostiAccessori.CAB
                                            Centro_Cod = 0

                                        Case enum_AlgoritmoCostiAccessori.SBTF

                                            Select Case Elem_Cod
                                                Case CARBURANTI
                                                    Centro_Cod = -6
                                                    Tipo_Centro = "XM"
                                                    Centro = "Carburanti non a Magazzino"
                                                Case CAT_MAG_SERVIZI_PROFESSIONALI
                                                    Centro_Cod = -7
                                                    Tipo_Centro = "XM"
                                                    Centro = "Servizi Professionali non a Magazzino"
                                                Case ALTRE_MATERIE
                                                    Centro_Cod = -8
                                                    Tipo_Centro = "XM"
                                                    Centro = "Altre Risorse non a Magazzino"
                                                Case RICAMBI
                                                    Centro_Cod = -9
                                                    Tipo_Centro = "XM"
                                                    Centro = "Ricambi non a Magazzino"
                                                Case Else
                                                    Centro_Cod = 0
                                            End Select

                                    End Select

                                End If

                                'Centro_Cod = objXml.XML_ReadInt(xmlMovimentoDettaglio.GetAttribute("id_destinazione"))
                                ' tag nico sostituito sa_cod
                                Dim objFabb As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                                If Not New ArrayList({-5, -6, -7, -8, -9}).Contains(Centro_Cod) Then
                                    Centro = objFabb.FabbricatoDes_from_FabbricatoCod(
                                                                        objParametriAgenda.Piva,
                                                                        Sa_Cod,
                                                                        Centro_Cod,
                                                                        objParametri_Server) 'Magazzino
                                End If


                                Dim objProdotto As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R

                                Risorsa_Des = objProdotto.Prodotto_Des(objParametriAgenda.Piva,
                                                                       Elem_Cod,
                                                                       Pro_Cod,
                                                                       Mat_Cod,
                                                                       False,
                                                                        strCategoria,
                                                                        objParametri_Server)

                                If Not Risorsa_Des Is DBNull.Value Then
                                    If CStr(Risorsa_Des) <> "" Then Categoria_Des = strCategoria
                                Else
                                    Categoria_Des = ""
                                End If

                                If Elem_Cod = CAT_MAG_SERVIZI_PROFESSIONALI _
                                    AndAlso Lotto <> "" AndAlso Lotto <> "Indefinito" Then
                                    Risorsa_Des &= " (Lotto: " & Lotto & ")"
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


                                'Se sono nei servizi professionali non a magazzino, il prezzo è 0
                                If (IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) OrElse
                                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count = 0
                                    ) AndAlso Elem_Cod = CAT_MAG_SERVIZI_PROFESSIONALI Then

                                    Prezzo = 0
                                Else

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




                                Dim objContab As New AgronicaCoreContabDAL.Parco_Macchine_R
                                'DT = objContab.MacchinaDes(objParametriAgenda.Piva, _
                                '                           objParametriAgenda.Data, _
                                '                           Mat_Cod, _
                                '                            "", "", _
                                '                           objParametri_Server)

                                DT = objContab.Leggi_daMacCod("",
                                                       Mat_Cod,
                                                        "", "",
                                                       objParametri_Server)

                                objContab = Nothing

                                If DT.Rows.Count > 0 Then

                                    Categoria_Des = DT.Rows(0).Item("Class_Desc")
                                    If DT.Rows(0).Item("Mac_Des") <> "" Then
                                        Risorsa_Des = DT.Rows(0).Item("Mac_Des")
                                    Else
                                        Risorsa_Des = DT.Rows(0).Item("Ditta_Des") & " " & DT.Rows(0).Item("Modello")
                                    End If
                                    Costo_Unitario = Format(Prezzo * 1, "0.00")   'Format(CDbl(objRS("Ammortamento").Value), "0.00")
                                    Costo = Format(Prezzo * Qta_Ril, "0.00")
                                    Codice = DT.Rows(0).Item("Codice")

                                End If

                            Case CAU_IMPUTAZIONE_MANODOPERA

                                Dim Corrispettivo_Orario As Decimal = 0

                                Centro_Cod = -2
                                Centro = "Manodopera"
                                Tipo_Centro = "MD"

                                Dim Udm_Des_Contatto As String = ""
                                Dim objAnagrafe As New AgronicaCoreAnagrafeDAL.Contatti_R
                                objAnagrafe.Recupera_DatiContatto2(Mat_Cod, Categoria_Des, Risorsa_Des, Corrispettivo_Orario, Udm_Des_Contatto, 0, "", True, "", objParametri_Server, Cod_Rapporto)
                                objAnagrafe = Nothing

                                Select Case ALGORITMO_COSTI_ACCESSORI
                                    Case enum_AlgoritmoCostiAccessori.CAB
                                        ' se uso i costi accessori avanzati, il costo è dato dal turno e dall'attività, non dal costo unitario della risorsa
                                        If Id_Attivita <> 0 And Turno_Cod <> 0 Then
                                            Costo_Unitario = Format(Prezzo_Unitario, "0.00")
                                            Costo = Format(Prezzo_Unitario * Qta_Ril, "0.00")
                                        Else
                                            'ho dovuto aggiungere il * 1 perchè altrimenti settava a  "0.00"
                                            If Udm_Des = Udm_Des_Contatto Then
                                                Costo_Unitario = Format(Corrispettivo_Orario * 1, "0.00")
                                                Costo = Format(Corrispettivo_Orario * Qta_Ril, "0.00")
                                            Else
                                                Costo_Unitario = "0.00"
                                                Costo = "0.00"
                                            End If
                                        End If

                                    Case enum_AlgoritmoCostiAccessori.SBTF
                                        ' se uso i costi accessori avanzati, il costo è dato dalla qualifica, dalla tariffa e dall'attività, non dal costo unitario della risorsa
                                        If Id_Attivita <> 0 AndAlso Qualifica_Cod AndAlso Tariffa_Cod <> 0 Then
                                            Costo_Unitario = Format(Prezzo_Unitario, "0.00")
                                            Costo = Format(Prezzo_Unitario * Qta_Ril, "0.00")
                                        Else
                                            'GRILLI 2016-09-26: Il corrispettivo orario nell'agoritmo sbtf non ha senso
                                            'ho dovuto aggiungere il * 1 perchè altrimenti settava a  "0.00"
                                            'If Udm_Des = Udm_Des_Contatto Then
                                            '    Costo_Unitario = Format(Corrispettivo_Orario * 1, "0.00")
                                            '    Costo = Format(Corrispettivo_Orario * Qta_Ril, "0.00")
                                            'Else
                                            Costo_Unitario = "0.00"
                                            Costo = "0.00"
                                            'End If
                                        End If


                                End Select




                            Case CAU_IMPUTAZIONE_TERZISTI

                                Dim Corrispettivo_Orario As Decimal = 0

                                Centro_Cod = -3
                                Centro = "C/Terzisti"
                                Tipo_Centro = "CT"

                                Dim Udm_Des_Contatto As String = ""
                                Dim objAnagrafe As New AgronicaCoreAnagrafeDAL.Contatti_R
                                objAnagrafe.Recupera_DatiContatto2(Mat_Cod, Categoria_Des, Risorsa_Des, Corrispettivo_Orario, Udm_Des_Contatto, 0, "", True, "", objParametri_Server, Cod_Rapporto)
                                'ho dovuto aggiungere il * 1 perchè altrimenti settava a  "0.00"
                                objAnagrafe = Nothing

                                If Udm_Des = Udm_Des_Contatto Then
                                    Costo_Unitario = Format(Corrispettivo_Orario * 1, "0.00")
                                    Costo = Format(Corrispettivo_Orario * Qta_Ril, "0.00")
                                Else
                                    Costo_Unitario = "0.00"
                                    Costo = "0.00"
                                End If

                            Case CAU_IMPUTAZIONE_TECNICO_RESPONSABILE

                                Dim Corrispettivo_Orario As Decimal = 0

                                Centro_Cod = -4
                                Centro = "Tecnico Responsabile"
                                Tipo_Centro = "TR"

                                Dim Udm_Des_Contatto As String = ""
                                Dim objAnagrafe As New AgronicaCoreAnagrafeDAL.Contatti_R
                                objAnagrafe.Recupera_DatiContatto2(Mat_Cod, Categoria_Des, Risorsa_Des, Corrispettivo_Orario, Udm_Des_Contatto, 0, "", True, "", objParametri_Server, Cod_Rapporto)
                                'ho dovuto aggiungere il * 1 perchè altrimenti settava a  "0.00"
                                objAnagrafe = Nothing

                                If Udm_Des = Udm_Des_Contatto Then
                                    Costo_Unitario = Format(Corrispettivo_Orario * 1, "0.00")
                                    Costo = Format(Corrispettivo_Orario * Qta_Ril, "0.00")
                                Else
                                    Costo_Unitario = "0.00"
                                    Costo = "0.00"
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
                                                Id_Attivita,
                                                Turno_Cod,
                                                Ore, Minuti,
                                                Qualifica_Cod,
                                                Tariffa_Cod,
                                                Cod_Rapporto,
                                                dtScarico,
                                                Codice,
                                                Lotto)



                    Next

                End If

            Next

        End If

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

    'Private Sub dgrMateriali_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles dgrMateriali.RowCommand


    '    AggiornaCostiAccessori_Click(Me, Nothing)



    '    Dim IndiceRigaGriglia As Integer = 0

    '    Dim Centro As String = ""
    '    Dim Centro_Cod As Int32
    '    Dim Categoria_Des As String = ""
    '    Dim Risorsa_Des As String = ""
    '    Dim Udm_Des As String = ""
    '    Dim Qta_Ril As Decimal
    '    Dim Udm_Cod As Int32
    '    Dim Elem_Cod As Int32
    '    Dim Riga As Int32
    '    Dim Tipo_Centro As String = ""
    '    Dim Pro_Cod As Int32
    '    Dim Ditta_Cod As Int32
    '    Dim Mat_Cod As Int32
    '    Dim Costo_Unitario As String = ""
    '    Dim Costo As String = ""
    '    Dim Sa_Cod As Integer

    '    IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

    '    Dim dtScarico As DataTable

    '    dtScarico = Session("dtScarico")

    '    Select Case e.CommandName

    '        Case "AggiungiCosto"

    '            If Not Session("DT_CentriCosto") Is Nothing Then

    '                Dim DT_CentriCosto As DataTable = Session("DT_CentriCosto")

    '                Select Case CInt(DT_CentriCosto.Rows(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod"))

    '                    Case Is > 0

    '                        If Not IsNothing(Session("Dt_Prodotti")) Then

    '                            Dim Dt_Prodotti As DataTable = Session("Dt_Prodotti")

    '                            If Dt_Prodotti.Rows.Count >= IndiceRigaGriglia Then

    '                                Dim objFabb As New AgronicaCoreAnagrafeDAL.Fabbricati_R
    '                                Centro = objFabb.FabbricatoDes_from_FabbricatoCod( _
    '                                            objParametriAgenda.Piva, _
    '                                            dgrCentriCosto.DataKeys(dgrCentriCosto.SelectedIndex).Item("Sa_Cod"), _
    '                                            dgrCentriCosto.DataKeys(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod"), _
    '                                            objParametri_Server) 'Magazzino

    '                                Tipo_Centro = "M"
    '                                Centro_Cod = CInt(dgrCentriCosto.DataKeys(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod"))
    '                                Sa_Cod = dgrCentriCosto.DataKeys(dgrCentriCosto.SelectedIndex).Item("Sa_Cod")

    '                                Elem_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_6")
    '                                Pro_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_3")
    '                                Mat_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_8")

    '                                Categoria_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_0")
    '                                Risorsa_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_1")

    '                                Udm_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_2")
    '                                Udm_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_5")

    '                                Qta_Ril = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_4")
    '                                Costo_Unitario = Format(Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
    '                                Costo = Format(Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

    '                                Ditta_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_7")
    '                                Riga = 0 'IndiceRigaGriglia

    '                            End If

    '                        End If

    '                    Case -1

    '                        If Not IsNothing(Session("Dt_Macchine")) Then

    '                            Dim Dt_Macchine As DataTable = Session("Dt_Macchine")

    '                            If Dt_Macchine.Rows.Count >= IndiceRigaGriglia Then

    '                                Centro = "Parco Macchine"
    '                                Tipo_Centro = "PM"
    '                                Centro_Cod = -1
    '                                Sa_Cod = 0

    '                                Categoria_Des = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_0")
    '                                Risorsa_Des = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_1")

    '                                Elem_Cod = MACCHINE
    '                                Pro_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_3")
    '                                Mat_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_8")

    '                                Udm_Des = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_2")
    '                                Udm_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_5")

    '                                Qta_Ril = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_4")
    '                                Costo_Unitario = Format(Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
    '                                Costo = Format(Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

    '                                Ditta_Cod = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_7")
    '                                Riga = 0 'IndiceRigaGriglia

    '                            End If

    '                        End If

    '                    Case -2

    '                        If Not IsNothing(Session("Dt_Manodopera")) Then

    '                            Dim Dt_Manodopera As DataTable = Session("Dt_Manodopera")

    '                            If Dt_Manodopera.Rows.Count >= IndiceRigaGriglia Then

    '                                Centro = "Manodopera"
    '                                Tipo_Centro = "MD"
    '                                Centro_Cod = -2
    '                                Sa_Cod = 0

    '                                Categoria_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")
    '                                Risorsa_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_1")

    '                                Elem_Cod = 0
    '                                Pro_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_3")
    '                                Mat_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_8")

    '                                Udm_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_2")
    '                                Udm_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_5")

    '                                Qta_Ril = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_4")
    '                                Costo_Unitario = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
    '                                Costo = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

    '                                Ditta_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_7")
    '                                Riga = 0 'IndiceRigaGriglia

    '                            End If

    '                        End If

    '                    Case -3 'TERZISTI


    '                        If Not IsNothing(Session("Dt_Terzisti")) Then

    '                            Dim Dt_Manodopera As DataTable = Session("Dt_Terzisti")

    '                            If Dt_Manodopera.Rows.Count >= IndiceRigaGriglia Then

    '                                Centro_Cod = -3
    '                                Centro = "C/Terzisti"
    '                                Tipo_Centro = "CT"
    '                                Sa_Cod = 0

    '                                Categoria_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")
    '                                Risorsa_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_1")

    '                                Elem_Cod = 0
    '                                Pro_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_3")
    '                                Mat_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_8")

    '                                Udm_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_2")
    '                                Udm_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_5")

    '                                Qta_Ril = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_4")
    '                                Costo_Unitario = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
    '                                Costo = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

    '                                Ditta_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_7")
    '                                Riga = 0 'IndiceRigaGriglia

    '                            End If

    '                        End If



    '                    Case -4  'Tecnico Responsabile

    '                        If Not IsNothing(Session("Dt_TecnicoResponsabile")) Then

    '                            Dim Dt_Manodopera As DataTable = Session("Dt_TecnicoResponsabile")

    '                            If Dt_Manodopera.Rows.Count >= IndiceRigaGriglia Then

    '                                Centro_Cod = -4
    '                                Centro = "Tecnico Responsabile"
    '                                Tipo_Centro = "TR"
    '                                Sa_Cod = 0

    '                                Categoria_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")
    '                                Risorsa_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_1")

    '                                Elem_Cod = 0
    '                                Pro_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_3")
    '                                Mat_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_8")

    '                                Udm_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_2")
    '                                Udm_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_5")

    '                                Qta_Ril = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_4")
    '                                Costo_Unitario = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
    '                                Costo = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

    '                                Ditta_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_7")
    '                                Riga = 0 'IndiceRigaGriglia

    '                            End If

    '                        End If

    '                End Select

    '                InserisciRiga_dtScarico(Centro, _
    '                                        Sa_Cod, _
    '                                        Centro_Cod, _
    '                                        Categoria_Des, _
    '                                        Risorsa_Des, _
    '                                        Udm_Des, _
    '                                        Qta_Ril, _
    '                                        Udm_Cod, _
    '                                        Elem_Cod, _
    '                                        Riga, _
    '                                        Tipo_Centro, _
    '                                        Pro_Cod, _
    '                                        Ditta_Cod, _
    '                                        Mat_Cod, _
    '                                        Costo_Unitario, _
    '                                        Costo, _
    '                                        0, 0, _
    '                                        0, 0, _
    '                                        dtScarico)


    '            End If

    '            'InserisciCosto(CType(dgrMateriali.Rows(IndiceRigaGriglia).FindControl("Col_4"), TextBox), Nothing)

    '    End Select

    '    Session("dtScarico") = dtScarico
    '    AggiornaDgrScarico()
    'End Sub

    Public Sub Popola_TuttiProdotti(elem_cod As Integer)

        Dim objDt As DataTable = Costruisci_Dt_Materiali()

        Dim catmag_R As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R()
        Dim DTCatMag As DataTable = catmag_R.Leggi(elem_cod, "", False, "", "", objParametri_Server)
        Dim nomeCatergoria As String = DTCatMag.Rows(0).Item("NomeComune")

        Dim matPri_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim DTProdotti As DataTable = matPri_R.LeggiJoinUdmExtra(0, elem_cod, -1, "", "", objParametri_Server)

        'Dim car_R As New AgronicaCoreMetaSchemaDAL.Carburanti_R
        'Dim dtcar As DataTable = car_R.Leggi(0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

        Dim filtroRicerca As String = Txt_FiltroMaterialiAvanzati.Text
        Dim dRow As DataRow()

        If filtroRicerca <> "" Then
            dRow = DTProdotti.Select("Mat_Des like '%" & filtroRicerca & "%' OR Cod_Articolo like '%" & filtroRicerca & "%'")
        Else
            dRow = DTProdotti.Select()
        End If

        For Each riga As DataRow In dRow

            Dim DR As DataRow = objDt.NewRow

            DR("Col_0") = nomeCatergoria
            DR("Col_1") = riga.Item("Mat_Des")
            DR("Col_2") = riga.Item("Udm_Des")
            DR("Col_3") = 0 'Pro_Cod
            DR("Col_4") = 0 'QTA
            DR("Col_5") = riga.Item("Udm_Cod")
            DR("Col_6") = riga.Item("Elem_Cod")
            DR("Col_7") = riga.Item("Ditta_Cod")
            DR("Col_8") = riga.Item("Mat_Cod")
            DR("Col_9") = 0D 'Prezzo unitario
            DR("Col_10") = 0 'Qualifica_Cod
            DR("Col_11") = 0 'Cod_Rapporto
            DR("Col_12") = riga.Item("Cod_Articolo") 'Lotto

            objDt.Rows.Add(DR)
        Next

        Session("Dt_TuttiProdotti") = objDt

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
            'txtFiltro = Txt_FiltroMateriali
            'LblRisultatoRicerca = Lbl_RisFiltro
        End If

        Try

            objDt = Costruisci_Dt_Materiali()

            'Dim filtro As String = ""
            'If Elem_Cod = 197 Then
            '    filtro = " elem_cod <> " + CStr(Elem_Cod) + " AND elem_cod <> 198"
            'Else
            '    filtro = " elem_cod <> " + CStr(Elem_Cod) + " "
            'End If

            Dim strFiltroDesc As String = " LIKE '%" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(txtFiltro.Text) & "%'"


            Dim objGiac As New AgronicaCoreContabDAL.Giacenze_R

            '" AND CategorieMagazzino.Elem_Cod IN (2,200,205,700) ", _
            DTGiacenze = objGiac.SchedaGiacenzeMagazzino(objParametriAgenda.Data,
                                                       objParametriAgenda.Piva, Sa_Cod,
                                                       Destinazione, 0, 0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, True,
                                                       " AND CategorieMagazzino.Elem_Cod = " & Elem_Cod,
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
                                                       "", objParametri_Server, objParametri_Utenti,
                                                       " AND Mat_Des " & strFiltroDesc,
                                                         xFiltroAggiuntivo_14:=" AND Mat_Des " & strFiltroDesc,
                                                         xFiltroAggiuntivo_15:=" AND Mat_Des " & strFiltroDesc)



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
                    DR("Col_12") = DrGiacenze(i).Item("Lotto")

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

            'Preparo la query per recuperare le macchine del centro aziendale
            'Sa_Cod=-1 : Macchine di un altro centro a disposizione di tutti
            Dim Str As String = ""
            If Txt_FiltroMaterialiAvanzati.Text <> "" Then
                Str = " PM.Mac_Des LIKE '%" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(Txt_FiltroMaterialiAvanzati.Text) & "%'"
            End If

            Dim objPM As New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim Dt_Macchine As DataTable
            Dt_Macchine = objPM.LeggiMacchine_xCostiAccessori(objParametriAgenda.Piva,
                                                                           objParametriAgenda.Data,
                                                                           Str, "", objParametri_Server)
            objPM = Nothing



            For i = 0 To Dt_Macchine.Rows.Count - 1

                Dr = objDTableParcoMacchine.NewRow()

                Dr.Item("Col_0") = Dt_Macchine.Rows(i).Item("Col_0")
                Dr.Item("Col_1") = Dt_Macchine.Rows(i).Item("Col_1")
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
                    'Grilli: c'era scritto "indefinito, ma valerio ha voluto che mettessi "ora" per SBTF per coerenza col LAN
                    Dr.Item("Col_2") = "ora"
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
                            'Grilli: c'era scritto "indefinito, ma valerio ha voluto che mettessi "ora" per SBTF per coerenza col LAN
                            Dr.Item("Col_2") = "ora"
                            Dr.Item("Col_5") = "-1"
                    End Select
                End If




                Dr.Item("Col_9") = Prezzo_Unitario

                Dr.Item("Col_3") = 0
                Dr.Item("Col_4") = 0
                Dr.Item("Col_6") = MACCHINE
                Dr.Item("Col_7") = 0

                Dr.Item("Col_13") = Dt_Macchine.Rows(i).Item("Col_13")

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
            Dim Dt_Manodopera As DataTable

            Dim Str As String = ""
            'Str = " ( (Rapporti_Contabili.Cod_Rapporto in(-1,-4,-6))  or Rapporti_Contabili.Dipendente=1 ) "
            Str = " (( Rapporti_Contabili.Cod_Rapporto in(-1,-4,-6) ) or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Legale=1) "

            If Txt_FiltroMaterialiAvanzati.Text <> "" Then
                Str &= " AND CASE WHEN (Contatti.Rag_Soc is null or Contatti.Rag_Soc = '') then Contatti.Cognome  + ' ' +  Contatti.Nome  else Contatti.Rag_Soc End  LIKE '%" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(Txt_FiltroMaterialiAvanzati.Text) & "%'"
            End If
            'Dim strFiltroDesc As String = " LIKE '%" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(Txt_FiltroMaterialiAvanzati.Text) & "%'"

            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessori(objParametriAgenda.Piva,
                                                                                     False,
                                                                                     False,
                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                      Str, " Contatti.Sa_Cod desc, Contatti.Rag_Soc, cognome, nome ASC  ", objParametri_Server)


            objRapp_Contabili = Nothing

            'objDTableManodopera.Columns.Add("Col_0", System.Type.GetType("System.String"))  'rapporto contabile
            'objDTableManodopera.Columns.Add("Col_2", System.Type.GetType("System.String"))  'Udm_Des
            'objDTableManodopera.Columns.Add("Col_3", System.Type.GetType("System.Int32")) 'Pro_Cod
            'objDTableManodopera.Columns.Add("Col_4", System.Type.GetType("System.Decimal"))  'Qta_Ril 
            'objDTableManodopera.Columns.Add("Col_6", System.Type.GetType("System.Int32")) 'Elem_Cod
            'objDTableManodopera.Columns.Add("Col_7", System.Type.GetType("System.Int32")) 'Ditta_Cod

            For i = 0 To Dt_Manodopera.Rows.Count - 1

                Dr = objDTableManodopera.NewRow()

                Dr.Item("Col_0") = CStr(Dt_Manodopera.Rows(i).Item("Rapporto_Des"))

                'Se c'è aggiungo anche il progressivo
                If Not IsDBNull(Dt_Manodopera.Rows(i).Item("Settore_Des")) AndAlso CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")).Trim() <> "" Then
                    Dr.Item("Col_0") &= " (Progressivo: " + CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")).Trim() + ")"
                End If

                Dr.Item("Col_1") = Dt_Manodopera.Rows(i).Item("Col_1")

                If Not IsDBNull(Dt_Manodopera.Rows(i).Item("Qualifica_Des")) AndAlso CStr(Dt_Manodopera.Rows(i).Item("Qualifica_Des")).Trim() <> "" Then
                    Dr.Item("Col_1") &= " (Qualifica: " + CStr(Dt_Manodopera.Rows(i).Item("Qualifica_Des")).Trim() + ")"
                End If

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

                Dr.Item("Col_10") = Dt_Manodopera.Rows(i).Item("Qualifica_Cod")
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

    Private Sub Popola_Terzisti_OLD()


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

            If Txt_FiltroMaterialiAvanzati.Text <> "" Then
                Str &= " AND CASE WHEN (Contatti.Rag_Soc is null or Contatti.Rag_Soc = '') then Contatti.Cognome  + ' ' +  Contatti.Nome  else Contatti.Rag_Soc End  LIKE '%" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(Txt_FiltroMaterialiAvanzati.Text) & "%'"
            End If

            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessori(objParametriAgenda.Piva,
                                                                                     False,
                                                                                     False,
                                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                                      Str, " Contatti.Sa_Cod desc, Contatti.Rag_Soc, cognome, nome ASC  ", objParametri_Server)

            objRapp_Contabili = Nothing



            For i = 0 To Dt_Manodopera.Rows.Count - 1

                Dr = objDTableManodopera.NewRow()

                Dr.Item("Col_0") = CStr(Dt_Manodopera.Rows(i).Item("Rapporto_Des")) +
                                  "  (Progressivo: " + CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")) + ")"

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

                objDTableManodopera.Rows.Add(Dr)

            Next

            Dt_Manodopera = Nothing

            Session("Dt_Terzisti") = objDTableManodopera


        Catch ex As Exception

            Session("Dt_Terzisti") = Nothing

            'MsgBox("Impossibile caricare i centri di costo. " + ex.Message, MsgBoxStyle.Exclamation, "GiasOnline")

        End Try


    End Sub

    Private Sub Popola_Terzisti()


        Dim i, j As Integer

        Dim objDTableManodopera As DataTable
        Dim Dr As DataRow

        Try

            objDTableManodopera = Costruisci_Dt_Materiali()

            'Preparo la query per recuperare tutti i dipendenti(manodopera)
            'Sa_Cod=-1 : manodopera di un altro centro a disposizione di tutti

            'Risorse_Umane.Settore_Des as Col_2

            Dim objRapp_Contabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
            Dim Dt_Manodopera As DataTable
            Dim Dt_Manodopera_T As DataTable
            Dim Dt_Macchine As DataTable

            Dim Str As String = ""
            Str = " ( (Rapporti_Contabili.Cod_Rapporto = -5)  or Rapporti_Contabili.Terzista=1 ) "

            If Txt_FiltroMaterialiAvanzati.Text <> "" Then
                Str &= " AND CASE WHEN (Contatti.Rag_Soc is null or Contatti.Rag_Soc = '') then Contatti.Cognome  + ' ' +  Contatti.Nome  else Contatti.Rag_Soc End  LIKE '%" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(Txt_FiltroMaterialiAvanzati.Text) & "%'"
            End If

            'Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessori(objParametriAgenda.Piva, _
            '                                                                         False, _
            '                                                                         False, _
            '                                                                          AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
            '                                                                          Str, " Contatti.Sa_Cod desc, Contatti.Rag_Soc, cognome, nome ASC  ", objParametri_Server)

            Dt_Manodopera = objRapp_Contabili.RapportiContabilixCostiAccessori_ImpreseGias(objParametriAgenda.Piva,
                                                                         False,
                                                                         False,
                                                                         AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                          Str, " Contatti.Sa_Cod desc, Contatti.Rag_Soc, cognome, nome ASC  ", objParametri_Server)

            'objRapp_Contabili = Nothing

            'per i terzisti imprese gias
            'carico anche contatti dipendenti-legali
            Dim Pive_Terzisti As String = ""

            For i = 0 To Dt_Manodopera.Rows.Count - 1
                If Dt_Manodopera.Rows(i).Item("rag_soc") <> "" Then
                    Pive_Terzisti &= "'" & Dt_Manodopera.Rows(i).Item("cod_contatto") & "',"
                End If
            Next

            If Pive_Terzisti <> "" Then

                Pive_Terzisti = Left(Pive_Terzisti, Pive_Terzisti.Length - 1)

                'MANODOPERA TERZISTA
                Dim StrTM = " (( Rapporti_Contabili.Cod_Rapporto in(-1,-4,-6) ) or Rapporti_Contabili.Dipendente=1 or Rapporti_Contabili.Legale=1) "

                'If Txt_FiltroMaterialiAvanzati.Text <> "" Then
                '    StrTM &= " AND CASE WHEN (Contatti.Rag_Soc is null or Contatti.Rag_Soc = '') then Contatti.Cognome  + ' ' +  Contatti.Nome  else Contatti.Rag_Soc End  LIKE '%" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(Txt_FiltroMaterialiAvanzati.Text) & "%'"
                'End If

                Dt_Manodopera_T = objRapp_Contabili.RapportiContabilixCostiAccessori_Terzisti(Pive_Terzisti,
                                                                                         False,
                                                                                         False,
                                                                                         StrTM, " Contatti.Sa_Cod desc, Contatti.Rag_Soc, cognome, nome ASC  ", objParametri_Server)



                'MACCHINE TERZISTA
                Dim StrM As String = ""
                'If Txt_FiltroMaterialiAvanzati.Text <> "" Then
                '    StrM = " PM.Mac_Des LIKE '%" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(Txt_FiltroMaterialiAvanzati.Text) & "%'"
                'End If

                Dim objPM As New AgronicaCoreContabDAL.Parco_Macchine_R

                Dt_Macchine = objPM.LeggiMacchine_xCostiAccessori_Terzisti(Pive_Terzisti,
                                                                               objParametriAgenda.Data,
                                                                               StrM, "", objParametri_Server)
                objPM = Nothing

            End If




            For i = 0 To Dt_Manodopera.Rows.Count - 1

                Dr = objDTableManodopera.NewRow()


                Dr.Item("Col_0") = CStr(Dt_Manodopera.Rows(i).Item("Rapporto_Des"))

                'Se c'è aggiungo anche il progressivo
                If CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")) <> "" Then
                    Dr.Item("Col_0") &= "  (Progressivo: " + CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")) + ")"
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

                objDTableManodopera.Rows.Add(Dr)

                If Not Dt_Manodopera_T Is Nothing AndAlso Dt_Manodopera_T.Rows.Count > 0 AndAlso Dt_Manodopera.Rows(i).Item("rag_soc") <> "" Then

                    Dim DrT() As DataRow
                    DrT = Dt_Manodopera_T.Select("piva='" & Dt_Manodopera.Rows(i).Item("cod_contatto") & "'")

                    If Not DrT Is Nothing AndAlso DrT.Length > 0 Then
                        For j = 0 To DrT.Length - 1
                            Dr = objDTableManodopera.NewRow()
                            Dr.Item("Col_0") = "Terzista - Contatto - " & CStr(DrT(j).Item("Rapporto_Des")) +
                                              "  (Progressivo: " + CStr(DrT(j).Item("Settore_Des")) + ")"
                            Dr.Item("Col_1") = DrT(j).Item("Col_1")
                            Dr.Item("Col_8") = DrT(j).Item("Col_8")
                            Dr.Item("Col_9") = DrT(j).Item("Col_9")
                            Dr.Item("Col_5") = DrT(j).Item("Col_5")
                            If Dr.Item("Col_5") = 2 Then
                                Dr.Item("Col_2") = "ora"
                            Else
                                Dr.Item("Col_2") = "ha"
                            End If
                            Dr.Item("Col_3") = 0
                            Dr.Item("Col_4") = 0
                            Dr.Item("Col_6") = 0
                            Dr.Item("Col_7") = 0
                            objDTableManodopera.Rows.Add(Dr)
                        Next
                    End If

                End If

                If Not Dt_Macchine Is Nothing AndAlso Dt_Macchine.Rows.Count > 0 AndAlso Dt_Manodopera.Rows(i).Item("rag_soc") <> "" Then

                    Dim DrM() As DataRow
                    DrM = Dt_Macchine.Select("piva='" & Dt_Manodopera.Rows(i).Item("cod_contatto") & "'")

                    If Not DrM Is Nothing AndAlso DrM.Length > 0 Then

                        For j = 0 To DrM.Length - 1

                            Dr = objDTableManodopera.NewRow()

                            Dr.Item("Col_0") = "Terzista - Macchina - " & DrM(j).Item("Col_0")
                            Dr.Item("Col_1") = DrM(j).Item("Col_1") & " " & Dt_Manodopera.Rows(i).Item("Col_1")
                            Dr.Item("Col_8") = DrM(j).Item("Col_8")

                            Dim Mezzo As Integer = 0
                            Dim Prezzo_Unitario As Decimal
                            Dim Udm_Cod As Integer = 0

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

                            objDTableManodopera.Rows.Add(Dr)

                        Next

                    End If

                End If


            Next

            Dt_Manodopera = Nothing

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
            Dim Str As String = " (Rapporti_Contabili.Cod_Rapporto = -12) "

            If Txt_FiltroMaterialiAvanzati.Text <> "" Then
                Str &= " AND CASE WHEN (Contatti.Rag_Soc is null or Contatti.Rag_Soc = '') then Contatti.Cognome  + ' ' +  Contatti.Nome  else Contatti.Rag_Soc End  LIKE '%" & AgronicaCoreDataProvider.UtilityProvider.Agro_SQL_SaveText(Txt_FiltroMaterialiAvanzati.Text) & "%'"
            End If

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
                If CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")) <> "" Then
                    Dr.Item("Col_0") &= "  (Progressivo: " + CStr(Dt_Manodopera.Rows(i).Item("Settore_Des")) + ")"
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


        script.AppendLine(" setTimeout(function () { BloccaSbloccaTotale(); }, 100);  ")

        ' script.AppendLine("     BloccaSbloccaTotale();")

        script.AppendLine("     AbilitaDisabilita_SupTrattata();")

        script.AppendLine("     $('#chkSelezionaTuttiImpianti').click(function (){ ")
        script.AppendLine("         SelezionaDeselezionaTutti();")
        script.AppendLine("     });")

        script.AppendLine("     $('.ChkSelezionaImpianto').click(function (){ ")
        script.AppendLine("         ChkSelezionaImpianto_Click($(this).find('input'),false);")
        script.AppendLine("     });")


        Select Case objParametriAgenda.Lav_Cod

            Case Is <> LAVCOD_TRATTAMENTO_POST_RACCOLTA

                If SupTrattata = True Then
                    script.AppendLine("     RicalcolaSuperficieCoinvolta(); ")

                    script.AppendLine("     $('.Sup_Coinvolta').keyup(function (){")
                    script.AppendLine("         Sup_Coinvolta_Keyup($(this));")
                    script.AppendLine("     });")

                    script.AppendLine("     $('.SommaSuperficieTrattata').keyup(function () {")
                    script.AppendLine("         SommaSuperficieTrattata_Keyup(); ")
                    script.AppendLine("     });")

                    script.AppendLine("     $('.AcquaHa').keyup(function () {")
                    script.AppendLine("         AcquaHA_Keyup();")
                    script.AppendLine("     });")

                    script.AppendLine("     $('.AcquaTot').keyup( function () {")
                    script.AppendLine("         AcquaTot_Keyup();")
                    script.AppendLine("     });")
                End If

        End Select



        script.AppendLine("     $('#dialogImpostazioniColonne').dialog({ ")
        script.AppendLine("             autoOpen: false,")
        script.AppendLine("             modal: true,")
        script.AppendLine("             buttons: {")
        script.AppendLine("                 'Aggiungi': function () {")
        script.AppendLine("                 $(this).dialog('close');")
        script.AppendLine("                 SalvaImpostazioniColonne();")
        script.AppendLine("             },")
        script.AppendLine("             'Annulla': function () {")
        script.AppendLine("                 $(this).dialog('close');")
        script.AppendLine("                 return false;")
        script.AppendLine("             }")
        script.AppendLine("         }")
        script.AppendLine("     });")


        script.AppendLine("     $('#btn_Impostazioni_Colonne').click(function () {")
        'script.AppendLine("         alert('');")
        'script.AppendLine("         if ($('.rigaImpianti').length == 0) {")
        script.AppendLine("             $('#dialogImpostazioniColonne').dialog('open');")
        script.AppendLine("             $('#dialogImpostazioniColonne').parent().appendTo($('form:first')); ")
        'script.AppendLine("         }")
        script.AppendLine("     });")



        script.AppendLine("}); ")


        ScriptManager.RegisterStartupScript(UpdatePanelImpianti, UpdatePanelImpianti.GetType(),
                                         String.Format("jQuery_{0}", UpdatePanelImpianti.ClientID), script.ToString, True)





    End Sub

    Private Sub ScriptCostiAccessoriEtAl()

        Dim Script As New StringBuilder


        Script.Length = 0

        Script.AppendLine("$(document).ready(function () { ")


        If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Planning Then

            'blocco delle combo.
            Script.AppendLine(" BloccaCombo_CentroAziendale(); ")
            Script.AppendLine(" BloccaCombo_Magazzini(); ")

        End If

        'script.AppendLine("     $('#btn_costi_accessori').click(function () { ")
        'script.AppendLine("         $('#" & AggiornaCostiAccessori.ClientID & "').click();")
        'script.AppendLine("         $('#dialogCostiAccessori').dialog('open');")
        'script.AppendLine("         $('#dialogCostiAccessori').parent().appendTo($('form:first')); ")
        'script.AppendLine("     });")

        Script.AppendLine("     $('#btn_costi_accessori_avanzati').click(function () { ")
        Script.AppendLine("         $('#" & AggiornaCostiAccessoriAvanzati.ClientID & "').click();")
        Script.AppendLine("         $('#dialogCostiAccessoriAvanzati').dialog('open');")
        Script.AppendLine("         $('#dialogCostiAccessoriAvanzati').parent().appendTo($('form:first')); ")
        Script.AppendLine("     });")

        Script.AppendLine("     $('#apri_pannello_ricette').click(function () { ")
        Script.AppendLine("         $('#dialogRicette').dialog('open');")
        'script.AppendLine("         $('#btn_carica_dialog').show();")
        'script.AppendLine("         $('#btn_conferma_dialog').hide();")
        Script.AppendLine("         $('#dialogRicette').parent().appendTo($('form:first')); ")
        Script.AppendLine("     });")

        Script.AppendLine("}); ")

        ScriptManager.RegisterStartupScript(UpdatePanelToolBar, UpdatePanelToolBar.GetType(),
                                        String.Format("jQuery_{0}", UpdatePanelToolBar.ClientID), Script.ToString, True)
    End Sub

    Protected Sub GridView_Planning_Sorting(ByVal sender As Object, ByVal e As GridViewSortEventArgs)

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
            SortExpDirection += " " + Session("SortDirection")
            dv.Sort = SortExpDirection
            GridView_Impianti.DataSource = dv.ToTable()
            ViewState("DT_Impianti") = dv.ToTable()

            GridView_Impianti.DataBind()

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
        CType(Page.Master, Operazione).CaricaGriglia_Impianti()
    End Sub


#Region "Costi Accessori Avanzati"

    '##########################################################################################
    Private Sub Crea_Griglia_CentriCostoAvanzati()

        'Aggiungo al datagrid dei centri di costo i magazzini (nella query c'è già il filtro sul tipo di fabbricato)
        'Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        'Dim objDTableCentriCosto As DataTable = objFabbricati.LeggixCostiAccessori(objParametriAgenda.Piva, 0, _
        '                                                          objParametriAgenda.Data, _
        '                                                          AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
        '                                                          "", "", objParametri_Server)

        '' "1=2", "", objParametri_Server)

        'objFabbricati = Nothing

        Dim objDTableCentriCosto As New DataTable
        objDTableCentriCosto.Columns.Add("Fabbricato_Des")
        objDTableCentriCosto.Columns.Add("Fabbricato_Cod")
        objDTableCentriCosto.Columns.Add("Sa_Cod")

        Dim Dr As DataRow

        'Select Case ALGORITMO_COSTI_ACCESSORI
        '    Case enum_AlgoritmoCostiAccessori.CAB

        '    Case enum_AlgoritmoCostiAccessori.SBTF
        '        'con questo algoritmo, i terzisti non si vedono...

        '        'Aggiungo al datagrid la riga degli articoli fuori da magazzino
        '        Dr = objDTableCentriCosto.NewRow
        '        Dr.Item(0) = "Servizi Professionali non a Magazzino"
        '        Dr.Item(1) = -5 'Fabbricato_Cod = -5 corrisponde agli Articoli non a magazzino
        '        Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
        '        objDTableCentriCosto.Rows.Add(Dr)

        'End Select

        'Aggiungo al datagrid la riga del Servizi Professionali
        Dr = objDTableCentriCosto.NewRow
        Dr.Item(0) = "Servizi Professionali"
        Dr.Item(1) = -7 'Fabbricato_Cod = -7 corrisponde al Servizi Professionali
        Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
        objDTableCentriCosto.Rows.Add(Dr)

        'Aggiungo al datagrid la riga dei Carburanti
        Dr = objDTableCentriCosto.NewRow
        Dr.Item(0) = "Carburanti"
        Dr.Item(1) = -6 'Fabbricato_Cod = -6 corrisponde al Carburanti
        Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
        objDTableCentriCosto.Rows.Add(Dr)

        'Aggiungo al datagrid la riga del Ricambi
        Dr = objDTableCentriCosto.NewRow
        Dr.Item(0) = "Ricambi"
        Dr.Item(1) = -9 'Fabbricato_Cod = -9 corrisponde a Ricambi
        Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
        objDTableCentriCosto.Rows.Add(Dr)

        'Aggiungo al datagrid la riga del Altre Risorse
        Dr = objDTableCentriCosto.NewRow
        Dr.Item(0) = "Altre Risorse"
        Dr.Item(1) = -8 'Fabbricato_Cod = -8 corrisponde al Altre Risorse
        Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
        objDTableCentriCosto.Rows.Add(Dr)


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

        'Select Case ALGORITMO_COSTI_ACCESSORI
        'Case enum_AlgoritmoCostiAccessori.CAB
        'Aggiungo al datagrid la riga del terzisti
        Dr = objDTableCentriCosto.NewRow
        Dr.Item(0) = Resources.AgronicaAgenda_2010.CTerzisti
        Dr.Item(1) = -3 'Fabbricato_Cod = -3 corrisponde ai C/Terzisti
        Dr.Item(2) = 0  'Sa_Cod=0, è valorizzato solo per i fabbricati
        objDTableCentriCosto.Rows.Add(Dr)

        'Case enum_AlgoritmoCostiAccessori.SBTF

        'End Select

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

        For i = 0 To dgrCentriCostoAvanzati.Rows.Count - 1
            Dim tipo As String = objDTableCentriCosto.Rows(i).Item(0)
            Select Case tipo
                Case Resources.AgronicaAgenda_2010.ParcoMacchine
                    dgrCentriCostoAvanzati.Rows(i).Cells(1).Controls(0).Visible = False
                Case Resources.AgronicaAgenda_2010.Manodopera

                Case Resources.AgronicaAgenda_2010.CTerzisti

                Case Resources.AgronicaAgenda_2010.TecnicoResponsabile

                Case Else
                    dgrCentriCostoAvanzati.Rows(i).Cells(1).Controls(0).Visible = False
            End Select
        Next

    End Sub

    '##########################################################################################
    Private Sub CreaComboMagazzini(Fabbricato_Cod As Integer)
        'in realtà il fabbricato cod può essere solo un numero negativo che rappresenta la tipologia di costo

        'Estraggo l'elem_cod di riferimento
        Dim elem_cod As Integer
        Select Case Fabbricato_Cod
            Case -6 'Carburanti
                elem_cod = 2
            Case -7 ' Servizi Professionali
                elem_cod = 700
            Case -8 ' Altre Risorse
                elem_cod = 200
            Case -9 ' Ricambi
                elem_cod = 401
            Case Else
                Exit Sub
        End Select

        'pulisco la combo
        CmbCentriDiCostoAvanzati.Items.Clear()

        'aggiungo il caso del Nessuna gestione magazzini
        CmbCentriDiCostoAvanzati.Items.Add(New ListItem("Nessuna gestione dei Magazzini", Fabbricato_Cod & "§" & 0))

        'Se sono nel reale (non nel planning) mostro anche i verim agazzini
        If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then

            'cerco tutti i amgazzini con l'elem cod in pancia
            Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
            Dim objDTableCentriCosto As DataTable = objFabbricati.LeggiFabbricatiConElemCodMovimentati(
                                                                      objParametriAgenda.Piva, 0, elem_cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                      "", "", objParametri_Server)

            objFabbricati = Nothing

            'Popolo la combo con i magazzini
            For Each r As DataRow In objDTableCentriCosto.Rows
                CmbCentriDiCostoAvanzati.Items.Add(New ListItem(r(0), r(1) & "§" & r(2)))
                'Elementi: Fabbricato_Cod§Sa_Cod
            Next

            'Se c'è almento un magazzino, preimposto il primo, altrimenti metto "nessun magazzino"
            CmbCentriDiCostoAvanzati.SelectedIndex = If(objDTableCentriCosto.Rows.Count > 0, 1, 0)

        End If

        'Faccio scattare l'evento per caricare la gridview
        CmbCentriDiCostoAvanzati_SelectedIndexChanged(Nothing, Nothing)

        'MARCO G: Imposto la combobox con il filtro di ricerca
        impostaPluginCombo()

    End Sub


    'click sul bottone dei costi accessori
    Protected Sub AggiornaCostiAccessoriAvanzati_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AggiornaCostiAccessoriAvanzati.Click

        ' CalcolaCosti.Checked = False

        tabellaCostiAccessoriAvanzati.Visible = True

        'caricaSuSessionDTControlliDgrScaricoAvanzati()

        'Filtro_MaterialiAvanzati.Visible = False

        Crea_Griglia_CentriCostoAvanzati()

        AggiornaDgrScaricoAvanzati()
        Session("dtScarico_old") = Session("dtScarico")

        AggiornaMagazzino()

    End Sub

    Private Sub AggiornaDgrScaricoAvanzati()
        If Not IsNothing(Session("dtScarico")) Then

            'Vettore di DataColumn
            Dim ScaricoKeys(5) As String

            'Valorizzo le celle del vettore
            ScaricoKeys(0) = "Udm_Selezionata"
            ScaricoKeys(1) = "ID_Attivita"
            ScaricoKeys(2) = "Turno_Cod"
            ScaricoKeys(3) = "Qta_Ril"

            ScaricoKeys(4) = "Qualifica_Cod"
            ScaricoKeys(5) = "Tariffa_Cod"

            dgrScaricoAvanzati.DataSource = Session("dtScarico")
            dgrScaricoAvanzati.DataKeyNames = ScaricoKeys
            dgrScaricoAvanzati.DataBind()

            '  Marco Grilli, 26/08/2016 12:25:29: Nascondo le colonne che non riguardano l'algoritmo
            Select Case ALGORITMO_COSTI_ACCESSORI
                Case enum_AlgoritmoCostiAccessori.CAB

                    'dgrScaricoAvanzati.Columns(dgrScaricoAvanzati.Columns.IndexOf(dgrScaricoAvanzati.Columns(""))).Visible = False

                    For i As Integer = 0 To dgrScaricoAvanzati.Columns.Count - 1
                        If dgrScaricoAvanzati.Columns(i).HeaderText.ToLower = "qualifica" OrElse
                                dgrScaricoAvanzati.Columns(i).HeaderText.ToLower = "tariffa" Then

                            dgrScaricoAvanzati.Columns(i).Visible = False
                        End If
                    Next

                Case enum_AlgoritmoCostiAccessori.SBTF

                    For i As Integer = 0 To dgrScaricoAvanzati.Columns.Count - 1

                        If dgrScaricoAvanzati.Columns(i).HeaderText.ToLower = "turno" Then
                            dgrScaricoAvanzati.Columns(i).Visible = False
                        End If

                    Next
            End Select

            'dopo aver fatto il bind verifico come impostare la dropdown
            ControllaDDLCostiAccessori(dgrScaricoAvanzati)

            'dopo aver fatto il bind verifico come impostare la txt_QtaRil


        End If
    End Sub

    Private Sub SalvaCostiAccessoriAvanzati_Click(sender As Object, e As System.EventArgs) Handles SalvaCostiAccessoriAvanzati.Click

        SalvaCostiAccessori_SuAgendaMovimenti(True, dgrScaricoAvanzati)

    End Sub


    Private Sub AnnullaCostiAccessoriAvanzati_Click(sender As Object, e As System.EventArgs) Handles AnnullaCostiAccessoriAvanzati.Click

        Session("dtScarico") = Session("dtScarico_old")
        AggiornaGridViewCostiAccessoriVisibili()

    End Sub

    'Private Sub BTN_FiltraMateriali_Click(sender As Object, e As System.EventArgs) Handles BTN_FiltraMateriali.Click

    '    Dim DT_CentriCosto As DataTable = Session("DT_CentriCosto")
    '    Dim Dt As DataTable

    '    'Cambio l'intestazione delle colonne nel datagrid
    '    Me.dgrMateriali.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.CategoriaProdotto
    '    Me.dgrMateriali.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Prodotto
    '    Me.dgrMateriali.Columns(3).HeaderText = Resources.AgronicaAgenda_2010.Giacenza

    '    'If Not IsNothing(Session("Dt_Prodotti")) Then
    '    '    Dt.Clear()
    '    'End If

    '    'If Not IsNothing(Session("Dt_Prodotti")) Then
    '    '    Dt = Session("Dt_Prodotti")
    '    'Else
    '    Popola_ProdottiMagazzino(dgrCentriCosto.DataKeys(dgrCentriCosto.SelectedIndex).Item("Sa_Cod"),
    '                             dgrCentriCosto.DataKeys(dgrCentriCosto.SelectedIndex).Item("Fabbricato_Cod"), _
    '                             objParametriAgenda.Elem_Cod, _
    '                             False)

    '    Dt = Session("Dt_Prodotti")
    '    ' End If

    '    Me.dgrMateriali.DataSource = Dt
    '    Me.dgrMateriali.DataBind()

    '    ControllaDDLCostiAccessori(dgrScarico)


    'End Sub


    Private Sub BTN_FiltraMaterialiAvanzati_Click(sender As Object, e As System.EventArgs) Handles BTN_FiltraMaterialiAvanzati.Click

        Dim Dt As DataTable
        Dim Fabbricato_Cod, Sa_Cod As Integer

        If ViewState("ProvenienzaCentroDiCosto") = "CMB" Then
            Fabbricato_Cod = CmbCentriDiCostoAvanzati.SelectedValue.Split("§")(0)
            Sa_Cod = CmbCentriDiCostoAvanzati.SelectedValue.Split("§")(1)
        ElseIf ViewState("ProvenienzaCentroDiCosto") = "DGR" Then
            Fabbricato_Cod = CInt(dgrCentriCostoAvanzati.DataKeys(dgrCentriCostoAvanzati.SelectedIndex).Item("Fabbricato_Cod"))
            Sa_Cod = CInt(dgrCentriCostoAvanzati.DataKeys(dgrCentriCostoAvanzati.SelectedIndex).Item("Sa_Cod"))
        Else
            Exit Sub
        End If

        Select Case Fabbricato_Cod

            Case Is > 0

                Dim elem_cod As Integer
                Select Case CInt(dgrCentriCostoAvanzati.DataKeys(dgrCentriCostoAvanzati.SelectedIndex).Item("Fabbricato_Cod"))
                    Case -6 'Carburanti
                        elem_cod = 2
                    Case -7 ' Servizi Professionali
                        elem_cod = 700
                    Case -8 ' Altre Risorse
                        elem_cod = 200
                    Case -9 ' Ricambi
                        elem_cod = 401
                End Select

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.CategoriaProdotto
                Me.dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Prodotto
                Me.dgrMaterialiAvanzati.Columns(3).Visible = True
                Me.dgrMaterialiAvanzati.Columns(3).HeaderText = "Lotto"
                Me.dgrMaterialiAvanzati.Columns(4).HeaderText = Resources.AgronicaAgenda_2010.Giacenza

                Popola_ProdottiMagazzino(Sa_Cod, Fabbricato_Cod, elem_cod, True)
                Dt = Session("Dt_Prodotti")

            Case -1 'macchina

                'Cambio l'intestazione delle colonne nel datagrid
                dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.MacchinaAttrezzatura
                dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Descrizione
                Me.dgrMaterialiAvanzati.Columns(3).Visible = False
                Me.dgrMaterialiAvanzati.Columns(4).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

                Popola_ParcoMacchine()
                Dt = Session("Dt_Macchine")


            Case -2 'manodopera

                Me.dgrMaterialiAvanzati.Columns(3).Visible = False

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.RapportoContabile
                Me.dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Manodopera
                Me.dgrMaterialiAvanzati.Columns(3).Visible = False
                Me.dgrMaterialiAvanzati.Columns(4).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

                Popola_Manodopera()
                Dt = Session("Dt_Manodopera")


            Case -3 'terzista

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.RapportoContabile
                Me.dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Terzista
                Me.dgrMaterialiAvanzati.Columns(3).Visible = False
                Me.dgrMaterialiAvanzati.Columns(4).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

                Popola_Terzisti()
                Dt = Session("Dt_Terzisti")


            Case -4 'tecnico

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.RapportoContabile
                Me.dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.TecnicoResponsabile
                Me.dgrMaterialiAvanzati.Columns(3).Visible = False
                Me.dgrMaterialiAvanzati.Columns(4).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

                Popola_TecnicoResponsabile()
                Dt = Session("Dt_TecnicoResponsabile")

            Case -6, -7, -8, -9

                Dim elem_cod As Integer
                Select Case Fabbricato_Cod
                    Case -6 'Carburanti
                        elem_cod = 2
                    Case -7 ' Servizi Professionali
                        elem_cod = 700
                    Case -8 ' Altre Risorse
                        elem_cod = 200
                    Case -9 ' Ricambi
                        elem_cod = 401
                End Select

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.CategoriaProdotto
                Me.dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Prodotto
                Me.dgrMaterialiAvanzati.Columns(3).HeaderText = "Codice Articolo"

                Popola_TuttiProdotti(elem_cod)
                Dt = Session("Dt_TuttiProdotti")

        End Select


        Me.dgrMaterialiAvanzati.DataSource = Dt
        Me.dgrMaterialiAvanzati.DataBind()

        ControllaDDLCostiAccessori(dgrScaricoAvanzati)


    End Sub

    'Private Sub dgrCentriCosto_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles dgrCentriCosto.RowCommand

    '    Dim IndiceRigaGriglia As Integer = Convert.ToInt32(e.CommandArgument)

    '    Session("IndiceRigaGrigliaCliccato") = IndiceRigaGriglia

    '    Select Case e.CommandName

    '        Case "NuovoElemento"
    '            Dim tipo As String = CType(dgrCentriCosto.Rows(IndiceRigaGriglia).Cells(0).Controls(0), LinkButton).Text
    '            Select Case tipo
    '                Case Resources.AgronicaAgenda_2010.ParcoMacchine
    '                    ApriMacchinaCosti(objParametriAgenda.Piva)
    '                Case Resources.AgronicaAgenda_2010.Manodopera
    '                    ApriContattiCosti(objParametriAgenda.Piva, COD_LEGALE, UpdateCostiAccessori)
    '                Case Resources.AgronicaAgenda_2010.CTerzisti
    '                    ApriContattiCosti(objParametriAgenda.Piva, COD_TERZISTA, UpdateCostiAccessori)
    '                Case Resources.AgronicaAgenda_2010.TecnicoResponsabile
    '                    ApriContattiCosti(objParametriAgenda.Piva, COD_TECNICORESPONSABILE, UpdateCostiAccessori)
    '            End Select


    '    End Select

    'End Sub

    Private Sub dgrCentriCostoAvanzati_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles dgrCentriCostoAvanzati.RowCommand

        Dim IndiceRigaGriglia As Integer = Convert.ToInt32(e.CommandArgument)

        Session("IndiceRigaGrigliaCliccato") = IndiceRigaGriglia

        Select Case e.CommandName

            Case "NuovoElemento"
                Dim tipo As String = CType(dgrCentriCostoAvanzati.Rows(IndiceRigaGriglia).Cells(0).Controls(0), LinkButton).Text
                Select Case tipo
                    Case Resources.AgronicaAgenda_2010.ParcoMacchine
                        'ApriMacchinaCosti(objParametriAgenda.Piva)
                    Case Resources.AgronicaAgenda_2010.Manodopera
                        ApriContattiCosti(objParametriAgenda.Piva, COD_LEGALE, UpdateCostiAccessoriAvanzati)
                    Case Resources.AgronicaAgenda_2010.CTerzisti
                        ApriContattiCosti(objParametriAgenda.Piva, COD_TERZISTA, UpdateCostiAccessoriAvanzati)
                    Case Resources.AgronicaAgenda_2010.TecnicoResponsabile
                        ApriContattiCosti(objParametriAgenda.Piva, COD_TECNICORESPONSABILE, UpdateCostiAccessoriAvanzati)
                End Select


        End Select



    End Sub


    Private Sub dgrCentriCostoAvanzati_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles dgrCentriCostoAvanzati.SelectedIndexChanged

        Dim Fabbricato_Cod As Integer = CInt(dgrCentriCostoAvanzati.DataKeys(dgrCentriCostoAvanzati.SelectedIndex).Item("Fabbricato_Cod"))
        Select Case Fabbricato_Cod

            Case -1, -2, -3, -4
                'Case Is > 0, -1, -2, -3, -4, -5
                CmbCentriDiCostoAvanzati.Items.Clear()
                ViewState("ProvenienzaCentroDiCosto") = "DGR"
                AggiornaListaCostiAccessoriAvanzati()

            Case Is < -5
                CreaComboMagazzini(Fabbricato_Cod)

        End Select

        'Coloro la riga
        dgrCentriCostoAvanzati.SelectedRow.BackColor = Drawing.Color.Gold

        'MARCO G: Imposto la combobox con il filtro di ricerca
        impostaPluginCombo()

    End Sub


    Private Sub dgrMaterialiAvanzati_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles dgrMaterialiAvanzati.RowCommand

        caricaSuSessionDTControlliDgrScaricoAvanzati()
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
        Dim Qualifica_Cod As Integer = 0
        Dim Cod_Rapporto As Integer = 0
        Dim Lotto As String = ""
        Dim codice As String = ""

        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Dim dtScarico As DataTable

        dtScarico = Session("dtScarico")

        Select Case e.CommandName

            Case "AggiungiCosto"

                Dim provenienza As String = ViewState("ProvenienzaCentroDiCosto")

                'If Not Session("DT_CentriCosto") Is Nothing Then
                If (provenienza = "CMB" AndAlso Not IsNothing(CmbCentriDiCostoAvanzati.SelectedItem)) _
                    OrElse (provenienza = "DGR" AndAlso Not IsNothing(Session("DT_CentriCosto"))) Then

                    Dim Fabbricato_Cod_CentroDiCosto, Sa_Cod_CentroDiCosto As Integer
                    If provenienza = "CMB" Then
                        Fabbricato_Cod_CentroDiCosto = CmbCentriDiCostoAvanzati.SelectedValue.Split("§")(0)
                        Sa_Cod_CentroDiCosto = CmbCentriDiCostoAvanzati.SelectedValue.Split("§")(1)

                    ElseIf provenienza = "DGR" Then
                        Dim DT_CentriCosto As DataTable = Session("DT_CentriCosto")
                        Fabbricato_Cod_CentroDiCosto = CInt(DT_CentriCosto.Rows(dgrCentriCostoAvanzati.SelectedIndex).Item("Fabbricato_Cod"))
                        Sa_Cod_CentroDiCosto = CInt(DT_CentriCosto.Rows(dgrCentriCostoAvanzati.SelectedIndex).Item("Sa_Cod"))
                    Else
                        Exit Sub
                    End If

                    Select Case Fabbricato_Cod_CentroDiCosto

                        Case Is > 0

                            If Not IsNothing(Session("Dt_Prodotti")) Then

                                Dim Dt_Prodotti As DataTable = Session("Dt_Prodotti")

                                If Dt_Prodotti.Rows.Count >= IndiceRigaGriglia Then

                                    Dim objFabb As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                                    Centro = objFabb.FabbricatoDes_from_FabbricatoCod(
                                                objParametriAgenda.Piva,
                                                Sa_Cod_CentroDiCosto,
                                                Fabbricato_Cod_CentroDiCosto,
                                                objParametri_Server) 'Magazzino

                                    Tipo_Centro = "M"
                                    Centro_Cod = Fabbricato_Cod_CentroDiCosto
                                    Sa_Cod = Sa_Cod_CentroDiCosto

                                    Elem_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_6")
                                    Pro_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_8")
                                    Categoria_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Udm_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_4")

                                    If Elem_Cod = CAT_MAG_SERVIZI_PROFESSIONALI Then
                                        Select Case ALGORITMO_COSTI_ACCESSORI
                                            Case enum_AlgoritmoCostiAccessori.CAB
                                                Qta_Ril = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_4")

                                            Case enum_AlgoritmoCostiAccessori.SBTF
                                                'Grilli: priempostare il dato come la somma delle superfici dell'appezzamento. NO dell'impainto
                                                Dim listaImp As List(Of Impianto) = GetImpianti()
                                                'Dim app_R As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                                                Dim imp_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                                Dim tot As Decimal = 0

                                                For Each i As Impianto In listaImp
                                                    tot += imp_R.LeggiSuperficie(i.Piva, i.Sa_Cod, i.Appezza, i.ID_Reg, objParametri_Server)
                                                    'tot += i.Sup_Imp
                                                    'tot += app_R.Superficie_from_PivaSaCodAppezza(i.Piva, i.Sa_Cod, i.Appezza, objParametri_Server)
                                                Next

                                                Qta_Ril = tot

                                        End Select
                                    End If

                                    Costo_Unitario = Format(Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_7")
                                    Riga = 0 'IndiceRigaGriglia

                                    Lotto = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_12")

                                    If Lotto <> "" Then
                                        Risorsa_Des &= " (Lotto: " & Lotto & ")"
                                    End If

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

                                    codice = Dt_Macchine.Rows(IndiceRigaGriglia).Item("Col_13")

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
                                    Riga = 0 'IndiceRigaGriglia

                                    Qualifica_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_10")
                                    Cod_Rapporto = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_11")

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
                                    Elem_Cod = 0

                                    If InStr(LCase(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")), LCase("contatto")) <> 0 Then
                                        Centro = "Manodopera"
                                        Tipo_Centro = "MD"
                                        Centro_Cod = -2
                                    End If

                                    If InStr(LCase(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")), LCase("macchina")) <> 0 Then
                                        Centro = "Parco Macchine"
                                        Tipo_Centro = "PM"
                                        Centro_Cod = -1
                                        Elem_Cod = MACCHINE
                                    End If

                                    Categoria_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Pro_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_8")

                                    Udm_Des = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Qta_Ril = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_4")
                                    Costo_Unitario = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Col_7")
                                    'Cod_Rapporto = Dt_Manodopera.Rows(IndiceRigaGriglia).Item("Cod_Rapporto")
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

                            'Case Is = -5

                            '    If Not IsNothing(Session("Dt_TuttiProdotti")) Then

                            '        Dim Dt_Prodotti As DataTable = Session("Dt_TuttiProdotti")

                            '        If Dt_Prodotti.Rows.Count >= IndiceRigaGriglia Then

                            '            Centro_Cod = -5
                            '            Centro = "Servizi Professionali non a Magazzino"
                            '            Tipo_Centro = "XM" ' Extra Magazzino
                            '            Sa_Cod = 0

                            '            Elem_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_6")
                            '            Pro_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_3")
                            '            Mat_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_8")
                            '            Categoria_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_0")
                            '            Risorsa_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_1")

                            '            Udm_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_2")
                            '            Udm_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_5")

                            '            Select Case ALGORITMO_COSTI_ACCESSORI
                            '                Case enum_AlgoritmoCostiAccessori.CAB
                            '                    Qta_Ril = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_4")

                            '                Case enum_AlgoritmoCostiAccessori.SBTF

                            '                    'Grilli: priempostare il dato come la somma delle superfici dell'appezzamento
                            '                    Dim listaImp As List(Of Impianto) = GetImpianti()
                            '                    Dim app_R As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                            '                    Dim tot As Decimal = 0

                            '                    For Each i As Impianto In listaImp

                            '                        If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
                            '                            tot += app_R.Superficie_from_PivaSaCodAppezza(i.Piva, i.Sa_Cod, i.Appezza, objParametri_Server)
                            '                        Else
                            '                            tot += i.Qta2
                            '                        End If


                            '                    Next

                            '                    Qta_Ril = tot

                            '            End Select

                            '            Costo_Unitario = Format(Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                            '            Costo = Format(Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                            '            Ditta_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_7")
                            '            Riga = 0 'IndiceRigaGriglia

                            '        End If

                            '    End If

                        Case -6, -7, -8, -9

                            Dim descrCentroDiCosto As String
                            Select Case Fabbricato_Cod_CentroDiCosto
                                Case -6 'Carburanti
                                    descrCentroDiCosto = "Carburanti non a Magazzino"
                                Case -7 ' Servizi Professionali
                                    descrCentroDiCosto = "Servizi Professionali non a Magazzino"
                                Case -8 ' Altre Risorse
                                    descrCentroDiCosto = "Altre Risorse non a Magazzino"
                                Case -9 ' Ricambi
                                    descrCentroDiCosto = "Ricambi non a Magazzino"
                                Case Else
                                    Exit Sub
                            End Select

                            If Not IsNothing(Session("Dt_TuttiProdotti")) Then

                                Dim Dt_Prodotti As DataTable = Session("Dt_TuttiProdotti")

                                If Dt_Prodotti.Rows.Count >= IndiceRigaGriglia Then

                                    Centro_Cod = Fabbricato_Cod_CentroDiCosto
                                    Centro = descrCentroDiCosto
                                    Tipo_Centro = "XM" ' Extra Magazzino
                                    Sa_Cod = 0

                                    Elem_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_6")
                                    Pro_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_3")
                                    Mat_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_8")
                                    Categoria_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_0")
                                    Risorsa_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_1")

                                    Udm_Des = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_2")
                                    Udm_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_5")

                                    Select Case ALGORITMO_COSTI_ACCESSORI
                                        Case enum_AlgoritmoCostiAccessori.CAB
                                            Qta_Ril = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_4")

                                        Case enum_AlgoritmoCostiAccessori.SBTF

                                            If Fabbricato_Cod_CentroDiCosto = -7 Then ' Servizi professionali
                                                'Grilli: priempostare il dato come la somma delle superfici dell'appezzamento. No dell'impianto
                                                Dim listaImp As List(Of Impianto) = GetImpianti()
                                                Dim imp_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                                Dim tot As Decimal = 0

                                                For Each i As Impianto In listaImp

                                                    If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
                                                        tot += imp_R.LeggiSuperficie(i.Piva, i.Sa_Cod, i.Appezza, i.ID_Reg, objParametri_Server)
                                                    Else
                                                        tot += i.Qta2
                                                    End If


                                                Next

                                                Qta_Ril = tot

                                            Else
                                                Qta_Ril = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_4")
                                            End If


                                    End Select

                                    Costo_Unitario = Format(Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_9") * 1, "0.00")
                                    Costo = Format(Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_9") * Qta_Ril, "0.00")

                                    Ditta_Cod = Dt_Prodotti.Rows(IndiceRigaGriglia).Item("Col_7")
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
                                            0, 0,
                                            0, 0,
                                            Qualifica_Cod, 0,
                                            Cod_Rapporto,
                                            dtScarico,
                                            codice,
                                            Lotto)


                End If

                'InserisciCosto(CType(dgrMateriali.Rows(IndiceRigaGriglia).FindControl("Col_4"), TextBox), Nothing)

        End Select

        Session("dtScarico") = dtScarico
        AggiornaDgrScaricoAvanzati()

        'Coloro la riga
        dgrCentriCostoAvanzati.SelectedRow.BackColor = Drawing.Color.Gold

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
            lbl_Ricette.Text = "Selezionare una cultura per caricare le ricette"
            Exit Sub
        End If

        Dim Dt_Ricette As DataTable
        Dt_Ricette = objRicette.Leggi_LavCod_Cul(objParametriAgenda.Lav_Cod,
                        objParametriAgenda.Veg_Cod.Split("/")(0),
                        objParametriAgenda.Data,
                        objParametriAgenda.Data,
                            "",
                            "",
                            HttpContext.Current.Session("ASG_objParametri_Server"))

        If Dt_Ricette.Rows.Count = 0 Then
            lbl_Ricette.Text = "Nessuna ricetta attiva su questa specie  una cultura per caricare le ricette"
            Exit Sub
        Else
            lbl_Ricette.Text = ""
        End If

        Dim i As Integer = 0
        Dim Descrizione As String = ""


        rbl_RicetteAttive.Items.Clear()
        For i = 0 To Dt_Ricette.Rows.Count - 1
            Descrizione = ""
            Select Case Dt_Ricette.Rows(i).Item("Tipo_Ricetta")
                Case enum_TipoRicetta.Standard
                    Descrizione = "Linea tecnica: " & Dt_Ricette.Rows(i).Item("Ricetta_Operazione_Des")
                Case enum_TipoRicetta.Standard_Destinazioni
                    Descrizione = "Ricetta aziendale: " & Dt_Ricette.Rows(i).Item("Ricetta_Operazione_Des")
                Case enum_TipoRicetta.PianoDistribuzioneConcimi
                    Descrizione = "Piano Distribuzione: " & Dt_Ricette.Rows(i).Item("Ricetta_Operazione_Des")
            End Select
            rbl_RicetteAttive.Items.Add(New ListItem(Descrizione, Dt_Ricette.Rows(i).Item("ricetta_cod") & "|" & Dt_Ricette.Rows(i).Item("Ricetta_Operazione_Cod") & "|" & Dt_Ricette.Rows(i).Item("Tipo_Ricetta")))
        Next

    End Sub


    Protected Sub Btn_Inserisci_Impianti_Ricetta_Click(sender As Object, e As EventArgs) Handles Btn_Inserisci_Impianti_Ricetta.Click
        Dim ricetta As String = rbl_RicetteAttive.SelectedValue
        If ricetta = "" Then
            Messaggi.AgroMsgBox("Selezionare una ricetta prima di procedere ", Page, , UpdatePanelRicette)
            Exit Sub
        End If
        ricetta_cod = Split(ricetta, "|")(0)
        Session("ricetta_cod") = ricetta_cod
        Ricetta_Operazione_Cod = Split(ricetta, "|")(1)
        Session("Ricetta_Operazione_Cod") = Ricetta_Operazione_Cod
        Ricetta_Tipo = Split(ricetta, "|")(2)
        Session("Ricetta_Tipo") = Ricetta_Tipo

        Dim impiantiPreSelezionati As Integer = GetImpianti().Count

        Dim ListaImpiantiRicetta As New List(Of Impianto)
        Inserisci_Impianti_Della_Ricetta(ListaImpiantiRicetta)
        Dim impiantiPostSelezionati As Integer = GetImpianti().Count
        If ListaImpiantiRicetta.Count = 0 Then
            If impiantiPreSelezionati = 0 Then
                Messaggi.AgroMsgBox("La ricetta non indica nessuna destinazione e non è selezionato alcun impianto nella tabella.", Page, , UpdatePanelRicette)
                chiudiDialog()
                Exit Sub
            Else
                Messaggi.AgroMsgBox("La ricetta non indica nessuna destinazione ma sono selezionati " & impiantiPreSelezionati & " impianti nella tabella.", Page, , UpdatePanelRicette)
                'nascondiVisualizzaPulsantiCaricaConferma(False)
                Exit Sub
            End If

        Else
            Messaggi.AgroMsgBox("Sono stati aggiunti " & ListaImpiantiRicetta.Count & " impianti dalla ricetta.", Page, , UpdatePanelRicette)
            'nascondiVisualizzaPulsantiCaricaConferma(False)
            Exit Sub
        End If

    End Sub


    Public Sub Inserisci_Impianti_Della_Ricetta(ByRef ListaImpiantiRicetta As List(Of Impianto))

        If ListaImpiantiRicetta.Count = 0 Then
            Dim dt As DataTable = New AgronicaCoreContabDAL.Ricette_Destinazioni_R().Leggi(CInt(ricetta_cod), CInt(Ricetta_Operazione_Cod), 0, 0, 0, "", 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            Dim imp As Impianto
            For i = 0 To dt.Rows.Count - 1
                imp = New Impianto

                If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
                    imp.Piva = dt.Rows(i).Item("Piva")
                    imp.Sa_Cod = dt.Rows(i).Item("Sa_Cod")
                    imp.Appezza = dt.Rows(i).Item("Appezza")
                    imp.ID_Reg = dt.Rows(i).Item("ID_Reg")
                Else
                    imp.Programmazione_Entita_cod = dt.Rows(i).Item("Programmazione_Entita_Cod")
                End If

                ListaImpiantiRicetta.Add(imp)
            Next
        End If

        For i = 0 To ListaImpiantiRicetta.Count - 1

            If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then


                For j = 0 To GridView_Impianti.Rows.Count - 1

                    If ListaImpiantiRicetta(i).Piva = GridView_Impianti.DataKeys(j).Item("Piva") And
                       ListaImpiantiRicetta(i).Sa_Cod = GridView_Impianti.DataKeys(j).Item("Sa_Cod") And
                       ListaImpiantiRicetta(i).Appezza = GridView_Impianti.DataKeys(j).Item("Appezza") And
                       ListaImpiantiRicetta(i).ID_Reg = GridView_Impianti.DataKeys(j).Item("Id_Reg") Then
                        CType(GridView_Impianti.Rows(j).FindControl("ChkSelezionaImpianto"), CheckBox).Checked = True
                        CType(GridView_Impianti.Rows(j).FindControl("Txt_Trattata"), TextBox).Text = GridView_Impianti.DataKeys(j).Item("Sup_Imp")
                        Exit For
                    End If

                Next

            Else
                For j = 0 To GridView_Planning.Rows.Count - 1

                    If ListaImpiantiRicetta(i).Programmazione_Entita_cod = GridView_Planning.DataKeys(j).Item("Programmazione_Entita_Cod") Then
                        CType(GridView_Planning.Rows(j).FindControl("ChkSelezionaPlanning"), CheckBox).Checked = True
                        CType(GridView_Planning.Rows(j).FindControl("Txt_Trattata_Planning"), TextBox).Text = ListaImpiantiRicetta(i).Qta2
                        Exit For
                    End If

                Next
            End If

        Next

        Dim script As New StringBuilder
        script.AppendLine("$(document).ready(function () { ")
        If objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale Then
            script.AppendLine("     RicalcolaSuperficieCoinvolta(); ")
            script.AppendLine("     RicalcolaSuperficieTotale(); ")
        Else
            script.AppendLine("     RicalcolaSuperficieCoinvoltaPlanning(); ")
            script.AppendLine("     RicalcolaSuperficieTotalePlanning(); ")
        End If

        script.AppendLine("}); ")

        ScriptManager.RegisterStartupScript(UpdatePanelRicette, UpdatePanelRicette.GetType(),
                                         String.Format("jQuery_{0}", UpdatePanelRicette.ClientID), script.ToString, True)

    End Sub


    Protected Sub Btn_Inserisci_Costi_Ricetta_Click(sender As Object, e As EventArgs) Handles Btn_Inserisci_Costi_Ricetta.Click
        Dim ricetta As String = rbl_RicetteAttive.SelectedValue
        If ricetta = "" Then
            Messaggi.AgroMsgBox("Selezionare una ricetta prima di procedere ", Page, , UpdatePanelRicette)
            Exit Sub
        End If
        ricetta_cod = Split(ricetta, "|")(0)
        Session("ricetta_cod") = ricetta_cod
        Ricetta_Operazione_Cod = Split(ricetta, "|")(1)
        Session("Ricetta_Operazione_Cod") = Ricetta_Operazione_Cod
        Ricetta_Tipo = Split(ricetta, "|")(2)
        Session("Ricetta_Tipo") = Ricetta_Tipo

        Dim impiantiPreSelezionati As Integer = GetImpianti().Count

        Dim dt_CostiRicetta As DataTable = Inserisci_Costi_Della_Ricetta()

        If IsNothing(dt_CostiRicetta) Or dt_CostiRicetta.Rows.Count = 0 Then
            Messaggi.AgroMsgBox("La ricetta non indica nessun costo accessorio.", Page, , UpdatePanelRicette)
        End If

    End Sub


    Private Function Inserisci_Costi_Della_Ricetta() As DataTable

        Dim dt_RicetteDett As DataTable = New AgronicaCoreContabDAL.Ricette_Dettagli_R().Leggi(CInt(ricetta_cod),
                                                                                               CInt(Ricetta_Operazione_Cod),
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
            Messaggi.AgroMsgBox("Selezionare una ricetta prima di procedere ", Page, , UpdatePanelRicette)
            Exit Sub
        End If
        ricetta_cod = Split(ricetta, "|")(0)
        Session("ricetta_cod") = ricetta_cod
        Ricetta_Operazione_Cod = Split(ricetta, "|")(1)
        Session("Ricetta_Operazione_Cod") = Ricetta_Operazione_Cod
        Ricetta_Tipo = Split(ricetta, "|")(2)
        Session("Ricetta_Tipo") = Ricetta_Tipo

        Select Case Ricetta_Tipo
            Case enum_TipoRicetta.Standard_Destinazioni, enum_TipoRicetta.PianoDistribuzioneConcimi
            Case Else
                Dim listaImpiantiSelezionati As List(Of Impianto) = GetImpianti()
                If listaImpiantiSelezionati.Count = 0 Then
                    Messaggi.AgroMsgBox("Non è selezionato nessun impianto dalla tabella", Page, , UpdatePanelRicette)
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
        script.AppendLine("             $('#dialogRicette').dialog('close');")
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
            Return Ricetta_Operazione_Cod
        End Get
        Set(value As String)
            Ricetta_Operazione_Cod = value
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

    Public Property Property_RBL_Salva() As Global.System.Web.UI.WebControls.RadioButtonList
        Get
            Return RBL_Salva
        End Get
        Set(value As Global.System.Web.UI.WebControls.RadioButtonList)
            RBL_Salva = value
        End Set
    End Property

    Public Property Property_UpdatePanelToolBar() As UpdatePanel
        Get
            Return UpdatePanelToolBar
        End Get
        Set(value As UpdatePanel)
            UpdatePanelToolBar = value
        End Set
    End Property

    Public Property Property_UpdatePanelToolBar1() As UpdatePanel
        Get
            Return UpdatePanelToolBar1
        End Get
        Set(value As UpdatePanel)
            UpdatePanelToolBar1 = value
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

    Public Property Property_UpdatePanelMagazzino() As Global.System.Web.UI.UpdatePanel
        Get
            Return UpdatePanelMagazzino
        End Get
        Set(value As Global.System.Web.UI.UpdatePanel)
            UpdatePanelMagazzino = value
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

    'da Agenda.Master
    Public Property Property_Lbl_Titolo() As Global.System.Web.UI.WebControls.Label
        Get
            Return CType(Me.Master, Agenda).Property_Lbl_Titolo
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            CType(Me.Master, Agenda).Property_Lbl_Titolo = value
        End Set
    End Property

    'da Agenda.Master
    Public Property Property_ImgBtn_AnnullaTutto() As Global.System.Web.UI.WebControls.ImageButton
        Get
            Return CType(Me.Master, Agenda).Property_ImgBtn_AnnullaTutto
        End Get
        Set(value As Global.System.Web.UI.WebControls.ImageButton)
            CType(Me.Master, Agenda).Property_ImgBtn_AnnullaTutto = value
        End Set
    End Property

    Public Property Property_NoteGiustPanel() As Global.System.Web.UI.WebControls.Panel
        Get
            Return Me.NoteGiustPanel
        End Get
        Set(value As Global.System.Web.UI.WebControls.Panel)
            Me.NoteGiustPanel = value
        End Set
    End Property

    Public Property Property_UpdatePanelData() As Global.System.Web.UI.UpdatePanel
        Get
            Return UpdatePanelData
        End Get
        Set(value As Global.System.Web.UI.UpdatePanel)
            UpdatePanelData = value
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

    Public Property Property_AttivitaDefaultCostiAccessori() As Object
        Get
            Return ViewState("AttivitaDiDefault")
        End Get
        Set(value As Object)
            ViewState("AttivitaDiDefault") = value
        End Set
    End Property

    'Public Property Property_() As Global.System.Web.UI.WebControls.TextBox
    '    Get
    '        Return XX
    '    End Get
    '    Set(value As Global.System.Web.UI.WebControls.TextBox)
    '        XX = value
    '    End Set
    'End Property

    Public Property Property_hf_esistonoCostiCollegatiCDG() As String
        Get
            Return hf_esistonoCostiCollegatiCDG.Value
        End Get
        Set(value As String)
            hf_esistonoCostiCollegatiCDG.Value = value
        End Set
    End Property

#End Region






    Private Sub dgrScaricoAvanzati_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles dgrScaricoAvanzati.RowCommand

        caricaSuSessionDTControlliDgrScaricoAvanzati()
        AggiornaCostiAccessoriAvanzati_Click(Me, Nothing)

        Dim IndiceRigaGriglia As Integer = 0
        Dim Dr As DataRow

        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Select Case e.CommandName

            Case "EliminaCosto"

                If Not Session("dtScarico") Is Nothing Then

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



    Public Sub ApriContattiCosti(ByVal piva As String, ByVal TipoRapp_cont As Integer, ByVal UpdatPanel As UpdatePanel)

        Dim QueryString As String
        QueryString = "?o=" &
                        Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Server) &
                        "&tipo_rapporto=" &
                        Stringa_Codifica(TipoRapp_cont, AgroKey_EncoderDecoder, Server) &
                        "&lav_cod=" &
                        Stringa_Codifica(objParametriAgenda.Lav_Cod, AgroKey_EncoderDecoder, Server) &
                        "&piva=" &
                        Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) &
                        "&orig=" &
                        Stringa_Codifica("", AgroKey_EncoderDecoder, Server) &
                        "&codcont=" &
                        Stringa_Codifica("0", AgroKey_EncoderDecoder, Server) &
                        "&dialog=" &
                        Stringa_Codifica("true", AgroKey_EncoderDecoder, Server)

        'OLD URL: "../GestioneContatti/Contatto.aspx"
        Dim StrWindowOpen As String = AgronicaCoreDataProvider.UtilityProvider.JqueryModalDialogScript(
                 "../Anagrafica/New_Contatto_Edit.aspx", QueryString, UpdatPanel.ClientID,
                768, 1024, 0, 0,
                , , , , , , NomeForm:="aspnetForm")

        Dim tags As Boolean = True
        ScriptManager.RegisterStartupScript(UpdatPanel, _
                                            UpdatPanel.GetType(), _
                                            String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, tags)

    End Sub


    'Public Sub ApriMacchinaCosti(ByVal piva As String)

    '    Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
    '    objGiasOnline.PaginaRichiesta = enum_PagineGiasOnline.ParcoMacchine_Edit
    '    objGiasOnline.Piva = objParametriAgenda.Piva
    '    objGiasOnline.Operazione = enum_TipoOperazioneDB.Scrittura

    '    Dim strJS As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline( _
    '                              Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
    '                              objGiasOnline)


    '    Dim StrWindowOpen As String = AgronicaCoreDataProvider.UtilityProvider.JqueryModalDialogScript( _
    '     strJS, "", UpdateCostiAccessori.ClientID, _
    '    550, 850, 0, 0, _
    '    , , , , , , NomeForm:="aspnetForm")

    '    Dim tags As Boolean = True
    '    ScriptManager.RegisterStartupScript(UpdateCostiAccessori, _
    '                                        UpdateCostiAccessori.GetType(), _
    '                                        String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, tags)


    'End Sub

    'Protected Sub SalvaCostiDefault_Click(sender As Object, e As EventArgs) Handles SalvaCostiDefault.Click

    '    SalvaCostiAccessori_SuAgendaMovimenti(True, dgrScarico)

    '    Dim dtScarico As DataTable
    '    Dim i As Integer = 0

    '    If Not Session("dtScarico") Is Nothing Then

    '        dtScarico = Session("dtScarico")

    '        Dim val2save As String = ""
    '        Dim strMacCod As String = ""
    '        Dim strContCod As String = ""

    '        val2save += "lav_cod=" + objParametriAgenda.Lav_Cod

    '        For i = 0 To dtScarico.Rows.Count - 1
    '            Select Case dtScarico.Rows(i).Item("elem_cod")
    '                Case MACCHINE
    '                    strMacCod &= dtScarico.Rows(i).Item("mat_cod") & ","
    '                Case 0
    '                    strContCod &= dtScarico.Rows(i).Item("mat_cod") & ","
    '            End Select
    '        Next

    '        If strMacCod <> "" Then
    '            strMacCod = "|mac_cod={" + strMacCod.Substring(0, strMacCod.Length - 1) + "}"
    '            val2save += strMacCod
    '        End If

    '        If strContCod <> "" Then
    '            strContCod = "|cod_cont={" + strContCod.Substring(0, strContCod.Length - 1) + "}"
    '            val2save += strContCod
    '        End If

    '        Dim objProf_W As New AgronicaCoreProfilazioneBIZ.Profilazione_W
    '        If (objProf_W.Scrivi_Inserisce_O_Aggiorna(objParametriAgenda.Piva, _
    '                                                  objParametriAgenda.Lav_Cod, _
    '                                                  "macXlav", _
    '                                                  objParametri_Server.PivaSuperUser, _
    '                                                  "Macchine/Contatti per operazioni", val2save, _
    '                                                  AGRODATAINIZIO, AGRODATAFINE, _
    '                                                  objParametriAgenda.Lav_Cod, _
    '                                                  0, _
    '                                                  objParametri_Server)) Then
    '        End If

    '    End If


    'End Sub


    Private Sub BottoneNascostoContatti_Click(sender As Object, e As System.EventArgs) Handles BottoneNascostoContatti.Click

        Popola_Manodopera()
        Popola_Terzisti()
        Popola_TecnicoResponsabile()

        If Not IsNothing(Session("IndiceRigaGrigliaCliccato")) AndAlso IsNumeric(Session("IndiceRigaGrigliaCliccato")) Then
            'dgrCentriCosto_selezionato_tip()
            If ViewState("ProvenienzaCentroDiCosto") = "CMB" Then
                CmbCentriDiCostoAvanzati.SelectedIndex = CInt(Session("IndiceRigaGrigliaCliccato"))
                CmbCentriDiCostoAvanzati_SelectedIndexChanged(Me, Nothing)
            ElseIf ViewState("ProvenienzaCentroDiCosto") = "DGR" Then
                dgrCentriCostoAvanzati.SelectedIndex = CInt(Session("IndiceRigaGrigliaCliccato"))
                dgrCentriCostoAvanzati_SelectedIndexChanged(Me, Nothing)
            End If

            Session("IndiceRigaGrigliaCliccato") = Nothing
        Else
            If ViewState("ProvenienzaCentroDiCosto") = "CMB" AndAlso CmbCentriDiCostoAvanzati.SelectedIndex >= 0 Then
                'dgrCentriCosto_selezionato_tip()
                CmbCentriDiCostoAvanzati_SelectedIndexChanged(Me, Nothing)
            ElseIf ViewState("ProvenienzaCentroDiCosto") = "DGR" AndAlso dgrCentriCostoAvanzati.SelectedIndex >= 0 Then
                'dgrCentriCosto_selezionato_tip()
                dgrCentriCostoAvanzati_SelectedIndexChanged(Me, Nothing)
            End If

        End If

    End Sub



    Private Sub SalvaCostiAvanzatiDefault_Click(sender As Object, e As System.EventArgs) Handles SalvaCostiAvanzatiDefault.Click

        SalvaCostiAccessori_SuAgendaMovimenti(True, dgrScaricoAvanzati)

        Dim dtScarico As DataTable
        Dim i As Integer = 0

        If Not Session("dtScarico") Is Nothing Then

            dtScarico = Session("dtScarico")

            Dim val2save As String = ""
            Dim strMacCod As String = ""
            Dim strContCod As String = ""

            val2save += "lav_cod=" + objParametriAgenda.Lav_Cod

            For i = 0 To dtScarico.Rows.Count - 1
                Select Case dtScarico.Rows(i).Item("elem_cod")
                    Case MACCHINE
                        strMacCod &= dtScarico.Rows(i).Item("mat_cod") & ","
                    Case 0
                        strContCod &= dtScarico.Rows(i).Item("mat_cod") & ","
                End Select
            Next

            If strMacCod <> "" Then
                strMacCod = "|mac_cod={" + strMacCod.Substring(0, strMacCod.Length - 1) + "}"
                val2save += strMacCod
            End If

            If strContCod <> "" Then
                strContCod = "|cod_cont={" + strContCod.Substring(0, strContCod.Length - 1) + "}"
                val2save += strContCod
            End If

            Dim objProf_W As New AgronicaCoreProfilazioneBIZ.Profilazione_W
            If (objProf_W.Scrivi_Inserisce_O_Aggiorna(objParametriAgenda.Piva, _
                                                      objParametriAgenda.Lav_Cod, _
                                                      "macXlav", _
                                                      objParametri_Server.PivaSuperUser, _
                                                      "Macchine/Contatti per operazioni", val2save, _
                                                      AGRODATAINIZIO, AGRODATAFINE, _
                                                      objParametriAgenda.Lav_Cod, _
                                                      0, _
                                                      objParametri_Server)) Then
            End If

        End If

    End Sub

    Protected Sub CmbCentriDiCostoAvanzati_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CmbCentriDiCostoAvanzati.SelectedIndexChanged

        ViewState("ProvenienzaCentroDiCosto") = "CMB"
        AggiornaListaCostiAccessoriAvanzati()

        'MARCO G: Imposto la combobox con il filtro di ricerca
        impostaPluginCombo()

        'Coloro la riga
        If Not IsNothing(dgrCentriCostoAvanzati.SelectedRow) Then
            dgrCentriCostoAvanzati.SelectedRow.BackColor = Drawing.Color.Gold
        End If

    End Sub

    Private Sub AggiornaListaCostiAccessoriAvanzati()

        caricaSuSessionDTControlliDgrScaricoAvanzati()
        AggiornaCostiAccessoriAvanzati_Click(Me, Nothing)

        Dim Dt As DataTable

        ' lo rendo visibile solo se ho scelto un magazzino (codice cdc>0)
        'Filtro_MaterialiAvanzati.Visible = False

        ' svuoto la variabile nel viewstate
        ViewState("SaCodFabbricato") = ""

        Lbl_RisFiltroAvanzati.Text = ""

        Dim Fabbricato_Cod, Sa_Cod As Integer

        If ViewState("ProvenienzaCentroDiCosto") = "CMB" AndAlso CmbCentriDiCostoAvanzati.SelectedValue.Contains("§") Then
            Fabbricato_Cod = CmbCentriDiCostoAvanzati.SelectedValue.Split("§")(0)
            Sa_Cod = CmbCentriDiCostoAvanzati.SelectedValue.Split("§")(1)
        ElseIf ViewState("ProvenienzaCentroDiCosto") = "DGR" Then
            Fabbricato_Cod = CInt(dgrCentriCostoAvanzati.DataKeys(dgrCentriCostoAvanzati.SelectedIndex).Item("Fabbricato_Cod"))
            Sa_Cod = CInt(dgrCentriCostoAvanzati.DataKeys(dgrCentriCostoAvanzati.SelectedIndex).Item("Sa_Cod"))
        Else
            Exit Sub
        End If

        Select Case Fabbricato_Cod

            Case Is > 0

                'CreaComboMagazzini(Fabbricato_Cod)

                Dim elem_cod As Integer
                Select Case CInt(dgrCentriCostoAvanzati.DataKeys(dgrCentriCostoAvanzati.SelectedIndex).Item("Fabbricato_Cod"))
                    Case -6 'Carburanti
                        elem_cod = 2
                    Case -7 ' Servizi Professionali
                        elem_cod = 700
                    Case -8 ' Altre Risorse
                        elem_cod = 200
                    Case -9 ' Ricambi
                        elem_cod = 401
                End Select

                'Filtro_MaterialiAvanzati.Visible = True

                ViewState("SaCodFabbricato") = Sa_Cod

                Me.dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.CategoriaProdotto
                Me.dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Prodotto
                Me.dgrMaterialiAvanzati.Columns(3).Visible = True
                Me.dgrMaterialiAvanzati.Columns(3).HeaderText = "Lotto"
                Me.dgrMaterialiAvanzati.Columns(4).HeaderText = Resources.AgronicaAgenda_2010.Giacenza

                Popola_ProdottiMagazzino(ViewState("SaCodFabbricato"), Fabbricato_Cod, elem_cod, True)
                Dt = Session("Dt_Prodotti")

                'Session("Dt_Prodotti") = Dt

                Me.dgrMaterialiAvanzati.DataSource = Dt
                Me.dgrMaterialiAvanzati.DataBind()

                ' commentato e aggiunto exit nicoletta 13/03/2014
                Exit Sub

            Case -1

                'Cambio l'intestazione delle colonne nel datagrid
                dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.MacchinaAttrezzatura
                dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Descrizione
                Me.dgrMaterialiAvanzati.Columns(3).Visible = False
                Me.dgrMaterialiAvanzati.Columns(4).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

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
                Me.dgrMaterialiAvanzati.Columns(3).Visible = False
                Me.dgrMaterialiAvanzati.Columns(4).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

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
                Me.dgrMaterialiAvanzati.Columns(3).Visible = False
                Me.dgrMaterialiAvanzati.Columns(4).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

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
                Me.dgrMaterialiAvanzati.Columns(3).Visible = False
                Me.dgrMaterialiAvanzati.Columns(4).HeaderText = Resources.AgronicaAgenda_2010.UnitàDiMisura

                If Not IsNothing(Session("Dt_TecnicoResponsabile")) Then
                    Dt = Session("Dt_TecnicoResponsabile")
                Else
                    ' nico
                    Popola_TecnicoResponsabile()
                    Dt = Session("Dt_TecnicoResponsabile")
                End If

                Me.dgrMaterialiAvanzati.DataSource = Dt
                Me.dgrMaterialiAvanzati.DataBind()

            Case -5

                ''Cambio l'intestazione delle colonne nel datagrid
                'Me.dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.CategoriaProdotto
                'Me.dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Prodotto
                'Me.dgrMaterialiAvanzati.Columns(3).HeaderText = "Codice Articolo"

                'If Not IsNothing(Session("Dt_TuttiProdotti")) Then
                '    Dt = Session("Dt_TuttiProdotti")
                'Else
                '    Popola_TuttiProdotti()
                '    Dt = Session("Dt_TuttiProdotti")
                'End If

                'Me.dgrMaterialiAvanzati.DataSource = Dt
                'Me.dgrMaterialiAvanzati.DataBind()

            Case -6, -7, -8, -9

                'CreaComboMagazzini(Fabbricato_Cod)

                Dim elem_cod As Integer
                Select Case Fabbricato_Cod
                    Case -6 'Carburanti
                        elem_cod = 2
                    Case -7 ' Servizi Professionali
                        elem_cod = 700
                    Case -8 ' Altre Risorse
                        elem_cod = 200
                    Case -9 ' Ricambi
                        elem_cod = 401
                End Select

                'Cambio l'intestazione delle colonne nel datagrid
                Me.dgrMaterialiAvanzati.Columns(1).HeaderText = Resources.AgronicaAgenda_2010.CategoriaProdotto
                Me.dgrMaterialiAvanzati.Columns(2).HeaderText = Resources.AgronicaAgenda_2010.Prodotto
                Me.dgrMaterialiAvanzati.Columns(3).HeaderText = "Codice Articolo"

                Popola_TuttiProdotti(elem_cod)
                Dt = Session("Dt_TuttiProdotti")

                Me.dgrMaterialiAvanzati.DataSource = Dt
                Me.dgrMaterialiAvanzati.DataBind()

        End Select

        ' nico
        ControllaDDLCostiAccessori(dgrScaricoAvanzati)

    End Sub

    Private Sub impostaPluginCombo()
        'MARCO G: Imposto la combobox con il filtro di ricerca
        Dim StrSelect As String = "$(document).ready(function () { $('#" & CmbCentriDiCostoAvanzati.ClientID & "').combobox();});"
        ScriptManager.RegisterStartupScript(UpdateCostiAccessoriAvanzati, UpdateCostiAccessoriAvanzati.GetType(),
                                String.Format("jQuery_{0}", CmbCentriDiCostoAvanzati.ClientID), StrSelect, True)
    End Sub

    Private Sub caricaSuSessionDTControlliDgrScaricoAvanzati()

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

                If IsNumeric(CType(dgrScaricoAvanzati.Rows(i).FindControl("Cmb_Qualifica"), DropDownList).SelectedValue) Then
                    dummylist.Rows(i).Item("Qualifica_cod") = CType(dgrScaricoAvanzati.Rows(i).FindControl("Cmb_Qualifica"), DropDownList).SelectedValue
                Else
                    dummylist.Rows(i).Item("Qualifica_cod") = 0
                End If

                If IsNumeric(CType(dgrScaricoAvanzati.Rows(i).FindControl("Cmb_Tariffa"), DropDownList).SelectedValue) Then
                    dummylist.Rows(i).Item("Tariffa_cod") = CType(dgrScaricoAvanzati.Rows(i).FindControl("Cmb_Tariffa"), DropDownList).SelectedValue
                Else
                    dummylist.Rows(i).Item("Tariffa_cod") = 0
                End If

                If dummylist.Rows(i).Item("Id_Attivita") <> 0 Then
                    Select Case ALGORITMO_COSTI_ACCESSORI

                        Case enum_AlgoritmoCostiAccessori.CAB
                            If dummylist.Rows(i).Item("Turno_Cod") <> 0 AndAlso IsDate(txt_DataOperazione.Text) Then

                                Dim objAttivita As New AgronicaCoreContabDAL.Attivita_R
                                dummylist.Rows(i).Item("Costo_Unitario") = Format(objAttivita.CostoOrario(dummylist.Rows(i).Item("Id_Attivita"), dummylist.Rows(i).Item("Turno_Cod"), txt_DataOperazione.Text, objParametri_Server), "0.00")
                                dummylist.Rows(i).Item("Costo") = Format(dummylist.Rows(i).Item("Costo_Unitario") * dummylist.Rows(i).Item("Qta_Ril"), "0.00")

                            End If
                        Case enum_AlgoritmoCostiAccessori.SBTF
                            If dummylist.Rows(i).Item("Qualifica_cod") <> 0 AndAlso dummylist.Rows(i).Item("Tariffa_cod") <> 0 Then
                                '  Marco Grilli, 08/09/2016 12:28:47: Leggo il costo orario data tariffa e qualifica
                                Dim objQxT_R As New AgronicaCoreContabDAL.QualificheXTariffe_R
                                Dim dtQxT As DataTable = objQxT_R.Leggi(objParametriAgenda.Piva, 1, dummylist.Rows(i).Item("Qualifica_cod"), dummylist.Rows(i).Item("Tariffa_cod"), "", "", objParametri_Server)

                                '  Marco Grilli, 08/09/2016 12:29:05:  se ho trovato una tariffa per la specifica qualifica, la imposto
                                If dtQxT.Rows.Count > 0 Then
                                    dummylist.Rows(i).Item("Costo_Unitario") = Format(dtQxT.Rows(0).Item("Valore"), "0.00")
                                    dummylist.Rows(i).Item("Costo") = Format(dummylist.Rows(i).Item("Costo_Unitario") * dummylist.Rows(i).Item("Qta_Ril"), "0.00")

                                Else
                                    'Tariffa non trovata
                                    dummylist.Rows(i).Item("Costo_Unitario") = "0.00"
                                    dummylist.Rows(i).Item("Costo") = "0.00"

                                End If

                            End If

                    End Select

                End If

            Next

            Session("dtScarico") = dummylist
        End If

    End Sub

End Class