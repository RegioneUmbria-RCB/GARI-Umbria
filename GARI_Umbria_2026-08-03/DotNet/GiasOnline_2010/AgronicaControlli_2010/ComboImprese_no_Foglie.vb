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


<DefaultProperty("Text"), ToolboxData("<{0}:ComboImprese_no_Foglie runat=server></{0}:ComboImprese_no_Foglie>")> Public Class ComboImprese_no_Foglie
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboCentroAziendale
    '    Inherits ScriptControl
    '    Implements IScriptControl

    Public ddl_ComboImprese As DropDownList
    Private _Piva As String

    Private GiaCreato As Boolean

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)


        ddl_ComboImprese = New DropDownList
        ddl_ComboImprese.ID = Me.ClientID & "ComboImprese"
        ddl_ComboImprese.CssClass = "myCombo"
        'ddl_ComboImprese.ClientIDMode = UI.ClientIDMode.Static

        Me.Controls.Add(ddl_ComboImprese)
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

        'foglia = 0
        Dim objGer As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
        Dim DT As DataTable = objGer.Leggi("", 2, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim hashIm As New Hashtable
        For Each dr As DataRow In DT.Rows
            If Not hashIm.ContainsKey(dr.Item("figlio")) Then
                hashIm.Add(dr.Item("figlio"), dr.Item("figlio"))
            End If
        Next

        AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(ddl_ComboImprese, True, "", "", _
                                                                     "", " ORDER BY Rag_Soc asc", _
                                                                     HttpContext.Current.Session("ASG_objParametri_Server"), _
                                                                     HttpContext.Current.Session("ASG_objParametri_Utenti"))

        'rimuovo quelle che non sono figlie
        For i As Integer = ddl_ComboImprese.Items.Count - 1 To 0 Step -1
            If hashIm.ContainsKey(ddl_ComboImprese.Items(i).Value) = False Then
                If ddl_ComboImprese.Items(i).Value <> "" Then
                    ddl_ComboImprese.Items.RemoveAt(i)
                End If
            End If
        Next

    End Sub



#Region "JS"
    Public Function GetJS()
        Dim StrSelect As New StringBuilder

        'StrSelect.AppendLine("<script type='text/javascript'>")
        StrSelect.AppendLine("$(document).ready(function () { ")

        StrSelect.AppendLine("   $('#" & ddl_ComboImprese.ClientID & "').combobox();")

        StrSelect.AppendLine("});")

        'StrSelect.AppendLine("</script>")
        Return StrSelect.ToString
    End Function
#End Region

End Class
