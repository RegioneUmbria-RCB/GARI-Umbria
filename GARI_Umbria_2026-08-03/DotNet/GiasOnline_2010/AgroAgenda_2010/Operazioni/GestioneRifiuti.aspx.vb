Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreUtility


Public Class GestioneRifiuti
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda

    'Dim PaginaRitorno As String = "../Menu/Menu.aspx"

    Const ColonnaCarico As Integer = 2
    Const ColonnaScarico As Integer = 3
    Const ColonnaRifiuto As Integer = 4
    Const ColonnaUdM As Integer = 5
    Const ColonnaQta As Integer = 6
       

    Private Sub DuplicaOperazione_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto
    End Sub

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Session.Remove("DT_Rifiuti")
        objParametriAgenda.Svuota_DatiOperazione()

        Dim link As String = CType(Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
        Response.Redirect(link)

    End Sub

    Private Sub AAA_GestioneUscitaPagina()

        Session.Remove("DT_Rifiuti")
        objParametriAgenda.Svuota_DatiOperazione()

        Dim link As String = CType(Master, Agenda).TrovaRedirectCorretto(False, enum_PagineGiasOnline.MenuAgenda, objParametriAgenda)
        Response.Redirect(link)

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("../Custom500.aspx")
        End If

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
        objParametriAgenda = New ParametriAgenda

        InizializzaScript()

        If Not Page.IsPostBack Then

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            Dim UtenteAbilitato_Lettura As Boolean = False
            UtenteAbilitato_Lettura = objPermessi.Controlla_Permessi_Utente( _
                                       Session("ASG_Utente_Username"), _
                                       Session("ASG_IdServizio"), _
                                       enum_Security_Attivita.Gestione_Rifiuti, _
                                       enum_Security_Operazione.Lettura, _
                                       Date.Now, _
                                       "", _
                                       objParametri_Utenti)

            If Not UtenteAbilitato_Lettura Then
                AnnullaTutto(Me, Nothing)
                ' Exit Sub
            End If

            'AgronicaCoreUtility.CaricaListControl.Contatti(Cmb_Smaltitore, True, "SELEZIONA", "", _
            '                               objParametriAgenda.Piva, _
            '                               "", 0, COD_SMALTITORE, _
            '                               True, False, 0, 0, False, 0, _
            '                               ID_CF_NOFILTRO, _
            '                               "", _
            '                               "", _
            '                               objParametri_Server)

            CreaDt_Rifiuti()

            Select Case objParametriAgenda.Tipo_Operazione

                Case TipiEnumerativi.enum_TipoOperazioneDB.Scrittura

                    Dim MessaggioErrore As String = ""
                    Inserisci_Rifiuto(1, "", "", "", 0, "", MessaggioErrore)
                    Inserisci_Rifiuto(2, "", "", "", 0, "", MessaggioErrore)
                    Inserisci_Rifiuto(3, "", "", "", 0, "", MessaggioErrore)

                Case TipiEnumerativi.enum_TipoOperazioneDB.Lettura

                    div_comandi.Visible = False
                    Ripristina_Dati_nei_Controlli()

                Case Else

                    Ripristina_Dati_nei_Controlli()

            End Select

            AggiornaGriglia_Rifiuti()

        Else
            Exit Sub
        End If


    End Sub

    Private Sub CreaDt_Rifiuti()

        Dim Dt As New DataTable

        Dt.Columns.Add(New DataColumn("Contatore", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Codice", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Rifiuto", GetType(String)))
        Dt.Columns.Add(New DataColumn("DataCarico", GetType(String)))
        Dt.Columns.Add(New DataColumn("DataScarico", GetType(String)))
        Dt.Columns.Add(New DataColumn("Udm_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Qta", GetType(String)))

        Dim DtKeys(0) As DataColumn
        DtKeys(0) = Dt.Columns("Contatore")
        Dt.PrimaryKey = DtKeys

        Session("DT_Rifiuti") = Dt

    End Sub

    Private Sub AggiornaGriglia_Rifiuti()

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dim Contatore As Integer = 0

        If Session("DT_Rifiuti") IsNot Nothing Then

            Dt = Session("DT_Rifiuti")

            Dim DtKeysG(0) As String
            DtKeysG(0) = "Contatore"

            GridViewRifiuti.DataSource = Dt
            GridViewRifiuti.DataKeyNames = DtKeysG
            GridViewRifiuti.DataBind()

            For i = 0 To GridViewRifiuti.Rows.Count - 1

                Contatore = GridViewRifiuti.DataKeys(i).Item(0)
                Dr = Dt.Rows.Find(Contatore)

                Select Case Dr.Item("Codice")
                    Case Is <> 0
                        GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("CmbRifiuti").Visible = True
                        AgronicaCoreUtility.CaricaListControl.Rifiuti(CType(GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("CmbRifiuti"), DropDownList), _
                                                     True, "SELEZIONA", "0", "", "cer_cod", True, objParametri_Server)
                        GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("TxtRifiuti").Visible = False
                    Case Else
                        GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("CmbRifiuti").Visible = False
                        GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("TxtRifiuti").Visible = True
                End Select

                AgronicaCoreUtility.CaricaListControl.Udm_Optimize(CType(GridViewRifiuti.Rows(i).Cells(ColonnaUdM).Controls(1), DropDownList), _
                                                                   Nothing, "", 0, 0, CAU_CARICO, RIFIUTI, False, 0, 0, True, "SELEZIONA", "0", "", "", "", objParametri_Server, objParametri_Utenti)

            Next



            For i = 0 To GridViewRifiuti.Rows.Count - 1

                Contatore = GridViewRifiuti.DataKeys(i).Item(0)

                Dr = Dt.Rows.Find(Contatore)

                Select Case Dr.Item("Codice")
                    Case Is <> 0
                        CType(GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("CmbRifiuti"), DropDownList).SelectedValue = Dr.Item("Codice")
                    Case Else
                        CType(GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("TxtRifiuti"), TextBox).Text = Dr.Item("Rifiuto")
                End Select

                CType(GridViewRifiuti.Rows(i).Cells(ColonnaCarico).Controls(1), TextBox).Text = Dr.Item("DataCarico")
                CType(GridViewRifiuti.Rows(i).Cells(ColonnaScarico).Controls(1), TextBox).Text = Dr.Item("DataScarico")
                CType(GridViewRifiuti.Rows(i).Cells(ColonnaUdM).Controls(1), DropDownList).SelectedValue = Dr.Item("Udm_Cod")
                CType(GridViewRifiuti.Rows(i).Cells(ColonnaQta).Controls(1), TextBox).Text = Dr.Item("Qta")

            Next

        End If

    End Sub

    Private Sub Inserisci_Rifiuto(ByVal Codice As Integer, _
                                  ByVal Rifiuto As String, _
                                  ByVal DataCarico As String, ByVal DataScarico As String, _
                                  ByVal Udm_Cod As Integer, ByVal Qta As String, _
                                  ByRef MesssaggioErrore As String)

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dim Contatore As Integer = 0
        Dim MaxContatore As Integer = 0

        If Session("DT_Rifiuti") IsNot Nothing Then

            Dt = Session("DT_Rifiuti")

            'selezione max contatore
            For i = 0 To Dt.Rows.Count - 1
                Contatore = Dt.Rows(i).Item("Contatore")
                If MaxContatore < Contatore Then
                    MaxContatore = Contatore
                End If
            Next

            Dr = Dt.NewRow
            Dr.Item("Contatore") = MaxContatore + 1
            Dr.Item("Codice") = Codice
            Dr.Item("Rifiuto") = Rifiuto
            Dr.Item("DataCarico") = DataCarico
            Dr.Item("DataScarico") = DataScarico
            Dr.Item("Udm_Cod") = Udm_Cod
            Dr.Item("Qta") = Qta
            Dt.Rows.Add(Dr)

            Session("DT_Rifiuti") = Dt

        End If

    End Sub

    Private Sub Salva_Dati()

        Dim Contatore As Integer
        Dim DataCarico As String
        Dim DataScarico As String
        Dim Codice As Integer
        Dim Rifiuto As String
        Dim Udm_Cod As String
        Dim Qta As String

        Dim Dt As New DataTable
        Dim Dr As DataRow

        Dt = Session("DT_Rifiuti")

        '------------------------------------------
        'salvo i valori già indicati (date,rifiuto, udm, qta)
        For i = 0 To GridViewRifiuti.Rows.Count - 1

            'Trovo la riga 
            Contatore = GridViewRifiuti.DataKeys(i).Item(0)

            DataCarico = CType(GridViewRifiuti.Rows(i).Cells(ColonnaCarico).Controls(1), TextBox).Text
            DataScarico = CType(GridViewRifiuti.Rows(i).Cells(ColonnaScarico).Controls(1), TextBox).Text

            If GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("CmbRifiuti").Visible = True Then
                Codice = CInt(CType(GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("CmbRifiuti"), DropDownList).SelectedValue)
                Rifiuto = ""
            ElseIf GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("TxtRifiuti").Visible = True Then
                Codice = 0
                Rifiuto = CType(GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("TxtRifiuti"), TextBox).Text
            End If

            Udm_Cod = CType(GridViewRifiuti.Rows(i).Cells(ColonnaUdM).Controls(1), DropDownList).SelectedValue
            Qta = CType(GridViewRifiuti.Rows(i).Cells(ColonnaQta).Controls(1), TextBox).Text

            Dr = Dt.Rows.Find(Contatore)

            Dr.Item("Codice") = Codice
            Dr.Item("Rifiuto") = Rifiuto
            Dr.Item("DataCarico") = DataCarico
            Dr.Item("DataScarico") = DataScarico
            Dr.Item("Udm_Cod") = Udm_Cod
            Dr.Item("Qta") = Qta

        Next

        Session("DT_Rifiuti") = Dt

    End Sub

    Protected Sub Btn_Aggiungi_Click(sender As Object, e As EventArgs) Handles Btn_Aggiungi.Click

        Dim MessaggioErrore As String = ""

        'salvo ciò che è gia stato inserito
        Salva_Dati()

        '------------------------------------------
        'inserisco riga nuova vuota
        Inserisci_Rifiuto(0, "", "", "", 0, "", MessaggioErrore)

        '------------------------------------------
        'ri-setto i valori pre-inseriti dall'utente
        AggiornaGriglia_Rifiuti()


    End Sub

    Private Sub InizializzaScript()

        Dim Str As New StringBuilder

        Str.AppendLine("$(document).ready(function () {")

        Str.AppendLine(" $('.bottone').button(); ")

        Str.AppendLine("    $('.datepicker').datepicker({ ")
        Str.AppendLine("    dateFormat:  'dd/mm/yy',")
        Str.AppendLine("    disabled: false,")
        Str.AppendLine("    changeMonth: true,")
        Str.AppendLine("    changeYear: true")
        Str.AppendLine("});")
        Str.AppendLine("$.datepicker.regional['it'];")

        Str.AppendLine("});")

        ScriptManager.RegisterStartupScript(upDati, upDati.GetType(),
                                         String.Format("jQuery_{0}", upDati.ClientID), Str.ToString, True)
    End Sub

    Protected Sub ImgBtnSalvaTutto_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnSalvaTutto.Click


        '----------------------------------------------------------------
        'controlli conformità dati
        Dim MessaggioErrore As String = ""


        If Txt_Smaltitore.Text = "" Then
            MessaggioErrore &= "Selezionare lo smaltitore!<br>"
        End If
        'If Not IsDate(Txt_Data.Text) Then
        '    MessaggioErrore &= "Indicare la data di smaltimento!<br>"
        'End If
        If Txt_Convenzione.Text = "" Then
            MessaggioErrore &= "Indicare gli estremi della convenzione!<br>"
        End If
        If Txt_DocConvenzione.Text = "" Then
            MessaggioErrore &= "Indicare gli estremi del documento della convenzione!<br>"
        End If

        Dim Rifiuto As String

        For i = 0 To GridViewRifiuti.Rows.Count - 1

            Rifiuto = ""

            If GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("CmbRifiuti").Visible = True Then
                If CType(GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("CmbRifiuti"), DropDownList).SelectedValue = "0" Then
                    MessaggioErrore &= "Indicare un rifiuto per ogni riga!" & "<br>"
                Else
                    Rifiuto = CType(GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).Controls(1), DropDownList).SelectedItem.Text
                End If
            Else
                If CType(GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("TxtRifiuti"), TextBox).Text = "" Then
                    MessaggioErrore &= "Indicare un rifiuto per ogni riga!" & "<br>"
                Else
                    Rifiuto = CType(GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("TxtRifiuti"), TextBox).Text
                End If
            End If


            If Not IsDate(CType(GridViewRifiuti.Rows(i).Cells(ColonnaCarico).Controls(1), TextBox).Text) Then
                MessaggioErrore &= "Indicare la data di carico del rifiuto: " & Rifiuto & "<br>"
            End If
            If Not IsDate(CType(GridViewRifiuti.Rows(i).Cells(ColonnaScarico).Controls(1), TextBox).Text) Then
                MessaggioErrore &= "Indicare la data di scarico del rifiuto: " & Rifiuto & "<br>"
            End If

            If CType(GridViewRifiuti.Rows(i).Cells(ColonnaUdM).Controls(1), DropDownList).SelectedValue = "0" Then
                MessaggioErrore &= "Indicare l'unità di misura del rifiuto: " & Rifiuto & "<br>"
            End If
            If Not IsNumeric(CType(GridViewRifiuti.Rows(i).Cells(ColonnaQta).Controls(1), TextBox).Text) Then
                MessaggioErrore &= "Indicare la quantità del rifiuto: " & Rifiuto & "<br>"
            End If

        Next



        If MessaggioErrore <> "" Then
            Messaggi.AgroMsgBox(MessaggioErrore, Page, , upDati)
            Exit Sub
        End If

        Dim Id_Agenda As Integer = 0

        '----------------------------------------------------------------
        'salvataggio
        Try

            '-----------------------------------------------------
            '----------- CONNESSIONE E TRANSAZIONE ---------------
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)
            '-----------------------------------------------------

            Dim Agenda As Operazione_Agenda = CreaOggettoAgenda()

            If IsNothing(Agenda) Then
                Throw New Exception(Resources.AgronicaAgenda_2010.NonÈStatoPossibileCreareLOperazione)
            End If

            Dim objAgendaScrivi As New Agenda_Operazione_Helper


            If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                Dim CancellataOperazione As Boolean = False
                CancellataOperazione = objAgendaScrivi.Cancella(objParametriAgenda.Piva,
                                                                 0,
                                                                 objParametriAgenda.Id_Agenda,
                                                                 False,
                                                                 objParametri_Server, logCancellazione:=False)
            End If

            Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)

            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)

        Catch ex As Exception

            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)

            Messaggi.AgroMsgBox(Resources.AgronicaAgenda_2010.AttenzioneLOperazioneNonÈStataRegistrataBr & ex.Message, Page, , upDati)

        Finally

            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

        End Try

        If Id_Agenda <> 0 Then
            AAA_GestioneUscitaPagina()
        End If


    End Sub

    Private Function CreaOggettoAgenda() As Operazione_Agenda

        'data = primo scarico
        Dim Data As Date = AGRODATAFINE

        For i = 0 To GridViewRifiuti.Rows.Count - 1
            If Data > CDate(CType(GridViewRifiuti.Rows(i).Cells(ColonnaScarico).Controls(1), TextBox).Text) Then
                Data = CDate(CType(GridViewRifiuti.Rows(i).Cells(ColonnaScarico).Controls(1), TextBox).Text)
            End If
        Next


        Dim Lav_Cod As String = LAVCOD_GESTIONE_RIFIUTI

        If GridViewRifiuti.Rows.Count = 0 Then
            Messaggi.AgroMsgBox("Selezionare almeno un rifiuto", Page, , upDati)
            Return Nothing
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
        Dim Agenda As Operazione_Agenda
        Dim Movimento As Movimento
        Dim Movimento_Dettaglio As Movimento_Dettaglio


        Agenda = New Operazione_Agenda

        Agenda.Tipo_Operazione = objParametriAgenda.Tipo_Operazione
        Agenda.Id_Agenda = objParametriAgenda.Id_Agenda
        Agenda.Data = Data
        Agenda.Piva = objParametriAgenda.Piva
        Agenda.Sa_Cod = 0
        Agenda.Lav_Cod = Lav_Cod
        Agenda.Des_Lib = "Gestione Rifiuti"
        Agenda.BaseCode = BaseCode
        Agenda.TopCode = TopCode

        '------------------------------------------------
        '----- MOVIMENTO SMALTITORE
        '------------------------------------------------

        Agenda.Movimenti = New List(Of Movimento)

        Movimento = New Movimento

        Movimento.Id_Agenda = objParametriAgenda.Id_Agenda
        Movimento.Piva = Agenda.Piva
        Movimento.Sa_Cod = Agenda.Sa_Cod
        Movimento.Data = Data
        Movimento.Lav_Cod = Lav_Cod
        Movimento.Cau_Mov = CAU_REGISTRAZIONI
        Movimento.Mov_Desc = "Smaltimento Rifiuti"

        Movimento.Cod_Risum = 0 'CInt(Me.Cmb_Smaltitore.SelectedValue.Split("|")(0))
        Movimento.Extra_Str = Txt_Smaltitore.Text
        Movimento.Doc_Numero_Sin = Txt_Convenzione.Text
        Movimento.Doc_Numero_Des = Txt_DocConvenzione.Text

        Movimento.BaseCode = BaseCode
        Movimento.TopCode = TopCode

        Agenda.Movimenti.Add(Movimento)

        '------------------------------------------------
        '----- MOVIMENTI RIFIUTI
        '------------------------------------------------

        For i = 0 To GridViewRifiuti.Rows.Count - 1

            Movimento = New Movimento

            Movimento.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento.Piva = Agenda.Piva
            Movimento.Sa_Cod = Agenda.Sa_Cod
            Movimento.Data = CDate(CType(GridViewRifiuti.Rows(i).Cells(ColonnaScarico).Controls(1), TextBox).Text)
            Movimento.Lav_Cod = Lav_Cod
            Movimento.Cau_Mov = CAU_SCARICO
            If GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("CmbRifiuti").Visible = True Then
                Movimento.Mov_Desc = "Scarico " & CType(GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).Controls(1), DropDownList).SelectedItem.Text
            Else
                Movimento.Mov_Desc = "Scarico " & CType(GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("TxtRifiuti"), TextBox).Text
            End If

            Movimento.Extra_Date = CDate(CType(GridViewRifiuti.Rows(i).Cells(ColonnaCarico).Controls(1), TextBox).Text)

            Movimento.BaseCode = BaseCode
            Movimento.TopCode = TopCode

            Movimento.Movimenti_Dettagli = New List(Of Movimento_Dettaglio)

            Movimento_Dettaglio = New Movimento_Dettaglio

            Movimento_Dettaglio.Id_Agenda = objParametriAgenda.Id_Agenda
            Movimento_Dettaglio.Piva = Agenda.Piva
            Movimento_Dettaglio.Sa_Cod = Agenda.Sa_Cod
            Movimento_Dettaglio.Data = CDate(CType(GridViewRifiuti.Rows(i).Cells(ColonnaScarico).Controls(1), TextBox).Text)
            Movimento_Dettaglio.Lav_Cod = Lav_Cod
            Movimento_Dettaglio.Cau_Mov = CAU_SCARICO

            Movimento_Dettaglio.Elem_Cod = RIFIUTI

            If GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("CmbRifiuti").Visible = True Then
                Movimento_Dettaglio.Pro_Cod = CType(GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("CmbRifiuti"), DropDownList).SelectedValue
                Movimento_Dettaglio.Extra_Str = ""
            Else
                Movimento_Dettaglio.Pro_Cod = 0
                Movimento_Dettaglio.Extra_Str = CType(GridViewRifiuti.Rows(i).Cells(ColonnaRifiuto).FindControl("TxtRifiuti"), TextBox).Text
            End If

            Movimento_Dettaglio.Mat_Cod = 0

            Movimento_Dettaglio.Udm_Cod = CType(GridViewRifiuti.Rows(i).Cells(ColonnaUdM).Controls(1), DropDownList).SelectedValue

            Dim Qta As String = CType(GridViewRifiuti.Rows(i).Cells(ColonnaQta).Controls(1), TextBox).Text
            Qta = Replace(Qta, ".", ",")

            Movimento_Dettaglio.Qta = CDec(Qta)

            Movimento_Dettaglio.Contabilizzato = NONCONTABILE

            Movimento_Dettaglio.BaseCode = BaseCode
            Movimento_Dettaglio.TopCode = TopCode

            Movimento.Movimenti_Dettagli.Add(Movimento_Dettaglio)

            Agenda.Movimenti.Add(Movimento)

        Next

        Return Agenda

    End Function

    Private Sub Ripristina_Dati_nei_Controlli()

        Dim i As Integer = 0
        Dim j As Integer = 0

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
            objParametriAgenda.Data = Agenda.Data

            Txt_Data.Text = Agenda.Data.ToShortDateString

            objParametriAgenda.salva()

            'MOVIMENTI
            If Not IsNothing(Agenda.Movimenti) Then

                For i = 0 To Agenda.Movimenti.Count - 1

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case CAU_SCARICO 'rifiuti

                            'MOVIMENTI_DETTAGLI
                            If Not IsNothing(Agenda.Movimenti(i).Movimenti_Dettagli) Then

                                For j = 0 To Agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                                    Inserisci_Rifiuto(Agenda.Movimenti(i).Movimenti_Dettagli(j).Pro_Cod, _
                                                      Agenda.Movimenti(i).Movimenti_Dettagli(j).Extra_Str, _
                                                      Agenda.Movimenti(i).Extra_Date, _
                                                      Agenda.Movimenti(i).Data, _
                                                      Agenda.Movimenti(i).Movimenti_Dettagli(j).Udm_Cod, _
                                                      Agenda.Movimenti(i).Movimenti_Dettagli(j).Qta, _
                                                      "")


                                Next

                            End If

                        Case CAU_REGISTRAZIONI

                            'Cmb_Smaltitore.SelectedIndex = _
                            '  Cmb_Smaltitore.Items.IndexOf( _
                            '      Cmb_Smaltitore.Items.FindByValue(Agenda.Movimenti(i).Cod_Risum))
                            Txt_Smaltitore.Text = Agenda.Movimenti(i).Extra_Str
                            Txt_Convenzione.Text = Agenda.Movimenti(i).Doc_Numero_Sin
                            Txt_DocConvenzione.Text = Agenda.Movimenti(i).Doc_Numero_Des

                    End Select
                Next

            End If

        End If


    End Sub

    Private Sub GridViewRifiuti_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridViewRifiuti.RowCommand

        Dim IndiceRigaGriglia As Integer = Convert.ToInt32(e.CommandArgument)
        Dim Contatore As Integer = GridViewRifiuti.DataKeys(IndiceRigaGriglia).Item(0)

        'salvo ciò che eventualmente è gia stato inserito
        Salva_Dati()

        Dim Dt As New DataTable
        Dim Dr As DataRow

        If Session("DT_Rifiuti") IsNot Nothing Then

            Dt = Session("DT_Rifiuti")

            Select Case e.CommandName

                Case "Elimina"

                    Dr = Dt.Rows.Find(Contatore)

                    Dr.Delete()

                    Session("DT_Rifiuti") = Dt

                    AggiornaGriglia_Rifiuti()

            End Select

        End If
       

    End Sub

End Class