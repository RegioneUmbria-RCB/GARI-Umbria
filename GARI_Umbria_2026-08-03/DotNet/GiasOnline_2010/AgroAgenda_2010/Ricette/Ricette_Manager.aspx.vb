'Imports System.Web
'Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreGestioneRichieste
'Imports AgronicaControlli_2010
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreXML.XML_Stampe

Public Class Ricette_Manager
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Dim objParametriAgenda As ParametriAgenda

    Dim Qs_Origine As String
    Dim Qs_Piva As String

    '################################################################################################################
    Private Sub Ricette_Manager_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto

    End Sub


    '################################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        'se ho aperto la pagina dal menu agenda non ho aperto un popup e quindi devo tornare al menu agenda,
        If Qs_Origine <> "" Then
            Response.Redirect(Qs_Origine & "?p=" & Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server))
        Else
            Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine
            paginaOnLineRitorno = enum_PagineGiasOnline.MenuCartellaAziendale
            Response.Redirect(CType(Master, Agenda).TrovaRedirectCorretto(True, paginaOnLineRitorno, objParametriAgenda))
        End If

        Page.Master.FindControl("Form1").Controls.Add( _
            New LiteralControl( _
                "<script language='javascript'>window.close();</script>"))

    End Sub

    '################################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If


        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---
        objParametriAgenda = New ParametriAgenda


        Dim vDal As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim Sostituzioni As String
        Sostituzioni = vDal.Leggi_Valore(6, "Nomenclatura_Ricette", "", "", objParametri_Server)

        AgronicaControlli_2010.UI_ControlsHelper.RinominaControlli(Sostituzioni, Me, Nothing)

        If Not IsNothing(Request.QueryString("origine")) Then
            Qs_Origine = Stringa_Decodifica(Request.QueryString("origine").ToString, _
                                                AgroKey_EncoderDecoder, _
                                                Server)
        Else
            Qs_Origine = ""
        End If


        If Not IsNothing(Request.QueryString("p")) Then
            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
                                                AgroKey_EncoderDecoder, _
                                                Server)
        Else
            Qs_Piva = ""
        End If

        InizializzaVarie()

        If Not Page.IsPostBack Then
            'output.Write("Page has just been loaded")

            txt_ValiditaInizio.Text = "01/01/" & Today.Year.ToString
            txt_ValiditaFine.Text = "31/12/" & Today.Year.ToString

            Crea_Dt_Ricette()

            CaricaCombo_Specie()

        Else
            'output.Write("Postback has occured")

            Exit Sub

        End If



    End Sub


    Private Sub InizializzaVarie()

        Dim script As New StringBuilder

        script.Length = 0

        script.AppendLine("$(document).ready(function () { ")

        script.AppendLine("    $('#" & Cmb_Specie.ClientID & "').combobox();")

        script.AppendLine(" $('.bottone').button(); ")

        script.AppendLine("     $('#" + txt_ValiditaInizio.ClientID + "').datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });")
        script.AppendLine("     $('#" + txt_ValiditaFine.ClientID + "').datepicker({ dateFormat: 'dd/mm/yy', changeYear: true, changeMonth: true });")
        script.AppendLine("     $.datepicker.regional['it']; ")

        script.AppendLine("}); ")

        ScriptManager.RegisterStartupScript(Page, Page.GetType(),
                                        String.Format("jQuery_{0}", Page.ClientID), script.ToString, True)


    End Sub



    '################################################################################################################
    Private Sub Crea_Dt_Ricette()

        Dim Icona_Agenda As String = "<img src='../AB_Immagini/icone16/OperazioneAgenda16.ico' border='0'/>"
        Dim Icona_Stampa As String = "<img  src='../AB_Immagini/icone16/Stampa16b.ico' border='0'/>"
        Dim Testo As String

        Dim i, j As Integer

        Dim Dt As New DataTable
        Dim Dt_Ricette As New DataTable
        Dim Dt_RicettexAgenda As New DataTable
        Dim Dt_RicetteOperazioni As New DataTable
        Dim Dr As DataRow

        Dim Piva As String
        Dim Filtro As String = ""
        Dim strId_Agenda As String = ""
        Dim DataInizio As Date = AGRODATAINIZIO
        Dim DataFine As Date = AGRODATAFINE
        Dim Veg_Cod As Integer = 0

        '----- Definisco la struttura del DataTable
        Dt.Columns.Add(New DataColumn("Ricetta_Cod", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("Ricetta_Numero", GetType(String)))
        Dt.Columns.Add(New DataColumn("Ricetta_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Inizio", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita_Fine", GetType(String)))
        Dt.Columns.Add(New DataColumn("Data_Ultima_Operazione", GetType(String)))
        Dt.Columns.Add(New DataColumn("strId_Agenda", GetType(String)))
        Dt.Columns.Add(New DataColumn("tipo_ricetta", GetType(Integer)))
        Dt.Columns.Add(New DataColumn("agenda", GetType(String)))
        Dt.Columns.Add(New DataColumn("stampa_ricaz", GetType(String)))
        Dt.Columns.Add(New DataColumn("stampa_cert", GetType(String)))

        'Creo gli oggetti COM+
        Dim objRicette As New AgronicaCoreContabDAL.Ricette_R
        Dim objRicettexAgenda As New AgronicaCoreContabDAL.RicettexAgenda_R
        Dim objRicetteOperazioni As New AgronicaCoreContabDAL.Ricette_Operazioni_R

        Select Case Me.Chk_VisualizzaPubbliche.Checked
            Case True
                If objParametriAgenda.Piva <> "" Then
                    Filtro = " (Piva = '' OR Piva='" & Agro_SQL_SaveText(objParametriAgenda.Piva) & "')"
                Else
                    Filtro = " Piva = '' "
                End If
            Case False
                If objParametriAgenda.Piva <> "" Then
                    Filtro = " (Piva='" & Agro_SQL_SaveText(objParametriAgenda.Piva) & "')"
                End If
        End Select

        If Txt_Filtro_Ricette.Text <> "" Then
            Filtro &= " AND Ricetta_Des LIKE '%" & Agro_SQL_SaveText(Me.Txt_Filtro_Ricette.Text) & "%'"
        End If

        If txt_ValiditaInizio.Text <> "" AndAlso IsDate(txt_ValiditaInizio.Text) = True Then
            DataInizio = CDate(txt_ValiditaInizio.Text)
        End If
        If txt_ValiditaFine.Text <> "" AndAlso IsDate(txt_ValiditaFine.Text) = True Then
            DataFine = CDate(txt_ValiditaFine.Text)
        End If

        If IsNumeric(Cmb_Specie.SelectedValue) Then
            Veg_Cod = Cmb_Specie.SelectedValue
        End If

        Dt_Ricette = objRicette.Leggi(0, _
                        "", _
                        0, _
                        0, _
                        Veg_Cod, _
                        DataInizio, _
                        DataFine, _
                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                        Filtro, _
                        "", _
                        objParametri_Server)


        objRicette = Nothing

        For i = 0 To Dt_Ricette.Rows.Count - 1

            Dr = Dt.NewRow

            Dr.Item("Ricetta_Cod") = Dt_Ricette.Rows(i).Item("Ricetta_Cod")
            Dr.Item("Ricetta_Numero") = Dt_Ricette.Rows(i).Item("Ricetta_Numero")
            Dr.Item("Ricetta_Des") = Dt_Ricette.Rows(i).Item("Ricetta_Des")

            If CDate(Dt_Ricette.Rows(i).Item("Validita_Inizio")).ToShortDateString = "01/01/1900" Then
                Dr.Item("Validita_Inizio") = "..."
            Else
                Dr.Item("Validita_Inizio") = CDate(Dt_Ricette.Rows(i).Item("Validita_Inizio")).ToShortDateString
            End If
            If CDate(Dt_Ricette.Rows(i).Item("Validita_Fine")).ToShortDateString = "31/12/2100" Then
                Dr.Item("Validita_Fine") = "..."
            Else
                Dr.Item("Validita_Fine") = CDate(Dt_Ricette.Rows(i).Item("Validita_Fine")).ToShortDateString
            End If

            Dt_RicetteOperazioni = objRicetteOperazioni.Leggi(Dt_Ricette.Rows(i).Item("Ricetta_Cod"), _
                                                      0, _
                                                      0, _
                                                      0, _
                                                      AGRODATAINIZIO, _
                                                      AGRODATAFINE, _
                                                      AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                                      "", _
                                                      " validita_inizio desc ", _
                                                      objParametri_Server)

            If Not Dt_RicetteOperazioni Is Nothing AndAlso Dt_RicetteOperazioni.Rows.Count > 0 Then

                If CDate(Dt_RicetteOperazioni.Rows(0).Item("Validita_Inizio")).ToShortDateString = "01/01/1900" Then
                    Dr.Item("Data_Ultima_Operazione") = "..."
                Else
                    Dr.Item("Data_Ultima_Operazione") = CDate(Dt_RicetteOperazioni.Rows(0).Item("Validita_Inizio")).ToShortDateString
                End If

            End If

            strId_Agenda = ""

            Dt_RicettexAgenda = objRicettexAgenda.Leggi(Dt_Ricette.Rows(i).Item("Ricetta_Cod"), _
                                                        0, _
                                                        0, _
                                                        AGRODATAINIZIO, _
                                                        AGRODATAFINE, _
                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                        "", _
                                                        "", _
                                                        objParametri_Server)

            If Not Dt_RicettexAgenda Is Nothing AndAlso Dt_RicettexAgenda.Rows.Count > 0 Then

                For j = 0 To Dt_RicettexAgenda.Rows.Count - 1
                    strId_Agenda &= Dt_RicettexAgenda.Rows(j).Item("id_agenda") & ","
                Next

            End If

            If strId_Agenda <> "" Then
                strId_Agenda = Left(strId_Agenda, strId_Agenda.Length - 1)
            End If

            Dr.Item("strId_Agenda") = strId_Agenda

            Dr.Item("tipo_ricetta") = Dt_Ricette.Rows(i).Item("tipo_ricetta")

            Dr.Item("stampa_cert") = "<a href=""javascript:void(0)"" onclick=""stampa('" & Stringa_Codifica(CStr(Dr.Item("Ricetta_Cod")), AgroKey_EncoderDecoder, Server) & "','" & Stringa_Codifica(CStr(1), AgroKey_EncoderDecoder, Server) & "')"">" & _
                                  Icona_Stampa & _
                                  "</a>"

            Select Case Dr.Item("tipo_ricetta")
                Case TipiEnumerativi.enum_TipoRicetta.Standard
                    Testo = "<a href='Ricette_Edit.aspx" & _
                                         "?r=" & Stringa_Codifica(Dt_Ricette.Rows(i).Item("Ricetta_Cod"), AgroKey_EncoderDecoder, Server) & _
                                          "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Trasferimento, AgroKey_EncoderDecoder, Server) & _
                                          "&destinazione=" & Stringa_Codifica("Ricette_Manager.aspx", AgroKey_EncoderDecoder, Server) & _
                                          "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) & _
                                          "&data_inizio=" & Stringa_Codifica(Dr.Item("Validita_Inizio"), AgroKey_EncoderDecoder, Server) & _
                                          "&data_fine=" & Stringa_Codifica(Dr.Item("Validita_Fine"), AgroKey_EncoderDecoder, Server) & _
                                          "&tipo_ricetta=" & Stringa_Codifica(Dr.Item("tipo_ricetta"), AgroKey_EncoderDecoder, Server) & _
                                          "' " & _
                                          "target='_parent'>" & _
                                          Icona_Agenda & _
                                          "</a>"
                    Dr.Item("stampa_ricaz") = "<a href=""javascript:void(0)"" onclick=""stampa('" & Stringa_Codifica(CStr(Dr.Item("Ricetta_Cod")), AgroKey_EncoderDecoder, Server) & "','" & Stringa_Codifica(CStr(2), AgroKey_EncoderDecoder, Server) & "')"">" & _
                                          Icona_Stampa & _
                                          "</a>"
                Case TipiEnumerativi.enum_TipoRicetta.PianoDistribuzioneConcimi
                    'Dr.Item("stampa_ricaz") = "---"
                    Testo = "---"

                    'Dr.Item("stampa_ricaz") = "<a href='<script language='javascript'>" & _
                    '        "window.open('Stampa/Ricetta_Stampa.aspx" & "?ricetta_cod=" & Stringa_Codifica(CStr(Dr.Item("Ricetta_Cod")), AgroKey_EncoderDecoder, Server) & _
                    '        "&ricetta_stampa_tipo=" & Stringa_Codifica(CStr(2), AgroKey_EncoderDecoder, Server) & _
                    '        "','Ricetta','height=700,width=1000,scrollbars=yes,top=0,left=0,scrollbars=yes,resizable=yes')" & _
                    '        "</script>'>" & _
                    '                      Icona_Stampa & _
                    '                      "</a>"
                    Dr.Item("stampa_ricaz") = "---"

                    'Dr.Item("stampa_cert") = "<a href='<script language='javascript'>" & _
                    '        "window.open('Stampa/Ricetta_Stampa.aspx" & "?ricetta_cod=" & Stringa_Codifica(CStr(Dr.Item("Ricetta_Cod")), AgroKey_EncoderDecoder, Server) & _
                    '        "&ricetta_stampa_tipo=" & Stringa_Codifica(CStr(1), AgroKey_EncoderDecoder, Server) & _
                    '        "','Ricetta','height=700,width=1000,scrollbars=yes,top=0,left=0,scrollbars=yes,resizable=yes')" & _
                    '        "</script>'>" & _
                    '                      Icona_Stampa & _
                    '                      "</a>"

                Case Else
                    Testo = "---"
                    Dr.Item("stampa_ricaz") = "<a href=""javascript:void(0)"" onclick=""stampa('" & Stringa_Codifica(CStr(Dr.Item("Ricetta_Cod")), AgroKey_EncoderDecoder, Server) & "','" & Stringa_Codifica(CStr(2), AgroKey_EncoderDecoder, Server) & "')"">" & _
                                          Icona_Stampa & _
                                          "</a>"
            End Select


            Dr.Item("agenda") = Testo

            Dt.Rows.Add(Dr)

        Next

        objRicettexAgenda = Nothing

        GridView_Ricette.DataSource = Dt
        GridView_Ricette.DataBind()


    End Sub


    Private Sub GridView_Ricette_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView_Ricette.RowCommand

        Dim Destinazione As String
        Dim IndiceRigaGriglia As Integer
        Dim Ricetta_Cod As Integer
        Dim strId_Agenda As String
        Dim Tipo_Ricetta As Integer


        IndiceRigaGriglia = Convert.ToInt32(e.CommandArgument)

        Ricetta_Cod = GridView_Ricette.Rows(IndiceRigaGriglia).Cells(0).Text()
        strId_Agenda = GridView_Ricette.Rows(IndiceRigaGriglia).Cells(13).Text()
        Tipo_Ricetta = GridView_Ricette.Rows(IndiceRigaGriglia).Cells(14).Text()

        Select Case e.CommandName

            Case "Info"

                Destinazione = "Ricette_Edit.aspx?r=" & Stringa_Codifica(Ricetta_Cod, AgroKey_EncoderDecoder, Server) & _
                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Lettura, AgroKey_EncoderDecoder, Server) & _
                                "&destinazione=" & Stringa_Codifica("Ricette_Manager.aspx", AgroKey_EncoderDecoder, Server) & _
                                "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) & _
                                "&data_inizio=" & Stringa_Codifica(GridView_Ricette.Rows(IndiceRigaGriglia).Cells(3).Text, AgroKey_EncoderDecoder, Server) & _
                                "&data_fine=" & Stringa_Codifica(GridView_Ricette.Rows(IndiceRigaGriglia).Cells(4).Text, AgroKey_EncoderDecoder, Server) & _
                                "&tipo_ricetta=" & Stringa_Codifica(Tipo_Ricetta, AgroKey_EncoderDecoder, Server)

                Response.Redirect(Destinazione)

            Case "Modifica"

                Destinazione = "Ricette_Edit.aspx?r=" & Stringa_Codifica(Ricetta_Cod, AgroKey_EncoderDecoder, Server) & _
                    "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Modifica, AgroKey_EncoderDecoder, Server) & _
                    "&destinazione=" & Stringa_Codifica("Ricette_Manager.aspx", AgroKey_EncoderDecoder, Server) & _
                    "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) & _
                    "&data_inizio=" & Stringa_Codifica(GridView_Ricette.Rows(IndiceRigaGriglia).Cells(3).Text, AgroKey_EncoderDecoder, Server) & _
                    "&data_fine=" & Stringa_Codifica(GridView_Ricette.Rows(IndiceRigaGriglia).Cells(4).Text, AgroKey_EncoderDecoder, Server) & _
                    "&tipo_ricetta=" & Stringa_Codifica(Tipo_Ricetta, AgroKey_EncoderDecoder, Server)


                If strId_Agenda = "&nbsp;" Then


                    Response.Redirect(Destinazione)

                Else

                    'se la ricetta è collegata ad opersazioni verifico se ho l'impostazione superuser settata
                    Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                    Dim Dt_Impostazioni_Super As New DataTable
                    Dt_Impostazioni_Super = ObjUtenti.Leggi(enum_Impostazioni_Utenti.SUPERUSER_COD_PERMETTI_MODIFICA_RICETTE_CON_OPERAZIONI_REGISTRATE, _
                                                2, _
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                                "", _
                                                "", _
                                                HttpContext.Current.Session("ASG_objParametri_Utenti"))

                    If Dt_Impostazioni_Super.Rows.Count > 0 Then
                        Select Case Dt_Impostazioni_Super.Rows(0).Item("Impostazione_Valore_1")
                            Case "0"

                            Case "1"

                                'c'è l'impostazione per permettere la modifica delle ricette con operazioni registrate
                                Response.Redirect(Destinazione & "&usata=1")

                            Case Else

                        End Select
                    End If


                    messaggi.AgroMsgBox("Impossibile modificare la Ricetta selezionata poichè le sono associate registrazioni d'agenda!", Page)
                    Destinazione = ""
                    Exit Sub

                End If

            Case "Cancella"

                If strId_Agenda = "&nbsp;" Then


                    Destinazione = "Ricette_Edit.aspx?r=" & Stringa_Codifica(Ricetta_Cod, AgroKey_EncoderDecoder, Server) & _
                                    "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Cancellazione, AgroKey_EncoderDecoder, Server) & _
                                    "&destinazione=" & Stringa_Codifica("Ricette_Manager.aspx", AgroKey_EncoderDecoder, Server) & _
                                    "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) & _
                                    "&data_inizio=" & Stringa_Codifica(GridView_Ricette.Rows(IndiceRigaGriglia).Cells(3).Text, AgroKey_EncoderDecoder, Server) & _
                                    "&data_fine=" & Stringa_Codifica(GridView_Ricette.Rows(IndiceRigaGriglia).Cells(4).Text, AgroKey_EncoderDecoder, Server) & _
                                "&tipo_ricetta=" & Stringa_Codifica(Tipo_Ricetta, AgroKey_EncoderDecoder, Server)

                    Response.Redirect(Destinazione)

                Else

                    Messaggi.AgroMsgBox("Impossibile modificare la Ricetta selezionata poichè le sono associate registrazioni d'agenda!", Page)
                    Exit Sub

                End If

            Case "Stampa"

                Dim strOpen As String
                Dim QueryStringParametri As String

                QueryStringParametri = "?ricetta_cod=" & Stringa_Codifica(CStr(Ricetta_Cod), AgroKey_EncoderDecoder, Server) & "&ricetta_stampa_tipo=" & Stringa_Codifica(CStr(1), AgroKey_EncoderDecoder, Server)

                strOpen = "<script language='javascript'>" & vbNewLine & _
                        "window.open('Stampa/Ricetta_Stampa.aspx" & QueryStringParametri & _
                        "','Ricetta','height=700,width=1000,scrollbars=yes,top=0,left=0,scrollbars=yes,resizable=yes')" & _
                        "</script>"

                'apro la finestra...
                Page.Master.FindControl("Form1").Controls.Add(New LiteralControl(strOpen))


            Case "Stampa_RicAz"

                Dim strOpen As String
                Dim QueryStringParametri As String

                QueryStringParametri = "?ricetta_cod=" & Stringa_Codifica(CStr(Ricetta_Cod), AgroKey_EncoderDecoder, Server) & "&ricetta_stampa_tipo=" & Stringa_Codifica(CStr(2), AgroKey_EncoderDecoder, Server)


                strOpen = "<script language='javascript'>" & vbNewLine & _
                        "window.open('Stampa/Ricetta_Stampa.aspx" & QueryStringParametri & _
                        "','Ricetta','height=700,width=1000,scrollbars=yes,top=0,left=0,scrollbars=yes,resizable=yes')" & _
                        "</script>"

                'apro la finestra...
                Page.Master.FindControl("Form1").Controls.Add(New LiteralControl(strOpen))


            Case "Copia"

                Destinazione = "Ricette_Edit.aspx?r=" & Stringa_Codifica(Ricetta_Cod, AgroKey_EncoderDecoder, Server) & _
                                "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Copia, AgroKey_EncoderDecoder, Server) & _
                                "&destinazione=" & Stringa_Codifica("Ricette_Manager.aspx", AgroKey_EncoderDecoder, Server) & _
                                "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) & _
                                "&data_inizio=" & Stringa_Codifica(GridView_Ricette.Rows(IndiceRigaGriglia).Cells(3).Text, AgroKey_EncoderDecoder, Server) & _
                                "&data_fine=" & Stringa_Codifica(GridView_Ricette.Rows(IndiceRigaGriglia).Cells(4).Text, AgroKey_EncoderDecoder, Server) & _
                                "&tipo_ricetta=" & Stringa_Codifica(Tipo_Ricetta, AgroKey_EncoderDecoder, Server)

                Response.Redirect(Destinazione)

            Case "CreaOperazioni"

                Destinazione = "Ricette_Edit.aspx?r=" & Stringa_Codifica(Ricetta_Cod, AgroKey_EncoderDecoder, Server) & _
                            "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Trasferimento, AgroKey_EncoderDecoder, Server) & _
                            "&destinazione=" & Stringa_Codifica("Ricette_Manager.aspx", AgroKey_EncoderDecoder, Server) & _
                            "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) & _
                            "&data_inizio=" & Stringa_Codifica(GridView_Ricette.Rows(IndiceRigaGriglia).Cells(3).Text, AgroKey_EncoderDecoder, Server) & _
                            "&data_fine=" & Stringa_Codifica(GridView_Ricette.Rows(IndiceRigaGriglia).Cells(4).Text, AgroKey_EncoderDecoder, Server) & _
                                "&tipo_ricetta=" & Stringa_Codifica(Tipo_Ricetta, AgroKey_EncoderDecoder, Server)

                Response.Redirect(Destinazione)


        End Select

    End Sub


    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Protected Sub Btn_Cerca_Ricette_Click(sender As Object, e As EventArgs) Handles Btn_Cerca_Ricette.Click
        Crea_Dt_Ricette()
    End Sub

    Protected Sub Btn_NuovaRicetta_Click(sender As Object, e As EventArgs) Handles Btn_NuovaRicetta.Click

        Dim Destinazione As String

        Destinazione = "Ricette_Edit.aspx?r=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) & _
                    "&o=" & Stringa_Codifica(1, AgroKey_EncoderDecoder, Server) & _
                    "&destinazione=" & Stringa_Codifica("Ricette_Manager.aspx", AgroKey_EncoderDecoder, Server) & _
                    "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) & _
                    "&tipo_ricetta=" & Stringa_Codifica(TipiEnumerativi.enum_TipoRicetta.Standard, AgroKey_EncoderDecoder, Server)

        Response.Redirect(Destinazione)

    End Sub

    Protected Sub Btn_NuovaRicettaImpianti_Click(sender As Object, e As EventArgs) Handles Btn_NuovaRicettaImpianti.Click

        Dim Destinazione As String

        Destinazione = "Ricette_Edit.aspx?r=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, Server) & _
                    "&o=" & Stringa_Codifica(1, AgroKey_EncoderDecoder, Server) & _
                    "&destinazione=" & Stringa_Codifica("Ricette_Manager.aspx", AgroKey_EncoderDecoder, Server) & _
                    "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, Server) & _
                    "&tipo_ricetta=" & Stringa_Codifica(TipiEnumerativi.enum_TipoRicetta.Standard_Destinazioni, AgroKey_EncoderDecoder, Server)

        Response.Redirect(Destinazione)

    End Sub

    Private Sub CaricaCombo_Specie()

        Dim objRicette As New AgronicaCoreContabDAL.Ricette_R
        Dim DtRicette As DataTable
        DtRicette = objRicette.Leggi(0, "", 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_JoinDescrizioni, "", " SpecieVegetali.Veg_Des ", objParametri_Server)

        Dim Trovata As Boolean = False

        Cmb_Specie.Items.Clear()
        Cmb_Specie.Items.Add(New ListItem("", ""))

        Dim i, j As Integer
        For i = 0 To DtRicette.Rows.Count - 1
            Trovata = False
            For j = 0 To Cmb_Specie.Items.Count - 1
                If InStr(Cmb_Specie.Items(j).Text, DtRicette.Rows(i).Item("veg_des")) <> 0 Then
                    Trovata = True
                    Exit For
                End If
            Next
            If Trovata = False Then
                Cmb_Specie.Items.Add(New ListItem(DtRicette.Rows(i).Item("veg_des"), DtRicette.Rows(i).Item("veg_cod")))
            End If
        Next
      


    End Sub



    Protected Sub Btn_PianificaAttivita_Click(sender As Object, e As EventArgs) Handles Btn_PianificaAttivita.Click
        Dim Origine As String = "../Ricette/Ricette_Manager.aspx"
        Dim TargetUrl As String = "../Ricette/Ricette_Edit.aspx?r=" & Stringa_Codifica(0, AgroKey_EncoderDecoder, objParametri_Server) & _
    "&o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, objParametri_Server) & _
    "&destinazione=" & Stringa_Codifica(Origine, AgroKey_EncoderDecoder, objParametri_Server) & _
    "&p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder, objParametri_Server) & _
    "&tipo_ricetta=" & Stringa_Codifica(TipiEnumerativi.enum_TipoRicetta.Standard_Destinazioni_Planning, AgroKey_EncoderDecoder, objParametri_Server)

        Response.Redirect(TargetUrl)
    End Sub
End Class