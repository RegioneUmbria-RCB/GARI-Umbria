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
'''        if($_combo.attr("id").endsWith("ddlLavorazioni")){
'''            //salvo nel campo nascosto il nome della funzione da richiamare
'''            $("#<%=fooName_PostedBack.ClientID %>").val("comboLavorazioni_IndexChanged");
'''            //scatena il postback simulato il click del btn nascosto
'''            $("#<%=btnDummyPostback.ClientID %>").click();
'''        }
''' </summary>
''' <remarks></remarks>

<DefaultProperty("Text"), ToolboxData("<{0}:ComboEpocheFertilizzazione runat=server></{0}:ComboEpocheFertilizzazione>")> Public Class ComboEpocheFertilizzazione
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboOperazioni
    '    Inherits ScriptControl
    '    Implements IScriptControl
    Private _bootstrap As Boolean
    Public ddl_ComboEpocheFertilizzazione As DropDownList

    Private _Veg_Cod As Integer
    Private _Regolamento_Cod As Integer
    Private _PrimaRiga_Flag As Boolean
    Private _PrimaRiga_Text As String
    Private _PrimaRiga_Value As String
    Private _Ricerca As Boolean = True

    Public Sub New()
        _bootstrap = False
    End Sub

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_ComboEpocheFertilizzazione = New DropDownList
        ddl_ComboEpocheFertilizzazione.ID = Me.ClientID & "ComboEpocheFertilizzazione"
        If (_bootstrap = False) Then
            ddl_ComboEpocheFertilizzazione.CssClass = "myCombo ComboEpocheFertilizzazione"
        Else
            ddl_ComboEpocheFertilizzazione.CssClass = "selectpicker ComboEpocheFertilizzazione"

            If _Ricerca = True Then
                ddl_ComboEpocheFertilizzazione.Attributes.Add("data-live-search", "true")
            End If

            ddl_ComboEpocheFertilizzazione.Attributes.Add("data-container", "body")
        End If
         
        Me.Controls.Add(ddl_ComboEpocheFertilizzazione)
        MyBase.OnInit(e)
    End Sub

#Region "Proprietà"
    Public Property Ricerca As Boolean
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

    Public Property Veg_Cod() As Integer
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal value As Integer)
            _Veg_Cod = value
        End Set
    End Property

    Public Property PrimaRiga_Flag() As Boolean
        Get
            Return _PrimaRiga_Flag
        End Get
        Set(ByVal value As Boolean)
            _PrimaRiga_Flag = value
        End Set
    End Property

    Public Property PrimaRiga_Text() As String
        Get
            Return _PrimaRiga_Text
        End Get
        Set(ByVal value As String)
            _PrimaRiga_Text = value
        End Set
    End Property

    Public Property PrimaRiga_Value() As String
        Get
            Return _PrimaRiga_Value
        End Get
        Set(ByVal value As String)
            _PrimaRiga_Value = value
        End Set
    End Property

    Public Property Regolamento_Cod() As Integer
        Get
            Return _Regolamento_Cod
        End Get
        Set(ByVal value As Integer)
            _Regolamento_Cod = value
        End Set
    End Property

    Public Property Valore_Combo() As String
        Get
            Return ddl_ComboEpocheFertilizzazione.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_ComboEpocheFertilizzazione.Items.FindByValue(value)) Then
                    ddl_ComboEpocheFertilizzazione.SelectedIndex = ddl_ComboEpocheFertilizzazione.Items.IndexOf(ddl_ComboEpocheFertilizzazione.Items.FindByValue(value))
                End If
            End If
        End Set
    End Property


    Public Property Testo_Combo() As String
        Get
            Return ddl_ComboEpocheFertilizzazione.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_ComboEpocheFertilizzazione.Items.FindByText(value)) Then
                    ddl_ComboEpocheFertilizzazione.SelectedIndex = ddl_ComboEpocheFertilizzazione.Items.IndexOf(ddl_ComboEpocheFertilizzazione.Items.FindByText(value))
                End If
            End If
        End Set
    End Property

#End Region




    Public Sub CaricaComboEpocheFertilizzazione()

        ddl_ComboEpocheFertilizzazione.Items.Clear()

        If _PrimaRiga_Flag = True Then
            ddl_ComboEpocheFertilizzazione.Items.Add(New ListItem(_PrimaRiga_Text, _PrimaRiga_Value))
        End If

        Dim objEpoche As New AgronicaCoreMetaSchemaDAL.EpocheModalita_R
        Dim Dt As DataTable
        Dim i As Integer

        Dt = objEpoche.LeggixSpecie(0, 0,
                                    _Veg_Cod,
                                    "", "",
                                    HttpContext.Current.Session("ASG_objParametri_Server"))

        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then
            For i = 0 To Dt.Rows.Count - 1
                ddl_ComboEpocheFertilizzazione.Items.Add(New ListItem(Dt.Rows(i).Item("EM_Des"),
                                                                      Dt.Rows(i).Item("EM_Cod")))
            Next
        End If

        Dt = Nothing

    End Sub


    Public Sub CaricaComboEpocheFertilizzazionePua()

        ddl_ComboEpocheFertilizzazione.Items.Clear()

        'If _PrimaRiga_Flag = True Then
        '    ddl_ComboEpocheFertilizzazione.Items.Add(New ListItem(_PrimaRiga_Text, _PrimaRiga_Value))
        'End If

        Try

            ListControl_PianoConcimazione_WS.PUA_EpocheModalita_WS(ddl_ComboEpocheFertilizzazione,
                                                                   _PrimaRiga_Flag, _PrimaRiga_Text, _PrimaRiga_Value,
                                                                   _Regolamento_Cod, _Veg_Cod,
                                                                    "", "")

        Catch ex As Exception


        End Try


    End Sub




#Region "JS"
    Public Function GetJS()
        Dim StrSelect As New StringBuilder

        If bootstrap = False Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" & ddl_ComboEpocheFertilizzazione.ClientID & "').combobox();")
            StrSelect.AppendLine("});")
        End If


        Return StrSelect.ToString
    End Function
#End Region

End Class
