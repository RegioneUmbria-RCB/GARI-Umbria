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
'''            $("#<%=fooName_PostedBack.ClientID %>").val("comboLavorazioni_IndexChanged");
'''            //scatena il postback simulato il click del btn nascosto
'''            $("#<%=btnDummyPostback.ClientID %>").click();
'''        }
''' </summary>
''' <remarks></remarks>

<DefaultProperty("Text"), ToolboxData("<{0}:ComboFiltriAggiuntiviAgenda runat=server></{0}:ComboFiltriAggiuntiviAgenda>")> Public Class ComboFiltriAggiuntiviAgenda
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboOperazioni
    '    Inherits ScriptControl
    '    Implements IScriptControl

    Public ddl_FiltriAggiuntivi As DropDownList
    Private _Lav_Cod As Integer
    Private _Disciplinare As Boolean
    Private _Bootstrap As Boolean
    Private _Ricerca As Boolean = True


    Public Sub New()
        Bootstrap = False
    End Sub


    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_FiltriAggiuntivi = New DropDownList
        ddl_FiltriAggiuntivi.ID = Me.ClientID & "ComboFiltriAggiuntiviAgenda"
        If _Bootstrap = False Then
            ddl_FiltriAggiuntivi.CssClass = "myCombo ComboFiltriAggiuntiviAgenda"
        Else
            ddl_FiltriAggiuntivi.CssClass = "selectpicker ComboFiltriAggiuntiviAgenda"
            If _Ricerca = False Then
                ddl_FiltriAggiuntivi.Attributes.Add("data-live-search", "true")
            End If

            ddl_FiltriAggiuntivi.Attributes.Add("data-container", "body")
        End If


        Me.Controls.Add(ddl_FiltriAggiuntivi)
        MyBase.OnInit(e)
    End Sub


    Public Property Ricerca() As Boolean
        Get
            Return _Ricerca
        End Get
        Set(ByVal value As Boolean)
            _Ricerca = value
        End Set
    End Property

    Public Property Lav_Cod() As Integer
        Get
            Return _Lav_Cod
        End Get
        Set(ByVal value As Integer)
            _Lav_Cod = value
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
#Region "Proprietà"
    Public Property Disciplinare() As Boolean
        Get
            Return _Disciplinare
        End Get
        Set(ByVal value As Boolean)
            _Disciplinare = value
        End Set
    End Property

    Public Property Valore_Combo() As String
        Get
            Return ddl_FiltriAggiuntivi.SelectedValue
        End Get
        Set(ByVal value As String)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_FiltriAggiuntivi.Items.FindByValue(value)) Then
                    ddl_FiltriAggiuntivi.Items.FindByValue(value).Selected = True
                End If
            End If
        End Set
    End Property

    Public Property Indice_Combo() As Integer
        Get
            Return ddl_FiltriAggiuntivi.SelectedIndex
        End Get
        Set(ByVal value As Integer)
            If Not IsNothing(value) Then
                If Not IsNothing(ddl_FiltriAggiuntivi.Items.Count >= value) Then
                    ddl_FiltriAggiuntivi.SelectedIndex = value
                End If
            End If
        End Set
    End Property

#End Region
     

    Public Sub CaricaComboFiltriAggiuntivi()

        Dim selectvalue As Integer = -1
        If ddl_FiltriAggiuntivi.SelectedValue <> "" Then
            selectvalue = ddl_FiltriAggiuntivi.SelectedValue
        End If
        ddl_FiltriAggiuntivi.Items.Clear()
        Select Case Lav_Cod
            Case LAVCOD_DISTRIBUZIONE_INSETTI
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.AvversitàInsetti, "2"))
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.InsettiAvversità, "1"))
                'il Nessuno Nessuno lo aggiungo solamente se non o selezionato il dpi e se ho i vari permessi
                AggiungiNessuno()

            Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.AvversitàProdotti, "2"))
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.ProdottiAvversità, "1"))
                'il Nessuno Nessuno lo aggiungo solamente se non o selezionato il dpi e se ho i vari permessi
                AggiungiNessuno()

                'ComboFiltriAggiuntiviAgenda.Items.Add(New ListItem("Nessuno", "0"))
            Case LAVCOD_DISERBO
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.InfestantiProdotti, "2"))
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.ProdottiInfestanti, "1"))
                AggiungiNessuno()

            Case LAVCOD_CONCIA_SEME
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.AvversitàProdotti, "2"))
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.ProdottiAvversità, "1"))
                AggiungiNessuno()

            Case LAVCOD_GEODISINFESTAZIONE
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.AvversitàProdotti, "2"))
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.ProdottiAvversità, "1"))
                AggiungiNessuno()

            Case LAVCOD_DISSECCAMENTO
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.InfestantiProdotti, "2"))
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.ProdottiInfestanti, "1"))
                AggiungiNessuno()


            Case LAVCOD_TRATTAMENTO_FITOREGOLATORE
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.Nessuno, "4"))
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.PerClassificazione, "5"))
                ddl_FiltriAggiuntivi.SelectedIndex = 1


            Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.AvversitàInfestantiProdotti, "2"))
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.ProdottiAvversitàInfestanti, "1"))
                'il Nessuno Nessuno lo aggiungo solamente se non o selezionato il dpi e se ho i vari permessi
                AggiungiNessuno()

        End Select


        'controllo se ho quel valore
        Dim i As Integer
        For i = 0 To ddl_FiltriAggiuntivi.Items.Count - 1
            If ddl_FiltriAggiuntivi.Items(i).Value = selectvalue Then
                ddl_FiltriAggiuntivi.Items(i).Selected = True
                Exit For
            End If
        Next


    End Sub

    Private Sub AggiungiNessuno()
        If _Disciplinare = True Then
            'controllo i permessi
            If Not (CBool(HttpContext.Current.Session("Collegamento_DPI"))) Then
                ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.Nessuno, "0"))
            End If
        Else
            ddl_FiltriAggiuntivi.Items.Add(New ListItem(My.Resources.AgronicaControlli_2010.Nessuno, "0"))
        End If
    End Sub


#Region "JS"
    Public Function GetJS()
        Dim StrSelect As New StringBuilder

        If Bootstrap = False Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" & ddl_FiltriAggiuntivi.ClientID & "').combobox();")
            StrSelect.AppendLine("});")
        End If

        Return StrSelect.ToString
    End Function

#End Region
End Class
