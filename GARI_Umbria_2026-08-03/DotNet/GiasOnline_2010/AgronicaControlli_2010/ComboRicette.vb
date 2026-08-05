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


<DefaultProperty("Text"), ToolboxData("<{0}:ComboRicette runat=server></{0}:ComboRicette>")> Public Class ComboRicette
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboRicette
    '    Inherits ScriptControl
    '    Implements IScriptControl

    Public ddl_Ricette As DropDownList
    Private _Piva As String

    Private GiaCreato As Boolean

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)


        ddl_Ricette = New DropDownList
        ddl_Ricette.ID = Me.ClientID & "ComboRicette"
        ddl_Ricette.CssClass = "myCombo ComboRicette"
        'ddl_Ricette.ClientIDMode = UI.ClientIDMode.Static

        Me.Controls.Add(ddl_Ricette)
        MyBase.OnInit(e)
    End Sub

#Region "Proprietà"
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
            Return ddl_Ricette.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Ricette.Items.FindByValue(value)) Then
                    ddl_Ricette.Items.FindByValue(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property Testo_Combo() As String
        Get
            If IsNothing(ddl_Ricette.SelectedItem) Then
                Return ""
            End If
            Return ddl_Ricette.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Ricette.Items.FindByText(value)) Then
                    ddl_Ricette.Items.FindByText(value).Selected = True
                End If
            End If
        End Set
    End Property

#End Region

    'Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)

    '    CaricaComboRicette()
    '    MyBase.OnPreRender(e)
    'End Sub



    ' ''' <summary>
    ' ''' Per Renderizzare il controllo
    ' ''' </summary>
    ' ''' <param name="writer"></param>
    ' ''' <remarks></remarks>
    'Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

    '    writer.Write(GetJS())
    '    MyBase.Render(writer)

    'End Sub




    Public Sub CaricaComboRicette(ByVal Operazione_Multicentro As Boolean)

        Dim clc = New AgronicaCoreUtility.CaricaListControl
        If Operazione_Multicentro Then
            clc.Centri_Aziendali(ddl_Ricette,
                                                     True, "Tutti i Centri Aziendali", "0",
                                                    _Piva,
                                                    True,
                                                    2,
                                                    "", "",
                                                    HttpContext.Current.Session("ASG_objParametri_Server"))
        Else
            clc.Centri_Aziendali(ddl_Ricette,
                                                            True, "Selezionare un Centro Aziendale", "0",
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

        'StrSelect.AppendLine("<script type='text/javascript'>")
        StrSelect.AppendLine("$(document).ready(function () { ")

        StrSelect.AppendLine("   $('#" & ddl_Ricette.ClientID & "').combobox();")

        StrSelect.AppendLine("});")

        'StrSelect.AppendLine("</script>")
        Return StrSelect.ToString
    End Function
#End Region

End Class