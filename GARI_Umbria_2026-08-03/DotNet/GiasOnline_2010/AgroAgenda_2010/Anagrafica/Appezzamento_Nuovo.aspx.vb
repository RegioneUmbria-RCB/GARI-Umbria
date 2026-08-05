Imports System.Web
Imports System.Web.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreUtility.CaricaListControl

Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreXML
Imports AgronicaCoreAnagrafeBIZ
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModello
Imports System.Web.Script.Serialization
Imports AgroAgenda_2010.Resources

Public Class Appezzamento_Nuovo
    Inherits System.Web.UI.Page

    Public objParametri_Server As AgronicaCoreParametri
    Public objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda

    Public Operazione As Integer
    Dim Operazione_Contatti As Integer

    ''----- Gestione Querystring
    'Dim Qs_Key As String
    'Dim Qs_Operazione As String
    Dim Qs_Piva As String
    'Dim Qs_PivaPadre As String
    Dim Qs_PivaNuova As String
    'Dim Qs_PaginaRitorno As String
    Dim Qs_Visibilita As Integer = 0

    Dim xPiva As String
    Dim xSa_Cod As String
    Dim xAppezza As String
    Dim xId_Imp As String
    Dim xCampo_Cod As String

    Dim xValiditaInizio As Date
    Dim xValiditaFine As Date

    Dim BaseCode As Integer
    Dim TopCode As Integer

    Public permessi As PermessiUtente

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_Cultivar(ByVal parametro As String) As String

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim cmb_cultivar As New DropDownList

        'AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(cmb_cultivar, True, "SELEZIONA", "", _
        '                                                                 " Imprese.rag_soc LIKE '%" & parametro & "%'", " ORDER BY Rag_Soc asc", _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Server"), _
        '                                                                 HttpContext.Current.Session("ASG_objParametri_Utenti"))
        AgronicaCoreUtility.CaricaListControl.Cultivar(cmb_cultivar, True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", parametro, 0, "", True, 0, 0, "", "",
                                                                         HttpContext.Current.Session("ASG_objParametri_Server"),
                                                                         HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim rval As String = ""
        For Each itm As ListItem In cmb_cultivar.Items
            rval &= "<option value=""" & itm.Value & """>" & itm.Text & "</option>"
        Next

        'Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        'HttpContext.Current.Session("prova") = "caio"

        Return rval

    End Function

    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Select_Finalita(ByVal parametro As String) As String

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim cmb_finalita As New DropDownList

        Dim rval As String = ""

        If parametro <> "0" Then

            Dim objImpostazioni_Utenti As New Utenti_Impostazioni_Read
            Dim DefaultFinalita = objImpostazioni_Utenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(
                enum_Impostazioni_Utenti.UTENTE_COD_FINALITA, HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Finalita(cmb_finalita, True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", parametro, 0, "", "", "",
                     HttpContext.Current.Session("ASG_objParametri_Server"))

            For Each itm As ListItem In cmb_finalita.Items
                rval &= "<option value=""" & itm.Value & """ " & If(itm.Value = DefaultFinalita, "selected", "") & ">" & itm.Text & "</Option>"
            Next

        End If

        Return rval

    End Function



    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function Variabili_In_Session(ByVal cmb_finalita As String, ByVal cmb_specie As String, Cmb_CodiciTerreno As String) As String

        HttpContext.Current.Session("cmb_finalita") = cmb_finalita
        HttpContext.Current.Session("cmb_specie") = cmb_specie
        HttpContext.Current.Session("Cmb_CodiciTerreno") = Cmb_CodiciTerreno

        Return "ok"

    End Function


    '##########################################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        ' Pulisco la Sessione
        'HttpContext.Current.Session("dt_Padri") = Nothing
        'HttpContext.Current.Session("dt_Codici") = Nothing

        Dim TargetUrl As String
        TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda, , Qs_Visibilita)
        Response.Redirect(TargetUrl)

    End Sub

    Private Sub Appezzamento_Nuovo_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        ' VAnni: 2/10/2017: nuovo AgroMasterPage
        'AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        CType(Me.Master, AgendaBootstrap).flag_MostraBtnIndietro = True

    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master.flag_pag_Anagrafica = True

        permessi = New PermessiUtente()

        objParametriAgenda = New ParametriAgenda

        xPiva = objParametriAgenda.Piva
        xSa_Cod = objParametriAgenda.Sa_Cod
        xAppezza = objParametriAgenda.Appezza
        xCampo_Cod = objParametriAgenda.Campo_Cod


        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If
        If Request.QueryString("visibilita") IsNot Nothing AndAlso IsNumeric(Request.QueryString("visibilita")) Then
            Qs_Visibilita = CInt(Request.QueryString("visibilita"))
        End If
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        Master.Lbl_Titolo.Text = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Campo_Edit.aspx", "CreazioneGuidataNuovoAppezzamentoEImpianto"), String)

        Dim year As String
        year = Date.Today.Year

        ' AgronicaCoreUtility.CaricaListControl.SpecieVegetale_Optimize_x_PDC_Modificata(Cmb_Specie, True, "SELEZIONA", "", objParametri_Server)
        CaricaCheckBoxList_SpecieVegetale_Optimize(Cmb_Specie, 0, True, "", "", 0, 0, objParametri_Server, objParametri_Utenti)
        CaricaListControl.CodiciTerreno(Cmb_CodiciTerreno, True, AgronicaAgenda_2010.Seleziona.ToUpper(), "", "", "", objParametri_Server)

        If Not IsPostBack Then

            ' Inserisco i dati nelle label in testata
            'Centro Aziendale
            Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            LblCentro.Text = objCentriAz.SaNome_from_SaCod(xPiva, xSa_Cod, objParametri_Server)

            'Appezzamento
            'Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            'LblAppezza.Text = objAppezza.AppezzamentoNome_from_Appezza(xPiva, xSa_Cod, xAppezza, objParametri_Server)


            If (Not IsNothing(xCampo_Cod) AndAlso IsNumeric(xCampo_Cod) AndAlso xCampo_Cod <> 0) Then
                Dim objCampo As New AgronicaCoreAnagrafeDAL.Campi_R
                LblCampo.Text = objCampo.CampoDes_from_CampoCod(xPiva, xSa_Cod, xCampo_Cod, objParametri_Server)
                objCampo = Nothing
                If controlloSeCampoSquadro(xPiva, xSa_Cod, xCampo_Cod) Then
                    'btn_salva1.Visible = False
                    'btn_salva3.Visible = False
                    'btn_salva4.Visible = False
                    'TxtSuperficie.Text = "0"
                    'TxtSuperficie.Enabled = False
                End If
            End If

        End If

    End Sub

    Private Function controlloSeCampoSquadro(xPiva As String, xSa_Cod As Integer, xCampo_Cod As Integer) As Boolean
        Dim objCampoPart As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
        Dim dt_campi = objCampoPart.Leggi(xPiva, xSa_Cod, xCampo_Cod, "", "", "", 0, 0, "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        If Not IsNothing(dt_campi) AndAlso dt_campi.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function


    '########################################################################################
    Private Sub ImgBtn_SalvaTutto_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaTutto.Click

        Salva_Tutto(0)

    End Sub

    Private Sub ImgBtn_SalvaTutto_Procedi1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaTutto_Procedi1.Click

        Salva_Tutto(1)

    End Sub

    Private Sub ImgBtn_SalvaTutto_Procedi2_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaTutto_Procedi2.Click

        Salva_Tutto(2)

    End Sub

    Private Sub ImgBtn_SalvaTutto_Procedi3_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaTutto_Procedi3.Click

        Salva_Tutto(3)

    End Sub

    '########################################################################################
    Private Sub Salva_Nuova_Distinta(ByRef Dpi_Cod As String,
                                    ByRef Reg_Cod As String,
                                    ByRef Regolamento_Concimazione_Cod As String,
                                    ByRef Flag_PubblicoPrivato As String)

        'Dim i As Integer
        Dim Messaggio As String
        Dim StringaDistinta As String
        Dim BooDummy As Boolean

        Dim xValiditaInizio As Date
        Dim xValiditaFine As Date

        'Dim ValiditaInizio_Impianto As Date
        'Dim ValiditaFine_Impianto As Date

        '--- Controllo NOME lotto

        'If Me.Txt_Lotto.Text = "" Then
        '    AgroMsgBox("Inserire il Lotto!", Page)
        '    Exit Sub
        'End If

        '--- Controllo Organismo Referente

        'If Esiste_Obbligo_SalvataggioOrganismoReferente(Server, Session, Page) = True Then

        '    If Not IsNothing(Me.Cmb_OrganismoReferente.SelectedItem) Then
        '        If Me.Cmb_OrganismoReferente.SelectedItem.Text = "" Then
        '            AgroMsgBox("E' obbligatorio impostare l'Organismo Referente!", Page)
        '            Exit Sub
        '        End If
        '    Else
        '        AgroMsgBox("E' obbligatorio impostare l'Organismo Referente!", Page)
        '        Exit Sub
        '    End If

        'End If


        '--- Controllo date

        If Not IsDate(xValiditaInizio) Then
            xValiditaInizio = AGRODATAINIZIO
            'Else
            '    xValiditaInizio = CDate(Txt_ValiditaInizio_Distinta.Text)
        End If

        If Not IsDate(xValiditaFine) Then
            xValiditaFine = AGRODATAFINE
            'Else
            '    xValiditaFine = CDate(Txt_ValiditaFine_Distinta.Text)
        End If

        'If Not IsDate(TxtValiditaInizio.Text) Then
        '    ValiditaInizio_Impianto = AGRODATAINIZIO
        'Else
        '    ValiditaInizio_Impianto = CDate(TxtValiditaInizio.Text)
        'End If

        'If Not IsDate(TxtValiditaFine.Text) Then
        '    ValiditaFine_Impianto = AGRODATAFINE
        'Else
        '    ValiditaFine_Impianto = CDate(TxtValiditaFine.Text)
        'End If


        If Operazione = enum_TipoOperazioneDB.Scrittura Then

            'If xValiditaInizio < ValiditaInizio_Impianto Then
            '    xValiditaInizio = ValiditaInizio_Impianto
            'End If
            'If xValiditaFine > ValiditaFine_Impianto Then
            '    xValiditaFine = ValiditaFine_Impianto
            'End If
            ''If xValiditaInizio < ValiditaInizio_Appezzamento Then
            ''    xValiditaInizio = ValiditaInizio_Appezzamento
            ''End If
            ''If xValiditaFine > ValiditaFine_Appezzamento Then
            ''    xValiditaFine = ValiditaFine_Appezzamento
            ''End If

        Else
            'If xValiditaInizio < ValiditaInizio_Impianto Then
            '    AgroMsgBox("La data di inizio esercizio non può essere antecedente alla data di inizio Impianto!", Page)
            '    Exit Sub
            'End If

            'If xValiditaFine > ValiditaFine_Impianto Then
            '    AgroMsgBox("La data di fine esercizio non può essere posteriore alla data di fine Impianto!", Page)
            '    Exit Sub
            'End If

            'If xValiditaInizio < CDate(InizioAppezzamento.Value) Then
            '    AgroMsgBox("La data di inizio esercizio non può essere antecedente alla data di inizio Appezzamento (" & InizioAppezzamento.Value & ")!", Page)
            '    Exit Sub
            'End If

            'If xValiditaFine > CDate(FineAppezzamento.Value) Then
            '    AgroMsgBox("La data di fine esercizio non può essere posteriore alla data di fine Appezzamento (" & FineAppezzamento.Value & ")!", Page)
            '    Exit Sub
            'End If
            Dim objImpianto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R

            'controllo validita date
            If objImpianto.Esistono_Distinte_Su_Impianto(xPiva,
                                             CInt(xSa_Cod),
                                             CInt(xAppezza),
                                             CInt(xId_Imp),
                                             0,
                                             xValiditaInizio,
                                             xValiditaFine,
                                             HttpContext.Current.Session("ASG_objParametri_Server")) = True Then
                AgroMsgBox(DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Anagrafica/Campo_Edit.aspx", "NellIntervalloDiTempoImpostatoEsisteGiàUnEsercizio"), String), Page)
                Exit Sub
            End If

        End If



        ''--- Apporti massimi di macroelementi

        'If Me.TxtN.Text <> "" Then
        '    If Not IsNumeric(Me.TxtN.Text) Then
        '        AgroMsgBox("Il Massimo Apporto di Azoto deve essere un numero!", Page)
        '        Exit Sub
        '    End If
        'End If
        'If InStr(TxtN.Text, ".") <> 0 Then
        '    TxtN.Text = Replace(TxtN.Text, ".", ",")
        'End If

        'If Me.TxtP2O5.Text <> "" Then
        '    If Not IsNumeric(Me.TxtP2O5.Text) Then
        '        AgroMsgBox("Il Massimo Apporto di Fosforo deve essere un numero!", Page)
        '        Exit Sub
        '    End If
        'End If
        'If InStr(TxtP2O5.Text, ".") <> 0 Then
        '    TxtP2O5.Text = Replace(TxtP2O5.Text, ".", ",")
        'End If

        'If Me.TxtK2O.Text <> "" Then
        '    If Not IsNumeric(Me.TxtK2O.Text) Then
        '        AgroMsgBox("Il Massimo Apporto di Potassio deve essere un numero!", Page)
        '        Exit Sub
        '    End If
        'End If
        'If InStr(TxtK2O.Text, ".") <> 0 Then
        '    TxtK2O.Text = Replace(TxtK2O.Text, ".", ",")
        'End If

        'If Me.TxtMgO.Text <> "" Then
        '    If Not IsNumeric(Me.TxtMgO.Text) Then
        '        AgroMsgBox("Il Massimo Apporto di Magnesio deve essere un numero!", Page)
        '        Exit Sub
        '    End If
        'End If
        'If InStr(TxtMgO.Text, ".") <> 0 Then
        '    TxtMgO.Text = Replace(TxtMgO.Text, ".", ",")
        'End If




        ''controllo i parametri inseriti
        'For i = 0 To DataGridFasi.Items.Count - 1

        '    If CType(DataGridFasi.Items(i).FindControl("TxtBudget"), TextBox).Text = "" Then

        '        CType(DataGridFasi.Items(i).FindControl("TxtBudget"), TextBox).Text = "0,00"
        '    Else

        '        If Not IsNumeric(CType(DataGridFasi.Items(i).FindControl("TxtBudget"), TextBox).Text) Then
        '            Messaggio = "Il Budget deve essere un valore numerico."
        '            AgroMsgBox(Messaggio, Page)
        '            Exit Sub
        '        End If

        '        If InStr(CType(DataGridFasi.Items(i).FindControl("TxtBudget"), TextBox).Text, ".") Then
        '            CType(DataGridFasi.Items(i).FindControl("TxtBudget"), TextBox).Text = Replace( _
        '            CType(DataGridFasi.Items(i).FindControl("TxtBudget"), TextBox).Text, ".", ",")
        '        End If

        '    End If

        'Next


        ''controllo i parametri inseriti nel DataGridLavorati
        'For i = 0 To DataGridLavorati.Items.Count - 1

        '    If CType(DataGridLavorati.Items(i).FindControl("TxtBudget"), TextBox).Text = "" Then

        '        CType(DataGridLavorati.Items(i).FindControl("TxtBudget"), TextBox).Text = "0,00"
        '    Else

        '        If Not IsNumeric(CType(DataGridLavorati.Items(i).FindControl("TxtBudget"), TextBox).Text) Then
        '            Messaggio = "Il Budget deve essere un valore numerico."
        '            AgroMsgBox(Messaggio, Page)
        '            Exit Sub
        '        End If

        '        If InStr(CType(DataGridLavorati.Items(i).FindControl("TxtBudget"), TextBox).Text, ".") Then
        '            CType(DataGridLavorati.Items(i).FindControl("TxtBudget"), TextBox).Text = Replace( _
        '            CType(DataGridLavorati.Items(i).FindControl("TxtBudget"), TextBox).Text, ".", ",")
        '        End If

        '    End If

        'Next

        ''controllo i valori inseriti nelle textbox
        'If Txt_ResaPrevista.Text = "" Then

        '    Txt_ResaPrevista.Text = "0,00"
        'Else

        '    If Not IsNumeric(Txt_ResaPrevista.Text) Then
        '        Messaggio = "La Produzione Prevista deve essere indicata con un valore numerico."
        '        AgroMsgBox(Messaggio, Page)
        '        Exit Sub
        '    End If

        '    If InStr(Txt_ResaPrevista.Text, ".") Then
        '        Txt_ResaPrevista.Text = Replace(Txt_ResaPrevista.Text, ".", ",")
        '    End If

        'End If

        'Txt_ResaPrevista.Text = "0,00"


        ''controllo i valori inseriti nelle textbox
        'If Txt_RicaviPrevisti.Text = "" Then

        '    Txt_RicaviPrevisti.Text = "0,00"
        'Else

        '    If Not IsNumeric(Txt_RicaviPrevisti.Text) Then
        '        Messaggio = "I Ricavi Previsti devono essere indicati con un valore numerico."
        '        AgroMsgBox(Messaggio, Page)
        '        Exit Sub
        '    End If

        '    If InStr(Txt_RicaviPrevisti.Text, ".") Then
        '        Txt_RicaviPrevisti.Text = Replace(Txt_RicaviPrevisti.Text, ".", ",")
        '    End If

        'End If

        'Txt_RicaviPrevisti.Text = "0,00"


        ''controllo i valori inseriti nelle textbox
        'If Me.TxtPianteHa.Text = "" Then
        '    TxtPianteHa.Text = "0"
        'Else
        '    If Not IsNumeric(TxtPianteHa.Text) Then
        '        Messaggio = "Il Numero di Piante deve essere un numero."
        '        AgroMsgBox(Messaggio, Page)
        '        Exit Sub
        '    End If
        '    If InStr(TxtPianteHa.Text, ".") Then
        '        TxtPianteHa.Text = Replace(TxtPianteHa.Text, ".", ",")
        '    End If
        'End If

        'TxtPianteHa.Text = "0"

        'If Me.Txt_semina_prevista.Text <> "" Then
        '    If Not IsDate(Me.Txt_semina_prevista.Text) Then
        '        Messaggio = "La Data Semina prevista deve essere specificata nel formato dd/mm/yyyy "
        '        AgroMsgBox(Messaggio, Page)
        '        Exit Sub
        '    End If
        'End If

        'If Me.Txt_fioritura_prevista.Text <> "" Then
        '    If Not IsDate(Me.Txt_fioritura_prevista.Text) Then
        '        Messaggio = "La Data Fioritura prevista deve essere specificata nel formato dd/mm/yyyy "
        '        AgroMsgBox(Messaggio, Page)
        '        Exit Sub
        '    End If
        'End If

        'If Me.Txt_raccolta_prevista.Text <> "" Then
        '    If Not IsDate(Me.Txt_raccolta_prevista.Text) Then
        '        Messaggio = "La Data Raccolta prevista deve essere specificata nel formato dd/mm/yyyy "
        '        AgroMsgBox(Messaggio, Page)
        '        Exit Sub
        '    End If
        'End If


        'Dim Flag_Redirect As Boolean = False
        Dim Flag_SalvaTutto As Boolean = False

        'apro la connessione e transazione 
        If objParametri_Server.objTransazione Is Nothing Then
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
        Else
            Flag_SalvaTutto = True
        End If

        Try

            ''--------------------------------------------------------------------------------------
            ''se sono in modifica cancello prima tutti CODICI - PARTICELLE associati alla distinta
            'If Me.Lbl_ProgettoCod.Text <> "0" Then

            '    Dim objCodici_W As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W

            '    objCodici_W.CancellaxProgetto(CStr(Me.Lbl_Piva.Text), _
            '                                CInt(Me.Lbl_SaCod.Text), _
            '                                CInt(Me.Lbl_Appezza.Text), _
            '                                CInt(Me.Lbl_IdReg.Text), _
            '                                CInt(Me.Lbl_ProgettoCod.Text), _
            '                                CInt(0), _
            '                                "", _
            '                                objParametri_Server)


            '    objCodici_W = Nothing

            '    Dim objParticelle_W As New AgronicaCoreAnagrafeDAL.ProgettixParticelle_W

            '    objParticelle_W.CancellaxProgetto(CStr(Me.Lbl_Piva.Text), _
            '                                        CInt(Me.Lbl_SaCod.Text), _
            '                                        CInt(Me.Lbl_Appezza.Text), _
            '                                        CInt(Me.Lbl_IdReg.Text), _
            '                                        CInt(Me.Lbl_ProgettoCod.Text), _
            '                                        "", "", "", 0, 0, "", _
            '                                        "", _
            '                                        objParametri_Server)

            '    objParticelle_W = Nothing

            'End If


            Dim objProgetto As New AgronicaCoreAnagrafeBIZ.Progetto_W

            '===============================================
            '=========== XML DISTINTA ==================
            '===============================================
            StringaDistinta = Xml_GeneraStringoneDistinta_Nuovo(Dpi_Cod, Reg_Cod, Regolamento_Concimazione_Cod, Flag_PubblicoPrivato)


            '==============================================
            '=========== SCRITTURA DISTINTA ================
            '===============================================
            BooDummy = objProgetto.Impresa_Progetto_Scrivi(
                                                CStr(StringaDistinta),
                                                Nothing,
                                                Nothing,
                                                objParametri_Server)

            objProgetto = Nothing

            'chiudo la transazione
            If Not Flag_SalvaTutto Then
                ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
                'chiudo la connessione 
                ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            End If
            '=================================

            'Attiva_Pannello(enum_Pannello.Pannello_Generale, False)
            'Me.ImgBtn_Modifica_Distinta.Visible = True
            'Me.ImgBtn_Salva_Distinta.Visible = False



            'Flag_Redirect = True
        Catch exc As Exception

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------

            'Faccio il rollback della transazione
            'If Not Transazione Is Nothing Then
            '    Transazione.Rollback()
            '    Transazione = Nothing
            'End If

            ''Chiudo la connessione se è apertta
            'If (Not Connessione Is Nothing) Then
            '    Connessione.Close()
            '    Connessione = Nothing
            'End If

            'chiudo la transazione con il rollback
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            'chiudo la connessione
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

            'Messaggio di errore
            Messaggio = AgronicaAgenda_2010.SiEVerificatoUnErrore &
                        Chr(13) &
                        exc.Message.ToString()

            'Visualizzo il messaggio di errore
            Call AgroMsgBox(Messaggio, Page)

            '------------------------------------------------

        End Try

        'If Flag_Redirect = True Then
        '    If Qs_Operazione <> enum_TipoOperazioneDB.Scrittura Then

        '        'Ricarico la pagina
        '        Response.Redirect("Edit_Impianto_2.aspx?P=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
        '                          "&o=" & Stringa_Codifica(Qs_Operazione, AgroKey_EncoderDecoder, Server) & _
        '                          "&k=" & Stringa_Codifica(Qs_Key, AgroKey_EncoderDecoder, Server))

        '    End If
        'End If
        'If Flag_SalvaTutto = False Then
        '    If Flag_Redirect = True Then
        '        If Qs_Operazione <> enum_TipoOperazioneDB.Scrittura Then

        '            'Ricarico la pagina
        '            Response.Redirect("Edit_Impianto_2.aspx?P=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) & _
        '                              "&o=" & Stringa_Codifica(Qs_Operazione, AgroKey_EncoderDecoder, Server) & _
        '                              "&k=" & Stringa_Codifica(Qs_Key, AgroKey_EncoderDecoder, Server))

        '        End If
        '    End If
        'End If



    End Sub


    '###################################################################################################
    Private Function Xml_GeneraStringoneDistinta_Nuovo(ByRef Dpi_Cod As String,
                                    ByRef Reg_Cod As String,
                                    ByRef Regolamento_Concimazione_Cod As String,
                                    ByRef Flag_PubblicoPrivato As String) As String

        'Dim i, Indice As Integer
        Dim j As Integer
        'Dim Start As Integer

        'Dim xPiva As String
        'Dim xSa_Cod As Integer
        'Dim xAppezza As Integer
        'Dim xId_Imp As Integer

        'Dim xValiditaInizio As Date
        'Dim xValiditaFine As Date
        'Dim ValiditaInizio_Impianto As Date
        'Dim ValiditaFine_Impianto As Date
        Dim xRicaviPrevisti As Double
        Dim xProduzionePrevista As Double
        Dim Data_Semina_Prevista, Data_Fioritura_Prevista, Data_Raccolta_Prevista As Date

        Dim str_Progetto As String
        Dim str_ProgettoFase As String
        Dim str_PrezzoUnitario As String
        Dim strDescrizioneProgetto As String
        Dim strNomeProgetto As String

        Dim str_DatiCodici As String
        Dim str_CodiceImpianto As String

        Dim str_DatiParticelle As String
        'Dim str_Particella As String

        Dim Id_Cod As Integer
        Dim Val_Cod As String

        'Dim Valore As String
        'Dim Prov As String
        'Dim Com As String
        'Dim Sezione As String
        'Dim Foglio As Integer
        'Dim Numero As Integer
        'Dim Subalterno As String
        'Dim ArrayParticella() As String

        Dim Cod_Progetto As Integer

        Dim P_Ha As Double

        Dim Operazione As enum_TipoOperazioneDB

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_DatiProgetto As System.Xml.XmlElement
        Dim XML_Progetto As System.Xml.XmlElement
        Dim XML_DatiCodici As System.Xml.XmlElement
        Dim XML_DatiParticelle As System.Xml.XmlElement

        'recupero dal viewstate le chiavi dell'impianto selezionato

        'xPiva = Me.Lbl_Piva.Text
        'xSa_Cod = CInt(Me.Lbl_SaCod.Text)
        'xAppezza = CInt(Me.Lbl_Appezza.Text)
        'xId_Imp = CInt(Me.Lbl_IdReg.Text)
        'Cod_Progetto = CInt(Me.Lbl_ProgettoCod.Text)

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------

        Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))

        ' Fisso la tipologia di operazione = scrittura (nuova distinta)
        Operazione = enum_TipoOperazioneDB.Scrittura

        ''controllo date...
        ''se nn sono state impostate x la distinta..gli assegno quelle dell'impianto!!

        'If IsDate(Me.Txt_ValiditaInizio_Distinta.Text) = True Then
        '    xValiditaInizio = Me.Txt_ValiditaInizio_Distinta.Text
        'Else
        '    xValiditaInizio = AGRODATAINIZIO
        'End If

        'If IsDate(Me.Txt_ValiditaFine_Distinta.Text) = True Then
        '    xValiditaFine = Me.Txt_ValiditaFine_Distinta.Text
        'Else
        '    xValiditaFine = AGRODATAFINE
        'End If

        'If Not IsDate(TxtValiditaInizio.Text) Then
        '    ValiditaInizio_Impianto = AGRODATAINIZIO
        'Else
        '    ValiditaInizio_Impianto = CDate(TxtValiditaInizio.Text)
        'End If

        'If Not IsDate(TxtValiditaFine.Text) Then
        '    ValiditaFine_Impianto = AGRODATAFINE
        'Else
        '    ValiditaFine_Impianto = CDate(TxtValiditaFine.Text)
        'End If


        'If (IsDate(Me.TxtValiditaInizio.Text) = True) And (Not IsDate(Me.Txt_ValiditaInizio_Distinta.Text) = True) Then
        '    xValiditaInizio = CDate(Me.TxtValiditaInizio.Text)
        'End If
        'If (IsDate(Me.TxtValiditaFine.Text) = True) And (Not IsDate(Me.Txt_ValiditaFine_Distinta.Text) = True) Then
        '    xValiditaFine = CDate(Me.TxtValiditaFine.Text)
        'End If


        ''se nn sono state impostate x la distinta..gli assegno quelle dell'impianto!!
        'If Me.Txt_RicaviPrevisti.Text <> "" Then
        '    xRicaviPrevisti = Me.Txt_RicaviPrevisti.Text
        'Else
        xRicaviPrevisti = 0
        'End If

        'If Me.Txt_ResaPrevista.Text <> "" Then
        '    xProduzionePrevista = Me.Txt_ResaPrevista.Text
        'Else
        xProduzionePrevista = 0
        'End If

        ' Genero codice OP se attivo algoritmo codifica
        strNomeProgetto = Replica_GIAS.LeggiCodiceProgressivo(xPiva, enum_SequenzaProgressiviTipi.CodiciOPAgriZoo, xValiditaInizio.Year, objParametri_Server)

        If strNomeProgetto = "" Then

            If TxtAppNome.Text = "" Then

                If Me.ChkTerrenoNudo.Checked = False Then

                    strNomeProgetto = Me.TxtAppNome.Text & " - " &
                                             Cmb_Specie.SelectedItem.Text
                Else
                    strNomeProgetto = Me.TxtAppNome.Text & " - " & AgronicaAgenda_2010.TerrenoNudo

                End If

            Else
                strNomeProgetto = Me.TxtAppNome.Text
            End If

        End If


        If Me.ChkTerrenoNudo.Checked = False Then

            Dim vc = HttpContext.Current.Session("cmb_specie")

            Dim objVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R

            Dim dt = objVeg.Leggi(vc, 0, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            strDescrizioneProgetto = AgronicaAgenda_2010.Esercizio & ": " &
                                        Me.TxtAppNome.Text & " - " &
                                        dt.Rows(0).Item("Veg_Des")

        Else

            Dim idcod = HttpContext.Current.Session("Cmb_CodiciTerreno")

            Dim objCod As New AgronicaCoreMetaSchemaDAL.Codici_Anagrafe_R

            Dim dt = objCod.Leggi(idcod, "", "", "", objParametri_Server)

            strDescrizioneProgetto = AgronicaAgenda_2010.Esercizio & ": " &
                                      Me.TxtAppNome.Text & " - " & dt.Rows(0).Item("descrizione")

        End If


        'If Me.Lbl_ProgettoCod.Text = "0" Then
        '    Operazione = enum_TipoOperazioneDB.Scrittura
        'Else
        '    Operazione = enum_TipoOperazioneDB.Modifica
        'End If

        'If Me.TxtPianteHa.Text <> "" Then
        '    P_Ha = Me.TxtPianteHa.Text
        'Else
        P_Ha = 0
        'End If


        'If Me.Txt_semina_prevista.Text <> "" Then
        '    Data_Semina_Prevista = Me.Txt_semina_prevista.Text
        'Else
        Data_Semina_Prevista = AGRODATAINIZIO
        'End If

        'If Me.Txt_fioritura_prevista.Text <> "" Then
        '    Data_Fioritura_Prevista = Me.Txt_fioritura_prevista.Text
        'Else
        Data_Fioritura_Prevista = AGRODATAINIZIO
        'End If

        'If Me.Txt_raccolta_prevista.Text <> "" Then
        '    Data_Raccolta_Prevista = Me.Txt_raccolta_prevista.Text
        'Else
        Data_Raccolta_Prevista = AGRODATAFINE
        'End If

        Dim Disciplinare_Cod As Integer = Dpi_Cod
        Dim Disciplinare_PubblicoPrivato As Integer = Flag_PubblicoPrivato

        'If Cmb_Disciplinare.SelectedItem.Value <> "" Then
        '    Disciplinare_Cod = Split(Cmb_Disciplinare.SelectedItem.Value, "/")(0)
        '    If Disciplinare_Cod <> 0 Then
        '        Disciplinare_PubblicoPrivato = Split(Cmb_Disciplinare.SelectedItem.Value, "/")(1)
        '    End If
        'End If

        Dim Stato As Integer = 0
        'If Not Cmb_Stato.SelectedItem Is Nothing Then
        '    Stato = CInt(Me.Cmb_Stato.SelectedItem.Value)
        'End If
        Dim objXML As New AgronicaCoreXML.AnagrafeXML


        objXML.Xml_ProgettoPerImpianto(enum_CodificaDecodifica.Codifica,
                                                       str_Progetto,
                                                        CInt(Operazione),
                                                xPiva,
                                                xSa_Cod,
                                                Cod_Progetto,
                                                strNomeProgetto,
                                                strDescrizioneProgetto,
                                                CAU_PROGETTO_PRODUZIONE,
                                                 0,
                                                0,
                                                Data_Semina_Prevista,
                                                Data_Raccolta_Prevista,
                                                "",
                                                xAppezza,
                                                xId_Imp,
                                                0,
                                                0,
                                                CInt(Stato),
                                                Reg_Cod,
                                                CInt(Disciplinare_Cod),
                                                Regolamento_Concimazione_Cod,
                                                0,
                                                xRicaviPrevisti,
                                                xProduzionePrevista,
                                                CDate(xValiditaInizio),
                                                CDate(xValiditaFine),
                                                BaseCode,
                                                TopCode, Disciplinare_PubblicoPrivato:=Disciplinare_PubblicoPrivato)

        XmlDoc.LoadXml(str_Progetto)

        XML_DatiProgetto = XmlDoc.SelectSingleNode("DatiProgetto")

        XML_Progetto = XML_DatiProgetto.SelectSingleNode("Progetto")

        '------------------------------------------------
        '----- Costruisco la stringa XML dei CODICI
        '------------------------------------------------

        XML_DatiCodici = XmlDoc.CreateElement("DatiCodici")

        XML_Progetto.AppendChild(XML_DatiCodici)

        str_DatiCodici = ""

        '----- Leggo i dati nei controlli e per ognuno genero un nodo 

        For j = 0 To 3

            Val_Cod = ""

            Select Case j

                Case 0
                    'If Me.TxtN.Text <> "" Then
                    Id_Cod = enum_CodiciAnagrafe.Impianto_LimiteN
                    'Val_Cod = Me.TxtN.Text
                    'End If

                Case 1
                    'If Me.TxtP2O5.Text <> "" Then
                    Id_Cod = enum_CodiciAnagrafe.Impianto_LimiteP
                    'Val_Cod = Me.TxtP2O5.Text
                    'End If

                Case 2
                    'If Me.TxtK2O.Text <> "" Then
                    Id_Cod = enum_CodiciAnagrafe.Impianto_LimiteK
                    'Val_Cod = Me.TxtK2O.Text
                    'End If

                Case 3
                    'If Me.TxtMgO.Text <> "" Then
                    Id_Cod = enum_CodiciAnagrafe.Impianto_LimiteMg
                    'Val_Cod = Me.TxtMgO.Text
                    'End If

            End Select

            If Val_Cod <> "" Then

                'Genero l'XML del singolo nodo solo se i codici sono valorizzati
                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                str_CodiceImpianto,
                                enum_TipoOperazioneDB.Scrittura,
                                Id_Cod,
                                Val_Cod,
                                CDate(xValiditaInizio),
                                CDate(xValiditaFine),
                                BaseCode,
                                TopCode,
                                "Impianto")

                'Inserisco l'XML nella stringa complessiva
                str_DatiCodici = str_DatiCodici & str_CodiceImpianto

            End If

        Next


        ''Genero l'XML del nodo Organismo referente
        'If Not IsNothing(Me.Cmb_OrganismoReferente.SelectedItem) Then
        '    If Me.Cmb_OrganismoReferente.SelectedItem.Text <> "" Then

        '        Id_Cod = enum_CodiciAnagrafe.Organismo_Referente
        '        Val_Cod = Me.Cmb_OrganismoReferente.SelectedItem.Value

        '        'Genero l'XML del singolo nodo
        '        Call XML_Codice(enum_CodificaDecodifica.Codifica, _
        '                        str_CodiceImpianto, _
        '                        enum_TipoOperazioneDB.Scrittura, _
        '                        Id_Cod, _
        '                        Val_Cod, _
        '                        CDate(xValiditaInizio), _
        '                        CDate(xValiditaFine), _
        '                        BaseCode, _
        '                        TopCode, _
        '                        "Impianto")

        '        'Inserisco l'XML nella stringa complessiva
        '        str_DatiCodici = str_DatiCodici & str_CodiceImpianto

        '    End If
        'End If

        ''Genero l'XML del nodo magazzino conferimento
        'If Not IsNothing(Me.Cmb_MagazzinoConferimento.SelectedItem) Then
        '    If Me.Cmb_MagazzinoConferimento.SelectedItem.Text <> "" Then

        '        Id_Cod = enum_CodiciAnagrafe.Magazzino_Conferimento
        '        Val_Cod = Me.Cmb_MagazzinoConferimento.SelectedItem.Value

        '        'Genero l'XML del singolo nodo
        '        Call XML_Codice(enum_CodificaDecodifica.Codifica, _
        '                        str_CodiceImpianto, _
        '                        enum_TipoOperazioneDB.Scrittura, _
        '                        Id_Cod, _
        '                        Val_Cod, _
        '                        CDate(xValiditaInizio), _
        '                        CDate(xValiditaFine), _
        '                        BaseCode, _
        '                        TopCode, _
        '                        "Impianto")

        '        'Inserisco l'XML nella stringa complessiva
        '        str_DatiCodici = str_DatiCodici & str_CodiceImpianto

        '    End If
        'End If

        ''Genero l'XML del nodo capitolato privato
        'If Me.Cmb_CapitolatoPrivato.SelectedItem.Text <> "" Then

        '    Id_Cod = enum_CodiciAnagrafe.Capitolato_Privato
        '    Val_Cod = Me.Cmb_CapitolatoPrivato.SelectedItem.Value

        '    'Genero l'XML del singolo nodo
        '    Call XML_Codice(enum_CodificaDecodifica.Codifica, _
        '                    str_CodiceImpianto, _
        '                    enum_TipoOperazioneDB.Scrittura, _
        '                    Id_Cod, _
        '                    Val_Cod, _
        '                    CDate(xValiditaInizio), _
        '                    CDate(xValiditaFine), _
        '                    BaseCode, _
        '                    TopCode, _
        '                    "Impianto")

        '    'Inserisco l'XML nella stringa complessiva
        '    str_DatiCodici = str_DatiCodici & str_CodiceImpianto

        'End If

        ''Genero l'XML dei nodi Altri codici identificativi dell'impianto
        'For Indice = 0 To ListCodici.Items.Count - 1

        '    'Recupero le informazioni
        '    Id_Cod = CInt(ListCodici.Items(Indice).Value)
        '    Start = InStr(ListCodici.Items(Indice).Text, " =", )
        '    Val_Cod = Right(ListCodici.Items(Indice).Text, ListCodici.Items(Indice).Text.Length - Start - 2)

        '    'Genero l'XML del singolo nodo
        '    Call XML_Codice(enum_CodificaDecodifica.Codifica, _
        '                    str_CodiceImpianto, _
        '                    enum_TipoOperazioneDB.Scrittura, _
        '                    Id_Cod, _
        '                    Val_Cod, _
        '                    CDate(xValiditaInizio), _
        '                    CDate(xValiditaFine), _
        '                    BaseCode, _
        '                    TopCode, _
        '                    "Impianto")

        '    'Inserisco l'XML nella stringa complessiva
        '    str_DatiCodici = str_DatiCodici & str_CodiceImpianto

        'Next


        XML_DatiCodici.InnerXml = str_DatiCodici


        '------------------------------------------------
        '----- Costruisco la stringa XML delle particelle
        '------------------------------------------------

        XML_DatiParticelle = XmlDoc.CreateElement("DatiParticellexProgetto")

        XML_Progetto.AppendChild(XML_DatiParticelle)

        str_DatiParticelle = ""

        ''----- Leggo gli i dati nei controlli e per ognuno genero un nodo 

        'For Indice = 0 To ListParticelle.Items.Count - 1

        '    'Recupero le informazioni
        '    Id_Cod = enum_CodiciAnagrafe.CodiceRigaRiferimentoQuadroP

        '    Start = InStr(ListParticelle.Items(Indice).Text, " =", )
        '    Val_Cod = Right(ListParticelle.Items(Indice).Text, ListParticelle.Items(Indice).Text.Length - Start - 2)

        '    Valore = ListParticelle.Items(Indice).Value
        '    ArrayParticella = Split(Valore, "£")

        '    Prov = ArrayParticella(0)
        '    Com = ArrayParticella(1)
        '    Sezione = ArrayParticella(2)
        '    Foglio = CInt(ArrayParticella(3))
        '    Numero = CInt(ArrayParticella(4))
        '    Subalterno = ArrayParticella(5)

        '    'Genero l'XML del singolo nodo
        '    Call XML_ParticellaxProgetto(enum_CodificaDecodifica.Codifica, _
        '                                str_Particella, _
        '                                enum_TipoOperazioneDB.Scrittura, _
        '                                Prov, _
        '                                Com, _
        '                                Sezione, _
        '                                Foglio, _
        '                                Numero, _
        '                                Subalterno, _
        '                                Id_Cod, _
        '                                Val_Cod, _
        '                                CDate(xValiditaInizio), _
        '                                CDate(xValiditaFine), _
        '                                BaseCode, _
        '                                TopCode)

        '    'Inserisco l'XML nella stringa complessiva
        '    str_DatiParticelle = str_DatiParticelle & str_Particella

        'Next


        XML_DatiParticelle.InnerXml = str_DatiParticelle



        ''------------------------------------------------
        ''----- Costruisco la stringa XML delle FASI
        ''------------------------------------------------

        'For i = 0 To DataGridFasi.Items.Count - 1

        '    str_ProgettoFase += GeneraBloccoProgettoFase(CInt(Operazione), _
        '                                                xPiva, _
        '                                                Cod_Progetto, _
        '                                                CInt(DataGridFasi.Items(i).Cells(2).Text), _
        '                                                DataGridFasi.Items(i).Cells(3).Text, _
        '                                                CInt(DataGridFasi.Items(i).Cells(0).Text), _
        '                                                CInt(DataGridFasi.Items(i).Cells(4).Text), , , , _
        '                                                CInt(CType(DataGridFasi.Items(i).FindControl("TxtBudget"), TextBox).Text), _
        '                                                , , _
        '                                                BaseCode, _
        '                                                TopCode)

        'Next

        'For i = 0 To DataGridLavorati.Items.Count - 1

        '    str_PrezzoUnitario += Xml_PrezzoUnitario(2, _
        '                                            xPiva, _
        '                                            Cod_Progetto, _
        '                                            CInt(DataGridLavorati.Items(i).Cells(3).Text), _
        '                                            CInt(DataGridLavorati.Items(i).Cells(13).Text), _
        '                                            CInt(DataGridLavorati.Items(i).Cells(14).Text), _
        '                                            CInt(DataGridLavorati.Items(i).Cells(0).Text), _
        '                                            CInt(DataGridLavorati.Items(i).Cells(1).Text), _
        '                                            CInt(DataGridLavorati.Items(i).Cells(2).Text), _
        '                                            CInt(DataGridLavorati.Items(i).Cells(5).Text), _
        '                                            CInt(DataGridLavorati.Items(i).Cells(6).Text), _
        '                                            Replace(DataGridLavorati.Items(i).Cells(8).Text, "&nbsp;", ""), _
        '                                            CDbl(CType(DataGridLavorati.Items(i).FindControl("TxtBudget"), TextBox).Text), _
        '                                            BaseCode, _
        '                                            TopCode)


        'Next

        XML_Progetto.InnerXml = str_ProgettoFase & str_PrezzoUnitario & XML_DatiCodici.OuterXml & XML_DatiParticelle.OuterXml

        XmlDoc.AppendChild(XML_DatiProgetto)

        Return XmlDoc.OuterXml


    End Function


    '########################################################################################
    Private Sub Salva_Tutto(ByVal tipo As Integer)

        Dim StrAppezzamento As String
        Dim StrXmlInserisci As String

        Dim StrImpianto As String

        Dim ZeroData As String
        Dim ZeroInt As Integer
        Dim ZeroString As String
        Dim ZeroDecimal As Decimal
        Dim NullString As String

        Dim Appezza As Integer
        Dim Sup_App As Decimal
        Dim X As Decimal
        Dim Y As Decimal
        Dim Zslm As Decimal
        Dim Esposizione As String
        Dim Pendenza As Decimal
        Dim Ubicazione As String
        Dim App_Nome As String
        Dim Campo_Spia As Integer
        Dim Campo_Spia_Area As Decimal
        'Dim Campo_Cod As Integer
        Dim Prossimo As Integer
        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XmlDoc2 As New System.Xml.XmlDocument
        Dim XmlDatiAppezzamenti As System.Xml.XmlElement
        Dim XmlAppezzamento As System.Xml.XmlElement
        '   *   CreateCANCELLATOObject("Agro_Anagrafe.Appezzamento_W")

        Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

        Dim intLastAppe As Integer
        Dim id_cod_terreno As Integer


        'Imposto le variabili dummy
        ZeroData = "0"
        ZeroInt = 0
        ZeroString = "0"
        ZeroDecimal = 0
        NullString = ""

        If tipo = 1 OrElse tipo = 3 Then
            Sup_App = 0
            Dim list_part As List(Of Particella_Anagrafica) = GetParticelleKendo()

            For Each part In list_part
                Sup_App += part.SuperficieImpiegata
            Next
        Else
            Sup_App = TxtSuperficie.Text.Replace(".", ",")
        End If
        X = 0
        Y = 0
        Zslm = 0
        Esposizione = ""
        Pendenza = 0
        Ubicazione = ""
        App_Nome = TxtAppNome.Text
        Campo_Spia = 0
        Campo_Spia_Area = 0
        'Campo_Cod = 0
        Prossimo = 0

        '--- Date di validita
        If Not IsDate(TxtValiditaInizio.Text) Then
            Validita_Inizio = AGRODATAINIZIO
        Else
            Validita_Inizio = CDate(TxtValiditaInizio.Text)
        End If

        If Not IsDate(TxtValiditaFine.Text) Then
            Validita_Fine = AGRODATAFINE
        Else
            Validita_Fine = CDate(TxtValiditaFine.Text)
        End If


        ' Salvo le date in quelle globali per utilizzarle nel salvataggio della distinta (date distinta = date impianto)
        xValiditaInizio = Validita_Inizio
        xValiditaFine = Validita_Fine

        If xCampo_Cod = "" Then
            xCampo_Cod = "0"
        End If

        If xValiditaFine < xValiditaInizio Then
            'FACCIO APPARIRE UN ALERT......
            AgroMsgBox(AgronicaAgenda_2010.DataInizioNonPuòEssereMaggioreDiDataFine, Page)
            Exit Sub
        End If

        Dim disciplinare As String = ""
        Dim Dpi_Cod As String = ""
        Dim Reg_Cod As String = ""
        Dim Regolamento_Concimazione_Cod As String = ""
        Dim Flag_PubblicoPrivato As String = ""
        Dim id_tr As String = ""
        Dim MetodoProduzione_Cod As Integer = 1
        Dim MetodoProduzione_Des As String = ""

        trovaDisciplinare(xPiva,
                          Validita_Inizio,
                          Validita_Fine,
                          disciplinare,
                          Dpi_Cod,
                          Reg_Cod,
                          Regolamento_Concimazione_Cod,
                          Flag_PubblicoPrivato,
                          id_tr,
                          MetodoProduzione_Cod,
                          MetodoProduzione_Des,
                          objParametri_Server,
                          objParametri_Utenti)

        Appezza = 0

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------

        Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))



        'Genero la stringa XML per APPEZZAMENTO
        Call XML_Appezzamento(enum_CodificaDecodifica.Codifica,
                             StrAppezzamento,
                             enum_TipoOperazioneDB.Scrittura,
                             xPiva,
                             xSa_Cod,
                             Appezza,
                             Sup_App,
                             ZeroData,
                             ZeroData,
                             X,
                             Y,
                             Zslm,
                             Esposizione,
                             Pendenza,
                             Ubicazione,
                             ZeroInt,
                             NullString,
                             ZeroDecimal,
                             ZeroDecimal,
                             ZeroDecimal,
                             ZeroDecimal,
                             ZeroDecimal,
                             ZeroDecimal,
                             ZeroDecimal,
                             ZeroDecimal,
                             ZeroDecimal,
                             ZeroDecimal,
                             ZeroDecimal,
                             ZeroDecimal,
                             ZeroString,
                             ZeroInt,
                             ZeroDecimal,
                             ZeroData,
                             ZeroDecimal,
                             ZeroData,
                             ZeroData,
                             ZeroDecimal,
                             ZeroData,
                             NullString,
                             App_Nome,
                             Campo_Spia,
                             Campo_Spia_Area,
                             NullString,
                             xCampo_Cod,
                             Prossimo,
                             ZeroData,
                             ZeroData,
                             Validita_Inizio,
                             Validita_Fine,
                             BaseCode,
                             TopCode)





        '------------------------------------------------
        '----- Costruisco la stringa XML complessiva di inserimento
        '------------------------------------------------

        'Creo il nodo "DatiAppezzamenti"
        XmlDatiAppezzamenti = XmlDoc.CreateElement("DatiAppezzamenti")

        'Inserisco il nodo "Appezzamento"
        XmlDatiAppezzamenti.InnerXml = StrAppezzamento

        'Rendo l'albero figlio del documento
        XmlDoc.AppendChild(XmlDatiAppezzamenti)

        'Faccio una copia del documento XML
        'XmlDoc2 = XmlDoc
        Dim StrCodiciAppezza As String = ""
        Call XML_Codice(enum_CodificaDecodifica.Codifica,
                        StrCodiciAppezza,
                        enum_TipoOperazioneDB.Scrittura,
                        enum_CodiciAnagrafe.MetodoDiProduzione,
                        MetodoProduzione_Cod,
                        AGRODATAINIZIO,
                        AGRODATAFINE,
                        BaseCode,
                        TopCode,
                        "Appezzamento")

        'Seleziono il nodo Appezzamento e all'interno inserisco i nodi figli...sintassi xpath
        XmlAppezzamento = XmlDoc.SelectSingleNode("//Appezzamento")

        'Aaggiungo i codici creati in precedenza...
        XmlAppezzamento.InnerXml = StrCodiciAppezza

        'Estraggo la stringa XML complessiva
        StrXmlInserisci = XmlDoc.InnerXml


        'Distruggo gli oggetti
        XmlDatiAppezzamenti = Nothing
        XmlDoc = Nothing
        'XmlAppezzamento = Nothing
        'XmlDoc2 = Nothing




        Dim Errore As Boolean = False
        Dim Errore2 As Boolean = False
        Dim strDummy As String
        Dim Id_Reg_New As Integer


        Try


            '------------------------------------------------
            '----- Modifico o Inserisco l'APPEZZAMENTO
            '------------------------------------------------
            intLastAppe = objAppezzamento.Appezzamento_Scrivi(
                                            CStr(StrXmlInserisci),
                                            xPiva,
                                            xSa_Cod,
                                            Appezza,
                                                objParametri_Server,
                                                objParametri_Utenti)


            ' Salvo ID Appezzamento nella variabile globale per salvare la Distinta d'Impianto
            xAppezza = Appezza

            If tipo = 1 OrElse tipo = 3 Then

                Salva_Particelle(CInt(hTipoFiltro.Value), Validita_Inizio, Validita_Fine)

            End If

            Dim strDummy2 As String
            Dim Id_Imp As Integer
            Dim Sup_Imp As Decimal
            Dim Cod_Resp As Integer
            Dim Cod_Ente As Integer
            Dim Data As Date
            Dim Cul_Cod As Integer
            Dim Grva_Cod_Veg As Integer
            Dim Data_Raccolta As String
            Dim Scarto As Integer
            Dim Ind_Mat_Cod As Integer
            Dim Ind_Mat_Ril As String
            Dim Sta_Ter As String
            Dim Cop_DI As String
            Dim Cop_DF As String
            Dim Setup_Cod As String
            Dim Port_Cod As Integer
            Dim Stru_Prot As Integer
            Dim Pro_Pag As Integer
            Dim Seme_Q As Integer
            Dim Seme_T As Integer
            Dim Seme_P As Integer
            Dim Seme_D As Integer
            Dim Stato_Residui As String
            Dim Denitrificazione As Integer
            Dim Volatilizzazione As Integer
            Dim ProfonditaLav As Integer
            Dim Cover As Integer
            Dim Monitorato As Integer
            Dim Codice_Ficale_Tecnico As String
            Dim Data_Conversione As String
            Dim Grfi_Cod As Integer
            Dim Imp_Cod As Integer
            Dim Finanziamento As Integer = 0
            Dim Su_Cod As Integer
            Dim Cop_Cod As Integer
            Dim Foral_Cod As Integer
            Dim Tecn_Cod As Integer
            Dim ProvenienzaSeme As Integer
            Dim Id_Consociazione As Integer

            Dim XmlDatiReg_Impianti As System.Xml.XmlElement
            Dim XmlImpianto As System.Xml.XmlElement
            Dim XMLDatiCodici As System.Xml.XmlElement

            Dim objImpianto As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
            Dim BoolDummy As String

            Dim objGias As New AgronicaCoreAnagrafeBIZ.Importa_GIAS

            Dim objCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            If Not IsNothing(HttpContext.Current.Session("cmb_specie")) And HttpContext.Current.Session("cmb_specie") <> "" And HttpContext.Current.Session("cmb_specie") <> NullString Then
                Cul_Cod = objCultivar.VarietaAltre(HttpContext.Current.Session("cmb_specie"), objParametri_Server)


                Dim flag_0NoBio_1SoloBio_2Entrambi As Integer
                Dim cultivar As New AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta
                cultivar.codice = Cul_Cod
                cultivar.specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie
                cultivar.specie.codice = HttpContext.Current.Session("cmb_specie")

                objCultivar.VegDes_CulDes_from_Vegcod_CulCod(cultivar.specie.codice, cultivar.codice,
                                                                      cultivar.specie.descrizione, cultivar.descrizione,
                                                                      objParametri_Server)

                Select Case (CType(Reg_Cod, Integer))
                    Case enum_Cod_Regolamento.Regolamento_bio
                        flag_0NoBio_1SoloBio_2Entrambi = 1
                    Case enum_Cod_Regolamento.Regolamento_Nessuno
                        flag_0NoBio_1SoloBio_2Entrambi = 0
                    Case Else
                        flag_0NoBio_1SoloBio_2Entrambi = 2
                End Select

                objGias.Crea_MateriaPrima_Specie_Varieta_Regolamento(cultivar, flag_0NoBio_1SoloBio_2Entrambi,
                                                                     objParametri_Server, objParametri_Utenti,
                                                                     True, True)

            Else
                ' Terreno nudo
                Cul_Cod = 0
            End If

            id_cod_terreno = 0
            If Not IsNothing(HttpContext.Current.Session("Cmb_CodiciTerreno")) And HttpContext.Current.Session("Cmb_CodiciTerreno") <> "" And HttpContext.Current.Session("Cmb_CodiciTerreno") <> NullString Then
                id_cod_terreno = CInt(HttpContext.Current.Session("Cmb_CodiciTerreno"))
            Else
                ' Terreno nudo
                id_cod_terreno = 0
            End If

            Id_Imp = 0
            ' la superficie dell'impianto è = a quella del appezzamento (solo in default)
            Sup_Imp = Sup_App
            Cod_Resp = 0
            Cod_Ente = 0
            Campo_Spia = 0
            Grva_Cod_Veg = 0
            Dim resa_prevista As Decimal = 0
            Dim resa_effettiva As Decimal = 0
            Scarto = 0
            Ind_Mat_Cod = 0
            Ind_Mat_Ril = "0"
            Sta_Ter = ""
            Cop_DI = "0"
            Cop_DF = "0"
            Setup_Cod = -1
            Port_Cod = -1
            Stru_Prot = 0
            Pro_Pag = 0
            Seme_Q = 0
            Seme_T = 0
            Seme_P = 0
            Seme_D = 0
            Stato_Residui = ""
            Denitrificazione = 0
            Volatilizzazione = 0
            ProfonditaLav = 0
            Codice_Ficale_Tecnico = ""
            Data_Conversione = "0"

            If Not IsNothing(HttpContext.Current.Session("cmb_finalita")) AndAlso HttpContext.Current.Session("cmb_finalita") <> "" AndAlso HttpContext.Current.Session("cmb_finalita") <> "null" Then
                Grfi_Cod = HttpContext.Current.Session("cmb_finalita")
            Else
                ' Terreno nudo
                Grfi_Cod = 0
            End If

            Imp_Cod = -1

            Su_Cod = -1
            Cop_Cod = -1
            Foral_Cod = -1
            Tecn_Cod = -1
            ProvenienzaSeme = 0
            Id_Consociazione = 0

            Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))


            'Genero la stringa XML per IMPIANTO
            Call XML_Impianto(enum_CodificaDecodifica.Codifica,
                                StrImpianto,
                                enum_TipoOperazioneDB.Scrittura,
                                xPiva,
                                xSa_Cod,
                                xCampo_Cod,
                                Appezza,
                                Id_Imp,
                                Sup_Imp,
                                Cod_Resp,
                                Cod_Ente,
                                Campo_Spia,
                                Data,
                                Cul_Cod,
                                Grva_Cod_Veg,
                                Data_Raccolta,
                                resa_prevista,
                                resa_effettiva,
                                Scarto,
                                Ind_Mat_Cod,
                                Ind_Mat_Ril,
                                Sta_Ter,
                                Cop_DI,
                                Cop_DF, 0,
                                0,
                                0,
                                Setup_Cod,
                                Port_Cod,
                                Stru_Prot,
                                Pro_Pag,
                                Seme_Q,
                                Seme_T,
                                Seme_P,
                                Seme_D,
                                Stato_Residui,
                                Denitrificazione,
                                Volatilizzazione,
                                ProfonditaLav,
                                Cover,
                                Monitorato,
                                Codice_Ficale_Tecnico,
                                Data_Conversione,
                                Grfi_Cod,
                                Imp_Cod,
                                Reg_Cod,
                                Finanziamento,
                                Su_Cod,
                                Cop_Cod,
                                Foral_Cod,
                                Tecn_Cod,
                                ProvenienzaSeme,
                                Validita_Inizio,
                                Validita_Fine,
                                BaseCode,
                                TopCode,
                                Id_Consociazione,
                                destinazioneuso:=id_cod_terreno)



            '------------------------------------------------
            '----- Costruisco la stringa XML complessiva di inserimento
            '------------------------------------------------



            'Creo il nodo "DatiReg_Impianti"
            XmlDatiReg_Impianti = XmlDoc2.CreateElement("DatiReg_Impianti")

            'Inserisco il nodo "Reg_Impianto"
            XmlDatiReg_Impianti.InnerXml = StrImpianto

            'Rendo l'albero figlio del documento
            XmlDoc2.AppendChild(XmlDatiReg_Impianti)

            ' Genero codice impianto se attivo algoritmo codifica
            Dim StrCodici As String
            Dim Codice_Impianto = Replica_GIAS.LeggiCodiceProgressivo(xPiva, enum_SequenzaProgressiviTipi.CodiciProgettoAgricoli, xValiditaInizio.Year, objParametri_Server)

            If Codice_Impianto <> "" Then

                XmlImpianto = XmlDoc2.SelectSingleNode("//Reg_Impianto")
                XMLDatiCodici = XmlDoc2.CreateElement("DatiCodici")

                Call XML_Codice(enum_CodificaDecodifica.Codifica,
                                StrCodici,
                                enum_TipoOperazioneDB.Scrittura,
                                enum_CodiciAnagrafe.Codice_Impianto,
                                Codice_Impianto,
                                CDate(xValiditaInizio),
                                CDate(xValiditaFine),
                                BaseCode,
                                TopCode,
                                "Impianto")

                XMLDatiCodici.InnerXml = StrCodici
                XmlImpianto.AppendChild(XMLDatiCodici)

            End If

            'Seleziono il nodo "Reg_Impianto"
            'XmlImpianto = XmlDoc.SelectSingleNode("//Reg_Impianto")

            'Creo il nodo "DatiCodici"
            'XmlDatiCodici = XmlDoc.CreateElement("DatiCodici")

            'Inserisco gli elementi "Codice" come figli del nodo "DatiCodici"
            'XmlDatiCodici.InnerXml = StrCodici

            'Rendo "DatiCodici" figlio del nodo "Reg_Impianto"
            'XmlImpianto.AppendChild(XmlDatiCodici)

            'Estraggo la stringa XML complessiva
            StrXmlInserisci = XmlDoc2.InnerXml

            'Distruggo gli oggetti
            XmlDatiReg_Impianti = Nothing
            XmlDoc2 = Nothing


            Try

                '------------------------------------------------
                '----- Modifico o Inserisco l'IMPIANTO
                '------------------------------------------------

                BoolDummy = objImpianto.Reg_Impianto_Scrivi(
                          CStr(StrXmlInserisci),
                          Nothing,
                          Nothing,
                          Nothing,
                          Id_Reg_New,
                          "",
                          objParametri_Server)

                objImpianto = Nothing

                ' Salvo ID dell'Impianto appena creato nella globale
                xId_Imp = Id_Reg_New

                ' Salvo la prima distinta dell'impianto
                Salva_Nuova_Distinta(Dpi_Cod, Reg_Cod, Regolamento_Concimazione_Cod, Flag_PubblicoPrivato)

                HttpContext.Current.Session("cmb_finalita") = Nothing
                HttpContext.Current.Session("cmb_specie") = Nothing
                HttpContext.Current.Session("Cmb_CodiciTerreno") = Nothing

            Catch ex As Exception

                Errore2 = True

                '------------------------------------------------
                'Si e' verificata una eccezione !!!!!!
                '------------------------------------------------
                'chiudo la transazione con il rollback
                If objParametri_Server.objConnessione IsNot Nothing Then
                    If objParametri_Server.objTransazione IsNot Nothing Then
                        'chiudo transazione
                        ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                    End If
                    'chiudo la connessione
                    ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
                End If

                'Messaggio di errore
                strDummy2 = ex.Message.ToString()


                'FACCIO APPARIRE UN ALERT......
                AgroMsgBox(AgronicaAgenda_2010.SiÈVerificatoUnErroreDuranteLaFaseDiSalvat & vbCrLf & strDummy, Page)

                '------------------------------------------------

            End Try

        Catch exc As Exception

            Errore = True

            '------------------------------------------------
            'Si e' verificata una eccezione !!!!!!
            '------------------------------------------------
            'chiudo la transazione con il rollback
            If objParametri_Server.objConnessione IsNot Nothing Then
                If objParametri_Server.objTransazione IsNot Nothing Then
                    'chiudo transazione
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                End If
                'chiudo la connessione
                ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            End If

            'Messaggio di errore
            strDummy = exc.Message.ToString()


            'FACCIO APPARIRE UN ALERT......
            AgroMsgBox(AgronicaAgenda_2010.SiÈVerificatoUnErroreDuranteLaFaseDiSalvat & vbCrLf & strDummy, Page)

            '------------------------------------------------

        End Try

        If Not Errore AndAlso Not Errore2 Then

            'Salvo come parametro globale l'appezzamento creato
            objParametriAgenda.Appezza = Appezza
            'Salvo come parametro globale l'impianto creato
            objParametriAgenda.Id_Imp = Id_Reg_New


            'Cambio la modalità di operazione (in modifica!)
            objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica

            'Gestione redirect
            Select Case tipo
                Case 0
                    Dim TargetUrl As String
                    TargetUrl = Master.TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAnagrafica, objParametriAgenda)
                    If Qs_Visibilita <> 0 Then
                        TargetUrl &= "?visibilita=" & CStr(Qs_Visibilita)
                    End If
                    Response.Redirect(TargetUrl)

                Case 1
                    Dim TargetUrl = "Appezzamento_Edit.aspx"
                    If Qs_Visibilita <> 0 Then
                        TargetUrl &= "?visibilita=" & CStr(Qs_Visibilita)
                    End If
                    Server.Transfer(TargetUrl, True)

                Case 2
                    Dim TargetUrl = "Impianto_Edit2.aspx"
                    If Qs_Visibilita <> 0 Then
                        TargetUrl &= "?visibilita=" & CStr(Qs_Visibilita)
                    End If
                    Server.Transfer(TargetUrl, True)

                Case 3
                    Dim TargetUrl = "Impianto_Edit2.aspx"
                    If Qs_Visibilita <> 0 Then
                        TargetUrl &= "?visibilita=" & CStr(Qs_Visibilita)
                    End If
                    Server.Transfer(TargetUrl, True)

            End Select

        End If


    End Sub


    Public Sub trovaDisciplinare(Piva As String,
                                 ByVal DataInizio As Date,
                                 ByVal DataFine As Date,
                                 ByRef Disciplinare As String,
                                 ByRef Dpi_Cod As String,
                                 ByRef Reg_Cod As String,
                                 ByRef Regolamento_Concimazione_Cod As String,
                                 ByRef Flag_PubblicoPrivato As String,
                                 ByRef id_tr As String,
                                 ByRef MetodoProduzione_Cod As Integer,
                                 ByRef MetodoProduzione_Des As String,
                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreParametri)

        Disciplinare = ""
        Dpi_Cod = 0
        Reg_Cod = 1
        Regolamento_Concimazione_Cod = 0
        Flag_PubblicoPrivato = 0
        id_tr = 0
        MetodoProduzione_Cod = 1
        MetodoProduzione_Des = "Convenzionale"

        Dim ic_r As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim val As String = ic_r.Leggi_Codice_from_Imprese_Codici(Piva, enum_CodiciAnagrafe.Disciplinare_Aziendale_Default, objParametri_Server)
        If val <> "" Then
            impostaDisciplinareDaPreferenza(val, DataInizio, DataFine, Disciplinare, Dpi_Cod, Reg_Cod, Regolamento_Concimazione_Cod, Flag_PubblicoPrivato, id_tr, MetodoProduzione_Cod, MetodoProduzione_Des, objParametri_Server, objParametri_Utenti)
        Else
            Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            val = ObjUtenti.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_DPI_PREDEFINITO, objParametri_Utenti, 1)
            If val <> "" Then
                impostaDisciplinareDaPreferenza(val, DataInizio, DataFine, Disciplinare, Dpi_Cod, Reg_Cod, Regolamento_Concimazione_Cod, Flag_PubblicoPrivato, id_tr, MetodoProduzione_Cod, MetodoProduzione_Des, objParametri_Server, objParametri_Utenti)
            End If

        End If


    End Sub


    Public Sub impostaDisciplinareDaPreferenza(ByVal preferenza As String,
                                 ByVal DataInizio As Date,
                                 ByVal DataFine As Date,
                                 ByRef Disciplinare As String,
                                 ByRef Dpi_Cod As String,
                                 ByRef Reg_Cod As String,
                                 ByRef Regolamento_Concimazione_Cod As String,
                                 ByRef Flag_PubblicoPrivato As String,
                                 ByRef id_tr As String,
                                 ByRef MetodoProduzione_Cod As Integer,
                                 ByRef MetodoProduzione_Des As String,
                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                 ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim ASG_SuperUser_CodFiscale = objParametri_Server.PivaSuperUser
        Dim ASG_Utente_Username_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, CostantiPersonalizzate.AgroKey_EncoderDecoder)

        Dim xletturautente As New AgronicaCoreUtentiDAL.Utenti_Read
        Dim pass As String = xletturautente.Password_From_UserName(objParametri_Server.SuperUserUsername, objParametri_Utenti)
        Dim ASG_Utente_Password_Crypt = Sicurezza.Stringa_Codifica_LANCompatibile(pass, CostantiPersonalizzate.AgroKey_EncoderDecoder)

        Dim agrowc = New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = True, .Flag_DisciplinareAttivo = True}

        Dim x As New AgronicaCoreDpiBIZ.CaricaListControl
        If preferenza.Contains("e:") Then
            'Metodo nuovo
            Dim ente = preferenza.Split("/")(0).Split(":")(1)
            Dim fp = preferenza.Split("/")(1).Split(":")(1)
            x.Trova_DisciplinareCompleto_Ente(ente,
                                              fp,
                                              ASG_SuperUser_CodFiscale,
                                              ASG_Utente_Username_Crypt,
                                              ASG_Utente_Password_Crypt,
                                              objParametri_Server,
                                              objParametri_Utenti,
                                              0, 0, 0, 0,
                                              True, True, True,
                                              agrowc,
                                              Disciplinare, Dpi_Cod, Reg_Cod, Regolamento_Concimazione_Cod, Flag_PubblicoPrivato, id_tr, MetodoProduzione_Cod, MetodoProduzione_Des)
        ElseIf preferenza = "-2" Then

            Disciplinare = "-2"
            Dpi_Cod = "0"
            Reg_Cod = "4"
            Regolamento_Concimazione_Cod = "0"
            Flag_PubblicoPrivato = "0"
            id_tr = "0"
            MetodoProduzione_Cod = 3
            MetodoProduzione_Des = "Biologico"

        Else

            'metodo Vecchio
            Dim ente = ""
            Dim fp = ""
            x.Trova_Ente_Disciplinare_NoSession(preferenza, ASG_SuperUser_CodFiscale,
                                              ASG_Utente_Username_Crypt,
                                              ASG_Utente_Password_Crypt, objParametri_Server, objParametri_Utenti, 0, 0, 0, 0, True, True, True,
                                              agrowc, ente, fp)

            If ente <> "" Then
                x.Trova_DisciplinareCompleto_Ente(ente,
                                              fp,
                                              ASG_SuperUser_CodFiscale,
                                              ASG_Utente_Username_Crypt,
                                              ASG_Utente_Password_Crypt,
                                              objParametri_Server,
                                              objParametri_Utenti,
                                              0, 0, 0, 0,
                                              True, True, True,
                                              agrowc,
                                              Disciplinare, Dpi_Cod, Reg_Cod, Regolamento_Concimazione_Cod, Flag_PubblicoPrivato, id_tr, MetodoProduzione_Cod, MetodoProduzione_Des)
            End If

        End If

    End Sub



    Public Sub Salva_Particelle(ByVal type As Integer, ByVal dataInizio As Date, ByVal dataFine As Date)

        Dim objParametriAgenda As New ParametriAgenda

        Dim chiaveP_hash As String
        Dim Particelle_hash As New Hashtable
        Dim chiaveM_hash As String
        Dim Macrousi_hash As New Hashtable
        Dim chiaveU_hash As String
        Dim Utilizzi_hash As New Hashtable


        Dim Dt_Particelle As New DataTable

        Dim DataValiditaInizio As Date = dataInizio
        Dim DataValiditaFine As Date = dataFine


        Dim Macrouso_Cod As String
        Dim Sup_Macrouso As String
        Dim Veg_Cod_Agea As String
        Dim Cul_Cod_Agea As String
        Dim Sup_Utilizzo As String

        Dim SuperficieIntersezione As Double
        Dim SuperficieIntersezioneMacrouso As Double

        Dim Ettari As Integer
        Dim Are, Centiare As Integer

        Dim intDummy As Integer

        Dim ObjPartW As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W
        Dim ObjPartMacrW As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousi_W
        Dim ObjPartMacrUtW As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_W

        Dim list_part As List(Of Particella_Anagrafica) = GetParticelleKendo()


        If list_part.Count > 0 Then
            ' divido le chiavi per ogni particella selezionata per il salvataggio 

            'arr_chiave = Split(chiave, "-")

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(dataInizio, dataFine)

            ' scarico tutte le particelle a disposizione (a seconda se ci sono macrousi e utilizzi o no)
            If objParametriAgenda.Campo_Cod = "0" Then

                Dim objParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

                Select Case type
                    Case 0 ' nessun check selezionato
                        'Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), DataValiditaInizio, DataValiditaFine, HttpContext.Current.Session("ASG_objParametri_Server"))
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), DataValiditaInizio, DataValiditaFine, objParametri_Server)
                    Case 1 ' macrousi ok
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_Macrousi(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), DataValiditaInizio, DataValiditaFine, objParametri_Server)
                    Case 2, 3, 4 ' utilizzi ok ' entrambi ok ' entrambi ok + varietà
                        Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), DataValiditaInizio, DataValiditaFine, objParametri_Server)
                    Case Else
                        Throw New Exception(AgronicaAgenda_2010.TipoRecuperoParticelleNonGestito)
                End Select
            Else

                Dim objCampixParticelleR As New AgronicaCoreAnagrafeDAL.CampixParticelle_R

                If objCampixParticelleR.Campo_Definito_Come_Squadro(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), CInt(objParametriAgenda.Campo_Cod), objParametri_Server) = True Then

                    Dim objParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.CampixParticelle_R

                    Select Case type
                        Case 0 ' nessun check selezionato
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), objParametriAgenda.Campo_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
                        Case 1 ' macrousi ok
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro_con_Macrousi(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), objParametriAgenda.Campo_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
                        Case 2, 3, 4 ' utilizzi ok ' entrambi ok  ' entrambi ok + varità
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_CAMPO_Squadro_con_MacrousiUtilizzo(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), objParametriAgenda.Campo_Cod, DataValiditaInizio, DataValiditaFine, objParametri_Server)
                        Case Else
                            Throw New Exception(AgronicaAgenda_2010.TipoRecuperoParticelleNonGestito)
                    End Select
                Else
                    Dim objParticelleCatastali_R As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

                    Select Case type
                        Case 0 ' nessun check selezionato
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), DataValiditaInizio, DataValiditaFine, objParametri_Server)
                        Case 1 ' macrousi ok
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_Macrousi(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), DataValiditaInizio, DataValiditaFine, objParametri_Server)
                        Case 2, 3, 4 ' utilizzi ok ' entrambi ok ' entrambi ok + varietà
                            Dt_Particelle = objParticelleCatastali_R.Recupera_Particelle_per_Centro_con_MacrousiUtilizzi(CStr(objParametriAgenda.Piva), CInt(objParametriAgenda.Sa_Cod), DataValiditaInizio, DataValiditaFine, objParametri_Server)
                    End Select
                End If
            End If

            Dim list_part2 = list_part
            ' ciclo su righe particelle
            For Each particella In list_part


                'creo la stringa CHIAVE
                chiaveP_hash = particella.CodiceIstat_Provincia & "|" & particella.CodiceIstat_Comune & "|" & particella.Sezione & "|" & particella.Foglio.ToString & "|" & particella.Numero.ToString & "|" & particella.Subalterno

                If Not Particelle_hash.ContainsKey(chiaveP_hash) Then

                    For Each particella2 In list_part2

                        If particella.CodiceIstat_Provincia = particella2.CodiceIstat_Provincia AndAlso
                            particella.CodiceIstat_Comune = particella2.CodiceIstat_Comune AndAlso
                            particella.Sezione = particella2.Sezione AndAlso
                            particella.Foglio = particella2.Foglio AndAlso
                            particella.Numero = particella2.Numero AndAlso
                            particella.Subalterno = particella2.Subalterno Then

                            If IsNumeric(particella2.SuperficieImpiegata) Then

                                SuperficieIntersezioneMacrouso = particella2.SuperficieImpiegata
                                SuperficieIntersezione += SuperficieIntersezioneMacrouso


                                ' Se è abilitato Macrousi
                                If (type = 1) OrElse (type = 3) OrElse (type = 4) Then
                                    Macrouso_Cod = particella2.Macrouso_Cod.ToString
                                    Sup_Macrouso = particella2.Sup_Macrouso.ToString
                                End If

                                ' Se è abilitato Utilizzi
                                If (type = 2) OrElse (type = 3) OrElse (type = 4) Then
                                    Cul_Cod_Agea = particella2.Cul_Cod_Agea.ToString
                                    Sup_Utilizzo = particella2.Sup_Utilizzo.ToString
                                End If

                                If (type = 4) Then
                                    Veg_Cod_Agea = particella2.Veg_Cod_Agea.ToString
                                End If


                                If type <> 0 Then

                                    chiaveM_hash = particella2.CodiceIstat_Provincia & "|" & particella2.CodiceIstat_Comune & "|" & particella2.Sezione & "|" & particella2.Foglio.ToString & "|" & particella2.Numero.ToString & "|" & particella2.Subalterno & "|" & Macrouso_Cod & "|" & Sup_Macrouso '& "|" & SuperficieIntersezioneMacrouso
                                    chiaveU_hash = particella2.CodiceIstat_Provincia & "|" & particella2.CodiceIstat_Comune & "|" & particella2.Sezione & "|" & particella2.Foglio.ToString & "|" & particella2.Numero.ToString & "|" & particella2.Subalterno & "|" & Macrouso_Cod & "|" & Sup_Macrouso & "|" & Veg_Cod_Agea & "|" & Cul_Cod_Agea & "|" & Sup_Utilizzo

                                    If Macrouso_Cod <> "" AndAlso Macrouso_Cod <> "&nbsp;" AndAlso Not Macrousi_hash.ContainsKey(chiaveM_hash) Then

                                        Macrousi_hash.Add(chiaveM_hash, "")

                                        intDummy = ObjPartMacrW.Scrivi(
                                                                CStr(xPiva),
                                                                CInt(xSa_Cod),
                                                                CInt(xAppezza),
                                                                CStr(particella2.CodiceIstat_Provincia),
                                                                CStr(particella2.CodiceIstat_Comune),
                                                                CStr(particella2.Sezione),
                                                                CInt(particella2.Foglio),
                                                                CInt(particella2.Numero),
                                                                CStr(particella2.Subalterno),
                                                                Macrouso_Cod,
                                                                CDbl(SuperficieIntersezioneMacrouso),
                                                                DataValiditaInizio,
                                                                DataValiditaFine,
                                                                objParametri_Server)
                                    End If

                                    If Veg_Cod_Agea <> "" AndAlso Veg_Cod_Agea <> "&nbsp;" AndAlso Not Utilizzi_hash.ContainsKey(chiaveU_hash) Then

                                        Utilizzi_hash.Add(chiaveU_hash, "")

                                        intDummy = ObjPartMacrUtW.Scrivi(
                                                                CStr(xPiva),
                                                                CInt(xSa_Cod),
                                                                CInt(xAppezza),
                                                                CStr(particella2.CodiceIstat_Provincia),
                                                                CStr(particella2.CodiceIstat_Comune),
                                                                CStr(particella2.Sezione),
                                                                CInt(particella2.Foglio),
                                                                CInt(particella2.Numero),
                                                                CStr(particella2.Subalterno),
                                                                Macrouso_Cod,
                                                                Veg_Cod_Agea,
                                                                Cul_Cod_Agea,
                                                                CDbl(SuperficieIntersezioneMacrouso),
                                                                DataValiditaInizio,
                                                                DataValiditaFine,
                                                                objParametri_Server)
                                    End If

                                End If

                            End If
                        End If

                    Next

                    If SuperficieIntersezione <> 0 Then

                        Call EttariAreCentiare_from_Ettari(SuperficieIntersezione, Ettari, Are, Centiare)

                        intDummy = ObjPartW.Scrivi(
                                        CStr(xPiva),
                                        CInt(xSa_Cod),
                                        CInt(xAppezza),
                                        CStr(particella.CodiceIstat_Provincia),
                                        CStr(particella.CodiceIstat_Comune),
                                        CStr(particella.Sezione),
                                        CInt(particella.Foglio),
                                        CInt(particella.Numero),
                                        CStr(particella.Subalterno),
                                        CDbl(SuperficieIntersezione),
                                        CDbl(Ettari),
                                        CInt(Are),
                                        CInt(Centiare),
                                        CDbl(0),
                                        CInt(0),
                                        CInt(0),
                                        CDbl(0),
                                        CInt(0),
                                        CInt(0),
                                        DataValiditaInizio,
                                        DataValiditaFine,
                                        objParametri_Server)
                        SuperficieIntersezione = 0
                    End If

                End If

            Next


            objParametri_Server.ResettaFinestra()

        End If ' end chiave <> 0

    End Sub

    Public Function GetParticelleKendo() As List(Of Particella_Anagrafica)
        Dim jss = New JavaScriptSerializer()
        Return jss.Deserialize(Of List(Of Particella_Anagrafica))(hParticelleSelezionate.Value)
    End Function

End Class