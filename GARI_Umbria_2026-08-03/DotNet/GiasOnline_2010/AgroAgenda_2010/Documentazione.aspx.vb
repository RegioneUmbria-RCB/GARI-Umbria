Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports Newtonsoft.Json

Public Class Documentazione
    Inherits System.Web.UI.Page

    Dim objParametri_Super_Server As AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreParametri

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        objParametri_Super_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim mst As AgendaBootstrap = CType(Master, AgendaBootstrap)
        mst.flag_MostraBtnIndietro = True
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto

        CreaPannelloManuali()
        CreaPannelloVideoCorsi()

    End Sub

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuBS_2017", "", "", objParametri_Server)
        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
            Response.Redirect("Menu/MenuBS_2017.aspx")
        Else
            Response.Redirect("Menu/MenuBS_Agenda_Nuovo.aspx")
        End If

    End Sub

    Private Sub CreaPannelloManuali()
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim strLinkManualeGiasOnline As String = objConfigSiti.Leggi_Valore(0, "LinkManualeGiasOnline", "", "", objParametri_Server)

        If String.IsNullOrEmpty(strLinkManualeGiasOnline) Then
            strLinkManualeGiasOnline = objConfigSiti.Leggi_Valore(0, "LinkManualeGiasOnline", "", "", objParametri_Super_Server)
        End If

        Dim strLinkManualeUma As String = objConfigSiti.Leggi_Valore(0, "LinkManualeUmaCarburanti", "", "", objParametri_Server)

        Dim strElencoManualiJson As String = objConfigSiti.Leggi_Valore(0, "ElencoManualiJson", "", "", objParametri_Server)

        If strLinkManualeGiasOnline <> "" OrElse strLinkManualeUma <> "" OrElse strElencoManualiJson <> "" Then
            pnlManuali.Style.Add("margin-bottom", "20px")
            pnlManuali.Controls.Add(New HtmlGenericControl("h3") With {.InnerHtml = "Manuali"})
        End If

        Dim primoManuale = True

        If strLinkManualeGiasOnline <> "" Then

            'Grilli 13/05/2019: Aggiungo un parametro random per evitare la cache del browser quando modifico il PDF
            strLinkManualeGiasOnline &= "?rnd=" & Sicurezza.Stringa_Codifica(DateTime.Now.ToString(), AgroKey_EncoderDecoder, Server)
            pnlManuali.Controls.Add(New Label() With {.Text = "Manuale Utente: "})
            Dim strPopup As String = "javascript:window.open('" & strLinkManualeGiasOnline & "','Manuale Utente');"
            pnlManuali.Controls.Add(New HyperLink() With {.Text = "Apri", .NavigateUrl = strPopup})
            primoManuale = False

        End If

        If strLinkManualeUma <> "" Then

            strLinkManualeUma &= "?rnd=" & Sicurezza.Stringa_Codifica(DateTime.Now.ToString(), AgroKey_EncoderDecoder, Server)
            If Not primoManuale Then
                pnlManuali.Controls.Add(New HtmlGenericControl("br"))
            End If
            pnlManuali.Controls.Add(New Label() With {.Text = "Manuale Modulo UMA Carburanti: "})
            Dim strPopup As String = "javascript:window.open('" & strLinkManualeUma & "','Manuale Modulo UMA Carburanti');"
            pnlManuali.Controls.Add(New HyperLink() With {.Text = "Apri", .NavigateUrl = strPopup})
            primoManuale = False

        End If

        If strElencoManualiJson <> "" Then
            Dim elencoManuali = JsonConvert.DeserializeObject(Of List(Of Manuale))(strElencoManualiJson)

            For Each manuale In elencoManuali

                Dim strLinkManuale = manuale.Link & "?rnd=" & Sicurezza.Stringa_Codifica(DateTime.Now.ToString(), AgroKey_EncoderDecoder, Server)

                If Not primoManuale Then
                    pnlManuali.Controls.Add(New HtmlGenericControl("br"))
                End If

                pnlManuali.Controls.Add(New Label() With {.Text = manuale.NomeManuale})

                Dim strPopup As String = "javascript:window.open('" & strLinkManuale & "','" & manuale.NomeManuale & "');"
                Dim strNomeLink = "Apri"
                If manuale.NomeLink <> "" Then
                    strNomeLink = manuale.NomeLink
                End If
                pnlManuali.Controls.Add(New HyperLink() With {.Text = strNomeLink, .NavigateUrl = strPopup})
                primoManuale = False

            Next

        End If

    End Sub

    Private Sub CreaPannelloVideoCorsi()
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim strLinkVideoCorsi As String = objConfigSiti.Leggi_Valore(0, "LinkVideoCorsi", "", "", objParametri_Server)

        If strLinkVideoCorsi <> "" Then
            pnlVideoCorsi.Style.Add("margin-bottom", "20px")
            pnlVideoCorsi.Controls.Add(New HtmlGenericControl("h3") With {.InnerHtml = "Video Corsi"})

            pnlVideoCorsi.Controls.Add(New Label() With {.Text = "Canale Youtube di Agronica: "})
            Dim strPopup As String = "javascript:window.open('" & strLinkVideoCorsi & "','Video Corsi');"
            pnlVideoCorsi.Controls.Add(New HyperLink() With {.Text = "Apri", .NavigateUrl = strPopup})

        End If

        '27/06/2024: Tolta questa parte perché i video sono del vecchio QDC
        'If objParametri_Server.PivaSuperUser = "05644051004" Then
        '    Dim str As String = "<ul style=""margin-bottom:80px;"">" &
        '                        "	<li>Accesso dal Portale del Socio: <a href=""javascript:window.open('https://youtu.be/1hZLxKMNer8','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Il Menù Agenda: <a href=""javascript:window.open('https://youtu.be/iAZcTO1Q_qE','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Piano Colturale: <a href=""javascript:window.open('https://youtu.be/QOFbrKr9VSQ','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Frazionamento Aggregazione appezzamenti: <a href=""javascript:window.open('https://youtu.be/lHlBPk5lACI','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Carico di Magazzino: <a href=""javascript:window.open('https://youtu.be/djFBZfSRbmM','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Ricevimento DDT: <a href=""javascript:window.open('https://youtu.be/1olYRIBjA-U','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Lavorazioni Aratura: <a href=""javascript:window.open('https://youtu.be/9ScyGEWeyrw','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Semina: <a href=""javascript:window.open('https://youtu.be/gW1Bk7EjF5M','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Trattamento antiparassitario: <a href=""javascript:window.open('https://youtu.be/AKJh1Bk_1MQ','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Diserbo: <a href=""javascript:window.open('https://youtu.be/PhoAMRf6fgg','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Distribuzione Concime: <a href=""javascript:window.open('https://youtu.be/T9NFO3I89sQ','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Distribuzione Ammendanti Organici o Liquami: <a href=""javascript:window.open('https://youtu.be/T5DG0ePPzJk','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Fertirrigazione e Concimazione Fogliare: <a href=""javascript:window.open('https://youtu.be/Kf2Q2MmWliE','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Irrigazione: <a href=""javascript:window.open('https://youtu.be/NIqwocVGHgo','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Altre lavorazioni: <a href=""javascript:window.open('https://youtu.be/-IoKETFVJBY','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Verifica Conformità: <a href=""javascript:window.open('https://youtu.be/cxWA1Qdp7zA','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Stampa Registro: <a href=""javascript:window.open('https://youtu.be/YivY3i_yehs','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Stampa Magazzino: <a href=""javascript:window.open('https://youtu.be/iP5OEO6LXQw','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Profitosan: <a href=""javascript:window.open('https://youtu.be/7GU92E5lHGs','Video Corsi');"">Apri</a></li> " &
        '                        "	<li>Profitosan - Scheda Prodotto: <a href=""javascript:window.open('https://youtu.be/FWSXH_RAypU','Video Corsi');"">Apri</a></li> " &
        '                        "</ul>"

        '    pnlVideoCorsi.Controls.Add(New LiteralControl(str))
        'End If
    End Sub

    Private Class Manuale
        Public NomeManuale As String
        Public NomeLink As String
        Public Link As String
    End Class

End Class