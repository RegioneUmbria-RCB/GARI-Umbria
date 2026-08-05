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
<DefaultProperty("Text"), ToolboxData("<{0}:ComboSpecie_con_Tutte runat=server></{0}:ComboSpecie_con_Tutte>")> Public Class ComboSpecie_con_Tutte
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboSpecie
    '    Inherits ScriptControl
    '    Implements IScriptControl

    Public ddl_Specie As DropDownList
    Private _Piva As String
    Private _Sa_Cod As Integer
    Private _Data As Date
    Private _ConsideraTerrenoNudo As Boolean
    Private _PrimaRiga_Flag As Boolean
    Private _PrimaRiga_Text As String
    Private _PrimaRiga_Value As String
    Private _SoloAziendali As Boolean
    Private _leggiAncheImpiantiBloccati As Boolean = False
    Private _Ricerca As Boolean = True
    Private _Bootstrap As Boolean


    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_Specie = New DropDownList
        ddl_Specie.ID = Me.ClientID & "ComboSpecie"
        ddl_Specie.ClientIDMode = UI.ClientIDMode.Static

        If _Bootstrap = False Then
            ddl_Specie.CssClass = "myCombo ComboSpecie"
        Else
            ddl_Specie.CssClass = "selectpicker ComboSpecie"
            If Ricerca = True Then
                ddl_Specie.Attributes.Add("data-live-search", "true")
            End If
            ddl_Specie.Attributes.Add("data-container", "body")
        End If

        Me.Controls.Add(ddl_Specie)
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


    Public Property leggiAncheImpiantiBloccati() As String
        Get
            Return _leggiAncheImpiantiBloccati
        End Get
        Set(ByVal value As String)
            _leggiAncheImpiantiBloccati = value
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


    Public Property Valore_Combo() As String
        Get
            Return ddl_Specie.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Specie.Items.FindByValue(value)) Then
                    ddl_Specie.Items.FindByValue(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property Testo_Combo() As String
        Get
            Return ddl_Specie.SelectedItem.Text
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_Specie.Items.FindByText(value)) Then
                    ddl_Specie.Items.FindByText(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property ConsideraTerrenoNudo() As Boolean
        Get
            Return _ConsideraTerrenoNudo
        End Get
        Set(ByVal value As Boolean)
            _ConsideraTerrenoNudo = value
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

    Public Property Data() As Date
        Get
            Return _Data
        End Get
        Set(ByVal value As Date)
            _Data = value
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
#End Region


    'Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)


    '    MyBase.OnPreRender(e)
    'End Sub





    Public Sub CaricaComboSpecie()
        If IsNothing(_SoloAziendali) Then
            _SoloAziendali = True
        End If
        If _SoloAziendali = True Then
            If _Data <> New Date Then
                Dim clc = New AgronicaCoreUtility.CaricaListControl
                clc.TutteSpecieColtivate_3(ddl_Specie,
                                                                              _PrimaRiga_Flag,
                                                                              _PrimaRiga_Text,
                                                                              _PrimaRiga_Value,
                                                                              _Piva,
                                                                              _Sa_Cod,
                                                                              _Data,
                                                                              _Data,
                                                                              _ConsideraTerrenoNudo,
                                                                              "", "", HttpContext.Current.Session("ASG_objParametri_Server"), leggiAncheImpiantiBloccati)


            Else
                Dim clc = New AgronicaCoreUtility.CaricaListControl
                clc.TutteSpecieColtivate_SenzaControlloData(ddl_Specie,
                                                                            _PrimaRiga_Flag,
                                                                            _PrimaRiga_Text,
                                                                            _PrimaRiga_Value,
                                                                            _Piva,
                                                                            _Sa_Cod,
                                                                            _ConsideraTerrenoNudo,
                                                                            "", "", HttpContext.Current.Session("ASG_objParametri_Server"), leggiAncheImpiantiBloccati)

            End If

        Else

            ddl_Specie.Items.Clear()

            Dim dt As DataTable
            Dim objspecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            dt = objspecie.Leggi(0, 0, "", "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
                                  "", "", HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim i As Integer
            If _PrimaRiga_Flag = True Then
                ddl_Specie.Items.Add(New ListItem(_PrimaRiga_Text, _PrimaRiga_Value))
            End If
            For i = 0 To dt.Rows.Count - 1
                ddl_Specie.Items.Add(New ListItem(dt.Rows(i).Item("veg_des"), dt.Rows(i).Item("veg_cod")))
            Next

        End If

    End Sub

#Region "JS"

    Public Function GetJS()
        Dim StrSelect As New StringBuilder

        ' StrSelect.AppendLine("<script type='text/javascript'>")

        If _Bootstrap = False Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" + ddl_Specie.ClientID + "').combobox();")
            StrSelect.AppendLine("});")
        End If




        ' StrSelect.AppendLine("</script>")
        Return StrSelect.ToString
    End Function
#End Region

End Class
