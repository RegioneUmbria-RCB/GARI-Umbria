Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModelsSTD.attivita.Attivita
Imports AgronicaCoreUtility

Public Class Installazione_Trappole
    Inherits System.Web.UI.Page




    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub



    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda
    Dim TabellaTrappole As Table
    Dim Id_Agenda_Old As Integer
    Public Master_Operazione As Operazione
    Dim Movimento_Dettaglio_Contabilizzato_Installazione As Integer
    Dim Movimento_Dettaglio_Pendente_Installazione As Integer
    Dim Movimento_Dettaglio_Contabilizzato_Magazzino As Integer
    Dim Movimento_Dettaglio_Pendente_Magazzino As Integer
    Dim Movimento_Dettaglio_Extra_Date As Date
    Dim Movimento_Dettaglio_Anno As Integer
    Dim premutosalva As Boolean = False

    Private Sub Installazione_Trappole_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Master_Operazione = CType(Page.Master, Operazione)

        AddHandler Master_Operazione.Property_BTN_ChangeData.Click, AddressOf Me.aggiornaDataScadenza
        AddHandler Master_Operazione.Property_ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        AddHandler Master_Operazione.Property_BTN_Magazzini.Click, AddressOf Me.CambioMagazzino
        AddHandler Master_Operazione.Property_BTN_ComboSpecie.Click, AddressOf Me.CambioSpecie
        AddHandler Master_Operazione.Property_ImgBtn_Salva.Click, AddressOf Me.SalvaTutto
        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Carico"), ImageButton).Click, AddressOf Me.BTN_CaricoMagazzino
        AddHandler CType(Page.Master, MasterPage).PreRender, AddressOf Me.MasterUnload
        AddHandler Master_Operazione.Property_Btn_Conferma_Ricetta.Click, AddressOf Me.Btn_Conferma_Ricetta
        AddHandler Master_Operazione.Property_AggiornaGrigliaImpianti.Click, AddressOf Me.ImageButton_Sblocca_Click
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        verificoCredenzialiDiAccesso()
        inizializzoObjParametri()
        inizializzoParametriPagina()

        If Not IsPostBack Then

            Session("Tabella") = Nothing
            VerificaPermessi()
            LeggiImpostazioni()
            caricaControlli()
            disabilitaControlli()

        Else


            Select Case CInt(objParametriAgenda.Lav_Cod)
                Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                    If Not IsNothing(Session("Tabella")) Then
                        If IsNothing(TabellaTrappole) Then
                            'premutosalva = False
                            RigeneraTabellaPerSalvataggio()
                        End If

                    End If

                Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                    'non viene creata o utilizzata la tabella

                Case Else
                    Throw New NotImplementedException
            End Select

        End If


        'Dim str As New StringBuilder
        'str.Append("  $(document).ready(function () {")
        'str.Append("    $('#ModificaDose').click(function () { $('#" & _
        '             Master_Operazione.Property_AggiornaGrigliaImpianti.ClientID & "').click(); }); ")
        'str.Append("});")
        'ScriptManager.RegisterStartupScript(Master_Operazione.Property_UpdatePanelToolBar1, Master_Operazione.Property_UpdatePanelToolBar1.GetType(),
        '                                      String.Format("jQuery_{0}", Master_Operazione.Property_UpdatePanelToolBar1.ClientID), str.ToString, True)

        'str = New StringBuilder
        'str.Append("  $(document).ready(function () {")
        'str.Append("    $('#InserisciDose').click(function () { $('#" & _
        '             Master_Operazione.Property_AggiornaGrigliaImpianti.ClientID & "').click(); }); ")
        'str.Append("});")
        'ScriptManager.RegisterStartupScript(Master_Operazione.Property_UpdatePanelToolBar1, Master_Operazione.Property_UpdatePanelToolBar1.GetType(),
        '                                      String.Format("jQuery_{0}", Master_Operazione.Property_UpdatePanelToolBar1.ClientID), str.ToString, True)

    End Sub


#Region "Metodi Eseguiti nel Load"

    Private Sub verificoCredenzialiDiAccesso()
        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If
    End Sub


    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
    End Sub


    Private Sub inizializzoParametriAgenda()
        objParametriAgenda = New ParametriAgenda
        'objParametriAgenda.Leggi()
        objParametriAgenda.OperazioneMulticentro = False
        Id_Agenda_Old = objParametriAgenda.Id_Agenda
        Select Case CInt(objParametriAgenda.Lav_Cod)
            Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                objParametriAgenda.OperazioneMulticentro = False
            Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                objParametriAgenda.OperazioneMulticentro = True
        End Select

    End Sub


    Private Sub inizializzoParametriPagina()

        inizializzoParametriAgenda()

        Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim Lav_Des As String = objOperazioniLeggi.LavorazioneDes_from_LavorazioneCod(objParametriAgenda.Lav_Cod, objParametri_Server)
        objParametriAgenda.Lav_Des = Lav_Des
        Master_Operazione.Property_Lbl_Titolo.Text = Lav_Des

        'valori copiati da agenda vecchia
        Movimento_Dettaglio_Contabilizzato_Installazione = 1
        Movimento_Dettaglio_Pendente_Installazione = 3
        Movimento_Dettaglio_Contabilizzato_Magazzino = 1
        Movimento_Dettaglio_Pendente_Magazzino = 4
        Movimento_Dettaglio_Extra_Date = AGRODATAINIZIO
        Movimento_Dettaglio_Anno = 1900

        Select Case CInt(objParametriAgenda.Lav_Cod)

            Case LAVCOD_INSTALLAZIONE_TRAPPOLE

                objParametriAgenda.TrappolaUso = enum_TrappoleUso.Monitor
                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_CAMPO)

            Case LAVCOD_CATTURE_MASSA

                objParametriAgenda.TrappolaUso = enum_TrappoleUso.CattureDiMassa
                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.TRATTAMENTO)

            Case LAVCOD_CONFUSIONE_SESSUALE

                objParametriAgenda.TrappolaUso = enum_TrappoleUso.ConfusioneSessuale
                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.TRATTAMENTO)

            Case LAVCOD_DISORIENTAMENTO_SESSUALE

                objParametriAgenda.TrappolaUso = enum_TrappoleUso.Disorientamento
                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.TRATTAMENTO)

        End Select
    End Sub


    Private Sub VerificaPermessi()
        Dim UtenteAbilitato_Lettura As Boolean = False
        Dim UtenteAbilitato_Modifica As Boolean = False
        Dim objUtility As New AgronicaCoreModello.Utility_Operazioni

        objUtility.Verifica_Permessi_OperazioniAgenda_X_PagineAgronicaAgenda(objParametri_Server, objParametri_Utenti, UtenteAbilitato_Lettura, UtenteAbilitato_Modifica)

        Session("UtenteAbilitato_Lettura") = UtenteAbilitato_Lettura

        If Not UtenteAbilitato_Lettura Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
            Exit Sub
        End If

        Session("UtenteAbilitato_Modifica") = UtenteAbilitato_Modifica

        If objParametriAgenda.Tipo_Operazione <> enum_TipoOperazioneDB.Lettura AndAlso Not UtenteAbilitato_Modifica Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
            Exit Sub
        End If

    End Sub


    Private Sub LeggiImpostazioni()
        '--------------------------------------
        'leggo le eventuali IMPOSTAZIONI UTENTE

        'viene fatta dal metodo MasterUnload() richiamato dalla master.
        ''AddHandler CType(Page.Master, MasterPage).PreRender, AddressOf Me.MasterUnload
        'Cmb_Trappola_Carica()
    End Sub


    Private Sub caricaControlli()
        Select Case objParametriAgenda.Tipo_Operazione

            Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                Me.Txt_NumeroTrappole.Text = ""

            Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica
                RipristinaControlliDaAgenda()
                Master_Operazione.CaricaCostiAccessori()

            Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura
                RipristinaControlliDaAgenda()
                Master_Operazione.CaricaCostiAccessori()

        End Select
    End Sub


    Private Sub disabilitaControlli()

        'controlo il permesso sulla specie, se non ce l'ho metto operazione in lettura
        If objParametriAgenda.Tipo_Operazione = CStr(TipiEnumerativi.enum_TipoOperazioneDB.Modifica) Then
            Try
                Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                Dim Dt As DataTable = objSpecVeg.SpecieVegetali_GestioneFiltroUtente_Leggi(objParametriAgenda.Veg_Cod.Split("/")(0),
                                                                 0,
                                                                 "",
                                                                 "",
                                                                 "",
                                                                 "",
                                                                 objParametri_Utenti)
                If Dt.Rows.Count = 0 Then
                    objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
                End If
            Catch ex As Exception
                objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
            End Try
        End If

        'impostazioni in base operazione di creazione/modifica..
        Select Case objParametriAgenda.Tipo_Operazione

            Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura

                Master_Operazione.Property_RBL_Salva.Visible = True
                Master_Operazione.Property_Box_Salva.Visible = True
                Master_Operazione.flag_MostraBtnSalvaCDG = True

                'impostazioni in base alla lavorazione
                Select Case CInt(objParametriAgenda.Lav_Cod)
                    Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                        Me.sfondoverde.Visible = True
                        Txt_GiacenzaInneschi.Visible = True

                        Me.ImageButton_MostraTrappole.Enabled = True
                        Me.ImageButton_MostraTrappole.Visible = True
                        Me.ImageButton_Sblocca.Enabled = False
                        Me.ImageButton_Sblocca.Visible = False

                    Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                        Me.sfondoverde.Visible = False
                        Txt_GiacenzaInneschi.Visible = True
                    Case Else
                        Throw New NotImplementedException
                End Select


            Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica

                'impostazioni in base alla lavorazione
                Select Case CInt(objParametriAgenda.Lav_Cod)
                    Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA

                        Me.ImageButton_MostraTrappole.Enabled = False
                        Me.ImageButton_MostraTrappole.Visible = False

                        'per evitare che vengano create nuove tabelle trappole e quindi nuovi id trappole che determinano
                        'incoerenza tra eventuali reinneschi e rilievi, non viene permessa la odifica delle trappole (numero e tipo)
                        'che causerebbero problemi. E' permesso solo agire sulla tabella e quindi impostare il numero di trappole
                        Me.ImageButton_Sblocca.Enabled = False
                        Me.ImageButton_Sblocca.Visible = False

                        cmb_Avversita.Enabled = False
                        cmb_Ditte.Enabled = False
                        cmb_Trappola.Enabled = False
                        Txt_CodAvversita.Enabled = False
                        Txt_DataScadenz.Enabled = True
                        Txt_Giacenza.Enabled = False
                        Txt_GiacenzaInneschi.Enabled = False
                        Txt_GiorniFeromone.Enabled = False
                        Txt_NumeroTrappole.Enabled = False

                        Master_Operazione.Property_Box_Salva.Enabled = True 'true per modifica
                        Master_Operazione.Property_Box_Salva.Visible = True 'true per modifica
                        Master_Operazione.Property_ImgBtn_Salva.Enabled = True 'true per modifica
                        'Per il salva e vai ai costi, abilito il controllo
                        Master_Operazione.Property_RBL_Salva.Enabled = True
                        Master_Operazione.Property_RBL_Salva.Items(1).Enabled = False
                        Master_Operazione.Property_RBL_Salva.Items(2).Enabled = False
                        Master_Operazione.flag_MostraBtnSalvaCDG = True

                        Master_Operazione.Property_txt_DataOperazione.Enabled = False

                        'Master_Operazione.Property_ImgBtn_Costi.Enabled = False

                        Master_Operazione.Property_txt_Note.Enabled = True 'true per modifica

                        Master_Operazione.Property_ComboSpecie.Enabled = False
                        Master_Operazione.Property_BTN_ComboSpecie.Enabled = False

                        Master_Operazione.Property_BTN_Magazzini.Enabled = False
                        Master_Operazione.Property_ComboMagazzini.Enabled = False

                        Master_Operazione.Property_BTN_CentroAziendale.Enabled = False
                        Master_Operazione.Property_ComboCentroAziendale.Enabled = False

                        Master_Operazione.Property_BTN_ComboOperazione.Enabled = False
                        Master_Operazione.Property_ComboOperazione.Enabled = False

                        Master_Operazione.Property_CBL_Consigli.Enabled = True

                        Master_Operazione.Property_GridView_Impianti.Enabled = False

                        BloccaComboJavaScript(True, True, True, True)

                        Me.sfondoverde.Visible = True
                        Txt_GiacenzaInneschi.Visible = True
                        If Not IsNothing(TabellaTrappole) Then
                            TabellaTrappole.Enabled = True
                        End If
                    Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                        Me.sfondoverde.Visible = False
                        Txt_GiacenzaInneschi.Visible = True

                        Me.ImageButton_MostraTrappole.Enabled = False
                        Me.ImageButton_MostraTrappole.Visible = False
                        Me.ImageButton_Sblocca.Enabled = False
                        Me.ImageButton_Sblocca.Visible = False

                        cmb_Avversita.Enabled = True
                        cmb_Ditte.Enabled = True
                        cmb_Trappola.Enabled = True
                        Txt_CodAvversita.Enabled = False
                        Txt_DataScadenz.Enabled = True
                        Txt_Giacenza.Enabled = False
                        Txt_GiacenzaInneschi.Enabled = False
                        Txt_GiorniFeromone.Enabled = False
                        Txt_NumeroTrappole.Enabled = True

                        Master_Operazione.Property_Box_Salva.Enabled = True 'true per modifica
                        Master_Operazione.Property_Box_Salva.Visible = True 'true per modifica
                        Master_Operazione.Property_ImgBtn_Salva.Enabled = True 'true per modifica
                        Master_Operazione.Property_RBL_Salva.Enabled = False
                        Master_Operazione.flag_MostraBtnSalvaCDG = True

                        Master_Operazione.Property_txt_DataOperazione.Enabled = True

                        'Master_Operazione.Property_ImgBtn_Costi.Enabled = True

                        Master_Operazione.Property_txt_Note.Enabled = True 'true per modifica

                        Master_Operazione.Property_ComboSpecie.Enabled = False
                        Master_Operazione.Property_BTN_ComboSpecie.Enabled = False

                        Master_Operazione.Property_BTN_Magazzini.Enabled = True
                        Master_Operazione.Property_ComboMagazzini.Enabled = True

                        Master_Operazione.Property_BTN_CentroAziendale.Enabled = False
                        Master_Operazione.Property_ComboCentroAziendale.Enabled = False

                        Master_Operazione.Property_BTN_ComboOperazione.Enabled = False
                        Master_Operazione.Property_ComboOperazione.Enabled = False

                        Master_Operazione.Property_CBL_Consigli.Enabled = True

                        Master_Operazione.Property_GridView_Impianti.Enabled = False

                        BloccaComboJavaScript(True, True, True, False)

                    Case Else
                        Throw New NotImplementedException
                End Select

            Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura


                Me.ImageButton_MostraTrappole.Enabled = False
                Me.ImageButton_MostraTrappole.Visible = False
                Me.ImageButton_Sblocca.Enabled = False
                Me.ImageButton_Sblocca.Visible = False

                'impostazioni in base alla lavorazione
                Select Case CInt(objParametriAgenda.Lav_Cod)
                    Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                        Me.sfondoverde.Visible = True
                        Txt_GiacenzaInneschi.Visible = True
                        If Not IsNothing(TabellaTrappole) Then
                            TabellaTrappole.Enabled = False
                        End If
                    Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                        Me.sfondoverde.Visible = False
                        Txt_GiacenzaInneschi.Visible = True
                    Case Else
                        Throw New NotImplementedException
                End Select

                cmb_Avversita.Enabled = False
                cmb_Ditte.Enabled = False
                cmb_Trappola.Enabled = False
                Txt_CodAvversita.Enabled = False
                Txt_DataScadenz.Enabled = False
                Txt_Giacenza.Enabled = False
                Txt_GiacenzaInneschi.Enabled = False
                Txt_GiorniFeromone.Enabled = False
                Txt_NumeroTrappole.Enabled = False

                Master_Operazione.Property_Box_Salva.Enabled = False 'true per modifica
                Master_Operazione.Property_Box_Salva.Visible = False 'true per modifica
                Master_Operazione.Property_ImgBtn_Salva.Enabled = False 'true per modifica
                Master_Operazione.Property_RBL_Salva.Enabled = False 'true per modifica

                Master_Operazione.Property_txt_DataOperazione.Enabled = False

                'Master_Operazione.Property_ImgBtn_Costi.Enabled = False

                Master_Operazione.Property_txt_Note.Enabled = False 'true per modifica

                Master_Operazione.Property_ComboSpecie.Enabled = False
                Master_Operazione.Property_BTN_ComboSpecie.Enabled = False

                Master_Operazione.Property_BTN_Magazzini.Enabled = False
                Master_Operazione.Property_ComboMagazzini.Enabled = False

                Master_Operazione.Property_BTN_CentroAziendale.Enabled = False
                Master_Operazione.Property_ComboCentroAziendale.Enabled = False

                Master_Operazione.Property_BTN_ComboOperazione.Enabled = False
                Master_Operazione.Property_ComboOperazione.Enabled = False

                Master_Operazione.Property_CBL_Consigli.Enabled = False

                Master_Operazione.Property_Box_Salva.Visible = False

                Master_Operazione.Property_GridView_Impianti.Enabled = False

                BloccaComboJavaScript(True, True, True, True)

        End Select


    End Sub



    Private Sub RipristinaControlliDaAgenda()


        Dim objAgenda As New Agenda_Operazione_Helper
        Dim Agenda As New Operazione_Agenda
        Agenda = objAgenda.Leggi(objParametriAgenda.Piva,
                                     CInt(objParametriAgenda.Sa_Cod),
                                     CInt(objParametriAgenda.Id_Agenda),
                                     0,
                                     objParametri_Server)


        If Not IsNothing(Agenda) Then

            objParametriAgenda.Piva = Agenda.Piva
            objParametriAgenda.Sa_Cod = Agenda.Sa_Cod
            objParametriAgenda.Lav_Cod = Agenda.Lav_Cod

            'NOTE
            If Not IsNothing(Agenda.Note) Then
                For i = 0 To Agenda.Note.Count - 1
                    objParametriAgenda.Note.Add(Agenda.Note(i))
                Next
            End If
            objParametriAgenda.salva()

            'MOVIMENTI
            If Not IsNothing(Agenda.Movimenti) Then

                Dim movimentiCosti As List(Of Movimento) = New List(Of Movimento)
                objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

                For i = 0 To Agenda.Movimenti.Count - 1

                    'controllo corrispondeza piva con agenda
                    If Agenda.Piva <> Agenda.Movimenti(i).Piva Then
                        Throw New ApplicationException
                    End If

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case enum_Agenda_Causali.RILIEVO_CAMPO, enum_Agenda_Causali.TRATTAMENTO


                            If Agenda.Lav_Cod <> objParametriAgenda.Lav_Cod Then
                                'controllino per lo sviluppo, da togliere
                                Throw New NotImplementedException
                            End If

                            Select Case CInt(Agenda.Lav_Cod)
                                Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                                    LeggiMovimentoAgenda_InstallazioneTrappole_CattureMassa(Agenda, i)
                                Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                                    LeggiMovimentoAgenda_Confusione_Disorientamento_Sessuale(Agenda, i)
                                Case Else
                                    Throw New NotImplementedException
                            End Select

                        Case enum_Agenda_Causali.SCARICO

                            'MOVIMENTI_DETTAGLI
                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                If Agenda.Movimenti(i).Movimenti_Dettagli.Count > 2 Then
                                    'previsto se ci son cost accessori
                                    'Throw New NotImplementedException
                                End If

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                    'controllo corrispondeza  piva con agenda, sa_cod potrebbe cambiare per chi ha fabbricato in altro centro
                                    If Agenda.Piva <> Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva Then
                                        Throw New ApplicationException
                                    End If

                                    Select Case Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod
                                        Case TRAPPOLE
                                            LeggiMovimentoAgenda_ScaricoTrappole(Agenda, i, j)

                                        Case INNESCHI
                                            Select Case CInt(objParametriAgenda.Lav_Cod)

                                                Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                                                    LeggiMovimentoAgenda_ScaricoInneschi(Agenda, i, j)

                                                Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                                                    'non previsto
                                                    Throw New NotImplementedException
                                                Case Else
                                                    'non previsto
                                                    Throw New NotImplementedException
                                            End Select

                                        Case Else
                                            '----COSTO ACCESSORIO---------------------
                                            'è un movimento dovuto ad un costo accessorio
                                            'Throw New NotImplementedException
                                            movimentiCosti.Add(Agenda.Movimenti(i))
                                            Exit For
                                    End Select
                                Next

                            End If



                        Case CAU_IMPUTAZIONE_PARCOMACCHINE
                            movimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_MANODOPERA
                            movimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_TERZISTI
                            movimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_UTILIZZO_PRODOTTI
                            movimentiCosti.Add(Agenda.Movimenti(i))

                        Case CAU_IMPUTAZIONE_TECNICO_RESPONSABILE
                            movimentiCosti.Add(Agenda.Movimenti(i))

                        Case Else
                            Throw New NotImplementedException

                    End Select
                Next

                objParametriAgenda.Movimenti = movimentiCosti

                'gestisco la selezione del fabbricato dell'azienda esterna se l'operazione era stata registrata con quella
                Dim util As New Utility_NS.Utility_Operazioni()
                util.ImpostaFabbricatoDelMagazzinoEsternoSePresente(Agenda, objParametriAgenda, objParametri_Server)



            End If

        End If

        'CONTROLLO SE CI SONO DEI COSTI COLLEGATI
        For Each mdRif As Movimento_Dettaglio_Riferimento In Agenda.Agenda_Riferimenti
            If mdRif.Lav_Cod_Rif = LAVCOD_COSTI_CDG Then
                'Messaggi.AgroMsgBuonFine("NB: Esistono costi collegati a questa operazione.", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                CType(Page.Master, Operazione).Property_hf_esistonoCostiCollegatiCDG = True
                Exit For
            End If
        Next

        '................................
        'se tutto è andato bene ora ho i dati e devo settare le combo, text e ricostruire la tabella... 



        'data, impianti, centro già impostati dalla master

        'Txt_SupSelezionata
        Dim ImpUtil As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim supImp As Decimal = 0
        For Each imp As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In objParametriAgenda.Impianti
            supImp += ImpUtil.LeggiSuperficie(Agenda.Piva, Agenda.Sa_Cod, imp.Appezza, imp.ID_Reg, objParametri_Server)
        Next
        Txt_SupSelezionata.Value = CStr(supImp)

        'dispenser, ditta fornitrice, avversita
        'imposta i valori preselezionati nelle combo della slave elencati.
        'essendo la combo dei dispenser (combo_trappola) dipendente dalla specie, che è nella master, i valori vengono impostati a seguito 
        'di un evento scatenato nella load della master, quindi dopo il caricamento della slave.
        'vedi metodo MasterUnload() sopra
        'qui salvo nel viewstate i valori da impostare
        'ViewState("dispenserSelezionato") = InstallazioneTrappole.CodiceProdotto
        'ViewState("dittaSelezionata") = InstallazioneTrappole.CodiceDitta
        'ViewState("avversitaSelezionata") = InstallazioneTrappole.AvCod
        'ImpostaValoriSelezionatiCombo()

        'codice avveristà
        Txt_CodAvversita.Text = objParametriAgenda.InstallazioneTrappola.SiglaAv

        'numero totale trappole
        Txt_NumeroTrappole.Text = objParametriAgenda.InstallazioneTrappola.NumeroTrappole

        'giacenze calcolate sopra con cmb_Trappola_SelectedIndexChanged(Nothing, Nothing)

        'durata feromone

        'scadenza

        Select Case CInt(objParametriAgenda.Lav_Cod)
            Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                'TabellaHtmlTrappole
                GeneraTabellaHtmlTrappole(False)
            Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                Me.sfondoverde.Visible = False
            Case Else
                'non serve la tabella
        End Select

        '................................


    End Sub


    Private Sub LeggiMovimentoAgenda_InstallazioneTrappole_CattureMassa(ByRef agenda As Operazione_Agenda, ByRef i As Integer)


        ''------------------------------------------
        ''----- Dichiarazione delle Variabili
        ''------------------------------------------

        'variabili univoche per ciascuna operazione agenda-trappola
        Dim NumeroTotaleTrappoleInstallate As Integer = 0
        Dim TrappolaCodiceProdotto As Integer = 0
        Dim TrappolaCodiceDittaTrappola As Integer = 0
        Dim TrappolaFreatimetro_CodicePersonalizzato As Decimal = 0
        Dim TrappolaSiglaAv As String = 0
        Dim TrappolaAvCod As Integer = 0
        Dim InneschiTotali As Integer = 0

        'lista degli appezzamenti coinvolti nell'operazione agenda
        Dim AppezzamentiCoinvolti_NumTrappole As New Hashtable()
        'elenco trappele (numero-nome personalizzato)
        Dim TrappoleLista As New Hashtable()
        'Coppia trappola - appezzamento che la contiene
        Dim Trappole_Appezzamenti As New Hashtable()
        'Coppia innesco - trappola che la contiene che la contiene
        Dim Inneschi_Trappole As New Hashtable()

        'variabili di controllo
        Dim conteggioTrappolePerVerifica As Integer = 0


        ''------------------------------------------
        ''----- Recupero le informazioni
        ''------------------------------------------



        objParametriAgenda.Data = agenda.Movimenti(i).Data

        Master_Operazione.SetNota(agenda.Movimenti(i).Mov_Desc)



        'MOVIMENTO_DETTAGLIO TECNICO (dovrebbe essere nothing)
        If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then

            For j = 0 To agenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1

                'controllo corrispondeza piva con agenda
                If agenda.Piva <> agenda.Movimenti(i).Piva Then
                    Throw New ApplicationException
                End If

                'non dovrebbe essercene nessuno
                Throw New NotImplementedException
            Next

        End If



        '---------------------
        '----MOVIMENTI_DETTAGLI
        '---------------------
        'uno per ciascun impianto influenzato dalle trappole installate
        If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli) Then

            For j = 0 To agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                'controllo corrispondeza piva con agenda
                'sa cod è uguale ma potrebbe cambiare se...
                If agenda.Piva <> agenda.Movimenti(i).Movimenti_Dettagli(j).Piva Then
                    Throw New ApplicationException
                End If

                'Movimento_Dettaglio.Elem_Cod = TRAPPOLE
                'implicito ma lo controllo dato che lo riscrivo
                If TRAPPOLE <> agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod Then
                    Throw New ApplicationException
                End If

                ' Movimento_Dettaglio.Pro_Cod = cmb_Trappola.SelectedItem.Value
                TrappolaCodiceProdotto = agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod
                If j > 0 AndAlso TrappolaCodiceProdotto <> agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod Then
                    'non devono esistere dettagli trappole sulla stessa operazione con dato diverso in Pro_Cod
                    Throw New ApplicationException
                End If

                'Movimento_Dettaglio.Udm_Cod = unità_misura_num_trappole ' unità di misura n.trappole
                'implicito
                If enum_UnitaMisura.Numero_Trappole <> agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod Then
                    Throw New ApplicationException
                End If

                'Movimento_Dettaglio.Qta = CInt(Txt_NumeroTrappole.Text)
                If j > 0 AndAlso NumeroTotaleTrappoleInstallate <> agenda.Movimenti(i).Movimenti_Dettagli(j).Qta Then
                    'non devono esistere dettagli trappole sulla stessa operazione con dato diverso in Qta
                    Throw New ApplicationException
                End If
                NumeroTotaleTrappoleInstallate = agenda.Movimenti(i).Movimenti_Dettagli(j).Qta

                'Movimento_Dettaglio.Contabilizzato = 1
                'implicito
                'Movimento_Dettaglio.Pendente = 3
                'implicito
                'Movimento_Dettaglio.Data = Data
                'Movimento_Dettaglio.BaseCode = BaseCode
                'implicito
                'Movimento_Dettaglio.TopCode = TopCode
                'implicito

                '---------------------
                'MOVIMENTO_DESTINAZIONI
                '---------------------
                'una sola destinazione per ciascun appezzamento coinvolto nell'operazione
                'a prescindere che ci sia o no la trappola
                Dim AppezzamentoCorrente As Integer = 0

                If IsNothing(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) OrElse
                             agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count <> 1 Then
                    'non devono esistere piu di 2
                    Throw New ApplicationException
                End If

                Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                objAppezzamento.Piva = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Piva
                objAppezzamento.Sa_Cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Sa_Cod
                objAppezzamento.Appezza = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Appezza
                objAppezzamento.ID_Reg = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Id_Destinazione

                'per mantenere compatibilità con agenda vecchia uso movdettecnico.count invece
                'di destinazioni.qta, che nell'agenda vecchia sono sempre 0
                'objAppezzamento.Qta = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta
                objAppezzamento.Qta = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count
                If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta2 > 0 Then
                    objAppezzamento.Qta2 = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta2
                Else
                    objAppezzamento.Qta2 = (New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read()).LeggiSuperficie(objAppezzamento.Piva, objAppezzamento.Sa_Cod, objAppezzamento.Appezza, objAppezzamento.ID_Reg, objParametri_Server)
                End If
                'controllo coerenza qta e num trapple, gestione agenda nuova e vecchia
                ControlloCoerenzaNumeroTrappole(agenda.Movimenti(i).Movimenti_Dettagli(j))


                'salvo la coppia appezzamento -numero trappole per questo appezzamento
                AppezzamentoCorrente = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Appezza
                AppezzamentiCoinvolti_NumTrappole.Add(AppezzamentoCorrente,
                                          agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count)

                conteggioTrappolePerVerifica += agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count

                Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                Dim Dt_Imp As New DataTable
                Dt_Imp = objImp.Leggi(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Piva,
                             agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Sa_Cod,
                             agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Appezza,
                             agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Id_Destinazione,
                             enumSelezioneVariabile.Selezione_JoinDescrizioni,
                             "", "", objParametri_Server)

                If Dt_Imp.Rows.Count > 0 Then

                    objParametriAgenda.Veg_Cod = CInt(Dt_Imp.Rows(0).Item("veg_cod"))

                End If

                objParametriAgenda.Impianti.Add(objAppezzamento)
                objParametriAgenda.salva()

                '---------------------
                'MOVIMENTI_DETTAGLI_TECNICI
                '---------------------
                'Un mov det tecnico per ciascuna effettiva installazione di trappola in un appezzamento
                'da qui ricavo il nome trappola e gli inneschi installati
                Dim Dose_Temp As Integer = 0

                If IsNothing(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici) Then
                    'se non ci sono dettagli allora la quantità di trappole inserite dell'appezzamento deve essere 0
                    'altrimenti c'è un errore  (qta in destinazione corrente), ma nell'agenda vecchia è comunque sempre 0
                    If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta <> "0" Then
                        Throw New ApplicationException
                    End If
                Else
                    If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta <>
                        agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count Then
                        'controllo coerenza qta e num trappole, gestione agenda nuova e vecchia
                        ControlloCoerenzaNumeroTrappole(agenda.Movimenti(i).Movimenti_Dettagli(j))
                    End If
                End If
                For x = 0 To agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count - 1

                    If x = 0 Then

                        'Parametri uguali per tutte le trappole dell'operazione:
                        'Movimento_Dettaglio_Tecnico.Ditta_cod = CStr(cmb_Ditte.SelectedItem.Value)
                        TrappolaCodiceDittaTrappola = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Ditta_cod
                        'Movimento_Dettaglio_Tecnico.Sigla_av = CStr(Txt_CodAvversita.Text)
                        TrappolaSiglaAv = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Sigla_av
                        'Movimento_Dettaglio_Tecnico.Av_Cod = cmb_Avversita.SelectedItem.Value
                        TrappolaAvCod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Av_Cod

                    ElseIf _
                        TrappolaCodiceDittaTrappola <> agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Ditta_cod OrElse
                        TrappolaSiglaAv <> agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Sigla_av OrElse
                        TrappolaAvCod <> agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Av_Cod _
                        Then

                        'non devono essere diversi tra i dettagli, genero eccezione
                        Throw New ApplicationException
                    End If



                    'univoche per ciascuna trappola
                    'Movimento_Dettaglio_Tecnico.Trap_num = iRiga
                    Dim TrappolaNumeroID As Integer = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Trap_num
                    'Movimento_Dettaglio_Tecnico.Freatimetro = codiceInterno
                    TrappolaFreatimetro_CodicePersonalizzato = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Freatimetro
                    'Dim TrappolaCodicePersonalizzatoDaExtraStrtingNonFreatimetro As String = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).ExtraStr

                    'aggiungo la trappola alla lista trappole
                    TrappoleLista.Add(TrappolaNumeroID, TrappolaFreatimetro_CodicePersonalizzato)

                    'aggiungo la trappola alla lista trappole appezzamenti
                    Trappole_Appezzamenti.Add(TrappolaNumeroID,
                                              agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Appezza)

                    'Movimento_Dettaglio_Tecnico.Dose = INNESCHI
                    Dim numeroInneschi As Decimal = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Dose

                    'aggiungo il numero inneschi per trappola
                    Inneschi_Trappole.Add(TrappolaNumeroID, numeroInneschi)

                    InneschiTotali += agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(x).Dose


                Next

            Next

            'controllo i conteggi trappole se coerenti
            If NumeroTotaleTrappoleInstallate <> conteggioTrappolePerVerifica Then
                Throw New ApplicationException
            End If

        Else
            'ci seve essere almeno un movimento dettaglio per l'ìinstallazione di una trappola in un appezzamento
            Throw New ApplicationException
        End If


        '---------Inserisco i dati in objParametriAgenda ------------

        'variabile per installazione trappole
        Dim InstallazioneTrap As New ParametriInstallazioneTrappole
        'aggiungo le variabili all'oggetto
        InstallazioneTrap.NumeroTrappole = NumeroTotaleTrappoleInstallate
        InstallazioneTrap.CodiceProdotto = TrappolaCodiceProdotto
        InstallazioneTrap.CodiceDitta = TrappolaCodiceDittaTrappola
        InstallazioneTrap.Freatimetro = TrappolaFreatimetro_CodicePersonalizzato
        InstallazioneTrap.SiglaAv = TrappolaSiglaAv
        InstallazioneTrap.AvCod = TrappolaAvCod
        InstallazioneTrap.NumeroInneschi = InneschiTotali
        'lista degli appezzamenti coinvolti nell'operazione agenda
        InstallazioneTrap.Appezzamenti_X_NumTrappole = AppezzamentiCoinvolti_NumTrappole
        'elenco trappele (numero-nome personalizzato)
        InstallazioneTrap.Trappole_X_Nome = TrappoleLista
        'Coppia trappola - appezzamento che la contiene
        InstallazioneTrap.Trappole_X_Appezzamento = Trappole_Appezzamenti
        'Coppia innesco - trappola che la contiene che la contiene
        InstallazioneTrap.Trappole_X_NumInneschi = Inneschi_Trappole

        objParametriAgenda.InstallazioneTrappola = InstallazioneTrap
    End Sub

    Private Sub LeggiMovimentoAgenda_Confusione_Disorientamento_Sessuale(ByRef agenda As Operazione_Agenda, ByRef i As Integer)


        ''------------------------------------------
        ''----- Dichiarazione delle Variabili
        ''------------------------------------------

        'variabili univoche per ciascuna operazione agenda-trappola
        Dim TrappolaCodiceProdotto As Integer = 0
        Dim VegCodTemp As Integer = 0

        ''------------------------------------------
        ''----- Recupero le informazioni
        ''------------------------------------------



        objParametriAgenda.Data = agenda.Movimenti(i).Data
        Master_Operazione.SetNota(agenda.Movimenti(i).Mov_Desc)


        'MOVIMENTO_DETTAGLIO TECNICO (dovrebbe essere nothing)
        If agenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count > 0 Then
            Throw New NotImplementedException
        End If



        '---------------------
        '----MOVIMENTI_DETTAGLI
        '---------------------
        'uno solo per queste operazioni disor e conf sessuale. contiene solo informazioni sul prodotto e il numero di trappole totali
        If agenda.Movimenti(i).Movimenti_Dettagli.Count > 1 Then
            'previsto sulo un mov dettaglio
            Throw New NotImplementedException
        End If

        'controllo corrispondeza piva con agenda
        'sa cod è uguale ma potrebbe cambiare se...
        If agenda.Piva <> agenda.Movimenti(i).Movimenti_Dettagli(0).Piva Then
            Throw New ApplicationException
        End If

        'Movimento_Dettaglio.Elem_Cod = TRAPPOLE
        'implicito ma lo controllo dato che lo riscrivo
        If TRAPPOLE <> agenda.Movimenti(i).Movimenti_Dettagli(0).Elem_Cod Then
            Throw New ApplicationException
        End If

        ' Movimento_Dettaglio.Pro_Cod = cmb_Trappola.SelectedItem.Value
        TrappolaCodiceProdotto = agenda.Movimenti(i).Movimenti_Dettagli(0).Pro_Cod


        'Movimento_Dettaglio.Udm_Cod = unità_misura_num_trappole ' unità di misura n.trappole
        'implicito
        If enum_UnitaMisura.Numero_Trappole <> agenda.Movimenti(i).Movimenti_Dettagli(0).Udm_Cod Then
            Throw New ApplicationException
        End If

        'Movimento_Dettaglio.Contabilizzato = 1
        'implicito
        'Movimento_Dettaglio.Pendente = 3
        'implicito
        'Movimento_Dettaglio.Data = Data
        'Movimento_Dettaglio.BaseCode = BaseCode
        'implicito
        'Movimento_Dettaglio.TopCode = TopCode
        'implicito

        '---------------------
        'MOVIMENTO_DESTINAZIONI
        '---------------------
        'una sola destinazione per ciscun appezzamento coinvolto nell'operazione
        'a prescindere che ci sia o no la trappola
        Dim AppezzamentoCorrente As Integer = 0

        If IsNothing(agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni) Then
            Throw New ApplicationException
        End If

        Dim sommaTrappoleDestinazioni As Integer = 0
        For index = 0 To agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni.Count - 1

            sommaTrappoleDestinazioni = sommaTrappoleDestinazioni + CInt(agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni(index).Qta)

            If index = 0 Then
                Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                Dim Dt_Imp As New DataTable
                Dt_Imp = objImp.Leggi(agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni(index).Piva,
                             agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni(index).Sa_Cod,
                             agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni(index).Appezza,
                             agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni(index).Id_Destinazione,
                             enumSelezioneVariabile.Selezione_JoinDescrizioni,
                             "", "", objParametri_Server)
                If Dt_Imp.Rows.Count > 0 Then

                    VegCodTemp = CInt(Dt_Imp.Rows(0).Item("veg_cod"))

                End If
            Else
                ''tralascio il controllo se le specie cambiano
                'Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                'Dim Dt_Imp As New DataTable
                'Dt_Imp = objImp.Leggi(agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni(index).Piva, _
                '             agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni(index).Sa_Cod, _
                '             agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni(index).Appezza, _
                '             agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni(index).Id_Destinazione, _
                '             enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                '             "", "", objParametri_Server)
                'If Dt_Imp.Rows.Count > 0 Then
                '    If VegCodTemp <> CInt(Dt_Imp.Rows(0).Item("veg_cod")) Then
                '        Throw New NotImplementedException
                '    End If
                'End If
            End If

            Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

            objAppezzamento.Piva = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni(index).Piva
            objAppezzamento.Sa_Cod = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni(index).Sa_Cod
            objAppezzamento.Appezza = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni(index).Appezza
            objAppezzamento.ID_Reg = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni(index).Id_Destinazione
            objAppezzamento.Qta = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni(index).Qta
            objAppezzamento.Qta2 = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Destinazioni(index).Qta2

            'appezzamenti usati dalla master per check nella tabella
            objParametriAgenda.Impianti.Add(objAppezzamento)
            objParametriAgenda.salva()

        Next

        If sommaTrappoleDestinazioni <> CInt(agenda.Movimenti(i).Movimenti_Dettagli(0).Qta) Then
            'cosa faccio se non corrispondono?
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IncoerenzaNelNumeroDelleTrappoleInstallate, Page, , )
            'Throw New NotImplementedException
        End If

        '---------------------
        'MOVIMENTI_DETTAGLI_TECNICI
        '---------------------
        'Un mov det tecnico per tutte le trappole
        If agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici.Count > 1 Then
            'previsto sulo un mov dettaglio
            Throw New NotImplementedException
        End If
        'Movimento_Dettaglio_Tecnico.Trap_num = 0 sempre
        Dim TrappolaNumeroID As Integer = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).Trap_num
        'Movimento_Dettaglio_Tecnico.Freatimetro = 0 sempre 


        '---------Inserisco i dati in objParametriAgenda ------------



        'variabile per installazione trappole
        Dim InstallazioneTrap As New ParametriInstallazioneTrappole
        'aggiungo le variabili all'oggetto
        InstallazioneTrap.NumeroTrappole = agenda.Movimenti(i).Movimenti_Dettagli(0).Qta
        InstallazioneTrap.CodiceProdotto = TrappolaCodiceProdotto
        InstallazioneTrap.CodiceDitta = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).Ditta_cod
        InstallazioneTrap.Freatimetro = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).Freatimetro
        InstallazioneTrap.SiglaAv = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).Sigla_av
        InstallazioneTrap.AvCod = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).Av_Cod
        InstallazioneTrap.NumeroInneschi = agenda.Movimenti(i).Movimenti_Dettagli(0).Movimenti_Dettagli_Tecnici(0).Dose
        'lista degli appezzamenti coinvolti nell'operazione agenda
        'InstallazioneTrap.Appezzamenti_X_NumTrappole = AppezzamentiCoinvolti_NumTrappole
        'elenco trappele (numero-nome personalizzato)
        'InstallazioneTrap.Trappole_X_Nome = TrappoleLista
        'Coppia trappola - appezzamento che la contiene
        'InstallazioneTrap.Trappole_X_Appezzamento = Trappole_Appezzamenti
        'Coppia innesco - trappola che la contiene che la contiene
        'InstallazioneTrap.Trappole_X_NumInneschi = Inneschi_Trappole


        objParametriAgenda.InstallazioneTrappola = InstallazioneTrap
        objParametriAgenda.Veg_Cod = VegCodTemp

    End Sub


    Private Sub LeggiMovimentoAgenda_ScaricoTrappole(ByRef agenda As Operazione_Agenda, ByRef i As Integer, ByRef j As Integer)
        Dim Destinazione As Integer
        Dim SaCodDestinazione As Integer
        ''Movimento_Dettaglio_Scarico_Trappola.Data = Data
        'Movimento_Dettaglio_Scarico_Trappola.Elem_Cod = TRAPPOLE
        'Movimento_Dettaglio_Scarico_Trappola.Pro_Cod = cmb_Trappola.SelectedItem.Value 'Dispenser_Cod
        ''Movimento_Dettaglio_Scarico_Trappola.Mov_det_des = "Scarico di Trappole"
        ''Movimento_Dettaglio_Scarico_Trappola.Udm_Cod = enum_UnitaDiMisura.Numero_Trappole
        'Movimento_Dettaglio_Scarico_Trappola.Qta = CInt(Txt_NumeroTrappole.Text)
        ''Movimento_Dettaglio_Scarico_Trappola.Contabilizzato = Movimento_Dettaglio_Contabilizzato_Magazzino
        ''Movimento_Dettaglio_Scarico_Trappola.Pendente = Movimento_Dettaglio_Pendente_Magazzino
        'Movimento_Dettaglio_Scarico_Trappola.Lav_Cod = Lav_Cod
        'Movimento_Dettaglio_Scarico_Trappola.Cau_Mov = CAU_SCARICO

        If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
            For x = 0 To agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1
                ''Destinazione = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione
                ''SaCodDestinazione = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod
                ''objParametriAgenda.Fabbricato = Destinazione.ToString & "|" & SaCodDestinazione.ToString
                ''Movimento_Destinazione_Scarico_Trappola.Data = Data
                'Movimento_Destinazione_Scarico_Trappola.Id_Destinazione = Split(objParametriAgenda.Fabbricato, "|")(0)
                ''Movimento_Destinazione_Scarico_Trappola.Tipo = MAGAZZINO
                'Movimento_Destinazione_Scarico_Trappola.Qta = CInt(Txt_NumeroTrappole.Text)

                Destinazione = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione
                SaCodDestinazione = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod
                objParametriAgenda.Fabbricato = Destinazione.ToString & "|" & SaCodDestinazione.ToString & "|" & agenda.Piva
            Next
        End If
    End Sub


    Private Sub LeggiMovimentoAgenda_ScaricoInneschi(ByRef agenda As Operazione_Agenda, ByRef i As Integer, ByRef j As Integer)
        Dim Destinazione As Integer
        Dim SaCodDestinazione As Integer
        'Movimento_Dettaglio_Scarico_Innesco.Elem_Cod = INNESCHI
        'Movimento_Dettaglio_Scarico_Innesco.Pro_Cod = Me.cmb_Avversita.SelectedItem.Value
        ''Movimento_Dettaglio_Scarico_Innesco.Mov_det_des = "Scarico di Inneschi Installazione Trappole"
        'Movimento_Dettaglio_Scarico_Innesco.Udm_Cod = enum_UnitaDiMisura.Numero_Inneschi
        'Movimento_Dettaglio_Scarico_Innesco.Qta = inneschiNumTotale
        ''Movimento_Dettaglio_Scarico_Innesco.Contabilizzato = 1
        ''Movimento_Dettaglio_Scarico_Innesco.Pendente = 4
        'Movimento_Dettaglio_Scarico_Innesco.Lav_Cod = Lav_Cod
        'Movimento_Dettaglio_Scarico_Innesco.Cau_Mov = CAU_SCARICO

        If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
            For x = 0 To agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1
                ''Movimento_Destinazione_Scarico_Trappola.Data = Data
                ''Movimento_Destinazione_Scarico_Innesco.Piva = objParametriAgenda.Piva
                ''Movimento_Destinazione_Scarico_Innesco.Sa_Cod = Split(objParametriAgenda.Fabbricato, "|")(1)
                ''Movimento_Destinazione_Scarico_Innesco.Id_Destinazione = Split(objParametriAgenda.Fabbricato, "|")(0)
                ''Movimento_Destinazione_Scarico_Innesco.Tipo = MAGAZZINO
                ''Movimento_Destinazione_Scarico_Innesco.Qta = inneschiNumTotale
                ''Movimento_Destinazione_Scarico_Innesco.BaseCode = BaseCode
                ''Movimento_Destinazione_Scarico_Innesco.TopCode = TopCode

                Destinazione = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione
                SaCodDestinazione = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod
                objParametriAgenda.Fabbricato = Destinazione.ToString & "|" & SaCodDestinazione.ToString & "|" & agenda.Piva
            Next
        End If

    End Sub


    Private Sub ControlloCoerenzaNumeroTrappole(ByRef Movimento_Dett As Movimento_Dettaglio)
        'il numero dei dettagli deve essere uguale al numero si trappole (qta in destinazione corrente)
        'se l'operazione è stata fatta con l'agenda vecchia Movimenti_Destinazioni(x).Qta =0
        'dato che non era gestito il multi impianto
        'quindi se qta=0 e dettecnoci>0 sorvolo dato che è un aoperazione gestita con agenda vecchia,
        'altrimenti genero eccezione dato che non dovrebb e succedere.
        'a meno che non si utilizzi agenda vecchia per salvare operazioni nuove...
        If Movimento_Dett.Movimenti_Destinazioni(0).Qta = 0 AndAlso
            Movimento_Dett.Movimenti_Dettagli_Tecnici.Count > 0 _
            Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SiÈApertaUnaOperazioneCreataConLAgendaPrec, Page, , )
        ElseIf Movimento_Dett.Movimenti_Destinazioni(0).Qta <> 0 AndAlso
            Movimento_Dett.Movimenti_Dettagli_Tecnici.Count <> Movimento_Dett.Movimenti_Destinazioni(0).Qta _
            Then
            'qta è stato impostato, quindi dovrebbe essere una operazione da agenda nuova,
            'però il numero di dettagli non corrisponde
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IncoerenzaNeiDati, Page, , )
            Throw New ApplicationException
        Else
            'ok
        End If
    End Sub


    Private Sub ImpostaValoriSelezionatiCombo()
        If IsNothing(objParametriAgenda.InstallazioneTrappola) Then
            Exit Sub
        End If

        'dispenser
        Dim TrappolaCodiceProdotto As String = ""
        Try

            If IsNothing(objParametriAgenda.InstallazioneTrappola.CodiceProdotto) OrElse
                objParametriAgenda.InstallazioneTrappola.CodiceProdotto = 0 Then
                cmb_Trappola.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.ProdottoNonTrovato, ""))
                cmb_Trappola.SelectedValue = "-1"
            Else
                TrappolaCodiceProdotto = objParametriAgenda.InstallazioneTrappola.CodiceProdotto
                cmb_Trappola.SelectedValue = TrappolaCodiceProdotto
                If cmb_Trappola.SelectedValue <> TrappolaCodiceProdotto Then
                    cmb_Trappola.SelectedValue = ""
                End If
            End If
        Catch ex As Exception
            cmb_Trappola.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.ProdottoNonTrovato, ""))
            cmb_Trappola.SelectedValue = "-1"
        End Try
        'richiamo metodo che genra evento, così mi imposta i valori giacenze
        cmb_Trappola_SelectedIndexChanged(Nothing, Nothing)

        'ditta fornitrice
        Dim TrappolaCodiceDittaTrappola As String = ""
        'Try
        '    TrappolaCodiceDittaTrappola = objParametriAgenda.InstallazioneTrappola.CodiceDitta
        '    cmb_Ditte.SelectedValue = TrappolaCodiceDittaTrappola
        'Catch ex As Exception
        '    cmb_Ditte.SelectedValue = Nothing
        '    cmb_Ditte.Items.Add(New ListItem("Ditta non trovata", TrappolaCodiceDittaTrappola))
        '    cmb_Ditte.SelectedValue = objParametriAgenda.InstallazioneTrappola.AvCod
        'End Try

        Try
            '    If IsNothing(objParametriAgenda.InstallazioneTrappola.CodiceDitta) Or
            'objParametriAgenda.InstallazioneTrappola.CodiceDitta = 0 Then
            '        cmb_Ditte.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DittaNonTrovata, ""))
            '        cmb_Ditte.SelectedValue = "-1"
            '    Else
            TrappolaCodiceDittaTrappola = objParametriAgenda.InstallazioneTrappola.CodiceDitta
            cmb_Ditte.SelectedValue = TrappolaCodiceDittaTrappola
            If cmb_Ditte.SelectedValue <> TrappolaCodiceDittaTrappola Then
                cmb_Ditte.SelectedValue = ""
            End If
            'End If
        Catch ex As Exception
            'cmb_Ditte.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.DittaNonTrovata, ""))
            cmb_Ditte.SelectedValue = "-1"
        End Try


        'avversita
        Dim TrappolaAvCod As String = ""
        'Try
        '    TrappolaAvCod = objParametriAgenda.InstallazioneTrappola.AvCod
        '    cmb_Avversita.SelectedValue = TrappolaAvCod
        'Catch ex As Exception
        '    cmb_Avversita.SelectedValue = Nothing
        '    cmb_Avversita.Items.Add(New ListItem("Avvestità non trovata", TrappolaAvCod))
        '    cmb_Avversita.SelectedValue = TrappolaAvCod
        'End Try

        Try

            If IsNothing(objParametriAgenda.InstallazioneTrappola.AvCod) OrElse
                objParametriAgenda.InstallazioneTrappola.AvCod = 0 Then
                cmb_Avversita.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.AvvestitàNonTrovata, ""))
                cmb_Avversita.SelectedValue = "-1"
            Else
                TrappolaAvCod = objParametriAgenda.InstallazioneTrappola.AvCod
                cmb_Avversita.SelectedValue = TrappolaAvCod
                If cmb_Avversita.SelectedValue <> TrappolaAvCod Then
                    cmb_Avversita.SelectedValue = ""
                End If
            End If
        Catch ex As Exception
            cmb_Avversita.Items.Add(New ListItem(Resources.AgronicaAgenda_2010.AvvestitàNonTrovata, ""))
            cmb_Avversita.SelectedValue = "-1"
        End Try

        cmb_Avversita_SelectedIndexChanged(Nothing, Nothing)

        'ViewState("dispenserSelezionato") = ""
        'ViewState("dittaSelezionata") = ""
        'ViewState("avversitaSelezionata") = ""

    End Sub


    Private Sub Cmb_Trappola_Carica()
        Dim aggiorna As Boolean = False
        Dim Veg_Cod_Val As String
        Veg_Cod_Val = Master_Operazione.Property_ComboSpecie.ddl_Specie.SelectedValue

        If Veg_Cod_Val <> "" Then
            If IsNothing(ViewState("VegCod_OLD")) Then
                aggiorna = True
            Else
                If ViewState("VegCod_OLD") <> Veg_Cod_Val Then
                    aggiorna = True
                End If
            End If
        End If

        Dim Fabbricato_Val As String
        Fabbricato_Val = Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.SelectedValue

        If IsNothing(ViewState("Magazzino")) Then
            aggiorna = True
        Else
            If ViewState("Magazzino") <> Fabbricato_Val Then
                aggiorna = True
            End If
        End If


        If aggiorna Then
            ViewState("Magazzino") = Fabbricato_Val

            objParametriAgenda.Fabbricato = Fabbricato_Val

            ViewState("VegCod_OLD") = Veg_Cod_Val
            objParametriAgenda.Veg_Cod = Veg_Cod_Val

            If ViewState("Magazzino") = "0" Or ViewState("Magazzino") = "" Then

                CaricaListControl.TrappolexSpecieVegetalixAvversita(CType(Me.cmb_Trappola, ListControl),
                                                                    True, "", "0",
                                                                    objParametriAgenda.Veg_Cod.Split("/")(0),
                                                                    0,
                                                                    objParametriAgenda.TrappolaUso,
                                                                    "", "", objParametri_Server)
                'Inserisco una riga vuota in testa alla combo delle trappole
                'Me.cmb_Trappola.Items.Insert(0, "")
            Else


                Dim Fabbricato_Cod As String = Split(Me.objParametriAgenda.Fabbricato, "|")(0)
                Dim Sa_Cod As Integer = Split(Me.objParametriAgenda.Fabbricato, "|")(1)
                Dim piva As String = Split(Me.objParametriAgenda.Fabbricato, "|")(2)

                AgronicaCoreUtility.CaricaListControl.TrappoleMagazzino(CType(Me.cmb_Trappola, ListControl),
                                                                        True, "", "0",
                                                                        enum_Agenda_Causali.SCARICO,
                                                                        piva,
                                                                        Sa_Cod,
                                                                        Fabbricato_Cod,
                                                                        objParametriAgenda.TrappolaUso,
                                                                        objParametriAgenda.Veg_Cod.Split("/")(0), 0,
                                                                        "", "", objParametri_Server, objParametri_Utenti)
            End If



            Me.cmb_Trappola.SelectedIndex = 0
            Me.cmb_Ditte.Items.Clear()
            Me.cmb_Avversita.Items.Clear()
            Me.Txt_CodAvversita.Text = ""
            Me.Txt_DataScadenz.Text = ""
            Me.Txt_Giacenza.Text = ""
            Me.Txt_GiacenzaInneschi.Text = ""
            Me.Txt_GiorniFeromone.Text = ""


        End If



        ImpostaValoriSelezionatiCombo()


    End Sub


    'Per la generazione della tabella con impianti e trappole
    Private Function GeneraTabellaHtmlTrappole(ByVal genera_DaMaster As Boolean) As Boolean
        InizializzaTabellaTrappole()

        If genera_DaMaster Then

            If Not IsNumeric(Txt_NumeroTrappole.Text) Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IndicareIlNumeroDiTrappole, Page, , UpdatePanelGridImpianti)
                Return False
            End If

            If Not IsNumeric(Txt_NumeroTrappole.Text) Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IndicareIlNumeroDiTrappole, Page, , UpdatePanelGridImpianti)
                Return False
            End If

            If CInt(Txt_NumeroTrappole.Text) < 1 Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IndicareIlNumeroDiTrappole, Page, , UpdatePanelGridImpianti)
                Return False
            End If

            If cmb_Trappola.SelectedItem.Value = "" OrElse cmb_Trappola.SelectedItem.Value = "0" OrElse cmb_Trappola.SelectedItem.Text = "" Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IndicareLaTrappola, Page, , UpdatePanelGridImpianti)
                Return False
            End If

            If cmb_Avversita.SelectedItem.Value = "" OrElse cmb_Avversita.SelectedItem.Value = "0" OrElse cmb_Avversita.SelectedItem.Text = "" Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IndicareLAvversità, Page, , UpdatePanelGridImpianti)
                Return False
            End If

            If Txt_CodAvversita.Text = "" Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnaAvversità, Page, , UpdatePanelGridImpianti)
                Return False
            End If

            'If cmb_Ditte.SelectedItem.Value = "" OrElse cmb_Ditte.SelectedItem.Value = "0" OrElse cmb_Ditte.SelectedItem.Text = "" Then
            '    Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IndicareLaDitta, Page, , UpdatePanelGridImpianti)
            '    Return False
            'End If

        End If



        'controllo la giacenza trappole
        'trappole controllate solo in salva
        Dim messaggio_temp As String = ""
        If genera_DaMaster AndAlso (objParametriAgenda.Fabbricato <> "" AndAlso objParametriAgenda.Fabbricato <> "0") AndAlso
          Not controllaGiacenza_Trappole_Inneschi(CInt(Me.Txt_NumeroTrappole.Text), 0, messaggio_temp) Then
            Me.Txt_NumeroTrappole.BorderColor = Drawing.Color.DeepPink
            Me.Txt_Giacenza.BorderColor = Drawing.Color.DeepPink
            'Return False
        Else
            'Me.Txt_NumeroTrappole.BorderColor = Drawing.Color.Green
        End If

        Dim N_Righe As Integer = Txt_NumeroTrappole.Text


        Dim ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
        If genera_DaMaster Then
            'genero tabella da impianti master
            ListaImpianti = CType(Master, Operazione).GetImpianti()
        Else
            'genero tabella da oggettto parametri agenda, quando sono in modifica e devo generare
            'la tabella prima che venga chiamata la load della master
            ListaImpianti = objParametriAgenda.Impianti
        End If


        Dim N_Colonne As Integer = ListaImpianti.Count
        If N_Colonne = 0 Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareAlmenoUnImpiantoColturale, Page, , UpdatePanelGridImpianti)
            Return False
        End If
        If N_Righe = 0 Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IndicareIlNumeroDiTrappole, Page, , UpdatePanelGridImpianti)
            Return False
        End If

        Dim trow As New TableRow

        'intestazione'colonne
        trow.CssClass = "ui-widget-header"
        Dim tcell As New TableCell
        tcell.Controls.Add(New LiteralControl("Trappola"))
        'tcell.CssClass = "ui-widget-header"
        tcell.Attributes.Add("TipoGruppo", "TestataColonna")
        tcell.Attributes.Add("TipoCella", "CodiceTrappola")
        trow.Cells.Add(tcell)

        tcell = New TableCell
        tcell.Controls.Add(New LiteralControl("Codice Interno"))
        ' tcell.CssClass = "ui-widget-header"
        tcell.Attributes.Add("TipoGruppo", "TestataColonna")
        tcell.Attributes.Add("TipoCella", "CodiceInternoTrappola")
        trow.Cells.Add(tcell)


        tcell = New TableCell
        tcell.Controls.Add(New LiteralControl("Inneschi"))
        'tcell.CssClass = "ui-widget-header"
        tcell.Attributes.Add("TipoGruppo", "TestataColonna")
        tcell.Attributes.Add("TipoCella", "NumeroInneschi")
        trow.Cells.Add(tcell)

        Dim i As Integer
        For i = 0 To N_Colonne - 1
            tcell = New TableCell
            Dim objAnagrafe As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            Dim nomeApp As String = objAnagrafe.AppezzamentoNome_from_Appezza(ListaImpianti(i).Piva,
                                                                             ListaImpianti(i).Sa_Cod,
                                                                             ListaImpianti(i).Appezza,
                                                                             objParametri_Server)
            Dim objAnagrafe2 As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim nomeCentro As String = objAnagrafe2.SaNome_from_SaCod(ListaImpianti(i).Piva, ListaImpianti(i).Sa_Cod, objParametri_Server)
            tcell.Controls.Add(New LiteralControl("" & nomeCentro & " </br> " & nomeApp))
            'tcell.CssClass = "ui-widget-header"
            tcell.Attributes.Add("TipoGruppo", "TestataColonna")
            tcell.Attributes.Add("TipoCella", "TestataAppezzamento")
            tcell.Attributes.Add("IdAzienda", CStr(ListaImpianti(i).Piva))
            tcell.Attributes.Add("IdCentro", CStr(ListaImpianti(i).Sa_Cod))
            tcell.Attributes.Add("IdAppezzamento", CStr(ListaImpianti(i).Appezza))
            tcell.Attributes.Add("IdReg", CStr(ListaImpianti(i).ID_Reg))
            trow.Cells.Add(tcell)
        Next
        TabellaTrappole.Rows.Add(trow)

        If genera_DaMaster Then
            'genero tabella da impianti master

            'Calcolo l'ultimo valore usato per la trappola
            Dim ObjReinn As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
            Dim NTrappola As Integer = ObjReinn.RilevaNumeroTrappola(CStr(ListaImpianti(0).Piva), CStr(ListaImpianti(0).Sa_Cod), CStr(Me.Txt_CodAvversita.Text), objParametri_Server)

            Dim trow2 As New TableRow
            'Aggiungo le Righe
            For j = 0 To N_Righe - 1
                trow2 = New TableRow

                '(j + 1)
                Dim IdTrappola As String = CStr(NTrappola + j)

                tcell = New TableCell
                tcell.Controls.Add(New LiteralControl(CStr(Me.Txt_CodAvversita.Text) & ": " & IdTrappola))
                tcell.Attributes.Add("TipoGruppo", "Dato")
                tcell.Attributes.Add("TipoCella", "CodiceTrappola")
                tcell.Attributes.Add("IdTrappola", IdTrappola)
                trow2.Cells.Add(tcell)

                tcell = New TableCell
                Dim txt As New TextBox
                txt.ID = "TXTCodicePersonalizzato#" & IdTrappola
                txt.CssClass = "txtUI"
                txt.Style.Add("width", "100px")
                txt.MaxLength = 10
                tcell.Controls.Add(txt)
                tcell.Attributes.Add("TipoGruppo", "Dato")
                tcell.Attributes.Add("TipoCella", "CodiceInternoTrappola")
                tcell.Attributes.Add("IdTrappola", IdTrappola)
                trow2.Cells.Add(tcell)

                tcell = New TableCell
                txt = New TextBox
                txt.CssClass = "txtUI"
                txt.Style.Add("width", "100px")
                txt.ID = "TXTInneschi#" & CStr(j + 1)
                txt.Text = "1"
                txt.MaxLength = 2
                tcell.Controls.Add(txt)
                tcell.Attributes.Add("TipoGruppo", "Dato")
                tcell.Attributes.Add("TipoCella", "NumeroInneschi")
                tcell.Attributes.Add("IdTrappola", IdTrappola)
                trow2.Cells.Add(tcell)


                For i = 0 To N_Colonne - 1

                    tcell = New TableCell
                    Dim chk = New RadioButton
                    chk.GroupName = "grupporiga" & j
                    chk.CssClass = "CHKAssocia"
                    chk.ID = "TCHK#" & j & "#" & i
                    If i = 0 Then
                        chk.Checked = True
                    End If

                    tcell.Controls.Add(chk)
                    tcell.Attributes.Add("TipoGruppo", "Dato")
                    tcell.Attributes.Add("TipoCella", "NumeroInneschi")
                    tcell.Attributes.Add("IdTrappola", IdTrappola)
                    tcell.Attributes.Add("IdAzienda", CStr(ListaImpianti(i).Piva))
                    tcell.Attributes.Add("IdCentro", CStr(ListaImpianti(i).Sa_Cod))
                    tcell.Attributes.Add("IdAppezzamento", CStr(ListaImpianti(i).Appezza))
                    tcell.Attributes.Add("IdReg", CStr(ListaImpianti(i).ID_Reg))
                    trow2.Cells.Add(tcell)
                Next

                TabellaTrappole.Rows.Add(trow2)
            Next



        Else
            'genero tabella da oggettto parametri agenda

            Dim trow2 As New TableRow
            'Aggiungo le Righe
            Dim j As Integer = 0 'conbtatore righe, per grupporiga checkbox
            For Each obj As DictionaryEntry In objParametriAgenda.InstallazioneTrappola.Trappole_X_Nome
                trow2 = New TableRow

                tcell = New TableCell
                tcell.Controls.Add(New LiteralControl(CStr(Me.Txt_CodAvversita.Text) & ": " & CStr(obj.Key)))
                tcell.Attributes.Add("TipoGruppo", "Dato")
                tcell.Attributes.Add("TipoCella", "CodiceTrappola")
                tcell.Attributes.Add("IdTrappola", CStr(obj.Key))
                trow2.Cells.Add(tcell)

                tcell = New TableCell
                Dim txt As New TextBox
                txt.ID = "TXTCodicePersonalizzato#" & CStr(obj.Key)
                txt.CssClass = "txtUI"
                txt.Style.Add("width", "100px")
                txt.Text = obj.Value
                txt.MaxLength = System.Math.Max(10, txt.Text.Length)
                tcell.Controls.Add(txt)
                tcell.Attributes.Add("TipoGruppo", "Dato")
                tcell.Attributes.Add("TipoCella", "CodiceInternoTrappola")
                tcell.Attributes.Add("IdTrappola", CStr(obj.Key))
                trow2.Cells.Add(tcell)

                tcell = New TableCell
                txt = New TextBox
                txt.CssClass = "txtUI"
                txt.Style.Add("width", "100px")
                txt.ID = "TXTInneschi#" & CStr(obj.Key)
                txt.Text = CStr(objParametriAgenda.InstallazioneTrappola.Trappole_X_NumInneschi(obj.Key))
                txt.MaxLength = System.Math.Max(2, txt.Text.Length)
                tcell.Controls.Add(txt)
                tcell.Attributes.Add("TipoGruppo", "Dato")
                tcell.Attributes.Add("TipoCella", "NumeroInneschi")
                tcell.Attributes.Add("IdTrappola", CStr(obj.Key))
                trow2.Cells.Add(tcell)

                For i = 0 To N_Colonne - 1
                    'tcell = New TableCell
                    'Dim chk = New CheckBox
                    'chk.CssClass = "CHKAssocia"
                    'chk.ID = "TCHK#" & j & "#" & i
                    'If i = 0 Then
                    '    chk.Checked = True
                    'End If
                    'tcell.Controls.Add(chk)
                    'trow2.Cells.Add(tcell)

                    tcell = New TableCell
                    Dim chk = New RadioButton
                    chk.GroupName = "grupporiga" & j
                    chk.CssClass = "CHKAssocia"
                    chk.ID = "TCHK#" & j & "#" & i
                    Dim appezzamento As String = CStr(ListaImpianti(i).Appezza)
                    If CStr(objParametriAgenda.InstallazioneTrappola.Trappole_X_Appezzamento(obj.Key)) = appezzamento Then
                        chk.Checked = True
                    End If
                    tcell.Controls.Add(chk)
                    tcell.Attributes.Add("TipoGruppo", "Dato")
                    tcell.Attributes.Add("TipoCella", "NumeroInneschi")
                    tcell.Attributes.Add("IdTrappola", CStr(obj.Key))
                    tcell.Attributes.Add("IdAzienda", CStr(ListaImpianti(i).Piva))
                    tcell.Attributes.Add("IdCentro", CStr(ListaImpianti(i).Sa_Cod))
                    tcell.Attributes.Add("IdAppezzamento", CStr(ListaImpianti(i).Appezza))
                    tcell.Attributes.Add("IdReg", CStr(ListaImpianti(i).ID_Reg))

                    trow2.Cells.Add(tcell)
                Next

                TabellaTrappole.Rows.Add(trow2)
                j += 1
            Next

        End If

        Session("Tabella") = TabellaTrappole
        PlaceTabella.Controls.Add(TabellaTrappole)
        Return True

    End Function


    Private Sub InizializzaTabellaTrappole()
        TabellaTrappole = New Table
        TabellaTrappole.ID = "TabellaTrappole"
        TabellaTrappole.CssClass = "ui-widget-content"
        TabellaTrappole.CellPadding = 5
        TabellaTrappole.CellSpacing = 0
        TabellaTrappole.ClientIDMode = UI.ClientIDMode.Static
        TabellaTrappole.BorderWidth = 1
        TabellaTrappole.Style.Add("width", "100%")
    End Sub


    Protected Sub ImgBtn_Trappole_Inserisci_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_DoseInserisci.Click

    End Sub

    'Private Sub ImgBtn_TrappoleMostra_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_TrappoleMostra.Click

    'End Sub

    Private Sub ImageButton_MostraTrappole_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImageButton_MostraTrappole.Click
        blocca()
    End Sub

    Private Sub blocca()

        '---------------------------------------
        ' recupero il CENTRO
        If (objParametriAgenda.Sa_Cod = "" OrElse objParametriAgenda.Sa_Cod = "0") AndAlso Not objParametriAgenda.OperazioneMulticentro Then
            Dim messaggio_errore As String = Resources.AgronicaAgenda_2010.SelezionareUnCentroAziendale
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneBr & messaggio_errore, Page, ,
      Master_Operazione.Property_UpdatePanelToolBar)
            Exit Sub
        End If

        If GeneraTabellaHtmlTrappole(True) Then

            TabellaTrappole.Visible = True

            Me.ImageButton_MostraTrappole.Enabled = False
            Me.ImageButton_MostraTrappole.Visible = False
            Me.ImageButton_Sblocca.Enabled = True
            Me.ImageButton_Sblocca.Visible = True

            cmb_Avversita.Enabled = False
            cmb_Ditte.Enabled = False
            cmb_Trappola.Enabled = False
            Txt_CodAvversita.Enabled = False
            Txt_DataScadenz.Enabled = True
            Txt_Giacenza.Enabled = False
            Txt_GiacenzaInneschi.Enabled = False
            Txt_GiorniFeromone.Enabled = False
            Txt_NumeroTrappole.Enabled = False

            Master_Operazione.Property_Box_Salva.Enabled = True 'true per modifica
            Master_Operazione.Property_Box_Salva.Visible = True 'true per modifica
            Master_Operazione.Property_ImgBtn_Salva.Enabled = True 'true per modifica
            Master_Operazione.Property_RBL_Salva.Enabled = True 'true per modifica

            Master_Operazione.Property_txt_DataOperazione.Enabled = False

            'Master_Operazione.Property_ImgBtn_Costi.Enabled = False

            Master_Operazione.Property_txt_Note.Enabled = True 'true per modifica

            Master_Operazione.Property_ComboSpecie.Enabled = False
            Master_Operazione.Property_BTN_ComboSpecie.Enabled = False

            Master_Operazione.Property_BTN_Magazzini.Enabled = False
            Master_Operazione.Property_ComboMagazzini.Enabled = False

            Master_Operazione.Property_BTN_CentroAziendale.Enabled = False
            Master_Operazione.Property_ComboCentroAziendale.Enabled = False

            Master_Operazione.Property_BTN_ComboOperazione.Enabled = False
            Master_Operazione.Property_ComboOperazione.Enabled = False

            Master_Operazione.Property_CBL_Consigli.Enabled = True
            Master_Operazione.Property_GridView_Impianti.Enabled = False

            BloccaComboJavaScript(True, True, True, True)


        Else

        End If
    End Sub

    Private Sub BloccaComboJavaScript(ByVal centri As Boolean, ByVal operazioni As Boolean, ByVal specie As Boolean, ByVal magazzini As Boolean)
        'blocco via javascript pulsanti combo
        Dim script As New StringBuilder
        script.AppendLine("$(document).ready(function () { ")
        script.AppendLine("setTimeout('eseguiScr()',500); ")
        script.AppendLine("}); ")


        script.AppendLine("function eseguiScr(){ ")
        If centri Then
            script.AppendLine("     BloccaCombo_CentroAziendale();")
        End If
        If operazioni Then
            script.AppendLine("     BloccaCombo_Operazioni();")
        End If
        If specie Then
            script.AppendLine("     BloccaCombo_Specie();")
        End If
        If magazzini Then
            script.AppendLine("     BloccaCombo_Magazzini();")
        End If

        script.AppendLine("} ")



        ScriptManager.RegisterStartupScript(Master_Operazione.Property_UpdatePanelPerScript,
                                Master_Operazione.Property_UpdatePanelPerScript.GetType(),
                                "jQuery_{0}", script.ToString, True)


    End Sub

    Private Sub ImageButton_Sblocca_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImageButton_Sblocca.Click
        sblocca()
    End Sub

    Private Sub sblocca()
        If Not IsNothing(TabellaTrappole) Then
            TabellaTrappole.Visible = False
            TabellaTrappole.Dispose()
        End If
        TabellaTrappole = Nothing
        Session("Tabella") = Nothing

        Me.Txt_NumeroTrappole.BorderColor = Drawing.Color.FromArgb(141, 185, 219)
        Me.Txt_Giacenza.BorderColor = Drawing.Color.FromArgb(141, 185, 219)

        Me.ImageButton_MostraTrappole.Enabled = True
        Me.ImageButton_MostraTrappole.Visible = True
        Me.ImageButton_Sblocca.Enabled = False
        Me.ImageButton_Sblocca.Visible = False

        cmb_Avversita.Enabled = True
        cmb_Ditte.Enabled = True
        cmb_Trappola.Enabled = True
        Txt_CodAvversita.Enabled = True
        Txt_DataScadenz.Enabled = True
        Txt_Giacenza.Enabled = True
        Txt_GiacenzaInneschi.Enabled = True
        Txt_GiorniFeromone.Enabled = True
        Txt_NumeroTrappole.Enabled = True

        Master_Operazione.Property_Box_Salva.Enabled = True 'true per modifica
        Master_Operazione.Property_Box_Salva.Visible = True 'true per modifica
        Master_Operazione.Property_ImgBtn_Salva.Enabled = True 'true per modifica
        Master_Operazione.Property_RBL_Salva.Enabled = True 'true per modifica

        Master_Operazione.Property_txt_DataOperazione.Enabled = True

        'Master_Operazione.Property_ImgBtn_Costi.Enabled = True

        Master_Operazione.Property_txt_Note.Enabled = True 'true per modifica

        Master_Operazione.Property_ComboSpecie.Enabled = True
        Master_Operazione.Property_BTN_ComboSpecie.Enabled = True

        Master_Operazione.Property_BTN_Magazzini.Enabled = True
        Master_Operazione.Property_ComboMagazzini.Enabled = True

        Master_Operazione.Property_BTN_CentroAziendale.Enabled = True
        Master_Operazione.Property_ComboCentroAziendale.Enabled = True

        Master_Operazione.Property_BTN_ComboOperazione.Enabled = True
        Master_Operazione.Property_ComboOperazione.Enabled = True

        Master_Operazione.Property_CBL_Consigli.Enabled = True
        Master_Operazione.Property_GridView_Impianti.Enabled = True

    End Sub












    Protected Sub cmb_Trappola_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmb_Trappola.SelectedIndexChanged

        Dim clc = New AgronicaCoreUtility.CaricaListControl
        If cmb_Trappola.SelectedIndex > 0 Then 'Se seleziono un elemento valido dalla combo delle trappole..
            'Me.cmb_Avversita.Enabled = True
            'Me.cmb_Ditte.Enabled = True

            'Carico tutte le avversità che sono combattute dalla trappola scelta per quella specie vegetale
            clc.AvversitaxTrappole(CType(Me.cmb_Avversita, ListControl),
                                                                    True, "", "0",
                                                                    objParametriAgenda.Veg_Cod.Split("/")(0),
                                                                    Me.cmb_Trappola.SelectedItem.Value,
                                                                    objParametriAgenda.TrappolaUso,
                                                                    "", "", objParametri_Server)



            'Carico le ditte della trappola scelta
            clc.DittexTrappole(CType(Me.cmb_Ditte, ListControl),
                                                                True, "", "0",
                                                                CInt(Me.cmb_Trappola.SelectedItem.Value),
                                                                "", "", objParametri_Server)

            'Carico i dati relativi alla durata della trappola

            Dim Giorni As Integer
            Giorni = TrapDur_from_TrapCod2(CInt(Me.cmb_Trappola.SelectedItem.Value), objParametri_Server)
            Me.Txt_GiorniFeromone.Text = Giorni
            aggiornaDataScadenza()

            LeggoLaGiacenzaDelleTrappoleEInneschi()

        Else 'Se seleziono la riga vuota nella combo delle trappole svuoto le combo delle ditte e delle avversità

            Me.cmb_Ditte.Items.Clear()
            Me.cmb_Avversita.Items.Clear()

        End If
    End Sub



    Private Sub LeggoLaGiacenzaDelleTrappoleEInneschi()
        'leggo la giacenza delle trappole e inneschi
        If objParametriAgenda.Fabbricato <> "" AndAlso objParametriAgenda.Fabbricato <> "0" Then
            Dim GiacenzeTrappole As Integer = 0
            Dim GiacenzeInneschi As Integer = 0
            Select Case CInt(objParametriAgenda.Lav_Cod)
                Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                    LeggiGiacenze(GiacenzeTrappole, GiacenzeInneschi, CDate(Master_Operazione.Property_txt_DataOperazione.Text)) ' CDate(objParametriAgenda.Data))
                    Me.Txt_Giacenza.Text = CStr(GiacenzeTrappole)
                    Me.Txt_GiacenzaInneschi.Text = CStr(GiacenzeInneschi)
                Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                    LeggiGiacenze(GiacenzeTrappole, Nothing, CDate(Master_Operazione.Property_txt_DataOperazione.Text)) ' CDate(objParametriAgenda.Data))
                    Me.Txt_Giacenza.Text = CStr(GiacenzeTrappole)
                    Me.Txt_GiacenzaInneschi.Text = ""
                Case Else
                    Throw New NotImplementedException
            End Select

        Else
            Me.Txt_Giacenza.Text = ""
            Me.Txt_GiacenzaInneschi.Text = ""
        End If
    End Sub



    Public Function TrapDur_from_TrapCod2(ByVal TrapCod As Integer,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As Integer

        Dim Dt As DataTable

        Dim objTrappole As New AgronicaCoreMetaSchemaDAL.Trappole_R

        Dt = objTrappole.Leggi(CInt(TrapCod),
                               0, 0, 0,
                               enumSelezioneVariabile.Selezione_JoinDescrizioni,
                               "", "", objParametri)

        objTrappole = Nothing

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then
            Return Dt.Rows(0).Item("Trap_Dur")
        End If

        Dt = Nothing

    End Function


    Public Function Abbreviazione_from_AvCod2(ByVal AvCod As Integer,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As String

        Dim Dt As DataTable

        Dim objAvversita As New AgronicaCoreMetaSchemaDAL.Avversita_R

        Dt = objAvversita.Leggi(CInt(AvCod), "",
                                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                "", "", objParametri)

        objAvversita = Nothing

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then
            Return Dt.Rows(0).Item("Abbreviazione")
        End If

        Dt = Nothing

    End Function


    Protected Sub cmb_Avversita_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmb_Avversita.SelectedIndexChanged

        If Me.cmb_Avversita.SelectedItem.Value = "" OrElse Me.cmb_Avversita.SelectedItem.Value = "0" OrElse Me.cmb_Avversita.SelectedItem.Value = "-1" Then
            Me.Txt_CodAvversita.Text = ""
            Exit Sub
        End If


        Me.Txt_CodAvversita.Text = Abbreviazione_from_AvCod2(CInt(Me.cmb_Avversita.SelectedItem.Value), objParametri_Server)

        'leggo la giacenza delle trappole
        If objParametriAgenda.Fabbricato <> "" AndAlso objParametriAgenda.Fabbricato <> "0" Then
            Dim GiacenzeInneschi As Integer = 0
            LeggiGiacenze(Nothing, GiacenzeInneschi, CDate(objParametriAgenda.Data))
            Me.Txt_GiacenzaInneschi.Text = CStr(GiacenzeInneschi)
        Else
            Me.Txt_GiacenzaInneschi.Text = ""
        End If

    End Sub


    Private Sub LeggiGiacenze(ByRef GiacenzaTrappole As Integer, ByRef GiacenzaInneschi As Integer, ByVal DataControllo As Date)

        Dim Fabbricato_Cod As Integer
        Dim Sa_cod As Integer
        Dim piva As String

        Dim objGiacenze As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

        If Not IsNothing(GiacenzaTrappole) AndAlso objParametriAgenda.Fabbricato <> "" AndAlso objParametriAgenda.Fabbricato <> "0" Then

            Fabbricato_Cod = Split(objParametriAgenda.Fabbricato, "|")(0)
            Sa_cod = Split(objParametriAgenda.Fabbricato, "|")(1)
            piva = Split(objParametriAgenda.Fabbricato, "|")(2)

            'Leggo le giacenze delle TRAPPOLE
            If Not IsNothing(Me.cmb_Trappola.SelectedItem) AndAlso Me.cmb_Trappola.SelectedItem.Text <> "" Then

                GiacenzaTrappole = objGiacenze.Verifica_Giacenze_Con_Magazzino_Esterno(piva,
                    CInt(Sa_cod),
                    CInt(Fabbricato_Cod),
                    CInt(197),
                    CInt(Me.cmb_Trappola.SelectedItem.Value),
                    0,
                    0, 0, LOTTO_NONDEFINITO,
                    0, 0,
                    AGRODATAINIZIO,
                    DataControllo,
                    objParametriAgenda.Piva,
                    objParametriAgenda.Sa_Cod,
                    objParametriAgenda.Id_Agenda,
                    objParametri_Server)

            End If

            'Leggo le giacenze degli INNESCHI
            If Not IsNothing(GiacenzaInneschi) AndAlso Not IsNothing(Me.cmb_Avversita.SelectedItem) AndAlso cmb_Avversita.Items.Count > 0 AndAlso Me.cmb_Avversita.SelectedItem.Text <> "" Then

                GiacenzaInneschi = objGiacenze.Verifica_Giacenze_Con_Magazzino_Esterno(piva,
                    CInt(Sa_cod),
                    CInt(Fabbricato_Cod),
                    CInt(198),
                    CInt(Me.cmb_Avversita.SelectedItem.Value),
                    0,
                    0, 0, LOTTO_NONDEFINITO,
                    0, 0,
                    AGRODATAINIZIO,
                    DataControllo,
                    objParametriAgenda.Piva,
                    objParametriAgenda.Sa_Cod,
                    objParametriAgenda.Id_Agenda,
                    objParametri_Server)

            End If
        End If
    End Sub


    Private Sub RipristinaSessione()
        Trappole_Disposed()
        ripristinaObjParametriAgenda()
    End Sub

    Private Sub ripristinaObjParametriAgenda()
        objParametriAgenda.Svuota_DatiOperazione()
        objParametriAgenda.OperazioneMulticentro = True
    End Sub

    Private Sub Trappole_Disposed()
        'Master
        CType(Page.Master, Operazione).Operazioni_Dispose()

        'Page
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
    End Sub
#End Region

#Region "Creazione oggetto Agenda"

    Private Function CreaOggettoAgenda(ByVal ListaImp As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto), ByVal Sa_Cod As Integer, ByVal NumeroTrappoleCentro As Decimal, ByRef messaggio_errore As String) As Operazione_Agenda

        Dim Agenda As Operazione_Agenda = Nothing
        Dim inneschiNumTotale As Integer = 0 'usato solo per trappole e massa, ignorato per conf e dis sessuale

        '--------------AGENDA------------------
        If Not Crea_Agenda(ListaImp, Sa_Cod, Agenda, messaggio_errore) Then
            Return Nothing
        End If


        '--------------NOTE------------------
        If Not Crea_Agenda_Note(Agenda) Then
            Return Nothing
        End If


        '---------------MOVIMENTI COSTI ACCESSORI--------
        If Not Crea_Agenda_Movimento_CostiAccessori(Agenda) Then
            Return Nothing
        End If


        '------------- MOVIMENTO RILIEVO / TRATTAMENTO---------
        'inneschiNumTotale usato solo per trappole e massa, ignorato per conf e dis sessuale
        Select Case CInt(objParametriAgenda.Lav_Cod)
            Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                If Not Crea_Agenda_Movimento_InstallazioneTrappole_CattureMassa(ListaImp, inneschiNumTotale, Agenda, messaggio_errore) Then
                    Return Nothing
                End If

            Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                If Not Crea_Agenda_Movimento_Confusione_Disorientamento_Sessuale(ListaImp, Agenda, NumeroTrappoleCentro, messaggio_errore) Then
                    Return Nothing
                End If

            Case Else
                Throw New NotImplementedException
        End Select


        '-------------- MOVIMENTO SCARICO---------------------------
        'modifica per multicentro,solo se l'operazione agenda riguarda il magazzino del centro selezionato
        'If Sa_Cod = Mag_Sa_Cod AndAlso objParametriAgenda.Fabbricato <> "0" Then
        If objParametriAgenda.Fabbricato <> "0" Then
            'se è selezionato il magazzino
            'uno scarico totale per le trappole e uno pergli inneschi (solo per massa e trappole)
            'inneschiNumTotale usato solo per trappole e massa, ignorato per conf e dis sessuale
            If Not Crea_Agenda_Movimento_Scarico(inneschiNumTotale, Agenda, NumeroTrappoleCentro, messaggio_errore) Then
                Return Nothing
            End If
        End If

        Return Agenda

    End Function

    Private Function Crea_Agenda(ByRef ListaImp As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto), ByRef Sa_Cod As Integer, ByRef Agenda As Operazione_Agenda, ByRef messaggio_errore As String) As Boolean

        '---------------------------------------
        ' recupero la NOTE
        'Dim strNota As String = CType(Master, Operazione).GetNota()

        '---------------------------------------
        ' recupero la DATA
        Dim Data As Date
        If objParametriAgenda.Data = AGRODATAINIZIO Then
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.IndicareUnaData
            Return False
        Else
            Data = objParametriAgenda.Data
        End If

        '---------------------------------------
        ' recupero la SPECIE
        Dim Veg_Cod As String = ""
        Dim Veg_Des As String = ""
        If objParametriAgenda.Veg_Cod.Split("/")(0) = "-1" OrElse objParametriAgenda.Veg_Cod.Split("/")(0) = "0" Then
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.SelezionareUnaSpecieVegetale
            Return False
        Else
            Veg_Cod = objParametriAgenda.Veg_Cod.Split("/")(0)
            Veg_Des = Master_Operazione.Property_ComboSpecie.Testo_Combo
        End If


        '---------------------------------------
        ' recupero la OPERAZIONE
        'Dim Lav_Cod As String = ""
        Dim Lav_Des As String = ""
        If objParametriAgenda.Lav_Cod = "" Then
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.SelezionareUnOperazione
            Return False
        Else
            'Lav_Cod = objParametriAgenda.Lav_Cod
            Lav_Des = Master_Operazione.Property_ComboOperazione.Testo_Combo
            objParametriAgenda.Lav_Des = Lav_Des
            'Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
            'Lav_Des = objOperazioniLeggi.LavorazioneDes_from_LavorazioneCod(objParametriAgenda.Lav_Cod, objParametri_Server)
            'objParametriAgenda.Lav_Des = Lav_Des
            'Lav_Des = objParametriAgenda.Lav_Des
            'If Lav_Des <> "" AndAlso Lav_Des <> Master_Operazione.Property_ComboOperazione.Testo_Combo Then
            '    'devo controllare corrispondenza tra combo e operazione
            '    Throw New NotImplementedException
            'End If
        End If



        Dim BaseCode As Integer
        Dim TopCode As Integer

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------
        UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode,
                                                 Session("ASG_ProgressivoGIAS"))



        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------


        Dim trovato As Boolean
        Dim ListaVarieta As New List(Of String)
        Dim StrVarieta As String = ""
        For i = 0 To ListaImp.Count - 1
            trovato = False
            For j = 0 To ListaVarieta.Count - 1
                If ListaVarieta(j) = ListaImp(i).Cul_Des Then
                    trovato = True
                    Exit For
                End If
            Next
            If Not trovato Then
                ListaVarieta.Add(ListaImp(i).Cul_Des)
                StrVarieta &= ", " & ListaImp(i).Cul_Des
            End If
        Next

        StrVarieta = StrVarieta.Substring(2, (StrVarieta.Length - 2))

        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = objParametriAgenda.Tipo_Operazione
        'Agenda.Id_Agenda = 0
        Agenda.Id_Agenda = objParametriAgenda.Id_Agenda
        Agenda.Data = Data
        Agenda.Piva = objParametriAgenda.Piva
        Agenda.Sa_Cod = Sa_Cod
        Agenda.Lav_Cod = objParametriAgenda.Lav_Cod
        Agenda.Des_Lib = Lav_Des & " (" & Veg_Des & "  [" & StrVarieta & "])"

        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode

        Return True
    End Function

    Private Function Crea_Agenda_Note(ByRef Agenda As Operazione_Agenda) As Boolean
        Dim Nota As Nota
        Dim ListaConsigli As List(Of Nota)
        ListaConsigli = CType(Master, Operazione).GetConsigli()
        If ListaConsigli.Count > 0 Then
            Agenda.Note = New List(Of Nota)
            For i = 0 To ListaConsigli.Count - 1
                Nota = New Nota
                Nota.Id_Agenda = objParametriAgenda.Id_Agenda
                Nota.Nota_Cod = ListaConsigli(i).Nota_Cod
                Agenda.Note.Add(Nota)
            Next
        End If
        Return True
    End Function

    Private Function Crea_Agenda_Movimento_CostiAccessori(ByRef Agenda As Operazione_Agenda) As Boolean
        For i = 0 To objParametriAgenda.Movimenti.Count - 1
            objParametriAgenda.Movimenti(i).Id_Agenda = objParametriAgenda.Id_Agenda
            objParametriAgenda.Movimenti(i).Sa_Cod = Agenda.Sa_Cod
            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli) Then
                Dim j As Integer
                For j = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda = objParametriAgenda.Id_Agenda
                    If objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = 0 Then
                        objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = Agenda.Sa_Cod
                    End If
                Next
            End If

            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then
                Dim j As Integer
                For j = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Id_Agenda = objParametriAgenda.Id_Agenda
                    If objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Sa_Cod = 0 Then
                        objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Sa_Cod = Agenda.Sa_Cod
                    End If

                Next
            End If

            objParametriAgenda.Movimenti(i).Data = Agenda.Data
            Agenda.Movimenti.Add(objParametriAgenda.Movimenti(i))
        Next
        Return True
    End Function

    Private Function Crea_Agenda_Movimento_InstallazioneTrappole_CattureMassa(ByVal ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto), ByRef inneschiNumTotale As Integer, ByRef Agenda As Operazione_Agenda, ByRef messaggio_errore As String) As Boolean


        Dim NumeMovDetTecTrappole As Integer = 0


        Select Case CInt(objParametriAgenda.Lav_Cod)
            Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                'continua, questo metodo deve essere chioamato solo in questi casi altrimenti genero accezione
            Case Else
                Throw New Exception
        End Select

        If IsNothing(TabellaTrappole) Then
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.InserirePrimaLaTabellaDelleTrappole
            Return False
        End If

        Dim Movimento_OperazioneColturale As New Movimento

        Movimento_OperazioneColturale.Id_Agenda = objParametriAgenda.Id_Agenda
        Movimento_OperazioneColturale.Piva = Agenda.Piva
        Movimento_OperazioneColturale.Sa_Cod = Agenda.Sa_Cod
        Movimento_OperazioneColturale.Lav_Cod = objParametriAgenda.Lav_Cod
        Movimento_OperazioneColturale.Cau_Mov = objParametriAgenda.Cau_Mov
        Movimento_OperazioneColturale.Mov_Desc = Master_Operazione.GetNota()
        Movimento_OperazioneColturale.Data = Agenda.Data
        Movimento_OperazioneColturale.BaseCode = Agenda.BaseCode
        Movimento_OperazioneColturale.TopCode = Agenda.TopCode


        Dim xCalcolo_QD_SuperficieTotale As Decimal = (
            From ST In ListaImpianti
            Select CType(ST.Qta2, Decimal)
        ).Sum

        '------------------------------------------------
        '----- MOVIMENTI DETTAGLI
        '------------------------------------------------

        Dim iColonna As Integer = 3
        'aggiungo una operazione MOVIMENTI DETTAGLI per ciascun appezzamento (tabella trappole tolte colonne Trappola,CodiceInterno,Inneschi)
        For iColonna = 3 To TabellaTrappole.Rows(0).Cells.Count - 1

            Dim Movimento_Dettaglio As New Movimento_Dettaglio

            Movimento_Dettaglio.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento_Dettaglio.Piva = Agenda.Piva
            Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio.Elem_Cod = TRAPPOLE
            Movimento_Dettaglio.Pro_Cod = cmb_Trappola.SelectedItem.Value
            Movimento_Dettaglio.Udm_Cod = enum_UnitaMisura.Numero_Trappole ' unità di misura n.trappole
            Movimento_Dettaglio.Qta = CInt(Txt_NumeroTrappole.Text)
            Movimento_Dettaglio.Contabilizzato = Movimento_Dettaglio_Contabilizzato_Installazione
            Movimento_Dettaglio.Pendente = Movimento_Dettaglio_Pendente_Installazione
            Movimento_Dettaglio.Extra_Date = Movimento_Dettaglio_Extra_Date
            Movimento_Dettaglio.Anno = Movimento_Dettaglio_Anno
            Movimento_Dettaglio.Data = Agenda.Data
            Movimento_Dettaglio.BaseCode = Agenda.BaseCode
            Movimento_Dettaglio.TopCode = Agenda.TopCode


            '------------------------------------------------
            '----- MOVIMENTI DESTINAZIONI
            '------------------------------------------------

            'per ciascun movimento dettaglio creo un figlio mov. destinazione
            Dim Movimento_Destinazione As New Movimento_Destinazione

            Movimento_Destinazione.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento_Destinazione.Data = Agenda.Data

            'Seleziono i dati dell'impianto dalla lista impianti selezionati
            'Prima:confidando nell'ordinamento. devo però bloccare la tabella di selezione impianti
            'Movimento_Destinazione.Id_Destinazione = ListaImpianti(iColonna - 3).ID_Reg...
            'Ora ho messo attrributi nella cella:
            'tcell.Attributes.Add("TipoGruppo", "Dato")
            'tcell.Attributes.Add("TipoCella", "NumeroInneschi")
            'tcell.Attributes.Add("IdTrappola", IdTrappola)
            'tcell.Attributes.Add("IdAzienda", CStr(ListaImpianti(i).Piva))
            'tcell.Attributes.Add("IdCentro", CStr(ListaImpianti(i).Sa_Cod))
            'tcell.Attributes.Add("IdAppezzamento", CStr(ListaImpianti(i).Appezza))
            'tcell.Attributes.Add("IdReg", CStr(ListaImpianti(i).ID_Reg))
            Movimento_Destinazione.Piva = TabellaTrappole.Rows(0).Cells(iColonna).Attributes("IdAzienda")
            Movimento_Destinazione.Sa_Cod = TabellaTrappole.Rows(0).Cells(iColonna).Attributes("IdCentro")
            Movimento_Destinazione.Appezza = TabellaTrappole.Rows(0).Cells(iColonna).Attributes("IdAppezzamento")
            Movimento_Destinazione.Id_Destinazione = TabellaTrappole.Rows(0).Cells(iColonna).Attributes("IdReg")

            'quantità delle trappole realmente posizionate nell'impianto
            Movimento_Destinazione.Qta = 0 'aggiorno dopo mentre leggo righe per fare i Movimento_Dettaglio_Tecnico

            Movimento_Destinazione.BaseCode = Agenda.BaseCode
            Movimento_Destinazione.TopCode = Agenda.TopCode

            'Estraggo la sup trattata
            For Each imp In ListaImpianti
                If imp.Piva = Movimento_Destinazione.Piva AndAlso imp.Sa_Cod = Movimento_Destinazione.Sa_Cod _
                    AndAlso imp.Appezza = Movimento_Destinazione.Appezza AndAlso imp.ID_Reg = Movimento_Destinazione.Id_Destinazione Then

                    Movimento_Destinazione.Qta2 = imp.Qta2
                    Exit For
                End If
            Next

            If xCalcolo_QD_SuperficieTotale <> 0 Then
                Movimento_Destinazione.QuotaDistribuzione = Movimento_Destinazione.Qta2 / xCalcolo_QD_SuperficieTotale
            End If

            Dim iRiga As Integer = 1
            Dim righeConCheck As Integer = 0
            Dim trapSenzaInnesco As Boolean = False
            For iRiga = 1 To TabellaTrappole.Rows.Count - 1

                'per ciascun movimento_destinazione e quindi per ciascun  appezzamento
                'cerco le checkbox selezionate ed aggiungo al movimento un movimento_dettaglio che indica che la trappola è presente nel campo
                'se il moviumento ha solo un figlio destinazione e non ha dettagli allora l'appezzamento è setto l'influenza di una trappola
                'ma non la contiene
                If (CType((TabellaTrappole.Rows(iRiga).Cells(iColonna).Controls(0)), RadioButton).Checked) Then

                    'tengo traccia del numero di trappole presenti nell'appezzamento, da inserire poi nell'attributo qta del movimento_destinazione
                    righeConCheck = righeConCheck + 1

                    'Dim codiceInterno As String
                    'codiceInterno = CType(Ricerca.FindControlIterative(TabellaTrappole.Rows(iRiga), "TXTCodicePersonalizzato#" & iRiga), TextBox).Text

                    Dim codiceTrappola As String
                    codiceTrappola = TabellaTrappole.Rows(iRiga).Cells(1).Attributes("IdTrappola")


                    Dim codicePersonalizzatoTrappola As String
                    codicePersonalizzatoTrappola = CType((TabellaTrappole.Rows(iRiga).Cells(1).Controls(0)), TextBox).Text
                    If Not IsNumeric(codicePersonalizzatoTrappola) Then
                        If codicePersonalizzatoTrappola <> "" Then
                            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.IlCodicePersonalizzatoDellaTrappolaDeveEss
                            Return False
                        End If
                    End If

                    'per le operazioni conf e dis sessuale gli inneschi non servono, vengono ignorati anche se presenti nella tabella
                    ' e nell'agenda
                    Dim inneschiStr As String = CType((TabellaTrappole.Rows(iRiga).Cells(2).Controls(0)), TextBox).Text
                    If Not IsNumeric(inneschiStr) Then
                        messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.ControllareIlNumeroDiInneschi
                        Return False
                    End If

                    If CInt(inneschiStr) < 1 Then

                        '  Galassi, 13/10/2016 11:01:39: Aggiunto per permettere di utilizzare anche le trappole senza innesco (diverse segnalazioni. Vedi Federica)
                        If CInt(inneschiStr) = 0 Then
                            trapSenzaInnesco = True
                        Else
                            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.ControllareIlNumeroDiInneschiDeveEssereMag
                            Return False
                        End If

                    End If
                    Dim inneschiNellaTrappola As Integer = CType(inneschiStr, Integer)
                    inneschiNumTotale = inneschiNumTotale + inneschiNellaTrappola


                    '------------------------------------------------
                    '----- MOVIMENTO DETTAGLIO TECNICO X INSTALLAZIONE TRAPPOLE
                    '------------------------------------------------

                    Dim Movimento_Dettaglio_Tecnico As New Movimento_Dettaglio_Tecnico

                    Movimento_Dettaglio_Tecnico.Id_Agenda = objParametriAgenda.Id_Agenda
                    Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
                    Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
                    Movimento_Dettaglio_Tecnico.Data = Agenda.Data
                    Movimento_Dettaglio_Tecnico.Ditta_cod = CStr(cmb_Ditte.SelectedItem.Value)
                    Movimento_Dettaglio_Tecnico.Dose = inneschiNellaTrappola
                    If codicePersonalizzatoTrappola <> "" Then
                        Movimento_Dettaglio_Tecnico.Freatimetro = CDbl(codicePersonalizzatoTrappola)
                    Else
                        Movimento_Dettaglio_Tecnico.Freatimetro = 0
                    End If
                    Movimento_Dettaglio_Tecnico.Sigla_av = CStr(Txt_CodAvversita.Text)
                    Movimento_Dettaglio_Tecnico.Trap_num = codiceTrappola
                    Movimento_Dettaglio_Tecnico.Inn1_data = Agenda.Data
                    Movimento_Dettaglio_Tecnico.Av_Cod = cmb_Avversita.SelectedItem.Value
                    Movimento_Dettaglio_Tecnico.BaseCode = Agenda.BaseCode
                    Movimento_Dettaglio_Tecnico.TopCode = Agenda.TopCode
                    'Movimento_Dettaglio_Tecnico.Codice_Personalizzato = codiceInterno
                    'aggiungo il movimento dettaglio tecnico al movimento dettaglio
                    Movimento_Dettaglio.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

                End If

            Next

            '--------------------------------------
            'controllo giacenza inneschi
            Select Case CInt(objParametriAgenda.Lav_Cod)
                Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                    If (objParametriAgenda.Fabbricato <> "" AndAlso objParametriAgenda.Fabbricato <> "0") AndAlso
                      Not controllaGiacenza_Trappole_Inneschi(0, inneschiNumTotale, messaggio_errore) Then
                        'al suo interno si controlla se premuto ok per proseguire senza giacenze

                        '--------------BLOCCO SALVATAGGIO SE_SUPERA_GIACENZE-------------------
                        If Not IsNothing(Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE")) AndAlso Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE") = True Then

                            Dim MErrore As String
                            MErrore = "In base alle impostazioni utente NON è possibile usare un prodotto con giacenza non sufficiente! " & vbCr & messaggio_errore
                            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & MErrore, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                            Return False

                        End If
                        '---------------------------------------------------------------------

                        'controllo se ho acconsentito precedenrtemente al salvataggio senza giacenze
                        If Giacenza_SI_NO.Value = "0" Then

                            'se non ho acconsentito genere agrosino che mi rilancera il salvataggio via jscript
                            messaggio_errore = messaggio_errore & vbCr & Resources.AgronicaAgenda_2010.BrBIProcedereUgualmenteSenzaGestireLeGiace
                            'AgroSiNo
                            'Messaggi.AgroSiNo(messaggio, "Salva", Page, , UpdatePanelGridImpianti)
                            'impedisco di procedere per ora
                            Messaggi.AgroSiNo(messaggio_errore & vbCr & Resources.AgronicaAgenda_2010.BrBIAltrimentiInserireUnValoreValidoOTogli, "Giacenze", Page, , UpdatePanelGridImpianti)

                            Return False

                        Else
                            'se ho già cliccato  ok vado avanti

                            '  Galassi, 13/10/2016 11:01:39: Aggiunto per permettere di utilizzare anche le trappole senza innesco (diverse segnalazioni. Vedi Federica)
                            If trapSenzaInnesco Then

                                Messaggi.AgroMsgBox("AVVISO: Una o più trappole sono state inserite senza innesco! Verificare di avere utilizzato delle trappole che non necessitano dell'innesco", Page, ,
                                    CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                            End If
                        End If
                    End If
                Case Else
                    Throw New NotImplementedException
            End Select


            Movimento_Destinazione.Qta = righeConCheck

            NumeMovDetTecTrappole += righeConCheck

            'aggiungo il movimento destinazione al movimento detaglio
            Movimento_Dettaglio.Movimenti_Destinazioni.Add(Movimento_Destinazione)

            'aggiungo il movimento dettaglio al movimento
            Movimento_OperazioneColturale.Movimenti_Dettagli.Add(Movimento_Dettaglio)


        Next

        If NumeMovDetTecTrappole <> CInt(Txt_NumeroTrappole.Text) Then
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.ControllareIlNumeroDiTrappole
            Throw New NotImplementedException
            'Return False
        End If


        'aggiungo il movimento rilievo in campo-installazione trappola all'operazione agenda
        Agenda.Movimenti.Add(Movimento_OperazioneColturale)
        Return True
    End Function

    Private Function Crea_Agenda_Movimento_Confusione_Disorientamento_Sessuale(ByVal ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto), ByRef Agenda As Operazione_Agenda, ByVal NumeroTrappoleCentro As Decimal, ByRef messaggio_errore As String) As Boolean

        Select Case CInt(objParametriAgenda.Lav_Cod)
            Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
                'continua, questo metodo deve essere chioamato solo in questi casi altrimenti genero accezione
            Case Else
                Throw New Exception
        End Select


        Dim Movimento_OperazioneColturale As New Movimento

        Movimento_OperazioneColturale.Id_Agenda = objParametriAgenda.Id_Agenda
        Movimento_OperazioneColturale.Piva = Agenda.Piva
        Movimento_OperazioneColturale.Sa_Cod = Agenda.Sa_Cod
        Movimento_OperazioneColturale.Lav_Cod = objParametriAgenda.Lav_Cod
        Movimento_OperazioneColturale.Cau_Mov = objParametriAgenda.Cau_Mov
        Movimento_OperazioneColturale.Mov_Desc = Master_Operazione.GetNota()
        Movimento_OperazioneColturale.Data = Agenda.Data
        Movimento_OperazioneColturale.BaseCode = Agenda.BaseCode
        Movimento_OperazioneColturale.TopCode = Agenda.TopCode




        '------------------------------------------------
        '----- MOVIMENTI DETTAGLI
        '------------------------------------------------

        'Per confusione e disorientamento, essendoci molte trappole, devo suddividerle equamente tra gli appezzamenti,
        'inoltre in questo caso aggiungo un solo movimento dettaglio , che al suo interno ha
        'una destinazione per ciascun appezzamento e un solo dettaglio tecnico
        'aggiungo una operazione MOVIMENTI DETTAGLI per ciascun appezzamento (tabella trappole tolte colonne Trappola,CodiceInterno,Inneschi)

        Dim Movimento_Dettaglio As New Movimento_Dettaglio

        Movimento_Dettaglio.Id_Agenda = objParametriAgenda.Id_Agenda
        Movimento_Dettaglio.Piva = Agenda.Piva
        Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
        Movimento_Dettaglio.Elem_Cod = TRAPPOLE
        Movimento_Dettaglio.Pro_Cod = cmb_Trappola.SelectedItem.Value
        Movimento_Dettaglio.Udm_Cod = enum_UnitaMisura.Numero_Trappole ' unità di misura n.trappole
        Movimento_Dettaglio.Qta = NumeroTrappoleCentro 'senza multicentro era CInt(Txt_NumeroTrappole.Text)
        Movimento_Dettaglio.Contabilizzato = Movimento_Dettaglio_Contabilizzato_Installazione
        Movimento_Dettaglio.Pendente = Movimento_Dettaglio_Pendente_Installazione
        Movimento_Dettaglio.Extra_Date = Movimento_Dettaglio_Extra_Date
        Movimento_Dettaglio.Anno = Movimento_Dettaglio_Anno
        Movimento_Dettaglio.Data = Agenda.Data
        Movimento_Dettaglio.BaseCode = Agenda.BaseCode
        Movimento_Dettaglio.TopCode = Agenda.TopCode


        '------------------------------------------------
        '----- MOVIMENTI DESTINAZIONI
        '------------------------------------------------

        'una destinazione per ciascun appezzamento

        'calcolo prima la superficie totale, 
        'mi serve per il calcolo della qta (quantità trappole per superficie)
        Dim SuperficieTotale As Decimal = 0
        For Each Imp As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In ListaImpianti
            SuperficieTotale = SuperficieTotale + Imp.Qta2
        Next

        Dim QuantitaTotale_New As Decimal = 0

        Dim xCalcolo_QD_SuperficieTotale As Decimal = (
            From ST In ListaImpianti
            Select CType(ST.Qta2, Decimal)
        ).Sum

        For i = 0 To ListaImpianti.Count - 1

            Dim Movimento_Destinazione As New Movimento_Destinazione

            Movimento_Destinazione.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento_Destinazione.Data = Agenda.Data
            Movimento_Destinazione.Piva = Agenda.Piva
            Movimento_Destinazione.Sa_Cod = Agenda.Sa_Cod
            Movimento_Destinazione.Appezza = ListaImpianti(i).Appezza
            Movimento_Destinazione.Id_Destinazione = ListaImpianti(i).ID_Reg

            'quantità delle trappole partizionate per superficie-----------------------------------
            Dim QuantitaTotale As Decimal = NumeroTrappoleCentro 'senza multicentro era CDbl(Me.Txt_NumeroTrappole.Text)
            Dim Frazione As Decimal = ListaImpianti(i).Qta2 / SuperficieTotale
            Dim QuantitaParziale As Decimal = Math.Floor(QuantitaTotale * Frazione)
            QuantitaTotale_New = QuantitaTotale_New + QuantitaParziale
            'se dividendo il numero di trappole sugli impianti
            'e troncando le qta parziali la somma è inferiore al numero indicato dall'utente
            'aggiungo la differenza all'ultimo impianto
            If i = ListaImpianti.Count - 1 Then
                If QuantitaTotale <> QuantitaTotale_New Then
                    QuantitaParziale = QuantitaParziale + (QuantitaTotale - QuantitaTotale_New)
                End If
            End If
            Movimento_Destinazione.Qta = QuantitaParziale
            Movimento_Destinazione.Qta2 = ListaImpianti(i).Qta2

            If xCalcolo_QD_SuperficieTotale <> 0 Then
                Movimento_Destinazione.QuotaDistribuzione = ListaImpianti(i).Qta2 / xCalcolo_QD_SuperficieTotale
            End If
            '--------------------------------------------------------------------

            Movimento_Destinazione.BaseCode = Agenda.BaseCode
            Movimento_Destinazione.TopCode = Agenda.TopCode

            'aggiungo il movimento destinazione al movimento detaglio
            Movimento_Dettaglio.Movimenti_Destinazioni.Add(Movimento_Destinazione)

        Next



        '------------------------------------------------
        '----- MOVIMENTO DETTAGLIO TECNICO X conf e dis sessuale
        '------------------------------------------------

        'un solo mov dettaglio tecnico, contiene solo informazioni sulla trappola
        'non c'è un mov dettaglio tecnico per ciascuna trappola ma le trappole sono migliaia, non vengono considerate
        'singolarmante ma ripartito il loro numero tramite la destinazione , il conteggio tatale nel movimento e il movimento dettaglio
        'per tenere conto del tipo

        Dim Movimento_Dettaglio_Tecnico As New Movimento_Dettaglio_Tecnico

        Movimento_Dettaglio_Tecnico.Id_Agenda = objParametriAgenda.Id_Agenda
        Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
        Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
        Movimento_Dettaglio_Tecnico.Data = Agenda.Data
        Movimento_Dettaglio_Tecnico.Ditta_cod = CStr(cmb_Ditte.SelectedItem.Value)
        Movimento_Dettaglio_Tecnico.Dose = 0
        Movimento_Dettaglio_Tecnico.Freatimetro = 0 'non c'è codice della trappola
        Movimento_Dettaglio_Tecnico.Sigla_av = CStr(Txt_CodAvversita.Text)
        Movimento_Dettaglio_Tecnico.Trap_num = 0
        Movimento_Dettaglio_Tecnico.Inn1_data = Agenda.Data
        Movimento_Dettaglio_Tecnico.Av_Cod = cmb_Avversita.SelectedItem.Value
        Movimento_Dettaglio_Tecnico.BaseCode = Agenda.BaseCode
        Movimento_Dettaglio_Tecnico.TopCode = Agenda.TopCode
        'Movimento_Dettaglio_Tecnico.Codice_Personalizzato = codiceInterno

        'aggiungo il movimento dettaglio tecnico al movimento dettaglio
        Movimento_Dettaglio.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)


        'aggiungo il movimento dettaglio al movimento
        Movimento_OperazioneColturale.Movimenti_Dettagli.Add(Movimento_Dettaglio)



        'aggiungo il movimento rilievo in campo-installazione trappola all'operazione agenda
        Agenda.Movimenti.Add(Movimento_OperazioneColturale)

        Return True
    End Function


    Private Function Crea_Agenda_Movimento_Scarico(ByRef inneschiNumTotale As Integer, ByRef Agenda As Operazione_Agenda, ByVal NumeroTrappoleCentro As Decimal, ByRef Messaggio_Errore As String) As Boolean

        Dim sa_cod_magazzino As String = ""
        Dim fabbricatox_Cod As String = ""

        Dim magazzinoEsterno As Boolean = True
        If objParametriAgenda.Fabbricato <> "0" AndAlso Split(objParametriAgenda.Fabbricato, "|").Count = 3 AndAlso Split(objParametriAgenda.Fabbricato, "|")(2) <> objParametriAgenda.Piva Then
            'se magazzino è della azienda padre
            magazzinoEsterno = True
            Dim sa_cod_magazzino_predefinito_azienda As Integer
            Dim fabbricatox_Cod_magazzino_predefinito_azienda As Integer
            Dim fabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R()
            fabbricati.Ricava_PrimoMagazzino_Impresa(Agenda.Piva, sa_cod_magazzino_predefinito_azienda, fabbricatox_Cod_magazzino_predefinito_azienda, objParametri_Server)
            sa_cod_magazzino = sa_cod_magazzino_predefinito_azienda
            fabbricatox_Cod = fabbricatox_Cod_magazzino_predefinito_azienda

        Else
            'se magazzino è quello dell'azienda
            magazzinoEsterno = False
            sa_cod_magazzino = Split(objParametriAgenda.Fabbricato, "|")(1)
            fabbricatox_Cod = Split(objParametriAgenda.Fabbricato, "|")(0)

        End If

        If CInt(Txt_NumeroTrappole.Text) > 0 Then
            'deve esserci sempre un movimento di scarico trappola
            'per ciascuna operazione (trappole, massa, confusione..)

            Dim Movimento_Scarico As New Movimento
            Movimento_Scarico.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento_Scarico.Piva = Agenda.Piva
            Movimento_Scarico.Sa_Cod = sa_cod_magazzino 'Agenda.Sa_Cod è sbagliato se ho magazzino in altro centro se multicentro è sbagliat ousare Split(objParametriAgenda.Fabbricato, "|")(1)
            Movimento_Scarico.Data = Agenda.Data
            Movimento_Scarico.Lav_Cod = objParametriAgenda.Lav_Cod
            Movimento_Scarico.Cau_Mov = CAU_SCARICO
            Movimento_Scarico.Mov_Desc = "Scarico Magazzino"
            Movimento_Scarico.BaseCode = Agenda.BaseCode
            Movimento_Scarico.TopCode = Agenda.TopCode


            '------------------------------------------------
            '----- MOVIMENTI DETTAGLI SCARICO TRAPPOLA
            '------------------------------------------------

            'Scarico la trappola
            Dim Movimento_Dettaglio_Scarico_Trappola As New Movimento_Dettaglio

            Movimento_Dettaglio_Scarico_Trappola.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento_Dettaglio_Scarico_Trappola.Piva = Agenda.Piva
            Movimento_Dettaglio_Scarico_Trappola.Sa_Cod = sa_cod_magazzino 'ok per multicentro
            Movimento_Dettaglio_Scarico_Trappola.Data = Agenda.Data
            Movimento_Dettaglio_Scarico_Trappola.Elem_Cod = TRAPPOLE
            Movimento_Dettaglio_Scarico_Trappola.Pro_Cod = cmb_Trappola.SelectedItem.Value 'Dispenser_Cod
            Movimento_Dettaglio_Scarico_Trappola.Mov_Det_Des = "Scarico di Trappole"
            Movimento_Dettaglio_Scarico_Trappola.Udm_Cod = enum_UnitaMisura.Numero_Trappole
            Movimento_Dettaglio_Scarico_Trappola.Qta = NumeroTrappoleCentro 'senza multicentro era CInt(Txt_NumeroTrappole.Text)
            Movimento_Dettaglio_Scarico_Trappola.Contabilizzato = Movimento_Dettaglio_Contabilizzato_Magazzino
            Movimento_Dettaglio_Scarico_Trappola.Pendente = Movimento_Dettaglio_Pendente_Magazzino
            Movimento_Dettaglio_Scarico_Trappola.Lav_Cod = objParametriAgenda.Lav_Cod
            Movimento_Dettaglio_Scarico_Trappola.Cau_Mov = CAU_SCARICO

            '------------------------------------------------
            '----- MOVIMENTI DESTINAZIONI SCARICO TRAPPOLA
            '------------------------------------------------

            Dim Movimento_Destinazione_Scarico_Trappola As New Movimento_Destinazione
            Movimento_Destinazione_Scarico_Trappola.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento_Destinazione_Scarico_Trappola.Data = Agenda.Data
            Movimento_Destinazione_Scarico_Trappola.Piva = objParametriAgenda.Piva
            Movimento_Destinazione_Scarico_Trappola.Sa_Cod = sa_cod_magazzino
            Movimento_Destinazione_Scarico_Trappola.Id_Destinazione = fabbricatox_Cod
            Movimento_Destinazione_Scarico_Trappola.Tipo = MAGAZZINO
            Movimento_Destinazione_Scarico_Trappola.Qta = NumeroTrappoleCentro 'senza multicentro era CInt(Txt_NumeroTrappole.Text)
            Movimento_Destinazione_Scarico_Trappola.BaseCode = Agenda.BaseCode
            Movimento_Destinazione_Scarico_Trappola.TopCode = Agenda.TopCode

            'TRAPPOLE
            '-------------- aggiungo il movimento destinazione al movimento dettaglio
            Movimento_Dettaglio_Scarico_Trappola.Movimenti_Destinazioni.Add(Movimento_Destinazione_Scarico_Trappola)
            '-------------- aggiungo il movimento dettaglio allo scarico
            Movimento_Scarico.Movimenti_Dettagli.Add(Movimento_Dettaglio_Scarico_Trappola)

            If inneschiNumTotale > 0 Then
                'trappole e catture massa
                Select Case CInt(objParametriAgenda.Lav_Cod)
                    Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                        'ok, continua
                    Case Else
                        Throw New Exception 'non deve arrivare qui
                End Select
                '------------------------------------------------
                '----- MOVIMENTI DETTAGLIO SCARICO INNESCO
                '------------------------------------------------
                Dim Movimento_Dettaglio_Scarico_Innesco As New Movimento_Dettaglio
                Movimento_Dettaglio_Scarico_Innesco.Id_Agenda = objParametriAgenda.Id_Agenda
                Movimento_Dettaglio_Scarico_Innesco.Piva = Agenda.Piva
                Movimento_Dettaglio_Scarico_Innesco.Sa_Cod = sa_cod_magazzino
                Movimento_Dettaglio_Scarico_Innesco.Data = Agenda.Data
                Movimento_Dettaglio_Scarico_Innesco.Elem_Cod = INNESCHI
                Movimento_Dettaglio_Scarico_Innesco.Pro_Cod = Me.cmb_Avversita.SelectedItem.Value
                Movimento_Dettaglio_Scarico_Innesco.Mov_Det_Des = "Scarico di Inneschi Installazione Trappole"
                Movimento_Dettaglio_Scarico_Innesco.Udm_Cod = enum_UnitaMisura.Numero_Inneschi
                Movimento_Dettaglio_Scarico_Innesco.Qta = inneschiNumTotale
                Movimento_Dettaglio_Scarico_Innesco.Contabilizzato = Movimento_Dettaglio_Contabilizzato_Magazzino
                Movimento_Dettaglio_Scarico_Innesco.Pendente = Movimento_Dettaglio_Pendente_Magazzino
                Movimento_Dettaglio_Scarico_Innesco.Lav_Cod = objParametriAgenda.Lav_Cod
                Movimento_Dettaglio_Scarico_Innesco.Cau_Mov = CAU_SCARICO


                '------------------------------------------------
                '----- MOVIMENTI DESTINAZIONI SCARICO INNESCO
                '------------------------------------------------
                Dim Movimento_Destinazione_Scarico_Innesco As New Movimento_Destinazione
                Movimento_Destinazione_Scarico_Innesco.Id_Agenda = objParametriAgenda.Id_Agenda
                Movimento_Destinazione_Scarico_Innesco.Data = Agenda.Data
                Movimento_Destinazione_Scarico_Innesco.Piva = objParametriAgenda.Piva
                Movimento_Destinazione_Scarico_Innesco.Sa_Cod = sa_cod_magazzino
                Movimento_Destinazione_Scarico_Innesco.Id_Destinazione = fabbricatox_Cod
                Movimento_Destinazione_Scarico_Innesco.Tipo = MAGAZZINO
                Movimento_Destinazione_Scarico_Innesco.Qta = inneschiNumTotale
                Movimento_Destinazione_Scarico_Innesco.BaseCode = Agenda.BaseCode
                Movimento_Destinazione_Scarico_Innesco.TopCode = Agenda.TopCode

                'INNESCHI
                '-------------- aggiungo il movimento destinazione al movimento dettaglio
                Movimento_Dettaglio_Scarico_Innesco.Movimenti_Destinazioni.Add(Movimento_Destinazione_Scarico_Innesco)
                '-------------- aggiungo il movimento dettaglio allo scarico
                Movimento_Scarico.Movimenti_Dettagli.Add(Movimento_Dettaglio_Scarico_Innesco)
            Else
                'disorientamento e confusione sessuale (non ci sono inneschi)
                If objParametriAgenda.Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE OrElse
                    (objParametriAgenda.Lav_Cod) = LAVCOD_CATTURE_MASSA Then
                    'deve esserci sempre almeno un movimento di scarico innesco 
                    'per trappole e massa, se viene richiamato questo metodo
                    'lancio eccezione, vuol dire che qualcosa è stato pensato male
                    'Throw New NotImplementedException
                    'Return False
                End If

            End If

            '-------------- aggiungo il movimento scarico all'agenda
            Agenda.Movimenti.Add(Movimento_Scarico)

            Return True

        Else
            'deve esserci sempre almeno un movimento di scarico trappola se viene richiamato questo metodo
            'lancio eccezione, vuol dire che qualcosa è stato pensato male
            Throw New NotImplementedException
            Return False
        End If

    End Function

#End Region

#Region "Eventi Gestiti Dalla Master"

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)
        sblocca()
        RipristinaSessione()

        Dim objParametriAgenda As New ParametriAgenda
        Dim link As String = ""
        Try
            Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda.SitoOrigine
            Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

            If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                link = RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                       enum_PagineGiasOnline_2010.RegistazioneSmart,
                                       enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

            ElseIf sitoorigine = Enum_SiteRedirector.GiasNG Then
                MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                            Enum_SiteRedirector.GiasNG,
                                                            objParametriAgenda.PaginaSitoOrigine,
                                                            link,
                                                            objParametri_Server)
            Else
                link = CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
            End If

        Catch ex As Exception
            link = CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
        End Try

        Response.Redirect(link)

    End Sub


    Private Sub CambioMagazzino()
        Cmb_Trappola_Carica()
        'LeggoLaGiacenzaDelleTrappoleEInneschi()
    End Sub


    Private Sub CambioSpecie()
        Cmb_Trappola_Carica()
    End Sub


    Private Sub MasterUnload()
        'con questo mi ricarica tutte le combo e toglie selezionato in modifica

        'questa operazione è fatta dall'evento master unload
        'ma il load della pagina master potrebbe avere cambiato dei valori in objparametri (come nel caso di sa_cod).
        'se alla fine modifico nella pagina figlia un valore viene in automatico riscritta in sessione
        'la objparametri della pagina figlia, sovrascrivendo la master e perdendo quindi le modifiche fatte
        'objParametriAgenda.Leggi()

        If Not IsPostBack Then
            Cmb_Trappola_Carica()
        End If
    End Sub

#End Region

#Region "Salvataggio"

    Private Sub SalvaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        '--------------BLOCCO SALVATAGGIO SENZA MAGAZZINO-------------------
        If Not IsNothing(Session("UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO")) AndAlso Session("UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO") = True Then
            If objParametriAgenda.Fabbricato = "0" OrElse objParametriAgenda.Fabbricato = "" Then
                Dim MErrore As String
                MErrore = "In base alle impostazioni utente NON è possibile salvare l'operazione senza utilizzare il magazzino!"
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & MErrore, Page, ,
                    CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                Exit Sub
            End If

        End If
        '---------------------------------------------------------------------


        '-----------------------------------------------------------------------------------------------------
        '-----------------------TEMPORANERO PER EVITARE SALVATAGGIO MAGAZZINI ESTERNI------------------------
        If objParametriAgenda.Fabbricato <> "0" AndAlso Split(objParametriAgenda.Fabbricato, "|").Count = 3 AndAlso Split(objParametriAgenda.Fabbricato, "|")(2) <> objParametriAgenda.Piva Then
            Dim MErrore As String
            MErrore = Resources.AgronicaAgenda_2010.ATTENZIONEBrAlMomentoNonÈPermessoIlSalvata1
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & MErrore, Page, ,
                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Exit Sub
        End If
        '-----------------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------------


        Dim messaggio_errore As String = ""
        If SalvaOperazioneAgenda(messaggio_errore) Then
            'premutosalva = True
            gestisciTipoSalvataggio()
        Else
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & messaggio_errore, Page, ,
                  Master_Operazione.Property_UpdatePanelToolBar)
        End If
    End Sub


    Private Function SalvaOperazioneAgenda(ByRef messaggio_errore As String) As Boolean
        Dim res As Boolean = False


        'Controllo se i dati sono presenti nei controlli

        'se sono in modifica e non ho avversità o trappola o ditta allora avevo codici non validi, occorre cancellare e reinserire

        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica AndAlso
            (objParametriAgenda.Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE OrElse objParametriAgenda.Lav_Cod = LAVCOD_CATTURE_MASSA) Then
            If cmb_Trappola.SelectedItem.Value = "" OrElse cmb_Trappola.SelectedItem.Value = "0" OrElse cmb_Trappola.SelectedItem.Text = "" Then
                messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.TrappolaNonValida & Resources.AgronicaAgenda_2010.SiConsigliaDiCancellareLOperazioneERicrear
                Return False
            End If

            If cmb_Avversita.SelectedItem.Value = "" OrElse cmb_Avversita.SelectedItem.Value = "0" OrElse cmb_Avversita.SelectedItem.Text = "" Then
                messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.LAvversitàConCuiEraStataCreataLOperazioneN & Resources.AgronicaAgenda_2010.SiConsigliaDiCancellareLOperazioneERicrear
                Return False
            End If

            If Txt_CodAvversita.Text = "" Then
                messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.LAvversitàConCuiEraStataCreataLOperazioneN & Resources.AgronicaAgenda_2010.SiConsigliaDiCancellareLOperazioneERicrear
                Return False
            End If

            'If cmb_Ditte.SelectedItem.Value = "" Or cmb_Ditte.SelectedItem.Value = "0" Or cmb_Ditte.SelectedItem.Text = "" Then
            '    messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.LaDittaConCuiEraStataCreataLOperazioneNonÈ & Resources.AgronicaAgenda_2010.SiConsigliaDiCancellareLOperazioneERicrear
            '    Return False
            'End If
        End If


        If cmb_Trappola.SelectedItem.Value = "" OrElse cmb_Trappola.SelectedItem.Value = "0" OrElse cmb_Trappola.SelectedItem.Text = "" Then
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.SelezionareUnaTrappola
            Return False
        End If

        If cmb_Avversita.SelectedItem.Value = "" OrElse cmb_Avversita.SelectedItem.Value = "0" OrElse cmb_Avversita.SelectedItem.Text = "" Then
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.SelezionareUnaAvversità
            Return False
        End If

        If Txt_CodAvversita.Text = "" Then
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.SelezionareUnaAvversità
            Return False
        End If

        'If cmb_Ditte.SelectedItem.Value = "" Or cmb_Ditte.SelectedItem.Value = "0" Or cmb_Ditte.SelectedItem.Text = "" Then
        '    messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.SelezionareUnaDitta
        '    Return False
        'End If


        If Not IsNumeric(Txt_NumeroTrappole.Text) Then
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.IndicareIlNumeroDiTrappole
            Return False
        End If

        If CInt(Txt_NumeroTrappole.Text) < 1 Then
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.IndicareUnNumeroDiTrappoleValidoMaggiore1
            Return False
        End If


        'controllo la giacenza trappole
        If (objParametriAgenda.Fabbricato <> "" AndAlso objParametriAgenda.Fabbricato <> "0") AndAlso
            Not controllaGiacenza_Trappole_Inneschi(CInt(Me.Txt_NumeroTrappole.Text), 0, messaggio_errore) Then


            '--------------BLOCCO SALVATAGGIO SE_SUPERA_GIACENZE-------------------
            If Not IsNothing(Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE")) AndAlso Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE") = True Then

                Dim MErrore As String
                MErrore = "In base alle impostazioni utente NON è possibile usare un prodotto con giacenza non sufficiente! " & vbCr & messaggio_errore
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & MErrore, Page, ,
                    CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                Return False

            End If
            '---------------------------------------------------------------------

            'controllo se ho acconsentito precedenrtemente al salvataggio senza giacenze
            If Giacenza_SI_NO.Value = "0" Then

                'se non ho acconsentito genere agrosino che mi rilancera il salvataggio via jscript
                messaggio_errore = messaggio_errore & vbCr & Resources.AgronicaAgenda_2010.TrappoleSalvaNoGestioneGiacenze
                'AgroSiNo
                'Messaggi.AgroSiNo(messaggio, "Salva", Page, , UpdatePanelGridImpianti)
                'impedisco di procedere per ora
                Messaggi.AgroSiNo(messaggio_errore & vbCr & Resources.AgronicaAgenda_2010.TrappoleInserireValoreValidoDisabilitaG, "Giacenze", Page, , UpdatePanelGridImpianti)

                Return False

            Else
                'se ho già cliccato  ok vado avanti

            End If
        End If



        '---------------------------------------
        ' recupero il CENTRO
        If (objParametriAgenda.Sa_Cod = "" OrElse objParametriAgenda.Sa_Cod = "0") AndAlso Not objParametriAgenda.OperazioneMulticentro Then
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.SelezionareUnCentroAziendale
            Return False
        End If



        '---------------------------------------
        ' recupero gli IMPIANTI
        Dim ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
        ListaImpianti = CType(Master, Operazione).GetImpianti()
        If ListaImpianti.Count = 0 Then
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.SelezionareAlmenoUnImpiantoColturale
            Return False
        End If

        '---------------------------------------
        'Se è stato selezionato 'Tutti i centri aziendali' ma tutti gli impianti appartengono ad un solo centro, imposto il valore sull'objparametriAgenda
        If objParametriAgenda.Sa_Cod = "0" Then
            Dim listaSaCodImpianti As List(Of Integer) = (From x As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In ListaImpianti Select x.Sa_Cod).Distinct().ToList()
            If listaSaCodImpianti.Count = 1 Then
                objParametriAgenda.Sa_Cod = listaSaCodImpianti.First().ToString()
            End If
        End If
        '---------------------------------------

        Try


            '-----------------------------------------------------
            '----------- CONNESSIONE E TRANSAZIONE ---------------
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            '-----------------------------------------------------


            Select Case objParametriAgenda.Sa_Cod

                '--------------------------------------------------
                '--------------------------------------------------
                '--------- OPERAZIONE MULTI-CENTRO    -------------
                '--------------------------------------------------
                '--------------------------------------------------
                Case "0"

                    'apro transazione per scrittura multipla di movimenti
                    '-----------------------------------------------------
                    '' ''----------- CONNESSIONE E TRANSAZIONE ---------------
                    ' ''Dim objDP As New AgronicaCoreDataProvider.ConnessioniTransazioni
                    ' ''ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
                    '' ''-----------------------------------------------------

                    'creazione lista centri coinvolti nell'operazione
                    Dim ListaSacod As New List(Of Integer)
                    Dim Trovato As Boolean
                    For i = 0 To ListaImpianti.Count - 1
                        Trovato = False
                        For j = 0 To ListaSacod.Count - 1
                            If ListaImpianti(i).Sa_Cod = ListaSacod(j) Then
                                Trovato = True
                                Exit For
                            End If
                        Next
                        If Not Trovato Then
                            ListaSacod.Add(ListaImpianti(i).Sa_Cod)
                        End If
                    Next

                    'calcolo le qta per singolo Centro

                    '----------
                    'quantità delle trappole partizionate per superficie-----------------------------------

                    'calcolo prima la superficie totale di tutti gli impianti, 
                    'mi serve per il calcolo della qta (quantità trappole per superficie)
                    Dim SuperficieTotale As Decimal = 0 'superficie totale tutti i centri
                    Dim SuperficieXCentro As New Hashtable() 'superficie per ciascun centro
                    For Each Imp As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In ListaImpianti
                        SuperficieTotale = SuperficieTotale + Imp.Sup_Imp
                        'Sommo le sup impianti relative a questo centro aziendale
                        If IsNothing(SuperficieXCentro(Imp.Sa_Cod)) Then
                            'nuovo centro
                            SuperficieXCentro.Add(Imp.Sa_Cod, CDbl(Imp.Sup_Imp))
                        Else
                            'centro esiste
                            SuperficieXCentro(Imp.Sa_Cod) = SuperficieXCentro(Imp.Sa_Cod) + CDbl(Imp.Sup_Imp)
                        End If
                    Next

                    Dim ListaTrappoleXCentri As New Hashtable()

                    Dim QuantitaTotale As Decimal = CDbl(Me.Txt_NumeroTrappole.Text)
                    Dim QuantitaTotale_New As Decimal = 0

                    Dim c1 As Integer = 0
                    For Each SupXCen As DictionaryEntry In SuperficieXCentro

                        Dim Frazione As Decimal = SupXCen.Value / SuperficieTotale
                        Dim QuantitaParziale As Decimal = Math.Floor(QuantitaTotale * Frazione)
                        QuantitaTotale_New = QuantitaTotale_New + QuantitaParziale

                        'se dividendo il numero di trappole sugli impianti
                        'e troncando le qta parziali la somma è inferiore al numero indicato dall'utente
                        'aggiungo la differenza all'ultimo impianto
                        If c1 = SuperficieXCentro.Count - 1 Then
                            If QuantitaTotale <> QuantitaTotale_New Then
                                QuantitaParziale = QuantitaParziale + (QuantitaTotale - QuantitaTotale_New)
                            End If
                        End If
                        c1 += 1
                        ListaTrappoleXCentri.Add(SupXCen.Key, QuantitaParziale)
                    Next



                    '----------

                    'Inizio il ciclo sui centri Aziendali
                    'Ciclo per ogni centro aziendale
                    For i = 0 To ListaSacod.Count - 1
                        Dim ListaImpiantixQuestoSaCod As New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

                        'Seleziono solamente gli impianti relativi a questo centro aziendale
                        For j = 0 To ListaImpianti.Count - 1
                            If ListaImpianti(j).Sa_Cod = ListaSacod(i) Then
                                ListaImpiantixQuestoSaCod.Add(ListaImpianti(j))
                            End If
                        Next

                        '--------------------------------------------------
                        '--------------------------------------------------
                        '--------- CREAZIONE OGGETTO DA SALVARE   ---------
                        '--------------------------------------------------
                        '--------------------------------------------------

                        Dim Agenda As Operazione_Agenda = CreaOggettoAgenda(ListaImpiantixQuestoSaCod, ListaSacod(i), ListaTrappoleXCentri(ListaSacod(i)), messaggio_errore)

                        If IsNothing(Agenda) Then
                            Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                        End If


                        Dim objAgendaScrivi As New Agenda_Operazione_Helper
                        Dim Id_Agenda As Integer

                        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                            'Dal 06/23 conserviamo la data e l'username di creazione originali per tutti i record Agenda
                            allinea_DataUsernameCreazione(Agenda, objParametri_Server, Tipo_Attivita.QuadernoDiCampagna, 0)

                            Dim CancellataOperazione As Boolean = False
                            CancellataOperazione = objAgendaScrivi.Cancella(objParametriAgenda.Piva,
                                                                             objParametriAgenda.Sa_Cod,
                                                                             objParametriAgenda.Id_Agenda, False,
                                                                             objParametri_Server, logCancellazione:=False)
                        End If

                        Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                        'Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                        ''non spostare, l'operazione agenda vecchia in modifica devo eliminarla dopo
                        'Dim util As New Utility_NS.Utility_Operazioni()
                        'util.Gestisci_Magazzino_Aziendale(Agenda, Id_Agenda, objParametriAgenda, objParametri_Server)

                        ''se la scrittura è andata a buon fine e sono in modifica
                        ''CANCELLO la vecchia operazione

                        'If Id_Agenda <> 0 And objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
                        '    If Id_Agenda_Old > 0 Then
                        '        Dim CancellataOperazione As Boolean = False

                        '        CancellataOperazione = objAgendaScrivi.Cancella(objParametriAgenda.Piva, _
                        '                                                         objParametriAgenda.Sa_Cod, _
                        '                                                         Id_Agenda_Old, False, _
                        '                                                         objParametri_Server)


                        '        'modifico l'aggancio alla ricetta
                        '        Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_W
                        '        Dim ModRicetta As Boolean
                        '        ModRicetta = objRicetta.Modifica_Agenda(0, 0, _
                        '                                                Id_Agenda_Old, _
                        '                                                Id_Agenda, _
                        '                                                AGRODATAINIZIO, _
                        '                                                AGRODATAFINE, _
                        '                                                "", _
                        '                                                objParametri_Server)
                        '    End If
                        'End If


                        'se sono n scrittura devo agganciare la ricetta se presente
                        If Id_Agenda <> 0 AndAlso objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
                            ' se Session("UtilizzataRicetta") esiste e se è true allora ho utilizzato la ricetta per creare l'operazione
                            If Not IsNothing(Session("UtilizzataRicetta")) AndAlso Session("UtilizzataRicetta") Then

                                Dim ricetta_cod As String = Session("ricetta_cod")
                                Dim Ricetta_Operazione_Cod As String = Session("Ricetta_Operazione_Cod")

                                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                                Dim Impostazione_RicetteXagenda As String = objUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_RiferimentoRicetteOperazioni,
                                                                        HttpContext.Current.Session("ASG_objParametri_Utenti"),
                                                                        1)
                                If Impostazione_RicetteXagenda <> "0" Then
                                    Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_W
                                    If Not objRicetta.Scrivi(ricetta_cod, Ricetta_Operazione_Cod, Id_Agenda, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server) Then
                                        Throw New Exception(Resources.AgronicaAgenda_2010.BNonÈRiuscitoLAggancioDellaRicettaBBr)
                                    End If
                                End If



                            End If
                        End If



                        'objParametriAgenda.Id_Agenda = Id_Agenda
                        objAgendaScrivi = Nothing
                    Next



                    '' ''chiudi connessione e commit transazione
                    ' ''ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
                    ' ''ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)



                Case Else

                    '--------------------------------------------------
                    '--------------------------------------------------
                    '--------- OPERAZIONE SINGOLO CENTRO    -----------
                    '--------------------------------------------------
                    '--------------------------------------------------

                    '--------------------------------------------------
                    '--------------------------------------------------
                    '--------- CREAZIONE OGGETTO DA SALVARE   ---------
                    '--------------------------------------------------
                    '--------------------------------------------------

                    Dim Agenda As Operazione_Agenda = CreaOggettoAgenda(ListaImpianti, objParametriAgenda.Sa_Cod, CDbl(Me.Txt_NumeroTrappole.Text), messaggio_errore)

                    If IsNothing(Agenda) Then
                        Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                    End If

                    ' ''-----------------------------------------------------
                    ' ''----------- CONNESSIONE E TRANSAZIONE ---------------
                    ''ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
                    ' ''-----------------------------------------------------

                    Dim objAgendaScrivi As New Agenda_Operazione_Helper
                    Dim Id_Agenda As Integer

                    If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                        'Recupero i vecchi costi CdG prima che vengano cancellati
                        Dim mdr_Rif As New Agenda_Movimenti_Dettagli_Riferimenti_Helper()
                        Dim lista_mdRif As List(Of Movimento_Dettaglio_Riferimento) = mdr_Rif.LeggiRiferimentiAgenda(Agenda.Piva, 0, Agenda.Id_Agenda, 0, "", objParametri_Server)

                        'Dal 06/23 conserviamo la data e l'username di creazione originali per tutti i record Agenda
                        allinea_DataUsernameCreazione(Agenda, objParametri_Server, Tipo_Attivita.QuadernoDiCampagna, 0)

                        Dim CancellataOperazione As Boolean = objAgendaScrivi.Cancella(objParametriAgenda.Piva,
                                                                                       objParametriAgenda.Sa_Cod,
                                                                                       objParametriAgenda.Id_Agenda, False,
                                                                                       objParametri_Server, logCancellazione:=False)

                        'Riscrivo i vecchi costi
                        Dim mdRif_helper As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                        For Each mdRif As Movimento_Dettaglio_Riferimento In lista_mdRif
                            If mdRif.Lav_Cod_Rif = LAVCOD_COSTI_CDG Then
                                mdRif_helper.Scrivi(mdRif, objParametri_Server)
                                Exit For
                            End If
                        Next

                    End If

                    Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                    'Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                    ''non spostare, l'operazione agenda vecchia in modifica devo eliminarla dopo
                    'Dim util As New Utility_NS.Utility_Operazioni()
                    'util.Gestisci_Magazzino_Aziendale(Agenda, Id_Agenda, objParametriAgenda, objParametri_Server)

                    ''se la scrittura è andata a buon fine e sono in modifica
                    ''CANCELLO la vecchia operazione

                    'If Id_Agenda <> 0 And objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
                    '    If Id_Agenda_Old > 0 Then
                    '        Dim CancellataOperazione As Boolean = False

                    '        CancellataOperazione = objAgendaScrivi.Cancella(objParametriAgenda.Piva, _
                    '                                                         objParametriAgenda.Sa_Cod, _
                    '                                                         Id_Agenda_Old, False, _
                    '                                                         objParametri_Server)


                    '        'modifico l'aggancio alla ricetta
                    '        Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_W
                    '        Dim ModRicetta As Boolean
                    '        ModRicetta = objRicetta.Modifica_Agenda(0, 0, _
                    '                                                Id_Agenda_Old, _
                    '                                                Id_Agenda, _
                    '                                                AGRODATAINIZIO, _
                    '                                                AGRODATAFINE, _
                    '                                                "", _
                    '                                                objParametri_Server)
                    '    End If
                    'End If

                    objAgendaScrivi = Nothing


                    'se sono n scrittura devo agganciare la ricetta se presente
                    If Id_Agenda <> 0 AndAlso objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
                        ' se Session("UtilizzataRicetta") esiste e se è true allora ho utilizzato la ricetta per creare l'operazione
                        If Not IsNothing(Session("UtilizzataRicetta")) AndAlso Session("UtilizzataRicetta") Then

                            Dim ricetta_cod As String = Session("ricetta_cod")
                            Dim Ricetta_Operazione_Cod As String = Session("Ricetta_Operazione_Cod")

                            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                            Dim Impostazione_RicetteXagenda As String = objUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_RiferimentoRicetteOperazioni,
                                                                    HttpContext.Current.Session("ASG_objParametri_Utenti"),
                                                                    1)
                            If Impostazione_RicetteXagenda <> "0" Then
                                Dim objRicetta As New AgronicaCoreContabDAL.RicettexAgenda_W
                                If Not objRicetta.Scrivi(ricetta_cod, Ricetta_Operazione_Cod, Id_Agenda, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server) Then
                                    Throw New Exception(Resources.AgronicaAgenda_2010.BNonÈRiuscitoLAggancioDellaRicettaBBr)
                                End If
                            End If



                        End If
                    End If

                    If CType(Ricerca.FindControlIterative(Page.Master, "RBL_Salva"), RadioButtonList).SelectedValue = 0 OrElse
                                CType(Ricerca.FindControlIterative(Page.Master, "RBL_Salva"), RadioButtonList).SelectedValue = 3 Then
                        objParametriAgenda.Id_Agenda = Id_Agenda
                    End If



            End Select

            res = True

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

        Catch ex As Exception

            'commit transazione rollback
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            'Throw New Exception("[ Agenda_Operazione_Helper.scrivi() ] : " & ex.Message)

            'Messaggi.AgroMsgBox("Attenzione, l'operazione non è stata registrata! <br> " & ex.Message, Page, , _
            '                   CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

            messaggio_errore = ex.Message
            res = False

        Finally

            'chiudi connessione
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try



        Return res

    End Function


    Private Sub gestisciTipoSalvataggio()
        Session("UtilizzataRicetta") = False
        Dim TipoSalvataggio As String = Master_Operazione.Property_RBL_Salva.SelectedValue + 1
        Select Case TipoSalvataggio
            Case enum_Tipo_Salvataggio.Salva_e_Esci
                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")


                Dim link As String = ""
                Try
                    Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda.SitoOrigine
                    Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

                    If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                        link = RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                               Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                               enum_PagineGiasOnline_2010.RegistazioneSmart,
                                               enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

                    ElseIf sitoorigine = Enum_SiteRedirector.GiasNG Then
                        MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                        Enum_SiteRedirector.GiasNG,
                                                                        objParametriAgenda.PaginaSitoOrigine,
                                                                        link,
                                                                        objParametri_Server)

                    Else
                        link = CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                    End If

                Catch ex As Exception
                    link = CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                End Try


                strJS.AppendLine("      ChiamataParent_Id_Ageda(" & objParametriAgenda.Id_Agenda & "); ")
                strJS.AppendLine("      window.location = '" & link & "'; ")

                strJS.AppendLine(" });")
                ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

                RipristinaSessione()
                'Response.Redirect(CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda))

                If objParametriAgenda.Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE OrElse objParametriAgenda.Lav_Cod = LAVCOD_CATTURE_MASSA Then
                    sblocca()
                End If


            Case enum_Tipo_Salvataggio.Salva_e_Nuovo
                Trappole_Disposed()
                Giacenza_SI_NO.Value = "0"
                objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                objParametriAgenda.Note = New List(Of Nota)
                objParametriAgenda.Movimenti = New List(Of Movimento)

                If objParametriAgenda.Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE OrElse objParametriAgenda.Lav_Cod = LAVCOD_CATTURE_MASSA Then
                    sblocca()
                End If

                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")
                'strJS.AppendLine("      BloccaSbloccaTotale(); ")
                strJS.AppendLine("      window.location = '../Operazioni/Installazione_Trappole.aspx'; ")
                strJS.AppendLine(" });")
                ScriptManager.RegisterStartupScript(Master_Operazione.Property_UpdatePanelPerScript, Master_Operazione.Property_UpdatePanelPerScript.GetType(),
                                              String.Format("jQuery_{0}", Master_Operazione.Property_UpdatePanelPerScript.ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                  Master_Operazione.Property_UpdatePanelToolBar)

            Case enum_Tipo_Salvataggio.Salva_e_Duplica
                Giacenza_SI_NO.Value = "0"

                'Dim strJS As New StringBuilder
                'strJS.AppendLine("$(document).ready(function () { ")
                'strJS.AppendLine("      BloccaSbloccaTotale(); ")
                'strJS.AppendLine(" });")
                'ScriptManager.RegisterStartupScript(Master_Operazione.Property_UpdatePanelToolBar1, Master_Operazione.Property_UpdatePanelToolBar1.GetType(),
                '                              String.Format("jQuery_{0}", Master_Operazione.Property_UpdatePanelToolBar1.ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                  Master_Operazione.Property_UpdatePanelToolBar)

                'genero tabella nuovam,ente, devo aggiornare i codici trappole e non posso usare la stessa tabella


                If objParametriAgenda.Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE OrElse objParametriAgenda.Lav_Cod = LAVCOD_CATTURE_MASSA Then
                    sblocca()
                End If

                LeggoLaGiacenzaDelleTrappoleEInneschi()

                If PlaceTabella.Controls.Count > 0 Then
                    PlaceTabella.Controls.RemoveAt(0)
                End If

                'If GeneraTabellaHtmlTrappole(True) Then
                '    Dim fdfd As Integer
                'End If

                If objParametriAgenda.Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE OrElse objParametriAgenda.Lav_Cod = LAVCOD_CATTURE_MASSA Then
                    blocca()
                End If

            Case enum_Tipo_Salvataggio.Salva_e_Vai_ai_Costi

                Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda.SitoOrigine

                Dim PaginaLink As String = "../AnalisiCostiProduzione/GestioneCosti.aspx"

                Dim link As String = ""
                Try
                    Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

                    If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                        link = RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                                   enum_PagineGiasOnline_2010.RegistazioneSmart,
                                                   enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

                    Else
                        link = CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                    End If

                Catch ex As Exception
                    link = CType(Master.Master, DomandaIrriguaBootstrap).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
                End Try

                PaginaLink &= "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
                              "&id_agenda=" & Stringa_Codifica(objParametriAgenda.Id_Agenda, AgroKey_EncoderDecoder) &
                              If(sitoorigine = Enum_SiteRedirector.GiasNG, "", "&origine=" & Stringa_Codifica(link, AgroKey_EncoderDecoder)) &
                              "&entrata_diretta=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing)

                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")
                strJS.AppendLine("      ChiamataParent_Id_Ageda(" & objParametriAgenda.Id_Agenda & "); ")
                strJS.AppendLine("      window.location = '" & PaginaLink & "'; ")
                strJS.AppendLine(" });")

                ScriptManager.RegisterStartupScript(Master_Operazione.Property_UpdatePanelPerScript, Master_Operazione.Property_UpdatePanelPerScript.GetType(),
                                                    String.Format("jQuery_{0}", Master_Operazione.Property_UpdatePanelPerScript.ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                      CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

                RipristinaSessione()
                'Response.Redirect(CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda))

                If objParametriAgenda.Lav_Cod = LAVCOD_INSTALLAZIONE_TRAPPOLE OrElse objParametriAgenda.Lav_Cod = LAVCOD_CATTURE_MASSA Then
                    sblocca()
                End If

        End Select
    End Sub


    Private Sub RigeneraTabellaPerSalvataggio()

        TabellaTrappole = Session("Tabella")

        'reimposto i valori inseriti
        For i = 1 To TabellaTrappole.Rows.Count - 1

            CType(TabellaTrappole.Rows(i).Cells(1).Controls(0), TextBox).Text =
                Request.Form(CType(TabellaTrappole.Rows(i).Cells(1).Controls(0), TextBox).ClientID.Replace("_", "$"))

            CType(TabellaTrappole.Rows(i).Cells(2).Controls(0), TextBox).Text =
                Request.Form(CType(TabellaTrappole.Rows(i).Cells(2).Controls(0), TextBox).ClientID.Replace("_", "$"))

            Dim val As String = ""
            For j = 0 To Request.Form.AllKeys.Length - 1
                If Not IsNothing(Request.Form.AllKeys(j)) AndAlso (Request.Form.AllKeys(j).EndsWith("grupporiga" & i - 1)) Then
                    val = Request.Form(Request.Form.AllKeys(j))
                End If
            Next

            For j = 3 To TabellaTrappole.Rows(0).Cells.Count - 1
                If val = CType(TabellaTrappole.Rows(i).Cells(j).Controls(0), CheckBox).ID Then
                    CType(TabellaTrappole.Rows(i).Cells(j).Controls(0), CheckBox).Checked = True
                Else
                    CType(TabellaTrappole.Rows(i).Cells(j).Controls(0), CheckBox).Checked = False
                End If
            Next

        Next

        PlaceTabella.Controls.Add(TabellaTrappole)

    End Sub


#End Region

    Private Sub aggiornaDataScadenza()
        Try
            If Not IsNothing(Txt_DataScadenz) AndAlso Not IsNothing(Txt_GiorniFeromone) AndAlso Not IsNothing(Master_Operazione.Property_txt_DataOperazione) Then

                If IsNumeric(Me.Txt_GiorniFeromone.Text) Then
                    Me.Txt_DataScadenz.Text = CDate(Master_Operazione.Property_txt_DataOperazione.Text).AddDays(CInt(Me.Txt_GiorniFeromone.Text))
                    LeggoLaGiacenzaDelleTrappoleEInneschi()
                End If


            End If
        Catch ex As Exception
            'Me.Txt_DataScadenz.Text = System.DateTime.Now
        End Try
    End Sub

    Private Function controllaGiacenza_Trappole_Inneschi(NumTrappoleDaControllare As Integer, NumInneschiDaControllare As Integer, ByRef Messaggio_Errore As String) As Boolean


        Dim messaggio As String = ""


        If NumTrappoleDaControllare > 0 Then

            Dim GiacenzeTrappole As Integer = 0

            'guardo se la quantità è conforme per la data di intervento
            LeggiGiacenze(GiacenzeTrappole, Nothing, CDate(objParametriAgenda.Data))
            If NumTrappoleDaControllare > GiacenzeTrappole Then
                messaggio = messaggio & String.Format(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaQuantitàDiTrappoleBBAlBX0BÈP, objParametriAgenda.Data, GiacenzeTrappole)
            End If

            'guardo se la quantità è conforme per la data attuale
            LeggiGiacenze(GiacenzeTrappole, Nothing, Date.Today)
            If NumTrappoleDaControllare > GiacenzeTrappole Then
                messaggio = messaggio & String.Format(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaGiacenzaAttualeDiTrappoleBBX, GiacenzeTrappole)
            End If

        End If

        If NumInneschiDaControllare > 0 Then

            Dim GiacenzeInneschi As Integer = 0

            'guardo se la quantità è conforme per la data di intervento
            LeggiGiacenze(Nothing, GiacenzeInneschi, CDate(objParametriAgenda.Data))
            If NumInneschiDaControllare > GiacenzeInneschi Then
                messaggio = messaggio & String.Format(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaQuantitàDiInneschiBBAlBX0BÈP, objParametriAgenda.Data, GiacenzeInneschi)
            End If

            'guardo se la quantità è conforme per la data attuale
            LeggiGiacenze(Nothing, GiacenzeInneschi, Date.Today)
            If NumInneschiDaControllare > GiacenzeInneschi Then
                messaggio = messaggio & String.Format(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaGiacenzaAttualeDiInneschiBBX, GiacenzeInneschi)
            End If

        End If

        If messaggio <> "" Then

            Messaggio_Errore = Messaggio_Errore & messaggio
            Return False

        End If

        Return True


    End Function


    Private Sub BTN_CaricoMagazzino(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim xChiave As String = ""
        Call Albero.ChiaveAlbero_Codifica(xChiave,
                                               enum_TipoNodo.p_PortafoglioProdotti,
                                               objParametriAgenda.Fabbricato.Split("|")(2),
                                               objParametriAgenda.Fabbricato.Split("|")(1), , , , , , , , , , , ,
                                               objParametriAgenda.Fabbricato.Split("|")(0))

        ' FormProdotto non è presente su DomandaIrrigua: redirect cross-site verso AgroAgenda
        ' tramite il meccanismo standard ParametriAgenda_2010 → GestioneRichieste → FormProdotto.aspx
        Dim objPA2010 As New ParametriAgenda_2010()
        objPA2010.PaginaRichiesta     = enum_PagineAgenda_2010.Pagina_FormProdotto
        objPA2010.Chiave              = xChiave
        objPA2010.OperazioneMagazzino = "C"
        objPA2010.Mode                = "magazzino"
        objPA2010.Lavorazione         = objParametriAgenda.Lav_Cod
        objPA2010.DataSelezionata     = objParametriAgenda.Data
        objPA2010.Sa_Cod              = objParametriAgenda.Sa_Cod
        objPA2010.Id_Agenda           = objParametriAgenda.Id_Agenda
        objPA2010.Piva                = objParametriAgenda.Piva
        objPA2010.Salva()

        Dim url As String = RedirectGestione.PreparaPassaggio_ConParametriSitoInSessione_e_Ritorna_IndirizzoCompletoConQueryString(
            Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua,
            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
            "")

        Dim strJS As String = "<script language='javascript'>" &
            "window.open('" & url & "'," &
            "'stampe'," &
            "'height=700,width=1000,menubar=yes,resizable=yes,scrollbars=yes,top=0,left=0');" &
            "</script>"

        ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel),
                                            CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel).GetType(),
                                            "jQuery_{0}", strJS, False)

    End Sub

    Protected Sub GridView_Impianti_SelectedIndexChanged(sender As Object, e As EventArgs)

    End Sub





#Region "Per Ricette"

    Private Sub Btn_Conferma_Ricetta(sender As Object, e As EventArgs)


        Dim listaImpiantiSelezionati As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto) = Master_Operazione.GetImpianti()
        If listaImpiantiSelezionati.Count = 0 Then
            Exit Sub
        End If

        Ripristina_Dati_nei_ControlliDaRicetta()

    End Sub

    Private Sub Ripristina_Dati_nei_ControlliDaRicetta()

        Session("UtilizzataRicetta") = False


        Dim ricetta_cod As String = Session("ricetta_cod")
        Dim Ricetta_Operazione_Cod As String = Session("Ricetta_Operazione_Cod")


        Dim dt_RicetteOperazioni As DataTable = New AgronicaCoreContabDAL.Ricette_Operazioni_R().Leggi(CInt(ricetta_cod), CInt(Ricetta_Operazione_Cod), 0, 0, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        Dim dt_RicetteDettagli As DataTable = New AgronicaCoreContabDAL.Ricette_Dettagli_R().Leggi(CInt(ricetta_cod), CInt(Ricetta_Operazione_Cod), 0, "", 0, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        Dim dt_RicetteDettagliTecnici As DataTable = New AgronicaCoreContabDAL.Ricette_Dett_Tecnico_R().Leggi(CInt(ricetta_cod), CInt(Ricetta_Operazione_Cod), 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
        'Dim dt_RicetteDestinazioni As DataTable = New AgronicaCoreContabDAL.Ricette_Destinazioni_R().Leggi(CInt(ricetta_cod), CInt(Ricetta_Operazione_Cod), 0, 0, 0, "", 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

        If dt_RicetteOperazioni.Rows.Count = 0 Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.NonÈStataTrovataLaRicettaOperazione, Page, ,
                    CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Exit Sub
        End If

        If dt_RicetteOperazioni.Rows(0).Item("Lav_Cod") <> objParametriAgenda.Lav_Cod Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.LaRicettaHaUnaOperazioneAssociataDifferent, Page, ,
                    CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Exit Sub
        End If


        'CONTROLLARE
        CType(Page.Master, Operazione).SetNota(dt_RicetteOperazioni.Rows(0).Item("Note"))



        'ci possono essere piu dettagli per lo stesso trattamento, 
        'sfoglio i dettagli come fossero i movimenti, per leggere i cau_mov,
        'e essere sicuro di leggere quelli del trattamento e non quelli dei costi accessori o magazzino
        If dt_RicetteDettagli.Rows.Count = 0 Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.NonSonoStatiTrovatiDettagli, Page, ,
                    CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Exit Sub
        End If

        'Dim Trappola_Cod As Integer = 0
        Dim Trappola_Dispenser As Integer = 0
        'Dim Trappola_UDM As Integer = 0
        Dim Trappola_QTA As Integer = 0
        'Dim Inneschi_QTA As Integer = 0
        'Dim Trappola_IDPers As Integer = 0
        Dim Trappola_Ditta As Integer = 0
        'Dim Trappola_ID As Integer = 0
        Dim Trappola_AV_COD As Integer = 0
        'Dim Trappola_AV_Sigla As String = ""

        'estraggo i cau_mov trattamenti, possono essere più di uno perchè la ricetta na un dettaglio per ciascun prodotto che contiene il cau_cov
        Dim dr_RicetteDettagliTrattamenti() As DataRow
        Select Case CInt(objParametriAgenda.Lav_Cod)

            Case LAVCOD_INSTALLAZIONE_TRAPPOLE
                dr_RicetteDettagliTrattamenti = dt_RicetteDettagli.Select("Cau_Mov= '" & enum_Agenda_Causali.RILIEVO_CAMPO & "' ")

            Case LAVCOD_CATTURE_MASSA
                dr_RicetteDettagliTrattamenti = dt_RicetteDettagli.Select("Cau_Mov= '" & enum_Agenda_Causali.TRATTAMENTO & "' ")

            Case LAVCOD_CONFUSIONE_SESSUALE
                dr_RicetteDettagliTrattamenti = dt_RicetteDettagli.Select("Cau_Mov= '" & enum_Agenda_Causali.TRATTAMENTO & "' ")

            Case LAVCOD_DISORIENTAMENTO_SESSUALE
                dr_RicetteDettagliTrattamenti = dt_RicetteDettagli.Select("Cau_Mov= '" & enum_Agenda_Causali.TRATTAMENTO & "' ")

        End Select

        'Per le trappole, 
        'per ciascun impianto ho un dettaglio che indica il codice elemento, prodotto, udm, qta, uguali per tutti i dettagli, e mi interessa solo qta che indica il totale delle trappole installate in tutti gli impianti
        'per ciascun dettaglio ho una destinazione che indica l'impianto e la qta parziale (k), non mi interessa leggere
        'per ciascun dettaglio ho k dettagli tecnic, ciascuno identifica una trappola installata nell'impianto con il suo id e codice personaliz, 
        '   ma leggo solo av_cod e ditta cod, essendo uguali per tutte le trappole basta leggere un solo det tecn.
        '   il det tecnocop ha anche il numero inneschi nella dose, ma non li leggo, ci sono solo, come altri campi, quando è generata dall'agenda
        If dr_RicetteDettagliTrattamenti.Count > 0 Then

            'Trappola_Cod = CInt(dr_RicetteDettagliTrattamenti(0).Item("Elem_Cod")) '197 trappole
            Trappola_Dispenser = CInt(dr_RicetteDettagliTrattamenti(0).Item("Pro_Cod")) 'codice dispenser per la combo
            'Trappola_UDM = CInt(dr_RicetteDettagliTrattamenti(0).Item("Udm_Cod")) 'udm, numero trappole
            Trappola_QTA = CInt(dr_RicetteDettagliTrattamenti(0).Item("Qta")) 'Numero totale trappole

            'MOVIMENTI_DETTAGLI_TECNICI

            Dim Dr_DettagliTEcRicette_Avversita() As DataRow = dt_RicetteDettagliTecnici.Select("Ricetta_Dettaglio_Cod= " & dr_RicetteDettagliTrattamenti(0).Item("Ricetta_Dettaglio_Cod") & " ")
            If Dr_DettagliTEcRicette_Avversita.Count > 0 Then


                'Inneschi_QTA = CDbl(Dr_DettagliTEcRicette_Avversita(0).Item("dose"))
                'Trappola_IDPers = CDbl(Dr_DettagliTEcRicette_Avversita(0).Item("Freatimetro"))
                Trappola_Ditta = CInt(Dr_DettagliTEcRicette_Avversita(0).Item("Ditta_Cod"))
                'Trappola_ID = CInt(Dr_DettagliTEcRicette_Avversita(0).Item("Trap_Num"))
                Trappola_AV_COD = CInt(Dr_DettagliTEcRicette_Avversita(0).Item("Av_Cod"))
                'Trappola_AV_Sigla = CInt(Dr_DettagliTEcRicette_Avversita(0).Item("Sigla_AV"))

                'Non inserisco le trappole ma faccio fare manualmente, in modo che si possa cambiare il numero e la posizione


                Me.Txt_NumeroTrappole.Text = Trappola_QTA

                cmb_Trappola.SelectedValue = Trappola_Dispenser
                If cmb_Trappola.SelectedValue <> Trappola_Dispenser Then
                    cmb_Trappola.SelectedValue = "-1"
                End If

                cmb_Trappola_SelectedIndexChanged(Nothing, Nothing)

                cmb_Ditte.SelectedValue = Trappola_Ditta
                If cmb_Ditte.SelectedValue <> Trappola_Ditta Then
                    cmb_Ditte.SelectedValue = ""
                End If

                cmb_Avversita.SelectedValue = Trappola_AV_COD
                If cmb_Avversita.SelectedValue <> Trappola_AV_COD Then
                    cmb_Avversita.SelectedValue = ""
                End If

                cmb_Avversita_SelectedIndexChanged(Nothing, Nothing)

            End If

        End If

        Session("UtilizzataRicetta") = True

    End Sub

#End Region

End Class