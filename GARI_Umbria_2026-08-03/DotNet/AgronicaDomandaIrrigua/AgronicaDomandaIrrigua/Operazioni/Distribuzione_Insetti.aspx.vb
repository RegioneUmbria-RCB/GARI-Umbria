Imports System.Web

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility

Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModelsSTD.attivita.Attivita

Public Class Distribuzione_Insetti
    Inherits System.Web.UI.Page

    Public Master_Operazione As Operazione
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda
    Dim Id_Agenda_Old As Integer

    Private Sub Distribuzione_Insetti_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Master_Operazione = CType(Page.Master, Operazione)

        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Salva"), ImageButton).Click, AddressOf Me.SalvaTutto
        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Carico"), ImageButton).Click, AddressOf Me.BTN_CaricoMagazzino

        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "BTN_ComboCentroAziendale"), Button).Click, AddressOf Me.CambioSpecie
        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "BTN_ComboSpecie"), Button).Click, AddressOf Me.CambioSpecie
        AddHandler CType(Ricerca.FindControlIterative(Page.Master, "BTN_Magazzini"), Button).Click, AddressOf Me.CambioMagazzino

        CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Carico"), ImageButton).Visible = True
        'CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_DoseConsigliata"), ImageButton).Visible = False
        'CType(Ricerca.FindControlIterative(Page.Master, "ImgBtn_Disciplinare"), ImageButton).Visible = False

        'AddHandler Master_Operazione.Property_Btn_Conferma_Ricetta.Click, AddressOf Me.Btn_Conferma_Ricetta
        'AddHandler Master_Operazione.Property_BTN_ComboSpecie.Click, AddressOf Me.CambioSpecie
        'AddHandler CType(Page.Master, MasterPage).PreRender, AddressOf Me.MasterUnload
        ''AddHandler Master_Operazione.Property_AggiornaGrigliaImpianti.Click, AddressOf Me.ImageButton_Sblocca_Click

    End Sub

    Private Sub InizializzaScriptClient()

        'script FILTRI AGGIUNTIVI
        ScriptManager.RegisterStartupScript(UpdatePanelFiltriAggiuntivi, UpdatePanelFiltriAggiuntivi.GetType(),
                                  String.Format("jQuery_{0}", ComboFiltriAggiuntiviAgenda1.ClientID), ComboFiltriAggiuntiviAgenda1.GetJS(), True)


        Dim STR_UpdatePanelDose As New StringBuilder

        STR_UpdatePanelDose.AppendLine("$(document).ready(function () { ")

        'STR_UpdatePanelDose.AppendLine(" $('#" & ChkImpollinatore.ClientID & "').click(); ")

        STR_UpdatePanelDose.AppendLine("    $('#" & Txt_Dose_HA.ClientID & "').keyup( function () {")
        STR_UpdatePanelDose.AppendLine("        AggiornaDOSI();")
        STR_UpdatePanelDose.AppendLine("    });")


        STR_UpdatePanelDose.AppendLine("    $('#" & Txt_DoseTot_HA.ClientID & "').keyup(function () {")
        STR_UpdatePanelDose.AppendLine("        AggiornaDOSI();")
        STR_UpdatePanelDose.AppendLine("    });")

        STR_UpdatePanelDose.AppendLine("    $('#" & rbl_QtaDose.ClientID & "').click(function () {")
        STR_UpdatePanelDose.AppendLine("        Abilita_Disabilita_DOSI();")
        STR_UpdatePanelDose.AppendLine("    });")

        STR_UpdatePanelDose.AppendLine("    $('#" & rbl_QtaTot.ClientID & "').click( function () {")
        STR_UpdatePanelDose.AppendLine("        Abilita_Disabilita_DOSI();")
        STR_UpdatePanelDose.AppendLine("    }); ")

        STR_UpdatePanelDose.AppendLine("        Abilita_Disabilita_DOSI();")

        STR_UpdatePanelDose.AppendLine("        $('#" & Txt_Insetti.ClientID & "').keydown( function () {")
        STR_UpdatePanelDose.AppendLine("        Verifica_Tasto_Premuto();")
        STR_UpdatePanelDose.AppendLine("    });")

        STR_UpdatePanelDose.AppendLine(" });")


        ScriptManager.RegisterStartupScript(UpdatePanelFormulati, UpdatePanelFormulati.GetType(),
                                      String.Format("jQuery_{0}", UpdatePanelFormulati.ClientID), STR_UpdatePanelDose.ToString, True)


    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        verificoCredenzialiDiAccesso()
        inizializzoObjParametri()
        inizializzoParametriPagina()

        Id_Agenda_Old = objParametriAgenda.Id_Agenda

        InizializzaScriptClient()

        '##############################################################
        '#####  Recupero la chiave che identifica l'oggetto  ##########
        '##############################################################
        If Not IsPostBack Then

            AggiornaFiltroRicerca()


            '==================================
            '======= VERIFICA PERMESSI ========
            '==================================
            VerificaPermessi()

            Txt_DoseTot_HA.Text = "0"
            Txt_Dose_HA.Text = "0"

            If IsNumeric(objParametriAgenda.Veg_Cod.Split("/")(0)) AndAlso CInt(objParametriAgenda.Veg_Cod.Split("/")(0)) > 0 Then
                CaricaGriglia_Avversita()
            End If

            'Cella_Avversita.Visible = True

            CaricaGriglia_Dosi()

            '--------------------------------------
            'leggo le eventuali IMPOSTAZIONI UTENTE

            Select Case objParametriAgenda.Tipo_Operazione

                Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura

                    SettaImpostazioniUtente()
                    '--------------------------------

                Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura, TipiEnumerativi.enum_TipoOperazioneDB.Modifica

                    Ripristina_Dati_nei_Controlli()

                    'controlo il permesso sulla specie, se non ce l'ho metto operazione in lettura
                    If objParametriAgenda.Tipo_Operazione = CStr(TipiEnumerativi.enum_TipoOperazioneDB.Modifica) Then
                        Try
                            Dim objSpecVeg As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                            Dim Dt As DataTable = objSpecVeg.SpecieVegetali_GestioneFiltroUtente_Leggi(objParametriAgenda.Veg_Cod.Split("/")(0), _
                                                                             0, _
                                                                             "", _
                                                                             "", _
                                                                             "", _
                                                                             "", _
                                                                             objParametri_Utenti)
                            If Dt.Rows.Count = 0 Then
                                objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
                            End If
                        Catch ex As Exception
                            objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Lettura
                        End Try
                    End If

                    If objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Lettura Then

                        CType(Ricerca.FindControlIterative(Page.Master, "Box_Salva"), Panel).Visible = False

                        Riga_Formulati.Visible = False

                    End If

                    If objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Modifica Then

                        CType(Ricerca.FindControlIterative(Page.Master, "RBL_Salva"), RadioButtonList).Visible = False

                        CType(Page.Master, Operazione).CaricaCostiAccessori()

                        SettaImpostazioniUtente()

                    End If

            End Select



        Else
            Dim app As Integer = CType(Ricerca.FindControlIterative(Page.Master, "ComboSpecie"), AgronicaControlli_2010.ComboSpecie).Valore_Combo.Split("/")(0)
            If objParametriAgenda.Veg_Cod.Split("/")(0) <> app Then
                objParametriAgenda.Veg_Cod.Split("/")(0) = app
            End If

            Exit Sub
        End If


    End Sub

    Private Sub verificoCredenzialiDiAccesso()
        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If
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

        If (objParametriAgenda.Tipo_Operazione <> enum_TipoOperazioneDB.Lettura) AndAlso Not UtenteAbilitato_Modifica Then
            Response.Redirect("~/classi/Agro_Pages/AccessoNonConsentito.aspx")
            Exit Sub
        End If

    End Sub


    Private Sub SettaImpostazioniUtente()

        If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then

            'If Cella_Avversita.Visible = True Then
            If Not ChkImpollinatore.Checked Then

                Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim Dt_Impostazioni As DataTable

                Dt_Impostazioni = ObjUtenti.Leggi(0, _
                                                  1, _
                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                  "", _
                                                  "", _
                                                  objParametri_Utenti)

                If Dt_Impostazioni IsNot Nothing AndAlso Dt_Impostazioni.Rows.Count > 0 Then

                    For i = 0 To Dt_Impostazioni.Rows.Count - 1

                        Select Case CInt(Dt_Impostazioni.Rows(i).Item("Impostazione_Cod"))

                            ''AVVERSITA  --> valore: 88=gruppi 77=singole
                            'Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_SINGOLE_GRUPPI_AVVERSITA
                            '    Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                            '        Case "77"
                            '            Me.RBL_Avversita.SelectedValue = "0"
                            '        Case Else
                            '            Me.RBL_Avversita.SelectedValue = "1"
                            '    End Select
                            '    RBL_Avversita_SelectedIndexChanged(Me, Nothing)

                            'PRODOTTO  --> valore: 88=totale 77=dose/ha
                            Case enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_QTA_PRODOTTO
                                Select Case Dt_Impostazioni.Rows(i).Item("Impostazione_Valore_1")
                                    Case "77"
                                        rbl_QtaDose.Checked = True
                                        rbl_QtaTot.Checked = False
                                    Case Else
                                        rbl_QtaDose.Checked = False
                                        rbl_QtaTot.Checked = True
                                End Select

                        End Select

                    Next

                End If

            End If

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
        objParametriAgenda.OperazioneMulticentro = True
        Id_Agenda_Old = objParametriAgenda.Id_Agenda
        objParametriAgenda.Elem_Cod = INSETTI


    End Sub

    Private Sub inizializzoParametriPagina()

        inizializzoParametriAgenda()

        Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim Lav_Des As String = objOperazioniLeggi.LavorazioneDes_from_LavorazioneCod(objParametriAgenda.Lav_Cod, objParametri_Server)
        objParametriAgenda.Lav_Des = lav_des
        Master_Operazione.Property_Lbl_Titolo.Text = Lav_Des

        'valori copiati da agenda vecchia
        'Movimento_Dettaglio_Contabilizzato_Installazione = 1
        'Movimento_Dettaglio_Pendente_Installazione = 3
        'Movimento_Dettaglio_Contabilizzato_Magazzino = 1
        'Movimento_Dettaglio_Pendente_Magazzino = 4
        'Movimento_Dettaglio_Extra_Date = AGRODATAINIZIO
        'Movimento_Dettaglio_Anno = 1900

        objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.TRATTAMENTO)

       
    End Sub

    Private Sub Insetti_Disposed()
        'Master
        CType(Page.Master, Operazione).Operazioni_Dispose()

        'Page
        Session.Remove("UtenteAbilitato_Lettura")
        Session.Remove("UtenteAbilitato_Modifica")
        Session.Remove("DtAvv")
        Session.Remove("DtAvvGru")
        Session.Remove("vs_dtDosi")

    End Sub

    Private Sub CaricaGriglia_Avversita(Optional ByVal Ins_Cod As String = "")

        '----- Definizione delle variabili

        Dim DtAvv As New DataTable
        Dim Dr As DataRow
        Dim i As Integer

        '----- Definisco la struttura dei DataTable

        DtAvv.Columns.Add(New DataColumn("Av_Des", GetType(String)))
        DtAvv.Columns.Add(New DataColumn("Av_Cod", GetType(Integer)))
        DtAvv.Columns.Add(New DataColumn("Av_Gru", GetType(Integer)))

        Dim DtKeys(1) As DataColumn

        DtKeys(0) = DtAvv.Columns("Av_Cod")
        DtKeys(1) = DtAvv.Columns("Av_Gru")

        DtAvv.PrimaryKey = DtKeys

        If objParametriAgenda.Veg_Cod.Split("/")(0) = "" Then
            objParametriAgenda.Veg_Cod = 0
        End If
        '---------------------------------------------------------------
        If objParametriAgenda.Veg_Cod.Split("/")(0) = "" OrElse objParametriAgenda.Veg_Cod.Split("/")(0) = "-1" OrElse objParametriAgenda.Veg_Cod.Split("/")(0) = "0" Then
            Exit Sub
        End If

        '-----------------------
        'SINGOLE

        Dim filtroAvv As String = ""
        Dim DtAvvTmp As DataTable
        Dim DtAvvGruTmp As DataTable

        'filtro infestanti/gruppi infestanti
        filtroAvv &= "  NOT EXISTS (SELECT * FROM InfestantiAttive " &
                       " WHERE InfestantiAttive.Av_Cod = Avversita.Av_Cod ) "

        'elimino avversita/gruppi con (#) e non usare IN SCRITTURA
        filtroAvv &= " AND Avversita.Av_Des_Vol NOT LIKE '%non usare%' " &
                     " AND Avversita.Av_Des_Vol NOT LIKE '%(#)%' "

        If Ins_Cod <> "" Then

            Dim objInsetti As New AgronicaCoreMetaSchemaDAL.InsettiUtili_R
            DtAvvTmp = objInsetti.LeggiXAvversitaXSpecie(Ins_Cod, _
                                        0, _
                                        objParametriAgenda.Veg_Cod.Split("/")(0), _
                                        0, _
                        AGRODATAINIZIO, AGRODATAFINE, _
                        filtroAvv, _
                        "", _
                        objParametri_Server)

        Else

            Dim objAvv As New AgronicaCoreMetaSchemaDAL.SpecieVegetalixAvversita_R
            DtAvvTmp = objAvv.Leggi(0, objParametriAgenda.Veg_Cod.Split("/")(0), 0, _
                                    AGRODATAINIZIO, AGRODATAFINE, _
                                    AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                    filtroAvv, _
                                    "", _
                                    objParametri_Server)

        End If



        If Not IsNothing(DtAvvTmp) Then
            For i = 0 To DtAvvTmp.Rows.Count - 1
                Dr = DtAvv.NewRow
                Dr.Item("Av_Des") = DtAvvTmp.Rows(i).Item("Av_Des_Vol") & " (<i>" & DtAvvTmp.Rows(i).Item("Av_Des_Lat") & "</i>)"
                Dr.Item("Av_Cod") = DtAvvTmp.Rows(i).Item("Av_Cod")
                Dr.Item("Av_Gru") = 0
                Try
                    DtAvv.Rows.Add(Dr)
                Catch ex As Exception
                End Try
            Next
        End If

        '-----------------------
        'GRUPPI
        Dim filtroAvvGru As String
        'filtro infestanti/gruppi infestanti
        filtroAvvGru = "  NOT EXISTS (SELECT *	FROM GruppoAvversitaAttive " & _
                     " WHERE GruppoAvversitaAttive.Av_Gru = GruppoAvversita.Av_Gru ) "

        'elimino avversita/gruppi con (#) e non usare IN SCRITTURA
        filtroAvvGru &= " AND GruppoAvversita.Av_Gru_Des NOT LIKE '%non usare%' " & _
                        " AND GruppoAvversita.Av_Gru_Des NOT LIKE '%(#)%' "


        If Ins_Cod <> "" Then

            Dim objInsetti As New AgronicaCoreMetaSchemaDAL.InsettiUtili_R
            DtAvvGruTmp = objInsetti.LeggiXGruAvversitaXSpecie(Ins_Cod, _
                                        0, _
                                        objParametriAgenda.Veg_Cod.Split("/")(0), _
                                        0, _
                        AGRODATAINIZIO, AGRODATAFINE, _
                        filtroAvv, _
                        "", _
                        objParametri_Server)

        Else

            Dim objAvvGru As New AgronicaCoreMetaSchemaDAL.GruppoAvversita_R
            DtAvvGruTmp = objAvvGru.Leggi(0, 1, _
                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                filtroAvvGru, _
                                "", _
                                objParametri_Server)

        End If

        If Not IsNothing(DtAvvGruTmp) Then
            For i = 0 To DtAvvGruTmp.Rows.Count - 1
                Dr = DtAvv.NewRow
                Dr.Item("Av_Des") = DtAvvGruTmp.Rows(i).Item("Av_Gru_Des") & " (<i>" & DtAvvGruTmp.Rows(i).Item("Av_Gru_Des_Lat") & "</i>)"
                Dr.Item("Av_Gru") = DtAvvGruTmp.Rows(i).Item("Av_Gru")
                Dr.Item("Av_Cod") = 0
                Try
                    DtAvv.Rows.Add(Dr)
                Catch ex As Exception
                End Try

            Next
        End If

        '----------------------------------------------------------------------
        '----- Associo il DataTable con la DataGrid

        'uso il dataview per ordinare
        Dim DvAvv As New DataView
      
        DtAvv.TableName = "Av_Des"
        DvAvv.Table = DtAvv
        DvAvv.Sort = "Av_Des ASC"

        GridViewAvversita.DataSource = DvAvv.ToTable
        GridViewAvversita.DataBind()

        Session("DtAvv") = DvAvv.ToTable


    End Sub




#Region "Eventi Gestiti Dalla Master"

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)
        'sblocca()
        'RipristinaSessione()
        Insetti_Disposed()

        Dim link As String = ""
        Try
            Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda.SitoOrigine
            Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

            If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                       enum_PagineGiasOnline_2010.RegistazioneSmart,
                                       enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

            ElseIf sitoorigine = Enum_SiteRedirector.GiasNG Then
                AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
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

    Private Sub BTN_CaricoMagazzino(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim PaginaLink As String
        'Case LAVCOD_SCARICO, LAVCOD_CARICO, LAVCOD_VENDITA, LAVCOD_ACQUISTO, LAVCOD_TRASFERIMENTO
        PaginaLink = "../GestioneMagazzini/FormProdotto.aspx"

        Dim xChiave As String = ""
        Dim mode As String = ""
        Dim Carico_Scarico As String = ""
        Call Albero.ChiaveAlbero_Codifica(xChiave, _
                                               enum_TipoNodo.p_PortafoglioProdotti, _
                                               objParametriAgenda.Fabbricato.Split("|")(2), _
                                               objParametriAgenda.Fabbricato.Split("|")(1), , , , , , , , , , , , _
                                               objParametriAgenda.Fabbricato.Split("|")(0))

        Dim Lav_Cod As Integer = LAVCOD_CARICO
        Carico_Scarico = "C"
        mode = "magazzino"


        PaginaLink = PaginaLink & _
                        "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder) & _
                        "&c=" & Stringa_Codifica(Carico_Scarico, AgroKey_EncoderDecoder) & _
                        "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder) & _
                        "&orig=" & Stringa_Codifica(enum_PagineAgenda_2010.Menu, AgroKey_EncoderDecoder) & _
                        "&mode=" & Stringa_Codifica(mode, AgroKey_EncoderDecoder) & _
                         "&l=" & Stringa_Codifica(objParametriAgenda.Lav_Cod, AgroKey_EncoderDecoder) & _
                        "&d=" & Stringa_Codifica(CStr(objParametriAgenda.Data), AgroKey_EncoderDecoder) & _
                        "&s=" & Stringa_Codifica(objParametriAgenda.Sa_Cod, AgroKey_EncoderDecoder) & _
                        "&a=" & Stringa_Codifica(objParametriAgenda.Id_Agenda, AgroKey_EncoderDecoder) & _
                        "&exit=true"

        Dim strJS As String
        strJS = "<script language='javascript'>" & _
            "           window.open('" & PaginaLink & "' ," & _
            "           'stampe'," & _
            "           'height=700," & _
            "           width=1000," & _
            "           menubar=yes," & _
            "           resizable=yes," & _
            "           scrollbars=yes," & _
            "           top=0,left=0');" & _
            " </script> "

        ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel), _
                                            CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel).GetType(), _
                                            "jQuery_{0}", strJS, False)


    End Sub

    Private Sub CambioMagazzino()
        Cmb_Insetti.Items.Clear()
        'Cmb_Insetti_Carica()
    End Sub


    Private Sub CambioSpecie()
        Cmb_Insetti.Items.Clear()
        CaricaGriglia_Avversita()
    End Sub


    Private Sub MasterUnload()
        'con questo mi ricarice tutte le combo e toglie selezionato in modifica

        'questa operazione è fatta dall'evento master unload
        'ma il load della pagina master potrebbe avere cambiato dei valori in objparametri (come nel caso di sa_cod).
        'se alla fine modifico nella pagina fioglia un valore viene in automatico riscritta in sessione
        'la objparametri della pagina figlia, sovrascrivendo la master e perdendo quindi le modifiche fatte
        'objParametriAgenda.Leggi()

        If Not IsPostBack Then
            'Cmb_Trappola_Carica()
        End If
    End Sub

#End Region

    Protected Sub ImgBtn_Cerca_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Cerca.Click

        Cmb_Insetti_Carica()

    End Sub

    Private Sub Cmb_Insetti_Carica()


        Cmb_Insetti.Items.Clear()

        Dim FiltroRicerca As String = ""

        '-----------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------
        Dim cercaInsettiSenzaAvversita As Boolean = False
        Dim strGruAvversita As String = ""
        Dim strAvversita As String = ""
        Dim Valore_Ricerca As String
        Valore_Ricerca = ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue

        ' If Cella_Avversita.Visible = True Then

        If Not ChkImpollinatore.Checked Then
            Select Case Valore_Ricerca
                Case "0" 'nessuno
                    cercaInsettiSenzaAvversita = True
                    'Cmb_Insetti.FiltroRicerca = 0
                Case "1" 'Prodotti --> Avversità
                    'Cmb_Insetti.Opt_Singola_Gruppo = -1
                    'Cmb_Insetti.FiltroRicerca = 1
                Case "2" 'Avversità --> Prodotti
                    'Cmb_Insetti.FiltroRicerca = 2
                    'Cmb_Insetti.Opt_Singola_Gruppo = CInt(RBL_Avversita.SelectedValue)

                    Dim Array_AvCod(0) As Integer
                    Dim Array_AvGru(0) As Integer

                    Dim NumAvv As Integer = 0

                    For i = 0 To GridViewAvversita.Rows.Count - 1
                        If CType(GridViewAvversita.Rows(i).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True Then
                            ReDim Preserve Array_AvCod(NumAvv)
                            Array_AvCod(NumAvv) = GridViewAvversita.Rows(i).Cells(1).Text
                            ReDim Preserve Array_AvGru(NumAvv)
                            Array_AvGru(NumAvv) = GridViewAvversita.Rows(i).Cells(2).Text
                            If Array_AvCod(NumAvv) <> "0" Then
                                If NumAvv = 0 Then
                                    strAvversita = Array_AvCod(NumAvv)
                                Else
                                    strAvversita &= "," & Array_AvCod(NumAvv)
                                End If
                            End If
                            If Array_AvGru(NumAvv) <> "0" Then
                                If NumAvv = 0 Then
                                    strGruAvversita = Array_AvGru(NumAvv)
                                Else
                                    strGruAvversita &= "," & Array_AvGru(NumAvv)
                                End If
                            End If
                            NumAvv += 1
                        End If
                    Next

                    If NumAvv = 0 Then
                        cercaInsettiSenzaAvversita = True
                    End If

            End Select

        Else
            cercaInsettiSenzaAvversita = True
        End If

    

        If strAvversita <> "" Then
            FiltroRicerca = " InsettiUtilixAvversita.AV_COD in ( " & strAvversita & " ) "
        End If

        If strGruAvversita <> "" Then
            FiltroRicerca = " AvversitaxGruppoAvversita.AV_GRU in ( " & strGruAvversita & " ) "
        End If

        '-----------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------

        If Txt_Insetti.Text <> "" Then
            If FiltroRicerca <> "" Then
                FiltroRicerca &= " AND "
            End If
            FiltroRicerca = " InsettiUtili.INS_DES LIKE '%" & Txt_Insetti.Text & "%'"
        End If


        Dim Fabbricato_Val As String
        Fabbricato_Val = Master_Operazione.Property_ComboMagazzini.ddl_Magazzini.SelectedValue

        If Fabbricato_Val = "0" OrElse Fabbricato_Val = "" Then

            If cercaInsettiSenzaAvversita Then

                AgronicaCoreUtility.CaricaListControl.InsettiUtiliSenzaAvversitaEGruppi(CType(Me.Cmb_Insetti, ListControl), _
                                                           True, "", "0", _
                                                           "", _
                                                     FiltroRicerca, _
                                                     "", objParametri_Server)

            Else

                AgronicaCoreUtility.CaricaListControl.InsettiUtiliXAvversitaEGruppi(CType(Me.Cmb_Insetti, ListControl), _
                                                           True, "", "0", _
                                                           "", _
                                                     FiltroRicerca, _
                                                     "", objParametri_Server)

            End If

        Else


            Dim Fabbricato_Cod As String = Split(Me.objParametriAgenda.Fabbricato, "|")(0)
            Dim Sa_Cod As Integer = Split(Me.objParametriAgenda.Fabbricato, "|")(1)
            Dim piva As String = Split(Me.objParametriAgenda.Fabbricato, "|")(2)

            AgronicaCoreUtility.CaricaListControl.InsettiUtiliXAvversitaEGruppi_Magazzino(CType(Me.Cmb_Insetti, ListControl), _
                                                                    True, "", "0", _
                                                                    enum_Agenda_Causali.SCARICO, _
                                                                    piva, _
                                                                    Sa_Cod, _
                                                                    Fabbricato_Cod, _
                                                                     cercaInsettiSenzaAvversita, FiltroRicerca, "", objParametri_Server, objParametri_Utenti)
        End If



        Cmb_Insetti.SelectedIndex = 0
        Lbl_Giacenza.Text = ""
        Lbl_Num_Insetti.InnerText = "Trovati " & Cmb_Insetti.Items.Count - 1 & " Insetti/Acari"


    End Sub

    Protected Sub Cmb_Insetti_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Cmb_Insetti.SelectedIndexChanged

        Lbl_Giacenza.Text = ""

        If Cmb_Insetti.SelectedItem.Text <> "" Then
            Select Case objParametriAgenda.Fabbricato
                Case Is <> "0"
                    Lbl_Giacenza.Text = Math.Round(Giacenza_Insetti(CInt(Cmb_Insetti.SelectedValue)), 4).ToString
            End Select
        End If

        Dim Valore_Ricerca As String
        If CStr(Cmb_Insetti.SelectedItem.Value) <> "0" Then
            Valore_Ricerca = ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue
            If Not ChkImpollinatore.Checked AndAlso Valore_Ricerca = "1" Then

                'cerco le avversita
                CaricaGriglia_Avversita(CStr(Cmb_Insetti.SelectedItem.Value))

            End If
        End If


    End Sub

    Private Function Giacenza_Insetti(ByVal Ins_Cod As Integer) As Decimal

        Dim Giacenza As Decimal = 0
        Dim objGia As New AgronicaCoreContabDAL.Giacenze_R
        Dim Dt_Giacenze As DataTable

        Dt_Giacenze = objGia.SchedaGiacenzeMagazzino(objParametriAgenda.Data, _
                                                     Split(objParametriAgenda.Fabbricato, "|")(2), _
                                                     Split(objParametriAgenda.Fabbricato, "|")(1), _
                                                     Split(objParametriAgenda.Fabbricato, "|")(0), _
                                                     INSETTI, _
                                                     CInt(Ins_Cod), _
                                                     0, _
                                                     0, 0, 0, 0, LOTTO_NONDEFINITO, _
                                                     False, _
                                                     "", "", "", "", "", "", "", "", "", "", "", _
                                                     "", _
                                                     objParametri_Server, objParametri_Utenti)

        If Not IsNothing(Dt_Giacenze) AndAlso Dt_Giacenze.Rows.Count > 0 Then
            For i = 0 To Dt_Giacenze.Rows.Count - 1
                'ignoro le giacenze infinitesime
                If Dt_Giacenze.Rows(i).Item("Giacenza") <> 0 And Not (Dt_Giacenze.Rows(i).Item("Giacenza") < QTA_GiancenzeVisualizzate And Dt_Giacenze.Rows(i).Item("Giacenza") > -QTA_GiancenzeVisualizzate) Then
                    Giacenza = Dt_Giacenze.Rows(i).Item("Giacenza")
                End If
            Next
        End If

        Return Giacenza

    End Function


    Protected Sub ImgBtn_DoseInserisci_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_DoseInserisci.Click

        Dim InsCod As String = ""
        Dim InsDes As String = ""

        Dim Dose As Decimal
        Dim Dose_Tot As Decimal
        Dim UdmCod As Integer = 0
        Dim UdmDes As String = ""

        'controllo sulle giacenze di magazzino
        If ControllaGiacenzaMagazzino() = False Then
            Exit Sub
        End If

        'controllo selezione insetto
        If Cmb_Insetti.SelectedItem.Text = "" Then
            Messaggi.AgroMsgBox("Selezionare gli insetti/acari.", Page, , updateDoseInserisci)
            Exit Sub
        Else
            InsCod = Cmb_Insetti.SelectedValue
            InsDes = Cmb_Insetti.SelectedItem.Text
        End If


        'controllo selezione qta
        If Not IsNumeric(Txt_Dose_HA.Text) Then
            'If Not IsNumeric(Txt_Dose_HA.Text) Or CInt((Txt_Dose_HA.Text)) <= 0 Then
            Messaggi.AgroMsgBox("E' necessario inserire il numero di insetti!", Page, , updateDoseInserisci)
            Exit Sub
        Else
            Dose = Txt_Dose_HA.Text
            Dose_Tot = Txt_DoseTot_HA.Text
        End If

        '-----
        ScriptManager.RegisterStartupScript(UpdateProgress2, UpdateProgress2.GetType, "azzera",
                                                "$(document).ready(function () {$('#" & HiddenVarie.ClientID & "').val(''); });", True)


        'Inserisco il formulato nella griglia delle dosi
        Call Dosi_Inserisci(InsCod, InsDes, Dose,
                            Dose_Tot, UdmCod, UdmDes)

        'dopo che ho inserito il primo formulato non posso più modificare il magazzino
        'CType(Page.Master.FindControl("ComboMagazzini"), AgronicaControlli_2010.ComboMagazzini).Enabled = True
        Griglia_Dosi_Data_Bind()

        Dim script As New StringBuilder

        script.AppendLine("$(document).ready(function () { ")
        script.AppendLine("     BloccaSbloccaTotale();")
        script.AppendLine("     BloccaSbloccaChkImpollinatore();")
        script.AppendLine("}); ")

        ScriptManager.RegisterStartupScript(UpdatePanelMiscela, UpdatePanelMiscela.GetType(),
                                         String.Format("jQuery_{0}", UpdatePanelMiscela.ClientID), script.ToString, True)

        'pulisco la selezione
        Cmb_Insetti.Items.Clear()
        Lbl_Giacenza.Text = ""

    End Sub

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


        Dim AlmenoUno As Boolean = False
        'controllo selezione avversita
        If Not ChkImpollinatore.Checked Then

            'If Cella_Avversita.Visible = False Then
            '    Dim msg As String = "La cella avversita dovrebbe essere visibile"
            '    'Throw New Exception(msg)
            '    Messaggi.AgroMsgBox(msg, Page, , _
            '           CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            '    Exit Sub
            'End If
            For i = 0 To GridViewAvversita.Rows.Count - 1
                If CType(GridViewAvversita.Rows(i).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True Then
                    AlmenoUno = True
                    Exit For
                End If
            Next
            If Not AlmenoUno Then
                Messaggi.AgroMsgBox("Selezionare l'avversità/gruppo! ", Page, ,
                   CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                Exit Sub
            End If
            'Else

            'If Cella_Avversita.Visible = True Then
            '    Dim msg As String = "La cella avversita non dovrebbe essere visibile"
            '    'Throw New Exception(msg)
            '    Messaggi.AgroMsgBox(msg, Page, , _
            '           CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            '    Exit Sub
            'End If
        End If


        '-----------------------------------------------------------------------------------------------------
        '-----------------------TEMPORANERO PER EVITARE SALVATAGGIO MAGAZZINI ESTERNI------------------------
        If objParametriAgenda.Fabbricato <> "0" AndAlso Split(objParametriAgenda.Fabbricato, "|").Count = 3 AndAlso Split(objParametriAgenda.Fabbricato, "|")(2) <> objParametriAgenda.Piva Then
            Dim MErrore As String
            MErrore = Resources.AgronicaAgenda_2010.ATTENZIONEBrAlMomentoNonÈPermessoIlSalvata
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & MErrore, Page, ,
                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Exit Sub
        End If
        '-----------------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------------

        Dim messaggio_errore As String = ""
        Dim TipoSalvataggio As Integer = CType(Ricerca.FindControlIterative(Page.Master, "RBL_Salva"), RadioButtonList).SelectedValue + 1
        If Salva(TipoSalvataggio, messaggio_errore) Then
            fine_salvataggio(TipoSalvataggio)
        Else
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & messaggio_errore, Page, ,
                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
        End If

    End Sub

    Private Function Salva(ByVal TipoSalvataggio As Integer, ByRef messaggio_errore As String) As Boolean

        Dim res As Boolean = False

        '---------------------------------------
        ' recupero il CENTRO
        Dim Sa_Cod As Integer = 0
        If objParametriAgenda.Sa_Cod = "" Then
            messaggio_errore = Resources.AgronicaAgenda_2010.SelezionareUnCentroAziendale
            Return False
        Else
            Sa_Cod = CInt(objParametriAgenda.Sa_Cod)
        End If


        '---------------------------------------
        ' recupero il MAGAZZINO
        Dim Fabbricato_Cod As String = ""
        Fabbricato_Cod = objParametriAgenda.Fabbricato


        '////////////////////////////////////////////////////////////
        '//////////////////DATI PER L OPERAZIONE///////////////////////
        '////////////////////////////////////////////////////////////

        '---------------------------------------
        ' recupero gli IMPIANTI
        Dim ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
        ListaImpianti = CType(Master, Operazione).GetImpianti()
        If ListaImpianti.Count = 0 Then
            messaggio_errore = Resources.AgronicaAgenda_2010.SelezionareAlmenoUnImpiantoColturale
            Return False
        End If

        'se ho l'agenda metto a totale
        If objParametriAgenda.Fabbricato.Length > 1 Then
            rbl_QtaDose.Checked = False
            rbl_QtaTot.Checked = True
        End If

        'modifico le dosi a ettaro in modo da avare una precisione maggiore
        Dim dt_dosi As DataTable = Session("vs_dtDosi")
        'Dim sup_trattata As Decimal = Txt_SupSelezionata.Value
        'For i = 0 To dt_dosi.Rows.Count - 1
        '    dt_dosi.Rows(i).Item("Dose_Ha") = CDbl(CDbl(dt_dosi.Rows(i).Item("Qta_tot")) / sup_trattata)
        'Next

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
                    Dim listalista As New List(Of List(Of Decimal))

                    If Session("vs_dtDosi") IsNot Nothing Then

                        Dim tot As Decimal = Txt_SupTrattata.Value
                        For j = 0 To ListaSacod.Count - 1
                            Dim l As New List(Of Decimal)
                            listalista.Add(l)
                        Next

                        For j = 0 To ListaSacod.Count - 1

                            Dim sommatoriaQtaquestoSa_Cod As Decimal = 0

                            For i = 0 To ListaImpianti.Count - 1
                                If ListaImpianti(i).Sa_Cod = ListaSacod(j) Then
                                    sommatoriaQtaquestoSa_Cod += ListaImpianti(i).Qta2
                                End If
                            Next

                            Dim rapporto As Decimal = sommatoriaQtaquestoSa_Cod / tot

                            'controllo se sono all'ultimo giro o no
                            If j = ListaSacod.Count - 1 Then

                                For i = 0 To dt_dosi.Rows.Count - 1
                                    Dim qta As Decimal = dt_dosi.Rows(i).Item("Qta_tot")
                                    Dim jj As Integer = 0
                                    For jj = 0 To listalista.Count - 2
                                        qta = qta - listalista(jj)(i)
                                    Next
                                    listalista(j).Add(qta)
                                Next

                            Else
                                For i = 0 To dt_dosi.Rows.Count - 1
                                    Dim qta As Decimal
                                    qta = dt_dosi.Rows(i).Item("Qta_tot") * rapporto
                                    listalista(j).Add(qta)
                                Next
                            End If
                        Next
                    End If

                    'effettuo verifica arrotondamento
                    For i = 0 To dt_dosi.Rows.Count - 1
                        'per ogni prodotto mi calcolo la somma totale
                        Dim sum_qta As Decimal = 0
                        For j = 0 To listalista.Count - 1
                            sum_qta = sum_qta + listalista(j)(i)
                        Next
                        If sum_qta <> dt_dosi.Rows(i).Item("Qta_tot") Then
                            listalista(listalista.Count - 1)(i) = listalista(listalista.Count - 1)(i) + (dt_dosi.Rows(i).Item("Qta_tot") - sum_qta)
                        End If
                    Next



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

                        Dim Agenda As Operazione_Agenda
                        Agenda = CreaOggettoAgenda(ListaImpiantixQuestoSaCod, ListaSacod(i), listalista(i))

                        If IsNothing(Agenda) Then
                            Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                        End If

                        Dim objAgendaScrivi As New Agenda_Operazione_Helper
                        Dim Id_Agenda As Integer = 0

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

                        'se sono in scrittura devo agganciare la ricetta se presente
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
                    Next

                    '--------------------------------------------------
                    '--------------------------------------------------
                    '--------- OPERAZIONE SINGOLO CENTRO    -----------
                    '--------------------------------------------------
                    '--------------------------------------------------
                Case Else



                    '--------------------------------------------------
                    '--------------------------------------------------
                    '--------- CREAZIONE OGGETTO DA SALVARE   ---------
                    '--------------------------------------------------
                    '--------------------------------------------------
                    Dim Qta_da_scaricare As New List(Of Decimal)
                    'la qta da scaricare è esattamente quella indicata nella qta_totale
                    For i = 0 To dt_dosi.Rows.Count - 1
                        Qta_da_scaricare.Add(dt_dosi.Rows(i).Item("Qta_Tot"))
                    Next

                    Dim Agenda As Operazione_Agenda
                    Agenda = CreaOggettoAgenda(ListaImpianti, objParametriAgenda.Sa_Cod, Qta_da_scaricare)

                    If IsNothing(Agenda) Then
                        Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                    End If

                    Dim objAgendaScrivi As New Agenda_Operazione_Helper
                    Dim Id_Agenda As Integer = 0

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
                    If CType(Ricerca.FindControlIterative(Page.Master, "RBL_Salva"), RadioButtonList).SelectedValue = 0 Then
                        objParametriAgenda.Id_Agenda = Id_Agenda
                    End If

            End Select

            res = True

            'commit transazione
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

        Catch ex As Exception

            'commit transazione rollback
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            messaggio_errore = Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & ex.Message
            res = False

        Finally

            'chiudi connessione
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try


        Return res


    End Function

    Private Function CreaOggettoAgenda(ByVal ListaImpianti As List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto),
                                               ByVal Sa_Cod As Integer,
                                               ByVal Qta_da_scaricare As List(Of Decimal)) As Operazione_Agenda

        '---------------------------------------
        ' recupero i Consigli
        Dim ListaConsigli As List(Of Nota)
        ListaConsigli = CType(Master, Operazione).GetConsigli()


        '---------------------------------------
        ' recupero le NOTE
        Dim strNota As String = CType(Master, Operazione).GetNota()


        '---------------------------------------
        ' recupero la DATA
        Dim Data As Date
        If objParametriAgenda.Data = AGRODATAINIZIO Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IndicareUnaData, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        Else
            Data = objParametriAgenda.Data
        End If

        '---------------------------------------
        ' recupero la SPECIE
        Dim Veg_Cod As String = ""
        Dim Veg_Des As String = ""
        If objParametriAgenda.Veg_Cod.Split("/")(0) = "-1" Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnaSpecieVegetale, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        Else
            Veg_Cod = objParametriAgenda.Veg_Cod.Split("/")(0)
            Veg_Des = CType(Ricerca.FindControlIterative(Page.Master, "ComboSpecie"), AgronicaControlli_2010.ComboSpecie).Testo_Combo
        End If

        '---------------------------------------
        ' recupero la OPERAZIONE
        Dim Lav_Cod As String = ""
        Dim Lav_Des As String = ""
        If objParametriAgenda.Lav_Cod = "" Then
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareUnOperazione, Page, , UpdatePanelMiscela)
            Return Nothing
        Else
            Lav_Cod = objParametriAgenda.Lav_Cod
            Lav_Des = CType(Ricerca.FindControlIterative(Page.Master, "ComboOperazione"), AgronicaControlli_2010.ComboOperazioni).Testo_Combo
        End If


        '---------------------------------------
        ' recupero le DOSI
        Dim Dt_Dosi As New DataTable
        If Session("vs_dtDosi") IsNot Nothing Then
            Dt_Dosi = Session("vs_dtDosi")
            If Dt_Dosi.Rows.Count < 1 Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareAlmenoUnFormulato, Page, ,
                                    CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                Return Nothing
            End If
        Else
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SelezionareAlmenoUnFormulato, Page, ,
                                CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
            Return Nothing
        End If




        Dim BaseCode As Integer = 0
        Dim TopCode As Integer = 0

        '------------------------------------------------
        '----- Calcolo i valori di BaseCode e TopCode
        '------------------------------------------------
        UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, Session("ASG_ProgressivoGIAS"))


        '------------------------------------------------
        '----- AGENDA
        '------------------------------------------------
        Dim Agenda As Operazione_Agenda
        Dim Nota As Nota
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio_Tecnico As Movimento_Dettaglio_Tecnico
        Dim Movimento_Dettaglio As Movimento_Dettaglio
        Dim Movimento_Destinazione As Movimento_Destinazione

        Dim trovato As Boolean
        Dim ListaVarieta As New List(Of String)
        Dim StrVarieta As String = ""
        For i = 0 To ListaImpianti.Count - 1
            trovato = False
            For j = 0 To ListaVarieta.Count - 1
                If ListaVarieta(j) = ListaImpianti(i).Cul_Des Then
                    trovato = True
                    Exit For
                End If
            Next
            If Not trovato Then
                ListaVarieta.Add(ListaImpianti(i).Cul_Des)
                StrVarieta &= ", " & ListaImpianti(i).Cul_Des
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
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = Lav_Des & " (" & Veg_Des & "  [" & StrVarieta & "])"

        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode


        '------------------------------------------------
        '----- NOTE
        '------------------------------------------------

        If ListaConsigli.Count > 0 Then
            Agenda.Note = New List(Of Nota)
            For i = 0 To ListaConsigli.Count - 1
                Nota = New Nota
                Nota.Id_Agenda = objParametriAgenda.Id_Agenda
                Nota.Nota_Cod = ListaConsigli(i).Nota_Cod
                Agenda.Note.Add(Nota)
            Next
        End If


        '------------------------------------------------
        '----- MOVIMENTI
        '------------------------------------------------
        Agenda.Movimenti = New List(Of Movimento)


        'COSTI ACCESSORI
        For i = 0 To objParametriAgenda.Movimenti.Count - 1
            objParametriAgenda.Movimenti(i).Id_Agenda = objParametriAgenda.Id_Agenda
            objParametriAgenda.Movimenti(i).Sa_Cod = Agenda.Sa_Cod
            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli) Then
                Dim j As Integer = 0
                For j = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda = objParametriAgenda.Id_Agenda
                    If objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = 0 Then
                        objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = Agenda.Sa_Cod
                    End If
                Next
            End If

            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then
                Dim j As Integer = 0
                For j = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                    objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Id_Agenda = objParametriAgenda.Id_Agenda
                    If objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Sa_Cod = 0 Then
                        objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Sa_Cod = Agenda.Sa_Cod
                    End If
                Next
            End If
            objParametriAgenda.Movimenti(i).Data = Data
            Agenda.Movimenti.Add(objParametriAgenda.Movimenti(i))
        Next




        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO LAVORAZIONE
        '------------------------------------------------
        '------------------------------------------------
        Movimento = New Movimento

        Movimento.Id_Agenda = objParametriAgenda.Id_Agenda
        Movimento.Piva = Agenda.Piva
        Movimento.Sa_Cod = Agenda.Sa_Cod
        Movimento.Data = Data
        Movimento.Lav_Cod = Lav_Cod
        Movimento.Cau_Mov = CAU_TRATTAMENTO
        Movimento.Mov_Desc = strNota

        Movimento.Mezzo = 1
        If rbl_QtaDose.Checked Then
            'dose
            Movimento.Modalita = 11
        Else
            'totale
            Movimento.Modalita = 10
        End If

        'Caso Nessuno Nessuno
        Movimento.Num_Protocollo = 0
        Movimento.Disciplinare_PubblicoPrivato = 0

        Movimento.BaseCode = BaseCode
        Movimento.TopCode = TopCode

        Agenda.Movimenti.Add(Movimento)

        '------------------------------------------------
        '----- MOVIMENTO DETTAGLIO TECNICO X AVVERSITA
        '------------------------------------------------
        If Not ChkImpollinatore.Checked Then

            'guardo tutte le avversità
            Dim listAvCod As New List(Of Integer)
            Dim listAvGruCod As New List(Of Integer)
            For i = 0 To GridViewAvversita.Rows.Count - 1
                If CType(GridViewAvversita.Rows(i).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True Then
                    If IsNumeric(GridViewAvversita.Rows(i).Cells(1).Text) AndAlso CInt(GridViewAvversita.Rows(i).Cells(1).Text) <> 0 Then
                        listAvCod.Add(CInt(GridViewAvversita.Rows(i).Cells(1).Text))
                    ElseIf IsNumeric(GridViewAvversita.Rows(i).Cells(2).Text) AndAlso CInt(GridViewAvversita.Rows(i).Cells(2).Text) <> 0 Then
                        listAvGruCod.Add(CInt(GridViewAvversita.Rows(i).Cells(2).Text))
                    End If
                End If
            Next

            'avversità
            If Not IsNothing(listAvCod) AndAlso listAvCod.Count > 0 Then
                For j = 0 To listAvCod.Count - 1
                    Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico
                    Movimento_Dettaglio_Tecnico.Id_Agenda = objParametriAgenda.Id_Agenda
                    Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
                    Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
                    Movimento_Dettaglio_Tecnico.Data = Data
                    Movimento_Dettaglio_Tecnico.Av_Cod = listAvCod(j)
                    Movimento_Dettaglio_Tecnico.Av_Gru = 0
                    Movimento_Dettaglio_Tecnico.BaseCode = BaseCode
                    Movimento_Dettaglio_Tecnico.TopCode = TopCode
                    Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)
                Next
            End If

            'gruppi avversità
            If Not IsNothing(listAvGruCod) AndAlso listAvGruCod.Count > 0 Then
                For j = 0 To listAvGruCod.Count - 1
                    Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico
                    Movimento_Dettaglio_Tecnico.Id_Agenda = objParametriAgenda.Id_Agenda
                    Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
                    Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
                    Movimento_Dettaglio_Tecnico.Data = Data
                    Movimento_Dettaglio_Tecnico.Av_Cod = 0
                    Movimento_Dettaglio_Tecnico.Av_Gru = listAvGruCod(j)
                    Movimento_Dettaglio_Tecnico.BaseCode = BaseCode
                    Movimento_Dettaglio_Tecnico.TopCode = TopCode
                    Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)
                Next
            End If

        Else

            Movimento_Dettaglio_Tecnico = New Movimento_Dettaglio_Tecnico
            Movimento_Dettaglio_Tecnico.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
            Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio_Tecnico.Data = Data
            Movimento_Dettaglio_Tecnico.Av_Cod = -1
            Movimento_Dettaglio_Tecnico.Av_Gru = -1
            Movimento_Dettaglio_Tecnico.Sigla_av = "IMPOLL"
            Movimento_Dettaglio_Tecnico.ExtraStr = "IMPOLL"
            Movimento_Dettaglio_Tecnico.BaseCode = BaseCode
            Movimento_Dettaglio_Tecnico.TopCode = TopCode
            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)


        End If
        '------------------------------------------------
        '----- MOVIMENTI DETTAGLI
        '------------------------------------------------

        Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

        Dim DoseTotale As Decimal
        Dim DoseHa As Decimal = 0
        Dim SuperficieTotale As Decimal = CDbl(Txt_SupTrattata.Value.Replace(".", ","))

        For i = 0 To Dt_Dosi.Rows.Count - 1

            Movimento_Dettaglio = New Movimento_Dettaglio

            Movimento_Dettaglio.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento_Dettaglio.Piva = Agenda.Piva
            Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio.Data = Data
            Movimento_Dettaglio.Lav_Cod = Lav_Cod
            Movimento_Dettaglio.Cau_Mov = CAU_TRATTAMENTO

            Movimento_Dettaglio.Elem_Cod = INSETTI
            Movimento_Dettaglio.Pro_Cod = Dt_Dosi.Rows(i).Item("Ins_Cod")
            Movimento_Dettaglio.Mat_Cod = 0
            Movimento_Dettaglio.Udm_Cod = 38

            Movimento_Dettaglio.Contabilizzato = NONCONTABILE

            DoseHa = Dt_Dosi.Rows(i).Item("Dose_HA")
            Movimento_Dettaglio.Qta = DoseHa

            Movimento_Dettaglio.BaseCode = BaseCode
            Movimento_Dettaglio.TopCode = TopCode

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

            '------------------------------------------------
            '----- MOVIMENTI DESTINAZIONI
            '------------------------------------------------

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

            Dim xCalcolo_QD_SuperficieTotale As Decimal = (
                From ST In ListaImpianti
                Select CType(ST.Qta2, Decimal)
            ).Sum

            For j = 0 To ListaImpianti.Count - 1

                Movimento_Destinazione = New Movimento_Destinazione

                Movimento_Destinazione.Id_Agenda = objParametriAgenda.Id_Agenda
                Movimento_Destinazione.Data = Data

                Movimento_Destinazione.Piva = ListaImpianti(j).Piva
                Movimento_Destinazione.Sa_Cod = ListaImpianti(j).Sa_Cod
                Movimento_Destinazione.Appezza = ListaImpianti(j).Appezza
                Movimento_Destinazione.Id_Destinazione = ListaImpianti(j).ID_Reg
                Movimento_Destinazione.Tipo = 0
                Movimento_Destinazione.Qta = Math.Round(DoseHa * CDbl(ListaImpianti(j).Qta2), 0)
                Movimento_Destinazione.Qta2 = ListaImpianti(j).Qta2

                If xCalcolo_QD_SuperficieTotale <> 0 Then
                    Movimento_Destinazione.QuotaDistribuzione = ListaImpianti(j).Qta2 / xCalcolo_QD_SuperficieTotale
                End If

                Movimento_Destinazione.BaseCode = BaseCode
                Movimento_Destinazione.TopCode = TopCode

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(i).Movimenti_Destinazioni.Add(Movimento_Destinazione)
            Next

        Next


        '------------------------------------------------
        '------------------------------------------------
        '----- MOVIMENTO SCARICO
        '------------------------------------------------
        '------------------------------------------------

        If objParametriAgenda.Fabbricato <> "0" Then

            Dim sa_cod_magazzino As String = ""
            Dim fabbricatox_Cod As String = ""

            Dim magazzinoEsterno As Boolean = True
            If objParametriAgenda.Fabbricato <> "0" AndAlso Split(objParametriAgenda.Fabbricato, "|").Count = 3 AndAlso Split(objParametriAgenda.Fabbricato, "|")(2) <> objParametriAgenda.Piva Then
                'se magazzino è della azienda padre
                magazzinoEsterno = True
                Dim sa_cod_magazzino_predefinito_azienda As Integer = 0
                Dim fabbricatox_Cod_magazzino_predefinito_azienda As Integer = 0
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

            Movimento = New Movimento

            Movimento.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento.Piva = Agenda.Piva
            Movimento.Sa_Cod = sa_cod_magazzino
            ' Movimento.Sa_Cod = Sa_Cod sbagliato se magazzino in un altro centro
            Movimento.Data = Data
            Movimento.Lav_Cod = Lav_Cod

            Movimento.Cau_Mov = CAU_SCARICO
            Movimento.Mov_Desc = "Scarico Magazzino"
            Movimento.Mezzo = 1

            Movimento.BaseCode = BaseCode
            Movimento.TopCode = TopCode

            Agenda.Movimenti.Add(Movimento)

            '------------------------------------------------
            '----- MOVIMENTI DETTAGLI
            '------------------------------------------------

            Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

            For i = 0 To Dt_Dosi.Rows.Count - 1

                Movimento_Dettaglio = New Movimento_Dettaglio

                Movimento_Dettaglio.Id_Agenda = objParametriAgenda.Id_Agenda
                Movimento_Dettaglio.Piva = Agenda.Piva
                Movimento_Dettaglio.Sa_Cod = sa_cod_magazzino
                Movimento_Dettaglio.Data = Data
                Movimento_Dettaglio.Lav_Cod = Lav_Cod
                Movimento_Dettaglio.Cau_Mov = CAU_SCARICO

                Movimento_Dettaglio.Elem_Cod = INSETTI
                Movimento_Dettaglio.Pro_Cod = Dt_Dosi.Rows(i).Item("Ins_Cod")
                Movimento_Dettaglio.Mat_Cod = 0
                Movimento_Dettaglio.Udm_Cod = 38

                Movimento_Dettaglio.Contabilizzato = NONCONTABILE

                DoseTotale = Qta_da_scaricare(i)
                Movimento_Dettaglio.Qta = DoseTotale

                Movimento_Dettaglio.BaseCode = BaseCode
                Movimento_Dettaglio.TopCode = TopCode

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Add(Movimento_Dettaglio)

                '------------------------------------------------
                '----- MOVIMENTI DESTINAZIONI
                '------------------------------------------------
                Dim indice As Integer = Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli.Count - 1

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(indice).Movimenti_Destinazioni = New List(Of Movimento_Destinazione)

                Movimento_Destinazione = New Movimento_Destinazione

                Movimento_Destinazione.Id_Agenda = objParametriAgenda.Id_Agenda
                Movimento_Destinazione.Data = Data

                Movimento_Destinazione.Piva = objParametriAgenda.Piva
                Movimento_Destinazione.Sa_Cod = sa_cod_magazzino
                Movimento_Destinazione.Appezza = 0
                Movimento_Destinazione.Id_Destinazione = fabbricatox_Cod
                Movimento_Destinazione.Tipo = MAGAZZINO
                Movimento_Destinazione.Qta = DoseTotale

                Movimento_Destinazione.BaseCode = BaseCode
                Movimento_Destinazione.TopCode = TopCode

                Agenda.Movimenti(Agenda.Movimenti.Count - 1).Movimenti_Dettagli(indice).Movimenti_Destinazioni.Add(Movimento_Destinazione)

            Next

        End If

        Return Agenda
    End Function

    Private Sub fine_salvataggio(ByVal TipoSalvataggio As Integer)

        Session("UtilizzataRicetta") = False
        Select Case TipoSalvataggio
            Case 1
                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")


                Dim link As String = ""

                Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda.SitoOrigine
                Try

                    Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

                    If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.RegistazioneSmart Then
                        link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                               Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                               enum_PagineGiasOnline_2010.RegistazioneSmart,
                                               enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

                    ElseIf sitoorigine = Enum_SiteRedirector.GiasNG Then
                        AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
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

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, , _
                                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

                Insetti_Disposed()
                objParametriAgenda.Svuota_DatiOperazione()

                If sitoorigine <> Enum_SiteRedirector.GiasNG Then
                    Response.Redirect(CType(Master.Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda))
                End If


            Case 2
                Insetti_Disposed()
                objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                objParametriAgenda.Note = New List(Of Nota)
                objParametriAgenda.Movimenti = New List(Of Movimento)

                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")
                strJS.AppendLine("      BloccaSbloccaTotale(); ")
                strJS.AppendLine("     BloccaSbloccaChkImpollinatore();")
                strJS.AppendLine("      window.location = '../Operazioni/Distribuzione_Insetti.aspx'; ")
                strJS.AppendLine(" });")
                ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, , _
                                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))

            Case 3
                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")
                strJS.AppendLine("      BloccaSbloccaTotale(); ")
                strJS.AppendLine("     BloccaSbloccaChkImpollinatore();")
                strJS.AppendLine(" });")
                ScriptManager.RegisterStartupScript(CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel), CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).GetType(),
                                              String.Format("jQuery_{0}", CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar1"), UpdatePanel).ClientID), strJS.ToString, True)

                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, , _
                                  CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))


        End Select
    End Sub

    Private Function ControllaGiacenzaMagazzino()
        'controllo se ho già visualizzato il messaggio di alert 
        If HiddenVarie.Value <> "" Then
            HiddenVarie.Value = ""
            Return True
        End If

        'verifico se il magazzino è stato selezionato
        If objParametriAgenda.Fabbricato.Length > 3 Then

            Dim frm_InsCod As Integer = 0
            Dim qta_in_data As Decimal

            Dim Messaggio As String = ""

            Dim QtaTot As Decimal
            QtaTot = Txt_DoseTot_HA.Text
            frm_InsCod = CInt(Cmb_Insetti.SelectedValue)

            'guardo se la quantità è conforme per la data di intervento
            Dim objMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            qta_in_data = objMovDet.Verifica_Giacenze_Con_Magazzino_Esterno(Split(objParametriAgenda.Fabbricato, "|")(2), _
                                CInt(Split(objParametriAgenda.Fabbricato, "|")(1)), _
                                CInt(Split(objParametriAgenda.Fabbricato, "|")(0)), _
                                INSETTI, _
                                frm_InsCod, _
                                0, _
                                0, 0, LOTTO_NONDEFINITO, _
                                0, CInt(38), _
                                AGRODATAINIZIO, _
                                CDate(objParametriAgenda.Data), _
                                objParametriAgenda.Piva, _
                                objParametriAgenda.Sa_Cod, _
                                objParametriAgenda.Id_Agenda, _
                                objParametri_Server)
            If QtaTot > Math.Round(qta_in_data, 4) Then
                Messaggio = Messaggio & String.Format(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaQuantitàDiBX0BAlBX1BÈPariABX1, CStr(Cmb_Insetti.SelectedItem.Text), objParametriAgenda.Data, qta_in_data, "n")
            End If

            qta_in_data = objMovDet.Verifica_Giacenze_Con_Magazzino_Esterno(Split(objParametriAgenda.Fabbricato, "|")(2), _
                                CInt(Split(objParametriAgenda.Fabbricato, "|")(1)), _
                                CInt(Split(objParametriAgenda.Fabbricato, "|")(0)), _
                                INSETTI, _
                                frm_InsCod, _
                                0, _
                                0, 0, LOTTO_NONDEFINITO, _
                                0, CInt(38), _
                                AGRODATAINIZIO, _
                                Date.Today, _
                                objParametriAgenda.Piva, _
                                objParametriAgenda.Sa_Cod, _
                                objParametriAgenda.Id_Agenda, _
                                objParametri_Server)
            If QtaTot > Math.Round(qta_in_data, 4) Then
                Messaggio = Messaggio & String.Format(Resources.AgronicaAgenda_2010.ATTENZIONEBrLaGiacenzaAttualeDiBX0BX1X2Non1, CStr(Cmb_Insetti.SelectedItem.Text), qta_in_data, "n")
            End If

            If Messaggio <> "" Then
                '--------------BLOCCO SALVATAGGIO SE_SUPERA_GIACENZE-------------------
                If Not IsNothing(Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE")) AndAlso Session("UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE") = True Then

                    Dim MErrore As String
                    MErrore = "In base alle impostazioni utente NON è possibile usare un prodotto con giacenza non sufficiente! " & vbCr & Messaggio
                    Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & MErrore, Page, , _
                        CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                    Return False

                End If
                '---------------------------------------------------------------------
                Messaggio = Messaggio & vbCr & Resources.AgronicaAgenda_2010.BrBIProcedereUgualmenteIB
                'AgroSiNo
                ' VAnni: 28/2/2020: Non usare il resx sulla stringa "Salva"
                Messaggi.AgroSiNo(Messaggio, "Salva", Page, , Script_Giacenza_Magazzino)
                Return False
            End If

        End If

        Return True

    End Function

    Private Sub Ripristina_Dati_nei_Controlli()

        'Questa subroutine legge le informazioni dal database
        'e ripristina lo stato dei controlli sulla form.

        '------------------------------------------
        '----- Dichiarazione delle Variabili
        '------------------------------------------

        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim x As Integer = 0

        Dim Ins_Cod As Integer = 0
        Dim Ins_Des As String
        Dim Udm_Cod As Integer = 0
        Dim Udm_Des As String = ""
        Dim Dose As String = ""
        Dim Dose_Totale As Decimal
        Dim Dose_HA As Decimal

        Dim Impollinazione As Boolean = False
        Dim AvCod() As String
        Dim AvGru() As String
        Dim N_Avv As Integer = 0

        Dim Destinazione As Integer
        Dim SaCodDestinazione As Integer = 0

        Dim Sup_Tot As Decimal = 0
        Dim Sup_Tot_Sel As Decimal = 0

        '------------------------------------------
        '----- Recupero le informazioni
        '------------------------------------------

        Dim objAgenda As New Agenda_Operazione_Helper

        Dim Agenda As New Operazione_Agenda

        Agenda = objAgenda.Leggi(objParametriAgenda.Piva, _
                                     CInt(objParametriAgenda.Sa_Cod), _
                                     CInt(objParametriAgenda.Id_Agenda), _
                                     0, _
                                     objParametri_Server)

        objAgenda = Nothing

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

                Dim MovimentiCosti As List(Of Movimento) = New List(Of Movimento)
                objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)

                For i = 0 To Agenda.Movimenti.Count - 1

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case CAU_TRATTAMENTO

                            objParametriAgenda.Data = Agenda.Movimenti(i).Data

                            CType(Page.Master, Operazione).SetNota(Agenda.Movimenti(i).Mov_Desc)

                            Select Case Agenda.Movimenti(i).Modalita
                                Case 10
                                    'Totale
                                    Me.rbl_QtaTot.Checked = True
                                    Me.rbl_QtaDose.Checked = False
                                Case 11
                                    'dose
                                    Me.rbl_QtaDose.Checked = True
                                    Me.rbl_QtaTot.Checked = False
                            End Select


                            'MOVIMENTO_DETTAGLIO TECNICO x AVVERSITA
                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                                    If Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Av_Cod = -1 Then
                                        Impollinazione = True
                                        'controlli vari di coerenza
                                        If Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count > 1 Then
                                            Throw New Exception("Ci deve essere un solo mov dettaglio tecnico con avv -1 se è una impollinazione")
                                        End If
                                        If Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Av_Gru <> -1 Then
                                            Throw New Exception("Avcod e avgru devono essere entrambi -1 se è una impollinazione")
                                        End If
                                        Exit For
                                    End If
                                    ReDim Preserve AvCod(N_Avv)
                                    ReDim Preserve AvGru(N_Avv)
                                    AvCod(N_Avv) = Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Av_Cod.ToString
                                    AvGru(N_Avv) = Agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Av_Gru.ToString
                                    N_Avv += 1
                                Next

                            End If

                            'MOVIMENTI_DETTAGLI
                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                    Ins_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod
                                    Ins_Des = ""

                                    Dim objInsDes As New AgronicaCoreMetaSchemaDAL.InsettiUtili_R
                                    Dim DT_Ins As DataTable
                                    DT_Ins = objInsDes.Leggi(Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod, _
                                                               AGRODATAINIZIO, _
                                                              AGRODATAFINE, _
                                                              AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                               "", _
                                                               "", _
                                                               objParametri_Server)

                                    If DT_Ins IsNot Nothing AndAlso DT_Ins.Rows.Count > 0 Then
                                        Ins_Des = DT_Ins.Rows(0).Item("Ins_Des")
                                    End If

                                    Udm_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod

                                    Dim udmArea As String = AgronicaCoreMetaSchemaDAL.UDM_Helper.GetUdmSim_AREA(objParametri_Server, objParametri_Utenti)

                                    Udm_Des = "[n]"

                                    Dose = Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta

                                    'MOVIMENTI_DESTINAZIONI
                                    If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then

                                        'If i = 0 Then
                                        If objParametriAgenda.Impianti.Count = 0 Then
                                            For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1

                                                Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                                                objAppezzamento.Piva = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva
                                                objAppezzamento.Sa_Cod = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod
                                                objAppezzamento.Appezza = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza
                                                objAppezzamento.ID_Reg = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione

                                                objAppezzamento.Qta2 = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Qta2

                                                Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                                                Dim Dt_Imp As New DataTable
                                                Dt_Imp = objImp.Leggi(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Piva, _
                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod, _
                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Appezza, _
                                                             Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione, _
                                                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, _
                                                             "", "", objParametri_Server)
                                                objImp = Nothing

                                                If Dt_Imp.Rows.Count > 0 Then

                                                    If x = 0 Then
                                                        objParametriAgenda.Veg_Cod = CInt(Dt_Imp.Rows(0).Item("veg_cod"))
                                                    End If

                                                    If j = 0 Then
                                                        Sup_Tot += Dt_Imp.Rows(0).Item("sup_imp")
                                                    End If

                                                    If j = 0 Then
                                                        If objAppezzamento.Qta2 = 0 Then
                                                            Sup_Tot += Dt_Imp.Rows(0).Item("sup_imp")
                                                            'objAppezzamento.Qta2 = Dt_Imp.Rows(0).Item("sup_imp")
                                                            objAppezzamento.Qta2 = 0 'Superficie trattata = 0
                                                        Else
                                                            Sup_Tot_Sel += objAppezzamento.Qta2
                                                        End If
                                                    End If
                                                Else
                                                    j = 0
                                                End If

                                                objParametriAgenda.Impianti.Add(objAppezzamento)
                                                objParametriAgenda.salva()
                                            Next

                                        End If

                                    End If

                                    If Sup_Tot_Sel = 0 Then
                                        'per avere retrocompatibilità
                                        Sup_Tot_Sel = Sup_Tot
                                    End If

                                    '/ha
                                    Dose_Totale = Math.Round(Dose * Sup_Tot_Sel, 0)
                                    Dose_HA = Dose

                                    Dosi_Inserisci(Ins_Cod, _
                                                   Ins_Des, _
                                                    Dose_HA, _
                                                    Dose_Totale, _
                                                    Udm_Cod, _
                                                    Udm_Des)

                                Next

                                Txt_SupSelezionata.Value = Sup_Tot.ToString
                                Txt_SupTrattata.Value = Sup_Tot_Sel.ToString

                            End If

                            'seleziono le avversità
                            If N_Avv > 0 AndAlso Not Impollinazione Then

                                CaricaGriglia_Avversita()

                                Dim jj As Integer = 0
                                Dim kk As Integer = 0

                                If AvCod(0) > 0 Then
                                    'ho avversità singole
                                    For jj = 0 To N_Avv - 1
                                        For kk = 0 To GridViewAvversita.Rows.Count - 1
                                            If GridViewAvversita.Rows(kk).Cells(1).Text = AvCod(jj) Then
                                                CType(GridViewAvversita.Rows(kk).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True
                                                Exit For
                                            End If
                                        Next
                                    Next
                                Else
                                    For jj = 0 To N_Avv - 1
                                        For kk = 0 To GridViewAvversita.Rows.Count - 1
                                            If GridViewAvversita.Rows(kk).Cells(2).Text = AvGru(jj) Then
                                                CType(GridViewAvversita.Rows(kk).FindControl("ChkSelezionaAvversita"), CheckBox).Checked = True
                                                Exit For
                                            End If
                                        Next
                                    Next
                                End If

                            End If

                            If Impollinazione Then
                                'Cella_Avversita.Visible = False
                                ChkImpollinatore.Checked = True
                                'ChkImpollinatoreCheckedChanged()
                            End If


                        Case CAU_SCARICO

                            'MOVIMENTI_DETTAGLI
                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                    Select Case Agenda.Movimenti(i).Movimenti_Dettagli(j).Elem_Cod

                                        Case INSETTI
                                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
                                                For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1
                                                    Destinazione = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione
                                                    SaCodDestinazione = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod
                                                    objParametriAgenda.Fabbricato = Destinazione.ToString & "|" & SaCodDestinazione.ToString & "|" & Agenda.Piva
                                                Next
                                            End If

                                        Case FERTILIZZANTI
                                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
                                                For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1
                                                    Destinazione = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione
                                                    SaCodDestinazione = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod
                                                    objParametriAgenda.Fabbricato = Destinazione.ToString & "|" & SaCodDestinazione.ToString & "|" & Agenda.Piva
                                                Next
                                            End If


                                        Case FORMULATI
                                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni) Then
                                                For x = 0 To Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count - 1
                                                    Destinazione = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Id_Destinazione
                                                    SaCodDestinazione = Agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(x).Sa_Cod
                                                    objParametriAgenda.Fabbricato = Destinazione.ToString & "|" & SaCodDestinazione.ToString & "|" & Agenda.Piva
                                                Next
                                            End If


                                        Case Else
                                            '----COSTO ACCESSORIO---------------------
                                            'è un movimento dovuto ad un costo accessorio
                                            'Throw New NotImplementedException
                                            MovimentiCosti.Add(Agenda.Movimenti(i))
                                            Exit For

                                    End Select

                                Next

                            End If



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

                'gestisco la selezione del fabbricato dell'azienda esterna se l'operazione era stata registrata con quella
                Dim util As New Utility_NS.Utility_Operazioni()
                util.ImpostaFabbricatoDelMagazzinoEsternoSePresente(Agenda, objParametriAgenda, objParametri_Server)

            End If


        End If

        'aggiorno il Datagrid delle dosi
        Griglia_Dosi_Data_Bind()

        Dim script As New StringBuilder

        script.AppendLine("$(document).ready(function () { ")
        script.AppendLine("     BloccaSbloccaTotale();")
        script.AppendLine("     BloccaSbloccaChkImpollinatore();")
        script.AppendLine("}); ")

        ScriptManager.RegisterStartupScript(UpdatePanelMiscela, UpdatePanelMiscela.GetType(),
                                         String.Format("jQuery_{0}", UpdatePanelMiscela.ClientID), script.ToString, True)

    End Sub


#Region "GESTIONE DOSI"
    ''' <summary>
    ''' DOSI
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CaricaGriglia_Dosi()
        '----- Definizione delle variabili

        Dim Dt As New DataTable
        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("Ins_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ins_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Udm_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dose_HA", GetType(Decimal)))
        Dt.Columns.Add(New DataColumn("Qta_Tot", GetType(Decimal)))

        '----- Definisco l'insieme di colonne che costituiscono la chiave della tabella
        'Vettore di DataColumn
        Dim DtKeys(0) As DataColumn
        'Valorizzo le celle del vettore
        DtKeys(0) = Dt.Columns("Ins_Cod")

        'Assegno il vettore delle chiavi al DataTable
        Dt.PrimaryKey = DtKeys
        '----- Salvo il DataTable dentro il session
        Session("vs_dtDosi") = Dt
        Griglia_Dosi_Data_Bind()

    End Sub


    ''' <summary>
    ''' Bind Delle Dosi
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Griglia_Dosi_Data_Bind()
        GridView_Dosi.DataSource = Session("vs_dtDosi")
        GridView_Dosi.DataBind()
    End Sub

    ''' <summary>
    ''' inserisce una nuova riga all'interno del dt delle dosi mantenuto in sessione
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Dosi_Inserisci(ByVal Ins_Cod As String, _
                           ByVal Ins_Des As String, _
                           ByVal Dose_HA As Decimal, _
                           ByVal Qta_Tot As Decimal, _
                           ByVal Udm_Cod As Integer, _
                           ByVal Udm_Des As String)

        '----- Dimensiono le variabili

        Dim Dt As DataTable
        Dim Dr As DataRow
        Dim IndiceRiga As Integer = 0
        Dim ElementoPresente As Boolean
        Dim Messaggio As String

        '----- Recupero i dati 

        '   Li ho gia' tutti ...
        '   Non ho la necessita' di recuperare nulla
        '   Lascio questa nota per usi futuri di Copia e Incolla

        '----- Verifico che il formulato non sia gia' presente nel datatable

        'Inizializzo
        ElementoPresente = False

        'Recupero il datatable
        Dt = Session("vs_dtDosi")

        'Ciclo nelle righe del datatable
        For IndiceRiga = 0 To Dt.Rows.Count - 1
            If Dt.Rows(IndiceRiga).Item("Ins_Cod") = Ins_Cod Then
                ElementoPresente = True
                Messaggio = Resources.AgronicaAgenda_2010.NonEConsentitoInserireUnFormulatoGiaPresen
                Messaggi.AgroMsgBox(Messaggio, Page, , updateDoseInserisci)
                Exit Sub
            End If
        Next


        '----- Inserisco il nuovo record

        'Creo una nuova riga
        Dr = Dt.NewRow

        'Definisco i valori
        Dr.Item("Ins_Cod") = Ins_Cod
        Dr.Item("Ins_Des") = Ins_Des

        Dr.Item("Udm_Cod") = Udm_Cod
        Dr.Item("Udm_Des") = Udm_Des

        Dr.Item("Dose_HA") = Dose_HA
        Dr.Item("Qta_Tot") = Qta_Tot

        Dt.Rows.Add(Dr)

        '----- Salvo il DataTable dentro il session

        Session("vs_dtDosi") = Dt

        '----- Azzero i controlli di provenienza dei dati

        Me.Txt_Insetti.Text = ""
        Me.Txt_Dose_HA.Text = ""
        Me.Txt_DoseTot_HA.Text = ""
        Me.Lbl_Giacenza.Text = ""
        Me.Lbl_Num_Insetti.InnerText = ""

    End Sub


    ''' <summary>
    ''' Cancellazione / Modifica di una dose
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub GridView_Dosi_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView_Dosi.RowCommand

        Dim IndiceRigaGriglia As Integer = 0
        Dim InsCod As Integer = 0
        Dim InsDes As String = ""

        Dim Dt As DataTable
        Dim Dr As DataRow

        Dim Dose As Decimal = 0
        Dim DoseTotale As Decimal = 0
        Dim UdmCod As Integer = 0

        'Recupero l'indice di riga del datagrid
        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        'Recupero il datatable
        Dim Dt_Dosi As DataTable

        Dt_Dosi = Session("vs_dtDosi")

        If Dt_Dosi IsNot Nothing Then

            InsCod = Dt_Dosi.Rows(IndiceRigaGriglia).Item("Ins_Cod")
            InsDes = Dt_Dosi.Rows(IndiceRigaGriglia).Item("Ins_Des")
            UdmCod = Dt_Dosi.Rows(IndiceRigaGriglia).Item("Udm_Cod")
            Dose = Dt_Dosi.Rows(IndiceRigaGriglia).Item("Dose_HA")
            DoseTotale = Dt_Dosi.Rows(IndiceRigaGriglia).Item("Qta_Tot")

            Select Case e.CommandName

                Case "Modifica"

                    '----RICALCOLA---- e arrotonda
                    Me.Txt_Dose_HA.Text = Math.Round(Dose, 4)
                    Me.Txt_DoseTot_HA.Text = Math.Round(DoseTotale, 4)
                    '----RICALCOLA---- 

                    '---------------------------------------------
                    ' ELIMINO LA RIGA DAL DT
                    Dt = Session("vs_dtDosi")
                    'Trovo la riga da cancellare    (chiave = FrCod)
                    Dr = Dt.Rows.Find(InsCod)
                    Dr.Delete()

                    Session("vs_dtDosi") = Dt

                    Griglia_Dosi_Data_Bind()

                    Cmb_Insetti.Items.Add(New ListItem(InsDes, InsCod))
                    Cmb_Insetti.SelectedValue = InsCod

                    Lbl_Giacenza.Text = ""
                    Dim Giacenza As Decimal = 0
                    Select Case objParametriAgenda.Fabbricato
                        Case Is <> "0"
                            Giacenza = Giacenza_Insetti(CInt(InsCod))
                            If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
                                Giacenza += DoseTotale
                            End If
                            Lbl_Giacenza.Text = Math.Round(Giacenza, 4).ToString
                    End Select


                Case "Cancella"

                    '---------------------------------------------
                    ' ELIMINO LA RIGA DAL DT
                    Dt = Session("vs_dtDosi")

                    'Trovo la riga da cancellare    (chiave = FrCod)
                    Dr = Dt.Rows.Find(InsCod)

                    Dr.Delete()

                    Session("vs_dtDosi") = Dt

                    Griglia_Dosi_Data_Bind()

            End Select

        End If

        Dim STR_UpdatePanelDose As New StringBuilder
        STR_UpdatePanelDose.AppendLine("BloccaSbloccaTotale();")
        STR_UpdatePanelDose.AppendLine("     BloccaSbloccaChkImpollinatore();")
        ScriptManager.RegisterStartupScript(UpdatePanelMiscela, UpdatePanelMiscela.GetType(),
                                      String.Format("jQuery_{0}", UpdatePanelMiscela.ClientID), STR_UpdatePanelDose.ToString, True)


    End Sub


#End Region

    'Private Sub ChkImpollinatore_CheckedChanged(sender As Object, e As System.EventArgs) Handles ChkImpollinatore.CheckedChanged

    '    ChkImpollinatoreCheckedChanged()

    'End Sub

    'Private Sub ChkImpollinatoreCheckedChanged()
    '    If ChkImpollinatore.Checked Then
    '        Cella_Avversita.Visible = False
    '    Else
    '        Cella_Avversita.Visible = True
    '    End If
    'End Sub

    Public Sub CaricaComboFiltriAggiuntivi()
        ComboFiltriAggiuntiviAgenda1.Lav_Cod = objParametriAgenda.Lav_Cod
        ComboFiltriAggiuntiviAgenda1.CaricaComboFiltriAggiuntivi()
        ComboFiltriAggiuntiviAgenda1.ddl_FiltriAggiuntivi.SelectedValue = "0"
    End Sub

    Private Sub AggiornaFiltroRicerca()
        CaricaComboFiltriAggiuntivi()
    End Sub


    Private Sub btn_cerca_avv_Click(sender As Object, e As System.EventArgs) Handles btn_cerca_avv.Click

        Dim DtAvv As DataTable = Session("DtAvv")

        Dim DrAvv() As DataRow
        Dim DtAvvF As New DataTable

        If DtAvv IsNot Nothing AndAlso DtAvv.Rows.Count > 0 Then
            DrAvv = DtAvv.Select("av_des like '%" & Txt_Avv.Text & "%'")
            If DrAvv IsNot Nothing AndAlso DrAvv.Length > 0 Then
                DtAvvF = DtAvv.Clone
                For i = 0 To DrAvv.Length - 1
                    DtAvvF.ImportRow(DrAvv(i))
                Next
            End If
        End If

        GridViewAvversita.DataSource = DtAvvF
        GridViewAvversita.DataBind()

    End Sub


End Class