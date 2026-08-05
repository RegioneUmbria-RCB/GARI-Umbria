Imports System.Web.Services
Imports System.Text
Imports System.Data
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services

Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMetaSchemaDAL
Imports System.ComponentModel


''' <summary>
''' Per utilizzare questo controllo è necessario includere all'iterno della pagina i seguenti script 
''' jquery.cookie.js
''' jquery.hotkeys.js
''' jquery.jstree.js
''' RICORDARE DI AGGIUNGERE LA FUNZIONE JS
''' function DoPostBack_Combo($_combo,valoreOpt) {
'''        if($_combo.attr("id").endsWith("ddlLavorazioni")){
'''            //salvo nel campo nascosto il nome della funzione da richiamare
'''            $("#<%=fooName_PostedBack.ClientID %>").val("comboLavorazioni_IndexChanged");
'''            //scatena il postback simulato il click del btn nascosto
'''            $("#<%=btnDummyPostback.ClientID %>").click();
'''        }
''' </summary>
''' <remarks></remarks>


<DefaultProperty("Text"), ToolboxData("<{0}:ComboImprese runat=server></{0}:ComboImprese>")> Public Class ComboImprese
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboCentroAziendale
    '    Inherits ScriptControl
    '    Implements IScriptControl

    Public ddl_ComboImprese As DropDownList
    Private _Piva As String
    Private _bootstrap As Boolean
    Private GiaCreato As Boolean
    Private _Ricerca As Boolean = True


    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_ComboImprese = New DropDownList
        ddl_ComboImprese.ID = Me.ClientID & "ComboImprese"
        If _bootstrap = False Then
            ddl_ComboImprese.CssClass = "myCombo"
        Else
            ddl_ComboImprese.CssClass = "selectpicker ComboImprese"
            If _Ricerca = True Then
                ddl_ComboImprese.Attributes.Add("data-live-search", "true")
            End If

            ddl_ComboImprese.Attributes.Add("data-container", "body")
        End If

        Me.Controls.Add(ddl_ComboImprese)
        MyBase.OnInit(e)
    End Sub

#Region "Proprietà"

    Public Property Ricerca() As Boolean
        Get
            Return _Ricerca
        End Get
        Set(ByVal value As Boolean)
            _Ricerca = value
        End Set
    End Property


    Public Property bootstrap() As Boolean
        Get
            Return _bootstrap
        End Get
        Set(ByVal value As Boolean)
            _bootstrap = value
        End Set
    End Property

    Public Property Piva() As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property

    Public Property Valore_Combo() As String
        Get
            Return ddl_ComboImprese.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_ComboImprese.Items.FindByValue(value)) Then
                    ddl_ComboImprese.Items.FindByValue(value).Selected = True
                End If
            End If
        End Set
    End Property

#End Region

  




    Public Sub CaricaComboImprese()

        AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(ddl_ComboImprese, True, "", "", _
                                                                     "", " ORDER BY Rag_Soc asc", _
                                                                     HttpContext.Current.Session("ASG_objParametri_Server"), _
                                                                     HttpContext.Current.Session("ASG_objParametri_Utenti"))


    End Sub



#Region "JS"
    Public Function GetJS()
        Dim StrSelect As New StringBuilder

        If bootstrap = False Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" & ddl_ComboImprese.ClientID & "').combobox();")
            StrSelect.AppendLine("});")
        End If

        Return StrSelect.ToString
    End Function
#End Region

End Class
