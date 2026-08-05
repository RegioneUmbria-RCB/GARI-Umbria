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


<DefaultProperty("Text"), ToolboxData("<{0}:ComboUtenti runat=server></{0}:ComboUtenti>")> Public Class ComboUtenti
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboCentroAziendale
    '    Inherits ScriptControl
    '    Implements IScriptControl

    Public ddl_ComboUtenti As DropDownList


    Private GiaCreato As Boolean

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)


        ddl_ComboUtenti = New DropDownList
        ddl_ComboUtenti.ID = Me.ClientID & "ComboUtenti"
        ddl_ComboUtenti.CssClass = "myCombo"
        'ddl_ComboImprese.ClientIDMode = UI.ClientIDMode.Static

        Me.Controls.Add(ddl_ComboUtenti)
        MyBase.OnInit(e)
    End Sub

#Region "Proprietà"
    
    Public Property Valore_Combo() As String
        Get
            Return ddl_ComboUtenti.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_ComboUtenti.Items.FindByValue(value)) Then
                    ddl_ComboUtenti.Items.FindByValue(value).Selected = True
                End If
            End If
        End Set
    End Property

#End Region






    Public Sub CaricaComboUtenti()

        Dim objUtenti_d As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim dt_Utenti As DataTable = objUtenti_d.Leggi("", 5, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", HttpContext.Current.Session("ASG_objParametri_Utenti"))

        dt_Utenti.Columns.Add("NomeVisualizzato")
        For Each dr As DataRow In dt_Utenti.Rows
            dr("NomeVisualizzato") = If(dr.Item("Flag_Azienda_Persona") = "2", dr.Item("Nome") & " - " & dr.Item("Cognome"), dr.Item("Rag_Soc"))
        Next

        dt_Utenti.DefaultView.Sort = "NomeVisualizzato"
        dt_Utenti = dt_Utenti.DefaultView.ToTable

        ddl_ComboUtenti.Items.Clear()
        ddl_ComboUtenti.Items.Add(New ListItem("Tutti gli Utenti", ""))

        For Each dr As DataRow In dt_Utenti.Rows
            ddl_ComboUtenti.Items.Add(New ListItem(dr.Item("NomeVisualizzato"), dr.Item("CodFisc")))
        Next


    End Sub



#Region "JS"
    Public Function GetJS()
        Dim StrSelect As New StringBuilder

        'StrSelect.AppendLine("<script type='text/javascript'>")
        StrSelect.AppendLine("$(document).ready(function () { ")

        StrSelect.AppendLine("   $('#" & ddl_ComboUtenti.ClientID & "').combobox();")

        StrSelect.AppendLine("});")

        'StrSelect.AppendLine("</script>")
        Return StrSelect.ToString
    End Function
#End Region

End Class
