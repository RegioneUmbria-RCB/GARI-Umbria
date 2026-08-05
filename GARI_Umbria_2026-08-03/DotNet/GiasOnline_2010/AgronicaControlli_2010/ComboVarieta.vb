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
Imports AgronicaCoreDataProvider

''' <summary>
''' Per utilizzare questo controllo è necessario includere all'iterno della pagina i seguenti script 
''' jquery
''' RICORDARE DI AGGIUNGERE LA FUNZIONE JS
''' function DoPostBack_Combo($_combo,valoreOpt) {
'''        if($_combo.attr("id").endsWith("ComboSpecie")){
'''            //salvo nel campo nascosto il nome della funzione da richiamare
'''            $("#<%=fooName_PostedBack.ClientID %>").val("comboLavorazioni_IndexChanged");
'''            //scatena il postback simulato il click del btn nascosto
'''            $("#<%=btnDummyPostback.ClientID %>").click();
'''        }
''' </summary>
''' <remarks></remarks>
<DefaultProperty("Text"), ToolboxData("<{0}:ComboVarieta runat=server></{0}:ComboVarieta>")> Public Class ComboVarieta
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboSpecie
    '    Inherits ScriptControl
    '    Implements IScriptControl

    Public ddl_Varieta As DropDownList
    Private _Piva As String
    Private _Sa_Cod As Integer
    'Private _Campo_Cod As Integer
    Private _campi As List(Of String)
    Private _Veg_Cod As Integer
    Private _PrimaRiga_Flag As Boolean
    Private _PrimaRiga_Text As String
    Private _PrimaRiga_Value As String
    Private _SoloAziendali As Boolean
    Private _Bootstrap As Boolean
    Private _Ricerca As Boolean = True


    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_Varieta = New DropDownList
        ddl_Varieta.ID = Me.ClientID & "ComboVarieta"
        If _Bootstrap = False Then
            ddl_Varieta.CssClass = "myCombo"
        Else
            ddl_Varieta.CssClass = "selectpicker ComboVarieta"

            If _Ricerca = True Then
                ddl_Varieta.Attributes.Add("data-live-search", "true")
            End If

            ddl_Varieta.Attributes.Add("data-container", "body")
        End If


        Me.Controls.Add(ddl_Varieta)
        MyBase.OnInit(e)
    End Sub

    'Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
    '    CaricaComboSpecie()
    '    MyBase.OnPreRender(e)
    'End Sub

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
    Public Property SoloAziendali() As Boolean
        Get
            Return _SoloAziendali
        End Get
        Set(ByVal value As Boolean)
            _SoloAziendali = value
        End Set
    End Property

    Public Property Campi() As List(Of String)
        Get
            Return _campi
        End Get
        Set(value As List(Of String))
            _campi = value
        End Set
    End Property

    Public Property Valore_Combo() As String
        Get
            Return ddl_Varieta.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Varieta.Items.FindByValue(value)) Then
                    ddl_Varieta.Items.FindByValue(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property Testo_Combo() As String
        Get
            Return ddl_Varieta.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Varieta.Items.FindByText(value)) Then
                    ddl_Varieta.Items.FindByText(value).Selected = True
                End If
            End If
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

    'Public Property Campo_Cod() As Integer
    '    Get
    '        Return _Campo_Cod
    '    End Get
    '    Set(ByVal value As Integer)
    '        _Campo_Cod = value
    '    End Set
    'End Property

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

    Public Property Veg_Cod() As Integer
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal value As Integer)
            _Veg_Cod = value
        End Set
    End Property

#End Region







    Public Sub CaricaComboVarieta(Optional ByVal objParametri_Server As AgronicaCoreParametri = Nothing,
                                 Optional ByVal objParametri_Utenti As AgronicaCoreParametri = Nothing)

        If objParametri_Server Is Nothing AndAlso
                Not HttpContext.Current.Session Is Nothing AndAlso
                Not HttpContext.Current.Session("ASG_objParametri_Server") Is Nothing Then
            objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        End If

        If objParametri_Utenti Is Nothing AndAlso
                Not HttpContext.Current.Session Is Nothing AndAlso
                Not HttpContext.Current.Session("ASG_objParametri_Utenti") Is Nothing Then
            objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
        End If

        If IsNothing(_SoloAziendali) Then
            _SoloAziendali = True
        End If
        If SoloAziendali = True Then
            Dim _Campo_Cod As Integer = 0

            Dim Filtro As String = ""
            If Not IsNothing(_campi) Then
                For i = 0 To _campi.Count - 1
                    Dim sa As Integer = _campi(i).Split("/")(1)
                    Dim ca As Integer = _campi(i).Split("/")(2)

                    Dim FiltroTemp As String = " ( Appezzamento.Sa_Cod = " & sa & " AND Appezzamento.Campo_Cod = " & ca & " ) "
                    If Filtro = "" Then
                        Filtro = FiltroTemp
                    Else
                        Filtro = Filtro & " OR " & FiltroTemp
                    End If

                Next
            End If
            If Filtro <> "" Then
                Filtro = "(" & Filtro & ")"
            End If
            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.CaricaCombo_TutteVarietaColtivate(ddl_Varieta,
                                                                            _PrimaRiga_Flag,
                                                                            _PrimaRiga_Text,
                                                                            _PrimaRiga_Value,
                                                                            _Piva,
                                                                            _Sa_Cod,
                                                                            _Campo_Cod,
                                                                            _Veg_Cod,
                                                                            Filtro, "",
                                                                            objParametri_Server)
        Else
            Dim dt As DataTable
            Dim objVarieta As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            dt = objVarieta.Leggi(0, _Veg_Cod, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                  "", "", objParametri_Server)
            Dim i As Integer
            If _PrimaRiga_Flag = True Then
                ddl_Varieta.Items.Add(New ListItem(_PrimaRiga_Text, _PrimaRiga_Value))
            End If
            For i = 0 To dt.Rows.Count - 1
                ddl_Varieta.Items.Add(New ListItem(dt.Rows(i).Item("Cul_Des"), dt.Rows(i).Item("Cul_Cod")))
            Next

        End If

    End Sub

#Region "JS"

    Public Function GetJS()
        Dim StrSelect As New StringBuilder
        If _Bootstrap = True Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" + ddl_Varieta.ClientID + "').combobox();")
            StrSelect.AppendLine("});")
        End If
        Return StrSelect.ToString
    End Function

#End Region

End Class

<DefaultProperty("Text"), ToolboxData("<{0}:ComboVarieta runat=server></{0}:ComboVarieta>")> Public Class ComboVarietaMulti
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboSpecie
    '    Inherits ScriptControl
    '    Implements IScriptControl

    Public ddl_Varieta As ListBox
    Private _Piva As String
    Private _Sa_Cod As Integer
    'Private _Campo_Cod As Integer
    Private _campi As List(Of String)
    Private _Veg_Cod As Integer
    Private _PrimaRiga_Flag As Boolean
    Private _PrimaRiga_Text As String
    Private _PrimaRiga_Value As String
    Private _SoloAziendali As Boolean
    Private _Bootstrap As Boolean
    Private _Ricerca As Boolean = True

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_Varieta = New ListBox
        ddl_Varieta.SelectionMode = ListSelectionMode.Multiple
        ddl_Varieta.ID = Me.ClientID & "ComboVarieta"


        If _Bootstrap = False Then
            ddl_Varieta.CssClass = "myCombo ComboVarieta"
            ddl_Varieta.Width = "250"
        Else
            ddl_Varieta.CssClass = "selectpicker ComboVarieta"

            If _Ricerca = True Then
                ddl_Varieta.Attributes.Add("data-live-search", "true")
            End If

            ddl_Varieta.Attributes.Add("data-container", "body")
            ddl_Varieta.Attributes.Add("multiple", "true")
        End If

        Me.Controls.Add(ddl_Varieta)
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

    Public Property SoloAziendali() As Boolean
        Get
            Return _SoloAziendali
        End Get
        Set(ByVal value As Boolean)
            _SoloAziendali = value
        End Set
    End Property

    Public Property Campi() As List(Of String)
        Get
            Return _campi
        End Get
        Set(value As List(Of String))
            _campi = value
        End Set
    End Property

    Public ReadOnly Property Valori_Combo As List(Of String)
        Get
            Dim valori As New List(Of String)
            For i = 0 To ddl_Varieta.Items.Count - 1
                If ddl_Varieta.Items(i).Selected Then
                    valori.Add(ddl_Varieta.Items(i).Value)
                End If
            Next
            Return valori
        End Get
    End Property

    'Public Property Testo_Combo() As String
    '    Get
    '        Return ddl_Varieta.SelectedItem.Text
    '    End Get
    '    Set(ByVal value As String)
    '        If Not IsNothing(value) Then
    '            If Not IsNothing(ddl_Varieta.Items.FindByText(value)) Then
    '                ddl_Varieta.Items.FindByText(value).Selected = True
    '            End If
    '        End If
    '    End Set
    'End Property


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

    'Public Property Campo_Cod() As Integer
    '    Get
    '        Return _Campo_Cod
    '    End Get
    '    Set(ByVal value As Integer)
    '        _Campo_Cod = value
    '    End Set
    'End Property

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

    Public Property Veg_Cod() As Integer
        Get
            Return _Veg_Cod
        End Get
        Set(ByVal value As Integer)
            _Veg_Cod = value
        End Set
    End Property

#End Region



    Public Sub CaricaComboVarieta()
        If IsNothing(_SoloAziendali) Then
            _SoloAziendali = True
        End If
        If SoloAziendali = True Then
            Dim _Campo_Cod As Integer = 0

            Dim Filtro As String = ""
            If Not IsNothing(_campi) Then
                For i = 0 To _campi.Count - 1
                    Dim sa As Integer = _campi(i).Split("/")(1)
                    Dim ca As Integer = _campi(i).Split("/")(2)

                    Dim FiltroTemp As String = " ( Appezzamento.Sa_Cod = " & sa & " AND Appezzamento.Campo_Cod = " & ca & " ) "
                    If Filtro = "" Then
                        Filtro = FiltroTemp
                    Else
                        Filtro = Filtro & " OR " & FiltroTemp
                    End If

                Next
            End If
            If Filtro <> "" Then
                Filtro = "(" & Filtro & ")"
            End If
            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.CaricaCombo_TutteVarietaColtivate(ddl_Varieta,
                                                                            _PrimaRiga_Flag,
                                                                            _PrimaRiga_Text,
                                                                            _PrimaRiga_Value,
                                                                            _Piva,
                                                                            _Sa_Cod,
                                                                            _Campo_Cod,
                                                                            _Veg_Cod,
                                                                            Filtro, "",
                                                                            HttpContext.Current.Session("ASG_objParametri_Server"))
        Else
            Dim dt As DataTable
            Dim objVarieta As New AgronicaCoreMetaSchemaDAL.Cultivar_R
            dt = objVarieta.Leggi(0, _Veg_Cod, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                  "", "", HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim i As Integer
            If _PrimaRiga_Flag = True Then
                ddl_Varieta.Items.Add(New ListItem(_PrimaRiga_Text, _PrimaRiga_Value))
            End If
            For i = 0 To dt.Rows.Count - 1
                ddl_Varieta.Items.Add(New ListItem(dt.Rows(i).Item("Cul_Des"), dt.Rows(i).Item("Cul_Cod")))
            Next

        End If

    End Sub

#Region "JS"

    Public Function GetJS2()
        Dim StrSelect As New StringBuilder
        If _Bootstrap = False Then

            'http://wenzhixin.net.cn/p/multiple-select/#the-placeholder
            'StrSelect.AppendLine("<script type='text/javascript'>")
            StrSelect.AppendLine("$(document).ready(function () { ")

            'width: 100,filter: true, 
            Dim Parametro_Filtro As String = "true"
            Dim Parametro_Testo As String = "'Tutte le varietà'" 'placeholder: "Here is the placeholder"
            StrSelect.AppendLine("   $('#" & ddl_Varieta.ClientID & "').multipleSelect({ " & _
                                 "filter: " & Parametro_Filtro & ", " & _
                                 "placeholder: " & Parametro_Testo & ", " & _
                                 "" & _
                                 "" & _
                                 "" & _
                                 "onClose: function(){comboVarietaChiusa();}" & _
                                 "});")

            StrSelect.AppendLine("});")

        End If


        Return StrSelect.ToString
    End Function



#End Region

End Class
