


Imports System.Web
Imports System.Web.Services
Imports System.Xml
Imports AgronicaControlli_2010
Imports AgronicaConversioneCartografiaGias.Agronica
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreMeteoBiz
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModelsSTD.attivita.Attivita
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq



Public Class RilievoPiogge
    Inherits System.Web.UI.Page


    Private Shared Function GetDoorkey() As String
        Dim Doorkey As String = "Y4h8u3B5w2"
        Return Doorkey
    End Function

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub



    Dim objParametri_Server, objParametri_Utenti, objParametri_Super_Server As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda
    Dim Id_Agenda_Old As Integer
    Public Master_Operazione As Operazione
    Dim Movimento_Dettaglio_Contabilizzato As Integer = 1 '1 se data<corrente -1 se data futura
    Dim Movimento_Dettaglio_Pendente As Integer = 3
    Dim Movimento_Dettaglio_Extra_Date As Date
    Dim Movimento_Dettaglio_Anno As Integer = 1900


    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Master_Operazione = CType(Page.Master, Operazione)

        'AddHandler Master_Operazione.Property_BTN_ChangeData.Click, AddressOf Me.aggiornaPerDataScadenza
        AddHandler Master_Operazione.Property_ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
        AddHandler Master_Operazione.Property_BTN_CentroAziendale.Click, AddressOf Me.CambioCentro
        AddHandler Master_Operazione.Property_ImgBtn_Salva.Click, AddressOf Me.SalvaTutto
        AddHandler CType(Page.Master, MasterPage).PreRender, AddressOf Me.MasterUnload

    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim strJS As New StringBuilder
        strJS.AppendLine("$(document).ready(function () { ")
        strJS.AppendLine("     $('#chkSelezionaTuttiCentri').click(function (){ ")
        strJS.AppendLine("         SelezionaDeselezionaTuttiCentri();")
        strJS.AppendLine("      });")
        strJS.AppendLine("     $('#chkSelezionaTuttiRilievi').click(function (){ ")
        strJS.AppendLine("         SelezionaDeselezionaTuttiRilievi();")
        strJS.AppendLine("       });")
        strJS.AppendLine("     $('.ChkSelezionaRilievo').click(function (){ ")
        strJS.AppendLine("         ChkSelezionaRilievo_Click();")
        strJS.AppendLine("     });")

        strJS.AppendLine("    $('#" & Text_BoxDataA.ClientID & "').datepicker({ ")
        strJS.AppendLine("    dateFormat:  'dd/mm/yy',")
        strJS.AppendLine("    disabled: false,")
        strJS.AppendLine("    changeMonth: true,")
        strJS.AppendLine("    changeYear: true")
        strJS.AppendLine("  });")

        strJS.AppendLine("    $('#" & Text_BoxDataDa.ClientID & "').datepicker({ ")
        strJS.AppendLine("    dateFormat:  'dd/mm/yy',")
        strJS.AppendLine("    disabled: false,")
        strJS.AppendLine("    changeMonth: true,")
        strJS.AppendLine("    changeYear: true")
        strJS.AppendLine("  });")

        strJS.AppendLine("  $.datepicker.regional['it'];")


        strJS.AppendLine(" });")
        ScriptManager.RegisterStartupScript(Master_Operazione.Property_UpdatePanelPerScript, Master_Operazione.Property_UpdatePanelPerScript.GetType(),
                                      String.Format("jQuery_{0}", Master_Operazione.Property_UpdatePanelPerScript.ClientID), strJS.ToString, True)



        verificoCredenzialiDiAccesso()
        inizializzoObjParametri()
        inizializzoParametriPagina()

        If Not IsPostBack Then


            VerificaPermessi()
            LeggiImpostazioni()
            caricaControlli()
            disabilitaControlli()

        Else

            ColoroLaGriglia(GridView_Piogge)
            ColoroLaGrigliaCentriPerMeteo()


        End If


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
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        '---
    End Sub


    Private Sub inizializzoParametriAgenda()
        objParametriAgenda = New ParametriAgenda
        'objParametriAgenda.Leggi()
        Id_Agenda_Old = objParametriAgenda.Id_Agenda
        objParametriAgenda.OperazioneMulticentro = True
    End Sub


    Private Sub inizializzoParametriPagina()

        inizializzoParametriAgenda()

        Dim objOperazioniLeggi As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        Dim Lav_Des As String = objOperazioniLeggi.LavorazioneDes_from_LavorazioneCod(objParametriAgenda.Lav_Cod, objParametri_Server)
        objParametriAgenda.Lav_Des = Lav_Des
        Master_Operazione.Property_Lbl_Titolo.Text = Lav_Des

        'valori copiati da agenda vecchia

        Movimento_Dettaglio_Pendente = 3
        Movimento_Dettaglio_Extra_Date = AGRODATAINIZIO
        Movimento_Dettaglio_Anno = 1900

        Select Case CInt(objParametriAgenda.Lav_Cod)

            Case LAVCOD_RILIEVO_PIOGGE
                objParametriAgenda.Cau_Mov = CStr(enum_Agenda_Causali.RILIEVO_CAMPO)


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

        'combo ore
        AgronicaCoreUtility.CaricaListControl.Ore(CType(Me.DropDownOre, ListControl), False, "", "",
                                          "", "", objParametri_Server)
        Me.DropDownOre.SelectedIndex = Me.DropDownOre.Items.IndexOf(Me.DropDownOre.Items.FindByValue("23"))


        Select Case CInt(objParametriAgenda.Lav_Cod)
            Case LAVCOD_RILIEVO_PIOGGE
                Me.LabelInfo1.Text = Resources.AgronicaAgenda_2010.NonÈPossibileSalvareDueRilieviPioggeSulloS
                Me.LabelInfo2.Text = Resources.AgronicaAgenda_2010.LeRigheRosaIndicanoCheÈPresenteUnRilievoPi
                Me.LabelInfo2.BackColor = Drawing.Color.Pink

            Case Else
                Throw New NotImplementedException
        End Select


        Select Case objParametriAgenda.Tipo_Operazione

            Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura

            Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica, TipiEnumerativi.enum_TipoOperazioneDB.Lettura
                RipristinaControlliDaAgenda()
                Master_Operazione.CaricaCostiAccessori()


        End Select
    End Sub


    Private Sub disabilitaControlli()

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permessoSTazioni As Boolean = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"),
                                    Session("ASG_IdServizio"),
                                    enum_Security_Attivita.Consultazione_Meteo_Dati_Stazioni_Metos,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)

        If Not permessoSTazioni Then
            RBL_TipoRilievo.Items.RemoveAt(2)
        End If

        'Master_Operazione.Property_RBL_Salva.Enabled = False
        Master_Operazione.Property_Div_ProvenienzaRisorse.Visible = False
        Master_Operazione.Property_Div_Specie.Visible = False
        Master_Operazione.Property_GridView_Impianti.Visible = False
        Master_Operazione.Property_NoteGiustPanel.Visible = False
        Master_Operazione.Property_CBL_Consigli.Visible = False
        Master_Operazione.Property_txt_Note.Visible = False
        Master_Operazione.Property_UpdatePanelData.Visible = False

        Me.sfondoverde.Visible = True



        'impostazioni in base operazione di creazione/modifica..
        Select Case objParametriAgenda.Tipo_Operazione

            'impostazioni in base alla lavorazione


            Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura

                Master_Operazione.Property_RBL_Salva.Visible = True
                Master_Operazione.Property_Box_Salva.Visible = True

                Me.RBL_TipoRilievo.Visible = True
                Me.RBL_TipoRilievo.SelectedValue = "0"

                Me.sfondoverde.Visible = True
                Me.sfondoverdeCentri.Visible = True

                AbilitaStep2(False)
                AbilitaStep1(False)

            Case TipiEnumerativi.enum_TipoOperazioneDB.Modifica

                AbilitaTutto(False)

                Master_Operazione.Property_Box_Salva.Enabled = True 'true per modifica
                Master_Operazione.Property_Box_Salva.Visible = True 'true per modifica
                Master_Operazione.Property_ImgBtn_Salva.Enabled = True 'true per modifica
                Master_Operazione.Property_RBL_Salva.Enabled = False

                Me.NoteTabella.Visible = True
                Me.LabelInfo1.Visible = True
                Me.ImageInfo1.Visible = True
                Me.ImageInfo2.Visible = True


                'Master_Operazione.Property_CBL_Consigli.Enabled = True

                GridView_Piogge.Visible = True
                GridView_Piogge.Enabled = True


            Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura

                AbilitaTutto(False)
                'Master_Operazione.Property_CBL_Consigli.Enabled = False
                GridView_Piogge.Visible = True

        End Select




    End Sub


    Sub AbilitaTutto(ByVal abilita As Boolean)
        Master_Operazione.Property_Box_Salva.Enabled = abilita 'true per modifica
        Master_Operazione.Property_Box_Salva.Visible = abilita 'true per modifica
        Master_Operazione.Property_ImgBtn_Salva.Enabled = abilita 'true per modifica
        Master_Operazione.Property_RBL_Salva.Enabled = abilita

        'Master_Operazione.Property_txt_DataOperazione.Enabled = abilita

        Me.UpdatePanelFiltroCentri.Visible = abilita

        Me.RBL_TipoRilievo.Visible = False
        Me.RBL_TipoRilievo.Enabled = False
        Me.RBL_TipoRilievo.SelectedValue = "0"

        Me.sfondoverde.Visible = abilita
        Me.ImageButton_MostraRilievi.Enabled = abilita
        Me.ImageButton_MostraRilievi.Visible = abilita
        Me.ImageButton_Sblocca.Enabled = abilita
        Me.ImageButton_Sblocca.Visible = abilita

        Me.sfondoverdeCentri.Visible = abilita
        Me.IMGB_InserisciCentri.Enabled = abilita
        Me.IMGB_InserisciCentri.Visible = abilita
        Me.IMGB_RimuoviCentri.Enabled = abilita
        Me.IMGB_RimuoviCentri.Visible = abilita

        Me.TabellaFiltriMeteo.Visible = abilita

        Me.NoteTabella.Visible = abilita

        Me.LabelInfo1.Visible = abilita
        Me.LabelInfo2.Visible = abilita
        Me.ImageInfo1.Visible = abilita
        Me.ImageInfo2.Visible = abilita

        'Master_Operazione.Property_ImgBtn_Costi.Enabled = False

        Master_Operazione.Property_txt_Note.Enabled = abilita 'true per modifica


        Master_Operazione.Property_BTN_ComboOperazione.Enabled = abilita
        Master_Operazione.Property_ComboOperazione.Enabled = abilita

        'Master_Operazione.Property_CBL_Consigli.Enabled = abilita

        BloccaComboJavaScript(Not abilita, Not abilita, Not abilita, Not abilita)


        GridView_Piogge.Visible = abilita
        GridView_Piogge.Enabled = abilita


    End Sub


#End Region




#Region "Operazioni sequenza Carica Centri"


    Private Sub CambioCentro(sender As Object, e As EventArgs)

    End Sub


    Private Sub RBL_TipoRilievo_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles RBL_TipoRilievo.SelectedIndexChanged
        Inserisci_Centri()
    End Sub


    Protected Sub IMGB_RimuoviCentri_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles IMGB_RimuoviCentri.Click
        Rimuovi_Centri()
    End Sub

    Private Sub Rimuovi_Centri()
        Rimuovi_Rilievi()
        AbilitaStep1(False)
    End Sub


    Protected Sub IMGB_InserisciCentri_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles IMGB_InserisciCentri.Click
        Inserisci_Centri()
    End Sub

    Private Sub Inserisci_Centri()
        CaricaGrigliaCentri()
        AbilitaStep1(True)
    End Sub


    Private Sub CaricaGrigliaCentri()
        If Me.RBL_TipoRilievo.SelectedValue = "0" Then
            'dai meteo
            CaricaGrigliaCentriPerMeteo()
        ElseIf Me.RBL_TipoRilievo.SelectedValue = "1" Then
            'manuale
            CaricaGrigliaCentriPerManuale()
        ElseIf Me.RBL_TipoRilievo.SelectedValue = "2" OrElse RBL_TipoRilievo.SelectedValue = "3" Then
            'manuale
            CaricaGrigliaCentriPerStazioni()
        Else
            Throw New NotImplementedException
        End If
    End Sub


    Private Sub AbilitaStep1(ByVal abilita As Boolean)



        Me.TabellaFiltriMeteo.Visible = False

        'da visualizzare quando si preme
        Me.ImageButton_MostraRilievi.Visible = abilita
        Me.IMGB_RimuoviCentri.Visible = abilita

        'da disabilitare quando si preme caricamento centri
        'Master_Operazione.Property_BTN_CentroAziendale.Enabled = Not abilita
        'Master_Operazione.Property_ComboCentroAziendale.Enabled = Not abilita
        'BloccaComboJavaScript(abilita, Not abilita, Not abilita, Not abilita)
        Me.RBL_TipoRilievo.Enabled = Not abilita

        'da nascondere quando si preme
        Me.LabelInserisciCentri.Visible = Not abilita
        Me.IMGB_InserisciCentri.Visible = Not abilita


        'sempre false
        Me.ImageButton_Sblocca.Visible = False 'sempre false, si attiva solo quando si preme mostra rilievi


        'impostazioni in base al tipo di rilievo piogge
        If Me.RBL_TipoRilievo.SelectedValue = "0" Then
            'dati meteo

            'da visualizzare quando si preme 
            Me.TabellaFiltriMeteo.Visible = abilita
            LabelCaricaPioggiaDaMeteo.Visible = abilita
            GridViewCentriMeteo.Visible = abilita
            GridViewCentriManuale.Visible = Not abilita

            'nascondere sempre per meteo
            LabelCaricaPioggiaManuale.Visible = False
            GridViewCentriManuale.DataSource = Nothing
            GridViewCentriManuale.DataBind()

            If abilita Then

            Else
                'se premuto sblocca rimuovo la griglia
                GridViewCentriMeteo.DataSource = Nothing
                GridViewCentriMeteo.DataBind()
            End If

        ElseIf Me.RBL_TipoRilievo.SelectedValue = "1" Then

            'viaualizzare se si preme
            LabelCaricaPioggiaManuale.Visible = abilita
            GridViewCentriManuale.Visible = abilita
            GridViewCentriMeteo.Visible = Not abilita

            'nascondere sempre per manuale
            Me.TabellaFiltriMeteo.Visible = False
            LabelCaricaPioggiaDaMeteo.Visible = False
            GridViewCentriMeteo.DataSource = Nothing
            GridViewCentriMeteo.DataBind()

            'manuale
            If abilita Then

            Else
                'se premuto sblocca rimuovo la griglia

                GridViewCentriManuale.DataSource = Nothing
                GridViewCentriManuale.DataBind()
            End If

        ElseIf Me.RBL_TipoRilievo.SelectedValue = "2" OrElse RBL_TipoRilievo.SelectedValue = "3" Then
            'dati stazioni

            'da visualizzare quando si preme 
            Me.TabellaFiltriMeteo.Visible = abilita
            LabelCaricaPioggiaDaMeteo.Visible = abilita
            LabelCaricaPioggiaDaMeteo.Visible = abilita
            GridViewCentriStazioni.Visible = abilita


            'nascondere sempre per manuale
            GridViewCentriMeteo.Visible = Not abilita
            GridViewCentriMeteo.DataSource = Nothing
            GridViewCentriMeteo.DataBind()
            'nascondere sempre per meteo
            LabelCaricaPioggiaManuale.Visible = Not abilita
            GridViewCentriManuale.DataSource = Nothing
            GridViewCentriManuale.DataBind()

            If abilita Then

            Else
                'se premuto sblocca rimuovo la griglia
                GridViewCentriStazioni.DataSource = Nothing
                GridViewCentriStazioni.DataBind()
            End If
        Else
            Throw New NotImplementedException
        End If


    End Sub


    Private Sub CaricaGrigliaCentriPerManuale()
        Dim Dt As New DataTable("Piogge")

        Dt.Columns.Add("PIVA", GetType(String))
        Dt.Columns.Add("Rag_Soc", GetType(String))
        Dt.Columns.Add("Sa_Cod", GetType(Integer))
        Dt.Columns.Add("Sa_nome", GetType(String))

        Dim i As Integer = 0
        Dim Piva As String = objParametriAgenda.Piva
        Dim Sa_Cod As Integer = CInt(Master_Operazione.Property_ComboCentroAziendale.Valore_Combo)
        Dim DTCentri As DataTable = New AgronicaCoreAnagrafeDAL.CentriAziendali_Read().Leggi(Piva, Sa_Cod, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", " Sa_Nome ", objParametri_Server)
        For i = 0 To DTCentri.Rows.Count - 1
            Dim row As DataRow = Dt.NewRow
            row = Dt.NewRow
            row.Item("PIVA") = DTCentri.Rows(i).Item("PIVA")
            row.Item("Rag_Soc") = New AgronicaCoreAnagrafeDAL.Imprese_Read().RagSoc_from_Piva(DTCentri.Rows(i).Item("PIVA"), objParametri_Server)
            row.Item("Sa_Cod") = DTCentri.Rows(i).Item("Sa_Cod")
            row.Item("Sa_nome") = DTCentri.Rows(i).Item("Sa_nome")

            Dt.Rows.Add(row)
        Next

        Dim DtKeys(1) As String

        DtKeys(0) = "PIVA"
        DtKeys(1) = "Sa_Cod"

        GridViewCentriManuale.DataKeyNames = DtKeys

        GridViewCentriManuale.DataSource = Dt
        GridViewCentriManuale.DataBind()

        'coloro le righe
        Dim i2 As Integer = 0
        For i2 = 0 To Me.GridViewCentriManuale.Rows.Count - 1
            If i2 Mod 2 = 0 Then
                Me.GridViewCentriManuale.Rows(i2).BackColor = Drawing.Color.Beige
            End If
        Next


    End Sub


    Private Sub CaricaGrigliaCentriPerMeteo()
        Dim dt As DataTable = InizializzaGrigliaCentriPerMeteo()
        Dim i As Integer = 0
        Dim Piva As String = objParametriAgenda.Piva
        Dim Sa_Cod As Integer = CInt(Master_Operazione.Property_ComboCentroAziendale.Valore_Combo)
        Dim DTCentri As DataTable = New AgronicaCoreAnagrafeDAL.CentriAziendali_Read().Leggi(Piva, Sa_Cod, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", " Sa_Nome ", objParametri_Server)
        For i = 0 To DTCentri.Rows.Count - 1
            Dim row As DataRow = dt.NewRow
            row = dt.NewRow
            row.Item("PIVA") = DTCentri.Rows(i).Item("PIVA")
            row.Item("Rag_Soc") = New AgronicaCoreAnagrafeDAL.Imprese_Read().RagSoc_from_Piva(DTCentri.Rows(i).Item("PIVA"), objParametri_Server)
            row.Item("Sa_Cod") = DTCentri.Rows(i).Item("Sa_Cod")
            row.Item("Sa_nome") = DTCentri.Rows(i).Item("Sa_nome")
            row.Item("X") = DTCentri.Rows(i).Item("X")
            row.Item("Y") = DTCentri.Rows(i).Item("Y")

            dt.Rows.Add(row)
        Next
        finalizzaGrigliaCentriPerMeteo(dt)


        'caricotextbox
        For i = 0 To GridViewCentriMeteo.Rows.Count - 1

            CType(GridViewCentriMeteo.Rows(i).Cells(3).Controls(1), TextBox).Text = CStr(DTCentri.Rows(i).Item("X"))
            CType(GridViewCentriMeteo.Rows(i).Cells(4).Controls(1), TextBox).Text = CStr(DTCentri.Rows(i).Item("Y"))
        Next

    End Sub


    Private Sub RiempiComboConStazioni(ByVal ddl As DropDownList, ByVal dt As DataTable, ByVal piva As String, ByVal sa_cod As Integer)
        'Pulisco il controllo
        ddl.Items.Clear()

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return
        End If

        'l'ordinamento (NULL LAST) viene dalla query
        Dim txt As String
        For Each row In dt.Rows

            txt = row("Descrizione") & " (Distanza "
            If IsDBNull(row("Distanza")) OrElse CInt(row("Distanza")) = CInt("-1") Then
                txt &= "non disponibile"
            Else
                txt &= CInt(CInt(row("Distanza")) / 1000) & " km"
            End If
            txt &= ")"
            txt &= IIf(CDate(row("data_ultimo_agg")) > AGRODATAINIZIO, " (agg. al " & CStr(CDate(row("data_ultimo_agg"))) & ")", "")
            txt &= IIf(String.IsNullOrEmpty(row("fornitore").ToString), "", " - " & row("fornitore"))

            ddl.Items.Add(New ListItem() With {
                        .Text = txt,
                        .Value = row("id")
                    })
        Next

        '***************************************************************************
        'GABRIELE 2023 10 06
        'OBSOLETO
        '***************************************************************************
        'If sa_cod > 0 Then
        '    Dim reader = New AgronicaCoreMeteoDAL.DSS_Centri_Aziendali_Agronica_Stazioni_Meteo_R
        '    Dim def_staz = reader.Leggi("Piva_SuperUser = '" & objParametri_Server.PivaSuperUser & "' AND piva = '" & piva & "' AND sa_cod = " & sa_cod.ToString, "", objParametri_Server)
        '    If def_staz IsNot Nothing AndAlso def_staz.Rows.Count > 0 Then
        '        ddl.SelectedValue = def_staz.Rows(0)("Stazione_Cod")
        '    End If
        'End If
    End Sub


    Private Sub CaricaGrigliaCentriPerStazioni()

        Dim Dt As New DataTable("Piogge")

        Dt.Columns.Add("PIVA", GetType(String))
        Dt.Columns.Add("Rag_Soc", GetType(String))
        Dt.Columns.Add("Sa_Cod", GetType(Integer))
        Dt.Columns.Add("Sa_nome", GetType(String))
        Dt.Columns.Add("Long", GetType(Decimal))
        Dt.Columns.Add("Lat", GetType(Decimal))

        Dim i As Integer = 0
        Dim Piva As String = objParametriAgenda.Piva
        Dim Sa_Cod As Integer = CInt(Master_Operazione.Property_ComboCentroAziendale.Valore_Combo)
        Dim DTCentri As DataTable = New AgronicaCoreAnagrafeDAL.CentriAziendali_Read().Leggi(Piva, Sa_Cod, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", " Sa_Nome ", objParametri_Server)
        For i = 0 To DTCentri.Rows.Count - 1
            Dim row As DataRow = Dt.NewRow
            row = Dt.NewRow
            row.Item("PIVA") = DTCentri.Rows(i).Item("PIVA")
            row.Item("Rag_Soc") = New AgronicaCoreAnagrafeDAL.Imprese_Read().RagSoc_from_Piva(DTCentri.Rows(i).Item("PIVA"), objParametri_Server)
            row.Item("Sa_Cod") = DTCentri.Rows(i).Item("Sa_Cod")
            row.Item("Sa_nome") = DTCentri.Rows(i).Item("Sa_nome")
            row.Item("Long") = DTCentri.Rows(i).Item("Long")
            row.Item("Lat") = DTCentri.Rows(i).Item("Lat")

            Dt.Rows.Add(row)
        Next
        Dim DtKeys(3) As String

        DtKeys(0) = "PIVA"
        DtKeys(1) = "Sa_Cod"
        DtKeys(2) = "Long"
        DtKeys(3) = "Lat"

        GridViewCentriStazioni.DataKeyNames = DtKeys

        GridViewCentriStazioni.DataSource = Dt
        GridViewCentriStazioni.DataBind()

        ColoroLaGrigliaCentriPerMeteo()

        'Dim DT_Stazioni As DataTable = Nothing

        'caricocombo
        For i = 0 To GridViewCentriStazioni.Rows.Count - 1

            Dim longcentro As Decimal = DTCentri.Rows(i).Item("Long")
            Dim latcentro As Decimal = DTCentri.Rows(i).Item("Lat")
            CType(GridViewCentriStazioni.Rows(i).Cells(3).Controls(1), TextBox).Text = CStr(longcentro)
            CType(GridViewCentriStazioni.Rows(i).Cells(4).Controls(1), TextBox).Text = CStr(latcentro)

            '**********************************************************************
            ' GABRIELE 2023 10 06
            ' all'editing del campo non si scatenerebbe il ricarico della combo
            ' delle stazioni meteo per la verifica della prossimità
            '**********************************************************************
            CType(GridViewCentriStazioni.Rows(i).Cells(3).Controls(1), TextBox).Enabled = False
            CType(GridViewCentriStazioni.Rows(i).Cells(4).Controls(1), TextBox).Enabled = False

            Dim DT_Stazioni = GetStazioni(Piva, latcentro, longcentro)

            RiempiComboConStazioni(CType(GridViewCentriStazioni.Rows(i).Cells(7).Controls(1), DropDownList), DT_Stazioni, Piva, DTCentri.Rows(i).Item("Sa_Cod"))

        Next

        'ViewState("RilievoPiogge_tempStazioni") = DT_Stazioni

    End Sub


    Private Function InizializzaGrigliaCentriPerMeteo() As DataTable
        Dim Dt As New DataTable("Piogge")

        Dt.Columns.Add("PIVA", GetType(String))
        Dt.Columns.Add("Rag_Soc", GetType(String))
        Dt.Columns.Add("Sa_Cod", GetType(Integer))
        Dt.Columns.Add("Sa_nome", GetType(String))
        Dt.Columns.Add("X", GetType(Integer))
        Dt.Columns.Add("Y", GetType(Integer))

        Return Dt
    End Function


    Private Sub finalizzaGrigliaCentriPerMeteo(Dt As DataTable)

        Dim DtKeys(3) As String

        DtKeys(0) = "PIVA"
        DtKeys(1) = "Sa_Cod"
        DtKeys(2) = "X"
        DtKeys(3) = "Y"

        GridViewCentriMeteo.DataKeyNames = DtKeys

        GridViewCentriMeteo.DataSource = Dt
        GridViewCentriMeteo.DataBind()

        ColoroLaGrigliaCentriPerMeteo()

    End Sub


    Private Sub ColoroLaGrigliaCentriPerMeteo()

        If Not IsNothing(GridViewCentriMeteo) AndAlso GridViewCentriMeteo.Rows.Count > 0 Then

            GridViewCentriMeteo.Columns(5).ControlStyle.BackColor = Drawing.Color.Pink
            GridViewCentriMeteo.Columns(3).ControlStyle.BackColor = Drawing.Color.FromArgb(198, 233, 156)
            GridViewCentriMeteo.Columns(4).ControlStyle.BackColor = Drawing.Color.FromArgb(198, 233, 156)

            'coloro le righe
            Dim i2 As Integer = 0
            For i2 = 0 To Me.GridViewCentriMeteo.Rows.Count - 1
                If i2 Mod 2 = 0 Then
                    Me.GridViewCentriMeteo.Rows(i2).BackColor = Drawing.Color.Beige
                End If
            Next

        End If

    End Sub


#End Region




#Region "Operazioni Sequenza Carica Rilievi"


    Private Sub ImageButton_MostraRilievi_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImageButton_MostraRilievi.Click
        Inserisci_Rilievi()
    End Sub


    Private Sub Inserisci_Rilievi()

        Dim msgerr As String = ""




        If Me.RBL_TipoRilievo.SelectedValue = "0" Then
            'dai meteo

            '---------------------------------------
            ' recupero i centri selezionati
            Dim ListaCentri As List(Of AgronicaCoreModello.Anagrafe.Centro_Aziendale)
            If Not RecuperaCentriSelezionatiMeteo(ListaCentri, msgerr) Then
                Messaggi.AgroMsgBox(msgerr, Page, , Master_Operazione.Property_UpdatePanelToolBar)
                Exit Sub
            End If

            If ListaCentri.Count = 0 Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.OccorreSelezionareAlmenoUnCentroAziendaleP, Page, , Master_Operazione.Property_UpdatePanelToolBar)
                Exit Sub
            End If

            If Not CreoTabellRilieviMeteoDaCentri(ListaCentri, msgerr) Then
                Messaggi.AgroMsgBox(msgerr, Page, , Master_Operazione.Property_UpdatePanelToolBar)
                Exit Sub
            End If

            'carico gliglia riepilogo piogge registrate
            'caricaGridViewRilieviSalvati(Me.GridViewCentriMeteo)

        ElseIf Me.RBL_TipoRilievo.SelectedValue = "1" Then
            'manuale

            If Not CreoTabellRilieviManualiDaCentri(msgerr) Then
                Messaggi.AgroMsgBox(msgerr, Page, , Master_Operazione.Property_UpdatePanelToolBar)
                Exit Sub
            End If

            'carico gliglia riepilogo piogge registrate
            'caricaGridViewRilieviSalvati(Me.GridViewCentriManuale)


        ElseIf Me.RBL_TipoRilievo.SelectedValue = "2" OrElse RBL_TipoRilievo.SelectedValue = "3" Then
            'dai stazioni

            '---------------------------------------
            ' recupero i centri selezionati
            Dim ListaCentri As List(Of AgronicaCoreModello.Anagrafe.Centro_Aziendale)
            If Not RecuperaCentriSelezionatiStazioniMetos(ListaCentri, msgerr) Then
                Messaggi.AgroMsgBox(msgerr, Page, , Master_Operazione.Property_UpdatePanelToolBar)
                Exit Sub
            End If

            If ListaCentri.Count = 0 Then
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.OccorreSelezionareAlmenoUnCentroAziendaleP, Page, , Master_Operazione.Property_UpdatePanelToolBar)
                Exit Sub
            End If

            If Not CreoTabellRilieviMeteoDaStazioni(ListaCentri, msgerr) Then
                Messaggi.AgroMsgBox(msgerr, Page, , Master_Operazione.Property_UpdatePanelToolBar)
                Exit Sub
            End If

        Else
            Throw New NotImplementedException
        End If

        AbilitaStep2(True)


    End Sub


    Private Sub AbilitaStep2(ByVal abilita As Boolean)



        If Me.RBL_TipoRilievo.SelectedValue = "0" Then
            'dai meteo

            LabelCaricaPioggiaDaMeteo.Visible = abilita
            LabelCaricaPioggiaManuale.Visible = False
            GridView_Piogge.Visible = abilita

            If abilita Then

            Else
                Me.GridView_Piogge.DataSource = Nothing
                Me.GridView_Piogge.DataBind()
            End If

        ElseIf Me.RBL_TipoRilievo.SelectedValue = "1" Then
            'manuale

            LabelCaricaPioggiaDaMeteo.Visible = False
            LabelCaricaPioggiaManuale.Visible = abilita

            GridView_Piogge.Visible = abilita

            If abilita Then

            Else
                Me.GridView_Piogge.DataSource = Nothing
                Me.GridView_Piogge.DataBind()
            End If

        ElseIf Me.RBL_TipoRilievo.SelectedValue = "2" OrElse RBL_TipoRilievo.SelectedValue = "3" Then
            'stazioni

            LabelCaricaPioggiaDaMeteo.Visible = abilita
            LabelCaricaPioggiaManuale.Visible = False
            GridView_Piogge.Visible = abilita

            If abilita Then

            Else
                Me.GridView_Piogge.DataSource = Nothing
                Me.GridView_Piogge.DataBind()
            End If
        Else
            Throw New NotImplementedException
        End If

        Me.ImageButton_MostraRilievi.Enabled = Not abilita
        Me.ImageButton_MostraRilievi.Visible = Not abilita
        Me.ImageButton_Sblocca.Enabled = abilita
        Me.ImageButton_Sblocca.Visible = abilita

        Me.NoteTabella.Visible = abilita

        Master_Operazione.Property_Box_Salva.Enabled = abilita 'true per modifica
        Master_Operazione.Property_Box_Salva.Visible = abilita 'true per modifica
        Master_Operazione.Property_ImgBtn_Salva.Enabled = abilita 'true per modifica
        'Master_Operazione.Property_RBL_Salva.Enabled = abilita 'true per modifica

        'Master_Operazione.Property_txt_DataOperazione.Enabled = Not abilita

        'Master_Operazione.Property_ImgBtn_Costi.Enabled = False

        Master_Operazione.Property_txt_Note.Enabled = abilita 'true per modifica

        Master_Operazione.Property_ComboSpecie.Enabled = Not abilita
        Master_Operazione.Property_BTN_ComboSpecie.Enabled = Not abilita

        'Master_Operazione.Property_BTN_CentroAziendale.Enabled = Not abilita
        'Master_Operazione.Property_ComboCentroAziendale.Enabled = Not abilita

        'Master_Operazione.Property_BTN_ComboOperazione.Enabled = Not abilita
        'Master_Operazione.Property_ComboOperazione.Enabled = Not abilita

        'Master_Operazione.Property_CBL_Consigli.Enabled = abilita
        'Master_Operazione.Property_GridView_Impianti.Enabled = Not abilita

        'BloccaComboJavaScript(abilita, abilita, abilita, abilita)
    End Sub


    Private Function CreoTabellRilieviManualiDaCentri(ByRef msgerr As String) As Boolean
        '----------------------------------------------------------------------
        '----------------GRIGLIA Rilievi --------------------------
        '----------------------------------------------------------------------
        '----------------------------------------------------------------------
        '----------------Creo la Datatable per la griglia ---------------------
        '----------------------------------------------------------------------
        Dim dt As DataTable = InizializzaDataTablePerGrigliePiogge()
        '----------------------------------------------------------------------
        '----------------Carico i dati.................................--------
        '----------------------------------------------------------------------
        Dim i As Integer = 0
        Dim row As DataRow

        Dim Piva As String = 0
        Dim Sacod As Integer = 0
        Dim num As String = "1"

        For i = 0 To Me.GridViewCentriManuale.Rows.Count - 1
            If CType(GridViewCentriManuale.Rows(i).FindControl("ChkSelezionaCentro"), CheckBox).Checked = True Then
                Piva = CStr(GridViewCentriManuale.DataKeys(i).Item("PIVA"))
                Sacod = CInt(GridViewCentriManuale.DataKeys(i).Item("Sa_Cod"))
                num = CStr(CType(GridViewCentriManuale.Rows(i).FindControl("Txt_NumRilievi"), TextBox).Text)
                Dim causa As String = ""
                If Not checkCoord(num, causa) Then
                    msgerr &= String.Format(Resources.AgronicaAgenda_2010.INumeriRilieviDelCentroX0NonSonoAmmissibil, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), num, causa)
                    Return False
                End If
                If CInt(num) > 30 Then
                    msgerr &= String.Format(Resources.AgronicaAgenda_2010.INumeriRilieviDelCentroX0NonAmmissibilMax, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), num)
                    Return False
                End If
                If CInt(num) = 0 Then
                    msgerr &= String.Format(Resources.AgronicaAgenda_2010.INumeriRilieviDelCentroX0NonAmmissibilAlmeno1, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), num)
                    Return False
                End If

                For j = 0 To CInt(num) - 1


                    row = dt.NewRow
                    row.Item("PIVA") = Piva
                    row.Item("Rag_Soc") = New AgronicaCoreAnagrafeDAL.Imprese_Read().RagSoc_from_Piva(Piva, objParametri_Server)
                    row.Item("Sa_Cod") = Sacod
                    row.Item("Sa_nome") = (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server)
                    dt.Rows.Add(row)


                Next



            End If

        Next


        '----------------------------------------------------------------------
        '----------------Finalizzo la griglia----------------------------------
        '----------------------------------------------------------------------
        finalizzaDataTablePiogge_Bind_GrigliePiogge(dt, GridView_Piogge)
        '----------------------------------------------------------------------
        '----------------------------------------------------------------------

        Return True
    End Function


    Private Function RecuperaCentriSelezionatiStazioniMetos(ByRef ListaCentri As List(Of AgronicaCoreModello.Anagrafe.Centro_Aziendale), ByRef msgerrror As String) As Boolean
        ListaCentri = New List(Of AgronicaCoreModello.Anagrafe.Centro_Aziendale)

        For i = 0 To Me.GridViewCentriStazioni.Rows.Count - 1

            'non spostare dal ciclo, devo essere reinizializzate
            Dim Centro As AgronicaCoreModello.Anagrafe.Centro_Aziendale
            Dim Piva As String = 0
            Dim Sacod As Integer = 0
            Dim nomeStazione As String = ""


            If CType(GridViewCentriStazioni.Rows(i).FindControl("ChkSelezionaCentro"), CheckBox).Checked = True Then
                Piva = CStr(GridViewCentriStazioni.DataKeys(i).Item("PIVA"))
                Sacod = CInt(GridViewCentriStazioni.DataKeys(i).Item("Sa_Cod"))

                nomeStazione = CStr(CType(GridViewCentriStazioni.Rows(i).FindControl("CMB_Stazione"), DropDownList).SelectedValue)


                Centro = New AgronicaCoreModello.Anagrafe.Centro_Aziendale(Piva, Sacod)
                Centro.NomeStazioneMeteoDiRiferimento = nomeStazione
                ListaCentri.Add(Centro)
            End If

        Next
        Return True
    End Function


    Private Function RecuperaCentriSelezionatiMeteo(ByRef ListaCentri As List(Of AgronicaCoreModello.Anagrafe.Centro_Aziendale), ByRef msgerrror As String) As Boolean
        ListaCentri = New List(Of AgronicaCoreModello.Anagrafe.Centro_Aziendale)

        For i = 0 To Me.GridViewCentriMeteo.Rows.Count - 1

            'non spostare dal ciclo, devo essere reinizializzate
            Dim Centro As AgronicaCoreModello.Anagrafe.Centro_Aziendale
            Dim Piva As String = 0
            Dim Sacod As Integer = 0
            Dim x As String = "0"
            Dim y As String = "0"
            Dim quadrante As String = "0"


            If CType(GridViewCentriMeteo.Rows(i).FindControl("ChkSelezionaCentro"), CheckBox).Checked = True Then
                Piva = CStr(GridViewCentriMeteo.DataKeys(i).Item("PIVA"))
                Sacod = CInt(GridViewCentriMeteo.DataKeys(i).Item("Sa_Cod"))
                If CType(GridViewCentriMeteo.Rows(i).FindControl("RBL_Coordinate"), RadioButtonList).SelectedValue = 0 Then
                    quadrante = "0"
                    x = CStr(CType(GridViewCentriMeteo.Rows(i).FindControl("Txt_CoordX"), TextBox).Text)
                    y = CStr(CType(GridViewCentriMeteo.Rows(i).FindControl("Txt_CoordY"), TextBox).Text)
                Else
                    x = "0"
                    y = "0"
                    quadrante = CStr(CType(GridViewCentriMeteo.Rows(i).FindControl("Txt_NumeroQuadrante"), TextBox).Text)
                End If



                If Not Verifica_Coordinate(x, y, quadrante, msgerrror) Then
                    msgerrror = String.Format(Resources.AgronicaAgenda_2010.CentroX0RigaX1Br, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), i) & msgerrror
                    Messaggi.AgroMsgBox(msgerrror, Page, , Master_Operazione.Property_UpdatePanelToolBar)
                    Return False
                End If

                Centro = New AgronicaCoreModello.Anagrafe.Centro_Aziendale(Piva, Sacod)
                Centro.X = CInt(x)
                Centro.Y = CInt(y)
                Centro.Qadrante = CInt(quadrante)
                ListaCentri.Add(Centro)
            End If

        Next
        Return True
    End Function


    Private Function CreoTabellRilieviMeteoDaCentri(ByRef ListaCentri As List(Of AgronicaCoreModello.Anagrafe.Centro_Aziendale), ByRef msgerr As String) As Boolean
        '----------------------------------------------------------------------
        '----------------GRIGLIA Rilievi --------------------------
        '----------------------------------------------------------------------
        '----------------------------------------------------------------------
        '----------------Creo la Datatable per la griglia ---------------------
        '----------------------------------------------------------------------
        Dim dt As DataTable = InizializzaDataTablePerGrigliePiogge()
        '----------------------------------------------------------------------
        '----------------Carico i dati.................................--------
        '----------------------------------------------------------------------
        Dim DT_Piogge As DataTable
        Dim row As DataRow
        Dim DatiCaricatiPerAlmenoUnCentro As Boolean = False
        Dim msgWarning As String = ""
        Dim quadranteReturn As Integer = 0

        Dim Totale As Decimal = 0

        For j = 0 To ListaCentri.Count - 1

            If Not Carica_Dati_Meteo(ListaCentri(j).X, ListaCentri(j).Y, ListaCentri(j).Qadrante, quadranteReturn, DT_Piogge, msgerr) Then
                msgerr = Resources.AgronicaAgenda_2010.CentroAz & (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(ListaCentri(j).Azienda.Piva, ListaCentri(j).Sa_Cod, objParametri_Server) & " : " & msgerr & "<br>"
                Return False
            Else

                If msgerr <> "" Then
                    'mostro un warning se msgerr <>"" perché vuol dire che carica dati meteo non ha ritornato false ma ha ritornato
                    'un messaggio, che indica probabilmente che per alcuni centri non sono stati trovati dati
                    msgWarning &= String.Format(Resources.AgronicaAgenda_2010.CentroX0Br, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(ListaCentri(j).Azienda.Piva, ListaCentri(j).Sa_Cod, objParametri_Server)) & msgerr & "<br>"
                    msgerr = ""
                End If


                settoNumeroQuadranteSeZero(ListaCentri(j), quadranteReturn)



                DatiCaricatiPerAlmenoUnCentro = True

                Dim CumuloPeriodoCentro As Decimal = 0

                For Each rPioggia In DT_Piogge.Rows

                    CumuloPeriodoCentro += CDec(rPioggia("mm_pioggia"))

                    row = dt.NewRow
                    row.Item("PIVA") = ListaCentri(j).Azienda.Piva
                    row.Item("Rag_Soc") = (New AgronicaCoreAnagrafeDAL.Imprese_Read().RagSoc_from_Piva(ListaCentri(j).Azienda.Piva, objParametri_Server))
                    row.Item("Sa_Cod") = ListaCentri(j).Sa_Cod
                    row.Item("Sa_nome") = (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(ListaCentri(j).Azienda.Piva, ListaCentri(j).Sa_Cod, objParametri_Server)

                    row.Item("Data") = rPioggia("Data")
                    row.Item("Ora") = rPioggia("Ora")
                    row.Item("Minuti") = 0
                    row.Item("mm_pioggia") = rPioggia("mm_pioggia")
                    row.Item("Note") = rPioggia("note")
                    row.Item("temp_minima") = rPioggia("temp_minima")
                    row.Item("temp_massima") = rPioggia("temp_massima")
                    row.Item("umidita") = rPioggia("umidita")
                    row.Item("irraggiamento") = rPioggia("irraggiamento")

                    row.Item("TotalePeriodoCentro") = CumuloPeriodoCentro


                    dt.Rows.Add(row)
                Next

                Totale += CumuloPeriodoCentro

            End If

        Next

        lblDataA3.Text = Totale

        If DatiCaricatiPerAlmenoUnCentro Then
            'mostro un warning se msgerr <>"" perché vuol dire che carica dati meteo non ha ritornato false ma ha ritornato
            'un messaggio, che indica probabilmente che per alcuni centri non sono stati trovati dati
            If msgWarning <> "" Then
                Messaggi.AgroMsgBox(msgWarning, Page, , Master_Operazione.Property_UpdatePanelToolBar)
            End If
        Else
            msgerr = Resources.AgronicaAgenda_2010.NonSonoPresentiDatiSullePioggePerNESSUNCEN
            Return False
        End If

        '----------------------------------------------------------------------
        '----------------Finalizzo la griglia----------------------------------
        '----------------------------------------------------------------------
        finalizzaDataTablePiogge_Bind_GrigliePiogge(dt, GridView_Piogge)
        '----------------------------------------------------------------------
        '----------------------------------------------------------------------
        '----------------------------------------------------------------------

        'carico text
        For j = 0 To GridView_Piogge.Rows.Count - 1
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_Data"), TextBox).Text = dt.Rows(j).Item("Data")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_Ora"), TextBox).Text = dt.Rows(j).Item("Ora")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_Note"), TextBox).Text = dt.Rows(j).Item("note")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_mm_pioggia"), TextBox).Text = dt.Rows(j).Item("mm_pioggia")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_temp_minima"), TextBox).Text = dt.Rows(j).Item("temp_minima")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_temp_massima"), TextBox).Text = dt.Rows(j).Item("temp_massima")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_umidita"), TextBox).Text = dt.Rows(j).Item("umidita")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_irraggiamento"), TextBox).Text = dt.Rows(j).Item("irraggiamento")
        Next

        '----------------------------------------------------------------------
        '----------------------------------------------------------------------
        '----------------------------------------------------------------------
        Return True
    End Function


    Private Function CreoTabellRilieviMeteoDaStazioni(ByRef ListaCentri As List(Of AgronicaCoreModello.Anagrafe.Centro_Aziendale), ByRef msgerr As String) As Boolean
        '----------------------------------------------------------------------
        '----------------GRIGLIA Rilievi --------------------------
        '----------------------------------------------------------------------
        '----------------------------------------------------------------------
        '----------------Creo la Datatable per la griglia ---------------------
        '----------------------------------------------------------------------
        Dim dt As DataTable = InizializzaDataTablePerGrigliePiogge()
        '----------------------------------------------------------------------
        '----------------Carico i dati.................................--------
        '----------------------------------------------------------------------
        Dim DT_Piogge As DataTable
        Dim row As DataRow
        Dim DatiCaricatiPerAlmenoUnCentro As Boolean = False
        Dim msgWarning As String = ""
        Dim quadranteReturn As Integer = 0

        Dim Totale As Decimal = 0

        For j = 0 To ListaCentri.Count - 1

            If Not Carica_Dati_Stazione(ListaCentri(j).NomeStazioneMeteoDiRiferimento, DT_Piogge, msgerr) Then
                msgerr = Resources.AgronicaAgenda_2010.CentroAz & (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(ListaCentri(j).Azienda.Piva, ListaCentri(j).Sa_Cod, objParametri_Server) & " : " & msgerr & "<br>"
                Return False
            Else

                If msgerr <> "" Then
                    'mostro un warning se msgerr <>"" perché vuol dire che carica dati meteo non ha ritornato false ma ha ritornato
                    'un messaggio, che indica probabilmente che per alcuni centri non sono stati trovati dati
                    msgWarning &= String.Format(Resources.AgronicaAgenda_2010.CentroX0Br, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(ListaCentri(j).Azienda.Piva, ListaCentri(j).Sa_Cod, objParametri_Server)) & msgerr & "<br>"
                    msgerr = ""
                End If


                settoNumeroQuadranteSeZero(ListaCentri(j), quadranteReturn)



                DatiCaricatiPerAlmenoUnCentro = True

                Dim CumuloPeriodoCentro As Decimal = 0

                For Each rPioggia In DT_Piogge.Rows

                    CumuloPeriodoCentro += CDec(rPioggia("mm_pioggia"))

                    row = dt.NewRow

                    row.Item("PIVA") = ListaCentri(j).Azienda.Piva
                    row.Item("Rag_Soc") = (New AgronicaCoreAnagrafeDAL.Imprese_Read().RagSoc_from_Piva(ListaCentri(j).Azienda.Piva, objParametri_Server))
                    row.Item("Sa_Cod") = ListaCentri(j).Sa_Cod
                    row.Item("Sa_nome") = (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(ListaCentri(j).Azienda.Piva, ListaCentri(j).Sa_Cod, objParametri_Server)

                    row.Item("Data") = rPioggia("Data")
                    row.Item("Ora") = 0
                    row.Item("Minuti") = 0
                    row.Item("mm_pioggia") = rPioggia("mm_pioggia")
                    row.Item("Note") = rPioggia("note")
                    row.Item("temp_minima") = rPioggia("temp_minima")
                    row.Item("temp_massima") = rPioggia("temp_massima")
                    row.Item("umidita") = rPioggia("umidita")
                    row.Item("irraggiamento") = rPioggia("irraggiamento")

                    row.Item("TotalePeriodoCentro") = CumuloPeriodoCentro


                    dt.Rows.Add(row)

                Next

                Totale += CumuloPeriodoCentro

            End If

        Next

        lblDataA3.Text = Totale

        If DatiCaricatiPerAlmenoUnCentro Then
            'mostro un warning se msgerr <>"" perché vuol dire che carica dati meteo non ha ritornato false ma ha ritornato
            'un messaggio, che indica probabilmente che per alcuni centri non sono stati trovati dati
            If msgWarning <> "" Then
                Messaggi.AgroMsgBox(msgWarning, Page, , Master_Operazione.Property_UpdatePanelToolBar)
            End If
        Else
            msgerr = Resources.AgronicaAgenda_2010.NonSonoPresentiDatiSullePioggePerNESSUNCEN
            Return False
        End If

        '----------------------------------------------------------------------
        '----------------Finalizzo la griglia----------------------------------
        '----------------------------------------------------------------------
        finalizzaDataTablePiogge_Bind_GrigliePiogge(dt, GridView_Piogge)
        '----------------------------------------------------------------------
        '----------------------------------------------------------------------
        '----------------------------------------------------------------------

        'carico text
        For j = 0 To GridView_Piogge.Rows.Count - 1
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_Data"), TextBox).Text = dt.Rows(j).Item("Data")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_Ora"), TextBox).Text = dt.Rows(j).Item("Ora")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_Note"), TextBox).Text = dt.Rows(j).Item("note")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_mm_pioggia"), TextBox).Text = dt.Rows(j).Item("mm_pioggia")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_temp_minima"), TextBox).Text = dt.Rows(j).Item("temp_minima")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_temp_massima"), TextBox).Text = dt.Rows(j).Item("temp_massima")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_umidita"), TextBox).Text = dt.Rows(j).Item("umidita")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_irraggiamento"), TextBox).Text = dt.Rows(j).Item("irraggiamento")
        Next

        '----------------------------------------------------------------------
        '----------------------------------------------------------------------
        '----------------------------------------------------------------------
        Return True
    End Function


    Private Function Carica_Dati_Meteo(ByVal Coordinata_X As Integer, ByVal Coordinata_Y As Integer, ByVal Numero_Quadrante As Integer, ByRef quadranteReturn As Integer, ByRef DT_Piogge As DataTable, ByRef messaggioErrore As String) As Boolean
        DT_Piogge = New DataTable

        'Recupera i filtri
        Dim Data_DA, Data_A As Date
        Dim Ora As String
        Dim SogliaMM As Decimal

        If Me.Text_BoxDataDa.Text <> "" AndAlso Me.Text_BoxDataA.Text <> "" Then
            Try
                Data_DA = CDate(Me.Text_BoxDataDa.Text)
                Data_A = CDate(Me.Text_BoxDataA.Text)
            Catch ex As Exception
                messaggioErrore = Resources.AgronicaAgenda_2010.InserireUnFormatoDiDataValidoPerLIntervall
                Return False
            End Try

        Else
            messaggioErrore = Resources.AgronicaAgenda_2010.ENecessarioSpecificareEntrambeLeDateDellIn1
            Return False
        End If

        Ora = Me.DropDownOre.SelectedValue

        'disabilito ore temporaneamente, presuppongo 23
        If Ora <> "23" Then
            messaggioErrore = "Ora <> 23."
            Return False
        End If

        If Me.TextBoxSoglia.Text <> "" AndAlso IsNumeric(Me.TextBoxSoglia.Text) Then

            'If InStr(Me.Txt_SogliaMM.Text, ".") <> 0 Then
            '    Me.Txt_SogliaMM.Text = Replace(Me.Txt_SogliaMM.Text, ".", ",")
            'End If


            SogliaMM = CDbl(Me.TextBoxSoglia.Text)


        Else
            messaggioErrore = Resources.AgronicaAgenda_2010.ENecessarioSpecificareLaSogliaDeiMillimetr
            Return False
        End If


        Dim Flag_Spazio As String

        If Coordinata_X <> 0 AndAlso Coordinata_Y <> 0 Then
            Flag_Spazio = "C"
        ElseIf Numero_Quadrante <> 0 Then
            Flag_Spazio = "Q"
        Else
            messaggioErrore = Resources.AgronicaAgenda_2010.LeCoordinateOIlQuadranteSceltoPerIlCaricam
            Return False
        End If


        'crea xml x ws
        Dim objMeteo As New AgronicaCoreWebService.Meteo
        Dim XML_Parametri As XmlElement
        Dim Str_XML_Parametri As String = ""
        Dim Doorkey As String = "Y4h8u3B5w2"
        Dim Security_Token As String = ""
        Dim Flag_Dettagli As String = "L" 'L=info minime; H=dettagli aggiuntivi
        Dim Flag_Aggrega As String = "G" ' G=giornaliero; H= orario; I=intervallo
        Dim Numero_Stazione As Integer = 0

        Try

            XML_Parametri = objMeteo.Crea_XMLParametri_DatiPrecipitazioni(Nothing,
                                                                        Doorkey,
                                                                        Session("ASG_Utente_Username_Crypt"),
                                                                        Session("ASG_Utente_Password_Crypt"),
                                                                        Security_Token,
                                                                        Flag_Dettagli,
                                                                        Flag_Aggrega,
                                                                        Flag_Spazio,
                                                                        SogliaMM,
                                                                        Ora,
                                                                        Numero_Quadrante,
                                                                        Numero_Stazione,
                                                                        Coordinata_X,
                                                                        Coordinata_Y,
                                                                        Data_DA,
                                                                        Data_A)

            Str_XML_Parametri = XML_Parametri.OuterXml

            Str_XML_Parametri = objMeteo.Cripta_XML_Parametri(Str_XML_Parametri)


        Catch ex As Exception
            messaggioErrore = Resources.AgronicaAgenda_2010.SiÈVerificatoIlSeguenteErroreDuranteLaPrep &
                              vbCrLf & ex.Message
            Return False
        End Try

        If Str_XML_Parametri = "" Then
            messaggioErrore = Resources.AgronicaAgenda_2010.XmlParametriVuotaImpossibileChiamareIlWebS
            Return False
        Else

            Dim Str_XML_Risultato As String
            Try
                Str_XML_Risultato = objMeteo.Chiama_WS_Meteo_DatiPrecipitazioni(Str_XML_Parametri)
            Catch ex As Exception
                messaggioErrore = Resources.AgronicaAgenda_2010.NonÈStatoRecuperatoAlcunRisultatoDalWebSer & ex.Message
                Return False
            End Try


            If Str_XML_Risultato = "" Then
                messaggioErrore = Resources.AgronicaAgenda_2010.NonÈStatoRecuperatoAlcunRisultatoDalWebSer
                Return False
            Else
                Dim Num_Quadrante As Integer
                Dim Numero_Nodi_Dato As Integer
                Dim Codice_Errore As Integer
                Dim Messaggio_Errore As String
                Dim XMLs_NodiDato As XmlNodeList

                Try

                    'Elabora la risposta del WS
                    objMeteo.Leggi_XML_Risultato(Str_XML_Risultato,
                                                    Num_Quadrante,
                                                    Numero_Nodi_Dato,
                                                    Codice_Errore,
                                                    Messaggio_Errore,
                                                    XMLs_NodiDato)

                    quadranteReturn = Num_Quadrante

                Catch ex As Exception
                    messaggioErrore = Resources.AgronicaAgenda_2010.SiÈVerificatoIlSeguenteErroreDuranteLElabo &
                    vbCrLf & ex.Message
                    Return False
                End Try

                If Codice_Errore <> 0 Then
                    messaggioErrore = Resources.AgronicaAgenda_2010.SiÈVerificatoIlSeguenteErroreDuranteIlRecu & vbCrLf & Messaggio_Errore
                    Return False
                Else


                    'Leggi i nodi dato e Carica il datagrid
                    If Not IsNothing(XMLs_NodiDato) AndAlso XMLs_NodiDato.Count > 0 Then

                        Try

                            Dim i As Integer
                            Dim Data_Rilievo As Date
                            Dim mm_pioggia As Decimal
                            Dim XML_Dato As XmlElement

                            'carico la struttura del datatable

                            Dim Dr_Piogge As DataRow



                            '----- Definisco la struttura del DataTable
                            DT_Piogge.Columns.Add(New DataColumn("Data", GetType(String)))
                            DT_Piogge.Columns.Add(New DataColumn("Ora", GetType(String)))
                            DT_Piogge.Columns.Add(New DataColumn("mm_pioggia", GetType(Decimal)))
                            DT_Piogge.Columns.Add(New DataColumn("Note", GetType(String)))
                            DT_Piogge.Columns.Add(New DataColumn("Temp_minima", GetType(Decimal)))
                            DT_Piogge.Columns.Add(New DataColumn("Temp_massima", GetType(Decimal)))
                            DT_Piogge.Columns.Add(New DataColumn("umidita", GetType(Decimal)))
                            DT_Piogge.Columns.Add(New DataColumn("irraggiamento", GetType(Decimal)))

                            '-------------------------------------------------------------------

                            For i = 0 To XMLs_NodiDato.Count - 1

                                XML_Dato = XMLs_NodiDato.Item(i)

                                Data_Rilievo = XML_Dato.GetAttribute("gg")
                                mm_pioggia = XML_Dato.GetAttribute("mm")

                                Dr_Piogge = DT_Piogge.NewRow


                                Dr_Piogge.Item("Data") = Data_Rilievo.ToShortDateString
                                Dr_Piogge.Item("Ora") = Me.DropDownOre.SelectedValue
                                Dr_Piogge.Item("mm_pioggia") = mm_pioggia
                                Dr_Piogge.Item("note") = ""
                                Dr_Piogge.Item("temp_minima") = 0
                                Dr_Piogge.Item("temp_massima") = 0
                                Dr_Piogge.Item("umidita") = 0
                                Dr_Piogge.Item("irraggiamento") = 0

                                'Associo alla tabella la nuova riga creata
                                DT_Piogge.Rows.Add(Dr_Piogge)

                            Next



                        Catch ex As Exception
                            messaggioErrore = Resources.AgronicaAgenda_2010.SiÈVerificatoIlSeguenteErroreDuranteIlCari & vbCrLf & ex.Message
                            Return False
                        End Try

                    Else
                        messaggioErrore = Resources.AgronicaAgenda_2010.NonSonoPresentiDatiSullePioggePerLInterval
                    End If

                End If

            End If 'Str_XML_Risultato

        End If 'Str_XML_Parametri

        Return True

    End Function



    Private Function Carica_Dati_Stazione(ByVal nomeStazione As String, ByRef DT_Piogge As DataTable, ByRef messaggioErrore As String) As Boolean
        DT_Piogge = New DataTable

        'Recupera i filtri
        Dim Data_DA, Data_A As Date
        Dim Ora As String
        Dim SogliaMM As Decimal

        If Me.Text_BoxDataDa.Text <> "" AndAlso Me.Text_BoxDataA.Text <> "" Then
            Try
                Data_DA = CDate(Me.Text_BoxDataDa.Text)
                Data_A = CDate(Me.Text_BoxDataA.Text)
                stazioni_verifica_seDataUltAggPres(Data_A, nomeStazione)
            Catch ex As Exception
                messaggioErrore = Resources.AgronicaAgenda_2010.InserireUnFormatoDiDataValidoPerLIntervall
                Return False
            End Try

        Else
            messaggioErrore = Resources.AgronicaAgenda_2010.ENecessarioSpecificareEntrambeLeDateDellIn1
            Return False
        End If

        Ora = Me.DropDownOre.SelectedValue

        'disabilito ore temporaneamente, presuppongo 23
        If Ora <> "23" Then
            messaggioErrore = "Ora <> 23."
            Return False
        End If

        If Me.TextBoxSoglia.Text <> "" AndAlso IsNumeric(Me.TextBoxSoglia.Text) Then

            'If InStr(Me.Txt_SogliaMM.Text, ".") <> 0 Then
            '    Me.Txt_SogliaMM.Text = Replace(Me.Txt_SogliaMM.Text, ".", ",")
            'End If


            SogliaMM = CDbl(Me.TextBoxSoglia.Text)


        Else
            messaggioErrore = Resources.AgronicaAgenda_2010.ENecessarioSpecificareLaSogliaDeiMillimetr
            Return False
        End If

        Dim objParams As New JObject
        objParams("TipoSorgente") = GetTipoSorgenteMeteo()
        objParams("Sorgente") = nomeStazione
        objParams("DataInizio") = Data_DA
        objParams("DataFine") = Data_A

        'Dim objMeteoNT As New AgronicaCoreWebService.MeteoNT
        'Dim ss As String = objMeteoNT.DatiMeteoElaboraPiogge(objParams, objParametri_Server)
        Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)
        Dim ss As String = objMeteoSuite.DatiMeteoElaboraPiogge(objParams)
        Dim obj = JsonConvert.DeserializeObject(ss)

        If Not obj("RispostaOK") Then
            messaggioErrore = obj("Errore")
            Return False
        End If

        '----- Definisco la struttura del DataTable
        DT_Piogge.Columns.Add(New DataColumn("Data", GetType(String)))
        DT_Piogge.Columns.Add(New DataColumn("Ora", GetType(String)))
        DT_Piogge.Columns.Add(New DataColumn("mm_pioggia", GetType(Decimal)))
        DT_Piogge.Columns.Add(New DataColumn("Note", GetType(String)))
        DT_Piogge.Columns.Add(New DataColumn("Temp_minima", GetType(Decimal)))
        DT_Piogge.Columns.Add(New DataColumn("Temp_massima", GetType(Decimal)))
        DT_Piogge.Columns.Add(New DataColumn("umidita", GetType(Decimal)))
        DT_Piogge.Columns.Add(New DataColumn("irraggiamento", GetType(Decimal)))

        '-------------------------------------------------------------------

        Dim datiGG = JArray.Parse(obj("RispostaStringa"))

        Dim Dr_Piogge As DataRow

        Dim DataOra As DateTime
        Dim Prec As Decimal

        For Each dgg In datiGG

            Prec = CDec(dgg("Prec"))

            If Prec >= SogliaMM Then

                Dr_Piogge = DT_Piogge.NewRow

                DataOra = CDate(dgg("DataOra"))

                Dr_Piogge.Item("Data") = DataOra.ToShortDateString
                Dr_Piogge.Item("Ora") = Me.DropDownOre.SelectedValue
                Dr_Piogge.Item("mm_pioggia") = Prec
                Dr_Piogge.Item("note") = ""
                If dgg("TempMin") IsNot Nothing Then
                    Dr_Piogge.Item("temp_minima") = CDec(dgg("TempMin"))
                Else
                    Dr_Piogge.Item("temp_minima") = 0
                End If
                If dgg("TempMax") IsNot Nothing Then
                    Dr_Piogge.Item("temp_massima") = CDec(dgg("TempMax"))
                Else
                    Dr_Piogge.Item("temp_massima") = 0
                End If
                If dgg("UmRel") IsNot Nothing Then
                    Dr_Piogge.Item("umidita") = CDec(dgg("UmRel"))
                Else
                    Dr_Piogge.Item("umidita") = 0
                End If

                Dr_Piogge.Item("irraggiamento") = 0

                'Associo alla tabella la nuova riga creata
                DT_Piogge.Rows.Add(Dr_Piogge)

            End If
        Next

        Return True

    End Function



    Private Function InizializzaDataTablePerGrigliePiogge() As DataTable
        Dim Dt As New DataTable("Piogge")

        Dt.Columns.Add("PIVA", GetType(String))
        Dt.Columns.Add("Rag_Soc", GetType(String))
        Dt.Columns.Add("Sa_Cod", GetType(Integer))
        Dt.Columns.Add("Sa_nome", GetType(String))
        Dt.Columns.Add("ID_Agenda", GetType(Integer))
        Dt.Columns.Add("Data", GetType(Date))
        Dt.Columns.Add("Ora", GetType(Integer))
        Dt.Columns.Add("Minuti", GetType(Integer))
        Dt.Columns.Add("mm_pioggia", GetType(String))
        Dt.Columns.Add("Note", GetType(String))
        Dt.Columns.Add("temp_minima", GetType(Integer))
        Dt.Columns.Add("temp_massima", GetType(Integer))
        Dt.Columns.Add("umidita", GetType(Integer))
        Dt.Columns.Add("irraggiamento", GetType(Integer))
        Dt.Columns.Add("TotalePeriodoCentro", GetType(String))



        Return Dt
    End Function


    Private Sub finalizzaDataTablePiogge_Bind_GrigliePiogge(Dt As DataTable, GW As GridView)

        ''Vettore di DataColumn
        Dim DtKeys(3) As String

        'Valorizzo le celle del vettore
        DtKeys(0) = "PIVA"
        DtKeys(1) = "Sa_Cod"
        DtKeys(2) = "Data"
        DtKeys(3) = "ID_Agenda"

        GW.DataKeyNames = DtKeys

        GW.DataSource = Dt
        GW.DataBind()

        ColoroLaGriglia(GW)

    End Sub


    Private Sub ColoroLaGriglia(GW As GridView)

        If Not IsNothing(GW) AndAlso GW.Rows.Count > 0 Then



            'coloro le righe
            Dim i2 As Integer = 0
            For i2 = 0 To GW.Rows.Count - 1
                If i2 Mod 2 = 0 Then
                    GW.Rows(i2).BackColor = Drawing.Color.Beige
                End If

                'se  esiste l'operaznione di rilievo piogge fatta lo stesso giorno coloro la riga e deseleziono
                'solo se leggo i dati meteo quindi se date è valorizzato posso colorare la riga, non se inserisco manualmente
                If Not IsDBNull(GW.DataKeys(i2).Item("Data")) AndAlso
                    Not IsNothing(GW.DataKeys(i2).Item("Data")) AndAlso
                    IsDate(CStr(GW.DataKeys(i2).Item("Data"))) AndAlso
                    objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
                    Dim filtro As String = " Validita_Inizio =  " & Agro_SQL_SaveDate(CDate(CStr(GW.DataKeys(i2).Item("Data")))) & "  "
                    Dim dt As DataTable = New AgronicaCoreContabDAL.Agenda_R().Leggi(CStr(GW.DataKeys(i2).Item("PIVA")), CInt(GW.DataKeys(i2).Item("Sa_Cod")), 0, objParametriAgenda.Lav_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, filtro, "", objParametri_Server)
                    If dt.Rows.Count > 0 Then
                        GW.Rows(i2).BackColor = Drawing.Color.Pink
                    End If
                End If

            Next
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

        script.AppendLine("} ")



        ScriptManager.RegisterStartupScript(Master_Operazione.Property_UpdatePanelPerScript,
                                Master_Operazione.Property_UpdatePanelPerScript.GetType(),
                                "jQuery_{0}", script.ToString, True)


    End Sub


    Private Sub ImageButton_Sblocca_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImageButton_Sblocca.Click
        Rimuovi_Rilievi()
    End Sub


    Private Sub Rimuovi_Rilievi()
        AbilitaStep2(False)
    End Sub


#End Region




#Region "Eventi Gestiti Dalla Master"


    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)
        Rimuovi_Rilievi()
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


        Session("RilievoPiogge_WS_SuperUser_Username_Crypt") = Nothing
        Session("RilievoPiogge_WS__SuperUser_Password_Crypt") = Nothing
        Session("RilievoPiogge_WS__Qs_Piva_Crypt") = Nothing
        Session("RilievoPiogge_WS__Piva") = Nothing
        Session("RilievoPiogge_WS__Sa_Cod") = Nothing
        Session("RilievoPiogge_WS__IndiceRigaGriglia") = Nothing
        Session("RilievoPiogge_WS__UrlRitaglioServizio") = Nothing

        Response.Redirect(link)
    End Sub


    Private Sub CambioSpecie()

    End Sub


    Private Sub MasterUnload()

        'objParametriAgenda.Leggi()

    End Sub


#End Region




#Region "Salvataggio"


    Private Sub SalvaTutto(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim messaggio_errore As String = ""
        If SalvaOperazioneAgenda(messaggio_errore) Then
            'premutosalva = True
            gestisciTipoSalvataggio()
        Else
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione & messaggio_errore, Page, ,
                  Master_Operazione.Property_UpdatePanelToolBar)
        End If
    End Sub


    Private Function SalvaOperazioneAgenda(ByRef messaggio_errore As String) As Boolean
        Dim res As Boolean = False


        '---------------------------------------
        ' recupero tabella delle Piogge
        If Me.GridView_Piogge.Rows.Count = 0 Then
            messaggio_errore = messaggio_errore & Resources.AgronicaAgenda_2010.LaTabellaDellePioggeÈVuota
            Return False
        End If

        Try


            '-----------------------------------------------------
            '----------- CONNESSIONE E TRANSAZIONE ---------------
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            '-----------------------------------------------------

            Dim Piva As String = 0
            Dim Sacod As Integer = 0
            Dim Data As String = "0"
            Dim Ora As String = "0"
            Dim Minuti As String = "0"
            Dim Pioggia As String = "0"
            Dim Note As String = "0"
            Dim TMin As String = "0"
            Dim TMax As String = "0"
            Dim Umidita As String = "0"
            Dim Irraggiamento As String = "0"
            Dim RigheSelezionate As Integer = 0
            For i = 0 To GridView_Piogge.Rows.Count - 1


                If CType(GridView_Piogge.Rows(i).FindControl("ChkSelezionaRilievo"), CheckBox).Checked = True Then


                    Piva = CStr(GridView_Piogge.DataKeys(i).Item("PIVA"))
                    Sacod = CInt(GridView_Piogge.DataKeys(i).Item("Sa_Cod"))

                    Data = (CType(GridView_Piogge.Rows(i).FindControl("Txt_Data"), TextBox).Text)
                    Ora = (CType(GridView_Piogge.Rows(i).FindControl("Txt_Ora"), TextBox).Text)
                    Minuti = (CType(GridView_Piogge.Rows(i).FindControl("Txt_Minuti"), TextBox).Text)
                    Pioggia = (CType(GridView_Piogge.Rows(i).FindControl("Txt_mm_pioggia"), TextBox).Text)
                    Note = (CType(GridView_Piogge.Rows(i).FindControl("Txt_Note"), TextBox).Text)
                    TMin = (CType(GridView_Piogge.Rows(i).FindControl("Txt_temp_minima"), TextBox).Text)
                    TMax = (CType(GridView_Piogge.Rows(i).FindControl("Txt_temp_massima"), TextBox).Text)
                    Umidita = (CType(GridView_Piogge.Rows(i).FindControl("Txt_umidita"), TextBox).Text)
                    Irraggiamento = (CType(GridView_Piogge.Rows(i).FindControl("Txt_irraggiamento"), TextBox).Text)
                    Pioggia = Pioggia.Replace(".", ",")



                    If Not IsDate(Data) Then
                        messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.LaDataPerIlCentroX0NellaRigaX1NonÈValida, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), (i + 1))
                        Throw New Exception(messaggio_errore)
                    End If

                    If Not IsNumeric(Ora) Then
                        messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.LOraDelCentroX0NellaRigaX1NonÈValida, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), (i + 1))
                        Throw New Exception(messaggio_errore)
                    End If

                    If CInt(Ora) < 0 OrElse CInt(Ora) >= 24 Then
                        messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.LOraDelCentroX0NellaRigaX1NonÈValida, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), (i + 1))
                        Throw New Exception(messaggio_errore)
                    End If

                    If CInt(Minuti) < 0 OrElse CInt(Minuti) >= 60 Then
                        messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.LOraDelCentroX0NellaRigaX1NonÈValida, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), (i + 1))
                        Throw New Exception(messaggio_errore)
                    End If

                    If Not IsNumeric(Pioggia) Then
                        messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.LaQuantitàDiPioggiaDelCentroX0NellaRigaX1N, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), (i + 1))
                        Throw New Exception(messaggio_errore)
                    End If

                    If CInt(Pioggia) < 0 OrElse CInt(Pioggia) > 1000 Then
                        messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.LaQuantitàDiPioggiaDelCentroX0NellaRigaX1N, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), (i + 1))
                        Throw New Exception(messaggio_errore)
                    End If

                    If Not IsNumeric(TMin) Then
                        messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.LaTemperaturaDelCentroX0NellaRigaX1NonÈVal, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), (i + 1))
                        Throw New Exception(messaggio_errore)
                    End If

                    If CInt(TMin) < -30 OrElse CInt(TMin) > 80 Then
                        messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.LaTemperaturaDelCentroX0NellaRigaX1NonÈVal, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), (i + 1))
                        Throw New Exception(messaggio_errore)
                    End If

                    If Not IsNumeric(TMax) Then
                        messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.LaTemperaturaDelCentroX0NellaRigaX1NonÈVal, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), (i + 1))
                        Throw New Exception(messaggio_errore)
                    End If

                    If CInt(TMax) < -30 OrElse CInt(TMax) > 80 Then
                        messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.LaTemperaturaDelCentroX0NellaRigaX1NonÈVal, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), (i + 1))
                        Return False
                    End If

                    If Not IsNumeric(Umidita) Then
                        messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.LaQuantitàDiUmiditàDelCentroX0NellaRigaX1N, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), (i + 1))
                        Throw New Exception(messaggio_errore)
                    End If

                    If CInt(Umidita) < 0 OrElse CInt(Umidita) > 100 Then
                        messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.LaQuantitàDiUmiditàDelCentroX0NellaRigaX1N, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), (i + 1))
                        Throw New Exception(messaggio_errore)
                    End If

                    If Not IsNumeric(Irraggiamento) Then
                        messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.LIrraggiamentoDelCentroX0NellaRigaX1NonÈVal, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), (i + 1))
                        Throw New Exception(messaggio_errore)
                    End If

                    'If CInt(Irraggiamento) < 0 Or CInt(Irraggiamento) > 100 Then
                    '    messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.LIrraggiamentoDelCentroX0NellaRigaX1NonÈVal, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server), (i + 1))
                    '    Throw New Exception(messaggio_errore)
                    'End If

                    '-------------------------------------------------------------------------------
                    '-------Salvataggio di una operazione agenda per riga--------------------
                    '-------------------------------------------------------------------------------
                    Dim Agenda As Operazione_Agenda = CreaOggettoAgenda(Piva, Sacod, Data, Ora, Minuti, Pioggia, Note, TMin, TMax, Umidita, Irraggiamento, messaggio_errore)
                    If IsNothing(Agenda) Then
                        Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
                    End If


                    'se  esiste l'operaznione di rilievo piogge fatta lo stesso giorno fermo l'operazione e visualizzo errore
                    If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura Then
                        Dim filtro As String = " Validita_Inizio =  " & Agro_SQL_SaveDate(CDate(Data)) & "  "
                        Dim dt As DataTable = New AgronicaCoreContabDAL.Agenda_R().Leggi(Piva, Sacod, 0, Agenda.Lav_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, filtro, "", objParametri_Server)
                        If dt.Rows.Count > 0 Then

                            messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.EsistonoGiàX0RilieviPioggeInDataX1PerIlCen, dt.Rows.Count, Data, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server))
                            messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.DeselezionareLaRigaX0OEliminareIlPrecedent, (i + 1))
                            Throw New Exception(messaggio_errore)
                        End If
                    End If

                    'In modifica escludo l'id agenda corrente che naturalmente avrà la stessa data
                    If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
                        Dim filtro As String = " Validita_Inizio =  " & Agro_SQL_SaveDate(CDate(Data)) & " AND not Agenda.Id_Agenda = " & Agro_SQL_SaveNum(objParametriAgenda.Id_Agenda) & "  "
                        Dim dt As DataTable = New AgronicaCoreContabDAL.Agenda_R().Leggi(Piva, Sacod, 0, Agenda.Lav_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, filtro, "", objParametri_Server)
                        If dt.Rows.Count > 0 Then


                            messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.EsistonoGiàX0RilieviPioggeInDataX1PerIlCen, dt.Rows.Count, Data, (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server))
                            messaggio_errore &= String.Format(Resources.AgronicaAgenda_2010.DeselezionareLaRigaX0OEliminareIlPrecedent, (i + 1))
                            Throw New Exception(messaggio_errore)

                        End If
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


                    RigheSelezionate += 1



                    'objParametriAgenda.Id_Agenda = Id_Agenda

                End If

            Next


            ''chiudi connessione e commit transazione
            'ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            'ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

            If RigheSelezionate = 0 Then
                messaggio_errore &= Resources.AgronicaAgenda_2010.NonÈStatoSelezionatoNessunRilievo
                Throw New Exception(messaggio_errore)
            End If







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

                Rimuovi_Rilievi()



            Case enum_Tipo_Salvataggio.Salva_e_Nuovo
                Trappole_Disposed()

                objParametriAgenda.Impianti = New List(Of AgronicaCoreModello.ParametriAgenda_Temp.Impianto)
                objParametriAgenda.Note = New List(Of Nota)
                objParametriAgenda.Movimenti = New List(Of Movimento)

                Rimuovi_Centri()



                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                  Master_Operazione.Property_UpdatePanelToolBar)

                Dim strJS As New StringBuilder
                strJS.AppendLine("$(document).ready(function () { ")
                'strJS.AppendLine("      alert(''); ")
                strJS.AppendLine("      window.location = '../Operazioni/RilievoPiogge.aspx'; ")
                strJS.AppendLine(" });")

                ScriptManager.RegisterClientScriptBlock(Master_Operazione.Property_UpdatePanelToolBar,
                                                    Master_Operazione.Property_UpdatePanelToolBar.GetType(),
                                              String.Format("jQuery_{0}", Master_Operazione.Property_UpdatePanelToolBar.ClientID),
                                              strJS.ToString, True)






            Case enum_Tipo_Salvataggio.Salva_e_Duplica



                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.RegistrazioneEffettuataConSuccesso, Page, ,
                                  Master_Operazione.Property_UpdatePanelToolBar)


                Rimuovi_Rilievi()


        End Select
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




#Region "Creazione oggetto Agenda per salvataggio"


    Private Function CreaOggettoAgenda(Piva As String, Sacod As Integer, Data As String, Ora As String, Minuti As String, Pioggia As String, Note As String, TMin As String, TMax As String, Umidita As String, Irraggiamento As String, messaggio_errore As String) As Operazione_Agenda
        Dim Agenda As Operazione_Agenda

        '--------------AGENDA------------------
        If Not Crea_Agenda(Data, Piva, Sacod, Agenda, messaggio_errore) Then
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

            Case LAVCOD_RILIEVO_PIOGGE
                If Not Crea_Agenda_Movimento_Piogge(Agenda, Piva, Sacod, Data, Ora, Minuti, Pioggia, Note, TMin, TMax, Umidita, Irraggiamento, messaggio_errore) Then
                    Return Nothing
                End If

            Case Else
                Throw New NotImplementedException
        End Select

        Return Agenda

    End Function


    Private Function Crea_Agenda(ByRef Data As String, ByRef PIVA As String, ByRef Sa_Cod As Integer, ByRef Agenda As Operazione_Agenda, ByRef messaggio_errore As String) As Boolean

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
        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = objParametriAgenda.Tipo_Operazione
        Select Case Agenda.Tipo_Operazione
            Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                Agenda.Id_Agenda = 0
            Case Else
                Agenda.Id_Agenda = objParametriAgenda.Id_Agenda
        End Select
        'Agenda.Id_Agenda = 0

        Agenda.Data = CDate(Data)
        Agenda.Piva = objParametriAgenda.Piva
        Agenda.Sa_Cod = Sa_Cod
        Agenda.Lav_Cod = objParametriAgenda.Lav_Cod
        Agenda.Des_Lib = Lav_Des '& " (Centro: " & New AgronicaCoreAnagrafeDAL.CentriAziendali_Read().SaNome_from_SaCod(PIVA, Sa_Cod, objParametri_Server) & ")"

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
                Select Case Agenda.Tipo_Operazione
                    Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                        Nota.Id_Agenda = 0
                    Case Else
                        Nota.Id_Agenda = objParametriAgenda.Id_Agenda
                End Select
                Nota.Nota_Cod = ListaConsigli(i).Nota_Cod
                Agenda.Note.Add(Nota)
            Next
        End If
        Return True
    End Function

    Private Function Crea_Agenda_Movimento_CostiAccessori(ByRef Agenda As Operazione_Agenda) As Boolean
        For i = 0 To objParametriAgenda.Movimenti.Count - 1
            Select Case Agenda.Tipo_Operazione
                Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                    objParametriAgenda.Movimenti(i).Id_Agenda = 0
                Case Else
                    objParametriAgenda.Movimenti(i).Id_Agenda = objParametriAgenda.Id_Agenda
            End Select

            objParametriAgenda.Movimenti(i).Sa_Cod = Agenda.Sa_Cod
            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli) Then
                Dim j As Integer
                For j = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli.Count - 1
                    Select Case Agenda.Tipo_Operazione
                        Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                            objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda = 0
                        Case Else
                            objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Id_Agenda = objParametriAgenda.Id_Agenda
                    End Select

                    If objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = 0 Then
                        objParametriAgenda.Movimenti(i).Movimenti_Dettagli(j).Sa_Cod = Agenda.Sa_Cod
                    End If
                Next
            End If

            If Not IsNothing(objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then
                Dim j As Integer
                For j = 0 To objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1
                    Select Case Agenda.Tipo_Operazione
                        Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                            objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Id_Agenda = 0
                        Case Else
                            objParametriAgenda.Movimenti(i).Movimenti_Dettagli_Tecnici(j).Id_Agenda = objParametriAgenda.Id_Agenda
                    End Select
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


    Private Function Crea_Agenda_Movimento_Piogge(ByRef Agenda As Operazione_Agenda, Piva As String, Sacod As Integer, Data As String, Ora As String, Minuti As String, Pioggia As String, Note As String, TMin As String, TMax As String, Umidita As String, Irraggiamento As String, messaggio_errore As String) As Boolean

        Select Case CInt(objParametriAgenda.Lav_Cod)

            Case LAVCOD_RILIEVO_PIOGGE
                'continua, questo metodo deve essere chioamato solo in questi casi altrimenti genero accezione
            Case Else
                Throw New Exception
        End Select

        Dim Movimento_OperazioneColturale As New Movimento

        Select Case Agenda.Tipo_Operazione
            Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                Movimento_OperazioneColturale.Id_Agenda = 0
            Case Else
                Movimento_OperazioneColturale.Id_Agenda = objParametriAgenda.Id_Agenda
        End Select
        Movimento_OperazioneColturale.Piva = Agenda.Piva
        Movimento_OperazioneColturale.Sa_Cod = Agenda.Sa_Cod
        Movimento_OperazioneColturale.Lav_Cod = objParametriAgenda.Lav_Cod
        Movimento_OperazioneColturale.Cau_Mov = objParametriAgenda.Cau_Mov
        Movimento_OperazioneColturale.Mov_Desc = Note
        Movimento_OperazioneColturale.Data = Agenda.Data
        Movimento_OperazioneColturale.Ora = CDate(Ora & ":" & Minuti).ToShortTimeString


        '----------------------------------------------------------
        '----------------------------------------------------------
        '----------------------------------------------------------
        'attenzione, modifica nfinchè non vengono utilizzate le ore e i minuti,
        'se sono in modifica riscrivo il vallore precedernte, dato che ora salvo sempre 23:00 essendo te textbox nascoste
        'se si vuole riutilizzare le ore e minuti basta visualuizzarle di nuovo nell'aspx, e togliere il blocco seguente e dovrebbero funzionare correttamente
        Try
            If objParametriAgenda.Tipo_Operazione = TipiEnumerativi.enum_TipoOperazioneDB.Modifica Then
                Dim mov As List(Of Movimento) = New Agenda_Movimenti_Helper().Leggi(Agenda.Piva, Agenda.Sa_Cod, objParametriAgenda.Id_Agenda, objParametri_Server)
                For kk = 0 To mov.Count - 1
                    If mov(kk).Cau_Mov = enum_Agenda_Causali.RILIEVO_CAMPO Then
                        Movimento_OperazioneColturale.Ora = mov(kk).Ora
                    End If
                Next

            End If
        Catch

        End Try
        '----------------------------------------------------------
        '----------------------------------------------------------
        '----------------------------------------------------------


        Movimento_OperazioneColturale.BaseCode = Agenda.BaseCode
        Movimento_OperazioneColturale.TopCode = Agenda.TopCode



        '------------------------------
        '----- MOVIMENTO DET TECNICO --
        '------------------------------
        Dim Movimento_Dettaglio_Tecnico As New Movimento_Dettaglio_Tecnico

        Select Case Agenda.Tipo_Operazione
            Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura
                Movimento_Dettaglio_Tecnico.Id_Agenda = 0
            Case Else
                Movimento_Dettaglio_Tecnico.Id_Agenda = objParametriAgenda.Id_Agenda
        End Select

        Movimento_Dettaglio_Tecnico.Piva = Agenda.Piva
        Movimento_Dettaglio_Tecnico.Sa_Cod = Agenda.Sa_Cod
        Movimento_Dettaglio_Tecnico.Data = AGRODATAINIZIO
        Movimento_Dettaglio_Tecnico.BaseCode = Agenda.BaseCode
        Movimento_Dettaglio_Tecnico.TopCode = Agenda.TopCode
        Select Case CInt(objParametriAgenda.Lav_Cod)
            Case LAVCOD_RILIEVO_PIOGGE
                Agenda.Des_Lib &= " ( " & Pioggia & " mm )" '; TMin: " & TMin & " °C; TMax: " & TMax & " °C; Umidita': " & Umidita & "% ] "
                Movimento_Dettaglio_Tecnico.Qta_Ril = Pioggia
                Movimento_Dettaglio_Tecnico.Piezo1 = TMin
                Movimento_Dettaglio_Tecnico.Piezo2 = TMax
                Movimento_Dettaglio_Tecnico.Piezo3 = Umidita
                Movimento_Dettaglio_Tecnico.Piezo4 = Irraggiamento
            Case Else
                Throw New NotImplementedException
        End Select

        Movimento_OperazioneColturale.Movimenti_Dettagli_Tecnici.Add(Movimento_Dettaglio_Tecnico)

        Agenda.Movimenti.Add(Movimento_OperazioneColturale)

        Return True
    End Function


#End Region





#Region "Sequenza Caricamento operazione da DB"


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

                Dim MovimentiCosti As List(Of Movimento) = New List(Of Movimento)

                Dim letto As Boolean = False
                For i = 0 To Agenda.Movimenti.Count - 1

                    'controllo corrispondeza piva con agenda
                    If Agenda.Piva <> Agenda.Movimenti(i).Piva Then
                        Throw New ApplicationException
                    End If

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case enum_Agenda_Causali.RILIEVO_CAMPO
                            If letto Then
                                Throw New NotImplementedException
                            End If
                            letto = True

                            If Agenda.Lav_Cod <> objParametriAgenda.Lav_Cod Then
                                'controllino per lo sviluppo, da togliere
                                Throw New NotImplementedException
                            End If

                            Select Case CInt(objParametriAgenda.Lav_Cod)
                                Case LAVCOD_RILIEVO_PIOGGE
                                    LeggiMovimentoAgenda_Piogge(Agenda, i)
                                Case Else
                                    Throw New NotImplementedException
                            End Select



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
        End If




    End Sub


    Private Sub LeggiMovimentoAgenda_Piogge(ByRef agenda As Operazione_Agenda, ByRef i As Integer)

        Dim Piva As String = agenda.Piva
        Dim Sacod As Integer = agenda.Sa_Cod
        Dim Data As Date = agenda.Movimenti(i).Data
        If agenda.Data <> agenda.Movimenti(i).Data Then
            Throw New NotImplementedException
        End If
        Dim Ora As Date = agenda.Movimenti(i).Ora
        Dim TxtOre As String = CStr(Ora.Hour)
        Dim TxtMinuti As String = CStr(Ora.Minute)
        Dim Pioggia As String = agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(0).Qta_Ril
        Dim Note As String = agenda.Movimenti(i).Mov_Desc
        Dim TMin As String = agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(0).Piezo1
        Dim TMax As String = agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(0).Piezo2
        Dim Umidita As String = agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(0).Piezo3
        Dim Irraggiamento As String = agenda.Movimenti(i).Movimenti_Dettagli_Tecnici(0).Piezo4


        objParametriAgenda.Data = agenda.Movimenti(i).Data



        Dim dt As DataTable = InizializzaDataTablePerGrigliePiogge()
        '----------------------------------------------------------------------
        '----------------Carico i dati.................................--------
        '----------------------------------------------------------------------

        Dim row As DataRow


        row = dt.NewRow
        row.Item("PIVA") = Piva
        row.Item("Rag_Soc") = (New AgronicaCoreAnagrafeDAL.Imprese_Read().RagSoc_from_Piva(Piva, objParametri_Server))
        row.Item("Sa_Cod") = Sacod
        row.Item("Sa_nome") = (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server)

        row.Item("Data") = Data
        row.Item("Ora") = TxtOre
        row.Item("Minuti") = TxtMinuti
        row.Item("mm_pioggia") = Pioggia
        row.Item("Note") = Note
        row.Item("temp_minima") = TMin
        row.Item("temp_massima") = TMax
        row.Item("umidita") = Umidita
        row.Item("irraggiamento") = Irraggiamento
        row.Item("TotalePeriodoCentro") = Pioggia
        lblDataA3.Text = Pioggia
        dt.Rows.Add(row)

        '----------------------------------------------------------------------
        '----------------Finalizzo la griglia----------------------------------
        '----------------------------------------------------------------------
        finalizzaDataTablePiogge_Bind_GrigliePiogge(dt, GridView_Piogge)
        '----------------------------------------------------------------------
        '----------------------------------------------------------------------
        '----------------------------------------------------------------------

        'carico text
        For j = 0 To GridView_Piogge.Rows.Count - 1
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_Data"), TextBox).Text = dt.Rows(j).Item("Data")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_Ora"), TextBox).Text = dt.Rows(j).Item("Ora")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_Note"), TextBox).Text = dt.Rows(j).Item("note")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_mm_pioggia"), TextBox).Text = dt.Rows(j).Item("mm_pioggia")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_temp_minima"), TextBox).Text = dt.Rows(j).Item("temp_minima")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_temp_massima"), TextBox).Text = dt.Rows(j).Item("temp_massima")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_umidita"), TextBox).Text = dt.Rows(j).Item("umidita")
            CType(Me.GridView_Piogge.Rows(j).FindControl("Txt_irraggiamento"), TextBox).Text = dt.Rows(j).Item("irraggiamento")
        Next


        VisualizzaRilieviPioggeDelPeriodo(Piva, Sacod, Data.AddMonths(-1), Data.AddMonths(1))


    End Sub

    Sub VisualizzaRilieviPioggeDelPeriodo(ByRef Piva As String, ByRef Sacod As Integer, ByRef dataI As Date, ByRef dataF As Date)



        Dim dt As DataTable = InizializzaDataTablePerGrigliePiogge()
        Dim row As DataRow
        Dim dt_mov As DataTable = New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R().LeggiPiogge(Piva, Sacod, 0, dataI, dataF, "", objParametri_Server)

        For i = 0 To dt_mov.Rows.Count() - 1

            row = dt.NewRow
            row.Item("PIVA") = dt_mov.Rows(i).Item("PIVA")
            row.Item("Rag_Soc") = New AgronicaCoreAnagrafeDAL.Imprese_Read().RagSoc_from_Piva(dt_mov.Rows(i).Item("PIVA"), objParametri_Server)
            row.Item("Sa_Cod") = dt_mov.Rows(i).Item("Sa_Cod")
            row.Item("Sa_nome") = (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(dt_mov.Rows(i).Item("PIVA"), dt_mov.Rows(i).Item("Sa_Cod"), objParametri_Server)
            row.Item("ID_Agenda") = dt_mov.Rows(i).Item("ID_Agenda")
            row.Item("Data") = CDate(dt_mov.Rows(i).Item("validita_inizio"))
            Try
                row.Item("Ora") = CDate(dt_mov.Rows(i).Item("Ora")).Hour
                row.Item("Minuti") = CDate(dt_mov.Rows(i).Item("Ora")).Minute
            Catch ex As Exception
                row.Item("Ora") = 23
                row.Item("Minuti") = 0
            End Try
            row.Item("mm_pioggia") = dt_mov.Rows(i).Item("Qta_Ril")
            row.Item("Note") = dt_mov.Rows(i).Item("Mov_Desc")
            row.Item("temp_minima") = dt_mov.Rows(i).Item("Piezo1")
            row.Item("temp_massima") = dt_mov.Rows(i).Item("Piezo2")
            row.Item("umidita") = dt_mov.Rows(i).Item("Piezo3")
            row.Item("irraggiamento") = dt_mov.Rows(i).Item("Piezo4")
            row.Item("TotalePeriodoCentro") = ""


            dt.Rows.Add(row)

        Next

        'sommo totali periodo per centro
        Dim ht As New Hashtable
        For i = 0 To dt.Rows.Count() - 1

            If ht.Contains(dt.Rows(i).Item("Sa_Cod")) Then
                ht.Item(dt.Rows(i).Item("Sa_Cod")) = CDbl(ht.Item(dt.Rows(i).Item("Sa_Cod"))) + CDbl(Replace(dt.Rows(i).Item("mm_pioggia"), ".", ","))
            Else
                ht.Add(dt.Rows(i).Item("Sa_Cod"), dt.Rows(i).Item("mm_pioggia"))
            End If
        Next
        For i = 0 To dt.Rows.Count() - 1
            dt.Rows(i).Item("TotalePeriodoCentro") = ht.Item(dt.Rows(i).Item("Sa_Cod"))
        Next

        finalizzaDataTablePiogge_Bind_GrigliePiogge(dt, GridViewRilieviSalvati)

        If Not IsNothing(GridViewRilieviSalvati) AndAlso GridViewRilieviSalvati.Rows.Count > 0 Then

            'coloro riga selezionata
            Dim i2 As Integer = 0
            For i2 = 0 To GridViewRilieviSalvati.Rows.Count - 1

                If Not IsDBNull(GridViewRilieviSalvati.DataKeys(i2).Item("ID_Agenda")) AndAlso
                    Not IsNothing(GridViewRilieviSalvati.DataKeys(i2).Item("ID_Agenda")) AndAlso
                    IsNumeric(GridViewRilieviSalvati.DataKeys(i2).Item("ID_Agenda")) AndAlso
                    CInt(GridViewRilieviSalvati.DataKeys(i2).Item("ID_Agenda")) = objParametriAgenda.Id_Agenda Then

                    GridViewRilieviSalvati.Rows(i2).BackColor = Drawing.Color.GreenYellow

                End If

            Next
        End If


    End Sub



#End Region





    Private Function checkCoord(ByVal valoreRilevato As String, ByRef messaggio_errore As String) As Boolean

        Dim causa As String = ""
        If valoreRilevato.Contains(".") OrElse valoreRilevato.Contains(",") Then
            causa = Resources.AgronicaAgenda_2010.NonÈUnNumeroIntero
            Return False
        End If

        If Not IsNumeric(valoreRilevato) Then
            causa = Resources.AgronicaAgenda_2010.NonÈUnNumero
            Return False
        End If
        If CInt(valoreRilevato) < 0 Then
            causa = Resources.AgronicaAgenda_2010.NonÈUnNumeroPositivo
            Return False
        End If

        Return True

    End Function





    Private Function Carica_Dati_Da_DB(Piva As String, Sacod As Integer, DT_Piogge As DataTable) As Boolean
        Dim Dt As New DataTable("Piogge")

        Dt.Columns.Add("PIVA", GetType(String))
        Dt.Columns.Add("Rag_Soc", GetType(String))
        Dt.Columns.Add("Sa_Cod", GetType(Integer))
        Dt.Columns.Add("Sa_nome", GetType(String))
        Dt.Columns.Add("Data", GetType(Date))
        Dt.Columns.Add("Ora", GetType(Integer))
        Dt.Columns.Add("mm_pioggia", GetType(String))
        Dt.Columns.Add("Note", GetType(String))
        Dt.Columns.Add("temp_minima", GetType(Integer))
        Dt.Columns.Add("temp_massima", GetType(Integer))
        Dt.Columns.Add("umidita", GetType(Integer))
        Dt.Columns.Add("irraggiamento", GetType(Integer))

        Return True

    End Function




    Private Sub GridViewCentriMeteo_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridViewCentriMeteo.RowCommand

        'Recupero l'indice di riga del datagrid
        Dim IndiceRigaGriglia As Integer
        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)


        Select Case e.CommandName

            Case "RicercaPerIndirizzo"

                Dim Piva As String
                Dim Sa_Cod As Integer
                Piva = GridViewCentriMeteo.DataKeys(IndiceRigaGriglia).Item("PIVA")
                Sa_Cod = GridViewCentriMeteo.DataKeys(IndiceRigaGriglia).Item("Sa_Cod")
                Session("GridViewCentriStazioni") = False
                RicercaPerIndirizzo(IndiceRigaGriglia, Piva, Sa_Cod, 1)




            Case "SalvaCoordinate"

                Salva_CoordinateXY_CentroAziendale(IndiceRigaGriglia)


        End Select



    End Sub


    Private Sub RicercaPerIndirizzo(ByVal IndiceRigaGriglia As Integer, ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal tipo As Integer)

        Dim obj_Centro As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
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
        Dim Address_Crypt As String = ""

        obj_Centro.Indirizzo_from_PivaSaCod(Piva, Sa_Cod,
                            Ind_Des, Frz_Des,
                            CAP, Stato,
                            Comune, Provincia, Sigla_Prov,
                            pro_cod_istat, com_cod_istat, Cod_Regione,
                            objParametri_Server)

        Address_Crypt = Stringa_Codifica_LANCompatibile(Comune & "," & Ind_Des, AgroKey_EncoderDecoder)

        Dim parX As String
        Dim parY As String

        If tipo = 1 Then 'Rilevazione Meteo Dati

            parX = Me.GridViewCentriMeteo.Rows(IndiceRigaGriglia).FindControl("Txt_CoordX").ClientID
            parY = Me.GridViewCentriMeteo.Rows(IndiceRigaGriglia).FindControl("Txt_CoordY").ClientID

        ElseIf tipo = 2 Then

            parY = Me.GridViewCentriStazioni.Rows(IndiceRigaGriglia).FindControl("Txt_Longitudine").ClientID
            parX = Me.GridViewCentriStazioni.Rows(IndiceRigaGriglia).FindControl("Txt_Latitudine").ClientID

        Else

            Messaggi.AgroMsgBox("", Page, "Form", UpdatePanelFiltroCentri)
        End If


        Dim Qs_Piva_Crypt As String = Stringa_Codifica_LANCompatibile(Piva, AgroKey_EncoderDecoder)

        Dim sSa_Cod_Crypt As String = ""


        'Recupero il fuso per la conversione UTM
        Dim AgroWebC As New AgroWebConfig()
        Dim Fuso As String = AgroWebC.Fuso
        Dim Fuso_Crypt As String = Stringa_Codifica_LANCompatibile(Fuso, AgroKey_EncoderDecoder)

        'Dim UrlRitaglioServizio As String = AgroWebC.Ritaglio_Servizio
        Dim UrlRitaglioSito As String = "../Agro_GoogleMaps/xMapFinder/Container.aspx"
        'IP_Server = UrlRitaglio.Substring(7, UrlRitaglio.IndexOf("/", 7) - 7)

        'objParametri_Server.

        Dim SuperUser_Username_Crypt As String = Stringa_Codifica_LANCompatibile(CStr(Session("ASG_SuperUser_Username")), AgroKey_EncoderDecoder)
        Dim SuperUser_Password_Crypt As String = Stringa_Codifica_LANCompatibile(CStr(Session("ASG_SuperUser_Password")), AgroKey_EncoderDecoder)
        Dim txtCordX_crypt As String = Stringa_Codifica_LANCompatibile(parX, AgroKey_EncoderDecoder)
        Dim txtCordY_crypt As String = Stringa_Codifica_LANCompatibile(parY, AgroKey_EncoderDecoder)
        Dim tipo_crypt As String = Stringa_Codifica_LANCompatibile(tipo, AgroKey_EncoderDecoder)

        Dim link As String = UrlRitaglioSito &
            "?p=" & Qs_Piva_Crypt &
            "&s=" & sSa_Cod_Crypt &
            "&f=" & Fuso_Crypt &
            "&n=" & SuperUser_Username_Crypt &
            "&w=" & SuperUser_Password_Crypt &
            "&a=" & Address_Crypt &
            "&x=" & txtCordX_crypt &
            "&y=" & txtCordY_crypt &
            "&t=" & tipo_crypt &
            "&b=" & Stringa_Codifica_LANCompatibile(BottoneNascostoWS.ClientID, AgroKey_EncoderDecoder)

        'GABRIELE GESTIONE del click al bottone nascosto all'OK del popup (MIO MALGRADO)

        Session("RilievoPiogge_CoordinateGIS") = IndiceRigaGriglia

        'salvati per postback
        'Session("RilievoPiogge_WS_SuperUser_Username_Crypt") = SuperUser_Username_Crypt
        'Session("RilievoPiogge_WS__SuperUser_Password_Crypt") = SuperUser_Password_Crypt
        'Session("RilievoPiogge_WS__Qs_Piva_Crypt") = Qs_Piva_Crypt
        Session("RilievoPiogge_WS__Piva") = Piva
        Session("RilievoPiogge_WS__Sa_Cod") = Sa_Cod
        'Session("RilievoPiogge_WS__IndiceRigaGriglia") = IndiceRigaGriglia
        'Session("RilievoPiogge_WS__UrlRitaglioServizio") = UrlRitaglioServizio

        Dim strJS As New StringBuilder
        strJS.AppendLine("$(document).ready(function () { ")
        'strJS.AppendLine("      alert(''); ")
        'strJS.AppendLine("      window.showModalDialog( '" & link & "','','dialogWidth:600px; dialogHeight:600px; status:no; center:yes; edge:raised; help:no; scroll:no; resizable:no; '); ")
        strJS.AppendLine("      var child = window.open( '" & link & "','map_win','width=700, height=600'); ")
        'strJS.AppendLine("      child.postMessage({ message: 'requestResult' }, '*');")
        'strJS.AppendLine("      clickButtonWS(); ")
        strJS.AppendLine(" });")

        ScriptManager.RegisterClientScriptBlock(Master_Operazione.Property_UpdatePanelToolBar,
                                                Master_Operazione.Property_UpdatePanelToolBar.GetType(),
                                                String.Format("jQuery_{0}", Master_Operazione.Property_UpdatePanelToolBar.ClientID),
                                                strJS.ToString, True)
    End Sub


    Private Sub Salva_CoordinateXY_CentroAziendale(ByRef i As Integer)

        Dim msgerrror As String = ""
        Dim Piva As String = CStr(GridViewCentriMeteo.DataKeys(i).Item("PIVA"))
        Dim Sacod As Integer = CInt(GridViewCentriMeteo.DataKeys(i).Item("Sa_Cod"))
        Dim Coordinata_X_Str As String = CType(GridViewCentriMeteo.Rows(i).FindControl("Txt_CoordX"), TextBox).Text
        Dim Coordinata_Y_Str As String = CType(GridViewCentriMeteo.Rows(i).FindControl("Txt_CoordY"), TextBox).Text

        If Not Verifica_Coordinate(Coordinata_X_Str, Coordinata_Y_Str, "0", msgerrror) Then
            msgerrror = " Centro " & (New AgronicaCoreAnagrafeDAL.CentriAziendali_Read()).SaNome_from_SaCod(Piva, Sacod, objParametri_Server) & "; Riga: " & i & " . <br> " & msgerrror
            Messaggi.AgroMsgBox(msgerrror, Page, , Master_Operazione.Property_UpdatePanelToolBar)
            Exit Sub
        End If

        Try

            Dim obj_CentroW As New AgronicaCoreAnagrafeDAL.CentriAziendali_Write
            Dim risp As Boolean

            Const XYtoLatLong As Integer = 1

            Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R

            Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(XYtoLatLong, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim cconverter As New CoordinateConverter

            Dim ParametriCartograficiWGS84ED50 As New ParametriCoordinateConverter With {
                .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
                .CStoText = dtLeggiTrasformazione(0)("CSTo"),
                .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
                .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
                .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
                .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
            }

            Dim X As Integer = Coordinata_X_Str
            Dim Y As Integer = Coordinata_Y_Str
            Dim Lat, Lng As Double

            cconverter.WGS84_from_ED50(ParametriCartograficiWGS84ED50, Y, X, Lat, Lng)

            risp = obj_CentroW.Modifica_CoordinateLatLong_e_XY(Piva, Sacod, Lat, Lng, X, Y, objParametri_Server)

            If risp Then
                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.SalvataggioDelleCoordinateAvvenutoConSucce, Page, , Master_Operazione.Property_UpdatePanelToolBar)
            Else
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IlSalvataggioDelleCoordinateNonÈAndatoABuo, Page, , Master_Operazione.Property_UpdatePanelToolBar)
            End If

        Catch ex As Exception
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SiÈVerificatoUnErroreDuranteIlSalvataggioD & ex.Message, Page, , Master_Operazione.Property_UpdatePanelToolBar)
        End Try
    End Sub


    Private Function Verifica_Coordinate(ByRef X As String,
                                         ByRef Y As String,
                                         ByRef quadrante As String,
                                         ByRef msgerrror As String) As Boolean
        Dim lX As String = "X"
        Dim lY As String = "Y"

        Dim causa As String = ""
        If Not checkCoord(X, causa) Then
            msgerrror &= String.Format(Resources.AgronicaAgenda_2010.LaCoordinataX0NonÈAmmissibileX1X2, lX, X, causa)
            Return False
        End If

        If Not checkCoord(Y, causa) Then
            msgerrror &= String.Format(Resources.AgronicaAgenda_2010.LaCoordinataX0NonÈAmmissibileX1X2, lY, Y, causa)
            Return False
        End If

        If Not checkCoord(quadrante, causa) Then
            msgerrror &= String.Format(Resources.AgronicaAgenda_2010.IlQuadranteNonÈAmmissibileX0X1, quadrante, causa)
            Return False
        End If

        Return True

    End Function


    Private Sub BottoneNascostoWS_Click(sender As Object, e As System.EventArgs) Handles BottoneNascostoWS.Click

        '**********************************************************************
        ' GABRIELE 2023 10 06
        ' all'OK del popup GIS eseguo questo codice
        '**********************************************************************
        If IsNothing(Session("RilievoPiogge_CoordinateGIS")) Then

            Return
        End If

        If Not IsNumeric(Session("RilievoPiogge_CoordinateGIS")) Then

            Session("RilievoPiogge_CoordinateGIS") = Nothing

            Return
        End If

        Dim IndiceRigaGriglia As Integer = Session("RilievoPiogge_CoordinateGIS")

        Session("RilievoPiogge_CoordinateGIS") = Nothing

        If Not IsNothing(Session("GridViewCentriStazioni")) AndAlso Session("GridViewCentriStazioni") = True Then

            Dim gridRow = GridViewCentriStazioni.Rows(IndiceRigaGriglia)

            Dim Txt_Lat As String = CType(gridRow.FindControl("Txt_Latitudine"), TextBox).Text
            Dim Txt_Lng As String = CType(gridRow.FindControl("Txt_Longitudine"), TextBox).Text

            Dim lat = CDec(Txt_Lat)
            Dim lng = CDec(Txt_Lng)

            Dim Piva As String = Session("RilievoPiogge_WS__Piva")
            Dim Sa_Cod As Integer = Session("RilievoPiogge_WS__Sa_Cod")

            Dim DT_Stazioni = GetStazioni(Piva, lat, lng)

            RiempiComboConStazioni(CType(gridRow.Cells(7).Controls(1), DropDownList), DT_Stazioni, Piva, Sa_Cod)

        End If




        ''salvati per postback
        'Dim SuperUser_Username_Crypt As String = Session("RilievoPiogge_WS_SuperUser_Username_Crypt")
        'Dim SuperUser_Password_Crypt As String = Session("RilievoPiogge_WS__SuperUser_Password_Crypt")
        'Dim Qs_Piva_Crypt As String = Session("RilievoPiogge_WS__Qs_Piva_Crypt")
        'Dim Piva As String = Session("RilievoPiogge_WS__Piva")
        'Dim Sa_Cod As Integer = Session("RilievoPiogge_WS__Sa_Cod")
        'Dim IndiceRigaGriglia As Integer = Session("RilievoPiogge_WS__IndiceRigaGriglia")
        'Dim UrlRitaglioServizio As String = Session("RilievoPiogge_WS__UrlRitaglioServizio")

        ''Dim objws As ws_mappe.Gias_Service
        ''Dim ris As String



        ''objws = New ws_mappe.Gias_Service
        ''objws.Url = UrlRitaglioServizio
        'Dim objCentro As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
        'Dim X, Y As String
        'objCentro.Recupera_CoordinateUTM(X, Y, Piva, Sa_Cod, "", objParametri_Server)
        ''objCentro.Modifica_CoordinateLatLong_e_XY(Piva, Sacod, Latitudine, Longitudine, CInt(Coordinata_X_Str), CInt(Coordinata_Y_Str), objParametri_Server)
        ''ris = objws.RecuperaCoordinate(SuperUser_Username_Crypt, SuperUser_Password_Crypt, Qs_Piva_Crypt, "")

        'If Not X Is Nothing AndAlso X <> "" AndAlso Not Y Is Nothing AndAlso Y <> "" Then


        '    If Not IsNothing(Session("GridViewCentriStazioni")) AndAlso Session("GridViewCentriStazioni") = True Then
        '        Dim Txt_Latitudine As Decimal
        '        Dim Txt_Longitudine As Decimal


        '        CalcolaLatLongDaXY(X, Y, Txt_Latitudine, Txt_Longitudine)


        '        CType(GridViewCentriStazioni.Rows(-IndiceRigaGriglia).FindControl("Txt_Latitudine"), TextBox).Text = Txt_Latitudine
        '        CType(GridViewCentriStazioni.Rows(-IndiceRigaGriglia).FindControl("Txt_Longitudine"), TextBox).Text = Txt_Longitudine

        '        Dim DT_Stazioni = GetStazioni(Piva, Txt_Latitudine, Txt_Longitudine)

        '        RiempiComboConStazioni(CType(GridViewCentriStazioni.Rows(-IndiceRigaGriglia).Cells(7).Controls(1), DropDownList), DT_Stazioni, Piva, Sa_Cod)

        '    Else
        '        CType(GridViewCentriMeteo.Rows(IndiceRigaGriglia).FindControl("Txt_CoordX"), TextBox).Text = X
        '        CType(GridViewCentriMeteo.Rows(IndiceRigaGriglia).FindControl("Txt_CoordY"), TextBox).Text = Y
        '    End If

        'End If

    End Sub

    Private Sub settoNumeroQuadranteSeZero(centro_Aziendale As AgronicaCoreModello.Anagrafe.Centro_Aziendale, quadranteReturn As Integer)
        Dim mess As String = ""

        Try
            If Not IsNothing(GridViewCentriMeteo) Then
                For i = 0 To Me.GridViewCentriMeteo.Rows.Count - 1

                    Dim tempquadrante As String = CStr(CType(GridViewCentriMeteo.Rows(i).FindControl("Txt_NumeroQuadrante"), TextBox).Text)

                    If CStr(GridViewCentriMeteo.DataKeys(i).Item("PIVA")) = centro_Aziendale.Azienda.Piva AndAlso
                        CInt(GridViewCentriMeteo.DataKeys(i).Item("Sa_Cod")) = centro_Aziendale.Sa_Cod AndAlso
                        (Not checkCoord(tempquadrante, mess) OrElse
                        tempquadrante = "0") Then
                        CType(GridViewCentriMeteo.Rows(i).FindControl("Txt_NumeroQuadrante"), TextBox).Text = CStr(quadranteReturn)
                    End If
                Next
            End If
        Catch

        End Try

    End Sub

    Sub stazioni_verifica_seDataUltAggPres(ByRef dataFinale As Date, ByVal nomestazione As String)

        '**********************************************************************************
        'GABRIELE DA VERIFICARE UTILIZZO ViewState("RilievoPiogge_tempStazioni")
        '**********************************************************************************

        Dim Dt_stazioni As DataTable
        If Not IsNothing(ViewState("RilievoPiogge_tempStazioni")) Then
            Dt_stazioni = ViewState("RilievoPiogge_tempStazioni")
            Dim staz As DataRow() = Dt_stazioni.Select("Id = " & nomestazione.ToString)
            If Not IsNothing(staz) AndAlso staz.Count > 0 Then
                If Not IsDBNull(staz(0).Item("data_ultimo_agg")) AndAlso staz(0).Item("data_ultimo_agg") < dataFinale Then
                    dataFinale = CDate(staz(0).Item("data_ultimo_agg"))
                End If
            End If
        End If
    End Sub


#Region "Metos coordinate"

    Private Sub GridViewCentriStazioni_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridViewCentriStazioni.RowCommand

        'Recupero l'indice di riga del datagrid
        Dim IndiceRigaGriglia As Integer
        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)


        Select Case e.CommandName

            Case "RicercaPerIndirizzoLatLong"

                Dim Piva As String
                Dim Sa_Cod As Integer
                Piva = GridViewCentriStazioni.DataKeys(IndiceRigaGriglia).Item("PIVA")
                Sa_Cod = GridViewCentriStazioni.DataKeys(IndiceRigaGriglia).Item("Sa_Cod")
                'per capire che mi trovo nel caso stazioni metos al riutorno imposto Session("GridViewCentriStazioni") =true
                'al ritorno deve reimpostare la combo della riga
                Session("GridViewCentriStazioni") = True
                RicercaPerIndirizzo(IndiceRigaGriglia, Piva, Sa_Cod, 2)


            Case "SalvaCoordinateLatLong"

                Salva_CoordinateLatLong_CentroAziendale(IndiceRigaGriglia)


        End Select



    End Sub


    Private Sub Salva_CoordinateLatLong_CentroAziendale(ByRef i As Integer)

        Dim Piva As String = CStr(GridViewCentriStazioni.DataKeys(i).Item("PIVA"))
        Dim Sacod As Integer = CInt(GridViewCentriStazioni.DataKeys(i).Item("Sa_Cod"))
        Dim Txt_Latitudine As String = CType(GridViewCentriStazioni.Rows(i).FindControl("Txt_Latitudine"), TextBox).Text
        Dim Txt_Longitude As String = CType(GridViewCentriStazioni.Rows(i).FindControl("Txt_Longitudine"), TextBox).Text

        Try

            Dim lat As Decimal = CDbl(Txt_Latitudine)
            Dim lng As Decimal = CDbl(Txt_Longitude)

            'Const LatLongToXY As Integer = 4
            ''Const XYtoLatLong As Integer = 1
            'Dim leggiTrasformazione As New AgronicaCoreGisDAL.GIS_SistemiRiferimentoCartografia_R
            'Dim dtLeggiTrasformazione As DataTable = leggiTrasformazione.Leggi(LatLongToXY, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            'Dim cconverter As New CoordinateConverter
            'Dim ParametriCartograficiWGS84ED50 As New ParametriCoordinateConverter With {
            '    .CSFromText = dtLeggiTrasformazione(0)("CSFrom"),
            '    .CStoText = dtLeggiTrasformazione(0)("CSTo"),
            '    .CStoGeoText = dtLeggiTrasformazione(0)("CStoGeo"),
            '    .AgronicaLatOffset = dtLeggiTrasformazione(0)("AgronicaLatOffset"),
            '    .AgronicaLonOffset = dtLeggiTrasformazione(0)("AgronicaLonOffset"),
            '    .LibreriaDaUsare = dtLeggiTrasformazione(0)("LibreriaDaUsare")
            '}

            'Dim CoordWktSudEst As String = "((" &
            '    Convert.ToString(lng, System.Globalization.CultureInfo.InvariantCulture) &
            '    "," &
            '    Convert.ToString(lat, System.Globalization.CultureInfo.InvariantCulture) &
            '    "))"

            'Dim risultato As String = cconverter.WKTPolygonWGS84_from_WKTPolygonED50(CoordWktSudEst, True, ParametriCartograficiWGS84ED50)
            Dim X As Integer '= CDbl(risultato.Split(" ")(1).Replace("(", "").Replace(")", "").Replace(".", ","))
            Dim Y As Integer '= CDbl(risultato.Split(" ")(2).Replace("(", "").Replace(")", "").Replace(".", ","))
            X = 0
            Y = 0

            Dim obj_CentroW As New AgronicaCoreAnagrafeDAL.CentriAziendali_Write

            Dim risp = obj_CentroW.Modifica_CoordinateLatLong_e_XY(Piva, Sacod, lat, lng, X, Y, objParametri_Server)

            If risp Then
                Messaggi.AgroMsgBuonFine(Resources.AgronicaAgenda_2010.SalvataggioDelleCoordinateAvvenutoConSucce, Page, , Master_Operazione.Property_UpdatePanelToolBar)
            Else
                Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.IlSalvataggioDelleCoordinateNonÈAndatoABuo, Page, , Master_Operazione.Property_UpdatePanelToolBar)
            End If

        Catch ex As Exception
            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.SiÈVerificatoUnErroreDuranteIlSalvataggioD & ex.Message, Page, , Master_Operazione.Property_UpdatePanelToolBar)
        End Try
    End Sub


#End Region


    '*** GABRIELE 2023 10 05 ***

    Private Function GetTipoSorgenteMeteo() As Integer

        If RBL_TipoRilievo.SelectedValue = "3" Then

            Return enum_Meteo_Tiposorgente.Pubbliche
        End If

        Return enum_Meteo_Tiposorgente.RetiPartner
    End Function


    Private Function GetStazioni(piva As String, lat As Double, lng As Double) As DataTable

        'Dim objMeteo As New AgronicaCoreWebService.MeteoNT
        Dim objMeteoSuite As New InterfacciaMeteoSuite(objParametri_Server, objParametri_Super_Server)

        Dim objP As New JObject
        objP("PIVA_Superuser") = objParametri_Server.PivaSuperUser
        objP("PIVA") = piva
        objP("TipoSorgente") = GetTipoSorgenteMeteo()
        objP("DistanzaDa") = New JObject(
            New JProperty("lat", lat),
            New JProperty("lng", lng)
            )

        'Dim json = JObject.Parse(objMeteo.LeggiStazioniXSorgente(objP, objParametri_Server))
        Dim json = JObject.Parse(objMeteoSuite.LeggiStazioniXSorgente(objP))

        Dim arrStazioni = JArray.Parse(json("RispostaStringa").ToString())

        Dim DT_Stazioni = New DataTable

        DT_Stazioni.Columns.Add(New DataColumn("id", GetType(Integer)))
        DT_Stazioni.Columns.Add(New DataColumn("descrizione", GetType(String)))
        DT_Stazioni.Columns.Add(New DataColumn("fornitore", GetType(String)))
        DT_Stazioni.Columns.Add(New DataColumn("rif_fornitore", GetType(String)))
        DT_Stazioni.Columns.Add(New DataColumn("data_ultimo_agg", GetType(Date)))
        DT_Stazioni.Columns.Add(New DataColumn("distanza", GetType(Decimal)))

        Dim row As DataRow

        For Each elem In arrStazioni

            row = DT_Stazioni.NewRow

            row("id") = CInt(elem("id_stazione"))
            row("descrizione") = elem("nome_stazione").ToString
            row("fornitore") = elem("fornitore").ToString
            row("rif_fornitore") = elem("rif_fornitore").ToString

            If elem("ultimo_aggiornamento") IsNot Nothing Then
                row("data_ultimo_agg") = CDate(elem("ultimo_aggiornamento"))
            Else
                row("data_ultimo_agg") = DBNull.Value
            End If

            If elem("distanza") IsNot Nothing Then
                row("distanza") = CDec(elem("distanza"))
            Else
                row("distanza") = DBNull.Value
            End If

            DT_Stazioni.Rows.Add(row)
        Next

        'Dim DT_Stazioni = objMeteo.ElencoStazioniConDistanza(
        '    objParametri_Server.PivaSuperUser,
        '    GetDoorkey(),
        '    HttpContext.Current.Session("ASG_Utente_Username"),
        '    HttpContext.Current.Session("ASG_Utente_Username_Crypt"),
        '    HttpContext.Current.Session("ASG_Utente_Password_Crypt"),
        '    piva,
        '    objParametri_Server,
        '    lat,
        '    lng,
        '    GetTipoSorgenteMeteo())

        Return DT_Stazioni
    End Function


End Class