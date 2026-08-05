Imports System.Xml
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility


Public Class MonitoraggioCE_Edit
    Inherits System.Web.UI.Page

    Dim objParametri_Utenti As AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreParametri

    '----- Gestione Querystring
    Dim Qs_Piva As String

    Dim P_get As String
    Dim saCod_get As Integer
    Dim idAgenda_get As Integer
    Dim idMov_get As Integer
    Dim stabSaCod_get As Integer
    Dim stabFabbr_get As Integer
    Dim act As String


    '##########################################################################################################
    Private Sub GestioneCE_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
    End Sub

    '################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

         Dim QueryString As String

        QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server)
        Response.Redirect("./GestioneMonitoraggioCE.aspx" & QueryString)

    End Sub


    '##################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

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

            objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
            objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))


            '##############################################################
            '#####  Recupero le informazioni dal DB  ######################
            '##############################################################

            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, AgroKey_EncoderDecoder)

            If Not IsNothing(Request.QueryString("act")) Then

                act = Stringa_Decodifica(Request.QueryString("act").ToString, AgroKey_EncoderDecoder)
                P_get = Qs_Piva
            End If

            If Not IsNothing(Request.QueryString("saCod")) Then
                saCod_get = Stringa_Decodifica(Request.QueryString("saCod").ToString, AgroKey_EncoderDecoder)
            End If

            If Not IsNothing(Request.QueryString("idAgenda")) Then
                idAgenda_get = Stringa_Decodifica(Request.QueryString("idAgenda").ToString, AgroKey_EncoderDecoder)
            End If

            If Not IsNothing(Request.QueryString("idMov")) Then
                idMov_get = Stringa_Decodifica(Request.QueryString("idMov").ToString, AgroKey_EncoderDecoder)

            End If

            If Not IsNothing(Request.QueryString("stabSaCod")) Then
                stabSaCod_get = Stringa_Decodifica(Request.QueryString("stabSaCod").ToString, AgroKey_EncoderDecoder)
            End If

            If Not IsNothing(Request.QueryString("stabFabbr")) Then
                stabFabbr_get = Stringa_Decodifica(Request.QueryString("stabFabbr").ToString, AgroKey_EncoderDecoder)
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
                '===== Postback
                '==========================================
                Select Case Txt_InsertCE.Value

                    Case "1" 'inseriti dei CE -> fare il refresh dei menù a tendina
                        'caricamento dei menu combo corpi estranei
                        Carica_CE()
                        'dopo il caricamento riazzero la casella
                        Txt_InsertCE.Value = ""

                End Select

                Exit Sub

            End If

            '################################################################

            'se l'utente ha il magazzino impostato nel filtro di visibilità
            '-> far vedere solo quello
            'altrimenti caricarli tutti
            'se nuovo; se info o edit carica sempre tutto
            CaricaStabilimenti(Qs_Piva)

            CaricaPrefissi()

            'inizializzo i campi
            Me.Txt_Bolla_Anno.Text = Date.Now.Year
            Me.Txt_Data_DDT.Text = Date.Today

            'controllo se sono in modifica
            If act = "info" OrElse act = "mod" Then
                Ripristina_Dati_nei_Controlil()
            End If

            'caricamento dei menu combo corpi estranei
            Carica_CE()


        Catch exc As Exception
            Messaggi.AgroMsgBox("Problemi nel caricamento della pagina: " & vbCrLf & exc.Message, Page)
        End Try

    End Sub


    '###################################################################################
    Private Sub ImgBtn_Esci_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) 
        Dim QueryString As String

        QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server)
        Response.Redirect("./GestioneMonitoraggioCE.aspx" & QueryString)

    End Sub

    '##################################################################################
    Private Sub CaricaPrefissi()
        'carico il sin
        Select Case CInt(Session("ASG_ProgressivoGIAS"))
            Case enum_CodiceGIAS_Clienti.Fruttagel

                Dim prefisso As String

                CaricaListControl.Prefisso_BolleAccettazione_FRG(Me.Cmb_Prefisso,
                                                                 False, "", "",
                                                                 Me.Cmb_Magazzino.SelectedItem.Text.ToLower)

                Dim objADD As New AgronicaCoreContabHLP.AccettazioneDaDiversi
                'Me.Txt_Sin.Text = objADD.Ricava_PrefissoBolleAccettazione_FRG(Me.Cmb_Magazzino.SelectedItem.Text)
                prefisso = objADD.Ricava_PrefissoBolleAccettazione_FRG(Me.Cmb_Magazzino.SelectedItem.Text)

                Me.Cmb_Prefisso.SelectedIndex = Me.Cmb_Prefisso.Items.IndexOf(Me.Cmb_Prefisso.Items.FindByText(prefisso))

        End Select
    End Sub

    '##################################################################################
    Private Sub Cmb_Magazzino_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Magazzino.SelectedIndexChanged

        CaricaPrefissi()

    End Sub

    '##################################################################################
    Private Sub Cmb_Prefisso_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Prefisso.SelectedIndexChanged

        CaricaDatiBolla(True, 1)

    End Sub

    '##################################################################################
    Private Sub Btn_CaricaDatiBolla_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CaricaDatiBolla.Click

        CaricaDatiBolla(True, 1)

    End Sub

    '###################################################################################
    'Modificata in data 04/10/2011: corretto bug
    'nel caso in cui vengano trovate bolle con lo stesso numero di ddt nella stessa data,
    'dopo aver registrato il modulo di carico riferito alla prima bolla,
    'non era più possibile inserire il modulo di carico della seconda bolla,
    'perché veniva fatto il controllo di esistenza sempre sulla prima
    Private Sub CaricaDatiBolla(ByVal Controlla As Boolean, ByVal Provenienza As Integer)

        ' verifico che siano stati inseriti i dati correttamente
        If VerificaCampi(False, Provenienza) = False Then
            Exit Sub
        End If

        ''modifica del 21/09/2010: il controllo va fatto anche quando si ricerca la bolla a partire dal ddt di conferimento
        ''spostato il controllo + sotto
        ''controllo se la registrazione per questa bolla è già stata inserita
        'If Provenienza = 1 Then
        '    If Controlla = True Then
        '        Dim objCE As New AgronicaCoreAnagrafeDAL.CorpiEstranei_R
        '        If objCE.EsisteRegistrazioneBolla(Qs_Piva, CStr(Txt_Sin.Text), CInt(txt_Numero_Bolla.Text), "", Txt_Bolla_Anno.Text, objParametri_Server) = True Then
        '            AgroMsgBox("E' già stata inserita la scheda Corpi Estranei riferita alla bolla: " & Me.Txt_Sin.Text & Right("00000" + Me.txt_Numero_Bolla.Text, 5), Page)
        '            Me.ImgBtnSalvaTutto.Enabled = False
        '            Exit Sub
        '        End If
        '        Me.ImgBtnSalvaTutto.Enabled = True
        '    End If
        'End If


        'inizializzo i dati di ricerca
        Dim Sin_B, Sin_D, Des_B, Des_D, Data_D As String
        Dim N_B, N_D, Anno_B As Integer
        Dim CodContatto As String
        Dim Sa_Cod As Integer = 0
        Dim Fabbricato_Cod As Integer = 0

        If Provenienza = 1 Then
            'Sin_B = CStr(Txt_Sin.Text)
            Sin_B = CStr(Me.Cmb_Prefisso.SelectedValue)

            If Not IsNumeric(Me.txt_Numero_Bolla.Text) Then
                Messaggi.AgroMsgBox("Il numero bolla deve essere numerico!", Page)
                Exit Sub
            End If
            N_B = CInt(Me.txt_Numero_Bolla.Text)

            Des_B = ""

            If Not IsNumeric(Me.Txt_Bolla_Anno.Text) Then
                Messaggi.AgroMsgBox("L'anno della bolla deve essere numerico!", Page)
                Exit Sub
            End If
            Anno_B = Me.Txt_Bolla_Anno.Text

            Sin_D = ""
            Des_D = ""
            N_D = 0
            Data_D = ""
        Else
            Sin_B = ""
            N_B = 0
            Des_B = ""
            Anno_B = 0

            Sin_D = CStr(Txt_DocNumeroSin_DDT.Text)
            Des_D = CStr(Txt_DocNumeroDes_DDT.Text)

            If Not IsNumeric(Me.Txt_DocNumero_DDT.Text) Then
                Messaggi.AgroMsgBox("Il numero DDT deve essere numerico!", Page)
                Exit Sub
            End If
            N_D = CInt(Me.Txt_DocNumero_DDT.Text)

            If Not IsDate(Me.Txt_Data_DDT.Text) Then
                Messaggi.AgroMsgBox("La data DDT deve essere impostata nl formato corretto!", Page)
                Exit Sub
            End If
            Data_D = Me.Txt_Data_DDT.Text
        End If

        If Cmb_Produttori.SelectedValue <> "0" AndAlso Cmb_Produttori.SelectedValue <> "" Then
            CodContatto = CStr(Cmb_Produttori.SelectedValue)
        Else
            CodContatto = ""
        End If

        Sa_Cod = Me.Cmb_Magazzino.SelectedValue.Split("|")(1)
        Fabbricato_Cod = Me.Cmb_Magazzino.SelectedValue.Split("|")(0)

        ' carico i dati
        Dim objAccettazioneR As New AgronicaCoreContabDAL.AccettazioneDaDiversi_R
        Dim objFF As New AgronicaCoreStampeDAL.FreshAndFood
        Dim DT As DataTable

        '27/01/2021: gestione lettura bolle di conferimento web
        If Anno_B <> 0 Then
            'ricerca x numero bolla
            Select Case Anno_B

                Case Is >= 2021
                    DT = objFF.RecuperaDatiBollaxMonitoraggioCE_NEW(Qs_Piva, Sin_B, N_B, Des_B,
                                                                            Sin_D, N_D, Des_D,
                                                                            Anno_B,
                                                                            Data_D,
                                                                            CodContatto,
                                                                            Sa_Cod,
                                                                            "",
                                                                            "",
                                                                            objParametri_Server)
                Case Else
                    DT = objAccettazioneR.RecuperaDatiBollaxMonitoraggioCE_OLD(Qs_Piva, Sin_B, N_B, Des_B,
                                                          Sin_D, N_D, Des_D,
                                                          Anno_B,
                                                          Data_D,
                                                          CodContatto,
                                                          Sa_Cod,
                                                          Fabbricato_Cod,
                                                          "",
                                                          "",
                                                          objParametri_Server)


            End Select

        Else
            'ricerca x numero ddt
            Select Case Year(Data_D)
                Case Is >= 2021
                    DT = objFF.RecuperaDatiBollaxMonitoraggioCE_NEW(Qs_Piva, Sin_B, N_B, Des_B,
                                                                            Sin_D, N_D, Des_D,
                                                                            Anno_B,
                                                                            Data_D,
                                                                            CodContatto,
                                                                            Sa_Cod,
                                                                            "",
                                                                            "",
                                                                            objParametri_Server)
                Case Else
                    DT = objAccettazioneR.RecuperaDatiBollaxMonitoraggioCE_OLD(Qs_Piva, Sin_B, N_B, Des_B,
                                                                              Sin_D, N_D, Des_D,
                                                                              Anno_B,
                                                                              Data_D,
                                                                              CodContatto,
                                                                              Sa_Cod,
                                                                              Fabbricato_Cod,
                                                                              "",
                                                                              "",
                                                                              objParametri_Server)

            End Select

        End If

        Cmb_Produttori.Items.Clear()
        Cmb_Produttori.Items.Add(New ListItem("", "0"))

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            If DT.Rows.Count = 1 Then
                'è stata trovata una sola bolla, carico i dati

                'caricamento dati bolla da num bolla o num ddt

                If Not IsDBNull(DT.Rows(0).Item("Doc_Numero_Sin_Bolla")) Then
                    'Me.Txt_Sin.Text = DT.Rows.Item(0)("Doc_Numero_Sin_Bolla")
                    Me.Cmb_Prefisso.SelectedIndex = Me.Cmb_Prefisso.Items.IndexOf(Me.Cmb_Prefisso.Items.FindByText(DT.Rows.Item(0)("Doc_Numero_Sin_Bolla")))
                End If
                If Not IsDBNull(DT.Rows(0).Item("Doc_Numero_Bolla")) Then
                    Me.txt_Numero_Bolla.Text = DT.Rows.Item(0)("Doc_Numero_Bolla")
                End If
                If Not IsDBNull(DT.Rows.Item(0)("Data_Accett_Bolla")) Then
                    Me.Txt_Bolla_Anno.Text = CStr(CDate(DT.Rows.Item(0)("Data_Accett_Bolla")).Year)
                End If

                If Not IsDBNull(DT.Rows(0).Item("Doc_Numero_Sin_Conf")) Then
                    Txt_DocNumeroSin_DDT.Text = DT.Rows.Item(0)("Doc_Numero_Sin_Conf")
                End If
                If Not IsDBNull(DT.Rows(0).Item("Doc_Numero_Conf")) Then
                    Txt_DocNumero_DDT.Text = DT.Rows.Item(0)("Doc_Numero_Conf")
                End If
                If Not IsDBNull(DT.Rows(0).Item("Doc_Numero_Des_Conf")) Then
                    Txt_DocNumeroDes_DDT.Text = DT.Rows.Item(0)("Doc_Numero_Des_Conf")
                End If
                If Not IsDBNull(DT.Rows.Item(0)("Data_Conf")) Then
                    Me.Txt_Data_DDT.Text = CDate(DT.Rows.Item(0)("Data_Conf"))
                End If

                If Not IsDBNull(DT.Rows(0).Item("Mat_Des_Raccolta")) Then
                    Txt_Varieta.Text = DT.Rows.Item(0)("Mat_Des_Raccolta")
                End If
                If Not IsDBNull(DT.Rows(0).Item("Rag_Soc_Produttore")) Then
                    Txt_Produttore.Text = DT.Rows.Item(0)("Rag_Soc_Produttore")
                End If
                If Not IsDBNull(DT.Rows(0).Item("Rag_Soc_Conferente")) Then
                    Me.Txt_Conferente.Text = DT.Rows.Item(0)("Rag_Soc_Conferente")
                End If
                If Not IsDBNull(DT.Rows(0).Item("Qta_Raccolta")) Then
                    Txt_Kg.Text = Format(CDbl(DT.Rows.Item(0)("Qta_Raccolta")), "#,###,##0")
                End If
                If Not IsDBNull(DT.Rows(0).Item("Data_Accett_Bolla")) Then
                    Txt_DataBolla.Text = DT.Rows.Item(0)("Data_Accett_Bolla")
                    Txt_DataArrivo.Text = DT.Rows.Item(0)("Data_Accett_Bolla")
                End If
                If Not IsDBNull(DT.Rows(0).Item("Punteggio")) Then
                    Txt_Punteggio.Text = DT.Rows.Item(0)("Punteggio")
                End If
                If Not IsDBNull(DT.Rows(0).Item("Calibro")) Then
                    Txt_Classifica.Text = DT.Rows.Item(0)("Calibro")
                End If

                'NO DEFAULT
                'Txt_Data_Inizio_Cottura.Text = Now.Day & "/" & Now.Month & "/" & Now.Year
                'Txt_Ora_Inizio_Cottura.Text = Now.Hour & ":" & Now.Minute

                If DT.Rows(0).Item("Veg_Cod") = enum_SpecieVegetali.Spinacio Then
                    Pannello_Pesticidi.Visible = True
                Else
                    Pannello_Pesticidi.Visible = False
                End If

            End If '1 bolla trovata

            'caricamento della cmb
            Dim i As Integer
            Dim des, cod As String

            For i = 0 To DT.Rows.Count - 1

                If Not IsDBNull(DT.Rows(i).Item("Cod_Contatto_Produttore")) Then
                    cod = DT.Rows(i).Item("Cod_Contatto_Produttore")
                    des = ""
                    If Not IsDBNull(DT.Rows(i).Item("Rag_Soc_Produttore")) Then
                        des = CStr(DT.Rows(i).Item("Rag_Soc_Produttore"))
                    End If

                    des &= " - " & cod
                    des &= " - DDT n."

                    If Not IsDBNull(DT.Rows(i).Item("Doc_Numero_Sin_Conf")) Then
                        des = des & CStr(DT.Rows(i).Item("Doc_Numero_Sin_Conf"))
                    End If

                    If Not IsDBNull(DT.Rows(i).Item("Doc_Numero_Conf")) Then
                        des = des & CStr(DT.Rows(i).Item("Doc_Numero_Conf"))
                    End If

                    If Not IsDBNull(DT.Rows(i).Item("Doc_Numero_Des_Conf")) Then
                        des = des & CStr(DT.Rows(i).Item("Doc_Numero_Des_Conf"))
                    End If

                    If Not IsDBNull(DT.Rows(i).Item("Data_Conf")) Then
                        des = des & " Del " & CStr(DT.Rows(i).Item("Data_Conf"))
                    End If

                    des &= " - " & CStr(DT.Rows(i).Item("Stabilimento"))

                    Cmb_Produttori.Items.Add(New ListItem(des, cod))

                End If

            Next 'ciclo x caricamento produttori

            If Me.Cmb_Produttori.Items.Count = 2 Then
                'riga vuota + un solo produttore
                '-> seleziono già il produttore
                Me.Cmb_Produttori.SelectedIndex = 1
            End If


            'controllo se la registrazione per questa bolla è già stata inserita
            If Controlla Then

                Salvataggio_Disabilita()

                If Provenienza = 2 AndAlso DT.Rows.Count > 1 Then
                    ' se sto cercando dal numero di ddt e sono state trovate più bolle,
                    'non faccio il controllo sulla prima trovata 
                    'lo faccio quando l'utente seleziona il produttore nel menù a tendina
                    Messaggi.AgroMsgBox("Sono state trovate più bolle riferite al DDT n. " & Sin_D & CStr(N_D) & Des_D & " del " & CStr(Data_D) & vbCrLf & _
                                "Selezionare il Produttore e la relativa bolla nel menù a tendina.", Page)

                Else
                    Dim objCE As New AgronicaCoreAnagrafeDAL.CorpiEstranei_R
                    Dim prefisso As String

                    'prefisso = CStr(Txt_Sin.Text)
                    prefisso = Me.Cmb_Prefisso.SelectedValue

                    If objCE.EsisteRegistrazioneBolla(Qs_Piva,
                                                      prefisso,
                                                      CInt(txt_Numero_Bolla.Text),
                                                      "",
                                                      Txt_Bolla_Anno.Text,
                                                      objParametri_Server) = True Then

                        Messaggi.AgroMsgBox("E' già stata inserita la scheda Corpi Estranei riferita alla bolla: " & prefisso & Right("00000" + Me.txt_Numero_Bolla.Text, 5), Page)
                        Pulisci_Tutto()
                        Exit Sub

                    End If

                    Salvataggio_Abilita()

                End If

            End If
        Else
            Messaggi.AgroMsgBox("Non è stata trovata alcuna bolla con il filtro di ricerca impostato.", Page)
        End If


    End Sub

    '###################################################################################
    Private Sub Btn_CaricaProduttori_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_CaricaProduttori.Click
        CaricaDatiBolla(True, 2)
    End Sub

    '###################################################################################
    Private Sub Cmb_Produttori_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cmb_Produttori.SelectedIndexChanged
        CaricaDatiBolla(True, 2)
    End Sub

    '##################################################################################
    Private Function CaricaStabilimenti(ByVal Qspiva As String) As Boolean

        Try

            Dim Impostazione_Valore_1 As String
            Dim Piva As String = ""
            Dim Sa_Cod As Integer = 0
            Dim Fabbricato_Cod As Integer = 0

            If act <> "info" OrElse act <> "mod" Then
                'Se sto aprendo un documento esistente, non devo leggere l'impostazione,
                'perché devo caricare sempre tutto l'elenco (e la ddl verrà bloccata)

                Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Impostazione_Valore_1 = objImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_MAGAZZINO_RIFERIMENTO,
                                                                                                 objParametri_Utenti, 1)

                Dim objADD As New AgronicaCoreContabHLP.AccettazioneDaDiversi
                objADD.Leggi_ChiaveMagazzino_Default(Piva, Sa_Cod, Fabbricato_Cod, Impostazione_Valore_1)
            End If

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

    '###################################################################################
    Private Sub Ripristina_Dati_nei_Controlil()

        ''carico i dati da agenda
        Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R
        Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R
        Dim objMovimentiDettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim DT As DataTable

        DT = objAgenda.Leggi(P_get, saCod_get, idAgenda_get, 0,
                             enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "", "",
                             objParametri_Server)

        If DT.Rows.Count = 0 Then
            Messaggi.AgroMsgBox("ERRORE!!! Registrazione non trovata!", Page)
            Exit Sub
        End If

        DT = objMovimenti.Leggi(P_get, saCod_get, idAgenda_get, idMov_get,
                                0, "",
                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                "", "",
                                objParametri_Server)
        If DT.Rows.Count = 0 Then
            Messaggi.AgroMsgBox("ERRORE!!! Registrazione non trovata!", Page)
            Exit Sub
        End If

        If Not IsDBNull(DT.Rows(0).Item("Data_Movimento")) Then
            Txt_DataArrivo.Text = DT.Rows.Item(0)("Data_Movimento")
        End If

        If stabFabbr_get = 0 AndAlso stabSaCod_get <> 0 Then
            ' Cerca il primo magazzino con la parte finale uguale a stabSaCod_get
            For i As Integer = 0 To Me.Cmb_Magazzino.Items.Count - 1
                Dim valueParts = Me.Cmb_Magazzino.Items(i).Value.Split("|")
                If valueParts.Length = 2 AndAlso IsNumeric(valueParts(1)) AndAlso CInt(valueParts(1)) = stabSaCod_get Then
                    Me.Cmb_Magazzino.SelectedIndex = i
                    Cmb_Magazzino_SelectedIndexChanged(Me.Cmb_Magazzino, EventArgs.Empty)
                    Exit For
                End If
            Next
        Else
            Dim chiaveMagazzino As String = CStr(stabFabbr_get & "|" & stabSaCod_get)
            Me.Cmb_Magazzino.SelectedIndex = Me.Cmb_Magazzino.Items.IndexOf(Me.Cmb_Magazzino.Items.FindByValue(chiaveMagazzino))
            Cmb_Magazzino_SelectedIndexChanged(Me.Cmb_Magazzino, EventArgs.Empty)
        End If


        If Not IsDBNull(DT.Rows(0).Item("Doc_Numero_Sin")) Then
            'Txt_Sin.Text = DT.Rows.Item(0)("Doc_Numero_Sin")
            Me.Cmb_Prefisso.SelectedIndex = Me.Cmb_Prefisso.Items.IndexOf(Me.Cmb_Prefisso.Items.FindByText(DT.Rows.Item(0)("Doc_Numero_Sin")))
        End If

        If Not IsDBNull(DT.Rows(0).Item("Doc_Numero")) Then
            txt_Numero_Bolla.Text = DT.Rows.Item(0)("Doc_Numero")
        End If
        If Not IsDBNull(DT.Rows(0).Item("Extra_Int")) Then
            Txt_Bolla_Anno.Text = DT.Rows.Item(0)("Extra_Int")
        End If

        If Not IsDBNull(DT.Rows(0).Item("Colli")) Then
            Txt_Carico_N.Text = DT.Rows.Item(0)("Colli")
        End If
        If Not IsDBNull(DT.Rows(0).Item("Mov_Desc")) Then
            Txt_Note.Text = DT.Rows.Item(0)("Mov_Desc")
        End If
        If Not IsDBNull(DT.Rows(0).Item("Extra_Date")) Then
            Txt_Data_Inizio_Cottura.Text = DT.Rows.Item(0)("Extra_Date")
        End If
        If Not IsDBNull(DT.Rows(0).Item("Ora")) Then
            Txt_Ora_Inizio_Cottura.Text = CDate(DT.Rows.Item(0)("Ora")).ToShortTimeString
        End If
        If Not IsDBNull(DT.Rows(0).Item("Extra_Str")) Then
            Txt_Livello_Qualitativo.Text = DT.Rows.Item(0)("Extra_Str")
        End If
        If Not IsDBNull(DT.Rows(0).Item("Natura_Beni")) Then
            Txt_Confezione_Marchio_1.Text = DT.Rows.Item(0)("Natura_Beni")
        End If

        If Not IsDBNull(DT.Rows(0).Item("Aspetto")) Then
            Txt_Confezione_Marchio_2.Text = DT.Rows.Item(0)("Aspetto")
        End If

        If Not IsDBNull(DT.Rows(0).Item("Tipo_Sconto")) AndAlso DT.Rows.Item(0)("Tipo_Sconto") <> 0 Then
            Rbl_Pesticidi.SelectedValue = DT.Rows.Item(0)("Tipo_Sconto")
        End If

        ''carico i dati della bolla
        CaricaDatiBolla(False, 1)


        '' proseguo con il caricamento dei dettagli dalla tabella movimenti_dettagli
        DT = objMovimentiDettagli.Leggi(P_get, saCod_get, idAgenda_get, idMov_get,
                                        0, 0, 0, 0, "", 0, 0, 0, 0, 0, 0,
                                        enumSelezioneVariabile.Selezione_TabellaCompleta,
                                        "", "",
                                        objParametri_Server)

        Dim qta As Integer
        Dim prodCod As Integer
        Dim CE As String
        Dim Pericolosita As Integer
        Dim objCE As New AgronicaCoreAnagrafeDAL.CorpiEstranei_R

        For i As Integer = 0 To DT.Rows.Count - 1
            Pericolosita = DT.Rows(i).Item("Cod_Progetto")
            qta = DT.Rows(i).Item("QTA")
            prodCod = DT.Rows(i).Item("Pro_Cod")
            CE = objCE.LeggiDescrizioneCE(prodCod, objParametri_Server)
            If DT.Rows(i).Item("Fase_Cod") = 1 Then
                'Linea Produzione 1
                Select Case DT.Rows(i).Item("Mat_Cod")
                    Case "1"
                        'controllo se è il codice fittizio 
                        If qta <> 0 Then
                            Aggiungi_Item(qta, prodCod, CE, Pericolosita, List_AE_1)
                        End If
                    Case "2"
                        Aggiungi_Item(qta, prodCod, CE, Pericolosita, Lista_CO_1)
                    Case "3"
                        Aggiungi_Item(qta, prodCod, CE, Pericolosita, Lista_CM_1)
                End Select
            Else
                'Linea Produzione 2
                Select Case DT.Rows(i).Item("Mat_Cod")
                    Case "1"
                        'controllo se è il codice fittizio 
                        If qta <> 0 Then
                            Aggiungi_Item(qta, prodCod, CE, Pericolosita, List_AE_2)
                        End If
                    Case "2"
                        Aggiungi_Item(qta, prodCod, CE, Pericolosita, Lista_CO_2)
                    Case "3"
                        Aggiungi_Item(qta, prodCod, CE, Pericolosita, Lista_CM_2)
                End Select
            End If
        Next

        If act = "mod" Then
            Cmb_Magazzino.Enabled = False
            RBL_Salva.Enabled = False
            'imposto su salva e esci
            RBL_Salva.SelectedValue = 0
        End If

        If act = "info" Then
            Cmb_Magazzino.Enabled = False
            RBL_Salva.Visible = False
            RBL_Salva.Enabled = False
            ImgBtnSalvaTutto.Visible = False
            ImgBtnSalvaTutto.Enabled = False
        End If


    End Sub


    '##################################################################################
    Private Function Carica_CE() As Boolean

        Try

            Dim DT As New DataTable
            Dim objCorpoEstraneo As New AgronicaCoreAnagrafeDAL.CorpiEstranei_R
            DT = objCorpoEstraneo.Leggi(0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                        "", "", objParametri_Server)


            If DT.Rows.Count = 0 Then
                Messaggi.AgroMsgBox("Non sono stati trovati corpi estranei!", Page)
            End If
            'linea 1 
            CaricaListControl.CorpiEstranei(CType(Cmb_TipoCE_AE_1, ListControl),
                                            True, "", "0",
                                            DT, "", "", objParametri_Server)
            CaricaListControl.CorpiEstranei(CType(Cmb_TipoCE_CO_1, ListControl),
                                            True, "", "0",
                                            DT, "", "", objParametri_Server)
            CaricaListControl.CorpiEstranei(CType(Cmb_TipoCE_CM_1, ListControl),
                                            True, "", "0",
                                            DT, "", "", objParametri_Server)
            'linea 2 
            CaricaListControl.CorpiEstranei(CType(Cmb_TipoCE_AE_2, ListControl),
                                            True, "", "0",
                                            DT, "", "", objParametri_Server)
            CaricaListControl.CorpiEstranei(CType(Cmb_TipoCE_CO_2, ListControl),
                                            True, "", "0",
                                            DT, "", "", objParametri_Server)
            CaricaListControl.CorpiEstranei(CType(Cmb_TipoCE_CM_2, ListControl),
                                            True, "", "0",
                                            DT, "", "", objParametri_Server)
            Return True

        Catch ex As Exception
            Messaggi.AgroMsgBox("Errore Caricamento Corpi Estranei: " & ex.Message, Page)
            Return False
        End Try

    End Function


    '########################################################################################
    Private Function VerificaCampi(ByVal Tutto As Boolean, ByVal Provenienza As Integer) As Boolean
        'controllo se le info nei vari campi sono state inserite
        If Provenienza = 1 Then
            'If Me.Txt_Sin.Text = "" Then
            If Me.Cmb_Prefisso.SelectedValue = "" Then
                Messaggi.AgroMsgBox("E' necessario selezionare il prefisso della bolla!", Page)
                Return False
            End If

            If Me.txt_Numero_Bolla.Text = "" Then
                Messaggi.AgroMsgBox("E' necessario inserire il numero della bolla!", Page)
                Return False
            End If

            If Me.Txt_Bolla_Anno.Text = "" Then
                Messaggi.AgroMsgBox("E' necessario inserire l'anno della bolla!", Page)
                Return False
            End If
        Else
            If Me.Txt_DocNumero_DDT.Text = "" Then
                Messaggi.AgroMsgBox("E' necessario inserire il numero del DDT!", Page)
                Return False
            End If

            If Me.Txt_Data_DDT.Text = "" Then
                Messaggi.AgroMsgBox("E' necessario inserire la data del DDT!", Page)
                Return False
            End If
        End If

        If Tutto Then

            If Me.Txt_DataArrivo.Text = "" Then
                Messaggi.AgroMsgBox("E' necessario inserire la data di arrivo!", Page)
                Return False
            End If

            If Me.Txt_Carico_N.Text = "" Then
                Messaggi.AgroMsgBox("E' necessario inserire il numero del carico!", Page)
                Return False
            End If

            If Me.Txt_Ora_Inizio_Cottura.Text = "" Then
                Messaggi.AgroMsgBox("E' necessario inserire l'ora di inizio cottura!", Page)
                Return False
            End If
            If Me.Txt_Data_Inizio_Cottura.Text = "" Then
                Messaggi.AgroMsgBox("E' necessario inserire la data di inizio cottura!", Page)
                Return False
            End If

            If Txt_Livello_Qualitativo.Text = "" Then
                Messaggi.AgroMsgBox("E' necessario inserire il livello qualitativo!", Page)
                Return False
            End If

            If Txt_Confezione_Marchio_1.Text = "" Then
                Messaggi.AgroMsgBox("E' necessario inserire la confezione marchio!", Page)
                Return False
            End If
        End If

        Return True

    End Function

    '########################################################################################
    Private Sub ImgBtnSalvaTutto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnSalvaTutto.Click

        Dim Ris As Boolean
        Ris = SalvaTutto()

        If Ris = True Then
            If Me.RBL_Salva.SelectedItem.Value = "0" Then
                'faccio il redirect
                Dim QueryString As String
                QueryString = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server)
                Response.Redirect("./GestioneMonitoraggioCE.aspx" & QueryString)
            Else
                'messaggio ok e pulisco
                Messaggi.AgroMsgBox("Salvataggio avvenuto con successo!", Page)
                'pulisci
                Dim app_sin As String
                Dim app_n As String
                Dim app_des As String
                Dim app_anno As Int32

                'app_sin = Me.Txt_Sin.Text
                app_sin = Me.Cmb_Prefisso.SelectedValue
                app_n = Me.txt_Numero_Bolla.Text
                app_des = ""
                app_anno = Me.Txt_Bolla_Anno.Text
                Pulisci_Tutto()
                '                PrecaricaSucessiva(app_sin, app_n, app_des, app_anno)
            End If

        End If

    End Sub

    '########################################################################################
    Private Function SalvaTutto() As Boolean

        If VerificaCampi(True, 1) = False Then
            Return False
        End If

        Dim TopCode As Integer
        Dim BaseCode As Integer

        UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))

        'genero il DT per salvare i movimenti dettagli
        Dim objAgroXml As New AgronicaCoreXML.XML_Contab
        Dim DT As DataTable = objAgroXml.DtForXml_Genera_MovimentiDettagli()

        'aggiungo tutti gli elementi inseriti in AE
        Dim i As Integer
        Dim QTA As Integer
        Dim Prod_Cod As Integer
        Dim Indice_Pericolosita As Integer
        Dim Fase_Cod As Integer
        Dim AlmenoUnCorpoEstraneo As Boolean = False


        'LINEA PRODUZIONE 1'LINEA PRODUZIONE 1'LINEA PRODUZIONE 1'LINEA PRODUZIONE 1'LINEA PRODUZIONE 1
        'Aereoseparatori
        For i = 0 To List_AE_1.Items.Count - 1

            AlmenoUnCorpoEstraneo = True
            Fase_Cod = 1
            QTA = CInt(List_AE_1.Items(i).Text.Split("=")(1))
            ' scompongo 

            Prod_Cod = List_AE_1.Items(i).Value.Split("|")(0)
            Indice_Pericolosita = List_AE_1.Items(i).Value.Split("|")(1)

            ''aggiungo la riga
            objAgroXml.DtForXml_InserisciRiga_MovimentiDettagli(DT,
                                                                AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura,
                                                                Qs_Piva,
                                                                , ,
                                                                , , ,
                                                                CORPI_ESTRANEI,
                                                                Prod_Cod,
                                                                enum_Rilevamento_Corpi_Estranei.Aereoseparatore,
                                                                Indice_Pericolosita,
                                                                Fase_Cod, , ,
                                                                CInt(enum_UnitaMisura.Numero),
                                                                ,
                                                                QTA,
                                                                , , , , , , , , ,
                                                                Me.Txt_Bolla_Anno.Text,
                                                                , , , , , , , ,
                                                                Me.Txt_DataArrivo.Text,
                                                                AGRODATAFINE,
                                                                , , , , , , ,
                                                                BaseCode, TopCode,
                                                                , , , Nothing)

        Next

        'Cernitrice Ottica
        For i = 0 To Lista_CO_1.Items.Count - 1
            AlmenoUnCorpoEstraneo = True
            Fase_Cod = 1
            QTA = CInt(Lista_CO_1.Items(i).Text.Split("=")(1))
            ' scompongo 
            Prod_Cod = Lista_CO_1.Items(i).Value.Split("|")(0)
            Indice_Pericolosita = Lista_CO_1.Items(i).Value.Split("|")(1)
            ''aggiungo la riga
            objAgroXml.DtForXml_InserisciRiga_MovimentiDettagli( _
                                            DT, _
                                            AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, _
                                            Qs_Piva, _
                                            , , _
                                            , , , _
                                            CORPI_ESTRANEI, _
                                            Prod_Cod, _
                                            enum_Rilevamento_Corpi_Estranei.Cernitrice_Ottica, _
                                            Indice_Pericolosita, _
                                            Fase_Cod, , , _
                                            CInt(enum_UnitaMisura.Numero), _
                                            , _
                                            QTA, _
                                            , , , , , , , , , _
                                            Me.Txt_Bolla_Anno.Text, _
                                            , , , , , , , , _
                                            Me.Txt_DataArrivo.Text, _
                                            AGRODATAFINE, _
                                            , , , , , , , _
                                            BaseCode, TopCode, _
                                            , , )

        Next


        'Cernita Manuale
        For i = 0 To Lista_CM_1.Items.Count - 1
            AlmenoUnCorpoEstraneo = True
            Fase_Cod = 1
            QTA = CInt(Lista_CM_1.Items(i).Text.Split("=")(1))
            ' scompongo 
            Prod_Cod = Lista_CM_1.Items(i).Value.Split("|")(0)
            Indice_Pericolosita = Lista_CM_1.Items(i).Value.Split("|")(1)
            ''aggiungo la riga
            objAgroXml.DtForXml_InserisciRiga_MovimentiDettagli( _
                                            DT, _
                                            AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, _
                                            Qs_Piva, _
                                            , , _
                                             , , , _
                                             CORPI_ESTRANEI, _
                                             Prod_Cod, _
                                             enum_Rilevamento_Corpi_Estranei.Cernita_Manuale, _
                                             Indice_Pericolosita, _
                                             Fase_Cod, , , _
                                             CInt(enum_UnitaMisura.Numero), _
                                             , _
                                             QTA, _
                                             , , , , , , , , , _
                                             Me.Txt_Bolla_Anno.Text, _
                                             , , , , , , , , _
                                             Me.Txt_DataArrivo.Text, _
                                             AGRODATAFINE, _
                                             , , , , , , , _
                                             BaseCode, TopCode, _
                                             , , )
        Next




        'LINEA PRODUZIONE 2'LINEA PRODUZIONE 2'LINEA PRODUZIONE 2'LINEA PRODUZIONE 2'LINEA PRODUZIONE 2
        'Aereoseparatori
        For i = 0 To List_AE_2.Items.Count - 1

            AlmenoUnCorpoEstraneo = True
            Fase_Cod = 2
            QTA = CInt(List_AE_2.Items(i).Text.Split("=")(1))
            ' scompongo 

            Prod_Cod = List_AE_2.Items(i).Value.Split("|")(0)
            Indice_Pericolosita = List_AE_2.Items(i).Value.Split("|")(1)

            ''aggiungo la riga
            objAgroXml.DtForXml_InserisciRiga_MovimentiDettagli( _
                                            DT, _
                                            AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, _
                                            Qs_Piva, _
                                            , , _
                                            , , , _
                                            CORPI_ESTRANEI, _
                                            Prod_Cod, _
                                            enum_Rilevamento_Corpi_Estranei.Aereoseparatore, _
                                            Indice_Pericolosita, _
                                            Fase_Cod, , , _
                                            CInt(enum_UnitaMisura.Numero), _
                                            , _
                                            QTA, _
                                            , , , , , , , , , _
                                            Me.Txt_Bolla_Anno.Text, _
                                            , , , , , , , , _
                                            Me.Txt_DataArrivo.Text, _
                                            AGRODATAFINE, _
                                            , , , , , , , _
                                            BaseCode, TopCode, _
                                            , , )

        Next

        'Cernitrice Ottica
        For i = 0 To Lista_CO_2.Items.Count - 1
            AlmenoUnCorpoEstraneo = True
            Fase_Cod = 2
            QTA = CInt(Lista_CO_2.Items(i).Text.Split("=")(1))
            ' scompongo 
            Prod_Cod = Lista_CO_2.Items(i).Value.Split("|")(0)
            Indice_Pericolosita = Lista_CO_2.Items(i).Value.Split("|")(1)
            ''aggiungo la riga
            objAgroXml.DtForXml_InserisciRiga_MovimentiDettagli( _
                                            DT, _
                                            AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, _
                                            Qs_Piva, _
                                            , , _
                                            , , , _
                                            CORPI_ESTRANEI, _
                                            Prod_Cod, _
                                            enum_Rilevamento_Corpi_Estranei.Cernitrice_Ottica, _
                                            Indice_Pericolosita, _
                                            Fase_Cod, , , _
                                            CInt(enum_UnitaMisura.Numero), _
                                            , _
                                            QTA, _
                                            , , , , , , , , , _
                                            Me.Txt_Bolla_Anno.Text, _
                                            , , , , , , , , _
                                            Me.Txt_DataArrivo.Text, _
                                            AGRODATAFINE, _
                                            , , , , , , , _
                                            BaseCode, TopCode, _
                                            , , )

        Next


        'Cernita Manuale
        For i = 0 To Lista_CM_2.Items.Count - 1
            AlmenoUnCorpoEstraneo = True
            Fase_Cod = 2
            QTA = CInt(Lista_CM_2.Items(i).Text.Split("=")(1))
            ' scompongo 
            Prod_Cod = Lista_CM_2.Items(i).Value.Split("|")(0)
            Indice_Pericolosita = Lista_CM_2.Items(i).Value.Split("|")(1)
            ''aggiungo la riga
            objAgroXml.DtForXml_InserisciRiga_MovimentiDettagli( _
                                            DT, _
                                            AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, _
                                            Qs_Piva, _
                                            , , _
                                            , , , _
                                            CORPI_ESTRANEI, _
                                            Prod_Cod, _
                                            enum_Rilevamento_Corpi_Estranei.Cernita_Manuale, _
                                            Indice_Pericolosita, _
                                            Fase_Cod, , , _
                                            CInt(enum_UnitaMisura.Numero), _
                                            , _
                                            QTA, _
                                            , , , , , , , , , _
                                            Me.Txt_Bolla_Anno.Text, _
                                            , , , , , , , , _
                                            Me.Txt_DataArrivo.Text, _
                                            AGRODATAFINE, _
                                            , , , , , , , _
                                            BaseCode, TopCode, _
                                            , , )
        Next


        'inserisco un elemento fittizio per registrare la bolla (lo inserisco nell'aereoseparatore)
        If Not AlmenoUnCorpoEstraneo Then
            QTA = 0
            ' scompongo 
            Prod_Cod = 0
            Indice_Pericolosita = 0
            ''aggiungo la riga
            objAgroXml.DtForXml_InserisciRiga_MovimentiDettagli( _
                                            DT, _
                                            AgronicaCoreXML.XML_Contab.enum_TipoOperazioneDB.Scrittura, _
                                            Qs_Piva, _
                                            , , _
                                            , , , _
                                            CORPI_ESTRANEI, _
                                            Prod_Cod, _
                                            enum_Rilevamento_Corpi_Estranei.Aereoseparatore, _
                                            Indice_Pericolosita, _
                                            , , , _
                                            CInt(enum_UnitaMisura.Numero), _
                                            , _
                                            QTA, _
                                            , , , , , , , , , _
                                            Me.Txt_Bolla_Anno.Text, _
                                            , , , , , , , , _
                                            Me.Txt_DataArrivo.Text, _
                                            AGRODATAFINE, _
                                            , , , , , , , _
                                            BaseCode, TopCode, _
                                            , , )

        End If

        Dim StrDummy As String

        Try

            Dim Errore As Boolean = False
            Dim Str_XML_Inserisci As String
            Dim Str_XML_Canc As String
            Dim Xml As XmlDocument
            Dim objAgenda As New AgronicaCoreContabBIZ.Agenda_W
            Dim boolDummy As Boolean
            Dim Num_Carico As Integer
            Dim Ora_Cottura As Date
            Dim OUTPUT_ID_Agenda As Integer = 0

            Dim Pesticidi As Integer
            If Me.Pannello_Pesticidi.Visible = True Then
                Pesticidi = Rbl_Pesticidi.SelectedValue
            Else
                Pesticidi = 0
            End If

            If IsNumeric(Me.Txt_Carico_N.Text) Then
                Num_Carico = CInt(Me.Txt_Carico_N.Text)
            Else
                Messaggi.AgroMsgBox("Il numero del carico deve essere un valore numerico!", Page)
                Exit Function
            End If

            If IsDate(Me.Txt_Ora_Inizio_Cottura.Text) Then
                Ora_Cottura = CDate(Me.Txt_Ora_Inizio_Cottura.Text)
            Else
                Messaggi.AgroMsgBox("L'ora di inizio cottura specificata non è corretta!", Page)
                Exit Function
            End If

            'generazione xml di alto livello
            Str_XML_Inserisci = objAgroXml.MacroXML_Corpi_Estranei(Errore, _
                                                        Xml, _
                                                        Qs_Piva, _
                                                        Me.Cmb_Prefisso.SelectedValue, _
                                                        Me.txt_Numero_Bolla.Text, _
                                                        "", _
                                                        Me.Txt_Bolla_Anno.Text, _
                                                        Me.Txt_DataArrivo.Text, _
                                                        Num_Carico, _
                                                        Me.Txt_Data_Inizio_Cottura.Text, _
                                                        Ora_Cottura, _
                                                        Me.Txt_Livello_Qualitativo.Text, _
                                                        Me.Txt_Confezione_Marchio_1.Text, _
                                                        Me.Txt_Confezione_Marchio_2.Text, _
                                                        Pesticidi, _
                                                        Me.Txt_Note.Text, _
                                                        DT, _
                                                        BaseCode, TopCode, _
                                                        objParametri_Server)


            ' generato l xml posso procedere con il salvataggio


            '------------------------------------------------
            '----- apro connessione e transazione
            '------------------------------------------------
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            boolDummy = objAgenda.Agenda_Scrivi(Str_XML_Inserisci, _
                                                OUTPUT_ID_Agenda, _
                                                0, _
                                                Session("ASG_IdServizio"), _
                                                0, _
                                                "", _
                                                objParametri_Server)

            If Not boolDummy Then
                Throw New Exception("La scrittura dei dati non è andata a buon fine.")
            End If

            'controllo se sono in modifica ed effettuo la cancellazione
            If act = "mod" Then

                Dim objAgendaR As New AgronicaCoreContabBIZ.Agenda_R
                Str_XML_Canc = objAgendaR.Agenda_Leggi(P_get, saCod_get, idAgenda_get, 0, True, objParametri_Server)

                boolDummy = objAgenda.Agenda_Scrivi(Str_XML_Canc, _
                                                    OUTPUT_ID_Agenda, _
                                                    0, _
                                                    Session("ASG_IdServizio"), _
                                                    0, "", _
                                                    objParametri_Server)

                If Not boolDummy Then
                    Throw New Exception("Errore durante la modifica dell'operazione.")
                End If

            End If

            'chiudo la transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            'chiudo la connessione
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)


        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------
            'chiudo la transazione con il rollback
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            'chiudo la connessione
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            'Messaggio di errore
            StrDummy = exc.Message.ToString()

            'FACCIO APPARIRE UN ALERT......
            Messaggi.AgroMsgBox("Si è verificato un errore durante la fase di salvataggio: " & vbCrLf & StrDummy, Page)
            Return False
            '------------------------------------------------

        End Try

        Return True


    End Function

    '########################################################################################
    'Private Sub PrecaricaSucessiva(ByVal sin As String, ByVal n As Int32, ByVal des As String, ByVal anno As Int32)
    '    'controllo se esiste 

    '    n = n + 1
    '    Dim objAccettazioneR As New AgronicaCoreContabDAL.AccettazioneDaDiversi_R
    '    Dim DT As DataTable
    '    DT = objAccettazioneR.RecuperaDatiBollaxMonitoraggioCE(Qs_Piva, sin, n, des, _
    '                                                            "", 0, "", _
    '                                                            anno, _
    '                                                            0, _
    '                                                            enumSelezioneVariabile.Selezione_JoinDescrizioni, _
    '                                                            "", _
    '                                                            "", _
    '                                                            objParametri_Server)

    '    If DT.Rows.Count > 0 Then
    '        Me.txt_Numero_Bolla.Text = n
    '        Me.Txt_Sin.Text = sin
    '        Me.Txt_Bolla_Anno.Text = anno
    '        CaricaDatiBolla(True)
    '    End If

    'End Sub

    '########################################################################################
    Private Sub Salvataggio_Abilita()
        Me.ImgBtnSalvaTutto.Enabled = True
        Me.ImgBtnSalvaTutto.Visible = True
    End Sub

    '########################################################################################
    Private Sub Salvataggio_Disabilita()
        Me.ImgBtnSalvaTutto.Enabled = False
        Me.ImgBtnSalvaTutto.Visible = False
    End Sub

    '########################################################################################
    Private Sub Pulisci_Tutto()

        Me.Cmb_Produttori.Items.Clear()

        'Me.Txt_Sin.Text = ""
        'Me.Txt_Bolla_Anno.Text = ""
        Me.Txt_DataBolla.Text = ""
        Me.txt_Numero_Bolla.Text = ""
        'Me.Cmb_Prefisso.SelectedIndex = 0

        'Me.Txt_Data_DDT.Text = ""
        Me.Txt_DocNumero_DDT.Text = ""
        Me.Txt_DocNumeroDes_DDT.Text = ""
        Me.Txt_DocNumeroSin_DDT.Text = ""

        Me.Txt_Conferente.Text = ""

        Me.Txt_Carico_N.Text = ""
        Me.Txt_Classifica.Text = ""
        Me.Txt_Data_Inizio_Cottura.Text = ""
        Me.Txt_DataArrivo.Text = ""
        Me.Txt_Kg.Text = ""
        Me.Txt_Livello_Qualitativo.Text = ""
        Me.Txt_N_AE_1.Text = ""
        Me.Txt_N_CM_1.Text = ""
        Me.Txt_N_CO_1.Text = ""
        Me.Txt_N_AE_2.Text = ""
        Me.Txt_N_CM_2.Text = ""
        Me.Txt_N_CO_2.Text = ""
        Me.Txt_Note.Text = ""
        Me.Txt_Ora_Inizio_Cottura.Text = ""
        Me.Txt_Produttore.Text = ""
        Me.Txt_Punteggio.Text = ""
        Me.Txt_Varieta.Text = ""
        Me.Txt_Confezione_Marchio_1.Text = ""
        Me.Txt_Confezione_Marchio_2.Text = ""
        Me.Rbl_Pesticidi.SelectedValue = 3
        Me.List_AE_1.Items.Clear()
        Me.Lista_CM_1.Items.Clear()
        Me.Lista_CO_1.Items.Clear()
        Me.List_AE_2.Items.Clear()
        Me.Lista_CM_2.Items.Clear()
        Me.Lista_CO_2.Items.Clear()

    End Sub


    '########################################################################################
    Private Function VerificaQta(ByVal Txt As System.Web.UI.WebControls.TextBox, _
                                ByVal Cmb As System.Web.UI.WebControls.DropDownList _
                                ) As Boolean

        If Cmb.SelectedIndex < 1 Then
            Messaggi.AgroMsgBox("E' necessario selezionare un Corpo Estraneo!", Page)
            Return False
        End If

        If Txt.Text = "" Then
            Messaggi.AgroMsgBox("Non e' ammesso un valore nullo per la quantità!", Page)
            Return False
        Else
            If Not IsNumeric(Txt.Text) Then
                Messaggi.AgroMsgBox("E' necessario inserire una quantità numerica!", Page)
                Return False
            End If
        End If

        Return True

    End Function


    '########################################################################################
    Private Sub Aggiungi_Item(ByVal QTA As Integer, _
                                ByVal Codice As Integer, _
                                ByVal Descrizione As String, _
                                ByVal Indice_Pericolosita As Integer, _
                                ByRef List As System.Web.UI.WebControls.ListBox _
                                )
        Dim Testo2 As String
        Dim Valore As String
        Testo2 = Descrizione & " = " & QTA
        Valore = Codice & "|" & Indice_Pericolosita
        List.Items.Add(New ListItem(Testo2, Valore))

    End Sub

    '########################################################################################
    Private Sub Aggiungi_Item(ByVal Cmb As System.Web.UI.WebControls.DropDownList, _
                                ByVal Txt As System.Web.UI.WebControls.TextBox, _
                                ByRef List As System.Web.UI.WebControls.ListBox, _
                                ByVal Indice_Pericolosita As Integer _
                                )

        If Cmb.SelectedIndex < 1 Then
            Throw New Exception("Selezionare un codice!")
            Messaggi.AgroMsgBox("Selezionare un codice!", Page)
            Exit Sub
        End If

        If Txt.Text = "" Then
            Throw New Exception("Non e' ammesso un valore nullo!")
            Messaggi.AgroMsgBox("Non e' ammesso un valore nullo!", Page)
            Exit Sub
        End If


        Dim Indice As Integer
        Dim Testo As String
        Dim Testo2 As String
        Dim Valore As String
        'Verifico che non esista gia' l'elemento
        For Indice = 0 To List.Items.Count - 1

            Testo = List.Items(Indice).Value.Split("|")(0)

            If Cmb.SelectedItem.Value = Testo Then
                Messaggi.AgroMsgBox("Il Corpo estraneo è già stato Inserito!", Page)
                Exit Sub
            End If
        Next

        'Inserisco l'elemento nella Listbox
        Testo = Cmb.SelectedItem.Text
        Testo2 = Testo & " = " & Txt.Text

        Valore = Cmb.SelectedItem.Value & "|" & Indice_Pericolosita

        List.Items.Add(New ListItem(Testo2, Valore))

        Cmb.SelectedIndex = 0
        Txt.Text = ""

    End Sub

    '########################################################################################
    Private Sub Delete_Item(ByRef List As System.Web.UI.WebControls.ListBox)

        Dim Indice As Integer

        If List.SelectedIndex > -1 Then

            'Recupero l'indice dell'elemento selezionato
            Indice = List.SelectedIndex
            'Elimino l'elemento selezionato
            List.Items.RemoveAt(Indice)
        End If

    End Sub


    '########################################################################################
    Private Function ControllaLimitePericolosita(ByVal CodiceCE As Int32, ByVal QTA_AE As Int32, ByVal QTA_CO As Int32, ByVal QTA_CM As Int32) As Integer

        Dim objCE As New AgronicaCoreAnagrafeDAL.CorpiEstranei_R

        If objCE.SuperatoLimitePericolosita(CodiceCE, QTA_AE, QTA_CO, QTA_CM, objParametri_Server) = True Then
            Messaggi.AgroMsgBox("LIMITE DI PERICOLOSITA' SUPERATO!", Page)
            Return 1
        End If

        Return 0

    End Function


    '####################################################################################
    Private Sub ImgBtn_GestioneCE_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_GestioneCE.Click
        ' Funzionalità nascosta
        Gestione_CE()

    End Sub


    '####################################################################################
    Private Sub Gestione_CE()

        Dim QueryString As String = "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server)

        Dim StrWindowOpen As String = Page_ModalDialog_Script("GestioneCE.aspx", QueryString, "",
                                                              700, 1000, 0, 0,
                                                              NomeForm:="FORM1")

        ScriptManager.RegisterClientScriptBlock(ImgBtn_GestioneCE, ImgBtn_GestioneCE.GetType(),
                                                String.Format("jQuery_{0}", "openmodal"), StrWindowOpen, False)

    End Sub


    '###################################################################################
    Private Sub ImgBtn_Del_AE_1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Del_AE_1.Click
        Delete_Item(List_AE_1)
    End Sub

    '###################################################################################
    Private Sub ImgBtn_Ins_AE_1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Ins_AE_1.Click
        Dim indice_Pericolosita As Integer
        If VerificaQta(Txt_N_AE_1, Cmb_TipoCE_AE_1) Then
            indice_Pericolosita = ControllaLimitePericolosita(Cmb_TipoCE_AE_1.SelectedItem.Value, Me.Txt_N_AE_1.Text, 0, 0)
            Aggiungi_Item(Me.Cmb_TipoCE_AE_1, Me.Txt_N_AE_1, Me.List_AE_1, indice_Pericolosita)
        End If
    End Sub

    '###################################################################################
    Private Sub ImgBtn_Del_AE_2_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Del_AE_2.Click
        Delete_Item(List_AE_2)
    End Sub

    '###################################################################################
    Private Sub ImgBtn_Ins_AE_2_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Ins_AE_2.Click
        Dim indice_Pericolosita As Integer
        If VerificaQta(Txt_N_AE_2, Cmb_TipoCE_AE_2) Then
            indice_Pericolosita = ControllaLimitePericolosita(Cmb_TipoCE_AE_2.SelectedItem.Value, Me.Txt_N_AE_2.Text, 0, 0)
            Aggiungi_Item(Me.Cmb_TipoCE_AE_2, Me.Txt_N_AE_2, Me.List_AE_2, indice_Pericolosita)
        End If
    End Sub

    '###################################################################################
    Private Sub ImgBtn_Del_CO_1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Del_CO_1.Click
        Delete_Item(Lista_CO_1)
    End Sub

    '###################################################################################
    Private Sub ImgBtn_Ins_CO_1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Ins_CO_1.Click
        Dim indice_Pericolosita As Integer
        If VerificaQta(Txt_N_CO_1, Cmb_TipoCE_CO_1) Then
            indice_Pericolosita = ControllaLimitePericolosita(Cmb_TipoCE_CO_1.SelectedItem.Value, 0, Me.Txt_N_CO_1.Text, 0)
            Aggiungi_Item(Me.Cmb_TipoCE_CO_1, Me.Txt_N_CO_1, Me.Lista_CO_1, indice_Pericolosita)
        End If
    End Sub

    '###################################################################################
    Private Sub ImgBtn_Ins_CO_2_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Ins_CO_2.Click
        Dim indice_Pericolosita As Integer
        If VerificaQta(Txt_N_CO_2, Cmb_TipoCE_CO_2) Then
            indice_Pericolosita = ControllaLimitePericolosita(Cmb_TipoCE_CO_2.SelectedItem.Value, 0, Me.Txt_N_CO_2.Text, 0)
            Aggiungi_Item(Me.Cmb_TipoCE_CO_2, Me.Txt_N_CO_2, Me.Lista_CO_2, indice_Pericolosita)
        End If
    End Sub

    '###################################################################################
    Private Sub ImgBtn_Del_CO_2_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Del_CO_2.Click
        Delete_Item(Lista_CO_2)
    End Sub

    '###################################################################################
    Private Sub ImgBtn_Del_CM_1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Del_CM_1.Click
        Delete_Item(Lista_CM_1)
    End Sub

    '###################################################################################
    Private Sub ImgBtn_Ins_CM_1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Ins_CM_1.Click
        Dim indice_Pericolosita As Integer
        If VerificaQta(Txt_N_CM_1, Cmb_TipoCE_CM_1) Then
            indice_Pericolosita = ControllaLimitePericolosita(Cmb_TipoCE_CM_1.SelectedItem.Value, 0, 0, Me.Txt_N_CM_1.Text)
            Aggiungi_Item(Me.Cmb_TipoCE_CM_1, Me.Txt_N_CM_1, Me.Lista_CM_1, indice_Pericolosita)
        End If
    End Sub

    '###################################################################################
    Private Sub ImgBtn_Ins_CM_2_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Ins_CM_2.Click
        Dim indice_Pericolosita As Integer
        If VerificaQta(Txt_N_CM_2, Cmb_TipoCE_CM_2) Then
            indice_Pericolosita = ControllaLimitePericolosita(Cmb_TipoCE_CM_2.SelectedItem.Value, 0, 0, Me.Txt_N_CM_2.Text)
            Aggiungi_Item(Me.Cmb_TipoCE_CM_2, Me.Txt_N_CM_2, Me.Lista_CM_2, indice_Pericolosita)
        End If
    End Sub

    '###################################################################################
    Private Sub ImgBtn_Del_CM_2_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Del_CM_2.Click
        Delete_Item(Lista_CM_2)
    End Sub


End Class