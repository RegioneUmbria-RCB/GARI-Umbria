Public Class LOG_Interferenze
    Inherits System.Web.UI.Page





    '#########################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "attachment; filename = log" & Session.SessionID.ToString & ".xls")


        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        ''----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        'Dim UtenteAbilitato As Boolean

        'UtenteAbilitato = Agro_Check_Permessi( _
        '                        Session("ASG_Utente_Username"), _
        '                        Session("ASG_IdServizio"), _
        '                        enum_Security_Attivita.Interferenze_Visualizzazione_Ridotta, _
        '                        enum_Security_Operazione.Lettura, _
        '                        Server, Session, Page)

        ''----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.

        'If Not UtenteAbilitato Then
        '    Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        'End If


        '##############################################################
        '#####  Recupero i filtri scelti  #############################
        '##############################################################

        'Dim DT As New DataTable

        ''DT = Session("SuperDT_Risultati")


        'Recupero il datatable degli IMPIANTI
        'DT_Impianti = Session("AgroSementi_DT_Impianti")
        'Dim DT_Impianti As DataTable = Session("SuperDT_Impianti")

        'Recupero il datatable delle INTERFERENZE
        'DT_Interferenze = Session("AgroSementi_DT_Interferenze")
        Dim DT_Interferenze As DataTable = Session("SuperDT_Interferenze")

        '##############################################################
        '#####  Costruisco la tabella   ###############################
        '##############################################################

        Dim Riga As HtmlTableRow
        Dim i, j As Integer

        Riga = New HtmlTableRow

        For j = 0 To 4
            Riga.Cells.Add(New HtmlTableCell)
            AgronicaCoreSementieriBIZ.Utility.ElaboraCellaHTML(Riga.Cells(j), 2, "", "", "Gainsboro", "center", "top")
        Next

        'Riga.Cells(0).InnerHtml = "Tipo<br>Interferenza"
        Riga.Cells(0).InnerHtml = "Impianto 1"
        Riga.Cells(1).InnerHtml = "Impianto 2"
        Riga.Cells(2).InnerHtml = "Descrizione"
        Riga.Cells(3).InnerHtml = "Distanze"
        Riga.Cells(4).InnerHtml = "Confermate"

        TableInterferenze.Rows.Add(Riga)

        For i = 0 To DT_Interferenze.Rows.Count - 1

            Riga = New HtmlTableRow

            'Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells.Add(New HtmlTableCell)
            'Riga.Cells(0).InnerHtml = ""
            Riga.Cells(0).InnerHtml = DT_Interferenze.Rows(i).Item("Impianto_A")
            Riga.Cells(1).InnerHtml = DT_Interferenze.Rows(i).Item("Impianto_B")
            Riga.Cells(2).InnerHtml = DT_Interferenze.Rows(i).Item("Descrizione")
            Dim dist As String = ""
            dist += "<b>Distanza : </b>" &
                     "<br>&nbsp;&nbsp;&nbsp;" &
                     DT_Interferenze.Rows(i).Item("Distanza") & " [m] <br /><br /> "
            dist += "<b>Distanza Di Legge : </b>" &
                     "<br>&nbsp;&nbsp;&nbsp;" &
                     DT_Interferenze.Rows(i).Item("Distanza_di_legge") & " [m] <br /><br /> "
            dist += "<b>Distanza Moltiplicata : </b>" &
                    "<br>&nbsp;&nbsp;&nbsp;" &
         DT_Interferenze.Rows(i).Item("Distanza_di_legge_moltiplicata") & " [m]  "

            Riga.Cells(3).InnerHtml = dist

            'ElaboraCellaHTML(Riga.Cells(4), 0, "", "", "", "center", "middle")
            Riga.Cells(4).Style.Item("text-align") = "center"
            Riga.Cells(4).Style.Item("vertical-align") = "middle"
            Riga.Cells(4).Style.Item("font-size") = "x-large"
            Riga.Cells(4).InnerHtml = "[&nbsp;&nbsp;]" '"☐"

            'For j = 0 To 4

            '    'aggiungo la cella
            '    Riga.Cells.Add(New HtmlTableCell)
            '    Riga.Cells(j).InnerHtml = DT.Rows(i).Item(j)

            'Next

            'Aggiungo la Riga alla Tabella 
            Me.TableInterferenze.Rows.Add(Riga)

        Next


    End Sub




End Class