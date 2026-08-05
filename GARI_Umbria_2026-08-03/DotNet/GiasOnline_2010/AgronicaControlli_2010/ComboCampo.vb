Imports System.Web.Services
Imports System.Text
Imports System.Data
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.Script.Services


<DefaultProperty("Text"), ToolboxData("<{0}:ComboCampo runat=server></{0}:ComboCampo>")> Public Class ComboCampo
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboCentroAziendale
    '    Inherits ScriptControl
    '    Implements IScriptControl

    Public ddl_Campo As DropDownList
    Private _Piva As String
    Private _Sa_Cod As Integer

    Private _Bootstrap As Boolean

    Private GiaCreato As Boolean
    Private _Ricerca As Boolean = True

    Public Sub New()
        _Bootstrap = False
    End Sub


    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        ddl_Campo = New DropDownList
        ddl_Campo.ID = Me.ClientID & "ComboCampo"
        If Bootstrap = False Then
            ddl_Campo.CssClass = "myCombo ComboCampo"
            ddl_Campo.Width = "250"
        Else
            ddl_Campo.CssClass = "selectpicker ComboCampo"
            If _Ricerca = True Then
                ddl_Campo.Attributes.Add("data-live-search", "true")
            End If

            ddl_Campo.Attributes.Add("data-container", "body")
        End If

        Me.Controls.Add(ddl_Campo)
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

    Public Property Bootstrap() As Boolean
        Get
            Return _Bootstrap
        End Get
        Set(ByVal value As Boolean)
            _Bootstrap = value
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

    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
        End Set
    End Property

    Public Property Valore_Combo() As String
        Get
            Return ddl_Campo.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Campo.Items.FindByValue(value)) Then
                    ddl_Campo.Items.FindByValue(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property Testo_Combo() As String
        Get
            If IsNothing(ddl_Campo.SelectedItem) Then
                Return ""
            End If
            Return ddl_Campo.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Campo.Items.FindByText(value)) Then
                    ddl_Campo.Items.FindByText(value).Selected = True
                End If
            End If
        End Set
    End Property

#End Region






    Public Sub CaricaComboCampo()

        AgronicaCoreUtility.CaricaListControl.Campi(ddl_Campo,
                                                 True, _
                                                  "Tutti i campi", "xxxxxxxxxxx/0/0", False,
                                                  _Piva,
                                                _Sa_Cod, _
                                                0,
                                                "", "",
                                                HttpContext.Current.Session("ASG_objParametri_Server"))


    End Sub



#Region "JS"
    Public Function GetJS()
        Dim StrSelect As New StringBuilder

        'StrSelect.AppendLine("<script type='text/javascript'>")
        StrSelect.AppendLine("$(document).ready(function () { ")
        If Bootstrap = False Then
            StrSelect.AppendLine("   $('#" & ddl_Campo.ClientID & "').combobox();")
        End If

        StrSelect.AppendLine("});")

        'StrSelect.AppendLine("</script>")
        Return StrSelect.ToString
    End Function
#End Region

End Class



<DefaultProperty("Text"), ToolboxData("<{0}:ComboCampo runat=server></{0}:ComboCampo>")> Public Class ComboCampoMulti
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboCentroAziendale
    '    Inherits ScriptControl
    '    Implements IScriptControl

    Public ddl_Campo As ListBox
    Private _Piva As String
    Private _Sa_Cod As Integer
    Private _bootstrap As Boolean
    Private GiaCreato As Boolean

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        ddl_Campo = New ListBox
        ddl_Campo.SelectionMode = ListSelectionMode.Multiple
        ddl_Campo.ID = Me.ClientID & "ComboCampo"
        If bootstrap = False Then
            ddl_Campo.CssClass = "myCombo ComboCampo"
            ddl_Campo.Width = "250"
        Else
            ddl_Campo.CssClass = "selectpicker ComboCampo"
            ddl_Campo.Attributes.Add("data-live-search", "true")
            ddl_Campo.Attributes.Add("data-container", "body")
        End If


        'ddl_Campo.ClientIDMode = UI.ClientIDMode.Static

        Me.Controls.Add(ddl_Campo)
        MyBase.OnInit(e)
    End Sub

#Region "Proprietà"



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

    Public Property Sa_Cod() As Integer
        Get
            Return _Sa_Cod
        End Get
        Set(ByVal value As Integer)
            _Sa_Cod = value
        End Set
    End Property

    Public ReadOnly Property Valori_Combo As List(Of String)
        Get
            Dim valori As New List(Of String)
            For i = 0 To ddl_Campo.Items.Count - 1
                If ddl_Campo.Items(i).Selected Then
                    valori.Add(ddl_Campo.Items(i).Value)
                End If
            Next
            Return valori
        End Get
        'Set(ByVal value As String)
        '    If Not IsNothing(value) Then
        '        If Not IsNothing(ddl_Campo.Items.FindByValue(value)) Then
        '            ddl_Campo.Items.FindByValue(value).Selected = True
        '        End If
        '    End If
        'End Set
    End Property

    'Public Property Testo_Combo() As String
    '    Get
    '        If IsNothing(ddl_Campo.SelectedItem) Then
    '            Return ""
    '        End If
    '        Return ddl_Campo.SelectedItem.Text
    '    End Get
    '    Set(ByVal value As String)
    '        If Not IsNothing(value) Then
    '            If Not IsNothing(ddl_Campo.Items.FindByText(value)) Then
    '                ddl_Campo.Items.FindByText(value).Selected = True
    '            End If
    '        End If
    '    End Set
    'End Property

#End Region






    Public Sub CaricaComboCampo()

        AgronicaCoreUtility.CaricaListControl.Campi(ddl_Campo,
                                                  False, _
                                                 "Tutti i campi", "xxxxxxxxxxx/0/0", True,
                                                _Piva,
                                                _Sa_Cod, _
                                                0,
                                                "", "",
                                                HttpContext.Current.Session("ASG_objParametri_Server"))


    End Sub



#Region "JS"


    Public Function GetJS2()
        Dim StrSelect As New StringBuilder
        'http://wenzhixin.net.cn/p/multiple-select/#the-placeholder
        'StrSelect.AppendLine("<script type='text/javascript'>")


        If bootstrap = False Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            Dim Parametro_Filtro As String = "true"
            Dim Parametro_Testo As String = "'Tutti i Campi'" 'placeholder: "Here is the placeholder"
            StrSelect.AppendLine("   $('#" & ddl_Campo.ClientID & "').multipleSelect({ " & _
                                 "filter: " & Parametro_Filtro & ", " & _
                                 "placeholder: " & Parametro_Testo & ", " & _
                                 "" & _
                                 "" & _
                                 "" & _
                                 "onClose: function(){comboCampoChiusa();}" & _
                                 "});")

            StrSelect.AppendLine("});")
        End If


        Return StrSelect.ToString
    End Function

#End Region

End Class
