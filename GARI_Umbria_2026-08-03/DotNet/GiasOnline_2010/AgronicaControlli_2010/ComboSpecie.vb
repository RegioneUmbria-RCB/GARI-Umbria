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
<DefaultProperty("Text"), ToolboxData("<{0}:ComboSpecie runat=server></{0}:ComboSpecie>")> Public Class ComboSpecie
    Inherits System.Web.UI.WebControls.WebControl

    'Public Class ComboSpecie
    '    Inherits ScriptControl
    '    Implements IScriptControl

    Public ddl_Specie As DropDownList
    Private _Piva As String
    Private _Sa_Cod As Integer
    'Private _Campo_Cod As Integer
    Private _campi As List(Of String)
    Private _Data As Date
    Private _ConsideraTerrenoNudo As Boolean
    Private _dettagliTerrenoNudo As Boolean = False
    Private _PrimaRiga_Flag As Boolean
    Private _PrimaRiga_Text As String
    Private _PrimaRiga_Value As String
    Private _FiltroAggiuntivo As String = ""
    Private _Usa_Filtro_Utente As Boolean = False
    Private _Carica_Tutte_Specie_Esistenti As Boolean = False
    Private _CaricaxPDC As Boolean = True
    Private _Veg_Cod_ModificaLettura As Integer = 0
    'Private _SoloAziendali As Boolean
    Private _leggiAncheImpiantiBloccati As Boolean = False
    Private _Ricerca As Boolean = True

    Private _Bootstrap As Boolean

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_Specie = New DropDownList
        ddl_Specie.ID = Me.ClientID & "ComboSpecie"
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

    Public Property DettagliTerrenoNudo() As Boolean
        Get
            Return _dettagliTerrenoNudo
        End Get
        Set(ByVal value As Boolean)
            _dettagliTerrenoNudo = value
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

    Public Property Campi() As List(Of String)
        Get
            Return _campi
        End Get
        Set(value As List(Of String))
            _campi = value
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
    Public Property Usa_Filtro_Utente() As Boolean
        Get
            Return _Usa_Filtro_Utente
        End Get
        Set(value As Boolean)
            _Usa_Filtro_Utente = value
        End Set
    End Property
    Public Property Carica_Tutte_Specie_Esistenti() As Boolean
        Get
            Return _Carica_Tutte_Specie_Esistenti
        End Get
        Set(value As Boolean)
            _Carica_Tutte_Specie_Esistenti = value
        End Set
    End Property

    Public Property CaricaxPDC() As Boolean
        Get
            Return _CaricaxPDC
        End Get
        Set(value As Boolean)
            _CaricaxPDC = value
        End Set
    End Property

    Public Property FiltroAggiuntivo() As String
        Get
            Return _FiltroAggiuntivo
        End Get
        Set(value As String)
            _FiltroAggiuntivo = value
        End Set
    End Property
    Public Property Veg_Cod_ModificaLettura() As Integer
        Get
            Return _Veg_Cod_ModificaLettura
        End Get
        Set(value As Integer)
            _Veg_Cod_ModificaLettura = value
        End Set
    End Property
#End Region



    'carica le specie nella combo utilizzando anche il filtro utente se presente e scegliendo in base ai parametri se
    'caricare tutte le specie o solo quelle dell'azienda/centro
    'Se apro una operazione in modifica o lettura allora posso specificare la specie nel parametro Veg_Cod_ModificaLettura
    ' in modo tale che anche se questaè filtrata viene comunque aggiunta alla combo
    Public Sub CaricaComboSpecie(Optional ByVal objParametri_Server As AgronicaCoreParametri = Nothing,
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

        If Carica_Tutte_Specie_Esistenti Then

            If _CaricaxPDC = True Then
                AgronicaCoreUtility.CaricaListControl.SpecieVegetale_Optimize_x_PDC(ddl_Specie,
                                                     _PrimaRiga_Flag,
                                                     _PrimaRiga_Text,
                                                     _PrimaRiga_Value,
                                                     objParametri_Server)
            Else
                AgronicaCoreUtility.CaricaListControl.SpecieVegetale_Optimize(ddl_Specie,
                                                           _PrimaRiga_Flag,
                                                           _PrimaRiga_Text,
                                                           _PrimaRiga_Value,
                                                           0,
                                                           "",
                                                           Usa_Filtro_Utente,
                                                           0,
                                                            Veg_Cod_ModificaLettura, 0, FiltroAggiuntivo, "", objParametri_Server, objParametri_Utenti)

            End If



        Else

            Dim _Campo_Cod As Integer = 0

            Dim FiltroCampi As String = ""
            If Not IsNothing(_campi) Then
                For i = 0 To _campi.Count - 1
                    Dim sa As Integer = _campi(i).Split("/")(1)
                    Dim ca As Integer = _campi(i).Split("/")(2)

                    Dim FiltroTemp As String = " ( Appezzamento.Sa_Cod = " & sa & " AND Appezzamento.Campo_Cod = " & ca & " ) "
                    If FiltroCampi = "" Then
                        FiltroCampi = FiltroTemp
                    Else
                        FiltroCampi = FiltroCampi & " OR " & FiltroTemp
                    End If

                Next
            End If
            If FiltroCampi <> "" Then
                FiltroCampi = "(" & FiltroCampi & ")"
            End If

            ''Dim filtroFinale As String = FiltroAggiuntivo
            ''If Filtro <> "" Then
            ''    If filtroFinale <> "" Then
            ''        filtroFinale = filtroFinale & " AND " & Filtro
            ''    Else
            ''        filtroFinale = Filtro
            ''    End If
            ''End If
            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.SpecieVegetale_Coltivate_Optimize(ddl_Specie,
                                                               _Piva,
                                                               _Sa_Cod,
                                                               _Data,
                                                               _ConsideraTerrenoNudo,
                                                               _PrimaRiga_Flag,
                                                               _PrimaRiga_Text,
                                                               _PrimaRiga_Value,
                                                                0,
                                                                  "",
                                                               Usa_Filtro_Utente,
                                                                0,
                                                                 Veg_Cod_ModificaLettura, 0, _dettagliTerrenoNudo,
                                                                 FiltroAggiuntivo,
                                                                  "",
                                                                 objParametri_Server, objParametri_Utenti,
                                                                 leggiAncheImpiantiBloccati,
                                                                 _Campo_Cod, FiltroCampi)


        End If



    End Sub


#Region "JS"

    Public Function GetJS()
        Dim StrSelect As New StringBuilder


        If (Bootstrap = False) Then
            StrSelect.AppendLine("$(document).ready(function () { ")
            StrSelect.AppendLine("   $('#" + ddl_Specie.ClientID + "').combobox();")
            StrSelect.AppendLine("});")
        End If




        Return StrSelect.ToString
    End Function
#End Region

End Class
