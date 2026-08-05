Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Partial Class Filtro_ElaboratiContabili
    Inherits System.Web.UI.Page

#Region " Filtro_ElaboratiContabili "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    Private Report As Integer
    Private Qs_Piva, Qs_Rag_Soc, Qs_Anno, Qs_Di, Qs_Df As String
    Private Qs_Esercizio As String
    Private Qs_Chk_CE As Integer = 0
    Private Qs_Chk_SP As Integer = 0
    Private Qs_Ric_Cod_Eco As Integer = 0
    Private Qs_Ric_Cod_Pat As Integer = 0
    Private Qs_Cod_Conto_Eco As Integer = 0
    Private Qs_Cod_Conto_Pat As Integer = 0
    Private Qs_Cod_RisUm As Integer = 0
    Private Qs_Cod_Liquidita As Integer = 0
    


    'Private GestCont_Flag_ConsideraSaldiIniziali As Boolean = True
    'Private GestCont_DataInizio As Date = AGRODATAINIZIO

    Const NUM_COLONNE_ALIQUOTE_REG_CORRISPETTIVI As Integer = 4
    Const NUM_COLONNE_NONIMP_REG_CORRISPETTIVI As Integer = 2
    Const NUM_COLONNE_ESCLIVA_REG_CORRISPETTIVI As Integer = 2

    Private _objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    '##################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Faccio scadere subito la pagina memorizzata nella cache
        Response.Expires = 0

        Try

            '##############################################################
            '#####  Verifico Credenziali di Accesso  ######################
            '##############################################################

            '----- Verifico che l'utente sia autenticato

            'If IsNothing(Session("ASG_Utente_Username")) Or Session("ASG_Utente_Username") = "" Then
            '    Me.FindControl("Form1").Controls.Add(New LiteralControl("<script language='javascript'>window.close()</script>"))
            '    Exit Sub
            'End If

            If Session("ASG_objParametri_Server") Is Nothing Then
                Response.Redirect("~/Custom500.aspx")
            End If

            '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina
            Dim UtenteAbilitato As Boolean
            '  Dim strDummy As String      'controllo accesso negato.....

            'UtenteAbilitato = Controlla_Permessi_Utente_2(Server, Session, Page,
            '                            Session("ASG_Utente_Username"),
            '                            Session("ASG_IdServizio"),
            '                            enum_Security_Attivita.Stampe_Contabilita,
            '                            enum_Security_Operazione.Lettura,
            '                            strDummy)


            '----- !!!!!!!!!!! -------------

            'Attivazione forzata provvisoria

            UtenteAbilitato = True

            '----- !!!!!!!!!!! -------------

            '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio ad AlberoImprese.

            If UtenteAbilitato = False Then
                Me.FindControl("Form1").Controls.Add(New LiteralControl("<script language='javascript'> window.close() </script>"))
                Exit Sub
            End If


            '##############################################################
            '###################### QUERYSTRING ###########################
            '##############################################################


            Report = Stringa_Decodifica(Request.QueryString("r").ToString, AgroKey_EncoderDecoder, Server)

            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, AgroKey_EncoderDecoder, Server)

            Qs_Rag_Soc = Stringa_Decodifica(Request.QueryString("rs").ToString, AgroKey_EncoderDecoder, Server)

            Qs_Anno = Stringa_Decodifica(Request.QueryString("a").ToString, AgroKey_EncoderDecoder, Server)
            If Qs_Anno = "" Or Qs_Anno = "0" Then
                Qs_Anno = Date.Today.Year.ToString
            End If

            If Not Qs_Esercizio Is Nothing Then
                Qs_Esercizio = Stringa_Decodifica(Request.QueryString("es").ToString, AgroKey_EncoderDecoder, Server)
            Else
                Qs_Esercizio = 0
            End If

            Qs_Di = Stringa_Decodifica(Request.QueryString("di").ToString, AgroKey_EncoderDecoder, Server)

            Qs_Df = Stringa_Decodifica(Request.QueryString("df").ToString, AgroKey_EncoderDecoder, Server)

            Qs_Chk_CE = Stringa_Decodifica(CStr(Request.QueryString("chk_ce")), AgroKey_EncoderDecoder, Server)

            Qs_Chk_SP = Stringa_Decodifica(CStr(Request.QueryString("chk_sp")), AgroKey_EncoderDecoder, Server)

            Qs_Ric_Cod_Eco = Stringa_Decodifica(CStr(Request.QueryString("rce")), AgroKey_EncoderDecoder, Server)

            Qs_Cod_Conto_Eco = Stringa_Decodifica(CStr(Request.QueryString("cce")), AgroKey_EncoderDecoder, Server)

            Qs_Ric_Cod_Pat = Stringa_Decodifica(CStr(Request.QueryString("rcp")), AgroKey_EncoderDecoder, Server)

            Qs_Cod_Conto_Pat = Stringa_Decodifica(CStr(Request.QueryString("ccp")), AgroKey_EncoderDecoder, Server)

            Qs_Cod_RisUm = CInt(Stringa_Decodifica(CStr(Request.QueryString("cru")), AgroKey_EncoderDecoder, Server))

            Qs_Cod_Liquidita = CInt(Stringa_Decodifica(CStr(Request.QueryString("liq")), AgroKey_EncoderDecoder, Server))



            If IsNothing(Session("ASG_objParametri_Server")) Then
                'AgroMsgBox("Sessione scaduta", Page)
                Me.FindControl("Form1").Controls.Add(New LiteralControl("<script language='javascript'> window.close() </script>"))
                Exit Sub
            End If


            _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            _objParametriUtenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


            If Me.IsPostBack Then

                Select Case Me.Txt_FiltroConti.Value

                    Case "", "undefined" 'NO
                        If Me.Txt_FiltroConti.Value.ToLower = "undefined" Then
                            Me.Txt_FiltroConti.Value = ""
                        End If
                        'nessun filtro
                        Me.Chk_Conti.Checked = False

                    Case Else 'SI

                        'imposta filtro sui conti
                        Me.Chk_Conti.Checked = True
                        'Me.Txt_FiltroConti.Text = ""

                End Select


                Exit Sub


            End If


            '----------------------------------------
            '------ inizio gestione contabilità -------
            '----------------------------------------
            Dim GestCont_Flag_ConsideraSaldiIniziali As Boolean '= True
            Dim GestCont_DataInizio As Date '= AGRODATAINIZIO

            'Lettura parametri di inizio gestione contabile
            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim strerr As String = ""
            objImprese.LeggiOpzioni_InizioGestioneContabile(_objParametriServer,
                                                            Qs_Piva,
                                                            GestCont_Flag_ConsideraSaldiIniziali,
                                                            GestCont_DataInizio,
                                                            strerr)

            ViewState("GestCont_DataInizio") = GestCont_DataInizio
            ViewState("GestCont_Flag_ConsideraSaldiIniziali") = GestCont_Flag_ConsideraSaldiIniziali

            If strerr <> "" Then
                AgroMsgBox(strerr, Page)
            End If


            '----------------------------------------
            '------ Stampa diretta del mastrino-------
            '----------------------------------------
            If Report = enum_CodificaStampe.Mastrino And (Qs_Chk_CE = 1 Or Qs_Chk_SP = 1) Then
                'caso di chiamata alla stampa dalla form partitadoppia del giaslan
                'c'è un conto selezionato e si richiede direttamente la stampa del mastrino
                '(non bisogna passare dal filtro)
                Disattiva_Pannelli()
                Me.ImgBtnStampa.Enabled = False
                Me.ImgBtnStampa.Visible = False
                Me.ImgBtnPDF.Enabled = False
                Me.ImgBtnPDF.Visible = False
                Me.Table_Generale.Visible = False
                Me.Pannello_stampa.Visible = False
                Me.LblTitolo.Text = "stampa mastrino aperta in altra tab del browser"

                StampaDirettaMastrino()

                Exit Sub
            End If


            Configura_Pannelli()

            Me.Txt_Data.Text = Date.Today


        Catch ex As Exception
            AgroMsgBox("PageLoad: " & ex.Message, Page)
        End Try

    End Sub


    '##############################################################
    Private Sub ImgBtnEsci_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnEsci.Click
        Me.FindControl("Form1").Controls.Add(New LiteralControl("<script language='javascript'>window.close()</script>"))
    End Sub

    Private Sub Attiva_StampaDiProvaDefinitiva()
        Me.Table_StampaProvaDefinitiva.Visible = True
    End Sub
    Private Sub Attiva_Riga_TipoPianoConti()
        Me.Riga0.Visible = True
        Me.Table0.Visible = True
    End Sub
    Private Sub Attiva_Riga_Date()
        Me.Riga1.Visible = True
        Me.Table_Temporale.Visible = True
    End Sub
    Private Sub Attiva_Riga_Data()
        Me.Riga1b.Visible = True
        Me.Table_Data.Visible = True
    End Sub
    Private Sub Attiva_Riga_Sezionali()
        Me.Riga2.Visible = True
        Me.Table_Sezionali.Visible = True
    End Sub
    Private Sub Attiva_Riga_Contatti()
        Me.Riga3.Visible = True
        Me.Table_Contatti.Visible = True
    End Sub
    Private Sub Attiva_Riga_Contatti2()
        Me.Riga3bis.Visible = True
        Me.Table_Contatti2.Visible = True
    End Sub
    Private Sub Attiva_Riga_IstCredito_Contatti()
        Me.Riga4.Visible = True
        Me.Table_IstitutoCreditoCliente.Visible = True
    End Sub
    Private Sub Attiva_Riga_IstCredito()
        Me.Riga5.Visible = True
        Me.Table_IstitutoCredito.Visible = True
    End Sub
    Private Sub Attiva_Riga_RisFinanza()
        Me.Riga5b.Visible = True
        Me.Table_RisFinanza.Visible = True
    End Sub
    Private Sub Attiva_Riga_Agenti()
        Me.Riga6.Visible = True
        Me.Table_Agenti.Visible = True
    End Sub
    Private Sub Attiva_Riga_RegIVA()
        Me.Riga7.Visible = True
        Me.Table_RegistriIVA.Visible = True
    End Sub
    Private Sub Attiva_Riga_Corrispettivi()
        'Me.Riga8.Visible = True
        'Me.Table_VerificaCorrispettivi.Visible = True
        Me.Riga2B.Visible = True
        Me.TableNoteCorrispettivi.Visible = True
    End Sub
    Private Sub Attiva_Riga_NumPagina()
        Me.Riga9.Visible = True
        Me.Table_NumPagina.Visible = True
    End Sub
    Private Sub Attiva_Riga_NumRiga()
        Me.riga9b.Visible = True
        Me.Table_NumRiga.Visible = True
    End Sub
    Private Sub Attiva_Riga_RibaFatture()
        Me.Riga10.Visible = True
        Me.Table_RibaFatture.Visible = True
    End Sub
    Private Sub Attiva_Riga_Riscossioni()
        Me.Riga11.Visible = True
        Me.Table_Riscossioni.Visible = True
    End Sub
    Private Sub Attiva_Riga_Scadenza()
        Me.Riga12.Visible = True
        Me.Table_Scadenza.Visible = True
    End Sub
    Private Sub Attiva_Riga_TipoPagamento()
        Me.Riga12b.Visible = True
        Me.Tabella12b_TipoPagamento.Visible = True
    End Sub
    Private Sub Attiva_Riga_NumeriDoc()
        Me.Riga13.Visible = True
        Me.Table_NumeriDoc.Visible = True
    End Sub
    Private Sub Attiva_Riga_Ordinamento()
        Me.Riga14.Visible = True
        Me.Table_Ordinamento.Visible = True
    End Sub
    Private Sub Attiva_Riga_Bilancio()
        Me.Riga15a.Visible = True
        Me.Table_Bilancio.Visible = True
    End Sub
    Private Sub Attiva_Riga_ContiUE()
        Me.Riga15b.Visible = True
        Me.Table_ContiUE.Visible = True
    End Sub
    Private Sub Attiva_Riga_BilancioSinteticoAnalitico()
        Me.Riga15c.Visible = True
        Me.Table_SinteticoAnalitico.Visible = True
    End Sub
    Private Sub Attiva_Riga_ContiMovimenti()
        Me.Riga16.Visible = True
        Me.Table_ContiMovimenti.Visible = True
    End Sub
    Private Sub Attiva_Riga_CE_Layout()
        Me.Riga16a.Visible = True
        Me.Table_CE_Layout.Visible = True
    End Sub
    Private Sub Attiva_Riga_Saldo0()
        Me.Riga16b.Visible = True
        Me.Table_SaldoConti0.Visible = True
    End Sub
    Private Sub Attiva_Riga_No_Saldo0()
        Me.Riga16ab.Visible = True
        Me.Table_No_Saldo0.Visible = True
    End Sub
    Private Sub Attiva_Riga_Dettagli_SP()
        Me.TrDettagliAuto.Visible = True
        Me.Table_DettagliAuto.Visible = True
    End Sub
    Private Sub Attiva_Riga_AnnoConfronto()
        Me.riga22.Visible = True
        Me.Table_AnnoConfronto.Visible = True
    End Sub
    Private Sub Attiva_Riga_FiltroConti()
        Me.Riga18.Visible = True
        Me.Table_FiltroConti.Visible = True
    End Sub
    Private Sub Attiva_Riga_AnnoConti()
        Me.riga19.Visible = True
        Me.Table_AnnoConti.Visible = True
    End Sub
    Private Sub Attiva_Riga_Esercizio()
        Me.RigaEsercizio.Visible = True
        Me.TableEsercizio2.Visible = True
    End Sub
    Private Sub Attiva_Riga_ContiEco()
        Me.Riga20.Visible = True
        Me.Table_ContiEco.Visible = True
    End Sub
    Private Sub Attiva_Riga_ContiEcoPiuAnni()
        Me.Riga20b.Visible = True
        Me.Table_Conti_PiuAnni.Visible = True
    End Sub
    Private Sub Attiva_Riga_ContiPat()
        Me.riga21.Visible = True
        Me.Table_ContiPat.Visible = True
    End Sub
    Private Sub Attiva_Riga_ConsideraSaldiRiporto()
        Me.Riga1C.Visible = True
        Me.TableConsideraSaldiRip.Visible = True
    End Sub
    Private Sub Attiva_Riga_EscludiIvaIndetraibile()
        Me.Riga1d.Visible = True
        Me.Table1d.Visible = True
    End Sub

    Private Sub Disattiva_Riga_ConsideraSaldiRiporto()
        Me.Riga1C.Visible = False
        Me.TableConsideraSaldiRip.Visible = False
    End Sub

    Private Sub Disattiva_Riga_EscludiIvaIndetraibile()
        Me.Riga1d.Visible = False
        Me.Table1d.Visible = False
    End Sub

    Private Sub Disattiva_Riga_ContiEcoPiuAnni()
        Me.Riga20b.Visible = False
        Me.Table_Conti_PiuAnni.Visible = False
    End Sub
    Private Sub Disattiva_Riga_ContiEco()
        Me.Riga20.Visible = False
        Me.Table_ContiEco.Visible = False
    End Sub
    Private Sub Disattiva_Riga_ContiPat()
        Me.riga21.Visible = False
        Me.Table_ContiPat.Visible = False
    End Sub
    Private Sub Disattiva_Riga_RisFinanza()
        Me.Riga5b.Visible = False
        Me.Table_RisFinanza.Visible = False
    End Sub
    Private Sub Disattiva_Riga_Data()
        Me.Riga1b.Visible = False
        Me.Table_Data.Visible = False
    End Sub
    Private Sub Disattiva_Riga_Saldo0()
        Me.Riga16b.Visible = False
        Me.Table_SaldoConti0.Visible = False
    End Sub
    Private Sub Disattiva_Riga_No_Saldo0()
        Me.Riga16ab.Visible = False
        Me.Table_No_Saldo0.Visible = False
    End Sub
    Private Sub Disattiva_Riga_Dettagli_SP()
        Me.TrDettagliAuto.Visible = False
        Me.Table_DettagliAuto.Visible = False
    End Sub
    Private Sub Disattiva_Riga_Contatti()
        Me.Riga3.Visible = False
        Me.Table_Contatti.Visible = False
    End Sub
    Private Sub Disattiva_Riga_AnnoConti()
        Me.riga19.Visible = False
        Me.Table_AnnoConti.Visible = False
    End Sub

    Private Sub Disattiva_Riga_Esercizio()
        Me.RigaEsercizio.Visible = False
        Me.TableEsercizio2.Visible = False
    End Sub

    Private Sub Attiva_Riga_DataStampa()
        Me.Riga23.Visible = True
        Me.Table_NumPagina.Visible = True
    End Sub
    Private Sub Disattiva_Riga_DataStampa()
        Me.Riga23.Visible = False
        Me.Table_NumPagina.Visible = False
    End Sub

    '##############################################################
    Private Sub Disattiva_Pannelli()

        Me.Table_StampaProvaDefinitiva.Visible = False

        Disattiva_Riga_ContiEcoPiuAnni()
        Disattiva_Riga_No_Saldo0()
        Disattiva_Riga_Dettagli_SP()

        Me.Riga0.Visible = False
        Me.Table0.Visible = False

        Me.Riga1.Visible = False
        Me.Table_Temporale.Visible = False

        Me.Riga1b.Visible = False
        Me.Table_Data.Visible = False

        Me.Riga1C.Visible = False
        Me.TableConsideraSaldiRip.Visible = False

        Me.Riga1d.Visible = False
        Me.Table1d.Visible = False

        Me.Riga2.Visible = False
        Me.Table_Sezionali.Visible = False

        Me.Riga2B.Visible = False
        Me.TableNoteCorrispettivi.Visible = False

        Me.Riga3.Visible = False
        Me.Table_Contatti.Visible = False

        Me.Riga3bis.Visible = False
        Me.Table_Contatti2.Visible = False

        Me.Riga4.Visible = False
        Me.Table_IstitutoCreditoCliente.Visible = False

        Me.Riga5.Visible = False
        Me.Table_IstitutoCredito.Visible = False

        Me.Riga5b.Visible = False
        Me.Table_RisFinanza.Visible = False

        Me.Riga6.Visible = False
        Me.Table_Agenti.Visible = False

        Me.Riga7.Visible = False
        Me.Table_RegistriIVA.Visible = False

        Me.Riga8.Visible = False
        Me.Table_VerificaCorrispettivi.Visible = False

        Me.Riga9.Visible = False
        Me.Table_NumPagina.Visible = False

        Me.riga9b.Visible = False
        Me.Table_NumRiga.Visible = False

        Me.Riga10.Visible = False
        Me.Table_RibaFatture.Visible = False

        Me.Riga11.Visible = False
        Me.Table_Riscossioni.Visible = False

        Me.Riga12.Visible = False
        Me.Table_Scadenza.Visible = False

        Me.Riga12b.Visible = False
        Me.Tabella12b_TipoPagamento.Visible = False

        Me.Riga13.Visible = False
        Me.Table_NumeriDoc.Visible = False

        Me.Riga14.Visible = False
        Me.Table_Ordinamento.Visible = False

        Me.Riga15a.Visible = False
        Me.Table_Bilancio.Visible = False

        Me.Riga15b.Visible = False
        Me.Table_ContiUE.Visible = False

        Me.Riga15c.Visible = False
        Me.Table_SinteticoAnalitico.Visible = False

        Me.Riga16.Visible = False
        Me.Table_ContiMovimenti.Visible = False

        Me.Riga16a.Visible = False
        Me.Table_CE_Layout.Visible = False

        Me.Riga16b.Visible = False
        Me.Table_SaldoConti0.Visible = False

        Me.riga22.Visible = False
        Me.Table_AnnoConfronto.Visible = False

        Me.Riga18.Visible = False
        Me.Table_FiltroConti.Visible = False

        Me.riga19.Visible = False
        Me.Table_AnnoConti.Visible = False

        Me.Riga20.Visible = False
        Me.Table_ContiEco.Visible = False

        Me.riga21.Visible = False
        Me.Table_ContiPat.Visible = False

        Disattiva_Riga_Esercizio()

        Disattiva_Riga_DataStampa()

        'Me.Rbl_TempDISATTIVATA.Visible = False

        'Me.Table_RegistroVuoto.Visible = False
        'With Me.Table_NumPagina
        '    .Style.Item("Top") = "230px"
        '    .Style.Item("Left") = "8px"
        'End With

    End Sub

    '##############################################################
    Private Sub Carica_Agenti()
        'caricamento agenti
        AgronicaCoreUtility.CaricaListControl.Contatti_3(Me.Cmb_Agenti,
                                                         True, "", "",
                                                         Qs_Piva,
                                                         0, 0, 0, 0, 0, 1,
                                                         AGRODATAINIZIO, AGRODATAFINE,
                                                         "", "",
                                                         _objParametriServer)
    End Sub

    '##############################################################
    Private Sub Carica_RisorseFinanziarie(ByVal Cod_contatto As String, ByVal Cau_Risorsa As enum_Liquidita_CauRisorsa)
        '0 ha significato, è la prima cassa
        'le casse create in seguito hanno cod_liquidita > 0
        AgronicaCoreUtility.CaricaListControl.RisorseFinanziarie(Me.Cmb_RisorseFinanza,
                                                                 True, "", "-1",
                                                                 Qs_Piva,
                                                                 Cod_contatto,
                                                                 Cau_Risorsa,
                                                                 "", "",
                                                                 _objParametriServer)

        'selezione default
        Select Case Me.Cmb_RisorseFinanza.Items.Count
            'Case Is = 2
            '    Me.Cmb_RisorseFinanza.SelectedIndex = 1
            'Case Is > 2
            '    Me.Cmb_RisorseFinanza.SelectedIndex = 2
            Case Is >= 2
                Me.Cmb_RisorseFinanza.SelectedIndex = 1
        End Select
    End Sub

    '##############################################################
    Private Sub Carica_Ist_Credito(ByVal Cod_contatto As String)
        AgronicaCoreUtility.CaricaListControl.Ist_Credito(Me.Cmb_IstitutoCredito,
                                                          True, "", "",
                                                          Qs_Piva,
                                                          Cod_contatto,
                                                          "", "",
                                                          _objParametriServer)
    End Sub

    '##############################################################
    Private Sub Carica_Sezionali_FiltroObbligato()
        'Tipo_Value = 6 -> Sezionale_Cod | LiquidazioneIva | ChkDefault
        AgronicaCoreUtility.CaricaListControl.Imprese_Sezionali(Me.Cmb_Sezionali,
                                                                False, "", "",
                                                                6,
                                                                Qs_Piva,
                                                                0,
                                                                "", "",
                                                                _objParametriServer)

    End Sub

    '##############################################################
    Private Sub Carica_Sezionali_FiltroNONObbligato()
        'Tipo_Value = 6 -> Sezionale_Cod | LiquidazioneIva | ChkDefault
        AgronicaCoreUtility.CaricaListControl.Imprese_Sezionali(Me.Cmb_Sezionali,
                                                                True, "Tutti", "-1|-1|1",
                                                                6,
                                                                Qs_Piva,
                                                                0,
                                                                "", "",
                                                                _objParametriServer)

    End Sub

    '##############################################################
    Private Sub Carica_RapportiContabili(ByVal Filtro As String)
        AgronicaCoreUtility.CaricaListControl.RapportiContabili(Me.Cmb_RappContabili,
                                                                True, "Tutti", "",
                                                                0,
                                                                False,
                                                                0,
                                                                False,
                                                                False,
                                                                False,
                                                                False,
                                                                False,
                                                                False,
                                                                False,
                                                                Filtro,
                                                                "",
                                                                _objParametriServer)
    End Sub

    '##############################################################
    Private Sub Carica_AnnoContabile()

        '------- ANNO --------------------

        Me.Cmb_AnnoContabile.Items.Clear()

        'Carica gli anni presenti nella tabella RicxConti per quella partita iva
        AgronicaCoreUtility.CaricaListControl.PianoContiEco_AnnoContabile(Me.Cmb_AnnoContabile,
                                                                          True, "", "",
                                                                          Qs_Piva,
                                                                          0, 0,
                                                                          "", "",
                                                                          _objParametriServer)

        Me.Cmb_AnnoContabile.SelectedIndex = Me.Cmb_AnnoContabile.Items.IndexOf(Me.Cmb_AnnoContabile.Items.FindByValue(Qs_Anno))

    End Sub

    '##############################################################
    Private Sub Carica_Esercizio()

        '------- ESERCIZIO --------------------

        Me.cmb_Esercizio.Items.Clear()

        'Carica gli anni presenti nella tabella RicxConti per quella partita iva
        AgronicaCoreUtility.CaricaListControl.PianoContiEco_AnnoContabile(Me.cmb_Esercizio,
                                                                          True, "", "",
                                                                          Qs_Piva,
                                                                          0, 0,
                                                                          "", " Anno DESC ",
                                                                          _objParametriServer)

        'Me.cmb_Esercizio.SelectedIndex = _
        '           Me.cmb_Esercizio.Items.IndexOf(Me.cmb_Esercizio.Items.FindByValue(Qs_Esercizio))

    End Sub


    '##############################################################
    Private Sub Cmb_AnnoContabile_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles Cmb_AnnoContabile.SelectedIndexChanged
        Cambia_AnnoContabile()
    End Sub

    '##############################################################
    Private Sub Cmb_Esercizio_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cmb_Esercizio.SelectedIndexChanged
        Cambia_Esercizio()
    End Sub

    '##############################################################


    '##############################################################
    Private Sub Cambia_AnnoContabile()

        If Not IsNothing(Me.Cmb_AnnoContabile) AndAlso Me.Cmb_AnnoContabile.SelectedValue <> "" Then

            '------- RICLASSIFICAZIONE ECO --------------------

            AgronicaCoreUtility.CaricaListControl.PianoContiEco_Riclassificazioni_2(Me.Cmb_Riclassificazione_Eco,
                                                                                    False, "", "",
                                                                                    Qs_Piva,
                                                                                    Me.Cmb_AnnoContabile.SelectedValue,
                                                                                    0, 0, "",
                                                                                    "", "",
                                                                                    _objParametriServer)

            'Me.Cmb_Riclassificazione_Eco.SelectedIndex = _
            '           Me.Cmb_AnnoContabile.Items.IndexOf(Me.Cmb_Riclassificazione_Eco.Items.FindByValue(Qs_Anno))

            Me.Cmb_Riclassificazione_Eco.SelectedIndex = 0

            Cambia_Riclassificazione_Eco()


            '------- RICLASSIFICAZIONE PAT --------------------

            AgronicaCoreUtility.CaricaListControl.PianoContiPat_Riclassificazioni(Me.Cmb_Riclassificazione_Pat,
                                                                                  False, "", "",
                                                                                  Qs_Piva,
                                                                                  Me.Cmb_AnnoContabile.SelectedValue,
                                                                                  0, 0, "",
                                                                                  "", "",
                                                                                  _objParametriServer)

            'Me.Cmb_Riclassificazione_Eco.SelectedIndex = _
            '           Me.Cmb_AnnoContabile.Items.IndexOf(Me.Cmb_Riclassificazione_Eco.Items.FindByValue(Qs_Anno))

            Me.Cmb_Riclassificazione_Pat.SelectedIndex = 0

            Cambia_Riclassificazione_Pat()

        End If

    End Sub

    '##############################################################
    Private Sub Cambia_Riclassificazione_Eco()

        If Not IsNothing(Me.Cmb_Riclassificazione_Eco) AndAlso Me.Cmb_Riclassificazione_Eco.SelectedValue <> "" Then

            '------- CONTI ECO --------------------
            AgronicaCoreUtility.CaricaListControl.PianoContiEco_Conti(Me.Cmb_Conti_Eco,
                                                                      True, "", "",
                                                                      Qs_Piva,
                                                                      Me.Cmb_Riclassificazione_Eco.SelectedValue,
                                                                      Me.Cmb_AnnoContabile.SelectedValue,
                                                                      0, CE_CONTO_IMPUTABILE_NOFILTRO, CONTO_UE_NOFILTRO, "", "", False,
                                                                      "", "",
                                                                      _objParametriServer)

            'Me.Cmb_Riclassificazione_Eco.SelectedIndex = _
            '           Me.Cmb_AnnoContabile.Items.IndexOf(Me.Cmb_Riclassificazione_Eco.Items.FindByValue(Qs_Anno))

            Me.Cmb_Conti_Eco.SelectedIndex = 0

        End If

    End Sub

    '##############################################################
    Private Sub Cambia_Riclassificazione_Pat()

        If Not IsNothing(Me.Cmb_Riclassificazione_Pat) AndAlso Me.Cmb_Riclassificazione_Pat.SelectedValue <> "" Then

            '------- CONTI PAT --------------------

            AgronicaCoreUtility.CaricaListControl.PianoContiPat_Conti(Me.Cmb_Conti_Pat,
                                                                      True, "", "",
                                                                      Qs_Piva,
                                                                      Me.Cmb_Riclassificazione_Pat.SelectedValue,
                                                                      Me.Cmb_AnnoContabile.SelectedValue,
                                                                      0, SP_CONTO_IMPUTABILE_NOFILTRO, CONTO_UE_NOFILTRO, "", False,
                                                                      "", "",
                                                                      _objParametriServer)

            'Me.Cmb_Riclassificazione_Eco.SelectedIndex = _
            '           Me.Cmb_AnnoContabile.Items.IndexOf(Me.Cmb_Riclassificazione_Eco.Items.FindByValue(Qs_Anno))

            Me.Cmb_Conti_Pat.SelectedIndex = 0

        End If

    End Sub

    '##############################################################
    Protected Sub ImgBtn_CercaConto_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_CercaConto.Click
        Carica_Conti_PiuAnni(Me.Txt_Cercaconto.Text)
    End Sub


    '##############################################################
    Private Sub Carica_Conti_PiuAnni(ByVal FiltroContoDescr As String)

        Dim annoMin As Integer
        Select Case Report
            Case enum_CodificaStampe.PianoDeiConti
                annoMin = CDate(Me.Txt_Data.Text).Year
            Case Else
                annoMin = CDate(Me.Txt_DataInizio.Text).Year
        End Select

        '28/02/2017: cambiato value della combo, serviva dare/avere nel cod
        AgronicaCoreUtility.CaricaListControl.PianoContiEcoPat_PiuAnni(Me.Cmb_Conti_PiuAnni,
                                                                       True, "", "",
                                                                       Qs_Piva,
                                                                       annoMin,
                                                                       0,
                                                                       BILANCIO_PERSONALIZZATO,
                                                                       CONTO_UE_NOFILTRO, "", "",
                                                                       CE_CONTO_IMPUTABILE_NOFILTRO,
                                                                       FiltroContoDescr,
                                                                       3,
                                                                       "", "",
                                                                       _objParametriServer)

        If FiltroContoDescr <> "" Then
            If Me.Cmb_Conti_PiuAnni.Items.Count > 1 Then
                Me.Cmb_Conti_PiuAnni.SelectedIndex = 1
            End If
        Else
            Me.Cmb_Conti_PiuAnni.SelectedIndex = 0
        End If

        Cambia_ContoPiuAnni()

    End Sub


    ''##############################################################
    'Private Sub ChkList_Bilancio_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ChkList_Bilancio.SelectedIndexChanged
    '    Selezione_Chk_CE_SP()
    'End Sub

    ''##############################################################
    'Private Sub Selezione_Chk_CE_SP()
    '    Disattiva_Riga_ContiEco()
    '    Disattiva_Riga_ContiPat()
    '    Disattiva_Riga_RisFinanza()
    '    Disattiva_Riga_Contatti()

    '    If Me.ChkList_Bilancio.Items(0).Selected = True Then
    '        Attiva_Riga_ContiEco()
    '        Attiva_Riga_Contatti()
    '    End If
    '    If Me.ChkList_Bilancio.Items(1).Selected = True Then
    '        Attiva_Riga_ContiPat()
    '        'Attiva_Riga_RisFinanza()
    '    End If
    'End Sub

    '##############################################################
    Private Sub Cmb_Conti_Pat_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles Cmb_Conti_Pat.SelectedIndexChanged
        Cambia_ContoPat()
    End Sub

    '##############################################################
    Private Sub Cambia_ContoPat()

        Disattiva_Riga_RisFinanza()
        Disattiva_Riga_Contatti()

        Select Case Me.Cmb_Conti_Pat.SelectedValue

            Case enum_Conti_Patrimoniali.DebitiVersoFornitori, _
                enum_Conti_Patrimoniali.CreditiVersoClienti
                Attiva_Riga_Contatti()
            Case enum_Conti_Patrimoniali.DenaroValoriInCassa
                Attiva_Riga_RisFinanza()
                Carica_RisorseFinanziarie(Qs_Piva, enum_Liquidita_CauRisorsa.LiquiditaImmediata)
            Case enum_Conti_Patrimoniali.DepositiBancariPostali
                Attiva_Riga_RisFinanza()
                Carica_RisorseFinanziarie(Qs_Piva, enum_Liquidita_CauRisorsa.RisorsaFinanziaria)
        End Select

    End Sub

    '##############################################################
    Protected Sub Cmb_Conti_PiuAnni_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Cmb_Conti_PiuAnni.SelectedIndexChanged
        Cambia_ContoPiuAnni()
    End Sub

    '##############################################################
    Private Sub Cambia_ContoPiuAnni()

        Disattiva_Riga_RisFinanza()
        Disattiva_Riga_Contatti()
        Disattiva_Riga_EscludiIvaIndetraibile()

        'Select Case Me.Cmb_Conti_PiuAnni.SelectedValue

        '    Case CStr(enum_Conti_Patrimoniali.DebitiVersoFornitori) & "|SP", _
        '        CStr(enum_Conti_Patrimoniali.CreditiVersoClienti) & "|SP"
        '        Attiva_Riga_Contatti()
        '    Case CStr(enum_Conti_Patrimoniali.DenaroValoriInCassa) & "|SP"
        '        Attiva_Riga_RisFinanza()
        '        Carica_RisorseFinanziarie(Qs_Piva, enum_Liquidita_CauRisorsa.LiquiditaImmediata)
        '    Case CStr(enum_Conti_Patrimoniali.DepositiBancariPostali) & "|SP"
        '        Attiva_Riga_RisFinanza()
        '        Carica_RisorseFinanziarie(Qs_Piva, enum_Liquidita_CauRisorsa.RisorsaFinanziaria)
        'End Select

        If Me.Cmb_Conti_PiuAnni.SelectedValue <> "" Then

            Dim SP_CE As String = Me.Cmb_Conti_PiuAnni.SelectedValue.Split("|")(1)

            If SP_CE = "SP" Then

                Dim codConto As Integer = Me.Cmb_Conti_PiuAnni.SelectedValue.Split("|")(0)
                Select Case codConto

                    Case CStr(enum_Conti_Patrimoniali.DebitiVersoFornitori),
                        CStr(enum_Conti_Patrimoniali.CreditiVersoClienti)
                        Attiva_Riga_Contatti()
                    Case CStr(enum_Conti_Patrimoniali.DenaroValoriInCassa)
                        Attiva_Riga_RisFinanza()
                        Carica_RisorseFinanziarie(Qs_Piva, enum_Liquidita_CauRisorsa.LiquiditaImmediata)
                    Case CStr(enum_Conti_Patrimoniali.DepositiBancariPostali)
                        Attiva_Riga_RisFinanza()
                        Carica_RisorseFinanziarie(Qs_Piva, enum_Liquidita_CauRisorsa.RisorsaFinanziaria)
                End Select

            Else
                Dim dareAvere As String = Me.Cmb_Conti_PiuAnni.SelectedValue.Split("|")(2)

                If dareAvere = "D" Then
                    Attiva_Riga_EscludiIvaIndetraibile()
                End If

            End If

        End If

    End Sub

    '##############################################################
    Protected Sub Rbl_PianoDeiConti_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Rbl_PianoDeiConti.SelectedIndexChanged
        Selezione_Tipo_PianoConti()
    End Sub

    '##############################################################
    Private Sub Selezione_Tipo_PianoConti()

        Disattiva_Riga_Data()
        Disattiva_Riga_Saldo0()
        Disattiva_Riga_No_Saldo0()
        Disattiva_Riga_Dettagli_SP()
        Disattiva_Riga_Contatti()
        Disattiva_Riga_RisFinanza()
        Disattiva_Riga_ContiEcoPiuAnni()
        Disattiva_Riga_AnnoConti()
        Disattiva_Riga_ContiEco()
        Disattiva_Riga_ContiPat()
        Disattiva_Riga_Esercizio()

        Select Case Me.Rbl_PianoDeiConti.SelectedValue

            Case 0 'senza saldo

                Attiva_Riga_AnnoConti()
                Attiva_Riga_ContiEco()
                Attiva_Riga_ContiPat()

            Case 1 'con saldo 
                Attiva_Riga_Data()
                Attiva_Riga_Saldo0()
                'Attiva_Riga_Contatti()
                'Attiva_Riga_RisFinanza()

                Attiva_Riga_ContiEcoPiuAnni()
                Carica_Conti_PiuAnni("")

                Me.Txt_Data.Text = Date.Today.ToShortDateString

            Case 2 'con saldo riporto
                Attiva_Riga_Saldo0()
                'Attiva_Riga_Contatti()
                'Attiva_Riga_RisFinanza()

                Attiva_Riga_ContiEcoPiuAnni()
                Carica_Conti_PiuAnni("")

        End Select


    End Sub


    Private Sub Cambia_Esercizio()
        If Not IsNothing(Me.cmb_Esercizio) AndAlso Me.cmb_Esercizio.SelectedValue <> "" Then
            Txt_DataInizio.Text = ViewState("GestCont_DataInizio")
            txt_DataFine.Text = CDate("31/12/" & CStr(Me.cmb_Esercizio.SelectedValue))
            Chk_ConsideraSaldiRip.Checked = True

            Txt_DataInizio.Enabled = False
            Lbl_DataInizio.Visible = False
            Txt_DataInizio.Visible = False
            Chk_ConsideraSaldiRip.Enabled = False
            Chk_ConsideraSaldiRip.Visible = False
        Else

            Txt_DataInizio.Enabled = True
            Lbl_DataInizio.Visible = True
            Txt_DataInizio.Visible = True
            Chk_ConsideraSaldiRip.Enabled = True
            Chk_ConsideraSaldiRip.Visible = True

        End If
    End Sub



    '##############################################################
    Private Sub Configura_Pannelli()

        Disattiva_Pannelli()

        Dim Mode As Integer

        Select Case Report
            Case enum_CodificaStampe.Bilancio_Civilistico
                Me.LblTitolo.Text = "Filtro - Bilancio"
            Case enum_CodificaStampe.PianoDeiConti
                Me.LblTitolo.Text = "Filtro - Piano dei Conti "
            Case enum_CodificaStampe.Bilanci_DiVerifica_Confronto
                Me.LblTitolo.Text = "Filtro - Confronto Bilanci"
            Case enum_CodificaStampe.Mastrino
                Me.LblTitolo.Text = "Filtro - Mastrino"
            Case enum_CodificaStampe.GiornaleContabile
                Me.LblTitolo.Text = "Filtro - Giornale Contabile"
            Case enum_CodificaStampe.Lista_InsolutiClienti
                Me.LblTitolo.Text = "Filtro - Scadenzario Clienti"
            Case enum_CodificaStampe.Lista_InsolutiFornitori
                Me.LblTitolo.Text = "Filtro - Scadenzario Fornitori"
            Case enum_CodificaStampe.EstrattoConto_Contatti
                Me.LblTitolo.Text = "Filtro - Estratto Conto Contatti"
            Case enum_CodificaStampe.RiBa_Report_Presentazione
                Me.LblTitolo.Text = "Filtro - Ri.Ba. o Anticipo Fatture"
            Case enum_CodificaStampe.Registro_FattureAcquisto
                Me.LblTitolo.Text = "Filtro - Reg. IVA Acquisti"
            Case enum_CodificaStampe.Registro_FattureVendita
                Me.LblTitolo.Text = "Filtro - Reg. IVA Vendite"
            Case enum_CodificaStampe.Registro_Corrispettivi
                Me.LblTitolo.Text = "Filtro - Reg. Corrispettivi"
        End Select

        '---------------------------------------------
        '------  inizio gestione contabile ----------
        '-----------------------------------------------
        Dim GestCont_Flag_ConsideraSaldiIniziali As Boolean '= True
        Dim GestCont_DataInizio As Date '= AGRODATAINIZIO

        GestCont_DataInizio = ViewState("GestCont_DataInizio")
        GestCont_Flag_ConsideraSaldiIniziali = ViewState("GestCont_Flag_ConsideraSaldiIniziali")


        Select Case Report

            Case enum_CodificaStampe.Bilancio_Civilistico

                Attiva_Riga_Date()
                '25/03/2016
                'Attiva_Riga_AnnoConti()
                Attiva_Riga_Esercizio()
                Attiva_Riga_ConsideraSaldiRiporto()
                Attiva_Riga_BilancioSinteticoAnalitico()
                Attiva_Riga_Dettagli_SP()
                Attiva_Riga_CE_Layout()
                Attiva_Riga_No_Saldo0()
                Attiva_Riga_ContiUE()
                Attiva_Riga_Bilancio()
                ' Mode = 0
                Mode = 1
                Imposta_AnnoContabile_Intervallo()



                '25/03/2016
                'Me.Txt_DataInizio.Text = "01/01/" & Year(Date.Today)
                Chk_ConsideraSaldiRip.Checked = GestCont_Flag_ConsideraSaldiIniziali
                Me.Txt_DataInizio.Text = GestCont_DataInizio

                'Private Sub Cambia_Esercizio()
                If Not IsNothing(Me.Cmb_AnnoContabile) AndAlso Me.Cmb_AnnoContabile.SelectedValue <> "" Then
                    Txt_DataInizio.Text = GestCont_DataInizio
                    txt_DataFine.Text = "31/12/" & Me.Cmb_AnnoContabile.SelectedValue
                    Chk_ConsideraSaldiRip.Checked = True
                Else

                End If
                'End Sub


                ' Attiva_Riga_AnnoDate()

                Carica_AnnoContabile()
                Carica_Esercizio()

                '**********************************
                '**********************************
                Dim log_errori As String = ""
                Database_OperazioniPreliminari(log_errori)
                '**********************************
                '**********************************
                Database_VerificaAnnoImputazione()

                Database_VerificaSezionaleDocumentiEContabilizzazione()

                Database_VerificaSezionaleContiVSDocumenti()

                '##############################################################

            Case enum_CodificaStampe.Bilanci_DiVerifica_Confronto

                Mode = 0
                Imposta_AnnoContabile_Intervallo()
                ' Attiva_Riga_AnnoDate()

                ' Carica_Filtri_PianoConti()

                'Attiva_Riga_FiltroConti
                Attiva_Riga_AnnoConti()
                Attiva_Riga_AnnoConfronto()

                'AgronicaCoreUtility.CaricaListControl.PianoContiEco_AnnoContabile(Me.Cmb_AnnoContabile,
                '                                                             False, "", "",
                '                                                             Qs_Piva,
                '                                                             0, 0,
                '                                                             "", "",
                '                                                             _objParametriServer)

                'Me.Cmb_AnnoContabile.SelectedIndex = _
                '    Me.Cmb_AnnoContabile.Items.IndexOf(Me.Cmb_AnnoContabile.Items.FindByValue(Qs_Anno))

                Carica_AnnoContabile()

                AgronicaCoreUtility.CaricaListControl.PianoContiEco_AnnoContabile(Me.Cmb_AnnoContabile_Confronto,
                                                                                  False, "", "",
                                                                                  Qs_Piva,
                                                                                  0, 0,
                                                                                  "", "",
                                                                                  _objParametriServer)

                Try
                    Me.Cmb_AnnoContabile_Confronto.SelectedIndex = _
                        Me.Cmb_AnnoContabile_Confronto.Items.IndexOf(Me.Cmb_AnnoContabile_Confronto.Items.FindByValue(CStr(CInt(Qs_Anno) - 1)))
                Catch ex As Exception
                    'se non c'è l'anno precedente
                End Try

                '##############################################################

            Case enum_CodificaStampe.PianoDeiConti

                Attiva_Riga_AnnoConti()
                ''Attiva_Riga_Date()
                'Mode = 1
                Imposta_AnnoContabile_Intervallo()
                'Attiva_Riga_ContiUE()

                Attiva_Riga_TipoPianoConti()
                Selezione_Tipo_PianoConti()

                Carica_AnnoContabile()
                Cambia_AnnoContabile()

                Attiva_Riga_ContiEco()
                Attiva_Riga_ContiPat()

                Attiva_Riga_Bilancio()
                Attiva_Riga_ContiUE()

                'Selezione_Chk_CE_SP()
                'Attiva_Riga_ContiMovimenti()

                Attiva_Riga_AnnoConti()


                '  Attiva_Riga_Sezionali()
                'Carica_Sezionali_FiltroNONObbligato()
                ' Carica_Sezionali_FiltroObbligato()

                'Attiva_Riga_Contatti()
                Carica_RapportiContabili("")

                'si attiva in base al chk su stato patrimoniale
                'Attiva_Riga_RisFinanza()
                Carica_RisorseFinanziarie(Qs_Piva, enum_Liquidita_CauRisorsa.Nessuna)

                '**********************************
                '**********************************
                Dim log_errori As String = ""
                Database_OperazioniPreliminari(log_errori)
                '**********************************
                '**********************************
                Database_VerificaAnnoImputazione()

                Database_VerificaSezionaleDocumentiEContabilizzazione()

                Database_VerificaSezionaleContiVSDocumenti()

                '##############################################################

            Case enum_CodificaStampe.Mastrino

                'Attiva_Riga_AnnoConti()
                Attiva_Riga_Date()
                Mode = 1
                Imposta_AnnoContabile_Intervallo()

                'disattivata gestione dell'anno
                'Carica_AnnoContabile()
                'Cambia_AnnoContabile()
                Attiva_Riga_ContiEcoPiuAnni()
                Carica_Conti_PiuAnni("")


                'selezione default sulle banche
                Dim ValueBanche As String = CStr(enum_Conti_Patrimoniali.DepositiBancariPostali) & "|SP|D"
                Me.Cmb_Conti_PiuAnni.SelectedIndex = Me.Cmb_Conti_PiuAnni.Items.IndexOf(Me.Cmb_Conti_PiuAnni.Items.FindByValue(ValueBanche))


                Chk_ConsideraSaldiRip.Checked = False
                Attiva_Riga_ConsideraSaldiRiporto()

                Attiva_Riga_Sezionali()
                Carica_Sezionali_FiltroNONObbligato()
                ' Carica_Sezionali_FiltroObbligato()

                'di default attivo la banca
                'Attiva_Riga_Contatti()
                Carica_RapportiContabili("")

                'si attiva in base al chk su stato patrimoniale
                Attiva_Riga_RisFinanza()
                Carica_RisorseFinanziarie(Qs_Piva, enum_Liquidita_CauRisorsa.RisorsaFinanziaria)

                '**********************************
                '**********************************
                Dim log_errori As String = ""
                Database_OperazioniPreliminari(log_errori)
                '**********************************
                '**********************************
                Database_VerificaAnnoImputazione()

                Database_VerificaSezionaleDocumentiEContabilizzazione()

                Database_VerificaSezionaleContiVSDocumenti()

                '##############################################################

            Case enum_CodificaStampe.GiornaleContabile

                'Attiva_Riga_AnnoConti()
                Attiva_Riga_Date()
                Mode = 1
                Imposta_AnnoContabile_Intervallo()


                Attiva_Riga_NumPagina()
                Attiva_Riga_NumRiga()
                Attiva_StampaDiProvaDefinitiva()


                '**********************************
                '**********************************
                Dim log_errori As String = ""
                Database_OperazioniPreliminari(log_errori)
                '**********************************
                '**********************************

                Database_VerificaAnnoImputazione()

                Database_VerificaSezionaleDocumentiEContabilizzazione()

                Database_VerificaSezionaleContiVSDocumenti()

                '##############################################################

            Case enum_CodificaStampe.Lista_InsolutiClienti, _
             enum_CodificaStampe.Lista_InsolutiFornitori

                '===========================================

                Attiva_Riga_Date()
                Attiva_Riga_Contatti()
                Attiva_Riga_Scadenza()
                Attiva_Riga_TipoPagamento()
                Attiva_Riga_Riscossioni()
                Attiva_Riga_Agenti()
                Attiva_Riga_Ordinamento()
                Attiva_Riga_Sezionali()

                Mode = 1
                Imposta_AnnoContabile_Intervallo()

                'CONTATTI
                Dim Filtro As String

                Filtro = "  ( Cod_Rapporto <> -6 AND Cod_Rapporto <> -7 AND Cod_Rapporto <> -8 "

                Select Case Report
                    Case enum_CodificaStampe.Lista_InsolutiClienti
                        Filtro &= " AND ( Cliente = 1 OR Terzista = 1 )  ) "
                    Case enum_CodificaStampe.Lista_InsolutiFornitori
                        Filtro &= " AND ( Fornitore = 1 OR Dipendente = 1 OR Terzista = 1 )  ) "
                End Select

                'AgronicaCoreUtility.CaricaListControl.RapportiContabili(Me.Cmb_RappContabili,
                '                                                        True, "", "",
                '                                                        0,
                '                                                        False,
                '                                                        0,
                '                                                        False,
                '                                                        False,
                '                                                        False,
                '                                                        False,
                '                                                        False,
                '                                                        False,
                '                                                        False,
                '                                                        Filtro,
                '                                                        "",
                '                                                        _objParametriServer)

                Carica_RapportiContabili(Filtro)


                ''carica TUTTI i contatti
                'RapportoContabile_Cambia(Me.Txt_CercaContatto.Text)

                Carica_Agenti()

                '  Carica_Sezionali_FiltroObbligato()
                Carica_Sezionali_FiltroNONObbligato()

                '===========================================

                AgronicaCoreUtility.CaricaListControl.ReportInsoluti_Ordinamento(Me.Cmb_Ordinamento, False, "", "")

                '##############################################################

            Case enum_CodificaStampe.EstrattoConto_Contatti

                Attiva_Riga_Date()
                'Attiva_Riga_AnnoConti()
                Attiva_Riga_Contatti2()

                Mode = 1
                Imposta_AnnoContabile_Intervallo()

                'Carica_AnnoContabile()
                'Cambia_AnnoContabile()
                Attiva_Riga_ConsideraSaldiRiporto()

                AgronicaCoreUtility.CaricaListControl.Contatti_4(Me.cmb_Contatti2,
                                                                 False, "", "",
                                                                 Qs_Piva,
                                                                 "",
                                                                 0, "",
                                                                 0,
                                                                 True,
                                                                 ID_CF_NOFILTRO,
                                                                 "", "",
                                                                 _objParametriServer)


                '##############################################################


            Case enum_CodificaStampe.RiBa_Report_Presentazione

                Attiva_Riga_IstCredito()
                Attiva_Riga_NumeriDoc()
                Attiva_Riga_RibaFatture()
                Attiva_Riga_Scadenza()

                Me.Rbl_RibaFatture.SelectedIndex = 0

                'Me.Table_Anno_Date.Visible = False
                'Me.Table_Contatti.Visible = False

                AgronicaCoreUtility.CaricaListControl.Anni(Me.Cmb_AnnoFatture,
                                                           False, "", "",
                                                           2000,
                                                           2030)

                Me.Cmb_AnnoFatture.SelectedIndex = Me.Cmb_AnnoFatture.Items.IndexOf(Me.Cmb_AnnoFatture.Items.FindByValue(CStr(Date.Today.Year)))

                'AgronicaCoreUtility.CaricaListControl.Ist_Credito(Me.Cmb_IstitutoCredito,
                '                                                  True, "", "",
                '                                                  Qs_Piva,
                '                                                  Qs_Piva,
                '                                                  "", "",
                '                                                  _objParametriServer)
                Carica_Ist_Credito(Qs_Piva)

                Me.Rbl_Scadenza.SelectedIndex = 5
                Me.Txt_Scadenza.Text = Date.Today.ToShortDateString

                Me.Rbl_NumeroDoc.SelectedIndex = 0
                Me.Txt_NumeroDoc.Text = "0"

                ''===========================================

                '##############################################################

            Case enum_CodificaStampe.Registro_FattureAcquisto,
                enum_CodificaStampe.Registro_FattureVendita

                Attiva_Riga_NumPagina()
                Attiva_Riga_Sezionali()
                Attiva_Riga_RegIVA()
                Attiva_Riga_Ordinamento()
                Attiva_Riga_DataStampa()

                '===========================================

                AgronicaCoreUtility.CaricaListControl.RegistriIva_Ordinamento(Me.Cmb_Ordinamento, False, "", "")

                Select Case Report
                    Case enum_CodificaStampe.Registro_FattureVendita
                        Me.Cmb_Ordinamento.SelectedIndex = Me.Cmb_Ordinamento.Items.IndexOf(Me.Cmb_Ordinamento.Items.FindByValue(CStr(enum_RegistriIva_Ordinamento.NumeroDoc)))
                    Case enum_CodificaStampe.Registro_FattureAcquisto
                        Me.Cmb_Ordinamento.SelectedIndex = Me.Cmb_Ordinamento.Items.IndexOf(Me.Cmb_Ordinamento.Items.FindByValue(CStr(enum_RegistriIva_Ordinamento.NumeroProtocollo)))
                End Select

                ' Me.Table_RegistroVuoto.Visible = True

                'AgronicaCoreUtility.CaricaListControl.Imprese_Sezionali(Me.Cmb_Sezionali,
                '                                                        False, "", "",
                '                                                        1,
                '                                                        Qs_Piva,
                '                                                        0,
                '                                                        "", "",
                '                                                        _objParametriServer)

                Carica_Sezionali_FiltroObbligato()

                Dim LiquidazioneIva As Integer
                LiquidazioneIva = Split(Me.Cmb_Sezionali.SelectedValue, "|")(1)

                AgronicaCoreUtility.CaricaListControl.Anni(Me.Cmb_Anno,
                                                           False, "", "",
                                                           2000,
                                                           2030)

                Me.Cmb_Anno.SelectedIndex = Me.Cmb_Anno.Items.IndexOf(Me.Cmb_Anno.Items.FindByValue(CStr(Date.Today.Year)))

                Me.Rbl_MensileTrimestrale.SelectedValue = LiquidazioneIva
                Me.Rbl_Selezione.SelectedValue = 1
                Rbl_MensileTrimestrale_SelectedIndexChanged(Me, Nothing)

                '##############################################################

            Case enum_CodificaStampe.Registro_Corrispettivi

                Attiva_Riga_NumPagina()
                Attiva_Riga_Sezionali()
                Attiva_Riga_Date()
                Attiva_Riga_Corrispettivi()
                Attiva_Riga_DataStampa()

                'FILTRO DATE
                Mode = 1
                Imposta_AnnoContabile_Intervallo()

                Qs_Di = ""
                Qs_Df = ""

                Carica_Sezionali_FiltroObbligato()

                'AgronicaCoreUtility.CaricaListControl.Imprese_Sezionali(Me.Cmb_Sezionali,
                '                                                        False, "", "",
                '                                                        1,
                '                                                        Qs_Piva,
                '                                                        0,
                '                                                        "", "",
                '                                                        _objParametriServer)

                '##############################################################

            Case Else

                Attiva_Riga_Date()

                '##############################################################

        End Select

    End Sub

    '##############################################################
    Private Sub Database_VerificaAnnoImputazione()

        Try

            Dim objDoc As New AgronicaCoreStampeDAL.DocContab
            Dim dt As New DataTable
            Dim log As New StringBuilder
            dt = objDoc.DocumentiImputatiAnnoSbagliato(Qs_Piva, 0, "", _objParametriServer)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                log.AppendLine("I seguenti documenti contengono uno o più dettagli con anno di imputazione diverso da quello della data di registrazione: ")
                log.AppendLine()

                For Each row In dt.Rows
                    log.AppendLine("* " & row.Item("Documento") & " - Data Reg: " & CDate(row.Item("Data_Registrazione")) & " - Anno: " & row.Item("Anno"))
                Next

                Dim titoloBox = "Verifica Anno Imputazione" & vbCrLf
                AgroMsgBox(titoloBox & vbCrLf & log.ToString(), Page)

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(_objParametriServer,
                                                 "Database_LogErrori",
                                                 _objParametriServer.PivaSuperUser & "_" & Session("ASG_Utente_Username") & "_VerificaAnnoImputazione.txt",
                                                 Session("ASG_Utente_Username"),
                                                 "VerificaAnnoImputazione",
                                                 log.ToString)

            End If

        Catch ex As Exception

        End Try

    End Sub


    '##############################################################
    Private Sub Database_VerificaSezionaleDocumentiEContabilizzazione()

        Try

            Dim objDoc As New AgronicaCoreStampeDAL.DocContab
            Dim dt As New DataTable
            Dim log As New StringBuilder

            'abbiamo deciso di controllare solo dall'01/01/2018 per evitare di segnalare cose su anni vecchi sui quali hanno già chiuso il bilancio
            dt = objDoc.DocumentiSezionaleCod_DiversoDa_SezionaleCodContabilizzazione(Qs_Piva, "01/01/2018", "", _objParametriServer)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

                log.AppendLine("I seguenti documenti hanno un sezionale diverso da quello impostato sulla loro contabilizzazione: ")
                log.AppendLine()

                For Each row In dt.Rows
                    log.AppendLine("* " & row.Item("des_lib") & " - Data: " & CDate(row.Item("data_movimento")) & " - Sezionale: " & row.Item("sezionale_des_documento") & " / sulla contabil. impostato sezionale: " & row.Item("sezionale_des_contabil") & vbCrLf)
                Next

                Dim titoloBox = "Verifica sui sezionali:" & vbCrLf
                AgroMsgBox(titoloBox & vbCrLf & log.ToString(), Page)

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(_objParametriServer,
                                                 "Database_LogErrori",
                                                 _objParametriServer.PivaSuperUser & "_" & Session("ASG_Utente_Username") & "_VerificaSezionaliContabilizzazione.txt",
                                                 Session("ASG_Utente_Username"),
                                                 "VerificaSezionaliContabilizzazione",
                                                 log.ToString)

            End If


        Catch ex As Exception

        End Try

    End Sub

    '##############################################################
    Private Sub Database_VerificaSezionaleContiVSDocumenti()

        Try

            Dim objDoc As New AgronicaCoreStampeDAL.DocContab
            Dim dt As New DataTable
            Dim log As New StringBuilder

            'abbiamo deciso di controllare solo dall'01/01/2018 per evitare di segnalare cose su anni vecchi sui quali hanno già chiuso il bilancio
            dt = objDoc.DocumentiSezionaleCod_DiversoDa_SezionaleCodConto(Qs_Piva, "01/01/2018", "", _objParametriServer)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

                log.AppendLine("I seguenti documenti hanno un sezionale diverso da quello impostato sul conto: ")
                log.AppendLine()

                For Each row In dt.Rows
                    log.AppendLine("* " & row.Item("des_lib") & " - Data: " & CDate(row.Item("data_movimento")) & " - Sezionale: " & row.Item("sezionale_des_documento") & " / sul conto " & row.Item("Conto_Descr") & " è impostato: " & row.Item("sezionale_des_conto") & vbCrLf)
                Next

                Dim titoloBox = "Verifica sui sezionali documenti e conti:" & vbCrLf
                AgroMsgBox(titoloBox & vbCrLf & log.ToString(), Page)

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(_objParametriServer,
                                                 "Database_LogErrori",
                                                 _objParametriServer.PivaSuperUser & "_" & Session("ASG_Utente_Username") & "_VerificaSezionaliConti.txt",
                                                 Session("ASG_Utente_Username"),
                                                 "VerificaSezionaliConti",
                                                 log.ToString)

            End If


        Catch ex As Exception

        End Try

    End Sub



    '##############################################################
    Private Sub Btn_VerificaAliquoteCorrisp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_VerificaAliquoteCorrisp.Click
        VerificaAliquoteCorrisp(1, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing)
    End Sub

    '##############################################################
    Private Sub VerificaAliquoteCorrisp(ByVal Flag_1Messaggio_2Codici As Integer,
                                        ByRef Cod_iva_1 As Integer,
                                        ByRef Cod_iva_2 As Integer,
                                        ByRef Cod_iva_3 As Integer,
                                        ByRef Cod_iva_4 As Integer,
                                        ByRef Cod_iva_non_imp_1 As Integer,
                                        ByRef Cod_iva_non_imp_2 As Integer,
                                        ByRef Cod_iva_escl_iva_1 As Integer,
                                        ByRef Cod_iva_escl_iva_2 As Integer)

        'default
        Cod_iva_1 = -999
        Cod_iva_2 = -999
        Cod_iva_3 = -999
        Cod_iva_4 = -999

        Cod_iva_non_imp_1 = -999
        Cod_iva_non_imp_2 = -999
        Cod_iva_escl_iva_1 = -999
        Cod_iva_escl_iva_2 = -999

        Dim objStampe As New AgronicaCoreStampeDAL.RegistriContab
        Dim messaggio As String = ""
        Dim dT As DataTable
        Dim i As Integer

        'elenco aliquote movimentate
        dT = objStampe.RegistroCorrispettivi_Importi_2(enum_TipologiaIva.Aliquota,
                                                       Qs_Piva,
                                                       Split(Me.Cmb_Sezionali.SelectedValue, "|")(0),
                                                       Me.Txt_DataInizio.Text,
                                                       Me.txt_DataFine.Text,
                                                       "",
                                                       _objParametriServer)

        If Not IsNothing(dT) Then

            Select Case Flag_1Messaggio_2Codici

                Case 1    'evento bottone
                    Select Case dT.Rows.Count
                        Case Is <= NUM_COLONNE_ALIQUOTE_REG_CORRISPETTIVI
                            messaggio &= "OK! Nell'intervallo selezionato sono movimentate " & CStr(dT.Rows.Count) & " aliquote." & vbCrLf
                        Case Else
                            messaggio &= "Nell'intervallo selezionato sono movimentate " & CStr(dT.Rows.Count) & " aliquote --> verranno stampate solo le prime " & CStr(NUM_COLONNE_ALIQUOTE_REG_CORRISPETTIVI) & "." & vbCrLf
                    End Select
                    '--------------
                Case 2    'recupero cod_iva per stampa report
                    For i = 0 To dT.Rows.Count - 1
                        If i = 0 Then
                            Cod_iva_1 = dT.Rows(i).Item("cod_iva")
                        End If
                        If i = 1 Then
                            Cod_iva_2 = dT.Rows(i).Item("cod_iva")
                        End If
                        If i = 2 Then
                            Cod_iva_3 = dT.Rows(i).Item("cod_iva")
                        End If
                        If i = 3 Then
                            Cod_iva_4 = dT.Rows(i).Item("cod_iva")
                        End If
                    Next

            End Select

        End If

        '//////////////////////////////////////////////////////////

        'controllo sui non imponibili
        dT = objStampe.RegistroCorrispettivi_Importi_2(enum_TipologiaIva.NonImponibile,
                                                       Qs_Piva,
                                                       Split(Me.Cmb_Sezionali.SelectedValue, "|")(0),
                                                       Me.Txt_DataInizio.Text,
                                                       Me.txt_DataFine.Text,
                                                       "",
                                                       _objParametriServer)


        If Not IsNothing(dT) Then

            Select Case Flag_1Messaggio_2Codici

                Case 1    'evento bottone
                    Select Case dT.Rows.Count
                        Case Is <= NUM_COLONNE_NONIMP_REG_CORRISPETTIVI
                            messaggio &= "OK! Nell'intervallo selezionato sono movimentati " & CStr(dT.Rows.Count) & " articoli non imponibili." & vbCrLf
                        Case Else
                            messaggio &= "Nell'intervallo selezionato sono movimentati " & CStr(dT.Rows.Count) & " articoli non imponibili " & " --> verranno stampati solo i primi " & CStr(NUM_COLONNE_NONIMP_REG_CORRISPETTIVI) & "." & vbCrLf
                    End Select
                    '--------------
                Case 2    'recupero cod_iva per stampa report
                    'Dim i_nonimp As Integer = 0
                    'Dim i_artescl As Integer = 0

                    For i = 0 To dT.Rows.Count - 1

                        '   If dT.Rows(i).Item("Tipologia") = 2 Then
                        If i = 0 Then
                            Cod_iva_non_imp_1 = dT.Rows(i).Item("cod_iva")
                            '  i_nonimp += 1
                        End If
                        If i = 1 Then
                            Cod_iva_non_imp_2 = dT.Rows(i).Item("cod_iva")
                        End If
                        '   End If

                    Next

            End Select

        End If

        '//////////////////////////////////////////////////////////

        'controllo sugli esclusi iva
        dT = objStampe.RegistroCorrispettivi_Importi_2(enum_TipologiaIva.EsclusioneIva,
                                                       Qs_Piva,
                                                       Split(Me.Cmb_Sezionali.SelectedValue, "|")(0),
                                                       Me.Txt_DataInizio.Text,
                                                       Me.txt_DataFine.Text,
                                                       "",
                                                       _objParametriServer)


        If Not IsNothing(dT) Then

            Select Case Flag_1Messaggio_2Codici

                Case 1    'evento bottone
                    Select Case dT.Rows.Count
                        Case Is <= NUM_COLONNE_NONIMP_REG_CORRISPETTIVI
                            messaggio &= "OK! Nell'intervallo selezionato sono movimentati " & CStr(dT.Rows.Count) & " articoli esclusione iva." & vbCrLf
                        Case Else
                            messaggio &= "Nell'intervallo selezionato sono movimentati " & CStr(dT.Rows.Count) & " articoli esclusione iva" & " --> verranno stampati solo i primi " & CStr(NUM_COLONNE_ESCLIVA_REG_CORRISPETTIVI) & "." & vbCrLf
                    End Select
                    '--------------
                Case 2    'recupero cod_iva per stampa report
                    'Dim i_nonimp As Integer = 0
                    'Dim i_artescl As Integer = 0

                    For i = 0 To dT.Rows.Count - 1

                        '2 = escl iva
                        ' If dT.Rows(i).Item("Tipologia") = 3 Then
                        If i = 0 Then
                            Cod_iva_escl_iva_1 = dT.Rows(i).Item("cod_iva")
                            'i_artescl += 1
                        End If
                        If i = 1 Then
                            Cod_iva_escl_iva_2 = dT.Rows(i).Item("cod_iva")
                        End If
                        '  End If

                    Next

            End Select

        End If

        If messaggio <> "" Then
            AgroMsgBox(messaggio, Page)
        End If

    End Sub

    '##############################################################
    Private Sub Rbl_Temp_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Imposta_AnnoContabile_Intervallo()

    End Sub


    '##############################################################
    Private Sub Imposta_AnnoContabile_Intervallo()

        'Me.Table_Temporale.Visible = False
        'Me.Table_AnnoContabile.Visible = False

        If Me.Table_AnnoConti.Visible = True Then

            Me.Cmb_AnnoContabile.Enabled = True
            Me.Cmb_AnnoContabile.SelectedIndex = _
                  Me.Cmb_AnnoContabile.Items.IndexOf(Me.Cmb_AnnoContabile.Items.FindByValue(Qs_Anno))

        End If

        If Me.Table_Temporale.Visible = True Then

            Dim objHLP As New AgronicaCoreContabHLP.Contabilita

            'Me.BtnImpostaData1.Disabled = False
            'Me.BtnImpostaData2.Disabled = False

            If Qs_Di = "" Or Qs_Df = "" Then
                Qs_Di = AGRODATAINIZIO
                Qs_Df = AGRODATAFINE

                objHLP.DataInizioFineMese_from_Data(Qs_Di, Qs_Df, Date.Today)
            End If

            Me.Txt_DataInizio.Text = Qs_Di
            Me.txt_DataFine.Text = Qs_Df

        End If

        'Select Case Me.Rbl_Temp.SelectedValue

        '    Case 0
        '        Me.Table_AnnoContabile.Visible = True
        '        Me.Cmb_AnnoContabile.Enabled = True
        '        Me.Cmb_AnnoContabile.SelectedIndex = _
        '              Me.Cmb_AnnoContabile.Items.IndexOf(Me.Cmb_AnnoContabile.Items.FindByValue(Qs_Anno))

        '        Me.BtnImpostaData1.Disabled = True
        '        Me.BtnImpostaData2.Disabled = True
        '        Me.Txt_DataInizio.Text = ""
        '        Me.txt_DataFine.Text = ""

        '    Case 1
        '        Me.Table_Temporale.Visible = True

        '        Dim objHLP As New AgronicaCoreContabHLP.Contabilita

        '        Me.Cmb_AnnoContabile.Enabled = False
        '        Me.Cmb_AnnoContabile.SelectedIndex = 0

        '        Me.BtnImpostaData1.Disabled = False
        '        Me.BtnImpostaData2.Disabled = False

        '        If Qs_Di = "" Or Qs_Df = "" Then
        '            Qs_Di = AGRODATAINIZIO
        '            Qs_Df = AGRODATAFINE

        '            objHLP.DataInizioFineMese_from_Data(Qs_Di, Qs_Df, Date.Today)
        '        End If

        '        Me.Txt_DataInizio.Text = Qs_Di
        '        Me.txt_DataFine.Text = Qs_Df

        'End Select


    End Sub

    '##############################################################
    Private Sub ImgBtn_Indietro_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Indietro.Click
        CalcolaDate_ScorriMese(1, Me.Txt_DataInizio.Text, Me.txt_DataFine.Text)
    End Sub

    '##############################################################
    Private Sub ImgBtn_Avanti_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Avanti.Click
        CalcolaDate_ScorriMese(0, Me.Txt_DataInizio.Text, Me.txt_DataFine.Text)
    End Sub

    '##############################################################
    Private Sub Rbl_MensileTrimestrale_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rbl_MensileTrimestrale.SelectedIndexChanged

        Imposta_Filtri_RegistriIVA()

    End Sub

    '##############################################################
    Private Sub Rbl_Selezione_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Rbl_Selezione.SelectedIndexChanged

        Imposta_Filtri_RegistriIVA()

    End Sub


    '##############################################################
    Private Sub Cmb_Mese_Da_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Mese_Da.SelectedIndexChanged

        'se mensile e con selezione
        If Me.Rbl_MensileTrimestrale.SelectedValue = 0 And Me.Rbl_Selezione.SelectedValue = 1 Then
            Me.Cmb_Mese_A.SelectedIndex = Me.Cmb_Mese_Da.SelectedIndex
        End If

    End Sub

    '##############################################################
    Private Sub Imposta_Filtri_RegistriIVA()

        Me.riga_mensile.Visible = False
        Me.riga_mensile_da.Visible = False
        Me.riga_mensile_a.Visible = False
        Me.riga_trimestre.Visible = False

        Me.Cmb_Mese_A.Items.Clear()
        Me.Cmb_Mese_Da.Items.Clear()

        Me.Cmb_Mese_A.Enabled = False
        Me.Cmb_Mese_Da.Enabled = False

        Me.Cbl_Trimestri.Items(0).Selected = False
        Me.Cbl_Trimestri.Items(1).Selected = False
        Me.Cbl_Trimestri.Items(2).Selected = False
        Me.Cbl_Trimestri.Items(3).Selected = False

        Me.Cbl_Trimestri.Enabled = False

        Select Case Me.Rbl_MensileTrimestrale.SelectedValue
            Case 0 'mensile
                'mi serve solo il tutti/seleziona
                'Me.riga_mensile.Visible = True
            Case 1 'trimestrale
                Me.riga_trimestre.Visible = True
        End Select

        Select Case Me.Rbl_Selezione.SelectedValue

            Case 0 'tutti
                'ho già disattivato



            Case 1 'seleziona

                Select Case Me.Rbl_MensileTrimestrale.SelectedValue

                    Case 0 'mensile
                        Me.riga_mensile.Visible = True
                        Me.riga_mensile_da.Visible = True
                        Me.riga_mensile_a.Visible = True
                        Me.Cmb_Mese_A.Enabled = True
                        Me.Cmb_Mese_Da.Enabled = True

                        AgronicaCoreUtility.CaricaListControl.Mesi2(Me.Cmb_Mese_A, False, "", "")
                        AgronicaCoreUtility.CaricaListControl.Mesi2(Me.Cmb_Mese_Da, False, "", "")

                    Case 1 'trimestrale

                        Me.Cbl_Trimestri.Enabled = True

                        '  Giulia, 29/11/2016 09.46.10: disattivazione di impostazione trimestrale di default
                        'Select Case Date.Today.Month
                        '    Case 1, 2, 3
                        '        Me.Cbl_Trimestri.Items(0).Selected = True
                        '    Case 4, 5, 6
                        '        Me.Cbl_Trimestri.Items(1).Selected = True
                        '    Case 7, 8, 9
                        '        Me.Cbl_Trimestri.Items(2).Selected = True
                        '    Case 10, 11, 12
                        '        Me.Cbl_Trimestri.Items(3).Selected = True
                        'End Select

                End Select

        End Select


    End Sub


    '##############################################################
    Private Sub ImgBtn_Conti_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Conti.Click

        Imposta_Filtro_Conti()

    End Sub


    '##############################################################
    Private Sub Imposta_Filtro_Conti()

        Dim Anno As Integer
        Anno = Me.Cmb_AnnoContabile.SelectedValue
        'If Me.Cmb_AnnoContabile.SelectedIndex > 0 Then
        '    Anno = Me.Cmb_AnnoContabile.SelectedValue
        'Else
        '   AgroMsgBox("Per impostare un filtro sui conti è necessario aver selezionato l'anno contabile del piano dei conti.", Page)
        '    Exit Sub
        'End If

        'If Me.Chk_Conti.Checked = True Then

        Dim queryString As String

        queryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                      "&rs=" & Stringa_Codifica(Qs_Rag_Soc, AgroKey_EncoderDecoder, Server) &
                      "&a=" & Stringa_Codifica(CStr(Anno), AgroKey_EncoderDecoder, Server) &
                      "&rc=" & Stringa_Codifica(CStr(BILANCIO_PERSONALIZZATO), AgroKey_EncoderDecoder, Server) &
                      "&elenco=" & Stringa_Codifica(Me.Txt_FiltroConti.Value, AgroKey_EncoderDecoder, Server)

        'Page_NewWindow()
        'Page_ModalDialog(Server, Session, Page,
        '                "PianoConti/Popup_PianoConti.aspx", QueryString, "Txt_FiltroConti",
        '                PopupConti_HEIGHT, PopupConti_WIDTH, 0, 0,
        '                , , , , , , )

        'End If


    End Sub

    '##############################################################
    Private Sub ImgBtn_RicercaContatto2_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_RicercaContatto2.Click
        Cerca_Contatto2(Me.Txt_CercaNomeContatto.Text, Me.Txt_CercaPivaContatto.Text)
    End Sub

    '##############################################################
    Private Sub Cerca_Contatto2(ByVal CercaNomeContatto As String, ByVal CercaPivaContatto As String)

        Dim FiltroAgg As String = ""
        Dim FlagPubblico As Boolean = True

        If CercaNomeContatto <> "" Then
            FiltroAgg = "  ( " & _
            " Contatti.Nome like '%" & Agro_SQL_SaveText(CercaNomeContatto) & "%' " & _
            " OR Contatti.Cognome like '%" & Agro_SQL_SaveText(CercaNomeContatto) & "%' " & _
            " OR Contatti.rag_soc like '%" & Agro_SQL_SaveText(CercaNomeContatto) & "%' " & _
           " ) "
        End If

        If CercaPivaContatto <> "" Then
            FlagPubblico = False
        End If

        AgronicaCoreUtility.CaricaListControl.Contatti_4(Me.cmb_Contatti2,
                                                         False, "", "",
                                                         Qs_Piva,
                                                         Agro_SQL_SaveText(CercaPivaContatto),
                                                         0, "",
                                                         0,
                                                         FlagPubblico,
                                                         ID_CF_NOFILTRO,
                                                         FiltroAgg, "",
                                                         _objParametriServer)


        If CercaNomeContatto <> "" And CercaPivaContatto <> "" And Not IsNothing(Me.Cmb_Contatti.Items) Then
            If Me.Cmb_Contatti.Items.Count > 1 Then
                Me.Cmb_Contatti.SelectedIndex = 1
            End If
        End If

    End Sub


    '##############################################################
    Private Sub Cmb_RappContabili_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_RappContabili.SelectedIndexChanged

        RapportoContabile_Cambia(Me.Txt_CercaContatto.Text)

    End Sub

    '##############################################################
    Private Sub RapportoContabile_Cambia(ByVal Str_FiltroContatti As String)

        Dim FiltroAgg As String = ""
        Dim Cod_Rapporto As Integer = 0

        If Str_FiltroContatti <> "" Then
            FiltroAgg = "  ( " & _
            " Contatti.Nome like '%" & Agro_SQL_SaveText(Str_FiltroContatti) & "%' " & _
            " OR Contatti.Cognome like '%" & Agro_SQL_SaveText(Str_FiltroContatti) & "%' " & _
            " OR Contatti.rag_soc like '%" & Agro_SQL_SaveText(Str_FiltroContatti) & "%' " & _
           " ) "
        End If

        If Me.Cmb_RappContabili.SelectedIndex > 0 AndAlso Me.Cmb_RappContabili.SelectedValue <> "" Then
            Cod_Rapporto = Me.Cmb_RappContabili.SelectedValue
        End If

        AgronicaCoreUtility.CaricaListControl.Contatti(Me.Cmb_Contatti,
                                                       True, "Tutti i contatti", "",
                                                       Qs_Piva,
                                                       "", 0,
                                                       Cod_Rapporto,
                                                       True,
                                                       False, 0, 0,
                                                       False, 0,
                                                       ID_CF_NOFILTRO,
                                                       FiltroAgg, "",
                                                       _objParametriServer)

        'If Me.Cmb_RappContabili.SelectedIndex > 0 Then

        '    'Combo_RapportiContabili_Evento(Server, Session, Page,
        '    '                    Me.Cmb_Contatti,
        '    '                    Qs_Piva,
        '    '                    Me.Cmb_RappContabili.SelectedValue)

        '    AgronicaCoreUtility.CaricaListControl.Combo_RapportiContabili_Evento(
        '                            _objParametriServer,
        '                            Me.Cmb_Contatti,
        '                            Qs_Piva,
        '                            Me.Cmb_RappContabili.SelectedValue,
        '                            FiltroAgg)


        'Else
        '    'Combo_RapportiContabili_Evento(Server, Session, Page,
        '    '                                Me.Cmb_Contatti,
        '    '                                Qs_Piva,
        '    '                                0)

        '    AgronicaCoreUtility.CaricaListControl.Combo_RapportiContabili_Evento(
        '                            _objParametriServer,
        '                            Me.Cmb_Contatti,
        '                            Qs_Piva,
        '                            0,
        '                            FiltroAgg)


        'End If

        If Str_FiltroContatti <> "" And Not IsNothing(Me.Cmb_Contatti.Items) Then
            If Me.Cmb_Contatti.Items.Count > 1 Then
                Me.Cmb_Contatti.SelectedIndex = 1
            End If
        End If

    End Sub

    '##############################################################
    Private Sub ImgBtn_CercaContatti_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_CercaContatti.Click
        RapportoContabile_Cambia(Me.Txt_CercaContatto.Text)
    End Sub

    'Private Sub ImgBtn_StampaRegistroVuoto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_StampaRegistroVuoto.Click
    '    StampaRegistroVuoto()
    'End Sub

    ''########################################################################################################################################
    'Private Sub StampaRegistroVuoto()

    '    Dim TargetURL As String
    '    Dim QueryString As String

    '    '----- Verifico i dati

    '    If Not IsNumeric(Me.Txt_NumPagineDa.Text) Then
    '        AgroMsgBox("Specificare la prima pagina!", Page)
    '        Exit Sub
    '    End If

    '    If Not IsNumeric(Me.Txt_NumPagineA.Text) Then
    '        AgroMsgBox("Specificare l'ultima pagina!", Page)
    '        Exit Sub
    '    End If

    '    If CInt(Me.Txt_NumPagineA.Text) < CInt(Me.Txt_NumPagineDa.Text) Then
    '        AgroMsgBox("L'intervallo delle pagine non è corretto", Page)
    '        Exit Sub
    '    End If

    '    TargetURL = "../RegistriPreparazioni/RegistroVuoto/RegistroVuoto.aspx"

    '    If TargetURL <> String.Empty Then

    '        QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
    '                        "&rp=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) & _
    '                        "&np=" & Stringa_Codifica(CStr(Me.Txt_NumPagine.Text), AgroKey_EncoderDecoder, Server) & _
    '                        "&npda=" & Stringa_Codifica(CStr(CInt(Me.Txt_NumPagineDa.Text)), AgroKey_EncoderDecoder, Server) & _
    '                        "&npa=" & Stringa_Codifica(CStr(CInt(Me.Txt_NumPagineA.Text)), AgroKey_EncoderDecoder, Server) & _
    '                        "&si=" & Stringa_Codifica((IIf(Me.Chk_IntestazioneRptVuoto.Checked, 1, 0)).ToString, AgroKey_EncoderDecoder, Server) & _
    '                        "&pr=" & Stringa_Codifica(CStr(Me.Txt_ProgrSigla_Registro.Text), AgroKey_EncoderDecoder, Server) & _
    '                        "&rs=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.QS_SaveText(Qs_Rag_Soc), AgroKey_EncoderDecoder, Server) & _
    '                     "&random=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Server)

    '        Page_NewWindow(Page, TargetURL, QueryString, "RegistroVuoto", , , , , , , , )

    '    End If

    'End Sub

    '##############################################################
    Protected Sub ImgBtn_Elenco_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Elenco.Click
        Link_ElencoDocumenti()
    End Sub

    '##############################################################
    Private Sub Link_ElencoDocumenti()
        Dim TargetURL As String = "../ElencoReport.aspx"
        Dim queryString As String
        Dim catCod As enum_CategorieDocumenti

        Select Case Report
            Case enum_CodificaStampe.GiornaleContabile
                catCod = enum_CategorieDocumenti.LibroGiornale

            Case Else
                'non gestiti
                Exit Sub
        End Select

        queryString = "?cc=" & Stringa_Codifica(catCod, AgroKey_EncoderDecoder, Server)

        AgronicaCoreDataProvider.UtilityProvider_2010.Page_NewWindow_2010(Page, TargetURL, queryString, , 500, 800, , , , , , , False)
    End Sub


    '##############################################################
    Private Sub ImgBtnStampa_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnStampa.Click
        Stampa(False)
    End Sub

    '##############################################################
    Private Sub ImgBtnPDF_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnPDF.Click
        Stampa(True)
    End Sub

    '##############################################################
    Private Sub Stampa(ByVal PDF As Boolean)


        Dim Pagina_Titolo As String
        Dim AnnoRegIva As String = "0"
        Dim AnnoPianoConti As Integer = 0
        Dim Esercizio As Integer = 0
        Dim Ric_Cod_Eco As Integer = 0
        Dim Ric_Cod_Pat As Integer = 0
        Dim Cod_Conto_Eco As Integer = 0
        Dim Cod_Conto_Pat As Integer = 0
        Dim AnnoConfronto As String
        Dim Data_Inizio As String = AGRODATAINIZIO
        Dim Data_Fine As String = AGRODATAFINE
        Dim DateInizio As Date = AGRODATAINIZIO
        Dim DateFine As Date = AGRODATAFINE
        Dim Data_Saldo As String = Date.Today
        Dim Log As String = ""
        Dim Filtro_Conti As String = ""
        Dim Tipo_Scadenza As Integer = 0
        Dim Tipo_Riscossione As Integer = 0
        Dim Scadenza As String = ""
        Dim Tipo_NumeroDoc As Integer = 0
        Dim NumeroDoc As Integer = 0
        Dim AnnoDoc As Integer = 0
        Dim Cod_RisUm As Integer = 0
        Dim Nome_Agente As String = ""
        Dim Cod_RisUm_Agente As Integer = 0
        Dim Cod_Rapporto As Integer = 0
        Dim Cod_Liquidita As Integer = -1
        Dim Str_Trimestri As String
        Dim Da_Mese As String
        Dim A_Mese As String
        Dim Tipologia As Integer
        Dim Cod_Istituto_Impresa As Integer = 0
        Dim Cod_Istituto_Contatto As Integer = 0
        Dim Chk_CE As Boolean = False
        Dim Chk_StatoPatrimoniale As Boolean = False
        Dim Flag_ContiSaldo0 As Boolean = False
        Dim Flag_NoSaldo0 As Boolean = False
        Dim Conti_UE_Tutti As Integer = -1
        Dim Sintetico0_Analitico1 As Integer

        Dim SP_Crediti_Clienti As Boolean = False
        Dim SP_Banche As Boolean = False
        Dim SP_Casse As Boolean = False
        Dim SP_Debiti_Fornitori As Boolean = False

        Dim CE_Layout_0Europeo_1CostiRicavi As Integer
        'Dim Conti_0Movimentati_1Tutti As Integer = -1
        ' Dim Conti_0ImponibileNoZero_1Tutti As Integer = -1
        'enum_ReportInsoluti_Ordinamento = enum_ReportInsoluti_Ordinamento.Scadenza
        Dim Ordinamento, Tipo_PianoConti As Integer
        Dim Sezionale_Cod As Integer = SEZIONALE_NOFILTRO
        Dim Sezionale_Des As String = ""
        Dim Sezionale_ChkDefault As Integer = -1
        Dim Num_Pagina As Integer = 0
        Dim Num_Riga As Integer = 0
        Dim DiProva0_Definitiva1 As enum_StampaDefinitivaDiProva
        Dim cod_contatto As String = ""
        Dim Lista_tipiPag As String = ""
        Dim FlagEscludiIvaIndetraibile As Boolean = False

        Dim Param_Intervallo_Date As String = ""
        Dim Param_Rag_Soc As String = ""
        Dim Param_Piva_CodFiscale As String = ""
        Dim Param_Indirizzo As String = ""

        Dim objDocContab As New AgronicaCoreStampeDAL.DocContab

        Select Case Report

            Case enum_CodificaStampe.PianoDeiConti
                Pagina_Titolo = "PianoDeiConti"

            Case enum_CodificaStampe.Bilancio_Civilistico
                Pagina_Titolo = "Bilancio_Civilistico"

            Case enum_CodificaStampe.Lista_InsolutiClienti
                Pagina_Titolo = "Scadenzario_Clienti"

            Case enum_CodificaStampe.Lista_InsolutiFornitori
                Pagina_Titolo = "Scadenzario_Fornitori"

            Case enum_CodificaStampe.EstrattoConto_Contatti
                Pagina_Titolo = "EstrattoConto_Contatti"

            Case enum_CodificaStampe.Registro_FattureAcquisto
                Pagina_Titolo = "Registro_Acquisti"

            Case enum_CodificaStampe.Registro_FattureVendita
                Pagina_Titolo = "Registro_Vendite"

            Case enum_CodificaStampe.Registro_Corrispettivi
                Pagina_Titolo = "Registro_Corrispettivi"

            Case enum_CodificaStampe.RiBa_Report_Presentazione
                Pagina_Titolo = "Registro_RiBa"

            Case enum_CodificaStampe.Mastrino
                Pagina_Titolo = "Mastrino_Contabile"

            Case enum_CodificaStampe.GiornaleContabile
                Pagina_Titolo = "Giornale_Contabile"

        End Select

        '-----------------------------------------------
        '------ STAMPA DI PROVA / DEFINITIVA ----------
        '-----------------------------------------------
        If Me.Table_StampaProvaDefinitiva.Visible = True Then
            DiProva0_Definitiva1 = Me.Rbl_ProvaDefinitiva.SelectedValue
        End If

        '------------------------------
        '------ PERIODO TEMP ----------
        '------------------------------
        ' If Me.Table_Anno_Date.Visible = True Then


        '---------------------------------------------
        '------  inizio gestione contabile ----------
        '-----------------------------------------------
        Dim GestCont_Flag_ConsideraSaldiIniziali As Boolean '= True
        Dim GestCont_DataInizio As Date '= AGRODATAINIZIO
        '   Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        'objImprese.LeggiOpzioni_InizioGestioneContabile(_objParametriServer,
        '                                                Qs_Piva,
        '                                                GestCont_Flag_ConsideraSaldiIniziali,
        '                                                GestCont_DataInizio)

        GestCont_DataInizio = ViewState("GestCont_DataInizio")
        GestCont_Flag_ConsideraSaldiIniziali = ViewState("GestCont_Flag_ConsideraSaldiIniziali")

        'controllo aggiunto il 28/02/2017
        If TableConsideraSaldiRip.Visible = True Then
            GestCont_Flag_ConsideraSaldiIniziali = Chk_ConsideraSaldiRip.Checked
        End If

        If Me.Table_Temporale.Visible = True Then

            '  Anno = 0

            If Me.Txt_DataInizio.Text = "" Then
                Log &= "Inserire la data inizio del periodo di competenza." & vbCrLf & vbCrLf
            Else
                Data_Inizio = Me.Txt_DataInizio.Text
                If Not IsDate(Data_Inizio) Then
                    Log &= "La data inizio non è una data valida." & vbCrLf & vbCrLf
                Else
                    DateInizio = Data_Inizio
                End If

                'Anno = CStr(CDate(Me.Txt_DataInizio.Text).Year)
            End If

            If Me.txt_DataFine.Text = "" Then
                Log &= "Inserire la data fine del periodo di competenza." & vbCrLf & vbCrLf
            Else
                Data_Fine = Me.txt_DataFine.Text
                If Not IsDate(Data_Fine) Then
                    Log &= "La data fine non è una data valida." & vbCrLf & vbCrLf
                Else
                    DateFine = Data_Fine
                End If
            End If

            If DateInizio <> AGRODATAINIZIO And DateFine <> AGRODATAFINE Then
                If DateInizio > DateFine Then
                    Log &= "La data inizio non può essere maggiore alla data fine." & vbCrLf & vbCrLf
                End If
            End If

        End If

        '------------------------------
        '------ DATA ----------
        '------------------------------
        If Me.Table_Data.Visible = True Then

            If Me.Txt_Data.Text = "" Then
                Log &= "Specificare la data." & vbCrLf & vbCrLf
            Else
                Data_Saldo = Me.Txt_Data.Text
            End If

        End If



        '------------------------------
        '--------- SEZIONALI ----------
        '------------------------------

        If Me.Table_Sezionali.Visible = True Then
            Sezionale_Cod = Split(Me.Cmb_Sezionali.SelectedValue, "|")(0)
            Sezionale_Des = Me.Cmb_Sezionali.SelectedItem.Text
            Sezionale_ChkDefault = Split(Me.Cmb_Sezionali.SelectedValue, "|")(2)
        End If


        '------------------------------
        '------ ANNO CONFRONTO --------
        '------------------------------
        If Report = enum_CodificaStampe.Bilanci_DiVerifica_Confronto Then

            If Me.Cmb_AnnoContabile_Confronto.SelectedIndex < 0 Or Me.Cmb_AnnoContabile_Confronto.SelectedValue = 0 Then
                Log &= "Selezionare l'anno contabile del bilancio da confrontare." & vbCrLf & vbCrLf
            Else
                If Me.Cmb_AnnoContabile.SelectedValue = Me.Cmb_AnnoContabile_Confronto.SelectedValue Then
                    Log &= "Per confrontare due bilanci è necessario selezionare due anni contabili diversi." & vbCrLf & vbCrLf
                Else
                    If Me.Cmb_AnnoContabile_Confronto.SelectedValue > Me.Cmb_AnnoContabile.SelectedValue Then
                        Log &= "L'anno del bilancio da confrontare deve essere antecedente all'anno selezionato." & vbCrLf & vbCrLf
                    Else
                        AnnoConfronto = Me.Cmb_AnnoContabile_Confronto.SelectedValue
                    End If
                End If
            End If
        End If

        '------------------------------------------
        '------ FILTRI SUL PIANO DEI CONTI --------
        '------------------------------------------
        If Me.Table_Bilancio.Visible = True Then

            If Me.ChkList_Bilancio.Visible = True Then
                If Me.ChkList_Bilancio.Items(0).Selected = True Then
                    Chk_CE = True
                End If
                If Me.ChkList_Bilancio.Items(1).Selected = True Then
                    Chk_StatoPatrimoniale = True
                End If
                If Chk_CE = False And Chk_StatoPatrimoniale = False Then
                    Log &= "e' necessario selezionare il conto economico e/o lo stato patrimoniale." & vbCrLf & vbCrLf
                End If
            End If
        End If

        If Me.Table_ContiUE.Visible = True Then
            Conti_UE_Tutti = Me.Rbl_ContiUE.SelectedValue()
        End If

        If Me.Table_SinteticoAnalitico.Visible = True Then
            Sintetico0_Analitico1 = Me.Rbl_SinteticoAnalitico.SelectedValue()
        End If

        If Me.Table_AnnoConti.Visible = True Then
            If Me.Cmb_AnnoContabile.SelectedValue <> "" Then
                AnnoPianoConti = Me.Cmb_AnnoContabile.SelectedValue
            Else
                Log &= "Non è stato rilevato alcun Piano dei Conti." & vbCrLf &
                       "Impossibile procedere con la stampa." & vbCrLf & vbCrLf
            End If
        End If

        If Me.TableEsercizio2.Visible = True Then
            If Me.cmb_Esercizio.SelectedValue <> "" Then
                Esercizio = Me.cmb_Esercizio.SelectedValue
            Else
                'Log &= "Esercizio." & vbCrLf &
                '        "" & vbCrLf & vbCrLf
            End If
        End If


        If Me.Table_ContiEco.Visible = True Then
            If Me.Cmb_Riclassificazione_Eco.SelectedValue <> "" Then
                Ric_Cod_Eco = Me.Cmb_Riclassificazione_Eco.SelectedValue
            End If
            If Me.Cmb_Conti_Eco.SelectedValue <> "" Then
                Cod_Conto_Eco = Me.Cmb_Conti_Eco.SelectedValue
            End If
        End If

        If Me.Table_ContiPat.Visible = True Then
            If Me.Cmb_Riclassificazione_Pat.SelectedValue <> "" Then
                Ric_Cod_Pat = Me.Cmb_Riclassificazione_Pat.SelectedValue
            End If
            If Me.Cmb_Conti_Pat.SelectedValue <> "" Then
                Cod_Conto_Pat = Me.Cmb_Conti_Pat.SelectedValue
            End If
        End If

        If Me.Table_Conti_PiuAnni.Visible = True Then
            AnnoPianoConti = 0
            Ric_Cod_Eco = BILANCIO_PERSONALIZZATO
            Ric_Cod_Pat = BILANCIO_PERSONALIZZATO
            If Me.Cmb_Conti_PiuAnni.SelectedValue <> "" Then
                Select Case CStr(Me.Cmb_Conti_PiuAnni.SelectedValue).Split("|")(1)
                    Case "CE"
                        Chk_CE = True
                        Cod_Conto_Eco = CStr(Me.Cmb_Conti_PiuAnni.SelectedValue).Split("|")(0)
                    Case "SP"
                        Chk_StatoPatrimoniale = True
                        Cod_Conto_Pat = CStr(Me.Cmb_Conti_PiuAnni.SelectedValue).Split("|")(0)
                End Select
            End If
        End If


        If Me.Table_CE_Layout.Visible = True Then
            CE_Layout_0Europeo_1CostiRicavi = Me.Rbl_CE_Layout.SelectedValue()
        End If

        If Me.Table_SaldoConti0.Visible = True Then
            Flag_ContiSaldo0 = Me.Chk_Saldo0.Checked
        End If

        If Me.Table_No_Saldo0.Visible = True Then
            Flag_NoSaldo0 = Me.Chk_No_Saldo0.Checked
        End If

        If Me.TableEscludiIvaIndetraibile.Visible = True Then
            FlagEscludiIvaIndetraibile = Me.Chk_EscludiIvaIndetraibile.Checked
        End If

        '-----------------------------------------------
        '------ FILTRI SUI DETTAGLI SP BILANCIO --------
        '-----------------------------------------------
        If Me.Table_DettagliAuto.Visible = True Then

            If Me.CheckBoxListDettagliAuto.Items(0).Selected = True Then
                SP_Crediti_Clienti = True
            End If
            If Me.CheckBoxListDettagliAuto.Items(1).Selected = True Then
                SP_Banche = True
            End If
            If Me.CheckBoxListDettagliAuto.Items(2).Selected = True Then
                SP_Casse = True
            End If
            If Me.CheckBoxListDettagliAuto.Items(3).Selected = True Then
                SP_Debiti_Fornitori = True
            End If

        End If


        '------------------------------
        '------ FILTRO CONTI ----------
        '------------------------------
        If Me.Table_FiltroConti.Visible = True Then
            Filtro_Conti = Me.Txt_FiltroConti.Value
        End If


        '------------------------------
        '--- ISTITUTI CREDITO  --------
        '------------------------------
        If Me.Table_IstitutoCredito.Visible = True Then
            If Me.Cmb_IstitutoCredito.SelectedIndex > 0 Then
                Cod_Istituto_Impresa = Me.Cmb_IstitutoCredito.SelectedValue
            Else
                Cod_Istituto_Impresa = 0
            End If
        End If

        '------------------------------
        '--- FILTRO NUMERI DOC --------
        '------------------------------
        If Me.Table_NumeriDoc.Visible = True Then
            Select Case Me.Rbl_NumeroDoc.SelectedValue
                Case 0
                    NumeroDoc = 0
                    AnnoDoc = 0
                    Tipo_NumeroDoc = 0
                Case Else
                    Tipo_NumeroDoc = Me.Rbl_NumeroDoc.SelectedValue
                    AnnoDoc = Me.Cmb_AnnoFatture.SelectedValue
                    If Me.Txt_NumeroDoc.Text = "" Then
                        Log &= "Inserire il numero del documento al quale fare riferimento per il filtro!" & vbCrLf & vbCrLf
                    Else
                        NumeroDoc = Me.Txt_NumeroDoc.Text
                    End If
            End Select

        End If

        '------------------------------
        '------ CONTATTI --------------
        '------------------------------
        If Me.Table_Contatti.Visible = True Then
            If Me.Cmb_RappContabili.SelectedIndex > 0 Then
                Cod_Rapporto = Me.Cmb_RappContabili.SelectedValue
            Else
                Cod_Rapporto = 0
            End If
            If Me.Cmb_Contatti.SelectedIndex > 0 Then
                Cod_RisUm = CStr(Me.Cmb_Contatti.SelectedValue).Split("|")(0)
            Else
                Cod_RisUm = 0
                'Log &= "E' stato selezionato il rapporto contabile " & Me.Cmb_RappContabili.SelectedItem.Text & " " & vbCrLf & _
                '"Selezionare il contatto!" & vbCrLf
            End If
        Else
            Cod_RisUm = 0
            Cod_Rapporto = 0
        End If

        If Me.Table_Contatti2.Visible = True Then
            cod_contatto = Me.cmb_Contatti2.SelectedValue
            If cod_contatto = "" Then
                Log &= "Selezionare un contatto prima di richierne la stampa!" & vbCrLf & vbCrLf
            End If
        End If

        '------------------------------
        '------ RISORSA FINANZIARIA --------------
        '------------------------------
        If Me.Table_RisFinanza.Visible = True Then
            If Me.Cmb_RisorseFinanza.SelectedIndex <> -1 Then
                If Me.Cmb_RisorseFinanza.SelectedValue > -1 Then
                    Cod_Liquidita = Me.Cmb_RisorseFinanza.SelectedValue
                Else
                    Cod_Liquidita = -1
                End If
            End If
        End If

        '------------------------------
        '------ AGENTE --------------
        '------------------------------
        If Me.Table_Agenti.Visible = True Then
            If Me.Cmb_Agenti.SelectedIndex > 0 Then
                Cod_RisUm_Agente = CStr(Me.Cmb_Agenti.SelectedValue).Split("|")(0)
                Nome_Agente = Me.Cmb_Agenti.SelectedItem.Text
            Else
                Cod_RisUm_Agente = 0
                'Log &= "E' stato selezionato il rapporto contabile " & Me.Cmb_RappContabili.SelectedItem.Text & " " & vbCrLf & _
                '"Selezionare il contatto!" & vbCrLf
            End If
        End If

        '------------------------------
        '------ SCADENZA --------------
        '------------------------------
        If Me.Table_Scadenza.Visible = True Then

            Select Case Me.Rbl_Scadenza.SelectedValue

                Case 0
                    Scadenza = AGRODATAFINE
                    Tipo_Scadenza = 0
                Case Else
                    Tipo_Scadenza = Me.Rbl_Scadenza.SelectedValue
                    If Me.Txt_Scadenza.Text = "" Then
                        Log &= "Inserire la data di scadenza!" & vbCrLf & vbCrLf
                    Else
                        Scadenza = Me.Txt_Scadenza.Text
                    End If
            End Select
        Else
            Scadenza = ""
            Tipo_Scadenza = 0
        End If

        '------------------------------
        '------ TIPO PAGAMENTO --------------
        '------------------------------
        If Me.Tabella12b_TipoPagamento.Visible = True Then
            Dim codTipopag As Integer
            For i = 0 To CBL_TipoPagamento.Items.Count - 1
                If CBL_TipoPagamento.Items(i).Selected = True Then
                    codTipopag = CBL_TipoPagamento.Items(i).Value
                    Lista_tipiPag &= CStr(codTipopag) & ", "
                End If
            Next
            If Lista_tipiPag <> "" Then
                Lista_tipiPag = Mid(Lista_tipiPag, 1, Lista_tipiPag.Length - 2) 'tolgo virgola e spazio
                Lista_tipiPag = "(" & Lista_tipiPag & ")"
            End If
        End If

        '--------------------------------------
        '------ FILTRO RISCOSSIONI --------------
        '--------------------------------------
        If Me.Table_Riscossioni.Visible = True Then
            Tipo_Riscossione = Me.Rbl_Riscossioni.SelectedValue
        End If

        '--------------------------------------
        '------ NUM PAGINA --------------
        '--------------------------------------
        If Me.Table_NumPagina.Visible = True Then
            If Me.Txt_NumPagina.Text = "" Or Not IsNumeric(Me.Txt_NumPagina.Text) Then
                Log = "E' necessario specificare il numero di pagina dal quale iniziare la numerazione delle pagine del registro."
            Else
                Num_Pagina = CInt(Me.Txt_NumPagina.Text)
            End If
        End If

        '--------------------------------------
        '------ NUM RIGA --------------
        '--------------------------------------
        If Me.Table_NumRiga.Visible = True Then
            If Me.Txt_NumRiga.Text = "" Or Not IsNumeric(Me.Txt_NumRiga.Text) Then
                Log = "E' necessario specificare il numero della registrazione dal quale iniziare la numerazione delle pagine del registro."
            Else
                Num_Riga = CInt(Me.Txt_NumRiga.Text)
            End If
        End If

        '--------------------------------------
        '------ SCELTA TRIMESTRI --------------
        '--------------------------------------
        If Me.Table_RegistriIVA.Visible = True Then

            AnnoRegIva = Me.Cmb_Anno.SelectedValue

            Select Case Me.Rbl_MensileTrimestrale.SelectedValue

                Case 0 'mensile

                    Tipologia = 0
                    Str_Trimestri = ""

                    Select Case Me.Rbl_Selezione.SelectedValue
                        Case 0 'tutti
                            Da_Mese = "01"
                            A_Mese = "12"
                        Case 1 'selezione
                            Da_Mese = Me.Cmb_Mese_Da.SelectedValue
                            A_Mese = Me.Cmb_Mese_A.SelectedValue
                    End Select

                    '---------------------

                Case 1 'trimestrale

                    Tipologia = 1
                    Da_Mese = ""
                    A_Mese = ""

                    Select Case Me.Rbl_Selezione.SelectedValue

                        Case 0 'tutti
                            Str_Trimestri = "1|1|1|1"
                        Case 1
                            If Me.Cbl_Trimestri.Items(0).Selected = False Then
                                Str_Trimestri = "0"
                            Else
                                Str_Trimestri = "1"
                            End If
                            If Me.Cbl_Trimestri.Items(1).Selected = False Then
                                Str_Trimestri &= "|0"
                            Else
                                Str_Trimestri &= "|1"
                            End If
                            If Me.Cbl_Trimestri.Items(2).Selected = False Then
                                Str_Trimestri &= "|0"
                            Else
                                Str_Trimestri &= "|1"
                            End If
                            If Me.Cbl_Trimestri.Items(3).Selected = False Then
                                Str_Trimestri &= "|0"
                            Else
                                Str_Trimestri &= "|1"
                            End If

                            '  Giulia, 29/11/2016 09.51.43: Se non è stato scelto un trimestre specifico
                            If Str_Trimestri = "0|0|0|0" Then
                                Log &= "Selezionare un trimestre specifico prima di richierne la stampa!" & vbCrLf & vbCrLf
                            End If

                    End Select

            End Select

        Else
            Tipologia = -1
            Str_Trimestri = ""
            Da_Mese = ""
            A_Mese = ""
        End If



        If Log <> "" Then
            AgroMsgBox(Log, Page)
            Exit Sub
        End If

        '------------------------------
        '------ PIANO DEI CONTI ----------
        '------------------------------
        If Me.Table0.Visible = True Then
            Tipo_PianoConti = Me.Rbl_PianoDeiConti.SelectedValue
        End If

        '------------------------------
        '------ ORDINAMENTO ----------
        '------------------------------
        If Me.Table_Ordinamento.Visible = True Then
            Ordinamento = Me.Cmb_Ordinamento.SelectedValue
        End If


        '##############################################################
        '##############################################################

        Dim TargetURL, queryString As String
        Dim Flag_Stampe2010 As Boolean
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim Valore As String

        Valore = objConfigSiti.Recupera_Valore_ByChiave(0, "Stampe2010", _objParametriServer)
        If Valore = "" Then
            Flag_Stampe2010 = False
        Else
            Flag_Stampe2010 = CBool(Valore)
        End If

        '--------------------------
        'controlli sui conti
        '--------------------------
        ' If ViewState("Piva_riferimento") <> BILANCIO_EUROPEO Then
        Dim LogConti As String = ""


        Select Case Report
            Case enum_CodificaStampe.Mastrino,
                enum_CodificaStampe.GiornaleContabile,
                enum_CodificaStampe.Bilancio_Civilistico,
                enum_CodificaStampe.PianoDeiConti,
                enum_CodificaStampe.EstrattoConto_Contatti

                If Data_Inizio < GestCont_DataInizio And Data_Fine < GestCont_DataInizio Then
                    AgroMsgBox("L'intervallo selezionato per la stampa è antecedente alla data di inizio gestione contabile: " & GestCont_DataInizio.ToShortDateString, Page)
                    Exit Sub
                End If

                Try

                    'verifica se i conti patrimoniali significativi sono codificati con quelli gias
                    Dim objRicContiR As New AgronicaCoreContabDAL.RicxConti_Patrimonio_R
                    Dim flagVerifica As Boolean
                    flagVerifica = objRicContiR.Verifica_Codifica_ContiPatrimoniali(LogConti, Qs_Piva, BILANCIO_PERSONALIZZATO, AnnoPianoConti, CDate(Data_Inizio).Year, "", _objParametriServer)

                    If flagVerifica = False Then
                        AgroMsgBox("Attenzione codifiche conti patrimoniali mancanti: " & LogConti, Page)
                    End If

                Catch ex As Exception
                    LogConti &= "Verifica_Codifica_ContiPatrimoniali: " & vbCrLf & ex.Message & vbCrLf
                End Try

                Try

                    'verifica se i conti economici significativi sono codificati con quelli gias
                    Dim objRicContiR As New AgronicaCoreContabDAL.RicxConti_R
                    Dim flagVerifica As Boolean
                    flagVerifica = objRicContiR.Verifica_Codifica_ContiEconomici(LogConti, Qs_Piva, BILANCIO_PERSONALIZZATO, AnnoPianoConti, CDate(Data_Inizio).Year, "", _objParametriServer)

                    If flagVerifica = False Then
                        AgroMsgBox("Attenzione codifiche conti economici mancanti: " & LogConti, Page)
                    End If

                Catch ex As Exception
                    LogConti &= "Verifica_Codifica_ContiEconomici: " & vbCrLf & ex.Message & vbCrLf
                End Try

        End Select

        If LogConti <> "" Then
            AgroMsgBox(LogConti, Page)
            Exit Sub
        End If

        ' End If 'ViewState("Piva_riferimento")

        '/************* PARAMETRI X INTESTAZIONE DOCUMENTI ***************************/
        objDocContab.Prepara_Parametri_Intestazione_ReportContab(Qs_Piva, _
                                                                    Data_Inizio, _
                                                                    Data_Fine, _
                                                                    Param_Rag_Soc, _
                                                                    Param_Piva_CodFiscale, _
                                                                    Param_Indirizzo, _
                                                                    Param_Intervallo_Date, _
                                                                    _objParametriServer)


        '/************* NUOVI ARROTONDAMENTI ***************************/
        Dim Flag_NuovaVersioneRound As Boolean
        Flag_NuovaVersioneRound = UsaNuoviArrotondamenti(_objParametriServer)

        '30/07/2019: aggiunto controllo sulal configurazione, ora la chiave deve sempre essere a true
        If Flag_NuovaVersioneRound = False Then
            AgroMsgBox("Config_siti: key flag nuova versione round non impostata correttamente. Contattare l'amministratore prima di procedere con la stampa.", Page)
            Exit Sub
        End If

        Select Case Report

            Case enum_CodificaStampe.Mastrino

                TargetURL = "BilancioMatrino/Mastrino.aspx"

                queryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                              "&a=" & Stringa_Codifica(AnnoPianoConti, AgroKey_EncoderDecoder, Server) &
                              "&r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                              "&di=" & Stringa_Codifica(Data_Inizio, AgroKey_EncoderDecoder, Server) &
                              "&df=" & Stringa_Codifica(Data_Fine, AgroKey_EncoderDecoder, Server) &
                              "&chk_ce=" & Stringa_Codifica(Chk_CE, AgroKey_EncoderDecoder, Server) &
                              "&chk_sp=" & Stringa_Codifica(Chk_StatoPatrimoniale, AgroKey_EncoderDecoder, Server) &
                              "&rce=" & Stringa_Codifica(Ric_Cod_Eco, AgroKey_EncoderDecoder, Server) &
                              "&cce=" & Stringa_Codifica(Cod_Conto_Eco, AgroKey_EncoderDecoder, Server) &
                              "&rcp=" & Stringa_Codifica(Ric_Cod_Pat, AgroKey_EncoderDecoder, Server) &
                              "&ccp=" & Stringa_Codifica(Cod_Conto_Pat, AgroKey_EncoderDecoder, Server) &
                              "&cru=" & Stringa_Codifica(Cod_RisUm, AgroKey_EncoderDecoder, Server) &
                              "&liq=" & Stringa_Codifica(Cod_Liquidita, AgroKey_EncoderDecoder, Server) &
                              "&gcfcsi=" & Stringa_Codifica(GestCont_Flag_ConsideraSaldiIniziali, AgroKey_EncoderDecoder, Server) &
                              "&gcdi=" & Stringa_Codifica(GestCont_DataInizio, AgroKey_EncoderDecoder, Server) &
                              "&szc=" & Stringa_Codifica(Sezionale_Cod, AgroKey_EncoderDecoder, Server) &
                              "&szd=" & Stringa_Codifica(Sezionale_Des, AgroKey_EncoderDecoder, Server) &
                              "&szdf=" & Stringa_Codifica(Sezionale_ChkDefault, AgroKey_EncoderDecoder, Server) &
                              "&chkii=" & Stringa_Codifica(FlagEscludiIvaIndetraibile, AgroKey_EncoderDecoder, Server)

                'se si modifica qualcosa qui, modificarla anche su StampaDirettaMastrino

                '###########################################################


            Case enum_CodificaStampe.GiornaleContabile

                TargetURL = "LibroGiornale/LibroGiornale.aspx"

                queryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                              "&a=" & Stringa_Codifica(AnnoPianoConti, AgroKey_EncoderDecoder, Server) &
                              "&r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                              "&di=" & Stringa_Codifica(Data_Inizio, AgroKey_EncoderDecoder, Server) &
                              "&df=" & Stringa_Codifica(Data_Fine, AgroKey_EncoderDecoder, Server) &
                              "&fc=" & Stringa_Codifica(Filtro_Conti, AgroKey_EncoderDecoder, Server) &
                              "&chk_ce=" & Stringa_Codifica(Chk_CE, AgroKey_EncoderDecoder, Server) &
                              "&chk_sp=" & Stringa_Codifica(Chk_StatoPatrimoniale, AgroKey_EncoderDecoder, Server) &
                              "&ue=" & Stringa_Codifica(Conti_UE_Tutti, AgroKey_EncoderDecoder, Server) &
                              "&rce=" & Stringa_Codifica(Ric_Cod_Eco, AgroKey_EncoderDecoder, Server) &
                              "&cce=" & Stringa_Codifica(Cod_Conto_Eco, AgroKey_EncoderDecoder, Server) &
                              "&rcp=" & Stringa_Codifica(Ric_Cod_Pat, AgroKey_EncoderDecoder, Server) &
                              "&ccp=" & Stringa_Codifica(Cod_Conto_Pat, AgroKey_EncoderDecoder, Server) &
                              "&szc=" & Stringa_Codifica(Sezionale_Cod, AgroKey_EncoderDecoder, Server) &
                              "&szd=" & Stringa_Codifica(Sezionale_Des, AgroKey_EncoderDecoder, Server) &
                              "&cru=" & Stringa_Codifica(Cod_RisUm, AgroKey_EncoderDecoder, Server) &
                              "&liq=" & Stringa_Codifica(Cod_Liquidita, AgroKey_EncoderDecoder, Server) &
                              "&np=" & Stringa_Codifica(Num_Pagina, AgroKey_EncoderDecoder, Server) &
                              "&nr=" & Stringa_Codifica(Num_Riga, AgroKey_EncoderDecoder, Server) &
                              "&stDef=" & Stringa_Codifica(DiProva0_Definitiva1, AgroKey_EncoderDecoder, Server) &
                              "&gcfcsi=" & Stringa_Codifica(GestCont_Flag_ConsideraSaldiIniziali, AgroKey_EncoderDecoder, Server) &
                              "&gcdi=" & Stringa_Codifica(GestCont_DataInizio, AgroKey_EncoderDecoder, Server)


                '###########################################################

            Case enum_CodificaStampe.PianoDeiConti

                '---- PIANO DEI CONTI

                TargetURL = "BilancioVerifica/BilancioVerifica.aspx"

                queryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                              "&a=" & Stringa_Codifica(AnnoPianoConti, AgroKey_EncoderDecoder, Server) &
                              "&r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                              "&ds=" & Stringa_Codifica(Data_Saldo, AgroKey_EncoderDecoder, Server) &
                              "&fc=" & Stringa_Codifica(Filtro_Conti, AgroKey_EncoderDecoder, Server) &
                              "&chk_ce=" & Stringa_Codifica(Chk_CE, AgroKey_EncoderDecoder, Server) &
                              "&chk_sp=" & Stringa_Codifica(Chk_StatoPatrimoniale, AgroKey_EncoderDecoder, Server) &
                              "&ue=" & Stringa_Codifica(Conti_UE_Tutti, AgroKey_EncoderDecoder, Server) &
                              "&cs=" & Stringa_Codifica(Flag_ContiSaldo0, AgroKey_EncoderDecoder, Server) &
                              "&rce=" & Stringa_Codifica(Ric_Cod_Eco, AgroKey_EncoderDecoder, Server) &
                              "&cce=" & Stringa_Codifica(Cod_Conto_Eco, AgroKey_EncoderDecoder, Server) &
                              "&rcp=" & Stringa_Codifica(Ric_Cod_Pat, AgroKey_EncoderDecoder, Server) &
                              "&ccp=" & Stringa_Codifica(Cod_Conto_Pat, AgroKey_EncoderDecoder, Server) &
                              "&cru=" & Stringa_Codifica(Cod_RisUm, AgroKey_EncoderDecoder, Server) &
                              "&liq=" & Stringa_Codifica(Cod_Liquidita, AgroKey_EncoderDecoder, Server) &
                              "&tpc=" & Stringa_Codifica(Tipo_PianoConti, AgroKey_EncoderDecoder, Server) &
                              "&gcfcsi=" & Stringa_Codifica(GestCont_Flag_ConsideraSaldiIniziali, AgroKey_EncoderDecoder, Server) &
                              "&gcdi=" & Stringa_Codifica(GestCont_DataInizio, AgroKey_EncoderDecoder, Server) &
                              "&szc=" & Stringa_Codifica(Sezionale_Cod, AgroKey_EncoderDecoder, Server) &
                              "&szd=" & Stringa_Codifica(Sezionale_Des, AgroKey_EncoderDecoder, Server) &
                              "&szdf=" & Stringa_Codifica(Sezionale_ChkDefault, AgroKey_EncoderDecoder, Server)

                '###########################################################

            Case enum_CodificaStampe.Bilancio_Civilistico

                'If Year(Data_Inizio) <> Year(Data_Fine) Then
                '    AgroMsgBox("Attenzione, sono stati selezionati anni diversi tra la data di inizio e la data di fine.", Page)
                '    Exit Sub
                'End If

                TargetURL = "Bilancio/Bilancio.aspx"

                'Chk_CE
                'Chk_StatoPatrimoniale
                queryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                              "&a=" & Stringa_Codifica(AnnoPianoConti, AgroKey_EncoderDecoder, Server) &
                              "&es=" & Stringa_Codifica(Esercizio, AgroKey_EncoderDecoder, Server) &
                              "&r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                              "&di=" & Stringa_Codifica(Data_Inizio, AgroKey_EncoderDecoder, Server) &
                              "&df=" & Stringa_Codifica(Data_Fine, AgroKey_EncoderDecoder, Server) &
                              "&fc=" & Stringa_Codifica(Filtro_Conti, AgroKey_EncoderDecoder, Server) &
                              "&ue=" & Stringa_Codifica(Conti_UE_Tutti, AgroKey_EncoderDecoder, Server) &
                              "&chk_ce=" & Stringa_Codifica(Chk_CE, AgroKey_EncoderDecoder, Server) &
                              "&chk_sp=" & Stringa_Codifica(Chk_StatoPatrimoniale, AgroKey_EncoderDecoder, Server) &
                              "&chk_ns0=" & Stringa_Codifica(Flag_NoSaldo0, AgroKey_EncoderDecoder, Server) &
                              "&sa=" & Stringa_Codifica(Sintetico0_Analitico1, AgroKey_EncoderDecoder, Server) &
                              "&lce=" & Stringa_Codifica(CE_Layout_0Europeo_1CostiRicavi, AgroKey_EncoderDecoder, Server) &
                              "&gcfcsi=" & Stringa_Codifica(GestCont_Flag_ConsideraSaldiIniziali, AgroKey_EncoderDecoder, Server) &
                              "&gcdi=" & Stringa_Codifica(GestCont_DataInizio, AgroKey_EncoderDecoder, Server) &
                              "&szc=" & Stringa_Codifica(Sezionale_Cod, AgroKey_EncoderDecoder, Server) &
                              "&szd=" & Stringa_Codifica(Sezionale_Des, AgroKey_EncoderDecoder, Server) &
                              "&szdf=" & Stringa_Codifica(Sezionale_ChkDefault, AgroKey_EncoderDecoder, Server) &
                              "&sp_cc=" & Stringa_Codifica(SP_Crediti_Clienti, AgroKey_EncoderDecoder, Server) &
                              "&sp_b=" & Stringa_Codifica(SP_Banche, AgroKey_EncoderDecoder, Server) &
                              "&sp_c=" & Stringa_Codifica(SP_Casse, AgroKey_EncoderDecoder, Server) &
                              "&sp_df=" & Stringa_Codifica(SP_Debiti_Fornitori, AgroKey_EncoderDecoder, Server)


                '"&ue=" & Stringa_Codifica(Conti_UE_Tutti, AgroKey_EncoderDecoder, Server) &

                '"&mov=" & Stringa_Codifica(Conti_0Movimentati_1Tutti, AgroKey_EncoderDecoder, Server) '&
                '"&saldo=" & Stringa_Codifica(Conti_0ImponibileNoZero_1Tutti, AgroKey_EncoderDecoder, Server)


                '###########################################################

            Case enum_CodificaStampe.Bilanci_DiVerifica_Confronto

                TargetURL = "BilancioConfronto/BilancioConfronto.aspx"

                queryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                              "&rs=" & Stringa_Codifica(Qs_Rag_Soc, AgroKey_EncoderDecoder, Server) &
                              "&a=" & Stringa_Codifica(AnnoPianoConti, AgroKey_EncoderDecoder, Server) &
                              "&ac=" & Stringa_Codifica(AnnoConfronto, AgroKey_EncoderDecoder, Server) &
                              "&r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                              "&di=" & Stringa_Codifica(Data_Inizio, AgroKey_EncoderDecoder, Server) &
                              "&df=" & Stringa_Codifica(Data_Fine, AgroKey_EncoderDecoder, Server) &
                              "&fc=" & Stringa_Codifica(Filtro_Conti, AgroKey_EncoderDecoder, Server)


                '###########################################################

            Case enum_CodificaStampe.Lista_InsolutiClienti,
                enum_CodificaStampe.Lista_InsolutiFornitori

                TargetURL = "EstrattoConto_ClientiFornitori/Pagamenti.aspx"

                queryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                              "&a=" & Stringa_Codifica(AnnoPianoConti, AgroKey_EncoderDecoder, Server) &
                              "&r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                              "&di=" & Stringa_Codifica(Data_Inizio, AgroKey_EncoderDecoder, Server) &
                              "&df=" & Stringa_Codifica(Data_Fine, AgroKey_EncoderDecoder, Server) &
                              "&cr=" & Stringa_Codifica(Cod_Rapporto, AgroKey_EncoderDecoder, Server) &
                              "&cru=" & Stringa_Codifica(Cod_RisUm, AgroKey_EncoderDecoder, Server) &
                              "&tsc=" & Stringa_Codifica(Tipo_Scadenza, AgroKey_EncoderDecoder, Server) &
                              "&sc=" & Stringa_Codifica(Scadenza, AgroKey_EncoderDecoder, Server) &
                              "&tf=" & Stringa_Codifica(Tipo_Riscossione, AgroKey_EncoderDecoder, Server) &
                              "&cra=" & Stringa_Codifica(Cod_RisUm_Agente, AgroKey_EncoderDecoder, Server) &
                              "&ag=" & Stringa_Codifica(Nome_Agente, AgroKey_EncoderDecoder, Server) &
                              "&szc=" & Stringa_Codifica(Sezionale_Cod, AgroKey_EncoderDecoder, Server) &
                              "&szd=" & Stringa_Codifica(Sezionale_Des, AgroKey_EncoderDecoder, Server) &
                              "&tp=" & Stringa_Codifica(Lista_tipiPag, AgroKey_EncoderDecoder, Server) &
                              "&ord=" & Stringa_Codifica(Ordinamento, AgroKey_EncoderDecoder, Server)


                '###########################################################

            Case enum_CodificaStampe.EstrattoConto_Contatti

                'AgroMsgBox("Nuovo report in sviluppo.", Page)
                'Exit Sub

                TargetURL = "EstrattoConto_Contatti/EstrattoConto_Contatti.aspx"

                queryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                              "&a=" & Stringa_Codifica(AnnoPianoConti, AgroKey_EncoderDecoder, Server) &
                              "&r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                              "&di=" & Stringa_Codifica(Data_Inizio, AgroKey_EncoderDecoder, Server) &
                              "&df=" & Stringa_Codifica(Data_Fine, AgroKey_EncoderDecoder, Server) &
                              "&cc=" & Stringa_Codifica(cod_contatto, AgroKey_EncoderDecoder, Server) &
                              "&rsc=" & Stringa_Codifica(Me.cmb_Contatti2.SelectedItem.Text, AgroKey_EncoderDecoder, Server) &
                              "&gcfcsi=" & Stringa_Codifica(GestCont_Flag_ConsideraSaldiIniziali, AgroKey_EncoderDecoder, Server) &
                              "&gcdi=" & Stringa_Codifica(GestCont_DataInizio, AgroKey_EncoderDecoder, Server) &
                              "&szc=" & Stringa_Codifica(Sezionale_Cod, AgroKey_EncoderDecoder, Server) &
                              "&szd=" & Stringa_Codifica(Sezionale_Des, AgroKey_EncoderDecoder, Server) &
                              "&szdf=" & Stringa_Codifica(Sezionale_ChkDefault, AgroKey_EncoderDecoder, Server)

                'Stringa_Codifica(Cod_Rapporto, AgroKey_EncoderDecoder, Server) &
                '"&cru=" & Stringa_Codifica(Cod_RisUm, AgroKey_EncoderDecoder, Server) &
                '"&tsc=" & Stringa_Codifica(Tipo_Scadenza, AgroKey_EncoderDecoder, Server) &
                '"&sc=" & Stringa_Codifica(Scadenza, AgroKey_EncoderDecoder, Server) &
                '"&tf=" & Stringa_Codifica(Tipo_Riscossione, AgroKey_EncoderDecoder, Server) &
                '"&cra=" & Stringa_Codifica(Cod_RisUm_Agente, AgroKey_EncoderDecoder, Server) &
                '"&ag=" & Stringa_Codifica(Nome_Agente, AgroKey_EncoderDecoder, Server) &
                '"&ord=" & Stringa_Codifica(Ordinamento, AgroKey_EncoderDecoder, Server)

                '###########################################################

            Case enum_CodificaStampe.RiBa_Report_Presentazione

                TargetURL = "RegistroRiBa/Report_PresentazioneRIBA.aspx"

                queryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                              "&r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                              "&cr=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                              "&cru=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                              "&tsc=" & Stringa_Codifica(Tipo_Scadenza, AgroKey_EncoderDecoder, Server) &
                              "&sc=" & Stringa_Codifica(Scadenza, AgroKey_EncoderDecoder, Server) &
                              "&cid=" & Stringa_Codifica(Cod_Istituto_Impresa, AgroKey_EncoderDecoder, Server) &
                              "&cld=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                              "&cia=" & Stringa_Codifica(Cod_Istituto_Contatto, AgroKey_EncoderDecoder, Server) &
                              "&cla=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) &
                              "&flapag=" & Stringa_Codifica(True, AgroKey_EncoderDecoder, Server) &
                              "&dp=" & Stringa_Codifica(AGRODATAFINE, AgroKey_EncoderDecoder, Server) &
                              "&tnd=" & Stringa_Codifica(Tipo_NumeroDoc, AgroKey_EncoderDecoder, Server) &
                              "&nd=" & Stringa_Codifica(NumeroDoc, AgroKey_EncoderDecoder, Server) &
                              "&ad=" & Stringa_Codifica(AnnoDoc, AgroKey_EncoderDecoder, Server) &
                              "&tipo=" & Stringa_Codifica(Me.Rbl_RibaFatture.SelectedValue, AgroKey_EncoderDecoder, Server)


            Case enum_CodificaStampe.Registro_FattureAcquisto,
                enum_CodificaStampe.Registro_FattureVendita

                If Flag_NuovaVersioneRound = True Then
                    TargetURL = "RegistriIVA/Registri_IVA_3.aspx"
                Else
                    TargetURL = "RegistriIVA/Registri_IVA_2.aspx"
                End If

                queryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                              "&tipo=" & Stringa_Codifica(CStr(Tipologia), AgroKey_EncoderDecoder, Server) &
                              "&r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                              "&tri=" & Stringa_Codifica(Str_Trimestri, AgroKey_EncoderDecoder, Server) &
                              "&dm=" & Stringa_Codifica(Da_Mese, AgroKey_EncoderDecoder, Server) &
                              "&am=" & Stringa_Codifica(A_Mese, AgroKey_EncoderDecoder, Server) &
                              "&a=" & Stringa_Codifica(AnnoRegIva, AgroKey_EncoderDecoder, Server) &
                              "&szc=" & Stringa_Codifica(Sezionale_Cod, AgroKey_EncoderDecoder, Server) &
                              "&szd=" & Stringa_Codifica(Sezionale_Des, AgroKey_EncoderDecoder, Server) &
                              "&np=" & Stringa_Codifica(Num_Pagina, AgroKey_EncoderDecoder, Server) &
                              "&fsd=" & Stringa_Codifica(Me.Chk_DataStampa.Checked, AgroKey_EncoderDecoder, Server) &
                              "&ord=" & Stringa_Codifica(Ordinamento, AgroKey_EncoderDecoder, Server) &
                                 "&pmr=" & Stringa_Codifica(Param_Rag_Soc, AgroKey_EncoderDecoder, Server) &
                                 "&pmp=" & Stringa_Codifica(Param_Piva_CodFiscale, AgroKey_EncoderDecoder, Server) &
                                 "&pmi=" & Stringa_Codifica(Param_Indirizzo, AgroKey_EncoderDecoder, Server) &
                                 "&pmd=" & Stringa_Codifica(Param_Intervallo_Date, AgroKey_EncoderDecoder, Server)


            Case enum_CodificaStampe.Registro_Corrispettivi

                If Flag_NuovaVersioneRound = True Then
                    'TargetURL = "RegistroCorrispettivi/Registro_Corrispettivi_3.aspx"
                    TargetURL = "RegistroCorrispettivi/RegistroCorrispettiviNew.aspx"

                    queryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                         "&di=" & Stringa_Codifica(Data_Inizio, AgroKey_EncoderDecoder, Server) &
                         "&df=" & Stringa_Codifica(Data_Fine, AgroKey_EncoderDecoder, Server) &
                        "&szc=" & Stringa_Codifica(Sezionale_Cod, AgroKey_EncoderDecoder, Server) &
                        "&szd=" & Stringa_Codifica(Sezionale_Des, AgroKey_EncoderDecoder, Server) &
                         "&np=" & Stringa_Codifica(Num_Pagina, AgroKey_EncoderDecoder, Server) &
                         "&fsn=" & Stringa_Codifica(Me.Chk_NoteCorrispettivi.Checked, AgroKey_EncoderDecoder, Server) &
                         "&fsd=" & Stringa_Codifica(Me.Chk_DataStampa.Checked, AgroKey_EncoderDecoder, Server) &
                         "&ri=" & Stringa_Codifica(Me.Chk_StampaRiepilogo.Checked, AgroKey_EncoderDecoder, Server) &
                         "&pmr=" & Stringa_Codifica(Param_Rag_Soc, AgroKey_EncoderDecoder, Server) &
                         "&pmp=" & Stringa_Codifica(Param_Piva_CodFiscale, AgroKey_EncoderDecoder, Server) &
                         "&pmi=" & Stringa_Codifica(Param_Indirizzo, AgroKey_EncoderDecoder, Server) &
                         "&pmd=" & Stringa_Codifica(Param_Intervallo_Date, AgroKey_EncoderDecoder, Server)

                Else
                    TargetURL = "RegistroCorrispettivi/Registro_Corrispettivi_2.aspx"

                    Dim Cod_iva_1, Cod_iva_2, Cod_iva_3, Cod_iva_4, Cod_iva_non_imp_1, Cod_iva_non_imp_2, Cod_iva_escl_iva_1, Cod_iva_escl_iva_2 As Integer

                    VerificaAliquoteCorrisp(2,
                                            Cod_iva_1,
                                            Cod_iva_2,
                                            Cod_iva_3,
                                            Cod_iva_4,
                                            Cod_iva_non_imp_1,
                                            Cod_iva_non_imp_2,
                                            Cod_iva_escl_iva_1,
                                            Cod_iva_escl_iva_2)

                    queryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                                  "&di=" & Stringa_Codifica(Data_Inizio, AgroKey_EncoderDecoder, Server) &
                                  "&df=" & Stringa_Codifica(Data_Fine, AgroKey_EncoderDecoder, Server) &
                                  "&sez=" & Stringa_Codifica(Sezionale_Cod, AgroKey_EncoderDecoder, Server) &
                                  "&np=" & Stringa_Codifica(Num_Pagina, AgroKey_EncoderDecoder, Server) &
                                  "&ci1=" & Stringa_Codifica(Cod_iva_1, AgroKey_EncoderDecoder, Server) &
                                  "&ci2=" & Stringa_Codifica(Cod_iva_2, AgroKey_EncoderDecoder, Server) &
                                  "&ci3=" & Stringa_Codifica(Cod_iva_3, AgroKey_EncoderDecoder, Server) &
                                  "&ci4=" & Stringa_Codifica(Cod_iva_4, AgroKey_EncoderDecoder, Server) &
                                  "&ni1=" & Stringa_Codifica(Cod_iva_non_imp_1, AgroKey_EncoderDecoder, Server) &
                                  "&ni2=" & Stringa_Codifica(Cod_iva_non_imp_2, AgroKey_EncoderDecoder, Server) &
                                  "&ei1=" & Stringa_Codifica(Cod_iva_escl_iva_1, AgroKey_EncoderDecoder, Server) &
                                  "&ei2=" & Stringa_Codifica(Cod_iva_escl_iva_2, AgroKey_EncoderDecoder, Server) &
                                  "&fsn=" & Stringa_Codifica(Me.Chk_NoteCorrispettivi.Checked, AgroKey_EncoderDecoder, Server) &
                                  "&ri=" & Stringa_Codifica(Me.Chk_StampaRiepilogo.Checked, AgroKey_EncoderDecoder, Server)

                End If

        End Select

        If PDF = True Then
            queryString &= "&PDF=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server)
        Else
            queryString &= "&ForzaAnteprima=" & Stringa_Codifica("true", AgroKey_EncoderDecoder, Server)
        End If

        Page_NewWindow(Page, TargetURL, queryString, Pagina_Titolo, , , , , , , , )

    End Sub


    '##############################################################
    Private Sub StampaDirettaMastrino()

        Dim GestCont_Flag_ConsideraSaldiIniziali As Boolean '= True
        Dim GestCont_DataInizio As Date '= AGRODATAINIZIO

        '   Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        'objImprese.LeggiOpzioni_InizioGestioneContabile(_objParametriServer,
        '                                                Qs_Piva,
        '                                                GestCont_Flag_ConsideraSaldiIniziali,
        '                                                GestCont_DataInizio)

        GestCont_DataInizio = ViewState("GestCont_DataInizio")
        GestCont_Flag_ConsideraSaldiIniziali = ViewState("GestCont_Flag_ConsideraSaldiIniziali")

        If CDate(Qs_Di) < GestCont_DataInizio And CDate(Qs_Df) < GestCont_DataInizio Then
            AgroMsgBox("L'intervallo selezionato per la stampa è antecedente alla data di inizio gestione contabile: " & GestCont_DataInizio.ToShortDateString, Page)
            Exit Sub
        End If

        Dim querystring As String
        querystring = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                      "&a=" & Stringa_Codifica(Qs_Anno, AgroKey_EncoderDecoder, Server) &
                      "&r=" & Stringa_Codifica(Report, AgroKey_EncoderDecoder, Server) &
                      "&di=" & Stringa_Codifica(Qs_Di, AgroKey_EncoderDecoder, Server) &
                      "&df=" & Stringa_Codifica(Qs_Df, AgroKey_EncoderDecoder, Server) &
                      "&chk_ce=" & Stringa_Codifica(Qs_Chk_CE, AgroKey_EncoderDecoder, Server) &
                      "&chk_sp=" & Stringa_Codifica(Qs_Chk_SP, AgroKey_EncoderDecoder, Server) &
                      "&rce=" & Stringa_Codifica(Qs_Ric_Cod_Eco, AgroKey_EncoderDecoder, Server) &
                      "&cce=" & Stringa_Codifica(Qs_Cod_Conto_Eco, AgroKey_EncoderDecoder, Server) &
                      "&rcp=" & Stringa_Codifica(Qs_Ric_Cod_Pat, AgroKey_EncoderDecoder, Server) &
                      "&ccp=" & Stringa_Codifica(Qs_Cod_Conto_Pat, AgroKey_EncoderDecoder, Server) &
                      "&cru=" & Stringa_Codifica(Qs_Cod_RisUm, AgroKey_EncoderDecoder, Server) &
                      "&liq=" & Stringa_Codifica(Qs_Cod_Liquidita, AgroKey_EncoderDecoder, Server) &
                      "&gcfcsi=" & Stringa_Codifica(GestCont_Flag_ConsideraSaldiIniziali, AgroKey_EncoderDecoder, Server) &
                      "&gcdi=" & Stringa_Codifica(GestCont_DataInizio, AgroKey_EncoderDecoder, Server) &
                      "&szc=" & Stringa_Codifica(SEZIONALE_NOFILTRO, AgroKey_EncoderDecoder, Server) &
                      "&szd=" & Stringa_Codifica("", AgroKey_EncoderDecoder, Server) &
                      "&szdf=" & Stringa_Codifica(-1, AgroKey_EncoderDecoder, Server)

        'Page_NewWindow(Page, "BilancioMatrino/Mastrino.aspx", Querystring, "Mastrino_Contabile", , , , , , , , )

        Response.Redirect("BilancioMatrino/Mastrino.aspx" & querystring)

    End Sub

    '#####################################################################
    Private Sub Database_OperazioniPreliminari(ByRef Log_Errori As String)

        '-------------------------------------------------------------------------------------------------------------
        '---- inserimento cassa in ist_credito per poter fare gli inner join tra liquidita e ist_credito -------------
        '-------------------------------------------------------------------------------------------------------------
        Dim objPag As New AgronicaCoreContabBIZ.Pagamento_R
        objPag.Verifica_CASSA(_objParametriServer)

        '-----------------------------------------------------------------------------
        '---- Query di update per sistemare dati salvati male dal GiasLan   -------------
        '------------------------------------------------------------------------------
        Dim objPagR As New AgronicaCoreContabDAL.Pagamenti_R
        Dim objPagW As New AgronicaCoreContabDAL.Pagamenti_W
        Dim Dt As DataTable
        Dim LogPagamenti As String = ""
        Dim j As Integer

        '/////////////////////////////////
        '//////// VERIFICA TIPO COD AVERE///////////
        '/////////////////////////////////

        Try

            Dt = objPagR.LeggiXUpdate_TipoCodAvere_PagamentiDoc(_objParametriServer)

            If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

                LogPagamenti &= "Verifica tabella Pagamenti (tipo_cod_avere)." & vbCrLf
                LogPagamenti &= "Ci sono " & CStr(Dt.Rows.Count) & " record da sistemare." & vbCrLf

                For j = 0 To Dt.Rows.Count - 1

                    LogPagamenti &= "id_agenda= " & CStr(Dt.Rows(j).Item("id_agenda")) & " - " &
                                    "lav_cod= " & CStr(Dt.Rows(j).Item("lav_cod")) & " - " &
                                    "des_lib= " & CStr(Dt.Rows(j).Item("des_lib")) & " - " &
                                    "data= " & CStr(Dt.Rows(j).Item("validita_inizio")) & " - " &
                                    "data_pagamento= " & CStr(Dt.Rows(j).Item("data_pagamento")) & " " &
                                    "cod_conto_pat_avere= " & CStr(Dt.Rows(j).Item("cod_conto_pat_avere")) & " " &
                                    "tipo_cod_dare= " & CStr(Dt.Rows(j).Item("tipo_cod_dare")) & " " &
                                    "tipo_cod_avere= " & CStr(Dt.Rows(j).Item("tipo_cod_avere")) & " " &
                                    vbCrLf & vbCrLf


                Next

                objPagW.Update_TipoCodAvere_PagamentiDoc(_objParametriServer)

                LogPagamenti &= "Query di update tipo_cod_avere avvenuta."

            End If

        Catch ex As Exception
            LogPagamenti &= "tabella Pagamenti (tipo_cod_avere), errore: " & vbCrLf & ex.Message & vbCrLf
            Log_Errori &= "tabella Pagamenti (tipo_cod_avere), errore: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '///////////////////////////////////////////////////
        '//////// VERIFICA PAGAMENTI NDC RICEVUTE///////////
        '//////////////////////////////////////////////////

        Try

            Dt = objPagR.LeggiXUpdate_PagamentiNoteAccreditoRicevute(_objParametriServer)

            If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

                LogPagamenti &= "Verifica tabella Pagamenti (PagamentiNoteAccreditoRicevute)." & vbCrLf
                LogPagamenti &= "Ci sono " & CStr(Dt.Rows.Count) & " record da sistemare." & vbCrLf

                For j = 0 To Dt.Rows.Count - 1

                    LogPagamenti &= "id_agenda= " & CStr(Dt.Rows(j).Item("id_agenda")) & " - " &
                                    "id_mov= " & CStr(Dt.Rows(j).Item("id_mov")) & " - " &
                                    "data_pagamento= " & CStr(Dt.Rows(j).Item("data_pagamento")) & " " &
                                    "cod_conto_pat_avere= " & CStr(Dt.Rows(j).Item("cod_conto_pat_avere")) & " " &
                                    vbCrLf & vbCrLf


                Next

                objPagW.Update_CodContoPatAvere_NoteAccreditoRicevute(_objParametriServer)

                LogPagamenti &= "Query di update tabella Pagamenti (NoteAccreditoRicevute)."

            End If

        Catch ex As Exception
            LogPagamenti &= "tabella Pagamenti (NoteAccreditoRicevute), errore: " & vbCrLf & ex.Message & vbCrLf
            Log_Errori &= "tabella Pagamenti (NoteAccreditoRicevute), errore: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '///////////////////////////////////////////////////
        '//////// VERIFICA PAGAMENTI NDC EMESSE///////////
        '//////////////////////////////////////////////////

        Try

            Dt = objPagR.LeggiXUpdate_PagamentiNoteAccreditoEmesse(_objParametriServer)

            If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

                LogPagamenti &= "Verifica tabella Pagamenti (PagamentiNoteAccreditoEmesse)." & vbCrLf
                LogPagamenti &= "Ci sono " & CStr(Dt.Rows.Count) & " record da sistemare." & vbCrLf

                For j = 0 To Dt.Rows.Count - 1

                    LogPagamenti &= "id_agenda= " & CStr(Dt.Rows(j).Item("id_agenda")) & " - " &
                                    "id_mov= " & CStr(Dt.Rows(j).Item("id_mov")) & " - " &
                                    "data_pagamento= " & CStr(Dt.Rows(j).Item("data_pagamento")) & " " &
                                    "cod_conto_pat_dare= " & CStr(Dt.Rows(j).Item("cod_conto_pat_dare")) & " " &
                                    vbCrLf & vbCrLf


                Next

                objPagW.Update_CodContoPatDare_NoteAccreditoEmesse(_objParametriServer)

                LogPagamenti &= "Query di update tabella Pagamenti (NoteAccreditoEmesse)."

            End If

        Catch ex As Exception
            LogPagamenti &= "tabella Pagamenti (NoteAccreditoEmesse), errore: " & vbCrLf & ex.Message & vbCrLf
            Log_Errori &= "tabella Pagamenti (NoteAccreditoEmesse), errore: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '/////////////////////////////////
        '//////// VERIFICA ANNO ///////////
        '/////////////////////////////////

        Try

            Dt = objPagR.LeggiXUpdate_Anno_PagamentiDoc(_objParametriServer)

            If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

                LogPagamenti &= "Verifica tabella Pagamenti (Anno)." & vbCrLf
                LogPagamenti &= "Ci sono " & CStr(Dt.Rows.Count) & " record da sistemare." & vbCrLf

                For j = 0 To Dt.Rows.Count - 1

                    LogPagamenti &= "id_agenda= " & CStr(Dt.Rows(j).Item("id_agenda")) & " - " &
                                    "lav_cod= " & CStr(Dt.Rows(j).Item("lav_cod")) & " - " &
                                    "des_lib= " & CStr(Dt.Rows(j).Item("des_lib")) & " - " &
                                    "data= " & CStr(Dt.Rows(j).Item("validita_inizio")) & " - " &
                                    "data_pagamento= " & CStr(Dt.Rows(j).Item("data_pagamento")) & " " &
                                    "anno= " & CStr(Dt.Rows(j).Item("anno")) & " " &
                                    vbCrLf & vbCrLf

                Next

                objPagW.Update_Anno_PagamentiDoc(_objParametriServer)

                LogPagamenti &= "Query di update Anno avvenuta."

            End If

        Catch ex As Exception
            LogPagamenti &= "tabella Pagamenti (anno), errore: " & vbCrLf & ex.Message & vbCrLf
            Log_Errori &= "tabella Pagamenti (anno), errore: " & vbCrLf & ex.Message & vbCrLf
        End Try


        If LogPagamenti <> "" Then

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(_objParametriServer,
                                             "Database_LogErrori",
                                             _objParametriServer.PivaSuperUser & "_" & Session("ASG_Utente_Username") & "_TabellaPagamenti.txt",
                                             Session("ASG_Utente_Username"),
                                             "Verifica tipo_Cod_Avere",
                                             LogPagamenti)

        End If

        '-----------------------------------------
        '21/03/2016: LEGGIMI
        'questa parte è stata commentata visto che dalla versione del giaslan 21/03/2016 è possibile:
        '- inserire nuovi conti e automaticamente questi vengono inseriti sul piano dei conti di tutti gli anni
        '- codificare i conti patrimoniali ed economici con quelli gias


        ''-----------------------------------------------------------------------------
        ''---- Query di insert CONTI PATRIMONIALI E ECONOMICI -------------
        ''------------------------------------------------------------------------------

        ''verifico se è in uso il piano dei conti personalizzato o quello base di gias
        'Dim objRic As New AgronicaCoreContabDAL.Riclassificazioni_Patrimonio_R
        'Dim LogConti As String = ""
        'Dim Piva_riferimento As String
        'Piva_riferimento = objRic.Leggi_PivaRiferimento(Qs_Piva, BILANCIO_PERSONALIZZATO, "", _objParametriServer)

        'ViewState("Piva_riferimento") = Piva_riferimento

        'If Piva_riferimento = PIVA_BILANCIO_EUROPEO Then

        '    '---------------------------------------------------
        '    '------- PIANO DEI CONTI SP EUROPEO - GIAS -------------
        '    '---------------------------------------------------

        '    Dim objRicConti_R As New AgronicaCoreContabDAL.RicxConti_Patrimonio_R

        '    Try

        '        Dt = objRicConti_R.LeggiDistinctAnno(Qs_Piva, BILANCIO_PERSONALIZZATO, "", _objParametriServer)

        '        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

        '            'conti Gias
        '            'verifico che ci siano i codici di codifica valorizzati
        '            Dim objRicConti_W As New AgronicaCoreContabDAL.RicxConti_Patrimonio_W
        '            Dim risp As Boolean

        '            Try
        '                risp = objRicConti_W.Update_CodificaContoPat_Base(_objParametriServer)

        '            Catch ex As Exception
        '                Log_Errori &= "verifica CodificaContoPat: " & vbCrLf & ex.Message & vbCrLf
        '                LogConti &= "verifica CodificaContoPat: " & vbCrLf & ex.Message & vbCrLf
        '            End Try

        '            ' LogPagamenti &= "Ci sono " & CStr(Dt.Rows.Count) & " record da sistemare." & vbCrLf
        '            Dim anno As Integer
        '            Dim flag_esiste As Boolean

        '            For j = 0 To Dt.Rows.Count - 1

        '                'azzero a ogni giro
        '                flag_esiste = False

        '                anno = Dt.Rows(j).Item("Anno")

        '                'verifica se per ogni anno sono stati inseriti i codici dei conti iva
        '                'se non presenti li inserisco

        '                Try

        '                    '--------------------------------------------
        '                    '--------- IVA A CREDITO -----------
        '                    '--------------------------------------------
        '                    flag_esiste = objRicConti_R.Esiste_RicXConto(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Patrimoniali.IvaACredito, anno, "", _objParametriServer)

        '                    If flag_esiste = False Then

        '                        LogConti &= "piva= " & CStr(Qs_Piva) & " - " & _
        '                                        "ric_cod_pat= " & CStr(BILANCIO_PERSONALIZZATO) & " - " & _
        '                                        "anno= " & CStr(anno) & " - " & _
        '                                        "cod_conto_pat= " & CStr(enum_Conti_Patrimoniali.IvaACredito) & " " & _
        '                                        "IVA A CREDITO non presente. " & _
        '                                        vbCrLf & vbCrLf

        '                        objRicConti_W.Scrivi(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Patrimoniali.IvaACredito, anno, _
        '                                             GIAS_Id_Riclassificazione_IvaACredito, _
        '                                             "D", 0, 0, 0, SP_CONTO_IMPUTABILE, _
        '                                             enum_Conti_Patrimoniali.IvaACredito,
        '                                             AGRODATAINIZIO, AGRODATAFINE, _objParametriServer)


        '                    End If

        '                Catch ex As Exception
        '                    Log_Errori &= "verifica conto IVA A CREDITO: " & vbCrLf & ex.Message & vbCrLf
        '                    LogConti &= "verifica conto IVA A CREDITO: " & vbCrLf & ex.Message & vbCrLf
        '                End Try


        '                Try

        '                    '--------------------------------------------
        '                    '--------- IVA A CREDITO ACQUISTI INTRACOM -----------
        '                    '--------------------------------------------
        '                    flag_esiste = objRicConti_R.Esiste_RicXConto(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Patrimoniali.IvaACreditoAcqIntra, anno, "", _objParametriServer)

        '                    If flag_esiste = False Then

        '                        LogConti &= "piva= " & CStr(Qs_Piva) & " - " & _
        '                                        "ric_cod_pat= " & CStr(BILANCIO_PERSONALIZZATO) & " - " & _
        '                                        "anno= " & CStr(anno) & " - " & _
        '                                        "cod_conto_pat= " & CStr(enum_Conti_Patrimoniali.IvaACreditoAcqIntra) & " " & _
        '                                        "iva a credito x acquisti intra non presente. " & _
        '                                        vbCrLf & vbCrLf

        '                        objRicConti_W.Scrivi(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Patrimoniali.IvaACreditoAcqIntra, anno, _
        '                                             GIAS_Id_Riclassificazione_IvaACredito_AcqIntra, _
        '                                             "D", 0, 0, 0, SP_CONTO_IMPUTABILE, _
        '                                             enum_Conti_Patrimoniali.IvaACreditoAcqIntra,
        '                                             AGRODATAINIZIO, AGRODATAFINE, _objParametriServer)

        '                    End If

        '                Catch ex As Exception
        '                    Log_Errori &= "verifica conto IVA A CREDITO X ACQUISTI INTRA: " & vbCrLf & ex.Message & vbCrLf
        '                    LogConti &= "verifica conto IVA A CREDITO X ACQUISTI INTRA: " & vbCrLf & ex.Message & vbCrLf
        '                End Try


        '                Try

        '                    '--------------------------------------------
        '                    '--------- IVA A DEBITO -----------
        '                    '--------------------------------------------
        '                    flag_esiste = objRicConti_R.Esiste_RicXConto(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Patrimoniali.IvaADebito, anno, "", _objParametriServer)

        '                    If flag_esiste = False Then

        '                        LogConti &= "piva= " & CStr(Qs_Piva) & " - " & _
        '                                        "ric_cod_pat= " & CStr(BILANCIO_PERSONALIZZATO) & " - " & _
        '                                        "anno= " & CStr(anno) & " - " & _
        '                                        "cod_conto_pat= " & CStr(enum_Conti_Patrimoniali.IvaADebito) & " " & _
        '                                        "iva a DEBITO non presente. " & _
        '                                        vbCrLf & vbCrLf

        '                        objRicConti_W.Scrivi(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Patrimoniali.IvaADebito, anno, _
        '                                             GIAS_Id_Riclassificazione_IvaADebito, _
        '                                             "A", 0, 0, 0, SP_CONTO_IMPUTABILE, _
        '                                             enum_Conti_Patrimoniali.IvaADebito,
        '                                             AGRODATAINIZIO, AGRODATAFINE, _objParametriServer)

        '                    End If

        '                Catch ex As Exception
        '                    Log_Errori &= "verifica conto IVA A DEBITO: " & vbCrLf & ex.Message & vbCrLf
        '                    LogConti &= "verifica conto IVA A DEBITO: " & vbCrLf & ex.Message & vbCrLf
        '                End Try

        '                Try

        '                    '--------------------------------------------
        '                    '--------- IVA A DEBITO X ACQUISTI INTRACOM -----------
        '                    '--------------------------------------------
        '                    flag_esiste = objRicConti_R.Esiste_RicXConto(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Patrimoniali.IvaADebitoAcqIntra, anno, "", _objParametriServer)

        '                    If flag_esiste = False Then

        '                        LogConti &= "piva= " & CStr(Qs_Piva) & " - " & _
        '                                        "ric_cod_pat= " & CStr(BILANCIO_PERSONALIZZATO) & " - " & _
        '                                        "anno= " & CStr(anno) & " - " & _
        '                                        "cod_conto_pat= " & CStr(enum_Conti_Patrimoniali.IvaADebitoAcqIntra) & " " & _
        '                                        "IVA A DEBITO non presente. " & _
        '                                        vbCrLf & vbCrLf

        '                        objRicConti_W.Scrivi(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Patrimoniali.IvaADebitoAcqIntra, anno, _
        '                                             GIAS_Id_Riclassificazione_IvaADebito_AcqIntra, _
        '                                             "A", 0, 0, 0, SP_CONTO_IMPUTABILE, _
        '                                             enum_Conti_Patrimoniali.IvaADebitoAcqIntra,
        '                                             AGRODATAINIZIO, AGRODATAFINE, _objParametriServer)

        '                    End If

        '                Catch ex As Exception
        '                    Log_Errori &= "verifica conto IVA A DEBITO X ACQUISTI INTRA: " & vbCrLf & ex.Message & vbCrLf
        '                    LogConti &= "verifica conto IVA A DEBITO X ACQUISTI INTRA: " & vbCrLf & ex.Message & vbCrLf
        '                End Try

        '                Try

        '                    '--------------------------------------------
        '                    '--------- ErarioRitenuteLavoroAutonomo -----------
        '                    '--------------------------------------------
        '                    flag_esiste = objRicConti_R.Esiste_RicXConto(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Patrimoniali.ErarioRitenuteLavoroAutonomo, anno, "", _objParametriServer)

        '                    If flag_esiste = False Then

        '                        LogConti &= "piva= " & CStr(Qs_Piva) & " - " & _
        '                                        "ric_cod_pat= " & CStr(BILANCIO_PERSONALIZZATO) & " - " & _
        '                                        "anno= " & CStr(anno) & " - " & _
        '                                        "cod_conto_pat= " & CStr(enum_Conti_Patrimoniali.ErarioRitenuteLavoroAutonomo) & " " & _
        '                                        "ErarioRitenuteLavoroAutonomo non presente. " & _
        '                                        vbCrLf & vbCrLf

        '                        objRicConti_W.Scrivi(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Patrimoniali.ErarioRitenuteLavoroAutonomo, anno, _
        '                                              GIAS_Id_Riclassificazione_ErarioRitenuteLavoroAutonomo, _
        '                                             "A", 0, 0, 0, SP_CONTO_IMPUTABILE, _
        '                                             enum_Conti_Patrimoniali.ErarioRitenuteLavoroAutonomo, _
        '                                             AGRODATAINIZIO, AGRODATAFINE, _objParametriServer)

        '                    End If

        '                Catch ex As Exception
        '                    Log_Errori &= "verifica conto ErarioRitenuteLavoroAutonomo: " & vbCrLf & ex.Message & vbCrLf
        '                    LogConti &= "verifica conto ErarioRitenuteLavoroAutonomo: " & vbCrLf & ex.Message & vbCrLf
        '                End Try


        '                Try

        '                    '--------------------------------------------
        '                    '--------- DebitiVsEnasarco -----------
        '                    '--------------------------------------------
        '                    flag_esiste = objRicConti_R.Esiste_RicXConto(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Patrimoniali.DebitiVsEnasarco, anno, "", _objParametriServer)

        '                    If flag_esiste = False Then

        '                        LogConti &= "piva= " & CStr(Qs_Piva) & " - " & _
        '                                        "ric_cod_pat= " & CStr(BILANCIO_PERSONALIZZATO) & " - " & _
        '                                        "anno= " & CStr(anno) & " - " & _
        '                                        "cod_conto_pat= " & CStr(enum_Conti_Patrimoniali.DebitiVsEnasarco) & " " & _
        '                                        "DebitiVsEnasarco non presente. " & _
        '                                        vbCrLf & vbCrLf

        '                        objRicConti_W.Scrivi(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Patrimoniali.DebitiVsEnasarco, anno, _
        '                                              GIAS_Id_Riclassificazione_DebitiVsEnasarco, _
        '                                             "A", 0, 0, 0, SP_CONTO_IMPUTABILE, _
        '                                             enum_Conti_Patrimoniali.DebitiVsEnasarco,
        '                                             AGRODATAINIZIO, AGRODATAFINE, _objParametriServer)

        '                    End If

        '                Catch ex As Exception
        '                    Log_Errori &= "verifica conto DebitiVsEnasarco: " & vbCrLf & ex.Message & vbCrLf
        '                    LogConti &= "verifica conto DebitiVsEnasarco: " & vbCrLf & ex.Message & vbCrLf
        '                End Try


        '            Next 'PER OGNI ANNO DI RICXCONTI

        '        End If 'SP

        '    Catch ex As Exception
        '        Log_Errori &= "RicxConti_Patrimonio, lettura anni: " & vbCrLf & ex.Message & vbCrLf
        '        LogConti &= "RicxConti_Patrimonio, lettura anni: " & vbCrLf & ex.Message & vbCrLf
        '    End Try


        '    '---------------------------------------------------
        '    '------- PIANO DEI CONTI CE EUROPEO - GIAS -------------
        '    '---------------------------------------------------

        '    Dim objRicContiEco_R As New AgronicaCoreContabDAL.RicxConti_R

        '    Try

        '        Dt = objRicContiEco_R.LeggiDistinctAnno(Qs_Piva, BILANCIO_PERSONALIZZATO, "", _objParametriServer)

        '        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

        '            Dim objRicConti_W As New AgronicaCoreContabDAL.RicxConti_W

        '            ''NON STO A FARLO, TANTO SONO STATI INSERITI DI RECENTE DAL MIGRA E HANNO GIA' LA CODIFICA
        '            ''conti Gias
        '            ''verifico che ci siano i codici di codifica valorizzati
        '            'Dim risp As Boolean

        '            'Try
        '            '    risp = objRicConti_W.Update_CodificaConto_Base(_objParametriServer)

        '            'Catch ex As Exception
        '            '    Log_Errori &= "verifica CodificaConto conti base: " & vbCrLf & ex.Message & vbCrLf
        '            '    LogConti &= "verifica CodificaConto conti base: " & vbCrLf & ex.Message & vbCrLf
        '            'End Try

        '            ' LogPagamenti &= "Ci sono " & CStr(Dt.Rows.Count) & " record da sistemare." & vbCrLf
        '            Dim anno As Integer
        '            Dim flag_esiste As Boolean

        '            For j = 0 To Dt.Rows.Count - 1

        '                'azzero a ogni giro
        '                flag_esiste = False

        '                anno = Dt.Rows(j).Item("Anno")

        '                'verifica se per ogni anno sono stati inseriti i codici dei conti 
        '                'se non presenti li inserisco

        '                Try

        '                    '--------------------------------------------
        '                    '--------- OMAGGI ALLA CLIENTELA -----------
        '                    '--------------------------------------------
        '                    flag_esiste = objRicContiEco_R.Esiste_Codifica_Conto(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Economici.OmaggiAllaClientela, anno, "", _objParametriServer)

        '                    If flag_esiste = False Then

        '                        LogConti &= "piva= " & CStr(Qs_Piva) & " - " & _
        '                                        "ric_cod= " & CStr(BILANCIO_PERSONALIZZATO) & " - " & _
        '                                        "anno= " & CStr(anno) & " - " & _
        '                                        "cod_conto= " & CStr(enum_Conti_Economici.OmaggiAllaClientela) & " " & _
        '                                        "OMAGGI ALLA CLIENTELA non presente. " & _
        '                                        vbCrLf & vbCrLf

        '                        objRicConti_W.Scrivi(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Economici.OmaggiAllaClientela, anno, _
        '                                             GIAS_Id_Riclassificazione_OmaggiAllaClientela, _
        '                                             "D", 0, CE_CONTO_IMPUTABILE, _
        '                                             0, enum_Conti_Economici.OmaggiAllaClientela, 0, _
        '                                             AGRODATAINIZIO, AGRODATAFINE, _
        '                                             _objParametriServer)

        '                    End If

        '                Catch ex As Exception
        '                    Log_Errori &= "verifica conto OmaggiAllaClientela: " & vbCrLf & ex.Message & vbCrLf
        '                    LogConti &= "verifica conto OmaggiAllaClientela: " & vbCrLf & ex.Message & vbCrLf
        '                End Try


        '                Try

        '                    '--------------------------------------------
        '                    '--------- RICAVI X IVA IN COMPENSAZIONE -----------
        '                    '--------------------------------------------
        '                    flag_esiste = objRicContiEco_R.Esiste_Codifica_Conto(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Economici.RicavixIVAincompensazione, anno, "", _objParametriServer)

        '                    If flag_esiste = False Then

        '                        LogConti &= "piva= " & CStr(Qs_Piva) & " - " & _
        '                                        "ric_cod= " & CStr(BILANCIO_PERSONALIZZATO) & " - " & _
        '                                        "anno= " & CStr(anno) & " - " & _
        '                                        "cod_conto= " & CStr(enum_Conti_Economici.OmaggiAllaClientela) & " " & _
        '                                        "RicavixIVAincompensazione non presente. " & _
        '                                        vbCrLf & vbCrLf

        '                        objRicConti_W.Scrivi(Qs_Piva, BILANCIO_PERSONALIZZATO, enum_Conti_Economici.RicavixIVAincompensazione, anno, _
        '                                             GIAS_Id_Riclassificazione_RicavixIVAincompensazione, _
        '                                             "A", 0, CE_CONTO_IMPUTABILE, _
        '                                             0, enum_Conti_Economici.RicavixIVAincompensazione, 0, _
        '                                             AGRODATAINIZIO, AGRODATAFINE, _
        '                                             _objParametriServer)

        '                    End If

        '                Catch ex As Exception
        '                    Log_Errori &= "verifica conto RicavixIVAincompensazione: " & vbCrLf & ex.Message & vbCrLf
        '                    LogConti &= "verifica conto RicavixIVAincompensazione: " & vbCrLf & ex.Message & vbCrLf
        '                End Try


        '            Next 'PER OGNI ANNO DI RICXCONTI

        '        End If 'CE

        '    Catch ex As Exception
        '        Log_Errori &= "RicxConti, lettura anni: " & vbCrLf & ex.Message & vbCrLf
        '        LogConti &= "RicxConti, lettura anni: " & vbCrLf & ex.Message & vbCrLf
        '    End Try


        'Else

        '    '---------------------------------------------------
        '    '------- PIANO DEI CONTI PERSONALIZZATO -------------
        '    '---------------------------------------------------

        '    'non devo fare le insert

        '    'il controllo sulle codifiche lo faccio fare alla stampa

        'End If 'PIVA_Riferimento



        'If LogConti <> "" Then

        '    Dim objLog As New GestioneLogStampe
        '    objLog.Gestione_LogErrori_Stampe(_objParametriServer,
        '                                     "Database_LogErrori",
        '                                     _objParametriServer.PivaSuperUser & "_" & Session("ASG_Utente_Username") & "_TabellaRicXconti_Patrimonio.txt",
        '                                     Session("ASG_Utente_Username"),
        '                                     "Verifica IVA NS CREDITO e DEBITO",
        '                                     LogConti)

        'End If

        '-----------------------------------------

    End Sub

End Class
