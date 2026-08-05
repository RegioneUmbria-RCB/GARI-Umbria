Public Class LOG_Variazioni
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'La Pagina deve essere visualizzata come un foglio Excel
        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "attachment; filename = Variazioni_" & Session.SessionID.ToString & ".xls")

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '##############################################################
        '#####  Costruisco la tabella   ###############################
        '##############################################################

        'Recupero il datatable delle VARIAZIONI
        Dim DT_Variazioni As DataTable = Session("LogDT_Variazioni")

        Dim hdrs() As String = {"Data", "Referente", "Operazione", "Indirizzo", "CAP", "Comune", "Prov", "Lat.", "Long.", "Specie", "Tipologia", "Superficie"}

        Dim Riga As HtmlTableRow
        Dim Cella As HtmlTableCell

        Riga = New HtmlTableRow
        Riga.Style.Item("height") = "2.5em"

        For Each hdr In hdrs
            Cella = New HtmlTableCell
            AgronicaCoreSementieriBIZ.Utility.ElaboraCellaHTML(Cella, 2, "", "", "Gainsboro", "center", "middle")
            Cella.InnerHtml = hdr
            Riga.Cells.Add(Cella)
        Next

        TableVariazioni.Rows.Add(Riga)

        For Each row In DT_Variazioni.Rows

            Riga = New HtmlTableRow
            Riga.Style.Item("height") = "2.5em"
            Riga.Style.Item("vertical-align") = "middle"

            For Each col In DT_Variazioni.Columns
                Cella = New HtmlTableCell
                Cella.InnerHtml = row.Item(col)
                Riga.Cells.Add(Cella)
            Next

            'Aggiungo la Riga alla Tabella 
            TableVariazioni.Rows.Add(Riga)

        Next

    End Sub


End Class