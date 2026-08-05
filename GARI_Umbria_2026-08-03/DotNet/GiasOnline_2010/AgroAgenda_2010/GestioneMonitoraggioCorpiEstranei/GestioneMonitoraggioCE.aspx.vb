Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility
Imports System.Xml
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreModello.ParametriAgenda_Temp



Public Class GestioneMonitoraggioCE
    Inherits System.Web.UI.Page


    '----- Gestione Querystring
    Dim Qs_Piva, Qs_Rag_Soc As String

    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Public Master_Operazione As Agenda

    '##########################################################################################################
    Private Sub GestioneCE_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        Master_Operazione = CType(Page.Master, Agenda)
        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
    End Sub

    '##########################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim objParametriAgenda As New ParametriAgenda With {
            .Piva = Qs_Piva
        }

        Dim link As String = ""
        Try
            Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")
            Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

            If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.Menu Then

                link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                    Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                    enum_PagineGiasOnline_2010.Menu,
                    enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

            ElseIf sitoorigine = Enum_SiteRedirector.Sito_GiasOnline AndAlso paginaOnLineRitorno = enum_PagineGiasOnline.MenuCartellaAziendale Then

                objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Visite_Lista
                link = AgronicaCoreModello.Utility_Operazioni.Link_GiasOnline_STR(enum_PagineGiasOnline.MenuCartellaAziendale, objParametriAgenda)

            ElseIf sitoorigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 AndAlso paginaOnLineRitorno = enum_PagineAgenda_2010.Menu_BS Then
                link = "../Menu/MenuBS_Agenda_Nuovo.aspx"
            Else
                link = "../Menu/MenuBS_2017.aspx"
            End If

        Catch ex As Exception
            link = "../Menu/MenuBS_2017.aspx"
        End Try

        Response.Redirect(link)

    End Sub

    '################################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        CType(Page.Master.FindControl("Lbl_Titolo"), Label).Text = "Filtro Monitoraggio Corpi Estranei"

        Response.Expires = 0

        '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

        Try

            '##############################################################
            '#####  Verifico Credenziali di Accesso  ######################
            '##############################################################

            If Session("ASG_Utente_Username") = "" Then
                Response.Redirect("~/Custom500.aspx")
            End If

         
            '########################################################################################
            '##### inizializzazione oggetti objParametri_Utenti e objParametri_Server  ##############
            '########################################################################################
            '---
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            '---


            '##############################################################
            '#####  Recupero le informazioni da querystring  ##############
            '##############################################################

            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, AgroKey_EncoderDecoder)

            If Not IsNothing(Request.QueryString("rs")) Then

                Qs_Rag_Soc = Stringa_Decodifica(Request.QueryString("rs").ToString, AgroKey_EncoderDecoder)

            Else
                ' TODo
                '''''Qs_Rag_Soc = RagSoc_from_Piva(Server, Session, Page, Qs_Piva)
            End If


            '##############################################################
            '#####  Verifico se sono in Post-Back  ########################
            '##############################################################

            If Not Page.IsPostBack Then

                '==========================================
                '===== Pagina caricata per la prima volta
                '==========================================

                '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

                Dim utenteAbilitato As Boolean = False

                Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

                utenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                    objParametri_Utenti.UtenteUsername,
                                            enum_Id_Servizio.GiasOnline,
                                            enum_Security_Attivita.Gest_CartellaAziendale_MonitoraggioCE,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now, "", objParametri_Utenti)

                If Not utenteAbilitato Then
                    Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
                    Exit Sub
                End If

            Else

                '==========================================
                '===== Pagina in postback
                '==========================================

                Select Case Me.SI_NO.Value

                    Case "0" 'NO

                    Case "1" 'SI
                        CancellaMonitoraggioCE()
                End Select

                Exit Sub

            End If

            '==========================================
            '===== Carica controlli
            '==========================================

            CaricaTipologia()

            'Non carico tutte le orticole visibili dall'utente
            'CaricaSpecie()
            'carico solo le specie vegetali oggetto del monitoraggio
            CaricaSpecieVegetaliMonitoraggioCE()

            'se l'utente ha il magazzino impostato nel filtro di visibilità
            '-> far vedere solo quello
            'altrimenti caricarli tutti
            CaricaStabilimenti(Qs_Piva)

            '----------------------------------

            'Default
            Me.Txt_DataInizio.Text = "01/" & Right("00" + Date.Today.Month.ToString, 2) & "/" & Date.Today.Year.ToString

            'eliminato caricamento automatico in data 09/11/2011
            ' Toolbar_Carica()

            Lbl_NumModuliCarico.Visible = False

            '########################################################################

        Catch exc As Exception

            Messaggi.AgroMsgBox("Problemi nel caricamento della pagina: " & vbCrLf & exc.Message, Page)

        End Try

    End Sub


    '##################################################################################
    Private Function CaricaStabilimenti(ByVal Qspiva As String) As Boolean

        Try

            Dim Impostazione_Valore_1 As String
            Dim Piva As String = ""
            Dim Sa_Cod As Integer = 0
            Dim Fabbricato_Cod As Integer = 0

            Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Impostazione_Valore_1 = objImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_MAGAZZINO_RIFERIMENTO,
                                                                                             objParametri_Utenti, 1)

            Dim objADD As New AgronicaCoreContabHLP.AccettazioneDaDiversi
            objADD.Leggi_ChiaveMagazzino_Default(Piva, Sa_Cod, Fabbricato_Cod, Impostazione_Valore_1)


            Dim clc = New CaricaListControl
            clc.Fabbricati(Me.Cmb_Magazzino,
                           False, "", "",
                           Qspiva,
                           Sa_Cod, Fabbricato_Cod,
                           MAGAZZINO,
                           True,
                           "",
                           " Fabbricati.Fabbricato_Des ",
                           AGRODATAFINE,
                           objParametri_Server)

            Return True

        Catch ex As Exception
            Messaggi.AgroMsgBox("Caricamento dei magazzini. Si è verificato il seguente errore: " & ex.Message, Page)
            Return False
        End Try

    End Function


    '##################################################################################
    Private Function CaricaProduttore(ByVal Rag_Soc As String) As Boolean

        Try
            
            Dim FiltroAggiuntivo As String = ""

            If Rag_Soc <> "" Then
                FiltroAggiuntivo &= "  (Imprese.Rag_Soc LIKE '%" & Agro_SQL_SaveText(Rag_Soc) & "%')   "
            End If

            Dim objGerarchiaImpresa As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            Dim DT_Prod As DataTable = objGerarchiaImpresa.LeggiFigli("", "",
                                                                      1,
                                                                      0,
                                                                      enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                      FiltroAggiuntivo, "Imprese.Rag_Soc",
                                                                      objParametri_Server)

            If DT_Prod.Rows.Count > 0 Then

                ' Seleziono solo le colonne Rag_Soc e Figlio, eliminando i duplicati
                Dim DT_Prod_Distinct As DataTable = DT_Prod.DefaultView.ToTable(True, "Rag_Soc", "Figlio")

                AgronicaCoreUtility.CaricaListControl.Produttori(CType(Cmb_Produttore, ListControl),
                                                                 True, "", "",
                                                                 DT_Prod_Distinct, "", "", objParametri_Server)

                If Me.Cmb_Produttore.Items.Count > 1 Then
                    Cmb_Produttore.SelectedIndex = 1
                End If

                Return True
            End If
            Return False

        Catch ex As Exception
            Messaggi.AgroMsgBox("Caricamento dei produttori. Si è verificato il seguente errore: " & ex.Message, Page)
            Return False
        End Try

    End Function


    '##################################################################################
    Private Function CaricaTipologia() As Boolean

        Cmb_TipologiaRitrovamento.Items.Clear()

        Cmb_TipologiaRitrovamento.Items.Add(New ListItem("Tutti", "0"))
        Cmb_TipologiaRitrovamento.Items.Add(New ListItem("Aereoseparatore", "1"))
        Cmb_TipologiaRitrovamento.Items.Add(New ListItem("Cernitrice Ottica", "2"))
        Cmb_TipologiaRitrovamento.Items.Add(New ListItem("Cernita Manuale", "3"))

    End Function


    '##################################################################################
    Private Function CaricaProdotto() As String

        Try

            If Cmb_Specie.SelectedValue <> "" Then

                Dim Veg_Cod As Integer
                Dim DT_MP As DataTable
                Dim objMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                Dim FiltroAggiuntivo As String

                Veg_Cod = Cmb_Specie.SelectedValue

                Select Case Me.Rbl_Biologico.SelectedValue
                    Case -1
                        FiltroAggiuntivo = ""
                    Case enum_Cod_Regolamento.Regolamento_bio
                        FiltroAggiuntivo = " Materie_Prime.Regolamento = 4 "
                    Case 0
                        FiltroAggiuntivo = " Materie_Prime.Regolamento <> 4 "
                    Case Else
                        FiltroAggiuntivo = ""
                End Select

                DT_MP = objMateriePrime.Leggi("", 0,
                                              TRASFORMATI_VEGETALI,
                                              0, "", Veg_Cod, 0, 0, 0, 0, 0, 0, "", 0, "", True, False, "",
                                              enumSelezioneVariabile.Selezione_JoinCompleta,
                                              FiltroAggiuntivo, "", objParametri_Server)

                If DT_MP.Rows.Count > 0 Then
                    CaricaListControl.Prodotto(CType(Me.Cmb_Prodotto, ListControl),
                                               True, "", "0",
                                               DT_MP, "", "", objParametri_Server)

                    'If me.Cmb_Prodotto.Items.Count > 1 Then
                    '   me.Cmb_Prodotto.SelectedIndex = 1
                    'End If
                    Return True
                Else
                    Me.Cmb_Prodotto.Items.Clear()
                    Me.Cmb_Prodotto.Enabled = False
                End If

            End If

            Return False

        Catch ex As Exception
            Messaggi.AgroMsgBox("Caricamento Specie. Si è verificato il seguente errore: " & ex.Message, Page)
            Return False
        End Try

    End Function


    '##################################################################################
    Private Function CaricaSpecie() As Boolean

        Try

            'filtro le orticole, visto che il monitoraggio viene fatto sulle orticole a foglia
            CaricaListControl.SpecieVegetale_Optimize(CType(Cmb_Specie, ListControl),
                                                      True, "Nessuna Specie", "0",
                                                      3, "", True, 0, 0, 0,
                                                      "", "",
                                                      objParametri_Server, objParametri_Utenti)
            If Cmb_Specie.Items.Count = 0 Then
                Throw New Exception("Errore Caricamento Specie")
            End If
            Return True

        Catch ex As Exception
            Messaggi.AgroMsgBox("Errore Caricamento Specie: " & ex.Message, Page)
            Return False
        End Try

    End Function


    '##################################################################################
    Private Sub CaricaSpecieVegetaliMonitoraggioCE()

        Try

            Cmb_Specie.Items.Add(New ListItem("Nessuna Specie", "0"))
            Cmb_Specie.Items.Add(New ListItem("Bietola da coste", 69))
            Cmb_Specie.Items.Add(New ListItem("Bietola da foglia (da taglio)", 107))
            Cmb_Specie.Items.Add(New ListItem("Cicoria", 14))
            Cmb_Specie.Items.Add(New ListItem("Cime di Rapa", 5000322))
            Cmb_Specie.Items.Add(New ListItem("Spinacio", 60))

        Catch ex As Exception
            Messaggi.AgroMsgBox("Errore Caricamento Specie: " & ex.Message, Page)
        End Try

    End Sub

    '##################################################################################
    Private Function CaricaAppezzamento() As Boolean

        Try

            Dim ok As Boolean = False

            Dim Piva As String = ""
            If Cmb_Produttore.SelectedValue <> "" AndAlso Cmb_Produttore.SelectedValue <> "0" Then
                ok = True
                Piva = Cmb_Produttore.SelectedValue
            End If

            Dim Veg_cod As Integer = 0
            If Cmb_Specie.SelectedValue <> "0" Then
                Veg_cod = Cmb_Specie.SelectedValue
            End If

            If ok Then

                Dim objAppezzamento As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                Dim DT_Appezza As DataTable = objAppezzamento.LeggiAppezzamentiDaVegCod(Piva, Veg_cod, enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                                                        "", "", objParametri_Server)

                If DT_Appezza.Rows.Count > 0 Then
                    Cmb_Appezzamento.Enabled = True
                    CaricaListControl.Appezzamento(CType(Cmb_Appezzamento, ListControl),
                                                   True, "", "",
                                                   DT_Appezza, "", "", objParametri_Server)

                    'If Cmb_Appezzamento.Items.Count > 1 Then
                    '    Cmb_Appezzamento.SelectedIndex = 1
                    'End If

                    Return True
                Else
                    Cmb_Appezzamento.Items.Clear()
                    Cmb_Appezzamento.Enabled = False
                End If
            End If
            Cmb_Appezzamento.Items.Clear()
            Cmb_Appezzamento.Enabled = False
            Return False

        Catch ex As Exception
            Messaggi.AgroMsgBox("Caricamento Prodotto. Si è verificato il seguente errore: " & ex.Message, Page)
            Return False
        End Try

    End Function


    '##################################################################################
    Private Sub Btn_Carica_Produttore_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Carica_Produttore.Click

        Dim Risultato As Boolean

        Risultato = CaricaProduttore(Me.Txt_FiltroRagSoc_Produttore.Text)

        If Not Risultato Then
            Messaggi.AgroMsgBox("Non è stato trovato alcun produttore con il criterio di filtro impostato.", Page)
            Cmb_Produttore.Items.Clear()
        Else
            CaricaAppezzamento()
        End If

    End Sub

    '##################################################################################
    Private Sub Cmb_Produttore_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Produttore.SelectedIndexChanged

        CaricaAppezzamento()

    End Sub

    '##################################################################################
    Private Sub Cmb_Specie_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Specie.SelectedIndexChanged

        Dim Risultato As Boolean

        If Me.Cmb_Specie.SelectedValue <> "0" Then

            Me.Cmb_Prodotto.Enabled = True
            'carico il prodotto
            Risultato = CaricaProdotto()

            If Not Risultato Then
                'AgroMsgBox("Non è stato trovato alcun prodotto con il criterio di filtro impostato.", Page)
                Me.Cmb_Prodotto.Items.Clear()
                Me.Cmb_Prodotto.Enabled = False
            End If

        Else
            Me.Cmb_Prodotto.Items.Clear()
            Me.Cmb_Prodotto.Enabled = False

            'Me.Cmb_Appezzamento.Items.Clear()
            'Me.Cmb_Appezzamento.Enabled = False
        End If

        CaricaAppezzamento()

    End Sub

    '##################################################################################
    Private Sub Rbl_Biologico_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rbl_Biologico.SelectedIndexChanged
        CaricaProdotto()
    End Sub

    '##############################################################
    Private Sub ID_Trova_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Trova.Click
        Toolbar_Carica()
    End Sub

    '##############################################################
    Private Sub ID_Nuovo_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Nuovo.Click
        Toolbar_Nuovo()
    End Sub

    '##############################################################
    Private Sub ID_Modifica_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Modifica.Click
        Toolbar_Modifica()
    End Sub

    '##############################################################
    Private Sub ID_Informazioni_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Informazioni.Click
        Toolbar_Info()
    End Sub

    '##############################################################
    Private Sub ID_Azzera_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Azzera.Click
        Toolbar_Reset()
    End Sub

    '##############################################################
    Private Sub ID_Stampa_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Stampa.Click
        Toolbar_Stampa(enum_TipoStampa.Stampa)
    End Sub

    '##############################################################
    Private Sub ID_StampaAggregata_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_StampaAggregata.Click
        Toolbar_Stampa(enum_TipoStampa.StampaAggregata)
    End Sub

    '##############################################################
    Private Sub ID_Insetto_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ID_Insetto.Click
        Toolbar_GestioneCE()
    End Sub


    '##############################################################
    Private Sub btn_delete_riga_Click(sender As Object, e As System.EventArgs) Handles btn_delete_riga.Click

        If VerificaCancellazione() Then
            CancellaMonitoraggioCE()
        End If

    End Sub

    '####################################################################################
    Private Function VerificaCancellazione() As Boolean

        Dim bRet As Boolean = True
        Dim k As Integer
        Dim j As Integer = 0

        For i As Integer = 0 To DataGrid_CE.Rows.Count - 1
            If CType(DataGrid_CE.Rows(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True Then
                j += 1
                k = i
            End If
        Next

        If j > 1 Then
            Messaggi.AgroMsgBox("Selezionare un elemento per volta!", Page)
            bRet = False
        End If

        Return bRet

    End Function



    '####################################################################################
    Private Sub CancellaMonitoraggioCE()

        'per ogni riga che è attiva effettuo la cancellazione 
        Dim i As Integer

        Dim ogjAgenda As New AgronicaCoreContabDAL.Agenda_W
        Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_W
        Dim objMovimentiDettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_W

        Dim Errore As Boolean = False
        Dim StrDummy As String

        '------------------------------------------------
        '----- apro connessione e transazione
        '------------------------------------------------
        AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

        Try
            For i = 0 To DataGrid_CE.Rows.Count - 1
                If CType(DataGrid_CE.Rows(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True Then

                    objMovimentiDettagli.Cancella(DataGrid_CE.DataKeys(i).Item(0),
                                                  DataGrid_CE.DataKeys(i).Item(1),
                                                  DataGrid_CE.DataKeys(i).Item(2),
                                                  DataGrid_CE.DataKeys(i).Item(3),
                                                  0,
                                                  "",
                                                  objParametri_Server)

                    objMovimenti.Cancella(DataGrid_CE.DataKeys(i).Item(0),
                                          DataGrid_CE.DataKeys(i).Item(1),
                                          DataGrid_CE.DataKeys(i).Item(2),
                                          DataGrid_CE.DataKeys(i).Item(3),
                                          "",
                                          objParametri_Server)

                    ogjAgenda.Cancella(DataGrid_CE.DataKeys(i).Item(0),
                                       DataGrid_CE.DataKeys(i).Item(1),
                                       DataGrid_CE.DataKeys(i).Item(2),
                                       "",
                                       objParametri_Server)
                End If

            Next

            'chiudo la transazione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            'chiudo la connessione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)


        Catch exc As Exception
            Errore = True
            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------
            'chiudo la transazione con il rollback
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            'chiudo la connessione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            'Messaggio di errore
            StrDummy = exc.Message.ToString()

            'FACCIO APPARIRE UN ALERT......
            Messaggi.AgroMsgBox("Si è verificato un errore durante la fase di cancellazione: " & vbCrLf & StrDummy, Page)
            '------------------------------------------------
        End Try

        Toolbar_Carica()

        Me.SI_NO.Value = "0"

    End Sub


    '####################################################################################
    Private Sub Toolbar_GestioneCE()

        Dim QueryString As String = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server)

        Dim StrWindowOpen As String = Page_ModalDialog_Script("GestioneCE.aspx", QueryString, "",
                                                              700, 1000, 0, 0,
                                                              NomeForm:="FORM1")

        ScriptManager.RegisterClientScriptBlock(ID_Insetto, ID_Insetto.GetType(),
                                                String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, False)

    End Sub

    '####################################################################################
    Private Sub Toolbar_Nuovo()

        Dim QueryString As String = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server)
        Response.Redirect("./MonitoraggioCE_Edit.aspx" & QueryString)

    End Sub



    '####################################################################################
    Private Sub Toolbar_Modifica()

        ' controllo se è stata selezionata solamente una casella
        Dim j As Integer = 0
        Dim k As Integer

        For i As Integer = 0 To DataGrid_CE.Rows.Count - 1
            If CType(DataGrid_CE.Rows(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True Then
                j += 1
                k = i
            End If
        Next
        If j <> 1 Then
            Messaggi.AgroMsgBox("E' necessario selezionare una sola registrazione!", Page)
            Exit Sub
        End If

        Dim p As String = DataGrid_CE.DataKeys(k).Item(0)
        Dim saCod As Integer = DataGrid_CE.DataKeys(k).Item(1)
        Dim idAgenda As Integer = DataGrid_CE.DataKeys(k).Item(2)
        Dim IdMov As Integer = DataGrid_CE.DataKeys(k).Item(3)

        'Il centro e/o magazzino del documento selezionato sarebbe recuperabile anche da dentro la pagina di edit
        'ma visto che devo considerare il doppio metodo di scrittura e qui ce l'ho già a portata di mano
        'lo passo direttamente in query string
        Dim Stab_SaCod As Integer = DataGrid_CE.DataKeys(k).Item(4)
        Dim Stab_FabbrCod As Integer = DataGrid_CE.DataKeys(k).Item(5)

        Dim QueryString As String = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                                    "&saCod=" & Stringa_Codifica(saCod, AgroKey_EncoderDecoder, Server) &
                                    "&idAgenda=" & Stringa_Codifica(idAgenda, AgroKey_EncoderDecoder, Server) &
                                    "&idMov=" & Stringa_Codifica(IdMov, AgroKey_EncoderDecoder, Server) &
                                    "&stabSaCod=" & Stringa_Codifica(Stab_SaCod, AgroKey_EncoderDecoder, Server) &
                                    "&stabFabbr=" & Stringa_Codifica(Stab_FabbrCod, AgroKey_EncoderDecoder, Server) &
                                    "&act=" & Stringa_Codifica("mod", AgroKey_EncoderDecoder, Server)

        Response.Redirect("./MonitoraggioCE_Edit.aspx" & QueryString)

    End Sub


    '####################################################################################
    Private Sub Toolbar_Reset()

        Try

            'resetto i parametri di ricerca
            Me.Txt_DataFine.Text = ""
            Me.Txt_DataInizio.Text = ""
            Me.Txt_FiltroRagSoc_Produttore.Text = ""

            If Me.Cmb_Appezzamento.Items.Count > 0 Then
                Me.Cmb_Appezzamento.SelectedIndex = 0
            End If

            If Me.Cmb_Prodotto.Items.Count > 0 Then
                Me.Cmb_Prodotto.SelectedIndex = 0
            End If

            If Me.Cmb_Produttore.Items.Count > 0 Then
                Me.Cmb_Produttore.SelectedIndex = 0
            End If

            Me.Cmb_Specie.SelectedIndex = 0

            DataGrid_CE.DataSource = Nothing
            DataGrid_CE.DataBind()

            Lbl_NumModuliCarico.Visible = False
            Lbl_NumModuliCarico.Text = "Numero Moduli di Carico trovati: "

        Catch exc As Exception
            'FACCIO APPARIRE UN ALERT......
            Messaggi.AgroMsgBox("Si è verificato un errore durante il reset dei filtri: " & vbCrLf & exc.Message.ToString(), Page)
            '------------------------------------------------
        End Try
    End Sub


    '####################################################################################
    Private Sub Toolbar_Info()

        ' controllo se è stata selezionata solamente una casella
        Dim j As Integer = 0
        Dim k As Integer

        For i As Integer = 0 To DataGrid_CE.Rows.Count - 1
            If CType(DataGrid_CE.Rows(i).FindControl("ChkSelezionaOperazione"), CheckBox).Checked = True Then
                j += 1
                k = i
            End If
        Next
        If j <> 1 Then
            Messaggi.AgroMsgBox("E' necessario selezionare una sola registrazione!", Page)
            Exit Sub
        End If

        Dim p As String = DataGrid_CE.DataKeys(k).Item(0)
        Dim saCod As Integer = DataGrid_CE.DataKeys(k).Item(1)
        Dim idAgenda As Integer = DataGrid_CE.DataKeys(k).Item(2)
        Dim IdMov As Integer = DataGrid_CE.DataKeys(k).Item(3)

        'Il centro e/o magazzino del documento selezionato sarebbe recuperabile anche da dentro la pagina di edit
        'ma visto che devo considerare il doppio metodo di scrittura e qui ce l'ho già a portata di mano
        'lo passo direttamente in query string
        Dim Stab_SaCod As Integer = DataGrid_CE.DataKeys(k).Item(4)
        Dim Stab_FabbrCod As Integer = DataGrid_CE.DataKeys(k).Item(5)

        Dim QueryString As String = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                                    "&saCod=" & Stringa_Codifica(saCod, AgroKey_EncoderDecoder, Server) &
                                    "&idAgenda=" & Stringa_Codifica(idAgenda, AgroKey_EncoderDecoder, Server) &
                                    "&idMov=" & Stringa_Codifica(IdMov, AgroKey_EncoderDecoder, Server) &
                                    "&stabSaCod=" & Stringa_Codifica(Stab_SaCod, AgroKey_EncoderDecoder, Server) &
                                    "&stabFabbr=" & Stringa_Codifica(Stab_FabbrCod, AgroKey_EncoderDecoder, Server) &
                                    "&act=" & Stringa_Codifica("info", AgroKey_EncoderDecoder, Server)

        Response.Redirect("./MonitoraggioCE_Edit.aspx" & QueryString)

    End Sub


    '####################################################################################
    Private Sub Toolbar_Carica()

        Try

            Dim DT As DataTable
            Dim xFiltroAggiuntivo As String = ""

            Dim Flag_Appezza As Boolean = False
            Dim Piva_App As String
            Dim Sa_Cod_App As Integer
            Dim Appezza_App As Integer
            Dim Id_Reg As Integer


            Dim dal As Date = Estremo_Validita_Inizio
            If Txt_DataInizio.Text <> "" Then
                dal = Txt_DataInizio.Text
            End If

            Dim al As Date = Estremo_Validita_Fine
            If Txt_DataFine.Text <> "" Then
                al = Txt_DataFine.Text
            End If

            Dim Piva_Produttore As String = Cmb_Produttore.SelectedValue

            Dim Specie As Integer = 0
            If (Cmb_Specie.SelectedValue <> "0") Then
                Specie = Cmb_Specie.SelectedValue
            End If

            Dim Prodotto As Integer = 0
            If (Cmb_Prodotto.SelectedValue <> "") Then
                Prodotto = Me.Cmb_Prodotto.SelectedValue
            End If

            If Cmb_Appezzamento.SelectedValue <> "" Then
                Piva_App = Cmb_Appezzamento.SelectedValue.Split("|")(0)
                Sa_Cod_App = Cmb_Appezzamento.SelectedValue.Split("|")(1)
                Appezza_App = Cmb_Appezzamento.SelectedValue.Split("|")(2)
                Id_Reg = Cmb_Appezzamento.SelectedValue.Split("|")(3)
                Flag_Appezza = True
            End If

            Dim Tipologia As Integer = Cmb_TipologiaRitrovamento.SelectedValue
            Dim Pericolosita As Integer = Rbl_Pericolosita.SelectedValue

            Dim Sa_Cod As Integer = Me.Cmb_Magazzino.SelectedValue.Split("|")(1)
            Dim Fabbricato_Cod As Integer = Me.Cmb_Magazzino.SelectedValue.Split("|")(0)


            Select Case Me.Rbl_Biologico.SelectedValue
                Case -1
                    xFiltroAggiuntivo = ""
                Case enum_Cod_Regolamento.Regolamento_bio
                    xFiltroAggiuntivo = " Materie_Prime.Regolamento = 4 "
                Case 0
                    xFiltroAggiuntivo = " Materie_Prime.Regolamento <> 4 "
                Case Else
                    xFiltroAggiuntivo = ""
            End Select

            Dim objCE_R As New AgronicaCoreAnagrafeDAL.CorpiEstranei_R
            DT = objCE_R.LeggiCorpiEstraneiBolle(True,
                                                 Piva_Produttore,
                                                 dal, al,
                                                 Specie,
                                                 Prodotto,
                                                 Flag_Appezza,
                                                 Sa_Cod_App, Appezza_App, Id_Reg,
                                                 Tipologia, Pericolosita,
                                                 Sa_Cod,
                                                 Fabbricato_Cod,
                                                 xFiltroAggiuntivo, "",
                                                 objParametri_Server)

            'Vettore di DataColumn
            Dim DtKeysCE(5) As String
            DtKeysCE(0) = "Piva"
            DtKeysCE(1) = "Sa_Cod"
            DtKeysCE(2) = "Id_Agenda"
            DtKeysCE(3) = "Id_Mov"
            DtKeysCE(4) = "Stabilimento_Sa_Cod"
            DtKeysCE(5) = "Stabilimento_Fabbr_Cod"
     
            DataGrid_CE.DataSource = DT
            DataGrid_CE.DataKeyNames = DtKeysCE
            DataGrid_CE.DataBind()

            If DT IsNot Nothing Then
                Lbl_NumModuliCarico.Visible = True
                Lbl_NumModuliCarico.Text = "Numero Moduli di Carico trovati: " & CStr(DT.Rows.Count)
            End If

        Catch exc As Exception
            'FACCIO APPARIRE UN ALERT......
            Messaggi.AgroMsgBox("Si è verificato un errore durante la fase di caricamento: " & vbCrLf & exc.Message.ToString(), Page)
            '------------------------------------------------
        End Try
    End Sub

    Private Enum enum_TipoStampa

        Stampa = 1
        StampaAggregata = 2

    End Enum



    '####################################################################################
    Private Sub Toolbar_Stampa(ByVal Tipo_Stampa As enum_TipoStampa)

        'creo gli oggetti in sessione
        If Txt_DataInizio.Text <> "" Then
            Session("dal") = Txt_DataInizio.Text
        Else
            Session("dal") = Estremo_Validita_Inizio
        End If

        If Txt_DataFine.Text <> "" Then
            Session("al") = Txt_DataFine.Text
        Else
            Session("al") = Estremo_Validita_Fine
        End If

        Session("piva_produttore") = Cmb_Produttore.SelectedValue

        If (Cmb_Specie.SelectedValue <> "0") Then
            Session("veg_cod") = Cmb_Specie.SelectedValue
        Else
            Session("veg_cod") = 0
        End If

        If (Cmb_Prodotto.SelectedValue <> "") Then
            Session("mat_cod") = Me.Cmb_Prodotto.SelectedValue
        Else
            Session("mat_cod") = 0
        End If

        If Cmb_Appezzamento.SelectedValue <> "" Then
            Session("sa_cod") = Cmb_Appezzamento.SelectedValue.Split("|")(1)
            Session("appezza") = Cmb_Appezzamento.SelectedValue.Split("|")(2)
            Session("id_reg") = Cmb_Appezzamento.SelectedValue.Split("|")(3)
            Session("flag_appezza") = True
        Else
            Session("sa_cod") = 0
            Session("appezza") = 0
            Session("id_reg") = 0
            Session("flag_appezza") = False
        End If

        Session("tipologia") = Cmb_TipologiaRitrovamento.SelectedValue
        Session("pericolosita") = Rbl_Pericolosita.SelectedValue
        Session("regolamento") = Me.Rbl_Biologico.SelectedValue

        Session("sa_cod_fabbr") = Me.Cmb_Magazzino.SelectedValue.Split("|")(1)
        Session("fabbr_cod") = Me.Cmb_Magazzino.SelectedValue.Split("|")(0)

        Dim XmlParametri As String = ""

        Dim report As Integer = enum_CodificaStampe.ExportExcel_MonitoraggioCE
        Select Case Tipo_Stampa

            Case enum_TipoStampa.Stampa

                report = enum_CodificaStampe.ExportExcel_MonitoraggioCE

            Case enum_TipoStampa.StampaAggregata

                report = enum_CodificaStampe.ExportExcel_MonitoraggioCE_Aggregata

        End Select

        '-----------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------
        Dim XmlDoc As New XmlDocument
        Dim Xml_FiltroStampa As XmlElement
        Xml_FiltroStampa = XmlDoc.CreateElement("FiltroStampa")



        Dim StrVariabiliStampe As String = ""
        Dim StrNodiVariabili As String = ""
        Dim StrNodo As String = ""
        Dim vVarStampe(13) As ElementoStampe

        vVarStampe(0).Nome = "dal"
        vVarStampe(0).Valore = Session("dal")

        vVarStampe(1).Nome = "al"
        vVarStampe(1).Valore = Session("al")

        vVarStampe(2).Nome = "piva_produttore"
        vVarStampe(2).Valore = Session("piva_produttore")

        vVarStampe(3).Nome = "veg_cod"
        vVarStampe(3).Valore = Session("veg_cod")

        vVarStampe(4).Nome = "mat_cod"
        vVarStampe(4).Valore = Session("mat_cod")

        vVarStampe(5).Nome = "flag_appezza"
        vVarStampe(5).Valore = Session("flag_appezza")

        vVarStampe(6).Nome = "sa_cod"
        vVarStampe(6).Valore = Session("sa_cod")

        vVarStampe(7).Nome = "appezza"
        vVarStampe(7).Valore = Session("appezza")

        vVarStampe(8).Nome = "id_reg"
        vVarStampe(8).Valore = Session("id_reg")

        vVarStampe(9).Nome = "tipologia"
        vVarStampe(9).Valore = Session("tipologia")

        vVarStampe(10).Nome = "pericolosita"
        vVarStampe(10).Valore = Session("pericolosita")

        vVarStampe(11).Nome = "regolamento"
        vVarStampe(11).Valore = Session("regolamento")

        vVarStampe(12).Nome = "sa_cod_fabbr"
        vVarStampe(12).Valore = Session("sa_cod_fabbr")

        vVarStampe(13).Nome = "fabbr_cod"
        vVarStampe(13).Valore = Session("fabbr_cod")

        ' todo
        StrNodo = XML_VariabiliStampe(vVarStampe)


        Xml_FiltroStampa.SetAttribute("username", Session("ASG_Utente_Username").ToString)
        Xml_FiltroStampa.SetAttribute("username_codfisc", Session("ASG_Utente_CodFiscale").ToString)
        Xml_FiltroStampa.SetAttribute("report", report)
        Xml_FiltroStampa.SetAttribute("user_profilo", Session("ASG_SuperUser_Username").ToString)
        Xml_FiltroStampa.SetAttribute("superuser_codfiscale", Session("ASG_SuperUser_CodFiscale").ToString)

        'metto a nothing gli oggetti di sessione
        Session("dal") = Nothing
        Session("al") = Nothing
        Session("piva_produttore") = Nothing
        Session("veg_cod") = Nothing
        Session("mat_cod") = Nothing
        Session("flag_appezza") = Nothing
        Session("sa_cod") = Nothing
        Session("appezza") = Nothing
        Session("id_reg") = Nothing
        Session("tipologia") = Nothing
        Session("pericolosita") = Nothing
        Session("regolamento") = Nothing


        'Rendo l'albero figlio del documento
        XmlDoc.AppendChild(Xml_FiltroStampa)

        'Inserisco l'XML nella stringa complessiva
        StrNodiVariabili = StrNodiVariabili & StrNodo

        'Inserisco gli elementi "VariabiliStampe" come figli del nodo "FiltroStampa"
        Xml_FiltroStampa.InnerXml = StrNodiVariabili

        'Estraggo la stringa XML complessiva
        XmlParametri = XmlDoc.InnerXml

        XmlParametri = XmlParametri.Replace("'", Chr(34))

        Dim strOpen As String = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamenteIParametri(
                                         Enum_SiteRedirector.Sito_GiasOnline,
                                         report,
                                         CStr(Session("ASG_Utente_Username")),
                                         CStr(Session("ASG_SuperUser_CodFiscale")),
                                         StrNodiVariabili,
                                         "",
                                         "",
                                         "",
                                         "",
                                         "",
                                         "",
                                         "")

        Page.Master.FindControl("FORM1").Controls.Add(New LiteralControl(strOpen))

        '-----------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------

    End Sub

End Class
