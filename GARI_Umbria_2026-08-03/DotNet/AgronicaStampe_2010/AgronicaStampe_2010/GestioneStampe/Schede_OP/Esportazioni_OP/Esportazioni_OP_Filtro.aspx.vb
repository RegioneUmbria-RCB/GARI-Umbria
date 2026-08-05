Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Esportazioni_OP_Filtro
    Inherits System.Web.UI.Page

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private Qs_Report As Integer

    Const PaginaLinkEsportazioneOP_Catasto = "Esportazione_OP_Catasto.aspx"
    Const PaginaLinkEsportazioneOP_Produttori = "Esportazione_OP_Produttori.aspx"


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Tolgo la pagina dalla cache
        Response.Expires = 0

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '#################################################################################
        '#####  Recupero i dati dalla QueryString 
        '#################################################################################


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Qs_Report = Stringa_Decodifica(Request.QueryString("r").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)

        '=====================================================
        '----- Inizializzo i controlli
        '=====================================================

        If Not Me.IsPostBack Then

            '==========================================
            '===== Pagina caricata per la prima volta
            '==========================================

        Else

            '==========================================
            '===== Pagina ricaricata in POSTBACK
            '==========================================

            'Evito di re-inizializzare i controlli
            Exit Sub


        End If

        'If Not QS_Piva Is Nothing Then
        '    ViewState("piva") = QS_Piva
        'End If

        Me.TxtValiditaInizio.Text = Today.ToShortDateString

        Select Case Qs_Report
            Case enum_CodificaStampe.Esportazione_OP_Catasto
                riga_cod_unione.Visible = False
                riga_istat.Visible = False
                riga_cuaa.Visible = False
            Case enum_CodificaStampe.Esportazione_OP_Produttori
                riga_cod_unione.Visible = True
                riga_istat.Visible = True
                riga_cuaa.Visible = True
        End Select


    End Sub

    Protected Sub Btn_Stampa_Click(sender As Object, e As EventArgs) Handles Btn_Stampa.Click


        If Not IsDate(TxtValiditaInizio.Text) Then

            AgroMsgBox("Selezionare la data di riferimento per la stampa!", Page)

        Else

            Dim TargetURL As String = ""

            Select Case Qs_Report

                Case enum_CodificaStampe.Esportazione_OP_Catasto
                    TargetURL = PaginaLinkEsportazioneOP_Catasto & _
                     "?data=" & Stringa_Codifica(TxtValiditaInizio.Text, AgroKey_EncoderDecoder, Server)
                    
                Case enum_CodificaStampe.Esportazione_OP_Produttori

                    If Txt_CodiceUnione.Text = "" Then
                        AgroMsgBox("Impostare il codice unione!", Page)
                        Exit Sub
                    End If
                    If Txt_Istat.Text = "" Then
                        AgroMsgBox("Impostare l'istat regione!", Page)
                        Exit Sub
                    End If
                    If Txt_CodiceUnione.Text = "" Then
                        AgroMsgBox("Impostare il cuaa!", Page)
                        Exit Sub
                    End If

                    TargetURL = PaginaLinkEsportazioneOP_Produttori & _
                                   "?data=" & Stringa_Codifica(TxtValiditaInizio.Text, AgroKey_EncoderDecoder, Server) & _
                                   "&cod=" & Stringa_Codifica(Txt_CodiceUnione.Text, AgroKey_EncoderDecoder, Server) & _
                                   "&reg=" & Stringa_Codifica(Txt_Istat.Text, AgroKey_EncoderDecoder, Server) & _
                                   "&cuaa=" & Stringa_Codifica(Txt_Cuaa.Text, AgroKey_EncoderDecoder, Server)
            End Select

            If TargetURL <> String.Empty Then
                Response.Redirect(TargetURL)
            End If

        End If

    End Sub



End Class