

Imports System.Web.Services
Imports System.Text
Imports System.Data
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services

Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreMetaSchemaDAL
Imports System.ComponentModel


''' <summary>
''' Per utilizzare questo controllo è necessario includere all'iterno della pagina i seguenti script 
''' jquery.cookie.js
''' jquery.hotkeys.js
''' jquery.jstree.js
''' RICORDARE DI AGGIUNGERE LA FUNZIONE JS
''' function DoPostBack_Combo($_combo,valoreOpt) {
'''        if($_combo.attr("id").endsWith("ComboMagazzini")){
'''            //salvo nel campo nascosto il nome della funzione da richiamare
'''            $("#<%= fooName_PostedBack.ClientID %>").val("comboLavorazioni_IndexChanged");
'''            //scatena il postback simulato il click del btn nascosto
'''            $("#<%=btnDummyPostback.ClientID %>").click();
'''        }
''' </summary>
''' <remarks></remarks>

<DefaultProperty("Text"), ToolboxData("<{0}:ComboFormulati runat=server></{0}:ComboFormulati>")> Public Class ComboPrincipiAttivi
    Inherits System.Web.UI.WebControls.WebControl


    Public ddlPrincipiAttivi As DropDownList



    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddlPrincipiAttivi = New DropDownList
        ddlPrincipiAttivi.ID = Me.ClientID & "ComboPrincipiAttivi"


        ddlPrincipiAttivi.CssClass = "selectpicker ComboFormulati"

        ddlPrincipiAttivi.Attributes.Add("data-live-search", "true")

        ddlPrincipiAttivi.Attributes.Add("data-container", "body")
        ddlPrincipiAttivi.Attributes.Add("onchange", " CambiaPrincipiAttivi(); ")


        Me.Controls.Add(ddlPrincipiAttivi)
        MyBase.OnInit(e)
    End Sub

    Public Sub CaricaComboPrincipiAttivi()


        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(
                                HttpContext.Current.Session("ASG_objParametri_Server"))


        AgronicaCoreUtility.CaricaListControl.PrincipiAttivi(
            ddlPrincipiAttivi,
            False,
            "",
            "",
            "",
            "",
            "",
            objParametri_Server
        )
    End Sub
End Class
