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
'''        if($_combo.attr("id").endsWith("ComboFertilizzanti")){
'''            //salvo nel campo nascosto il nome della funzione da richiamare
'''            $("#<%=fooName_PostedBack.ClientID %>").val("ComboFertilizzanti_IndexChanged");
'''            //scatena il postback simulato il click del btn nascosto
'''            $("#<%=btnDummyPostback.ClientID %>").click();
'''        }
''' </summary>
''' <remarks></remarks>

<DefaultProperty("Text"), ToolboxData("<{0}:ComboFaseFenologica runat=server></{0}:ComboFaseFenologica>")> Public Class ComboFaseFenologica
    Inherits System.Web.UI.WebControls.WebControl



    Public ddl_faseFenologica As DropDownList

    Private _TipoFaseFenologica As Integer = 0
    Public Property TipoFaseFenologica As Integer
        Get
            Return _TipoFaseFenologica
        End Get
        Set(ByVal value As Integer)
            _TipoFaseFenologica = value
        End Set
    End Property

    Private _Ricerca As Boolean = True
    Public Property Ricerca As Boolean
        Get
            Return _Ricerca
        End Get
        Set(ByVal value As Boolean)
            _Ricerca = value
        End Set
    End Property


    Private _Bootstrap As Boolean
    Public Property Bootstrap As Boolean
        Get
            Return _Bootstrap
        End Get
        Set(ByVal value As Boolean)
            _Bootstrap = value
        End Set
    End Property


    Public Property selectedValue As String
        Get
            Return ddl_faseFenologica.SelectedValue
        End Get
        Set(ByVal Value As String)
            ddl_faseFenologica.SelectedValue = Value
        End Set
    End Property
    

    Public Sub New()
        _Bootstrap = False
    End Sub

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        ddl_faseFenologica = New DropDownList
        ddl_faseFenologica.ID = Me.ClientID & "ComboFaseFenologica"

        If (_Bootstrap = False) Then
            ddl_faseFenologica.CssClass = "ComboFaseFenologica txtUI"
            ddl_faseFenologica.Attributes.Add("onchange", " CambiaFaseFenologica(); ")
        Else
            ddl_faseFenologica.CssClass = "selectpicker ComboFaseFenologica"
            If _Ricerca = True Then
                ddl_faseFenologica.Attributes.Add("data-live-search", "true")
            End If


            ddl_faseFenologica.Attributes.Add("data-container", "body")
            ddl_faseFenologica.Attributes.Add("onchange", " CambiaFaseFenologica(); ")
        End If


        Me.Controls.Add(ddl_faseFenologica)
        MyBase.OnInit(e)

    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)


        Dim mostra As String = ""
        If (_Bootstrap = False) Then
            If _TipoFaseFenologica = fasiFenologicheTipo.Nascosta Then
                mostra = "Display: none;"
            End If
            ddl_faseFenologica.Attributes.Add("style", " min-width: 525px;width: 90%; " & mostra)
        End If

        MyBase.Render(writer)
    End Sub
     
     


    Public Sub CaricaComboFasiFenologiche(byval vegCod As Integer)

        Dim Dt As DataTable


        ddl_faseFenologica.Items.Clear()


        Dim oLeggiFF As New AgronicaCoreMetaSchemaDAL.FasiFenologichexSpecie_R

        Dim filtro As String = ""

        Dim intestazione As String = "Fase fenlogica"

        If _TipoFaseFenologica = AgronicaCoreDataProvider.TipiEnumerativi.fasiFenologicheTipo.BBCH_Grappolo Then
            filtro = " FasiFenologiche.ff_cod > 30"
            intestazione = "BBCH grap"
        End If

        If _TipoFaseFenologica = AgronicaCoreDataProvider.TipiEnumerativi.fasiFenologicheTipo.BBCH_Foglia Then
            filtro = " FasiFenologiche.ff_cod <= 30"
            intestazione = "BBCH fogl"
        End If
        
        Dt = oLeggiFF.Leggi( _
            0, _
            vegCod, _ 
             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, _
             filtro, _
             "", _
            HttpContext.Current.Session("ASG_objParametri_Server") _
        )


     


        If Not IsNothing(Dt) Then

            Dim dr As DataRow = Dt.NewRow()
            dr("ff_des") = intestazione
            dr("ff_cod") = 0
            dr("progressivo") = -1

            Dt.Rows.InsertAt(dr, 0)

            Dt.TableName = "FaseFenologica"

            ddl_faseFenologica.DataTextField = "ff_des"
            ddl_faseFenologica.DataValueField = "ff_cod"
            ddl_faseFenologica.DataSource = Dt
            ddl_faseFenologica.DataBind()


        End If

        Dt = Nothing


    End Sub


End Class

