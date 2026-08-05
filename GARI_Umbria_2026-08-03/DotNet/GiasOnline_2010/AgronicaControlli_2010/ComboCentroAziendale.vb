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


<DefaultProperty("Text"), ToolboxData("<{0}:ComboCentroAziendale runat=server></{0}:ComboCentroAziendale>")> Public Class ComboCentroAziendale
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboCentroAziendale
    '    Inherits ScriptControl
    '    Implements IScriptControl

    Public ddl_CentroAziendale As DropDownList
    Private _Piva As String
    Private _bootstrap As Boolean
    Private GiaCreato As Boolean
    Private _Ricerca As Boolean = True

    Public Sub New()
        _bootstrap = False
    End Sub

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        ddl_CentroAziendale = New DropDownList
        ddl_CentroAziendale.ID = Me.ClientID & "ComboCentroAziendale"
        If _bootstrap = False Then
            ddl_CentroAziendale.CssClass = "myCombo ComboCentroAziendale"
        Else

            ddl_CentroAziendale.CssClass = "selectpicker ComboCentroAziendale"
            If (_Ricerca = True) Then
                ddl_CentroAziendale.Attributes.Add("data-live-search", "true")
            End If
            ddl_CentroAziendale.Attributes.Add("data-container", "body")
        End If

        'ddl_CentroAziendale.ClientIDMode = UI.ClientIDMode.Static

        Me.Controls.Add(ddl_CentroAziendale)
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
            Return ddl_CentroAziendale.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_CentroAziendale.Items.FindByValue(value)) Then
                    ddl_CentroAziendale.Items.FindByValue(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property Testo_Combo() As String
        Get
            If IsNothing(ddl_CentroAziendale.SelectedItem) Then
                Return ""
            End If
            Return ddl_CentroAziendale.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_CentroAziendale.Items.FindByText(value)) Then
                    ddl_CentroAziendale.Items.FindByText(value).Selected = True
                End If
            End If
        End Set
    End Property

#End Region
 




    Public Sub CaricaComboCentroAziendale(ByVal Operazione_Multicentro As Boolean)

        Dim clc = New AgronicaCoreUtility.CaricaListControl
        If Operazione_Multicentro Then
            clc.Centri_Aziendali(ddl_CentroAziendale,
                                                     True,
                                                     My.Resources.AgronicaControlli_2010.TuttiICentriAziendali, "0",
                                                    _Piva,
                                                    True,
                                                    2,
                                                    "", "",
                                                    HttpContext.Current.Session("ASG_objParametri_Server"))
        Else
            clc.Centri_Aziendali(ddl_CentroAziendale,
                                                            True, My.Resources.AgronicaControlli_2010.SelezionareUnCentroAziendale, "0",
                                                            _Piva,
                                                            True,
                                                            2,
                                                            "", "",
                                                            HttpContext.Current.Session("ASG_objParametri_Server"))
        End If

    End Sub



#Region "JS"
    Public Function GetJS()
        Dim StrSelect As New StringBuilder

        If _bootstrap = False Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" & ddl_CentroAziendale.ClientID & "').combobox();")
            StrSelect.AppendLine("});")
        End If

        Return StrSelect.ToString
    End Function
#End Region

End Class
