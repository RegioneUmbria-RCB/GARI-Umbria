Imports AgronicaCoreVarieDAL

Public Class bootstrap_datepicker
    Inherits System.Web.UI.WebControls.Label

#Region "Methods & Event Handlers"
    Protected Overrides Sub OnPreRender(e As EventArgs)
        Page.ClientScript.RegisterClientScriptInclude("bootstrap-datepicker", Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.bootstrap-datepicker.js"))
        ' Giulia: 19/5/2020: risolto l'errore 404 che non trovava la risorsa (dava fastidio il codice lingua ".it")
        '       ora però dà errore javascript runtime: WebResource.axd:6 Uncaught TypeError: Cannot set property 'it' of undefined perché non trova l'array dates
        '       a sto punto commento l'inclusione tanto non funziona e non ha mai funzionato (almeno dal 2016)
        'Page.ClientScript.RegisterClientScriptInclude("bootstrap-datepicker-it", Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.bootstrap-datepicker._it.js"))
    End Sub
#End Region

End Class
