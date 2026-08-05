Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Public Class Esporta_GiasToSap_XLS
    Inherits System.Web.UI.Page

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Protected WithEvents TableGiasToSap As System.Web.UI.HtmlControls.HtmlTable

    'oggetto objparametri x server e utenti
    Dim objparametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objparametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
    End Sub

#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Inserire qui il codice utente necessario per inizializzare la pagina
        Dim Dt As DataTable

        Dim QS_Filtro As String
        Dim Qs_TipoFile As String

        Dim objStampe As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        objparametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri
        objparametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean
        'Dim strDummy As String      'controllo accesso negato.....


        UtenteAbilitato = objStampe.Controlla_Permessi_Utente(Session("ASG_Utente_Username"),
                                                              Session("ASG_IdServizio"),
                                                              enum_Security_Attivita.Gest_Stampe,
                                                              enum_Security_Operazione.Modifica,
                                                              Date.Now,
                                                              "",
                                                              objparametri_Utenti)

        'UtenteAbilitato = objStampe.Controlla_Permessi_Utente_2(
        '                            Server, Session, Page,
        '                            Session("ASG_Utente_Username"),
        '                            Session("ASG_IdServizio"),
        '                            enum_Security_Attivita.Gest_Stampe,
        '                            enum_Security_Operazione.Modifica,
        '                            strDummy)

        '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.


        '----- !!!!!!!!!!! -------------

        'Attivazione forzata provvisoria

        UtenteAbilitato = True

        '----- !!!!!!!!!!! -------------


        If Not UtenteAbilitato Then
            Response.Redirect("../../Messaggi/AccessoNegato.htm")
        End If


        '##############################################################
        '#####  Verifico se sono in Post-Back  ########################
        '##############################################################

        'If Not Page.IsPostBack Then
        '    'output.Write("Page has just been loaded")

        '    'la prima volta che carico la pagina la metto in primo piano
        '    '(in caso contrario rimane in primo piano la pagina del GiasOnline)
        '    Dim strFocus As String = "<script language='javascript'> window.focus() </script>"
        '    Dim MioControllo As Control = TryCast(Me.Master.FindControl("MainContent"), Control)
        '    MioControllo.Controls.Add(New LiteralControl(strFocus))
        'Else

        '    'output.Write("Postback has occured")
        '    Exit Sub
        'End If


        '=====================================================
        '----- Recupero i valori dalla querystring
        '=====================================================

        QS_Filtro = Stringa_Decodifica(Request.QueryString("f").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        Qs_TipoFile = Stringa_Decodifica(Request.QueryString("t").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        '0=excel  1=html
        If Qs_TipoFile = "0" Then

            'La Pagina deve essere visualizzata come un foglio Excel
            Response.ContentType = "application/vnd.ms-excel"
            Response.AddHeader("Content-Disposition", "inline; filename = esportagiastosap.xls")

        End If

        '##############################################################
        '#####  Costruisco la tabella   ###############################
        '##############################################################

        Dt = Session("Dt_DatiGias")

        Dim Riga As HtmlTableRow
        Dim i, j As Integer

        Me.TableGiasToSap.Rows(0).Cells(0).InnerHtml = QS_Filtro
        Me.TableGiasToSap.Rows(0).Cells(0).ColSpan = 18

        'Creo la prima riga con l'intestazione
        Riga = New HtmlTableRow

        For j = 0 To 17
            Riga.Cells.Add(New HtmlTableCell)
            AgronicaCoreDataProvider.UtilityProvider.ElaboraCellaHTML(Riga.Cells(j),
                                                                      2, "", "",
                                                                      "Gainsboro",
                                                                      "center", "top")
        Next

        Riga.Cells(0).InnerHtml = "Chiave"
        Riga.Cells(1).InnerHtml = "Piano<br>Semina"
        Riga.Cells(2).InnerHtml = "Codice<br>Varieta'"
        Riga.Cells(3).InnerHtml = "Descrizione<br>Varieta'"
        Riga.Cells(4).InnerHtml = "Partita Iva<br>Cooperativa"
        Riga.Cells(5).InnerHtml = "Ragione Sociale<br>Cooperativa"
        Riga.Cells(6).InnerHtml = "Codice SAP<br>Cooperativa"
        Riga.Cells(7).InnerHtml = "Partita Iva<br>Socio"
        Riga.Cells(8).InnerHtml = "Ragione Sociale<br>Socio"
        Riga.Cells(9).InnerHtml = "Codice SAP<br>Socio"
        Riga.Cells(10).InnerHtml = "Data<br>Semina"
        Riga.Cells(11).InnerHtml = "Sup"
        Riga.Cells(12).InnerHtml = "Quantita'<br>Seme"
        Riga.Cells(13).InnerHtml = "UDM<br>Seme"
        Riga.Cells(14).InnerHtml = "Lotto"
        Riga.Cells(15).InnerHtml = "Centro<br>Aziendale"
        Riga.Cells(16).InnerHtml = "Sup"
        Riga.Cells(17).InnerHtml = "Quantita'<br>Seme"

        Me.TableGiasToSap.Rows.Add(Riga)

        For i = 0 To Dt.Rows.Count - 1

            Riga = New HtmlTableRow

            For j = 0 To Dt.Columns.Count - 1

                'aggiungo la cella
                Riga.Cells.Add(New HtmlTableCell)

                Select Case j

                    Case 11, 12
                        'arrotondo la qtà seminata
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt.Rows(i).Item(j)), Dt.Rows(i).Item(j), "&nbsp;")
                        If Riga.Cells(j).InnerHtml <> "&nbsp;" Then
                            Riga.Cells(j).InnerHtml = Format(CDbl(Riga.Cells(j).InnerHtml), "0.00") & "&nbsp;"
                        End If
                        'Riga.Cells(j).Align = "right"

                    Case 10
                        'stampo la data nel formato short
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt.Rows(i).Item(j)), Dt.Rows(i).Item(j), "&nbsp;")
                        If Riga.Cells(j).InnerHtml <> "&nbsp;" Then
                            Riga.Cells(j).InnerHtml = Format(CDate(Riga.Cells(j).InnerHtml), "ddMMyyyy") & "&nbsp;"
                        End If
                        '        Riga.Cells(j).Align = "center"

                    Case 16, 17
                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt.Rows(i).Item(j)), Dt.Rows(i).Item(j), "&nbsp;")

                    Case Else

                        Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(Dt.Rows(i).Item(j)), Dt.Rows(i).Item(j), "&nbsp;") & "&nbsp;"

                End Select

            Next

            'Aggiungo la Riga alla Tabella 
            Me.TableGiasToSap.Rows.Add(Riga)

        Next
    End Sub

End Class