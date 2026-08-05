Public Class WebForm1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Visualizzatore_Mappa.Strxml = XDocument.Load(Server.MapPath(".") & "\testshp.xml").ToString
        Visualizzatore_Mappa.ns = "http://www.opengis.net/gml"
        Visualizzatore_Mappa.Layer_Selezionato = 1
    End Sub

End Class