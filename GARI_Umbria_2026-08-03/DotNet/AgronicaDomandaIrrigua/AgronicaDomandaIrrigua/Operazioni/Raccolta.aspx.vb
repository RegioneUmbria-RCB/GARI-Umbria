Imports System.Web
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreGestioneRichieste
Imports AgronicaControlli_2010

Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Xml
Imports AgronicaCoreModelsSTD.attivita.Attivita


Public Class Raccolta
    Inherits System.Web.UI.Page


#Region "Init"

    Dim LottoMagazzinoSalvato As String

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda
    Dim Tipo_Raccolta As enum_RACCOLTA_TIPO
    Dim MagazzinoDefault As Boolean = True
    Public Master_Operazione As Operazione


    Private Sub Raccolta_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Session("ObjparametriAgendaProvenienzaPiva") = Nothing
        Session("ObjparametriAgendaProvenienzaSa_Cod") = Nothing
        Session("ObjparametriAgendaProvenienzaVeg_Cod") = Nothing
        Session("ObjparametriAgendaProvenienzaData") = Nothing
        Session("ObjparametriAgendaProvenienzaSa_Cod_magazzino") = Nothing
        Session("ObjparametriAgendaProvenienzafabbricato_Cod") = Nothing

        If Not IsPostBack Then
            Session("RaccoltaInModificaConOpCuraCollegata") = False
        End If

        Master_Operazione = CType(Page.Master, Operazione)

        Master_Operazione.Property_Div_ProvenienzaRisorse.Visible = False

        AddHandler Master_Operazione.Property_BTN_ChangeData.Click, AddressOf Me.aggiornaDataOperazione
        AddHandler Master_Operazione.Property_ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        AddHandler Master_Operazione.Property_BTN_Magazzini.Click, AddressOf Me.CambioMagazzino
        AddHandler Master_Operazione.Property_BTN_ComboSpecie.Click, AddressOf Me.CambioSpecie
        AddHandler Master_Operazione.Property_ImgBtn_Salva.Click, AddressOf Me.SalvaTutto
        AddHandler CType(Page.Master, MasterPage).PreRender, AddressOf Me.MasterUnload


    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim strJS As New StringBuilder
        strJS.AppendLine("$(document).ready(function () {                       ")
        strJS.AppendLine("       $('#" & txt_DataMagazzino.ClientID & "').datepicker({                    ")
        strJS.AppendLine("               dateFormat: 'dd/mm/yy',          ")
        strJS.AppendLine("               disabled: false,          ")
        strJS.AppendLine("               changeMonth: true,         ")
        strJS.AppendLine("               changeYear: true          ")
        strJS.AppendLine("          });                   ")

        strJS.AppendLine("       $('#" & txt_OraMagazzino.ClientID & "').timepicker()")
        strJS.AppendLine("       ;    ")

        strJS.AppendLine("                     ")
        strJS.AppendLine("                     ")
        strJS.AppendLine("                     ")


        strJS.AppendLine("    $('.QtaRiga').keyup(function () {                       ")
        strJS.AppendLine("        var SupTot = 0.00;                      ")
        strJS.AppendLine("        $('.QtaRiga').each(function () {                      ")
        strJS.AppendLine("          var app = $(this).val().replace(',', '.');                      ")
        strJS.AppendLine("          SupTot += parseFloat(app);                      ")
        strJS.AppendLine("      });                      ")
        strJS.AppendLine("      var Qta = SupTot + '';                      ")
        strJS.AppendLine("      Qta=Qta.replace('.', ',');                      ")
        strJS.AppendLine("     $('.QtaTotale').val(Qta);                      ")
        strJS.AppendLine("     })   ")

        strJS.AppendLine(" $('#" & tabs.ClientID & "').tabs(); ")

        strJS.AppendLine(" });")
        ScriptManager.RegisterStartupScript(Master_Operazione.Property_UpdatePanelPerScript, Master_Operazione.Property_UpdatePanelPerScript.GetType(),
                                      String.Format("jQuery_{0}", Master_Operazione.Property_UpdatePanelPerScript.ClientID), strJS.ToString, True)



        verificoCredenzialiDiAccesso()
        inizializzoObjParametri()
        inizializzoParametriPagina()


        If Not IsPostBack Then
            caricaComboLavorazione()
        End If


        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
            LeggiImpostazioni()
            Opzioni_Impianti.Visible = True
        Else
            LeggiImpostazioni()
            Opzioni_Impianti.Visible = False
            'per ora non gestita quando in base all'extraint dell'operazione che mi dice le impostazioni
            'attive l salvataggio, ma mando un alert
            'quindi vedo la pagina in base alle impostazioni e non in base a come è stata salvata
            'ImpostaVisibilita()
        End If


        If Not IsPostBack Then

            VerificaPermessi()
            caricaControlli()
            disabilitaControlli()

        Else


        End If



    End Sub

#End Region



#Region "Metodi Eseguiti nel Load"

    Private Sub verificoCredenzialiDiAccesso()
        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If
    End Sub

    Private Sub inizializzoObjParametri()
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
    End Sub

    Private Sub inizializzoParametriAgenda()
        objParametriAgenda = New ParametriAgenda
        objParametriAgenda.OperazioneMulticentro = True
    End Sub

    Private Sub inizializzoParametriPagina()

        inizializzoParametriAgenda()

        Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim Lav_Des As String = objOperazioniLeggi.LavorazioneDes_from_LavorazioneCod(objParametriAgenda.Lav_Cod, objParametri_Server)
        objParametriAgenda.Lav_Des = Lav_Des
        Master_Operazione.Property_Lbl_Titolo.Text = Lav_Des


        Select Case CInt(objParametriAgenda.Lav_Cod)

            Case LAVCOD_RACCOLTA
                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_RACCOLTA)


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

        Select Case objParametriAgenda.Tipo_Operazione
            Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                'verificare se gestire diversamente per non perdere informazioni in modifica
        End Select

        '--------------------------------------
        'leggo le eventuali IMPOSTAZIONI UTENTE
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim Tipo_Raccolta_Val As String
        Tipo_Raccolta_Val = ObjUtenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(
                                                        enum_Impostazioni_Utenti.UTENTE_COD_RACCOLTA_TIPO,
                                                         objParametri_Utenti)
        If IsNumeric(Tipo_Raccolta_Val) Then
            Tipo_Raccolta = CInt(Tipo_Raccolta_Val)
        Else
            Tipo_Raccolta = enum_RACCOLTA_TIPO.Leggera_Con_Dettagli_Magazzino
        End If

        Dim Tipologia_Prodotto_Val As String
        Tipologia_Prodotto_Val = ObjUtenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(
                                                        enum_Impostazioni_Utenti.UTENTE_COD_RACCOLTA_TIPOLOGIA_PRODOTTO,
                                                         objParametri_Utenti)

        Dim Tipologia_Prodotto As RACCOLTA_TIPOLOGIA_PRODOTTO
        If IsNumeric(Tipologia_Prodotto_Val) Then
            Tipologia_Prodotto = CInt(Tipologia_Prodotto_Val)
        Else
            Tipologia_Prodotto = RACCOLTA_TIPOLOGIA_PRODOTTO.Non_Specificata
        End If

        ImpostaVisibilita(Tipologia_Prodotto)




        'Dim Carenza As String
        'Carenza = ObjUtenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser( _
        '                                        enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_CARENZA_NON_RISPETTATA, _
        '                                      objParametri_Utenti)
        ' If Carenza = "1" Then
        'CheckBoxCarenza.Checked = True
        GridView_Rilievo.Columns(8).Visible = True
        'Else
        'CheckBoxCarenza.Checked = False
        'GridView_Rilievo.Columns(8).Visible = True
        'End If



        If Not IsPostBack Then
            Dim Chk_ApriImpianto_Val As String = ""
            Chk_ApriImpianto_Val = ObjUtenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(
                                                            enum_Impostazioni_Utenti.UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO,
                                                             objParametri_Utenti)

            Chk_NuovoEsercizio.Checked = False
            Chk_NuovoImpianto.Checked = False

            If String.IsNullOrEmpty(Chk_ApriImpianto_Val) Then
                'In caso l'utente non abbiamo mai salvato le impostizoni -- imposto il default = 1 
                'Modalità default su apertura nuovi impianti/esercizi in caso di scelta dell'opzione di chiusura di quelli raccolti
                Chk_ApriImpianto_Val = enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.OLD_APRI_NUOVI_ESERCIZI_NUOVI_IMPIANTI
            End If

            Select Case CInt(Chk_ApriImpianto_Val)
                    Case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.OLD_APRI_NUOVI_ESERCIZI_NUOVI_IMPIANTI
                        Chk_NuovoEsercizio.Checked = True
                        Chk_NuovoImpianto.Checked = True
                    Case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.OLD_NON_APRIRE_NUOVI_ESERCIZI_NUOVI_IMPIANTI
                        Chk_NuovoEsercizio.Checked = False
                        Chk_NuovoImpianto.Checked = False
                    Case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_ESERCIZI_DEFAULT,
                     enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_ESERCIZI_VINCOLO
                        Chk_NuovoEsercizio.Checked = True
                        Chk_NuovoImpianto.Checked = False
                    Case enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_IMPIANTI_ESERCIZI_DEFAULT,
                    enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO.CHIUDI_APRI_IMPIANTI_ESERCIZI_VINCOLO
                        Chk_NuovoEsercizio.Checked = False
                        Chk_NuovoImpianto.Checked = True
                End Select
            Else
            End If


        'Impostazione fissa, tipologia pagina etc




    End Sub

    Private Sub ImpostaVisibilita(ByVal Tipologia_Prodotto As RACCOLTA_TIPOLOGIA_PRODOTTO)
        Select Case Tipo_Raccolta
            Case enum_RACCOLTA_TIPO.Fast
                'senza opzioni
                DivOpzioni.Visible = False

                'senza magazzino e data magazzino
                DivMagazzino.Visible = False

                'senza quantita e senza indicare prodotto
                DivProdotto.Visible = False

            Case enum_RACCOLTA_TIPO.Leggera, enum_RACCOLTA_TIPO.Leggera_Con_Dettagli_Magazzino
                'senza opzioni
                DivOpzioni.Visible = True

                'Con magazzino ma senza data magazzino e lotto
                If Tipo_Raccolta = enum_RACCOLTA_TIPO.Leggera Then
                    DivMagazzino.Visible = False
                Else
                    DivMagazzino.Visible = True
                    MagazzinoDefault = True
                End If


                'con quantita e prodotto
                DivProdotto.Visible = True

            Case enum_RACCOLTA_TIPO.Standard
                MagazzinoDefault = True

            Case enum_RACCOLTA_TIPO.Raccolta_e_Cura
                MagazzinoDefault = True


            Case Else
                Throw New Exception("Tipo raccolta non codificata in enum_RACCOLTA_TIPO")
        End Select


        Select Case Tipologia_Prodotto
            Case RACCOLTA_TIPOLOGIA_PRODOTTO.Non_Specificata
                'CheckBoxSemilavorati lascio libero
                CheckBoxSemilavorati.Visible = True
            Case RACCOLTA_TIPOLOGIA_PRODOTTO.Default_Su_Semilavorati
                'default, quindi solo alla richiesta
                If Not IsPostBack Then
                    CheckBoxSemilavorati.Checked = True
                    CheckBoxSemilavorati.Visible = True
                End If
            Case RACCOLTA_TIPOLOGIA_PRODOTTO.Default_Su_Trasormati
                'default, quindi solo alla richiesta
                If Not IsPostBack Then
                    CheckBoxSemilavorati.Checked = False
                    CheckBoxSemilavorati.Visible = True
                End If
            Case RACCOLTA_TIPOLOGIA_PRODOTTO.Fissa_Su_Semilavorati
                CheckBoxSemilavorati.Checked = True
                CheckBoxSemilavorati.Visible = False
            Case RACCOLTA_TIPOLOGIA_PRODOTTO.Fissa_Su_Trasormati
                CheckBoxSemilavorati.Checked = False
                CheckBoxSemilavorati.Visible = False
        End Select



    End Sub

    Private Sub caricaControlli()

        CaricaComboMagazzini()
        'in base alle impostazioni utente raccolta:
        If MagazzinoDefault Then
            If ComboMagazzinoDestinazione.ddl_Magazzini.Items.Count > 1 Then
                ComboMagazzinoDestinazione.ddl_Magazzini.SelectedIndex = 1
            End If
        End If

        caricaUdm()

        'in base alle impostazioni utente raccolta:
        'il tabacco funziona come la standard
        'in più metto udm a numero e valore a uno
        If Tipo_Raccolta = enum_RACCOLTA_TIPO.Raccolta_e_Cura Then
            If CmbUdm.Items.Count > 0 Then
                CmbUdm.SelectedValue = enum_UnitaMisura.Numero
            End If
            QtaTot.Text = "1"
            'Attenzione, devo usare i trasformati, perchè con i semilavorati
            'anche se mi sarebbero comodi perchè rintracciano dall'impianto
            'ho il problema che l'azienda uds non riesce ad associare il cod progetto nella query delle giacenze non essendo un suo impianto
            CheckBoxSemilavorati.Checked = False
            CheckBoxSemilavorati.Visible = False
            'metto il lotto come generato e univoco
            RadioButtonNomeLotto.SelectedValue = "2"
            'preparo per la la lavorazione post raccolta
            ComboLavorazione.SelectedValue = 1
            CambiataLavorazione()

        End If

        If Tipo_Raccolta <> enum_RACCOLTA_TIPO.Fast Then
            caricaMateriePrime()
            caricaNomeLotto()
        End If

        Dim data As String = objParametriAgenda.Data.ToShortDateString
        txt_DataMagazzino.Text = data


        Select Case objParametriAgenda.Tipo_Operazione

            Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura


            Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica
                RipristinaControlliDaAgenda()
                Master_Operazione.CaricaCostiAccessori()
            Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura
                RipristinaControlliDaAgenda()
                Master_Operazione.CaricaCostiAccessori()

        End Select



    End Sub

#End Region



#Region "EventiControlli"

    Private Sub CaricaComboMagazzini()


        'ComboMagazzini.Sa_Cod = objParametriAgenda.Sa_Cod
        ComboMagazzinoDestinazione.Piva = objParametriAgenda.Piva
        ComboMagazzinoDestinazione.Flag_CodCentroFabbricato = True
        ComboMagazzinoDestinazione.TipoMagazzino = MAGAZZINO
        'If objParametriAgenda.Lav_Cod = LAVCOD_TRAPIANTO Or objParametriAgenda.Lav_Cod = LAVCOD_SEMINA Or objParametriAgenda.Lav_Cod = LAVCOD_SOVESCIO Then
        '    ComboMagazzinoDestinazione.Flag_GestioneMagazziniImpresaPadre = True
        'End If
        ComboMagazzinoDestinazione.CaricaComboMagazzini()
        ''imposto il valore
        'If ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue <> "0" AndAlso Split(ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue, "|").Count = 3 Then
        '    ComboMagazzinoDestinazione.Valore_Combo = ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue
        '    ComboMagazzinoDestinazione.ddl_Magazzini.SelectedIndex = ComboMagazzinoDestinazione.ddl_Magazzini.Items.IndexOf(ComboMagazzinoDestinazione.ddl_Magazzini.Items.FindByValue(ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue))
        'Else
        '    ComboMagazzinoDestinazione.Valore_Combo = ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue & "|" & objParametriAgenda.Piva
        '    ComboMagazzinoDestinazione.ddl_Magazzini.SelectedIndex = ComboMagazzinoDestinazione.ddl_Magazzini.Items.IndexOf(ComboMagazzinoDestinazione.ddl_Magazzini.Items.FindByValue(ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue & "|" & objParametriAgenda.Piva))
        'End If


    End Sub

    Private Sub caricaUdm()
        Dim Elem_Cod As Integer = TRASFORMATI_VEGETALI
        If CheckBoxSemilavorati.Checked Then
            Elem_Cod = SEMILAVORATI_VEGETALI
        End If

        'carico con categoria 201, verificare trasformati 
        CaricaListControl.Udm_Optimize(CmbUdm, 0, "",
                                     0,
                                     0,
                                     CAU_CARICO,
                                     Elem_Cod,
                                     False,
                                     0,
                                     0,
                                     False, "", "", "", "", "", objParametri_Server, objParametri_Utenti)
    End Sub

    Private Sub caricaMateriePrime()
        Dim Elem_Cod As Integer = TRASFORMATI_VEGETALI
        If CheckBoxSemilavorati.Checked Then
            Elem_Cod = SEMILAVORATI_VEGETALI
        End If

        If Elem_Cod = TRASFORMATI_VEGETALI Then
            Label11.Text = "Prodotto (Trasformato)"
        End If
        If Elem_Cod = SEMILAVORATI_VEGETALI Then
            Label11.Text = "Prodotto (Semilavorato)"
        End If

        'carico i trasformati o semilavorati se cambio check
        Dim clc = New AgronicaCoreUtility.CaricaListControl
        clc.Materie_Prime(cmb_Prodotto, False, "", "", CAU_CARICO, objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, 0, Elem_Cod, False, "", "", "", "", 0, 0, 0, 0, 0, "", objParametriAgenda.Veg_Cod.Split("/")(0), 0, 0, 0, 0, 0, 0, objParametriAgenda.Data, "", "", objParametri_Server, objParametri_Utenti)
        If cmb_Prodotto.Items.Count = 0 Then
            cmb_Prodotto.Items.Add(New ListItem("Genera prodotto per la specie (varietà altre)", "-1"))
        Else
            'seleziono 
        End If

        If Tipo_Raccolta = enum_RACCOLTA_TIPO.Raccolta_e_Cura Then
            If Not IsNothing(cmb_Prodotto.Items.FindByText("Tabacco - Altre (Cod.Articolo: RACC/335/05010411)")) Then
                cmb_Prodotto.SelectedValue = cmb_Prodotto.Items.FindByText("Tabacco - Altre (Cod.Articolo: RACC/335/05010411)").Value
            End If
        End If

    End Sub

    Private Sub caricaNomeLotto()


        If RadioButtonNomeLotto.SelectedValue = "1" Then
            TextBoxLotto.Text = "Generato: 'aaammgg'"
            TextBoxLotto.Enabled = False
        ElseIf RadioButtonNomeLotto.SelectedValue = "2" Then
            TextBoxLotto.Text = "Generato Univoco"
            TextBoxLotto.Enabled = False
        Else
            TextBoxLotto.Text = ""
            TextBoxLotto.Enabled = True
        End If
    End Sub

    Protected Sub CheckBoxSemilavorati_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxSemilavorati.CheckedChanged
        If Tipo_Raccolta <> enum_RACCOLTA_TIPO.Fast Then
            caricaMateriePrime()
            caricaUdm()
        End If
    End Sub

    Private Sub CheckBoxNomeLotto_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonNomeLotto.SelectedIndexChanged
        caricaNomeLotto()
    End Sub

    Private Sub CambioSpecie()
        If Tipo_Raccolta <> enum_RACCOLTA_TIPO.Fast Then
            caricaMateriePrime()
        End If
    End Sub

    Protected Sub RadioButtonListRipartizione_SelectedIndexChanged(sender As Object, e As EventArgs) Handles RadioButtonListRipartizione.SelectedIndexChanged

        Select Case RadioButtonListRipartizione.SelectedValue
            Case "1" 'Sup
                cancellaTabellaTaccoltaManuale()
                QtaTot.Enabled = True
            Case "2" 'piante
                cancellaTabellaTaccoltaManuale()
                QtaTot.Enabled = True
            Case "3" 'manuale
                generaTabellaTaccoltaManuale()
                distribuisci(True)
                QtaTot.Enabled = False
        End Select

    End Sub

    Private Sub cancellaTabellaTaccoltaManuale()
        GridView_Rilievo.DataSource = New DataTable
        GridView_Rilievo.DataBind()
        Sblocca()
    End Sub

    Private Sub generaTabellaTaccoltaManuale()
        '---------------------------------------
        ' recupero gli IMPIANTI
        Dim ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
        ListaImpianti = CType(Master, Operazione).GetImpianti()
        If ListaImpianti.Count = 0 Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareAlmenoUnImpiantoColturale, Page, , Master_Operazione.Property_UpdatePanelToolBar)
            RadioButtonListRipartizione.SelectedValue = "1"
            Exit Sub
        End If

        'ricavo i valori avversita, fasi etc
        Dim Veg_Cod_Val As String = Master_Operazione.Property_ComboSpecie.ddl_Specie.SelectedValue
        If Veg_Cod_Val = "" OrElse Veg_Cod_Val = "0" OrElse Veg_Cod_Val = "-1" OrElse Not IsNumeric(Split(Veg_Cod_Val, "/")(0)) Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnaSpecieVegetale, Page, , Master_Operazione.Property_UpdatePanelToolBar)
            RadioButtonListRipartizione.SelectedValue = "1"
            Exit Sub
        End If
        objParametriAgenda.Veg_Cod = CInt(Split(Veg_Cod_Val, "/")(0))

        'GridView_Rilievo
        CreoTabellaRilievoDaImpianti(ListaImpianti)

        Blocca(True, True, True, True)

    End Sub

    Private Sub CreoTabellaRilievoDaImpianti(ByVal ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto))

        Dim dt As DataTable = InizializzaGrigliaRilievo()

        Dim i As Integer = 0

        Dim ArrayPiva(0) As String
        Dim ArraySaCod(0) As Integer
        Dim ArrayAppezza(0) As Integer
        Dim ArrayIdReg(0) As Integer
        Dim N_Imp As Integer = 0


        For i = 0 To ListaImpianti.Count - 1
            ReDim Preserve ArrayPiva(N_Imp)
            ReDim Preserve ArraySaCod(N_Imp)
            ReDim Preserve ArrayAppezza(N_Imp)
            ReDim Preserve ArrayIdReg(N_Imp)

            ArrayPiva(N_Imp) = ListaImpianti(i).Piva
            ArraySaCod(N_Imp) = ListaImpianti(i).Sa_Cod
            ArrayAppezza(N_Imp) = ListaImpianti(i).Appezza
            ArrayIdReg(N_Imp) = ListaImpianti(i).ID_Reg
            N_Imp += 1
        Next

        Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim DtImp As DataTable
        Dim DrImp() As DataRow

        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(CDate(objParametriAgenda.Data), CDate(objParametriAgenda.Data))
        DtImp = objImp.Leggi_Dati_Impianti_Distinte(ArrayPiva,
                                            ArraySaCod,
                                            ArrayAppezza,
                                            ArrayIdReg,
                                            "", "",
                                            objParametri_Server)
        objParametri_Server.ResettaFinestra()

        For i = 0 To ListaImpianti.Count - 1

            Dim row As DataRow = dt.NewRow

            row.Item("Piva") = ListaImpianti(i).Piva
            row.Item("Sa_Cod") = ListaImpianti(i).Sa_Cod
            row.Item("Appezza") = ListaImpianti(i).Appezza
            row.Item("ID_Reg") = ListaImpianti(i).ID_Reg
            row.Item("Progetto_Cod") = ListaImpianti(i).Progetto_Cod

            row.Item("Rag_Soc") = ""
            row.Item("Sa_nome") = ""
            row.Item("App_nome") = ListaImpianti(i).App_Nome
            row.Item("Veg_Cod") = 0
            row.Item("Veg_Des") = ""
            row.Item("Cul_Cod") = 0
            row.Item("Cul_Des") = ""
            row.Item("Grfi_Cod") = 0
            row.Item("Cop_Cod") = 0

            If DtImp IsNot Nothing AndAlso DtImp.Rows.Count > 0 Then

                DrImp = DtImp.Select(" Piva='" & Agro_SQL_SaveText(ListaImpianti(i).Piva) & "'" &
                                    " AND Sa_Cod=" & Agro_SQL_SaveNum(ListaImpianti(i).Sa_Cod) &
                                        " AND Appezza=" & Agro_SQL_SaveNum(ListaImpianti(i).Appezza) &
                                        " AND Id_Reg=" & Agro_SQL_SaveNum(ListaImpianti(i).ID_Reg) & "")

                If DrImp IsNot Nothing AndAlso DrImp.Length > 0 Then

                    row.Item("Sa_nome") = DrImp(0).Item("sa_nome")
                    If ListaImpianti(i).App_Nome = "" Then
                        row.Item("App_nome") = DrImp(0).Item("app_nome")
                    End If
                    row.Item("Grfi_Cod") = DrImp(0).Item("grfi_cod")
                    row.Item("Cop_Cod") = DrImp(0).Item("cop_cod")

                    row.Item("P_Ha") = DrImp(0).Item("P_Ha")
                    row.Item("Progetto_Cod") = DrImp(0).Item("Progetto_Cod") 'Sovrascrivo la colonna perché nell'oggetto ListaImpianti potrebbe essere valorizzata a zero, mentre è sempre ricavata da DtImp
                End If

            End If

            If IsNumeric(ListaImpianti(i).Qta2) Then
                row.Item("Sup") = ListaImpianti(i).Qta2
            Else
                row.Item("Sup") = ListaImpianti(i).Sup_Imp
            End If

            If IsNumeric(row.Item("P_Ha")) Then
                row.Item("P_Tot") = CInt(row.Item("P_Ha") * row.Item("Sup"))
            End If

            row.Item("Descrizione") = row.Item("App_nome")

            dt.Rows.Add(row)
        Next

        finalizzaGrigliaRilievo(dt)

    End Sub

    Private Function InizializzaGrigliaRilievo() As DataTable

        Dim Dt As New DataTable("Irrigazioni")

        Dt.Columns.Add("Piva", GetType(String))
        Dt.Columns.Add("Rag_Soc", GetType(String))
        Dt.Columns.Add("Sa_Cod", GetType(Integer))
        Dt.Columns.Add("Sa_nome", GetType(String))
        Dt.Columns.Add("Appezza", GetType(Integer))
        Dt.Columns.Add("ID_Reg", GetType(Integer))
        Dt.Columns.Add("Progetto_Cod", GetType(Integer))
        Dt.Columns.Add("App_nome", GetType(String))
        Dt.Columns.Add("Veg_Cod", GetType(Integer))
        Dt.Columns.Add("Veg_Des", GetType(String))
        Dt.Columns.Add("Cul_Cod", GetType(Integer))
        Dt.Columns.Add("Cul_Des", GetType(String))
        Dt.Columns.Add("Sup", GetType(Decimal))
        Dt.Columns.Add("P_Ha", GetType(Decimal))
        Dt.Columns.Add("P_Tot", GetType(Decimal))
        Dt.Columns.Add("Descrizione", GetType(String))
        Dt.Columns.Add("Grfi_Cod", GetType(Integer))
        Dt.Columns.Add("Cop_Cod", GetType(Integer))

        Return Dt

    End Function

    Private Sub finalizzaGrigliaRilievo(Dt As DataTable)

        Dim DtKeys(11) As String

        DtKeys(0) = "Piva"
        DtKeys(1) = "Sa_Cod"
        DtKeys(2) = "Appezza"
        DtKeys(3) = "ID_Reg"
        DtKeys(4) = "Progetto_Cod"
        DtKeys(5) = "Veg_Cod"
        DtKeys(6) = "Cul_Cod"
        DtKeys(7) = "Sup"
        DtKeys(8) = "P_Ha"
        DtKeys(9) = "P_Tot"
        DtKeys(10) = "Grfi_Cod"
        DtKeys(11) = "Cop_Cod"

        GridView_Rilievo.DataKeyNames = DtKeys
        GridView_Rilievo.DataSource = Dt
        GridView_Rilievo.DataBind()

        verificaCarenza()

    End Sub

    Private Sub distribuisci(ByVal SupPiante As Boolean)
        If Not IsNumeric(QtaTot.Text) OrElse (IsNumeric(QtaTot.Text) AndAlso CDbl(QtaTot.Text) < 0) Then
            Dim msg As String = "Valore non valido per la quantità, inserire un valore maggiore di 0"
            Messaggi.AgroMsgBuonFine(msg, Page, ,
                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Exit Sub
        End If

        Dim Totale As Decimal = QtaTot.Text

        Dim Suptot As Decimal = 0
        For i = 0 To GridView_Rilievo.Rows.Count - 1
            If SupPiante Then
                Suptot = Suptot + GridView_Rilievo.DataKeys(i).Item("Sup")
            Else
                Suptot = Suptot + GridView_Rilievo.DataKeys(i).Item("P_Tot")
            End If
        Next

        Dim count As Decimal = 0

        For i = 0 To GridView_Rilievo.Rows.Count - 1
            Dim Sup As Decimal = 0
            If SupPiante Then
                Sup = GridView_Rilievo.DataKeys(i).Item("Sup")
            Else
                Sup = GridView_Rilievo.DataKeys(i).Item("P_Tot")
            End If

            Dim dose As Decimal
            If i < GridView_Rilievo.Rows.Count - 1 Then
                dose = Agro_Math.ArrotondaVal_2(Totale / Suptot * Sup)
                count = count + dose
            Else
                dose = Totale - count
            End If

            CType(GridView_Rilievo.Rows(i).Cells(7).Controls(1), TextBox).Text = CStr(dose)

        Next

    End Sub

#End Region



#Region "RipristinoValori"

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
                    Case LAVCOD_RACCOLTA

                    Case Else
                        Throw New NotImplementedException
                End Select


            Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica

                Master_Operazione.Property_Box_Salva.Enabled = True 'true per modifica
                Master_Operazione.Property_Box_Salva.Visible = True 'true per modifica
                Master_Operazione.Property_ImgBtn_Salva.Enabled = True 'true per modifica
                Master_Operazione.Property_RBL_Salva.Enabled = True
                Master_Operazione.Property_RBL_Salva.Items(1).Enabled = False
                Master_Operazione.Property_RBL_Salva.Items(2).Enabled = False
                Master_Operazione.flag_MostraBtnSalvaCDG = True

                Master_Operazione.Property_txt_DataOperazione.Enabled = False


                Master_Operazione.Property_txt_Note.Enabled = True 'true per modifica

                Master_Operazione.Property_ComboSpecie.Enabled = False
                Master_Operazione.Property_BTN_ComboSpecie.Enabled = False


                ComboMagazzinoDestinazione.Enabled = False

                Master_Operazione.Property_BTN_CentroAziendale.Enabled = False
                Master_Operazione.Property_ComboCentroAziendale.Enabled = False

                Master_Operazione.Property_BTN_ComboOperazione.Enabled = False
                Master_Operazione.Property_ComboOperazione.Enabled = False

                Master_Operazione.Property_CBL_Consigli.Enabled = True

                Master_Operazione.Property_GridView_Impianti.Enabled = False

                Blocca(True, True, True, True)



                'impostazioni in base alla lavorazione
                Select Case CInt(objParametriAgenda.Lav_Cod)
                    Case LAVCOD_RACCOLTA

                    Case Else
                        Throw New NotImplementedException
                End Select

            Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura



                Master_Operazione.Property_Box_Salva.Enabled = False 'true per modifica
                Master_Operazione.Property_Box_Salva.Visible = False 'true per modifica
                Master_Operazione.Property_ImgBtn_Salva.Enabled = False 'true per modifica
                Master_Operazione.Property_RBL_Salva.Enabled = False 'true per modifica

                Master_Operazione.Property_txt_DataOperazione.Enabled = False

                Master_Operazione.Property_txt_Note.Enabled = False 'true per modifica

                Master_Operazione.Property_ComboSpecie.Enabled = False
                Master_Operazione.Property_BTN_ComboSpecie.Enabled = False

                ComboMagazzinoDestinazione.Enabled = False

                Master_Operazione.Property_BTN_CentroAziendale.Enabled = False
                Master_Operazione.Property_ComboCentroAziendale.Enabled = False

                Master_Operazione.Property_BTN_ComboOperazione.Enabled = False
                Master_Operazione.Property_ComboOperazione.Enabled = False

                Master_Operazione.Property_CBL_Consigli.Enabled = False

                Master_Operazione.Property_Box_Salva.Visible = False

                Master_Operazione.Property_GridView_Impianti.Enabled = False

                Blocca(True, True, True, True)

                'impostazioni in base alla lavorazione
                Select Case CInt(objParametriAgenda.Lav_Cod)
                    Case LAVCOD_RACCOLTA

                    Case Else
                        Throw New NotImplementedException
                End Select

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

            RipristinaRaccolta(Agenda)

            'Gestione cura e op riferite
            RipristinaDaRiferimenti(Agenda)

        End If

    End Sub

    Private Sub RipristinaDaRiferimenti(ByVal Agenda As Operazione_Agenda)
        Dim x As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
        Dim dtrifCarico As DataTable = x.Leggi_Specifica("", 0, 0, 0, 0, LAVCOD_SCARICO, "",
                                                         objParametriAgenda.Piva,
                                                         objParametriAgenda.Sa_Cod,
                                                         objParametriAgenda.Id_Agenda,
                                                         -1, -1, LAVCOD_RACCOLTA, "",
                                                         "", "", objParametri_Server)
        Dim dtrifScarico As DataTable = x.Leggi_Specifica("", 0, 0, 0, 0, LAVCOD_CARICO, "",
                                                        objParametriAgenda.Piva,
                                                        objParametriAgenda.Sa_Cod,
                                                        objParametriAgenda.Id_Agenda,
                                                        -1, -1, LAVCOD_RACCOLTA, "",
                                                        "", "", objParametri_Server)

        If dtrifCarico.Rows.Count <> dtrifScarico.Rows.Count Then
            Throw New Exception("Dovrebbe essere uguali")
        End If
        If dtrifCarico.Rows.Count <> 0 Then
            If dtrifCarico.Rows.Count = 1 Then

                ''Vengo dalla raccolta quindi ho i carichi e scarichi collegati
                'Dim Id_Agenda_Carico As Integer = dtrifCarico.Rows(0).Item("Id_Agenda")
                'Dim Id_Agenda_Scarico As Integer = dtrifScarico.Rows(0).Item("Id_Agenda")

                'devo ripristinare i valori in sessione
                Dim objAgenda As New Agenda_Operazione_Helper
                Dim AgendaScarico As New Operazione_Agenda
                Dim AgendaCarico As New Operazione_Agenda
                AgendaCarico = objAgenda.Leggi(dtrifCarico.Rows(0).Item("Piva"),
                                             CInt(dtrifCarico.Rows(0).Item("Sa_Cod")),
                                             CInt(dtrifCarico.Rows(0).Item("Id_Agenda")),
                                             0,
                                             objParametri_Server)
                AgendaScarico = objAgenda.Leggi(dtrifScarico.Rows(0).Item("Piva"),
                                             CInt(dtrifScarico.Rows(0).Item("Sa_Cod")),
                                             CInt(dtrifScarico.Rows(0).Item("Id_Agenda")),
                                             0,
                                             objParametri_Server)

                If IsNothing(AgendaScarico) OrElse IsNothing(AgendaCarico) Then
                    Throw New Exception("l'operazione collegata non esiste")
                End If

                ComboLavorazione.SelectedValue = 1
                CambiataLavorazione()

                ComboUDS.SelectedValue = AgendaCarico.Movimenti(0).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Piva
                ComboCentroCura.SelectedValue = AgendaCarico.Movimenti(0).Movimenti_Dettagli(0).Movimenti_Destinazioni(0).Sa_Cod

                Session("RaccoltaInModificaConOpCuraCollegata") = True
            Else
                Throw New Exception("Dovrebbe essercene due o nessuno")
            End If
        End If
    End Sub

    Private Sub RipristinaRaccolta(ByVal Agenda As Operazione_Agenda)
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

            Dim MovimentiCosti As List(Of Movimento) = New List(Of Movimento)
            objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

            For i = 0 To Agenda.Movimenti.Count - 1

                'controllo corrispondeza Piva con agenda
                If Agenda.Piva <> Agenda.Movimenti(i).Piva Then
                    Throw New ApplicationException
                End If

                Select Case Agenda.Movimenti(i).Cau_Mov

                    Case enum_Agenda_Causali.RILIEVO_RACCOLTA

                        If Agenda.Lav_Cod <> objParametriAgenda.Lav_Cod Then
                            'controllino per lo sviluppo, da togliere
                            Throw New NotImplementedException
                        End If

                        Select Case CInt(objParametriAgenda.Lav_Cod)
                            Case LAVCOD_RACCOLTA
                                LeggiMovimentoAgenda_Raccolta(Agenda, i)
                            Case Else
                                Throw New NotImplementedException
                        End Select

                    Case enum_Agenda_Causali.CARICO
                        'carico prodotti

                        'MOVIMENTI_DETTAGLI
                        If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                            'If Agenda.Movimenti(i).Movimenti_Dettagli.Count > 1 Then
                            '    'solo un prodotto
                            '    Throw New Exception("Per ora è gestito un solo prodotto")
                            'End If

                            For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                'controllo corrispondeza  Piva con agenda, Sa_Cod potrebbe cambiare per chi ha fabbricato in altro centro
                                If Agenda.Piva <> Agenda.Movimenti(i).Movimenti_Dettagli(j).Piva Then
                                    Throw New ApplicationException
                                End If

                                Select Case Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod
                                    Case TRASFORMATI_VEGETALI, SEMILAVORATI_VEGETALI
                                        'ok ma per ora gestito un solo prodotto
                                        LeggiMovimentoAgendaCarico(Agenda, Agenda.Movimenti(i))
                                    Case Else
                                        'non previsto
                                        Throw New Exception("Tipologia non prevista")
                                End Select
                            Next

                        End If


                        '----COSTO ACCESSORIO---------------------
                    Case enum_Agenda_Causali.SCARICO
                        'se c'è è costo accessorio
                        MovimentiCosti.Add(Agenda.Movimenti(i))


                    Case CAU_IMPUTAZIONE_PARCOMACCHINE
                        MovimentiCosti.Add(Agenda.Movimenti(i))

                    Case CAU_IMPUTAZIONE_MANODOPERA
                        MovimentiCosti.Add(Agenda.Movimenti(i))

                    Case CAU_IMPUTAZIONE_TERZISTI
                        MovimentiCosti.Add(Agenda.Movimenti(i))

                    Case CAU_IMPUTAZIONE_UTILIZZO_PRODOTTI
                        MovimentiCosti.Add(Agenda.Movimenti(i))

                    Case CAU_IMPUTAZIONE_TECNICO_RESPONSABILE
                        MovimentiCosti.Add(Agenda.Movimenti(i))

                    Case Else
                        Throw New NotImplementedException

                End Select
            Next

            objParametriAgenda.Movimenti = MovimentiCosti

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
        Dim SupImp As Decimal = 0
        For Each imp As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In objParametriAgenda.Impianti
            SupImp += ImpUtil.LeggiSuperficie(Agenda.Piva, Agenda.Sa_Cod, imp.Appezza, imp.ID_Reg, objParametri_Server)
        Next
        Txt_SupSelezionata.Value = CStr(SupImp)

    End Sub

    Private Sub LeggiMovimentoAgendaCarico(Agenda As Operazione_Agenda, movimento As Movimento)
        Dim Destinazione As Integer
        Dim SaCodDestinazione As Integer


        'se con trasformati un dettaglio e una destinazione,
        'se con semilavorati un dettaglio e una destinazione per ciascun impianto
        ', con il codice progetto  nel Cod_Progetto del dettaglio

        'MOVIMENTO_DETTAGLIO TECNICO (dovrebbe essere nothing)
        If Not IsNothing(movimento.Movimenti_Dettagli_Tecnici) Then
            For j = 0 To movimento.Movimenti_Dettagli_Tecnici.Count - 1
                'controllo corrispondeza Piva con agenda
                If Agenda.Piva <> movimento.Piva Then
                    Throw New ApplicationException
                End If
                'non dovrebbe essercene nessuno
                Throw New NotImplementedException
            Next
        End If
        If Not IsNothing(movimento.Movimenti_Dettagli(0).Movimenti_Destinazioni) Then
            For x = 0 To movimento.Movimenti_Dettagli(0).Movimenti_Destinazioni.Count - 1

                Destinazione = movimento.Movimenti_Dettagli(0).Movimenti_Destinazioni(x).Id_Destinazione
                SaCodDestinazione = movimento.Movimenti_Dettagli(0).Movimenti_Destinazioni(x).Sa_Cod
                ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue = Destinazione.ToString & "|" & SaCodDestinazione.ToString & "|" & Agenda.Piva
            Next
        Else
            Throw New Exception("IsNothing(movimento.Movimenti_Dettagli(0).Movimenti_Destinazioni")
        End If

        Dim generalizzata As Boolean = False
        If movimento.Movimenti_Dettagli(0).Elem_Cod = TRASFORMATI_VEGETALI Then
            generalizzata = True
        ElseIf movimento.Movimenti_Dettagli(0).Elem_Cod = SEMILAVORATI_VEGETALI Then
            generalizzata = False
        Else
            Throw New NotImplementedException
        End If


        txt_DataMagazzino.Text = movimento.Data.ToShortDateString
        txt_OraMagazzino.Text = movimento.Ora.ToShortTimeString


        'non occorre leggere altri dati

        'If generalizzata Then
        '    'un movimento dettaglio e una destinazione (trasformati)

        'Else
        '    '
        '    'un movimento dettaglio e una destinazione per ciascun impianto (semilavorati)


        '    Next

    End Sub

    Private Sub LeggiMovimentoAgenda_Raccolta(ByRef agenda As Operazione_Agenda, ByVal i As Integer)


        objParametriAgenda.Data = agenda.Movimenti(i).Data
        Master_Operazione.SetNota(agenda.Movimenti(i).Mov_Desc)

        Dim Modalita As Integer = agenda.Movimenti(i).Modalita()
        Dim Tipo_Raccolta_Salvata As enum_RACCOLTA_TIPO = agenda.Movimenti(i).Extra_Int  'salvo il tipo raccolta, puo essere utile
        Dim Elem_Cod As Integer
        Dim Mat_Cod As Integer
        Dim Udm_Cod As Integer
        Dim Qta As Integer
        ' Dim Cod_Progetto As Integer = agenda.Movimenti(i).Movimenti_Dettagli(j).Cod_Progetto
        Dim Cal_Cod As Integer
        Dim Lotto As String

        'MOVIMENTO_DETTAGLIO TECNICO (dovrebbe essere nothing)
        If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then
            For j = 0 To agenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                'controllo corrispondeza Piva con agenda
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
        'uno per ciascun prodotto, quindi uno solo
        If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli) Then

            If agenda.Movimenti(i).Movimenti_Dettagli.Count <> 1 Then
                Throw New Exception("Ci deve essere un solo dettaglio per un solo prodotto al momento")
            End If

            For j = 0 To agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                'controllo corrispondeza Piva con agenda
                'sa cod è uguale ma potrebbe cambiare se...
                If agenda.Piva <> agenda.Movimenti(i).Movimenti_Dettagli(j).Piva Then
                    Throw New ApplicationException
                End If

                'non ci deve essere nessun dettaglio tecnico
                If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count <> 0 Then
                    Throw New ApplicationException
                End If

                'ci deve essere almeno un movimento destinazione, uno per ciascun Appezzamento 
                If IsNothing(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
                    Throw New ApplicationException
                End If



                Elem_Cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod
                Mat_Cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Mat_Cod
                Udm_Cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod
                Qta = agenda.Movimenti(i).Movimenti_Dettagli(j).Qta
                'Cod_Progetto  = agenda.Movimenti(i).Movimenti_Dettagli(j).Cod_Progetto
                Cal_Cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Cal_Cod
                Lotto = agenda.Movimenti(i).Movimenti_Dettagli(j).Lotto

                'una destinazione per ciascun impianto
                Dim idnudo As Integer = 0
                Dim idnudoold As Integer = 0
                Dim cambiatonudo As Boolean = False

                For r = 0 To agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1



                    Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                    objAppezzamento.Piva = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Piva
                    objAppezzamento.Sa_Cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Sa_Cod
                    objAppezzamento.Appezza = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Appezza
                    objAppezzamento.ID_Reg = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Id_Destinazione
                    objAppezzamento.Qta = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Qta

                    objAppezzamento.Qta2 = CStr(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Qta2)

                    Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                    Dim Dt_Imp As New DataTable
                    Dt_Imp = objImp.Leggi(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Piva,
                                 agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Sa_Cod,
                                 agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Appezza,
                                 agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Id_Destinazione,
                                 enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                 "", "", objParametri_Server)

                    If Dt_Imp.Rows.Count > 0 Then

                        'objParametriAgenda.Veg_Cod = CInt(Dt_Imp.Rows(0).Item("Veg_Cod"))

                        If r = 0 Then
                            objParametriAgenda.Veg_Cod = CInt(Dt_Imp.Rows(0).Item("veg_cod"))
                        End If

                        If objParametriAgenda.Veg_Cod = 0 AndAlso CInt(Dt_Imp.Rows(0).Item("id_cod")) <> 0 Then
                            If r = 0 Then
                                idnudo = CInt(Dt_Imp.Rows(0).Item("id_cod"))
                                idnudoold = idnudo
                            Else
                                idnudo = CInt(Dt_Imp.Rows(0).Item("id_cod"))
                                If idnudoold <> idnudo Then
                                    cambiatonudo = True
                                End If
                            End If
                            If r = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1 Then
                                If cambiatonudo Then
                                    objParametriAgenda.Veg_Cod = "0"
                                Else
                                    objParametriAgenda.Veg_Cod = "0/" & idnudo
                                End If
                            End If
                        End If


                    End If

                    objAppezzamento.Veg_Cod = CInt(Dt_Imp.Rows(0).Item("Veg_Cod"))
                    objAppezzamento.Cul_Cod = CInt(Dt_Imp.Rows(0).Item("Cul_Cod"))
                    objAppezzamento.Cul_Des = Dt_Imp.Rows(0).Item("Cul_Des") ' (New AgronicaCoreMetaSchemaDAL.Cultivar_R()).CulDes_from_CulCod(objAppezzamento.Cul_Cod, objParametri_Server)
                    objAppezzamento.Sup_Imp = CDbl(Dt_Imp.Rows(0).Item("Sup_Imp"))

                    If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(r).Qta2 = 0 Then
                        objAppezzamento.Qta2 = CStr(objAppezzamento.Sup_Imp)
                    End If


                    'aggiungo impioanto a parameteri agenda, cosi pagina master pouò ricrreare i check,
                    'ma devo inserirlo sono se non è gia presente altrimenti mi sdoppia le colonne
                    Dim presente As Boolean = False
                    For Each ap As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In objParametriAgenda.Impianti
                        If ap.Piva = objAppezzamento.Piva AndAlso
                            ap.Sa_Cod = objAppezzamento.Sa_Cod AndAlso
                                ap.Appezza = objAppezzamento.Appezza AndAlso
                                ap.ID_Reg = objAppezzamento.ID_Reg Then

                            presente = True
                        End If
                    Next
                    If Not presente Then
                        objParametriAgenda.Impianti.Add(objAppezzamento)
                        objParametriAgenda.salva()
                    End If



                Next


            Next


        Else
            'ci seve essere almeno un movimento dettaglio 
            Throw New ApplicationException
        End If


        'Imposto i valori

        'genero la tabella
        CreoTabellaRilievoDaImpianti(objParametriAgenda.Impianti)
        Dim Tota As Decimal = 0
        For i = 0 To GridView_Rilievo.Rows.Count - 1
            CType(GridView_Rilievo.Rows(i).Cells(7).Controls(1), TextBox).Text = CStr(objParametriAgenda.Impianti(i).Qta)
            Tota = Tota + CDec(objParametriAgenda.Impianti(i).Qta)
        Next


        RadioButtonListMeccanicaManuale.SelectedValue = Modalita


        If Elem_Cod = SEMILAVORATI_VEGETALI Then
            CheckBoxSemilavorati.Checked = True
        ElseIf Elem_Cod = TRASFORMATI_VEGETALI Then
            CheckBoxSemilavorati.Checked = False
        Else
            Throw New Exception
        End If
        If Tipo_Raccolta <> enum_RACCOLTA_TIPO.Fast Then
            caricaMateriePrime()
            caricaUdm()

            cmb_Prodotto.SelectedValue = Mat_Cod
            CmbUdm.SelectedValue = Udm_Cod
        End If

        If Tota <> Qta Then
            'Throw New Exception("Il calcolo non corrisponde")
        End If

        QtaTot.Text = Qta

        TextBoxLotto.Text = Lotto
        QtaTot.Enabled = False
        RadioButtonNomeLotto.SelectedValue = "0"
        ' CheckBoxNomeLotto.Checked = False


        'imposto la ripartizione a manuale e blocco la texbox qta totale
        RadioButtonListRipartizione.SelectedValue = 3


        'non serve, usato nel carico Dim Cod_Progetto As Integer = agenda.Movimenti(i).Movimenti_Dettagli(j).Cod_Progetto

        ' da gestire Dim Cal_Cod As Integer

        If Tipo_Raccolta_Salvata <> Tipo_Raccolta Then
            Dim msg As String = "Attenzione, l'operazione è stata salvata con l'impostazione " & Tipo_Raccolta_Salvata.ToString & " mentre le impostazioni attuali dell'utente sono: " & Tipo_Raccolta.ToString & " quindi potrebbero perdersi delle informazioni se si procede al salvataggio"
            Messaggi.AgroMsgBuonFine(msg, Page, ,
                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
        End If



    End Sub

    Private Sub Blocca(ByVal centri As Boolean, ByVal operazioni As Boolean, ByVal specie As Boolean, ByVal magazzini As Boolean)

        Master_Operazione.Property_txt_DataOperazione.Enabled = False
        Master_Operazione.Property_GridView_Impianti.Enabled = False

        Master_Operazione.Property_ComboSpecie.Enabled = False
        Master_Operazione.Property_BTN_ComboSpecie.Enabled = False

        Master_Operazione.Property_BTN_CentroAziendale.Enabled = False
        Master_Operazione.Property_ComboCentroAziendale.Enabled = False

        Master_Operazione.Property_BTN_ComboOperazione.Enabled = False
        Master_Operazione.Property_ComboOperazione.Enabled = False

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
        'If magazzini Then
        '    script.AppendLine("     BloccaCombo_Magazzini();")
        'End If

        script.AppendLine("} ")



        ScriptManager.RegisterStartupScript(Master_Operazione.Property_UpdatePanelPerScript,
                                Master_Operazione.Property_UpdatePanelPerScript.GetType(),
                                "jQuery_{0}", script.ToString, True)


    End Sub

    Private Sub Sblocca()
        Master_Operazione.Property_txt_DataOperazione.Enabled = True
        Master_Operazione.Property_GridView_Impianti.Enabled = True

        Master_Operazione.Property_ComboSpecie.Enabled = True
        Master_Operazione.Property_BTN_ComboSpecie.Enabled = True

        Master_Operazione.Property_BTN_CentroAziendale.Enabled = True
        Master_Operazione.Property_ComboCentroAziendale.Enabled = True

        Master_Operazione.Property_BTN_ComboOperazione.Enabled = True
        Master_Operazione.Property_ComboOperazione.Enabled = True

    End Sub

#End Region



#Region "Creazione oggetto Agenda"

    Private Function CreaOggettoAgenda(ByVal ListaImp As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto), ByVal Sa_Cod As Integer, ByRef messaggio_errore As String, ByRef ListaOpAgendaCollegate As List(Of Operazione_Agenda)) As Operazione_Agenda
        Dim Agenda As Operazione_Agenda

        Dim Lotto As String = ""



        Dim Elem_Cod As Integer = TRASFORMATI_VEGETALI
        If CheckBoxSemilavorati.Checked Then
            Elem_Cod = SEMILAVORATI_VEGETALI
        End If

        Dim Mat_Cod_Generato As Integer = 0
        If IsNumeric(cmb_Prodotto.SelectedValue) Then
            Mat_Cod_Generato = CInt(cmb_Prodotto.SelectedValue)
        End If
        '--------------AGENDA------------------
        If Not Crea_Agenda(ListaImp, Sa_Cod, Agenda, messaggio_errore, Lotto, Elem_Cod, Mat_Cod_Generato) Then
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


        '------------- MOVIMENTO RACCOLTA---------
        Select Case CInt(objParametriAgenda.Lav_Cod)

            Case LAVCOD_RACCOLTA
                If Not Crea_Agenda_Movimento_Raccolta(Agenda, ListaImp, messaggio_errore, Lotto, Elem_Cod, Mat_Cod_Generato) Then
                    Return Nothing
                End If

            Case Else
                Throw New NotImplementedException
        End Select


        '-------------- MOVIMENTO CARICO---------------------------

        If Tipo_Raccolta <> enum_RACCOLTA_TIPO.Fast AndAlso ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue <> "0" Then
            Dim Movimento_Carico_Creato As Movimento
            If Not Crea_Agenda_Movimento_Carico(Agenda, ListaImp, messaggio_errore, Lotto, Movimento_Carico_Creato) Then
                Return Nothing
            End If


            If ComboLavorazione.SelectedValue <> "0" AndAlso ComboLavorazione.SelectedValue <> "" Then
                'devo creare lo scarico e il carico nell'azienda 2 con i riferimenti
                Dim Operazione As Integer = CInt(ComboLavorazione.SelectedValue)
                Select Case Operazione
                    Case 1


                        'solo con i trasformati
                        If CheckBoxSemilavorati.Checked Then
                            messaggio_errore &= "Attenzione, per la cura occorre selezionare i trasformati e non i semilavorati vegetali"
                            Return Nothing
                        End If

                        ListaOpAgendaCollegate = New List(Of Operazione_Agenda)

                        'solo se l'azienda che cura è un'altra
                        If objParametriAgenda.Piva <> ComboUDS.SelectedValue Then
                            Dim Agenda_Scarico As New Operazione_Agenda
                            Dim Agenda_Carico_Uds As New Operazione_Agenda

                            Dim IdAgedaScaricoOld As Integer = 0
                            Dim IdAgedaCaricoOld As Integer = 0

                            CreaOggettoAgendaScaricoProdottoPerInvioUDS(Agenda, Movimento_Carico_Creato, Agenda_Scarico, IdAgedaScaricoOld)

                            CreaOggettoAgendaCaricoPressoUDS(Agenda, Movimento_Carico_Creato, Agenda_Carico_Uds, IdAgedaCaricoOld)



                            ListaOpAgendaCollegate.Add(Agenda_Scarico)
                            ListaOpAgendaCollegate.Add(Agenda_Carico_Uds)
                        End If



                End Select
            End If

        End If

        LottoMagazzinoSalvato = Lotto

        Return Agenda

    End Function

    Private Sub CreaOggettoAgendaCaricoPressoUDS(ByVal Agenda As Operazione_Agenda, ByVal Movimento_Carico_Creato As Movimento, ByVal Agenda_Scarico As Operazione_Agenda, ByVal Id_AgendaCaricoUDS_OLD As Integer)

        Dim Az_Dest As String = ComboUDS.SelectedValue
        Dim Centro_Dest As Integer = ComboCentroCura.SelectedValue

        Agenda_Scarico.Tipo_Operazione = Agenda.Tipo_Operazione
        Agenda_Scarico.Id_Agenda = Id_AgendaCaricoUDS_OLD
        Agenda_Scarico.Data = Agenda.Data
        Agenda_Scarico.Piva = Az_Dest
        Agenda_Scarico.Sa_Cod = Centro_Dest
        Agenda_Scarico.Lav_Cod = LAVCOD_CARICO
        Agenda_Scarico.Des_Lib = "Carico Raccolta per Lavorazione presso UDS (Az: " & objParametriAgenda.Piva & " - " & objParametriAgenda.RagSoc & ")"
        Agenda_Scarico.BaseCode = Agenda.BaseCode
        Agenda_Scarico.TopCode = Agenda.TopCode

        Dim Movimento_Scarico As New Movimento
        Movimento_Scarico.Id_Agenda = Agenda_Scarico.Id_Agenda
        Movimento_Scarico.Piva = Az_Dest
        Movimento_Scarico.Sa_Cod = Centro_Dest
        Movimento_Scarico.Data = Movimento_Carico_Creato.Data
        Movimento_Scarico.Ora = Movimento_Carico_Creato.Ora
        Movimento_Scarico.Lav_Cod = Agenda_Scarico.Lav_Cod
        Movimento_Scarico.Cau_Mov = CAU_CARICO
        Movimento_Scarico.Mov_Desc = "Carico di Magazzino per Lavorazione presso UDS (Az: " & objParametriAgenda.Piva & " - " & objParametriAgenda.RagSoc & ")"
        Movimento_Scarico.BaseCode = Agenda_Scarico.BaseCode
        Movimento_Scarico.TopCode = Agenda_Scarico.TopCode

        For Each MovimentoDettaglioCarico As Movimento_Dettaglio In Movimento_Carico_Creato.Movimenti_Dettagli

            Dim MovimentoDettaglioScarico As New Movimento_Dettaglio
            MovimentoDettaglioScarico.Id_Agenda = Agenda_Scarico.Id_Agenda
            MovimentoDettaglioScarico.Piva = Az_Dest
            MovimentoDettaglioScarico.Sa_Cod = Centro_Dest
            MovimentoDettaglioScarico.Data = MovimentoDettaglioCarico.Data
            MovimentoDettaglioScarico.Elem_Cod = MovimentoDettaglioCarico.Elem_Cod
            MovimentoDettaglioScarico.Pro_Cod = MovimentoDettaglioCarico.Pro_Cod
            MovimentoDettaglioScarico.Cod_Progetto = MovimentoDettaglioCarico.Cod_Progetto
            MovimentoDettaglioScarico.Mat_Cod = MovimentoDettaglioCarico.Mat_Cod
            MovimentoDettaglioScarico.Mov_Det_Des = "Carico di Prodotti Aziendali per lavorazione presso UDS (Az: " & objParametriAgenda.Piva & " - " & objParametriAgenda.RagSoc & ")"
            MovimentoDettaglioScarico.Udm_Cod = MovimentoDettaglioCarico.Udm_Cod
            MovimentoDettaglioScarico.Qta = MovimentoDettaglioCarico.Qta
            MovimentoDettaglioScarico.Cal_Cod = MovimentoDettaglioCarico.Cal_Cod
            MovimentoDettaglioScarico.Lotto = MovimentoDettaglioCarico.Lotto
            MovimentoDettaglioScarico.Contabilizzato = MovimentoDettaglioCarico.Contabilizzato
            MovimentoDettaglioScarico.Pendente = MovimentoDettaglioCarico.Pendente
            MovimentoDettaglioScarico.Lav_Cod = Agenda_Scarico.Lav_Cod
            MovimentoDettaglioScarico.Cau_Mov = CAU_SCARICO
            MovimentoDettaglioScarico.Anno = 1900

            For Each MovimentoDestinazioneCarico As Movimento_Destinazione In MovimentoDettaglioCarico.Movimenti_Destinazioni

                Dim MovimentoDestinazioneScarico As New Movimento_Destinazione
                MovimentoDestinazioneScarico.Id_Agenda = Agenda_Scarico.Id_Agenda
                MovimentoDestinazioneScarico.Data = MovimentoDestinazioneCarico.Data
                MovimentoDestinazioneScarico.Piva = Az_Dest
                MovimentoDestinazioneScarico.Sa_Cod = Centro_Dest
                Dim prg As New AgronicaCoreAnagrafeDAL.Fabbricati_R()
                Dim Id_Destinazione As Integer = prg.LeggiMagazzino(Az_Dest, Centro_Dest, objParametri_Server)
                If Id_Destinazione = 0 Then
                    Id_Destinazione = CreaMagazzino(Agenda_Scarico, Az_Dest, Centro_Dest)
                    If Id_Destinazione = 0 Then
                        Throw New Exception("Non c'è nessun magazzino nel dentro dell'UDS selezionata, creare prima un magazzino e ritentare.")
                    End If
                End If
                MovimentoDestinazioneScarico.Id_Destinazione = Id_Destinazione ' devo trovare il magazzino
                MovimentoDestinazioneScarico.Tipo = MAGAZZINO
                MovimentoDestinazioneScarico.Qta = MovimentoDestinazioneCarico.Qta
                MovimentoDestinazioneScarico.BaseCode = Agenda_Scarico.BaseCode
                MovimentoDestinazioneScarico.TopCode = Agenda_Scarico.TopCode

                MovimentoDettaglioScarico.Movimenti_Destinazioni.Add(MovimentoDestinazioneScarico)
            Next
            Movimento_Scarico.Movimenti_Dettagli.Add(MovimentoDettaglioScarico)
        Next

        Agenda_Scarico.Movimenti.Add(Movimento_Scarico)
    End Sub

    Private Sub CreaOggettoAgendaScaricoProdottoPerInvioUDS(ByVal Agenda As Operazione_Agenda, ByVal Movimento_Carico_Creato As Movimento, ByVal Agenda_Scarico As Operazione_Agenda, ByVal Id_AgendaScaricoPErUDS_OLD As Integer)
        Agenda_Scarico.Tipo_Operazione = Agenda.Tipo_Operazione
        Agenda_Scarico.Id_Agenda = Id_AgendaScaricoPErUDS_OLD
        Agenda_Scarico.Data = Agenda.Data
        Agenda_Scarico.Piva = Agenda.Piva
        Agenda_Scarico.Sa_Cod = Movimento_Carico_Creato.Sa_Cod
        Agenda_Scarico.Lav_Cod = LAVCOD_SCARICO
        Agenda_Scarico.Des_Lib = "Scarico Raccolta per Lavorazione presso UDS " & ComboUDS.SelectedItem.Text & " - " & ComboCentroCura.SelectedItem.Text
        Agenda_Scarico.BaseCode = Agenda.BaseCode
        Agenda_Scarico.TopCode = Agenda.TopCode

        Dim Movimento_Scarico As New Movimento
        Movimento_Scarico.Id_Agenda = Agenda_Scarico.Id_Agenda
        Movimento_Scarico.Piva = Movimento_Carico_Creato.Piva
        Movimento_Scarico.Sa_Cod = Movimento_Carico_Creato.Sa_Cod
        Movimento_Scarico.Data = Movimento_Carico_Creato.Data
        Movimento_Scarico.Ora = Movimento_Carico_Creato.Ora
        Movimento_Scarico.Lav_Cod = Agenda_Scarico.Lav_Cod
        Movimento_Scarico.Cau_Mov = CAU_SCARICO
        Movimento_Scarico.Mov_Desc = "Scarico di Magazzino per Lavorazione presso UDS " & ComboUDS.SelectedItem.Text & " - " & ComboCentroCura.SelectedItem.Text
        Movimento_Scarico.BaseCode = Agenda_Scarico.BaseCode
        Movimento_Scarico.TopCode = Agenda_Scarico.TopCode

        For Each MovimentoDettaglioCarico As Movimento_Dettaglio In Movimento_Carico_Creato.Movimenti_Dettagli

            Dim MovimentoDettaglioScarico As New Movimento_Dettaglio
            MovimentoDettaglioScarico.Id_Agenda = Agenda_Scarico.Id_Agenda
            MovimentoDettaglioScarico.Piva = MovimentoDettaglioCarico.Piva
            MovimentoDettaglioScarico.Sa_Cod = MovimentoDettaglioCarico.Sa_Cod
            MovimentoDettaglioScarico.Data = MovimentoDettaglioCarico.Data
            MovimentoDettaglioScarico.Elem_Cod = MovimentoDettaglioCarico.Elem_Cod
            MovimentoDettaglioScarico.Pro_Cod = MovimentoDettaglioCarico.Pro_Cod
            MovimentoDettaglioScarico.Cod_Progetto = MovimentoDettaglioCarico.Cod_Progetto
            MovimentoDettaglioScarico.Mat_Cod = MovimentoDettaglioCarico.Mat_Cod
            MovimentoDettaglioScarico.Mov_Det_Des = "Scarico di Prodotti Aziendali per lavorazione presso UDS " & ComboUDS.SelectedItem.Text & " - " & ComboCentroCura.SelectedItem.Text
            MovimentoDettaglioScarico.Udm_Cod = MovimentoDettaglioCarico.Udm_Cod
            MovimentoDettaglioScarico.Qta = MovimentoDettaglioCarico.Qta
            MovimentoDettaglioScarico.Cal_Cod = MovimentoDettaglioCarico.Cal_Cod
            MovimentoDettaglioScarico.Lotto = MovimentoDettaglioCarico.Lotto
            MovimentoDettaglioScarico.Contabilizzato = MovimentoDettaglioCarico.Contabilizzato
            MovimentoDettaglioScarico.Pendente = MovimentoDettaglioCarico.Pendente
            MovimentoDettaglioScarico.Lav_Cod = Agenda_Scarico.Lav_Cod
            MovimentoDettaglioScarico.Cau_Mov = CAU_SCARICO
            MovimentoDettaglioScarico.Anno = 1900

            For Each MovimentoDestinazioneCarico As Movimento_Destinazione In MovimentoDettaglioCarico.Movimenti_Destinazioni

                Dim MovimentoDestinazioneScarico As New Movimento_Destinazione
                MovimentoDestinazioneScarico.Id_Agenda = Agenda_Scarico.Id_Agenda
                MovimentoDestinazioneScarico.Data = MovimentoDestinazioneCarico.Data
                MovimentoDestinazioneScarico.Piva = MovimentoDestinazioneCarico.Piva
                MovimentoDestinazioneScarico.Sa_Cod = MovimentoDestinazioneCarico.Sa_Cod
                MovimentoDestinazioneScarico.Id_Destinazione = MovimentoDestinazioneCarico.Id_Destinazione
                MovimentoDestinazioneScarico.Tipo = MAGAZZINO
                MovimentoDestinazioneScarico.Qta = MovimentoDestinazioneCarico.Qta
                MovimentoDestinazioneScarico.BaseCode = Agenda_Scarico.BaseCode
                MovimentoDestinazioneScarico.TopCode = Agenda_Scarico.TopCode

                MovimentoDettaglioScarico.Movimenti_Destinazioni.Add(MovimentoDestinazioneScarico)
            Next
            Movimento_Scarico.Movimenti_Dettagli.Add(MovimentoDettaglioScarico)
        Next

        Agenda_Scarico.Movimenti.Add(Movimento_Scarico)
    End Sub

    Private Function CreaMagazzino(ByVal Agenda_Scarico As Operazione_Agenda, ByRef Az_Dest As String, ByRef Centro_Dest As Integer) As Integer
        Dim Ind_Des As String = ""
        Dim Frz_Des As String = ""
        Dim CAP As String = ""
        Dim Stato As String = ""
        Dim Comune As String = ""
        Dim Provincia As String = ""
        Dim Sigla_Prov As String = ""
        Dim pro_cod_istat As String = ""
        Dim com_cod_istat As String = ""
        Dim Cod_Regione As String = ""

        Dim x As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
        x.Indirizzo_from_PivaSaCod(Az_Dest, Centro_Dest, Ind_Des, Frz_Des,
                                 CAP,
                                 Stato,
                                 Comune,
                                 Provincia,
                                 Sigla_Prov,
                                 pro_cod_istat,
                                 com_cod_istat,
                                 Cod_Regione,
                                 objParametri_Server
                                )

        Dim XML_Anagrafe As New AgronicaCoreXML.XML_Anagrafe
        Dim XmlDoc2 As New XmlDocument
        Dim log2 As String = ""
        Dim Fabbricato_Des = "Magazzino Cure"
        Dim XMLFAbbricato As XmlElement = XML_Anagrafe.XML_2_Fabbricati(log2,
                                                XmlDoc2,
                                                Agenda_Scarico.BaseCode,
                                                Agenda_Scarico.TopCode,
                                                enum_TipoOperazioneDB.Scrittura,
                                                Az_Dest,
                                                Centro_Dest,
                                                0,
                                                Fabbricato_Des,
                                                TipiEnumerativi.enum_FabbricatiTipi.MagazzinoAziendale,
                                                1,
                                                pro_cod_istat,
                                                com_cod_istat,
                                                0,
                                                Ind_Des,
                                                "",
                                                CAP,
                                                "ITALIA",
                                                "",
                                                Nothing,
                                                Nothing,
                                                Nothing,
                                                Nothing,
                                                Nothing,
                                                , , , , , , , , , ,
                                                TipiEnumerativi.enum_TitoloPossesso.Proprieta,
                                                , , , , , , , , , , , , , ,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                , , , , , , , )


        Dim Fabbricato_W As New AgronicaCoreAnagrafeBIZ.Fabbricato_W
        Dim fabbricato_cod As Integer
        Fabbricato_W.Fabbricato_Scrivi(XMLFAbbricato.OuterXml, Az_Dest, Centro_Dest, fabbricato_cod, objParametri_Server)
        Return fabbricato_cod

    End Function

    Private Function Crea_Agenda(ByRef ListaImp As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto), ByRef Sa_Cod As Integer, ByRef Agenda As Operazione_Agenda, ByRef messaggio_errore As String, ByRef Lotto As String, ByVal Elem_Cod As Integer, ByRef Mat_Cod_Ritorno As Integer) As Boolean

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


        If Not IsDate(txt_DataMagazzino.Text) Then
            txt_DataMagazzino.Text = objParametriAgenda.Data.ToShortDateString
        End If

        If Not IsDate(txt_OraMagazzino.Text) Then
            txt_OraMagazzino.Text = "00:00"
        End If

        '---------------------------------------
        ' recupero la SPECIE
        Dim Veg_Cod As String = ""
        Dim Veg_Des As String = ""
        If objParametriAgenda.Veg_Cod.Split("/")(0) = "-1" Then
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
        End If



        Dim BaseCode As Integer
        Dim TopCode As Integer

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------
        Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))



        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------


        Dim trovato As Boolean
        Dim ListaVarieta As New List(Of String)
        Dim ListaCul_Cod As New List(Of Integer)
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
                ListaCul_Cod.Add(ListaImp(i).Cul_Cod)
                StrVarieta &= ", " & ListaImp(i).Cul_Des
            End If
        Next

        If StrVarieta.Length >= 2 Then
            StrVarieta = StrVarieta.Substring(2)
        End If



        'GENERO PRODOTTO SE NON INDICATO (per ora solo sulla specie, varietà altre)
        Dim Mat_Des_Ritorno As String = ""
        If Mat_Cod_Ritorno = -1 Then
            Mat_Cod_Ritorno = GeneraProdotto(Veg_Cod, Veg_Des, ListaCul_Cod, Elem_Cod, Mat_Des_Ritorno)
        Else
            Mat_Des_Ritorno = New AgronicaCoreAnagrafeDAL.Materie_Prime_R().MatDes_from_MatCod("", Elem_Cod, Mat_Cod_Ritorno, "", "", "", objParametri_Server)
        End If



        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = objParametriAgenda.Tipo_Operazione
        Agenda.Id_Agenda = objParametriAgenda.Id_Agenda
        Agenda.Data = Data
        Agenda.Piva = objParametriAgenda.Piva
        Agenda.Sa_Cod = Sa_Cod
        Agenda.Lav_Cod = objParametriAgenda.Lav_Cod


        If RadioButtonNomeLotto.SelectedValue = "1" Then
            '"Generato dalla Data"
            Lotto = Agenda.Data.Year & "" & Agenda.Data.Month.ToString.PadLeft(2, "0") & "" & Agenda.Data.Day.ToString.PadLeft(2, "0") & ""
        ElseIf RadioButtonNomeLotto.SelectedValue = "2" Then
            '"Generato dalla Data"
            Lotto = GeneraIdLottoRaccolta(objParametriAgenda.Piva)
        Else
            Lotto = TextBoxLotto.Text.Trim
        End If


        Dim Articolo As String = Mat_Des_Ritorno
        If Tipo_Raccolta = enum_RACCOLTA_TIPO.Fast Then
            Agenda.Des_Lib = Lav_Des & " (" & Veg_Des & "  [" & StrVarieta & "])"
        Else
            If Lotto = "" Then
                Agenda.Des_Lib = Lav_Des & " (" & Veg_Des & "  [" & StrVarieta & "]) - Articolo: " & Articolo
            Else
                Agenda.Des_Lib = Lav_Des & " (" & Veg_Des & "  [" & StrVarieta & "]) - Lotto: " & Lotto & " - Articolo: " & Articolo
            End If

        End If


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

    Private Function Crea_Agenda_Movimento_Raccolta(ByRef Agenda As Operazione_Agenda, ByRef ListaImp As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto), ByRef messaggio_errore As String, ByVal Lotto As String, ByVal Elem_Cod As Integer, ByVal Mat_Cod As Integer) As Boolean

        Select Case CInt(objParametriAgenda.Lav_Cod)

            Case LAVCOD_RACCOLTA
                'continua, questo metodo deve essere chioamato solo in questi casi altrimenti genero accezione
            Case Else
                Throw New Exception
        End Select


        Dim Udm As Integer = CInt(CmbUdm.SelectedValue)

        Dim Movimento_OperazioneColturale As New Movimento

        Movimento_OperazioneColturale.Id_Agenda = Agenda.Id_Agenda
        Movimento_OperazioneColturale.Piva = Agenda.Piva
        Movimento_OperazioneColturale.Sa_Cod = Agenda.Sa_Cod
        Movimento_OperazioneColturale.Lav_Cod = objParametriAgenda.Lav_Cod
        Movimento_OperazioneColturale.Cau_Mov = objParametriAgenda.Cau_Mov
        Movimento_OperazioneColturale.Mov_Desc = Master_Operazione.GetNota()
        Movimento_OperazioneColturale.Data = Agenda.Data
        Movimento_OperazioneColturale.BaseCode = Agenda.BaseCode
        Movimento_OperazioneColturale.TopCode = Agenda.TopCode

        Movimento_OperazioneColturale.Modalita = RadioButtonListMeccanicaManuale.SelectedValue

        Movimento_OperazioneColturale.Extra_Int = Tipo_Raccolta 'salvo il tipo raccolta, puo essere utile

        '----------------------------------------------------------
        '----- MOVIMENTI DETTAGLI , DET.TECNICI, DESTINAZIONI -----
        '----------------------------------------------------------
        Dim Movimento_Dettaglio As New Movimento_Dettaglio
        Movimento_Dettaglio.Id_Agenda = Agenda.Id_Agenda
        Movimento_Dettaglio.Piva = Agenda.Piva
        Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod

        Movimento_Dettaglio.Mov_Det_Des = "Dettagli Prodotti Aziendali Ottenuti da Raccolta"

        Movimento_Dettaglio.Elem_Cod = Elem_Cod
        Movimento_Dettaglio.Pro_Cod = 0
        Movimento_Dettaglio.Mat_Cod = Mat_Cod
        Movimento_Dettaglio.Udm_Cod = Udm
        'dopo  Movimento_Dettaglio.Qta = QtaTot
        Movimento_Dettaglio.Contabilizzato = NONCONTABILE
        Movimento_Dettaglio.Pendente = enum_Pendenza.MovESENTE
        Movimento_Dettaglio.Cal_Cod = 12 'Materie_Prime_Calibri cal_cod=12 indefinito
        Movimento_Dettaglio.Lotto = Lotto
        Movimento_Dettaglio.Data = Agenda.Data
        Movimento_Dettaglio.Anno = 1900
        Movimento_Dettaglio.BaseCode = Agenda.BaseCode
        Movimento_Dettaglio.TopCode = Agenda.TopCode

        Dim xCalcolo_QD_SuperficieTotale As Decimal = (
                From ST In ListaImp
                Select CType(ST.Qta2, Decimal)
            ).Sum

        Dim tot As Decimal = 0
        For i = 0 To ListaImp.Count - 1

            Dim Movimento_Destinazione As New Movimento_Destinazione

            Movimento_Destinazione.Id_Agenda = Agenda.Id_Agenda
            Movimento_Destinazione.Piva = ListaImp(i).Piva
            Movimento_Destinazione.Sa_Cod = ListaImp(i).Sa_Cod
            Movimento_Destinazione.Appezza = ListaImp(i).Appezza
            Movimento_Destinazione.Id_Destinazione = ListaImp(i).ID_Reg
            Movimento_Destinazione.Qta = ListaImp(i).Qta
            Movimento_Destinazione.Qta2 = ListaImp(i).Qta2
            tot = tot + ListaImp(i).Qta
            Movimento_Destinazione.BaseCode = Agenda.BaseCode
            Movimento_Destinazione.TopCode = Agenda.TopCode

            If xCalcolo_QD_SuperficieTotale <> 0 Then
                Movimento_Destinazione.QuotaDistribuzione = ListaImp(i).Qta2 / xCalcolo_QD_SuperficieTotale
            End If

            'salvo qui progetto cod per mettere poi nel dettaglio della destinazione nel caso di semilavorati
            Movimento_Destinazione.parametroGenerico = ListaImp(i).Progetto_Cod

            Movimento_Dettaglio.Movimenti_Destinazioni.Add(Movimento_Destinazione)
        Next

        Movimento_Dettaglio.Qta = tot

        Movimento_OperazioneColturale.Movimenti_Dettagli.Add(Movimento_Dettaglio)
        'se ho almeno un movimento dettaglio nell'operazione la aggiungo all'agenda
        If Movimento_OperazioneColturale.Movimenti_Dettagli.Count > 0 Then
            'aggiungo il movimento rilievo in campo-installazione trappola all'operazione agenda
            Agenda.Movimenti.Add(Movimento_OperazioneColturale)
        Else
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.NessunDatoSalvato
            Return False
        End If



        Return True
    End Function

    Private Function Crea_Agenda_Movimento_Carico(ByRef Agenda As Operazione_Agenda, ByRef ListaImp As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto), ByRef messaggio_errore As String, ByVal Lotto As String, ByRef Movimento_Carico As Movimento) As Boolean


        Dim Movimento_Raccolta As Movimento = Agenda.Movimenti(Agenda.Movimenti.Count - 1)
        If Movimento_Raccolta.Cau_Mov <> objParametriAgenda.Cau_Mov Then
            Throw New Exception()
        End If



        Dim Sa_Cod_magazzino As String = ""
        Dim fabbricatox_Cod As String = ""

        Dim magazzinoEsterno As Boolean = True
        If ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue <> "0" AndAlso Split(ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue, "|").Count = 3 AndAlso Split(ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue, "|")(2) <> objParametriAgenda.Piva Then
            'se magazzino è della azienda padre
            magazzinoEsterno = True
            Dim Sa_Cod_magazzino_predefinito_azienda As Integer
            Dim fabbricatox_Cod_magazzino_predefinito_azienda As Integer
            Dim fabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R()
            fabbricati.Ricava_PrimoMagazzino_Impresa(Agenda.Piva, Sa_Cod_magazzino_predefinito_azienda, fabbricatox_Cod_magazzino_predefinito_azienda, objParametri_Server)
            Sa_Cod_magazzino = Sa_Cod_magazzino_predefinito_azienda
            fabbricatox_Cod = fabbricatox_Cod_magazzino_predefinito_azienda

        Else
            'se magazzino è quello dell'azienda
            magazzinoEsterno = False
            Sa_Cod_magazzino = Split(ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue, "|")(1)
            fabbricatox_Cod = Split(ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue, "|")(0)

        End If

        If False Then 'CAU_CONFERIMENTO Then
            Sa_Cod_magazzino = -1
            fabbricatox_Cod = -1
            '...
        End If

        Dim generalizzata As Boolean = False
        If Movimento_Raccolta.Movimenti_Dettagli(0).Elem_Cod = TRASFORMATI_VEGETALI Then
            generalizzata = True
        ElseIf Movimento_Raccolta.Movimenti_Dettagli(0).Elem_Cod = SEMILAVORATI_VEGETALI Then
            generalizzata = False
        Else
            Throw New Exception()
        End If



        Movimento_Carico = New Movimento
        Movimento_Carico.Id_Agenda = Agenda.Id_Agenda
        Movimento_Carico.Piva = Agenda.Piva
        Movimento_Carico.Sa_Cod = Sa_Cod_magazzino 'Agenda.Sa_Cod è sbagliato se ho magazzino in altro centro se multicentro è sbagliat ousare Split(ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue, "|")(1)
        Movimento_Carico.Data = CDate(txt_DataMagazzino.Text)
        Movimento_Carico.Ora = CDate(txt_OraMagazzino.Text)
        Movimento_Carico.Lav_Cod = objParametriAgenda.Lav_Cod
        Movimento_Carico.Cau_Mov = CAU_CARICO
        Movimento_Carico.Mov_Desc = "Carico di Magazzino Da Raccolta"
        Movimento_Carico.BaseCode = Agenda.BaseCode
        Movimento_Carico.TopCode = Agenda.TopCode

        '(01/02/2017 fede) il cal_cod è sempre lo stesso per ora 
        'anche se ho diversi dettagli (caso semilavorati)
        Dim Progr As Integer = GeneraCampionatura()

        If generalizzata Then
            'un movimento dettaglio e una destinazione (trasformati)
            '------------------------------------------------
            '----- MOVIMENTI DETTAGLI CARICO 
            '------------------------------------------------

            Dim Movimento_Dettaglio_Carico_Trappola As New Movimento_Dettaglio

            Movimento_Dettaglio_Carico_Trappola.Id_Agenda = Agenda.Id_Agenda
            Movimento_Dettaglio_Carico_Trappola.Piva = Agenda.Piva
            Movimento_Dettaglio_Carico_Trappola.Sa_Cod = Sa_Cod_magazzino 'ok per multicentro
            Movimento_Dettaglio_Carico_Trappola.Data = CDate(txt_DataMagazzino.Text)
            Movimento_Dettaglio_Carico_Trappola.Elem_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Elem_Cod
            Movimento_Dettaglio_Carico_Trappola.Pro_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Pro_Cod
            Movimento_Dettaglio_Carico_Trappola.Mat_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Mat_Cod
            Movimento_Dettaglio_Carico_Trappola.Mov_Det_Des = "Carico di Trasformati Aziendali da Raccolta"
            Movimento_Dettaglio_Carico_Trappola.Udm_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Udm_Cod
            Movimento_Dettaglio_Carico_Trappola.Qta = Movimento_Raccolta.Movimenti_Dettagli(0).Qta
            Movimento_Dettaglio_Carico_Trappola.Cal_Cod = Progr 'negativo, puntatore
            Movimento_Dettaglio_Carico_Trappola.Lotto = Movimento_Raccolta.Movimenti_Dettagli(0).Lotto
            Movimento_Dettaglio_Carico_Trappola.Contabilizzato = NONCONTABILE
            Movimento_Dettaglio_Carico_Trappola.Pendente = enum_Pendenza.MovGiustificato
            Movimento_Dettaglio_Carico_Trappola.Lav_Cod = objParametriAgenda.Lav_Cod
            Movimento_Dettaglio_Carico_Trappola.Cau_Mov = Movimento_Carico.Cau_Mov
            Movimento_Dettaglio_Carico_Trappola.Anno = 1900

            'Movimento_Dettaglio_Carico_Trappola.Raccolto_Campionatura = XML_GeneraBlocco_RaccoltoCampionatura()
            '------------------------------------------------
            '----- MOVIMENTI DESTINAZIONI SCARICO 
            '------------------------------------------------
            'per generalizzata uno altrimenti per ciasdcun Appezza
            Dim Movimento_Destinazione_Carico_Trappola As New Movimento_Destinazione
            Movimento_Destinazione_Carico_Trappola.Id_Agenda = Agenda.Id_Agenda
            Movimento_Destinazione_Carico_Trappola.Data = CDate(txt_DataMagazzino.Text)
            Movimento_Destinazione_Carico_Trappola.Piva = objParametriAgenda.Piva
            Movimento_Destinazione_Carico_Trappola.Sa_Cod = Sa_Cod_magazzino
            Movimento_Destinazione_Carico_Trappola.Id_Destinazione = fabbricatox_Cod
            Movimento_Destinazione_Carico_Trappola.Tipo = MAGAZZINO
            Movimento_Destinazione_Carico_Trappola.Qta = Movimento_Raccolta.Movimenti_Dettagli(0).Qta
            Movimento_Destinazione_Carico_Trappola.BaseCode = Agenda.BaseCode
            Movimento_Destinazione_Carico_Trappola.TopCode = Agenda.TopCode


            Movimento_Dettaglio_Carico_Trappola.Movimenti_Destinazioni.Add(Movimento_Destinazione_Carico_Trappola)
            Movimento_Carico.Movimenti_Dettagli.Add(Movimento_Dettaglio_Carico_Trappola)


            '-------------- aggiungo il movimento scarico all'agenda
            Agenda.Movimenti.Add(Movimento_Carico)
        Else
            '
            'un movimento dettaglio e una destinazione per ciascun impianto (semilavorati)

            For Each destinazione In Movimento_Raccolta.Movimenti_Dettagli(0).Movimenti_Destinazioni
                '------------------------------------------------
                '----- MOVIMENTI DETTAGLI SCARICO 
                '------------------------------------------------

                Dim Movimento_Dettaglio_Carico_Trappola As New Movimento_Dettaglio

                Movimento_Dettaglio_Carico_Trappola.Id_Agenda = Agenda.Id_Agenda
                Movimento_Dettaglio_Carico_Trappola.Piva = Agenda.Piva
                Movimento_Dettaglio_Carico_Trappola.Sa_Cod = Sa_Cod_magazzino 'ok per multicentro
                Movimento_Dettaglio_Carico_Trappola.Data = CDate(txt_DataMagazzino.Text)
                Movimento_Dettaglio_Carico_Trappola.Elem_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Elem_Cod
                Movimento_Dettaglio_Carico_Trappola.Pro_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Pro_Cod
                Movimento_Dettaglio_Carico_Trappola.Mat_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Mat_Cod
                Movimento_Dettaglio_Carico_Trappola.Mov_Det_Des = "Carico di Semiavorati Aziendali da Raccolta"
                Movimento_Dettaglio_Carico_Trappola.Udm_Cod = Movimento_Raccolta.Movimenti_Dettagli(0).Udm_Cod
                Movimento_Dettaglio_Carico_Trappola.Qta = destinazione.Qta 'DIFFERENZA - SEMILAVORATO
                Movimento_Dettaglio_Carico_Trappola.Cal_Cod = Progr 'negativo, puntatore
                Movimento_Dettaglio_Carico_Trappola.Lotto = Movimento_Raccolta.Movimenti_Dettagli(0).Lotto
                Movimento_Dettaglio_Carico_Trappola.Contabilizzato = NONCONTABILE
                Movimento_Dettaglio_Carico_Trappola.Pendente = enum_Pendenza.MovESENTE
                Movimento_Dettaglio_Carico_Trappola.Lav_Cod = objParametriAgenda.Lav_Cod
                Movimento_Dettaglio_Carico_Trappola.Cau_Mov = Movimento_Carico.Cau_Mov
                Movimento_Dettaglio_Carico_Trappola.Anno = 1900


                Movimento_Dettaglio_Carico_Trappola.Cod_Progetto = destinazione.parametroGenerico 'DIFFERENZA - SEMILAVORATO

                '------------------------------------------------
                '----- MOVIMENTI DESTINAZIONI SCARICO 
                '------------------------------------------------
                'per generalizzata uno altrimenti per ciasdcun Appezza
                Dim Movimento_Destinazione_Carico_Trappola As New Movimento_Destinazione
                Movimento_Destinazione_Carico_Trappola.Id_Agenda = Agenda.Id_Agenda
                Movimento_Destinazione_Carico_Trappola.Data = CDate(txt_DataMagazzino.Text)
                Movimento_Destinazione_Carico_Trappola.Piva = objParametriAgenda.Piva
                Movimento_Destinazione_Carico_Trappola.Sa_Cod = Sa_Cod_magazzino
                Movimento_Destinazione_Carico_Trappola.Appezza = 0 'destinazione.Appezza noo 'DIFFERENZA - SEMILAVORATO
                Movimento_Destinazione_Carico_Trappola.Id_Destinazione = fabbricatox_Cod
                Movimento_Destinazione_Carico_Trappola.Tipo = MAGAZZINO
                Movimento_Destinazione_Carico_Trappola.Qta = destinazione.Qta 'DIFFERENZA - SEMILAVORATO
                Movimento_Destinazione_Carico_Trappola.BaseCode = Agenda.BaseCode
                Movimento_Destinazione_Carico_Trappola.TopCode = Agenda.TopCode


                Movimento_Dettaglio_Carico_Trappola.Movimenti_Destinazioni.Add(Movimento_Destinazione_Carico_Trappola)
                Movimento_Carico.Movimenti_Dettagli.Add(Movimento_Dettaglio_Carico_Trappola)
            Next


            '-------------- aggiungo il movimento scarico all'agenda
            Agenda.Movimenti.Add(Movimento_Carico)

        End If


        Return True


    End Function

#End Region



#Region "Eventi Gestiti Dalla Master"

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        RipristinaSessione()
        Dim objParametriAgenda As New ParametriAgenda
        Dim link As String = ""

        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 AndAlso
            objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Verifica_Conformita Then
            Dim strJS As New StringBuilder
            strJS.AppendLine("$(document).ready(function () { ")
            strJS.AppendLine("      window.close(); ")
            strJS.AppendLine(" });")

            ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

            Exit Sub
        End If

        Try
            Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda.SitoOrigine
            Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

            If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
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

    End Sub

    Private Sub MasterUnload()
        'con questo mi ricarice tutte le combo e toglie selezionato in modifica

        'questa operazione è fatta dall'evento master unload
        'ma il load della pagina master potrebbe avere cambiato dei valori in objparametri (come nel caso di Sa_Cod).
        'se alla fine modifico nella pagina fioglia un valore viene in automatico riscritta in sessione
        'la objparametri della pagina figlia, sovrascrivendo la master e perdendo quindi le modifiche fatte
        'objParametriAgenda.Leggi()

    End Sub

    Private Sub RipristinaSessione()
        Session("DtGridViewColli") = Nothing
        objParametriAgenda.Svuota_DatiOperazione()
        objParametriAgenda.OperazioneMulticentro = True
    End Sub

    Private Sub aggiornaDataOperazione(sender As Object, e As EventArgs)
        Dim data As String = Master_Operazione.Property_txt_DataOperazione.Text
        txt_DataMagazzino.Text = data
        'TextBoxDataInizioCura.Text = data
        'TextBoxDataFineCura.Text = CDate(data).AddDays(7).ToShortDateString

        'dato che la master carica prima e se c'è una sola scpecie la preseleziona 
        'e questo avviene successivamente al carico della combo delel materie prime
        'devo rilanciarla altrimenti se entro con tutte le specie (-1 in objparametriagnda)
        'e solo dopo viene indicata la specie, non mi carica la combo anche se per quella specie c'è il valore
        'Dato che il pulsante aggiroan data viene richiamato dalal master via javascript e fa quindi un nuovo caricamento della pagina
        'lo uso per ricaricare la combo.
        'potre gestire nella master ma dato che funziona non la tocco
        If Tipo_Raccolta <> enum_RACCOLTA_TIPO.Fast Then
            caricaMateriePrime()
        End If

    End Sub

#End Region



#Region "Salvataggio"

    Private Sub SalvaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        '--------------BLOCCO SALVATAGGIO SENZA MAGAZZINO-------------------
        If Not IsNothing(Session("UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO")) AndAlso Session("UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO") = True Then
            If ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue = "0" OrElse ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue = "" Then
                Dim MErrore As String
                MErrore = "In base alle impostazioni utente NON è possibile salvare l'operazione senza utilizzare il magazzino!"
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & MErrore, Page, ,
                    CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                Exit Sub
            End If

        End If
        '---------------------------------------------------------------------

        Dim messaggio_errore As String = ""
        Dim messaggio_alert As String = ""

        If SalvaOperazioneAgenda(messaggio_errore, messaggio_alert) Then
            gestisciTipoSalvataggio()
        Else
            If messaggio_errore <> "" Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & messaggio_errore, Page, ,
                               Master_Operazione.Property_UpdatePanelToolBar)
            Else
                If messaggio_alert <> "" Then
                    ' VAnni: 28/2/2020: Non usare il resx sulla stringa "Salva"
                    Messaggi.AgroSiNo(messaggio_alert & vbCr & Resources.AgronicaAgenda_2010.BrBIProcedereUgualmenteIB, "Salva", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                    'Messaggi.AgroSiNo(messaggio_alert, "DEL", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel), messaggio_alert.Replace("'", "&#39;"))
                End If
            End If
        End If

    End Sub

    Private Function SalvaOperazioneAgenda(ByRef messaggio_errore As String, ByRef messaggio_alert As String) As Boolean

        Dim AnagBiz As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W

        Dim res As Boolean = False

        If ComboLavorazione.SelectedValue <> "0" AndAlso ComboLavorazione.SelectedValue <> "" Then
            'devo saltare ad una operazione post raccolta
            Dim Operazione As Integer = CInt(ComboLavorazione.SelectedValue)
            Select Case Operazione
                Case 1
                    If ComboUDS.SelectedValue = "" OrElse ComboUDS.SelectedValue = "0" OrElse ComboCentroCura.SelectedValue = "" OrElse ComboCentroCura.SelectedValue = "0" Then
                        messaggio_errore = messaggio_errore & "selezionare l'uds e il centro di cura"
                        Return False
                    End If
            End Select
        End If

        '---------------------------------------
        ' recupero il CENTRO
        If (objParametriAgenda.Sa_Cod = "" OrElse objParametriAgenda.Sa_Cod = "0") AndAlso Not objParametriAgenda.OperazioneMulticentro Then
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.SelezionareUnCentroAziendale
            Return False
        End If


        '---------------------------------------
        ' recupero gli IMPIANTI
        Dim ListaImpianti2 As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
        ListaImpianti2 = CType(Master, Operazione).GetImpianti()
        If ListaImpianti2.Count = 0 Then
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.SelezionareAlmenoUnImpiantoColturale
            Return False
        End If

        '---------------------------------------
        'Se è stato selezionato 'Tutti i centri aziendali' ma tutti gli impianti appartengono ad un solo centro, imposto il valore sull'objparametriAgenda
        If objParametriAgenda.Sa_Cod = "0" Then
            Dim listaSaCodImpianti As List(Of Integer) = (From x As AgronicaCoreModello.ParametriAgenda_Temp.Impianto In ListaImpianti2 Select x.Sa_Cod).Distinct().ToList()
            If listaSaCodImpianti.Count = 1 Then
                objParametriAgenda.Sa_Cod = listaSaCodImpianti.First().ToString()
            End If
        End If
        '---------------------------------------

        'genero o leggo la gridview
        Select Case RadioButtonListRipartizione.SelectedValue
            Case "1" 'Sup
                generaTabellaTaccoltaManuale()
                If Tipo_Raccolta <> enum_RACCOLTA_TIPO.Fast Then
                    distribuisci(True)
                Else
                    QtaTot.Text = "0"
                End If
            Case "2" 'piante
                generaTabellaTaccoltaManuale()
                If Tipo_Raccolta <> enum_RACCOLTA_TIPO.Fast Then
                    distribuisci(False)
                Else
                    QtaTot.Text = "0"
                End If
            Case "3" 'manuale
        End Select

        'recupero impianti dalla gridview

        If Tipo_Raccolta <> enum_RACCOLTA_TIPO.Fast AndAlso ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue <> "0" AndAlso (Not IsNumeric(QtaTot.Text) OrElse (IsNumeric(QtaTot.Text) AndAlso CDbl(QtaTot.Text) <= 0)) Then
            messaggio_errore = "Inserire un valore per la quantità raccolta"
            Return False
        End If

        If QtaTot.Text.Trim = "" Then
            QtaTot.Text = "0"
        End If

        If Not IsNumeric(QtaTot.Text) OrElse (IsNumeric(QtaTot.Text) AndAlso CDbl(QtaTot.Text) < 0) Then
            Dim msg As String = "Valore non valido per la quantità, inserire un valore maggiore di 0"
            Messaggi.AgroMsgBuonFine(msg, Page, ,
                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Exit Function
        End If


        Dim QtaTotText As Decimal = QtaTot.Text
        Dim totale As Decimal = 0
        Dim App_Nome As String = ""


        '(02/11/2015 fede) introdotto blocco se carenza non rispettata
        '- impostazione_utente NON presente --> blocco il salvataggio per compatibilità col pregresso (ad oggi è sempre bloccato)
        '- impostazione_utente = 1 --> blocco il salvataggio
        '- impostazione_utente = 0 --> NON blocco il salvataggio

        Dim BloccoCarenza As String = ""
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        BloccoCarenza = ObjUtenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(
                                                enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_RACCOLTA_CARENZA_NON_RISPETTATA,
                                              objParametri_Utenti)

        Dim ListaImpianti As New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

        Dim AvvisoCarenza As String = ""

        For i = 0 To GridView_Rilievo.Rows.Count - 1

            If IsDate(GridView_Rilievo.Rows(i).Cells(8).Text.Substring(0, 10)) Then
                Dim dataCar As DateTime = GridView_Rilievo.Rows(i).Cells(8).Text.Substring(0, 10)
                App_Nome = GridView_Rilievo.Rows(i).Cells(0).Text
                Dim dat As Long = DateDiff(DateInterval.Day, objParametriAgenda.Data, dataCar)
                If dat > 0 Then
                    'CarenzaOk = False
                    'messaggio_errore = "Attenzione non è rispettato il tempo di carenza per l'impianto alla riga " & i + 1
                    AvvisoCarenza &= App_Nome & "(" & GridView_Rilievo.Rows(i).Cells(8).Text & "),"
                    'Return False
                End If
            End If

            Dim Impianto As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto
            Impianto.Piva = GridView_Rilievo.DataKeys(i).Item("Piva")
            Impianto.Sa_Cod = GridView_Rilievo.DataKeys(i).Item("Sa_Cod")
            Impianto.Appezza = GridView_Rilievo.DataKeys(i).Item("Appezza")
            Impianto.ID_Reg = GridView_Rilievo.DataKeys(i).Item("ID_Reg")
            Impianto.Progetto_Cod = GridView_Rilievo.DataKeys(i).Item("Progetto_Cod")
            Impianto.Veg_Cod = GridView_Rilievo.DataKeys(i).Item("Veg_Cod")
            Impianto.Cul_Cod = GridView_Rilievo.DataKeys(i).Item("Cul_Cod")
            Impianto.Cul_Des = (New AgronicaCoreMetaSchemaDAL.Cultivar_R()).CulDes_from_CulCod(Impianto.Cul_Cod, objParametri_Server)
            Dim qta As String = CType(GridView_Rilievo.Rows(i).Cells(7).Controls(1), TextBox).Text
            If IsNumeric(qta) AndAlso CInt(qta) >= 0 Then
                Impianto.Qta = CType(GridView_Rilievo.Rows(i).Cells(7).Controls(1), TextBox).Text
            Else
                Impianto.Qta = 0
            End If
            Impianto.Qta2 = CStr(GridView_Rilievo.DataKeys(i).Item("Sup"))

            totale = totale + Impianto.Qta
            ListaImpianti.Add(Impianto)

        Next

        Select Case BloccoCarenza
            Case "", "1"
                If AvvisoCarenza <> "" Then
                    messaggio_errore = "Attenzione non è rispettato il tempo di carenza per gli appezzamenti " & Left(AvvisoCarenza, AvvisoCarenza.Length - 1)
                    Return False
                End If
            Case "0"
                If AvvisoCarenza <> "" Then
                    messaggio_alert = AvvisoCarenza
                End If
                If messaggio_alert <> "" Then
                    If HiddenVarie.Value = "" Then
                        messaggio_alert = "Attenzione non è rispettato il tempo di carenza per gli appezzamenti " & Left(AvvisoCarenza, AvvisoCarenza.Length - 1)
                        Return False
                    End If
                    HiddenVarie.Value = ""
                End If
        End Select

        'recupero impianti dalla gridview
        If Not IsNumeric(QtaTot.Text) OrElse (IsNumeric(QtaTot.Text) AndAlso CDbl(QtaTot.Text) <> totale) Then
            messaggio_errore = "La quantità raccolta nella casella e la somma delle quantità non corrispondono"
            Return False
        End If



        Try


            '-----------------------------------------------------
            '----------- CONNESSIONE E TRANSAZIONE ---------------
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            '-----------------------------------------------------

            Dim BaseCode As Integer
            Dim TopCode As Integer

            Select Case objParametriAgenda.Sa_Cod

                '--------------------------------------------------
                '--------------------------------------------------
                '--------- OPERAZIONE MULTI-CENTRO    -------------
                '--------------------------------------------------
                '--------------------------------------------------
                Case "0"

                    ' ''apro transazione per scrittura multipla di movimenti
                    ' ''-----------------------------------------------------
                    ' ''----------- CONNESSIONE E TRANSAZIONE ---------------
                    ''Dim objDP As New AgronicaCoreDataProvider.ConnessioniTransazioni
                    ''ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
                    ' ''-----------------------------------------------------

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
                        Dim ListaOpAgendaCollegate As List(Of Operazione_Agenda)

                        Dim Agenda As Operazione_Agenda = CreaOggettoAgenda(ListaImpiantixQuestoSaCod, ListaSacod(i), messaggio_errore, ListaOpAgendaCollegate)

                        If IsNothing(Agenda) Then
                            Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                        End If

                        Dim objAgendaScrivi As New Agenda_Operazione_Helper

                        'se la scrittura è andata a buon fine e sono in modifica
                        'CANCELLO la vecchia operazione

                        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                            'Dal 06/23 conserviamo la data e l'username di creazione originali per tutti i record Agenda
                            allinea_DataUsernameCreazione(Agenda, objParametri_Server, Tipo_Attivita.QuadernoDiCampagna, 0)

                            Dim CancellataOperazione As Boolean = False
                            CancellataOperazione = objAgendaScrivi.Cancella(objParametriAgenda.Piva,
                                                                             objParametriAgenda.Sa_Cod,
                                                                             objParametriAgenda.Id_Agenda, False,
                                                                             objParametri_Server, logCancellazione:=False)


                        End If

                        Dim Id_Agenda As Integer
                        Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

                        BaseCode = Agenda.BaseCode
                        TopCode = Agenda.TopCode

                        'Operazioni collegate
                        'elimino le operazioni (i riferimenti non li cancella quando elimina l'operazione)
                        'perchè id_agenda è nel riferito non il principale
                        Dim Id_Agenda_Carico As Integer = 0
                        Dim Id_Agenda_Scarico As Integer = 0
                        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                            Dim x As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                            Dim dtrifScarico As DataTable = x.Leggi_Specifica("", 0, 0, 0, 0, LAVCOD_SCARICO, "",
                                                                       objParametriAgenda.Piva,
                                                                                objParametriAgenda.Sa_Cod,
                                                                                objParametriAgenda.Id_Agenda,
                                                                                -1, -1, LAVCOD_CURA, "",
                                                                                 "", "", objParametri_Server)
                            Dim dtrifCarico As DataTable = x.Leggi_Specifica("", 0, 0, 0, 0, LAVCOD_CARICO, "",
                                                        objParametriAgenda.Piva,
                                                        objParametriAgenda.Sa_Cod,
                                                        objParametriAgenda.Id_Agenda,
                                                        -1, -1, LAVCOD_CURA, "",
                                                         "", "", objParametri_Server)

                            If dtrifCarico.Rows.Count <> dtrifScarico.Rows.Count Then
                                Throw New Exception("Dovrebbe essere uguali")
                            End If
                            If dtrifCarico.Rows.Count <> 0 Then
                                If dtrifCarico.Rows.Count = 1 Then
                                    Id_Agenda_Carico = dtrifCarico.Rows(0).Item("Id_Agenda")
                                    Id_Agenda_Scarico = dtrifScarico.Rows(0).Item("Id_Agenda")
                                Else
                                    Throw New Exception("Dovrebbe essercene due o nessuno")
                                End If

                                'cancello le op linkate
                                Dim CancellataOperazione As Boolean = objAgendaScrivi.Cancella(
                                                                     dtrifCarico.Rows(0).Item("Piva"),
                                                                     dtrifCarico.Rows(0).Item("Sa_Cod"),
                                                                     dtrifCarico.Rows(0).Item("Id_Agenda"), False,
                                                                     objParametri_Server, logCancellazione:=False)


                                CancellataOperazione = objAgendaScrivi.Cancella(
                                                                     dtrifScarico.Rows(0).Item("Piva"),
                                                                     dtrifScarico.Rows(0).Item("Sa_Cod"),
                                                                     dtrifScarico.Rows(0).Item("Id_Agenda"), False,
                                                                     objParametri_Server, logCancellazione:=False)


                                'cancello tutti i riferimenti
                                Dim objMovimentiDettagliRif As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W
                                CancellataOperazione = objMovimentiDettagliRif.Cancella_byChiaveRif(objParametriAgenda.Piva,
                                                                                 objParametriAgenda.Sa_Cod,
                                                                                 Id_Agenda,
                                                                                  -1,
                                                                                -1,
                                                                                  "", objParametri_Server)

                            End If




                        End If



                        'Scrivo le op colegate e i riferimenti
                        If Not IsNothing(ListaOpAgendaCollegate) Then
                            For Each opcollegata As Operazione_Agenda In ListaOpAgendaCollegate
                                Dim Id_Agenda_Rif As Integer
                                ' Dim Id_Mov_Rif As Integer = opcollegata.Movimenti(0).Id_Mov

                                'dovrei cambiare gli id_agenda
                                Dim newid As Integer = 0
                                If opcollegata.Lav_Cod = LAVCOD_SCARICO Then
                                    newid = Id_Agenda_Scarico
                                End If
                                If opcollegata.Lav_Cod = LAVCOD_CARICO Then
                                    newid = Id_Agenda_Carico
                                End If
                                If newid <> 0 Then
                                    opcollegata.Id_Agenda = newid
                                    For Each mov As Movimento In opcollegata.Movimenti
                                        mov.Id_Agenda = newid
                                        For Each movd As Movimento_Dettaglio In mov.Movimenti_Dettagli
                                            movd.Id_Agenda = newid
                                            For Each movde As Movimento_Destinazione In movd.Movimenti_Destinazioni
                                                movde.Id_Agenda = newid
                                            Next
                                            For Each movdet As Movimento_Dettaglio_Tecnico In movd.Movimenti_Dettagli_Tecnici
                                                movdet.Id_Agenda = newid
                                            Next
                                        Next
                                    Next
                                End If

                                'crivo le op linkate
                                Id_Agenda_Rif = objAgendaScrivi.Scrivi(opcollegata, objParametri_Server)

                                'scrivo i riferimenti
                                Dim RifAg As New Movimento_Dettaglio_Riferimento
                                RifAg.Piva_Rif = Agenda.Piva
                                RifAg.Sa_Cod_Rif = Agenda.Sa_Cod
                                RifAg.Id_Agenda_Rif = Id_Agenda
                                RifAg.Id_Mov_Rif = -1
                                RifAg.Id_Mov_Det_Rif = -1
                                RifAg.Lav_Cod_Rif = Agenda.Lav_Cod
                                RifAg.Piva = opcollegata.Piva
                                RifAg.Sa_Cod = opcollegata.Sa_Cod
                                RifAg.Id_Agenda = Id_Agenda_Rif
                                RifAg.Id_Mov = -1
                                RifAg.Id_Mov_Det = -1
                                RifAg.Lav_Cod = opcollegata.Lav_Cod
                                Dim objRifScrivi As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                                objRifScrivi.Scrivi(RifAg, objParametri_Server)
                            Next
                        End If

                        objAgendaScrivi = Nothing

                    Next



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

                    Dim ListaOpAgendaCollegate As List(Of Operazione_Agenda)

                    Dim Agenda As Operazione_Agenda = CreaOggettoAgenda(ListaImpianti, objParametriAgenda.Sa_Cod, messaggio_errore, ListaOpAgendaCollegate)
                    If IsNothing(Agenda) Then
                        Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                    End If


                    Dim objAgendaScrivi As New Agenda_Operazione_Helper

                    'se la scrittura è andata a buon fine e sono in modifica
                    'CANCELLO la vecchia operazione

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

                    Dim Id_Agenda As Integer = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)


                    'Operazioni collegate
                    'elimino le operazioni (i riferimenti non li cancella quando elimina l'operazione)
                    'perchè id_agenda è nel riferito non il principale
                    Dim Id_Agenda_Carico As Integer = 0
                    Dim Id_Agenda_Scarico As Integer = 0
                    If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                        Dim x As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                        Dim dtrifScarico As DataTable = x.Leggi_Specifica("", 0, 0, 0, 0, LAVCOD_SCARICO, "",
                                                                   objParametriAgenda.Piva,
                                                                            objParametriAgenda.Sa_Cod,
                                                                            objParametriAgenda.Id_Agenda,
                                                                            -1, -1, LAVCOD_CURA, "",
                                                                             "", "", objParametri_Server)
                        Dim dtrifCarico As DataTable = x.Leggi_Specifica("", 0, 0, 0, 0, LAVCOD_CARICO, "",
                                                    objParametriAgenda.Piva,
                                                    objParametriAgenda.Sa_Cod,
                                                    objParametriAgenda.Id_Agenda,
                                                    -1, -1, LAVCOD_CURA, "",
                                                     "", "", objParametri_Server)

                        If dtrifCarico.Rows.Count <> dtrifScarico.Rows.Count Then
                            Throw New Exception("Dovrebbe essere uguali")
                        End If
                        If dtrifCarico.Rows.Count <> 0 Then
                            If dtrifCarico.Rows.Count = 1 Then
                                Id_Agenda_Carico = dtrifCarico.Rows(0).Item("Id_Agenda")
                                Id_Agenda_Scarico = dtrifScarico.Rows(0).Item("Id_Agenda")
                            Else
                                Throw New Exception("Dovrebbe essercene due o nessuno")
                            End If

                            'cancello le op linkate
                            Dim CancellataOperazione As Boolean = objAgendaScrivi.Cancella(
                                                                 dtrifCarico.Rows(0).Item("Piva"),
                                                                 dtrifCarico.Rows(0).Item("Sa_Cod"),
                                                                 dtrifCarico.Rows(0).Item("Id_Agenda"), False,
                                                                 objParametri_Server, logCancellazione:=False)


                            CancellataOperazione = objAgendaScrivi.Cancella(
                                                                 dtrifScarico.Rows(0).Item("Piva"),
                                                                 dtrifScarico.Rows(0).Item("Sa_Cod"),
                                                                 dtrifScarico.Rows(0).Item("Id_Agenda"), False,
                                                                 objParametri_Server, logCancellazione:=False)


                            'cancello tutti i riferimenti
                            Dim objMovimentiDettagliRif As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W
                            CancellataOperazione = objMovimentiDettagliRif.Cancella_byChiaveRif(objParametriAgenda.Piva,
                                                                             objParametriAgenda.Sa_Cod,
                                                                             Id_Agenda,
                                                                              -1,
                                                                            -1,
                                                                              "", objParametri_Server)

                        End If




                    End If



                    'Scrivo le op colegate e i riferimenti
                    If Not IsNothing(ListaOpAgendaCollegate) Then
                        For Each opcollegata As Operazione_Agenda In ListaOpAgendaCollegate
                            Dim Id_Agenda_Rif As Integer
                            ' Dim Id_Mov_Rif As Integer = opcollegata.Movimenti(0).Id_Mov

                            'dovrei cambiare gli id_agenda
                            Dim newid As Integer = 0
                            If opcollegata.Lav_Cod = LAVCOD_SCARICO Then
                                newid = Id_Agenda_Scarico
                            End If
                            If opcollegata.Lav_Cod = LAVCOD_CARICO Then
                                newid = Id_Agenda_Carico
                            End If
                            If newid <> 0 Then
                                opcollegata.Id_Agenda = newid
                                For Each mov As Movimento In opcollegata.Movimenti
                                    mov.Id_Agenda = newid
                                    For Each movd As Movimento_Dettaglio In mov.Movimenti_Dettagli
                                        movd.Id_Agenda = newid
                                        For Each movde As Movimento_Destinazione In movd.Movimenti_Destinazioni
                                            movde.Id_Agenda = newid
                                        Next
                                        For Each movdet As Movimento_Dettaglio_Tecnico In movd.Movimenti_Dettagli_Tecnici
                                            movdet.Id_Agenda = newid
                                        Next
                                    Next
                                Next
                            End If

                            'crivo le op linkate
                            Id_Agenda_Rif = objAgendaScrivi.Scrivi(opcollegata, objParametri_Server)

                            'scrivo i riferimenti
                            Dim RifAg As New Movimento_Dettaglio_Riferimento
                            RifAg.Piva_Rif = Agenda.Piva
                            RifAg.Sa_Cod_Rif = Agenda.Sa_Cod
                            RifAg.Id_Agenda_Rif = Id_Agenda
                            RifAg.Id_Mov_Rif = -1
                            RifAg.Id_Mov_Det_Rif = -1
                            RifAg.Lav_Cod_Rif = Agenda.Lav_Cod
                            RifAg.Piva = opcollegata.Piva
                            RifAg.Sa_Cod = opcollegata.Sa_Cod
                            RifAg.Id_Agenda = Id_Agenda_Rif
                            RifAg.Id_Mov = -1
                            RifAg.Id_Mov_Det = -1
                            RifAg.Lav_Cod = opcollegata.Lav_Cod
                            Dim objRifScrivi As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                            objRifScrivi.Scrivi(RifAg, objParametri_Server)
                        Next
                    End If

                    objAgendaScrivi = Nothing

                    If CType(Ricerca.FindControlIterative(Page.Master, "RBL_Salva"), RadioButtonList).SelectedValue = 3 Then
                        objParametriAgenda.Id_Agenda = Id_Agenda
                    End If
            End Select

            'AGGIORNAMENTO APPEZZAMENTI, IMPIANTI, ESERCIZI
            For i = 0 To ListaImpianti.Count - 1

                Select Case rbl_Chiusura.SelectedValue

                    Case "1", "2", "3"

                        Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
                        Dim DTAgenda As DataTable
                        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)
                        DTAgenda = ObjAgenda.LeggiCronologiaMovimenti(ListaImpianti(i).Piva,
                                                    ListaImpianti(i).Sa_Cod,
                                                    ListaImpianti(i).Appezza,
                                                    ListaImpianti(i).ID_Reg,
                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                    " Data_Movimento > " & Agro_SQL_SaveDate(CDate(objParametriAgenda.Data)) & " ",
                                                    "  Data_Movimento desc ",
                                                    objParametri_Server)
                        objParametri_Server.ResettaFinestra()

                        If DTAgenda.Rows.Count > 0 Then
                            'ci sono operazioni fatte successivamente alla chiusura NON CHIUDO
                        Else
                            '---CREAZIONE NOTA LOG
                            Dim objAgronicaLogAnagrafeW As New AgronicaCoreAnagrafeDAL.AgronicaLogAnagrafe_W
                            Dim NoteLog As String = componiNoteLog(rbl_Chiusura, Chk_NuovoEsercizio, Chk_NuovoImpianto)

                            'CHIUDO L'ESERCIZIO
                            Dim objProgetto As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_W
                            Select Case ListaImpianti(i).Progetto_Cod
                                Case 0
                                    'in modifica non è valorizzato
                                    objProgetto.Modifica_Validita_Fine2(ListaImpianti(i).Piva,
                                                           ListaImpianti(i).Sa_Cod,
                                                            ListaImpianti(i).Appezza,
                                                            ListaImpianti(i).ID_Reg,
                                                           objParametriAgenda.Data,
                                                           "",
                                                           objParametri_Server)
                                Case Else
                                    objProgetto.Modifica_Validita_Fine(ListaImpianti(i).Piva,
                                                           ListaImpianti(i).Progetto_Cod,
                                                           objParametriAgenda.Data,
                                                           "",
                                                           objParametri_Server)
                            End Select

                            objAgronicaLogAnagrafeW.Scrivi(enum_TipoOperazioneDB.Modifica,
                                                           enum_TipoEntita_Des.Progetti,
                                                           ListaImpianti(i).Piva, ListaImpianti(i).Progetto_Cod,
                                                           ListaImpianti(i).Sa_Cod,
                                                           ListaImpianti(i).Appezza,
                                                           ListaImpianti(i).ID_Reg,
                                                           Nothing,
                                                           NoteLog, enum_Id_Servizio.GiasOnline, objParametri_Server)


                            'CHIUDO L'IMPIANTO
                            Select Case rbl_Chiusura.SelectedValue
                                Case "2", "3"
                                    Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Write
                                    objImpianti.AggiornaValiditaFine(ListaImpianti(i).Piva,
                                                    ListaImpianti(i).Sa_Cod,
                                                    ListaImpianti(i).Appezza,
                                                    ListaImpianti(i).Campo_Cod,
                                                    ListaImpianti(i).ID_Reg,
                                                    objParametriAgenda.Data,
                                                    objParametri_Server)

                                    objAgronicaLogAnagrafeW.Scrivi(enum_TipoOperazioneDB.Modifica,
                                                           enum_TipoEntita_Des.Impianti,
                                                           ListaImpianti(i).Piva, ListaImpianti(i).Sa_Cod,
                                                           ListaImpianti(i).Appezza,
                                                           ListaImpianti(i).ID_Reg,
                                                           Nothing,
                                                           Nothing,
                                                           NoteLog, enum_Id_Servizio.GiasOnline, objParametri_Server)

                            End Select

                            'CHIUDO L'APPEZZAMENTO
                            Select Case rbl_Chiusura.SelectedValue
                                Case "3"
                                    Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Write
                                    Dim objUtentixAppezzamenti As New AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W
                                    Dim ObjPartW As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_W
                                    Dim ObjPartMacrW As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousi_W
                                    Dim ObjPartMacrUtW As New AgronicaCoreAnagrafeDAL.AppezzaxParticellexMacrousixUtilizzo_W

                                    objAppezza.AggiornaValiditaFine(ListaImpianti(i).Piva,
                                                                                         ListaImpianti(i).Sa_Cod,
                                                                                         ListaImpianti(i).Campo_Cod,
                                                                                         ListaImpianti(i).Appezza,
                                                                                         objParametriAgenda.Data,
                                                                                            "",
                                                                                            objParametri_Server)

                                    objUtentixAppezzamenti.AggiornaValiditaFine(
                                                                    ListaImpianti(i).Piva,
                                                                   ListaImpianti(i).Sa_Cod,
                                                                   ListaImpianti(i).Campo_Cod,
                                                                   ListaImpianti(i).Appezza,
                                                                   objParametriAgenda.Data,
                                                                    "",
                                                                    objParametri_Server)

                                    ObjPartW.AggiornaValiditaFine(ListaImpianti(i).Piva,
                                                                   ListaImpianti(i).Sa_Cod,
                                                                   ListaImpianti(i).Campo_Cod,
                                                                   ListaImpianti(i).Appezza,
                                                                   "", "", "", 0, 0, "",
                                                                   objParametriAgenda.Data,
                                                                      "",
                                                                      objParametri_Server)
                                    ObjPartMacrW.AggiornaValiditaFine(ListaImpianti(i).Piva,
                                                                       ListaImpianti(i).Sa_Cod,
                                                                       ListaImpianti(i).Campo_Cod,
                                                                       ListaImpianti(i).Appezza,
                                                                       "", "", "", 0, 0, "", "",
                                                                       objParametriAgenda.Data,
                                                                          "",
                                                                          objParametri_Server)

                                    ObjPartMacrUtW.AggiornaValiditaFine(ListaImpianti(i).Piva,
                                                                       ListaImpianti(i).Sa_Cod,
                                                                       ListaImpianti(i).Campo_Cod,
                                                                       ListaImpianti(i).Appezza,
                                                                       "", "", "", 0, 0, "", "", "", "",
                                                                       objParametriAgenda.Data,
                                                                          "",
                                                                          objParametri_Server)


                                    objAgronicaLogAnagrafeW.Scrivi(enum_TipoOperazioneDB.Modifica,
                                                           enum_TipoEntita_Des.Appezza,
                                                           ListaImpianti(i).Piva, ListaImpianti(i).Sa_Cod,
                                                           ListaImpianti(i).Appezza,
                                                           Nothing, Nothing, Nothing,
                                                           NoteLog, enum_Id_Servizio.GiasOnline, objParametri_Server)

                            End Select

                            'eventuale apertura nuovo esercizio / impianto
                            Dim ErroreNuovo As String = ""
                            Select Case rbl_Chiusura.SelectedValue
                                Case "1"
                                    If Chk_NuovoEsercizio.Checked Then
                                        Apertura_Progetto(ListaImpianti(i).Piva, ListaImpianti(i).Sa_Cod, ListaImpianti(i).Appezza, ListaImpianti(i).ID_Reg, ListaImpianti(i).Progetto_Cod, BaseCode, TopCode, ErroreNuovo, NoteLog)
                                        If ErroreNuovo <> "" Then
                                            Throw New Exception("Non è stato possibile creare il nuovo esercizio:" & ErroreNuovo)
                                        End If

                                    End If
                                Case "2"
                                    If Chk_NuovoImpianto.Checked Then
                                        AnagBiz.Apertura_Impianto(ListaImpianti(i).Piva, ListaImpianti(i).Sa_Cod, ListaImpianti(i).Appezza, ListaImpianti(i).ID_Reg, BaseCode, TopCode, ErroreNuovo, objParametri_Server, NoteLog)
                                        If ErroreNuovo <> "" Then
                                            Throw New Exception("Non è stato possibile creare il nuovo esercizio:" & ErroreNuovo)
                                        End If
                                    End If
                            End Select


                        End If

                End Select

            Next

            res = True

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

        Catch ex As Exception

            'commit transazione rollback
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            messaggio_errore = ex.Message
            res = False

        Finally

            'chiudi connessione
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try



        Return res

    End Function


    Private Sub gestisciTipoSalvataggio()

        Dim objParametriAgenda_Id_Agenda As Integer = objParametriAgenda.Id_Agenda
        objParametriAgenda.Id_Agenda = 0

        If ComboLavorazione.SelectedValue <> "0" AndAlso ComboLavorazione.SelectedValue <> "" Then
            'se sono in modifica non devo fare una nuova raccolta, 
            'per evitare errori e possibili duplicazioni non apro l'operazione di cura
            'anche se sarebbe corretto aprire l'op di cura precedentemente creata
            'di contro se sono in modifica di una raccolta che in precedenza era semplice
            'e aggiungo solo successivamente la cura non me la apre e devo 
            'andare nell'azienda di cura e aprirla a mano
            If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Or
              (objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica AndAlso
               Not IsNothing(Session("RaccoltaInModificaConOpCuraCollegata")) AndAlso
               Session("RaccoltaInModificaConOpCuraCollegata") = False) Then
                'devo saltare ad una operazione post raccolta
                Dim Operazione As Integer = CInt(ComboLavorazione.SelectedValue)
                Select Case Operazione
                    Case 1
                        'devo fare lo scarico dei prodotti dal'azienda e il carico nell'uds
                        'devo portarmi dietro poi i dati dell'uds e del centro
                        'e il lotto raccolto (o l'id agenda con cui dalla pagina ricavo il lotto)
                        Session("ObjparametriAgendaProvenienzaPiva") = objParametriAgenda.Piva
                        Session("ObjparametriAgendaProvenienzaSa_Cod") = objParametriAgenda.Sa_Cod
                        Session("ObjparametriAgendaProvenienzaVeg_Cod") = objParametriAgenda.Veg_Cod
                        Session("ObjparametriAgendaProvenienzaData") = objParametriAgenda.Data
                        Session("ObjparametriAgendaProvenienzaSa_Cod_magazzino") = Split(ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue, "|")(1)
                        Session("ObjparametriAgendaProvenienzafabbricato_Cod") = Split(ComboMagazzinoDestinazione.ddl_Magazzini.SelectedValue, "|")(0)
                        objParametriAgenda.Svuota_DatiOperazione()
                        objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Raccolta
                        objParametriAgenda.Id_Agenda = 0
                        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
                        objParametriAgenda.TipoOperazioneAgenda = TipiEnumerativi.enum_Tipo_Operazione_Agenda.QuadernoDiCampagna
                        objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
                        objParametriAgenda.Programmazione_Cod = 0
                        objParametriAgenda.Lav_Cod = LAVCOD_CURA
                        objParametriAgenda.Piva = ComboUDS.SelectedValue
                        objParametriAgenda.Sa_Cod = ComboCentroCura.SelectedValue
                        objParametriAgenda.Data = txt_DataMagazzino.Text
                        objParametriAgenda.OperazioneMulticentro = False
                        Dim Elem_Cod As Integer = TRASFORMATI_VEGETALI
                        If CheckBoxSemilavorati.Checked Then
                            Elem_Cod = SEMILAVORATI_VEGETALI
                        End If
                        objParametriAgenda.Veg_Cod = Session("ObjparametriAgendaProvenienzaVeg_Cod")
                        objParametriAgenda.Elem_Cod = Elem_Cod
                        objParametriAgenda.Lotto = LottoMagazzinoSalvato
                        objParametriAgenda.RagSoc = ""
                        Response.Redirect("../GestioneMagazzini/OperazioneDiCura.aspx")
                End Select
            End If

        End If


        Session("UtilizzataRicetta") = False
        Dim TipoSalvataggio As String = Master_Operazione.Property_RBL_Salva.SelectedValue + 1
        Select Case TipoSalvataggio
            Case enum_Tipo_Salvataggio.Salva_e_Esci

                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")

                If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 AndAlso
                        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Verifica_Conformita Then
                    strJS.AppendLine("      window.close(); ")
                End If

                strJS.AppendLine("      ChiamataParent_Id_Ageda(" & objParametriAgenda.Id_Agenda & "); ")

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

                strJS.AppendLine("      window.location = '" & link & "'; ")
                strJS.AppendLine(" });")
                ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

                RipristinaSessione()
                'Response.Redirect(CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda))





            Case enum_Tipo_Salvataggio.Salva_e_Nuovo

                Giacenza_SI_NO.Value = "0"
                objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                objParametriAgenda.Note = New List(Of Nota)
                objParametriAgenda.Movimenti = New List(Of Movimento)





                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                  Master_Operazione.Property_UpdatePanelToolBar)

                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")
                'strJS.AppendLine("      alert(''); ")
                strJS.AppendLine("      window.location = '../Operazioni/Raccolta.aspx'; ")
                strJS.AppendLine(" });")

                ScriptManager.RegisterClientScriptBlock(Master_Operazione.Property_UpdatePanelToolBar,
                                                    Master_Operazione.Property_UpdatePanelToolBar.GetType(),
                                              String.Format("jQuery_{0}", Master_Operazione.Property_UpdatePanelToolBar.ClientID),
                                              strJS.ToString, True)






            Case enum_Tipo_Salvataggio.Salva_e_Duplica
                Giacenza_SI_NO.Value = "0"

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                       Master_Operazione.Property_UpdatePanelToolBar)

            Case enum_Tipo_Salvataggio.Salva_e_Vai_ai_Costi

                Dim PaginaLink As String = "../AnalisiCostiProduzione/GestioneCosti.aspx"

                Dim link As String = ""

                Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda.SitoOrigine

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
                          "&id_agenda=" & Stringa_Codifica(objParametriAgenda_Id_Agenda, AgroKey_EncoderDecoder) &
                          IIf(sitoorigine = Enum_SiteRedirector.GiasNG, "", "&origine=" & Stringa_Codifica(link, AgroKey_EncoderDecoder)) &
                          "&entrata_diretta=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Nothing)

                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")
                strJS.AppendLine("      ChiamataParent_Id_Ageda(" & objParametriAgenda_Id_Agenda & "); ")

                If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 AndAlso objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Verifica_Conformita Then
                    strJS.AppendLine("      window.close(); ")
                Else
                    strJS.AppendLine("      window.location = '" & PaginaLink & "'; ")
                End If

                strJS.AppendLine(" });")

                ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                  String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

                RipristinaSessione()


        End Select
    End Sub

    Private Function GeneraProdotto(Veg_Cod As Integer, Veg_Des As String, ListaCul_Cod As List(Of Integer),
                                   Elem_Cod As Integer, ByRef Mat_Des_Ritorno As String) As String

        'GENERO PRODOTTO SE NON INDICATO (per ora solo sulla specie, varietà altre)
        If Elem_Cod <> TRASFORMATI_VEGETALI AndAlso Elem_Cod <> SEMILAVORATI_VEGETALI Then
            Throw New Exception
        End If

        Dim OUTPUT_Mat_Cod As Integer

        Dim Piva_Creazione As String = objParametri_Server.PivaSuperUser 'azienda che lo crea
        Dim Sa_Cod_Creazione As Integer = PUBBLICO 'pubblico
        Dim Cul_Cod As Integer = New AgronicaCoreMetaSchemaDAL.Cultivar_R().CulCod_Altre_from_VegCod(Veg_Cod, objParametri_Server)
        If Cul_Cod < 1 Then
            Throw New Exception("non c'è la varietà altre")
        End If
        Dim objImportaGias As New AgronicaCoreAnagrafeBIZ.Importa_GIAS
        Dim Regolamento As enum_Cod_Regolamento = enum_Cod_Regolamento.Regolamento_Nessuno
        Dim Flag_Biologico As Boolean = False

        Dim Cod_Articolo As String = "RACC/" & Right("000" & Veg_Cod, 3) &
                       "/" & Right("00000000" & CStr(Cul_Cod), 8)

        Mat_Des_Ritorno = Veg_Des & " - Altre"

        Dim objCore_MP_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim flag_esiste As Boolean = True
        If Elem_Cod = SEMILAVORATI_VEGETALI Then
            flag_esiste = objCore_MP_R.Esiste_SemilavoratoVegetale(Piva_Creazione,
                                                              Veg_Cod,
                                                              Cul_Cod,
                                                              0,
                                                               " Sa_Cod = -1 ",
                                                               objParametri_Server)
        Else
            flag_esiste = objCore_MP_R.Esiste_TrasformatoVegetale(Piva_Creazione,
                                                  Veg_Cod,
                                                  Cul_Cod,
                                                  0,
                                                   " Sa_Cod = -1 ",
                                                   objParametri_Server)
        End If


        If Not flag_esiste Then

            Dim objCore_XML_Anagrafe As New AgronicaCoreXML.XML_Anagrafe
            Dim objCore_MP_W As New AgronicaCoreAnagrafeBIZ.Materie_Prime_W
            Dim Flag_Insert As Boolean
            Dim Basecode As Integer = 0
            Dim Topcode As Integer = 0
            AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(Basecode,
                                                                   Topcode,
                                                                    Session("ASG_ProgressivoGIAS"))


            If Elem_Cod = SEMILAVORATI_VEGETALI Then
                objImportaGias.Creazione_Automatica_SemilavoratoVegetale(objParametri_Server,
                                                        objCore_XML_Anagrafe,
                                                         objCore_MP_W,
                                                         Flag_Insert,
                                                         OUTPUT_Mat_Cod,
                                                         Basecode,
                                                         Topcode,
                                                         Piva_Creazione,
                                                         Sa_Cod_Creazione,
                                                         Cod_Articolo,
                                                         Mat_Des_Ritorno,
                                                         Veg_Cod,
                                                         Cul_Cod,
                                                         Regolamento,
                                                         Flag_Biologico)
            Else
                objImportaGias.Creazione_Automatica_TrasformatoVegetale(objParametri_Server,
                                                        objCore_XML_Anagrafe,
                                                         objCore_MP_W,
                                                         Flag_Insert,
                                                         OUTPUT_Mat_Cod,
                                                         Basecode,
                                                         Topcode,
                                                         Piva_Creazione,
                                                         Sa_Cod_Creazione,
                                                         Cod_Articolo,
                                                         Mat_Des_Ritorno,
                                                         Veg_Cod,
                                                         Cul_Cod,
                                                         Regolamento,
                                                         Flag_Biologico)
            End If

        Else
            Dim dt As DataTable = objCore_MP_R.Leggi(Piva_Creazione, -1, Elem_Cod, 0, "",
                                                   Veg_Cod,
                                                   Cul_Cod,
                                                   0, 0, 0, 0, 0, "", 0, "", False, False, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            OUTPUT_Mat_Cod = dt.Rows(0).Item("Mat_Cod")

        End If


        Return OUTPUT_Mat_Cod

    End Function

#End Region



#Region "Lavorazione post raccolta"

    Private Sub caricaComboLavorazione()
        ComboLavorazione.Items.Clear()
        ComboLavorazione.Items.Add(New ListItem("Nessuna", ""))
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitato As Boolean = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"),
                                    Session("ASG_IdServizio"),
                                    enum_Security_Attivita.Agenda_Operazione_Di_Cura,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)
        If UtenteAbilitato Then
            ComboLavorazione.Items.Add(New ListItem("Operazione di Cura Tabacco", "1"))
        End If

    End Sub

    Protected Sub ComboLavorazione_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboLavorazione.SelectedIndexChanged
        CambiataLavorazione()
    End Sub

    Private Sub CambiataLavorazione()

        Div2.Visible = False

        Dim lavorazione As String = ComboLavorazione.SelectedValue
        Select Case lavorazione
            Case "1"
                'tabacco
                Div2.Visible = True
                caricaComboUDS()
                caricaComboCentroCura()


            Case ""
                'nessuna

            Case Else
                Throw New Exception("Non previsto")
        End Select


    End Sub

#End Region



#Region "Cura Tabacco"

    Private Sub caricaComboUDS()
        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim ListaPivaUds As String = objFabbricati.Lista_piva_Con_Forni(objParametri_Server)
        AgronicaCoreUtility.CaricaListControl.Imprese(ComboUDS, True, "", "-1",
                                                              "", " imprese.piva in (" & ListaPivaUds & ") ", "", objParametri_Server)
        'preseleziono l'uds aziendale se presente
        ComboUDS.SelectedValue = objParametriAgenda.Piva
        If ComboUDS.Items.Count = 2 Then
            ComboUDS.SelectedIndex = 1
        End If
    End Sub

    Protected Sub ComboUDS_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboUDS.SelectedIndexChanged
        caricaComboCentroCura()
        'caricaComboForno()
    End Sub

    Private Sub caricaComboCentroCura()
        Dim objFabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim ListaSaCodUds As String = objFabbricati.Lista_Sacod_Con_Forni(ComboUDS.SelectedValue, objParametri_Server)
        AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(ComboCentroCura, True, "", "-1",
                                                ComboUDS.SelectedValue,
                                                True, 2, " Centri_Aziendali.Sa_Cod in (" & ListaSaCodUds & ") ", "", objParametri_Server)
        If ComboCentroCura.Items.Count = 2 Then
            ComboCentroCura.SelectedIndex = 1
        End If
    End Sub

#End Region




#Region "Varie"

    'Private Function XML_GeneraBlocco_RaccoltoCampionatura( _
    '                                    ByVal IndiceCalibro As Integer, _
    '                                    ByVal IndiceImpianto As Integer) _
    '                                    As String

    '    Dim StringaXML As String = ""
    '    Dim StringoneXML As String = ""
    '    Dim j As Integer

    '    If MatrixQuantita(IndiceCalibro, IndiceImpianto) <> 0 Then

    '        '--------------
    '        'NODO CALIBRO
    '        StringaXML = XML_Agenda_RaccoltoCampionatura( _
    '                         enum_TipoOperazioneDB.Scrittura, _
    '                         , _
    '                         "calibro", _
    '                         CInt(Me.DataGrid_Scheda.Items(IndiceCalibro).Cells(2).Text), _
    '                         , _
    '                         , _
    '                         CStr(Me.DataGrid_Scheda.Items(IndiceCalibro).Cells(3).Text), _
    '                         frm_DataOperazione, _
    '                         #12/31/2100#, _
    '                         frm_BaseCode, _
    '                         frm_TopCode)

    '        StringoneXML = StringoneXML + StringaXML


    '        '--------------
    '        'NODI INDICI MATURITA'
    '        For j = 0 To Me.DataGrid_ParametriQualitativi.Items.Count - 1

    '            If IsNumeric(CType(Me.DataGrid_ParametriQualitativi.Items(j).FindControl("Text_Rilievo"), TextBox).Text) Then

    '                StringaXML = XML_Agenda_RaccoltoCampionatura( _
    '                                 enum_TipoOperazioneDB.Scrittura, _
    '                                 , _
    '                                 "indice", _
    '                                 CInt(Me.DataGrid_ParametriQualitativi.Items(j).Cells(0).Text), _
    '                                 CInt(Me.DataGrid_ParametriQualitativi.Items(j).Cells(2).Text), _
    '                                 CType(Me.DataGrid_ParametriQualitativi.Items(j).FindControl("Text_Rilievo"), TextBox).Text, _
    '                                 CStr(Me.DataGrid_ParametriQualitativi.Items(j).Cells(1).Text), _
    '                                 frm_DataOperazione, _
    '                                 #12/31/2100#, _
    '                                 frm_BaseCode, _
    '                                 frm_TopCode)

    '                StringoneXML = StringoneXML + StringaXML

    '            End If

    '        Next


    '    End If


    '    Return StringoneXML

    'End Function
    'Public Function XML_Agenda_RaccoltoCampionatura( _
    '                 ByVal TipoOperazioneDB As enum_TipoOperazioneDB, _
    '                 Optional ByVal Progressivo As Integer = 0, _
    '                 Optional ByVal Tipo As String = "calibro", _
    '                 Optional ByVal Tipo_Cod As Integer = 0, _
    '                 Optional ByVal Udm_Cod As Integer = 0, _
    '                 Optional ByVal Val_Cod As String = "0", _
    '                 Optional ByVal Descrizione As String = "", _
    '                 Optional ByVal Validita_Inizio As Date = #1/1/1900#, _
    '                 Optional ByVal Validita_Fine As Date = #12/31/2100#, _
    '                 Optional ByVal BaseCode As Integer = 0, _
    '                 Optional ByVal TopCode As Integer = 200000000) _
    '                 As String

    '    Dim XmlDoc As New System.Xml.XmlDocument
    '    Dim XmlTxt As System.Xml.XmlElement

    '    '----- Genero la stringa XML a partire dai valori dei parametri

    '    'Creo il nodo 
    '    XmlTxt = XmlDoc.CreateElement("Raccolto_Campionatura")

    '    'Imposto gli attributi
    '    XmlTxt.SetAttribute("TipoOperazioneDB", CStr(TipoOperazioneDB))
    '    XmlTxt.SetAttribute(LCase("progressivo"), Progressivo)
    '    XmlTxt.SetAttribute(LCase("tipo"), CStr(Tipo))
    '    XmlTxt.SetAttribute(LCase("tipo_cod"), CStr(Tipo_Cod))
    '    XmlTxt.SetAttribute(LCase("udm_cod"), CStr(Udm_Cod))
    '    XmlTxt.SetAttribute(LCase("val_cod"), CStr(Val_Cod))
    '    XmlTxt.SetAttribute(LCase("descrizione"), CStr(Descrizione))
    '    XmlTxt.SetAttribute(LCase("validita_inizio"), Format(Validita_Inizio, "dd/MM/yyyy"))
    '    XmlTxt.SetAttribute(LCase("validita_fine"), Format(Validita_Fine, "dd/MM/yyyy"))
    '    XmlTxt.SetAttribute(LCase("basecode"), CStr(BaseCode))
    '    XmlTxt.SetAttribute(LCase("topcode"), CStr(TopCode))

    '    'Imposto XmlTxt come figlio del documento principale
    '    XmlDoc.AppendChild(XmlTxt)

    '    'Restituisco in uscita la stringa creata
    '    Return XmlDoc.InnerXml

    '    'Distruggo gli oggetti
    '    XmlTxt = Nothing
    '    XmlDoc = Nothing

    'End Function
    Private Function GeneraCampionatura() As Integer
        Dim Progr As Integer
        'il cal_cod dell'operazione ha il 12 , indefinito in Materie_Prime_Calibri
        'il cal_cod del carico invece ha il codice 'progressivo' nella tabella Materie_Prime_Campionature
        'viene creato uno nuovo ad ogni salvataggio,
        'vedi nota nel 2003:
        '' '' '' '' '' '' '' '' ''MODIFICA IN DATA 23/07/2009 BY MAGA:
        '' '' '' '' '' '' '' '' ''se il lavorato raccolto è stato utilizzato in scarichi e/o bolle emesse e/o fatture emesse
        '' '' '' '' '' '' '' '' ''(ovvero con causale <> conferimento)
        '' '' '' '' '' '' '' '' ''andando a modificare la raccolta, il cal_cod viene rigenerato, per cui cambia la chiave del prodotto
        '' '' '' '' '' '' '' '' ''(composta da elem_cod,pro_cod,mat_cod,cod_progetto,fase_cod,lotto,cal_cod,udm_cod)
        '' '' '' '' '' '' '' '' ''e di conseguenza si disallinea la giacenza: risulta caricato un prodotto, mentre ne viene scaricato un altro

        '' '' '' '' '' '' '' '' ''Introdotto, quindi, il controllo sul prodotto: se risultano presenti scarichi o bolle emesse o fatture emesse di esso
        '' '' '' '' '' '' '' '' ''IL SALVATAGGIO VIENE DISABILITATO e viene suggerito di cancellare prima l'operazione di uscita del prodotto.
        'viene creato automaticapete dal biz, e nel 2003 viene attaccato l'xml al dettaglio con XML_Agenda_RaccoltoCampionatura,
        'il biz crea la nuova voce.
        'agenda non ha il biz, quindi mi tocca creare la voce ora a mano e naturalmente con indefinito
        'che schifezza l'online 2003, il lan, le tabelle agenda e compagnia bella
        Dim Materie_Prime_Campionature As New AgronicaCoreContabDAL.Materie_Prime_Campion_W
        Dim ObjSequenze = New AgronicaCoreDataProvider.Agro_Sequenze

        Progr = ObjSequenze.NuovoId_Tabella(
                    "Materie_Prime_Campionature",
                    0,
                    2000000000,
                    objParametri_Server)
        Progr = -Math.Abs(Progr)
        Materie_Prime_Campionature.Scrivi(
                                                Progr,
                                                "calibro",
                                                12,
                                                0,
                                                0,
                                                "Indefinito",
                                                0,
                                                0,
                                                "",
                                                CDate(txt_DataMagazzino.Text),
                                                AGRODATAFINE,
                                                objParametri_Server)
        Return Progr
    End Function


    Private Function GeneraIdLottoRaccolta(ByVal piva As String) As String

        '' ''Id univoco della infornatura di un prodotto del magazzino,
        '' ''lo uso per indicare il lotto del carico e scarico dal forno del prodotto tabacco in cura

        ' ''Dim seq As New AgronicaCoreDataProvider.Agro_Sequenze
        ' ''Dim id As Integer = seq.NuovoId_Tabella("GeneraIdLottoRaccolta", 0, 2000000000, objParametri_Server)

        '' ''Il codice anche se ha dei valori parlanti non deve essere parlante, basta ce ne sia uno univoco
        '' ''i dati importati da arpt avranno in testata 14-02- e il codice loro di 10 cifre ricavato dall'azienda e progressivo loro aziendale
        ' ''Dim risp As String = CStr(id).PadLeft(10, "0") '10 caratteri per id
        ' ''risp = "02" & risp '2 caratteri per l'op non arpt, metto 02
        ' ''risp = "LR" & risp 'lotto raccolta

        'Id univoco della infornatura di un prodotto del magazzino,
        'lo uso per indicare il lotto del carico e scarico dal forno del prodotto tabacco in cura

        'Dim datas As String = data.Year & "" & data.Month.ToString.PadLeft(2, "0") & "" & data.Day.ToString.PadLeft(2, "0") & ""

        'seconda versione, lotto parlante con piva
        Dim seq As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim id As Integer = seq.NuovoId_Tabella("GeneraIdLottoRaccolta", 0, 2000000000, objParametri_Server)

        'Il codice anche se ha dei valori parlanti non deve essere parlante, basta ce ne sia uno univoco
        'i dati importati da arpt avranno in testata 14-02- e il codice loro di 10 cifre ricavato dall'azienda e progressivo loro aziendale
        Dim risp As String = CStr(id).PadLeft(10, "0") '10 caratteri per id
        risp = risp & "/" & piva
        risp = "LR/" & risp 'Lotto Raccolta
        Return risp

    End Function


#End Region


    Protected Sub verificaCarenza()

        'If Not CheckBoxCarenza.Checked Then
        '    Exit Sub
        'End If

        If objParametriAgenda.Veg_Cod.Split("/")(0) = "-1" OrElse objParametriAgenda.Veg_Cod.Split("/")(0) = "0" Then
            Exit Sub
        End If



        Dim Piva As String
        Dim Sa_Cod As Integer
        Dim Appezza As Integer
        Dim ID_Reg As Integer
        Dim ObjDPI As New AgronicaCoreDpiBIZ.DPI_Verifica
        Dim Dummy As String
        Dim Validita_Inizio As Date
        Dim Validita_Fine As Date
        Dim Err_Code As Short
        Dim impresprog As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R

        Dim Grfi_Cod As Integer
        Dim Cop_Cod As Integer

        For i = 0 To GridView_Rilievo.Rows.Count - 1

            Piva = GridView_Rilievo.DataKeys(i).Item("Piva")
            Sa_Cod = GridView_Rilievo.DataKeys(i).Item("Sa_Cod")
            Appezza = GridView_Rilievo.DataKeys(i).Item("Appezza")
            ID_Reg = GridView_Rilievo.DataKeys(i).Item("ID_Reg")


            Dim dt2 As DataTable = impresprog.LeggiDistinta_Attiva_inData(
                                                CStr(Piva),
                                                CInt(Sa_Cod),
                                                CInt(Appezza),
                                                CInt(ID_Reg),
                                                CDate(objParametriAgenda.Data),
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "", "", objParametri_Server)

            Validita_Inizio = CDate(dt2.Rows(0).Item("Validita_Inizio"))
            Validita_Fine = CDate(dt2.Rows(0).Item("Validita_Fine"))

            Grfi_Cod = GridView_Rilievo.DataKeys(i).Item("Grfi_Cod")
            Cop_Cod = GridView_Rilievo.DataKeys(i).Item("Cop_Cod")

            Dummy = ObjDPI.DPI_Verifica_Carenza(Err_Code,
                                                CStr(Piva),
                                                CInt(Sa_Cod),
                                                CInt(Appezza),
                                                CInt(ID_Reg),
                                                CInt(objParametriAgenda.Veg_Cod.Split("/")(0)),
                                                Grfi_Cod,
                                                Cop_Cod,
                                                Validita_Inizio,
                                                Validita_Fine,
                                                CDate(objParametriAgenda.Data),
                                                objParametri_Server,
                                                objParametri_Utenti)


            GridView_Rilievo.Rows(i).Cells(8).Text = Dummy

        Next

    End Sub



    Private Function Apertura_Progetto(ByVal Piva As String, ByVal Sa_Cod As Integer,
                                       ByVal Appezza As Integer, ByVal Id_Reg As Integer,
                                       ByVal Progetto_Cod As Integer,
                                       ByVal BaseCode As Integer, ByVal TopCode As Integer,
                                       ByRef Errore As String,
                                       Optional NoteLog As String = "") As Boolean



        Dim DesChiaveImpianto As String
        Dim objProgRead As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
        'Dim objRegCodiciRead As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
        Dim objProgWrite As New AgronicaCoreAnagrafeBIZ.Progetto_W
        Dim objXML As New AgronicaCoreXML.XML_Anagrafe
        'Dim objXMLUt As New AgronicaCoreXML.XML_Utility
        Dim DT, DT_Codici As DataTable
        'Dim DT_CapPri As DataTable
        Dim StringaXmlCreazione As String
        Dim flag_insert = True
        Dim OUT_Piva As String = ""
        Dim OUT_Progetto_Cod As Integer
        Dim XML_DatiProgetto As XmlElement
        Dim XmlDoc As XmlDocument

        Dim Validita_fine_impianto As Date

        'vecchi dati distinta
        Dim OLD_Progetto_Cod As Integer
        Dim p_ha As Double
        Dim regolamento_cod As Integer
        Dim stato_impianto As Integer
        Dim data_inizio_prevista As Date
        Dim data_fine_prevista As Date
        Dim data_inizio_prevista_prec As Date
        Dim data_fine_prevista_prec As Date
        Dim produzione_prevista As Double
        Dim ricavi_previsti As Double
        Dim data_inizio_prec As Date
        Dim data_fine_prec As Date
        'Dim capitolato_privato As String

        Dim data_fioritura_prevista As Date
        Dim data_fioritura_prevista_prec As Date

        'nuovi dati distinta
        Dim data_inizio As Date
        Dim data_fine As Date
        Dim progetto_nome As String
        Dim progetto_des As String

        'leggo i dati della distinta più vecchia (l'ultima)
        DT = objProgRead.Leggi(Piva,
                                Progetto_Cod,
                                CAU_PROGETTO_PRODUZIONE,
                                0,
                                Sa_Cod,
                                Appezza,
                                Id_Reg,
                                0, 0,
                                enumSelezioneVariabile.Selezione_JoinCompleta,
                                "",
                                " Imprese_Progetti.Validita_Inizio DESC",
                                objParametri_Server)

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            With DT.Rows(0)
                OLD_Progetto_Cod = .Item("Progetto_Cod")
                p_ha = .Item("p_ha")
                regolamento_cod = .Item("regolamento_cod")
                stato_impianto = .Item("stato_impianto")
                data_inizio_prevista_prec = .Item("data_inizio_prevista")
                data_fine_prevista_prec = .Item("data_fine_prevista")
                produzione_prevista = .Item("produzione_prevista")
                ricavi_previsti = .Item("ricavi_previsti")
                data_inizio_prec = .Item("validita_inizio")
                data_fine_prec = .Item("validita_fine")
                data_fioritura_prevista_prec = .Item("Data_Fioritura_Prevista")
                Validita_fine_impianto = .Item("Fine_Impianto")
            End With

            'credo le nuove date e il lotto

            'imposto la data inizio come la data fine della distinta precedente piu un giorno
            data_inizio = DateAdd(DateInterval.Day, 1, data_fine_prec)

            If data_inizio > Validita_fine_impianto Then
                'errore!!!! non dovrebbe mai finire qui
                Errore += DesChiaveImpianto & "errore, la data inizio del nuovo esercizio supera la data di fine impianto!" & vbCrLf
                Return False
            End If

            'la edit impianto fa: imposto la data fine come la data fine della distinta precedente + 1 anno
            'che è uguale a fare la data inizio + 1 anno - 1 giorno
            data_fine = DateAdd(DateInterval.Year, 1, data_fine_prec)

            If data_fine > Validita_fine_impianto Then
                'la data fine della distinta non può superare la data fine dell'impianto
                'quindi la imposto uguale alla data fine impianto
                data_fine = Validita_fine_impianto
            End If

            'data semina prevista
            If data_inizio_prevista_prec <> AGRODATAINIZIO Then
                data_inizio_prevista = DateAdd(DateInterval.Year, 1, data_inizio_prevista_prec)
            Else
                data_inizio_prevista = AGRODATAINIZIO
            End If

            'data raccolta prevista
            If data_fine_prevista_prec <> AGRODATAFINE Then
                data_fine_prevista = DateAdd(DateInterval.Year, 1, data_fine_prevista_prec)
            Else
                data_fine_prevista = AGRODATAFINE
            End If

            'data fioritura prevista
            If data_fioritura_prevista_prec <> AGRODATAINIZIO Then
                data_fioritura_prevista = DateAdd(DateInterval.Year, 1, data_fioritura_prevista_prec)
            Else
                data_fioritura_prevista = AGRODATAINIZIO
            End If

            'lotto
            If data_inizio.Year <> data_fine.Year Then
                progetto_nome = "Lotto " & CStr(data_inizio.Year) & "/" & CStr(data_fine.Year)
            Else
                progetto_nome = "Lotto " & CStr(data_inizio.Year)
            End If

            progetto_des = progetto_nome

            ''leggo il capitolato privato che è un codice associato alla distinta
            'DT_CapPri = objRegCodiciRead.LeggixProgetto(Piva, _
            '                                            Sa_Cod, _
            '                                            Appezza, _
            '                                            Id_Reg, _
            '                                            "", _
            '                                            OLD_Progetto_Cod, _
            '                                            enum_CodiciAnagrafe.Capitolato_Privato, _
            '                                            "", _
            '                                            enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
            '                                            "", _
            '                                            "", _
            '                                            objParametri_Server)

            'If Not IsNothing(DT_CapPri) AndAlso DT_CapPri.Rows.Count > 0 Then
            '    capitolato_privato = DT_CapPri.Rows(0).Item("val_cod")
            '    DT_Codici = objXMLUt.CaricaGriglia_CodiciProgetto_for_XML
            '    objXMLUt.Inserisci_Riga_Dt_CodiciProgetto_for_XML(DT_Codici, _
            '                                                        enum_TipoOperazioneDB.Scrittura, _
            '                                                        Piva, _
            '                                                        Sa_Cod, _
            '                                                        Appezza, _
            '                                                        Id_Reg, _
            '                                                        0, _
            '                                                        enum_CodiciAnagrafe.Capitolato_Privato, _
            '                                                        capitolato_privato, _
            '                                                        data_inizio, _
            '                                                        data_fine)
            'Else
            '    DT_Codici = Nothing
            'End If


            'XML DISTINTA

            XmlDoc = New XmlDocument

            XML_DatiProgetto = objXML.XML_ProgettiImpianto(Errore,
                                        XmlDoc,
                                        False,
                                        BaseCode,
                                        TopCode,
                                        DT_Codici,
                                        enum_TipoOperazioneDB.Scrittura,
                                        Piva,
                                        Sa_Cod,
                                        Appezza,
                                        Id_Reg,
                                        0,
                                        progetto_nome,
                                        progetto_des,
                                        CAU_PROGETTO_PRODUZIONE,
                                        data_inizio,
                                        data_fine,
                                        0,
                                        0,
                                        ricavi_previsti,
                                        produzione_prevista,
                                        "",
                                        0,
                                        0,
                                        0,
                                        stato_impianto,
                                        regolamento_cod,
                                        0,
                                        0,
                                        0,
                                        data_inizio_prevista,
                                        data_fine_prevista,
                                        p_ha,
                                        data_fioritura_prevista)

            If Errore <> "" Then
                Return False
            End If

            StringaXmlCreazione = XML_DatiProgetto.OuterXml()

            If StringaXmlCreazione <> "" Then

                'salvo la nuova distinta

                Try

                    '-----------------------------------------------------
                    '--------------- SCRITTURA DISTINTA --------------------
                    flag_insert = objProgWrite.Impresa_Progetto_Scrivi(
                        CStr(StringaXmlCreazione),
                        OUT_Piva,
                        OUT_Progetto_Cod,
                        objParametri_Server,
                        NoteLog:=NoteLog)

                    '-----------------------------------------------------

                Catch ex As Exception
                    Errore &= ex.Message & vbCrLf
                    Return False
                End Try

            Else
                Errore &= DesChiaveImpianto & "errore durante la creazione dei dati del nuovo esercizio." & vbCrLf
                Return False
            End If


        Else
            Errore &= DesChiaveImpianto & "errore durante la lettura dei dati del nuovo esercizio." & vbCrLf
            Return False
        End If


        If Errore <> "" Then
            Return False
        End If

        objProgWrite = Nothing

        Return True

    End Function

    Private Shared Function componiNoteLog(rbl_Chiusura As RadioButtonList, Chk_NuovoEsercizio As CheckBox, Chk_NuovoImpianto As CheckBox) As String
        Dim NoteLog As String = "Raccolta.aspx:"


        If Chk_NuovoEsercizio.Checked AndAlso rbl_Chiusura.SelectedValue = "1" Then
            NoteLog += " chiusura e apertura esercizi"
        Else
            NoteLog += " chiusura esercizi"
        End If


        If rbl_Chiusura.SelectedValue = "2" Then
            If Chk_NuovoImpianto.Checked Then
                NoteLog += ", chiusura e apertura impianti"
            Else
                NoteLog += ", chiusura impianti"
            End If
        End If

        If rbl_Chiusura.SelectedValue = "3" Then
            NoteLog += ", chiusura impianti, chiusura appezzamenti"
        End If

        Return NoteLog
    End Function

End Class

